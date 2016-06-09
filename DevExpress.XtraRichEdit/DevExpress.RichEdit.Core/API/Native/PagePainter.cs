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
using Page = DevExpress.XtraRichEdit.Layout.Page;
using DevExpress.XtraRichEdit.Layout.Export;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraRichEdit.API.Layout {
	#region PageRenderer
	class PageRenderer : LayoutVisitor {
		PagePainter painter;
		internal PageRenderer(PagePainter painter) {
			this.painter = painter;
		}
		protected PagePainter PagePainter { get { return painter; } }
		protected LayoutPage Page { get { return PagePainter.Page; ; } }
		protected DocumentLayoutExporter Exporter { get { return PagePainter.Exporter; } }
		internal override bool VisitCommentsManually { get { return PagePainter.DrawCommentsManually; } }
		bool ShouldOptimizeBox(LayoutElement box) {
			return box.Bounds.Height < Exporter.MinReadableTextHeight;
		}
		bool ShouldDrawRow(LayoutRow row) {
			return Exporter.ShouldExportRow(row.ModelRow);
		}
		Rectangle GetDrawingBounds(Rectangle bounds) {
			Rectangle drawingBounds = bounds;
			drawingBounds.Offset(Page.Bounds.Location);
			return drawingBounds;
		}
		bool ShouldExportRowBackground(RowExtendedBoxes rowExtendedBoxes) {
			if (rowExtendedBoxes.HighlightAreas != null && rowExtendedBoxes.HighlightAreas.Count > 0)
				return true;
			if (rowExtendedBoxes.FieldHighlightAreas != null && rowExtendedBoxes.FieldHighlightAreas.Count > 0)
				return true;
			if (rowExtendedBoxes.RangePermissionHighlightAreas != null && rowExtendedBoxes.RangePermissionHighlightAreas.Count > 0)
				return true;
			if (rowExtendedBoxes.CommentHighlightAreas != null && rowExtendedBoxes.CommentHighlightAreas.Count > 0)
				return true;
			return false;
		}
		#region VisitPage
		protected internal override void VisitPage(LayoutPage page) {
			Exporter.ExportPage(page.ModelPage, () => {
#if !DXPORTABLE
				PrintingPageCanvas printingCanvas = PagePainter.Canvas as PrintingPageCanvas;
				if (printingCanvas != null)
					printingCanvas.StartDrawing();
#endif
				PagePainter.DrawPage(page);
			});
		}
		#endregion
		#region VisitPageArea
		protected internal override void VisitPageArea(LayoutPageArea pageArea) {
			PagePainter.DrawPageArea(pageArea);
			Exporter.AfterExportPageArea();
		}
		#endregion
		#region VisitColumn
		protected internal override void VisitColumn(LayoutColumn column) {
			LayoutHeaderFooterBase headerFooter = column.Parent as LayoutHeaderFooterBase;
			RectangleF oldClipBounds = RectangleF.Empty;
			if (headerFooter != null) {
				RectangleF clipBounds = Exporter.CalculateHeaderFooterClipBounds(headerFooter.ModelHeaderFooter.ContentBounds);
				oldClipBounds = Exporter.BeginHeaderFooterExport(clipBounds);
			}
			Exporter.PushBackgroundLayer();
			Exporter.ColumnClipBounds = Exporter.GetClipBounds();
			PagePainter.DrawColumn(column);
			Exporter.SetClipBounds(Exporter.ColumnClipBounds);
			Exporter.PopBackgroundLayer();
			if (headerFooter != null)
				Exporter.EndHeaderFooterExport(oldClipBounds);
		}
		#endregion
		#region VisitComments
		internal override void VisitComments(LayoutPage page) {
			if (Exporter.ShouldExportComments(page.ModelPage)) {
				Exporter.BeforeExportComments(page.ModelPage);
				PagePainter.DrawComments(page);
			}
		}
		#endregion
		#region VisitComment
		protected internal override void VisitComment(LayoutComment comment) {
			GraphicsDocumentLayoutExporter exporter = Exporter as GraphicsDocumentLayoutExporter;
			Rectangle pageContentClipping = Rectangle.Empty;
			Rectangle oldVisibleBounds = Rectangle.Empty;
			if (exporter != null) {
				pageContentClipping = exporter.Bounds;
				exporter.Bounds = comment.ContentBounds;
				oldVisibleBounds = exporter.VisibleBounds;
				if (exporter != null)
					exporter.VisibleBounds = Rectangle.Empty;
			}
			RectangleF oldClipRect = RectangleF.Empty;
			RectangleF clipRect = RectangleF.Empty;
			DevExpress.XtraRichEdit.Model.PieceTable oldPieceTable = null;
			if (!Exporter.BeginExportCompositeObject(GetDrawingBounds(comment.ContentBounds), comment.PieceTable, ref oldClipRect, ref clipRect, ref oldPieceTable))
				return;
			Exporter.PushBackgroundLayer();
			Exporter.BeforeExportComment(comment.ModelComment);
			if (Exporter.ShouldExportComment(comment.ModelComment)) {
#if !DXPORTABLE
				DevExpress.XtraPrinting.PanelBrick oldContainer = null;
				Point oldOffset = Point.Empty;
				Exporter.BeginExportComment(comment.ModelComment, ref oldContainer, ref oldOffset);
				PrintingPageCanvas printingCanvas = PagePainter.Canvas as PrintingPageCanvas;
				if (printingCanvas != null)
					printingCanvas.BeginDrawComment(comment.ContentBounds.Location);
				PagePainter.DrawComment(comment);
				if (printingCanvas != null)
					printingCanvas.EndDrawComment();
				Exporter.EndExportComment(oldContainer, oldOffset);
#endif
			}
			Exporter.AfterExportComment(comment.ModelComment, Exporter.CalculateNewLocationBounds(Page.ModelPage.CommentBounds));
			Exporter.PopBackgroundLayer();
			Exporter.EndExportCompositeObject(oldClipRect, oldPieceTable);
			if (exporter != null) {
				exporter.Bounds = pageContentClipping;
				exporter.VisibleBounds = oldVisibleBounds;
			}
		}
		#endregion
		#region VisitHeader
		protected internal override void VisitHeader(LayoutHeader header) {
			RectangleF clipBounds = Exporter.CalculateHeaderFooterClipBounds(header.ModelHeaderFooter.ContentBounds);
			if (clipBounds == RectangleF.Empty)
				return;
			Exporter.PushBackgroundLayer();
			Exporter.ApplyActivePieceTable(header.ModelHeaderFooter);
			PagePainter.DrawHeader(header);
			Exporter.RectorePieceTable();
			Exporter.PopBackgroundLayer();
		}
		#endregion
		#region VisitFooter
		protected internal override void VisitFooter(LayoutFooter footer) {
			RectangleF clipBounds = Exporter.CalculateHeaderFooterClipBounds(footer.ModelHeaderFooter.ContentBounds);
			if (clipBounds == RectangleF.Empty)
				return;
			Exporter.PushBackgroundLayer();
			Exporter.ApplyActivePieceTable(footer.ModelHeaderFooter);
			PagePainter.DrawFooter(footer);
			Exporter.RectorePieceTable();
			Exporter.PopBackgroundLayer();
		}
		#endregion
		#region VisitTextBox
		protected internal override void VisitTextBox(LayoutTextBox textBox) {
#if DXPORTABLE
			DrawTextBoxCore(textBox);
#else
			if (textBox.RotationAngle != 0 && Exporter is PrintingDocumentExporter)
				DrawTextBoxAsImage(textBox);
			else
				DrawTextBoxCore(textBox);
#endif
		}
#if !DXPORTABLE
		void DrawTextBoxAsImage(LayoutTextBox textBox) {
			PrintingDocumentExporter oldExporter = (PrintingDocumentExporter)Exporter;
			oldExporter.ExportRotatedFloatingTextBox(textBox.ModelFloatingObject, (newExporter) => {
				PagePainter.Initialize(newExporter);
				DrawTextBoxCore(textBox);
			});
			PagePainter.Initialize(oldExporter);
		}
#endif
		void DrawTextBoxCore(LayoutTextBox textBox) {
			RectangleF oldClipRect = RectangleF.Empty;
			RectangleF clipRect = RectangleF.Empty;
			DevExpress.XtraRichEdit.Model.PieceTable oldPieceTable = null;
			bool transformApplied = Exporter.BeforeExportRotatedContent(textBox.ModelFloatingObject);
			try {
				Exporter.ExportFloatingObjectShape(textBox.ModelFloatingObject, textBox.AnchorRun.Shape);
#if !DXPORTABLE
				PrintingDocumentExporter printingExporter = Exporter as PrintingDocumentExporter;
				if (printingExporter != null)
					printingExporter.CurrentPage = textBox.ModelFloatingObject.DocumentLayout.Pages[0];
#endif
				Rectangle bounds = GetDrawingBounds(textBox.ContentBounds);
				if (!Exporter.BeginExportCompositeObject(bounds, ((IPieceTableContainer)textBox).PieceTable, ref oldClipRect, ref clipRect, ref oldPieceTable))
					return;
				Exporter.PushBackgroundLayer();
				Exporter.SetBackColor(textBox.AnchorRun.Shape.FillColor, bounds);
				PagePainter.DrawTextBox(textBox);
				Exporter.PopBackgroundLayer();
				Exporter.EndExportCompositeObject(oldClipRect, oldPieceTable);
#if !DXPORTABLE
				if (printingExporter != null)
					printingExporter.CurrentPage = null;
#endif
			}
			finally {
				Exporter.AfterExportRotatedContent(transformApplied);
			}
		}
		#endregion
		#region VisitTable
		protected internal override void VisitTable(LayoutTable table) {
			Exporter.BeforeExportTable(table.ModelTable);
			table.ModelTable.ExportBackground(Exporter);
			PagePainter.DrawTable(table);
			RectangleF clip = Exporter.ApplyClipBounds(Exporter.GetClipBounds());   
			table.ModelTable.ExportTo(Exporter);
			Exporter.RestoreClipBounds(clip);
		}
		#endregion
		#region VisitTableCell
		protected internal override void VisitTableCell(LayoutTableCell tableCell) {
			RectangleF oldClipBounds = Exporter.BeginExportTableCell(tableCell.ModelTableCell);
			PagePainter.DrawTableCell(tableCell);
			Exporter.EndExportTableCell(oldClipBounds);
		}
		#endregion
		#region VisitRowBackground
		internal override void VisitRowBackground(RowExtendedBoxes rowExtendedBoxes) {
			if (ShouldExportRowBackground(rowExtendedBoxes))
				PagePainter.DrawRowBackground(rowExtendedBoxes);
		}
		#endregion
		#region VisitRow
		protected internal override void VisitRow(LayoutRow row) {
			if (!ShouldDrawRow(row))
				return;
			Exporter.SetCurrentRow(row.ModelRow);
			PagePainter.DrawRow(row);
			Exporter.SetCurrentRow(null);
		}
		#endregion
		#region VisitUnderlineBoxes
		internal override void VisitUnderlineBoxes(UnderlineBoxCollection underlineBoxes) {
			if (underlineBoxes.Count == 0)
				return;
			LayoutRow currentRow = underlineBoxes[0].GetParentByType<LayoutRow>();
			if (!ShouldOptimizeBox(currentRow))
				PagePainter.DrawUnderlineBoxes(underlineBoxes);
		}
		#endregion
		#region VisitStrikeoutBoxes
		internal override void VisitStrikeoutBoxes(StrikeoutBoxCollection strikeoutBoxes) {
			if (strikeoutBoxes.Count == 0)
				return;
			LayoutRow currentRow = strikeoutBoxes[0].GetParentByType<LayoutRow>();
			if (!ShouldOptimizeBox(currentRow))
				PagePainter.DrawStrikeoutBoxes(strikeoutBoxes);
		}
		#endregion
		internal override void VisitBookmarkBoxes(BookmarkBoxCollection bookmakBoxes) {
			PagePainter.DrawBookmarkBoxes(bookmakBoxes);
		}
		protected internal override void VisitTableRow(LayoutTableRow tableRow) {
			PagePainter.DrawTableRow(tableRow);
		}
		protected internal override void VisitFloatingPicture(LayoutFloatingPicture floatingPicture) {
			PagePainter.DrawFloatingPicture(floatingPicture);
		}
		protected internal override void VisitPlainTextBox(PlainTextBox plainTextBox) {
			PagePainter.DrawPlainTextBox(plainTextBox);
		}
		protected internal override void VisitSpecialTextBox(PlainTextBox specialTextBox) {
			PagePainter.DrawSpecialTextBox(specialTextBox);
		}
		protected internal override void VisitInlinePictureBox(InlinePictureBox inlinePictureBox) {
			PagePainter.DrawInlinePictureBox(inlinePictureBox);
		}
		protected internal override void VisitFloatingObjectAnchorBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
			PagePainter.DrawFloatingObjectAnchorBox(floatingObjectAnchorBox);
		}
		protected internal override void VisitSpaceBox(PlainTextBox spaceBox) {
			PagePainter.DrawSpaceBox(spaceBox);
		}
		protected internal override void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
			PagePainter.DrawParagraphMarkBox(paragraphMarkBox);
		}
		protected internal override void VisitSectionBreakBox(PlainTextBox sectionBreakBox) {
			PagePainter.DrawSectionBreakBox(sectionBreakBox);
		}
		protected internal override void VisitLineBreakBox(PlainTextBox lineBreakBox) {
			PagePainter.DrawLineBreakBox(lineBreakBox);
		}
		protected internal override void VisitPageBreakBox(PlainTextBox pageBreakBox) {
			PagePainter.DrawPageBreakBox(pageBreakBox);
		}
		protected internal override void VisitColumnBreakBox(PlainTextBox columnBreakBox) {
			PagePainter.DrawColumnBreakBox(columnBreakBox);
		}
		protected internal override void VisitHyphenBox(PlainTextBox hyphenBox) {
			PagePainter.DrawHyphenBox(hyphenBox);
		}
		protected internal override void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
			PagePainter.DrawTabSpaceBox(tabSpaceBox);
		}
		protected internal override void VisitPageNumberBox(PlainTextBox pageNumberBox) {
			PagePainter.DrawPageNumberBox(pageNumberBox);
		}
		protected internal override void VisitNumberingListMarkBox(NumberingListMarkBox numberingListMarkBox) {
			PagePainter.DrawNumberingListMarkBox(numberingListMarkBox);
		}
		protected internal override void VisitNumberingListWithSeparatorBox(NumberingListWithSeparatorBox numberingListWithSeparatorBox) {
			PagePainter.DrawNumberingListWithSeparatorBox(numberingListWithSeparatorBox);
		}
		protected internal override void VisitUnderlineBox(UnderlineBox underlineBox) {
			PagePainter.DrawUnderlineBox(underlineBox);
		}
		protected internal override void VisitStrikeoutBox(StrikeoutBox strikeoutBox) {
			PagePainter.DrawStrikeoutBox(strikeoutBox);
		}
		protected internal override void VisitHighlightAreaBox(HighlightAreaBox highlightAreaBox) {
			PagePainter.DrawHighlightAreaBox(highlightAreaBox);
		}
		protected internal override void VisitBookmarkStartBox(BookmarkBox bookmarkStartBox) {
			PagePainter.DrawBookmarkStartBox(bookmarkStartBox);
		}
		protected internal override void VisitBookmarkEndBox(BookmarkBox bookmarkEndBox) {
			PagePainter.DrawBookmarkEndBox(bookmarkEndBox);
		}
		protected internal override void VisitLineNumberBox(LineNumberBox lineNumberBox) {
			PagePainter.DrawLineNumberBox(lineNumberBox);
		}
		internal override void VisitCustomRunBox(PlainTextBox customRunBox) {
			PagePainter.DrawCustomRunBox(customRunBox);
		}
		internal override void VisitDataContainerRunBox(PlainTextBox dataContainerRunBox) {
			PagePainter.DrawDataContainerRunBox(dataContainerRunBox);
		}
		protected internal override void VisitFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox) {
			if (Exporter.CanExportFieldHighlightAreas)
				PagePainter.DrawFieldHighlightAreaBox(fieldHighlightAreaBox);
		}
		protected internal override void VisitRangePermissionHighlightAreaBox(RangePermissionHighlightAreaBox rangePermissionHighlightAreaBox) {
			if (Exporter.CanExportRangePermissionHighlightAreas)
				PagePainter.DrawRangePermissionHighlightAreaBox(rangePermissionHighlightAreaBox);
		}
		protected internal override void VisitRangePermissionStartBox(RangePermissionBox rangePermissionStartBox) {
			PagePainter.DrawRangePermissionStartBox(rangePermissionStartBox);
		}
		protected internal override void VisitRangePermissionEndBox(RangePermissionBox rangePermissionEndBox) {
			PagePainter.DrawRangePermissionEndBox(rangePermissionEndBox);
		}
		protected internal override void VisitCommentHighlightAreaBox(CommentHighlightAreaBox commentHighlightAreaBox) {
			PagePainter.DrawCommentHighlightAreaBox(commentHighlightAreaBox);
		}
		protected internal override void VisitCommentStartBox(CommentBox commentStartBox) {
			PagePainter.DrawCommentStartBox(commentStartBox);
		}
		protected internal override void VisitCommentEndBox(CommentBox commentEndBox) {
			PagePainter.DrawCommentEndBox(commentEndBox);
		}
		internal override void VisitFloatingParagraphFrame(DevExpress.XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) {
			PagePainter.DrawFloatingParagraphFrame(paragraphFrame);
		}
		internal override void VisitParagraphFrame(XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) {
			PagePainter.DrawParagraphFrame(paragraphFrame);
		}
		internal override void VisitErrorBoxes() {
			PagePainter.DrawErrorBoxes();
		}
		internal override void VisitImeBoxes() {
			PagePainter.DrawImeBoxes();
		}
		#region Base visitor methods
		internal void DefaultVisitPage(LayoutPage page) {
			base.VisitPage(page);
		}
		internal void DefaultVisitPageArea(LayoutPageArea pageArea) {
			base.VisitPageArea(pageArea);
		}
		internal void DefaultVisitColumn(LayoutColumn column) {
			base.VisitColumn(column);
		}
		internal void DefaultVisitComments(LayoutPage page) {
			base.VisitComments(page);
		}
		internal void DefaultVisitComment(LayoutComment comment) {
			base.VisitComment(comment);
		}
		internal void DefaultVisitHeader(LayoutHeader header) {
			base.VisitHeader(header);
		}
		internal void DefaultVisitFooter(LayoutFooter footer) {
			base.VisitFooter(footer);
		}
		internal void DefaultVisitTextBox(LayoutTextBox textBox) {
			base.VisitTextBox(textBox);
		}
		internal void DefaultVisitTable(LayoutTable table) {
			base.VisitTable(table);
		}
		internal void DefaultVisitTableRow(LayoutTableRow tableRow) {
			base.VisitTableRow(tableRow);
		}
		internal void DefaultVisitTableCell(LayoutTableCell tableCell) {
			base.VisitTableCell(tableCell);
		}
		internal void DefaultVisitRowBackground(RowExtendedBoxes rowExtendedBoxes) {
			base.VisitRowBackground(rowExtendedBoxes);
		}
		internal void DefaultVisitRow(LayoutRow row) {
			base.VisitRow(row);
		}
		internal void DefaultVisitUnderlineBoxes(UnderlineBoxCollection underlineBoxes) {
			base.VisitUnderlineBoxes(underlineBoxes);
		}
		internal void DefaultVisitStrikeoutBoxes(StrikeoutBoxCollection strikeoutBoxes) {
			base.VisitStrikeoutBoxes(strikeoutBoxes);
		}
		internal void DefaultVisitBookmarkBoxes(BookmarkBoxCollection bookmarkBoxes) {
			base.VisitBookmarkBoxes(bookmarkBoxes);
		}
		internal void DefaultVisitHiddenTextUnderlineBoxes(HiddenTextUnderlineBoxCollection collection) {
			base.VisitHiddenTextUnderlineBoxes(collection);
		}
		internal void DefaultVisitSpecialAreaTextBoxes(LayoutElementCollection<SpecialAreaBox> collection) {
			base.VisitSpecialAreaTextBoxes(collection);
		}
		#endregion
	}
	#endregion
	#region NotPrintableElementsRenderer
	class NotPrintableElementsRenderer : PageRenderer {
		Pen pen;
		public NotPrintableElementsRenderer(PagePainter pagePainter)
			: base(pagePainter) {
		}
		internal override bool VisitCommentsManually { get { return true; } }
		protected internal override void VisitHiddenTextUnderlineBox(HiddenTextUnderlineBox hiddenTextUnderlineBox) {
			PagePainter.DrawHiddenTextUnderlineBox(hiddenTextUnderlineBox, this.pen);
		}
		internal override void VisitHiddenTextUnderlineBoxes(HiddenTextUnderlineBoxCollection collection) {
			this.pen = Exporter.BeginExportNotPrintableBoxes();
			try {
				PagePainter.DrawHiddenTextUnderlineBoxes(collection);
			}
			finally {
				Exporter.EndExportNotPrintableBoxes(this.pen);
				this.pen = null;
			}
		}
		internal override void VisitSpecialAreaBox(SpecialAreaBox specialAreaBox) {
			PagePainter.DrawSpecialAreaBox(specialAreaBox, this.pen);
		}
		internal override void VisitSpecialAreaTextBoxes(LayoutElementCollection<SpecialAreaBox> collection) {
			if (!Exporter.DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText)
				return;
			this.pen = Exporter.BeginExportNotPrintableBoxes();
			try {
				PagePainter.DrawSpecialAreaBoxes(collection);
			}
			finally {
				Exporter.EndExportNotPrintableBoxes(this.pen);
				this.pen = null;
			}
		}
		protected internal override void VisitColumnBreakBox(PlainTextBox columnBreakBox) {
		}
		protected internal override void VisitCommentHighlightAreaBox(CommentHighlightAreaBox commentHighlightAreaBox) {
		}
		internal override void VisitCustomRunBox(PlainTextBox customRunBox) {
		}
		internal override void VisitDataContainerRunBox(PlainTextBox dataContainerRunBox) {
		}
		protected internal override void VisitFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox) {
		}
		protected internal override void VisitFloatingObjectAnchorBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
		}
		internal override void VisitFloatingParagraphFrame(XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) {
		}
		protected internal override void VisitFloatingPicture(LayoutFloatingPicture floatingPicture) {
		}
		protected internal override void VisitHighlightAreaBox(HighlightAreaBox highlightAreaBox) {
		}
		protected internal override void VisitHyphenBox(PlainTextBox hyphenBox) {
		}
		protected internal override void VisitInlinePictureBox(InlinePictureBox inlinePictureBox) {
		}
		protected internal override void VisitLineBreakBox(PlainTextBox lineBreakBox) {
		}
		protected internal override void VisitLineNumberBox(LineNumberBox lineNumberBox) {
		}
		protected internal override void VisitNumberingListMarkBox(NumberingListMarkBox numberingListMarkBox) {
		}
		protected internal override void VisitNumberingListWithSeparatorBox(NumberingListWithSeparatorBox numberingListWithSeparatorBox) {
		}
		protected internal override void VisitPageBreakBox(PlainTextBox pageBreakBox) {
		}
		protected internal override void VisitPageNumberBox(PlainTextBox pageNumberBox) {
		}
		internal override void VisitParagraphFrame(XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) {
		}
		protected internal override void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
		}
		protected internal override void VisitPlainTextBox(PlainTextBox plainTextBox) {
		}
		protected internal override void VisitRangePermissionHighlightAreaBox(RangePermissionHighlightAreaBox rangePermissionHighlightAreaBox) {
		}
		protected internal override void VisitSectionBreakBox(PlainTextBox sectionBreakBox) {
		}
		protected internal override void VisitSpaceBox(PlainTextBox spaceBox) {
		}
		protected internal override void VisitSpecialTextBox(PlainTextBox specialTextBox) {
		}
		protected internal override void VisitStrikeoutBox(StrikeoutBox strikeoutBox) {
		}
		protected internal override void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
		}
		protected internal override void VisitUnderlineBox(UnderlineBox underlineBox) {
		}
	}
	#endregion
	#region PagePainter
	public class PagePainter {
		#region Fields
		LayoutPage page;
		DocumentLayoutExporter exporter;
		PageCanvas canvas;
		PageRenderer pageRenderer;
		bool drawCommentsManually;
		#endregion
		public PagePainter()
			: this(false) {
		}
		internal PagePainter(bool drawOnlyNotPrintableElements) {
			this.drawCommentsManually = false;
			this.pageRenderer = drawOnlyNotPrintableElements ? new NotPrintableElementsRenderer(this) : new PageRenderer(this);
		}
		public LayoutPage Page { get { return page; } }
		public PageCanvas Canvas { get { return canvas; } }
		internal DocumentLayoutExporter Exporter { get { return exporter; } }
		internal bool DrawCommentsManually { get { return drawCommentsManually; } set { drawCommentsManually = value; } }
		#region Initialize
		internal void Initialize(DocumentLayoutExporter exporter, LayoutPage page) {
			this.page = page;
			Initialize(exporter);
		}
		internal void Initialize(DocumentLayoutExporter exporter) {
			this.exporter = exporter;
			if (exporter is GraphicsDocumentLayoutExporter)
				this.canvas = new GdiPageCanvas(exporter.Painter, Exporter.DocumentModel);
			else {
#if !DXPORTABLE
				PrintingDocumentExporter printingExporter = exporter as PrintingDocumentExporter;
				this.canvas = printingExporter != null ? new PrintingPageCanvas(printingExporter) : null;
#endif
			}
		}
		#endregion
		internal virtual void Draw() {
			this.pageRenderer.Visit(Page);
		}
		public virtual void DrawPage(LayoutPage page) {
			this.pageRenderer.DefaultVisitPage(Page);
		}
		public virtual void DrawPageArea(LayoutPageArea pageArea) {
			this.pageRenderer.DefaultVisitPageArea(pageArea);
		}
		public virtual void DrawColumn(LayoutColumn column) {
			this.pageRenderer.DefaultVisitColumn(column);
		}
		internal void DrawComments(LayoutPage page) {
			this.pageRenderer.DefaultVisitComments(page);
		}
		public virtual void DrawComment(LayoutComment comment) {
			this.pageRenderer.DefaultVisitComment(comment);
		}
		internal void DrawCommentManually(LayoutComment comment) {
			this.pageRenderer.VisitComment(comment);
		}
		public virtual void DrawHeader(LayoutHeader header) {
			this.pageRenderer.DefaultVisitHeader(header);
		}
		public virtual void DrawFooter(LayoutFooter footer) {
			this.pageRenderer.DefaultVisitFooter(footer);
		}
		internal void DrawFloatingParagraphFrame(DevExpress.XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) {
			Exporter.ExportParagraphFrameBox(paragraphFrame);
		}
		internal void DrawParagraphFrame(DevExpress.XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) {
			Exporter.ExportParagraphFrame(paragraphFrame);
		}
		public virtual void DrawTextBox(LayoutTextBox textBox) {
			this.pageRenderer.DefaultVisitTextBox(textBox);
		}
		public virtual void DrawFloatingPicture(LayoutFloatingPicture floatingPicture) {
			Exporter.ExportFloatingObjectBox(floatingPicture.ModelFloatingObject);
		}
		public virtual void DrawTable(LayoutTable table) {
			this.pageRenderer.DefaultVisitTable(table);
		}
		public virtual void DrawTableRow(LayoutTableRow tableRow) {
			this.pageRenderer.DefaultVisitTableRow(tableRow);
		}
		public virtual void DrawTableCell(LayoutTableCell tableCell) {
			this.pageRenderer.DefaultVisitTableCell(tableCell);
		}
		internal void DrawRowBackground(RowExtendedBoxes rowExtendedBoxes) {
			this.pageRenderer.DefaultVisitRowBackground(rowExtendedBoxes);
		}
		public virtual void DrawRow(LayoutRow row) {
			this.pageRenderer.DefaultVisitRow(row);
		}
		public virtual void DrawPlainTextBox(PlainTextBox plainTextBox) {
			Exporter.ExportTextBox((DevExpress.XtraRichEdit.Layout.TextBox)plainTextBox.ModelBox);
		}
		public virtual void DrawSpecialTextBox(PlainTextBox specialTextBox) {
			Exporter.ExportTextBox((DevExpress.XtraRichEdit.Layout.TextBox)specialTextBox.ModelBox);
		}
		public virtual void DrawInlinePictureBox(InlinePictureBox inlinePictureBox) {
			Exporter.ExportInlinePictureBox((DevExpress.XtraRichEdit.Layout.InlinePictureBox)inlinePictureBox.ModelBox);
		}
		public virtual void DrawFloatingObjectAnchorBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
		}
		public virtual void DrawSpaceBox(PlainTextBox spaceBox) {
			Exporter.ExportSpaceBox(spaceBox.ModelBox);
		}
		public virtual void DrawParagraphMarkBox(PlainTextBox paragraphMarkBox) {
			Exporter.ExportParagraphMarkBox((DevExpress.XtraRichEdit.Layout.ParagraphMarkBox)paragraphMarkBox.ModelBox);
		}
		public virtual void DrawSectionBreakBox(PlainTextBox sectionBreakBox) {
			Exporter.ExportSectionMarkBox((DevExpress.XtraRichEdit.Layout.SectionMarkBox)sectionBreakBox.ModelBox);
		}
		public virtual void DrawLineBreakBox(PlainTextBox lineBreakBox) {
			Exporter.ExportLineBreakBox((DevExpress.XtraRichEdit.Layout.LineBreakBox)lineBreakBox.ModelBox);
		}
		public virtual void DrawPageBreakBox(PlainTextBox pageBreakBox) {
			Exporter.ExportPageBreakBox((DevExpress.XtraRichEdit.Layout.PageBreakBox)pageBreakBox.ModelBox);
		}
		public virtual void DrawColumnBreakBox(PlainTextBox columnBreakBox) {
			Exporter.ExportColumnBreakBox((DevExpress.XtraRichEdit.Layout.ColumnBreakBox)columnBreakBox.ModelBox);
		}
		public virtual void DrawHyphenBox(PlainTextBox hyphen) {
			Exporter.ExportHyphenBox((DevExpress.XtraRichEdit.Layout.HyphenBox)hyphen.ModelBox);
		}
		public virtual void DrawTabSpaceBox(PlainTextBox tabSpaceBox) {
			Exporter.ExportTabSpaceBox((DevExpress.XtraRichEdit.Layout.TabSpaceBox)tabSpaceBox.ModelBox);
		}
		public virtual void DrawPageNumberBox(PlainTextBox pageNumberBox) {
			Exporter.ExportLayoutDependentTextBox((DevExpress.XtraRichEdit.Layout.LayoutDependentTextBox)pageNumberBox.ModelBox);
		}
		public virtual void DrawNumberingListMarkBox(NumberingListMarkBox numberingListMarkBox) {
			Exporter.ExportNumberingListBox((DevExpress.XtraRichEdit.Layout.NumberingListBox)numberingListMarkBox.ModelBox);
		}
		public virtual void DrawNumberingListWithSeparatorBox(NumberingListWithSeparatorBox numberingListWithSeparatorBox) {
			Exporter.ExportNumberingListBox((DevExpress.XtraRichEdit.Layout.NumberingListBox)numberingListWithSeparatorBox.ModelBox);
		}
		internal void DrawUnderlineBoxes(UnderlineBoxCollection underlineBoxes) {
			this.pageRenderer.DefaultVisitUnderlineBoxes(underlineBoxes);
		}
		internal void DrawStrikeoutBoxes(StrikeoutBoxCollection strikeoutBoxes) {
			this.pageRenderer.DefaultVisitStrikeoutBoxes(strikeoutBoxes);
		}
		internal void DrawBookmarkBoxes(BookmarkBoxCollection bookmarkBoxes) {
			this.pageRenderer.DefaultVisitBookmarkBoxes(bookmarkBoxes);
		}
		public virtual void DrawUnderlineBox(UnderlineBox underlineBox) {
			LayoutRow currentRow = underlineBox.GetParentByType<LayoutRow>();
			Exporter.ExportUnderlineBox(currentRow.ModelRow, (DevExpress.XtraRichEdit.Layout.UnderlineBox)underlineBox.ModelBox);
		}
		public virtual void DrawStrikeoutBox(StrikeoutBox strikeoutBox) {
			LayoutRow currentRow = strikeoutBox.GetParentByType<LayoutRow>();
			Exporter.ExportStrikeoutBox(currentRow.ModelRow, (DevExpress.XtraRichEdit.Layout.UnderlineBox)strikeoutBox.ModelBox);
		}
		public virtual void DrawHighlightAreaBox(HighlightAreaBox highlightAreaBox) {
			Exporter.ExportHighlightArea((DevExpress.XtraRichEdit.Layout.HighlightArea)highlightAreaBox.ModelBox);
		}
		internal virtual void DrawBookmarkStartBox(BookmarkBox bookmarkStartBox) {
			Exporter.ExportBookmarkStartBox((DevExpress.XtraRichEdit.Layout.VisitableDocumentIntervalBox)bookmarkStartBox.ModelBox);
		}
		internal virtual void DrawBookmarkEndBox(BookmarkBox bookmarkEndBox) {
			Exporter.ExportBookmarkEndBox((DevExpress.XtraRichEdit.Layout.VisitableDocumentIntervalBox)bookmarkEndBox.ModelBox);
		}
		internal virtual void DrawHiddenTextUnderlineBox(HiddenTextUnderlineBox hiddenTextUnderlineBox, Pen pen) {
			Exporter.ExportHiddenTextBox(hiddenTextUnderlineBox.ModelBox, pen);
		}
		internal virtual void DrawHiddenTextUnderlineBoxes(HiddenTextUnderlineBoxCollection collection) {
			this.pageRenderer.DefaultVisitHiddenTextUnderlineBoxes(collection);
		}
		public virtual void DrawLineNumberBox(LineNumberBox lineNumberBox) {
			Exporter.ExportLineNumberBox((DevExpress.XtraRichEdit.Layout.LineNumberBox)lineNumberBox.ModelBox);
		}
		internal virtual void DrawCustomRunBox(PlainTextBox customRunBox) {
			Exporter.ExportCustomRunBox((DevExpress.XtraRichEdit.Layout.CustomRunBox)customRunBox.ModelBox);
		}
		internal virtual void DrawDataContainerRunBox(PlainTextBox dataContainerRunBox) {
			Exporter.ExportDataContainerRunBox((DevExpress.XtraRichEdit.Layout.DataContainerRunBox)dataContainerRunBox.ModelBox);
		}
		public virtual void DrawFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox) {
			Exporter.ExportHighlightArea((DevExpress.XtraRichEdit.Layout.HighlightArea)fieldHighlightAreaBox.ModelBox);
		}
		public virtual void DrawRangePermissionHighlightAreaBox(RangePermissionHighlightAreaBox rangePermissionHighlightAreaBox) {
			Exporter.ExportHighlightArea((DevExpress.XtraRichEdit.Layout.HighlightArea)rangePermissionHighlightAreaBox.ModelBox);
		}
		internal virtual void DrawRangePermissionStartBox(RangePermissionBox rangePermissionStartBox) {
			Exporter.ExportBookmarkStartBox(rangePermissionStartBox.ModelBox);
		}
		internal virtual void DrawRangePermissionEndBox(RangePermissionBox rangePermissionEndBox) {
			Exporter.ExportBookmarkEndBox(rangePermissionEndBox.ModelBox);
		}
		public virtual void DrawCommentHighlightAreaBox(CommentHighlightAreaBox commentHighlightAreaBox) {
			Exporter.ExportHighlightArea((DevExpress.XtraRichEdit.Layout.HighlightArea)commentHighlightAreaBox.ModelBox);
		}
		internal virtual void DrawCommentStartBox(CommentBox commentStartBox) {
			Exporter.ExportCommentStartBox(commentStartBox.ModelBox);
		}
		internal virtual void DrawCommentEndBox(CommentBox commentEndBox) {
			Exporter.ExportCommentEndBox(commentEndBox.ModelBox);
		}
		internal virtual void DrawImeBoxes() {
			Exporter.ExportImeBoxes();
		}
		internal virtual void DrawErrorBoxes() {
			Exporter.ExportErrorBoxes();
		}
		internal virtual void DrawSpecialAreaBox(SpecialAreaBox specialAreaBox, Pen pen) {
			Exporter.ExportSpecialTextBox(specialAreaBox.ModelBox, pen);
		}
		internal virtual void DrawSpecialAreaBoxes(LayoutElementCollection<SpecialAreaBox> collection) {
			this.pageRenderer.DefaultVisitSpecialAreaTextBoxes(collection);
		}
		internal virtual void DrawCustomMarkBox(CustomMarkBox customMarkBox) {
			Exporter.ExportCustomMarkBox(customMarkBox.ModelBox);
		}
	}
	#endregion
	interface ISupportCustomPainting {
		void BeginCustomPaint();
		void EndCustomPaint();
	}
	public enum CanvasOwnerType { Control, Printer }
	#region PageCanvas
	public abstract class PageCanvas {
		DocumentModel documentModel;
		DocumentLayoutUnit drawingLayoutUnit;
		protected PageCanvas(DocumentModel documentModel) {
			this.documentModel = documentModel;
			this.drawingLayoutUnit = (DocumentLayoutUnit)DocumentModel.LayoutUnit;
		}
		protected DocumentModel DocumentModel { get { return documentModel; } }
		protected DocumentLayoutUnit DrawingLayoutUnit { get { return drawingLayoutUnit; } }
		#region DrawRectangle
		public abstract void DrawRectangle(RichEditPenBase pen, Rectangle bounds, DocumentLayoutUnit unit);
		public void DrawRectangle(RichEditPenBase pen, int x, int y, int width, int height, DocumentLayoutUnit unit) {
			DrawRectangle(pen, new Rectangle(x, y, width, height), unit);
		}
		public void DrawRectangle(RichEditPenBase pen, Rectangle bounds) {
			DrawRectangle(pen, bounds, DrawingLayoutUnit);
		}
		public void DrawRectangle(RichEditPenBase pen, int x, int y, int width, int height) {
			DrawRectangle(pen, new Rectangle(x, y, width, height));
		}
		#endregion
		#region DrawLine
		public abstract void DrawLine(RichEditPenBase pen, Point point1, Point point2, DocumentLayoutUnit unit);
		public void DrawLine(RichEditPenBase pen, int x1, int y1, int x2, int y2, DocumentLayoutUnit unit) {
			DrawLine(pen, new Point(x1, y1), new Point(x2, y2), unit);
		}
		public void DrawLine(RichEditPenBase pen, Point point1, Point point2) {
			DrawLine(pen, point1, point2, DrawingLayoutUnit);
		}
		public void DrawLine(RichEditPenBase pen, int x1, int y1, int x2, int y2) {
			DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
		}
		#endregion
		#region DrawLines
		public abstract void DrawLines(RichEditPenBase pen, Point[] points, DocumentLayoutUnit unit);
		public void DrawLines(RichEditPenBase pen, Point[] points) {
			DrawLines(pen, points, DrawingLayoutUnit);
		}
		#endregion
		#region DrawImage
		public abstract void DrawImage(OfficeImage image, Rectangle bounds, XtraPrinting.ImageSizeMode sizeMode, DocumentLayoutUnit unit);
		public void DrawImage(OfficeImage image, Point point, DocumentLayoutUnit unit) {
			DrawImage(image, ConvertToDrawingLayoutUnits(point, unit));
		}
		public void DrawImage(OfficeImage image, Rectangle bounds, XtraPrinting.ImageSizeMode sizeMode) {
			DrawImage(image, bounds, sizeMode, DrawingLayoutUnit);
		}
		public void DrawImage(OfficeImage image, Point point) {
			DrawImage(image, new Rectangle(point, Size.Empty), XtraPrinting.ImageSizeMode.Normal);
		}
		#endregion
		#region DrawString
		public abstract void DrawString(string str, Font font, RichEditBrushBase brush, Rectangle bounds, DocumentLayoutUnit unit);
		public void DrawString(string str, Font font, RichEditBrushBase brush, Point point, DocumentLayoutUnit unit) {
			DrawString(str, font, brush, ConvertToDrawingLayoutUnits(point, unit));
		}
		public void DrawString(string str, Font font, RichEditBrushBase brush, Rectangle bounds) {
			DrawString(str, font, brush, bounds, DrawingLayoutUnit);
		}
		public void DrawString(string str, Font font, RichEditBrushBase brush, Point point) {
			DrawString(str, font, brush, new Rectangle(point, Size.Empty));
		}
		#endregion
		#region DrawEllipse
		public abstract void DrawEllipse(RichEditPenBase pen, Rectangle bounds, DocumentLayoutUnit unit);
		public void DrawEllipse(RichEditPenBase pen, Rectangle bounds) {
			DrawEllipse(pen, bounds, DrawingLayoutUnit);
		}
		#endregion
		#region FillEllipse
		public abstract void FillEllipse(RichEditBrushBase brush, Rectangle bounds);
		public void FillEllipse(RichEditBrushBase brush, Rectangle bounds, DocumentLayoutUnit unit) {
			FillEllipse(brush, ConvertToDrawingLayoutUnits(bounds, unit));
		}
		#endregion
		#region FillRectangle
		public abstract void FillRectangle(RichEditBrushBase brush, Rectangle bounds);
		public void FillRectangle(RichEditBrushBase brush, Rectangle bounds, DocumentLayoutUnit unit) {
			FillRectangle(brush, ConvertToDrawingLayoutUnits(bounds, unit));
		}
		#endregion
		#region ConvertToDrawingLayoutUnits
		public int ConvertToDrawingLayoutUnits(int value, DocumentLayoutUnit unit) {
			if (DrawingLayoutUnit == unit)
				return value;
			switch (unit) {
				case DocumentLayoutUnit.Document:
					return DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(value);
				case DocumentLayoutUnit.Twip:
					return DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(value);
				case DocumentLayoutUnit.Pixel:
					return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(value);
				default:
					return value;
			}
		}
		public Point ConvertToDrawingLayoutUnits(Point point, DocumentLayoutUnit unit) {
			if (DrawingLayoutUnit == unit)
				return point;
			Point result = new Point();
			switch (unit) {
				case DocumentLayoutUnit.Document:
					result.X = DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(point.X);
					result.Y = DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(point.Y);
					break;
				case DocumentLayoutUnit.Twip:
					result.X = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(point.X);
					result.Y = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(point.Y);
					break;
				case DocumentLayoutUnit.Pixel:
					result.X = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.X);
					result.Y = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.Y);
					break;
			}
			return result;
		}
		public Rectangle ConvertToDrawingLayoutUnits(Rectangle rect, DocumentLayoutUnit unit) {
			if (DrawingLayoutUnit == unit)
				return rect;
			Rectangle result = Rectangle.Empty;
			switch (unit) {
				case DocumentLayoutUnit.Document:
					result = DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(rect);
					break;
				case DocumentLayoutUnit.Twip:
					int x = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(rect.X);
					int y = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(rect.Y);
					int width = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(rect.Width);
					int height = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(rect.Height);
					result = new Rectangle(x, y, width, height);
					break;
				case DocumentLayoutUnit.Pixel:
					result = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(rect);
					break;
			}
			return result;
		}
		#endregion
		internal virtual void Synchronize(PageCanvas canvas) {
		}
	}
	#endregion
	#region GdiPageCanvas
	public class GdiPageCanvas : PageCanvas {
		DevExpress.Office.Drawing.Painter innerPainter;
		internal GdiPageCanvas(DevExpress.Office.Drawing.Painter painter, DevExpress.XtraRichEdit.Model.DocumentModel documentModel)
			: base(documentModel) {
			this.innerPainter = painter;
		}
		DevExpress.Office.Drawing.Painter InnerPainter { get { return innerPainter; } }
		void BeginCustomPaint() {
			ISupportCustomPainting customPainter = InnerPainter as ISupportCustomPainting;
			if (customPainter != null)
				customPainter.BeginCustomPaint();
		}
		void EndCustomPaint() {
			ISupportCustomPainting customPainter = InnerPainter as ISupportCustomPainting;
			if (customPainter != null)
				customPainter.EndCustomPaint();
		}
		#region BeginDraw
		Pen BeginDraw(RichEditPenBase pen, DocumentLayoutUnit unit) {
			BeginCustomPaint();
			Pen result = pen.CreatePlatformIndependentPen();
			result.Width = Math.Max(1, ConvertToDrawingLayoutUnits(pen.Thickness, unit));
			return result;
		}
		Brush BeginDraw(RichEditBrushBase brush) {
			BeginCustomPaint();
			return new SolidBrush(brush.GetColor());
		}
		#endregion
		#region EndDraw
		void EndDraw(Brush brush) {
			brush.Dispose();
			EndCustomPaint();
		}
		void EndDraw(Pen pen) {
			pen.Dispose();
			EndCustomPaint();
		}
		#endregion
		#region DrawRectangle
		public override void DrawRectangle(RichEditPenBase pen, Rectangle bounds, DocumentLayoutUnit unit) {
			Pen platformPen = BeginDraw(pen, unit);
			try {
				InnerPainter.DrawRectangle(platformPen, ConvertToDrawingLayoutUnits(bounds, unit));
			}
			finally {
				EndDraw(platformPen);
			}
		}
		#endregion
		#region DrawLine
		public override void DrawLine(RichEditPenBase pen, Point point1, Point point2, DocumentLayoutUnit unit) {
			Pen platformPen = BeginDraw(pen, unit);
			try {
				int x1 = ConvertToDrawingLayoutUnits(point1.X, unit);
				int y1 = ConvertToDrawingLayoutUnits(point1.Y, unit);
				int x2 = ConvertToDrawingLayoutUnits(point2.X, unit);
				int y2 = ConvertToDrawingLayoutUnits(point2.Y, unit);
				InnerPainter.DrawLine(platformPen, x1, y1, x2, y2);
			}
			finally {
				EndDraw(platformPen);
			}
		}
		#endregion
		#region DrawLines
		public override void DrawLines(RichEditPenBase pen, Point[] points, DocumentLayoutUnit unit) {
			Pen platformPen = BeginDraw(pen, unit);
			try {
#if !DXPORTABLE
				PointF[] convertedPoints = new PointF[points.Length];
				for (int i = 0; i < convertedPoints.Length; i++)
					convertedPoints[i] = ConvertToDrawingLayoutUnits(points[i], unit);
				InnerPainter.DrawLines(platformPen, convertedPoints);
#endif
			}
			finally {
				EndDraw(platformPen);
			}
		}
		#endregion
		#region DrawImage
		public override void DrawImage(OfficeImage image, Rectangle bounds, DevExpress.XtraPrinting.ImageSizeMode sizeMode, DocumentLayoutUnit unit) {
			BeginCustomPaint();
			Size imageSize = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(image.NativeImage.Size);
			Rectangle convertedBounds = ConvertToDrawingLayoutUnits(bounds, unit);
			if (bounds.Size == Size.Empty)
				convertedBounds.Size = imageSize;
			InnerPainter.DrawImage(image, convertedBounds, imageSize, sizeMode);
			EndCustomPaint();
		}
		#endregion
		#region DrawString
		public override void DrawString(string str, Font font, RichEditBrushBase brush, Rectangle bounds, DocumentLayoutUnit unit) {
			BeginCustomPaint();
			float doubleFontSize = DocumentModel.LayoutUnitConverter.PointsToFontUnitsF(font.SizeInPoints) * 2;
			DevExpress.Office.Drawing.FontInfo fontInfo = DocumentModel.FontCache.CreateFontInfo(font.Name, (int)doubleFontSize, font.Bold, font.Italic, font.Strikeout, font.Underline);
			Size measuredSize = DocumentModel.FontCache.Measurer.MeasureString(str, fontInfo);
			Rectangle convertedBounds = ConvertToDrawingLayoutUnits(bounds, unit);
			if (bounds.Size == Size.Empty)
				convertedBounds.Size = measuredSize;
			bool useClipping = convertedBounds.Size.Width < measuredSize.Width || convertedBounds.Size.Height < measuredSize.Height;
			RectangleF oldClipBounds = RectangleF.Empty;
			if (useClipping)
				oldClipBounds = InnerPainter.ApplyClipBounds(convertedBounds);
			InnerPainter.DrawString(str, fontInfo, brush.GetColor(), convertedBounds);
			if (useClipping)
				InnerPainter.RestoreClipBounds(oldClipBounds);
			EndCustomPaint();
		}
		#endregion
		#region DrawEllipse
		public override void DrawEllipse(RichEditPenBase pen, Rectangle bounds, DocumentLayoutUnit unit) {
			Pen platformPen = BeginDraw(pen, unit);
			try {
				InnerPainter.DrawEllipse(platformPen, ConvertToDrawingLayoutUnits(bounds, unit));
			}
			finally {
				EndDraw(platformPen);
			}
		}
		#endregion
		#region FillRectangle
		public override void FillRectangle(RichEditBrushBase brush, Rectangle bounds) {
			Brush platformBrush = BeginDraw(brush);
			try {
				InnerPainter.FillRectangle(platformBrush, bounds);
			}
			finally {
				EndDraw(platformBrush);
			}
		}
		#endregion
		#region FillEllipse
		public override void FillEllipse(RichEditBrushBase brush, Rectangle bounds) {
			Brush platformBrush = BeginDraw(brush);
			try {
				InnerPainter.FillEllipse(platformBrush, bounds);
			}
			finally {
				EndDraw(platformBrush);
			}
		}
		#endregion
	}
	#endregion
	#region PrintingPageCanvas
