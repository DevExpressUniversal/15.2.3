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
using DevExpress.Office;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
#if !SL
using System.Data;
using DevExpress.DataAccess.Native.ObjectBinding;
using System.Drawing;
using System.Drawing.Printing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Spreadsheet.Charts;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.Spreadsheet {
	#region WorksheetVisibilityType
	[Flags]
	public enum WorksheetVisibilityType {
		Visible = DevExpress.XtraSpreadsheet.Model.SheetVisibleState.Visible,
		Hidden = DevExpress.XtraSpreadsheet.Model.SheetVisibleState.Hidden,
		VeryHidden = DevExpress.XtraSpreadsheet.Model.SheetVisibleState.VeryHidden
	}
	#endregion
	#region WorksheetViewType
	public enum WorksheetViewType {
		Normal = DevExpress.XtraSpreadsheet.Model.SheetViewType.Normal,
		PageLayout = DevExpress.XtraSpreadsheet.Model.SheetViewType.PageLayout,
		PageBreakPreview = DevExpress.XtraSpreadsheet.Model.SheetViewType.PageBreakPreview
	}
	#endregion
	#region WorksheetPageNumberingStartType
	public enum WorksheetPageNumberingStartType {
		Continuous,
		Restart
	}
	#endregion
	#region PageOrientation
	public enum PageOrientation {
		Default = DevExpress.XtraSpreadsheet.Model.ModelPageOrientation.Default,
		Landscape = DevExpress.XtraSpreadsheet.Model.ModelPageOrientation.Landscape,
		Portrait = DevExpress.XtraSpreadsheet.Model.ModelPageOrientation.Portrait
	}
	#endregion
	#region PageOrder
	public enum PageOrder {
		DownThenOver = DevExpress.XtraSpreadsheet.Model.PagePrintOrder.DownThenOver,
		OverThenDown = DevExpress.XtraSpreadsheet.Model.PagePrintOrder.OverThenDown,
	}
	#endregion
	#region CommentsPrintMode
	public enum CommentsPrintMode {
		AsDisplayed = DevExpress.XtraSpreadsheet.Model.ModelCommentsPrintMode.AsDisplayed,
		AtEnd = DevExpress.XtraSpreadsheet.Model.ModelCommentsPrintMode.AtEnd,
		None = DevExpress.XtraSpreadsheet.Model.ModelCommentsPrintMode.None,
	}
	#endregion
	#region ErrorsPrintMode
	public enum ErrorsPrintMode {
		Blank = DevExpress.XtraSpreadsheet.Model.ModelErrorsPrintMode.Blank,
		Dash = DevExpress.XtraSpreadsheet.Model.ModelErrorsPrintMode.Dash,
		Displayed = DevExpress.XtraSpreadsheet.Model.ModelErrorsPrintMode.Displayed,
		NA = DevExpress.XtraSpreadsheet.Model.ModelErrorsPrintMode.NA
	}
	#endregion
	#region IDataValueConverter
	public interface IDataValueConverter {
		bool TryConvert(object value, int index, out CellValue result);
	}
	#endregion
	public class DataImportOptions {
		public DataImportOptions() {
			ImportFormulas = false;
			ReferenceStyle = Spreadsheet.ReferenceStyle.UseDocumentSettings;
			Converter = null;
		}
		public bool ImportFormulas { get; set; }
		public ReferenceStyle ReferenceStyle { get; set; }
		public IDataValueConverter Converter { get; set; }
		public System.Globalization.CultureInfo FormulaCulture { get; set; }
	}
	public class DataSourceImportOptions : DataImportOptions {
		public DataSourceImportOptions()
			: base() {
			PropertyNames = null;
		}
		public string[] PropertyNames { get; set; }
	}
	[Flags]
	public enum MergeCellsMode {
		Default = 0,
		IgnoreIntersections = 1,
		ByRows = 2,
		ByColumns = 4,
	}
	public class SortField {
		public IComparer<CellValue> Comparer { get; set; }
		public int ColumnOffset { get; set; }
	}
	public interface IComparers {
		IComparer<CellValue> Ascending { get; }
		IComparer<CellValue> Descending { get; }
	}
	#region WorksheetOutlineOptions
	public interface WorksheetOutlineOptions {
		bool SummaryRowsBelow { get; set; }
		bool SummaryColumnsRight { get; set; }
	}
	#endregion
	#region Worksheet
	public interface Worksheet : ExternalWorksheet {
		CellCollection Cells { get; }
		RowCollection Rows { get; }
		ColumnCollection Columns { get; }
		HyperlinkCollection Hyperlinks { get; }
		WorksheetView ActiveView { get; }
		WorksheetPrintOptions PrintOptions { get; }
		WorksheetHeaderFooterOptions HeaderFooterOptions { get; }
		ArrayFormulaCollection ArrayFormulas { get; }
		CommentCollection Comments { get; }
		PageBreaksCollection VerticalPageBreaks { get; }
		PageBreaksCollection HorizontalPageBreaks { get; }
		TableCollection Tables { get; }
		new string Name { get; set; }
		int Index { get; }
		bool Visible { get; set; }
		WorksheetVisibilityType VisibilityType { get; set; }
		IWorkbook Workbook { get; }
		float DefaultColumnWidthInCharacters { get; set; }
		int DefaultColumnWidthInPixels { get; set; }
		float DefaultColumnWidth { get; set; }
		double DefaultRowHeight { get; set; }
		new DefinedNameCollection DefinedNames { get; }
		ShapeCollection Shapes { get; }
		PictureCollection Pictures { get; }
		ChartCollection Charts { get; }
		ConditionalFormattingCollection ConditionalFormattings { get; }
		IRangeProvider Range { get; }
		void AutoOutline();
		void ClearOutline();
		void Subtotal(Range range, int groupByColumn, List<int> subtotalColumnList, int functionCode, string functionText);
		void RemoveSubtotal(Range range);
		WorksheetOutlineOptions OutlineOptions { get; }
		void AddPrintRange(Range range);
		void ClearPrintRange();
		void SetPrintRange(Range range);
		Range GetDataRange();
		Range GetPrintableRange();
		Range GetPrintableRange(bool usePrintAreaDefinedName);
		Range GetUsedRange();
		Range this[string reference] { get; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		Cell this[int rowIndex, int columnIndex] { get; }
		void Move(int position);
		void CopyFrom(Worksheet source);
		IEnumerable<Cell> GetExistingCells();
		Range SelectedCell { get; set; }
		Range Selection { get; set; }
		IList<Range> GetSelectedRanges();
		bool SetSelectedRanges(IList<Range> ranges);
		bool SetSelectedRanges(IList<Range> ranges, bool expandToMergedCellsSize);
		Shape SelectedShape { get; set; }
		Picture SelectedPicture { get; set; }
		Chart SelectedChart { get; set; }
		IList<Shape> GetSelectedShapes();
		bool SetSelectedShapes(IList<Shape> shapes);
		void Clear(Range range);
		void ClearContents(Range range);
		void ClearFormats(Range range);
		void ClearHyperlinks(Range range);
		void ClearComments(Range range);
		void MergeCells(Range range);
		void MergeCells(Range range, MergeCellsMode mode);
		void UnMergeCells(Range range);
		void InsertCells(Range range);
		void DeleteCells(Range range);
		void InsertCells(Range range, InsertCellsMode mode);
		void DeleteCells(Range range, DeleteMode mode);
		int SplitLeftPosition { get; }
		int SplitTopPosition { get; }
		void FreezePanes(int rowOffset, int columnOffset);
		void FreezePanes(int rowOffset, int columnOffset, Range topLeft);
		void FreezeColumns(int columnOffset);
		void FreezeColumns(int columnOffset, Range topLeft);
		void FreezeRows(int rowOffset);
		void FreezeRows(int rowOffset, Range topLeft);
		void UnfreezePanes();
		void ScrollToRow(int rowIndex);
		void ScrollToRow(string rowHeading);
		void ScrollToColumn(int columnIndex);
		void ScrollToColumn(string columnHeading);
		void ScrollTo(int rowIndex, int columnIndex);
		void ScrollTo(Range scrolledAreaTopLeftCell);
		IEnumerable<Cell> Search(string text);
		IEnumerable<Cell> Search(string text, SearchOptions options);
		void Sort(Range range);
		void Sort(Range range, int columnOffset);
		void Sort(Range range, bool ascending);
		void Sort(Range range, int columnOffset, bool ascending);
		void Sort(Range range, int columnOffset, IComparer<CellValue> comparer);
		void Sort(Range range, IEnumerable<SortField> sortFields);
		IComparers Comparers { get; }
		bool IsProtected { get; }
		void Protect(string password, WorksheetProtectionPermissions permissions);
		bool Unprotect(string password);
		ProtectedRangeCollection ProtectedRanges { get; }
		object Tag { get; set; }
		SheetAutoFilter AutoFilter { get; }
		DataValidationCollection DataValidations { get; }
		SparklineGroupCollection SparklineGroups { get; }
		IgnoredErrorCollection IgnoredErrors { get; }
		PivotTableCollection PivotTables { get; }
		void Calculate();
	}
	#endregion
	[Flags]
	public enum WorksheetProtectionPermissions {
		SelectLockedCells = 0x000000001,
		SelectUnlockedCells = 0x000000002,
		FormatCells = 0x000000004,
		FormatColumns = 0x000000008,
		FormatRows = 0x000000010,
		InsertColumns = 0x000000020,
		InsertRows = 0x000000040,
		InsertHyperlinks = 0x000000080,
		DeleteColumns = 0x000000100,
		DeleteRows = 0x000000200,
		Sort = 0x000000400,
		AutoFilters = 0x000000800,
		PivotTables = 0x000001000,
		Objects = 0x000002000,
		Scenarios = 0x000004000,
		Default = SelectLockedCells | SelectUnlockedCells
	}
	#region Margins
	public interface Margins {
		float Left { get; set; }
		float Top { get; set; }
		float Right { get; set; }
		float Bottom { get; set; }
		float Header { get; set; }
		float Footer { get; set; }
	}
	#endregion
	#region WorksheetView
	public interface WorksheetView {
		Margins Margins { get; }
		PageOrientation Orientation { get; set; }
		PaperKind PaperKind { get; set; }
		WorksheetViewType ViewType { get; set; }
		bool ShowFormulas { get; set; }
		bool ShowGridlines { get; set; }
		bool ShowHeadings { get; set; }
		int Zoom { get; set; }
		Color TabColor { get; set; }
	}
	#endregion
	#region WorksheetPrintOptions
	public interface WorksheetPrintOptions {
		WorksheetPageNumbering PageNumbering { get; }
		int FitToWidth { get; set; }
		int FitToHeight { get; set; }
		bool FitToPage { get; set; }
		int Scale { get; set; }
		bool CenterHorizontally { get; set; }
		bool CenterVertically { get; set; }
		PageOrder PageOrder { get; set; }
		bool PrintGridlines { get; set; }
		bool PrintHeadings { get; set; }
		bool BlackAndWhite { get; set; }
		CommentsPrintMode CommentsPrintMode { get; set; }
		ErrorsPrintMode ErrorsPrintMode { get; set; }
		bool AutoPageBreaks { get; set; }
		int NumberOfCopies { get; set; }
	}
	#endregion
	#region WorksheetPageNumbering
	public interface WorksheetPageNumbering {
		int Start { get; set; }
		WorksheetPageNumberingStartType StartType { get; set; }
	}
	#endregion
	#region WorksheetHeaderFooterOptions
	public interface WorksheetHeaderFooterOptions {
		bool AlignWithMargins { get; set; }
		bool DifferentFirst { get; set; }
		bool DifferentOddEven { get; set; }
		bool ScaleWithDoc { get; set; }
		WorksheetHeaderFooter OddHeader { get; }
		WorksheetHeaderFooter OddFooter { get; }
		WorksheetHeaderFooter EvenHeader { get; }
		WorksheetHeaderFooter EvenFooter { get; }
		WorksheetHeaderFooter FirstHeader { get; }
		WorksheetHeaderFooter FirstFooter { get; }
	}
	#endregion
	#region WorksheetHeaderFooter
	public interface WorksheetHeaderFooter {
		bool IsEmpty { get; }
		string Left { get; set; }
		string Center { get; set; }
		string Right { get; set; }
		void FromLCR(string left, string center, string right);
		void FromString(string value);
	}
	#endregion
	#region HeaderFooterCode
	public static class HeaderFooterCode {
		public const string LeftSection = "&L";
		public const string CenterSection = "&C";
		public const string RightSection = "&R";
		public const string PageNumber = "&P";
		public const string PageTotal = "&N";
		public const string Strikethrough = "&S";
		public const string Superscript = "&X";
		public const string Subscript = "&Y";
		public const string Date = "&D";
		public const string Time = "&T";
		public const string Underline = "&U";
		public const string DoubleUnderline = "&E";
		public const string WorkbookFilePath = "&Z";
		public const string WorkbookFileName = "&F";
		public const string WorksheetName = "&A";
		public const string Bold = "&B";
		public const string Italic = "&I";
		public const string Ampersand = "&&";
		const string currentFont = "-";
		static Dictionary<SpreadsheetFontStyle, string> fontStyleTable = CreateFontStyleTable();
		static Dictionary<SpreadsheetFontStyle, string> CreateFontStyleTable() {
			Dictionary<SpreadsheetFontStyle, string> result = new Dictionary<SpreadsheetFontStyle, string>();
			result.Add(SpreadsheetFontStyle.Bold, "bold");
			result.Add(SpreadsheetFontStyle.BoldItalic, "bold italic");
			result.Add(SpreadsheetFontStyle.Italic, "italic");
			result.Add(SpreadsheetFontStyle.Regular, "regular");
			return result;
		}
		public static string FontSize(int size) {
			if (size < 1 || size > 999)
				throw new ArgumentOutOfRangeException("Font size out of range 1...999.");
			return string.Format("&{0}", size.ToString());
		}
		public static string Font(string fontName, SpreadsheetFontStyle fontStyle) {
			if (string.IsNullOrEmpty(fontName))
				fontName = currentFont;
			return string.Format("&\"{0},{1}\"", fontName, fontStyleTable[fontStyle]);
		}
		public static string FontColor(Color color) {
			return string.Format("&K{0}{1}{2}", color.R.ToString("X2"), color.G.ToString("X2"), color.B.ToString("X2"));
		}
		public static string FontColor(int themeColorId, int tint) {
			if (themeColorId < 0 || themeColorId > 11)
				throw new ArgumentOutOfRangeException("Theme color id out of range 0...11.");
			if (tint < -100 || tint > 100)
				throw new ArgumentOutOfRangeException("Tint/shade out of range -100...100.");
			return string.Format("&K{0}{1}{2}", themeColorId.ToString("D2"), tint < 0 ? "-" : "+", Math.Abs(tint).ToString("D3"));
		}
	}
	#endregion
	public enum SearchBy {
		Rows,
		Columns,
	}
	public enum SearchIn {
		Formulas,
		ValuesAndFormulas,
		Values,
	}
	public class SearchOptions {
		public SearchBy SearchBy { get; set; }
		public SearchIn SearchIn { get; set; }
		public bool MatchCase { get; set; }
		public bool MatchEntireCellContents { get; set; }
	}
}
#region NativeWorksheet Implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.API.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelMargins = DevExpress.XtraSpreadsheet.Model.Margins;
	using ModelPrintSetup = DevExpress.XtraSpreadsheet.Model.PrintSetup;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using ModelHeaderFooterOptions = DevExpress.XtraSpreadsheet.Model.HeaderFooterOptions;
	using ModelHeaderFooterBuilder = DevExpress.XtraSpreadsheet.Model.HeaderFooterBuilder;
	using ModelSearchOptions = DevExpress.XtraSpreadsheet.Model.ModelSearchOptions;
	using ModelSearchLookIn = DevExpress.XtraSpreadsheet.Model.ModelSearchLookIn;
	using ModelSearchDirection = DevExpress.XtraSpreadsheet.Model.ModelSearchDirection;
	using SpreadsheetSearchEngine = DevExpress.XtraSpreadsheet.Model.SpreadsheetSearchEngine;
	using System.Reflection;
	using System.Globalization;
	using DevExpress.Compatibility.System.ComponentModel;
	using DevExpress.Compatibility.System;
	using Model.CopyOperation;
	using Compatibility.System.Data;
#if !DXPORTABLE
	using DevExpress.DataAccess.ObjectBinding;
#endif
	#region NativeWorksheet
	partial class NativeWorksheet : Worksheet, IRangeProvider, WorksheetOutlineOptions {
		#region Fields
		readonly NativeWorkbook workbook;
		readonly ModelWorksheet modelWorksheet;
		readonly IComparers comparers;
		NativeCellCollection cells;
		NativeRowCollection rows;
		NativeColumnCollection columns;
		NativeHyperlinkCollection hyperlinks;
		NativeShapeCollection shapes;
		NativePictureCollection pictures;
		NativeChartCollection charts;
		NativeConditionalFormattingCollection conditionalFormattings;
		NativeCommentCollection comments;
		NativeTableCollection tables;
		NativeMergedCellsCollection mergedCellsCollection;
		NativeArrayFormulaCollection arrayFormulaCollection;
		NativeDefinedNameCollection definedNames;
		NativeWorksheetView activeView;
		NativeWorksheetPrintOptions printOptions;
		NativeWorksheetHeaderFooterOptions headerFooterOptions;
		NativeProtectedRangeCollection protectedRanges;
		bool isValid;
		NativePageBreaksCollection verticalPageBreaks;
		NativePageBreaksCollection horizontalPageBreaks;
		NativeSheetAutoFilter autoFilter;
		NativeDataValidationCollection dataValidations;
		NativeSparklineGroupCollection sparklineGroups;
		NativeIgnoredErrorCollection ignoredErrors;
		NativePivotTableCollection pivotTables;
		#endregion
		public NativeWorksheet(NativeWorkbook workbook, ModelWorksheet innerWorksheet) {
			Guard.ArgumentNotNull(workbook, "workbook");
			Guard.ArgumentNotNull(innerWorksheet, "innerWorksheet");
			this.workbook = workbook;
			this.modelWorksheet = innerWorksheet;
			this.isValid = true;
			this.comparers = new CellValueComparers(workbook.ModelWorkbook);
			CreateApiObjects();
			Initialize();
			SubscribeInternalAPIEvents();
		}
		#region Properties
		DefinedNameCollection Worksheet.DefinedNames { get { return this.DefinedNames; } }
		public HyperlinkCollection Hyperlinks { get { return hyperlinks; } }
		public ShapeCollection Shapes {
			get {
				if (shapes == null && IsValid)
					CreateShapes();
				return shapes;
			}
		}
		public PictureCollection Pictures {
			get {
				if (pictures == null && IsValid)
					CreatePictures();
				return pictures;
			}
		}
		public ChartCollection Charts {
			get {
				if (charts == null && IsValid)
					CreateCharts();
				return charts;
			}
		}
		public TableCollection Tables { get { return tables; } }
		public IRangeProvider Range { get { return this; } }
		public ConditionalFormattingCollection ConditionalFormattings {
			get {
				if (conditionalFormattings == null)
					conditionalFormattings = new NativeConditionalFormattingCollection(this);
				return conditionalFormattings;
			}
		}
		public CommentCollection Comments { get { return comments; } }
		public PageBreaksCollection VerticalPageBreaks {
			get {
				if (verticalPageBreaks == null)
					verticalPageBreaks = new NativePageBreaksCollection(ModelWorksheet.ColumnBreaks, IndicesChecker.MaxColumnCount - 1);
				return verticalPageBreaks;
			}
		}
		public PageBreaksCollection HorizontalPageBreaks {
			get {
				if (horizontalPageBreaks == null)
					horizontalPageBreaks = new NativePageBreaksCollection(ModelWorksheet.RowBreaks, IndicesChecker.MaxRowCount - 1);
				return horizontalPageBreaks;
			}
		}
		public SheetAutoFilter AutoFilter {
			get {
				if (autoFilter == null)
					autoFilter = new NativeSheetAutoFilter(ModelWorksheet.AutoFilter, this);
				return autoFilter;
			}
		}
		public ModelWorksheet ModelWorksheet { [DebuggerStepThrough]  get { return modelWorksheet; } }
		protected internal IDocumentModel DocumentModel { [DebuggerStepThrough] get { return ModelWorkbook; } }
		protected internal ModelWorkbook ModelWorkbook { [DebuggerStepThrough] get { return this.modelWorksheet.Workbook; } }
		protected internal bool IsValid { [DebuggerStepThrough] get { return isValid; } set { isValid = value; } }
		protected internal InternalAPI InternalAPI { [DebuggerStepThrough] get { return ModelWorkbook.InternalAPI; } }
		#region DefinedNames
		protected internal DefinedNameCollection DefinedNames { get { return NativeDefinedNames; } }
		protected internal NativeDefinedNameCollection NativeDefinedNames {
			get {
				if (definedNames == null)
					definedNames = new NativeDefinedNameCollection(modelWorksheet.DefinedNames, ModelWorkbook.DataContext, ModelWorksheet.SheetId, NativeWorkbook.NativeWorksheets);
				return definedNames;
			}
		}
		#endregion
		protected internal NativeTableCollection NativeTables { get { return tables; } }
		protected internal NativeCommentCollection NativeComments { get { return comments; } }
		#region Index
		public int Index {
			get {
				CheckValid();
				return ModelWorkbook.Sheets.IndexOf(ModelWorksheet);
			}
		}
		#endregion
		public NativeWorkbook NativeWorkbook { get { return workbook; } }
		public IWorkbook Workbook { get { return workbook; } }
		public CellCollection Cells { get { return this.cells; } }
		public RowCollection Rows { get { return this.rows; } }
		#region Columns
		public ColumnCollection Columns {
			get {
				if (columns == null && IsValid)
					columns = CreateColumnCollection();
				return this.columns;
			}
		}
		#endregion
		protected internal MergedCellsCollection MergedCells {
			get {
				if (this.mergedCellsCollection == null)
					this.mergedCellsCollection = new NativeMergedCellsCollection(this);
				return this.mergedCellsCollection;
			}
		}
		public WorksheetView ActiveView { get { return activeView; } }
		public WorksheetPrintOptions PrintOptions { get { return printOptions; } }
		public WorksheetHeaderFooterOptions HeaderFooterOptions { get { return headerFooterOptions; } }
		public ArrayFormulaCollection ArrayFormulas { get { return arrayFormulaCollection; } }
		#region Name
		public string Name {
			get {
				CheckValid();
				return ModelWorksheet.Name;
			}
			set {
				CheckValid();
				ModelWorksheet.Name = value;
			}
		}
		#endregion
		#region DefaultHeight
		public double DefaultRowHeight {
			get {
				DevExpress.XtraSpreadsheet.Model.SheetFormatProperties properties = ModelWorksheet.Properties.FormatProperties;
				if (properties.IsCustomHeight && properties.DefaultRowHeight > 0) {
					float result = NativeWorkbook.ModelUnitsToUnitsF(properties.DefaultRowHeight);
					return result;
				}
				float twoPixelPadding = ModelWorkbook.LayoutUnitConverter.PixelsToLayoutUnitsF(2, ModelWorkbook.DpiX);
				float resultHeight = twoPixelPadding + ModelWorkbook.CalculateDefaultRowHeightInLayoutUnits();
				resultHeight = ModelWorkbook.ToDocumentLayoutUnitConverter.ToModelUnits(resultHeight);
				return Math.Round(NativeWorkbook.ModelUnitsToUnitsF(resultHeight));
			}
			set {
				DevExpress.XtraSpreadsheet.Model.SheetFormatProperties properties = ModelWorksheet.Properties.FormatProperties;
				float result = NativeWorkbook.UnitsToModelUnitsF((float)value);
				properties.DefaultRowHeight = result;
				properties.IsCustomHeight = true;
			}
		}
		#endregion
		#region DefaultWidthInCharacters
		public float DefaultColumnWidthInCharacters {
			get {
				return ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>().CalculateDefaultColumnWidthInChars(ModelWorksheet, ModelWorksheet.Workbook.MaxDigitWidthInPixels);
			}
			set {
				ModelWorksheet.Properties.FormatProperties.DefaultColumnWidth = value;
			}
		}
		#endregion
		public int DefaultColumnWidthInPixels {
			get {
				float layoutWidth = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>().CalculateDefaultColumnWidth(ModelWorksheet, ModelWorksheet.Workbook.MaxDigitWidth, ModelWorksheet.Workbook.MaxDigitWidthInPixels);
				float inModels = ModelWorkbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutWidth);
				int inPixels = ModelWorkbook.UnitConverter.ModelUnitsToPixels((int)inModels, Model.DocumentModel.Dpi);
				return inPixels;
			}
			set {
				int widthInPixels = value;
				Model.IColumnWidthCalculationService service = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>();
				float widhInCharacters = service.ConvertPixelsToCharacters(ModelWorksheet, widthInPixels);
				ModelWorksheet.Properties.FormatProperties.DefaultColumnWidth = service.RemoveGaps(ModelWorksheet, widhInCharacters);
			}
		}
		#region DefaultWidth
		public float DefaultColumnWidth {
			get {
				float layoutWidth = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>().CalculateDefaultColumnWidth(ModelWorksheet, ModelWorksheet.Workbook.MaxDigitWidth, ModelWorksheet.Workbook.MaxDigitWidthInPixels);
				float modelWidth = ModelWorkbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutWidth);
				return NativeWorkbook.ModelUnitsToUnitsF(modelWidth);
			}
			set {
				float modelWidth = NativeWorkbook.UnitsToModelUnits(value);
				int widthInPixels = ModelWorkbook.UnitConverter.ModelUnitsToPixels((int)modelWidth, Model.DocumentModel.Dpi);
				float widthInLayouts = ModelWorkbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelWidth);
				float widhInCharacters = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>().ConvertLayoutsToCharacters(ModelWorksheet, widthInLayouts, widthInPixels);
				ModelWorksheet.Properties.FormatProperties.DefaultColumnWidth = widhInCharacters;
			}
		}
		#endregion
		#region Visible
		public bool Visible {
			get { return modelWorksheet.VisibleState == Model.SheetVisibleState.Visible; }
			set {
				Model.SheetVisibleState state = value ? Model.SheetVisibleState.Visible : Model.SheetVisibleState.Hidden;
				modelWorksheet.SetVisibleStateAndValidateActiveSheet(state);
			}
		}
		#endregion
		#region VisibilityType
		public WorksheetVisibilityType VisibilityType {
			get { return (WorksheetVisibilityType)modelWorksheet.VisibleState; }
			set { modelWorksheet.SetVisibleStateAndValidateActiveSheet((Model.SheetVisibleState)value); }
		}
		#endregion
		#region SelectedCell
		public Range SelectedCell {
			get {
				Model.CellPosition position = ModelWorksheet.Selection.ActiveCell;
				return new NativeRange(new Model.CellRange(ModelWorksheet, position, position), this);
			}
			set {
				if (!Object.ReferenceEquals(value.Worksheet, this))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
				NativeWorksheet argumentWorksheet = value.Worksheet as NativeWorksheet;
				Model.CellRange argumentModelRange = argumentWorksheet.GetModelSingleRange(value);
				Model.SheetViewSelection selection = ModelWorksheet.Selection;
				Model.CellPosition position = argumentModelRange.TopLeft;
				int count = selection.SelectedRanges.Count;
				for (int i = 0; i < count; i++) {
					if (selection.SelectedRanges[i].ContainsCell(position.Column, position.Row)) {
						selection.SetActiveRangeIndex(i);
						selection.SetActiveCellCore(position);
						return;
					}
				}
				selection.SetSelection(position);
			}
		}
		#endregion
		#region Selection
		public Range Selection {
			get { return new NativeRange(ModelWorksheet.Selection.AsRange(), this); }
			set {
				if (!Object.ReferenceEquals(value.Worksheet, this))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
				NativeWorksheet argumentWorksheet = value.Worksheet as NativeWorksheet;
				Model.CellRangeBase argumentModelRange = argumentWorksheet.GetModelRange(value);
				Model.CellRangeBase modelRangeInThisWorksheet = argumentModelRange.Clone(ModelWorksheet);
				Model.SheetViewSelection selection = ModelWorksheet.Selection;
				if (modelRangeInThisWorksheet.EqualsPosition(selection.AsRange()))
					return;
				selection.SetSelection(modelRangeInThisWorksheet);
			}
		}
		#endregion
		public Shape SelectedShape {
			get {
				Model.SheetViewSelection selection = ModelWorksheet.Selection;
				if (!selection.IsDrawingSelected)
					return null;
				return LookupShape(selection.Sheet.DrawingObjects[selection.SelectedDrawingIndexes[0]]);
			}
			set {
				NativePicture obj = value as NativePicture;
				if (obj != null) {
					if (!Object.ReferenceEquals(obj.ModelPicture.Worksheet, this.ModelWorksheet))
						throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseShapeFromAnotherWorksheet));
					SetSelectedDrawingIndex(obj.ModelPicture.IndexInCollection);
				}
				else {
					NativeChart chart = value as NativeChart;
					if (chart != null) {
						if (!Object.ReferenceEquals(chart.ModelChart.Worksheet, this.ModelWorksheet))
							throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseShapeFromAnotherWorksheet));
						SetSelectedDrawingIndex(chart.ModelChart.IndexInCollection);
					}
					else
						SetSelectedDrawingIndex(-1);
				}
			}
		}
		void SetSelectedDrawingIndex(int index) {
			Model.SheetViewSelection selection = ModelWorksheet.Selection;
			ModelWorksheet.Workbook.BeginUpdate();
			try {
				if (index < 0) {
					selection.ClearDrawingSelection();
					selection.RaiseSelectionChanged();
				}
				else
					selection.SetSelectedDrawingIndex(index);
			}
			finally {
				ModelWorksheet.Workbook.EndUpdate();
			}
		}
		public Picture SelectedPicture { get { return SelectedShape as Picture; } set { SelectedShape = value; } }
		public Chart SelectedChart { get { return SelectedShape as Chart; } set { SelectedShape = value; } }
		public IList<Shape> GetSelectedShapes() {
			List<Shape> result = new List<Shape>();
			Model.SheetViewSelection selection = ModelWorksheet.Selection;
			if (!selection.IsDrawingSelected)
				return result;
			int count = selection.SelectedDrawingIndexes.Count;
			for (int i = 0; i < count; i++) {
				Shape shape = LookupShape(selection.Sheet.DrawingObjects[selection.SelectedDrawingIndexes[i]]);
				if (shape != null)
					result.Add(shape);
			}
			return result;
		}
		public bool SetSelectedShapes(IList<Shape> shapes) {
			List<int> indices = new List<int>();
			int count = shapes.Count;
			for (int i = 0; i < count; i++) {
				NativePicture obj = shapes[i] as NativePicture;
				if (obj != null) {
					if (!Object.ReferenceEquals(obj.ModelPicture.Worksheet, this.ModelWorksheet))
						return false;
					indices.Add(obj.ModelPicture.IndexInCollection);
				}
				else {
					NativeChart chart = shapes[i] as NativeChart;
					if (chart != null) {
						if (!Object.ReferenceEquals(chart.ModelChart.Worksheet, this.ModelWorksheet))
							return false;
						indices.Add(chart.ModelChart.IndexInCollection);
					}
				}
			}
			ModelWorksheet.Workbook.BeginUpdate();
			try {
				Model.SheetViewSelection selection = ModelWorksheet.Selection;
				selection.ClearDrawingSelection();
				foreach (int index in indices)
					selection.AddSelectedDrawingIndex(index);
				selection.RaiseSelectionChanged();
				return true;
			}
			finally {
				ModelWorksheet.Workbook.EndUpdate();
			}
		}
		#endregion
		#region CheckValid
		protected internal void CheckValid() {
			if (!IsValid)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseDeletedWorksheet);
		}
		#endregion
		#region CheckIndexValid
		void CheckIndexValid(int firstRowIndex, int firstColumnIndex) {
			if (firstColumnIndex < 0 || firstRowIndex < 0)
				throw new IndexOutOfRangeException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorNegativeIndexNotAllowed));
		}
		#endregion
		#region CreateApiObjects
		protected internal virtual void CreateApiObjects() {
			Model.CellIntervalRange allColumns = Model.CellIntervalRange.CreateColumnInterval(modelWorksheet, 0, Model.PositionType.Absolute, modelWorksheet.MaxColumnCount - 1, Model.PositionType.Absolute);
			this.cells = new NativeCellCollection(allColumns, this);
			this.rows = new NativeRowCollection(this);
			this.columns = null; 
			this.hyperlinks = new NativeHyperlinkCollection(this);
			this.shapes = null; 
			this.pictures = null; 
			this.charts = null; 
			this.conditionalFormattings = null; 
			this.comments = new NativeCommentCollection(this);
			this.mergedCellsCollection = null; 
			this.arrayFormulaCollection = new NativeArrayFormulaCollection(this);
			this.activeView = new NativeWorksheetView(NativeWorkbook, ModelWorksheet);
			this.printOptions = new NativeWorksheetPrintOptions(ModelWorksheet.PrintSetup);
			this.headerFooterOptions = new NativeWorksheetHeaderFooterOptions(ModelWorksheet.HeaderFooter);
			this.definedNames = null; 
			this.tables = new NativeTableCollection(this);
			this.autoFilter = null;
			this.verticalPageBreaks = null;
			this.horizontalPageBreaks = null;
			this.dataValidations = null;
			this.sparklineGroups = null;
			this.ignoredErrors = null;
			this.pivotTables = null;
		}
		#endregion
		#region Initialize
		protected internal virtual void Initialize() {
			this.arrayFormulaCollection.Initialize();
			this.tables.Initialize();
			this.hyperlinks.Initialize();
			this.comments.Initialize();
		}
		#endregion
		#region Invalidate
		protected internal virtual void Invalidate() {
			UnsubscribeInternalAPIEvents();
			if (definedNames != null) {
				definedNames.IsValid = false;
				definedNames = null;
			}
			if (rows != null) {
				rows.Invalidate();
				rows = null;
			}
			if (columns != null) {
				columns.Invalidate();
				columns = null;
			}
			if (hyperlinks != null) {
				hyperlinks.Invalidate();
				hyperlinks = null;
			}
			if (shapes != null) {
				shapes.ClearCore();
				shapes = null;
			}
			if (pictures != null) {
				pictures.ClearCore();
				pictures = null;
			}
			if (charts != null) {
				charts.IsValid = false;
				charts.ClearCore();
				charts = null;
			}
			if (conditionalFormattings != null) {
				conditionalFormattings.Invalidate();
				conditionalFormattings = null;
			}
			if (comments != null) {
				comments.Invalidate();
				comments = null;
			}
			if (mergedCellsCollection != null) {
				mergedCellsCollection.Clear();
				mergedCellsCollection = null;
			}
			if (arrayFormulaCollection != null) {
				arrayFormulaCollection.Invalidate();
				arrayFormulaCollection = null;
			}
			if (activeView != null) {
				activeView.Invalidate();
				activeView = null;
			}
			if (printOptions != null) {
				printOptions.Invalidate();
				printOptions = null;
			}
			if (headerFooterOptions != null) {
				headerFooterOptions.Invalidate();
				headerFooterOptions = null;
			}
			if (tables != null) {
				tables.ClearCore();
				tables = null;
			}
			if (verticalPageBreaks != null) {
				verticalPageBreaks.IsValid = false;
				verticalPageBreaks = null;
			}
			if (horizontalPageBreaks != null) {
				horizontalPageBreaks.IsValid = false;
				horizontalPageBreaks = null;
			}
			if (autoFilter != null) {
				autoFilter.IsValid = false;
				autoFilter = null;
			}
			if (dataValidations != null) {
				dataValidations.IsValid = false;
				dataValidations = null;
			}
			if (sparklineGroups != null) {
				sparklineGroups.IsValid = false;
				sparklineGroups = null;
			}
			if (ignoredErrors != null) {
				ignoredErrors.IsValid = false;
				ignoredErrors = null;
			}
			if (pivotTables != null) {
				pivotTables.IsValid = false;
				pivotTables = null;
			}
		}
		#endregion
		#region SubscribeInternalAPIEvents
		protected internal virtual void SubscribeInternalAPIEvents() {
			InternalAPI internalApi = modelWorksheet.Workbook.InternalAPI;
			internalApi.BeforeRowRemoved += OnBeforeRowRemoved;
			internalApi.BeforeRowsCleared += OnBeforeRowsCleared;
			internalApi.ColumnRemoved += OnColumnRemoved;
			internalApi.ShiftCellsLeft += OnShiftCellsLeft;
			internalApi.ShiftCellsUp += OnShiftCellsUp;
			internalApi.HyperlinkAdd += OnHyperlinkAdd;
			internalApi.HyperlinkRemoveAt += OnHyperlinkRemoveAt;
			internalApi.HyperlinkCollectionClear += OnHyperlinkCollectionClear;
			internalApi.AfterMergedCellsInserted += OnMergedCellsInserted;
			internalApi.BeforeMergedCellsRemoved += OnMergedCellsRemoved;
			internalApi.BeforeMergedCellsCleared += OnMergedCellsCleared;
			internalApi.ArrayFormulaAdd += OnArrayFormulaAdd;
			internalApi.ArrayFormulaRemoveAt += OnArrayFormulaRemove;
			internalApi.ArrayFormulaCollectionClear += OnArrayFormulasCleared;
			internalApi.TableAdd += OnTableAdd;
			internalApi.TableRemoveAt += OnTableRemoveAt;
			internalApi.TableCollectionClear += OnTableCollectionClear;
			modelWorksheet.DrawingObjects.DrawingAdded += OnBeforePictureAdded;
			modelWorksheet.DrawingObjects.DrawingInserted += OnBeforePictureInserted;
			modelWorksheet.DrawingObjects.DrawingRemoved += OnBeforePictureRemoved;
			modelWorksheet.DrawingObjects.CollectionCleared += OnBeforePicturesCollectionCleared;
			ModelWorkbook.InnerCellRemoving += OnCellRemoving;
		}
		#endregion
		#region UnsubscribeInternalAPIEvents
		protected internal virtual void UnsubscribeInternalAPIEvents() {
			InternalAPI internalApi = modelWorksheet.Workbook.InternalAPI;
			internalApi.BeforeRowRemoved -= OnBeforeRowRemoved;
			internalApi.BeforeRowsCleared -= OnBeforeRowsCleared;
			internalApi.ColumnRemoved -= OnColumnRemoved;
			internalApi.ShiftCellsLeft -= OnShiftCellsLeft;
			internalApi.ShiftCellsUp -= OnShiftCellsUp;
			internalApi.HyperlinkAdd -= OnHyperlinkAdd;
			internalApi.HyperlinkRemoveAt -= OnHyperlinkRemoveAt;
			internalApi.HyperlinkCollectionClear -= OnHyperlinkCollectionClear;
			internalApi.AfterMergedCellsInserted -= OnMergedCellsInserted;
			internalApi.BeforeMergedCellsRemoved -= OnMergedCellsRemoved;
			internalApi.BeforeMergedCellsCleared -= OnMergedCellsCleared;
			internalApi.ArrayFormulaAdd -= OnArrayFormulaAdd;
			internalApi.ArrayFormulaRemoveAt -= OnArrayFormulaRemove;
			internalApi.ArrayFormulaCollectionClear -= OnArrayFormulasCleared;
			internalApi.TableAdd -= OnTableAdd;
			internalApi.TableRemoveAt -= OnTableRemoveAt;
			internalApi.TableCollectionClear -= OnTableCollectionClear;
			modelWorksheet.DrawingObjects.DrawingAdded -= OnBeforePictureAdded;
			modelWorksheet.DrawingObjects.DrawingInserted -= OnBeforePictureInserted;
			modelWorksheet.DrawingObjects.DrawingRemoved -= OnBeforePictureRemoved;
			modelWorksheet.DrawingObjects.CollectionCleared -= OnBeforePicturesCollectionCleared;
			ModelWorkbook.InnerCellRemoving -= OnCellRemoving;
		}
		#endregion
		#region GetCell
		public NativeCell GetCell(string position) {
			Model.CellPosition cellPosition = Model.CellReferenceParser.Parse(position);
			return GetCell(cellPosition.Row, cellPosition.Column);
		}
		#endregion
		#region GetCell
		public NativeCell GetCell(int rowIndex, int columnIndex) {
			return GetCell(new ModelCellKey(ModelWorksheet.SheetId, columnIndex, rowIndex));
		}
		protected internal NativeCell GetCell(ModelCellKey key) {
			Cell result;
			NativeRow row = rows.GetRowCore(key.RowIndex);
			result = row.GetCell(key);
			Debug.Assert(key.RowIndex == result.RowIndex);
			Debug.Assert(key.ColumnIndex == result.ColumnIndex);
			return result as NativeCell;
		}
		#endregion
		#region GetModelCell
		public Model.ICell GetModelCell(ModelCellKey key) {
			return ModelWorksheet[key.ColumnIndex, key.RowIndex];
		}
		#endregion
		#region GetReadOnlyModelCell
		public Model.ICell GetReadOnlyModelCell(ModelCellKey key) {
			Model.ICell cell = ModelWorksheet.GetRegisteredCell(key.ColumnIndex, key.RowIndex);
			if (cell == null)
				cell = new Model.FakeCell(new ModelCellPosition(key.ColumnIndex, key.RowIndex), ModelWorksheet);
			return cell;
		}
		#endregion
		#region BeginUpdate
		public void BeginUpdate() {
			CheckValid();
			DocumentModel.History.BeginTransaction();
		}
		#endregion
		#region EndUpdate
		public void EndUpdate() {
			CheckValid();
			DocumentModel.History.EndTransaction();
		}
		#endregion
		#region CreateRange
		protected internal virtual NativeRange CreateRange(Model.CellRange range) {
			return new NativeRange(range, this);
		}
		#endregion
		#region CreateRange
		public Range CreateRange(string reference, ReferenceStyle style) {
			NativeRangeReferenceParseHelper helper = new NativeRangeReferenceParseHelper();
			Model.WorkbookDataContext context = modelWorksheet.DataContext;
			context.PushCurrentWorksheet(modelWorksheet);
			try {
				return helper.Process(context, reference, this, style);
			}
			finally {
				context.PopCurrentWorksheet();
			}
		}
		#endregion
		#region CreateRange
		public Range CreateRange(int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex) {
			Model.CellPosition.ValidateColumnIndex(startColumnIndex);
			Model.CellPosition.ValidateColumnIndex(endColumnIndex);
			Model.CellPosition.ValidateRowIndex(startRowIndex);
			Model.CellPosition.ValidateRowIndex(endRowIndex);
			Model.CellPosition topLeft = new ModelCellPosition(startColumnIndex, startRowIndex);
			Model.CellPosition bottomRight = new ModelCellPosition(endColumnIndex, endRowIndex);
			Model.CellRange modelRange = new Model.CellRange(modelWorksheet, topLeft, bottomRight).TryConvertToCellInterval();
			return CreateRange(modelRange);
		}
		#endregion
		#region CreateColumnCollection
		NativeColumnCollection CreateColumnCollection() {
			return new NativeColumnCollection(this);
		}
		#endregion
		void BeginImport() {
			DocumentModel.BeginUpdate();
			ModelWorksheet.DataContext.PushImportExportMode(true);
		}
		void EndImport() {
			ModelWorksheet.DataContext.PopImportExportMode();
			DocumentModel.EndUpdate();
		}
		Model.CellRange CreateHorizontalRange(int rowIndex, int columnIndex, int columnCount) {
			ModelCellPosition topLeft = new ModelCellPosition(columnIndex, rowIndex);
			ModelCellPosition bottomRight = new ModelCellPosition(columnIndex + columnCount - 1, rowIndex);
			return new Model.CellRange(ModelWorksheet, topLeft, bottomRight);
		}
		void ApplyBatchCellChecks(int rowIndex, int columnIndex, int columnCount) {
			Model.ISheetPosition range = CreateHorizontalRange(rowIndex, columnIndex, columnCount);
			ModelWorksheet.CheckingChangingPartOfArray(range);
			ModelWorksheet.RemoveArrayFormulasFromCollectionInsideRange(range);
		}
		void ApplyBatchCellChecks(Model.ISheetPosition range) {
			ModelWorksheet.CheckingChangingPartOfArray(range);
			ModelWorksheet.RemoveArrayFormulasFromCollectionInsideRange(range);
		}
		#region Import array
		public void Import(object[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportValue);
		}
		public void Import(object[] array, int firstRowIndex, int firstColumnIndex, bool isVertical, IDataValueConverter converter) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, new DataImportOptions() { Converter = converter }, ImportValue);
		}
		public void Import(object[] array, int firstRowIndex, int firstColumnIndex, bool isVertical, DataImportOptions options) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, options, ImportValue);
		}
		public void Import(int[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportIntegerValue);
		}
		public void Import(short[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportShortValue);
		}
		public void Import(byte[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportByteValue);
		}
		public void Import(bool[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportBooleanValue);
		}
		public void Import(long[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportLongValue);
		}
		public void Import(float[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportFloatValue);
		}
		public void Import(double[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportDoubleValue);
		}
		public void Import(decimal[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportDecimalValue);
		}
		public void Import(DateTime[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportDateTimeValue);
		}
		public void Import(string[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, null, ImportStringValue);
		}
		public void Import(string[] array, int firstRowIndex, int firstColumnIndex, bool isVertical, DataImportOptions options) {
			ImportArrayCore(array, firstRowIndex, firstColumnIndex, isVertical, options, ImportStringValue);
		}
		void ImportArrayCore<T>(T[] array, int firstRowIndex, int firstColumnIndex, bool isVertical, DataImportOptions options, ImportValueAction<T> importValueAction) {
			CheckValid();
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			if (array.Length < 0)
				return;
			if (options == null)
				options = new DataImportOptions();
			BeginImport();
			try {
				if (!isVertical)
					ImportHorizontalVector(array, firstRowIndex, firstColumnIndex, options, importValueAction);
				else {
					ImportVerticalVector(array, firstRowIndex, firstColumnIndex, options, importValueAction);
				}
			}
			finally {
				EndImport();
			}
		}
		void ImportHorizontalVector<T>(T[] array, int firstRowIndex, int firstColumnIndex, DataImportOptions options, ImportValueAction<T> importValueAction) {
			int count = array.Length;
			Model.Row row = ModelWorksheet.Rows[firstRowIndex];
			Model.CellRange range = new Model.CellRange(ModelWorksheet, new ModelCellPosition(firstColumnIndex, firstRowIndex), new ModelCellPosition(firstColumnIndex + count - 1, firstRowIndex));
			ApplyBatchCellChecks(range);
			for (int i = 0; i < count; i++) {
				importValueAction(array[i], row, firstColumnIndex, i, options);
			}
		}
		void ImportVerticalVector<T>(T[] array, int firstRowIndex, int firstColumnIndex, DataImportOptions options, ImportValueAction<T> importValueAction) {
			int count = array.Length;
			Model.CellRange range = new Model.CellRange(ModelWorksheet, new ModelCellPosition(firstColumnIndex, firstRowIndex), new ModelCellPosition(firstColumnIndex, firstRowIndex + count - 1));
			ApplyBatchCellChecks(range);
			for (int i = 0; i < count; i++) {
				Model.Row row = ModelWorksheet.Rows[firstRowIndex + i];
				importValueAction(array[i], row, firstColumnIndex, 0, options);
			}
		}
		void ImportVector<T>(T source, int firstRowIndex, int firstColumnIndex, bool isVertical, int index, DataImportOptions options, ImportValueAction<T> importValueAction) {
			int columnOffset = isVertical ? 0 : index;
			int rowOffset = isVertical ? index : 0;
			Model.Row row = ModelWorksheet.Rows[firstRowIndex + rowOffset];
			ApplyBatchCellChecks(firstColumnIndex + rowOffset, firstColumnIndex + columnOffset, 1);
			importValueAction(source, row, firstColumnIndex, columnOffset, options);
		}
		#endregion
		#region Import TwoDimensionalArray
		delegate void ImportValueAction<T>(T value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options);
		public void Import(object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportValue);
		}
		public void Import(object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, new DataImportOptions() { Converter = converter }, ImportValue);
		}
		public void Import(object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, options, ImportValue);
		}
		public void Import(int[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportIntegerValue);
		}
		public void Import(short[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportShortValue);
		}
		public void Import(byte[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportByteValue);
		}
		public void Import(bool[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportBooleanValue);
		}
		public void Import(long[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportLongValue);
		}
		public void Import(float[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportFloatValue);
		}
		public void Import(double[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportDoubleValue);
		}
		public void Import(decimal[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportDecimalValue);
		}
		public void Import(DateTime[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportDateTimeValue);
		}
		public void Import(string[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, null, ImportStringValue);
		}
		public void Import(string[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			Import2DArrayCore(twoDimensionalArray, firstRowIndex, firstColumnIndex, options, ImportStringValue);
		}
		void Import2DArrayCore<T>(T[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, DataImportOptions options, ImportValueAction<T> importValueAction) {
			CheckValid();
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			if (options == null)
				options = new DataImportOptions();
			BeginImport();
			try {
				Model.IRowCollection rows = ModelWorksheet.Rows;
				int height = twoDimensionalArray.GetLength(0);
				int width = twoDimensionalArray.GetLength(1);
				for (int i = 0; i < height; i++) {
					Model.Row row = rows[firstRowIndex + i];
					ApplyBatchCellChecks(firstRowIndex + i, firstColumnIndex, width);
					for (int j = 0; j < width; j++)
						importValueAction(twoDimensionalArray[i, j], row, firstColumnIndex, j, options);
				}
			}
			finally {
				EndImport();
			}
		}
		#endregion
		#region Import IEnumerable
		public void Import(IEnumerable source, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			Import(source, firstRowIndex, firstColumnIndex, isVertical, new DataImportOptions());
		}
		public void Import(IEnumerable source, int firstRowIndex, int firstColumnIndex, bool isVertical, IDataValueConverter converter) {
			Import(source, firstRowIndex, firstColumnIndex, isVertical, new DataImportOptions() { Converter = converter });
		}
		public void Import(IEnumerable source, int firstRowIndex, int firstColumnIndex, bool isVertical, DataImportOptions options) {
			CheckValid();
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			if (options == null)
				options = new DataImportOptions();
			BeginImport();
			try {
				int index = 0;
				foreach (object value in source) {
					ImportVector(value, firstRowIndex, firstColumnIndex, isVertical, index, options, ImportValue);
					index++;
				}
			}
			finally {
				EndImport();
			}
		}
		#endregion
		#region Import Custom collection
		public void Import(object dataSource, int firstRowIndex, int firstColumnIndex) {
			Import(dataSource, firstRowIndex, firstColumnIndex, new DataSourceImportOptions());
		}
		public void Import(object dataSource, int firstRowIndex, int firstColumnIndex, DataSourceImportOptions options) {
			CheckValid();
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			BeginImport();
			try {
				ImportCore(dataSource, firstRowIndex, firstColumnIndex, options);
			}
			finally {
				EndImport();
			}
		}
		void ImportCore(object dataSource, int firstRowIndex, int firstColumnIndex, DataSourceImportOptions options) {
			IEnumerable enumerable = dataSource as IEnumerable;
			if (enumerable == null) {
				ResultTypedList typedList = ObjectDataSourceFillHelper.CreateTypedList(dataSource, "data");
				dataSource = typedList;
				enumerable = typedList;
			}
			PropertyDescriptorCollection properties = GetProperties(dataSource, options.PropertyNames);
			if (properties == null)
				return;
			int width = properties.Count;
			if (width <= 0)
				return;
			int index = 0;
			Model.IRowCollection rows = ModelWorksheet.Rows;
			foreach (object value in enumerable) {
				if (value == null)
					continue;
				Model.Row row = rows[firstRowIndex + index];
				ApplyBatchCellChecks(firstRowIndex + index, firstColumnIndex, width);
				for (int i = 0; i < width; i++) {
					object currentFieldValue = properties[i].GetValue(value);
					ImportValue(currentFieldValue, row, firstColumnIndex, i, options);
				}
				index++;
			}
		}
		PropertyDescriptorCollection GetSourceProperties(object list) {
			ITypedList typedList = list as ITypedList;
			if (typedList != null)
				return typedList.GetItemProperties(null);
			IList l = list as IList;
			if (l != null && l.Count > 0)
				return GetItemProperties(l[0]);
			IEnumerable enumerable = list as IEnumerable;
			if (enumerable != null) {
				IEnumerator enumerator = enumerable.GetEnumerator();
				if (enumerator.MoveNext())
					return GetItemProperties(enumerator.Current);
			}
			return null;
		}
		static PropertyDescriptorCollection GetItemProperties(object item) {
			PropertyDescriptorCollection col = TypeDescriptor.GetProperties(item);
			if (col == null || col.Count == 0)
				return null;
			return col;
		}
		PropertyDescriptorCollection GetProperties(object dataSource, string[] propertyNames) {
			PropertyDescriptorCollection properties = GetSourceProperties(dataSource);
			if (properties == null || propertyNames == null || propertyNames.Length <= 0)
				return properties;
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach (string propertyName in propertyNames) {
				PropertyDescriptor descriptor = properties.Find(propertyName, false);
				if (descriptor == null)
					throw new ArgumentException("Can not find property \'" + propertyName + "\'.");
				result.Add(descriptor);
			}
			return new PropertyDescriptorCollection(result.ToArray());
		}
		#endregion
#if !SL
		#region Import
		public void Import(DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex) {
			Import(source, addHeader, firstRowIndex, firstColumnIndex, new DataImportOptions());
		}
		public void Import(DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			Import(source, addHeader, firstRowIndex, firstColumnIndex, new DataImportOptions() { Converter = converter });
		}
		public void Import(DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			CheckValid();
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			if (options == null)
				options = new DataImportOptions();
			BeginImport();
			try {
				firstRowIndex = ImportHeader(source, addHeader, firstRowIndex, firstColumnIndex);
				Model.IRowCollection rows = ModelWorksheet.Rows;
				DataRowCollection sourceRows = source.Rows;
				DataColumnCollection sourceColumns = source.Columns;
				int sourceRowsCount = sourceRows.Count;
				int sourceColumnsCount = sourceColumns.Count;
				for (int i = 0; i < sourceRowsCount; i++) {
					Model.Row row = rows[firstRowIndex + i];
					ApplyBatchCellChecks(firstRowIndex + i, firstColumnIndex, sourceColumnsCount);
					for (int j = 0; j < sourceColumnsCount; j++)
						ImportValue(sourceRows[i][j], row, firstColumnIndex, j, options);
				}
			}
			finally {
				EndImport();
			}
		}
		int ImportHeader(DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex) {
			if (addHeader) {
				Model.ICellCollection cells = ModelWorksheet.Rows[firstRowIndex].Cells;
				DataColumnCollection sourceColumns = source.Columns;
				int count = sourceColumns.Count;
				ApplyBatchCellChecks(firstRowIndex, firstColumnIndex, count);
				for (int i = 0; i < count; i++)
					SetCellValue(GetNextCell(cells, firstColumnIndex + i), sourceColumns[i].Caption);
				return firstRowIndex + 1;
			}
			return firstRowIndex;
		}
		#endregion
#if !DXPORTABLE
		#region Import
		public void Import(IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex) {
			Import(source, addHeader, firstRowIndex, firstColumnIndex, new DataImportOptions());
		}
		public void Import(IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			Import(source, addHeader, firstRowIndex, firstColumnIndex, new DataImportOptions() { Converter = converter });
		}
		public void Import(IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			CheckValid();
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			if (options == null)
				options = new DataImportOptions();
			BeginImport();
			try {
				firstRowIndex = ImportHeader(source, addHeader, firstRowIndex, firstColumnIndex);
				Model.IRowCollection rows = ModelWorksheet.Rows;
				int i = 0;
				while (source.Read()) {
					Model.Row row = rows[firstRowIndex + i];
					IDataRecord record = (IDataRecord)source;
					int count = record.FieldCount;
					ApplyBatchCellChecks(firstRowIndex + i, firstColumnIndex, count);
					for (int j = 0; j < count; j++)
						ImportValue(record[j], row, firstColumnIndex, j, options);
					i++;
				}
			}
			finally {
				EndImport();
			}
		}
		#endregion
		#region ImportHeader
		int ImportHeader(IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex) {
			if (addHeader) {
				Model.ICellCollection cells = ModelWorksheet.Rows[firstRowIndex].Cells;
				int count = source.FieldCount;
				ApplyBatchCellChecks(firstRowIndex, firstColumnIndex, count);
				for (int i = 0; i < count; i++)
					SetCellValue(GetNextCell(cells, firstColumnIndex + i), source.GetName(i));
				return firstRowIndex + 1;
			}
			return firstRowIndex;
		}
		#endregion
#endif
#endif
		Model.ICell GetNextCell(Model.ICellCollection cells, int columnIndex) {
			return cells.GetCellAssumeSequential(columnIndex);
		}
		void SetCellValue(Model.ICell cell, Model.VariantValue value) {
			cell.SetValueNoChecks(value);
		}
		void SetCellFormula(Model.ICell cell, string formulaString, DataImportOptions options) {
			Model.Formula formula = null;
			Model.WorkbookDataContext context = cell.Sheet.Workbook.DataContext;
			bool useR1C1 = false;
			if (options.ReferenceStyle == ReferenceStyle.UseDocumentSettings)
				useR1C1 = context.Workbook.Properties.UseR1C1ReferenceStyle;
			else
				useR1C1 = options.ReferenceStyle == ReferenceStyle.R1C1;
			CultureInfo culture = options.FormulaCulture;
			if (culture == null)
				culture = cell.Context.Workbook.Culture;
			context.PushUseR1C1(useR1C1);
			context.PushCulture(culture);
			try {
				formula = new Model.Formula(cell, formulaString);
			}
			catch (ArgumentException e) {
				string cultureName = culture == System.Globalization.CultureInfo.InvariantCulture ? "Invariant" : culture.Name;
				string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula), formulaString, cultureName);
				throw new ArgumentException(message, e);
			}
			finally {
				context.PopCulture();
				context.PopUseR1C1();
			}
			cell.TransactedSetFormula(formula);
		}
		#region ImportValue
		void ImportDoubleValue(double value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), ConvertNaN(value));
		}
		void ImportDecimalValue(decimal value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), ConvertNaN(Convert.ToDouble(value)));
		}
		void ImportFloatValue(float value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), ConvertNaN(value));
		}
		void ImportIntegerValue(int value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), value);
		}
		void ImportShortValue(short value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), value);
		}
		void ImportByteValue(byte value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), value);
		}
		void ImportLongValue(long value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), value);
		}
		void ImportBooleanValue(bool value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			SetCellValue(GetNextCell(cells, columnIndex), value);
		}
		void ImportDateTimeValue(DateTime value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			Model.VariantValue modelValue = new Model.VariantValue();
			modelValue.SetDateTime(value, ModelWorksheet.DataContext);
			DevExpress.XtraSpreadsheet.Model.ICell cell = cells[columnIndex];
			SetCellValue(cell, modelValue);
			cell.FormatString = ModelWorksheet.Workbook.Cache.NumberFormatCache[14].FormatCode;
		}
		void ImportStringValue(string value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			if (value != null) {
				if (options.ImportFormulas && value.TrimStart().StartsWith("=", StringComparison.Ordinal))
					SetCellFormula(GetNextCell(cells, columnIndex), value, options);
				else
					SetCellValue(GetNextCell(cells, columnIndex), value);
			}
			else
				GetNextCell(cells, columnIndex).ClearContent();
		}
		void ImportValue(object value, Model.Row row, int firstColumnIndex, int columnOffset, DataImportOptions options) {
			int columnIndex = firstColumnIndex + columnOffset;
			Model.ICellCollection cells = row.Cells;
			if (options.Converter != null) {
				CellValue cellValue;
				if (options.Converter.TryConvert(value, columnOffset, out cellValue)) {
					if (cellValue == null || cellValue == CellValue.Empty)
						GetNextCell(cells, columnIndex).ClearContent();
					else
						SetCellValue(GetNextCell(cells, columnIndex), cellValue.ModelVariantValue);
					return;
				}
			}
			if (value == null || value is DBNull) {
				GetNextCell(cells, columnIndex).ClearContent();
				return;
			}
			string strValue = value as string;
			if (strValue != null) {
				if (options.ImportFormulas && strValue.TrimStart().StartsWith("=", StringComparison.Ordinal))
					SetCellFormula(GetNextCell(cells, columnIndex), strValue, options);
				else
					SetCellValue(GetNextCell(cells, columnIndex), strValue);
				return;
			}
			Type valueType = value.GetType();
			if (valueType == typeof(bool)) {
				SetCellValue(GetNextCell(cells, columnIndex), (bool)value);
				return;
			}
			if (valueType == typeof(DateTime)) {
				Model.VariantValue modelValue = new Model.VariantValue();
				modelValue.SetDateTime((DateTime)value, ModelWorksheet.DataContext);
				DevExpress.XtraSpreadsheet.Model.ICell cell = cells[columnIndex];
				SetCellValue(cell, modelValue);
				cell.FormatString = ModelWorksheet.Workbook.Cache.NumberFormatCache[14].FormatCode;
				return;
			}
			if (valueType == typeof(double) || valueType == typeof(decimal) || valueType == typeof(Single) ||
				valueType == typeof(Int16) || valueType == typeof(Int32) || valueType == typeof(Int64) ||
				valueType == typeof(byte)) {
				SetCellValue(GetNextCell(cells, columnIndex), ConvertNaN(Convert.ToDouble(value)));
				return;
			}
			if (valueType == typeof(DateTimeOffset) || valueType == typeof(TimeSpan) || valueType == typeof(Guid)) {
				strValue = value.ToString();
				SetCellValue(GetNextCell(cells, columnIndex), strValue);
				return;
			}
			GetNextCell(cells, columnIndex).ClearContent();
		}
		double ConvertNaN(double value) {
			if (double.IsInfinity(value) || double.IsNaN(value))
				value = ushort.MaxValue;
			else if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(value))
				value = 0;
			return value;
		}
		#endregion
		#region OnBeforeRowRemoved
		void OnBeforeRowRemoved(object sender, Model.BeforeRowRemoveEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			this.rows.RemoveApiObject(e.Index, e.DeletedRowsCount);
		}
		#endregion
		#region OnColumnRemoved
		void OnColumnRemoved(object sender, Model.ColumnRemovedEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			this.rows.ClearCachedItems(e.Index);
		}
		#endregion
		#region OnBeforeRowsCleared
		void OnBeforeRowsCleared(object sender, Model.BeforeRowsClearedEventArgs e) {
			this.rows.Invalidate();
		}
		#endregion
		#region OnShiftCellsLeft
		void OnShiftCellsLeft(object sender, Model.ShiftCellsLeftEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			this.rows.ClearCachedItems_ShiftCellsLeft(e.FirstColumnIndex, e.FirstRowIndex, e.LastRowIndex);
		}
		#endregion
		#region OnShiftCellsUp
		void OnShiftCellsUp(object sender, Model.ShiftCellsUpEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			this.rows.ClearCachedItems_ShiftCellsUp(e.FirstColumnIndex, e.LastColumnIndex, e.FirstRowIndex);
		}
		#endregion
		#region OnHyperlinkAdd
		void OnHyperlinkAdd(object sender, Model.HyperlinkAddEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			((NativeHyperlinkCollection)Hyperlinks).OnAdd(e.Hyperlink);
		}
		#endregion
		#region OnHyperlinkRemoveAt
		void OnHyperlinkRemoveAt(object sender, Model.HyperlinkRemoveAtEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			((NativeHyperlinkCollection)Hyperlinks).OnRemoveAt(e.Index);
		}
		#endregion
		#region OnHyperlinkClear
		void OnHyperlinkCollectionClear(object sender, Model.HyperlinkCollectionClearEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			((NativeHyperlinkCollection)Hyperlinks).OnClear();
		}
		#endregion
		#region OnMergedCellsInserted / Removed / Cleared
		void OnMergedCellsInserted(object sender, Model.MergedCellsCollectionChangedEventArgs e) {
			if (this.mergedCellsCollection == null || !Object.ReferenceEquals(ModelWorksheet, e.Worksheet))
				return;
			this.mergedCellsCollection.OnAdd(e.MergedCellRange);
		}
		void OnMergedCellsRemoved(object sender, Model.MergedCellsCollectionChangedEventArgs e) {
			if (this.mergedCellsCollection == null || !Object.ReferenceEquals(ModelWorksheet, e.Worksheet))
				return;
			this.mergedCellsCollection.OnRemove(e.MergedCellRange);
		}
		void OnMergedCellsCleared(object sender, Model.MergedCellsCollectionClearedEventArgs e) {
			if (this.mergedCellsCollection == null || !Object.ReferenceEquals(ModelWorksheet, e.Worksheet))
				return;
			this.mergedCellsCollection.OnClear();
		}
		#endregion
		#region OnArrayFormulaAdd
		void OnArrayFormulaAdd(object sender, Model.ArrayFormulaAddEventArgs e) {
		}
		#endregion
		#region OnArrayFormulaRemove
		void OnArrayFormulaRemove(object sender, Model.ArrayFormulaRemoveEventArgs e) {
		}
		#endregion
		#region OnArrayFormulasCleared
		void OnArrayFormulasCleared(object sender, Model.ArrayFormulaCollectionClearEventArgs e) {
		}
		#endregion
		#region OnTableAdd
		void OnTableAdd(object sender, Model.TableAddEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			((NativeTableCollection)Tables).OnAdd(e.Table);
		}
		#endregion
		#region OnTableRemoveAt
		void OnTableRemoveAt(object sender, Model.TableRemoveAtEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			((NativeTableCollection)Tables).OnRemoveAt(e.Index);
		}
		#endregion
		#region OnTableClear
		void OnTableCollectionClear(object sender, Model.TableCollectionClearEventArgs e) {
			if (!Object.ReferenceEquals(e.Worksheet, modelWorksheet))
				return;
			((NativeTableCollection)Tables).OnClear();
		}
		#endregion
		#region OnCellRemoving
		void OnCellRemoving(object sender, Model.CellRemovingEventArgs e) {
			ModelCellKey key = e.Key;
			NativeRow row = rows[key.RowIndex];
			NativeCell cell = row.CellCollection.TryGetCell(key);
			if (cell != null)
				cell.Invalidate();
		}
		#endregion
		#region Move
		public void Move(int position) {
			CheckValid();
			if (position < 0 || position > ModelWorkbook.Sheets.Count)
				throw new IndexOutOfRangeException();
			ModelWorkbook.MoveSheet(ModelWorksheet, position);
		}
		#endregion
		#region GetTopLeft
		public Model.CellPosition GetTopLeft(Range range) {
			return GetModelRange(range).TopLeft;
		}
		public Model.CellRange GetModelSingleRange(Range range) {
			Model.CellRangeBase modelRange = GetModelRange(range);
			CheckUnionRange(modelRange);
			return (Model.CellRange)modelRange;
		}
		void CheckUnionRange(Model.CellRangeBase modelRange) {
			if (modelRange.RangeType == Model.CellRangeType.UnionRange)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUnionRange));
		}
		public Model.CellRangeBase GetModelRange(Range range) {
			NativeRange nativeRange = range as NativeRange;
			NativeRowColumnBase rowColumnRange = range as NativeRowColumnBase;
			if (rowColumnRange != null)
				nativeRange = ((NativeRange)rowColumnRange.NativeCellsRange);
			if (nativeRange != null) {
				NativeWorksheet otherWorksheet = nativeRange.Worksheet as NativeWorksheet;
				if (!otherWorksheet.IsValid || !Object.ReferenceEquals(otherWorksheet, this)) {
					throw new ArgumentException();
				}
				return nativeRange.ModelRange;
			}
			NativeCell cellRange = range as NativeCell;
			if (cellRange != null) {
				NativeWorksheet otherWorksheet = cellRange.Worksheet as NativeWorksheet;
				if (!otherWorksheet.IsValid || !Object.ReferenceEquals(otherWorksheet, this)) {
					throw new ArgumentException();
				}
				return cellRange.ReadOnlyModelCell.GetRange();
			}
			return null;
		}
		#endregion
		#region Copy
		public void CopyFrom(Worksheet source) {
			NativeWorksheet sourceNative = source as NativeWorksheet;
			Model.Worksheet modelTargetWorksheet = this.ModelWorksheet;
			Model.Worksheet modelSourceWorksheet = sourceNative.ModelWorksheet;
			Model.CopyOperation.SourceTargetRangesForCopy ranges = new Model.CopyOperation.SourceTargetRangesForCopy(modelSourceWorksheet, modelTargetWorksheet);
			Model.CopyOperation.CopyWorksheetOperation operation = new Model.CopyOperation.CopyWorksheetOperation(ranges);
			operation.Execute();
		}
		#endregion
		#region GetNameFromRange
		public string GetDefinedNameFromRange(Range range) {
			Model.CellRangeBase modelRange = GetModelRange(range);
			if (modelRange == null)
				return string.Empty;
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			DefinedName definedName = NativeDefinedNames.GetDefinedNameByReference(modelRange);
			if (definedName != null)
				return definedName.Name;
			return String.Empty;
		}
		#endregion
		#region GetExistingCells
		public IEnumerable<Cell> GetExistingCells() {
			IEnumerator<Model.ICellBase> modelEnumerator = ModelWorksheet.GetExistingCells().GetEnumerator();
			return new Enumerable<Cell>(new EnumeratorConverter<Model.ICellBase, Cell>(modelEnumerator, ConvertModelCellToApiCell));
		}
		Cell ConvertModelCellToApiCell(Model.ICellBase cell) {
			return this[cell.RowIndex, cell.ColumnIndex];
		}
		#endregion
		#region InternalAPI_BeforePictureAdded
		void OnBeforePictureAdded(object sender, Model.DrawingObjectsCollectionChangedEventArgs e) {
			if (shapes == null)
				return;
			OnPictureAddedCore(e);
			OnChartAddedCore(e);
		}
		void OnPictureAddedCore(Model.DrawingObjectsCollectionChangedEventArgs e) {
			Model.Picture picture = e.Drawing as Model.Picture;
			if (picture != null) {
				NativePicture nativePicture = new NativePicture(picture, this);
				this.shapes.AddCore(nativePicture);
				if (this.pictures != null)
					this.pictures.AddCore(nativePicture);
			}
		}
		void OnChartAddedCore(Model.DrawingObjectsCollectionChangedEventArgs e) {
			Model.Chart chart = e.Drawing as Model.Chart;
			if (chart != null) {
				NativeChart nativeChart = new NativeChart(chart, this);
				this.shapes.AddCore(nativeChart);
				if (this.charts != null)
					this.charts.AddCore(nativeChart);
			}
		}
		#endregion
		#region InternalAPI_BeforePictureInserted
		void OnBeforePictureInserted(object sender, Model.DrawingInsertedEventArgs e) {
			if (shapes == null)
				return;
			OnPictureInsertedCore(e);
			OnChartInsertedCore(e);
		}
		void OnPictureInsertedCore(Model.DrawingInsertedEventArgs e) {
			Model.Picture picture = e.Drawing as Model.Picture;
			if (picture != null) {
				NativePicture nativePicture = new NativePicture(picture, this);
				this.shapes.InsertCore(e.Index, nativePicture);
				if (this.pictures != null) {
					int index = e.Index;
					for (int i = 0; i < index; i++) {
						if (!(this.shapes[i] is NativePicture))
							index--;
					}
					this.pictures.InsertCore(index, nativePicture);
				}
			}
		}
		void OnChartInsertedCore(Model.DrawingInsertedEventArgs e) {
			Model.Chart chart = e.Drawing as Model.Chart;
			if (chart != null) {
				NativeChart nativeChart = new NativeChart(chart, this);
				this.shapes.InsertCore(e.Index, nativeChart);
				if (this.charts != null) {
					int index = e.Index;
					for (int i = 0; i < index; i++) {
						if (!(this.shapes[i] is NativeChart))
							index--;
					}
					this.charts.InsertCore(index, nativeChart);
				}
			}
		}
		#endregion
		#region InternalAPI_BeforePictureRemoved
		void OnBeforePictureRemoved(object sender, Model.DrawingObjectsCollectionChangedEventArgs e) {
			if (shapes == null)
				return;
			OnPictureRemovedCore(e);
			OnChartRemovedCore(e);
		}
		void OnPictureRemovedCore(Model.DrawingObjectsCollectionChangedEventArgs e) {
			Model.Picture picture = e.Drawing as Model.Picture;
			if (picture != null) {
				NativePicture nativePicture = null;
				for (int i = 0; i < this.Shapes.Count; i++) {
					nativePicture = this.shapes[i] as NativePicture;
					if (nativePicture != null && nativePicture.ModelPicture == picture)
						break;
				}
				if (nativePicture != null && nativePicture.IsValid) {
					nativePicture.IsValid = false;
					RemoveNativeDrawing(nativePicture);
				}
			}
		}
		void OnChartRemovedCore(Model.DrawingObjectsCollectionChangedEventArgs e) {
			Model.Chart chart = e.Drawing as Model.Chart;
			if (chart != null) {
				NativeChart nativeChart = null;
				for (int i = 0; i < this.Shapes.Count; i++) {
					nativeChart = this.shapes[i] as NativeChart;
					if (nativeChart != null && nativeChart.ModelChart == chart)
						break;
				}
				if (nativeChart != null && nativeChart.IsValid) {
					RemoveNativeDrawing(nativeChart);
					nativeChart.IsValid = false;
				}
			}
		}
		internal void RemoveNativeDrawing(NativeDrawingObject nativeDrawingObject) {
			if (this.shapes != null)
				this.shapes.RemoveCore(nativeDrawingObject);
			if (this.pictures != null) {
				NativePicture nativePicture = nativeDrawingObject as NativePicture;
				if (nativePicture != null)
					this.pictures.RemoveCore(nativePicture);
			}
			if (this.charts != null) {
				NativeChart nativeChart = nativeDrawingObject as NativeChart;
				if (nativeChart != null)
					this.charts.RemoveCore(nativeChart);
			}
		}
		#endregion
		#region InternalAPI_BeforePicturesCollectionCleared
		void OnBeforePicturesCollectionCleared(object sender, EventArgs e) {
			ClearNativeDrawings();
		}
		internal void ClearNativeDrawings() {
			if (shapes != null && shapes.Count != 0)
				shapes.Clear();
			if (pictures != null && pictures.Count != 0)
				pictures.Clear();
			if (charts != null && charts.Count != 0)
				charts.Clear();
		}
		internal void ClearNativeDrawingsCore() {
			if (shapes != null)
				shapes.ClearCore();
			if (pictures != null)
				pictures.ClearCore();
			if (charts != null)
				charts.ClearCore();
		}
		#endregion
		#region CreateShapes
		void CreateShapes() {
			this.shapes = new NativeShapeCollection(this);
			modelWorksheet.DrawingObjects.ForEach(RegisterDrawingObject);
		}
		void CreatePictures() {
			this.pictures = new NativePictureCollection(this);
			foreach (Shape shape in Shapes) {
				NativePicture picture = shape as NativePicture;
				if (picture != null)
					this.pictures.AddCore(picture);
			}
		}
		void CreateCharts() {
			this.charts = new NativeChartCollection(this);
			foreach (Shape shape in Shapes) {
				NativeChart chart = shape as NativeChart;
				if (chart != null)
					this.charts.AddCore(chart);
			}
		}
		void RegisterDrawingObject(Model.IDrawingObject drawing) {
			Model.Picture picture = drawing as Model.Picture;
			if (picture != null)
				this.shapes.AddCore(new NativePicture(picture, this));
			Model.Chart chart = drawing as Model.Chart;
			if (chart != null)
				this.shapes.AddCore(new NativeChart(chart, this));
		}
		Shape LookupShape(Model.IDrawingObject drawingObject) {
			foreach (NativeDrawingObject obj in Shapes)
				if (Object.ReferenceEquals(obj.ModelDrawingObject, drawingObject.DrawingObject))
					return obj;
			return null;
		}
		#endregion
		#region CreateConditionalFormattingIconSetInsideValue
		[Obsolete("Use the ConditionalFormattings.CreateIconSetValue(ConditionalFormattingValueType valueType, string value, ConditionalFormattingValueOperator comparisonOperator) method instead.", true)]
		public ConditionalFormattingIconSetInsideValue CreateConditionalFormattingIconSetInsideValue(ConditionalFormattingValueType valueType, string value, ConditionalFormattingValueOperator comparisonOperator) {
			return null;
		}
		#endregion
		#region CreateConditionalFormattingInsideValue
		[Obsolete("Use the ConditionalFormattings.CreateValue(ConditionalFormattingValueType valueType, string value) method instead.", true)]
		public ConditionalFormattingInsideValue CreateConditionalFormattingInsideValue(ConditionalFormattingValueType valueType, string value) {
			return null;
		}
		#endregion
		#region CreateConditionalFormattingExtremumValue
		[Obsolete("Use the ConditionalFormattings.CreateValue(ConditionalFormattingValueType valueType, string value) method instead.", true)]
		public ConditionalFormattingExtremumValue CreateConditionalFormattingExtremumValue(ConditionalFormattingValueType valueType, string value) {
			return null;
		}
		#endregion
		#region CreateConditionalFormattingExtremumValue
		[Obsolete("Use the ConditionalFormattings.CreateValue(ConditionalFormattingValueType valueType) method instead.", true)]
		public ConditionalFormattingExtremumValue CreateConditionalFormattingExtremumValue() {
			return null;
		}
		#endregion
		#region AddPrintRange
		public void AddPrintRange(Range range) {
			ModelWorksheet.AddPrintRange(GetModelRange(range));
		}
		#endregion
		#region ClearPrintRange
		public void ClearPrintRange() {
			ModelWorksheet.ClearPrintRange();
		}
		#endregion
		#region SetPrintRange
		public void SetPrintRange(Range range) {
			ModelWorksheet.SetPrintRange(GetModelRange(range));
		}
		#endregion
		#region GetPrintRange
		public Range GetPrintableRange() {
			Model.CellRange range = ModelWorksheet.GetPrintRange();
			return new NativeRange(range, this);
		}
		public Range GetPrintableRange(bool usePrintAreaDefinedName) {
			Model.CellRangeBase range;
			if (usePrintAreaDefinedName) {
				range = ModelWorksheet.GetPrintRangeUsingDefinedNameForLayout(); 
				if (range == null)
					return null;
			}
			else
				range = ModelWorksheet.GetPrintRange();
			return new NativeRange(range, this);
		}
		#endregion
		#region GetDataRange
		public Range GetDataRange() {
			Model.CellRange range = modelWorksheet.GetDataRange();
			return new NativeRange(range, this);
		}
		#endregion
		#region GetUsedRange
		public Range GetUsedRange() {
			Model.CellRange range = ModelWorksheet.GetUsedRange();
			return new NativeRange(range, this);
		}
		#endregion
		public Range this[string reference] { get { return CreateRange(reference, ReferenceStyle.A1); } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		public Cell this[int rowIndex, int columnIndex] { get { return Cells[rowIndex, columnIndex]; } }
		public void CopyRange(Range source, Model.CellRangeBase target, PasteSpecial pasteType) {
			Model.CellRange sourceRange = (source.Worksheet as NativeWorksheet).GetModelSingleRange(source);
			Model.ModelPasteSpecialFlags flags = (Model.ModelPasteSpecialFlags)pasteType;
			CopyEverythingBase operation = null;
				SourceTargetRangesForCopy ranges = new SourceTargetRangesForCopy(sourceRange, target);
				operation = new RangeCopyOperation(ranges, flags);
			operation.ErrorHandler = API.Native.Implementation.ApiErrorHandler.Instance;
			operation.Execute();
		}
		public void MoveRangeTo(Model.CellRangeBase sourceRangeBase, Range target) {
			CheckUnionRange(sourceRangeBase);
			Model.CellRange sourceRange = (Model.CellRange)sourceRangeBase;
			NativeWorksheet targetWorksheet = target.Worksheet as NativeWorksheet;
			Model.CellRangeBase targetRange = targetWorksheet.GetModelSingleRange(target);
			DocumentModel.BeginUpdate();
			targetWorksheet.DocumentModel.BeginUpdate();
			try {
				var ranges = new SourceTargetRangesForCopy(sourceRange, targetRange);
				var operation = new Model.CopyOperation.CutRangeOperation(ranges);
				operation.ErrorHandler = API.Native.Implementation.ApiErrorHandler.Instance;
				operation.Execute();
				Model.CellRangeBase rangeToClear = operation.GetRangeToClearAfterCut();
				modelWorksheet.ClearAll(rangeToClear, API.Native.Implementation.ApiErrorHandler.Instance);
				modelWorksheet.ClearCellsNoShift(rangeToClear);
			}
			finally {
				DocumentModel.EndUpdate();
				targetWorksheet.DocumentModel.EndUpdate();
			}
		}
		public void ValidateRange(Range range) {
			Model.CellRangeBase modelRange = GetModelRange(range);
			if (!Object.ReferenceEquals(modelRange.Worksheet, ModelWorksheet))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
		}
		#region Clear
		public void Clear(Range range) {
			ModelWorksheet.ClearAll(GetModelRange(range), API.Native.Implementation.ApiErrorHandler.Instance);
		}
		#endregion
		#region ClearContents
		public void ClearContents(Range range) {
			ModelWorksheet.ClearContents(GetModelRange(range), ApiErrorHandler.Instance);
		}
		#endregion
		#region ClearFormats
		public void ClearFormats(Range range) {
			ModelWorksheet.ClearFormats(GetModelRange(range));
		}
		#endregion
		#region ClearHyperlinks
		public void ClearHyperlinks(Range range) {
			ModelWorksheet.ClearHyperlinks(GetModelRange(range), false, true);
		}
		#endregion
		#region ClearComments
		public void ClearComments(Range range) {
			ModelWorksheet.ClearComments(GetModelRange(range));
		}
		#endregion
		#region MergeCells
		public void MergeCells(Range range) {
			Model.CellRangeBase modelRange = GetModelRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Relative);
			ModelWorksheet.MergeCells(modelRange, true, ApiErrorHandler.Instance);
		}
		public void MergeCells(Range range, MergeCellsMode mode) {
			bool expandWithIntersectedMergedCells = (mode & MergeCellsMode.IgnoreIntersections) == 0;
			bool mergeByRows = (mode & MergeCellsMode.ByRows) != 0;
			bool mergeByColumns = (mode & MergeCellsMode.ByColumns) != 0;
			Model.CellRangeBase modelRange = GetModelRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Relative);
			Model.MergeCellsMode modelMode = Model.MergeCellsMode.Default;
			if (mergeByRows != mergeByColumns)
				modelMode = mergeByRows ? Model.MergeCellsMode.ByRows : Model.MergeCellsMode.ByColumns;
			ModelWorksheet.MergeCells(modelRange, expandWithIntersectedMergedCells, modelMode, ApiErrorHandler.Instance);
		}
		#endregion
		#region UnMergeCells
		public void UnMergeCells(Range range) {
			ModelWorksheet.UnMergeCells(GetModelRange(range), ApiErrorHandler.Instance);
		}
		#endregion
		#region DeleteCells
		public void DeleteCells(Range range) {
			this.DeleteCells(range, DeleteMode.ShiftCellsLeft);
		}
		public void DeleteCells(Range range, DeleteMode mode) {
			Model.CellRangeBase modelRange = GetModelRange(range);
			switch (mode) {
				case DeleteMode.EntireColumn:
					CheckUnionRange(modelRange);
					Columns.Remove(range.LeftColumnIndex, range.RightColumnIndex - range.LeftColumnIndex + 1);
					break;
				case DeleteMode.EntireRow:
					CheckUnionRange(modelRange);
					Rows.Remove(range.TopRowIndex, range.BottomRowIndex - range.TopRowIndex + 1);
					break;
				case DeleteMode.ShiftCellsLeft: {
						((NativeWorksheet)range.Worksheet).ModelWorksheet.RemoveRange(modelRange, Model.RemoveCellMode.ShiftCellsLeft, ApiErrorHandler.Instance);
						break;
					}
				default: {
						((NativeWorksheet)range.Worksheet).ModelWorksheet.RemoveRange(modelRange, Model.RemoveCellMode.ShiftCellsUp, ApiErrorHandler.Instance);
						break;
					}
			}
		}
		#endregion
		#region InsertCells
		public void InsertCells(Range range) {
			if (range.RowCount == modelWorksheet.MaxRowCount)
				InsertCells(range, InsertCellsMode.EntireColumn);
			else if (range.ColumnCount == modelWorksheet.MaxColumnCount)
				InsertCells(range, InsertCellsMode.EntireRow);
			else
				InsertCells(range, InsertCellsMode.ShiftCellsDown);
		}
		public void InsertCells(Range range, InsertCellsMode mode) {
			Model.CellRangeBase modelRange = GetModelRange(range);
			switch (mode) {
				case InsertCellsMode.EntireColumn:
					InsertColumns(range.LeftColumnIndex, range.ColumnCount);
					break;
				case InsertCellsMode.EntireRow:
					InsertRows(range.TopRowIndex, range.RowCount);
					break;
				case InsertCellsMode.ShiftCellsDown:
					if (range.RowCount < modelWorksheet.MaxRowCount) {
						CanInsertCells(modelRange, Model.InsertCellMode.ShiftCellsDown);
						ModelWorksheet.InsertRange(modelRange, Model.InsertCellMode.ShiftCellsDown, Model.InsertCellsFormatMode.ClearFormat, ApiErrorHandler.Instance);
					}
					break;
				case InsertCellsMode.ShiftCellsRight:
					if (range.ColumnCount < modelWorksheet.MaxColumnCount) {
						CanInsertCells(modelRange, Model.InsertCellMode.ShiftCellsRight);
						ModelWorksheet.InsertRange(modelRange, Model.InsertCellMode.ShiftCellsRight, Model.InsertCellsFormatMode.ClearFormat, ApiErrorHandler.Instance);
					}
					break;
			}
		}
		internal void InsertColumns(int columnIndex, int count) {
			InsertColumns(columnIndex, count, Model.InsertCellsFormatMode.ClearFormat);
		}
		internal void InsertColumns(int columnIndex, int count, Model.InsertCellsFormatMode formatMode) {
			if (count >= modelWorksheet.MaxColumnCount)
				return;
			ModelWorksheet.InsertColumns(columnIndex, count, formatMode, ApiErrorHandler.Instance);
		}
		internal void InsertRows(int rowIndex, int count) {
			InsertRows(rowIndex, count, Model.InsertCellsFormatMode.ClearFormat);
		}
		internal void InsertRows(int rowIndex, int count, Model.InsertCellsFormatMode formatMode) {
			if (count >= modelWorksheet.MaxRowCount)
				return;
			ModelWorksheet.InsertRows(rowIndex, count, formatMode, ApiErrorHandler.Instance);
		}
		void CanInsertCells(Model.CellRangeBase modelRange, Model.InsertCellMode mode) {
			if (!ModelWorksheet.AutoFilter.CanRangeInsert(modelRange, mode))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ChangingRangeOfAutoFilterNotAllowed);
		}
		#endregion
		internal double GetColumnWidth(int columnIndex) {
			Model.DocumentModel documentModel = ModelWorksheet.Workbook;
			float maxDigitWidth = documentModel.MaxDigitWidth;
			float maxDigitWidthInPixels = documentModel.MaxDigitWidthInPixels;
			float layoutWidth = documentModel.GetService<Model.IColumnWidthCalculationService>().CalculateColumnWidth(ModelWorksheet, columnIndex, maxDigitWidth, maxDigitWidthInPixels);
			float modelWidth = documentModel.ToDocumentLayoutUnitConverter.ToModelUnits(layoutWidth);
			return NativeWorkbook.ModelUnitsToUnitsF(modelWidth);
		}
		internal double GetColumnWidthInCharacters(int columnIndex) {
			Model.IColumnRange column = ModelWorksheet.Columns.GetColumnRangeForReading(columnIndex);
			if (!column.IsVisible)
				return 0;
			if (column.IsCustomWidth || column.Width != 0)
				return column.Width;
			return DefaultColumnWidthInCharacters;
		}
		internal double GetRowHeight(int rowIndex) {
			Model.Row row = ModelWorksheet.Rows.TryGetRow(rowIndex);
			if (row == null)
				return DefaultRowHeight;
			if (row.IsHidden)
				return 0;
			if (row.IsCustomHeight)
				return NativeWorkbook.ModelUnitsToUnitsF(row.Height);
			Model.SheetFormatProperties formatProperties = ModelWorksheet.Properties.FormatProperties;
			if (formatProperties.IsCustomHeight && formatProperties.DefaultRowHeight > 0 && row.Height == 0)
				return NativeWorkbook.ModelUnitsToUnitsF(formatProperties.DefaultRowHeight);
			Model.IColumnWidthCalculationService service = ModelWorkbook.GetService<Model.IColumnWidthCalculationService>();
			float resultHeight = service.CalculateRowMaxCellHeight(row, ModelWorkbook.MaxDigitWidth, ModelWorkbook.MaxDigitWidthInPixels);
			resultHeight = ModelWorkbook.ToDocumentLayoutUnitConverter.ToModelUnits(resultHeight);
			return NativeWorkbook.ModelUnitsToUnitsF(resultHeight);
		}
		internal void RemoveTable(NativeTable table) {
			tables.Remove(table);
		}
		#region IRangeProvider members
		Range IRangeProvider.this[string reference] { get { return CreateRange(reference, ReferenceStyle.A1); } }
		Range IRangeProvider.FromLTRB(int leftColumnIndex, int topRowIndex, int rightColumnIndex, int bottomRowIndex) {
			return CreateRange(topRowIndex, leftColumnIndex, bottomRowIndex, rightColumnIndex);
		}
		Range IRangeProvider.Parse(string reference) {
			return CreateRange(reference, ReferenceStyle.A1);
		}
		Range IRangeProvider.Parse(string reference, ReferenceStyle style) {
			return CreateRange(reference, style);
		}
		Range IRangeProvider.Union(params Range[] ranges) {
			return NativeRangeReferenceParseHelper.CreateUnion(ranges);
		}
		Range IRangeProvider.Union(IEnumerable<Range> enumerable) {
			return NativeRangeReferenceParseHelper.CreateUnion(enumerable);
		}
		#endregion
		#region SplitPosition
		public int SplitLeftPosition {
			get {
				CheckValid();
				return ModelWorksheet.ActiveView.HorizontalSplitPosition;
			}
		}
		public int SplitTopPosition {
			get {
				CheckValid();
				return ModelWorksheet.ActiveView.VerticalSplitPosition;
			}
		}
		#endregion
		#region Freeze/Unfreeze
		public void FreezePanes(int rowOffset, int columnOffset) {
			FreezePanesCore(rowOffset, columnOffset, null);
		}
		public void FreezePanes(int rowOffset, int columnOffset, Range topLeft) {
			FreezePanesCore(rowOffset, columnOffset, topLeft);
		}
		void FreezePanesCore(int rowOffset, int columnOffset, Range topLeft) {
			CheckFrozenRowOffsetValid(rowOffset, topLeft);
			CheckFrozenColumnOffsetValid(columnOffset, topLeft);
			ModelCellPosition position = GetModelCellPosition(rowOffset + 1, columnOffset + 1);
			if (topLeft == null)
				ModelWorksheet.FreezePanes(position);
			else
				ModelWorksheet.FreezePanes(position, GetModelCellPosition(topLeft));
		}
		public void FreezeColumns(int columnOffset) {
			FreezeColumnsCore(columnOffset, null);
		}
		public void FreezeColumns(int columnOffset, Range topLeft) {
			FreezeColumnsCore(columnOffset, topLeft);
		}
		void FreezeColumnsCore(int columnOffset, Range topLeft) {
			CheckFrozenColumnOffsetValid(columnOffset, topLeft);
			int xSplit = columnOffset + 1;
			if (topLeft == null)
				ModelWorksheet.FreezeColumns(xSplit);
			else
				ModelWorksheet.FreezeColumns(xSplit, GetModelCellPosition(topLeft));
		}
		void CheckFrozenColumnOffsetValid(int columnOffset, Range topLeft) {
			if (columnOffset < 0)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorNegativeFrozenColumnOffset, "columnOffset");
			int leftColumnIndex = topLeft != null ? topLeft.LeftColumnIndex : 0;
			leftColumnIndex += 1;
			if (leftColumnIndex + columnOffset >= IndicesChecker.MaxRowCount)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorColumnOffsetRefersBeyondWorksheet, "columnOffset");
		}
		public void FreezeRows(int rowOffset) {
			FreezeRowsCore(rowOffset, null);
		}
		public void FreezeRows(int rowOffset, Range topLeft) {
			FreezeRowsCore(rowOffset, topLeft);
		}
		void FreezeRowsCore(int rowOffset, Range topLeft) {
			CheckFrozenRowOffsetValid(rowOffset, topLeft);
			int ySplit = rowOffset + 1;
			if (topLeft == null)
				ModelWorksheet.FreezeRows(ySplit);
			else
				ModelWorksheet.FreezeRows(ySplit, GetModelCellPosition(topLeft));
		}
		void CheckFrozenRowOffsetValid(int rowOffset, Range topLeft) {
			if (rowOffset < 0)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorNegativeFrozenRowOffset, "rowOffset");
			int topRowIndex = topLeft != null ? topLeft.TopRowIndex : 0;
			topRowIndex += 1;
			if (topRowIndex + rowOffset >= IndicesChecker.MaxRowCount)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorRowOffsetRefersBeyondWorksheet, "rowOffset");
		}
		public void UnfreezePanes() {
			ModelWorksheet.UnfreezePanes();
		}
		protected internal ModelCellPosition GetModelCellPosition(Range range) {
			return new ModelCellPosition(range.LeftColumnIndex, range.TopRowIndex);
		}
		protected internal ModelCellPosition GetModelCellPosition(int row, int column) {
			return new ModelCellPosition(column, row);
		}
		#endregion
		#region ScrollToRow/ScrollToColumn
		public void ScrollToRow(int rowIndex) {
			NativeIndicesChecker.CheckRowIndex(rowIndex);
			modelWorksheet.ScrollToRow(rowIndex);
		}
		public void ScrollToRow(string rowHeading) {
			int index = NativeCellReferenceParser.GetRowIndexByHeading(rowHeading);
			ScrollToRow(index);
		}
		public void ScrollToColumn(int columnIndex) {
			NativeIndicesChecker.CheckColumnIndex(columnIndex);
			modelWorksheet.ScrollToColumn(columnIndex);
		}
		public void ScrollToColumn(string columnHeading) {
			bool R1C1ReferenceStyle = NativeWorkbook.DocumentSettings.R1C1ReferenceStyle;
			int index = NativeCellReferenceParser.GetColumnIndexByHeading(columnHeading, R1C1ReferenceStyle);
			ScrollToColumn(index);
		}
		public void ScrollTo(int rowIndex, int columnIndex) {
			NativeIndicesChecker.CheckRowIndex(rowIndex);
			NativeIndicesChecker.CheckColumnIndex(columnIndex);
			modelWorksheet.ScrollTo(rowIndex, columnIndex);
		}
		public void ScrollTo(Range scrolledAreaTopLeftCell) {
			if (!Object.ReferenceEquals(scrolledAreaTopLeftCell.Worksheet, this))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
			ScrollTo(scrolledAreaTopLeftCell.TopRowIndex, scrolledAreaTopLeftCell.LeftColumnIndex);
		}
		#endregion
		#region ExternalWorksheet Members
		public CellValue GetCellValue(int columnIndex, int rowIndex) {
			return this[rowIndex, columnIndex].Value;
		}
		IEnumerable<ExternalDefinedName> ExternalWorksheet.DefinedNames {
			get { return DefinedNames; }
		}
		#endregion
		public IEnumerable<Cell> Search(string text) {
			ModelSearchOptions modelOptions = CreateDefaultSearchOptions();
			modelOptions.Sheet = modelWorksheet;
			return SearchCore(text, modelOptions);
		}
		public IEnumerable<Cell> Search(string text, SearchOptions options) {
			ModelSearchOptions modelOptions = ConvertSearchOptions(options);
			modelOptions.Sheet = modelWorksheet;
			return SearchCore(text, modelOptions);
		}
		internal ModelSearchOptions CreateDefaultSearchOptions() {
			return workbook.CreateDefaultSearchOptions();
		}
		internal ModelSearchOptions ConvertSearchOptions(SearchOptions options) {
			return workbook.ConvertSearchOptions(options);
		}
		internal IEnumerable<Cell> SearchCore(string text, ModelSearchOptions options) {
			return workbook.SearchCore(text, options, ConvertModelCellToApiCell);
		}
		public IComparers Comparers { get { return comparers; } }
		public void Sort(Range range) {
			Sort(range, 0);
		}
		public void Sort(Range range, int columnOffset) {
			Sort(range, columnOffset, true);
		}
		public void Sort(Range range, bool ascending) {
			Sort(range, 0, ascending);
		}
		public void Sort(Range range, int columnOffset, bool ascending) {
			IComparer<Model.VariantValue> comparer;
			if (ascending)
				comparer = new Model.SortVariantValueComparer(ModelWorkbook.SharedStringTable);
			else
				comparer = new Model.DescendingSortVariantValueComparer(ModelWorkbook.SharedStringTable);
			SortCore(range, columnOffset, comparer);
		}
		public void Sort(Range range, int columnOffset, IComparer<CellValue> comparer) {
			SortCore(range, columnOffset, new Model.ApiCellValueComparer(ModelWorkbook.DataContext, comparer));
		}
		public void Sort(Range range, IEnumerable<SortField> sortFields) {
			if (ContainsPivotTable(range)) {
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged);
				return;
			}
			Model.CellRange modelRange = GetModelSingleRange(range);
			List<Model.ModelSortField> fields = new List<Model.ModelSortField>();
			foreach (SortField sortField in sortFields) {
				int columnOffset = sortField.ColumnOffset;
				if (columnOffset < 0 || columnOffset > range.ColumnCount - 1)
					return;
				Model.ModelSortField field = ConvertSortField(sortField);
				if (field != null)
					fields.Add(field);
			}
			Model.RangeSortEngine engine = new Model.RangeSortEngine(ModelWorksheet);
			engine.Sort(modelRange, fields);
		}
		public bool ContainsPivotTable(Range range) {
			if (range != null) {
				NativeWorksheet nWorksheet = range.Worksheet as NativeWorksheet;
				if (nWorksheet != null) {
					Model.CellRangeBase cRange = nWorksheet.GetModelRange(range);
					return ModelWorkbook.ActiveSheet.PivotTables.ContainsItemsInRange(cRange, true);
				}
			}
			return false;
		}
		Model.ModelSortField ConvertSortField(SortField sortField) {
			if (sortField.Comparer == null)
				return null;
			Model.ModelSortField result = new Model.ModelSortField();
			result.ColumnOffset = sortField.ColumnOffset;
			result.Comparer = new Model.ApiCellValueComparer(ModelWorkbook.DataContext, sortField.Comparer);
			return result;
		}
		void SortCore(Range range, int columnOffset, IComparer<Model.VariantValue> comparer) {
			if (columnOffset < 0 || columnOffset > range.ColumnCount - 1)
				return;
			if (ContainsPivotTable(range)) {
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged);
				return;
			}
			Model.CellRange modelRange = GetModelSingleRange(range);
			Model.ModelSortField sortField = new Model.ModelSortField();
			sortField.Comparer = comparer;
			sortField.ColumnOffset = columnOffset;
			List<Model.ModelSortField> sortFields = new List<Model.ModelSortField>();
			sortFields.Add(sortField);
			Model.RangeSortEngine engine = new Model.RangeSortEngine(ModelWorksheet);
			engine.Sort(modelRange, sortFields);
		}
		Cell ConvertModelCellToApiCell(Model.ICell cell) {
			return this[cell.RowIndex, cell.ColumnIndex];
		}
		public IList<Range> GetSelectedRanges() {
			List<Range> result = new List<Spreadsheet.Range>();
			int count = ModelWorksheet.Selection.SelectedRanges.Count;
			for (int i = 0; i < count; i++)
				result.Add(new NativeRange(ModelWorksheet.Selection.SelectedRanges[i], this));
			return result;
		}
		public bool SetSelectedRanges(IList<Range> ranges) {
			return SetSelectedRanges(ranges, true);
		}
		public bool SetSelectedRanges(IList<Range> ranges, bool expandToMergedCellsSize) {
			if (ranges == null)
				return false;
			List<Model.CellRangeBase> modelRanges = new List<Model.CellRangeBase>();
			int count = ranges.Count;
			for (int i = 0; i < count; i++) {
				Model.CellRangeBase range = GetModelRange(ranges[i]);
				if (range != null)
					modelRanges.Add(range.Clone());
			}
			return ModelWorksheet.Selection.SetSelectedRanges(modelRanges, expandToMergedCellsSize);
		}
		#region Protection
		public void Protect(string password, WorksheetProtectionPermissions permissions) {
			Model.WorksheetProtectionOptions protection = modelWorksheet.Properties.Protection;
			protection.BeginUpdate();
			try {
				protection.SelectLockedCellsLocked = (permissions & WorksheetProtectionPermissions.SelectLockedCells) == 0;
				protection.SelectUnlockedCellsLocked = (permissions & WorksheetProtectionPermissions.SelectUnlockedCells) == 0;
				protection.FormatCellsLocked = (permissions & WorksheetProtectionPermissions.FormatCells) == 0;
				protection.FormatColumnsLocked = (permissions & WorksheetProtectionPermissions.FormatColumns) == 0;
				protection.FormatRowsLocked = (permissions & WorksheetProtectionPermissions.FormatRows) == 0;
				protection.InsertColumnsLocked = (permissions & WorksheetProtectionPermissions.InsertColumns) == 0;
				protection.InsertRowsLocked = (permissions & WorksheetProtectionPermissions.InsertRows) == 0;
				protection.InsertHyperlinksLocked = (permissions & WorksheetProtectionPermissions.InsertHyperlinks) == 0;
				protection.DeleteColumnsLocked = (permissions & WorksheetProtectionPermissions.DeleteColumns) == 0;
				protection.DeleteRowsLocked = (permissions & WorksheetProtectionPermissions.DeleteRows) == 0;
				protection.SortLocked = (permissions & WorksheetProtectionPermissions.Sort) == 0;
				protection.AutoFiltersLocked = (permissions & WorksheetProtectionPermissions.AutoFilters) == 0;
				protection.PivotTablesLocked = (permissions & WorksheetProtectionPermissions.PivotTables) == 0;
				protection.ObjectsLocked = (permissions & WorksheetProtectionPermissions.Objects) == 0;
				protection.ScenariosLocked = (permissions & WorksheetProtectionPermissions.Scenarios) == 0;
			}
			finally {
				protection.EndUpdate();
			}
			modelWorksheet.Protect(password);
		}
		public bool IsProtected { get { return modelWorksheet.IsProtected; } }
		public bool Unprotect(string password) {
			return modelWorksheet.UnProtect(password) == null;
		}
		public ProtectedRangeCollection ProtectedRanges {
			get {
				if (protectedRanges == null)
					protectedRanges = new NativeProtectedRangeCollection(this);
				return protectedRanges;
			}
		}
		#endregion
		#region Outline
		public void AutoOutline() {
			if (rows != null)
				rows.AutoOutline();
			Columns.AutoOutline();
		}
		public void ClearOutline() {
			Model.ClearWorksheetOutlineCommand command = new Model.ClearWorksheetOutlineCommand(this.ModelWorksheet);
			command.Execute();
		}
		public void Subtotal(Range range, int groupByColumn, List<int> subtotalColumnList, int functionCode, string functionText) {
			Model.SubtotalModelCommand command = new Model.SubtotalModelCommand(this.ModelWorksheet, API.Native.Implementation.ApiErrorHandler.Instance, GetModelRange(range));
			command.ChangedColumnIndex = groupByColumn - range.LeftColumnIndex;
			command.SubTotalColumnIndices = new List<int>();
			foreach (int subtotalColumn in subtotalColumnList)
				command.SubTotalColumnIndices.Add(subtotalColumn - range.LeftColumnIndex);
			command.FunctionType = functionCode;
			command.Text = functionText;
			command.NeedInsertTextColumn = true;
			command.Execute();
		}
		public void RemoveSubtotal(Range range) {
			Model.SubtotalRemoveCommand command = new Model.SubtotalRemoveCommand(this.ModelWorksheet, GetModelRange(range) as Model.CellRange, ApiErrorHandler.Instance);
			command.Execute();
		}
		public WorksheetOutlineOptions OutlineOptions { get { return this; } }
		#endregion
		public override string ToString() {
			return String.Format("Worksheet: \"{0}\" Index:\"{1}\"", Name, Index);
		}
		public object Tag {
			get {
				CheckValid();
				return ModelWorksheet.Tag;
			}
			set {
				CheckValid();
				ModelWorksheet.Tag = value;
			}
		}
		#region DataValidations
		public DataValidationCollection DataValidations {
			get {
				CheckValid();
				if (dataValidations == null)
					dataValidations = new NativeDataValidationCollection(this);
				return dataValidations;
			}
		}
		#endregion
		#region SparklineGroups
		public SparklineGroupCollection SparklineGroups {
			get {
				CheckValid();
				if (sparklineGroups == null)
					sparklineGroups = new NativeSparklineGroupCollection(this);
				return sparklineGroups;
			}
		}
		#endregion
		#region IgnoredErrors
		public IgnoredErrorCollection IgnoredErrors {
			get {
				CheckValid();
				if (ignoredErrors == null)
					ignoredErrors = new NativeIgnoredErrorCollection(this);
				return ignoredErrors;
			}
		}
		#endregion
		#region PivotTables
		public PivotTableCollection PivotTables {
			get {
				CheckValid();
				if (pivotTables == null)
					pivotTables = new NativePivotTableCollection(this);
				return pivotTables;
			}
		}
		#endregion
		#region WorksheetOutlineOptions Members
		bool WorksheetOutlineOptions.SummaryRowsBelow {
			get {
				return modelWorksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow;
			}
			set {
				if (modelWorksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow != value) {
					modelWorksheet.RowGroupCache = null;
					modelWorksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow = value;
				}
			}
		}
		bool WorksheetOutlineOptions.SummaryColumnsRight {
			get {
				return modelWorksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight;
			}
			set {
				if (modelWorksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight != value) {
					modelWorksheet.ColumnGroupCache = null;
					modelWorksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight = value;
				}
			}
		}
		#endregion
		public void Calculate() {
			ModelWorkbook.CalculationChain.CalculateWorksheet(ModelWorksheet);
		}
	}
	#endregion
	#region NativeMargins
	partial class NativeMargins : Margins {
		#region Fields
		readonly NativeWorkbook workbook;
		readonly ModelMargins modelMargins;
		#endregion
		public NativeMargins(NativeWorkbook workbook, ModelMargins modelMargins) {
			Guard.ArgumentNotNull(workbook, "workbook");
			Guard.ArgumentNotNull(modelMargins, "modelMargins");
			this.modelMargins = modelMargins;
			this.workbook = workbook;
		}
		#region Properties
		public float Left {
			get { return workbook.ModelUnitsToUnitsF(modelMargins.Left); }
			set { modelMargins.Left = workbook.UnitsToModelUnits(value); }
		}
		public float Top {
			get { return workbook.ModelUnitsToUnitsF(modelMargins.Top); }
			set { modelMargins.Top = workbook.UnitsToModelUnits(value); }
		}
		public float Right {
			get { return workbook.ModelUnitsToUnitsF(modelMargins.Right); }
			set { modelMargins.Right = workbook.UnitsToModelUnits(value); }
		}
		public float Bottom {
			get { return workbook.ModelUnitsToUnitsF(modelMargins.Bottom); }
			set { modelMargins.Bottom = workbook.UnitsToModelUnits(value); }
		}
		public float Header {
			get { return workbook.ModelUnitsToUnitsF(modelMargins.Header); }
			set { modelMargins.Header = workbook.UnitsToModelUnits(value); }
		}
		public float Footer {
			get { return workbook.ModelUnitsToUnitsF(modelMargins.Footer); }
			set { modelMargins.Footer = workbook.UnitsToModelUnits(value); }
		}
		#endregion
	}
	#endregion
	#region NativeWorksheetView
	partial class NativeWorksheetView : WorksheetView {
		#region Fields
		readonly ModelWorksheet modelWorksheet;
		readonly NativeWorkbook workbook;
		NativeMargins margins;
		#endregion
		public NativeWorksheetView(NativeWorkbook workbook, ModelWorksheet modelWorksheet) {
			Guard.ArgumentNotNull(modelWorksheet, "modelWorksheet");
			Guard.ArgumentNotNull(workbook, "workbook");
			this.modelWorksheet = modelWorksheet;
			this.workbook = workbook;
			CreateApiObjects();
		}
		#region Properties
		public Margins Margins { get { return margins; } }
		public PageOrientation Orientation {
			get { return (PageOrientation)modelWorksheet.PrintSetup.Orientation; }
			set { modelWorksheet.PrintSetup.Orientation = (Model.ModelPageOrientation)value; }
		}
		public PaperKind PaperKind {
			get { return modelWorksheet.PrintSetup.PaperKind; }
			set { modelWorksheet.PrintSetup.PaperKind = value; }
		}
		public WorksheetViewType ViewType {
			get { return (WorksheetViewType)modelWorksheet.ActiveView.ViewType; }
			set { modelWorksheet.ActiveView.ViewType = (Model.SheetViewType)value; }
		}
		public bool ShowFormulas {
			get { return modelWorksheet.ActiveView.ShowFormulas; }
			set { modelWorksheet.ActiveView.ShowFormulas = value; }
		}
		public bool ShowGridlines {
			get { return modelWorksheet.ActiveView.ShowGridlines; }
			set { modelWorksheet.ActiveView.ShowGridlines = value; }
		}
		public bool ShowHeadings {
			get { return modelWorksheet.ActiveView.ShowRowColumnHeaders; }
			set { modelWorksheet.ActiveView.ShowRowColumnHeaders = value; }
		}
		public int Zoom {
			get { return modelWorksheet.ActiveView.ZoomScale; }
			set { modelWorksheet.ActiveView.ZoomScale = value; }
		}
		public Color TabColor {
			get { return modelWorksheet.GetTabColor(); }
			set { modelWorksheet.SetTabColor(value); }
		}
		#endregion
		protected internal virtual void CreateApiObjects() {
			this.margins = new NativeMargins(workbook, modelWorksheet.Margins);
		}
		protected internal virtual void Invalidate() {
			this.margins = null;
		}
	}
	#endregion
	#region NativeWorksheetPrintOptions
	partial class NativeWorksheetPrintOptions : WorksheetPrintOptions {
		#region Fields
		readonly ModelPrintSetup modelPrintSetup;
		NativeWorksheetPageNumbering pageNumbering;
		#endregion
		public NativeWorksheetPrintOptions(ModelPrintSetup modelPrintSetup) {
			Guard.ArgumentNotNull(modelPrintSetup, "modelPrintSetup");
			this.modelPrintSetup = modelPrintSetup;
			CreateApiObjects();
		}
		#region Properties
		public WorksheetPageNumbering PageNumbering { get { return pageNumbering; } }
		public CommentsPrintMode CommentsPrintMode {
			get { return (CommentsPrintMode)modelPrintSetup.CommentsPrintMode; }
			set { modelPrintSetup.CommentsPrintMode = (Model.ModelCommentsPrintMode)value; }
		}
		public ErrorsPrintMode ErrorsPrintMode {
			get { return (ErrorsPrintMode)modelPrintSetup.ErrorsPrintMode; }
			set { modelPrintSetup.ErrorsPrintMode = (Model.ModelErrorsPrintMode)value; }
		}
		public PageOrder PageOrder {
			get { return (PageOrder)modelPrintSetup.PagePrintOrder; }
			set { modelPrintSetup.PagePrintOrder = (Model.PagePrintOrder)value; }
		}
		public int Scale {
			get { return modelPrintSetup.Scale; }
			set { modelPrintSetup.Scale = value; }
		}
		public bool BlackAndWhite {
			get { return modelPrintSetup.BlackAndWhite; }
			set { modelPrintSetup.BlackAndWhite = value; }
		}
		public bool AutoPageBreaks {
			get { return modelPrintSetup.AutoPageBreaks; }
			set { modelPrintSetup.AutoPageBreaks = value; }
		}
		public bool FitToPage {
			get { return modelPrintSetup.FitToPage; }
			set { modelPrintSetup.FitToPage = value; }
		}
		public int NumberOfCopies {
			get { return modelPrintSetup.Copies; }
			set { modelPrintSetup.Copies = value; }
		}
		public int FitToWidth {
			get { return modelPrintSetup.FitToWidth; }
			set { modelPrintSetup.FitToWidth = value; }
		}
		public int FitToHeight {
			get { return modelPrintSetup.FitToHeight; }
			set { modelPrintSetup.FitToHeight = value; }
		}
		public bool CenterHorizontally {
			get { return modelPrintSetup.CenterHorizontally; }
			set { modelPrintSetup.CenterHorizontally = value; }
		}
		public bool CenterVertically {
			get { return modelPrintSetup.CenterVertically; }
			set { modelPrintSetup.CenterVertically = value; }
		}
		public bool PrintGridlines {
			get { return modelPrintSetup.PrintGridLines; }
			set { modelPrintSetup.PrintGridLines = value; }
		}
		public bool PrintHeadings {
			get { return modelPrintSetup.PrintHeadings; }
			set { modelPrintSetup.PrintHeadings = value; }
		}
		#endregion
		protected internal virtual void CreateApiObjects() {
			this.pageNumbering = new NativeWorksheetPageNumbering(modelPrintSetup);
		}
		protected internal virtual void Invalidate() {
			this.pageNumbering = null;
		}
	}
	#endregion
	#region NativeWorksheetPageNumbering
	partial class NativeWorksheetPageNumbering : WorksheetPageNumbering {
		#region Fields
		readonly ModelPrintSetup modelPrintSetup;
		#endregion
		public NativeWorksheetPageNumbering(ModelPrintSetup modelPrintSetup) {
			Guard.ArgumentNotNull(modelPrintSetup, "modelPrintSetup");
			this.modelPrintSetup = modelPrintSetup;
		}
		#region Properties
		public int Start {
			get { return modelPrintSetup.FirstPageNumber; }
			set {
				modelPrintSetup.FirstPageNumber = value;
				modelPrintSetup.UseFirstPageNumber = true;
			}
		}
		public WorksheetPageNumberingStartType StartType {
			get { return modelPrintSetup.UseFirstPageNumber ? WorksheetPageNumberingStartType.Restart : WorksheetPageNumberingStartType.Continuous; }
			set { modelPrintSetup.UseFirstPageNumber = value == WorksheetPageNumberingStartType.Restart; }
		}
		#endregion
	}
	#endregion
	#region NativeWorksheetHeaderFooterOptions
	partial class NativeWorksheetHeaderFooterOptions : WorksheetHeaderFooterOptions {
		#region Fields
		readonly ModelHeaderFooterOptions modelHeaderFooter;
		NativeWorksheetHeaderFooter oddHeader;
		NativeWorksheetHeaderFooter oddFooter;
		NativeWorksheetHeaderFooter evenHeader;
		NativeWorksheetHeaderFooter evenFooter;
		NativeWorksheetHeaderFooter firstHeader;
		NativeWorksheetHeaderFooter firstFooter;
		#endregion
		public NativeWorksheetHeaderFooterOptions(ModelHeaderFooterOptions modelHeaderFooter) {
			Guard.ArgumentNotNull(modelHeaderFooter, "modelHeaderFooter");
			this.modelHeaderFooter = modelHeaderFooter;
			CreateApiObjects();
		}
		#region Properties
		public bool AlignWithMargins {
			get { return modelHeaderFooter.AlignWithMargins; }
			set { modelHeaderFooter.AlignWithMargins = value; }
		}
		public bool DifferentFirst {
			get { return modelHeaderFooter.DifferentFirst; }
			set { modelHeaderFooter.DifferentFirst = value; }
		}
		public bool DifferentOddEven {
			get { return modelHeaderFooter.DifferentOddEven; }
			set { modelHeaderFooter.DifferentOddEven = value; }
		}
		public bool ScaleWithDoc {
			get { return modelHeaderFooter.ScaleWithDoc; }
			set { modelHeaderFooter.ScaleWithDoc = value; }
		}
		public WorksheetHeaderFooter OddHeader { get { return oddHeader; } }
		public WorksheetHeaderFooter OddFooter { get { return oddFooter; } }
		public WorksheetHeaderFooter EvenHeader { get { return evenHeader; } }
		public WorksheetHeaderFooter EvenFooter { get { return evenFooter; } }
		public WorksheetHeaderFooter FirstHeader { get { return firstHeader; } }
		public WorksheetHeaderFooter FirstFooter { get { return firstFooter; } }
		#endregion
		protected internal virtual void CreateApiObjects() {
			this.oddHeader = new NativeWorksheetHeaderFooter(modelHeaderFooter, NativeHeaderFooterType.OddHeader);
			this.oddFooter = new NativeWorksheetHeaderFooter(modelHeaderFooter, NativeHeaderFooterType.OddFooter);
			this.evenHeader = new NativeWorksheetHeaderFooter(modelHeaderFooter, NativeHeaderFooterType.EvenHeader);
			this.evenFooter = new NativeWorksheetHeaderFooter(modelHeaderFooter, NativeHeaderFooterType.EvenFooter);
			this.firstHeader = new NativeWorksheetHeaderFooter(modelHeaderFooter, NativeHeaderFooterType.FirstHeader);
			this.firstFooter = new NativeWorksheetHeaderFooter(modelHeaderFooter, NativeHeaderFooterType.FirstFooter);
		}
		protected internal virtual void Invalidate() {
			this.oddHeader = null;
			this.oddFooter = null;
			this.evenHeader = null;
			this.evenFooter = null;
			this.firstHeader = null;
			this.firstFooter = null;
		}
	}
	#endregion
	#region NativeWorksheetHeaderFooter
	public enum NativeHeaderFooterType {
		OddHeader,
		OddFooter,
		EvenHeader,
		EvenFooter,
		FirstHeader,
		FirstFooter
	}
	partial class NativeWorksheetHeaderFooter : WorksheetHeaderFooter {
		#region Fields
		readonly ModelHeaderFooterOptions modelHeaderFooter;
		readonly NativeHeaderFooterType headerFooterType;
		ModelHeaderFooterBuilder builder;
		#endregion
		public NativeWorksheetHeaderFooter(ModelHeaderFooterOptions modelHeaderFooter, NativeHeaderFooterType headerFooterType) {
			Guard.ArgumentNotNull(modelHeaderFooter, "modelHeaderFooter");
			this.modelHeaderFooter = modelHeaderFooter;
			this.headerFooterType = headerFooterType;
			this.builder = new ModelHeaderFooterBuilder(GetValue());
		}
		#region WorksheetHeaderFooter Members
		public bool IsEmpty { get { return builder.IsEmpty; } }
		public string Left {
			get { return builder.Left; }
			set {
				builder.Left = value;
				SetValue(builder.ToString());
			}
		}
		public string Center {
			get { return builder.Center; }
			set {
				builder.Center = value;
				SetValue(builder.ToString());
			}
		}
		public string Right {
			get { return builder.Right; }
			set {
				builder.Right = value;
				SetValue(builder.ToString());
			}
		}
		public void FromLCR(string left, string center, string right) {
			builder.Left = left;
			builder.Center = center;
			builder.Right = right;
			SetValue(builder.ToString());
		}
		public void FromString(string value) {
			builder.FromString(value);
			SetValue(builder.ToString());
		}
		#endregion
		string GetValue() {
			switch (headerFooterType) {
				case NativeHeaderFooterType.OddHeader:
					return modelHeaderFooter.OddHeader;
				case NativeHeaderFooterType.OddFooter:
					return modelHeaderFooter.OddFooter;
				case NativeHeaderFooterType.EvenHeader:
					return modelHeaderFooter.EvenHeader;
				case NativeHeaderFooterType.EvenFooter:
					return modelHeaderFooter.EvenFooter;
				case NativeHeaderFooterType.FirstHeader:
					return modelHeaderFooter.FirstHeader;
				case NativeHeaderFooterType.FirstFooter:
					return modelHeaderFooter.FirstFooter;
			}
			return string.Empty;
		}
		void SetValue(string value) {
			switch (headerFooterType) {
				case NativeHeaderFooterType.OddHeader:
					modelHeaderFooter.OddHeader = value;
					break;
				case NativeHeaderFooterType.OddFooter:
					modelHeaderFooter.OddFooter = value;
					break;
				case NativeHeaderFooterType.EvenHeader:
					modelHeaderFooter.EvenHeader = value;
					break;
				case NativeHeaderFooterType.EvenFooter:
					modelHeaderFooter.EvenFooter = value;
					break;
				case NativeHeaderFooterType.FirstHeader:
					modelHeaderFooter.FirstHeader = value;
					break;
				case NativeHeaderFooterType.FirstFooter:
					modelHeaderFooter.FirstFooter = value;
					break;
			}
		}
	}
	#endregion
	#region RangeReferenceParseHelper
	partial class NativeRangeReferenceParseHelper {
		#region allowedExpressionTypeTable
		static HashSet<Type> allowedExpressionTypeTable = CreateAllowedExpressionTypeTable();
		static HashSet<Type> CreateAllowedExpressionTypeTable() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(Model.ParsedThingRef));
			result.Add(typeof(Model.ParsedThingRef3d));
			result.Add(typeof(Model.ParsedThingArea));
			result.Add(typeof(Model.ParsedThingArea3d));
			result.Add(typeof(Model.ParsedThingName));
			result.Add(typeof(Model.ParsedThingNameX));
			result.Add(typeof(Model.ParsedThingRange));
			result.Add(typeof(Model.ParsedThingUnion));
			result.Add(typeof(Model.ParsedThingIntersect));
			result.Add(typeof(Model.ParsedThingAttrSpace));
			result.Add(typeof(Model.ParsedThingAttrSpaceSemi));
			result.Add(typeof(Model.ParsedThingTable));
			result.Add(typeof(Model.ParsedThingTableExt));
			return result;
		}
		#endregion
		Model.ParsedExpression ParseReference(Model.WorkbookDataContext dataContext, string reference, ReferenceStyle style) {
			Model.ParsedExpression expression = null;
			bool useR1C1 = false;
			if (style == ReferenceStyle.UseDocumentSettings)
				useR1C1 = dataContext.UseR1C1ReferenceStyle;
			else
				useR1C1 = style == ReferenceStyle.R1C1;
			dataContext.PushCulture(System.Globalization.CultureInfo.InvariantCulture);
			dataContext.PushUseR1C1(useR1C1);
			try {
				expression = dataContext.ParseExpression(reference, Model.OperandDataType.None, false);
				return expression;
			}
			finally {
				dataContext.PopUseR1C1();
				dataContext.PopCulture();
			}
		}
		public virtual Range Process(Model.WorkbookDataContext dataContext, string reference, NativeWorksheet nativeWorksheet, ReferenceStyle style) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			Model.ParsedExpression expression = ParseReference(dataContext, reference, style);
			ValidateExpressionType(expression);
			Model.VariantValue calculatedRange = expression.Evaluate(dataContext);
			return ConvertToRange(calculatedRange, nativeWorksheet, expression[0] is Model.ParsedThingName);
		}
		public virtual Range ConvertToRange(Model.VariantValue calculatedRange, NativeWorksheet nativeWorksheet, bool isDefinedName) {
			Model.CellRangeBase modelRangebase = calculatedRange.CellRangeValue;
			if (modelRangebase != null && modelRangebase.RangeType == Model.CellRangeType.UnionRange)
				CheckUnionRange((Model.CellUnion)modelRangebase);
			if (modelRangebase == null || modelRangebase.Worksheet == null) {
				if (isDefinedName)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpressionInDefinedName);
				else
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpression);
			}
			if (!object.ReferenceEquals(modelRangebase.Worksheet, nativeWorksheet.ModelWorksheet))
				nativeWorksheet = (NativeWorksheet)nativeWorksheet.NativeWorkbook.Worksheets[modelRangebase.Worksheet.Name];
			return new NativeRange(modelRangebase, nativeWorksheet);
		}
		void CheckUnionRange(Model.CellUnion cellUnion) {
			if (!cellUnion.CheckSheetSameness())
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUnionRangesWithDifferentWorksheets);
			cellUnion.Worksheet = cellUnion.InnerCellRanges[0].Worksheet;
		}
		public virtual void ValidateExpressionType(Model.ParsedExpression expression) {
			bool correctExpression = true;
			if (expression == null)
				correctExpression = false;
			else
				for (int i = 0; i < expression.Count; i++)
					if (!(expression[i] is Model.ParsedThingMemBase) && !(expression[i] is Model.ParsedThingAttrBase) && !allowedExpressionTypeTable.Contains(expression[i].GetType())) {
						correctExpression = false;
						break;
					}
			if (!correctExpression)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpression);
		}
		public static void ValidateRowColumnIndex(int index, string message) {
		}
		internal static Range CreateUnion(IEnumerable<Range> enumerable) {
			List<Model.CellRangeBase> resultRangesList = new List<Model.CellRangeBase>();
			NativeWorksheet nativeWorksheet = null;
			foreach (Range currentRange in enumerable) {
				if (nativeWorksheet == null)
					nativeWorksheet = (NativeWorksheet)currentRange.Worksheet;
				else
					if (!object.ReferenceEquals(currentRange.Worksheet, nativeWorksheet))
						SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUnionRangesWithDifferentWorksheets);
				Model.CellRangeBase modelRange = nativeWorksheet.GetModelRange(currentRange);
				foreach (Model.CellRange modelRangeArea in modelRange.GetAreasEnumerable())
					resultRangesList.Add(modelRangeArea);
			}
			if (resultRangesList.Count == 0)
				return null;
			if (resultRangesList.Count == 1)
				return new NativeRange(resultRangesList[0], nativeWorksheet);
			Model.CellUnion union = new Model.CellUnion(nativeWorksheet.ModelWorksheet, resultRangesList);
			return new NativeRange(union, nativeWorksheet);
		}
		internal static Range CreateUnion(params Range[] args) {
			return CreateUnion((IEnumerable<Range>)args);
		}
	}
	#endregion
	#region NativeCellValueComparer
	partial class NativeCellValueComparer : IComparer<CellValue> {
		readonly IComparer<Model.VariantValue> comparer;
		public NativeCellValueComparer(IComparer<Model.VariantValue> comparer) {
			Guard.ArgumentNotNull(comparer, "comparer");
			this.comparer = comparer;
		}
		public int Compare(CellValue x, CellValue y) {
			return comparer.Compare(x.ModelVariantValue, y.ModelVariantValue);
		}
	}
	#endregion
	#region CellValueComparers
	public class CellValueComparers : IComparers {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823")]
		readonly ModelWorkbook documentModel;
		readonly IComparer<CellValue> ascending;
		readonly IComparer<CellValue> descending;
		public CellValueComparers(ModelWorkbook documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.ascending = new NativeCellValueComparer(new Model.SortVariantValueComparer(documentModel.SharedStringTable));
			this.descending = new NativeCellValueComparer(new Model.DescendingSortVariantValueComparer(documentModel.SharedStringTable));
		}
		public IComparer<CellValue> Ascending { get { return ascending; } }
		public IComparer<CellValue> Descending { get { return descending; } }
	}
	#endregion
#if DATA_SHEET
	partial class NativeVirtualWorksheet : NativeWorksheet, VirtualWorksheet {
		public NativeVirtualWorksheet(NativeWorkbook workbook, Model.VirtualWorksheet innerWorksheet)
			: base(workbook, innerWorksheet) {
		}
		public new Model.VirtualWorksheet ModelWorksheet { get { return (Model.VirtualWorksheet)base.ModelWorksheet; } }
		#region VirtualWorksheet Members
		public IVirtualData Data { get { return ModelWorksheet.Data; } }
		#endregion
	}
#endif
}
#endregion
