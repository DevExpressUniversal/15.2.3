#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
namespace DevExpress.XtraReports.Web.ReportDesigner.Native {
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
		ICollection<KeyValuePair<TKey, TValue>> DictionaryAsCollection {
			get { return dictionary; }
		}
		readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		public void Add(TKey key, TValue value) {
			dictionary.Add(key, value);
			RaiseCollectionChanged();
		}
		public bool ContainsKey(TKey key) {
			return dictionary.ContainsKey(key);
		}
		public ICollection<TKey> Keys {
			get { return dictionary.Keys; }
		}
		public bool Remove(TKey key) {
			var result = dictionary.Remove(key);
			RaiseCollectionChanged();
			return result;
		}
		public bool TryGetValue(TKey key, out TValue value) {
			var result = dictionary.TryGetValue(key, out value);
			RaiseCollectionChanged();
			return result;
		}
		public ICollection<TValue> Values {
			get { return dictionary.Values; }
		}
		public TValue this[TKey key] {
			get { return dictionary[key]; }
			set {
				dictionary[key] = value;
				RaiseCollectionChanged();
			}
		}
		public void Add(KeyValuePair<TKey, TValue> item) {
			dictionary.Add(item.Key, item.Value);
			RaiseCollectionChanged();
		}
		public void Clear() {
			dictionary.Clear();
			RaiseCollectionChanged();
		}
		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return DictionaryAsCollection.Contains(item);
		}
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			DictionaryAsCollection.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return dictionary.Count; }
		}
		public bool IsReadOnly {
			get { return DictionaryAsCollection.IsReadOnly; }
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			var result = DictionaryAsCollection.Remove(item);
			RaiseCollectionChanged();
			return result;
		}
		public Dictionary<TKey, TValue>.Enumerator GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event Action CollectionChanged;
		void RaiseCollectionChanged() {
			var ev = CollectionChanged;
			if(ev != null) {
				ev();
			}
		}
	}
}
