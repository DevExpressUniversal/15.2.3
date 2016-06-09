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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xlsx;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraExport.Csv;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Imaging;
#if !SL
using System.Drawing;
using System.Drawing.Imaging;
#endif
namespace DevExpress.Export.Xl {
	#region IXlExport
	public interface IXlExport {
		int CurrentRowIndex { get; }
		int CurrentColumnIndex { get; }
		int CurrentOutlineLevel { get; }
		IXlDocument BeginExport(Stream stream);
		void EndExport();
		IXlSheet BeginSheet();
		void EndSheet();
		IXlGroup BeginGroup();
		void EndGroup();
		IXlColumn BeginColumn();
		void EndColumn();
		void SkipColumns(int count);
		IXlRow BeginRow();
		void EndRow();
		void SkipRows(int count);
		IXlCell BeginCell();
		void EndCell();
		void SkipCells(int count);
		IXlFormulaEngine FormulaEngine { get; }
		IXlDocumentOptions DocumentOptions { get; }
		XlDocumentProperties DocumentProperties { get; }
		IXlPicture BeginPicture();
		void EndPicture();
	}
	#endregion
	#region XlSummary
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Spelling", "DF1000")]
	public enum XlSummary {
		Average = 1, 
		Count = 2, 
		CountA = 3, 
		Max = 4, 
		Min = 5, 
		Product = 6, 
		StdevS = 7, 
		StdevP = 8, 
		Sum = 9, 
		VarS = 10, 
		VarP = 11, 
	}
	#endregion
	#region IXlFormulaParameter
	public interface IXlFormulaParameter {
		string ToString(CultureInfo culture);
	}
	#endregion
	#region IXlFormulaEngine
	public interface IXlFormulaEngine {
		IXlFormulaParameter Param(XlVariantValue value);
		IXlFormulaParameter Subtotal(XlCellRange range, XlSummary summary, bool ignoreHidden);
		IXlFormulaParameter Subtotal(IList<XlCellRange> ranges, XlSummary summary, bool ignoreHidden);
		IXlFormulaParameter VLookup(XlVariantValue lookupValue, XlCellRange table, int columnIndex, bool rangeLookup);
		IXlFormulaParameter VLookup(IXlFormulaParameter lookupValue, XlCellRange table, int columnIndex, bool rangeLookup);
		IXlFormulaParameter Text(XlVariantValue value, string netFormatString, bool isDateTimeFormatString);
		IXlFormulaParameter Text(IXlFormulaParameter value, string netFormatString, bool isDateTimeFormatString);
		IXlFormulaParameter Concatenate(params IXlFormulaParameter[] parameters);
	}
	#endregion
	#region IXlFormulaParser
	public interface IXlFormulaParser {
		XlExpression Parse(string formula, IXlExpressionContext context);
	}
	#endregion
	#region XlDocumentFormat
	public enum XlDocumentFormat {
		Xlsx,
		Xls,
		Csv
	}
	#endregion
	#region IXlDocumentOptions
	public interface IXlDocumentOptions {
		XlDocumentFormat DocumentFormat { get; }
		CultureInfo Culture { get; set; }
		bool SupportsFormulas { get; }
		bool SupportsDocumentParts { get; }
		bool SupportsOutlineGrouping { get; }
		int MaxColumnCount { get; }
		int MaxRowCount { get; }
	}
	#endregion
	#region XlDocumentTheme
	public enum XlDocumentTheme {
		None,
		Office2010,
		Office2013
	}
	#endregion
	#region IXlExporter
	public interface IXlExporter {
		IXlDocument CreateDocument(Stream stream);
	}
	#endregion
	#region IXlDocument
	public interface IXlDocument : IDisposable {
		IXlDocumentOptions Options { get; }
		XlDocumentProperties Properties { get; }
		XlDocumentTheme Theme { get; set; }
		IXlSheet CreateSheet();
	}
	#endregion
	#region IXlSheet
	public interface IXlSheet : IDisposable {
		string Name { get; set; }
		IXlMergedCells MergedCells { get; }
		XlCellPosition SplitPosition { get; set; }
		XlCellRange AutoFilterRange { get; set; }
		IList<XlConditionalFormatting> ConditionalFormattings { get; }
		IList<XlDataValidation> DataValidations { get; }
		XlSheetVisibleState VisibleState { get; set; }
		XlPageMargins PageMargins { get; set; }
		XlPageSetup PageSetup { get; set; }
		XlHeaderFooter HeaderFooter { get; }
		XlPrintTitles PrintTitles { get; }
		XlCellRange PrintArea { get; set; }
		XlPrintOptions PrintOptions { get; set; }
		IXlPageBreaks ColumnPageBreaks { get; }
		IXlPageBreaks RowPageBreaks { get; }
		IList<XlHyperlink> Hyperlinks { get; }
		XlCellRange DataRange { get; }
		XlCellRange ColumnRange { get; }
		XlIgnoreErrors IgnoreErrors { get; set; }
		IXlOutlineProperties OutlineProperties { get; }
		IXlSheetViewOptions ViewOptions { get; }
		IList<XlSparklineGroup> SparklineGroups { get; }
		int CurrentRowIndex { get; }
		int CurrentColumnIndex { get; }
		int CurrentOutlineLevel { get; }
		int BeginGroup(bool collapsed);
		int BeginGroup(int outlineLevel, bool collapsed);
		void EndGroup();
		IXlColumn CreateColumn();
		IXlColumn CreateColumn(int columnIndex);
		void SkipColumns(int count);
		IXlRow CreateRow();
		IXlRow CreateRow(int rowIndex);
		void SkipRows(int count);
		IXlPicture CreatePicture();
	}
	#endregion
	#region IXlGroup
	public interface IXlGroup {
		int OutlineLevel { get; set; }
		bool IsCollapsed { get; set; }
	}
	#endregion
	#region IXlRow
	public interface IXlRow : IDisposable {
		int RowIndex { get; }
		XlCellFormatting Formatting { get; set; }
		bool IsHidden { get; set; }
		bool IsCollapsed { get; set; }
		int HeightInPixels { get; set; }
		IXlCell CreateCell();
		IXlCell CreateCell(int columnIndex);
		void SkipCells(int count);
		void BlankCells(int count, XlCellFormatting formatting);
		void BulkCells(IEnumerable values, XlCellFormatting formatting);
		void ApplyFormatting(XlCellFormatting formatting);
	}
	#endregion
	#region IXlCell
	public interface IXlCell : IDisposable {
		XlVariantValue Value { get; set; }
		int RowIndex { get; }
		int ColumnIndex { get; }
		XlCellPosition Position { get; }
		XlCellFormatting Formatting { get; set; }
		void ApplyFormatting(XlCellFormatting formatting);
		void SetFormula(IXlFormulaParameter formula);
		void SetFormula(XlExpression formula);
		void SetFormula(string formula);
		void SetSharedFormula(XlExpression formula, XlCellRange range);
		void SetSharedFormula(string formula, XlCellRange range);
		void SetSharedFormula(XlCellPosition hostCell);
		void SetRichText(XlRichTextString value);
	}
	#endregion
	#region IXlColumn
	public interface IXlColumn : IDisposable {
		int ColumnIndex { get; }
		bool IsHidden { get; set; }
		bool IsCollapsed { get; set; }
		int WidthInPixels { get; set; }
		float WidthInCharacters { get; set; }
		XlCellFormatting Formatting { get; set; }
		void ApplyFormatting(XlCellFormatting formatting);
	}
	#endregion
	#region XlAnchorType
	public enum XlAnchorType {
		TwoCell,
		OneCell,
		Absolute
	}
	#endregion
	#region XlAnchorPoint
	public struct XlAnchorPoint {
		readonly int column;
		readonly int row;
		readonly int columnOffset;
		readonly int rowOffset;
		readonly int cellWidth;
		readonly int cellHeight;
		public XlAnchorPoint(int column, int row)
			: this(column, row, 0, 0, 0, 0) {
		}
		public XlAnchorPoint(int column, int row, int columnOffsetInPixels, int rowOffsetInPixels)
			: this(column, row, columnOffsetInPixels, rowOffsetInPixels, 0, 0) {
		}
		internal XlAnchorPoint(int column, int row, int columnOffsetInPixels, int rowOffsetInPixels, int cellWidthInPixels, int cellHeightInPixels) {
			Guard.ArgumentNonNegative(cellWidthInPixels, "cellWidthInPixels");
			Guard.ArgumentNonNegative(cellHeightInPixels, "cellHeightInPixels");
			this.column = column;
			this.row = row;
			this.columnOffset = columnOffsetInPixels;
			this.rowOffset = rowOffsetInPixels;
			this.cellWidth = cellWidthInPixels;
			this.cellHeight = cellHeightInPixels;
		}
		public int Column { get { return column; } }
		public int Row { get { return row; } }
		public int ColumnOffsetInPixels { get { return columnOffset; } }
		public int RowOffsetInPixels { get { return rowOffset; } }
		public float RelativeColumnOffset {
			get {
				if (this.cellWidth == 0)
					return 0f;
				return (float)(columnOffset * 1.0 / cellWidth);
			}
		}
		public float RelativeRowOffset {
			get {
				if (this.cellHeight == 0)
					return 0f;
				return (float)(rowOffset * 1.0 / cellHeight);
			}
		}
		public override bool Equals(object obj) {
			if (obj is XlAnchorPoint) {
				XlAnchorPoint other = (XlAnchorPoint)obj;
				return column == other.column && row == other.row &&
					columnOffset == other.columnOffset && rowOffset == other.rowOffset &&
					cellWidth == other.cellWidth && cellHeight == other.cellHeight;
			}
			return false;
		}
		public override int GetHashCode() {
			return column ^ row ^ columnOffset ^ rowOffset ^ cellWidth ^ cellHeight;
		}
	}
	#endregion
	#region IXlPicture
	public interface IXlPicture : IDisposable {
		string Name { get; set; }
		Image Image { get; set; }
		ImageFormat Format { get; set; }
		XlAnchorType AnchorType { get; set; }
		XlAnchorType AnchorBehavior { get; set; }
		XlAnchorPoint TopLeft { get; set; }
		XlAnchorPoint BottomRight { get; set; }
		XlPictureHyperlink HyperlinkClick { get; }
		void SetAbsoluteAnchor(int x, int y, int width, int height);
		void SetOneCellAnchor(XlAnchorPoint topLeft, int width, int height);
		void SetTwoCellAnchor(XlAnchorPoint topLeft, XlAnchorPoint bottomRight, XlAnchorType behavior);
		void FitToCell(XlCellPosition position, int cellWidth, int cellHeight, bool center);
		void StretchToCell(XlCellPosition position);
	}
	#endregion
	#region IXlOutlineProperties
	public interface IXlOutlineProperties {
		bool SummaryBelow { get; set; }
		bool SummaryRight { get; set; }
	}
	#endregion
	#region XlDocumentSecurity
	[Flags]
	public enum XlDocumentSecurity {
		None = 0x0000,
		ReadonlyRecommended = 0x0002,
		ReadonlyEnforced = 0x0004,
		Locked = 0x0008
	}
	#endregion
	#region XlCustomPropertyValue
	public struct XlCustomPropertyValue {
		public static readonly XlCustomPropertyValue Empty = new XlCustomPropertyValue();
		XlVariantValueType type;
		double numericValue;
		DateTime dateTimeValue;
		string textValue;
		public XlVariantValueType Type { get { return type; } }
		public double NumericValue {
			get { return numericValue; }
			private set {
				this.numericValue = value;
				this.textValue = null;
				this.type = XlVariantValueType.Numeric;
			}
		}
		public DateTime DateTimeValue {
			get { return dateTimeValue; }
			private set {
				this.dateTimeValue = value;
				this.numericValue = 0;
				this.textValue = null;
				this.type = XlVariantValueType.DateTime;
			}
		}
		public bool BooleanValue {
			get { return this.numericValue != 0; }
			private set {
				this.numericValue = value ? 1 : 0;
				this.textValue = null;
				this.type = XlVariantValueType.Boolean;
			}
		}
		public string TextValue {
			get { return textValue; }
			private set {
				this.textValue = value;
				this.numericValue = 0;
				this.type = XlVariantValueType.Text;
			}
		}
		public static implicit operator XlCustomPropertyValue(double value) {
			XlCustomPropertyValue result = new XlCustomPropertyValue();
			result.NumericValue = value;
			return result;
		}
		public static implicit operator XlCustomPropertyValue(DateTime value) {
			XlCustomPropertyValue result = new XlCustomPropertyValue();
			result.DateTimeValue = value;
			return result;
		}
		public static implicit operator XlCustomPropertyValue(char value) {
			XlCustomPropertyValue result = new XlCustomPropertyValue();
			result.TextValue = char.ToString(value);
			return result;
		}
		public static implicit operator XlCustomPropertyValue(string value) {
			if (value == null)
				return XlCustomPropertyValue.Empty;
			XlCustomPropertyValue result = new XlCustomPropertyValue();
			result.TextValue = value;
			return result;
		}
		public static implicit operator XlCustomPropertyValue(bool value) {
			XlCustomPropertyValue result = new XlCustomPropertyValue();
			result.BooleanValue = value;
			return result;
		}
	}
	#endregion
	#region XlDocumentCustomProperties
	public class XlDocumentCustomProperties {
		readonly Dictionary<string, XlCustomPropertyValue> items = new Dictionary<string, XlCustomPropertyValue>();
		#region Properties
		public int Count { get { return items.Count; } }
		public XlCustomPropertyValue this[string name] {
			get {
				XlCustomPropertyValue result;
				if (items.TryGetValue(name, out result))
					return result;
				return XlCustomPropertyValue.Empty;
			}
			set {
				if (value.Type == XlVariantValueType.None)
					items.Remove(name);
				else
					items[name] = value;
			}
		}
		public IEnumerable<string> Names { get { return items.Keys; } }
		#endregion
		public void Clear() {
			items.Clear();
		}
	}
	#endregion
	#region XlDocumentProperties
	public class XlDocumentProperties {
		string version;
		readonly XlDocumentCustomProperties custom = new XlDocumentCustomProperties();
		public string Application { get; set; }
		public string Manager { get; set; }
		public string Company { get; set; }
		public string Version {
			get { return version; }
			set {
				if (!string.IsNullOrEmpty(value) && !IsValidVersionString(value))
					throw new ArgumentException("Version must be of the form XX.YYYY, where X and Y represent numerical values.");
				version = value;
			}
		}
		public XlDocumentSecurity Security { get; set; }
		public string Title { get; set; }
		public string Subject { get; set; }
		public string Author { get; set; }
		public string Keywords { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public DateTime Created { get; set; }
		public XlDocumentCustomProperties Custom { get { return custom; } }
		bool IsValidVersionString(string value) {
			if (value.Length != 7)
				return false;
			for (int i = 0; i < 7; i++) {
				if (i == 2 && value[i] != '.')
					return false;
				if (i != 2 && !char.IsDigit(value[i]))
					return false;
			}
			return true;
		}
	}
	#endregion
	#region XlExport (exporter factory)
	public static class XlExport {
		public static IXlExporter CreateExporter(XlDocumentFormat documentFormat) {
			return CreateExporter(documentFormat, null);
		}
		public static IXlExporter CreateExporter(XlDocumentFormat documentFormat, IXlFormulaParser formulaParser) {
			if (documentFormat == XlDocumentFormat.Xlsx)
				return new XlsxDataAwareExporter(formulaParser);
			if (documentFormat == XlDocumentFormat.Xls)
				return new XlsDataAwareExporter(formulaParser);
			return new CsvDataAwareExporter();
		}
	}
	#endregion
}
namespace DevExpress.XtraExport.Implementation {
	using DevExpress.Utils;
	#region XlDocument
	public class XlDocument : IXlDocument {
		IXlExport exporter;
		public XlDocument(IXlExport exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
			Theme = XlDocumentTheme.Office2013;
		}
		#region IXlDocument Members
		public IXlDocumentOptions Options {
			get { return exporter.DocumentOptions; }
		}
		public XlDocumentProperties Properties {
			get { return exporter.DocumentProperties; }
		}
		public XlDocumentTheme Theme { get; set; }
		public IXlSheet CreateSheet() {
			return new XlSheetProxy(exporter, exporter.BeginSheet());
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing && exporter != null) {
				exporter.EndExport();
				exporter = null;
			}
		}
		#endregion
	}
	#endregion
	#region XlSheetProxy
	class XlSheetProxy : IXlSheet, IXlShapeContainer {
		IXlExport exporter;
		readonly IXlSheet subject;
		public XlSheetProxy(IXlExport exporter, IXlSheet subject) {
			this.exporter = exporter;
			this.subject = subject;
		}
		#region IXlSheet Members
		public string Name {
			get { return subject.Name; }
			set { subject.Name = value; }
		}
		public IXlMergedCells MergedCells {
			get { return subject.MergedCells; }
		}
		public XlCellPosition SplitPosition {
			get { return subject.SplitPosition; }
			set { subject.SplitPosition = value; }
		}
		public XlCellRange AutoFilterRange {
			get { return subject.AutoFilterRange; }
			set { subject.AutoFilterRange = value; }
		}
		public IList<XlConditionalFormatting> ConditionalFormattings {
			get { return subject.ConditionalFormattings; }
		}
		public IList<XlDataValidation> DataValidations {
			get { return subject.DataValidations; }
		}
		public XlSheetVisibleState VisibleState {
			get { return subject.VisibleState; }
			set { subject.VisibleState = value; }
		}
		public XlPageMargins PageMargins {
			get { return subject.PageMargins; }
			set { subject.PageMargins = value; }
		}
		public XlPageSetup PageSetup {
			get { return subject.PageSetup; }
			set { subject.PageSetup = value; }
		}
		public XlHeaderFooter HeaderFooter {
			get { return subject.HeaderFooter; }
		}
		public XlPrintTitles PrintTitles {
			get { return subject.PrintTitles; }
		}
		public XlCellRange PrintArea {
			get { return subject.PrintArea; }
			set { subject.PrintArea = value; }
		}
		public XlPrintOptions PrintOptions {
			get { return subject.PrintOptions; }
			set { subject.PrintOptions = value; }
		}
		public IXlPageBreaks ColumnPageBreaks {
			get { return subject.ColumnPageBreaks; }
		}
		public IXlPageBreaks RowPageBreaks {
			get { return subject.RowPageBreaks; }
		}
		public IList<XlHyperlink> Hyperlinks {
			get { return subject.Hyperlinks; }
		}
		public XlCellRange DataRange {
			get { return subject.DataRange; }
		}
		public XlCellRange ColumnRange {
			get { return subject.ColumnRange; }
		}
		public XlIgnoreErrors IgnoreErrors {
			get { return subject.IgnoreErrors; }
			set { subject.IgnoreErrors = value; }
		}
		public IXlOutlineProperties OutlineProperties {
			get { return subject.OutlineProperties; }
		}
		public IXlSheetViewOptions ViewOptions {
			get { return subject.ViewOptions; }
		}
		public IList<XlSparklineGroup> SparklineGroups {
			get { return subject.SparklineGroups; }
		}
		public int CurrentOutlineLevel {
			get { return subject.CurrentOutlineLevel; }
		}
		public int BeginGroup(bool collapsed) {
			return subject.BeginGroup(collapsed);
		}
		public int BeginGroup(int outlineLevel, bool collapsed) {
			return subject.BeginGroup(outlineLevel, collapsed);
		}
		public void EndGroup() {
			subject.EndGroup();
		}
		public IXlColumn CreateColumn() {
			return subject.CreateColumn();
		}
		public IXlColumn CreateColumn(int columnIndex) {
			return subject.CreateColumn(columnIndex);
		}
		public IXlRow CreateRow() {
			return subject.CreateRow();
		}
		public IXlRow CreateRow(int rowIndex) {
			return subject.CreateRow(rowIndex);
		}
		public void SkipColumns(int count) {
			subject.SkipColumns(count);
		}
		public void SkipRows(int count) {
			subject.SkipRows(count);
		}
		public int CurrentRowIndex {
			get { return subject.CurrentRowIndex; }
		}
		public int CurrentColumnIndex {
			get { return subject.CurrentColumnIndex; }
		}
		public IXlPicture CreatePicture() {
			return subject.CreatePicture();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if (exporter != null) {
				exporter.EndSheet();
				exporter = null;
			}
		}
		#endregion
		#region IXlShapeContainer Members
		IList<XlShape> IXlShapeContainer.Shapes {
			get { 
				IXlShapeContainer container = subject as IXlShapeContainer;
				if(container == null)
					return null;
				return container.Shapes;
			}
		}
		#endregion
	}
	#endregion
	#region XlSheet
	public class XlSheet : IXlSheet, IXlOutlineProperties, IXlSheetViewOptions, IXlShapeContainer {
		#region Fields
		static char[] illegalCharacters = new char[] { '\x0000', '\x0003', ':', '\\', '*', '?', '/', '[', ']' };
		readonly XlMergedCellsCollection mergedCells = new XlMergedCellsCollection();
		readonly List<XlConditionalFormatting> conditionalFormattings = new List<XlConditionalFormatting>();
		readonly List<XlDataValidation> dataValidations = new List<XlDataValidation>();
		readonly XlHeaderFooter headerFooter = new XlHeaderFooter();
		readonly XlPrintTitles printTitles;
		readonly XlPageBreaksCollection columnPageBreaks;
		readonly XlPageBreaksCollection rowPageBreaks;
		readonly List<XlHyperlink> hyperlinks = new List<XlHyperlink>();
		readonly List<XlTable> tables = new List<XlTable>();
		readonly List<XlSparklineGroup> sparklineGroups = new List<XlSparklineGroup>();
		readonly List<XlShape> shapes = new List<XlShape>();
		IXlExport exporter;
		int leftColumnIndex = -1;
		int rightColumnIndex = -1;
		int topRowIndex = -1;
		int bottomRowIndex = -1;
		int firstColumnIndex = -1;
		int lastColumnIndex = -1;
		bool summaryBelow = false;
		bool summaryRight = true;
		string name;
		bool showFormulas = false;
		bool showGridLines = true;
		bool showRowColumnHeaders = true;
		bool showZeroValues = true;
		bool showOutlineSymbols = true;
		bool rightToLeft = false;
		#endregion
		public XlSheet(IXlExport exporter) {
			this.exporter = exporter;
			this.printTitles = new XlPrintTitles(this);
			this.columnPageBreaks = new XlPageBreaksCollection(exporter != null ? exporter.DocumentOptions.MaxColumnCount - 1 : 16383);
			this.rowPageBreaks = new XlPageBreaksCollection(exporter != null ? exporter.DocumentOptions.MaxRowCount - 1 : 1048575);
			DefaultMaxDigitWidthInPixels = 7; 
			DefaultColumnWidthInPixels = 64;
			DefaultRowHeightInPixels = 20;
			VisibleState = XlSheetVisibleState.Visible;
			IgnoreErrors = XlIgnoreErrors.NumberStoredAsText;
		}
		#region Properties
		public string Name {
			get { return name; }
			set {
				CheckSheetName(value);
				name = value;
			}
		}
		public IXlMergedCells MergedCells { get { return mergedCells; } }
		public XlCellPosition SplitPosition { get; set; }
		public XlCellRange AutoFilterRange { get; set; }
		public float DefaultMaxDigitWidthInPixels { get; private set; }
		public float DefaultColumnWidthInPixels { get; private set; }
		public float DefaultRowHeightInPixels { get; private set; }
		public IList<XlConditionalFormatting> ConditionalFormattings { get { return conditionalFormattings; } }
		public IList<XlDataValidation> DataValidations { get { return dataValidations; } }
		public XlSheetVisibleState VisibleState { get; set; }
		public XlPageMargins PageMargins { get; set; }
		public XlPageSetup PageSetup { get; set; }
		public XlHeaderFooter HeaderFooter { get { return headerFooter; } }
		public XlPrintTitles PrintTitles { get { return printTitles; } }
		public XlCellRange PrintArea { get; set; }
		public XlPrintOptions PrintOptions { get; set; }
		public IXlPageBreaks ColumnPageBreaks { get { return columnPageBreaks; } }
		public IXlPageBreaks RowPageBreaks { get { return rowPageBreaks; } }
		public IList<XlHyperlink> Hyperlinks { get { return hyperlinks; } }
		public XlCellRange DataRange {
			get {
				if (leftColumnIndex < 0) 
					return null;
				return new XlCellRange(new XlCellPosition(leftColumnIndex, topRowIndex), new XlCellPosition(rightColumnIndex, bottomRowIndex));
			}
		}
		public XlCellRange ColumnRange {
			get {
				if (firstColumnIndex < 0) 
					return null;
				return new XlCellRange(new XlCellPosition(firstColumnIndex, -1), new XlCellPosition(lastColumnIndex, -1));
			}
		}
		public XlIgnoreErrors IgnoreErrors { get; set; }
		public IXlOutlineProperties OutlineProperties { get { return this; } }
		public IList<XlTable> Tables { get { return tables; } }
		public IXlSheetViewOptions ViewOptions { get { return this; } }
		public IList<XlSparklineGroup> SparklineGroups { get { return sparklineGroups; } }
		#endregion
		#region Registration
		internal void RegisterCellPosition(IXlCell cell) {
			if (cell == null)
				return;
			if (leftColumnIndex < 0)
				leftColumnIndex = cell.ColumnIndex;
			else
				leftColumnIndex = Math.Min(leftColumnIndex, cell.ColumnIndex);
			if (rightColumnIndex < 0)
				rightColumnIndex = cell.ColumnIndex;
			else
				rightColumnIndex = Math.Max(rightColumnIndex, cell.ColumnIndex);
			if (topRowIndex < 0)
				topRowIndex = cell.RowIndex;
			else
				topRowIndex = Math.Min(topRowIndex, cell.RowIndex);
			if (bottomRowIndex < 0)
				bottomRowIndex = cell.RowIndex;
			else
				bottomRowIndex = Math.Max(bottomRowIndex, cell.RowIndex);
		}
		internal void RegisterColumnIndex(IXlColumn column) {
			if (column == null)
				return;
			if (firstColumnIndex < 0)
				firstColumnIndex = column.ColumnIndex;
			else
				firstColumnIndex = Math.Min(firstColumnIndex, column.ColumnIndex);
			if (lastColumnIndex < 0)
				lastColumnIndex = column.ColumnIndex;
			else
				lastColumnIndex = Math.Max(lastColumnIndex, column.ColumnIndex);
		}
		#endregion
		#region IXlOutlineProperties Members
		bool IXlOutlineProperties.SummaryBelow {
			get { return summaryBelow; }
			set { summaryBelow = value; }
		}
		bool IXlOutlineProperties.SummaryRight {
			get { return summaryRight; }
			set { summaryRight = value; }
		}
		#endregion
		public int CurrentOutlineLevel {
			get { return exporter.CurrentOutlineLevel; }
		}
		public int BeginGroup(bool collapsed) {
			int currentLevel = exporter.CurrentOutlineLevel;
			IXlGroup group = exporter.BeginGroup();
			group.OutlineLevel = currentLevel + 1;
			if (collapsed)
				group.IsCollapsed = collapsed;
			return exporter.CurrentOutlineLevel;
		}
		public int BeginGroup(int outlineLevel, bool collapsed) {
			if (outlineLevel < 1 || outlineLevel > 8)
				throw new ArgumentOutOfRangeException("Outline level out of range 1..8!");
			IXlGroup group = exporter.BeginGroup();
			group.OutlineLevel = outlineLevel;
			if (collapsed)
				group.IsCollapsed = collapsed;
			return exporter.CurrentOutlineLevel;
		}
		public void EndGroup() {
			exporter.EndGroup();
		}
		public IXlColumn CreateColumn() {
			return new XlColumnProxy(exporter, exporter.BeginColumn());
		}
		public IXlColumn CreateColumn(int columnIndex) {
			int count = columnIndex - exporter.CurrentColumnIndex;
			if(count < 0)
				throw new ArgumentException("Value must be greater than or equals to current column index.");
			if(count > 0)
				SkipColumns(count);
			return new XlColumnProxy(exporter, exporter.BeginColumn());
		}
		public IXlRow CreateRow() {
			return new XlRowProxy(exporter, exporter.BeginRow());
		}
		public IXlRow CreateRow(int rowIndex) {
			int count = rowIndex - exporter.CurrentRowIndex;
			if(count < 0)
				throw new ArgumentException("Value must be greater than or equals to current row index.");
			if(count > 0)
				SkipRows(count);
			return new XlRowProxy(exporter, exporter.BeginRow());
		}
		public void SkipColumns(int count) {
			exporter.SkipColumns(count);
		}
		public void SkipRows(int count) {
			exporter.SkipRows(count);
		}
		public int CurrentRowIndex {
			get { return exporter.CurrentRowIndex; }
		}
		public int CurrentColumnIndex {
			get { return exporter.CurrentColumnIndex; }
		}
		public IXlPicture CreatePicture() {
			return exporter.BeginPicture();
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			exporter = null;
		}
		#endregion
		void CheckSheetName(string value) {
			if(string.IsNullOrEmpty(value) || value.Length > 31)
				throw new ArgumentException("Worksheet name must be greater than or equal to 1 and less than or equal to 31 characters long.");
			if(value.IndexOfAny(illegalCharacters) >= 0 || value[0] == '\'' || value[value.Length-1] == '\'')
				throw new ArgumentException("Worksheet name contains illegal characters.");
		}
		#region IXlSheetViewOptions Members
		bool IXlSheetViewOptions.ShowFormulas {
			get { return showFormulas; }
			set { showFormulas = value; }
		}
		bool IXlSheetViewOptions.ShowGridLines {
			get { return showGridLines; }
			set { showGridLines = value; }
		}
		bool IXlSheetViewOptions.ShowRowColumnHeaders {
			get { return showRowColumnHeaders; }
			set { showRowColumnHeaders = value; }
		}
		bool IXlSheetViewOptions.ShowZeroValues {
			get { return showZeroValues; }
			set { showZeroValues = value; }
		}
		bool IXlSheetViewOptions.ShowOutlineSymbols {
			get { return showOutlineSymbols; }
			set { showOutlineSymbols = value; }
		}
		bool IXlSheetViewOptions.RightToLeft {
			get { return rightToLeft; }
			set { rightToLeft = value; }
		}
		#endregion
		#region IXlShapeContainer Members
		IList<XlShape> IXlShapeContainer.Shapes {
			get { return shapes; }
		}
		#endregion
	}
	#endregion
	#region XlGroup
	public class XlGroup : IXlGroup {
		public int OutlineLevel { get; set; }
		public bool IsCollapsed { get; set; }
	}
	#endregion
	#region XlRowProxy
	class XlRowProxy : IXlRow {
		IXlExport exporter;
		readonly IXlRow subject;
		public XlRowProxy(IXlExport exporter, IXlRow subject) {
			this.exporter = exporter;
			this.subject = subject;
		}
		#region IXlRow Members
		public int RowIndex {
			get {
				return subject.RowIndex;
			}
		}
		public XlCellFormatting Formatting {
			get {
				return subject.Formatting;
			}
			set {
				subject.Formatting = value;
			}
		}
		public bool IsHidden {
			get {
				return subject.IsHidden;
			}
			set {
				subject.IsHidden = value;
			}
		}
		public bool IsCollapsed {
			get {
				return subject.IsCollapsed;
			}
			set {
				subject.IsCollapsed = value;
			}
		}
		public int HeightInPixels {
			get {
				return subject.HeightInPixels;
			}
			set {
				subject.HeightInPixels = value;
			}
		}
		public IXlCell CreateCell() {
			return subject.CreateCell();
		}
		public IXlCell CreateCell(int columnIndex) {
			return subject.CreateCell(columnIndex);
		}
		public void ApplyFormatting(XlCellFormatting formatting) {
			subject.ApplyFormatting(formatting);
		}
		public void BlankCells(int count, XlCellFormatting formatting) {
			subject.BlankCells(count, formatting);
		}
		public void SkipCells(int count) {
			subject.SkipCells(count);
		}
		public void BulkCells(IEnumerable values, XlCellFormatting formatting) {
			subject.BulkCells(values, formatting);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if (exporter != null) {
				exporter.EndRow();
				exporter = null;
			}
		}
		#endregion
	}
	#endregion
	#region XlRow
	public class XlRow : IXlRow {
		IXlExport exporter;
		public XlRow(IXlExport exporter) {
			this.exporter = exporter;
			HeightInPixels = -1;
		}
		public int RowIndex { get; set; }
		public XlCellFormatting Formatting { get; set; }
		public bool IsHidden { get; set; }
		public bool IsCollapsed { get; set; }
		public int HeightInPixels { get; set; }
		public IXlCell CreateCell() {
			return new XlCellProxy(exporter, exporter.BeginCell());
		}
		public IXlCell CreateCell(int columnIndex) {
			int count = columnIndex - exporter.CurrentColumnIndex;
			if(count < 0)
				throw new ArgumentException("Value must be greater than or equals to current column index.");
			if(count > 0)
				SkipCells(count);
			return new XlCellProxy(exporter, exporter.BeginCell());
		}
		#region IDisposable Members
		public void Dispose() {
			exporter = null;
		}
		#endregion
		public void ApplyFormatting(XlCellFormatting formatting) {
			if (formatting != null) {
				if (this.Formatting == null)
					this.Formatting = new XlCellFormatting();
				this.Formatting.MergeWith(formatting);
			}
		}
		public void BlankCells(int count, XlCellFormatting formatting) {
			Guard.ArgumentPositive(count, "count");
			Guard.ArgumentNotNull(formatting, "formatting");
			for (int i = 0; i < count; i++) {
				using (IXlCell cell = CreateCell()) {
					cell.ApplyFormatting(formatting);
				}
			}
		}
		public void BulkCells(IEnumerable values, XlCellFormatting formatting) {
			Guard.ArgumentNotNull(values, "values");
			foreach(object value in values) {
				using(IXlCell cell = CreateCell()) {
					cell.Value = XlVariantValue.FromObject(value);
					cell.ApplyFormatting(formatting);
				}
			}
		}
		public void SkipCells(int count) {
			exporter.SkipCells(count);
		}
	}
	#endregion
	#region XlColumnProxy
	class XlColumnProxy : IXlColumn {
		IXlExport exporter;
		readonly IXlColumn subject;
		public XlColumnProxy(IXlExport exporter, IXlColumn subject) {
			this.exporter = exporter;
			this.subject = subject;
		}
		#region IXlColumn Members
		public int ColumnIndex {
			get {
				return subject.ColumnIndex;
			}
		}
		public XlCellFormatting Formatting {
			get {
				return subject.Formatting;
			}
			set {
				subject.Formatting = value;
			}
		}
		public bool IsHidden {
			get {
				return subject.IsHidden;
			}
			set {
				subject.IsHidden = value;
			}
		}
		public bool IsCollapsed {
			get {
				return subject.IsCollapsed;
			}
			set {
				subject.IsCollapsed = value;
			}
		}
		public int WidthInPixels {
			get {
				return subject.WidthInPixels;
			}
			set {
				subject.WidthInPixels = value;
			}
		}
		public float WidthInCharacters {
			get {
				return subject.WidthInCharacters;
			}
			set {
				subject.WidthInCharacters = value;
			}
		}
		public void ApplyFormatting(XlCellFormatting formatting) {
			subject.ApplyFormatting(formatting);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if (exporter != null) {
				exporter.EndColumn();
				exporter = null;
			}
		}
		#endregion
	}
	#endregion
	#region XlColumn
	public class XlColumn : IXlColumn {
		XlSheet sheet;
		int widthInPixels = -1;
		public XlColumn(XlSheet sheet) {
			this.sheet = sheet;
		}
		public int ColumnIndex { get; set; }
		public XlCellFormatting Formatting { get; set; }
		public bool IsHidden { get; set; }
		public bool IsCollapsed { get; set; }
		public int OutlineLevel { get; set; }
		public int WidthInPixels {
			get { return widthInPixels; }
			set {
				if (value < 0)
					value = -1;
				widthInPixels = value;
			}
		}
		public float WidthInCharacters {
			get {
				if (widthInPixels < 0)
					return -0.08f;
				float widthWithGaps = ColumnWidthConverter.PixelsToCharactersWidth(widthInPixels, sheet.DefaultMaxDigitWidthInPixels);
				if (widthWithGaps >= 1.71)
					return ColumnWidthConverter.PixelsToCharactersWidth(widthInPixels - 5, sheet.DefaultMaxDigitWidthInPixels);
				return ColumnWidthConverter.PixelsToCharactersWidth((float)(widthInPixels - 5 * widthWithGaps / 1.71), sheet.DefaultMaxDigitWidthInPixels);
			}
			set {
				if (value < 0)
					widthInPixels = -1;
				else
					widthInPixels = ColumnWidthConverter.CharactersWidthToPixels(value, sheet.DefaultMaxDigitWidthInPixels);
			}
		}
		#region IDisposable Members
		public void Dispose() {
			sheet = null;
		}
		#endregion
		public void ApplyFormatting(XlCellFormatting formatting) {
			if (formatting != null) {
				if (this.Formatting == null)
					this.Formatting = new XlCellFormatting();
				this.Formatting.MergeWith(formatting);
			}
		}
	}
	#endregion
	#region XlCellProxy
	class XlCellProxy : IXlCell {
		IXlExport exporter;
		readonly IXlCell subject;
		public XlCellProxy(IXlExport exporter, IXlCell subject) {
			this.exporter = exporter;
			this.subject = subject;
		}
		#region IXlCell Members
		public XlVariantValue Value {
			get { return subject.Value; }
			set { subject.Value = value; }
		}
		public int RowIndex {
			get { return subject.RowIndex; }
		}
		public int ColumnIndex {
			get { return subject.ColumnIndex; }
		}
		public XlCellPosition Position {
			get { return subject.Position; }
		}
		public XlCellFormatting Formatting {
			get { return subject.Formatting; }
			set { subject.Formatting = value; }
		}
		public void ApplyFormatting(XlCellFormatting formatting) {
			subject.ApplyFormatting(formatting);
		}
		public void SetFormula(IXlFormulaParameter formula) {
			subject.SetFormula(formula);
		}
		public void SetFormula(XlExpression formula) {
			subject.SetFormula(formula);
		}
		public void SetFormula(string formula) {
			subject.SetFormula(formula);
		}
		public void SetSharedFormula(XlExpression formula, XlCellRange range) {
			subject.SetSharedFormula(formula, range);
		}
		public void SetSharedFormula(string formula, XlCellRange range) {
			subject.SetSharedFormula(formula, range);
		}
		public void SetSharedFormula(XlCellPosition hostCell) {
			subject.SetSharedFormula(hostCell);
		}
		public void SetRichText(XlRichTextString value) {
			subject.SetRichText(value);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if (exporter != null) {
				exporter.EndCell();
				exporter = null;
			}
		}
		#endregion
	}
	#endregion
	#region XlCell
	public class XlCell : IXlCell {
		string formulaString;
		XlVariantValue variantValue;
		XlRichTextString richTextValue;
		public XlVariantValue Value {
			get { return variantValue; }
			set {
				variantValue = value;
				richTextValue = null;
			}
		}
		public int RowIndex { get; internal set; }
		public int ColumnIndex { get; set; }
		public XlCellPosition Position { get { return new XlCellPosition(ColumnIndex, RowIndex); } }
		public XlCellFormatting Formatting { get; set; }
		internal IXlFormulaParameter Formula { get; set; }
		internal XlExpression Expression { get; set; }
		internal string FormulaString {
			get { return formulaString; }
			set {
				if (!string.IsNullOrEmpty(value) && value[0] == '=')
					formulaString = value.Substring(1);
				else
					formulaString = value;
			}
		}
		internal XlCellRange SharedFormulaRange { get; set; }
		internal XlCellPosition SharedFormulaPosition { get; set; }
		internal bool HasFormula { get { return Formula != null || Expression != null || !string.IsNullOrEmpty(FormulaString) || SharedFormulaPosition.IsValid; } }
		internal bool HasFormulaWithoutValue { get { return HasFormula && Value.IsEmpty; } }
		internal XlRichTextString RichTextValue { get { return richTextValue; } }
		public XlCell() {
			SharedFormulaPosition = XlCellPosition.InvalidValue;
		}
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		public void ApplyFormatting(XlCellFormatting formatting) {
			if (formatting != null) {
				if (this.Formatting == null)
					this.Formatting = new XlCellFormatting();
				this.Formatting.MergeWith(formatting);
			}
		}
		public void SetFormula(IXlFormulaParameter formula) {
			this.Formula = formula;
			this.Expression = null;
			this.FormulaString = null;
			this.SharedFormulaRange = null;
			this.SharedFormulaPosition = XlCellPosition.InvalidValue;
		}
		public void SetFormula(XlExpression formula) {
			this.Formula = null;
			this.Expression = formula;
			this.FormulaString = null;
			this.SharedFormulaRange = null;
			this.SharedFormulaPosition = XlCellPosition.InvalidValue;
		}
		public void SetFormula(string formula) {
			this.Formula = null;
			this.Expression = null;
			this.FormulaString = formula;
			this.SharedFormulaRange = null;
			this.SharedFormulaPosition = XlCellPosition.InvalidValue;
		}
		public void SetSharedFormula(XlExpression formula, XlCellRange range) {
			this.Formula = null;
			this.Expression = formula;
			this.FormulaString = null;
			this.SharedFormulaRange = range;
			this.SharedFormulaPosition = XlCellPosition.InvalidValue;
		}
		public void SetSharedFormula(string formula, XlCellRange range) {
			this.Formula = null;
			this.Expression = null;
			this.FormulaString = formula;
			this.SharedFormulaRange = range;
			this.SharedFormulaPosition = XlCellPosition.InvalidValue;
		}
		public void SetSharedFormula(XlCellPosition hostCell) {
			this.Formula = null;
			this.Expression = null;
			this.FormulaString = null;
			this.SharedFormulaRange = null;
			this.SharedFormulaPosition = hostCell;
		}
		public void SetRichText(XlRichTextString value) {
			richTextValue = value;
			if(value == null)
				variantValue = XlVariantValue.Empty;
			else
				variantValue = value.Text;
		}
	}
	#endregion
	#region XlPicture
	public class XlPicture : XlDrawingObject, IXlPicture {
		IXlExport exporter;
		readonly XlPictureHyperlink hyperlinkClick = new XlPictureHyperlink();
		public XlPicture(IXlExport exporter) {
			this.exporter = exporter;
		}
		public Image Image { get; set; }
		public ImageFormat Format { get; set; }
		public XlPictureHyperlink HyperlinkClick { get { return hyperlinkClick; } }
		public void FitToCell(XlCellPosition position, int cellWidth, int cellHeight, bool center) {
			if (Image == null)
				throw new InvalidOperationException("Image is not specified!");
			int x = 0;
			int y = 0;
			int width = cellWidth;
			int height = width * Image.Height / Image.Width;
			if (height > cellHeight) {
				height = cellHeight;
				width = height * Image.Width / Image.Height;
			}
			if (center) {
				x = (cellWidth - width) / 2;
				y = (cellHeight - height) / 2;
			}
			AnchorType = XlAnchorType.TwoCell;
			AnchorBehavior = XlAnchorType.TwoCell;
			TopLeft = new XlAnchorPoint(position.Column, position.Row, x, y, cellWidth, cellHeight);
			BottomRight = new XlAnchorPoint(position.Column, position.Row, x + width, y + height, cellWidth, cellHeight);
		}
		public void StretchToCell(XlCellPosition position) {
			AnchorType = XlAnchorType.TwoCell;
			AnchorBehavior = XlAnchorType.TwoCell;
			TopLeft = new XlAnchorPoint(position.Column, position.Row);
			BottomRight = new XlAnchorPoint(position.Column + 1, position.Row + 1);
		}
		#region IDisposable Members
		public void Dispose() {
			if (exporter != null) {
				exporter.EndPicture();
				exporter = null;
			}
		}
		#endregion
		internal byte[] GetImageBytes(ImageFormat format) {
			if (Image == null)
				return null;
			using (MemoryStream ms = new MemoryStream()) {
				Metafile metafile = Image as Metafile;
				if (metafile != null) {
					float targetDpi = 200f;
					Size targetSize = new Size(
						(int)(Image.Width * targetDpi / Image.HorizontalResolution),
						(int)(Image.Height * targetDpi / Image.VerticalResolution));
					int width = (int)(targetSize.Width * DpiX / 96.0);
					int height = (int)(targetSize.Height * DpiY / 96.0);
					using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb)) {
						using (Graphics graphics = Graphics.FromImage(bitmap)) {
							graphics.DrawImage(metafile, 0, 0, width, height);
							bitmap.Save(ms, format);
						}
					}
				}
				else
					Image.Save(ms, format);
				return ms.ToArray();
			}
		}
		float DpiX { get { return DevExpress.XtraPrinting.GraphicsDpi.Pixel; } }
		float DpiY { get { return DevExpress.XtraPrinting.GraphicsDpi.Pixel; } }
	}
