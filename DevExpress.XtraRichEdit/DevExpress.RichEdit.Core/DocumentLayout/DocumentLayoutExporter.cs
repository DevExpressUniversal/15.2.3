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
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	internal delegate void ExportPageAction(Page page);
	internal interface IPageFloatingObjectExporter {
		void ExportPageAreaCore(Page page, PieceTable pieceTable, ExportPageAction action);
	}
	#region DocumentLayoutExporter (abstract class)
	public abstract class DocumentLayoutExporter : IDocumentLayoutExporter, IPageFloatingObjectExporter, IDisposable {
		#region DocumentLayoutExporterContext
		class DocumentLayoutExporterContext : DevExpress.Office.ICloneable<DocumentLayoutExporterContext> {
			Color backColor = DXColor.Empty;
			public Color BackColor { get { return backColor; } set { backColor = value; } }
			public void CopyFrom(DocumentLayoutExporterContext source) {
				BackColor = source.BackColor;
			}
			public DocumentLayoutExporterContext Clone() {
				DocumentLayoutExporterContext result = new DocumentLayoutExporterContext();
				result.BackColor = BackColor;
				return result;
			}
		}
		#endregion
		#region BackgroundLayer
		class BackgroundLayer {
			struct BackgroundItem {
				Color color;
				Rectangle bounds;
				public BackgroundItem(Color color, Rectangle bounds) {
					this.color = color;
					this.bounds = bounds;
				}
				public Color Color { get { return color; } }
				public Rectangle Bounds { get { return bounds; } }
			}
			List<BackgroundItem> backgroundItems = new List<BackgroundItem>();
			Rectangle layerBounds = Rectangle.Empty;
			public void SetBackColor(Color color, Rectangle bounds) {
				this.backgroundItems.Add(new BackgroundItem(color, bounds));
				UpdateBounds(bounds);
			}
			void UpdateBounds(Rectangle bounds) {
				if (this.layerBounds.IsEmpty)
					this.layerBounds = bounds;
				else {
					int left = Math.Min(this.layerBounds.Left, bounds.Left);
					int top = Math.Min(this.layerBounds.Top, bounds.Top);
					int right = Math.Max(this.layerBounds.Right, bounds.Right);
					int bottom = Math.Max(this.layerBounds.Bottom, bounds.Bottom);
					this.layerBounds = Rectangle.FromLTRB(left, top, right, bottom);
				}
			}
			public Color? GetBackColor(Rectangle bounds) {
				if (this.layerBounds.Contains(bounds)) {
					for (int i = backgroundItems.Count - 1; i >= 0; i--) {
						BackgroundItem item = this.backgroundItems[i];
						if (item.Bounds.Contains(bounds))
							return item.Color;
					}
				}
				return null;
			}
		}
		#endregion
		#region Fields
		readonly DocumentModel documentModel;
		PieceTable pieceTable;
		bool showWhitespace;
		int minReadableTextHeight;
		Row currentRow;
		TableCellViewInfo currentCell;
		Dictionary<PieceTable, HyperlinkCalculator> hyperlinkCalculators;
		Color currentBackColor = DXColor.Empty;
		Color currentCellBackColor = DXColor.Empty;
		Stack<DocumentLayoutExporterContext> contextStack;
		List<BackgroundLayer> backgroundLayers;
		#endregion
		protected DocumentLayoutExporter(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.pieceTable = documentModel.MainPieceTable;
			this.hyperlinkCalculators = new Dictionary<PieceTable, HyperlinkCalculator>();
			this.contextStack = new Stack<DocumentLayoutExporterContext>();
			this.backgroundLayers = new List<BackgroundLayer>();
			PushBackgroundLayer();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } set { pieceTable = value; } }
		public virtual bool ShowWhitespace { get { return showWhitespace; } set { showWhitespace = value; } }
		public int MinReadableTextHeight { get { return minReadableTextHeight; } set { minReadableTextHeight = value; } }
		public Row CurrentRow { get { return currentRow; } }
		public TableCellViewInfo CurrentCell { get { return currentCell; } }
		public abstract Painter Painter { get; }
		public Color CurrentCellBackColor { get { return currentCellBackColor; } set { currentCellBackColor = value; } }
		public bool ReadOnly { get; set; }
		public RectangleF ColumnClipBounds { get { return columnClipBounds; } set { columnClipBounds = value; } }
		internal virtual bool CanExportFieldHighlightAreas { get { return true; } }
		internal virtual bool CanExportRangePermissionHighlightAreas { get { return true; } }
		#endregion
		protected internal Color GetActualBackColor(Rectangle bounds) {
			return !DXColor.IsTransparentOrEmpty(CurrentCellBackColor) ? CurrentCellBackColor : GetBackColor(bounds);
		}
		protected internal void PushBackgroundLayer() {
			this.backgroundLayers.Add(new BackgroundLayer());
		}
		protected internal void PopBackgroundLayer() {
			this.backgroundLayers.RemoveAt(this.backgroundLayers.Count - 1);
		}
		protected internal Color GetBackColor(Rectangle bounds) {
			Color? color = null;
			int layerIndex = this.backgroundLayers.Count - 1;
			while (color == null && layerIndex >= 0) {
				color = this.backgroundLayers[layerIndex].GetBackColor(bounds);
				layerIndex--;
			}
			return color ?? DXColor.Empty;
		}
		protected internal void SetBackColor(Color color, Rectangle bounds) {
			this.backgroundLayers[this.backgroundLayers.Count - 1].SetBackColor(color, bounds);
		}
		protected internal virtual Rectangle CalcRowContentBounds(Row row) {
			return row.Bounds;
		}
		protected virtual void SetCurrentCell(TableCellViewInfo cell) {
			this.currentCell = cell;
		}
		internal virtual void BeforeExportTable(TableViewInfo table) { }
		protected internal virtual void ExportImeBoxes() { }
		protected internal virtual void ExportSpecialTextBox(SpecialTextBox box, Pen pen) { }
		protected internal virtual void ExportErrorBoxes() { }
		internal virtual Pen BeginExportNotPrintableBoxes() {
			return null;
		}
		internal virtual Rectangle CalculateNewLocationBounds(Rectangle bounds) {
			return bounds;
		}
