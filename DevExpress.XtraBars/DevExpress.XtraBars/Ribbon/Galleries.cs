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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraBars.Ribbon.Gallery {
	public enum CheckDrawMode { Default, OnlyImage, ImageAndText }
	public enum GalleryScrollMode { Standard, Smooth }
	public enum ItemCheckMode { None, SingleCheck, SingleRadio, Multiple, SingleCheckInGroup, SingleRadioInGroup, MultipleInGroup }
	public enum GalleryItemAlignment { Near, Center, Far, Custom }
	public enum GalleryItemAutoSizeMode { Default, Vertical, None }
	[ToolboxItem(false)]
	public abstract class BaseGallery : Component, IAppearanceOwner, IImageCollectionHelper, ISupportXtraAnimation, IAsyncImageLoaderClient, ISearchControlClient {
		internal static readonly object itemClickEvent = new object();
		internal static readonly object itemDoubleClickEvent = new object();
		internal static readonly object itemCheckedChanged = new object();
		internal static readonly object endScroll = new object();
		internal static readonly object customDrawItemImage = new object();
		internal static readonly object customDrawItemText = new object();
		internal static readonly object filterMenuPopupEvent = new object();
		internal static readonly object filterMenuItemClickEvent = new object();
		internal static readonly object galleryItemHover = new object();
		internal static readonly object galleryItemLeave = new object();
		internal static readonly object marqueeSelectionCompleted = new object();
		[ThreadStatic]
		static GalleryItemImagePopupForm[] hoverImageForms;
		static GalleryItemImagePopupForm[] HoverImageForms {
			get {
				if(hoverImageForms == null) {
					hoverImageForms = new GalleryItemImagePopupForm[formsCount];
					for(int i = 0; i < formsCount; i++) hoverImageForms[i] = new GalleryItemImagePopupForm();
					nextForm = 0;		
				}
				return hoverImageForms;
			} 
		}
		static int formsCount = 7;
		static int nextForm;
		static BaseGallery() { }
		internal static void HideHoverForms() {
			for(int n = 0; n < formsCount; n++)
				HoverImageForms[n].Hide();
		}
		GalleryControlPainter painter;
		Size imageSize;
		Size hoverImageSize;
		bool showGroupCaption;
		bool showItemText;
		bool drawImageBackground = true;
		bool fixedImageSize;
		bool fixedHoverImageSize;
		bool allowHoverImages;
		CheckDrawMode checkDrawMode = CheckDrawMode.Default;
		int rowCount, columnCount;
		int distanceBetweenItems;
		int distanceItemImageToText;
		int distanceItemCaptionToDescription;
		int updateCount;
		object tag;
		Locations itemImageLocation;
		SkinPaddingEdges itemImagePadding;
		SkinPaddingEdges itemTextPadding;
		RibbonGalleryAppearances appearance;
		GalleryItemGroupCollection groups;
		GalleryItem downItem;
		object images;
		object hoverImages;
		GalleryItemAlignment firstItemVertAlignment;
		GalleryItemAlignment lastItemVertAlignment;
		int firstItemVertIndent;
		int lastItemVertIndent;
		bool useMaxImageSize;
		Timer hoverTimer;
		RibbonHitInfo hoverHitInfo = null;
		ImageLayoutMode itemImageLayout;
		GalleryScrollMode scrollMode;
		float scrollSpeed;
		ItemCheckMode itemCheckMode;
		bool clearSelectionOnClickEmptySpace = false;
		bool allowMarqueeSelection = false;
		int maxItemWidth;
		GalleryItemAutoSizeMode itemAutoSizeMode;
		Size itemSize;
		Padding groupContentMargin;
		bool allowHtmlText;
		bool showImages = true;
		public BaseGallery() {
			this.appearance = new RibbonGalleryAppearances(this);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.groups = new GalleryItemGroupCollection(this);
			this.imageSize = Size.Empty;
			this.hoverImageSize = Size.Empty;
			this.itemImageLocation = Locations.Default;
			this.showGroupCaption = DefaultShowGroupCaption;
			this.showItemText = false;
			this.allowHoverImages = false;
			this.rowCount = 3;
			this.columnCount = 5;
			this.distanceBetweenItems = 0;
			this.distanceItemImageToText = 0;
			this.updateCount = 0;
			this.distanceItemCaptionToDescription = 0;
			this.itemImagePadding = new SkinPaddingEdges();
			this.itemTextPadding = new SkinPaddingEdges();
			this.fixedImageSize = true;
			this.fixedHoverImageSize = true;
			this.downItem = null;
			this.itemImageLayout = ImageLayoutMode.MiddleCenter;
			this.scrollMode = GalleryScrollMode.Standard;
			this.scrollSpeed = 1.0f;
			this.itemCheckMode = DefaultItemCheckMode;
			this.firstItemVertAlignment = GalleryItemAlignment.Near;
			this.lastItemVertAlignment = GalleryItemAlignment.Far;
			this.firstItemVertIndent = 0;
			this.lastItemVertIndent = 0;
			this.useMaxImageSize = false;
			this.allowMarqueeSelection = false;
			this.CheckSelectedItemViaKeyboard = DefaultCheckSelectedItemViaKeyboard;
			this.maxItemWidth = 0;
			this.itemSize = Size.Empty;
			this.itemAutoSizeMode = GalleryItemAutoSizeMode.Default;
			this.groupContentMargin = new Padding();
			this.allowHtmlText = false;
		}
		bool allowGlyphSkinning = false;
		[DefaultValue(false)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				LayoutChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowItemImage {
			get { return showImages; }
			set {
				if(ShowItemImage == value)
					return;
				showImages = value;
				LayoutChanged();
			}
		}
		protected virtual ItemCheckMode DefaultItemCheckMode {
			get { return ItemCheckMode.None; }
		}
		protected virtual bool DefaultCheckSelectedItemViaKeyboard { get { return false; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ClearAnimatedItems();
				Appearance.Dispose();
				Appearance.Changed -= new EventHandler(OnAppearanceChanged);
				searchControl = null;
			}
			base.Dispose(disposing);
		}
		BaseImageLoader imageLoader;
		protected internal BaseImageLoader ImageLoader {
			get {
				if(imageLoader == null)
					imageLoader = new GalleryAsyncImageLoader(this);
				return imageLoader;
			}
		}
		internal bool ForceCalcViewInfo { get; set; }
		protected internal virtual void ClearAnimatedItems() { }
		protected internal virtual Point ToolTipPointToScreen(Point pt) { return pt; }
		protected internal virtual bool DesignModeCore { 
			get {
				if(Ribbon == null) return false;
				return Ribbon.IsDesignMode;
			} 
		}
		public void Destroy() {
			DestroyItems();
			Groups.Clear();
		}
		public void DestroyItems() {
			List<GalleryItem> res = GetAllItems();
			foreach(GalleryItemGroup group in Groups) {
				group.Items.Clear();
			}
			foreach(GalleryItem item in res) {
				if(item.Image != null) {
					item.Image.Dispose();
				}
				if(item.HoverImage != null) {
					item.HoverImage.Dispose();
				}
			}
		}
		protected internal int GetMaxRowCount() {
			int res = 0;
			foreach(GalleryItemGroup group in Groups) {
				if(!group.Visible)
					continue;
				res++;
			}
			return Math.Max(1, res);
		}
		protected internal int GetMaxColumnCount() {
			int res = 0;
			foreach(GalleryItemGroup group in Groups) {
				int count = 0;
				if(!group.Visible)
					continue;
				foreach(GalleryItem item in group.Items) {
					if(item.Visible)
						count++;
				}
				res = Math.Max(res, count);
			}
			return Math.Max(1, res);
		}
		public List<GalleryItem> GetAllItems() {
			List<GalleryItem> res = new List<GalleryItem>();
			foreach(GalleryItemGroup group in Groups) {
				foreach(GalleryItem item in group.Items)
					res.Add(item);
			}
			return res;
		}
		public List<GalleryItem> GetCheckedItems() {
			List<GalleryItem> res = new List<GalleryItem>();
			foreach(GalleryItemGroup group in Groups) {
				foreach(GalleryItem item in group.Items) {
					if(item.Checked)
						res.Add(item);
				}
			}
			return res;
		}
		public GalleryItem GetCheckedItem() {
			List<GalleryItem> items = GetCheckedItems();
			return items.Count == 1 ? items[0] : null;
		}
		protected internal bool HasCheckedItems {
			get {
				for(int i = 0; i < Groups.Count; i++) {
					for(int j = 0; j < Groups[i].Items.Count; j++) {
						if(Groups[i].Items[j].Checked)
							return true;
					}
				}
				return false;
			}
		}
		public List<GalleryItem> GetVisibleItems() {
			List<GalleryItem> res = new List<GalleryItem>();
			foreach(GalleryItemGroup group in Groups) {
				if(group.Visible) {
					foreach(GalleryItem item in group.Items) {
						if(item.Visible)
							res.Add(item);
					}
				}
			}
			return res;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GalleryItem FocusedItem {
			get { return GetFocusedItem(); }
			set { SetItemFocused(value); }
		}
		protected virtual void SetItemFocused(GalleryItem value) {
		}
		protected virtual GalleryItem GetFocusedItem() {
			return null;
		}
		protected internal void InitHoverTimer(RibbonHitInfo hitInfo) {
			if(!hitInfo.InGalleryItem) {
				HoverHitInfo = null;
				return;
			}
			if(HoverHitInfo != null && HoverHitInfo.InGalleryItem && HoverHitInfo.GalleryItem == hitInfo.GalleryItem) return;
			DestroyHoverTimer();
			HoverHitInfo = hitInfo;
			hoverTimer = new Timer();
			hoverTimer.Interval = 250;
			hoverTimer.Tick += new EventHandler(OnHoverTimerTick);
			hoverTimer.Start();
		}
		protected void DestroyHoverTimer() {
			if(hoverTimer != null) hoverTimer.Dispose();
			hoverTimer = null;
		}
		protected internal virtual bool ImagePopupFormActAsPopup { get { return false; } }
		protected virtual void OnHoverTimerTick(object sender, EventArgs e) {
			if(ShouldRaiseItemHover)
				RaiseGalleryItemHover(HoverHitInfo.GalleryItem);
			DestroyHoverTimer();	
		}
		protected internal virtual bool IsRightToLeft { get { return false; } }
		protected RibbonHitInfo HoverHitInfo { get { return hoverHitInfo; } set { hoverHitInfo = value; } }
		protected virtual bool ShouldRaiseItemHover { get { return false; } }
		void OnAppearanceChanged(object sender, EventArgs e) {
			RefreshGallery();
		}
		protected internal virtual bool Enabled { get { return true; } }
		protected internal virtual ISkinProvider Provider {
			get { 
				if(Ribbon != null)
					return Ribbon.ViewInfo.Provider;
				if(GetController() == null) return null;
				return GetController().LookAndFeel;
			}
		}
		protected internal virtual UserLookAndFeel LookAndFeelCore {
			get { return Provider as UserLookAndFeel; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryClearSelectionOnClickEmptySpace"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(false)]
		public bool ClearSelectionOnClickEmptySpace {
			get { return clearSelectionOnClickEmptySpace; }
			set { clearSelectionOnClickEmptySpace = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemAutoSizeMode"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(GalleryItemAutoSizeMode.Default)]
		public GalleryItemAutoSizeMode ItemAutoSizeMode {
			get { return itemAutoSizeMode; }
			set {
				if(ItemAutoSizeMode == value)
					return;
				itemAutoSizeMode = value;
				LayoutChanged();
			}
		}
		bool ShouldSerializeItemSize() { return !ItemSize.IsEmpty; }
		void ResetItemSize() { ItemSize = Size.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemSize"),
#endif
 System.ComponentModel.Category("Behavior")]
		public Size ItemSize {
			get { return itemSize; }
			set {
				if(ItemSize == value)
					return;
				itemSize = value;
				LayoutChanged();
			}
		}
		[ System.ComponentModel.Category("Behavior"), DefaultValue(false)]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value)
					return;
				allowHtmlText = value;
				LayoutChanged();
			}
		}
		bool ShouldSerializeGroupContentMargin() { return GroupContentMargin != Padding.Empty; }
		void ResetGroupContentMargin() { GroupContentMargin = Padding.Empty; }
		[System.ComponentModel.Category("Behavior")]
		public Padding GroupContentMargin {
			get { return groupContentMargin; }
			set {
				if(GroupContentMargin == value)
					return;
				groupContentMargin = value;
				LayoutChanged();
			}
		}
		protected internal virtual Size DefaultItemSizeWithoutText { get { return new Size(22, 22); } }
		protected internal virtual Size DefaultItemSizeWithText { get { return new Size(96, 22); } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryMaxItemWidth"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(0)]
		public int MaxItemWidth {
			get { return maxItemWidth; }
			set {
				if(MaxItemWidth == value)
					return;
				maxItemWidth = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryAllowMarqueeSelection"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(false)]
		public bool AllowMarqueeSelection {
			get { return allowMarqueeSelection; }
			set { allowMarqueeSelection = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryUseMaxImageSize"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(false)]
		public bool UseMaxImageSize {
			get { return useMaxImageSize; }
			set {
				if(UseMaxImageSize == value)
					return;
				useMaxImageSize = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryFirstItemVertAlignment"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(GalleryItemAlignment.Near)]
		public GalleryItemAlignment FirstItemVertAlignment {
			get { return firstItemVertAlignment; }
			set {
				if(FirstItemVertAlignment == value) return;
				firstItemVertAlignment = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryLastItemVertAlignment"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(GalleryItemAlignment.Far)]
		public GalleryItemAlignment LastItemVertAlignment {
			get { return lastItemVertAlignment; }
			set {
				if(LastItemVertAlignment == value) return;
				lastItemVertAlignment = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryFirstItemVertIndent"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(0)]
		public int FirstItemVertIndent {
			get { return firstItemVertIndent; }
			set {
				if(FirstItemVertIndent == value) return;
				firstItemVertIndent = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryLastItemVertIndent"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(0)]
		public int LastItemVertIndent {
			get { return lastItemVertIndent; }
			set {
				if(LastItemVertIndent == value) return;
				lastItemVertIndent = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemCheckMode"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(ItemCheckMode.None)]
		public virtual ItemCheckMode ItemCheckMode {
			get { return itemCheckMode; }
			set {
				if(ItemCheckMode == value) return;
				itemCheckMode = value;
				OnItemCheckModeChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryCheckSelectedItemViaKeyboard"),
#endif
 System.ComponentModel.Category("Appearance")]
		public bool CheckSelectedItemViaKeyboard { get; set; }
		bool ShouldSerializeCheckSelectedItemViaKeyboard() {
			return CheckSelectedItemViaKeyboard != DefaultCheckSelectedItemViaKeyboard;
		}
		void ResetCheckSelectedItemViaKeyboard() {
			CheckSelectedItemViaKeyboard = DefaultCheckSelectedItemViaKeyboard;
		}
		GalleryOptionsImageLoad optionsImageLoad;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryOptionsImageLoad"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GalleryOptionsImageLoad OptionsImageLoad {
			get {
				if(optionsImageLoad == null)
					optionsImageLoad = new GalleryOptionsImageLoad();
				return optionsImageLoad;
			}
		}
		protected virtual void OnItemCheckModeChanged() {
			LayoutChanged();
		}
		bool inToggleItemCheck;
		protected bool InToggleItemCheck { get { return inToggleItemCheck; } private set { inToggleItemCheck = value; } }
		bool AddToSelectionModifierKeyPressed { get { return (Control.ModifierKeys & (Keys.Control | Keys.Shift)) != 0; } }
		public void SetItemCheck(GalleryItem item, bool newValue) {
			SetItemCheck(item, newValue, !AddToSelectionModifierKeyPressed);
		}
		protected virtual void SetItemCheck(GalleryItem item, bool newValue, bool unselectOtherItems, bool forceSetItemCheck) {
			if(ItemCheckMode == ItemCheckMode.None || InToggleItemCheck)
				return;
			InToggleItemCheck = true;
			bool isItemChecked = newValue;
			if(ItemCheckMode == ItemCheckMode.SingleCheckInGroup || ItemCheckMode == ItemCheckMode.SingleRadioInGroup) {
				SetItemsCheckState(item.GalleryGroup, false, item);
				item.Checked = ItemCheckMode == ItemCheckMode.SingleRadioInGroup && !forceSetItemCheck ? true : isItemChecked;
			}
			else if(ItemCheckMode == ItemCheckMode.MultipleInGroup) {
				if(unselectOtherItems)
					SetItemsCheckState(item.GalleryGroup, false, (GalleryItem)null);
				item.Checked = isItemChecked;
			}
			else if(ItemCheckMode == ItemCheckMode.SingleCheck || ItemCheckMode == ItemCheckMode.SingleRadio || unselectOtherItems) {
				SetItemsCheckState(false, item);
				item.Checked = ItemCheckMode == ItemCheckMode.SingleRadio && !forceSetItemCheck? true : isItemChecked;
			}
			else {
				item.Checked = isItemChecked;
			}
			InToggleItemCheck = false;
		}
		public virtual void SetItemCheck(GalleryItem item, bool newValue, bool unselectOtherItems) {
			SetItemCheck(item, newValue, unselectOtherItems, false);
		}
		protected internal void SetItemsCheckState(bool isChecked) {
			BeginUpdate();
			try {
				foreach(GalleryItemGroup group in Groups) {
					SetItemsCheckState(group, isChecked, (GalleryItem)null);
				}
			}
			finally { EndUpdate(); }
		}
		protected void SetItemsCheckState(bool isChecked, GalleryItemGroup exceptGroup) {
			BeginUpdate();
			try {
				foreach(GalleryItemGroup group in Groups) {
					if(exceptGroup != group)
						SetItemsCheckState(group, isChecked, (GalleryItem)null);
				}
			}
			finally { EndUpdate(); }
		}
		protected void SetItemsCheckState(bool isChecked, List<GalleryItem> exceptItems) {
			BeginUpdate();
			try {
				foreach(GalleryItemGroup group in Groups) {
					SetItemsCheckState(group, isChecked, exceptItems);
				}
			}
			finally { EndUpdate(); }
		}
		protected void SetItemsCheckState(bool isChecked, GalleryItem exceptItem) {
			BeginUpdate();
			try {
				foreach(GalleryItemGroup group in Groups) {
					SetItemsCheckState(group, isChecked, exceptItem);
				}
			}
			finally { EndUpdate(); }
		}
		protected void SetItemsCheckState(GalleryItemGroup group, bool isChecked, GalleryItem exceptItem) {
			BeginUpdate();
			try {
				foreach(GalleryItem item in group.Items) {
					if(item == exceptItem)
						continue;
					item.Checked = isChecked;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected void SetItemsCheckState(GalleryItemGroup group, bool isChecked, List<GalleryItem> exceptItems) {
			BeginUpdate();
			try {
				foreach(GalleryItem item in group.Items) {
					if(exceptItems.Contains(item))
						continue;
					item.Checked = isChecked;
				}
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryScrollMode"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(GalleryScrollMode.Standard)]
		public GalleryScrollMode ScrollMode {
			get { return scrollMode; }
			set {
				if(ScrollMode == value) return;
				scrollMode = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryScrollSpeed"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(1.0f)]
		public float ScrollSpeed {
			get { return scrollSpeed; }
			set {
				if(ScrollSpeed == value) return;
				if(value <= 0.0f) value = 1.0f;
				scrollSpeed = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemImageLayout"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(ImageLayoutMode.MiddleCenter)]
		public ImageLayoutMode ItemImageLayout {
			get { return itemImageLayout; }
			set {
				if(ItemImageLayout == value) return;
				itemImageLayout = value;
				LayoutChanged();
			}
		}
		protected internal SkinElement GetSkin(string name) { return RibbonSkins.GetSkin(Provider)[name]; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryCheckDrawMode"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(CheckDrawMode.Default)]
		public CheckDrawMode CheckDrawMode {
			get { return checkDrawMode; }
			set {
				if(CheckDrawMode == value) return;
				checkDrawMode = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryImages"),
#endif
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)), 
		System.ComponentModel.Category("Appearance"), DefaultValue(null)]
		public object Images {
			get { return images; }
			set {
				if(images == value) return;
				images = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryHoverImages"),
#endif
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)), 
		System.ComponentModel.Category("Appearance"), DefaultValue(null)]
		public object HoverImages {
			get { return hoverImages; }
			set {
				if(hoverImages == value) return;
				hoverImages = value;
				RefreshGallery();
			}
		}
		public abstract void MakeVisible(GalleryItem item);
		protected internal abstract RibbonControl Ribbon { get; }
		protected internal GalleryItem DownItem { 
			get { return downItem; } 
			set {
				if(DownItem == value) return;
				GalleryItem prev = DownItem;
				downItem = value;
				if(prev != null) Invalidate(prev);
				if(DownItem != null) Invalidate(DownItem);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryFixedHoverImageSize"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(true)]
		public bool FixedHoverImageSize {
			get { return fixedHoverImageSize; }
			set { fixedHoverImageSize = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryDrawImageBackground"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(true)]
		public bool DrawImageBackground {
			get { return drawImageBackground; }
			set {
				if(DrawImageBackground == value) return;
				drawImageBackground = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryFixedImageSize"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(true)]
		public virtual bool FixedImageSize {
			get { return fixedImageSize; }
			set {
				if(fixedImageSize == value) return;
				fixedImageSize = value;
				LayoutChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void LayoutChanged() {
			RefreshGallery();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryAllowHoverImages"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(false)]
		public bool AllowHoverImages {
			get { return allowHoverImages; }
			set { allowHoverImages = value; }
		}
		bool ShouldSerializeHoverImageSize() { return HoverImageSize != Size.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryHoverImageSize"),
#endif
 System.ComponentModel.Category("Behavior")]
		public Size HoverImageSize {
			get { return hoverImageSize; }
			set { hoverImageSize = value; }
		}
		bool ShouldSerializeItemImagePadding() { return !ItemImagePadding.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemImagePadding"),
#endif
 System.ComponentModel.Category("Layout")]
		public SkinPaddingEdges ItemImagePadding {
			get { return itemImagePadding; }
			set {
				if(itemImagePadding == value) return;
				itemImagePadding = value;
				LayoutChanged();
			}
		}
		bool ShouldSerializeItemTextPadding() { return !ItemTextPadding.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemTextPadding"),
#endif
 System.ComponentModel.Category("Layout")]
		public SkinPaddingEdges ItemTextPadding {
			get { return itemTextPadding; }
			set {
				if(itemTextPadding == value) return;
				itemTextPadding = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryShowItemText"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(false)]
		public bool ShowItemText {
			get { return showItemText; }
			set {
				if(showItemText == value) return;
				showItemText = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		bool ShouldSerializeImageSize() { return ImageSize != Size.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryImageSize"),
#endif
 System.ComponentModel.Category("Behavior")]
		public Size ImageSize {
			get { return imageSize; }
			set {
				if(imageSize == value) return;
				imageSize = value;
				RemoveImageShowingAnimations(false);
				LayoutChanged();
			}
		}
		protected virtual bool DefaultShowGroupCaption { get { return false; } }
		bool ShouldSerializeShowGroupCaption() { return ShowGroupCaption != DefaultShowGroupCaption; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryShowGroupCaption"),
#endif
 System.ComponentModel.Category("Behavior")]
		public bool ShowGroupCaption {
			get { return showGroupCaption; }
			set {
				if(showGroupCaption == value) return;
				showGroupCaption = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryRowCount"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(3)]
		public int RowCount {
			get { return rowCount; }
			set {
				value = Math.Max(value, 1);
				if(rowCount == value) return;
				rowCount = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryColumnCount"),
#endif
 System.ComponentModel.Category("Behavior"), DefaultValue(5)]
		public int ColumnCount {
			get { return columnCount; }
			set {
				value = Math.Max(value, 1);
				if(columnCount == value) return;
				columnCount = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryDistanceBetweenItems"),
#endif
 System.ComponentModel.Category("Layout"), DefaultValue(0)]
		public int DistanceBetweenItems {
			get { return distanceBetweenItems; }
			set {
				value = Math.Max(0, value);
				if(distanceBetweenItems == value) return;
				distanceBetweenItems = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryDistanceItemImageToText"),
#endif
System.ComponentModel.Category("Layout"), DefaultValue(0)]
		public int DistanceItemImageToText {
			get { return distanceItemImageToText; }
			set {
				value = Math.Max(0, value);
				if(distanceItemImageToText == value) return;
				distanceItemImageToText = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryDistanceItemCaptionToDescription"),
#endif
System.ComponentModel.Category("Layout"), DefaultValue(0)]
		public int DistanceItemCaptionToDescription {
			get { return distanceItemCaptionToDescription; }
			set {
				value = Math.Max(0, value);
				if(distanceItemCaptionToDescription == value) return;
				distanceItemCaptionToDescription = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemImageLocation"),
#endif
System.ComponentModel.Category("Appearance"), DefaultValue(Locations.Default)]
		public Locations ItemImageLocation {
			get { return itemImageLocation; }
			set {
				if(itemImageLocation == value) return;
				itemImageLocation = value;
				LayoutChanged();
			}
		}
		Control IImageCollectionHelper.OwnerControl {
			get { return null; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public GalleryItemGroupCollection Groups { get { return groups; } }
		protected internal abstract BaseGalleryViewInfo CreateViewInfo();
		protected abstract BaseDesignTimeManager DesignTimeManager { get; } 
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryAppearance"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RibbonGalleryAppearances Appearance { get { return appearance; } }
		protected internal virtual GalleryControlPainter Painter { 
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			} 
		}
		protected virtual void AssignParams(BaseGallery gallery) {
			Appearance.Assign(gallery.Appearance);
			this.ShowItemImage = gallery.ShowItemImage;
			this.DrawImageBackground = gallery.DrawImageBackground;
			this.ImageSize = gallery.ImageSize;
			this.HoverImageSize = gallery.HoverImageSize;
			this.AllowHoverImages = gallery.AllowHoverImages;
			this.ShowGroupCaption = gallery.ShowGroupCaption;
			this.ShowItemText = gallery.ShowItemText;
			this.FixedImageSize = gallery.FixedImageSize;
			this.FixedHoverImageSize = gallery.FixedHoverImageSize;
			this.RowCount = gallery.RowCount;
			this.ColumnCount = gallery.ColumnCount;
			this.DistanceBetweenItems = gallery.DistanceBetweenItems;
			this.DistanceItemImageToText = gallery.DistanceItemImageToText;
			this.DistanceItemCaptionToDescription = gallery.DistanceItemCaptionToDescription;
			this.ItemImageLocation = gallery.ItemImageLocation;
			this.ItemImagePadding = gallery.ItemImagePadding;
			this.ItemTextPadding = gallery.ItemTextPadding;
			this.Images = gallery.Images;
			this.HoverImages = gallery.HoverImages;
			this.UseMaxImageSize = gallery.UseMaxImageSize;
			this.ItemCheckMode = gallery.ItemCheckMode;
			this.AllowMarqueeSelection = gallery.AllowMarqueeSelection;
			this.ItemSize = gallery.ItemSize;
			this.ItemAutoSizeMode = gallery.ItemAutoSizeMode;
		}
		protected virtual void CopyGroups(BaseGallery gallery) {
			Groups.Clear();
			for(int contIndex = 0; contIndex < gallery.Groups.Count; contIndex++) {
				Groups.Add((GalleryItemGroup)gallery.Groups[contIndex].Clone());
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void CreateFrom(BaseGallery gallery) {
			Assign(gallery);
		}
		public void Assign(BaseGallery gallery) {
			BeginUpdate();
			try {
				AssignParams(gallery);
				AssignEventHandlers(gallery);
				CopyGroups(gallery);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void AssignEventHandlers(BaseGallery gallery) { }
		protected internal virtual void ReleaseEventHandlers() { }
		public GalleryItem GetItemByValue(object value) {
			foreach(GalleryItemGroup group in Groups) {
				GalleryItem item = group.GetItemByValue(value);
				if(item != null)
					return item;
			}
			return null;
		}
		protected internal virtual GalleryItemGroupViewInfo GetGroupInfo(GalleryItemGroup group) {
			return null;
		}
		protected internal virtual GalleryItemViewInfo GetItemInfo(GalleryItem item) {
			return null;
		}
		protected void ResetPainter() {
			this.painter = null;
		}
		protected abstract GalleryControlPainter CreatePainter();
		protected internal virtual GalleryItemPainter ItemPainter { get { return Painter.ItemPainter; } }
		public abstract void RefreshGallery();
		protected internal abstract void UpdateGallery();
		protected internal bool IsLockUpdate { get { return updateCount != 0; } }
		protected internal virtual bool IsLoading { get { return Ribbon == null || Ribbon.Manager.IsLoading; } }
		public void BeginUpdate() {
			this.updateCount++;
		}
		public void EndUpdate() {
			CancelUpdate();
			if(!IsLockUpdate) RefreshGallery();
		}
		protected internal virtual void OnCheckedChanged(GalleryItem item) {
			OnCheckedChanged(item, false);
		}
		protected internal virtual void OnCheckedChanged(GalleryItem item, bool forceSetItemCheck) {
			SetItemCheck(item, item.Checked, false, forceSetItemCheck);
			RaiseItemCheckedChangedEvent(item);
			Invalidate(item);
		}
		public abstract void Invalidate();
		public virtual void Invalidate(GalleryItem item) {
			Invalidate(); 
		}
		public virtual void Invalidate(Rectangle rect) { }
		public void CancelUpdate() {
			if(UpdateCount == 0) return;
			this.updateCount--;
		}
		protected int UpdateCount { get { return updateCount; } }
		public virtual void ScrollTo(GalleryItemGroup group, bool bAnimated) { ScrollTo(group, bAnimated, VertAlignment.Default); }
		public virtual void ScrollTo(GalleryItem item, bool bAnimated) { ScrollTo(item, bAnimated, VertAlignment.Default); }
		public virtual void ScrollTo(GalleryItemGroup group, bool bAnimated, VertAlignment groupAlignment) { }
		public virtual void ScrollTo(GalleryItem item, bool bAnimated, VertAlignment itemAlignment) { }
#if DEBUGTEST
		protected internal GalleryItemImagePopupForm GetForm(int index) { return HoverImageForms[Math.Min(Math.Max(0, index), formsCount - 1)]; }
		protected internal void SetNextFormIndex(int index) { nextForm = Math.Min(Math.Max(0, index), formsCount - 1); }
#endif
		public GalleryItem GetItemByCaption(string caption) {
			foreach(GalleryItemGroup group in Groups) {
				foreach(GalleryItem item in group.Items) {
					if(item.Caption == caption)
						return item;
				}
			}
			return null;
		}
		protected internal GalleryItemImagePopupForm FindForm(GalleryItemViewInfo activeItem) {
			if(HoverImageForms == null) return null;
			if(activeItem == null) return null;
			for(int i = 0; i < formsCount; i++) {
				if(HoverImageForms[i].ItemInfo == null) continue;
				if(HoverImageForms[i].ItemInfo.Item == activeItem.Item && HoverImageForms[i].Visible) return HoverImageForms[i];
			}
			return null;
		}
		protected internal int FindFreeFormIndex() {
			if(!HoverImageForms[nextForm].Visible) return nextForm;
			for(int i = nextForm + 1 >= formsCount ? 0 : nextForm + 1; i != nextForm; i++) {
				if(i == formsCount) i = 0;
				if(!HoverImageForms[i].Visible) return i;
			}
			return -1;
		}
		protected internal GalleryItemImagePopupForm GetNextForm(BaseGalleryViewInfo galleryInfo, GalleryItemViewInfo activeItem) {
			GalleryItemImagePopupForm form = FindForm(activeItem);
			if(form != null) return form;
			int freeIndex = FindFreeFormIndex();
			nextForm = freeIndex == -1 ? nextForm : freeIndex;
			form = HoverImageForms[nextForm];
			form.Hide();
			form.ViewInfo = galleryInfo;
			form.ItemInfo = activeItem;
			if(nextForm == formsCount - 1) nextForm = 0;
			else nextForm++;
			return form;
		}
		protected internal virtual void HideImageForms(GalleryItemViewInfo activeItem, bool hideAnimation) {
			GalleryItemImagePopupForm form = FindForm(activeItem);
			for(int i = 0; i < formsCount; i++) {
				if(HoverImageForms[i].Visible == false || form == HoverImageForms[i]) {
					continue;
				}
				if(HoverImageForms[i].Visible) {
					BoundsAnimationInfo info = XtraAnimator.Current.Get(HoverImageForms[i], HoverImageForms[i].ItemInfo) as BoundsAnimationInfo;
					if(info != null && !info.GrowUp) continue;
					if(hideAnimation) HoverImageForms[i].HidePopup();
					else HoverImageForms[i].Hide();
				}
			}
		}
		protected internal virtual void ShowImageForm(BaseGalleryViewInfo galleryInfo, GalleryItemViewInfo activeItem) {
			GalleryItemImagePopupForm form = GetNextForm(galleryInfo, activeItem);
			if(form.IsDisposed)
				return;
			BoundsAnimationInfo info = XtraAnimator.Current.Get(form, activeItem) as BoundsAnimationInfo;
			if(form.Visible && (info != null || form.Grew)) return;
			form.ShowPopup();
		}
		protected internal abstract BarAndDockingController GetController();
		[System.ComponentModel.Category("Action")]
		public event EventHandler EndScroll {
			add { this.Events.AddHandler(endScroll, value); }
			remove { this.Events.RemoveHandler(endScroll, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler ItemClick {
			add { this.Events.AddHandler(itemClickEvent, value); }
			remove { this.Events.RemoveHandler(itemClickEvent, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler ItemDoubleClick {
			add { this.Events.AddHandler(itemDoubleClickEvent, value); }
			remove { this.Events.RemoveHandler(itemDoubleClickEvent, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler ItemCheckedChanged {
			add { this.Events.AddHandler(itemCheckedChanged, value); }
			remove { this.Events.RemoveHandler(itemCheckedChanged, value); }
		}
		protected internal bool RaiseCustomDrawItemImage(GraphicsCache cache, GalleryItemViewInfo itemInfo, Rectangle bounds) {
			GalleryItemCustomDrawEventHandler handler = this.Events[customDrawItemImage] as GalleryItemCustomDrawEventHandler;
			GalleryItemCustomDrawEventArgs e = new GalleryItemCustomDrawEventArgs(cache, this, itemInfo, bounds);
			if(handler != null) handler(this, e);
			return e.Handled;
		}
		protected internal bool RaiseCustomDrawItemText(GraphicsCache cache, GalleryItemViewInfo itemInfo, Rectangle bounds) {
			GalleryItemCustomDrawEventHandler handler = this.Events[customDrawItemText] as GalleryItemCustomDrawEventHandler;
			GalleryItemCustomDrawEventArgs e = new GalleryItemCustomDrawEventArgs(cache, this, itemInfo, bounds);
			if(handler != null) handler(this, e);
			return e.Handled;
		}
		protected internal void RaiseItemClickEvent(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) {
			GalleryItemClickEventHandler handler = this.Events[itemClickEvent] as GalleryItemClickEventHandler;
			if(handler == null) return;
			handler(this, new GalleryItemClickEventArgs(inRibbonGalleryLink, gallery, item));
		}
		protected internal void RaiseItemDoubleClickEvent(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) {
			GalleryItemClickEventHandler handler = this.Events[itemDoubleClickEvent] as GalleryItemClickEventHandler;
			if(handler == null) return;
			handler(this, new GalleryItemClickEventArgs(inRibbonGalleryLink, gallery, item));
		}
		protected internal void RaiseEndScrollEvent() {
			EventHandler handler = this.Events[endScroll] as EventHandler;
			if(handler != null)
				handler(this, new EventArgs());
		}
		protected internal void RaiseItemCheckedChangedEvent(GalleryItem item) {
			GalleryItemEventHandler handler = this.Events[itemCheckedChanged] as GalleryItemEventHandler;
			if(handler == null) return;
				handler(this, new GalleryItemEventArgs(this, item));
		}
		GalleryItem lastCheckedItem = null;
		protected GalleryItem LastCheckedItem { get { return lastCheckedItem; } }
		protected internal virtual void OnItemClick(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) {
			if(!item.Enabled)
				return;
			if(item.Gallery == this) {
				bool itemCheck = !item.Checked;
				if((ItemCheckMode == ItemCheckMode.MultipleInGroup || ItemCheckMode == ItemCheckMode.Multiple) && item.Checked && !AddToSelectionModifierKeyPressed) { 
					List<GalleryItem> checkedItems = null;
					if(ItemCheckMode == ItemCheckMode.Multiple)
						checkedItems = GetCheckedItems();
					else if(ItemCheckMode == ItemCheckMode.MultipleInGroup)
						checkedItems = item.GalleryGroup.GetCheckedItems();
					if(checkedItems.Count > 1)
						itemCheck = true;
				}
				if(ItemCheckMode == ItemCheckMode.MultipleInGroup || ItemCheckMode == ItemCheckMode.Multiple) {
					if((Control.ModifierKeys & Keys.Shift) == 0 || LastCheckedItem == null || LastCheckedItem.GalleryGroup == null) {
						this.lastCheckedItem = item;
						SetItemCheck(item, itemCheck);
					}
					else {
						List<GalleryItem> checkedItems = GetItems(LastCheckedItem, item, ItemCheckMode == ItemCheckMode.MultipleInGroup);
						SetItemsCheckState(false, checkedItems);
						foreach(GalleryItem ci in checkedItems) {
							SetItemCheck(ci, true, false);
						}
					}
				}
				else {
					SetItemCheck(item, itemCheck);
				}
			}
			OnItemClickCore(inRibbonGalleryLink, gallery, item);
			item.RaiseItemClickEvent(inRibbonGalleryLink);
			RaiseItemClickEvent(inRibbonGalleryLink, gallery, item);
		}
		protected virtual void OnItemClickCore(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) {
		}
		protected internal virtual List<GalleryItem> GetItems(GalleryItem startItem, GalleryItem endItem, bool onlyInGroup) {
			List<GalleryItem> res = new List<GalleryItem>();
			if(onlyInGroup && startItem.GalleryGroup != endItem.GalleryGroup) {
				if(Groups.IndexOf(startItem.GalleryGroup) > Groups.IndexOf(endItem.GalleryGroup)) {
					endItem = startItem.GalleryGroup.Items[0];
				}
				else {
					endItem = startItem.GalleryGroup.Items[startItem.GalleryGroup.Items.Count - 1];
				}
			}
			if(Groups.IndexOf(startItem.GalleryGroup) > Groups.IndexOf(endItem.GalleryGroup) || startItem.GalleryGroup.Items.IndexOf(startItem) > endItem.GalleryGroup.Items.IndexOf(endItem)) {
				GalleryItem tmp = startItem;
				startItem = endItem;
				endItem = tmp;
			}
			int startGroupIndex = Groups.IndexOf(startItem.GalleryGroup);
			int endGroupIndex = onlyInGroup ? startGroupIndex : Groups.IndexOf(endItem.GalleryGroup);
			for(int i = startGroupIndex; i <= endGroupIndex; i++) {
				int startItemIndex = i == startGroupIndex ? Groups[i].Items.IndexOf(startItem) : 0;
				int endItemIndex = Groups[i] == endItem.GalleryGroup ? Groups[i].Items.IndexOf(endItem) : Groups[i].Items.Count - 1;
				for(int j = startItemIndex; j <= endItemIndex; j++) {
					res.Add(Groups[i].Items[j]);
				}
			}
			return res;
		}
		protected internal void OnItemDoubleClick(RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) {
			SetItemCheck(item, !item.Checked);
			RaiseItemDoubleClickEvent(inRibbonGalleryLink, gallery, item);
		}
		[System.ComponentModel.Category("Events")]
		public event GalleryItemCustomDrawEventHandler CustomDrawItemText {
			add { this.Events.AddHandler(customDrawItemText, value); }
			remove { this.Events.RemoveHandler(customDrawItemText, value); }
		}
		[System.ComponentModel.Category("Events")]
		public event GalleryItemCustomDrawEventHandler CustomDrawItemImage {
			add { this.Events.AddHandler(customDrawItemImage, value); }
			remove { this.Events.RemoveHandler(customDrawItemImage, value); }
		}
		[System.ComponentModel.Category("Layout")]
		public event GalleryFilterMenuEventHandler FilterMenuPopup {
			add { this.Events.AddHandler(filterMenuPopupEvent, value); }
			remove { this.Events.RemoveHandler(filterMenuPopupEvent, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryFilterMenuItemClickEventHandler FilterMenuItemClick {
			add { this.Events.AddHandler(filterMenuItemClickEvent, value); }
			remove { this.Events.RemoveHandler(filterMenuItemClickEvent, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemHover {
			add { this.Events.AddHandler(galleryItemHover, value); }
			remove { this.Events.RemoveHandler(galleryItemHover, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemLeave {
			add { this.Events.AddHandler(galleryItemLeave, value); }
			remove { this.Events.RemoveHandler(galleryItemLeave, value); }
		}
		[System.ComponentModel.Category("Action")]
		public event GallerySelectionEventHandler MarqueeSelectionCompleted {
			add { this.Events.AddHandler(marqueeSelectionCompleted, value); }
			remove { this.Events.RemoveHandler(marqueeSelectionCompleted, value); }
		}
		internal bool IsMarqueeSelection(Point downPoint, Point upPoint) {
			return Math.Abs(downPoint.X - upPoint.X) > 3 || Math.Abs(downPoint.Y - upPoint.Y) > 3;
		}
		protected internal virtual void OnSelectionComplete(BaseGalleryViewInfo viewInfo) {
			BeginUpdate();
			try {
				List<GalleryItem> selItems = new List<GalleryItem>();
				foreach(GalleryItemGroup group in Groups) {
					foreach(GalleryItem item in group.Items) {
						GalleryItemViewInfo info = viewInfo.GetItemInfo(item);
						if(info != null && viewInfo.Selection.IntersectsWith(info.Bounds)) {
							selItems.Add(info.Item);
						}
					}
				}
				OnSelectionComplete(selItems);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnSelectionComplete(List<GalleryItem> items) {
			if(!AddToSelectionModifierKeyPressed) {
				SetItemsCheckState(false, items);
			}
			foreach(GalleryItem item in items) {
				SetItemCheck(item, true, false);
			}
			RaiseGallerySelectionComplete(items);
		}
		protected internal void RaiseGallerySelectionComplete(List<GalleryItem> items) {
			GallerySelectionEventHandler handler = Events[marqueeSelectionCompleted] as GallerySelectionEventHandler;
			if(handler != null)
				handler(this, new GallerySelectionEventArgs(this, items));
		}
		protected internal void RaiseFilterMenuPopup(ImageGalleryFilterMenu filterMenu) {
			GalleryFilterMenuEventHandler handler = this.Events[filterMenuPopupEvent] as GalleryFilterMenuEventHandler;
			if(handler == null) return;
			handler(this, new GalleryFilterMenuEventArgs(filterMenu));
		}
		protected internal void RaiseGalleryItemHover(GalleryItem item) {
			GalleryItemEventHandler handler = this.Events[galleryItemHover] as GalleryItemEventHandler;
			if(handler == null) return;
			handler(this, new GalleryItemEventArgs(this, item));
		}
		protected internal void RaiseGalleryItemLeave(GalleryItem item) {
			GalleryItemEventHandler handler = this.Events[galleryItemLeave] as GalleryItemEventHandler;
			if(handler == null) return;
			handler(this, new GalleryItemEventArgs(this, item));
		}
		protected internal void RaiseFilterMenuItemClick(ImageGalleryFilterMenu filterMenu, DXMenuCheckItem item) {
			GalleryFilterMenuItemClickEventHandler handler = this.Events[filterMenuItemClickEvent] as GalleryFilterMenuItemClickEventHandler;
			if(handler == null) return;
			handler(this, new GalleryFilterMenuClickEventArgs(filterMenu, item));
		}
		bool IAppearanceOwner.IsLoading {
			get { return Ribbon == null || !Ribbon.IsHandleCreated || IsLoading; }
		}
		protected internal void FindItem(GalleryItem item, ref int groupIndex, ref int itemIndex) {
			for(int i = 0; i < Groups.Count; i++) {
				for(int j = 0; j < Groups[i].Items.Count; j++) {
					if(item == Groups[i].Items[j]) {
						groupIndex = i;
						itemIndex = j;
						return;
					}
				}
			}
		}
		protected internal virtual void OnGalleryGroupCaptionControlChanged(GalleryItemGroup galleryItemGroup, Control prevControl, Control newControl) {
		}
		protected internal virtual bool IsGroupCaptionControlAllowed { get { return false; } }
		protected internal virtual void OnItemVisibleChanged(GalleryItem galleryItem) {
			RefreshGallery();
		}
		IComponentChangeService componentChangeService;
		protected internal virtual void FireGalleryChanged() {
			if(!DesignModeCore)
				return;
			if(componentChangeService == null) {
				if(Site != null)
					componentChangeService = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			}
			if(componentChangeService != null)
				componentChangeService.OnComponentChanged(this, null, null, null);
		}
		protected internal virtual void OnItemPropertiesChanged(string propName, bool updateLayout) {
		}
		protected internal virtual void OnItemCollectionChanged(GalleryItemCollection galleryItemCollection) {
		}
		protected internal virtual void OnGroupCollectionChanged(GalleryItemGroupCollection galleryItemGroupCollection) {
		}
		private static object getThumbnailImage = new object();
		private static object getLoadingImage = new object();
		public event GalleryThumbnailImageEventHandler GetThumbnailImage {
			add { Events.AddHandler(getThumbnailImage, value); }
			remove { Events.RemoveHandler(getThumbnailImage, value); }
		}
		public virtual ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			GalleryThumbnailImageEventArgs args = (GalleryThumbnailImageEventArgs)e;
			GalleryThumbnailImageEventHandler handler = (GalleryThumbnailImageEventHandler)Events[getThumbnailImage];
			if(handler != null) handler(this, args);
			return e;
		}
		public event GetGalleryLoadingImageEventHandler GetLoadingImage {
			add { Events.AddHandler(getLoadingImage, value); }
			remove { Events.RemoveHandler(getLoadingImage, value); }
		}
		public virtual Image RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			GetLoadingImageEventHandler handler = (GetLoadingImageEventHandler)Events[getLoadingImage];
			if(handler != null) handler(this, e);
			return e.LoadingImage;
		}
		public virtual void AddAnimation(ImageLoadInfo info) { }
		public virtual void ForceItemRefresh(ImageLoadInfo imageInfo) { }
		protected internal virtual void RefreshItem(GalleryImageLoadInfo info) {
			GalleryItem item = info.Item;
			item.LockRefresh();
			item.Image = info.ThumbImage;
			item.UnlockRefresh();
			RemoveItemFromAnimation(item);
			RefreshItemCore(info);
		}
		protected internal virtual void RemoveItemFromAnimation(GalleryItem item) {
		}
		protected internal void RefreshItemCore(GalleryImageLoadInfo info) {
			GalleryItem item = info.Item;
			if(item == null || item.Gallery == null) return;
			GalleryItemViewInfo vi = item.Gallery.GetItemInfo(item);
			if(vi != null) {
				vi.UpdateImageContentBounds();
				Rectangle rect = GetInvalidateRectangle(vi);
				item.Gallery.Invalidate(rect);
			}
			else { item.Gallery.Invalidate(item); }
		}
		protected internal Rectangle GetInvalidateRectangle(GalleryItemViewInfo vi) {
			if(OptionsImageLoad.AnimationType == ImageContentAnimationType.Slide) {
				Size size = new Size((int)(vi.ImageBounds.Width + ImageShowingAnimationHelper.OutsideWidth), (int)(vi.ImageBounds.Height + ImageShowingAnimationHelper.OutsideHeight));
				return new Rectangle(vi.ImageBounds.Location, size);
			}
			if(OptionsImageLoad.AnimationType == ImageContentAnimationType.Push) {
				Point location = new Point(vi.ImageBounds.X, vi.ImageBounds.Y - ImageShowingAnimationHelper.EasyOutAnimationOdds);
				Size size = new Size(vi.ImageBounds.Width, vi.ImageBounds.Height + ImageShowingAnimationHelper.EasyOutAnimationOdds + ImageShowingAnimationHelper.EasyInAnimationOdds);
				return new Rectangle(location, size);
			}
			return vi.ImageBounds;
		}
		Random rand = new Random();
		protected internal void OnRunAnimation(ImageLoadInfo info) {
			RemoveImageShowingAnimations(true);
			int delay = OptionsImageLoad.RandomShow ? rand.Next() % 300 : 0;
			if(info.RenderImageInfo == null) return;
			XtraAnimator.Current.AddAnimation(new GalleryImageShowingAnimationInfo(this, info.InfoId, info.RenderImageInfo, 1000 + delay, delay));
		}
		protected internal virtual void RemoveImageShowingAnimations(bool onlyInvisible) {
			if(!OptionsImageLoad.AsyncLoad) return;
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				if(XtraAnimator.Current.Animations[i] is GalleryImageShowingAnimationInfo) {
					GalleryImageShowingAnimationInfo ai = (GalleryImageShowingAnimationInfo)XtraAnimator.Current.Animations[i];
					RemoveImageShowingAnimationCore(ai, onlyInvisible);
				}
			}
		}
		protected virtual bool IsItemVisible(GalleryItemGroup group, int itemIndex) {
			for(int i = 0; i < Groups.Count; i++) {
				if(group == Groups[i]) return IsItemVisible(i, itemIndex);
			}
			return false;
		}
		protected virtual bool IsItemVisible(int groupIndex, int itemIndex) {
			return false;
		}
		protected internal void RemoveImageShowingAnimationCore(GalleryImageShowingAnimationInfo ai, bool onlyInvisible) {
			if(ai.Item == null || !(ai.Item.LoadInfo is GalleryImageLoadInfo)) return;
			GalleryImageLoadInfo info = (GalleryImageLoadInfo)ai.Item.LoadInfo;
			if(onlyInvisible && IsItemVisible(info.Item.GalleryGroup, info.DataSourceIndex)) return;
			ai.Item.LoadInfo.IsInAnimation = false;
			if(info.IsLoaded) {
				GalleryItem item = info.Item;
				item.LockRefresh();
				item.Image = info.ThumbImage;
				info.IsAnimationEnd = true;
				item.UnlockRefresh();
			}
			XtraAnimator.Current.Animations.Remove(ai);
		}
		[Browsable(false)]
		public bool CanAnimate {
			get { return true; }
		}
		[Browsable(false)]
		public virtual Control OwnerControl { get { return null; } }
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			SearchCriteriaInfo info = searchInfo as SearchCriteriaInfo;
			ApplyFindFilterCore(info.CriteriaOperator);
		}
		protected virtual void ApplyFindFilterCore(DevExpress.Data.Filtering.CriteriaOperator criteriaOperator) {
			foreach(GalleryItemGroup group in Groups) {
				((ISupportSearchDataAdapter)group.Items).FilterCriteria = criteriaOperator;
				group.Visible = group.Items.Count != 0 || Object.Equals(criteriaOperator, null);
			}
			RefreshGallery();
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() { return CreateSearchProviderCore(); }
		protected virtual SearchControlProviderBase CreateSearchProviderCore() { return new GalleryControlCriteriaProvider(); }
		bool ISearchControlClient.IsAttachedToSearchControl { get { return searchControl != null; } }
		protected virtual bool IsAttachedToSearchControl { get { return searchControl != null; } }
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) { SetSearchControlCore(searchControl); }
		protected virtual void SetSearchControlCore(ISearchControl searchControl){
			if(!AllowSearch) return;
			if(this.searchControl == searchControl) return;
			this.searchControl = searchControl;
			ApplyFindFilterCore(null);
		}
		protected virtual bool AllowSearch { get { return true; } }
		ISearchControl searchControl;
		protected internal abstract void UpdateVisual();
	}
	[ToolboxItem(false)]
	public class InRibbonGallery : BaseGallery, IImageCollectionHelper {
		private static readonly object initDropDownGallery = new object();
		private static readonly object popupCloseEvent = new object();
		int minimumColumnCount;
		RibbonGalleryBarItem ownerItem;
		public InRibbonGallery(RibbonGalleryBarItem ownerItem)
			: base() {
			this.minimumColumnCount = 1;
			this.ownerItem = ownerItem;
		}
		protected internal override void UpdateVisual() {
			OwnerItem.OnItemChanged(true, true);
		}
		RibbonControlStyle ribbonStyle = RibbonControlStyle.Default;
		protected internal RibbonControlStyle GetRibbonStyle() {
			if(OwnerItem.Ribbon.MergeOwner != null)
				return OwnerItem.Ribbon.MergeOwner.GetRibbonStyle();
			return OwnerItem.Ribbon.GetRibbonStyle();
		}
		protected internal override GalleryControlPainter Painter {
			get {
				RibbonControlStyle current = GetRibbonStyle();
				if((current == RibbonControlStyle.MacOffice || ribbonStyle == RibbonControlStyle.MacOffice) && current != ribbonStyle)
					ResetPainter();
				ribbonStyle = current;
				return base.Painter;
			}
		}
		protected internal override bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(Ribbon); } }
		protected internal override bool Enabled { get { return OwnerItem != null && OwnerItem.Enabled && Ribbon.Enabled; } }
		protected override void Dispose(bool disposing) {
			if(disposing && GalleryDropDown != null)
				GalleryDropDown.Dispose();
			base.Dispose(disposing);
		}
		public override void Invalidate() {
			if(OwnerItem != null) OwnerItem.OnItemChanged(true);
		}
		public override void Invalidate(Rectangle rect) {
			if(Ribbon != null) Ribbon.Invalidate(rect);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BaseDesignTimeManager GetDesignTimeManager() {
			return DesignTimeManager;
		}
		protected override BaseDesignTimeManager DesignTimeManager {
			get { 
				return Ribbon != null? Ribbon.ViewInfo.DesignTimeManager: null;
			}
		}
		protected internal override bool DesignModeCore { 
			get {
				if(Ribbon == null) return false;
				return Ribbon.IsDesignMode; 
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool FixedImageSize {
			get { return true; }
			set { }
		}
		public override void Invalidate(GalleryItem item) {
			if(OwnerItem != null) OwnerItem.OnItemChanged(true);
		}
		public override void MakeVisible(GalleryItem item) {
			if(OwnerItem != null) OwnerItem.MakeVisible(item);
		}
		public override void RefreshGallery() {
			if(IsLockUpdate || IsLoading) return;
			if(OwnerItem != null) OwnerItem.OnItemChanged(false);
		}
		protected internal override BaseGalleryViewInfo CreateViewInfo() { return new InRibbonGalleryViewInfo(this); }
		protected override GalleryControlPainter CreatePainter() {
			if(GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return new MacStyleInRibbonGalleryPainter();
			return new InRibbonGalleryPainter(); 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonGalleryBarItem OwnerItem { 
			get { return ownerItem; } 
			set { ownerItem = value; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GalleryDropDown GalleryDropDown {
			get { return OwnerItem.GalleryDropDown; }
			set { OwnerItem.GalleryDropDown = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("InRibbonGalleryMinimumColumnCount"),
#endif
System.ComponentModel.Category("Behavior"), DefaultValue(1)]
		public int MinimumColumnCount {
			get { return minimumColumnCount; }
			set {
				if(minimumColumnCount == value) return;
				minimumColumnCount = value;
				LayoutChanged();
			}
		}
		protected internal override void UpdateGallery() {
			if(IsLockUpdate) return;
			if(OwnerItem != null) OwnerItem.OnItemChanged(false);
		}
		[System.ComponentModel.Category("Layout")]
		public event InplaceGalleryEventHandler InitDropDownGallery {
			add { this.Events.AddHandler(initDropDownGallery, value); }
			remove { this.Events.RemoveHandler(initDropDownGallery, value); }
		}
		[System.ComponentModel.Category("Layout")]
		public event InplaceGalleryEventHandler PopupClose {
			add { this.Events.AddHandler(popupCloseEvent, value); }
			remove { this.Events.RemoveHandler(popupCloseEvent, value); }
		}
		protected internal void RaiseInitDropDownGallery(RibbonGalleryBarItemLink inplaceGallery, InDropDownGallery popupGallery) {
			InplaceGalleryEventHandler handler = this.Events[initDropDownGallery] as InplaceGalleryEventHandler;
			if(handler == null) return;
			handler(this, new InplaceGalleryEventArgs(OwnerItem, popupGallery));
		}
		protected internal void RaisePopupCloseEvent(InDropDownGallery popupGallery) {
			InplaceGalleryEventHandler handler = this.Events[popupCloseEvent] as InplaceGalleryEventHandler;
			if(handler == null) return;
			handler(this, new InplaceGalleryEventArgs(OwnerItem, popupGallery));
		}
		protected internal override BarAndDockingController GetController() {
			if(Ribbon == null) return null;
			if(Ribbon.IsMerged && Ribbon.MergeOwner != null)
				return Ribbon.MergeOwner.GetController();
			return Ribbon.GetController();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonControl GetRibbon() {
			return Ribbon;
		}
		protected internal override RibbonControl Ribbon {
			get {
				if(OwnerItem.Ribbon != null) return OwnerItem.Ribbon;
				return null;
			}
		}
		Control IImageCollectionHelper.OwnerControl {
			get { return Ribbon; }
		}
		protected override bool ShouldRaiseItemHover {
			get {
				if(Ribbon == null) return false;
				RibbonControl actualRibbon = Ribbon;
				if(Ribbon.IsMerged) actualRibbon = Ribbon.GetMdiParentRibbon();
				if(!actualRibbon.ViewInfo.HotObject.InGallery || HoverHitInfo == null) return false;
				if(HoverHitInfo.GalleryItem == actualRibbon.ViewInfo.HotObject.GalleryItem) return true;
				return false;
			}
		}
		public override void AddAnimation(ImageLoadInfo info) {
			if(Ribbon == null || !Ribbon.IsHandleCreated) return;
			Ribbon.Invoke(new Action<ImageLoadInfo>(OnRunAnimation), info);
		}
		public override void ForceItemRefresh(ImageLoadInfo imageInfo) {
			GalleryImageLoadInfo info = imageInfo as GalleryImageLoadInfo;
			if(info == null || Ribbon == null || !Ribbon.IsHandleCreated) return;
			Ribbon.Invoke(new Action<GalleryImageLoadInfo>(RefreshItem), info);
		}
	}
	public enum ShowScrollBar { Show, Hide, Auto }
	[ToolboxItem(false)]
	public class StandaloneGallery : BaseGallery, IImageCollectionHelper, ISupportAnimatedScroll, IContextItemCollectionOptionsOwner, IContextItemCollectionOwner {
		private static readonly object contextButtonCustomize = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object customContextButtonToolTip = new object();
		private static readonly object itemRightClickEvent = new object();
		internal static int ScrollBarSmallChanged = 10;
		StandaloneGalleryViewInfo viewInfo;
		DevExpress.XtraEditors.ScrollBarBase scrollBar;
		bool allowFilter;
		ImageGalleryFilterMenu menu;
		string filterCaption;
		int minimumWidth = 0;
		int scrollYPosition = 0;
		ShowScrollBar showScrollBar = ShowScrollBar.Show;
		GallerySizeMode autoSize;
		bool useOptimizedScrolling;
		bool stretchItems;
		public StandaloneGallery() {
			this.filterCaption = string.Empty;
			this.allowFilter = true;
			this.menu = null;
			this.scrollBar = new DevExpress.XtraEditors.VScrollBar();
			this.ScrollBar.Scroll += new ScrollEventHandler(OnScroll);
			this.autoSize = GallerySizeMode.Default;
			this.useOptimizedScrolling = true;
			this.stretchItems = false;
		}
		protected internal override void UpdateVisual() {
			ViewInfo.UpdateVisual();
			Invalidate();
			if(OwnerControl != null) 
				OwnerControl.Update();
		}
		protected internal override bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(OwnerControl); } }
		protected override void AssignParams(BaseGallery gallery) {
			base.AssignParams(gallery);
			StandaloneGallery sg = gallery as StandaloneGallery;
			if(sg == null)
				return;
			this.autoSize = sg.AutoSize;
			this.allowFilter = sg.AllowFilter;
			this.filterCaption = sg.FilterCaption;
			this.minimumWidth = sg.MinimumWidth;
			this.showScrollBar = sg.ShowScrollBar;
			this.useOptimizedScrolling = sg.UseOptimizedScrolling;
			this.stretchItems = sg.StretchItems;
		}
		protected override GalleryItem GetFocusedItem() {
			return ViewInfo == null || ViewInfo.KeyboardSelectedItem == null? null : ViewInfo.KeyboardSelectedItem.Item;
		}
		protected override void SetItemFocused(GalleryItem item) {
			if(ViewInfo == null)
				return;
			GalleryItemViewInfo vi = GetItemInfo(item);
			if(vi == null)
				return;
			ViewInfo.SetKeyboardSelectedItem(vi);
		}
		protected internal override GalleryItemGroupViewInfo GetGroupInfo(GalleryItemGroup group) {
			return ViewInfo.GetGroupInfo(group);
		}
		protected internal override GalleryItemViewInfo GetItemInfo(GalleryItem item) {
			return ViewInfo.GetItemInfo(item);
		}
		protected void RemoveScrollBar() {
			ScrollBar.Scroll -= new ScrollEventHandler(OnScroll);
			OwnerControl.Controls.Remove(ScrollBar);
		}
		protected void AddScrollBar(ScrollBarBase scroll) {
			this.scrollBar = scroll;
			ScrollBar.Scroll +=new ScrollEventHandler(OnScroll);
			if(OwnerControl != null) {
				OwnerControl.Controls.Add(scroll);
			}
		}
		protected override BaseDesignTimeManager DesignTimeManager {
			get {
				return Ribbon != null ? Ribbon.ViewInfo.DesignTimeManager : null;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.ScrollBar.Scroll -= new ScrollEventHandler(OnScroll);
				this.scrollBar.Dispose();
			}
			base.Dispose(disposing);
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler ItemRightClick {
			add { this.Events.AddHandler(itemRightClickEvent, value); }
			remove { this.Events.RemoveHandler(itemRightClickEvent, value); }
		}
		protected internal void RaiseItemRightClickEvent(BaseGallery gallery, GalleryItem item) {
			GalleryItemClickEventHandler handler = this.Events[itemRightClickEvent] as GalleryItemClickEventHandler;
			if(handler == null) return;
			handler(this, new GalleryItemClickEventArgs(null, gallery, item));
		}
		internal void OnRightItemClick(StandaloneGallery gallery, GalleryItem galleryItem) {
			RaiseItemRightClickEvent(gallery, galleryItem);
		}
		protected internal override void OnGalleryGroupCaptionControlChanged(GalleryItemGroup galleryItemGroup, Control prevControl, Control newControl) {
		}
		protected internal override void ClearAnimatedItems() {
			if(ViewInfo != null) ViewInfo.ClearAnimatedItems();
		}
		protected internal GallerySizeMode GetAutoSize() { return AutoSize == GallerySizeMode.Default ? GallerySizeMode.None : AutoSize; }
		protected internal override Point ToolTipPointToScreen(Point pt) {
			if(OwnerControl == null) return base.ToolTipPointToScreen(pt);
			return OwnerControl.PointToScreen(pt);
		}
		protected internal virtual StandaloneGalleryViewInfo ViewInfo {
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo() as StandaloneGalleryViewInfo;
				return viewInfo;
			}
		}
		public override void MakeVisible(GalleryItem item) {
			if(OwnerControl == null || OwnerControl.IsDisposed)
				return;
			ViewInfo.MakeVisible(item);
		}
		protected internal override RibbonControl Ribbon {
			get { return null; }
		}
		protected internal virtual GalleryItemViewInfo SelectedItemInfo { 
			get { 
				if(ViewInfo == null) return null;
				foreach(GalleryItemGroupViewInfo gr in ViewInfo.Groups) {
					foreach(GalleryItemViewInfo itemInfo in gr.Items) {
						if(itemInfo.Item.Checked) return itemInfo;
					}
				}
				return null;
			} 
		}
		bool firstMove = true;
		internal bool FirstMove { get { return firstMove; } set { firstMove = value; } }
		protected internal virtual void MakeVisibleSelectedItem() {
			if (ViewInfo != null && SelectedItemInfo != null) ViewInfo.MakeVisible(SelectedItemInfo.Item);
		}
		protected internal virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			RibbonHitInfo hi = ViewInfo.CalcHitInfo(point);
			if(hi.InGalleryItem) {
				IContextItemCollectionOwner owner = this as IContextItemCollectionOwner;
				if(!owner.IsDesignMode) {
					ToolTipControlInfo contextInfo = hi.GalleryItemInfo.ContextButtonsViewInfo.GetToolTipInfo(point);
					if(contextInfo != null && !AllowHoverImages)
						return contextInfo;
				}
				ToolTipControlInfo info = hi.GalleryItem.GetToolTipInfo(hi, point);
				if(AllowHoverImages) return hi.GalleryItem.GetHoverTooltip(info, hi);
				return info;
			}
			return null;
		}
		protected internal virtual void SetKeyboardSelectedItem() {
			if(ViewInfo != null) ViewInfo.SetKeyboardSelectedItem(SelectedItemInfo);
		}
		public override void RefreshGallery() {
			if(IsLockUpdate || IsLoading) return;
			ViewInfo.Reset();
			ViewInfo.CalcViewInfo(ViewInfo.Bounds);
		}
		protected internal override void UpdateGallery() {
			if(IsLockUpdate || IsLoading) return;
			ViewInfo.SetDirty();
			ViewInfo.CalcViewInfo(ViewInfo.Bounds);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollYPosition {
			get { return scrollYPosition; }
			set {
				int oldValue = ScrollYPosition;
				value = Math.Max(ViewInfo.MinScrollYPosition, value);
				value = Math.Min(ViewInfo.MaxScrollYPosition, value);
				if(scrollYPosition == value) return;
				scrollYPosition = value;
				OnScrollYPositionChanged(oldValue);
			}
		}
		protected virtual int GetScrollDelta(Rectangle bounds, VertAlignment alignment) {
			if(alignment == VertAlignment.Center)
				return ViewInfo.GetDefineCoor(bounds.Location) - (ViewInfo.GetDefineSize(ViewInfo.GalleryContentBounds.Size) - ViewInfo.GetDefineSize(bounds.Size)) / 2 - ViewInfo.GetDefineCoor(ViewInfo.GalleryContentBounds.Location);
			if(alignment == VertAlignment.Bottom)
				return ViewInfo.GetDefineEndCoor(bounds) - ViewInfo.GetDefineEndCoor(ViewInfo.GalleryContentBounds);
			return ViewInfo.GetDefineCoor(bounds.Location) - ViewInfo.GetDefineCoor(ViewInfo.GalleryContentBounds.Location);
		}
		const float ScrollTime = 0.7f;
		protected virtual void CheckViewInfo() { 
		}
		public override void ScrollTo(GalleryItem item, bool bAnimated, VertAlignment itemAlignment) {
			CheckViewInfo();
			GalleryItemViewInfo itemInfo = ViewInfo.GetItemInfo(item);
			if(itemInfo == null)
				return;
			if(!bAnimated) {
				ScrollYPosition += GetScrollDelta(itemInfo.Bounds, itemAlignment);
				return;
			}
			ScrollHelper.Scroll(ScrollYPosition, ScrollYPosition + GetScrollDelta(itemInfo.Bounds, itemAlignment), ScrollTime / ScrollSpeed, ScrollMode == GalleryScrollMode.Smooth);
		}
		public override void ScrollTo(GalleryItemGroup group, bool bAnimated, VertAlignment groupAlignment) {
			CheckViewInfo();
			GalleryItemGroupViewInfo groupInfo = ViewInfo.GetGroupInfo(group);
			if(groupInfo == null)
				return;
			if(!bAnimated) {
				ScrollYPosition += GetScrollDelta(groupInfo.Bounds, groupAlignment);
				return;
			}
			ScrollHelper.Scroll(ScrollYPosition, ScrollYPosition + GetScrollDelta(groupInfo.Bounds, groupAlignment), ScrollTime / ScrollSpeed, ScrollMode == GalleryScrollMode.Smooth);
		}
		protected virtual void OnScrollYPositionChanged(int oldValue) {
			ViewInfo.IsReady = false;
			ViewInfo.CheckViewInfo();
			Invalidate();
		}
		internal int MinimumWidth { get { return minimumWidth; } set { minimumWidth = value; } }
		public override void Invalidate(Rectangle rect) {
			if(OwnerControl != null) OwnerControl.Invalidate(rect);
		}
		public override void Invalidate() {
			if(OwnerControl != null) OwnerControl.Invalidate(ViewInfo.Bounds);
		}
		protected override bool DefaultShowGroupCaption { get { return true; } }
		protected internal new StandaloneGalleryPainter Painter { get { return base.Painter as StandaloneGalleryPainter; } }
		protected internal override BaseGalleryViewInfo CreateViewInfo() { return new StandaloneGalleryViewInfo(this); }
		protected override GalleryControlPainter CreatePainter() { return new StandaloneGalleryPainter(); }
		[ Browsable(false)]
		protected internal ScrollBarBase ScrollBar { get { return scrollBar; } }
		protected internal bool ScrollBarVisible { get { return ScrollBar != null && ScrollBar.Visible; } }
		protected void SetScrollBar(ScrollBarBase scrollBar) { this.scrollBar = scrollBar; }
		protected virtual void UpdateScrollBarVisibility() {
			ScrollYPosition = 0;
			UpdateScrollBarVisibilityCore();
			RefreshGallery();
		}
		protected internal virtual void UpdateScrollBarVisibilityCore() {
			if(ShowScrollBar == ShowScrollBar.Show) ScrollBar.Visible = true;
			else if(ShowScrollBar == ShowScrollBar.Hide || ShowScrollBar == ShowScrollBar.Auto) ScrollBar.Visible = false;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneGalleryUseOptimizedScrolling"),
#endif
 DefaultValue(true)]
		public bool UseOptimizedScrolling {
			get { return useOptimizedScrolling; }
			set { useOptimizedScrolling = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneGalleryAutoSize"),
#endif
 DefaultValue(GallerySizeMode.Default)]
		public GallerySizeMode AutoSize {
			get { return autoSize; }
			set {
				if(AutoSize == value) return;
				autoSize = value;
				OnAutoSizeChanged();
			}
		}
		protected virtual void OnAutoSizeChanged() {
			LayoutChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneGalleryShowScrollBar"),
#endif
 DefaultValue(ShowScrollBar.Show)]
		public ShowScrollBar ShowScrollBar {
			get { return showScrollBar; }
			set {
				if(ShowScrollBar == value) return;
				showScrollBar = value;
				UpdateScrollBarVisibility();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneGalleryAllowFilter"),
#endif
DefaultValue(true)]
		public virtual bool AllowFilter {
			get { return allowFilter; }
			set {
				if(allowFilter == value) return;
				allowFilter = value;
				LayoutChanged();
			}
		}
		[
Category("Appearance"), DefaultValue(false)]
		public bool StretchItems {
			get { return stretchItems; }
			set {
				if(StretchItems == value) return;
				stretchItems = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneGalleryFilterCaption"),
#endif
Category("Appearance"), DefaultValue("")]
		public string FilterCaption {
			get { return filterCaption; }
			set {
				if(filterCaption == value) return;
				filterCaption = value;
				LayoutChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ImageGalleryFilterMenu FilterMenu { get { return menu; } }
		protected virtual IDXMenuManager MenuManager {
			get { return null; }
		}
		protected virtual ImageGalleryFilterMenu CreateFilterMenu() {
			return new ImageGalleryFilterMenu(this);
		}
		protected internal virtual void ShowFilterMenu() {
			if(OwnerControl == null) return;
			if(FilterMenu != null) {
				CloseMenu();
			}
			this.menu = CreateFilterMenu();
			this.menu.CloseUp += new EventHandler(OnFilterMenuCloseUp);
			FilterMenu.IsRightToLeft = IsRightToLeft;
			Point filterMenuLocation = new Point(IsRightToLeft ? ViewInfo.FilterAreaBounds.Right : ViewInfo.FilterAreaBounds.X, ViewInfo.FilterAreaBounds.Bottom);
			MenuManagerHelper.ShowMenu(FilterMenu, LookAndFeelCore, MenuManager, OwnerControl, filterMenuLocation);
			Invalidate();
		}
		internal long MenuCloseTime;
		void OnFilterMenuCloseUp(object sender, EventArgs e) {
			MenuCloseTime = DateTime.Now.Ticks;
			if(sender != FilterMenu)
				return;
			CloseMenu();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool MenuOpened { get { return this.menu != null; } }
		protected internal virtual void CloseMenu() {
			if(!MenuOpened) return;
			if(FilterMenu != null) {
				FilterMenu.HidePopup();
				if(FilterMenu != null)
					FilterMenu.Dispose();
			}
			this.menu = null;
			Invalidate();
		}
		protected internal override BarAndDockingController GetController() {
			if(Ribbon == null) return null;
			return Ribbon.GetController();
		}
		protected virtual void OnScroll(object sender, ScrollEventArgs e) {
			OnScrollCore(sender, e);
		}
		protected internal virtual void OnScrollCore(object sender, ScrollEventArgs e) {
			ScrollYPosition = e.NewValue;
			if(OwnerControl == null) return;
			OwnerControl.Invalidate(ViewInfo.Bounds);
			OwnerControl.Refresh();
		}
		protected virtual Control ImageCollectionOwnerControl {
			get {
				return null;
			}
		}
		Control IImageCollectionHelper.OwnerControl {
			get {
				return ImageCollectionOwnerControl;
			}
		}
		protected override bool ShouldRaiseItemHover {
			get {
				if(!ViewInfo.HitInfo.InGalleryItem) return false;
				if(ViewInfo.HitInfo.GalleryItem == HoverHitInfo.GalleryItem) return true;
				return false;
			}
		}
		private void ResetToolTipController() { ToolTipController = null; }
		private bool ShouldSerializeToolTipController() { return ToolTipController != null; }
		ToolTipController toolTipController;
		public virtual ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value)
					return;
				OnToolTipControllerChanging();
				toolTipController = value;
				OnToolTipContollerChanged();
			}
		}
		protected virtual void OnToolTipContollerChanged() {
		}
		protected virtual void OnToolTipControllerChanging() {
		}
		protected internal virtual void AccessibleNotifyClients(AccessibleEvents accessibleEvents, int objectIndex, int childIndex) {
		}
		protected virtual void InitializeOwnerControl() {
			if(OwnerControl == null || GetController() == null) 
				return;
			ScrollBar.LookAndFeel.ParentLookAndFeel = LookAndFeelCore;
			if(ToolTipController != null) {
				ToolTipController.DefaultController.RemoveClientControl(OwnerControl);
				ToolTipController.AddClientControl(OwnerControl);
			}
			else {
				ToolTipController.DefaultController.RemoveClientControl(OwnerControl);
				ToolTipController.DefaultController.AddClientControl(OwnerControl);
			}
		}
		protected virtual void ClearOwnerControl() {
			if(OwnerControl == null)
				return;
			OwnerControl.Controls.Remove(ScrollBar);
			if(ToolTipController != null)
				ToolTipController.RemoveClientControl(OwnerControl);
			else ToolTipController.DefaultController.RemoveClientControl(OwnerControl);
		}
		protected internal override ISkinProvider Provider {
			get {
				GalleryControl gallery = OwnerControl as GalleryControl;
				if(gallery != null && !gallery.LookAndFeel.UseDefaultLookAndFeel)
					return gallery.LookAndFeel;
				return base.Provider;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public StandaloneGalleryViewInfo GetViewInfo() {
			return ViewInfo;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonHitInfo CalcHitInfo(Point pt) {
			return ViewInfo.CalcHitInfo(pt);
		}
		#region ISupportSmoothScroll Members
		AnimatedScrollHelper scrollHelper;
		protected AnimatedScrollHelper ScrollHelper {
			get {
				if(scrollHelper == null)
					scrollHelper = new AnimatedScrollHelper(this);
				return scrollHelper;
			}
		}
		void ISupportAnimatedScroll.OnScroll(double currentScrollValue) {
			ScrollYPosition = (int)currentScrollValue;
		}
		void ISupportAnimatedScroll.OnScrollFinish() {
			RaiseEndScrollEvent();
		}
		#endregion
		protected internal override void RemoveItemFromAnimation(GalleryItem item) {
			ViewInfo.RemoveAnimatedItem(item);
		}
		protected override bool IsItemVisible(int groupIndex, int itemIndex) {
			if(ViewInfo.Groups.Count - 1 < groupIndex || ViewInfo.Groups[groupIndex].Items.Count - 1 < itemIndex) return false;
			return !ViewInfo.Groups[groupIndex].Items[itemIndex].IsInvisible;
		}
		#region ContextButtons
		ContextItemCollection contextButtons;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtons();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		protected virtual ContextItemCollection CreateContextButtons() {
			return new ContextItemCollection(this);
		}
		ContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null)
					contextButtonOptions = CreateContextButtonOptions();
				return contextButtonOptions;
			}
		}
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new ContextItemCollectionOptions(this);
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			LayoutChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Visibility") {
				Invalidate();
				if(OwnerControl != null)
					OwnerControl.Update();
			}
			else
				LayoutChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode { get { return DesignModeCore; } }
		bool IContextItemCollectionOwner.IsRightToLeft {
			get {
				if(OwnerControl != null)
					if(OwnerControl.RightToLeft == RightToLeft.Yes) return true;
				return false;
			}
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			LayoutChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.None; } }
		public event GalleryContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextButtonCustomize, value); }
			remove { Events.RemoveHandler(contextButtonCustomize, value); }
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		protected internal virtual void RaiseContextButtonCustomize(GalleryContextButtonCustomizeEventArgs e) {
			GalleryContextButtonCustomizeEventHandler handler = (GalleryContextButtonCustomizeEventHandler)Events[contextButtonCustomize];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event GalleryContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		protected internal virtual void RaiseCustomContextButtonToolTip(GalleryContextButtonToolTipEventArgs e) {
			GalleryContextButtonToolTipEventHandler handler = (GalleryContextButtonToolTipEventHandler)Events[customContextButtonToolTip];
			if(handler != null) handler(this, e);
		}
		#endregion
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class GalleryControlGallery : StandaloneGallery, IAppearanceOwner {
		GalleryControl galleryControl;
		GalleryControlHandler handler;
		Color backColor;
		Image backgroundImage;
		ImageLayoutMode backgroundImageLayout;
		Orientation orientation;
		bool autoFitColumns;
		HorzAlignment contentHorzAlignment;
		public GalleryControlGallery()
			: base() {
			this.galleryControl = null;
			this.backColor = Color.Empty;
			this.backgroundImageLayout = ImageLayoutMode.Stretch;
			this.orientation = Orientation.Vertical;
			this.autoFitColumns = true;
			this.contentHorzAlignment = HorzAlignment.Default;
		}
		protected internal override void FireGalleryChanged() {
			if(!DesignModeCore)
				return;
			if(GalleryControl != null)
				GalleryControl.FireGalleryChanged();
		}
		protected internal override bool Enabled {
			get {
				return GalleryControl != null ? GalleryControl.Enabled : base.Enabled;
			}
		}
		protected override void AssignParams(BaseGallery gallery) {
			base.AssignParams(gallery);
			GalleryControlGallery cg = gallery as GalleryControlGallery;
			if(cg == null)
				return;
			this.autoFitColumns = cg.AutoFitColumns;
			this.contentHorzAlignment = cg.ContentHorzAlignment;
			this.orientation = cg.Orientation;
			this.backColor = cg.BackColor;
			this.BackgroundImageLayout = cg.BackgroundImageLayout;
			this.backgroundImage = cg.BackgroundImage;
		}
		protected override void OnScrollYPositionChanged(int oldValue) {
			if(UseOptimizedScrolling) {
				base.OnScrollYPositionChanged(oldValue);
				return;
			}
			ViewInfo.IsReady = false;
			ViewInfo.CheckViewInfo();
			int delta = oldValue - ScrollYPosition;
			int y = delta < 0? -delta: 0;
			if(Orientation == Orientation.Vertical)
				WindowScroller.ScrollVertical(GalleryControl.Client, GalleryControl.Client.ClientRectangle, y, delta);
			else
				WindowScroller.ScrollHorizontal(GalleryControl.Client, GalleryControl.Client.ClientRectangle, y, delta); 
		}
		protected override void CheckViewInfo() {
			if(GalleryControl != null)
				GalleryControl.CheckViewInfo();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				ClearOwnerControl();
			}
		}
		public GalleryControlGallery(GalleryControl galleryControl) : this() {
			this.galleryControl = galleryControl;
			InitializeOwnerControl();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGalleryContentHorzAlignment"),
#endif
 DefaultValue(HorzAlignment.Default)]
		public HorzAlignment ContentHorzAlignment {
			get { return contentHorzAlignment; }
			set {
				if(ContentHorzAlignment == value) return;
				contentHorzAlignment = value;
				OnContentHorizAlignmentChanged();
			}
		}
		protected virtual void OnContentHorizAlignmentChanged() {
			RefreshGallery();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGalleryAutoFitColumns"),
#endif
 DefaultValue(true)]
		public bool AutoFitColumns {
			get { return autoFitColumns; }
			set {
				if(AutoFitColumns == value) return;
				autoFitColumns = value;
				OnAutoFitColumnsChanged();
			}
		}
		protected virtual void OnAutoFitColumnsChanged() {
			RefreshGallery();
		}
		protected override IDXMenuManager MenuManager {
			get {
				return GalleryControl != null ? GalleryControl.MenuManager : null;
			}
		}
		private void ResetBackColor() { BackColor = Color.Empty; }
		private bool ShouldSerializeBackColor() { return !BackColor.Equals(Color.Empty); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGalleryBackColor"),
#endif
 Category("Appearance")]
		public Color BackColor {
			get { return backColor; }
			set {
				if(BackColor == value)
					return;
				backColor = value;
				OnBackColorChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGalleryOrientation"),
#endif
 Category("Behavior"), DefaultValue(Orientation.Vertical)]
		public Orientation Orientation {
			get { return orientation; }
			set {
				if(Orientation == value)
					return;
				orientation = value;
				OnOrientationChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ToolTipController ToolTipController {
			get {
				return GalleryControl != null ? GalleryControl.ToolTipController : null;
			}
			set { 
				if(GalleryControl != null)
					GalleryControl.ToolTipController = value; 
			}
		}
		protected virtual void OnOrientationChanged() {
			RemoveScrollBar();
			if(Orientation == Orientation.Vertical)
				AddScrollBar(new DevExpress.XtraEditors.VScrollBar());
			else
				AddScrollBar(new DevExpress.XtraEditors.HScrollBar());
			RefreshGallery();
		}
		public override void Invalidate() {
			if(GalleryControl != null)
				GalleryControl.InvalidateGallery();
		}
		protected virtual void OnBackColorChanged() {
			if(GalleryControl != null)
				GalleryControl.InvalidateGallery();
		}
		protected internal override void OnGalleryGroupCaptionControlChanged(GalleryItemGroup galleryItemGroup, Control prevControl, Control newControl) {
			if(GalleryControl == null)
				return;
			GalleryControl.Client.Controls.Remove(prevControl);
			if(prevControl != null && GalleryControl.Parent != null) {
				if(GalleryControl.IsDesignMode)
					GalleryControl.Parent.Controls.Add(prevControl);
				prevControl.BringToFront();
			}
			if(newControl != null) {
				foreach(GalleryItemGroup group in Groups) {
					if(group.CaptionControl == newControl && group != galleryItemGroup) {
						group.SetCaptionControlCore(null);
						break;
					}
				}
			}
			ForceCalcViewInfo = true;
			try {
				ViewInfo.Reset();
				GalleryControl.Client.Controls.Add(newControl);
			} finally {
				ForceCalcViewInfo = false;
			}
		}
		protected internal override bool ImagePopupFormActAsPopup { get { return true; } }
		protected internal override bool IsGroupCaptionControlAllowed { get { return true; } }
		void ResetBackgroundImage() { BackgroundImage = null; }
		bool ShouldSerializeBackgroundImage() { return BackgroundImage != null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGalleryBackgroundImage"),
#endif
 Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image BackgroundImage {
			get { return backgroundImage; }
			set {
				if(BackgroundImage == value)
					return;
				backgroundImage = value;
				OnBackgroundImageChanged();
			}
		}
		private void ResetBackgroundImageLayout() { BackgroundImageLayout = ImageLayoutMode.Stretch; }
		private bool ShouldSerializeBackgroundImageLayout() { return BackgroundImageLayout != ImageLayoutMode.Stretch; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGalleryBackgroundImageLayout"),
#endif
 Category("Appearance")]
		public ImageLayoutMode BackgroundImageLayout {
			get { return backgroundImageLayout; }
			set {
				if(BackgroundImageLayout == value)
					return;
				backgroundImageLayout = value;
				OnBackgroundImageLayoutChanged();
			}
		}
		protected virtual void OnBackgroundImageLayoutChanged() {
			LayoutChanged();
		}
		protected virtual void OnBackgroundImageChanged() {
			LayoutChanged();
		}
		[Browsable(false)]
		public GalleryControl GalleryControl { 
			get { return galleryControl; } 
			internal set {
				if(GalleryControl == value)
					return;
				galleryControl = value;
				OnGalleryControlChanged();
			} 
		}
		protected void OnGalleryControlChanged() {
		}
		public override Control OwnerControl { get { return GalleryControl; } }
		protected internal GalleryControlHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void LayoutChanged() {
			if(GalleryControl == null || GalleryControl.IsLoading)
				return;
			base.LayoutChanged();
			GalleryControl.AdjustSize();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PopupGalleryEdit PopupGalleryEdit { get; set; }
		protected virtual GalleryControlHandler CreateHandler() {
			return new GalleryControlHandler(this);
		}
		protected internal override BarAndDockingController GetController() {
			return GalleryControl != null? GalleryControl.GetController() : null;
		}
		protected internal override BaseGalleryViewInfo CreateViewInfo() {
			return new GalleryControlGalleryViewInfo(this);
		}
		protected override GalleryControlPainter CreatePainter() {
			return new GalleryControlGalleryPainter();
		}
		protected internal override bool DesignModeCore {
			get { return GalleryControl != null ? GalleryControl.DesignModeCore : false; }
		}
		protected internal override bool IsLoading {
			get { return GalleryControl != null? GalleryControl.IsLoading : false; }
		}
		BaseDesignTimeManager designTimeManager;
		protected override BaseDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManager == null)
					designTimeManager = new BaseDesignTimeManager(this, GalleryControl.Site);
				return designTimeManager;
			}
		}
		public override void RefreshGallery() {
			if(IsLockUpdate || IsLoading || GalleryControl == null) return;
			if(ResetViewRequired)
				GalleryControl.ResetViewInfo();
			GalleryControl.CheckViewInfo();
			GalleryControl.InvalidateGallery();
			GalleryControl.RaiseSizeableChangedCore();
		}
		protected virtual bool ResetViewRequired { get { return !InToggleItemCheck; } }
		protected internal override void AccessibleNotifyClients(AccessibleEvents accessibleEvents, int objectIndex, int childIndex) {
			if(GalleryControl != null)
				GalleryControl.AccessibleNotifyClients(accessibleEvents, objectIndex, childIndex);
		}
		protected override void OnAutoSizeChanged() {
			if(GalleryControl != null)
				GalleryControl.OnGalleryAutoSizeChanged();
		}
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { 
				return GalleryControl != null && (GalleryControl.IsLoading);  
			}
		}
		#endregion
		public override void AddAnimation(ImageLoadInfo info) {
			if(GalleryControl == null || !GalleryControl.IsHandleCreated) return;
			GalleryControl.Invoke(new Action<ImageLoadInfo>(OnRunAnimation), info);
		}
		public override void ForceItemRefresh(ImageLoadInfo imageInfo) {
			GalleryImageLoadInfo info = imageInfo as GalleryImageLoadInfo;
			if(info == null || GalleryControl == null || !GalleryControl.IsHandleCreated) return;
			GalleryControl.Invoke(new Action<GalleryImageLoadInfo>(RefreshItem), info);
		}
	}
	public class InDropDownGallery : StandaloneGallery {
		GalleryDropDown galleryDropDown;
		GalleryDropDownBarControl barControl;
		GallerySizeMode sizeMode;
		bool autoHide;
		bool sizerBelow;
		bool synchWithInRibbonGallery;
		public InDropDownGallery() : base() {
			this.sizeMode = GallerySizeMode.Default;
			this.galleryDropDown = null;
			this.barControl = null;
			this.autoHide = true;
			this.sizerBelow = true;
			this.synchWithInRibbonGallery = false;
		}
		protected internal override bool IsRightToLeft { get { return OwnerControl == null ? false : GalleryDropDown.Manager.IsRightToLeft; } }
		public InDropDownGallery(GalleryDropDown galleryDropDown)
			: this() {
			this.galleryDropDown = galleryDropDown;
		}
		protected internal override void FireGalleryChanged() {
			if(GalleryDropDown != null)
				GalleryDropDown.FireChanged();
			base.FireGalleryChanged();
		}
		protected internal override RibbonControl Ribbon {
			get {
				if(GalleryDropDown != null) return GalleryDropDown.Ribbon;
				return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GalleryDropDown GalleryDropDown { get { return galleryDropDown; } }
		protected internal virtual bool SizerBelow { get { return sizerBelow; } set { sizerBelow = value; } }
		protected internal override BaseGalleryViewInfo CreateViewInfo() { return new InDropDownGalleryViewInfo(this); }
		protected override GalleryControlPainter CreatePainter() { return new InDropDownGalleryPainter(); }
		protected internal new InDropDownGalleryPainter Painter { get { return base.Painter as InDropDownGalleryPainter; } }
		protected internal new InDropDownGalleryViewInfo ViewInfo { get { return base.ViewInfo as InDropDownGalleryViewInfo; } }
		protected internal override BarAndDockingController GetController() {
			BarAndDockingController res = base.GetController();
			if(res != null)
				return res;
			if(GalleryDropDown.Manager != null)
				return GalleryDropDown.Manager.GetController();
			return null;
		}
		protected override void OnScrollYPositionChanged(int oldValue) {
			if(UseOptimizedScrolling) {
				base.OnScrollYPositionChanged(oldValue);
				return;
			}
			ViewInfo.IsReady = false;
			ViewInfo.CheckViewInfo();
			int delta = oldValue - ScrollYPosition;
			int y = delta < 0? -delta: 0;
			WindowScroller.ScrollVertical(BarControl, BarControl.ClientRectangle, y, delta);
		}
		public override Control OwnerControl { get { return BarControl; } }
		protected override ImageGalleryFilterMenu CreateFilterMenu() {
			ImageGalleryFilterMenu res = new ImageGalleryFilterMenu(this);
			res.OwnerPopup = GalleryDropDown;
			return res;
		}
		public override void MakeVisible(GalleryItem item) {
			if(Ribbon == null || Ribbon.IsDisposed)
				return;
			base.MakeVisible(item);
		}
		protected internal GalleryDropDownBarControl BarControl {
			get { return barControl; }
			set {
				if(BarControl != null)
					ClearOwnerControl();
				barControl = value;
				if(BarControl != null)
					InitializeOwnerControl();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SynchWithInRibbonGallery {
			get { return synchWithInRibbonGallery; }
			set { synchWithInRibbonGallery = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("InDropDownGalleryAutoHideGallery"),
#endif
 DefaultValue(true)]
		public bool AutoHideGallery {
			get { return autoHide; }
			set {
				if(AutoHideGallery == value) return;
				autoHide = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("InDropDownGallerySizeMode"),
#endif
 DefaultValue(GallerySizeMode.Default)]
		public GallerySizeMode SizeMode {
			get { return sizeMode; }
			set {
				if(SizeMode == value) return;
				sizeMode = value;
				LayoutChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ToolTipController ToolTipController {
			get { return Ribbon == null? null: Ribbon.ToolTipController; }
			set { }
		}
		protected override bool ShouldRaiseItemHover {
			get {
				if(Ribbon == null) return false;
				if(!ViewInfo.HitInfo.InGalleryItem || !GalleryDropDown.Visible) return false;
				if(ViewInfo.HitInfo.GalleryItem == HoverHitInfo.GalleryItem) return true;
				return false;
			}
		}
		protected override Control ImageCollectionOwnerControl {
			get {
				if(GalleryDropDown == null) return null;
				for(int i = 0; i < GalleryDropDown.Container.Components.Count; i++) {
					if(GalleryDropDown.Container.Components[i] is RibbonControl) return GalleryDropDown.Container.Components[i] as RibbonControl;
				}
				return null;
			}
		}
		public override void RefreshGallery() {
			if(Ribbon == null || Ribbon.Manager == null) {
				ViewInfo.IsReady = false;
				ViewInfo.Groups.Clear();				
				ViewInfo.IsCalculateBestSize = true;
				return;
			}
			base.RefreshGallery();
			if(Ribbon.Manager.IsCustomizing) {
				Invalidate();
			}
		}
		protected internal override void AccessibleNotifyClients(AccessibleEvents accessibleEvents, int objectIndex, int childIndex) {
			BarControl.AccessibleNotifyClients(accessibleEvents, objectIndex, childIndex);
		}
		protected override IDXMenuManager MenuManager {
			get { 
				if(Ribbon != null)
					return Ribbon.Manager;
				if(GalleryDropDown.Manager != null)
					return GalleryDropDown.Manager;
				return null;
			}
		}
		public override void AddAnimation(ImageLoadInfo info) {
			if(Ribbon == null || !Ribbon.IsHandleCreated) return;
			Ribbon.Invoke(new Action<ImageLoadInfo>(OnRunAnimation), info);
		}
		public override void ForceItemRefresh(ImageLoadInfo imageInfo) {
			GalleryImageLoadInfo info = imageInfo as GalleryImageLoadInfo;
			if(info == null || Ribbon == null || !Ribbon.IsHandleCreated) return;
			Ribbon.Invoke(new Action<GalleryImageLoadInfo>(RefreshItem), info);
		}
	}
	public class GalleryImageShowingAnimationInfo : ImageShowingAnimationInfo {
		public GalleryImageShowingAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms, int delay)
			: base(anim, animationId, imageInfo, ms, delay) {
				this.imageUpdated = false;
		}
		bool imageUpdated;
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame - FirstAnimationFrame)) / (FrameCount - FirstAnimationFrame);
			if(k >= 0) {
				Item.LoadInfo.IsLoaded = true;
				if(!imageUpdated) {
					imageUpdated = true;
					if(!(AnimatedObject is BaseGallery)) return;
					BaseGallery vi = (BaseGallery)AnimatedObject;
					vi.RefreshItem(Item.LoadInfo as GalleryImageLoadInfo);
				}
			}
			base.FrameStepCore();
		}
		protected override void Invalidate() {
			if(!(AnimatedObject is BaseGallery)) return;
			BaseGallery vi = (BaseGallery)AnimatedObject;
			if(vi.ImageSize != Item.LoadInfo.ImageMaxSize) Item.LoadInfo.IsAnimationEnd = true;
			vi.RefreshItemCore(Item.LoadInfo as GalleryImageLoadInfo);
		}
	}
	public class GalleryImageLoadInfo : ImageLoadInfo {
		GalleryItem item;
		public GalleryImageLoadInfo(int dataSourceIndex, GalleryItem item, ImageContentAnimationType animationType, ImageLayoutMode mode, Size maxSize, Size desiredSize)
			: base(dataSourceIndex, dataSourceIndex, animationType, mode, maxSize, desiredSize) {
			this.item = item;
		}
		public GalleryItem Item { get { return item; } }
	}
	public delegate void GalleryThumbnailImageEventHandler(object sender, GalleryThumbnailImageEventArgs e);
	public delegate void GetGalleryLoadingImageEventHandler(object sender, GetGalleryLoadingImageEventArgs e);
	public class GetGalleryLoadingImageEventArgs : GetLoadingImageEventArgs {
		GalleryItem item;
		public GetGalleryLoadingImageEventArgs(GalleryItem item, int dataSourceIndex) : base(dataSourceIndex) {
			this.item = item;
		}
		public GalleryItem Item { get { return item; } }
	}
	public class GalleryThumbnailImageEventArgs : ThumbnailImageEventArgs {
		GalleryItem item;
		public GalleryThumbnailImageEventArgs(GalleryItem item, int dataSourceIndex, AsyncImageLoader loader, ImageLoadInfo info)
			: base(dataSourceIndex, loader, info) {
			this.item = item;
		}
		public GalleryItem Item { get { return item; } }
	}
	public class GalleryAsyncImageLoader : AsyncImageLoader {
		public GalleryAsyncImageLoader(IAsyncImageLoaderClient viewInfo) : base(viewInfo) { }
		protected override Image RaiseGetLoadingImage(ImageLoadInfo info) {
			GalleryImageLoadInfo gvImageInfo = (GalleryImageLoadInfo)info;
			return ViewInfo.RaiseGetLoadingImage(new GetGalleryLoadingImageEventArgs(gvImageInfo.Item, gvImageInfo.DataSourceIndex));
		}
		protected override ThumbnailImageEventArgs RaiseGetThumbnailImage(ImageLoadInfo info) {
			GalleryImageLoadInfo gvImageInfo = (GalleryImageLoadInfo)info;
			return ViewInfo.RaiseGetThumbnailImage(new GalleryThumbnailImageEventArgs(gvImageInfo.Item, gvImageInfo.DataSourceIndex, this, gvImageInfo));
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GalleryOptionsImageLoad : BaseOptions {
		bool asyncLoad, randowShow;
		ImageContentAnimationType animationType;
		Size desiredThumbnailSize;
		public GalleryOptionsImageLoad() {
			this.asyncLoad = false;
			this.randowShow = true;
			this.animationType = ImageContentAnimationType.None;
			this.desiredThumbnailSize = Size.Empty;
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryOptionsImageLoadAsyncLoad"),
#endif
 DefaultValue(false)]
		public bool AsyncLoad {
			get { return asyncLoad; }
			set { asyncLoad = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryOptionsImageLoadAnimationType"),
#endif
 DefaultValue(ImageContentAnimationType.None)]
		public ImageContentAnimationType AnimationType {
			get { return animationType; }
			set { animationType = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryOptionsImageLoadRandomShow"),
#endif
 DefaultValue(true)]
		public bool RandomShow {
			get { return randowShow; }
			set { randowShow = value; }
		}
		void ResetDesiredThumbnailSize() { DesiredThumbnailSize = Size.Empty; }
		bool ShouldSerializeDesiredThumbnailSize() { return DesiredThumbnailSize != Size.Empty; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryOptionsImageLoadDesiredThumbnailSize")]
#endif
		public Size DesiredThumbnailSize {
			get { return desiredThumbnailSize; }
			set { desiredThumbnailSize = value; }
		}
		public override void Assign(BaseOptions options) {
			GalleryOptionsImageLoad optionsImageLoad = options as GalleryOptionsImageLoad;
			if(optionsImageLoad == null)
				return;
			this.animationType = optionsImageLoad.animationType;
			this.asyncLoad = optionsImageLoad.asyncLoad;
			this.randowShow = optionsImageLoad.randowShow;
			this.desiredThumbnailSize = optionsImageLoad.desiredThumbnailSize;
		}
	}
	public delegate void GalleryContextButtonCustomizeEventHandler(object sender, GalleryContextButtonCustomizeEventArgs e);
	public class GalleryContextButtonCustomizeEventArgs : EventArgs {
		public GalleryContextButtonCustomizeEventArgs(ContextItem item, GalleryItem galleryItem) {
			Item = item;
			GalleryItem = galleryItem;
		}
		public ContextItem Item { get; private set; }
		public GalleryItem GalleryItem { get; private set; }
	}
	public delegate void GalleryContextButtonToolTipEventHandler(object sender, GalleryContextButtonToolTipEventArgs e);
	public class GalleryContextButtonToolTipEventArgs : EventArgs {
		GalleryItem galleryItem;
		ContextButtonToolTipEventArgs contextToolTipArgs;
		public GalleryContextButtonToolTipEventArgs(GalleryItem galleryItem, ContextButtonToolTipEventArgs contextToolTipArgs) {
			this.galleryItem = galleryItem;
			this.contextToolTipArgs = contextToolTipArgs;
		}
		public GalleryItem GalleryItem { get { return galleryItem; } }
		public ContextItem Item { get { return contextToolTipArgs.Item; } }
		public object Value { get { return contextToolTipArgs.Value; } }
		public string Text {
			get { return contextToolTipArgs.Text; }
			set { contextToolTipArgs.Text = value; }
		}
	}
}
