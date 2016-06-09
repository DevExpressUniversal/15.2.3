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
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout.Customization.Behaviours;
namespace DevExpress.XtraLayout.Painting {
	public class BaseLayoutItemPainter : ObjectPainter {
		ObjectPainter borderPainter;
		Brush selectionBrush;
		Pen selectionPen = null;
		Pen clientAreaBorderPen = null;
		Pen borderPen1 = null, borderPen2 = null;
		public LayoutPaintStyle GetPaintStyle(BaseViewInfo e) {
			return e.Owner == null ? LayoutPaintStyle.NullStyle : e.Owner.PaintStyle;
		}
		public BaseLayoutItemPainter() { }
		public ObjectPainter GetBorderPainter(BaseViewInfo e) {
			bool recreateBorderPainter = false;
			if(e.Owner is DashboardLayoutControlItemBase || e.Owner is DashboardLayoutControlGroupBase) recreateBorderPainter = true;
			if(borderPainter == null || recreateBorderPainter) borderPainter = CreateBorderPainter(e);
			return borderPainter;
		}
		protected virtual ObjectPainter CreateBorderPainter(BaseViewInfo e) { return new ObjectPainter(); }
		public virtual Utils.Padding GetPadding(BaseViewInfo e) { return Utils.Padding.Empty; }
		public virtual Utils.Padding GetSpacing(BaseViewInfo e) { return Utils.Padding.Empty; }
		protected Brush SelectionBrush {
			get {
				if(selectionBrush == null)
					selectionBrush = new SolidBrush(Color.FromArgb(50, 101, 200, 225));
				return selectionBrush;
			}
		}
		protected Pen SelectionPen {
			get {
				if(selectionPen == null) {
					selectionPen = new Pen(Color.FromArgb(168, 0, 182, 249));
				}
				return selectionPen;
			}
		}
		protected Pen ClientAreaBorderPen {
			get {
				if(clientAreaBorderPen == null) {
					clientAreaBorderPen = new Pen(Color.FromArgb(100, 41, 97, 187));
				}
				return clientAreaBorderPen;
			}
		}
		protected Pen HotTrackedPen {
			get {
				return ResizingLayoutGlyph.HotTrackedPen;
			}
		}
		protected Pen GetBorderPen(BaseViewInfo e) {
			if(!(e.OwnerILayoutControl.EnableCustomizationMode || e.OwnerILayoutControl.DesignMode)) {
				if(borderPen1 == null)
					borderPen1 = new Pen(e.OwnerILayoutControl.OptionsView.ItemBorderColor);
				return borderPen1;
			}
			else {
				if(borderPen2 == null)
					borderPen2 = new Pen(Color.FromArgb(178, 178, 178));
				return borderPen2;
			}																   
		}
		protected Pen GetBorderForFlowLinesPen(BaseViewInfo e) {
			double coef = 1.2;
			Color borderColor = GetBorderPen(e).Color;
			return new Pen(Color.FromArgb(borderColor.A, (int)(borderColor.R * coef), (int)(borderColor.G * coef), (int)(borderColor.B * coef)));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BaseLayoutItemViewInfo vi = e as BaseLayoutItemViewInfo;
			if(vi != null && !vi.Owner.ActualItemVisibility) return;
			if(vi.ClientAreaRelativeToControl.Contains(e.Cache.PaintArgs.ClipRectangle)) {
				DrawBackground(vi);
				DrawControlsArea(vi);
				DrawSelection(vi);
			}
			else {
				if(e.Cache.PaintArgs.ClipRectangle.IntersectsWith(InflateRectangleCore(vi.PainterBoundsRelativeToControl, vi.Owner))) {
					DrawBackground(vi);
					DrawTextArea(vi);
					DrawControlsArea(vi);
					DrawSelection(vi);
				}
			}
		}
		protected virtual void DrawTextArea(BaseLayoutItemViewInfo e) { }
		protected virtual void DrawSelection(BaseLayoutItemViewInfo e) {
			if(e.Owner.ViewInfo.State == ObjectState.Selected && (e.Owner != Handlers.DragDropDispatcherFactory.Default.DragItem || Handlers.DragDropDispatcherFactory.Default.State == Handlers.DragDropDispatcherState.Regular)) {
				Rectangle rect = InflateRectangleCore(e.PainterBoundsRelativeToControl, e.Owner);
				e.Cache.DrawRectangle(SelectionPen, rect);
			}
		}
		protected internal virtual void DrawSelectionInTab(BaseLayoutItemViewInfo e) {
			if(e.State == ObjectState.Selected && (e.Owner != Handlers.DragDropDispatcherFactory.Default.DragItem || Handlers.DragDropDispatcherFactory.Default.State == Handlers.DragDropDispatcherState.Regular)) {
				e.Cache.FillRectangle(SelectionBrush, e.ClientAreaRelativeToControl);
				e.Cache.DrawRectangle(SelectionPen, e.ClientAreaRelativeToControl);
			}
		}
		protected virtual void DrawBackground(BaseLayoutItemViewInfo e) {
			DrawBorder(e);
		}
		public virtual void DrawControlsArea(BaseLayoutItemViewInfo e) { }
		bool CanDrawBorderCore(BaseViewInfo e) {
			BaseLayoutItem item = e.Owner;
			if(item == null) return false;
			if(item.NeedSupressDrawBorder) return false;
			if(e.OwnerILayoutControl == null) return false;
			if(item.Parent == null && item == e.OwnerILayoutControl.RootGroup) return true;
			if(item.Parent == null) return false;
			if(!item.Parent.GroupBordersVisible && !item.CanCustomize) return true;
			if(!item.CanCustomize) return false;
			return true;
		}
		protected virtual bool CanDrawBorder(BaseViewInfo e) {
			if(!CanDrawBorderCore(e)) return false;
			if(e.OwnerILayoutControl == null) return false;
			if(Handlers.DragDropDispatcherFactory.Default.DragItem == e.Owner && Handlers.DragDropDispatcherFactory.Default.State != Handlers.DragDropDispatcherState.Regular) return false;
			if(e.OwnerILayoutControl.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !e.OwnerILayoutControl.DesignMode) {
				if(e.OwnerILayoutControl != null && Handlers.DragDropDispatcherFactory.Default.State == Handlers.DragDropDispatcherState.ClientDragging) return true;
				return false;
			}
			if(e.OwnerILayoutControl.DesignMode) return true;
			if(e.OwnerILayoutControl.OptionsView.DrawItemBorders && !((e.OwnerILayoutControl as ISupportImplementor).Implementor.PaintingType == PaintingType.Skinned)) return true;
			if(!e.OwnerILayoutControl.EnableCustomizationMode) return false;
			return true;
		}
		internal static Rectangle InflateRectangleCore(Rectangle painterBounds, BaseLayoutItem item) {
			if(item.Owner != null && item.Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !item.Owner.DesignMode) {
				if(item.Parent != null) { 
					painterBounds.Width++;
					painterBounds.Height++;
				}
			} else {
				if(item.X == 0) {
					painterBounds.X += 1;
					painterBounds.Width -= 1;
				}
				if(item.Y == 0) {
					painterBounds.Y += 1;
					painterBounds.Height -= 1;
				}
			}
			return painterBounds;
		}
		protected virtual void DrawHotTracked(BaseViewInfo e) {
			DrawHotTrackedCore(e.Cache, e.PainterBoundsRelativeToControl, HotTrackedPen, e.Owner);
		}
		internal static void DrawHotTrackedCore(GraphicsCache cache, Rectangle PainterBounds, Pen HotTrackedPen, BaseLayoutItem item) {
			Rectangle rect = InflateRectangleCore(PainterBounds, item);
			float opacity = item.ViewInfo.OpacityAnimation;
			Pen hotPen = HotTrackedPen.Clone() as Pen;
			hotPen.Color = Color.FromArgb((byte)(HotTrackedPen.Color.A * opacity), HotTrackedPen.Color);
			cache.DrawRectangle(hotPen, rect);
			cache.FillRectangle(new SolidBrush(Color.FromArgb((byte)(38* opacity), HotTrackedPen.Color)), rect);
		}
		protected bool IfAllowDrawBorderAtRuntime(BaseViewInfo e) {
			if(e.OwnerILayoutControl.DesignMode) return false;
			if(e.OwnerILayoutControl.EnableCustomizationMode) return true;
			if(e.OwnerILayoutControl.OptionsView.DrawItemBorders) return true;
			return false;
		}
		protected virtual void DrawBorderLines(BaseViewInfo e, Pen pen) {
			Rectangle rect = e.PainterBoundsRelativeToControl;
			BaseLayoutItem owner = e.Owner;
			DrawBorderLinesCore(e, pen, rect, owner);
		}
		static void DrawBorderLinesCore(BaseViewInfo e, Pen pen, Rectangle rect, BaseLayoutItem owner) {
			if(owner != null && owner.Parent != null && owner.Parent.LayoutMode == Utils.LayoutMode.Flow) {
				rect.Width++;
				rect.Height++;
				e.Graphics.DrawRectangle(pen, rect);
				return;
			}
			Rectangle parentRect = Rectangle.Empty;
			if(owner.Parent != null) {
				parentRect = owner.Parent.ViewInfo.ClientAreaRelativeToControl;
			}
			if(rect.X == parentRect.X || owner.Owner != null && owner.Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True)
				e.Graphics.DrawLine(pen, rect.X, rect.Top, rect.X, rect.Bottom);
			if(rect.Y == parentRect.Y || owner.Owner != null && owner.Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True)
				e.Graphics.DrawLine(pen, rect.X, rect.Top, rect.Right, rect.Top);
			e.Graphics.DrawLine(pen, rect.X, rect.Bottom, rect.Right, rect.Bottom);
			e.Graphics.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
		}
		protected virtual void DrawBorder(BaseViewInfo e) {
			if(e.State == ObjectState.Hot || (e.State == ObjectState.Pressed && e.OwnerILayoutControl != null && e.OwnerILayoutControl.OptionsView.DrawAdornerLayer != DefaultBoolean.True)) {
				if(e.OwnerILayoutControl.DesignMode || e.OwnerILayoutControl.EnableCustomizationMode)
					DrawHotTracked(e);
			}
			if(CanDrawBorder(e)) {
				Pen mPen = GetBorderPen(e);
				mPen.DashStyle = DashStyle.Solid;
				DrawBorderLines(e, mPen);
			}
			if(e.OwnerILayoutControl != null && e.Owner == Handlers.DragDropDispatcherFactory.Default.DragItem && e.OwnerILayoutControl.OptionsView.DrawAdornerLayer != DefaultBoolean.True && Handlers.DragDropDispatcherFactory.Default.State != Handlers.DragDropDispatcherState.Regular)
				e.Cache.FillRectangle(new SolidBrush(Color.FromArgb(150, Color.LightGray)), e.BoundsRelativeToControl);
		}
	}
}
