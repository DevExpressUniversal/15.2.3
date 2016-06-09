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
using System.Reflection;
namespace DevExpress.XtraGauges.Core.Base {
	public class BaseReadOnlyList<T> : BaseObject, IReadOnlyCollection<T>
		where T : class, ISupportAcceptOrder {
		IList<T> listCore;
		ICollection collectionCore;
		bool isCompatibleWithINamedCore;
		protected override void OnCreate() {
			this.isCompatibleWithINamedCore = typeof(INamed).IsAssignableFrom(typeof(T));
			this.listCore = CreateListCore();
			this.collectionCore = listCore as ICollection;
		}
		protected override void OnDispose() {
			if(listCore != null) {
				listCore.Clear();
				listCore = null;
			}
		}
		protected internal bool IsCompatibleWithINamed {
			get { return isCompatibleWithINamedCore; }
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
			List<T> sortedList = new List<T>(List);
			sortedList.Sort(new AcceptComparer<T>());
			foreach(T e in sortedList) visitor.Visit(e);
		}
		public void Accept(VisitDelegate<T> visit) {
			if(visit == null) return;
			List<T> sortedList = new List<T>(List);
			sortedList.Sort(new AcceptComparer<T>());
			foreach(T e in sortedList) visit(e);
		}
		public T this[int index] {
			get { return List[index]; }
		}
		public T this[string elementName] {
			get { return GetElementByName(elementName); }
		}
		protected T GetElementByName(string elementName) {
			if(IsCompatibleWithINamed) {
				foreach(INamed element in List) {
					if(element.Name == elementName) return element as T;
				}
			}
			return null;
		}
	}
	public class BaseChangeableList<T> : BaseReadOnlyList<T>, IChangeableCollection<T>, ICollection, IList
		where T : class, ISupportAcceptOrder {
		event CollectionChangedHandler<T> collectionChanged;
		protected override void OnDispose() {
			collectionChanged = null;
			base.OnDispose();
		}
		public void Add(T element) {
			OnBeforeElementAdded(element);
			List.Add(element);
			OnElementAdded(element);
		}
		public void AddRange(T[] elements) {
			foreach(T element in elements) Add(element);
		}
		public void AddRange(IReadOnlyCollection<T> elements) {
			foreach(T element in elements) Add(element);
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
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}
		protected virtual void OnBeforeElementAdded(T element) { }
		protected virtual void OnElementAdded(T element) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<T>(element, ElementChangedType.ElementAdded));
		}
		protected virtual void OnBeforeElementRemoved(T element) { }
		protected virtual void OnElementRemoved(T element) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<T>(element, ElementChangedType.ElementRemoved));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangedEventArgs<T> ea) {
			if(collectionChanged != null) collectionChanged(ea);
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
	public class BaseReadOnlyDictionary<T> : BaseObject, IReadOnlyCollection<T>
		where T : class, ISupportAcceptOrder, IDisposable {
		Dictionary<string, T> collectionCore;
		protected override void OnCreate() {
			this.collectionCore = CreateCollectionCore();
		}
		public void Clear() {
			collectionCore.Clear();
		}
		protected override void OnDispose() {
		}
		public void DisposeItems() {
			foreach(T item in this)
				item.Dispose();
		}
		protected Dictionary<string, T> CreateCollectionCore() {
			return new Dictionary<string, T>();
		}
		protected internal Dictionary<string, T> Collection {
			get { return collectionCore; }
		}
		public int Count {
			get { return Collection.Count; }
		}
		public bool Contains(T element) {
			return Collection.ContainsValue(element);
		}
		public bool Contains(string name) {
			return Collection.ContainsKey(name);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Collection.GetEnumerator();
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return Collection.Values.GetEnumerator();
		}
		public void Accept(IVisitor<T> visitor) {
			if(visitor == null) return;
			List<T> sortedList = new List<T>(Collection.Values);
			sortedList.Sort(new AcceptComparer<T>());
			foreach(T e in sortedList) visitor.Visit(e);
		}
		public void Accept(VisitDelegate<T> visit) {
			if(visit == null) return;
			List<T> sortedList = new List<T>(Collection.Values);
			sortedList.Sort(new AcceptComparer<T>());
			foreach(T e in sortedList) visit(e);
		}
		public T this[string name] {
			get {
				T value;
				return Collection.TryGetValue(name, out value) ? value : null;
			}
		}
	}
	public class BaseElementReadOnlyDictionary<T> : BaseReadOnlyDictionary<IElement<T>>, IElementsReadOnlyCollection<T>
		where T : class, IElement<T> {
		IComposite<T> ownerCore;
		ElementChangedHandler<T> collectionChanged;
		public BaseElementReadOnlyDictionary(IComposite<T> owner)
			: base() {
			this.ownerCore = owner;
		}
		protected override void OnDispose() {
			this.collectionChanged = null;
			this.ownerCore = null;
			base.OnDispose();
		}
		protected IComposite<T> Owner {
			get { return ownerCore; }
		}
		public new event ElementChangedHandler<T> Changed {
			add { collectionChanged += value; }
			remove { collectionChanged -= value; }
		}
		protected virtual void RaiseCollectionChanged(ElementChangedEventArgs<T> ea) {
			if(collectionChanged != null) collectionChanged(ea);
		}
	}
	public class BaseElementDictionary<T> : BaseElementReadOnlyDictionary<T>
		where T : class, IElement<T> {
		public BaseElementDictionary(IComposite<T> owner) : base(owner) { }
		public void Add(IElement<T> element) {
			if(Collection.ContainsValue(element)) return;
			if(string.IsNullOrEmpty(element.Name) || Collection.ContainsKey(element.Name)) return;
			Collection.Add(element.Name, element);
			AffinityHelper.SetCompositeAffinity<T>(Owner, element as BaseElement<T>);
			NameLockHelper.LockName<T>(element as BaseElement<T>);
			RaiseCollectionChanged(new ElementChangedEventArgs<T>(element, ElementChangedType.ElementAdded));
		}
		public void Remove(IElement<T> element) {
			if(!Collection.ContainsValue(element)) return;
			Collection.Remove(element.Name);
			AffinityHelper.SetCompositeAffinity(null, element as BaseElement<T>);
			NameLockHelper.UnlockName(element as BaseElement<T>);
			RaiseCollectionChanged(new ElementChangedEventArgs<T>(element, ElementChangedType.ElementRemoved));
		}
	}
}