#if !DXPORTABLE
		internal virtual void BeginExportComment(CommentViewInfo commentViewInfo, ref DevExpress.XtraPrinting.PanelBrick oldContainer, ref Point oldOffset) { }
		internal virtual void EndExportComment(DevExpress.XtraPrinting.PanelBrick oldContainer, Point oldOffset) { }
#endif
		internal virtual void EndExportNotPrintableBoxes(Pen pen) { }
		internal virtual void ExportHiddenTextBox(HiddenTextUnderlineBox box, Pen pen) { }
		protected internal string GetBoxText(Box box) {
			return box.GetText(PieceTable);
		}
		protected virtual int GetLastExportBoxInRowIndex(Row row) {
			return row.Boxes.Count - 1;
		}
		public virtual void ExportPage(DevExpress.XtraRichEdit.Layout.Page page, Action action) {
			action();
		}
		void IPageFloatingObjectExporter.ExportPageAreaCore(Page page, PieceTable pieceTable, ExportPageAction action) {
			ExportPageAreaCore(page, pieceTable, action);
		}
		void ExportPageAreaCore(Page page, PieceTable pieceTable, ExportPageAction action) {
			ExportFloatingObjects(page.BackgroundFloatingObjects, pieceTable);
			action(page);
			IList<FloatingObjectBox> floatingObjects = page.GetSortedNonBackgroundFloatingObjects();
			ExportFloatingObjects(floatingObjects, pieceTable);
			IList<ParagraphFrameBox> paragraphFrames = page.GetSortedParagraphFrames();
			ExportParagraphFrames(paragraphFrames, pieceTable);
		}
		internal void ApplyActivePieceTable(PageArea pageArea) {
			this.pieceTable = pageArea.PieceTable;
		}
		internal void RectorePieceTable() {
			this.pieceTable = documentModel.MainPieceTable;
		}
		private void ExportPageHeaderFooterBase(HeaderFooterPageAreaBase pageArea) {
			if (pageArea == null)
				return;
			RectangleF clipBounds = CalculateHeaderFooterClipBounds(pageArea.ContentBounds);
			if (clipBounds == RectangleF.Empty)
				return;
			ApplyActivePieceTable(pageArea);
			RectangleF oldClipBounds = BeginHeaderFooterExport(clipBounds);
			try {
				ExportPageHeaderFooter(pageArea);
			}
			finally {
				EndHeaderFooterExport(oldClipBounds);
				RectorePieceTable();
			}
		}
		protected virtual void ExportPageHeader(Page page) {
			ExportPageHeaderFooterBase(page.Header);
		}
		protected virtual void ExportPageFooter(Page page) {
			ExportPageHeaderFooterBase(page.Footer);
		}
		protected virtual void ExportPageHeaderFooter(HeaderFooterPageAreaBase area) {
			area.ExportTo(this);
		}
		protected internal virtual RectangleF BeginHeaderFooterExport(RectangleF clipBounds) {
			return ApplyClipBounds(clipBounds);
		}
		protected internal virtual void EndHeaderFooterExport(RectangleF oldClipBounds) {
			RestoreClipBounds(oldClipBounds);
		}
		internal RectangleF CalculateHeaderFooterClipBounds(RectangleF contentBounds) {
			RectangleF clipBounds = GetClipBounds();
			clipBounds.Height = Math.Max(0, contentBounds.Bottom - clipBounds.Top);
			return clipBounds;
		}
		protected virtual void ExportPageMainAreas(Page page) {
			page.Areas.ExportTo(this);
		}
		void ExportFloatingObjects(IList<FloatingObjectBox> floatingObjects, PieceTable pieceTable) {
			if (floatingObjects == null)
				return;
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++) {
				FloatingObjectBox floatingObject = floatingObjects[i];
				if (floatingObject.PieceTable == pieceTable)
					ExportFloatingObjectBox(floatingObject);
			}
		}
		void ExportParagraphFrames(IList<ParagraphFrameBox> paragraphFrames, PieceTable pieceTable) {
			if (paragraphFrames == null)
				return;
			int count = paragraphFrames.Count;
			for (int i = 0; i < count; i++) {
				ParagraphFrameBox paragraphFrame = paragraphFrames[i];
				if (paragraphFrame.PieceTable == pieceTable)
					ExportParagraphFrameBox(paragraphFrame);
			}
		}
