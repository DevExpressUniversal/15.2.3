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

using System.Collections;
using System.Collections.Generic;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class DictionaryContainer<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TValue : class {
		Dictionary<TKey, TValue> dictionary;
		 protected DictionaryContainer(int count) {
				this.dictionary = new Dictionary<TKey, TValue>(count);
		 }
		protected DictionaryContainer() {
			this.dictionary = new Dictionary<TKey, TValue>();
		}
		protected void EnsureCapcity(int count) {
			if(dictionary.Count == 0)
				dictionary = new Dictionary<TKey, TValue>(count);
		}
		public Dictionary<TKey, TValue>.KeyCollection Keys { get { return dictionary.Keys; } }
		public Dictionary<TKey, TValue>.ValueCollection Values { get { return dictionary.Values; } }
		public int Count { get { return dictionary.Count; } }
		protected TValue GetDictionaryValue(TKey key) {
			TValue res;
			return dictionary.TryGetValue(key, out res) ? res : null;
		}
		protected void SetDictionaryValue(TKey key, TValue value) {
			dictionary[key] = value;
		}
		protected void AddDictionaryValue(TKey key, TValue value) {
			dictionary.Add(key, value); 
		}
		public virtual void Clear() {
			dictionary.Clear();
		}
		protected void Remove(TKey key) {
			dictionary.Remove(key);
		}
		public void RemoveItems(List<TKey> removedChildren) {
			for(int i = 0; i < removedChildren.Count; i++) {
				Remove(removedChildren[i]);
			}
		}
		public bool TryGetValue(TKey key, out TValue value) {
			if(dictionary.TryGetValue(key, out value))
				return true;
			value = default(TValue);
			return false;
		}
		public bool IsEmpty() {
			return dictionary.Count == 0;
		}
		#region IEnumerable<KeyValuePair<TKey,TValue>> Members
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		#endregion
	}
}
