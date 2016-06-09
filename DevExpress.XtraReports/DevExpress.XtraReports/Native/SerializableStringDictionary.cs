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
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Globalization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Native {
	[DesignerSerializer("DevExpress.XtraReports.Design.StringDictionarySerializer," + AssemblyInfo.SRAssemblyReportsExtensionsFull, AttributeConstants.CodeDomSerializer)]
	public class SerializableStringDictionary : SerializableDictionary<string, string>  {
	}
	public class SerializableDictionary<T, V> : IDictionary<T, V>, ICollection {
		Dictionary<T, V> contents = new Dictionary<T, V>();
		#region IDictionary<string,string> Members
		public void Add(T key, V value) {
			contents[key] = value;
		}
		public bool ContainsKey(T key) {
			return contents.ContainsKey(key);
		}
		public ICollection<T> Keys {
			get { return contents.Keys; }
		}
		public bool Remove(T key) {
			return contents.Remove(key);
		}
		public bool TryGetValue(T key, out V value) {
			return contents.TryGetValue(key, out value);
		}
		public ICollection<V> Values {
			get { return contents.Values; }
		}
		public V this[T key] {
			get {
				V result;
				TryGetValue(key, out result);
				return result;
			}
			set {
				contents[key] = value;
			}
		}
		#endregion
		#region ICollection<KeyValuePair<string,string>> Members
		public void Add(KeyValuePair<T, V> item) {
			((ICollection<KeyValuePair<T, V>>)contents).Add(item);
		}
		public void Clear() {
			contents.Clear();
		}
		public bool Contains(KeyValuePair<T, V> item) {
			return ((ICollection<KeyValuePair<T, V>>)contents).Contains(item);
		}
		public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex) {
			((ICollection<KeyValuePair<T, V>>)contents).CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return contents.Count; }
		}
		public bool IsReadOnly {
			get { return ((ICollection<KeyValuePair<T, V>>)contents).IsReadOnly; }
		}
		public bool Remove(KeyValuePair<T, V> item) {
			return ((ICollection<KeyValuePair<T, V>>)contents).Remove(item);
		}
		#endregion
		#region IEnumerable<KeyValuePair<T,V>> Members
		public IEnumerator<KeyValuePair<T, V>> GetEnumerator() {
			return ((IEnumerable<KeyValuePair<T, V>>)contents).GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return new SerializableDictionaryEnumerator<T, V>(contents);
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object _syncRoot;
		object ICollection.SyncRoot {
			get {
				if(this._syncRoot == null) {
					System.Threading.Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}
		#endregion
	}
	class SerializableDictionaryEnumerator<TKey, TValue> : IEnumerator {
		IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
		object current;
		public SerializableDictionaryEnumerator(IDictionary<TKey, TValue> dictionary) {
			enumerator = dictionary.GetEnumerator();
		}
		object IEnumerator.Current {
			get { return current; }
		}
		bool IEnumerator.MoveNext() {
			if(enumerator.MoveNext()) {
				current = new SerializableKeyValuePair<TKey, TValue>(enumerator.Current.Key, enumerator.Current.Value);
				return true;
			}
			return false;
		}
		void IEnumerator.Reset() {
			current = null;
			enumerator.Reset();
		}
	}
	public class SerializableKeyValuePair<TKey, TValue> {
		private TKey key;
		private TValue value;
		public SerializableKeyValuePair(TKey key, TValue value) {
			this.key = key;
			this.value = value;
		}
		public SerializableKeyValuePair() {
		}
		[XtraSerializableProperty]
		public TKey Key {
			get { return key; }
			set { key = value; }
		}
		[XtraSerializableProperty]
		public TValue Value {
			get { return value; }
			set { this.value = value; }
		}
		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			builder.Append('[');
			if(this.Key != null) {
				builder.Append(this.Key.ToString());
			}
			builder.Append(", ");
			if(this.Value != null) {
				builder.Append(this.Value.ToString());
			}
			builder.Append(']');
			return builder.ToString();
		}
	}
}
