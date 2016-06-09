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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	public interface PivotItemCollection : ISimpleCollection<PivotItem> {
		bool Contains(PivotItem item);
		int IndexOf(PivotItem item);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativePivotItemCollection
	partial class NativePivotItemCollection : NativeObjectBase, PivotItemCollection {
		#region Fields
		readonly List<NativePivotItem> innerList;
		readonly NativePivotField parentPivotField;
		#endregion
		public NativePivotItemCollection(NativePivotField parentPivotField) {
			Guard.ArgumentNotNull(parentPivotField, "parentPivotField");
			this.innerList = new List<NativePivotItem>();
			this.parentPivotField = parentPivotField;
			Populate();
			SubscribeEvents();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return innerList.Count;
			}
		}
		public PivotItem this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		protected internal Model.PivotItemCollection ModelCollection { get { return parentPivotField.ModelItem.Items; } }
		protected internal NativePivotField Parent { get { return parentPivotField; } }
		#endregion
		#region Internal
		void Populate() {
			ModelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(Model.PivotItem item) {
			if (item.IsDataItem)
				innerList.Add(CreateNativeObject(item, Count));
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value) {
				InvalidateItems();
				UnsubscribeEvents();
			}
		}
		void InvalidateItems() {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				innerList[i].IsValid = false;
		}
		NativePivotItem CreateNativeObject(Model.PivotItem item, int index) {
			NativePivotItem nativeItem = new NativePivotItem(item, Parent);
			nativeItem.ItemIndex = index;
			return nativeItem;
		}
		#endregion
		#region SubscribeEvents
		void SubscribeEvents() {
			ModelCollection.OnAdd += OnAdd;
			ModelCollection.OnRemoveAt += OnRemoveAt;
			ModelCollection.OnInsert += OnInsert;
			ModelCollection.OnClear += OnClear;
			ModelCollection.OnAddRange += OnAddRange;
			ModelCollection.OnMove += OnMove;
		}
		void UnsubscribeEvents() {
			ModelCollection.OnAdd -= OnAdd;
			ModelCollection.OnRemoveAt -= OnRemoveAt;
			ModelCollection.OnInsert -= OnInsert;
			ModelCollection.OnClear -= OnClear;
			ModelCollection.OnAddRange -= OnAddRange;
			ModelCollection.OnMove -= OnMove;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.PivotItem> modelArgs = e as UndoableCollectionAddEventArgs<Model.PivotItem>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				innerList[index].IsValid = false;
				CorrectIndexes(index, DecrementIndex);
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<Model.PivotItem> modelArgs = e as UndoableCollectionInsertEventArgs<Model.PivotItem>;
			if (modelArgs != null) {
				int index = modelArgs.Index;
				innerList.Insert(modelArgs.Index, CreateNativeObject(modelArgs.Item, index));
				CorrectIndexes(index, IncrementIndex);
			}
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.PivotItem> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.PivotItem>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.PivotItem> collection = modelArgs.Collection;
			foreach (Model.PivotItem modelItem in collection)
				RegisterItem(modelItem);
		}
		void OnMove(object sender, EventArgs e) {
			UndoableCollectionMoveEventArgs modelArgs = e as UndoableCollectionMoveEventArgs;
			if (modelArgs == null)
				return;
			int sourceIndex = modelArgs.SourceIndex;
			int targetIndex = modelArgs.TargetIndex;
			NativePivotItem item = innerList[sourceIndex];
			CorrectIndexes(sourceIndex, DecrementIndex);
			innerList.Remove(item);
			item.ItemIndex = targetIndex;
			innerList.Insert(targetIndex, item);
			CorrectIndexes(targetIndex, IncrementIndex);
		}
		void CorrectIndexes(int startIndex, Action<NativePivotItem> action) {
			for (int i = startIndex + 1; i < Count; i++)
				action(innerList[i]);
		}
		void IncrementIndex(NativePivotItem nativePivotItem) {
			nativePivotItem.ItemIndex++;
		}
		void DecrementIndex(NativePivotItem nativePivotItem) {
			nativePivotItem.ItemIndex--;
		}
		#endregion
		#region ISimpleCollection<PivotItem> Members
		public IEnumerator<PivotItem> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<PivotItem, NativePivotItem>(innerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return innerList.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		bool ICollection.IsSynchronized {
			get {
				CheckValid();
				return ((IList)this.innerList).IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				CheckValid();
				return ((IList)this.innerList).SyncRoot;
			}
		}
		#endregion
		#region PivotItemCollection Members
		public bool Contains(PivotItem item) {
			return IndexOf(item) != -1;
		}
		public int IndexOf(PivotItem item) {
			CheckValid();
			NativePivotItem nativeItem = (NativePivotItem)item;
			if (nativeItem != null)
				return innerList.IndexOf(nativeItem);
			return -1;
		}
		#endregion
	}
	#endregion
}
