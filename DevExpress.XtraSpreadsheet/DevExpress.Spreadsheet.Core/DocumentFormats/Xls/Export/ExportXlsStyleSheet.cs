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

#define EXPORT_CHARTS
using System;
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Xls;
using System.Text;
using DevExpress.XtraSpreadsheet.Internal;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraExport.Xls;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class DrawingTableInfo {
		public DrawingTableInfo() {
		}
		public DrawingTableInfo(int index, int refCount) {
			Index = index;
			RefCount = refCount;
		}
		public int Index { get; set; }
		public int RefCount { get; set; }
	}
	public class ExportXlsStyleSheet : ExportStyleSheet {
		#region Fields
		int defaultStyleCount = 0;
		int extendedStyleCount = 0;
		int totalXFCount;
		readonly Dictionary<int, int> xfTable = new Dictionary<int, int>();
		readonly Dictionary<CellStyleBase, int> xfStyleTable = new Dictionary<CellStyleBase, int>();
		readonly Dictionary<RunFontInfo, int> chartFontTable = new Dictionary<RunFontInfo, int>();
		readonly List<CellStyleBase> cellStyles = new List<CellStyleBase>();
		readonly List<int> xfList = new List<int>();
		readonly Dictionary<int, int> fontTable = new Dictionary<int, int>();
		readonly List<RunFontInfo> fontList = new List<RunFontInfo>();
		readonly List<RunFontInfo> yMultFontList = new List<RunFontInfo>();
		readonly List<RunFontInfo> dataLabExtFontList = new List<RunFontInfo>();
		XlsRPNContext rpnContext;
		readonly Dictionary<DevExpress.Office.Utils.OfficeImage, DrawingTableInfo> drawingTable = new Dictionary<DevExpress.Office.Utils.OfficeImage, DrawingTableInfo>();
		bool defaultStyleXFUsed;
		bool defaultCellXFUsed;
		#endregion
		public ExportXlsStyleSheet(DocumentModel workbook)
			: base(workbook) {
		}
		#region Properties
		protected int DefaultStylesCount { get { return defaultStyleCount; } set { defaultStyleCount = value; } }
		protected int ExtendedStylesCount { get { return extendedStyleCount; } set { extendedStyleCount = value; } }
		protected internal List<CellStyleBase> CellStyles { get { return cellStyles; } }
		protected internal Dictionary<int, int> XFTable { get { return xfTable; } }
		protected internal Dictionary<RunFontInfo, int> ChartFontTable { get { return chartFontTable; } }
		protected internal Dictionary<CellStyleBase, int> XFStyleTable { get { return xfStyleTable; } }
		protected internal List<int> XFList { get { return xfList; } }
		protected internal Dictionary<int, int> FontTable { get { return fontTable; } }
		protected internal List<RunFontInfo> FontList { get { return fontList; } }
		protected internal List<RunFontInfo> YMultFontList { get { return yMultFontList; } }
		protected internal List<RunFontInfo> DataLabExtFontList { get { return dataLabExtFontList; } }
		protected internal XlsRPNContext RPNContext { get { return rpnContext; } set { rpnContext = value; } }
		protected internal Dictionary<DevExpress.Office.Utils.OfficeImage, DrawingTableInfo> DrawingTable { get { return drawingTable; } }
		protected internal int ObjectsCount { get; set; }
		protected internal int TableCount { get; set; }
		protected internal bool DefaultStyleXFUsed { get { return defaultStyleXFUsed; } }
		protected internal bool DefaultCellXFUsed { get { return defaultCellXFUsed; } }
		protected internal override int MaxNumberFormatIndex { get { return 365; } } 
		protected internal bool ClipboardMode { get; set; }
		#endregion
		protected internal override CellRange GetMaximumCellRange(IWorksheet sheet) {
			CellPosition topLeft = new CellPosition(0, 0);
			CellPosition bottomRight = new CellPosition(XlsDefs.MaxColumnCount - 1, XlsDefs.MaxRowCount - 1);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		#region RegisterCellStyleFormats
		protected internal override void RegisterCellStyleFormats() {
			CollectStyles(Workbook);
			int count = CellStyles.Count;
			System.Diagnostics.Debug.Assert(count >= DefaultStylesCount);
			for (int i = 0; i < DefaultStylesCount; i++) {
				CellStyleBase style = CellStyles[i];
				int formatIndex = style.FormatIndex;
				RegisterItem(CellStyleFormatTable, formatIndex);
				RegisterStyleXF(style);
			}
			RegisterFormatIndex(Workbook.StyleSheet.DefaultCellFormatIndex, true);
			for (int i = DefaultStylesCount; i < ExtendedStylesCount; i++) {
				CellStyleBase style = CellStyles[i];
				int formatIndex = style.FormatIndex;
				RegisterItem(CellStyleFormatTable, formatIndex);
				RegisterStyleXF(style);
			}
			HashSet<CellStyleBase> referencedStyles = new HashSet<CellStyleBase>();
			foreach (int formatIndex in CellFormatTable.Keys) {
				CellFormat cellFormat = (CellFormat)Workbook.Cache.CellFormatCache[formatIndex];
				if (!referencedStyles.Contains(cellFormat.Style))
					referencedStyles.Add(cellFormat.Style);
			}
			CellStyles.Sort(ExtendedStylesCount, count - ExtendedStylesCount, new CellStyleByNameComparer());
			for (int i = ExtendedStylesCount; i < count; i++) {
				CellStyleBase style = CellStyles[i];
				if (referencedStyles.Contains(style)) {
					int formatIndex = style.FormatIndex;
					RegisterItem(CellStyleFormatTable, formatIndex);
					RegisterStyleXF(style);
				}
			}
			foreach (int formatIndex in CellFormatTable.Keys) {
				RegisterFormatIndex(formatIndex, false);
			}
			for (int i = ExtendedStylesCount; i < count; i++) {
				CellStyleBase style = CellStyles[i];
				if (!referencedStyles.Contains(style)) {
					int formatIndex = style.FormatIndex;
					RegisterItem(CellStyleFormatTable, formatIndex);
					RegisterStyleXF(style);
				}
			}
		}
		void RegisterStyleXF(CellStyleBase style) {
			if (!XFStyleTable.ContainsKey(style)) {
				int formatIndex = style.FormatIndex;
				XFStyleTable.Add(style, XFList.Count);
				XFList.Add(formatIndex);
				if (totalXFCount < XlsDefs.MaxStyleXFCount)
					totalXFCount++;
			}
		}
		void RegisterFormatIndex(int formatIndex, bool isDefault) {
			if (!XFTable.ContainsKey(formatIndex)) {
				XFTable.Add(formatIndex, XFList.Count);
				XFList.Add(formatIndex);
			}
			else if (isDefault)
				XFList.Add(formatIndex);
			if (totalXFCount < XlsDefs.MaxCellXFCount)
				totalXFCount++;
		}
		#region Collect styles
		void CollectStyles(DocumentModel workbook) {
			CellStyleBase style = workbook.StyleSheet.CellStyles[0];
			CellStyles.Add(style);
			RegisterDefaultOutlineCellStyle(true);
			RegisterDefaultOutlineCellStyle(false);
			DefaultStylesCount = CellStyles.Count;
			RegisterDefaultNumberAndHLinkStyles();
			RegisterExtendedBuiltInStyles();
			ExtendedStylesCount = CellStyles.Count;
			foreach (int styleIndex in CellStyleIndexTable.Keys) {
				if (styleIndex == 0) continue;
				style = workbook.StyleSheet.CellStyles[styleIndex];
				if (style is OutlineCellStyle) continue;
				BuiltInCellStyle builtInStyle = style as BuiltInCellStyle;
				if (builtInStyle != null) {
					if (builtInStyle.BuiltInId >= 3 && builtInStyle.BuiltInId <= 11) continue;
					if (builtInStyle.BuiltInId >= 15 && builtInStyle.BuiltInId <= 53) continue;
				}
				CellStyles.Add(style);
			}
		}
		void RegisterDefaultOutlineCellStyle(bool isRow) {
			for (int i = 1; i <= 7; i++) {
				string outlineStyle = OutlineCellStyle.CalculateName(isRow, i);
				CellStyleBase style = Workbook.StyleSheet.CellStyles.GetCellStyleByName(outlineStyle);
				if (style != null)
					CellStyles.Add(style);
				else
					RegisterDefaultOutlineStyle(isRow, i);
			}
		}
		void RegisterDefaultOutlineStyle(bool isRow, int outlineLevel) {
			CellStyleFormat styleFormat = new CellStyleFormat(Workbook);
			OutlineCellStyle outlineStyle = new OutlineCellStyle(Workbook, isRow, outlineLevel, styleFormat);
			CellStyles.Add(outlineStyle);
		}
		void RegisterDefaultNumberAndHLinkStyles() {
			Dictionary<int, BuiltInCellStyle> builtInStyles = GetBuiltInStyles();
			for (int i = 3; i <= 7; i++) {
				BuiltInCellStyle style;
				if (builtInStyles.ContainsKey(i))
					style = builtInStyles[i];
				else
					style = new BuiltInCellStyle(Workbook, i);
				CellStyles.Add(style);
			}
			CellStyles.AddRange(GetBuiltInStyles(8));
			CellStyles.AddRange(GetBuiltInStyles(9));
		}
		void RegisterExtendedBuiltInStyles() {
			for (int i = 10; i <= 53; i++) {
				if (i >= 12 && i <= 14) continue;
				string styleName = BuiltInCellStyleCalculator.CalculateName(i);
				CellStyleBase style = Workbook.StyleSheet.CellStyles.GetCellStyleByName(styleName);
				if (style == null)
					style = new BuiltInCellStyle(Workbook, i);
				CellStyles.Add(style);
			}
		}
		#endregion
		Dictionary<int, BuiltInCellStyle> GetBuiltInStyles() {
			Dictionary<int, BuiltInCellStyle> result = new Dictionary<int, BuiltInCellStyle>();
			CellStyleCollection cellStyles = Workbook.StyleSheet.CellStyles;
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				BuiltInCellStyle style = cellStyles[i] as BuiltInCellStyle;
				if (style != null) {
					if (!result.ContainsKey(style.BuiltInId))
						result.Add(style.BuiltInId, style);
				}
			}
			return result;
		}
		List<CellStyleBase> GetBuiltInStyles(int builtInId) {
			List<CellStyleBase> result = new List<CellStyleBase>();
			CellStyleCollection cellStyles = Workbook.StyleSheet.CellStyles;
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				BuiltInCellStyle style = cellStyles[i] as BuiltInCellStyle;
				if ((style != null) && (style.BuiltInId == builtInId))
					result.Add(style);
			}
			return result;
		}
		#endregion
		protected internal override void RegisterNumberFormats() {
			int count = Workbook.Cache.NumberFormatCache.Count;
			for (int i = 0; i < count; i++)
				RegisterItem(NumberFormatTable, i);
		}
		#region RegisterFonts
		protected internal override void RegisterFonts() {
			base.RegisterFonts();
			RegisterSharedStringsFonts();
			RegisterCommentsFonts();
			RegisterChartFonts();
			CollectFonts();
		}
		void CollectFonts() {
			RunFontInfoCache fontCache = Workbook.Cache.FontInfoCache;
			FontList.Clear();
			RunFontInfo defaultFont = fontCache[0];
			FontList.Add(defaultFont);
			FontTable.Add(0, FontList.Count - 1);
			RunFontInfo fontInfo = defaultFont.Clone();
			fontInfo.Bold = true;
			FontList.Add(fontInfo);
			fontInfo = defaultFont.Clone();
			fontInfo.Italic = true;
			FontList.Add(fontInfo);
			fontInfo = defaultFont.Clone();
			fontInfo.Bold = true;
			fontInfo.Italic = true;
			FontList.Add(fontInfo);
			foreach (int index in FontInfoTable.Keys) {
				if (FontList.Count >= XlsDefs.MaxFontRecordsCount) break;
				if (index == 0) continue;
				FontList.Add(fontCache[index]);
				FontTable.Add(index, FontList.Count - 1);
			}
		}
		protected void RegisterSharedStringsFonts() {
			SharedStringTable sharedStrings = Workbook.SharedStringTable;
			RunFontInfoCache fontInfoCache = Workbook.Cache.FontInfoCache;
			int count = SharedStringsIndicies.Count;
			for (int i = 0; i < count; i++) {
				int index = SharedStringsIndicies[i];
				FormattedStringItem formattedString = sharedStrings[index] as FormattedStringItem;
				if (formattedString != null) {
					foreach (FormattedStringItemPart part in formattedString.Items) {
						int fontIndex = fontInfoCache.GetItemIndex(part.Font);
						RegisterItem(FontInfoTable, fontIndex);
					}
				}
			}
		}
		protected void RegisterCommentsFonts() {
			RunFontInfoCache fontInfoCache = Workbook.Cache.FontInfoCache;
			for (int i = 0; i < Workbook.Sheets.Count; i++) {
				Worksheet sheet = Workbook.Sheets[i];
				for (int j = 0; j < sheet.Comments.Count; j++) {
					Comment comment = sheet.Comments[j];
					if (comment.Reference.OutOfLimits()) continue;
					CommentRunCollection runs = comment.Runs;
					for (int k = 0; k < runs.Count; k++) {
						int fontIndex = fontInfoCache.GetItemIndex(runs[k].Info);
						RegisterItem(FontInfoTable, fontIndex);
					}
				}
			}
		}
		#region RegisterChartFonts
		void RegisterChartFonts() {
			WorksheetCollection sheetsCollection = Workbook.Sheets;
			int sheetsCount = sheetsCollection.Count;
			for (int i = 0; i < sheetsCount; i++) {
				Worksheet sheet = sheetsCollection[i];
				int drawingObjectsCount = sheet.DrawingObjects.Count;
				for (int j = 0; j < drawingObjectsCount; j++) {
					Chart chart = sheet.DrawingObjects[j] as Chart;
					if (chart != null)
						RegisterChartFonts(chart);
				}
			}
		}
		delegate void RegisterFont(RunFontInfo fontInfo);
		void RegisterChartFonts(Chart chart) {
			RegisterDefaultFonts();
			RegisterFontsFromLegend(chart.Legend);
			RegisterChartFontsFromAxes(chart.PrimaryAxes);
			if (chart.HasSecondaryAxisGroup)
				RegisterChartFontsFromAxes(chart.SecondaryAxes);
			TitleOptions title = chart.Title;
			RegisterFontsFromText(title.Text, title.TextProperties, ChartElement.ChartTitle);
			RegisterFontsFromDataLabels(chart.Views);
		}
		void RegisterDefaultFonts() {
			chartFontTable.Clear();
			chartFontTable.Add(XlsCharacterPropertiesHelper.DefaultChartFont, 1);
			chartFontTable.Add(XlsCharacterPropertiesHelper.DefaultAxisTitleFont, 2);
			chartFontTable.Add(XlsCharacterPropertiesHelper.DefaultChartTitleFont, 3);
		}
		void RegisterFontsFromLegend(Legend legend) {
			if (legend == null)
				return;
			RegisterFontsFromDefaultCharacterProperties(legend.TextProperties.Paragraphs, RegisterChartFont, ChartElement.Legend);
			LegendEntryCollection entriesCollection = legend.Entries;
			int entriesCount = entriesCollection.Count;
			for (int i = 0; i < entriesCount; i++)
				RegisterFontsFromDefaultCharacterProperties(entriesCollection[i].TextProperties.Paragraphs, RegisterChartFont, ChartElement.Legend);
		}
		void RegisterChartFontsFromAxes(AxisGroup axes) {
			ChartElement element = ChartElement.AxisTitle;
			for (int i = 0; i < axes.Count; i++) {
				AxisBase axis = axes[i];
				RegisterFontsFromDisplayUnit(axis as ValueAxis, element);
				TitleOptions title = axis.Title;
				RegisterFontsFromText(title.Text, title.TextProperties, element);
				RegisterFontsFromDefaultCharacterProperties(axis.TextProperties.Paragraphs, RegisterChartFont, ChartElement.Axis);
			}
		}
		void RegisterFontsFromDisplayUnit(ValueAxis valueAxis, ChartElement element) {
			if (valueAxis == null)
				return;
			DisplayUnitOptions displayUnit = valueAxis.DisplayUnit;
			if (displayUnit.UnitType != DisplayUnitType.None && displayUnit.ShowLabel) {
				IChartText text = displayUnit.Text;
				RegisterFontsFromText(text, displayUnit.TextProperties, element);
				if (text.TextType == ChartTextType.Rich)
					RegisterFontsFromRichText(text as ChartRichText, RegisterChartYMultFont, element);
				else
					RegisterFontsFromDefaultCharacterProperties(displayUnit.TextProperties.Paragraphs, RegisterChartYMultFont, element);
			}
		}
		void RegisterFontsFromDataLabels(ChartViewCollection views) {
			int viewCount = views.Count;
			bool isFilledRadar = viewCount > 0 ? IsFilledRadar(views) : false;
			for (int i = 0; i < viewCount; i++) {
				ChartViewWithDataLabels view = views[i] as ChartViewWithDataLabels;
				if (view != null) {
					RegisterFontsFromDataLabels(view.DataLabels, view.ViewType, isFilledRadar);
					int seriesCount = view.Series.Count;
					for (int j = 0; j < seriesCount; j++) {
						SeriesWithDataLabelsAndPoints series = view.Series[j] as SeriesWithDataLabelsAndPoints;
						if (series != null)
							RegisterFontsFromDataLabels(series.DataLabels, series.View.ViewType, isFilledRadar);
					}
				}
			}
		}
		void RegisterFontsFromDataLabels(DataLabels dataLabels, ChartViewType viewType, bool isFilledRadar) {
			int labelsCount = dataLabels.Labels.Count;
			ChartElement element = ChartElement.DataLabels;
			for (int i = 0; i < labelsCount; i++) {
				DataLabel label = dataLabels.Labels[i];
				IChartText text = label.Text;
				RegisterFontsFromText(text, label.TextProperties, element);
				if (IsWrapped(label, viewType, isFilledRadar)) {
					if (text.TextType == ChartTextType.Rich)
						RegisterFontsFromRichText(text as ChartRichText, RegisterChartDataLabExtFont, element);
					else
						RegisterFontsFromDefaultCharacterProperties(label.TextProperties.Paragraphs, RegisterChartDataLabExtFont, element);
				}
			}
			RegisterFontsFromDefaultCharacterProperties(dataLabels.TextProperties.Paragraphs, RegisterChartFont, element);
			if (IsWrapped(dataLabels, viewType, isFilledRadar))
				RegisterFontsFromDefaultCharacterProperties(dataLabels.TextProperties.Paragraphs, RegisterChartDataLabExtFont, element);
		}
		bool IsWrapped(DataLabelBase dataLabel, ChartViewType viewType, bool isFilledRadar) {
			bool options = !dataLabel.ShowValue && !dataLabel.ShowPercent && !dataLabel.ShowBubbleSize;
			if (viewType == ChartViewType.Area || viewType == ChartViewType.Area3D || isFilledRadar)
				return options && !dataLabel.ShowSeriesName && dataLabel.ShowCategoryName;
			return options && dataLabel.ShowSeriesName && !dataLabel.ShowCategoryName;
		}
		bool IsFilledRadar(ChartViewCollection views) {
			RadarChartView radarView = views[0] as RadarChartView;
			if (radarView == null)
				return false;
			return radarView.RadarStyle == RadarChartStyle.Filled;
		}
		void RegisterFontsFromText(IChartText text, TextProperties textProperties, ChartElement element) {
			RegisterFontsFromRichText(text as ChartRichText, RegisterChartFont, element);
			RegisterFontsFromDefaultCharacterProperties(textProperties.Paragraphs, RegisterChartFont, element);
		}
		void RegisterFontsFromRichText(ChartRichText richText, RegisterFont registerFont, ChartElement element) {
			if (richText == null)
				return;
			DrawingTextParagraphCollection paragraphs = richText.Paragraphs;
			int paragraphCount = paragraphs.Count;
			for (int i = 0; i < paragraphCount; i++) {
				DrawingTextParagraph paragraph = paragraphs[i];
				DrawingTextParagraphProperties paragraphProperties = paragraphs[0].ParagraphProperties;
				int runsCount = paragraph.Runs.Count;
				if (runsCount == 0)
					RegisterChartFont(paragraph.EndRunProperties, paragraphProperties, registerFont, element);
				for (int j = 0; j < runsCount; j++) {
					DrawingTextCharacterProperties runProperties = ((DrawingTextRunBase)paragraph.Runs[j]).RunProperties;
					RegisterChartFont(runProperties, paragraphProperties, registerFont, element);
				}
			}
		}
		void RegisterFontsFromDefaultCharacterProperties(DrawingTextParagraphCollection paragraphs, RegisterFont registerFont, ChartElement element) {
			if (paragraphs.Count == 0)
				return;
			DrawingTextCharacterProperties paragraphProperties = paragraphs[0].ParagraphProperties.DefaultCharacterProperties;
			RunFontInfo fontInfo = XlsCharacterPropertiesHelper.GetRunFontInfo(paragraphProperties, element, Workbook.StyleSheet.Palette);
			registerFont(fontInfo);
		}
		void RegisterChartFont(DrawingTextCharacterProperties characterProperties, DrawingTextParagraphProperties paragraphProperties, RegisterFont registerFont, ChartElement element) {
			RunFontInfo fontInfo = XlsCharacterPropertiesHelper.GetRunFontInfo(characterProperties, paragraphProperties, element, Workbook.StyleSheet.Palette);
			registerFont(fontInfo);
		}
		void RegisterChartFont(RunFontInfo fontInfo) {
			if (!chartFontTable.ContainsKey(fontInfo))
				chartFontTable.Add(fontInfo, chartFontTable.Count + 1);
		}
		void RegisterChartYMultFont(RunFontInfo fontInfo) {
			if (!yMultFontList.Contains(fontInfo))
				yMultFontList.Add(fontInfo);
		}
		void RegisterChartDataLabExtFont(RunFontInfo fontInfo) {
			if (!dataLabExtFontList.Contains(fontInfo))
				dataLabExtFontList.Add(fontInfo);
		}
		#endregion
		#endregion
		public int GetXFIndex(CellStyleBase style) {
			int result = XFStyleTable[style];
			if (result >= Math.Min(XlsDefs.MaxStyleXFCount, totalXFCount)) {
				defaultStyleXFUsed = true;
				result = XlsDefs.DefaultStyleXFIndex;
			}
			return result;
		}
		public int GetXFIndex(int index) {
			int result = XFTable[index];
			if (result >= Math.Min(XlsDefs.MaxCellXFCount, totalXFCount)) {
				defaultCellXFUsed = true;
				result = XlsDefs.DefaultCellXFIndex;
			}
			return result;
		}
		public int GetFontIndex(int index) {
			if (FontTable.ContainsKey(index))
				return FontTable[index];
			return 0;
		}
		public void RegisterSheetDefinitions() {
			this.rpnContext = new XlsRPNContext(Workbook.DataContext);
			for (int i = 0; i < Workbook.Sheets.Count; i++) {
				Worksheet sheet = Workbook.Sheets[i];
				this.rpnContext.SheetNames.Add(sheet.Name);
			}
			SheetDefinitionWalkerXls walker = new SheetDefinitionWalkerXls(this.rpnContext);
			RegisterDefinedNames(walker);
			for (int i = 0; i < Workbook.Sheets.Count; i++) {
				Worksheet sheet = Workbook.Sheets[i];
				Workbook.DataContext.PushCurrentWorksheet(sheet);
				try {
					RegisterSheetDefinitions(sheet, walker);
				}
				finally {
					Workbook.DataContext.PopCurrentWorksheet();
				}
			}
		}
		void RegisterDefinedNames(DefinedNameCollection names) {
			foreach (DefinedNameBase definedName in names)
				this.rpnContext.RegisterDefinedName(definedName.Name, definedName.ScopedSheetId);
		}
		void ProcessDefinedNames(DefinedNameCollection names, SheetDefinitionWalkerXls walker) {
			foreach (DefinedNameBase definedName in names) {
				ParsedExpression expression = definedName.Expression;
				if (expression != null) {
					Workbook.DataContext.PushDefinedNameProcessing(definedName);
					try {
						walker.Process(expression);
					}
					finally {
						Workbook.DataContext.PopDefinedNameProcessing();
					}
				}
			}
		}
		protected internal void RegisterDefinedNames(SheetDefinitionWalkerXls walker) {
			RegisterDefinedNames(Workbook.DefinedNames);
			for (int i = 0; i < Workbook.Sheets.Count; i++)
				RegisterDefinedNames(Workbook.Sheets[i].DefinedNames);
			ProcessDefinedNames(Workbook.DefinedNames, walker);
			for (int i = 0; i < Workbook.Sheets.Count; i++) {
				Worksheet sheet = Workbook.Sheets[i];
				Workbook.DataContext.PushCurrentWorksheet(sheet);
				try {
					ProcessDefinedNames(sheet.DefinedNames, walker);
					foreach(ConditionalFormatting cf in sheet.ConditionalFormattings) {
						List<ParsedExpression> expressions = cf.GetExpressions(RPNContext);
						foreach(ParsedExpression expression in expressions)
							walker.Process(expression);
					}
				}
				finally {
					Workbook.DataContext.PopCurrentWorksheet();
				}
			}
		}
		protected internal void RegisterSheetDefinitions(Worksheet sheet, SheetDefinitionWalkerXls walker) {
#if DATA_SHEET
			if (sheet.IsDataSheet)
				return;
#endif
			foreach (SharedFormula formula in sheet.SharedFormulas) {
				walker.Process(formula.Expression);
			}
			CellRange range = GetMaximumCellRange(sheet);
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell == null || !cell.HasFormula) 
					continue;
				FormulaBase cellFormula = cell.GetFormula();
				if (cellFormula is ArrayFormulaPart) continue;
				if (cellFormula is SharedFormulaRef) continue;
				sheet.DataContext.PushCurrentCell(cell);
				try {
					walker.Process(cellFormula.Expression);
				}
				finally {
					sheet.DataContext.PopCurrentCell();
				}
			}
