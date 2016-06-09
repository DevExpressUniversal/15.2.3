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
using DevExpress.XtraBars.Objects;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraBars.ViewInfo {
	public class SubMenuLinkInfo {
		public BarLinkViewInfo LinkInfo;
		public int LinkHeight;
		public int LinkWidth;
		public int Column;
		public int Row;
		public bool MultiColumn;
		public SubMenuLinkInfo(BarLinkViewInfo li, int height, int width) {
			LinkInfo = li;
			LinkHeight = height;
			LinkWidth = width;
			Column = -1;
			Row = -1;
		}
	}
	public abstract class CustomSubMenuBarControlViewInfo : CustomLinksControlViewInfo {
		public List<SubMenuLinkInfo> BarLinksViewInfo;
		public int GlyphWidth, TextWidth, TextIndent, MaxLinkWidth, MaxMulticolumnLinkWidth;
		public Rectangle MenuBar;
		MenuAppearance appearanceMenu;
		Rectangle linksBounds, menuCaptionBounds;
		protected internal abstract bool ShowMenuCaption { get; }
		public virtual new CustomLinksControl BarControl { get { return base.BarControl as CustomLinksControl; } }
		protected CustomPopupBarControl PopupBarControl { get { return base.BarControl as CustomPopupBarControl; } }
		public CustomSubMenuBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
			this.appearanceMenu = new MenuAppearance();
			BarLinksViewInfo = new List<SubMenuLinkInfo>();
			Clear();
		}
		public virtual bool ShowNavigationHeader {
			get {
				return Manager.GetPopupShowMode(PopupBarControl) == PopupShowMode.Inplace;
			}
		}
		List<MenuNavigationItem> navigationItems;
		public List<MenuNavigationItem> NavigationItems {
			get {
				if(navigationItems == null)
					navigationItems = CreateNavigationItems();
				return navigationItems;
			}
		}
		protected virtual List<MenuNavigationItem> CreateNavigationItems() {
			List<MenuNavigationItem> res = new List<MenuNavigationItem>();
			foreach(IPopup popup in Manager.SelectionInfo.OpenedPopups) {
				res.Add(new MenuNavigationItem(this) { Popup = popup });
			}
			res.Add(new MenuNavigationItem(this) { Popup = PopupBarControl });
			return res;
		}
		protected virtual MenuDrawMode GetDefaultMenuDrawMode() {
			IPopup popup = BarControl as IPopup;
			AppMenuBarControl bc = popup.ParentPopup as AppMenuBarControl;
			if(bc != null) {
				if(bc.AppMenu.MenuDrawMode != MenuDrawMode.Default)
					return bc.AppMenu.MenuDrawMode;
				return MenuDrawMode.LargeImagesTextDescription;
			}
			return MenuDrawMode.SmallImagesText;
		}
		protected internal virtual MenuDrawMode GetMenuDrawMode() {
			if(DrawMode == MenuDrawMode.Default) return GetDefaultMenuDrawMode();
			return DrawMode;
		}
		protected internal abstract MenuDrawMode DrawMode { get; }
		public virtual int MenuBarWidth { 
			get {
				if(BarControl is CustomPopupBarControl) {
					return (BarControl as CustomPopupBarControl).MenuBarWidth;
				}
				return 0; 
			} 
		}
		public Rectangle MenuCaptionBounds { get { return menuCaptionBounds; } set { menuCaptionBounds = value; } }
		public override void Clear() {
			this.menuCaptionBounds = this.linksBounds = Rectangle.Empty;
			MaxLinkWidth = GlyphWidth = TextWidth = 0;
			MenuBar = Rectangle.Empty;
			if(BarLinksViewInfo != null) 
				BarLinksViewInfo.Clear();
			base.Clear();
		}
		public Rectangle LinksBounds { get { return linksBounds; } set { linksBounds = value; } }
		public Rectangle NavigationHeaderBounds {
			get;
			set;
		}
		public Rectangle NavigationHeaderContentBounds {
			get;
			set;
		}
		public virtual MenuAppearance AppearanceMenu { get { return appearanceMenu; } }
		public override StateAppearances Appearance { get { return AppearanceMenu.AppearanceMenu; } }
		protected virtual MenuAppearance PopupAppearance {
			get { return null; }
		}
		protected override void UpdateAppearance() {
			AppearanceMenu.Reset();
			if(PopupAppearance != null) {
				MenuAppearance temp = new MenuAppearance();
				temp.Combine(Manager.GetController().AppearancesBar.SubMenu, Manager.GetController().PaintStyle.DrawParameters.Colors.MenuAppearance);
				AppearanceMenu.Combine(PopupAppearance, temp);
			} else {
				AppearanceMenu.Combine(Manager.GetController().AppearancesBar.SubMenu, Manager.GetController().PaintStyle.DrawParameters.Colors.MenuAppearance);
			}
			AppearanceMenu.UpdateRightToLeft(IsRightToLeft);
			Appearance.UpdateRightToLeft(IsRightToLeft);
			this.navigationHeaderAppearance = null;
			this.navigationItemAppearance = null;
		}
		public override Region GetBarLinksRegion(object sourceObject) {
			Region res = new Region();
			foreach(SubMenuLinkInfo info in BarLinksViewInfo) {
				res.Union(info.LinkInfo.Bounds);
			}
			return res;
		}
		protected virtual Size BarMinSize { get { return new Size(16, 16); } }
		protected virtual int PreCalcLinksMaxHeight(int maxHeight) { return maxHeight;  }
		protected Size CalcGalleryItemSize(SubMenuLinkInfo info, List<SubMenuLinkInfo> links) {
			Size itemSize = new Size();
			int count = GetGalleryEndItemIndex(info.LinkInfo.Link) - BarControl.VisibleLinks.IndexOf(info.LinkInfo.Link);
			int start = links.IndexOf(info);
			for(int i = start; i <= start + count; i++) {
				if(GetLinkMultiColumn(info.LinkInfo.Link)) {
					itemSize.Width = Math.Max(links[i].LinkWidth, itemSize.Width);
					itemSize.Height = Math.Max(links[i].LinkHeight, itemSize.Height);
				}
			}
			return itemSize;
		}
		protected virtual Size CalcLinksSize(List<SubMenuLinkInfo> links) {
			Size res = Size.Empty;
			int galleryWidth = 0;
			Size itemSize = Size.Empty;
			foreach(SubMenuLinkInfo info in links) {
				if(!info.MultiColumn) {
					res.Height += info.LinkHeight;
					res.Width = Math.Max(res.Width, info.LinkWidth);
					galleryWidth = 0;
					itemSize = Size.Empty;
				}
				else {
					int itemIndent =  GetGalleryItemIndent(info.LinkInfo.Link);
					if(itemSize.IsEmpty)
						itemSize = CalcGalleryItemSize(info, links);
					int itemHorzIndent = info.Column == 0? 0: itemIndent;
					int itemVertIndent = info.Row == 0? 0: itemIndent;
					if(info.Column == 0) {
						res.Height += itemSize.Height + itemVertIndent;
						res.Width = Math.Max(res.Width, galleryWidth);
						galleryWidth = 0;
					}
					galleryWidth += itemSize.Width + itemHorzIndent;
					res.Width = Math.Max(res.Width, galleryWidth);
				}
			}
			return res;
		}
		public override Size CalcBarSize(Graphics g, object sourceObject, int width, int maxHeight) {
			Update();
			maxHeight = PreCalcLinksMaxHeight(maxHeight);
			int topLinkIndex = 0;
			g = GInfo.AddGraphics(g);
			Size res = Size.Empty;
			try {
				if(PopupBarControl == null) maxHeight = -1;
				else topLinkIndex = PopupBarControl.TopLinkIndex;
				CalcMaxWidthes();
				List<SubMenuLinkInfo> links = PreCalcViewInfo(g, sourceObject, topLinkIndex, maxHeight < 0 ? Rectangle.Empty : new Rectangle(0, 0, 0, maxHeight), maxHeight > 0, true);
				res = CalcLinksSize(links);
				if(MaxLinkWidth == 0) MaxLinkWidth = res.Width;
				links.Clear();
				if(res.IsEmpty) 
					res = BarMinSize;
				Size caption = CalcMenuCaptionSize();
				if(!caption.IsEmpty) MaxLinkWidth = Math.Max(caption.Width, MaxLinkWidth);
				res.Width = MaxLinkWidth + MenuBarWidth;
				if(!caption.IsEmpty) {
					res.Width = Math.Max(res.Width, caption.Width);
					res.Height += caption.Height;
				}
				if(ShowNavigationHeader) {
					NavigationHeaderBounds = new Rectangle(Point.Empty, new Size(res.Width, CalcNavigationHeaderSize(GInfo.Graphics).Height));
					NavigationHeaderContentBounds = CalcNavigationHeaderContentBounds(NavigationHeaderBounds);
					res.Height += NavigationHeaderBounds.Height;
					res.Width = Math.Max(res.Width, NavigationHeaderBounds.Width);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		StateAppearances navigationItemAppearance;
		public StateAppearances NavigationItemAppearance {
			get {
				if(navigationItemAppearance == null) {
					navigationItemAppearance = CreateNavigationItemAppearance();
				}
				return navigationItemAppearance;
			}
		}
		AppearanceObject navigationHeaderAppearance;
		public AppearanceObject NavigationHeaderAppearance {
			get {
				if(navigationHeaderAppearance == null)
					navigationHeaderAppearance = GetNavigationHeaderAppearance();
				return navigationHeaderAppearance;
			}
		}
		protected virtual AppearanceObject GetNavigationHeaderAppearance() {
			return PaintStyle.GetNavigationHeaderAppearanceDefault().Normal;
		}
		protected internal virtual StateAppearances CreateNavigationItemAppearance() {
			return PaintStyle.GetNavigationHeaderAppearanceDefault();
		}
		protected internal virtual AppearanceObject GetNavigationItemAppearance(MenuNavigationItem item) {
			if(item.Popup == PopupBarControl)
				return NavigationHeaderAppearance;
			return NavigationItemAppearance.GetAppearance(item.State);
		}
		protected internal virtual Size CalcNavigationHeaderSize(Graphics g) {
			if(!ShowNavigationHeader)
				return Size.Empty;
			Size res = Size.Empty;
			if(NavigationItems.Count > 1) {
				res = CalcNavigationBackArrowSize(g);
				res.Width += BackArrow2NavigationItemIndent;
			}
			Size navItemSize = Size.Empty;
			for(int i = 0; i < NavigationItems.Count; i++) {
				NavigationItems[i].Bounds = new Rectangle(NavigationItems[i].Bounds.Location, CalcNavigationItemSize(g, NavigationItems[i]));
				res.Width += NavigationItems[i].Bounds.Width + NavigationItemHorzIndent;
				res.Height = Math.Max(res.Height, NavigationItems[i].Bounds.Height);
			}
			return PaintStyle.CalcMenuNavigationHeaderSize(g, res);
		}
		protected virtual Size CalcNavigationItemSize(Graphics g, MenuNavigationItem item) {
			AppearanceObject obj = GetNavigationItemAppearance(item);
			return obj.CalcTextSize(g, item.Text, 0).ToSize();
		}
		protected virtual int NavigationItemHorzIndent { get { return 4; } }
		protected virtual int BackArrow2NavigationItemIndent { get { return 4; } }
		private Size CalcNavigationBackArrowSize(Graphics graphics) {
			return PaintStyle.CalcNavigationBackArrowSize(graphics);
		}
		public new BarSubMenuPainter Painter { get { return base.Painter as BarSubMenuPainter; } }
		public abstract string MenuCaption { get; }
		protected virtual Size CalcMenuCaptionSize() {
			if(!ShowMenuCaption) return Size.Empty;
			Size res = Size.Empty;
			GInfo.AddGraphics(null);
			try {
				res = Painter.CalcCaptionSize(GInfo.Graphics, this, MenuCaption);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected virtual bool IsAllowEmptyItemLink { get { return BarControl.VisibleLinks.Count == 0 && BarControl.Manager.IsCustomizing; } }
		protected virtual void CalcMaxGlyphWidth() {
			int maxWidth = 0;
			int startIndex = (IsAllowEmptyItemLink ? -1 : 0);
			for(int n = startIndex; n < BarControl.VisibleLinks.Count; n++) {
				BarItemLink link = (n == -1 ? BarControl.ControlLinks.EmptyLink : BarControl.VisibleLinks[n] as BarItemLink);
				BarLinkViewInfo linkViewInfo = CreateLinkViewInfo(link);
				linkViewInfo.UpdateLinkInfo(this);
				maxWidth = Math.Max(maxWidth, linkViewInfo.GlyphSize.Width);
			}
			GlyphWidth = maxWidth;
		}
		protected BarLinkViewInfo CreateLinkViewInfo(BarItemLink link) {
			BarLinkViewInfo linkViewInfo = link.CreateViewInfo();
			if(GetMenuDrawMode() == MenuDrawMode.LargeImagesText) linkViewInfo.ForceDrawMode = BarLinkDrawMode.InMenuLarge;
			if(GetMenuDrawMode() == MenuDrawMode.LargeImagesTextDescription) linkViewInfo.ForceDrawMode = BarLinkDrawMode.InMenuLargeWithText;
			linkViewInfo.drawParameters = this.DrawParameters;
			linkViewInfo.ParentViewInfo = this;
			return linkViewInfo;
		}
		void onGetLinkGlyphWidth(object sender, BarLinkGetValueEventArgs e) {
			Size size = (Size)e.Value;
			if(e.LinkInfo.DrawMode != BarLinkDrawMode.InMenuGallery)
				size.Width = GlyphWidth;
			e.Value = size;
		}
		protected SubMenuLinkInfo CalcLinkInfo(Graphics g, BarItemLink link, int linkIndex, Point curPos, object sourceObject, bool preCalc) {
			SubMenuLinkInfo subInfo;
			int sepHeight = 0;
			Size linkSize;
			BarLinkViewInfo linkViewInfo = CreateLinkViewInfo(link);
			linkViewInfo.GlyphSizeEvent += new BarLinkGetValueEventHandler(onGetLinkGlyphWidth);
			linkSize = linkViewInfo.CalcLinkSize(g, sourceObject);
			sepHeight = link.GetBeginGroup() && linkIndex > 0 && !GetLinkMultiColumn(link) ? DrawParameters.Constants.SubMenuSeparatorHeight : 0;
			linkViewInfo.Bounds = new Rectangle(curPos.X, curPos.Y + sepHeight, linkSize.Width, linkSize.Height);
			if(!preCalc) linkViewInfo.CalcViewInfo(g, sourceObject, linkViewInfo.Bounds);
			subInfo = new SubMenuLinkInfo(linkViewInfo, linkViewInfo.Bounds.Height + sepHeight, linkViewInfo.Bounds.Width);
			linkViewInfo.GlyphSizeEvent -= new BarLinkGetValueEventHandler(onGetLinkGlyphWidth);
			return subInfo;
		}
		protected int CalcRestLinksHeight(Graphics g, int index, object sourceObject) {
			int height = 0;
			int startLinkIndex = 0;
			bool prevMultiColumn = false;
			for(int n = index; n < BarControl.VisibleLinks.Count; n++) {
				if(BarControl.VisibleLinks[n] is BarScrollItemLink)
					continue;
				bool multiColumn = GetLinkMultiColumn(BarControl.VisibleLinks[n]);
				SubMenuLinkInfo info = CalcLinkInfo(g, BarControl.VisibleLinks[n], n, Point.Empty, sourceObject, true);
				if(multiColumn && !prevMultiColumn)
					startLinkIndex = GetGalleryStartItemIndex(BarControl.VisibleLinks[n]);
				prevMultiColumn = multiColumn;
				if(!multiColumn) {
					height += info.LinkHeight;
				}
				else {
					int columnCount = GetGalleryColumnCount(info.LinkInfo.Link);
					int rowIndex = (n - startLinkIndex) / columnCount;
					int columnIndex = (n - startLinkIndex) & columnCount;
					int itemIndent = GetGalleryItemIndent(info.LinkInfo.Link);
					int itemVertIndent = rowIndex == 0 ? 0 : itemIndent;
					if(columnIndex == 0) {
						height += info.LinkHeight + itemVertIndent;
					}
				}
			}
			return height;
		}
		protected internal int GetGalleryStartItemIndex(BarItemLink barItemLink) {
			for(int i = BarControl.VisibleLinks.IndexOf(barItemLink) - 1; i >= 0; i--) {
				if(!GetLinkMultiColumn(BarControl.VisibleLinks[i])) 
					return i + 1;
			}
			return 0;
		}
		protected internal int GetGalleryEndItemIndex(BarItemLink barItemLink) {
			for(int i = BarControl.VisibleLinks.IndexOf(barItemLink) + 1; i < BarControl.VisibleLinks.Count; i++) {
				if(!GetLinkMultiColumn(BarControl.VisibleLinks[i]))
					return i - 1;
			}
			return BarControl.VisibleLinks.Count - 1;
		}
		Dictionary<BarItemLink, int> galleryLinkWidth;
		protected internal Dictionary<BarItemLink, int> GalleryLinkWidth {
			get {
				if(galleryLinkWidth == null)
					galleryLinkWidth = new Dictionary<BarItemLink, int>();
				return galleryLinkWidth;
			}
		}
		void AddGalleryLinkWidth(int startIndex, int endIndex, int galleryItemWidth) {
			for(int i = startIndex; i < endIndex; i++) {
				GalleryLinkWidth.Add(BarControl.VisibleLinks[i], galleryItemWidth);
			}
		}
		public virtual void CalcMaxWidthes() {
			Graphics g = GInfo.AddGraphics(null);
			int maxWidth = 0;
			int galleryWidth = 0;
			int columnIndex = 0;
			int galleryStartIndex = -1;
			int galleryItemMaxWidth = 0;
			int galleryItemCount = 0;
			int columnCount = 0;
			int maxGalleryItemInColumn = 0;
			int itemIndent = 0;
			MaxMulticolumnLinkWidth = 0;
			GalleryLinkWidth.Clear();
			try {
				CalcMaxGlyphWidth();
				for(int n = 0; n < BarControl.VisibleLinks.Count; n++) {
					bool multiColumn = GetLinkMultiColumn(BarControl.VisibleLinks[n]);
					if(multiColumn && columnCount == 0) 
						columnCount = GetGalleryColumnCount(BarControl.VisibleLinks[n]);
					SubMenuLinkInfo subInfo = CalcLinkInfo(g, BarControl.VisibleLinks[n], n, Point.Empty, SourceObject, true);
					if(!multiColumn) {
						if(galleryStartIndex != -1) {
							maxWidth = Math.Max(maxWidth, maxGalleryItemInColumn * galleryItemMaxWidth + (maxGalleryItemInColumn - 1) * itemIndent);
						}
						maxWidth = Math.Max(subInfo.LinkInfo.Bounds.Width, maxWidth);
						galleryWidth = 0;
						columnIndex = 0;
						columnCount = 0;
						if(galleryStartIndex >= 0)
							AddGalleryLinkWidth(galleryStartIndex, n, galleryItemMaxWidth);
						galleryStartIndex = -1;
						galleryItemMaxWidth = 0;
					}
					if(multiColumn) {
						if(galleryStartIndex == -1) {
							itemIndent = GetGalleryItemIndent(BarControl.VisibleLinks[n]);
							galleryStartIndex = n;
							galleryItemCount = GetGalleryEndItemIndex(subInfo.LinkInfo.Link) - GetGalleryStartItemIndex(subInfo.LinkInfo.Link) + 1;
							maxGalleryItemInColumn = Math.Min(columnCount, galleryItemCount);
						}
						galleryItemMaxWidth = Math.Max(subInfo.LinkWidth, galleryItemMaxWidth);
						MaxMulticolumnLinkWidth = Math.Max(MaxMulticolumnLinkWidth, galleryItemMaxWidth);
						galleryWidth += subInfo.LinkWidth + (columnIndex == 0? 0: itemIndent);
						columnIndex++;
						maxWidth = Math.Max(galleryWidth, maxWidth);
						if(columnIndex == columnCount) {
							columnIndex = 0;
							galleryWidth = 0;
						}
					}
				}
				if(galleryStartIndex >= 0) {
					maxWidth = Math.Max(maxWidth, maxGalleryItemInColumn * galleryItemMaxWidth + (maxGalleryItemInColumn - 1) * itemIndent);
				}
				this.MaxLinkWidth = Math.Max(maxWidth, CalcMenuCaptionSize().Width);
				MaxMulticolumnLinkWidth = Math.Max(MaxMulticolumnLinkWidth, galleryItemMaxWidth);
				if(galleryStartIndex >= 0) {
					AddGalleryLinkWidth(galleryStartIndex, BarControl.VisibleLinks.Count, galleryItemMaxWidth);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual Rectangle CalcLinksBounds(Rectangle bounds) {
			this.menuCaptionBounds = Rectangle.Empty;
			if(ShowNavigationHeader) {
				bounds.Y += NavigationHeaderBounds.Height;
				bounds.Height -= NavigationHeaderBounds.Height;
			}
			if(!ShowMenuCaption) return bounds;
			Size size = CalcMenuCaptionSize();
			if(size.IsEmpty) return bounds;
			Rectangle res = bounds;
			this.menuCaptionBounds = bounds;
			this.menuCaptionBounds.Height = size.Height;
			res.Y = menuCaptionBounds.Bottom;
			res.Height = Math.Max(0, bounds.Bottom - res.Y);
			return res;
		}
		protected bool IsLastLinkRow(int linkIndex, int linksCount) {
			return linkIndex >= linksCount - 1;
		}
		public List<SubMenuLinkInfo> PreCalcViewInfo(Graphics g, object sourceObject, int topLinkIndex, Rectangle rect, bool useMaxHeight, bool preCalc) {
			List<SubMenuLinkInfo> links = new List<SubMenuLinkInfo>();
			ArrayList list = new ArrayList();
			int upPrevHeight = 0, downPrevHeight = 0;
			foreach(BarItemLink link in BarControl.VisibleLinks) {
				if(link is BarScrollItemLink) continue;
				list.Add(link);
			}
			if(PopupBarControl != null) {
				upPrevHeight = PopupBarControl.ControlLinks.UpScrollLink.LinkHeight;
				downPrevHeight = PopupBarControl.ControlLinks.DownScrollLink.LinkHeight;
				PopupBarControl.ControlLinks.UpScrollLink.LinkHeight = 0;
				if(!preCalc) {
					BarControl.VisibleLinks.RemoveItem(PopupBarControl.ControlLinks.UpScrollLink);
					BarControl.VisibleLinks.RemoveItem(PopupBarControl.ControlLinks.DownScrollLink);
				}
			}
			int maxHeight = rect.Height;
			int restHeight = rect.Height;
			if(MenuBarWidth > 0 && !preCalc) MenuBar = new Rectangle(rect.Location, new Size(MenuBarWidth, rect.Height));
			int startX = rect.X + MenuBarWidth;
			Point curPos = new Point(startX, rect.Y);
			g = GInfo.AddGraphics(g);
			int maxWidth = MaxLinkWidth, cc = 0;
			int startIndex = (IsAllowEmptyItemLink ? -1 : 0);
			if(startIndex == 0) startIndex = topLinkIndex;
			int vcount = list.Count;
			bool insertUpScroll = false, insertDownScroll = false;
			int galleryColumnCount = 0;
			int galleryItemIndent = 0;
			bool prevMultiColumn = false;
			int galleryMaxWidth = 0;
			int startGalleryIndex = 0;
			int endGalleryIndex = 0;
			int rowCount = 0;
			int galleryItemCount = 0;
			bool useMaxItemWidth = false;
			for(int loo = 0; loo < 2; loo ++) {
				for(int n = startIndex; n < vcount; n++) {
					SubMenuLinkInfo subInfo;
					if(startIndex > 0 && n == startIndex) {
						subInfo = CalcLinkInfo(g, PopupBarControl.ControlLinks.UpScrollLink, 0, curPos, sourceObject, preCalc);
						links.Add(subInfo);
						curPos.Y += subInfo.LinkHeight;
						restHeight -= subInfo.LinkHeight;
						insertUpScroll = true;
					}
					BarItemLink link = (n == -1 ? BarControl.ControlLinks.EmptyLink : list[n] as BarItemLink);
					if(link is BarScrollItemLink) continue;
					bool multiColumn = GetLinkMultiColumn(link);
					if(multiColumn && !prevMultiColumn) {
						galleryColumnCount = GetGalleryColumnCount(link);
						galleryItemIndent = GetGalleryItemIndent(link);
						useMaxItemWidth = GetUseMaxItemWidth(link);
						startGalleryIndex = n;
						endGalleryIndex = GetGalleryEndItemIndex(link);
						galleryItemCount = endGalleryIndex - startGalleryIndex + 1;
						rowCount = galleryItemCount / galleryColumnCount;
						if(galleryItemCount % galleryColumnCount > 0)
							rowCount++;
					}
					if(!multiColumn) {
						useMaxItemWidth = false;
					}
					int galleryItemColumn = multiColumn? (n - startGalleryIndex) % galleryColumnCount: 0;
					int galleryItemRow = multiColumn? (n - startGalleryIndex) / galleryColumnCount: 0;
					int nextRowLinkDelta = multiColumn? galleryColumnCount : 1;
					if(useMaxItemWidth && GalleryLinkWidth.ContainsKey(link)) {
						GalleryLinkWidth[link] = MaxMulticolumnLinkWidth;
					}
					subInfo = CalcLinkInfo(g, link, n, curPos, sourceObject, preCalc);
					subInfo.MultiColumn = multiColumn;
					subInfo.Row = galleryItemRow;
					subInfo.Column = galleryItemColumn;
					if(useMaxHeight && vcount > 0) {
						int rest = restHeight - subInfo.LinkHeight;
						if(rest < 0 || (!IsLastLinkRow(n, vcount) && rest < DrawParameters.Constants.GetSubMenuScrollLinkHeight())) {
							if(rest < 0 || rest < CalcRestLinksHeight(g, n + nextRowLinkDelta, sourceObject)) {
								BarScrollItemLink scrollLink = PopupBarControl.ControlLinks.DownScrollLink;
								scrollLink.LinkHeight = restHeight;
								subInfo = CalcLinkInfo(g, scrollLink, 0, curPos, sourceObject, preCalc);
								n = vcount;
								insertDownScroll = true;
							}
						}
					}
					links.Add(subInfo);
					if(!multiColumn) {
						curPos.Y += subInfo.LinkHeight;
						restHeight -= subInfo.LinkHeight;
						maxWidth = Math.Max(maxWidth, subInfo.LinkInfo.Bounds.Width);
					}
					else {
						bool lastColumnItem = (galleryItemColumn == galleryColumnCount - 1) || n == endGalleryIndex;
						int galleryItemHorzIndent = lastColumnItem ? 0 : galleryItemIndent;
						int galleryItemVertIndent = galleryItemRow == rowCount - 1 ? 0 : galleryItemIndent;
						if(lastColumnItem) {
							curPos.Y += subInfo.LinkHeight + galleryItemVertIndent;
							restHeight -= subInfo.LinkHeight + galleryItemVertIndent;
							maxWidth = Math.Max(galleryMaxWidth, maxWidth);
							galleryMaxWidth = 0;
							curPos.X = subInfo.Row == rowCount - 1? startX: rect.X;
						}
						else {
							curPos.X += subInfo.LinkWidth + galleryItemHorzIndent;
							galleryMaxWidth += subInfo.LinkWidth + galleryItemHorzIndent;
						}
					}
					prevMultiColumn = multiColumn;
				}
				if(loo == 0) {
					if(insertDownScroll || !insertUpScroll || restHeight == 0) break;
					PopupBarControl.ControlLinks.UpScrollLink.LinkHeight = restHeight + DrawParameters.Constants.GetSubMenuScrollLinkHeight();
					restHeight = maxHeight;
					curPos.Y = rect.Y;
					links.Clear();
				}
			}
			if(PopupBarControl != null) {
				if(!preCalc) {
					if(insertUpScroll) BarControl.VisibleLinks.AddItem(PopupBarControl.ControlLinks.UpScrollLink);
					if(insertDownScroll) BarControl.VisibleLinks.AddItem(PopupBarControl.ControlLinks.DownScrollLink);
				} else {
					PopupBarControl.ControlLinks.UpScrollLink.LinkHeight = upPrevHeight;
					PopupBarControl.ControlLinks.DownScrollLink.LinkHeight = downPrevHeight;
				}
			}
			if(links.Count > 0 && !preCalc) {
				if(rect.Width > 0) maxWidth = rect.Width;
				int linkWidth = maxWidth - MenuBarWidth;
				Rectangle r;
				cc = 0;
				foreach(SubMenuLinkInfo info in links) {
					BarLinkViewInfo linkViewInfo = info.LinkInfo;
					linkViewInfo.UpdateLinkWidthInSubMenu(linkWidth);
					linkViewInfo.ReverseLinkRects();
					if(linkViewInfo.Link.GetBeginGroup() && cc > 0) {
						r = linkViewInfo.Bounds;
						r.Y -= DrawParameters.Constants.SubMenuSeparatorHeight;
						r.Height = DrawParameters.Constants.SubMenuSeparatorHeight;
						linkViewInfo.Rects.AddExtRectangle(new RectInfo(null, r, RectInfoType.SubMenuSeparator));
					}
					if(linkViewInfo is BarScrollItemLinkViewInfo && cc > 0) {
						r = linkViewInfo.Bounds;
						r.Y = rect.Bottom - r.Height;
						linkViewInfo.Bounds = r;
					}
					cc++;
				}
			}
			GInfo.ReleaseGraphics();
			return links;
		}
		protected internal virtual bool UseLargeImagesInGallery(BarItemLink link) {
			OptionsMultiColumn optionsMultiColumn = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsMultiColumn != null && optionsMultiColumn.LargeImages != DefaultBoolean.Default)
				return optionsMultiColumn.LargeImages == DefaultBoolean.True;
			optionsMultiColumn = GetOwnerOptionsGalleryMenu();
			if(optionsMultiColumn != null && optionsMultiColumn.LargeImages != DefaultBoolean.Default)
				return optionsMultiColumn.LargeImages == DefaultBoolean.True;
			DefaultBoolean res = Manager.GetController().PropertiesBar.OptionsMultiColumn.LargeImages;
			return res == DefaultBoolean.Default ? false : res == DefaultBoolean.False;
		}
		protected internal virtual bool GetShowItemTextInGallery(BarItemLink link) {
			OptionsMultiColumn optionsGallery = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsGallery != null && optionsGallery.ShowItemText != DefaultBoolean.Default)
				return optionsGallery.ShowItemText == DefaultBoolean.True;
			optionsGallery = GetOwnerOptionsGalleryMenu();
			if(optionsGallery != null && optionsGallery.ShowItemText != DefaultBoolean.Default)
				return optionsGallery.ShowItemText == DefaultBoolean.True;
			DefaultBoolean res = Manager.GetController().PropertiesBar.OptionsMultiColumn.ShowItemText;
			return res == DefaultBoolean.Default? false: res == DefaultBoolean.True;
		}
		protected internal int GetItemMaxWidth(BarItemLink link) {
			OptionsMultiColumn optionsGallery = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsGallery != null && optionsGallery.MaxItemWidth > 0)
				return optionsGallery.MaxItemWidth;
			optionsGallery = GetOwnerOptionsGalleryMenu();
			if(optionsGallery != null && optionsGallery.MaxItemWidth > 0)
				return optionsGallery.MaxItemWidth;
			int res = Manager.GetController().PropertiesBar.OptionsMultiColumn.MaxItemWidth;
			return res > 0 ? res : 0;
		}
		protected internal virtual bool GetLinkMultiColumn(BarItemLink link) {
			return PopupBarControl.GetLinkMultiColumn(link);
		}
		protected BarHeaderItem GetHeaderItemForLink(BarItemLink link) {
			return PopupBarControl.GetHeaderItemForLink(link);
		}
		protected OptionsMultiColumn GetHeaderItemOptionsGalleryMenu(BarItemLink link) {
			BarHeaderItem item = GetHeaderItemForLink(link);
			return item == null? null: item.OptionsMultiColumn;
		}
		protected OptionsMultiColumn GetOwnerOptionsGalleryMenu() {
			return PopupBarControl.GetOptionsGalleryMenu();
		}
		protected internal ItemHorizontalAlignment GetGalleryImageHorizontalAlignment(BarItemLink link) {
			OptionsMultiColumn optionsMultiColumn = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsMultiColumn != null && optionsMultiColumn.ImageHorizontalAlignment != ItemHorizontalAlignment.Default)
				return optionsMultiColumn.ImageHorizontalAlignment;
			optionsMultiColumn = GetOwnerOptionsGalleryMenu();
			if(optionsMultiColumn != null && optionsMultiColumn.ImageHorizontalAlignment != ItemHorizontalAlignment.Default)
				return optionsMultiColumn.ImageHorizontalAlignment;
			ItemHorizontalAlignment res = Manager.GetController().PropertiesBar.OptionsMultiColumn.ImageHorizontalAlignment;
			return res != ItemHorizontalAlignment.Default? res: ItemHorizontalAlignment.Center;
		}
		protected internal ItemVerticalAlignment GetGalleryImageVerticalAlignment(BarItemLink link) {
			OptionsMultiColumn optionsMultiColumn = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsMultiColumn != null && optionsMultiColumn.ImageVerticalAlignment != ItemVerticalAlignment.Default)
				return optionsMultiColumn.ImageVerticalAlignment;
			optionsMultiColumn = GetOwnerOptionsGalleryMenu();
			if(optionsMultiColumn != null && optionsMultiColumn.ImageVerticalAlignment != ItemVerticalAlignment.Default)
				return optionsMultiColumn.ImageVerticalAlignment;
			ItemVerticalAlignment res = Manager.GetController().PropertiesBar.OptionsMultiColumn.ImageVerticalAlignment;
			return res != ItemVerticalAlignment.Default ? res : ItemVerticalAlignment.Center;
		}
		protected internal bool GetUseMaxItemWidth(BarItemLink link) {
			OptionsMultiColumn optionsGallery = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsGallery != null && optionsGallery.UseMaxItemWidth != DefaultBoolean.Default)
				return optionsGallery.UseMaxItemWidth == DefaultBoolean.True;
			optionsGallery = GetOwnerOptionsGalleryMenu();
			if(optionsGallery != null && optionsGallery.UseMaxItemWidth != DefaultBoolean.Default)
				return optionsGallery.UseMaxItemWidth == DefaultBoolean.True;
			DefaultBoolean res = Manager.GetController().PropertiesBar.OptionsMultiColumn.UseMaxItemWidth;
			return res == DefaultBoolean.Default ? false : res == DefaultBoolean.True;
		}
		protected virtual int GetGalleryItemIndent(BarItemLink link) {
			OptionsMultiColumn optionsMultiColumn = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsMultiColumn != null && optionsMultiColumn.ItemIndent > -1)
				return optionsMultiColumn.ItemIndent;
			optionsMultiColumn = GetOwnerOptionsGalleryMenu();
			if(optionsMultiColumn != null && optionsMultiColumn.ItemIndent > -1)
				return optionsMultiColumn.ItemIndent;
			int res = Manager.GetController().PropertiesBar.OptionsMultiColumn.ItemIndent;
			return res > 0 ? res : DefaultGalleryItemIndent;
		}
		protected internal virtual int GetGalleryRowCount(BarItemLink current) {
			int startIndex = GetGalleryStartItemIndex(current);
			int endIndex = GetGalleryEndItemIndex(current);
			int columnCount = GetGalleryColumnCount(current);
			int row = (endIndex - startIndex + 1) / columnCount;
			if((endIndex - startIndex + 1) % columnCount > 0)
				row++;
			return row;
		}
		protected internal virtual int GetGalleryColumnCount(BarItemLink link) {
			OptionsMultiColumn optionsGallery = GetHeaderItemOptionsGalleryMenu(link);
			if(optionsGallery != null && optionsGallery.ColumnCount > 0)
				return optionsGallery.ColumnCount;
			optionsGallery = GetOwnerOptionsGalleryMenu();
			if(optionsGallery != null && optionsGallery.ColumnCount > 0)
				return optionsGallery.ColumnCount;
			int res = Manager.GetController().PropertiesBar.OptionsMultiColumn.ColumnCount;
			return res > 0? res: DefaultGalleryColumnCount;
		}
		protected virtual int DefaultGalleryColumnCount { get { return 5; } }
		protected virtual int DefaultGalleryItemIndent { get { return 0; } }
		public virtual int CalcGlyphWidth(int glyphWidth) {
			return glyphWidth;
		}
		public void CalcWidthes() {
			foreach(SubMenuLinkInfo linkInfo in BarLinksViewInfo) {
				GlyphWidth = CalcGlyphWidth(Math.Max(linkInfo.LinkInfo.GlyphRect.Width + DrawParameters.Constants.SubMenuGlyphHorzIndent + DrawParameters.Constants.SubMenuGlyphCaptionIndent, GlyphWidth));
				TextWidth = Math.Max(linkInfo.LinkInfo.Bounds.Width - GlyphWidth, TextWidth);
				TextIndent = linkInfo.LinkInfo.CaptionRect.X - linkInfo.LinkInfo.Bounds.X;
			}
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			Clear();
			Update();
			g = GInfo.AddGraphics(g);
			try {
				this.fBounds = rect;
				NavigationHeaderBounds = CalcNavigationHeaderBounds(Bounds);
				NavigationHeaderContentBounds = CalcNavigationHeaderContentBounds(NavigationHeaderBounds);
				CalcNavigationItemsViewInfo(NavigationHeaderContentBounds);
				this.LinksBounds = CalcLinksBounds(Bounds);
				this.SourceObject = sourceObject;
				CalcMaxWidthes();
				RemoveAnimatedItems(BarLinksViewInfo);
				BarLinksViewInfo = PreCalcViewInfo(g, sourceObject, PopupBarControl != null ? PopupBarControl.TopLinkIndex : 0, LinksBounds, true, false); 
				CalcWidthes();
				UpdateAnimatedLinks(BarLinksViewInfo);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			ready = true;
		}
		protected virtual bool CanUseUniversalNavigation() {
			if(Manager == null || Manager.Form == null) return false;
			DevExpress.XtraBars.Ribbon.RibbonControl ribbon = Manager.Form as DevExpress.XtraBars.Ribbon.RibbonControl;
			if(ribbon == null) return false;
			return ribbon.GetRibbonStyle() == Ribbon.RibbonControlStyle.OfficeUniversal;
		}
		protected virtual void CalcNavigationItemsViewInfo(Rectangle rect) {
			if(!ShowNavigationHeader) return;
			if(CanUseUniversalNavigation())
				CalcUniversalNavigationItemsViewInfo(rect);
			else
				CalcDefaultNavigationItemsViewInfo(rect);
		}
		protected virtual void CalcUniversalNavigationItemsViewInfo(Rectangle rect) {
			int x = rect.X;
			Size glyph = CalcNavigationBackArrowSize(GInfo.Graphics);
			NavigationArrowBounds = new Rectangle(x, rect.Y + (rect.Height - glyph.Height) / 2, glyph.Width, glyph.Height);
			if(NavigationItems.Count > 1)
				x += glyph.Width + BackArrow2NavigationItemIndent;
			for(int i = 0; i < NavigationItems.Count; i++) {
				NavigationItems[i].Bounds = new Rectangle(x, rect.Y + (rect.Height - NavigationItems[i].Bounds.Height) / 2, NavigationItems[i].Bounds.Width, NavigationItems[i].Bounds.Height);
				x = NavigationItems[i].Bounds.Right + NavigationItemHorzIndent;
			}
		}
		protected virtual void CalcDefaultNavigationItemsViewInfo(Rectangle rect) {
			int x = rect.X;
			if(NavigationItems.Count == 1) { 
				NavigationItems[0].Bounds = new Rectangle(rect.X + (rect.Width - NavigationItems[0].Bounds.Width) / 2, rect.Y + (rect.Height - NavigationItems[0].Bounds.Height) / 2, NavigationItems[0].Bounds.Width, NavigationItems[0].Bounds.Height);
				return;
			}
			Size glyph = CalcNavigationBackArrowSize(GInfo.Graphics);
			NavigationArrowBounds = new Rectangle(x, rect.Y + (rect.Height - glyph.Height) / 2, glyph.Width, glyph.Height);
			x += glyph.Width + BackArrow2NavigationItemIndent;
			for(int i = 0; i < NavigationItems.Count; i++) {
				NavigationItems[i].Bounds = new Rectangle(x, rect.Y + (rect.Height - NavigationItems[i].Bounds.Height) / 2, NavigationItems[i].Bounds.Width, NavigationItems[i].Bounds.Height);
				x = NavigationItems[i].Bounds.Right + NavigationItemHorzIndent;
			}
		}
		public Rectangle NavigationArrowBounds { get; set; }
		protected virtual Rectangle CalcNavigationHeaderBounds(Rectangle rect) {
			return new Rectangle(rect.X, rect.Y, rect.Width, CalcNavigationHeaderSize(GInfo.Graphics).Height);
		}
		protected virtual Rectangle CalcNavigationHeaderContentBounds(Rectangle rect) {
			if(!ShowNavigationHeader)
				return rect;
			return PaintStyle.CalcNavigationHeaderContentBounds(GInfo.Graphics, rect);
		}
		protected internal virtual void RemoveAnimatedItems(List<SubMenuLinkInfo> links) {
			foreach(SubMenuLinkInfo linkInfo in links) {
				if(XtraAnimator.Current.Get((ISupportXtraAnimation)BarControl, linkInfo.LinkInfo.Link) != null)
					XtraAnimator.Current.Animations.Remove((ISupportXtraAnimation)BarControl, linkInfo.LinkInfo.Link);
			}
		}
		protected virtual void UpdateAnimatedLinks(List<SubMenuLinkInfo> links) {
			foreach(SubMenuLinkInfo linkInfo in links) {
				linkInfo.LinkInfo.Link.UpdateAnimatedLink(linkInfo.LinkInfo, (ISupportXtraAnimation)BarControl, BarControl.AnimationInvoker);
			}
		}
		public override BarLinkViewInfo GetLinkViewInfo(BarItemLink link, LinkViewInfoRange infoRange) {
			bool found = false;
			BarLinkViewInfo prevInfo = null;
			foreach(SubMenuLinkInfo info in BarLinksViewInfo) {
				if(found && infoRange == LinkViewInfoRange.Next) return info.LinkInfo;
				if(info.LinkInfo.Link == link) {
					found = true;
					if(infoRange != LinkViewInfoRange.Next) 
						return (infoRange == LinkViewInfoRange.Current ? info.LinkInfo : prevInfo);
				}
				prevInfo = info.LinkInfo;
			}
			return null;
		}
		public override BarLinkViewInfo GetLinkViewInfoByPoint(Point p, bool includeSeparator) {
			foreach(SubMenuLinkInfo info in BarLinksViewInfo) {
				if(info.LinkInfo.Bounds.Contains(p)) return info.LinkInfo;
				foreach(RectInfo rInfo in info.LinkInfo.Rects.ExtRectangles) {
					if(includeSeparator && (rInfo.Type == RectInfoType.SubMenuSeparator) &&
						rInfo.Rect.Contains(p)) return info.LinkInfo;
				}
			}
			return null;
		}
		protected internal SubMenuLinkInfo GetLinkViewInfo(BarItemLink link, List<SubMenuLinkInfo> list) {
			return list.Find((l) => l.LinkInfo.Link == link);
		}
		SubMenuLinkInfo GetAnyNonScrollLink() {
			foreach(SubMenuLinkInfo mi in BarLinksViewInfo) {
				if(mi.LinkInfo.Link is BarScrollItemLink) continue;
				return mi;
			}
			return null;
		}
		public virtual void MakeLinkVisible(BarItemLink link) {
			if(PopupBarControl == null) return;
			int index = BarControl.VisibleLinks.IndexOf(link), delta, vCount = BarControl.VisibleLinks.Count;
			if(index == -1) return;
			if(link is BarScrollItemLink) return;
			if(link is BarRecentExpanderItemLink) return;
			if(BarLinksViewInfo.Count == 0) return;
			if(GetLinkViewInfo(link, LinkViewInfoRange.Current) != null) return;
			SubMenuLinkInfo info = GetAnyNonScrollLink();
			if(info == null) return;
			int viIndex = BarControl.VisibleLinks.IndexOf(info.LinkInfo.Link);
			int topIndex = PopupBarControl.TopLinkIndex;
			delta = index > viIndex ? 1 : -1;
			Graphics g = GInfo.AddGraphics(null);
			try {
				for(int n = 0; n < vCount; n++) {
					topIndex += delta;
					if(topIndex < 0) return;
					List<SubMenuLinkInfo> list = PreCalcViewInfo(g, SourceObject, topIndex, LinksBounds, true, true);
					if(GetLinkViewInfo(link, list) != null) {
						PopupBarControl.TopLinkIndex = topIndex;
						return;
					}
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual MenuNavigationItem GetNavigationItemByPoint(Point pt) {
			foreach(MenuNavigationItem item in NavigationItems) {
				if(item.Bounds.Contains(pt)) {
					return item;
				}
			}
			return null;
		}
		MenuNavigationItem downNavigationItem;
		protected internal MenuNavigationItem DownNavigationItem {
			get { return downNavigationItem; }
			set {
				if(DownNavigationItem == value)
					return;
				downNavigationItem = value;
				PopupBarControl.Invalidate();
			}
		}
		MenuNavigationItem hoverNavigationItem;
		protected internal MenuNavigationItem HoverNavigationItem {
			get { return hoverNavigationItem; }
			set {
				if(HoverNavigationItem == value)
					return;
				hoverNavigationItem = value;
				PopupBarControl.Invalidate();
			}
		}
		protected internal virtual void ProcessNavigationItemsMouseMove(MouseEventArgs e) {
			HoverNavigationItem = GetNavigationItemByPoint(e.Location);
		}
		protected internal virtual void ProcessNavigationItemsMouseDown(DXMouseEventArgs ee) {
			DownNavigationItem = GetNavigationItemByPoint(ee.Location);
		}
		protected internal virtual bool ProcessNavigationItemsMouseUp(MouseEventArgs ee) {
			MenuNavigationItem item = GetNavigationItemByPoint(ee.Location);
			if(item == null)
				return false;
			MenuNavigationItem down = DownNavigationItem;
			DownNavigationItem = null;
			if(down == item && item.Popup != PopupBarControl) {
				PopupBarControl.OnMenuNavigationItemClick(item);
				return true;
			}
			return false;
		}
		protected internal SubMenuLinkInfo GetLastGalleryItemInRow(SubMenuLinkInfo subInfo) {
			int linkIndex = BarLinksViewInfo.IndexOf(subInfo);
			for(int i = linkIndex + 1; i < BarLinksViewInfo.Count; i++) {
				if(BarLinksViewInfo[i].Row != subInfo.Row)
					return BarLinksViewInfo[i - 1];
			}
			return BarLinksViewInfo[BarLinksViewInfo.Count - 1];
		}
	}
	public class PopupContainerControlViewInfo : CustomSubMenuBarControlViewInfo {
		Rectangle closeButtonRect, sizeGripRect, buttonsRect;
		public PopupContainerControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar)
			: base(manager, parameters, bar) {
			this.sizeGripInfo = new SizeGripObjectInfoArgs();
		}
		public bool ShowCloseButton { get { return ((PopupContainerBarControl)BarControl).ShowCloseButton; } }
		public bool ShowSizeGrip { get { return ((PopupContainerBarControl)BarControl).ShowSizeGrip; } }
		public Rectangle CloseButtonRect { get { return closeButtonRect; } }
		public Rectangle SizeGripRect { get { return sizeGripRect; } }
		public Rectangle ButtonsRect { get { return buttonsRect; } }
		protected internal override bool ShowMenuCaption { get { return false; } }
		public override string MenuCaption { get { return string.Empty; } }
		protected internal override MenuDrawMode DrawMode { get { return MenuDrawMode.Default; } }
		SizeGripObjectInfoArgs sizeGripInfo;
		public SizeGripObjectInfoArgs SizeGripInfo { get { return sizeGripInfo; } }
		protected internal virtual void CalcButtonsBounds(Graphics g, object sourceObject, Rectangle r) {
			closeButtonRect = Rectangle.Empty;
			sizeGripRect = Rectangle.Empty;
			buttonsRect = Rectangle.Empty;
			bool isLeftSizeGrip = IsLeftSizeGrip;
			if(!ShowCloseButton && !ShowSizeGrip) return;
			if(ShowCloseButton) closeButtonRect.Size = BarControl.CloseButton.CalcBestFit(g);
			SizeGripInfo.Graphics = g;
			if(ShowSizeGrip) sizeGripRect.Size = CalcSizeGripBestFit();
			buttonsRect.Size = new Size(r.Width, Math.Max(CloseButtonRect.Height + PopupContainerBarControl.ButtonsIndent * 2, SizeGripRect.Height));
			if(IsTopSizeBar) {
				buttonsRect.Location = new Point(r.X, r.Y);
				sizeGripRect.Y = buttonsRect.Y;
			}
			else {
				buttonsRect.Location = new Point(r.X, r.Bottom - ButtonsRect.Height);
				sizeGripRect.Y = ButtonsRect.Bottom - sizeGripRect.Height;
			}
			closeButtonRect.Y = ButtonsRect.Y + (ButtonsRect.Height - closeButtonRect.Height) / 2;	
			if(!isLeftSizeGrip || !ShowSizeGrip) {
				if(ShowCloseButton) closeButtonRect.X = ButtonsRect.X + PopupContainerBarControl.ButtonsIndent;
				if(ShowSizeGrip) sizeGripRect.X = r.Right - sizeGripRect.Width;
			}
			else {
				if(ShowCloseButton) closeButtonRect.X = ButtonsRect.Right - closeButtonRect.Width - PopupContainerBarControl.ButtonsIndent;
				if(ShowSizeGrip) sizeGripRect.X = r.X;
			}
			SizeGripInfo.Bounds = SizeGripRect;
			SizeGripInfo.GripPosition = CalcSizeGripPosition(IsTopSizeBar, IsLeftSizeGrip);
		}
		protected virtual SizeGripPosition CalcSizeGripPosition(bool isTop, bool isLeft) {
			if(BarControl.CurrentSizing) return BarControl.CurrentSizeGripPosition;
			if(isTop)
				return isLeft? SizeGripPosition.LeftTop : SizeGripPosition.RightTop;
			return isLeft ? SizeGripPosition.LeftBottom : SizeGripPosition.RightBottom;
		}
		public new PopupContainerBarControl BarControl { get { return base.BarControl as PopupContainerBarControl; } }
		Screen lastScreen = null;
		Rectangle lastScreenBounds = Rectangle.Empty;
		protected internal virtual bool IsTopSizeBar {
			get {
				if(BarControl.CurrentSizing) return BarControl.CurrentIsTopSizeBar;
				if(!ShowCloseButton && !ShowSizeGrip) return false;
				return BarControl.Form.Location.Y < BarControl.Form.ViewInfo.OwnerRectangle.Y; 
			}
		}
		protected virtual bool IsLeftSizeGrip {
			get {
				if(BarControl.CurrentSizing) 
					return BarControl.CurrentSizeGripPosition == SizeGripPosition.LeftBottom || BarControl.CurrentSizeGripPosition == SizeGripPosition.LeftTop? true: false;
				if(!BarControl.Form.Visible)
					return false;
				Rectangle bounds = BarControl.Form.Bounds;
				Screen scr = null;
				if(bounds == lastScreenBounds) scr = lastScreen;
				if(scr == null) {
					lastScreen = scr = Screen.FromPoint(bounds.Location);
					lastScreenBounds = bounds;
				}
				int deltaLeft = bounds.Left - scr.WorkingArea.Left,
					deltaRight = scr.WorkingArea.Right - bounds.Right;
				if(scr.WorkingArea.Width > 0 && deltaLeft > scr.WorkingArea.Width / 3) {
					if(scr.WorkingArea.Right - bounds.Right < 50) return true;
				}
				return false;
			}
		}
		protected virtual int CalcControlBottomIndent() {
			int indent = 0;
			if(ShowCloseButton) indent = CloseButtonRect.Height;
			if(ShowSizeGrip) indent = Math.Max(indent, SizeGripRect.Height);
			return indent;
		}
		protected virtual Size CalcSizeGripBestFit() {
			SizeGripObjectPainter p = null;
			if(Manager.GetController().LookAndFeel.Style != DevExpress.LookAndFeel.LookAndFeelStyle.Skin)
				p = new SizeGripObjectPainter();
			else
				p = new SkinSizeGripObjectPainter(Manager.GetController().LookAndFeel);
			return p.CalcObjectMinBounds(SizeGripInfo).Size; 
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			g = GInfo.AddGraphics(g);
			try {
				NavigationHeaderBounds = CalcNavigationHeaderBounds(rect);
				NavigationHeaderContentBounds = CalcNavigationHeaderContentBounds(NavigationHeaderBounds);
				CalcNavigationItemsViewInfo(NavigationHeaderContentBounds);
				rect.Y += NavigationHeaderBounds.Height;
				rect.Height -= NavigationHeaderBounds.Height;
				CalcButtonsBounds(g, sourceObject, rect);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
	}
	public class SubMenuBarControlViewInfo : CustomSubMenuBarControlViewInfo {
		public virtual new SubMenuBarControl BarControl { get { return base.BarControl as SubMenuBarControl; } }
		public SubMenuBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		protected internal override bool IsRightToLeft { get { return Manager != null && Manager.IsRightToLeft; } }
		public override bool ShowNavigationHeader {
			get {
				if(BarControl.ContainerLink.ShowNavigationHeader != DefaultBoolean.Default)
					return Manager.GetPopupShowMode(null) == PopupShowMode.Inplace && BarControl.ContainerLink.ShowNavigationHeader == DefaultBoolean.True;
				return base.ShowNavigationHeader;
			}
		}
		protected override MenuAppearance PopupAppearance {
			get { 
				if(BarControl != null && (BarControl.OwnerLink is BarCustomContainerItemLink)) {
					BarCustomContainerItemLink link = BarControl.OwnerLink as BarCustomContainerItemLink;
					return link.Item.MenuAppearance;
				}
				return base.PopupAppearance;
			}
		}
		public BarCustomContainerItem Item { get { return ItemLink == null ? null : ItemLink.Item; } }
		public BarCustomContainerItemLink ItemLink { get { return BarControl == null ? null : BarControl.OwnerLink as BarCustomContainerItemLink; } }
		public override string MenuCaption {
			get {
				if(Item == null) return string.Empty;
				return Item.MenuCaption;
			}
		}
		protected internal override MenuDrawMode DrawMode { get { return Item == null ? MenuDrawMode.Default : Item.MenuDrawMode; } }
		protected internal override bool ShowMenuCaption { 
			get {
				if(ShowNavigationHeader)
					return false;
				return Item != null && MenuCaption.Length > 0 && Item.ShowMenuCaption; 
			} 
		}
	}
	public class PopupMenuBarControlViewInfo : CustomSubMenuBarControlViewInfo {
		public virtual new PopupMenuBarControl BarControl { get { return base.BarControl as PopupMenuBarControl; } }
		public PopupMenuBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public override bool ShowNavigationHeader {
			get {
				if(string.IsNullOrEmpty(BarControl.Menu.MenuCaption)) return false;
				if(BarControl.Menu.ShowNavigationHeader != DefaultBoolean.Default)
					return Manager.GetPopupShowMode(null) == PopupShowMode.Inplace && BarControl.Menu.ShowNavigationHeader == DefaultBoolean.True;
				return base.ShowNavigationHeader;
			}
		}
		protected override MenuAppearance PopupAppearance {
			get { 
				if(BarControl != null && BarControl.Menu != null)
					return BarControl.Menu.MenuAppearance;
				return base.PopupAppearance;
			}
		}
		public PopupMenu Menu { get { return BarControl == null ? null : BarControl.Menu; } }
		public override string MenuCaption {
			get {
				if(Menu == null) return string.Empty;
				return Menu.MenuCaption;
			}
		}
		protected internal override MenuDrawMode DrawMode { get { return Menu == null ? MenuDrawMode.Default : Menu.MenuDrawMode; } }
		protected internal override bool ShowMenuCaption { 
			get {
				if(ShowNavigationHeader)
					return false;
				return Menu != null && MenuCaption.Length > 0 && Menu.ShowCaption;  
			} 
		}
	}
	public class MenuNavigationItem {
		public MenuNavigationItem(CustomSubMenuBarControlViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		public ObjectState State {
			get {
				if(ViewInfo.DownNavigationItem == this)
					return ObjectState.Pressed;
				if(ViewInfo.HoverNavigationItem == this)
					return ObjectState.Hot;
				return ObjectState.Normal;
			}
		}
		public IPopup Popup { get; set; }
		public CustomSubMenuBarControlViewInfo ViewInfo { get; private set; }
		public Rectangle Bounds { get; set; }
		public string Text {
			get {
				CustomPopupBarControl control = Popup as CustomPopupBarControl;
				if(control == null) return string.Empty;
				if(control.PopupCreator is PopupMenu) return GetMenuCaption(control.PopupCreator as PopupMenu);
				if(control.OwnerLink is BarItemLink) return GetSubMenuCaption(control.OwnerLink as BarItemLink);
				return string.Empty;
			}
		}
		string GetMenuCaption(PopupMenu menu) {
			if(menu == null) return string.Empty;
			return string.IsNullOrEmpty(menu.MenuCaption) ? menu.Name : menu.MenuCaption;
		}
		string GetSubMenuCaption(BarItemLink link) {
			if(link == null) return string.Empty;
			BarSubItemLink subItem = link as BarSubItemLink;
			if(subItem != null && string.IsNullOrEmpty(subItem.Item.MenuCaption)) return subItem.Item.Caption;
			return link.Caption;
		}
	}
}
