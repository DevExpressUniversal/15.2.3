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
using System.Text;
using System.Collections;
namespace DevExpress.XtraPivotGrid.Data {
	public class BaseCollection<T> : IList<T>, IList, ICollection<T> {
		readonly List<T> innerList;
		public BaseCollection() {
			this.innerList = new List<T>();
		}
		public BaseCollection(int capacity) {
			this.innerList = new List<T>();
		}
		protected List<T> InnerList { get { return innerList; } }
		protected virtual void OnInsertComplete(int index, T item) {
		}
		protected virtual void OnInsertComplete(int index, IEnumerable<T> collection) {
		}
		protected virtual void OnRemoveComplete(int index, T value) {
		}
		protected virtual void OnReplaceComplete(int index, T oldItem, T newItem) {
		}
		protected virtual void OnClear() {
		}
		protected virtual void OnClearComplete() {
		}
		protected virtual bool CanAddItem(T item) {
			return true;
		}
		#region IList<T> Members
		public int IndexOf(T item) {
			return InnerList.IndexOf(item);
		}
		public void Insert(int index, T item) {
			InnerList.Insert(index, item);
			OnInsertComplete(index, item);
		}
		public void RemoveAt(int index) {
			T item = this[index];
			InnerList.RemoveAt(index);
			OnRemoveComplete(index, item);
		}
		public T this[int index] {
			get { return InnerList[index]; }
			set {
				if(object.ReferenceEquals(this[index], value)) return;
				T oldItem = InnerList[index];
				InnerList[index] = value;
				OnReplaceComplete(index, oldItem, value);
			}
		}
		#endregion
		#region ICollection<T> Members
		public void Add(T item) {
			if(!CanAddItem(item)) return;
			InnerList.Add(item);
			OnInsertComplete(InnerList.Count - 1, item);
		}
		public void AddRange(IEnumerable<T> collection) {
			int index = InnerList.Count;
			InnerList.AddRange(collection);
			OnInsertComplete(index, collection);
		}
		public void Clear() {
			OnClear();
			InnerList.Clear();
			OnClearComplete();
		}
		public bool Contains(T item) {
			return InnerList.Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}
		public int Count { get { return InnerList.Count; } }
		public bool IsReadOnly { get { return false; } }
		public bool Remove(T item) {
			int itemIndex = InnerList.IndexOf(item);
			if(itemIndex < 0) return false;
			InnerList.RemoveAt(itemIndex);
			OnRemoveComplete(itemIndex, item);
			return true;
		}
		#endregion
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		public void ForEach(Action<T> action) {
			foreach(T item in InnerList) {
				action(item);
			}
		}
		public void Sort(IComparer<T> comparer) {
			innerList.Sort(comparer);
		}
		#region IList Members
		int IList.Add(object value) {
			Add((T)value);
			return Count - 1;
		}
		void IList.Clear() {
			Clear();
		}
		bool IList.Contains(object value) {
			return Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return IndexOf((T)value);
		}
		void IList.Insert(int index, object value) {
			Insert(index, (T)value);
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		bool IList.IsReadOnly {
			get { return false; }
		}
		void IList.Remove(object value) {
			Remove((T)value);
		}
		void IList.RemoveAt(int index) {
			RemoveAt(index);
		}
		object IList.this[int index] {
			get { return this[index]; }
			set { this[index] = (T)value; }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			((ICollection)InnerList).CopyTo(array, index);
		}
		int ICollection.Count {
			get { return Count; }
		}
		bool ICollection.IsSynchronized {
			get { return ((ICollection)InnerList).IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return ((ICollection)InnerList).SyncRoot; }
		}
		#endregion
	}
}
