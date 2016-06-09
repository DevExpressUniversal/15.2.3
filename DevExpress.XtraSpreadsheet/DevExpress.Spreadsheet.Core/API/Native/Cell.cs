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
using System.ComponentModel;
using DevExpress.Compatibility.System;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region CellKey
	[Serializable, System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential), System.Runtime.InteropServices.ComVisible(false)]
	public struct CellKey : IComparable<CellKey>, IEquatable<CellKey> {
		readonly long m_value;
		[System.Diagnostics.DebuggerStepThrough]
		public CellKey(int sheetId, int columnIndex, int rowIndex) {
			m_value = ((long)sheetId << 34) + ((long)columnIndex << 20) + rowIndex;
		}
		internal CellKey(DevExpress.XtraSpreadsheet.Model.CellKey modelKey) {
			m_value = modelKey.ToLong();
		}
		public int RowIndex { get { return (int)(m_value & 0x000FFFFF); } } 
		public int ColumnIndex { get { return (int)((m_value >> 20) & 0x00003FFF); } } 
		public int SheetId { get { return (int)(m_value >> 34); } }
		public override bool Equals(object obj) {
			if (!(obj is CellKey))
				return false;
			CellKey other = (CellKey)obj;
			return this.m_value == other.m_value;
		}
		public bool Equals(CellKey other) {
			return this.m_value == other.m_value;
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return String.Format("Column={0}, Row={1}, SheetId={2}, m_value={3}", ColumnIndex, RowIndex, this.SheetId, m_value);
		}
		public static bool operator ==(CellKey index1, CellKey index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(CellKey index1, CellKey index2) {
			return index1.m_value != index2.m_value;
		}
		public long ToLong() {
			return m_value;
		}
		#region IComparable<CellKey> Members
		public int CompareTo(CellKey other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region Cell
	public interface Cell : Range {
		int RowIndex { get; }
		int ColumnIndex { get; }
		bool IsTopLeftCellInArrayFormulaRange { get; }
		Range GetArrayFormulaRange();
		string DisplayText { get; }
		bool IsDisplayedAsDateTime { get; }
		bool HasQuotePrefix { get; }
		object Tag { get; set; }
		DevExpress.Spreadsheet.Formulas.ParsedExpression ParsedExpression { get; set; }
	}
	#endregion
	#region CellCollection
	public interface CellCollection : Range {
		Cell this[string position] { get; }
	}
	#endregion
}
#region NativeCell Implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using Model = DevExpress.XtraSpreadsheet.Model;
	using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelVariantValue = DevExpress.XtraSpreadsheet.Model.VariantValue;
	using ModelArrayFormula = DevExpress.XtraSpreadsheet.Model.ArrayFormula;
	using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
	using ModelSearchOptions = DevExpress.XtraSpreadsheet.Model.ModelSearchOptions;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Collections.Generic;
	using DevExpress.Spreadsheet;
	using System.Diagnostics;
	using DevExpress.Office.Utils;
	using System.Globalization;
	using DevExpress.Compatibility.System.Drawing;
	#region NativeCellContent
	partial class NativeCellContent : IFormatBaseAccessor {
		Model.ICell modelCell;
		ModelCellKey key;
		readonly NativeWorksheet worksheet;
		public NativeCellContent(ModelCellKey key, NativeWorksheet worksheet) {
			Guard.ArgumentNotNull(key, "key");
			this.key = key;
			this.worksheet = worksheet;
		}
		#region IFormatBaseAccessor Members
		public Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get { return ReadOnlyModelCell; } }
		public Model.IFormatBaseBatchUpdateable ReadWriteFormat { get { return ReadWriteModelCell; } }
		#endregion
		#region ReadWriteModelCell
		public Model.ICell ReadWriteModelCell {
			get {
				if (modelCell == null) {
					modelCell = worksheet.GetModelCell(key);
					return modelCell;
				}
				if (modelCell.Key != key)
					modelCell = worksheet.GetModelCell(key);
				return modelCell;
			}
		}
		#endregion
		#region ReadOnlyModelCell
		public Model.ICell ReadOnlyModelCell {
			get {
				if (modelCell == null)
					return worksheet.GetReadOnlyModelCell(key);
				if (modelCell.Key != key) {
					modelCell = null;
					return worksheet.GetReadOnlyModelCell(key);
				}
				return modelCell;
			}
		}
		#endregion
		#region Value
		public CellValue Value {
			get {
				Model.ICell readOnlyCell = ReadOnlyModelCell;
				ModelVariantValue modelValue = readOnlyCell.Value;
				if (modelValue.IsError)
					return CellValue.GetNativeErrorValue(modelValue.ErrorValue.Type);
				return new CellValue(modelValue, readOnlyCell.Context);
			}
			set {
				Model.ICell cell = ReadWriteModelCell;
				Model.CellContentSnapshot snapshot = null; 
				if(worksheet.ModelWorkbook.EventOptions.RaiseOnModificationsViaAPI)
					snapshot = new Model.CellContentSnapshot(cell);
				if (value == null)
					cell.Value = ModelVariantValue.Empty;
				else
					cell.Value = value.ModelVariantValue;
				RaiseCellValueChanged(snapshot);
			}
		}
		#endregion
		#region HasQuotePrefix
		public bool HasQuotePrefix {
			get {
				Model.ICell readOnlyCell = ReadOnlyModelCell;
				return readOnlyCell.QuotePrefix;
			}
		}
		#endregion
		#region Formula
		public string Formula {
			get {
				return ReadOnlyModelCell.FormulaBody;
			}
			set {
				string formulaString = value;
				if (string.IsNullOrEmpty(formulaString)) {
					if (ReadOnlyModelCell.HasFormula) {
						Model.CellContentSnapshot snapshot = new Model.CellContentSnapshot(ReadOnlyModelCell);
						ReadWriteModelCell.ClearContent();
						RaiseCellValueChanged(snapshot);
					}
					return;
				}
				if (!formulaString.StartsWith("=", StringComparison.Ordinal))
					formulaString = "=" + formulaString;
				Model.Formula formula = null;
				try {
					formula = new Model.Formula(ReadWriteModelCell, formulaString);
				}
				catch (ArgumentException e) {
					System.Globalization.CultureInfo culture = ReadOnlyModelCell.Context.Culture;
					string cultureName = culture == System.Globalization.CultureInfo.InvariantCulture ? "Invariant" : culture.Name;
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula), formulaString, cultureName);
					throw new ArgumentException(message, e);
				}
				SetFormula(formula);
			}
		}
		void SetFormula(Model.Formula formula) {
			Model.CellContentSnapshot snapshot = new Model.CellContentSnapshot(ReadOnlyModelCell);
			ReadWriteModelCell.SetFormula(formula);
			RaiseCellValueChanged(snapshot);
		}
		#endregion
		#region FormulaInvariant
		public string FormulaInvariant {
			get {
				return ReadOnlyModelCell.FormulaBodyInvariant;
			}
			set {
				Model.WorkbookDataContext context = ReadOnlyModelCell.Sheet.Workbook.DataContext;
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
				NativeRangeArrayFormulaAccessor accessor = new NativeRangeArrayFormulaAccessor(ReadOnlyModelCell.GetRange());
				return accessor.GetValue();
			}
			set {
				NativeRangeArrayFormulaAccessor accessor = new NativeRangeArrayFormulaAccessor(ReadWriteModelCell.GetRange());
				accessor.SetValue(value);
			}
		}
		#endregion
		#region ArrayFormulaInvariant
		public string ArrayFormulaInvariant {
			get {
				Model.WorkbookDataContext context = ReadOnlyModelCell.Sheet.Workbook.DataContext;
				context.PushCulture(CultureInfo.InvariantCulture);
				try {
					return ArrayFormula;
				}
				finally {
					context.PopCulture();
				}
			}
			set {
				Model.WorkbookDataContext context = ReadOnlyModelCell.Sheet.Workbook.DataContext;
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
		#region Invalidate
		public void Invalidate() {
			modelCell = null;
		}
		#endregion
		void RaiseCellValueChanged(Model.CellContentSnapshot snapshot) {
			((Model.DocumentModel)worksheet.DocumentModel).RaiseCellValueChangedProgrammatically(snapshot);
		}
		public void SubstituteModelCell(Model.ICell newCell) {
			Guard.ReferenceEquals(this.worksheet.ModelWorksheet, newCell.Sheet);
			this.key = newCell.Key;
			this.modelCell = newCell;
		}
		public void SubstituteModelCell(Model.CellKey key) {
			Guard.Equals(key.SheetId, this.worksheet.ModelWorksheet.SheetId);
			this.key = key;
			this.modelCell = null;
		}
	}
	#endregion
	#region NativeCell
	partial class NativeCell : Cell {
		readonly NativeWorksheet nativeWorksheet;
		ModelCellKey key;
		NativeCellContent cellContent;
		public NativeCell(NativeWorksheet worksheet, ModelCellKey key)
			: base() {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.nativeWorksheet = worksheet;
			this.key = key;
			this.cellContent = new NativeCellContent(key, worksheet);
		}
		#region Properties
		public Worksheet Worksheet { get { return nativeWorksheet; } }
		public Model.ICell ReadOnlyModelCell { get { return cellContent.ReadOnlyModelCell; } }
		public Model.ICell ReadWriteModelCell { get { return cellContent.ReadWriteModelCell; } }
		public string DisplayText { get { return ReadOnlyModelCell.Text; } }
		public bool IsDisplayedAsDateTime {
			get {
				Model.ICell cell = ReadOnlyModelCell;
				ModelVariantValue value = cell.Value;
				if (!value.IsNumeric)
					return false;
				if (Model.WorkbookDataContext.IsErrorDateTimeSerial(value.NumericValue,
					nativeWorksheet.ModelWorkbook.DataContext.DateSystem))
					return false;
				Model.NumberFormat numberFormat = cell.ActualFormat;
				return numberFormat.IsDateTime && !numberFormat.IsText;
			}
		}
		public Color FillColor { get { return this.Format.Fill.BackgroundColor; } set { this.Format.Fill.BackgroundColor = value; } }
		public string FontName { get { return this.Format.Font.Name; } set { this.Format.Font.Name = value; } }
		public double FontSize { get { return this.Format.Font.Size; } set { this.Format.Font.Size = value; } }
		public Color FontColor { get { return this.Format.Font.Color; } set { this.Format.Font.Color = value; } }
		#region FontBold
		public bool FontBold {
			get {
				return FontStyleConverter.IsBold(this.Format.Font.FontStyle);
			}
			set {
				Formatting formatting = this.Format;
				formatting.Font.FontStyle = FontStyleConverter.SetBold(formatting.Font.FontStyle, value);
			}
		}
		#endregion
		#region FontItalic
		public bool FontItalic {
			get {
				return FontStyleConverter.IsItalic(this.Format.Font.FontStyle);
			}
			set {
				Formatting formatting = this.Format;
				formatting.Font.FontStyle = FontStyleConverter.SetItalic(formatting.Font.FontStyle, value);
			}
		}
		#endregion
		public SpreadsheetVerticalAlignment VerticalAlignment { get { return Format.Alignment.Vertical; } set { Format.Alignment.Vertical = value; } }
		public SpreadsheetHorizontalAlignment HorizontalAlignment { get { return Format.Alignment.Horizontal; } set { Format.Alignment.Horizontal = value; } }
		Column Column { get { return nativeWorksheet.Columns[ColumnIndex]; } }
		public int ColumnIndex { [DebuggerStepThrough] get { return key.ColumnIndex; } }
		public int RowIndex { [DebuggerStepThrough] get { return key.RowIndex; } }
		public CellValue Value {
			get {
				CellValue result = cellContent.Value;
				result.IsDateTime = IsDisplayedAsDateTime;
				return result;
			}
			set {
				nativeWorksheet.DocumentModel.BeginUpdate();
				try {
					cellContent.Value = value;
					if (value != null && value.IsDateTime) {
						Formatting format = this.Format;
						if (string.IsNullOrEmpty(format.NumberFormat))
							format.NumberFormat = DevExpress.XtraSpreadsheet.Model.ShortDateNumberFormat.Instance.GetActualFormat(CultureInfo.InvariantCulture).FormatCode;
					}
				}
				finally {
					nativeWorksheet.DocumentModel.EndUpdate();
				}
			}
		}
		public string Formula { get { return cellContent.Formula; } set { cellContent.Formula = value; } }
		public string FormulaInvariant { get { return cellContent.FormulaInvariant; } set { cellContent.FormulaInvariant = value; } }
		public string ArrayFormula { get { return cellContent.ArrayFormula; } set { cellContent.ArrayFormula = value; } }
		public string ArrayFormulaInvariant { get { return cellContent.ArrayFormulaInvariant; } set { cellContent.ArrayFormulaInvariant = value; } }
		public Spreadsheet.Formulas.ParsedExpression ParsedExpression {
			get {
				if (!ReadOnlyModelCell.HasFormula)
					return null;
				NativeWorkbook nativeWorkbook = nativeWorksheet.NativeWorkbook;
				Model.WorkbookDataContext dataContext = nativeWorkbook.DocumentModel.DataContext;
				dataContext.PushCurrentCell(ReadOnlyModelCell);
				try {
					return Spreadsheet.Formulas.ParsedExpression.FromModelExporession(ReadOnlyModelCell.Formula.Expression, nativeWorkbook);
				}
				finally {
					dataContext.PopCurrentCell();
				}
			}
			set {
				string formula = string.Empty;
				if (value != null) {
					DevExpress.Spreadsheet.Formulas.ExpressionContext context = new Spreadsheet.Formulas.ExpressionContext(ColumnIndex, RowIndex, Worksheet);
					formula = value.ToString(context);
				}
				cellContent.Formula = formula;
			}
		}
		public double ColumnWidthInCharacters { get { return Column.WidthInCharacters; } set { Column.WidthInCharacters = (float)value; } }
		public double ColumnWidth { get { return Column.Width; } set { Column.Width = (float)value; } }
		public int ColumnWidthInPixels { get { return Column.WidthInPixels; } set { Column.WidthInPixels = value; } }
		public double RowHeight { get { return Worksheet.Rows[RowIndex].Height; } set { Worksheet.Rows[RowIndex].Height = (float)value; } }
		#region IsTopLeftCellInArrayFormulaRange
		public bool IsTopLeftCellInArrayFormulaRange {
			get {
				Range arrayFormulaRange = this.GetArrayFormulaRange();
				if (arrayFormulaRange != null)
					return Object.ReferenceEquals(arrayFormulaRange[0], this);
				return false;
			}
		}
		#endregion
		public string Name {
			get {
				return nativeWorksheet.GetDefinedNameFromRange(this);
			}
			set {
				string reference = String.Concat('=', GetThisAsNativeRange().GetReferenceA1(ReferenceElement.IncludeSheetName | ReferenceElement.ColumnAbsolute | ReferenceElement.RowAbsolute));
				nativeWorksheet.DefinedNames.Add(value, reference);
			}
		}
		#region Dependents/Precedents
		public IList<Range> Precedents {
			get {
				Model.ICell cell = ReadOnlyModelCell;
				Model.CellRangeBase precedentsRange = cell.Worksheet.Workbook.CalculationChain.GetPrecedents(cell, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(this.nativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> DirectPrecedents {
			get {
				Model.ICell cell = ReadOnlyModelCell;
				Model.CellRangeBase precedentsRange = cell.Worksheet.Workbook.CalculationChain.GetDirectPrecedents(cell, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(this.nativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> Dependents {
			get {
				Model.CellRangeBase precedentsRange = nativeWorksheet.ModelWorkbook.CalculationChain.GetDependents(key, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(this.nativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> DirectDependents {
			get {
				Model.CellRangeBase precedentsRange = nativeWorksheet.ModelWorkbook.CalculationChain.GetDirectDependents(key, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(this.nativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		#endregion
		#region Calculate
		public void Calculate() {
			nativeWorksheet.ModelWorkbook.CalculationChain.CalculateRange(GetThisAsModelCellRange());
		}
		#endregion
		public bool IsMerged {
			get {
				return nativeWorksheet.ModelWorksheet.MergedCells.GetMergedCellRange(ColumnIndex, RowIndex) != null;
			}
		}
		public bool HasFormula { get { return ReadOnlyModelCell.HasFormula; } }
		public bool HasArrayFormula {
			get {
				NativeRange nativeRange = GetThisAsNativeRange();
				return nativeRange.HasArrayFormula;
			}
		}
		public Range CurrentRegion {
			get {
				ModelCellRange region = nativeWorksheet.ModelWorksheet.GetCurrentRegion(GetThisAsModelCellRange());
				return new NativeRange(region, nativeWorksheet);
			}
		}
		public bool HasQuotePrefix { get { return cellContent.HasQuotePrefix; } }
		public object Tag {
			get {
				return nativeWorksheet.ModelWorksheet.CellTags.GetValue(this.ColumnIndex, this.RowIndex);
			}
			set {
				nativeWorksheet.ModelWorksheet.CellTags.SetValue(this.ColumnIndex, this.RowIndex, value);
			}
		}
		public AreasCollection Areas { get { return new NativeAreasCollection(GetThisAsNativeRange()); } }
		#endregion
		NativeRange GetThisAsNativeRange() {
			ModelCellRange range = GetThisAsModelCellRange();
			return nativeWorksheet.CreateRange(range);
		}
		#region Intersect
		public Range Intersect(Range other) {
			if (!IsIntersecting(other))
				return null;
			return new NativeRange(GetThisAsModelCellRange(), nativeWorksheet);
		}
		#endregion
		#region IsIntersect
		public bool IsIntersecting(Range other) {
			Model.CellRangeBase modelRange = this.nativeWorksheet.GetModelRange(other);
			return modelRange.ContainsCell(key);
		}
		#endregion
		#region Union
		public Range Union(Range other) {
			if (other == null)
				return GetThisAsNativeRange();
			return NativeRangeReferenceParseHelper.CreateUnion(GetThisAsNativeRange(), other);
		}
		#endregion
		#region Offset
		public Range Offset(int rowCount, int columnCount) {
			Model.CellPositionOffset offset = new Model.CellPositionOffset(columnCount, rowCount);
			ModelCellRange range = GetThisAsModelCellRange().GetShifted(offset);
			return new NativeRange(range, nativeWorksheet);
		}
		#endregion
		int Range.TopRowIndex { get { return RowIndex; } }
		int Range.BottomRowIndex { get { return RowIndex; } }
		int Range.LeftColumnIndex { get { return ColumnIndex; } }
		int Range.RightColumnIndex { get { return ColumnIndex; } }
		int Range.RowCount { get { return 1; } }
		int Range.ColumnCount { get { return 1; } }
		Range Range.GetRangeWithAbsoluteReference() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetRangeWithAbsoluteReference();
		}
		Range Range.GetRangeWithRelativeReference() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetRangeWithRelativeReference();
		}
		string Range.GetReferenceA1() {
			return GetThisAsModelCellRange().ToString();
		}
		string Range.GetReferenceA1(ReferenceElement flags) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceA1(flags);
		}
		string Range.GetReferenceR1C1(ReferenceElement flags, Cell baseCell) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceR1C1(flags, baseCell);
		}
		string Range.GetReferenceR1C1(Cell baseCell) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceR1C1(baseCell);
		}
		public void SetValue(object value) {
			this.Value = CellValue.FromObject(value);
		}
		#region CreateArrayFormula
		public void CreateArrayFormula(string formula, int rowCount, int columnCount) {
			Range range = nativeWorksheet.CreateRange(key.RowIndex,
							  key.ColumnIndex,
							  key.RowIndex + rowCount - 1,
							  key.ColumnIndex + columnCount - 1);
			nativeWorksheet.ArrayFormulas.Add(range, formula);
		}
		#endregion
		#region GetArrayFormulaRange
		public Range GetArrayFormulaRange() {
			Model.Worksheet modelWorksheet = nativeWorksheet.ModelWorksheet;
			int index = modelWorksheet.ArrayFormulaRanges.FindArrayFormulaIndex(ReadOnlyModelCell);
			if (index >= 0)
				return Worksheet.ArrayFormulas[index].Range;
			return null;
		}
		#endregion
		#region GetThisAsModelCellRange
		ModelCellRange GetThisAsModelCellRange() {
			ModelCellPosition topLeft = new ModelCellPosition(ColumnIndex, RowIndex);
			ModelCellPosition bottomRight = topLeft;
			ModelCellRange range = new ModelCellRange(nativeWorksheet.ModelWorksheet, topLeft, bottomRight);
			return range;
		}
		#endregion
		#region GetHashCode
		public override int GetHashCode() {
			return ReadOnlyModelCell.GetHashCode();
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			NativeCell nativeCell = obj as NativeCell;
			if (nativeCell == null)
				return false;
			return key.Equals(nativeCell.key) && Object.ReferenceEquals(this.nativeWorksheet, nativeCell.nativeWorksheet);
		}
		#endregion
		#region Style
		public Style Style {
			get {
				return new NativeCellStyle(ReadOnlyModelCell.Style);
			}
			set {
				Model.CellStyleBase modelStyle = ((NativeCellStyle)value).ModelCellStyle;
				ReadWriteModelCell.Style = modelStyle;
			}
		}
		#endregion
		#region Formatting
		Formatting Format {
			get {
				return new NativeActualCellFormat(cellContent);
			}
		}
		string Formatting.NumberFormat { get { return Format.NumberFormat; } set { Format.NumberFormat = value; } }
		Alignment Formatting.Alignment { get { return Format.Alignment; } }
		SpreadsheetFont Formatting.Font { get { return Format.Font; } }
		Borders Formatting.Borders { get { return Format.Borders; } }
		Fill Formatting.Fill { get { return Format.Fill; } }
		Protection Formatting.Protection { get { return Format.Protection; } }
		StyleFlags Formatting.Flags { get { return Format.Flags; } }
		bool Formatting.Equals(object other) {
			NativeCell otherCell = other as NativeCell;
			if (otherCell != null)
				return key.Equals(otherCell.key) && Object.ReferenceEquals(this.nativeWorksheet, otherCell.nativeWorksheet);
			return Format.Equals(other);
		}
		void Formatting.BeginUpdate() {
			Format.BeginUpdate();
		}
		void Formatting.EndUpdate() {
			Format.EndUpdate();
		}
		#endregion
		void Range.SetInsideBorders(Color color, BorderLineStyle style) {
		}
		#region Invalidate
		public void Invalidate() {
			cellContent.Invalidate();
		}
		#endregion
		#region GetMergedRanges
		public IList<Range> GetMergedRanges() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetMergedRanges();
		}
		#endregion
		Formatting Range.BeginUpdateFormatting() {
			ReadWriteModelCell.BeginUpdate();
			return GetThisAsNativeRange().BeginUpdateFormatting();
		}
		void Range.EndUpdateFormatting(Formatting formatting) {
			GetThisAsNativeRange().EndUpdateFormatting(formatting);
			ReadWriteModelCell.EndUpdate();
		}
		Cell Range.this[int rowOffset, int columnOffset] { get { return GetThisAsNativeRange()[rowOffset, columnOffset]; } }
		Cell Range.this[int position] { get { return GetThisAsNativeRange()[position]; } }
		void Range.CopyFrom(Range source) { 
			GetThisAsNativeRange().CopyFrom(source);
		}
		void Range.MoveTo(Range target) {
			GetThisAsNativeRange().MoveTo(target);
		}
		void Range.CopyFrom(Range source, PasteSpecial pasteOptions) { 
			GetThisAsNativeRange().CopyFrom(source, pasteOptions);
		}
		IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() {
			yield return this;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return (this as IEnumerable<Cell>).GetEnumerator();
		}
		public override string ToString() {
			ModelCellPosition pos = new ModelCellPosition(key.ColumnIndex, key.RowIndex);
			string value = this.Value.ToString();
			if (value.Length > 20)
				value = value.Substring(0, 20) + "...";
			return String.Format("Cell:{0} formula:\"{1}\" value:\"{2}\" type:{3}", pos.ToString(), this.Formula, value, this.Value.Type.ToString());
		}
		public void SubstituteModelCell(Model.ICell newCell) {
			key = newCell.Key;
			this.cellContent.SubstituteModelCell(newCell);
		}
		public void SubstituteModelCell(Model.CellKey key, NativeCellContent cellContent) {
			this.key = key;
			this.cellContent = cellContent;
		}
		public IEnumerable<Cell> Search(string text) {
			ModelSearchOptions modelOptions = nativeWorksheet.CreateDefaultSearchOptions();
			modelOptions.Range = GetThisAsModelCellRange();
			return nativeWorksheet.SearchCore(text, modelOptions);
		}
		public IEnumerable<Cell> Search(string text, SearchOptions options) {
			ModelSearchOptions modelOptions = nativeWorksheet.ConvertSearchOptions(options);
			modelOptions.Range = GetThisAsModelCellRange();
			return nativeWorksheet.SearchCore(text, modelOptions);
		}
		#region ExistingCells
		public IEnumerable<Cell> ExistingCells {
			get {
				ModelCellRange modelRange = GetThisAsModelCellRange();
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
	#region NativeCellCollection
	partial class NativeCellCollection : NativeRange, CellCollection {
		public NativeCellCollection(ModelCellRange range, NativeWorksheet nativeWorksheet)
			: base(range, nativeWorksheet) {
		}
		#region Item
		public Cell this[string position] {
			get {
				NativeCell result = (this.Worksheet as NativeWorksheet).GetCell(position);
				return result;
			}
		}
		#endregion
	}
	#endregion
	#region NativeCellCollectionWithCache
	partial class NativeCellCollectionWithCache : NativeRange, CellCollection {
		const int maxCacheSize = 16;
		const int cacheDelta = maxCacheSize / 4;
		Dictionary<Model.CellKey, NativeCell> cellsCache;
		List<Model.CellKey> mruKeys;
		readonly object syncRoot = new object();
		public NativeCellCollectionWithCache(ModelCellRange range, NativeWorksheet nativeWorksheet)
			: base(range, nativeWorksheet) {
			cellsCache = new Dictionary<Model.CellKey, NativeCell>();
			mruKeys = new List<ModelCellKey>();
		}
		#region Properties
		public int CellsCacheCount { get { return this.cellsCache.Count; } }
		#region Item
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		public new Cell this[int row, int column] {
			get {
				Model.CellKey key = new ModelCellKey(ModelWorksheet.SheetId, column, row);
				if (!IndicesChecker.CheckIsColumnIndexValid(column))
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex, "column");
				if (!IndicesChecker.CheckIsRowIndexValid(row))
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex, "row");
				return this[key];
			}
		}
		protected internal Cell this[ModelCellKey key] {
			get {
				lock (syncRoot) {
					if (cellsCache.ContainsKey(key)) {
						UpdateMostRecentlyUsed(key);
						return cellsCache[key];
					}
					RemoveLeastRecentlyUsed();
					NativeCell result = new NativeCell(NativeWorksheet, key);
					cellsCache.Add(key, result);
					mruKeys.Insert(0, key);
					return result;
				}
			}
		}
		void UpdateMostRecentlyUsed(Model.CellKey key) {
			int index = mruKeys.IndexOf(key);
			if (index < 0)
				return;
			mruKeys.RemoveAt(index);
			mruKeys.Insert(0, key);
		}
		void RemoveLeastRecentlyUsed() {
			int mruKeysCount = mruKeys.Count;
			if (mruKeysCount < maxCacheSize)
				return;
			int startIndex = mruKeysCount - cacheDelta;
			for (int i = 0; i < cacheDelta; i++) {
				ModelCellKey key = mruKeys[i + startIndex];
				cellsCache[key].Invalidate();
				cellsCache.Remove(key);
			}
			mruKeys.RemoveRange(startIndex, cacheDelta);
		}
		#endregion
		#region Item
		public Cell this[string position] {
			get {
				Model.CellPosition cellPosition = Model.CellReferenceParser.Parse(position);
				return this[cellPosition.Row, cellPosition.Column];
			}
		}
		#endregion
		#endregion
		protected internal NativeCell TryGetCell(ModelCellKey key) {
			lock (syncRoot) {
				NativeCell result;
				cellsCache.TryGetValue(key, out result);
				return result;
			}
		}
		#region Clear
		public void ClearCachedItems() {
			lock (syncRoot) {
				cellsCache.Clear();
				mruKeys.Clear();
			}
		}
		#endregion
		#region Invalidate
		public void Invalidate() {
			lock (syncRoot) {
				foreach (NativeCell cell in cellsCache.Values)
					cell.Invalidate();
			}
		}
		#endregion
		#region Invalidate
		public void Invalidate(int startIndex) {
			lock (syncRoot) {
				foreach (Model.CellKey key in cellsCache.Keys)
					if (key.ColumnIndex >= startIndex)
						cellsCache[key].Invalidate();
			}
		}
		#endregion
		#region Invalidate
		public void Invalidate(int startIndex, int endIndex) {
			lock (syncRoot) {
				foreach (Model.CellKey key in cellsCache.Keys)
					if (key.ColumnIndex >= startIndex && key.ColumnIndex <= endIndex)
						cellsCache[key].Invalidate();
			}
		}
		#endregion
	}
	#endregion
}
#endregion
