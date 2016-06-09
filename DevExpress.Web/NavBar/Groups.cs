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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;
using DevExpress.Web.Design;
using System.Collections;
namespace DevExpress.Web {
	public enum ExpandButtonPosition { Default, Left, Right }
	public enum ItemBulletStyle { None, Disc, Circle, Square, Decimal, LowerRoman, UpperRoman, LowerAlpha, UpperAlpha }
	public enum GroupItemLinkMode { Default, TextOnly, TextAndImage, ContentBounds }
	public class NavBarGroup : CollectionItem {
		private const string DefaultText = "Group";
		private object fDataItem = null;
		private NavBarItemCollection fItems = null;
		private ImageProperties fCollapseImage = null;
		private ImageProperties fExpandImage = null;
		private ImageProperties fHeaderImage = null;
		private ImageProperties fHeaderImageCollapsed = null;
		private ItemImageProperties fItemImage = null;
		private NavBarGroupHeaderStyle fHeaderStyle = null;
		private NavBarGroupHeaderStyle fHeaderStyleCollapsed = null;
		private NavBarGroupContentStyle fContentStyle = null;
		private NavBarItemStyle fItemStyle = null;
		private ITemplate fContentTemplate = null;
		private ITemplate fHeaderTemplate = null;
		private ITemplate fHeaderTemplateCollapsed = null;
		private ITemplate fItemTemplate = null;
		private ITemplate fItemTextTemplate = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupAllowDragging"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowDragging {
			get { return GetBoolProperty("AllowDragging", true); }
			set { SetBoolProperty("AllowDragging", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupAllowExpanding"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowExpanding {
			get { return GetBoolProperty("AllowExpanding", true); }
			set { SetBoolProperty("AllowExpanding", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupCollapseImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties CollapseImage {
			get {
				if(fCollapseImage == null)
					fCollapseImage = new ImageProperties(this);
				return fCollapseImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return fDataItem; }
			set { SetDataItem(value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		DefaultValue(""), Localizable(false)]
		public string DataPath {
			get { return GetStringProperty("DataPath", ""); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupEnabled"),
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
	DevExpressWebLocalizedDescription("NavBarGroupExpanded"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Expanded {
			get { return GetBoolProperty("Expanded", true); }
			set {
				SetBoolProperty("Expanded", true, value);
				if((NavBar != null) && Visible)
					NavBar.ValidateAutoCollapse(this);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupShowExpandButton"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean ShowExpandButton {
			get { return GetDefaultBooleanProperty("ShowExpandButton", DefaultBoolean.Default); }
			set {
				SetDefaultBooleanProperty("ShowExpandButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupExpandImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties ExpandImage {
			get {
				if(fExpandImage == null)
					fExpandImage = new ImageProperties(this);
				return fExpandImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupHeaderImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties HeaderImage {
			get {
				if(fHeaderImage == null)
					fHeaderImage = new ImageProperties(this);
				return fHeaderImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupHeaderImageCollapsed"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties HeaderImageCollapsed {
			get {
				if(fHeaderImageCollapsed == null)
					fHeaderImageCollapsed = new ImageProperties(this);
				return fHeaderImageCollapsed;
			}
		}
		static object expandButtonPositionDefault = ExpandButtonPosition.Default;
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupExpandButtonPosition"),
#endif
		Category("Appearance"), DefaultValue(ExpandButtonPosition.Default), NotifyParentProperty(true)]
		public ExpandButtonPosition ExpandButtonPosition {
			get { return (ExpandButtonPosition)GetEnumProperty("ExpandButtonPosition ", expandButtonPositionDefault); }
			set {
				SetEnumProperty("ExpandButtonPosition ", expandButtonPositionDefault, value);
				LayoutChanged();
			}
		}
		static object groupItemLinkModeDefault = GroupItemLinkMode.Default;
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupItemLinkMode"),
#endif
		DefaultValue(GroupItemLinkMode.Default), NotifyParentProperty(true)]
		public GroupItemLinkMode ItemLinkMode {
			get { return (GroupItemLinkMode)GetEnumProperty("ItemLinkMode", groupItemLinkModeDefault); }
			set {
				SetEnumProperty("ItemLinkMode", groupItemLinkModeDefault, value);
				LayoutChanged();
			}
		}
		static object imagePositionLeft = ImagePosition.Left;
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupItemImagePosition"),
#endif
		Category("Appearance"), DefaultValue(ImagePosition.Left), NotifyParentProperty(true)]
		public ImagePosition ItemImagePosition {
			get { return (ImagePosition)GetEnumProperty("ItemImagePosition", imagePositionLeft); }
			set {
				SetEnumProperty("ItemImagePosition", imagePositionLeft, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupItemImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ItemImageProperties ItemImage {
			get {
				if(fItemImage == null)
					fItemImage = new ItemImageProperties(this);
				return fItemImage;
			}
		}
		static object itemBulletStyleNone = ItemBulletStyle.None;
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupItemBulletStyle"),
#endif
		DefaultValue(ItemBulletStyle.None), NotifyParentProperty(true)]
		public ItemBulletStyle ItemBulletStyle {
			get { return (ItemBulletStyle)GetEnumProperty("ItemBulletStyle", itemBulletStyleNone); }
			set {
				SetEnumProperty("ItemBulletStyle", itemBulletStyleNone, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupNavigateUrl"),
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
	DevExpressWebLocalizedDescription("NavBarGroupTarget"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true),
		TypeConverter(typeof(TargetConverter))]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set { SetStringProperty("Text", DefaultText, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public NavBarItemCollection Items {
			get {
				if(fItems == null)
					fItems = CreateItemsCollection();
				return fItems;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupHeaderStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavBarGroupHeaderStyle HeaderStyle {
			get {
				if(fHeaderStyle == null)
					fHeaderStyle = new NavBarGroupHeaderStyle();
				return fHeaderStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupHeaderStyleCollapsed"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavBarGroupHeaderStyle HeaderStyleCollapsed {
			get {
				if(fHeaderStyleCollapsed == null)
					fHeaderStyleCollapsed = new NavBarGroupHeaderStyle();
				return fHeaderStyleCollapsed;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupContentStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavBarGroupContentStyle ContentStyle {
			get {
				if(fContentStyle == null)
					fContentStyle = new NavBarGroupContentStyle();
				return fContentStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupItemStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavBarItemStyle ItemStyle {
			get {
				if(fItemStyle == null)
					fItemStyle = new NavBarItemStyle();
				return fItemStyle;
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarGroupTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ContentTemplate {
			get { return fContentTemplate; }
			set {
				fContentTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarGroupTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderTemplate {
			get { return fHeaderTemplate; }
			set {
				fHeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarGroupTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderTemplateCollapsed {
			get { return fHeaderTemplateCollapsed; }
			set {
				fHeaderTemplateCollapsed = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTemplate {
			get { return fItemTemplate; }
			set {
				fItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTextTemplate {
			get { return fItemTextTemplate; }
			set {
				fItemTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxNavBar NavBar {
			get {
				return (Collection != null) &&
					(Collection.Owner is ASPxNavBar) ? Collection.Owner as ASPxNavBar : null;
			}
		}
		protected internal bool HasContent {
			get { return HasVisibleItems() || NavBar != null && NavBar.GetGroupContentTemplate(this) != null; }
		}
		public NavBarGroup()
			: base() {
		}
		public NavBarGroup(string text)
			: this(text, "", "", "", "") {
		}
		public NavBarGroup(string text, string name)
			: this(text, name, "", "", "") {
		}
		public NavBarGroup(string text, string name, string imageUrl)
			: this(text, name, imageUrl, "", "") {
		}
		public NavBarGroup(string text, string name, string imageUrl, string navigateUrl)
			: this(text, name, imageUrl, navigateUrl, "") {
		}
		public NavBarGroup(string text, string name, string imageUrl, string navigateUrl, string target)
			: base() {
			Text = text;
			Name = name;
			HeaderImage.Url = imageUrl;
			NavigateUrl = navigateUrl;
			Target = target;
		}
		public override void Assign(CollectionItem source) {
			if (source is NavBarGroup) {
				NavBarGroup src = source as NavBarGroup;
				AllowDragging = src.AllowDragging;
				AllowExpanding = src.AllowExpanding;
				ClientVisible = src.ClientVisible;
				CollapseImage.Assign(src.CollapseImage);
				Enabled = src.Enabled;
				Expanded = src.Expanded;
				ShowExpandButton = src.ShowExpandButton;
				ExpandButtonPosition = src.ExpandButtonPosition;
				ExpandImage.Assign(src.ExpandImage);
				HeaderImage.Assign(src.HeaderImage);
				HeaderImageCollapsed.Assign(src.HeaderImageCollapsed);
				ItemLinkMode = src.ItemLinkMode;
				ItemImagePosition = src.ItemImagePosition;
				ItemImage.Assign(src.ItemImage);
				ItemBulletStyle = src.ItemBulletStyle;
				Items.Assign(src.Items);
				Name = src.Name;
				NavigateUrl = src.NavigateUrl;
				Target = src.Target;
				Text = src.Text;
				ToolTip = src.ToolTip;
				Visible = src.Visible;
				HeaderStyle.Assign(src.HeaderStyle);
				HeaderStyleCollapsed.Assign(src.HeaderStyleCollapsed);
				ContentStyle.Assign(src.ContentStyle);
				ItemStyle.Assign(src.ItemStyle);
				ContentTemplate = src.ContentTemplate;
				HeaderTemplate = src.HeaderTemplate;
				HeaderTemplateCollapsed = src.HeaderTemplateCollapsed;
				ItemTemplate = src.ItemTemplate;
				ItemTextTemplate = src.ItemTextTemplate;
			}
			base.Assign(source);
		}
		public Control FindControl(string id) {
			Control control = FindControlInHeader(id, Expanded);
			if (control != null)
				return control;
			control = FindControlInHeader(id, !Expanded);
			if (control != null)
				return control;
			control = FindControlInContent(id);
			if (control != null)
				return control;
			return null;
		}
		protected Control FindControlInHeader(string id, bool expanded) {
			return TemplateContainerBase.FindTemplateControl(NavBar, NavBar.GetGroupHeaderTemplateContainerID(this, expanded), id);
		}
		protected Control FindControlInContent(string id) {
			return TemplateContainerBase.FindTemplateControl(NavBar, NavBar.GetGroupContentTemplateContainerID(this), id);
		}
		public bool HasVisibleItems() {
			return Items.GetVisibleItemCount() > 0;
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected virtual NavBarItemCollection CreateItemsCollection() {
			return new NavBarItemCollection(this);
		}
		protected internal void SetDataItem(object value) {
			fDataItem = value;
		}
		protected internal void SetDataPath(string value) {
			SetStringProperty("DataPath", "", value);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Items, HeaderStyle, HeaderStyleCollapsed, ContentStyle, ItemStyle, 
				ExpandImage, CollapseImage, ItemImage, HeaderImage, HeaderImageCollapsed };
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class NavBarGroupCollection : Collection<NavBarGroup> {
		public NavBarGroupCollection()
			: base() {
		}
		public NavBarGroupCollection(ASPxNavBar navBar)
			: base(navBar) {
		}
		protected internal ASPxNavBar NavBar {
			get { return Owner as ASPxNavBar; }
		}
		[Obsolete("This method is now obsolete. Use the GetVisibleItem(int index) method instead.")]
		public NavBarGroup GetVisibleGroup(int index) {
			return GetVisibleItem(index);
		}
		[Obsolete("This method is now obsolete. Use the GetVisibleItemCount() method instead.")]
		public int GetVisibleGroupCount() {
			return GetVisibleItemCount();
		}
		public NavBarGroup Add() {
			return AddInternal(new NavBarGroup());
		}
		public NavBarGroup Add(string text) {
			return Add(text, "", "", "", "");
		}
		public NavBarGroup Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public NavBarGroup Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public NavBarGroup Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public NavBarGroup Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			return AddInternal(new NavBarGroup(text, name, imageUrl, navigateUrl, target));
		}
		public NavBarGroup FindByName(string name) {
			return FindByIndex(IndexOfName(name));
		}
		public NavBarGroup FindByText(string text) {
			return FindByIndex(IndexOfText(text));
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(NavBarGroup group) {
				return group.Name == name;
			});
		}
		public int IndexOfText(string text) {
			return IndexOf(delegate(NavBarGroup group) {
				return group.Text == text;
			});
		}
		public void CollapseAll(NavBarGroup exceptGroup) {
			for (int i = 0; i < Count; i++) {
				if (this[i] != exceptGroup)
					this[i].Expanded = false;
			}
		}
		public void CollapseAll() {
			CollapseAll(null);
		}
		public void ExpandAll() {
			for (int i = 0; i < Count; i++) {
				this[i].Expanded = true;
			}
		}
		protected override void OnChanged() {
			if (NavBar != null)
				NavBar.GroupsChanged();
		}
	}
}