#endregion
	#region ColumnWidthConverter
	static class ColumnWidthConverter {
		public static float PixelsToCharactersWidth(float pixels, float maxDigitWidthInPixels) {
			return Math.Min(255, Math.Max(0, (int)(pixels / maxDigitWidthInPixels * 100.0f + 0.5f) / 100.0f));
		}
		public static int CharactersWidthToPixels(float widthInCharacters, float maxDigitWidthInPixels) {
			double padding = widthInCharacters >= 1 ? 5 : 5 * widthInCharacters;
			double width = Math.Truncate((widthInCharacters * maxDigitWidthInPixels + padding) / maxDigitWidthInPixels * 256) / 256;
			return (int)Math.Truncate(((256 * width + Math.Truncate(128 / maxDigitWidthInPixels)) / 256) * maxDigitWidthInPixels);
		}
	}
	#endregion
	#region XlNetNumberFormat
	class XlNetNumberFormat {
		string formatString = string.Empty;
		public string FormatString {
			get { return formatString; }
			set {
				if (string.IsNullOrEmpty(value))
					formatString = string.Empty;
				else
					formatString = value;
			}
		}
		public bool IsDateTimeFormat { get; set; }
		public override bool Equals(object obj) {
			XlNetNumberFormat other = obj as XlNetNumberFormat;
			if (other == null)
				return false;
			return IsDateTimeFormat == other.IsDateTimeFormat && string.Equals(FormatString, other.FormatString);
		}
		public override int GetHashCode() {
			return formatString.GetHashCode() ^ IsDateTimeFormat.GetHashCode();
		}
	}
	#endregion
	#region XlCalculationOptions
	internal class XlCalculationOptions {
		public bool FullCalculationOnLoad { get; set; }
	}
	#endregion
}
