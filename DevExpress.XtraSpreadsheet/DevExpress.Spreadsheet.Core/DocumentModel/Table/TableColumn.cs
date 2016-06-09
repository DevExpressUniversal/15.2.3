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
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region TotalsRowFunctionType(enum)
	public enum TotalsRowFunctionType {
		None,
		Sum,
		Min,
		Max,
		Average,
		Count,
		CountNums,
		StdDev,
		Var,
		Custom,
	}
	#endregion
	#region TableColumnInfo
	public class TableColumn : MultiIndexObject<TableColumn, DocumentModelChangeActions>, ITableColumnInfoFormat {
		#region Static Members
		protected internal static int DefaultQueryTableFieldId = -1;
		readonly static TableColumnApplyFlagsIndexAccessor tableColumnApplyFlagsIndexAccessor = new TableColumnApplyFlagsIndexAccessor();
		readonly static IIndexAccessorBase<TableColumn, DocumentModelChangeActions>[] indexAccessors = GetIndexAccessors();
		static IIndexAccessorBase<TableColumn, DocumentModelChangeActions>[] GetIndexAccessors() {
			IIndexAccessorBase<TableColumn, DocumentModelChangeActions>[] result = new IIndexAccessorBase<TableColumn, DocumentModelChangeActions>[FormatCount + 1];
			for (int i = 0; i < ElementCount; i++)
				result[i] = new TableColumnDifferentialFormatIndexAccessor(i);
			for (int i = 0; i < ElementCount; i++)
				result[ElementCount + i] = new TableColumnCellFormatIndexAccessor(i);
			result[FormatCount] = tableColumnApplyFlagsIndexAccessor;
			return result;
		}
		public static TableColumnDifferentialFormatIndexAccessor GetDifferentialFormatIndexAccessor(int elementIndex) {
			return (TableColumnDifferentialFormatIndexAccessor)indexAccessors[elementIndex];
		}
		public static TableColumnCellFormatIndexAccessor GetCellFormatIndexAccessor(int elementIndex) {
			int index = ElementCount + elementIndex;
			return (TableColumnCellFormatIndexAccessor)indexAccessors[index];
		}
		public static TableColumnApplyFlagsIndexAccessor TableColumnApplyFlagsIndexAccessor { get { return tableColumnApplyFlagsIndexAccessor; } }
		#endregion
		public const string DefaultColumnNamePattern = "Column{0}";
		#region Fields
		internal const int HeaderRowIndex = 0;
		internal const int TotalsRowIndex = 1;
		internal const int DataIndex = 2;
		internal const int ElementCount = 3;
		internal const int FormatCount = 6;
		Table table;
		string totalsRowLabel;
		string name;
		string uniqueName; 
		int id;
		int queryTableFieldId;
		TableTotalsRowFormulaProvider totalsRowFormulaProvider;
		TableColumnFormulaProvider columnFormulaProvider;
		TotalsRowFunctionType totalsRowFunction;
		XmlColumnProperties xmlProperties;
		readonly TableColumnElementFormatManager elementFormatManager;
		readonly int[] cellFormatIndexes;
		readonly int[] differentialFormatIndexes;
		int applyFlagsIndex;
		#endregion
		public TableColumn(Table table, string name) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.name = name;
			this.totalsRowLabel = String.Empty;
			queryTableFieldId = DefaultQueryTableFieldId;
			cellFormatIndexes = new int[ElementCount];
			differentialFormatIndexes = new int[ElementCount];
			InitialFormatIndexes();
			elementFormatManager = new TableColumnElementFormatManager(this);
		}
		public TableColumn(Table table)
			: this(table, string.Empty) {
		}
		#region Events
		#region NameChanged
		NameChangedEventHandler onNameChanged;
		internal event NameChangedEventHandler NameChanged {
			add { onNameChanged += value; }
			remove { onNameChanged -= value; }
		}
		protected internal virtual void RaiseNameChanged(string name, string oldName) {
			if (onNameChanged != null) {
				NameChangedEventArgs args = new NameChangedEventArgs(name, oldName);
				onNameChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region Properties
		internal int[] CellFormatIndexes { get { return cellFormatIndexes; } }
		internal int[] DifferentialFormatIndexes { get { return differentialFormatIndexes; } }
		internal int ApplyFlagsIndex { get { return applyFlagsIndex; } }
		internal new TableColumnInfoBatchUpdateHelper BatchUpdateHelper { get { return (TableColumnInfoBatchUpdateHelper)base.BatchUpdateHelper; } }
		internal new DocumentModel DocumentModel { get { return table.DocumentModel; } }
		internal Worksheet Worksheet { get { return table.Worksheet; } }
		internal Table Table { get { return table; } }
		#region Name
		public string Name {
			get { return name; }
			set {
				TableColumnRenameCommand command = new TableColumnRenameCommand(this, value);
				command.Execute();
			}
		}
		protected internal void SetNameCore(string newName) {
			string oldName = name;
			name = newName;
			RaiseNameChanged(newName, oldName);
		}
		#endregion
		protected internal bool HasTotalsRowFormula { get { return totalsRowFormulaProvider != null && totalsRowFormulaProvider.HasFormula; } }
		protected internal Formula TotalsRowFormula {
			get {
				if (!HasTotalsRowFormula)
					return null;
				return totalsRowFormulaProvider.Formula;
			}
		}
		public bool TotalsRowFormulaIsArray { get { return HasTotalsRowFormula && totalsRowFormulaProvider.Formula is ArrayFormula; } }
		public string TotalsRowFormulaText {
			get {
				if (totalsRowFunction == TotalsRowFunctionType.None)
					return string.Empty;
				if (totalsRowFunction == TotalsRowFunctionType.Custom && HasTotalsRowFormula)
					return totalsRowFormulaProvider.FormulaBody;
				return string.Empty;
			}
		}
		protected internal TableTotalsRowFormulaProvider TotalsRowFormulaProvider {
			get {
				if (totalsRowFormulaProvider == null)
					totalsRowFormulaProvider = new TableTotalsRowFormulaProvider(this);
				return totalsRowFormulaProvider;
			}
		}
		protected internal bool HasColumnFormula { get { return columnFormulaProvider != null && columnFormulaProvider.HasFormula; } }
		protected internal Formula ColumnFormula {
			get {
				if (!HasColumnFormula)
					return null;
				return columnFormulaProvider.Formula;
			}
		}
		public bool ColumnFormulaIsArray { get { return HasColumnFormula && columnFormulaProvider.Formula is ArrayFormula; } }
		public string ColumnFormulaText {
			get {
				if (!HasColumnFormula)
					return string.Empty;
				return columnFormulaProvider.FormulaBody;
			}
		}
		protected internal TableColumnFormulaProvider ColumnFormulaProvider {
			get {
				if (columnFormulaProvider == null)
					columnFormulaProvider = new TableColumnFormulaProvider(this);
				return columnFormulaProvider;
			}
		}
		public TotalsRowFunctionType TotalsRowFunction {
			get { return totalsRowFunction; }
			set {
				if (totalsRowFunction == value)
					return;
				SetTotalsRowFunction(value);
			}
		}
		public string TotalsRowFunctionText { get { return GetTotalRowFunctionText(); } }
		public string TotalsRowLabel {
			get { return totalsRowLabel; }
			set {
				if (totalsRowLabel == value)
					return;
				SetTotalsRowLabel(value);
			}
		}
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public int Id { get { return id; } set { id = value; } }
		public int QueryTableFieldId { get { return queryTableFieldId; } set { queryTableFieldId = value; } }
		public XmlColumnProperties XmlProperties { get { return xmlProperties; } set { xmlProperties = value; } }
		protected internal TableCellStyleApplyFlagsInfo ApplyFlagsInfo { get { return IsUpdateLocked ? BatchUpdateHelper.ApplyFlagsInfo : ApplyFlagsInfoCore; } }
		TableCellStyleApplyFlagsInfo ApplyFlagsInfoCore { get { return tableColumnApplyFlagsIndexAccessor.GetInfo(this); } }
		public TableStyleStripeInfo CachedStripeInfo { get; set; }
		#endregion
		#region TotalsRowLabel
		protected internal void SetTotalsRowLabel(string value) {
			DocumentModel.BeginUpdate();
			try {
				SetTotalsRowLabelTransacted(value);
				SetTotalsRowFunctionTransacted(TotalsRowFunctionType.None);
				TotalsRowFormulaProvider.SetFormulaTransacted(null);
				SetTotalsCellValue();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void SetTotalsRowLabelTransacted(string value) {
			if(totalsRowLabel == value)
				return;
			DocumentHistory history = DocumentModel.History;
			TableColumnTotalsRowLabelHistoryItem historyItem = new TableColumnTotalsRowLabelHistoryItem(this, totalsRowLabel, value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetTotalsRowLabelCore(string value) {
			this.totalsRowLabel = value;
		}
		#endregion
		#region TotalsRowFunction
		void SetTotalsRowFunction(TotalsRowFunctionType value) {
			DocumentModel.BeginUpdate();
			try {
				SetTotalsRowFunctionTransacted(value);
				SetTotalsRowLabelTransacted(string.Empty);
				TotalsRowFormulaProvider.SetFormulaTransacted(null);
				SetTotalsCellValue();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void SetTotalsRowFunctionTransacted(TotalsRowFunctionType value) {
			if(totalsRowFunction == value)
				return;
			DocumentHistory history = DocumentModel.History;
			TableColumnTotalsRowFunctionHistoryItem historyItem = new TableColumnTotalsRowFunctionHistoryItem(this, totalsRowFunction, value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetTotalsRowFunctionCore(TotalsRowFunctionType value) {
			this.totalsRowFunction = value;
		}
		string GetTotalRowFunctionText() {
			switch (totalsRowFunction) {
				case TotalsRowFunctionType.Average: return "average";
				case TotalsRowFunctionType.Count: return "count";
				case TotalsRowFunctionType.CountNums: return "countNums";
				case TotalsRowFunctionType.Custom: return "custom";
				case TotalsRowFunctionType.Max: return "max";
				case TotalsRowFunctionType.Min: return "min";
				case TotalsRowFunctionType.None: return "none";
				case TotalsRowFunctionType.StdDev: return "stdDev";
				case TotalsRowFunctionType.Sum: return "sum";
				case TotalsRowFunctionType.Var: return "var";
				default: throw new ArgumentException("Unknown TotalsRowFunctionType: " + totalsRowFunction);
			}
		}
		int GetTotalRowFunctionId(TotalsRowFunctionType functionType) {
			switch (functionType) {
				case TotalsRowFunctionType.Average: return 101;
				case TotalsRowFunctionType.Count: return 103;
				case TotalsRowFunctionType.CountNums: return 102;
				case TotalsRowFunctionType.Max: return 104;
				case TotalsRowFunctionType.Min: return 105;
				case TotalsRowFunctionType.StdDev: return 107;
				case TotalsRowFunctionType.Sum: return 109;
				case TotalsRowFunctionType.Var: return 110;
				default: throw new ArgumentException("Not allowed TotalsRowFunctionType: " + totalsRowFunction);
			}
		}
		#endregion
		#region TotalsRowFormula
		#region SetTotalsRowFormula
		public void SetTotalsRowFormula(string formulaText, bool isArrayFormula) {
			DocumentModel.BeginUpdate();
			try {
				TotalsRowFormulaProvider.SetFormula(formulaText, isArrayFormula);
				ApplyTotalsRowFormulaChanges();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void SetTotalsRowFormula(Formula formula) {
			DocumentModel.BeginUpdate();
			try {
				TotalsRowFormulaProvider.SetFormula(formula);
				ApplyTotalsRowFormulaChanges();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ApplyTotalsRowFormulaChanges() {
			if (!HasTotalsRowFormula)
				SetTotalsRowFunctionTransacted(TotalsRowFunctionType.None);
			else
				SetTotalsRowFunctionTransacted(TotalsRowFunctionType.Custom);
			SetTotalsRowLabelTransacted(string.Empty);
			SetTotalsCellValue();
		}
		protected internal void SetTotalsRowFormulaCore(ParsedExpression expression, bool isArrayFormula) {
			TotalsRowFormulaProvider.SetFormulaCore(expression, isArrayFormula);
		}
		protected internal void SetTotalsRowFormulaTemporarily(string formulaText, bool isArrayFormula) {
			TotalsRowFormulaProvider.SetFormulaTemporarily(formulaText, isArrayFormula);
		}
		#endregion
		#endregion
		protected internal void ParseTemporarilySavedFormulaBodies() {
			if (HasTotalsRowFormula)
				TotalsRowFormulaProvider.ParseTemporarilySavedFormulaBody();
			if (HasColumnFormula)
				ColumnFormulaProvider.ParseTemporarilySavedFormulaBody();
		}
		#region SetTotalsRowCellValue
		void SetTotalsRowFormulaToCell(ICell cell, Formula formula) {
			if (formula == null)
				cell.TransactedSetValue(VariantValue.Empty);
			cell.SetFormulaCore(formula);
		}
		void SetTotalsRowFunctionToCell(ICell cell, TotalsRowFunctionType functionType) {
			ParsedExpression totalsColumnExpression = new ParsedExpression();
			totalsColumnExpression.Add(new ParsedThingInteger() { Value = GetTotalRowFunctionId(functionType) });
			ParsedThingTable tableExpression = new ParsedThingTable();
			tableExpression.TableName = table.Name;
			tableExpression.SetIncludedColumns(name, string.Empty);
			totalsColumnExpression.Add(tableExpression);
			totalsColumnExpression.Add(new ParsedThingFuncVar(FormulaCalculator.FuncSubtotalCode, 2));
			Formula formula = new Formula(cell, totalsColumnExpression);
			cell.SetFormulaCore(formula);
		}
		void SetTotalsRowLabelToCell(ICell cell, string label) {
			if (string.IsNullOrEmpty(label))
				cell.TransactedSetValue(VariantValue.Empty);
			else
				cell.TransactedSetValue(label);
		}
		#endregion
		#region ColumnFormula
		#region SetColumnFormula
		public void SetColumnFormula(string formulaText, bool isArrayFormula) {
			DocumentModel.BeginUpdate();
			try {
				ColumnFormulaProvider.SetFormula(formulaText, isArrayFormula);
				ApplyColumnFormulaChanges();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void SetColumnFormula(Formula formula) {
			DocumentModel.BeginUpdate();
			try {
				ColumnFormulaProvider.SetFormula(formula);
				ApplyColumnFormulaChanges();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ApplyColumnFormulaChanges() {
			if (!HasColumnFormula)
				return;
			CellRange dataRange = GetColumnRangeWithoutHeadersAndTotalsRows();
			string formulaText = "=" + ColumnFormulaText;
			if (ColumnFormulaIsArray) {
				foreach (CellBase cell in dataRange.GetAllCellsEnumerable())
					Worksheet.CreateArray(formulaText, ((ICell)cell).GetRange());
			}
			else
				Worksheet.LocateFormulaToMultipleCells(dataRange, dataRange.TopLeft, formulaText, false);
		}
		protected internal void SetColumnFormulaCore(ParsedExpression expression, bool isArrayFormula) {
			ColumnFormulaProvider.SetFormulaCore(expression, isArrayFormula);
		}
		protected internal void SetColumnFormulaTemporarily(string formulaText, bool isArrayFormula) {
			ColumnFormulaProvider.SetFormulaTemporarily(formulaText, isArrayFormula);
		}
		#endregion
		#endregion
		protected internal virtual CellRange GetColumnRange() {
			int columnIndex = FindColumnIndex();
			return table.Range.GetSubColumnRange(columnIndex, columnIndex);
		}
		protected internal virtual CellRange GetColumnRangeWithoutHeadersAndTotalsRows() {
			int columnIndex = FindColumnIndex();
			return table.Range.GetSubColumnRange(columnIndex, columnIndex, table.HasHeadersRow ? 1 : 0, table.HasTotalsRow ? 1 : 0);
		}
		public virtual CellRange GetTotalRange() {
			if (table.HasTotalsRow)
				return GetTotalRangeCore();
			return null;
		}
		protected internal virtual CellRange GetTotalRangeCore() {
			int columnIndex = FindColumnIndex();
			int rowIndex = table.Range.BottomRight.Row;
			CellPosition position = new CellPosition(table.Range.TopLeft.Column + columnIndex, rowIndex);
			return new CellRange(table.Worksheet, position, position);
		}
		protected internal virtual CellRange GetRangeCore() {
			int columnIndex = table.Range.TopLeft.Column + FindColumnIndex();
			CellPosition topLeft = new CellPosition(columnIndex, table.Range.TopLeft.Row);
			CellPosition bottomRight = new CellPosition(columnIndex, table.Range.BottomRight.Row);
			return new CellRange(table.Worksheet, topLeft, bottomRight);
		}
		#region GetCell
		public ICell GetTotalCell() {
			if (!table.HasTotalsRow)
				return null;
			return GetTotalCellCore();
		}
		public ICell GetTotalCellCore() {
			int index = FindColumnIndex();
			return table.Range.GetCellRelative(index, table.Range.Height - 1) as ICell;
		}
		protected internal int FindColumnIndex() {
			for (int i = 0; i < Table.Columns.Count; i++) {
				if (object.ReferenceEquals(table.Columns[i], this))
					return i;
			}
			return -1;
		}
		ICell GetHeaderCell() {
			if (!table.HasHeadersRow)
				return null;
			int index = FindColumnIndex();
			return table.Range.GetCellRelative(index, 0) as ICell;
		}
		#endregion
		#region SetCellValues
		protected internal void SetCellValues() {
			SetHeadersCellValue();
			SetTotalsCellValue();
		}
		#region SetTotalsCellValue
		void SetTotalsCellValue() {
			ICell totalCell = GetTotalCell();
			if (totalCell != null)
				SetTotalsCellValue(totalCell);
		}
		protected internal void SetTotalsCellValue(ICell cell) {
			if (totalsRowFunction == TotalsRowFunctionType.None)
				SetTotalsRowLabelToCell(cell, totalsRowLabel);
			else if (totalsRowFunction == TotalsRowFunctionType.Custom)
				SetTotalsRowFormulaToCell(cell, TotalsRowFormula);
			else
				SetTotalsRowFunctionToCell(cell, totalsRowFunction);
		}
		#endregion
		#region SetHeadersCellValue
		protected internal void SetHeadersCellValue() {
			ICell headerCell = GetHeaderCell();
			if (headerCell != null)
				headerCell.TransactedSetValue(name);
		}
		#endregion
		#endregion
		protected internal virtual void OnCellValueChanged(ICell cell) {
			int hostRowIndex = cell.RowIndex - table.Range.TopLeft.Row;
			if (hostRowIndex == 0 && table.HasHeadersRow)
				Name = cell.Text;
			else
				if (table.HasTotalsRow && hostRowIndex == table.Range.Height - 1) {
					if (cell.HasFormula) {
						FormulaBase cellFormula = cell.GetFormula();
						SharedFormulaRef sharedFormulaRef = cellFormula as SharedFormulaRef;
						SetTotalsRowFunctionTransacted(TotalsRowFunctionType.Custom);
						SetTotalsRowLabelTransacted(string.Empty);
						if (sharedFormulaRef != null) {
							ParsedExpression formulaExpression = sharedFormulaRef.GetNormalCellFormula(cell);
							TotalsRowFormulaProvider.SetFormulaTransacted(new Formula(cell, formulaExpression));
						}
						else
							TotalsRowFormulaProvider.SetFormulaTransacted((Formula)cell.GetFormula());
					}
					else {
						SetTotalsRowLabelTransacted(cell.Value.ToText(table.DocumentModel.DataContext).InlineTextValue);
						SetTotalsRowFunctionTransacted(TotalsRowFunctionType.None);
						TotalsRowFormulaProvider.SetFormulaTransacted(null);
					}
				}
		}
		protected internal virtual void OnCellRemoved(ICell cell) {
			int hostRowIndex = cell.RowIndex - table.Range.TopLeft.Row;
			if (table.HasTotalsRow && hostRowIndex == table.Range.Height - 1) 
				ClearTotalsRow();
		}
		protected internal void ClearTotalsRow() {
			SetTotalsRowLabelTransacted(string.Empty);
			SetTotalsRowFunctionTransacted(TotalsRowFunctionType.None);
			TotalsRowFormulaProvider.SetFormulaTransacted(null);
		}
		#region Format
		internal CellFormat GetCellFormat(int elementIndex) {
			if (BatchUpdateHelper != null)
				return (CellFormat)BatchUpdateHelper.GetCellFormatInfo(elementIndex);
			int formatIndex = cellFormatIndexes[elementIndex];
			return (CellFormat)DocumentModel.Cache.CellFormatCache[formatIndex];
		}
		internal DifferentialFormat GetDifferentialFormat(int elementIndex) {
			if (BatchUpdateHelper != null)
				return (DifferentialFormat)BatchUpdateHelper.GetDifferentialFormatInfo(elementIndex);
			int formatIndex = differentialFormatIndexes[elementIndex];
			return (DifferentialFormat)DocumentModel.Cache.CellFormatCache[formatIndex];
		}
		void InitialFormatIndexes() {
			for (int i = 0; i < ElementCount; i++) {
				cellFormatIndexes[i] = DocumentModel.StyleSheet.DefaultCellFormatIndex;
				differentialFormatIndexes[i] = CellFormatCache.DefaultDifferentialFormatIndex;
			}
		}
		#endregion
		#region MultiIndexObject Members
		protected override IDocumentModel GetDocumentModel() {
			return table.DocumentModel;
		}
		public override TableColumn GetOwner() {
			return this;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new TableColumnInfoBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new TableColumnInfoBatchInitHelper(this);
		}
		protected override IIndexAccessorBase<TableColumn, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal void AssignCellFormatIndex(int elementIndex, int value) {
			this.cellFormatIndexes[elementIndex] = value;
		}
		internal void AssignDifferentialFormatIndex(int elementIndex, int value) {
			this.differentialFormatIndexes[elementIndex] = value;
		}
		internal void AssignApplyFlagsIndex(int value) {
			this.applyFlagsIndex = value;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected internal delegate DocumentModelChangeActions SetApplyFlagsInfoDelegate(ref TableCellStyleApplyFlagsInfo info, int elementIndex, bool newValue);
		protected internal virtual void SetApplyFlagsInfo(int elementIndex, SetApplyFlagsInfoDelegate setter, bool newValue) {
			IIndexAccessor<TableColumn, TableCellStyleApplyFlagsInfo, DocumentModelChangeActions> indexHolder = TableColumnApplyFlagsIndexAccessor;
			TableCellStyleApplyFlagsInfo info = GetInfoForModification(indexHolder);
			DocumentModelChangeActions changeActions = setter(ref info, elementIndex, newValue);
			ReplaceInfoForFlags(indexHolder, info, changeActions);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			TableColumn other = obj as TableColumn;
			if (other == null || !base.Equals(other))
				return false;
			return
				this.totalsRowLabel == other.totalsRowLabel &&
				this.name == other.name &&
				this.uniqueName == other.uniqueName &&
				this.id == other.id &&
				this.queryTableFieldId == other.queryTableFieldId &&
				((!this.HasTotalsRowFormula && !other.HasTotalsRowFormula) || (HasTotalsRowFormula && TotalsRowFormula == other.TotalsRowFormula)) &&
				((!this.HasColumnFormula && !other.HasColumnFormula) || (HasColumnFormula && ColumnFormula == other.ColumnFormula)) &&
				this.totalsRowFunction == other.totalsRowFunction &&
				this.xmlProperties == other.xmlProperties;
		}
		public override int GetHashCode() {
			CombinedHashCode result = new CombinedHashCode();
			result.AddInt(base.GetHashCode());
			result.AddInt(id);
			result.AddInt(queryTableFieldId);
			result.AddInt((int)totalsRowFunction);
			result.AddObject(GetType()); 
			if (!String.IsNullOrEmpty(totalsRowLabel))
				result.AddObject(totalsRowLabel);
			if (!String.IsNullOrEmpty(name))
				result.AddObject(name);
			if (!String.IsNullOrEmpty(uniqueName))
				result.AddObject(uniqueName);
			if (HasColumnFormula)
				result.AddObject(ColumnFormula);
			if (HasTotalsRowFormula)
				result.AddObject(TotalsRowFormula);
			return result.CombinedHash32;
		}
		#endregion
		#region ITableColumnFormatInfo Members
		public ITableColumnInfoElementFormat HeaderRow { get { return elementFormatManager.GetFormatInfo(HeaderRowIndex); } }
		public ITableColumnInfoElementFormat Data { get { return elementFormatManager.GetFormatInfo(DataIndex); } }
		public ITableColumnInfoElementFormat TotalsRow { get { return elementFormatManager.GetFormatInfo(TotalsRowIndex); } }
		#region FormatStringPropertyChanger
		string IDifferentialFormatPropertyChanger.GetFormatString(int elementIndex) {
			return GetDifferentialFormat(elementIndex).FormatString;
		}
		void IDifferentialFormatPropertyChanger.SetFormatString(int elementIndex, string value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.FormatString == value && info.MultiOptionsInfo.ApplyNumberFormat)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFormatStringCore, value);
		}
		DocumentModelChangeActions SetFormatStringCore(FormatBase info, string value) {
			((DifferentialFormat)info).FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontInfoPropertyChanger
		#region FontName
		string IDifferentialFormatPropertyChanger.GetFontName(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Name;
		}
		void IDifferentialFormatPropertyChanger.SetFontName(int elementIndex, string value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Name == value && info.MultiOptionsInfo.ApplyFontName)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontNameCore, value);
		}
		DocumentModelChangeActions SetFontNameCore(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSize
		double IDifferentialFormatPropertyChanger.GetFontSize(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Size;
		}
		void IDifferentialFormatPropertyChanger.SetFontSize(int elementIndex, double value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Size == value && info.MultiOptionsInfo.ApplyFontSize)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontSizeCore, value);
		}
		DocumentModelChangeActions SetFontSizeCore(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontBold
		bool IDifferentialFormatPropertyChanger.GetFontBold(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Bold;
		}
		void IDifferentialFormatPropertyChanger.SetFontBold(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Bold == value && info.MultiOptionsInfo.ApplyFontBold)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontBoldCore, value);
		}
		DocumentModelChangeActions SetFontBoldCore(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontItalic
		bool IDifferentialFormatPropertyChanger.GetFontItalic(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Italic;
		}
		void IDifferentialFormatPropertyChanger.SetFontItalic(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Italic == value && info.MultiOptionsInfo.ApplyFontItalic)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontItalicCore, value);
		}
		DocumentModelChangeActions SetFontItalicCore(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontStrikeThrough
		bool IDifferentialFormatPropertyChanger.GetFontStrikeThrough(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.StrikeThrough;
		}
		void IDifferentialFormatPropertyChanger.SetFontStrikeThrough(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.StrikeThrough == value && info.MultiOptionsInfo.ApplyFontStrikeThrough)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontStrikeThroughCore, value);
		}
		DocumentModelChangeActions SetFontStrikeThroughCore(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontCondense
		bool IDifferentialFormatPropertyChanger.GetFontCondense(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Condense;
		}
		void IDifferentialFormatPropertyChanger.SetFontCondense(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Condense == value && info.MultiOptionsInfo.ApplyFontCondense)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontCondenseCore, value);
		}
		DocumentModelChangeActions SetFontCondenseCore(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontExtend
		bool IDifferentialFormatPropertyChanger.GetFontExtend(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Extend;
		}
		void IDifferentialFormatPropertyChanger.SetFontExtend(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Extend == value && info.MultiOptionsInfo.ApplyFontExtend)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontExtendCore, value);
		}
		DocumentModelChangeActions SetFontExtendCore(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontOutline
		bool IDifferentialFormatPropertyChanger.GetFontOutline(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Outline;
		}
		void IDifferentialFormatPropertyChanger.SetFontOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Outline == value && info.MultiOptionsInfo.ApplyFontOutline)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontOutlineCore, value);
		}
		DocumentModelChangeActions SetFontOutlineCore(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontShadow
		bool IDifferentialFormatPropertyChanger.GetFontShadow(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Shadow;
		}
		void IDifferentialFormatPropertyChanger.SetFontShadow(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Shadow == value && info.MultiOptionsInfo.ApplyFontShadow)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontShadowCore, value);
		}
		DocumentModelChangeActions SetFontShadowCore(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontCharset
		int IDifferentialFormatPropertyChanger.GetFontCharset(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Charset;
		}
		void IDifferentialFormatPropertyChanger.SetFontCharset(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Charset == value && info.MultiOptionsInfo.ApplyFontCharset)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontCharsetCore, value);
		}
		DocumentModelChangeActions SetFontCharsetCore(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontFontFamily
		int IDifferentialFormatPropertyChanger.GetFontFontFamily(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.FontFamily;
		}
		void IDifferentialFormatPropertyChanger.SetFontFontFamily(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.FontFamily == value && info.MultiOptionsInfo.ApplyFontFamily)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontFontFamilyCore, value);
		}
		DocumentModelChangeActions SetFontFontFamilyCore(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontUnderline
		XlUnderlineType IDifferentialFormatPropertyChanger.GetFontUnderline(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Underline;
		}
		void IDifferentialFormatPropertyChanger.SetFontUnderline(int elementIndex, XlUnderlineType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Underline == value && info.MultiOptionsInfo.ApplyFontUnderline)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontUnderlineCore, value);
		}
		DocumentModelChangeActions SetFontUnderlineCore(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontColor
		Color IDifferentialFormatPropertyChanger.GetFontColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Color;
		}
		void IDifferentialFormatPropertyChanger.SetFontColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Color == value && info.MultiOptionsInfo.ApplyFontColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontColorCore, value);
		}
		DocumentModelChangeActions SetFontColorCore(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontScript
		XlScriptType IDifferentialFormatPropertyChanger.GetFontScript(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Script;
		}
		void IDifferentialFormatPropertyChanger.SetFontScript(int elementIndex, XlScriptType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Script == value && info.MultiOptionsInfo.ApplyFontScript)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontScriptCore, value);
		}
		DocumentModelChangeActions SetFontScriptCore(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSchemeStyle
		XlFontSchemeStyles IDifferentialFormatPropertyChanger.GetFontSchemeStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.SchemeStyle;
		}
		void IDifferentialFormatPropertyChanger.SetFontSchemeStyle(int elementIndex, XlFontSchemeStyles value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.SchemeStyle == value && info.MultiOptionsInfo.ApplyFontSchemeStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontScriptCore, value);
		}
		DocumentModelChangeActions SetFontScriptCore(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region FillInfoPropertyChanger
		#region ClearFill
		void IDifferentialFormatPropertyChanger.ClearFill(int elementIndex) {
			if (!GetDifferentialFormat(elementIndex).MultiOptionsInfo.ApplyFillNone)
				ClearFillCore(elementIndex);
		}
		void ClearFillCore(int elementIndex) {
			TableColumnDifferentialFormatIndexAccessor accessor = GetDifferentialFormatIndexAccessor(elementIndex);
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
		#region FillPatternType
		XlPatternType IDifferentialFormatPropertyChanger.GetFillPatternType(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.PatternType;
		}
		void IDifferentialFormatPropertyChanger.SetFillPatternType(int elementIndex, XlPatternType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.PatternType == value && info.MultiOptionsInfo.ApplyFillPatternType)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillPatternTypeCore, value);
		}
		DocumentModelChangeActions SetFillPatternTypeCore(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FillForeColor
		Color IDifferentialFormatPropertyChanger.GetFillForeColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.ForeColor;
		}
		void IDifferentialFormatPropertyChanger.SetFillForeColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.ForeColor == value && info.MultiOptionsInfo.ApplyFillForeColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillForeColorCore, value);
		}
		DocumentModelChangeActions SetFillForeColorCore(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FillBackColor
		Color IDifferentialFormatPropertyChanger.GetFillBackColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.BackColor;
		}
		void IDifferentialFormatPropertyChanger.SetFillBackColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.BackColor == value && info.MultiOptionsInfo.ApplyFillBackColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillBackColorCore, value);
		}
		DocumentModelChangeActions SetFillBackColorCore(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region GradientFillInfoPropertyChanger
		#region GradientStops
		IGradientStopCollection IDifferentialFormatPropertyChanger.GetGradientStops(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.GradientStops;
		}
		#endregion
		#region Type
		ModelGradientFillType IDifferentialFormatPropertyChanger.GetGradientFillType(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Type;
		}
		void IDifferentialFormatPropertyChanger.SetGradientFillType(int elementIndex, ModelGradientFillType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Type == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetGradientTypeCore, value);
		}
		DocumentModelChangeActions SetGradientTypeCore(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Degree
		double IDifferentialFormatPropertyChanger.GetDegree(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Degree;
		}
		void IDifferentialFormatPropertyChanger.SetDegree(int elementIndex, double value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Degree == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetDegreeCore, value);
		}
		DocumentModelChangeActions SetDegreeCore(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceLeft
		float IDifferentialFormatPropertyChanger.GetConvergenceLeft(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Left;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceLeft(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Left == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceLeftCore, value);
		}
		DocumentModelChangeActions SetConvergenceLeftCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceRight
		float IDifferentialFormatPropertyChanger.GetConvergenceRight(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Right;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceRight(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Right == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceRightCore, value);
		}
		DocumentModelChangeActions SetConvergenceRightCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceTop
		float IDifferentialFormatPropertyChanger.GetConvergenceTop(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Top;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceTop(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Top == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceTopCore, value);
		}
		DocumentModelChangeActions SetConvergenceTopCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceBottom
		float IDifferentialFormatPropertyChanger.GetConvergenceBottom(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Bottom;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceBottom(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Bottom == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceBottomCore, value);
		}
		DocumentModelChangeActions SetConvergenceBottomCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region FillTypePropertyChanger
		ModelFillType IDifferentialFormatPropertyChanger.GetFillType(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.FillType;
		}
		void IDifferentialFormatPropertyChanger.SetFillType(int elementIndex, ModelFillType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.FillType == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillTypeCore, value);
		}
		DocumentModelChangeActions SetFillTypeCore(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region BorderInfoPropertyChanger
		#region BorderLeftLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderLeftLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.LeftLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftLineStyle == value && info.BorderOptionsInfo.ApplyLeftLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderLeftLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderLeftLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderRightLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.RightLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightLineStyle == value && info.BorderOptionsInfo.ApplyRightLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderRightLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderRightLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderTopLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.TopLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopLineStyle == value && info.BorderOptionsInfo.ApplyTopLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderTopLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderTopLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderBottomLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.BottomLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomLineStyle == value && info.BorderOptionsInfo.ApplyBottomLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderBottomLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderBottomLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderHorizontalLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.HorizontalLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalLineStyle == value && info.BorderOptionsInfo.ApplyHorizontalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderHorizontalLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderVerticalLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.VerticalLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalLineStyle == value && info.BorderOptionsInfo.ApplyVerticalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderVerticalLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalUpLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderDiagonalUpLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalUpLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalUpLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalUpLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalUpLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalDownLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderDiagonalDownLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalDownLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalDownLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalDownLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalDownLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderLeftColor
		Color IDifferentialFormatPropertyChanger.GetBorderLeftColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.LeftColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftColor == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderLeftColorCore, value);
		}
		DocumentModelChangeActions SetBorderLeftColorCore(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightColor
		Color IDifferentialFormatPropertyChanger.GetBorderRightColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.RightColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightColor == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderRightColorCore, value);
		}
		DocumentModelChangeActions SetBorderRightColorCore(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopColor
		Color IDifferentialFormatPropertyChanger.GetBorderTopColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.TopColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopColor == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderTopColorCore, value);
		}
		DocumentModelChangeActions SetBorderTopColorCore(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomColor
		Color IDifferentialFormatPropertyChanger.GetBorderBottomColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.BottomColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomColor == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderBottomColorCore, value);
		}
		DocumentModelChangeActions SetBorderBottomColorCore(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalColor
		Color IDifferentialFormatPropertyChanger.GetBorderHorizontalColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.HorizontalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalColor == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderHorizontalColorCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalColorCore(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalColor
		Color IDifferentialFormatPropertyChanger.GetBorderVerticalColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.VerticalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalColor == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderVerticalColorCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalColorCore(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalColor
		Color IDifferentialFormatPropertyChanger.GetBorderDiagonalColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalColor == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalColorCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalColorCore(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderLeftColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderLeftColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.LeftColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftColorIndex == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderLeftColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderLeftColorIndexCore(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderRightColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.RightColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightColorIndex == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderRightColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderRightColorIndexCore(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderTopColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.TopColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopColorIndex == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderTopColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderTopColorIndexCore(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderBottomColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.BottomColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomColorIndex == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderBottomColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderBottomColorIndexCore(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderHorizontalColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.HorizontalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalColorIndex == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderHorizontalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndexCore(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderVerticalColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.VerticalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalColorIndex == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderVerticalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalColorIndexCore(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderDiagonalColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalColorIndex == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndexCore(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderOutline
		bool IDifferentialFormatPropertyChanger.GetBorderOutline(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.Outline;
		}
		void IDifferentialFormatPropertyChanger.SetBorderOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.Outline == value && info.BorderOptionsInfo.ApplyOutline)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderOutlineCore, value);
		}
		DocumentModelChangeActions SetBorderOutlineCore(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region CellAlignmentPropertyChanger
		#region WrapText
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentWrapText(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.WrapText;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentWrapText(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.WrapText == value && info.MultiOptionsInfo.ApplyAlignmentWrapText)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentWrapTextCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentWrapTextCore(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region JustifyLastLine
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentJustifyLastLine(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.JustifyLastLine;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentJustifyLastLine(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.JustifyLastLine == value && info.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentJustifyLastLineCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentJustifyLastLineCore(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ShrinkToFit
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentShrinkToFit(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.ShrinkToFit;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentShrinkToFit(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.ShrinkToFit == value && info.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentShrinkToFitCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentShrinkToFitCore(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region TextRotation
		int IDifferentialFormatPropertyChanger.GetCellAlignmentTextRotation(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.TextRotation;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentTextRotation(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.TextRotation == value && info.MultiOptionsInfo.ApplyAlignmentTextRotation)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentTextRotationCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentTextRotationCore(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Indent
		byte IDifferentialFormatPropertyChanger.GetCellAlignmentIndent(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.Indent;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentIndent(int elementIndex, byte value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Indent == value && info.MultiOptionsInfo.ApplyAlignmentIndent)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentIndentCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentIndentCore(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region RelativeIndent
		int IDifferentialFormatPropertyChanger.GetCellAlignmentRelativeIndent(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.RelativeIndent;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentRelativeIndent(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.RelativeIndent == value && info.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentRelativeIndentCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentRelativeIndentCore(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Horizontal
		XlHorizontalAlignment IDifferentialFormatPropertyChanger.GetCellAlignmentHorizontal(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.Horizontal;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentHorizontal(int elementIndex, XlHorizontalAlignment value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Horizontal == value && info.MultiOptionsInfo.ApplyAlignmentHorizontal)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentHorizontalCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentHorizontalCore(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Vertical
		XlVerticalAlignment IDifferentialFormatPropertyChanger.GetCellAlignmentVertical(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.Vertical;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentVertical(int elementIndex, XlVerticalAlignment value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Vertical == value && info.MultiOptionsInfo.ApplyAlignmentVertical)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentVerticalCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentVerticalCore(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlReadingOrder
		XlReadingOrder IDifferentialFormatPropertyChanger.GetCellAlignmentReadingOrder(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.ReadingOrder;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentReadingOrder(int elementIndex, XlReadingOrder value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.ReadingOrder == value && info.MultiOptionsInfo.ApplyAlignmentReadingOrder)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentReadingOrderCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentReadingOrderCore(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region CellProtectionPropertyChanger
		#region Locked
		bool IDifferentialFormatPropertyChanger.GetCellProtectionLocked(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Protection.Locked;
		}
		void IDifferentialFormatPropertyChanger.SetCellProtectionLocked(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Protection.Locked == value && info.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellProtectionLockedCore, value);
		}
		DocumentModelChangeActions SetCellProtectionLockedCore(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Hidden
		bool IDifferentialFormatPropertyChanger.GetCellProtectionHidden(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Protection.Hidden;
		}
		void IDifferentialFormatPropertyChanger.SetCellProtectionHidden(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Protection.Hidden == value && info.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellProtectionHiddenCore, value);
		}
		DocumentModelChangeActions SetCellProtectionHiddenCore(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		public bool GetApplyDifferentialFormat(int elementIndex) {
			DifferentialFormat format = GetDifferentialFormat(elementIndex);
			return format.MultiOptionsIndex != MultiOptionsInfo.DefaultIndex ||
				format.BorderOptionsIndex != BorderOptionsInfo.DefaultIndex;
		}
		#region ICellStylePropertyChanger Members
		public CellStyleBase GetCellStyle(int elementIndex) {
			return GetCellFormat(elementIndex).Style;
		}
		public void SetCellStyle(int elementIndex, CellStyleBase value) {
			if (value == DocumentModel.StyleSheet.CellStyles[0]) {
				SetCellStyleCore(elementIndex, value, false);
				return;
			}
			if (GetCellFormat(elementIndex).Style == value)
				return;
			SetCellStyleCore(elementIndex, value, true);
		}
		void SetCellStyleCore(int elementIndex, CellStyleBase value, bool performSetProperty) {
			DocumentModel.BeginUpdate();
			try {
				SetApplyCellStyle(elementIndex, value != null);
				if (performSetProperty)
					SetPropertyValueCore(GetCellFormatIndexAccessor(elementIndex), SetStyleIndex, value);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		DocumentModelChangeActions SetStyleIndex(FormatBase info, CellStyleBase value) {
			((CellFormat)info).ApplyStyle(value);
			return DocumentModelChangeActions.None;
		}
		public bool GetApplyCellStyle(int elementIndex) {
			return ApplyFlagsInfo.GetApplyCellStyle(elementIndex);
		}
		void SetApplyCellStyle(int elementIndex, bool value) {
			if (GetApplyCellStyle(elementIndex) == value)
				return;
			SetApplyFlagsInfo(elementIndex, SetApplyCellStyle, value);
		}
		DocumentModelChangeActions SetApplyCellStyle(ref TableCellStyleApplyFlagsInfo info, int elementIndex, bool value) {
			info.SetApplyCellStyle(elementIndex, value);
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
	}
	#endregion
	#region TableFormulaProvider
	public abstract class TableFormulaProvider {
		#region Fields
		readonly TableColumn column;
		byte[] binaryFormula;
		#endregion
		protected TableFormulaProvider(TableColumn column) {
			this.column = column;
		}
		#region Properties
		public byte[] BinaryFormula { get { return binaryFormula; } protected set { binaryFormula = value; } }
		public TableColumn Column { get { return column; } }
		public bool HasFormula { get { return binaryFormula != null; } }
		public Formula Formula { get { return HasFormula ? FormulaFactory.CreateTableFormulaInstance(CellForFormula, binaryFormula, Worksheet) : null; } }
		public string FormulaBody { get { return GetFormulaBody(); } }
		protected WorkbookDataContext Context { get { return column.DocumentModel.DataContext; } }
		protected internal Worksheet Worksheet { get { return column.Worksheet; } }
		protected DocumentModel DocumentModel { get { return column.DocumentModel; } }
		#endregion
		protected abstract CellRange RangeForArrayFormula { get; }
		public abstract ICell CellForFormula { get; }
		#region CreateFormula
		protected Formula CreateFormula(string formulaText, bool isArrayFormula) {
			Formula formula = null;
			if (!string.IsNullOrEmpty(formulaText)) {
				if (isArrayFormula)
					formula = new ArrayFormula(RangeForArrayFormula, formulaText);
				else
					formula = new Formula(CellForFormula, formulaText);
			}
			return formula;
		}
		protected Formula CreateFormula(ParsedExpression expression, bool isArrayFormula) {
			Formula formula = null;
			if (expression != null && expression.Count > 0) {
				if (isArrayFormula)
					formula = new ArrayFormula(RangeForArrayFormula, expression);
				else
					formula = new Formula(null, expression);
			}
			return formula;
		}
		protected Formula CreateFormula(bool isArrayFormula) {
			Formula formula = null;
			if (isArrayFormula)
				formula = new ArrayFormula(RangeForArrayFormula);
			else
				formula = new Formula(CellForFormula);
			return formula;
		}
		#endregion
		public void SetFormula(string formulaText, bool isArrayFormula) {
			Formula formula = CreateFormula(formulaText, isArrayFormula);
			SetFormula(formula);
		}
		public void SetFormula(Formula formula) {
			SetFormulaTransacted(formula);
		}
		protected internal void SetFormulaCore(byte[] newBinaryFormula) {
			BinaryFormula = newBinaryFormula;
		}
		protected internal void SetFormulaTransacted(Formula formula) {
			if(formula == null && !HasFormula)
				return;
			byte[] newBinaryFormula = null;
			if(formula != null)
				newBinaryFormula = formula.GetBinary(Context);
			DocumentHistory history = DocumentModel.History;
			TableColumnFormulaHistoryItem historyItem = new TableColumnFormulaHistoryItem(this, BinaryFormula, newBinaryFormula);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetFormulaCore(ParsedExpression expression, bool isArrayFormula) {
			Formula formula = CreateFormula(expression, isArrayFormula);
			binaryFormula = formula.GetBinary(DocumentModel.DataContext);
		}
		protected internal void SetFormulaTemporarily(string formulaText, bool isArrayFormula) {
			Formula formula = CreateFormula(isArrayFormula);
			if (formula != null)
				this.binaryFormula = formula.GetTemporarilyBody(formulaText);
		}
		protected internal void ParseTemporarilySavedFormulaBody() {
			if (!HasFormula)
				return;
			Formula formula = Formula;
			formula.Cell.FormulaInfo = new FormulaInfo();
			formula.GetExpression(DocumentModel.DataContext);
			this.binaryFormula = formula.GetBinary(DocumentModel.DataContext);
		}
		protected internal string GetFormulaBody(){
			if (!HasFormula)
				return string.Empty;
			string formulaBody = Formula.GetBody(CellForFormula);
			if (!string.IsNullOrEmpty(formulaBody))
				return formulaBody.Substring(1);
			return string.Empty;
		}
	}
	public class TableTotalsRowFormulaProvider : TableFormulaProvider {
		public TableTotalsRowFormulaProvider(TableColumn column)
			: base(column) {
		}
		protected override CellRange RangeForArrayFormula { get { return Column.GetTotalRangeCore(); } }
		public override ICell CellForFormula { get { return GetCellForFormula(); } }
		ICell GetCellForFormula() {
			CellRange columnRange = Column.GetTotalRangeCore();
			return new FakeCell(columnRange.BottomRight, columnRange.Worksheet as Worksheet);
		}
	}
	public class TableColumnFormulaProvider : TableFormulaProvider {
		public TableColumnFormulaProvider(TableColumn column)
			: base(column) {
		}
		protected override CellRange RangeForArrayFormula { get { return Column.GetRangeCore(); } }
		public override ICell CellForFormula { get { return GetCellForFormula(); } }
		ICell GetCellForFormula() {
			CellRange columnRange = Column.GetRangeCore();
			return new FakeCell(columnRange.TopLeft, columnRange.Worksheet as Worksheet);
		}
	}
	#endregion
	#region TableColumnFormatIndexAccessorBase (abstract class)
	public abstract class TableColumnFormatIndexAccessorBase<TInfo> : IIndexAccessor<TableColumn, TInfo, DocumentModelChangeActions>
		where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
		readonly int elementIndex;
		protected TableColumnFormatIndexAccessorBase(int elementIndex) {
			this.elementIndex = elementIndex;
		}
		protected int ElementIndex { get { return elementIndex; } }
		#region IIndexAccessor Members
		public int GetDeferredInfoIndex(TableColumn owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public int GetInfoIndex(TableColumn owner, TInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public TInfo GetInfo(TableColumn owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(TableColumn owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		public void InitializeDeferredInfo(TableColumn owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(TableColumn owner, TableColumn from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(TableColumn owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public abstract int GetIndex(TableColumn owner);
		public abstract void SetIndex(TableColumn owner, int value);
		public abstract TInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper);
		public abstract void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TInfo info);
		public abstract IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableColumn owner);
		protected abstract UniqueItemsCache<TInfo> GetInfoCache(TableColumn owner);
		#endregion
	}
	#endregion
	#region TableColumnDifferentialFormatIndexAccessor
	public class TableColumnDifferentialFormatIndexAccessor : TableColumnFormatIndexAccessorBase<FormatBase> {
		public TableColumnDifferentialFormatIndexAccessor(int elementIndex)
			: base(elementIndex) {
		}
		#region IIndexAccessor Members
		public override int GetIndex(TableColumn owner) {
			return owner.DifferentialFormatIndexes[ElementIndex];
		}
		public override void SetIndex(TableColumn owner, int value) {
			owner.AssignDifferentialFormatIndex(ElementIndex, value);
		}
		public override FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableColumnInfoBatchUpdateHelper)helper).GetDifferentialFormatInfo(ElementIndex);
		}
		public override void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			DifferentialFormat sourceDiffFormat = info as DifferentialFormat;
			TableColumnInfoBatchUpdateHelper tableColumnInfoBatchUpdateHelper = helper as TableColumnInfoBatchUpdateHelper;
			System.Diagnostics.Debug.Assert(sourceDiffFormat != null);
			System.Diagnostics.Debug.Assert(tableColumnInfoBatchUpdateHelper != null);
			TableColumn owner = helper.BatchUpdateHandler as TableColumn;
			if (!Object.ReferenceEquals(sourceDiffFormat.DocumentModel, owner.DocumentModel)) {
				FormatBase targetCellFormat = tableColumnInfoBatchUpdateHelper.GetDifferentialFormatInfo(ElementIndex);
				targetCellFormat.CopyFrom(sourceDiffFormat);
			}
			else
				tableColumnInfoBatchUpdateHelper.SetDifferentialFormatInfo(ElementIndex, info.Clone());
		}
		public override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableColumn owner) {
			return new TableColumnDifferentialFormatIndexChangeHistoryItem(owner, ElementIndex);
		}
		protected override UniqueItemsCache<FormatBase> GetInfoCache(TableColumn owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		#endregion
	}
	#endregion
	#region TableColumnCellFormatIndexAccessor
	public class TableColumnCellFormatIndexAccessor : TableColumnFormatIndexAccessorBase<FormatBase> {
		public TableColumnCellFormatIndexAccessor(int elementIndex)
			: base(elementIndex) {
		}
		#region IIndexAccessor Members
		public override int GetIndex(TableColumn owner) {
			return owner.CellFormatIndexes[ElementIndex];
		}
		public override void SetIndex(TableColumn owner, int value) {
			owner.AssignCellFormatIndex(ElementIndex, value);
		}
		public override FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableColumnInfoBatchUpdateHelper)helper).GetCellFormatInfo(ElementIndex);
		}
		public override void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			CellFormat sourceCellFormat = info as CellFormat;
			TableColumnInfoBatchUpdateHelper tableColumnInfoBatchUpdateHelper = helper as TableColumnInfoBatchUpdateHelper;
			System.Diagnostics.Debug.Assert(sourceCellFormat != null);
			System.Diagnostics.Debug.Assert(tableColumnInfoBatchUpdateHelper != null);
			TableColumn owner = helper.BatchUpdateHandler as TableColumn;
			if (!Object.ReferenceEquals(sourceCellFormat.DocumentModel, owner.DocumentModel)) {
				FormatBase targetCellFormat = tableColumnInfoBatchUpdateHelper.GetCellFormatInfo(ElementIndex);
				targetCellFormat.CopyFrom(sourceCellFormat);
			}
			else
				tableColumnInfoBatchUpdateHelper.SetCellFormatInfo(ElementIndex, info.Clone());
		}
		public override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableColumn owner) {
			return new TableColumnCellFormatIndexChangeHistoryItem(owner, ElementIndex);
		}
		protected override UniqueItemsCache<FormatBase> GetInfoCache(TableColumn owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		#endregion
	}
	#endregion
	#region TableColumnInfoBatchUpdateHelper
	public class TableColumnInfoBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		FormatBase[] differentialFormats;
		FormatBase[] cellFormats;
		TableCellStyleApplyFlagsInfo applyFlagsInfo;
		public TableColumnInfoBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
			this.differentialFormats = new FormatBase[TableColumn.ElementCount];
			this.cellFormats = new FormatBase[TableColumn.ElementCount];
		}
		public TableCellStyleApplyFlagsInfo ApplyFlagsInfo { get { return applyFlagsInfo; } set { applyFlagsInfo = value; } }
		public FormatBase GetCellFormatInfo(int elementIndex) {
			return cellFormats[elementIndex];
		}
		public void SetCellFormatInfo(int elementIndex, FormatBase value) {
			cellFormats[elementIndex] = value;
		}
		public FormatBase GetDifferentialFormatInfo(int elementIndex) {
			return differentialFormats[elementIndex];
		}
		public void SetDifferentialFormatInfo(int elementIndex, FormatBase value) {
			differentialFormats[elementIndex] = value;
		}
	}
	#endregion
	#region TableColumnInfoBatchInitHelper
	public class TableColumnInfoBatchInitHelper : FormatBaseBatchUpdateHelper {
		public TableColumnInfoBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region ITableColumnInfoFormat
	public interface ITableColumnInfoFormat : IDifferentialFormatPropertyChanger, ICellStylePropertyChanger {
		ITableColumnInfoElementFormat HeaderRow { get; }
		ITableColumnInfoElementFormat Data { get; }
		ITableColumnInfoElementFormat TotalsRow { get; }
	}
	#endregion
	#region TableFormatApplyFlagsInfo
	public struct TableCellStyleApplyFlagsInfo : ICloneable<TableCellStyleApplyFlagsInfo>, ISupportsSizeOf, ISupportsCopyFrom<TableCellStyleApplyFlagsInfo> {
		int packedValues;
		public int PackedValues { get { return packedValues; } set { this.packedValues = value; } }
		int GetMaskCore(int elementIndex) {
			return (int)Math.Pow(2, elementIndex);
		}
		public bool GetApplyCellStyle(int elementIndex) {
			return GetBooleanValue(GetMaskCore(elementIndex));
		}
		public void SetApplyCellStyle(int elementIndex, bool value) {
			SetBooleanValue(GetMaskCore(elementIndex), value);
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(int mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(int mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(TableCellStyleApplyFlagsInfo));
		}
		#endregion
		#region ICloneable<TableCellStyleApplyFlagsInfo> Members
		public TableCellStyleApplyFlagsInfo Clone() {
			TableCellStyleApplyFlagsInfo clone = new TableCellStyleApplyFlagsInfo();
			clone.PackedValues = this.PackedValues;
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<TableCellStyleApplyFlagsInfo> Members
		public void CopyFrom(TableCellStyleApplyFlagsInfo value) {
			this.packedValues = value.packedValues;
		}
		#endregion
	}
	#endregion
	#region TableColumnApplyFlagsIndexAccessor
	public class TableColumnApplyFlagsIndexAccessor : IIndexAccessor<TableColumn, TableCellStyleApplyFlagsInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<TableColumnInfo, TableFormatApplyFlagsInfo> Members
		public int GetIndex(TableColumn owner) {
			return owner.ApplyFlagsIndex;
		}
		public int GetDeferredInfoIndex(TableColumn owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(TableColumn owner, int value) {
			owner.AssignApplyFlagsIndex(value);
		}
		public int GetInfoIndex(TableColumn owner, TableCellStyleApplyFlagsInfo value) {
			return value.PackedValues;
		}
		public TableCellStyleApplyFlagsInfo GetInfo(TableColumn owner) {
			TableCellStyleApplyFlagsInfo info = new TableCellStyleApplyFlagsInfo();
			info.PackedValues = owner.ApplyFlagsIndex;
			return info;
		}
		public bool IsIndexValid(TableColumn owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableColumn owner) {
			return new TableColumnInfoApplyFlagsIndexChangeHistoryItem(owner);
		}
		public TableCellStyleApplyFlagsInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableColumnInfoBatchUpdateHelper)helper).ApplyFlagsInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TableCellStyleApplyFlagsInfo info) {
			((TableColumnInfoBatchUpdateHelper)helper).ApplyFlagsInfo = info.Clone();
		}
		public void InitializeDeferredInfo(TableColumn owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(TableColumn owner, TableColumn from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(TableColumn owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region ITableColumnElementFormat
	public interface ITableColumnInfoElementFormat {
		IDifferentialFormat DifferentialFormat { get; }
		CellStyleBase CellStyle { get; set; }
		bool ApplyCellStyle { get; }
		bool ApplyDifferentialFormat { get; }
	}
	#endregion
	#region TableColumnElementFormatManager
	public class TableColumnElementFormatManager : ITableColumnInfoElementFormat {
		int elementIndex;
		readonly TableColumn info;
		readonly DifferentialFormatPropertyChangeManager differentialFormatPropertyChangeManager;
		public TableColumnElementFormatManager(TableColumn info) {
			this.info = info;
			differentialFormatPropertyChangeManager = new DifferentialFormatPropertyChangeManager(info);
		}
		#region ITableColumnElementFormatInfo Members
		public IDifferentialFormat DifferentialFormat { get { return differentialFormatPropertyChangeManager.GetFormatInfo(elementIndex); } }
		public CellStyleBase CellStyle { get { return info.GetCellStyle(elementIndex); } set { info.SetCellStyle(elementIndex, value); } }
		public bool ApplyCellStyle { get { return info.GetApplyCellStyle(elementIndex); } }
		public bool ApplyDifferentialFormat { get { return info.GetApplyDifferentialFormat(elementIndex); } }
		#endregion
		protected internal ITableColumnInfoElementFormat GetFormatInfo(int elementIndex) {
			this.elementIndex = elementIndex;
			return this;
		}
	}
	#endregion
}
