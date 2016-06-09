#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	class LRUCache<K, V> { 
		struct Entry {
			K key;
			V value;
			public K Key { get { return key; } }
			public V Value { get { return value; } }
			public Entry(K key, V value) {
				this.key = key;
				this.value = value;
			}
		}
		int capacity;
		Dictionary<K, LinkedListNode<Entry>> cacheItems;
		LinkedList<Entry> lruList;
		object syncObject;
		public LRUCache(int capacity) {
			this.capacity = capacity;
			this.cacheItems = new Dictionary<K, LinkedListNode<Entry>>();
			this.lruList = new LinkedList<Entry>();
			this.syncObject = new object();
		}
		public V GetOrAdd(K key, Func<V> valueIfCacheEmpty) {
			lock(syncObject) {
				LinkedListNode<Entry> node;
				if(!cacheItems.TryGetValue(key, out node)) {
					V value = valueIfCacheEmpty();
					Add(key, value);
					return value;
				}
				lruList.Remove(node);
				lruList.AddFirst(node);
				return node.Value.Value;
			}
		}
		void Add(K key, V value) {
			if(lruList.Count >= capacity)
				RemoveLeastUsed();
			Entry e = new Entry(key, value);
			LinkedListNode<Entry> node = new LinkedListNode<Entry>(e);
			cacheItems.Add(key, node);
			lruList.AddFirst(node);
		}
		void RemoveLeastUsed() {
			LinkedListNode<Entry> node = lruList.Last;
			lruList.RemoveLast();
			cacheItems.Remove(node.Value.Key);
		}
	}
}
