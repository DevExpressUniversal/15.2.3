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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Spreadsheet;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region Events
	public class RowColumnVisibilityChangedEventArgs : EventArgs {
		public RowColumnVisibilityChangedEventArgs(int startIndex, int endIndex, bool hide)
			: base() {
			StartIndex = startIndex;
			EndIndex = endIndex;
			Hide = hide;
		}
		public int StartIndex { get; private set; }
		public int EndIndex { get; private set; }
		public bool Hide { get; private set; }
	}
	#endregion
	#region Worksheet
	public partial class Worksheet : InternalSheetBase, IWorksheet, IDocumentModelPartWithApplyChanges {
		#region Fields
#if BTREE
		RowCollectionBtree rows;
		CellTree cells;
#else
		RowCollection rows;
#endif
		ColumnCollection columns;
		MergedCellCollection mergedCells;
		ModelHyperlinkCollection hyperlinks;
		DefinedNameCollection definedNames;
		TableCollection tables;
		PivotTableCollection pivotTables;
		SharedFormulaCollection sharedFormulaList;
		ArrayFormulaRangesCollection arrayFormulaRangesList;
		PageBreakCollection columnBreaks;
		PageBreakCollection rowBreaks;
		CommentCollection comments;
		ModelProtectedRangeCollection protectedRanges;
		DataValidationCollection dataValidations;
		ConditionalFormattingCollection conditionalFormattings;
		SparklineGroupCollection sparklineGroups;
		IgnoredErrorCollection ignoredErrors;
		SheetViewSelection selection;
		PivotSelection pivotSelection;
		SheetViewSelection referenceEditSelection;
		CellValueCache cellValueCache;
		SheetAutoFilter autoFilter;
		VmlDrawing vmlDrawing; 
		int maxRowCount;
		int maxColumnCount;
		WebRanges webRanges;
		CellTagsCache cellTags;
		GroupCache rowGroupCache;
		GroupCache columnGroupCache;
		int cachedRowIndex = -1;
		int cachedColumnIndex = -1;
		int cachedColumnCollectionIndex = -1;
		int cachedRowCollectionIndex = -1;
		int cachedContentVersion = -1;
		Row cachedRow;
		ICell cachedCell;
		HashSet<CellKey> invalidDataCircles;
		#endregion
		public event EventHandler<RowColumnVisibilityChangedEventArgs> ColumnVisibilityChanged;
		public event EventHandler<RowColumnVisibilityChangedEventArgs> RowVisibilityChanged;
		public Worksheet(DocumentModel workbook)
			: base(workbook) {
			Initialize();
		}
		protected virtual void Initialize() {
			SetProperties(new WorksheetProperties(this));
#if BTREE
			this.rows = new RowCollectionBtree(this);
#else
			this.rows = new RowCollection(this);
#endif
			this.columns = new ColumnCollection(this);
			this.mergedCells = new MergedCellCollection(this);
			this.hyperlinks = new ModelHyperlinkCollection(this);
			this.definedNames = new DefinedNameCollection(this);
			this.tables = new TableCollection(this);
			this.pivotTables = new PivotTableCollection(this);
			this.sharedFormulaList = new SharedFormulaCollection(this);
			this.arrayFormulaRangesList = new ArrayFormulaRangesCollection(this);
			this.columnBreaks = new PageBreakCollection(this);
			this.rowBreaks = new PageBreakCollection(this);
			this.comments = new CommentCollection(this);
			this.protectedRanges = new ModelProtectedRangeCollection();
			this.dataValidations = new DataValidationCollection(Workbook);
			this.conditionalFormattings = new ConditionalFormattingCollection(this);
			this.sparklineGroups = new SparklineGroupCollection(this);
			this.ignoredErrors = new IgnoredErrorCollection(this);
			this.vmlDrawing = new VmlDrawing();
			this.selection = new SheetViewSelection(this);
			this.pivotSelection = new PivotSelection(this);
			this.referenceEditSelection = new SheetViewSelection(this);
			this.cellValueCache = new CellValueCache();
			this.autoFilter = new SheetAutoFilter(this);
			this.maxRowCount = IndicesChecker.MaxRowCount;
			this.maxColumnCount = IndicesChecker.MaxColumnCount;
			this.NeedColumnUnhideNotificated = true;
			this.NeedRowUnhideNotificated = true;
			this.invalidDataCircles = new HashSet<CellKey>();
#if BTREE
			this.cells = new CellTree();
#endif
			SubscribeEvents();
		}
		public Worksheet(DocumentModel workbook, string name)
			: this(workbook) {
			SetName(name);
		}
		#region Properties
#if BTREE
		public CellTree Cells { get { return cells; } }
#endif
		public new WorksheetProperties Properties { get { return (WorksheetProperties)base.Properties; } }
		public virtual IRowCollection Rows { get { return this.rows; } }
		public ColumnCollection Columns { get { return columns; } }
		public MergedCellCollection MergedCells { get { return this.mergedCells; } }
		public ModelHyperlinkCollection Hyperlinks { get { return hyperlinks; } }
		public virtual DefinedNameCollection DefinedNames { get { return definedNames; } }
		public WorkbookDataContext DataContext { get { return Workbook.DataContext; } }
		public TableCollection Tables { get { return tables; } }
		public PivotTableCollection PivotTables { get { return pivotTables; } }
		public virtual SharedFormulaCollection SharedFormulas { get { return sharedFormulaList; } }
		public virtual ArrayFormulaRangesCollection ArrayFormulaRanges { get { return arrayFormulaRangesList; } }
		public new DocumentModel Workbook { get { return (DocumentModel)base.Workbook; } }
		public virtual Margins Margins { get { return Properties.Margins; } }
		public virtual PrintSetup PrintSetup { get { return Properties.PrintSetup; } }
		public virtual HeaderFooterOptions HeaderFooter { get { return Properties.HeaderFooter; } }
		public PageBreakCollection ColumnBreaks { get { return columnBreaks; } }
		public PageBreakCollection RowBreaks { get { return rowBreaks; } }
		public CommentCollection Comments { get { return comments; } protected set { comments = value; } }
		public ModelProtectedRangeCollection ProtectedRanges { get { return protectedRanges; } }
		public DataValidationCollection DataValidations { get { return dataValidations; } }
		public ConditionalFormattingCollection ConditionalFormattings { get { return conditionalFormattings; } }
		public SparklineGroupCollection SparklineGroups { get { return sparklineGroups; } }
		public IgnoredErrorCollection IgnoredErrors { get { return ignoredErrors; } }
		public virtual VmlDrawing VmlDrawing { get { return vmlDrawing; } set { vmlDrawing = value; } }
		public virtual SheetViewSelection Selection { get { return selection; } }
		public virtual PivotSelection PivotSelection { get { return pivotSelection; } }
		public virtual SheetViewSelection ReferenceEditSelection { get { return referenceEditSelection; } }
		public WebRanges WebRanges {
			get {
				if (webRanges == null)
					webRanges = new WebRanges(this);
				return webRanges;
			}
			set { webRanges = value; }
		}
		public virtual CellTagsCache CellTags {
			get {
				if (cellTags == null)
					cellTags = new CellTagsCache(this);
				return cellTags;
			}
		}
		internal GroupCache RowGroupCache { get { return rowGroupCache; } set { rowGroupCache = value; } }
		internal GroupCache ColumnGroupCache { get { return columnGroupCache; } set { columnGroupCache = value; } }
		public virtual SharedStringTable SharedStringTable { get { return Workbook.SharedStringTable; } }
		public override SheetType SheetType { get { return SheetType.RegularWorksheet; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043")]
		public ICell this[CellPosition position] {
			[System.Diagnostics.DebuggerStepThrough]
			get { return this[position.Column, position.Row]; }
		}
		public ICell this[string position] {
			[System.Diagnostics.DebuggerStepThrough]
			get { return this[CellReferenceParser.Parse(position)]; }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		public ICell this[int column, int row] {
			get {
				DocumentModel documentModel = Workbook;
				documentModel.BeginUpdate();
				try {
					return GetCellOrCreate(column, row);
				}
				finally {
					documentModel.EndUpdate();
				}
			}
		}
		public virtual CellValueCache CellValueCache { get { return cellValueCache; } }
		public SheetAutoFilter AutoFilter { get { return autoFilter; } }
		public override bool IsSelected { get { return ActiveView.TabSelected; } set { ActiveView.TabSelected = value; } }
		public virtual int MaxRowCount {
			get { return maxRowCount; }
			set {
				if (value <= IndicesChecker.MaxRowCount && value > 0)
					maxRowCount = value;
			}
		}
		public virtual int MaxColumnCount {
			get { return maxColumnCount; }
			set {
				if (value <= IndicesChecker.MaxColumnCount && value > 0)
					maxColumnCount = value;
			}
		}
		public bool ShowRowHeaders { get { return Workbook.ViewOptions.ShowRowHeaders && ActiveView.ShowRowColumnHeaders; } }
		public bool ShowColumnHeaders { get { return Workbook.ViewOptions.ShowColumnHeaders && ActiveView.ShowRowColumnHeaders; } }
		public object Tag { get; set; }
		internal bool NeedColumnUnhideNotificated { get; set; }
		internal bool NeedRowUnhideNotificated { get; set; }
		public HashSet<CellKey> InvalidDataCircles { get { return invalidDataCircles; } }
		public bool ShowInvalidDataCircles { get { return invalidDataCircles.Count > 0; } }
		public PivotTableStaticInfo PivotTableStaticInfo { get; set; }
		#endregion
		#region IWorksheet Members
		DefinedNameCollectionBase IWorksheet.DefinedNames { get { return DefinedNames; } }
		public IModelWorkbook ExternalWorkbook { get { return base.Workbook; } }
		IRowCollectionBase ICellTable.Rows { get { return Rows; } }
		ITableCollection IWorksheet.Tables { get { return Tables; } }
		public virtual bool IsDataSheet { get { return false; } }
		public virtual ICell TryGetCell(int column, int row) {
			return GetRegisteredCell(column, row);
		}
		ICellBase ICellTable.TryGetCell(int column, int row) {
			return TryGetCell(column, row);
		}
		ICell GetCell(int column, int row) {
			return this[column, row];
		}
		ICellBase ICellTable.GetCell(int column, int row) {
			return GetCell(column, row);
		}
		VariantValue ICellTable.GetCalculatedCellValue(int column, int row) {
			ICell cell = TryGetCell(column, row);
			if (cell == null)
				return VariantValue.Empty;
			VariantValue result;
			Workbook.CalculationChain.TryGetCalculatedValue(cell, out result);
			return result;
		}
		public virtual ICell GetCellForFormatting(int column, int row) {
			ICell result = TryGetCell(column, row);
			if (result == null)
				return new FakeCell(new CellPosition(column, row), this);
			else
				return result;
		}
		public ICell GetCellForFormatting(CellPosition at) {
			return GetCellForFormatting(at.Column, at.Row);
		}
		#endregion
		#region IDocumentModelPartWithApplyChanges
		IDocumentModel IDocumentModelPart.DocumentModel { get { return this.Workbook; } }
		DocumentModel IDocumentModelPartWithApplyChanges.Workbook { get { return this.Workbook; } }
		void IDocumentModelPartWithApplyChanges.ApplyChanges(DocumentModelChangeActions actions) {
			this.ApplyChanges(actions);
		}
		#endregion
		#region SubscribeEvents
		protected internal virtual void SubscribeEvents() {
			DrawingObjects.DrawingRemoved += OnDrawingRemoved;
		}
		#endregion
		#region UnsubscribeEvents
		protected internal virtual void UnsubscribeEvents() {
			DrawingObjects.DrawingRemoved -= OnDrawingRemoved;
		}
		#endregion
		#region AddPrintRange
		public void AddPrintRange(CellRangeBase range) {
			Guard.ArgumentNotNull(range, "range");
			Workbook.BeginUpdate();
			try {
				if (!PrintAreaCalculator.PrintRangeIsValid(this, false))
					ClearPrintRange();
				AddPrintRangeCore(range);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		protected internal void AddPrintRangeCore(CellRangeBase range) {
			DefinedNameBase printRange;
			if (DefinedNames.TryGetItemByName(PrintAreaCalculator.PrintAreaDefinedName, out printRange)) {
				CellRangeBase oldRange = ((DefinedName)printRange).GetReferencedRange();
				if (oldRange != null)
					range = range.MergeWithRange(oldRange);
				range = range.GetWithModifiedPositionType(PositionType.Absolute);
				printRange.Expression = GenerateRangeExpression(range);
			}
			else {
				range = range.GetWithModifiedPositionType(PositionType.Absolute);
				CreateDefinedName(PrintAreaCalculator.PrintAreaDefinedName, GenerateRangeExpression(range));
			}
		}
		ParsedExpression GenerateRangeExpression(CellRangeBase range) {
			ParsedExpression result = new ParsedExpression();
			BasicExpressionCreator.CreateCellRangeExpression(
				result,
				range,
				BasicExpressionCreatorParameter.ShouldCreate3d | BasicExpressionCreatorParameter.ShouldCreateRelative,
				OperandDataType.Reference,
				DataContext);
			return result;
		}
		#endregion
		#region ClearPrintRange
		public void ClearPrintRange() {
			RemoveDefinedName(PrintAreaCalculator.PrintAreaDefinedName);
		}
		#endregion
		#region SetPrintRange
		public void SetPrintRange(CellRangeBase range) {
			Guard.ArgumentNotNull(range, "range");
			Workbook.BeginUpdate();
			try {
				ClearPrintRange();
				AddPrintRangeCore(range);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		#endregion
		public SheetViewSelection GetActualSelection() {
			return Workbook.ReferenceEditMode ? ReferenceEditSelection : Selection;
		}
		public virtual CellRange GetPrintRange() {
			return new PrintRangeCalculator(this).CalculateWithoutDefindeName();
		}
		public virtual CellRangeBase GetPrintRangeUsingDefinedNameForPrinting() {
			bool returnFirstCellRangeForEmptyDocument = false;
			return new PrintAreaCalculator(this).CalculateUsingDefinedName(returnFirstCellRangeForEmptyDocument);
		}
		public virtual CellRangeBase GetPrintRangeUsingDefinedNameForLayout() {
			bool returnFirstCellRangeForEmptyDocument = true;
			return new PrintAreaCalculator(this).CalculateUsingDefinedName(returnFirstCellRangeForEmptyDocument);
		}
		public virtual CellRange GetDataRange() {
			return new DataRangeCalculator(this).CalculateDataRange();
		}
		public virtual CellRange GetUsedRange() {
			return GetUsedRange(false);
		}
		public CellRange GetUsedRange(bool includeEmptyCells) {
			UsedRangeCalculator calculator = new UsedRangeCalculator(this);
			calculator.IncludeEmptyCells = includeEmptyCells;
			return calculator.Calculate();
		}
		public CellPosition GetLastUsedCellPosition() {
			CellRange usedRange = GetUsedRange();
			return usedRange.BottomRight;
		}
		protected internal virtual Row CreateRowCore(int index) {
			return new Row(index, this);
		}
		protected internal virtual ICell CreateCellCore(long index) {
			return CreateCellCore(new CellPosition((int)(index % IndicesChecker.MaxColumnCount), (int)(index / IndicesChecker.MaxColumnCount)));
		}
		protected internal virtual ICell CreateCellCore(CellPosition position) {
			if (Workbook.SuppressCellCreation)
				Exceptions.ThrowInvalidOperationException("New cell creation is prohibited at this time.");
			Cell newCell = new Cell(position, this);
			if (!DataContext.ImportExportMode) {
				int defaultCellFormatIndex = Workbook.StyleSheet.DefaultCellFormatIndex;
				Row row = Rows.TryGetRow(position.Row);
				if (row != null && row.FormatIndex > defaultCellFormatIndex)
					newCell.SetCellFormatIndex(row.FormatIndex);
				else {
					IColumnRange columnRange = Columns.TryGetColumnRange(position.Column);
					if (columnRange != null && columnRange.FormatIndex > defaultCellFormatIndex)
						newCell.SetCellFormatIndex(columnRange.FormatIndex);
				}
			}
			return newCell;
		}
		#region Name
		void SetName(string name) {
			CheckValidName(name);
			this.SetNameCore(name);
		}
		protected internal override void Rename(string newName) {
			DocumentModelDeferredChanges deferredChanges = Workbook.DeferredChanges;
			if (deferredChanges != null && deferredChanges.IsSetContentMode) {
				RenameCore(newName);
				return;
			}
			string oldName = Name;
			SheetRenamingEventArgs args = new SheetRenamingEventArgs(oldName, newName);
			if (Workbook.RaiseSheetRenaming(args))
				return;
			newName = args.NewName;
			RenameCore(newName);
			Workbook.RaiseInnerSheetRenamed(oldName, newName);
			Workbook.RaiseSheetRenamed(oldName, newName);
			Workbook.RaiseSchemaChanged();
		}
		void RenameCore(string newName) {
			CheckValidName(newName);
			Workbook.SheetDefinitions.RenameSheet(Name, newName);
			base.Rename(newName);
		}
		protected internal void CheckValidName(string sheetName) {
			IWorksheetNameCreationService worksheetNameCreationService = Workbook.GetService<IWorksheetNameCreationService>();
			if (worksheetNameCreationService == null)
				return;
			string[] existingSheetNames = Workbook.Sheets.GetSheetNames();
			WorksheetNameError error = worksheetNameCreationService.VerifyName(sheetName, existingSheetNames);
			if (error == WorksheetNameError.None)
				return;
			else if (error == WorksheetNameError.Duplicate)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDuplicateSheetName);
			else if (error == WorksheetNameError.Blank)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorBlankSheetName);
			else if (error == WorksheetNameError.StartOrEndWithSingleQuote)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorSheetNameStartOrEndWithSingleQuote);
			else if (error == WorksheetNameError.ContainsProhibitedCharacters)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorSheetNameContainsNotAllowedCharacters);
			else if (error == WorksheetNameError.ExceedAllowedLength)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorSheetNameExceedAllowedLength);
		}
		protected internal void GenerateSheetName() {
			IWorksheetNameCreationService worksheetNameCreationService = Workbook.GetService<IWorksheetNameCreationService>();
			if (worksheetNameCreationService != null) {
				string[] existingSheetNames = this.Workbook.Sheets.GetSheetNames();
				SetNameCore(worksheetNameCreationService.GetNormalizedName(Name, existingSheetNames, false));
			}
		}
		#endregion
		#region Formulas
		internal virtual void PrepareFormulas() {
			foreach (Table table in Tables) {
				foreach (TableColumn column in table.Columns)
					column.ParseTemporarilySavedFormulaBodies();
			}
		}
		#region SharedFormula
		public virtual SharedFormula LocateFormulaToMultipleCells(CellRangeBase range, CellPosition hostCellPosition, string formula, bool tryCreateShared) {
			Guard.ArgumentNotNull(range, "range");
			System.Diagnostics.Debug.Assert(range.CellCount > 0);
			CheckingChangingPartOfArray(range);
			CheckingChangingMergedCell(range, XtraSpreadsheetStringId.Msg_ErrorAttemptToCreateSharedFormulaInMergedCells);
			if (PivotTables.ContainsItemsInRange(range, true))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged);
			Workbook.BeginUpdate();
			try {
				RemoveArrayFormulasFromCollectionInsideRange(range);
				ICell hostCell = this[hostCellPosition];
				SharedFormula sharedFormula = new SharedFormula(hostCell, formula, range.GetFirstInnerCellRange());
				ParsedExpression expression = sharedFormula.Expression;
				if (tryCreateShared && range.RangeType != CellRangeType.UnionRange) {
					CellRange cellRange = (CellRange)range;
					SharedFormulaValidateRPNVisitor validator = new SharedFormulaValidateRPNVisitor(cellRange);
					if (validator.Validate(expression)) {
						int sharedFormulaIndex = SharedFormulas.Add(sharedFormula);
						foreach (ICell cell in range.GetAllCellsEnumerable())
							CreateSharedFormulaRefTransacted(cell, sharedFormulaIndex, sharedFormula);
						return sharedFormula;
					}
				}
				SharedFormulaToNormalRPNVisitor converter = new SharedFormulaToNormalRPNVisitor(DataContext);
				foreach (ICell cell in range.GetAllCellsEnumerable()) {
					DataContext.PushCurrentCell(cell);
					try {
						ParsedExpression convertedExpression = converter.Process(expression.Clone());
						cell.TransactedSetFormula(new Formula(cell, convertedExpression));
					}
					catch {
						DataContext.PopCurrentCell();
					}
				}
				return null;
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		internal void CreateSharedFormulaRefTransacted(ICell cell, int sharedFormulaIndex, SharedFormula sharedFormula) {
			System.Diagnostics.Debug.Assert(sharedFormula.Range.ContainsCell(cell));
			SharedFormulaRef sharedFormulaRef = new SharedFormulaRef(cell, sharedFormulaIndex, sharedFormula);
			cell.SetFormulaCore(sharedFormulaRef);
			OnCellValueChanged(cell);
		}
		#endregion
		#endregion
		#region GetCell
#if BTREE
		public virtual ICell GetCellOrCreate(int columnIndex, int rowIndex) {
			long index = (long)rowIndex * (long)IndicesChecker.MaxColumnCount + columnIndex;
			return cells.GetOrCreate(index, CreateCellCore);
		}
		public Row GetRegisteredRowOrCreate(int rowIndex) {
			IRowCollection rows = Rows;
			if (cachedRow != null && cachedRow.Index == rowIndex) {
				return cachedRow;
			}
			Row result = rows[rowIndex];
				cachedRow = result;
				cachedColumnIndex = -1;
				cachedColumnCollectionIndex = -1;
				cachedCell = null;
				cachedRowIndex = rowIndex;
				cachedContentVersion = Workbook.ContentVersion;
				cachedRow = result;
			return result;
		}
#else
		public virtual ICell GetCellOrCreate(int columnIndex, int rowIndex) {
			Row row = GetRegisteredRowOrCreate(rowIndex);
			ICellCollection cells = row.Cells;
			if (cachedCell != null) {
				if (cachedColumnIndex == columnIndex)
					return cachedCell;
				if (cachedColumnCollectionIndex >= 0 && cachedColumnIndex + 1 == columnIndex) {
					if (cachedColumnCollectionIndex + 1 < cells.InnerList.Count) {
						cachedCell = cells.InnerList[cachedColumnCollectionIndex + 1];
						if (cachedCell.ColumnIndex == columnIndex) {
							cachedColumnCollectionIndex++;
							cachedColumnIndex = columnIndex;
							return cachedCell;
						}
					}
					else {
						ICell cell = cells.CreateNewCell(columnIndex);
						cachedColumnCollectionIndex++;
						cells.Insert(cachedColumnCollectionIndex, cell);
						cachedCell = cell;
						cachedColumnIndex = columnIndex;
						return cachedCell;
					}
				}
			}
			ICell newCell;
			int position = cells.TryGetCellIndex(columnIndex);
			if (position < 0) {
				position = ~position;
				ICell cell = cells.CreateNewCell(columnIndex);
				cells.Insert(position, cell);
				newCell = cell;
			}
			else
				newCell = cells.InnerList[position];
			cachedColumnIndex = columnIndex;
			cachedCell = newCell;
			cachedColumnCollectionIndex = position;
			return cachedCell;
		}
		public Row GetRegisteredRowOrCreate(int rowIndex) {
			IRowCollection rows = Rows;
			if (cachedRow != null) {
				if (cachedRowIndex == rowIndex)
					return cachedRow;
				if (cachedRowCollectionIndex >= 0 && cachedRowIndex + 1 == rowIndex) {
					if (cachedRowCollectionIndex + 1 < rows.InnerList.Count) {
						cachedRow = rows.InnerList[cachedRowCollectionIndex + 1];
						if (cachedRow.Index == rowIndex) {
							cachedRowCollectionIndex++;
							cachedRowIndex = rowIndex;
							cachedColumnIndex = -1;
							cachedColumnCollectionIndex = -1;
							return cachedRow;
						}
					}
				}
			}
			Row row = null;
			int position = rows.TryGetRowIndex(rowIndex);
			if (position < 0) {
				position = ~position;
				row = rows.CreateRow(rowIndex);
				rows.Insert(position, row);
			}
			else
				row = rows.InnerList[position];
			cachedColumnIndex = -1;
			cachedColumnCollectionIndex = -1;
			cachedCell = null;
			cachedRowIndex = rowIndex;
			cachedContentVersion = Workbook.ContentVersion;
			cachedRowCollectionIndex = position;
			rows.SetCachedData(cachedRowIndex, cachedRowCollectionIndex);
			cachedRow = row;
			return cachedRow;
		}
#endif
		public void ResetCachedData() {
			cachedContentVersion = -1;
			cachedRowIndex = -1;
			cachedColumnIndex = -1;
			cachedColumnCollectionIndex = -1;
			cachedRowCollectionIndex = -1;
		}
		public void SetCachedCell(ICell cell, int innerIndex) {
			if (cell.RowIndex != cachedRowIndex)
				ResetCachedData();
			else {
				cachedContentVersion = Workbook.ContentVersion;
				cachedCell = cell;
				cachedColumnIndex = cell.ColumnIndex;
				cachedRowIndex = cell.RowIndex;
				cachedColumnCollectionIndex = innerIndex;
			}
		}
		public virtual ICell GetRegisteredCell(int columnIndex, int rowIndex) {
			Row row = TryGetRegisteredRow(rowIndex);
			if (row == null)
				return null;
			if (cachedContentVersion == Workbook.ContentVersion) {
				if (cachedColumnIndex == columnIndex)
					return cachedCell;
				if (cachedColumnCollectionIndex >= 0 && cachedColumnIndex + 1 == columnIndex) {
					if (cachedColumnCollectionIndex + 1 < row.Cells.InnerList.Count) {
						cachedCell = row.Cells.InnerList[cachedColumnCollectionIndex + 1];
						if (cachedCell.ColumnIndex == columnIndex) {
							cachedColumnCollectionIndex++;
							cachedColumnIndex = columnIndex;
							return cachedCell;
						}
					}
					else
						return null;
				}
			}
			cachedColumnIndex = columnIndex;
			cachedColumnCollectionIndex = row.Cells.TryGetCellIndex(cachedColumnIndex);
			if (cachedColumnCollectionIndex < 0) {
				cachedCell = null;
				return null;
			}
			cachedCell = row.Cells.InnerList[cachedColumnCollectionIndex];
			return cachedCell;
		}
		public Row TryGetRegisteredRow(int rowIndex) {
			if (cachedRowIndex == rowIndex)
				return cachedRow;
			if (cachedRowCollectionIndex >= 0 && cachedRowIndex + 1 == rowIndex) {
				if (cachedRowCollectionIndex + 1 < Rows.InnerList.Count) {
					cachedRow = Rows.InnerList[cachedRowCollectionIndex + 1];
					if (cachedRow.Index == rowIndex) {
						cachedRowCollectionIndex++;
						cachedRowIndex = rowIndex;
						cachedColumnIndex = -1;
						cachedColumnCollectionIndex = -1;
						return cachedRow;
					}
				}
				else
					return null;
			}
			cachedRowIndex = rowIndex;
			cachedRowCollectionIndex = Rows.TryGetRowIndex(cachedRowIndex);
			if (cachedRowCollectionIndex < 0) {
				cachedRow = null;
				return null;
			}
			cachedRow = Rows.InnerList[cachedRowCollectionIndex];
			cachedColumnIndex = -1;
			cachedColumnCollectionIndex = -1;
			return cachedRow;
		}
		#endregion
		#region MergedCells
		public bool MergeCells(CellRangeBase cellRange, IErrorHandler errorHandler) {
			return MergeCells(cellRange, true, errorHandler);
		}
		public bool MergeCells(CellRangeBase cellRange, bool expandWithIntersectedMergedCells, IErrorHandler errorHandler) {
			return MergeCells(cellRange, expandWithIntersectedMergedCells, MergeCellsMode.Default, errorHandler);
		}
		public bool MergeCells(CellRangeBase cellRange, bool expandWithIntersectedMergedCells, MergeCellsMode mode, IErrorHandler errorHandler) {
			CellMergeCommand command = new CellMergeCommand(this, cellRange, expandWithIntersectedMergedCells, mode, errorHandler);
			return command.Execute();
		}
		public bool UnMergeCells(CellRangeBase cellRange, IErrorHandler errorHandler) {
			CellUnmergeCommand command = new CellUnmergeCommand(this, cellRange, errorHandler);
			return command.Execute();
		}
		protected internal void CheckingChangingMergedCell(CellRangeBase range, XtraSpreadsheetStringId messageStringId) {
			CellUnion union = range as CellUnion;
			if (union != null) {
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					if (this.MergedCells.GetMergedCellRangesIntersectsRange((CellRange)innerRange).Count != 0)
						SpreadsheetExceptions.ThrowInvalidOperationException(messageStringId);
			}
			else
				if (this.MergedCells.GetMergedCellRangesIntersectsRange((CellRange)range).Count != 0)
					SpreadsheetExceptions.ThrowInvalidOperationException(messageStringId);
		}
		#endregion
		#region Remove range
		public void RemoveRange(CellRangeBase removableRange, RemoveCellMode mode, IErrorHandler handler) {
			RemoveRange(removableRange, mode, false, false, handler);
		}
		public virtual void RemoveRange(CellRangeBase removableRange, RemoveCellMode mode, bool suppressTableChecks, bool clearFormat, IErrorHandler handler) {
			RemoveRangeCommand command = new RemoveRangeCommand(this, removableRange, mode, suppressTableChecks, clearFormat, handler);
			command.Execute();
		}
		protected internal void OnSingleRangeRemoving(RemoveRangeNotificationContext context) {
			Tables.OnBeforeRangeRemoving(context);
			Workbook.OnSingleRangeRemoving(context);
		}
		public void RemoveColumns(int startIndex, int count, IErrorHandler handler) {
			RemoveColumnsCommand command = new RemoveColumnsCommand(this, startIndex, count, handler);
			command.Execute();
			Workbook.RaiseColumnsRemoved(new ColumnsChangedEventArgs(this.Name, startIndex, count));
		}
		public void RemoveRows(int startIndex, int count, IErrorHandler handler) {
			RemoveRowsCommand command = new RemoveRowsCommand(this, startIndex, count, handler);
			command.Execute();
			Workbook.RaiseRowsRemoved(new RowsChangedEventArgs(this.Name, startIndex, count));
		}
		#endregion
		#region Insert range
		public void InsertRange(CellRangeBase cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode, IErrorHandler handler) {
			InsertRange(cellRange, mode, formatMode, false, handler);
		}
		protected internal virtual void InsertRange(CellRangeBase cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode, bool suppressTableChecks, IErrorHandler handler) {
			InsertRangeCommand command = new InsertRangeCommand(this, cellRange, mode, suppressTableChecks, formatMode, handler);
			command.Execute();
		}
		public void InsertColumns(int startIndex, int count, InsertCellsFormatMode formatMode, IErrorHandler handler) {
			CellIntervalRange columnRange = CellIntervalRange.CreateColumnInterval(this, startIndex, PositionType.Relative, startIndex + count - 1, PositionType.Relative);
			InsertRange(columnRange, InsertCellMode.ShiftCellsRight, formatMode, handler);
			Workbook.RaiseColumnsInserted(new ColumnsChangedEventArgs(this.Name, startIndex, count));
		}
		public void InsertRows(int startIndex, int count, InsertCellsFormatMode formatMode, IErrorHandler handler) {
			CellIntervalRange rowRange = CellIntervalRange.CreateRowInterval(this, startIndex, PositionType.Relative, startIndex + count - 1, PositionType.Relative);
			InsertRange(rowRange, InsertCellMode.ShiftCellsDown, formatMode, handler);
			Workbook.RaiseRowsInserted(new RowsChangedEventArgs(this.Name, startIndex, count));
		}
		#endregion
		protected CellRangeBase TryGetRange(string reference) {
			VariantValue rangeValue;
			try {
				this.Workbook.DataContext.PushCurrentWorksheet(this);
				ParsedExpression referenceExpression = this.Workbook.DataContext.ParseExpression(reference, OperandDataType.None, false);
				if (referenceExpression == null)
					return null;
				rangeValue = referenceExpression.Evaluate(this.Workbook.DataContext);
			}
			finally {
				this.Workbook.DataContext.PopCurrentWorksheet();
			}
			if (rangeValue == VariantValue.Empty || !rangeValue.IsCellRange)
				return null;
			return rangeValue.CellRangeValue;
		}
		#region Clear
		public virtual void Clear() {
			this.sharedFormulaList.Clear();
			this.ClearArrayFormulas();
			this.tables.Clear();
			this.pivotTables.Clear();
			this.rows.Clear();
			this.definedNames.ClearCore();
			this.mergedCells.Clear();
			this.columns.Clear();
			ClearDrawingObjectsCollection();
			this.ClearConditionalFormattings();
			this.sparklineGroups.Clear();
			this.ignoredErrors.Clear();
			this.invalidDataCircles.Clear();
		}
		public void ClearAll(CellRangeBase range, IErrorHandler handler) {
			this.ClearAll(range, ModelPasteSpecialFlags.All, handler, true, false);
		}
		public void ClearAll(CellRangeBase range, ModelPasteSpecialFlags flags, IErrorHandler handler, bool shouldClearAutoFilter, bool removeHyperlinksIntersectedClearedRange) {
			this.ClearAll(range, flags, handler, shouldClearAutoFilter, removeHyperlinksIntersectedClearedRange, true);
		}
		public void ClearAll(CellRangeBase range, ModelPasteSpecialFlags flags, IErrorHandler handler, bool shouldClearAutoFilter, bool removeHyperlinksIntersectedClearedRange, bool shouldClearCellNoShift) {
			Workbook.BeginUpdate();
			try {
				Worksheet emptySourceWorksheet = new Worksheet(this.Workbook);
				foreach (CellRange targetRangeToClear in range.GetAreasEnumerable()) {
					CellRange sourceRange = targetRangeToClear.GetFirstInnerCellRange().Clone(emptySourceWorksheet) as CellRange;
					var ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(sourceRange, targetRangeToClear);
					var copyOp = new Model.CopyOperation.RangeCopyOperation(ranges, flags);
					copyOp.ErrorHandler = handler;
					copyOp.ShouldSkipFilteredRows = true;
					copyOp.RemoveHyperlinksIntersectedClearedRange = removeHyperlinksIntersectedClearedRange;
					copyOp.TableColumnNamesCorrectedOperation = new TableColumnNamesClearOperation();
					copyOp.Execute();
					if (shouldClearCellNoShift) {
						CellRangeBase clearableCellsNoShiftRange = GetClearableCellsNoShiftRange(targetRangeToClear);
						if (clearableCellsNoShiftRange != null)
							ClearCellsNoShift(clearableCellsNoShiftRange);
					}
				}
				if (shouldClearAutoFilter)
					ClearAutoFilter(range);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		CellRangeBase GetClearableCellsNoShiftRange(CellRange range) {
			List<Table> tables = Tables.GetItems(range, true);
			int count = tables.Count;
			CellRangeBase result = range.Clone();
			for (int i = 0; i < count; i++) 
				result = result.ExcludeRange(tables[i].Range);
			return result;
		}
		public void ClearContents(CellRangeBase range, IErrorHandler handler) {
			RemoveRange(range, RemoveCellMode.Default, handler);
		}
		public void ClearFormats(CellRangeBase range) {
			ClearFormats(range, true);
		}
		public void ClearFormats(CellRangeBase range, bool checkFilteredRanges) {
			Workbook.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
					ClearFormatsCore(range, checkFilteredRanges);
					ClearConditionalFormattingsFromRange(range);
					ClearAutoFilter(range);
					IgnoredErrors.Clear(range);
				}
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		protected internal virtual void ClearFormatsCore(CellRangeBase range, bool checkFilteredRanges) {
			int defaultFormatIndex = Workbook.StyleSheet.DefaultCellFormatIndex;
			bool isProtected = IsProtected;
			List<CellRange> filteredRanges = checkFilteredRanges ? GetFilteredRanges(range) : null;
			foreach (ICellBase cellInfo in range.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (IsRowFiltered(cell.RowIndex, filteredRanges) && !IsRowVisible(cell.RowIndex))
					continue;
				bool savedLocked = cell.ActualProtection.Locked;
				cell.ApplyFormatIndex(defaultFormatIndex);
				if (isProtected && savedLocked != cell.ActualProtection.Locked) {
					cell.ApplyProtection = true;
					cell.Protection.Locked = savedLocked;
				}
			}
		}
		protected internal virtual void ClearComments(CellRangeBase range) {
			this.ClearComments((current) => range.ContainsCell(current.Reference.Column, current.Reference.Row));
		}
		protected internal virtual void ClearComments(Predicate<Comment> conditionToDelete) {
			Workbook.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
					int count = this.comments.Count;
					for (int commentIndex = count - 1; commentIndex >= 0; commentIndex--) {
						Comment current = comments[commentIndex];
						if (conditionToDelete(current))
							RemoveCommentAt(commentIndex);
					}
				}
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		public void ClearConditionalFormattingsFromRange(CellRangeBase range, Predicate<ConditionalFormatting> filter) {
			Workbook.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
					this.ConditionalFormattings.SubtractCellRange(range, filter);
					int count = ConditionalFormattings.Count;
					List<int> itemsToDelete = new List<int>(count);
					for (int cfIndex = 0; cfIndex < count; cfIndex++) {
						ConditionalFormatting cf = ConditionalFormattings[cfIndex];
						if (cf.CellRange == null)
							itemsToDelete.Add(cfIndex);
					}
					int itemsToDeleteCount = itemsToDelete.Count;
					for (int i = itemsToDeleteCount - 1; i >= 0; i--) {
						int conditionalFormattingIndexToDelete = itemsToDelete[i];
						RemoveConditionalFormattingAtWithHistoryAndNotification(conditionalFormattingIndexToDelete);
					}
				}
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		public void ClearConditionalFormattingsFromRange(CellRangeBase range) {
			this.ClearConditionalFormattingsFromRange(range, null);
		}
		void ClearAutoFilter(CellRangeBase clearRangeBase) {
			CellRange clearRange;
			if (clearRangeBase.RangeType == CellRangeType.UnionRange)
				clearRange = clearRangeBase.GetFirstInnerCellRange();
			else
				clearRange = clearRangeBase as CellRange;
			AutoFilter.OnRangeRemovingCore(clearRange, RemoveCellMode.Default);
		}
		#endregion
		#region Table
		public bool RemoveTable(Table table) {
			int index = this.tables.IndexOf(table);
			if (index < 0)
				return false;
			return RemoveTableAt(index);
		}
		public bool RemoveTableAt(int index) {
			TableRemoveCommand command = new TableRemoveCommand(this, index);
			command.Execute();
			return true;
		}
		public virtual Table GetTableByCellPosition(int columnIndex, int rowIndex) {
			return tables.TryGetItem(new CellPosition(columnIndex, rowIndex));
		}
		#endregion
		public virtual IEnumerable<ICellBase> GetExistingCells() {
			return new Enumerable<ICellBase>(new EntireWorksheetExistingCellRangeEnumerator(this));
		}
		internal void UpdateGridlineColor() {
			this.ActiveView.UpdateGridlineColor();
		}
		protected internal CellPosition CalculateVisibleTopLeftPosition(CellRange range) {
			System.Diagnostics.Debug.Assert(range.Worksheet == this);
			CellPosition topLeft = range.TopLeft;
			int left = topLeft.Column;
			int top = topLeft.Row;
			CellPosition bottomRight = range.BottomRight;
			int right = bottomRight.Column;
			int bottom = bottomRight.Row;
			int x = left;
			for (int i = left; i <= right; i++) {
				if (IsColumnVisible(i)) {
					x = i;
					break;
				}
			}
			int y = top;
			for (int i = top; i <= bottom; i++) {
				if (IsRowVisible(i)) {
					y = i;
					break;
				}
			}
			return new CellPosition(x, y);
		}
		protected internal CellPosition CalculateVisibleBottomRightPosition(CellRange range) {
			System.Diagnostics.Debug.Assert(range.Worksheet == this);
			CellPosition topLeft = range.TopLeft;
			int left = topLeft.Column;
			int top = topLeft.Row;
			CellPosition bottomRight = range.BottomRight;
			int right = bottomRight.Column;
			int bottom = bottomRight.Row;
			int x = right;
			for (int i = right; i >= left; i--) {
				if (IsColumnVisible(i)) {
					x = i;
					break;
				}
			}
			int y = bottom;
			for (int i = bottom; i >= top; i--) {
				if (IsRowVisible(i)) {
					y = i;
					break;
				}
			}
			return new CellPosition(x, y);
		}
		protected internal virtual void ApplyChanges(DocumentModelChangeActions actions) {
			Workbook.ApplyChanges(actions);
		}
		#region Hide/Unhide
		public virtual void Hide() {
			IsSelected = false;
			VisibleState = SheetVisibleState.Hidden;
		}
		protected internal void RaiseHideUnhideRows(int startIndex, int endIndex, bool hide) {
			if (RowVisibilityChanged != null)
				RowVisibilityChanged(this, new RowColumnVisibilityChangedEventArgs(startIndex, endIndex, hide));
		}
		protected internal void RaiseHideUnhideColumns(int startIndex, int endIndex, bool hide) {
			if (ColumnVisibilityChanged != null)
				ColumnVisibilityChanged(this, new RowColumnVisibilityChangedEventArgs(startIndex, endIndex, hide));
		}
		public IModelErrorInfo HideRows(int startIndex, int endIndex) {
			return HideRows(startIndex, endIndex, true);
		}
		public IModelErrorInfo HideRows(int startIndex, int endIndex, bool needUpdateOutline) {
			if (startIndex == 0 && endIndex == IndicesChecker.MaxRowIndex)
				return new ModelErrorInfo(ModelErrorType.ErrorAttemptToHideAllRows);
			HideRowsCommand command = new HideRowsCommand(this, startIndex, endIndex, needUpdateOutline);
			command.Execute();
			return null;
		}
		public void UnhideRows(int startIndex, int endIndex) {
			UnhideRows(startIndex, endIndex, true);
		}
		public void UnhideRows(int startIndex, int endIndex, bool needUpdateOutline) {
			UnhideRowsCommand command = new UnhideRowsCommand(this, startIndex, endIndex, needUpdateOutline);
			command.Execute();
		}
		public IModelErrorInfo HideColumns(int startIndex, int endIndex) {
			return HideColumns(startIndex, endIndex, true);
		}
		public IModelErrorInfo HideColumns(int startIndex, int endIndex, bool needUpdateOutline) {
			if (PerformHideOrUnhideAllColumns(startIndex, endIndex))
				return new ModelErrorInfo(ModelErrorType.ErrorAttemptToHideAllColumns);
			HideColumnsCommand command = new HideColumnsCommand(this, startIndex, endIndex, needUpdateOutline);
			command.Execute();
			return null;
		}
		bool PerformHideOrUnhideAllColumns(int startIndex, int endIndex) {
			return startIndex == 0 && endIndex == IndicesChecker.MaxColumnIndex;
		}
		public void UnhideColumns(int startIndex, int endIndex) {
			UnhideColumns(startIndex, endIndex, true);
		}
		public void UnhideColumns(int startIndex, int endIndex, bool needUpdateOutline) {
			if (PerformHideOrUnhideAllColumns(startIndex, endIndex))
				UnhideAllColumns(startIndex, endIndex, needUpdateOutline);
			else
				UnhideColumnsCore(startIndex, endIndex, needUpdateOutline);
		}
		void UnhideAllColumns(int startIndex, int endIndex, bool needUpdateOutline) {
			Workbook.BeginUpdate();
			try {
				UnhideRows(0, IndicesChecker.MaxRowIndex, needUpdateOutline);
				UnhideColumnsCore(startIndex, endIndex, needUpdateOutline);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		void UnhideColumnsCore(int startIndex, int endIndex, bool needUpdateOutline) {
			UnhideColumnsCommand command = new UnhideColumnsCommand(this, startIndex, endIndex, needUpdateOutline);
			command.Execute();
		}
		internal void UpdateColumnOutlineGroup(int firstIndex, int lastIndex, int outlineLevel, bool collapse) {
			GroupList groups = new ColumnGroupCollector(this).StrongCollectGroups(firstIndex, lastIndex);
			foreach (GroupItem group in groups.Groups)
				if (group.Level <= outlineLevel && (group.Start >= firstIndex && group.End <= lastIndex || !collapse))
					Columns.GetIsolatedColumn(group.ButtonPosition).IsCollapsed = collapse;
			columnGroupCache = null;
		}
		internal void UpdateRowOutlineGroup(int firstIndex, int lastIndex, int outlineLevel, bool collapse) {
			GroupList groups = new RowGroupCollector(this).StrongCollectGroups(firstIndex, lastIndex);
			foreach (GroupItem group in groups.Groups)
				if (group.Level <= outlineLevel && (group.Start >= firstIndex && group.End <= lastIndex || !collapse))
					Rows[group.ButtonPosition].IsCollapsed = collapse;
			rowGroupCache = null;
		}
		protected internal void SetVisibleStateAndValidateActiveSheet(SheetVisibleState value) {
			SheetVisibleState oldState = VisibleState;
			VisibleState = value;
			if (Object.ReferenceEquals(Workbook.ActiveSheet, this) && oldState == SheetVisibleState.Visible && oldState != value) {
				Worksheet nextSheet = Workbook.CalculateActualActiveSheet();
				if (nextSheet != null)
					Workbook.ActiveSheet = nextSheet;
			}
		}
		protected internal override void ChangeVisibleState(SheetVisibleState newValue) {
			DocumentModelDeferredChanges deferredChanges = Workbook.DeferredChanges;
			if (deferredChanges != null && deferredChanges.IsSetContentMode) {
				base.ChangeVisibleState(newValue);
				return;
			}
			SheetVisibleState oldValue = VisibleState;
			base.ChangeVisibleState(newValue);
			Workbook.RaiseSheetVisibleStateChanged(Name, oldValue, newValue);
		}
		#endregion
		#region Freeze/Unfreeze Panes
		protected internal void FreezePanes(CellPosition splitPosition) {
			FreezePanes(splitPosition, ModelWorksheetView.DefaultItem.TopLeftCell);
		}
		protected internal void FreezePanes(CellPosition splitPosition, CellPosition topLeftCell) {
			FreezePanes(splitPosition.Column, splitPosition.Row, topLeftCell);
		}
		protected internal void FreezePanes(int xSplit, int ySplit, CellPosition topLeftCell) {
			int splitLeftPosition = GetSplitLeftPosition(xSplit, topLeftCell);
			int splitTopPosition = GetSplitTopPosition(ySplit, topLeftCell);
			CellPosition splitTopLeftCell = new CellPosition(splitLeftPosition, splitTopPosition);
			FreezePanesCore(xSplit, ySplit, splitTopLeftCell, ViewPaneType.BottomRight, topLeftCell);
		}
		int GetSplitLeftPosition(int xSplit, CellPosition topLeftCell) {
			return xSplit + topLeftCell.Column;
		}
		int GetSplitTopPosition(int ySplit, CellPosition topLeftCell) {
			return ySplit + topLeftCell.Row;
		}
		protected internal void UnfreezePanes() {
			ModelWorksheetViewInfo item = ModelWorksheetView.DefaultItem;
			bool isFrozen = IsFrozen();
			bool needRaiseScrollPositionChanged = false;
			FreezePanesCore(item.HorizontalSplitPosition, item.VerticalSplitPosition, item.SplitTopLeftCell, item.ActivePaneType, item.SplitState, ActiveView.TopLeftCell, ref needRaiseScrollPositionChanged);
			if (isFrozen)
				Workbook.RaisePanesUnfrozen(Name);
			if (needRaiseScrollPositionChanged)
				RaiseScrollPositionChanged();
		}
		protected internal void FreezeRows(int ySplit) {
			FreezeRows(ySplit, ModelWorksheetView.DefaultItem.TopLeftCell);
		}
		protected internal void FreezeRows(int ySplit, CellPosition topLeftCell) {
			int splitTopPosition = GetSplitTopPosition(ySplit, topLeftCell);
			CellPosition splitTopLeftCell = new CellPosition(0, splitTopPosition);
			FreezePanesCore(ModelWorksheetView.DefaultItem.HorizontalSplitPosition, ySplit, splitTopLeftCell, ViewPaneType.BottomLeft, topLeftCell);
		}
		protected internal void FreezeColumns(int xSplit) {
			FreezeColumns(xSplit, ModelWorksheetView.DefaultItem.TopLeftCell);
		}
		protected internal void FreezeColumns(int xSplit, CellPosition topLeftCell) {
			int splitLeftPosition = GetSplitLeftPosition(xSplit, topLeftCell);
			CellPosition splitTopLeftCell = new CellPosition(splitLeftPosition, 0);
			FreezePanesCore(xSplit, ModelWorksheetView.DefaultItem.VerticalSplitPosition, splitTopLeftCell, ViewPaneType.TopRight, topLeftCell);
		}
		void FreezePanesCore(int xSplit, int ySplit, CellPosition splitTopLeftCell, ViewPaneType activePane, CellPosition topLeftCell) {
			bool needRaisePanesFrozenEvent = ActiveView.HorizontalSplitPosition != xSplit || ActiveView.VerticalSplitPosition != ySplit ||
				!ActiveView.TopLeftCell.EqualsPosition(topLeftCell);
			bool needRaiseScrollPositionChanged = false;
			FreezePanesCore(xSplit, ySplit, splitTopLeftCell, activePane, ViewSplitState.Frozen, topLeftCell, ref needRaiseScrollPositionChanged);
			if (needRaisePanesFrozenEvent)
				Workbook.RaisePanesFrozen(Name, ySplit - 1, xSplit - 1, topLeftCell);
			if (needRaiseScrollPositionChanged)
				RaiseScrollPositionChanged();
		}
		void FreezePanesCore(int xSplit, int ySplit, CellPosition splitTopLeftCell, ViewPaneType activePane, ViewSplitState state, CellPosition topLeftCell, ref bool needRaiseScrollPositionChanged) {
			ActiveView.BeginUpdate();
			try {
				CellPosition oldScrollPosition = ActiveView.ScrolledTopLeftCell;
				ActiveView.HorizontalSplitPosition = xSplit;
				ActiveView.VerticalSplitPosition = ySplit;
				ActiveView.SplitTopLeftCell = splitTopLeftCell;
				ActiveView.ActivePaneType = activePane;
				ActiveView.SplitState = state;
				ActiveView.TopLeftCell = topLeftCell;
				needRaiseScrollPositionChanged = !ActiveView.ScrolledTopLeftCell.EqualsPosition(oldScrollPosition);
			}
			finally {
				Workbook.ApplyChanges(DocumentModelChangeType.FreezePanes);
				ActiveView.EndUpdate();
			}
		}
		public bool IsFrozen() {
			return ActiveView.IsFrozen();
		}
		public bool IsOnlyColumnsFrozen() {
			return ActiveView.IsOnlyColumnsFrozen();
		}
		public bool IsOnlyRowsFrozen() {
			return ActiveView.IsOnlyRowsFrozen();
		}
		#endregion
		#region TabColor
		public virtual Color GetTabColor() {
			return Properties.GetTabColor();
		}
		public virtual void SetTabColor(Color color) {
			Color oldColor = GetTabColor();
			if (oldColor == color)
				return;
			Properties.SetTabColor(color);
			Workbook.RaiseSheetTabColorChanged(Name, oldColor, color);
		}
		#endregion
		#region AutoFit
		public virtual void TryBestFitColumn(int columnIndex, ColumnBestFitMode mode) {
			ColumnAutoFitCalculator calculator = new ColumnAutoFitCalculator(this);
			calculator.TryBestFitColumn(columnIndex, mode);
		}
		public virtual void TryBestFitColumn(ICell cell, ColumnBestFitMode mode) {
			ColumnAutoFitCalculator calculator = new ColumnAutoFitCalculator(this);
			calculator.TryBestFitColumn(cell, mode);
		}
		public virtual void TryBestFitColumn(CellRangeBase range, ColumnBestFitMode mode) {
			ColumnAutoFitCalculator calculator = new ColumnAutoFitCalculator(this);
			calculator.TryBestFitColumn(range, mode);
		}
		public virtual void TryBestFitRow(int rowIndex) {
			RowAutoFitCalculator calculator = new RowAutoFitCalculator(this);
			calculator.TryBestFitRow(rowIndex);
		}
		public virtual void TryBestFitRow(CellRangeBase range) {
			RowAutoFitCalculator calculator = new RowAutoFitCalculator(this);
			calculator.TryBestFitRow(range);
		}
		#endregion
		#region GetCurrentRange
		public virtual CellRange GetCurrentRegion(CellRangeBase rangeBase) {
			CellRange range = rangeBase.GetFirstInnerCellRange();
			CellRange result = new CellRange(this, range.TopLeft, range.TopLeft);
			bool haveRelative = true;
			while (haveRelative) {
				CellPosition pos = result.TopLeft;
				int leftColumn = pos.Column;
				int topRow = pos.Row;
				pos = result.BottomRight;
				int rightColumn = pos.Column;
				int botRow = pos.Row;
				while (haveRelative) {
					haveRelative = false;
					int newLeftColumn = LookupEmptyColumn(leftColumn, -1, topRow, botRow);
					if (newLeftColumn != leftColumn)
						haveRelative = true;
					leftColumn = newLeftColumn;
					int newTopRow = LookupEmptyRow(topRow, -1, leftColumn, rightColumn);
					if (newTopRow != topRow)
						haveRelative = true;
					topRow = newTopRow;
					int newRightColumn = LookupEmptyColumn(rightColumn, 1, topRow, botRow);
					if (newRightColumn != rightColumn)
						haveRelative = true;
					rightColumn = newRightColumn;
					int newBotRow = LookupEmptyRow(botRow, 1, leftColumn, rightColumn);
					if (newBotRow != botRow)
						haveRelative = true;
					botRow = newBotRow;
				}
				result = new CellRange(this, new CellPosition(leftColumn, topRow), new CellPosition(rightColumn, botRow));
				foreach (CellRange mergedRange in mergedCells.GetEVERYMergedRangeSLOWEnumerable()) {
					if (mergedRange.Intersects(result)) {
						ICellBase cell = mergedRange.TryGetCellRelative(0, 0);
						if (!result.ContainsRange(mergedRange) && cell != null && !cell.Value.IsEmpty)
							haveRelative = true;
						result = result.UnionWith(mergedRange).CellRangeValue as CellRange;
					}
					else
						if (ContactsOrIntersects(mergedRange, result)) {
							ICellBase cell = mergedRange.TryGetCellRelative(0, 0);
							if (cell == null || cell.Value.IsEmpty)
								continue;
							haveRelative = true;
							result = result.UnionWith(mergedRange).CellRangeValue as CellRange;
						}
				}
			}
			return result == null ? range : result;
		}
		bool ContactsOrIntersects(CellRangeBase range1, CellRangeBase range2) {
			if (Math.Max(range1.TopLeft.Column, range2.TopLeft.Column) <= (1 + Math.Min(range1.BottomRight.Column, range2.BottomRight.Column)) &&
				Math.Max(range1.TopLeft.Row, range2.TopLeft.Row) <= (1 + Math.Min(range1.BottomRight.Row, range2.BottomRight.Row)))
				return true;
			return false;
		}
		int LookupEmptyRow(int row, int direction, int startColumn, int endColumn) {
			if (startColumn > 0)
				--startColumn;
			if (endColumn < IndicesChecker.MaxColumnCount - 1)
				++endColumn;
			if ((direction >= 0 && row >= IndicesChecker.MaxRowCount - 1) || (direction <= 0 && row <= 0))
				return row;
			row += direction;
			for (int i = startColumn; i <= endColumn; ++i) {
				ICellBase cell = TryGetCell(i, row);
				if (cell != null && !cell.Value.IsEmpty)
					return row;
			}
			return row - direction;
		}
		int LookupEmptyColumn(int column, int direction, int startRow, int endRow) {
			if (startRow > 0)
				--startRow;
			if (endRow < IndicesChecker.MaxRowCount - 1)
				++endRow;
			if ((direction >= 0 && column >= IndicesChecker.MaxColumnCount - 1) || (direction <= 0 && column <= 0))
				return column;
			column += direction;
			for (int i = startRow; i <= endRow; ++i) {
				ICellBase cell = TryGetCell(column, i);
				if (cell != null && !cell.Value.IsEmpty)
					return column;
			}
			return column - direction;
		}
		#endregion
		#region ScrollToRow/ScrollToColumn
		public void ScrollToRow(int row) {
			int column = ActiveView.ScrolledTopLeftCell.Column;
			ScrollTo(row, column);
		}
		public void ScrollToColumn(int column) {
			int row = ActiveView.ScrolledTopLeftCell.Row;
			ScrollTo(row, column);
		}
		public void ScrollTo(int row, int column) {
			Workbook.BeginUpdate();
			try {
				if (object.ReferenceEquals(this, Workbook.ActiveSheet))
					Workbook.ApplyChanges(DocumentModelChangeActions.ResetAllLayout);
				SetScrollPosition(column, row);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		protected internal void SetScrollPosition(int column, int row) {
			CellPosition newPosition = new CellPosition(column, row);
			bool needRaiseEvent = !ActiveView.ScrolledTopLeftCell.EqualsPosition(newPosition);
			ActiveView.ScrolledTopLeftCell = newPosition;
			if (needRaiseEvent)
				RaiseScrollPositionChanged();
		}
		void RaiseScrollPositionChanged() {
			CellPosition actualPosition = ActiveView.ScrolledTopLeftCell;
			Workbook.RaiseScrollPositionChanged(Name, actualPosition.Column, actualPosition.Row);
		}
		#endregion
		public void ResetCachedContentVersions() {
			Columns.ResetTryGetColumnIndexCache();
			foreach (ICellBase info in GetExistingCells()) {
				ICell cell = info as ICell;
				if (cell != null)
					cell.ResetCachedContentVersions();
			}
		}
		public void ResetCachedTransactionVersions() {
			foreach (ICellBase info in GetExistingCells()) {
				ICell cell = info as ICell;
				if (cell != null)
					cell.ResetCachedTransactionVersions();
			}
			foreach (Row row in Rows)
				row.ResetCachedTransactionVersions();
		}
		public virtual bool CanEditSelection() {
			if (!Properties.Protection.SheetLocked)
				return true;
			return CheckSelectionAccess();
		}
		public virtual bool CheckSelectionAccess() {
			int count = Selection.SelectedRanges.Count;
			for (int i = 0; i < count; i++)
				if (!Workbook.CheckRangeAccess(Selection.SelectedRanges[i]))
					return false;
			return true;
		}
		#region CanEditCellContent
		public bool CanEditCellContent(int columnIndex, int rowIndex, bool checkAccessRights) {
			if (!Properties.Protection.SheetLocked)
				return true;
			CellPosition cellPosition = new CellPosition(columnIndex, rowIndex);
			return CanEditCellContentCore(cellPosition, checkAccessRights);
		}
		public bool CanEditCellContent(CellPosition cellPosition, bool checkAccessRights) {
			if (!Properties.Protection.SheetLocked)
				return true;
			return CanEditCellContentCore(cellPosition, checkAccessRights);
		}
		bool CanEditCellContentCore(CellPosition cellPosition, bool checkAccessRights) {
			DevExpress.XtraSpreadsheet.Model.ICell cell = TryGetCell(cellPosition.Column, cellPosition.Row);
			if (cell == null)
				cell = new FakeCell(cellPosition, this);
			if (!cell.ActualProtection.Locked)
				return true;
			if (!checkAccessRights)
				return false;
			return CheckCellAccess(cellPosition);
		}
		bool CheckCellAccess(CellPosition cellPosition) {
			IList<ModelProtectedRange> ranges = ProtectedRanges.LookupProtectedRanges(cellPosition);
			foreach (ModelProtectedRange range in ranges) {
				if (range.IsAccessGranted)
					return true;
			}
			return false;
		}
		#endregion
		protected override void ResetProtectedRanges() {
			ProtectedRanges.Reset();
		}
		public bool TryEditRangeContent(CellRange range, Action<CellRange> showUnprotectRangeForm, Action showReadonlyMessange) {
			if (ReadOnly)
				return false;
			if (!Properties.Protection.SheetLocked || !range.ContainsLockedCells())
				return true;
			IList<ModelProtectedRange> ranges = ProtectedRanges.LookupProtectedRangesContainingEntireRange(range);
			if (ranges.Count == 0) {
				if (showReadonlyMessange != null)
					showReadonlyMessange();
				return false;
			}
			foreach (ModelProtectedRange protectedRange in ranges)
				if (protectedRange.IsAccessGranted)
					return true;
			if (showUnprotectRangeForm != null)
				showUnprotectRangeForm(range);
			foreach (ModelProtectedRange protectedRange in ranges)
				if (protectedRange.IsAccessGranted)
					return true;
			return false;
		}
		#region ClearCellsNoShift
		public void ClearCellsNoShift(CellRangeBase range) {
			IList<CellPosition> removedPositions = GetExistingCellPositions(range);
			removedPositions.ForEach(ClearCellNoShift);
			if (removedPositions.Count > 0) {
				cachedCell = null;
				ResetCachedData();
			}
		}
		IList<CellPosition> GetExistingCellPositions(CellRangeBase range) {
			bool isProtected = IsProtected;
			List<CellPosition> result = new List<CellPosition>();
			IEnumerable<ICellBase> existingCells = range.GetExistingCellsEnumerable(false);
			foreach (ICellBase cellInfo in existingCells) {
				ICell cell = cellInfo as ICell;
				if (cell != null && (!isProtected || cell.FormatInfo.ActualProtection.Locked))
					result.Add(cell.Position);
			}
			return result;
		}
		void ClearCellNoShift(CellPosition position) {
			Row row = Rows[position.Row];
			row.Cells.RemoveAtColumnIndex(position.Column);
		}
		#endregion
		#region Tree
		public List<ITableBase> TryGetTableBases(CellRange range) {
			List<ITableBase> result = new List<ITableBase>();
			List<Table> intersectedTables = tables.GetItems(range, true);
			if (intersectedTables.Count > 0)
				foreach (ITableBase table in intersectedTables)
					result.Add(table);
			List<PivotTable> intersectedPivotTables = pivotTables.GetItems(range, true);
			foreach (ITableBase table in intersectedPivotTables)
				result.Add(table);
			return result;
		}
		public ITableBase TryGetTableBase(CellPosition position) {
			ITableBase table = Tables.TryGetItem(position);
			if(table == null)
				table = PivotTables.TryGetItem(position);
			return table;
		}
		public bool ContainsTableBase(CellRangeBase range) {
			return 
				tables.ContainsItemsInRange(range, true) ||
				pivotTables.ContainsItemsInRange(range, true);
		}
		public bool ContainsTableBases(CellRangeBase range) {
			return
				tables.ContainsItemsInRange(range, true) ||
				pivotTables.ContainsItemsInRange(range, true);
		}
		public Table TryGetTable(CellPosition position) {
			return Tables.TryGetItem(position);
		}
		public PivotTable TryGetPivotTable(CellPosition position) {
			return PivotTables.TryGetItem(position);
		}
		#endregion
		public bool RangeContainsNotEmptyCell(CellRange range) {
			CellRangeNonEmptyVisibleCellsEnumerator enumerator = new CellRangeNonEmptyVisibleCellsEnumerator(this, range.TopLeft, range.BottomRight);
			enumerator.Reset();
			return enumerator.MoveNext();
		}
		public bool IsEmptySheet() {
			CellRange usedRange = this.GetUsedRange();
			if(usedRange == null)
				return true;
			if(usedRange.CellCount > 1)
				return false;
			ICell cell = this.TryGetCell(usedRange.LeftColumnIndex, usedRange.TopRowIndex);
			return cell == null;
		}
		#region TableBase methods
		public ICollection<ITableBase> TryGetSelectedTableBases(bool orIntersects) {
			IList<CellRange> ranges = Selection.SelectedRanges;
			ICollection<ITableBase> result = new HashSet<ITableBase>();
			int rangesCount = ranges.Count;
			for (int i = 0; i < rangesCount; i++) {
				CellRange range = ranges[i];
				PopulateCollection(result, Tables.GetItems(range, orIntersects));
				PopulateCollection(result, PivotTables.GetItems(range, orIntersects));
			}
			return result;
		}
		void PopulateCollection<TTable>(ICollection<ITableBase> targetCollection, ICollection<TTable> sourceCollection) where TTable : ITableBase {
			foreach (TTable item in sourceCollection)
				targetCollection.Add(item);
		}
		public ITableBase TryGetActiveTableBase() {
			CellPosition activePosition = Selection.ActiveCell;
			ITableBase result = tables.TryGetItem(activePosition);
			if (result != null)
				return result;
			return pivotTables.TryGetItem(activePosition);
		}
		public void ApplyStyleToActiveTableBases(string styleName, bool orIntersects) {
			ICollection<ITableBase> selectedTables = TryGetSelectedTableBases(orIntersects);
			int count = selectedTables.Count;
			if (count == 0)
				return;
			TableStyleCollection styles = Workbook.StyleSheet.TableStyles;
			Workbook.BeginUpdate();
			try {
				foreach (ITableBase selectedTable in selectedTables)
					selectedTable.Style = styles[styleName];
			} finally {
				Workbook.EndUpdate();
			}
		}
		#endregion
	}
	#endregion
	#region PivotTableStaticInfo
	public class PivotTableStaticInfo {
		PivotTableAxis axis;
		int tableIndex;
		public PivotTableStaticInfo(int tableIndex, PivotTableAxis axis) {
			this.axis = axis;
			this.tableIndex = tableIndex;
			this.FieldReferenceIndex = -1;
		}
		#region Properties
		public int TableIndex { get { return tableIndex; } }
		public int FieldIndex { get; set; }
		public int FieldReferenceIndex { get; set; }
		public bool IsPageFilter { get { return axis == PivotTableAxis.Page; } }
		public bool IsRowFieldsFilter { get { return axis == PivotTableAxis.Row; } }
		public PivotTableAxis Axis { get { return axis; } }
		public List<int> FieldIndexList { get; set; }
		public bool IsContextMenuFieldGroupActive { get; set; }
		public PivotFilter LabelFilter { get; set; }
		public PivotFilter MeasureFilter { get; set; }
		public Point PointActivateFilter { get; set; }
		#endregion
	}
	#endregion
	#region CellValueCache
	public class CellValueCache {
		readonly Dictionary<ICell, string> inlineStrings = new Dictionary<ICell, string>();
		public void RegisterInlineString(ICell cell, string value) {
			inlineStrings.Add(cell, value);
		}
		public void UnregisterInlineString(ICell cell) {
			inlineStrings.Remove(cell);
		}
		public string GetInlineString(ICell cell) {
			return inlineStrings[cell];
		}
	}
	#endregion
}
