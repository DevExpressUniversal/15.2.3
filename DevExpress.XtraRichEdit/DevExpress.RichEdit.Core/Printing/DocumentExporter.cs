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
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Services;
using System.Collections;
using System.Globalization;
using DevExpress.Office.Utils.Internal;
using Debug = System.Diagnostics.Debug;
#if !DXPORTABLE
using DevExpress.XtraRichEdit.Printing;
using DevExpress.Office.Layout.Export;
using DevExpress.Office.Printing;
#endif
#if !SL
using System.Drawing.Printing;
using DevExpress.XtraPrinting.BrickExporters;
using System.Drawing.Drawing2D;
#if !DXPORTABLE
using DevExpress.XtraPrinting.XamlExport;
using System.Diagnostics;
#endif
#else
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Drawing.Printing;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
#if !DXPORTABLE
	#region PrintingDocumentExporter
	public class PrintingDocumentExporter : BoundedDocumentLayoutExporter {
		#region Fields
		PrintingSystemBase printingSystem;
		readonly BrickStringFormat sf;
		Point offset;
		PointF textBoxOffset;
		ICharacterLinePainter horizontalPrinterLinePainter;
		ICharacterLinePainter verticalPrinterLinePainter;
		readonly Dictionary<TableCellViewInfo, OfficePanelBrick> cellBricks;
		bool suppressPutIntoTableCellBrick;
		PanelBrick currentContainer;
		int commentIndex;
		bool startFromNotFirstPage = false;
		Page currentPage;   
		#endregion
		public PrintingDocumentExporter(DocumentModel documentModel, TextColors textColors)
			: base(documentModel, Rectangle.Empty, new EmptyPatternLinePaintingSupport(), textColors) {
			this.sf = CreateBrickStringFormat();
			this.sf.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			this.horizontalPrinterLinePainter = new HorizontalPrinterCharacterLinePainter(this);
			this.verticalPrinterLinePainter = new VerticalPrinterCharacterLinePainter(this);
			this.cellBricks = new Dictionary<TableCellViewInfo, OfficePanelBrick>();
		}
		#region Properties
		protected override Point Offset { get { return offset; } }
		protected PointF TextBoxOffset { get { return textBoxOffset; } set { textBoxOffset = value; } }
		protected internal PrintingSystemBase PrintingSystem { get { return printingSystem; } set { printingSystem = value; } }
		protected internal ICharacterLinePainter HorizontalPrinterLinePainter { get { return horizontalPrinterLinePainter; } }
		protected internal ICharacterLinePainter VerticalPrinterLinePainter { get { return verticalPrinterLinePainter; } }
		public override Painter Painter {
			get { throw new NotImplementedException(); }
		}
		int CommentIndex { get { return commentIndex; } set { commentIndex = value; } }
		protected internal bool StartFromNotFirstPage { get { return startFromNotFirstPage; } set { startFromNotFirstPage = value; } }
		internal Page CurrentPage { get { return currentPage; } set { currentPage = value; } }
		internal override bool CanExportFieldHighlightAreas { get { return false; } }
		internal override bool CanExportRangePermissionHighlightAreas { get { return false; } }
		#endregion
#if SL
		static BrickStringFormat CreateBrickStringFormat() {
			return new BrickStringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
		}
#else
		static BrickStringFormat CreateBrickStringFormat() {
			return new BrickStringFormat(CreateStringFormat());
		}
		static StringFormat CreateStringFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
			result.FormatFlags &= ~StringFormatFlags.LineLimit;
			return result;
		}
