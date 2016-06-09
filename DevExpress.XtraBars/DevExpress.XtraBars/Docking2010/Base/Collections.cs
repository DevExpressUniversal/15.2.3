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
namespace DevExpress.XtraBars.Docking2010.Base {
	public class BaseReadOnlyList<T> : BaseObject,
		IReadOnlyCollection<T> where T : class {
		IList<T> listCore;
		ICollection collectionCore;
		protected override void OnCreate() {
			base.OnCreate();
			this.listCore = CreateListCore();
			this.collectionCore = listCore as ICollection;
		}
		protected override void OnDispose() {
			Ref.Clear(ref listCore);
			base.OnDispose();
		}
		protected List<T> CreateListCore() {
			return new List<T>();
		}
		protected internal IList<T> List {
			get { return listCore; }
		}
		protected ICollection Collection {
			get { return collectionCore; }
		}
		public int Count {
			get { return List.Count; }
		}
		public bool Contains(T element) {
			return List.Contains(element);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return List.GetEnumerator();
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return List.GetEnumerator();
		}
		public void Accept(IVisitor<T> visitor) {
			if(visitor == null) return;
			foreach(T e in List) visitor.Visit(e);
		}
		public void Accept(VisitDelegate<T> visit) {
			if(visit == null) return;
			foreach(T e in List) visit(e);
		}
		public T this[int index] {
			get { return List[index]; }
		}
		public void CopyTo(T[] array, int index) {
			List.CopyTo(array, index);
		}
		public void CopyTo(Array array, int index) {
			Collection.CopyTo(array, index);
		}
	}
	public class BaseMutableList<T> : BaseReadOnlyList<T>,
		IBaseCollection<T>, ICollection, IList where T : class {
		event CollectionChangedHandler<T> collectionChangedCore;
		protected override void OnDispose() {
			collectionChangedCore = null;
			base.OnDispose();
		}
		public bool Add(T element) {
			if(CanAdd(element)) {
				OnBeforeElementAdded(element);
				List.Add(element);
				OnElementAdded(element);
				return true;
			}
			return false;
		}
		protected virtual bool CanAdd(T element) {
			return List.IndexOf(element) < 0;
		}
		public void AddRange(T[] elements) {
			OnBeforeElementRangeAdded();
			for(int i = 0; i < elements.Length; i++) 
				Add(elements[i]);
			OnElementRangeAdded();
		}
		public void AddRange(IEnumerable<T> elements) {
			OnBeforeElementRangeAdded();
			foreach(T element in elements) Add(element);
			OnElementRangeAdded();
		}
		public bool Remove(T element) {
			OnBeforeElementRemoved(element);
			bool result = List.Remove(element);
			if(result) OnElementRemoved(element);
			else OnElementRemoveCanceled(element);
			return result;
		}
		int clearInProgress;
		public void Clear() {
			using(ClearBatch.Enter(this)) {
				T[] elements = ToArray();
				RemoveRange(elements);
			}
		}
		protected internal T[] CleanUp() {
			T[] elements = ToArray();
			using(LockCollectionChanged()) {
				RemoveRange(elements);
				return elements;
			}
		}
		public void RemoveRange(T[] elements) {
			OnBeforeElementRangeRemoved();
			for(int i = 0; i < elements.Length; i++)
				Remove(elements[i]);
			OnElementRangeRemoved();
		}
		public int IndexOf(T element) {
			return List.IndexOf(element);
		}
		public event CollectionChangedHandler<T> CollectionChanged {
			add { collectionChangedCore += value; }
			remove { collectionChangedCore -= value; }
		}
		protected virtual void OnBeforeElementRangeAdded() { }
		protected virtual void OnElementRangeAdded() { }
		protected virtual void OnBeforeElementRangeRemoved() { }
		protected virtual void OnElementRangeRemoved() { }
		protected virtual void OnBeforeElementAdded(T element) { }
		protected virtual void OnElementAdded(T element) {
			RaiseCollectionChanged(element, CollectionChangedType.ElementAdded);
		}
		protected virtual void OnBeforeElementRemoved(T element) { }
		protected virtual void OnElementRemoved(T element) {
			RaiseCollectionChanged(element, CollectionChangedType.ElementRemoved, clearInProgress > 0);
		}
		protected virtual void OnElementRemoveCanceled(T element) { }
		protected virtual void RaiseCollectionChanged(T element, CollectionChangedType changedType) {
			RaiseCollectionChanged(element, changedType, false);
		}
		protected virtual void RaiseCollectionChanged(T element, CollectionChangedType changedType, bool clearCollection) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<T>(element, changedType, clearCollection));
		}
		int lockCollectionChanded;
		protected virtual void RaiseCollectionChanged(CollectionChangedEventArgs<T> ea) {
			if(lockCollectionChanded > 0) return;
			if(collectionChangedCore != null) collectionChangedCore(ea);
		}
		public bool IsReadOnly {
			get { return List.IsReadOnly; }
		}
		public bool IsSynchronized {
			get { return Collection.IsSynchronized; }
		}
		public object SyncRoot {
			get { return Collection.SyncRoot; }
		}
		public IEnumerable<T> Find(Predicate<T> match) {
			foreach(T element in List) {
				if(!match(element)) continue;
				yield return element;
			}
		}
		public T FindFirst(Predicate<T> match) {
			foreach(T element in List) {
				if(!match(element)) continue;
				return element;
			}
			return null;
		}
		public bool Exists(Predicate<T> match)  {
			foreach(T element in List)
				if(match(element)) return true;
			return false;
		}
		public T[] ToArray() {
			T[] array = new T[Collection.Count];
			CopyTo(array, 0);
			return array;
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		object IList.this[int index] {
			get { return List[index]; }
			set { List[index] = (T)value; }
		}
		int IList.Add(object value) {
			Add(value as T);
			return List.IndexOf((T)value);
		}
		bool IList.Contains(object value) {
			return List.Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return List.IndexOf((T)value);
		}
		void IList.Insert(int index, object value) {
			InsertCore(index, (T)value);
		}
		protected internal void Sort(IComparer<T> comparer) {
			((List<T>)List).Sort(comparer);
		}
		protected bool MoveCore(int index, T element) {
			int prevIndex = List.IndexOf(element);
			index = Math.Max(0, Math.Min(List.Count - 1, index));
			if(prevIndex != -1 && prevIndex != index) {
				List.RemoveAt(prevIndex);
				List.Insert(index, element);
				return List.Contains(element);
			}
			return false;
		}
		protected bool InsertCore(int index, T element) {
			if(element != null && CanAdd(element)) {
				OnBeforeElementAdded(element);
				index = Math.Max(0, Math.Min(List.Count, index));
				List.Insert(index, element);
				OnElementAdded(element);
				return true;
			}
			return false;
		}
		void IList.Remove(object value) {
			Remove((T)value);
		}
		void IList.RemoveAt(int index) {
			T element = this[index];
			List.RemoveAt(index);
			if(element != null) OnElementRemoved(element);
		}
		protected internal IDisposable LockCollectionChanged() {
			return new CollectionChangedLocker(this);
		}
		class CollectionChangedLocker : IDisposable {
			BaseMutableList<T> List;
			public CollectionChangedLocker(BaseMutableList<T> list) {
				List = list;
				List.lockCollectionChanded++;
			}
			public void Dispose() {
				List.lockCollectionChanded--;
				List = null;
			}
		}
		class ClearBatch : IDisposable {
			public static IDisposable Enter(BaseMutableList<T> list) {
				return new ClearBatch(list);
			}
			BaseMutableList<T> List;
			public ClearBatch(BaseMutableList<T> list) {
				List = list;
				List.clearInProgress++;
			}
			void IDisposable.Dispose() {
				List.clearInProgress--;
				List = null;
			}
		}
	}
	public class BaseMutableListEx<T> : BaseMutableList<T> where T : class, DevExpress.Utils.Base.IBaseObject {
		protected override void OnDispose() {
			T[] elements = ToArray();
			Array.ForEach(elements, UnregisterElementOnDisposeCollection);
			for(int i = 0; i < elements.Length; i++) {
				T element = elements[i];
				Ref.Dispose(ref element);
			}
			base.OnDispose();
		}
		protected virtual void UnregisterElementOnDisposeCollection(T element) {
			element.Disposed -= OnElementDisposed;
		}
		protected override void OnElementAdded(T element) {
			base.OnElementAdded(element);
			element.Disposed += OnElementDisposed;
		}
		protected override void OnElementRemoved(T element) {
			element.Disposed -= OnElementDisposed;
			base.OnElementRemoved(element);
		}
		protected virtual void OnElementDisposed(object sender, EventArgs ea) {
			T element = sender as T;
			RaiseCollectionChanged(element, CollectionChangedType.ElementDisposed);
			if(List != null && List.Contains(element))
				Remove(element);
		}
	}
	public abstract class BaseMutableListEx<T, TOwner> : BaseMutableListEx<T>
		where T : class, DevExpress.Utils.Base.IBaseObject
		where TOwner : class, DevExpress.Utils.Base.ISupportBatchUpdate {
		TOwner tOwnerCore;
		protected BaseMutableListEx(TOwner owner) {
			tOwnerCore = owner;
		}
		protected override void OnDispose() {
			tOwnerCore = null;
			base.OnDispose();
		}
		public TOwner Owner { 
			get { return tOwnerCore; } 
		}
		IBatchUpdate batchUpdateRangeOperation;
		protected override void OnBeforeElementRangeAdded() {
			batchUpdateRangeOperation = BatchUpdate.Enter(Owner);
			base.OnBeforeElementRangeAdded();
		}
		protected override void OnElementRangeAdded() {
			base.OnElementRangeAdded();
			Ref.Dispose(ref batchUpdateRangeOperation);
		}
		protected override void OnBeforeElementRangeRemoved() {
			batchUpdateRangeOperation = BatchUpdate.Enter(Owner);
			base.OnBeforeElementRangeRemoved();
		}
		protected override void OnElementRangeRemoved() {
			base.OnElementRangeRemoved();
			Ref.Dispose(ref batchUpdateRangeOperation);
		}
		protected override void OnElementDisposed(object sender, EventArgs ea) {
			using(BatchUpdate.Enter(Owner)) {
				base.OnElementDisposed(sender, ea);
			}
		}
		public bool Move(int index, T element) {
			using(IBatchUpdate update = BatchUpdate.Enter(Owner)) {
				bool result = MoveCore(index, element);
				if(result && Owner != null)
					NotifyOwnerOnMove(index, element);
				if(!result && update != null)
					update.Cancel();
				return result;
			}
		}
		public bool Insert(int index, T element) {
			using(IBatchUpdate update = BatchUpdate.Enter(Owner)) {
				bool result = InsertCore(index, element);
				if(result && Owner != null)
					NotifyOwnerOnInsert(index);
				if(!result && update != null)
					update.Cancel();
				return result;
			}
		}
		protected abstract void NotifyOwnerOnInsert(int index);
		protected virtual void NotifyOwnerOnMove(int index, T element) { }
	}
}
