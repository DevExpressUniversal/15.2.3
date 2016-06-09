#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.Utils {
	[Serializable]
	public class LightDictionaryException : ArgumentException {
		protected LightDictionaryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public LightDictionaryException(string message) : base(message) { }
	}
	public enum ReadByNonExistentKeyMode { ReturnDefault, ThrowException }
	public class LightDictionary<KeyType, ItemValueType> : IEnumerable<ItemValueType>, IDictionary<KeyType, ItemValueType> {
		[Serializable]
		public class DuplicatedKeyException : LightDictionaryException {
			protected DuplicatedKeyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
			public DuplicatedKeyException(KeyType key, ItemValueType existingValue, ItemValueType newValue)
				: base(string.Format(
					"Item has already been added. \n  Key: '{0}'\n  Value in dictionary: '{1}'\n  Value being added: '{2}'",
					key, existingValue, newValue)) {
				if(existingValue is object) {
					this.Data.Add("existingValueHashCode", ((object)existingValue).GetHashCode());
				}
				if(newValue is object) {
					this.Data.Add("newValueHashCode", ((object)newValue).GetHashCode());
				}
			}
		}
		private static List<KeyType> KeysEmptyList = new List<KeyType>(0);
		private class KeyValuePair {
			public KeyType Key;
			public ItemValueType Value;
			public KeyValuePair Next;
			public KeyValuePair(KeyType key, ItemValueType value) {
				this.Key = key;
				this.Value = value;
			}
		}
		private class ValuesEnumerator : IEnumerator<ItemValueType> {
			private LightDictionary<KeyType, ItemValueType> source;
			private KeyValuePair current;
			private bool init;
			public ValuesEnumerator(LightDictionary<KeyType, ItemValueType> source) {
				this.source = source;
				this.init = true;
			}
			#region IEnumerator<ItemValueType> Members
			public ItemValueType Current {
				get { return current.Value; }
			}
			public void Dispose() {
				source = null;
				current = null;
			}
			object IEnumerator.Current {
				get { return current.Value; }
			}
			public bool MoveNext() {
				if(init) {
					current = source.head;
					init = false;
				}
				else {
					current = current.Next;
				}
				return current != null;
			}
			public void Reset() {
				init = true;
			}
			#endregion
		}
		private KeyValuePair head;
		private KeyValuePair tail;
		private KeyValuePair lastRequestedItem;
		private int lastRequestedItemIndex;
		private int count;
		private bool allowNullKey = false;
		private ReadByNonExistentKeyMode readByNonExistentKeyMode = ReadByNonExistentKeyMode.ReturnDefault;
		private List<KeyType> keysCache = null;
		private List<ItemValueType> valuesCache = null;
		private KeyValuePair AddItem(KeyType key, ItemValueType itemValue) {
			return AddItem(-1, key, itemValue);
		}
		private KeyValuePair AddItem(int index, KeyType key, ItemValueType value) {
			OnChanging(ListChangedType.ItemAdded, key, default(ItemValueType), value);
			if(index > Count || index < -1) {
				throw new ArgumentOutOfRangeException("index", index, null);
			}
			KeyValuePair newItem = new KeyValuePair(key, value);
			if(index == -1 || index == Count) {
				if(head == null)
					head = newItem;
				else
					tail.Next = newItem;
				tail = newItem;
			}
			else {
				if(index == 0) {
					newItem.Next = head;
					head = newItem;
				}
				else {
					KeyValuePair item = GetPair(index - 1);
					newItem.Next = item.Next;
					item.Next = newItem;
				}
			}
			count++;
			OnChanged(ListChangedType.ItemAdded, key, default(ItemValueType), value);
			return newItem;
		}
		private KeyValuePair FindPrevItem(KeyValuePair item) {
			KeyValuePair currentItem = head;
			while(currentItem != null) {
				if(currentItem.Next == item) {
					return currentItem;
				}
				currentItem = currentItem.Next;
			}
			return null;
		}
		private KeyValuePair FindPair(KeyType key) {
			if(key == null && !AllowNullKey) {
				throw new ArgumentNullException();
			}
			KeyValuePair result = null;
			KeyValuePair currentItem;
			if(lastRequestedItem != null) {
				if(EqualityComparer<KeyType>.Default.Equals(lastRequestedItem.Key, key)) {
					result = lastRequestedItem;
				}
				else if((lastRequestedItem.Next != null) && EqualityComparer<KeyType>.Default.Equals(lastRequestedItem.Key, key)) {
					result = lastRequestedItem.Next;
				}
			}
			if(result == null) {
				int currentIndex = 0;
				currentItem = head;
				while(currentItem != null) {
					if(EqualityComparer<KeyType>.Default.Equals(currentItem.Key, key)) {
						result = currentItem;
						break;
					}
					currentIndex++;
					currentItem = currentItem.Next;
				}
				if(result != null) {
					lastRequestedItem = result;
					lastRequestedItemIndex = currentIndex;
				}
			}
			return result;
		}
		private KeyValuePair GetPair(int index) {
			KeyValuePair result = null;
			if((lastRequestedItemIndex != -1)
				&& (lastRequestedItem != null)) {
				if(index == lastRequestedItemIndex) {
					result = lastRequestedItem;
				}
				if(index == lastRequestedItemIndex + 1) {
					if(lastRequestedItem == tail) {
						throw new ArgumentOutOfRangeException();
					}
					else {
						result = lastRequestedItem.Next;
					}
				}
			}
			if(result == null) {
				if(index == count - 1) {
					result = tail;
				}
				else {
					int currentIndex = 0;
					KeyValuePair currentItem = head;
					while(currentItem != null) {
						if(currentIndex == index) {
							result = currentItem;
							break;
						}
						currentIndex++;
						currentItem = currentItem.Next;
					}
				}
				if(result == null) {
					throw new ArgumentOutOfRangeException();
				}
				lastRequestedItem = result;
				lastRequestedItemIndex = index;
			}
			return result;
		}
		private bool Remove(KeyValuePair item) {
			if(item != null) {
				OnChanging(ListChangedType.ItemDeleted, item.Key, item.Value, default(ItemValueType));
				lastRequestedItemIndex = -1;
				lastRequestedItem = null;
				count--;
				KeyValuePair prev = FindPrevItem(item);
				KeyValuePair next = item.Next;
				if(prev != null)
					prev.Next = next;
				if(prev == null)
					head = next;
				if(next == null)
					tail = prev;
				OnChanged(ListChangedType.ItemDeleted, item.Key, item.Value, default(ItemValueType));
				return true;
			}
			return false;
		}
		protected Hashtable InnerItems {
			get {
				Hashtable result = new Hashtable();
				KeyValuePair currentItem = head;
				while(currentItem != null) {
					if(!result.Contains(currentItem.Key)) {
						result.Add(currentItem.Key, currentItem.Value);
					}
					else {
						result["There are duplicates for key " + currentItem.Key.ToString()] = null;
					}
					currentItem = currentItem.Next;
				}
				return result;
			}
		}
		protected virtual ItemValueType GetValueTypeDefault() {
			return default(ItemValueType);
		}
		protected virtual void OnChanged(ListChangedType listChangedType, KeyType key, ItemValueType oldValue, ItemValueType newValue) {
			keysCache = null;
			valuesCache = null;
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		protected virtual void OnChanging(ListChangedType listChangedType, KeyType key, ItemValueType oldValue, ItemValueType newValue) {
			if(Changing != null) {
				Changing(this, EventArgs.Empty);
			}
		}
		public LightDictionary() { }
		public LightDictionary(ReadByNonExistentKeyMode readByNonExistentKeyMode) {
			this.readByNonExistentKeyMode = readByNonExistentKeyMode;
		}
		public void Add(KeyType key, ItemValueType itemValue) {
			KeyValuePair item = FindPair(key);
			if(item == null) {
				AddItem(key, itemValue);
			}
			else {
				throw new DuplicatedKeyException(key, item.Value, itemValue);
			}
		}
		public void Insert(int index, KeyType key, ItemValueType value) {
			KeyValuePair item = FindPair(key);
			if(item == null) {
				AddItem(index, key, value);
			}
			else {
				throw new DuplicatedKeyException(key, item.Value, value);
			}
		}
		public bool Remove(int index) {
			return Remove(GetPair(index));
		}
		public bool Remove(KeyType key) {
			return Remove(FindPair(key));
		}
		public bool ContainsKey(KeyType key) {
			return (FindPair(key) != null);
		}
		public bool ContainsValue(ItemValueType value) {
			return (FindKeyByValue(value) != null);
		}
		public bool TryGetValue(KeyType key, out ItemValueType value) {
			value = default(ItemValueType);
			KeyValuePair pair = FindPair(key);
			if(pair != null) {
				value = pair.Value;
				return true;
			}
			return false;
		}
		public KeyType FindKeyByValue(ItemValueType value) {
			KeyValuePair currentItem = head;
			while(currentItem != null) {
				if(Object.ReferenceEquals(currentItem.Value, value)) {
					return currentItem.Key;
				}
				currentItem = currentItem.Next;
			}
			return default(KeyType);
		}
		public KeyType GetKey(int index) {
			return GetPair(index).Key;
		}
		public void Clear() {
			if(count != 0) {
				OnChanging(ListChangedType.Reset, default(KeyType), default(ItemValueType), default(ItemValueType));
				lastRequestedItemIndex = -1;
				lastRequestedItem = null;
				count = 0;
				KeyValuePair currentItem = head;
				KeyValuePair nextItem = head;
				while(nextItem != null) {
					nextItem = currentItem.Next;
					currentItem.Value = GetValueTypeDefault();
					currentItem.Next = null;
				}
				head = null;
				tail = null;
				OnChanged(ListChangedType.Reset, default(KeyType), default(ItemValueType), default(ItemValueType));
			}
		}
		public List<KeyType> GetKeys() {
			if(head == null) {
				return KeysEmptyList;
			}
			else {
				if(keysCache == null) {
					keysCache = new List<KeyType>(1);
					KeyValuePair currentItem = head;
					while(currentItem != null) {
						keysCache.Add(currentItem.Key);
						currentItem = currentItem.Next;
					}
				}
				return keysCache;
			}
		}
		public List<ItemValueType> GetValues() {
			if(head == null) {
				return new List<ItemValueType>();
			}
			else {
				if(valuesCache == null) {
					valuesCache = new List<ItemValueType>(1);
					KeyValuePair currentItem = head;
					while(currentItem != null) {
						valuesCache.Add(currentItem.Value);
						currentItem = currentItem.Next;
					}
				}
				return valuesCache;
			}
		}
		public void CopyTo(LightDictionary<KeyType, ItemValueType> dest) {
			KeyValuePair currentItem = head;
			while(currentItem != null) {
				dest[currentItem.Key] = currentItem.Value;
				currentItem = currentItem.Next;
			}
		}
		public override string ToString() {
			string result = base.ToString();
			if(Count > 0) {
				KeyValuePair currentItem = head;
				while(currentItem != null) {
					result += " " + currentItem.Key + ";";
					currentItem = currentItem.Next;
				}
				result = result.TrimEnd(';');
			}
			return result;
		}
		public ItemValueType this[int index] {
			get { return GetPair(index).Value; }
			set {
				KeyValuePair pair = GetPair(index);
				ItemValueType oldValue = pair.Value;
				OnChanging(ListChangedType.ItemChanged, pair.Key, oldValue, value);
				GetPair(index).Value = value;
				OnChanged(ListChangedType.ItemChanged, pair.Key, oldValue, value);
			}
		}
		public ItemValueType this[KeyType key] {
			get {
				KeyValuePair item = FindPair(key);
				if(item == null) {
					if(ReadByNonExistentKeyMode == ReadByNonExistentKeyMode.ThrowException) {
						throw Guard.CreateArgumentOutOfRangeException(key, "key");
					}
					else {
						return GetValueTypeDefault();
					}
				}
				else {
					return item.Value;
				}
			}
			set {
				KeyValuePair pair = FindPair(key);
				if(pair == null) {
					AddItem(key, value);
				}
				else {
					ItemValueType oldValue = pair.Value;
					OnChanging(ListChangedType.ItemChanged, key, oldValue, value);
					pair.Value = value;
					OnChanged(ListChangedType.ItemChanged, key, oldValue, value);
				}
			}
		}
		public int Count {
			get { return count; }
		}
		public ICollection<KeyType> Keys {
			get { return GetKeys(); }
		}
		public ICollection<ItemValueType> Values {
			get { return GetValues(); }
		}
		public bool AllowNullKey {
			get { return allowNullKey; }
			set { allowNullKey = value; }
		}
		public ReadByNonExistentKeyMode ReadByNonExistentKeyMode {
			get { return readByNonExistentKeyMode; }
			set { readByNonExistentKeyMode = value; }
		}
		public event EventHandler Changing;
		public event EventHandler Changed;
		#region IEnumerable<ItemValueType> Members
		public IEnumerator<ItemValueType> GetEnumerator() {
			return new ValuesEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.MethodShouldBeUsed, "IEnumerator<ItemValueType> GetEnumerator"));
		}
		#endregion
		#region IDictionary<KeyType,ItemValueType> Members
		public void Add(KeyValuePair<KeyType, ItemValueType> item) {
			AddItem(item.Key, item.Value);
		}
		public bool Contains(KeyValuePair<KeyType, ItemValueType> item) {
			return false;
		}
		public void CopyTo(KeyValuePair<KeyType, ItemValueType>[] array, int arrayIndex) {
			throw new Exception("The method or operation is not implemented.");
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(KeyValuePair<KeyType, ItemValueType> item) {
			return false;
		}
		IEnumerator<KeyValuePair<KeyType, ItemValueType>> IEnumerable<KeyValuePair<KeyType, ItemValueType>>.GetEnumerator() {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
	}
}
