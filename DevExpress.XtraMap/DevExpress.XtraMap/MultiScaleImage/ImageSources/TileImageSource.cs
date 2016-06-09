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
namespace DevExpress.XtraMap {
	public delegate void MapWebRequestEventHandler(object sender, MapWebRequestEventArgs e);
	public class MapWebRequestEventArgs : EventArgs {
		readonly Uri uri;
		readonly HttpWebRequest client;
		public Uri Uri {
			get {
				return uri;
			}
		}
		public ICredentials Credentials {
			get {
				return client.Credentials;
			}
			set {
				client.Credentials = value;
			}
		}
		public bool UseDefaultCredentials {
			get {
				return client.UseDefaultCredentials;
			}
			set {
				client.UseDefaultCredentials = value;
			}
		}
		public WebHeaderCollection Headers {
			get {
				return client.Headers;
			}
			set {
				client.Headers = value;
			}
		}
		public IWebProxy Proxy {
			get {
				return client.Proxy;
			}
			set {
				client.Proxy = value;
			}
		}
		public string UserAgent {
			get {
				return client.UserAgent;
			}
			set {
				client.UserAgent = value;
			}
		}
		public MapWebRequestEventArgs(Uri uri, HttpWebRequest client) {
			this.uri = uri;
			this.client = client;
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class TileImageFailedEventArgs : EventArgs {
		readonly Exception exception;
		public Exception Exception {
			get {
				return exception;
			}
		}
		public TileImageFailedEventArgs(Exception exception) {
			this.exception = exception;
		}
	}
	public enum TileStatus {
		NotReady,
		Ready,
		Loading,
		Cancelling,
		Cancelled
	}
	public abstract class TileImageSource : MapDisposableObject {
		public const int bytesPerPixel = 4;
		const int maxLoadFailedAttempts = 5;
		readonly object statusLocker;
		readonly object disposeLocker;
		TileStatus status;
		TileIndex index;
		Image source;
		DateTime timeStamp;
		bool imageLoadFailed = false;
		int loadFailedAttempts;
		protected object DisposeLocker { get { return disposeLocker; } }
		protected internal bool ImageLoadFailed { get { return imageLoadFailed; } }
		public TileStatus Status { get { return status; } }
		public DateTime TimeStamp { get { return timeStamp; } set { timeStamp = value; } }
		public TileIndex Index { get { return index; } }
		public Image Source { get { return source; } protected set { source = value; } }
		public TileImageSourceDownloadStack DownloadStack { get; set; }
		public bool CanDelete { get { return this.loadFailedAttempts > maxLoadFailedAttempts; } }
		public event EventHandler ImageDisposed;
		public event EventHandler ImageLoaded;
		public event EventHandler<TileImageFailedEventArgs> ImageFailed;
		public event EventHandler ImageCancelled;
		protected TileImageSource(TileIndex index) {
			this.timeStamp = DateTime.Now;
			this.status = TileStatus.NotReady;
			this.source = null;
			this.statusLocker = new object();
			this.disposeLocker = new object();
			this.index = index;
		}
		protected override void DisposeOverride() {
		   lock(DisposeLocker) {
				if(source != null) {
					source.Dispose();
					source = null;
					if(ImageDisposed != null)
						ImageDisposed(this, new EventArgs());
				}
			}
		}
		protected void RaiseImageLoaded(byte[] rawData) {
			SetStatusSafe(TileStatus.Ready);
			lock(DisposeLocker) {
				if(DownloadStack != null)
					DownloadStack.NotifySourceSuccess(this, rawData);
			}
			if(ImageLoaded != null)
				ImageLoaded(this, new EventArgs());
		}
		protected void RaiseImageFailed(Exception exception) {
			SetStatusSafe(TileStatus.NotReady);
			lock(DisposeLocker) {
				loadFailedAttempts++;
				imageLoadFailed = true;
				if(DownloadStack != null)
					DownloadStack.NotifySourceFail(this);
			}
			if(ImageFailed != null)
				ImageFailed(this, new TileImageFailedEventArgs(exception));
		}
		protected void RaiseImageCanceled() {
			SetStatusSafe(TileStatus.Cancelled);
			lock(DisposeLocker) {
				loadFailedAttempts = 0;
				if(DownloadStack != null)
					DownloadStack.NotifySourceFail(this);
			}
			if(ImageCancelled != null)
				ImageCancelled(this, new EventArgs());
		}
		protected void SetStatusSafe(TileStatus status) {
			lock(statusLocker) {
				this.status = status;
			}
		}
		public void Load() {
			if((status != TileStatus.Loading) && (status != TileStatus.Ready))
				DoLoad();
		}
		public void Cancel() {
			DoCancel();
		}
		public abstract int GetMemoryUsage();
		protected abstract void DoLoad();
		protected abstract void DoCancel();
		internal static TileImageSource GetTileImageSource(TileIndex index, Uri uri, IntSize expectedSize, string referer) {
			if(uri != null) {
				if(uri.OriginalString.Contains("file:")) {
					return new DiskImageSource(index, new FileInfo(uri.LocalPath));
				}
				else
					return new RemoteImageSource(index, uri, expectedSize, referer);
			}
			return null;
		}
	}
	public class DiskImageSource : TileImageSource {
		readonly FileInfo file;
		Size size;
		public DiskImageSource(TileIndex index, FileInfo file)
			: base(index) {
			this.file = file;
		}
		void LoadFileAsync(object target) {
			try {
				if(File.Exists(this.file.FullName)) {
					Image original = Image.FromFile(this.file.FullName);
					Source = BitmapUtils.ToArgbBitmap(original as Bitmap, true);
					this.size = Source.Size;
					if((Status == TileStatus.Loading) && !IsDisposed)
						RaiseImageLoaded(null);
				}
				else
					throw new FileNotFoundException();
			}
			catch(Exception exception) {
				RaiseImageFailed(exception);
			}
		}
		protected override void DoLoad() {
			SetStatusSafe(TileStatus.Loading);
			ThreadPool.QueueUserWorkItem(new WaitCallback(LoadFileAsync));
		}
		protected override void DoCancel() {
			SetStatusSafe(TileStatus.Cancelling);
		}
		public override int GetMemoryUsage() {
			return size.Width * size.Height * bytesPerPixel;
		}
	}
}
