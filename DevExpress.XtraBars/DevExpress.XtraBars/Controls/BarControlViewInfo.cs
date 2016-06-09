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
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Skins;
namespace DevExpress.XtraBars.ViewInfo {
	public class CustomControlViewInfo : CustomViewInfo, IAppearanceOwner {
		CustomPainter painter;
		CustomControl barControl;
		protected Rectangle fBounds;
		StateAppearances appearance;
		public CustomControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl barControl) : base(manager, parameters) {
			this.barControl = barControl;
			this.painter = Manager.Helper.GetControlPainter(BarControl.GetType());
			this.appearance = new StateAppearances(this);
			Clear();
		}
		protected internal virtual bool IsRightToLeft { get { return false; } }
		public virtual bool IsAllowDrawBackground { get { return false; } }
		public virtual bool IsDrawForeground { get { return true; } }
		public virtual CustomPainter Painter { get { return painter; } }
		public virtual CustomControl BarControl { get { return barControl; } }
		public virtual Rectangle Bounds { get { return fBounds; } }
		public virtual StateAppearances Appearance { get { return appearance; } }
		protected virtual void UpdateAppearance() {
			Appearance.Reset();
			Appearance.UpdateRightToLeft(IsRightToLeft);
		}
		protected override void Update() {
			UpdateAppearance();
		}
		public override void Clear() {
			this.fBounds = Rectangle.Empty;
			base.Clear();
		}
		public int CalcMinWidth(Graphics g, object sourceObject) {
			Size size = CalcMinSize(g, sourceObject);
			return (IsVertical ? size.Height: size.Width);
		}
		public virtual Size CalcMinSize(Graphics g, object sourceObject) {
			return Size.Empty;
		}
		public virtual Size CalcBarSize(Graphics g, object sourceObject, int width, int maxHeight) {
			return Size.Empty;
		}
		public virtual Region GetBarLinksRegion(object sourceObject) {
			return null;
		}
		public virtual BarLinkViewInfo GetLinkViewInfo(BarItemLink link, LinkViewInfoRange infoRange) {
			return null;
		}
		public virtual BarLinkViewInfo GetLinkViewInfoByPoint(Point p, bool includeSeparator) {
			return null;
		}
		public virtual bool IsVertical { get { return BarControl.IsVertical; } }
		public virtual void UpdateControlRegion(Control control) {
		}
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
	}
	public class DockControlViewInfo : CustomControlViewInfo {
		public DockControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl barControl) : base(manager, parameters, barControl) {
		}
		protected override void UpdateAppearance() {
			AppearanceHelper.Combine(Appearance.Normal, new AppearanceObject[] { BarControl.Appearance, Manager.GetController().AppearancesBar.Dock, DrawParameters.Appearance(BarAppearance.Dock)});
			if(BarControl != null && !BarControl.AllowTransparencyCore) {
				Appearance.Normal.BackColor = AppearanceHelper.RemoveTransparency(Appearance.Normal.BackColor, DrawParameters.Appearance(BarAppearance.Dock).BackColor);
				Appearance.Normal.BackColor2 = AppearanceHelper.RemoveTransparency(Appearance.Normal.BackColor2, DrawParameters.Appearance(BarAppearance.Dock).BackColor2);
			}
			Appearance.UpdateRightToLeft(IsRightToLeft);
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle r) {
			this.fBounds = r;
			base.CalcViewInfo(g, sourceObject, r);
		}
		public new BarDockControl BarControl { get { return base.BarControl as BarDockControl; } }
	}
	public class BarControlRowViewInfo : CustomViewInfo {
			BarControlViewInfo barInfo;
			ArrayList links;
			Rectangle bounds;
			public BarControlRowViewInfo(BarManager manager, BarControlViewInfo ViewInfo) : base(manager, null) {
				this.barInfo = ViewInfo;
				this.bounds = Rectangle.Empty;
				this.links = new ArrayList();
			}
			public virtual BarControlViewInfo BarInfo { get { return barInfo; } }
			public virtual ArrayList Links { get { return links; } }
			public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		}
	public class CustomLinksControlViewInfo : CustomControlViewInfo {
		public CustomLinksControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		protected virtual BarAppearance DefaultBarApperanceStyle {
			get {
				return BarAppearance.Bar;
			}
		}
		protected internal override bool IsRightToLeft { get { return Manager != null && Manager.IsRightToLeft; } }
		protected virtual BarAppearance DefaultBarAppearanceHoveredStyle {
			get {
				return BarAppearance.BarHovered;
			}
		}
		protected virtual BarAppearance DefaultBarAppearancePressedStyle {
			get {
				return BarAppearance.BarPressed;
			}
		}
		protected virtual BarAppearance DefaultBarAppearanceDisabledStyle {
			get {
				return BarAppearance.BarDisabled;
			}
		}
		protected virtual StateAppearances DefaultBarAppearance {
			get {
				StateAppearances res = null;
				BarAppearance style = DefaultBarApperanceStyle;
				switch(style) {
					case BarAppearance.MainMenu: res = Manager.GetController().AppearancesBar.MainMenuAppearance; break;
					case BarAppearance.StatusBar: res = Manager.GetController().AppearancesBar.StatusBarAppearance; break;
					default: res = Manager.GetController().AppearancesBar.BarAppearance; break;
				}
				return res;
			}
		}
		protected override void UpdateAppearance() {
			Appearance.Combine(new StateAppearances[] { DefaultBarAppearance, Manager.GetController().PaintStyle.DrawParameters.Colors.StateAppearance(DefaultBarApperanceStyle) });
			Appearance.UpdateRightToLeft(IsRightToLeft);
		}
	}
	public class BarControlViewInfo : CustomLinksControlViewInfo {
		Hashtable linkSizes;
		BarItemLinkReadOnlyCollection notVisibleLinks;
		BarItemLinkReadOnlyCollection reallyVisibleLinks;
		ArrayList rows;
		Rectangle dragBorder, sizeGrip;
		SizeGripObjectPainter sizeGripPainter;
		public BarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
			this.linkSizes = new Hashtable();
			this.rows = new ArrayList();
			this.reallyVisibleLinks = new BarItemLinkReadOnlyCollection();
			this.notVisibleLinks = new BarItemLinkReadOnlyCollection();
			this.sizeGripPainter = CreateSizeGripPainter();
			Clear();
		}
		protected virtual SizeGripObjectPainter CreateSizeGripPainter() {
			return Manager.PaintStyle.LinksLookAndFeel.Painter.SizeGrip;
		}
		public virtual bool DrawTransparent { get { return false; } }
		public new BarPainter Painter { get { return base.Painter as BarPainter; } }
		public SizeGripObjectPainter SizeGripPainter { get { return sizeGripPainter; } }
		public virtual Bar Bar { 
			get { 
				CustomBarControl bc = BarControl as CustomBarControl;
				if(bc != null) return bc.Bar;
				return null;
			}
		}
		Color GetTransparentForeColor() {
			return LookAndFeelHelper.CheckTransparentForeColor(Manager.GetController().LookAndFeel.ActiveLookAndFeel, Color.Transparent, BarControl);
		}
		protected override void UpdateAppearance() {
			if(Bar == null) return;
			StateAppearances skinAppearance = Manager.GetController().PaintStyle.DrawParameters.Colors.StateAppearance(DefaultBarApperanceStyle);
			if(Bar != null && !Bar.OptionsBar.DrawBorder && Bar.DockControl != null && Bar.DockControl.AllowTransparencyCore) {
				skinAppearance = (StateAppearances)((ICloneable)skinAppearance).Clone();
				skinAppearance.Normal.ForeColor = GetTransparentForeColor();
				skinAppearance.Hovered.ForeColor = GetTransparentForeColor();
				skinAppearance.Pressed.ForeColor = GetTransparentForeColor();
			}
			Appearance.Combine(new StateAppearances[] { Bar.BarAppearance, DefaultBarAppearance, skinAppearance });
			Appearance.UpdateRightToLeft(IsRightToLeft);
		}
		protected override BarAppearance DefaultBarApperanceStyle {
			get {
				if(Bar == null) return BarAppearance.Bar;
				if(Bar.IsFloating) return BarAppearance.Bar;
				if(!Bar.OptionsBar.DrawBorder)
					return BarAppearance.BarNoBorder;
				if(Bar.IsMainMenu) return BarAppearance.MainMenu;
				if(Bar.IsStatusBar) return BarAppearance.StatusBar;
				return BarAppearance.Bar;
			}
		}
		public virtual bool LinksUseRealBounds { get { return false; } }
		public new virtual CustomLinksControl BarControl { get { return base.BarControl as CustomLinksControl; } }
		public virtual ArrayList Rows { get { return rows; } }
		public virtual bool DrawDragBorder {
			get { 
				return Bar != null && Bar.OptionsBar.DrawDragBorder && Bar.OptionsBar.DrawBorder;  
			} 
		}
		public virtual bool DrawSizeGrip { 
			get { 
				if(Bar != null && Bar.DockStyle == BarDockStyle.Bottom && Bar.IsStatusBar && Bar.OptionsBar.DrawSizeGrip) {
					Form form = Manager.Form as Form;
					if(form == null) return false;
					if(form.FormBorderStyle == FormBorderStyle.Sizable || form.FormBorderStyle == FormBorderStyle.SizableToolWindow) {
						if(form.WindowState == FormWindowState.Maximized) return false;
						return true;
					}
					return false;
				}
				return false;
			} 
		}
		public virtual BarItemLinkReadOnlyCollection NotVisibleLinks { get { return notVisibleLinks; } } 
		public virtual BarItemLinkReadOnlyCollection ReallyVisibleLinks { get { return reallyVisibleLinks; } } 
		public virtual Rectangle DragBorder { 
			get { return dragBorder; }
			set { dragBorder = value; }
		}
		public virtual Rectangle SizeGrip { 
			get { return sizeGrip; }
			set { sizeGrip = value; }
		}
		public virtual bool RotateWhenVertical {
			get { return Bar != null && IsVertical && Bar.OptionsBar.RotateWhenVertical; }
		}
		public override void UpdateControlRegion(Control control) {
			if(control == null || !control.IsHandleCreated || Bar == null) return;
			if(Bar.IsMainMenu || Bar.IsStatusBar) {
				control.Region = null;
				return;
			}
			if(Bar.IsFloating) return;
			Region reg = new Region(control.ClientRectangle);
			Rectangle r = new Rectangle(0, 0, 1, 1);
			reg.Exclude(r);
			r.X = control.ClientRectangle.Right - 1;reg.Exclude(r);
			r.Y = control.ClientRectangle.Bottom - 1;reg.Exclude(r);
			r.X = control.ClientRectangle.X;reg.Exclude(r);
			control.Region = reg;
		}
		protected virtual bool IsAlwaysBestFit { get { return false; } }
		public override void Clear() {
			base.Clear();
			this.dragBorder = Rectangle.Empty;
			this.sizeGrip = Rectangle.Empty;
			if(Rows != null) 
				this.Rows.Clear();
			if(NotVisibleLinks != null)
				NotVisibleLinks.ClearItems();
		}
		public virtual void OnLinkStateUpdated(BarLinkViewInfo updatedLink) {
		}
		public override Size CalcBarSize(Graphics g, object sourceObject, int width, int maxHeight) {
			Update();
			bool freeGraphics = g == null;
			RefreshReallyVisibleLinks();
			if(g == null)
				g = Graphics.FromHwnd(IntPtr.Zero);
			ArrayList rows = PreCalcBarDrawInfo(g, sourceObject, width);
			Size res = Size.Empty;
			int minX, maxX, minY, maxY;
			minX = maxX = minY = maxY = 0;
			for(int n = 0; n < rows.Count; n++) {
				BarControlRowViewInfo rowInfo = rows[n] as BarControlRowViewInfo;
				minX = Math.Min(rowInfo.Bounds.X, n == 0 ? 999 : minX);
				minY = Math.Min(rowInfo.Bounds.Y, n == 0 ? 999 : minY);
				maxX = Math.Max(rowInfo.Bounds.Right, maxX);
				maxY = Math.Max(rowInfo.Bounds.Bottom, maxY);
			}
			res.Width = maxX - minX;
			res.Height = maxY - minY;
			if(IsShowCustomizationLink(rows)) {
				if(IsVertical) {
					res.Height += DrawParameters.Constants.BarQuickButtonWidth;
					res.Width = Math.Max(res.Width, PaintStyle.CalcBarCustomizationLinkMinHeight(g, sourceObject, IsVertical, IsMainMenu));
				}
				else {
					res.Width += DrawParameters.Constants.BarQuickButtonWidth;
					res.Height = Math.Max(res.Height, PaintStyle.CalcBarCustomizationLinkMinHeight(g, sourceObject, IsVertical, IsMainMenu));
				}
			}
			if(!IsVertical) {
				res.Width += CalcIndent(BarIndent.Left) + CalcIndent(BarIndent.Right);
				res.Height += CalcIndent(BarIndent.Top) + CalcIndent(BarIndent.Bottom);
				res.Width += CalcIndent(BarIndent.DragBorder) + CalcIndent(BarIndent.SizeGrip);
			}
			else {
				res.Width += CalcIndent(BarIndent.Top) + CalcIndent(BarIndent.Bottom);
				res.Height += CalcIndent(BarIndent.Left) + CalcIndent(BarIndent.Right);
				res.Height += CalcIndent(BarIndent.DragBorder) + CalcIndent(BarIndent.SizeGrip);
			}
			int resultWidth = IsVertical ? res.Height : res.Width;
			if(!IsAlwaysBestFit && width > -1 && resultWidth < width) {
				if(!IsAllItemsFit(rows) || rows.Count > 1) { 
					if(IsVertical)
						res.Height = width;
					else
						res.Width = width;
				}
			}
			if(Bar != null) {
				if(IsVertical)
					res.Width = Math.Max(res.Width, Bar.OptionsBar.MinHeight);
				else
					res.Height = Math.Max(res.Height, Bar.OptionsBar.MinHeight);
			}
			rows.Clear();
			if(freeGraphics) g.Dispose();
			return res;
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			Clear();
			this.SourceObject = sourceObject;
			this.fBounds = rect;
			Update();
			RefreshReallyVisibleLinks();
			GInfo.AddGraphics(g);
			try {
				if(IsVertical) 
					CalcVerticalViewInfo(GInfo.Graphics, sourceObject, rect);
				else
					CalcHorizontalViewInfo(GInfo.Graphics, sourceObject, rect);
				RemoveInvisibleAnimatedItems();
				CheckUpdateEditorBounds();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			this.fBounds = rect;
			this.ready = true;
		}
		protected virtual void CheckUpdateEditorBounds() {
			if(Manager.ActiveEditor == null || Manager.ActiveEditItemLink == null || 
				Manager.ActiveEditItemLink.LinkViewInfo == null)
				return;
			if(Manager.ActiveEditItemLink.Item.Visibility != BarItemVisibility.Always || Manager.ActiveEditor.Bounds != ((BarEditLinkViewInfo)Manager.ActiveEditItemLink.LinkViewInfo).EditRectangle) {
				Manager.SelectionInfo.CloseEditor();
			}
		}
		public override BarLinkViewInfo GetLinkViewInfo(BarItemLink link, LinkViewInfoRange infoRange) {
			bool found = false;
			BarLinkViewInfo prevInfo = null;
			foreach(BarControlRowViewInfo rowInfo in Rows) {
				foreach(BarLinkViewInfo linkInfo in rowInfo.Links) {
					if(found && infoRange == LinkViewInfoRange.Next) return linkInfo;
					if(linkInfo.Link == link) {
						found = true;
						if(infoRange != LinkViewInfoRange.Next) 
							return (infoRange == LinkViewInfoRange.Current ? linkInfo : prevInfo);
					}
					prevInfo = linkInfo;
				}
			}
			return null;
		}
		public override BarLinkViewInfo GetLinkViewInfoByPoint(Point p, bool includeSeparator) {
			if(!IsReady) return null;
			foreach(BarControlRowViewInfo rowInfo in Rows) {
				for(int i = rowInfo.Links.Count - 1; i >= 0; i--) {
					BarLinkViewInfo info = rowInfo.Links[i] as BarLinkViewInfo;
					if(info.Bounds.Contains(p)) return info;
					foreach(RectInfo rInfo in info.Rects.ExtRectangles) {
						if(includeSeparator && rInfo.Rect.Contains(p)) {
							if((rInfo.Type == RectInfoType.HorzSeparator && IsVertical) ||
								(rInfo.Type == RectInfoType.VertSeparator && !IsVertical)) return info;
						}
					}
				}
			}
			return null;
		}
		public override Region GetBarLinksRegion(object sourceObject) {
			Region res = new Region(this.Bounds);
			foreach(BarControlRowViewInfo rowInfo in Rows) {
				foreach(BarLinkViewInfo info in rowInfo.Links) {
					res.Exclude(info.Bounds);
					foreach(RectInfo rInfo in info.Rects.ExtRectangles) {
						res.Exclude(rInfo.Rect);
					}
				}
			}
			return res;
		}
		protected void RefreshReallyVisibleLinks() {
			ReallyVisibleLinks.ClearItems();
			ReallyVisibleLinks.AddLinkRange(BarControl.VisibleLinks);
			if(!IsAllowQuickCustomization(SourceObject) && ReallyVisibleLinks.Count > 0 && ReallyVisibleLinks[ReallyVisibleLinks.Count - 1] is BarQBarCustomizationItemLink) {
				ReallyVisibleLinks.RemoveItemAt(ReallyVisibleLinks.Count - 1);
			}
			if(Bar!= null && Bar.OptionsBar.BarState == BarState.Collapsed && !Bar.AnimatedBounds && ReallyVisibleLinks.Count > 0) {
				BarItemLink qlink = ReallyVisibleLinks[ReallyVisibleLinks.Count - 1] as BarQBarCustomizationItemLink;
				ReallyVisibleLinks.ClearItems();
				if(qlink != null) ReallyVisibleLinks.AddItem(qlink);
			}
		}
		BarControlRowViewInfo CreateRowViewInfo(ArrayList rows) {
			BarControlRowViewInfo vi = new BarControlRowViewInfo(Manager, this);
			rows.Add(vi);
			return vi;
		}
		public virtual bool IsMainMenu { get { return Bar != null && Bar.IsMainMenu; } }
		public virtual bool IsStatusBar { get { return Bar != null && Bar.IsStatusBar; } }
		public virtual int VertIndent { get { return DrawParameters.Constants.BarLinksVertIndent; } }
		public virtual int HorzIndent { get { return DrawParameters.Constants.BarLinksHorzIndent; } }
		protected virtual int CalcIndent(BarIndent indent) { return Painter.CalcIndent(indent, this); }
		public virtual Size CalcSizeGripSize() {
			Size res = Size.Empty;
			if(!DrawSizeGrip) return res;
			GInfo.AddGraphics(null);
			try {
				res = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SizeGripPainter, new SizeGripObjectInfoArgs()).Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected virtual Rectangle CalcSizeGrip(Rectangle rect) {
			Rectangle res = Rectangle.Empty;
			Size size = CalcSizeGripSize();
			if(size.IsEmpty) return res;
			res.Size = size;
			res.Y = rect.Bottom - size.Height - CalcIndent(BarIndent.Bottom);
			if(IsRightToLeft) {
				res.X = rect.X + CalcIndent(BarIndent.Right);
			} else
			res.X = rect.Right - CalcIndent(BarIndent.Right) - CalcIndent(BarIndent.SizeGrip);
			return res;
		}
		protected virtual Rectangle CalcDragBorder(Rectangle rect) {
			Rectangle res = Rectangle.Empty;
			if(!DrawDragBorder) return res;
			res = rect;
			if(IsVertical) {
				res.Height = DrawParameters.Constants.DragBorderSize;
			} else {
				res.Width = DrawParameters.Constants.DragBorderSize;
				if(IsRightToLeft) {
					res.X = rect.Right - DrawParameters.Constants.DragBorderSize;
				}
			}
			return res;
		}
		protected virtual ArrayList PreCalcBarDrawInfo(Graphics g, object sourceObject, int width) {
			ArrayList list = null;
			int restWidth = 0;
			int qButtonWidth = !IsAllowQuickCustomization(sourceObject) && IsAllowQuickCustomizationNonFit(sourceObject)? DrawParameters.Constants.BarQuickButtonWidth : 0;
			if(width < qButtonWidth) qButtonWidth = 0;
			while(list == null) {
				list = PreCalcMultiLineSize(g, sourceObject, width, ref restWidth);
				if(list == null) {
					BarControl.ExcludeRarelyItemFromVisible(ReallyVisibleLinks);
					width -= qButtonWidth;
					qButtonWidth = 0;
				}
			}
			return list;
		}
		protected virtual void CalcQuickCustomizationInfo(Graphics g, object sourceObject, Rectangle rect, ref  int separatorWidth) {
		}
		protected virtual void UpdateLinkRectsByRealRects(BarLinkViewInfo itemInfo) {
			itemInfo.Bounds = itemInfo.Rects.RealBounds;
			itemInfo.SelectRect = itemInfo.Rects.RealBounds;
			if(itemInfo.IsDrawPart(BarLinkParts.OpenArrow)) {
				if(IsVertical && itemInfo.IsDrawVerticalRotated) {
					int arrowWidth = itemInfo.Rects[BarLinkParts.OpenArrow].Width;
					int x = itemInfo.Rects[BarLinkParts.OpenArrow].X;
					if(itemInfo is BarSubItemLinkViewInfo) {
						arrowWidth = itemInfo.SelectRect.Width;
						x = itemInfo.SelectRect.X;
					}
					itemInfo.Rects[BarLinkParts.OpenArrow] = new Rectangle(new Point(x, itemInfo.SelectRect.Bottom - itemInfo.Rects[BarLinkParts.OpenArrow].Height), new Size(arrowWidth, itemInfo.Rects[BarLinkParts.OpenArrow].Height));
				}
				else
				itemInfo.Rects[BarLinkParts.OpenArrow] = new Rectangle(new Point(itemInfo.Rects[BarLinkParts.OpenArrow].X, itemInfo.SelectRect.Y), new Size(itemInfo.Rects[BarLinkParts.OpenArrow].Width, itemInfo.SelectRect.Height));
		}
		}
		protected internal override bool IsRightToLeft { get { return Manager != null && Manager.IsRightToLeft; } }
		protected virtual void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			this.rows = PreCalcBarDrawInfo(g, sourceObject, IsVertical ? rect.Height : rect.Width);
			this.dragBorder = CalcDragBorder(rect);
			this.sizeGrip = CalcSizeGrip(rect);
			Point offs = rect.Location;
			Point linkOffs;
			if(IsVertical) {
				offs.Y += CalcIndent(BarIndent.Left) + CalcIndent(BarIndent.DragBorder);
				rect.Height -= (CalcIndent(BarIndent.Right) + CalcIndent(BarIndent.SizeGrip));
				linkOffs = offs;
			} else {
				offs.X += CalcIndent(BarIndent.Left) + CalcIndent(BarIndent.DragBorder);
				linkOffs = offs;
				if(IsRightToLeft) {
					rect.X += (CalcIndent(BarIndent.Right) + CalcIndent(BarIndent.SizeGrip));
					rect.Width -= CalcIndent(BarIndent.Left) + CalcIndent(BarIndent.DragBorder);
					offs.X = CalcIndent(BarIndent.Left); 
					linkOffs.X = offs.X * -1;
				}
				else {
					rect.Width -= (CalcIndent(BarIndent.Right) + CalcIndent(BarIndent.SizeGrip));
				}
			}
			int separatorWidth = IsVertical ? rect.Height : rect.Width;
			int separatorWidthOffs = 0;
			CalcQuickCustomizationInfo(g, sourceObject, rect, ref separatorWidth);
			foreach(BarControlRowViewInfo rowInfo in Rows) {
				RealignRowViewInfo(rowInfo, separatorWidth - (IsVertical ? offs.Y : offs.X));
				if(IsRightToLeft) {
					ReverseRightToLeft(rowInfo, rect);
				}
				Rectangle r = rowInfo.Bounds;
				r.Offset(offs);
				rowInfo.Bounds = r;
				foreach(BarLinkViewInfo itemInfo in rowInfo.Links) {
					if(!(itemInfo.Link is BarQBarCustomizationItemLink)) { 
						Rectangle br = itemInfo.Bounds;
						br.Offset(linkOffs);
						itemInfo.Bounds = br;
						Rectangle rBounds;
						if(IsVertical)
							rBounds = new Rectangle(rowInfo.Bounds.X, br.Y, rowInfo.Bounds.Width, br.Height);
						else 
							rBounds = new Rectangle(br.X, rowInfo.Bounds.Y, br.Width, rowInfo.Bounds.Height);
						itemInfo.Rects.RealBounds = rBounds;
						if(IsVertical) {
							if(!itemInfo.IsDrawVerticalRotated) {
								br.X = rowInfo.Bounds.X;
								br.Width = rowInfo.Bounds.Width;
							}
							else {
								br.Offset((rowInfo.Bounds.Width - itemInfo.Bounds.Width) / 2, 0);
							}
						} else {
							br.Offset(0, (rowInfo.Bounds.Height - itemInfo.Bounds.Height) / 2);
						}
						itemInfo.Bounds = br;
					}
					itemInfo.CalcViewInfo(g, sourceObject, itemInfo.Bounds);
					itemInfo.Link.SetLinkViewInfo(itemInfo);
					if(LinksUseRealBounds && !itemInfo.Rects.RealBounds.IsEmpty) {
						UpdateLinkRectsByRealRects(itemInfo);
					}
					foreach(RectInfo rInfo in itemInfo.Rects.ExtRectangles) {
						if(rInfo.Type != RectInfoType.BackGround) 
							rInfo.Rect.Offset(linkOffs);
						if(rInfo.Type == RectInfoType.VertSeparator) {
							if(IsVertical) {
								rInfo.Rect.Height = separatorWidth;
								rInfo.Rect.Offset(0, separatorWidthOffs);
							} else 
								rInfo.Rect.Height = rowInfo.Bounds.Height;
						}
						if(rInfo.Type == RectInfoType.HorzSeparator) {
							if(IsVertical) 
								rInfo.Rect.Width = rowInfo.Bounds.Width;
							else {
								rInfo.Rect.Width = separatorWidth;
								rInfo.Rect.Offset(separatorWidthOffs, 0);
							}
						}
					}
				}
			}
		}
		void ReverseRightToLeft(BarControlRowViewInfo rowInfo, Rectangle rect) {
			if(rect.Width < 1) return;
			foreach(BarLinkViewInfo itemInfo in rowInfo.Links) {
				Rectangle bounds = itemInfo.Bounds;
				Rectangle newBounds = new Rectangle(rect.Right - (bounds.Width + bounds.X), bounds.Y, bounds.Width, bounds.Height);
				itemInfo.Rects.ReverseRightToLeft(rect);
				itemInfo.Bounds = newBounds;
			}
		}
		protected virtual void RemoveInvisibleAnimatedItems() {
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				EditorAnimationInfo info = XtraAnimator.Current.Animations[i] as EditorAnimationInfo;
				if(info == null || info.AnimatedObject != BarControl) continue;
				if(!HasItem(info))
					XtraAnimator.RemoveObject(info.AnimatedObject, info.AnimationId);
			}
		}
		protected virtual bool HasItem(EditorAnimationInfo info) {
			foreach(BarControlRowViewInfo rowInfo in Rows) {
				foreach(BarLinkViewInfo itemInfo in rowInfo.Links) {
					BarEditLinkViewInfo editInfo = itemInfo as BarEditLinkViewInfo;
					if(editInfo == null || !(editInfo.EditViewInfo is IAnimatedItem)) continue;
					if(editInfo.Link == info.Link) return true;
				}
			}
			return false;
		}
		protected virtual void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			CalcHorizontalViewInfo(g, sourceObject, rect);
		}
		protected virtual ArrayList PreCalcMultiLineSize(Graphics g, object sourceObject, int AMaxBarWidth, ref int ARestWidth) {
			ArrayList rows = new ArrayList();
			BarControlRowViewInfo rowViewInfo = null, prevRowInfo = null;
			Size linkSize, minSize = CalcMinSize(g, sourceObject);
			int maxBarWidth = 0;
			bool qLinkPresent = ReallyVisibleLinks.Count > 0 && (ReallyVisibleLinks[ReallyVisibleLinks.Count - 1] is BarQBarCustomizationItemLink);
			maxBarWidth = (AMaxBarWidth < 0 ? 1000 : AMaxBarWidth);
			maxBarWidth = Math.Max(CalcMinWidth(g, sourceObject), maxBarWidth);
			if(IsAllowQuickCustomization(sourceObject) || qLinkPresent) 
				maxBarWidth -= DrawParameters.Constants.BarQuickButtonWidth;
			maxBarWidth -= (CalcIndent(BarIndent.Left) + CalcIndent(BarIndent.Right) + CalcIndent(BarIndent.DragBorder) + CalcIndent(BarIndent.SizeGrip));
			ARestWidth = maxBarWidth;
			UpdateLinkWidthes(sourceObject, maxBarWidth);
			bool bGroupStarted = false;
			int startIndex = 0, linksCount = ReallyVisibleLinks.Count, currentPosition = 0;
			if(linksCount == 0 || ReallyVisibleLinks[0] is BarQBarCustomizationItemLink) startIndex = -1;
			if(linksCount > 0 && ReallyVisibleLinks[linksCount - 1] is BarQBarCustomizationItemLink) linksCount --;
			for(int n = startIndex; n < linksCount; n++) {
				BarItemLink link = null;
				if(n == -1) link = BarControl.ControlLinks.EmptyLink;
				else {
					if(ReallyVisibleLinks.Count <= n)
						break;
					link = (BarItemLink)ReallyVisibleLinks[n];
				}
				linkSize = CalcLinkSize(link, g, sourceObject);
				if(ShouldCreateNewRow(rows, rowViewInfo, link, linkSize, currentPosition, maxBarWidth)) {
					bool bNeedRestart = false;
					prevRowInfo = rowViewInfo;
					if(rows.Count > 0 && !IsMultiLine(sourceObject)) {
						return null; 
					}
					rowViewInfo = CreateRowViewInfo(rows);
					if(bGroupStarted && !link.IsStartGroup) { 
						int skipCount = RearrangeLinks(rows, prevRowInfo);
						bNeedRestart = true;
						n -= (skipCount + 1);
					}
					SetNewRowStart(rowViewInfo, prevRowInfo, ref currentPosition);
					bGroupStarted = false;
					if(bNeedRestart) continue;
				}
				UpdateLinkInfo(rows, rowViewInfo, prevRowInfo, link, linkSize, ref currentPosition, ref bGroupStarted);
				ARestWidth = maxBarWidth - currentPosition;
			}
			UpdateRowRects(rows);
			return rows;
		}
		bool ShouldCreateNewRow(ArrayList rows, BarControlRowViewInfo currentRow, BarItemLink currentLink, 
			Size currentLinkSize, int curPosition, int maxBarWidth) {
			if(rows.Count == 0) return true;
			bool result = rows.Count == 0;
			int linkWidth = (IsVertical ? currentLinkSize.Height : currentLinkSize.Width);
			int separatorWidth = 0;
			if(currentLink.IsStartGroup && AllowRowSeparator) {
				separatorWidth = DrawParameters.Constants.BarSeparatorWidth;
			}
			if(currentRow.Links.Count > 0) {
				bool isMultiline = currentRow.BarInfo.Bar != null ? currentRow.BarInfo.Bar.OptionsBar.MultiLine : false;
				if((curPosition + linkWidth + separatorWidth > maxBarWidth) && !((currentLink is BarStaticItemLink) && !isMultiline && currentLink.Item.Alignment != BarItemLinkAlignment.Right)) return true; 
				if(currentLink.AlwaysStartNewLine) return true; 
				if(rows.Count > 1 && currentLink.GetBeginGroup() && !(currentRow.Links[0] as BarLinkViewInfo).Link.GetBeginGroup())
					return true;
			}
			return false;
		}
		protected virtual bool IsAllowQuickCustomizationNonFit(object sourceObject) {
			if(Bar != null && !Bar.IsStatusBar) {
				return true;
			}
			return false;
		}
		protected virtual bool IsAllowQuickCustomization(object sourceObject) {
			if(!Manager.AllowQuickCustomization) return false;
			if((Bar != null && Bar.OptionsBar.AllowQuickCustomization && Bar.OptionsBar.DrawBorder)) return true;
			return false;
		}
		protected virtual bool IsMultiLine(object sourceObject) {
			if(BarControl.IsMultiLine) return true;
			return false;
		}
		protected virtual void UpdateLinkWidthes(object sourceObject, int totalWidth) {
		}
		void UpdateRowWidth(BarControlRowViewInfo rowViewInfo) {
			if(rowViewInfo == null) return;
			int maxWidth = 0;
			Rectangle r;
			foreach(BarLinkViewInfo linkInfo in rowViewInfo.Links) {
				r = rowViewInfo.Bounds;
				if(IsVertical) {
					maxWidth = Math.Max(linkInfo.Bounds.Width, maxWidth);
					r.Width = maxWidth;
				} else {
					maxWidth = Math.Max(linkInfo.Bounds.Height, maxWidth);
					r.Height = maxWidth;
				}
				rowViewInfo.Bounds = r;
			}
		}
		void SetNewRowStart(BarControlRowViewInfo rowViewInfo, BarControlRowViewInfo prevRowInfo, ref int currentPosition) {
			Rectangle r = rowViewInfo.Bounds;
			if(!IsVertical) {
				r.X = currentPosition = 0;
				r.Y = (prevRowInfo == null ? CalcIndent(BarIndent.Top) : prevRowInfo.Bounds.Bottom + CalcIndent(BarIndent.Top) + CalcIndent(BarIndent.Bottom) );
			} else {
				r.Y = currentPosition = 0;
				r.X = (prevRowInfo == null ? CalcIndent(BarIndent.Top) : prevRowInfo.Bounds.Right + CalcIndent(BarIndent.Top) + CalcIndent(BarIndent.Bottom));
			}
			rowViewInfo.Bounds = r;
		}
		void UpdateLinkInfo(ArrayList rows, BarControlRowViewInfo rowViewInfo, BarControlRowViewInfo prevRowInfo, BarItemLink link, Size linkSize, ref int currentPosition, ref bool bGroupStarted) {
			BarLinkViewInfo linkInfo = link.CreateViewInfo();
			linkInfo.drawParameters = this.DrawParameters;
			linkInfo.ParentViewInfo = rowViewInfo;
			if(link.IsStartGroup) {
				DoLinkGroupStart(rows, linkInfo, rowViewInfo, prevRowInfo, ref currentPosition, ref bGroupStarted);
			}
			int rX = 0, rY = 0;
			if(IsVertical) {
				rY = currentPosition;
				rX = rowViewInfo.Bounds.X;
			} else {
				rX = currentPosition;
				rY = rowViewInfo.Bounds.Y;
			}
			linkInfo.Bounds = new Rectangle(rX, rY, linkSize.Width, linkSize.Height);
			rowViewInfo.Links.Add(linkInfo);
			Rectangle r = rowViewInfo.Bounds;
			if(IsVertical) {
				r.Height += linkSize.Height;
				r.Width = Math.Max(linkSize.Width, rowViewInfo.Bounds.Width);
				currentPosition += linkSize.Height;
			} else {
				r.Height = Math.Max(linkSize.Height, rowViewInfo.Bounds.Height);
				r.Width += linkSize.Width;
				currentPosition += linkSize.Width;
			}
			rowViewInfo.Bounds = r;
		}
		protected virtual bool AllowRowSeparator { get { return true; } }
		void DoLinkGroupStart(ArrayList rows, BarLinkViewInfo linkInfo, BarControlRowViewInfo rowViewInfo, BarControlRowViewInfo prevRowInfo, ref int currentPosition, ref bool bGroupStarted) {
			if(rowViewInfo.Links.Count == 0 && rows.Count > 1) { 
				if(!linkInfo.Link.GetBeginGroup()) return;
				if(!AllowRowSeparator) return;
				Rectangle r, rv;
				r = rv = rowViewInfo.Bounds;
				if(IsVertical) {
					r.Width = DrawParameters.Constants.BarSeparatorWidth;
					rv.X += r.Width;
				} else {
					r.Height = DrawParameters.Constants.BarSeparatorWidth;
					rv.Y += r.Height;
				}
				rowViewInfo.Bounds = rv;
				linkInfo.Rects.AddExtRectangle(new RectInfo(null, r, IsVertical ? RectInfoType.VertSeparator : RectInfoType.HorzSeparator));
			} else { 
				if(rowViewInfo.Links.Count > 0) {
					bGroupStarted = true;
					if(!linkInfo.Link.GetBeginGroup()) return;
					Rectangle r = rowViewInfo.Bounds;
					if(IsVertical) {
						linkInfo.Rects.AddExtRectangle(new RectInfo(null, new Rectangle(rowViewInfo.Bounds.X, currentPosition, 0, DrawParameters.Constants.BarSeparatorWidth), RectInfoType.HorzSeparator));
						r.Height += DrawParameters.Constants.BarSeparatorWidth;
					} else {
						linkInfo.Rects.AddExtRectangle(new RectInfo(null, new Rectangle(currentPosition, rowViewInfo.Bounds.Y, DrawParameters.Constants.BarSeparatorWidth, 0), RectInfoType.VertSeparator));
						r.Width += DrawParameters.Constants.BarSeparatorWidth;
					}
					rowViewInfo.Bounds = r;
					currentPosition += DrawParameters.Constants.BarSeparatorWidth;
				}
			}
		}
		protected void SynchronizeRowRects(ArrayList rows) {
			if(rows.Count > 1) return; 
			int h = IsVertical? Bounds.Width: Bounds.Height;
			if(h == 0) return;
			h -= (CalcIndent(BarIndent.Top) + CalcIndent(BarIndent.Bottom));
			h = h / (rows.Count > 0 ? rows.Count : 1);
			foreach(BarControlRowViewInfo rowInfo in rows) {
				Rectangle r = rowInfo.Bounds;
				if(!IsVertical) {
					if(r.Height < h) r.Height = h;
				}
				else {
					if(r.Width < h) r.Width = h;
				}
				rowInfo.Bounds = r;
			}
		}
		void UpdateRowRects(ArrayList rows) {
			SynchronizeRowRects(rows);
		}
		int RearrangeLinks(ArrayList rows, BarControlRowViewInfo prevRowInfo) {
			int skipCount = 0;
			for(int k = prevRowInfo.Links.Count - 1; k >= 0; k--) {
				skipCount++;
				BarLinkViewInfo linkInfo = prevRowInfo.Links[k] as BarLinkViewInfo;
				prevRowInfo.Links.RemoveAt(k);
				Rectangle r = prevRowInfo.Bounds;
				if(IsVertical) {
					r.Height -= linkInfo.Bounds.Height;
				} else {
					r.Width -= linkInfo.Bounds.Width;
				}
				prevRowInfo.Bounds = r;
				if(linkInfo.Link.IsStartGroup) {
					if(linkInfo.Link.GetBeginGroup()) {
						if(IsVertical) {
							r.Height -= DrawParameters.Constants.BarSeparatorWidth;
						} else {
							r.Width -= DrawParameters.Constants.BarSeparatorWidth;
						}
					}
					prevRowInfo.Bounds = r;
					break;
				}
			}
			UpdateRowWidth(prevRowInfo);
			return skipCount;
		}
		int GetVerticalSeparatorWidth(BarLinkViewInfo linkInfo) { 
			foreach(RectInfo rInfo in linkInfo.Rects.ExtRectangles) {
				if(rInfo.Type == RectInfoType.VertSeparator)
					return rInfo.Rect.Width;
			}
			return 0;
		}
		void RealignRowViewInfo(BarControlRowViewInfo rowInfo, int rowWidth) { 
			if(IsVertical) return;
			int leftX = 0;
			int rightX = rowWidth;
			for(int i = 0; i < rowInfo.Links.Count; i++) {
				BarLinkViewInfo linkInfo = (BarLinkViewInfo)rowInfo.Links[i];
				if(linkInfo is BarQBarCustomizationLinkViewInfo)
					continue;
				if(linkInfo.Link.Alignment == BarItemLinkAlignment.Right)
					continue;
				Rectangle r = linkInfo.Bounds;
				int separatorWidth = GetVerticalSeparatorWidth(linkInfo);
				r.X = leftX + separatorWidth;
				leftX += linkInfo.Bounds.Width + separatorWidth;
				int delta = r.X - linkInfo.Bounds.X;
				linkInfo.Bounds = r;
				linkInfo.Rects.OffsetExtRectangles(delta, 0);
			}
			for(int i = rowInfo.Links.Count - 1; i >= 0 ; i--) {
				BarLinkViewInfo linkInfo = (BarLinkViewInfo)rowInfo.Links[i];
				if(linkInfo is BarQBarCustomizationLinkViewInfo)
					continue;
				if(linkInfo.Link.Alignment != BarItemLinkAlignment.Right)
					continue;
				Rectangle r = linkInfo.Bounds;
				int separatorWidth = GetVerticalSeparatorWidth(linkInfo);
				r.X = rightX - linkInfo.Bounds.Width;
				rightX -= linkInfo.Bounds.Width + separatorWidth;
				int delta = r.X - linkInfo.Bounds.X;
				linkInfo.Bounds = r;
				linkInfo.Rects.OffsetExtRectangles(delta, 0);
			}
		}
		protected bool IsAllItemsFit(ArrayList rows) {
			int itemsCount = 0;
			foreach(BarControlRowViewInfo rowInfo in rows) {
				foreach(BarLinkViewInfo linkInfo in rowInfo.Links) {
					if(linkInfo.Link is BarQBarCustomizationItemLink) continue;
					itemsCount++;
				}
			}
			int vCount = BarControl.VisibleLinks.Count;
			if(vCount > 0 && BarControl.VisibleLinks[vCount -1] is BarQBarCustomizationItemLink) vCount--;
			return (vCount <= itemsCount);
		}
		protected bool IsShowCustomizationLink(ArrayList rows) {
			bool result = false;
			if(IsAllowQuickCustomization(SourceObject) || (!IsMultiLine(SourceObject) && !IsAllItemsFit(rows))) 
				result = true;
			return result;
		}
		protected virtual Hashtable LinkSizes { get { return linkSizes; } }
		public virtual void ClearHash() { LinkSizes.Clear(); }
		public virtual Size CalcLinkSize(BarItemLink link, Graphics g, object sourceObject) {
			if(LinkSizes.Contains(link)) return (Size)LinkSizes[link];
			Size size = link.CalcLinkSize(g, sourceObject);
			LinkSizes[link] = size;
			return size;
		}
	}
	public class QuickCustomizationBarControlViewInfo : BarControlViewInfo {
		public QuickCustomizationBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public new virtual QuickCustomizationBarControl BarControl { get { return base.BarControl as QuickCustomizationBarControl; } }
		protected override void UpdateAppearance() {
			Appearance.Combine(new StateAppearances[] {  Manager.GetController().AppearancesBar.BarAppearance, DrawParameters.StateAppearance(BarAppearance.Bar)});
			Appearance.UpdateRightToLeft(IsRightToLeft);
		}
	}
	public class DockedBarControlViewInfo : BarControlViewInfo {
		int averageWidth = 0;
		int springLinkCount = 0;
		public DockedBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public new virtual DockedBarControl BarControl { get { return base.BarControl as DockedBarControl; } }
		public BarLinkViewInfo GetQuickCustomizationLink() {
			if(Rows.Count == 0) return null;
			if(BarControl.QuickCustomizationLink == null) return null;
			return GetLinkViewInfo(BarControl.QuickCustomizationLink, LinkViewInfoRange.Current);
		}
		protected override void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			base.CalcHorizontalViewInfo(g, sourceObject, rect);
			UpdateNotVisibleLinks();
		}
		protected virtual Rectangle CalcQuickCustomizationLinkBounds(Rectangle barBounds, Rectangle linkBounds, Size linkSize) {
			Rectangle r = linkBounds;
			if(IsVertical) {
				r.Height = linkSize.Height;
				r.Y = barBounds.Bottom - r.Height;
				r.Inflate(-1, 0);
			} else {
				r.Width = linkSize.Width;
				r.X = barBounds.Right - r.Width;
				r.Inflate(0, -1);
			}
			return r;
		}
		protected override void CalcQuickCustomizationInfo(Graphics g, object sourceObject, Rectangle rect, ref  int separatorWidth) {
			if(IsAllowQuickCustomization(sourceObject) || (IsAllowQuickCustomizationNonFit(sourceObject) && !IsAllItemsFit(Rows))) {
				BarLinkViewInfo itemInfo = BarControl.AddQuickCustomizationLink().CreateViewInfo();
				itemInfo.ParentViewInfo = this;
				itemInfo.Bounds = rect;
				Rectangle r = rect;
				Size size = itemInfo.CalcLinkSize(g, sourceObject); 
				itemInfo.Bounds = CalcQuickCustomizationLinkBounds(rect, r, size);
				separatorWidth -= DrawParameters.Constants.BarQuickButtonWidth;
				if(!IsAllItemsFit(Rows)) 
					(itemInfo as BarQBarCustomizationLinkViewInfo).DrawExpandMark = true;
				((BarControlRowViewInfo)Rows[Rows.Count - 1]).Links.Add(itemInfo);
			}
		}
		protected virtual void UpdateNotVisibleLinks() {
			NotVisibleLinks.ClearItems();
			foreach(BarItemLink link in BarControl.VisibleLinks) {
				if(link is BarQBarCustomizationItemLink) continue;
				if(GetLinkViewInfo(link, LinkViewInfoRange.Current) == null)
					NotVisibleLinks.AddItem(link);
			}
		}
		protected override void UpdateLinkWidthes(object sourceObject, int totalWidth) {
			foreach(BarItemLink link in ReallyVisibleLinks) {
				ISpringLink sp = link as ISpringLink;
				if(sp == null || !sp.SpringAllow) continue;
				springLinkCount ++;
				sp.SpringWidth = averageWidth;
			}
		}
		protected override ArrayList PreCalcMultiLineSize(Graphics g, object sourceObject, int AMaxBarWidth, ref int ARestWidth) {
			int restWidth = 0;
			UpdateLinkWidthes(null, 0);
			averageWidth = 0;
			springLinkCount = 0;
			ClearHash();
			ArrayList rows = base.PreCalcMultiLineSize(g, sourceObject, AMaxBarWidth, ref restWidth);
			if(!BarControl.IsCanSpringLinks || springLinkCount == 0 || rows == null) 
				return rows;
			foreach(BarItemLink link in ReallyVisibleLinks) {
				ISpringLink sp = link as ISpringLink;
				if(sp == null || !sp.SpringAllow) continue;
				Size size = CalcLinkSize(link, g, sourceObject);
				int w = IsVertical ? size.Height : size.Width;
				sp.SpringTempWidth = w;
				restWidth += w;
			}
			ClearHash();
			averageWidth = restWidth / springLinkCount;
			if(averageWidth < 4) averageWidth = 4;
			return base.PreCalcMultiLineSize(g, sourceObject, AMaxBarWidth, ref restWidth);
		}
	}
	public class FloatingBarControlViewInfo : BarControlViewInfo {
		public FloatingBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		protected override bool IsAlwaysBestFit { get { return true; } }
		protected override bool IsAllowQuickCustomization(object sourceObject) { return false; } 
		protected override bool IsMultiLine(object sourceObject) { return true; }
		public override bool DrawDragBorder { get { return false; } }
		public override bool DrawSizeGrip { get { return false; } }
		public override int VertIndent { get { return 1; } }
		public override int HorzIndent { get { return 0; } }
		public override Size CalcBarSize(Graphics g, object sourceObject, int width, int maxHeight) {
			Size size = base.CalcBarSize(g, sourceObject, width, maxHeight);
			if(size.Width < 46) size.Width = 46;
			return size;
		}
	}
}