#if EXPORT_CHARTS
			if (Workbook.DocumentCapabilities.ChartsAllowed) {
				foreach (IDrawingObject drawing in sheet.DrawingObjects)
					RegisterSheetDefinitions(drawing as Chart, walker);
			}
#endif
			foreach (DataValidation dataValidation in sheet.DataValidations) {
				if (XlsDataValidationHelper.IsXlsCompliant(dataValidation)) {
					RegisterSheetDefinitions(dataValidation.Expression1, walker);
					RegisterSheetDefinitions(dataValidation.Expression2, walker);
				}
			}
		}
		protected internal void RegisterSheetDefinitions(Table table, SheetDefinitionWalkerXls walker) {
			foreach (TableColumn info in table.Columns) {
				ParsedExpression expression = null;
				if (info.HasColumnFormula) {
					XlsColumnFormulaInfo columnFormulaInfo = XlsColumnFormulaInfo.FromColumnFormula(info.ColumnFormula, walker.RPNContext);
					expression = columnFormulaInfo.ParsedExpression;
				}
				if (info.HasTotalsRowFormula) {
					XlsTotalColumnFormulaInfo totalColumnFormulaInfo = XlsTotalColumnFormulaInfo.FromTotalColumnFormula(info.TotalsRowFormula, walker.RPNContext);
					expression = totalColumnFormulaInfo.ParsedExpression;
				}
				if (expression != null)
					walker.Process(expression);
			}
		}
		protected internal override void RegisterDifferentialFormatIndex(Worksheet sheet) {
		}
		protected internal void RegisterSheetDefinitions(Chart chart, SheetDefinitionWalkerXls walker) {
			if (chart == null)
				return;
			foreach (IChartView view in chart.Views)
				RegisterSheetDefinitions(view, walker);
			RegisterSheetDefinitions(chart.Title.Text as ChartTextRef, walker);
			foreach (AxisBase axis in chart.PrimaryAxes)
				RegisterSheetDefinitions(axis.Title.Text as ChartTextRef, walker);
			foreach (AxisBase axis in chart.SecondaryAxes)
				RegisterSheetDefinitions(axis.Title.Text as ChartTextRef, walker);
		}
		protected internal void RegisterSheetDefinitions(IChartView view, SheetDefinitionWalkerXls walker) {
			foreach (ISeries series in view.Series)
				RegisterSheetDefinitions(series as SeriesBase, walker);
		}
		protected internal void RegisterSheetDefinitions(SeriesBase series, SheetDefinitionWalkerXls walker) {
			RegisterSheetDefinitions(series.Text as ChartTextRef, walker);
			RegisterSheetDefinitions(series.Arguments as ChartDataReference, walker);
			RegisterSheetDefinitions(series.Values as ChartDataReference, walker);
			BubbleSeries bubbleSeries = series as BubbleSeries;
			if (bubbleSeries != null)
				RegisterSheetDefinitions(bubbleSeries.BubbleSize as ChartDataReference, walker);
		}
		protected internal void RegisterSheetDefinitions(ChartTextRef textRef, SheetDefinitionWalkerXls walker) {
			if (textRef != null && textRef.Expression != null)
				walker.Process(textRef.Expression);
		}
		protected internal void RegisterSheetDefinitions(ChartDataReference dataRef, SheetDefinitionWalkerXls walker) {
			if (dataRef != null && dataRef.Expression != null)
				walker.Process(dataRef.Expression);
		}
		protected internal void RegisterSheetDefinitions(ParsedExpression expression, SheetDefinitionWalkerXls walker) {
			if (expression != null && expression.Count > 0)
				walker.Process(expression);
		}
		public void RegisterObjects() {
			ObjectsCount = 0;
			RegisterDrawings();
			RegisterComments();
		}
		void RegisterDrawings() {
			DrawingTable.Clear();
			for (int i = 0; i < Workbook.Sheets.Count; i++)
				RegisterDrawings(Workbook.Sheets[i]);
		}
