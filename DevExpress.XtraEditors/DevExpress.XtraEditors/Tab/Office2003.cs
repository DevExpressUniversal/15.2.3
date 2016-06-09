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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Drawing;
namespace DevExpress.XtraTab.Drawing {
	public class Office2003TabHeaderViewInfo : FlatTabHeaderViewInfo { 
		AppearanceObject headerBorderAppearance;
		public Office2003TabHeaderViewInfo(BaseTabControlViewInfo viewInfo) : base(viewInfo) { 
			headerBorderAppearance = new AppearanceObject();
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new Office2003TabPageViewInfo(page);
		}
		protected override Size CalcButtonsSize() {
			Size res = base.CalcButtonsSize();
			if(res.IsEmpty) return res;
			if(IsSideLocation)
				res.Height ++;
			else
				res.Width ++;
			return res;
		}
		protected override int GetButtonsAdditionalOffsetInMultiRow() {
			return 2;
		}
		protected override void UpdatePageBounds(BaseTabPageViewInfo info) { 
			if(!info.IsActiveState) return;
			Rectangle r = info.Bounds;
			if(IsSideLocation) {
				r.Inflate(0, 1);
				if(info.VisibleIndex > 0) {
					r.Y --; r.Height ++;
				}
			} else {
				r.Inflate(1, 0);
				if(info.VisibleIndex > 0) {
					r.X --; r.Width ++;
				}
			}
			info.Bounds = r;
		}
		protected internal override EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			return new EditorButtonPainter(new Office2003TabHeaderButtonPainter());
		}
		public override void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Properties.AppearancePage.Header }, ViewInfo.GetDefaultAppearance(TabPageAppearance.PageHeader));
			AppearanceHelper.Combine(HeaderBorderAppearance, new AppearanceObject[] {Properties.AppearancePage.HeaderActive }, ViewInfo.GetDefaultAppearance(TabPageAppearance.PageHeaderActive));
			if(IsSideLocation) {
				if(HeaderBorderAppearance.GradientMode == LinearGradientMode.Vertical) {
					HeaderBorderAppearance.GradientMode = LinearGradientMode.Horizontal;
				}
				HeaderBorderAppearance.BackColor = HeaderLocation == TabHeaderLocation.Right ? HeaderBorderAppearance.BackColor : HeaderBorderAppearance.BackColor2;
			} else {
				HeaderBorderAppearance.BackColor = HeaderLocation == TabHeaderLocation.Bottom ? HeaderBorderAppearance.BackColor : HeaderBorderAppearance.BackColor2;
			}
		}
		public AppearanceObject HeaderBorderAppearance { get { return headerBorderAppearance; } }
	}
	public class Office2003TabPainter : FlatTabPainter {
		public Office2003TabPainter(IXtraTab tabControl) : base(tabControl) { }
		protected override BorderPainter CreateBorderPainter() { return new RotatedTabBorderPainter(); }
		protected override void DrawPageSeparator(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			Office2003TabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo as Office2003TabHeaderViewInfo;
			Color c1 = ControlPaint.Dark(hInfo.PaintAppearance.BackColor2 == Color.Empty ? hInfo.PaintAppearance.BackColor : hInfo.PaintAppearance.BackColor2, 0.1f);
			Color c2 = ControlPaint.LightLight(hInfo.PaintAppearance.BackColor);
			Rectangle border = new Rectangle(pInfo.Bounds.Right - 2, pInfo.Bounds.Y + 2, 1, pInfo.Bounds.Height - 5);
			if(e.ViewInfo.HeaderInfo.IsSideLocation) 
				border = new Rectangle(pInfo.Bounds.X + 2, pInfo.Bounds.Bottom - 2, pInfo.Bounds.Width - 5, 1);
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(c1), border);
			border.Offset(1, 1);
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(c2), border);
		}
		protected override int FlatBorderThin { get { return 3; } }
		protected override void DrawActiveHeaderPage(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			Rectangle buttons = pInfo.ViewInfo.HeaderInfo.ButtonsBounds;
			if(buttons.IntersectsWith(pInfo.Bounds))
				e.Graphics.ExcludeClip(buttons);
			base.DrawActiveHeaderPage(e, pInfo);
		}
	}
	public class Office2003TabHeaderRowBorderPainter : BorderSidePainter {
		protected override int GetBorderWidth(BorderSide side) { return 2; }
		BaseTabControlViewInfo GetTabInfo(ObjectInfoArgs e) {
			TabBorderObjectInfoArgs ee = e as TabBorderObjectInfoArgs;
			return ee == null ? null : ee.ViewInfo;
		}
		protected override BorderSide GetBorderSides(ObjectInfoArgs e) {
			BaseTabControlViewInfo viewInfo = GetTabInfo(e);
			if(viewInfo == null) return BorderSide.Bottom;
			if(viewInfo.HeaderInfo.IsSideLocation) {
				return viewInfo.HeaderInfo.IsLeftLocation ? BorderSide.Right : BorderSide.Left;
			} 
			return viewInfo.HeaderInfo.IsTopLocation ? BorderSide.Bottom : BorderSide.Top;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			Rectangle res = base.GetObjectClientRectangle(e);
			return res; 
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			Rectangle res = base.CalcBoundsByClientRectangle(e, client);
			return res; 
		}
		protected override void DrawSide(ObjectInfoArgs e, BorderSide side, Rectangle bounds) {
			BaseTabControlViewInfo viewInfo = GetTabInfo(e);
			if(viewInfo == null) return;
			Office2003TabHeaderViewInfo hInfo = viewInfo.HeaderInfo as Office2003TabHeaderViewInfo;
			AppearanceObject app = hInfo.HeaderBorderAppearance;
			Brush border = app.GetBorderBrush(e.Cache);
			Brush brush = app.GetBackBrush(e.Cache);
			bool far = hInfo.IsBottomLocation || hInfo.IsRightLocation;
			int d1 = far ? 2 : -1;
			int d2 = far ? -2 : 1;
			if(!hInfo.IsSideLocation) {
				bounds.Height = 1;
				bounds.Offset(0, d1);
			} else {
				bounds.Width = 1;
				bounds.Offset(d1, 0);
			}
			e.Cache.Paint.FillRectangle(e.Graphics, border, bounds);
			if(!hInfo.IsSideLocation) {
				bounds.Height = 2;
				bounds.Y += d2;
			} else {
				bounds.Width = 2;
				bounds.X += d2;
			}
			e.Cache.Paint.FillRectangle(e.Graphics, brush, bounds);
		}
	}
	public class Office2003TabPageViewInfo : BaseTabPageViewInfo {
		public Office2003TabPageViewInfo(IXtraTabPage page) : base(page) { }
		protected override AppearanceDefault UpdatePageDefaultAppearance(AppearanceDefault defaultAppearance) { 
			if(ViewInfo != null) {
				defaultAppearance = defaultAppearance.Clone() as AppearanceDefault;
				if(ViewInfo.HeaderInfo.IsSideLocation) 
					defaultAppearance.GradientMode = LinearGradientMode.Horizontal;
				if(ViewInfo.HeaderInfo.IsRightLocation || ViewInfo.HeaderInfo.IsBottomLocation) {
					Color clr2 = defaultAppearance.BackColor2;
					if(!clr2.IsEmpty) {
						defaultAppearance.BackColor2 = defaultAppearance.BackColor;
						defaultAppearance.BackColor = clr2;
					}
				}
			}
			return defaultAppearance;
		}
	}
	public class Office2003TabHeaderButtonPainter : Office2003ButtonInBarsObjectPainter {
		protected override Brush GetNormalBackBrush(ObjectInfoArgs e) {
			ObjectState prev = e.State;
			if(prev == ObjectState.Disabled) e.State = ObjectState.Normal;
			Brush res = base.GetNormalBackBrush(e);
			e.State = prev;
			return res;
		}
	}
}
