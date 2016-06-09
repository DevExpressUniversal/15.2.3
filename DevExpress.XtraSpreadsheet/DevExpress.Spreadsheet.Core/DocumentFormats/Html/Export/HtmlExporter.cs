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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.Office.Export.Html;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Localization;
using System.Globalization;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Html {
	#region CssPropertiesExportType
	[ComVisible(true)]
	public enum CssPropertiesExportType {
		Style = 0,
		Link = 1,
		Inline = 2
	}
	#endregion
	#region HtmlFontUnit
	[ComVisible(true)]
	public enum HtmlFontUnit {
		Point,
		Pixel
	}
	#endregion
	#region ExportRootTag
	[ComVisible(true)]
	public enum ExportRootTag {
		Html = 0,
		Body = 1
	}
	#endregion
	#region UriExportType
	[ComVisible(true)]
	public enum UriExportType {
		Relative,
		Absolute
	}
	#endregion
	#region HtmlExporter
	public class HtmlExporter : HtmlExporterBase, IDocumentModelSkinColorProvider {
		#region Fields
		readonly HtmlDocumentExporterOptions options;
		readonly DocumentModel documentModel;
		readonly Stack<DXWebControlBase> controlStack;
		static readonly Dictionary<XlHorizontalAlignment, string> alignHT = CreateHorizontalAlignTable();
		Worksheet sheet;
		CellRange range;
		float defaultRowHeightInPixels;
		readonly Dictionary<int, float> columnWidthsInPixels;
		readonly Dictionary<int, int> columnWidths;
		readonly IColumnWidthCalculationService measureService;
		readonly ICellErrorTextProvider errorTextProvider = new DefaultCellErrorTextProvider();
		readonly HtmlStyleBuilder styleBuilder;
		readonly OfficeHtmlImageHelper imageHelper;
		#endregion
		public HtmlExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.options = options;
			this.controlStack = new Stack<DXWebControlBase>();
			this.columnWidthsInPixels = new Dictionary<int, float>();
			this.columnWidths = new Dictionary<int, int>();
			this.measureService = documentModel.GetService<IColumnWidthCalculationService>();
			Initialize(options.TargetUri, options.UriExportType == UriExportType.Absolute);
			this.styleBuilder = CreateHtmlStyleBuilder();
			this.imageHelper = new OfficeHtmlImageHelper(documentModel, documentModel.UnitConverter);
			if (this.options.OverrideImageResolution != DocumentModel.Dpi) {
				this.imageHelper.HorizontalResolution = this.options.OverrideImageResolution;
				this.imageHelper.VerticalResolution = this.options.OverrideImageResolution;
			}
		}
		#region Properties
		protected override bool EmbedImages { get { return options.EmbedImages; } }
		protected override bool ExportToBodyTag { get { return options.ExportRootTag == ExportRootTag.Body; } }
		protected override bool ExportStylesAsStyleTag { get { return options.CssPropertiesExportType == CssPropertiesExportType.Style; } }
		protected override bool ExportStylesAsLink { get { return options.CssPropertiesExportType == CssPropertiesExportType.Link; } }
		protected override Encoding Encoding { get { return options.Encoding; } }
		protected override bool UseHtml5 { get { return true; } }
		internal DXWebControlBase CurrentParent { get { return controlStack.Peek(); } }
		protected internal bool IsExportInlineStyle { get { return options.CssPropertiesExportType == CssPropertiesExportType.Inline; } }
		protected internal virtual bool DisposeConvertedImagesImmediately { get { return options.DisposeConvertedImagesImmediately; } }
		Color IDocumentModelSkinColorProvider.SkinGridlineColor { get { return DXColor.Empty; } }
		Color IDocumentModelSkinColorProvider.SkinForeColor { get { return DXColor.Black; } }
		Color IDocumentModelSkinColorProvider.SkinBackColor { get { return DXColor.Transparent; } }
		#endregion
		static Dictionary<XlHorizontalAlignment, string> CreateHorizontalAlignTable() {
			Dictionary<XlHorizontalAlignment, string> table = new Dictionary<XlHorizontalAlignment, string>();
			table.Add(XlHorizontalAlignment.General, "left");
			table.Add(XlHorizontalAlignment.Left, "left");
			table.Add(XlHorizontalAlignment.Center, "center");
			table.Add(XlHorizontalAlignment.CenterContinuous, "center");
			table.Add(XlHorizontalAlignment.Right, "right");
			table.Add(XlHorizontalAlignment.Justify, "justify");
			table.Add(XlHorizontalAlignment.Distributed, "left");
			table.Add(XlHorizontalAlignment.Fill, "left");
			return table;
		}
		protected internal virtual void PushControl(DXWebControlBase parent) {
			controlStack.Push(parent);
		}
		protected internal virtual DXWebControlBase PopControl() {
			return controlStack.Pop();
		}
		protected internal virtual void AddControlToChild(DXWebControlBase parent, DXWebControlBase control) {
			parent.Controls.Add(control);
		}
		protected override void ExportBodyContent(DXWebControlBase root) {
			if (measureService == null)
				return;
			this.sheet = documentModel.Sheets[Math.Min(Math.Max(0, options.SheetIndex), documentModel.SheetCount - 1)];
			CellRange printableRange = CalculateRange(this.sheet);
			this.range = printableRange;
			this.defaultRowHeightInPixels = documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(measureService.CalculateDefaultRowHeight(sheet), DocumentModel.DpiY);
			EmptyWebControl clipboardFragmentComment = (options.AddClipboardHtmlFragmentTags) ? new DXHtmlClipboardFragment() : new EmptyWebControl();
			AddControlToChild(root, clipboardFragmentComment);
			PushControl(root);
			PushControl(clipboardFragmentComment);
			try {
				ExportTopLevelTable();
			}
			finally {
				PopControl();
				PopControl();
			}
		}
		CellRange CalculateRange(Worksheet worksheet) {
			CellRange range = worksheet.GetPrintRange();
			System.Diagnostics.Debug.Assert(range.TopLeft.Row == 0);
			System.Diagnostics.Debug.Assert(range.TopLeft.Column == 0);
			if (String.IsNullOrEmpty(options.Range))
				return range;
			var parser = new Internal.OptionsRangeParser();
			CellRange calculatedRange = parser.CalculateOptionsRange(worksheet, options.Range);
			if (calculatedRange == null)
				return range;
			return calculatedRange;
		}
		void ExportTopLevelTable() {
			DXHtmlTable table = new DXHtmlTable();
			AddControlToChild(CurrentParent, table);
			PushControl(table);
			try {
				float totalTableWidthInPixels = Math.Min(32767, ExportColumns());
				if(options.UseCssForWidthAndHeight)
					table.Style.Add("width", new DXWebUnit(totalTableWidthInPixels, DXWebUnitType.Pixel).ToString());
				else
					table.Attributes.Add("width", new DXWebUnit(totalTableWidthInPixels, DXWebUnitType.Pixel).ToString());
				table.Style.Add("table-layout", "fixed");
				table.Style.Add("border-spacing", "0");
				table.Style.Add("border-collapse", "collapse");
				ExportRows(table);
			}
			finally {
				PopControl();
			}
		}
		float ExportColumns() {
			int totalTableWidth = 0;
			if (options.UseColumnGroupTag) {
				DXHtmlGenericControl tableColumnGroup = new DXHtmlGenericControl(DXHtmlTextWriterTag.Colgroup);
				AddControlToChild(CurrentParent, tableColumnGroup);
				PushControl(tableColumnGroup);
				try {
					int leftColumn = this.range.TopLeft.Column;
					int rightColumn = this.range.BottomRight.Column;
					for (int i = leftColumn; i <= rightColumn; i++) {
						int width = measureService.CalculateColumnWidthTmp(sheet, i);
						totalTableWidth += width;
						float widthInPixels = documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(width, DocumentModel.DpiX);
						this.columnWidths.Add(i, width);
						this.columnWidthsInPixels.Add(i, widthInPixels);
						if (widthInPixels > 0) {
							DXHtmlGenericControl tableColumn = new DXHtmlGenericControl(DXHtmlTextWriterTag.Col);
							if(options.UseCssForWidthAndHeight)
								tableColumn.Style.Add("width", new DXWebUnit(widthInPixels, DXWebUnitType.Pixel).ToString());
							else
								tableColumn.Attributes.Add("width", new DXWebUnit(widthInPixels, DXWebUnitType.Pixel).ToString());
							AddControlToChild(CurrentParent, tableColumn);
						}
					}
				}
				finally {
					PopControl();
				}
			}
			else {
				int leftColumn = this.range.TopLeft.Column;
				int rightColumn = this.range.BottomRight.Column;
				for (int i = leftColumn; i <= rightColumn; i++) {
					int width = measureService.CalculateColumnWidthTmp(sheet, i);
					totalTableWidth += width;
					float widthInPixels = documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(width, DocumentModel.DpiX);
					this.columnWidths.Add(i, width);
					this.columnWidthsInPixels.Add(i, widthInPixels);
				}
			}
			return documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(totalTableWidth, DocumentModel.DpiX);
		}
		void ExportRows(DXHtmlTable table) {
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			for (int i = topRow; i <= bottomRow; i++) {
				DXHtmlTableRow row = ExportRow(i);
				if(row != null)
					table.Rows.Add(row);
			}
		}
		DXHtmlTableRow ExportRow(int rowIndex) {
			Row row = sheet.Rows.TryGetRow(rowIndex);
			if (row != null && row.IsHidden)
				return null;
			DXHtmlTableRow tableRow = new DXHtmlTableRow();
			PushControl(tableRow);
			try {
				CellRange rowRange = new CellRange(sheet, new CellPosition(range.TopLeft.Column, rowIndex), new CellPosition(range.BottomRight.Column, rowIndex));
				CellRangeLayoutVisibleCellsEnumerator cellEnumerator = new CellRangeLayoutVisibleCellsEnumerator(sheet, rowRange.TopLeft, rowRange.BottomRight, false);
				IEnumerable<ICellBase> cells = new Enumerable<ICellBase>(cellEnumerator);
				if (row == null || row.CellsCount <= 0)
					ExportEmptyRowContent(tableRow, rowIndex, rowRange, cells);
				else
					ExportRowContent(tableRow, row, rowRange, cells);
				return tableRow;
			}
			finally {
				PopControl();
			}
		}
		void ExportRowContent(DXHtmlTableRow tableRow, Row row, CellRange rowRange, IEnumerable<ICellBase> cells) {
			int rowHeight = measureService.CalculateRowHeight(sheet, row.Index);
			float rowHeightInPixels = documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(rowHeight, DocumentModel.DpiY);
			if(options.UseCssForWidthAndHeight)
				tableRow.Style.Add("height", new DXWebUnit(rowHeightInPixels, DXWebUnitType.Pixel).ToString());
			else
				tableRow.Attributes.Add("height", new DXWebUnit(rowHeightInPixels, DXWebUnitType.Pixel).ToString());
			ExportRowCellsCore(tableRow, rowRange, cells);
		}
		void ExportRowCellsCore(DXHtmlTableRow tableRow, CellRange rowRange, IEnumerable<ICellBase> cells) {
			CellRangesCachedRTree mergedCells = new CellRangesCachedRTree();
			mergedCells.InsertRange(sheet.MergedCells.GetMergedCellRangesIntersectsRange(rowRange));
			int firstExportedCellIndex = range.TopLeft.Column;
			int lastExportedCellIndex = firstExportedCellIndex - 1;
			int rowIndex = rowRange.TopLeft.Row;
			foreach (ICellBase cellInfo in cells) {
				ICell cell = (ICell)cellInfo;
				if (cell == null)
					continue;
				if (cell.ColumnIndex > lastExportedCellIndex + 1)
					ExportEmptyCells(tableRow, rowIndex, lastExportedCellIndex + 1, cell.ColumnIndex - 1, mergedCells);
				lastExportedCellIndex = ExportCellCore(tableRow, cell, mergedCells, ExportExistingCell);
			}
			int lastCellIndex = range.BottomRight.Column;
			if (lastCellIndex > lastExportedCellIndex)
				ExportEmptyCells(tableRow, rowIndex, lastExportedCellIndex + 1, lastCellIndex, mergedCells);
		}
		delegate DXHtmlTableCell ExportCellDelegate(ICell cell, CellRange mergedCellRange);
		int ExportCellCore(DXHtmlTableRow tableRow, ICell cell, CellRangesCachedRTree mergedCells, ExportCellDelegate exportCellAction) {
			int lastExportedCellIndex = cell.ColumnIndex;
			CellRange mergedCellRange = mergedCells.Search(cell.ColumnIndex, cell.RowIndex);
			if (range != null && mergedCellRange != null)
				mergedCellRange = CropMergedCellRangeToRange(mergedCellRange);
			if (mergedCellRange == null || (mergedCellRange.TopLeft.Column == cell.ColumnIndex && mergedCellRange.TopLeft.Row == cell.RowIndex)) {
				DXHtmlTableCell tableCell = exportCellAction(cell, mergedCellRange);
				if (tableCell != null) {
					if (mergedCellRange != null) {
						if (mergedCellRange.Width != 1) {
							lastExportedCellIndex = cell.ColumnIndex + mergedCellRange.Width - 1;
						}
					}
					tableRow.Cells.Add(tableCell);
				}
			}
			return lastExportedCellIndex;
		}
		void ExportMergedCellsBorders(DXHtmlTableCell tableCell, ICell cell, CellRange mergedCellRange) {
			if (mergedCellRange == null)
				return;
			ICell bottomRightCell = cell.Worksheet.GetCellForFormatting(mergedCellRange.BottomRight.Column, mergedCellRange.BottomRight.Row);
			IActualBorderInfo actualBottomRightBorder = bottomRightCell.ActualBorder;
			int visibleColumnCount = GetVisibleColumnCount(mergedCellRange);
			if (visibleColumnCount > 1) {
				tableCell.ColSpan = visibleColumnCount;
				ExportBorder("right", actualBottomRightBorder.RightLineStyle, actualBottomRightBorder.RightColor);
			}
			int visibleRowCount = GetVisibleRowCount(mergedCellRange);
			if (visibleRowCount > 1) {
				tableCell.RowSpan = visibleRowCount;
				ExportBorder("bottom", actualBottomRightBorder.BottomLineStyle, actualBottomRightBorder.BottomColor);
			}
		}
		int GetVisibleRowCount(CellRange range) {
			int result = 0;
			for (int i = range.TopRowIndex; i <= range.BottomRowIndex; i++) {
				Row row = this.sheet.Rows.TryGetRow(i);
				if(row != null && row.IsHidden)
					continue;
				result++;
			}
			return result;
		}
		int GetVisibleColumnCount(CellRange range) {
			int result = 0;
			for (int i = range.LeftColumnIndex; i <= range.RightColumnIndex; i++) {
				Column column = this.sheet.Columns.TryGetColumn(i);
				if (column != null && column.IsHidden)
					continue;
				result++;
			}
			return result;
		}
		CellRange CropMergedCellRangeToRange(CellRange mergedCellRange) {
			VariantValue result = mergedCellRange.IntersectionWith(range);
			return result.CellRangeValue as CellRange;
		}
		void ExportEmptyCells(DXHtmlTableRow tableRow, int rowIndex, int firstColumnIndex, int lastColumnIndex, CellRangesCachedRTree mergedCells) {
			for (int i = firstColumnIndex; i <= lastColumnIndex; i++) {
				ICell cell = new FakeCell(new CellPosition(i, rowIndex), sheet);
				ExportCellCore(tableRow, cell, mergedCells, ExportEmptyCell);
			}
		}
		DXHtmlTableCell ExportEmptyCell(ICell cell, CellRange mergedCellRange) {
			float width = columnWidthsInPixels[cell.ColumnIndex];
			if (width <= 0)
				return null;
			DXHtmlTableCell tableCell = new DXHtmlTableCell();
			if (!options.UseColumnGroupTag) {
				if (options.UseCssForWidthAndHeight)
					tableCell.Style.Add("width", new DXWebUnit(width, DXWebUnitType.Pixel).ToString());
				else
					tableCell.Attributes.Add("width", new DXWebUnit(width, DXWebUnitType.Pixel).ToString());
			}
			styleBuilder.Begin(tableCell);
			try {
				if (!ExportPictures(tableCell, cell)) {
					DXHtmlLiteralControl literal = new DXHtmlLiteralControl();
					literal.Text = "&nbsp;";
					AddControlToChild(tableCell, literal);
				}
				ExportMergedCellsBorders(tableCell, cell, mergedCellRange);
			}
			finally {
				styleBuilder.End(tableCell);
			}
			return tableCell;
		}
		void ExportEmptyRowContent(DXHtmlTableRow tableRow, int rowIndex, CellRange rowRange, IEnumerable<ICellBase> cells) {
			if(options.UseCssForWidthAndHeight)
				tableRow.Style.Add("height", new DXWebUnit(defaultRowHeightInPixels, DXWebUnitType.Pixel).ToString());
			else
				tableRow.Attributes.Add("height", new DXWebUnit(defaultRowHeightInPixels, DXWebUnitType.Pixel).ToString());
			ExportRowCellsCore(tableRow, rowRange, cells);
		}
		DXHtmlTableCell ExportExistingCell(ICell cell, CellRange mergedCellRange) {
			DXHtmlTableCell tableCell = new DXHtmlTableCell();
			int width = CalculateCellWidth(cell, mergedCellRange);
			if (width <= 0)
				return null;
			if (!options.UseColumnGroupTag) {
				if(options.UseCssForWidthAndHeight)
					tableCell.Style.Add("width", new DXWebUnit(width, DXWebUnitType.Pixel).ToString());
				else
					tableCell.Attributes.Add("width", new DXWebUnit(width, DXWebUnitType.Pixel).ToString());
			}
			Rectangle bounds = new Rectangle(0, 0, width, Int32.MaxValue);
			CellForegroundDisplayFormat displayFormat = CellTextBoxBase.CalculateForegroundDisplayFormat(cell, bounds, errorTextProvider, true, this, 1.0f);
			HtmlExporterTextProperties textProperties;
			textProperties = new HtmlExporterTextProperties(displayFormat.FontInfo,
				displayFormat.ForeColor,
				cell.ActualBackgroundColor,
				displayFormat.FontInfo.Underline,
				displayFormat.FontInfo.Font.Strikeout,
				false,
				(int)Math.Round(displayFormat.FontInfo.SizeInPoints));
			XlHorizontalAlignment actualHorizontalAlignment = cell.ActualHorizontalAlignment;
			IActualCellAlignmentInfo alignment = cell.ActualAlignment;
			styleBuilder.Begin(tableCell);
			try {
				SetTextProperties(tableCell, textProperties, actualHorizontalAlignment, alignment);
				ExportBorders(cell.ActualBorder);
				ExportPictures(tableCell, cell);
				ExportMergedCellsBorders(tableCell, cell, mergedCellRange);
			}
			finally {
				styleBuilder.End(tableCell);
			}
			ModelHyperlink hyperlink = GetHyperlink(cell);
			if (hyperlink != null && hyperlink.IsExternal) {
				DXHtmlAnchor anchor = new DXHtmlAnchor();
				anchor.HRef = HyperlinkUriHelper.ConvertToUrl(hyperlink.TargetUri);
				if (!string.IsNullOrEmpty(hyperlink.TooltipText))
					anchor.ToolTip = hyperlink.TooltipText;
				AddControlToChild(tableCell, anchor);
				ExportExistingCellContent(anchor, displayFormat, actualHorizontalAlignment, alignment);
			}
			else
				ExportExistingCellContent(tableCell, displayFormat, actualHorizontalAlignment, alignment);
			return tableCell;
		}
		ModelHyperlink GetHyperlink(ICell cell) {
			int index = sheet.Hyperlinks.GetHyperlink(cell);
			if (index >= 0)
				return sheet.Hyperlinks[index];
			return null;
		}
		void ExportExistingCellContent(DXHtmlContainerControl container, CellForegroundDisplayFormat displayFormat, XlHorizontalAlignment actualHorizontalAlignment, IActualCellAlignmentInfo alignment) {
			DXHtmlLiteralControl literal = new DXHtmlLiteralControl();
			literal.Text = displayFormat.Text.Replace("\n", "<br>");
			byte indent = alignment.Indent;
			if (indent > 0 && (actualHorizontalAlignment == XlHorizontalAlignment.Left || actualHorizontalAlignment == XlHorizontalAlignment.Right)) {
				int indentInLayoutUnits = CellTextBoxBase.CalculateIndentValue(documentModel, indent, false);
				string paddingValue = new DXWebUnit(documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(indentInLayoutUnits, DocumentModel.DpiX)).ToString();
				if (options.UseSpanTagForIndentation) {
					DXHtmlGenericControl span = new DXHtmlGenericControl(DXHtmlTextWriterTag.Span);
					if (actualHorizontalAlignment == XlHorizontalAlignment.Right)
						span.Style.Add(DXHtmlTextWriterStyle.PaddingRight, paddingValue);
					else
						span.Style.Add(DXHtmlTextWriterStyle.PaddingLeft, paddingValue);
					AddControlToChild(span, literal);
					AddControlToChild(container, span);
				}
				else {
					if (actualHorizontalAlignment == XlHorizontalAlignment.Right)
						container.Style.Add(DXHtmlTextWriterStyle.PaddingRight, paddingValue);
					else
						container.Style.Add(DXHtmlTextWriterStyle.PaddingLeft, paddingValue);
					AddControlToChild(container, literal);
				}
			}
			else
				AddControlToChild(container, literal);
		}
		bool ExportPictures(DXHtmlTableCell tableCell, ICell cell) {
			if (!options.ExportImages)
				return false;
			List<IDrawingObject> drawings = sheet.DrawingObjects.GetDrawingsHostedInCell(cell);
			if (drawings.Count <= 0)
				return false;
			styleBuilder.AppendStyle(DXHtmlTextWriterStyle.Padding, "0");
			styleBuilder.AppendStyle(DXHtmlTextWriterStyle.VerticalAlign, "top");
			DXHtmlDivision divControl = new DXHtmlDivision();
			divControl.Style.Add(DXHtmlTextWriterStyle.Position, "relative");
			divControl.Style.Add(DXHtmlTextWriterStyle.Height, "0px");
			divControl.Style.Add(DXHtmlTextWriterStyle.Width, "0px");
			divControl.Style.Add(DXHtmlTextWriterStyle.Top, "-1px");
			AddControlToChild(tableCell, divControl);
			PushControl(divControl);
			try {
				foreach (IDrawingObject drawing in drawings) {
					Picture picture = drawing as Picture;
					if (picture != null)
						ExportPicture(cell, drawing, picture.Image);
					else {
						Chart chart = drawing as Chart;
						if (chart != null) {
							float width = documentModel.UnitConverter.ModelUnitsToPixelsF(drawing.Width, DocumentModel.DpiX);
							float height = documentModel.UnitConverter.ModelUnitsToPixelsF(drawing.Height, DocumentModel.DpiY);
							OfficeImage officeImage = chart.GetChartImage(Size.Round(new SizeF(width, height)));
							if (officeImage != null)
								ExportPictureCore(cell, drawing, officeImage);
						}
					}
				}
			}
			finally {
				PopControl();
			}
			return true;
		}
		void ExportPicture(ICell cell, IDrawingObject picture, OfficeImage image) {
			IDrawingObjectNonVisualProperties properties = picture.DrawingObject.Properties;
			if (!string.IsNullOrEmpty(properties.HyperlinkClickUrl) && properties.HyperlinkClickIsExternal) {
				DXHtmlAnchor anchor = new DXHtmlAnchor();
				anchor.HRef = HyperlinkUriHelper.ConvertToUrl(properties.HyperlinkClickUrl);
				if (!string.IsNullOrEmpty(properties.HyperlinkClickTargetFrame))
					anchor.Target = properties.HyperlinkClickTargetFrame;
				if (!string.IsNullOrEmpty(properties.HyperlinkClickTooltip))
					anchor.ToolTip = properties.HyperlinkClickTooltip;
				AddControlToChild(CurrentParent, anchor);
				PushControl(anchor);
				try {
					ExportPictureCore(cell, picture, image);
				}
				finally {
					PopControl();
				}
			}
			else
				ExportPictureCore(cell, picture, image);
		}
		void ExportPictureCore(ICell cell, IDrawingObject picture, OfficeImage image) {
			WebImageControl imageControl = new WebImageControl();
			AddControlToChild(CurrentParent, imageControl);
			imageControl.Style.Add(DXHtmlTextWriterStyle.Position, "absolute");
			float left = documentModel.UnitConverter.ModelUnitsToPixelsF(picture.From.ColOffset, DocumentModel.DpiX);
			float top = documentModel.UnitConverter.ModelUnitsToPixelsF(picture.From.RowOffset, DocumentModel.DpiY);
			imageControl.Style.Add(DXHtmlTextWriterStyle.Left, new DXWebUnit(left, DXWebUnitType.Pixel).ToString());
			imageControl.Style.Add(DXHtmlTextWriterStyle.Top, new DXWebUnit(top, DXWebUnitType.Pixel).ToString());
			SizeF actualSize = new SizeF(picture.Width, picture.Height);
			imageHelper.ApplyImageProperties(imageControl, image, Size.Round(actualSize), ImageRepository, DisposeConvertedImagesImmediately, false);
		}
		int CalculateCellWidth(ICell cell, CellRange mergedCellRange) {
			if (mergedCellRange == null || mergedCellRange.Width <= 1)
				return columnWidths[cell.ColumnIndex];
			else {
				int result = 0;
				int lastColumnIndex = cell.ColumnIndex + mergedCellRange.Width - 1;
				for (int i = cell.ColumnIndex; i <= lastColumnIndex; i++)
					result += columnWidths[i];
				return result;
			}
		}
		void SetTextProperties(DXHtmlControl control, HtmlExporterTextProperties textProperties, XlHorizontalAlignment horizontalAlignment, IActualCellAlignmentInfo alignment) {
			FontInfo fontInfo = textProperties.FontInfo;
			bool useFontSizeInPixels = options.FontUnit == HtmlFontUnit.Pixel;
			XlVerticalAlignment verticalAlignment = alignment.Vertical;
			styleBuilder.AppendStyle(fontInfo.Name, textProperties.FontSize, GraphicsUnit.Point, fontInfo.Bold, fontInfo.Italic, textProperties.Strikeout, textProperties.Underline, textProperties.ForeColor, textProperties.BackColor, useFontSizeInPixels);
			if (!alignment.WrapText)
				styleBuilder.AppendStyle(DXHtmlTextWriterStyle.WhiteSpace, "nowrap");
			if (horizontalAlignment != XlHorizontalAlignment.Left)
				styleBuilder.AppendStyle(DXHtmlTextWriterStyle.TextAlign, GetHtmlHorizontalAlignment(horizontalAlignment));
			if (verticalAlignment != XlVerticalAlignment.Center)
				styleBuilder.AppendStyle(DXHtmlTextWriterStyle.VerticalAlign, GetHtmlVerticalAlignment(verticalAlignment));
		}
		void ExportBorders(IActualBorderInfo borders) {
			ExportBorder("top", borders.TopLineStyle, borders.TopColor);
			ExportBorder("bottom", borders.BottomLineStyle, borders.BottomColor);
			ExportBorder("left", borders.LeftLineStyle, borders.LeftColor);
			ExportBorder("right", borders.RightLineStyle, borders.RightColor);
		}
		void ExportBorder(string border, XlBorderLineStyle lineStyle, Color color) {
			if (lineStyle == XlBorderLineStyle.None)
				return;
			if (DXColor.IsTransparentOrEmpty(color))
				return;
			styleBuilder.AppendStyle("border-" + border + "-style", GetHtmlBorderStyle(lineStyle));
			styleBuilder.AppendStyle("border-" + border + "-color", HtmlConvert.ToHtml(color));
			styleBuilder.AppendStyle("border-" + border + "-width", DXWebUnit.Pixel(BorderInfo.LinePixelThicknessTable[lineStyle]).ToString());
		}
		string GetHtmlBorderStyle(XlBorderLineStyle lineStyle) {
			switch (lineStyle) {
				default:
					return "none";
				case XlBorderLineStyle.Hair:
					return "solid";
				case XlBorderLineStyle.Thin:
					return "solid";
				case XlBorderLineStyle.Thick:
					return "solid";
				case XlBorderLineStyle.Medium:
					return "solid";
				case XlBorderLineStyle.Dotted:
					return "dotted";
				case XlBorderLineStyle.Dashed:
					return "dashed";
				case XlBorderLineStyle.Double:
					return "double";
				case XlBorderLineStyle.DashDot:
					return "dashed";
				case XlBorderLineStyle.DashDotDot:
					return "dotted";
				case XlBorderLineStyle.MediumDashDot:
					return "dotted";
				case XlBorderLineStyle.MediumDashDotDot:
					return "dotted";
				case XlBorderLineStyle.MediumDashed:
					return "dashed";
				case XlBorderLineStyle.SlantDashDot:
					return "dotted";
			}
		}
		public string GetHtmlHorizontalAlignment(XlHorizontalAlignment align) {
			return alignHT[align];
		}
		protected internal string GetHtmlVerticalAlignment(XlVerticalAlignment vAlignment) {
			switch (vAlignment) {
				case XlVerticalAlignment.Top:
					return "top";
				case XlVerticalAlignment.Bottom:
					return "bottom";
				case XlVerticalAlignment.Distributed:
				case XlVerticalAlignment.Justify:
				default:
					return "middle";
			}
		}
		protected virtual HtmlStyleBuilder CreateHtmlStyleBuilder() {
			if (IsExportInlineStyle)
				return new HtmlStyleBuilder();
			else
				return new HtmlClassStyleBuilder(ScriptContainer);
		}
	}
	#endregion
	#region HtmlStyleBuilder
	public class HtmlStyleBuilder {
		DXHtmlControl control;
		protected DXHtmlControl Control { get { return control; } }
		public virtual void Begin(DXHtmlControl control) {
			System.Diagnostics.Debug.Assert(this.control == null);
			System.Diagnostics.Debug.Assert(control != null);
			this.control = control;
		}
		public virtual void AppendStyle(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, Color foreColor, Color backColor, bool useFontSizeInPixels) {
			HtmlStyleRender.GetHtmlStyle(fontFamilyName, size, unit, bold, italic, strikeout, underline, foreColor, backColor, useFontSizeInPixels, control.Style);
		}
		public virtual void AppendStyle(DXHtmlTextWriterStyle key, string value) {
			control.Style.Add(key, value);
		}
		public virtual void AppendStyle(string key, string value) {
			control.Style.Add(key, value);
		}
		public virtual void End(DXHtmlControl control) {
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(this.control, control));
			this.control = null;
		}
	}
	#endregion
	#region HtmlClassStyleBuilder
	public class HtmlClassStyleBuilder : HtmlStyleBuilder {
		readonly IScriptContainer scriptContainer;
		readonly StringBuilder style;
		public HtmlClassStyleBuilder(IScriptContainer scriptContainer) {
			Guard.ArgumentNotNull(scriptContainer, "scriptContainer");
			this.style = new StringBuilder();
			this.scriptContainer = scriptContainer;
		}
		public override void Begin(DXHtmlControl control) {
			base.Begin(control);
			this.style.Clear();
		}
		public override void AppendStyle(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, Color foreColor, Color backColor, bool useFontSizeInPixels) {
			string text = HtmlStyleRender.GetHtmlStyle(fontFamilyName, size, unit, bold, italic, strikeout, underline, foreColor, backColor, useFontSizeInPixels);
			if (style.Length > 0 && !String.IsNullOrEmpty(text) && text[0] != ';')
				style.Append(';');
			style.Append(text.Trim());
		}
		public override void AppendStyle(DXHtmlTextWriterStyle key, string value) {
			AppendStyle(DevExpress.XtraPrinting.HtmlExport.Native.DXCssTextWriter.GetStyleName(key), value);
		}
		public override void AppendStyle(string key, string value) {
			if (style.Length > 0 && style[style.Length - 1] != ';')
				style.Append(';');
			style.Append(key);
			style.Append(':');
			style.Append(value.Trim());
		}
		public override void End(DXHtmlControl control) {
			string styleName = scriptContainer.RegisterCssClass(style.ToString());
			control.CssClass = styleName;
			base.End(control);
		}
	}
	#endregion
	#region HtmlExporterTextProperties
	public class HtmlExporterTextProperties {
		readonly FontInfo fontInfo;
		readonly Color foreColor;
		readonly Color backColor;
		readonly bool underline;
		readonly bool strikeout;
		readonly bool allCaps;
		readonly int fontSize;
		public HtmlExporterTextProperties(FontInfo fontInfo, Color foreColor, Color backColor, bool underline, bool strikeout,  bool allCaps, int fontSize) {
			this.fontInfo = fontInfo;
			this.foreColor = foreColor;
			this.backColor = backColor;
			this.underline = underline;
			this.strikeout = strikeout;
			this.allCaps = allCaps;
			this.fontSize = fontSize;
		}
		public FontInfo FontInfo { get { return fontInfo; } }
		public Color ForeColor { get { return foreColor; } }
		public Color BackColor { get { return backColor; } }
		public bool Underline { get { return underline; } }
		public bool Strikeout { get { return strikeout; } }
		public bool AllCaps { get { return allCaps; } }
		public int FontSize { get { return fontSize; } }
		public override bool Equals(object obj) {
			HtmlExporterTextProperties other = obj as HtmlExporterTextProperties;
			if (Object.ReferenceEquals(other, null))
				return false;
			return FontInfo == other.FontInfo &&
				ForeColor == other.ForeColor &&
				BackColor == other.BackColor &&
				Underline == other.Underline &&
				Strikeout == other.Strikeout &&
				AllCaps == other.AllCaps &&
				FontSize == other.FontSize;
		}
		public override int GetHashCode() {
			return FontInfo.GetHashCode() ^
				ForeColor.GetHashCode() ^
				BackColor.GetHashCode() ^
				(int)(allCaps ? 0x80000000 : 0) ^
				(underline ? 0x40000000 : 0) ^
				(strikeout ? 0x20000000 : 0) ^
				FontSize.GetHashCode();
		}
	}
	#endregion
}
