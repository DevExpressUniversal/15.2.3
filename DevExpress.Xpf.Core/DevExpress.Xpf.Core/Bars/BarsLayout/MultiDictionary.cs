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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Core.Internal {
	public interface IMultiDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable {
		ICollection<TValue> Values { get; }
		ICollection<TKey> Keys { get; }
		ICollection<TValue> this[TKey key] { get; }
		void Add(TKey key, TValue value);
		void AddRange(TKey key, IEnumerable<TValue> values);
		bool Remove(TKey key);
		bool RemoveItem(TKey key, TValue value);
		bool Contains(TKey key, TValue value);
		bool ContainsKey(TKey key);
		bool ContainsValue(TValue value);
		IDictionary<TKey, ICollection<TValue>> ToDictionary();
	}
	public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable {
		TValue this[TKey key] { get; }
		IEnumerable<TKey> Keys { get; }
		IEnumerable<TValue> Values { get; }
		bool ContainsKey(TKey key);
		bool TryGetValue(TKey key, out TValue value);
	}
	public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable {
		int Count { get; }
	}
	public class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, ICollection<TValue>>, IReadOnlyCollection<KeyValuePair<TKey, ICollection<TValue>>>,
		IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>, IEnumerable {
		readonly Dictionary<TKey, ICollection<TValue>> dictionary;
		int count;
		ValueCollection values;
		int version;
		public MultiDictionary() {
			dictionary = new Dictionary<TKey, ICollection<TValue>>();
		}
		public MultiDictionary(int capacity) {
			dictionary = new Dictionary<TKey, ICollection<TValue>>(capacity);
		}
		public MultiDictionary(IEqualityComparer<TKey> comparer) {
			dictionary = new Dictionary<TKey, ICollection<TValue>>(comparer);
		}
		public MultiDictionary(int capacity, IEqualityComparer<TKey> comparer) {
			dictionary = new Dictionary<TKey, ICollection<TValue>>(capacity, comparer);
		}
		public MultiDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
			: this(enumerable, null) {
		}
		public MultiDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable, IEqualityComparer<TKey> comparer) {
			dictionary = new Dictionary<TKey, ICollection<TValue>>(comparer);
			foreach (var keyValuePair in enumerable)
				Add(keyValuePair.Key, keyValuePair.Value);
		}
		public ICollection<TValue> Values {
			get { return values ?? (values = new ValueCollection(this)); }
		}
		public ICollection<TKey> Keys {
			get { return dictionary.Keys; }
		}
		public ICollection<TValue> this[TKey key] {
			get { return new InnerCollectionView(this, key); }
		}
		public int Count {
			get { return count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public void Add(TKey key, TValue value) {
			ICollection<TValue> collection1;
			if (dictionary.TryGetValue(key, out collection1)) {
				collection1.Add(value);
			}
			else {
				ICollection<TValue> collection2 = NewCollection((IEnumerable<TValue>)null);
				collection2.Add(value);
				dictionary.Add(key, collection2);
			}
			++count;
			++version;
		}
		public void AddRange(TKey key, IEnumerable<TValue> values) {
			ICollection<TValue> collection1;
			if (dictionary.TryGetValue(key, out collection1)) {
				foreach (TValue obj in values) {
					collection1.Add(obj);
					++count;
				}
			}
			else {
				ICollection<TValue> collection2 = NewCollection(values);
				dictionary.Add(key, collection2);
				count += collection2.Count;
			}
			++version;
		}
		public bool Remove(TKey key) {
			ICollection<TValue> collection;
			if (!dictionary.TryGetValue(key, out collection) || !dictionary.Remove(key))
				return false;
			count -= collection.Count;
			++version;
			return true;
		}
		public bool RemoveItem(TKey key, TValue value) {
			ICollection<TValue> collection;
			if (!dictionary.TryGetValue(key, out collection) || !collection.Remove(value))
				return false;
			if (collection.Count == 0)
				dictionary.Remove(key);
			--count;
			++version;
			return true;
		}
		public bool Contains(TKey key, TValue value) {
			ICollection<TValue> collection;
			if (dictionary.TryGetValue(key, out collection))
				return collection.Contains(value);
			return false;
		}
		public bool ContainsKey(TKey key) {
			ICollection<TValue> collection;
			if (dictionary.TryGetValue(key, out collection))
				return collection.Count > 0;
			return false;
		}
		public bool ContainsValue(TValue value) {
			return dictionary.Values.Any(collection => collection.Contains(value));
		}
		public IDictionary<TKey, ICollection<TValue>> ToDictionary() {
			return new Dictionary<TKey, ICollection<TValue>>(dictionary, dictionary.Comparer);
		}
		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}
		public void Clear() {
			count = 0;
			++version;
			dictionary.Clear();
		}
		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return Contains(item.Key, item.Value);
		}
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			foreach (var keyValuePair in this)
				array[arrayIndex++] = keyValuePair;
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			return RemoveItem(item.Key, item.Value);
		}
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return new Enumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerable<ICollection<TValue>> IReadOnlyDictionary<TKey, ICollection<TValue>>.Values {
			get { return dictionary.Values; }
		}
		IEnumerable<TKey> IReadOnlyDictionary<TKey, ICollection<TValue>>.Keys {
			get { return Keys; }
		}
		int IReadOnlyCollection<KeyValuePair<TKey, ICollection<TValue>>>.Count {
			get { return dictionary.Count; }
		}
		IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		bool IReadOnlyDictionary<TKey, ICollection<TValue>>.TryGetValue(TKey key, out ICollection<TValue> value) {
			value = new InnerCollectionView(this, key);
			return true;
		}
		protected virtual ICollection<TValue> NewCollection(IEnumerable<TValue> collection = null) {
			if (collection == null)
				return new List<TValue>();
			return new List<TValue>(collection);
		}
		public class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable {
			readonly MultiDictionary<TKey, TValue> dictionary;
			readonly IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> keyEnumerator;
			readonly int version;
			KeyValuePair<TKey, ICollection<TValue>> currentListPair;
			KeyValuePair<TKey, TValue> currentValuePair;
			IEnumerator<TValue> valuesEnumerator;
			internal Enumerator(MultiDictionary<TKey, TValue> multiDictionary) {
				dictionary = multiDictionary;
				keyEnumerator = multiDictionary.dictionary.GetEnumerator();
				currentListPair = keyEnumerator.Current;
				valuesEnumerator = null;
				currentValuePair = new KeyValuePair<TKey, TValue>();
				version = multiDictionary.version;
			}
			public KeyValuePair<TKey, TValue> Current {
				get { return currentValuePair; }
			}
			object IEnumerator.Current {
				get { return currentValuePair; }
			}
			public bool MoveNext() {
				if (version != dictionary.version)
					throw new InvalidOperationException("Enumeration is modified");
				if (valuesEnumerator != null && valuesEnumerator.MoveNext()) {
					currentValuePair = new KeyValuePair<TKey, TValue>(currentListPair.Key, valuesEnumerator.Current);
					return true;
				}
				if (MoveNextKey())
					return MoveNext();
				currentValuePair = new KeyValuePair<TKey, TValue>();
				return false;
			}
			public void Reset() {
				if (version != dictionary.version)
					throw new InvalidOperationException("Enumeration is modified");
				keyEnumerator.Reset();
				currentListPair = keyEnumerator.Current;
				valuesEnumerator = null;
				currentValuePair = new KeyValuePair<TKey, TValue>();
			}
			public void Dispose() {
				if (keyEnumerator != null)
					keyEnumerator.Dispose();
				if (valuesEnumerator == null)
					return;
				valuesEnumerator.Dispose();
			}
			~Enumerator() {
				try {
					Dispose();
				}
				finally {
				}
			}
			bool MoveNextKey() {
				if (!keyEnumerator.MoveNext())
					return false;
				currentListPair = keyEnumerator.Current;
				valuesEnumerator = currentListPair.Value.GetEnumerator();
				return true;
			}
		}
		class InnerCollectionView : ICollection<TValue>, IEnumerable<TValue>, IEnumerable {
			readonly TKey key;
			readonly MultiDictionary<TKey, TValue> multidictionary;
			public InnerCollectionView(MultiDictionary<TKey, TValue> multidictionary, TKey key) {
				this.multidictionary = multidictionary;
				this.key = key;
			}
			public int Count {
				get {
					ICollection<TValue> collection;
					if (multidictionary.dictionary.TryGetValue(key, out collection))
						return collection.Count;
					return 0;
				}
			}
			public bool IsReadOnly {
				get {
					ICollection<TValue> collection;
					if (multidictionary.dictionary.TryGetValue(key, out collection))
						return multidictionary.dictionary[key].IsReadOnly;
					return multidictionary.NewCollection((IEnumerable<TValue>)null).IsReadOnly;
				}
			}
			public void Add(TValue item) {
				multidictionary.Add(key, item);
			}
			public void Clear() {
				multidictionary.Remove(key);
			}
			public bool Contains(TValue item) {
				ICollection<TValue> collection;
				if (multidictionary.dictionary.TryGetValue(key, out collection))
					return collection.Contains(item);
				return false;
			}
			public void CopyTo(TValue[] array, int arrayIndex) {
				ICollection<TValue> collection;
				if (!multidictionary.dictionary.TryGetValue(key, out collection))
					return;
				collection.CopyTo(array, arrayIndex);
			}
			public bool Remove(TValue item) {
				return multidictionary.RemoveItem(key, item);
			}
			public IEnumerator<TValue> GetEnumerator() {
				return new Enumerator(multidictionary, key);
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
			class Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable {
				readonly IEnumerator<TValue> enumerator;
				readonly MultiDictionary<TKey, TValue> multiDictionary;
				readonly int version;
				public Enumerator(MultiDictionary<TKey, TValue> multiDictionary, TKey key) {
					this.multiDictionary = multiDictionary;
					version = multiDictionary.version;
					ICollection<TValue> collection;
					if (!multiDictionary.dictionary.TryGetValue(key, out collection))
						return;
					enumerator = collection.GetEnumerator();
				}
				public TValue Current {
					get {
						if (enumerator == null)
							return default(TValue);
						return enumerator.Current;
					}
				}
				object IEnumerator.Current {
					get { return Current; }
				}
				public bool MoveNext() {
					if (version != multiDictionary.version)
						throw new InvalidOperationException("Enumeration is modified");
					if (enumerator == null)
						return false;
					return enumerator.MoveNext();
				}
				public void Reset() {
					if (version != multiDictionary.version)
						throw new InvalidOperationException("Enumeration is modified");
					if (enumerator == null)
						return;
					enumerator.Reset();
				}
				public void Dispose() {
					if (enumerator == null)
						return;
					enumerator.Dispose();
				}
				~Enumerator() {
					try {
						Dispose();
					}
					finally {
					}
				}
			}
		}
		class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable {
			readonly MultiDictionary<TKey, TValue> multiDictionary;
			public ValueCollection(MultiDictionary<TKey, TValue> multiDictionary) {
				this.multiDictionary = multiDictionary;
			}
			public int Count {
				get { return multiDictionary.Count; }
			}
			public bool IsReadOnly {
				get { return true; }
			}
			public bool Contains(TValue item) {
				return multiDictionary.ContainsValue(item);
			}
			public void CopyTo(TValue[] array, int arrayIndex) {
				foreach (TValue obj in this)
					array[arrayIndex++] = obj;
			}
			public bool Remove(TValue item) {
				throw new NotSupportedException("ReadOnly_Modification");
			}
			public void Add(TValue item) {
				throw new NotSupportedException("ReadOnly_Modification");
			}
			public void Clear() {
				throw new NotSupportedException("ReadOnly_Modification");
			}
			public IEnumerator<TValue> GetEnumerator() {
				return new ValueCollectionEnumerator(multiDictionary);
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
			class ValueCollectionEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator {
				readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
				bool valid;
				internal ValueCollectionEnumerator(MultiDictionary<TKey, TValue> multidictionary) {
					enumerator = multidictionary.GetEnumerator();
					valid = false;
				}
				public TValue Current {
					get {
						if (valid)
							return enumerator.Current.Value;
						return default(TValue);
					}
				}
				object IEnumerator.Current {
					get { return Current; }
				}
				public bool MoveNext() {
					valid = enumerator.MoveNext();
					return valid;
				}
				public void Reset() {
					enumerator.Reset();
					valid = false;
				}
				public void Dispose() {
					if (enumerator == null)
						return;
					enumerator.Dispose();
				}
				~ValueCollectionEnumerator() {
					try {
						Dispose();
					}
					finally {
					}
				}
			}
		}
	}
}
