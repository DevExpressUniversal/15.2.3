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
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Animations;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Parser;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Paint {
	#region Paint Args
	public abstract class DiagramItemObjectInfoArgs : ObjectInfoArgs {
		DiagramItem item;
		DiagramAppearanceObject paintAppearance;
		public DiagramItemObjectInfoArgs()
			: this(null) {
		}
		public DiagramItemObjectInfoArgs(GraphicsCache cache) {
			this.Cache = cache;
		}
		public virtual void Initialize(DiagramItemInfo itemInfo, DiagramControlViewInfo viewInfo) {
			this.Bounds = itemInfo.Bounds;
			this.State = itemInfo.State;
			this.item = itemInfo.Item;
			this.paintAppearance = itemInfo.PaintAppearance;
		}
		public virtual void Clear() {
			this.paintAppearance = null;
			this.item = null;
		}
		public DiagramAppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public DiagramItem Item { get { return item; } }
	}
	public abstract class DiagramPathViewItemObjectInfoArgs : DiagramItemObjectInfoArgs {
		DiagramItemView itemView;
		DiagramItemFadeOutAnimation fadeOutAnimation;
		DiagramPaintCache paintCache;
		public DiagramPathViewItemObjectInfoArgs() {
			this.itemView = null;
			this.fadeOutAnimation = null;
			this.paintCache = null;
		}
		public DiagramPathViewItemObjectInfoArgs(GraphicsCache cache) {
			this.Cache = cache;
		}
		public DiagramPathViewItemObjectInfoArgs(GraphicsCache cache, DiagramItemView itemView) : this(cache) {
			this.itemView = itemView;
		}
		public virtual void Initialize(DiagramItemInfo itemInfo) {
			Initialize(itemInfo, null);
		}
		public override void Initialize(DiagramItemInfo itemInfo, DiagramControlViewInfo viewInfo) {
			DiagramPathViewItemInfo pathViewItemInfo = (DiagramPathViewItemInfo)itemInfo;
			base.Initialize(itemInfo, viewInfo);
			this.itemView = pathViewItemInfo.View;
			if(viewInfo != null) {
				this.fadeOutAnimation = viewInfo.AnimationController.ItemFadeOutAnimation;
				this.paintCache = viewInfo.PaintCache;
			}
		}
		public override void Clear() {
			base.Clear();
			this.itemView = null;
			this.fadeOutAnimation = null;
			this.paintCache = null;
		}
		public DiagramItemView ItemView { get { return itemView; } }
		public bool InAnimation { get { return this.fadeOutAnimation != null; } }
		public DiagramItemFadeOutAnimation FadeOutAnimation { get { return fadeOutAnimation; } }
		public DiagramPaintCache PaintCache { get { return paintCache; } }
	}
	public class DiagramContainerObjectInfoArgs : DiagramItemObjectInfoArgs {
		DiagramItemFadeOutAnimation fadeOutAnimation;
		public DiagramContainerObjectInfoArgs() {
			this.fadeOutAnimation = null;
		}
		public DiagramContainerObjectInfoArgs(GraphicsCache cache)
			: base(cache) {
		}
		public override void Initialize(DiagramItemInfo itemInfo, DiagramControlViewInfo viewInfo) {
			DiagramContainerInfo containerItemInfo = (DiagramContainerInfo)itemInfo;
			base.Initialize(itemInfo, viewInfo);
			if(viewInfo != null) {
				fadeOutAnimation = viewInfo.AnimationController.ItemFadeOutAnimation;
			}
		}
		public override void Clear() {
			base.Clear();
			this.fadeOutAnimation = null;
		}
		public bool InAnimation { get { return this.fadeOutAnimation != null; } }
		public DiagramItemFadeOutAnimation FadeOutAnimation { get { return fadeOutAnimation; } }
	}
	public class DiagramShapeObjectInfoArgs : DiagramPathViewItemObjectInfoArgs {
		public DiagramShapeObjectInfoArgs() {
		}
		public DiagramShapeObjectInfoArgs(GraphicsCache cache)
			: base(cache) {
		}
		public DiagramShapeObjectInfoArgs(GraphicsCache cache, DiagramItemView itemView)
			: base(cache, itemView) {
		}
		public double GetAngle() {
			return Shape != null ? Shape.Angle : 0;
		}
		public ShapeDescription GetShape() {
			return Shape != null ? Shape.Shape : null;
		}
		public DoubleCollection GetShapeParameters() {
			return Shape != null ? Shape.Parameters : null;
		}
		public DiagramShape Shape { get { return (DiagramShape)Item; } }
	}
	public class DiagramConnectorObjectInfoArgs : DiagramPathViewItemObjectInfoArgs {
		double zoomFactor;
		Color pointDragBackColor;
		Color selectionPartBackColor;
		public DiagramConnectorObjectInfoArgs() {
			this.pointDragBackColor = this.selectionPartBackColor = Color.Empty;
			this.zoomFactor = 1;
		}
		public DiagramConnectorObjectInfoArgs(GraphicsCache cache)
			: base(cache) {
		}
		public DiagramConnectorObjectInfoArgs(GraphicsCache cache, DiagramItemView itemView)
			: base(cache, itemView) {
		}
		public override void Initialize(DiagramItemInfo itemInfo, DiagramControlViewInfo viewInfo) {
			base.Initialize(itemInfo, viewInfo);
			if(viewInfo != null) {
				Initialize(viewInfo);
			}
		}
		public virtual void Initialize(DiagramControlViewInfo viewInfo) {
			this.zoomFactor = viewInfo.ZoomFactor;
			this.pointDragBackColor = viewInfo.DefaultAppearances.GetConnectorPointDragBackColor();
			this.selectionPartBackColor = viewInfo.DefaultAppearances.GetSelectionPartBackColor();
		}
		public override void Clear() {
			base.Clear();
			this.zoomFactor = 1;
			this.pointDragBackColor = this.selectionPartBackColor = Color.Empty;
		}
		public double ZoomFactor { get { return zoomFactor; } }
		public Color PointDragBackColor { get { return pointDragBackColor; } }
		public Color SelectionPartBackColor { get { return selectionPartBackColor; } }
	}
	#endregion
	#region Painters
	public abstract class DiagramItemPainterBase : ObjectPainter {
		public DiagramItemPainterBase() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
		}
	}
	public abstract class DiagramPathViewItemPainterBase : DiagramItemPainterBase {
		public DiagramPathViewItemPainterBase() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramPathViewItemObjectInfoArgs drawArgs = (DiagramPathViewItemObjectInfoArgs)e;
			base.DrawObject(e);
			Graphics graphics = e.Cache.Graphics;
			SmoothingMode smoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				DoDraw(drawArgs);
			}
			finally {
				graphics.SmoothingMode = smoothingMode;
			}
		}
		protected abstract void DoDraw(DiagramPathViewItemObjectInfoArgs drawArgs);
	}
	public class DiagramContainerPainter : DiagramItemPainterBase {
		public DiagramContainerPainter() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramContainerObjectInfoArgs drawArgs = (DiagramContainerObjectInfoArgs)e;
			base.DrawObject(drawArgs);
			DrawBorder(drawArgs);
		}
		protected virtual void DrawBorder(DiagramContainerObjectInfoArgs drawArgs) {
			using(Pen dashPen = new Pen(GetBorderColor(drawArgs), drawArgs.PaintAppearance.BorderSize)) {
				dashPen.DashStyle = DashStyle.Dash;
				drawArgs.Graphics.DrawRectangle(dashPen, drawArgs.Bounds);
			}
		}
		protected Color GetBorderColor(DiagramContainerObjectInfoArgs drawArgs) {
			Color color = drawArgs.PaintAppearance.BorderColor;
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.Item, color);
			}
			return color;
		}
	}
	public class DiagramConnectorPainter : DiagramPathViewItemPainterBase {
		public DiagramConnectorPainter() {
		}
		protected override void DoDraw(DiagramPathViewItemObjectInfoArgs e) {
			DiagramConnectorObjectInfoArgs drawArgs = (DiagramConnectorObjectInfoArgs)e;
			DrawConnector(drawArgs);
			if(AllowDrawShadow(drawArgs)) DrawShadow(drawArgs);
		}
		protected virtual void DrawConnector(DiagramConnectorObjectInfoArgs e) {
			DiagramConnectorObjectInfoArgs drawArgs = (DiagramConnectorObjectInfoArgs)e;
			DiagramItemDrawArgs args = new DiagramItemDrawArgs(e.Cache.GetSolidBrush(GetConnectorColor(e)), e.Cache.GetPen(GetConnectorColor(e), GetPenWidth(drawArgs)), GetTextBrush(e));
			e.ItemView.Draw(e.Cache, args);
		}
		protected virtual int GetPenWidth(DiagramConnectorObjectInfoArgs drawArgs) {
			return AllowDrawShadow(drawArgs) ? drawArgs.PaintAppearance.BorderSize : GetShadowPenWidth(drawArgs);
		}
		protected virtual int GetShadowPenWidth(DiagramConnectorObjectInfoArgs drawArgs) {
			int borderSize = drawArgs.PaintAppearance.BorderSize;
			return borderSize == 1 ? 2 : drawArgs.PaintAppearance.BorderSize + 2;
		}
		protected virtual void DrawShadow(DiagramConnectorObjectInfoArgs e) {
			DiagramConnectorObjectInfoArgs drawArgs = (DiagramConnectorObjectInfoArgs)e;
			DiagramItemDrawArgs args = new DiagramItemDrawArgs(e.Cache.GetSolidBrush(GetConnectorColor(e)), e.Cache.GetPen(GetShadowColor(drawArgs), GetShadowPenWidth(drawArgs)), GetTextBrush(drawArgs));
			drawArgs.ItemView.Draw(e.Cache, args);
		}
		protected virtual bool AllowDrawShadow(DiagramConnectorObjectInfoArgs drawArgs) {
			return drawArgs.ZoomFactor < 3;
		}
		protected virtual Color GetShadowColor(DiagramConnectorObjectInfoArgs drawArgs) {
			int alpha = Math.Min(255, (int)(80.0 * drawArgs.ZoomFactor));
			return Color.FromArgb(alpha, GetConnectorColor(drawArgs));
		}
		protected virtual Color GetConnectorColor(DiagramConnectorObjectInfoArgs drawArgs) {
			AppearanceObject appearance = drawArgs.PaintAppearance;
			Color color = appearance.BackColor;
			if(color.IsWhite()) {
				color = appearance.BorderColor;
			}
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.Item, color);
			}
			return color;
		}
		protected Brush GetTextBrush(DiagramConnectorObjectInfoArgs drawArgs) {
			return drawArgs.Cache.GetSolidBrush(GetTextColor(drawArgs));
		}
		protected virtual Color GetTextColor(DiagramConnectorObjectInfoArgs drawArgs) {
			return drawArgs.PaintAppearance.ForeColor;
		}
	}
	public class DiagramConnectorDragPreviewPainter : DiagramConnectorPainter {
		public DiagramConnectorDragPreviewPainter() {
		}
		protected override bool AllowDrawShadow(DiagramConnectorObjectInfoArgs drawArgs) {
			return false;
		}
		protected override int GetPenWidth(DiagramConnectorObjectInfoArgs drawArgs) {
			return 1;
		}
		protected override Color GetConnectorColor(DiagramConnectorObjectInfoArgs drawArgs) {
			return Color.FromArgb(120, base.GetConnectorColor(drawArgs));
		}
		protected override Color GetTextColor(DiagramConnectorObjectInfoArgs drawArgs) {
			return Color.FromArgb(120, base.GetTextColor(drawArgs));
		}
	}
	public class DiagramConnectorPointDragPreviewPainter : DiagramConnectorPainter {
		public DiagramConnectorPointDragPreviewPainter() {
		}
		protected override void DrawConnector(DiagramConnectorObjectInfoArgs e) {
			using(Pen connectorPen = CreatePen(e)) {
				DiagramItemDrawArgs args = new DiagramItemDrawArgs(e.Cache.GetSolidBrush(GetConnectorColor(e)), connectorPen);
				e.ItemView.Draw(e.Cache, args);
			}
		}
		protected virtual Pen CreatePen(DiagramConnectorObjectInfoArgs e) {
			Pen pen = new Pen(GetConnectorColor(e), e.PaintAppearance.BorderSize);
			pen.DashStyle = DashStyle.Dash;
			pen.DashPattern = new float[] { 4, 4 };
			return pen;
		}
		protected override bool AllowDrawShadow(DiagramConnectorObjectInfoArgs drawArgs) {
			return false;
		}
		protected override Color GetConnectorColor(DiagramConnectorObjectInfoArgs drawArgs) {
			return drawArgs.PointDragBackColor;
		}
	}
	public class DiagramConnectorSelectionPartPainter : DiagramConnectorPainter {
		public DiagramConnectorSelectionPartPainter() {
		}
		protected override void DrawConnector(DiagramConnectorObjectInfoArgs e) {
			base.DrawConnector(e);
		}
		protected override int GetPenWidth(DiagramConnectorObjectInfoArgs drawArgs) {
			return 2;
		}
		protected override bool AllowDrawShadow(DiagramConnectorObjectInfoArgs drawArgs) {
			return false;
		}
		protected override Color GetConnectorColor(DiagramConnectorObjectInfoArgs drawArgs) {
			return drawArgs.SelectionPartBackColor;
		}
	}
	public abstract class DiagramShapePainterBase : DiagramPathViewItemPainterBase {
		public DiagramShapePainterBase() {
		}
		protected override void DoDraw(DiagramPathViewItemObjectInfoArgs e) {
			DiagramShapeObjectInfoArgs drawArgs = (DiagramShapeObjectInfoArgs)e;
			DrawShape(drawArgs);
		}
		protected virtual void DrawShape(DiagramShapeObjectInfoArgs e) {
			DiagramItemDrawArgs args = new DiagramItemDrawArgs(e.Cache.GetSolidBrush(GetBackColor(e)), e.Cache.GetPen(GetBorderColor(e), GetPenWeight(e)), GetTextBrush(e), DrawShadow, GetShadowColor(e), ShadowSize, e.Bounds, e.GetShape(), e.GetAngle(), e.GetShapeParameters(), e.PaintCache);
			e.ItemView.Draw(e.Cache, args);
		}
		protected virtual Color GetBackColor(DiagramPathViewItemObjectInfoArgs drawArgs){
			Color backColor = drawArgs.PaintAppearance.BackColor;
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.Item, backColor);
			}
			return backColor;
		}
		protected virtual Color GetBorderColor(DiagramPathViewItemObjectInfoArgs drawArgs) {
			Color borderColor = drawArgs.PaintAppearance.BorderColor;
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.Item, borderColor);
			}
			return borderColor;
		}
		protected virtual Color GetShadowColor(DiagramPathViewItemObjectInfoArgs drawArgs) {
			Color shadowColor = drawArgs.PaintAppearance.BackColor;
			if(drawArgs.InAnimation) {
				return drawArgs.FadeOutAnimation.GetColor(drawArgs.Item, shadowColor);
			}
			return shadowColor;
		}
		protected virtual Brush GetTextBrush(DiagramShapeObjectInfoArgs drawArgs) {
			return drawArgs.Cache.GetSolidBrush(drawArgs.PaintAppearance.ForeColor);
		}
		protected virtual int GetPenWeight(DiagramShapeObjectInfoArgs drawArgs) {
			return drawArgs.PaintAppearance.BorderSize;
		}
		protected abstract int ShadowSize { get; }
		protected abstract bool DrawShadow { get; }
	}
	public abstract class DiagramShapePainter : DiagramShapePainterBase {
		public DiagramShapePainter() {
		}
		protected override bool DrawShadow { get { return true; } }
		protected override int ShadowSize { get { return 10; } } 
	}
	public class DefaultDiagramShapePainter : DiagramShapePainter {
	}
	public class ToolboxDiagramShapePainter : DefaultDiagramShapePainter {
		protected override int GetPenWeight(DiagramShapeObjectInfoArgs drawArgs) {
			return 1;
		}
		protected override bool DrawShadow { get { return false; } }
	}
	public class ToolboxHighDiagramShapePainter : ToolboxDiagramShapePainter {
		protected override int GetPenWeight(DiagramShapeObjectInfoArgs drawArgs) {
			return 4;
		}
	}
	public class DiagramShapeDragPreviewPainter : DiagramShapePainter {
		protected override Color GetBackColor(DiagramPathViewItemObjectInfoArgs drawArgs) {
			Color baseColor = base.GetBackColor(drawArgs);
			return drawArgs.InAnimation ? baseColor : Color.FromArgb(50, baseColor);
		}
		protected override bool DrawShadow { get { return false; } }
	}
	#endregion
	public class ShapeDraw {
		public static void DrawShape(GraphicsCache cache, DiagramShapeInfo shapeInfo, DiagramShapePainter painter, Rectangle bounds) {
			DiagramItemView view = GetShapeView(shapeInfo.Shape, shapeInfo.PaintAppearance, bounds);
			DiagramShapeObjectInfoArgs drawArgs = new DiagramShapeObjectInfoArgs(cache, view);
			drawArgs.PaintAppearance = shapeInfo.PaintAppearance;
			try {
				DoDraw(cache, painter, drawArgs, bounds);
			}
			finally {
				drawArgs.Clear();
				if(view != null) view.Dispose();
			}
		}
		public static void DrawConnector(GraphicsCache cache, ShapeGeometry shape, DiagramPathViewItemPainterBase painter, DiagramAppearanceObject appearance, Rectangle bounds, Action<DiagramConnectorObjectInfoArgs> initializer) {
			DiagramItemView view = GetConnectorView(new DiagramSimplePathView(shape), appearance, bounds);
			DiagramConnectorObjectInfoArgs drawArgs = new DiagramConnectorObjectInfoArgs(cache, view);
			drawArgs.PaintAppearance = appearance;
			if(initializer != null) initializer(drawArgs);
			try {
				DoDraw(cache, painter, drawArgs, bounds);
			}
			finally {
				drawArgs.Clear();
				if(view != null) view.Dispose();
			}
		}
		static void DoDraw(GraphicsCache cache, DiagramPathViewItemPainterBase painter, DiagramPathViewItemObjectInfoArgs drawArgs, Rectangle bounds) {
			drawArgs.Bounds = bounds;
			try {
				ObjectPainter.DrawObject(cache, painter, drawArgs);
			}
			finally {
				drawArgs.Cache = null;
				drawArgs.Clear();
			}
		}
		static DiagramItemView GetShapeView(IXtraPathView item, DiagramAppearanceObject appearance, Rectangle bounds) {
			DiagramItemView view;
			using(DiagramSimpleShapeParser shapeParser = new DiagramSimpleShapeParser(new DiagramItemParseStrategy())) {
				view = shapeParser.Parse(item, bounds, appearance);
			}
			return view;
		}
		static DiagramItemView GetConnectorView(IXtraPathView item, DiagramAppearanceObject appearance, Rectangle bounds) {
			DiagramItemView view;
			using(DiagramSimpleConnectorParser connectorParser = new DiagramSimpleConnectorParser(new DiagramItemParseStrategy())) {
				view = connectorParser.Parse(item, bounds, appearance);
			}
			return view;
		}
	}
}
