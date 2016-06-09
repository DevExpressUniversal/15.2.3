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
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Ribbon.Accessible;
namespace DevExpress.XtraBars.Ribbon.Internal {
	public interface IRibbonItemViewInfoProvider {
		RibbonItemViewInfo CreateViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item);
	}
	public class RibbonApplicationButtonItem : BarButtonItem {
		public RibbonApplicationButtonItem(RibbonControl ribbon) : base(ribbon.Manager, true) { }
	}
	public class RibbonQuickToolbarBarItem : BarButtonItem {
		RibbonControl ribbon;
		public RibbonQuickToolbarBarItem(RibbonControl ribbon, BarManager manager) : base(manager, true) {
			this.ribbon = ribbon;
			this.ActAsDropDown = true;
		}
		protected internal override RibbonControl Ribbon { get { return ribbon; } }
		public BaseRibbonViewInfo ViewInfo { get { return Ribbon.ViewInfo; } }
		protected internal override bool CanCloseSubOnClick(BarItemLink link) { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SuperToolTip SuperTip {
			get {
				if(base.SuperTip != null)
					return base.SuperTip;
				SuperToolTip sp = new SuperToolTip();
				ToolTipItem item = new ToolTipItem();
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.MoreCommands);
				sp.Items.Add(item);
				return sp;
			}
			set {
				base.SuperTip = value;
			}
		}
	}
	public class RibbonApplicationButtonItemLink : BarButtonItemLink {
		public RibbonApplicationButtonItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		protected override void OnLinkClick() {
			base.OnLinkClick();
			((RibbonHandler)Ribbon.Handler).ShowApplicationButtonPopup();
		}
		protected internal override Image GetGlyph() {
			if(Ribbon.ViewInfo != null)
				return Ribbon.ViewInfo.GetApplicationIcon();
			return base.GetGlyph();
		}
	}
	public class RibbonQuickToolbarBarItemLink : BarButtonItemLink, IRibbonItemViewInfoProvider {
		protected RibbonQuickToolbarBarItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		RibbonItemViewInfo IRibbonItemViewInfoProvider.CreateViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) {
			return new RibbonQuickToolbarDropItemViewInfo(viewInfo, item);
		}
	}
	public class RibbonToolbarPopupItem : BarButtonItem {
		public RibbonToolbarPopupItem() : base() { }
		public RibbonToolbarPopupItem(BarManager manager, bool privateItem) : base(manager, privateItem) {
			ButtonStyle = BarButtonStyle.Check;
		}
		RibbonPageGroup pageGroup = null;
		public RibbonPageGroup PageGroup { get { return pageGroup; } set { pageGroup = value; } }
		RibbonGalleryBarItemLink galleryLink = null;
		public override bool Enabled {
			get {
				if(GalleryLink != null) 
					return GalleryLink.Enabled;
				if (PageGroup != null)
					return PageGroup.Visible;
				return true;
			}
			set {
				base.Enabled = value;
			}
		}
		public override Image Glyph {
			get {
				if(GalleryLink != null) return GalleryLink.Glyph;
				if(PageGroup != null) return PageGroup.Glyph;
				return base.Glyph;
			}
			set {
				base.Glyph = value;
			}
		}
		public override Image GlyphDisabled {
			get {
				if(GalleryLink != null) return GalleryLink.Item.GlyphDisabled;
				return base.GlyphDisabled;
			}
			set {
				base.GlyphDisabled = value;
			}
		}
		public override int ImageIndex {
			get {
				if(GalleryLink != null) return GalleryLink.ImageIndex;
				if(PageGroup != null) return PageGroup.ImageIndex;
				return base.ImageIndex;
			}
			set {
				base.ImageIndex = value;
			}
		}
		public override int ImageIndexDisabled {
			get {
				if(GalleryLink != null) return GalleryLink.Item.ImageIndexDisabled;
				return base.ImageIndexDisabled;
			}
			set {
				base.ImageIndexDisabled = value;
			}
		}
		public override SuperToolTip SuperTip {
			get {
				if(PageGroup != null) return PageGroup.SuperTip;
				if(GalleryLink != null) return GalleryLink.Item.SuperTip;
				return base.SuperTip;
			}
			set {
				base.SuperTip = value;
			}
		}
		public override string Hint {
			get {
				if(GalleryLink != null) return GalleryLink.Item.Hint;
				return base.Hint;
			}
			set {
				base.Hint = value;
			}
		}
		public RibbonGalleryBarItemLink GalleryLink { get { return galleryLink; } set { galleryLink = value; } }
		protected internal override void OnClick(BarItemLink link) {
			base.OnClick(link);
			Down = true;
		}
	}
	public class RibbonToolbarPopupItemLink : BarButtonItemLink {
		public RibbonToolbarPopupItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		public RibbonPageGroup PageGroup { 
			get {
				if(Item == null) return null;
				return (Item as RibbonToolbarPopupItem).PageGroup; 
			} 
			set { (Item as RibbonToolbarPopupItem).PageGroup = value; } 
		}
		public RibbonGalleryBarItemLink GalleryLink { get { return (Item as RibbonToolbarPopupItem).GalleryLink; } set { (Item as RibbonToolbarPopupItem).GalleryLink = value; } }
		public override int ImageIndex {
			get {
				return Item.ImageIndex;
			}
			set {
				base.ImageIndex = value;
			}
		}
	}
	public class RibbonGroupItem : BarButtonItem {
		public RibbonGroupItem(RibbonPageGroup pageGroup) {
			this.fIsPrivateItem = true;
			PageGroup = pageGroup;
			this.ButtonStyle = BarButtonStyle.DropDown;
			this.ActAsDropDown = true;
			this.ImageIndex = PageGroup.ImageIndex;
			this.Glyph = PageGroup.Glyph;
			Manager = PageGroup.Manager;
		}
		public RibbonPageGroup PageGroup { get; private set; }
		protected internal override RibbonItemStyles GetRibbonAllowedStyles(RibbonItemViewInfo itemInfo) {
			if(Ribbon.IsOfficeTablet)
				return RibbonItemStyles.SmallWithoutText;
			return base.GetRibbonAllowedStyles(itemInfo);
		}
		public override string Caption {
			get { return PageGroup.Text; }
			set { base.Caption = value; }
		}
		public override Image Glyph {
			get { return PageGroup.Glyph; }
			set { base.Glyph = value; }
		}
		public override int ImageIndex {
			get { return PageGroup.ImageIndex; }
			set { base.ImageIndex = value; }
		}
	}
	public class RibbonPageGroupItemLink : BarButtonItemLink, IRibbonItemViewInfoProvider, ISupportRibbonKeyTip {
		protected RibbonPageGroupItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		RibbonItemViewInfo IRibbonItemViewInfoProvider.CreateViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) {
			return new RibbonGroupItemViewInfo(viewInfo, item);
		}
		protected override void OnLinkClick() {
			ShowPopup();
		}
		public override int ImageIndex {
			get { return Item.ImageIndex; }
			set { base.ImageIndex = value; }
		}
		public override bool CanPressDropDownButton {
			get {
				return true;
			}
		}
		protected override bool AllowClearSelectionOnClick { get { return false; } }
		public override void ShowPopup() {
			if(Ribbon.IsOfficeTablet) {
				ShowPopupMenu();
			}
			else {
				ShowPopupGroup();
			}
		}
		protected virtual void ShowPopupMenu() {
			RibbonGroupItem item = (RibbonGroupItem)Item;
			item.PageGroup.GroupInfo.ShowContentMenu(this);
		}
		protected virtual void ShowPopupGroup() {
			RibbonGroupItem item = (RibbonGroupItem)Item;
			item.PageGroup.GroupInfo.ShowContentDropDown();
		}
		Point ISupportRibbonKeyTip.ShowPoint {
			get {
				Point pt = Point.Empty;
				RibbonPageGroupViewInfo groupInfo = RibbonItemInfo.Owner as RibbonPageGroupViewInfo;
				if(RibbonItemInfo == null || groupInfo == null) return pt;
				pt.X = RibbonItemInfo.Bounds.X + RibbonItemInfo.Bounds.Width / 2;
				if(Ribbon.IsOfficeTablet)
					pt.Y = GetKeyTipYPos(2);
				else
					pt.Y = groupInfo.Bounds.Bottom;
				return Ribbon.PointToScreen(pt);
			}
		}
		ContentAlignment ISupportRibbonKeyTip.Alignment { get { return ContentAlignment.TopCenter; } }
		void ISupportRibbonKeyTip.Click() {
			Ribbon.KeyTipManager.HideKeyTips();
			OnLinkAction(BarLinkAction.KeyClick, Keys.Space);
			if(!Ribbon.IsOfficeTablet)
				Ribbon.PopupGroupForm.Control.KeyTipManager.ActivatePanelKeyTips();
		}
		string userKeyTip = string.Empty;
		string ISupportRibbonKeyTip.ItemUserKeyTip { 
			get {
				if(userKeyTip == string.Empty)
					if((OwnerItem as RibbonGroupItem) != null)
						userKeyTip = ((OwnerItem as RibbonGroupItem).PageGroup as ISupportRibbonKeyTip).ItemUserKeyTip;
				return userKeyTip;
			}		
			set{
				if(value != null) { userKeyTip = value.ToUpper(); }
				else
					userKeyTip = string.Empty;
			}
		}
	}
	public class RibbonGroupItemViewInfo : RibbonButtonItemViewInfo {
		public RibbonGroupItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		public bool ShouldDrawDropDownGlyph {
			get {
				return ViewInfo.GetRibbonStyle() == RibbonControlStyle.TabletOffice || 
					ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal;
			}
		}
		public bool ShouldExcludeGlyph { get { return ViewInfo.GetRibbonStyle() == RibbonControlStyle.TabletOffice; } }
		public bool ShouldDrawText { get { return ViewInfo.GetRibbonStyle() != RibbonControlStyle.OfficeUniversal; } }
		public bool ShouldDrawGlyph { get { return ViewInfo.GetRibbonStyle() != RibbonControlStyle.OfficeUniversal; } }
		protected internal override ObjectState CalcState() {
			ObjectState res = base.CalcState();
			RibbonViewInfo vi = ViewInfo as RibbonViewInfo;
			if(vi != null && vi.Ribbon.IsPopupGroupOpened) res &= (~ObjectState.Hot);
			return res;
		}
		protected override RibbonItemStyles[] CreateLevels() {
			return base.CreateLevels();
		}
		public override RibbonItemInfo GetInfo() {
			switch(ViewInfo.GetRibbonStyle()) {
				case RibbonControlStyle.OfficeUniversal:
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcButtonNoTextSize,
						ItemCalculator.Helper.CalcButtonViewInfo,
						ItemCalculator.Helper.DrawPageGroupButton,
						ItemCalculator.Helper.DrawPageGroupButtonText,
						ItemCalculator.Helper.GetButtonElementInfo);
				case RibbonControlStyle.TabletOffice:
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcButtonSize,
						ItemCalculator.Helper.CalcButtonViewInfo,
						ItemCalculator.Helper.DrawPageGroupButton,
						ItemCalculator.Helper.DrawPageGroupButtonText,
						ItemCalculator.Helper.GetButtonElementInfo);
				default:
					return new RibbonItemInfo(
					   ItemCalculator.Helper.CalcLargeDropDownButtonSize,
					   ItemCalculator.Helper.CalcLargeButtonViewInfo,
					   ItemCalculator.Helper.DrawLargeGroupDropDownButton,
					   ItemCalculator.Helper.DrawLargeGroupDropDownButtonText,
					   ItemCalculator.Helper.GetLargeButtonElementInfo);
			}
		}
		public Rectangle GetDropDownGlyphBounds() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal)
				return UniversalDropDownGlyphBounds;
			return TabletDropDownGlyphBounds;
		}
		public Image GetDropDownGlyph() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal)
				return UniversalDropDownGlyph;
			return TabletDropDownGlyph;
		}
		public Rectangle UniversalDropDownGlyphBounds {
			get { return RectangleHelper.GetCenterBounds(Bounds, UniversalDropDownGlyph.Size); }
		}
		public Image UniversalDropDownGlyph {
			get { return RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinButtonArrow].Image.GetImages().Images[0]; } 
		}
		Image tabletDropDownGlyph;
		public Image TabletDropDownGlyph { 
			get {
				if(tabletDropDownGlyph == null) {
					System.IO.Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraBars.Ribbon.Images.TabletDropDownGlyph.png");
					tabletDropDownGlyph = Image.FromStream(str);
				}
				return tabletDropDownGlyph;
			} 
		}
		public Rectangle TabletDropDownGlyphBounds { 
			get {
				Point offset = GlyphBounds.IsEmpty ? new Point(3, 3) : new Point(- TabletDropDownGlyph.Width / 2 + 2, - TabletDropDownGlyph.Height / 2 + 1);
				Rectangle rect = GlyphBounds.IsEmpty ? Bounds : GlyphBounds;
				return new Rectangle(rect.Right - TabletDropDownGlyph.Width - offset.X, rect.Bottom - TabletDropDownGlyph.Height - offset.Y, TabletDropDownGlyph.Width, TabletDropDownGlyph.Height);
			} 
		}
	}
	public class RibbonQuickToolbarDropItemViewInfo : RibbonDropDownItemViewInfo {
		public RibbonQuickToolbarDropItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		public override Size ImageSize { 
			get {
				Size res = base.ImageSize;
				if(ExtraGlyph != null) return ExtraGlyph.Size;
				return res; 
			}
		}
		public override Image ExtraGlyph {
			get {
				SkinElement glyph = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinQuickToolbarGlyph];
				if(glyph.Glyph != null && glyph.Glyph.GetImages() != null) return glyph.Glyph.GetImages().Images[1];
				return base.ExtraGlyph;
			}
			set {
				base.ExtraGlyph = value;
			}
		}
		protected internal override bool IsDroppedDown {
			get {
				RibbonControl ribbon = ViewInfo.OwnerControl as RibbonControl;
				return (ribbon != null && ribbon.IsPopupToolbarOpened);
			}
		}
		protected override int CalcImageIndex(ObjectState state) {
			if(IsDroppedDown) return 3;
			return base.CalcImageIndex(state);
		}
	}
	static class BarCheckItemsList {
		public static List<BarCheckItem> CreateCheckItemsList(RibbonControl ribbon) {
			if(ribbon == null) return null;
			RibbonQuickAccessToolbar toolbar = ribbon is RibbonPopupToolbarControl ? ((RibbonPopupToolbarControl)ribbon).SourceRibbon.Toolbar : ribbon.Toolbar;
			if(toolbar == null)
				return null;
			List<BarCheckItem> list = new List<BarCheckItem>();
			foreach(BarItemLink link in toolbar.ItemLinks) {
				bool isMergedVisible = ((BarItemLinkCollection)link.Links).CanVisibleMerged(link);
				if(!isMergedVisible || link.Item.Visibility == BarItemVisibility.Never || link.Item.Visibility == BarItemVisibility.OnlyInCustomizing) continue;
				BarCheckItem item = new BarCheckItem(ribbon.Manager, true);
				item.Tag = link;
				item.Caption = GetItemName(link);
				item.Checked = link.Visible;
				item.AllowHtmlText = link.AllowHtmlText;
				item.MergeType = link.Item.MergeType;
				item.ItemClick += delegate(object sender, ItemClickEventArgs e) {
					BarItemLink lnk = (BarItemLink)e.Item.Tag;
					lnk.Visible = ((BarCheckItem)e.Item).Checked;
				};
				list.Add(item);
			}
			return list;
		}
		static string GetItemName(BarItemLink link) {
			if(!string.IsNullOrEmpty(link.Caption)) return link.Caption;
			if(!string.IsNullOrEmpty(link.Item.Hint)) return link.Item.Hint;
			if(link.Item.Tag is RibbonPageGroup) return (link.Item.Tag as RibbonPageGroup).Text;
			if(link.Item.SuperTip != null) return link.Item.SuperTip.ToString();
			return BarLocalizer.Active.GetLocalizedString(BarString.RibbonCaptionTextNone);
		}
	}
	public class RibbonCustomizationPopupMenu : PopupMenu {
		BarButtonItem changeToolbarLocationItem;
		BarButtonItem addToQuickAccessToolbar, removeFromQuickAccessToolbar;
		BarButtonItem customizeRibbonItem;
		BarCheckItem minimizeRibbon;
		public RibbonCustomizationPopupMenu(RibbonControl ribbon) {
			this.AllowRibbonQATMenu = false;
			this.minimizeRibbon = null;
			this.addToQuickAccessToolbar = null;
			this.changeToolbarLocationItem = null;
			this.removeFromQuickAccessToolbar = null;
			this.customizeRibbonItem = null;
			this.Ribbon = ribbon;
			this.MenuDrawMode = MenuDrawMode.SmallImagesText;
			InitializeMenu();
		}
		protected virtual void DeInitializeMenu() {
			ItemLinks.Clear();
			if(this.changeToolbarLocationItem != null) this.changeToolbarLocationItem.Dispose();
			if(this.addToQuickAccessToolbar != null) this.addToQuickAccessToolbar.Dispose();
			if(this.removeFromQuickAccessToolbar != null) this.removeFromQuickAccessToolbar.Dispose();
			if(this.minimizeRibbon != null) this.minimizeRibbon.Dispose();
			if(this.customizeRibbonItem != null) this.customizeRibbonItem.Dispose();
		}
		public virtual void InitializeMenu() {
			DeInitializeMenu();
			this.minimizeRibbon = new BarCheckItem();
			this.minimizeRibbon.IsPrivateItem = true;
			this.minimizeRibbon.Manager = Ribbon.Manager;
			this.minimizeRibbon.CheckedChanged += new ItemClickEventHandler(OnMinimizeRibbon_CheckedChanged);
			this.minimizeRibbon.Enabled = GetMinimizedRibbonItemAccessibilityState();
			this.changeToolbarLocationItem = new BarButtonItem(Ribbon.Manager, true);
			this.changeToolbarLocationItem.ItemClick += new ItemClickEventHandler(OnChangeToolbarLocationItem_ItemClick);
			this.customizeRibbonItem = new BarButtonItem(Ribbon.Manager, true);
			this.customizeRibbonItem.ItemClick += new ItemClickEventHandler(OnCustomizeRibbon_ItemClick);
			this.addToQuickAccessToolbar = new BarButtonItem(Ribbon.Manager, true);
			this.addToQuickAccessToolbar.CloseSubMenuOnClick = false;
			this.addToQuickAccessToolbar.ItemClick += new ItemClickEventHandler(OnAddToQuickAccessToolbar_ItemClick);
			this.removeFromQuickAccessToolbar = new BarButtonItem(Ribbon.Manager, true);
			this.removeFromQuickAccessToolbar.CloseSubMenuOnClick = false;
			this.removeFromQuickAccessToolbar.ItemClick += new ItemClickEventHandler(OnRemoveFromQuickAccessToolbar_ItemClick);
			ItemLinks.Add(addToQuickAccessToolbar);
			ItemLinks.Add(removeFromQuickAccessToolbar);
			ItemLinks.Add(changeToolbarLocationItem, true);
			if(Ribbon.ShowCustomizationOption) ItemLinks.Add(customizeRibbonItem, true);
			ItemLinks.Add(minimizeRibbon, false);
		}
		protected virtual bool GetMinimizedRibbonItemAccessibilityState() {
			RibbonMinimizedControl minimizedRibbon = Ribbon as RibbonMinimizedControl;
			if(minimizedRibbon != null && minimizedRibbon.IsFullScreenModeActive)
				return false;
			if(Ribbon != null && Ribbon.ViewInfo.IsPopupFullScreenModeActive)
				return false;
			return true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ItemLinks.Clear();
				this.changeToolbarLocationItem.Dispose();
				this.addToQuickAccessToolbar.Dispose();
				this.removeFromQuickAccessToolbar.Dispose();
				this.minimizeRibbon.Dispose();
				this.customizeRibbonItem.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnRibbonDisposeCore() {
		}
		void OnMinimizeRibbon_CheckedChanged(object sender, ItemClickEventArgs e) {
			Ribbon.Minimized = minimizeRibbon.Checked;
		}
		void OnChangeToolbarLocationItem_ItemClick(object sender, ItemClickEventArgs e) {
			Ribbon.OnChangeQuickToolbarPosition();
		}
		void OnAddToQuickAccessToolbar_ItemClick(object sender, ItemClickEventArgs e) {
			Ribbon.OnAddToToolbar(e.Item.Tag as BarItemLink);
			CloseAll();
		}
		void OnCustomizeRibbon_ItemClick(object sender, ItemClickEventArgs e) {
			Ribbon.OnCustomizeRibbon();
		}
		void OnRemoveFromQuickAccessToolbar_ItemClick(object sender, ItemClickEventArgs e) {
			RibbonToolbarPopupItemLink plink = e.Item.Tag as RibbonToolbarPopupItemLink;
			if(plink != null && plink.PageGroup != null) Ribbon.OnRemovePageGroupLinkFromToolbar(plink);
			else Ribbon.OnRemoveFromToolbar(e.Item.Tag as BarItemLink);
			CloseAll();
		}
		void CloseAll() {
			if(Ribbon != null && Ribbon.Manager != null)
				Ribbon.Manager.SelectionInfo.OnCloseAll(BarMenuCloseType.All);
		}
		protected virtual bool ShouldEnableRemoveFromQuickAccessToolbar(BarItemLink link) {
			RibbonToolbarPopupItemLink popupLink = link as RibbonToolbarPopupItemLink;
			RibbonPageGroup pageGroup = popupLink != null? popupLink.PageGroup: null;
			if(pageGroup != null) {
				for(int i = 0; i < Ribbon.SourceToolbar.ItemLinks.Count; i++) {
					RibbonToolbarPopupItemLink plink = Ribbon.SourceToolbar.ItemLinks[i] as RibbonToolbarPopupItemLink;
					if(plink != null && plink.PageGroup != null && plink.PageGroup.GetOriginalPageGroup() == pageGroup.GetOriginalPageGroup()) return true;
				}
			}
			return link != null && Ribbon.SourceToolbar.Contains(link.Item);
		}
		protected virtual bool ShouldEnableAddToQuickAccessToolbar(BarItemLink link) {
			return !ShouldEnableRemoveFromQuickAccessToolbar(link);
		}
		protected internal void UpdateMenu(bool allowChangeLocation, BarItemLink link) {
			bool currentBelow = Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Below;
			this.changeToolbarLocationItem.Visibility = allowChangeLocation ? BarItemVisibility.Always : BarItemVisibility.Never;
			this.changeToolbarLocationItem.Caption = BarLocalizer.Active.GetLocalizedString(currentBelow ? BarString.RibbonToolbarAbove : BarString.RibbonToolbarBelow);
			this.customizeRibbonItem.Visibility = BarItemVisibility.Always;
			this.customizeRibbonItem.Caption = BarLocalizer.Active.GetLocalizedString(BarString.CustomizeRibbon);
			this.addToQuickAccessToolbar.Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonToolbarAdd);
			this.removeFromQuickAccessToolbar.Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonToolbarRemove);
			this.minimizeRibbon.Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonToolbarMinimizeRibbon);
			this.minimizeRibbon.Checked = Ribbon.Minimized;
			this.minimizeRibbon.Visibility = Ribbon.AllowMinimizeRibbon ? BarItemVisibility.Always : BarItemVisibility.Never;
			this.minimizeRibbon.Enabled = GetMinimizedRibbonItemAccessibilityState();
			this.addToQuickAccessToolbar.Visibility = BarItemVisibility.Never;
			this.removeFromQuickAccessToolbar.Visibility = BarItemVisibility.Never;
			if(!ShouldShowAddRemoveMenuItems(link)) return;
			if(link != null && ShouldEnableRemoveFromQuickAccessToolbar(link)) {
				this.removeFromQuickAccessToolbar.Visibility = BarItemVisibility.Always;
				this.removeFromQuickAccessToolbar.Tag = link;
			}
			else if(link != null && ShouldEnableAddToQuickAccessToolbar(link)) {
				this.addToQuickAccessToolbar.Visibility = BarItemVisibility.Always;
				this.addToQuickAccessToolbar.Tag = link;
			}
			foreach(BarItemLink l in ItemLinks)
				l.Item.AllowRightClickInMenu = false;
		}
		protected virtual bool ShouldShowAddRemoveMenuItems(BarItemLink link) {
			if(Ribbon.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal ||
				Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Hidden || 
				Ribbon.Toolbar.IsToolbarCustomizationItem(link)) return false;
			return true;
		}
		internal bool InShow { get; set; }
		public virtual void Show(int x, int y, bool allowChangeLocation, BarItemLink link) {
			InShow = true;
			try {
				UpdateMenu(allowChangeLocation, link);
				ShowPopup(new Point(x, y));
			}
			finally {
				InShow = false;
			}
		}
		protected override void OnCloseUp(CustomPopupBarControl prevControl) {
			if(!InShow)
				ClearMenu();
			base.OnCloseUp(prevControl);
		}
		protected virtual void ClearMenu() {
			foreach(BarItemLink link in ItemLinks) {
				if(link.Item != null)
					link.Item.Tag = null;
			}
		}
		protected internal override LocationInfo CalcLocationInfo(Point p) {
			LocationInfo info = base.CalcLocationInfo(p);
			info = new LocationInfo(p, p, info.VerticalOpen, info.OpenXBack);
			return info;
		}
	}
	public class RibbonQuickToolbarCustomizeItem : BarListItem {
		RibbonControl ribbon;
		public RibbonQuickToolbarCustomizeItem(RibbonControl ribbon, BarManager manager)
			: base() {
				ShowNavigationHeader = DefaultBoolean.False;
			this.fIsPrivateItem = true;
			this.ribbon = ribbon;
			Strings.Add("-");
			Strings.Add("-");
			Strings.Add("-");
			this.Manager = manager;
			CreateItems();
		}
		protected override BarInListItem CreateInListItem() {
			BarInListItem res = base.CreateInListItem();
			res.CloseSubMenuOnClick = false;
			return res;
		}
		protected override void OnListItemClick(ListItemClickEventArgs e) {
			if(e.Index == 1) Ribbon.OnChangeQuickToolbarPosition();
			if(e.Index == 2) Ribbon.OnCustomizeRibbon();
			if(SelectionInfo != null) SelectionInfo.OnCloseAll(BarMenuCloseType.All);
		}
		BarSelectionInfo SelectionInfo { get { return Manager == null ? null : Manager.SelectionInfo; } }
		protected internal override void OnGetItemData() {
			if(IsOpened()) return;
			bool currentBelow = Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Below;
			if(Ribbon.AllowChangeToolbarLocationMenuItem)Strings[1] = BarLocalizer.Active.GetLocalizedString(currentBelow ? BarString.RibbonToolbarAbove : BarString.RibbonToolbarBelow);
			if(Ribbon.AllowCustomization) Strings[2] = BarLocalizer.Active.GetLocalizedString(BarString.CustomizeRibbon);
			base.OnGetItemData();
			CreateItems();
			foreach(BarItemLink link in ItemLinks)
				link.Item.AllowRightClickInMenu = false;
		}
		protected override void CreateItems() {
			base.CreateItems();
			int count = 0;
			List<BarCheckItem> list = BarCheckItemsList.CreateCheckItemsList(ribbon);
			if(list != null) 
				foreach(BarCheckItem item in list) 
					ItemLinks.Insert(count++, item);
			if(Ribbon.OptionsTouch.ShowTouchUISelectorVisibilityItemInQATMenu)
				ItemLinks.Insert(count, CreateTouchMouseModeItem());
		}
		private BarCheckItem CreateTouchMouseModeItem() {
			BarCheckItem item = new BarCheckItem(ribbon.Manager, true);
			item.Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeCommandText);
			item.Checked = Ribbon.OptionsTouch.ShowTouchUISelectorInQAT;
			item.CheckedChanged += delegate(object sender, ItemClickEventArgs e) {
				Ribbon.OptionsTouch.ShowTouchUISelectorInQAT = ((BarCheckItem)e.Item).Checked;
			};
			return item;
		}
		protected internal override RibbonControl Ribbon { get { return ribbon; } }
		public BaseRibbonViewInfo ViewInfo { get { return Ribbon.ViewInfo; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SuperToolTip SuperTip {
			get {
				if(base.SuperTip != null)
					return base.SuperTip;
				SuperToolTip sp = new SuperToolTip();
				ToolTipItem item = new ToolTipItem();
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.CustomizeToolbarSuperTipText);
				sp.Items.Add(item);
				return sp;
			}
			set {
				base.SuperTip = value;
			}
		}
	}
	public class RibbonQuickToolbarCustomizeItemLink : BarListItemLink, IRibbonItemViewInfoProvider {
		protected RibbonQuickToolbarCustomizeItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			AllowOpenEmpty = true;
		}
		protected internal override bool AllowRibbonQATMenu {
			get { return false; }
		}
		RibbonItemViewInfo IRibbonItemViewInfoProvider.CreateViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) {
			return new RibbonQuickToolbarCustomizeItemViewInfo(viewInfo, item);
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new AccessibleQuickToolbarCustomizeItemLink(this);
		}
		protected override void OpenCore(bool setOpened) {
			if(!setOpened) {
				base.OpenCore(setOpened);
				return;
			}
			RibbonCustomizationMenuEventArgs args = new RibbonCustomizationMenuEventArgs(this, Ribbon);
			Ribbon.RaiseShowCustomizationMenu(args);
			if(!args.ShowCustomizationMenu.HasValue || args.ShowCustomizationMenu.Value)
				base.OpenCore(setOpened);
			else
				base.OpenCore(false);
		}
		protected override CustomPopupBarControl CreateSubBarControl() {
			return new RibbonToolbarSubMenuBarControl(Manager, this);
		}
	}
	public class RibbonQuickToolbarCustomizeItemViewInfo : RibbonDropDownItemViewInfo {
		public RibbonQuickToolbarCustomizeItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		public override Size ImageSize {
			get {
				Size res = base.ImageSize;
				if(ExtraGlyph != null) return ExtraGlyph.Size;
				return res;
			}
		}
		public override Image ExtraGlyph {
			get {
				SkinElement glyph = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinQuickToolbarGlyph];
				if(glyph.Glyph != null && glyph.Glyph.GetImages() != null) return glyph.Glyph.GetImages().Images[0];
				return base.ExtraGlyph;
			}
			set {
				base.ExtraGlyph = value;
			}
		}
		public override RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcButtonNoTextSize,
				ItemCalculator.Helper.CalcButtonNoTextViewInfo,
				ItemCalculator.Helper.DrawButton,
				ItemCalculator.Helper.DrawButtonText,
				ItemCalculator.Helper.GetButtonElementInfo);
		}
	}
	public class RibbonTouchMouseModeItem : BarButtonItem {
		RibbonControl ribbon;
		public RibbonTouchMouseModeItem(RibbonControl ribbon, BarManager manager)
			: base(manager, true) {
			this.ribbon = ribbon;
			ButtonStyle = BarButtonStyle.DropDown;
			ActAsDropDown = true;
			AllowDrawArrow = true;
		}
		protected internal override RibbonControl Ribbon { get { return ribbon; } }
		public BaseRibbonViewInfo ViewInfo { get { return Ribbon.ViewInfo; } }
		protected internal override bool CanCloseSubOnClick(BarItemLink link) { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SuperToolTip SuperTip {
			get {
				if(base.SuperTip != null)
					return base.SuperTip;
				SuperToolTip sp = new SuperToolTip();
				ToolTipItem item = new ToolTipItem();
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeCommandText);
				sp.Items.Add(item);
				return sp;
			}
			set {
				base.SuperTip = value;
			}
		}
		Image glyph;
		public override Image Glyph {
			get {
				if(glyph == null) {
					if(Ribbon.OptionsTouch.TouchMouseModeSelectorGlyph == null) {
						System.IO.Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraBars.Ribbon.Images.TouchMouseMode_16x16.png");
						glyph = Image.FromStream(str);
					}
					else glyph = Ribbon.OptionsTouch.TouchMouseModeSelectorGlyph;
				}
				return glyph;
			}
			set { }
		}
		GalleryDropDown galleryDropDown;
		public override PopupControl DropDownControl {
			get {
				if(galleryDropDown == null)
					galleryDropDown = CreateGallery();
				return galleryDropDown;
			}
			set {
			}
		}
		protected internal virtual void UpdateGallery() {
			GalleryDropDown gallery = (GalleryDropDown)DropDownControl;
			int itemIndex = Ribbon.OptionsTouch.GetTouchUI()? 1: 0;
			gallery.Gallery.Groups[0].Items[itemIndex].Checked = true;
		}
		protected internal virtual GalleryDropDown CreateGallery() {
			GalleryDropDown gallery = new GalleryDropDown();
			gallery.ShowNavigationHeader = DefaultBoolean.False;
			gallery.Ribbon = Ribbon;
			gallery.Gallery.ColumnCount = 1;
			gallery.Gallery.ShowScrollBar = Gallery.ShowScrollBar.Hide;
			gallery.Gallery.AutoSize = GallerySizeMode.Both;
			gallery.Gallery.RowCount = 2;
			gallery.Gallery.ShowItemText = true;
			gallery.Gallery.FixedImageSize = false;
			gallery.Gallery.ItemCheckMode = Gallery.ItemCheckMode.SingleRadio;
			gallery.Gallery.ItemCheckedChanged += OnGalleryItemCheckedChanged;
			gallery.Gallery.ItemImageLocation = Locations.Left;
			gallery.Gallery.Groups.Add(new GalleryItemGroup() { Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeGalleryGroupText) });
			gallery.Gallery.Groups[0].Items.Add(new GalleryItem() { 
				Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeMouseItemText), 
				Description = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeMouseItemDescription),
				Tag = false,
				Image = Ribbon.OptionsTouch.MouseModeGlyph != null ? Ribbon.OptionsTouch.MouseModeGlyph : Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraBars.Ribbon.Images.MouseMode_32x32.png"))
			});
			gallery.Gallery.Groups[0].Items.Add(new GalleryItem() { 
				Caption = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeTouchItemText),
				Description = BarLocalizer.Active.GetLocalizedString(BarString.RibbonTouchMouseModeTouchItemDescription),
				Tag = true,
				Image = Ribbon.OptionsTouch.TouchModeGlyph != null ? Ribbon.OptionsTouch.TouchModeGlyph : Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraBars.Ribbon.Images.TouchMode_32x32.png"))
			});
			return gallery;
		}
		void OnGalleryItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			Ribbon.OptionsTouch.TouchUI = ((bool)e.Item.Tag)? DefaultBoolean.True: DefaultBoolean.False;
		}
	}
	public class RibbonTouchMouseModeItemLink : BarButtonItemLink {
		protected RibbonTouchMouseModeItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { 
		}
		public override void ShowPopup() {
			((RibbonTouchMouseModeItem)Item).UpdateGallery();
			base.ShowPopup();
		}
	}
	public static class RibbonReduceOperationHelper {
		public static RibbonControl Ribbon { get; set; }
		public static ReduceOperation SelectedOperation { get; set; }
	}
}