#endif
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override RichEditPatternLinePainter CreateHorizontalLinePainter(IPatternLinePaintingSupport linePaintingSupport) {
			return new RichEditHorizontalPatternLinePainter(linePaintingSupport, DocumentModel.LayoutUnitConverter);
		}
		protected internal override RichEditPatternLinePainter CreateVerticalLinePainter(IPatternLinePaintingSupport linePaintingSupport) {
			return new RichEditVerticalPatternLinePainter(linePaintingSupport, DocumentModel.LayoutUnitConverter);
		}
		public override void ExportCustomRunBox(CustomRunBox box) {
			ICustomRunBoxLayoutExporterService service = DocumentModel.GetService<ICustomRunBoxLayoutExporterService>();
			if (service != null) {
				var brick = service.ExportToPrintingSystem(PieceTable, PrintingSystem, box);
				VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(box.Bounds));
				AddBrickToCurrentPage(brick);
			}
		}
		protected override void ExportRowBookmarkBoxes() {
			BookmarkBoxCollection boxes = CurrentRow.InnerBookmarkBoxes;
			if (boxes == null)
				return;
			int lastIndex = boxes.Count - 1;
			for (int i = 0; i <= lastIndex; i++)
				boxes[i].ExportTo(this);
		}
		public override void ExportBookmarkStartBox(VisitableDocumentIntervalBox box) {
			BookmarkPrintControl brickOwner = new BookmarkPrintControl();
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter, brickOwner);
			Rectangle bounds = new Rectangle(CurrentRow.Bounds.Location, new Size(0, CurrentRow.Bounds.Height));
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(bounds));
			Bookmark bookmarkInfo = box.Interval as Bookmark;
			brick.Text = String.Empty;
			brick.AnchorName = bookmarkInfo != null ? bookmarkInfo.Name : String.Empty;
			brick.ForeColor = DXColor.Empty;
			brick.BackColor = DXColor.Empty;
			brick.Font = null;
			brick.StringFormat = sf;
			brick.NoClip = true;
			brickOwner.IsTarget = true;
			brickOwner.Name = brick.AnchorName;
			AddBrickToCurrentPage(brick);
		}
		public void Export(DocumentLayout documentLayout, PrintingSystemBase printingSystem) {
			this.printingSystem = printingSystem;
			PrintingSystem.Graph.DefaultBrickStyle.BorderWidth = 0;
			SetPageBackColor();
			SetupDefaultPageSettings();
			CommentIndex = 0;
		}
		protected internal virtual void SetPageBackColor() {
			Color pageBackColor = DocumentModel.DocumentProperties.PageBackColor;
			if (!DocumentModel.PrintingOptions.EnablePageBackgroundOnPrint)
				PrintingSystem.Graph.PageBackColor = DXColor.White;
			else
				PrintingSystem.Graph.PageBackColor = pageBackColor;
		}
		protected internal virtual void SetupDefaultPageSettings() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			Section section = DocumentModel.Sections.First;
			SectionMargins sectionMargins = section.Margins;
			Margins margins = new Margins(
				unitConverter.ModelUnitsToHundredthsOfInch(sectionMargins.Left),
				unitConverter.ModelUnitsToHundredthsOfInch(sectionMargins.Right),
				unitConverter.ModelUnitsToHundredthsOfInch(Math.Abs(sectionMargins.Top)),
				unitConverter.ModelUnitsToHundredthsOfInch(Math.Abs(sectionMargins.Bottom))
			);
			SectionPage sectionPage = section.Page;
			Size pageSize = new Size(sectionPage.Width, sectionPage.Height);
			pageSize = unitConverter.ModelUnitsToHundredthsOfInch(pageSize);
			PaperKindInfo paperKindInfo = CalculateActualPaperKind(sectionPage);
			Rectangle bounds = CalculateActualPageRectangle(new Rectangle(0, 0, pageSize.Width, pageSize.Height), paperKindInfo.Landscape);
			PrintingSystem.PageSettings.Assign(margins, new Margins(0, 0, 0, 0), paperKindInfo.PaperKind, bounds.Size, paperKindInfo.Landscape);
		}
		double CalculateScaleFactor(Page page, Size pageSize) {
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			page.EnsureCommentBounds(page, DocumentModel);
			int commentBoundsRight = unitConverter.LayoutUnitsToHundredthsOfInch(page.CommentBounds.Right);
			return (double)(pageSize.Width) / commentBoundsRight;
		}
		Margins CalculateScaleMargins(Margins margins, double scaleFactor) {
			return new Margins((int)(margins.Left * scaleFactor), (int)(margins.Right * scaleFactor), (int)(margins.Top * scaleFactor), (int)(margins.Bottom * scaleFactor));
		}
		Margins CalculateMargins(DevExpress.XtraRichEdit.Layout.Page page) {
			Rectangle originalPageBounds = page.Bounds;
			Rectangle originalPageClientBounds = page.ClientBounds;
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			Margins margins = new Margins(
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageClientBounds.Left - originalPageBounds.Left),
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageBounds.Right - originalPageClientBounds.Right),
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageClientBounds.Top - originalPageBounds.Top),
				unitConverter.LayoutUnitsToHundredthsOfInch(originalPageBounds.Bottom - originalPageClientBounds.Bottom)
			);
			return margins;
		}
		int prevPageSkipCount = 0;
		public override void ExportPage(DevExpress.XtraRichEdit.Layout.Page page, Action action) {
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			PaperKindInfo paperKindInfo = CalculateActualPaperKind(page);
			Rectangle pageBounds = CalculateActualPageRectangle(page.Bounds, paperKindInfo.Landscape);
			double scaleFactor = 1.0;
			for (int i = 0; i <= page.NumSkippedPages - prevPageSkipCount; i++) {
				if ((page.PageIndex > 0) || startFromNotFirstPage)
					CreateNewDetail();
				Margins margins = CalculateMargins(page);
				Size pageSize = unitConverter.LayoutUnitsToHundredthsOfInch(pageBounds.Size);
				if ((page.Comments.Count > 0) && (DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible)) {
					scaleFactor = CalculateScaleFactor(page, pageSize);
				}
				Margins scaleMargins = CalculateScaleMargins(margins, scaleFactor);
				printingSystem.InsertPageBreak(0, scaleMargins, paperKindInfo.PaperKind, pageSize, paperKindInfo.Landscape);
				SetPageContentAlgorithm();
				this.Bounds = page.Bounds;
				this.offset = new Point(-page.ClientBounds.X, -page.ClientBounds.Y);
				ExportPageCore(page, margins, paperKindInfo, pageSize, i < page.NumSkippedPages - prevPageSkipCount, null, scaleFactor, action);
				cellBricks.Clear();
			}
			prevPageSkipCount = page.NumSkippedPages;
		}
		protected internal void ExportPageCore(Page page, Margins margins, PaperKindInfo paperKindInfo, Size pageSize, bool exportEmptyPage, Rectangle? bounds, double scaleFactor, Action action) {
			PanelBrick oldContainer = currentContainer;
			try {
				PointF originPoint = currentContainer != null ? currentContainer.Rect.Location : PointF.Empty;
				currentContainer = new TransformationBrick();
				ReadonlyPageData pageData = new ReadonlyPageData(margins, new Margins(0, 0, 0, 0), paperKindInfo.PaperKind, PageSizeInfo.GetPageSize(paperKindInfo.PaperKind, pageSize), paperKindInfo.Landscape);
				SizeF containerSize = pageData.UsefulPageRectF.Size;
				PointF containerLocation = new PointF(0, 0);
				if (bounds != null) {
					currentContainer.NoClip = false;
					Rectangle textBoxBounds = bounds.GetValueOrDefault();
					containerSize = textBoxBounds.Size;
					Rectangle drawingBounds = GetDrawingBounds(textBoxBounds);
					containerLocation = new PointF(drawingBounds.Location.X, drawingBounds.Location.Y);
				}
				SizeF newSize = new SizeF(containerSize.Width, containerSize.Height);
				currentContainer.Rect = new RectangleF(new PointF(containerLocation.X - originPoint.X, containerLocation.Y - originPoint.Y), newSize);
				TextBoxOffset = currentContainer.Location;
				if (oldContainer == null)
					printingSystem.Graph.DrawBrick(currentContainer);
				else
					oldContainer.Bricks.Add(currentContainer);
				if (!exportEmptyPage && action != null)
					action();
				if (CurrentPage == null)
					currentContainer.Scale(scaleFactor);
			}
			finally {
				if (CurrentPage == null)
					currentContainer = oldContainer;
			}
		}
		protected internal override void AfterExportPageArea() {
			cellBricks.Clear();
		}
		void CreateNewDetail() {
			printingSystem.Graph.Modifier = BrickModifier.None;
			printingSystem.Graph.Modifier = BrickModifier.Detail;
		}
		void SetPageContentAlgorithm() {
			DocumentBand docBand = ((PSDocument)this.PrintingSystem.Document).ActiveBand;
			IPageContentService serv = ((IServiceProvider)this.PrintingSystem).GetService(typeof(IPageContentService)) as IPageContentService;
			if (serv != null && docBand != null) {
				serv.SetAlgorithm(docBand, new SimplePageContentAlgorithm());
			}
		}
		PanelBrick oldContainer;
		internal override bool BeginExportCompositeObject(Rectangle bounds, PieceTable pieceTable, ref RectangleF oldClipRect, ref RectangleF clipRect, ref PieceTable oldPieceTable) {
			oldPieceTable = PieceTable;
			PieceTable = pieceTable;
			if (CurrentPage != null) {
				this.oldContainer = currentContainer;
				SectionPage sectionPage = this.DocumentModel.Sections.First.Page;
				PaperKind paperKind = sectionPage.PaperKind;
				PaperKindInfo paperKindInfo = new PaperKindInfo(paperKind, paperKind != PaperKind.Custom ? sectionPage.Landscape : false);
				Margins margins = CalculateMargins(CurrentPage);
				Rectangle pageBounds = CalculateActualPageRectangle(CurrentPage.Bounds, paperKindInfo.Landscape);
				Size pageSize = DocumentModel.LayoutUnitConverter.LayoutUnitsToHundredthsOfInch(pageBounds.Size);
				ExportPageCore(CurrentPage, margins, paperKindInfo, pageSize, false, bounds, 1, null);
				currentContainer.Scale(1);
			}
			return true;
		}
		internal override void EndExportCompositeObject(RectangleF oldClipRect, PieceTable oldPieceTable) {
			if (CurrentPage != null) {
				currentContainer = this.oldContainer;
				TextBoxOffset = currentContainer.Location;
				this.oldContainer = null;
			}
			PieceTable = oldPieceTable;
		}
		protected internal virtual void ExportPageBase(DevExpress.XtraRichEdit.Layout.Page page) {
			base.ExportPage(page);
			if (ShouldExportComments(page)) {
				BeforeExportComments(page);
				ExportComments(page);
			}
		}
		protected internal override bool ShouldExportComments(DevExpress.XtraRichEdit.Layout.Page page) {
			return (page.Comments.Count > 0) && (DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible);
		}
		protected internal override void BeforeExportComments(DevExpress.XtraRichEdit.Layout.Page page) {
			Rectangle pageCommentBounds = CalculateNewLocationBounds(page.CommentBounds);
			if (!DocumentModel.PrintingOptions.EnableCommentBackgroundOnPrint)
				PrintRectangle(DXColor.Transparent, pageCommentBounds);
			else
				PrintRectangle(DXColor.LightGray, pageCommentBounds);
		}
		public void ExportComments(DevExpress.XtraRichEdit.Layout.Page page) {
			Rectangle pageCommentBounds = CalculateNewLocationBounds(page.CommentBounds);
			for (int i = 0; i < page.Comments.Count; i++)
				ExportComment(page.Comments[i], pageCommentBounds);
		}
		protected internal override void BeforeExportComment(CommentViewInfo commentViewInfo) {
			if (commentViewInfo.IsWholeContentVisible)
				CommentIndex += 1;
			Rectangle commentViewInfoBounds = CalculateNewLocationBounds(commentViewInfo.Bounds);
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			int radius = unitConverter.LayoutUnitsToHundredthsOfInch(50);
			Color fillColor = DocumentModel.CommentColorer.GetColor(commentViewInfo.Comment);
			Color borderColor = CommentOptions.SetBorderColor(fillColor);
			if (!DocumentModel.PrintingOptions.EnableCommentFillOnPrint)
				PrintRoudedRectangle(commentViewInfoBounds, DXColor.Transparent, borderColor, radius);
			else
				PrintRoudedRectangle(commentViewInfoBounds, fillColor, borderColor, radius);
			ExportCommentHeading(commentViewInfo);
		}
		protected internal override bool ShouldExportComment(CommentViewInfo commentViewInfo) {
			return !commentViewInfo.CommentContainTableInFirstRow;
		}
		protected internal override void AfterExportComment(CommentViewInfo commentViewInfo, Rectangle pageCommentBounds) {
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			int radius = unitConverter.LayoutUnitsToHundredthsOfInch(50);
			Color fillColor = DocumentModel.CommentColorer.GetColor(commentViewInfo.Comment);
			Color borderColor = CommentOptions.SetBorderColor(fillColor);
			if (commentViewInfo.Comment.ParentComment == null)
				ExportLines(commentViewInfo, pageCommentBounds, radius, borderColor);
			if (commentViewInfo.IsWholeContentVisible) {
				Rectangle commentViewInfoMoreButton = CalculateNewSizeBounds(commentViewInfo.CommentMoreButtonBounds);
				commentViewInfoMoreButton = CalculateNewLocationBounds(commentViewInfoMoreButton);
				PrintRoudedRectangle(commentViewInfoMoreButton, DXColor.White, DXColor.Black, radius);
				ExportCommentMoreButton(commentViewInfo);
			}
		}
		void ExportComment(CommentViewInfo commentViewInfo, Rectangle pageCommentBounds) {
			BeforeExportComment(commentViewInfo);
			if (ShouldExportComment(commentViewInfo))
				ExportComment(commentViewInfo);
			AfterExportComment(commentViewInfo, pageCommentBounds);
		}
		void PrintRoudedRectangle(Rectangle bounds, Color fillColor, Color borderColor, int radius) {
			OfficeRoundedRectangleBrick brick = new OfficeRoundedRectangleBrick(DocumentModel.LayoutUnitConverter, radius, fillColor);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
			brick.BackColor = DXColor.Transparent; ;
			brick.BorderColor = borderColor;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		public override void ExportComment(CommentViewInfo commentViewInfo) {
			PieceTable = commentViewInfo.Comment.Content.PieceTable;
			PanelBrick oldContainer = currentContainer;
			try {
				currentContainer = new OfficePanelBrick(DocumentModel.LayoutUnitConverter);
				Size containerSize = commentViewInfo.ContentBounds.Size;
				Point containerLocation = commentViewInfo.ContentBounds.Location;
				VisualBrickHelper.InitializeBrick(currentContainer, PrintingSystem, GetDrawingBounds(new Rectangle(containerLocation, containerSize)));
				oldContainer.Bricks.Add(currentContainer);
				currentContainer.NoClip = false;
				currentContainer.BackColor = DXColor.Empty;
				Point oldOffset = offset;
				offset = new Point(0, 0);
				base.ExportPage(commentViewInfo.CommentDocumentLayout.Pages[0]);
				offset = oldOffset;
			}
			finally {
				PieceTable = DocumentModel.MainPieceTable;
				currentContainer = oldContainer;
			}
		}
		internal override void BeginExportComment(CommentViewInfo commentViewInfo, ref PanelBrick oldContainer, ref Point oldOffset) {
			oldContainer = currentContainer;
			currentContainer = new OfficePanelBrick(DocumentModel.LayoutUnitConverter);
			Size containerSize = commentViewInfo.ContentBounds.Size;
			Point containerLocation = commentViewInfo.ContentBounds.Location;
			VisualBrickHelper.InitializeBrick(currentContainer, PrintingSystem, GetDrawingBounds(new Rectangle(containerLocation, containerSize)));
			oldContainer.Bricks.Add(currentContainer);
			currentContainer.NoClip = false;
			currentContainer.BackColor = DXColor.Empty;
			oldOffset = offset;
			offset = new Point(0, 0);
		}
		internal override void EndExportComment(PanelBrick oldContainer, Point oldOffset) {
			offset = oldOffset;
			currentContainer = oldContainer;
		}
		void ExportLines(CommentViewInfo commentViewInfo, Rectangle pageCommentBounds, int radius, Color lineColor) {
			Rectangle commentViewInfoBounds = CalculateNewLocationBounds(commentViewInfo.Bounds);
			Rectangle tightBounds = CalculateNewLocationBounds(commentViewInfo.Character.TightBounds);
			int x1 = tightBounds.Right;
			int y1 = tightBounds.Bottom;
			int x2 = pageCommentBounds.Left;
			int y2 = y1;
			PrintLine(lineColor, new Rectangle(x1, y1, (pageCommentBounds.Left - tightBounds.Right), 100), LineType.lTHorizontal);
			x1 = x2;
			x2 = commentViewInfoBounds.Left;
			y2 = commentViewInfoBounds.Top + radius;
			if (y2 >= y1)
				PrintLine(lineColor, new Rectangle(x1, y1, x2 - x1, y2 - y1), LineType.lTDown);
			else
				PrintLine(lineColor, new Rectangle(x1, y2, x2 - x1, y1 - y2), LineType.lTUp);
		}
		internal override Rectangle CalculateNewLocationBounds(Rectangle bounds) {
			return new Rectangle(new Point(bounds.X + offset.X, bounds.Location.Y + offset.Y), bounds.Size);
		}
		Rectangle CalculateNewSizeBounds(Rectangle bounds) {
			return new Rectangle(bounds.X - bounds.Width, bounds.Y - 15, bounds.Width * 2, bounds.Height + 15);
		}
		void PrintLine(Color color, Rectangle bounds, LineType lineType) {
			OfficeLineBrick brick = new OfficeLineBrick(DocumentModel.LayoutUnitConverter, lineType, color);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
			brick.BackColor = DXColor.Transparent;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		void ExportCommentHeading(CommentViewInfo commentViewInfo) {
			Rectangle commentHeadingBounds = CalculateNewLocationBounds(commentViewInfo.CommentHeadingBounds);
			Font font = commentViewInfo.CommentHeadingFontInfo.Font;
			string text = commentViewInfo.CommentHeading;
			ExportCommentHeadingCore(commentHeadingBounds, text, font);
		}
		void ExportCommentMoreButton(CommentViewInfo commentViewInfo) {
			Rectangle commentMoreButton = CalculateNewLocationBounds(commentViewInfo.CommentMoreButtonBounds);
			commentMoreButton = CalculateNewSizeBounds(commentMoreButton);
			Font font = commentViewInfo.CommentHeadingFontInfo.Font;
			string text = " ... [" + Convert.ToString(this.CommentIndex) + "]";
			ExportCommentHeadingCore(commentMoreButton, text, font);
		}
		void ExportCommentHeadingCore(Rectangle bounds, string text, Font font) {
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, (bounds));
			brick.Text = text;
			brick.BackColor = DXColor.Empty;
			brick.Font = font;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		protected internal virtual PaperKindInfo CalculateActualPaperKind(DevExpress.XtraRichEdit.Layout.Page page) {
			PageArea pageArea = page.Areas.First;
			if (pageArea == null)
				return PaperKindInfo.Default;
			Column column = pageArea.Columns.First;
			if (column == null)
				return PaperKindInfo.Default;
			Row row = column.Rows.First;
			if (row == null)
				return PaperKindInfo.Default;
			Paragraph paragraph = row.Paragraph;
			if (paragraph == null)
				return PaperKindInfo.Default;
			ParagraphIndex paragraphIndex = paragraph.Index;
			SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
			Section section = DocumentModel.Sections[sectionIndex];
			SectionPage sectionPage = section.Page;
			return CalculateActualPaperKind(sectionPage);
		}
		protected internal virtual PaperKindInfo CalculateActualPaperKind(SectionPage sectionPage) {
#if SL
			PaperKind paperKind = (PaperKind)sectionPage.PaperKind;
			PaperKindInfo result = new PaperKindInfo(paperKind, paperKind != PaperKind.Custom ? sectionPage.Landscape : false);
#else
			PaperKind paperKind = sectionPage.PaperKind;
			PaperKindInfo result;
			if (paperKind != PaperKind.Custom)
				result = new PaperKindInfo(paperKind, sectionPage.Landscape);
			else
				result = new PaperKindInfo(paperKind, sectionPage.Width > sectionPage.Height ? sectionPage.Landscape : false); 
#endif
			return result;
		}
		Rectangle CalculateActualPageRectangle(Rectangle bounds, bool landscape) {
			if (landscape)
				return new Rectangle(bounds.Y, bounds.X, bounds.Height, bounds.Width);
			else
				return bounds;
		}
		public override void ExportInlinePictureBox(InlinePictureBox box) {
			ExportPictureCore(box, GetNativeImage(box), GetDrawingBounds(box.Bounds), box.GetSizing(PieceTable), false, true);
		}
		Image GetNativeImage(InlinePictureBox box) {
			OfficeImage image = GetBoxImage(box);
			InternalOfficeImageHelper.EnsureLoadComplete(image);
#if !SL
			return (Image)image.NativeImage.Clone();
#else
			byte[] brickImageBytes = DevExpress.Data.Printing.Native.DxDibImageConverter.Encode(image.NativeImage, false);
			return Image.FromStream(new System.IO.MemoryStream(brickImageBytes));
#endif
		}
		protected virtual OfficeImage GetBoxImage(InlinePictureBox box) {
			return box.GetImage(PieceTable, ReadOnly);
		}
		void ExportPictureCore(Box box, Image image, Rectangle bounds, ImageSizeMode sizeMode, bool transparent, bool disposeImage) {
			OfficeImageBrick brick = new OfficeImageBrick(DocumentModel.LayoutUnitConverter);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
			brick.DisposeImage = disposeImage;
			brick.Image = image;
			brick.SizeMode = sizeMode;
			brick.NoClip = true;
			brick.BackColor = DXColor.Transparent;
			AssignHyperlinkUrl(box, brick);
			AddBrickToCurrentPage(brick);
		}
#if !SL
		public override void ExportFloatingObjectBox(FloatingObjectBox box) {
			Matrix transform = box.CreateBackwardTransformUnsafe();
			if (transform != null) {
				ExportRotatedFloatingObjectBox(box, transform);
				ExportNotRotatedContent(box);
			}
			else
				base.ExportFloatingObjectBox(box);
		}
		public override void ExportParagraphFrameBox(ParagraphFrameBox box) {
			Matrix transform = box.CreateBackwardTransformUnsafe();
			if (transform != null) {
				ExportRotatedParagraphFrameBox(box, transform);
				ExportNotRotatedContent(box);
			}
			else
				base.ExportParagraphFrameBox(box);
		}
		internal void ExportRotatedFloatingTextBox(FloatingObjectBox textBox, Action<DocumentLayoutExporter> exportAction) {
			Matrix transform = textBox.CreateBackwardTransformUnsafe();
			ExportRotatedFloatingObjectBox(textBox, transform, exportAction);
		}
		void ExportRotatedFloatingObjectBox(FloatingObjectBox box, Matrix transform, Action<DocumentLayoutExporter> exportAction) {
			Image image = CreateRotatedFloatingObjectBoxImage(box, transform, exportAction);
			Rectangle bounds = GetDrawingBounds(RectangleUtils.BoundingRectangle(box.Bounds, transform));
			ExportPictureCore(box, image, bounds, ImageSizeMode.StretchImage, true, true);
		}
		void ExportRotatedFloatingObjectBox(FloatingObjectBox box, Matrix transform) {
			Image image = CreateRotatedFloatingObjectBoxImage(box, transform);
			Rectangle bounds = GetDrawingBounds(RectangleUtils.BoundingRectangle(box.Bounds, transform));
			ExportPictureCore(box, image, bounds, ImageSizeMode.StretchImage, true, true);
		}
		void ExportRotatedParagraphFrameBox(ParagraphFrameBox box, Matrix transform) {
			Image image = CreateRotatedFloatingObjectBoxImage(box, transform);
			Rectangle bounds = GetDrawingBounds(RectangleUtils.BoundingRectangle(box.Bounds, transform));
			ExportPictureCore(box, image, bounds, ImageSizeMode.StretchImage, true, true);
		}
		Image CreateRotatedFloatingObjectBoxImage(Box box, Matrix transform, Action<DocumentLayoutExporter> exportAction) {
			Rectangle boundingRect = GetDrawingBounds(RectangleUtils.BoundingRectangle(box.Bounds, transform));
			Bitmap image = new Bitmap(boundingRect.Width, boundingRect.Height);
			image.SetResolution(300, 300);
			using (RichEditPrintingImageGraphics graphics = new RichEditPrintingImageGraphics(image, this.printingSystem)) {
				Debug.Assert(graphics.PageUnit == GraphicsUnit.Document);
				IGraphicsPainter painter = new GdiGraphicsPainter(graphics);
				graphics.Painter = painter;
				using (GraphicsDocumentLayoutExporter exporter = new GraphicsDocumentLayoutExporter(DocumentModel, painter, new WinFormsGraphicsDocumentLayoutExporterAdapter(), this.Bounds, TextColors)) {
					exporter.PieceTable = this.PieceTable;
					Rectangle bounds = exporter.GetDrawingBounds(box.Bounds);
					graphics.TranslateTransform(-bounds.X + (boundingRect.Width - bounds.Width) / 2, -bounds.Y + (boundingRect.Height - bounds.Height) / 2);
					exportAction(exporter);
				}
			}
			return image;
		}
		Image CreateRotatedFloatingObjectBoxImage(Box box, Matrix transform) {
			Action<DocumentLayoutExporter> exportAction = (e) => {
				FloatingObjectBox floatingObjectBox = box as FloatingObjectBox;
				if (floatingObjectBox != null)
					e.ExportRotatedContent(floatingObjectBox);
				ParagraphFrameBox paragraphFrameBox = box as ParagraphFrameBox;
				if (paragraphFrameBox != null)
					e.ExportRotatedContent(paragraphFrameBox);
			};
			return CreateRotatedFloatingObjectBoxImage(box, transform, exportAction);
		}
#endif
		public override void ExportFloatingObjectPicture(FloatingObjectBox box, PictureFloatingObjectContent pictureContent) {
			OfficeImageBrick brick = new OfficeImageBrick(DocumentModel.LayoutUnitConverter);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(box.ContentBounds));
			OfficeImage image = pictureContent.Image;
			InternalOfficeImageHelper.EnsureLoadComplete(image);
#if !SL
			brick.Image = (Image)image.NativeImage.Clone();
			brick.BackColor = DXColor.Transparent;
#else
			byte[] brickImageBytes = DevExpress.Data.Printing.Native.DxDibImageConverter.Encode(image.NativeImage, false);
			brick.Image = Image.FromStream(new System.IO.MemoryStream(brickImageBytes));
#endif
			brick.DisposeImage = true;
			brick.SizeMode = ImageSizeMode.StretchImage;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		public override void ExportFloatingObjectTextBox(FloatingObjectBox box, TextBoxFloatingObjectContent textBoxContent, DocumentLayout textBoxDocumentLayout) {
			PieceTable oldPieceTable = this.PieceTable;
			this.PieceTable = textBoxContent.TextBox.PieceTable;
			try {
				Page page = textBoxDocumentLayout.Pages.First;
				SectionPage sectionPage = this.DocumentModel.Sections.First.Page;
#if SL
				PaperKind paperKind = (PaperKind)sectionPage.PaperKind;
#else
				PaperKind paperKind = sectionPage.PaperKind;
#endif
				PaperKindInfo paperKindInfo = new PaperKindInfo(paperKind, paperKind != PaperKind.Custom ? sectionPage.Landscape : false);
				Margins margins = CalculateMargins(page);
				Rectangle pageBounds = CalculateActualPageRectangle(page.Bounds, paperKindInfo.Landscape);
				Size pageSize = DocumentModel.LayoutUnitConverter.LayoutUnitsToHundredthsOfInch(pageBounds.Size);
				ExportPageCore(page, margins, paperKindInfo, pageSize, false, box.Bounds, 1, () => { ExportPageBase(page); });
				TextBoxOffset = currentContainer.Location;
			}
			finally {
				this.PieceTable = oldPieceTable;
			}
		}
		public override void ExportParagraphFrameTextBox(ParagraphFrameBox box, DocumentLayout textBoxDocumentLayout) {
			PieceTable oldPieceTable = this.PieceTable;
			this.PieceTable = box.PieceTable;
			try {
				Page page = textBoxDocumentLayout.Pages.First;
				SectionPage sectionPage = this.DocumentModel.Sections.First.Page;
#if SL
				PaperKind paperKind = (PaperKind)sectionPage.PaperKind;
#else
				PaperKind paperKind = sectionPage.PaperKind;
#endif
				PaperKindInfo paperKindInfo = new PaperKindInfo(paperKind, paperKind != PaperKind.Custom ? sectionPage.Landscape : false);
				Margins margins = CalculateMargins(page);
				Rectangle pageBounds = CalculateActualPageRectangle(page.Bounds, paperKindInfo.Landscape);
				Size pageSize = DocumentModel.LayoutUnitConverter.LayoutUnitsToHundredthsOfInch(pageBounds.Size);
				ExportPageCore(page, margins, paperKindInfo, pageSize, false, box.Bounds, 1, () => { ExportPageBase(page); });
				TextBoxOffset = currentContainer.Location;
			}
			finally {
				this.PieceTable = oldPieceTable;
			}
		}
		void PrintRectangle(Color fillColor, Rectangle bounds) {
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
			brick.BackColor = fillColor;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		public override void ExportFloatingObjectShape(FloatingObjectBox box, Shape shape) {
			Rectangle contentBounds = GetDrawingBounds(box.ContentBounds);
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			TextRunBase currentRun = box.GetRun(box.PieceTable);
			Color fillColor = run.Shape.FillColor;
			if (!DXColor.IsTransparentOrEmpty(fillColor))
				PrintRectangle(fillColor, contentBounds);
			int penWidth = GetShapeOutlinePenWidth(run, box);
			if (penWidth >= 0) {
				Color outlineColor = run.Shape.OutlineColor;
				Rectangle shapeBounds = GetDrawingBounds(box.Bounds);
				Rectangle bounds;
				bounds = new Rectangle(shapeBounds.X, shapeBounds.Y, contentBounds.X - shapeBounds.X, shapeBounds.Height);
				if (bounds.Width == 0 && penWidth >= 0)
					bounds.Width = 1;
				PrintRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.X, shapeBounds.Y, contentBounds.Width, contentBounds.Y - shapeBounds.Y);
				if (bounds.Height == 0 && penWidth >= 0)
					bounds.Height = 1;
				PrintRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.Right, shapeBounds.Y, shapeBounds.Right - contentBounds.Right, shapeBounds.Height);
				if (bounds.Width == 0 && penWidth >= 0)
					bounds.Width = 1;
				PrintRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.X, contentBounds.Bottom, contentBounds.Width, shapeBounds.Bottom - contentBounds.Bottom);
				if (bounds.Height == 0 && penWidth >= 0)
					bounds.Height = 1;
				PrintRectangle(outlineColor, bounds);
			}
		}
		public override void ExportParagraphFrameShape(ParagraphFrameBox box, Shape shape) {
			Rectangle contentBounds = box.ContentBounds;
			if (box.FrameProperties == null || box.DocumentLayout == null) {
				Color backColor = box.GetParagraph().BackColor;
				SetBackColor(backColor, contentBounds);
				PrintRectangle(backColor, GetDrawingBounds(box.Bounds));
				return;
			}
			Rectangle paragraphBackgroundBounds = GetDrawingBounds(box.ActualSizeBounds);
			Rectangle actualBounds = box.ActualSizeBounds;
			ParagraphProperties boxParagraphProperties = box.GetParagraph().ParagraphProperties;
			RowCollection rows = box.DocumentLayout.Pages.Last.Areas.Last.Columns.Last.Rows;
			bool isContainsTable = box.DocumentLayout.Pages.Last.Areas.Last.Columns.Last.Tables.Count > 0;
			DrawParagagraphBackground(paragraphBackgroundBounds, rows);
			if (isContainsTable)
				DrawParagraphBordersWithoutTableBounds(actualBounds, boxParagraphProperties, rows);
			else
				DrawParagraphBordersWithCorners(actualBounds, boxParagraphProperties);
		}
		private void FillRectangle(Color fillColor, Rectangle actualContentBounds) {
			PrintRectangle(fillColor, actualContentBounds);
		}
		private void DrawParagraphBordersWithCorners(Rectangle actualContentBounds, ParagraphProperties boxParagraphProperties) {
			DrawParagraphBorders(actualContentBounds, boxParagraphProperties);
			DrawParagraphBordersCorners(actualContentBounds, boxParagraphProperties);
		}
		private static int GetActualBoxHeight(Rectangle actualBounds, RowCollection rows, Row currentRow, Rectangle rowBounds) {
			if (currentRow == rows.First && currentRow == rows.Last)
				return actualBounds.Height;
			if (currentRow == rows.First)
				return rowBounds.Height + rowBounds.Y - actualBounds.Y;
			if (currentRow == rows.Last)
				return actualBounds.Bottom - rowBounds.Y;
			return rowBounds.Height;
		}
		private void DrawParagagraphBackground(Rectangle actualBounds, RowCollection rows) {
			for (int index = 0; index < rows.Count; index++) {
				Row currentRow = rows[index];
				if (!currentRow.Paragraph.IsInCell()) {
					Rectangle rowBounds = GetDrawingBounds(currentRow.Bounds);
					int y = currentRow != rows.First ? rowBounds.Y : actualBounds.Y;
					int height = GetActualBoxHeight(actualBounds, rows, currentRow, rowBounds);
					Rectangle actualParagraphBounds = new Rectangle(actualBounds.X, y, actualBounds.Width, height);
					FillRectangle(currentRow.Paragraph.BackColor, actualParagraphBounds);
				}
			}
		}
		private void DrawParagraphBordersWithoutTableBounds(Rectangle actualBounds, ParagraphProperties boxParagraphProperties, RowCollection rows) {
			for (int index = 0; index < rows.Count; index++) {
				Row currentRow = rows[index];
				if (!currentRow.Paragraph.IsInCell()) {
					Rectangle rowBounds = currentRow.Bounds;
					int y = currentRow != rows.First ? rowBounds.Y : actualBounds.Y;
					int height = 0;
					do {
						height += GetActualBoxHeight(actualBounds, rows, rows[index], rows[index].Bounds);
						if (index < rows.Count - 1)
							index++;
					}
					while (index < rows.Count - 1 && !rows[index].Paragraph.IsInCell());
					Rectangle actualParagraphBounds = new Rectangle(actualBounds.X, y, actualBounds.Width, height);
					DrawParagraphBordersWithCorners(actualParagraphBounds, boxParagraphProperties);
				}
			}
		}
		private void DrawParagraphBorders(Rectangle contentBounds, ParagraphProperties paragraphProperties) {
			DocumentModelUnitToLayoutUnitConverter converter = DocumentModel.ToDocumentLayoutUnitConverter;
			BorderInfo leftBorder = paragraphProperties.LeftBorder;
			BorderInfo rightBorder = paragraphProperties.RightBorder;
			BorderInfo topBorder = paragraphProperties.TopBorder;
			BorderInfo bottomBorder = paragraphProperties.BottomBorder;
			TableCellVerticalBorderViewInfo borderViewInfo = new TableCellVerticalBorderViewInfo(null, rightBorder, 0, 0, converter);
			Rectangle rightBorderBounds = new Rectangle(contentBounds.Right, contentBounds.Y, 0, contentBounds.Height);
			ExportTableBorder(borderViewInfo, rightBorderBounds);
			borderViewInfo = new TableCellVerticalBorderViewInfo(null, leftBorder, 0, 0, converter);
			Rectangle leftBorderBounds = new Rectangle(contentBounds.Left, contentBounds.Y, 0, contentBounds.Height);
			ExportTableBorder(borderViewInfo, leftBorderBounds);
			SingleLineCornerViewInfo leftCorner = new SingleLineCornerViewInfo(leftBorder, rightBorder, topBorder, bottomBorder, 0.0f, 0.0f, CornerViewInfoType.OuterVerticalStart);
			SingleLineCornerViewInfo rightCorner = new SingleLineCornerViewInfo(leftBorder, rightBorder, topBorder, bottomBorder, 0.0f, 0.0f, CornerViewInfoType.OuterVerticalEnd);
			ParagraphHorizontalBorderViewInfo horizontalBorderViewInfo = new ParagraphHorizontalBorderViewInfo(topBorder, converter, leftCorner, rightCorner);
			Rectangle topBorderBounds = new Rectangle(contentBounds.X, contentBounds.Top, contentBounds.Width, 0);
			ExportTableBorder(horizontalBorderViewInfo, topBorderBounds);
			Rectangle bottomBorderBounds = new Rectangle(contentBounds.X, contentBounds.Bottom, contentBounds.Width, 0);
			horizontalBorderViewInfo = new ParagraphHorizontalBorderViewInfo(bottomBorder, converter, leftCorner, rightCorner);
			ExportTableBorder(horizontalBorderViewInfo, bottomBorderBounds);
		}
		private void DrawParagraphBordersCorners(Rectangle contentBounds, ParagraphProperties paragraphProperties) {
			DocumentModelUnitToLayoutUnitConverter converter = DocumentModel.ToDocumentLayoutUnitConverter;
			BorderInfo leftBorder = paragraphProperties.LeftBorder;
			BorderInfo rightBorder = paragraphProperties.RightBorder;
			BorderInfo topBorder = paragraphProperties.TopBorder;
			BorderInfo bottomBorder = paragraphProperties.BottomBorder;
			CornerViewInfoBase topLeftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, null, null, topBorder, leftBorder, 0);
			ExportTableBorderCorner(topLeftCorner, contentBounds.Left, contentBounds.Top);
			CornerViewInfoBase topRightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, topBorder, null, null, rightBorder, 0);
			ExportTableBorderCorner(topRightCorner, contentBounds.Right, contentBounds.Top);
			CornerViewInfoBase bottomLeftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, null, leftBorder, bottomBorder, null, 0);
			ExportTableBorderCorner(bottomLeftCorner, contentBounds.Left, contentBounds.Bottom);
			CornerViewInfoBase bottomRightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.OuterHorizontalEnd, converter, bottomBorder, rightBorder, null, null, 0);
			ExportTableBorderCorner(bottomRightCorner, contentBounds.Right, contentBounds.Bottom);
		}
		public override void ExportNumberingListBox(NumberingListBox box) {
			ExportBoxCore(box);
		}
		public override void ExportLineNumberBox(LineNumberBox box) {
			ExportBoxCore(box);
		}
		public override void ExportTextBox(DevExpress.XtraRichEdit.Layout.TextBox box) {
			ExportBoxCore(box);
		}
		public override void ExportLayoutDependentTextBox(LayoutDependentTextBox box) {
			ExportBoxCore(box);
		}
		public override void ExportHyphenBox(HyphenBox box) {
			ExportBoxCore(box);
		}
		public override void ExportSpaceBox(Box box) {
		}
		public override void ExportTabSpaceBox(TabSpaceBox box) {
			if (box.TabInfo.Leader == TabLeaderType.None)
				return;
			string tabLeaderText = GetTabLeaderText(box, CorrectTextDrawingBounds(box, box.Bounds));
			ExportBoxCore(box, tabLeaderText);
		}
		public override void ExportLineBreakBox(LineBreakBox box) {
		}
		public override void ExportPageBreakBox(PageBreakBox box) {
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
		}
		public override void ExportColumnBreakBox(ColumnBreakBox box) {
		}
		public override void ExportSectionMarkBox(SectionMarkBox box) {
		}
		protected internal void ExportBoxCore(Box box) {
			string text = GetBoxText(box);
			ExportBoxCore(box, text);
		}
		protected internal void ExportBoxCore(Box box, string text) {
			BookmarkPrintControl brickOwner = new BookmarkPrintControl();
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter, brickOwner);
			Rectangle bounds = CorrectTextDrawingBounds(box, box.Bounds);
			Rectangle drawingBounds = GetDrawingBounds(bounds);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, drawingBounds);
			brick.Text = text;
			brick.ForeColor = box.GetActualForeColor(PieceTable, TextColors, GetActualBackColor(box.Bounds));
			brick.BackColor = DXColor.Empty;
			brick.Font = GetFont(box);
			brick.StringFormat = sf;
			brick.NoClip = true;
			brick.BorderWidth = 0f;
			AssignHyperlinkUrl(box, brick);
			if (box.IsHyperlinkSupported) {
				var url = GetUrl(box);
				var anchor = GetAnchor(box);
				bool isBrickCrossReference = String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(anchor);
				if (isBrickCrossReference) {
					brick.Target = "_self";
					brick.Url = anchor;
					brickOwner.IsLink = true;
					brickOwner.Name = brick.Text;
					brickOwner.NavigateUrl = anchor;
				}
			}
			AddBrickToCurrentPage(brick);
		}
		protected internal void AssignHyperlinkUrl(Box box, IBaseBrick brick) {
			try {
				if (box.IsHyperlinkSupported)
					brick.Url = GetUrl(box);
			}
			catch {
				brick.Url = String.Empty;
			}
		}
		public override void ExportHighlightArea(HighlightArea area) {
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
			Rectangle bounds = area.Bounds;
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(bounds));
			brick.Text = String.Empty;
			brick.BackColor = area.Color;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		protected virtual Font GetFont(Box box) {
			FontInfo fontInfo = box.GetFontInfo(PieceTable);
			return fontInfo.Font;
		}
		protected Rectangle CorrectTextDrawingBounds(Box box, Rectangle textBounds) {
			int free = box.GetFontInfo(PieceTable).Free;
			Rectangle correctedTextBounds = textBounds;
			correctedTextBounds.Y += free;
			return correctedTextBounds;
		}
		protected internal virtual Rectangle GetDrawingBoundsInLayoutUnits(Rectangle bounds) {
			return base.GetDrawingBounds(bounds);
		}
		protected internal override Rectangle GetDrawingBounds(Rectangle bounds) {
			Rectangle boundsInLayoutUnits = base.GetDrawingBounds(bounds);
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(boundsInLayoutUnits);
		}
		public override void ExportUnderlineBox(Row row, UnderlineBox underlineBox) {
			Box box = GetCharacterLineBoxByIndex(row, underlineBox.StartAnchorIndex);
			UnderlineType lineType = box.GetFontUnderlineType(PieceTable);
			Underline line = DocumentModel.UnderlineRepository.GetPatternLineByType(lineType);
			UnderlineBrick brick = new UnderlineBrick(DocumentModel.LayoutUnitConverter);
			ExportLineBoxCore(underlineBox, line, brick, box.GetActualUnderlineColor(PieceTable, TextColors, GetBackColor(GetDrawingBounds(box.Bounds))));
		}
		public override void ExportStrikeoutBox(Row row, UnderlineBox strikeoutBox) {
			Box box = GetCharacterLineBoxByIndex(row, strikeoutBox.StartAnchorIndex);
			StrikeoutType lineType = box.GetFontStrikeoutType(PieceTable);
			Strikeout line = DocumentModel.StrikeoutRepository.GetPatternLineByType(lineType);
			StrikeoutBrick brick = new StrikeoutBrick(DocumentModel.LayoutUnitConverter);
			ExportLineBoxCore(strikeoutBox, line, brick, box.GetActualStrikeoutColor(PieceTable, TextColors, GetBackColor(GetDrawingBounds(box.Bounds))));
		}
		void ExportLineBoxCore<T>(UnderlineBox lineBox, PatternLine<T> line, PatternLineBrick<T> brick, Color lineColor) where T : struct {
			Rectangle clipBounds = line.CalcLineBounds(lineBox.ClipBounds, lineBox.UnderlineThickness);
			Rectangle bounds = new Rectangle(clipBounds.Left, lineBox.UnderlineBounds.Top, clipBounds.Width, lineBox.UnderlineBounds.Height);
			bounds = GetDrawingBoundsInLayoutUnits(bounds);
			Rectangle actualLineBounds = line.CalcLineBounds(bounds, lineBox.UnderlineThickness);
			actualLineBounds.X = 0;
			actualLineBounds.Y += (int)line.CalcLinePenVerticalOffset(actualLineBounds) - bounds.Y;
			bounds.Height = Math.Max(bounds.Height, actualLineBounds.Height);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds));
			brick.LineBounds = actualLineBounds;
			brick.BackColor = DXColor.Transparent;
			brick.PatternLineType = line.Id;
			brick.BorderColor = lineColor;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		protected internal virtual void AddBrickToCurrentPage(Brick brick) {
			AddBrickToCurrentPage(brick, CurrentCell);
		}
		protected internal virtual void AddBrickToCurrentPage(Brick brick, TableCellViewInfo cell) {
			if (cell == null || suppressPutIntoTableCellBrick)
				AddBrickToCurrentContainer(brick);
			else {
				OfficePanelBrick panelBrick;
				if (!cellBricks.TryGetValue(cell, out panelBrick))
					AddBrickToCurrentContainer(brick);
				else {
					PointF location = brick.Location;
					location.X -= panelBrick.AbsoluteLocation.X;
					location.Y -= panelBrick.AbsoluteLocation.Y;
					brick.Location = location;
					panelBrick.Bricks.Add(brick);
				}
			}
		}
		protected internal virtual void AddBrickToCurrentContainer(Brick brick) {
			PointF location = brick.Location;
			location.X -= TextBoxOffset.X;
			location.Y -= TextBoxOffset.Y;
			brick.Location = location;
			currentContainer.Bricks.Add(brick);
		}
		public override void ExportTableCell(TableCellViewInfo cell) {
			Rectangle rect = cell.TableViewInfo.GetCellBounds(cell);
			int top = cell.TableViewInfo.Anchors[cell.TopAnchorIndex].VerticalPosition;
			int bottom = cell.TableViewInfo.Anchors[cell.BottomAnchorIndex].VerticalPosition;
			rect.Y = top;
			rect.Height = bottom - top;
			OfficePanelBrick brick = new OfficePanelBrick(DocumentModel.LayoutUnitConverter);
			Rectangle bounds = rect;
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(bounds));
			brick.AbsoluteLocation = brick.Location;
			brick.BackColor = cell.Cell.GetActualBackgroundColor();
			AddBrickToCurrentPage(brick, cell.TableViewInfo.ParentTableCellViewInfo);
			cellBricks[cell] = brick;
			ExportInnerTables(cell); 
		}
		protected internal virtual void ExportInnerTables(TableCellViewInfo cell) {
			TableViewInfoCollection innerTables = cell.InnerTables;
			int innerTableCount = innerTables.Count;
			TableCellViewInfo oldCurrentCell = CurrentCell;
			SetCurrentCell(cell);
			for (int i = 0; i < innerTableCount; i++) {
				innerTables[i].ExportBackground(this);
			}
			SetCurrentCell(oldCurrentCell);
		}
		public override void ExportTableRow(TableRowViewInfoBase row) {
			Rectangle rect = row.GetBounds();
			OfficeTextBrick brick = new OfficeTextBrick(DocumentModel.LayoutUnitConverter);
			Rectangle bounds = rect;
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(bounds));
			brick.BackColor = row.TableViewInfo.Table.BackgroundColor;
			brick.NoClip = true;
			AddBrickToCurrentPage(brick);
		}
		public override void ExportTableBorder(TableBorderViewInfoBase border, Rectangle cellBounds) {
			bool oldValue = suppressPutIntoTableCellBrick;
			this.suppressPutIntoTableCellBrick = true;
			try {
				TableBorderPainter borderPainter = GetBorderPainter(border);
				if (borderPainter != null) {
					borderPainter.DrawBorder(border, cellBounds);
				}
			}
			finally {
				suppressPutIntoTableCellBrick = oldValue;
			}
		}
		public override void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y) {
			bool oldValue = suppressPutIntoTableCellBrick;
			this.suppressPutIntoTableCellBrick = true;
			try {
				TableCornerPainter borderPainter = GetBorderPainter(corner);
				if (borderPainter != null)
					borderPainter.DrawCorner(new PrinterPainterWrapper(this, PrintingSystem), x, y, corner);
			}
			finally {
				suppressPutIntoTableCellBrick = oldValue;
			}
		}
		protected override void ExportTablesBackground(Column column) {
			TableViewInfoCollection columnTables = column.InnerTables;
			if (columnTables != null)
				ExportTopLevelTables(columnTables);
		}
		void ExportTopLevelTables(TableViewInfoCollection columnTables) {
			int count = columnTables.Count;
			for (int i = 0; i < count; i++)
				if (columnTables[i].Table.NestedLevel == 0)
					columnTables[i].ExportBackground(this);
		}
		protected virtual TableBorderPainter GetBorderPainter(TableBorderViewInfoBase viewInfo) {
			BorderInfo border = viewInfo.Border;
			BorderLineStyle borderStyle = border.Style;
			if (borderStyle == BorderLineStyle.None || borderStyle == BorderLineStyle.Nil || borderStyle == BorderLineStyle.Disabled)
				return null;
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			int width = borderCalculator.GetActualWidth(border);
			width = viewInfo.Converter.ToLayoutUnits(width);
			float[] compoundArray = borderCalculator.GetDrawingCompoundArray(border);
			PrinterPainterWrapper painterWrapper = new PrinterPainterWrapper(this, PrintingSystem);
			if (compoundArray.Length == 4)
				return new DoubleBorderPainter(painterWrapper, compoundArray, width);
			else if (compoundArray.Length == 6)
				return new TripleBorderPainter(painterWrapper, compoundArray, width);
			else
				return new SingleBorderPainter(painterWrapper, width, GetTableBorderLine(border.Style));
		}
		protected virtual TableCornerPainter GetBorderPainter(CornerViewInfoBase corner) {
			if (corner is NoneLineCornerViewInfo)
				return null;
			return new TableCornerPainter();
		}
	}
	#endregion
