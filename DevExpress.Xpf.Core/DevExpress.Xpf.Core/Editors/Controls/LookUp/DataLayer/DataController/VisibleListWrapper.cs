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
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class VisibleListWrapper : IBindingList, IDisposable {
		bool disposed;
		protected VisibleListWrapper() {
			EventLocker = new Locker();
			SelectionLocker = new Locker();
		}
		public IEnumerator GetEnumerator() {
			return GetEnumeratorInternal();
		}
		public Locker SelectionLocker { get; private set; }
		public Locker EventLocker { get; private set; }
		protected abstract IEnumerator GetEnumeratorInternal();
		public void CopyTo(Array array, int index) {
			CopyToInternal(array, index);
		}
		protected abstract void CopyToInternal(Array array, int index);
		public int Count { get { return GetCountInternal(); } }
		protected abstract int GetCountInternal();
		public object SyncRoot { get; private set; }
		public bool IsSynchronized { get; private set; }
		public int Add(object value) {
			throw new NotImplementedException();
		}
		public bool Contains(object value) {
			return ContainsInternal(value);
		}
		protected abstract bool ContainsInternal(object value);
		public void Clear() {
			throw new NotImplementedException();
		}
		public int IndexOf(object value) {
			return IndexOfInternal(value);
		}
		protected abstract int IndexOfInternal(object value);
		public void Insert(int index, object value) {
			throw new NotImplementedException();
		}
		public void Remove(object value) {
			throw new NotImplementedException();
		}
		public void RemoveAt(int index) {
			throw new NotImplementedException();
		}
		public void Refresh() {
			RefreshInternal();
		}
		protected abstract void RefreshInternal();
		public object this[int index] {
			get { return IndexerGetInternal(index); }
			set { throw new NotImplementedException(); }
		}
		protected abstract object IndexerGetInternal(int index);
		public bool IsReadOnly { get; private set; }
		public bool IsFixedSize { get; private set; }
		public object AddNew() {
			throw new NotImplementedException();
		}
		public void AddIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotImplementedException();
		}
		public int Find(PropertyDescriptor property, object key) {
			throw new NotImplementedException();
		}
		public void RemoveIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		public void RemoveSort() {
			throw new NotImplementedException();
		}
		public bool AllowNew { get; private set; }
		public bool AllowEdit { get; private set; }
		public bool AllowRemove { get; private set; }
		public bool SupportsChangeNotification { get { return true; } }
		public bool SupportsSearching { get; private set; }
		public bool SupportsSorting { get; private set; }
		public bool IsSorted { get; private set; }
		public PropertyDescriptor SortProperty { get; private set; }
		public ListSortDirection SortDirection { get; private set; }
		event ListChangedEventHandler Handler;
		public event ListChangedEventHandler ListChanged {
			add { Handler += value; }
			remove { Handler -= value; }
		}
		protected void RaiseListChanged(ListChangedEventArgs e) {
			Handler.Do(x => x(this, e));
		}
		public void Dispose() {
			if (disposed)
				return;
			disposed = true;
			DisposeInternal();
		}
		protected abstract void DisposeInternal();
	}
}
