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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DefaultProperty("Items")]
	public class MenuItem: CollectionItem, IEnumerable, IHierarchyData {
		internal const string DefaultText = "Item";
		private object dataItem = null;
		private MenuItemCollection items = null;
		private ASPxMenuBase menu = null;
		private MenuItemImageProperties image = null;
		private MenuItemImageProperties subMenuItemImage = null;
		private ItemImageProperties popOutImage = null;
		private MenuScrollButtonImageProperties scrollUpButtonImage = null;
		private MenuScrollButtonImageProperties scrollDownButtonImage = null;
		private ItemImageProperties subMenuPopOutImage = null;
		private MenuItemStyle itemStyle = null;
		private MenuScrollButtonStyle scrollButtonStyle = null;
		private MenuItemStyle subMenuItemStyle = null;
		private MenuStyle subMenuStyle = null;
		private ITemplate subMenuTemplate = null;
		private ITemplate template = null;
		private ITemplate textTemplate = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemAdaptivePriority"),
#endif
		DefaultValue(0), NotifyParentProperty(true)]
		public int AdaptivePriority {
			get {
				return GetIntProperty("AdaptivePriority", 0);
			}
			set {
				CommonUtils.CheckNegativeValue((double)value, "AdaptivePriority");
				SetIntProperty("AdaptivePriority", 0, value);
			}
		}
		[
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string AdaptiveText {
			get { return GetStringProperty("AdaptiveText", ""); }
			set {
				SetStringProperty("AdaptiveText", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemBeginGroup"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", false); }
			set {
				SetBoolProperty("BeginGroup", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemChecked"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool Checked {
			get { return GetBoolProperty("Checked", false); }
			set {
				if(Checked != value) {
					SetChecked(value);
					if(Menu != null)
						Menu.SetCheckedState(this);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return dataItem; }
			set { SetDataItem(value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		DefaultValue(""), Localizable(false)]
		public string DataPath {
			get { return GetStringProperty("DataPath", ""); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Depth {
			get {
				if (Menu != null)
					return !IsRootItem ? Parent.Depth + 1 : -1;
				else
					return -1;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string IndexPath {
			get {
				if(IsRootItem)
					return "";
				if(Parent.IsRootItem)
					return Index.ToString();
				return Parent.IndexPath + RenderUtils.IndexSeparator.ToString() + Index.ToString();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemDropDownMode"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool DropDownMode {
			get { return GetBoolProperty("DropDownMode", false); }
			set { SetBoolProperty("DropDownMode", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemEnabled"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				SetBoolProperty("Enabled", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemGroupName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string GroupName {
			get {
				return GetStringProperty("GroupName", "");
			}
			set {
				if(GroupName != value) {
					if(Menu != null && string.IsNullOrEmpty(value))
						Menu.RemoveItemGroup(this);
					SetStringProperty("GroupName", "", value);
					if(Menu != null && !string.IsNullOrEmpty(value)) {
						Menu.AddItemGroup(this);
						Menu.ValidateCheckedState();
					}
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemEnableScrolling"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableScrolling {
			get { return GetBoolProperty("EnableScrolling", false); }
			set {
				SetBoolProperty("EnableScrolling", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemNavigateUrl"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string NavigateUrl {
			get { return GetStringProperty("NavigateUrl", ""); }
			set {
				SetStringProperty("NavigateUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemSelected"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool Selected {
			get { return GetBoolProperty("Selected", false); }
			set {
				if(Menu != null) {
					if(value)
						Menu.SelectedItem = this;
					else
						if(Menu.SelectedItem == this)
							Menu.SelectedItem = null;
				}
				else
					SetSelected(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemTarget"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true),
		TypeConverter(typeof(TargetConverter))]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set {
				SetStringProperty("Text", DefaultText, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemItems"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public MenuItemCollection Items {
			get {
				if(items == null)
					items = CreateItemsCollection();
				return items;
			}
		}
		protected internal bool IsRootItem { get { return Parent == null; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuItemImageProperties Image {
			get {
				if(image == null)
					image = new MenuItemImageProperties(this);
				return image;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemSubMenuItemImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuItemImageProperties SubMenuItemImage {
			get {
				if(subMenuItemImage == null)
					subMenuItemImage = new MenuItemImageProperties(this);
				return subMenuItemImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemPopOutImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ItemImageProperties PopOutImage {
			get {
				if(popOutImage == null)
					popOutImage = new ItemImageProperties(this);
				return popOutImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemScrollUpButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuScrollButtonImageProperties ScrollUpButtonImage {
			get {
				if(scrollUpButtonImage == null)
					scrollUpButtonImage = new MenuScrollButtonImageProperties(this);
				return scrollUpButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemScrollDownButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuScrollButtonImageProperties ScrollDownButtonImage {
			get {
				if(scrollDownButtonImage == null)
					scrollDownButtonImage = new MenuScrollButtonImageProperties(this);
				return scrollDownButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemSubMenuPopOutImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ItemImageProperties SubMenuPopOutImage {
			get {
				if(subMenuPopOutImage == null)
					subMenuPopOutImage = new ItemImageProperties(this);
				return subMenuPopOutImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemItemStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemStyle ItemStyle {
			get {
				if(itemStyle == null)
					itemStyle = new MenuItemStyle();
				return itemStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemScrollButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuScrollButtonStyle ScrollButtonStyle {
			get {
				if(scrollButtonStyle == null)
					scrollButtonStyle = new MenuScrollButtonStyle();
				return scrollButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemSubMenuItemStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemStyle SubMenuItemStyle {
			get {
				if(subMenuItemStyle == null)
					subMenuItemStyle = new MenuItemStyle();
				return subMenuItemStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuItemSubMenuStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuStyle SubMenuStyle {
			get {
				if(subMenuStyle == null)
					subMenuStyle = new MenuStyle();
				return subMenuStyle;
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate Template {
			get { return template; }
			set {
				template = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate TextTemplate {
			get { return textTemplate; }
			set {
				textTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate SubMenuTemplate {
			get { return subMenuTemplate; }
			set {
				subMenuTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasChildren {
			get { return Items.Count > 0; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasVisibleChildren {
			get { return Items.GetVisibleItemCount() > 0; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxMenuBase Menu {
			get {
				if (menu != null)
					return menu;
				else if (Collection is MenuItemCollection)
					return (Collection as MenuItemCollection).Menu;
				return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MenuItem Parent {
			get {
				if (Collection is MenuItemCollection)
					return (Collection as MenuItemCollection).MenuItem;
				return null;
			}
		}
		bool hasImageCell;
		protected internal virtual bool HasImageCell {
			get {
				return GetCachedValue<bool>(delegate() {
					if(Parent != null) {
						if(!Menu.IsLargeItems(Parent)) {
							if(Menu.IsVertical(Parent)) {
								Unit gutterWidth = Menu.GetGutterWidth(Parent);
								return !gutterWidth.IsEmpty && gutterWidth.Value > 0 || Menu.HasParentImageCellInternal(Parent);
							}
							else
								return !Menu.GetItemImageProperties(this).IsEmpty;
						}
						return false;
					}
					return false;
				}, "HasImageCell", ref hasImageCell);
			}
		}
		bool hasPopOutImageCell;
		protected internal virtual bool HasPopOutImageCell {
			get {
				return GetCachedValue<bool>(delegate() {
					if(Menu.CanItemSubMenuRender(this)) {
						if(HasVisibleChildren && (Menu.ShowPopOutImages == DefaultBoolean.True || Menu.IsDropDownMode(this)))
							return true;
						if(Menu.IsVertical(Parent)) {
							foreach(MenuItem subItem in Parent.Items.GetVisibleItems()) {
								if(subItem.HasVisibleChildren && (Menu.ShowPopOutImages != DefaultBoolean.False || Menu.IsDropDownMode(subItem)))
									return true;
							}
						}
						return false;
					}
					return false;
				}, "HasPopOutImageCell", ref hasPopOutImageCell);
			}
		}
		Dictionary<string, bool> evaled = new Dictionary<string, bool>();
		protected T GetCachedValue<T>(Func<T> create, string key, ref T value) {
			if(!evaled.ContainsKey(key) || !Menu.UseCachedObjects()) {
				value = create();
				evaled[key] = true;
			}
			return value;
		}
		public MenuItem()
			: base() {
		}
		public MenuItem(string text)
			: this(text, "", "", "", "") {
		}
		public MenuItem(string text, string name)
			: this(text, name, "", "", "") {
		}
		public MenuItem(string text, string name, string imageUrl)
			: this(text, name, imageUrl, "", "") {
		}
		public MenuItem(string text, string name, string imageUrl, string navigateUrl)
			: this(text, name, imageUrl, navigateUrl, "") {
		}
		public MenuItem(string text, string name, string imageUrl, string navigateUrl, string target)
			: base() {
			Text = text;
			Image.Url = imageUrl;
			Name = name;
			NavigateUrl = navigateUrl;
			Target = target;
		}
		protected internal MenuItem(ASPxMenuBase menuControl)
			: this() {
			this.menu = menuControl;
		}
		public override void Assign(CollectionItem source) {
			if (source is MenuItem) {
				MenuItem src = source as MenuItem;
				AdaptivePriority = src.AdaptivePriority;
				AdaptiveText = src.AdaptiveText;
				BeginGroup = src.BeginGroup;
				ClientEnabled = src.ClientEnabled;
				ClientVisible = src.ClientVisible;
				DropDownMode = src.DropDownMode;
				Enabled = src.Enabled;
				Items.Assign(src.Items);
				EnableScrolling = src.EnableScrolling;
				Name = src.Name;
				NavigateUrl = src.NavigateUrl;
				Selected = src.Selected;
				Target = src.Target;
				Text = src.Text;
				ToolTip = src.ToolTip;
				Visible = src.Visible;
				GroupName = src.GroupName;
				Checked = src.Checked;
				Image.Assign(src.Image);
				SubMenuItemImage.Assign(src.SubMenuItemImage);
				PopOutImage.Assign(src.PopOutImage);
				ScrollUpButtonImage.Assign(src.ScrollUpButtonImage);
				ScrollDownButtonImage.Assign(src.ScrollDownButtonImage);
				SubMenuPopOutImage.Assign(src.SubMenuPopOutImage);
				ItemStyle.Assign(src.ItemStyle);
				ScrollButtonStyle.Assign(src.ScrollButtonStyle); 
				SubMenuItemStyle.Assign(src.SubMenuItemStyle);
				SubMenuStyle.Assign(src.SubMenuStyle);
				Template = src.Template;
				TextTemplate = src.TextTemplate;
				SubMenuTemplate = src.SubMenuTemplate;
			}
			base.Assign(source);
		}
		public Control FindControl(string id) {
			return TemplateContainerBase.FindTemplateControl(Menu, Menu.GetItemTextTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(Menu, Menu.GetItemTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(Menu, Menu.GetMenuTemplateContainerID(this), id);
		}
		public override string ToString() {
			if (Text != "")
				return Text;
			return GetType().Name;
		}
		protected virtual MenuItemCollection CreateItemsCollection() {
			return new MenuItemCollection(this);
		}
		protected internal void SetDataItem(object value) {
			this.dataItem = value;
		}
		protected internal void SetChecked(bool value) {
			SetBoolProperty("Checked", false, value);
		}
		protected internal void SetDataPath(string value) {
			SetStringProperty("DataPath", "", value);
		}
		protected internal void SetSelected(bool value) {
			SetBoolProperty("Selected", false, value);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Items, Image, SubMenuItemImage, PopOutImage, ScrollUpButtonImage, ScrollDownButtonImage, SubMenuPopOutImage, 
				SubMenuStyle, SubMenuItemStyle, ItemStyle, ScrollButtonStyle };
		}
		protected override bool IsLoading() {
			if (Menu != null)
				return (Menu as IWebControlObject).IsLoading();
			return base.IsLoading();
		}
		protected override bool IsDesignMode() {
			if (Menu != null)
				return (Menu as IWebControlObject).IsDesignMode();
			return base.IsDesignMode();
		}
		protected override void LayoutChanged() {
			if (Menu != null)
				(Menu as IWebControlObject).LayoutChanged();
			else
				base.LayoutChanged();
		}
		protected override void TemplatesChanged() {
			if (Menu != null)
				(Menu as IWebControlObject).TemplatesChanged();
			else
				base.TemplatesChanged();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Items.GetEnumerator();
		}
		bool IHierarchyData.HasChildren {
			get {
				return Items.Count > 0;
			}
		}
		Object IHierarchyData.Item { get { return this; } }
		string IHierarchyData.Path {
			get {
				return "";
			}
		}
		string IHierarchyData.Type {
			get {
				return this.GetType().Name;
			}
		}
		IHierarchicalEnumerable IHierarchyData.GetChildren() {
			return Items;
		}
		IHierarchyData IHierarchyData.GetParent() {
			return Parent;
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class MenuItemCollection : HierarchicalCollection<MenuItem> {
		public MenuItemCollection()
			: base() {
		}
		public MenuItemCollection(MenuItem menuItem)
			: base(menuItem) {
		}
		protected internal ASPxMenuBase Menu {
			get { return (MenuItem != null) ? MenuItem.Menu : null; }
		}
		protected internal MenuItem MenuItem {
			get { return Owner as MenuItem; }
		}
		public MenuItem Add() {
			return AddInternal(new MenuItem());
		}
		public MenuItem Add(string text) {
			return Add(text, "", "", "", "");
		}
		public MenuItem Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public MenuItem Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public MenuItem Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public MenuItem Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			return AddInternal(new MenuItem(text, name, imageUrl, navigateUrl, target));
		}
		public MenuItem FindByName(string name) {
			return FindRecursive(delegate(MenuItem item) {
				return item.Name == name;
			});
		}
		public MenuItem FindByText(string text) {
			return FindRecursive(delegate(MenuItem item) {
				return item.Text == text;
			});
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(MenuItem item) {
				return item.Name == name;
			});
		}
		public int IndexOfText(string text) {
			return IndexOf(delegate(MenuItem item) {
				return item.Text == text;
			});
		}
		protected internal MenuItem CreateMenuItem() {
			return CreateKnownType(0) as MenuItem;
		}
		protected override void OnChanged() {
			if (Menu != null)
				Menu.ItemsChanged();
		}
	}
}
namespace DevExpress.Web.Internal {
	[DefaultProperty("Items")]
	public class AdaptiveMenuItem : MenuItem {
		public AdaptiveMenuItem()
			: base("") {
		}
		protected internal override bool HasPopOutImageCell {
			get { return true; }
		}
		protected internal override bool HasImageCell {
			get { return true; }
		}
	}
}
