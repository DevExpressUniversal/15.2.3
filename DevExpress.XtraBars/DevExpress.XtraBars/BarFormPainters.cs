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
using DevExpress.XtraBars.Controls;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Painters {
	public class ControlFormPainter : CustomPainter {
		public ControlFormPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public virtual void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			if(!vi.OwnerRectangle.IsEmpty) 
				DrawOpenedBorder(e, vi);
			else
				DrawRealBorder(e, vi);
		}
		protected virtual void DrawOpenedBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			Rectangle ownerRectangle = vi.OwnerRectangle;
			if(!ownerRectangle.IsEmpty) {
				Rectangle r = new Rectangle(vi.ControlForm.PointToScreen(vi.WindowRect.Location), vi.WindowRect.Size);
				int Y = BarManager.zeroPoint.Y;
				if(r.Y == ownerRectangle.Bottom) Y = r.Y;
				if(r.Bottom  == ownerRectangle.Top) Y = r.Bottom - 1;
				if(Y != BarManager.zeroPoint.Y) {
					Rectangle w = new Rectangle(r.X + 1, Y, r.Width - 2, 1);
					if(ownerRectangle.Left + 1 > w.X)
						w.X = ownerRectangle.Left + 1;
					w.Width = r.Right - 1 - w.X;
					if(ownerRectangle.Right - 1 < w.Right) 
						w.Width = ownerRectangle.Right - w.X - 1;
					DrawRealBorder(e, vi);
					w.Location = vi.ControlForm.PointToClient(w.Location);
					PaintHelper.FillRectangle(e.Graphics, vi.DrawParameters.Colors.Brushes(BarColor.SubMenuEmptyBorderColor), w);
					return;
				}
			}
			DrawRealBorder(e, vi);
		}
		protected virtual void DrawRealBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			Rectangle r = vi.WindowRect;
			PaintHelper.DrawRectangle(e.Graphics, DrawParameters.Colors.MenuAppearance.Menu.GetBorderPen(e.Cache), r);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, DrawParameters.Colors.MenuAppearance.Menu.GetBackPen(e.Cache), r);
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			ControlFormViewInfo vi = (ControlFormViewInfo)info;
			DrawBorder(e, vi);
			DrawCaption(e, vi);
			DrawBackground(e, vi);
		}
		protected virtual void DrawBackground(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			e.Graphics.ExcludeClip(vi.ControlRect);
			e.Graphics.ExcludeClip(vi.CaptionRect);
			PaintHelper.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(vi.ControlForm.BackColor), vi.ContentRect);
		}
		public virtual void DrawCaption(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			if(vi.CaptionRect.IsEmpty || vi.ControlForm.TitleBar == null) return;
			vi.ControlForm.TitleBar.DoDraw(e);
		}
	}
	public class FloatingBarControlFormPainter : ControlFormPainter {
		public FloatingBarControlFormPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawBackground(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			FloatingBarControlFormViewInfo v = vi as FloatingBarControlFormViewInfo;
			e.Graphics.ExcludeClip(v.ControlRect);
			Brush brush = v.BarControl.ViewInfo.Appearance.Normal.GetBackBrush(e.Cache);
			Color clr = PaintHelper.GetBrushColor(brush);
			if(clr.A != 255) brush = e.Cache.GetSolidBrush(Color.FromArgb(clr.R, clr.G, clr.B));
			PaintHelper.FillRectangle(e.Graphics, brush, v.ContentRect);
		}
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			FloatingBarControlFormViewInfo v = vi as FloatingBarControlFormViewInfo;
			Pen pen = DrawParameters.Colors.Pens(BarColor.FloatingBarBorderColor);
			Rectangle r = vi.WindowRect;
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
			Brush brush = v.BarControl.ViewInfo.Appearance.Normal.GetBackBrush(e.Cache);
			if(brush == null) brush = DrawParameters.GetBackBrush(BarAppearance.Bar, e.Cache);
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.X + 1, r.Y, r.Width - 2, 1));
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.X, r.Y + 1, 1, r.Height - 2));
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.X + 1, r.Bottom - 1, r.Width - 2, 1));
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.Right - 1, r.Y + 1, 1, r.Height - 2));
		}
	}
}
