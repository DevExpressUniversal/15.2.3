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
using System.Linq;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class LocalVisibleListWrapper : VisibleListWrapper {
		readonly CurrentDataView dataView;
		public CurrentDataView DataView { get { return dataView; } }
		public LocalVisibleListWrapper(CurrentDataView dataView) {
			this.dataView = dataView;
			dataView.ListChanged += DataViewListChanged;
		}
		void DataViewListChanged(object sender, ListChangedEventArgs e) {
			SelectionLocker.DoLockedAction(() => {
				if (e.ListChangedType == ListChangedType.ItemChanged) {
					if (EventLocker.IsLocked)
						return;
					RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, e.NewIndex, e.NewIndex));
					return;
				}
				RaiseListChanged(e);
			});
		}
		protected override IEnumerator GetEnumeratorInternal() {
			return dataView.Select(x => x.f_component).GetEnumerator();
		}
		protected override void CopyToInternal(Array array, int index) {
			for (int i = index; i < Count; i++)
				array.SetValue(this[i], i);
		}
		protected override int GetCountInternal() {
			return dataView.VisibleRowCount;
		}
		protected override bool ContainsInternal(object value) {
			return IndexOf(value) > -1;
		}
		protected override int IndexOfInternal(object value) {
			return dataView.IndexOf(value);
		}
		protected override object IndexerGetInternal(int index) {
			return dataView[index].f_component;
		}
		protected override void DisposeInternal() {
			dataView.ListChanged -= DataViewListChanged;
		}
		protected override void RefreshInternal() {
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, null));
		}
	}
	public class LocalVisibleListWrapper<T> : LocalVisibleListWrapper, IList<T> {
		public LocalVisibleListWrapper(CurrentDataView dataView)
			: base(dataView) {
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			foreach (object item in this) {
				yield return (T)item;
			}
		}
		void ICollection<T>.Add(T item) {
			throw new NotImplementedException();
		}
		bool ICollection<T>.Contains(T item) {
			return base.Contains(item);
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			base.CopyTo(array, arrayIndex);
		}
		bool ICollection<T>.Remove(T item) {
			throw new NotImplementedException();
		}
		int IList<T>.IndexOf(T item) {
			return base.IndexOf(item);
		}
		void IList<T>.Insert(int index, T item) {
			throw new NotImplementedException();
		}
		T IList<T>.this[int index] {
			get { return (T)this[index]; }
			set { throw new NotImplementedException(); }
		}
	}
	public class EditorsCompositeCollection : IList, INotifyCollectionChanged, IDisposable {
		public EditorsCompositeCollection() {
			ContentCollection = new CollectionContainer();
			CustomCollection = new CollectionContainer();
			((INotifyCollectionChanged)CustomCollection).CollectionChanged += OnContainedCollectionChanged;
			((INotifyCollectionChanged)ContentCollection).CollectionChanged += OnContainedCollectionChanged;
		}
		public object this[int index] {
			get { return IndexerGetInternal(index); }
			set { this[index] = value; }
		}
		public int Count { get { return GetCountInternal(); } }
		public bool IsFixedSize { get; private set; }
		public bool IsReadOnly { get; private set; }
		public object SyncRoot { get; private set; }
		public bool IsSynchronized { get; private set; }
		IList CustomList { get { return CustomCollection.Collection as IList; } }
		IList ContentList { get { return ContentCollection.Collection as IList; } }
		CollectionContainer ContentCollection { get; set; }
		CollectionContainer CustomCollection { get; set; }
		NotifyCollectionChangedEventHandler handler;
		public event NotifyCollectionChangedEventHandler CollectionChanged { add { handler += value; } remove { handler += value; } }
		public void SetCustomCollection(IList customItemsSource) {
			CustomCollection.Collection = customItemsSource;
		}
		public void SetContentCollection(IList contentItemsSource) {
			ContentCollection.Collection = contentItemsSource;
		}
		public IEnumerator GetEnumerator() {
			return new MergedEnumerator(CustomList.Return(x => x.GetEnumerator(), () => null), ContentList.Return(x => x.GetEnumerator(), () => null));
		}
		public void CopyTo(Array array, int index) {
			for (int i = index; i < Count; i++)
				array.SetValue(this[i], i);
		}
		public void Dispose() {
			((INotifyCollectionChanged)CustomCollection).CollectionChanged -= OnContainedCollectionChanged;
			((INotifyCollectionChanged)ContentCollection).CollectionChanged -= OnContainedCollectionChanged;
			ContentCollection = null;
			CustomCollection = null;
		}
		public bool Contains(object value) {
			return IndexOf(value) > -1;
		}
		public int IndexOf(object value) {
			if (IsCustomCollectionContains(value)) return CustomList.IndexOf(value);
			else if (ContentList != null) {
				int index = ContentList.IndexOf(value);
				return index > -1 ? index + GetCustomCollectionCount() : index;
			}
			return -1;
		}
		void OnContainedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			int count = ((CollectionContainer)sender).Collection == CustomList ? 0 : CustomList.Count;
			int oldIndex = e.OldStartingIndex >= 0 ? e.OldStartingIndex + count : e.OldStartingIndex;
			int newIndex = e.NewStartingIndex >= 0 ? e.NewStartingIndex + count : e.NewStartingIndex;
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					e = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					e = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					e = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems, oldIndex);
					break;
				case NotifyCollectionChangedAction.Move:
					e = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, newIndex, oldIndex);
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
			}
			RaiseCollectionChanged(e);
		}
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (handler != null)
				handler(this, e);
		}
		int GetCountInternal() {
			return GetCustomCollectionCount() + GetContentCollectionCount();
		}
		object IndexerGetInternal(int index) {
			if (Count <= index) return null;
			int count = GetCustomCollectionCount();
			if (count > index) return CustomList[index];
			return ContentList[index - count];
		}
		int GetContentCollectionCount() {
			return ContentList.Return(x => x.Count, () => 0);
		}
		int GetCustomCollectionCount() {
			return CustomList.Return(x => x.Count, () => 0);
		}
		bool IsCustomCollectionContains(object value) {
			return CustomList.Return(x => x.Contains(value), () => false);
		}
		#region IList Members
		int IList.Add(object value) {
			throw new NotImplementedException();
		}
		void IList.Clear() {
			throw new NotImplementedException();
		}
		void IList.Insert(int index, object value) {
			throw new NotImplementedException();
		}
		void IList.Remove(object value) {
			throw new NotImplementedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