#if !SL
	#region RichEditPrintingImageGraphics
	public class RichEditPrintingImageGraphics : ImageGraphics {
		IGraphicsPainter painter;
		public RichEditPrintingImageGraphics(Image img, PrintingSystemBase ps)
			: base(img, ps) {
		}
		public IGraphicsPainter Painter { get { return painter; } set { painter = value; } }
		public override void DrawString(string s, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			if (painter != null && painter.HasTransform)
				this.Graphics.DrawString(s, font, brush, bounds, format);
			else
				base.DrawString(s, font, brush, bounds, format);
		}
	}
	#endregion
#endif
	#region HorizontalPrinterCharacterLinePainter
	public class HorizontalPrinterCharacterLinePainter : ICharacterLinePainter {
		PrintingDocumentExporter exporter;
		public HorizontalPrinterCharacterLinePainter(PrintingDocumentExporter exporter) {
			this.exporter = exporter;
		}
		protected DocumentModel DocumentModel { get { return Exporter.DocumentModel; } }
		protected PrintingDocumentExporter Exporter { get { return exporter; } }
		protected virtual void DrawCore(Underline line, RectangleF boundsF, Color color) {
			UnderlineBrick brick = new UnderlineBrick(DocumentModel.LayoutUnitConverter);
			Rectangle bounds = Exporter.GetDrawingBoundsInLayoutUnits(new Rectangle((int)boundsF.X, (int)boundsF.Y, (int)boundsF.Width, (int)boundsF.Height));
			VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds));
			brick.BackColor = DXColor.Transparent;
			brick.NoClip = true;
			brick.PatternLineType = line.Id;
			brick.BorderColor = color;
			brick.LineBounds = new RectangleF(0, 0, bounds.Width, bounds.Height);
			Exporter.AddBrickToCurrentPage(brick);
		}
		#region ICharacterLinePainter Members
		public void DrawStrikeout(StrikeoutDouble underline, RectangleF bounds, Color color) {
		}
		public void DrawStrikeout(StrikeoutSingle underline, RectangleF bounds, Color color) {
		}
		public void DrawUnderline(UnderlineWave underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDoubleWave underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineThickLongDashed underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineThickDashDotDotted underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineThickDashDotted underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineThickDashed underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineThickDotted underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineThickSingle underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineLongDashed underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineHeavyWave underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDouble underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDashDotDotted underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDashDotted underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDashSmallGap underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDashed underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineDotted underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		public void DrawUnderline(UnderlineSingle underline, RectangleF bounds, Color color) {
			DrawCore(underline, bounds, color);
		}
		#endregion
	}
	public class VerticalPrinterCharacterLinePainter : HorizontalPrinterCharacterLinePainter {
		public VerticalPrinterCharacterLinePainter(PrintingDocumentExporter exporter)
			: base(exporter) {
		}
		protected override void DrawCore(Underline line, RectangleF boundsF, Color color) {
			VerticalUnderlineBrick brick = new VerticalUnderlineBrick(DocumentModel.LayoutUnitConverter);
			Rectangle bounds = Exporter.GetDrawingBoundsInLayoutUnits(new Rectangle((int)boundsF.X, (int)boundsF.Y, (int)boundsF.Width, (int)boundsF.Height));
			VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds));
			brick.BackColor = DXColor.Transparent;
			brick.NoClip = true;
			brick.PatternLineType = line.Id;
			brick.BorderColor = color;
			brick.LineBounds = new RectangleF(0, 0, bounds.Width, bounds.Height);
			Exporter.AddBrickToCurrentPage(brick);
		}
	}
	#endregion
	#region PrinterPainterWrapper
	public class PrinterPainterWrapper : IPainterWrapper {
		readonly PrintingDocumentExporter exporter;
		readonly PrintingSystemBase printingSystem;
		public PrinterPainterWrapper(PrintingDocumentExporter exporter, PrintingSystemBase printingSystem) {
			Guard.ArgumentNotNull(exporter, "exporter");
			Guard.ArgumentNotNull(printingSystem, "printingSystem");
			this.exporter = exporter;
			this.printingSystem = printingSystem;
		}
		#region IPainterWrapper Members
		public ICharacterLinePainter HorizontalLinePainter { get { return exporter.HorizontalPrinterLinePainter; } }
		public ICharacterLinePainter VerticalLinePainter { get { return exporter.VerticalPrinterLinePainter; } }
		public void FillRectangle(Color color, RectangleF boundsF) {
			Rectangle bounds = exporter.GetDrawingBoundsInLayoutUnits(new Rectangle((int)boundsF.X, (int)boundsF.Y, (int)boundsF.Width, (int)boundsF.Height));
			bounds = exporter.DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds);
			OfficeRectBrick brick = new OfficeRectBrick(exporter.DocumentModel.LayoutUnitConverter);
			VisualBrickHelper.InitializeBrick(brick, printingSystem, bounds);
			brick.BackColor = color;
			brick.NoClip = true;
			exporter.AddBrickToCurrentPage(brick);
		}
		public PointF GetSnappedPoint(PointF point) {
			return new PointF((int)point.X, (int)point.Y);
		}
		public void SnapHeights(float[] heights) {
		}
		public void SnapWidths(float[] widths) {
		}
		#endregion
	}
	#endregion
