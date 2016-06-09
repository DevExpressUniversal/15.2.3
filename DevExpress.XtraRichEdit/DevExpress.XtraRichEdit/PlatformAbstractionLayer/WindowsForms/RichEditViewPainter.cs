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
using System.Drawing.Drawing2D;
using DevExpress.LookAndFeel;
using DevExpress.Office.Drawing;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Office.Layout;
using DevExpress.Skins;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Painters {
	public delegate void DrawAtPageDelegate(GraphicsCache cache);
	#region RichEditViewPainter (abstract class)
	public abstract class RichEditViewPainter : IDisposable {
		#region Fields
		readonly RichEditView view;
		readonly RichEditControl control;
		readonly RichEditSelectionPainters selectionPainters;
		readonly RichEditHoverPainters hoverPainters;
		readonly List<IDecoratorPainter> decoratorPainters;
		GraphicsCache cache;
		Graphics graphics;
		Matrix originalTransform;
		int physicalLeftInvisibleWidth;
		int minReadableTextHeight;
		#endregion
		protected RichEditViewPainter(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.control = (RichEditControl)view.Control;
			this.decoratorPainters = new List<IDecoratorPainter>();
			this.selectionPainters = new RichEditSelectionPainters();
			this.hoverPainters = new RichEditHoverPainters();
		}
		#region Properties
		public RichEditView View { get { return view; } }
		public virtual UserLookAndFeel LookAndFeel { get { return Control.LookAndFeel; } }
		protected List<IDecoratorPainter> DecoratorPainters { get { return decoratorPainters; } }
		protected RichEditSelectionPainters SelectionPainters { get { return selectionPainters; } }
		protected RichEditHoverPainters HoverPainters { get { return hoverPainters; } }
		protected internal RichEditControl Control { get { return control; } }
		protected internal DocumentModel DocumentModel { get { return control.DocumentModel; } }
		protected GraphicsCache Cache { get { return cache; } }
		protected Graphics Graphics { get { return graphics; } }
		protected internal DocumentLayoutUnitConverter UnitConverter { get { return View.DocumentModel.LayoutUnitConverter; } }
		protected bool HideCommentLine { get { return SkinPaintHelper.GetRichEditSkinBoolProperty(LookAndFeel, SkinPaintHelper.HideCommentLine, false); } }
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				int count = DecoratorPainters.Count;
				for (int i = 0; i < count; i++) {
					IDisposable disposable = DecoratorPainters[i] as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void AddDecoratorPainter(IDecoratorPainter painter) {
			this.DecoratorPainters.Add(painter);
		}
		public void AddHoverPainter<THoverLayoutItem, THoverPainter, TBox>()
			where THoverLayoutItem : IHoverLayoutItem<TBox>
			where THoverPainter : IHoverPainter<TBox>
			where TBox : Box {
			HoverPainters.Add<THoverLayoutItem, THoverPainter>();
		}
		protected internal virtual void BeginDraw(GraphicsCache cache) {
			CacheInitialize(cache);
			this.minReadableTextHeight = (int)Math.Round(Control.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(6, graphics.DpiY) / View.ZoomFactor);
			this.physicalLeftInvisibleWidth = View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			AddSelectionPainters(cache);
			this.originalTransform = graphics.Transform.Clone();
			graphics.TranslateTransform(-physicalLeftInvisibleWidth, 0);
		}
		protected virtual void AddSelectionPainters(GraphicsCache cache) {
			SelectionPainters.AddDefault(new SemitransparentSelectionPainter(cache));
			SelectionPainters.Add<RectangularObjectSelectionLayout>(new RectangularObjectSelectionPainter(cache));
		}
		protected internal void CacheInitialize(GraphicsCache cache) {
			this.cache = cache;
			this.graphics = cache.Graphics;
		}
		protected internal virtual void EndDraw() {
			graphics.Transform = originalTransform.Clone();
			this.originalTransform = null;
			CacheDispose();
		}
		protected internal void CacheDispose() {
			this.graphics = null;
			this.cache = null;
		}
		#region Begin/End DrawPagesContent
		Matrix originalDrawPagesTransform;
		HdcZoomModifier zoomModifier;
		protected internal virtual Matrix BeginDrawPagesContent() {
			this.originalDrawPagesTransform = graphics.Transform.Clone();
			float zoomFactor = View.ZoomFactor;
			graphics.ScaleTransform(zoomFactor, zoomFactor);
			this.zoomModifier = new HdcZoomModifier(graphics, View.ZoomFactor);
			return graphics.Transform.Clone();
		}
		protected internal virtual void EndDrawPagesContent() {
			this.zoomModifier.Dispose();
			this.zoomModifier = null;
			graphics.Transform = originalDrawPagesTransform;
		}
		#endregion
		#region Begin/End DrawPageContent
		HdcOriginModifier originModifier;
		RectangleF oldClipBounds;
		protected internal virtual void BeginDrawPageContent(PageViewInfo page, Matrix transform) {
			graphics.ResetTransform();
			graphics.TranslateTransform(page.ClientBounds.X, page.ClientBounds.Y);
			graphics.MultiplyTransform(transform);
			GdiPlusBoxMeasurer measurer = (GdiPlusBoxMeasurer)Control.InnerControl.Measurer;
			Point origin = page.ClientBounds.Location;
			origin.X -= physicalLeftInvisibleWidth;
			origin.X = measurer.SnapToPixels(origin.X, graphics.DpiX);
			this.originModifier = new HdcOriginModifier(graphics, origin, View.ZoomFactor, HdcOriginModifier.Mode.Combine);
		}
		protected internal virtual void ClipPageContent(PageViewInfo page, Painter painter) {
			Rectangle clipBounds = View.CalculatePageContentClipBounds(page);
			oldClipBounds = cache.Graphics.ClipBounds;
			Rectangle newClipBounds = Rectangle.Intersect(Rectangle.Round(oldClipBounds), clipBounds);
			painter.ClipBounds = newClipBounds;
		}
		protected internal virtual void EndDrawPageContent(PageViewInfo page, Matrix transform) {
			this.originModifier.Dispose();
			this.originModifier = null;
		}
		#endregion
		#region Begin/End DrawInPixels
		GraphicsState state;
		HdcDpiModifier pixelHdcDpiModifier;
		protected internal virtual void BeginDrawInPixels(GraphicsCache cache) {
			this.cache = cache;
			this.graphics = cache.Graphics;
			this.state = graphics.Save();
			Matrix transform = this.graphics.Transform;
			float dpi = View.DocumentModel.LayoutUnitConverter.Dpi;
			float scaleX = this.graphics.DpiX / dpi;
			float scaleY = this.graphics.DpiY / dpi;
			transform.Translate(transform.OffsetX * (scaleX - 1), transform.OffsetY * (scaleY - 1));
			this.graphics.PageUnit = GraphicsUnit.Pixel;
			this.graphics.PageScale = 1.0f;
			this.pixelHdcDpiModifier = new HdcDpiModifier(graphics, new Size(4096, 4096), (int)Math.Round(Control.DpiX));
			this.graphics.Transform = transform;
		}
		protected internal virtual void EndDrawInPixels() {
			this.pixelHdcDpiModifier.Dispose();
			this.pixelHdcDpiModifier = null;
			this.graphics.Restore(state);
			this.cache = null;
			this.graphics = null;
		}
		#endregion
		protected internal virtual void DrawPageContent(PageViewInfo pageViewInfo, Painter painter) {
			GraphicsDocumentLayoutExporterAdapter adapter = new WinFormsGraphicsDocumentLayoutExporterAdapter();
			Rectangle pageBounds = GetPageBounds(pageViewInfo);
			using (DocumentLayoutExporter exporter = View.CreateDocumentLayoutExporter(painter, adapter, pageViewInfo, pageBounds)) {
				exporter.ShowWhitespace = control.DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText;
				exporter.MinReadableTextHeight = minReadableTextHeight;
				exporter.SetBackColor(Control.BackgroundPainter.GetActualPageBackColor(), pageBounds);
				exporter.ReadOnly = Control.ReadOnly;
				Debug.Assert(pageViewInfo.Page.SecondaryFormattingComplete);
				DevExpress.XtraRichEdit.API.Layout.PagePainter pagePainter = new API.Layout.PagePainter();
				DevExpress.XtraRichEdit.API.Layout.LayoutPage page = Control.DocumentLayout.GetPage(pageViewInfo.Page.PageIndex);
				pagePainter.Initialize(exporter, page);
				BeforePagePaintEventArgs args = new BeforePagePaintEventArgs(pagePainter, page, exporter, DevExpress.XtraRichEdit.API.Layout.CanvasOwnerType.Control);
				Control.InnerControl.RaiseBeforePagePaint(args);
				if (args.Painter != null)
					args.Painter.Draw();
			}
		}
		protected internal virtual void DrawPageCommentsArea(PageViewInfo pageViewInfo, Painter painter) {
			Page page = pageViewInfo.Page;
			GraphicsDocumentLayoutExporterAdapter adapter = new WinFormsGraphicsDocumentLayoutExporterAdapter();
			int count = page.Comments.Count;
			if (HideCommentLine) {
				SetCommentViewInfoIsActive(page.Comments);
				SetCommentViewInfoFocused(page.Comments);
			}
			if (CommentsVisible(count)) {
				for (int i = 0; i < count; i++) {
					CommentViewInfo commentViewInfo = page.Comments[i];
					RectangleF oldClipBounds = painter.ClipBounds;
					IntersectClipBounds(painter, oldClipBounds, page.CommentBounds.Left, page.CommentBounds.Top, page.CommentBounds.Width, page.CommentBounds.Height);
					DrawEmptyComment(cache, commentViewInfo);
					painter.DrawString(commentViewInfo.CommentHeading, commentViewInfo.CommentHeadingFontInfo, Color.Black, commentViewInfo.CommentHeadingBounds);
					if (commentViewInfo.ActualSize > commentViewInfo.ContentBounds.Height) {
						DrawEmptyExtensionComment(cache, commentViewInfo);
					}
					painter.ClipBounds = oldClipBounds;
				}
			}
		}
		protected virtual bool CommentsVisible(int count) {
			return (View.DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible) && (count > 0);
		}
		protected internal virtual void DrawCommentsBackground(PageViewInfo pageViewInfo, GraphicsCache cache) {
			int count = pageViewInfo.Page.Comments.Count;
			if ((CommentsVisible(count))) {
				Rectangle bounds = pageViewInfo.ClientCommentsBounds;
				bounds.Y++;
				bounds.Height -= 2;
				bounds.Width--;
				cache.FillRectangle(Color.LightGray, bounds);
				cache.Graphics.DrawLine(Pens.DarkGray, bounds.X, bounds.Top, bounds.X, bounds.Bottom - 1);
			}
		}
		protected internal virtual Rectangle GetPageBounds(PageViewInfo page) {
			return page.Page.Bounds;
		}
		protected internal virtual void DrawPagesContent(Matrix transform) {
			using (Painter painter = control.MeasurementAndDrawingStrategy.CreateDocumentPainter(new GraphicsCacheDrawingSurface(cache))) {
				int count = View.PageViewInfos.Count;
				for (int i = 0; i < count; i++) {
					PageViewInfo pageViewInfo = View.PageViewInfos[i];
					BeginDrawPageContent(pageViewInfo, transform);
					ClipPageContent(pageViewInfo, painter);
					DrawPageCommentsArea(pageViewInfo, painter);
					DrawPageContent(pageViewInfo, painter);
					RestoreOldClipBounds(painter);
					RectangleF oldClipBounds = painter.ClipBounds;
					ClipPageSelection(pageViewInfo, painter, oldClipBounds);
					DrawPageSelection(graphics, pageViewInfo);
					painter.ClipBounds = oldClipBounds;
					DrawHover(painter);
					EndDrawPageContent(pageViewInfo, transform);
				}
			}
		}
		void ClipPageSelection(PageViewInfo pageViewInfo, Painter painter, RectangleF oldClipBounds) {
			CommentSelectionLayout layout = View.SelectionLayout as CommentSelectionLayout;
			if (layout != null) {
				CommentViewInfoHelper helper = new CommentViewInfoHelper();
				CommentViewInfo comment = helper.FindCommentViewInfo(pageViewInfo.Page, layout.PieceTable);
				if (comment != null) {
					IntersectClipBounds(painter, oldClipBounds, comment.ContentBounds.X, comment.ContentBounds.Y, comment.ContentBounds.Width, comment.ContentBounds.Height);
				}
			}
		}
		protected virtual void RestoreOldClipBounds(Painter painter) {
			painter.ClipBounds = oldClipBounds;
		}
		protected internal virtual RectangleF GetNewClientBounds(RectangleF clipBounds, RectangleF oldClipBounds) {
			return RectangleF.Intersect(oldClipBounds, clipBounds);
		}
		protected internal virtual void DrawBoxesInPixels(ICustomMarkExporter customMarkExporter) {
			using (Painter painter = control.MeasurementAndDrawingStrategy.CreateDocumentPainter(new GraphicsCacheDrawingSurface(cache))) {
				WinFormsNotPrintableGraphicsBoxExporter exporter = new WinFormsNotPrintableGraphicsBoxExporter(DocumentModel, painter, View, customMarkExporter);
				int count = View.PageViewInfos.Count;
				for (int i = 0; i < count; i++) {
					PageViewInfo pageViewInfo = View.PageViewInfos[i];
					Matrix oldTransform = BeginDrawPageContentInPixels(pageViewInfo);
					RectangleF oldClipBounds = ClipPageContentForDrawingInPixels(pageViewInfo, painter);
					DevExpress.XtraRichEdit.API.Layout.PagePainter pagePainter = new API.Layout.PagePainter(true);
					pagePainter.Initialize(exporter, Control.DocumentLayout.GetPage(pageViewInfo.Page.PageIndex));
					pagePainter.Draw();
					painter.ClipBounds = oldClipBounds;
					if (CommentsVisible(count)) {
						DrawCommentLines(painter, exporter, pageViewInfo);
						DrawCommentContent(painter, exporter, pagePainter, customMarkExporter);
					}
					EndDrawPageContentInPixels(oldTransform);
				}
			}
		}
		Matrix BeginDrawPageContentInPixels(PageViewInfo pageViewInfo) {
			Matrix transform = graphics.Transform.Clone();
			graphics.ResetTransform();
			Point origin = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(pageViewInfo.ClientBounds.Location, graphics.DpiX, graphics.DpiY);
			origin.X = (int)(origin.X);
			origin.Y = (int)(origin.Y);
			origin.X -= (int)(DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(this.physicalLeftInvisibleWidth, graphics.DpiX));
			GdiPlusBoxMeasurer measurer = (GdiPlusBoxMeasurer)Control.InnerControl.Measurer;
			origin.X = measurer.SnapToPixels(origin.X, graphics.DpiX);
			graphics.TranslateTransform(origin.X, origin.Y);
			graphics.MultiplyTransform(transform);
			return transform;
		}
		void EndDrawPageContentInPixels(Matrix oldTransform) {
			graphics.Transform = oldTransform.Clone();
			oldTransform.Dispose();
		}
		RectangleF ClipPageContentForDrawingInPixels(PageViewInfo pageViewInfo, Painter painter) {
			Rectangle clipBounds = View.CalculatePageContentClipBounds(pageViewInfo);
			clipBounds = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(clipBounds, graphics.DpiX, graphics.DpiY);
			RectangleF actualClipBounds = new RectangleF();
			actualClipBounds.X = clipBounds.X * View.ZoomFactor;
			actualClipBounds.Y = clipBounds.Y * View.ZoomFactor;
			actualClipBounds.Width = clipBounds.Width * View.ZoomFactor;
			actualClipBounds.Height = clipBounds.Height * View.ZoomFactor;
			RectangleF oldClipBounds = cache.Graphics.ClipBounds;
			painter.ClipBounds = GetNewClientBounds(actualClipBounds, oldClipBounds);
			return oldClipBounds;
		}
		protected internal void SetCommentViewInfoIsActive(List<CommentViewInfo> comments) {
			if (comments != null && comments.Count > 0) {
				int index = 0;
				while (!comments[index].IsActive && index < comments.Count - 1)
					index++;
				if (comments[index].IsActive) {
					CommentViewInfo commentViewInfo = comments[index];
					if (commentViewInfo != null) {
						CommentViewInfoHelper helper = new CommentViewInfoHelper();
						helper.SetIsActive(comments, commentViewInfo);
					}
				}
			}
		}
		protected internal void SetCommentViewInfoFocused(List<CommentViewInfo> comments) {
			if (comments != null && comments.Count > 0) {
				int index = 0;
				while (!comments[index].Focused && index < comments.Count - 1)
					index++;
				if (comments[index].Focused) {
					CommentViewInfo commentViewInfo = comments[index];
					if (commentViewInfo != null) {
						CommentViewInfoHelper helper = new CommentViewInfoHelper();
						helper.SetFocused(comments, commentViewInfo);
					}
				}
			}
		}
		void DrawCommentLines(Painter painter, WinFormsNotPrintableGraphicsBoxExporter exporter, PageViewInfo pageViewInfo) {
			exporter.CurrentPageInfo = pageViewInfo;
			if (exporter.CurrentPageInfo != null) {
				Page page = pageViewInfo.Page;
				int countComments = page.Comments.Count;
				Point commentLineOffset = new Point(SkinPaintHelper.GetRichEditSkinIntProperty(LookAndFeel, SkinPaintHelper.CommentLineOffsetX),
					SkinPaintHelper.GetRichEditSkinIntProperty(LookAndFeel, SkinPaintHelper.CommentLineOffsetY));
				for (int j = 0; j < countComments; j++) {
					CommentViewInfo commentViewInfo = page.Comments[j];
					if ((!HideCommentLine) || (HideCommentLine && (commentViewInfo.Focused || commentViewInfo.IsActive))) {
						Color fillColor = DocumentModel.CommentColorer.GetColor(commentViewInfo.Comment);
						Color borderColor = CommentOptions.SetBorderColor(fillColor);
						Pen pen = new Pen(borderColor);
						if (commentViewInfo.Comment.ParentComment == null)
							DrawCommentLine(painter, exporter, commentViewInfo, page, pen, commentLineOffset);
						pen.Dispose();
					}
				}
			}
			exporter.CurrentPageInfo = null;
		}
		void DrawCommentLine(Painter painter, WinFormsNotPrintableGraphicsBoxExporter exporter, CommentViewInfo commentViewInfo, Page page, Pen pen, Point commentLineOffset) {
			Rectangle pageClientBounds = exporter.GetActualBounds(page.ClientBounds);
			Rectangle tightBounds = exporter.GetActualBounds(commentViewInfo.Character.TightBounds);
			Rectangle pageCommentBounds = exporter.GetActualBounds(page.CommentBounds);
			Rectangle commentViewInfoBounds = exporter.GetActualBounds(commentViewInfo.Bounds);
			float x1 = pageClientBounds.Left;
			float y1 = tightBounds.Bottom;
			float x2 = pageCommentBounds.Left;
			float y2 = y1;
			RectangleF oldClipBounds = painter.ClipBounds;
			IntersectClipBounds(painter, oldClipBounds, tightBounds.Right, y1 - 1, (pageCommentBounds.Left - tightBounds.Right), 3);
			painter.DrawLine(pen, x1, y1, x2, y2);
			x1 = x2;
			painter.ClipBounds = oldClipBounds;
			int radius = 5;
			x2 = commentViewInfoBounds.Left + commentLineOffset.X;
			y2 = commentViewInfoBounds.Top + radius + commentLineOffset.Y;
			IntersectClipBounds(painter, oldClipBounds, pageCommentBounds.Left, pageCommentBounds.Top, commentViewInfoBounds.Left + commentLineOffset.X - pageCommentBounds.Left, pageCommentBounds.Height);
			painter.PushPixelOffsetMode(true);
			painter.PushSmoothingMode(true);
			painter.DrawLine(pen, x1, y1, x2, y2);
			painter.PopPixelOffsetMode();
			painter.PopSmoothingMode();
			painter.ClipBounds = oldClipBounds;
		}
		void DrawCommentContent(Painter painter, WinFormsNotPrintableGraphicsBoxExporter exporter, DevExpress.XtraRichEdit.API.Layout.PagePainter pagePainter, ICustomMarkExporter customMarkExporter) {
			int commentsCount = pagePainter.Page.Comments.Count;
			for (int i = 0; i < commentsCount; i++) {
				CommentViewInfo commentViewInfo = pagePainter.Page.Comments[i].ModelComment;
				Rectangle commentViewInfoContentBounds = exporter.GetActualBounds(commentViewInfo.ContentBounds);
				RectangleF oldClipBounds = painter.ClipBounds;
				IntersectClipBounds(painter, oldClipBounds, commentViewInfoContentBounds.X, commentViewInfoContentBounds.Y, commentViewInfoContentBounds.Width, commentViewInfoContentBounds.Height);
				Matrix oldTransform = cache.Graphics.Transform;
				cache.Graphics.TranslateTransform(commentViewInfo.ContentBounds.X, commentViewInfo.ContentBounds.Y);
				pagePainter.DrawComment(pagePainter.Page.Comments[i]);
				cache.Graphics.Transform = oldTransform;
				painter.ClipBounds = oldClipBounds;
			}
		}
		void IntersectClipBounds(Painter painter, RectangleF oldClipBounds, float x, float y, float width, float height) {
			RectangleF clipBounds = new RectangleF(x, y, width, height);
			clipBounds.Intersect(oldClipBounds);
			painter.ClipBounds = clipBounds;
		}
		protected internal virtual int CalculateRulerHeight() {
			return 0;
		}
		public virtual void DrawDecorators(GraphicsCache cache) {
			using (Painter painter = control.MeasurementAndDrawingStrategy.CreateDocumentPainter(new GraphicsCacheDrawingSurface(cache))) {
				DrawDecorators(painter);
				DecorateTables(painter);
			}
		}
		protected internal virtual void DecorateTables(Painter painter) {
			TableViewInfoController controller = View.TableController;
			if (controller != null)
				DecorateTablesCore(controller, painter);
		}
		void DecorateTablesCore(TableViewInfoController controller, Painter painter) {
			using (ITableViewInfoDecorator decorator = controller.CreateDecorator(painter))
				decorator.Decorate();
		}
		protected internal virtual void DrawDecorators(Painter painter) {
			int count = DecoratorPainters.Count;
			PageViewInfoCollection viewInfos = View.PageViewInfos;
			for (int i = 0; i < count; i++) {
				IDecoratorPainter decoratorPainter = DecoratorPainters[i];
				decoratorPainter.DrawDecorators(painter, viewInfos);
			}
		}
		public virtual void Draw(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			BeginDraw(cache);
			DrawEmptyPages(cache);
			Matrix oldTransform = BeginDrawPagesContent();
			DrawPagesContent(oldTransform);
			EndDrawPagesContent();
			Control.RaiseCustomDrawActiveView(cache);
			EndDraw();
			BeginDrawInPixels(cache);
			DrawBoxesInPixels(customMarkExporter);
			ApplyGraphicsTransform(cache, customMarkExporter.CustomMarkVisualInfoCollection);
			EndDrawInPixels();
		}
		Rectangle GetCommentBounds(CommentViewInfo commentViewInfo) {
			return commentViewInfo.ContentBounds;
		}
		protected virtual void ApplyGraphicsTransform(GraphicsCache cache, CustomMarkVisualInfoCollection customMarkVisualInfoCollection) {
			int count = customMarkVisualInfoCollection.Count;
			if (count == 0)
				return;
			Point[] pts = new Point[count * 2];
			for (int i = 0; i < count; i++) {
				CustomMarkVisualInfo visualInfo = customMarkVisualInfoCollection[i];
				Rectangle bounds = visualInfo.Bounds;
				Point topLeft = new Point(bounds.X, bounds.Y);
				Point bottomRight = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height);
				pts[i * 2] = topLeft;
				pts[i * 2 + 1] = bottomRight;
			}
			cache.Graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, pts);
			for (int i = 0; i < count; i++) {
				CustomMarkVisualInfo visualInfo = customMarkVisualInfoCollection[i];
				Point topLeft = pts[i * 2];
				Point bottomRight = pts[i * 2 + 1];
				visualInfo.Bounds = Rectangle.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
			}
		}
		protected internal virtual void DrawAtPageCore(GraphicsCache cache, PageViewInfo page, DrawAtPageDelegate draw) {
			Rectangle pageViewInfoClientBounds = page.ClientBounds;
			Graphics gr = cache.Graphics;
			Matrix oldTransform = gr.Transform.Clone();
			try {
				gr.ResetTransform();
				gr.TranslateTransform(pageViewInfoClientBounds.X - View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth(), pageViewInfoClientBounds.Y);
				gr.MultiplyTransform(oldTransform);
				gr.ScaleTransform(View.ZoomFactor, View.ZoomFactor);
				using (HdcZoomModifier zoomModifier = new HdcZoomModifier(gr, View.ZoomFactor)) {
					Point origin = pageViewInfoClientBounds.Location;
					origin.X -= View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
					using (HdcOriginModifier originModifier = new HdcOriginModifier(gr, origin, View.ZoomFactor, HdcOriginModifier.Mode.Combine)) {
						draw(cache);
					}
				}
			}
			finally {
				gr.Transform = oldTransform.Clone();
			}
		}
		protected internal virtual void DrawCaretCore(GraphicsCache cache, Caret caret) {
			caret.Draw(cache.Graphics);
		}
		public virtual void DrawCaretAtPage(GraphicsCache cache, Caret caret, PageViewInfo page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				DrawCaretCore(c, caret);
			};
			DrawAtPageCore(cache, page, draw);
		}
		public virtual void DrawReversibleHorizontalLineAtPage(GraphicsCache cache, int y, PageViewInfo page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				Rectangle bounds = page.Page.Bounds;
				bounds.Y = y;
				bounds.Height = 0;
				DrawReversibleHorizontalLine(c, bounds);
			};
			DrawAtPageCore(cache, page, draw);
		}
		public virtual void DrawReversibleVerticalLineAtPage(GraphicsCache cache, int x, PageViewInfo page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				Rectangle bounds = page.Page.Bounds;
				bounds.X = x;
				bounds.Width = 0;
				DrawReversibleVerticalLine(c, bounds);
			};
			DrawAtPageCore(cache, page, draw);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void DrawReversibleHorizontalLine(GraphicsCache cache, Rectangle bounds) {
			IntPtr hdc = cache.Graphics.GetHdc();
			try {
				DrawReversibleHorizontalLine(hdc, bounds);
			}
			finally {
				cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawReversibleHorizontalLine(IntPtr hdc, Rectangle bounds) {
			DrawReversible(hdc, bounds, DrawReversibleHorizontalLineCore);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void DrawReversibleVerticalLine(GraphicsCache cache, Rectangle bounds) {
			IntPtr hdc = cache.Graphics.GetHdc();
			try {
				DrawReversibleVerticalLine(hdc, bounds);
			}
			finally {
				cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawReversibleVerticalLine(IntPtr hdc, Rectangle bounds) {
			DrawReversible(hdc, bounds, DrawReversibleVerticalLineCore);
		}
		public virtual void DrawReversibleFrameAtPage(GraphicsCache cache, Rectangle bounds, PageViewInfo page) {
			DrawAtPageDelegate draw = delegate(GraphicsCache c) {
				DrawReversibleFrame(c, bounds);
			};
			DrawAtPageCore(cache, page, draw);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void DrawReversibleFrame(GraphicsCache cache, Rectangle bounds) {
			IntPtr hdc = cache.Graphics.GetHdc();
			try {
				DrawReversibleFrame(hdc, bounds);
			}
			finally {
				cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawReversibleFrame(IntPtr hdc, Rectangle bounds) {
			DrawReversible(hdc, bounds, DrawReversibleFrameCore);
		}
		protected internal delegate void DrawReversibleDelegate(IntPtr hdc, Rectangle bounds);
		protected internal virtual void DrawReversible(IntPtr hdc, Rectangle bounds, DrawReversibleDelegate draw) {
			Win32.BinaryRasterOperation oldRop2 = Win32.SetROP2(hdc, Win32.BinaryRasterOperation.R2_NOTXORPEN);
			try {
				IntPtr pen = Win32.CreatePen(Win32.PenStyle.PS_DOT, 0, 0x0);
				IntPtr oldPen = Win32.SelectObject(hdc, pen);
				try {
					IntPtr brush = Win32.GetStockObject(Win32.StockObject.NULL_BRUSH);
					IntPtr oldBrush = Win32.SelectObject(hdc, brush);
					try {
						draw(hdc, bounds);
					}
					finally {
						Win32.SelectObject(hdc, oldBrush);
					}
				}
				finally {
					Win32.SelectObject(hdc, oldPen);
					Win32.DeleteObject(pen);
				}
			}
			finally {
				Win32.SetROP2(hdc, oldRop2);
			}
		}
		protected internal virtual void DrawReversibleFrameCore(IntPtr hdc, Rectangle bounds) {
			Win32.MoveTo(hdc, bounds.Left, bounds.Top);
			Win32.LineTo(hdc, bounds.Right, bounds.Top);
			Win32.LineTo(hdc, bounds.Right, bounds.Bottom);
			Win32.LineTo(hdc, bounds.Left, bounds.Bottom);
			Win32.LineTo(hdc, bounds.Left, bounds.Top);
		}
		protected internal virtual void DrawReversibleHorizontalLineCore(IntPtr hdc, Rectangle bounds) {
			Win32.MoveTo(hdc, bounds.Left, bounds.Top);
			Win32.LineTo(hdc, bounds.Right, bounds.Top);
		}
		protected internal virtual void DrawReversibleVerticalLineCore(IntPtr hdc, Rectangle bounds) {
			Win32.MoveTo(hdc, bounds.Left, bounds.Top);
			Win32.LineTo(hdc, bounds.Left, bounds.Bottom);
		}
		protected internal virtual void DrawPageSelection(Graphics gr, PageViewInfo page) {
			GraphicsState stateOld = gr.Save();
			try {
				CommentSelectionLayout layout = View.SelectionLayout as CommentSelectionLayout;
				if (layout != null) {
					CommentViewInfoHelper helper = new CommentViewInfoHelper();
					CommentViewInfo comment = helper.FindCommentViewInfo(page.Page, layout.PieceTable);
					if (comment != null)
						gr.TranslateTransform(comment.ContentBounds.X, comment.ContentBounds.Y);
				}
				PageSelectionLayoutsCollection selections = View.SelectionLayout.GetPageSelection(page.Page);
				if (selections == null)
					return;
				for (int i = 0; i < selections.Count; i++) {
					DrawPageSelectionCore(selections[i]);
				}
			}
			finally {
				gr.Restore(stateOld);
			}
		}
		void DrawPageSelectionCore(ISelectionLayoutItem selection) {
			if (selection != null)
				selection.Draw(SelectionPainters.Get(selection.GetType()));
		}
		protected internal virtual void DrawHover(Painter painter) {
			IHoverLayoutItem hoverLayoutItem = View.HoverLayout;
			if (hoverLayoutItem != null)
				DrawHoverCore(hoverLayoutItem, painter);
		}
		void DrawHoverCore(IHoverLayoutItem hoverLayoutItem, Painter painter) {
			using (IHoverPainter hoverPainter = HoverPainters.Get(hoverLayoutItem, painter))
				hoverPainter.Draw();
		}
		protected internal virtual void DrawEmptyPages(GraphicsCache cache) {
			int count = View.PageViewInfos.Count;
			for (int i = 0; i < count; i++)
				DrawEmptyPage(cache, View.PageViewInfos[i]);
		}
		protected internal virtual void ResetCache() {
		}
		protected internal virtual void DrawEmptyPage(GraphicsCache cache, PageViewInfo page) {
			DrawCommentsBackground(page, cache);
		}
		protected internal virtual void DrawEmptyComment(GraphicsCache cache, CommentViewInfo commentViewInfo) {
			Rectangle bounds = commentViewInfo.Bounds;
			int radius = UnitConverter.DocumentsToLayoutUnits(15);
			Color fillColor = DocumentModel.CommentColorer.GetColor(commentViewInfo.Comment);
			Color borderColor = CommentOptions.SetBorderColor(fillColor);
			DrawRoundedRectangle(cache, bounds, radius, fillColor, borderColor);
			Pen penDot = new Pen(Color.Black);
			penDot.DashStyle = DashStyle.Dot;
			cache.DrawRectangle(penDot, commentViewInfo.ContentBounds);
			penDot.Dispose();
		}
		protected internal virtual void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo commentViewInfo) {
			Rectangle commentMoreButtonBounds = commentViewInfo.CommentMoreButtonBounds;
			if (commentMoreButtonBounds.Width == 0 || commentMoreButtonBounds.Height == 0)
				return;
			int radius = UnitConverter.DocumentsToLayoutUnits(15);
			Color fillColor = Color.White;
			Color lineColor = Color.Black;
			DrawRoundedRectangle(cache, commentMoreButtonBounds, radius, fillColor, lineColor);
			cache.DrawString("  ... ", commentViewInfo.CommentHeadingFontInfo.Font, Brushes.Black, commentViewInfo.CommentMoreButtonBounds, StringFormat.GenericTypographic);
		}
		protected void DrawRoundedRectangle(GraphicsCache cache, Rectangle rect, int radius, Color fillColor, Color lineColor) {
			CommentPainter painter = new CommentPainter();
			GraphicsPath path = painter.CreatePathRoundedRectangle(rect, radius);
			SolidBrush brush = new SolidBrush(fillColor);
			cache.Graphics.FillPath(brush, path);
			cache.Graphics.DrawPath(new Pen(lineColor), path);
		}
	}
	#endregion
	#region RichEditPrintingSkins
	public static class RichEditPrintingSkins {
		public static readonly string SkinControlBackground = "PreviewBackground";
		public static readonly string SkinPageBorder = "PageBorder";
		public static readonly string SkinRulerForeColor = "RulerForeColor";
	}
	#endregion
	#region RichEditViewBackgroundPainter (abstract class)
	public abstract class RichEditViewBackgroundPainter : IDisposable {
		readonly RichEditView view;
		protected RichEditViewBackgroundPainter(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public RichEditView View { get { return view; } }
		public virtual void Draw(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(cache.GetSolidBrush(SystemColors.ControlDarkDark), bounds);
		}
		protected internal virtual Color GetActualPageBackColor() {
			DocumentProperties documentProperties = View.DocumentModel.DocumentProperties;
			Color pageBackColor = documentProperties.PageBackColor;
			if (documentProperties.DisplayBackgroundShape && pageBackColor != DXColor.Empty)
				return pageBackColor;
			return View.ActualBackColor;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region RichEditViewSkinPainterHelper
	public class RichEditViewSkinPainterHelper {
		readonly RichEditView view;
		readonly PageImageCache pageImageCache;
		readonly CommentImageCache commentImageCache;
		SkinElement mainAreaElement;
		public RichEditViewSkinPainterHelper(RichEditView view) {
			this.view = view;
			this.pageImageCache = new PageImageCache();
			this.commentImageCache = new CommentImageCache();
		}
		protected internal RichEditView View { get { return view; } }
		protected internal RichEditControl Control { get { return (RichEditControl)view.Control; } }
		protected internal UserLookAndFeel LookAndFeel { get { return Control.LookAndFeel; } }
		protected internal virtual PageImage GetPageImage(Page page, Rectangle pixelBounds, int commentsAreaWidthInPixel) {
			PageImage result;
			PageImageCacheKey key = new PageImageCacheKey(pixelBounds.Size, commentsAreaWidthInPixel);
			if (!pageImageCache.TryGetValue(key, out result)) {
				Rectangle rect = pixelBounds;
				rect.X = 0;
				rect.Y = 0;
				result = CreatePageImage(rect, commentsAreaWidthInPixel);
				pageImageCache.Add(key, result);
			}
			return result;
		}
		protected internal virtual void DrawPageImage(GraphicsCache cache, Rectangle pixelBounds, int commentsAreaWidthInPixel) {
			if (commentsAreaWidthInPixel <= 0) {
				SkinElementInfo element = CreatePageSkinElementInfo(pixelBounds);
				element.Bounds = pixelBounds;
				element.ImageIndex = 1;
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, element);
			}
			else {
				Rectangle mainAreaBounds = new Rectangle(pixelBounds.X, pixelBounds.Y, pixelBounds.Size.Width - commentsAreaWidthInPixel, pixelBounds.Size.Height);
				Rectangle commentAreaBounds = new Rectangle(pixelBounds.X + pixelBounds.Size.Width - commentsAreaWidthInPixel, pixelBounds.Y, commentsAreaWidthInPixel, pixelBounds.Size.Height);
				SkinElementInfo mainAreaElementInfo = CreateMainAreaSkinElementInfo(mainAreaBounds);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, mainAreaElementInfo);
				SkinElement commentAreaElement = SkinPaintHelper.GetRichEditSkinElement(LookAndFeel, RichEditSkins.SkinCommentsArea);
				if (commentAreaElement != null) {
					SkinElementInfo commentAreaElementInfo = new SkinElementInfo(commentAreaElement, commentAreaBounds);
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, commentAreaElementInfo);
				}
			}
		}
		protected internal SkinElementInfo CreateMainAreaSkinElementInfo(Rectangle mainAreaBounds) {
			SkinElement mainAreaElement = GetPageMainAreaElement();
			Color pageBackColor = GetActualPageBackColor(View.DocumentModel.DocumentProperties, View.ActualBackColor);
			pageImageCache.ColoredPageWithCommentSkinElement = SkinElementColorer.PaintElementWithColor(mainAreaElement, Color.White, pageBackColor);
			return new SkinElementInfo(pageImageCache.ColoredPageWithCommentSkinElement, mainAreaBounds);
		}
		protected internal virtual PageImage CreatePageImage(Rectangle pixelBounds, int commentsAreaWidthInPixel) {
			Bitmap pageImage = new Bitmap(pixelBounds.Width, pixelBounds.Height);
			using (Graphics gr = Graphics.FromImage(pageImage)) {
				using (GraphicsCache cache = new GraphicsCache(gr)) {
					DrawPageImage(cache, pixelBounds, commentsAreaWidthInPixel);
				}
			}
			return new PageImage(pageImage, new Point(0, 0));
		}
		protected internal virtual SkinElement GetPageMainAreaElement() {
			if (mainAreaElement != null)
				return mainAreaElement;
			SkinElement sourceElement = SkinPaintHelper.GetSkinElement(LookAndFeel, RichEditPrintingSkins.SkinPageBorder);
			mainAreaElement = CreateCroppedElement(sourceElement, "_left", edges => edges.Right = 0,
				image => {
					Rectangle rect = image.GetImageBounds(1);
					return new Rectangle(rect.Left, rect.Top, rect.Width - image.SizingMargins.Right, rect.Height);
				});
			return mainAreaElement;
		}
		protected internal virtual SkinElement CreateCroppedElement(SkinElement source, string nameSuffix, Action<SkinPaddingEdges> cropEdge, Func<SkinImage, Rectangle> getSourceRect) {
			SkinElement result = source.Copy(source.Owner, source.ElementName + nameSuffix);
			result.IsCustomElement = true;
			result.Original = source;
			SkinPaddingEdges borderThin = (SkinPaddingEdges)source.Info.Border.Thin.Clone();
			cropEdge(borderThin);
			result.Info.Border.Thin = borderThin;
			SkinPaddingEdges contentMargins = (SkinPaddingEdges)source.Info.ContentMargins.Clone();
			cropEdge(contentMargins);
			result.Info.ContentMargins = contentMargins;
			result.Info.Image = CropImage(source.Image, cropEdge, getSourceRect);
			result.Info.Glyph = CropGlyph(source.Glyph, cropEdge, getSourceRect);
			return result;
		}
		SkinGlyph CropGlyph(SkinGlyph skinGlyph, Action<SkinPaddingEdges> cropEdge, Func<SkinImage, Rectangle> getSourceRect) {
			return (SkinGlyph)CropImage(skinGlyph, cropEdge, getSourceRect);
		}
		SkinImage CropImage(SkinImage source, Action<SkinPaddingEdges> cropEdge, Func<SkinImage, Rectangle> getSourceRect) {
			if (source == null)
				return null;
			if (source.Image == null)
				return new SkinImage((Image)null);
			SkinImage image = new SkinImage(CreateCroppedImage(source, getSourceRect(source)));
			image.ImageCount = 1;
			image.SetImageNameCore(source.ImageName);
			image.Layout = source.Layout;
			SkinPaddingEdges sizingMargins = (SkinPaddingEdges)source.SizingMargins.Clone();
			cropEdge(sizingMargins);
			image.SizingMargins = sizingMargins;
			image.Stretch = source.Stretch;
			image.TransparentColor = source.TransparentColor;
			image.UseOwnImage = source.UseOwnImage;
			return image;
		}
		Image CreateCroppedImage(SkinImage source, Rectangle sourceRect) {
			Image sourceImage = source.Image;
			Bitmap img = new Bitmap(sourceRect.Width, sourceRect.Height);
			img.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
			using (Graphics g = Graphics.FromImage(img)) {
				g.Clear(Color.Transparent);
				g.DrawImage(sourceImage, new Rectangle(Point.Empty, sourceRect.Size), sourceRect, GraphicsUnit.Pixel);
			}
			return img;
		}
		protected internal SkinElementInfo CreatePageSkinElementInfo(Rectangle bounds) {
			if (pageImageCache.ColoredPageSkinElement == null) {
				SkinElement el = SkinPaintHelper.GetSkinElement(LookAndFeel, RichEditPrintingSkins.SkinPageBorder);
				Color pageBackColor = GetActualPageBackColor(View.DocumentModel.DocumentProperties, View.ActualBackColor);
				pageImageCache.ColoredPageSkinElement = SkinElementColorer.PaintElementWithColor(el, Color.White, pageBackColor);
			}
			return new SkinElementInfo(pageImageCache.ColoredPageSkinElement, bounds);
		}
		protected internal virtual Color GetActualPageBackColor(DocumentProperties properties, Color viewBackColor) {
			Color pageBackColor = properties.PageBackColor;
			if (properties.DisplayBackgroundShape && pageBackColor != DXColor.Empty)
				return pageBackColor;
			return viewBackColor;
		}
		protected internal virtual Rectangle CalculateViewInfoPixelBounds(Rectangle bounds) {
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			RichEditControl control = (RichEditControl)View.Control;
			bounds.Offset(control.ViewBounds.X - View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth(), control.ViewBounds.Y);
			bounds.X = unitConverter.LayoutUnitsToPixels(bounds.X);
			bounds.Width = unitConverter.LayoutUnitsToPixels(bounds.Width);
			bounds.Y = unitConverter.LayoutUnitsToPixels(bounds.Y);
			bounds.Height = unitConverter.LayoutUnitsToPixels(bounds.Height);
			return bounds;
		}
		protected internal virtual void DrawEmptyComment(GraphicsCache cache, CommentViewInfo comment) {
			CommentImage commentImage = GetCommentImage(comment);
			Point location = comment.Bounds.Location;
			location.Offset(commentImage.Offset);
			cache.Graphics.DrawImageUnscaled(commentImage.Image, location);
		}
		protected internal virtual CommentImage GetCommentImage(CommentViewInfo comment) {
			CommentImage result;
			Color fillColor = View.DocumentModel.CommentColorer.GetColor(comment.Comment);
			Rectangle commentBounds = CalculateCommentViewInfoPixelBounds(comment.Bounds);
			CommentImageCacheKey key = new CommentImageCacheKey(commentBounds.Size, fillColor);
			if (!commentImageCache.TryGetValue(key, out result)) {
				Rectangle rect = commentBounds;
				rect.X = 0;
				rect.Y = 0;
				result = CreateCommentImage(comment, commentBounds);
				commentImageCache.Add(key, result);
			}
			return result;
		}
		protected internal virtual void DrawCommentImage(GraphicsCache cache, CommentViewInfo comment, Rectangle commentBounds) {
			Color fillColor = View.DocumentModel.CommentColorer.GetColor(comment.Comment);
			SkinElementInfo element = CreateCommentSkinElementInfo(commentBounds, RichEditSkins.SkinCommentBorder, fillColor);
			if (element.Element != null) {
				DrawCommentImageCore(cache, commentBounds, element);
			}
			else {
			}
		}
		protected internal void DrawCommentImageCore(GraphicsCache cache, Rectangle commentBounds, SkinElementInfo element) {
			element.Bounds = commentBounds;
			element.ImageIndex = 1;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, element);
		}
		protected internal virtual CommentImage CreateCommentImage(CommentViewInfo comment, Rectangle commentBounds) {
			Bitmap commentImage = new Bitmap(commentBounds.Width, commentBounds.Height);
			using (Graphics gr = Graphics.FromImage(commentImage)) {
				using (GraphicsCache cache = new GraphicsCache(gr)) {
					DrawCommentImage(cache, comment, new Rectangle(new Point(0, 0), commentBounds.Size));
				}
			}
			return new CommentImage(commentImage, new Point(0, 0));
		}
		protected internal virtual void DrawEmptyExtensionComment(GraphicsCache cache, CommentViewInfo comment, SkinElement moreButtonElement) {
			Rectangle rect = comment.CommentMoreButtonBounds;
			SkinElementInfo element = new SkinElementInfo(moreButtonElement, rect);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, element);
		}
		protected internal SkinElementInfo CreateCommentSkinElementInfo(Rectangle bounds, string elementName, Color color) {
			SkinElement coloredCommentSkinElement;
			if (!commentImageCache.ColoredCommentSkinElementCollection.TryGetValue(color, out coloredCommentSkinElement)) {
				SkinElement el = GetSkinElement(LookAndFeel, elementName);
				if (el != null) {
					coloredCommentSkinElement = SkinElementColorer.PaintElementWithColor(el, color);
					commentImageCache.ColoredCommentSkinElementCollection.Add(color, coloredCommentSkinElement);
				}
			}
			return new SkinElementInfo(coloredCommentSkinElement, bounds);
		}
		public SkinElement GetSkinElement(ISkinProvider provider, string elementName) {
			Skin skin = RichEditSkins.GetSkin(provider);
			return skin != null ? skin[elementName] : null;
		}
		protected internal virtual Color GetActualCommentBackColor(DocumentProperties properties, Color viewBackColor) {
			Color commentBackColor = properties.PageBackColor;
			if (properties.DisplayBackgroundShape && commentBackColor != DXColor.Empty)
				return commentBackColor;
			return viewBackColor;
		}
		protected internal virtual Rectangle CalculateCommentViewInfoPixelBounds(Rectangle rect) {
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			rect.X = unitConverter.LayoutUnitsToPixels(rect.X);
			rect.Width = unitConverter.LayoutUnitsToPixels(rect.Width);
			rect.Y = unitConverter.LayoutUnitsToPixels(rect.Y);
			rect.Height = unitConverter.LayoutUnitsToPixels(rect.Height);
			return rect;
		}
		protected internal void ResetCache() {
			this.pageImageCache.Reset();
			this.commentImageCache.Reset();
		}
	}
	#endregion
}
