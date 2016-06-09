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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Collections;
namespace DevExpress.Web.Internal {
	public abstract class MenuLiteControlBase : SharedCacheControl {
		MenuItem parentItem;
		public MenuLiteControlBase(MenuItem parentItem)
			: base() {
			this.parentItem = parentItem;
		}
		public MenuLiteControlBase(MenuLiteControlBase owner)
			: base(owner) {
			this.parentItem = owner.ParentItem;
		}
		protected ASPxMenuBase Menu { get { return ParentItem.Menu; } }
		public RenderHelper RenderHelper { get { return Menu.RenderHelper; } }
		protected MenuItem ParentItem { get { return parentItem; } }
		public bool IsRightToLeft { get { return (Menu as ISkinOwner).IsRightToLeft(); } }
		protected bool HasPopOutImages {
			get {
				return GetCachedValue<bool>("HasPopOutImages", delegate {
					foreach(MenuItem item in ParentItem.Items.GetVisibleItems())
						if(Menu.HasPopOutImage(item))
							return true;
					return false;
				});
			}
		}
		protected Orientation MenuOrientation {
			get {
				return GetCachedValue<Orientation>("MenuOrientation", delegate {
					return Menu.GetOrientation(ParentItem);
				});
			}
		}
		public static MenuItemTemplateContainer CreateTemplateContainer(Control control, MenuItem item, string ID, ITemplate template) {
			MenuItemTemplateContainer container = new MenuItemTemplateContainer(item);
			template.InstantiateIn(container);
			container.AddToHierarchy(control, ID);
			return container;
		}
	}
	public class MenuLite : MenuLiteControlBase {
		bool isCreateContent = true;
		bool isSetImageCssClass = true;
		MenuLiteContent contentControl;
		public MenuLite(MenuItem parentItem)
			: base(parentItem) {
		}
		public bool IsCreateContent { get { return isCreateContent; } set { isCreateContent = value; } }
		public bool IsSetImageCssClass { get { return isSetImageCssClass; } set { isSetImageCssClass = value; } }
		protected MenuLiteContent ContentControl { get { return contentControl; } set { contentControl = value; } }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected MenuStyle MenuStyle {
			get {
				return GetCachedValue<MenuStyle>("MenuStyle", delegate {
					return Menu.GetMenuStyle(ParentItem);
				});
			}
		}
		protected Paddings MenuPaddings {
			get {
				return GetCachedValue<Paddings>("MenuPaddings", delegate {
					return Menu.GetMenuPaddings(ParentItem);
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ContentControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(IsCreateContent) {
				ContentControl = CreateMenuLiteContent();
				AddChild(ContentControl);
				Menu.RegisterSubMenuContentControl(ParentItem, ContentControl);
			}
		}
		protected virtual MenuLiteContent CreateMenuLiteContent() {
			return new MenuLiteContent(this);
		}
		protected virtual void AddChild(MenuLiteContent child) {
			Controls.Add(child);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MenuStyle.AssignToControl(this, AttributesRange.All, false, false, true);
			RenderUtils.SetPaddings(this, MenuPaddings);
			if(Menu.IsShowAsToolbar())
				RenderUtils.AppendDefaultDXClassName(this, Menu.GetToolbarModeCssClassName());
			if(Menu.IsAdaptivityMenu(ParentItem))
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.AdaptiveMenuCssClass);
		}
	}
	public class MenuLiteDesignMode : MenuLite {
		public MenuLiteDesignMode(MenuItem parentItem)
			: base(parentItem) {
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Table; }
		}
		protected override void AddChild(MenuLiteContent child) {
			WebControl row = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			Controls.Add(row);
			WebControl cell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			row.Controls.Add(cell);
			cell.Controls.Add(child);
		}
		protected override MenuLiteContent CreateMenuLiteContent() {
			return new MenuLiteContentDesignMode(this);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.PrepareTableElement(this);
		}
	}
	public class MenuLiteContent : MenuLiteControlBase {
		public MenuLiteContent(MenuLite owner)
			: base(owner) {
		}
		protected new MenuLite Owner { get { return (MenuLite)base.Owner; } }
		protected WebControl ContentControl { get; private set; }
		public WebControl ScrollAreaDiv { get; private set; }
		public DivButtonControl ScrollUpButton { get; private set; }
		public DivButtonControl ScrollDownButton { get; private set; }
		protected internal bool HasGutter {
			get {
				return GetCachedValue<bool>("HasGutter", delegate {
					return Menu.GetOrientation(ParentItem) == Orientation.Vertical && (GutterWidth != 0 || HasImages);
				});
			}
		}
		protected internal AppearanceStyleBase GutterStyle {
			get {
				return GetCachedValue<AppearanceStyleBase>("GutterStyle", delegate {
					return Menu.GetMenuGutterStyle(ParentItem);
				});
			}
		}
		protected internal Unit GutterWidth {
			get {
				return GetCachedValue<Unit>("GutterWidth", delegate {
					return Menu.GetGutterWidth(ParentItem);
				});
			}
		}
		protected ITemplate MenuTemplate {
			get {
				return GetCachedValue<ITemplate>("MenuTemplate", delegate {
					return Menu.GetMenuTemplate(ParentItem);
				});
			}
		}
		protected string MenuTemplateContainerID {
			get {
				return GetCachedValue<string>("MenuTemplateContainerID", delegate {
					return Menu.GetMenuTemplateContainerID(ParentItem);
				});
			}
		}
		protected bool HasImages {
			get {
				return GetCachedValue<bool>("HasImages", delegate {
					foreach(MenuItem child in ParentItem.Items.GetVisibleItems())
						if(!Menu.GetItemImageProperties(child).IsEmpty)
							return true;
					return false;
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ContentControl = null;
			ScrollAreaDiv = null;
			ScrollUpButton = null;
			ScrollDownButton = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(Menu.CanScrollSubMenu(ParentItem))
				ScrollUpButton = RenderHelper.CreateScrollUpButton(this, ParentItem);
			WebControl itemsParent = this;
			if(Menu.CanScrollSubMenu(ParentItem)) {
				ScrollAreaDiv = RenderHelper.CreateScrollArea(this, ParentItem);
				itemsParent = ScrollAreaDiv;
			}
			CreateControlInnerHierarchy(itemsParent);
			if(Menu.CanScrollSubMenu(ParentItem))
				ScrollDownButton = RenderHelper.CreateScrollDownButton(this, ParentItem);
		}
		protected void CreateControlInnerHierarchy(WebControl parent) {
			if(MenuTemplate != null) {
				ContentControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				CreateTemplateContainer(ContentControl, ParentItem, MenuTemplateContainerID, MenuTemplate);
			}
			else {
				ContentControl = CreateContentControl();
				int visibleItemsCount = ParentItem.Items.GetVisibleItemCount();
				if(visibleItemsCount > 0) {
					AddChildToContentControl(new MenuItemLite(ParentItem.Items.GetVisibleItem(0)));
					for(int i = 1; i < visibleItemsCount; i++) {
						MenuItem item = ParentItem.Items.GetVisibleItem(i);
						AddChildToContentControl(new MenuItemSpacingLite(this, Menu.ShowItemSeparator(ParentItem, item), Menu.IsAdaptivityMenu(item)));
						AddChildToContentControl(new MenuItemLite(item));
					}
				}
			}
			parent.Controls.Add(ContentControl);
		}
		protected virtual WebControl CreateContentControl() {
			return RenderUtils.CreateWebControl(HtmlTextWriterTag.Ul);
		}
		protected virtual void AddChildToContentControl(WebControl child) {
			ContentControl.Controls.Add(child);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(ContentControl, MenuStyles.DXCssClass);
			if(Menu.ItemLinkMode == ItemLinkMode.TextOnly)
				RenderUtils.AppendDefaultDXClassName(ContentControl, MenuStyles.TextOnlyMenuCssClass);
			else if(Menu.ItemLinkMode == ItemLinkMode.TextAndImage)
				RenderUtils.AppendDefaultDXClassName(ContentControl, MenuStyles.TextAndImageMenuCssClass);
			if(ParentItem.IsRootItem && !Menu.IsAutoWidthMode(ParentItem)) {
				HorizontalAlign horizontalAlign = Menu.GetControlStyle().HorizontalAlign;
				if(horizontalAlign == HorizontalAlign.Right)
					RenderUtils.SetStyleStringAttribute(ContentControl, "float", "right");
				else if(horizontalAlign == HorizontalAlign.Center)
					RenderUtils.SetStyleStringAttribute(ContentControl, "margin", "0px auto");
			}
			if(MenuTemplate == null) {
				if(Owner.IsSetImageCssClass)
					RenderUtils.AppendDefaultDXClassName(ContentControl, Menu.RenderStyles.GetImageCssClass(Menu.GetItemImagePosition(ParentItem)));
				if(!HasImages && !Menu.IsAdaptivityMenu(ParentItem))
					RenderUtils.AppendDefaultDXClassName(ContentControl, MenuStyles.WithoutImagesCssClass);
				if(HasGutter) {
					GutterStyle.AssignToControl(ContentControl);
					if(!GutterWidth.IsEmpty)
						ContentControl.Style.Add("background-size", GutterWidth.ToString() + " 1px");
				}
				if(!DesignMode) {
					foreach(Control control in ContentControl.Controls) {
						MenuItemSpacingLite spacingControl = control as MenuItemSpacingLite;
						if(spacingControl != null)
							if(spacingControl.IsHideSpacing && spacingControl.IsHideSeparator)
								spacingControl.Visible = false;
					}
				}
			}
			else {
				if(IsRightToLeft)
					ContentControl.Attributes["dir"] = "rtl";
			}
			if(ScrollAreaDiv != null)
				RenderHelper.PrepareScrollArea(ScrollAreaDiv, ParentItem);
			if(ScrollUpButton != null)
				RenderHelper.PrepareScrollUpButton(ScrollUpButton, ParentItem);
			if(ScrollDownButton != null)
				RenderHelper.PrepareScrollDownButton(ScrollDownButton, ParentItem);
			if(Menu.GetOrientation(ParentItem) == Orientation.Vertical && ParentItem.Items.GetVisibleItemCount() > 0) {
				Unit width = Menu.GetItemWidth(ParentItem, ParentItem.Items.GetVisibleItem(0));
				if(!width.IsEmpty)
					ContentControl.Width = width;
			}
		}
	}
	public class MenuLiteContentDesignMode : MenuLiteContent {
		public MenuLiteContentDesignMode(MenuLite owner)
			: base(owner) {
		}
		protected override WebControl CreateContentControl() {
			return RenderUtils.CreateWebControl(HtmlTextWriterTag.Table);
		}
		protected override void AddChildToContentControl(WebControl child) {
			GetTableRow().Controls.Add(child);
		}
		protected WebControl GetTableRow() {
			if(MenuOrientation == Orientation.Horizontal && ContentControl.Controls.Count > 0) {
				if(ContentControl.Controls.Count == 0)
					return CreateTableRow();
				return ContentControl.Controls[0] as WebControl;
			}
			return CreateTableRow();
		}
		protected WebControl CreateTableRow() {
			WebControl row = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			ContentControl.Controls.Add(row);
			return row;
		}
	}
	public class MainMenuLite : MenuLiteControlBase {
		protected MenuLite MenuControl { get; private set; }
		public MainMenuLite(MenuItem parentItem)
			: base(parentItem) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MenuControl = null;
		}
		protected override void CreateControlHierarchy() {
			MenuControl = CreateMenuControl();
			Controls.Add(MenuControl);
		}
		protected virtual MenuLite CreateMenuControl() {
			return DesignMode ? new MenuLiteDesignMode(ParentItem) : new MenuLite(ParentItem);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(Menu, MenuControl);
			RenderUtils.SetVisibility(MenuControl, Menu.IsClientVisible(), true);
		}
	}
	public class PopupMenuLite : MenuLiteControlBase {
		MenuLite contentControl;
		WebControl borderCorrectorControl;
		public PopupMenuLite(MenuItem parentItem)
			: base(parentItem) {
		}
		protected MenuLite ContentControl { get { return contentControl; } set { contentControl = value; } }
		protected WebControl BorderCorrectorControl { get { return borderCorrectorControl; } set { borderCorrectorControl = value; } }
		protected bool IsLoadedOnCallback { get { return !IsMainMenu && Menu.IsCallBacksEnabled(); } }
		protected bool IsCreateContent { get { return !IsLoadedOnCallback || Menu.IsCallback; } }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected bool IsMainMenu {
			get {
				return GetCachedValue<bool>("IsMainMenu", delegate {
					return Menu.IsMainMenu(ParentItem);
				});
			}
		}
		protected bool IsPopupMenuVisible {
			get {
				return GetCachedValue<bool>("IsPopupMenuVisible", delegate {
					return Menu.IsPopupMenuControlVisible(this);
				});
			}
		}
		protected string ZIndex {
			get {
				return GetCachedValue<string>("ZIndex", delegate {
					return Menu.GetMenuZIndex(ParentItem).ToString();
				});
			}
		}
		protected bool HasBorderCorrector {
			get {
				return GetCachedValue<bool>("HasBorderCorrector", delegate {
					return Menu.BorderBetweenItemAndSubMenu == BorderBetweenItemAndSubMenuMode.HideAll || Menu.BorderBetweenItemAndSubMenu == BorderBetweenItemAndSubMenuMode.HideRootOnly && ParentItem.Depth == 0;
				});
			}
		}
		protected string BorderCorrectorZIndex {
			get {
				return GetCachedValue<string>("BorderCorrectorZIndex", delegate {
					return Menu.GetMenuBorderCorrectorZIndex(ParentItem).ToString();
				});
			}
		}
		protected AppearanceStyleBase BorderCorrectorStyle {
			get {
				return GetCachedValue<AppearanceStyleBase>("BorderCorrectorStyle", delegate {
					return Menu.GetMenuBorderCorrectorStyle(ParentItem, false);
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ContentControl = null;
			BorderCorrectorControl = null;
		}
		protected override void CreateControlHierarchy() {
			ContentControl = CreateMenuControl();
			ContentControl.IsSetImageCssClass = false;
			ContentControl.IsCreateContent = IsCreateContent;
			Controls.Add(ContentControl);
			if(HasBorderCorrector) {
				BorderCorrectorControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(BorderCorrectorControl);
			}
		}
		protected virtual MenuLite CreateMenuControl() {
			return DesignMode ? new MenuLiteDesignMode(ParentItem) : new MenuLite(ParentItem);
		}
		protected override void PrepareControlHierarchy() {
			if(ParentItem.IsRootItem) {
				RenderUtils.AssignAttributes(Menu, this);
				Width = Unit.Empty;
				Height = Unit.Empty;
				RenderUtils.AppendDefaultDXClassName(ContentControl, MenuStyles.MainPopupMenuCssClass);
			}
			else
				ID = Menu.GetMenuElementID(ParentItem);
			if(!Menu.DesignMode)
				Style.Add("z-index", ZIndex);
			if(!IsPopupMenuVisible) {
				if(Menu.DesignMode)
					Visible = false;
				else
					Style.Add("display", "none");
			}
			if(BorderCorrectorControl != null) {
				BorderCorrectorControl.Style.Add("display", "none");
				BorderCorrectorControl.Style.Add("z-index", BorderCorrectorZIndex);
				BorderCorrectorStyle.AssignToControl(BorderCorrectorControl, AttributesRange.Common);
			}
			if(Menu.ShowSubMenuShadow)
				RenderUtils.AppendDefaultDXClassName(ContentControl, MenuStyles.PopupMenuShadowCssClass);
			RenderUtils.SetOpacity(ContentControl, Menu.Opacity);
		}
	}
	public class MenuItemSpacingLite : MenuLiteControlBase {
		bool hasSeparator;
		bool beforeAdaptiveItem;
		WebControl separatorControl;
		public MenuItemSpacingLite(MenuLiteContent owner, bool hasSeparator, bool beforeAdaptiveItem)
			: base(owner) {
			this.hasSeparator = hasSeparator;
			this.beforeAdaptiveItem = beforeAdaptiveItem;
		}
		protected bool HasSeparator { get { return hasSeparator; } }
		protected bool BeforeAdaptiveItem { get { return beforeAdaptiveItem; } }
		protected WebControl SeparatorControl { get { return separatorControl; } set { separatorControl = value; } }
		protected override HtmlTextWriterTag TagKey { get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected AppearanceStyleBase SeparatorStyle {
			get {
				return GetCachedValue<AppearanceStyleBase>("SeparatorStyle", delegate {
					return Menu.GetMenuSeparatorStyle(ParentItem);
				});
			}
		}
		protected Unit ItemSpacing {
			get {
				return GetCachedValue<Unit>("ItemSpacing", delegate {
					return Menu.GetMenuStyle(ParentItem).ItemSpacing;
				});
			}
		}
		protected internal Unit ItemTextIndent {
			get {
				return GetCachedValue<Unit>("ItemTextIndent", delegate {
					return Menu.GetTextIndent(ParentItem);
				});
			}
		}
		protected Unit SpacingWidth {
			get {
				return GetCachedValue<Unit>("SpacingWidth", delegate {
					return MenuOrientation == Orientation.Horizontal
						? ItemSpacing
						: Unit.Empty;
				});
			}
		}
		protected Unit SpacingHeight {
			get {
				return GetCachedValue<Unit>("SpacingHeight", delegate {
					return MenuOrientation == Orientation.Vertical
						? ItemSpacing
						: Unit.Empty;
				});
			}
		}
		bool IsSizeZero(Unit size) {
			return !size.IsEmpty && size.Value == 0;
		}
		protected internal bool IsHideSpacing {
			get {
				return GetCachedValue<bool>("IsHideSpacing", delegate {
					return MenuOrientation == Orientation.Horizontal && IsSizeZero(SpacingWidth)
						|| MenuOrientation == Orientation.Vertical && IsSizeZero(SpacingHeight);
				});
			}
		}
		protected internal bool IsHideSeparator {
			get {
				return !HasSeparator || BeforeAdaptiveItem
					|| MenuOrientation == Orientation.Horizontal && IsSizeZero(SeparatorStyle.Width)
					|| MenuOrientation == Orientation.Vertical && IsSizeZero(SeparatorStyle.Height);
			}
		}
		protected Paddings SeparatorPaddings {
			get {
				return GetCachedValue<Paddings>("SeparatorPaddings", delegate {
					Paddings paddings = MenuOrientation == Orientation.Vertical
						? new Paddings(Unit.Empty, ItemSpacing, Unit.Empty, ItemSpacing)
						: new Paddings(ItemSpacing, Unit.Empty, ItemSpacing, Unit.Empty);
					paddings.CopyFrom(SeparatorStyle.Paddings);
					return paddings;
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			SeparatorControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(HasSeparator) {
				SeparatorControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.B);
				Controls.Add(SeparatorControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!IsHideSeparator) {
				CssClass = SeparatorStyle.CssClass;
				SeparatorPaddings.AssignToControl(this);
				SeparatorControl.Width = SeparatorStyle.Width;
				SeparatorControl.Height = SeparatorStyle.Height;
				SeparatorControl.BackColor = SeparatorStyle.BackColor;
				SeparatorStyle.BackgroundImage.AssignToControl(SeparatorControl);
				if(!ItemTextIndent.IsEmpty)
					Style.Add(IsRightToLeft ? "padding-right" : "padding-left", ItemTextIndent.ToString());
			}
			else {
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.SpacingCssClass);
				if(BeforeAdaptiveItem)
					RenderUtils.AppendDefaultDXClassName(this, MenuStyles.AdaptiveMenuItemSpacingCssClass);
				Width = SpacingWidth;
				Height = SpacingHeight;
				if(SeparatorControl != null)
					SeparatorControl.Visible = false;
			}
		}
	}
	public abstract class MenuItemLiteBase : MenuLiteControlBase {
		MenuItem item;
		public MenuItemLiteBase(MenuItem item)
			: base(item.Parent) {
			this.item = item;
		}
		public MenuItemLiteBase(MenuItemLiteBase owner)
			: base(owner) {
			this.item = owner.Item;
		}
		protected MenuItem Item { get { return item; } }
		protected ImagePosition ImagePosition {
			get {
				return GetCachedValue<ImagePosition>("ImagePosition", delegate {
					return Menu.GetItemImagePosition(ParentItem);
				});
			}
		}
		protected MenuItemStyle ItemStyle {
			get {
				return GetCachedValue<MenuItemStyle>("ItemStyle", delegate {
					return Menu.GetItemStyle(Item);
				});
			}
		}
		protected bool IsDropDownMode {
			get {
				return GetCachedValue<bool>("IsDropDownMode", delegate {
					return Menu.IsDropDownMode(Item);
				});
			}
		}
		protected bool IsItemEnabled {
			get {
				return GetCachedValue<bool>("IsItemEnabled", delegate {
					return Menu.GetItemEnabled(Item);
				});
			}
		}
		protected string Text {
			get {
				return GetCachedValue<string>("Text", delegate {
					return Menu.HtmlEncode(Menu.GetItemText(Item));
				});
			}
		}
		protected string AdaptiveText {
			get {
				return GetCachedValue<string>("AdaptiveText", delegate {
					return Menu.HtmlEncode(Menu.GetItemAdaptiveText(Item));
				});
			}
		}
		protected bool HasItemTextTemplate {
			get {
				return GetCachedValue<bool>("HasItemTextTemplate", delegate {
					return Menu.GetItemTextTemplate(Item) != null;
				});
			}
		}
		protected ItemLinkMode ItemLinkMode {
			get {
				return GetCachedValue<ItemLinkMode>("ItemLinkMode", delegate {
					return Menu.ItemLinkMode;
				});
			}
		}
		protected bool HasNavigateUrl {
			get {
				return GetCachedValue<bool>("HasNavigateUrl", delegate {
					return !string.IsNullOrEmpty(Menu.GetItemNavigateUrl(Item));
				});
			}
		}
		protected bool HasImage {
			get {
				return GetCachedValue<bool>("HasImage", delegate {
					return !ImageProperties.IsEmpty;
				});
			}
		}
		protected ItemImagePropertiesBase ImageProperties {
			get {
				return GetCachedValue<ItemImagePropertiesBase>("ImageProperties", delegate {
					return Menu.GetItemImageProperties(Item);
				});
			}
		}
		protected bool HasPopOutControl {
			get {
				return GetCachedValue<bool>("HasPopOutControl", delegate {
					return Menu.HasPopOutImage(Item);
				});
			}
		}
	}
	public class MenuItemLite : MenuItemLiteBase {
		protected TableRow TableRow { get; private set; }
		MenuItemContentLite contentControl;
		MenuItemPopOutLite popOutControl;
		public MenuItemLite(MenuItem item)
			: base(item) {
		}
		protected MenuItemContentLite ContentControl { get { return contentControl; } set { contentControl = value; } }
		protected MenuItemPopOutLite PopOutControl { get { return popOutControl; } set { popOutControl = value; } }
		protected override HtmlTextWriterTag TagKey { get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected bool HasImageReplacement {
			get {
				return GetCachedValue<bool>("HasImageReplacement", delegate {
					return !HasImage && Menu.HasParentImageCellInternal(ParentItem);
				});
			}
		}
		protected bool HasPopOutImageReplacement {
			get {
				return GetCachedValue<bool>("HasPopOutImageReplacement", delegate {
					return !HasPopOutControl && HasPopOutImages;
				});
			}
		}
		protected bool IsPopOutControlBeforeContent {
			get {
				return GetCachedValue<bool>("IsPopOutControlBeforeContent", delegate {
					return ImagePosition == ImagePosition.Right;
				});
			}
		}
		protected bool HasItemTemplate {
			get {
				return GetCachedValue<bool>("HasItemTemplate", delegate {
					return ItemTemplate != null;
				});
			}
		}
		protected ITemplate ItemTemplate {
			get {
				return GetCachedValue<ITemplate>("ItemTemplate", delegate {
					return Menu.GetItemTemplate(Item);
				});
			}
		}
		protected string ItemTemplateContainerID {
			get {
				return GetCachedValue<string>("ItemTemplateContainerID", delegate {
					return Menu.GetItemTemplateContainerID(Item);
				});
			}
		}
		protected AppearanceStyleBase ItemTemplateStyle {
			get {
				return GetCachedValue<AppearanceStyleBase>("ItemTemplateStyle", delegate {
					return Menu.GetItemTemplateStyleInternal(Item);
				});
			}
		}
		protected Paddings ItemTemplatePaddings {
			get {
				return GetCachedValue<Paddings>("ItemTemplatePaddings", delegate {
					return Menu.GetItemTemplatePaddingsInternal(Item);
				});
			}
		}
		protected string ItemToolTip {
			get {
				return GetCachedValue<string>("ItemToolTip", delegate {
					return Menu.GetItemCellToolTip(Item);
				});
			}
		}
		protected internal Unit ItemTextIndent {
			get {
				return GetCachedValue<Unit>("ItemTextIndent", delegate {
					return Menu.GetTextIndent(ParentItem);
				});
			}
		}
		protected bool IsAddClearElement {
			get {
				return GetCachedValue<bool>("IsAddClearElement", delegate {
					return MenuOrientation == Orientation.Vertical;
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ContentControl = null;
			PopOutControl = null;
		}
		protected WebControl GetContainer() {
			if(!DesignMode)
				return this;
			if(TableRow == null)
				CreateTableLayout();
			TableCell cell = RenderUtils.CreateTableCell();
			TableRow.Cells.Add(cell);
			return cell;
		}
		protected void CreateTableLayout() {
			InternalTable table = RenderUtils.CreateTable();
			Controls.Add(table);
			TableRow = RenderUtils.CreateTableRow();
			table.Rows.Add(TableRow);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(HasItemTemplate)
				CreateTemplateContainer(GetContainer(), Item, ItemTemplateContainerID, ItemTemplate);
			else {
				if(HasPopOutControl && IsPopOutControlBeforeContent)
					CreatePopOutControl();
				CreateContentControl();
				if(HasPopOutControl && !IsPopOutControlBeforeContent)
					CreatePopOutControl();
			}
			if(IsAddClearElement)
				Controls.Add(RenderUtils.CreateClearElement());
		}
		protected void CreatePopOutControl() {
			PopOutControl = new MenuItemPopOutLite(this);
			GetContainer().Controls.Add(PopOutControl);
		}
		protected void CreateContentControl() {
			ContentControl = new MenuItemContentLite(this);
			WebControl contentControlContainer = GetContainer();
			contentControlContainer.Controls.Add(ContentControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(HasItemTemplate) {
				ItemTemplateStyle.AssignToControl(this, AttributesRange.All);
				RenderUtils.SetPaddings(this, ItemTemplatePaddings);
				if(IsRightToLeft)
					Attributes["dir"] = "rtl";
			}
			else {
				ItemStyle.AssignToControl(this, AttributesRange.Common);
				if(ContentControl == null || (!ContentControl.IsTextInHyperLink && !ContentControl.ShowAsHyperLink) || !IsItemEnabled)
					ItemStyle.AssignToControl(this, AttributesRange.Font);
			}
			if(MenuOrientation == Orientation.Horizontal)
				Width = Menu.GetItemWidth(ParentItem, Item);
			Height = ItemStyle.Height;
			ToolTip = ItemToolTip;
			if(!ItemTextIndent.IsEmpty && !HasImage)
				Style.Add(IsRightToLeft ? "padding-right" : "padding-left", ItemTextIndent.ToString());
			if(HasPopOutControl) {
				if(IsDropDownMode)
					RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ItemDropDownModeCssClass);
				else
					RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ItemWithSubMenuCssClass);
			}
			else if(HasPopOutImageReplacement)
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ItemWithoutSubMenuCssClass);
			if(HasImageReplacement)
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ItemWithoutImageCssClass);
			if(HasItemTemplate)
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ItemTemplateCssClassName);
			if(Menu.IsAdaptivityMenu(Item))
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.AdaptiveMenuItemCssClass);
		}
	}
	public class MenuItemContentLite : MenuItemLiteBase {
		InternalHyperLink hyperLinkControl;
		public MenuItemContentLite(MenuItemLite itemControl) : base(itemControl) { }
		protected MenuItemLite ItemControl { get { return (MenuItemLite)Owner; } }
		protected InternalHyperLink HyperLinkControl { get { return hyperLinkControl; } set { hyperLinkControl = value; } }
		protected bool IsImageBeforeText {
			get {
				return GetCachedValue<bool>("IsImageBeforeText", delegate {
					return ImagePosition == ImagePosition.Left || ImagePosition == ImagePosition.Top;
				});
			}
		}
		protected internal bool ShowAsHyperLink {
			get {
				return GetCachedValue<bool>("ShowAsHyperLink", delegate {
					return HasNavigateUrl && IsItemEnabled && !HasItemTextTemplate && ItemLinkMode == ItemLinkMode.ContentBounds;
				});
			}
		}
		protected internal bool IsImageInHyperLink {
			get {
				return GetCachedValue<bool>("IsImageInHyperLink", delegate {
					return HasNavigateUrl && !ShowAsHyperLink && HasImage && ItemLinkMode != ItemLinkMode.TextOnly;
				});
			}
		}
		protected internal bool IsTextInHyperLink {
			get {
				return GetCachedValue<bool>("IsTextInHyperLink", delegate {
					return HasNavigateUrl && !ShowAsHyperLink && !HasItemTextTemplate;
				});
			}
		}
		protected bool IsLargeItems {
			get {
				return GetCachedValue<bool>("IsLargeItems", delegate {
					return Menu.IsLargeItems(ParentItem);
				});
			}
		}
		protected string NavigateUrl {
			get {
				return GetCachedValue<string>("NavigateUrl", delegate {
					return Menu.GetItemNavigateUrl(Item);
				});
			}
		}
		protected string HyperLinkTarget {
			get {
				return GetCachedValue<string>("HyperLinkTarget", delegate {
					return Menu.GetItemTarget(Item);
				});
			}
		}
		protected bool IsAccessibilityCompliant {
			get {
				return GetCachedValue<bool>("IsAccessibilityCompliant", delegate {
					return Menu.IsAccessibilityCompliantRender(true);
				});
			}
		}
		protected AppearanceStyleBase HyperLinkStyle {
			get {
				return GetCachedValue<AppearanceStyleBase>("HyperLinkStyle", delegate {
					return Menu.GetItemLinkStyle(ParentItem, Item);
				});
			}
		}
		protected string LinkToolTip {
			get {
				return GetCachedValue<string>("LinkToolTip", delegate {
					return Menu.GetItemLinkToolTip(Item);
				});
			}
		}
		protected Paddings ContentPaddings {
			get {
				return GetCachedValue<Paddings>("ContentPaddings", delegate {
					Paddings result = new Paddings();
					result.CopyFrom(ItemStyle.Paddings);
					Unit spacing = Menu.GetPopOutImageCellSpacing(Item);
					if(!spacing.IsEmpty && (HasPopOutControl || (MenuOrientation == Orientation.Vertical && HasPopOutImages))) {
						if(ImagePosition == ImagePosition.Right) {
							Unit paddingsSum = UnitUtils.GetPaddingsSum(result.GetPaddingLeft(), spacing);
							if(IsRightToLeft)
								result.PaddingRight = paddingsSum;
							else
								result.PaddingLeft = paddingsSum;
						}
						else {
							Unit paddingsSum = UnitUtils.GetPaddingsSum(result.GetPaddingRight(), spacing);
							if(IsRightToLeft)
								result.PaddingLeft = paddingsSum;
							else
								result.PaddingRight = paddingsSum;
						}
					}
					return result;
				});
			}
		}
		protected override HtmlTextWriterTag TagKey { get { return ShowAsHyperLink ? HtmlTextWriterTag.A : HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			HyperLinkControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(HasImage && IsImageBeforeText)
				CreateImageControl();
			CreateTextControl();
			if(HasImage && !IsImageBeforeText)
				CreateImageControl();
		}
		WebControl GetCreateHyperLink() {
			if(HyperLinkControl == null) {
				HyperLinkControl = new InternalHyperLink(true);
				HyperLinkControl.IsAlwaysHyperLink = true;
				Controls.Add(HyperLinkControl);
			}
			return HyperLinkControl;
		}
		protected void AddContentControlCore(bool isInHyperLink, Control control) {
			WebControl container = isInHyperLink ? GetCreateHyperLink() : this;
			container.Controls.Add(control);
		}
		protected void CreateTextControl() {
			AddContentControlCore(IsTextInHyperLink, new MenuItemTextLite(this, !HasNavigateUrl ? LinkToolTip : ""));
		}
		protected void CreateImageControl() {
			AddContentControlCore(IsImageInHyperLink, new MenuItemImageLite(this, !HasNavigateUrl ? LinkToolTip : ""));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ContentContainerCssClass);
			if(!string.IsNullOrEmpty(Menu.GetItemText(Item)))
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.ItemHasTextCssClass);
			if(ShowAsHyperLink)
				RenderUtils.AppendDefaultDXClassName(this, MenuStyles.DXCssClass);
			ContentPaddings.AssignToControl(this);
			RenderUtils.SetHorizontalAlign(this, ItemStyle.HorizontalAlign);
			RenderUtils.SetVerticalAlign(this, ItemStyle.VerticalAlign);
			if(HyperLinkControl != null)
				PrepareHyperLink(HyperLinkControl);
			else if(ShowAsHyperLink) {
				RenderUtils.SetStringAttribute(this, "href", ResolveClientUrl(NavigateUrl));
				RenderUtils.SetStringAttribute(this, "target", HyperLinkTarget);
				RenderUtils.SetStringAttribute(this, "title", LinkToolTip);
				RenderUtils.PrepareHyperLinkForAccessibility(this, IsItemEnabled, IsAccessibilityCompliant, true);
				HyperLinkStyle.AssignToHyperLink(this, false);
			}
		}
		protected void PrepareHyperLink(HyperLink hyperLink) {
			hyperLink.CssClass = MenuStyles.DXCssClass;
			RenderUtils.PrepareHyperLink(hyperLink, "", NavigateUrl, HyperLinkTarget, LinkToolTip, IsItemEnabled);
			RenderUtils.PrepareHyperLinkForAccessibility(hyperLink, IsItemEnabled, IsAccessibilityCompliant, true);
			HyperLinkStyle.AssignToHyperLink(hyperLink, !IsTextInHyperLink);
		}
	}
	public class MenuItemTextLite : MenuItemLiteBase {
		string itemToolTip;
		WebControl textSpan, adaptiveTextSpan;
		LiteralControl textControl, adaptiveTextControl;
		public MenuItemTextLite(MenuItemLiteBase owner, string toolTip)
			: base(owner) {
			this.itemToolTip = toolTip;
		}
		protected string ItemToolTip { get { return itemToolTip; } set { itemToolTip = value; } }
		protected WebControl TextSpan { get { return textSpan; } set { textSpan = value; } }
		protected WebControl AdaptiveTextSpan { get { return adaptiveTextSpan; } set { adaptiveTextSpan = value; } }
		protected LiteralControl TextControl { get { return textControl; } set { textControl = value; } }
		protected LiteralControl AdaptiveTextControl { get { return adaptiveTextControl; } set { adaptiveTextControl = value; } }
		protected ITemplate ItemTextTemplate {
			get {
				return GetCachedValue<ITemplate>("ItemTextTemplate", delegate {
					return Menu.GetItemTextTemplate(Item);
				});
			}
		}
		protected string ItemTextTemplateContainerID {
			get {
				return GetCachedValue<string>("ItemTextTemplateContainerID", delegate {
					return Menu.GetItemTextTemplateContainerID(Item);
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			TextSpan = null;
			AdaptiveTextSpan = null;
			TextControl = null;
			AdaptiveTextControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!string.IsNullOrEmpty(Text) || !string.IsNullOrEmpty(AdaptiveText) || !HasImage) {
				if(HasItemTextTemplate)
					CreateTemplateContainer(this, Item, ItemTextTemplateContainerID, ItemTextTemplate);
				else {
					if(!string.IsNullOrEmpty(Text) || !HasImage) {
						TextSpan = new WebControl(HtmlTextWriterTag.Span);
						Controls.Add(TextSpan);
						TextControl = RenderUtils.CreateLiteralControl();
						TextSpan.Controls.Add(TextControl);
					}
					if(!string.IsNullOrEmpty(AdaptiveText)) {
						AdaptiveTextSpan = new WebControl(HtmlTextWriterTag.Span);
						Controls.Add(AdaptiveTextSpan);
						AdaptiveTextControl = RenderUtils.CreateLiteralControl();
						AdaptiveTextSpan.Controls.Add(AdaptiveTextControl);
					}
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(TextSpan != null) {
				PrepareTextSpan(TextSpan);
				if(AdaptiveTextSpan != null)
					RenderUtils.AppendDefaultDXClassName(TextSpan, MenuStyles.AdaptiveItemRegularTextCssClass);
			}
			if(AdaptiveTextSpan != null) {
				PrepareTextSpan(AdaptiveTextSpan);
				RenderUtils.AppendDefaultDXClassName(AdaptiveTextSpan, MenuStyles.AdaptiveItemTextCssClass);
			}
			if(TextControl != null)
				TextControl.Text = GetProcessedText(Text);
			if(AdaptiveTextControl != null)
				AdaptiveTextControl.Text = GetProcessedText(AdaptiveText);
		}
		private void PrepareTextSpan(WebControl textSpan) {
			if(!string.IsNullOrEmpty(ItemToolTip))
				textSpan.ToolTip = ItemToolTip;
			RenderUtils.SetVerticalAlignClass(textSpan, ItemStyle.VerticalAlign);
			RenderUtils.SetWrap(textSpan, ItemStyle.Wrap);
		}
		private string GetProcessedText(string text) {
			return string.IsNullOrEmpty(text) ? "&nbsp;" : RenderUtils.ProtectTextWhitespaces(text);
		}
	}
	public class MenuItemImageLite : MenuItemLiteBase {
		string itemToolTip;
		Image imageControl;
		public MenuItemImageLite(MenuItemLiteBase owner, string toolTip)
			: base(owner) {
			this.itemToolTip = toolTip;
		}
		protected string ItemToolTip { get { return itemToolTip; } set { itemToolTip = value; } }
		protected Image ImageControl { get { return imageControl; } set { imageControl = value; } }
		protected Margins ImageMargins {
			get {
				return GetCachedValue<Margins>("ImageMargins", delegate {
					Margins result = new Margins();
					Unit spacing = Menu.GetItemImageSpacing(Item);
					if(!spacing.IsEmpty) {
						if(ImagePosition == ImagePosition.Left) {
							if(IsRightToLeft)
								result.MarginLeft = spacing;
							else
								result.MarginRight = spacing;
						}
						else if(ImagePosition == ImagePosition.Right) {
							if(IsRightToLeft)
								result.MarginRight = spacing;
							else
								result.MarginLeft = spacing;
						}
						else if(ImagePosition == ImagePosition.Top)
							result.MarginBottom = spacing;
						else if(ImagePosition == ImagePosition.Bottom)
							result.MarginTop = spacing;
					}
					return result;
				});
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ImageControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(ImagePosition == ImagePosition.Bottom && !string.IsNullOrEmpty(Text))
				CreateBr();
			CreateImage();
			if(ImagePosition == ImagePosition.Top && !string.IsNullOrEmpty(Text))
				CreateBr();
		}
		protected void CreateBr() {
			Controls.Add(RenderUtils.CreateBr());
		}
		protected void CreateImage() {
			ImageControl = RenderUtils.CreateImage();
			(ImageControl as InternalImage).PreventFixIETitle = true;
			Controls.Add(ImageControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ImageMargins.AssignToControl(ImageControl);
			ImageProperties.AssignToControl(ImageControl, DesignMode, !IsItemEnabled);
			if(string.IsNullOrEmpty(ImageControl.ToolTip))
				ImageControl.ToolTip = ItemToolTip;
			RenderUtils.AppendDefaultDXClassName(ImageControl, MenuStyles.ImageCssClass);
			RenderUtils.SetVerticalAlignClass(ImageControl, ItemStyle.VerticalAlign);
		}
	}
	public class MenuItemPopOutLite : MenuItemLiteBase {
		Image imageControl;
		public MenuItemPopOutLite(MenuItemLite owner) : base(owner) { }
		protected Image ImageControl { get { return imageControl; } set { imageControl = value; } }
		protected MenuItemStyleBase DropDownButtonStyle {
			get {
				return GetCachedValue<MenuItemStyleBase>("DropDownButtonStyle", delegate {
					return Menu.GetItemDropDownButtonStyle(ParentItem, Item, false);
				});
			}
		}
		protected Paddings DropDownButtonPaddings {
			get {
				return GetCachedValue<Paddings>("DropDownButtonPaddings", delegate {
					return Menu.GetItemDropDownButtonContentPaddings(ParentItem, Item);
				});
			}
		}
		protected ItemImageProperties PopOutImageProperties {
			get {
				return GetCachedValue<ItemImageProperties>("PopOutImageProperties", delegate {
					if(Menu.IsAdaptivityMenu(Item))
						return Menu.GetAdaptiveMenuImageProperties(Item);
					if(MenuOrientation == Orientation.Horizontal)
						return Menu.GetHorizontalPopOutImageProperties(Item);
					return Menu.GetVerticalPopOutImageProperties(Item);
				});
			}
		}
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ImageControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ImageControl = RenderUtils.CreateImage();
			Controls.Add(ImageControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopOutImageProperties.AssignToControl(ImageControl, DesignMode);
			RenderUtils.AppendDefaultDXClassName(ImageControl, MenuStyles.PopOutImageCssClass);
			CssClass = MenuStyles.PopOutContainerCssClass;
			if(ItemStyle.VerticalAlign != VerticalAlign.NotSet)
				RenderUtils.SetVerticalAlign(this, ItemStyle.VerticalAlign);
			if(IsDropDownMode || Menu.HasLightweightPopOutImageCell(Item)) {
				DropDownButtonStyle.AssignToControl(this, AttributesRange.Common);
				RenderUtils.SetPaddings(this, DropDownButtonPaddings);
			}
		}
	}
}
