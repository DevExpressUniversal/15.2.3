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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Utils;
using DevExpress.Skins.XtraForm;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using System.Linq;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public abstract class BaseRibbonViewInfo : ISupportXtraAnimation {
		Rectangle bounds, contentBounds;
		int largeButtonHeight, buttonHeight, buttonTextHeight, buttonGroupHeight, editorHeight, singleLineLargeButtonHeight, groupCaptionHeight;
		protected int pnlHeight;
		Size buttonArrowSize;
		GraphicsInfo ginfo;
		RibbonHitInfo hotObject, pressedObject, keyboardActiveInfo;
		NavigationObject keyboardActiveObject;
		RibbonItemViewInfoCalculator itemCalculator;
		RibbonItemTextWrapper textWrapper;
		bool isReady, allowCachedItemInfo;
		BaseRibbonDesignTimeManager designTimeManager;
		public BaseRibbonViewInfo() {
			this.hotObject = CreateHitInfo();
			this.pressedObject = CreateHitInfo();
			this.keyboardActiveInfo = CreateHitInfo();
			this.ginfo = new GraphicsInfo();
			this.bounds = this.contentBounds = Rectangle.Empty;
			this.isReady = false;
			this.allowCachedItemInfo = false;
		}
		internal virtual bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(OwnerControl); } }
		public bool AllowCachedItemInfo { get { return allowCachedItemInfo; } set { allowCachedItemInfo = value; } }
		public virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(Manager == null || Manager.SelectionInfo == null) return null;
			RibbonHitInfo hi = CalcHitInfo(point);
			if((hi.InItem || hi.InGalleryItem) && hi.Item != null && hi.Item.Item != null) {
				return hi.Item.GetToolTipInfo(hi, point);
			}
			if(hi.HitTest == RibbonHitTest.PageGroupCaptionButton) {
				ToolTipControlInfo res = new ToolTipControlInfo();
				res.Object = hi.PageGroup;
				res.SuperTip = hi.PageGroup.SuperTip;
				return res;
			}
			if(hi.HitTest == RibbonHitTest.ApplicationButton) {
				ToolTipControlInfo res = new ToolTipControlInfo();
				res.Object = OwnerControl;
				res.SuperTip = (OwnerControl as RibbonControl).ApplicationButtonSuperTip;
				res.ToolTipPosition = OwnerControl.PointToScreen(point);
				res.ToolTipLocation = ToolTipLocation.Default;
				return res;
			}
			if(hi.InFormButton) {
				ToolTipControlInfo res = new ToolTipControlInfo();
				res.Object = OwnerControl;
				res.SuperTip = CreateFormButtonSuperTip(hi.HitTest);
				res.ToolTipPosition = OwnerControl.PointToScreen(point);
				res.ToolTipLocation = ToolTipLocation.Default;
				return res;
			}
			return null;
		}
		protected virtual SuperToolTip CreateFormButtonSuperTip(RibbonHitTest hi) {
			SuperToolTip stip = new SuperToolTip();
			return stip;	
		}
		public abstract RibbonAppearances PaintAppearance { get; }
		protected virtual void UpdateItemHotInfo(RibbonHitInfo hitInfo) {
			if(hitInfo.IsEmpty) return;
			if(hitInfo.InItem && hitInfo.ItemInfo != null) {
				RibbonItemViewInfo info = FindItem(hitInfo.Item, hitInfo.ItemInfo.Bounds);
				if(info == null) return;
				if(Manager.SelectionInfo.HighlightedLink == hitInfo.Item) {
					Manager.SelectionInfo.SetHighlightedLink(info.Item as BarItemLink);
				}
				hitInfo.SetItem(info);
				info.UpdatePaintAppearance();
				return;
			}
			return;
		}
		protected abstract RibbonItemViewInfo FindItem(IRibbonItem item, Rectangle bounds);
		protected internal BaseRibbonDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManager == null) {
					designTimeManager = CreateDesignTimeManager();
					InitializeDesignTimeManager(designTimeManager);
				}
				return designTimeManager;
			}
		}
		protected virtual void InitializeDesignTimeManager(BaseRibbonDesignTimeManager manager) { }
		protected internal int GetFormFrameLeftWidth() {
			SkinElement element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinFormFrameLeft];
			if(element == null) return 0;
			return element.Size.MinSize.Width;
		}
		protected internal int GetFormFrameRightWidth() {
			SkinElement element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinFormFrameRight];
			if(element == null) return 0;
			return element.Size.MinSize.Width;
		}
		public virtual Point MousePosition {
			get {
				if(OwnerControl.IsHandleCreated) return OwnerControl.PointToClient(Control.MousePosition);
				return Point.Empty;
			}
		}
		protected abstract BaseHandler Handler { get; }
		public virtual void UpdateHotObject(bool force) {
			if(Handler != null) {
				if(force) HotObject = null;
				Handler.UpdateHotObject(DXMouseEventArgs.GetMouseArgs(OwnerControl, null), false);
			}
		}
		public abstract bool IsDesignMode { get; }
		protected internal virtual BaseRibbonDesignTimeManager CreateDesignTimeManager() { return null; }
		protected internal abstract void OnItemChanged(RibbonItemViewInfo item);
		public virtual bool IsStatusBar { get { return false; } }
		public abstract Control OwnerControl { get; }
		public bool IsReady { get { return isReady; } set { isReady = value; } }
		public Size LargeImageSize { 
			get {
				int imSize = 32;
				if(Manager.GetController().PropertiesRibbon.ScaleIcons)
					imSize = (int)(imSize * Manager.DrawParameters.RibbonImageScaleFactor);
				return new Size(imSize, imSize); 
			} 
		}
		public Size ImageSize { 
			get {
				int imSize = 16;
				if(Manager.GetController().PropertiesRibbon.ScaleIcons)
					imSize = (int)(imSize * Manager.DrawParameters.RibbonImageScaleFactor);
				return new Size(imSize, imSize); 
			} 
		}
		protected internal virtual RibbonControlStyle GetRibbonStyle() { return RibbonControlStyle.Default; }
		protected internal virtual bool IsOfficeTablet { get { return false; } }
		public virtual RibbonItemTextWrapper TextWrapper {
			get {
				if(textWrapper == null) textWrapper = new RibbonItemTextWrapper(GetRibbonStyle());
				return textWrapper;
			}
		}
		protected virtual bool ShouldSkipObjectChanging(RibbonHitInfo hi) {
			if(hi.InGallery) {
				if(hi.GalleryItem != null && hi.GalleryItem.Gallery == null)
					return true;
				return false;
			}
			if((hi.InItem || hi.ItemInfo != null) && (hi.Item == null || hi.Item.Item == null)) return true;
			return false;
		}
		public virtual RibbonHitInfo HotObject {
			get { return hotObject; }
			set {
				if(value == null) value = CreateHitInfo();
				RibbonHitInfo prev = HotObject;
				bool equals = prev.Equals(value);
				if(!equals) OnHotObjectChanging(HotObject, value);
				hotObject = value;
				if(equals) return;
				OnHotObjectChanged(prev, HotObject);
			}
		}
		public virtual bool IsRibbonFormActive { 
			get {
				if(OwnerControl.FindForm() == null)
					return true;
				Control ctrl = OwnerControl.Parent;
				for(; ctrl != null; ctrl = ctrl.Parent) {
					if(ctrl == Form.ActiveForm) return true;
				}
				return false;
			} 
		} 
		protected virtual void OnHotObjectChanging(RibbonHitInfo prev, RibbonHitInfo current) {
			if(!WindowsFormsSettings.GetAllowHoverAnimation(Provider))
				return;
			RibbonItemViewInfo vi;
			if(prev != null) {
				if(prev.InItem && (prev.Item.IsDisposed || prev.Item.Item == null)) {
					prev.SetHitTest(RibbonHitTest.None);
					prev.SetItem(null, RibbonHitTest.None);
				}
				else if(prev.ItemInfo != null && !ShouldSkipObjectChanging(prev)) {
					if(prev.InGallery && current != null && current.InGallery) {
						if(prev.GalleryInfo == current.GalleryInfo) {
							prev.GalleryInfo.AddBitmapFadeAnimation(prev, current);
							return;
						}
					}
					vi = prev.ItemInfo;
					if(((BarItemLink)vi.Item).RibbonItemInfo == vi) {
						if(vi.CanAnimate) {
							XtraAnimator.Current.AddBitmapFadeAnimation(vi.OwnerControl as ISupportXtraAnimation, vi, ItemAnimationLength != -1 ? ItemAnimationLength : XtraAnimator.Current.CalcBarAnimationLength(ObjectState.Hot, ObjectState.Normal),
								null, GetForeInfo(vi), false);
						}
					}
				}
			}
			if(current != null) {
				if(current.ItemInfo != null && !ShouldSkipObjectChanging(current)) {
					vi = current.ItemInfo;
					if(vi.CanAnimate) {
						XtraAnimator.Current.AddBitmapAnimation(vi.OwnerControl as ISupportXtraAnimation, vi, ItemAnimationLength != -1 ? ItemAnimationLength : XtraAnimator.Current.CalcBarAnimationLength(ObjectState.Normal, ObjectState.Hot),
							null, GetForeInfo(vi), new BitmapAnimationImageCallback(OnGetItemBitmap));
					}
				}
			}
		}
		protected virtual int ItemAnimationLength { get { return -1; } }
		ObjectPaintInfo GetForeInfo(RibbonItemViewInfo itemInfo) {
			return new ObjectPaintInfo(new RibbonItemPainter(), new RibbonDrawInfo(itemInfo));
		}
		Bitmap OnGetItemBitmap(BaseAnimationInfo info) {
			RibbonItemViewInfo itemInfo = info.AnimationId as RibbonItemViewInfo;
			if(itemInfo == null || !itemInfo.CanAnimate) return null;
			return XtraAnimator.Current.CreateBitmap(GetForeInfo(itemInfo));
		}
		protected internal virtual void OnHotObjectChanged(RibbonHitInfo prev, RibbonHitInfo current) {
			if(!ShouldSkipObjectChanging(prev)) NotifyHotTrack(prev, false);
			if(IsRibbonFormActive && !ShouldSkipObjectChanging(current)) {
				NotifyHotTrack(current, true);
			}
			Invalidate(prev);
			if(IsRibbonFormActive) Invalidate(current);
			else Invalidate(OwnerControl.Bounds);
		}
		protected internal void ResetPressedObject() {
			PressedObject = null;
		}
		public virtual RibbonHitInfo PressedObject {
			get { return pressedObject; }
			set {
				if(value == null) value = CreateHitInfo();
				RibbonHitInfo prev = pressedObject;
				pressedObject = value;
				if(prev.Equals(value)) return;
				OnPressedObjectChanged(PressedObject, prev);
			}
		}
		protected virtual void OnPressedObjectChanged(RibbonHitInfo current, RibbonHitInfo prev) {
			if(IsRibbonFormActive && !ShouldSkipObjectChanging(current)) {
				NotifyPressed(current);
				Invalidate(prev);
				Invalidate(HotObject);
			}
		}
		protected virtual void NotifyPressed(RibbonHitInfo current) {
			if(current.InItem)
				current.ItemInfo.UpdatePaintAppearance();
		}
		public NavigationObject KeyboardActiveObject {
			get { return keyboardActiveObject; }
			set {
				if(value == null) {
					keyboardActiveObject = null;
					KeyboardActiveInfo = null;
					return;
				}
				if(KeyboardActiveObject == value) return;
				NavigationObjectRibbonItem item = value as NavigationObjectRibbonItem;
				BarEditItemLink editLink = item != null ? item.Item as BarEditItemLink : null;
				if(Manager.ActiveEditItemLink == editLink) {
					this.keyboardActiveObject = value;
					UpdateKeyboardActiveInfo();
					return;
				}
				bool restoreFocus = KeyboardActiveInfo == null;
				if(Manager.SelectionInfo.ActiveEditor != null && !Manager.SelectionInfo.CloseEditor(restoreFocus))
					return;
				if(Manager == null)
					return;
				if(Manager != null && (editLink == null || !editLink.EditorActive) && !Manager.SelectionInfo.CloseEditor()) {
					Manager.SelectionInfo.HideEditor();
				}
				if(editLink != null && !editLink.EditorActive) {
					editLink.ShowEditor();
				}
				this.keyboardActiveObject = value;
				UpdateKeyboardActiveInfo();
			}
		}
		public RibbonHitInfo KeyboardActiveInfo {
			get { return keyboardActiveInfo; }
			set {
				if(value == null) value = new RibbonHitInfo();
				if(KeyboardActiveInfo.Equals(value)) return;
				RibbonHitInfo prev = keyboardActiveInfo;
				keyboardActiveInfo = value;
				Invalidate(prev);
				Invalidate(keyboardActiveInfo);
			}
		}
		protected virtual void UpdateKeyboardActiveInfo() {
			if(KeyboardActiveObject == null) {
				KeyboardActiveInfo = null;
				return;
			}
		}
		public virtual SkinElement GetElement(string elementName) {
			return RibbonSkins.GetSkin(Provider)[elementName];
		}
		protected virtual void NotifyHotTrack(RibbonHitInfo hitInfo, bool hotTracked) {
			if(hitInfo.InGallery) {
				hitInfo.GalleryInfo.NotifyHotTrack(hitInfo, hotTracked);
			}
			else if(hitInfo.InItem){
				BarItemLink link = HotObject.Item as BarItemLink;
				if(link != null && link.Item == null)
					return;
				Manager.SelectionInfo.HighlightedLink = link;
				hitInfo.ItemInfo.UpdatePaintAppearance();
			}
		}
		public abstract void Invalidate(RibbonItemViewInfo itemInfo);
		public abstract void Invalidate(RibbonHitInfo hitInfo);
		public virtual void Invalidate(Rectangle bounds) {
			if(OwnerControl == null || !OwnerControl.IsHandleCreated) return;
			OwnerControl.Invalidate(bounds);
		}
		protected internal virtual int LargeButtonTextLinesCount { get { return 2; } }
		protected internal virtual void CalcConstants() {
			this.buttonTextHeight = PaintAppearance.Item.CalcDefaultTextSize(GInfo.Graphics).Height;
			this.groupCaptionHeight = PaintAppearance.PageGroupCaption.CalcDefaultTextSize(GInfo.Graphics).Height;
			this.buttonArrowSize = ItemCalculator.CalcButtonArrowSize(GInfo.Graphics);
			this.buttonTextHeight = Math.Max(buttonArrowSize.Height, buttonTextHeight);
			this.singleLineLargeButtonHeight = itemCalculator.CalcSingleLargeButtonHeight(GInfo.Graphics);
			this.largeButtonHeight = itemCalculator.CalcLargeButtonHeight(GInfo.Graphics);
			this.largeButtonHeight = Math.Max(LargeButtonHeight, itemCalculator.CalcLargeSplitButtonHeight(GInfo.Graphics));
			this.buttonHeight = itemCalculator.CalcButtonHeight(GInfo.Graphics);
			this.buttonGroupHeight = itemCalculator.CalcButtonGroupHeight(GInfo.Graphics);
			this.editorHeight = ItemCalculator.CalcEditHeight(GInfo.Graphics);
			if(EditorHeight < ButtonHeight)
				this.editorHeight = ButtonHeight;
			else
				this.buttonHeight = EditorHeight;
			this.textWrapper = null;
		}
		public GraphicsInfo GInfo {
			get {
				return ginfo;
			}
		}
		public abstract ISkinProvider Provider { get; }
		public Size ButtonArrowSize { get { return buttonArrowSize; } }
		public int ButtonTextHeight { get { return buttonTextHeight; } }
		public int GroupCaptionHeight { get { return groupCaptionHeight; } }
		public int LargeButtonHeight { get { return largeButtonHeight; } }
		public int SingleLineLargeButtonHeight { get { return singleLineLargeButtonHeight; } }
		public int ButtonHeight { get { return buttonHeight; } }
		public int EditorHeight { get { return editorHeight; } }
		public int PanelHeight { get { return pnlHeight; } }
		public int ButtonGroupHeight { get { return buttonGroupHeight; } }
		public virtual int LargeButtonMinWidth { get { return 42; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public RibbonItemViewInfoCalculator ItemCalculator { 
			get {
				if(itemCalculator == null)
					itemCalculator = CreateItemCalculator();
				return itemCalculator; 
			} 
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
		}
		protected virtual bool HasAnimatedItem(EditorAnimationInfo info, RibbonItemViewInfoCollection coll) {
			BarEditItemLink edit;
			if(coll == null) return false;
			for(int i = 0; i < coll.Count; i++) {
				edit = coll[i].Item as BarEditItemLink;
				if(edit != null && edit == info.Link) return true;
			}
			return false;
		}
		public RibbonHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		protected virtual bool IsManagerDestroing {
			get {
				return Manager != null && Manager.IsDestroying;
			}
		}
		public virtual RibbonHitInfo CalcHitInfo(Point hitPoint) {
			RibbonHitInfo res = CreateHitInfo();
			if (IsManagerDestroing) return res;
			res.HitPoint = hitPoint;
			res.ContainsSet(Bounds, RibbonHitTest.Panel);
			return res;
		}
		public virtual RibbonBarManager Manager { get { return null; } }
		protected internal virtual RibbonItemViewInfo CreateItemViewInfo(IRibbonItem item) {
			return Manager.CreateItemViewInfo(this, item);
		}
		protected internal virtual RibbonHitInfo CreateHitInfo() { return new RibbonHitInfo(); }
		protected internal virtual Rectangle CalcContentBounds() { return Bounds; }
		protected virtual RibbonItemViewInfoCalculator CreateItemCalculator() { return new RibbonItemViewInfoCalculator(this);  }
		protected internal virtual void Clear() { 
			this.itemCalculator = null;
		}
		protected internal virtual bool ShouldCreateItemInfo(BarItemLink link) {
			if(link.Item == null || !link.CanVisible) return false;
			BarButtonGroup group = link.Item as BarButtonGroup;
			if((group != null && !group.ShouldCreateViewInfo()) || (link.Item.Visibility == BarItemVisibility.Never && !IsDesignMode)) return false;
			return true;
		}
		public virtual BarItemLink GetLinkByPoint(Point pt, bool enterButtonGroup) { return null; }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		Control ISupportXtraAnimation.OwnerControl { get { return OwnerControl; } }
		protected internal virtual SkinElementInfo GetScrollButtonInfo(string scrollElementName, RibbonHitTest buttonHitTest, Rectangle buttonRect) {
			SkinElement elem = RibbonSkins.GetSkin(Provider)[scrollElementName];
			if(elem == null) return null;
			SkinElementInfo info = new SkinElementInfo(elem, buttonRect);
			info.ImageIndex = 0;
			if(PressedObject.HitTest == buttonHitTest || HotObject.HitTest == buttonHitTest) info.ImageIndex = 1;
			if(!OwnerControl.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected internal virtual Size GetScrollButtonSize(string scrollElementName, int height) {
			SkinElement elem = RibbonSkins.GetSkin(Provider)[scrollElementName];
			if(elem == null || elem.Image == null || elem.Image.Image == null) return new Size(16, height);
			return new Size(elem.Image.GetImageBounds(0).Width, height);
		}
		internal bool SuppressInvalidate { get; set; }
	}
	public class RibbonViewInfoPopupToolbarOnly : RibbonOneGroupControlViewInfo {
		RibbonQuickAccessToolbarViewInfo sourceToolbarInfo;
		public RibbonViewInfoPopupToolbarOnly(RibbonControl control, RibbonQuickAccessToolbarViewInfo sourceToolbarInfo)
			: base(control) {
			this.sourceToolbarInfo = sourceToolbarInfo;
		}
		protected override void InitializeDesignTimeManager(BaseRibbonDesignTimeManager manager) {
			manager.Initialize(Ribbon, null);
		}
		public override bool IsAllowCaption { get { return false; } }
		public override bool IsAllowDisplayRibbon { get { return true; } }
		public Size CalcPopupToolbarSize() { return ((RibbonQuickAccessDropDownToolbarViewInfo)Toolbar).CalcPopupSize(); }
		public RibbonQuickAccessToolbarViewInfo SourceToolbarInfo { get { return sourceToolbarInfo; } }
		protected override void CalcPageHeader() { }
		protected override void CalcPanel() { }
		protected override void CalcCaption() {
			CalcToolbarAtTopLocation();
		}
		protected override void CalcToolbarAtTopLocation() {
			Rectangle rect = new Rectangle(ContentBounds.X, ContentBounds.Y, ContentBounds.Width, Toolbar.CalcMinHeight());
			if(Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Below) rect.Y += Header.Bounds.Height + Panel.Bounds.Height;
			Toolbar.CalcViewInfo(rect);
		}
		protected override RibbonQuickAccessToolbarViewInfo CreateToolbarInfo() {
			return new RibbonQuickAccessDropDownToolbarViewInfo(Ribbon.Toolbar);
		}
		protected class RibbonQuickAccessDropDownToolbarViewInfo : RibbonQuickAccessToolbarViewInfo {
			public RibbonQuickAccessDropDownToolbarViewInfo(RibbonQuickAccessToolbar toolbar)
				: base(toolbar) {
			}
			protected internal override SkinElementInfo GetBackgroundInfo() { return null; }
			protected internal override SkinElementInfo GetToolbarInfo() {
				return new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinQuickToolbarDropDown], Bounds);
			}
			public virtual Size CalcPopupSize() {
				GInfo.AddGraphics(null);
				try {
					CreateItemsViewInfo();
					return UpdateContentBackgroundBounds(new Rectangle(Point.Empty, CalcBestSize())).Size;
				}
				finally {
					GInfo.ReleaseGraphics();
				}
			}
		}
	}
	public class CustomBaseRibbonViewInfo : RibbonViewInfo {
		public CustomBaseRibbonViewInfo(RibbonControl control) : base(control) { }
		public override int TabHeaderHeight { get { return 0; } }
		public override bool GetShowPageHeaders() { return false; }
		public override bool IsAllowCaption { get { return false; } }
		public override bool IsAllowDisplayRibbon { get { return true; } }
		public override RibbonQuickAccessToolbarLocation GetToolbarLocation() { return RibbonQuickAccessToolbarLocation.Hidden; }
		public override bool GetIsMinimized() { return false; }
		public override bool IsRibbonMerged { get { return ((CustomBaseRibbonControl)Ribbon).SourceRibbon.IsMergeOwner; } }
		public override bool IsAllowApplicationButton { get { return false; } }
	}
	public class PreviewRibbonViewInfo : CustomBaseRibbonViewInfo {
		public PreviewRibbonViewInfo(RibbonControl control) : base(control) { }
		public override bool GetShowPageHeaders() {
			return true;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			bounds.Width = 10000;
			base.CalcViewInfo(bounds);
			SkinElementInfo info = GetPanelInfo();
			info.Bounds = new Rectangle(info.Bounds.X, info.Bounds.Y, Panel.CalcTotalWidth(), info.Bounds.Height);
			bounds.Width = ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, info.Bounds).Width;
			IsReady = false;
			base.CalcViewInfo(bounds);
		}
	}
	public class RibbonMinimizedControlViewInfo : CustomBaseRibbonViewInfo {
		public RibbonMinimizedControlViewInfo(RibbonControl control) : base(control) { }
		internal override bool IsRightToLeft { get { return Manager.Ribbon.ViewInfo.IsRightToLeft; } }
	}
	public class RibbonFullscreenMinimizedControlViewInfo : RibbonMinimizedControlViewInfo {
		public RibbonFullscreenMinimizedControlViewInfo(RibbonControl control) : base(control) { }
		public RibbonFullscreenMinimizedControl RibbonMinimizedControl { get { return (RibbonFullscreenMinimizedControl)Ribbon; } }
		public override RibbonQuickAccessToolbarLocation GetToolbarLocation() {
			if(RibbonMinimizedControl.SourceRibbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Below)
				return RibbonQuickAccessToolbarLocation.Below;
			return base.GetToolbarLocation();
		}
	}
	public class RibbonOneGroupControlPanelViewInfo : RibbonPanelViewInfo {
		public RibbonOneGroupControlPanelViewInfo(RibbonViewInfo vi) : base(vi) { }
	}
	public class RibbonOneGroupControlViewInfo : RibbonViewInfo {
		public RibbonOneGroupControlViewInfo(RibbonControl control) : base(control) { }
		public override bool GetIsMinimized() { return false; }
		internal override bool IsRightToLeft {
			get {
				RibbonOneGroupControl control = Ribbon as RibbonOneGroupControl;
				return control == null ? base.IsRightToLeft : control.SourceRibbon.ViewInfo.IsRightToLeft;
			}
		}
		protected override RibbonPanelViewInfo CreatePanelInfo() {
			return new RibbonOneGroupControlPanelViewInfo(this);
		}
		protected internal override RibbonPageGroupLayoutCalculator CreateGroupLayoutCalculator(RibbonPageGroupViewInfo groupInfo) {
			return new RibbonPageGroupLayoutCalculator(groupInfo);
		}
		public override int TabHeaderHeight { get { return 0; } }
		protected override void CalcPageHeader() { }
		public override bool IsAllowCaption { get { return false; } }
		protected internal override bool IsAllowGroupMinimize(RibbonPageGroupViewInfo groupInfo) {
			return false;
		}
		protected override void InitializeDesignTimeManager(BaseRibbonDesignTimeManager manager) {
			manager.Initialize(Ribbon, ((RibbonOneGroupControl)Ribbon).SourceRibbon.Site);
		}
		protected internal override int CalcMinHeight() {
			if(Panel.Groups.Count == 0) return 100;
			return Panel.Bounds.Height;
		}
		public override void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.None) return;
			if(hitInfo.InPageGroup) {
				Panel.Invalidate(hitInfo.PageGroup);
				return;
			}
			if(hitInfo.InPageItem) {
				Panel.Invalidate(hitInfo);
			}
		}
		protected internal override void Clear() {
			base.IsReady = false;
			Panel.Clear();
			Toolbar.ClearItemsViewInfo();
		}
		public override bool IsAllowDisplayRibbon { get { return true; } }
		public override RibbonQuickAccessToolbarLocation GetToolbarLocation() { return RibbonQuickAccessToolbarLocation.Hidden; }
	}
	public class RibbonViewInfo : BaseRibbonViewInfo {
		RibbonPageHeaderViewInfo header;
		RibbonPanelViewInfo panel;
		RibbonQuickAccessToolbarViewInfo toolbar;
		RibbonControl ribbon;
		RibbonAppearances paintAppearance, defaultAppearance;
		RibbonCaptionViewInfo caption;
		RibbonApplicationButtonInfo applicationButton;
		Size applicationButtonSize;
		RibbonPage firstVisiblePage;
		bool applicationButtonPopupActive = false;
		bool paintAppearanceDirty = true;
		ObjectPainter fullScreenBarPainter;
		int defaultIndentBetweenColumns, defaultIndentBetweenButtonGroups, groupContentHeight, defaultIndentBetweenRows, defaultIndentBetweenButtonGroupRows,
			defaultIndentBetweenPageGroups, defaultPageGroupCaptionVerticalIndent, defaultPageGroupCaptionHorizontalIndent, defaultPageGroupCaptionSeparatorHorizontalIndent, defaultPageHeaderTopIndent,
			tabHeaderHeight;
		public RibbonViewInfo(RibbonControl ribbon) {
			this.applicationButtonSize = Size.Empty;
			this.paintAppearance = new RibbonAppearances(null);
			this.defaultAppearance = null;
			this.ribbon = ribbon;
			this.header = CreateHeaderInfo();
			this.panel = CreatePanelInfo();
			this.toolbar = CreateToolbarInfo();
			this.caption = CreateCaptionInfo();
			this.applicationButton = CreateApplicationButtonInfo();
			this.IsFullScreenModeActive = IsPopupFullScreenModeActive = false;
			this.FullScreenBarGlyphBounds = Rectangle.Empty;
		}
		protected internal bool ShouldDrawGroupBorder {
			get {
				if(GetRibbonStyle() == RibbonControlStyle.TabletOffice) return false;
				return Ribbon.DrawGroupsBorder;
			}
		}
		protected internal virtual bool ShouldDrawGroupCaption { 
			get{
				if(!ShouldDrawGroupBorder) return false;
				if(Ribbon.DrawGroupCaptions == DefaultBoolean.Default)
					return !Ribbon.IsOfficeTablet;
				return Ribbon.DrawGroupCaptions == DefaultBoolean.True;
			}
		}
		public ObjectPainter FullScreenBarPainter {
			get {
				if(fullScreenBarPainter == null) fullScreenBarPainter = CreateFullScreenBarPainter();
				return fullScreenBarPainter;
			}
		}
		protected internal override bool IsOfficeTablet {
			get {
				return Ribbon.IsOfficeTablet;
			}
		}
		protected virtual ObjectPainter CreateFullScreenBarPainter() {
			return new RibbonFullScreenBarPainter();
		}
		protected override RibbonItemViewInfoCalculator CreateItemCalculator() {
			if(Ribbon.RibbonStyle == RibbonControlStyle.MacOffice)
				return new RibbonMacStyleItemViewInfoCalculator(this);
			return base.CreateItemCalculator();
		}
		protected internal bool SupportTransparentPages {
			get {
				object prop = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.SkinSupportTransparentPages];
				if (prop == null)
					return false;
				return (bool)prop;
			}
		}
		protected internal bool IsTransparentPagesOnGlassForm {
			get {
				return SupportTransparentPages && Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 && Form != null && Form.IsGlassForm;
			}
		}
		protected internal override RibbonControlStyle GetRibbonStyle() {
			return Ribbon.GetRibbonStyle();
		}
		public int ApplicationButtonLeftIndent {
			get {
				return RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptFormApplicationButtonLeftIndent);
			}
		}
		protected internal bool IsColored {
			get {
				object obj = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptIsColoredBackstageView];
				return obj == null ? false : (bool)obj;
			}
		}
		protected virtual internal bool ShouldDrawPageCategories() {
			if(Header.PageCategories.Count == 0) return false;
			if(!Ribbon.ShowCategoryInCaption) return false;
			switch(GetRibbonStyle()) {
				case RibbonControlStyle.MacOffice:
				case RibbonControlStyle.OfficeUniversal:
					return false;
				default:
					return true;
			}
		}
		internal override bool IsRightToLeft {
			get {
				if(WindowsFormsSettings.GetIsRightToLeft(Ribbon)) return true;
				return base.IsRightToLeft;
			}
		}
		protected override RibbonItemViewInfo FindItem(IRibbonItem item, Rectangle bounds) { return Panel.Groups.FindItem(item, bounds); }
		public virtual bool GetIsMinimized() {
			return Ribbon.Minimized;
		}
		public RibbonPage FirstVisiblePage { get { return firstVisiblePage; } set { firstVisiblePage = value; } }
		public int ApplicationButtonRightIndent {
			get {
				return RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptFormApplicationButtonRightIndent);
			}
		}
		public bool IsPaintAppearanceDirty { get { return paintAppearanceDirty; } }
		protected override BaseHandler Handler { get { return Ribbon.Handler; } }
		public virtual bool IsRibbonMerged { get { return Ribbon.MergedRibbon != null; } }
		public override RibbonAppearances PaintAppearance {
			get {
				if(IsPaintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		protected override int ItemAnimationLength { get { return Ribbon.ItemAnimationLength; } }
		public void SetAppearanceDirty() {
			this.paintAppearanceDirty = true;
			this.defaultAppearance = null;
		}
		protected override void UpdateKeyboardActiveInfo() {
			base.UpdateKeyboardActiveInfo();
			if(KeyboardActiveObject != null) KeyboardActiveInfo = KeyboardActiveObject.CreateHitInfo(this);
		}
		protected override SuperToolTip CreateFormButtonSuperTip(RibbonHitTest hi) {
			SuperToolTip stip = new SuperToolTip();
			ToolTipItem item = new ToolTipItem();
			if(hi == RibbonHitTest.FormMinimizeButton)
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.MinimizeButton);
			else if(hi == RibbonHitTest.FormMaximizeButton) {
				if(Form.WindowState == FormWindowState.Maximized)
					item.Text = BarLocalizer.Active.GetLocalizedString(BarString.RestoreButton);
				else						
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.MaximizeButton);
			}
			else if(hi == RibbonHitTest.FormCloseButton)
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.CloseButton);
			else if(hi == RibbonHitTest.FormFullScreenButton)
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.FullScreenButton);
			else
				item.Text = BarLocalizer.Active.GetLocalizedString(BarString.HelpButton);
			stip.Items.Add(item);
			return stip;
		}
		protected override void OnHotObjectChanging(RibbonHitInfo prev, RibbonHitInfo current) {
			if(!IsRibbonFormActive) return;
			if(prev.HitTest == RibbonHitTest.Item) {
				CheckUpdateSystemLinkGlyph(prev, ObjectState.Normal);
			}
			if(current.HitTest == RibbonHitTest.Item) {
				CheckUpdateSystemLinkGlyph(current, ObjectState.Hot);
			}
			base.OnHotObjectChanging(prev, current);
			if(prev.PageGroup == current.PageGroup && prev.InPageGroup) return;
			if(prev != null && prev.InPageGroup) {
				XtraAnimator.Current.AddFadeAnimation(OwnerControl as ISupportXtraAnimation, prev.PageGroupInfo, Ribbon.GroupAnimationLength != -1 ? Ribbon.GroupAnimationLength : 300, prev.PageGroupInfo.Bounds, false);
				if(Ribbon.GroupAnimationLength == 0) {
					Invalidate(prev.PageGroupInfo.Bounds);
				}
			}
			if(current != null && current.InPageGroup) {
				if(WindowsFormsSettings.GetAllowHoverAnimation(Provider))
					XtraAnimator.Current.AddFadeAnimation(OwnerControl as ISupportXtraAnimation, current.PageGroupInfo, Ribbon.GroupAnimationLength != -1 ? Ribbon.GroupAnimationLength : 300, current.PageGroupInfo.Bounds, true);
				if(Ribbon.GroupAnimationLength == 0) {
					Invalidate(current.PageGroupInfo.Bounds);
				}
			}
			if(prev.HitTest == RibbonHitTest.PageHeader) {
				RibbonPageViewInfo page = Header.Pages[prev.Page];
				if(page != null && WindowsFormsSettings.GetAllowHoverAnimation(Provider))
					XtraAnimator.Current.AddBitmapFadeAnimation(OwnerControl as ISupportXtraAnimation, page, Ribbon.PageAnimationLength != -1 ? Ribbon.PageAnimationLength : XtraAnimator.Current.CalcBarAnimationLength(ObjectState.Hot, ObjectState.Normal),
						null, new ObjectPaintInfo(new RibbonHeaderPagePainter(), new RibbonDrawInfo(page)), false);
			}
			if(current.HitTest == RibbonHitTest.PageHeader) {
				RibbonPageViewInfo page = Header.Pages[current.Page];
				if(page != null && WindowsFormsSettings.GetAllowHoverAnimation(Provider))
					XtraAnimator.Current.AddBitmapAnimation(OwnerControl as ISupportXtraAnimation, page, Ribbon.PageAnimationLength != -1 ? Ribbon.PageAnimationLength : XtraAnimator.Current.CalcBarAnimationLength(ObjectState.Normal, ObjectState.Hot),
						null, new ObjectPaintInfo(new RibbonHeaderPagePainter(), new RibbonDrawInfo(page)), new BitmapAnimationImageCallback(OnGetPageBitmap));
			}
			if(prev.HitTest == RibbonHitTest.ApplicationButton || current.HitTest == RibbonHitTest.ApplicationButton) {
				ObjectState start = ObjectState.Hot, dest = ObjectState.Normal;
				if(current.HitTest == RibbonHitTest.ApplicationButton) { start = ObjectState.Normal; dest = ObjectState.Hot; }
				if(WindowsFormsSettings.GetAllowHoverAnimation(Provider))
					XtraAnimator.Current.AddBitmapAnimation(OwnerControl as ISupportXtraAnimation, RibbonHitTest.ApplicationButton, Ribbon.ApplicationButtonAnimationLength != -1 ? Ribbon.ApplicationButtonAnimationLength : XtraAnimator.Current.CalcAnimationLength(start, dest),
						null, new ObjectPaintInfo(new RibbonApplicationButtonPainter(), ApplicationButton),
						new BitmapAnimationImageCallback(OnGetApplicationButtonBitmap));
			}
			if(prev.HitTest == RibbonHitTest.PanelLeftScroll) Invalidate(Panel.LeftScrollButtonBounds);
			if(prev.HitTest == RibbonHitTest.PanelRightScroll) Invalidate(Panel.RightScrollButtonBounds);
			if(prev.HitTest == RibbonHitTest.PageHeaderLeftScroll) Invalidate(Header.LeftScrollBounds);
			if(prev.HitTest == RibbonHitTest.PageHeaderRightScroll) Invalidate(Header.RightScrollBounds);
		}
		void CheckUpdateSystemLinkGlyph(RibbonHitInfo info, ObjectState state) {
			if(info.HitTest != RibbonHitTest.Item)
				return;
			BarItemLink link = info.Item;
			if(link == null || !Ribbon.IsSystemLink(link))
				return;
			Ribbon.UpdateSystemLinkGlyph(link, state);
		}
		Bitmap OnGetApplicationButtonBitmap(BaseAnimationInfo info) {
			return XtraAnimator.Current.CreateBitmap(new RibbonApplicationButtonPainter(), GetApplicationButtonInfo());
		}
		Bitmap OnGetPageBitmap(BaseAnimationInfo info) {
			RibbonPageViewInfo itemInfo = info.AnimationId as RibbonPageViewInfo;
			if(itemInfo == null) return null;
			return XtraAnimator.Current.CreateBitmap(new ObjectPaintInfo(new RibbonHeaderPagePainter(), new RibbonDrawInfo(itemInfo)));
		}
		public override RibbonHitInfo HotObject {
			get { return base.HotObject; }
			set {
				if(ApplicationButtonPopupActive) {
					base.HotObject = null;
					return;
				}
				base.HotObject = value;
			}
		}
		public override RibbonHitInfo PressedObject {
			get { return base.PressedObject; }
			set {
				if(ApplicationButtonPopupActive) {
					RibbonHitInfo hi = CreateHitInfo();
					hi.SetHitTest(RibbonHitTest.ApplicationButton);
					base.PressedObject = hi;
					return;
				}
				base.PressedObject = value;
			}
		}
		public bool ApplicationButtonPopupActive {
			get { return applicationButtonPopupActive; }
			set {
				if(ApplicationButtonPopupActive == value) return;
				applicationButtonPopupActive = value;
				HotObject = null;
				PressedObject = null;
			}
		}
		public virtual bool ShouldUseAppButtonContainerControlBkgnd {
			get {
				if(Ribbon == null || IsDesignMode) return false;
				IBackstageViewControl bsvc = Ribbon.ApplicationButtonDropDownControl as IBackstageViewControl;
				if(bsvc == null) return false;
				return bsvc.WindowsUIStyleActive;
			}
		}
		public bool IsFullScreenModeActive { get; set; }
		public bool IsPopupFullScreenModeActive { get; set; }
		protected internal virtual void EnterToPopupFullScreenMode() {
			IsPopupFullScreenModeActive = true;
			IsFullScreenModeActive = false;
		}
		protected internal virtual void RestoreFromPopupFullScreenMode() {
			IsPopupFullScreenModeActive = false;
			IsFullScreenModeActive = true;
		}
		protected internal virtual bool CanProcessFullScreenActions(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return false;
			RibbonHitInfo hitInfo = CalcHitInfo(e.Location);
			if(hitInfo.HitTest != RibbonHitTest.FullScreenModeBar) return false;
			return CanProcessFullScreenActions();
		}
		protected internal virtual bool CanProcessFullScreenActions() {
			return IsFullScreenModeActive;
		}
		protected internal virtual bool CanTranslateToFullScreenBarClick(MouseEventArgs e, FormCaptionButtonAction kind) {
			if(kind == FormCaptionButtonAction.FullScreen) return true;
			if(kind == FormCaptionButtonAction.Restore && IsFullScreenModeActive) return true;
			return false;
		}
		protected internal virtual int RenderImageOffset {
			get {
				if(Form == null) return 0;
				return Math.Abs(Form.Location.Y);
			}
		}
		protected internal virtual bool CanCloseAllPopups {
			get {
				RibbonMinimizedControl minimizedRibbon = Ribbon as RibbonMinimizedControl;
				if(minimizedRibbon == null || minimizedRibbon.SourceRibbon == null)
					return CanCloseMinimizedRibbon(Ribbon);
				return CanCloseMinimizedRibbon(minimizedRibbon.SourceRibbon);
			}
		}
		protected internal virtual bool CanCloseMinimizedRibbon(RibbonControl ribbon) {
			bool fsPopupActive = ribbon.ViewInfo.IsPopupFullScreenModeActive;
			return !fsPopupActive;
		}
		public override bool IsDesignMode { get { return Ribbon != null && Ribbon.IsDesignMode; } }
		protected internal override BaseRibbonDesignTimeManager CreateDesignTimeManager() {
			return new RibbonDesignTimeManager();
		}
		protected override void InitializeDesignTimeManager(BaseRibbonDesignTimeManager manager) {
			manager.Initialize(Ribbon, null);
		}
		protected internal override void OnItemChanged(RibbonItemViewInfo item) {
			Ribbon.Refresh();
		}
		protected internal RibbonAppearances DefaultAppearance {
			get {
				if(defaultAppearance == null) defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
			set {
				defaultAppearance = value;
			}
		}
		AppearanceDefault GetAppearance(string skinName, AppearanceDefault defApp) {
			SkinElement element = GetElement(skinName);
			if(element != null)
				element.Apply(defApp);
			return defApp;
		}
		AppearanceDefault GetPageGroupCaptionAppearance(AppearanceDefault defApp) {
			return GetAppearance(RibbonSkins.SkinTabPanelGroupCaption, defApp);
		}
		AppearanceDefault GetTabHeaderPageAppearance(AppearanceDefault defApp) { 
			return GetAppearance(RibbonSkins.SkinTabHeaderPage, defApp);
		}
		protected string GetCaptionSkinName() {
			if(Caption != null) return Caption.GetCaptionSkinName();
			return RibbonSkins.SkinFormCaption;
		}
		protected SkinElement GetTabHeaderPageElement() {
			return GetElement(RibbonSkins.SkinTabHeaderPage);
		}
		protected bool IsOffice2010Style { get { return Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007; } }
		protected virtual RibbonAppearances CreateDefaultAppearance() {
			RibbonAppearances res = new RibbonAppearances(null);
			AppearanceDefault item = GetAppearance(RibbonSkins.SkinButton, new AppearanceDefault(Color.Black, Color.Empty));
			res.Item.Assign(item);
			res.ItemHovered.Assign(item);
			res.ItemPressed.Assign(item);
			SkinElement elem = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinAppMenuItemDescription];
			if(elem != null) {
				res.ItemDescriptionHovered.ForeColor = elem.GetForeColor(ObjectState.Hot);
				res.ItemDescriptionPressed.ForeColor = elem.GetForeColor(ObjectState.Pressed);
				res.ItemDescriptionDisabled.ForeColor = elem.GetForeColor(ObjectState.Disabled);
			}
			item.ForeColor = RibbonSkins.GetSkin(Provider).Colors[RibbonSkins.OptColorButtonDisabled];
			res.ItemDisabled.Assign(item);
			HorzAlignment groupCaptionHorzAlignment = HorzAlignment.Center;
			res.PageGroupCaption.Assign(GetPageGroupCaptionAppearance(new AppearanceDefault(Color.Black, Color.Empty, groupCaptionHorzAlignment, VertAlignment.Center)));
			res.PageHeader.Assign(GetTabHeaderPageAppearance(new AppearanceDefault(Color.Black, Color.Empty)));
			Color pageCategoryColor = IsOffice2010Style? Color.White: Color.Black;
			HorzAlignment pageCategoryHorzAlignment = IsOffice2010Style ? HorzAlignment.Center : HorzAlignment.Default;
			res.PageCategory.Assign(GetAppearance(RibbonSkins.SkinContextTabCategory, new AppearanceDefault(pageCategoryColor, Color.Empty, pageCategoryHorzAlignment)));
			res.FormCaption.Assign(GetAppearance(GetCaptionSkinName(), new AppearanceDefault(Color.Black, Color.Empty)));
			res.FormCaption.TextOptions.WordWrap = WordWrap.NoWrap;
			SkinElement formCaption = RibbonSkins.GetSkin(Provider)[GetCaptionSkinName()];
			res.FormCaption.TextOptions.Trimming = Trimming.EllipsisCharacter;
			if(formCaption != null) {
				res.FormCaptionForeColor2 = formCaption.Properties.GetColor(RibbonSkins.OptFormCaptionTextColor2);
				res.FormCaptionForeColorInactive = formCaption.Properties.GetColor(RibbonSkins.OptFormCaptionTextColorDisabled);
			}
			if(!OwnerControl.Enabled) {
				Color resColor = GetTabHeaderPageElement().GetForeColor(ObjectState.Disabled);
				res.PageHeader.ForeColor = resColor.IsEmpty? RibbonSkins.GetSkin(Provider).GetSystemColor(SystemColors.GrayText): resColor;
				resColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelGroupCaption].GetForeColor(ObjectState.Disabled);
				res.PageGroupCaption.ForeColor = resColor.IsEmpty? RibbonSkins.GetSkin(Provider).GetSystemColor(SystemColors.GrayText): resColor;
				resColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinContextTabCategory].GetForeColor(ObjectState.Disabled);
				res.PageCategory.ForeColor = resColor.IsEmpty? RibbonSkins.GetSkin(Provider).GetSystemColor(SystemColors.GrayText): resColor;
			}
			return res;
		}
		protected virtual void UpdatePaintAppearance() {
			this.paintAppearanceDirty = false;
			PaintAppearance.Combine(Ribbon.GetController().AppearancesRibbon, DefaultAppearance);
			if(Caption.IsGlassForm)
				PaintAppearance.FormCaption.Font = AppearanceObject.DefaultMenuFont;
			UpdatePaintAppearanceToRTL(IsRightToLeft);
		}
		protected virtual void UpdatePaintAppearanceToRTL(bool value) {
			PaintAppearance.FormCaption.TextOptions.RightToLeft = value;
			PaintAppearance.Item.TextOptions.RightToLeft = value;
			PaintAppearance.ItemDescription.TextOptions.RightToLeft = value;
			PaintAppearance.ItemDescriptionDisabled.TextOptions.RightToLeft = value;
			PaintAppearance.ItemDescriptionHovered.TextOptions.RightToLeft = value;
			PaintAppearance.ItemDescriptionPressed.TextOptions.RightToLeft = value;
			PaintAppearance.ItemDisabled.TextOptions.RightToLeft = value;
			PaintAppearance.ItemHovered.TextOptions.RightToLeft = value;
			PaintAppearance.ItemPressed.TextOptions.RightToLeft = value;
			PaintAppearance.PageCategory.TextOptions.RightToLeft = value;
			PaintAppearance.PageGroupCaption.TextOptions.RightToLeft = value;
			PaintAppearance.PageHeader.TextOptions.RightToLeft = value;
		}
		public virtual bool IsAllowApplicationButton {
			get {
				if(Ribbon.ShowApplicationButton == DefaultBoolean.False)
					return false;
				if(GetRibbonStyle() == RibbonControlStyle.OfficeUniversal)
					return true;
				if(IsOfficeTablet)
					return false;
				if(Ribbon.ShowApplicationButton == DefaultBoolean.True)
					return Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 ? true : Form != null ;
				if(Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
					return false;
				if(Ribbon.IsOffice2010LikeStyle)
					return true;
				return Form != null && !Form.IsMdiChild;
			}
		}
		public virtual ArrayList GetVisiblePages() {
			return Ribbon.PageCategories.TotalCategory.GetVisiblePages();
		}
		public virtual bool GetShowPageHeaders() {
			bool showPages = Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.Default || Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.Show;
			showPages |= Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.ShowOnMultiplePages && GetVisiblePages().Count > 1;
			return showPages;
		}
		public virtual Size RibbonRequiredFormSize { get { return new Size(300, 220); } }
		public virtual Size ApplicationButtonSize { get { return applicationButtonSize; } }
		public virtual int DefaultIndentBetweenRows { get { return defaultIndentBetweenRows; } }
		public virtual int DefaultIndentBetweenButtonGroupRows { get { return defaultIndentBetweenButtonGroupRows; } }
		public virtual int DefaultIndentBetweenColumns { get { return defaultIndentBetweenColumns; } }
		public virtual int DefaultPageGroupCaptionVerticalIndent { get { return defaultPageGroupCaptionVerticalIndent; } }
		public virtual int DefaultPageGroupCaptionHorizontalIndent { get { return defaultPageGroupCaptionHorizontalIndent; } }
		public virtual int DefaultPageGroupCaptionSeparatorHorizontalIndent { get { return defaultPageGroupCaptionSeparatorHorizontalIndent; } }
		public virtual int DefaultIndentBetweenButtonGroups { get { return defaultIndentBetweenButtonGroups; } }
		public virtual int DefaultIndentBetweenPageGroups { get { return defaultIndentBetweenPageGroups; } }
		public virtual int DefaultPageHeaderTopIndent { get { return defaultPageHeaderTopIndent; } }
		public virtual int GroupContentHeight { get { return groupContentHeight; } } 
		protected internal override void CalcConstants() {
			this.defaultPageHeaderTopIndent = CalcDefaultPageHeaderTopIndent();
			this.tabHeaderHeight = CalcTabHeaderHeight();
			this.defaultIndentBetweenColumns = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptIndentBetweenColumns);
			this.defaultIndentBetweenRows = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptIndentBetweenRows);
			this.defaultIndentBetweenButtonGroupRows = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptIndentBetweenButtonGroupRows);
			this.defaultIndentBetweenButtonGroups = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptIndentBetweenButtonGroups);
			this.defaultIndentBetweenPageGroups = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptIndentBetweenPageGroups);
			this.defaultPageGroupCaptionVerticalIndent = CalcPageGroupCaptionVerticalIndent();
			this.defaultPageGroupCaptionHorizontalIndent = CalcPageGroupCaptionHorizontalIndent();
			this.defaultPageGroupCaptionSeparatorHorizontalIndent = CalcPageGroupCaptionSeparatorHorizontalIndent();
			this.applicationButtonSize = CalcApplicationButtonSize();
			base.CalcConstants();
			this.groupContentHeight = CreateGroupLayoutCalculator(null).CalcGroupContentHeight(this);
		}
		protected virtual int CalcDefaultPageHeaderTopIndent() {
			int res = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptPageHeaderTopIndent);
			return res > 0 ? res : 4;
		}
		protected int CalcPageGroupCaptionSeparatorHorizontalIndent() {
			int res = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptPageGroupCaptionSeparatorHorizontalIndent);
			return res > 0 ? res : 2;
		}
		protected int CalcPageGroupCaptionVerticalIndent() {
			int res = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptPageGroupCaptionVerticalIndent);
			return res > 0 ? res : 2;
		}
		protected int CalcPageGroupCaptionHorizontalIndent() {
			int res = RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptPageGroupCaptionHorizontalIndent);
			return res > 0 ? res : 6;
		}
		public virtual RibbonApplicationButtonInfo GetApplicationButtonInfo() {
			RibbonApplicationButtonInfo res = ApplicationButton;
			res.State = ObjectState.Normal;
			if(KeyboardActiveInfo.HitTest == RibbonHitTest.ApplicationButton) res.State |= ObjectState.Hot;
			if(HotObject.HitTest == RibbonHitTest.ApplicationButton) res.State |= ObjectState.Hot;
			if(PressedObject.HitTest == RibbonHitTest.ApplicationButton) res.State |= ObjectState.Pressed;
			if(IsApplicationMenuOpened) res.State |= ObjectState.Pressed;
			res.TextBounds = ApplicationButtonTextBounds;
			return res;
		}
		protected virtual Size CalcApplicationButtonSize() {
			if(!IsAllowApplicationButton) return Size.Empty;
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) {
				return CalcOffice2010ApplicationButtonSize();
			}
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, new RibbonApplicationButtonPainter(), ApplicationButton).Size;
		}
		public virtual Bitmap GetApplicationIcon() {
			object allowAppIcon = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptAllowDrawApplicationIcon];
			if(allowAppIcon != null && ((bool)allowAppIcon) == false && Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007)
				return null;
			if(Ribbon.ApplicationIcon == null) return RibbonControl.GetDefaultApplicationIcon(Ribbon);
			return Ribbon.ApplicationIcon;
		}
		protected virtual Size CalcOffice2010ApplicationButtonSize() {
			Rectangle clientRect = Rectangle.Empty;
			ApplicationButtonTextBounds = Rectangle.Empty;
			if(!string.IsNullOrEmpty(GetApplicationButtonText())) {
				clientRect.Size = PaintAppearance.PageHeader.CalcTextSize(GInfo.Cache, GetApplicationButtonText(), 0).ToSize();
				ApplicationButtonTextBounds = new Rectangle(Point.Empty, clientRect.Size);
			}
			if(clientRect.Size == Size.Empty)
				clientRect.Size = GetApplicationIcon() == null? Size.Empty: GetApplicationIcon().Size;
			SkinElementInfo info = ApplicationButton.GetInfo();
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info, clientRect).Size;
		}
		protected internal virtual string GetApplicationButtonText() {
			if(GetRibbonStyle() == RibbonControlStyle.OfficeUniversal) {
				if(!string.IsNullOrEmpty(Ribbon.ApplicationButtonText)) return Ribbon.ApplicationButtonText;
				if(Ribbon.ApplicationIcon != null) return string.Empty;
				return "File";
			}
			if(GetRibbonStyle() == RibbonControlStyle.Office2007) return string.Empty;
			return Ribbon.ApplicationButtonText;
		}
		protected internal override int LargeButtonTextLinesCount { get { return CreateGroupLayoutCalculator(null).CalcLargeButtonTextLinesCount(); } }
		protected internal virtual RibbonPageGroupLayoutCalculator CreateGroupLayoutCalculator(RibbonPageGroupViewInfo pageInfo) {
			if(Ribbon.IsOfficeTablet)
				return new RibbonPageGroupTabletStyleLayoutCalculator(pageInfo);
			if(Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return new RibbonPageGroupMacStyleLayoutCalculator(pageInfo);
			return new RibbonPageGroupComplexLayoutCalculator(pageInfo);
		}
		bool IsRibbonFormWithOffice2007ApplicationButton {
			get { return (Form != null && Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007 && Ribbon.ShowApplicationButton != DefaultBoolean.False); }
		}
		protected internal bool IsEmptyTabHeader {
			get {
				if(GetShowPageHeaders() || IsRibbonFormWithOffice2007ApplicationButton) return false;
				Form parentForm = Ribbon.FindForm();
				if(parentForm == null) return true;
				if(Ribbon.IsOffice2010LikeStyle && Ribbon.ShowApplicationButton != DefaultBoolean.False)
					return false;
				if(parentForm.IsMdiContainer && Ribbon.AllowMdiChildButtons)
					return false;
				if(Header.SupportHeaderItemsInCaption && Ribbon.PageHeaderItemLinks.HasVisibleItems)
					return true;
				return !Ribbon.PageHeaderItemLinks.HasVisibleItems;
			}
		}
		protected internal virtual int CalcTabHeaderHeight() {
			if(IsEmptyTabHeader) return 0;
			bool graphicsAdded = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				graphicsAdded = true;
			}
			try {
				Rectangle client = new Rectangle(0, 0, 0, Header.CalcBestHeight());
				client = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(GetTabHeaderPageElement()), client);
				int res = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(GetElement(RibbonSkins.SkinTabHeaderBackground)), client).Height + PanelYOffsetCore;
				int itemsHeight = Math.Max(EditorHeight, ButtonHeight);
				if(GetRibbonStyle() == RibbonControlStyle.TabletOffice && (GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above || GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Default)) {
					itemsHeight = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, Toolbar.GetToolbarInfo(), new Rectangle(0, 0, 100, itemsHeight)).Height;
				}
				res = Math.Max(res, itemsHeight);
				return res;
			}
			finally { 
				if(graphicsAdded)
					GInfo.ReleaseGraphics();
			}
		}
		RibbonSkinProvider ribbonSkinProvider;
		protected RibbonSkinProvider RibbonSkinProvider {
			get {
				if(ribbonSkinProvider == null)
					ribbonSkinProvider = new RibbonSkinProvider(this);
				return ribbonSkinProvider;
			}
		}
		public override ISkinProvider Provider { 
			get {
				if(Ribbon.OptionsTouch.AffectOnlyRibbon)
					return RibbonSkinProvider;
				return Ribbon.GetController().LookAndFeel; 
			} 
		}
		public override Control OwnerControl { get { return Ribbon; } }
		protected virtual RibbonCaptionViewInfo CreateCaptionInfo() { return new RibbonCaptionViewInfo(this); }
		protected virtual RibbonApplicationButtonInfo CreateApplicationButtonInfo() { return new RibbonApplicationButtonInfo(this); }
		protected virtual RibbonQuickAccessToolbarViewInfo CreateToolbarInfo() {
			return new RibbonQuickAccessToolbarViewInfo(Ribbon.Toolbar);
		}
		public override void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.None) return;
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) {
				Invalidate(ApplicationButton.Bounds);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.HeaderPanel) {
				Invalidate(Header.Bounds);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.PageHeader) {
				Header.Invalidate(hitInfo.Page);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.FullScreenModeBar) {
				Ribbon.Invalidate(FullScreenModeBarBounds);
				return;
			}
			if(hitInfo.InPanel) Panel.Invalidate(hitInfo);
			if(hitInfo.InToolbar) {
				Invalidate(hitInfo.Toolbar.Bounds);
			}
			if(hitInfo.HitTest == RibbonHitTest.PanelLeftScroll) Invalidate(Panel.LeftScrollButtonBounds);
			else if(hitInfo.HitTest == RibbonHitTest.PanelRightScroll) Invalidate(Panel.RightScrollButtonBounds);
			else if(hitInfo.HitTest == RibbonHitTest.PageHeaderLeftScroll) Invalidate(Header.LeftScrollBounds);
			else if(hitInfo.HitTest == RibbonHitTest.PageHeaderRightScroll) Invalidate(Header.RightScrollBounds);
		}
		public override void Invalidate(RibbonItemViewInfo itemInfo) {
			Panel.Invalidate(itemInfo);
		}
		public virtual void Invalidate(RibbonPageGroup pageGroup) {
			Panel.Invalidate(pageGroup);
		}
		public virtual SkinElementInfo GetPanelBorderInfo() {
			SkinElement elem = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinContextTabPanelBorder];
			if(elem == null)
				return null;
			SkinElementInfo info = new SkinElementInfo(elem, Panel.Bounds);
			if(!Ribbon.Enabled) {
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			}
			return info;
		}
		protected virtual bool ShouldUseTabPanelNoBorder() {
			return !ShouldDrawGroupCaption && RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelNoGroupBorders] != null;
		}
		public virtual SkinElementInfo GetPanelInfo() {
			bool contextTabPanel = false; 
			string panelInfoString = ShouldUseTabPanelNoBorder() ? RibbonSkins.SkinTabPanelNoGroupBorders : RibbonSkins.SkinTabPanel;
			RibbonPageCategory cat = Ribbon.SelectedPage != null? Ribbon.SelectedPage.Category: null;
			if(cat != null && !cat.IsDefaultColor) {
				panelInfoString = RibbonSkins.SkinContextTabPanel;
				if(!ShouldDrawGroupBorder && RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinContextTabPanelNoGroupBorders] != null)
					panelInfoString = RibbonSkins.SkinContextTabPanelNoGroupBorders;
				contextTabPanel = true;
			}
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[panelInfoString], Panel.Bounds);
			if(contextTabPanel) {
				info.BackAppearance = (AppearanceObject)info.BackAppearance.Clone();
				info.BackAppearance.BackColor = Ribbon.SelectedPage.Category.Color;
			}
			if(!Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			info.RightToLeft = IsRightToLeft;
			return info;
		}
		protected internal int PanelYOffset {
			get {
				if(GetIsMinimized()) return 0;
				return PanelYOffsetCore;
			}
		}
		protected internal int PanelYOffsetCore {
			get {
				return RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptTabHeaderDownGrow);
			}
		}
		public virtual RibbonPage SelectedPage { get { return Ribbon != null ? Ribbon.SelectedPage : null; } }
		public virtual int TabHeaderHeight { get { return tabHeaderHeight; } } 
		public virtual RibbonPageHeaderViewInfo Header { get { return header; } }
		public RibbonPanelViewInfo Panel { get { return panel; } }
		public RibbonQuickAccessToolbarViewInfo Toolbar { get { return toolbar; } }
		public RibbonCaptionViewInfo Caption { get { return caption; } }
		public bool IsApplicationMenuOpened {
			get {
				if(Ribbon.ApplicationButtonPopupControl != null)
					return Ribbon.ApplicationButtonPopupControl.Visible;
				else if(Ribbon.ApplicationButtonControl != null)
					return Ribbon.ApplicationButtonContentControl.ContentVisible;
				return false;
			}
		}
		public RibbonApplicationButtonInfo ApplicationButton { get { return applicationButton; } }
		public Rectangle ToolbarBounds {
			get {
				if(Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Hidden) return Rectangle.Empty;
				return Ribbon.ViewInfo.Toolbar.ContentBackgroundBounds;
			}
		}
		Rectangle applicationButtonTextBounds;
		public Rectangle ApplicationButtonTextBounds {
			get { return applicationButtonTextBounds; }
			set { applicationButtonTextBounds = value; }
		}
		public Rectangle ApplicationButtonHitBounds {
			get {
				Rectangle res = ApplicationButton.Bounds;
				if(res == Rectangle.Empty || Caption.Bounds.IsEmpty) return res;
				if(Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007 && !IsRightToLeft)
					res.Location = Caption.Bounds.Location;
				res.Width = ApplicationButton.Bounds.Right - res.X;
				res.Height = ApplicationButton.Bounds.Bottom - res.Y;
				if(res.Width > 0 && res.Height > 0) return res;
				return ApplicationButton.Bounds;
			}
		}
		public virtual Rectangle FullScreenModeBarBounds {
			get {
				if(Form == null || !IsFullScreenModeActive) return Rectangle.Empty;
				return new Rectangle(Point.Empty, new Size(Ribbon.Width, FullScreenBarHeight));
			}
		}
		public virtual Rectangle FullScreenBarGlyphBounds { get; set; }
		public virtual Color FullScreenBarGlyphContentColor {
			get { return LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.ControlText); }
		}
		public RibbonControl Ribbon { get { return ribbon; } }
		protected internal int CalcMinWidth() {
			int width = Panel.CalcMinWidth() + Panel.DefaultIndentBetweenGroups * 2;
			return width;
		}
		protected internal virtual int CalcMinHeight() {
			if(!IsAllowDisplayRibbon) return IsAllowCaption ? Caption.CalcCaptionHeight() : 0;
			if(IsFullScreenModeActive) return FullScreenBarHeight;
			if(IsPopupFullScreenModeActive) return TabHeaderHeight + (IsAllowCaption ? Caption.CalcCaptionHeight() : 0);
			int res = TabHeaderHeight;
			if(IsAllowCaption)
				res += Caption.CalcCaptionHeight();
			if((GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Below || !IsAllowCaption) && GetRibbonStyle() != RibbonControlStyle.TabletOffice)
				res += Toolbar.Bounds.Height;
			if((Ribbon.TotalPageCategory.Pages.HasVisibleItems && !GetIsMinimized()) || (Ribbon.TotalPageCategory.Pages.Count > 0 && Ribbon.IsDesignMode)) res += Panel.Bounds.Height;
			if(!IsEmptyTabHeader && !GetIsMinimized() && Panel.Bounds.Height > 0) res -= PanelYOffset;
			return res;
		}
		protected internal virtual int FullScreenBarHeight { get { return 28; } }
		public virtual RibbonQuickAccessToolbarLocation GetToolbarLocation() {
			RibbonQuickAccessToolbarLocation res = Ribbon.ToolbarLocation;
			if(res == RibbonQuickAccessToolbarLocation.Default) res = RibbonQuickAccessToolbarLocation.Above;
			return res;
		}
		protected virtual RibbonPage GetFirstVisiblePage() {
			return Ribbon.PageCategories.TotalCategory.GetFirstVisiblePage();
		}
		bool prevGlass = false;
		public override void CalcViewInfo(Rectangle bounds) {
			Clear();
			GInfo.AddGraphics(null);
			try {
				if(prevGlass != Caption.IsGlassForm) UpdatePaintAppearance();
				this.prevGlass = Caption.IsGlassForm;
				UpdateInternalVisiblePages();
				FirstVisiblePage = GetFirstVisiblePage();
				CalcConstants();
				Bounds = bounds;
				ContentBounds = CalcContentBounds();
				CalcCaption();
				if(!IsAllowDisplayRibbon) {
					this.IsReady = true;
					return;
				}
				CalcApplicationButton();
				ContentBounds = CalcContentBounds(); 
				CalcPageHeader();
				if(GetRibbonStyle() == RibbonControlStyle.TabletOffice)
					CalcToolbarAtTopLocation();
				CalcApplicationButton();
				FullScreenBarGlyphBounds = CalcFullScreenBarGlyphBounds();
				UpdateCaptionAndToolbar();
				CalcPanel();
				if(GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Below) CalcToolbarAtBelowLocation();
				UpdateItemHotInfo(HotObject);
				if(Ribbon.IsKeyboardActive) {
					Ribbon.NavigatableObjects = null;
					Ribbon.NavigatableObjectList = null;
				}
				RemoveInvisibleAnimatedItems();
				CheckViewInfoRTL();
				Caption.RecalcTextBounds();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			this.IsReady = true;
		}
		#region RTL
		internal virtual void CheckViewInfoRTL() {
			if(!IsRightToLeft) return;
			CheckHeaderRTL();
			CheckApplicationButtonRTL();
			CheckPagesRTL();
			CheckCategoriesRTL();
		}
		internal virtual void CheckPagesRTL() {
			if(!IsRightToLeft) return;
			foreach(RibbonPageViewInfo page in Header.Pages) {
				page.Bounds = BarUtilites.ConvertBoundsToRTL(page.Bounds, Ribbon.Bounds);
				page.ContentBounds = BarUtilites.ConvertBoundsToRTL(page.ContentBounds, Ribbon.Bounds);
				page.ImageBounds = BarUtilites.ConvertBoundsToRTL(page.ImageBounds, Ribbon.Bounds);
				page.TextBounds = BarUtilites.ConvertBoundsToRTL(page.TextBounds, Ribbon.Bounds);
			}
		}
		internal virtual void CheckCategoriesRTL() {
			if(!IsRightToLeft) return;
			foreach(RibbonPageCategoryViewInfo category in Header.PageCategories) {
				category.UpdateBounds(BarUtilites.ConvertBoundsToRTL(category.Bounds, Ribbon.Bounds));
			}
		}
		internal virtual void CheckApplicationButtonRTL() {
			if(!IsRightToLeft) return;
			ApplicationButton.Bounds = BarUtilites.ConvertBoundsToRTL(ApplicationButton.Bounds, Ribbon.Bounds);
			ApplicationButtonTextBounds = BarUtilites.ConvertBoundsToRTL(ApplicationButtonTextBounds, Ribbon.Bounds);
		}
		internal virtual void CheckHeaderRTL() {
			if(!IsRightToLeft) return;
			Header.PageHeaderBounds = BarUtilites.ConvertBoundsToRTL(Header.PageHeaderBounds, Ribbon.Bounds);
			Header.CalcHeaderItemsLayout(BarUtilites.ConvertBoundsToRTL(Header.PageHeaderItemsBounds, Ribbon.Bounds));
		}
		internal virtual void CheckToolbarRTL() {
			if(!IsRightToLeft) return;
			Rectangle ownerRect = IsToolbarCaption ? Caption.ContentBounds : Ribbon.Bounds;
			Toolbar.Bounds = BarUtilites.ConvertBoundsToRTL(Toolbar.Bounds, ownerRect);
			Toolbar.ContentBackgroundBounds = BarUtilites.ConvertBoundsToRTL(Toolbar.ContentBackgroundBounds, ownerRect);
			Toolbar.ContentBounds = BarUtilites.ConvertBoundsToRTL(Toolbar.ContentBounds, ownerRect);
			Toolbar.ItemsContentBackgroundBounds = BarUtilites.ConvertBoundsToRTL(Toolbar.ItemsContentBackgroundBounds, ownerRect);
			Toolbar.ItemsContentBounds = BarUtilites.ConvertBoundsToRTL(Toolbar.ItemsContentBounds, ownerRect);
			Toolbar.OuterBounds = BarUtilites.ConvertBoundsToRTL(Toolbar.OuterBounds, ownerRect);
			Toolbar.UpdateItemsLayout(Toolbar.ContentBounds, 0, Toolbar.VisibleButtonCount);
		}
		protected internal virtual bool IsToolbarCaption {
			get {
				if(GetRibbonStyle() == RibbonControlStyle.TabletOffice) return false;
				if(GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Below) return false;
				if(Ribbon.ViewInfo.Ribbon.RibbonForm == null) return false; 
				return true;
			}
		}
		#endregion
		void UpdateInternalVisiblePages() {
			if(Ribbon.AutoHideEmptyItems && !Ribbon.IsDesignMode) {
				CheckRibbonVisibility(Ribbon);
				if(Ribbon.MergedRibbon != null) CheckRibbonVisibility(Ribbon.MergedRibbon);
				if(Ribbon.SelectedPage == null || !Ribbon.SelectedPage.ActualVisible ) {
					Ribbon.UpdateSelectedPageWithoutViewInfo();
				}
			}
		}
		void CheckRibbonVisibility(RibbonControl ribbon) {
			CheckVisibleCategories(ribbon.PageCategories);
			CheckVisibleCategories(ribbon.MergedCategories);
			CheckVisiblePages(ribbon.DefaultPageCategory);
		}
		void CheckVisibleCategories(RibbonPageCategoryCollection collection) {
			foreach(RibbonPageCategory category in collection) CheckVisiblePages(category);
		}
		void CheckVisiblePages(RibbonPageCategory category) {
			foreach(RibbonPage page in category.Pages) CheckVisibleGroups(page);
			foreach(RibbonPage mergedPage in category.MergedPages) CheckVisibleGroups(mergedPage);
			category.InternalVisible = category.Pages.HasVisibleItems || category.MergedPages.HasVisibleItems;
		}
		void CheckVisibleGroups(RibbonPage page) {
			foreach(RibbonPageGroup group in page.Groups) group.InternalVisible = group.ItemLinks.HasVisibleItems;
			foreach(RibbonPageGroup mergedGroup in page.MergedGroups) mergedGroup.InternalVisible = mergedGroup.ItemLinks.HasVisibleItems;
			page.InternalVisible = page.Groups.HasVisibleItems || page.MergedGroups.HasVisibleItems;
		}
		protected internal virtual Rectangle CalcFullScreenBarGlyphBounds() {
			int yOffset = 0;
			if(Ribbon.ViewInfo != null && Ribbon.ViewInfo.Form != null) {
				yOffset = Math.Abs(Ribbon.ViewInfo.Form.Location.Y);
			}
			Point pt = new Point(Bounds.Right - FullScreenGlyphRightPadding - FullScreenGlyphImage.Width, yOffset + (Bounds.Height - yOffset) / 2 - FullScreenGlyphImage.Height / 2);
			return new Rectangle(pt, FullScreenGlyphImage.Size);
		}
		protected virtual void RemoveInvisibleAnimatedItems() {
			if(IsDesignMode) return;
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				EditorAnimationInfo info = XtraAnimator.Current.Animations[i] as EditorAnimationInfo;
				if(info == null || info.AnimatedObject != Ribbon) continue;
				if(!HasAnimatedItem(info) && info.Link is BarEditItemLink) {
					HasAnimatedItem(info);
					XtraAnimator.RemoveObject(Ribbon, info.Link);
				}
			}	
		}
		protected virtual bool HasAnimatedItem(EditorAnimationInfo info) {
			if(Toolbar != null && HasAnimatedItem(info, Toolbar.Items)) return true;
			if(Header != null && HasAnimatedItem(info, Header.PageHeaderItems)) return true;
			if(Panel == null) return false;
			for(int j = 0; j < Panel.Groups.Count; j++) {
				if(HasAnimatedItem(info, Panel.Groups[j].Items)) return true;
			}
			return false;
		}
		public bool FormMinimized { get { return Form != null && Form.WindowState == FormWindowState.Minimized; } }
		public bool FormMaximized { get { return Form != null && Form.WindowState == FormWindowState.Maximized; } }
		public RibbonForm Form {
			get {
				RibbonForm form = (Ribbon != null ? Ribbon.FindForm() : null) as RibbonForm;
				return form != null && form.RibbonFormStyle == RibbonFormStyle.Ribbon && form.Ribbon == Ribbon ? form : null;
			}
		}
		public virtual bool IsAllowCaption {
			get {
				if(Form == null || (Form.MdiParent != null && (Form.FormBorderStyle == FormBorderStyle.None || Form.WindowState == FormWindowState.Maximized))) return false;
				if(Ribbon.Parent != Form || Ribbon.Location != Form.DisplayRectangle.Location) return false;
				return true;
			}
		}
		public virtual bool IsAllowDisplayRibbon {
			get {
				RibbonControl mdiParentRibbon = Ribbon.GetMdiParentRibbon();
				if(Ribbon.IsMerged || (Ribbon.IsMdiChildRibbon && mdiParentRibbon != null && mdiParentRibbon.MdiMergeStyle == RibbonMdiMergeStyle.Always)) return false;
				RibbonForm form = Form;
				if(form == null)
					return true;
				if(Ribbon.ApplicationButtonContentControl != null && Ribbon.ApplicationButtonContentControl.ContentVisible) return true;
				if(form.RibbonVisibility == RibbonVisibility.Hidden) return false;
				if(form.RibbonVisibility == RibbonVisibility.Visible) return true;
				if(Form.WindowState == FormWindowState.Normal) {
					if(form.ClientSize.Width < RibbonRequiredFormSize.Width) return false;
					if(form.ClientSize.Height < RibbonRequiredFormSize.Height) return false;
				}
				return true;
			}
		}
		public virtual bool CalcToolbarHitInfo(RibbonHitInfo res) {
			if(!FormMaximized || GetToolbarLocation() != RibbonQuickAccessToolbarLocation.Above || res.HitPoint.Y > Toolbar.Bounds.Y) return res.ContainsSet(Toolbar.Bounds, RibbonHitTest.Toolbar);
			return Toolbar.Bounds.X < res.HitPoint.X && Toolbar.Bounds.Right > res.HitPoint.X;
		}
		public override RibbonHitInfo CalcHitInfo(Point hitPoint) {
			RibbonHitInfo res = CreateHitInfo();
			if (IsManagerDestroing) return res;
			res.HitPoint = hitPoint;
			if(res.ContainsSet(FullScreenModeBarBounds, RibbonHitTest.FullScreenModeBar))
				return res;
			if(res.ContainsSet(ApplicationButtonHitBounds, RibbonHitTest.ApplicationButton)) return res;
			if(IsOfficeTablet && CalcToolbarHitInfo(res)) {
				res.SetHitTest(RibbonHitTest.Toolbar);
				res.Toolbar = Toolbar;
				Toolbar.CalcHitInfo(res);
				return res;
			}
			if(res.ContainsSet(Header.Bounds, RibbonHitTest.HeaderPanel)) {
				Header.CalcHitInfo(res);
				return res;
			}
			if(res.ContainsSet(Panel.Bounds, RibbonHitTest.Panel)) {
				Panel.CalcHitInfo(res);
				return res;
			}
			if(!IsOfficeTablet && CalcToolbarHitInfo(res)) {
				res.SetHitTest(RibbonHitTest.Toolbar);
				res.Toolbar = Toolbar;
				Toolbar.CalcHitInfo(res);
				return res;
			}
			foreach(RibbonPageCategoryViewInfo pc in Header.PageCategories) {
				if(res.ContainsSet(pc.UpperBounds, RibbonHitTest.PageHeaderCategory) || res.ContainsSet(pc.LowerBounds, RibbonHitTest.PageHeaderCategory)) {
					res.PageCategoryInfo = pc;
					res.PageCategory = pc.Category;
					return res;
				}
			}
			if(res.ContainsSet(Caption.Bounds, RibbonHitTest.FormCaption)) {
				Caption.CalcHitInfo(res);
				return res;
			}
			return res;
		}
		public override RibbonBarManager Manager { get { return Ribbon.Manager; } }
		protected internal virtual bool IsAllowGroupMinimize(RibbonPageGroupViewInfo groupInfo) {
			return true;
		}
		protected internal override RibbonItemViewInfo CreateItemViewInfo(IRibbonItem item) {
			RibbonItemViewInfo vi = Manager.CreateItemViewInfo(this, item);
			BarEditItemLink link = item as BarEditItemLink;
			if(link == null || IsDesignMode) return vi;
			BarAnimatedItemsHelper.UpdateAnimatedItem(Ribbon, AnimationInvoker, vi as RibbonEditItemViewInfo);
			return vi;
		}
		CustomAnimationInvoker animationInvoker = null;
		internal CustomAnimationInvoker AnimationInvoker {
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(Ribbon.OnAnimation);
				return animationInvoker;
			}
		}
		protected virtual void CalcPanel() {
			if(GetIsMinimized()) return;
			Panel.Page = Ribbon.SelectedPage;
			Panel.CalcPanelContent();
			AllowCachedItemInfo = true;
			this.pnlHeight = Panel.Bounds.Height;
		}
		protected virtual void CalcPageHeader() {
			Header.CalcPageHeader();
		}
		protected virtual void CalcCaption() {
			if(IsAllowCaption)
				Caption.CalcCaption();
			else
				CalcToolbarAtTopLocation();
		}
		protected virtual void UpdateCaptionAndToolbar() {
			UpdateToolbar();
			UpdateCaptionTextBounds();
		}
		protected virtual void UpdateToolbar() {
			if(IsAllowCaption)
				Caption.UpdateToolbar();
		}
		protected virtual void UpdateCaptionTextBounds() {
			if(IsAllowCaption)
				Caption.UpdateCaptionTextBounds();
		}
		protected internal virtual int UniversalOfficeApplicationButtonLeftIndent { get { return 16; } }
		protected internal virtual int Office2010ApplicationButtonLeftIndent {
			get {
				object leftIndent = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptOffice2010ApplicationButtonLeftIndent];
				return leftIndent == null ? 0 : (int)leftIndent;
			}
		}
		protected internal virtual int Office2010ApplicationButtonRightIndent {
			get {
				object rightIndent = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptOffice2010ApplicationButtonRightIndent];
				return rightIndent == null ? 0 : (int)rightIndent;
			}
		}
		protected internal virtual bool CanUseFullScreenMode {
			get {
				if(Form == null || Form.IsMdiChild) return false;
				return Ribbon.RibbonStyle == RibbonControlStyle.Office2013 && (Ribbon.ShowFullScreenButton == DefaultBoolean.True || Ribbon.ShowFullScreenButton == DefaultBoolean.Default);
			}
		}
		protected virtual Rectangle CalcApplicationButtonCore() {
			Rectangle res = Rectangle.Empty;
			switch(GetRibbonStyle()) {
				case RibbonControlStyle.Office2007:
					res = new Rectangle(Caption.ClientBounds.Location, ApplicationButtonSize);
					res.X += ApplicationButtonLeftIndent;
					res.Y = Caption.ClientBounds.Y + Caption.ClientBounds.Height - res.Height / 2;
					res.Offset(ApplicationButton.GetInfo().Element.Offset.Offset);
					ApplicationButtonTextBounds = Rectangle.Empty;
					break;
				case RibbonControlStyle.OfficeUniversal:
					res = new Rectangle(ContentBounds.X + UniversalOfficeApplicationButtonLeftIndent, Header.PageHeaderBounds.Y, ApplicationButtonSize.Width, Header.PageHeaderBounds.Height);
					break;
				default:
					res = new Rectangle(ContentBounds.X + Office2010ApplicationButtonLeftIndent, Header.PageHeaderBounds.Y, ApplicationButtonSize.Width, Header.PageHeaderBounds.Height);
					break;
			}
			return res;
		}
		protected virtual void CalcApplicationButton() {
			ApplicationButton.Bounds = Rectangle.Empty;
			if(ApplicationButtonSize.IsEmpty) return;
			ApplicationButton.Bounds = CalcApplicationButtonCore();
			ApplicationButtonTextBounds = CalcApplicationButtonTextBounds();
		}
		protected virtual Rectangle CalcApplicationButtonTextBounds() {
			Rectangle res = new Rectangle();
			if(Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007)
				return res;
			SkinElementInfo info = ApplicationButton.GetInfo();
			info.Bounds = ApplicationButton.Bounds;
			res = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info);
			if(ApplicationButtonTextBounds.Width >= res.Width)
				return res;
			return new Rectangle(res.X + (res.Width - ApplicationButtonTextBounds.Width) / 2, res.Y, ApplicationButtonTextBounds.Width, res.Height);
		}
		protected virtual void CalcToolbarAtBelowLocation() {
			if(!IsAllowDisplayRibbon) return;
			if(GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Hidden || GetRibbonStyle() == RibbonControlStyle.TabletOffice) return;
			Rectangle rect = new Rectangle(ContentBounds.X, ContentBounds.Y, ContentBounds.Width, Toolbar.CalcMinHeight());
			if(!Panel.Bounds.IsEmpty)
				rect.Y = Panel.Bounds.Bottom;
			else
				rect.Y = Header.Bounds.Bottom;
			Toolbar.CalcViewInfo(rect);
		}
		protected virtual void CalcToolbarAtTopLocation() {
			if(!IsAllowDisplayRibbon) return;
			Rectangle rect = new Rectangle(ContentBounds.X, ContentBounds.Y, ContentBounds.Width, Toolbar.CalcMinHeight());
			if(GetRibbonStyle() == RibbonControlStyle.TabletOffice)
				rect = new Rectangle(ContentBounds.X, Header.Bounds.Y, Header.PageHeaderBounds.X - ContentBounds.X, Header.Bounds.Height);
			if(GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Hidden) {
				rect = Rectangle.Empty;
			}
			Toolbar.CalcViewInfo(rect);
		}
		protected virtual RibbonPageHeaderViewInfo CreateHeaderInfo() { return new RibbonPageHeaderViewInfo(this); }
		protected virtual RibbonPanelViewInfo CreatePanelInfo() { return new RibbonPanelViewInfo(this); }
		protected internal override void Clear() {
			base.Clear();
			this.IsReady = false;
			Header.Clear();
			Panel.Clear();
			Toolbar.ClearItemsViewInfo();
			Caption.Bounds = Rectangle.Empty;
			Header.Bounds = Rectangle.Empty;
			Toolbar.Bounds = Rectangle.Empty;
			Toolbar.ContentBounds = Rectangle.Empty;
			ApplicationButton.Bounds = Rectangle.Empty;
		}
		public override BarItemLink GetLinkByPoint(Point pt, bool enterButtonGroup) {
			if(GetToolbarLocation() != RibbonQuickAccessToolbarLocation.Hidden) {
				if(Toolbar.Bounds.Contains(pt)) return Toolbar.GetLinkByPoint(pt);
			}
			if(Header.Bounds.Contains(pt)) return Header.GetLinkByPoint(pt);
			if(Panel.Bounds.Contains(pt)) return Panel.GetLinkByPoint(pt, enterButtonGroup);
			return null;
		}
		public Image FullScreenGlyphImage {
			get { return Ribbon.FullScreenBarGlyph.GetImage(FullScreenBarGlyphContentColor); }
		}
		protected internal virtual SkinElementInfo GetAppButtonContainerInfo(Rectangle bounds) {
			return new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinApplicationButtonContainerControl], bounds);
		}
		public static readonly int FullScreenGlyphRightPadding = 23;
		protected internal virtual bool IsExpandItemInRibbonPanel(RibbonItemViewInfo itemInfo) {
			return Ribbon.IsExpandButtonInPanel && itemInfo is RibbonExpandCollapseItemViewInfo;
		}
	}
	public class RibbonPanelItemsViewInfo {
		RibbonViewInfo viewInfo;
		RibbonHeaderPainter panelItemsPainter;
		public RibbonPanelItemsViewInfo(RibbonViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected RibbonViewInfo ViewInfo { get { return viewInfo; } }
		protected RibbonControl Ribbon { get { return viewInfo.Ribbon; } }
		public RibbonHeaderPainter PanelItemsPainter {
			get {
				if(panelItemsPainter == null) panelItemsPainter = CreatePanelItemsPainter();
				return panelItemsPainter;
			}
		}
		RibbonItemViewInfo expandItemInfo;
		protected internal RibbonItemViewInfo ExpandItemInfo { get { return expandItemInfo; } }
		protected virtual RibbonHeaderPainter CreatePanelItemsPainter() { return new RibbonHeaderPainter(); }
		protected virtual void CreateExpandItemViewInfo() {
			try {
				if(!Ribbon.GetShowExpandCollapseButton() || Ribbon.IsExpandCollapseItemInPageHeaderItemLinks || ViewInfo.IsEmptyTabHeader)
					return;
				expandItemInfo = Ribbon.Manager.CreateItemViewInfo(ViewInfo, Ribbon.ExpandCollapseItemLink);
				Ribbon.ExpandCollapseItemLink.LinkViewInfo.UpdateLinkState();
			}
			catch { }
		}
		protected virtual Size GetHeaderItemsBestWidth(RibbonItemViewInfo itemInfo) {
			Size size = itemInfo.CalcBestSize();
			size.Height = CalcItemHeight(size.Height);
			return size;
		}
		protected int CalcItemHeight(int height) {
			RibbonMinimizedControl minimizedRibbon = Ribbon as RibbonMinimizedControl;
			if(minimizedRibbon != null && minimizedRibbon.SourceRibbon != null && minimizedRibbon.SourceRibbon.ViewInfo.Header != null)
				return Math.Min(minimizedRibbon.SourceRibbon.ViewInfo.Header.PageHeaderItemHeight, height);
			if(ViewInfo.Header != null)
				return Math.Min(ViewInfo.Header.PageHeaderItemHeight, height);
			return height;
		}
		public virtual void CalcPanelItems() {
			if(!Ribbon.IsExpandButtonInPanel) {
				expandItemInfo = null;
				return;
			}
			CreateExpandItemViewInfo();
			CalcExpandItemBounds();
		}
		protected void CalcExpandItemBounds() {
			if(expandItemInfo == null) return;
			Point loc = GetRibbonBottomRightPoint();
			Point margins = GetPanelContentMargins();
			Size size = GetHeaderItemsBestWidth(expandItemInfo);
			Point offset = GetButtonOffset();
			int x = ViewInfo.IsRightToLeft ? loc.X + margins.X - offset.X : loc.X - size.Width - margins.X + offset.X;
			int y = loc.Y - size.Height - margins.Y + offset.Y;
			expandItemInfo.Bounds = new Rectangle(new Point(x, y), size);
		}
		protected internal Point GetPanelContentMargins() {
			SkinElement elem = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinTabPanel];
			if(elem == null) return Point.Empty;
			return new Point(elem.ContentMargins.Right, elem.ContentMargins.Bottom);
		}
		protected Point GetRibbonBottomRightPoint() {
			if(Ribbon is RibbonMinimizedControl) {
				MinimizedRibbonPopupForm form = Ribbon.FindForm() as MinimizedRibbonPopupForm;
				if(form != null){
					int right = ViewInfo.IsRightToLeft ? 0 : form.Width;
					return new Point(right, form.Height);
				}
			}
			int x = ViewInfo.IsRightToLeft ? ViewInfo.ContentBounds.X : ViewInfo.ContentBounds.Right;
			int y = ViewInfo.ContentBounds.Bottom;
			return new Point(x, y);
		}
		protected internal virtual bool CalcHitInfo(RibbonHitInfo hitInfo) {
			if(expandItemInfo != null && expandItemInfo.Bounds.Contains(hitInfo.HitPoint)) {
				if(hitInfo.ContainsSet(expandItemInfo.Bounds, RibbonHitTest.Item)) {
					expandItemInfo.CalcHitInfo(hitInfo);
					return true;
				}
			}
			return false;
		}
		protected internal virtual void UpdateSystemLinkGlyph(BarItemLink link, ObjectState state) {
			if(link == Ribbon.ExpandCollapseItemLink) {
				if(!Ribbon.GetShowExpandCollapseButton() || expandItemInfo == null)
					return;
				expandItemInfo.ExtraGlyph = Ribbon.Minimized ? GetPinGlyphImage(state) : GetCollapseButtonGlyphImage(state);
			}
		}
		protected internal Point GetButtonOffset() {
			SkinElement elem = null;
			if(Ribbon.Minimized) {
				elem = BarSkins.GetSkin(ViewInfo.Provider)["PinButtonGlyph"];
				if(elem == null) elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinDockWindowButtons];
			}
			if(elem == null) elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinCollapseButtonGlyph];
			if(elem != null) return elem.Offset.Offset;
			return Point.Empty;
		}
		protected internal Image GetPinGlyphImage(ObjectState state) {
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)["PinButtonGlyph"];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			elem = GetDockSkinElement(state);
			if(elem == null || elem.Image == null) return Ribbon.GetController().GetBitmap("ExpandImage");
			return elem.Image.GetImages().Images[4];
		}
		protected SkinElement GetDockSkinElement(ObjectState state) {
			if(state == ObjectState.Pressed) return BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinDockWindowButtonsPressed];
			if(state == ObjectState.Hot) return BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinDockWindowButtonsHot];
			return BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinDockWindowButtons];
		}
		protected internal Image GetCollapseButtonGlyphImage(ObjectState state) {
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinCollapseButtonGlyph];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			return Ribbon.GetController().GetBitmap("CollapseImage");
		}
		protected Image GetGlyph(SkinGlyph glyph, ObjectState state) {
			ImageCollection coll = glyph.GetImages();
			if(state == ObjectState.Pressed && coll.Images.Count == 3) {
				return coll.Images[2];
			}
			if(state == ObjectState.Hot && coll.Images.Count >= 2) {
				return coll.Images[1];
			}
			return coll.Images[0];
		}
	}
	public class RibbonPageHeaderViewInfo {
		public static int HeaderToCaptionIndent = 16;
		public int DefaultPanelHeight = 96;
		public int DefaultTabOffset = 0;
		[Obsolete("Use CriticalMinWidth"), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public int CrititalMinWidth = 10;
		public int CriticalMinWidth = 10;
		public int DefaultPageHeaderMinWidth = 6;
		public int DefaultIndentBetweenPages = 4;
		public int DefaultIndentBetweenCategoryPages = 6;
		public int HeaderRightIndent = 27;
		public int MinPageHeaderWidth = 20;
		public int DefaultPageHeaderRightIndent = 8;
		RibbonHeaderPainter headerPainter;
		RibbonViewInfo viewInfo;
		Rectangle pageHeaderBounds, bounds;
		Rectangle leftScrollBounds, rightScrollBounds;
		RibbonPageViewInfoCollection pages;
		RibbonPageCategoryViewInfoCollection pageCategories;
		RibbonItemViewInfoCollection pageHeaderItems;
		int pageHeaderScrollOffset;
		float pageSeparateLineOpacity = 0.0f;
		Rectangle pageHeaderItemsBounds;
		bool showScrollButtons;
		public RibbonPageHeaderViewInfo(RibbonViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.bounds = this.pageHeaderBounds = Rectangle.Empty;
			this.pages = new RibbonPageViewInfoCollection();
			this.pageCategories = new RibbonPageCategoryViewInfoCollection();
			this.pageHeaderScrollOffset = 0;
			this.showScrollButtons = false;
		}
		public RibbonHeaderPainter HeaderPainter {
			get {
				if(headerPainter == null) headerPainter = CreateHeaderPainter();
				return headerPainter;
			}
		}
		public virtual int CalcBestHeight() {
			CreatePagesViewInfo();
			int height = ViewInfo.PaintAppearance.PageHeader.CalcDefaultTextSize(ViewInfo.GInfo.Graphics).Height;
			foreach(RibbonPageViewInfo vi in Pages) {
				height = Math.Max(vi.PaintAppearance.CalcDefaultTextSize(ViewInfo.GInfo.Graphics).Height, height);
			}
			foreach(RibbonPageViewInfo vi in Pages) {
				height = Math.Max(height, vi.GetImageSize().Height);
			}
			return height + 1;
		}
		bool? IsWindows8 = null;
		protected internal virtual bool IsGlassHeader {
			get {
				if(IsWindows8 == null)
					IsWindows8 = (bool?)NativeVista.IsWindows8;
				if(IsWindows8.Value)
					return false;
				return AllowGlassHeader;
			}
		}
		protected internal virtual bool AllowGlassHeader {
			get {
				if(Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007 || !Ribbon.AllowGlassTabHeader)
					return false;
				return ViewInfo.Form != null && ViewInfo.Form.IsGlassForm && (ViewInfo.Form.WindowState == FormWindowState.Normal || (ViewInfo.Form.WindowState == FormWindowState.Maximized && NativeVista.IsWindows7));
			}
		}
		public virtual SkinElementInfo GetContextPageInfo() {
			SkinElement element = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinContextTabHeaderPage];
			SkinElementInfo info = new SkinElementInfo(element);
			if(!Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetHeaderInfo() {
			string skinName = RibbonSkins.SkinTabHeaderBackground;
			Rectangle rect = Bounds;
			if(Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				skinName = RibbonSkins.SkinMacTabHeaderBackground;
			else if(IsGlassHeader) {
				skinName = RibbonSkins.SkinTabHeaderBackground2010;
				rect = new Rectangle(0, 0, Bounds.Width, Bounds.Bottom);
			}
			if(Ribbon.Pages.Count == 0 || (ViewInfo.GetIsMinimized() && IsFormInactive)) {
				rect.Height++;
			}
			SkinElement elem = RibbonSkins.GetSkin(ViewInfo.Provider)[skinName];
			if(elem == null)
				elem = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinTabHeaderBackground];
			SkinElementInfo res = new SkinElementInfo(elem, rect);
			if(ViewInfo.GetIsMinimized()) res.ImageIndex = 1;
			if(res.Element.Image != null && res.Element.Image.ImageCount > 2) {
				if(IsFormInactive) res.ImageIndex = 2;
			}
			if(!Ribbon.Enabled)
				res.Attributes = PaintHelper.RibbonDisabledAttributes;
			return res;
		}
		protected bool IsFormInactive {
			get { return ViewInfo.Form != null && ViewInfo.Form != Form.ActiveForm && !IsChildPopupsActive(); }
		}
		protected virtual bool IsChildPopupsActive() {
			Form activeForm = Form.ActiveForm;
			if(activeForm == null)
				return false;
			if(activeForm is RibbonMiniToolbarPopupForm) return true;
			if(Ribbon.MinimizedRibbonPopupForm == activeForm) return true;
			if(Ribbon.PopupGroupForm == activeForm) return true;
			if(Ribbon.PopupToolbar == activeForm) return true;
			foreach(IPopup popup in Ribbon.Manager.SelectionInfo.OpenedPopups) {
				if(activeForm == popup.PopupForm)
					return true;
			}
			return false;
		}
		protected internal virtual void Invalidate(RibbonPage page) {
			RibbonPageViewInfo info = Pages[page];
			if(info != null) ViewInfo.Invalidate(info.Bounds);
		}
		protected internal virtual void CalcHitInfo(RibbonHitInfo hitInfo) {
			if(hitInfo.ContainsSet(LeftScrollBounds, RibbonHitTest.PageHeaderLeftScroll)) return;
			if(hitInfo.ContainsSet(RightScrollBounds, RibbonHitTest.PageHeaderRightScroll)) return;
			if(hitInfo.ContainsSet(Ribbon.AutoHiddenPagesMenuItemLink.Bounds, RibbonHitTest.Item) && Ribbon.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal) {
				Ribbon.AutoHiddenPagesMenuItemLink.RibbonItemInfo.CalcHitInfo(hitInfo);
				return;
			}
			if(PageHeaderItemsBounds.Contains(hitInfo.HitPoint)) {
				foreach(RibbonItemViewInfo itemInfo in PageHeaderItems) {
					if(ViewInfo.IsExpandItemInRibbonPanel(itemInfo)) continue;
					if(hitInfo.ContainsSet(itemInfo.Bounds, RibbonHitTest.Item)) {
						itemInfo.CalcHitInfo(hitInfo);
						return;
					}
				}
			}
			if(PageHeaderBounds.Contains(hitInfo.HitPoint)) {
				for(int n = 0; n < Pages.Count; n++) {
					RibbonPageViewInfo page = Pages[n];
					if(hitInfo.ContainsSet(page.Bounds, RibbonHitTest.PageHeader)) {
						hitInfo.Page = page.Page;
						hitInfo.PageCategory = page.Category;
						hitInfo.PageCategoryInfo = page.CategoryInfo;
						return;
					}
				}
			}
			if(!Ribbon.ShowCategoryInCaption) return;
			for(int n = 0; n < PageCategories.Count; n++) { 
				RibbonPageCategoryViewInfo category = PageCategories[n];
				if(hitInfo.ContainsSet(category.Bounds, RibbonHitTest.PageHeaderCategory)) {
					hitInfo.PageCategory = category.Category;
					hitInfo.PageCategoryInfo = category;
					return;
				}
			}
		}
		protected virtual RibbonHeaderPainter CreateHeaderPainter() { return new RibbonHeaderPainter(); }
		public int PageHeaderItemHeight { get { return Bounds.Height - 2; } }
		public Rectangle LeftScrollBounds { get { return leftScrollBounds; } }
		public Rectangle RightScrollBounds { get { return rightScrollBounds; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ControlContentBounds { get { return ViewInfo.ContentBounds; } }
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		public int PageHeaderScrollOffset { get { return pageHeaderScrollOffset; } set { pageHeaderScrollOffset = value; } }
		public int GetPageHeaderMinWidth() { return Math.Max(DefaultPageHeaderMinWidth, Ribbon.PageHeaderMinWidth); }
		public RibbonControl Ribbon { get { return ViewInfo.Ribbon; } }
		public RibbonPageCategoryViewInfoCollection PageCategories { get { return pageCategories; } }
		protected virtual RibbonItemViewInfoCollection CreatePageHeaderItems() { return new RibbonItemViewInfoCollection(this); }
		public RibbonItemViewInfoCollection PageHeaderItems {
			get {
				if(pageHeaderItems == null) pageHeaderItems = CreatePageHeaderItems();
				return pageHeaderItems;
			}
		}
		public RibbonPageViewInfoCollection Pages { get { return pages; } }
		public Rectangle PageHeaderBounds { get { return pageHeaderBounds; } set { pageHeaderBounds = value; } }
		public float PageSeparateLineOpacity { get { return pageSeparateLineOpacity; } }
		public virtual void CalcPageHeader() {
			Clear();
			this.bounds = CalcBounds();
			this.showScrollButtons = false;
			this.leftScrollBounds = Rectangle.Empty;
			this.rightScrollBounds = Rectangle.Empty;
			CreatePagesViewInfo();
			CreatePageCategoriesViewInfo();
			CreateHeaderItemsViewInfo();
			CalcHeaderItemsLayout(CalcHeaderItemsAvailableRect(Bounds));
			Rectangle availableRect = CalcAvailableHeaderRect();
			Size bestSize = new Size(CalcHeaderBestWidth(), availableRect.Height);
			this.pageHeaderBounds = CalcActualHeaderRect(availableRect, bestSize);
			CalcPageSeparateLineOpacity(PageHeaderBounds.Width);
			CalcHeaderLayout(bestSize);
			if(!ViewInfo.GetShowPageHeaders()) {
				PageCategories.Clear();
				Pages.Clear();
				if(ViewInfo.IsEmptyTabHeader) {
					this.bounds = Rectangle.Empty;
					this.bounds.Y = ViewInfo.Caption.Bounds.Bottom;
				}
				else
					this.bounds.Height = ViewInfo.TabHeaderHeight;
				return;
			}
		}
		int minimumFormWidth = 600;
		protected virtual Rectangle CalcHeaderItemsAvailableRect(Rectangle bounds) {
			if(!IsHeaderItemsInCaption) return bounds;
			return ViewInfo.IsRightToLeft ? BarUtilites.ConvertBoundsToRTL(ViewInfo.Caption.ContentBounds, ViewInfo.Bounds) : ViewInfo.Caption.ContentBounds;
		}
		protected virtual Rectangle CalcBounds() {
			Rectangle rect = ControlContentBounds;
			if(ViewInfo.Caption.Bounds.IsEmpty) {
				if(ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above) rect.Y += ViewInfo.Toolbar.Bounds.Height;
			}
			else {
				rect.Y = ViewInfo.Caption.Bounds.Bottom;
			}
			rect.Height = ViewInfo.TabHeaderHeight;
			return rect;
		}
		protected internal virtual void InitializeLeftScroll() {
			XtraAnimator.Current.AddObject(ViewInfo, RibbonHitTest.PageHeaderLeftScroll, 20000, 10000, new CustomAnimationInvoker(OnLeftScroll));		
		}
		protected internal virtual void InitializeRightScroll() { 
			XtraAnimator.Current.AddObject(ViewInfo, RibbonHitTest.PageHeaderRightScroll, 20000, 10000, new CustomAnimationInvoker(OnRightScroll));
		}
		protected virtual void ScrollPageHeader() {
			bool gAdd = false;
			if(ViewInfo.GInfo.Graphics == null) {
				ViewInfo.GInfo.AddGraphics(null);
				gAdd = true;
			}
			try {
				CalcPageHeader();
			}
			finally {
				if(gAdd) ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		Rectangle GetScrollRectToInvalidate() {
			Rectangle rect = ViewInfo.Bounds;
			rect.Height = Bounds.Bottom;
			return rect;
		}
		protected virtual void OnLeftScroll(BaseAnimationInfo info) {
			if(info.PrevFrame < 0) return;
			int delta = info.CurrentFrame - info.PrevFrame;
			PageHeaderScrollOffset = Math.Max(PageHeaderScrollOffset - delta, 0);
			if(PageHeaderScrollOffset == 0) XtraAnimator.RemoveObject(ViewInfo, info.AnimationId);
			ScrollPageHeader();
			ViewInfo.Invalidate(GetScrollRectToInvalidate());
		}
		protected virtual void OnRightScroll(BaseAnimationInfo info) {
			if(info.PrevFrame < 0) return;
			int delta = info.CurrentFrame - info.PrevFrame;
			int max = (Ribbon.AllowTrimPageText ? CalcHeaderMinWidth() : CalcHeaderMinWidthWithText()) - PageHeaderBounds.Width;
			PageHeaderScrollOffset = Math.Min(PageHeaderScrollOffset + delta, max);
			if(PageHeaderScrollOffset == max) XtraAnimator.RemoveObject(ViewInfo, info.AnimationId);
			ScrollPageHeader();
			ViewInfo.Invalidate(GetScrollRectToInvalidate());
		}
		public virtual bool ShowScrollButtons { get { return showScrollButtons; } }
		public virtual bool ShowLeftScrollButton {
			get {
				return ShowScrollButtons && !LeftScrollBounds.IsEmpty;
			}
		}
		public virtual bool ShowRightScrollButton {
			get {
				return ShowScrollButtons && !RightScrollBounds.IsEmpty;
			}
		}
		protected virtual int GetHeaderItemsBestWidth() {
			int width = 0;
			Size size = Size.Empty;
			foreach(RibbonItemViewInfo itemInfo in PageHeaderItems) {
				if(ViewInfo.IsExpandItemInRibbonPanel(itemInfo)) continue;
				size = itemInfo.CalcBestSize();
				size.Height = Math.Min(PageHeaderItemHeight, size.Height);
				itemInfo.Bounds = new Rectangle(Point.Empty, size);
				width += itemInfo.Bounds.Width;
			}
			return width;
		}
		public virtual Rectangle PageHeaderItemsBounds { get { return pageHeaderItemsBounds; } }
		protected internal virtual void CalcHeaderItemsLayout(Rectangle rect){
			int width = GetHeaderItemsBestWidth();
			pageHeaderItemsBounds = new Rectangle(rect.Right - width, rect.Y, width, rect.Height);
			int x = pageHeaderItemsBounds.X;
			foreach(RibbonItemViewInfo itemInfo in PageHeaderItems) {
				if(ViewInfo.IsExpandItemInRibbonPanel(itemInfo)) continue;
				Size itemSize = itemInfo.CalcBestSize();
				itemInfo.Bounds = new Rectangle(x, RectangleHelper.GetCenterBounds(PageHeaderItemsBounds, itemSize).Y, itemSize.Width, IsHeaderItemsInCaption ? itemSize.Height : PageHeaderItemHeight);
				x += itemInfo.Bounds.Width;
				if(Ribbon.ViewInfo.IsRightToLeft)
					itemInfo.Bounds = BarUtilites.ConvertBoundsToRTL(itemInfo.Bounds, PageHeaderItemsBounds);
			}
		}
		public Rectangle DesignerRect { 
			get {
				if(!ViewInfo.IsDesignMode) return Rectangle.Empty;
				if(PageHeaderItems.Count > 0) {
					if(PageHeaderItems.Count != 1 || !ViewInfo.IsExpandItemInRibbonPanel(PageHeaderItems[0]))
						return PageHeaderItemsBounds;
				}
				int x = ViewInfo.IsRightToLeft ? PageHeaderItemsBounds.Left : PageHeaderItemsBounds.Right - PageHeaderItemsBounds.Height;
				return new Rectangle(x, PageHeaderItemsBounds.Top, PageHeaderItemsBounds.Height, PageHeaderItemsBounds.Height);
			} 
		}
		protected virtual void CalcPageSeparateLineOpacity(int availWidth) {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) {
				this.pageSeparateLineOpacity = 0.0f;
				return;
			}
			int widthWithoutSeparatorLine = CalcHeaderWidthWithoutSeparateLine();
			int minWidthWithText = CalcHeaderMinWidthWithText();
			if(availWidth > widthWithoutSeparatorLine) this.pageSeparateLineOpacity = 0.0f;
			else {
				this.pageSeparateLineOpacity = widthWithoutSeparatorLine - minWidthWithText;
				this.pageSeparateLineOpacity = this.pageSeparateLineOpacity == 0? 0: ((float)(widthWithoutSeparatorLine - availWidth)) / this.pageSeparateLineOpacity;
			}
			this.pageSeparateLineOpacity = Math.Min(this.pageSeparateLineOpacity, 1.0f);
		}
		public virtual RibbonPageViewInfo SelectedPageInfo {
			get {
				for(int i = 0; i < Pages.Count; i++)
					if(Ribbon.SelectedPage == Pages[i].Page) return Pages[i];
				return null;
			}
		}
		protected virtual bool ShouldProcessPageCategory(RibbonPageCategory category) {
			if(ViewInfo.Ribbon.IsDesignMode)
				return true;
			if(!category.ActualVisible)
				return false;
			foreach(RibbonPage page in category.Pages) {
				if(page.ActualVisible)
					return true;
			}
			foreach(RibbonPage page in category.MergedPages) {
				if(page.ActualVisible)
					return true;
			}
			return false;
		}
		protected internal virtual void CreatePageCategoriesViewInfo() {
			ArrayList items = MergeOrderArrayBuilder.GetArrangedItems(Ribbon.PageCategories, Ribbon.MergedCategories, ViewInfo.IsRibbonMerged);
			for(int i = 0; i < items.Count; i++) {
				if(!ShouldProcessPageCategory((RibbonPageCategory)items[i])) continue;
				PageCategories.Add(CreatePageCategoryViewInfo((RibbonPageCategory)items[i]));
			}
		}
		protected internal virtual RibbonItemViewInfo CreateHeaderItemViewInfo(IRibbonItem item) {
			return ViewInfo.CreateItemViewInfo(item);
		}
		protected internal virtual void ClearHeaderItemsViewInfo() {
			PageHeaderItems.Clear();
		}
		protected internal virtual bool ShoulCreateItemInfo(IRibbonItem item) {
			BarItemLink link = item as BarItemLink;
			return ViewInfo.ShouldCreateItemInfo(link);
		}
		protected virtual Image GetExtraGlyph() {
			SkinElement glyph = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinQuickToolbarButtonGlyph];
			if(glyph.Glyph != null) return glyph.Glyph.Image;
			return null;
		}
		protected internal virtual void UpdateItemViewInfo(RibbonItemViewInfo itemInfo) {
			itemInfo.Owner = this;
			itemInfo.AllowedStyles = GetItemStyle((BarItemLink)itemInfo.Item);
			itemInfo.ExtraGlyph = GetExtraGlyph();
		}   
		protected internal virtual void CreateHeaderItemsViewInfo() {
			ClearHeaderItemsViewInfo();
			CreateExpandCollapseItemViewInfo();
			CreateAutoHiddenPagesMenuItemViewInfo();
			CreateTouchUIItemViewInfo();
			List<BarItemLink> links = InplaceLinksHelper.GetLinks(Ribbon.Manager, Ribbon.PageHeaderItemLinks, Ribbon.AllowInplaceLinks, Ribbon.IsDesignMode, (link) => ShoulCreateItemInfo(link));
			foreach(IRibbonItem item in links) {
				if(!ShoulCreateItemInfo(item)) continue;
				RibbonItemViewInfo itemInfo = CreateHeaderItemViewInfo(item);
				if(itemInfo == null) continue;
				UpdateItemViewInfo(itemInfo);
				if(PageHeaderItems.Count > 0 && itemInfo.Item.BeginGroup) 
					PageHeaderItems.Add(new RibbonSeparatorItemViewInfo(this, item));
				PageHeaderItems.Add(itemInfo);
			}
			CreateMDIItemsViewInfo();
		}
		protected virtual void CreateTouchUIItemViewInfo() {
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.OfficeUniversal)
				return;
			if(!Ribbon.OptionsTouch.ShowTouchUISelectorInQAT || ViewInfo.IsEmptyTabHeader || IsTouchUIItemInPageHeaderItemLinks)
				return;
			this.touchUIItemInfo = Ribbon.Manager.CreateItemViewInfo(ViewInfo, Ribbon.Toolbar.TouchMouseModeItemLink);
			Ribbon.Toolbar.TouchMouseModeItemLink.LinkViewInfo.UpdateLinkState();
			PageHeaderItems.Add(this.touchUIItemInfo);
		}
		protected internal RibbonItemStyles GetItemStyle(BarItemLink link) {
			return RibbonItemInToolbarStyleCalculator.GetItemStyle(link, true);
		}
		RibbonItemViewInfo expandCollapseItemInfo = null, autoHiddenPagesMenuItemInfo = null, mdiCloseItemInfo = null, mdiMinimizeItemInfo = null, mdiRestoreItemInfo = null, touchUIItemInfo = null;
		protected virtual RibbonItemViewInfo CreateSystemItemViewInfo(IRibbonItem item, Image glyph) {
			RibbonItemViewInfo info = CreateHeaderItemViewInfo(item);
			UpdateItemViewInfo(info);
			info.ExtraGlyph = glyph;
			PageHeaderItems.Add(info);
			return info;
		}
		protected Image GetGlyph(SkinGlyph glyph, ObjectState state) {
			ImageCollection coll = glyph.GetImages();
			if(state == ObjectState.Pressed && coll.Images.Count == 3) { 
				return coll.Images[2];
			}
			if(state == ObjectState.Hot && coll.Images.Count >= 2) {
				return coll.Images[1];
			}
			return coll.Images[0];
		}
		protected Image GetCloseButtonGlyphImage(ObjectState state) { 
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinCloseButtonGlyph];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			return Ribbon.GetController().GetBitmap("CloseImage");
		}
		protected Image GetMinimizeButtonGlyphImage(ObjectState state) {
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinMinimizeButtonGlyph];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			return Ribbon.GetController().GetBitmap("MinImage");
		}
		protected Image GetRestoreButtonGlyphImage(ObjectState state) {
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinRestoreButtonGlyph];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			return Ribbon.GetController().GetBitmap("RestoreImage");
		}
		protected internal Image GetExpandButtonGlyphImage(ObjectState state) {
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinExpandButtonGlyph];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			return Ribbon.GetController().GetBitmap("ExpandImage");
		}
		protected internal Image GetCollapseButtonGlyphImage(ObjectState state) {
			SkinElement elem = BarSkins.GetSkin(ViewInfo.Provider)[BarSkins.SkinCollapseButtonGlyph];
			if(elem != null && elem.Glyph != null)
				return GetGlyph(elem.Glyph, state);
			return Ribbon.GetController().GetBitmap("CollapseImage");
		}
		protected internal ObjectState GetSystemLinkState(BarItemLink link) {
			if(Ribbon.Manager.PressedLink == link || ViewInfo.PressedObject.Item == link)
				return ObjectState.Pressed;
			if(Ribbon.Manager.HighlightedLink == link || ViewInfo.HotObject.Item  == link)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		protected bool ShouldCreateMDIItemsViewInfo() {
			Form frm = Ribbon.FindForm();
			if(frm == null || frm.ActiveMdiChild == null || frm.ActiveMdiChild.WindowState != FormWindowState.Maximized || !Ribbon.AllowMdiChildButtons) return false;
			return true;
		}
		protected virtual void CreateMDIItemsViewInfo() {
			Form frm = Ribbon.FindForm();
			if(!ShouldCreateMDIItemsViewInfo())
				return;
			if(frm.ActiveMdiChild.MinimizeBox)
				mdiMinimizeItemInfo = CreateSystemItemViewInfo(Ribbon.MDIMinimizeItemLink, GetMinimizeButtonGlyphImage(GetSystemLinkState(Ribbon.MDIMinimizeItemLink)));
			if(frm.ActiveMdiChild.MaximizeBox)
				mdiRestoreItemInfo = CreateSystemItemViewInfo(Ribbon.MDIRestoreItemLink, GetRestoreButtonGlyphImage(GetSystemLinkState(Ribbon.MDIRestoreItemLink)));
			if(frm.ActiveMdiChild.ControlBox)
				mdiCloseItemInfo = CreateSystemItemViewInfo(Ribbon.MDICloseItemLink, GetCloseButtonGlyphImage(GetSystemLinkState(Ribbon.MDICloseItemLink)));
		}
		bool IsExpandCollapseItemInPageHeaderItemLinks {
			get {
				foreach(BarItemLink link in Ribbon.PageHeaderItemLinks) {
					if(link.Item == Ribbon.ExpandCollapseItem)
						return true;
				}
				return false;
			}
		}
		bool IsTouchUIItemInPageHeaderItemLinks {
			get {
				foreach(BarItemLink link in Ribbon.PageHeaderItemLinks) {
					if(link.Item == Ribbon.Toolbar.TouchMouseModeItem)
						return true;
				}
				return false;
			}
		}
		protected virtual void CreateExpandCollapseItemViewInfo() {
			try {
				if(!Ribbon.GetShowExpandCollapseButton() || IsExpandCollapseItemInPageHeaderItemLinks || ViewInfo.IsEmptyTabHeader)
					return;
				expandCollapseItemInfo = Ribbon.Manager.CreateItemViewInfo(ViewInfo, Ribbon.ExpandCollapseItemLink);
				Ribbon.ExpandCollapseItemLink.LinkViewInfo.UpdateLinkState();
				PageHeaderItems.Add(expandCollapseItemInfo);
			}
			catch(Exception e) {
				throw new NullReferenceException(CreateNullRefLog(), e);
			}
		}
		protected virtual void CreateAutoHiddenPagesMenuItemViewInfo() {
			try {
				if(Ribbon.GetRibbonStyle() != RibbonControlStyle.OfficeUniversal) return;
				autoHiddenPagesMenuItemInfo = Ribbon.Manager.CreateItemViewInfo(ViewInfo, Ribbon.AutoHiddenPagesMenuItemLink);
				Ribbon.AutoHiddenPagesMenuItemLink.LinkViewInfo.UpdateLinkState();
			}
			catch(Exception e) {
				throw new NullReferenceException(CreateNullRefLog(), e);
			}
		}
		string CreateNullRefLog() {
			string message = string.Empty;
			if(Ribbon == null) message = "Ribbon = null";
			else if(Ribbon.Manager == null) {
				message = "Ribbon.Manager = null ";
				message += "Ribbon.IsDisposed = " + Ribbon.IsDisposed + " ";
			}
			else {
				message += "Ribbon.IsDisposed = " + Ribbon.IsDisposed + " ";
				if(Ribbon.FindForm() == null)
					message += "Ribbon.Form = null ";
				else {
					message += "Ribbon.Form = " + Ribbon.FindForm().ToString() + " IsMdiChild = " + Ribbon.FindForm().IsMdiChild + " IsVisible = " + Ribbon.FindForm().Visible;
					message += "IsDisposed = " + Ribbon.FindForm().IsDisposed;
				}
				if(Ribbon.ExpandCollapseItemLink == null)
					message += " Ribbon.ExpandCollapseItemLink = null";
				else
					message += " Ribbon.ExpandCollapseItemLink = " + Ribbon.ExpandCollapseItemLink.GetHashCode() + " ";
				if(Ribbon.ExpandCollapseItemLink.LinkViewInfo == null)
					message += " Ribbon.ExpandCollapseItemLink.LinkViewInfo = null";
				else
					message += " Ribbon.ExpandCollapseItemLink.LinkViewInfo = " + Ribbon.ExpandCollapseItemLink.LinkViewInfo.GetHashCode();
				if(Ribbon.AutoHiddenPagesMenuItemLink.LinkViewInfo == null)
					message += " Ribbon.AutoHiddenPagesMenuItemLink = null";
				else
					message += " Ribbon.AutoHiddenPagesMenuItemLink = " + Ribbon.AutoHiddenPagesMenuItemLink.GetHashCode() + " ";
				if(Ribbon.AutoHiddenPagesMenuItemLink.LinkViewInfo == null)
					message += " Ribbon.AutoHiddenPagesMenuItemLink.LinkViewInfo = null";
				else
					message += " Ribbon.AutoHiddenPagesMenuItemLink.LinkViewInfo = " + Ribbon.AutoHiddenPagesMenuItemLink.LinkViewInfo.GetHashCode();
			}
			return message;
		}
		protected virtual ArrayList GetPagesForCategory(RibbonPageCategory cat) {
			ArrayList res = MergeOrderArrayBuilder.GetArrangedItems(cat.Pages, cat.MergedPages, ViewInfo.IsRibbonMerged);
			return res;
		}
		protected virtual ArrayList GetPages() {
			ArrayList res = new ArrayList();
			if(!ViewInfo.GetShowPageHeaders())
				return res;
			res.AddRange(GetPagesForCategory(Ribbon.DefaultPageCategory));
			ArrayList cats = MergeOrderArrayBuilder.GetArrangedItems(Ribbon.PageCategories, Ribbon.MergedCategories, ViewInfo.IsRibbonMerged);
			foreach(RibbonPageCategory cat in cats) {
				res.AddRange(GetPagesForCategory(cat));
			}
			return res;
		}
		protected internal virtual void CreatePagesViewInfo() {
			ArrayList res = GetPages();
			for(int i = 0; i < res.Count; i++) {
				RibbonPage page = (RibbonPage)res[i];
				if(!page.ActualVisible && !ViewInfo.Ribbon.IsDesignMode) continue;
				Pages.Add(CreatePageViewInfo(page));
			}
		}
		protected internal int IndentBetweenPages {
			get {
				string skinName = ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice ? RibbonSkins.OptIndentBetweenMacPages : RibbonSkins.OptIndentBetweenPages;
				object res = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[skinName];
				if(res == null) 
					return ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice? 0: DefaultIndentBetweenPages;
				return (int)res;
			}
		}
		protected internal int IndentBetweenCategoryPages {
			get {
				if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)
					return 0;
				object res = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.OptIndentBetweenCategoryPages];
				if(res == null) return DefaultIndentBetweenCategoryPages;
				return (int)res;
			}
		}
		protected virtual int GetIndentBetweenPages(int index) {
			if(index < Pages.Count - 1) {
				if(Pages[index].Category != Pages[index + 1].Category && Ribbon.ShowCategoryInCaption) {
					if(Pages[index].Page.IsInDefaultCategory) return IndentBetweenPages + (IndentBetweenCategoryPages - IndentBetweenPages) / 2;	
					else return IndentBetweenCategoryPages;
				}
				else return IndentBetweenPages;
			}
			return 0;
		}
		protected internal virtual int CalcPrecalculatedWidth() {
			if(Pages.Count == 0) return 0;
			int res = 0;
			for(int i = 0; i < Pages.Count; i++) {
				if(Pages[i].PrecalculatedWidth != 0)
					res += Pages[i].PrecalculatedWidth + GetIndentBetweenPages(i);
			}
			return res;
		}
		protected internal virtual int CalcHeaderBestWidth() {
			for(int i = 0; i < Pages.Count; i++) {
				Pages[i].PrecalculatedWidth = Pages[i].CalcBestWidth();
			}
			return CalcPrecalculatedWidth() + CalcEmptyPageCategoriesWidth();
		}
		protected internal int CalcEmptyPageCategoriesWidth() {
			if(!ViewInfo.Ribbon.IsDesignMode) return 0;
			int width = 0;
			foreach(RibbonPageCategoryViewInfo categoryInfo in PageCategories) {
				if(categoryInfo.Pages.Count == 0) width += 50;
			}
			return width;
		}
		protected internal virtual int CalcHeaderMinWidthWithText() {
			for(int i = 0; i < Pages.Count; i++) {
				Pages[i].PrecalculatedWidth = Pages[i].CalcMinWidthWithText();
			}
			return CalcPrecalculatedWidth() + CalcEmptyPageCategoriesWidth();
		}
		protected internal virtual int CalcHeaderWidthWithoutSeparateLine() {
			int width = 0;
			for(int i = 0; i < Pages.Count; i++) {
				width += Pages[i].CalcWidthWithoutSeparateLine();
			}
			return width;
		}
		protected virtual int CalcPagesIndent() {
			int width = 0;
			for(int i = 0; i < Pages.Count - 1; i++)
				width += GetIndentBetweenPages(i);
			return width;
		}
		protected internal virtual int CalcHeaderMinWidth() {
			if(Pages.Count == 0) return 0;
			return Pages.Count * GetPageHeaderMinWidth() + CalcPagesIndent() + CalcEmptyPageCategoriesWidth();
		}
		protected virtual RibbonPageViewInfo CreatePageViewInfo(RibbonPage page) { return new RibbonPageViewInfo(ViewInfo, page); }
		protected virtual RibbonPageCategoryViewInfo CreatePageCategoryViewInfo(RibbonPageCategory category) { return new RibbonPageCategoryViewInfo(ViewInfo, category); }
		protected internal virtual void Clear() {
			ClearHeaderItemsViewInfo();
			Pages.Clear();
			PageCategories.Clear();
		}
		public virtual int HeaderLeftIndent {
			get {
				int res = 4;
				if(Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) {
					object leftIndent = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.OptTabHeaderLeftXIndent];
					if(leftIndent == null)
						res = Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 ? 0 : 28;
					else
						res = (int)leftIndent;
				}
				else if(ViewInfo.ApplicationButtonSize.Width > 0)
					res += ViewInfo.ApplicationButtonSize.Width + (ViewInfo.ApplicationButtonLeftIndent + ViewInfo.ApplicationButtonRightIndent);
				return res;
			}
		}
		protected virtual Rectangle GetAvailableHeaderRectCore(Rectangle ownerRect, params Rectangle[] rects) {
			int[][] buffer = BarUtilites.CreateZBuffer(ownerRect);
			foreach(Rectangle rect in rects) {
				if(rect.IsEmpty || !ownerRect.IntersectsWith(rect)) continue;
				for(int i = Math.Max(ownerRect.Left, rect.Left); i < Math.Min(ownerRect.Right, rect.Right); i++) { buffer[1][i]++; }
			}
			List<Rectangle> emptyAreas = BarUtilites.GetEmptyAreas(buffer, ownerRect.Width);
			if(emptyAreas.Count == 0) return ownerRect;
			Rectangle r = emptyAreas.Where(x => x.Width == emptyAreas.Max(max => max.Width)).First();
			return new Rectangle(r.X, ownerRect.Y, r.Width, ownerRect.Height);
		}
		protected internal virtual Rectangle CalcAvailableHeaderRect() {
			RibbonControlStyle style = ViewInfo.GetRibbonStyle();
			List<Rectangle> rects = new List<Rectangle>();
			if(!ViewInfo.Header.IsHeaderItemsInCaption)
				rects.Add(PageHeaderItemsBounds);
			rects.Add(GetApplicationButtonArea());
			if(!ViewInfo.Caption.GetCaptionCategoriesArea().IsEmpty)
				if(ViewInfo.IsRightToLeft)
					rects.Add(BarUtilites.ConvertBoundsToRTL(ViewInfo.Caption.GetCaptionButtonsArea(), viewInfo.Bounds));
				else
					rects.Add(ViewInfo.Caption.GetCaptionButtonsArea());
			Rectangle r = GetAvailableHeaderRectCore(new Rectangle(Bounds.X, 0, Bounds.Width, Bounds.Height + Bounds.Y), rects.ToArray());
			int rightIndent = (style == RibbonControlStyle.OfficeUniversal && ViewInfo.Header.IsHeaderItemsInCaption) ? 0 : HeaderRightIndent;
			return new Rectangle(r.X + HeaderLeftIndent, Bounds.Y, r.Width - HeaderLeftIndent - rightIndent, Bounds.Height);
		}
		protected virtual Rectangle GetApplicationButtonArea() {
			if(Ribbon.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal) return ViewInfo.ApplicationButton.Bounds;
			if(Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007) return Rectangle.Empty;
			return new Rectangle(ViewInfo.Header.Bounds.X, ViewInfo.Header.Bounds.Y, 
				ViewInfo.Office2010ApplicationButtonLeftIndent + ViewInfo.ApplicationButtonSize.Width + ViewInfo.Office2010ApplicationButtonRightIndent + ViewInfo.Header.HeaderLeftIndent,
				ViewInfo.ApplicationButtonSize.Height);
		}
		protected virtual Rectangle CalcActualHeaderRect(Rectangle availableHeaderRect, Size bestSize) {
			Rectangle rect = new Rectangle(availableHeaderRect.Location, new Size(Math.Min(bestSize.Width, availableHeaderRect.Width), Math.Min(bestSize.Height, availableHeaderRect.Height)));
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.TabletOffice)
				rect.X = availableHeaderRect.X + (availableHeaderRect.Width - rect.Width) / 2;
			return rect;
		}
		protected virtual void CalcHeaderLayout(Size bestSize) {
			if(Pages.Count == 0) return;
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal)
				CalcUniversalHeaderLayout();
			else
				CalcHeaderLayoutCore(bestSize);
		}
		protected virtual void CalcHeaderLayoutCore(Size bestSize) {
			if(bestSize.Width <= PageHeaderBounds.Width || ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				CalcSimpleHeaderLayout();
			else
				CalcComplexHeaderLayout();
			UpdateHeaderLayoutBounds(false);
			CalcPageCategoriesViewInfo();
			UpdateHeaderLayoutBounds(true);
			Rectangle availRect = CalcAvailableHeaderRect();
			if(ShouldUpdateContextPagesLayout || ShouldUpdateCategoriesTextBounds(bestSize.Width, availRect.Width)) {
				UpdatePageCategoriesLayout(bestSize.Width, availRect.Width);
				UpdateRightPageCategories();
				UpdateContextPagesLayout();
				if(Pages.Count > 0) {
					availRect = CalcAvailableHeaderRect();
					this.pageHeaderBounds.Width = Math.Min(availRect.Width, Pages[Pages.Count - 1].Bounds.Right - Pages[0].Bounds.X);
					if(Pages[0].Bounds.X > this.pageHeaderBounds.X && this.pageHeaderBounds.Width < availRect.Width) this.pageHeaderBounds.X = Pages[0].Bounds.X;
				}
			}
			foreach(RibbonPageViewInfo page in Pages)
				page.CalcViewInfo();
		}
		protected virtual void CalcUniversalHeaderLayout() {
			CalcUniversalHeaderLayoutBounds();
			CalcPageCategoriesViewInfo();
			pageHeaderBounds.Width = CalcAvailableHeaderRect().Width;
			if(ShouldUpdateContextPagesLayout) {
				UpdateRightPageCategories();
				UpdateContextPagesLayout();
			}
			foreach(RibbonPageViewInfo page in Pages) {
				if(page.Bounds.IsEmpty) continue;
				page.CalcViewInfo();
			}
		}
		protected virtual internal IEnumerable<BarButtonItem> GetHidedPages() {
			foreach(RibbonPageViewInfo page in Pages.OfType<RibbonPageViewInfo>().Where<RibbonPageViewInfo>(page => page.Bounds.Width == 0)) {
				BarButtonItem item = new BarButtonItem(Ribbon.Manager, page.Page.Text);
				item.ItemClick += delegate { ActiveDropDownPageIndex = Pages.IndexOf(page); };
				yield return item;
			}
		}
		protected virtual int GetLastVisiblePageIndex() {
			return Pages.IndexOf(Pages.OfType<RibbonPageViewInfo>().LastOrDefault<RibbonPageViewInfo>(x => !x.Bounds.IsEmpty && x.CategoryInfo == null));
		}
		protected virtual int CalcCategoriesWidth() {
			int width = 0;
			foreach(RibbonPageViewInfo page in Pages.OfType<RibbonPageViewInfo>().Where(x => x.CategoryInfo != null))
				width += page.CalcBestWidth();
			return width;
		}
		int activeDropDownPageIndex;
		protected virtual internal int ActiveDropDownPageIndex {
			get { return activeDropDownPageIndex; }
			set {
				if(ActiveDropDownPageIndex == value) return;
				activeDropDownPageIndex = value;
				ViewInfo.IsReady = false;
				Ribbon.Invalidate();
			}
		}
		protected virtual bool IsExistHidedPage { get { return Pages.OfType<RibbonPageViewInfo>().Any<RibbonPageViewInfo>(x => x.Bounds.IsEmpty && x.CategoryInfo == null); } }
		protected virtual void CalcUniversalHeaderLayoutBounds() {
			int availWidth = CalcAvailableHeaderRect().Width - CalcCategoriesWidth() - autoHiddenPagesMenuItemInfo.CalcBestSize().Width;
			if(ActiveDropDownPageIndex >= 0)
				availWidth -= Pages[ActiveDropDownPageIndex].CalcBestWidth();
			CalcVisiblePagesBounds(availWidth);
			CalcAutoHiddenPagesMenuItemBounds();
			CalcCategoriesBounds();
		}
		protected virtual void CalcVisiblePagesBounds(int availWidth) {
			int pagesWidth = 0;
			int pageX = CalcPagesStartPos();
			int lastVisibleIndex = 0;
			for(int i = 0; i < Pages.Count; i++) {
				if(Pages[i].CategoryInfo != null) continue;
				Rectangle bounds = new Rectangle(pageX, PageHeaderBounds.Top, Pages[i].CalcBestWidth(), PageHeaderBounds.Height);
				pagesWidth += bounds.Width;
				if(pagesWidth > availWidth) break;
				lastVisibleIndex = i;
				Pages[i].Bounds = bounds;
				pageX += bounds.Width + GetIndentBetweenPages(i);
			}
			ActiveDropDownPageIndex = CalcActiveDropDownPageIndex();
			if(ActiveDropDownPageIndex < 0) return;
			Pages[ActiveDropDownPageIndex].Bounds = new Rectangle(pageX, PageHeaderBounds.Top, Pages[ActiveDropDownPageIndex].CalcBestWidth(), PageHeaderBounds.Height);
		}
		protected virtual int CalcActiveDropDownPageIndex() {
			if(ActiveDropDownPageIndex < 0 || !IsExistHidedPage) return -1;
			if(!Pages[ActiveDropDownPageIndex].Bounds.IsEmpty) return GetLastVisiblePageIndex() + 1;
			return ActiveDropDownPageIndex;
		}
		protected virtual void CalcAutoHiddenPagesMenuItemBounds() {
			autoHiddenPagesMenuItemInfo.Bounds = Rectangle.Empty;
			if(!IsExistHidedPage) return;
			int index = ActiveDropDownPageIndex < 0 ? GetLastVisiblePageIndex() : ActiveDropDownPageIndex;
			int x = index < 0 ? ViewInfo.ApplicationButton.Bounds.Right : Pages[index].Bounds.Right;
			autoHiddenPagesMenuItemInfo.Bounds = new Rectangle(x, PageHeaderBounds.Top, autoHiddenPagesMenuItemInfo.CalcBestSize().Width, PageHeaderBounds.Height);
			if(ViewInfo.IsRightToLeft)
				autoHiddenPagesMenuItemInfo.Bounds = BarUtilites.ConvertBoundsToRTL(autoHiddenPagesMenuItemInfo.Bounds, ViewInfo.Ribbon.Bounds);
		}
		protected virtual void CalcCategoriesBounds() {
			int lastVisiblePageIndex = GetLastVisiblePageIndex();
			if(lastVisiblePageIndex < 0) return;
			int pageX = autoHiddenPagesMenuItemInfo.Bounds.IsEmpty ? Pages[lastVisiblePageIndex].Bounds.Right : autoHiddenPagesMenuItemInfo.Bounds.Right;
			for(int i = 0; i < Pages.Count; i++) {
				if(Pages[i].CategoryInfo == null) continue;
				Pages[i].Bounds = new Rectangle(pageX, PageHeaderBounds.Top, Pages[i].CalcBestWidth(), PageHeaderBounds.Height);
				pageX += Pages[i].Bounds.Width + GetIndentBetweenPages(i);
			}
		}
		protected virtual bool ShouldUpdateCategoriesTextBounds(int bestWidth, int availWidth) {
			if(bestWidth >= availWidth || !Ribbon.ShowCategoryInCaption) return false;
			foreach(RibbonPageCategoryViewInfo vi in PageCategories) {
				if(vi.BestTextWidth > vi.TextBounds.Width) return true;
			}
			return false;
		}
		protected virtual int GetHeaderRightIndent() {
			if(ViewInfo.Form == null) return Math.Max(this.PageHeaderItemsBounds.Width + 2, 16);
			else return Math.Max(Bounds.Right - ViewInfo.Caption.ContentBounds.Right + HeaderToCaptionIndent, PageHeaderItemsBounds.Width + 2);
		}
		protected virtual void UpdateRightPageCategories() {
			if(Ribbon.PageCategoryAlignment != RibbonPageCategoryAlignment.Right || PageCategories.Count == 0) return;
			int deltaX = CalcAvailableHeaderRect().Right - PageCategories[PageCategories.Count - 1].Bounds.Right;
			if(deltaX < 0) return;
			foreach(RibbonPageCategoryViewInfo category in PageCategories) {
				category.UpdateBounds(new Rectangle(category.Bounds.X + deltaX, category.Bounds.Y, category.Bounds.Width, category.Bounds.Height));
			}
		}
		protected virtual bool ShouldUpdateContextPagesLayout {
			get {
				if(Ribbon.PageCategoryAlignment == RibbonPageCategoryAlignment.Right || ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) return true;
				foreach(RibbonPageCategory pageCategory in Ribbon.PageCategories) {
					if(pageCategory.ActualVisible && pageCategory.Pages.Count == 0) return true;
				}
				return false;
			}
		}
		protected virtual void UpdatePageCategoriesLayout(int bestWidth, int availWidth) {
			int deltaX = 0;
			int deltaWidth = availWidth - bestWidth, deltaTextWidth;
			foreach(RibbonPageCategoryViewInfo category in PageCategories) {
				int width = category.Bounds.Width;
				deltaTextWidth = category.BestTextWidth - category.TextBounds.Width;
				if(deltaTextWidth > 0 && deltaWidth > 0) {
					deltaTextWidth = Math.Min(deltaWidth, deltaTextWidth);
					width += deltaTextWidth;
					deltaWidth -= deltaTextWidth;
				}
				else deltaTextWidth = 0;
				category.UpdateBounds(new Rectangle(category.Bounds.X + deltaX, category.Bounds.Y, width, category.Bounds.Height));
				if(category.Pages.Count == 0) deltaX += width + IndentBetweenPages;
				else deltaX += deltaTextWidth;
			}
		}
		protected virtual void UpdateContextPagesLayout() {
			foreach(RibbonPageCategoryViewInfo categoryInfo in PageCategories) {
				categoryInfo.UpdateContextPagesLayout();
			}
		}
		protected virtual void CalcPageCategoriesViewInfo() {
			for(int i = 0; i < PageCategories.Count; i++) {
				PageCategories[i].CalcViewInfo();
			}
		}
		protected virtual void CalcSimpleHeaderLayout() {
			PageHeaderScrollOffset = 0;
			for(int i = 0; i < Pages.Count; i++) {
				Pages[i].PrecalculatedWidth = Pages[i].CalcBestWidth();
			}
		}
		protected virtual int GetPageHeaderRightXCoord() {
			if(ViewInfo.Form != null) return ViewInfo.Caption.ContentBounds.Right - HeaderToCaptionIndent;
			return ControlContentBounds.Right;
		}
		public void UpdateHeaderLayout() {
			UpdateHeaderLayoutBounds(true);
			foreach(RibbonPageCategoryViewInfo catInfo in PageCategories) {
				catInfo.UpdateContextPagesLayout();
				foreach(RibbonPageViewInfo pageInfo in catInfo.Pages) {
					pageInfo.UpdateLayout();
				}
			}
			int bestWidth = CalcPrecalculatedWidth() + CalcEmptyPageCategoriesWidth();
			Rectangle availableRect = CalcAvailableHeaderRect();
			Size bestSize = new Size(bestWidth, availableRect.Height);
			this.pageHeaderBounds = CalcActualHeaderRect(availableRect, bestSize);
		}
		protected virtual void UpdateHeaderLayoutBounds(bool processCollapsedCategories) {
			int pageX = CalcPagesStartPos();
			int deltaX = 0;
			RibbonPageViewInfo prevPage = null;
			for(int i = 0; i < Pages.Count; i++) {
				if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice && processCollapsedCategories) {
					if(prevPage != null && prevPage.CategoryInfo != null && prevPage.Category != Pages[i].Category) {
						deltaX += prevPage.CategoryInfo.LowerBounds.Width - prevPage.CategoryInfo.RealLowerBounds.Width;
					}
				}
				Pages[i].Bounds = new Rectangle(pageX - deltaX, PageHeaderBounds.Top, Pages[i].PrecalculatedWidth, PageHeaderBounds.Height);
				pageX += Pages[i].Bounds.Width + GetIndentBetweenPages(i);
				prevPage = Pages[i];
			}
		}
		protected virtual int CalcPagesStartPos() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.TabletOffice) {
				Rectangle rect = CalcAvailableHeaderRect();
				int startPos =  rect.Left + (rect.Width - PageHeaderBounds.Width) / 2;
				if(startPos + PageHeaderBounds.Width > PageHeaderItemsBounds.X)
					startPos = PageHeaderItemsBounds.X - PageHeaderBounds.Width;
				return startPos;
			}
			return PageHeaderBounds.Left - PageHeaderScrollOffset;
		}
		protected internal virtual bool ShouldHidePages() { return PageHeaderBounds.Width < MinPageHeaderWidth; }
		protected internal virtual bool ShouldLayoutWithPageScroll() { return PageHeaderBounds.Width >= MinPageHeaderWidth && PageHeaderBounds.Width < CalcHeaderMinWidth(); }
		protected internal virtual bool ShouldLayoutWithTextCut(int minWidthWithText) { return PageHeaderBounds.Width >= CalcHeaderMinWidth() && PageHeaderBounds.Width < minWidthWithText; }
		protected internal virtual bool ShouldLayoutWithoutTextCut(int minWidthWithText) { return PageHeaderBounds.Width >= minWidthWithText; }
		protected virtual void CalcComplexHeaderLayout() {
			int minWidthWithText = CalcHeaderMinWidthWithText();
			if(ShouldHidePages()) {
				HidePages();
				HidePageCategories();
				return;
			}
			if(ShouldLayoutWithoutTextCut(minWidthWithText)) {
				CalcComplexHeaderLayoutWithText(minWidthWithText);
				return;
			}
			if(ShouldLayoutWithTextCut(minWidthWithText) && ViewInfo.Ribbon.AllowTrimPageText) {
				CalcComplexHeaderLayoutWithCutText(minWidthWithText);
				return;
			}
			CalcHeaderLayoutWithScroll(minWidthWithText);
		}
		protected virtual void HidePages() {
			Pages.Clear();
		}
		protected virtual void HidePageCategories() {
			PageCategories.Clear();
		}
		public virtual SkinElementInfo GetLeftScrollButtonInfo() {
			SkinElementInfo info = ViewInfo.GetScrollButtonInfo(RibbonSkins.SkinLeftScrollButton, RibbonHitTest.PageHeaderLeftScroll, LeftScrollBounds);
			if(!Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetRightScrollButtonInfo() {
			SkinElementInfo info = ViewInfo.GetScrollButtonInfo(RibbonSkins.SkinRightScrollButton, RibbonHitTest.PageHeaderRightScroll, RightScrollBounds);
			if(!Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual Size GetScrollButtonSize(string scrollElementName) {
			return ViewInfo.GetScrollButtonSize(scrollElementName, PageHeaderBounds.Height);
		}
		protected virtual Size GetLeftScrollButtonSize() {
			return GetScrollButtonSize(RibbonSkins.SkinLeftScrollButton);
		}
		protected virtual Size GetRightScrollButtonSize() {
			return GetScrollButtonSize(RibbonSkins.SkinRightScrollButton);
		}
		protected virtual void CalcHeaderLayoutWithScroll(int minWidthWithText) {
			Size sz;
			this.showScrollButtons = true;
			this.leftScrollBounds = Rectangle.Empty;
			int minWidth = ViewInfo.Ribbon.AllowTrimPageText? CalcHeaderMinWidth(): minWidthWithText;
			if(PageHeaderBounds.Width - (minWidth - PageHeaderScrollOffset) > 0) this.pageHeaderScrollOffset = minWidth - PageHeaderBounds.Width;
			if(PageHeaderScrollOffset != 0)
				this.leftScrollBounds = new Rectangle(new Point(PageHeaderBounds.Left, PageHeaderBounds.Top), GetLeftScrollButtonSize());
			this.rightScrollBounds = Rectangle.Empty;
			if(PageHeaderScrollOffset < minWidth - PageHeaderBounds.Width) {
				sz = GetRightScrollButtonSize();
				this.rightScrollBounds = new Rectangle(new Point(PageHeaderBounds.Right - sz.Width, PageHeaderBounds.Top), sz);
			}
			for(int i = 0; i < Pages.Count; i++) {
				Pages[i].PrecalculatedWidth = ViewInfo.Ribbon.AllowTrimPageText ? GetPageHeaderMinWidth() : Pages[i].CalcMinWidthWithText();
			}
		}
		protected virtual void CalcComplexHeaderLayoutWithText(int minWidthWithText) {
			PageHeaderScrollOffset = 0;
			int resDeltaWidth = PageHeaderBounds.Width - minWidthWithText;
			int deltaWidth = resDeltaWidth / Pages.Count;
			int remWidth = resDeltaWidth % Pages.Count;
			int pageX = PageHeaderBounds.Left;
			for(int i = 0; i < Pages.Count; i++) {
				int pageWidth = Pages[i].CalcMinWidthWithText() + deltaWidth;
				if(i < remWidth) pageWidth++;
				Pages[i].PrecalculatedWidth = pageWidth;
			}
		}
		internal class PageInfoSorter : IComparer {
			public int Compare(object x, object y) {
				return ((RibbonPageViewInfo)y).PrecalculatedWidth.CompareTo(((RibbonPageViewInfo)x).PrecalculatedWidth);
			}
		}
		protected virtual void CalcComplexHeaderLayoutWithCutText(int minWidthWithText) {
			PageHeaderScrollOffset = 0;
			CalcHeaderMinWidthWithText();
			ArrayList sortedPages = new ArrayList(Pages);
			sortedPages.Sort(new PageInfoSorter());
			int delta = minWidthWithText - PageHeaderBounds.Width;
			while(delta > 0) {
				for(int n = 0; n < sortedPages.Count; n++) {
					((RibbonPageViewInfo)sortedPages[n]).PrecalculatedWidth--; delta--;
					if(delta == 0 || n == sortedPages.Count - 1) break;
					if(((RibbonPageViewInfo)sortedPages[n]).PrecalculatedWidth >= ((RibbonPageViewInfo)sortedPages[n + 1]).PrecalculatedWidth) {
						break;
					}
				}
			}
		}
		protected internal virtual void UpdateSystemLinkGlyph(BarItemLink link, ObjectState state) {
			if(link == Ribbon.ExpandCollapseItemLink) {
				if(!Ribbon.GetShowExpandCollapseButton() || expandCollapseItemInfo == null)
					return;
				expandCollapseItemInfo.ExtraGlyph = Ribbon.Minimized ? GetExpandButtonGlyphImage(state) : GetCollapseButtonGlyphImage(state);
			}
			if(!ShouldCreateMDIItemsViewInfo())
				return;
			if(link == Ribbon.MDICloseItemLink) { 
				mdiCloseItemInfo.ExtraGlyph = GetCloseButtonGlyphImage(state);
			}
			else if(link == Ribbon.MDIRestoreItemLink) {
				mdiRestoreItemInfo.ExtraGlyph = GetRestoreButtonGlyphImage(state);
			}
			else if(link == Ribbon.MDIMinimizeItemLink){
				mdiMinimizeItemInfo.ExtraGlyph = GetMinimizeButtonGlyphImage(state);
			}
		}
		protected internal virtual BarItemLink GetLinkByPoint(Point pt) {
			for(int i = 0; i < PageHeaderItems.Count; i++) {
				if(PageHeaderItems[i].Bounds.Contains(pt)) return (PageHeaderItems[i].Item as BarItemLink);
			}
			return null;
		}
		public bool IsHeaderItemsInCaption { 
			get {
				if(!SupportHeaderItemsInCaption) return false;
				if(ViewInfo.IsEmptyTabHeader) return true;
				return ViewInfo.Ribbon.Bounds.Width < minimumFormWidth; 
			} 
		}
		protected internal virtual bool SupportHeaderItemsInCaption {
			get { return ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal; }
		}
	}
	public class RibbonPageCategoryViewInfoCollection : CollectionBase {
		public void Add(RibbonPageCategoryViewInfo categoryInfo) { List.Add(categoryInfo); }
		public void Insert(RibbonPageCategoryViewInfo categoryInfo, int index) { List.Insert(index, categoryInfo); }
		public RibbonPageCategoryViewInfo this[int index] { get { return List[index] as RibbonPageCategoryViewInfo; } }
		public RibbonPageCategoryViewInfo this[RibbonPageCategory category] {
			get {
				for(int n = 0; n < Count; n++) {
					if(this[n].Category == category) return this[n];
				}
				return null;
			}	
		}
		public bool Contains(RibbonPageCategoryViewInfo category) { return List.Contains(category); }
		public int IndexOf(RibbonPageCategoryViewInfo category) {
			if(!Contains(category)) return -1;
			return List.IndexOf(category);
		}
	}
	public class RibbonPageViewInfoCollection : CollectionBase {
		public void Add(RibbonPageViewInfo pageInfo) { List.Add(pageInfo); }
		public void Insert(RibbonPageViewInfo pageInfo, int index) { List.Insert(index, pageInfo); }
		public RibbonPageViewInfo this[int index] { get { return List[index] as RibbonPageViewInfo; } }
		public RibbonPageViewInfo this[RibbonPage page] {
			get {
				for(int n = 0; n < Count; n++) {
					if(this[n].Page == page) return this[n];
				}
				return null;
			}
		}
		public int IndexOf(RibbonPageViewInfo viewInfo) { return List.IndexOf(viewInfo); }
	}
	public class RibbonPanelViewInfo {
		Rectangle bounds;
		Rectangle contentBounds;
		int contentWidth;
		int minContentWidth;
		RibbonViewInfo viewInfo;
		RibbonPageGroupViewInfoCollection groups;
		RibbonPage page;
		RibbonPanelGroupPainter groupPainter;
		ObjectPainter panelPainter;
		int panelScrollOffset;
		bool showScrollButtons;
		Rectangle leftScrollButtonBounds, rightScrollButtonBounds;
		public RibbonPanelViewInfo(RibbonViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.page = null;
			this.bounds = Rectangle.Empty;
			this.groups = new RibbonPageGroupViewInfoCollection();
			this.panelScrollOffset = 0;
			this.leftScrollButtonBounds = this.rightScrollButtonBounds = Rectangle.Empty;
		}
		public RibbonPanelGroupPainter GroupPainter {
			get {
				if(groupPainter == null) groupPainter = CreatePanelGroupPainter();
				return groupPainter;
			}
		}
		public int DefaultIndentBetweenGroups { get { return ViewInfo.DefaultIndentBetweenPageGroups; } }
		internal void SetPanelScrollOffset(int offset) { panelScrollOffset = offset; }
		public int PanelScrollOffset { get { return panelScrollOffset; } }
		public bool ShowScrollButtons { get { return showScrollButtons; } }
		public Rectangle LeftScrollButtonBounds { get { return leftScrollButtonBounds; } }
		public Rectangle RightScrollButtonBounds { get { return rightScrollButtonBounds; } }
		public bool ShowLeftScrollButton { get { return ShowScrollButtons && !LeftScrollButtonBounds.IsEmpty; } }
		public bool ShowRightScrollButton { get { return ShowScrollButtons && !RightScrollButtonBounds.IsEmpty; } }
		public ObjectPainter PanelPainter {
			get {
				if(panelPainter == null) panelPainter = CreatePanelPainter();
				return panelPainter;
			}
		}
		public virtual void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.InGallery) {
				InvalidateGallery(hitInfo);
				return;
			}
			if(hitInfo.InPageItem) {
				Invalidate(hitInfo.ItemInfo);
				return;
			}
			if(hitInfo.InPageGroup)
				Invalidate(hitInfo.PageGroup);
		}
		public virtual void Invalidate(RibbonPageGroup pageGroup) {
			RibbonPageGroupViewInfo info = Groups[pageGroup];
			if(info != null) ViewInfo.Invalidate(info.Bounds);
		}
		public virtual BarItemLink GetLinkByPoint(Point pt, bool enterButtonGroup) {
			foreach(RibbonPageGroupViewInfo groupInfo in Groups) {
				if(groupInfo.Minimized) continue;
				if(groupInfo.Bounds.Contains(pt)) return groupInfo.GetLinkByPoint(pt, enterButtonGroup);
			}
			return null;
		}
		public virtual void Invalidate(RibbonItemViewInfo itemInfo) {
			if(itemInfo == null) {
				ViewInfo.Invalidate(Bounds);
				return;
			}
			ViewInfo.Invalidate(itemInfo.Bounds);
		}
		public virtual void InvalidateGallery(RibbonHitInfo hitInfo) {
			if(hitInfo.GalleryInfo != null)
				hitInfo.GalleryInfo.Invalidate(hitInfo);
			else {
				ViewInfo.Invalidate(Bounds);
			}
		}
		protected virtual RibbonPanelGroupPainter CreatePanelGroupPainter() { return new RibbonPanelGroupPainter(); }
		protected virtual ObjectPainter CreatePanelPainter() { return new RibbonPanelPainter(); }
		protected internal virtual void CalcHitInfo(RibbonHitInfo hitInfo) {
			if(hitInfo.ContainsSet(LeftScrollButtonBounds, RibbonHitTest.PanelLeftScroll)) return;
			if(hitInfo.ContainsSet(RightScrollButtonBounds, RibbonHitTest.PanelRightScroll)) return;
			if(ViewInfo.Ribbon.IsExpandButtonInPanel && PanelItems.CalcHitInfo(hitInfo)) return;
			for(int n = 0; n < Groups.Count; n++) {
				RibbonPageGroupViewInfo group = Groups[n];
				if(hitInfo.ContainsSet(group.Bounds, RibbonHitTest.PageGroup)) {
					hitInfo.PageGroup = group.PageGroup;
					hitInfo.PageGroupInfo = group;
					group.CalcHitInfo(hitInfo);
					return;
				}
			}
		}
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		internal int ContentWidth { get { return contentWidth; } set { contentWidth = value; } }
		internal int MinContentWidth { get { return minContentWidth; } set { minContentWidth = value; } }
		public RibbonPageGroupViewInfoCollection Groups { get { return groups; } }
		public GraphicsInfo GInfo { get { return ViewInfo.GInfo; } }
		public RibbonPage Page { get { return page; } set { page = value; } }
		protected internal virtual RibbonPanelLayoutCalculator CreatePanelLayoutCalculator() {
			return new RibbonPanelComplexLayoutCalculator(this); 
		}
		internal void SetShowScrollButtons(bool showScrollButtons) { this.showScrollButtons = showScrollButtons; }
		internal void SetLeftScrollButtonBounds(Rectangle bounds) {
			this.leftScrollButtonBounds = ViewInfo.IsRightToLeft ? BarUtilites.ConvertBoundsToRTL(bounds, ViewInfo.Ribbon.Bounds) : bounds;
		}
		internal void SetRigthScrollButtonBounds(Rectangle bounds) {
			this.rightScrollButtonBounds = ViewInfo.IsRightToLeft ? BarUtilites.ConvertBoundsToRTL(bounds, ViewInfo.Ribbon.Bounds) : bounds;
		}
		protected internal virtual void InitializeLeftScroll() {
			XtraAnimator.Current.AddObject(ViewInfo, RibbonHitTest.PanelLeftScroll, 20000, 10000, new CustomAnimationInvoker(OnLeftScroll));
		}
		protected internal virtual void InitializeRightScroll() {
			XtraAnimator.Current.AddObject(ViewInfo, RibbonHitTest.PanelRightScroll, 20000, 10000, new CustomAnimationInvoker(OnRightScroll));
		}
		RibbonPanelLayoutCalculator calculator = null;
		protected RibbonPanelLayoutCalculator Calculator { 
			get {
				if(calculator == null) calculator = new RibbonPanelLayoutCalculator(this);
				return calculator;
			} 
		}
		internal void SetDeltaPanelScroll(int scroll) {
			this.panelScrollOffset = Math.Max(PanelScrollOffset - scroll, 0);
			int max = MinContentWidth - ContentWidth;
			this.panelScrollOffset = Math.Min(PanelScrollOffset, max);
			ScrollPanel();
			ViewInfo.Invalidate(Bounds);
		}
		protected virtual void OnLeftScroll(BaseAnimationInfo info) {
			if(info.PrevFrame < 0) return;
			int delta = info.CurrentFrame - info.PrevFrame;
			this.panelScrollOffset = Math.Max(PanelScrollOffset - delta, 0);
			if(PanelScrollOffset == 0) XtraAnimator.RemoveObject(ViewInfo, info.AnimationId);
			ScrollPanel();
			ViewInfo.Invalidate(Bounds);
		}
		protected virtual void OnRightScroll(BaseAnimationInfo info) {
			if(info.PrevFrame < 0) return;
			int delta = info.CurrentFrame - info.PrevFrame;
			int max = MinContentWidth - ContentWidth;
			this.panelScrollOffset = Math.Min(PanelScrollOffset + delta, max);
			if(PanelScrollOffset >= max) XtraAnimator.RemoveObject(ViewInfo, info.AnimationId);
			ScrollPanel();
			ViewInfo.Invalidate(Bounds);
		}
		bool inScroll = false;
		protected internal bool InScroll { get { return inScroll; } set { inScroll = value; } }
		protected virtual void ScrollPanel() {
			InScroll = true;
			try {
				Calculator.CalcScrollInfo();
				Calculator.UpdatePanelLayout();
				ScrollItems();
			}
			finally {
				InScroll = false;
			}
		}
		protected virtual void ScrollButtonGroupItems(RibbonButtonGroupItemViewInfo group) {
			for(int j = 0; j < group.Items.Count; j++) {
				group.Items[j].IsReady = false;
				group.Items[j].CheckViewInfo(GInfo.Graphics);
			}
		}
		protected virtual void ScrollItems() {
			for(int i = 0; i < Groups.Count; i++) { 
				RibbonPageGroupViewInfo group = Groups[i];
				for(int j = 0; j < group.Items.Count; j++) {
					group.Items[j].IsReady = false;
					group.Items[j].CheckViewInfo(GInfo.Graphics);
					if(group.Items[j] is RibbonButtonGroupItemViewInfo)
						ScrollButtonGroupItems(group.Items[j] as RibbonButtonGroupItemViewInfo);
				}
			}
		}
		public virtual SkinElementInfo GetLeftScrollButtonInfo() {
			SkinElementInfo info = ViewInfo.GetScrollButtonInfo(RibbonSkins.SkinLeftScrollButton, RibbonHitTest.PanelLeftScroll, LeftScrollButtonBounds);
			info.RightToLeft = ViewInfo.IsRightToLeft;
			if(!ViewInfo.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetRightScrollButtonInfo() {
			SkinElementInfo info = ViewInfo.GetScrollButtonInfo(RibbonSkins.SkinRightScrollButton, RibbonHitTest.PanelRightScroll, RightScrollButtonBounds);
			info.RightToLeft = ViewInfo.IsRightToLeft;
			if(!ViewInfo.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual Size GetScrollButtonSize(string scrollElementName) {
			return ViewInfo.GetScrollButtonSize(scrollElementName, ContentBounds.Height);
		}
		protected internal virtual Size GetLeftScrollButtonSize() {
			return GetScrollButtonSize(RibbonSkins.SkinLeftScrollButton);
		}
		protected internal virtual Size GetRightScrollButtonSize() {
			return GetScrollButtonSize(RibbonSkins.SkinRightScrollButton);
		}
		RibbonPanelItemsViewInfo panelItems;
		protected internal RibbonPanelItemsViewInfo PanelItems {
			get {
				if(panelItems == null) panelItems = new RibbonPanelItemsViewInfo(ViewInfo);
				return panelItems;
			}
		}
		public virtual void CalcPanelContent() {
			GInfo.AddGraphics(null);
			try {
				PanelItems.CalcPanelItems();
				Rectangle rect = ViewInfo.ContentBounds;
				if(ViewInfo.IsEmptyTabHeader) {
					if(ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above) {
						if(ViewInfo.IsAllowCaption) {
							rect.Y = ViewInfo.Header.Bounds.Bottom;
						}
						else
							rect.Y = ViewInfo.Toolbar.Bounds.Bottom;
					}
					else {
						if(ViewInfo.IsAllowCaption) {
							rect.Y = ViewInfo.Header.Bounds.Bottom;
						}
					}
				}
				else {
					rect.Y = ViewInfo.Header.Bounds.Bottom - ViewInfo.PanelYOffset;
				}
				rect.Height = CalcPanelHeight();
				Bounds = rect;
				ContentBounds = CalcContentBounds();
				ContentWidth = ContentBounds.Width;
				if(Page == null) {
					Bounds = new Rectangle(Bounds.Location, Size.Empty);
					ContentBounds = new Rectangle(ContentBounds.Location, Size.Empty);
					return;
				}
				if(!ViewInfo.AllowCachedItemInfo || ViewInfo.Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007 || ViewInfo.Ribbon.ApplicationButtonContentControl == null || !ViewInfo.Ribbon.ApplicationButtonContentControl.ContentVisible) {
					CreateGroupsViewInfo();
					CreatePanelLayoutCalculator().CalcPanelLayout();
					UpdateContentWidth();
				}
				UpdateLayoutByRibbonStyle();
				this.bounds = CalcBoundsByContentBounds();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		Rectangle OffsetRect(Rectangle rect, int delta) {
			rect.X += delta;
			return rect;
		}
		protected virtual void UpdateLayoutByRibbonStyle() {
			if(ViewInfo.GetRibbonStyle() != RibbonControlStyle.TabletOffice) return;
			int deltaCore = Math.Max((Bounds.Width - CalcTotalWidth()) / 2, 0);
			int delta = ViewInfo.IsRightToLeft ? -deltaCore : deltaCore;
			if(delta == 0) return;
			foreach(RibbonPageGroupViewInfo vi in Groups) {
				vi.Bounds = OffsetRect(vi.Bounds, delta);
				vi.ContentBounds = OffsetRect(vi.ContentBounds, delta);
				foreach(RibbonItemViewInfo itemInfo in vi.Items) {
					itemInfo.Bounds = new Rectangle(itemInfo.Bounds.X + delta, itemInfo.Bounds.Y, itemInfo.Bounds.Width, itemInfo.Bounds.Height);
				}
			}
		}
		protected internal void UpdateContentWidth() {
			this.contentBounds.Width = Math.Max(CalcTotalWidth(), ContentBounds.Width);
			this.contentBounds.Width = Math.Max(ContentBounds.Width, ViewInfo.Header.PageHeaderBounds.Left - ViewInfo.Header.PageHeaderScrollOffset + ViewInfo.Header.CalcHeaderMinWidth());
		}
		protected internal int CalcTotalGroupsIndent() {
			if(Groups.Count == 0) return 0;
			return (Groups.Count - 1) * this.DefaultIndentBetweenGroups;
		}
		protected internal int CalcTotalWidth() {
			int resWidth = 0;
			for(int i = 0; i < Groups.Count; i++) {
				resWidth += Groups[i].Bounds.Width;
			}
			resWidth += CalcTotalGroupsIndent();
			return resWidth;
		}
		protected internal int CalcMinWidth() {
			int resWidth = 0;
			for(int i = 0; i < Groups.Count; i++) {
				resWidth += Groups[i].CalcMinWidth();
			}
			resWidth += CalcTotalGroupsIndent();
			return resWidth;
		}
		protected virtual int CalcPanelHeight() {
			RibbonPageGroupViewInfo info = CreateGroupViewInfo(RibbonPageGroup.NullGroup);
			Size size = info.CalcSizeByContentSize(new Size(100, ViewInfo.GroupContentHeight));
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, ViewInfo.GetPanelInfo(), new Rectangle(Point.Empty, size)).Height;
		}
		protected internal int GetExpandItemRegionWidth() {
			if(!ViewInfo.Ribbon.IsExpandButtonInPanel || ViewInfo.Header == null || PanelItems.ExpandItemInfo == null)
				return 0;
			return PanelItems.ExpandItemInfo.Bounds.Width - PanelItems.GetButtonOffset().X;
		}
		protected virtual Rectangle CalcContentBounds() {
			Rectangle rect = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, ViewInfo.GetPanelInfo());
			rect.Width = Math.Max(0, rect.Width - GetExpandItemRegionWidth());
			return rect;
		}
		protected virtual Rectangle CalcBoundsByContentBounds() {
			Rectangle rect = ContentBounds;
			rect.Width += GetExpandItemRegionWidth();
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, ViewInfo.GetPanelInfo(), rect);
		}
		protected internal virtual void Clear() {
			Bounds = Rectangle.Empty;
			foreach(RibbonPageGroupViewInfo groupInfo in Groups) {
				groupInfo.Clear();
			}
			Groups.Clear();
		}
		protected internal virtual RibbonPageGroupViewInfo CreateGroupViewInfo(RibbonPageGroup group) { return new RibbonPageGroupViewInfo(ViewInfo, group); }
		protected internal virtual void CreateGroupsViewInfo() {
			ArrayList items = MergeOrderArrayBuilder.GetArrangedItems(Page.Groups, Page.MergedGroups, ViewInfo.IsRibbonMerged);
			for(int i = 0; i < items.Count; i++) {
				RibbonPageGroup pageGroup = (RibbonPageGroup)items[i];
				if(!pageGroup.ActualVisible && !ViewInfo.Ribbon.IsDesignMode) continue;
				Groups.Add(CreateGroupViewInfo(pageGroup));
				if(pageGroup != null && pageGroup.PopupGroupInfo != null)
					pageGroup.PopupGroupInfo.Items.Clear();
			}
		}
	}
	public class RibbonItemViewInfoCollection : CollectionBase {
		object owner;
		public RibbonItemViewInfoCollection(object owner) { this.owner = owner; }
		public void Add(RibbonItemViewInfo item) { List.Add(item); }
		public RibbonItemViewInfo this[int index] { get { return List[index] as RibbonItemViewInfo; } }
		public RibbonItemViewInfo this[IRibbonItem item] {
			get {
				for(int n = 0; n < List.Count; n++) {
					if(this[n].Item == item) return this[n];
				}
				return null;
			}
		}
		public object Owner { get { return owner; } }
		public int IndexOf(RibbonItemViewInfo item) { return List.IndexOf(item); }
		public bool Contains(RibbonItemViewInfo item) { return List.Contains(item); }
		public virtual void Insert(int index, object value) { List.Insert(index, value); }
		public RibbonItemViewInfo Find(IRibbonItem item, Rectangle bounds) {
			for(int n = 0; n < Count; n++) {
				RibbonItemViewInfo info = this[n].Find(item, bounds);
				if(info != null) return info;
			}
			return null;
		}
	}
	public class RibbonPageCategoryViewInfo {
		public static int DefaultPageCategoryWidth = 50;
		Rectangle bounds, upperBounds, lowerBounds, textBounds;
		int bestTextWidth;
		RibbonPageCategory category;
		RibbonViewInfo viewInfo;
		RibbonPageViewInfoCollection pages;
		public RibbonPageCategoryViewInfo(RibbonViewInfo viewInfo, RibbonPageCategory category) {
			this.viewInfo = viewInfo;
			this.bounds = Rectangle.Empty;
			this.upperBounds = Rectangle.Empty;
			this.lowerBounds = Rectangle.Empty;
			this.textBounds = Rectangle.Empty;
			this.category = category;
			this.pages = new RibbonPageViewInfoCollection();
			InitializePages();
		}
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		public RibbonPageCategory Category { get { return category; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle UpperBounds { get { return upperBounds; } }
		public Rectangle LowerBounds { get { return lowerBounds; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle RealLowerBounds { 
			get {
				if(Animated)
					return AnimatedBounds;
				return GetRealLowerBounds();
			} 
		}
		Rectangle animatedBounds;
		public Rectangle AnimatedBounds {
			get { return animatedBounds; }
			set { animatedBounds = value; }
		}
		public Rectangle CollapsedBounds {
			get { return Pages.Count > 0 ? Pages[0].Bounds : Rectangle.Empty; }
		}
		public Rectangle CollapsedPagesBounds {
			get {
				Rectangle rect = RealLowerBounds;
				if(Pages.Count > 0) {
					rect.X += Pages[0].Bounds.Width;
					rect.Width -= Pages[0].Bounds.Width;
				}
				return rect;
			}
		}
		protected internal virtual bool Animated { get { return XtraAnimator.Current.Get(ViewInfo.Ribbon, Category) != null; } }
		protected internal virtual SkinElementInfo GetPageCategoryUpperInfo(RibbonViewInfo vi, Rectangle bounds) {
			SkinElement se = null;
			if(ViewInfo.Form != null && ViewInfo.Form.IsGlassForm) se = RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinContextTabCategory2];
			else se = RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinContextTabCategory];
			if(se == null || se.Image == null) return null;
			SkinElementInfo info = new SkinElementInfo(se, bounds);
			if(!ViewInfo.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		protected internal int GetPageCategorySeparatorWidth(RibbonViewInfo vi) {
			if(ViewInfo.Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return 0;
			SkinElement se = RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinContextTabCategorySeparator];
			if(se == null || se.Image == null) return 1;
			return se.Image.GetImageBounds(0).Width;
		}
		protected virtual void UpdateMacStyleContextPagesLayout() {
			int x = RealLowerBounds.Right;
			for(int i = Pages.Count - 1; i >= 1; i--) {
				x -= Pages[i].Bounds.Width;
				Pages[i].Bounds = new Rectangle(new Point(x, Pages[i].Bounds.Y), Pages[i].Bounds.Size);
			}
		}
		public virtual void UpdateContextPagesLayout() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) {
				UpdateMacStyleContextPagesLayout();
				return;
			}
			UpdateSimpleContextPagesLayout();
		}
		protected bool AutoStretchPageHeaders { get { return Category.AutoStretchPageHeaders; } }
		protected virtual void UpdateSimpleContextPagesLayout() {
			if(Pages.Count == 0) return;
			int avaliableWidth = CalcUnusedCategorySpace(), stretchError = CalcAutoStretchPageHeadersError();
			int deltaX = Bounds.X - Pages[0].Bounds.X + GetPageCategorySeparatorWidth(ViewInfo);
			for(int i = 0; i < Pages.Count; i++) {
				RibbonPageViewInfo page = Pages[i];
				Rectangle pageBounds = new Rectangle(new Point(page.Bounds.X + deltaX, page.Bounds.Y), page.Bounds.Size);
				if(AutoStretchPageHeaders) {
					int extraWidth = avaliableWidth / Pages.Count;
					pageBounds.Width += extraWidth + (i == Pages.Count - 1 ? stretchError : 0);
					if(i > 0) pageBounds.X += extraWidth * i;
				}
				page.Bounds = pageBounds;
			}
		}
		protected int CalcUnusedCategorySpace() {
			int res = Bounds.Width - 2 * GetPageCategorySeparatorWidth(ViewInfo);
			for(int i = 0; i < Pages.Count; i++) {
				res -= Pages[i].Bounds.Width;
			}
			return Math.Max(0, res);
		}
		protected int CalcAutoStretchPageHeadersError() {
			if(Pages.Count == 0) return 0;
			int avaliableSpace = CalcUnusedCategorySpace();
			if(avaliableSpace == 0) return 0;
			return Math.Max(0, avaliableSpace - (avaliableSpace / Pages.Count) * Pages.Count);
		}
		public RibbonPageViewInfoCollection Pages { get { return pages; } }
		public virtual void ClearPages() { Pages.Clear(); }
		public virtual void InitializePages() {
			ClearPages();
			foreach(RibbonPageViewInfo pageInfo in ViewInfo.Header.Pages) {
				if(pageInfo.Category == Category || pageInfo.Category == Category.MergedCategory) {
					Pages.Add(pageInfo);
					pageInfo.SetCategoryInfo(this);
				}
				else if(Pages.Count > 0) return;
			}
		}
		public virtual void CalcViewInfo() {
			this.bounds = CalcBounds();
			CalcRects();
		}
		protected virtual int CalcTextWidth() {
			return ViewInfo.PaintAppearance.PageCategory.CalcTextSize(ViewInfo.GInfo.Graphics, Category.Text, 0).ToSize().Width;
		}
		public int BestTextWidth { get { return bestTextWidth; } }
		protected virtual void CalcRects() {
			this.bestTextWidth = CalcTextWidth();
			this.upperBounds = CalcUpperBounds();
			this.lowerBounds = CalcLowerBounds();
			this.textBounds = CalcTextBounds();
		}
		protected internal virtual Rectangle GetRealLowerBounds() {
			if(!Category.Expanded && Pages.Count > 0) {
				return Pages[0].Bounds;
			}
			return LowerBounds;
		}
		protected virtual bool ShouldProcessPage(RibbonPage page) {
			return page.ActualVisible && page.PageInfo != null;
		}
		public RibbonPageViewInfoCollection PagesInfo { 
			get {
				if(ViewInfo == null) return null;
				return ViewInfo.Header.Pages; 
			} 
		}
		protected virtual RibbonPageViewInfo FindDefaultCategoryLastPageInfo() {
			for(int i = ViewInfo.Header.Pages.Count - 1; i >= 0; i--) {
				if(ViewInfo.Header.Pages[i].Category == ViewInfo.Ribbon.PageCategories.DefaultCategory) return ViewInfo.Header.Pages[i];
			}
			return null;
		}
		protected virtual RibbonPageViewInfo FindLastNonEmptyCategoryPageInfo(int index) {
			for(int i = index - 1; i >= 0; i--) { 
				RibbonPageViewInfoCollection pageColl = ViewInfo.Header.PageCategories[i].Pages;
				if(pageColl.Count > 0) return pageColl[pageColl.Count - 1];
			}
			return FindDefaultCategoryLastPageInfo();
		}
		protected virtual int GetTop() {
			if(ViewInfo.Form == null || ViewInfo.Form.WindowState != FormWindowState.Maximized || ViewInfo.Form.Bounds.Y >= 0)
				return 0;
			return -ViewInfo.Form.Bounds.Y - 1;
		}
		public virtual Rectangle CalcEmptyCategoryBounds() { 
			int startX = 0;
			int categoryIndex = ViewInfo.Header.PageCategories.IndexOf(this);
			RibbonPageViewInfo page = FindLastNonEmptyCategoryPageInfo(categoryIndex);
			if(page == null) startX = ViewInfo.Header.HeaderLeftIndent;
			else startX = page.Bounds.Right + ViewInfo.Header.IndentBetweenPages;
			return new Rectangle(startX, GetTop(), DefaultPageCategoryWidth, ViewInfo.Header.Bounds.Bottom - 1 - GetTop());
		}
		public virtual Rectangle CalcBounds() { 
			if(Category == null || PagesInfo == null) return Rectangle.Empty;
			if(Pages.Count == 0) return CalcEmptyCategoryBounds();
			int width = GetPageCategorySeparatorWidth(ViewInfo);
			return new Rectangle(Pages[0].Bounds.X - width, GetTop(), Pages[Pages.Count - 1].Bounds.Right - Pages[0].Bounds.X + width * 2, Pages[0].Bounds.Bottom - 1 - GetTop());
		}
		public virtual void UpdateBounds(Rectangle rect) {
			if(Bounds == rect) return;
			this.bounds = rect;
			CalcRects();
		}
		public virtual Rectangle CalcUpperBounds() {
			if(!ViewInfo.ShouldDrawPageCategories()) return CalcLowerBounds();
			Rectangle rect = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (ViewInfo.Header.Bounds.Height - 1));
			if(ViewInfo.Form == null) return rect;
			if(rect.Right > ViewInfo.Caption.ContentBounds.Right - RibbonPageHeaderViewInfo.HeaderToCaptionIndent) rect.Width -= rect.Right - ViewInfo.Caption.ContentBounds.Right + RibbonPageHeaderViewInfo.HeaderToCaptionIndent;
			rect.Width = Math.Max(rect.Width, 0);
			return rect;
		}
		public virtual Rectangle CalcLowerBounds() {
			return new Rectangle(Bounds.X, ViewInfo.Header.Bounds.Y, Bounds.Width, ViewInfo.Header.Bounds.Height - 1);
		}
		public virtual Rectangle CalcTextBounds() {
			if(UpperBounds.Width == 0) return Rectangle.Empty;
			SkinElementInfo info = GetPageCategoryUpperInfo(ViewInfo, UpperBounds);
			if(info == null) return UpperBounds;
			return ObjectPainter.GetObjectClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info);
		}
	}
	public class RibbonPageViewInfo {
		public static int BestWidthIndent = 8;
		public static int IndentWithoutSeparateLine = 0;
		public static int MinIndent = 4;
		public static int DefaultImageToTextIndent = 2;
		Rectangle bounds, imageBounds, textBounds, contentBounds;
		Size textSize;
		RibbonPage page;
		RibbonViewInfo viewInfo;
		int precalculatedWidth, indent;
		RibbonPageCategoryViewInfo categoryInfo;
		public RibbonPageViewInfo(RibbonViewInfo viewInfo, RibbonPage page) {
			this.viewInfo = viewInfo;
			this.bounds = Rectangle.Empty;
			this.page = page;
			this.precalculatedWidth = 0;
		}
		internal int PrecalculatedWidth { get { return precalculatedWidth; } set { precalculatedWidth = value; } }
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		public RibbonPage Page { get { return page; } }
		public RibbonPageCategory Category { get { return page.Category; } }
		public RibbonPageCategoryViewInfo CategoryInfo { get { return categoryInfo; } }
		internal void SetCategoryInfo(RibbonPageCategoryViewInfo categoryInfo) { this.categoryInfo = categoryInfo; }
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ImageBounds { get { return imageBounds; } set { imageBounds = value; } }
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; } }
		public Size TextSize { get { return textSize; } }
		public int Indent { get { return indent; } set { indent = value; } }
		public virtual void CalcViewInfo() {
			CalcBounds();
		}
		public virtual AppearanceObject PaintAppearance {
			get {
				AppearanceObject pageHeader = page.Ribbon.ViewInfo.PaintAppearance.PageHeader;
				AppearanceObject paintAppearance = new AppearanceObject(pageHeader);
				paintAppearance.Font = page.Appearance.Options.UseFont ? page.Appearance.Font : pageHeader.Font;
				paintAppearance.ForeColor = page.Appearance.Options.UseForeColor ? page.Appearance.ForeColor : pageHeader.ForeColor;
				return GetHeaderPageInfo().Element.GetForeColorAppearance(paintAppearance, page.Ribbon.ViewInfo.OwnerControl.Enabled ? GetHeaderPageInfo().State : ObjectState.Disabled);
			} 
		}
		public virtual ObjectState CalcState() {
			ObjectState res = ObjectState.Normal;
			if(ViewInfo.HotObject.Page == Page) res |= ObjectState.Hot;
			if(ViewInfo.SelectedPage == Page && IsAllowSelectedState) {
				res |= ObjectState.Selected;
			}
			if(ViewInfo.KeyboardActiveInfo.Page == page) res |= ObjectState.Hot;
			return res;
		}
		protected bool IsApplicationButton2010ContentOpened {
			get {
				if(ViewInfo.Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007)
					return false;
				return ViewInfo.IsApplicationMenuOpened;
			}
		}
		protected internal virtual bool IsAllowSelectedState {
			get {
				if(IsApplicationButton2010ContentOpened) return false;
				if(!ViewInfo.GetIsMinimized()) return true;
				if(ViewInfo.Ribbon == null) return true;
				if(ViewInfo.Ribbon.MinimizedRibbonPopupForm != null) return true;
				return ViewInfo.Ribbon.MinimizedRibbonPopupForm != null;
			}
		}
		public virtual SkinElementInfo GetHeaderPageInfo() {
			return GetHeaderPageInfo(false);
		}
		public virtual SkinElementInfo GetHeaderPageMaskInfo() {
			string name;
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) 
				name = RibbonSkins.SkinContextMacTabHeaderPageMask;
			else  
				name = RibbonSkins.SkinContextTabHeaderPageMask;
			SkinElement element = RibbonSkins.GetSkin(ViewInfo.Provider)[name];
			if(element == null)
				element = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinContextTabHeaderPageMask];
			if(element == null) return null;
			SkinElementInfo info = new SkinElementInfo(element, Bounds);
			UpdareImageIndexByState(info);
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		protected void UpdareImageIndexByState(SkinElementInfo info) {
			ObjectState state = CalcState();
			info.State = state;
			if ((state & ObjectState.Selected) != 0)
			{
				info.ImageIndex = 3;
				if ((state & ObjectState.Hot) != 0) info.ImageIndex = 4;
				return;
			}
			if ((state & ObjectState.Hot) != 0) info.ImageIndex = 1;
		}
		public virtual SkinElementInfo GetHeaderPageInfo(bool getDefault) {
			string pageInfoString = string.Empty;
			bool isContextTabPage = (Category == null || !Category.IsDefaultColor) && !getDefault;
			SkinElement element = null;
			if(ViewInfo.Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice) { 
				pageInfoString = isContextTabPage? RibbonSkins.SkinMacContextTabHeaderPage: RibbonSkins.SkinMacTabHeaderPage;
				element = RibbonSkins.GetSkin(ViewInfo.Provider)[pageInfoString];
			}
			if(element == null) {
				pageInfoString = isContextTabPage? RibbonSkins.SkinContextTabHeaderPage: RibbonSkins.SkinTabHeaderPage;
				element = RibbonSkins.GetSkin(ViewInfo.Provider)[pageInfoString];
			}
			SkinElementInfo res = new SkinElementInfo(element, Bounds);
			UpdareImageIndexByState(res);
			if(!ViewInfo.Ribbon.Enabled)
				res.Attributes = PaintHelper.RibbonDisabledAttributes;
			res.RightToLeft = ViewInfo.IsRightToLeft;
			return res;
		}
		protected virtual void CalcBounds() {
			ContentBounds = CalcContentBounds();
			this.imageBounds.Size = GetImageSize();
			this.textSize = this.textBounds.Size = GetTextSize();
			Indent = CalcIndent();
			LayoutImage();
			LayoutText();
		}
		protected virtual Rectangle CalcContentBounds() {
			SkinElementInfo info = GetHeaderPageInfo(false);
			Rectangle rect = ObjectPainter.GetObjectClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info);
			if(ViewInfo.Ribbon.AllowTrimPageText) {
				int width = CalcMinWidthWithText();
				if(rect.Width < width) {
					if(rect.Width + info.Element.ContentMargins.Left + info.Element.ContentMargins.Right > width + MinIndent * 2)
						rect.Width = width;
					else 
						rect.Width = Bounds.Width - MinIndent * 2;
					rect.X = Bounds.X + (Bounds.Width - rect.Width) / 2;
				}
			}
			return rect;
		}
		protected virtual int CalcIndent() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) return 0;
			int width = TextBounds.Width + GetImageAreaWidth();
			width = (ContentBounds.Width - width);
			if(width < MinIndent) width = MinIndent;
			return width / 2;
		}
		protected virtual void LayoutText() {
			this.textBounds.Location = new Point(ContentBounds.X + Indent, ContentBounds.Y + (ContentBounds.Height - TextBounds.Height) / 2);
			this.textBounds.Width = ContentBounds.Width - Indent * 2;
			if(Page.ImageAlign != HorzAlignment.Center)
				this.textBounds.Width -= GetImageAreaWidth();
			if(ImageBounds.Width == 0 || Page.ImageAlign == HorzAlignment.Center) {
				return;
			}
			else if(Page.ImageAlign == HorzAlignment.Far)
				this.textBounds.X = ImageBounds.X - ImageToTextIndent - TextBounds.Width;
			else
				this.textBounds.X = ImageBounds.Right + ImageToTextIndent;
		}
		protected virtual void LayoutImage() {
			this.imageBounds.Location = new Point(ContentBounds.X, ContentBounds.Y + (ContentBounds.Height - ImageBounds.Height) / 2);
			if(ImageBounds.Size == Size.Empty) 
				return;
			imageBounds.Width = Math.Min(imageBounds.Width, ContentBounds.Width - Indent * 2);
			if(Page.ImageAlign == HorzAlignment.Far)
				this.imageBounds.X = ContentBounds.Right - Indent - ImageBounds.Width;
			else if(Page.ImageAlign == HorzAlignment.Center)
				this.imageBounds.X = ContentBounds.X + (ContentBounds.Width - ImageBounds.Width) / 2;
			else
				this.imageBounds.X = ContentBounds.X + Indent;
		}
		protected internal virtual int GetTextAscentHeight() { 
			ViewInfo.GInfo.AddGraphics(null);
			try {
				return TextUtils.GetFontAscentHeight(ViewInfo.GInfo.Graphics, ViewInfo.PaintAppearance.PageHeader.Font);
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual Size GetTextSize() {
			ViewInfo.GInfo.AddGraphics(null);
			try {
				if(ViewInfo.IsTransparentPagesOnGlassForm && !(DevExpress.Utils.Paint.XPaint.Graphics is DevExpress.Utils.Paint.XPaintMixed)) {
					using(StringFormat sf = (StringFormat)ViewInfo.PaintAppearance.PageHeader.TextOptions.GetStringFormat().Clone()) {
						sf.Alignment = StringAlignment.Center;
						sf.Trimming = StringTrimming.Character;
						sf.FormatFlags |= StringFormatFlags.NoWrap;
						Size res = ObjectPainter.CalcTextSizeOnGlass(ViewInfo.GInfo.Graphics, PaintAppearance.Font, Page.Text, Rectangle.Empty, sf);
						res.Height = Math.Max(res.Height, ViewInfo.PaintAppearance.PageHeader.CalcTextSize(ViewInfo.GInfo.Graphics, Page.Text, 0).ToSize().Height);
						return res;
					}
				}
				return PaintAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, Page.Text, 0).ToSize();
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual Size GetImageSize() { 
			Image img = Page.GetImage();
			return img == null? Size.Empty: img.Size;
		}
		int ImageToTextIndent { get { return Page.ImageToTextIndent == -1 ? DefaultImageToTextIndent : Page.ImageToTextIndent; } }
		protected virtual int GetTextWidth() { return GetTextSize().Width + 1; }
		protected virtual int GetImageAreaWidth() { 
			Size imgSize = GetImageSize();
			return imgSize.Width == 0 ? 0 : imgSize.Width + ImageToTextIndent;
		}
		protected virtual int GetImageAndTextAreaWidth() {
			if(Page.ImageAlign == HorzAlignment.Center) return Math.Max(GetTextWidth(), GetImageAreaWidth());
			return GetTextWidth() + GetImageAreaWidth();
		}
		public virtual int CalcBestWidth() {
			int widthIndent = ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice ? 0 : BestWidthIndent;
			int width = Math.Max(ViewInfo.Ribbon.PageHeaderMinWidth, GetImageAndTextAreaWidth() + widthIndent) + 1;
			SkinElementInfo info = GetHeaderPageInfo(false);
			return ObjectPainter.CalcBoundsByClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(0, 0, width, 1)).Width;
		}
		bool ShouldCollapsePage {
			get { return IsCollapsedCategory && Page.Category.Pages.IndexOf(Page) != 0; }
		}
		bool IsCollapsedCategory {
			get {
				return ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice && !Page.Category.Expanded;
			}
		} 
		public virtual int CalcWidthWithoutSeparateLine() {
			return Math.Max(ViewInfo.Ribbon.PageHeaderMinWidth, GetImageAndTextAreaWidth() + IndentWithoutSeparateLine);
		}
		public virtual int CalcMinWidthWithText() {
			int width = Math.Max(ViewInfo.Ribbon.PageHeaderMinWidth, GetImageAndTextAreaWidth() + (ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice? 0: MinIndent));
			if(ViewInfo.Ribbon.AllowTrimPageText)
				return width;
			SkinElementInfo info = GetHeaderPageInfo(false);
			return ObjectPainter.CalcBoundsByClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(0, 0, width, 1)).Width;
		}
		protected internal virtual void UpdateLayout() {
			ContentBounds = CalcContentBounds();
			LayoutImage();
			LayoutText();
		}
	}
	public class RibbonPageGroupViewInfoCollection : CollectionBase {
		public void Add(RibbonPageGroupViewInfo pageInfo) { List.Add(pageInfo); }
		public void Insert(RibbonPageGroupViewInfo pageInfo, int index) { List.Insert(index, pageInfo); }
		public RibbonPageGroupViewInfo this[int index] { get { return List[index] as RibbonPageGroupViewInfo; } }
		public RibbonPageGroupViewInfo this[RibbonPageGroup pageGroup] {
			get {
				for(int n = 0; n < Count; n++) {
					if(this[n].PageGroup == pageGroup) return this[n];
				}
				return null;
			}
		}
		public RibbonItemViewInfo FindItem(IRibbonItem item, Rectangle bounds) {
			for(int n = 0; n < Count; n++) {
				RibbonItemViewInfo info = this[n].Items.Find(item, bounds);
				if(info != null) return info;
			}
			return null;
		}
		public int IndexOf(RibbonPageGroupViewInfo group) { return List.IndexOf(group); }
	}
	public interface IRibbonGroupInfo {
		RibbonItemViewInfoCollection Items { get; }
		Rectangle ContentBounds { get; set; }
		bool IsButtonGroup(int index);
		RibbonViewInfo ViewInfo { get;}
		int DefaultIndentBetweenButtonGroups { get; }
		int DefaultIndentBetweenColumns { get; }
		VertAlignment ButtonGroupsVertAlign { get; }
		VertAlignment ItemsVertAlign { get; }
		bool AllowMinimize { get; }
		bool Minimized { get; }
		int PrecalculatedWidth { get; set; }
		int CalcMinimizedWidth();
		void Minimize();
		int CalcBestWidth(int contentHeight);
		RibbonSeparatorItemViewInfo CreateSeparatorViewInfo(IRibbonItem item);
		bool AllowHorizontalCenterdItems { get; }
		bool HasLargeItems { get; }
		bool HasGallery { get; }
		int LargeRibbonButtonHeight { get; }
		bool IsSingleLineLargeButton { get; }
		int GroupContentHeight { get; }
		BarItemLinkCollection ItemLinks { get; }
		BarButtonItemLink ContentButtonLink { get; }
		RibbonPageGroupItemsLayout ItemsLayout { get; }
	}
	public class RibbonPageGroupViewInfo : IRibbonGroupInfo, IGroupBoxButtonsPanelOwner {
		Rectangle bounds, contentBounds;
		RibbonPageGroup pageGroup;
		RibbonItemViewInfoCollection items;
		GroupObjectInfoArgs drawInfo;
		int bestCaptionAreaWidth = 0;
		int precalculatedWidth;
		bool minimized;
		RibbonViewInfo viewInfo;
		ObjectPainter painter;
		RibbonItemViewInfo contentButtonViewInfo;
		public RibbonPageGroupViewInfo(RibbonViewInfo viewInfo, RibbonPageGroup group) {
			this.viewInfo = viewInfo;
			this.items = new RibbonItemViewInfoCollection(this);
			this.precalculatedWidth = 0;
			this.pageGroup = group;
			this.contentBounds = this.bounds = Rectangle.Empty;
			this.minimized = false;
			this.drawInfo = new GroupObjectInfoArgs();
		}
		protected internal RibbonItemViewInfo ContentButtonViewInfo {
			get {
				if(contentButtonViewInfo == null) {
					contentButtonViewInfo = CreateContentButtonViewInfo();
					contentButtonViewInfo.Owner = this;
				}
				return contentButtonViewInfo;
			}
		}
		RibbonPageGroupItemsLayout IRibbonGroupInfo.ItemsLayout { get { return PageGroup.ItemsLayout; } }
		BarButtonItemLink IRibbonGroupInfo.ContentButtonLink {
			get { return PageGroup.ContentButtonLink; }
		}
		public bool HasGallery { get { return HasGalleryCore(this); } }
		public int GroupContentHeight { get { return ViewInfo.GroupContentHeight; } }
		public bool IsSingleLineLargeButton { get { return false; } }
		public int LargeRibbonButtonHeight { get { return ViewInfo.LargeButtonHeight; } }
		public bool HasLargeItems { get { return HasLargeItemsCore(this); } }
		public bool AllowHorizontalCenterdItems { get { return true; } }
		public bool AllowMinimize { 
			get {
				if(Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
					return false;
				return PageGroup.AllowMinimize; 
			} 
		}
		public BarItemLinkCollection ItemLinks { get { return PageGroup.ItemLinks; } }
		public int DefaultIndentBetweenColumns { get { return ViewInfo.DefaultIndentBetweenColumns; } }
		public int DefaultIndentBetweenButtonGroups { get { return ViewInfo.DefaultIndentBetweenButtonGroups; } }
		internal static bool IsButtonGroup(IRibbonGroupInfo groupInfo, int index) {
			if(index < 0 || index >= groupInfo.Items.Count) return false;
			BarItemLink link = groupInfo.Items[index].Item as BarItemLink;
			return !(groupInfo.Items[index] is RibbonSeparatorItemViewInfo) && link.ActAsButtonGroup && !groupInfo.Items[index].IsLargeButton;
		}
		internal static bool HasGalleryCore(IRibbonGroupInfo groupInfo) {
			foreach(RibbonItemViewInfo item in groupInfo.Items) {
				if(item is InRibbonGalleryRibbonItemViewInfo)
					return true;
			}
			return false;
		}
		public bool IsButtonGroup(int index) {
			return IsButtonGroup(this, index);
		}
		public ObjectPainter Painter {
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			}
		}
		protected RibbonControl Ribbon { get { return ViewInfo.Ribbon; } }
		public virtual bool IsDroppedDown {
			get {
				if(Ribbon == null) return false;
				return Ribbon.IsPopupGroupOpened && Ribbon.PopupGroupForm.Group == PageGroup;
			}
		}
		protected internal virtual void UpdateSubLinkPopupMenuLocationInDesign(BarItemLink link) {
			if(!link.Manager.IsCustomizing) return;
			BarSubItemLink subLink = link as BarSubItemLink;
			if(subLink == null || !subLink.Opened || subLink.RibbonItemInfo == null) return;
			Point pt = new Point(subLink.RibbonItemInfo.Bounds.X, subLink.RibbonItemInfo.Bounds.Bottom);
			subLink.SubControl.Form.Location = ViewInfo.Ribbon.PointToScreen(pt);
		}
		public VertAlignment ItemsVertAlign { 
			get {
				if(ViewInfo.Ribbon.ItemsVertAlign == VertAlignment.Default) return VertAlignment.Center;
				return ViewInfo.Ribbon.ItemsVertAlign;
			} 
		}
		public VertAlignment ButtonGroupsVertAlign {
			get {
				if(ViewInfo.Ribbon.ButtonGroupsVertAlign == VertAlignment.Default) return VertAlignment.Center;
				return ViewInfo.Ribbon.ButtonGroupsVertAlign;
			}
		}
		public virtual int GetGroupKeyTipYPos(int rowPos) {
			RibbonPageGroupLayoutCalculator calc = ViewInfo.CreateGroupLayoutCalculator(this);
			if(rowPos == 0) return ContentBounds.Top + calc.IndentBetweenTwoItems;
			else if(rowPos == 1) return ContentBounds.Top + ContentBounds.Height / 2;
			else if(rowPos == 2) return ContentBounds.Bottom - calc.IndentBetweenTwoItems;
			return 0;
		}
		public virtual BarItemLink GetLinkByPoint(Point pt, bool enterButtonGroup) {
			foreach(RibbonItemViewInfo itemInfo in Items) {
				if(itemInfo is RibbonSeparatorItemViewInfo) continue;
				if(itemInfo.Bounds.Contains(pt)) { 
					RibbonButtonGroupItemViewInfo buttonInfo = itemInfo as RibbonButtonGroupItemViewInfo;
					if(buttonInfo != null && enterButtonGroup) return buttonInfo.GetLinkByPoint(pt);
					return itemInfo.Item as BarItemLink;
				}
			}
			return null;
		}
		public virtual ObjectState CalcState() {
			ObjectState res = ObjectState.Normal;
			if(ViewInfo.HotObject.InPageGroup && ViewInfo.HotObject.PageGroup == PageGroup) res |= ObjectState.Hot;
			if(IsDroppedDown || ViewInfo.PressedObject.InPageGroup && ViewInfo.PressedObject.PageGroup == PageGroup) res |= ObjectState.Pressed;
			return res;
		}
		internal int ForceStateImageIndex = -1;
		protected internal GroupObjectInfoArgs DrawInfo { get { return drawInfo; } }
		protected virtual bool AllowTopCaptionLocation {
			get {
				object res = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.OptPageGroupCaptionLocationTop];
				if(res == null)
					return true;
				return (bool)res;
			}
		}
		protected internal virtual void UpdateDrawInfo(GroupObjectInfoArgs res) {
			ObjectState state = CalcState();
			if(Minimized) res.CaptionBounds = Rectangle.Empty;
			res.AppearanceCaption.Reset();
			res.AppearanceCaption.Assign(ViewInfo.PaintAppearance.PageGroupCaption);
			res.StateIndex = 0;
			res.ButtonState = !Ribbon.Enabled ? ObjectState.Disabled : ObjectState.Normal;
			if((state & ObjectState.Hot) != 0) res.StateIndex = 1;
			if(Minimized && (state & ObjectState.Pressed) != 0) res.StateIndex = 2;
			if(ForceStateImageIndex != -1) res.StateIndex = ForceStateImageIndex;
			if(ViewInfo.HotObject.PageGroup == PageGroup && ViewInfo.HotObject.HitTest == RibbonHitTest.PageGroupCaptionButton) res.ButtonState |= ObjectState.Hot;
			if(ViewInfo.PressedObject.PageGroup == PageGroup && ViewInfo.PressedObject.HitTest == RibbonHitTest.PageGroupCaptionButton) res.ButtonState |= ObjectState.Pressed;
			if(ViewInfo.KeyboardActiveInfo.PageGroup == PageGroup && ViewInfo.KeyboardActiveInfo.HitTest == RibbonHitTest.PageGroupCaptionButton) res.ButtonState |= ObjectState.Hot;
			if(ViewInfo.Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice && AllowTopCaptionLocation)
				res.CaptionLocation = Locations.Top;
			res.RightToLeft = ViewInfo.IsRightToLeft;
		}
		public Rectangle ButtonBounds { get { return DrawInfo.ButtonBounds; } }
		public void UpdateImageAttributes(ImageAttributes attributes) {
			DrawInfo.Attributes = attributes;
		}
		public virtual int BestCaptionAreaWidth {
			get {
				if(bestCaptionAreaWidth == 0) bestCaptionAreaWidth = CalcCaptionAreaBestWidth();
				return bestCaptionAreaWidth;
			}
		}
		public GroupObjectInfoArgs GetDrawInfo() {
			UpdateDrawInfo(DrawInfo);
			return DrawInfo;
		}
		protected internal virtual SkinElementInfo GetBackgroundInfo() {
			GroupObjectInfoArgs e = GetDrawInfo();
			Rectangle rect = e.Bounds;
			rect.Height -= e.CaptionBounds.Height;
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinTabPanelGroupBody], rect);
			if(!ViewInfo.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		protected internal virtual void CalcHitInfo(RibbonHitInfo hitInfo) {
			if(hitInfo.ContainsSet(CaptionBounds, RibbonHitTest.PageGroupCaption)) {
				hitInfo.ContainsSet(DrawInfo.ButtonBounds, RibbonHitTest.PageGroupCaptionButton);
				return;
			}
			if(Minimized) return;
			for(int n = 0; n < Items.Count; n++) {
				RibbonItemViewInfo itemInfo = Items[n];
				if(itemInfo.Bounds.Contains(hitInfo.HitPoint)) {
					itemInfo.CalcHitInfo(hitInfo);
					continue;
				}
			}
		}
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		protected GraphicsInfo GInfo { get { return ViewInfo.GInfo; } }
		protected virtual ObjectPainter CreatePainter() { return new SkinRibbonGroupPainterEx(ViewInfo.Provider, ViewInfo); }
		protected virtual GroupObjectInfoArgs SetupDrawArgs() {
			GroupObjectInfoArgs res = new GroupObjectInfoArgs();
			res.SetButtonsPanelOwner(this);
			res.Bounds = Bounds;
			res.CaptionLocation = Locations.Bottom;
			res.ShowCaption = ViewInfo.ShouldDrawGroupCaption;
			res.Tag = this;
			res.Caption = PageGroup.Text;
			if(ViewInfo.ShouldDrawGroupCaption && PageGroup.ShowCaptionButton) {
				res.ButtonsPanel.BeginUpdate();
				res.ButtonsPanel.Buttons.Add(new XtraEditors.ButtonsPanelControl.RibbonPageGroupButton(res));
				res.ButtonsPanel.CancelUpdate();
			}
			res.ButtonLocation = GroupElementLocation.AfterText;
			res.StateIndex = 0;
			return res;
		}
		protected virtual void UpdateTextBounds() {
			Rectangle rect = DrawInfo.TextBounds;
			rect.Offset(0, -1);
			DrawInfo.TextBounds = rect;
		}
		protected virtual void CalcDrawArgs() {
			this.drawInfo = SetupDrawArgs();
			ObjectPainter.CalcObjectBounds(GInfo.Graphics, Painter, GetDrawInfo());
			UpdateTextBounds();
		}
		protected virtual int CalcCaptionAreaBestWidth() {
			GroupObjectInfoArgs args = new GroupObjectInfoArgs();
			args.Caption = PageGroup.Text;
			args.Bounds = new Rectangle(0, 0, 1000, 300);
			args.SetButtonsPanelOwner(this);
			if(PageGroup.ShowCaptionButton) {
				args.ButtonsPanel.BeginUpdate();
				args.ButtonsPanel.Buttons.Add(new XtraEditors.ButtonsPanelControl.RibbonPageGroupButton(args));
				args.ButtonsPanel.CancelUpdate();
			}
			args.ButtonLocation = GroupElementLocation.AfterText;
			args.Tag = this;
			ObjectPainter.CalcObjectBounds(GInfo.Graphics, Painter, args);
			int bestWidth = args.TextBounds.Width;
			SkinRibbonGroupPainterEx p = Painter as SkinRibbonGroupPainterEx;
			if(p != null) {
				SkinPaddingEdges edges = p.GetCaptionMargins(args);
				bestWidth += edges.Width;
				if(ShowCaptionButton) {
					bestWidth += p.ButtonToBorderDistance;
					bestWidth += args.ButtonBounds.Width + p.ButtonToTextBlockDistance;
				}
			}
			return bestWidth;
		}
		bool IsSmallItem(RibbonItemViewInfo item) {
			RibbonButtonItemViewInfo button = item as RibbonButtonItemViewInfo;
			if(button != null && button.CurrentLevel == RibbonItemStyles.Large) return false;
			if(item is InRibbonGalleryRibbonItemViewInfo) return false;
			return true;
		}
		bool CanInsertItem(int index) {
			if(index == 0) return false;
			int smallItemsCount = 0;
			for(int i = index - 1; i >= 0; i--, smallItemsCount++) {
				if(!IsSmallItem(Items[i])) break;
			}
			if((smallItemsCount % 3) == 0) return false;
			return true;
		}
		bool CanInsertItemAfterItem(int index) {
			if(index + 1 >= Items.Count) return false;
			if(index == 0) return false;
			int smallItemsCount = 0;
			for(int i = index + 1; i < Items.Count; i++, smallItemsCount++) {
				if(!IsSmallItem(Items[i])) break;
			}
			if((smallItemsCount % 3) == 0) return false;
			return true;
		}
		public bool Minimized { get { return minimized; } }
		public bool ShowPopup { get { return true; } }
		protected internal virtual void ShowContentDropDownCore(RibbonPageGroupViewInfo groupInfo, RibbonToolbarPopupItemLink toolbarLink) {
			PageGroup.ShowContentDropDownCore(groupInfo, toolbarLink);
		}
		PopupMenu contentMenu;
		protected PopupMenu ContentMenu {
			get {
				if(contentMenu == null)
					contentMenu = new PopupMenu(Ribbon.Manager);
				return contentMenu;
			}
		}
		public virtual void ShowContentMenu(BarItemLink link) {
			ContentMenu.ItemLinks.Clear();
			ContentMenu.MenuCaption = PageGroup.Text;
			List<BarItemLink> itemLinks = new List<BarItemLink>();
			foreach(BarItemLink itemLink in PageGroup.ItemLinks)
				itemLinks.Add(itemLink);
			List<BarItemLink> links = ProcessLinksByRibbonStyle(this, itemLinks);
			foreach(BarItemLink child in links) {
				if(IsLinkCollapsed(child))
					ContentMenu.ItemLinks.Add(child.Item);
			}
			Point startPoint = new Point(link.RibbonItemInfo.Bounds.X + link.RibbonItemInfo.Bounds.Width / 2, link.RibbonItemInfo.Bounds.Bottom);
			ContentMenu.ShowPopup(link.Ribbon.PointToScreen(startPoint));
		}
		private bool IsLinkCollapsed(BarItemLink link) {
			foreach(RibbonItemViewInfo itemInfo in Items) {
				if(itemInfo.Item == link)
					return false;
			}
			return true;
		}
		public BarButtonItem ContentButton { get { return PageGroup.ContentButton; } }
		public BarButtonItemLink ContentButtonLink { get { return PageGroup.ContentButtonLink; } }
		public virtual void ShowContentDropDown() {
			if(ContentButton == null || !Minimized) return;
			ShowContentDropDownCore(PageGroup.PopupGroupInfo, null);
		}
		protected internal RibbonItemViewInfo CreateContentButtonViewInfo() {
			return ViewInfo.CreateItemViewInfo(ContentButtonLink);
		}
		public void Minimize() {
			if(Minimized || !ViewInfo.IsAllowGroupMinimize(this)) return;
			Items.Clear();
			Items.Add(ContentButtonViewInfo);
			minimized = true;
		}
		public RibbonItemViewInfoCollection Items { get { return items; } }
		public RibbonPageGroup PageGroup { get { return pageGroup; } }
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual Rectangle CaptionBounds { get { return DrawInfo.CaptionBounds; } set { DrawInfo.CaptionBounds = value; } }
		public virtual Rectangle CaptionSeparatorBounds { get; set; }
		public virtual Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public virtual void CalcViewInfo(Rectangle bounds) {
			GInfo.AddGraphics(null);
			try {
				Bounds = ViewInfo.IsRightToLeft ? BarUtilites.ConvertBoundsToRTL(bounds, Ribbon.Bounds) : bounds;
				CalcDrawArgs();
				ContentBounds = CalcContentBounds(Bounds);
				CalcGroupContent();
				if(Minimized) CalcPopupGroupInfo(bounds);
				CheckPageGroupRTL();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CheckPageGroupRTL() {
			if(!ViewInfo.IsRightToLeft) return;
			foreach(RibbonItemViewInfo item in Items)
				item.Bounds = BarUtilites.ConvertBoundsToRTL(item.Bounds, ContentBounds);
		}
		protected virtual Rectangle CalcCaptionBounds() {
			return new Rectangle(ContentBounds.X + ViewInfo.DefaultPageGroupCaptionHorizontalIndent, CaptionSeparatorBounds.Bottom + ViewInfo.DefaultPageGroupCaptionVerticalIndent, ContentBounds.Width - ViewInfo.DefaultPageGroupCaptionHorizontalIndent * 2, ViewInfo.GroupCaptionHeight);
		}
		protected virtual Rectangle CalcContentBounds(Rectangle bounds) {
			if(ViewInfo.ShouldDrawGroupBorder) return DrawInfo.ControlClientBounds;
			bounds.Inflate(0, -DefaultIndentBetweenColumns);
			bounds.X += DefaultIndentBetweenColumns;
			return bounds;
		}
		protected internal void CalcPopupGroupInfo() {
			CalcPopupGroupInfo(new Rectangle(Bounds.Left, Bounds.Top, PageGroup.PopupGroupInfo.CalcBestWidth(ContentBounds.Height), Bounds.Height));
		}
		protected internal void CalcPopupGroupInfo(Rectangle rect) {
			PageGroup.PopupGroupInfo.CalcViewInfo(rect);
		}
		protected virtual int CalcBestGroupContent(int contentHeight) {
			CreateItemsViewInfo();
			RibbonPageGroupLayoutCalculator calc = ViewInfo.CreateGroupLayoutCalculator(this);
			calc.CalcSimpleLayout();
			int bestWidth = calc.UpdateGroupLayout(new Rectangle(0, 0, 0, contentHeight));
			if(!PageGroup.AllowTextClipping) bestWidth = Math.Max(bestWidth, BestCaptionAreaWidth);
			return bestWidth;
		}
		protected virtual void CalcGroupContent() {
			CreateItemsViewInfo();
			RibbonPageGroupLayoutCalculator calc = ViewInfo.CreateGroupLayoutCalculator(this);
			calc.CalcSimpleLayout();
			calc.UpdateGroupLayout(ContentBounds);
		}
		public virtual RibbonSeparatorItemViewInfo CreateSeparatorViewInfo(IRibbonItem item) {
			return new RibbonSeparatorItemViewInfo(this, item);
		}
		internal static void CreateItemsViewInfo(IRibbonGroupInfo groupInfo, List<BarItemLink> itemLinks) {
			if(groupInfo.Items.Count > 0) return;
			List<BarItemLink> plainLinks = ProcessLinksByRibbonStyle(groupInfo, itemLinks);
			for(int i = 0; i < plainLinks.Count; i++) {
				IRibbonItem item = plainLinks[i];
				if(!groupInfo.ViewInfo.ShouldCreateItemInfo(plainLinks[i])) continue;
				RibbonItemViewInfo itemInfo = groupInfo.ViewInfo.AllowCachedItemInfo ? item.GetCachedViewInfo() : null;
				BaseRibbonViewInfo ribbonInfo = groupInfo as BaseRibbonViewInfo;
				if(ribbonInfo == null) ribbonInfo = groupInfo.ViewInfo;
				if(itemInfo == null) {
					itemInfo = ribbonInfo.CreateItemViewInfo(item);
					if(itemInfo != null) item.OnViewInfoCreated(itemInfo);
				}
				if(itemInfo == null) continue;
				itemInfo.Owner = groupInfo;
				if(ShouldAddSeperator(groupInfo, item, itemInfo))
					groupInfo.Items.Add(groupInfo.CreateSeparatorViewInfo(item));
				groupInfo.Items.Add(itemInfo);
			}
		}
		static bool ShouldAddSeperator(IRibbonGroupInfo groupInfo, IRibbonItem item, RibbonItemViewInfo itemInfo) {
			if(!HasPrevLinks(groupInfo, itemInfo))
				return false;
			if(item.BeginGroup)
				return true;
			if(groupInfo.ViewInfo.GetRibbonStyle() != RibbonControlStyle.TabletOffice ||
				groupInfo.Items.Count == 0 ||
				groupInfo.Items[groupInfo.Items.Count - 1] is RibbonSeparatorItemViewInfo)
				return false;
			BarItemLink link = (BarItemLink)item;
			BarItemLink prevLink = groupInfo.Items.Count > 0? (BarItemLink)groupInfo.Items[groupInfo.Items.Count - 1].Item: null;
			return prevLink != null && prevLink.Holder != link.Holder;
		}
		internal static List<BarItemLink> ProcessLinksByRibbonStyle(IRibbonGroupInfo groupInfo, List<BarItemLink> itemLinks) {
			List<BarItemLink> res = new List<BarItemLink>();
			foreach(BarItemLink link in itemLinks) {
				if(link is BarButtonGroupLink && groupInfo.ViewInfo.IsOfficeTablet) {
					foreach(BarItemLink child in ((BarButtonGroupLink)link).InplaceLinks)
						res.Add(child);
				}
				else
					res.Add(link);
			}
			return res;
		}
		protected internal virtual void CreateItemsViewInfo() {
			List<BarItemLink> links = InplaceLinksHelper.GetLinks(PageGroup.Manager, PageGroup.ItemLinks, Ribbon.AllowInplaceLinks, Ribbon.IsDesignMode, (link) => ViewInfo.ShouldCreateItemInfo(link));
			CreateItemsViewInfo(this, links);
		}
		static bool HasPrevLinks(IRibbonGroupInfo groupInfo, RibbonItemViewInfo vi) {
			foreach(RibbonItemViewInfo item in groupInfo.Items) {
				if(item == vi) break;
				BarItemLink link = item.Item as BarItemLink;
				if(link.Visible)
					return true;
			}
			return false;
		}
		public virtual int Reduce() {
			return Reduce(null);
		}
		Size groupContentDeltaSize;
		public Size CalcSizeByContentSize(Size size) {
			if(!ViewInfo.ShouldDrawGroupBorder)
				return CalcSizeByContentSizeNoBorder(size);
			if(groupContentDeltaSize != Size.Empty) {
				return size += groupContentDeltaSize;
			}
			GroupObjectInfoArgs info = SetupDrawArgs();
			UpdateDrawInfo(info);
			info.Bounds = new Rectangle(0, 0, 500, 500);
			info.ControlClientBounds = Rectangle.Empty;
			info.IsReady = false;
			groupContentDeltaSize = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, Painter, info, new Rectangle(Point.Empty, size)).Size;
			groupContentDeltaSize -= size;
			return size += groupContentDeltaSize;
		}
		protected virtual Size CalcSizeByContentSizeNoBorder(Size size) {
			return new Size(size.Width, size.Height + ViewInfo.DefaultIndentBetweenColumns * 2);
		}
		protected virtual int CalcCaptionSeparatorHeight() {
			SkinElementInfo info = GetCaptionSeparatorInfo();
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, info).Size.Height;
		}
		protected virtual int CalcWidthByContentWidth(int contentWidth) {
			return CalcSizeByContentSize(new Size(contentWidth, 100)).Width;
		}
		public virtual int CalcBestWidth(int contentHeight) {
			Rectangle r = ContentBounds;
			GInfo.AddGraphics(null);
			try {
				this.contentBounds.Height = contentHeight;
				return CalcWidthByContentWidth(Math.Max(CalcMinContentWidth(), CalcBestGroupContent(contentHeight)));
			}
			finally {
				GInfo.ReleaseGraphics();
				ContentBounds = r;
			}
		}
		protected virtual int CalcMinContentWidth() {
			if(!ViewInfo.IsAllowGroupMinimize(this)) return 10;
			int minWidth = CalcMinimizedContentWidth();
			if(!PageGroup.AllowTextClipping || PageGroup.ItemLinks.Count == 0) {
				minWidth = Math.Max(minWidth, BestCaptionAreaWidth);
			}
			return minWidth;
		}
		public virtual int CalcMinimizedContentWidth() {
			if(!ViewInfo.IsAllowGroupMinimize(this)) return 10;
			return ContentButtonViewInfo.CalcBestSize().Width;
		}
		public virtual int CalcMinimizedWidth() {
			if(!ViewInfo.IsAllowGroupMinimize(this)) return 10;
			bool oldMinimized = this.minimized;
			this.minimized = true;
			int width = CalcWidthByContentWidth(CalcMinContentWidth());
			this.minimized = oldMinimized;
			return width;
		}
		public virtual int CalcMinWidth() {
			if(!ViewInfo.IsAllowGroupMinimize(this)) return 10;
			return CalcWidthByContentWidth(CalcMinContentWidth());
		}
		public virtual int PrecalculatedWidth { get { return precalculatedWidth; } set { precalculatedWidth = value; } }
		public virtual bool ShowCaptionButton { get { return PageGroup.ShowCaptionButton && !this.Minimized && Ribbon.GetRibbonStyle() != RibbonControlStyle.TabletOffice; } }
		protected internal void Clear() {
			this.contentButtonViewInfo = null;
		}
		internal static bool HasLargeItemsCore(IRibbonGroupInfo groupInfo) {
			foreach(RibbonItemViewInfo vi in groupInfo.Items) {
				if(vi.Item.IsLargeButton || vi is InRibbonGalleryRibbonItemViewInfo) return true;
			}
			return false;
		}
		protected internal virtual int Reduce(ReduceOperation op) {
			RibbonPageGroupLayoutCalculator calc = ViewInfo.CreateGroupLayoutCalculator(this);
			int width = PrecalculatedWidth;
			int deltaWidth = 0;
			int reduceValue;
			int noReduceCount = 0;
			for(; ; ) {
				if(op != null) {
					deltaWidth = calc.Reduce(op);
					break;
				} else
					reduceValue = calc.Reduce();
				if(reduceValue <= 0) noReduceCount++;
				else noReduceCount = 0;
				deltaWidth += reduceValue;
				if(deltaWidth >= 0 || noReduceCount > 20) break;
			}
			if(!Minimized) calc.UpdateGroupLayout(new Rectangle(ContentBounds.Location, new Size(PrecalculatedWidth, ViewInfo.GroupContentHeight)));
			return deltaWidth;
		}
		protected internal virtual SkinElementInfo GetCaptionSeparatorInfo() {
			return new SkinElementInfo(CommonSkins.GetSkin(ViewInfo.Provider)[CommonSkins.SkinLabelLine], CaptionSeparatorBounds);
		}
		#region IButtonsPanelOwner Members
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return true; }
		}
		XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return PageGroup.Enabled; }
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			return new GroupBoxButtonsPanelSkinPainter(ViewInfo.Ribbon.GetController().LookAndFeel);
		}
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		void IButtonsPanelOwner.Invalidate() {
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		#endregion
		#region IGroupBoxButtonsPanelOwner Members
		BaseButtonCollection collection = new BaseButtonCollection(null);
		BaseButtonCollection IGroupBoxButtonsPanelOwner.CustomHeaderButtons {
			get { return collection; }
		}
		bool IGroupBoxButtonsPanelOwner.IsRightToLeft {
			get { return ViewInfo.IsRightToLeft; }
		}
		void IGroupBoxButtonsPanelOwner.LayoutChanged() {
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonChecked(Docking2010.BaseButtonEventArgs ea) {
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonClick(Docking2010.BaseButtonEventArgs ea) {
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonUnchecked(Docking2010.BaseButtonEventArgs ea) {
		}
		#endregion
	}
	public class RibbonApplicationButtonInfo : ObjectInfoArgs {
		RibbonViewInfo viewInfo;
		Rectangle textBounds;
		public RibbonApplicationButtonInfo(RibbonViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; } }
		public virtual SkinElementInfo GetInfo() {
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[GetSkinName()], Bounds);
			ObjectState state = ViewInfo.Ribbon.ApplicationButtonContentControl.ContentVisible ? ObjectState.Pressed : State;
			if(ViewInfo.IsColored && ViewInfo.GetRibbonStyle() != RibbonControlStyle.Office2007) {
				res.ImageIndex = RibbonPainter.ImageIndexByColorScheme(ViewInfo.Ribbon.ColorScheme, state, 3);
			}
			else {
				res.ImageIndex = -1;
				res.State = state;
			}
			if(!ViewInfo.Ribbon.Enabled)
				res.Attributes = PaintHelper.RibbonDisabledAttributes;
			return res;
		}
		protected virtual string GetSkinName() {
			switch(ViewInfo.GetRibbonStyle()) {
				case RibbonControlStyle.Office2007: return RibbonSkins.SkinFormApplicationButton;
				case RibbonControlStyle.OfficeUniversal: return RibbonSkins.SkinTabHeaderPage;
				default: return RibbonSkins.SkinApplicationButton2010;
			}
		}
	}
	public class RibbonApplicationButtonTextPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			if(e.Bounds.IsEmpty) return;
			RibbonApplicationButtonInfo ee = (RibbonApplicationButtonInfo)e;
			if(!string.IsNullOrEmpty(ee.ViewInfo.GetApplicationButtonText())) {
				using(AppearanceObject obj = (AppearanceObject)ee.ViewInfo.PaintAppearance.PageHeader.Clone()) {
					SkinElement elem = ee.GetInfo().Element;
					if(e.State == ObjectState.Normal || elem.GetForeColor(e.State).IsEmpty)
						obj.ForeColor = elem.Color.GetForeColor();
					else
						obj.ForeColor = elem.GetForeColor(obj, e.State);	
					if(ee.ViewInfo.Form != null && ee.ViewInfo.Form.IsGlassForm && ee.ViewInfo.Ribbon.AllowGlassTabHeader)
						ObjectPainter.DrawTextOnGlass(e.Graphics, obj, ee.ViewInfo.GetApplicationButtonText(), ee.TextBounds, obj.GetStringFormat());
					else
						e.Paint.DrawString(e.Cache, ee.ViewInfo.GetApplicationButtonText(), obj.Font, obj.GetForeBrush(e.Cache), ee.TextBounds, obj.GetStringFormat());
				}
			}
		}
	}
	public class RibbonApplicationButtonPainter : ObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			RibbonApplicationButtonInfo ee = (RibbonApplicationButtonInfo)e;
			if(!ee.ViewInfo.IsAllowApplicationButton) return Rectangle.Empty;
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, ee.GetInfo());
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			RibbonApplicationButtonInfo ee = (RibbonApplicationButtonInfo)e;
			return ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, ee.GetInfo());
		}
		public override void DrawObject(ObjectInfoArgs e) {
			if(e.Bounds.IsEmpty) return;
			RibbonApplicationButtonInfo ee = (RibbonApplicationButtonInfo)e;
			SkinElementInfo info = ee.GetInfo();
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			Bitmap icon = ee.ViewInfo.GetApplicationIcon();
			if(icon == null || icon.Size.IsEmpty) return;
			Rectangle client = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
			if(icon.Size.Width <= client.Width && icon.Size.Height <= client.Height)
				client = RectangleHelper.GetCenterBounds(client, icon.Size);
			if(!string.IsNullOrEmpty(ee.ViewInfo.GetApplicationButtonText())){
				new RibbonApplicationButtonTextPainter().DrawObject(e);
			}
			else {
				e.Paint.DrawImage(e.Graphics, icon, client, new Rectangle(Point.Empty, icon.Size), true);
			}
		}
	}
	public class RibbonFullScreenBarPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			RibbonViewInfo viewInfo = ((RibbonDrawInfo)e).ViewInfo as RibbonViewInfo;
			if(viewInfo == null)
				return;
			DrawBackground(e.Cache, viewInfo);
			DrawGlyph(e.Cache, viewInfo);
		}
		protected virtual void DrawBackground(GraphicsCache cache, RibbonViewInfo viewInfo) {
			SkinElementInfo elementInfo = GetFullScreenBarElementInfo(viewInfo);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
		public virtual SkinElementInfo GetFullScreenBarElementInfo(RibbonViewInfo viewInfo) {
			SkinElement element = RibbonSkins.GetSkin(viewInfo.Provider)[RibbonSkins.SkinPopupGalleryFilterPanel];
			SkinElementInfo res = new SkinElementInfo(element, viewInfo.Bounds);
			res.ImageIndex = -1;
			res.State = ShouldDrawNormalBackgound(viewInfo) ? ObjectState.Normal : ObjectState.Hot;
			return res;
		}
		protected bool ShouldDrawNormalBackgound(RibbonViewInfo viewInfo) {
			return viewInfo.HotObject.HitTest != RibbonHitTest.FullScreenModeBar;
		}
		protected virtual void DrawGlyph(GraphicsCache cache, RibbonViewInfo viewInfo) {
			Rectangle bounds = viewInfo.FullScreenBarGlyphBounds;
			cache.Graphics.DrawImage(viewInfo.FullScreenGlyphImage, bounds);
		}
	}
	public enum RibbonHitTest {
		None, HeaderPanel, PageHeader, PageHeaderCategory, Panel, PageGroup, PageGroupCaption,
		PageGroupCaptionButton, ItemSeparator, Item, ItemDrop, Toolbar, ApplicationButton,
		Gallery, GalleryItem, GalleryItemGroup, GalleryUpButton, GalleryDownButton, GalleryLeftButton, GalleryRightButton,
		GalleryDropDownButton, GallerySizingPanel, GallerySizeGrip,
		GalleryFilter, GalleryImage, StatusBar, PageHeaderLeftScroll, PageHeaderRightScroll, PanelLeftScroll, PanelRightScroll,
		FormCaption, FormCloseButton, FormMinimizeButton, FormFullScreenButton, FormMaximizeButton, FormHelpButton, FullScreenModeBar
	} 
	public class RibbonHitInfo {
		Point hitPoint = Point.Empty;
		RibbonHitTest hitTest = RibbonHitTest.None;
		RibbonPageGroup pageGroup = null;
		RibbonPageGroupViewInfo pageGroupInfo;
		RibbonPage page = null;
		RibbonItemViewInfo itemInfo = null;
		RibbonQuickAccessToolbarViewInfo toolbar = null;
		RibbonPageCategory category = null;
		RibbonPageCategoryViewInfo categoryInfo;
		BaseGalleryViewInfo galleryInfo;
		GalleryItemViewInfo galleryItemInfo;
		GalleryItemGroupViewInfo galleryItemGroupInfo;
		RibbonStatusBar statusBar = null;
		IRibbonItem item = null;
		public RibbonHitInfo() { }
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public RibbonHitTest HitTest { get { return hitTest; } }
		public RibbonPageGroup PageGroup { get { return pageGroup; } set { pageGroup = value; } }
		public RibbonPage Page { 
			get { return page; } 
			set { 
				page = value;
				PageCategory = page.Category;
			} 
		}
		public RibbonStatusBar StatusBar { get { return statusBar; } set { statusBar = value; } }
		public RibbonQuickAccessToolbarViewInfo Toolbar { get { return toolbar; } set { toolbar = value; } }
		public BarItemLink Item { get { return item as BarItemLink; } }
		public DevExpress.XtraBars.Ribbon.Gallery.BaseGallery Gallery { get { return GalleryInfo != null ? GalleryInfo.Gallery : null; } }
		protected internal GalleryItemGroupViewInfo GalleryItemGroupInfo { get { return galleryItemGroupInfo; } set { galleryItemGroupInfo = value; } }
		public GalleryItemGroup GalleryItemGroup { get { return GalleryItemGroupInfo == null? null: GalleryItemGroupInfo.Group; } }
		public GalleryItem GalleryItem { get { return GalleryItemInfo == null ? null : GalleryItemInfo.Item; } }
		public RibbonPageCategoryViewInfo PageCategoryInfo { get { return categoryInfo; } set { categoryInfo = value; } }
		public RibbonPageCategory PageCategory { get { return category; } set { category = value; } }
		protected internal RibbonPageGroupViewInfo PageGroupInfo {
			get { return pageGroupInfo; }
			set {
				pageGroupInfo = value;
				if(value != null) pageGroup = value.PageGroup;
			}
		}
		protected internal BaseGalleryViewInfo GalleryInfo { get { return galleryInfo; } set { galleryInfo = value; } }
		protected internal GalleryItemViewInfo GalleryItemInfo { get { return galleryItemInfo; } set { galleryItemInfo = value; } }
		protected internal InRibbonGalleryRibbonItemViewInfo GalleryBarItemInfo { get { return ItemInfo as InRibbonGalleryRibbonItemViewInfo; } }
		protected internal RibbonItemViewInfo ItemInfo { get { return itemInfo; } }
		public bool InPanel { get { return InPageGroup; } }
		public bool IsEmpty { get { return HitTest == RibbonHitTest.None; } }
		public bool InToolbar { get { return Toolbar != null; } }
		public bool InPageCategory { get { return PageCategory != null; } }
		public bool InGallerySizeGrip { get { return HitTest == RibbonHitTest.GallerySizeGrip; } }
		public bool InGallerySizingPanel {
			get { return HitTest == RibbonHitTest.GallerySizingPanel || HitTest == RibbonHitTest.GallerySizeGrip; }
		}
		public bool InGalleryScrollBar {
			get {
				StandaloneGalleryViewInfo vi = GalleryInfo as StandaloneGalleryViewInfo;
				return HitTest == RibbonHitTest.Gallery && vi != null && vi.ControlBounds.Contains(HitPoint); 
			}
		} 
		public bool InGalleryFilter {
			get { return HitTest == RibbonHitTest.GalleryFilter; }
		}
		public bool InGalleryGroup {
			get {
				if(HitTest != RibbonHitTest.GalleryItemGroup && HitTest != RibbonHitTest.GalleryItem && HitTest != RibbonHitTest.GalleryImage) return false;
				return GalleryItemGroupInfo != null;
			}
		}
		public bool InGallery {
			get {
				return GalleryInfo != null &&
					(HitTest == RibbonHitTest.Gallery || HitTest == RibbonHitTest.GalleryItem || HitTest == RibbonHitTest.GalleryImage ||
					HitTest == RibbonHitTest.GalleryUpButton || HitTest == RibbonHitTest.GalleryDownButton || HitTest == RibbonHitTest.GalleryLeftButton || HitTest == RibbonHitTest.GalleryRightButton ||
					HitTest == RibbonHitTest.GalleryDropDownButton || HitTest == RibbonHitTest.GalleryItemGroup);
			}
		}
		public bool InGalleryItem {
			get {
				return InGallery && GalleryItemInfo != null && (HitTest == RibbonHitTest.GalleryItem || HitTest == RibbonHitTest.GalleryImage);
			}
		}
		public bool InPage {
			get { return HitTest == RibbonHitTest.PageHeader && Page != null; }
		}
		public bool InPageGroup {
			get {
				return
					PageGroup != null &&
					(HitTest == RibbonHitTest.PageGroup || HitTest == RibbonHitTest.PageGroupCaption ||
					 HitTest == RibbonHitTest.PageGroupCaptionButton || InGallery ||
					 InPageItem || HitTest == RibbonHitTest.ItemSeparator);
			}
		}
		public bool InFormButton { 
			get {
				return HitTest == RibbonHitTest.FormCloseButton ||
						HitTest == RibbonHitTest.FormMinimizeButton ||
						HitTest == RibbonHitTest.FormMaximizeButton ||
						HitTest == RibbonHitTest.FormHelpButton ||
						HitTest == RibbonHitTest.FormFullScreenButton;
			} 
		}
		public bool InPageItem { get { return PageGroup != null && InItem; } }
		public bool InPageHeaderScrollButtons { get { return HitTest == RibbonHitTest.PageHeaderLeftScroll || HitTest == RibbonHitTest.PageHeaderRightScroll; } }
		public bool InPanelScrollButtons { get { return HitTest == RibbonHitTest.PanelLeftScroll || HitTest == RibbonHitTest.PanelRightScroll; } }
		public bool InItem { get { return Item != null && (HitTest == RibbonHitTest.Item || HitTest == RibbonHitTest.ItemDrop); } }
		protected virtual bool IsLinksEquals(BarItemLink link1, BarItemLink link2) {
			return link1 == link2 || link1.ClonedFromLink == link2 || link2.ClonedFromLink == link1 || link1.ClonedFromLink == link2.ClonedFromLink;
		}
		public virtual bool IsEqualItems(RibbonHitInfo hitInfo) {
			if(hitInfo.ItemInfo == null || ItemInfo == null || !hitInfo.InItem || !InItem) return false;
			if(hitInfo.ItemInfo == ItemInfo) return true;
			if(ItemInfo != null && ItemInfo.ViewInfo.Manager.SelectionInfo.IsButtonGroupLinksEquals(hitInfo.ItemInfo.Item as BarItemLink, ItemInfo.Item as BarItemLink)) return true;
			return false;
		}
		public override bool Equals(object obj) {
			RibbonHitInfo hi = obj as RibbonHitInfo;
			if(hi == null) return false;
			if(hi.HitTest != HitTest) return false;
			if(hi.Page == Page && hi.PageGroup == PageGroup && hi.ItemInfo == ItemInfo && hi.GalleryInfo == GalleryInfo && hi.GalleryItemInfo == GalleryItemInfo) return true;
			if(IsEqualItems(hi)) return true;
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal void SetItem(RibbonItemViewInfo info) { SetItem(info, RibbonHitTest.Item); }
		protected internal void SetItem(RibbonItemViewInfo info, RibbonHitTest hitTest) {
			this.itemInfo = info;
			this.item = info == null? null: info.Item;
			this.hitTest = hitTest;
		}
		internal bool ContainsSet(Rectangle bounds, RibbonHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				this.hitTest = hitTest;
				return true;
			}
			return false;
		}
		internal void SetHitTest(RibbonHitTest hitTest) {
			this.hitTest = hitTest;
		}
	}
	public static class MergeOrderArrayBuilder {
		public static ArrayList GetArrangedItems(CollectionBase items, CollectionBase mergedItems, bool isMerged) {
			ArrayList res = new ArrayList();
			int index = 0;
			ISupportMergeOrder item = null;
			if(isMerged) {
				while((item = GetItemByOrderNearIndex(ref index, items, mergedItems, res)) != null) {
					res.Add(item);
				}
			}
			FillArrayByUnorderedItems(items, res, isMerged);
			FillArrayByUnorderedItems(mergedItems, res, isMerged);
			return res;
		}
		static void FillArrayByUnorderedItems(CollectionBase items, ArrayList res, bool isMerged) {
			foreach(ISupportMergeOrder item in items){
				if(!isMerged || item.MergeOrder == -1)
					res.Add(item);
			}
		}
		static ISupportMergeOrder GetItemByOrderNearIndex(ref int index, CollectionBase items, CollectionBase mergedItems, ArrayList res) {
			int resIndex = int.MaxValue;
			ISupportMergeOrder resItem = null;
			foreach(ISupportMergeOrder item in items) {
				if(item.MergeOrder >= index && item.MergeOrder < resIndex && !res.Contains(item)) {
					resIndex = item.MergeOrder;
					resItem = item;
				}
			}
			foreach(ISupportMergeOrder item in mergedItems) {
				if(item.MergeOrder >= index && item.MergeOrder < resIndex && !res.Contains(item)) {
					resIndex = item.MergeOrder;
					resItem = item;
				}
			}
			index = resIndex;
			return resItem;
		}
	}
	internal static class RibbonItemInToolbarStyleCalculator {
		internal static RibbonItemStyles GetItemStyle(BarItemLink link, bool inPageHeader) {
			bool showItemsCaption = inPageHeader ? link.Ribbon.ShowItemCaptionsInPageHeader : link.Ribbon.ShowItemCaptionsInQAT;
			if(!showItemsCaption && !(link is BarEditItemLink || link is BarStaticItemLink || (link is BarCheckItemLink && ((BarCheckItem)link.Item).CheckBoxVisibility != CheckBoxVisibility.None)))
				return RibbonItemStyles.SmallWithoutText;
			RibbonItemStyles res = RibbonItemStyles.SmallWithText;
			if(link.UserRibbonStyle != RibbonItemStyles.Default)
				res = ((IRibbonItem)link).AllowedStyles;
			else
				res = link.Item.RibbonStyle == RibbonItemStyles.Default ? RibbonItemStyles.SmallWithText : link.Item.RibbonStyle;
			return res & (~RibbonItemStyles.Large);
		}
	}
	public class RibbonSkinProvider : ISkinProviderEx {
		public RibbonSkinProvider(RibbonViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		RibbonViewInfo ViewInfo { get; set; }
		public Color GetMaskColor() {
			return ((ISkinProviderEx)ViewInfo.Ribbon.GetController().LookAndFeel).GetMaskColor();
		}
		public Color GetMaskColor2() {
			return ((ISkinProviderEx)ViewInfo.Ribbon.GetController().LookAndFeel).GetMaskColor2();
		}
		public float GetTouchScaleFactor() {
			return ((ISkinProviderEx)ViewInfo.Ribbon.GetController().LookAndFeel).GetTouchScaleFactor();
		}
		public bool GetTouchUI() {
			return ViewInfo.Ribbon.OptionsTouch.TouchUI == DefaultBoolean.True;
		}
		public string SkinName {
			get { return ((ISkinProviderEx)ViewInfo.Ribbon.GetController().LookAndFeel).SkinName; }
		}
	}
}
