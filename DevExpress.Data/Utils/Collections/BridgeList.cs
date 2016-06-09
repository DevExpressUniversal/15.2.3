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
using System.Collections;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
namespace DevExpress.Utils {
	class ListEnumerator<T> : IEnumerator<T> {
		int index = -1;
		IList<T> list;
		public ListEnumerator(IList<T> list) {
			this.list = list;
		}
		#region IEnumerator Members
		object IEnumerator.Current { get { return ((IEnumerator<T>)this).Current; } }
		bool IEnumerator.MoveNext() {
			index++;
			return index < list.Count;
		}
		void IEnumerator.Reset() {
			index = -1;
		}
		#endregion
		#region IEnumerator<T> Members
		T IEnumerator<T>.Current { get { return list[index]; } }
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			list = null;
		}
		#endregion
	}
	public abstract class BridgeList<T, Key> : IList<T>, IList {
#if DEBUGTEST
		public IList<Key> DebugTestKeys { get { return keys; } }
#endif
		protected readonly IList<Key> keys;
		IList<T> List { get { return (IList<T>)this; } }
		protected BridgeList(IList<Key> keys) {
			this.keys = keys;
		}
		protected abstract T GetItemByKey(Key key, int index);
		#region IList<T> Members
		int IList<T>.IndexOf(T item) {
			for(int i = 0; i < keys.Count; i++) {
				if(object.Equals(item, List[i]))
					return i;
			}
			return -1;
		}
		void IList<T>.Insert(int index, T item) {
			InsertCore(index, item);
		}
		protected virtual void InsertCore(int index, T item) {
			throw new NotImplementedException();
		}
		void IList<T>.RemoveAt(int index) {
			RemoveAtCore(index);
		}
		T IList<T>.this[int index] {
			get { return GetItemByKey(keys[index], index); }
			set { throw new NotImplementedException(); }
		}
		#endregion
		#region ICollection<T> Members
		void ICollection<T>.Add(T item) {
			AddCore(item);
		}
		protected virtual void AddCore(T item) {
			throw new NotImplementedException();
		}
		void ICollection<T>.Clear() {
			ClearCore();
		}
		protected virtual void ClearCore() {
		}
		bool ICollection<T>.Contains(T item) {
			return List.IndexOf(item) >= 0;
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			CopyTo(array, arrayIndex);
		}
		void CopyTo(Array array, int arrayIndex) {
			if((array != null) && (array.Rank != 1)) {
				throw new ArgumentException("array.Rank");
			}
			try {
				Array.Copy(keys.ToArray(), 0, array, arrayIndex, keys.Count);
			} catch(ArrayTypeMismatchException) {
				throw new ArgumentException("Invalid array type");
			}
		}
		int ICollection<T>.Count { get { return keys.Count; } }
		bool ICollection<T>.IsReadOnly { get { return IsReadOnlyCore(); } }
		protected virtual bool IsReadOnlyCore() {
			return true;
		}
		bool ICollection<T>.Remove(T item) {
			throw new NotImplementedException();
		}
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return new ListEnumerator<T>(this);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return List.GetEnumerator();
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			throw new NotImplementedException();
		}
		void IList.Clear() {
			List.Clear();
		}
		bool IList.Contains(object value) {
			return List.Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return List.IndexOf((T)value);
		}
		void IList.Insert(int index, object value) {
			throw new NotImplementedException();
		}
		bool IList.IsFixedSize { get { return IsFixedSize; } }
		protected virtual bool IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return List.IsReadOnly; } }
		void IList.Remove(object value) {
			throw new NotImplementedException();
		}
		void IList.RemoveAt(int index) {
			RemoveAtCore(index);
		}
		protected virtual void RemoveAtCore(int index) {
			throw new NotImplementedException();
		}
		object IList.this[int index] {
			get { return List[index]; }
			set { throw new NotImplementedException(); }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			CopyTo(array, index);
		}
		int ICollection.Count { get { return List.Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
	}
	public class SimpleBridgeList<T, Key> : BridgeList<T, Key> where T: class {
		readonly Func<Key, T> cast;
		public SimpleBridgeList(IList<Key> keys, Func<Key, T> cast) 
			: base(keys) {
			this.cast = cast;
		}
	static Func<Key, T> castFunc = key => key as T;
		public SimpleBridgeList(IList<Key> keys)
			: this(keys, castFunc) {			
		}
		protected override T GetItemByKey(Key key, int index) {
			return cast(key);
		}
	}
	public class CastList<T, TKey> : SimpleBridgeList<T, TKey> 
		where TKey : class, T
		where T : class {
		public CastList(IList<TKey> keys)
			: base(keys) {
		}
		protected override void InsertCore(int index, T item) {
			keys.Insert(index, (TKey)item);
		}
		protected override void RemoveAtCore(int index) {
			keys.RemoveAt(index);
		}
		protected override void AddCore(T item) {
			keys.Add((TKey)item);
		}
		protected override void ClearCore() {
			keys.Clear();
		}
		protected override bool IsReadOnlyCore() {
			return keys.IsReadOnly;
		}
	}
	public class SimpleEnumerableBridge<T, Key> : IEnumerable<T> where T : class {
		IEnumerable<Key> keys;
		 Func<Key, T> cast;
	static Func<Key, T> defaultCast = key => key as T;
		 public SimpleEnumerableBridge(IEnumerable<Key> keys)
			: this(keys, defaultCast) {
		}
		protected SimpleEnumerableBridge(IEnumerable<Key> keys,  Func<Key, T> cast) {
			this.keys = keys;
			this.cast = cast;
		}
		public IEnumerator<T> GetEnumerator() {
			return new EnumerableBridgeEnumerator<T, Key>(keys.GetEnumerator(), cast);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}
	class EnumerableBridgeEnumerator<T, Key> : IEnumerator<T> {
		IEnumerator<Key> keysEnumerator;
		Func<Key, T> cast;
		public EnumerableBridgeEnumerator(IEnumerator<Key> keysEnumerator, Func<Key, T> cast) {
			this.keysEnumerator = keysEnumerator;
			this.cast = cast;
		}
		public T Current { get { return cast(keysEnumerator.Current); } }
		public void Dispose() {
			keysEnumerator.Dispose();
		}
		object IEnumerator.Current { get { return keysEnumerator.Current; } }
		public bool MoveNext() {
			return keysEnumerator.MoveNext();
		}
		public void Reset() {
			keysEnumerator.Reset();
		}
	}
	public abstract class OnDemandList<T> : IList<T>, IList {
		T[] store;
		IList<T> List { get { return (IList<T>)this; } }
		protected OnDemandList(int itemsCount) {
			store = new T[itemsCount];
		}
		public T GetRealValue(int index) {
			return store[index];
		}
		#region IList<T> Members
		int IList<T>.IndexOf(T item) {
			for(int i = 0; i < store.Length; i++) {
				if(object.Equals(item, store[i]))
					return i;
			}
			return -1;
		}
		void IList<T>.Insert(int index, T item) {
			throw new NotImplementedException();
		}
		void IList<T>.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		T IList<T>.this[int index] {
			get {
				if(store[index] == null)
					store[index] = CreateItem(index);
				return store[index];
			}
			set { throw new NotImplementedException(); }
		}
		#endregion
		#region ICollection<T> Members
		void ICollection<T>.Add(T item) {
			throw new NotImplementedException();
		}
		void ICollection<T>.Clear() {
			throw new NotImplementedException();
		}
		bool ICollection<T>.Contains(T item) {
			return List.IndexOf(item) >= 0;
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			throw new NotImplementedException();
		}
		int ICollection<T>.Count { get { return store.Length; } }
		bool ICollection<T>.IsReadOnly { get { return true; } }
		bool ICollection<T>.Remove(T item) {
			throw new NotImplementedException();
		}
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return new ListEnumerator<T>(this);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return List.GetEnumerator();
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			throw new NotImplementedException();
		}
		void IList.Clear() {
			throw new NotImplementedException();
		}
		bool IList.Contains(object value) {
			return List.Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return List.IndexOf((T)value);
		}
		void IList.Insert(int index, object value) {
			throw new NotImplementedException();
		}
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return List.IsReadOnly; } }
		void IList.Remove(object value) {
			throw new NotImplementedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		object IList.this[int index] {
			get { return List[index]; }
			set { throw new NotImplementedException(); }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			throw new NotImplementedException();
		}
		int ICollection.Count { get { return List.Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
		protected abstract T CreateItem(int listIndex);
	}
	public class OffsetListAdapter<T> : IList<T>, IList {
		IList<T> source;
		int offset;
		int count;
		IList<T> List { get { return (IList<T>)this; } }
		public OffsetListAdapter(IList<T> source, int offset, int count) {
			if(!(source is IList) || !((IList)source).IsFixedSize) {
				throw new NotSupportedException();
			}
			if(offset < 0 || offset + count > source.Count)
				throw new ArgumentException();
			this.source = source;
			this.offset = offset;
			this.count = count;
		}
		#region IList<T> Members
		public int IndexOf(T item) {
			return ((IList)List).IndexOf(item);
		}
		public void Insert(int index, T item) {
			throw new NotSupportedException();
		}
		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public T this[int index] {
			get {
				return source[index + offset];
			}
			set {
				throw new NotSupportedException();
			}
		}
		#endregion
		#region ICollection<T> Members
		public void Add(T item) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public bool Contains(T item) {
			return ((IList)List).Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			throw new NotSupportedException();
		}
		public int Count { get { return count; } }
		public bool IsReadOnly { get { return true; } }
		public bool Remove(T item) {
			throw new NotSupportedException();
		}
		#endregion
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return new ListEnumerator<T>(this);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return List.GetEnumerator();
		}
		#endregion
		#region IList Members
		public int Add(object value) {
			throw new NotSupportedException();
		}
		public bool Contains(object value) {
			int index = source.IndexOf((T)value);
			return ContainsIndex(index);
		}
		public int IndexOf(object value) {
			int index = source.IndexOf((T)value);
			return ContainsIndex(index) ? index - offset : - 1;
		}
		bool ContainsIndex(int index) {
			return (index >= 0) && (index >= offset) && (index < offset + count);
		}
		public void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		public bool IsFixedSize { get { return true; } }
		public void Remove(object value) {
			throw new NotSupportedException();
		}
		object IList.this[int index] { get { return List[index]; } set { throw new NotSupportedException(); } }
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return null; ; }
		}
		#endregion
	}
}
