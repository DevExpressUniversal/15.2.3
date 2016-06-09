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
using System.Drawing;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Layout;
using DevExpress.Office.Layout.Export;
using DevExpress.Office.Printing;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using ModelMargins = DevExpress.XtraSpreadsheet.Model.Margins;
using ModelPageOrientation = DevExpress.XtraSpreadsheet.Model.ModelPageOrientation;
using LayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
using DevExpress.Export.Xl;
#if !SL
using System.Drawing.Printing;
using PrintMargins = System.Drawing.Printing.Margins;
using System.Drawing.Imaging;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
#else
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using PrintMargins = DevExpress.Xpf.Drawing.Printing.Margins;
using DevExpress.Xpf.Drawing.Printing;
#endif
namespace DevExpress.XtraSpreadsheet.Printing {
	#region PrintingDocumentExporter
	public partial class PrintingDocumentExporter : IDocumentModelSkinColorProvider {
		readonly DocumentModel documentModel;
		readonly Dictionary<int, BrickStringFormat> stringFormats;
		readonly ICellErrorTextProvider errorTextProvider = new PrintingCellErrorTextProvider();
		PrintingSystemBase printingSystem;
		DocumentLayout documentLayout;
		TransformationBrick currentContainer;
		TransformationBrick currentObjectsContainer;
		Point offset;
		Rectangle bounds;
		Point currentPageOffset;
		public PrintingDocumentExporter(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.stringFormats = new Dictionary<int, BrickStringFormat>();
			PopulateStringFormats();
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected internal PrintingSystemBase PrintingSystem { get { return printingSystem; }  }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		protected virtual Point Offset { get { return offset; } }
		public Color SkinGridlineColor { get { return DXColor.Empty; } }
		public Color SkinForeColor { get { return DXColor.Black; } }
		public Color SkinBackColor { get { return DXColor.Transparent; } }
#if SL
		BrickStringFormat CreateStringFormat(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			StringFormatFlags flags = StringFormatFlags.NoClip | StringFormatFlags.MeasureTrailingSpaces;
			if (!wrapText)
				flags |= StringFormatFlags.NoWrap;
			return new BrickStringFormat(flags,
				ConvertHorizontalAlignment(horizontalAlignment),
				ConvertVerticalAlignment(verticalAlignment));
		}
#else
		static StringFormat CreateStringFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoClip  | StringFormatFlags.MeasureTrailingSpaces;
			result.FormatFlags &= ~StringFormatFlags.LineLimit;
			return result;
		}
		BrickStringFormat CreateStringFormat(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			StringFormat format = CreateStringFormat();
			format.Alignment = ConvertHorizontalAlignment(horizontalAlignment);
			format.LineAlignment = ConvertVerticalAlignment(verticalAlignment);
			if (!wrapText)
				format.FormatFlags |= StringFormatFlags.NoWrap;
			return new BrickStringFormat(format);
		}
#endif
		void PopulateStringFormats() {
			for (int wrapText = 0; wrapText <= 1; wrapText++) {
				for (XlHorizontalAlignment i = XlHorizontalAlignment.General; i <= XlHorizontalAlignment.Distributed; i++) {
					for (XlVerticalAlignment j = XlVerticalAlignment.Top; j <= XlVerticalAlignment.Distributed; j++) {
						int hash = CalculateAlignmentHash(i, j, wrapText != 0);
						BrickStringFormat stringFormat = CreateStringFormat(i, j, wrapText != 0);
						stringFormats.Add(hash, stringFormat);
					}
				}
			}
		}
		int CalculateAlignmentHash(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			return ((int)horizontalAlignment) << 3 | (int)verticalAlignment | ((wrapText ? 0 : 1) << 6);
		}
		StringAlignment ConvertHorizontalAlignment(XlHorizontalAlignment horizontalAlignment) {
			switch (horizontalAlignment) {
				default:
				case XlHorizontalAlignment.General:
				case XlHorizontalAlignment.Fill:
				case XlHorizontalAlignment.Distributed:
				case XlHorizontalAlignment.Justify:
				case XlHorizontalAlignment.Left:
					return StringAlignment.Near;
				case XlHorizontalAlignment.CenterContinuous:
				case XlHorizontalAlignment.Center:
					return StringAlignment.Center;
				case XlHorizontalAlignment.Right:
					return StringAlignment.Far;
			}
		}
		StringAlignment ConvertVerticalAlignment(XlVerticalAlignment verticalAlignment) {
			switch (verticalAlignment) {
				default:
				case XlVerticalAlignment.Justify:
				case XlVerticalAlignment.Distributed:
				case XlVerticalAlignment.Bottom:
					return StringAlignment.Far;
				case XlVerticalAlignment.Center:
					return StringAlignment.Center;
				case XlVerticalAlignment.Top:
					return StringAlignment.Near;
			}
		}
		public void Export(DocumentLayout documentLayout, PrintingSystemBase printingSystem) {
			this.printingSystem = printingSystem;
			this.documentLayout = documentLayout;
			PrintingSystem.Graph.DefaultBrickStyle.BorderWidth = 0;
			SetupDefaultPageSettings();
			ExportCore();
		}
		void ExportCore() {
			IList<LayoutPage> pages = documentLayout.Pages;
			int pageCount = pages.Count;
			for (int i = 0; i < pages.Count; i++)
				ExportPage(pages[i], i, pageCount);
		}
		protected internal virtual void SetupDefaultPageSettings() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			Worksheet sheet = DocumentModel.Sheets.First;
			ModelMargins sectionMargins = sheet.Margins;
			PrintMargins margins = new PrintMargins(
				unitConverter.ModelUnitsToHundredthsOfInch(sectionMargins.Left),
				unitConverter.ModelUnitsToHundredthsOfInch(sectionMargins.Right),
				unitConverter.ModelUnitsToHundredthsOfInch(Math.Abs(sectionMargins.Top)),
				unitConverter.ModelUnitsToHundredthsOfInch(Math.Abs(sectionMargins.Bottom))
			);
#if SL
			PaperKind paperKind = (PaperKind)sheet.PrintSetup.PaperKind;
#else
			PaperKind paperKind = sheet.PrintSetup.PaperKind;
#endif
			Size pageSize = PaperSizeCalculator.CalculatePaperSize(sheet.PrintSetup.PaperKind);
			pageSize = Units.TwipsToHundredthsOfInch(pageSize);
			PrintingSystem.PageSettings.Assign(margins, new PrintMargins(0, 0, 0, 0), paperKind, pageSize, sheet.PrintSetup.Orientation == ModelPageOrientation.Landscape);
		}
		PrintMargins CalculateMargins(LayoutPage page) {
			Rectangle originalPageBounds = page.Bounds;
			Rectangle originalPageClientBounds = page.ClientBounds;
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			PrintMargins margins = new PrintMargins(
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageClientBounds.Left - originalPageBounds.Left),
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageBounds.Right - originalPageClientBounds.Right),
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageClientBounds.Top - originalPageBounds.Top),
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageBounds.Bottom - originalPageClientBounds.Bottom)
			);
			return margins;
		}
		protected internal virtual PaperKindInfo CalculateActualPaperKind(LayoutPage page) {
			Worksheet sheet = page.Sheet;
#if SL
			PaperKind paperKind = (PaperKind)sheet.PrintSetup.PaperKind;
#else
			PaperKind paperKind = sheet.PrintSetup.PaperKind;
#endif
			PaperKindInfo result = new PaperKindInfo(paperKind, paperKind != PaperKind.Custom ? sheet.PrintSetup.Orientation == ModelPageOrientation.Landscape : false);
			return result;
		}
		Rectangle CalculateActualPageRectangle(Rectangle bounds, bool landscape) {
			if (landscape)
				return new Rectangle(bounds.Y, bounds.X, bounds.Height, bounds.Width);
			else
				return bounds;
		}
		void SetPageContentAlgorithm() {
			DocumentBand docBand = ((PSDocument)this.PrintingSystem.Document).ActiveBand;
			IPageContentService serv = ((IServiceProvider)this.PrintingSystem).GetService(typeof(IPageContentService)) as IPageContentService;
			if (serv != null && docBand != null) {
				serv.SetAlgorithm(docBand, new SimplePageContentAlgorithm());
			}
		}
		void CreateNewDetail() {
			printingSystem.Graph.Modifier = BrickModifier.None;
			printingSystem.Graph.Modifier = BrickModifier.Detail;
		}
		public virtual void ExportPage(LayoutPage page, int pageIndex, int totalPages) {
			currentPageOffset = new Point(-page.GridColumns.ActualFirst.Near, -page.GridRows.ActualFirst.Near);
			PaperKindInfo paperKindInfo = CalculateActualPaperKind(page);
			Rectangle pageBounds = CalculateActualPageRectangle(page.Bounds, paperKindInfo.Landscape);
			CreateNewDetail();
			PrintMargins margins = CalculateMargins(page);
			Size pageSize = DocumentModel.LayoutUnitConverter.LayoutUnitsToHundredthsOfInch(pageBounds.Size);
			printingSystem.InsertPageBreak(0, margins, paperKindInfo.PaperKind, pageSize, paperKindInfo.Landscape);
			SetPageContentAlgorithm();
			this.Bounds = page.Bounds;
			this.offset = new Point(-page.ClientBounds.X, -page.ClientBounds.Y);
			UpdateCurrentHeaderFooterLayout(page.Sheet, totalPages);
			DrawHeader(pageIndex);
			TransformationBrick oldContainer = currentContainer;
			try {
				this.currentContainer = DrawCurrentContainer(page);
				TransformationBrick oldBorderContainer = currentBorderContainer;
				try {
					ReadonlyPageData pageData = new ReadonlyPageData(margins, new PrintMargins(0, 0, 0, 0), paperKindInfo.PaperKind, PageSizeInfo.GetPageSize(paperKindInfo.PaperKind, pageSize), paperKindInfo.Landscape);
					currentBorderContainer = DrawBordersContainer(pageData);
					TransformationBrick oldObjectsContainer = currentObjectsContainer;
					try {
						currentObjectsContainer = DrawObjectsContainer(pageData);
						ExportPageBase(page);
					}
					finally {
						currentObjectsContainer = oldObjectsContainer;
					}
				}
				finally {
					currentBorderContainer = oldBorderContainer;
				}
			}
			finally {
				currentContainer = oldContainer;
			}
			DrawFooter(pageIndex);
		}
		TransformationBrick DrawCurrentContainer(LayoutPage page) {
			TransformationBrick result = new TransformationBrick();
			result.NoClip = false; 
			Size size = new Size(page.GridColumns.Last.Far - page.GridColumns.First.Near, page.GridRows.Last.Far - page.GridRows.First.Near);
			Point location = new Point(page.GridColumns.ActualFirst.Near, page.GridRows.ActualFirst.Near);
			if (page.Sheet.PrintSetup.PrintHeadings) {
				size.Width += page.HeadingSize.Width;
				size.Height += page.HeadingSize.Height;
				location.Offset(-page.HeadingSize.Width, -page.HeadingSize.Height);
				currentPageOffset.Offset(page.HeadingSize.Width, page.HeadingSize.Height);
			}
			result.Rect = DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(new Rectangle(location, size));
			printingSystem.Graph.DrawBrick(result);
			return result;
		}
		TransformationBrick DrawBordersContainer(ReadonlyPageData pageData) {
			TransformationBrick result = new TransformationBrick();
			result.NoClip = true; 
			result.Rect = new RectangleF(PointF.Empty, pageData.UsefulPageRectF.Size);
			result.IsHitTestAllowed = false;
			printingSystem.Graph.DrawBrick(result);
			return result;
		}
		TransformationBrick DrawObjectsContainer(ReadonlyPageData pageData) {
			TransformationBrick result = new TransformationBrick();
			result.NoClip = false; 
			result.Rect = new RectangleF(PointF.Empty, pageData.UsefulPageRectF.Size);
			result.IsHitTestAllowed = false;
			printingSystem.Graph.DrawBrick(result);
			return result;
		}
		protected internal virtual void ExportPageBase(LayoutPage page) {
			ExportDefaultCellBackground(page);
			ExportPageBorders(page, page.VerticalGridBorders);
			ExportPageBorders(page, page.HorizontalGridBorders);
			ExportCellsBackground(page);
			ExportPageBorders(page, page.VerticalBorders);
			ExportPageBorders(page, page.HorizontalBorders);
			ExportCellsContent(page);
			ExportHeadingsContent(page);
			ExportDrawingBoxes(page);
		}
		void ExportDefaultCellBackground(LayoutPage page) {
		}
		void ExportCellsBackground(LayoutPage page) {
			IList<SingleCellTextBox> singleBoxes = page.Boxes;
			int count = singleBoxes.Count;
			for (int i = 0; i < count; i++)
				ExportCellBackground(page, singleBoxes[i], true);
			IList<ComplexCellTextBox> complexBoxes = page.ComplexBoxes;
			count = complexBoxes.Count;
			for (int i = 0; i < count; i++)
				ExportCellBackground(page, complexBoxes[i], false);
		}
		void ExportCellsContent(LayoutPage page) {
			IList<SingleCellTextBox> singleBoxes = page.Boxes;
			int count = singleBoxes.Count;
			for (int i = 0; i < count; i++)
				ExportCellTextBox(page, singleBoxes[i], true);
			IList<ComplexCellTextBox> complexBoxes = page.ComplexBoxes;
			count = complexBoxes.Count;
			for (int i = 0; i < count; i++)
				ExportCellTextBox(page, complexBoxes[i], false);
		}
		void ExportHeadingsContent(LayoutPage page) {
			if (!page.Sheet.PrintSetup.PrintHeadings)
				return;
			if (page.HeadingSize.Height != 0)
				ExportColumnHeadingsContent(page);
			if (page.HeadingSize.Width != 0)
				ExportRowHeadingsContent(page);
		}
		void ExportColumnHeadingsContent(LayoutPage page) {
			PanelBrick columnHeaderClipBrick = new PanelBrick();
			columnHeaderClipBrick.SeparableHorz = false;
			columnHeaderClipBrick.SeparableVert = false;
			columnHeaderClipBrick.NoClip = true;
			columnHeaderClipBrick.BackColor = DXColor.Transparent;
			columnHeaderClipBrick.BorderWidth = 0;
			Rectangle clipBrickBounds = new Rectangle(page.HeadingSize.Width, 0, page.GridColumns.ActualLast.Far - page.GridColumns.ActualFirst.Near, page.HeadingSize.Height);
			VisualBrickHelper.InitializeBrick(columnHeaderClipBrick, PrintingSystem, clipBrickBounds);
			AddBrickToCurrentPage(columnHeaderClipBrick);
			int x = 0;
			int width = 0;
			for (int column = page.GridColumns.ActualFirst.Index; column <= page.GridColumns.ActualLast.Index; column++) {
				PageGridItem item = page.GridColumns[column];
				x = x + width;
				width = item.Extent;
				Rectangle bounds = new Rectangle(x, 0, width, page.HeadingSize.Height);
				bounds = DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds);
				OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
				VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
				brick.BorderColor = DXColor.Black;
				brick.BorderStyle = BrickBorderStyle.Center;
				brick.BorderWidth = 1f;
				brick.BorderDashStyle = BorderDashStyle.Solid;
				brick.Sides = BorderSide.Right | BorderSide.Top | BorderSide.Left | BorderSide.Bottom;
				brick.BackColor = DXColor.Transparent;
				brick.StringFormat = BrickStringFormat.Create(TextAlignment.MiddleCenter, false);
				brick.Text = GetColumnText(page.Sheet, item.ModelIndex);
				brick.NoClip = true;
				columnHeaderClipBrick.Bricks.Add(brick);
			}
		}
		void ExportRowHeadingsContent(LayoutPage page) {
			PanelBrick rowHeaderClipBrick = new PanelBrick();
			rowHeaderClipBrick.SeparableHorz = false;
			rowHeaderClipBrick.SeparableVert = false;
			rowHeaderClipBrick.NoClip = true;
			rowHeaderClipBrick.BackColor = DXColor.Transparent;
			rowHeaderClipBrick.BorderWidth = 0;
			Rectangle clipBrickBounds = new Rectangle(0, page.HeadingSize.Height, page.HeadingSize.Width, page.GridRows.ActualLast.Far - page.GridRows.ActualFirst.Near);
			VisualBrickHelper.InitializeBrick(rowHeaderClipBrick, PrintingSystem, clipBrickBounds);
			AddBrickToCurrentPage(rowHeaderClipBrick);
			int y = 0;
			int height = 0;
			for (int row = page.GridRows.ActualFirst.Index; row <= page.GridRows.ActualLast.Index; row++) {
				PageGridItem item = page.GridRows[row];
				y = y + height;
				height = item.Extent;
				Rectangle bounds = new Rectangle(0, y, page.HeadingSize.Width, height);
				bounds = DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds);
				OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
				VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
				brick.BorderColor = DXColor.Black;
				brick.BorderStyle = BrickBorderStyle.Center;
				brick.BorderWidth = 1f;
				brick.BorderDashStyle = BorderDashStyle.Solid;
				brick.Sides = BorderSide.Right | BorderSide.Top | BorderSide.Bottom | BorderSide.Left;
				brick.BackColor = DXColor.Transparent;
				brick.StringFormat = BrickStringFormat.Create(TextAlignment.MiddleCenter, false);
				brick.Text = (item.ModelIndex + 1).ToString();
				brick.NoClip = true;
				rowHeaderClipBrick.Bricks.Add(brick);
			}
		}
		string GetColumnText(Worksheet sheet, int modelIndex) {
			if (sheet.Workbook.Properties.UseR1C1ReferenceStyle)
				return modelIndex.ToString();
			return CellReferenceParser.ColumnIndexToString(modelIndex);
		}
		void ExportDrawingBoxes(LayoutPage page) {
			foreach (DrawingBox drawingBox in page.DrawingBoxes)
				ExportDrawingBox(drawingBox);
		}
		protected Rectangle CorrectTextDrawingBounds(LayoutPage page, ICellTextBox cellTextBox, Rectangle textBounds) {
			ICell cell = cellTextBox.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			int free = cell.ActualFont.GetFontInfo().Free;
			Rectangle correctedTextBounds = textBounds;
			correctedTextBounds.Y += free;
			return correctedTextBounds;
		}
		protected internal virtual  Rectangle GetDrawingBounds(Rectangle bounds) {
			Rectangle boundsInLayoutUnits = bounds;
			boundsInLayoutUnits.Offset(Offset);
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(boundsInLayoutUnits);
		}
		void ExportCellBackground(LayoutPage page, ICellTextBox box, bool single) {
			ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			Color backColor = cell.ActualBackgroundColor;
			if (DXColor.IsTransparentOrEmpty(backColor))
				return;
			OfficeRectBrick brick = new OfficeRectBrick(DocumentModel.LayoutUnitConverter);
			Rectangle brickBounds = GetDrawingBounds(box.GetBounds(page));
			if (single)
				brickBounds.Offset(currentPageOffset);
			else {
				brickBounds.X = Math.Max(brickBounds.X, page.HeadingSize.Width);
				brickBounds.Y = Math.Max(brickBounds.Y, page.HeadingSize.Height);
			}
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, brickBounds);
			brick.BackColor = backColor;
			brick.NoClip = !(box is ComplexCellTextBox);
			AddBrickToCurrentPage(brick);
		}
		void ExportDrawingBox(DrawingBox box) {
			PanelBrick clipBrick = new PanelBrick();
			clipBrick.SeparableHorz = false;
			clipBrick.SeparableVert = false;
			clipBrick.NoClip = false;
			clipBrick.BackColor = DXColor.Transparent;
			clipBrick.BorderWidth = 0;
			VisualBrickHelper.InitializeBrick(clipBrick, PrintingSystem, GetDrawingBounds(box.ClipBounds));
			AddObjectBrickToCurrentPage(clipBrick);
			OfficeImageBrick brick = new OfficeImageBrick(DocumentModel.LayoutUnitConverter);
			RectangleF brickBounds = DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(box.Bounds);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, brickBounds);
			brick.Image = GetImage(box);
			brick.BackColor = DXColor.Transparent;
			brick.SizeMode = box.NoChangeAspect ? ImageSizeMode.ZoomImage : ImageSizeMode.StretchImage;
			clipBrick.Bricks.Add(brick);
		}
		Image GetImage(DrawingBox box) {
			PictureBox pictureBox = box as PictureBox;
			if (pictureBox != null)
				return GetImage(pictureBox.Picture, pictureBox.Bounds);
			ChartBox chartBox = box as ChartBox;
			if (chartBox != null)
				return GetImage(chartBox.Chart, chartBox.Bounds);
			return null;
		}
