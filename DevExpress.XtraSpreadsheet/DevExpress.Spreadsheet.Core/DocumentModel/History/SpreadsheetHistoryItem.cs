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
using DevExpress.Office.History;
using DevExpress.Office;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	public abstract class SpreadsheetHistoryItem : HistoryItem {
		protected SpreadsheetHistoryItem(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		public Worksheet Worksheet { get { return (Worksheet)DocumentModelPart; } }
		public DocumentModel Workbook { get { return (DocumentModel)base.DocumentModel; } }
	}
	public abstract class SpreadsheetWorkbookHistoryItem : HistoryItem {
		protected SpreadsheetWorkbookHistoryItem(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		public DocumentModel Workbook { get { return (DocumentModel)base.DocumentModel; } }
	}
	#region SpreadsheetSimpleTypeHistoryItem<T>
	public abstract class SpreadsheetSimpleTypeHistoryItem<T> : SpreadsheetHistoryItem {
		#region Fields
		readonly T oldValue;
		readonly T newValue;
		#endregion
		protected SpreadsheetSimpleTypeHistoryItem(IDocumentModelPart documentModelPart, T oldValue, T newValue)
			: base(documentModelPart) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public T OldValue { get { return oldValue; } }
		public T NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SpreadsheetStringHistoryItem
	public abstract class SpreadsheetStringHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly string oldValue;
		readonly string newValue;
		#endregion
		protected SpreadsheetStringHistoryItem(Worksheet worksheet, string oldValue, string newValue)
			: base(worksheet) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected SpreadsheetStringHistoryItem(DocumentModel documentModel, string oldValue, string newValue)
			: base(documentModel) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public string OldValue { get { return oldValue; } }
		public string NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SpreadsheetBooleanHistoryItem
	public abstract class SpreadsheetBooleanHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly bool oldValue;
		readonly bool newValue;
		#endregion
		protected SpreadsheetBooleanHistoryItem(Worksheet worksheet, bool oldValue, bool newValue)
			: base(worksheet) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected SpreadsheetBooleanHistoryItem(DocumentModel documentModel, bool oldValue, bool newValue)
			: base(documentModel) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public bool OldValue { get { return oldValue; } }
		public bool NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SpreadsheetDoubleHistoryItem
	public abstract class SpreadsheetDoubleHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly double oldValue;
		readonly double newValue;
		#endregion
		protected SpreadsheetDoubleHistoryItem(Worksheet worksheet, double oldValue, double newValue)
			: base(worksheet) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected SpreadsheetDoubleHistoryItem(DocumentModel documentModel, double oldValue, double newValue)
			: base(documentModel) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public double OldValue { get { return oldValue; } }
		public double NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SpreadsheetCellRangeHistoryItem
	public abstract class SpreadsheetCellRangeHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly CellRange oldRange;
		readonly CellRange newRange;
		#endregion
		protected SpreadsheetCellRangeHistoryItem(Worksheet worksheet, CellRange oldRange, CellRange newRange)
			: base(worksheet) {
			this.oldRange = oldRange;
			this.newRange = newRange;
		}
		protected SpreadsheetCellRangeHistoryItem(DocumentModel documentModel, CellRange oldRange, CellRange newRange)
			: base(documentModel) {
			this.oldRange = oldRange;
			this.newRange = newRange;
		}
		#region Properties
		public CellRange OldRange { get { return oldRange; } }
		public CellRange NewRange { get { return newRange; } }
		#endregion
	}
	#endregion
	#region SpreadsheetIntHistoryItem
	public abstract class SpreadsheetIntHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int oldValue;
		readonly int newValue;
		#endregion
		protected SpreadsheetIntHistoryItem(Worksheet worksheet, int oldValue, int newValue)
			: base(worksheet) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected SpreadsheetIntHistoryItem(DocumentModel workbook, int oldValue, int newValue)
			: base(workbook) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public int OldValue { get { return oldValue; } }
		public int NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SpreadsheetCellPositionHistoryItem
	public abstract class SpreadsheetCellPositionHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly CellPosition oldValue;
		readonly CellPosition newValue;
		#endregion
		protected SpreadsheetCellPositionHistoryItem(Worksheet worksheet, CellPosition oldValue, CellPosition newValue)
			: base(worksheet) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public CellPosition OldValue { get { return oldValue; } }
		public CellPosition NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SpreadsheetColumnInsertedHistoryItem
	public class SpreadsheetColumnInsertedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly Column column;
		readonly int position;
		#endregion
		public SpreadsheetColumnInsertedHistoryItem(Worksheet worksheet, Column column, int position)
			: base(worksheet) {
			this.column = column;
			this.position = position;
		}
		protected override void RedoCore() {
			Worksheet.Columns.InnerList.Insert(position, column);
		}
		protected override void UndoCore() {
			Worksheet.Columns.InnerList.RemoveAt(position);
			Worksheet.Columns.ResetTryGetColumnIndexCache();
		}
	}
	#endregion
	#region SpreadsheetColumnRangeRemovedHistoryItem
	public class SpreadsheetColumnRangeRemovedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int position;
		readonly int innerCount;
		readonly int startIndex;
		readonly int offsetCount;
		readonly bool removeRange;
		System.Collections.Generic.List<Column> columns;
		#endregion
		public SpreadsheetColumnRangeRemovedHistoryItem(Worksheet worksheet, int position, int innerCount, int startIndex, int offsetCount, bool removeRange)
			: base(worksheet) {
			this.position = position;
			this.innerCount = innerCount;
			this.startIndex = startIndex;
			this.offsetCount = offsetCount;
			this.removeRange = removeRange;
		}
		protected override void RedoCore() {
			if (removeRange)
				columns = Worksheet.Columns.InnerList.GetRange(position, innerCount);
			else
				columns = null;
			Worksheet.Columns.RemoveRangeInner(position, innerCount);
			Worksheet.Columns.OffsetColumnIndicesCore(position, startIndex, -offsetCount);
		}
		protected override void UndoCore() {
			Worksheet.Columns.OffsetColumnIndicesCore(position, startIndex, offsetCount);
			Worksheet.Columns.InsertRangeFromHistory(position, columns);
		}
	}
	#endregion
	#region SpreadsheetColumnsOffsetHistoryItem
	public class SpreadsheetColumnsOffsetHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int positionFrom;
		readonly int fromColumnIndex;
		readonly int offset;
		readonly List<Column> savedColumns = new List<Column>();
		int savedEndIndex = 0;
		#endregion
		public SpreadsheetColumnsOffsetHistoryItem(Worksheet worksheet, int positionFrom, int fromColumnIndex, int offset)
			: base(worksheet) {
			this.positionFrom = positionFrom;
			this.fromColumnIndex = fromColumnIndex;
			this.offset = offset;
		}
		protected override void RedoCore() {
			savedColumns.Clear();
			savedEndIndex = Worksheet.Columns.RedoOffsetColumnIndices(positionFrom, fromColumnIndex, offset, savedColumns);
		}
		protected override void UndoCore() {
			Worksheet.Columns.UndoOffsetColumnIndices(positionFrom, fromColumnIndex, -offset, savedColumns, savedEndIndex);
			savedColumns.Clear();
		}
	}
	#endregion
	#region SpreadsheetColumnStartIndexChangedHistoryItem
	public class SpreadsheetColumnStartIndexChangedHistoryItem : SpreadsheetIntHistoryItem {
		#region Fields
		readonly Column column;
		#endregion
		public SpreadsheetColumnStartIndexChangedHistoryItem(Worksheet worksheet, Column column, int oldValue, int newValue)
			: base(worksheet, oldValue, newValue) {
			this.column = column;
		}
		protected override void RedoCore() {
			column.AssignStartIndexCore(NewValue);
		}
		protected override void UndoCore() {
			column.AssignStartIndexCore(OldValue);
		}
	}
	#endregion
	#region SpreadsheetColumnRangeClearedHistoryItem
	public class SpreadsheetColumnRangeClearedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int position;
		readonly int innerCount;
		System.Collections.Generic.List<Column> columns;
		#endregion
		public SpreadsheetColumnRangeClearedHistoryItem(Worksheet worksheet, int position, int innerCount)
			: base(worksheet) {
			this.position = position;
			this.innerCount = innerCount;
		}
		protected override void RedoCore() {
			columns = Worksheet.Columns.InnerList.GetRange(position, innerCount);
			Worksheet.Columns.RemoveRangeInner(position, innerCount);
		}
		protected override void UndoCore() {
			Worksheet.Columns.InsertRangeFromHistory(position, columns);
		}
	}
	#endregion
	#region SpreadsheetRowInsertedHistoryItem
	public class SpreadsheetRowInsertedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly Row row;
		readonly int position;
		#endregion
		public SpreadsheetRowInsertedHistoryItem(Worksheet worksheet, Row row, int position)
			: base(worksheet) {
			this.row = row;
			this.position = position;
		}
		protected override void RedoCore() {
			Worksheet.Rows.InsertCore(position, row);
		}
		protected override void UndoCore() {
			Worksheet.Rows.RemoveCore(position);
		}
	}
	#endregion
	#region SpreadsheetRowsMovedHistoryItem
	public class SpreadsheetRowsMovedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int position;
		readonly int count;
		#endregion
		public SpreadsheetRowsMovedHistoryItem(Worksheet worksheet, int position, int count)
			: base(worksheet) {
			this.position = position;
			this.count = count;
		}
		protected override void RedoCore() {
			Worksheet.Rows.InsertRowsShiftDownCore(position, count);
		}
		protected override void UndoCore() {
			Worksheet.Rows.RemoveRangeCore(position, count);
		}
	}
	#endregion
	#region SpreadsheetRowRangeRemovedHistoryItem
	public class SpreadsheetRowRangeRemovedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int position;
		readonly int innerCount;
		System.Collections.Generic.List<Row> rows;
		readonly int offsetCount;
		readonly bool removeRange;
		#endregion
		public SpreadsheetRowRangeRemovedHistoryItem(Worksheet worksheet, int position, int innerCount, int offsetCount, bool removeRange)
			: base(worksheet) {
			this.position = position;
			this.innerCount = innerCount;
			this.offsetCount = offsetCount;
			this.removeRange = removeRange;
		}
		protected override void RedoCore() {
			if (removeRange) {
				rows = Worksheet.Rows.GetInnerRange(position, innerCount);
				Worksheet.Workbook.CalculationChain.OnBeforeRowRangeRemoving(rows);
			}
			else
				rows = null;
			Worksheet.Rows.RemoveRangeInner(position, innerCount, offsetCount, removeRange);
		}
		protected override void UndoCore() {
			Worksheet.Rows.InsertRangeFromHistory(position, rows, offsetCount);
			if (removeRange)
				Worksheet.Workbook.CalculationChain.OnAfterRowRangeInserting(rows);
		}
	}
	#endregion
	#region SpreadsheetDeleteRowHistoryItem
	public class SpreadsheetDeleteRowHistoryItem : SpreadsheetHistoryItem {
		readonly Row row;
		readonly int position;
		public SpreadsheetDeleteRowHistoryItem(Row row, int position)
			: base(row.Sheet) {
			this.row = row;
			this.position = position;
		}
		protected override void RedoCore() {
			Worksheet.Rows.RemoveCore(position);
		}
		protected override void UndoCore() {
			Worksheet.Rows.InsertCore(position, row);
		}
	}
	#endregion
	#region SpreadsheetCellInsertedHistoryItem
	public class SpreadsheetCellInsertedHistoryItem : SpreadsheetWorkbookHistoryItem {
		#region Fields
		readonly ICell cell;
		readonly ICellContainer container;
		readonly int position;
		string inlineTextValue = null;
		#endregion
		public SpreadsheetCellInsertedHistoryItem(DocumentModel documentModel, ICellContainer container, ICell cell, int position)
			: base(documentModel) {
			this.cell = cell;
			this.container = container;
			this.position = position;
		}
		protected override void RedoCore() {
			container.Cells.InsertCore(position, cell);
			if (inlineTextValue != null)
				cell.AssignValueCore(inlineTextValue);
		}
		protected override void UndoCore() {
			if (cell.Value.IsInlineText)
				inlineTextValue = cell.Value.InlineTextValue;
			container.Cells.RemoveAtCore(position);
		}
	}
	#endregion
	#region SpreadsheetCellRemovedHistoryItem
	public class SpreadsheetCellRemovedHistoryItem : SpreadsheetWorkbookHistoryItem {
		#region Fields
		readonly ICellContainer container;
		readonly int position;
		ICell cell;
		string inlineTextValue = null;
		#endregion
		public SpreadsheetCellRemovedHistoryItem(DocumentModel documentModel, ICellContainer container, int position)
			: base(documentModel) {
			this.container = container;
			this.position = position;
		}
		protected override void RedoCore() {
			cell = container.Cells.InnerList[position];
			if (cell.Value.IsInlineText)
				inlineTextValue = cell.Value.InlineTextValue;
			container.Cells.RemoveAtCore(position);
		}
		protected override void UndoCore() {
			container.Cells.InsertCore(position, cell);
			if (inlineTextValue != null)
				cell.AssignValue(inlineTextValue);
		}
	}
	#endregion
	#region SpreadsheetCellTagsChangedHistoryItem
	public class SpreadsheetCellTagsChangedHistoryItem : SpreadsheetSimpleTypeHistoryItem<object> {
		readonly CellPosition key;
		public SpreadsheetCellTagsChangedHistoryItem(Worksheet sheet, CellPosition key, object oldValue, object newValue)
			: base(sheet, oldValue, newValue) {
			this.key = key;
		}
		protected override void RedoCore() {
			Worksheet.CellTags.SetValue(key, NewValue);
		}
		protected override void UndoCore() {
			Worksheet.CellTags.SetValue(key, OldValue);
		}
	}
	#endregion
	#region SpreadsheetCellTagsClearedHistoryItem
	public class SpreadsheetCellTagsClearedHistoryItem : SpreadsheetHistoryItem {
		readonly List<CellPosition> keys;
		readonly List<object> values;
		public SpreadsheetCellTagsClearedHistoryItem(Worksheet sheet, Dictionary<CellPosition, object> cellTags)
			: base(sheet) {
			this.keys = new List<CellPosition>();
			this.values = new List<object>();
			keys.AddRange(cellTags.Keys);
			values.AddRange(cellTags.Values);
		}
		protected override void RedoCore() {
			Worksheet.CellTags.ClearCore();
		}
		protected override void UndoCore() {
			int count = keys.Count;
			for (int i = 0; i < count; i++)
				Worksheet.CellTags.SetValue(keys[i], values[i]); 
		}
	}
	#endregion
	#region WorksheetHistoryItems
	#region CellOffsetColumnIndexHistoryItem
	public class CellOffsetColumnIndexHistoryItem : SpreadsheetWorkbookHistoryItem {
		readonly int offset;
		readonly ICell cell;
		public CellOffsetColumnIndexHistoryItem(ICell cell, int offset)
			: base(cell.Worksheet.Workbook) {
			this.offset = offset;
			this.cell = cell;
		}
		protected override void UndoCore() {
			cell.OffsetColumnIndexCore(-offset);
			cell.Worksheet.ResetCachedData();
		}
		protected override void RedoCore() {
			cell.OffsetColumnIndexCore(offset);
			cell.Worksheet.ResetCachedData();
		}
	}
	#endregion
	#region CellOffsetRowIndexHistoryItem
	public class CellOffsetRowIndexHistoryItem : SpreadsheetWorkbookHistoryItem {
		readonly int offset;
		readonly ICell cell;
		readonly bool needChangeRow;
		public CellOffsetRowIndexHistoryItem(ICell cell, int offset, bool needChangeRow)
			: base(cell.Worksheet.Workbook) {
			this.offset = offset;
			this.cell = cell;
			this.needChangeRow = needChangeRow;
		}
		protected override void UndoCore() {
			cell.OffsetRowIndexInternal(-offset, needChangeRow);
			cell.Worksheet.ResetCachedData();
		}
		protected override void RedoCore() {
			cell.OffsetRowIndexInternal(offset, needChangeRow);
			cell.Worksheet.ResetCachedData();
		}
	}
	#endregion
	#region SharedFormulaAddHistoryItem
	public class SharedFormulaAddHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly SharedFormula sharedFormula;
		int sharedFormulaIndex;
		#endregion
		public SharedFormulaAddHistoryItem(Worksheet sheet, SharedFormula sharedFormula, int sharedFormulaIndex)
			: base(sheet) {
			this.sharedFormula = sharedFormula;
			this.sharedFormulaIndex = sharedFormulaIndex;
		}
		protected override void UndoCore() {
			Worksheet.SharedFormulas.RemoveCore(sharedFormulaIndex);
		}
		protected override void RedoCore() {
			Worksheet.SharedFormulas.AddCore(sharedFormulaIndex, sharedFormula);
		}
	}
	#endregion
	#region SharedFormulaRemoveHistoryItem
	public class SharedFormulaRemoveHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int sharedFormulaIndex;
		SharedFormula sharedFormula;
		#endregion
		public SharedFormulaRemoveHistoryItem(Worksheet sheet, int sharedFormulaIndex)
			: base(sheet) {
			this.sharedFormulaIndex = sharedFormulaIndex;
		}
		protected override void UndoCore() {
			Worksheet.SharedFormulas.AddCore(sharedFormulaIndex, sharedFormula);
		}
		protected override void RedoCore() {
			sharedFormula = Worksheet.SharedFormulas[sharedFormulaIndex];
			Worksheet.SharedFormulas.RemoveCore(sharedFormulaIndex);
		}
	}
	#endregion
	#endregion
	#region EmptyOperationHistoryItem
	public class EmptyOperationHistoryItem : SpreadsheetWorkbookHistoryItem {
		public EmptyOperationHistoryItem(IDocumentModel documentModel)
			: base(documentModel) {
		}
		protected override void RedoCore() {
			Workbook.IncrementContentVersion();
		}
		protected override void UndoCore() {
			Workbook.IncrementContentVersion();
		}
		protected override void Dispose(bool disposing) {
		}
	}
	#endregion
	#region ActionHistoryItem
	public class ActionHistoryItem<T> : SpreadsheetSimpleTypeHistoryItem<T> {
		readonly Action<T> setValueAction;
		public ActionHistoryItem(IDocumentModel documentModel, T oldValue, T newValue, Action<T> setValueAction)
			: base(documentModel.MainPart, oldValue, newValue) {
			this.setValueAction = setValueAction;
		}
		protected override void UndoCore() {
			setValueAction(OldValue);
		}
		protected override void RedoCore() {
			setValueAction(NewValue);
		}
	}
	#endregion
	#region HistoryHelper
	static class HistoryHelper {
		public static bool SetValue<T>(IDocumentModel documentModel, T oldValue, T newValue, Action<T> setValue) {
			if (!object.Equals(oldValue, newValue)) {
				ApplyHistory(new ActionHistoryItem<T>(documentModel, oldValue, newValue, setValue));
				return true;
			}
			return false;
		}
		public static bool SetValue<T>(IDocumentModel documentModel, T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> setValue) {
			if (!comparer.Equals(oldValue, newValue)) {
				ApplyHistory(new ActionHistoryItem<T>(documentModel, oldValue, newValue, setValue));
				return true;
			}
			return false;
		}
		public static void SetPackedValuesBit(IDocumentModel documentModel, uint packedValues, uint mask, bool value, Action<uint> setPackedValues) {
			uint oldPackedValues = packedValues;
			PackedValues.SetBoolBitValue(ref packedValues, mask, value);
			HistoryHelper.SetValue(documentModel, oldPackedValues, packedValues, setPackedValues);
		}
		public static void ApplyHistory(SpreadsheetHistoryItem item) {
			item.DocumentModel.History.Add(item);
			item.Execute();
		}
	}
	#endregion
	#region SpreadsheetDocumentHistory
	public class SpreadsheetDocumentHistory : DocumentHistory {
		bool hasEmptyOperation = false;
		EmptyOperationHistoryItem emptyOperationHistoryItemInstance;
		public SpreadsheetDocumentHistory(DocumentModel documentModel)
			: base(documentModel) {
			emptyOperationHistoryItemInstance = new EmptyOperationHistoryItem(documentModel);
		}
		public override void AddEmptyOperation() {
			if (TransactionLevel != 0)
				hasEmptyOperation = true;
			else
				Add(emptyOperationHistoryItemInstance);
		}
		public override HistoryItem CommitTransaction() {
			if (hasEmptyOperation) {
				Transaction.AddItem(emptyOperationHistoryItemInstance);
				hasEmptyOperation = false;
			}
			return base.CommitTransaction();
		}
		protected override void UndoCore() {
			base.UndoCore();
			UpdatePivotTables();
		}
		protected override void RedoCore() {
			base.RedoCore();
			UpdatePivotTables();
		}
		void UpdatePivotTables() {
			DocumentModel documentModel = (DocumentModel)DocumentModel;
			for (int i = 0; i < documentModel.Sheets.Count; ++i) {
				Worksheet sheet = documentModel.Sheets[i];
				for (int j = 0; j < sheet.PivotTables.Count; ++j) {
					PivotTable pivotTable = sheet.PivotTables[j];
					pivotTable.CalculationInfo.RefreshPivotTableCore(pivotTable.CalculationInfo.State & ~PivotTableOutOfDateState.WorksheetDataMask, new PivotTableHistoryTransaction(pivotTable));
				}
			}
			documentModel.ApplyChanges(DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility);
		}
	}
	#endregion
	#region SpreadsheetEmptyHistory
	public class SpreadsheetEmptyHistory : EmptyHistory {
		bool modified = false;
		public SpreadsheetEmptyHistory(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override bool Modified {
			get {
				return this.modified;
			}
			set {
				this.modified = value;
			}
		}
		public override void Add(HistoryItem item) {
			base.Add(item);
			this.modified = true;
		}
	}
	#endregion
}
