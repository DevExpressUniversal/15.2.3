#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfDeferredSortedDictionary<K, T> : IDictionary<K, T>, IEnumerable<KeyValuePair<K, PdfDeferredItem<T>>> where T : class {
		static IComparer<string> stringComparer = StringComparer.Ordinal;
		readonly SortedDictionary<K, PdfDeferredItem<T>> dictionary;
		public ICollection<K> Keys { get { return dictionary.Keys; } }
		public ICollection<T> Values {
			get {
				ICollection<T> result = new List<T>();
				foreach (PdfDeferredItem<T> value in dictionary.Values)
					result.Add(value.Item);
				return result;
			}
		}
		public T this[K key] {
			get { return dictionary[key].Item; }
			set { dictionary[key] = new PdfDeferredItem<T>(value); }
		}
		public int Count { get { return dictionary.Count; } }
		public bool IsReadOnly { get { return false; } }
		public PdfDeferredSortedDictionary() {
			dictionary = new SortedDictionary<K, PdfDeferredItem<T>>(stringComparer as IComparer<K>);
		}
		public void AddRange(PdfDeferredSortedDictionary<K, T> value) {
			if(value != null)
				foreach (KeyValuePair<K, PdfDeferredItem<T>> pair in value.dictionary)
					dictionary.Add(pair.Key, pair.Value);
		}
		public void AddDeferred(K key, object value, Func<object, T> create) {
			dictionary[key] = new PdfDeferredItem<T>(value, create);
		}
		public void Add(K key, T value) {
			dictionary.Add(key, new PdfDeferredItem<T>(value));
		}
		public bool ContainsKey(K key) {
			return dictionary.ContainsKey(key);
		}
		public bool Remove(K key) {
			return dictionary.Remove(key);
		}
		public bool TryGetValue(K key, out T value) {
			PdfDeferredItem<T> result;
			if (dictionary.TryGetValue(key, out result)) {
				value = result.Item;
				return true;
			}
			value = null;
			return false;
		}
		public void Add(KeyValuePair<K, T> item) {
			dictionary.Add(item.Key, new PdfDeferredItem<T>(item.Value));
		}
		public void Clear() {
			dictionary.Clear();
		}
		public bool Contains(KeyValuePair<K, T> item) {
			PdfDeferredItem<T> value;
			if (dictionary.TryGetValue(item.Key, out value)) {
				return item.Value.Equals(value.Item);
			}
			return false;
		}
		public void CopyTo(KeyValuePair<K, T>[] array, int arrayIndex) {
			int i = arrayIndex;
			foreach (KeyValuePair<K, T> value in this) {
				array[i++] = value;
			}
		}
		public bool Remove(KeyValuePair<K, T> item) {
			if (Contains(item))
				return dictionary.Remove(item.Key);
			return false;
		}
		public IEnumerator<KeyValuePair<K, T>> GetEnumerator() {
			foreach (KeyValuePair<K, PdfDeferredItem<T>> value in dictionary)
				yield return new KeyValuePair<K, T>(value.Key, value.Value.Item);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator<KeyValuePair<K, PdfDeferredItem<T>>> IEnumerable<KeyValuePair<K, PdfDeferredItem<T>>>.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		internal void ResolveAll() {
			using (IEnumerator<KeyValuePair<K, T>> enumerator = GetEnumerator())
				while (enumerator.MoveNext()) ;
		}
		internal PdfDeferredItem<T> GetDeferredItem(K key) {
			PdfDeferredItem<T> value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			return null;
		}
	}
}
