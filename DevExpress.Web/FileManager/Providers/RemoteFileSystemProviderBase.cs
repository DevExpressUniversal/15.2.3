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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
namespace DevExpress.Web {
	public abstract class CloudFileSystemProviderBase : FileSystemProviderBase {
		Dictionary<string, string> contentTypeRegistry;
		Dictionary<string, string> ContentTypeRegistry {
			get {
				if(this.contentTypeRegistry == null)
					this.contentTypeRegistry = new Dictionary<string, string>();
				return this.contentTypeRegistry;
			}
		}
		public CloudFileSystemProviderBase(string rootFolder)
			: base(rootFolder) { }
		protected const string PathSeparator = "/";
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Stream ReadFile(FileManagerFile file) { return null; }
		public override Stream GetThumbnail(FileManagerFile file) {
			return GetFileStream(file);
		}
		internal Stream GetFileStream(FileManagerFile file) {
			return GetFileStream(file, GetFileResponse);
		}
		internal Stream GetFileStream(FileManagerFile file, GetFileResponseDelegate getFileResponse) {
			Stream stream = new MemoryStream();
			try {
				using(var resp = getFileResponse(file)) {
					if(resp == null) {
						stream.Dispose();
						return null;
					}
					using(Stream respStream = resp.GetResponseStream())
						respStream.CopyTo(stream);
				}
				return stream;
			} catch {
				stream.Dispose();
				return null;
			}
		}
		internal delegate WebResponse GetFileResponseDelegate(FileManagerFile file);
		public virtual string GetDownloadUrl(FileManagerFile[] files) {
			return string.Empty;
		}
		protected virtual WebResponse GetFileResponse(FileManagerFile file) {
			return null;
		}
		protected virtual string PreparePath(string path) {
			return path.Replace("\\", PathSeparator).TrimStart('~').TrimEnd('/');
		}
		public event RequestEventHandler RequestEvent;
		public delegate void RequestEventHandler(FileManagerCloudProviderRequestEventArgs e);
		protected void RaiseRequestEvent(FileManagerCloudProviderRequestEventArgs e) {
			if(RequestEvent != null)
				RequestEvent(e);
		}
		protected internal void RegisterContentType(string extension, string contentType) {
			if(!ContentTypeRegistry.ContainsKey(extension))
				ContentTypeRegistry.Add(extension, contentType);
		}
		protected string GetContentType(string extension) {
			string contentType;
			if(ContentTypeRegistry.TryGetValue(extension, out contentType))
				return contentType;
			return null;
		}
	}
}
