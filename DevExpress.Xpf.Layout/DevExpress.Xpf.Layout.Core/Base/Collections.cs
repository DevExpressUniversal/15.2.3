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
namespace DevExpress.Xpf.Layout.Core.Base {
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
		protected IList<T> List {
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
	}
	public class BaseChangeableList<T> : BaseReadOnlyList<T>,
		IChangeableCollection<T>, ICollection, IList where T : class {
		event CollectionChangedHandler<T> collectionChangedCore;
		protected override void OnDispose() {
			collectionChangedCore = null;
			base.OnDispose();
		}
		public void Add(T element) {
			OnBeforeElementAdded(element);
			List.Add(element);
			OnElementAdded(element);
		}
		public void AddRange(T[] elements) {
			OnBeforeElementRangeAdded();
			foreach(T element in elements) Add(element);
			OnElementRangeAdded();
		}
		public void AddRange(IReadOnlyCollection<T> elements) {
			OnBeforeElementRangeAdded();
			foreach(T element in elements) Add(element);
			OnElementRangeAdded();
		}
		public bool Remove(T element) {
			OnBeforeElementRemoved(element);
			bool result = List.Remove(element);
			if(result) OnElementRemoved(element);
			return result;
		}
		public void CopyTo(T[] array, int index) {
			List.CopyTo(array, index);
		}
		public void Clear() {
			T[] elements = ToArray();
			for(int i = 0; i < elements.Length; i++) {
				Remove(elements[i]);
			}
		}
		public event CollectionChangedHandler<T> CollectionChanged {
			add { collectionChangedCore += value; }
			remove { collectionChangedCore -= value; }
		}
		protected virtual void OnBeforeElementRangeAdded() { }
		protected virtual void OnElementRangeAdded() { }
		protected virtual void OnBeforeElementAdded(T element) { }
		protected virtual void OnElementAdded(T element) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<T>(element, CollectionChangedType.ElementAdded));
		}
		protected virtual void OnBeforeElementRemoved(T element) { }
		protected virtual void OnElementRemoved(T element) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<T>(element, CollectionChangedType.ElementRemoved));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangedEventArgs<T> ea) {
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
		public void CopyTo(Array array, int index) {
			Collection.CopyTo(array, index);
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
			T element = (T)value;
			List.Insert(index, element);
			if(element != null) OnElementAdded(element);
		}
		void IList.Remove(object value) {
			Remove((T)value);
		}
		void IList.RemoveAt(int index) {
			T element = this[index];
			List.RemoveAt(index);
			if(element != null) OnElementRemoved(element);
		}
	}
}