#endif
	#region HyperlinkCalculator (helper class)
	class HyperlinkCalculator {
		struct Record {
			RunIndex start;
			RunIndex end;
			string url;
			string anchor;
			public Record(RunIndex start, RunIndex end, string url)
				: this(start, end, url, String.Empty) {
			}
			public Record(RunIndex start, RunIndex end, string url, string anchor) {
				this.start = start;
				this.end = end;
				this.url = url;
				this.anchor = anchor;
			}
			public RunIndex Start { get { return start; } }
			public RunIndex End { get { return end; } }
			public string Url { get { return url; } }
			public string Anchor { get { return anchor; } }
		}
		readonly List<Record> urls;
		readonly int count;
		readonly bool empty;
		int current;
		public HyperlinkCalculator(PieceTable pieceTable) {
			urls = new List<Record>();
			HyperlinkInfoCollection hyperlinkInfos = pieceTable.HyperlinkInfos;
			foreach (Field field in pieceTable.Fields) {
				if (hyperlinkInfos.IsHyperlink(field.Index)) {
					HyperlinkInfo info = hyperlinkInfos[field.Index];
					if (!String.IsNullOrEmpty(info.NavigateUri) || !String.IsNullOrEmpty(info.Anchor)) {
						string url = info.CreateUrl();
						urls.Add(new Record(field.FirstRunIndex, field.LastRunIndex, url, info.Anchor));
					}
				}
			}
			count = urls.Count;
			empty = count == 0;
			current = -1;
		}
		public string GetAnchor(RunIndex runIndex) {
			if (empty)
				return String.Empty;
			if ((current >= 0) && (urls[current].Start > runIndex))
				current = -1;
			while ((current < count - 1) && (urls[current + 1].Start <= runIndex))
				current++;
			if ((current < 0) || (urls[current].End < runIndex))
				return String.Empty;
			return urls[current].Anchor;
		}
		public string GetUrl(RunIndex runIndex) {
			if (empty)
				return String.Empty;
			if ((current >= 0) && (urls[current].Start > runIndex))
				current = -1;
			while ((current < count - 1) && (urls[current + 1].Start <= runIndex))
				current++;
			if ((current < 0) || (urls[current].End < runIndex))
				return String.Empty;
			return HyperlinkUriHelper.ConvertToUrl(urls[current].Url);
		}
	}
	#endregion
