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
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
#if !SL
using System.Windows.Forms;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentGiveFeedbackEventArgs = System.Windows.Forms.GiveFeedbackEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = System.Windows.Forms.QueryContinueDragEventArgs;
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
#else
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentGiveFeedbackEventArgs = DevExpress.Utils.GiveFeedbackEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = DevExpress.Utils.QueryContinueDragEventArgs;
#if SL4
using PlatformIndependentIDataObject = System.Windows.IDataObject;
using PlatformIndependentDataObject = System.Windows.DataObject;
#else
using PlatformIndependentIDataObject = DevExpress.Utils.IDataObject;
using PlatformIndependentDataObject = DevExpress.Utils.DataObject;
#endif
using DevExpress.XtraRichEdit.Drawing;
using System.Windows.Threading;
#endif
namespace DevExpress.XtraRichEdit.Mouse {
	public class WinFormsRichEditMouseHandlerStrategyFactory : RichEditMouseHandlerStrategyFactory {
		public override DragContentMouseHandlerStateBaseStrategy CreateDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state) {
			return new WinFormsDragContentMouseHandlerStateBaseStrategy(state);
		}
		public override DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state) {
			return new WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy(state);
		}
		public override RichEditMouseHandlerStrategy CreateMouseHandlerStrategy(RichEditMouseHandler mouseHandler) {
			return new WinFormsRichEditMouseHandlerStrategy(mouseHandler);
		}
		public override ResizeTableRowMouseHandlerStateStrategy CreateResizeTableRowMouseHandlerStateStrategy(ResizeTableRowMouseHandlerState state) {
			return new WinFormsResizeTableRowMouseHandlerStateStrategy(state);
		}
		public override ResizeTableVirtualColumnMouseHandlerStateStrategy CreateResizeTableVirtualColumnMouseHandlerStateStrategy(ResizeTableVirtualColumnMouseHandlerState state) {
			return new WinFormsResizeTableVirtualColumnMouseHandlerStateStrategy(state);
		}
		public override RichEditRectangularObjectResizeMouseHandlerStateStrategy CreateRichEditRectangularObjectResizeMouseHandlerStateStrategy(RichEditRectangularObjectResizeMouseHandlerState state) {
			return new WinFormsRichEditRectangularObjectResizeMouseHandlerStateStrategy(state);
		}
	}
	public class WinFormsRichEditMouseHandlerStrategy : RichEditMouseHandlerStrategy {
		public WinFormsRichEditMouseHandlerStrategy(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public RichEditControl WinControl { get { return (RichEditControl)Control; } }
		protected internal override void StartOfficeScroller(Point clientPoint) {
			if (!Control.InnerControl.Options.Behavior.OfficeScrollingAllowed)
				return;
			RichEditControl control = (RichEditControl)Control;
			Point screenPoint = control.PointToScreen(clientPoint);
			OfficeScroller.Start(control, screenPoint);
		}
		protected internal override IOfficeScroller CreateOfficeScroller() {
			return new RichEditOfficeScroller(Control);
		}
		protected internal override PlatformIndependentMouseEventArgs CreateFakeMouseMoveEventArgs() {
			Point screenMousePos = Cursor.Position;
			Point clientMousePoint = WinControl.PointToClient(screenMousePos);
			PlatformIndependentMouseEventArgs args = new PlatformIndependentMouseEventArgs(MouseButtons.None, 0, clientMousePoint.X, clientMousePoint.Y, 0);
			return ConvertMouseEventArgs(args);
		}
		protected internal override PlatformIndependentMouseEventArgs ConvertMouseEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
			Point location = WinControl.GetPhysicalPoint(screenMouseEventArgs.Location);
			DXMouseEventArgs eventArgs = DXMouseEventArgs.GetMouseArgs(screenMouseEventArgs);
			return new OfficeMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, location.X, location.Y, screenMouseEventArgs.Delta, eventArgs.IsHMouseWheel);
		}
		protected internal override PlatformIndependentDragEventArgs ConvertDragEventArgs(PlatformIndependentDragEventArgs screenDragEventArgs) {
			Point screenMousePos = new Point(screenDragEventArgs.X, screenDragEventArgs.Y);
			Point clientMousePoint = WinControl.PointToClient(screenMousePos);
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			int x = unitConverter.PixelsToLayoutUnits(clientMousePoint.X) - Control.ViewBounds.Left;
			int y = unitConverter.PixelsToLayoutUnits(clientMousePoint.Y) - Control.ViewBounds.Top;
			return new DragEventArgs(screenDragEventArgs.Data, screenDragEventArgs.KeyState, x, y, screenDragEventArgs.AllowedEffect, screenDragEventArgs.Effect);
		}
		protected internal override void AutoScrollerOnDragOver(Point pt) {
			MouseHandler.AutoScroller.OnMouseMove(WinControl.PointToClient(pt));
		}
		protected internal override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
		}
		protected internal override DragContentMouseHandlerStateBase CreateInternalDragState() {
			return new DragContentStandardMouseHandlerState(MouseHandler);
		}
	}
	public class ShadowedFloatingObjectPainter {
		DocumentLayoutUnitConverter unitConverter;
		OfficeImage image;
		Matrix transform;
		Rectangle bounds;
		Rectangle initialShapeBounds;
		Rectangle initialContentBounds;
		Color fillColor;
		Color outlineColor;
		float alpha = 1.0f;
		ImageSizeMode sizeMode = ImageSizeMode.StretchImage;
		ResizingShadowDisplayMode shadowDisplayMode = ResizingShadowDisplayMode.Content;
		public ShadowedFloatingObjectPainter(DocumentLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
		}
		public OfficeImage Image { get { return image; } set { image = value; } }
		public ImageSizeMode SizeMode { get { return sizeMode; } set { sizeMode = value; } }
		public ResizingShadowDisplayMode ShadowDisplayMode { get { return shadowDisplayMode; } set { shadowDisplayMode = value; } }
		public Matrix Transform { get { return transform; } set { transform = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle InitialShapeBounds { get { return initialShapeBounds; } set { initialShapeBounds = value; } }
		public Rectangle InitialContentBounds { get { return initialContentBounds; } set { initialContentBounds = value; } }
		public Color FillColor { get { return fillColor; } set { fillColor = value; } }
		public Color OutlineColor { get { return outlineColor; } set { outlineColor = value; } }
		public float Alpha { get { return alpha; } set { alpha = value; } }
		public void Paint(GraphicsCache cache) {
			Matrix originalTransform = null;
			SmoothingMode originalSmoothingMode = SmoothingMode.Default;
			Graphics graphics = cache.Graphics;
			if (transform != null) {
				originalTransform = graphics.Transform.Clone();
				Matrix newTransform = graphics.Transform;
				newTransform.Multiply(transform);
				graphics.Transform = newTransform;
				originalSmoothingMode = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
			}
			PaintCore(cache);
			if (transform != null) {
				graphics.Transform = originalTransform;
				graphics.SmoothingMode = originalSmoothingMode;
			}
		}
		void PaintCore(GraphicsCache cache) {
			Rectangle contentBounds = initialContentBounds;
			contentBounds.X += bounds.X - initialShapeBounds.X;
			contentBounds.Y += bounds.Y - initialShapeBounds.Y;
			contentBounds.Width += bounds.Width - initialShapeBounds.Width;
			contentBounds.Height += bounds.Height - initialShapeBounds.Height;
			DrawFeedbackShape(cache, bounds, contentBounds, FillColor, OutlineColor, unitConverter);
			if (image != null && ShouldDrawImage())
				DrawImage(cache, contentBounds);
			cache.DrawRectangle(cache.GetPen(Color.FromArgb(0x80, 0x00, 0x00, 0x00), 1), bounds);
		}
		bool ShouldDrawImage() {
			return ShadowDisplayMode == ResizingShadowDisplayMode.Content;
		}
		void DrawImage(GraphicsCache cache, Rectangle bounds) {
			if (alpha == 1.0f) {
				cache.Graphics.DrawImage(image.NativeImage, bounds);
				return;
			}
			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix33 = alpha;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			Size imgActualSize = image.NativeImage.Size;
			Rectangle imgRect = Rectangle.Round(DevExpress.Data.Utils.ImageTool.CalculateImageRectCore(bounds, imgActualSize, SizeMode));
			GraphicsClipState oldClipState = cache.ClipInfo.SaveClip();
			cache.ClipInfo.SetClip(bounds);
			try {
				cache.Graphics.DrawImage(image.NativeImage, new Point[] { imgRect.Location, new Point(imgRect.Right, imgRect.Top), new Point(imgRect.Left, imgRect.Bottom) }, new Rectangle(Point.Empty, imgActualSize), GraphicsUnit.Pixel, attributes);
			}
			finally {
				cache.ClipInfo.RestoreClip(oldClipState);
			}
		}
		void DrawFeedbackShape(GraphicsCache cache, Rectangle shapeBounds, Rectangle contentBounds, Color fillColor, Color outlineColor, DocumentLayoutUnitConverter unitConverter) {
			if (!DXColor.IsTransparentOrEmpty(fillColor))
				cache.FillRectangle(fillColor, contentBounds);
			if (initialShapeBounds != initialContentBounds) {
				Rectangle bounds;
				bounds = new Rectangle(shapeBounds.X, shapeBounds.Y, contentBounds.X - shapeBounds.X, shapeBounds.Height);
				cache.FillRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.X, shapeBounds.Y, contentBounds.Width, contentBounds.Y - shapeBounds.Y);
				cache.FillRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.Right, shapeBounds.Y, shapeBounds.Right - contentBounds.Right, shapeBounds.Height);
				cache.FillRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.X, contentBounds.Bottom, contentBounds.Width, shapeBounds.Bottom - contentBounds.Bottom);
				cache.FillRectangle(outlineColor, bounds);
			}
		}
	}
	public class WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy : DragFloatingObjectManuallyMouseHandlerStateStrategy {
		const float alpha = 0.6f;
		ShadowedFloatingObjectPainter painter;
		public WinFormsDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state)
			: base(state) {
		}
		public RichEditControl WinControl { get { return (RichEditControl)Control; } }
		protected internal override void ShowVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo, OfficeImage image) {
			if (image == null)
				return;
			this.painter.Image = image;
			this.painter.Bounds = bounds;
			this.painter.Transform = State.CreateVisualFeedbackTransform();
			WinControl.Painter.DeferredDraw(pageViewInfo, painter.Paint);
			WinControl.Refresh();
		}
		protected internal override void HideVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo) {
			WinControl.Invalidate();
		}
		protected internal override OfficeImage CreateFeedbackImage(OfficeImage originalImage) {
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			Size size = unitConverter.LayoutUnitsToPixels(InitialContentBounds.Size);
			using (Bitmap bmp = new Bitmap(Math.Max(size.Width, 1), Math.Max(size.Height, 1))) {
				using (Graphics gr = Graphics.FromImage(bmp)) {
					gr.Clear(Color.Transparent);
					if (originalImage != null)
						gr.DrawImage(originalImage.NativeImage, new Rectangle(Point.Empty, size));
				}
				return CreateSemitransparentFeedbackImage(bmp, alpha);
			}
		}
		protected internal virtual OfficeImage CreateSemitransparentFeedbackImage(Bitmap bitmap, float alpha) {
			Bitmap bmp = new Bitmap(bitmap.Width, bitmap.Height);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				gr.Clear(Color.Transparent);
				ColorMatrix matrix = new ColorMatrix();
				matrix.Matrix33 = alpha;
				ImageAttributes attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				gr.DrawImage(bitmap, new Point[] { Point.Empty, new Point(bitmap.Width, 0), new Point(0, bitmap.Height) }, new Rectangle(Point.Empty, bitmap.Size), GraphicsUnit.Pixel, attributes);
			}
			return OfficeImage.CreateImage(bmp);
		}
		protected internal override void BeginVisualFeedback() {
			this.painter = new ShadowedFloatingObjectPainter(DocumentModel.LayoutUnitConverter);
			this.painter.InitialShapeBounds = InitialShapeBounds;
			this.painter.InitialContentBounds = InitialContentBounds;
			Color fillColor = Run.Shape.FillColor;
			Color outlineColor = Run.Shape.OutlineColor;
			this.painter.FillColor = Color.FromArgb((int)(alpha * fillColor.A), fillColor);
			this.painter.OutlineColor = Color.FromArgb((int)(alpha * outlineColor.A), outlineColor);
		}
		protected internal override void EndVisualFeedback() {
			if (this.painter != null && this.painter.Image != null) {
				this.painter.Image.Dispose();
				this.painter.Image = null;
			}
			this.painter = null;
		}
	}
	public class WinFormsResizeTableRowMouseHandlerStateStrategy : ResizeTableRowMouseHandlerStateStrategy {
		public WinFormsResizeTableRowMouseHandlerStateStrategy(ResizeTableRowMouseHandlerState state)
			: base(state) {
		}
		public RichEditControl WinControl { get { return (RichEditControl)Control; } }
		protected internal override void DrawReversibleLineCore(int y) {
			WinControl.Painter.DrawReversibleHorizontalLine(y, PageViewInfo);
		}
		protected internal override void BeginVisualFeedback() {
			State.DrawReversibleLineCore();
		}
		protected internal override void ShowVisualFeedback() {
			State.DrawReversibleLineCore();
		}
		protected internal override void EndVisualFeedback() {
		}
	}
	public class WinFormsResizeTableVirtualColumnMouseHandlerStateStrategy : ResizeTableVirtualColumnMouseHandlerStateStrategy {
		public WinFormsResizeTableVirtualColumnMouseHandlerStateStrategy(ResizeTableVirtualColumnMouseHandlerState state)
			: base(state) {
		}
		public RichEditControl WinControl { get { return (RichEditControl)Control; } }
		protected internal override void DrawReversibleLineCore(int x) {
			WinControl.Painter.DrawReversibleVerticalLine(x, PageViewInfo);
		}
		protected internal override void BeginVisualFeedback() {
			State.DrawReversibleLineCore();
		}
		protected internal override void EndVisualFeedback() {
		}		
		protected internal override void ShowVisualFeedback() {
			State.DrawReversibleLineCore();
		}
		protected internal override void HideVisualFeedback() {
			State.DrawReversibleLineCore();
		}
	}
	public class WinFormsDragContentMouseHandlerStateBaseStrategy : DragContentMouseHandlerStateBaseStrategy {
		public WinFormsDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state)
			: base(state) {
		}
		public override void Finish() {
			RichEditControl control = (RichEditControl)Control;
			control.Invalidate();
		}
		protected internal override DragCaretVisualizer CreateCaretVisualizer() {
			return new WinFormsDragCaretVisualizer((RichEditControl)Control);
		}
	}
	public class WinFormsRichEditRectangularObjectResizeMouseHandlerStateStrategy : RichEditRectangularObjectResizeMouseHandlerStateStrategy {
		const float alpha = 0.6f;
		ShadowedFloatingObjectPainter painter;
		bool useReversibleFrame;
		public WinFormsRichEditRectangularObjectResizeMouseHandlerStateStrategy(RichEditRectangularObjectResizeMouseHandlerState state)
			: base(state) {
		}
		public RichEditControl WinControl { get { return (RichEditControl)Control; } }
		protected internal override void BeginVisualFeedback() {
			this.useReversibleFrame = !TryFloatingObjectVisualFeedback();
			if (this.useReversibleFrame)
				this.useReversibleFrame = !TryInlinePictureVisualFeedback();
			if (useReversibleFrame)
				DrawReversibleFrameCore();
		}
		bool TryFloatingObjectVisualFeedback() {
			FloatingObjectBox box = this.State.HotZone.Box as FloatingObjectBox;
			if (box == null)
				return false;
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			this.painter = new ShadowedFloatingObjectPainter(State.DocumentModel.LayoutUnitConverter);
			this.painter.Alpha = alpha;
			this.painter.InitialShapeBounds = box.Bounds;
			this.painter.InitialContentBounds = box.ContentBounds;
			Color fillColor = run.Shape.FillColor;
			Color outlineColor = run.Shape.OutlineColor;
			this.painter.FillColor = Color.FromArgb((int)(alpha * fillColor.A), fillColor);
			this.painter.OutlineColor = Color.FromArgb((int)(alpha * outlineColor.A), outlineColor);
			PictureFloatingObjectContent content = run.Content as PictureFloatingObjectContent;
			if (content != null)
				this.painter.Image = content.Image;
			this.painter.Transform = State.CreateVisualFeedbackTransform();
			return true;
		}
		bool TryInlinePictureVisualFeedback() {
			InlinePictureBox box = this.State.HotZone.Box as InlinePictureBox;
			if (box == null)
				return false;
			this.painter = new ShadowedFloatingObjectPainter(State.DocumentModel.LayoutUnitConverter);
			this.painter.Alpha = alpha;
			this.painter.InitialShapeBounds = box.Bounds;
			this.painter.InitialContentBounds = box.Bounds;
			PieceTable pieceTable = State.DocumentModel.ActivePieceTable;
			this.painter.Image = box.GetImage(pieceTable, WinControl.ReadOnly);
			this.painter.SizeMode = box.GetSizing(pieceTable);
			this.painter.ShadowDisplayMode = box.GetResizingShadowDisplayMode(pieceTable);
			return true;
		}
		protected internal override void ShowVisualFeedback() {
			if (useReversibleFrame)
				DrawReversibleFrameCore();
			else
				DrawVisualFeedback();
		}
		protected internal override void HideVisualFeedback() {
			if (useReversibleFrame) {
			}
			else
				WinControl.Invalidate();
		}
		protected internal override void EndVisualFeedback() {
			WinControl.Invalidate();
			this.painter = null;
		}
		void DrawReversibleFrameCore() {
			WinControl.Painter.DeferredDrawReversibleFrame(ObjectActualBounds, PageViewInfo);
			WinControl.Refresh();
		}
		void DrawVisualFeedback() {
			this.painter.Bounds = State.CalculateBoxBounds();
			this.painter.Transform = State.CreateVisualFeedbackTransform();
			WinControl.Painter.DeferredDraw(PageViewInfo, painter.Paint);
			WinControl.Refresh();
		}
	}
}
