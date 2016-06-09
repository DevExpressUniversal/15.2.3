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
using System.Drawing;
using System.Linq;
using System.Threading;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraScheduler {
	public static class GraphicsCachePool {
		static object storageLock = new object();
		static HashSet<CacheStorage> storage;
		public static GraphicsCache GetCache() {
			lock (storageLock) {
				if (storage == null) {
					storage = new HashSet<CacheStorage>();
					RunThreadChecks();
				}
				foreach (CacheStorage cacheStorage in storage) {
					if (cacheStorage.Thread == null || cacheStorage.Thread == Thread.CurrentThread) {
						cacheStorage.Thread = Thread.CurrentThread;
						return cacheStorage.Cache;
					}
				}
				GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero));
				storage.Add(new CacheStorage() { Cache = cache, Thread = Thread.CurrentThread });
				Monitor.Pulse(storageLock);
				return cache;
			}
		}
		static void RunThreadChecks() {
			ThreadPool.QueueUserWorkItem(state => {
				lock (storageLock) {
					int nullThreadsTime = 0;
					while (true) {
						try {
							int nullThreadCount = CheckFinishedThreads();
							if (!Monitor.Wait(storageLock, 500)) {
								if (nullThreadCount >= storage.Count - 1 && storage.Count > 1)
									nullThreadsTime += 500;
								if (nullThreadsTime >= 60000) {
									CleanUpThreadCache();
									nullThreadsTime = 0;
								}
							}
						} catch (ThreadAbortException) {
							CleanUpThreadCache();
						}
					}
				}
			});
		}
		static int CheckFinishedThreads() {
			int nullThreads = 0;
			foreach (CacheStorage cacheStorage in storage) {
				if (cacheStorage.Thread == null) {
					nullThreads++;
					continue;
				}
				if (cacheStorage.Thread.Join(0))
					cacheStorage.Thread = null;
			}
			return nullThreads;
		}
		static void CleanUpThreadCache() {
			List<CacheStorage> nullThreadCache = storage.Where(si => si.Thread == null).ToList();
			foreach (CacheStorage cacheStorage in nullThreadCache) {
				cacheStorage.Cache.Dispose();
				storage.Remove(cacheStorage);
			}
			nullThreadCache.Clear();
			nullThreadCache = null;
		}
	}
	public class CacheStorage {
		public Thread Thread { get; set; }
		public GraphicsCache Cache { get; set; }
	}
}
