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
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Drawing;
namespace DevExpress.XtraTab.Drawing {
	public class FlatTabHeaderViewInfo : BaseTabHeaderViewInfo { 
		AppearanceObject headerButtonPaintAppearance;
		public FlatTabHeaderViewInfo(BaseTabControlViewInfo viewInfo) : base(viewInfo) { 
			this.headerButtonPaintAppearance = new AppearanceObject();
		}
		public override AppearanceObject HeaderButtonPaintAppearance { get { return headerButtonPaintAppearance; } }
		public override void UpdatePaintAppearance() {
			bool isBottom = HeaderLocation == TabHeaderLocation.Bottom || HeaderLocation == TabHeaderLocation.Right;
			TabPageAppearance app = isBottom ? TabPageAppearance.PageHeader : TabPageAppearance.PageHeaderActive;
			AppearanceObject appearance = Properties.AppearancePage.Header;
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { appearance }, (new AppearanceDefault(SystemColors.ControlText, DevExpress.Utils.ColorUtils.FlatTabBackColor, 
				ViewInfo.GetDefaultAppearance(app).BorderColor)));
			appearance = isBottom ? Properties.AppearancePage.Header : Properties.AppearancePage.HeaderActive;
			if(!appearance.BorderColor.IsEmpty) 
				PaintAppearance.BorderColor = appearance.BorderColor;
			AppearanceHelper.Combine(HeaderButtonPaintAppearance, new AppearanceObject[] { Properties.AppearancePage.Header }, (new AppearanceDefault(SystemColors.ControlText, DevExpress.Utils.ColorUtils.FlatTabBackColor, 
				ViewInfo.GetDefaultAppearance(TabPageAppearance.PageHeaderActive).BorderColor)));
		}
		public override bool DefaultShowHeaderFocus { get { return false; } }
		protected internal override EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			return new EditorButtonPainter(new FlatTabHeaderButtonPainter());
		}
		protected override void UpdatePageBounds(BaseTabPageViewInfo info) { 
			if(!info.IsActiveState) return;
			Rectangle r = info.Bounds;
			if(IsSideLocation) {
				r.Inflate(0, 1);
			} else {
				r.Inflate(1, 0);
			}
			info.Bounds = r;
		}
		protected override TabOrientation CalcOrientation() { return IsSideLocation ? TabOrientation.Vertical : TabOrientation.Horizontal; }
	}
	public class FlatTabPainter : BaseTabPainter {
		BorderPainter borderPainter;
		public FlatTabPainter(IXtraTab tabControl) : base(tabControl) { 
			this.borderPainter = CreateBorderPainter();
		}
		protected override Rectangle GetHeaderClipBounds(TabDrawArgs e) {
			return e.ViewInfo.HeaderInfo.Client;
		}
		protected virtual int FlatBorderThin { get { return 2; } }
		protected override void ExcludeHeaderButtonsClip(TabDrawArgs e) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			Rectangle r = e.ViewInfo.HeaderInfo.ButtonsBounds;
			if(r.IsEmpty) return;
			r.Size = hInfo.SetSizeHeight(r.Size, hInfo.GetSizeHeight(r.Size) - FlatBorderThin);
			if(hInfo.IsBottomLocation) r.Y += FlatBorderThin;
			if(hInfo.IsRightLocation) r.X += FlatBorderThin;
			e.Cache.ClipInfo.ExcludeClip(r);
		}
		protected virtual BorderPainter CreateBorderPainter() { return new FlatTabBorderPainter(); }
		protected override void DrawHeaderBackground(TabDrawArgs e) {
			base.DrawHeaderBackground(e);
			BaseTabHeaderViewInfo headerInfo = e.ViewInfo.HeaderInfo;
			headerInfo.PaintAppearance.FillRectangle(e.Cache, headerInfo.Client);
		}
		protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			if(pInfo.IsActiveState) {
				DrawActiveHeaderPage(e, pInfo);
				return;
			}
			DrawPageSeparator(e, pInfo);
			DrawHeaderPageImageText(e, pInfo);
		}
		protected virtual void DrawPageSeparator(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			Rectangle border = new Rectangle(pInfo.Bounds.Right - 1, pInfo.Bounds.Y + 2, 1, pInfo.Bounds.Height - 3);
			if(e.ViewInfo.HeaderInfo.IsSideLocation) 
				border = new Rectangle(pInfo.Bounds.X + 2, pInfo.Bounds.Bottom - 1, pInfo.Bounds.Width - 3, 1);
			e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(SystemColors.ControlDark), border);
		}
		protected virtual void DrawActiveHeaderPage(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			Rectangle pageBounds = pInfo.Bounds;
			pInfo.PaintAppearance.DrawBackground(e.Cache, pageBounds);
			this.borderPainter.DrawObject(new FlatTabBorderObjectInfoArgs(e.ViewInfo, pInfo, e.Cache, pageBounds));
			DrawHeaderFocus(e, pInfo);
			DrawHeaderPageImageText(e, pInfo);
		}
	}
	public class FlatTabBorderPainter : RotatedTabBorderPainter {
		protected override Brush GetBrush(ObjectInfoArgs e, BorderSide side) {
			int degree = GetDegree(e);
			FlatTabBorderObjectInfoArgs ee = e as FlatTabBorderObjectInfoArgs;
			if(ee == null) return base.GetBrush(e, side);
			Brush dark = ee.Page.GetPageAppearance(ObjectState.Normal).GetBorderBrush(e.Cache),
				light = ee.Page.PaintAppearance.GetBorderBrush(e.Cache);
			if(ee.ViewInfo.HeaderInfo.IsBottomLocation || 
				ee.ViewInfo.HeaderInfo.IsRightLocation) {
				Brush temp = dark;
				dark = light;
				light = temp;
			}
			if(degree ==   0 && side == BorderSide.Right) return dark;
			if(degree ==  90 && side == BorderSide.Top) return dark;
			if(degree == 180 && side == BorderSide.Left) return dark;
			if(degree == 270 && side == BorderSide.Bottom) return dark;
			return light;
		}
	}
	public class FlatTabBorderObjectInfoArgs : TabBorderObjectInfoArgs {
		BaseTabPageViewInfo page;
		public FlatTabBorderObjectInfoArgs(BaseTabControlViewInfo viewInfo, BaseTabPageViewInfo page, GraphicsCache cache, Rectangle bounds) : base(viewInfo, cache, page.PaintAppearance, bounds, ObjectState.Normal) { 
			this.page = page;
		}
		public BaseTabPageViewInfo Page { get { return page; } }
	}
	public class FlatTabHeaderButtonPainter : FlatButtonObjectPainter {
		protected override void DrawNormal(ObjectInfoArgs e, AppearanceObject style) {
			Rectangle r = e.Bounds;
			r.X ++;	r.Y ++;	r.Width --; r.Height --;
			r.X ++;	r.Y ++;	r.Width --; r.Height --;
			r.Width --; r.Height --;
			DrawButtonBackground(e, style, r);
		}
		protected override void DrawHot(ObjectInfoArgs e, AppearanceObject style) {
			base.DrawNormal(e, style);
		}
	}
	public class FlatTabHeaderRowBorderPainter : BorderSidePainter {
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
		protected override void DrawSide(ObjectInfoArgs e, BorderSide side, Rectangle bounds) {
			BaseTabControlViewInfo viewInfo = GetTabInfo(e);
			if(viewInfo == null) return;
			viewInfo.HeaderInfo.PaintAppearance.FillRectangle(e.Cache, bounds);
		}
	}
}
