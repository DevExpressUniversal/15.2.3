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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Adorners;
using DevExpress.XtraDiagram.Animations;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Paint {
	public abstract class DiagramAdornerObjectInfoArgsBase : ObjectInfoArgs {
		DiagramElementBounds elementBounds;
		DiagramAppearanceObject paintAppearance;
		public DiagramAdornerObjectInfoArgsBase()
			: this(null) {
		}
		public DiagramAdornerObjectInfoArgsBase(GraphicsCache cache) {
			this.Cache = cache;
		}
		public virtual void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			this.paintAppearance = viewInfo.ShapePaintAppearance;
			this.elementBounds = adorner.Bounds;
		}
		public virtual void Clear() {
			this.paintAppearance = null;
		}
		public DiagramAppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public override sealed Rectangle Bounds {
			get { return DisplayBounds; }
			set { this.elementBounds = new DiagramElementBounds(ElementBounds.LogicalBounds, value); }
		}
		public DiagramElementBounds ElementBounds { get { return elementBounds; } }
		public Rectangle LogicalBounds { get { return ElementBounds.LogicalBounds; } }
		public Rectangle DisplayBounds { get { return ElementBounds.DisplayBounds; } }
	}
	public abstract class DiagramAdornerPainterBase : ObjectPainter {
		public DiagramAdornerPainterBase() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
		}
		protected void ClearRect(ObjectInfoArgs e, Rectangle rect) {
			DrawUtils.ClearRect(e, rect);
		}
		protected void ClearCircle(ObjectInfoArgs e, Rectangle rect) {
			DrawUtils.ClearCircle(e, rect);
		}
	}
	#region Selection
	public class DiagramSelectionAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		DiagramItemSelection selection;
		Color borderColor, sizeGripColor, sizeGripCornerColor;
		bool drawSizeGrips;
		bool drawRotationGlyph;
		Image rotateImage;
		DiagramItemFadeOutAnimation fadeOutAnimation;
		DiagramItemSelectionBoundsAnimation boundsAnimation;
		public DiagramSelectionAdornerObjectInfoArgs() {
			this.fadeOutAnimation = null;
			this.boundsAnimation = null;
			this.rotateImage = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramSelectionAdorner selectionAdorner = (DiagramSelectionAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.selection = selectionAdorner.Selection;
			this.drawSizeGrips = selectionAdorner.CanResize;
			this.drawRotationGlyph = selectionAdorner.CanRotate;
			this.borderColor = appearances.GetSelectionBorderColor();
			this.sizeGripColor = appearances.GetSelectionSizeGripColor();
			this.sizeGripCornerColor = appearances.GetSelectionSizeGripCornerColor();
			this.fadeOutAnimation = viewInfo.AnimationController.ItemFadeOutAnimation;
			this.boundsAnimation = viewInfo.AnimationController.ItemSelectionBoundsAnimation;
			this.rotateImage = viewInfo.RotationImage;
		}
		public override void Clear() {
			base.Clear();
			this.selection = null;
			this.fadeOutAnimation = null;
			this.boundsAnimation = null;
			this.rotateImage = null;
		}
		public bool DrawSizeGrips { get { return drawSizeGrips; } }
		public bool DrawRotationGlyph { get { return drawRotationGlyph; } }
		public Color BorderColor { get { return borderColor; } }
		public Color SizeGripColor { get { return sizeGripColor; } }
		public Color SizeGripCornerColor { get { return sizeGripCornerColor; } }
		public DiagramItemSelection Selection { get { return selection; } }
		public Image RotateImage { get { return rotateImage; } }
		public bool InFadeOutAnimation { get { return this.fadeOutAnimation != null; } }
		public bool InBoundsAnimation { get { return this.boundsAnimation != null; } }
		public DiagramItemFadeOutAnimation FadeOutAnimation { get { return fadeOutAnimation; } }
		public DiagramItemSelectionBoundsAnimation BoundsAnimation { get { return boundsAnimation; } }
	}
	public class DiagramSelectionAdornerPainter : DiagramAdornerPainterBase {
		public DiagramSelectionAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			base.DrawObject(e);
			DrawSelectionBorder(e);
			if(drawArgs.DrawRotationGlyph) DrawRotationGlyph(e);
			if(drawArgs.DrawSizeGrips) DrawSizeGrips(e);
		}
		protected virtual void DrawSelectionBorder(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			e.Graphics.DrawRectangle(drawArgs.Cache.GetPen(GetBorderColor(e), GetBorderPenWidth(e)), GetBorderDisplayBounds(drawArgs));
		}
		protected virtual int GetBorderPenWidth(ObjectInfoArgs e) {
			return 1;
		}
		protected Rectangle GetBorderDisplayBounds(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			if(drawArgs.InBoundsAnimation) {
				return drawArgs.BoundsAnimation.Bounds;
			}
			return drawArgs.DisplayBounds;
		}
		protected virtual void DrawRotationGlyph(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			e.Graphics.DrawImage(drawArgs.RotateImage, drawArgs.Selection.RotationGripBounds.DisplayBounds);
		}
		protected virtual void DrawSizeGrips(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			foreach(Rectangle rect in GetVisibleRects(drawArgs)) {
				DrawUtils.DrawSizeGrip(e, rect, GetSizeGripColor(e), GetSizeGripCornerColor(e));
			}
		}
		protected Color GetBorderColor(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			if(drawArgs.InFadeOutAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.BorderColor);
			}
			return drawArgs.BorderColor;
		}
		protected IEnumerable<Rectangle> GetVisibleRects(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			if(drawArgs.InBoundsAnimation) {
				return drawArgs.BoundsAnimation.Selection.DisplayRects;
			}
			return drawArgs.Selection.DisplayRects;
		}
		protected Color GetSizeGripColor(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			if(drawArgs.InFadeOutAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.SizeGripColor);
			}
			return drawArgs.SizeGripColor;
		}
		protected Color GetSizeGripCornerColor(ObjectInfoArgs e) {
			DiagramSelectionAdornerObjectInfoArgs drawArgs = (DiagramSelectionAdornerObjectInfoArgs)e;
			if(drawArgs.InFadeOutAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.SizeGripCornerColor);
			}
			return drawArgs.SizeGripCornerColor;
		}
	}
	#endregion
	#region ItemDrag Adorner
	public class DiagramItemDragPreviewAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		bool isShapeAdorner;
		Color borderColor;
		DiagramItemInfo itemInfo;
		DiagramShapeDragPreviewPainter shapePainter;
		public DiagramItemDragPreviewAdornerObjectInfoArgs() {
			this.isShapeAdorner = false;
			this.itemInfo = null;
			this.shapePainter = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramItemDragPreviewAdorner dragAdorner = (DiagramItemDragPreviewAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			DiagramItem item = dragAdorner.Item;
			this.borderColor = appearances.GetItemDragPreviewBorderColor();
			this.shapePainter = viewInfo.DragPreviewShapePainter;
			this.itemInfo = viewInfo.GetItemInfo(item);
			if(this.itemInfo == null && item is DiagramShape) {
				this.itemInfo = new DiagramShapeInfo((DiagramShape)item);
				this.itemInfo.PaintAppearance = viewInfo.ShapePaintAppearance;
			}
			this.isShapeAdorner = dragAdorner.IsShapeAdorner;
		}
		public override void Clear() {
			base.Clear();
			this.shapePainter = null;
			this.itemInfo = null;
		}
		public Rectangle GetItemRect() {
			return RectangleUtils.FitRect(LogicalBounds, ItemInfo.Item.Size);
		}
		public Color BorderColor { get { return borderColor; } }
		public bool IsShapeAdorner { get { return isShapeAdorner; } }
		public DiagramItemInfo ItemInfo { get { return itemInfo; } }
		public DiagramShapePainter ShapePainter { get { return shapePainter; } }
		public DiagramShapeInfo ShapeInfo { get { return ItemInfo as DiagramShapeInfo; } }
	}
	public class DiagramItemDragPreviewAdornerPainter : DiagramAdornerPainterBase {
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramItemDragPreviewAdornerObjectInfoArgs drawArgs = (DiagramItemDragPreviewAdornerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			if(drawArgs.IsShapeAdorner) DrawShapePreview(drawArgs);
			DrawDragBorder(drawArgs);
		}
		protected virtual void DrawDragBorder(DiagramItemDragPreviewAdornerObjectInfoArgs drawArgs) {
			using(Pen borderPen = GetBorderPen(drawArgs)) {
				drawArgs.Graphics.DrawRectangle(borderPen, drawArgs.LogicalBounds);
			}
		}
		protected virtual Pen GetBorderPen(DiagramItemDragPreviewAdornerObjectInfoArgs drawArgs) {
			Pen borderPen = new Pen(drawArgs.BorderColor);
			borderPen.DashPattern = new float[] { 3, 3 };
			return borderPen;
		}
		protected virtual void DrawShapePreview(ObjectInfoArgs e) {
			DiagramItemDragPreviewAdornerObjectInfoArgs drawArgs = (DiagramItemDragPreviewAdornerObjectInfoArgs)e;
			ShapeDraw.DrawShape(e.Cache, drawArgs.ShapeInfo, drawArgs.ShapePainter, drawArgs.GetItemRect());
		}
	}
	#endregion
	#region Ruler Shadow
	public abstract class DiagramRulerShadowAdornerObjectInfoArgsBase : DiagramAdornerObjectInfoArgsBase {
		Color shadowColor;
		Rectangle shadowRect;
		public DiagramRulerShadowAdornerObjectInfoArgsBase() {
			this.shadowRect = Rectangle.Empty;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramRulerShadowAdornerBase rulerShadowAdorner = (DiagramRulerShadowAdornerBase)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.shadowColor = appearances.GetRulerShadowColor();
		}
		protected void SetShadowRect(Rectangle shadowRect) {
			this.shadowRect = shadowRect;
		}
		public Color ShadowColor { get { return shadowColor; } }
		public Rectangle ShadowRect { get { return shadowRect; } }
	}
	public class DiagramHRulerShadowAdornerObjectInfoArgs : DiagramRulerShadowAdornerObjectInfoArgsBase {
		public DiagramHRulerShadowAdornerObjectInfoArgs() {
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			base.Initialize(viewInfo, appearances, adorner);
			SetShadowRect(viewInfo.CalcHRulerShadowRect(adorner.DisplayBounds));
		}
	}
	public class DiagramVRulerShadowAdornerObjectInfoArgs : DiagramRulerShadowAdornerObjectInfoArgsBase {
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			base.Initialize(viewInfo, appearances, adorner);
			SetShadowRect(viewInfo.CalcVRulerShadowRect(adorner.DisplayBounds));
		}
	}
	public abstract class DiagramRulerShadowAdornerPainterBase : DiagramAdornerPainterBase {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawShadow(e);
		}
		protected void DrawShadow(ObjectInfoArgs e) {
			using(Pen shadowPen = GetShadowPen(e)) {
				DrawStartLine(shadowPen, e);
				DrawStopShadowLine(shadowPen, e);
				DrawMedian(shadowPen, e);
			}
		}
		protected void DrawStartLine(Pen shadowPen, ObjectInfoArgs e) {
			DiagramRulerShadowAdornerObjectInfoArgsBase drawArgs = (DiagramRulerShadowAdornerObjectInfoArgsBase)e;
			DrawLineCore(shadowPen, e, GetStartLine(drawArgs.ShadowRect));
		}
		protected void DrawStopShadowLine(Pen shadowPen, ObjectInfoArgs e) {
			DiagramRulerShadowAdornerObjectInfoArgsBase drawArgs = (DiagramRulerShadowAdornerObjectInfoArgsBase)e;
			DrawLineCore(shadowPen, e, GetStopLine(drawArgs.ShadowRect));
		}
		protected void DrawMedian(Pen shadowPen, ObjectInfoArgs e) {
			DiagramRulerShadowAdornerObjectInfoArgsBase drawArgs = (DiagramRulerShadowAdornerObjectInfoArgsBase)e;
			DrawLineCore(shadowPen, e, GetMedian(drawArgs.ShadowRect));
		}
		protected abstract PointPair GetStartLine(Rectangle rect);
		protected abstract PointPair GetStopLine(Rectangle rect);
		protected abstract PointPair GetMedian(Rectangle rect);
		protected void DrawLineCore(Pen shadowPen, ObjectInfoArgs e, PointPair line) {
			e.Graphics.DrawLine(shadowPen, line.Start, line.End);
		}
		protected Pen GetShadowPen(ObjectInfoArgs e) {
			DiagramRulerShadowAdornerObjectInfoArgsBase drawArgs = (DiagramRulerShadowAdornerObjectInfoArgsBase)e;
			Pen shadowPen = new Pen(drawArgs.ShadowColor);
			shadowPen.DashPattern = new float[] { 2, 2 };
			return shadowPen;
		}
	}
	public class DiagramHRulerShadowAdornerPainter : DiagramRulerShadowAdornerPainterBase {
		protected override PointPair GetStartLine(Rectangle rect) {
			return new PointPair(rect.Location, PointUtils.ApplyOffset(rect.Location, 0, rect.Height));
		}
		protected override PointPair GetMedian(Rectangle rect) {
			Point point = PointUtils.ApplyOffset(rect.Location, rect.Width / 2, 0);
			return new PointPair(point, PointUtils.ApplyOffset(point, 0, rect.Height));
		}
		protected override PointPair GetStopLine(Rectangle rect) {
			Point point = PointUtils.ApplyOffset(rect.Location, rect.Width, 0);
			return new PointPair(point, PointUtils.ApplyOffset(point, 0, rect.Height));
		}
	}
	public class DiagramVRulerShadowAdornerPainter : DiagramRulerShadowAdornerPainterBase {
		protected override PointPair GetStartLine(Rectangle rect) {
			return new PointPair(rect.Location, PointUtils.ApplyOffset(rect.Location, rect.Width, 0));
		}
		protected override PointPair GetMedian(Rectangle rect) {
			Point point = PointUtils.ApplyOffset(rect.Location, 0, rect.Height / 2);
			return new PointPair(point, PointUtils.ApplyOffset(point, rect.Width, 0));
		}
		protected override PointPair GetStopLine(Rectangle rect) {
			Point point = PointUtils.ApplyOffset(rect.Location, 0, rect.Height);
			return new PointPair(point, PointUtils.ApplyOffset(point, rect.Width, 0));
		}
	}
	#endregion
	#region Bounds Snap Lines
	public abstract class DiagramBoundsSnapLineAdornerObjectInfoArgsBase : DiagramAdornerObjectInfoArgsBase {
		Color snapLineColor;
		Point snapLineStartPos;
		Point snapLineEndPos;
		public DiagramBoundsSnapLineAdornerObjectInfoArgsBase() {
			this.snapLineStartPos = this.snapLineEndPos = Point.Empty;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			base.Initialize(viewInfo, appearances, adorner);
			this.snapLineStartPos = GetSnapLineStartPos(viewInfo, adorner.DisplayBounds);
			this.snapLineEndPos = GetSnapLineEndPos(viewInfo, adorner.DisplayBounds);
			this.snapLineColor = appearances.GetSnapLineColor();
		}
		public Color SnapLineColor { get { return snapLineColor; } }
		public Point SnapLineStartPos { get { return snapLineStartPos; } }
		public Point SnapLineEndPos { get { return snapLineEndPos; } }
		public virtual int SnapLineThickness { get { return 1; } }
		protected abstract Point GetSnapLineStartPos(DiagramControlViewInfo viewInfo, Rectangle displayBounds);
		protected abstract Point GetSnapLineEndPos(DiagramControlViewInfo viewInfo, Rectangle displayBounds);
	}
	public class DiagramHBoundsSnapLineAdornerObjectInfoArgs : DiagramBoundsSnapLineAdornerObjectInfoArgsBase {
		public DiagramHBoundsSnapLineAdornerObjectInfoArgs() {
		}
		protected override Point GetSnapLineEndPos(DiagramControlViewInfo viewInfo, Rectangle displayBounds) {
			return viewInfo.CalcHBoundsSnapLineStartPos(displayBounds);
		}
		protected override Point GetSnapLineStartPos(DiagramControlViewInfo viewInfo, Rectangle displayBounds) {
			return viewInfo.CalcHBoundsSnapLineEndPos(displayBounds);
		}
	}
	public class DiagramVBoundsSnapLineAdornerObjectInfoArgs : DiagramBoundsSnapLineAdornerObjectInfoArgsBase {
		public DiagramVBoundsSnapLineAdornerObjectInfoArgs() {
		}
		protected override Point GetSnapLineEndPos(DiagramControlViewInfo viewInfo, Rectangle displayBounds) {
			return viewInfo.CalcVBoundsSnapLineStartPos(displayBounds);
		}
		protected override Point GetSnapLineStartPos(DiagramControlViewInfo viewInfo, Rectangle displayBounds) {
			return viewInfo.CalcVBoundsSnapLineEndPos(displayBounds);
		}
	}
	public class DiagramBoundsSnapLineAdornerPainter : DiagramAdornerPainterBase {
		public DiagramBoundsSnapLineAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawSnapLine(e);
		}
		protected virtual void DrawSnapLine(ObjectInfoArgs e) {
			DiagramBoundsSnapLineAdornerObjectInfoArgsBase drawArgs = (DiagramBoundsSnapLineAdornerObjectInfoArgsBase)e;
			using(Pen pen = new Pen(drawArgs.SnapLineColor, drawArgs.SnapLineThickness)) {
				pen.DashStyle = DashStyle.Dash;
				pen.DashPattern = new float[] { 5f, 2f };
				e.Graphics.DrawLine(pen, drawArgs.SnapLineStartPos, drawArgs.SnapLineEndPos);
			}
		}
	}
	#endregion
	#region Size Snap Lines
	public abstract class DiagramSizeSnapLineAdornerObjectInfoArgsBase : DiagramAdornerObjectInfoArgsBase {
		Color snapLineColor;
		PointPair snapLine;
		PointPair[] delimiters;
		SizeSnapLine sizeSnapLine;
		DiagramSizeSnapLineAnimationBase fadeAnimation;
		public DiagramSizeSnapLineAdornerObjectInfoArgsBase() {
			this.snapLine = null;
			this.delimiters = null;
			this.fadeAnimation = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramSizeSnapLineAdornerBase snapLineAdorner = (DiagramSizeSnapLineAdornerBase)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.snapLine = new PointPair(GetSnapLineStartPos(viewInfo, adorner), GetSnapLineEndPos(viewInfo, adorner));
			this.delimiters = GetDelimiters(SnapLine, viewInfo, adorner);
			this.sizeSnapLine = snapLineAdorner.SnapLine;
			this.fadeAnimation = GetSnapLineAnimation(viewInfo);
			this.snapLineColor = appearances.GetSnapLineColor();
		}
		public override void Clear() {
			base.Clear();
			this.fadeAnimation = null;
		}
		public Color SnapLineColor { get { return snapLineColor; } }
		public PointPair SnapLine { get { return snapLine; } }
		public PointPair[] Delimiters { get { return delimiters; } }
		public SizeSnapLine SizeSnapLine { get { return sizeSnapLine; } }
		public DiagramSizeSnapLineAnimationBase FadeAnimation { get { return fadeAnimation; } }
		public bool InFadeAnimation { get { return FadeAnimation != null; } }
		protected abstract DiagramSizeSnapLineAnimationBase GetSnapLineAnimation(DiagramControlViewInfo viewInfo);
		protected abstract Point GetSnapLineStartPos(DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner);
		protected abstract Point GetSnapLineEndPos(DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner);
		protected abstract PointPair[] GetDelimiters(PointPair snapLine, DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner);
	}
	public class DiagramHSizeSnapLineAdornerObjectInfoArgs : DiagramSizeSnapLineAdornerObjectInfoArgsBase {
		public DiagramHSizeSnapLineAdornerObjectInfoArgs() {
		}
		protected override DiagramSizeSnapLineAnimationBase GetSnapLineAnimation(DiagramControlViewInfo viewInfo) {
			return viewInfo.AnimationController.GetHSizeSnapLineAnimation();
		}
		protected override Point GetSnapLineStartPos(DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner) {
			return viewInfo.CalcHSizeSnapLineStartPos(adorner);
		}
		protected override Point GetSnapLineEndPos(DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner) {
			return viewInfo.CalcHSizeSnapLineEndPos(adorner);
		}
		protected override PointPair[] GetDelimiters(PointPair snapLine, DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner) {
			return viewInfo.CalcHSizeSnapLineDelimiters(snapLine, adorner);
		}
		public virtual int SnapLineThickness { get { return 1; } }
	}
	public class DiagramVSizeSnapLineAdornerObjectInfoArgs : DiagramSizeSnapLineAdornerObjectInfoArgsBase {
		public DiagramVSizeSnapLineAdornerObjectInfoArgs() {
		}
		protected override DiagramSizeSnapLineAnimationBase GetSnapLineAnimation(DiagramControlViewInfo viewInfo) {
			return viewInfo.AnimationController.GetVSizeSnapLineAnimation();
		}
		protected override Point GetSnapLineStartPos(DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner) {
			return viewInfo.CalcVSizeSnapLineStartPos(adorner);
		}
		protected override Point GetSnapLineEndPos(DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner) {
			return viewInfo.CalcVSizeSnapLineEndPos(adorner);
		}
		protected override PointPair[] GetDelimiters(PointPair snapLine, DiagramControlViewInfo viewInfo, DiagramAdornerBase adorner) {
			return viewInfo.CalcVSizeSnapLineDelimiters(snapLine, adorner);
		}
	}
	public abstract class DiagramSizeSnapLineAdornerPainterBase : DiagramAdornerPainterBase {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawSnapLine(e);
		}
		public virtual void DrawSnapLine(ObjectInfoArgs e) {
			DiagramSizeSnapLineAdornerObjectInfoArgsBase drawArgs = (DiagramSizeSnapLineAdornerObjectInfoArgsBase)e;
			using(Pen pen = CreateSnapLinePen(drawArgs)) {
				DrawLine(e, pen, GetSnapLinePoints(e));
			}
			Array.ForEach(drawArgs.Delimiters, delimiter => e.Graphics.DrawLine(e.Cache.GetPen(GetSnapLineColor(e)), delimiter.Start, delimiter.End));
		}
		protected void DrawLine(ObjectInfoArgs e, Pen pen, PointPair pair) {
			e.Graphics.DrawLine(pen, pair.Start, pair.Second);
		}
		protected abstract PointPair GetSnapLinePoints(ObjectInfoArgs e);
		protected Pen CreateSnapLinePen(ObjectInfoArgs e) {
			Pen snapLinePen = new Pen(GetSnapLineColor(e));
			snapLinePen.CustomStartCap = snapLinePen.CustomEndCap = new AdjustableArrowCap(3, 4);
			return snapLinePen;
		}
		protected Color GetSnapLineColor(ObjectInfoArgs e) {
			DiagramSizeSnapLineAdornerObjectInfoArgsBase drawArgs = (DiagramSizeSnapLineAdornerObjectInfoArgsBase)e;
			if(drawArgs.InFadeAnimation) {
				return drawArgs.FadeAnimation.GetColor(drawArgs.SnapLineColor);
			}
			return drawArgs.SnapLineColor;
		}
	}
	public class DiagramHSizeSnapLineAdornerPainter : DiagramSizeSnapLineAdornerPainterBase {
		protected override PointPair GetSnapLinePoints(ObjectInfoArgs e) {
			DiagramSizeSnapLineAdornerObjectInfoArgsBase drawArgs = (DiagramSizeSnapLineAdornerObjectInfoArgsBase)e;
			if(drawArgs.InFadeAnimation) {
				return drawArgs.SnapLine.SetHorz(drawArgs.FadeAnimation.LenghtValue);
			}
			return drawArgs.SnapLine;
		}
	}
	public class DiagramVSizeSnapLineAdornerPainter : DiagramSizeSnapLineAdornerPainterBase {
		protected override PointPair GetSnapLinePoints(ObjectInfoArgs e) {
			DiagramSizeSnapLineAdornerObjectInfoArgsBase drawArgs = (DiagramSizeSnapLineAdornerObjectInfoArgsBase)e;
			if(drawArgs.InFadeAnimation) {
				return drawArgs.SnapLine.SetVert(drawArgs.FadeAnimation.LenghtValue);
			}
			return drawArgs.SnapLine;
		}
	}
	#endregion
	#region Selection Preview
	public class DiagramSelectionPreviewAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		Color borderColor, backColor;
		DiagramSelectionPreviewFadeOutAnimation fadeOutAnimation;
		public DiagramSelectionPreviewAdornerObjectInfoArgs() {
			this.fadeOutAnimation = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			base.Initialize(viewInfo, appearances, adorner);
			this.fadeOutAnimation = viewInfo.AnimationController.SelectionPreviewFadeOutAnimation;
			this.borderColor = appearances.GetSelectionPreviewBorderColor();
			this.backColor = appearances.GetSelectionPreviewBackColor();
		}
		public override void Clear() {
			base.Clear();
			fadeOutAnimation = null;
		}
		public Color BorderColor { get { return borderColor; } }
		public Color BackColor { get { return backColor; } }
		public bool InAnimation { get { return FadeOutAnimation != null; } }
		public DiagramSelectionPreviewFadeOutAnimation FadeOutAnimation { get { return fadeOutAnimation; } }
	}
	public class DiagramSelectionPreviewAdornerPainter : DiagramAdornerPainterBase {
		public DiagramSelectionPreviewAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawSelection(e);
		}
		protected virtual void DrawSelection(ObjectInfoArgs e) {
			DiagramSelectionPreviewAdornerObjectInfoArgs drawArgs = (DiagramSelectionPreviewAdornerObjectInfoArgs)e;
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(GetBackgroundColor(e)), drawArgs.DisplayBounds);
			e.Graphics.DrawRectangle(e.Cache.GetPen(GetBorderColor(e)), drawArgs.DisplayBounds);
		}
		protected Color GetBorderColor(ObjectInfoArgs e) {
			DiagramSelectionPreviewAdornerObjectInfoArgs drawArgs = (DiagramSelectionPreviewAdornerObjectInfoArgs)e;
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.BorderColor);
			}
			return drawArgs.BorderColor;
		}
		protected Color GetBackgroundColor(ObjectInfoArgs e) {
			DiagramSelectionPreviewAdornerObjectInfoArgs drawArgs = (DiagramSelectionPreviewAdornerObjectInfoArgs)e;
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.BackColor);
			}
			return drawArgs.BackColor;
		}
	}
	#endregion
	#region Selection Parts
	public class DiagramSelectionPartAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		Color selectionColor;
		DiagramItem item;
		bool isPrimarySelection;
		public DiagramSelectionPartAdornerObjectInfoArgs() {
			this.item = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramSelectionPartAdorner selectionAdorner = (DiagramSelectionPartAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.item = (DiagramItem)selectionAdorner.Item;
			this.isPrimarySelection = selectionAdorner.IsPrimarySelection;
			this.selectionColor = appearances.GetSelectionPartBackColor();
		}
		public override void Clear() {
			base.Clear();
			this.item = null;
		}
		public Color SelectionColor { get { return selectionColor; } }
		public DiagramItem Item { get { return item; } }
		public bool IsPrimarySelection { get { return isPrimarySelection; } }
		public virtual int SelectionBorderThickness { get { return 2; } }
		public virtual int PrimarySelectionBorderThickness { get { return 4; } }
	}
	public class DiagramSelectionPartAdornerPainter : DiagramAdornerPainterBase {
		public DiagramSelectionPartAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawSelectionPart(e);
		}
		protected virtual void DrawSelectionPart(ObjectInfoArgs e) {
			DiagramSelectionPartAdornerObjectInfoArgs drawArgs = (DiagramSelectionPartAdornerObjectInfoArgs)e;
			e.Graphics.DrawRectangle(e.Cache.GetPen(drawArgs.SelectionColor, GetPenWidth(drawArgs)), drawArgs.DisplayBounds);
		}
		protected int GetPenWidth(DiagramSelectionPartAdornerObjectInfoArgs drawArgs) {
			return drawArgs.IsPrimarySelection ? 4 : 2;
		}
	}
	#endregion
	#region Shape Parameters
	public class DiagramShapeParametersAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		Color borderColor, backColor;
		DiagramShape shape;
		Size gripSize;
		ConfigurableArea points;
		public DiagramShapeParametersAdornerObjectInfoArgs() {
			this.shape = null;
			this.gripSize = Size.Empty;
			this.points = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramShapeParametersAdorner shapeAdorner = (DiagramShapeParametersAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.shape = shapeAdorner.Shape;
			this.gripSize = shapeAdorner.GripSize;
			this.points = shapeAdorner.Points;
			this.borderColor = appearances.GetShapeParameterBorderColor();
			this.backColor = appearances.GetShapeParameterBackColor();
		}
		public override void Clear() {
			base.Clear();
			this.shape = null;
			this.points = null;
		}
		public Color BorderColor { get { return borderColor; } }
		public Color BackColor { get { return backColor; } }
		public Size GripSize { get { return gripSize; } }
		public DiagramShape Shape { get { return shape; } }
		public ConfigurableArea Points { get { return points; } }
	}
	public class DiagramShapeParametersAdornerPainter : DiagramAdornerPainterBase {
		public DiagramShapeParametersAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramShapeParametersAdornerObjectInfoArgs drawArgs = (DiagramShapeParametersAdornerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			DrawShapeParameters(drawArgs);
		}
		protected virtual void DrawShapeParameters(DiagramShapeParametersAdornerObjectInfoArgs drawArgs) {
			Array.ForEach(drawArgs.Points.Parameters, parameter => DrawShapeParameters(drawArgs, parameter.Bounds));
		}
		protected virtual void DrawShapeParameters(DiagramShapeParametersAdornerObjectInfoArgs drawArgs, DiagramElementBounds bounds) {
			drawArgs.Graphics.FillRectangle(drawArgs.Cache.GetSolidBrush(drawArgs.BackColor), bounds.DisplayBounds);
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.BorderColor), bounds.DisplayBounds);
		}
	}
	#endregion
	#region Inplace Text
	public class DiagramInplaceEditorAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		Color borderColor;
		Color backColor;
		Color borderAlternativeColor;
		public DiagramInplaceEditorAdornerObjectInfoArgs() {
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			base.Initialize(viewInfo, appearances, adorner);
			this.borderColor = appearances.GetInplaceEditSurfaceBorderColor();
			this.backColor = appearances.GetInplaceEditSurfaceBackColor();
			this.borderAlternativeColor = appearances.GetInplaceEditSurfaceAlternativeBorderColor();
		}
		public Color BorderColor { get { return borderColor; } }
		public Color BackColor { get { return backColor; } }
		public Color BorderAlternativeColor { get { return borderAlternativeColor; } }
	}
	public class DiagramInplaceEditorAdornerPainter : DiagramAdornerPainterBase {
		public DiagramInplaceEditorAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramInplaceEditorAdornerObjectInfoArgs drawArgs = (DiagramInplaceEditorAdornerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			DrawEditSurface(drawArgs);
		}
		protected virtual void DrawEditSurface(DiagramInplaceEditorAdornerObjectInfoArgs drawArgs) {
			drawArgs.Graphics.FillRectangle(drawArgs.Cache.GetSolidBrush(drawArgs.BackColor), drawArgs.DisplayBounds);
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.BorderAlternativeColor), drawArgs.DisplayBounds);
			using(Pen pen = GetBorderPen(drawArgs)) {
				drawArgs.Graphics.DrawRectangle(pen, drawArgs.DisplayBounds);
			}
		}
		protected virtual Pen GetBorderPen(DiagramInplaceEditorAdornerObjectInfoArgs drawArgs) {
			Pen borderPen = new Pen(drawArgs.BorderColor);
			borderPen.DashPattern = new float[] { 2, 2 };
			return borderPen;
		}
	}
	#endregion
	#region Connector Selection
	public class DiagramConnectorSelectionAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		ConnectorSelectionColors selectionColors;
		ConnectorSelectionFreeBeginPointColors freeBeginPointColors;
		ConnectorSelectionFreeEndPointColors freeEndPointColors;
		ConnectorSelectionConnectedPointColors connectedBeginPointColors;
		ConnectorSelectionConnectedPointColors connectedEndPointColors;
		ConnectorSelectionIntermediatePointColors intermediatePointColors;
		bool beginPointConnected;
		bool endPointConnected;
		ConnectorSelection rects;
		DiagramConnectorSelectionBoundsAnimation boundsAnimation;
		public DiagramConnectorSelectionAdornerObjectInfoArgs() {
			this.beginPointConnected = this.endPointConnected = false;
			this.boundsAnimation = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramConnectorSelectionAdorner connectorAdorner = (DiagramConnectorSelectionAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.beginPointConnected = connectorAdorner.BeginPointConnected;
			this.endPointConnected = connectorAdorner.EndPointConnected;
			this.rects = connectorAdorner.Rects;
			this.boundsAnimation = viewInfo.AnimationController.ConnectorSelectionBoundsAnimation;
			this.selectionColors = appearances.GetConnectorSelectionColors();
			this.freeBeginPointColors = appearances.GetConnectorSelectionFreeBeginPointColors();
			this.freeEndPointColors = appearances.GetConnectorSelectionFreeEndPointColors();
			this.connectedBeginPointColors = appearances.GetConnectorSelectionConnectedBeginPointColors();
			this.connectedEndPointColors = appearances.GetConnectorSelectionConnectedEndPointColors();
			this.intermediatePointColors = appearances.GetConnectorSelectionIntermediatePointColors();
		}
		public override void Clear() {
			base.Clear();
			this.boundsAnimation = null;
		}
		public ConnectorSelectionColors SelectionColors { get { return selectionColors; } }
		public ConnectorSelectionFreeBeginPointColors FreeBeginPointColors { get { return freeBeginPointColors; } }
		public ConnectorSelectionFreeEndPointColors FreeEndPointColors { get { return freeEndPointColors; } }
		public ConnectorSelectionConnectedPointColors ConnectedBeginPointColors { get { return connectedBeginPointColors; } }
		public ConnectorSelectionConnectedPointColors ConnectedEndPointColors { get { return connectedEndPointColors; } }
		public ConnectorSelectionIntermediatePointColors IntermediatePointColors { get { return intermediatePointColors; } }
		public ConnectorSelection Rects { get { return rects; } }
		public bool BeginPointConnected { get { return beginPointConnected; } }
		public bool EndPointConnected { get { return endPointConnected; } }
		public bool InBoundsAnimation { get { return BoundsAnimation != null; } }
		public DiagramConnectorSelectionBoundsAnimation BoundsAnimation { get { return boundsAnimation; } }
	}
	public class DiagramConnectorSelectionAdornerPainter : DiagramAdornerPainterBase {
		public DiagramConnectorSelectionAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawSelection(e);
		}
		protected virtual void DrawSelection(ObjectInfoArgs e) {
			DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs = (DiagramConnectorSelectionAdornerObjectInfoArgs)e;
			if(drawArgs.InBoundsAnimation) DrawAnimatedBorder(drawArgs);
			DrawBeginPoint(drawArgs);
			DrawEndPoint(drawArgs);
			DrawIntermediatePoints(drawArgs);
		}
		protected virtual void DrawAnimatedBorder(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs) {
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.SelectionColors.Border), drawArgs.BoundsAnimation.Bounds);
			Array.ForEach(drawArgs.BoundsAnimation.Markers, marker => DrawUtils.DrawSizeGrip(drawArgs, marker, drawArgs.SelectionColors.SizeGrip, drawArgs.SelectionColors.GripCorner));
		}
		protected virtual void DrawBeginPoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs) {
			Rectangle displayRect = drawArgs.Rects.BeginPoint.DisplayBounds;
			if(drawArgs.BeginPointConnected) {
				DrawConnectedBeginPoint(drawArgs, displayRect);
			}
			else {
				DrawFreeBeginPoint(drawArgs, displayRect);
			}
		}
		protected virtual void DrawConnectedBeginPoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs, Rectangle displayRect) {
			drawArgs.Graphics.FillEllipse(drawArgs.Cache.GetSolidBrush(drawArgs.ConnectedBeginPointColors.Back), displayRect);
			drawArgs.Graphics.DrawEllipse(drawArgs.Cache.GetPen(drawArgs.ConnectedBeginPointColors.Border), displayRect);
			DrawUtils.DrawRadialCloudEffect(drawArgs, drawArgs.ConnectedBeginPointColors.Border, displayRect);
		}
		protected virtual void DrawFreeBeginPoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs, Rectangle displayRect) {
			ClearRect(drawArgs, displayRect);
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.FreeBeginPointColors.Border), displayRect);
			drawArgs.Graphics.DrawRectCorners(drawArgs.Cache.GetSolidBrush(drawArgs.FreeBeginPointColors.Corner), displayRect);
		}
		protected virtual void DrawEndPoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs) {
			Rectangle displayRect = drawArgs.Rects.EndPoint.DisplayBounds;
			if(drawArgs.EndPointConnected) {
				DrawConnectedEndPoint(drawArgs, displayRect);
			}
			else {
				DrawFreeEndPoint(drawArgs, displayRect);
			}
		}
		protected virtual void DrawConnectedEndPoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs, Rectangle baseRect) {
			Rectangle boundingRect = baseRect.Inflated(2);
			ClearCircle(drawArgs, boundingRect);
			drawArgs.Graphics.DrawEllipse(drawArgs.Cache.GetPen(drawArgs.ConnectedEndPointColors.Border), boundingRect);
			DrawUtils.DrawRadialCloudEffect(drawArgs, drawArgs.ConnectedEndPointColors.Border, boundingRect);
			DrawUtils.DrawRectangularCloudEffect(drawArgs, drawArgs.ConnectedEndPointColors.Back, baseRect);
		}
		protected virtual void DrawFreeEndPoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs, Rectangle displayRect) {
			drawArgs.Graphics.FillRectangle(drawArgs.Cache.GetSolidBrush(drawArgs.FreeEndPointColors.Back), displayRect);
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.FreeEndPointColors.Border), displayRect);
			drawArgs.Graphics.DrawRectCorners(drawArgs.Cache.GetSolidBrush(drawArgs.FreeEndPointColors.Corner), displayRect);
		}
		protected virtual void DrawIntermediatePoints(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs) {
			drawArgs.Rects.ForEachIntermediatePoint(rect => DrawIntermediatePoint(drawArgs, rect));
		}
		protected virtual void DrawIntermediatePoint(DiagramConnectorSelectionAdornerObjectInfoArgs drawArgs, DiagramElementBounds bounds) {
			DrawUtils.DrawRectangularCloudEffect(drawArgs, drawArgs.IntermediatePointColors.Back, bounds.DisplayBounds);
		}
	}
	#endregion
	#region Connector DragPreview
	public class DiagramConnectorDragPreviewAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		ShapeGeometry shape;
		DiagramConnectorDragPreviewPainter connectorPainter;
		DiagramControlViewInfo diagramViewInfo;
		public DiagramConnectorDragPreviewAdornerObjectInfoArgs() {
			this.shape = null;
			this.connectorPainter = null;
			this.diagramViewInfo = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramConnectorDragPreviewAdorner dragAdorner = (DiagramConnectorDragPreviewAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.connectorPainter = viewInfo.DragPreviewConnectorPainter;
			this.shape = dragAdorner.Shape;
			this.diagramViewInfo = viewInfo;
		}
		public override void Clear() {
			base.Clear();
			this.shape = null;
			this.connectorPainter = null;
			this.diagramViewInfo = null;
		}
		public ShapeGeometry Shape { get { return shape; } }
		public DiagramConnectorDragPreviewPainter ConnectorPainter { get { return connectorPainter; } }
		public DiagramControlViewInfo DiagramViewInfo { get { return diagramViewInfo; } }
	}
	public class DiagramConnectorDragPreviewAdornerPainter : DiagramAdornerPainterBase {
		public DiagramConnectorDragPreviewAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawConnectorPreview(e);
		}
		protected virtual void DrawConnectorPreview(ObjectInfoArgs e) {
			DiagramConnectorDragPreviewAdornerObjectInfoArgs drawArgs = (DiagramConnectorDragPreviewAdornerObjectInfoArgs)e;
			ShapeDraw.DrawConnector(e.Cache, drawArgs.Shape, drawArgs.ConnectorPainter, drawArgs.PaintAppearance, drawArgs.LogicalBounds, connectorDrawArgs => connectorDrawArgs.Initialize(drawArgs.DiagramViewInfo));
		}
	}
	#endregion
	#region ConnectorPoint Moving Preview
	public class DiagramConnectorPointDragPreviewAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		ShapeGeometry shape;
		DiagramConnectorPointDragPreviewPainter painter;
		DiagramControlViewInfo diagramViewInfo;
		public DiagramConnectorPointDragPreviewAdornerObjectInfoArgs() {
			this.shape = null;
			this.painter = null;
			this.diagramViewInfo = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramConnectorPointDragPreviewAdorner dragAdorner = (DiagramConnectorPointDragPreviewAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.shape = dragAdorner.Shape.Scale(viewInfo.ZoomFactor);
			this.painter = viewInfo.ConnectorPointDragPreviewPainter;
			this.diagramViewInfo = viewInfo;
		}
		public override void Clear() {
			base.Clear();
			this.shape = null;
			this.painter = null;
			this.diagramViewInfo = null;
		}
		public ShapeGeometry Shape { get { return shape; } }
		public DiagramConnectorPointDragPreviewPainter Painter { get { return painter; } }
		public DiagramControlViewInfo DiagramViewInfo { get { return diagramViewInfo; } }
	}
	public class DiagramConnectorPointDragPreviewAdornerPainter : DiagramAdornerPainterBase {
		public DiagramConnectorPointDragPreviewAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawConnectorPreview(e);
		}
		protected virtual void DrawConnectorPreview(ObjectInfoArgs e) {
			DiagramConnectorPointDragPreviewAdornerObjectInfoArgs drawArgs = (DiagramConnectorPointDragPreviewAdornerObjectInfoArgs)e;
			ShapeDraw.DrawConnector(e.Cache, drawArgs.Shape, drawArgs.Painter, drawArgs.PaintAppearance, drawArgs.DisplayBounds, connectorDrawArgs => connectorDrawArgs.Initialize(drawArgs.DiagramViewInfo));
		}
	}
	#endregion
	#region Connector Selection Parts
	public class DiagramConnectorSelectionPartAdornerObjectInfoArgs : DiagramSelectionPartAdornerObjectInfoArgs {
		ShapeGeometry shape;
		DiagramConnectorSelectionPartPainter painter;
		DiagramControlViewInfo diagramViewInfo;
		public DiagramConnectorSelectionPartAdornerObjectInfoArgs() {
			this.shape = null;
			this.painter = null;
			this.diagramViewInfo = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramConnectorSelectionPartAdorner selectionAdorner = (DiagramConnectorSelectionPartAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.shape = selectionAdorner.Shape.Scale(viewInfo.ZoomFactor);
			this.painter = viewInfo.ConnectorSelectionPartPainter;
			this.diagramViewInfo = viewInfo;
		}
		public override void Clear() {
			base.Clear();
			this.shape = null;
			this.painter = null;
			this.diagramViewInfo = null;
		}
		public ShapeGeometry Shape { get { return shape; } }
		public DiagramConnector Connector { get { return (DiagramConnector)base.Item; } }
		public DiagramConnectorSelectionPartPainter Painter { get { return painter; } }
		public DiagramControlViewInfo DiagramViewInfo { get { return diagramViewInfo; } }
	}
	public class DiagramConnectorSelectionPartAdornerPainter : DiagramSelectionPartAdornerPainter {
		public DiagramConnectorSelectionPartAdornerPainter() {
		}
		protected override void DrawSelectionPart(ObjectInfoArgs e) {
			DiagramConnectorSelectionPartAdornerObjectInfoArgs drawArgs = (DiagramConnectorSelectionPartAdornerObjectInfoArgs)e;
			ShapeDraw.DrawConnector(e.Cache, drawArgs.Shape, drawArgs.Painter, drawArgs.PaintAppearance, drawArgs.DisplayBounds, connectorDrawArgs => connectorDrawArgs.Initialize(drawArgs.DiagramViewInfo));
		}
	}
	#endregion
	#region Connection Points
	public class DiagramConnectionPointsAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		ShapedSelection rects;
		Color pointColor;
		public DiagramConnectionPointsAdornerObjectInfoArgs() {
			this.rects = null;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramConnectionPointsAdorner connectionPointsAdorner = (DiagramConnectionPointsAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.rects = connectionPointsAdorner.Selection;
			this.pointColor = appearances.GetConnectionPointColor();
		}
		public override void Clear() {
			base.Clear();
			this.rects = null;
		}
		public Color PointColor { get { return pointColor; } }
		public ShapedSelection Rects { get { return rects; } }
	}
	public class DiagramConnectionPointsAdornerPainter : DiagramAdornerPainterBase {
		public DiagramConnectionPointsAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramConnectionPointsAdornerObjectInfoArgs drawArgs = (DiagramConnectionPointsAdornerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			DrawSelection(drawArgs);
		}
		protected virtual void DrawSelection(DiagramConnectionPointsAdornerObjectInfoArgs drawArgs) {
			drawArgs.Rects.ForEachPoint(point => DrawConnectionPoint(drawArgs, point));
		}
		protected virtual void DrawConnectionPoint(DiagramConnectionPointsAdornerObjectInfoArgs drawArgs, DiagramElementBounds point) {
			DrawUtils.FillRectangle(drawArgs, point.DisplayBounds.Inflated(1), Color.FromArgb(120, drawArgs.PointColor));
			DrawUtils.DrawRectangularCloudEffect(drawArgs, drawArgs.PointColor, point.DisplayBounds.Inflated(1), 200);
		}
	}
	#endregion
	#region Glue To Item
	public class DiagramGlueToItemAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		Color borderColor;
		public DiagramGlueToItemAdornerObjectInfoArgs() {
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			base.Initialize(viewInfo, appearances, adorner);
			this.borderColor = appearances.GetItemGlueBorderColor();
		}
		public override void Clear() {
			base.Clear();
		}
		public Color BorderColor { get { return borderColor; } }
	}
	public class DiagramGlueToItemAdornerPainter : DiagramAdornerPainterBase {
		public DiagramGlueToItemAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramGlueToItemAdornerObjectInfoArgs drawArgs = (DiagramGlueToItemAdornerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			DrawBorder(drawArgs);
		}
		protected virtual void DrawBorder(DiagramGlueToItemAdornerObjectInfoArgs drawArgs) {
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.BorderColor, 3), drawArgs.DisplayBounds);
		}
	}
	#endregion
	#region Glue To Point
	public class DiagramGlueToPointAdornerObjectInfoArgs : DiagramAdornerObjectInfoArgsBase {
		Rectangle pointRect;
		Color pointColor;
		public DiagramGlueToPointAdornerObjectInfoArgs() {
			this.pointRect = Rectangle.Empty;
		}
		public override void Initialize(DiagramControlViewInfo viewInfo, DiagramDefaultAppearances appearances, DiagramAdornerBase adorner) {
			DiagramGlueToPointAdorner pointAdorner = (DiagramGlueToPointAdorner)adorner;
			base.Initialize(viewInfo, appearances, adorner);
			this.pointRect = pointAdorner.PointSize.CreateRect(DisplayBounds.Location);
			this.pointColor = appearances.GetPointGlueBorderColor();
		}
		public override void Clear() {
			base.Clear();
			this.pointRect = Rectangle.Empty;
		}
		public Color PointColor { get { return pointColor; } }
		public Rectangle PointRect { get { return pointRect; } }
	}
	public class DiagramGlueToPointAdornerPainter : DiagramAdornerPainterBase {
		public DiagramGlueToPointAdornerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramGlueToPointAdornerObjectInfoArgs drawArgs = (DiagramGlueToPointAdornerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			DrawPoint(drawArgs);
		}
		protected virtual void DrawPoint(DiagramGlueToPointAdornerObjectInfoArgs drawArgs) {
			drawArgs.Graphics.DrawRectangle(drawArgs.Cache.GetPen(drawArgs.PointColor, 3), drawArgs.PointRect);
		}
	}
	#endregion
}
