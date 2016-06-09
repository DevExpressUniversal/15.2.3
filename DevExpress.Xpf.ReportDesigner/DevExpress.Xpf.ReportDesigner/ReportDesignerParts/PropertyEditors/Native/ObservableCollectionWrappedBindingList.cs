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
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors.Native {
	public class ObservableCollectionWrappedBindingList<T> : IBindingList, IList<T> {
		public ObservableCollectionWrappedBindingList(Action<T> addNewItemAction = null, PropertyChangedEventHandler propertyChanged = null) {
			this.collection = new ObservableCollection<T>();
			this.propertyChanged = propertyChanged;
			this.addNewItemAction = addNewItemAction;
			this.collection.CollectionChanged += (s, e) => {
				if(listChanged != null) {
					listChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, null));
				}
			};
		}
		ObservableCollection<T> collection;
		PropertyChangedEventHandler propertyChanged;
		Action<T> addNewItemAction;
		#region IBindingList Members
		bool IBindingList.AllowEdit { get { return true; } }
		bool IBindingList.AllowNew { get { return true; } }
		bool IBindingList.AllowRemove { get { throw new NotImplementedException(); } }
		ListSortDirection IBindingList.SortDirection { get { throw new NotImplementedException(); } }
		PropertyDescriptor IBindingList.SortProperty { get { throw new NotImplementedException(); } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { throw new NotImplementedException(); } }
		bool IBindingList.SupportsSorting { get { throw new NotImplementedException(); } }
		bool IBindingList.IsSorted { get { throw new NotImplementedException(); } }
		ListChangedEventHandler listChanged;
		event ListChangedEventHandler IBindingList.ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotImplementedException();
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		void IBindingList.RemoveSort() {
			throw new NotImplementedException();
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotImplementedException();
		}
		object IBindingList.AddNew() {
			var item = Activator.CreateInstance<T>();
			AddItemWithChangeNotifications(item);
			return item;
		}
		#endregion
		#region IList Members
		bool IList.IsFixedSize { get { throw new NotImplementedException(); } }
		bool IList.IsReadOnly { get { throw new NotImplementedException(); } }
		object IList.this[int index] {
			get { return collection[index]; }
			set { collection[index] = (T)value; }
		}
		void IList.Clear() {
			collection.Clear();
		}
		void IList.Insert(int index, object value) {
			collection.Insert(index, (T)value);
		}
		void IList.Remove(object value) {
			collection.Remove((T)value);
		}
		void IList.RemoveAt(int index) {
			collection.RemoveAt(index);
		}
		bool IList.Contains(object value) {
			return collection.Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return collection.IndexOf((T)value);
		}
		int IList.Add(object value) {
			AddItemWithChangeNotifications((T)value);
			return collection.IndexOf((T)value);
		}
		#endregion
		#region ICollection Members
		int ICollection.Count { get { return collection.Count; } }
		bool ICollection.IsSynchronized { get { throw new NotImplementedException(); } }
		object ICollection.SyncRoot { get { throw new NotImplementedException(); } }
		void ICollection.CopyTo(Array array, int index) {
			collection.CopyTo((T[])array, index);
		}
		#endregion
		IEnumerator IEnumerable.GetEnumerator() {
			return collection.GetEnumerator();
		}
		#region IList<T> Members
		public int IndexOf(T item) {
			return collection.IndexOf(item);
		}
		public void Insert(int index, T item) {
			collection.Insert(index, item);
		}
		public void RemoveAt(int index) {
			collection.RemoveAt(index);
		}
		public T this[int index] {
			get { return collection[index]; }
			set { collection[index] = value; }
		}
		public void Add(T item) {
			AddItemWithChangeNotifications(item);
		}
		public void Clear() {
			collection.Clear();
		}
		public bool Contains(T item) {
			return collection.Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			collection.CopyTo(array, arrayIndex);
		}
		public int Count { get { return collection.Count; } }
		public bool IsReadOnly { get { throw new NotImplementedException(); } }
		public bool Remove(T item) {
			return collection.Remove(item);
		}
		public IEnumerator<T> GetEnumerator() {
			return collection.GetEnumerator();
		}
		#endregion
		public void Swap(int firstIndex, int secondIndex) {
			var firstItem = collection[firstIndex];
			var secondItem = collection[secondIndex];
			collection[firstIndex] = default(T);
			collection[secondIndex] = default(T);
			collection[firstIndex] = secondItem;
			collection[secondIndex] = firstItem;
		}
		public void AddItemWithoutDoActions(T item) {
			collection.Add(item);
		}
		void AddItemWithChangeNotifications(T item) {
			collection.Add(item);
			if(addNewItemAction != null)
				addNewItemAction(item);
			if(item is INotifyPropertyChanged)
				((INotifyPropertyChanged)item).PropertyChanged += propertyChanged;
		}
	}
}
