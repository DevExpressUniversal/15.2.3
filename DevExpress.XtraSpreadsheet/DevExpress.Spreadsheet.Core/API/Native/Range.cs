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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	[Flags]
	public enum ReferenceElement {
		RowAbsolute = 0x1,
		ColumnAbsolute = 0x2,
		IncludeSheetName = 0x4,
	}
	#region AreasCollection
	public interface AreasCollection : ISimpleCollection<Range> {
	}
	#endregion
	#region Range
	public interface Range : Formatting, IEnumerable<Cell> {
		string Name { get; set; }
		Worksheet Worksheet { get; }
		string Formula { get; set; }
		string FormulaInvariant { get; set; }
		string ArrayFormula { get; set; }
		string ArrayFormulaInvariant { get; set; }
		CellValue Value { get; set; }
		void SetValue(object value);
		AreasCollection Areas { get; }
		int LeftColumnIndex { get; }
		int TopRowIndex { get; }
		int RightColumnIndex { get; }
		int BottomRowIndex { get; }
		int ColumnCount { get; }
		int RowCount { get; }
		string GetReferenceA1();
		string GetReferenceA1(ReferenceElement options);
		Range GetRangeWithAbsoluteReference();
		Range GetRangeWithRelativeReference();
		string GetReferenceR1C1(Cell baseCell);
		string GetReferenceR1C1(ReferenceElement options, Cell baseCell);
		IList<Range> GetMergedRanges();
		Range Intersect(Range other);
		bool IsIntersecting(Range other);
		Range Union(Range other);
		Style Style { get; set; }
		bool HasArrayFormula { get; }
		bool HasFormula { get; }
		Range CurrentRegion { get; }
		Range Offset(int rowCount, int columnCount);
		Color FillColor { get; set; }
		double ColumnWidth { get; set; } 
		double ColumnWidthInCharacters { get; set; } 
		double RowHeight { get; set; } 
		IList<Range> DirectPrecedents { get; }
		IList<Range> Precedents { get; }
		IList<Range> DirectDependents { get; }
		IList<Range> Dependents { get; }
		void SetInsideBorders(Color color, BorderLineStyle style);
		Formatting BeginUpdateFormatting();
		void EndUpdateFormatting(Formatting newFormatting);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		Cell this[int rowOffset, int columnOffset] { get; }
		Cell this[int position] { get; }
		bool IsMerged { get; }
		void CopyFrom(Range source); 
		void CopyFrom(Range source, PasteSpecial pasteOptions); 
		void MoveTo(Range target);
		IEnumerable<Cell> Search(string text);
		IEnumerable<Cell> Search(string text, SearchOptions options);
		IEnumerable<Cell> ExistingCells { get; }
		void Calculate();
	}
	public enum ReferenceStyle {
		UseDocumentSettings,
		A1,
		R1C1
	}
	#endregion
	public interface IRangeProvider {
		Range this[string reference] { get; }
		Range FromLTRB(int leftColumnIndex, int topRowIndex, int rightColumnIndex, int bottomRowIndex);
		Range Parse(string reference);
		Range Parse(string reference, ReferenceStyle style);
		Range Union(params Range[] ranges);
		Range Union(IEnumerable<Range> enumerable);
	}
	#region DeleteMode
	public enum DeleteMode {
		ShiftCellsLeft,
		ShiftCellsUp,
		EntireRow,
		EntireColumn
	}
	#endregion
	#region InsertCellsMode
	public enum InsertCellsMode {
		ShiftCellsDown,
		ShiftCellsRight,
		EntireRow,
		EntireColumn
	}
	#endregion
	public enum PasteSpecial {
		Formulas = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.Formulas,
		Values = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.Values,
		NumberFormats = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.NumberFormats,
		Borders = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.Borders,
		Formats = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.FormatAndStyle,
		Comments = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.Comments,
		ColumnWidths = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.ColumnWidths,
		All = DevExpress.XtraSpreadsheet.Model.ModelPasteSpecialFlags.All
	}
}
#region NativeCellRange Implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
	using ModelCellRangeBase = DevExpress.XtraSpreadsheet.Model.CellRangeBase;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelPositionType = DevExpress.XtraSpreadsheet.Model.PositionType;
	using ModelSearchOptions = DevExpress.XtraSpreadsheet.Model.ModelSearchOptions;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Spreadsheet;
	using DevExpress.Office.Utils;
	using System.Collections;
	using System.Globalization;
	#region NativeRange
	partial class NativeRange : Range, IEnumerable<Cell> {
		ModelCellRangeBase modelRange;
		NativeWorksheet nativeWorksheet;
		NativeAreasCollection nativeAreasCollection;
		public NativeRange(ModelCellRangeBase range, NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			this.modelRange = range;
			this.nativeWorksheet = nativeWorksheet;
		}
		#region Properties
		protected internal NativeWorksheet NativeWorksheet { get { return this.nativeWorksheet; } }
		protected internal Model.Worksheet ModelWorksheet { get { return NativeWorksheet.ModelWorksheet; } }
		protected internal Model.DocumentModel ModelWorkbook { get { return ModelWorksheet.Workbook; } }
		public Model.CellRangeBase ModelRange { get { return modelRange; } }
		public int LeftColumnIndex { get { return ModelRange.TopLeft.Column; } }
		public int TopRowIndex { get { return ModelRange.TopLeft.Row; } }
		public int RightColumnIndex { get { return ModelRange.BottomRight.Column; } }
		public int BottomRowIndex { get { return ModelRange.BottomRight.Row; } }
		public int Width { get { return ModelRange.Width; } }
		public int Height { get { return modelRange.Height; } }
		public AreasCollection Areas {
			get {
				if (nativeAreasCollection == null)
					nativeAreasCollection = new NativeAreasCollection(this);
				return nativeAreasCollection;
			}
		}
		#region ColumnCount
		public int ColumnCount {
			get {
				return ModelRange.BottomRight.Column - ModelRange.TopLeft.Column + 1;
			}
		}
		#endregion
		#region RowCount
		public int RowCount {
			get {
				return ModelRange.BottomRight.Row - ModelRange.TopLeft.Row + 1;
			}
		}
		#endregion
		#region Name
		public string Name {
			get {
				return NativeWorksheet.GetDefinedNameFromRange(this);
			}
			set {
				string reference = GetReferenceA1(ReferenceElement.ColumnAbsolute | ReferenceElement.RowAbsolute | ReferenceElement.IncludeSheetName);
				nativeWorksheet.DefinedNames.Add(value, reference);
			}
		}
		#endregion
		#region Worksheet
		public Worksheet Worksheet { get { return nativeWorksheet; } }
		#endregion
		#region ColumnWidthInCharacters
		public double ColumnWidthInCharacters {
			get { return NativeWorksheet.GetColumnWidthInCharacters(LeftColumnIndex); }
			set {
				this.ModelWorkbook.BeginUpdate();
				try {
					foreach (Model.CellRange innerRange in modelRange.GetAreasEnumerable()) {
						for (int i = innerRange.LeftColumnIndex; i <= innerRange.RightColumnIndex; i++)
							NativeWorksheet.Columns[i].WidthInCharacters = value;
					}
				}
				finally {
					this.ModelWorkbook.EndUpdate();
				}
			}
		}
		#endregion
		#region ColumnWidth
		public double ColumnWidth {
			get { return NativeWorksheet.GetColumnWidth(LeftColumnIndex); }
			set {
				this.ModelWorkbook.BeginUpdate();
				try {
					foreach (Model.CellRange innerRange in modelRange.GetAreasEnumerable()) {
						for (int i = innerRange.LeftColumnIndex; i <= innerRange.RightColumnIndex; i++)
							NativeWorksheet.Columns[i].Width = value;
					}
				}
				finally {
					this.ModelWorkbook.EndUpdate();
				}
			}
		}
		#endregion
		#region Formula
		public string Formula {
			get {
				NativeRangeCellsFormulaAccessor accessor = new NativeRangeCellsFormulaAccessor(ModelRange);
				return accessor.GetValue();
			}
			set {
				string formula = value;
				if (string.IsNullOrEmpty(formula)) {
					foreach (Model.CellBase cellInfo in modelRange.GetExistingCellsEnumerable()) {
						Model.ICell cell = cellInfo as Model.ICell;
						if (cell != null && cell.HasFormula) {
							Model.CellContentSnapshot snapshot = new Model.CellContentSnapshot(cell);
							cell.ClearContent();
							cell.DocumentModel.RaiseCellValueChangedProgrammatically(snapshot);
						}
					}
					return;
				}
				if (!formula.StartsWith("=", StringComparison.Ordinal))
					formula = "=" + formula;
				if (RowCount == 1 && ColumnCount == 1)
					this[0].Formula = formula;
				else
					ModelWorksheet.LocateFormulaToMultipleCells(ModelRange, ModelRange.TopLeft, formula, true);
			}
		}
		#endregion
		#region FormulaInvariant
		public string FormulaInvariant {
			get {
				Model.WorkbookDataContext context = ModelWorkbook.DataContext;
				context.PushCulture(CultureInfo.InvariantCulture);
				try {
					return Formula;
				}
				finally {
					context.PopCulture();
				}
			}
			set {
				Model.WorkbookDataContext context = ModelWorkbook.DataContext;
				context.PushCulture(CultureInfo.InvariantCulture);
				try {
					Formula = value;
				}
				finally {
					context.PopCulture();
				}
			}
		}
		#endregion
		#region ArrayFormula
		public string ArrayFormula {
			get {
				NativeRangeArrayFormulaAccessor accessor = new NativeRangeArrayFormulaAccessor(ModelRange.GetFirstInnerCellRange());
				return accessor.GetValue();
			}
			set {
				NativeRangeArrayFormulaAccessor accessor = new NativeRangeArrayFormulaAccessor(ModelRange.GetFirstInnerCellRange());
				accessor.SetValue(value);
			}
		}
		#endregion
		#region ArrayFormulaInvariant
		public string ArrayFormulaInvariant {
			get {
				Model.WorkbookDataContext context = ModelWorkbook.DataContext;
				context.PushCulture(CultureInfo.InvariantCulture);
				try {
					return ArrayFormula;
				}
				finally {
					context.PopCulture();
				}
			}
			set {
				Model.WorkbookDataContext context = ModelWorkbook.DataContext;
				context.PushCulture(CultureInfo.InvariantCulture);
				try {
					ArrayFormula = value;
				}
				finally {
					context.PopCulture();
				}
			}
		}
		#endregion
		#region Value
		public CellValue Value {
			get {
				NativeRangeCellsValueAccessor accessor = new NativeRangeCellsValueAccessor(ModelRange);
				Model.VariantValue modelValue = accessor.GetValue();
				if (modelValue == Model.VariantValue.Missing)
					return null; 
				return new CellValue(modelValue, nativeWorksheet.ModelWorkbook.DataContext);
			}
			set {
				NativeRangeCellsValueAccessor accessor = new NativeRangeCellsValueAccessor(ModelRange);
				if (value == null)
					accessor.SetValue(DevExpress.XtraSpreadsheet.Model.VariantValue.Empty);
				else {
					accessor.SetAsDateTime = value.IsDateTime;
					accessor.SetValue(value.ModelVariantValue);
				}
			}
		}
		#endregion
		#region RowHeight
		public double RowHeight {
			get {
				return NativeWorksheet.Rows[TopRowIndex].Height;
			}
			set {
				SetHeight(value);
			}
		}
		#endregion
		#region Precedents
		public IList<Range> Precedents {
			get {
				Model.CellRangeBase precedentsRange = ModelWorkbook.CalculationChain.GetPrecedents(ModelRange, true);
				return ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> DirectPrecedents {
			get {
				Model.CellRangeBase precedentsRange = ModelWorkbook.CalculationChain.GetDirectPrecedents(ModelRange, true);
				return ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> Dependents {
			get {
				Model.CellRangeBase dependentsRange = ModelWorkbook.CalculationChain.GetDependents(ModelRange, true);
				return ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, dependentsRange);
			}
		}
		public IList<Range> DirectDependents {
			get {
				Model.CellRangeBase dependentsRange = ModelWorkbook.CalculationChain.GetDirectDependents(ModelRange, true);
				return ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, dependentsRange);
			}
		}
		protected internal static IList<Range> ConvertModelCellRangeBaseToListOfApiRanges(NativeWorkbook workbook, Model.CellRangeBase modelRange) {
			if (modelRange == null)
				return new List<Range>();
			List<Range> result = new List<Range>();
			foreach (ModelCellRange innerRange in modelRange.GetAreasEnumerable()) {
				NativeWorksheet nativeWorksheet = (NativeWorksheet)workbook.NativeWorksheets[innerRange.Worksheet.Name];
				result.Add(new NativeRange(innerRange, nativeWorksheet));
			}
			return result;
		}
		#endregion
		#region Calculate
		public void Calculate() {
			ModelWorkbook.CalculationChain.CalculateRange(ModelRange);
		}
		#endregion
		public bool IsMerged { get { return ModelRange.IsMerged; } }
		#region Item
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		public Cell this[int rowIndex, int columnIndex] {
			get {
				return GetCell(rowIndex, columnIndex, TopRowIndex, LeftColumnIndex);
			}
		}
		public Cell this[int position] {
			get {
				if (position < 0 || position >= this.modelRange.CellCount)
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorCellIndexInRangeOutOfRange, "index");
				if (this.Areas.Count > 0) {
					long localPosition = position;
					foreach (ModelCellRange innerRange in modelRange.GetAreasEnumerable()) {
						if (localPosition >= innerRange.CellCount)
							localPosition -= innerRange.CellCount;
						else
							return GetCellByIndex(localPosition, innerRange.Width, innerRange.TopRowIndex, innerRange.LeftColumnIndex);
					}
				}
				return GetCellByIndex(position, ColumnCount, TopRowIndex, LeftColumnIndex);
			}
		}
		Cell GetCellByIndex(long position, int columnCount, int topRowIndex, int leftColumnIndex) {
			int row = (int)(position / columnCount);
			int column = (int)(position % columnCount);
			return GetCell(row, column, topRowIndex, leftColumnIndex);
		}
		Cell GetCell(int rowIndex, int columnIndex, int topRowIndex, int leftColumnIndex) {
			int absoluteRowIndex = topRowIndex + rowIndex;
			int absoluteColumnIndex = leftColumnIndex + columnIndex;
			ModelCellPosition position = new ModelCellPosition(absoluteColumnIndex, absoluteRowIndex);
			if (!IndicesChecker.CheckIsColumnIndexValid(absoluteColumnIndex))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorColumnOffsetRefersBeyondWorksheet);
			if (!IndicesChecker.CheckIsRowIndexValid(absoluteRowIndex))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorRowOffsetRefersBeyondWorksheet);
			return NativeWorksheet.GetCell(position.Row, position.Column);
		}
		#endregion
		public bool HasArrayFormula {
			get {
				foreach (ArrayFormula arrayFormula in NativeWorksheet.ArrayFormulas)
					if (arrayFormula.Range.IsIntersecting(this))
						return true;
				return false;
			}
		}
		public bool HasFormula {
			get {
				NativeRangeCellsHasFormulaAccessor accessor = new NativeRangeCellsHasFormulaAccessor(ModelRange);
				return accessor.GetValue();
			}
		}
		public Range CurrentRegion {
			get {
				Model.CellRange result = ModelWorksheet.GetCurrentRegion(modelRange);
				return new NativeRange(result, NativeWorksheet);
			}
		}
		public void SetValue(object value) {
			this.Value = CellValue.FromObject(value);
		}
		#region Style
		public Style Style {
			get { return NativeRangeStyleAccessor.GetStyle(this); }
			set { NativeRangeStyleAccessor.SetStyle(this, value); }
		}
		#endregion
		#region Formatting
		NativeRangeFormat Format { get { return new NativeRangeFormat(this); } }
		string Formatting.NumberFormat { get { return Format.NumberFormat; } set { Format.NumberFormat = value; } }
		Alignment Formatting.Alignment { get { return Format.Alignment; } }
		SpreadsheetFont Formatting.Font { get { return Format.Font; } }
		Borders Formatting.Borders { get { return Format.Borders; } }
		Fill Formatting.Fill { get { return Format.Fill; } }
		Protection Formatting.Protection { get { return Format.Protection; } }
		StyleFlags Formatting.Flags { get { return Format.Flags; } }
		bool Formatting.Equals(object other) {
			NativeRange otherRange = other as NativeRange;
			if (otherRange != null)
				return ModelRange.Equals(otherRange.ModelRange);
			return Format.Equals(other);
		}
		void Formatting.BeginUpdate() {
			Format.BeginUpdate();
		}
		void Formatting.EndUpdate() {
			Format.EndUpdate();
		}
		#endregion
		#region FillColor
		public Color FillColor {
			get {
				return Format.Fill.BackgroundColor;
			}
			set {
				value = value.RemoveTransparency();
				Formatting style = this.BeginUpdateFormatting();
				try {
					style.Fill.BackgroundColor = value;
				}
				finally {
					this.EndUpdateFormatting(style);
				}
			}
		}
		#endregion
		#region NumberFormat
		public string NumberFormat {
			get {
				return Format.NumberFormat;
			}
			set {
				Formatting style = this.BeginUpdateFormatting();
				try {
					style.NumberFormat = value;
				}
				finally {
					this.EndUpdateFormatting(style);
				}
			}
		}
		#endregion
		#endregion
		#region Intersect
		public Range Intersect(Range rangeBase) {
			Model.VariantValue calculatedResult = modelRange.IntersectionWith(nativeWorksheet.GetModelRange(rangeBase));
			if (calculatedResult.Type == Model.VariantValueType.Error)
				return null;
			if (!calculatedResult.IsCellRange)
				throw new InvalidOperationException();
			return new NativeRange(calculatedResult.CellRangeValue, NativeWorksheet);
		}
		#endregion
		#region IsIntersecting
		public bool IsIntersecting(Range rangeBase) {
			return modelRange.Intersects(nativeWorksheet.GetModelRange(rangeBase));
		}
		#endregion
		#region Union
		public Range Union(Range other) {
			if (other == null)
				return new NativeRange(modelRange, nativeWorksheet);
			return NativeRangeReferenceParseHelper.CreateUnion(this, other);
		}
		#endregion
		#region Offset
		public Range Offset(int rowCount, int columnCount) {
			ModelCellRangeBase shifted = modelRange.GetShiftedAny(new Model.CellPositionOffset(columnCount, rowCount), modelRange.Worksheet);
			if (shifted == null)
				throw new ArgumentOutOfRangeException();
			return new NativeRange(shifted, nativeWorksheet);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			NativeRange other = obj as NativeRange;
			if (other != null) {
				return modelRange.Equals(other.ModelRange);
			}
			else
				return false;
		}
		#endregion
		#region GetHashCode
		public override int GetHashCode() {
			return modelRange.GetHashCode();
		}
		#endregion
		#region IEnumerable<Cell>.GetEnumerator
		IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() {
			IEnumerator<Model.CellKey> enumerator = modelRange.GetAllCellKeysEnumerator();
			while (enumerator.MoveNext()) {
				yield return NativeWorksheet.GetCell(enumerator.Current);
			}
		}
		#endregion
		#region System.Collections.IEnumerable.GetEnumerator
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return (this as IEnumerable<Cell>).GetEnumerator();
		}
		#endregion
		public Range GetRangeWithAbsoluteReference() {
			ModelCellRangeBase newRange = this.modelRange.GetWithModifiedPositionType(ModelPositionType.Absolute);
			return new NativeRange(newRange, this.nativeWorksheet);
		}
		public Range GetRangeWithRelativeReference() {
			ModelCellRangeBase newRange = this.modelRange.GetWithModifiedPositionType(ModelPositionType.Relative);
			return new NativeRange(newRange, this.nativeWorksheet);
		}
		#region GetReference
		public string GetReferenceA1() {
			return ModelRange.ToString();
		}
		public string GetReferenceA1(ReferenceElement options) {
			return GetReferenceCommon(options, false, null);
		}
		public string GetReferenceR1C1(ReferenceElement options, Cell baseCell) {
			return GetReferenceCommon(options, true, baseCell);
		}
		public string GetReferenceR1C1(Cell cell) {
			return GetReferenceCommonCore(true, cell, ModelPositionType.Relative, ModelPositionType.Relative, false);
		}
		string GetReferenceCommon(ReferenceElement options, bool isR1C1Style, Cell baseCell) {
			ModelPositionType rowPositionType = options.HasFlag(ReferenceElement.RowAbsolute) ? ModelPositionType.Absolute : ModelPositionType.Relative;
			ModelPositionType columnPositionType = options.HasFlag(ReferenceElement.ColumnAbsolute) ? ModelPositionType.Absolute : ModelPositionType.Relative;
			bool includeSheetName = options.HasFlag(ReferenceElement.IncludeSheetName);
			return GetReferenceCommonCore(isR1C1Style, baseCell, rowPositionType, columnPositionType, includeSheetName);
		}
		string GetReferenceCommonCore(bool isR1C1Style, Cell baseCell, ModelPositionType rowPositionType, ModelPositionType columnPositionType, bool includeSheetName) {
			Model.CellPosition modelCellPosition = baseCell != null ? ((NativeCell)baseCell).ReadOnlyModelCell.Position : Model.CellPosition.InvalidValue;
			return Model.CellRangeToString.GetReferenceCommon(ModelRange, isR1C1Style, modelCellPosition, rowPositionType, columnPositionType, includeSheetName);
		}
		#endregion
		#region CopyFrom
		public void CopyFrom(Range source) {
			this.CopyFrom(source, PasteSpecial.All);
		}
		public void CopyFrom(Range source, PasteSpecial pasteType) {
			nativeWorksheet.CopyRange(source, ModelRange, pasteType);
		}
		#endregion
		public void MoveTo(Range target) {
			nativeWorksheet.MoveRangeTo(ModelRange, target);
		}
		#region BeginUpdateFormatting
		public Formatting BeginUpdateFormatting() {
			ModelWorkbook.BeginUpdate(); 
			return Format;
		}
		#endregion
		#region EndUpdateFormatting
		public void EndUpdateFormatting(Formatting newFormatting) {
			Guard.ArgumentNotNull(newFormatting, "newFormatting");
			ModelWorkbook.EndUpdate();
		}
		#endregion
		#region SetInsideBorders
		public void SetInsideBorders(Color color, BorderLineStyle lineStyle) {
			NativeRangeFormat formatting = this.BeginUpdateFormatting() as NativeRangeFormat;
			(formatting.Borders as NativeRangeFormatBordersPart).SetInsideRangeBorders(color, lineStyle);
			this.EndUpdateFormatting(formatting);
		}
		#endregion
		#region GetMergedRanges
		public IList<Range> GetMergedRanges() {
			List<Model.CellRange> modelRanges = nativeWorksheet.ModelWorksheet.MergedCells.GetMergedCellRangesIntersectsRange(ModelRange);
			List<Range> result = new List<Range>();
			foreach (Model.CellRange modelRange in modelRanges)
				result.Add(new NativeRange(modelRange, nativeWorksheet));
			return result;
		}
		#endregion
		void SetHeight(double value) {
			this.ModelWorkbook.BeginUpdate();
			try {
				Model.CellIntervalRange modelIntervalRange = modelRange as Model.CellIntervalRange;
				bool isColumnInterval = modelIntervalRange != null && modelIntervalRange.IsColumnInterval;
				bool isWholeWorksheetInterval = modelIntervalRange != null && modelIntervalRange.IsWholeWorksheetRange();
				if (isColumnInterval || isWholeWorksheetInterval) {
					nativeWorksheet.DefaultRowHeight = value;
					int top = modelRange.TopLeft.Row;
					int bottom = modelRange.BottomRight.Row;
					foreach (Model.Row row in modelRange.Worksheet.Rows.GetExistingRows(top, bottom, false)) 
						row.SetCustomHeight(nativeWorksheet.NativeWorkbook.UnitsToModelUnitsF((float)value));
				}
				else {
					foreach (Model.CellRange innerRange in modelRange.GetAreasEnumerable()) {
						for (int index = innerRange.TopRowIndex; index < innerRange.BottomRowIndex + 1; index++) {
							Row row = NativeWorksheet.Rows[index];
							row.Height = value;
						}
					}
				}
			}
			finally {
				this.ModelWorkbook.EndUpdate();
			}
		}
		public IEnumerable<Cell> Search(string text) {
			ModelSearchOptions modelOptions = nativeWorksheet.CreateDefaultSearchOptions();
			modelOptions.Range = modelRange.Clone();
			return nativeWorksheet.SearchCore(text, modelOptions);
		}
		public IEnumerable<Cell> Search(string text, SearchOptions options) {
			ModelSearchOptions modelOptions = nativeWorksheet.ConvertSearchOptions(options);
			modelOptions.Range = modelRange.Clone();
			return nativeWorksheet.SearchCore(text, modelOptions);
		}
		public override string ToString() {
			return String.Format("Range: \"{0}\", worksheet:\"{1}\"", ModelRange.ToString(), Worksheet.Name);
		}
		#region ExistingCells
		public IEnumerable<Cell> ExistingCells {
			get {
				IEnumerator<Model.ICellBase> modelEnumerator = modelRange.GetExistingCellsEnumerator(false);
				return new Enumerable<Cell>(new EnumeratorConverter<Model.ICellBase, Cell>(modelEnumerator, ConvertModelCellToApiCell));
			}
		}
		Cell ConvertModelCellToApiCell(Model.ICellBase cell) {
			return nativeWorksheet[cell.RowIndex, cell.ColumnIndex];
		}
		#endregion
	}
	#endregion
	#region NativeAreasCollection
	partial class NativeAreasCollection : AreasCollection {
		readonly NativeRange nativeRange;
		public NativeAreasCollection(NativeRange nativeRange) {
			this.nativeRange = nativeRange;
		}
		public int Count { get { return nativeRange.ModelRange.AreasCount; } }
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return this; } }
		public Range this[int index] {
			get {
				int count = Count;
				if (index < 0 || index >= count)
					throw new ArgumentOutOfRangeException();
				if (count == 1)
					return nativeRange;
				ModelCellRange modelArea = nativeRange.ModelRange.GetAreaByIndex(index);
				return new NativeRange(modelArea, nativeRange.NativeWorksheet);
			}
		}
		public IEnumerator<Range> GetEnumerator() {
			ModelCellRangeBase modelRange = nativeRange.ModelRange;
			IEnumerator<ModelCellRange> enumerator = modelRange.GetAreasEnumerable().GetEnumerator();
			NativeWorksheet nativeWorksheet = nativeRange.NativeWorksheet;
			while (enumerator.MoveNext()) {
				yield return new NativeRange(enumerator.Current, nativeWorksheet);
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<Range>)this).GetEnumerator();
		}
		public void CopyTo(Array array, int index) {
			if (array == null)
				throw new ArgumentNullException("array");
			if (array.Rank != 1)
				throw new ArgumentException("A multidimensional array is not supported");
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException("An array with a non-zero lower bond is not supported");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException();
			if (array.Length - index < Count)
				throw new ArgumentOutOfRangeException();
			Range[] rangeArray = array as Range[];
			if (rangeArray != null) {
				IEnumerator<Range> enumerator = GetEnumerator();
				while (enumerator.MoveNext()) {
					rangeArray[index++] = enumerator.Current;
				}
			}
			else {
				object[] objects = array as object[];
				if (objects == null)
					throw new ArgumentException("Invalid array type");
				try {
					IEnumerator<Range> enumerator = GetEnumerator();
					while (enumerator.MoveNext()) {
						rangeArray[index++] = enumerator.Current;
					}
				}
				catch (ArrayTypeMismatchException) {
					throw new ArgumentException("Invalid array type");
				}
			}
		}
	}
	#endregion
	#region NativeRangeStyleAccessor
	internal static class NativeRangeStyleAccessor {
		public static void SetStyle(NativeRange range, Style newStyle) {
			NativeCellStyle nativeCellStyle = (newStyle as NativeCellStyle);
			if (nativeCellStyle == null)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseInvalidStyle);
			if (!Object.ReferenceEquals(nativeCellStyle.ModelCellStyle.DocumentModel, range.ModelWorkbook))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseStyleFromAnotherWorkbook);
			range.ModelWorkbook.BeginUpdate();
			try {
				Model.CellRangeBase modelRange = range.ModelRange;
				if (modelRange.RangeType == Model.CellRangeType.IntervalRange) {
					Model.CellIntervalRange intervalRange = modelRange as Model.CellIntervalRange;
					Model.CellStyleBase modelStyle = nativeCellStyle.ModelCellStyle;
					if (intervalRange.IsColumnInterval)
						ProcessIntervalRangeColumn(range, intervalRange, modelStyle);
					else
						ProcesIntervalRangeRow(range, intervalRange, modelStyle);
					return;
				}
				foreach (Cell cell in range) {
					cell.Style = newStyle;
				}
			}
			finally {
				range.ModelWorkbook.EndUpdate();
			}
		}
		static void ProcessIntervalRangeColumn(NativeRange nativeRange, Model.CellIntervalRange intervalRange, Model.CellStyleBase modelStyle) {
			int startColumnIndex = intervalRange.TopLeft.Column;
			int endColumnIndex = intervalRange.BottomRight.Column;
			for (int columnIndex = startColumnIndex; columnIndex < endColumnIndex + 1; columnIndex++) {
				Model.Column column = nativeRange.ModelWorksheet.Columns.GetIsolatedColumn(columnIndex);
				try {
					column.Style = modelStyle;
				}
				finally {
				}
				Model.Worksheet worksheet = intervalRange.Worksheet as Model.Worksheet;
				foreach (Model.Row row in worksheet.Rows.GetExistingRows(0, nativeRange.ModelWorksheet.MaxRowCount, false)) {
					Model.ICell cellInRow = row.Cells.TryGetCell(columnIndex);
					if (cellInRow != null)
						cellInRow.Style = modelStyle;
					else {
						if (row.FormatIndex > worksheet.Workbook.StyleSheet.DefaultCellFormatIndex)
							row.Cells[columnIndex].Style = modelStyle;
					}
				}
			}
		}
		static void ProcesIntervalRangeRow(NativeRange nativeRange, Model.CellIntervalRange intervalRange, Model.CellStyleBase modelStyle) {
			int startRowIndex = intervalRange.TopLeft.Row;
			int endRowIndex = intervalRange.BottomRight.Row;
			for (int rowIndex = startRowIndex; rowIndex < endRowIndex + 1; rowIndex++) {
				Model.Row row = nativeRange.ModelWorksheet.Rows[rowIndex];
				try {
					row.BeginUpdate();
					row.Style = modelStyle;
				}
				finally {
					row.EndUpdate();
				}
				foreach (Model.ICell cell in row.Cells.GetExistingCells(0, nativeRange.ModelWorksheet.MaxColumnCount, false)) {
					cell.Style = modelStyle;
				}
			}
		}
		public static Style GetStyle(NativeRange range) {
			int styleIndex = Int32.MinValue;
			Model.CellRangeBase modelRange = range.ModelRange;
			if (modelRange.RangeType == Model.CellRangeType.IntervalRange) {
				Model.CellIntervalRange intervalRange = modelRange as Model.CellIntervalRange;
				if (intervalRange.IsColumnInterval)
					styleIndex = GetStyleIndexFromColumn(range, intervalRange);
				else
					styleIndex = GetStyleIndexFromRow(range, intervalRange);
			}
			int nonEmptyCount = 0;
			foreach (Model.CellBase info in range.ModelRange.GetExistingCellsEnumerable()) {
				Model.ICell cell = info as Model.ICell;
				int cellStyleIndex = cell.FormatInfo.StyleIndex;
				if (styleIndex == Int32.MinValue)
					styleIndex = cellStyleIndex;
				if (styleIndex != cellStyleIndex)
					return null;
				nonEmptyCount++;
			}
			if (styleIndex == Int32.MinValue)
				styleIndex = 0;
			return range.Worksheet.Workbook.Styles[styleIndex];
		}
		static int GetStyleIndexFromColumn(NativeRange nativeRange, Model.CellIntervalRange intervalRange) {
			int result = Int32.MinValue;
			int startColumnIndex = intervalRange.TopLeft.Column;
			int endColumnIndex = intervalRange.BottomRight.Column;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++) {
				Model.IColumnRange column = nativeRange.ModelWorksheet.Columns.GetReadonlyColumnRange(columnIndex);
				if (result == Int32.MinValue)
					result = column.StyleIndex;
				if (result != column.StyleIndex)
					return Int32.MinValue;
			}
			return result;
		}
		static int GetStyleIndexFromRow(NativeRange nativeRange, Model.CellIntervalRange intervalRange) {
			int result = Int32.MinValue;
			int startRowIndex = intervalRange.TopLeft.Row;
			int endRowIndex = intervalRange.BottomRight.Row;
			for (int rowIndex = startRowIndex; rowIndex < endRowIndex + 1; rowIndex++) {
				Model.Row row = nativeRange.ModelWorksheet.Rows[rowIndex];
				if (result == Int32.MinValue)
					result = row.FormatInfo.StyleIndex;
				if (result != row.FormatInfo.StyleIndex)
					return Int32.MinValue;
			}
			return result;
		}
	}
	#endregion
	#region NativeRangeArrayFormulaAccessor
	partial class NativeRangeArrayFormulaAccessor {
		Model.CellRange modelRange;
		public NativeRangeArrayFormulaAccessor(Model.CellRange modelRange) {
			this.modelRange = modelRange;
		}
		Model.CellRange ModelRange { get { return this.modelRange; } }
		Model.Worksheet ModelWorksheet { get { return this.modelRange.Worksheet as Model.Worksheet; } }
		public string GetValue() {
			Model.ICell hostCell = ModelRange.GetFirstCellUnsafe() as Model.ICell;
			if (hostCell == null)
				return string.Empty;
			if (modelRange.Height == 1 && modelRange.Width == 1) {
				return GetFormulaFromSingleCell(hostCell);
			}
			int index = ModelWorksheet.ArrayFormulaRanges.FindArrayFormulaIndexExactRange(ModelRange);
			if (index >= 0) {
				ModelCellRange formulaRange = ModelWorksheet.ArrayFormulaRanges[index];
				if (formulaRange.Equals(ModelRange))
					return ModelWorksheet.ArrayFormulaRanges.GetArrayFormulaByRangeIndex(index).GetBody(hostCell);
			}
			return String.Empty;
		}
		string GetFormulaFromSingleCell(Model.ICell modelCell) {
			if (modelCell.HasFormula) {
				Model.FormulaBase modelCellFormula = modelCell.GetFormula();
				Model.ArrayFormula arrayFormula = modelCellFormula as Model.ArrayFormula;
				Model.ArrayFormulaPart arrayFormulaPart = modelCellFormula as Model.ArrayFormulaPart;
				if (arrayFormula != null || arrayFormulaPart != null)
					return modelCell.FormulaBody;
			}
			return String.Empty;
		}
		public void SetValue(string value) {
			if (String.IsNullOrEmpty(value))
				ModelWorksheet.RemoveArrayFormulasAndValuesFrom(ModelRange);
			else {
				if (!value.StartsWith("=", StringComparison.Ordinal))
					value = "=" + value;
				ModelWorksheet.CreateArray(value, ModelRange);
			}
		}
	}
	#endregion
	#region NativeRangeCellsFormulaAccessor
	partial class NativeRangeCellsFormulaAccessor : RangeCellsPropertyAccessor<string> {
		public NativeRangeCellsFormulaAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		#region CellFormulaStringPropertyModifier
		class CellFormulaStringPropertyModifier : Model.CellFormatPropertyModifier<string> {
			public CellFormulaStringPropertyModifier(string val)
				: base(val) {
			}
			public override void ModifyFormat(Model.ICell cell) {
				cell.SetFormulaBody(NewValue);
			}
			public override string GetFormatPropertyValue(Model.ICell cell) {
				return cell.FormulaBody;
			}
			public override string GetFormatPropertyValue(Model.FormatBase format) {
				throw new InvalidOperationException();
			}
		}
		#endregion
		protected internal override string CalculateValueCore(Model.ICell cell, DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier) {
			CellFormulaStringPropertyModifier typedModifier = (CellFormulaStringPropertyModifier)modifier;
			return typedModifier.GetFormatPropertyValue(cell);
		}
		protected internal override string CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier) {
			throw new InvalidOperationException();
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase CreateModifier(string newValue) {
			return new CellFormulaStringPropertyModifier(newValue);
		}
		protected override string GetDefaultValue() {
			return string.Empty;
		}
		protected override string GetValueForDifferentValues() {
			return null;
		}
	}
	#endregion
	#region NativeRangeCellsHasFormulaAccessor
	partial class NativeRangeCellsHasFormulaAccessor : RangeCellsPropertyAccessor<bool> {
		public NativeRangeCellsHasFormulaAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override bool CalculateValueCore(Model.ICell cell, Model.CellFormatPropertyModifierBase modifier) {
			return cell.HasFormula;
		}
		protected internal override bool CalculateValueCore(Model.FormatBase format, Model.CellFormatPropertyModifierBase modifier) {
			throw new InvalidOperationException();
		}
		protected internal override Model.CellFormatPropertyModifierBase CreateModifier(bool newValue) {
			return null;
		}
		protected override bool GetDefaultValue() {
			return false;
		}
		protected override bool GetValueForDifferentValues() {
			return false;
		}
	}
	#endregion
	#region NativeRangeCellsValueAccessor
	partial class NativeRangeCellsValueAccessor : RangeMergedCellsPropertyAccessor<Model.VariantValue> {
		public NativeRangeCellsValueAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		public bool SetAsDateTime { get; set; }
		#region CellValuePropertyModifier
		class CellValuePropertyModifier : Model.CellFormatPropertyModifier<Model.VariantValue> {
			public bool SetAsDateTime { get; set; }
			public CellValuePropertyModifier(Model.VariantValue val)
				: base(val) {
			}
			public override void ModifyFormat(Model.ICell cell) {
				Model.CellContentSnapshot snapshot = new Model.CellContentSnapshot(cell);
				cell.Value = NewValue;
				if (SetAsDateTime && string.IsNullOrEmpty(cell.ActualFormatString))
					cell.FormatString = DevExpress.XtraSpreadsheet.Model.ShortDateNumberFormat.Instance.GetActualFormat(CultureInfo.InvariantCulture).FormatCode;
				cell.DocumentModel.RaiseCellValueChangedProgrammatically(snapshot);
			}
			public override Model.VariantValue GetFormatPropertyValue(Model.ICell cell) {
				return cell.Value;
			}
			public override Model.VariantValue GetFormatPropertyValue(Model.FormatBase format) {
				throw new InvalidOperationException();
			}
		}
		#endregion
		protected internal override Model.VariantValue CalculateValueCore(Model.ICell cell, DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier) {
			CellValuePropertyModifier typedModifier = (CellValuePropertyModifier)modifier;
			return typedModifier.GetFormatPropertyValue(cell);
		}
		protected internal override Model.VariantValue CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier) {
			throw new InvalidOperationException();
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase CreateModifier(Model.VariantValue newValue) {
			CellValuePropertyModifier result = new CellValuePropertyModifier(newValue);
			result.SetAsDateTime = this.SetAsDateTime;
			return result;
		}
		protected override Model.VariantValue GetDefaultValue() {
			return Model.VariantValue.Empty; 
		}
		protected override Model.VariantValue GetValueForDifferentValues() {
			return Model.VariantValue.Missing; 
		}
	}
	#endregion
}
#endregion