#if !DXPORTABLE
	#region OfficeLineBrick
#if !SL
	[BrickExporter(typeof(OfficeLineBrickExporter))]
#endif
	public class OfficeLineBrick : VisualBrick {
		readonly DocumentLayoutUnitConverter unitConverter;
		readonly LineType lineType;
		readonly Color color;
		public OfficeLineBrick(DocumentLayoutUnitConverter unitConverter, LineType lineDown, Color color) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			this.lineType = lineDown;
			this.color = color;
		}
		internal LineType LineType { get { return lineType; } }
		internal Color LineColor { get { return color; } }
		internal DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
	}
	#endregion
#if !SL
	#region OfficeLineBrickExporter
	public class OfficeLineBrickExporter : VisualBrickExporter {
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			RectangleF bounds = (Brick as OfficeLineBrick).UnitConverter.DocumentsToLayoutUnits(clientRect);
			OfficeLineBrick lineBrick = Brick as OfficeLineBrick;
			if (lineBrick.LineType == LineType.lTDown)
				gr.DrawLine(new Pen(lineBrick.LineColor), bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
			if (lineBrick.LineType == LineType.lTUp)
				gr.DrawLine(new Pen(lineBrick.LineColor), bounds.Left, bounds.Bottom, bounds.Right, bounds.Top);
			if (lineBrick.LineType == LineType.lTHorizontal)
				gr.DrawLine(new Pen(lineBrick.LineColor), bounds.Left, bounds.Top, bounds.Right, bounds.Top);
		}
	}
	#endregion