#region IDocumentLayoutExporter implementation
		public virtual void ExportPage(Page page) {
			if (page.Header != null)
				ExportPageAreaCore(page, page.Header.PieceTable, ExportPageHeader);
			if (page.Footer != null)
				ExportPageAreaCore(page, page.Footer.PieceTable, ExportPageFooter);
			ExportPageAreaCore(page, documentModel.MainPieceTable, ExportPageMainAreas);
		}
		public virtual void ExportParagraphFramePage(Page page, RectangleF pageClipBounds, bool exportContent) {
			RectangleF oldClipBounds = GetClipBounds();
			SetClipBounds(pageClipBounds);
			try {
				ExportFloatingObjects(page.BackgroundFloatingObjects, pieceTable);
			}
			finally {
				SetClipBounds(pageClipBounds);
			}
			if (!exportContent)
				return;
			ExportPageMainAreas(page);
			IList<FloatingObjectBox> floatingObjects = page.GetSortedNonBackgroundFloatingObjects();
			ExportFloatingObjects(floatingObjects, documentModel.MainPieceTable);
			IList<ParagraphFrameBox> paragraphFrames = page.GetSortedParagraphFrames();
			ExportParagraphFrames(paragraphFrames, documentModel.MainPieceTable);
		}
		public virtual void ExportPageArea(PageArea pageArea) {
			pageArea.Columns.ExportTo(this);
			pageArea.LineNumbers.ExportTo(this);
			AfterExportPageArea();
		}
		protected internal virtual void AfterExportPageArea() { }
		RectangleF columnClipBounds;
		public virtual void ExportColumn(Column column) {
			ColumnClipBounds = GetClipBounds();
			ExportParagraphFrames(column, paragraphFrameBox => !paragraphFrameBox.IsInCell());
			ExportTablesBackground(column);
			ExportParagraphFrames(column, paragraphFrameBox => paragraphFrameBox.IsInCell());
			ExportRows(column);
			RestoreClipBounds(ColumnClipBounds);
			ExportTables(column);
		}
		protected virtual void ExportTables(Column column) {
			TableViewInfoCollection columnTables = column.InnerTables;
			if (columnTables != null) {
				columnTables.ExportTo(this);
			}
		}
		protected virtual void ExportTablesBackground(Column column) {
			TableViewInfoCollection columnTables = column.InnerTables;
			if (columnTables != null)
				columnTables.ExportBackground(this);
		}
		protected internal virtual void ExportRows(Column column) {
			column.Rows.ExportTo(this);
		}
		protected internal virtual void ExportParagraphFrames(Column column, Func<ParagraphFrameBox, bool> predicate) {
			ParagraphFrameBoxCollection paragraphFrames = column.InnerParagraphFrames;
			if (paragraphFrames == null)
				return;
			paragraphFrames.ForEach(paragraphFrame => { if (predicate(paragraphFrame))ExportParagraphBackgroundFrame(paragraphFrame); });
		}
		protected internal virtual void ExportParagraphBackgroundFrame(ParagraphFrameBox paragraphFrameBox) {
			if (paragraphFrameBox.DocumentLayout == null)
				ExportParagraphFrame(paragraphFrameBox);
		}
		protected internal virtual void ExportParagraphFrame(ParagraphFrameBox paragraphFrameBox) {
			Row row = paragraphFrameBox.FirstRow;
			if (row == null)
				return;
			Row oldRow = this.currentRow;
			SetCurrentRow(row);
			ApplyCurrentRowTableCellClipping(row);
			ExportParagraphFrameBox(paragraphFrameBox);
			SetCurrentRow(oldRow);
		}
		protected internal virtual void SetCurrentRow(Row row) {
			this.currentRow = row;
		}
		protected internal virtual bool ShouldExportRow(Row row) {
			return true;
		}
		public virtual void ExportRow(Row row) {
			SetCurrentRow(row);
			ApplyCurrentRowTableCellClipping(row);
			ExportRowCore();
			SetCurrentRow(null);
		}
		protected internal virtual void BeginExportTableCellContent(RectangleF clipBounds) {
			SetClipBounds(clipBounds);
		}
		protected internal virtual void EndExportTableContent(RectangleF oldClipBounds) {
			RestoreClipBounds(oldClipBounds);
		}
		internal RectangleF BeginExportTableCell(TableCellViewInfo cell) {
			RectangleF oldClipBounds = GetClipBounds();
			ApplyClipBounds(GetDrawingBounds(cell.TableViewInfo.GetCellBounds(cell)));
			this.currentCell = cell;
			currentCellBackColor = cell.Cell.Properties.BackgroundColor;
			return oldClipBounds;
		}
		internal void EndExportTableCell(RectangleF oldClipBounds) {
			RestoreClipBounds(oldClipBounds);
			currentCell = null;
			currentCellBackColor = DXColor.Empty;
		}
		protected internal void ApplyCurrentRowTableCellClipping(Row row) {
			TableCellRow cellRow = row as TableCellRow;
			if (cellRow != null) {
				TableCellViewInfo cell = cellRow.CellViewInfo;
				if (!Object.ReferenceEquals(cell, currentCell)) {
					Rectangle clipBounds = GetDrawingBounds(cell.TableViewInfo.GetCellBounds(cell));
					RestoreClipBounds(ColumnClipBounds);
					BeginExportTableCellContent(clipBounds);
					this.currentCell = cell;
					currentCellBackColor = cell.Cell.Properties.BackgroundColor;
				}
			}
			else {
				if (currentCell != null) {
					EndExportTableContent(ColumnClipBounds);
					currentCell = null;
					currentCellBackColor = DXColor.Empty;
				}
			}
		}
		protected internal virtual void ExportRowCore() {
			ExportRowContentBoxes();
			ExportRowUnderlineBoxes();
			ExportRowStrikeoutBoxes();
			ExportRowBookmarkBoxes();
		}
		protected virtual void ExportRowContentBoxes() {
			if (CurrentRow.NumberingListBox != null)
				CurrentRow.NumberingListBox.ExportTo(this);
			BoxCollection boxes = CurrentRow.Boxes;
			int lastIndex = GetLastExportBoxInRowIndex(CurrentRow);
			for (int i = 0; i <= lastIndex; i++)
				boxes[i].ExportTo(this);
		}
		protected virtual void ExportRowUnderlineBoxes() {
			UnderlineBoxCollection underlines = CurrentRow.InnerUnderlines;
			if (underlines == null)
				return;
			int count = underlines.Count;
			for (int i = 0; i < count; i++)
				ExportUnderlineBox(CurrentRow, underlines[i]);
		}
		protected virtual void ExportRowStrikeoutBoxes() {
			UnderlineBoxCollection strikeouts = CurrentRow.InnerStrikeouts;
			if (strikeouts == null)
				return;
			int count = strikeouts.Count;
			for (int i = 0; i < count; i++)
				ExportStrikeoutBox(CurrentRow, strikeouts[i]);
		}
		protected virtual void ExportRowErrorBoxes() {
			ExportRowErrorBoxes(CurrentRow);
		}
		protected internal virtual void ExportRowErrorBoxes(Row row) {
			ErrorBoxCollection errors = row.Errors;
			if (errors == null)
				return;
			int count = errors.Count;
			for (int i = 0; i < count; i++)
				ExportErrorBox(errors[i]);
		}
		protected virtual void ExportRowBookmarkBoxes() { }
		public virtual void ExportTextBox(TextBox box) {
		}
		public virtual void ExportSpecialTextBox(SpecialTextBox box) {
			ExportTextBox(box);
		}
		public virtual void ExportLayoutDependentTextBox(LayoutDependentTextBox box) {
		}
		public virtual void ExportHyphenBox(HyphenBox box) {
		}
		public virtual void ExportInlinePictureBox(InlinePictureBox box) {
		}
		public virtual void ExportCustomRunBox(CustomRunBox box) {
			ICustomRunBoxLayoutExporterService service = documentModel.GetService<ICustomRunBoxLayoutExporterService>();
			if (service != null)
				service.ExportCustomRunBox(PieceTable, Painter, box);
		}
		internal virtual bool BeginExportCompositeObject(Rectangle bounds, PieceTable pieceTable, ref RectangleF oldClipRect, ref RectangleF clipRect, ref PieceTable oldPieceTable) {
			return true;
		}
		internal virtual void EndExportCompositeObject(RectangleF oldClipRect, PieceTable oldPieceTable) {
		}
		public virtual void ExportFloatingObjectBox(FloatingObjectBox box) {
			if (ExportRotatedContent(box))
				return;
			ExportNotRotatedContent(box);
		}
		public virtual void ExportParagraphFrameBox(ParagraphFrameBox box) {
			if (ExportRotatedContent(box))
				return;
			ExportNotRotatedContent(box);
		}
		protected internal virtual bool BeforeExportRotatedContent(FloatingObjectBox box) {
			return true;
		}
		protected internal virtual void AfterExportRotatedContent(bool transformApplied) {
		}
		protected internal virtual bool ExportRotatedContent(FloatingObjectBox box) {
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			PieceTable previousPieceTable = PieceTable;
			this.PieceTable = run.PieceTable;
			try {
				ExportFloatingObjectShape(box, run.Shape);
				PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
				if (pictureContent != null) {
					ExportFloatingObjectPicture(box, pictureContent);
					return true;
				}
			}
			finally {
				this.PieceTable = previousPieceTable;
			}
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (box.DocumentLayout != null && textBoxContent != null)
				if (!textBoxContent.TextBoxProperties.Upright) {
					ExportFloatingObjectTextBox(box, textBoxContent, box.DocumentLayout);
					return true;
				}
			return false;
		}
		protected virtual void ExportNotRotatedContent(FloatingObjectBox box) {
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (box.DocumentLayout != null && textBoxContent != null) {
				if (textBoxContent.TextBoxProperties.Upright) {
					ExportFloatingObjectTextBox(box, textBoxContent, box.DocumentLayout);
				}
			}
		}
		protected internal virtual bool ExportRotatedContent(ParagraphFrameBox box) {
			PieceTable previousPieceTable = PieceTable;
			this.PieceTable = box.PieceTable;
			bool result;
			try {
				ExportParagraphFrameShape(box, null);
				result = ExportNotRotatedContent(box);
			}
			finally {
				this.PieceTable = previousPieceTable;
			}
			return result;
		}
		protected virtual bool ExportNotRotatedContent(ParagraphFrameBox box) {
			if (box.DocumentLayout != null) {
				ExportParagraphFrameTextBox(box, box.DocumentLayout);
				return true;
			}
			return false;
		}
		public virtual void ExportFloatingObjectShape(FloatingObjectBox box, Shape shape) {
		}
		public virtual void ExportFloatingObjectPicture(FloatingObjectBox box, PictureFloatingObjectContent pictureContent) {
		}
		public virtual void ExportFloatingObjectTextBox(FloatingObjectBox box, TextBoxFloatingObjectContent textBoxContent, DocumentLayout textBoxDocumentLayout) {
		}
		public virtual void ExportParagraphFrameShape(ParagraphFrameBox box, Shape shape) {
		}
		public virtual void ExportParagraphFramePicture(ParagraphFrameBox box, PictureFloatingObjectContent pictureContent) {
		}
		public virtual void ExportParagraphFrameTextBox(ParagraphFrameBox box, DocumentLayout textBoxDocumentLayout) {
		}
		protected internal virtual bool ShouldExportComments(DevExpress.XtraRichEdit.Layout.Page page) {
			return true;
		}
		protected internal virtual void BeforeExportComments(DevExpress.XtraRichEdit.Layout.Page page) {
		}
		protected internal virtual void BeforeExportComment(CommentViewInfo commentViewInfo) {
		}
		protected internal virtual bool ShouldExportComment(CommentViewInfo commentViewInfo) {
			return true;
		}
		protected internal virtual void AfterExportComment(CommentViewInfo commentViewInfo, Rectangle pageCommentBounds) {
		}
		public virtual void ExportComment(CommentViewInfo commentViewInfo) {
		}
		public virtual void ExportSpaceBox(Box box) {
		}
		public virtual void ExportTabSpaceBox(TabSpaceBox box) {
		}
		public virtual void ExportLineBreakBox(LineBreakBox box) {
		}
		public virtual void ExportParagraphMarkBox(ParagraphMarkBox box) {
		}
		public virtual void ExportSectionMarkBox(SectionMarkBox box) {
		}
		public virtual void ExportUnderlineBox(Row row, UnderlineBox underlineBox) {
		}
		public virtual void ExportStrikeoutBox(Row row, UnderlineBox strikeoutBox) {
		}
		public virtual void ExportErrorBox(ErrorBox errorBox) {
		}
		public virtual void ExportBookmarkStartBox(VisitableDocumentIntervalBox box) {
		}
		public virtual void ExportBookmarkEndBox(VisitableDocumentIntervalBox box) {
		}
		public virtual void ExportCommentStartBox(VisitableDocumentIntervalBox box) {
		}
		public virtual void ExportCommentEndBox(VisitableDocumentIntervalBox box) {
		}
		public virtual void ExportCustomMarkBox(CustomMarkBox box) {
		}
		public virtual void ExportSeparatorBox(SeparatorBox box) {
		}
		public virtual void ExportPageBreakBox(PageBreakBox box) {
		}
		public virtual void ExportColumnBreakBox(ColumnBreakBox box) {
		}
		public virtual void ExportNumberingListBox(NumberingListBox box) {
		}
		public virtual void ExportLineNumberBox(LineNumberBox box) {
		}
		public virtual void ExportTableBorder(TableBorderViewInfoBase border, Rectangle cellBounds) {
		}
		public virtual void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y) {
		}
		public virtual void ExportTableCell(TableCellViewInfo cell) {
		}
		public virtual void ExportTableRow(TableRowViewInfoBase row) {
		}
		public virtual void ExportDataContainerRunBox(DataContainerRunBox box) {
		}
		public virtual void ExportHighlightArea(HighlightArea area) {
		}
		public virtual bool IsAnchorVisible(ITableCellVerticalAnchor anchor) {
			return true;
		}
		public virtual bool IsTableRowVisible(TableRowViewInfoBase row) {
			return true;
		}