#if EXPORT_CHARTS
		void RegisterDrawings(Worksheet sheet) {
			List<IDrawingObject> drawings = sheet.DrawingObjects.GetDrawings();
			for (int i = 0; i < drawings.Count; i++) {
				IDrawingObject drawing = drawings[i];
				if (drawing.DrawingType == DrawingObjectType.Picture && sheet.Workbook.DocumentCapabilities.PicturesAllowed) {
					ObjectsCount++;
					Picture picture = drawing as Picture;
					if (!picture.PictureFill.Blip.Embedded) continue;
					OfficeImage image = picture.Image.RootImage;
					if (!DrawingTable.ContainsKey(image))
						DrawingTable.Add(image, new DrawingTableInfo(DrawingTable.Count, 1));
					else {
						DrawingTableInfo item = DrawingTable[image];
						item.RefCount++;
					}
				}
				else if (drawing.DrawingType == DrawingObjectType.Chart && sheet.Workbook.DocumentCapabilities.ChartsAllowed)
					ObjectsCount++;
				else if(drawing.DrawingType == DrawingObjectType.Shape && sheet.Workbook.DocumentCapabilities.ShapesAllowed)
					ObjectsCount++;
			}
		}
#else
		void RegisterDrawings(Worksheet sheet) {
			List<Picture> pictures = sheet.DrawingObjects.GetPictures();
			ObjectsCount += pictures.Count;
			for (int i = 0; i < pictures.Count; i++) {
				Picture picture = pictures[i];
				if (!picture.PictureFill.Blip.Embedded) continue;
				OfficeImage image = picture.Image.RootImage;
				if (!DrawingTable.ContainsKey(image))
					DrawingTable.Add(image, new DrawingTableInfo(DrawingTable.Count, 1));
				else {
					DrawingTableInfo item = DrawingTable[image];
					item.RefCount++;
				}
			}
		}
#endif
		void RegisterComments() {
			for (int i = 0; i < Workbook.Sheets.Count; i++)
				ObjectsCount += Workbook.Sheets[i].Comments.CountInXlsCellRange();
		}
	}
}
