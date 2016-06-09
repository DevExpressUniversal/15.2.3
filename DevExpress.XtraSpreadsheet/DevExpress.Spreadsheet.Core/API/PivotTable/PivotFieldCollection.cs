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
	#region PivotFieldCollection
	public interface PivotFieldCollection : ISimpleCollection<PivotField> {
		PivotField this[string name] { get; }
		bool Contains(PivotField field);
		int IndexOf(PivotField field);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativePivotFieldCollection
	partial class NativePivotFieldCollection : NativeObjectBase, PivotFieldCollection {
		#region Fields
		readonly List<NativePivotField> innerList;
		readonly NativePivotTable parentPivotTable;
		#endregion
		public NativePivotFieldCollection(NativePivotTable parentPivotTable) {
			Guard.ArgumentNotNull(parentPivotTable, "parentPivotTable");
			this.innerList = new List<NativePivotField>();
			this.parentPivotTable = parentPivotTable;
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
		public PivotField this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		protected internal Model.PivotFieldCollection ModelCollection { get { return parentPivotTable.ModelItem.Fields; } }
		protected internal NativePivotTable Parent { get { return parentPivotTable; } }
		#endregion
		#region Internal
		void Populate() {
			ModelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(Model.PivotField item) {
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
		NativePivotField CreateNativeObject(Model.PivotField item, int index) {
			NativePivotField nativeItem = new NativePivotField(item, Parent);
			nativeItem.FieldIndex = index;
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
		}
		void UnsubscribeEvents() {
			ModelCollection.OnAdd -= OnAdd;
			ModelCollection.OnRemoveAt -= OnRemoveAt;
			ModelCollection.OnInsert -= OnInsert;
			ModelCollection.OnClear -= OnClear;
			ModelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.PivotField> modelArgs = e as UndoableCollectionAddEventArgs<Model.PivotField>;
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
			UndoableCollectionInsertEventArgs<Model.PivotField> modelArgs = e as UndoableCollectionInsertEventArgs<Model.PivotField>;
			if (modelArgs != null) {
				int index = modelArgs.Index;
				innerList.Insert(index, CreateNativeObject(modelArgs.Item, index));
				CorrectIndexes(index, IncrementIndex);
			}
		}
		void CorrectIndexes(int startIndex, Action<NativePivotField> action) {
			for (int i = startIndex + 1; i < Count; i++)
				action(innerList[i]);
		}
		void IncrementIndex(NativePivotField nativePivotField) {
			nativePivotField.FieldIndex++;
		}
		void DecrementIndex(NativePivotField nativePivotField) {
			nativePivotField.FieldIndex--;
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.PivotField> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.PivotField>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.PivotField> collection = modelArgs.Collection;
			foreach (Model.PivotField modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<PivotField> Members
		public IEnumerator<PivotField> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<PivotField, NativePivotField>(innerList.GetEnumerator());
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
		#region PivotFieldCollection Members
		public PivotField this[string name] {
			get {
				CheckValid();
				for (int i = 0; i < Count; i++) {
					NativePivotField item = innerList[i];
					if (StringExtensions.ComparerInvariantCultureIgnoreCase.Equals(name, item.Name))
						return item;
				}
				return null;
			}
		}
		public bool Contains(PivotField field) {
			return IndexOf(field) != -1;
		}
		public int IndexOf(PivotField field) {
			CheckValid();
			NativePivotField nativeItem = (NativePivotField)field;
			if (nativeItem != null)
				return innerList.IndexOf(nativeItem);
			return -1;
		}
		#endregion
	}
	#endregion
}
