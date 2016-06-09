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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Internal;
using DevExpress.Utils;
namespace DevExpress.Utils {
	public interface IAssignableCollection {
		void Clear();
		void Assign(IAssignableCollection source);
	}
	#region DXCollectionUniquenessProviderType
	public enum DXCollectionUniquenessProviderType {
		None,
		MinimizeMemoryUsage,
		MaximizePerformance
	}
	#endregion
	#region DXCollectionBase<T>
	[Serializable, System.Runtime.InteropServices.ComVisible(false)]
	public class DXCollectionBase<T> : IList<T>, IList {
		readonly List<T> list;
		internal DXCollectionUniquenessProvider<T> uniquenessProvider;
		public DXCollectionBase()
			: this(DXCollectionUniquenessProviderType.MinimizeMemoryUsage) {
		}
		protected DXCollectionBase(DXCollectionUniquenessProviderType uniquenessProviderType) {
			this.list = new List<T>();
			this.UniquenessProviderType = uniquenessProviderType;
		}
		public DXCollectionBase(int capacity)
			: this(capacity, DXCollectionUniquenessProviderType.MinimizeMemoryUsage) {
		}
		protected DXCollectionBase(int capacity, DXCollectionUniquenessProviderType uniquenessProviderType) {
			this.list = new List<T>(capacity);
			this.UniquenessProviderType = uniquenessProviderType;
		}
		#region Properties
		public int Count { get { return this.InnerList.Count; } }
		[System.Runtime.InteropServices.ComVisible(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Capacity { get { return this.list.Capacity; } set { this.list.Capacity = value; } }
		protected virtual IList<T> InnerList { get { return this.list; } }
		protected internal IList<T> List { get { return this; } }
		protected internal DXCollectionUniquenessProvider<T> UniquenessProvider { get { return uniquenessProvider; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DXCollectionUniquenessProviderType UniquenessProviderType {
			get { return uniquenessProvider.Type; }
			set {
				if (this.InnerList != null && Count > 0)
					throw new ArgumentException("Can't change uniqueness type for non-empty collection");
				this.uniquenessProvider = CreateUniquenessProvider(value);
			}
		}
		#endregion
		protected internal DXCollectionUniquenessProvider<T> CreateUniquenessProvider(DXCollectionUniquenessProviderType strategy) {
			switch (strategy) {
				case DXCollectionUniquenessProviderType.MaximizePerformance:
					return new DictionaryUniquenessProvider<T>(this);
				case DXCollectionUniquenessProviderType.MinimizeMemoryUsage:
					return new SimpleUniquenessProvider<T>(this);
				case DXCollectionUniquenessProviderType.None:
				default:
					return new EmptyUniquenessProvider<T>();
			}
		}
		protected internal virtual int AddIfNotAlreadyAdded(T obj) {
			int index = uniquenessProvider.LookupObjectIndex(obj);
			return index >= 0 ? index : AddCore(obj);
		}
		protected internal int BinarySearch(T item, IComparer<T> comparer) {
			return list.BinarySearch(item, comparer);
		}
		protected virtual void InsertIfNotAlreadyInserted(int index, T obj) {
			if (uniquenessProvider.LookupObject(obj))
				throw new ArgumentException("obj");
			InsertCore(index, obj);
		}
		protected internal virtual bool RemoveIfAlreadyAdded(T obj) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			int index = List.IndexOf(obj);
			if (index >= 0)
				return RemoveAtCore(index);
			else
				return false;
		}
		public void Clear() {
			if (!this.OnClear())
				return;
			this.InnerList.Clear();
			this.OnClearComplete();
		}
		protected virtual bool OnClear() {
			return true;
		}
		protected virtual void OnClearComplete() {
			this.uniquenessProvider.OnClearComplete();
		}
		protected virtual bool OnInsert(int index, T value) {
			return true;
		}
		protected virtual void OnInsertComplete(int index, T value) {
			this.uniquenessProvider.OnInsertComplete(value);
		}
		protected virtual bool OnRemove(int index, T value) {
			return true;
		}
		protected virtual void OnRemoveComplete(int index, T value) {
			this.uniquenessProvider.OnRemoveComplete(value);
		}
		protected virtual bool OnSet(int index, T oldValue, T newValue) {
			return !uniquenessProvider.LookupObject(newValue);
		}
		protected virtual void OnSetComplete(int index, T oldValue, T newValue) {
			this.uniquenessProvider.OnSetComplete(oldValue, newValue);
		}
		protected virtual void OnValidate(T value) {
			if (value == null)
				throw new ArgumentNullException("value");
		}
		protected internal virtual bool RemoveAtCore(int index) {
			if ((index < 0) || (index >= this.InnerList.Count))
				ThrowIndexOutOfRangeException();
			T obj = this.InnerList[index];
			this.OnValidate(obj);
			if (!this.OnRemove(index, obj))
				return false;
			this.InnerList.RemoveAt(index);
			this.OnRemoveComplete(index, obj);
			return true;
		}
		public void RemoveAt(int index) {
			RemoveAtCore(index);
		}
		#region IEnumerable implmentation
		IEnumerator IEnumerable.GetEnumerator() {
			return this.InnerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable<T> implmentation
		public IEnumerator<T> GetEnumerator() {
			return this.InnerList.GetEnumerator();
		}
		#endregion
		#region ICollection implementation
		bool ICollection.IsSynchronized { get { return ((IList)this.InnerList).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((IList)this.InnerList).SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) {
			((ICollection)this.InnerList).CopyTo(array, index);
		}
		#endregion
		#region ICollection<T> implementation
		bool ICollection<T>.IsReadOnly { get { return IsReadOnly; } }
		void ICollection<T>.Add(T value) {
			AddIfNotAlreadyAdded(value);
		}
		protected virtual bool IsReadOnly { get { return InnerList.IsReadOnly; } }
		public virtual bool Remove(T value) {
			return RemoveIfAlreadyAdded(value);
		}
		public void CopyTo(T[] array, int index) {
			this.InnerList.CopyTo(array, index);
		}
		#endregion
		protected virtual T GetItem(int index) {
			if ((index < 0) || (index >= this.InnerList.Count))
				ThrowIndexOutOfRangeException();
			return this.InnerList[index];
		}
		protected virtual void SetItem(int index, T value) {
			if ((index < 0) || (index >= this.InnerList.Count))
				ThrowIndexOutOfRangeException();
			this.OnValidate(value);
			T obj = this.InnerList[index];
			if (!this.OnSet(index, obj, value))
				return;
			this.InnerList[index] = value;
			try {
				this.OnSetComplete(index, obj, value);
			} catch (Exception) {
				this.InnerList[index] = obj;
				throw;
			}
		}
		protected virtual int AddCore(T value) {
			this.OnValidate(value);
			if (!this.OnInsert(this.InnerList.Count, value))
				return -1;
			int result = InnerList.Count;
			this.InnerList.Add(value);
			try {
				this.OnInsertComplete(result, value);
			} catch (Exception) {
				this.InnerList.RemoveAt(result);
				throw;
			}
			return result;
		}
		protected internal virtual void InsertCore(int index, T value) {
			if ((index < 0) || (index > this.InnerList.Count))
				ThrowIndexOutOfRangeException();
			this.OnValidate(value);
			if (!this.OnInsert(index, value))
				return;
			this.InnerList.Insert(index, value);
			try {
				this.OnInsertComplete(index, value);
			} catch (Exception) {
				this.InnerList.RemoveAt(index);
				throw;
			}
		}
		protected internal virtual void RemoveCore(T value) {
			this.OnValidate(value);
			int index = uniquenessProvider.LookupObjectIndex(value);
			if (index < 0)
				ThrowArgumentNotFoundException();
			if (!this.OnRemove(index, value))
				return;
			this.InnerList.RemoveAt(index);
			this.OnRemoveComplete(index, value);
		}
		public virtual int Add(T value) {
			return AddIfNotAlreadyAdded(value);
		}
		#region IList<T> implementation
		T IList<T>.this[int index] { get { return GetItem(index); } set { SetItem(index, value); } }
		public void Insert(int index, T value) {
			InsertIfNotAlreadyInserted(index, value);
		}
		public virtual bool Contains(T value) {
			return this.InnerList.Contains(value);
		}
		public int IndexOf(T value) {
			return this.InnerList.IndexOf(value);
		}
		#endregion
		#region IList implementation
		bool IList.IsFixedSize { get { return ((IList)this.InnerList).IsFixedSize; } }
		bool IList.IsReadOnly { get { return IsReadOnly; } }
		object IList.this[int index] { get { return GetItem(index); } set { SetItem(index, (T)value); } }
		int IList.Add(object value) {
			return AddIfNotAlreadyAdded((T)value);
		}
		void IList.Insert(int index, object value) {
			InsertIfNotAlreadyInserted(index, (T)value);
		}
		void IList.Remove(object value) {
			RemoveIfAlreadyAdded((T)value);
		}
		bool IList.Contains(object value) {
			return ((IList)this.InnerList).Contains(value);
		}
		int IList.IndexOf(object value) {
			return ((IList)this.InnerList).IndexOf(value);
		}
		#endregion
		void ThrowIndexOutOfRangeException() {
			throw new ArgumentOutOfRangeException("index", EnvironmentGetResourceString("ArgumentOutOfRange_Index"));
		}
		void ThrowArgumentNotFoundException() {
			throw new ArgumentException(EnvironmentGetResourceString("Arg_RemoveArgNotFound"));
		}
		static string EnvironmentGetResourceString(string key) {
#if !SILVERLIGHT && !DXRESTRICTED
			MethodInfo mi = typeof(Environment).GetMethod("GetResourceString", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, new ParameterModifier[] { });
			if (mi != null)
				return (string)mi.Invoke(null, new object[] { key });
			else
#endif
				return key;
		}
		public void ForEach(Action<T> action) {
			Guard.ArgumentNotNull(action, "action");
			int count = this.Count;
			for (int i = 0; i < count; i++)
				action(InnerList[i]);
		}
		public virtual void AddRange(ICollection collection) {
			AddRangeCore(collection);
		}
		protected virtual void AddRangeCore(ICollection collection) {
			foreach (T item in collection)
				Add(item);
		}
		public virtual void Sort(IComparer<T> comparer) {
			list.Sort(comparer);
		}
		public T Find(Predicate<T> match) {
			Guard.ArgumentNotNull(match, "match");
			int count = Count;
			for (int i = 0; i < count; i++) {
				T item = list[i];
				if (match(item))
					return item;
			}
			return default(T);
		}
		protected internal virtual void FindAllCore(DXCollectionBase<T> result, Predicate<T> match) {
			Guard.ArgumentNotNull(match, "match");
			int count = Count;
			for (int i = 0; i < count; i++) {
				T item = list[i];
				if (match(item))
					result.Add(item);
			}
		}
		public virtual T[] ToArray() {
			return list.ToArray();
		}
		internal T InvokeGetItem(int index) {
			return GetItem(index);
		}
		internal IList<T> GetInnerList() {
			return InnerList;
		}
#if DEBUGTEST
		public void CallAddRangeCore(ICollection collection) {
			AddRangeCore(collection);
		}
#endif
	}
	#endregion
	#region DXCollectionWithSetItem<T>
	public class DXCollectionWithSetItem<T> : DXCollectionBase<T> {
		public DXCollectionWithSetItem()
			: base() {
		}
		protected DXCollectionWithSetItem(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		public virtual T this[int index] { get { return GetItem(index); } set { SetItem(index, value); } }
	}
	#endregion
	#region DXCollection<T>
	public class DXCollection<T> : DXCollectionBase<T> {
		public DXCollection()
			: base() {
		}
		protected DXCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		public virtual T this[int index] { get { return GetItem(index); } }
		protected override void SetItem(int index, T value) {
			throw new InvalidOperationException("Can't set item for this collection. This collection is read-only.");
		}
	}
	#endregion
	#region DXNamedItemCollection<T> (abstract class)
	public abstract class DXNamedItemCollection<T> : DXCollection<T> {
		readonly Dictionary<string, T> nameHash = new Dictionary<string, T>();
		protected DXNamedItemCollection()
			: base() {
		}
		protected DXNamedItemCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		protected internal Dictionary<string, T> NameHash { get { return nameHash; } }
		public T this[string name] {
			get {
				T result;
				if (NameHash.TryGetValue(name, out result))
					return result;
				else
					return default(T);
			}
		}
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			string name = GetItemName(value);
			if (NameHash.ContainsKey(name))
				NameHash[name] = value;
			else
				NameHash.Add(name, value);
		}
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			string name = GetItemName(value);
			if (NameHash.ContainsKey(name))
				NameHash.Remove(name);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			NameHash.Clear();
		}
		protected abstract string GetItemName(T item);
	}
	#endregion
	#region NotificationCollection<T>
	public class NotificationCollection<T> : DXCollection<T>, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		readonly BatchUpdateHelper batchUpdateHelper;
		bool changed;
		#endregion
		public NotificationCollection()
			: base() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		protected NotificationCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		#endregion
		#region Events
		EventHandler onBeginBatchUpdate;
		EventHandler onEndBatchUpdate;
		EventHandler onCancelBatchUpdate;
		CollectionChangingEventHandler<T> onCollectionChanging;
		CollectionChangedEventHandler<T> onCollectionChanged;
		public event CollectionChangedEventHandler<T> CollectionChanged { add { onCollectionChanged += value; } remove { onCollectionChanged -= value; } }
		public event CollectionChangingEventHandler<T> CollectionChanging { add { onCollectionChanging += value; } remove { onCollectionChanging -= value; } }
		internal event EventHandler BeginBatchUpdate { add { onBeginBatchUpdate += value; } remove { onBeginBatchUpdate -= value; } }
		internal event EventHandler EndBatchUpdate { add { onEndBatchUpdate += value; } remove { onEndBatchUpdate -= value; } }
		internal event EventHandler CancelBatchUpdate { add { onCancelBatchUpdate += value; } remove { onCancelBatchUpdate -= value; } }
		protected internal virtual void OnCollectionChanged(CollectionChangedEventArgs<T> e) {
			if (IsUpdateLocked)
				return;
			RaiseCollectionChanged(e);
		}
#if DEBUGTEST
		public void CallRaiseCollectionChanged(CollectionChangedEventArgs<T> e) {
			RaiseCollectionChanged(e);
		}
		public void CallRaiseCollectionChanging(CollectionChangingEventArgs<T> e) {
			RaiseCollectionChanging(e);
		}
#endif
		protected virtual void RaiseCollectionChanged(CollectionChangedEventArgs<T> e) {
			if (onCollectionChanged != null)
				onCollectionChanged(this, e);
		}
		protected internal virtual void OnCollectionChanging(CollectionChangingEventArgs<T> e) {
			if (IsUpdateLocked) {
				RaiseCollectionChanging(e);
				if (!e.Cancel)
					this.changed = true;
				return;
			}
			RaiseCollectionChanging(e);
		}
		protected virtual void RaiseCollectionChanging(CollectionChangingEventArgs<T> e) {
			if (onCollectionChanging != null)
				onCollectionChanging(this, e);
		}
		protected internal void OnFirstBeginUpdate() {
			RaiseBeginBatchUpdate();
			this.changed = false;
		}
		protected virtual void OnLastEndUpdate() {
			RaiseEndBatchUpdate();
			if (this.changed)
				OnCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChangedAction.EndBatchUpdate, default(T)));
		}
		protected internal virtual void OnLastCancelUpdate() {
			RaiseCancelBatchUpdate();
		}
		protected internal virtual void RaiseBeginBatchUpdate() {
			if (onBeginBatchUpdate != null)
				onBeginBatchUpdate(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseEndBatchUpdate() {
			if (onEndBatchUpdate != null)
				onEndBatchUpdate(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseCancelBatchUpdate() {
			if (onCancelBatchUpdate != null)
				onCancelBatchUpdate(this, EventArgs.Empty);
		}
		#endregion
		public override void AddRange(ICollection collection) {
			if (collection == null || collection.Count <= 0)
				return;
			BeginUpdate();
			try {
				AddRangeCore(collection);
			} finally {
				EndUpdate();
			}
		}
		protected override bool OnInsert(int index, T value) {
			CollectionChangingEventArgs<T> args = new CollectionChangingEventArgs<T>(CollectionChangedAction.Add, value);
			OnCollectionChanging(args);
			if (args.Cancel)
				return false;
			return base.OnInsert(index, value);
		}
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			OnCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChangedAction.Add, value));
		}
		protected override bool OnRemove(int index, T value) {
			CollectionChangingEventArgs<T> args = new CollectionChangingEventArgs<T>(CollectionChangedAction.Remove, value);
			OnCollectionChanging(args);
			if (args.Cancel)
				return false;
			return base.OnRemove(index, value);
		}
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			OnCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChangedAction.Remove, value));
		}
		protected override bool OnClear() {
			if (Count <= 0)
				return false;
			CollectionChangingEventArgs<T> args = new CollectionChangingEventArgs<T>(CollectionChangedAction.Clear, default(T));
			OnCollectionChanging(args);
			if (args.Cancel)
				return false;
			return base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChangedAction.Clear, default(T)));
		}
	}
	#endregion
	#region NamedItemNotificationCollection<T> (abstract class)
	public abstract class NamedItemNotificationCollection<T> : NotificationCollection<T> {
		readonly Dictionary<string, T> nameHash = new Dictionary<string, T>();
		protected NamedItemNotificationCollection()
			: base() {
		}
		protected NamedItemNotificationCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		protected internal Dictionary<string, T> NameHash { get { return nameHash; } }
		public T this[string name] {
			get {
				T result;
				if (NameHash.TryGetValue(name, out result))
					return result;
				else
					return default(T);
			}
		}
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			string name = GetItemName(value);
			if (NameHash.ContainsKey(name))
				NameHash[name] = value;
			else
				NameHash.Add(name, value);
		}
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			string name = GetItemName(value);
			if (NameHash.ContainsKey(name))
				NameHash.Remove(name);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			NameHash.Clear();
		}
		protected abstract string GetItemName(T item);
	}
	#endregion
	#region CollectionChangedAction
	public enum CollectionChangedAction {
		Add,
		Remove,
		Changed,
		Clear,
		EndBatchUpdate
	}
	#endregion
	#region CollectionChangedEventHandler<T>
	public delegate void CollectionChangedEventHandler<T>(object sender, CollectionChangedEventArgs<T> e);
	#endregion
	#region CollectionChangedEventArgs<T>
	public class CollectionChangedEventArgs<T> : EventArgs {
		readonly CollectionChangedAction action;
		readonly T element;
		public CollectionChangedEventArgs(CollectionChangedAction action, T element) {
			this.action = action;
			this.element = element;
		}
		public CollectionChangedAction Action { get { return action; } }
		public T Element { get { return element; } }
	}
	#endregion
	#region CollectionChangingEventHandler<T>
	public delegate void CollectionChangingEventHandler<T>(object sender, CollectionChangingEventArgs<T> e);
	#endregion
	#region CollectionChangingEventArgs<T>
	public class CollectionChangingEventArgs<T> : CollectionChangedEventArgs<T> {
		bool cancel;
		T newValue;
		T oldValue;
		string propertyName = String.Empty;
		public CollectionChangingEventArgs(CollectionChangedAction action, T element)
			: base(action, element) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public T NewValue { get { return newValue; } set { newValue = value; } }
		public T OldValue { get { return oldValue; } set { oldValue = value; } }
		public string PropertyName { get { return propertyName; } set { propertyName = value; } }
	}
	#endregion
	#region ISupportObjectChanging
	public interface ISupportObjectChanging {
		event CancelEventHandler Changing;
	}
	#endregion
	#region ISupportObjectChanged
	public interface ISupportObjectChanged {
		event EventHandler Changed;
	}
	#endregion
	#region NotificationCollectionChangedListenerBase<T> (abstract class)
	public abstract class NotificationCollectionChangedListenerBase<T> : IDisposable {
		#region Fields
		NotificationCollection<T> collection;
		bool isDisposed;
		#endregion
		protected NotificationCollectionChangedListenerBase(NotificationCollection<T> collection) {
			Guard.ArgumentNotNull(collection, "collection");
			this.collection = collection;
			SubscribeCollectionEvents();
			SubscribeExistingObjectsEvents();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal NotificationCollection<T> Collection { get { return collection; } }
		#endregion
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#region Changing
		CancelEventHandler onChanging;
		public event CancelEventHandler Changing { add { onChanging += value; } remove { onChanging -= value; } }
		protected internal virtual bool RaiseChanging() {
			if (onChanging != null) {
				CancelEventArgs args = new CancelEventArgs();
				onChanging(this, args);
				return args.Cancel;
			}
			return false;
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (collection != null) {
					UnsubscribeExistingObjectsEvents();
					UnsubscribeCollectionEvents();
					collection = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region SubscribeExistingObjectsEvents
		protected internal virtual void SubscribeExistingObjectsEvents() {
			int count = collection.Count;
			for (int i = 0; i < count; i++)
				SubscribeObjectEvents(collection.List[i]);
		}
		#endregion
		#region UnsubscribeExistingObjectsEvents
		protected internal virtual void UnsubscribeExistingObjectsEvents() {
			int count = collection.Count;
			for (int i = 0; i < count; i++)
				UnsubscribeObjectEvents(collection.List[i]);
		}
		#endregion
		#region SubscribeCollectionEvents
		protected internal virtual void SubscribeCollectionEvents() {
			collection.CollectionChanging += new CollectionChangingEventHandler<T>(OnCollectionChanging);
			collection.CollectionChanged += new CollectionChangedEventHandler<T>(OnCollectionChanged);
			collection.BeginBatchUpdate += new EventHandler(OnCollectionBeginBatchUpdate);
			collection.EndBatchUpdate += new EventHandler(OnCollectionEndBatchUpdate);
			collection.CancelBatchUpdate += new EventHandler(OnCollectionCancelBatchUpdate);
		}
		#endregion
		#region UnsubscribeCollectionEvents
		protected internal virtual void UnsubscribeCollectionEvents() {
			collection.CollectionChanging -= new CollectionChangingEventHandler<T>(OnCollectionChanging);
			collection.CollectionChanged -= new CollectionChangedEventHandler<T>(OnCollectionChanged);
			collection.BeginBatchUpdate -= new EventHandler(OnCollectionBeginBatchUpdate);
			collection.EndBatchUpdate -= new EventHandler(OnCollectionEndBatchUpdate);
			collection.CancelBatchUpdate -= new EventHandler(OnCollectionCancelBatchUpdate);
		}
		#endregion
		protected internal virtual void OnCollectionChanging(object sender, CollectionChangingEventArgs<T> e) {
			e.Cancel = RaiseChanging();
			if (e.Cancel)
				return;
			if (e.Action == CollectionChangedAction.Clear)
				UnsubscribeExistingObjectsEvents();
		}
		protected internal virtual void OnCollectionChanged(object sender, CollectionChangedEventArgs<T> e) {
			switch (e.Action) {
				case CollectionChangedAction.Add:
					SubscribeObjectEvents(e.Element);
					RaiseChanged();
					break;
				case CollectionChangedAction.Remove:
					UnsubscribeObjectEvents(e.Element);
					RaiseChanged();
					break;
				case CollectionChangedAction.Clear:
					RaiseChanged();
					break;
			}
		}
		protected internal virtual void OnCollectionBeginBatchUpdate(object sender, EventArgs e) {
			UnsubscribeExistingObjectsEvents();
		}
		protected internal virtual void OnCollectionCancelBatchUpdate(object sender, EventArgs e) {
			SubscribeExistingObjectsEvents();
		}
		protected internal virtual void OnCollectionEndBatchUpdate(object sender, EventArgs e) {
			SubscribeExistingObjectsEvents();
			RaiseChanged();
		}
#if DEBUGTEST
		public void CallSubscribeObjectEvents(T obj) {
			SubscribeObjectEvents(obj);
		}
		public void CallUnsubscribeObjectEvents(T obj) {
			UnsubscribeObjectEvents(obj);
		}
#endif
		protected abstract void SubscribeObjectEvents(T obj);
		protected abstract void UnsubscribeObjectEvents(T obj);
	}
	#endregion
	#region NotificationCollectionChangedListener<T>
	public class NotificationCollectionChangedListener<T> : NotificationCollectionChangedListenerBase<T> {
		public NotificationCollectionChangedListener(NotificationCollection<T> collection)
			: base(collection) {
		}
		#region SubscribeObjectEvents
		protected override void SubscribeObjectEvents(T obj) {
			ISupportObjectChanged supportObjectChanged = obj as ISupportObjectChanged;
			if (supportObjectChanged != null)
				supportObjectChanged.Changed += new EventHandler(OnObjectChanged);
			ISupportObjectChanging supportObjectChanging = obj as ISupportObjectChanging;
			if (supportObjectChanging != null)
				supportObjectChanging.Changing += new CancelEventHandler(OnObjectChanging);
		}
		#endregion
		#region UnsubscribeObjectEvents
		protected override void UnsubscribeObjectEvents(T obj) {
			ISupportObjectChanged supportObjectChanged = obj as ISupportObjectChanged;
			if (supportObjectChanged != null)
				supportObjectChanged.Changed -= new EventHandler(OnObjectChanged);
			ISupportObjectChanging supportObjectChanging = obj as ISupportObjectChanging;
			if (supportObjectChanging != null)
				supportObjectChanging.Changing -= new CancelEventHandler(OnObjectChanging);
		}
		#endregion
		protected internal virtual void OnObjectChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		protected internal virtual void OnObjectChanging(object sender, CancelEventArgs e) {
			e.Cancel = RaiseChanging();
		}
	}
	#endregion
}
namespace DevExpress.Internal {
	#region DXCollectionUniquenessProvider<T> (abstract class)
	public abstract class DXCollectionUniquenessProvider<T> {
		public abstract int LookupObjectIndex(T value);
		public abstract bool LookupObject(T value);
		public abstract void OnClearComplete();
		public abstract void OnInsertComplete(T value);
		public abstract void OnRemoveComplete(T value);
		public abstract void OnSetComplete(T oldValue, T newValue);
		public abstract DXCollectionUniquenessProviderType Type { get; }
	}
	#endregion
	#region EmptyUniquenessProvider<T>
	public class EmptyUniquenessProvider<T> : DXCollectionUniquenessProvider<T> {
		public override DXCollectionUniquenessProviderType Type { get { return DXCollectionUniquenessProviderType.None; } }
		public override int LookupObjectIndex(T value) {
			return -1;
		}
		public override bool LookupObject(T value) {
			return false;
		}
		public override void OnClearComplete() {
		}
		public override void OnInsertComplete(T value) {
		}
		public override void OnRemoveComplete(T value) {
		}
		public override void OnSetComplete(T oldValue, T newValue) {
		}
	}
	#endregion
	#region SimpleUniquenessProvider<T>
	public class SimpleUniquenessProvider<T> : DXCollectionUniquenessProvider<T> {
		#region Fields
		readonly DXCollectionBase<T> collection;
		#endregion
		public SimpleUniquenessProvider(DXCollectionBase<T> collection) {
			Guard.ArgumentNotNull(collection, "collection");
			this.collection = collection;
		}
		#region Properties
		protected internal DXCollectionBase<T> Collection { get { return collection; } }
		public override DXCollectionUniquenessProviderType Type { get { return DXCollectionUniquenessProviderType.MinimizeMemoryUsage; } }
		#endregion
		public override int LookupObjectIndex(T value) {
			return collection.IndexOf(value);
		}
		public override bool LookupObject(T value) {
			return collection.Contains(value);
		}
		public override void OnClearComplete() {
		}
		public override void OnInsertComplete(T value) {
		}
		public override void OnRemoveComplete(T value) {
		}
		public override void OnSetComplete(T oldValue, T newValue) {
		}
	}
	#endregion
	#region DictionaryUniquenessProvider<T>
	public class DictionaryUniquenessProvider<T> : DXCollectionUniquenessProvider<T> {
		#region Fields
		readonly DXCollectionBase<T> collection;
		readonly Dictionary<T, T> dictionary;
		#endregion
		public DictionaryUniquenessProvider(DXCollectionBase<T> collection) {
			Guard.ArgumentNotNull(collection, "collection");
			this.collection = collection;
			this.dictionary = new Dictionary<T, T>();
		}
		#region Properties
		protected internal DXCollectionBase<T> Collection { get { return collection; } }
		protected internal Dictionary<T, T> Dictionary { get { return dictionary; } }
		public override DXCollectionUniquenessProviderType Type { get { return DXCollectionUniquenessProviderType.MaximizePerformance; } }
		#endregion
		public override int LookupObjectIndex(T value) {
			T obj;
			if (dictionary.TryGetValue(value, out obj))
				return collection.IndexOf(value);
			else
				return -1;
		}
		public override bool LookupObject(T value) {
			T obj;
			return dictionary.TryGetValue(value, out obj);
		}
		public override void OnClearComplete() {
			dictionary.Clear();
		}
		public override void OnInsertComplete(T value) {
			dictionary.Add(value, value);
		}
		public override void OnRemoveComplete(T value) {
			dictionary.Remove(value);
		}
		public override void OnSetComplete(T oldValue, T newValue) {
			dictionary.Remove(oldValue);
			dictionary.Add(newValue, newValue);
		}
	}
	#endregion
	#region DXNotificationCollectionAccessor
	public static class DXNotificationCollectionAccessor {
		public static void OnCollectionChanged<T>(NotificationCollection<T> collection, CollectionChangedEventArgs<T> e) {
			collection.OnCollectionChanged(e);
		}
		public static void OnCollectionChanging<T>(NotificationCollection<T> collection, CollectionChangingEventArgs<T> e) {
			collection.OnCollectionChanging(e);
		}
	}
	#endregion
	#region DXCollectionAccessor
	public static class DXCollectionAccessor {
		public static T GetItem<T>(DXCollectionBase<T> collection, int index) {
			return collection.InvokeGetItem(index);
		}
		public static IList<T> GetInnerList<T>(DXCollectionBase<T> collection) {
			return collection.GetInnerList();
		}
	}
	#endregion
}
