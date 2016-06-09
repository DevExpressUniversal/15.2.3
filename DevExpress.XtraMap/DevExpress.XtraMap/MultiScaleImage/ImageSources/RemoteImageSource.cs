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
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap.Native {
	public class RemoteImageSource : TileImageSource {
		readonly Uri uri;
		readonly object disposeLocker = new object();
		IntSize size;
		HttpWebRequest httpRequest;
		protected internal string Referer { get; set; }
		public Uri Uri {
			get {
				return uri;
			}
		}
		public event MapWebRequestEventHandler WebRequest;
		public RemoteImageSource(TileIndex index, Uri uri, IntSize expectedSize) : base(index) {
			this.uri = uri;
			this.size = expectedSize;
		}
		public RemoteImageSource(TileIndex index, Uri uri, IntSize expectedSize, string referer)
			: base(index) {
			this.uri = uri;
			this.size = expectedSize;
			Referer = referer;
		}
		protected override void DisposeOverride() {
			lock(disposeLocker) {
				base.DisposeOverride();
			}
		}
		bool CheckHeaders(WebResponse response) {
			if (response.Headers.ToString().Contains("X-VE-Tile-Info"))
				foreach (string value in response.Headers.GetValues("X-VE-Tile-Info"))
					if (value.ToLower() == "no-tile")
						return false;
			return true;
		}
		HttpWebRequest CreateNewRequest() {
			HttpWebRequest request = HttpWebRequest.Create(Uri) as HttpWebRequest;
			if(request == null) 
				return null;
			if(!string.IsNullOrEmpty(Referer))
				request.Referer = Referer;
			request.Accept = "image/*";
			request.KeepAlive = true;
			return request;
		}
		protected override void DoLoad() {
			if(IsDisposed)
				return;
			HttpWebRequest newRequest = CreateNewRequest();
			if(newRequest == null) return;
			RaiseWebRequest(newRequest);
			this.httpRequest = newRequest;
			SetStatusSafe(TileStatus.Loading);
			ThreadPool.QueueUserWorkItem(new WaitCallback(LoadImageInBackground));
		}
		void RaiseWebRequest(HttpWebRequest request) {
			if(WebRequest != null)
				WebRequest(this, new MapWebRequestEventArgs(uri, request));
		}
		Image LoadImageFromStream(Stream stream) {
			Image result = null;
			if (stream != null) {
				try {
					result = Image.FromStream(stream, false, false);
				} finally {
					stream.Close();
				}
			}
			return result;
		}
		void LoadImageInBackground(object target) {
			lock(disposeLocker) {
				if(IsDisposed)
					return;
				HttpWebResponse response = null;
				Stream stream = null;
				try {
					if(httpRequest != null) 
						response = (HttpWebResponse)httpRequest.GetResponse();
					if(response != null && CheckHeaders(response)) {
						stream = response.GetResponseStream();
						byte[] rawImage = null;
						Image img = LoadImageFromStream(stream);
						using(MemoryStream ms = new MemoryStream()) {
							img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
							rawImage = ms.ToArray();
						}
						Source = BitmapUtils.ToArgbBitmap(img as Bitmap, true);
						if((Status == TileStatus.Loading))
							RaiseImageLoaded(rawImage);
					}
					else
						RaiseImageFailed(new Exception("no tile"));
				}
				catch(Exception exception) {
					RaiseImageFailed(exception);
				}
				finally {
					if(response != null) {
						response.Close();
						response = null;
					}
					httpRequest = null;
				}
			}
		}
		protected override void DoCancel() {
			try {
				SetStatusSafe(TileStatus.Cancelling);
					if(httpRequest != null)
						httpRequest.Abort();
				RaiseImageCanceled();
			} catch {
			}
		}
		public override int GetMemoryUsage() {
			return size.Width * size.Height * bytesPerPixel;
		}
	}
}
