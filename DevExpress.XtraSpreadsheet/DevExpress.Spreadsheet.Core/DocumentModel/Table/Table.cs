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
using System.Diagnostics;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableType
	public enum TableType { 
		Worksheet = 0,
		SharePoint = 1,
		Xml = 2,
		QueryTable = 3
	}
	#endregion
	#region ITableBase
	public interface ITableBase {
		ActualTableStyleCellFormatting GetActualCellFormatting(CellPosition cellPosition);
		CellRangeBase WholeRange { get; }
		TableStyle Style { get; set; }
	}
	#endregion
	#region ITableStyleOptions
	public interface ITableStyleOptions {
		bool HasHeadersRow { get; }
		bool HasTotalsRow { get; }
		bool ShowFirstColumn { get; }
		bool ShowLastColumn { get; }
		bool ShowColumnStripes { get; }
		bool ShowRowStripes { get; }
	}
	#endregion
	#region ITableStyleOwner
	public interface ITableStyleOwner {
		ITableStyleOptions Options { get; }
		CellRange Range { get; }
		TableStyleFormatCache Cache { get; }
		void CacheColumnStripeInfo(int index, TableStyleStripeInfo info);
		TableStyleStripeInfo GetColumnStripeInfo(int index);
	}
	#endregion
	#region TableRowEnum
	public enum TableRowEnum {
		NotDefined = 0x0000,
		Headers = 0x0001,
		Totals = 0x0002,
		Data = 0x0004,
		All = 0x0007,
		ThisRow = 0x0008,
	}
	#endregion
	#region Table
	public partial class Table : IDisposable, ITableFormatInfo, ITableBase, ITableStyleOwner, ITableStyleOptions, ISpreadsheetNamedObject, ISpreadsheetRangeObject {
		#region Fields
		readonly TableColumnInfoCollection columns;
		readonly TableElementFormatManager elementFormatManager;
		readonly TableAutoFilter autoFilter;
		readonly TableStyleFormatCache styleFormatCache;
		string originalName;
		string displayName;
		CellRange range;
		bool hasTotalsRow;
		bool hasHeadersRow;
		int queryTableContentId;
		#endregion
		public Table(Worksheet sheet, string name, CellRange range)
			:this(sheet, name, range, true){
		}
		public Table(Worksheet sheet, string name, CellRange range, bool hasHeadersRow)
			: this(sheet, name, name, range, hasHeadersRow) {
		}
		public Table(Worksheet sheet, string originalName, string displayName, CellRange range)
			: this(sheet, originalName, displayName, range, true) {
		}
		protected internal Table(Worksheet sheet, string originalName, string displayName, CellRange range, bool hasHeadersRow) {
			Guard.ArgumentIsNotNullOrEmpty(displayName, "displayName");
			Guard.ArgumentNotNull(sheet, "sheet");
			if (!Object.ReferenceEquals(sheet, range.Worksheet))
				Exceptions.ThrowInternalException();
			this.originalName = originalName;
			this.displayName = displayName;
			this.range = range;
			this.hasHeadersRow = hasHeadersRow;
			this.columns = new TableColumnInfoCollection();
			this.autoFilter = new TableAutoFilter(this);
			differentialFormatIndexes = new int[DifferentialFormatElementCount];
			cellFormatIndexes = new int[CellFormatElementCount];
			borderFormatIndexes = new int[BorderFormatElementCount];
			InitialFormatIndexes();
			elementFormatManager = new TableElementFormatManager(this);
			styleFormatCache = new TableStyleFormatCache(this);
		}
		#region Properties
		public string Name {
			get { return displayName; }
			set {
				if (StringExtensions.CompareInvariantCulture(displayName, value) == 0)
					return;
				SetName(value);
			}
		}
		public string OriginalName { get { return originalName; } }
		public Worksheet Worksheet { get { return (Worksheet)range.Worksheet; } }
		public TableColumnInfoCollection Columns { get { return columns; } }
		public bool HasHeadersRow { get { return hasHeadersRow; } }
		public bool HasTotalsRow { get { return hasTotalsRow; } }
		protected internal void SetHasHeadersRow(bool value) {
			if (hasHeadersRow != value)
				ApplyHistoryItem(new ChangeTableHasHeadersPropertyHistoryItem(this, hasHeadersRow, value));
		}
		protected internal void SetHasHeadersRowCore(bool value) {
			this.hasHeadersRow = value;
			styleFormatCache.SetInvalid();
		}
		protected internal void SetHasTotalsRow(bool value) {
			if (hasTotalsRow != value)
				ApplyHistoryItem(new ChangeTableHasTotalsPropertyHistoryItem(this, hasTotalsRow, value));
		}
		protected internal void SetHasTotalsRowCore(bool value) {
			this.hasTotalsRow = value;
			styleFormatCache.SetInvalid();
		}
		public bool ChangeHeaders(bool hasHeadersRow, bool hasAutoFilter, bool showAutoFilterButton, IErrorHandler errorHandler) {
			ChangeTableHeadersCommand command = new ChangeTableHeadersCommand(this, hasHeadersRow, hasAutoFilter, showAutoFilterButton, errorHandler);
			return command.Execute();
		}
		public bool ChangeTotals(bool hasTotalsRow, IErrorHandler errorHandler) {
			ChangeTableTotalsCommand command = new ChangeTableTotalsCommand(this, hasTotalsRow, errorHandler);
			return command.Execute();
		}
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region Range
		public CellRange Range {
			get { return range; }
			set {
				if (CellRange.Equals(Range, value))
					return;
				SetRange(value);
			}
		}
		#endregion
		#region Comment
		public string Comment {
			get { return TableInfo.Comment; }
			set {
				if (Comment == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetCommentCore, value);
			}
		}
		DocumentModelChangeActions SetCommentCore(TableInfo info, string value) {
			info.Comment = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TableType
		public TableType TableType {
			get { return TableInfo.TableType; }
			set {
				if (TableType == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetTableTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTableTypeCore(TableInfo info, TableType value) {
			info.TableType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Published
		public bool Published {
			get { return TableInfo.Published; }
			set {
				if (Published == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetPublishedCore, value);
			}
		}
		DocumentModelChangeActions SetPublishedCore(TableInfo info, bool value) {
			info.Published = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertRowProperty
		public bool InsertRowProperty {
			get { return TableInfo.InsertRowProperty; }
			set {
				if (InsertRowProperty == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetInsertRowPropertyCore, value);
			}
		}
		DocumentModelChangeActions SetInsertRowPropertyCore(TableInfo info, bool value) {
			info.InsertRowProperty = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertRowShift
		public bool InsertRowShift {
			get { return TableInfo.InsertRowShift; }
			set {
				if (InsertRowShift == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetInsertRowShiftCore, value);
			}
		}
		DocumentModelChangeActions SetInsertRowShiftCore(TableInfo info, bool value) {
			info.InsertRowShift = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowFirstColumn
		public bool ShowFirstColumn {
			get { return TableInfo.ShowFirstColumn; }
			set {
				if (ShowFirstColumn == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetShowFirstColumnCore, value);
				styleFormatCache.SetInvalid();
			}
		}
		DocumentModelChangeActions SetShowFirstColumnCore(TableInfo info, bool value) {
			info.ShowFirstColumn = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLastColumn
		public bool ShowLastColumn {
			get { return TableInfo.ShowLastColumn; }
			set {
				if (ShowLastColumn == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetShowLastColumnCore, value);
				styleFormatCache.SetInvalid();
			}
		}
		DocumentModelChangeActions SetShowLastColumnCore(TableInfo info, bool value) {
			info.ShowLastColumn = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowRowStripes
		public bool ShowRowStripes {
			get { return TableInfo.ShowRowStripes; }
			set {
				if (ShowRowStripes == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetShowRowStripesCore, value);
				styleFormatCache.SetInvalid();
			}
		}
		DocumentModelChangeActions SetShowRowStripesCore(TableInfo info, bool value) {
			info.ShowRowStripes = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowColumnStripes
		public bool ShowColumnStripes {
			get { return TableInfo.ShowColumnStripes; }
			set {
				if (ShowColumnStripes == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetShowColumnStripesCore, value);
				styleFormatCache.SetInvalid();
			}
		}
		DocumentModelChangeActions SetShowColumnStripesCore(TableInfo info, bool value) {
			info.ShowColumnStripes = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ConnectionId
		public int ConnectionId {
			get { return TableInfo.ConnectionId; }
			set {
				if (ConnectionId == value)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetConnectionIdCore, value);
			}
		}
		DocumentModelChangeActions SetConnectionIdCore(TableInfo info, int value) {
			info.ConnectionId = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public TableAutoFilter AutoFilter { get { return autoFilter; } }
		public bool HasAutoFilter { get { return AutoFilter.Range != null; } }
		public bool ShowAutoFilterButton {
			get {
				foreach (AutoFilterColumn item in AutoFilter.FilterColumns)
					if (!item.HiddenAutoFilterButton)
						return true;
				return false;
			}
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)Worksheet.Workbook; } }
		public int RowCount { get { return range.Height; } }
		public int QueryTableContentId { get { return queryTableContentId; } set { queryTableContentId = value; } }
		#endregion
		#region Name
		public void SetName(string value) {
			DocumentModel.CheckTableName(value);
			TableRenameCommand command = new TableRenameCommand(this, value);
			command.Execute();
		}
		protected internal void SetNameCore(string value) {
			string oldName = this.displayName;
			this.displayName = value;
			this.originalName = value;
			RaiseNameChanged(value, oldName);
		}
		protected internal void SetDisplayNameCore(string value) {
			this.displayName = value;
			this.originalName = value;
		}
		protected internal void SetOriginalNameCore(string value) {
			this.originalName = value;
		}
		#region NameChanged
		NameChangedEventHandler onNameChanged;
		public event NameChangedEventHandler NameChanged { add { onNameChanged += value; } remove { onNameChanged -= value; } }
		protected internal virtual void RaiseNameChanged(string name, string oldName) {
			if (onNameChanged != null) {
				NameChangedEventArgs args = new NameChangedEventArgs(name, oldName);
				onNameChanged(this, args);
			}
		}
		#endregion
		#region RangeChanged
		CellRangeChangedEventHandler onRangeChanged;
		public event CellRangeChangedEventHandler RangeChanged { add { onRangeChanged += value; } remove { onRangeChanged -= value; } }
		protected internal virtual void RaiseRangeChanged(CellRange oldRange, CellRange newRange) {
			if (onRangeChanged != null) {
				CellRangeChangedEventArgs args = new CellRangeChangedEventArgs(oldRange, newRange);
				onRangeChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region Range
		protected internal virtual void SetRange(CellRange newRange) {
			DocumentModel.BeginUpdate();
			try {
				ApplyHistoryItem(new ChangeTableRangeHistoryItem(this, range, newRange));
				AutoFilter.OnTableRangeSetting(newRange, HasTotalsRow);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void SetRangeCore(CellRange range) {
			CellRange oldRange = this.range;
			this.range = range;
			RaiseRangeChanged(oldRange, range);
			styleFormatCache.SetInvalid();
			DocumentModel.IncrementContentVersion();
		}
		#endregion
		public VariantValue GetRangeByCondition(TableRowEnum rowMask, string columnStart, string columnEnd, int currentRowIndex) {
			int totalRows = range.Height;
			int startRow = range.TopLeft.Row;
			int startColumn = range.TopLeft.Column;
			int startInnerRowIndex = int.MaxValue;
			int endInnerRowIndex = -1;
			int startDataRow = HasHeadersRow ? 1 : 0;
			int endDataRow = HasTotalsRow ? totalRows - 2 : totalRows - 1;
			PositionType rowPositionType = PositionType.Absolute;
			if ((rowMask & TableRowEnum.ThisRow) > 0) {
				if (currentRowIndex < startRow + startDataRow || currentRowIndex > startRow + endDataRow)
					return VariantValue.ErrorInvalidValueInFunction;
				startInnerRowIndex = currentRowIndex - startRow;
				endInnerRowIndex = startInnerRowIndex;
				rowPositionType = PositionType.Relative;
			}
			else {
				if ((rowMask & TableRowEnum.Headers) > 0 && HasHeadersRow) {
					startInnerRowIndex = 0;
					endInnerRowIndex = 0;
				}
				if ((rowMask & TableRowEnum.Data) > 0 || rowMask == TableRowEnum.NotDefined) {
					startInnerRowIndex = Math.Min(startInnerRowIndex, startDataRow);
					endInnerRowIndex = Math.Max(startInnerRowIndex, endDataRow);
				}
				if ((rowMask & TableRowEnum.Totals) > 0 && HasTotalsRow) {
					startInnerRowIndex = Math.Min(startInnerRowIndex, totalRows - 1);
					endInnerRowIndex = totalRows - 1;
				}
			}
			if (endInnerRowIndex < 0)
				return VariantValue.ErrorReference;
			int startInnerColumnIndex = 0;
			int endInnerColumnIndex = range.Width - 1;
			if (!string.IsNullOrEmpty(columnStart)) {
				TableColumn columnInfo = Columns[columnStart];
				if (columnInfo != null) {
					startInnerColumnIndex = columnInfo.FindColumnIndex();
					endInnerColumnIndex = startInnerColumnIndex;
				}
				if (!string.IsNullOrEmpty(columnEnd)) {
					columnInfo = Columns[columnEnd];
					if (columnInfo != null)
						endInnerColumnIndex = columnInfo.FindColumnIndex();
				}
			}
			startInnerRowIndex += startRow;
			endInnerRowIndex += startRow;
			startInnerColumnIndex += startColumn;
			endInnerColumnIndex += startColumn;
			CellRange resultRange = new CellRange(range.Worksheet, new CellPosition(startInnerColumnIndex, startInnerRowIndex, PositionType.Absolute, rowPositionType), new CellPosition(endInnerColumnIndex, endInnerRowIndex, PositionType.Absolute, rowPositionType));
			VariantValue result = new VariantValue();
			result.CellRangeValue = resultRange;
			return result;
		}
		#region Notifications
		public bool CanRangeRemove(CellRangeBase cellRange, RemoveCellMode mode) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				foreach (CellRangeBase innerRange in (cellRange as CellUnion).InnerCellRanges)
					if (!CanSingleRangeRemoveCore(innerRange as CellRange, mode))
						return false;
				return true;
			}
			return CanSingleRangeRemoveCore(cellRange as CellRange, mode);
		}
		bool CanSingleRangeRemoveCore(CellRange cellRange, RemoveCellMode mode) {
			if (cellRange.Includes(range)) 
				return true;
			if (cellRange.RangeType == CellRangeType.IntervalRange) {
				CellIntervalRange intervalRange = range as CellIntervalRange;
				if ((intervalRange != null && intervalRange.IsColumnInterval) || !HasHeadersRow)
					return true;
				VariantValue intersection = cellRange.IntersectionWith(Range);
				if (intersection != VariantValue.ErrorNullIntersection)
					return !intersection.CellRangeValue.GetFirstInnerCellRange().TopLeft.EqualsPosition(range.TopLeft);
			}
			if (cellRange.Intersects(range) && !(range.Includes(cellRange) || cellRange.Includes(range)))
				return false;
			CellRange headerRange = range.GetSubRowRange(0, 0);
			if (HasHeadersRow && cellRange.Includes(headerRange))
				return false;
			bool hasColumnMode = mode == RemoveCellMode.ShiftCellsLeft;
			CellRange prohibitedRange = GetProhibitedRange(hasColumnMode);
			if (!CheckProhibitedRangeIntersectsRange(cellRange, prohibitedRange))
				return true;
			return CheckIntersectionWithProhibitedRange(cellRange, prohibitedRange, hasColumnMode);
		}
		public bool CanRangeInsert(CellRangeBase cellRange, InsertCellMode mode) {
			if (cellRange.RangeType != CellRangeType.UnionRange)
				return CanSingleRangeInsert(cellRange as CellRange, mode);
			foreach (CellRange currentRange in (cellRange as CellUnion).InnerCellRanges)
				if (!CanSingleRangeInsert(currentRange, mode))
					return false;
			return true;
		}
		CellRange GetProhibitedRange(bool hasColumnMode) {
			CellPosition prohibitedTopLeft = hasColumnMode ? new CellPosition(0, Range.TopLeft.Row) : new CellPosition(Range.TopLeft.Column, 0);
			return new CellRange(Range.Worksheet, prohibitedTopLeft, Range.BottomRight);
		}
		CellRangeBase GetIntersectionWithProhibitedRange(CellRange range, CellRange prohibited) {
			return range.IntersectionWith(prohibited).CellRangeValue;
		}
		bool CheckProhibitedRangeIntersectsRange(CellRange range, CellRange prohibited) {
			return prohibited.Intersects(range);
		}
		bool CheckIntersectionWithProhibitedRange(CellRange cellRange, CellRange prohibitedRange, bool hasColumnMode) {
			CellRangeBase intersection = GetIntersectionWithProhibitedRange(cellRange, prohibitedRange);
			return hasColumnMode ? intersection.Height == prohibitedRange.Height : intersection.Width == prohibitedRange.Width;
		}
		bool CanSingleRangeInsert(CellRange cellRange, InsertCellMode mode) {
			if (cellRange.Includes(range))
				return true;
			bool hasColumnMode = mode == InsertCellMode.ShiftCellsRight;
			CellRange prohibitedRange = GetProhibitedRange(hasColumnMode);
			if (!CheckProhibitedRangeIntersectsRange(cellRange, prohibitedRange))
				return true;
			if (!CheckIntersectionWithProhibitedRange(cellRange, prohibitedRange, hasColumnMode))
				return false;
			if (hasColumnMode)
				return range.BottomRight.Column + cellRange.Width < Worksheet.MaxColumnCount;
			else
				return range.BottomRight.Row + cellRange.Height < Worksheet.MaxRowCount;
		}
		#region OnRangeRemoving
		protected internal void OnBeforeRangeRemoving(RemoveRangeNotificationContext context) {
			CellRange removableTableRange = context.Range.Intersection(range);
			if (removableTableRange != null && !removableTableRange.Includes(range) && removableTableRange.Includes(GetDataRange())) {
				CellRange clearableTableRange = removableTableRange.GetSubRowRange(0, 0);
				Worksheet.ClearCellsNoShift(clearableTableRange);
				context.Range.Resize(0, 1, 0, 0);
			}
		}
		public bool OnRangeRemoving(RemoveRangeNotificationContext context) {
			if (context.Range.Includes(range))
				return false;
			RemoveCellMode mode = context.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				OnRangeRemovingShiftCellsLeft(context);
			if (mode == RemoveCellMode.ShiftCellsUp)
				OnRangeRemovingShiftCellsUp(context);
			if (mode == RemoveCellMode.Default) 
				OnRangeRemovingDefault(context);
			AutoFilter.OnRangeRemoving(context);
			return true;
		}
		void OnRangeRemovingShiftCellsLeft(RemoveRangeNotificationContext context) {
			CellRange removableRange = context.Range;
			if (context.SuppressTableChecks || 
				removableRange.TopLeft.Row > range.BottomRight.Row ||
				removableRange.BottomRight.Row < range.TopLeft.Row ||
				removableRange.TopLeft.Column > range.BottomRight.Column)
				return;
			int deletedWidth;
			if (removableRange.TopLeft.Column < range.TopLeft.Column)
				if (removableRange.BottomRight.Column < range.TopLeft.Column) {
					deletedWidth = -removableRange.Width;
					Range = Range.GetResized(deletedWidth, 0, deletedWidth, 0);
				}
				else {
					deletedWidth = Range.TopLeft.Column - removableRange.TopLeft.Column;
					for (int i = 0; i < deletedWidth; ++i)
						RemoveColumnCore(0);
					Range = Range.GetResized(-deletedWidth, 0, -removableRange.Width, 0);
				}
			else {
				deletedWidth = removableRange.BottomRight.Column > Range.BottomRight.Column ? Range.BottomRight.Column : removableRange.BottomRight.Column;
				deletedWidth -= removableRange.TopLeft.Column - 1;
				int firstColumnDeleted = removableRange.TopLeft.Column - range.TopLeft.Column;
				for (int i = 0; i < deletedWidth; ++i)
					RemoveColumnCore(firstColumnDeleted);
				Range = Range.GetResized(0, 0, -deletedWidth, 0);
			}
		}
		void OnRangeRemovingShiftCellsUp(RemoveRangeNotificationContext context) {
			CellRange removableRange = context.Range;
			if (removableRange.LeftColumnIndex > range.RightColumnIndex ||
				removableRange.RightColumnIndex < range.LeftColumnIndex ||
				removableRange.TopRowIndex > range.BottomRowIndex)
				return;
			CellRange newTableRange = GetRangeRemovingShiftCellsUp(removableRange);
			if (HasTotalsRow && removableRange.BottomRowIndex >= range.BottomRowIndex) {
				SetHasTotalsRow(false);
				if (context.ClearTableColumnsTotalsRow)
					ClearColumnsTotalsRow();
			} else if (!HasTotalsRow && removableRange.BottomRowIndex > range.BottomRowIndex && context.ClearTableColumnsTotalsRow)
				ClearColumnsTotalsRow();
			Range = newTableRange;
		}
		void OnRangeRemovingDefault(RemoveRangeNotificationContext context) {
			CellRange removableRange = context.Range;
			TableColumnNamesClearOperation operation = new TableColumnNamesClearOperation();
			operation.ShouldCorrectColumnFormulas = false;
			operation.ShouldUpdateColumnCells = false;
			if (operation.Init(this, removableRange, new Dictionary<string, string>()))
				operation.Execute();
			context.ShouldClearCells = false;
			CellRange totalsRowRange = TryGetTotalsRowRange();
			if (totalsRowRange != null && removableRange.EqualsPosition(totalsRowRange)) {
				SetHasTotalsRow(false);
				Range = range.GetResized(0, 0, 0, -1);
			}
		}
		CellRange GetRangeRemovingShiftCellsUp(CellRange removableRange) {
			int deletedHeight;
			if (removableRange.TopLeft.Row < range.TopLeft.Row) {
				if (removableRange.BottomRight.Row < range.TopLeft.Row)
					deletedHeight = -removableRange.Height;
				else
					deletedHeight = removableRange.TopLeft.Row - range.TopLeft.Row;
				return range.GetResized(0, deletedHeight, 0, deletedHeight);
			}
			deletedHeight = removableRange.TopLeft.Row - 1;
			deletedHeight -= removableRange.BottomRight.Row > range.BottomRight.Row ? range.BottomRight.Row : removableRange.BottomRight.Row;
			return range.GetResized(0, 0, 0, deletedHeight);
		}
		void ClearColumnsTotalsRow() {
			int i = 0;
			int totalRowIndex = range.BottomRowIndex;
			CellRange totalsRange = range.GetSubRowRange(totalRowIndex, totalRowIndex);
			foreach (ICell cell in totalsRange.GetAllCellsEnumerable()) {
				Columns[i].ClearTotalsRow();
				i++;
			}
		}
		#endregion
		#region OnRangeInserting
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			AutoFilter.OnRangeInserting(notificationContext);
			if (notificationContext.Mode == InsertCellMode.ShiftCellsRight)
				OnRangeInsertingShiftRight(notificationContext.Range, notificationContext.SuppressTableChecks);
			else
				OnRangeInsertingShiftDown(notificationContext.Range, notificationContext.SuppressTableChecks);
		}
		void OnRangeInsertingShiftRight(CellRange cellRange, bool suppressTableChecks) {
			if (suppressTableChecks)
				return;
			if (cellRange.TopLeft.Row > range.BottomRight.Row || cellRange.BottomRight.Row < range.TopLeft.Row || cellRange.TopLeft.Column > range.BottomRight.Column)
				return;
			int insertedWidth = cellRange.Width;
			if (cellRange.TopLeft.Column <= range.TopLeft.Column)
				Range = range.GetResized(insertedWidth, 0, insertedWidth, 0);
			else {
				int startColumn = cellRange.TopLeft.Column - range.TopLeft.Column;
				TableColumnNamesCreateOperation operation = new TableColumnNamesCreateOperation();
				for (int i = 0; i < insertedWidth; i++) {
					string name = operation.GetNextName(columns);
					TableColumn column = new TableColumn(this, name);
					InsertColumnCore(column, i + startColumn);
				}
				Range = range.GetResized(0, 0, insertedWidth, 0);
			}
		}
		void OnRangeInsertingShiftDown(CellRange cellRange, bool suppressTableChecks) {
			int positionIncrement = suppressTableChecks ? 1 : 0;
			if (cellRange.TopLeft.Column > range.BottomRight.Column || cellRange.BottomRight.Column < range.TopLeft.Column || cellRange.TopLeft.Row > range.BottomRight.Row + positionIncrement)
				return;
			int insertedHeight = cellRange.Height;
			if (cellRange.TopLeft.Row <= range.TopLeft.Row - positionIncrement)
				Range = range.GetResized(0, insertedHeight, 0, insertedHeight);
			else
				Range = range.GetResized(0, 0, 0, insertedHeight);
		}
		#endregion
		public bool OnCellValueChanged(ICell cell) {
			if (!range.ContainsCell(cell))
				return false;
			int hostColumnIndex = cell.ColumnIndex - range.TopLeft.Column;
			TableColumn column = columns[hostColumnIndex];
			column.OnCellValueChanged(cell);
			return true;
		}
		public bool OnCellRemoved(ICell cell) {
			if (!range.ContainsCell(cell))
				return false;
			int hostColumnIndex = cell.ColumnIndex - range.TopLeft.Column;
			TableColumn column = columns[hostColumnIndex];
			column.OnCellRemoved(cell);
			return true;
		}
		#endregion
		#region Row methods
		public bool AddRows(int index, int count, IErrorHandler handler) {
			AddTableRowsCommand command = new AddTableRowsCommand(this, index, count, handler);
			return command.Execute();
		}
		public void DeleteRow(int index, IErrorHandler handler) {
			DeleteRows(index, 1, handler);
		}
		public void DeleteRows(int index, int count, IErrorHandler handler) {
			if (index < 0 || index >= RowCount)
				Exceptions.ThrowArgumentException("Row index is outside of bounds", index);
			if (HasHeadersRow) {
				if (index == 0 && count == 1)
					Exceptions.ThrowInvalidOperationException("Cannot delete header row."); 
				if (index == 1 && count >= RowCount - 1)
					Exceptions.ThrowInvalidOperationException("Table can't contain only headers row."); 
			}
			DocumentModel.BeginUpdate();
			try {
				int endIndex = Math.Min(index + count, RowCount) - 1;
				Worksheet.RemoveRange(range.GetSubRowRange(index, endIndex), RemoveCellMode.ShiftCellsUp, handler);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region Column methods
		public TableColumn AddColumn(IErrorHandler handler) {
			return InsertColumn(columns.Count, handler);
		}
		public TableColumn InsertColumn(int position, IErrorHandler handler) {
			TableColumnsInsertCommand command = new TableColumnsInsertCommand(this, position, 1, handler);
			command.Execute();
			return Columns[position];
		}
		public void InsertColumnCore(TableColumn column, int index) {
			DocumentHistory history = DocumentModel.History;
			TableInsertColumnHistoryItem historyItem = new TableInsertColumnHistoryItem(this, column, index);
			history.Add(historyItem);
			historyItem.Execute();
			if (AutoFilter.Enabled && AutoFilter.FilterColumns.Count < Columns.Count)
				AutoFilter.FilterColumns.Insert(index, new AutoFilterColumn(Worksheet) { ColumnId = index });
		}
		public void RemoveColumn(int position, IErrorHandler handler) {
			if (position < 0 || position >= columns.Count)
				Exceptions.ThrowArgumentOutOfRangeException("index", XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorTableColumnIndexOutside));
			if (columns.Count == 1)
				Worksheet.RemoveTable(this);
			else {
				TableColumnRemoveCommand command = new TableColumnRemoveCommand(this, position, handler);
				command.Execute();
			}
		}
		public void RemoveColumnCore(int index) {
			DocumentHistory history = DocumentModel.History;
			TableColumnRemoveAtHistoryItem historyItem = new TableColumnRemoveAtHistoryItem(this, index);
			history.Add(historyItem);
			historyItem.Execute();
			if (AutoFilter.Enabled && AutoFilter.FilterColumns.Count > Columns.Count)
				AutoFilter.FilterColumns.RemoveAt(index);
		}
		#endregion
		public bool IsDefault() {
			return TableInfoIndex == 0;
		}
		#region IDisposable Members
		public void Dispose() {
			columns.Dispose();
		}
		#endregion
		#region CheckIntegrity
		public void CheckIntegrity(CheckIntegrityFlags Flags) {
			if (columns.Count <= 0)
				IntegrityChecks.Fail("Table has no columns.(Columns count: " + columns.Count + ", range: " + range.ToString() + ")");
			if (RowCount <= 0)
				IntegrityChecks.Fail("Table has no rows.(range: " + range.ToString() + ")");
			if (columns.Count <= 0)
				IntegrityChecks.Fail("Table has no columns.(Columns count: " + columns.Count + ", range: " + range.ToString() + ")");
			if (columns.Count != range.Width)
				IntegrityChecks.Fail("Range width does not proper to columns count.(Columns count: " + columns.Count + ", range: " + range.ToString() + ")");
		}
		#endregion
		#region Format
		internal DifferentialFormat GetDifferentialFormat(int elementIndex) {
			if (BatchUpdateHelper != null)
				return (DifferentialFormat)BatchUpdateHelper.GetDifferentialFormatInfo(elementIndex);
			return GetDifferentialFormatInfoCore(differentialFormatIndexes[elementIndex]);
		}
		internal CellFormat GetCellFormat(int elementIndex) {
			if (BatchUpdateHelper != null)
				return (CellFormat)BatchUpdateHelper.GetCellFormatInfo(elementIndex);
			return (CellFormat)DocumentModel.Cache.CellFormatCache[cellFormatIndexes[elementIndex]];
		}
		internal DifferentialFormat GetBorderFormat(int elementIndex) {
			if (BatchUpdateHelper != null)
				return (DifferentialFormat)BatchUpdateHelper.GetBorderFormatInfo(elementIndex);
			return GetDifferentialFormatInfoCore(borderFormatIndexes[elementIndex]);
		}
		DifferentialFormat GetDifferentialFormatInfoCore(int formatIndex) {
			return (DifferentialFormat)DocumentModel.Cache.CellFormatCache[formatIndex];
		}
		#endregion
		#region Index management
		protected override IDocumentModel GetDocumentModel() {
			return DocumentModel;
		}
		public override Table GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new TableBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new TableBatchInitHelper(this);
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		void InitialFormatIndexes() {
			for (int i = 0; i < DifferentialFormatElementCount; i++)
				differentialFormatIndexes[i] = CellFormatCache.DefaultDifferentialFormatIndex;
			for (int i = 0; i < CellFormatElementCount; i++)
				cellFormatIndexes[i] = DocumentModel.StyleSheet.DefaultCellFormatIndex;
			for (int i = 0; i < BorderFormatElementCount; i++)
				borderFormatIndexes[i] = CellFormatCache.DefaultDifferentialFormatIndex;
		}
		internal void AssignTableInfoIndex(int value) {
			this.tableInfoIndex = value;
			this.styleFormatCache.SetInvalid();
		}
		internal void AssignDifferentialFormatIndex(int elementIndex, int value) {
			this.differentialFormatIndexes[elementIndex] = value;
		}
		internal void AssignCellFormatIndex(int elementIndex, int value) {
			this.cellFormatIndexes[elementIndex] = value;
		}
		internal void AssignBorderFormatIndex(int elementIndex, int value) {
			this.borderFormatIndexes[elementIndex] = value;
		}
		internal void AssignApplyFlagsIndex(int value) {
			this.applyFlagsIndex = value;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			Table other = obj as Table;
			if (other == null || !base.Equals(other))
				return false;
			return
				this.columns == other.columns &&
				this.originalName == other.originalName &&
				this.displayName == other.displayName &&
				this.range == other.range &&
				this.hasTotalsRow == other.hasTotalsRow &&
				this.hasHeadersRow == other.hasHeadersRow &&
				this.autoFilter == other.autoFilter &&
				this.queryTableContentId == other.queryTableContentId &&
				this.tableInfoIndex == other.tableInfoIndex &&
				this.applyFlagsIndex == other.applyFlagsIndex;
		}
		public override int GetHashCode() {
			CombinedHashCode result = new CombinedHashCode();
			result.AddInt(base.GetHashCode());
			result.AddObject(columns); 
			result.AddObject(originalName);
			result.AddObject(displayName);
			result.AddObject(range);
			result.AddInt(hasTotalsRow.GetHashCode());
			result.AddInt(hasHeadersRow.GetHashCode());
			result.AddObject(autoFilter);
			result.AddInt(queryTableContentId);
			result.AddInt(tableInfoIndex);
			result.AddInt(applyFlagsIndex);
			return result.CombinedHash32;
		}
		#endregion
		#region ITableFormatInfo Members
		public ITableElementFormatInfo HeaderRow { get { return elementFormatManager.GetFormatInfo(HeaderRowIndex); } }
		public ITableElementFormatInfo TotalsRow { get { return elementFormatManager.GetFormatInfo(TotalsRowIndex); } }
		public ITableElementFormatInfo Data { get { return elementFormatManager.GetFormatInfo(DataIndex); } }
		#endregion
		#region Style
		public TableStyle Style {
			get { return ApplyTableStyle ? TableStyles[TableInfo.StyleName] : null; }
			set {
				if (value == null) {
					if (ApplyTableStyle)
						SetPropertyValue(TableInfoIndexAccessor, SetResetStyleNameCore, TableStyleName.DefaultStyleName.Name);
					return;
				}
				TableStyleName styleName = value.Name;
				if (styleName.IsEquals(TableInfo.StyleName) && ApplyTableStyle)
					return;
				SetPropertyValue(TableInfoIndexAccessor, SetStyleNameCore, styleName.Name);
			}
		}
		public bool ApplyTableStyle { get { return TableInfo.ApplyTableStyle; } }
		public TableStyleFormatCache StyleFormatCache {
			get {
				if (!styleFormatCache.IsValid && ApplyTableStyle) {
					TableStyle style = TableStyles[TableInfo.StyleName];
					if (!style.Name.IsDefault) {
						TableStyleElementInfoCache styleCache = style.Cache;
						if (!styleCache.IsEmpty)
							styleFormatCache.Prepare(styleCache);
					}
				}
				return styleFormatCache;
			}
		}
		TableStyleCollection TableStyles { get { return DocumentModel.StyleSheet.TableStyles; } }
		DocumentModelChangeActions SetStyleNameCore(TableInfo info, string value) {
			info.StyleName = value;
			info.ApplyTableStyle = true;
			return DocumentModelChangeActions.None;
		}
		protected internal void ChangeSubscribeStyleCache(int oldInfoIndex, int newInfoIndex) {
			TableInfo oldInfo = DocumentModel.Cache.TableInfoCache[oldInfoIndex];
			TableInfo newInfo = DocumentModel.Cache.TableInfoCache[newInfoIndex];
			UnSubscribeCacheEvent(oldInfo.StyleName);
			SubscribeCacheEvent(newInfo.StyleName);
		}
		void SubscribeCacheEvent(string styleName) {
			if (TableStyles.ContainsStyleName(styleName))
				TableStyles[styleName].Cache.OnInvalid += OnInvalidFormatCache;
		}
		void UnSubscribeCacheEvent(string styleName) {
			if (TableStyles.ContainsStyleName(styleName))
				TableStyles[styleName].Cache.OnInvalid -= OnInvalidFormatCache;
		}
		void OnInvalidFormatCache(object sender, EventArgs e) {
			styleFormatCache.SetInvalid();
		}
		DocumentModelChangeActions SetResetStyleNameCore(TableInfo info, string value) {
			info.StyleName = value;
			info.ApplyTableStyle = false;
			return DocumentModelChangeActions.None;
		}
		#endregion
		protected internal void AssignTableInfo(TableInfo info) {
			AssignTableInfoIndex(DocumentModel.Cache.TableInfoCache.AddItem(info));
		}
		#region ITableBase  Members
		CellRangeBase ITableBase.WholeRange { get { return range; } }
		ActualTableStyleCellFormatting ITableBase.GetActualCellFormatting(CellPosition cellPosition) {
			TableStyle style = TableStyles[TableInfo.StyleName];
			return styleFormatCache.GetActualCellFormatting(cellPosition, style.Cache);
		}
		#endregion
		#region GetPartRanges
		public CellRange GetDataRange() {
			int totalIndex = Range.Height - 1;
			return Range.GetSubRowRange(HasHeadersRow ? 1 : 0, HasTotalsRow ? totalIndex - 1 : totalIndex);
		}
		protected internal CellRange TryGetHeadersRowRange() {
			if (!HasHeadersRow)
				return null;
			return Range.GetSubRowRange(0, 0);
		}
		protected internal CellRange TryGetTotalsRowRange() {
			if (!HasTotalsRow)
				return null;
			int totalIndex = Range.Height - 1;
			return Range.GetSubRowRange(totalIndex, totalIndex);
		}
		#endregion
		#region ITableStyleOwner members
		ITableStyleOptions ITableStyleOwner.Options { get { return this; } }
		TableStyleFormatCache ITableStyleOwner.Cache { get { return styleFormatCache; } }
		void ITableStyleOwner.CacheColumnStripeInfo(int index, TableStyleStripeInfo info) {
			Columns[index].CachedStripeInfo = info;
		}
		TableStyleStripeInfo ITableStyleOwner.GetColumnStripeInfo(int index) {
			return Columns[index].CachedStripeInfo;
		}
		#endregion
		#region ClearFill
		void IDifferentialFormatPropertyChanger.ClearFill(int elementIndex) {
			if (!GetDifferentialFormat(elementIndex).MultiOptionsInfo.ApplyFillNone)
				ClearFillCore(elementIndex);
		}
		void ClearFillCore(int elementIndex) {
			TableDifferentialFormatIndexAccessor accessor = GetDifferentialFormatIndexAccessor(elementIndex);
			DocumentModel.BeginUpdate();
			try {
				DifferentialFormat info = (DifferentialFormat)GetInfoForModification(accessor);
				info.Fill.Clear();
				ReplaceInfo(accessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		protected internal CellRange GetInsertingRange(int startColumnPosition, int insertedColumnsCount) {
			Guard.ArgumentNonNegative(startColumnPosition, "startColumnPosition");
			Guard.ArgumentPositive(insertedColumnsCount, "insertedColumnsCount");
			int tableColumnsCount = columns.Count;
			if (startColumnPosition > tableColumnsCount)
				Exceptions.ThrowInternalException();
			CellRange result;
			int endColumnPosition = startColumnPosition + insertedColumnsCount - 1;
			if (startColumnPosition < tableColumnsCount)
				result = range.GetSubColumnRange(startColumnPosition, endColumnPosition);
			else {
				result = range.GetSubColumnRange(startColumnPosition - 1, endColumnPosition - 1);
				result.Resize(1, 0, 1, 0);
			}
			return result;
		}
	}
	#endregion
	public class TableLeftPositionComparer : IComparer<ITableBase> {
		public int Compare(ITableBase x, ITableBase y) {
			return Comparer<int>.Default.Compare(x.WholeRange.TopLeft.Column, y.WholeRange.TopLeft.Column);
		}
	}
	#region TableBasesEnumerator
	#endregion
	#region TableRowAllCellsEnumerator
	public class TableRowAllCellsEnumerator : IOrderedEnumerator<ICell> {
		readonly Worksheet worksheet;
		IEnumerator<ITableBase> tablesEnumerator;
		IEnumerator<ICell> cellsEnumerator;
		Row row;
		int leftColumnIndex;
		int rightColumnIndex;
		public TableRowAllCellsEnumerator(Worksheet worksheet, Row row, IEnumerator<ITableBase> tablesEnumerator, int leftColumnIndex, int rightColumnIndex) {
			this.worksheet = worksheet;
			this.row = row;
			this.tablesEnumerator = tablesEnumerator;
			this.leftColumnIndex = leftColumnIndex;
			this.rightColumnIndex = rightColumnIndex;
			Reset();
		}
		public IWorksheet Worksheet { get { return worksheet; } }
		#region IEnumerator<> Members
		public ICell Current { get { return cellsEnumerator.Current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			while (cellsEnumerator == null || !cellsEnumerator.MoveNext()) {
				if (!tablesEnumerator.MoveNext())
					return false;
				cellsEnumerator = CreateCellsEnumerator(tablesEnumerator.Current);
			}
			return true;
		}
		public void Reset() {
			cellsEnumerator = null;
		}
		#endregion
		protected virtual IEnumerator<ICell> CreateCellsEnumerator(ITableBase table) {
			CellRangeBase tableRange = table.WholeRange;
			return new ContinuousCellsEnumerator(row.Cells.InnerList, Math.Max(leftColumnIndex, tableRange.TopLeft.Column), Math.Min(rightColumnIndex, tableRange.BottomRight.Column), false, worksheet, row.Index);
		}
		int IOrderedEnumerator<ICell>.CurrentValueOrder { get { return Current.ColumnIndex; } }
		bool IOrderedEnumerator<ICell>.IsReverseOrder { get { return false; } }
		#region IShiftableEnumerator
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
		}
		#endregion
	}
	#endregion
}