#endif
	#region OfficeRoundedRectangleBrick
#if !SL
	[BrickExporter(typeof(OfficeRoundedRectangleBrickExporter))]
#endif
	public class OfficeRoundedRectangleBrick : VisualBrick {
		readonly DocumentLayoutUnitConverter unitConverter;
		readonly int radius;
		readonly Color color;
		public OfficeRoundedRectangleBrick(DocumentLayoutUnitConverter unitConverter, int radius, Color color) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			this.radius = radius;
			this.color = color;
		}
		internal Color FillColor { get { return color; } }
		internal DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		internal int Radius { get { return radius; } }
	}
	#endregion
#if !SL
	#region OfficeRoundedRectangleBrickExporter
	public class OfficeRoundedRectangleBrickExporter : VisualBrickExporter {
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			RectangleF bounds = (Brick as OfficeRoundedRectangleBrick).UnitConverter.DocumentsToLayoutUnits(clientRect);
			Rectangle roundedRectangle = Rectangle.Round(bounds);
			OfficeRoundedRectangleBrick rectangleBrick = Brick as OfficeRoundedRectangleBrick;
			CommentPainter painter = new CommentPainter();
			GraphicsPath path = painter.CreatePathRoundedRectangle(roundedRectangle, rectangleBrick.Radius);
			SolidBrush brush = new SolidBrush(rectangleBrick.FillColor);
			gr.FillPath(brush, path);
			Rectangle newRectangle = new Rectangle();
			newRectangle.Width = roundedRectangle.Width - 1;
			newRectangle.Height = roundedRectangle.Height - 1;
			newRectangle.Location = roundedRectangle.Location;
			GraphicsPath newPath = painter.CreatePathRoundedRectangle(newRectangle, rectangleBrick.Radius - 1);
			gr.DrawPath(new Pen(rectangleBrick.BorderColor), newPath);
		}
	}
	#endregion
