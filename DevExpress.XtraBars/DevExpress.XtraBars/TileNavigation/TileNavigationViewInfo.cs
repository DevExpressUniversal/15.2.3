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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Navigation {
	class OfficeNavigationBarViewInfo : TileControlViewInfo {
		public OfficeNavigationBarViewInfo(ITileControl control) : base(control) { }
		protected internal NavigationBarCore OwnerCore { get { return Owner as NavigationBarCore; } }
		public override bool IsRightToLeft {
			get {
				if(OwnerCore != null && OwnerCore.Owner is OfficeNavigationBar) {
					return (OwnerCore.Owner as OfficeNavigationBar).IsRightToLeft;
				}
				return false;
			}
		}
		protected internal void ForceUpdateItemsAppearances() {
			foreach(var itemCache in ItemViewInfoCache) {
				var itemInfo = itemCache.Value as TileItemViewInfo;
				if(itemInfo != null) itemInfo.ForceUpdateAppearanceColors();
			}
		}
		protected internal Skin GetCommonSkin() {
			return CommonSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel);
		}
		protected override AppearanceDefault GetDefaultAppearance() {
			return new AppearanceDefault(GetDefaultBackColor()); 
		}
		Color GetDefaultBackColor() {
			var element = GetOfficeNavigationBarSkinElement();
			if(element != null && !OwnerCore.AllowItemSkinning)
				return element.Color.GetBackColor();
			return Owner.BackColor;
		}
		protected override int DragItemBoundsAnimationDelta { get { return 0; } }
		protected override int DragItemBoundsScaleDelta { get { return 0; } }
		protected override int PressedItemBoundsDelta { get { return 2; } }
		protected override int PressedItemBoundsAnimationLength { get { return 20; } }
		protected override int DragModeItemBoundsAnimationLength { get { return 5; } }
		protected override int TileItemTransitionLength { get { return 200; } }
		protected override int ClipBoundsTopIndent { get { return 0; } }
		protected override Rectangle ConstraintPadding(Rectangle rect) { return rect; }
		public override bool IsReady {
			get { return base.IsReady; }
			protected set {
				if(value == false)
					OwnerCore.ButtonsPanel.ViewInfo.SetDirty();
				base.IsReady = value;
			}
		}
		public bool CheckItemIsVisible(TileItem item) { 
			if(HiddenItems.Contains(item)) return false;
			if(OwnerCore.MaxItemCount > -1 && GetItemGlobalIndex(item) >= OwnerCore.MaxItemCount) return false;
			return true;
		}
		public int GetItemGlobalIndex(TileItem item) {
			int index = -1;
			foreach(TileGroup group in Owner.Groups) {
				if(!group.Visible) continue;
				foreach(TileItem tileItem in group.Items) {
					if(tileItem.Visible) index++;
					if(tileItem == item) return index;
				}
			}
			return index;
		}
		public TileGroupViewInfo GetLastVisibleGroupInfo() { 
			for(int i = Groups.Count - 1; i > -1; i--) {
				if(Groups[i].IsVisible)
					return Groups[i];
			}
			return Groups.Count == 0 ? null : Groups[0];
		}
		public override Size GetItemSize(TileItemViewInfo itemInfo) {
			return GetItemSizeCore(itemInfo);
		}
		protected virtual Size GetItemSizeCore(TileItemViewInfo itemInfo) {
			if(Owner.Properties.Orientation == Orientation.Horizontal) return GetItemSizeHor(itemInfo);
			return GetItemSizeVer(itemInfo);
		}
		private Size GetItemSizeVer(TileItemViewInfo itemInfo) {
			if(string.IsNullOrEmpty(itemInfo.Item.Text))
				return new Size(ContentBounds.Width, emptyWidth);
			Size size;
			if(OwnerCore.Compact)
				size = itemInfo.Item.Image == null ? new Size(0, emptyWidth) : itemInfo.Item.Image.Size;
			else
				size = ((NavigationBarItemViewInfo)itemInfo).GetTextSize(this.GInfo.Graphics);
			size.Width += itemInfo.ItemPadding.Horizontal;
			size.Height += itemInfo.ItemPadding.Vertical;
			UpdateMaxSize(size);
			return new Size(ContentBounds.Width, size.Height);
		}
		protected virtual Padding GetItemPadding(TileItemViewInfo itemInfo) {
			SkinElement officeNavigationBar = GetOfficeNavigationBarItemSkinElement();
			if(officeNavigationBar != null && itemInfo.Item.Control.Properties.ItemPadding == OfficeNavigationBar.DefaultItemPadding){
				var edges = officeNavigationBar.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return itemInfo.ItemPadding;
		}
		private Size GetItemSizeHor(TileItemViewInfo itemInfo) {
			if(string.IsNullOrEmpty(itemInfo.Item.Text))
				return new Size(emptyWidth, ContentBounds.Height);
			Size size;
			if(OwnerCore.Compact)
				size = itemInfo.Item.Image == null ? new Size(emptyWidth, 0) : itemInfo.Item.Image.Size;
			else
				size = ((NavigationBarItemViewInfo)itemInfo).GetTextSize(this.GInfo.Graphics);
			size.Width += itemInfo.ItemPadding.Horizontal;
			size.Height += itemInfo.ItemPadding.Vertical;
			UpdateMaxSize(size);
			return new Size(size.Width, ContentBounds.Height);
		}
		const int emptyWidth = 50;
		static StringFormat stringFormatVertical;
		public static StringFormat StringFormatVertical {
			get {
				if(stringFormatVertical == null) {
					stringFormatVertical = (StringFormat)TextOptions.DefaultStringFormat.Clone();
					stringFormatVertical.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical;
				}
				return stringFormatVertical;
			}
		}
		static StringFormat stringFormatHorizontal;
		public static StringFormat StringFormatHorizontal {
			get {
				if(stringFormatHorizontal == null) {
					stringFormatHorizontal = (StringFormat)TextOptions.DefaultStringFormat.Clone();
					stringFormatHorizontal.FormatFlags = StringFormatFlags.NoWrap;
				}
				return stringFormatHorizontal;
			}
		}
		protected override TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new NavigationBarLayoutCalculator(viewInfo);
		}
		protected override void OnHoverInfoChanged(TileControlHitInfo oldInfo, TileControlHitInfo newInfo) {
			if(oldInfo != null && oldInfo.ItemInfo != null)
				UpdateAndRepaintItem(oldInfo.ItemInfo);
			if(newInfo != null && newInfo.ItemInfo != null)
				UpdateAndRepaintItem(newInfo.ItemInfo);
		}
		void UpdateAndRepaintItem(TileItemViewInfo itemInfo) {
			itemInfo.ForceUpdateAppearanceColors();
			Owner.Invalidate(itemInfo.Bounds);
		}
		protected override void PressItem(TileControlHitInfo newInfo) {
			canStartPressAnimation = true;
			if(newInfo.ItemInfo.Item.Enabled)
				newInfo.ItemInfo.Item.OnItemPress();
			newInfo.ItemInfo.ForceUpdateAppearanceColors();
			RemoveAnimation(newInfo.ItemInfo);
			if(canStartPressAnimation && OwnerCore.AnimateItemPress)					  
				AddPressItemAnimation(newInfo);
		}
		protected override void UnPressItem(TileControlHitInfo oldInfo) {
			canStartPressAnimation = false;
			oldInfo.ItemInfo.ForceUpdateAppearanceColors();
			Owner.Invalidate(oldInfo.ItemInfo.Bounds);
			RemoveAnimation(oldInfo.ItemInfo);
			if(!OwnerCore.AnimateItemPress) return;
			AddUnPressItemAnimation(oldInfo);
		}
		public override HorzAlignment HorizontalContentAlignment {
			get {
				if(Owner.Properties.HorizontalContentAlignment == HorzAlignment.Default)
					return HorzAlignment.Near;
				return Owner.Properties.HorizontalContentAlignment;
			}
		}
		protected override void UpdateNavigationGrid() {
			base.UpdateNavigationGrid();
			if(OwnerCore.AutoSize)
				UpdateSize();
		}
		public Size MaxSize { get; private set; }
		protected virtual void UpdateMaxSize(Size size) {
			size.Height += Owner.Properties.Padding.Vertical;
			size.Width += Owner.Properties.Padding.Horizontal;
			if(OwnerCore.Orientation == Orientation.Horizontal) {
				if(MaxSize.Height < size.Height)
					MaxSize = new Size(size.Width, size.Height);
			}
			else {
				if(MaxSize.Width < size.Width)
					MaxSize = new Size(size.Width, size.Height);
			}
		}
		protected virtual void UpdateSize() {
			if(AllGroupsAreEmpty) return;
			if(OwnerCore.Orientation == Orientation.Horizontal)
				OwnerCore.Height = MaxSize.Height;
			else
				OwnerCore.Width = MaxSize.Width;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			MaxSize = Size.Empty;
			HiddenItems.Clear();
			EnsureDefaultButton();
			base.CalcViewInfo(bounds);
		}
		void EnsureDefaultButton() {
			var customizeButton = OwnerCore.ButtonsPanel.Buttons[0].Properties;
			customizeButton.Visible = OwnerCore.ShowCustomizationButton;
			if(OwnerCore.CustomButtons.Count <= 0) return;
			OwnerCore.ButtonsPanel.Buttons.Merge(OwnerCore.CustomButtons);
		}
		protected override void UpdateGroupsByScroll() {
			base.UpdateGroupsByScroll();
			CalcButtonsPanel();
		}
		void CalcButtonsPanel() {
			UpdateButtonsPanelViewInfo();
			if(HiddenItems.Count > 0) {
				CalcPanelLocationByHiddenItems();
				return;
			}
			ButtonsPanelSize = GetButtonsPanelSize();
			ButtonsPanelLocation = GetButtonsPanelLocation(Rectangle.Empty, ButtonsPanelSize);
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!ContentBounds.Contains(GetInflatedItemBounds(itemInfo.Bounds))) {
						HiddenItems.Add(itemInfo.Item);
					}
					else {
						ButtonsPanelLocation = GetButtonsPanelLocation(itemInfo.Bounds, ButtonsPanelSize);
					}
				}
			}
			if(HiddenItems.Count == 0) {
				UpdateButtonsPanelViewInfo();
				return;
			}
			RecalcViewInfo();
		}
		void CalcPanelLocationByHiddenItems() {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					ButtonsPanelLocation = GetButtonsPanelLocation(itemInfo.Bounds, ButtonsPanelSize);
				}
			}
			UpdateButtonsPanelViewInfo();
		}
		void AddItemToHiddenItem(TileItemViewInfo itemInfo) {
			HiddenItems.Add(itemInfo.Item);
			itemInfo.ForceUpdateAppearanceColors();
		}
		Size GetButtonsPanelSize() {
			OwnerCore.ButtonsPanel.ViewInfo.SetDirty();
			return OwnerCore.ButtonsPanel.ViewInfo.CalcMinSize(GInfo.Graphics);
		}
		void UpdateButtonsPanelViewInfo() {
			OwnerCore.ButtonsPanel.ViewInfo.SetDirty();
			OwnerCore.ButtonsPanel.ViewInfo.Calc(GInfo.Graphics, ButtonsPanelBounds);
		}
		Point GetButtonsPanelLocation(Rectangle itemBounds, Size panelSize) {
			int x, y;
			x = y = 0;
			if(OwnerCore.ShowButtonsBeforeItems) itemBounds = Rectangle.Empty;
			if(Owner.Properties.Orientation == Orientation.Horizontal) {
				x = IsRightToLeft ? itemBounds.Left - panelSize.Width : itemBounds.Right;
				y += ContentBounds.Top;
				y += ContentBounds.Height < panelSize.Height ? (ContentBounds.Height / 2) - (panelSize.Height / 2) : 0;
			}
			else {
				x += ContentBounds.Left;
				y = itemBounds.Bottom;
			}
			return new Point(x, y);
		}
		void RecalcViewInfo() {
			CreateLayout();
			ClearGroups();
			CreateGroups();
			LayoutGroups();
			UpdateGroupsLayout();
			UpdateGroupsTextWidth();
			UpdateScrollParams();
			UpdateGroupsByScroll();
		}
		Rectangle GetInflatedItemBounds(Rectangle bounds) {
			Rectangle res = bounds;
			if(OwnerCore.ShowButtonsBeforeItems) return res;
			if(Owner.Properties.Orientation == Orientation.Horizontal) {
				if(Owner.Properties.HorizontalContentAlignment != HorzAlignment.Far)
					if(IsRightToLeft) {
						res.X -= ButtonsPanelSize.Width;
						res.Width += ButtonsPanelSize.Width;
					}
					else {
						res.Width += ButtonsPanelSize.Width;
					}
			}
			else {
				res.Height += ButtonsPanelSize.Height;
			}
			return res;
		}
		Size ButtonsPanelSize { get; set; }
		Point ButtonsPanelLocation { get; set; }
		public Rectangle ButtonsPanelBounds { get { return new Rectangle(ButtonsPanelLocation, ButtonsPanelSize); } }
		protected internal List<TileItem> HiddenItems = new List<TileItem>();
		protected override Rectangle CalcContentBounds(Rectangle rect) {
			Rectangle res = base.CalcContentBounds(rect);
			var element = GetOfficeNavigationBarSkinElement();
			if(element != null && !OwnerCore.AllowItemSkinning) {
				var edges = element.ContentMargins;
				if(OwnerCore.Orientation == Orientation.Horizontal) {
					if(IsRightToLeft) {
						res.Width -= edges.Left;
					}
					else {
						res.X += edges.Left;
						res.Width -= edges.Width;
					}
					res.Y += edges.Top;
					res.Height -= edges.Height;
				}
				else {
					res.X += edges.Top;
					res.Y += edges.Left;
					res.Height -= edges.Width;
					res.Width -= edges.Height;
				}
			}
			if(!OwnerCore.ShowCustomizationButton)
				return res;
			Size panelSize = GetButtonsPanelSize();
			if(OwnerCore.Orientation == Orientation.Horizontal) {
				switch(Owner.Properties.HorizontalContentAlignment) {
					case HorzAlignment.Center:
					case HorzAlignment.Default:
					case HorzAlignment.Near: {
						if(OwnerCore.ShowButtonsBeforeItems) {
							res.Width -= panelSize.Width;
							res.X += panelSize.Width;
						}
						break;
					}
					case HorzAlignment.Far: {
						if(!OwnerCore.ShowButtonsBeforeItems) {
							res.Width -= panelSize.Width;
						}
						break;
					}
				}
			}
			else {
				res.Height -= panelSize.Height;
				res.Y += panelSize.Height;
			}
			return res;
		}
		TileControlHitInfo peekHoverInfo = TileControlHitInfo.Empty;
		public TileControlHitInfo PeekHoverInfo {
			get { return peekHoverInfo; }
			set {
				if(value == null)
					value = TileControlHitInfo.Empty;
				if(PeekHoverInfo.Equals(value))
					return;
				TileControlHitInfo oldInfo = PeekHoverInfo;
				peekHoverInfo = value;
				OnPeekHoverInfoChanged(oldInfo, PeekHoverInfo);
			}
		}
		protected override Bitmap RenderItemToBitmap(TileItemViewInfo itemInfo) {
			Bitmap bmp = new Bitmap(itemInfo.Bounds.Width, itemInfo.Bounds.Height, PixelFormat.Format64bppArgb);
			bool oldValue = itemInfo.UseRenderImage;
			itemInfo.UseRenderImage = false;
			try {
				itemInfo.RenderToBitmap = true;
				using(Graphics graphics = Graphics.FromImage(bmp)) {
					using(GraphicsCache cache = new GraphicsCache(graphics)) {
						RenderItemToBitmapCore(itemInfo, cache);
					}
				}
			}
			finally {
				itemInfo.RenderToBitmap = false;
				itemInfo.UseRenderImage = oldValue;
				itemInfo.AllowItemCheck = true;
			}
			return bmp;
		}
		protected virtual void OnPeekHoverInfoChanged(TileControlHitInfo oldInfo, TileControlHitInfo newInfo) {
			if(oldInfo != null && oldInfo.InItem && oldInfo.ItemInfo is NavigationBarItemViewInfo)
				((NavigationBarItemViewInfo)oldInfo.ItemInfo).StopHoverPeekTimer();
			if(newInfo != null && newInfo.InItem && newInfo.ItemInfo is NavigationBarItemViewInfo)
				((NavigationBarItemViewInfo)newInfo.ItemInfo).StartHoverPeekTimer();
		}
		internal Color GetDefaultButtonHotColor() {
			if(!Owner.AppearanceItem.Hovered.ForeColor.IsEmpty) return Owner.AppearanceItem.Hovered.ForeColor;
			return GetDefaultHotForeColor();
		}
		internal Color GetDefaultButtonPressedColor() {
			if(!Owner.AppearanceItem.Pressed.ForeColor.IsEmpty) return Owner.AppearanceItem.Pressed.ForeColor;
			return GetDefaultPressedForeColor();
		}
		internal Color GetDefaultButtonForeColor() {
			if(!Owner.AppearanceItem.Normal.ForeColor.IsEmpty) return Owner.AppearanceItem.Normal.ForeColor;
			return GetDefaultForeColor();
		}
		Skin GetSkin() {
			return NavPaneSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel);
		}
		internal SkinElement GetOfficeNavigationBarItemSkinElement() {
			if(OwnerCore.AllowItemSkinning)
				return GetSkin()[NavPaneSkins.SkinOfficeNavigationBarSkinningItem];
			return GetSkin()[NavPaneSkins.SkinOfficeNavigationBarItem];
		}
		SkinElement GetOfficeNavigationBarSkinElement() {
			return GetSkin()[NavPaneSkins.SkinOfficeNavigationBar];
		}
		public Color GetDefaultPressedForeColor() {
			SkinElement element = GetOfficeNavigationBarItemSkinElement();
			if(element == null) {
				return GetSkin().CommonSkin.Colors["ControlText"];
			}
			return element.Properties.GetColor("PressedColor");
		}
		public Color GetDefaultHotForeColor() {
			SkinElement element = GetOfficeNavigationBarItemSkinElement();
			if(element == null) {
				return GetSkin().CommonSkin.Colors["ControlText"];
			}
			return element.Properties.GetColor("HotColor");
		}
		public Color GetDefaultForeColor() {
			SkinElement element = GetOfficeNavigationBarItemSkinElement();
			if(element != null) {
				return element.Color.GetForeColor();
			}
			return GetSkin().CommonSkin.Colors["DisabledText"];
		}
		public Font GetDefaultFont() {
			SkinElement element = GetOfficeNavigationBarItemSkinElement();
			bool shouldPatch = false;
			if(element == null) {
				element = GetSkin()[NavPaneSkins.SkinGroupButton];
				shouldPatch = !OwnerCore.AllowItemSkinning;
			}
			Font defaultFont = OwnerCore.AllowItemSkinning ? XtraBars.Docking2010.Views.WindowsUI.SegoeUIFontsCache.GetSegoeUIFont() : XtraBars.Docking2010.Views.WindowsUI.SegoeUIFontsCache.GetSegoeUILightFont();
			if(shouldPatch) {
				defaultFont = new Font(defaultFont.FontFamily.Name, defaultFont.Size + defaultFontDelta);
			}
			return element != null ? element.GetFont(defaultFont) : defaultFont;
		}
		const int defaultFontDelta = 11;
		protected internal void UpdateVisualEffects() {
			base.UpdateVisualEffects(DevExpress.Utils.VisualEffects.UpdateAction.Update);
		}
	}
	class NavigationBarGroupViewInfo : TileGroupViewInfo {
		public NavigationBarGroupViewInfo(TileGroup group) : base(group) { }
	}
	class NavigationBarItemViewInfo : TileItemViewInfo {
		public NavigationBarItemViewInfo(TileItem item) : base(item) { }
		NavigationBarItemCore ItemCore { get { return Item as NavigationBarItemCore; } }
		OfficeNavigationBarViewInfo ControlInfoCore { get { return ControlInfo as OfficeNavigationBarViewInfo; } }
		public SkinElementInfo GetSkinInfo() {
			var skinProveder =  GroupInfo.ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel;
			SkinElement elem = NavPaneSkins.GetSkin(skinProveder)[NavPaneSkins.SkinOfficeNavigationBarSkinningItem];
			bool useNormalStateForSelectedItem = false;
			if(elem == null || elem.Image == null) {
				elem = NavPaneSkins.GetSkin(skinProveder)[GetItemElementName()];
				useNormalStateForSelectedItem = true;
			}
			if(elem == null) elem = CommonSkins.GetSkin(DefaultSkinProvider.Default)[CommonSkins.SkinButton];
			SkinElementInfo info = new SkinElementInfo(elem, Bounds);
			info.ImageIndex = -1;
			info.State = GetSkinElementState(useNormalStateForSelectedItem);
			return info;
		}
		protected override Padding GetItemPaddingCore() {
			if(Item.Control.Properties.ItemPadding != TileControl.DefaultItemPadding)
				return Item.Control.Properties.ItemPadding;
			SkinElement officeNavigationBarItem = ControlInfoCore.GetOfficeNavigationBarItemSkinElement();
			if(officeNavigationBarItem != null) {
				var edges = officeNavigationBarItem.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return TileControl.DefaultItemPadding;
		}
		string GetItemElementName() {
			return IsSelected ? NavPaneSkins.SkinItemSelected : NavPaneSkins.SkinItem;
		}
		ObjectState GetSkinElementState(bool useNormalStateForSelectedItem) {
			if(IsSelected)
				return useNormalStateForSelectedItem ?  ObjectState.Normal : ObjectState.Selected;
			if(IsPressed)
				return ObjectState.Pressed;
			if(IsHovered)
				return ObjectState.Hot;
			return Item.Enabled ? ObjectState.Normal : ObjectState.Disabled;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			if(IsHovered)
				return new AppearanceDefault(ControlInfoCore.GetDefaultHotForeColor(), Color.Transparent, Color.Transparent, Color.Transparent, ControlInfoCore.GetDefaultFont());
			else if(IsSelected)
				return new AppearanceDefault(ControlInfoCore.GetDefaultPressedForeColor(), Color.Transparent, Color.Transparent, Color.Transparent, ControlInfoCore.GetDefaultFont());
			return new AppearanceDefault(ControlInfoCore.GetDefaultForeColor(), Color.Transparent, Color.Transparent, Color.Transparent, ControlInfoCore.GetDefaultFont());
		}
		protected override bool ShouldUseTransition { get { return false; } }
		protected override TileItemElementViewInfo CreateElementInfo(TileItemViewInfo itemInfo, TileItemElement elem) {
			return new NavigationBarElementViewInfo(itemInfo, elem);
		}
		public override bool AllowHtmlText {
			get {
				if(GroupInfo.ControlInfo.Owner.Properties.Orientation == Orientation.Vertical) return false;
				return base.AllowHtmlText;
			}
		}
		public Size GetTextSize(Graphics g) {
			if(this.AllowHtmlText)
				return StringPainter.Default.Calculate(g, this.PaintAppearance, this.Item.Text, 0).Bounds.Size;
			return g.MeasureString(this.Item.Text, this.PaintAppearance.GetFont(), 0, ItemStringFormat).ToSize();
		}
		public StringFormat ItemStringFormat {
			get {
				if(ControlInfo.Owner.Properties.Orientation == Orientation.Horizontal)
					return OfficeNavigationBarViewInfo.StringFormatHorizontal;
				return OfficeNavigationBarViewInfo.StringFormatVertical;
			}
		}
		internal void StopHoverPeekTimer() {
			ItemCore.HoverPeekTimer.Stop();
		}
		internal void StartHoverPeekTimer() {
			ItemCore.HoverPeekTimer.Interval = GetBeakFormShowInterval();
			ItemCore.HoverPeekTimer.Start();
		}
		int GetBeakFormShowInterval() {
			return ((NavigationBarCore)ControlInfo.Owner).PeekFormShowDelay;
		}
		protected override Rectangle GetVisualEffectBounds() {
			if(Elements.Count == 0) return Rectangle.Empty;
			return Elements[0].TextBounds;
		}
	}
	class NavigationBarElementViewInfo : TileItemElementViewInfo {
		public NavigationBarElementViewInfo(TileItemViewInfo itemInfo, TileItemElement element) : base(itemInfo, element) { }
		protected override void UpdateContentElementPaintAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { GetAppearance(), ItemInfo.GetAppearance(), ItemInfo.GroupInfo.ControlInfo.GetAppearance(ItemInfo) }, ItemInfo.DefaultAppearance);
			PaintAppearance = res;
		}
		public override TileItemContentAlignment GetAlignment(int index, TileItemContentAlignment alignemnt) {
			if(alignemnt != TileItemContentAlignment.Default)
				return alignemnt;
			return TileItemContentAlignment.MiddleCenter;
		}
		protected override TileItemContentAlignment GetImageAlignment() {
			TileItemContentAlignment res = Element.ImageAlignment != TileItemContentAlignment.Default ? Element.ImageAlignment : Item.ImageAlignment;
			if(res == TileItemContentAlignment.Default) res = ItemInfo.ControlInfo.Owner.Properties.ItemImageAlignment;
			return res == TileItemContentAlignment.Default ? TileItemContentAlignment.MiddleCenter : res;
		}
		protected override Size CalcSimpleTextSize(Size maxTextSize) {
			SizeF size = ItemInfo.GroupInfo.ControlInfo.GInfo.Graphics.MeasureString(Text, PaintAppearance.Font, maxTextSize.Width, StringFormat);
			return new Size((int)size.Width + 1, (int)size.Height + 1);
		}
		public StringFormat StringFormat {
			get { return ((NavigationBarItemViewInfo)ItemInfo).ItemStringFormat; }
		}
	}
	class NavigationBarLayoutCalculator : TileControlLayoutCalculator {
		public NavigationBarLayoutCalculator(TileControlViewInfo viewInfo) : base(viewInfo) { }
		protected override TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new NavigationBarLayoutCalculator(viewInfo);
		}
		protected override TileControlLayoutItem GetNewLayoutItemCore(Rectangle bounds, TileItem item, TileItemViewInfo itemInfo, TileItemPosition position) {
			return new NavigationBarLayoutItem() { Bounds = bounds, Item = item, ItemInfo = itemInfo, Position = position };
		}
		protected override void MovePositionHorizontal(TileGroupLayoutInfo info, TileControlLayoutItem item, TileControlLayoutItem nextItem) {
			int x = info.Location.X;
			x = ViewInfo.IsRightToLeft ? x - item.Bounds.Width : item.Bounds.Right;
			if(nextItem != null) {
				x = ViewInfo.IsRightToLeft ? x - ViewInfo.IndentBetweenItems : x + ViewInfo.IndentBetweenItems;
			}
			info.Location = new Point(x, info.Location.Y);
		}
		protected override bool CanCreateItem(TileItem item, TileItemViewInfo dragItem) {
			OfficeNavigationBarViewInfo viewInfo = ViewInfo as OfficeNavigationBarViewInfo;
			if(!viewInfo.CheckItemIsVisible(item)) return false;
			return base.CanCreateItem(item, dragItem);
		}
		public override TileItemViewInfo GetItemAfterDragItem() {
			OfficeNavigationBarViewInfo viewInfo = ViewInfo as OfficeNavigationBarViewInfo;
			TileItemViewInfo itemInfo = base.GetItemAfterDragItem();
			if(itemInfo != null) 
				return itemInfo;
			if(viewInfo != null && viewInfo.HiddenItems.Count > 0)
				return viewInfo.HiddenItems[0].ItemInfo;
			return null;
		}
	}
	class NavigationBarLayoutItem : TileControlLayoutItem { }
	class NavigationBarButtonInfo : BaseButtonInfo {
		public NavigationBarButtonInfo(IBaseButton button) : base(button) { }
		protected override bool GetAllowGlyphSkinning() {
			if(Button is NavigationBarCustomizationButton) return true;
			return base.GetAllowGlyphSkinning();
		}
	}
	class NavigationBarButtonsPanelViewInfo : BaseButtonsPanelViewInfo {
		public NavigationBarButtonsPanelViewInfo(IButtonsPanel owner) : base(owner) { }
		NavigationBarButtonsPanel NavigationBarButtonsPanel { get { return Panel as NavigationBarButtonsPanel; } }
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			return new NavigationBarButtonInfo(button);
		}
		public override Size CalcMinSize(Graphics g) {
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			if(IsReady && !Content.Size.IsEmpty)
				return painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, Content.Size)).Size;
			Size result = new Size(0, 0);
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			int visibleButtons = 0;
			Size customButtonSize = Size.Empty;
			if(NavigationBarButtonsPanel != null)
				customButtonSize = NavigationBarButtonsPanel.ButtonSize;
			foreach(IBaseButton button in Panel.Buttons) {
				BaseButtonInfo buttonInfo = CreateButtonInfo(button);
				Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
				visibleButtons += buttonSize.Width != 0 ? 1 : 0;
				result.Width = Panel.IsHorizontal ? result.Width + Math.Max(buttonSize.Width, customButtonSize.Width) : Math.Max(customButtonSize.Width, Math.Max(buttonSize.Width, result.Width));
				result.Height = Panel.IsHorizontal ? Math.Max(result.Height, Math.Max(buttonSize.Height, customButtonSize.Height)) : result.Height + Math.Max(customButtonSize.Height, buttonSize.Height);
			}
			if(Panel.IsHorizontal)
				result.Width += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			else
				result.Height += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			MinSize = painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, result)).Size;
			return MinSize;
		}
	}
}