#if !SL
		public static Image GetImage(Image image) {
			return (Image)image.Clone();
		}
		Image GetImage(Picture picture, Rectangle bounds) {
			IPictureImageService service = picture.DocumentModel.GetService<IPictureImageService>();
			if (service == null)
				return GetImage(picture.Image.NativeImage);
			return service.GetImage(picture, bounds);
		}
		public static Image GetImage(Chart chart, Rectangle bounds) {
			IChartImageService service = chart.DocumentModel.GetService<IChartImageService>();
			if (service == null)
				return null;
			return service.GetImage(chart, bounds);
		}
#else
		public static Image GetImage(System.Windows.Controls.Image image) {
			byte[] brickImageBytes = DevExpress.Data.Printing.Native.DxDibImageConverter.Encode(image, false);
			return Image.FromStream(new System.IO.MemoryStream(brickImageBytes));
		}
		Image GetImage(Picture picture, Rectangle bounds) {
			return GetImage(picture.Image.NativeImage);
		}
		public static Image GetImage(Chart chart, Rectangle bounds) {
			return null;
		}
#endif
		void ExportCellTextBox(LayoutPage page, ICellTextBox box, bool single) {
			PanelBrick clipBrick = new PanelBrick();
			clipBrick.SeparableHorz = false;
			clipBrick.SeparableVert = false;
			clipBrick.NoClip = false;
			clipBrick.BackColor = DXColor.Transparent;
			clipBrick.BorderWidth = 0;
			Rectangle clipBrickBounds = GetDrawingBounds(box.GetClipBounds(page));
			clipBrickBounds.Offset(currentPageOffset);
			VisualBrickHelper.InitializeBrick(clipBrick, PrintingSystem, clipBrickBounds);
			AddBrickToCurrentPage(clipBrick);
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
			Rectangle bounds = CorrectTextDrawingBounds(page, box, box.GetTextBounds(page, this.documentLayout));
			RectangleF brickBounds = GetDrawingBounds(bounds);
			if (single)
				brickBounds.Offset(currentPageOffset);
			brickBounds.X -= clipBrick.Rect.X;
			brickBounds.Y -= clipBrick.Rect.Y;
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, brickBounds);
			CellForegroundDisplayFormat displayFormat = box.CalculateForegroundDisplayFormat(page, errorTextProvider, true, this, true);
			ICell cell = displayFormat.Cell;
			ConditionalFormattingPrinter condFmtPrinter = new ConditionalFormattingPrinter(cell, clipBrick, Rectangle.Round(brickBounds));
			IConditionalFormattingPainters condFmts = cell.Worksheet.ConditionalFormattings.GetPainters(cell, cell.ConditionalFormattingStoppedAtPriority);
			condFmtPrinter.Visit(condFmts.DataBar);
			condFmtPrinter.Visit(condFmts.IconSet);
			brick.BackColor = DXColor.Transparent;
			if (condFmts.ShowValues) {
				string text = displayFormat.Text;
				BrickStringFormat brickStringFormat = GetCellStringFormat(displayFormat.Cell);
				if ((brickStringFormat.FormatFlags & StringFormatFlags.NoWrap) != 0)
					text = SingleLineTextHelper.GetSingleLine(text);
				brick.ForeColor = displayFormat.ForeColor;
				brick.Text = text;
				ModelHyperlink hyperlink = displayFormat.Hyperlink;
				if (hyperlink != null) {
					brick.Url = hyperlink.TargetUri;
					brick.Hint = hyperlink.TooltipText;
				}
				Font font = displayFormat.FontInfo.Font;
				if (page.ScaleFactor == 1)
					brick.Font = font;
				else
					brick.Font = new Font(font.FontFamily, font.SizeInPoints * page.ScaleFactor, font.Style);
				brick.StringFormat = brickStringFormat;
			}
			brick.NoClip = true; 
			clipBrick.Bricks.Add(brick);
		}
		BrickStringFormat GetCellStringFormat(ICell cell) {
			IActualCellAlignmentInfo alignment = cell.ActualAlignment;
			return GetStringFormat(cell.ActualHorizontalAlignment, alignment.Vertical, alignment.WrapText);
		}
		BrickStringFormat GetStringFormat(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment, bool wrapText) {
			int hash = CalculateAlignmentHash(horizontalAlignment, verticalAlignment, wrapText);
			return stringFormats[hash];
		}
		protected internal virtual void AddBrickToCurrentPage(Brick brick) {
			currentContainer.Bricks.Add(brick);
		}
		protected internal virtual void AddObjectBrickToCurrentPage(Brick brick) {
			currentObjectsContainer.Bricks.Add(brick);
		}
	}
	#endregion
}
