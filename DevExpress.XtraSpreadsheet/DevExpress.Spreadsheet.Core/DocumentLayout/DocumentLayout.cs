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
using System.Drawing;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region DocumentLayout
	public class DocumentLayout {
		#region Fields
		readonly DocumentModel documentModel;
		readonly DevExpress.XtraSpreadsheet.Utils.ChunkedList<Page> pages;
		readonly Dictionary<XlBorderLineStyle, int> lineThicknessTable;
		readonly int twoPixelsPadding;
		readonly int fourPixelsPadding;
		ScrollInfo scrollInfo;
		CellRange visibleRange;
		Dictionary<string, HeaderFooterLayout> headerFooterTable;
		#endregion
		public DocumentLayout(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.pages = new DevExpress.XtraSpreadsheet.Utils.ChunkedList<Page>();
			this.lineThicknessTable = CreateLinePixelThicknessTable();
			this.twoPixelsPadding = (int)Math.Round(documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(2, DocumentModel.DpiX));
			this.fourPixelsPadding = twoPixelsPadding * 2;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public DocumentLayoutUnitConverter UnitConverter { get { return DocumentModel.LayoutUnitConverter; } }
		public DevExpress.XtraSpreadsheet.Utils.ChunkedList<Page> Pages { get { return pages; } }
		public HeaderPage HeaderPage { get; set; }
		public GroupItemsPage GroupItemsPage { get; set; }
		public Dictionary<XlBorderLineStyle, int> LineThicknessTable { get { return lineThicknessTable; } }
		public virtual int TwoPixelsPadding { get { return twoPixelsPadding; } }
		public virtual int FourPixelsPadding { get { return fourPixelsPadding; } }
		public float ScaleFactorLimitForVisibleGridlines { get { return 0.4f; } }
		public ScrollInfo ScrollInfo { get { return scrollInfo; } set { scrollInfo = value; } }
		public CellRange VisibleRange { get { return visibleRange; } set { visibleRange = value; } }
		#endregion
		Dictionary<XlBorderLineStyle, int> CreateLinePixelThicknessTable() {
			DocumentLayoutUnitConverter unitConverter = documentModel.LayoutUnitConverter;
			Dictionary<XlBorderLineStyle, int> linePixelThicknessTable = BorderInfo.LinePixelThicknessTable;
			Dictionary<XlBorderLineStyle, int> result = new Dictionary<XlBorderLineStyle, int>();
			foreach (XlBorderLineStyle key in linePixelThicknessTable.Keys) {
				int pixels = linePixelThicknessTable[key];
				if (pixels != 0) {
					result.Add(key, Math.Max(1, (int)Math.Round(unitConverter.PixelsToLayoutUnitsF(pixels, DocumentModel.Dpi))));
				}
				else
					result.Add(key, 0);
			}
			return result;
		}
		protected internal virtual Page GetPageByPoint(Point point) {
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				Page currentPage = pages[i];
				if (currentPage.GetHitTestClientBounds().Contains(point)) 
					return currentPage;
			}
			return null;
		}
		protected internal virtual HeaderPage GetHeaderPageByPoint(Point point) {
			if (this.HeaderPage == null || !this.HeaderPage.ContainsPoint(point))
				return null;
			return this.HeaderPage;
		}
		protected internal virtual GroupItemsPage GetGroupPageByPoint(Point point) {
			if (this.GroupItemsPage == null || !this.GroupItemsPage.ContainsPoint(point))
				return null;
			return this.GroupItemsPage;
		}
		protected internal virtual void GenerateHeadersContent(List<PageGrid> columnHeaderGrids, List<PageGrid> rowHeaderGrids, Size headerOffset, Size gridItemsOffset) {
			HeaderPage.GenerateContent(columnHeaderGrids, rowHeaderGrids, headerOffset, gridItemsOffset, true);
		}
		public Page GetExactOrNearPageByModelIndexes(int columnModelIndex, int rowModelIndex) {
			int pageCount = pages.Count;
			if (pageCount == 1)
				return pages[0];
			if (pageCount == 2)
				return ChoosePageFromTwoPages(columnModelIndex, rowModelIndex);
			if (pageCount == 4)
				return ChoosePageFromFourPages(columnModelIndex, rowModelIndex);
			return null;
		}
		Page ChoosePageFromTwoPages(int columnModelIndex, int rowModelIndex) {
			Page topLeftPage = pages[0];
			Page bottomRightPage = pages[1];
			bool isVerticalPages = topLeftPage.GridColumns.Last.ModelIndex < bottomRightPage.GridColumns.First.ModelIndex;
			if (isVerticalPages) {
				if (columnModelIndex <= topLeftPage.GridColumns.First.ModelIndex)
					return topLeftPage;
				return bottomRightPage;
			}
			if (rowModelIndex <= topLeftPage.GridRows.Last.ModelIndex)
				return topLeftPage;
			return bottomRightPage;
		}
		Page ChoosePageFromFourPages(int columnModelIndex, int rowModelIndex) {
			Page topLeftPage = pages[0];
			Page topRightPage = pages[1];
			if (columnModelIndex <= topLeftPage.GridColumns.Last.ModelIndex && rowModelIndex <= topLeftPage.GridRows.Last.ModelIndex)
				return topLeftPage;
			if (columnModelIndex >= topRightPage.GridColumns.First.ModelIndex && rowModelIndex <= topRightPage.GridRows.Last.ModelIndex)
				return topRightPage;
			Page bottomLeftPage = pages[2];
			if (columnModelIndex <= bottomLeftPage.GridColumns.Last.ModelIndex)
				return bottomLeftPage;
			return pages[3];
		}
		public Page GetExactPageByModelIndexes(int columnModelIndex, int rowModelIndex) {
			foreach (Page page in pages) {
				bool containsColumn = ContainsModelIndex(page.GridColumns, columnModelIndex);
				bool containsRow = ContainsModelIndex(page.GridRows, rowModelIndex);
				if (containsColumn && containsRow)
					return page;
			}
			return null;
		}
		bool ContainsModelIndex(PageGrid grid, int modelIndex) {
			return grid.First.ModelIndex <= modelIndex && grid.Last.ModelIndex >= modelIndex;
		}
		protected internal HeaderFooterLayout GetHeaderFooterLayout(string sheetName) {
			if (headerFooterTable == null)
				return null;
			HeaderFooterLayout value;
			return headerFooterTable.TryGetValue(sheetName, out value) ? value : null;
		}
		protected internal void CalculateHeaderFooterLayout(Worksheet sheet, Rectangle pageBounds) {
			if (headerFooterTable == null)
				headerFooterTable = new Dictionary<string, HeaderFooterLayout>();
			HeaderFooterLayoutCalculator calculator = new HeaderFooterLayoutCalculator(sheet, pageBounds);
			HeaderFooterLayout headerFooterLayout = calculator.CalculateHeaderFooter();
			if (headerFooterLayout != null)
				headerFooterTable.Add(sheet.Name, headerFooterLayout);
		}
	}
	#endregion
	public interface ICellErrorTextProvider {
		string GetCellText(ICell cell, string text);
	}
	public class DefaultCellErrorTextProvider : ICellErrorTextProvider {
		public string GetCellText(ICell cell, string text) {
			return text;
		}
	}
	public class PrintingCellErrorTextProvider : ICellErrorTextProvider {
		public string GetCellText(ICell cell, string text) {
			if (cell.Value.IsError)
				return GetCellErrorText(cell);
			else
				return text;
		}
		string GetCellErrorText(ICell cell) {
			switch (cell.Worksheet.PrintSetup.ErrorsPrintMode) {
				default:
				case ModelErrorsPrintMode.Displayed:
					return cell.Text;
				case ModelErrorsPrintMode.Blank:
					return String.Empty;
				case ModelErrorsPrintMode.Dash:
					return "--";
				case ModelErrorsPrintMode.NA:
					return "#N/A";
			}
		}
	}
	public interface ICellTextBox {
		bool HasBindingField { get; set; }
		bool HasBindingPicture { get; set; }
		bool HasPivotIndent { get; }
		bool HasPivotExpandCollapseButton { get; }
		bool IsPivotButtonCollapsed { get; set; }
		bool HasPivotLabelFilterButton { get; }
		bool HasPivotPageFilterButton { get; }
		bool IsPivotRowFieldsFilter { get; }
		int GetPivotTableFieldIndex();
		int GetPivotTableItemIndex();
		Rectangle GetBounds(Page page);
		Rectangle GetBounds(ICell cell, Rectangle textBounds, DocumentLayout layout);
		Rectangle GetFillBounds(Page page);
		Rectangle GetClipBounds(Page page);
		Rectangle GetTextBounds(Page page, DocumentLayout layout);
		ICell GetCell(PageGrid gridColumns, PageGrid gridRows, IWorksheet sheet);
		Rectangle CalculateActualTextBounds(Page page, DocumentLayout layout, int textWidth, XlHorizontalAlignment align);
		CellForegroundDisplayFormat CalculateForegroundDisplayFormat(Page page, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText);
		CellForegroundDisplayFormat CalculateForegroundDisplayFormat(Page page, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText, IDocumentModelSkinColorProvider skinColorProvider, bool useScaleFactor);
		CellBackgroundDisplayFormat CalculateBackgroundDisplayFormat(Page page, IDocumentModelSkinColorProvider skinColorProvider);
		Rectangle CalculateExpandCollapseButtonBounds(Page page, ICell cell);
	}
	public class CellForegroundDisplayFormat {
		public ICell Cell { get; set; }
		public Rectangle Bounds { get; set; }
		public string Text { get; set; }
		public FontInfo FontInfo { get; set; }
		public Color ForeColor { get; set; }
		public ModelHyperlink Hyperlink { get; set; }
		public XlBorderLineStyle DiagonalUpBorderLineStyle { get; set; }
		public XlBorderLineStyle DiagonalDownBorderLineStyle { get; set; }
		public Color DiagonalBorderColor { get; set; }
	}
	public class CellBackgroundDisplayFormat {
		public ICell Cell { get; set; }
		public Rectangle Bounds { get; set; }
		public XlPatternType PatternType { get; set; }
		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }
		public IActualGradientFillInfo GradientFill { get; set; }
		public bool ShouldUseForeColor { get { return PatternType > XlPatternType.Solid; } }
