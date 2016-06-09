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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraBars {
	public class RibbonGalleryBarItemLink : BarButtonItemLink {
		InRibbonGalleryRibbonItemViewInfo viewInfo;
		RibbonGalleryBarItemLink originalLink = null;
		protected RibbonGalleryBarItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			this.viewInfo = null;
			this.galleryItem = Item;
		}
		public override void Dispose() {
			HidePopup();
			DestroyScrollTimer();
			if(this.dropDown != null)
				this.dropDown.Dispose();
			this.dropDown = null;
			base.Dispose();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemLinkActAsButtonGroup")]
#endif
		public override bool ActAsButtonGroup {
			get { return false; }
			set { }
		}
		internal RibbonGalleryBarItemLink GetOriginalLink() {
			if(OriginalLink == null) return this;
			return OriginalLink;
		}
		internal RibbonGalleryBarItemLink OriginalLink { 
			get {
				if(originalLink != null && originalLink.OriginalLink != null) return originalLink.OriginalLink;
				return originalLink; 
			} 
			set { originalLink = value; } 
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemLinkItem")]
#endif
		public new RibbonGalleryBarItem Item { get { return base.Item as RibbonGalleryBarItem; } }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			if(this.IsLinkInMenu || Ribbon == null) return base.CreateAccessibleInstance();
			return new RibbonInplaceGalleryAccessible(Ribbon.AccessibleRibbon, this);
		}
		protected internal virtual InRibbonGalleryRibbonItemViewInfo ViewInfo { 
			get { return viewInfo; } 
			set {
				if(ViewInfo == value)
					return;
				viewInfo = value;
			} 
		}
		protected internal InRibbonGalleryViewInfo GalleryInfo { get { return ViewInfo != null ? ViewInfo.GalleryInfo : null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollYPosition {
			get {
				if(ScrollYPositionCore != -1)
					return ScrollYPositionCore;
				int delta = Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice ? GalleryInfo.ItemMaxSize.Width : GalleryInfo.ItemMaxSize.Height;
				delta -= Gallery.DistanceBetweenItems;
				if(Item.Gallery.ShowGroupCaption && GalleryInfo.Groups.Count != 0) delta = GalleryInfo.Groups[0].CaptionBounds.Height;
				int maxYPos = GalleryInfo != null ? GalleryInfo.MaxScrollYPosition : 0;
				return Math.Min(maxYPos, delta * ScrollRowIndex);	
			}
			set {
			}
		}
		int ScrollRowIndexFromPosition(int position) {
			int delta = Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice ? GalleryInfo.ItemMaxSize.Width : GalleryInfo.ItemMaxSize.Height;
			delta -= Gallery.DistanceBetweenItems;
			if(Item.Gallery.ShowGroupCaption && GalleryInfo.Groups.Count != 0) delta = GalleryInfo.Groups[0].CaptionBounds.Height;
			return position / delta;
		}
		int scrollYPositionCore = -1;
		internal int ScrollYPositionCore {
			get { return scrollYPositionCore; }
			set {
				if(ScrollYPositionCore == value)
					return;
				scrollYPositionCore = Math.Max(0, Math.Min(GalleryInfo.MaxScrollYPosition, value));
				ScrollRowIndex = ScrollRowIndexFromPosition(ScrollYPositionCore);
			}
		}
		void ResetScrollYPositionCore() { this.scrollYPositionCore = -1; }
		int scrollRowIndex = 0;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollRowIndex { 
			get { return scrollRowIndex; } 
			set {
				if(ScrollRowIndex == value)
					return;
				scrollRowIndex = Math.Min(GetRowsCount(), Math.Max(0, value));
				OnScrollRowIndexChanged();
				OnLinkChanged();
			}
		}
		internal int GetRowsCount() {
			int res = 0;
			foreach(GalleryItemGroupViewInfo vi in GalleryInfo.Groups) {
				res += vi.LinesCount;
				if(GalleryInfo.Gallery.ShowGroupCaption)
					res += 1;
			}
			return res;
		}
		protected virtual void OnScrollRowIndexChanged() {
			GalleryInfo.IsReady = false;
		}
		protected internal InRibbonGallery Gallery { get { return Item.Gallery; } }
		RibbonGalleryBarItem galleryItem;
		bool suppressUnpress = false;
		protected internal virtual void OnUnPressImageGallery(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			GalleryItem downItem = Gallery.DownItem;
			Gallery.DownItem = null;
			if(hitInfo.InGallery && Gallery.AllowMarqueeSelection && Gallery.IsMarqueeSelection(downPoint, hitInfo.HitPoint)) {
				Gallery.OnSelectionComplete(hitInfo.GalleryInfo);
				hitInfo.GalleryInfo.Selection = Rectangle.Empty;
			}
			if(hitInfo.InGalleryItem) {
				GalleryItem pressedItem = hitInfo.GalleryItemInfo.Item;
				if(e.Button == MouseButtons.Left) {
					Ribbon.Invalidate(hitInfo.GalleryItemInfo.Bounds);
					if(downItem == pressedItem) {
						Gallery.HideImageForms(null, true);
						if(Manager != null) {
							if(Ribbon == null || Ribbon.ViewInfo.CanCloseAllPopups)
								Manager.SelectionInfo.OnCloseAll(BarMenuCloseType.AllExceptMiniToolbars);
						}
						if(!this.suppressUnpress)
						galleryItem.Gallery.OnItemClick(this, galleryItem.Gallery, pressedItem);
					}
				}
			}
			DestroyScrollTimer();
			this.suppressUnpress = false;
		}
		protected internal virtual void OnPressScrollButtonCore(RibbonHitInfo hitInfo) {
			if(!hitInfo.GalleryInfo.CanScroll) return;
			int lineDelta = hitInfo.HitTest == RibbonHitTest.GalleryUpButton || hitInfo.HitTest == RibbonHitTest.GalleryLeftButton? -1 : 1;
			if((lineDelta < 0 && ScrollYPosition <= GalleryInfo.MinScrollYPosition) || (lineDelta > 0 && ScrollYPosition >= GalleryInfo.MaxScrollYPosition))
				return;
			ResetScrollYPositionCore();
			ScrollRowIndex += lineDelta;
		}
		protected virtual void OnPressScrollButton(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			if(scrollTimer != null && scrollTimer.Enabled) return;
			OnPressScrollButtonCore(hitInfo);
			InitScrollTimer(hitInfo.HitTest);
		}
		Point downPoint;
		protected internal virtual void OnPressImageGallery(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			this.suppressUnpress = false;
			if(!Enabled) return;
			if(e.Button != MouseButtons.Left || IsPopupVisible) return;
			if(hitInfo.HitTest == RibbonHitTest.GalleryUpButton || hitInfo.HitTest == RibbonHitTest.GalleryDownButton ||
				hitInfo.HitTest == RibbonHitTest.GalleryLeftButton || hitInfo.HitTest == RibbonHitTest.GalleryRightButton) {
				OnPressScrollButton(e, hitInfo);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.GalleryDropDownButton) {
				OnGalleryDropDown(hitInfo);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.GalleryItem || 
				hitInfo.HitTest == RibbonHitTest.GalleryImage) {
					hitInfo.GalleryInfo.Gallery.DownItem = hitInfo.GalleryItemInfo.Item;
					if(e.Clicks > 1) {
						hitInfo.Gallery.OnItemDoubleClick(this, hitInfo.Gallery, hitInfo.GalleryItem);
						this.suppressUnpress = true;
					}
					if(hitInfo.GalleryBarItemInfo.ViewInfo.OwnerControl is RibbonOneGroupControl) hitInfo.GalleryBarItemInfo.ViewInfo.OwnerControl.Refresh();	
					hitInfo.GalleryBarItemInfo.ViewInfo.OwnerControl.Invalidate(hitInfo.GalleryItemInfo.Bounds);
			}
			if(hitInfo.InGallery) {
				downPoint = hitInfo.HitPoint;
			}
		}
		protected override Point PopupLocation {
			get {
				if(RibbonItemInfo == null || RibbonItemInfo.OwnerControl == null) {
					if(BarControl == null) return Point.Empty;
					return BarControl.PointToScreen(new Point(Bounds.Right, Bounds.Top));
				}
				return ViewInfo != null? ScreenBounds.Location: new Point(ScreenBounds.X, ScreenBounds.Bottom);
			}
		}
		protected internal override ToolTipControlInfo GetToolTipInfo(RibbonHitInfo hi, Point point) {
			if(hi != null && hi.InGalleryItem) {
				if(hi.GalleryItem.SuperTip == null && hi.GalleryItem.Hint == "") return null;
				ToolTipControlInfo info = hi.GalleryItem.GetToolTipInfo(hi, point);
				info.ToolTipPosition = new Point(-10000, -10000);
				info.Object = hi.GalleryItemInfo;
				if(Gallery.AllowHoverImages) {
					return hi.GalleryItem.GetHoverTooltip(info, hi);
				}
				return info;
			}
			return base.GetToolTipInfo(hi, point);
		}
		Timer scrollTimer;
		RibbonHitTest scrollHitTest = RibbonHitTest.None;
		protected void InitScrollTimer(RibbonHitTest hitTest) {
			int interval = 50;
			if(this.scrollTimer == null || this.scrollHitTest != hitTest) {
				interval = 300;
				DestroyScrollTimer();
				this.scrollTimer = new Timer();
				this.scrollTimer.Tick += new EventHandler(OnScrollTimerTick);
			}
			this.scrollHitTest = hitTest;
			this.scrollTimer.Interval = interval;
			this.scrollTimer.Enabled = true;
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if(RibbonItemInfo == null || Ribbon == null || Control.MouseButtons != MouseButtons.Left) {
				DestroyScrollTimer();
				return;
			}
			if(RibbonItemInfo.ViewInfo.HotObject.HitTest == scrollHitTest && RibbonItemInfo.ViewInfo.HotObject.Gallery == Gallery) {
				OnPressImageGallery(new DXMouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), RibbonItemInfo.ViewInfo.HotObject);
				return;
			}
			else {
				DestroyScrollTimer();
			}
		}
		protected void DestroyScrollTimer() {
			if(scrollTimer != null) scrollTimer.Dispose();
			this.scrollTimer = null;
			this.scrollHitTest = RibbonHitTest.None;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemLinkCanPressDropDownButton")]
#endif
		public override bool CanPressDropDownButton {
			get { return Item.DropDownEnabled && Enabled; }
		}
		InRibbonGallery gallery;
		internal GalleryDropDown dropDown;
		BarItemLink toolbarLink = null;
		internal BarItemLink ToolbarLink { get { return toolbarLink; } }
		protected override PopupControl DropDownControl { 
			get {
				if(Gallery.GalleryDropDown != null)
					return Gallery.GalleryDropDown;
				if(dropDown == null) {
					dropDown = new GalleryDropDown();
					dropDown.MenuCaption = Caption;
				}
				return dropDown; 
			} 
		}
		protected internal GalleryDropDown DropDownCore { get { return DropDownControl as GalleryDropDown; } }
		internal virtual void OnGalleryDropDown(BarItemLink link) {
			if(IsPopupVisible || (RibbonItemInfo == null && !(link is RibbonToolbarPopupItemLink))) return;
			this.toolbarLink = link;
			ShowPopup();
		}
		protected virtual void OnGalleryDropDown(RibbonHitInfo hitInfo) {
			if(IsPopupVisible || RibbonItemInfo == null || (Manager != null && Manager.IsDesignMode)) return;
			this.toolbarLink = null;
			Ribbon.BeginInvoke(new MethodInvoker(ShowPopup));
		}
		protected internal override BarLinkViewInfo LinkViewInfo {
			get {
				if(ToolbarLink != null)
					return ToolbarLink.LinkViewInfo;
				return base.LinkViewInfo;
			}
		}
		protected override void PrepareShowPopup() {
			if(!CanPressDropDownButton || IsPopupVisible) return;
			this.gallery = Gallery;
			DropDownCore.Manager = Ribbon != null? Ribbon.Manager : Manager;
			DropDownCore.Ribbon = Ribbon;
			DropDownCore.OwnerGalleryLink = GetOriginalLink();
			if(Gallery.GalleryDropDown == null) {
				DropDownCore.OwnerGallery = Gallery;
				DropDownCore.Gallery.Assign(Item.Gallery);
				DropDownCore.Gallery.Tag = Gallery.Tag;
				DropDownCore.Gallery.ItemClick += new GalleryItemClickEventHandler(OnGalleryPopupItemClick);
				DropDownCore.Gallery.ItemDoubleClick += new GalleryItemClickEventHandler(OnGalleryPopupItemClick);
				DropDownCore.Gallery.ItemCheckedChanged += new GalleryItemEventHandler(OnGalleryPopupItemCheckedChanged);
				DropDownCore.Gallery.GalleryItemHover += new GalleryItemEventHandler(OnGalleryPopupItemHover);
				DropDownCore.Gallery.GalleryItemLeave += new GalleryItemEventHandler(OnGalleryPopupItemLeave);
				DropDownCore.Gallery.CustomDrawItemImage += new GalleryItemCustomDrawEventHandler(OnGalleryPopupCustomDrawItemImage);
				DropDownCore.Gallery.CustomDrawItemText += new GalleryItemCustomDrawEventHandler(OnGalleryPopupCustomDrawItemText);
			}
			DropDownCore.BeginUpdate();
			try {
				DropDownCore.Gallery.FilterMenuPopup += new GalleryFilterMenuEventHandler(OnGalleryPopupFilterMenuPopup);
				DropDownCore.Gallery.FilterMenuItemClick += new GalleryFilterMenuItemClickEventHandler(OnGalleryPopupFilterMenuItemClick);
				Item.Gallery.RaiseInitDropDownGallery(this, DropDownCore.Gallery);
				if(GalleryInfo != null)
					DropDownCore.MinimumWidth = GalleryInfo.Bounds.Width;
			}
			finally {
				DropDownCore.EndUpdate();
			}
		}
		void OnGalleryPopupCustomDrawItemText(object sender, GalleryItemCustomDrawEventArgs e) {
			e.Handled = Item.Gallery.RaiseCustomDrawItemText(e.Cache, (GalleryItemViewInfo)e.ItemInfo, e.Bounds);
		}
		void OnGalleryPopupCustomDrawItemImage(object sender, GalleryItemCustomDrawEventArgs e) {
			e.Handled = Item.Gallery.RaiseCustomDrawItemImage(e.Cache, (GalleryItemViewInfo)e.ItemInfo, e.Bounds);
		}
		void OnGalleryPopupItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			if(e.Item.OriginItem != null && ((InDropDownGallery)e.Gallery).SynchWithInRibbonGallery) {
				e.Item.OriginItem.SetCheckedInternal(e.Item.Checked);
				galleryItem.Gallery.OnCheckedChanged(e.Item.OriginItem, true);
				galleryItem.Gallery.MakeVisible(e.Item.OriginItem);
			}
		}
		protected override void OnPopupCloseUpCore() {
			if(DropDownCore == null) return;
			try {
				OnGalleryCloseUp();
			}
			finally {
				this.dropDown = null;
				this.gallery = null;
			}
		}
		protected virtual void OnGalleryCloseUp() {
			GalleryDropDown galleryDropDown = DropDownCore;
			galleryDropDown.Gallery.ItemClick -= new GalleryItemClickEventHandler(OnGalleryPopupItemClick);
			galleryDropDown.Gallery.ItemDoubleClick -= new GalleryItemClickEventHandler(OnGalleryPopupItemDoubleClick);
			DropDownCore.Gallery.ItemCheckedChanged -= new GalleryItemEventHandler(OnGalleryPopupItemCheckedChanged);
			galleryDropDown.Gallery.FilterMenuPopup -= new GalleryFilterMenuEventHandler(OnGalleryPopupFilterMenuPopup);
			galleryDropDown.Gallery.FilterMenuItemClick -= new GalleryFilterMenuItemClickEventHandler(OnGalleryPopupFilterMenuItemClick);
			galleryDropDown.Gallery.GalleryItemHover -= new GalleryItemEventHandler(OnGalleryPopupItemHover);
			galleryDropDown.Gallery.GalleryItemLeave -= new GalleryItemEventHandler(OnGalleryPopupItemLeave);
			galleryDropDown.Gallery.CustomDrawItemText -= new GalleryItemCustomDrawEventHandler(OnGalleryPopupCustomDrawItemText);
			galleryDropDown.Gallery.CustomDrawItemImage -= new GalleryItemCustomDrawEventHandler(OnGalleryPopupCustomDrawItemImage);
			this.gallery.RaisePopupCloseEvent(galleryDropDown.Gallery);
			if(this.gallery.GalleryDropDown == null) {
				foreach(GalleryItemGroup group in this.gallery.Groups) {
					if(galleryDropDown.Gallery.Groups.IndexOf(group) != -1) {
						group.ReplaceCollection(this.gallery.Groups);
					}
					group.ReplaceCollection(this.gallery.Groups);
				}
				galleryDropDown.Dispose();
			}
			if(Holder is PopupMenu)
				return;
			OnLinkChanged();
		}
		void OnGalleryPopupItemClick(object sender, GalleryItemClickEventArgs e) {
			galleryItem.Gallery.OnItemClick(e.InRibbonGalleryLink, e.Gallery, e.Item);
		}
		void OnGalleryPopupItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			galleryItem.Gallery.OnItemDoubleClick(e.InRibbonGalleryLink, e.Gallery, e.Item);
		}
		void OnGalleryPopupItemHover(object sender, GalleryItemEventArgs e) {
			galleryItem.Gallery.RaiseGalleryItemHover(e.Item);
		}
		void OnGalleryPopupItemLeave(object sender, GalleryItemEventArgs e) {
			galleryItem.Gallery.RaiseGalleryItemLeave(e.Item);
		}
		void OnGalleryPopupFilterMenuItemClick(object sender, GalleryFilterMenuClickEventArgs e) {
			galleryItem.Gallery.RaiseFilterMenuItemClick(e.FilterMenu, e.Item);
		}
		void OnGalleryPopupFilterMenuPopup(object sender, GalleryFilterMenuEventArgs e) {
			galleryItem.Gallery.RaiseFilterMenuPopup(e.FilterMenu);
		}
		InRibbonGalleryRibbonItemViewInfo cachedInfo = null;
		bool CanGetCachedInfo() {
			return cachedInfo != null && 
				(Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice && (cachedInfo.GalleryInfo is MacStyleInRibbonGalleryViewInfo) ||
				 Ribbon.GetRibbonStyle() != RibbonControlStyle.MacOffice && !(cachedInfo.GalleryInfo is MacStyleInRibbonGalleryViewInfo));
		}
		protected override RibbonItemViewInfo GetCachedRibbonItemViewInfo() {
			if(!CanGetCachedInfo())
				return null;
			if(cachedInfo != null) {
				if(cachedInfo.ViewInfo != null && cachedInfo.ViewInfo.OwnerControl != null && cachedInfo.ViewInfo.OwnerControl.IsDisposed)
					return null;
				cachedInfo.GalleryInfo.PrepareCached();
			}
			return cachedInfo;
		}
		protected internal override void OnLinkChanged() {
			base.OnLinkChanged();
			if(GalleryInfo != null) {
				GalleryInfo.Reset();
			}
		}
		protected override void OnRibbonViewInfoCreated(RibbonItemViewInfo itemInfo) {
			this.cachedInfo = itemInfo as InRibbonGalleryRibbonItemViewInfo;
		}
		GalleryItem lastMakeVisibleItem = null;
		protected internal GalleryItem LastMakeVisibleItem { get { return lastMakeVisibleItem; } }
		public void MakeVisible(GalleryItem item) {
			if(RibbonItemInfo != null && GalleryInfo != null) {
				GalleryInfo.MakeVisible(item);
				lastMakeVisibleItem = null;
			}
			else
				lastMakeVisibleItem = item;
		}
		protected ContainerKeyTipManager GalleryDropDownKeyTipManager { 
			get {
				if(Gallery.GalleryDropDown != null) return Gallery.GalleryDropDown.Gallery.BarControl.KeyTipManager; 
				GalleryDropDown gdd = DropDownControl as GalleryDropDown;
				if(gdd != null) { return gdd.Gallery.BarControl.KeyTipManager; }
				return null;
			} 
		}
		protected internal override void KeyTipItemClick() {
			ShowPopup();
			if(!IsPopupVisible)
				return;
			GalleryDropDownKeyTipManager.Ribbon = Ribbon;
			IHasRibbonKeyTipManager parentKeyTip = Holder as IHasRibbonKeyTipManager;
			if(GalleryDropDownKeyTipManager == null) return;
			if(parentKeyTip != null)
				GalleryDropDownKeyTipManager.Parent = parentKeyTip.KeyTipManager;
			GalleryDropDownKeyTipManager.ActivateKeyTips();
		}
		bool commandButtonVisible;
		protected internal bool CommandButtonVisible {
			get { return commandButtonVisible; }
		}
		protected internal virtual void UpdateMacStyleCommandButtonVisibility(Point pt) {
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.MacOffice || Ribbon.Minimized)
				return;
			if(ViewInfo.Bounds.Contains(Ribbon.PointToClient(pt)) || IsPopupVisible) {
				if(!CommandButtonVisible) {
					this.commandButtonVisible = true;
					ShowCommandButton();
				}
			}
			else if(CommandButtonVisible) {
				this.commandButtonVisible = false;
				HideCommandButton();
			}
		}
		protected virtual void HideCommandButton() {
			XtraAnimator.Current.AddFadeAnimation(Ribbon, this, 500, GalleryInfo.ButtonCommandBounds, false);
		}
		protected virtual void ShowCommandButton() {
			XtraAnimator.Current.AddFadeAnimation(Ribbon, this, 500, GalleryInfo.ButtonCommandBounds, true);
		}
	}
	public class SkinRibbonGalleryBarItemLink : RibbonGalleryBarItemLink {
		public SkinRibbonGalleryBarItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
	} 
	public class BarBaseButtonItemLink : BarItemLink {
		protected BarBaseButtonItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarBaseButtonItemLinkItem")]
#endif
		public new BarBaseButtonItem Item { get { return (BarBaseButtonItem)base.Item; } }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.BaseButtonLinkAccessible(this); 
		}
		protected override bool IsRequiredRecalcWhenCloned { get { return Item != null && Item.IsCheckButtonStyle; } }
	}
	public class BarToggleSwitchItemLink : BarItemLink {
		protected BarToggleSwitchItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarToggleSwitchItemLinkItem")]
#endif
		public new BarToggleSwitchItem Item { get { return (BarToggleSwitchItem)base.Item; } }
		protected override void OnLinkClick() {
			base.OnLinkClick();
			Item.Checked = !Item.Checked;
		}
	}
	public class BarCheckItemLink : BarBaseButtonItemLink {
		protected override bool IsRequiredRecalcWhenCloned { get { return true; } }
		protected BarCheckItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		protected override bool RibbonIsChecked { get { return Item != null && Item.Down; } }
	}
	public class BarButtonItemLink : BarBaseButtonItemLink {
		string dropDownKeyTip = string.Empty;
		protected BarButtonItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.ButtonLinkAccessible(this); 
		}
		public override bool Enabled {
			get { 
				if(Item != null && Item.LastClickedLink != null && !Item.LastClickedLink.Enabled)
					return false;
				return base.Enabled; 
			}
		}
		protected internal override ToolTipControlInfo GetToolTipInfo(Point point) {
			if(Opened)
				return new ToolTipControlInfo();
			return base.GetToolTipInfo(point);
		}
		protected internal override void KeyTipItemClick() {
			BarButtonItem item = Item as BarButtonItem;
			if(item != null && item.IsDropDownButtonStyle) {
				KeyTipItemClickCore();
				return;
			}
			base.KeyTipItemClick();
		}
		public override void AssignKeyTip(BarItemLink link) {
			base.AssignKeyTip(link);
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			DropDownKeyTip = buttonLink.DropDownKeyTip;
		}
		public override void Dispose() {
			base.Dispose();
			popup = null;
		}
		protected override bool HasDropDownButtonCore {
			get {
				if(RibbonItemInfo != null && !RibbonItemInfo.Item.IsLargeButton)
					return false;
				return Item.IsDropDownButtonStyle && !Item.ActAsDropDown;
			}
		}
		protected virtual PopupControl DropDownControl { get { return Item == null ? null : Item.DropDownControl; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarButtonItemLinkDropDownKeyTip"),
#endif
 XtraSerializableProperty()]
		public string DropDownKeyTip {
			get { return dropDownKeyTip; }
			set {
				if(value != null) dropDownKeyTip = value.ToUpper();
				else dropDownKeyTip = string.Empty;
				UserDefine |= BarLinkUserDefines.DropDownKeyTip;
				OnLinkChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarButtonItemLinkIsPopupVisible")]
#endif
		public virtual bool IsPopupVisible {
			get { return DropDownControl != null && DropDownControl.Visible; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarButtonItemLinkOpened")]
#endif
		public virtual bool Opened {
			get {
				if(DropDownControl != null && DropDownControl.Visible) {
					if(DropDownControl.Activator == this) return true;
				}
				return false;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarButtonItemLinkCanPressDropDownButton")]
#endif
		public virtual bool CanPressDropDownButton { get { return Item.CanPressDropDownButton; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarButtonItemLinkItem")]
#endif
		public new BarButtonItem Item { get { return (BarButtonItem)base.Item; } }
		public virtual void HidePopup() {
			HidePopupCore(false);
		}
		protected virtual void HidePopupCore(bool intern) {
			if(DropDownControl != null && Manager != null) {
				try {
					Manager.SelectionInfo.internalFocusLock ++;
					if(intern && DropDownControl.IPopup != null)
						Manager.SelectionInfo.ClosePopup(DropDownControl.IPopup);
					else
						DropDownControl.HidePopup();
				}
				finally {
					Manager.SelectionInfo.internalFocusLock --;
				}
			}
		}
		protected override bool ActDoubleClickAsSingle { get { return !Item.ActAsDropDown; } }
		protected internal override bool IsNeededKey(KeyEventArgs e) {
			if(Enabled && CanPressDropDownButton && !Opened) {
				if(IsLinkInMenu && e.KeyData == Keys.Right) return true;
				if(!IsLinkInMenu && e.KeyCode == Keys.Down) return true;
			}
			return base.IsNeededKey(e);
		}
		protected internal override void ProcessKey(KeyEventArgs e) {
			if(!Enabled) return;
			if(CanPressDropDownButton && !Opened) {
				if(IsLinkInMenu && e.KeyData == Keys.Right) {
					IHasRibbonKeyTipManager hasKeyTip = Holder as IHasRibbonKeyTipManager;
					if (hasKeyTip != null && hasKeyTip.KeyTipManager != null && hasKeyTip.KeyTipManager.Show == true)
					{
						hasKeyTip.KeyTipManager.HideKeyTips();
						hasKeyTip.KeyTipManager.ResetFilterString();
						this.KeyTipDropDownClick();
					}
					else
					ShowPopup();
					return;
				}
				if(!IsLinkInMenu && (e.KeyCode == Keys.Down || e.KeyData == Keys.F4)) {
					ShowPopup();
					return;
				}
			}
			base.ProcessKey(e);
		}
		protected override bool ShouldCloseAllPopups {
			get {
				if(!Opened && base.ShouldCloseAllPopups) return true;
				return false;
			}
		}
		protected override void OnLinkAction(BarLinkAction action, object actionArgs) {
			switch(action) {
				case BarLinkAction.Press : 
					MouseEventArgs e = actionArgs as MouseEventArgs;
					if(DropDownControl != null) {
						if(IsPopupVisible) {
							HidePopupCore(true);
							Manager.SelectionInfo.HighlightedLink = this;
						}
						else {
							if(Item.ActAsDropDown ) {
								if(BarControl != null) BarControl.Capture = false;
								ShowPopup();
							}
						}
					}
					break;
				case BarLinkAction.StartDrag:
					HidePopup();
					break;
				case BarLinkAction.Highlight:
					if(CanPressDropDownButton && !Opened) {
						if(Bar != null && Manager.SelectionInfo.AutoOpenMenuBar == Bar && 
							!(BarControl is QuickCustomizationBarControl)) ShowPopup();
					} else {
						goto case BarLinkAction.KeyboardHighlight;
					}
					break;
				case BarLinkAction.KeyboardHighlight:
					if(!Opened && Bar != null) {
					}
					break;
				case BarLinkAction.PressArrow:
					if(CanPressDropDownButton) {
						if(BarControl != null) BarControl.Capture = false;
						if(IsPopupVisible) HidePopupCore(true);
						else ShowPopup();
					}
					break;
				case BarLinkAction.KeyClick:
					if(Item.Enabled && Item.ActAsDropDown && DropDownControl != null && (BarControl != null || Ribbon != null)) {
						Manager.SelectionInfo.KeyboardHighlightedLink = this;
						if(BarControl != null) {
							Manager.SelectionInfo.ActiveBarControl = BarControl;
							BarControl.Capture = false;
						}
							ShowPopup();
							return;
						}
					base.OnLinkAction(action, actionArgs);
					if(Manager == null) return;
					if(AllowClearSelectionOnClick) Manager.SelectionInfo.OnItemClickClear(this);
					return;
			}
			base.OnLinkAction(action, actionArgs);
			if(action == BarLinkAction.MouseClick && Manager != null && Item.ButtonStyle == BarButtonStyle.Default)
				Manager.SelectionInfo.CloseDXToolbars();
		}
		BarButtonItemLink GetHolderItemLink() {
			var popupMenu = Holder as PopupMenu;
			if(popupMenu == null) return null;
			return popupMenu.Activator as BarButtonItemLink;
		}
		protected internal override bool ShouldRememberLastCommand {
			get {
				var itemLink = GetHolderItemLink();
				if(itemLink == null) return false;
				return itemLink.Item.RememberLastCommand;
			}
		}
		protected virtual bool AllowClearSelectionOnClick { get { return true; } }
		PopupControl popup = null;
		protected override void OnLinkClick() {
			base.OnLinkClick();
		}
		public virtual void ShowPopup() {
			if(!CanPressDropDownButton) return;
			if(IsPopupVisible) {
				if(DropDownControl.Activator != this) {
					DropDownControl.HidePopup();
				}
				else 
					return;
			}
			PrepareShowPopup();
			if(DropDownControl == null) return;
			BarLinkViewInfo vi = LinkViewInfo;
			if(vi == null) return;
			this.popup = DropDownControl;
			DropDownControl.Activator = this;
			DropDownControl.CloseUp += new EventHandler(OnPopupCloseUp);
			DropDownControl.ShowPopup(Manager, PopupLocation);
			if(DropDownControl.Visible) {
				if(Bar != null && Bar.AutoPopupMode == BarAutoPopupMode.All)
					Manager.SelectionInfo.AutoOpenMenuBar = Bar;
			}
			else {
				OnPopupCloseUp(this, EventArgs.Empty);
			}
			Invalidate();
		}
		protected virtual void PrepareShowPopup() { }
		protected virtual Point PopupLocation {
			get {
				BarLinkViewInfo vi = LinkViewInfo;
				if(vi == null) return Point.Empty;
				Point p = vi.Bounds.Location;
				p.Y += vi.Bounds.Height;
				return LinkPointToScreen(p);
			}
		}
		protected virtual void OnPopupCloseUp(object sender, EventArgs e) {
			if(this.popup == null) return;
			this.popup.CloseUp -= new EventHandler(OnPopupCloseUp);
			OnPopupCloseUpCore();
			this.popup = null;
		}
		protected virtual void OnPopupCloseUpCore() {
			if(Item == null) return;
			Invalidate();
		}
		protected override bool RibbonIsDroppedDown { get { return Opened; } }
		protected override bool RibbonIsChecked { get { return Item != null && Item.Down; } }
		protected internal override bool ContainsSubItemLink(BarItemLink link, int level) {
			if(!CanPressDropDownButton || !IsLinkInMenu) return base.ContainsSubItemLink(link, level);
			PopupMenu menu = DropDownControl as PopupMenu;
			if(menu == null) return false;
			BarItemLinkReadOnlyCollection links = menu.ItemLinks;
			if(links.Contains(link)) return true;
			for(int n = 0; n < links.Count; n++) {
				if(links[n] == link) return true; 
				if(links[n].ContainsSubItemLink(link, level + 1)) return true;
			}
			return false;
		}
		protected override bool CanStartTimerCore {
			get {
				if(!CanPressDropDownButton || !IsLinkInMenu) return false;
				if(DropDownControl is PopupMenu) return true;
				return false;
			}
		}
		protected override void OnTimerClose() {
			HidePopup();
		}
		protected override void OnTimerOpen() {
			ShowPopup();
		}
		protected override bool TimerIsOpened { get { return Opened; } }
		protected override IPopup TimerPopup { get { return DropDownControl == null ? null : DropDownControl.IPopup; } }
		protected virtual bool CanPressDropDownByKeyTip() { 
			if(IsLinkInMenu || Holder is PopupMenu) return false;
			if (!((IRibbonItem)this).IsLargeButton) return true;
			return Item.ActAsDropDown;
		}
		protected internal override void KeyTipItemClickCore() {
			if (CanPressDropDownByKeyTip() && Item.CanPressDropDownButton && !IsLinkInMenu && !(Holder is PopupMenu))
				KeyTipDropDownClick();
			else 
				base.KeyTipItemClickCore();
		}
		protected internal virtual void KeyTipDropDownClick() {
			OnLinkActionCore(BarLinkAction.PressArrow, null);
			if(Item.DropDownControl == null || Item.DropDownControl.IPopup == null) return;
			CustomLinksControl linksControl = Item.DropDownControl.IPopup.CustomControl as CustomLinksControl;
			if(linksControl == null) return;
			linksControl.KeyTipManager.Ribbon = Ribbon;
			IHasRibbonKeyTipManager parentKeyTipHolder = Holder as IHasRibbonKeyTipManager;
			if(parentKeyTipHolder != null) {
				linksControl.KeyTipManager.Parent = parentKeyTipHolder.KeyTipManager;
				parentKeyTipHolder.KeyTipManager.HideKeyTips();
			}
			linksControl.KeyTipManager.ActivateKeyTips();
		}
		protected override Rectangle KeyTipItemBounds {
			get {
				RibbonButtonItemViewInfo buttonInfo = RibbonItemInfo as RibbonButtonItemViewInfo;
				if(buttonInfo == null || (buttonInfo.CurrentLevel != RibbonItemStyles.SmallWithoutText && buttonInfo.CurrentLevel != RibbonItemStyles.SmallWithText))
					return base.KeyTipItemBounds;
				if(!GlyphBounds.IsEmpty) return GlyphBounds;
				return base.KeyTipItemBounds;
			}
		}
	}
	public class BarLargeButtonItemLink : BarButtonItemLink {
		protected BarLargeButtonItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarLargeButtonItemLinkItem")]
#endif
		public new BarLargeButtonItem Item { get { return base.Item as BarLargeButtonItem; } }
		protected override bool RibbonIsLargeButton { get { return true; } }
	}
	#region BarHeaderItemLink
	public class BarHeaderItemLink : BarStaticItemLink{
		internal BarHeaderItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : this(ALinks, AItem, ALinkedObject, true) {
			this.BeginGroup = false;
		}
		protected BarHeaderItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject, bool flag) : base(ALinks, AItem, ALinkedObject) {
		}
		[Browsable(false)]
		public new BarHeaderItem Item { get { return base.Item as BarHeaderItem; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool BeginGroup {
			get {
				return false;
			}
			set {
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsButtonGroup {
			get {
				return base.ActAsButtonGroup;
			}
			set {
				base.ActAsButtonGroup = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string KeyTip {
			get {
				return base.KeyTip;
			}
			set {
				base.KeyTip = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemLinkAlignment Alignment {
			get {
				return base.Alignment;
			}
		}
	}
	#endregion
	#region BarStaticItemLink
	public class BarStaticItemLink : BarItemLink, ISpringLink {
		int width, springWidth, springTempWidth;
		internal BarStaticItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : this(ALinks, AItem, ALinkedObject, true) {
		}
		protected BarStaticItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject, bool flag) : base(ALinks, AItem, ALinkedObject) {
			this.springWidth = this.springTempWidth = this.width = 0;
		}
		[Browsable(false)]
		public new BarStaticItem Item { get { return base.Item as BarStaticItem; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemLinkCaption"),
#endif
 XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override string Caption {
			get { return Item.Caption; }
			set {
				if(Caption == value) return;
				Item.Caption = value;
				LayoutChanged();
			}
		}
		[Browsable(false),  XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override int Width {
			get { 
				if(Item.AutoSize != BarStaticItemSize.Spring) return Item.Width;
				return width;
			}
			set {
			}
		}
		bool ISpringLink.SpringAllow { 
			get {
				if(Item.AutoSize != BarStaticItemSize.Spring) return false;
				return true;
			} 
		}
		int ISpringLink.SpringWidth { get { return springWidth; } set {	this.springWidth = value; } }
		int ISpringLink.SpringMinWidth { get { return 4; } }
		int ISpringLink.SpringTempWidth { get { return springTempWidth; } set { springTempWidth = value; } }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.StaticLinkAccessible(this); 
		}
		protected override Rectangle KeyTipItemBounds {
			get {
				if(!GlyphBounds.IsEmpty) return GlyphBounds;
				return base.KeyTipItemBounds;
			}
		}
		protected override bool KeyTipVisibleCore {
			get { return false; }
		}
	}
	#endregion
	internal class EditorToolTipControlInfo : ToolTipControlInfo {
		public EditorToolTipControlInfo(BarEditItemLink link, ToolTipControlInfo info) : base() {
			ToolTipInfo = info;
			EditLink = link;
		}
		public BarEditItemLink EditLink { get; set; }
		public ToolTipControlInfo ToolTipInfo { get; private set; }
	}	
	#region BarEditItemLink
	public class BarEditItemLink : BarItemLink, ISpringLink {
		int springWidth, springTempWidth;
		protected BarEditItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			this.springTempWidth = this.springWidth = 0;
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.EditLinkAccessible(this); 
		}
		protected internal virtual bool IsEditWidthSet { 
			get {
				int res = 0;
				if(IsUserDefine(BarLinkUserDefines.EditWidth) && !Manager.IsDesignMode) res = UserEditWidth;
				else res = Item.EditWidth;
				return res > 0;
			} 
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemLinkEditWidth"),
#endif
 XtraSerializableProperty()]
		public virtual int EditWidth {
			get {
				int res = 0;
				if(IsUserDefine(BarLinkUserDefines.EditWidth) && !Manager.IsDesignMode) res = UserEditWidth;
				else res = Item.EditWidth;
				return res > 0? res : Item.DefaultEditWidth;
			}
			set {
				if(Manager.IsDesignMode)
					Item.EditWidth = value;
				else
					UserEditWidth = value;
			}
		}
		public override int Width {
			get { return EditWidth; }
			set { EditWidth = value; }
		}
		protected internal override void OnUpdateLinkProperty(BarLinkProperty property) {
			if(property == BarLinkProperty.Visibility && Manager != null && Manager.ActiveEditItemLink == this) {
				CloseEditor();
			}
			base.OnUpdateLinkProperty(property);
		}
		protected internal override ToolTipControlInfo GetToolTipInfo(RibbonHitInfo hi, Point point) {
			if(RibbonItemInfo != null) {
				ToolTipControlInfo res = ((RibbonEditItemViewInfo)RibbonItemInfo).EditViewInfo.GetToolTipInfo(point);
				if(res != null)
					return new EditorToolTipControlInfo(this, res); 
			}
			else if (LinkViewInfo != null) {
				ToolTipControlInfo res = ((BarEditLinkViewInfo)LinkViewInfo).EditViewInfo.GetToolTipInfo(point);
				if(res != null)
					return res;
			}
			return base.GetToolTipInfo(hi, point);
		}
		[Browsable(false)]
		public new BarEditItem Item { get { return base.Item as BarEditItem; } }
		protected internal override bool NeedMouseCapture { get { return base.NeedMouseCapture && !EditorActive; } }
		protected internal bool IsAutoFillWidthInMenu {
			get {
				if(Item.AutoFillWidthInMenu != DefaultBoolean.Default)
					return Item.AutoFillWidthInMenu == DefaultBoolean.True;
				BarSubItem item = Holder as BarSubItem;
				if(item != null)
					return item.AutoFillEditorWidth;
				PopupMenu menu = Holder as PopupMenu;
				if(menu != null)
					return menu.AutoFillEditorWidth;
				return true;
			}
		}
		bool ISpringLink.SpringAllow { 
			get { 
				if(!Item.AutoFillWidth) return false;
				if(BarControl != null && BarControl.RotateWhenVertical) return false;
				return true;
			}
		}
		int ISpringLink.SpringWidth { get { return springWidth; } set {	this.springWidth = value; } }
		int ISpringLink.SpringMinWidth { get { return MinWidth; } }
		int ISpringLink.SpringTempWidth { get { return springTempWidth; } set { springTempWidth = value; } }
		[Browsable(false)]
		public override int MinWidth { get { return 20; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarEditItemLinkCaptionAlignment")]
#endif
		public HorzAlignment CaptionAlignment {
			get { return Item == null ? HorzAlignment.Default : Item.CaptionAlignment; }
			set {
				if(Item == null) return;
				Item.CaptionAlignment = value;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarEditItemLinkEditValue")]
#endif
		public object EditValue {
			get { return Item == null ? null : Item.EditValue; }
			set {
				if(Item == null) return;
				Item.EditValue = value;
			}
		}
		[Browsable(false)]
		public override bool CanResize {
			get { 
				if(((ISpringLink)this).SpringAllow) return false;
				if(BarControl != null && BarControl.RotateWhenVertical && BarControl.IsVertical) return false;
				return true;
			}
		}
		protected virtual bool IsRequireUpdateOnMouseMove {
			get {
				return (Edit != null);
			}
		}
		protected override void OnLinkAction(BarLinkAction action, object actionArgs) {
			switch(action) {
				case BarLinkAction.MouseClick:
					if(Item.EditorShowMode == EditorShowMode.MouseUp || Item.EditorShowMode == EditorShowMode.Click) 
						ShowLinkEditor(false);
					break;
				case BarLinkAction.Press:
					if(Item.EditorShowMode == EditorShowMode.MouseUp || Item.EditorShowMode == EditorShowMode.Click) break;
					goto case BarLinkAction.KeyClick;
				case BarLinkAction.KeyClick:
					ShowLinkEditor(action == BarLinkAction.Press);
					break;
				case BarLinkAction.UnHighlight:
					UpdateEditViewInfo(BarManager.zeroPoint);
					break;
				case BarLinkAction.KeyboardHighlight:
				case BarLinkAction.Highlight:
				case BarLinkAction.MouseMove : 
					if(LinkViewInfo == null && RibbonItemInfo == null) break;
					Point p = ScreenToLinkPoint(Control.MousePosition);
					if(Manager.ActiveEditor != null) break;
					bool needInvalidate = IsNeedInvalidate(p);
					UpdateEditViewInfo(p);
					if(needInvalidate)
						Invalidate();
					break;
			}
			base.OnLinkAction(action, actionArgs);
		}
		protected internal override bool IsNeedMouseCapture { get { return base.IsNeedMouseCapture && !EditorActive; } }
		protected internal override bool IsInterceptKey(KeyEventArgs e) { 
			if(!EditorActive) return true;
			if(e.KeyCode == Keys.ProcessKey) return false;
			if(ActiveEditor.IsNeededKey(e) && ActiveEditor.EditorContainsFocus) return false;
			return true; 
		}
		protected virtual bool IsNeedInvalidate(Point p) {
			BaseEditViewInfo editInfo = GetEditViewInfo();
			if(editInfo == null || Edit == null) return true;
			if(editInfo.UpdateObjectState(MouseButtons.None, p)) {
				return true;
			}
			return false;
		}
		public override void Focus() {
			ShowEditor();
		}
		public virtual void ShowEditor() {
			ShowLinkEditor(false);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarEditItemLinkCanOpenEdit")]
#endif
		public virtual bool CanOpenEdit {
			get { return Edit != null && Enabled && Item.CanOpenEdit; }
		}
		protected internal void ShowLinkEditor(bool byMouse) {
			if(Edit != null && Item.CanOpenEdit && Item.Enabled) {
				if(EditorActive) return;
				if(Manager.SelectionInfo.EditingLink == this) return;
				Manager.SelectionInfo.EditingLink = null;
				if(this.IsVertical && (Bar != null && Bar.OptionsBar.RotateWhenVertical)) return;
				if(!Item.RaiseShowingEditor(this)) return;
				if(RibbonItemInfo != null) RibbonItemInfo.CheckViewInfo(null);
				if(GetEditBounds().IsEmpty) return;
				Point mousePosCached = Control.MousePosition;	
				UpdateEditorInfoArgs updateEditorInfoArgs = new UpdateEditorInfoArgs(false, GetEditBounds(), Edit.Appearance, Item.EditValue, Manager.PaintStyle.LinksLookAndFeel);
				updateEditorInfoArgs.RightToLeft = Manager.IsRightToLeft;
				BaseEdit be = Manager.EditorHelper.UpdateEditor(Edit, updateEditorInfoArgs);
				be.CausesValidation = Item.CausesValidation;
				be.MenuManager = Manager.InternalMenuManager;
				RibbonPageGroupItemLinkCollection groupLinks = Links as RibbonPageGroupItemLinkCollection;
				if(groupLinks != null && !IsClonedLink)
					groupLinks.Ribbon.SelectedPage = groupLinks.PageGroup.Page;
				Manager.SelectionInfo.ShowEditor(this, be);
				if(byMouse && Manager.ActiveEditor != null) {
					Point p = Manager.SelectionInfo.ActiveEditor.PointToClient(mousePosCached);
					Manager.ActiveEditor.SendMouse(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0));
				}
				if(Ribbon != null)
					Ribbon.Invalidate();
				if(EditorActive) 
					Item.RaiseShownEditor(this);
				if(BarControl != null && BarControl.IsHandleCreated && byMouse)
					BarControl.BeginInvoke(new MethodInvoker(UnpressLink));
			}
		}
		void UnpressLink() {
			Manager.SelectionInfo.UnPressLink(this);
		}
		protected virtual void UpdateEditViewInfo(Point p) {
			RibbonEditItemViewInfo itemInfo = RibbonItemInfo as RibbonEditItemViewInfo;
			if(itemInfo != null) {
				itemInfo.UpdateEditViewInfo(null, p);
				return;
			}
			if(LinkViewInfo != null)
				LinkViewInfo.UpdateViewInfo(p);
		}
		protected virtual BaseEditViewInfo GetEditViewInfo() {
			RibbonEditItemViewInfo itemInfo = RibbonItemInfo as RibbonEditItemViewInfo;
			if(itemInfo != null) return itemInfo.EditViewInfo;
			BarEditLinkViewInfo linkViewInfo = LinkViewInfo as BarEditLinkViewInfo;
			if(linkViewInfo != null) return linkViewInfo.EditViewInfo;
			return null;
		}
		protected virtual Rectangle GetEditBounds() {
			RibbonEditItemViewInfo itemInfo = RibbonItemInfo as RibbonEditItemViewInfo;
			if(itemInfo != null) return itemInfo.EditBounds;
			BarEditLinkViewInfo linkViewInfo = LinkViewInfo as BarEditLinkViewInfo;
			if(linkViewInfo == null) return Rectangle.Empty;
			return linkViewInfo.EditRectangle;
		}
		protected internal override bool IsNeededKey(KeyEventArgs e) {
			if(EditorActive) {
				if(ActiveEditor.IsNeededKey(e)) return true;
				if(e.KeyData == Keys.Enter || e.KeyData == Keys.Escape) return true;
			} else {
				if(CanOpenEdit) {
					if(Edit != null && Edit.IsNeededKey(e.KeyData)) return true;
				}
			}
			return base.IsNeededKey(e);
		}
		protected internal override void ProcessKey(KeyEventArgs e) {
			if(EditorActive) {
				if(ActiveEditor.IsNeededKey(e)) {
					if(!IsInterceptKey(e)) return;
					ActiveEditor.SendKey(e);
					return;
				}
				if(e.KeyData == Keys.Enter) {
					ActiveEditor.SendKey(e);
					if(e.Handled || Manager == null) return;
					Manager.SelectionInfo.OnEditorEnter(this);
					return;
				}
				if(e.KeyData == Keys.Escape) {
					ActiveEditor.SendKey(e);
					if(e.Handled || Manager == null) return;
					CancelEditor();
					Manager.SelectionInfo.OnEditorEscape(this);
					return;
				}
			} else {
				if(CanOpenEdit) {
					if(Edit != null && Edit.IsNeededKey(e.KeyData)) {
						ShowLinkEditor(false);
						if(ActiveEditor != null) ActiveEditor.SendKey(e);
						return;
					}
				}
			}
			base.ProcessKey(e);
		}
		public virtual bool CloseEditor() {
			if(EditorActive) 
				return Manager.SelectionInfo.CloseEditor();
			return true;
		}
		public virtual void PostEditor() {
			if(EditorActive) 
				Manager.SelectionInfo.PostEditor();
		}
		public virtual void HideEditor() {
			if(EditorActive) 
				Manager.SelectionInfo.HideEditor();
		}
		public virtual void CancelEditor() {
			if(EditorActive) 
				Manager.SelectionInfo.CancelEditor();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarEditItemLinkEdit")]
#endif
		public virtual DevExpress.XtraEditors.Repository.RepositoryItem Edit {
			get { return Item == null ? null : Item.Edit; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarEditItemLinkActiveEditor")]
#endif
		public virtual BaseEdit ActiveEditor {
			get {
				return Manager.SelectionInfo.ActiveEditor;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarEditItemLinkEditorActive")]
#endif
		public virtual bool EditorActive {
			get { return Manager != null && Manager.SelectionInfo.EditingLink == this && Manager.SelectionInfo.ActiveEditor != null; }
		}
		protected override DevExpress.XtraEditors.Repository.RepositoryItem RibbonEdit { get { return Edit; } }
		protected internal override void KeyTipItemClickCore() {
			ShowLinkEditor(false);
		}
		internal HorzAlignment GetCaptionAlignment() {
			bool isRightToLeft = WindowsFormsSettings.GetIsRightToLeftLayout(Manager.Form) || 
				(Manager.GetForm() != null && WindowsFormsSettings.GetIsRightToLeftLayout(Manager.GetForm()));
			if(CaptionAlignment == HorzAlignment.Far)
				return isRightToLeft ? HorzAlignment.Near : HorzAlignment.Far;
			else if(CaptionAlignment == HorzAlignment.Default || CaptionAlignment == HorzAlignment.Near)
				return isRightToLeft ? HorzAlignment.Far : HorzAlignment.Near;
			return CaptionAlignment;
		}
	}
	#endregion
}
