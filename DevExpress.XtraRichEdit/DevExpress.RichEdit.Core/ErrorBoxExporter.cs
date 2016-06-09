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
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
#if !DXPORTABLE
using DevExpress.XtraRichEdit.Mouse;
#endif
#if !SL
using System.Drawing.Drawing2D;
using DevExpress.XtraRichEdit.Services;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region NotPrintableGraphicsBoxExporter (abstract class)
	public abstract class NotPrintableGraphicsBoxExporter : DocumentLayoutExporter {
		#region Fields
		readonly RichEditView view;
		readonly Painter painter;
		readonly ErrorBoxExporter errorExporter;
		readonly BookmarkBoxExporter bookmarkExporter;
		readonly CommentBoxExporter commentExporter;
		readonly ICustomMarkExporter customMarkExporter;
		readonly TableNotPrintableBorderExporter tableBorderExporter;
		TableViewInfo tableViewInfo;
		#endregion
		protected NotPrintableGraphicsBoxExporter(DocumentModel documentModel, Painter painter, RichEditView view, ICustomMarkExporter customMarkExporter)
			: base(documentModel) {
			Guard.ArgumentNotNull(painter, "painter");
			this.painter = painter;
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.errorExporter = CreateErrorBoxExporter();
			this.bookmarkExporter = CreateBookmarkBoxExporter();
			this.commentExporter = CreateCommentExporter();
			this.customMarkExporter = customMarkExporter;
			this.tableBorderExporter = CreateTableBorderExporter();
		}
		#region Properties
		public override Painter Painter { get { return painter; } }
		protected internal DocumentLayoutUnitConverter UnitConverter { get { return DocumentModel.LayoutUnitConverter; } }
		#endregion
		protected virtual ErrorBoxExporter CreateErrorBoxExporter() {
			return new ErrorBoxExporter(this);
		}
		protected virtual BookmarkBoxExporter CreateBookmarkBoxExporter() {
			return new BookmarkBoxExporter(this);
		}
		protected virtual CommentBoxExporter CreateCommentExporter() {
			return new CommentBoxExporter(this);
		}
		protected internal TableNotPrintableBorderExporter CreateTableBorderExporter() {
			return new TableNotPrintableBorderExporter(this);
		}
		protected internal abstract Rectangle GetActualBounds(Rectangle bounds);
		protected internal abstract float PixelsToDrawingUnits(float value);
		#region IDocumentLayoutExporter Members
		public override void ExportPageArea(PageArea pageArea) {
			pageArea.Columns.ExportTo(this);
		}
		protected override void ExportTablesBackground(Column column) {
		}
		protected internal override void ExportParagraphFrames(Column column, Func<ParagraphFrameBox, bool> predicate) {
		}
		internal override void BeforeExportTable(TableViewInfo table) {
			tableViewInfo = table;
		}
		protected override void ExportTables(Column column) {
			TableViewInfoCollection tables = column.InnerTables;
			if (tables != null) {
				int count = tables.Count;
				for (int i = 0; i < count; i++) {
					BeforeExportTable(tables[i]);
					tables[i].ExportTo(this);
				}
			}
		}
		protected internal override void ExportRowCore() {
			ExportErrorBoxes();
			ExportBookmarkBoxes();
			ExportRangePermissionBoxes();
			if ((view.DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible) || view.DocumentModel.CommentOptions.HighlightCommentedRange)
				ExportCommentBoxes();
			ExportImeBoxes();
			ExportHiddenTextBoxes();
			ExportSpecialTextBoxes();
			ExportCustomMarkBoxes();
		}
		protected internal override void ExportErrorBoxes() {
			ErrorBoxCollection errors = CurrentRow.InnerErrors;
			if (errors != null)
				errorExporter.ExportTo(errors);
		}
		protected internal virtual void ExportBookmarkBoxes() {
			BookmarkBoxCollection bookmarkBoxes = CurrentRow.InnerBookmarkBoxes;
			if (bookmarkBoxes != null)
				ExportBookmarkBoxesCore(bookmarkBoxes);
		}
		protected internal virtual void ExportCustomMarkBoxes() {
			CustomMarkBoxCollection customMarkBoxes = CurrentRow.InnerCustomMarkBoxes;
			if (customMarkBoxes != null)
				ExportCustomMarkBoxesCore(customMarkBoxes);
		}
		protected internal virtual void ExportRangePermissionBoxes() {
			BookmarkBoxCollection rangePermissionBoxes = CurrentRow.InnerRangePermissionBoxes;
			if (rangePermissionBoxes != null)
				ExportBookmarkBoxesCore(rangePermissionBoxes);
		}
		protected internal virtual void ExportCommentBoxes() {
			BookmarkBoxCollection commentBoxes = CurrentRow.InnerCommentBoxes;
			if (commentBoxes != null)
				ExportBookmarkBoxesCore(commentBoxes);
		}
		protected internal virtual void ExportBookmarkBoxesCore(BookmarkBoxCollection boxes) {
			int count = boxes.Count;
			for (int i = 0; i < count; i++)
				boxes[i].ExportTo(this);
		}
		protected internal virtual void ExportCustomMarkBoxesCore(CustomMarkBoxCollection boxes) {
			int count = boxes.Count;
			for (int i = 0; i < count; i++)
				boxes[i].ExportTo(this);
		}
		float lineWidth;
		float dotStep;
		protected internal virtual void ExportHiddenTextBoxes() {
			HiddenTextUnderlineBoxCollection boxes = CurrentRow.InnerHiddenTextBoxes;
			if (boxes == null || boxes.Count <= 0)
				return;
			Pen pen = BeginExportNotPrintableBoxes();
			try {
				int count = boxes.Count;
				for (int i = 0; i < count; i++)
					ExportHiddenTextBox(boxes[i], pen);
			}
			finally {
				EndExportNotPrintableBoxes(pen);
			}
		}
		internal override Pen BeginExportNotPrintableBoxes() {
			this.lineWidth = PixelsToDrawingUnits(0.5f);
			this.dotStep = PixelsToDrawingUnits(3f);
			return Painter.GetPen(DXColor.Black, 1);
		}
		internal override void EndExportNotPrintableBoxes(Pen pen) {
			Painter.ReleasePen(pen);
		}
		protected internal virtual void ExportSpecialTextBoxes() {
			if (!DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText)
				return;
			SpecialTextBoxCollection boxes = CurrentRow.InnerSpecialTextBoxes;
			if (boxes == null || boxes.Count <= 0)
				return;
			Pen pen = BeginExportNotPrintableBoxes();
			try {
				int count = boxes.Count;
				for (int i = 0; i < count; i++)
					ExportSpecialTextBox(boxes[i], pen);
			}
			finally {
				EndExportNotPrintableBoxes(pen);
			}
		}
		internal override void ExportHiddenTextBox(HiddenTextUnderlineBox box, Pen pen) {
			int bottom = CurrentRow.Bounds.Top + CurrentRow.BaseLineOffset + box.BottomOffset;
			Rectangle bounds = Rectangle.FromLTRB(box.Start, bottom, box.End, bottom);
			Rectangle drawBounds = GetActualBounds(bounds);
			DrawHiddenTextBox(drawBounds.Left, drawBounds.Right, drawBounds.Bottom, pen);
		}
		void DrawHiddenTextBox(int left, int right, int bottom, Pen pen) {
			DrawHorizontalDotLine(left, right, bottom, pen);
		}
		void DrawHorizontalDotLine(int left, int right, int y, Pen pen) {
			for (float i = left; i < right; i += dotStep)
				Painter.DrawLine(pen, i, y, i + lineWidth, y);
		}
		void DrawVerticalDotLine(int top, int bottom, int x, Pen pen) {
			for (float i = top; i < bottom; i += dotStep)
				Painter.DrawLine(pen, x, i, x, i + lineWidth);
		}
		protected virtual Rectangle GetActualCustomMarkBounds(Rectangle bounds) {
			return GetActualBounds(bounds);
		}
		protected internal virtual void ExportCustomMarkBoxCore(CustomMarkBox box) {
			Rectangle drawBounds = GetActualCustomMarkBounds(box.Bounds);
			customMarkExporter.ExportCustomMarkBox(box.CustomMark, drawBounds);
		}
		protected internal override void ExportSpecialTextBox(SpecialTextBox box, Pen pen) {
			int verticalOffset = box.GetFontInfo(PieceTable).Free;
			Rectangle bounds = GetActualBounds(box.Bounds);
			DrawHorizontalDotLine(bounds.Left, bounds.Right, bounds.Top + verticalOffset, pen);
			DrawVerticalDotLine(bounds.Top + verticalOffset, bounds.Bottom + verticalOffset, bounds.Right, pen);
			DrawHorizontalDotLine(bounds.Left, bounds.Right, bounds.Bottom + verticalOffset, pen);
			DrawVerticalDotLine(bounds.Top + verticalOffset, bounds.Bottom + verticalOffset, bounds.Left, pen);
		}
		public override void ExportBookmarkStartBox(VisitableDocumentIntervalBox bookmarkBox) {
			bookmarkExporter.ExportBookmarkStartBox(bookmarkBox);
		}
		public override void ExportBookmarkEndBox(VisitableDocumentIntervalBox bookmarkBox) {
			bookmarkExporter.ExportBookmarkEndBox(bookmarkBox);
		}
		public override void ExportCommentStartBox(VisitableDocumentIntervalBox commentbox) {
			commentExporter.ExportCommentStartBox(commentbox);
		}
		public override void ExportCommentEndBox(VisitableDocumentIntervalBox commentbox) {
			commentExporter.ExportCommentEndBox(commentbox);
		}
		public override void ExportCustomMarkBox(CustomMarkBox box) {
			ExportCustomMarkBoxCore(box);
		}
		protected internal override bool ExportRotatedContent(FloatingObjectBox box) {
			Rectangle bounds = GetDrawingBounds(box.Bounds);
			Point center = DevExpress.Office.Utils.RectangleUtils.CenterPoint(bounds);
			bool transformApplied = Painter.TryPushRotationTransform(center, DocumentModel.GetBoxEffectiveRotationAngleInDegrees(box));
			Painter.PushSmoothingMode(transformApplied);
			try {
				return base.ExportRotatedContent(box);
			}
			finally {
				Painter.PopSmoothingMode();
				if (transformApplied)
					Painter.PopTransform();
			}
		}
		public override void ExportFloatingObjectTextBox(FloatingObjectBox box, TextBoxFloatingObjectContent textBoxContent, DocumentLayout textBoxDocumentLayout) {
			Rectangle bounds = GetDrawingBounds(box.ContentBounds);
			RectangleF oldClipRect = GetClipBounds();
			RectangleF clipRect = IntersectClipBounds(oldClipRect, bounds);
			if (clipRect == RectangleF.Empty)
				return;
			SetClipBounds(clipRect);
			textBoxDocumentLayout.Pages.First.ExportTo(this);
			SetClipBounds(oldClipRect);
		}
		public override void ExportTableBorder(TableBorderViewInfoBase border, Rectangle cellBounds) {
			bool shouldDrawGridLinesForAllTable = DocumentModel.TableOptions.GridLines == RichEditTableGridLinesVisibility.Visible && IsTableBorderInvisible(border);
			bool shouldDrawGridLinesForCurrentTable = DocumentModel.TableOptions.GridLines == RichEditTableGridLinesVisibility.VisibleWhileDragging && ShouldDrawGridLinesForCurrentTableViewInfo;
			if (shouldDrawGridLinesForAllTable || shouldDrawGridLinesForCurrentTable)
				tableBorderExporter.DrawBorder(border, cellBounds);
		}
		bool ShouldDrawGridLinesForCurrentTableViewInfo {
			get {
#if !DXPORTABLE
				RichEditMouseHandler richEditMouseHandler = view.Control.InnerControl.MouseHandler as RichEditMouseHandler;
				return (richEditMouseHandler != null && richEditMouseHandler.TableViewInfo != null && richEditMouseHandler.TableViewInfo == tableViewInfo);
#else
				return false;
#endif
			}
		}
		public override void ExportCustomRunBox(CustomRunBox box) {
		}
		bool IsTableBorderInvisible(TableBorderViewInfoBase border) {
			BorderLineStyle borderLineStyle = border.Border.Style;
			return (borderLineStyle == BorderLineStyle.None || borderLineStyle == BorderLineStyle.Nil || borderLineStyle == BorderLineStyle.Disabled);
		}
		protected internal override void SetClipBounds(RectangleF clipBounds) {
			float x = UnitConverter.LayoutUnitsToPixelsF(clipBounds.X);
			float y = UnitConverter.LayoutUnitsToPixelsF(clipBounds.Y);
			float width = UnitConverter.LayoutUnitsToPixelsF(clipBounds.Width);
			float height = UnitConverter.LayoutUnitsToPixelsF(clipBounds.Height);
			float zoomFactor = view.ZoomFactor;
			Painter.ClipBounds = new RectangleF(x * zoomFactor, y * zoomFactor, width * zoomFactor, height * zoomFactor);
		}
		protected internal override RectangleF GetClipBounds() {
			float zoomFactor = view.ZoomFactor;
			RectangleF clipBounds = Painter.ClipBounds;
			float x = UnitConverter.PixelsToLayoutUnitsF(clipBounds.X);
			float y = UnitConverter.PixelsToLayoutUnitsF(clipBounds.Y);
			float width = UnitConverter.PixelsToLayoutUnitsF(clipBounds.Width);
			float height = UnitConverter.PixelsToLayoutUnitsF(clipBounds.Height);
			return new RectangleF(x / zoomFactor, y / zoomFactor, width / zoomFactor, height / zoomFactor);
		}
		#endregion
	}
	#endregion
	#region CustomMarkExporter
	public class CustomMarkExporter : ICustomMarkExporter {
		CustomMarkVisualInfoCollection visualInfoCollection;
		public CustomMarkExporter() {
			this.visualInfoCollection = new CustomMarkVisualInfoCollection();
		}
		public CustomMarkVisualInfoCollection CustomMarkVisualInfoCollection { get { return visualInfoCollection; } }
		#region ICustomMarkExporter Members
		public virtual void ExportCustomMarkBox(CustomMark customMark, Rectangle bounds) {
			visualInfoCollection.Add(new CustomMarkVisualInfo(customMark, bounds));
		}
		#endregion
	}
	#endregion
	public class CustomMarkVisualInfo {
		Rectangle bounds;
		readonly CustomMark customMark;
		public CustomMarkVisualInfo(CustomMark customMark, Rectangle bounds) {
			Guard.ArgumentNotNull(customMark, "customMark");
			this.bounds = bounds;
			this.customMark = customMark;
		}
		protected internal CustomMark CustomMark { get { return customMark; } }
		public object UserData { get { return CustomMark.UserData; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
	}
	public class CustomMarkVisualInfoCollection : List<CustomMarkVisualInfo> {
	}
	#region BookmarkBoxExporter
	public class BookmarkBoxExporter {
		const int BookmarkBoxWidth = 4;
		const int PenWidth = 2;
		NotPrintableGraphicsBoxExporter exporter;
		public BookmarkBoxExporter(NotPrintableGraphicsBoxExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
		}
		protected internal NotPrintableGraphicsBoxExporter Exporter { get { return exporter; } }
		public virtual void ExportBookmarkStartBox(VisitableDocumentIntervalBox bookmarkBox) {
			ExportBookmarkBoxCore(bookmarkBox, GetBookmarkStartPoints);
		}
		public virtual void ExportBookmarkEndBox(VisitableDocumentIntervalBox bookmarkBox) {
			ExportBookmarkBoxCore(bookmarkBox, GetBookmarkEndPoints);
		}
		delegate PointF[] GetPoints(float x, float y, float height);
		void ExportBookmarkBoxCore(VisitableDocumentIntervalBox bookmarkBox, GetPoints getPoints) {
			Bookmark bookmark = bookmarkBox.Interval as Bookmark;
			if (bookmark != null && bookmark.Name.Contains("Comment"))
				return;
			int horizontalPosition = bookmarkBox.HorizontalPosition;
			Row row = exporter.CurrentRow;
			int bottom = row.Boxes.First.Bounds.Bottom;
			int top = row.Bounds.Top + row.SpacingBefore;
			Rectangle bounds = Rectangle.FromLTRB(horizontalPosition, top, horizontalPosition, bottom);
			Rectangle drawingBounds = exporter.GetActualBounds(bounds);
			DrawBookmarkBox(bookmarkBox.Color, getPoints(drawingBounds.X, drawingBounds.Y, drawingBounds.Height));
		}
		protected virtual void DrawBookmarkBox(Color color, PointF[] points) {
			Painter painter = exporter.Painter;
			Pen pen = painter.GetPen(color, PenWidth);
			try {
				painter.DrawLines(pen, points);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
		PointF[] GetBookmarkStartPoints(float x, float y, float height) {
			return GetBookmarkPoints(x, y, height, exporter.PixelsToDrawingUnits(BookmarkBoxWidth));
		}
		PointF[] GetBookmarkEndPoints(float x, float y, float height) {
			return GetBookmarkPoints(x, y, height, exporter.PixelsToDrawingUnits(-BookmarkBoxWidth));
		}
		protected virtual PointF[] GetBookmarkPoints(float x, float y, float height, float width) {
			float offset = exporter.PixelsToDrawingUnits(PenWidth) / 2;
			float left = (float)(x - offset);
			float right = (float)(x - offset + width);
			float top = (float)(y + offset);
			float bottom = (float)(y + height - offset);
			return new PointF[] { new PointF(right, top), new PointF(left, top), new PointF(left, bottom), new PointF(right, bottom) };
		}
	}
	#endregion
	#region CommentBoxExporter
	public class CommentBoxExporter : BookmarkBoxExporter {
		const int verticalOffset = 5;
		const int penWidth = 1;
		public CommentBoxExporter(NotPrintableGraphicsBoxExporter exporter) : base(exporter) { }
		public virtual void ExportCommentStartBox(VisitableDocumentIntervalBox bookmarkBox) {
			base.ExportBookmarkStartBox(bookmarkBox);
		}
		public virtual void ExportCommentEndBox(VisitableDocumentIntervalBox bookmarkBox) {
			base.ExportBookmarkEndBox(bookmarkBox);
		}
		protected override PointF[] GetBookmarkPoints(float x, float y, float height, float width) {
			float offset = Exporter.PixelsToDrawingUnits(penWidth) / 2;
			float left = (float)(x - offset);
			float right = (float)(x - offset + width);
			float top1 = (float)(y + offset);
			float top2 = (float)(y + offset + verticalOffset);
			float bottom1 = (float)(y + height - offset - verticalOffset);
			float bottom2 = (float)(y + height - offset);
			return new PointF[] { new PointF(right, top1), new PointF(left, top2), new PointF(left, bottom1), new PointF(right, bottom2) };
		}
		protected override void DrawBookmarkBox(Color color, PointF[] points) {
			Painter painter = Exporter.Painter;
			Pen pen = painter.GetPen(color, penWidth);
			try {
				painter.DrawLines(pen, points);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
	}
	#endregion
	#region TableNotPrintableBorderExporter
	public class TableNotPrintableBorderExporter {
		#region Fields
		const int PenWidth = 1;
		readonly NotPrintableGraphicsBoxExporter exporter;
		#endregion
		public TableNotPrintableBorderExporter(NotPrintableGraphicsBoxExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
		}
		protected internal virtual void DrawBorder(TableBorderViewInfoBase border, Rectangle cellBounds) {
			Painter painter = exporter.Painter;
			Pen pen = painter.GetPen(Color.FromArgb(255, 30, 144, 255), exporter.PixelsToDrawingUnits(PenWidth));
			pen.DashPattern = CreateDashPattern();
			try {
				Rectangle drawingBounds = exporter.GetActualBounds(cellBounds);
				PointF[] points = GetPoints(border, drawingBounds.X, drawingBounds.Y, drawingBounds.Height, drawingBounds.Width);
				painter.DrawLines(pen, points);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
		protected internal float[] CreateDashPattern() {
			return new float[] { exporter.PixelsToDrawingUnits(3), exporter.PixelsToDrawingUnits(1) };
		}
		PointF[] GetPoints(TableBorderViewInfoBase border, float x, float y, float height, float width) {
			if ((border.BorderType & BorderTypes.Left) != 0)
				return new PointF[] { new PointF(x, y), new PointF(x, y + height) };
			if ((border.BorderType & BorderTypes.Right) != 0)
				return new PointF[] { new PointF(x + width, y), new PointF(x + width, y + height) };
			if ((border.BorderType & BorderTypes.Top) != 0)
				return new PointF[] { new PointF(x, y), new PointF(x + width, y) };
			if ((border.BorderType & BorderTypes.Bottom) != 0)
				return new PointF[] { new PointF(x, y + height), new PointF(x + width, y + height) };
			return null;
		}
	}
	#endregion
	#region UnderlineBoxExporter<T> (abstract class)
	public abstract class UnderlineBoxExporter<T> where T : UnderlineBox {
		#region Fields
		readonly NotPrintableGraphicsBoxExporter exporter;
		readonly RichEditPatternLinePainter linePainter;
		#endregion
		protected UnderlineBoxExporter(NotPrintableGraphicsBoxExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
			this.linePainter = CreateCharacterLinePainter();
		}
		#region Properties
		protected internal virtual int UnderlineHeight { get { return 2; } }
		protected internal virtual int UnderlineStep { get { return UnderlineHeight; } }
		protected internal NotPrintableGraphicsBoxExporter Exporter { get { return exporter; } }
		protected internal RichEditPatternLinePainter LinePainter { get { return linePainter; } }
		protected internal abstract Color UnderlineColor { get; }
		#endregion
		protected virtual RichEditPatternLinePainter CreateCharacterLinePainter() {
			return new RichEditHorizontalPatternLinePainter(Exporter.Painter, Exporter.UnitConverter);
		}
		public virtual void ExportTo(UnderlineBoxCollectionBase<T> boxes) {
			int count = boxes.Count;
			if (count <= 0)
				return;
			Painter painter = exporter.Painter;
			Pen pen = painter.GetPen(UnderlineColor);
			try {
				for (int i = 0; i < count; i++)
					ExportBox(boxes[i], pen);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
		public virtual void ExportBox(T errorBox, Pen pen) {
			Rectangle actualBounds = Exporter.GetActualBounds(errorBox.ClipBounds);
			DrawUnderline(actualBounds, pen);
		}
		protected virtual void DrawUnderline(Rectangle bounds, Pen pen) {
			RectangleF lineBounds = CalcUnderlineBounds(bounds);
			DrawUnderlineByLines(lineBounds, pen);
		}
		protected virtual RectangleF CalcUnderlineBounds(Rectangle bounds) {
			RectangleF result = new RectangleF();
			result.X = bounds.X;
			float height = Exporter.PixelsToDrawingUnits(UnderlineHeight);
			result.Y = bounds.Y + (bounds.Height - height) / 2;
			result.Width = bounds.Width;
			result.Height = height;
			return result;
		}
		protected virtual void DrawUnderlineByLines(RectangleF bounds, Pen pen) {
			LinePainter.DrawWaveUnderline(bounds, pen, bounds.Height);
		}
	}
	#endregion
	#region ErrorBoxExporter
	public class ErrorBoxExporter : UnderlineBoxExporter<ErrorBox> {
		public ErrorBoxExporter(NotPrintableGraphicsBoxExporter exporter)
			: base(exporter) {
		}
		protected internal override Color UnderlineColor { get { return DXColor.Red; } }
	}
	#endregion
	#region Old exporters
	#endregion
}
