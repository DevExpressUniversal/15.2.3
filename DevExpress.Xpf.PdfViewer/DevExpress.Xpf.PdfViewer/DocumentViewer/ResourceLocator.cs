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
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Resources;
using DevExpress.Utils;
namespace DevExpress.Xpf.PdfViewer.Internal {
	public class ResourceLoadedEventArgs : EventArgs {
		public bool Loaded { get; private set; }
		public Stream ResourceStream { get; private set; }
		public ResourceLoadedEventArgs(bool loaded, Stream stream = null) {
			Loaded = loaded;
			ResourceStream = stream;
		}
	}
	public class ResourceLoadingProgressEventArgs : EventArgs {
		public long TotalBytes { get; private set; }
		public long BytesRecieved { get; private set; }
		public ResourceLoadingProgressEventArgs(long recieved, long total) {
			TotalBytes = total;
			BytesRecieved = recieved;
		}
	}
	public class ResourceLocator {
		public event EventHandler<ResourceLoadedEventArgs> Loaded;
		public event EventHandler<ResourceLoadingProgressEventArgs> LoadingProgressChanged;
		Stream resourceStream;
		readonly WebClient webClient;
		readonly Uri uri;
		public ResourceLocator(Uri uri) {
			Guard.ArgumentNotNull(uri, "uri");
			this.uri = uri;
			webClient = new WebClient();
			webClient.DownloadProgressChanged += OnWebClientDownloadProgressChanged;
			webClient.DownloadDataCompleted += OnWebClientDownloadDataCompleted;
		}
		void OnWebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			if(LoadingProgressChanged != null)
				LoadingProgressChanged(this, new ResourceLoadingProgressEventArgs(e.BytesReceived, e.TotalBytesToReceive));
		}
		void OnWebClientDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
			if (e.Error == null)
				this.resourceStream = new MemoryStream(e.Result);
			RaiseLoaded(e.Error == null);
		}
		void RaiseLoaded(bool loaded) {
			if (Loaded != null)
				Loaded(this, new ResourceLoadedEventArgs(loaded, resourceStream));
		}
		public void LoadStream() {
			if (TryGetStreamFromApp()) {
				RaiseLoaded(true);
				return;
			}
			LoadStreamAsync();
		}
		public Stream LoadStreamSync() {
			return new MemoryStream(webClient.DownloadData(uri));
		}
		bool TryGetStreamFromApp() {
			StreamResourceInfo streamInfo;
			try {
				streamInfo = Application.GetResourceStream(uri);
			}
			catch {
				return false;
			}
			if (streamInfo == null) 
				return false;
			resourceStream = streamInfo.Stream;
			return true;
		}
		void LoadStreamAsync() {
			webClient.DownloadDataAsync(uri);
		}
		public void CancelLoadStream() {
			webClient.CancelAsync();
		}
	}
}
