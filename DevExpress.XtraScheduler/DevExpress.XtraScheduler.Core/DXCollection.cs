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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	#region DXCollectionBase
	[Serializable, System.Runtime.InteropServices.ComVisible(false)]
	public abstract class DXCollectionBase : IList, ICollection, IEnumerable {
		List<object> list;
		protected DXCollectionBase() {
			this.list = new List<object>();
		}
		protected DXCollectionBase(int capacity) {
			this.list = new List<object>(capacity);
		}
		public int Count { get { return this.InnerList.Count; } }
		[System.Runtime.InteropServices.ComVisible(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Capacity { get { return this.InnerList.Capacity; } set { this.InnerList.Capacity = value; } }
		protected internal virtual List<object> InnerList { get { return this.list; } }
		protected internal IList List { get { return this; } }
		protected internal int AddIfNotAlreadyAdded(object obj) {
			int index = List.IndexOf(obj);
			return index >= 0 ? index : List.Add(obj);
		}
		protected internal void RemoveIfAlreadyAdded(object obj) {
			int index = List.IndexOf(obj);
			if ( index >= 0 )
				List.RemoveAt(index);
		}
		public void Clear() {
			if ( !this.OnClear() )
				return;
			this.InnerList.Clear();
			this.OnClearComplete();
		}
		protected virtual bool OnClear() {
			return true;
		}
		protected virtual void OnClearComplete() {
		}
		protected virtual bool OnInsert(int index, object value) {
			return true;
		}
		protected virtual void OnInsertComplete(int index, object value) {
		}
		protected virtual bool OnRemove(int index, object value) {
			return true;
		}
		protected virtual void OnRemoveComplete(int index, object value) {
		}
		protected virtual void OnSet(int index, object oldValue, object newValue) {
		}
		protected virtual void OnSetComplete(int index, object oldValue, object newValue) {
		}
		protected virtual void OnValidate(object value) {
			if ( value == null )
				throw new ArgumentNullException("value");
		}
		public void RemoveAt(int index) {
			if ( (index < 0) || (index >= this.InnerList.Count) )
				ThrowIndexOutOfRangeException();
			object obj1 = this.InnerList[index];
			this.OnValidate(obj1);
			if ( !this.OnRemove(index, obj1) )
				return;
			this.InnerList.RemoveAt(index);
			this.OnRemoveComplete(index, obj1);
		}
		#region IEnumerable implmentation
		public IEnumerator GetEnumerator() {
			return this.InnerList.GetEnumerator();
		}
		#endregion
		#region ICollection implementation
		bool ICollection.IsSynchronized { get { return ((IList)InnerList).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((IList)InnerList).SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) {
			((ICollection)InnerList).CopyTo(array, index);
		}
		#endregion
		#region IList implementation
		bool IList.IsFixedSize { get { return ((IList)InnerList).IsFixedSize; } }
		bool IList.IsReadOnly { get { return ((IList)InnerList).IsReadOnly; } }
		object IList.this[int index] {
			get {
				if ( (index < 0) || (index >= this.InnerList.Count) )
					ThrowIndexOutOfRangeException();
				return this.InnerList[index];
			}
			set {
				if ( (index < 0) || (index >= this.InnerList.Count) )
					ThrowIndexOutOfRangeException();
				this.OnValidate(value);
				object obj = this.InnerList[index];
				this.OnSet(index, obj, value);
				this.InnerList[index] = value;
				try {
					this.OnSetComplete(index, obj, value);
				} catch ( Exception ) {
					this.InnerList[index] = obj;
					throw;
				}
			}
		}
		int IList.Add(object value) {
			this.OnValidate(value);
			if ( !this.OnInsert(this.InnerList.Count, value) )
				return -1;
			int result = this.InnerList.Count;
			this.InnerList.Add(value);
			try {
				this.OnInsertComplete(result, value);
			} catch ( Exception ) {
				this.InnerList.RemoveAt(result);
				throw;
			}
			return result;
		}
		bool IList.Contains(object value) {
			return this.InnerList.Contains(value);
		}
		int IList.IndexOf(object value) {
			return this.InnerList.IndexOf(value);
		}
		void IList.Insert(int index, object value) {
			if ( (index < 0) || (index > this.InnerList.Count) )
				ThrowIndexOutOfRangeException();
			this.OnValidate(value);
			if ( !this.OnInsert(index, value) )
				return;
			this.InnerList.Insert(index, value);
			try {
				this.OnInsertComplete(index, value);
			} catch ( Exception ) {
				this.InnerList.RemoveAt(index);
				throw;
			}
		}
		void IList.Remove(object value) {
			this.OnValidate(value);
			int index = this.InnerList.IndexOf(value);
			if ( index < 0 )
				ThrowArgumentNotFoundException();
			if ( !this.OnRemove(index, value) )
				return;
			this.InnerList.RemoveAt(index);
			this.OnRemoveComplete(index, value);
		}
		#endregion
		void ThrowIndexOutOfRangeException() {
			throw new ArgumentOutOfRangeException("index", EnvironmentGetResourceString("ArgumentOutOfRange_Index"));
		}
		void ThrowArgumentNotFoundException() {
			throw new ArgumentException(EnvironmentGetResourceString("Arg_RemoveArgNotFound"));
		}
		static string EnvironmentGetResourceString(string key) {
			MethodInfo mi = typeof(Environment).GetMethod("GetResourceString", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, new ParameterModifier[] { });
			if ( mi != null )
				return (string)mi.Invoke(null, new object[] { key });
			else
				return key;
		}
	}
	#endregion
	public abstract class SchedulerCollectionBase<T> : NotificationCollection<T>, INotifyCollectionChanged, IBindingList {
		private Dictionary<T, int> indexCache;
		private NotifyCollectionChangedEventHandler collectionChanged;
		private ListChangedEventHandler listChanged;
		protected SchedulerCollectionBase() {
		}
		protected SchedulerCollectionBase(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		private void AddIndexToCache(int index, T value) {
			if ( index != Count - 1 ) {
				List<KeyValuePair<T, int>> newIndices = new List<KeyValuePair<T, int>>(IndexCache.Count);
				foreach ( KeyValuePair<T, int> indexEntry in IndexCache ) {
					if ( indexEntry.Value >= index )
						newIndices.Add(new KeyValuePair<T, int>(indexEntry.Key, indexEntry.Value + 1));
				}
				foreach ( KeyValuePair<T, int> item in newIndices )
					IndexCache[item.Key] = item.Value;
			}
			IndexCache[value] = index;
		}
		private void RemoveIndexFromCache(int index, T value) {
			if ( index == Count )
				IndexCache.Remove(value);
			else {
				List<KeyValuePair<T, int>> newIndices = new List<KeyValuePair<T, int>>(IndexCache.Count);
				foreach ( KeyValuePair<T, int> indexEntry in IndexCache ) {
					if ( indexEntry.Value > index )
						newIndices.Add(new KeyValuePair<T, int>(indexEntry.Key, indexEntry.Value - 1));
				}
				IndexCache.Remove(value);
				foreach ( KeyValuePair<T, int> item in newIndices )
					IndexCache[item.Key] = item.Value;
			}
		}
		protected override void OnCollectionChanged(CollectionChangedEventArgs<T> e) {
			base.OnCollectionChanged(e);
			if ( IsUpdateLocked )
				return;
			if ( collectionChanged != null )
				ProcessNotifyCollectionChanged(e);
			if ( listChanged != null )
				ProcessListChanged(e);
		}
		protected override void OnInsertComplete(int index, T value) {
			if (IsIndexCacheEnabled)
				AddIndexToCache(index, value);
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			if (IsIndexCacheEnabled)
				RemoveIndexFromCache(index, value);
		}
		protected override void OnClearComplete() {
			if (IsIndexCacheEnabled)
				IndexCache.Clear();
			base.OnClearComplete();
		}
		protected virtual void ProcessNotifyCollectionChanged(CollectionChangedEventArgs<T> e) {
			if ( e.Action == CollectionChangedAction.Add )
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.Element));
			if ( e.Action == CollectionChangedAction.Remove )
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.Element));
			if ( e.Action == CollectionChangedAction.Clear || e.Action == CollectionChangedAction.EndBatchUpdate )
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		protected virtual void ProcessListChanged(CollectionChangedEventArgs<T> e) {
			if ( e.Action == CollectionChangedAction.Add )
				listChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, IndexOf(e.Element)));
			if ( e.Action == CollectionChangedAction.Remove )
				listChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, IndexOf(e.Element)));
			if ( e.Action == CollectionChangedAction.Clear || e.Action == CollectionChangedAction.EndBatchUpdate )
				listChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		public new int IndexOf(T item) {
			if (!IsIndexCacheEnabled)
				return base.IndexOf(item);
			int index = -1;
			if (IndexCache.ContainsKey(item))
				index = IndexCache[item];
			return index;
		}
		protected abstract bool IsIndexCacheEnabled { get; }
		protected Dictionary<T, int> IndexCache {
			get { return indexCache ?? (indexCache = new Dictionary<T, int>()); }
		}
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}
		event ListChangedEventHandler IBindingList.ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}
		#region IBindingList members
		void IBindingList.AddIndex(PropertyDescriptor property) {
		}
		object IBindingList.AddNew() {
			throw new NotSupportedException();
		}
		bool IBindingList.AllowEdit {
			get { return true; }
		}
		bool IBindingList.AllowNew {
			get { return false; }
		}
		bool IBindingList.AllowRemove {
			get { return true; }
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		bool IBindingList.IsSorted {
			get { throw new NotSupportedException(); }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
		}
		void IBindingList.RemoveSort() {
			throw new NotSupportedException();
		}
		ListSortDirection IBindingList.SortDirection {
			get { throw new NotSupportedException(); }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { throw new NotSupportedException(); }
		}
		bool IBindingList.SupportsChangeNotification {
			get { return true; }
		}
		bool IBindingList.SupportsSearching {
			get { return false; }
		}
		bool IBindingList.SupportsSorting {
			get { return false; }
		}
		#endregion
	}
}
