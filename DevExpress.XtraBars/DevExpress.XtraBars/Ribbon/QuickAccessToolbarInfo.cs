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
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.Skins;
using System.Drawing.Imaging;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RibbonQuickAccessToolbarInfoArgs : ObjectInfoArgs {
		RibbonQuickAccessToolbarViewInfo viewInfo;
		public RibbonQuickAccessToolbarInfoArgs(RibbonQuickAccessToolbarViewInfo viewInfo, GraphicsCache cache)
			: base(cache) {
			this.viewInfo = viewInfo;
		}
		public RibbonQuickAccessToolbarInfoArgs(RibbonQuickAccessToolbarViewInfo viewInfo, GraphicsCache cache, Rectangle bounds, ObjectState state)
			: base(cache, bounds, state) {
			this.viewInfo = viewInfo;
		}
		public RibbonQuickAccessToolbarInfoArgs(RibbonQuickAccessToolbarViewInfo viewInfo)
			: base() {
			this.viewInfo = viewInfo;
		}
		public RibbonQuickAccessToolbarViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class RibbonQuickAccessToolbarPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonQuickAccessToolbarViewInfo vi = ((RibbonQuickAccessToolbarInfoArgs)e).ViewInfo;
			if(vi.Bounds.IsEmpty) return;
			if(!vi.ViewInfo.IsOfficeTablet)
				DrawBackground(e, vi);
			DrawItems(e, vi);
			if(!vi.ViewInfo.IsAllowCaption)
				DrawPageCategories(e, vi);
			DrawDesignerRect(e, vi);
		}
		protected virtual void DrawDesignerRect(ObjectInfoArgs e, RibbonQuickAccessToolbarViewInfo vi) {
			if(!vi.ViewInfo.IsDesignMode) return;
			Pen pen = new Pen(Color.Red);
			pen.DashPattern = new float[] { 5.0f, 5.0f };
			e.Cache.DrawRectangle(pen, vi.DesignerRect);
		}
		protected virtual void DrawPageCategories(ObjectInfoArgs e, RibbonQuickAccessToolbarViewInfo vi) {
			if(vi.ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Below || !vi.Toolbar.Ribbon.ShowCategoryInCaption) return;
			RibbonDrawInfo info = new RibbonDrawInfo(vi.ViewInfo);
			info.Cache = e.Cache;
			new RibbonPageCategoryUpperPartPainter().DrawObject(info);
		}
		protected virtual void DrawBackground(ObjectInfoArgs e, RibbonQuickAccessToolbarViewInfo vi) {
			SkinElementInfo res = vi.GetBackgroundInfo();
			if(res != null) ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, res);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.GetToolbarInfo());
		}
		protected virtual void DrawItems(ObjectInfoArgs e, RibbonQuickAccessToolbarViewInfo vi) {
			for(int i = 0; i < vi.VisibleButtonCount; i++) {
				RibbonItemViewInfoCalculator.DrawItem(e.Cache, vi.Items[i]);
			}
		}
	}
	public class RibbonQuickAccessToolbarViewInfo {
		public int DropdownButtonWidth = 11;
		public int SpaceBeforeDropdownButton = 6;
		RibbonItemViewInfoCollection items;
		RibbonQuickAccessToolbar toolbar;
		Rectangle bounds, contentBackgroundBounds, itemsContentBackgroundBounds;
		Rectangle designerRect;
		Rectangle contentBounds, itemsContentBounds;
		int visibleButtonCount;
		bool visible = true;
		public RibbonQuickAccessToolbarViewInfo(RibbonQuickAccessToolbar toolbar) {
			this.toolbar = toolbar;
			this.bounds = Rectangle.Empty;
			this.contentBackgroundBounds = Rectangle.Empty;
			this.contentBounds = Rectangle.Empty;
			this.itemsContentBackgroundBounds = this.itemsContentBounds = Rectangle.Empty;
			this.items = new RibbonItemViewInfoCollection(this);
			this.visibleButtonCount = 0;
		}
		public ISkinProvider Provider { get { return ViewInfo == null ? UserLookAndFeel.Default : ViewInfo.Provider; } }
		public RibbonViewInfo ViewInfo {
			get {
				if(Toolbar == null) return null;
				return Toolbar.Ribbon.ViewInfo;
			}
		}
		public RibbonItemViewInfoCollection Items { get { return items; } }
		public RibbonQuickAccessToolbar Toolbar { get { return toolbar; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ContentBackgroundBounds { get { return contentBackgroundBounds; } set { contentBackgroundBounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ItemsContentBounds { get { return itemsContentBounds; } set { itemsContentBounds = value; } }
		public Rectangle ItemsContentBackgroundBounds { get { return itemsContentBackgroundBounds; } set { itemsContentBackgroundBounds = value; } }
		protected virtual GraphicsInfo GInfo { get { return ViewInfo.GInfo; } }
		protected internal virtual int IndentBeforeDropDownButton {
			get {
				SkinElementInfo backInfo = GetToolbarInfo();
				if(backInfo == null || backInfo.Element == null) return SpaceBeforeDropdownButton;
				int indentBeforeDropDown = backInfo.Element.Properties.GetInteger("IndentBeforeCustomizeItem");
				if(indentBeforeDropDown == 0) return SpaceBeforeDropdownButton;
				return indentBeforeDropDown;
			}
		}
		protected virtual bool IsCustomizeItemOutside { 
			get {
				SkinElementInfo backInfo = GetToolbarInfo();
				if(backInfo == null || backInfo.Element == null) return false;
				return backInfo.Element.Properties.GetBoolean("CustomizeItemOutsideToolbar");
			} 
		}
		protected virtual Rectangle UpdateItemsContentBounds(Rectangle rect) {
			if(!IsCustomizeItemOutside || !IsToolbarCustomizationItem(VisibleButtonCount - 1)) return rect;
			return new Rectangle(rect.Location, new Size(rect.Width - IndentBeforeDropDownButton - Items[VisibleButtonCount - 1].Bounds.Width, rect.Height) );
		}
		protected virtual void ClearBounds() {
			this.bounds = Rectangle.Empty;
			this.contentBounds = Rectangle.Empty;
			this.contentBackgroundBounds = Rectangle.Empty;
			this.itemsContentBounds = Rectangle.Empty;
			this.itemsContentBackgroundBounds = Rectangle.Empty;
		}
		Rectangle outerBounds = Rectangle.Empty;
		protected internal virtual Rectangle OuterBounds {
			get { return outerBounds; }
			set { outerBounds = value; }
		}
		protected internal bool SupportForRibbonStyle(RibbonControlStyle style) {
			return style != RibbonControlStyle.OfficeUniversal;
		} 
		public virtual void CalcViewInfo(Rectangle bounds) {
			Clear();
			if(!SupportForRibbonStyle(ViewInfo.GetRibbonStyle())) {
				this.outerBounds = this.bounds = this.designerRect = Rectangle.Empty;
				return;
			}
			else {
				this.outerBounds = bounds;
				this.bounds = bounds;
			}
			SkinElementInfo info = GetToolbarInfo();
			if(info.Element != null && ViewInfo.GetRibbonStyle() != RibbonControlStyle.TabletOffice) {
				this.bounds.X += info.Element.Offset.Offset.X;
				this.bounds.Width -= info.Element.Offset.Offset.X;
				this.bounds.Width = Math.Max(0, this.bounds.Width);
				this.bounds.Y += info.Element.Offset.Offset.Y;
			}
			if(Bounds.IsEmpty) return;
			GInfo.AddGraphics(null);
			try {
				CreateItemsViewInfo();
				this.contentBounds = CalcContentBounds();
				this.contentBackgroundBounds = CalcContentBackgroundBounds();
				CalcContentInfo(CalcBestSize().Width);
				if (Visible) {
					this.contentBackgroundBounds = UpdateContentBackgroundBounds(ContentBounds);
					this.itemsContentBounds = UpdateItemsContentBounds(ContentBounds);
					this.itemsContentBackgroundBounds = UpdateContentBackgroundBounds(ItemsContentBounds);
				}
				else {
					ClearBounds();
				}
				ViewInfo.CheckToolbarRTL();
				CalcDesignerRect();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void Clear() {
			ClearItemsViewInfo();
		}
		public virtual BarItemLink GetLinkByPoint(Point pt) {
			for(int i = 0; i < VisibleButtonCount; i++) {
				if(Items[i].Bounds.Contains(pt)) return (Items[i].Item as BarItemLink);
			}
			return null;
		}
		public bool Visible { get { return visible; } }
		protected virtual void CalcContentInfo(int bestWidth) {
			this.visible = true;
			if(ContentBounds.Width >= bestWidth)
				CalcFitContentInfo(bestWidth);
			else {
				int prevContentWidth = ContentBounds.Width;
				CalcNonFitContentInfo(bestWidth);
				if (ContentBounds.Width > prevContentWidth && VisibleButtonCount == 1) {
					this.visible = false;
				}
			}
		}
		protected virtual Rectangle UpdateContentBackgroundBounds(Rectangle bounds) {
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetToolbarInfo(), bounds);
		}
		protected virtual void CalcFitContentInfo(int bestWidth) {
			this.visibleButtonCount = Items.Count;
			this.contentBounds.Width = bestWidth;
			UpdateItemsLayout(ContentBounds, 0, Items.Count);
		}
		protected virtual void CalcNonFitContentInfo(int bestWidth) {
			int mw = Math.Max(ViewInfo.CreateItemViewInfo(Toolbar.DropDownItemLink).CalcBestSize().Width, Items[Items.Count - 1].CalcBestSize().Width) + SpaceBeforeDropdownButton;
			this.visibleButtonCount = GetVisibleButtonCount(ContentBounds.Width - mw);
			InsertDropdownButton(VisibleButtonCount);
			visibleButtonCount++;
			contentBounds.Width = CalcContentSize(0, VisibleButtonCount, true).Width;
			UpdateItemsLayout(ContentBounds, 0, VisibleButtonCount);
		}
		public Rectangle DropDownButtonBounds {
			get {
				Rectangle res = Rectangle.Empty;
				if(Toolbar.DropDownItemLink.RibbonItemInfo != null) res = Toolbar.DropDownItemLink.RibbonItemInfo.Bounds;
				if(res == Rectangle.Empty) {
					res = ContentBounds;
					res.X = res.Right - 10;
					res.Width = 10;
				}
				return res;
			}
		}
		public virtual void Invalidate(RibbonItemViewInfo itemInfo) {
			if(itemInfo == null) {
				ViewInfo.Invalidate(Bounds);
				return;
			}
			ViewInfo.Invalidate(itemInfo.Bounds);
		}
		protected virtual bool CalcItemHitInfo(RibbonHitInfo res, RibbonItemViewInfo itemInfo) {
			if(!ViewInfo.FormMaximized || ViewInfo.GetToolbarLocation() != RibbonQuickAccessToolbarLocation.Above || res.HitPoint.Y > itemInfo.Bounds.Y) return itemInfo.Bounds.Contains(res.HitPoint);
			return itemInfo.Bounds.X < res.HitPoint.X && itemInfo.Bounds.Right > res.HitPoint.X;
		}
		public virtual RibbonHitInfo CalcHitInfo(RibbonHitInfo res) {
			for(int itemIndex = 0; itemIndex < Items.Count; itemIndex++) {
				if(CalcItemHitInfo(res, Items[itemIndex])) {
					Items[itemIndex].CalcHitInfo(res);
					break;
				}
			}
			if(!Toolbar.Ribbon.ShowCategoryInCaption) return res;
			foreach(RibbonPageCategoryViewInfo vi in ViewInfo.Header.PageCategories) {
				if(res.ContainsSet(vi.Bounds, RibbonHitTest.PageHeaderCategory)) {
					res.PageCategory = vi.Category;
					res.PageCategoryInfo = vi;
					break;
				}
			}
			return res;
		}
		protected internal virtual bool IsToolbarCustomizationItem(int itemIndex) {
			if(itemIndex >= Items.Count || itemIndex < 0) return false;
			RibbonItemViewInfo itemInfo = Items[itemIndex]; 
			if(itemInfo.Item == Toolbar.CustomizeItemLink || itemInfo.Item == Toolbar.DropDownItemLink) return true;
			return false;
		}
		protected internal virtual void UpdateItemsLayout(Rectangle contentRect, int beginIndex, int count) {
			int itemIndex;
			Point Location = contentRect.Location;
			if(count == 1 && IsToolbarCustomizationItem(0) && !(Toolbar.Ribbon is RibbonPopupToolbarControl) && IsCustomizeItemOutside)
				Location.X += IndentBeforeDropDownButton;
			for(itemIndex = beginIndex; itemIndex < beginIndex + count; itemIndex++) {
				Size sz = Items[itemIndex].CalcBestSize();
				Items[itemIndex].Bounds = new Rectangle(new Point(Location.X, Location.Y + (contentRect.Height - sz.Height) / 2), sz);
				Location.X += Items[itemIndex].CalcBestSize().Width;
				if(itemIndex == beginIndex + count - 2 && IsToolbarCustomizationItem(itemIndex + 1)) 
					Location.X += IndentBeforeDropDownButton;
				if(ViewInfo.IsRightToLeft)
					Items[itemIndex].Bounds = BarUtilites.ConvertBoundsToRTL(Items[itemIndex].Bounds, ContentBackgroundBounds);
			}
		}
		public Rectangle DesignerRect { get { return designerRect; } }
		protected virtual void CalcDesignerRect() {
			this.designerRect = Rectangle.Empty;
			if(!ViewInfo.IsDesignMode) return;
			designerRect = new Rectangle(CalcDesignerRectPosition(), Bounds.Y, CalcDesignerRectWidth(), Bounds.Height);
		}
		protected virtual int CalcDesignerRectWidth() {
			if(VisibleButtonCount == 0) return 50;
			if(ViewInfo.IsRightToLeft)
				return Items[0].Bounds.Right - Items[VisibleButtonCount - 1].Bounds.Left;
			return Items[VisibleButtonCount - 1].Bounds.Right - Items[0].Bounds.Left;
		}
		protected virtual int CalcDesignerRectPosition() {
			if(Items.Count != 0) return ViewInfo.IsRightToLeft ? Items[Items.Count - 1].Bounds.X : Items[0].Bounds.X;
			return ViewInfo.IsRightToLeft ? ContentBounds.Right - CalcDesignerRectWidth() : ContentBounds.X;
		}
		public int VisibleButtonCount { get { return visibleButtonCount; } }
		protected virtual Rectangle CalcContentBackgroundBounds() { return Bounds; } 
		protected internal virtual Rectangle CalcContentBounds() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.TabletOffice)
				return Bounds;
			SkinElementInfo res = GetToolbarInfo();
			res.Bounds = Bounds;
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, res);
		}
		public int CalcMinHeight() { return CalcMinHeightCore(GetToolbarInfo()); }
		protected internal virtual int CalcMinHeightCore(SkinElementInfo info) {
			GInfo.AddGraphics(null);
			try {
				return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(0, 0, 0, Math.Max(ViewInfo.ButtonHeight, ViewInfo.EditorHeight))).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual Size CalcBestSize() {
			return CalcContentSize(0, Items.Count, Toolbar.ShowCustomizeItem);
		}
		protected internal int GetVisibleButtonCount(int width) {
			int itemIndex, resWidth = 0;
			for(itemIndex = 0; itemIndex < Items.Count - 1; itemIndex++) {
				resWidth += Items[itemIndex].CalcBestSize().Width;
				if(resWidth == width) break;
				if(resWidth > width) {
					break;
				}
			}
			if(itemIndex > 0 && Items[itemIndex] is RibbonQuickToolbarCustomizeItemViewInfo) itemIndex--;
			return Math.Max(0, itemIndex);
		}
		protected internal Size CalcContentSize(int beginIndex, int itemCount, bool allowDropDownButton) {
			Size size = Size.Empty, bestSize = Size.Empty;
			for(int itemIndex = beginIndex; itemIndex < beginIndex + itemCount; itemIndex++) {
				bestSize = Items[itemIndex].CalcBestSize();
				size.Width += bestSize.Width;
				size.Height = Math.Max(size.Height, bestSize.Height);
			}
			if(allowDropDownButton && ((itemCount > 1) || (itemCount == 1 && IsCustomizeItemOutside))) size.Width += IndentBeforeDropDownButton;
			return size;
		}
		protected internal void InsertDropdownButton(int pos) {
			Items.Insert(pos, ViewInfo.CreateItemViewInfo(Toolbar.DropDownItemLink));
		}
		protected virtual Image GetExtraGlyph() {
			SkinElement glyph = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinQuickToolbarButtonGlyph];
			if(glyph.Glyph != null) return glyph.Glyph.Image;
			return null;
		}
		protected internal virtual void CreateItemsViewInfo() {
			ClearItemsViewInfo();
			if(Toolbar.Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2010 && ViewInfo.Form != null)
				Items.Add(CreateSeparatorViewInfo());
			if(Toolbar.Ribbon.IsOfficeTablet && Toolbar.Ribbon.ShowApplicationButton != DefaultBoolean.False) {
				Items.Add(CreateItemViewInfo(Toolbar.ApplicationButtonItemLink));
			}
			List<BarItemLink> links = InplaceLinksHelper.GetLinks(Toolbar.Manager, Toolbar.ItemLinks, Toolbar.Ribbon.AllowInplaceLinks, Toolbar.Ribbon.IsDesignMode, (link) => ViewInfo.ShouldCreateItemInfo(link));
			for(int itemIndex = 0; itemIndex < links.Count; itemIndex++) {
				RibbonItemViewInfo itemInfo = CreateItemViewInfo(links[itemIndex]);
				if(itemInfo == null)
					continue;
				Items.Add(itemInfo);
			}
			if(Toolbar.Ribbon.OptionsTouch.ShowTouchUISelectorInQAT)
				AddPrivateItem(Toolbar.TouchMouseModeItemLink);
			if(Toolbar.ShowCustomizeItem)
				AddPrivateItem(Toolbar.CustomizeItemLink);
		}
		protected virtual RibbonItemViewInfo CreateItemViewInfo(BarItemLink link) {
			IRibbonItem item = link;
			if(!ViewInfo.ShouldCreateItemInfo(link)) return null;
			RibbonItemViewInfo itemInfo = ViewInfo.CreateItemViewInfo(item);
			if(itemInfo == null) return null;
			itemInfo.Owner = this;
			itemInfo.AllowedStyles = GetItemStyle(link);
			var checkInfo = itemInfo as RibbonCheckItemViewInfo;
			if(checkInfo == null || checkInfo.GetCheckBoxVisibility() == CheckBoxVisibility.None)
				itemInfo.ExtraGlyph = GetExtraGlyph();
			if(Items.Count > 0 && item.BeginGroup)
				Items.Add(CreateSeparatorViewInfo());
			return itemInfo;
		}
		protected virtual void AddPrivateItem(BarItemLink link) {
			RibbonItemViewInfo vi = ViewInfo.CreateItemViewInfo(link);
			vi.AllowedStyles = RibbonItemStyles.SmallWithoutText;
			vi.Owner = this;
			Items.Add(vi);
		}
		protected internal RibbonItemStyles GetItemStyle(BarItemLink link) {
			return RibbonItemInToolbarStyleCalculator.GetItemStyle(link, false);
		}
		protected internal virtual void ClearItemsViewInfo() {
			this.visibleButtonCount = 0;
			Items.Clear();
		}
		protected virtual RibbonQuickAccessToolbarLocation GetToolbarLocation() { return ViewInfo.GetToolbarLocation(); }
		protected internal virtual RibbonSeparatorItemViewInfo CreateSeparatorViewInfo() {
			return new RibbonSeparatorItemViewInfo(this);
		}
		protected internal virtual SkinElementInfo GetBackgroundInfo() {
			if(GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above && ViewInfo.Caption.Bounds.IsEmpty) {
				SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinQuickToolbarAboveBackground2], Bounds);
				if(!ViewInfo.Ribbon.Enabled)
					info.Attributes = PaintHelper.RibbonDisabledAttributes;
				return info;
			}
			return null;
		}
		[ThreadStatic]
		static ImageAttributes toolbarAttributes;
		static ImageAttributes ToolbarAttributes {
			get {
				if(toolbarAttributes == null) {
					toolbarAttributes = new ImageAttributes();
					ColorMatrix matrix = new ColorMatrix();
					matrix.Matrix33 = 0.4f;
					toolbarAttributes.SetColorMatrix(matrix);
				}
				return toolbarAttributes;
			}
		}
		protected internal virtual SkinElementInfo GetToolbarInfoInCaption() {
			bool isOffice2010StyleRibbon = Toolbar.Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007;
			string element = isOffice2010StyleRibbon ? RibbonSkins.SkinQuickToolbarInCaptionBackground2010 : RibbonSkins.SkinQuickToolbarInCaptionBackground;
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[element], ItemsContentBackgroundBounds);
			if(ViewInfo.Caption.IsGlassForm) res.Attributes = ToolbarAttributes;
			res.RightToLeft = ViewInfo.IsRightToLeft;
			return res;
		}
		protected internal virtual SkinElementInfo GetToolbarInfo() {
			SkinElementInfo info = null;
			if(GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above) {
				if(ViewInfo.Caption.Bounds.IsEmpty)
					info = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinQuickToolbarAboveBackground], ItemsContentBackgroundBounds);
				else 
					info = GetToolbarInfoInCaption();
			}
			else 
				info = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinQuickToolbarBelowBackground], Bounds);
			if(!ViewInfo.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
	}
}
