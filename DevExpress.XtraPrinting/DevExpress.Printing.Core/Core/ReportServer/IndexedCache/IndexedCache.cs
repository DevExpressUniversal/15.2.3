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
namespace DevExpress.ReportServer.IndexedCache {
	abstract class IndexedCache<T> : ICache<T>, IDisposable {
		protected readonly IList<IndexedCacheItem<T>> cache;
		protected bool IsDisposed { get; private set; }
		public IndexedCache() {
			cache = new List<IndexedCacheItem<T>>();
			IsDisposed = false;
		}
		public bool EnlargeCapacity(int capacity) {
			if(cache.Count >= capacity)
				return false;
			int oldCapacity = cache.Count;
			for(int i = oldCapacity; i < capacity; i++)
				cache.Add(new IndexedCacheItem<T>(CreateFakeValue(i, capacity)));
			return true;
		}
		public void Ensure(int[] indexes, Action<int[]> listener) {
			lock(this) {
				int[] indexesToRequest = EnsureIndexes(indexes);
				if(indexesToRequest.Length > 0)
					foreach(int index in indexesToRequest) {
						cache[index].State = IndexedCacheItemState.Requested;
						cache[index].AddItemCachedCallback(listener);
					}
				StartRequestIfNeeded();
			}
		}
		public int Capacity { get { return cache.Count; } }
		public void Clear() {
			foreach(var item in cache)
				item.Dispose();
			cache.Clear();
		}
		public void Dispose() {
			Clear();
			IsDisposed = true;
		}
		public T this[int index] { 
			get { return cache[index].Value; }
			set { }
		}
		public bool IsElementCached(int index) {
			return cache[index].State == IndexedCacheItemState.Cached;
		}
		protected int[] EnsureIndexes(int[] indexes) {
			List<int> value = new List<int>();
			foreach(int index in indexes) {
				if(cache[index].State == IndexedCacheItemState.NotRequested)
					value.Add(index);
			}
			return value.ToArray();
		}
		protected void OnRequestCompleted(Dictionary<int, T> result) {
			IndexedCacheItem<T> item;
			HashSet<Action<int[]>> callbacks = new HashSet<Action<int[]>>();
			int[] indexes = new int[result.Keys.Count];
			result.Keys.CopyTo(indexes, 0);
			foreach(var pair in result) {
				item = cache[pair.Key];
				var itemCallbacks = item.GetItemCachedCallbacks();
				foreach(var itemCallback in itemCallbacks)
					if(!callbacks.Contains(itemCallback))
						callbacks.Add(itemCallback);
				item.SetRealValue(pair.Value);
			}
			foreach(var callback in callbacks)
				callback(indexes);
			StartRequestIfNeeded();
		}
		protected abstract T CreateFakeValue(int index, int count);
		protected abstract void StartRequestIfNeeded();
	}
}
