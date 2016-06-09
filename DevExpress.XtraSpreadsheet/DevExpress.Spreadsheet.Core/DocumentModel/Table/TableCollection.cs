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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
using DevExpress.Office.History;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableCollection
	public class TableCollection : UndoableNamedItemTreeCollection<Table>, ITableCollection {
		public TableCollection(IDocumentModelPart documentModel)
			: base(documentModel, StringExtensions.ComparerInvariantCultureIgnoreCase) {
		}
		#region Properties
		protected internal DocumentModel Workbook { get { return (DocumentModel)base.DocumentModel; } }
		#endregion
		public override Table GetCloneItem(Table item, IDocumentModelPart documentModelPart) {
			throw new ArgumentException("not implemented");
		}
		protected override void OnModified() {
			base.OnModified();
			Workbook.IncrementContentVersion();
		}
		public override UndoableClonableCollection<Table> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new TableCollection(documentModelPart);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = InnerList.Count - 1; i >= 0; i--) {
				Table table = InnerList[i];
				if (!table.OnRangeRemoving(context)) {
					table.Worksheet.RemoveTableAt(i);
				}
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			for (int i = InnerList.Count - 1; i >= 0; i--)
				InnerList[i].OnRangeInserting(notificationContext);
		}
		public void OnCellValueChanged(ICell cell) {
			for (int i = 0; i < InnerList.Count; i++)
				if (InnerList[i].OnCellValueChanged(cell))
					return;
		}
		public void OnCellRemoved(ICell cell) {
			for (int i = 0; i < InnerList.Count; i++)
				if (InnerList[i].OnCellRemoved(cell))
					return;
		}
		public bool CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			if (mode == RemoveCellMode.Default || mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				return true;
			int intersectedTablesCount = 0;
			int includedTablesCount = 0;
			bool result = true;
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				Table table = InnerList[i];
				if ((range.RangeType != CellRangeType.IntervalRange || !((CellIntervalRange)range).IsColumnInterval) && !table.CanRangeRemove(range, mode))
					return false;
				CellRange tableRange = table.Range;
				if (range.Includes(tableRange))
					includedTablesCount++;
				if (tableRange.Intersects(range))
					intersectedTablesCount++;
				result = intersectedTablesCount <= 1 || intersectedTablesCount - includedTablesCount == 0;
				if (!result)
					return false;
			}
			return result;
		}
		public bool CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			int intersectedTablesCount = 0;
			int includedTablesCount = 0;
			int intersectedFirstTableRangeCount = 0;
			bool result = true;
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				Table table = InnerList[i];
				if (!table.CanRangeInsert(range, mode))
					return false;
				CellRange tableRange = table.Range;
				if (range.Includes(tableRange))
					includedTablesCount++;
				bool hasIntersept = tableRange.Intersects(range);
				if (hasIntersept)
					intersectedTablesCount++;
				if (CheckIncreaseIntersectedFirstTableRangeCount(tableRange, range, hasIntersept))
					intersectedFirstTableRangeCount++;
				result =
					intersectedTablesCount <= 1 ||
					intersectedTablesCount - includedTablesCount == 0 ||
					intersectedTablesCount == intersectedFirstTableRangeCount;
				if (!result)
					return false;
			}
			return result;
		}
		protected override void OnItemInserted(int index, Table item) {
			item.DocumentModel.InternalAPI.OnTableAdd(item.Worksheet, item);
			base.OnItemInserted(index, item);
		}
		protected override void OnItemRemoved(int index, Table item) {
			item.DocumentModel.InternalAPI.OnTableRemoveAt(item.Worksheet, index);
			base.OnItemRemoved(index, item);
		}
		bool CheckIncreaseIntersectedFirstTableRangeCount(CellRange tableRange, CellRangeBase range, bool hasIntersept) {
			if (!hasIntersept)
				return false;
			CellIntervalRange intervalRange = range as CellIntervalRange;
			if (intervalRange == null)
				return false;
			if (intervalRange.IsColumnInterval)
				return intervalRange.LeftColumnIndex <= tableRange.LeftColumnIndex;
			return intervalRange.TopRowIndex <= tableRange.TopRowIndex;
		}
		protected internal override void CheckIntegrity(CheckIntegrityFlags flags) {
			base.CheckIntegrity(flags);
			for (int i = 0; i < InnerList.Count; i++)
				InnerList[i].CheckIntegrity(flags);
		}
		public CellUnion GetTablesAsUnionRange(CellRange rangeSource, bool orIntersects) {
			List<Table> tablesIntersectedSourceRange = (rangeSource.Worksheet as Worksheet).Tables.GetItems(rangeSource, orIntersects);
			List<CellRangeBase> cellRanges = new List<CellRangeBase>();
			foreach (Table item in tablesIntersectedSourceRange) {
				cellRanges.Add(item.Range);
			}
			return new CellUnion(cellRanges);
		}
		public bool CanInsertSubtotal(CellRange range) {
			if (range != null)
				foreach (Table item in InnerList)
					if (item.Range.Intersects(range))
						return false;
			return true;
		}
		protected internal void OnBeforeRangeRemoving(RemoveRangeNotificationContext context) {
			if (context.Mode == RemoveCellMode.ShiftCellsUp) {
				Table table = GetFirstTable(context);
				if (table != null)
					table.OnBeforeRangeRemoving(context);
			}
		}
		Table GetFirstTable(RemoveRangeNotificationContext context) {
			List<Table> tables = GetItems(context.Range, true);
			return tables.Count > 0 ? tables[0] : null;
		}
		internal void MarkupTablesForInvalidateFormatCache(CellRange range) {
			MarkupTablesForInvalidateFormatCacheHistoryItem item = new MarkupTablesForInvalidateFormatCacheHistoryItem(this, range);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		internal void InvalidateFormatCache(CellRange range) {
			List<Table> tables = GetItems(range, true);
			int count = tables.Count;
			for (int i = 0; i < count; i++)
				tables[i].StyleFormatCache.SetInvalid();
		}
		#region GetUniqueIntersectedItems
		public ICollection<Table> GetUniqueIntersectedItems(IList<CellRange> ranges) {
			ICollection<Table> result = new HashSet<Table>();
			int rangesCount = ranges.Count;
			for (int i = 0; i < rangesCount; i++) {
				List<Table> intersectedItems = GetItems(ranges[i], true);
				int intersectedItemsCount = intersectedItems.Count;
				for (int j = 0; j < intersectedItemsCount; j++) {
					Table intersectedItem = intersectedItems[j];
					if (!result.Contains(intersectedItem))
						result.Add(intersectedItem);
				}
			}
			return result;
		}
		#endregion
	}	
	#endregion
	#region TablesCachedRTree
	public class RangeObjectCachedRTree<T> : CellRangeCachedRTreeBase<T> where T : class, ISpreadsheetRangeObject {
		protected override CellRange GetItemRange(T item) {
			return item.Range;
		}
	}
	#endregion
}
