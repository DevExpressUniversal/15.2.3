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

using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Customization {
	public class HotTrackFrame :  BaseLeafPrimitive {
		BoxShape boxShape1;
		BoxShape boxShape2;
		protected override void OnCreate() {
			base.OnCreate();
			AcceptOrder = int.MaxValue;
			boxShape1 = new BoxShape();
			boxShape2 = new BoxShape();
			HitTestEnabled = false;
			Shape1.Name = "CustomizationHotTrackShape1";
			Shape1.Appearance.BorderBrush = new SolidBrushObject(Color.Blue);
			Shape2.Name = "CustomizationHotTrackShape2";
			Shape2.Appearance.BorderBrush = new PenBrushObject(Color.White, new float[] { 3, 2 });
		}
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			Shapes.Add(Shape1);
			Shapes.Add(Shape2);
		}
		protected BoxShape Shape1 {
			get { return boxShape1; }
		}
		protected BoxShape Shape2 {
			get { return boxShape2; }
		}
		public RectangleF2D Box {
			get { return Shape1.Box; }
			set {
				Shape1.BeginUpdate();
				Shape1.Box = value;
				Shape1.Bounds = value;
				Shape1.EndUpdate();
				Shape2.BeginUpdate();
				Shape2.Box = value;
				Shape2.Bounds = value;
				Shape2.EndUpdate();
			}
		}
	}
	public class CustomizationFrameItemBase : CustomizationPointerPrimitive {
		RectangleF boundsCore;
		protected SizeF itemFrameSize = new SizeF(7, 7);
		CustomizationFrameBase ownerCore;
		public CustomizationFrameItemBase(CustomizationFrameBase owner) {
			this.ownerCore = owner;
		}
		public RectangleF Bounds {
			get { return boundsCore; }
			set {
				if (Bounds == value) return;
				boundsCore = value;
				UpdateBounds();
			}
		}
		protected CustomizationFrameBase Owner {
			get { return ownerCore; }
		}
		public virtual void CalcLayout() { }
		protected virtual void UpdateBounds() {
			BeginTransform();
			Location = boundsCore.Location;
			ScaleFactor = new FactorF2D(Bounds.Width / 12f, Bounds.Height / 12f);
			EndTransform();
		}
		public CursorInfo Cursor {
			get { return GetCursor(); }
		}
		protected virtual CursorInfo GetCursor() {
			return CursorInfo.Normal;
		}
		public virtual RectangleF CorrectRenderRect(RectangleF proposedRect) {
			proposedRect.Inflate(5, 5);
			return proposedRect;
		}
		public virtual void OnDraggingStarted(Point p) { }
		public virtual void OnDragging(Point p) { }
		public virtual void OnDraggingFinished() { }
		public virtual void OnLeftClick() { }
		public virtual void OnRightClick() { }
		public virtual void OnDoubleClick() { }
	}
	public class ResizeFrameItem : CustomizationFrameItemBase {
		protected Point startPoint = Point.Empty;
		protected Rectangle startRect = Rectangle.Empty;
		public ResizeFrameItem(CustomizationFrameBase owner)
			: base(owner) {
			ZOrder = 40;
		}
		protected virtual void CalcDiff(Point start, Point current, out int rDiffX, out int rDiffY) {
			rDiffY = current.Y - start.Y;
			rDiffX = current.X - start.X;
		}
		protected void CalcDiffEx(Point p, out int diffy, out int diffx) {
			CalcDiff(startPoint, p, out diffx, out diffy);
		}
		public override void OnDraggingStarted(Point p) {
			startPoint = p;
			startRect = Owner.Client.Bounds;
		}
		public override void OnDraggingFinished() {
			startPoint = Point.Empty;
		}
		RectangleF maxRenderRect = RectangleF.Empty;
		public override RectangleF CorrectRenderRect(RectangleF proposedRect) {
			RectangleF result1 = base.CorrectRenderRect(proposedRect);
			RectangleF result2 = RectangleF.Union(result1, maxRenderRect);
			maxRenderRect = result2;
			return result2;
		}
		public override void CalcLayout() {
			RectangleF boundsCore;
			boundsCore = Owner.Client.Bounds;
			boundsCore.X -= itemFrameSize.Width / 2;
			boundsCore.Y -= itemFrameSize.Height / 2;
			boundsCore.Size = itemFrameSize;
			Bounds = boundsCore;
		}
		protected virtual void CalcApplyDiff(Point p, int xm, int ym, int wm, int hm, bool useConstraintsX, bool useConstraintsY) {
			int diffy, diffx;
			int cdiffy, cdiffx, cdiffw, cdiffh;
			CalcDiffEx(p, out diffy, out diffx);
			Rectangle newRect = startRect;
			cdiffx = diffx * xm;
			cdiffy = diffy * ym;
			cdiffw = diffx * wm;
			cdiffh = diffy * hm;
			newRect.Offset(cdiffx, cdiffy);
			newRect.Width += cdiffw;
			newRect.Height += cdiffh;
			Owner.Client.Bounds = newRect;
			Owner.Client.ResetAutoLayout();
		}
	}
	public class MoveFrameItem : ResizeFrameItem {
		public MoveFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "MoveFrameItem"; this.Renderable = false; }
		public override void CalcLayout() {
			RectangleF boundsCore;
			boundsCore = Owner.Client.Bounds;
			boundsCore.Inflate(-itemFrameSize.Width / 2, -itemFrameSize.Height / 2);
			Bounds = boundsCore;
		}
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 1, 1, 0, 0, false, false);
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.Move;
		}
	}
	public class ResizeVTFrameItem : ResizeFrameItem {
		public ResizeVTFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeVTFrameItem"; }
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 0, 1, 0, -1, false, true);
		}
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.X += Owner.Client.Bounds.Width / 2;
			Bounds = boundsCore;
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.VSizing;
		}
	}
	public class ResizeVBFrameItem : ResizeFrameItem {
		public ResizeVBFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeVBFrameItem"; }
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 0, 0, 0, 1, false, true);
		}
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.X += Owner.Client.Bounds.Width / 2;
			boundsCore.Y += Owner.Client.Bounds.Height;
			Bounds = boundsCore;
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.VSizing;
		}
	}
	public class ResizeHLFrameItem : ResizeFrameItem {
		public ResizeHLFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeHLFrameItem"; }
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 1, 0, -1, 0, true, false);
		}
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.Y += Owner.Client.Bounds.Height / 2;
			Bounds = boundsCore;
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.HSizing;
		}
	}
	public class ResizeHRFrameItem : ResizeFrameItem {
		public ResizeHRFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeHRFrameItem"; }
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 0, 0, 1, 0, true, false);
		}
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.Y += Owner.Client.Bounds.Height / 2;
			boundsCore.X += Owner.Client.Bounds.Width;
			Bounds = boundsCore;
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.HSizing;
		}
	}
	public class ResizeTLFrameItem : ResizeFrameItem {
		public ResizeTLFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeTLFrameItem"; }
		protected override CursorInfo GetCursor() { return CursorInfo.NWSESizing; }
		public override void CalcLayout() {
			base.CalcLayout();
		}
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 1, 1, -1, -1, true, true);
		}
	}
	public class ResizeTRFrameItem : ResizeFrameItem {
		public ResizeTRFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeTRFrameItem"; }
		protected override CursorInfo GetCursor() { return CursorInfo.NESWSizing; }
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.X += Owner.Client.Bounds.Width;
			Bounds = boundsCore;
		}
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 0, 1, 1, -1, true, true);
		}
	}
	public class ResizeBLFrameItem : ResizeFrameItem {
		public ResizeBLFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeBLFrameItem"; }
		protected override CursorInfo GetCursor() { return CursorInfo.NESWSizing; }
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.Y += Owner.Client.Bounds.Height;
			Bounds = boundsCore;
		}
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 1, 0, -1, 1, true, true);
		}
	}
	public class ResizeBRFrameItem : ResizeFrameItem {
		public ResizeBRFrameItem(CustomizationFrameBase owner) : base(owner) { Name = "ResizeBRFrameItem"; }
		protected override CursorInfo GetCursor() { return CursorInfo.NWSESizing; }
		public override void CalcLayout() {
			base.CalcLayout();
			RectangleF boundsCore = Bounds;
			boundsCore.X += Owner.Client.Bounds.Width;
			boundsCore.Y += Owner.Client.Bounds.Height;
			Bounds = boundsCore;
		}
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 0, 0, 1, 1, true, true);
		}
	}
	public class SelectionFrameItem : MoveFrameItemRenderable {
		protected override CursorInfo GetCursor() {
			return CursorInfo.Normal;
		}
		public SelectionFrameItem(CustomizationFrameBase owner)
			: base(owner) {
			Name = "SelectionFrameItem";
		}
		public override void OnDragging(Point p) { }
	}
	public class MoveFrameItemRenderable : ResizeFrameItem {
		PolylineShape polyLine;
		PolylineShape polyLineHitTest;
		public MoveFrameItemRenderable(CustomizationFrameBase owner)
			: base(owner) {
			Name = "TopLineFrameItem";
			ZOrder = 50;
		}
		protected override void OnCreate() {
			polyLine = new PolylineShape();
			polyLine.Appearance.BorderBrush = new SolidBrushObject(Color.White);
			polyLine.Appearance.ContentBrush = new SolidBrushObject(Color.Black);
			polyLine.Name = "polyLine";
			polyLineHitTest = new PolylineShape();
			polyLineHitTest.Appearance.BorderBrush = BrushObject.Empty;
			polyLineHitTest.Appearance.ContentBrush = BrushObject.Empty;
			polyLineHitTest.Name = "polyLineHT";
			base.OnCreate();
		}
		protected override void UpdateBounds() {
			Rectangle rect = Owner.Client.Bounds;
			rect.Inflate(2, 2);
			Rectangle rect1 = rect;
			rect1.Inflate(-2, -2);
			polyLine.Points = new PointF[] { 
				new PointF(rect.X, rect.Y),
				new PointF(rect.Right, rect.Y),
				new PointF(rect.Right, rect.Bottom),
				new PointF(rect.X, rect.Bottom),
				new PointF(rect.X, rect.Y), 
				new PointF(rect1.X, rect1.Y),
				new PointF(rect1.Right, rect1.Y),
				new PointF(rect1.Right, rect1.Bottom),
				new PointF(rect1.X, rect1.Bottom),
				new PointF(rect1.X, rect1.Y), 
			};
			rect = Owner.Client.Bounds;
			int dx = (int)itemFrameSize.Width / 2;
			int dy = (int)itemFrameSize.Height / 2;
			rect.Inflate(dx, dy);
			rect1 = rect;
			rect1.Inflate(-2 * dx, -2 * dy);
			polyLineHitTest.Points = new PointF[] { 
				new PointF(rect.X, rect.Y),
				new PointF(rect.Right, rect.Y),
				new PointF(rect.Right, rect.Bottom),
				new PointF(rect.X, rect.Bottom),
				new PointF(rect.X, rect.Y), 
				new PointF(rect1.X, rect1.Y),
				new PointF(rect1.Right, rect1.Y),
				new PointF(rect1.Right, rect1.Bottom),
				new PointF(rect1.X, rect1.Bottom),
				new PointF(rect1.X, rect1.Y), 
			};
			this.OnTransformChanged();
		}
		protected override void OnDelayedCalculation() {
			base.OnDelayedCalculation();
			RectangleF boundsCore = Bounds;
			boundsCore.X += itemFrameSize.Width - 1;
			boundsCore.Y += itemFrameSize.Height / 2 - 1;
			boundsCore.Width = Owner.Client.Bounds.Width - itemFrameSize.Width + 1;
			boundsCore.Height = 2;
			Bounds = boundsCore;
		}
		public override void CalcLayout() {
			base.CalcLayout();
			SetCalculationDelayed();
		}
		public override void OnDragging(Point p) {
			CalcApplyDiff(p, 1, 1, 0, 0, false, false);
		}
		public override void OnDraggingFinished() {
			base.OnDraggingFinished();
#if !DXPORTABLE
			using(new ComponentTransaction(Owner.Client as IComponent)) { }
#endif
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.Move;
		}
		protected override void LoadModelShapes() {
			Shapes.Add(polyLine);
			Shapes.Add(polyLineHitTest);
		}
	}
}