#endregion
		protected internal virtual RectangleF ApplyClipBounds(RectangleF clipBounds) {
			RectangleF oldClipBounds = GetClipBounds();
			RectangleF actualClipBounds = GetClipBounds();
			if (float.IsNegativeInfinity(actualClipBounds.X) && float.IsNegativeInfinity(actualClipBounds.Y) && float.IsPositiveInfinity(actualClipBounds.Width) && float.IsPositiveInfinity(actualClipBounds.Height))
				actualClipBounds = clipBounds;
			else
				actualClipBounds.Intersect(clipBounds);
			ApplyClipBoundsCore(actualClipBounds);
			return oldClipBounds;
		}
		protected internal virtual void RestoreClipBounds(RectangleF clipBounds) {
			RestoreClipBoundsCore(clipBounds);
		}
		protected virtual void ApplyClipBoundsCore(RectangleF bounds) {
			SetClipBounds(bounds);
		}
		protected virtual void RestoreClipBoundsCore(RectangleF bounds) {
			SetClipBounds(bounds);
		}
		protected internal virtual RectangleF GetClipBounds() {
			return RectangleF.Empty;
		}
		protected internal virtual void SetClipBounds(RectangleF clipBounds) {
		}
		protected internal virtual RectangleF IntersectClipBounds(RectangleF oldClipBounds, RectangleF bounds) {
			if (float.IsInfinity(oldClipBounds.Left) && float.IsInfinity(oldClipBounds.Top) && float.IsInfinity(oldClipBounds.Width) && float.IsInfinity(oldClipBounds.Height))
				return bounds;
			else
				return RectangleF.Intersect(oldClipBounds, bounds);
		}
#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
#endregion
		protected internal virtual void FinishExport() {
		}
		protected internal virtual bool IsValidBounds(Rectangle boxBounds) {
#if DEBUGTEST || DEBUG
			Debug.Assert(!boxBounds.IsEmpty);
#endif
			return true;
		}
		protected internal virtual Rectangle GetDrawingBounds(Rectangle bounds) {
			return bounds;
		}
		HyperlinkCalculator GetHyperlinkCalculator(PieceTable pieceTable) {
			HyperlinkCalculator result;
			if (hyperlinkCalculators.TryGetValue(pieceTable, out result))
				return result;
			result = new HyperlinkCalculator(pieceTable);
			hyperlinkCalculators.Add(pieceTable, result);
			return result;
		}
		protected string GetUrl(Box box) {
			return GetHyperlinkCalculator(PieceTable).GetUrl(box.GetFirstPosition(PieceTable).RunIndex);
		}
		protected string GetAnchor(Box box) {
			return GetHyperlinkCalculator(PieceTable).GetAnchor(box.GetFirstPosition(PieceTable).RunIndex);
		}
	}
#endregion
}
