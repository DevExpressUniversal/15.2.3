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
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using System.Collections;
namespace DevExpress.XtraSpreadsheet.Model {
	#region InsertTableCommand
	public class InsertTableCommand : ErrorHandledWorksheetCommand {
		string displayName;
		CellRange range;
		bool hasHeaders;
		bool applyAutoFitTableColumn;
		string reference;
		string style;
		bool hasNearestBottomContent;
		public InsertTableCommand(IErrorHandler errorHandler, Worksheet worksheet, string displayName, bool hasHeaders, bool applyAutoFitTableColumn)
			: base(worksheet, errorHandler) {
			this.displayName = displayName;
			this.hasHeaders = hasHeaders;
			this.applyAutoFitTableColumn = applyAutoFitTableColumn;
		}
		#region Properties
		public string Reference { get { return reference; } set { reference = value; } }
		public bool HasHeaders { get { return hasHeaders; } set { hasHeaders = value; } }
		public string Style { get { return style; } set { style = value; } }
		public CellRange Range { get { return range; } set { range = value; } }
		#endregion
		protected internal override bool Validate() {
			IModelErrorInfo error = ValidateName(displayName);
			if (error == null) {
				CellRangeBase rangeBase = GetActualRange();
				error = ValidateRange(rangeBase);
				range = rangeBase as CellRange;
			}
			return HandleError(error);
		}
		CellRangeBase GetActualRange() {
			if (range != null)
				return range;
			DataContext.PushCurrentWorksheet(Worksheet);
			try {
				return CellRangeBase.TryParse(reference, DataContext);
			} finally {
				DataContext.PopCurrentWorksheet();
			}
		}
		protected internal IModelErrorInfo ValidateRange(CellRangeBase range) {
			if (range == null)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			if (range.RangeType == CellRangeType.UnionRange)
				return new ModelErrorInfo(ModelErrorType.UnionRangeNotAllowed);
			if (range.Worksheet == null)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			if (!object.ReferenceEquals(range.Worksheet, Worksheet))
				return new ModelErrorInfo(ModelErrorType.ErrorUseRangeFromAnotherWorksheet);
			CellRange cellRange = (CellRange)range;
			cellRange = (CellRange)cellRange.Clone();
			IModelErrorInfo error = CanPrepareRange(cellRange);
			if (error != null)
				return error;
			if (Worksheet.Tables.ContainsItemsInRange(cellRange, true))
				return new ModelErrorInfo(ModelErrorType.TableCanNotOverlapTable);
			if (Worksheet.PivotTables.ContainsItemsInRange(cellRange, true))
				return new ModelErrorInfo(ModelErrorType.TableCanNotOverlapTable);
			if (Worksheet.ArrayFormulaRanges.CheckMultiCellArrayFormulasInRange(cellRange))
				return new ModelErrorInfo(ModelErrorType.MuiltiCellArrayFormulaInTable);
			if (InvalidSheetFilterIntersection(cellRange))
				return new ModelErrorInfo(ModelErrorType.AutoFilterCannotBeChanged);
			return null;
		}
		IModelErrorInfo CanPrepareRange(CellRange tableRange) {
			bool oneRowTable = tableRange.Height == 1;
			if (GetAffectsLastRow(tableRange)) {
				if (oneRowTable) 
					return new ModelErrorInfo(ModelErrorType.TableCannotBeCreatedInTheLastRow);
				if (!hasHeaders)
					return CanRangeInsert(tableRange.GetSubRowRange(0, 0));
				return null;
			}
			if (oneRowTable && hasHeaders) {
				hasNearestBottomContent = HasNearestBottomContent(tableRange);
				if (hasNearestBottomContent)
					return CanRangeInsert(tableRange.GetSubRowRange(1, 1));
			} else if (!hasHeaders) {
				hasNearestBottomContent = HasNearestBottomContent(tableRange);
				if (hasNearestBottomContent)
					return CanRangeInsert(tableRange.GetSubRowRange(0, 0));
			}
			return null;
		}
		bool InvalidSheetFilterIntersection(CellRange range) {
			SheetAutoFilter filter = Worksheet.AutoFilter;
			if (filter.Enabled) {
				CellRange filterRange = filter.Range;
				if (range.Intersects(filterRange)) {
					if (range.TopLeft.Row < filterRange.TopLeft.Row)
						return true;
					if (range.ContainsRange(filterRange))
						return false;
					filterRange = filter.GetShrinkedRange();
					return !range.ContainsRange(filterRange);
				}
			}
			return false;
		}
		protected internal IModelErrorInfo ValidateName(string name) {
			if (string.IsNullOrEmpty(name))
				return null;
			return DocumentModel.CheckTableName(name);
		}
		bool GetAffectsLastRow(CellRange range) {
			return range.BottomRowIndex >= Worksheet.MaxRowCount - 1;
		}
		CellRange PrepareRange(CellRange range) {
			range = (CellRange)range.Clone();
			bool affectsLastRow = GetAffectsLastRow(range);
			if (affectsLastRow || (hasHeaders && range.Height > 1)) 
				return range;
			if (hasHeaders && hasNearestBottomContent) 
				InsertRange(range.GetSubRowRange(1, 1), InsertCellsFormatMode.FormatAsPrevious);
			if (!hasHeaders) {
				if (hasNearestBottomContent)
					InsertRange(range.GetSubRowRange(0, 0), InsertCellsFormatMode.FormatAsNext);
				else {
					CutPasteRange(range, range.GetResized(0, 1, 0, 1));
					CopyFormatting(range.GetSubRowRange(1, 1), range.GetSubRowRange(0, 0));
				}
			}
			range.Resize(0, 0, 0, 1);
			return range;
		}
		IModelErrorInfo CanRangeInsert(CellRange insertableRange) {
			return DocumentModel.CanRangeInsert(insertableRange, InsertCellMode.ShiftCellsDown, InsertCellsFormatMode.ClearFormat, NotificationChecks.All);
		}
		void InsertRange(CellRange range, InsertCellsFormatMode mode) {
			Worksheet.InsertRange(range, InsertCellMode.ShiftCellsDown, mode, ErrorHandler);
		}
		void CopyFormatting(CellRange sourceRange, CellRange targetRange) {
			SourceTargetRangesForCopy ranges = new SourceTargetRangesForCopy(sourceRange, targetRange);
			RangeCopyOperation copyOperation = new RangeCopyOperation(ranges, ModelPasteSpecialFlags.FormatAndStyle);
			copyOperation.SuppressChecks = true;
			copyOperation.ErrorHandler = ErrorHandler;
			copyOperation.Execute();
		}
		void CutPasteRange(CellRange sourceRange, CellRange targetRange) {
			CutRangeOperation operation = new CutRangeOperation(sourceRange, targetRange);
			operation.ErrorHandler = ErrorHandler;
			operation.SuppressChecks = true;
			operation.Execute();
		}
		bool HasNearestBottomContent(CellRange range) {
			int startRow = range.BottomRowIndex - range.TopRowIndex + 1;
			CellRange bottomRange = range.GetSubRowRange(startRow, startRow);
			return bottomRange != null && (bottomRange.HasData() || DocumentModel.CalculationChain.GetDirectDependents(bottomRange, false) != null);
		}
		void ProcessAutoFilters(TableAutoFilter tableFilter, SheetAutoFilter sheetFilter) {
			CellRange tableFilterRange = tableFilter.Range;
			if (sheetFilter.Enabled) {
				CellRange sheetFilterRange = sheetFilter.Range;
				if (tableFilterRange.Intersects(sheetFilterRange)) {
					bool needToCopyFromSheetFilter = tableFilter.Range.EqualsPosition(sheetFilterRange);
					if (needToCopyFromSheetFilter)
						tableFilter.CopyFrom(sheetFilter);
					sheetFilter.Disable();
					if (needToCopyFromSheetFilter)
						tableFilter.ReApplyFilter();
				}
			}
		}
		protected internal override void ExecuteCore() {
			if (string.IsNullOrEmpty(displayName)) {
				ITableNameCreationService service = DocumentModel.GetService<ITableNameCreationService>();
				displayName = service.GetNewTableName(DocumentModel.GetTableNames());
			}
			range = PrepareRange(range);
			Worksheet.UnMergeCells(range, ErrorHandler);
			Table table = new Table(Worksheet, displayName, range);
			ProcessAutoFilters(table.AutoFilter, Worksheet.AutoFilter);
			table.Style = DocumentModel.StyleSheet.TableStyles.DefaultTableStyle;
			Worksheet.Tables.Add(table);
			TableColumnsInitialCreationCommand command = new TableColumnsInitialCreationCommand(table);
			command.Execute();
			ApplyTableProperties(table);
			if (applyAutoFitTableColumn && table.ShowAutoFilterButton && table.HasHeadersRow)
				Worksheet.TryBestFitColumn(range.GetSubRowRange(0, 0), Layout.Engine.ColumnBestFitMode.IgnoreSetBestFit | Layout.Engine.ColumnBestFitMode.ExpandOnly);
			Result = table;
		}
		protected virtual void ApplyTableProperties(Table table) {
		}
	}
	#endregion
	#region InsertTableWithDefaultStyleCommand
	public class InsertTableWithDefaultStyleCommand : InsertTableCommand {
		public InsertTableWithDefaultStyleCommand(IErrorHandler errorHandler, Worksheet worksheet, string displayName, bool hasHeaders, bool applyAutoFitTableColum)
			: base(errorHandler, worksheet, displayName, hasHeaders, applyAutoFitTableColum) {
		}
		protected override void ApplyTableProperties(Table table) {
			table.Style = GetTableStyle();
			Worksheet.Selection.SetSelection(table.Range);
		}
		TableStyle GetTableStyle() {
			TableStyleCollection tableStyles = DocumentModel.StyleSheet.TableStyles;
			if (!tableStyles.HasDefaultTableStyleName)
				return TableStyle.CreateTablePredefinedStyle(DocumentModel, PredefinedTableStyleId.TableStyleMedium2);
			return tableStyles.DefaultTableStyle;
		}
	}
	#endregion
	#region InsertTableWithStyleCommand
	public class InsertTableWithStyleCommand : InsertTableCommand {
		public InsertTableWithStyleCommand(IErrorHandler errorHandler, Worksheet worksheet, string displayName, bool hasHeaders, bool applyAutoFitTableColumn, string style)
			: base(errorHandler, worksheet, displayName, hasHeaders, applyAutoFitTableColumn) {
			this.Style = style;
		}
		protected override void ApplyTableProperties(Table table) {
			table.Style = DocumentModel.StyleSheet.TableStyles[Style];
			Worksheet.Selection.SetSelection(table.Range);
		}
	}
	#endregion
	#region TableRemoveCommand
	public class TableRemoveCommand : SpreadsheetModelCommand {
		Table table;
		int index = Int32.MinValue;
		public TableRemoveCommand(Worksheet worksheet, int index)
			: base(worksheet) {
			this.table = worksheet.Tables[index];
			this.index = index;
		}
		protected internal override void ExecuteCore() {
			DocumentHistory history = DocumentModel.History;
			Worksheet.Tables.RemoveAt(index);
			TableRemovedFormulaWalker walker = new TableRemovedFormulaWalker(table.Name, table.Worksheet.DataContext);
			walker.Walk(table.DocumentModel);
		}
	}
	#endregion
	#region TableRemoveApiCommand
	public class TableRemoveApiCommand : ErrorHandledWorksheetCommand {
		readonly Table table;
		int index;
		public TableRemoveApiCommand(IErrorHandler errorHandler, Table table)
			: this(errorHandler, table.Worksheet) {
			this.table = table;
		}
		protected TableRemoveApiCommand(IErrorHandler errorHandler, Worksheet worksheet)
			: base(worksheet, errorHandler) {
		}
		protected internal override bool Validate() {
			index = Worksheet.Tables.IndexOf(table);
			if (index < 0)
				return HandleError(new ModelErrorInfo(ModelErrorType.UsingInvalidObject));
			return base.Validate();
		}
		protected internal override void ExecuteCore() {
			ClearTableReferences(table);
			Worksheet.Tables.RemoveAt(index);
			Worksheet.ClearAll(table.Range, this.ErrorHandler);
		}
		protected void ClearTableReferences(Table table) {
			TableConvertedToRangeFormulaWalker walker = new TableConvertedToRangeFormulaWalker(table.Name, DataContext);
			walker.Walk(DocumentModel);
		}
	}
	#endregion
	#region TableCollectionClearApiCommand
	public class TableCollectionClearApiCommand : TableRemoveApiCommand {
		public TableCollectionClearApiCommand(IErrorHandler errorHandler, Worksheet worksheet)
			: base(errorHandler, worksheet) {
		}
		protected internal override bool Validate() {
			return true;
		}
		protected internal override void ExecuteCore() {
			if (Worksheet.Tables.Count <= 0)
				return;
			List<CellRangeBase> tableRanges = new List<CellRangeBase>();
			foreach (Table table in Worksheet.Tables) {
				ClearTableReferences(table);
				tableRanges.Add(table.Range);
			}
			Worksheet.Tables.Clear();
			foreach (CellRangeBase tableRange in tableRanges)
				Worksheet.ClearAll(tableRange, this.ErrorHandler);
			DocumentModel.InternalAPI.OnTableCollectionClear(Worksheet);
		}
	}
	#endregion
	#region TableRenameCommand
	public class TableRenameCommand : SpreadsheetModelCommand {
		readonly Table table;
		readonly string newName;
		public TableRenameCommand(Table table, string newName)
			: base(table.Worksheet) {
			this.table = table;
			this.newName = newName;
		}
		protected internal override void ExecuteCore() {
			string oldName = table.Name;
			DocumentHistory history = DocumentModel.History;
			ChangeTableDisplayNameHistoryItem historyItem = new ChangeTableDisplayNameHistoryItem(table, oldName, newName);
			history.Add(historyItem);
			historyItem.Execute();
			TableRenamedFormulaWalker walker = new TableRenamedFormulaWalker(oldName, newName, table.Worksheet.DataContext);
			walker.Walk(table.DocumentModel);
		}
	}
	#endregion
	#region TableInsertColumnCommand
	#endregion
	#region TableRemoveColumnCommand
	public class TableColumnRemoveCommand : ErrorHandledWorksheetCommand {
		int index;
		Table table;
		public TableColumnRemoveCommand(Table table, int index, IErrorHandler errorHandler)
			: base(table.Worksheet, errorHandler) {
			this.table = table;
			this.index = index;
		}
		protected internal override void ExecuteCore() {
			CellRange deletingRange = table.Range.GetSubColumnRange(index, index);
			Worksheet.RemoveRange(deletingRange, RemoveCellMode.ShiftCellsLeft, true, false, ErrorHandler);
			table.RemoveColumnCore(index);
			table.Range = table.Range.GetResized(0, 0, -1, 0);
		}
	}
	#endregion
	#region CellDisplayTextInfo (struct)
	public struct CellDisplayTextInfo {
		#region Fields
		string displayText;
		bool fromFormulas;
		#endregion
		public CellDisplayTextInfo(string displayText, bool fromFormulas) {
			this.displayText = displayText;
			this.fromFormulas = fromFormulas;
		}
		public CellDisplayTextInfo(ICell cell) {
			if (cell != null && !cell.Value.IsEmpty) {
				this.displayText = cell.Text;
				this.fromFormulas = cell.HasFormula || !cell.Value.IsText;
			} else {
				this.displayText = string.Empty;
				this.fromFormulas = false;
			}
		}
		#region Properties
		public string DisplayText { get { return displayText; } }
		public bool FromFormulas { get { return fromFormulas; } }
		#endregion
		public bool IsEquals(string value) {
			return StringExtensions.CompareInvariantCultureIgnoreCase(value, displayText) == 0;
		}
	}
	#endregion
	#region TableColumnsInitialCreationCommand
	public class TableColumnsInitialCreationCommand : SpreadsheetModelCommand {
		#region Fields
		readonly Table table;
		readonly string defaultColumnNamePrefix;
		readonly HashSet<string> cachedTexts;
		readonly Dictionary<int, CellDisplayTextInfo> cachedInfoes;
		int currentPostfix;
		int lastPostfix;
		#endregion
		public TableColumnsInitialCreationCommand(Table table)
			: this(table, new Dictionary<int, CellDisplayTextInfo>()) {
		}
		public TableColumnsInitialCreationCommand(Table table, Dictionary<int, CellDisplayTextInfo> cachedInfoes)
			: base(table.Worksheet) {
			this.table = table;
			this.defaultColumnNamePrefix = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DefaultInitialTableColumnNamePrefix);
			this.cachedTexts = new HashSet<string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.cachedInfoes = cachedInfoes;
			this.currentPostfix = 1;
			this.lastPostfix = -1;
		}
		#region Properties
		CellRange TableRange { get { return table.Range; } }
		int ColumnCount { get { return TableRange.Width; } }
		#endregion
		protected internal override bool Validate() {
			return table.HasHeadersRow;
		}
		protected internal override void ExecuteCore() {
			for (int i = 0; i < ColumnCount; i++) {
				bool nameGenerated;
				string columnName = GetColumnName(i, out nameGenerated);
				AddColumn(i, columnName, nameGenerated);
			}
		}
		void AddColumn(int position, string newColumnName, bool nameGenerated) {
			TableColumn column = new TableColumn(table, newColumnName);
			table.InsertColumnCore(column, position);
			if (nameGenerated)
				column.SetHeadersCellValue();
		}
		string GetColumnName(int position, out bool nameGenerated) {
			CellDisplayTextInfo info = GetCellDisplayTextInfo(position);
			string displayText = info.DisplayText;
			nameGenerated = false;
			if (String.IsNullOrEmpty(displayText)) {
				displayText = GetColumnNameCore(defaultColumnNamePrefix, position);
				lastPostfix = currentPostfix;
				nameGenerated = true;
			} else if (cachedTexts.Contains(displayText)) {
				currentPostfix++;
				displayText = currentPostfix == 2 ? GetColumnNameCore(displayText, position) : GetName(displayText, currentPostfix);
				lastPostfix = currentPostfix;
				nameGenerated = true;
			} if (info.FromFormulas)
				nameGenerated = true;
			cachedTexts.Add(displayText);
			return displayText;
		}
		CellDisplayTextInfo GetCellDisplayTextInfo(int position) {
			if (cachedInfoes.ContainsKey(position))
				return cachedInfoes[position];
			ICell columnHeaderCell = TableRange.TryGetCellRelative(position, 0) as ICell;
			CellDisplayTextInfo result = new CellDisplayTextInfo(columnHeaderCell);
			cachedInfoes.Add(position, result);
			return result;
		}
		string GetColumnNameCore(string prefix, int position) {
			string result = GetName(prefix, currentPostfix);
			while (ContainsColumnName(result, position))
				result = GetName(prefix, ++currentPostfix);
			return result;
		}
		string GetName(string prefix, int postfix) {
			return String.Format("{0}{1}", prefix, postfix);
		}
		bool ContainsColumnName(string columnName, int position) {
			if (cachedTexts.Contains(columnName) || currentPostfix <= lastPostfix)
				return true;
			for (int i = position + 1; i < ColumnCount; i++)
				if (GetCellDisplayTextInfo(i).IsEquals(columnName)) 
					return true;
			return false;
		}
	}
	#endregion
	#region TableColumnNamesCorrectedOperationBase
	public abstract class TableColumnNamesCorrectedOperationBase {
		#region Fields
		protected const int DefaultColumnPostfix = 1;
		Dictionary<string, string> tableColumnsNamesRenamed;
		IList<int> renamedColumnPositions = new List<int>();
		Dictionary<string, int> cachedTexts = new Dictionary<string, int>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		Table table;
		string columnPrefix;
		int columnPostfix;
		bool shouldCorrectColumnFormulas;
		bool shouldUpdateColumnCells;
		#endregion
		#region Properties
		public bool ShouldCorrectColumnFormulas { get { return shouldCorrectColumnFormulas; } set { shouldCorrectColumnFormulas = value; } }
		public bool ShouldUpdateColumnCells { get { return shouldUpdateColumnCells; } set { shouldUpdateColumnCells = value; } }
		public Dictionary<string, string> TableColumnsNamesRenamed { get { return tableColumnsNamesRenamed; } protected set { tableColumnsNamesRenamed = value; } }
		public Table Table { get { return table; } protected set { table = value; } }
		protected IList<int> RenamedColumnPositions { get { return renamedColumnPositions; } }
		protected Dictionary<string, int> CachedTexts { get { return cachedTexts; } }
		protected TableColumnInfoCollection Columns { get { return table.Columns; } }
		protected CellRange TableRange { get { return table.Range; } }
		protected DocumentModel DocumentModel { get { return table.DocumentModel; } }
		protected string DefaultPrefix { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DefaultInitialTableColumnNamePrefix); } }
		protected string ColumnPrefix { get { return columnPrefix; } set { columnPrefix = value; } }
		protected int ColumnPostfix { get { return columnPostfix; } set { columnPostfix = value; } }
		#endregion
		public bool Init(Table table, CellRangeBase intersectedRangeBase, Dictionary<string, string> tableColumnsNamesRenamed) {
			if (table == null || intersectedRangeBase == null || tableColumnsNamesRenamed == null)
				return false;
			CellRange tableHeaderRange = table.TryGetHeadersRowRange();
			if (tableHeaderRange == null)
				return false;
			this.table = table;
			this.tableColumnsNamesRenamed = tableColumnsNamesRenamed;
			renamedColumnPositions = new List<int>();
			cachedTexts = new Dictionary<string, int>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			InitPrefixPostfix(DefaultPrefix, DefaultColumnPostfix);
			return InitCore(tableHeaderRange, intersectedRangeBase);
		}
		protected string GetColumnName(string prefix, int postfix) {
			return String.Format("{0}{1}", prefix, postfix);
		}
		protected string CalculateColumnNameByPostfixCore(int position) {
			string result = GetColumnName(columnPrefix, columnPostfix);
			while (ContainsColumnName(result, position))
				result = GetColumnName(columnPrefix, ++columnPostfix);
			return result;
		}
		protected void InitPrefixPostfix(string columnPrefix, int columnPostfix) {
			this.columnPrefix = columnPrefix;
			this.columnPostfix = columnPostfix;
		}
		public void Execute() {
			ExecuteCore();
			ChangeTableColumnNamesHistoryItem historyItem = new ChangeTableColumnNamesHistoryItem(this);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
			if (shouldCorrectColumnFormulas) {
				TableColumnRenamedFormulaWalker walker = new TableColumnRenamedFormulaWalker(table.Name, tableColumnsNamesRenamed, DocumentModel.DataContext);
				walker.Walk(DocumentModel);
			}
		}
		#region SetNames
		internal void SetOldNames() {
			OnBeforeChangeColumnNames();
			RenameColumns(tableColumnsNamesRenamed.Keys);
		}
		internal void SetNewNames() {
			OnBeforeChangeColumnNames();
			RenameColumns(tableColumnsNamesRenamed.Values);
		}
		void RenameColumns(ICollection<string> newColumnNames) {
			if (ShouldUpdateColumnCells)
				RenameColumnsWithUpdateCells(newColumnNames);
			else
				RenameColumnsWithoutUpdateCells(newColumnNames);
		}
		void RenameColumnsWithUpdateCells(ICollection<string> newColumnNames) {
			int i = 0;
			foreach (string newName in newColumnNames) {
				int position = renamedColumnPositions[i];
				RenameColumnCore(position, newName);
				SetCellValue(position, newName);
				i++;
			}
		}
		void RenameColumnsWithoutUpdateCells(ICollection<string> newColumnNames) {
			int i = 0;
			foreach (string newName in newColumnNames) {
				int position = renamedColumnPositions[i];
				RenameColumnCore(position, newName);
				i++;
			}
		}
		void RenameColumnCore(int position, string newName) {
			TableColumn column = Columns[position];
			column.SetNameCore(newName);
			Columns.AddHashKey(column);
			Columns.SubscribeOnNameChanged(column);
		}
		void OnBeforeChangeColumnNames() {
			foreach (int position in renamedColumnPositions) {
				TableColumn column = Columns[position];
				Columns.UnsubscribeOnNameChanged(column);
				Columns.RemoveHashKey(column);
			}
		}
		void SetCellValue(int position, string value) {
			ICell cell = TableRange.GetCellRelative(position, 0) as ICell;
			if (cell != null) {
				if (DocumentModel.AffectedCellsRepository != null)
					DocumentModel.AffectedCellsRepository.Add(new CellContentSnapshot(cell));
				cell.ReplaceValueByValueCore(value);
			}
		}
		#endregion
		protected void RegisterColumnNameCore(int position, string newName) {
			TableColumn column = Columns[position];
			tableColumnsNamesRenamed.Add(column.Name, newName);
			renamedColumnPositions.Add(position);
		}
		protected bool ContainsColumnName(string name, int currentPosition) {
			return cachedTexts.ContainsKey(name) ? true : ContainsColumnNameCore(name, currentPosition);
		}
		protected abstract void ExecuteCore();
		protected abstract bool InitCore(CellRange tableHeaderRange, CellRangeBase intersectionRange);
		protected abstract bool ContainsColumnNameCore(string name, int currentPosition);
	}
	#endregion
	#region TableColumnNamesClearOperation
	public class TableColumnNamesClearOperation : TableColumnNamesCorrectedOperationBase {
		int rightColumnIndex;
		#region Properties
		public CellRangeBase ClearableHeaderRange { get; set; }
		int TableLeftColumn { get { return TableRange.LeftColumnIndex; } }
		int TopRowIndex { get { return TableRange.TopRowIndex; } }
		#endregion
		protected override bool InitCore(CellRange tableHeaderRange, CellRangeBase intersectionRange) {
			VariantValue value =  tableHeaderRange.IntersectionWith(intersectionRange);
			if (value.IsError || value.CellRangeValue == null)
				return false;
			ClearableHeaderRange = value.CellRangeValue;
			rightColumnIndex = ClearableHeaderRange.BottomRight.Column - TableLeftColumn;
			return true;
		}
		#region ExecuteCore
		protected override void ExecuteCore() {
			int leftColumnIndex = ClearableHeaderRange.TopLeft.Column - TableLeftColumn;
			for (int i = 0; i < leftColumnIndex; i++)
				CachedTexts.Add(Columns[i].Name, i);
			int columnsCount = Columns.Count;
			for (int i = rightColumnIndex + 1; i < columnsCount; i++)
				CachedTexts.Add(Columns[i].Name, i);
			for (int i = leftColumnIndex; i <= rightColumnIndex; i++) {
				if (ClearableHeaderRange.ContainsCell(TableLeftColumn + i, TopRowIndex)) {
					string newName = CalculateColumnNameByPostfixCore(i);
					CachedTexts.Add(newName, i);
					RegisterColumnNameCore(i, newName);
				} else
					CachedTexts.Add(Columns[i].Name, i);
			}
		}
		#endregion
		protected override bool ContainsColumnNameCore(string name, int position) {
			for (int i = position + 1; i <= rightColumnIndex; i++) {
				if (!ClearableHeaderRange.ContainsCell(TableLeftColumn + position, TopRowIndex) && 
					(StringExtensions.CompareInvariantCultureIgnoreCase(name, Columns[i].Name) == 0))
					return true;
			}
			return false;
		}
	}
	#endregion
	#region TableColumnNamesChangeOperation
	public class TableColumnNamesChangeOperation : TableColumnNamesCorrectedOperationBase {
		#region Fields
		const int emptyPosition = -1;
		const int locateState = 0;
		const int createState = 1;
		const int recalculateState = 2;
		Dictionary<int, CellDisplayTextInfo> cachedCellInfoes = new Dictionary<int, CellDisplayTextInfo>();
		CellRange headerRange;
		CellDisplayTextInfo cellInfo;
		int state;
		int position;
		int startPosition;
		bool nameGenerated = false;
		string columnName;
		#endregion
		string DisplayText { get { return cellInfo.DisplayText; } }
		#region Init
		public bool Init(Table table, ICell intersectedCell, Dictionary<string, string> renamedColumnNames) {
			if (table == null || intersectedCell == null || renamedColumnNames == null || 
				!table.HasHeadersRow || intersectedCell.RowIndex != table.Range.TopRowIndex)
				return false;
			int startColumnPosition = intersectedCell.ColumnIndex - table.Range.LeftColumnIndex;
			return InitCore(table, startColumnPosition, renamedColumnNames);
		}
		public bool Init(Table table, int startColumnPosition, Dictionary<string, string> renamedColumnNames) {
			if (table == null || renamedColumnNames == null)
				return false;
			return InitCore(table, startColumnPosition, renamedColumnNames);
		}
		protected override bool InitCore(CellRange tableHeaderRange, CellRangeBase intersectedRangeBase) {
			if (intersectedRangeBase.RangeType == CellRangeType.UnionRange)
				return false;
			CellRange intersectedRange = intersectedRangeBase as CellRange;
			if (tableHeaderRange.TopRowIndex < intersectedRange.TopRowIndex)
				return false;
			int startPosition = Math.Max(0, intersectedRange.LeftColumnIndex - tableHeaderRange.LeftColumnIndex);
			return InitCore(tableHeaderRange, startPosition);
		}
		bool InitCore(Table table, int startColumnPosition, Dictionary<string, string> renamedColumnNames) {
			if (startColumnPosition < 0)
				return false;
			CellRange tableHeaderRange = table.TryGetHeadersRowRange();
			if (tableHeaderRange == null)
				return false;
			Table = table;
			TableColumnsNamesRenamed = renamedColumnNames;
			InitPrefixPostfix(DefaultPrefix, DefaultColumnPostfix);
			return InitCore(tableHeaderRange, startPosition);
		}
		bool InitCore(CellRange headerRange, int startPosition) {
			this.cachedCellInfoes = new Dictionary<int, CellDisplayTextInfo>();
			this.startPosition = startPosition;
			this.headerRange = headerRange;
			this.state = locateState;
			this.position = emptyPosition;
			this.nameGenerated = false;
			return CacheLeftCells();
		}
		bool CacheLeftCells() {
			if (startPosition == 0)
				return true;
			if (!CacheLeftCellsCore(0))
				return false;
			for (int i = 1; i < startPosition; i++)
				if (!CacheLeftCellsCore(i))
					return false;
			return true;
		}
		bool CacheLeftCellsCore(int position) {
			this.position = position;
			this.cellInfo = GetCellInfo(position);
			if (String.IsNullOrEmpty(DisplayText) || CachedTexts.ContainsKey(DisplayText))
				return false;
			this.nameGenerated = false;
			this.columnName = DisplayText;
			CachedTexts.Add(DisplayText, position);
			return true;
		}
		#endregion
		#region ExecuteCore
		protected override void ExecuteCore() {
			int count = Columns.Count - startPosition;
			for (int i = 0; i < count; i++) {
				CalculateColumnName();
				CachedTexts.Add(columnName, position);
				RegisterColumnName();
			}
		}
		void CalculateColumnName() {
			position++;
			cellInfo = GetCellInfo(position);
			string displayText = cellInfo.DisplayText;
			if (String.IsNullOrEmpty(displayText))
				CreateName();
			else if (CachedTexts.ContainsKey(displayText))
				RecalculateName();
			else
				LocateName();
		}
		void RegisterColumnName() {
			string oldName = Columns[position].Name;
			bool isDifferentName = StringExtensions.CompareInvariantCultureIgnoreCase(oldName, columnName) != 0;
			if (nameGenerated || isDifferentName) 
				RegisterColumnNameCore(position, columnName);
		}
		#endregion
		#region LocateName
		void LocateName() {
			nameGenerated = cellInfo.FromFormulas;
			columnName = cellInfo.DisplayText;
			state = locateState;
		}
		#endregion
		#region CreateName
		void CreateName() {
			nameGenerated = true;
			if (position == 0)
				columnName = CalculateColumnNameByPostfixCore(position);
			else if (state == locateState) {
				string previousColumnName = columnName;
				int previousColumnPostfix;
				string previousColumnPrefix = TryParseColumnNamePrefix(previousColumnName, out previousColumnPostfix);
				if (previousColumnPostfix == -1) {
					InitPrefixPostfix(DefaultPrefix, DefaultColumnPostfix);
					columnName = CalculateColumnNameByPostfixCore(position);
				} else {
					InitPrefixPostfix(previousColumnPrefix, previousColumnPostfix + 1);
					columnName = GetColumnName(ColumnPrefix, ColumnPostfix);
					ColumnPostfix++;
					if (ContainsColumnName(columnName, position)) {
						InitPrefixPostfix(previousColumnName, 2);
						columnName = CalculateColumnNameByPostfixCore(position);
					}
				}
			} else {
				ColumnPostfix++;
				CalculateColumnNameByPostfix(columnName);
			}
			state = createState;
		}
		#endregion
		#region RecalculateName
		void RecalculateName() {
			nameGenerated = true;
			if (state != recalculateState) {
				InitPrefixPostfix(DisplayText, 2);
				columnName = CalculateColumnNameByPostfixCore(position);
			} else {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(DisplayText, columnName) != 0)
					ColumnPrefix = DisplayText;
				ColumnPostfix++;
				CalculateColumnNameByPostfix(DisplayText);
			}
			state = recalculateState;
		}
		void CalculateColumnNameByPostfix(string displayText) {
			columnName = GetColumnName(ColumnPrefix, ColumnPostfix);
			if (ContainsColumnName(columnName, position)) {
				InitPrefixPostfix(displayText, 2);
				columnName = CalculateColumnNameByPostfixCore(position);
			}
		}
		#endregion
		#region Parse
		string TryParseColumnNamePrefix(string text, out int postfix) {
			string prefix = text;
			int i = prefix.Length - 1;
			if (i < 0 || !char.IsDigit(prefix[i])) {
				postfix = -1;
				return prefix;
			}
			int postfixLength;
			postfix = TryParseColumnNamePostfixCore(prefix, out postfixLength);
			return prefix.Substring(0, prefix.Length - postfixLength);
		}
		int TryParseColumnNamePostfixCore(string postfix, out int postfixLength) {
			postfixLength = 0;
			int i = postfix.Length - 1;
			int j = 1;
			int result = 0;
			while (i >= 0 && char.IsDigit(postfix[i])) {
				result += (postfix[i] - 48) * j;
				i--;
				j *= 10;
				postfixLength++;
			}
			return result;
		}
		#endregion
		protected override bool ContainsColumnNameCore(string name, int position) {
			int count = Columns.Count;
			for (int i = position + 1; i < count; i++)
				if (GetCellInfo(i).IsEquals(name))
					return true;
			return false;
		}
		CellDisplayTextInfo GetCellInfo(int position) {
			if (cachedCellInfoes.ContainsKey(position))
				return cachedCellInfoes[position];
			else {
				ICell columnHeaderCell = headerRange.TryGetCellRelative(position, 0) as ICell;
				CellDisplayTextInfo result = new CellDisplayTextInfo(columnHeaderCell);
				cachedCellInfoes.Add(position, result);
				return result;
			}
		}
	}
	#endregion
	#region TableColumnNamesCreateOperation
	public class TableColumnNamesCreateOperation {
		#region Fields
		string prefix;
		int currentPostfix;
		int lastPostfix;
		bool isCustomNameGenerate;
		string rightSpaces;
		#endregion
		public void Init(TableColumnInfoCollection columns, int startPosition) {
			string defaultPrefix = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DefaultInitialTableColumnNamePrefix);
			if (startPosition == 0) {
				InitPrefixPostfix(defaultPrefix, 0);
				return;
			}
			prefix = columns[startPosition - 1].Name;
			string trimmedPrefix = prefix.TrimEnd(' ');
			rightSpaces = prefix.Remove(0, trimmedPrefix.Length);
			int i = trimmedPrefix.Length - 1;
			int postfix = 0;
			int j = 1;
			while (i >= 0) {
				char value = trimmedPrefix[i];
				if (char.IsDigit(value)) {
					postfix += (value - 48) * j;
					i--;
					j *= 10;
					isCustomNameGenerate = true;
				} else
					break;
			}
			if (isCustomNameGenerate)
				InitPrefixPostfix(i >= 0 ? trimmedPrefix.Remove(i + 1) : string.Empty, postfix);
			else
				InitPrefixPostfix(defaultPrefix, 0);
		}
		public string GetNextName(TableColumnInfoCollection columns) {
			if (isCustomNameGenerate)
				return GetNextNameCore(columns);
			if (currentPostfix == 0) {
				currentPostfix++;
				return GetNameCore(columns);
			}
			return GetNextNameCore(columns);
		}
		#region Internal
		void InitPrefixPostfix(string prefix, int postfix) {
			this.prefix = prefix;
			this.currentPostfix = postfix;
			this.lastPostfix = -1;
		}
		string GetNameCore(TableColumnInfoCollection columns) {
			CalculateCurrentPostfix(columns);
			return GetName(prefix, currentPostfix);
		}
		string GetNextNameCore(TableColumnInfoCollection columns) {
			if (ContainsColumnName(columns, GetName(prefix, currentPostfix + 1))) {
				string newPrefix = GetName(prefix, currentPostfix) + rightSpaces;
				InitPrefixPostfix(newPrefix, 2);
				return GetNameCore(columns);
			}
			return GetName(prefix, ++currentPostfix);
		}
		string GetName(string prefix, int postfix) {
			return String.Format("{0}{1}", prefix, postfix);
		}
		void CalculateCurrentPostfix(TableColumnInfoCollection columns) {
			string name = GetName(prefix, currentPostfix);
			while (ContainsColumnName(columns, name))
				name = GetName(prefix, ++currentPostfix);
		}
		bool ContainsColumnName(TableColumnInfoCollection columns, string name) {
			if (currentPostfix <= lastPostfix)
				return true;
			bool result = columns.ContainsName(name);
			if (result)
				lastPostfix = currentPostfix;
			return result;
		}
		#endregion
	}
	#endregion
	#region TableColumnRenameOperation
	public class TableColumnRenameOperation {
		#region Fields
		const int defaultColumnPostfix = 1;
		readonly Dictionary<string, string> renamedColumnNames;
		readonly HashSet<string> cachedLeftNames;
		readonly TableColumn currentColumn;
		int currentPosition;
		string newName;
		#endregion
		public TableColumnRenameOperation(TableColumn column, string newName) {
			Guard.ArgumentNotNull(column, "column");
			this.renamedColumnNames = new Dictionary<string, string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.cachedLeftNames = new HashSet<string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.currentColumn = column;
			this.newName = newName;
			Init();
		}
		#region Properties
		internal Dictionary<string, string> RenamedColumnNames { get { return renamedColumnNames; } }
		Table Table { get { return currentColumn.Table; } }
		TableColumnInfoCollection Columns { get { return Table.Columns; } }
		DocumentModel DocumentModel { get { return Table.Worksheet.Workbook; } }
		#endregion
		void Init() {
			int count = Columns.Count;
			for (int position = 0; position < count; position++) {
				this.currentPosition = position;
				TableColumn column = Columns[position];
				if (object.ReferenceEquals(column, currentColumn))
					break;
				cachedLeftNames.Add(column.Name);
			}
		}
		#region Execute
		public void Execute() {
			if (String.IsNullOrEmpty(newName)) {
				CalculateDefaultColumnName();
				RenameColumnCore(currentPosition, newName);
			} else if (!Columns.ContainsName(newName))
				RenameColumnCore(currentPosition, newName);
			else {
				if (cachedLeftNames.Contains(newName)) {
					newName = GetNewColumnNameByPostfix(newName, 2);
					RenameColumnCore(currentPosition, newName);
				} else {
					int conflictedColumnPosition = TryGetRightConflictedColumnPosition();
					if (conflictedColumnPosition != -1) {
						string conflictedColumnName = GetNewColumnNameByPostfix(newName, 2);
						RenameColumnCore(conflictedColumnPosition, conflictedColumnName);
						RenameColumnCore(currentPosition, newName);
					} else if (CheckNameGenerated())
						RenameColumnCore(currentPosition, newName);
				}
			}
			TableColumnRenamedFormulaWalker walker = new TableColumnRenamedFormulaWalker(Table.Name, RenamedColumnNames, DocumentModel.DataContext);
			walker.Walk(DocumentModel);
		}
		bool CheckNameGenerated() {
			if (!Table.HasHeadersRow)
				return false;
			ICell cell = Table.Range.GetCellRelative(currentPosition, 0) as ICell;
			if (cell != null && !cell.Value.IsEmpty)
				return cell.HasFormula || !cell.Value.IsText;
			return true;
		}
		#endregion
		void RenameColumnCore(int position, string newName) {
			TableColumn column = Columns[position];
			string oldName = column.Name;
			renamedColumnNames.Add(oldName, newName);
			Worksheet sheet = Table.Worksheet;
			ChangeTableColumnNameHistoryItem historyItem = new ChangeTableColumnNameHistoryItem(column, oldName, newName);
			sheet.Workbook.History.Add(historyItem);
			historyItem.Execute();
			WorkbookDataContext context = sheet.DataContext;
			context.PushSetValueShouldAffectSharedFormula(false);
			try {
				column.SetHeadersCellValue();
			} finally {
				context.PopSetValueShouldAffectSharedFormula();
			}
		}
		string GetNewColumnNameByPostfix(string prefix, int postfix) {
			string result = GetColumnName(prefix, postfix);
			while (Columns.ContainsName(result))
				result = GetColumnName(prefix, ++postfix);
			return result;
		}
		void CalculateDefaultColumnName() {
			string defaultPrefix = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DefaultInitialTableColumnNamePrefix);
			int defaultPostfix = 1;
			newName = GetColumnName(defaultPrefix, defaultPostfix);
			while (ContainsColumnNameWithoutCurrentPosition())
				newName = GetColumnName(defaultPrefix, ++defaultPostfix);
		}
		bool ContainsColumnNameWithoutCurrentPosition() {
			if (cachedLeftNames.Contains(newName))
				return true;
			return TryGetRightConflictedColumnPosition() != -1;
		}
		string GetColumnName(string prefix, int postfix) {
			return String.Format("{0}{1}", prefix, postfix);
		}
		int TryGetRightConflictedColumnPosition() {
			int count = Columns.Count;
			for (int i = currentPosition + 1; i < count; i++)
				if (StringExtensions.CompareInvariantCultureIgnoreCase(Columns[i].Name, newName) == 0)
					return i;
			return -1;
		}
	}
	#endregion
	#region TableColumnsInsertCommand
	public class TableColumnsInsertCommand : ErrorHandledWorksheetCommand {
		#region Fields
		readonly Table table;
		readonly int startPosition;
		readonly int insertedColumnsCount;
		readonly InsertRangeCommand insertRangeCommand;
		#endregion
		public TableColumnsInsertCommand(Table table, int startPosition, int insertedColumnsCount, IErrorHandler errorHandler)
			: base(table.Worksheet, errorHandler) {
				Guard.ArgumentNotNull(errorHandler, "errorHandler");
			this.table = table;
			this.startPosition = startPosition;
			this.insertedColumnsCount = insertedColumnsCount;
			this.insertRangeCommand = new InsertRangeCommand(table.Worksheet, table.GetInsertingRange(startPosition, insertedColumnsCount), InsertCellMode.ShiftCellsRight, true, InsertCellsFormatMode.ClearFormat, ErrorHandler);
		}
		TableColumnInfoCollection Columns { get { return table.Columns; } }
		protected internal override bool Validate() {
			return insertRangeCommand.Validate();
		}
		protected internal override void ExecuteCore() {
			insertRangeCommand.ExecuteCore();
			TableColumnNamesCreateOperation operation = new TableColumnNamesCreateOperation();
			operation.Init(Columns, startPosition);
			for (int i = 0; i < insertedColumnsCount; i++) {
				int position = i + startPosition;
				string name = operation.GetNextName(Columns);
				TableColumn column = new TableColumn(table, name);
				table.InsertColumnCore(column, position);
				column.SetCellValues();
			}
			table.Range = table.Range.GetResized(0, 0, insertedColumnsCount, 0);
		}
	}
	#endregion
	#region TableColumnNamesCorrectedCommand
	public class TableColumnNamesCorrectedCommand : SpreadsheetModelCommand {
		readonly TableColumnNamesCorrectedOperationBase operation;
		public TableColumnNamesCorrectedCommand(TableColumnNamesCorrectedOperationBase operation)
			: base(operation.Table.Worksheet) {
			this.operation = operation;
		}
		protected internal override void ExecuteCore() {
			operation.ShouldCorrectColumnFormulas = true;
			operation.ShouldUpdateColumnCells = true;
			operation.Execute();
		}
	}
	#endregion
	#region TableColumnRenameCommand
	public class TableColumnRenameCommand : SpreadsheetModelCommand {
		readonly TableColumnRenameOperation operation;
		public TableColumnRenameCommand(TableColumn column, string newName)
			: base(column.Worksheet) {
			this.operation = new TableColumnRenameOperation(column, newName);
		}
		public Dictionary<string, string> RenamedColumnNames { get { return operation.RenamedColumnNames; } }
		protected internal override void ExecuteCore() {
			operation.Execute();
		}
	}
	#endregion
	#region TableConvertToRangeCommand
	public class TableConvertToRangeCommand : ErrorHandledWorksheetCommand {
		readonly Table table;
		readonly int index;
		public TableConvertToRangeCommand(Table table, int index, IErrorHandler errorHandler)
			: base(table.Worksheet, errorHandler) {
			this.table = table;
			this.index = index;
		}
		protected internal override void ExecuteCore() {
			if (!Worksheet.Tables.Contains(table))
				return;
			CellRange range = table.Range;
			CellPosition topLeft = range.TopLeft;
			CellPosition bottomRight = range.BottomRight;
			Worksheet.UnhideRows(range.TopRowIndex, range.BottomRowIndex);
			for (int i = topLeft.Column; i <= bottomRight.Column; i++)
				for (int j = topLeft.Row; j <= bottomRight.Row; j++) {
					ICell cell = range.Worksheet.TryGetCell(i, j) as ICell;
					CellPosition position = new CellPosition(i, j);
					if (cell == null) {
						CellFormat defaultFormat = (CellFormat)DocumentModel.Cache.CellFormatCache[DocumentModel.StyleSheet.DefaultCellFormatIndex];
						CellFormat format = TableStyleFormatBuilderFactory.ConvertToRangeBuilder.Build(table, position, defaultFormat);
						if (!defaultFormat.Equals(format)) {
							cell = range.Worksheet.GetCell(i, j) as ICell;
							if (cell != null)
								cell.ApplyFormat(format);
						}
					} else {
						CellFormat defaultFormat = cell.FormatInfo;
						CellFormat format = TableStyleFormatBuilderFactory.ConvertToRangeBuilder.Build(table, position, defaultFormat);
						if (!defaultFormat.Equals(format)) 
							cell.ApplyFormat(format);
					}
				}
			DocumentHistory history = DocumentModel.History;
			TableConvertedToRangeFormulaWalker walker = new TableConvertedToRangeFormulaWalker(table.Name, DataContext);
			walker.Walk(DocumentModel);
			Worksheet.Tables.RemoveAt(index);
		}
	}
	#endregion
	#region ChangeTableRangeCommandBase (abstract class)
	public abstract class ChangeTableRangeCommandBase : ErrorHandledWorksheetCommand {
		#region Fields
		readonly Table table;
		#endregion
		protected ChangeTableRangeCommandBase(Table table, IErrorHandler errorHandler)
			: base(table.Worksheet, errorHandler) {
			this.table = table;
		}
		#region Properties
		protected Table Table { get { return table; } }
		protected CellRange TableRange { get { return table.Range; } }
		protected bool TableHasAutoFilter { get { return table.HasAutoFilter; } }
		protected bool TableHasHeadersRow { get { return table.HasHeadersRow; } }
		protected bool TableHasTotalsRow { get { return table.HasTotalsRow; } }
		protected TableColumnInfoCollection TableColumns { get { return Table.Columns; } }
		protected CellRange TargetRange { get; set; }
		#endregion
		protected IModelErrorInfo CanAddRows(int index, int count) {
			if (index < 0 || index > table.RowCount)
				return new ModelErrorInfo(ModelErrorType.ErrorTableRowIndexOutside);
			if (TableHasHeadersRow && index == 0)
				return new ModelErrorInfo(ModelErrorType.ErrorInsertAboveTableHeaderRow);
			if (TableHasTotalsRow && index == table.RowCount)
				return new ModelErrorInfo(ModelErrorType.ErrorInsertBelowTableTotalRow);
			TargetRange = TableRange.GetSubRowRange(index, index);
			if (index > table.RowCount)
				TargetRange.Resize(0, -1, 0, -1);
			if (count > 1)
				TargetRange.Resize(0, 0, 0, count - 1);
			return DocumentModel.CanRangeInsert(TargetRange, InsertCellMode.ShiftCellsDown, InsertCellsFormatMode.ClearFormat, NotificationChecks.All);
		}
		protected void AddWorksheetRows() {
			Worksheet.InsertRange(TargetRange, InsertCellMode.ShiftCellsDown, InsertCellsFormatMode.ClearFormat, false, ErrorHandler);
		}
		protected void SetTableRange(CellRange range) {
			table.Range = range;
		}
		protected void SetAutoFilterRange(CellRange range) {
			table.AutoFilter.Range = range;
		}
		protected CellRange GetAutoFilterRange() {
			return TableHasTotalsRow ? TableRange.GetResized(0, 0, 0, -1) : TableRange.Clone();
		}
		protected virtual void ChangeAutoFilterRange() {
			if (TableHasAutoFilter)
				SetAutoFilterRange(GetAutoFilterRange());
		}
	}
	#endregion
	#region AddTableRowsCommand
	public class AddTableRowsCommand : ChangeTableRangeCommandBase { 
		#region Fields
		readonly int startIndex;
		readonly int count;
		#endregion
		public AddTableRowsCommand(Table table, int startIndex, int count, IErrorHandler errorHandler)
			: base(table, errorHandler) {
			this.startIndex = startIndex;
			this.count = count;
		}
		protected internal override bool Validate() {
			return HandleError(CanAddRows(startIndex, count));
		}
		protected internal override void ExecuteCore() {
			AddWorksheetRows();
			int endRow = Table.RowCount + count - TableRange.TopLeft.Row - 2;
			CellRange newRange = TableRange.GetSubRowRange(0, endRow);
			SetTableRange(newRange);
			ChangeAutoFilterRange();
		}
	} 
	#endregion
	#region ChangeTableHeadersTotalsCommandBase (abstract class)
	public abstract class ChangeTableHeadersTotalsCommandBase : ChangeTableRangeCommandBase {
		protected ChangeTableHeadersTotalsCommandBase(Table table, IErrorHandler errorHandler)
			: base(table, errorHandler) {
		}
		protected bool IsRangeEmpty(CellRange range) {
			if (range == null || Worksheet.Tables.ContainsItemsInRange(range, true))
				return false;
			return !range.HasData();
		}
		protected void AssignRowCellsValues() {
			int i = 0;
			foreach (ICell cell in TargetRange.GetAllCellsEnumerable()) {
				SetCellValue(i, cell);
				i++;
			}
		}
		protected abstract void SetCellValue(int columnIndex, ICell cell);
	}
	#endregion
	#region ChangeTableHeadersCommand
	public class ChangeTableHeadersCommand : ChangeTableHeadersTotalsCommandBase {
		#region Fields
		readonly bool hasHeadersRow;
		readonly bool hasAutoFilter;
		readonly bool showAutoFilterButton;
		#endregion
		public ChangeTableHeadersCommand(Table table, bool hasHeadersRow, bool hasAutoFilter, bool showAutoFilterButton, IErrorHandler errorHandler)
			: base(table, errorHandler) {
			this.hasHeadersRow = hasHeadersRow;
			this.hasAutoFilter = hasAutoFilter;
			this.showAutoFilterButton = showAutoFilterButton;
		}
		#region Properties
		bool TableShowAutoFilterButton { get { return Table.ShowAutoFilterButton; } }
		bool DifferencesHasHeadersRow { get; set; }
		bool ShouldClearHeadersRow { get; set; }
		#endregion
		protected internal override bool Validate() {
			DifferencesHasHeadersRow = hasHeadersRow != TableHasHeadersRow;
			bool validateDifferencesAutoFilterOptions = TableHasAutoFilter != hasAutoFilter || TableShowAutoFilterButton != showAutoFilterButton;
			bool validateClearAutoFilterOptions = !hasAutoFilter && !showAutoFilterButton;
			if (DifferencesHasHeadersRow) {
				if (hasHeadersRow)
					return validateDifferencesAutoFilterOptions && ValidateCreateHeaders();
				return validateClearAutoFilterOptions && ValidateRemoveHeaders();
			}
			return hasHeadersRow ? validateDifferencesAutoFilterOptions : validateClearAutoFilterOptions;
		}
		protected internal override void ExecuteCore() {
			ChangeHeadersRow();
			ChangeAutoFilterRange();
			ChangeShowAutoFilterButton();
		}
		protected override void SetCellValue(int columnIndex, ICell cell) {
			cell.Value = TableColumns[columnIndex].Name;
		}
		#region Internal
		bool ValidateCreateHeaders() {
			TargetRange = TableRange.GetSubRowRange(-1, -1);
			ShouldClearHeadersRow = IsRangeEmpty(TargetRange);
			if (ShouldClearHeadersRow)
				return true;
			TargetRange = TableRange.GetSubRowRange(0, 0);
			IModelErrorInfo error = DocumentModel.CanRangeInsert(TargetRange, InsertCellMode.ShiftCellsDown, InsertCellsFormatMode.ClearFormat, NotificationChecks.All);
			return HandleError(error);
		}
		bool ValidateRemoveHeaders() {
			TargetRange = TableRange.GetSubRowRange(0, 0);
			if (TargetRange == null)
				return false;
			IModelErrorInfo error = DocumentModel.CanRangeRemove(TargetRange, RemoveCellMode.Default, NotificationChecks.ArrayFormula);
			return HandleError(error);
		}
		void ChangeHeadersRow() {
			if (!DifferencesHasHeadersRow)
				return;
			if (hasHeadersRow)
				CreateHeaders();
			else
				RemoveHeaders();
			ChangeHasHeadersRow();
		}
		void CreateHeaders() {
			InsertHeadersRowRange();
			SetTableRange(TableRange.GetResized(0, -1, 0, 0));
			AssignRowCellsValues();
		}
		void RemoveHeaders() {
			SetTableRange(TableRange.GetResized(0, 1, 0, 0));
			RemoveHeadersRowRange();
		}
		void InsertHeadersRowRange() {
			if (!ShouldClearHeadersRow)
				AddWorksheetRows();
		}
		void RemoveHeadersRowRange() {
			Worksheet.RemoveRange(TargetRange, RemoveCellMode.Default, true, true, ErrorHandler);
		}
		void ChangeHasHeadersRow() {
			Table.SetHasHeadersRow(hasHeadersRow);
		}
		protected override void ChangeAutoFilterRange() {
			SetAutoFilterRange(hasAutoFilter ? GetAutoFilterRange() : null);
		}
		void ChangeShowAutoFilterButton() {
			AutoFilterColumnCollection filterColumns = Table.AutoFilter.FilterColumns;
			foreach (AutoFilterColumn item in filterColumns)
				item.HiddenAutoFilterButton = !showAutoFilterButton;
		}
		#endregion
	}
	#endregion
	#region ChangeTableTotalsCommand
	public class ChangeTableTotalsCommand : ChangeTableHeadersTotalsCommandBase {
		readonly bool hasTotalsRow;
		public ChangeTableTotalsCommand(Table table, bool hasTotalsRow, IErrorHandler errorHandler)
			: base(table, errorHandler) {
			this.hasTotalsRow = hasTotalsRow;
		}
		protected internal override bool Validate() {
			if (hasTotalsRow == TableHasTotalsRow)
				return false;
			return hasTotalsRow ? ValidateCreateTotals() : ValidateRemoveTotals();
		}
		protected internal override void ExecuteCore() {
			if (hasTotalsRow)
				CreateTotals();
			else
				RemoveTotals(TargetRange);
			ChangeAutoFilterRange();
		}
		protected override void SetCellValue(int columnIndex, ICell cell) {
			TableColumns[columnIndex].SetTotalsCellValue(cell);
		}
		bool ValidateCreateTotals() {
			int targetRowIndex = TableRange.Height;
			TargetRange = TableRange.GetSubRowRange(targetRowIndex, targetRowIndex); 
			if (TargetRange == null)
				return HandleError(new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray)); 
			if (CanAddRows(targetRowIndex, 1) == null)
				return true;
			return IsRangeEmpty(TargetRange) ? true : HandleError(new ModelErrorInfo(ModelErrorType.TableOverlap));
		}
		bool ValidateRemoveTotals() {
			TargetRange = Table.TryGetTotalsRowRange();
			return TargetRange != null; 
		}
		void CreateTotals() {
			AddWorksheetRows();
			SetTableRange(TableRange.GetResized(0, 0, 0, 1));
			AssignRowCellsValues();
			Table.SetHasTotalsRow(true);
		}
		void RemoveTotals(CellRange totalsRow) {
			bool canShiftUp = DocumentModel.CanRangeRemove(totalsRow, RemoveCellMode.ShiftCellsUp, NotificationChecks.All & ~NotificationChecks.ProtectedCells) == null;
			RemoveCellMode mode = canShiftUp ? RemoveCellMode.ShiftCellsUp : RemoveCellMode.Default;
			RemoveRangeNotificationContext context = RemoveRangeNotificationContext.Create(mode, true, false, false);
			context.Range = totalsRow;
			DocumentModel.OnSingleRangeRemoving(context);
			DocumentModel.OnAfterRangeRemoving(Worksheet.SheetId, totalsRow, mode);
		}
	}
	#endregion
}
