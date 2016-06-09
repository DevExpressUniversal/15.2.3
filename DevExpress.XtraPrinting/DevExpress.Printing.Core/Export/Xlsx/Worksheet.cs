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
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraExport.Implementation;
using DevExpress.Export.Xl;
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		#region Statics
		static readonly Dictionary<XlCommentsPrintMode, string> commentsPrintModeTable = CreateCommentsPrintModeTable();
		static readonly Dictionary<XlErrorsPrintMode, string> errorsPrintModeTable = CreateErrorsPrintModeTable();
		static readonly Dictionary<XlPagePrintOrder, string> pagePrintOrderTable = CreatePagePrintOrderTable();
		static readonly Dictionary<XlPageOrientation, string> pageOrientationTable = CreatePageOrientationTable();
		static Dictionary<XlCommentsPrintMode, string> CreateCommentsPrintModeTable() {
			Dictionary<XlCommentsPrintMode, string> result = new Dictionary<XlCommentsPrintMode, string>();
			result.Add(XlCommentsPrintMode.None, "none");
			result.Add(XlCommentsPrintMode.AtEnd, "atEnd");
			result.Add(XlCommentsPrintMode.AsDisplayed, "asDisplayed");
			return result;
		}
		static Dictionary<XlErrorsPrintMode, string> CreateErrorsPrintModeTable() {
			Dictionary<XlErrorsPrintMode, string> result = new Dictionary<XlErrorsPrintMode, string>();
			result.Add(XlErrorsPrintMode.Displayed, "displayed");
			result.Add(XlErrorsPrintMode.Dash, "dash");
			result.Add(XlErrorsPrintMode.Blank, "blank");
			result.Add(XlErrorsPrintMode.NA, "NA");
			return result;
		}
		static Dictionary<XlPagePrintOrder, string> CreatePagePrintOrderTable() {
			Dictionary<XlPagePrintOrder, string> result = new Dictionary<XlPagePrintOrder, string>();
			result.Add(XlPagePrintOrder.DownThenOver, "downThenOver");
			result.Add(XlPagePrintOrder.OverThenDown, "overThenDown");
			return result;
		}
		static Dictionary<XlPageOrientation, string> CreatePageOrientationTable() {
			Dictionary<XlPageOrientation, string> result = new Dictionary<XlPageOrientation, string>();
			result.Add(XlPageOrientation.Default, "default");
			result.Add(XlPageOrientation.Portrait, "portrait");
			result.Add(XlPageOrientation.Landscape, "landscape");
			return result;
		}
		#endregion
		XlSheet currentSheet = null;
		XlSheet pendingSheet;
		readonly OpenXmlRelationCollection sheetRelations = new OpenXmlRelationCollection();
		bool rowContentStarted = false;
		public IXlSheet BeginSheet() {
			BeginWriteXmlContent();
			WriteShStartElement("worksheet");
			WriteStringAttr("xmlns", XlsxPackageBuilder.RelsPrefix, null, XlsxPackageBuilder.RelsNamespace);
			XlSheet sheet = new XlSheet(this);
			sheet.Name = String.Format("Sheet{0}", sheetIndex + 1);
			this.pendingSheet = sheet;
			this.currentSheet = sheet;
			this.rowIndex = 0;
			this.columnIndex = 0;
			this.groups.Clear();
			this.columns.Clear();
			this.currentGroup = null;
			this.sharedFormulaTable.Clear();
			this.rowContentStarted = false;
			return sheet;
		}
		public void EndSheet() {
			if(currentSheet == null)
				throw new InvalidOperationException("BeginSheet/EndSheet calls consistency.");
			if(!IsUniqueSheetName())
				throw new InvalidOperationException(string.Format("Worksheet name '{0}' is not unique.", currentSheet.Name));
			ExportPendingSheet();
			WriteShEndElement(); 
			GenerateAutoFilterContent(currentSheet);
			GenerateMergedCellsContent(currentSheet);
			GenerateConditionalFormattings(currentSheet.ConditionalFormattings);
			GenerateDataValidations(currentSheet.DataValidations);
			GenerateHyperlinks(currentSheet.Hyperlinks);
			GeneratePrintOptions(currentSheet.PrintOptions);
			GeneratePageMargins(currentSheet.PageMargins);
			GeneratePageSetup(currentSheet.PageSetup);
			GenerateHeaderFooter(currentSheet.HeaderFooter);
			GenerateRowPageBreaks(currentSheet);
			GenerateColumnPageBreaks(currentSheet);
			GenerateIgnoredErrors(currentSheet);
			GenerateDrawingRef();
			GenerateTableParts();
			GenerateExtList(currentSheet);
			WriteShEndElement(); 
			CompressedStream stream = EndWriteXmlContent();
			SheetInfo info = new SheetInfo();
			info.SheetId = ++sheetIndex;
			info.RelationId = Builder.WorkbookRelations.GenerateId();
			Builder.OverriddenContentTypes.Add(String.Format(@"/xl/worksheets/sheet{0}.xml", info.SheetId), XlsxPackageBuilder.WorksheetContentType);
			Builder.WorkbookRelations.Add(new OpenXmlRelation(info.RelationId, String.Format(@"worksheets/sheet{0}.xml", info.SheetId), XlsxPackageBuilder.RelsWorksheetNamespace));
			AddPackageContent(String.Format(@"xl\worksheets\sheet{0}.xml", info.SheetId), stream);
			sheets.Add(currentSheet);
			sheetInfos.Add(currentSheet, info);
			GenerateSheetRelations(info.SheetId);
			GenerateDrawingContent();
			GenerateTablesContent();
			currentSheet = null;
			shapeId = 0;
			drawingRelations.Clear();
			drawingObjects.Clear();
			sheetRelations.Clear();
			exportedPictureHyperlinkTable.Clear();
			this.columns.Clear();
		}
		void ExportPendingSheet() {
			if (pendingSheet == null)
				return;
			GenerateSheetProperties(pendingSheet);
			GenerateSheetViews(pendingSheet);
			GenerateSheetFormatProperties(pendingSheet);
			ExportPendingColumns();
			WriteShStartElement("sheetData");
			pendingSheet = null;
		}
		#region Sheet Properties
		bool ShouldExportSheetProperties(IXlSheet sheet) {
			if(!sheet.OutlineProperties.SummaryBelow || !sheet.OutlineProperties.SummaryRight)
				return true;
			return sheet.PageSetup != null && sheet.PageSetup.FitToPage;
		}
		void GenerateSheetProperties(IXlSheet sheet) {
			if(!ShouldExportSheetProperties(sheet))
				return;
			WriteShStartElement("sheetPr");
			try {
				GenerateOutlineProperties(sheet);
				GeneratePageSetupProperties(sheet);
			}
			finally {
				WriteShEndElement(); 
			}
		}
		void GenerateOutlineProperties(IXlSheet sheet) {
			if(sheet.OutlineProperties.SummaryBelow && sheet.OutlineProperties.SummaryRight)
				return;
			WriteShStartElement("outlinePr");
			try {
				if(!pendingSheet.OutlineProperties.SummaryBelow)
					WriteShBoolValue("summaryBelow", false);
				if(!pendingSheet.OutlineProperties.SummaryRight)
					WriteShBoolValue("summaryRight", false);
			}
			finally {
				WriteShEndElement(); 
			}
		}
		void GeneratePageSetupProperties(IXlSheet sheet) {
			if(sheet.PageSetup == null || !sheet.PageSetup.FitToPage)
				return;
			WriteShStartElement("pageSetUpPr");
			try {
				WriteShBoolValue("fitToPage", true);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateSheetFormatProperties(IXlSheet sheet) {
			int outlineLevelCol = 0;
			foreach(XlColumn column in pendingColumns)
				outlineLevelCol = Math.Max(outlineLevelCol, column.OutlineLevel);
			if(outlineLevelCol == 0)
				return;
			WriteShStartElement("sheetFormatPr");
			try {
				WriteStringValue("defaultRowHeight", "15");
				WriteShIntValue("outlineLevelCol", outlineLevelCol);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Sheet Views
		void GenerateSheetViews(IXlSheet sheet) {
			WriteShStartElement("sheetViews");
			try {
				GenerateSheetView(sheet);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetView(IXlSheet sheet) {
			WriteShStartElement("sheetView");
			try {
				WriteIntValue("workbookViewId", 0);
				IXlSheetViewOptions view = sheet.ViewOptions;
				if(view.ShowFormulas)
					WriteBoolValue("showFormulas", view.ShowFormulas);
				if(!view.ShowGridLines)
					WriteBoolValue("showGridLines", view.ShowGridLines);
				if(!view.ShowRowColumnHeaders)
					WriteBoolValue("showRowColHeaders", view.ShowRowColumnHeaders);
				if(!view.ShowZeroValues)
					WriteBoolValue("showZeros", view.ShowZeroValues);
				if(view.RightToLeft)
					WriteBoolValue("rightToLeft", view.RightToLeft);
				if(!view.ShowOutlineSymbols)
					WriteBoolValue("showOutlineSymbols", view.ShowOutlineSymbols);
				if (ShouldGenerateSheetViewPane(sheet))
					GenerateSheetViewPane(sheet);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldGenerateSheetViewPane(IXlSheet sheet) {
			return !sheet.SplitPosition.EqualsPosition(XlCellPosition.TopLeft);
		}
		void GenerateSheetViewPane(IXlSheet sheet) {
			WriteShStartElement("pane");
			try {
				if (!sheet.SplitPosition.EqualsPosition(XlCellPosition.TopLeft))
					WriteStringValue("topLeftCell", sheet.SplitPosition.ToString());
				if (sheet.SplitPosition.Column != XlCellPosition.TopLeft.Column)
					WriteIntValue("xSplit", sheet.SplitPosition.Column);
				if (sheet.SplitPosition.Row != XlCellPosition.TopLeft.Row)
					WriteIntValue("ySplit", sheet.SplitPosition.Row);
				WriteStringValue("activePane", "bottomRight");
				WriteStringValue("state", "frozen");
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region AutoFilter
		void GenerateAutoFilterContent(IXlSheet sheet) {
			if (sheet.AutoFilterRange == null)
				return;
			WriteShStartElement("autoFilter");
			try {
				WriteStringValue("ref", sheet.AutoFilterRange.ToString());
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Merged Cells
		protected internal virtual void GenerateMergedCellsContent(IXlSheet sheet) {
			int count = sheet.MergedCells.Count;
			if (count <= 0)
				return;
			WriteShStartElement("mergeCells");
			try {
				WriteIntValue("count", count);
				foreach (XlCellRange mergedCell in sheet.MergedCells)
					ExportMergedCell(mergedCell);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportMergedCell(XlCellRange mergedCell) {
			WriteShStartElement("mergeCell");
			try {
				WriteStringValue("ref", mergedCell.ToString(true));
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region PrintOptions
		void GeneratePrintOptions(XlPrintOptions options) {
			if(options == null || options.IsDefault())
				return;
			WriteShStartElement("printOptions");
			try {
				if(options.HorizontalCentered)
					WriteBoolValue("horizontalCentered", options.HorizontalCentered);
				if(options.VerticalCentered)
					WriteBoolValue("verticalCentered", options.VerticalCentered);
				if(options.Headings)
					WriteBoolValue("headings", options.Headings);
				if(options.GridLines)
					WriteBoolValue("gridLines", options.GridLines);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region PageBreaks
		void GenerateColumnPageBreaks(IXlSheet sheet) {
			GeneratePageBreaksContent(sheet.ColumnPageBreaks, "colBreaks");
		}
		void GenerateRowPageBreaks(IXlSheet sheet) {
			GeneratePageBreaksContent(sheet.RowPageBreaks, "rowBreaks");
		}
		void GeneratePageBreaksContent(IXlPageBreaks breaks, string tagName) {
			if(breaks.Count <= 0)
				return;
			WriteShStartElement(tagName);
			try {
				WriteIntValue("count", breaks.Count);
				WriteIntValue("manualBreakCount", breaks.Count);
				foreach(int position in breaks)
					ExportPageBreak(position);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportPageBreak(int index) {
			WriteShStartElement("brk");
			try {
				WriteIntValue("id", index);
				WriteBoolValue("man", true);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region PageMargins
		void GeneratePageMargins(XlPageMargins margins) {
			if(margins == null)
				return;
			WriteShStartElement("pageMargins");
			try {
				WriteMarginValue(margins.LeftInches, "left");
				WriteMarginValue(margins.RightInches, "right");
				WriteMarginValue(margins.TopInches, "top");
				WriteMarginValue(margins.BottomInches, "bottom");
				WriteMarginValue(margins.HeaderInches, "header");
				WriteMarginValue(margins.FooterInches, "footer");
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteMarginValue(double value, string name) {
			WriteStringValue(name, value.ToString(CultureInfo.InvariantCulture));
		}
		#endregion
		#region PageSetup
		void GeneratePageSetup(XlPageSetup pageSetup) {
			if(pageSetup == null || pageSetup.IsDefault())
				return;
			WriteShStartElement("pageSetup");
			try {
				if(pageSetup.PaperKind != PaperKind.Letter)
					WriteIntValue("paperSize", (int)pageSetup.PaperKind);
				if(pageSetup.CommentsPrintMode != XlCommentsPrintMode.None)
					WriteStringValue("cellComments", commentsPrintModeTable[pageSetup.CommentsPrintMode]);
				if(pageSetup.ErrorsPrintMode != XlErrorsPrintMode.Displayed)
					WriteStringValue("errors", errorsPrintModeTable[pageSetup.ErrorsPrintMode]);
				if(pageSetup.PagePrintOrder != XlPagePrintOrder.DownThenOver)
					WriteStringValue("pageOrder", pagePrintOrderTable[pageSetup.PagePrintOrder]);
				if(pageSetup.PageOrientation != XlPageOrientation.Default)
					WriteStringValue("orientation", pageOrientationTable[pageSetup.PageOrientation]);
				if(pageSetup.Scale != 100)
					WriteIntValue("scale", pageSetup.Scale);
				if(pageSetup.BlackAndWhite)
					WriteBoolValue("blackAndWhite", pageSetup.BlackAndWhite);
				if(pageSetup.Draft)
					WriteBoolValue("draft", pageSetup.Draft);
				if(!pageSetup.AutomaticFirstPageNumber)
					WriteBoolValue("useFirstPageNumber", true);
				if(!pageSetup.UsePrinterDefaults)
					WriteBoolValue("usePrinterDefaults", false);
				if(pageSetup.Copies != 1)
					WriteIntValue("copies", pageSetup.Copies);
				if(pageSetup.FirstPageNumber != 1)
					WriteIntValue("firstPageNumber", pageSetup.FirstPageNumber);
				if(pageSetup.FitToWidth != 1)
					WriteIntValue("fitToWidth", pageSetup.FitToWidth);
				if(pageSetup.FitToHeight != 1)
					WriteIntValue("fitToHeight", pageSetup.FitToHeight);
				if(pageSetup.HorizontalDpi != 600)
					WriteIntValue("horizontalDpi", pageSetup.HorizontalDpi);
				if(pageSetup.VerticalDpi != 600)
					WriteIntValue("verticalDpi", pageSetup.VerticalDpi);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region HeaderFooter
		void GenerateHeaderFooter(XlHeaderFooter options) {
			if(options.IsDefault())
				return;
			WriteShStartElement("headerFooter");
			try {
				if(!options.AlignWithMargins)
					WriteBoolValue("alignWithMargins", options.AlignWithMargins);
				if(options.DifferentFirst)
					WriteBoolValue("differentFirst", options.DifferentFirst);
				if(options.DifferentOddEven)
					WriteBoolValue("differentOddEven", options.DifferentOddEven);
				if(!options.ScaleWithDoc)
					WriteBoolValue("scaleWithDoc", options.ScaleWithDoc);
				WriteHeaderFooterItem("oddHeader", options.OddHeader);
				WriteHeaderFooterItem("oddFooter", options.OddFooter);
				WriteHeaderFooterItem("evenHeader", options.EvenHeader);
				WriteHeaderFooterItem("evenFooter", options.EvenFooter);
				WriteHeaderFooterItem("firstHeader", options.FirstHeader);
				WriteHeaderFooterItem("firstFooter", options.FirstFooter);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteHeaderFooterItem(string tag, string value) {
			if(string.IsNullOrEmpty(value))
				return;
			WriteShString(tag, EncodeXmlChars(value), true);
		}
		#endregion
		#region GenerateSheetRelations
		void GenerateSheetRelations(int sheetId) {
			if(sheetRelations.Count == 0)
				return;
			string fileName = string.Format(@"xl\worksheets\_rels\sheet{0}.xml.rels", sheetId);
			BeginWriteXmlContent();
			Builder.GenerateRelationsContent(writer, sheetRelations);
			AddPackageContent(fileName, EndWriteXmlContent());
		}
		#endregion
		#region GenerateIgnoredErrors
		void GenerateIgnoredErrors(IXlSheet sheet) {
			if(sheet.IgnoreErrors == XlIgnoreErrors.None)
				return;
			XlCellRange dataRange = sheet.DataRange;
			if(dataRange == null)
				return;
			WriteShStartElement("ignoredErrors");
			try {
				WriteShStartElement("ignoredError");
				try {
					WriteStringValue("sqref", dataRange.ToString());
					if((sheet.IgnoreErrors & XlIgnoreErrors.CalculatedColumn) != 0)
						WriteBoolValue("calculatedColumn", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.EmptyCellReference) != 0)
						WriteBoolValue("emptyCellReference", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.EvaluationError) != 0)
						WriteBoolValue("evalError", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.Formula) != 0)
						WriteBoolValue("formula", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.FormulaRange) != 0)
						WriteBoolValue("formulaRange", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.ListDataValidation) != 0)
						WriteBoolValue("listDataValidation", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.NumberStoredAsText) != 0)
						WriteBoolValue("numberStoredAsText", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.TwoDigitTextYear) != 0)
						WriteBoolValue("twoDigitTextYear", true);
					if((sheet.IgnoreErrors & XlIgnoreErrors.UnlockedFormula) != 0)
						WriteBoolValue("unlockedFormula", true);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		bool IsUniqueSheetName() {
			foreach(IXlSheet sheet in sheets)
				if(string.Equals(currentSheet.Name, sheet.Name, StringComparison.OrdinalIgnoreCase))
					return false;
			return true;
		}
	}
	#region SheetInfo
	public class SheetInfo {
		public string RelationId { get; set; }
		public int SheetId { get; set; }
	}
	#endregion
}
