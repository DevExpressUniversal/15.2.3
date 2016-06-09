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

using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotDataFieldsCollection
	public class PivotDataFieldsCollection : UndoableCollection<PivotDataField> {
		public PivotDataFieldsCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
	}
	#endregion
	#region PivotFieldCollection
	public class PivotFieldCollection : UndoableCollection<PivotField> {
		public PivotFieldCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Notification
		public IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			IModelErrorInfo info = null;
			foreach (PivotField field in this)
				info = field.CanRangeInsert(range, mode) ?? info;
			return info;
		}
		public IModelErrorInfo CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			IModelErrorInfo info = null;
			foreach (PivotField field in this)
				info = field.CanRangeRemove(range, mode) ?? info;
			return info;
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach (PivotField field in this)
				field.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			foreach (PivotField field in this)
				field.OnRangeRemoving(context);
		}
		#endregion
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotFieldCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotField item in source) {
				PivotField newItem = new PivotField(newPivot);
				newItem.CopyFromNoHistory(newPivot, offset, item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotItemCollection
	public class PivotItemCollection : UndoableCollection<PivotItem> {
		int dataItemsCount;
		int hiddenItemsCount = 0;
		int collapsedItemsCount = 0;
		public PivotItemCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public int DataItemsCount { get { return dataItemsCount; } }
		public int HiddenItemsCount { get { return hiddenItemsCount; } }
		public int CollapsedItemsCount { get { return collapsedItemsCount; } }
		public bool HasCollapsedDataItems { get { return collapsedItemsCount > 0; } }
		public bool HasExpandedDataItems { get { return collapsedItemsCount < DataItemsCount; } }
		public bool AllDataItemsAreCollapsed { get { return collapsedItemsCount >= DataItemsCount; } }
		public bool AllDataItemsAreExpanded { get { return collapsedItemsCount <= 0; } }
		protected override void OnItemInserted(int index, PivotItem item) {
			base.OnItemInserted(index, item);
			item.HiddenChanged += item_HiddenChanged;
			item.HideDetailsChanged += item_HideDetailsChanged;
			IncrementDataItemsCount(item);
		}
		protected override void OnItemRemoved(int index, PivotItem item) {
			base.OnItemRemoved(index, item);
			item.HiddenChanged -= item_HiddenChanged;
			item.HideDetailsChanged -= item_HideDetailsChanged;
			DecrementDataItemsCount(item);
		}
		void item_HiddenChanged(object sender, EventArgs e) {
			PivotItem item = sender as PivotItem;
			if (item == null)
				return;
			if (item.ItemType == PivotFieldItemType.Data) {
				if (item.IsHidden)
					hiddenItemsCount++;
				else
					hiddenItemsCount--;
			}
		}
		void item_HideDetailsChanged(object sender, EventArgs e) {
			PivotItem item = sender as PivotItem;
			if (item == null)
				return;
			if (item.ItemType == PivotFieldItemType.Data) {
				if (item.HideDetails)
					collapsedItemsCount++;
				else
					collapsedItemsCount--;
			}
		}
		public override void AddRangeCore(IEnumerable<PivotItem> collection) {
			foreach (PivotItem item in collection) {
				IncrementDataItemsCount(item);
				item.HiddenChanged += item_HiddenChanged;
				item.HideDetailsChanged += item_HideDetailsChanged;
			}
			base.AddRangeCore(collection);
		}
		public override void ClearCore() {
			foreach (PivotItem item in this) {
				item.HiddenChanged -= item_HiddenChanged;
				item.HideDetailsChanged -= item_HideDetailsChanged;
			}
			dataItemsCount = 0;
			hiddenItemsCount = 0;
			collapsedItemsCount = 0;
			base.ClearCore();
		}
		void IncrementDataItemsCount(PivotItem item) {
			if (item.IsDataItem) {
				dataItemsCount++;
				if (item.IsHidden)
					hiddenItemsCount++;
				if (item.HideDetails)
					collapsedItemsCount++;
			}
		}
		void DecrementDataItemsCount(PivotItem item) {
			if (item.IsDataItem) {
				dataItemsCount--;
				if (item.IsHidden)
					hiddenItemsCount--;
				if (item.HideDetails)
					collapsedItemsCount--;
			}
		}
		internal List<int> GetVisibleItemIndexes() {
			List<int> result = new List<int>();
			for (int i = 0; i < dataItemsCount; i++)
				if (!this[i].IsHidden)
					result.Add(i);
			return result;
		}
		internal int GetVisibleItemsCount() {
			return dataItemsCount - hiddenItemsCount;
		}
		internal IEnumerator<PivotItem> GetVisibleItemsEnumerator() {
			for (int i = 0; i < dataItemsCount; i++) {
				PivotItem item = this[i];
				if (!item.IsHidden)
					yield return item;
			}
		}
		internal void UnhideAll() {
			foreach (PivotItem item in this)
				item.IsHidden = false;
			hiddenItemsCount = 0;
		}
		internal bool IsAllItemExpanded() {
			foreach (PivotItem item in this)
				if (item.HideDetails)
					return false;
			return true;
		}
		internal bool IsAllItemCollapsed() {
			foreach (PivotItem item in this)
				if (!item.HideDetails)
					return false;
			return true;
		}
		public void CopyFromNoHistory(PivotTable newPivot, PivotItemCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotItem item in source) {
				PivotItem newItem = new PivotItem(newPivot);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
			Debug.Assert(dataItemsCount == source.dataItemsCount);
			Debug.Assert(hiddenItemsCount == source.hiddenItemsCount);
			Debug.Assert(collapsedItemsCount == source.collapsedItemsCount);
		}
	}
	#endregion
	#region PivotFilterCollection
	public class PivotFilterCollection : UndoableCollection<PivotFilter> {
		public PivotFilterCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		internal void CorrectMeasureFieldIndexesAfterMove(int dataFieldSourceIndex, int dataFieldTargetIndex) {
			for (int i = 0; i < Count; i++) {
				PivotFilter filter = InnerList[i];
				if (!filter.IsMeasureFilter)
					continue;
				int measureFieldIndex = filter.MeasureFieldIndex.Value;
				if (measureFieldIndex == dataFieldSourceIndex)
					filter.MeasureFieldIndex = dataFieldTargetIndex;
				else if (dataFieldTargetIndex > dataFieldSourceIndex && measureFieldIndex > dataFieldSourceIndex && measureFieldIndex <= dataFieldTargetIndex)
					filter.MeasureFieldIndex--;
				else if (dataFieldSourceIndex > dataFieldTargetIndex && measureFieldIndex >= dataFieldTargetIndex && measureFieldIndex < dataFieldSourceIndex)
					filter.MeasureFieldIndex++;
			}
		}
		public void CopyFromNoHistory(PivotFilterCollection source) {
			ClearCore();
			Capacity = source.Count;
			DocumentModel documentModel = (DocumentModel)DocumentModel;
			foreach (PivotFilter item in source) {
				PivotFilter newItem = new PivotFilter(documentModel);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
		public void SetupPivotInfo(PivotTableStaticInfo info, bool multipleFieldFilters) {
			PivotFilter measureFilter = null;
			PivotFilter labelFilter = null;
			for (int i = 0; i < Count; i++) {
				PivotFilter filter = InnerList[i];
				if (filter.FieldIndex != info.FieldIndex)
					continue;
				if (filter.IsMeasureFilter)
					measureFilter = filter;
				else
					labelFilter = filter;
				if (!multipleFieldFilters)
					break;
			}
			info.LabelFilter = labelFilter;
			info.MeasureFilter = measureFilter;
		}
	}
	#endregion
	#region PivotAreaReferenceCollection
	public class PivotAreaReferenceCollection : UndoableCollection<PivotAreaReference> {
		public PivotAreaReferenceCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFrom(PivotAreaReferenceCollection sourceCollection, DocumentModel modelForNewItems) {
			this.Capacity = sourceCollection.Count;
			foreach (PivotAreaReference sourceItem in sourceCollection) {
				var newItem = sourceItem.Clone(modelForNewItems);
				this.Add(newItem);
			}
		}
		public void CopyFromNoHistory(PivotTable newPivot, PivotAreaReferenceCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotAreaReference item in source) {
				PivotAreaReference newItem = new PivotAreaReference(newPivot.DocumentModel);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotFormat
	public class PivotFormatCollection : UndoableCollection<PivotFormat> {
		public PivotFormatCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotTable newPivot, PivotFormatCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotFormat item in source) {
				PivotFormat newItem = new PivotFormat(newPivot.DocumentModel);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotConditionalFormatCollection
	public class PivotConditionalFormatCollection : UndoableCollection<PivotConditionalFormat> {
		public PivotConditionalFormatCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotConditionalFormatCollection source) {
			ClearCore();
			Capacity = source.Capacity;
			foreach (PivotConditionalFormat item in source) {
				PivotConditionalFormat newItem = new PivotConditionalFormat(newPivot.DocumentModel);
				newItem.CopyFromNoHistory(newPivot, offset, item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotAreaCollection
	public class PivotAreaCollection : UndoableCollection<PivotArea> {
		public PivotAreaCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotAreaCollection source) {
			ClearCore();
			Capacity = source.Capacity;
			foreach (PivotArea item in source) {
				PivotArea newItem = new PivotArea(newPivot.DocumentModel);
				newItem.CopyFromNoHistory(newPivot, offset, item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotChartFormatsCollection
	public class PivotChartFormatsCollection : UndoableCollection<PivotChartFormat> {
		public PivotChartFormatsCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotChartFormatsCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotChartFormat item in source) {
				PivotChartFormat newItem = new PivotChartFormat(newPivot.DocumentModel);
				newItem.CopyFromNoHistory(newPivot, offset, item);
				AddCore(newItem);
			}
		}
	}
	#endregion
}