#if !DXPORTABLE
	public class PrintingPageCanvas : PageCanvas {
		#region Fields
		PrintingDocumentExporter exporter;
		Point offset = Point.Empty;
		Queue<Action> drawingQueue;
		bool canDrawOnDemand = false;
		#endregion
		internal PrintingPageCanvas(PrintingDocumentExporter exporter)
			: base(exporter.DocumentModel) {
			this.exporter = exporter;
			this.drawingQueue = new Queue<Action>();
		}
		PrintingDocumentExporter Exporter { get { return exporter; } }
		Queue<Action> DrawingQueue { get { return drawingQueue; } }
		bool CanDrawOnDemand { get { return canDrawOnDemand; } set { canDrawOnDemand = value; } }
		internal void StartDrawing() {
			CanDrawOnDemand = true;
			ProcessDrawingQueue();
		}
		void ProcessDrawingQueue() {
			while (DrawingQueue.Count > 0) {
				Action drawingAction = DrawingQueue.Dequeue();
				drawingAction();
			}
		}
		internal override void Synchronize(PageCanvas canvas) {
			PrintingPageCanvas oldCanvas = canvas as PrintingPageCanvas;
			if (oldCanvas == null)
				return;
			while (oldCanvas.DrawingQueue.Count > 0) {
				DrawingQueue.Enqueue(oldCanvas.DrawingQueue.Dequeue());
			}
		}
		internal void BeginDrawComment(Point commentLocation) {
			this.offset = commentLocation;
		}
		internal void EndDrawComment() {
			this.offset = Point.Empty;
		}
		Rectangle GetRect(Rectangle rect) {
			rect.Offset(-this.offset.X, -this.offset.Y);
			return rect;
		}
		#region DrawRectangle
		public override void DrawRectangle(RichEditPenBase pen, Rectangle rectangle, DocumentLayoutUnit unit) {
			Rectangle convertedRect = ConvertToDrawingLayoutUnits(rectangle, unit);
			int thickness = ConvertToDrawingLayoutUnits(pen.Thickness, unit);
			thickness = Math.Max(1, DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(thickness));
			if (CanDrawOnDemand)
				DrawRectangleCore(pen.GetColor(), Color.Transparent, thickness, pen.DashStyle, convertedRect);
			else
				DrawingQueue.Enqueue(() => { DrawRectangleCore(pen.GetColor(), Color.Transparent, thickness, pen.DashStyle, convertedRect); });
		}
		void DrawRectangleCore(Color foreColor, Color backColor, int lineThickness, RichEditDashStyle lineStyle, Rectangle rectangle) {
			DevExpress.XtraPrinting.VisualBrick brick = new XtraPrinting.VisualBrick(XtraPrinting.BorderSide.All, lineThickness, foreColor, backColor);
			DevExpress.XtraPrinting.Native.VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, Exporter.GetDrawingBounds(GetRect(rectangle)));
			brick.BorderDashStyle = (XtraPrinting.BorderDashStyle)lineStyle;
			Exporter.AddBrickToCurrentPage(brick);
		}
		#endregion
		#region DrawLine
		public override void DrawLine(RichEditPenBase pen, Point point1, Point point2, DocumentLayoutUnit unit) {
			if (CanDrawOnDemand)
				DrawLineCore(pen, point1, point2, unit);
			else
				DrawingQueue.Enqueue(() => { DrawLineCore(pen, point1, point2, unit); });
		}
		void DrawLineCore(RichEditPenBase pen, Point point1, Point point2, DocumentLayoutUnit unit) {
			Point convertedPoint1 = ConvertToDrawingLayoutUnits(point1, unit);
			Point convertedPoint2 = ConvertToDrawingLayoutUnits(point2, unit);
			int thickness = Math.Max(1, ConvertToDrawingLayoutUnits(pen.Thickness, unit));
			int additionalOffset = (int)thickness / 2;
			if (point1.X == point2.X) {
				convertedPoint1.Offset(-additionalOffset, 0);
				convertedPoint2.Offset(-additionalOffset, 0);
			}
			if (point1.Y == point2.Y) {
				convertedPoint1.Offset(0, -additionalOffset);
				convertedPoint2.Offset(0, -additionalOffset);
			}
			DevExpress.XtraPrinting.LineBrick brick = new XtraPrinting.LineBrick();
			Rectangle bounds = Rectangle.Round(DevExpress.XtraPrinting.Native.RectF.FromPoints(convertedPoint1, convertedPoint2));
			if (bounds.Width == 0)
				bounds.Width = thickness;
			if (bounds.Height == 0)
				bounds.Height = thickness;
			DevExpress.XtraPrinting.Native.VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, Exporter.GetDrawingBounds(GetRect(bounds)));
			brick.LineDirection = DevExpress.XtraPrinting.LineBrick.GetDirection(convertedPoint1, convertedPoint2);
			brick.ForeColor = pen.GetColor();
			brick.BackColor = Color.Transparent;
			brick.LineStyle = (DashStyle)pen.DashStyle;
			brick.LineWidth = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(thickness);
			Exporter.AddBrickToCurrentPage(brick);
		}
		#endregion
		#region DrawLines
		public override void DrawLines(RichEditPenBase pen, Point[] points, DocumentLayoutUnit unit) {
			for (int i = 0; i < points.Length - 1; i++)
				DrawLine(pen, points[i], points[i + 1], unit);
		}
		#endregion
		#region DrawImage
		public override void DrawImage(OfficeImage image, Rectangle bounds, DevExpress.XtraPrinting.ImageSizeMode sizeMode, DocumentLayoutUnit unit) {
			if (CanDrawOnDemand)
				DrawImageCore(image, bounds, sizeMode, unit);
			else
				DrawingQueue.Enqueue(() => { DrawImageCore(image, bounds, sizeMode, unit); });
		}
		void DrawImageCore(OfficeImage image, Rectangle bounds, DevExpress.XtraPrinting.ImageSizeMode sizeMode, DocumentLayoutUnit unit) {
			Size imageSize = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(image.NativeImage.Size);
			Rectangle convertedBounds = ConvertToDrawingLayoutUnits(bounds, unit);
			if (bounds.Size == Size.Empty)
				convertedBounds.Size = imageSize;
			DevExpress.Office.Printing.OfficeImageBrick brick = new Office.Printing.OfficeImageBrick(DocumentModel.LayoutUnitConverter);
			DevExpress.XtraPrinting.Native.VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, Exporter.GetDrawingBounds(GetRect(convertedBounds)));
			brick.DisposeImage = false;
			brick.SizeMode = sizeMode;
			brick.BackColor = Color.Transparent;
			brick.Image = image.NativeImage;
			Exporter.AddBrickToCurrentPage(brick);
		}
		#endregion
		#region DrawString
		public override void DrawString(string str, Font font, RichEditBrushBase brush, Rectangle bounds, DocumentLayoutUnit unit) {
			if (CanDrawOnDemand)
				DrawStringCore(str, font, brush, bounds, unit);
			else
				DrawingQueue.Enqueue(() => { DrawStringCore(str, font, brush, bounds, unit); });
		}
		void DrawStringCore(string str, Font font, RichEditBrushBase brush, Rectangle bounds, DocumentLayoutUnit unit) {
			DevExpress.Office.Printing.OfficeTextBrick brick = new Office.Printing.OfficeTextBrick(DocumentModel.LayoutUnitConverter);
			Rectangle convertedBounds = ConvertToDrawingLayoutUnits(bounds, unit);
			if (bounds.Size == Size.Empty) {
				float doubleFontSize = DocumentModel.LayoutUnitConverter.PointsToFontUnitsF(font.SizeInPoints) * 2;
				DevExpress.Office.Drawing.FontInfo fontInfo = DocumentModel.FontCache.CreateFontInfo(font.Name, (int)doubleFontSize, font.Bold, font.Italic, font.Strikeout, font.Underline);
				convertedBounds.Size = DocumentModel.FontCache.Measurer.MeasureString(str, fontInfo);
			}
			DevExpress.XtraPrinting.Native.VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, Exporter.GetDrawingBounds(GetRect(convertedBounds)));
			brick.Font = font;
			brick.BackColor = Color.Transparent;
			brick.ForeColor = brush.GetColor();
			brick.Text = str;
			Exporter.AddBrickToCurrentPage(brick);
		}
		#endregion
		#region DrawEllipse
		public override void DrawEllipse(RichEditPenBase pen, Rectangle bounds, DocumentLayoutUnit unit) {
			Rectangle convertedBounds = ConvertToDrawingLayoutUnits(bounds, unit);
			int thickness = Math.Max(1, ConvertToDrawingLayoutUnits(pen.Thickness, unit));
			int additionalSize = (int)thickness / 2;
			convertedBounds.Inflate(additionalSize, additionalSize);
			if (CanDrawOnDemand)
				DrawEllipseCore(pen.GetColor(), Color.Transparent, thickness, pen.DashStyle, convertedBounds);
			else
				DrawingQueue.Enqueue(() => { DrawEllipseCore(pen.GetColor(), Color.Transparent, thickness, pen.DashStyle, convertedBounds); });
		}
		void DrawEllipseCore(Color foreColor, Color backColor, int lineThickness, RichEditDashStyle lineStyle, Rectangle bounds) {
			DevExpress.XtraPrinting.ShapeBrick brick = new XtraPrinting.ShapeBrick();
			brick.Shape = new DevExpress.XtraPrinting.Shape.ShapeEllipse();
			DevExpress.XtraPrinting.Native.VisualBrickHelper.InitializeBrick(brick, Exporter.PrintingSystem, Exporter.GetDrawingBounds(GetRect(bounds)));
			brick.ForeColor = foreColor;
			brick.BackColor = Color.Transparent;
			brick.FillColor = backColor;
			brick.LineWidth = lineThickness;
			brick.LineStyle = (DashStyle)lineStyle;
			Exporter.AddBrickToCurrentPage(brick);
		}
		#endregion
		#region FillEllipse
		public override void FillEllipse(RichEditBrushBase brush, Rectangle bounds) {
			Color color = brush.GetColor();
			if (CanDrawOnDemand)
				DrawEllipseCore(color, color, 1, RichEditDashStyle.Solid, bounds);
			else
				DrawingQueue.Enqueue(() => { DrawEllipseCore(color, color, 1, RichEditDashStyle.Solid, bounds); });
		}
		#endregion
		#region FillRectangle
		public override void FillRectangle(RichEditBrushBase brush, Rectangle bounds) {
			Color color = brush.GetColor();
			if (CanDrawOnDemand)
				DrawRectangleCore(color, color, 1, RichEditDashStyle.Solid, bounds);
			else
				DrawingQueue.Enqueue(() => { DrawRectangleCore(color, color, 1, RichEditDashStyle.Solid, bounds); });
		}
		#endregion
	}
#endif
	#endregion
}
namespace DevExpress.XtraRichEdit {
	public enum RichEditDashStyle {
		Solid = 0,
		Dash = 1,
		Dot = 2,
		DashDot = 3,
		DashDotDot = 4
	}
	public abstract class RichEditPenBase {
		int thickness;
		RichEditDashStyle dashStyle;
		protected RichEditPenBase(int thickness) {
			this.thickness = thickness;
			this.dashStyle = RichEditDashStyle.Solid;
		}
		public int Thickness { get { return thickness; } set { thickness = value; } }
		public RichEditDashStyle DashStyle { get { return dashStyle; } set { dashStyle = value; } }
		internal abstract Color GetColor();
		internal abstract Pen CreatePlatformIndependentPen();
	}
	public abstract class RichEditBrushBase {
		internal abstract Color GetColor();
	}
}
