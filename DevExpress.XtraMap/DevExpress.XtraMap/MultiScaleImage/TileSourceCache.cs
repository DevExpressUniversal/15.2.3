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
namespace DevExpress.XtraMap.Native {
	public class TileSourceCache : MapDisposableObject {
		const int constMaxParralelDownloads = 5;
		const int constPreseveLevel = 1;
		DiskCache diskCache;
		long memoryUsage;
		Dictionary<TileIndex, TileImageSource> memory;
		List<TileImageSource> timeline;
		MultiScaleTileSource tileSource;
		TileImageSourceDownloadStack downloadStack;
		protected internal TileImageSourceDownloadStack DownloadStack { get { return downloadStack; } }
		DiskCache DiskCache {
			get {
				return diskCache;
			}
			set {
				if(diskCache != value) {
					if(diskCache != null)
						diskCache.Dispose();
					diskCache = value;
				}
			}
		}
		void UpdateDiskCache() {
			CacheOptions options = this.tileSource.CacheOptions;
			if(DiskCache == null) {
				if(!string.IsNullOrEmpty(options.DiskFolder))
					DiskCache = new DiskCache(options.DiskFolder, options.DiskExpireTime, options.DiskLimit);
			}
			else {
				if(string.IsNullOrEmpty(options.DiskFolder)) {
					DiskCache = null;
				}
				else {
					if(!DiskCache.LocatedAt(options.DiskFolder))
						DiskCache = new DiskCache(options.DiskFolder, options.DiskExpireTime, options.DiskLimit);
					else {
						DiskCache.ExpireTime = this.tileSource.CacheOptions.DiskExpireTime;
						DiskCache.DiskLimit = this.tileSource.CacheOptions.DiskLimit;
					}
				}
			}
		}
		void ClearMemory() {
			foreach (KeyValuePair<TileIndex, TileImageSource> pair in memory)
				pair.Value.Dispose();
			memory.Clear();
			timeline.Clear();
		}
		protected override void DisposeOverride() {
			lock (memory) {
				if (downloadStack != null) {
					downloadStack.WebRequest -= OnWebRequest;
					downloadStack.Dispose();
					downloadStack = null;
				}
				ClearMemory();
				if (DiskCache != null)
					DiskCache.Dispose();
			}
		}
		public TileSourceCache(MultiScaleTileSource tileSource) {
			this.tileSource = tileSource;
			this.memory = new Dictionary<TileIndex, TileImageSource>();
			this.timeline = new List<TileImageSource>();
			this.downloadStack = new TileImageSourceDownloadStack(this, constMaxParralelDownloads);
			this.downloadStack.WebRequest += OnWebRequest;
		}
		void OnWebRequest(object sender, MapWebRequestEventArgs e) {
			this.tileSource.OnWebRequest(e);
		}
		public TileImageSource Retrieve(TileIndex index) {
			TileImageSource item = null;
			lock(memory) {
				if(memory.TryGetValue(index, out item))
					item.TimeStamp = DateTime.Now;
			}
			if(item != null)
				return item;
			if(DiskCache != null)
				item = DiskCache.Retrieve(index, this.tileSource.TilePrefix);
			return item;
		}
		public void Push(TileImageSource tileSource) {
			lock(memory) {
				memory[tileSource.Index] = tileSource;
				if(!timeline.Contains(tileSource))
					timeline.Add(tileSource);
				if(downloadStack != null && tileSource.Status != TileStatus.Ready)
					downloadStack.Push(tileSource);
			}
			CheckMemoryLimit();
		}
		public void CacheOptionsUpdated() {
			UpdateDiskCache();
			CheckMemoryLimit();
		}
		public void CheckMemoryLimit() {
			List<TileImageSource> goodbye = new List<TileImageSource>();
			lock(memory) {
				if(IsDisposed) 
					return;
				CacheOptions options = this.tileSource.CacheOptions;
				if(options != null) {
					timeline.Sort((TileImageSource a, TileImageSource b) => { return a.TimeStamp.CompareTo(b.TimeStamp); });
					memoryUsage = 0;
					long memoryLimit = (long)options.MemoryLimit << 20;
					for(int index = 0; index < timeline.Count; index++)
						memoryUsage += timeline[index].GetMemoryUsage();
					if(memoryUsage > memoryLimit) {
						int overhead = (int)(memoryUsage - memoryLimit);
						for(int index = 0; index < timeline.Count; index++) {
							TileImageSource item = timeline[index];
							int itemMemory = item.GetMemoryUsage();
							if((itemMemory > 0) && (item.Index.Level > constPreseveLevel)) {
								overhead -= itemMemory;
								goodbye.Add(item);
								if(overhead <= 0)
									break;
							}
						}
						for(int index = 0; index < goodbye.Count; index++) {
							TileImageSource item = goodbye[index];
							memory.Remove(item.Index);
							timeline.Remove(item);
							downloadStack.Exclude(item);
							item.Dispose();
						}
					}
				}
			}
		}
		public void PushToDisk(TileImageSource source, byte[] rawData) {
			if((source != null) && (rawData != null) && (DiskCache != null))
				DiskCache.Push(rawData, source.Index, this.tileSource.TilePrefix);
		}
		public void Lock() {
			if(DownloadStack != null)
				DownloadStack.Lock();
		}
		public void Unlock() {
			if(DownloadStack != null)
				DownloadStack.Unlock();
		}
		protected internal void StartLoadTiles() {
			if(DownloadStack != null)
				DownloadStack.StartLoadTiles();
		}
	}
}
