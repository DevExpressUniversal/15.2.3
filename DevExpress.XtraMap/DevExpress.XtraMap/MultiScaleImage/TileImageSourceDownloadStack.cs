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
using System.Threading;
using System.IO;
namespace DevExpress.XtraMap.Native {
	public class TileImageSourceDownloadStack : MapDisposableObject {
		int maxParallelDownloads;
		List<TileImageSource> sourceList;
		ManualResetEvent updateEvent;
		TileSourceCache cache;
		Thread thread;
		static int instanceCount;
		ManualResetEventSlim externalLock;
		ManualResetEventSlim internalLock;
		protected internal List<TileImageSource> SourceList { get { return sourceList; } }
		public static int InstanceCount { get { return instanceCount; } }
		public event MapWebRequestEventHandler WebRequest;
		public TileImageSourceDownloadStack(TileSourceCache cache, int maxParallelDownloads) {
			Interlocked.Increment(ref instanceCount);
			this.maxParallelDownloads = maxParallelDownloads;
			this.sourceList = new List<TileImageSource>(this.maxParallelDownloads);
			this.updateEvent = new ManualResetEvent(false);
			this.cache = cache;
			this.externalLock = new ManualResetEventSlim(true);
			this.internalLock = new ManualResetEventSlim(true);
		}
		internal void StartLoadTiles() {
			if(thread == null) {
				thread = MapUtils.CreateThread(new ThreadStart(ForkedStackUpdater), GetType().Name);
				thread.Start();
			}
		}
		void Remove(TileImageSource source) {
			if(source == null) return;
			source.DownloadStack = null;
			InternalRemove(source);
			Update();
		}
		void InternalRemove(TileImageSource source) {
			if(source != null) {
				lock(sourceList) {
					sourceList.Remove(source);
				}
				source.ImageCancelled -= ImageCancelled;
				source.ImageFailed -= ImageFailed;
				source.ImageLoaded -= ImageLoaded;
			}
		}
		void Update() {
			if(updateEvent != null)
				updateEvent.Set();
		}
		void ImageCancelled(object sender, EventArgs e) {
			Update();
		}
		void ImageFailed(object sender, TileImageFailedEventArgs e) {
			if(e.Exception.Message == "The remote server returned an error: (404) Not Found." ||
			e.Exception.Message == "no tile" || e.Exception is FileNotFoundException)
				Remove((TileImageSource)sender);
			else
				Update();
		}
		void ImageLoaded(object sender, EventArgs e) {
			RemoteImageSource ris = sender as RemoteImageSource;
			if(ris != null)
				ris.WebRequest -= OnWebRequest;
			Remove(sender as TileImageSource);
		}
		void ForkedStackUpdater() {
			while(true) {
				updateEvent.WaitOne();
				if(IsDisposed)
					break;
				updateEvent.Reset();
				int index = 0;
				lock(sourceList) {
					foreach(TileImageSource source in sourceList) {
						if(index < maxParallelDownloads) {
							if(source.Status != TileStatus.Loading) {
								RemoteImageSource ris = source as RemoteImageSource;
								if(ris != null)
									ris.WebRequest += OnWebRequest;
								source.Load();
							}
						}
						index++;
					}
				}
			}
		}
		void RaiseWebRequest(MapWebRequestEventArgs e) {
			if(WebRequest != null)
				WebRequest(this, e);
		}
		void OnWebRequest(object sender, MapWebRequestEventArgs e) {
			RaiseWebRequest(e);
		}
		void Terminate() {
			lock(sourceList) {
				int count = sourceList.Count;
				for(int i = count - 1; i >= 0; i--) {
					TileImageSource item = sourceList[i];
					InternalRemove(item);
					item.Cancel();
				}
			}
			sourceList.Clear();
			updateEvent.Set();
		}
		protected override void DisposeOverride() {
			Terminate();
			if(thread != null) {
				thread.Join();
				thread = null;
			}
			if(updateEvent != null) {
				updateEvent.Close();
				updateEvent = null;
			}
			Interlocked.Decrement(ref instanceCount);
		}
		public void Push(TileImageSource source) {
			InternalPush(source, false);
		}
		public void Exclude(TileImageSource source) {
			if(sourceList.Contains(source)) {
				Remove(source);
				source.Cancel();
				source.DownloadStack = null;
			}
		}
		public void NotifySourceSuccess(TileImageSource source, byte[] rawData) {
			if(IsDisposed)
				return;
			externalLock.Wait();
			internalLock.Reset();
			Remove(source);
			cache.PushToDisk(source, rawData);
			internalLock.Set();
		}
		public void NotifySourceFail(TileImageSource source) {
			if(IsDisposed)
				return;
			externalLock.Wait();
			internalLock.Reset();
			if(source != null && source.CanDelete)
				InternalRemove(source);
			else MoveToEnd(source);
			internalLock.Set();
		}
		void MoveToEnd(TileImageSource source) {
			InternalRemove(source);
			InternalPush(source, true);
		}
		void InternalPush(TileImageSource source, bool toEndList) {
			if(source != null) {
				lock(sourceList) {
					if(!sourceList.Contains(source)) {
						if(sourceList.Count < maxParallelDownloads || toEndList)
							sourceList.Add(source);
						else
							sourceList.Insert(maxParallelDownloads, source);
					}
				}
				source.ImageLoaded += ImageLoaded;
				source.ImageFailed += ImageFailed;
				source.ImageCancelled += ImageCancelled;
				source.DownloadStack = this;
				Update();
			}
		}
		public void Lock() {
			internalLock.Wait();
			externalLock.Reset();
		}
		public void Unlock() {
			externalLock.Set();
		}
	}
}
