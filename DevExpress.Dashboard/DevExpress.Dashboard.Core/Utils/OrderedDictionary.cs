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

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.DashboardCommon.Native {
	public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
		readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		readonly IList<KeyValuePair<TKey, TValue>> orderedPairs = new List<KeyValuePair<TKey, TValue>>();
		public TValue this[TKey key] { get { return dictionary[key]; } set { dictionary[key] = value; } }
		public ReadOnlyCollection<TKey> Keys { get { return new ReadOnlyCollection<TKey>(orderedPairs.Select(pair=>pair.Key).ToList()); } }
		public ReadOnlyCollection<TValue> Values { get { return new ReadOnlyCollection<TValue>(orderedPairs.Select(pair => pair.Value).ToList()); } }
		public int Count { get { return dictionary.Count; } }
		public bool IsReadOnly { get { return false; } }
		public OrderedDictionary() {
		}
		public void Add(TKey key, TValue value) {
			dictionary.Add(key, value);
			orderedPairs.Add(new KeyValuePair<TKey, TValue>(key, value));
		}
		public bool Remove(TKey key) {
			orderedPairs.Remove(new KeyValuePair<TKey, TValue>(key, dictionary[key]));
			return dictionary.Remove(key);
		}
		public bool ContainsKey(TKey key) {
			return dictionary.ContainsKey(key);
		}
		public bool TryGetValue(TKey key, out TValue value) {
			return dictionary.TryGetValue(key, out value);
		}
		public void Clear() {
			dictionary.Clear();
			orderedPairs.Clear();
		}
		ICollection<TKey> IDictionary<TKey, TValue>.Keys { get { return Keys; } }
		ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return Values; } }
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			orderedPairs.Remove(item);
			return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
			return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			return orderedPairs.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return orderedPairs.GetEnumerator();
		}
	}
}
