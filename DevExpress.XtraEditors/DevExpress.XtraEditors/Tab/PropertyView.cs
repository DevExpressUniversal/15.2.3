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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Drawing;
namespace DevExpress.XtraTab.Drawing {
	public class PropertyViewTabHeaderViewInfo : BaseTabHeaderViewInfo { 
		public override bool UseReversePainter { get { return true; } }
		public override bool DrawSelectedPageLast { get { return true; } }
		public override bool ExcludeFromClipping(BaseTabPageViewInfo info) { return false; }
		public PropertyViewTabHeaderViewInfo(BaseTabControlViewInfo viewInfo) : base(viewInfo) {
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new PropertyViewTabPageViewInfo(page);
		}
		protected internal override EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			return EditorButtonHelper.GetPainter(BorderStyles.UltraFlat);
		}
		public override void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Properties.AppearancePage.HeaderActive }, ViewInfo.GetDefaultAppearance(TabPageAppearance.PageHeaderActive));
		}
		public override AppearanceObject HeaderButtonPaintAppearance { get { return ViewInfo.PaintAppearance; } }
		protected override void NextStartPoint(BaseTabPageViewInfo info, ref Point topLeft) { 
			if(IsSideLocation) {
				if(!IsStandardOrientation) 
					topLeft.Y = info.Bounds.Bottom;
				else
					topLeft.Y = info.Bounds.Bottom - info.Bounds.Width + 4 ;
			} else {
				if(!IsStandardOrientation) 
					topLeft.X = info.Bounds.Right;
				else
					topLeft.X = info.Bounds.Right - info.Bounds.Height + 4;
			}
		}
		protected override void UpdatePageBounds(BaseTabPageViewInfo info) {
			PropertyViewTabPageViewInfo pageInfo = info as PropertyViewTabPageViewInfo;
			pageInfo.PagePath = pageInfo.CalcTabPath(info.Bounds, HeaderLocation, IsStandardOrientation);
			pageInfo.PageRegion = new Region(pageInfo.PagePath);
		}
		protected override int CalcHPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			if(!IsStandardOrientation) return base.CalcHPageIndent(info, indent);
			if(indent == IndentType.Left) return info.Bounds.Height;
			if(indent == IndentType.Right) return 4;
			return base.CalcHPageIndent(info, indent);
		}
		protected override int CalcVPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			if(!IsStandardOrientation) return base.CalcVPageIndent(info, indent);
			if(indent == IndentType.Top) return info.Bounds.Width;
			if(indent == IndentType.Bottom) return 4;
			return base.CalcVPageIndent(info, indent);
		}
		protected override void UpdateClipBounds() {
			foreach(BaseTabRowViewInfo rowInfo in Rows) {
				foreach(PropertyViewTabPageViewInfo info in rowInfo.Pages) {
					info.ClipRegion = GetPageClipRegion(info);
				}
			}
		}
		protected override Size GetPageBoundsByClientSize(BaseTabPageViewInfo info, Size client) {
			Size size = base.GetPageBoundsByClientSize(info, client);
			if(RealPageOrientation == TabOrientation.Horizontal) 
				size.Width += size.Width & 1;
			else
				size.Height += size.Height & 1;
			return size;
		}
		protected virtual Region GetPageClipRegion(PropertyViewTabPageViewInfo info) {
			return null;
		}
		public override bool DefaultShowHeaderFocus { get { return false; } }
		protected internal override bool UseReversedHitTest { get { return true; } }
		protected override Size CalcRowPagesSize(BaseTabRowViewInfo row) {
			Size size = Size.Empty;
			for(int n = 0; n < row.Pages.Count; n++) {
				BaseTabPageViewInfo info = row.Pages[n];
				bool isLast = n == row.Pages.Count - 1;
				Point pt = info.Bounds.Location;
				NextStartPoint(info, ref pt);
				pt.Offset(-info.Bounds.X, -info.Bounds.Y);
				if(isLast) {
					size.Width += info.Bounds.Width;
					size.Height += info.Bounds.Height;
				} else {
					size.Width += pt.X;
					size.Height += pt.Y;
				}
			}
			return size;
		}
		protected override Rectangle CalcPageFocusBounds(BaseTabPageViewInfo info, Rectangle contentBounds) {
			if(!ViewInfo.ShowHeaderFocus) return contentBounds;
			contentBounds.Inflate(IsSideLocation ? 0 : 1, IsSideLocation ? 1 : 0);
			return contentBounds;
		}
		protected override void ButtonsUpdateRowPages() {
			if(ViewInfo.IsMultiLine || Rows.Count == 0) return;
			BaseTabRowViewInfo row = Rows[Rows.Count - 1];
			int lastPage = row.Pages.Count;
			for(int n = 1; n < row.Pages.Count; n++) {
				BaseTabPageViewInfo page = row.Pages[n];
				if(page.Bounds.IntersectsWith(ButtonsBounds)) {
					lastPage = n;
					break;
				}
			}
			for(int n = lastPage; n < row.Pages.Count; n++) {
				row.Pages[n].AllowDraw = false;
			}
		}
	}
	public class PropertyViewTabPainter : BaseTabPainter {
		public PropertyViewTabPainter(IXtraTab tabControl) : base(tabControl) {
		}
		protected override Rectangle GetHeaderClipBounds(TabDrawArgs e) {
			return e.ViewInfo.HeaderInfo.Client;
		}
		protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			PropertyViewTabPageViewInfo pageInfo = pInfo as PropertyViewTabPageViewInfo;
			if(pageInfo.PagePath == null || pageInfo.Bounds.IsEmpty) return;
			e.Cache.Graphics.FillPath(pInfo.PaintAppearance.GetBackBrush(e.Cache, pInfo.Bounds), pageInfo.PagePath);
			e.Cache.Graphics.DrawPath(e.Cache.GetPen(GetBorderColor(pInfo)), pageInfo.PagePath);
			DrawHeaderPageBorder(e, pInfo);
			DrawHeaderFocus(e, pInfo);
			DrawHeaderPageImageText(e, pInfo);
		}
		protected virtual void DrawHeaderPageBorder(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			Rectangle clip = new Rectangle(pInfo.Bounds.X + 1, pInfo.Bounds.Bottom - 1, pInfo.Bounds.Width - 2, 1);
			TabHeaderLocation hloc = e.ViewInfo.HeaderInfo.HeaderLocation;
			bool topBottom = true;
			if(hloc == TabHeaderLocation.Top || hloc == TabHeaderLocation.Bottom) {
				if(hloc == TabHeaderLocation.Bottom)
					clip.Y = pInfo.Bounds.Top;
			} else {
				topBottom = false;
				clip = new Rectangle(pInfo.Bounds.Right - 1, pInfo.Bounds.Top + 1, 1, pInfo.Bounds.Height - 2);
				if(hloc == TabHeaderLocation.Right)
					clip.X = pInfo.Bounds.Left;
			}
			Color clr = e.ViewInfo.HeaderInfo.PaintAppearance.BorderColor;
			if(pInfo.IsActiveState) {
				clr = pInfo.PaintAppearance.BackColor2 == Color.Empty ? pInfo.PaintAppearance.BackColor : pInfo.PaintAppearance.BackColor2;
			} else {
				clip.Inflate(topBottom ? 1 : 0, topBottom ? 0 : 1);
			}
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(clr), clip);
		}
		Color GetBorderColor(BaseTabPageViewInfo pInfo) {
			if(pInfo.PaintAppearance.BorderColor != Color.Empty) return pInfo.PaintAppearance.BorderColor;
			return pInfo.ViewInfo.GetDefaultAppearance(pInfo.IsActiveState ? TabPageAppearance.PageHeaderActive : TabPageAppearance.PageHeader).BorderColor;
		}
		protected override void ExcludeHeaderButtonsClip(TabDrawArgs e) {
			Rectangle clip = e.ViewInfo.HeaderInfo.ButtonsBounds;
			if(e.ViewInfo.HeaderInfo.IsSideLocation) {
				clip.Width--;
				if(e.ViewInfo.HeaderInfo.IsRightLocation) clip.X++;
			}
			else {
				clip.Height--;
				if(e.ViewInfo.HeaderInfo.IsBottomLocation) clip.Y++;
			}
			e.Cache.ClipInfo.ExcludeClip(clip);
		}
		protected override void DrawHeaderButtons(TabDrawArgs e) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			if(hInfo.ButtonsBounds.IsEmpty) return;
			hInfo.Buttons.Draw(e.Cache);
			Rectangle r = hInfo.Buttons.Client;
			Rectangle client = e.ViewInfo.HeaderInfo.Rows.LastRow == null ? e.ViewInfo.HeaderInfo.Client : e.ViewInfo.HeaderInfo.Rows.LastRow.Client;
			if(e.ViewInfo.HeaderInfo.IsSideLocation) {
				r.X = client.X + (e.ViewInfo.HeaderInfo.IsRightLocation ? 1 : 0);
				r.Width = client.Width - (e.ViewInfo.HeaderInfo.IsLeftLocation ? 1 : 0);
			} else {
				r.Y = client.Top + (e.ViewInfo.HeaderInfo.IsBottomLocation ? 1 : 0);
				r.Height = client.Height - (e.ViewInfo.HeaderInfo.IsTopLocation ? 1 : 0);
			}
			e.Graphics.ExcludeClip(r);
		}
	}
	public class PropertyViewTabPageViewInfo : BaseTabPageViewInfo {
		GraphicsPath pagePath = null;
		public PropertyViewTabPageViewInfo(IXtraTabPage page) : base(page) { 
		}
		public override void Clear() {
			base.Clear();
			PagePath = null;
		}
		public GraphicsPath PagePath {
			get { return pagePath; }
			set {
				if(PagePath == value) return;
				if(PagePath != null) pagePath.Dispose();
				pagePath = value;
			}
		}
		protected virtual ITabPathCalculator GetPathCalculator() { return new PropertyTabPath2(); }
		public virtual GraphicsPath CalcTabPath(Rectangle bounds, TabHeaderLocation location, bool standard) {
			if(!standard) {
				GraphicsPath path = new GraphicsPath();
				path.AddRectangle(bounds);
				return path;
			}
			return GetPathCalculator().GetTabPath(bounds, location);
		}
	}
	public class SideHeaderRowBorderPainter : BorderSidePainter {
		protected override BorderSide GetBorderSides(ObjectInfoArgs e) {
			TabBorderObjectInfoArgs ee = e as TabBorderObjectInfoArgs;
			BaseTabControlViewInfo viewInfo = ee == null ? null : ee.ViewInfo;
			if(viewInfo == null) return BorderSide.Bottom;
			if(viewInfo.HeaderInfo.IsSideLocation) {
				return viewInfo.HeaderInfo.IsLeftLocation ? BorderSide.Right : BorderSide.Left;
			} 
			return viewInfo.HeaderInfo.IsTopLocation ? BorderSide.Bottom : BorderSide.Top;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return e.Bounds; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return client; }
	}
	public class PropertyViewClientBorderPainter : RotatedBorderSidePainter {
		protected override int GetDegree(ObjectInfoArgs e) {
			TabBorderObjectInfoArgs ee = e as TabBorderObjectInfoArgs;
			BaseTabControlViewInfo viewInfo = ee == null ? null : ee.ViewInfo;
			if(viewInfo == null) return 180;
			switch(viewInfo.HeaderInfo.HeaderLocation) {
				case TabHeaderLocation.Right : return 270;
				case TabHeaderLocation.Bottom : return 0;
				case TabHeaderLocation.Left : return 90;
			}
			return 180;
		}
	}
	#region Helpers
	public interface ITabPathCalculator {
		GraphicsPath GetTabPath(Rectangle r, TabHeaderLocation tabPos);
	}
	public class PropertyTabPath1 : ITabPathCalculator {
		public GraphicsPath GetTabPath(Rectangle r, TabHeaderLocation tabPos) {
			switch(tabPos) {
				case TabHeaderLocation.Top : 
				case TabHeaderLocation.Bottom :  
					return GetTabPathTopBottom(r, tabPos == TabHeaderLocation.Bottom);
			}
			return GetTabPathLeftRight(r, tabPos == TabHeaderLocation.Right);
		}
		GraphicsPath GetTabPathLeftRight(Rectangle r, bool right) {
			return null;
		}
		GraphicsPath GetTabPathTopBottom(Rectangle r, bool bottom) {
			GraphicsPath myPath = new GraphicsPath();
			int deltaY = r.Height - 3, deltaX = deltaY;
			int dy = 1, dx = 1, angY = 3;
			if(bottom) {
				r.Offset(0, r.Height - 1);
				r.Height = 0 - r.Height;
				deltaY = 0 - deltaY;
				dy = 0; angY = -3;
			}
			Point ps, pe;
			ps = new Point(r.X, r.Bottom - dy); 
			pe = new Point(r.X + deltaX, r.Bottom - dy - deltaY); myPath.AddLine(ps, pe); ps = pe;
			pe = new Point(r.X + deltaX + 5, r.Top); myPath.AddLine(ps, pe); ps = pe;
			pe = new Point(r.Right - dx - 3, r.Top); myPath.AddLine(ps, pe); ps = pe;
			pe = new Point(r.Right - dx, r.Top + angY); myPath.AddLine(ps, pe); ps = pe;
			pe = new Point(r.Right - dx, r.Bottom - dy); myPath.AddLine(ps, pe); ps = pe;
			pe = new Point(r.X, r.Bottom - dy); myPath.AddLine(ps, pe); ps = pe;
			return myPath;
		}
	}
	public class PropertyTabPath2 : ITabPathCalculator {
		public GraphicsPath GetTabPath(Rectangle bounds, TabHeaderLocation tabPos) {
			int degrees = 0;
			bool mirrorX = false, mirrorY = false;
			switch(tabPos) {
				case TabHeaderLocation.Right : degrees = 270; 
					bounds.Size = new Size(bounds.Height, bounds.Width);
					bounds.Offset(bounds.Height - 1, 0);
					break;
				case TabHeaderLocation.Left: degrees = 90; 
					bounds.Size = new Size(bounds.Height, bounds.Width);
					bounds.Offset(0, bounds.Width - 1);
					mirrorX = true;
					break;
				case TabHeaderLocation.Bottom: degrees = 180; 
					bounds.Offset(bounds.Width, bounds.Height - 1);
					mirrorX = true;
					break;
			}
			return GetTabPath(bounds, degrees, mirrorX, mirrorY);
		}
		GraphicsPath GetTabPath(Rectangle r, int degrees, bool mirrorX, bool mirrorY) {
			GraphicsPath myPath = new GraphicsPath();
			int deltaY = r.Height - 3, deltaX = deltaY;
			int dy = 1, dx = 1, angY = 3;
			Point ps, pe, offset = Rotate(degrees, Point.Empty, new PointF(mirrorX ? 9999 : 0, mirrorY ? 9999: 0), r.X, r.Y); 
			offset.Offset(-r.X, -r.Y);
			PointF center = PointF.Empty;
			if(mirrorX) center.X = (float)r.Width / 2f;
			if(mirrorY) center.Y = (float)r.Height / 2f;
			ps = Rotate(degrees, offset, center, r.X, r.Bottom - dy); 
			pe = Rotate(degrees, offset, center, r.X + deltaX, r.Bottom - dy - deltaY); myPath.AddLine(ps, pe); ps = pe;
			pe = Rotate(degrees, offset, center, r.X + deltaX + 5, r.Top); myPath.AddLine(ps, pe); ps = pe;
			pe = Rotate(degrees, offset, center, r.Right - dx - 3, r.Top); myPath.AddLine(ps, pe); ps = pe;
			pe = Rotate(degrees, offset, center, r.Right - dx, r.Top + angY); myPath.AddLine(ps, pe); ps = pe;
			pe = Rotate(degrees, offset, center, r.Right - dx, r.Bottom - dy); myPath.AddLine(ps, pe); ps = pe;
			pe = Rotate(degrees, offset, center, r.X, r.Bottom - dy); myPath.AddLine(ps, pe); ps = pe;
			return myPath;
		}
		Point Rotate(int degrees, Point offset, PointF center, int x, int y) {
			if(degrees == 0) return new Point(x, y);
			if(!center.IsEmpty) {
				if(center.X != 0) x = -x + (center.X == 9999 ? 0 : (int)(2*center.X));
				if(center.Y != 0) y = -y + (center.Y == 9999 ? 0 : (int)(2*center.Y));
			}
			float cos = (float)Math.Round(Math.Cos(degrees * (Math.PI / 180)), 2);
			float sin = (float)Math.Round(Math.Sin(degrees * (Math.PI / 180)), 2);
			Point p = Point.Empty;
			p.X = (int)(x * cos + y * sin);
			p.Y = (int)(x * (-sin) + y * cos);
			p.Offset(-offset.X, -offset.Y);
			return p;
		}
	}
	#endregion
}
