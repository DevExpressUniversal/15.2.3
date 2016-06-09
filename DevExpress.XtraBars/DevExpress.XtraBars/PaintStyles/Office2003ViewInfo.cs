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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraBars.Forms;
namespace DevExpress.XtraBars.ViewInfo {
	public class DockControlOffice2003ViewInfo : DockControlViewInfo {
		public DockControlOffice2003ViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl barControl) : base(manager, parameters, barControl) {
		}
		public override bool IsDrawForeground { get { return true; } }
	}
	public class DockedBarControlOffice2003ViewInfo : DockedBarControlViewInfo {
		public DockedBarControlOffice2003ViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		protected override int CalcIndent(BarIndent indent) {
			int res = base.CalcIndent(indent);
			if(IsVertical) {
				if(indent == BarIndent.Bottom) res += 1;
			}
			else {
				if(indent == BarIndent.Right) res += 1;
			}
			return res;
		}
		protected override Rectangle CalcQuickCustomizationLinkBounds(Rectangle barBounds, Rectangle linkBounds, Size linkSize) {
			Rectangle r = linkBounds;
			if(IsVertical) {
				r.Height = linkSize.Height;
				r.Y = (barBounds.Bottom - r.Height) + CalcIndent(BarIndent.Right); 
			}
			else {
				r.Width = linkSize.Width;
				r.X = (barBounds.Right - r.Width) + CalcIndent(BarIndent.Right);
			}
			return r;
		}
		public override void UpdateControlRegion(Control control) {
			if(IsVertical) {
				base.UpdateControlRegion(control);
				return;
			}
			if(control == null || !control.IsHandleCreated || Bar == null) return;
			if(Bar.IsMainMenu || Bar.IsStatusBar) {
			}
			if(Bar.IsFloating) return;
			Region reg = new Region(control.ClientRectangle);
			Rectangle r = new Rectangle(0, 0, 1, 1);
			reg.Exclude(r);
			r.X = control.ClientRectangle.Right - 1;reg.Exclude(r);
			r.Y = control.ClientRectangle.Bottom - 1;reg.Exclude(r);
			r.X = control.ClientRectangle.X;r.Width = 2; reg.Exclude(r);
			r.Width = 1; r.Y --;reg.Exclude(r);
			control.Region = reg;
		}
	}
	public class FloatingBarControlOffice2003ViewInfo : FloatingBarControlViewInfo {
		public FloatingBarControlOffice2003ViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		protected override bool AllowRowSeparator { get { return false; } }
		protected override int CalcIndent(BarIndent indent) {
			if(indent == BarIndent.Bottom) return 0;
			if(indent == BarIndent.Left || indent == BarIndent.Right) return 0;
			return base.CalcIndent(indent);
		}
	}
	public class FloatingBarControlFormOffice2003ViewInfo : FloatingBarControlFormViewInfo {
		public FloatingBarControlFormOffice2003ViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters, controlForm) {
		}
		public override TitleBarEl CreateTitleBarInstance() { return new FloatingBarTitleBarOffice2003El(ControlForm.Manager); }
		protected override Size CalcBorderSize() { return new Size(3, 3); }
		protected override int CalcContentIndent(BarIndent indent) {
			return 2;
		}
	}
	public class FloatingBarTitleBarOffice2003El : FloatingBarTitleBarEl {
		protected override Size ButtonImageSize { get { return new Size(11, 13); } }
		public FloatingBarTitleBarOffice2003El(BarManager manager) : base(manager) {
			Border.Border = BarItemBorderStyle.None;
			Font = new Font(Control.DefaultFont, FontStyle.Bold);
			SelectedForeColor = ForeColor = manager.DrawParameters.Colors[BarColor2003.FloatingTitleBarForeColor];
			SelectedBackColor = BackColor = manager.DrawParameters.Colors[BarColor2003.FloatingTitleBarBackColor];
		}
		protected override void UpdateBrush() {
			CurrentBackColor = Manager.DrawParameters.Appearance(BarAppearance.Dock).BackColor2;
		}
	}
	public class FloatingBarControlOffice2003FormPainter : FloatingBarControlFormPainter {
		public FloatingBarControlOffice2003FormPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawBackground(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			FloatingBarControlFormViewInfo v = vi as FloatingBarControlFormViewInfo;
			Brush brush = DrawParameters.Colors.Brushes(BarColor2003.FloatingTitleBarBorderColor);
			PaintHelper.FillRectangle(e.Graphics, brush, v.ContentRect);
		}
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			FloatingBarControlFormViewInfo v = vi as FloatingBarControlFormViewInfo;
			Pen pen = DrawParameters.Colors.Pens(BarColor2003.FloatingTitleBarBackColor);
			Rectangle r = vi.WindowRect;
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
			Brush brush = DrawParameters.Colors.Brushes(BarColor2003.FloatingTitleBarBorderColor);
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.X + 1, r.Y, r.Width - 2, 1));
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.X, r.Y + 1, 1, r.Height - 2));
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.X + 1, r.Bottom - 1, r.Width - 2, 1));
			PaintHelper.FillRectangle(e.Graphics, brush, new Rectangle(r.Right - 1, r.Y + 1, 1, r.Height - 2));
		}
	}
}
