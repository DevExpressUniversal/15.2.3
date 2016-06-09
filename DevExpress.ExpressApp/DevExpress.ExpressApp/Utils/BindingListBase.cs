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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Utils {
	public class BindingListBase<T> : CollectionBase, IBindingList, IList<T>, ISupportInitialize {
		private class Enumerator : IEnumerator<T> {
			private BindingListBase<T> source;
			private int current;
			private bool init;
			public Enumerator(BindingListBase<T> source) {
				this.source = source;
				this.init = true;
			}
			public T Current {
				get { return source[current]; }
			}
			public void Dispose() {
				source = null;
			}
			object IEnumerator.Current {
				get { return source[current]; }
			}
			public bool MoveNext() {
				if(init) {
					current = 0;
					init = false;
				}
				else {
					current++;
				}
				return current < source.Count;
			}
			public void Reset() {
				init = true;
			}
		}
		private int lockCount = 0;
		protected bool IsInitializing {
			get { return lockCount > 0; }
		}
		protected virtual void OnInitialize() {
		}
		protected virtual void OnInitializeComplete() {
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			OnListChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			OnListChanged();
		}
		protected virtual void OnListChanged() {
			if(lockCount == 0 && ListChanged != null) {
				ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
			}
		}
		#region IList<> impl
		public void AddIndex(PropertyDescriptor property) { }
		public object AddNew() { throw new Exception("The method or operation is not implemented."); }
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) { }
		public int Find(PropertyDescriptor property, object key) { return 0; }
		public void RemoveIndex(PropertyDescriptor property) { }
		public void RemoveSort() { }
		public ListSortDirection SortDirection { get { return ListSortDirection.Ascending; } }
		public PropertyDescriptor SortProperty { get { return null; } }
		public bool SupportsChangeNotification { get { return true; } }
		public bool SupportsSearching { get { return false; } }
		public bool SupportsSorting { get { return false; } }
		public bool AllowEdit { get { return true; } }
		public bool AllowNew { get { return false; } }
		public bool AllowRemove { get { return true; } }
		public bool IsSorted { get { return false; } }
		public event ListChangedEventHandler ListChanged;
		public int IndexOf(T item) {
			return ((IList)this).IndexOf(item);
		}
		public void Insert(int index, T item) {
			((IList)this).Insert(index, item);
		}
		public T this[int index] {
			get { return (T)((IList)this)[index]; }
			set { ((IList)this)[index] = value; }
		}
		public void Add(T item) {
			((IList)this).Add(item);
		}
		public bool Contains(T item) {
			return ((IList)this).Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			((ICollection)this).CopyTo(array, arrayIndex);
		}
		public bool IsReadOnly {
			get { return InnerList.IsReadOnly; }
		}
		public bool Remove(T item) {
			int index = this.IndexOf(item);
			if(index >= 0) {
				((IList)this).Remove(item);
				return true;
			}
			return false;
		}
		public new IEnumerator<T> GetEnumerator() {
			return new Enumerator(this);
		}
		#endregion
		#region ISupportInitialize Members
		public void BeginInit() {
			lockCount++;
			if(lockCount == 1) {
				OnInitialize();
			}
		}
		public virtual void EndInit() {
			lockCount--;
			if(lockCount == 0) {
				OnInitializeComplete();
				OnListChanged();
			}
		}
		#endregion
	}
}