#if !SL && !DXPORTABLE
		public Brush CreateGradientBrush() {
			if (GradientFill == null)
				return Brushes.Transparent;
			IActualGradientStopCollection stops = GradientFill.GradientStops;
			if (stops.Count <= 0)
				return Brushes.Transparent;
			if (GradientFill.Type == ModelGradientFillType.Path) {
				IActualConvergenceInfo conv = GradientFill.Convergence;
				RectangleF rect = Bounds;
				PointF[] points = new PointF[] {
					new PointF(rect.Left, rect.Top),
					new PointF(rect.Right, rect.Top),
					new PointF(rect.Right, rect.Bottom),
					new PointF(rect.Left, rect.Bottom),
				};
				PathGradientBrush brush = new PathGradientBrush(points);
				brush.CenterColor = stops[0].Color;
				brush.SurroundColors = new Color[] { stops[1].Color };
				brush.CenterPoint = new PointF(Bounds.Left + Bounds.Width * (conv.Left + conv.Right) / 2.0f, Bounds.Top + Bounds.Height * (conv.Top + conv.Bottom) / 2.0f);
				return brush;
			}
			else {
				if (stops.Count == 3) {
					LinearGradientBrush linearBrush = new LinearGradientBrush(Bounds, stops[0].Color, stops[1].Color, (float)GradientFill.Degree, true);
					linearBrush.SetBlendTriangularShape(0.5f);
					return linearBrush;
				}
				else
					return new LinearGradientBrush(Bounds, stops[0].Color, stops[stops.Count - 1].Color, (float)GradientFill.Degree, true);
			}
		}