#endif
	public enum LineType { lTHorizontal, lTUp, lTDown }
	#region ExportToPdf auxiliary classes
	public class ExportToPdfPrintingSystem : PrintingSystemBase {
		public ExportToPdfPrintingSystem() {
#if !SL
			this.BeforePagePaint += ExportToPdfPrintingSystem_BeforePagePaint;
#endif
		}
#if !SL
		void ExportToPdfPrintingSystem_BeforePagePaint(object sender, PagePaintEventArgs e) {
			DevExpress.XtraPrinting.Export.Pdf.PdfGraphics pdfGraphics = e.Graphics as DevExpress.XtraPrinting.Export.Pdf.PdfGraphics;
			if (pdfGraphics != null)
				pdfGraphics.ScaleStrings = true;
		}
#endif
		protected override XtraPrinting.Native.PrintingDocument CreateDocument() {
			PSLinkDocument document = new PSLinkDocument(this, new Action0(AfterBuildPagesProc));
			document.NavigationInfo = new NavigationInfo();
			return document;
		}
		private void AfterBuildPagesProc() {
			NavigationInfo navigationInfo = ((PrintingDocument)Document).NavigationInfo;
			if (navigationInfo != null) {
				List<BookmarkPrintControl> controls = GetBrickOwners(navigationInfo);
				new NavigationBuilder(new NavigationManager(controls)).SetNavigationPairs(navigationInfo.NavigationBricks, Pages);
				controls.Clear();
				navigationInfo.Clear();
			}
		}
		private List<BookmarkPrintControl> GetBrickOwners(NavigationInfo navigationInfo) {
			List<BookmarkPrintControl> controls = new List<BookmarkPrintControl>();
			foreach (var brick in navigationInfo.NavigationBricks) {
				BrickPagePair pair = brick as BrickPagePair;
				if (pair == null)
					continue;
				BookmarkPrintControl printCtrl = ((DevExpress.Office.Printing.OfficeTextBrick)pair.GetBrick(Pages)).BrickOwner as BookmarkPrintControl;
				if (printCtrl == null)
					continue;
				controls.Add(printCtrl);
			}
			return controls;
		}
	}
	internal class NavigationBuilder {
		Dictionary<IBrickOwner, BrickPagePair> navigationTargets = new Dictionary<IBrickOwner, BrickPagePair>();
		Dictionary<IBrickOwner, List<VisualBrick>> navigationLinks = new Dictionary<IBrickOwner, List<VisualBrick>>();
		Hashtable brickOwners = new Hashtable();
		NavigationManager navigationManager;
		public NavigationBuilder(NavigationManager navigationManager) {
			this.navigationManager = navigationManager;
		}
		public void SetNavigationPairs(BrickPagePairCollection bpPairs, PageList pages) {
			bpPairs.Sort(new BrickPagePairComparer(pages));
			foreach (BrickPagePair bpPair in bpPairs) {
				VisualBrick brick = bpPair.GetBrick(pages) as VisualBrick;
				if (brick == null)
					continue;
				if (brick.BrickOwner.IsNavigationLink) {
					IBrickOwner target = navigationManager.GetNavigationTarget(((BookmarkPrintControl)brick.BrickOwner));
					if (target != null) {
						BrickPagePair navigationPair;
						if (navigationTargets.TryGetValue(target, out navigationPair) && !brickOwners.ContainsKey(brick.BrickOwner)) {
							brick.NavigationPair = navigationPair;
						}
						else {
							brickOwners[brick.BrickOwner] = true;
							List<VisualBrick> links;
							if (!navigationLinks.TryGetValue(target, out links)) {
								links = new List<VisualBrick>();
								navigationLinks.Add(target, links);
							}
							links.Add(brick);
						}
					}
				}
				if (brick.BrickOwner.IsNavigationTarget) {
					List<VisualBrick> links2;
					if (navigationLinks.TryGetValue(brick.BrickOwner, out links2)) {
						foreach (VisualBrick item in links2)
							item.NavigationPair = bpPair;
						navigationLinks.Remove(brick.BrickOwner);
					}
					navigationTargets[brick.BrickOwner] = bpPair;
				}
			}
			foreach (IBrickOwner target in navigationLinks.Keys) {
				BrickPagePair navigationPair;
				if (!navigationTargets.TryGetValue(target, out navigationPair))
					continue;
				foreach (VisualBrick linkBrick in navigationLinks[target])
					linkBrick.NavigationPair = navigationPair;
			}
			navigationTargets.Clear();
			navigationLinks.Clear();
			brickOwners.Clear();
		}
	}
	internal class NavigationManager {
		#region	static
		static IList SelectControls(ControlSelector selector, IList components) {
			List<BookmarkPrintControl> controls = new List<BookmarkPrintControl>();
			if (selector != null) {
				NestedComponentEnumerator enumerator = new NestedComponentEnumerator(components);
				while (enumerator.MoveNext()) {
					if (selector.Select(enumerator.Current) && !controls.Contains(enumerator.Current))
						controls.Add(enumerator.Current);
				}
			}
			return controls;
		}
		#endregion
		Dictionary<String, BookmarkPrintControl> controlBookmarks = new Dictionary<String, BookmarkPrintControl>();
		IList targets;
		List<BookmarkPrintControl> controls = new List<BookmarkPrintControl>();
		public NavigationManager(List<BookmarkPrintControl> controls) {
			this.controls = controls;
			Initialize();
		}
		public void Clear() {
			controlBookmarks.Clear();
			if (targets != null) targets.Clear();
		}
		public bool TargetsContains(BookmarkPrintControl control) {
			return targets != null && targets.Contains(control);
		}
		private IList GetValidNames() {
			return new List<String>(controlBookmarks.Keys);
		}
		public void Initialize() {
			Clear();
			IList validControls = SelectControls(new ControlNamesSelector(), controls);
			foreach (BookmarkPrintControl c in validControls) {
				if (c.IsTarget)
					controlBookmarks[c.Name.ToUpper(CultureInfo.InvariantCulture)] = c;
			}
			CrossRefControlSelector refSelector = new CrossRefControlSelector();
			SelectControls(refSelector, controls);
			TargetControlSelector targetSelector = new TargetControlSelector(GetValidNames(), refSelector.TargetNames);
			this.targets = SelectControls(targetSelector, controls);
		}
		public BookmarkPrintControl GetNavigationTarget(BookmarkPrintControl control) {
			if (control == null)
				return null;
			String navigateUrl = control.NavigateUrl.ToUpper(CultureInfo.InvariantCulture);
			if (String.IsNullOrEmpty(navigateUrl))
				return null;
			if (!controlBookmarks.ContainsKey(navigateUrl))
				return null;
			return controlBookmarks[navigateUrl] as BookmarkPrintControl;
		}
	}
	public abstract class NestedObjectEnumeratorBase : IEnumerator, IEnumerable {
		#region inner class
		protected class EnumStack : Stack<object> {
			public IEnumerator StackEnumerator {
				get { return (IsEmpty == false) ? (IEnumerator)Peek() : null; }
			}
			public bool IsEmpty {
				get { return Count == 0; }
			}
			public EnumStack() {
			}
		}
		#endregion
		private EnumStack stack;
		private IList objects;
		protected EnumStack NestedObjectStack { get { return stack; } }
		protected NestedObjectEnumeratorBase(IList objects) {
			this.objects = objects;
			this.stack = new EnumStack();
		}
		object IEnumerator.Current {
			get { return stack.StackEnumerator.Current; }
		}
		public virtual bool MoveNext() {
			if (stack.IsEmpty) {
				stack.Push(GetEnumerator(objects));
				return stack.StackEnumerator.MoveNext();
			}
			while (stack.StackEnumerator.MoveNext() == false) {
				stack.Pop();
				if (stack.IsEmpty) return false;
			}
			return true;
		}
		protected abstract IList GetNestedObjects();
		protected virtual IEnumerator GetEnumerator(IList objects) {
			return objects.GetEnumerator();
		}
		public virtual void Reset() {
			stack.Clear();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
	}
	public class NestedComponentEnumerator : NestedObjectEnumeratorBase {
		public BookmarkPrintControl Current {
			get { return ((ComponentEnumerator)NestedObjectStack.StackEnumerator).Current; }
		}
		public NestedComponentEnumerator(IList comps)
			: base(comps) {
		}
		protected override IList GetNestedObjects() {
			return null;
		}
		protected override IEnumerator GetEnumerator(IList objects) {
			return new ComponentEnumerator(objects);
		}
	}
	public class ComponentEnumerator : IEnumerator {
		private IEnumerator en;
		public ComponentEnumerator(IList objs) {
			en = objs.GetEnumerator();
		}
		object IEnumerator.Current {
			get { return en.Current; }
		}
		public BookmarkPrintControl Current {
			get { return (BookmarkPrintControl)en.Current; }
		}
		public virtual bool MoveNext() {
			return en.MoveNext();
		}
		public virtual void Reset() {
			en.Reset();
		}
	}
	internal abstract class ControlSelector {
		public bool Select(BookmarkPrintControl control) {
			if (CanSelect(control)) {
				OnSelectComplete(control);
				return true;
			}
			return false;
		}
		public abstract bool CanSelect(BookmarkPrintControl control);
		public virtual void OnSelectComplete(BookmarkPrintControl control) {
		}
	}
	internal class ControlNamesSelector : ControlSelector {
		public override bool CanSelect(BookmarkPrintControl control) {
			return control != null && !String.IsNullOrEmpty(control.Name);
		}
	}
	internal class CrossRefControlSelector : ControlSelector {
		IList targetNames = new List<String>();
		public IList TargetNames {
			get { return targetNames; }
		}
		public override bool CanSelect(BookmarkPrintControl control) {
			return control != null && ((IBrickOwner)control).IsNavigationLink;
		}
		public override void OnSelectComplete(BookmarkPrintControl control) {
			if (control != null)
				targetNames.Add(control.NavigateUrl);
		}
	}
	internal class TargetControlSelector : ControlSelector {
		IList validNames;
		IList targetNames;
		public TargetControlSelector(IList validNames, IList targetNames) {
			this.validNames = validNames != null ? validNames : new string[0];
			this.targetNames = targetNames != null ? targetNames : new string[0];
		}
		public override bool CanSelect(BookmarkPrintControl control) {
			return control != null && control.IsTarget && validNames.Contains(control.Name) && targetNames.Contains(control.Name);
		}
	}
	public class BookmarkPrintControl : IBrickOwner {
		#region IBrickOwner Members
		string IBrickOwner.Name { get { return string.Empty; } }
		BrickOwnerType IBrickOwner.BrickOwnerType { get { return BrickOwnerType.Control; } }
		ControlLayoutRules IBrickOwner.LayoutRules { get { return ControlLayoutRules.None; } }
		TextEditMode IBrickOwner.TextEditMode { get { return TextEditMode.Unavailable; } }
		bool IBrickOwner.IsImageEditable { get { return false; } }
		bool? IBrickOwner.HasImage { get { return null; } }
		string IBrickOwner.Text { get { return string.Empty; } }
		string IBrickOwner.ControlsUnityName { get { return string.Empty; } }
		public bool InSubreport { get { return false; } }
		public bool IsCrossbandControl { get { return false; } }
		bool IBrickOwner.LockedInDesigner { get { return false; } }
		bool IBrickOwner.IsNavigationLink { get { return isLink; } }
		bool IBrickOwner.IsNavigationTarget { get { return isTarget; } }
		bool IBrickOwner.NeedCalcContainerHeight { get { return false; } }
		bool IBrickOwner.HasPageSummary { get { return false; } }
		ConvertHelper IBrickOwner.ConvertHelper { get { return ConvertHelper.Instance; } }
		BookmarkInfo IBrickOwner.EmptyBookmarkInfo { get { return BookmarkInfo.Empty; } }
		bool IBrickOwner.CanCacheImages { get { return true; } }
		object IBrickOwner.RealControl { get { return this; } }
		bool isLink = false;
		bool isTarget = false;
		public bool IsLink { get { return isLink; } set { isLink = value; } }
		public bool IsTarget { get { return isTarget; } set { isTarget = value; } }
		string name = String.Empty;
		string navigateUrl = string.Empty;
		public string Name { get { return name; } set { name = value; } }
		public string NavigateUrl { get { return navigateUrl; } set { navigateUrl = value; } }
		DevExpress.XtraReports.UI.VerticalAnchorStyles IBrickOwner.AnchorVertical { get { return DevExpress.XtraReports.UI.VerticalAnchorStyles.None; } }
		public BookmarkPrintControl() {
		}
		void IBrickOwner.UpdateBrickBounds(VisualBrick brick) { }
		bool IBrickOwner.IsSeparableVert(bool isBrickSeparableVert) { return isBrickSeparableVert; }
		bool IBrickOwner.IsSeparableHorz(bool isBrickSeparableHorz) { return isBrickSeparableHorz; }
		Font IBrickOwner.GetActualFont() { return null; }
		Color IBrickOwner.GetActualForeColor() {
			return DXColor.Transparent;
		}
		Color IBrickOwner.GetActualBackColor() {
			return DXColor.Transparent;
		}
		Color IBrickOwner.GetActualBorderColor() {
			return DXColor.Transparent;
		}
		float IBrickOwner.GetActualBorderWidth() {
			return 0;
		}
		BorderDashStyle IBrickOwner.GetActualBorderDashStyle() {
			return BorderDashStyle.Solid;
		}
		BorderSide IBrickOwner.GetActualBorderSide() {
			return BorderSide.None;
		}
		TextAlignment IBrickOwner.GetActualTextAlignment() { return TextAlignment.MiddleLeft; }
		void IBrickOwner.RaiseDraw(VisualBrick brick, IGraphics gr, RectangleF bounds) { }
		bool IBrickOwner.RaiseAfterPrintOnPage(VisualBrick brick, int pageNumber, int pageCount) { return true; }
		void IBrickOwner.RaiseHtmlItemCreated(VisualBrick brick, IScriptContainer scriptContainer, DXHtmlContainerControl contentCell) { }
		void IBrickOwner.RaiseSummaryCalculated(VisualBrick brick, string text, string format, object value) { }
		void IBrickOwner.AddToSummaryUpdater(VisualBrick brick, VisualBrick prototypeBrick) { }
		#endregion
	}
	#endregion
#endif
}
