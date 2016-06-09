#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using DevExpress.Utils;
namespace DevExpress.Office.Services {
	[ComVisible(true)]
	public interface IUriStreamService {
		Stream GetStream(string url);
		void RegisterProvider(IUriStreamProvider provider);
		void UnregisterProvider(IUriStreamProvider provider);
	}
	[ComVisible(true)]
	public interface IUriStreamProvider {
		Stream GetStream(string uri);
	}
}
namespace DevExpress.Office.Services.Implementation {
	#region UriStreamProviderCollection
	[ComVisible(true)]
	public class UriStreamProviderCollection : List<IUriStreamProvider> {
	}
	#endregion
	#region UriStreamService
	[ComVisible(true)]
	public class UriStreamService : IUriStreamService {
		UriStreamProviderCollection providers;
		public UriStreamService() {
			this.providers = new UriStreamProviderCollection();
			RegisterDefaultProviders();
		}
		public UriStreamProviderCollection Providers { get { return providers; } }
		public Stream GetStream(string uri) {
			if (String.IsNullOrEmpty(uri))
				return null;
			int count = Providers.Count;
			for (int i = 0; i < count; i++) {
				try {
					Stream result = Providers[i].GetStream(uri);
					if (result != null)
						return result;
				}
				catch {
				}
			}
			return null;
		}
		public void RegisterProvider(IUriStreamProvider provider) {
			if (provider == null)
				return;
			Providers.Insert(0, provider);
		}
		public void UnregisterProvider(IUriStreamProvider provider) {
			if (provider == null)
				return;
			int index = Providers.IndexOf(provider);
			if (index >= 0)
				Providers.RemoveAt(index);
		}
		protected internal virtual void RegisterDefaultProviders() {
			RegisterProvider(new WebUriStreamProvider());
		}
	}
	#endregion
#if DXPORTABLE
	#region WebUriStreamProvider
	[ComVisible(true)]
	public class WebUriStreamProvider : IUriStreamProvider {
		#region IUriStreamProvider Members
		public Stream GetStream(string uri) {
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			System.Threading.Tasks.Task<byte[]> task = client.GetByteArrayAsync(uri);
			task.Wait(30000);
			if (task.IsCompleted && task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				return new MemoryStream(task.Result);
			else
				return null;
		}
	#endregion
	}
	#endregion
#else
	#region WebUriStreamProvider
	[ComVisible(true)]
	public class WebUriStreamProvider : IUriStreamProvider {
		#region IUriStreamProvider Members
		public Stream GetStream(string uri) {
			WebRequest request = CreateWebRequest(uri);
			WebUriStreamCreator creator = new WebUriStreamCreator(request);
			return creator.CreateStream();
		}
		#endregion
		protected virtual WebRequest CreateWebRequest(string uri) {
#if SL
			try {
				return WebRequest.CreateHttp(uri);
			}
			catch {
			}
#endif
			return WebRequest.Create(uri);
		}
	}
	#endregion
	#region WebUriStreamCreator
	public class WebUriStreamCreator {
		#region Fields
		WebRequest request;
		MemoryStream stream;
		ManualResetEvent complete;
		byte[] buffer;
		Stream responseStream;
		long responseContentLength;
		#endregion
		public WebUriStreamCreator(WebRequest request) {
			this.request = request;
		}		
		public Stream CreateStream() {
			try {
				this.complete = new ManualResetEvent(false);
				IAsyncResult asyncResult = request.BeginGetResponse(new AsyncCallback(ResponseCallback), null);
#if !SL
				RegisteredWaitHandle handle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(OnWaitTimeout), null, 30000, true);
				complete.WaitOne();
				handle.Unregister(asyncResult.AsyncWaitHandle);
#else
				complete.WaitOne(30000);
#endif
				complete.Close();
				return stream;
			}
			catch {
				return null;
			}
		}
		protected internal virtual void ResponseCallback(IAsyncResult result) {
			try {
				WebResponse response = request.EndGetResponse(result);
				this.responseContentLength = response.ContentLength;
				this.responseStream = response.GetResponseStream();
				this.buffer = new byte[4096];
				this.stream = new MemoryStream();
				responseStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnChunkRead), null);
			}
			catch {
				try {
					complete.Set();
				}
				catch {
				}
			}
		}
		protected internal virtual void OnChunkRead(IAsyncResult result) {
			try {
				int bytesRead = responseStream.EndRead(result);
				if (bytesRead <= 0) {
					responseStream.Close();
					responseStream = null;
					stream.Seek(0, SeekOrigin.Begin);
					complete.Set();
				}
				else {
					stream.Write(buffer, 0, bytesRead);
					if ((responseContentLength > 0 && stream.Length >= responseContentLength)) {
						responseStream.Close();
						responseStream = null;
						stream.Seek(0, SeekOrigin.Begin);
						complete.Set();
					}
					else
						responseStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnChunkRead), null);
				}
			}
			catch {
				try {
					complete.Set();
				}
				catch {
				}
			}
		}
		protected internal virtual void OnWaitTimeout(object state, bool timedOut) {
			try {
				if (timedOut) {
					request.Abort();
					complete.Set();
				}
			}
			catch {
			}
		}
	}
	#endregion
#endif
	#region DataStringUriStreamProvider
	[ComVisible(true)]
	public class DataStringUriStreamProvider : IUriStreamProvider {
		public bool IsUriSupported(string uri) {
			if (uri.StartsWithInvariantCultureIgnoreCase("data:")) {
				System.Text.RegularExpressions.Match match = DataStringUriPattern.regex.Match(uri);
				if (StringExtensions.CompareInvariantCultureIgnoreCase(match.Groups["capacity"].Value, "base64") == 0) {
					return true;
				}
			}
			return false;
		}
		#region IUriStreamProvider Members
		public Stream GetStream(string uri) {
			if (uri.StartsWithInvariantCultureIgnoreCase("data:")) {
				System.Text.RegularExpressions.Match match = DataStringUriPattern.regex.Match(uri);
				if (StringExtensions.CompareInvariantCultureIgnoreCase(match.Groups["capacity"].Value, "base64") == 0) {
					string content = match.Groups["image"].Value;
					byte[] buffer;
					try {
						buffer = Convert.FromBase64String(content);
					}
					catch {
						content = Uri.UnescapeDataString(content);
						buffer = Convert.FromBase64String(content);
					}
					return new MemoryStream(buffer);
				}
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region DataStringUriPattern
	public static class DataStringUriPattern {
		const string imageTypePattern = @"(?<imageType>\w*)";
		const string capacityPattern = @"(?<capacity>\w+)";
		const string imagePattern = @"(?<image>.*)";
		const string spacePattern = @"\s*";
		const string slashPattern = @"\/";
		static string pattern = String.Format(@"{0}?data:{0}?image{0}?{4}?{0}?{1}?{0}?;{0}?{2}{0}?,{0}?{3}{0}?", spacePattern, imageTypePattern, capacityPattern, imagePattern, slashPattern);
		public static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Singleline);
	}
	#endregion
} 