#endif
	}
	public abstract class CellTextBoxBase : ICellTextBox {
		public static int ExpandCollapseButtonSizeInPixels = 9;
		public static int ExpandCollapseButtonPaddingInPixels = 5;
		#region Properties
		public bool HasBindingField { get; set; }
		public bool HasBindingPicture { get; set; }
		public PivotLayoutCellInfo PivotLayoutCellInfo { get; set; }
		public bool HasPivotIndent { get { return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.Indent : false; } }
		public bool HasPivotExpandCollapseButton { get { return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.CollapseButton : false; } }
		public bool HasPivotLabelFilterButton { get { return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.LabelFilterButton : false; } }
		public bool HasPivotPageFilterButton { get { return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.PageFilterButton : false; } }
		public bool IsPivotRowFieldsFilter { get { return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.RowFieldsFilter : false; } }
		public bool IsPivotButtonCollapsed { get; set; }
		#endregion
		public int GetPivotTableFieldIndex() {
			return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.FieldIndex : -1;
		}
		public int GetPivotTableItemIndex() {
			return PivotLayoutCellInfo != null ? PivotLayoutCellInfo.ItemIndex : -1;
		}
		public abstract Rectangle GetBounds(Page page);
		public abstract Rectangle GetFillBounds(Page page);
		public abstract Rectangle GetClipBounds(Page page);
		public Rectangle GetWebBounds(Page page) {
			Rectangle clipBounds = GetBounds(page);
			Point offset = Point.Empty;
			offset.X = -page.GridColumns.ActualFirst.Near;
			offset.Y = -page.GridRows.ActualFirst.Near;
			clipBounds.Offset(offset);
			return clipBounds;
		}
		public Rectangle GetWebClipBounds(Page page) {
			Rectangle clipBounds = GetClipBounds(page);
			Point offset = Point.Empty;
			offset.X = -page.GridColumns.ActualFirst.Near;
			offset.Y = -page.GridRows.ActualFirst.Near;
			clipBounds.Offset(offset);
			return clipBounds;
		}
		public abstract ICell GetCell(PageGrid gridColumns, PageGrid gridRows, IWorksheet sheet);
		public static Rectangle GetTextBounds(ICell cell, Rectangle bounds, Page page, DocumentLayout layout, bool hasPivotIndent) {
			Rectangle result = bounds;
			result.Width = Math.Max(0, result.Width - layout.FourPixelsPadding);
			result.X += layout.TwoPixelsPadding;
			IActualCellAlignmentInfo alignment = cell.ActualAlignment;
			result = ApplyIndent(alignment, result, layout, alignment.Indent, hasPivotIndent);
			return result;
		}
		public static Rectangle ApplyIndent(IActualCellAlignmentInfo alignment, Rectangle textBounds, DocumentLayout layout, int indent, bool hasPivotIndent) {
			return ApplyIndentCore(alignment, textBounds, layout, Math.Max(0, indent), hasPivotIndent);
		}
		public static int CalculateIndentValue(DocumentModel documentModel, int indent, bool hasPivotIndent) {
			int result = indent * 3 * documentModel.GetDefaultFontInfo().SpaceWidth;
			if (hasPivotIndent)
				result += CalculatePivotIndent(documentModel.LayoutUnitConverter, indent);
			return result;
		}
		static int CalculatePivotIndent(DocumentLayoutUnitConverter converter, int cellIndent) {
			return converter.PixelsToLayoutUnits(ExpandCollapseButtonSizeInPixels) + 4 * (cellIndent + 1);
		}
#if DEBUGTEST
		internal static int Test_CalculatePivotIndent(DocumentLayoutUnitConverter converter, int cellIndent) {
			return CalculatePivotIndent(converter, cellIndent);
		}
#endif
		public static Rectangle ApplyIndentCore(IActualCellAlignmentInfo alignment, Rectangle textBounds, DocumentLayout layout, int indent, bool hasPivotIndent) {
			if (indent != 0 || hasPivotIndent) {
				int indentValue = CalculateIndentValue(layout.DocumentModel, indent, hasPivotIndent);
				XlHorizontalAlignment horizontalAlignment = alignment.Horizontal;
				if (horizontalAlignment == XlHorizontalAlignment.Right) {
					textBounds.Width -= indentValue;
				}
				else if (horizontalAlignment != XlHorizontalAlignment.Center && horizontalAlignment != XlHorizontalAlignment.CenterContinuous) {
					textBounds.X += indentValue;
					textBounds.Width -= indentValue;
				}
			}
			return textBounds;
		}
		public Rectangle GetTextBounds(Page page, DocumentLayout layout) {
			return GetTextBounds(GetCell(page.GridColumns, page.GridRows, page.Sheet), GetBounds(page), page, layout, HasPivotIndent);
		}
		public Rectangle GetBounds(ICell cell, Rectangle textBounds, DocumentLayout layout) {
			Rectangle result = textBounds;
			result.X -= layout.TwoPixelsPadding;
			result.Width += layout.FourPixelsPadding;
			IActualCellAlignmentInfo alignment = cell.ActualAlignment;
			result = ApplyIndentCore(alignment, result, layout, -Math.Max(0, (int)alignment.Indent), HasPivotIndent);
			return result;
		}
		public Rectangle CalculateActualTextBounds(Page page, DocumentLayout layout, int textWidth, XlHorizontalAlignment align) {
			Rectangle textBounds = GetTextBounds(page, layout);
			switch (align) {
				default:
				case XlHorizontalAlignment.General:
				case XlHorizontalAlignment.Fill:
				case XlHorizontalAlignment.Justify:
				case XlHorizontalAlignment.Distributed:
				case XlHorizontalAlignment.Left:
					break;
				case XlHorizontalAlignment.Center:
				case XlHorizontalAlignment.CenterContinuous:
					textBounds.X += (textBounds.Width - textWidth) / 2;
					break;
				case XlHorizontalAlignment.Right:
					textBounds.X += textBounds.Width - textWidth;
					break;
			}
			textBounds.Width = textWidth;
			return textBounds;
		}
		public CellForegroundDisplayFormat CalculateForegroundDisplayFormat(Page page, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText) {
			return CalculateForegroundDisplayFormat(page, errorTextProvider, calculateForEmptyText, true);
		}
		public CellForegroundDisplayFormat CalculateForegroundDisplayFormat(Page page, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText, bool calculateBorderInfo) {
			return CalculateForegroundDisplayFormat(page, errorTextProvider, calculateForEmptyText, page.DocumentLayout.DocumentModel, false, calculateBorderInfo);
		}
		public CellForegroundDisplayFormat CalculateForegroundDisplayFormat(Page page, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText, IDocumentModelSkinColorProvider skinColorProvider, bool useScaleFactor) {
			return CalculateForegroundDisplayFormat(page, errorTextProvider, calculateForEmptyText, skinColorProvider, useScaleFactor, true);
		}
		public CellForegroundDisplayFormat CalculateForegroundDisplayFormat(Page page, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText, IDocumentModelSkinColorProvider skinColorProvider, bool useScaleFactor, bool calculateBorderInfo) {
			ICell cell = GetCell(page.GridColumns, page.GridRows, page.Sheet);
			Rectangle bounds = GetTextBounds(page, page.DocumentLayout);
			float scaleFactor = useScaleFactor ? page.ScaleFactor : 1.0f;
			return CalculateForegroundDisplayFormat(cell, bounds, errorTextProvider, calculateForEmptyText, skinColorProvider, scaleFactor, calculateBorderInfo);
		}
		public static CellForegroundDisplayFormat CalculateForegroundDisplayFormat(ICell cell, Rectangle bounds, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText, IDocumentModelSkinColorProvider skinColorProvider, float scaleFactor) {
			return CalculateForegroundDisplayFormat(cell, bounds, errorTextProvider, calculateForEmptyText, skinColorProvider, scaleFactor, true);
		}
		public static CellForegroundDisplayFormat CalculateForegroundDisplayFormat(ICell cell, Rectangle bounds, ICellErrorTextProvider errorTextProvider, bool calculateForEmptyText, IDocumentModelSkinColorProvider skinColorProvider, float scaleFactor, bool calculateBorderInfo) {
			CellForegroundDisplayFormat result = new CellForegroundDisplayFormat();
			CellFormatStringMeasurer measurer = new CellFormatStringMeasurer(cell);
			int width;
			if (scaleFactor != 1.0f)
				width = (int)Math.Round(bounds.Width / scaleFactor);
			else
				width = bounds.Width;
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = measurer;
			parameters.AvailableSpaceWidth = width;
			NumberFormatResult formatResult = cell.GetFormatResult(parameters);
			result.Text = errorTextProvider.GetCellText(cell, formatResult.Text);
			result.Cell = cell;
			result.Bounds = bounds;
			if (calculateBorderInfo)
				CalculateBorderInfo(result, cell, skinColorProvider);
			if (String.IsNullOrEmpty(result.Text) && !calculateForEmptyText)
				return result;
			IActualRunFontInfo cellActualFont = cell.ActualFont;
			result.Hyperlink = GetCellHyperlink(cell);
			result.FontInfo = cellActualFont.GetFontInfo();
			result.ForeColor = Cell.GetTextColor(skinColorProvider, cellActualFont, formatResult, false);
			return result;
		}
		static void CalculateBorderInfo(CellForegroundDisplayFormat result, ICell cell, IDocumentModelSkinColorProvider skinColorProvider) {
			IActualBorderInfo border = cell.ActualBorder;
			result.DiagonalDownBorderLineStyle = border.DiagonalDownLineStyle;
			result.DiagonalUpBorderLineStyle = border.DiagonalUpLineStyle;
			if (result.DiagonalDownBorderLineStyle != XlBorderLineStyle.None || result.DiagonalUpBorderLineStyle != XlBorderLineStyle.None)
				result.DiagonalBorderColor = GetDiagonalBorderColor(border, skinColorProvider);
		}
		public static Color GetDiagonalBorderColor(IActualBorderInfo border, IDocumentModelSkinColorProvider skinColorProvider) {
			Color result = border.DiagonalColor;
			return !DXColor.IsTransparentOrEmpty(result) ? result : skinColorProvider.SkinForeColor;
		}
		public CellBackgroundDisplayFormat CalculateBackgroundDisplayFormat(Page page, IDocumentModelSkinColorProvider skinColorProvider) {
			CellBackgroundDisplayFormat result = new CellBackgroundDisplayFormat();
			ICell cell = GetCell(page.GridColumns, page.GridRows, page.Sheet);
			result.Cell = cell;
			result.Bounds = GetFillBounds(page);
			result.PatternType = cell.ActualPatternType;
			result.BackColor = cell.ActualBackgroundColor;
			if (result.ShouldUseForeColor)
				result.ForeColor = GetFillForeColor(cell.ActualForegroundColor, skinColorProvider);
			if (DXColor.IsTransparentOrEmpty(result.BackColor)) {
				IActualFillInfo actualFill = cell.ActualFill;
				if (actualFill.FillType == ModelFillType.Gradient)
					result.GradientFill = actualFill.GradientFill;
			}
			return result;
		}
		internal static Color GetFillForeColor(Color color, IDocumentModelSkinColorProvider skinColorProvider) {
			if (!DXColor.IsTransparentOrEmpty(color))
				return color;
			else
				return skinColorProvider.SkinForeColor;
		}
		static ModelHyperlink GetCellHyperlink(ICell cell) {
			ModelHyperlinkCollection hyperlinks = cell.Worksheet.Hyperlinks;
			int index = hyperlinks.GetHyperlink(cell);
			if (index < 0)
				return null;
			return hyperlinks[index];
		}
		public Rectangle CalculateExpandCollapseButtonBounds(Page page, ICell cell) {
			if (!HasPivotExpandCollapseButton)
				return Rectangle.Empty;
			Rectangle cellBounds = GetBounds(page);
			DocumentLayoutUnitConverter converter = page.DocumentLayout.DocumentModel.LayoutUnitConverter;
			int pagging = converter.PixelsToLayoutUnits(ExpandCollapseButtonPaddingInPixels, DocumentModel.Dpi);
			int sizeInLayouts = converter.PixelsToLayoutUnits(ExpandCollapseButtonSizeInPixels, DocumentModel.Dpi);
			int left;
			IActualCellAlignmentInfo alignmentInfo = cell.ActualAlignment;
			left = cellBounds.Left + pagging;
			int top;
			if (alignmentInfo.Vertical == XlVerticalAlignment.Top)
				top = cellBounds.Top + pagging;
			else if (alignmentInfo.Vertical == XlVerticalAlignment.Bottom)
				top = cellBounds.Bottom - pagging - sizeInLayouts;
			else
				top = cellBounds.Top + cellBounds.Height / 2 - sizeInLayouts / 2;
			return new Rectangle(left, top, sizeInLayouts, sizeInLayouts);
		}
	}
	#region SingleCellTextBox
	public class SingleCellTextBox : CellTextBoxBase {
		int gridRowIndex;
		short gridColumnIndex;
		short clipFirstColumnIndex; 
		short clipLastColumnIndex;
		public int GridRowIndex { get { return gridRowIndex; } set { gridRowIndex = value; } }
		public int GridColumnIndex { get { return gridColumnIndex; } set { gridColumnIndex = (short)value; } }
		public int ClipFirstColumnIndex { get { return clipFirstColumnIndex; } set { clipFirstColumnIndex = (short)value; } }
		public int ClipLastColumnIndex { get { return clipLastColumnIndex; } set { clipLastColumnIndex = (short)value; } }
		public override Rectangle GetBounds(Page page) {
			PageGridItem column = page.GridColumns[GridColumnIndex];
			PageGridItem row = page.GridRows[GridRowIndex];
			return new Rectangle(page.ClientLeft + column.Near, page.ClientTop + row.Near, column.Extent, row.Extent);
		}
		public override Rectangle GetFillBounds(Page page) {
			return GetBounds(page);
		}
		public override Rectangle GetClipBounds(Page page) {
			PageGridItem firstColumn = page.GridColumns[clipFirstColumnIndex];
			PageGridItem lastColumn = page.GridColumns[clipLastColumnIndex];
			PageGridItem row = page.GridRows[GridRowIndex];
			return new Rectangle(page.ClientLeft + firstColumn.Near, page.ClientTop + row.Near, lastColumn.Far - firstColumn.Near, row.Extent);
		}
		public override ICell GetCell(PageGrid gridColumns, PageGrid gridRows, IWorksheet sheet) {
			PageGridItem column = gridColumns[GridColumnIndex];
			PageGridItem row = gridRows[GridRowIndex];
			ICell cell = (ICell)sheet.TryGetCell(column.ModelIndex, row.ModelIndex);
			if (cell == null)
				cell = new FakeCell(new CellPosition(column.ModelIndex, row.ModelIndex), sheet as Worksheet);
			return cell;
		}
	}
	#endregion
	#region ComplexCellTextBox
	public class ComplexCellTextBox : CellTextBoxBase {
		readonly ICell hostCell;
		Rectangle bounds;
		int clipFirstRowIndex; 
		int clipLastRowIndex;
		int clipFirstColumnIndex; 
		int clipLastColumnIndex;
		public ComplexCellTextBox(ICell hostCell) {
			Guard.ArgumentNotNull(hostCell, "hostCell");
			this.hostCell = hostCell;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public int ClipFirstRowIndex { get { return clipFirstRowIndex; } set { clipFirstRowIndex = value; } }
		public int ClipLastRowIndex { get { return clipLastRowIndex; } set { clipLastRowIndex = value; } }
		public int ClipFirstColumnIndex { get { return clipFirstColumnIndex; } set { clipFirstColumnIndex = value; } }
		public int ClipLastColumnIndex { get { return clipLastColumnIndex; } set { clipLastColumnIndex = value; } }
		public bool IsLongTextBox { get; set; }
		public override Rectangle GetBounds(Page page) {
			return bounds;
		}
		public override Rectangle GetFillBounds(Page page) {
			CellRange range = hostCell.Worksheet.MergedCells.GetMergedCellRange(hostCell);
			if (range != null)
				return GetBounds(page);
			PageGridItem firstColumn = page.GridColumns[clipFirstColumnIndex];
			PageGridItem firstRow = page.GridRows[clipFirstRowIndex];
			if (firstColumn.ModelIndex == hostCell.ColumnIndex && firstRow.ModelIndex == hostCell.RowIndex)
				return new Rectangle(page.ClientLeft + firstColumn.Near, page.ClientTop + firstRow.Near, firstColumn.Extent, firstRow.Extent);
			else {
				int gridColumnIndex = page.GridColumns.LookupItem(hostCell.ColumnIndex, clipFirstColumnIndex, clipLastColumnIndex);
				if (gridColumnIndex < 0)
					return Rectangle.Empty;
				int gridRowIndex = page.GridRows.LookupItem(hostCell.RowIndex, clipFirstRowIndex, clipLastRowIndex);
				if (gridRowIndex < 0)
					return Rectangle.Empty;
				firstColumn = page.GridColumns[gridColumnIndex];
				firstRow = page.GridRows[gridRowIndex];
				return new Rectangle(page.ClientLeft + firstColumn.Near, page.ClientTop + firstRow.Near, firstColumn.Extent, firstRow.Extent);
			}
		}
		public override Rectangle GetClipBounds(Page page) {
			PageGridItem firstColumn = page.GridColumns[clipFirstColumnIndex];
			PageGridItem lastColumn = page.GridColumns[clipLastColumnIndex];
			PageGridItem firstRow = page.GridRows[clipFirstRowIndex];
			PageGridItem lastRow = page.GridRows[clipLastRowIndex];
			return new Rectangle(page.ClientLeft + firstColumn.Near, page.ClientTop + firstRow.Near, lastColumn.Far - firstColumn.Near, lastRow.Far - firstRow.Near);
		}
		public override ICell GetCell(PageGrid gridColumns, PageGrid gridRows, IWorksheet sheet) {
			return hostCell;
		}
	}
	#endregion
	#region IDrawingBoxVisitor
	public interface IDrawingBoxVisitor {
		void Visit(PictureBox value);
		void Visit(ChartBox value);
		void Visit(ShapeBox value);
	}
	#endregion
	#region DrawingBox
	public abstract class DrawingBox {
		#region Fields
		readonly IDrawingObject drawing;
		Rectangle bounds;
		Rectangle clipBounds;
		Matrix backwardTransformMatrix;
		#endregion
		protected DrawingBox(IDrawingObject drawing, Rectangle bounds, Rectangle clipBounds) {
			this.drawing = drawing;
			this.bounds = bounds;
			this.clipBounds = clipBounds;
			float angle = drawing.GetRotationAngleInDegrees();
			this.backwardTransformMatrix = TransformMatrixExtensions.CreateTransformUnsafe(-angle, bounds);
		}
		#region Properties
		protected internal IDrawingObject Drawing { get { return drawing; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ClipBounds { get { return clipBounds; } }
		public bool NoChangeAspect { get { return drawing.NoChangeAspect; } }
		public int DrawingIndex { get { return drawing.IndexInCollection; } }
		public Matrix BackwardTransformMatrix { get { return backwardTransformMatrix; } }
		public DrawingObjectType DrawingType { get { return drawing.DrawingType; } }
		#endregion
		public Point TransformPointBackward(Point point) {
			if (backwardTransformMatrix == null)
				return point;
			else
				return backwardTransformMatrix.TransformPoint(point);
		}
		public abstract void Visit(IDrawingBoxVisitor visitor);
	}
	#endregion
	#region PictureBox
	public class PictureBox : DrawingBox {
		#region Fields
		float[][] colorMatrixElements;
		#endregion
		public PictureBox(Picture picture, Rectangle bounds, Rectangle clipBounds)
			: this(picture, bounds, clipBounds, null) {
		}
		public PictureBox(Picture picture, Rectangle bounds, Rectangle clipBounds, float[][] colorMatrixElements)
			: base(picture, bounds, clipBounds) {
			this.colorMatrixElements = colorMatrixElements;
#if !DXPORTABLE
			BorderPen = DrawingShapeController.GetPen(picture.DocumentModel, picture.ShapeStyle, picture.ShapeProperties);
#endif
		}
		#region Properties
		public Pen BorderPen { get; private set; }
		protected internal Picture Picture { get { return (Picture)Drawing; } }
		public OfficeImage Image { get { return Picture.Image; } }
		public Image NativeImage { get { return Image.NativeImage; } }
		public float[][] ColorMatrixElements { get { return colorMatrixElements; } }
		#endregion
		public override void Visit(IDrawingBoxVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region ChartBox
	public class ChartBox : DrawingBox {
		public ChartBox(Chart chart, Rectangle bounds)
			: base(chart, bounds, new Rectangle(0, 0, bounds.Width, bounds.Height)) {
		}
		public ChartBox(Chart chart, Rectangle bounds, Rectangle clipBounds)
			: base(chart, bounds, clipBounds) {
		}
		#region Properties
		public Chart Chart { get { return Drawing as Chart; } }
		#endregion
		public override void Visit(IDrawingBoxVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region ShapeBox
	public class ShapeBox : DrawingBox {
		public ShapeBox(ModelShape shape, Rectangle bounds)
			: base(shape, bounds, new Rectangle(0, 0, bounds.Width, bounds.Height)) { }
		public ShapeBox(ModelShape shape, Rectangle bounds, Rectangle clipBounds) :
			base(shape, bounds, clipBounds) { }
		#region Properties
		public ModelShape Shape { get { return Drawing as ModelShape; } }
		#endregion
		#region Overrides of DrawingBox
		public override void Visit(IDrawingBoxVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
	}
	#endregion
	#region IndicatorType
	public enum IndicatorType {
		Comment,
		NumberFormatting
	}
	#endregion
	#region IndicatorBox
	public class IndicatorBox {
		#region Fields
		readonly Rectangle clipBounds;
		readonly IndicatorType type;
		#endregion
		public IndicatorBox(Rectangle clipBounds, IndicatorType type) {
			this.clipBounds = clipBounds;
			this.type = type;
		}
		#region Properties
		public Rectangle ClipBounds { get { return clipBounds; } }
		public IndicatorType Type { get { return type; } }
		#endregion
	}
	#endregion
	#region CommentBox
	public class CommentBox {
		#region Fields
		readonly Comment comment;
		readonly Rectangle bounds;
		readonly Point endLinePoint;
		#endregion
		public CommentBox(Comment comment, Rectangle bounds, Point endLinePoint) {
			Guard.ArgumentNotNull(comment, "comment");
			this.comment = comment;
			this.bounds = bounds;
			this.endLinePoint = endLinePoint;
		}
		#region Properties
		public Rectangle Bounds { get { return bounds; } }
		public Color FillColor { get { return comment.Shape.Fillcolor; } }
		public CommentRunCollection TextRuns { get { return comment.Runs; } }
		public bool IsHidden { get { return comment.Shape.IsHidden; } }
		public bool LockAspectRatio { get { return comment.Shape.LockAspectRatio; } }
		public CellPosition Reference { get { return comment.Reference; } }
		internal Worksheet Worksheet { get { return comment.Worksheet; } }
		internal bool IsHovered { get; set; }
		#endregion
		public bool CanDraw() {
			return !IsHidden || IsHovered;
		}
		public Point GetIndicatorLineStartPoint() {
			return new Point(bounds.Left, bounds.Top);
		}
		public Point GetIndicatorLineEndPoint() {
			return endLinePoint;
		}
		public string GetNormalizedPlainText() {
			return comment.GetNormalizedLineBreaksPlainText();
		}
		public int GetCommentIndex() {
			return comment.Worksheet.Comments.IndexOf(comment);
		}
	}
	#endregion
	#region BorderLineBox
	public struct BorderLineBox {
		int firstGridIndex;
		int lastGridIndex;
		int colorIndex;
		XlBorderLineStyle lineStyle; 
		public int FirstGridIndex { get { return firstGridIndex; } set { firstGridIndex = value; } }
		public int LastGridIndex { get { return lastGridIndex; } set { lastGridIndex = value; } }
		public int ColorIndex { get { return colorIndex; } set { colorIndex = value; } }
		public XlBorderLineStyle LineStyle { get { return lineStyle; } set { lineStyle = value; } }
		public Rectangle GetBounds(Page page, PageBorderCollection borders) {
			return borders.GetBounds(page, this);
		}
		public Rectangle GetWebBounds(Page page, PageBorderCollection borders) {
			Rectangle bounds = borders.GetBounds(page, this);
			Point offset = Point.Empty;
			offset.X = -page.GridColumns.ActualFirst.Near;
			offset.Y = -page.GridRows.ActualFirst.Near;
			bounds.Offset(offset);
			return bounds;
		}
		internal Rectangle GetHorizontalBounds(Page page, PageHorizontalBorders borders) {
			int gridIndex = borders.GridIndex;
			PageGrid secondaryGrid = page.GridRows;
			int secondaryNear;
			if (gridIndex >= secondaryGrid.Count)
				secondaryNear = secondaryGrid.Last.Far;
			else
				secondaryNear = secondaryGrid[gridIndex].Near;
			PageGrid primaryGrid = page.GridColumns;
			int near = primaryGrid[FirstGridIndex].Near;
			int extent = primaryGrid[LastGridIndex].Far - near;
			int lineThickness = page.DocumentLayout.LineThicknessTable[LineStyle];
			if (LineStyle == XlBorderLineStyle.Double)
				return new Rectangle(page.ClientLeft + near, page.ClientTop + secondaryNear - lineThickness / 2, extent, lineThickness);
			else
				return new Rectangle(page.ClientLeft + near, page.ClientTop + secondaryNear, extent, lineThickness);
		}
		internal Rectangle GetVerticalBounds(Page page, PageVerticalBorders borders) {
			int gridIndex = borders.GridIndex;
			PageGrid secondaryGrid = page.GridColumns;
			int secondaryNear;
			if (gridIndex >= secondaryGrid.Count)
				secondaryNear = secondaryGrid.Last.Far;
			else
				secondaryNear = secondaryGrid[gridIndex].Near;
			PageGrid primaryGrid = page.GridRows;
			int near = primaryGrid[FirstGridIndex].Near;
			int extent = primaryGrid[LastGridIndex].Far - near;
			int lineThickness = page.DocumentLayout.LineThicknessTable[LineStyle];
			if (LineStyle == XlBorderLineStyle.Double)
				return new Rectangle(page.ClientLeft + secondaryNear - lineThickness / 2, page.ClientTop + near, lineThickness, extent);
			else
				return new Rectangle(page.ClientLeft + secondaryNear, page.ClientTop + near, lineThickness, extent);
		}
	}
	#endregion
	#region HeaderTextBox
	public class HeaderTextBox {
		#region fields
		HeaderTextBox previous;
		Rectangle bounds;
		HeaderBoxSelectType selectType;
		List<HeaderTextBox> parentCollection;
		#endregion
		public HeaderTextBox(Rectangle bounds, HeaderBoxType boxType, int modelIndex, HeaderTextBox previous, List<HeaderTextBox> parentCollection) {
			this.previous = previous;
			SetBounds(bounds);
			BoxType = boxType;
			ModelIndex = modelIndex;
			this.parentCollection = parentCollection;
		}
		#region properties
		public string Text { get; set; }
		public Rectangle Bounds { get { return GetBounds(); } set { SetBounds(value); } }
		public HeaderBoxType BoxType { get; private set; }
		public int ModelIndex { get; private set; }
		public int Width { get { return bounds.Width; } set { bounds.Width = value; } }
		public int Height { get { return bounds.Height; } set { bounds.Height = value; } }
		internal HeaderTextBox Previous { get { return previous; } }
		internal bool HasZeroPrevious { get { return previous != null && previous.ModelIndex != ModelIndex - 1; } }
		public HeaderBoxSelectType SelectType { get { return selectType; } set { selectType = value; } }
		#endregion
		void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		Rectangle GetBounds() {
			Point location = bounds.Location;
			if (previous != null) {
				if (BoxType == HeaderBoxType.ColumnHeader)
					location.X = previous.Bounds.Right;
				if (BoxType == HeaderBoxType.RowHeader)
					location.Y = previous.Bounds.Bottom;
			}
			return new Rectangle(location, bounds.Size);
		}
		internal HeaderTextBox GetZeroPrevious(Worksheet sheet) {
			if (BoxType == HeaderBoxType.ColumnHeader)
				return GetColumnZeroPrevious(sheet);
			else if (BoxType == HeaderBoxType.RowHeader)
				return GetRowZeroPrevious(sheet);
			return null;
		}
		HeaderTextBox GetColumnZeroPrevious(Worksheet sheet) {
			int columnIndex = sheet.Columns.TryGetColumnIndex(ModelIndex - 1);
			if (columnIndex >= 0)
				return CreateZeroPrevious(ModelIndex - 1, new Rectangle(this.Bounds.Location, new Size(0, this.Bounds.Height)), GetColumnText(sheet));
			return null;
		}
		HeaderTextBox GetRowZeroPrevious(Worksheet sheet) {
			int rowIndex = sheet.Rows.TryGetRowIndex(ModelIndex - 1);
			if (rowIndex >= 0)
				return CreateZeroPrevious(ModelIndex - 1, new Rectangle(this.Bounds.Location, new Size(this.Bounds.Width, 0)), ModelIndex.ToString());
			return null;
		}
		HeaderTextBox CreateZeroPrevious(int modelIndex, Rectangle newBounds, string text) {
			HeaderTextBox newPrevious = new HeaderTextBox(newBounds, this.BoxType, modelIndex, previous, parentCollection);
			newPrevious.Text = text;
			parentCollection.Insert(parentCollection.IndexOf(this), newPrevious);
			this.previous = newPrevious;
			return newPrevious;
		}
		string GetColumnText(Worksheet sheet) {
			if (sheet.Workbook.Properties.UseR1C1ReferenceStyle)
				return ModelIndex.ToString();
			return CellReferenceParser.ColumnIndexToString(ModelIndex - 1);
		}
	}
	#endregion
	#region HeaderBoxType
	public enum HeaderBoxType {
		ColumnHeader = 0,
		RowHeader = 1,
		SelectAllButton = 2
	}
	#endregion
	#region HeaderBoxSelectType
	public enum HeaderBoxSelectType {
		None = 0,
		Active = 1,
		Single = 2,
		Interval = 3
	}
	#endregion
	public abstract class PageBorderCollection {
		int gridIndex;
		List<BorderLineBox> boxes;
		protected PageBorderCollection(int gridIndex, List<BorderLineBox> boxes) {
			Guard.ArgumentNotNull(boxes, "boxes");
			this.gridIndex = gridIndex;
			this.boxes = boxes;
		}
		public int GridIndex { get { return gridIndex; } }
		public List<BorderLineBox> Boxes { get { return boxes; } }
		public abstract Rectangle GetBounds(Page page, BorderLineBox box);
	}
	public class PageHorizontalBorders : PageBorderCollection {
		public PageHorizontalBorders(int gridIndex, List<BorderLineBox> boxes)
			: base(gridIndex, boxes) {
		}
		public override Rectangle GetBounds(Page page, BorderLineBox box) {
			return box.GetHorizontalBounds(page, this);
		}
	}
	public class PageVerticalBorders : PageBorderCollection {
		public PageVerticalBorders(int gridIndex, List<BorderLineBox> boxes)
			: base(gridIndex, boxes) {
		}
		public override Rectangle GetBounds(Page page, BorderLineBox box) {
			return box.GetVerticalBounds(page, this);
		}
	}
}
