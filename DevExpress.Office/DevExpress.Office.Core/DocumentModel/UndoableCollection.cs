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
using System.Linq;
using System.Text;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.Office {
	public interface IReadOnlyCollection<T> : IEnumerable<T>, IEnumerable {
		int Count { get; }
	}
	public interface IReadOnlyList<T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable {
		T this[int index] { get; }
	}
	#region SimpleCollection (abstract class)
	public abstract class SimpleCollection<T> : ICollection<T>, IReadOnlyList<T>, IEnumerable<T> {
		#region Fields
		List<T> innerList;
		#endregion
		protected SimpleCollection() {
			this.innerList = new List<T>();
		}
		#region Properties
		public int Count { get { return innerList.Count; } }
		public int Capacity { get { return innerList.Capacity; } set { innerList.Capacity = value; } }
		public T this[int index] { get { return innerList[index]; } }
		public List<T> InnerList { get { return innerList; } }
		public T First { get { return Count > 0 ? InnerList[0] : default(T); } }
		public T Last { get { return Count > 0 ? InnerList[Count - 1] : default(T); } }
		public bool IsReadOnly { get { return false; } }
		#endregion
		public virtual bool Contains(T item) {
			return this.innerList.Contains(item);
		}
		public void ForEach(Action<T> action) {
			innerList.ForEach(action);
		}
		public virtual int IndexOf(T item) {
			return innerList.IndexOf(item);
		}
		public virtual void RemoveAt(int index) {
			T item = this[index];
			innerList.RemoveAt(index);
			OnItemRemoved(index, item);
		}
		public virtual bool Remove(T item) {
			int index = innerList.IndexOf(item);
			if (index < 0)
				return false;
			RemoveAt(innerList.IndexOf(item));
			return true;
		}
		public virtual int Add(T item) {
			int index = AddWithoutNotification(item);
			OnItemInserted(index, item);
			return index;
		}
		protected virtual int AddWithoutNotification(T item) {
			Guard.ArgumentNotNull(item, "Item");
			this.InnerList.Add(item);
			return Count - 1;
		}
		void ICollection<T>.Add(T item) {
			Add(item);
		}
		public virtual IEnumerator<T> GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public virtual void Clear() {
			if (Count <= 0)
				return;
			this.InnerList.Clear();
			OnModified();
		}
		public virtual void Insert(int index, T item) {
			innerList.Insert(index, item);
			OnItemInserted(index, item);
		}
		protected virtual void OnModified() { }
		protected virtual void OnItemInserted(int index, T item) {
			OnModified();
		}
		protected virtual void OnItemRemoved(int index, T item) {
			OnModified();
		}
		public void CopyTo(T[] array, int arrayIndex) {
			innerList.CopyTo(array, arrayIndex);
		}
		public virtual void Sort(int index, int count, IComparer<T> comparer) {
			innerList.Sort(index, count, comparer);
		}
	}
	#endregion
	#region UndoableCollection
	public class UndoableCollection<T> : SimpleCollection<T> {
		#region Fields
		readonly IDocumentModelPart documentModelPart;
		#endregion
		public UndoableCollection(IDocumentModelPart documentModelPart)
			: base() {
			this.documentModelPart = documentModelPart;
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return documentModelPart.DocumentModel; } }
		public IDocumentModelPart DocumentModelPart { get { return documentModelPart; } }
		#endregion
		public override int Add(T item) {
			Guard.ArgumentNotNull(item, "item");
			UndoableCollectionAddHistoryItem<T> historyItem = new UndoableCollectionAddHistoryItem<T>(DocumentModel, this, item);
			this.DocumentModel.History.Add(historyItem);
			historyItem.Execute();
			return InnerList.Count - 1;
		}
		public override void Insert(int index, T item) {
			Guard.ArgumentNotNull(item, "item");
			ValueChecker.CheckValue(index, 0, Count, "index");
			UndoableCollectionInsertHistoryItem<T> historyItem = new UndoableCollectionInsertHistoryItem<T>(DocumentModel, this, index, item);
			this.DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public override void RemoveAt(int index) {
			if (Count == 0)
				throw new ArgumentOutOfRangeException("index value out of range");
			ValueChecker.CheckValue(index, 0, Count - 1, "index");
			UndoableCollectionRemoveAtHistoryItem<T> historyItem = new UndoableCollectionRemoveAtHistoryItem<T>(DocumentModel, this, index);
			this.DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public override void Clear() {
			if (Count == 0)
				return;
			UndoableCollectionClearHistoryItem<T> historyItem = new UndoableCollectionClearHistoryItem<T>(DocumentModel, this);
			this.DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void Move(int sourceIndex, int targetIndex) {
			if (Count == 0)
				throw new ArgumentOutOfRangeException("index value out of range");
			ValueChecker.CheckValue(sourceIndex, 0, Count - 1, "sourceIndex");
			ValueChecker.CheckValue(targetIndex, 0, Count - 1, "targetIndex");
			if (sourceIndex == targetIndex)
				return;
			UndoableCollectionMoveHistoryItem<T> historyItem = new UndoableCollectionMoveHistoryItem<T>(DocumentModel, this, sourceIndex, targetIndex);
			this.DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public virtual int AddWithoutHistoryAndNotifications(T item) {
			return AddWithoutNotification(item);
		}
		public  virtual int AddCore(T item) {
			RaiseAdd(item);
			return AddInternal(item);
		}
		public virtual int AddInternal(T item) {
			return base.Add(item);
		}
		public virtual void AddRangeCore(IEnumerable<T> collection) {
			RaiseAddRange(collection);
			this.InnerList.AddRange(collection);
		}
		protected internal virtual void InsertCore(int index, T item) {
			RaiseInsert(index, item);
			InsertInternal(index, item);
		}
		protected internal virtual void InsertInternal(int index, T item) {
			base.Insert(index, item);
		}
		public virtual void RemoveAtCore(int index) {
			RaiseRemoveAt(index);
			RemoveAtInternal(index);
		}
		protected internal virtual void RemoveAtInternal(int index) {
			base.RemoveAt(index);
		}
		public virtual void ClearCore() {
			RaiseClear();
			base.Clear();
		}
		public virtual void MoveCore(int sourceIndex, int targetIndex) {
			RaiseMove(sourceIndex, targetIndex);
			T item = InnerList[sourceIndex];
			RemoveAtInternal(sourceIndex);
			InsertInternal(targetIndex, item);
		}
		protected virtual EventArgs CreateAddEventArgs(T item) {
			return new UndoableCollectionAddEventArgs<T>(item);
		}
		protected virtual EventArgs CreateInsertEventArgs(int index, T item) {
			return new UndoableCollectionInsertEventArgs<T>(index, item);
		}
		protected virtual EventArgs CreateAddRangeEventArgs(IEnumerable<T> collection) {
			return new UndoableCollectionAddRangeEventArgs<T>(collection);
		}
		#region OnAdd
		UndoableCollectionAddEventHandler onAdd;
		public event UndoableCollectionAddEventHandler OnAdd { add { onAdd += value; } remove { onAdd -= value; } }
		protected internal void RaiseAdd(T item) {
			if (onAdd != null)
				onAdd(this, CreateAddEventArgs(item));
		}
		#endregion
		#region OnRemoveAt
		UndoableCollectionRemoveAtEventHandler onRemoveAt;
		public event UndoableCollectionRemoveAtEventHandler OnRemoveAt { add { onRemoveAt += value; } remove { onRemoveAt -= value; } }
		protected internal void RaiseRemoveAt(int index) {
			if (onRemoveAt != null)
				onRemoveAt(this, new UndoableCollectionRemoveAtEventArgs(index));
		}
		#endregion
		#region OnInsert
		UndoableCollectionInsertEventHandler onInsert;
		public event UndoableCollectionInsertEventHandler OnInsert { add { onInsert += value; } remove { onInsert -= value; } }
		protected internal void RaiseInsert(int index, T item) {
			if (onInsert != null)
				onInsert(this, CreateInsertEventArgs(index, item));
		}
		#endregion
		#region OnClear
		UndoableCollectionClearEventHandler onClear;
		public event UndoableCollectionClearEventHandler OnClear { add { onClear += value; } remove { onClear -= value; } }
		protected internal void RaiseClear() {
			if (onClear != null)
				onClear(this);
		}
		#endregion
		#region OnAddRange
		UndoableCollectionAddRangeEventHandler onAddRange;
		public event UndoableCollectionAddRangeEventHandler OnAddRange { add { onAddRange += value; } remove { onAddRange -= value; } }
		protected internal void RaiseAddRange(IEnumerable<T> collection) {
			if (onAddRange != null)
				onAddRange(this, CreateAddRangeEventArgs(collection));
		}
		#endregion
		#region OnMove
		UndoableCollectionMoveEventHandler onMove;
		public event UndoableCollectionMoveEventHandler OnMove { add { onMove += value; } remove { onMove -= value; } }
		protected internal void RaiseMove(int sourceIndex, int targetIndex) {
			if (onMove != null)
				onMove(this, new UndoableCollectionMoveEventArgs(sourceIndex, targetIndex));
		}
		#endregion
	}
	#endregion
	#region UndoableClonableCollection (abstract class)
	public abstract class UndoableClonableCollection<T> : UndoableCollection<T>, ICloneable<UndoableClonableCollection<T>>, ISupportsCopyFrom<UndoableClonableCollection<T>> {
		protected UndoableClonableCollection(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		#region ICloneable<UndoableClonableCollection<T>> Members
		public UndoableClonableCollection<T> Clone() {
			UndoableClonableCollection<T> result = GetNewCollection(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<UndoableEquitableClonableCollection<T>> Members
		public void CopyFrom(UndoableClonableCollection<T> value) {
			ClearCore();
			int count = value.Count;
			for (int i = 0; i < count; i++)
				AddInternal(GetCloneItem(value[i], DocumentModelPart));
		}
		#endregion
		public abstract UndoableClonableCollection<T> GetNewCollection(IDocumentModelPart documentModelPart);
		public abstract T GetCloneItem(T item, IDocumentModelPart documentModelPart);
		#region Equals
		public override bool Equals(object obj) {
			UndoableClonableCollection<T> value = obj as UndoableClonableCollection<T>;
			if (value == null)
				return false;
			if (Count != value.Count)
				return false;
			for (int i = 0; i < Count; i++)
				if (!this[i].Equals(value[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			int result = GetType().GetHashCode() ^ Count;
			for (int i = 0; i < Count; i++)
				result ^= this[i].GetHashCode();
			return result;
		}
		#endregion
	}
	#endregion
	#region HistoryItems
	#region UndoableCollectionAddHistoryItem
	public class UndoableCollectionAddHistoryItem<T> : HistoryItem {
		readonly UndoableCollection<T> collection;
		T item;
		public UndoableCollectionAddHistoryItem(IDocumentModel documentModel, UndoableCollection<T> collection, T item)
			: base(documentModel.MainPart) {
			this.collection = collection;
			this.item = item;
		}
		protected override void UndoCore() {
			int index = this.collection.Count - 1;
			this.collection.RemoveAtCore(index);
		}
		protected override void RedoCore() {
			this.collection.AddCore(this.item);
		}
	}
	#endregion
	#region UndoableCollectionInsertHistoryItem
	public class UndoableCollectionInsertHistoryItem<T> : HistoryItem {
		UndoableCollection<T> collection;
		int index;
		T item;
		public UndoableCollectionInsertHistoryItem(IDocumentModel documentModel, UndoableCollection<T> collection, int index, T item)
			: base(documentModel.MainPart) {
			this.collection = collection;
			this.index = index;
			this.item = item;
		}
		protected override void UndoCore() {
			this.collection.RemoveAtCore(index);
		}
		protected override void RedoCore() {
			this.collection.InsertCore(this.index, this.item);
		}
	}
	#endregion
	#region UndoableCollectionRemoveAtHistoryItem
	public class UndoableCollectionRemoveAtHistoryItem<T> : HistoryItem {
		UndoableCollection<T> collection;
		int index;
		T item;
		public UndoableCollectionRemoveAtHistoryItem(IDocumentModel documentModel, UndoableCollection<T> collection, int index)
			: base(documentModel.MainPart) {
			this.collection = collection;
			this.index = index;
		}
		protected override void UndoCore() {
			this.collection.InsertCore(index, this.item);
		}
		protected override void RedoCore() {
			this.item = this.collection[index];
			this.collection.RemoveAtCore(index);
		}
	}
	#endregion
	#region UndoableCollectionClearHistoryItem
	public class UndoableCollectionClearHistoryItem<T> : HistoryItem {
		readonly UndoableCollection<T> collection;
		List<T> itemList;
		public UndoableCollectionClearHistoryItem(IDocumentModel documentModel, UndoableCollection<T> collection)
			: base(documentModel.MainPart) {
			this.collection = collection;
			this.itemList = new List<T>();
			this.itemList.AddRange(this.collection.InnerList);
		}
		protected override void UndoCore() {
			this.collection.AddRangeCore(itemList);
		}
		protected override void RedoCore() {
			this.collection.ClearCore();
		}
	}
	#endregion
	#region UndoableCollectionMoveHistoryItem
	public class UndoableCollectionMoveHistoryItem<T> : HistoryItem {
		readonly UndoableCollection<T> collection;
		int sourceIndex;
		int targetIndex;
		public UndoableCollectionMoveHistoryItem(IDocumentModel documentModel, UndoableCollection<T> collection, int sourceIndex, int targetIndex)
			: base(documentModel.MainPart) {
			this.collection = collection;
			this.sourceIndex = sourceIndex;
			this.targetIndex = targetIndex;
		}
		protected override void UndoCore() {
			this.collection.MoveCore(targetIndex, sourceIndex);
		}
		protected override void RedoCore() {
			this.collection.MoveCore(sourceIndex, targetIndex);
		}
	}
	#endregion
	#endregion
}
