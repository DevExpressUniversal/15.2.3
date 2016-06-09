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

using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.ViewInfo;
namespace DevExpress.XtraLayout.Painting {
	public class FakeSpaceItemPainter : EmptySpaceItemPainter {
		public FakeSpaceItemPainter() : base() { }
		protected override void DrawBorder(BaseViewInfo e) {
			e.Cache.FillRectangle(Brushes.Blue, e.BoundsRelativeToControl);
		}
	}
	public class EmptySpaceItemPainter : LayoutControlItemPainter {
		public EmptySpaceItemPainter() : base() { }
		public override void DrawObject(ObjectInfoArgs e) {
			BaseLayoutItemViewInfo vi = e as BaseLayoutItemViewInfo;
			if(!vi.Owner.ActualItemVisibility) return;
			if(vi.ClientAreaRelativeToControl.Contains(e.Cache.PaintArgs.ClipRectangle)) {
				DrawBackground(vi);
			}
			else {
				if(e.Cache.PaintArgs.ClipRectangle.IntersectsWith(vi.PainterBoundsRelativeToControl)) {
					DrawBackground(vi);
					DrawTextArea(vi);
					DrawSelection(vi);
				}
			}
		}
		protected Rectangle GetPaintBounds(Rectangle bounds, Padding spacing) {
			return new Rectangle(bounds.Left + spacing.Left, bounds.Y + spacing.Top, bounds.Width - spacing.Width, bounds.Height - spacing.Height);
		}
	}
	public class SkinSimpleSeparatorItemPainter : EmptySpaceItemPainter {
		public SkinSimpleSeparatorItemPainter() : base() { }
		public override void DrawObject(ObjectInfoArgs e) {
			BaseLayoutItemViewInfo vi = e as BaseLayoutItemViewInfo;
			if(vi.Owner == null || !vi.Owner.ActualItemVisibility) return;
			DrawSeparatorLine(vi);
			DrawSelection(vi);
		}
		protected virtual void DrawSeparatorLine(BaseLayoutItemViewInfo vi) {
			SimpleSeparator separator = vi.Owner as SimpleSeparator;
			SkinElementInfo info = separator.SkinElementInfo;
			info.Bounds = GetPaintBounds(vi.BoundsRelativeToControl, separator.Spacing);
			ObjectPainter.DrawObject(vi.Cache, SkinElementPainter.Default, info);
		}
		protected override void DrawSelection(BaseLayoutItemViewInfo e) {
			if(e.Owner.ViewInfo.State == ObjectState.Selected) {
				Rectangle rect = e.BoundsRelativeToControl;
				rect.Inflate(2, 2);
				e.Cache.FillRectangle(SelectionBrush, rect);
				e.Cache.DrawRectangle(SelectionPen, rect);
			}
		}
	}
	public class FlatSimpleSeparatorItemPainter : EmptySpaceItemPainter {
		public FlatSimpleSeparatorItemPainter() : base() { }
		public override void DrawObject(ObjectInfoArgs e) {
			BaseLayoutItemViewInfo vi = e as BaseLayoutItemViewInfo;
			if(vi.Owner == null || !vi.Owner.ActualItemVisibility) return;
			DrawSeparatorLine(vi);
			DrawSelection(vi);
		}
		protected virtual void DrawSeparatorLine(BaseLayoutItemViewInfo vi) {
			SimpleSeparator separator = vi.Owner as SimpleSeparator;
			Rectangle bounds = GetPaintBounds(vi.BoundsRelativeToControl, separator.Spacing);
			if(separator.LayoutType == LayoutType.Horizontal) {
				vi.Paint.DrawLine(vi.Graphics, vi.Cache.GetPen(vi.Owner.PaintAppearanceItemCaption.ForeColor), new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));
				vi.Paint.DrawLine(vi.Graphics, vi.Cache.GetPen(vi.Owner.PaintAppearanceItemCaption.BackColor), new Point(bounds.Left, bounds.Top + 1), new Point(bounds.Right, bounds.Top + 1));
			}
			else {
				vi.Paint.DrawLine(vi.Graphics, vi.Cache.GetPen(vi.Owner.PaintAppearanceItemCaption.ForeColor), new Point(bounds.Left, bounds.Top), new Point(bounds.Left, bounds.Bottom));
				vi.Paint.DrawLine(vi.Graphics, vi.Cache.GetPen(vi.Owner.PaintAppearanceItemCaption.BackColor), new Point(bounds.Left + 1, bounds.Top), new Point(bounds.Left + 1, bounds.Bottom));
			}
		}
		protected override void DrawSelection(BaseLayoutItemViewInfo e) {
			if(e.Owner.ViewInfo.State == ObjectState.Selected) {
				Rectangle rect = e.BoundsRelativeToControl;
				rect.Inflate(2, 2);
				e.Cache.FillRectangle(SelectionBrush, rect);
				e.Cache.DrawRectangle(SelectionPen, rect);
			}
		}
	}
	public class LineItemPainter : EmptySpaceItemPainter {
		LayoutType layoutType;
		Pen linePen = null;
		public LineItemPainter()
			: base() {
			layoutType = LayoutType.Horizontal;
		}
		public LineItemPainter(LayoutType layoutType)
			: base() {
			this.layoutType = layoutType;
		}
		protected Pen LinePen {
			get {
				if(linePen == null) linePen = new Pen(Color.Black, 6);
				return linePen;
			}
		}
		protected override void DrawBackground(BaseLayoutItemViewInfo e) {
			base.DrawBackground(e);
			Point p1, p2;
			if(layoutType == LayoutType.Horizontal) {
				p1 = new Point(e.Bounds.X, e.Bounds.Y + e.Bounds.Height / 2);
				p2 = new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height / 2);
			}
			else {
				p1 = new Point(e.Bounds.X + e.Bounds.Width / 2, e.Bounds.Y);
				p2 = new Point(e.Bounds.X + e.Bounds.Width / 2, e.Bounds.Y + e.Bounds.Height);
			}
			e.Cache.Graphics.DrawLine(LinePen, p1, p2);
		}
	}
}
