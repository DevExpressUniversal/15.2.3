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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.Web {
	public class RibbonTab : CollectionItem {
		const string DefaultText = "Tab";
		bool synchronizeName = true;
		bool synchronizeText = true;
		RibbonGroupCollection groups;
		TabStyle tabStyle;
		TabStyle activeTabStyle;
		public RibbonTab() { }
		public RibbonTab(string text)
			: this(text, text) { }
		public RibbonTab(string text, string name) {
			Text = text;
			Name = name;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonTabText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string Text
		{
			get { return GetStringProperty("Text", DefaultText); }
			set
			{
				SetStringProperty("Text", DefaultText, value);
				synchronizeText = false;
				if (synchronizeName && IsDesignMode() && string.IsNullOrEmpty(Name))
					Name = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonTabName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public string Name
		{
			get { return GetStringProperty("Name", string.Empty); }
			set
			{
				SetStringProperty("Name", string.Empty, value);
				synchronizeName = false;
				if (synchronizeText && IsDesignMode() && string.IsNullOrEmpty(Text))
					Text = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonTabVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible
		{
			get { return GetBoolProperty("Visible", true); }
			set
			{
				SetBoolProperty("Visible", true, value);
				LayoutChanged();
			}
		}
		[Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle ActiveTabStyle {
			get {
				if(activeTabStyle == null)
					activeTabStyle = new TabStyle();
				return activeTabStyle;
			}
		}
		[Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle TabStyle {
			get {
				if(tabStyle == null)
					tabStyle = new TabStyle();
				return tabStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonTabGroups"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, NotifyParentProperty(true)]
		public RibbonGroupCollection Groups
		{
			get
			{
				if (groups == null)
					groups = CreateRibbonGroupCollection();
				return groups;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		DefaultValue(""), Localizable(false)]
		public string DataPath {
			get { return GetStringProperty("DataPath", ""); }
			internal set { SetStringProperty("DataPath", "", value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRibbon Ribbon {
			get { return Collection != null && Collection is RibbonTabCollection ? ((RibbonTabCollection)Collection).Ribbon : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonContextTabCategory ContextTabCategory {
			get { return Collection != null && Collection is RibbonTabCollection ? ((RibbonTabCollection)Collection).ContextTabCategory : null; }
		}
		protected new internal bool ClientVisibleInternal {
			get { return base.ClientVisibleInternal; }
			set { 
				base.ClientVisibleInternal = value;
				LayoutChanged();
			}
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Groups;
		}
		protected internal virtual string GetName() {
			return Name;
		}
		protected internal virtual string GetText() {
			return Text;
		}
		protected internal string GetCommonIndex() {
			return IsContext ? Ribbon.AllTabs.FindIndex(tab => tab == this).ToString() : Index.ToString();
		}
		protected internal bool IsContext {
			get { return ContextTabCategory != null; }
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ActiveTabStyle, TabStyle, Groups });
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Name))
				return string.Format("{0} ({1})", base.ToString(), Name);
			return base.ToString();
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonTab tab = source as RibbonTab;
			if(tab != null) {
				Text = tab.Text;
				Name = tab.Name;
				Visible = tab.Visible;
				ActiveTabStyle.Assign(tab.ActiveTabStyle);
				TabStyle.Assign(tab.TabStyle);
				Groups.Assign(tab.Groups);
			}
		}
		protected virtual RibbonGroupCollection CreateRibbonGroupCollection() {
			return new RibbonGroupCollection(this);
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Groups" };
		}
	}
	public class RibbonContextTabCategory : CollectionItem {
		RibbonTabCollection tabs;
		public RibbonContextTabCategory() { }
		public RibbonContextTabCategory(string name) {
			Name = name;
		}
		public RibbonContextTabCategory(string name, Color color) : this(name) {
			Color = color;
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string Name {
			get { return GetStringProperty("Name", string.Empty); }
			set {
				SetStringProperty("Name", string.Empty, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRibbon Ribbon {
			get { return Collection != null && Collection is RibbonContextTabCategoryCollection ? ((RibbonContextTabCategoryCollection)Collection).Ribbon : null; }
		}
		[DefaultValue(typeof(Color), ""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public Color Color {
			get { return GetColorProperty("Color", Color.Empty); }
			set { SetColorProperty("Color", Color.Empty, value); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, NotifyParentProperty(true)]
		public RibbonTabCollection Tabs {
			get {
				if(tabs == null)
					tabs = CreateRibbonTabCollection();
				return tabs;
			}
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible {
			get {
				return GetBoolProperty("Visible", true);
			}
			set {
				SetBoolProperty("Visible", true, value);
				LayoutChanged();
			}
		}
		[DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ClientVisible {
			get {
				return GetBoolProperty("ClientVisible", false);
			}
			set {
				SetBoolProperty("ClientVisible", false, value);
				LayoutChanged();
			}
		}
		protected virtual RibbonTabCollection CreateRibbonTabCollection() {
			return new RibbonTabCollection(this, true);
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Tabs;
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Tabs });
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonContextTabCategory tabCategory = source as RibbonContextTabCategory;
			if(tabCategory != null) {
				Name = tabCategory.Name;
				Color = tabCategory.Color;
				Visible = tabCategory.Visible;
				ClientVisible = tabCategory.ClientVisible;
				Tabs.Assign(tabCategory.Tabs);
			}
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Tabs" };
		}
	}
	public class RibbonGroup : CollectionItem {
		const string DefaultText = "Group";
		bool synchronizeName = true;
		bool synchronizeText = true;
		RibbonItemCollection items;
		RibbonItemImageProperties image;
		RibbonGroupOneLineModeSettings oneLineModeSettings;
		public RibbonGroup() { }
		public RibbonGroup(string text)
			:this(text, text) { }
		public RibbonGroup(string text, string name) {
			Text = text;
			Name = name;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupText"),
#endif
		Localizable(true), Bindable(true), DefaultValue(""), RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true), AutoFormatDisable]
		public string Text
		{
			get { return GetStringProperty("Text", DefaultText); }
			set
			{
				SetStringProperty("Text", DefaultText, value);
				synchronizeText = false;
				if (IsDesignMode() && synchronizeName && string.IsNullOrEmpty(Name))
					Name = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupName"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true)]
		public string Name
		{
			get { return GetStringProperty("Name", string.Empty); }
			set
			{
				SetStringProperty("Name", string.Empty, value);
				synchronizeName = false;
				if (IsDesignMode() && synchronizeText && string.IsNullOrEmpty(Text))
					Text = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible
		{
			get { return GetBoolProperty("Visible", true); }
			set
			{
				SetBoolProperty("Visible", true, value);
				if(Collection != null)
					Collection.RefreshVisibleItemsList();
				LayoutChanged();
			}
		}
		[DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowDialogBoxLauncher {
			get { return GetBoolProperty("ShowDialogBoxLauncher", false); }
			set {
				SetBoolProperty("ShowDialogBoxLauncher", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupImage"),
#endif
		Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public RibbonItemImageProperties Image
		{
			get
			{
				if (image == null)
					image = new RibbonItemImageProperties(this);
				return image;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, NotifyParentProperty(true)]
		public RibbonItemCollection Items
		{
			get
			{
				if (items == null)
					items = CreateRibbonItemCollection();
				return items;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		DefaultValue(""), Localizable(false)]
		public string DataPath {
			get { return GetStringProperty("DataPath", ""); }
			internal set { SetStringProperty("DataPath", "", value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonTab Tab {
			get { return Collection != null && Collection is RibbonGroupCollection ? ((RibbonGroupCollection)Collection).Tab : null; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRibbon Ribbon {
			get {  return Tab != null ? Tab.Ribbon : null; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatDisable]
		public virtual RibbonGroupOneLineModeSettings OneLineModeSettings {
			get {
				if(oneLineModeSettings == null)
					oneLineModeSettings = new RibbonGroupOneLineModeSettings(this);
				return oneLineModeSettings;
			}
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Items;
		}
		protected internal virtual string GetName() {
			return Name;
		}
		protected internal virtual string GetText() {
			return Text;
		}
		protected internal virtual ItemImagePropertiesBase GetImage() {
			return Image;
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items, Image, OneLineModeSettings });
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Name))
				return string.Format("{0} ({1})", base.ToString(), Name);
			return base.ToString();
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonGroup group = source as RibbonGroup;
			if(group != null) {
				Text = group.Text;
				Name = group.Name;
				Visible = group.Visible;
				ShowDialogBoxLauncher = group.ShowDialogBoxLauncher;
				Image.Assign(group.Image);
				Items.Assign(group.Items);
				OneLineModeSettings.Assign(group.OneLineModeSettings);
			}
		}
		protected virtual RibbonItemCollection CreateRibbonItemCollection(){
			return new RibbonItemCollection(this);
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public abstract class RibbonItemBase : CollectionItem {
		bool synchronizeName = true;
		RibbonItemStyle itemStyle;
		public RibbonItemBase() { }
		public RibbonItemBase(string name)
			: this(name, name) { }
		public RibbonItemBase(string name, RibbonItemSize size)
			: this(name, name, size) { }
		public RibbonItemBase(string name, string text)
			: this(name, text, RibbonItemSize.Small) { }
		public RibbonItemBase(string name, string text, RibbonItemSize size) {
			Text = text;
			Name = name;
			ToolTip = text;
			Size = size;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseText"),
#endif
		DefaultValue(""), Localizable(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual string Text
		{
			get { return GetStringProperty("Text", string.Empty); }
			set
			{
				if (value == Text) return;
				SetStringProperty("Text", string.Empty, value);
				if (IsDesignMode() && synchronizeName && string.IsNullOrEmpty(Name))
					Name = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseName"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual string Name
		{
			get { return GetStringProperty("Name", string.Empty); }
			set
			{
				synchronizeName = false;
				SetStringProperty("Name", string.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseToolTip"),
#endif
		DefaultValue(""), Localizable(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual string ToolTip
		{
			get { return GetStringProperty("ToolTip", string.Empty); }
			set {
				if(value == ToolTip) return;
				SetStringProperty("ToolTip", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseBeginGroup"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool BeginGroup
		{
			get { return GetBoolProperty("BeginGroup", false); }
			set
			{
				if(value == BeginGroup) return;
				SetBoolProperty("BeginGroup", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible
		{
			get { return GetBoolProperty("Visible", true); }
			set
			{
				if(value == Visible) return;
				SetBoolProperty("Visible", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseClientEnabled"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ClientEnabled
		{
			get { return GetBoolProperty("ClientEnabled", true); }
			set
			{
				if(value == ClientEnabled) return;
				SetBoolProperty("ClientEnabled", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseSubGroupName"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string SubGroupName
		{
			get { return GetStringProperty("SubGroupName", string.Empty); }
			set { SetStringProperty("SubGroupName", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseSize"),
#endif
		DefaultValue(RibbonItemSize.Small), NotifyParentProperty(true), AutoFormatDisable]
		public virtual RibbonItemSize Size
		{
			get { return (RibbonItemSize)GetEnumProperty("Size", RibbonItemSize.Small); }
			set
			{
				if(value == Size) return;
				SetEnumProperty("Size", RibbonItemSize.Small, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemBaseItemStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public RibbonItemStyle ItemStyle
		{
			get
			{
				if (this.itemStyle == null)
					this.itemStyle = new RibbonItemStyle();
				return this.itemStyle;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		DefaultValue(""), Localizable(false)]
		public string DataPath {
			get { return GetStringProperty("DataPath", ""); }
			internal set { SetStringProperty("DataPath", "", value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]		
		public virtual RibbonGroup Group {
			get { return Collection != null && Collection is RibbonItemCollection ? ((RibbonItemCollection)Collection).Group : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRibbon Ribbon {
			get { return Group != null ? Group.Ribbon : null; }
		}
		protected internal virtual string GetText() {
			return Text;
		}
		protected internal virtual string GetName() {
			return Name;
		}
		protected internal virtual string GetToolTip() {
			return ToolTip;
		}
		protected internal virtual string GetSubGroupName() {
			return SubGroupName;
		}
		protected internal virtual RibbonItemSize GetSize() {
			return Size;
		}
		protected internal virtual bool IsButtonMode() {
			return false;
		}
		protected internal bool OneLineMode {
			get {
				return Ribbon != null && Ribbon.OneLineMode;
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonItemBase item = source as RibbonItemBase;
			if(item != null) {
				Text = item.Text;
				Name = item.Name;
				Size = item.Size;
				ToolTip = item.ToolTip;
				BeginGroup = item.BeginGroup;
				Visible = item.Visible;
				ClientEnabled = item.ClientEnabled;
				SubGroupName = item.SubGroupName;
				ItemStyle.Assign(item.ItemStyle);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ItemStyle });
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Name))
				return string.Format("{0} ({1})", base.ToString(), Name);
			return base.ToString();
		}
		protected internal abstract RibbonItemType ItemType { get; }
		protected internal virtual string GetIndexPath() {
			return string.Empty;
		}
		protected internal virtual ItemImagePropertiesBase GetSmallImage() {
			return new ItemImagePropertiesBase(this);
		}
		protected internal virtual ItemImagePropertiesBase GetLargeImage() {
			return new ItemImagePropertiesBase(this);
		}
	}
	public class RibbonTemplateItem : RibbonItemBase {
		ITemplate template;
		public RibbonTemplateItem()
			: base() { }
		public RibbonTemplateItem(string name)
			: base(name) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[Browsable(false), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(RibbonItemTemplateContainer)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate Template {
			get { return template; }
			set {
				template = value;
				TemplatesChanged();
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonTemplateItem item = source as RibbonTemplateItem;
			if(item != null) {
				Template = item.Template;
			}
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.Template; } }
		protected internal override bool IsButtonMode() {
			return Ribbon.OneLineMode;
		}
		public Control FindControl(string id) {
			string itemTemplateContainerID = RibbonHelper.GetItemTemplateContainerID(this);
			if(Ribbon != null)
				return TemplateContainerBase.FindTemplateControl(Ribbon, itemTemplateContainerID, id);
			else if(this.Group != null && this.Group.Tab != null && this.Group.Tab.Collection != null)
				return RibbonHelper.FindTemplateControlRecursive(this.Group.Tab.Collection.Owner as Control, itemTemplateContainerID, id);
			else
				return null;
		}
	}
	public class RibbonButtonItem : RibbonItemBase {
		RibbonItemImageProperties largeImage;
		RibbonItemImageProperties smallImage;
		public RibbonButtonItem()
			: base() { }
		public RibbonButtonItem(string name)
			: base(name) { }
		public RibbonButtonItem(string name, string text)
			: base(name, text) { }
		public RibbonButtonItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonButtonItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonButtonItemLargeImage"),
#endif
		Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual RibbonItemImageProperties LargeImage
		{
			get
			{
				if (largeImage == null)
					largeImage = new RibbonItemImageProperties(this);
				return largeImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonButtonItemSmallImage"),
#endif
		Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual RibbonItemImageProperties SmallImage
		{
			get
			{
				if (smallImage == null)
					smallImage = new RibbonItemImageProperties(this);
				return smallImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonButtonItemNavigateUrl"),
#endif
 UrlProperty,
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual string NavigateUrl
		{
			get { return GetStringProperty("NavigateUrl", string.Empty); }
			set { SetStringProperty("NavigateUrl", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonButtonItemTarget"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual string Target
		{
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonButtonItem item = source as RibbonButtonItem;
			if(item != null) {
				SmallImage.Assign(item.SmallImage);
				LargeImage.Assign(item.LargeImage);
				NavigateUrl = item.NavigateUrl;
				Target = item.Target;
			}
		}
		protected internal string GetNavigateUrl() {
			return NavigateUrl;
		} 
		protected internal string GetTarget() {
			return (Target != "") ? Target : (Ribbon != null ? Ribbon.Target : null);
		}
		protected internal override ItemImagePropertiesBase GetLargeImage() {
			return LargeImage;
		}
		protected internal override ItemImagePropertiesBase GetSmallImage() {
			return SmallImage;
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { LargeImage, SmallImage });
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.Button; } }
	}
	public class RibbonDropDownButtonItem : RibbonButtonItem, IHierarchyData {
		RibbonDropDownButtonCollection items;
		private ITemplate textTemplate = null;
		private ITemplate template = null;
		public RibbonDropDownButtonItem() { }
		public RibbonDropDownButtonItem(string name)
			: base(name) { }
		public RibbonDropDownButtonItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonDropDownButtonItem(string name, string text)
			: base(name, text) { }
		public RibbonDropDownButtonItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		public RibbonDropDownButtonItem(string name, string text, params RibbonDropDownButtonItem[] items)
			: base(name, text) {
			Items.AddRange(items);
		}
		public RibbonDropDownButtonItem(string name, string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(name, text, size) {
			Items.AddRange(items);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonDropDownButtonItemDropDownMode"),
#endif
		DefaultValue(true)]
		public virtual bool DropDownMode
		{
			get { return GetBoolProperty("DropDownMode", true); }
			set { SetBoolProperty("DropDownMode", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonDropDownButtonItemItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public virtual RibbonDropDownButtonCollection Items
		{
			get
			{
				if (items == null)
					items = new RibbonDropDownButtonCollection(this);
				return items;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RibbonDropDownButtonItem Parent {
			get {
				if(Collection is RibbonDropDownButtonCollection)
					return ((RibbonDropDownButtonCollection)Collection).Parent;
				return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RibbonGroup Group {
			get { return Parent != null ? Parent.Group : base.Group; }
		}
		protected internal virtual ITemplate TextTemplate {
			get { return textTemplate; }
			set {
				textTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal virtual ITemplate Template {
			get { return template; }
			set {
				template = value;
				TemplatesChanged();
			}
		}
		protected internal virtual RibbonDropDownButtonCollection GetItems() {
			return Items;
		}
		protected internal virtual bool GetDropDownMode() {
			return DropDownMode;
		}
		protected virtual internal bool? PopupMenuEncodeHtml { get; private set; }
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items });
		}
		protected internal override RibbonItemType ItemType {
			get { return GetDropDownMode() ? RibbonItemType.DropDownButton : RibbonItemType.DropDownMenuItem; } 
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonDropDownButtonItem item = source as RibbonDropDownButtonItem;
			if(item != null) {
				DropDownMode = item.DropDownMode;
				Items.Assign(item.Items);
			}
		}
		#region IHierarchyData Members
		IHierarchicalEnumerable IHierarchyData.GetChildren() {
			return Items;
		}
		IHierarchyData IHierarchyData.GetParent() {
			return Parent;
		}
		bool IHierarchyData.HasChildren {
			get { return Items.Any(); }
		}
		object IHierarchyData.Item {
			get { return this; }
		}
		string IHierarchyData.Path {
			get { return string.Empty; }
		}
		string IHierarchyData.Type {
			get { return GetType().Name; }
		}
		#endregion
		protected override IList GetDesignTimeItems() {
			return (IList)Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Items" });
		}
		protected internal override string GetIndexPath() {
			var indexPath = "";
			if (Parent != null) 
				indexPath = Parent.GetIndexPath();
			return string.IsNullOrEmpty(indexPath) ? Index.ToString() : string.Format("{0}i{1}", indexPath, Index);
		}
	}
	public class RibbonDropDownToggleButtonItem : RibbonDropDownButtonItem {
		public RibbonDropDownToggleButtonItem() { }
		public RibbonDropDownToggleButtonItem(string name)
			: base(name) { }
		public RibbonDropDownToggleButtonItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonDropDownToggleButtonItem(string name, string text)
			: base(name, text) { }
		public RibbonDropDownToggleButtonItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		public RibbonDropDownToggleButtonItem(string name, string text, params RibbonDropDownButtonItem[] items)
			: base(name, text, items) { }
		public RibbonDropDownToggleButtonItem(string name, string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items)
			: base(name, text, size, items) { }
		[DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual bool Checked {
			get { return GetBoolProperty("Checked", false); }
			set {
				SetBoolProperty("Checked", false, value);
				LayoutChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DropDownMode {
			get { return true; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonDropDownToggleButtonItem item = source as RibbonDropDownToggleButtonItem;
			if(item != null) {
				Checked = item.Checked;
			}
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.DropDownToggleButton; } }		
	}
	public class RibbonToggleButtonItem : RibbonButtonItem {
		public RibbonToggleButtonItem() { }
		public RibbonToggleButtonItem(string name)
			: base(name) { }
		public RibbonToggleButtonItem(string name, string text)
			: base(name, text) { }
		public RibbonToggleButtonItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonToggleButtonItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonToggleButtonItemChecked"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual bool Checked
		{
			get { return GetBoolProperty("Checked", false); }
			set { SetBoolProperty("Checked", false, value); }
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.ToggleButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string NavigateUrl {
			get { return base.NavigateUrl; }
			set { base.NavigateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Target {
			get { return base.Target; }
			set { base.Target = value; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonToggleButtonItem item = source as RibbonToggleButtonItem;
			if(item != null) {
				Checked = item.Checked;
			}
		}
	}
	public class RibbonOptionButtonItem : RibbonToggleButtonItem {
		public RibbonOptionButtonItem() { }
		public RibbonOptionButtonItem(string name)
			: base(name) { }
		public RibbonOptionButtonItem(string name, string text)
			: base(name, text) { }
		public RibbonOptionButtonItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonOptionButtonItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonOptionButtonItemOptionGroupName"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual string OptionGroupName
		{
			get { return GetStringProperty("OptionGroupName", ""); }
			set { SetStringProperty("OptionGroupName", "", value); }
		}
		[DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public override bool Checked {
			get { return base.Checked; }
			set {
				if(value && Ribbon != null)
					Ribbon.SetCheckedState(this);
				base.Checked = value;
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonOptionButtonItem item = source as RibbonOptionButtonItem;
			if(item != null) {
				OptionGroupName = item.OptionGroupName;
			}
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.OptionButton; } }
	}
	public abstract class RibbonColorButtonItemBase : RibbonButtonItem {
		ColorNestedControlProperties colorNestedControlProperties;
		public RibbonColorButtonItemBase() { }
		public RibbonColorButtonItemBase(string name)
			: base(name) { }
		public RibbonColorButtonItemBase(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonColorButtonItemBase(string name, string text)
			: base(name, text) { }
		public RibbonColorButtonItemBase(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		protected internal ColorNestedControlProperties ColorNestedControlProperties {
			get {
				if(colorNestedControlProperties == null)
					colorNestedControlProperties = new ColorNestedControlProperties();
				return colorNestedControlProperties;
			}
		}
		[DefaultValue(typeof(Color), ""), Localizable(true), NotifyParentProperty(true), AutoFormatDisable]
		public Color Color { 
			get { return GetColorProperty("Color", Color.Empty); }
			set {
				IsAutomaticColorSelected = false;
				SetColorProperty("Color", Color.Empty, value);
			}
		}
		[DefaultValue(ColorTable.DefaultColumnCount), NotifyParentProperty(true), AutoFormatDisable]
		public int ColumnCount {
			get { return ColorNestedControlProperties.ColumnCount; }
			set { ColorNestedControlProperties.ColumnCount = value; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatDisable]
		public ColorEditItemCollection Items {
			get { return ColorNestedControlProperties.Items; }
		}
		protected internal bool IsAutomaticColorSelected {
			get { return GetBoolProperty("IsAutomaticColorSelected", false); }
			set {
				if(value == true)
					Color = AutomaticColorInternal;
				SetBoolProperty("IsAutomaticColorSelected", false, value);
			}
		}
		protected bool EnableCustomColorsInternal {
			get { return ColorNestedControlProperties.EnableCustomColors; }
			set { ColorNestedControlProperties.EnableCustomColors = value; }
		}
		protected bool EnableAutomaticColorItemInternal {
			get { return ColorNestedControlProperties.EnableAutomaticColorItem; }
			set { ColorNestedControlProperties.EnableAutomaticColorItem = value; }
		}
		protected string AutomaticColorItemCaptionInternal {
			get { return ColorNestedControlProperties.AutomaticColorItemCaption; }
			set { ColorNestedControlProperties.AutomaticColorItemCaption = value; }
		}
		protected Color AutomaticColorInternal {
			get { return ColorNestedControlProperties.AutomaticColor; }
			set { ColorNestedControlProperties.AutomaticColor = value; }
		}
		protected string AutomaticColorItemValueInternal {
			get { return ColorNestedControlProperties.AutomaticColorItemValue; }
			set { ColorNestedControlProperties.AutomaticColorItemValue = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string NavigateUrl {
			get { return base.NavigateUrl; }
			set { base.NavigateUrl = value; }
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.ColorButton; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonColorButtonItemBase item = source as RibbonColorButtonItemBase;
			if(item != null) {
				Color = item.Color;
				ColorNestedControlProperties.Assign(item.ColorNestedControlProperties);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ColorNestedControlProperties });
		}
	}
	public class RibbonColorButtonItem : RibbonColorButtonItemBase {
		public RibbonColorButtonItem() { }
		public RibbonColorButtonItem(string name)
			: base(name) { }
		public RibbonColorButtonItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonColorButtonItem(string name, string text)
			: base(name, text) { }
		public RibbonColorButtonItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonColorButtonItemEnableCustomColors"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableCustomColors
		{
			get { return EnableCustomColorsInternal; }
			set { EnableCustomColorsInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonColorButtonItemEnableAutomaticColorItem"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableAutomaticColorItem {
			get { return EnableAutomaticColorItemInternal; }
			set { EnableAutomaticColorItemInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonColorButtonItemAutomaticColorItemCaption"),
#endif
		DefaultValue(StringResources.ColorEdit_AutomaticColor), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string AutomaticColorItemCaption {
			get { return AutomaticColorItemCaptionInternal; }
			set { AutomaticColorItemCaptionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonColorButtonItemAutomaticColor"),
#endif
		DefaultValue(typeof(Color), "Black"), TypeConverter(typeof(WebColorConverter)),
		NotifyParentProperty(true), AutoFormatDisable]
		public Color AutomaticColor {
			get { return AutomaticColorInternal; }
			set { AutomaticColorInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonColorButtonItemAutomaticColorItemValue"),
#endif
		DefaultValue("Auto"), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public string AutomaticColorItemValue {
			get { return AutomaticColorItemValueInternal; }
			set { AutomaticColorItemValueInternal = value; }
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public abstract class RibbonEditItemBase : RibbonItemBase {
		public RibbonEditItemBase() { }
		public RibbonEditItemBase(string name)
			: base(name) { }
		public RibbonEditItemBase(string name, string text)
			: base(name, text) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RibbonItemSize Size { get { return RibbonItemSize.Small; } }
		[DefaultValue(null), NotifyParentProperty(true), AutoFormatDisable, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value {
			get {
				if(!IsDesignMode() && Ribbon != null && Ribbon.EditorItems.ContainsKey(this))
					return Ribbon.EditorItems[this].Value;
				return GetObjectProperty("Value", null);
			}
			set {
				if(!IsDesignMode() && Ribbon != null && Ribbon.EditorItems.ContainsKey(this))
					Ribbon.EditorItems[this].Value = value;
				else
					SetObjectProperty("Value", null, value);
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonEditItemBase item = source as RibbonEditItemBase;
			if(item != null) {
				Value = item.Value;
			}
		}
	}
	public class RibbonTextBoxItem : RibbonEditItemBase {
		RibbonTextBoxProperties properties;
		public RibbonTextBoxItem() { }
		public RibbonTextBoxItem(string name)
			: base(name) { }
		public RibbonTextBoxItem(string name, string text)
			: base(name, text) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonTextBoxItemPropertiesTextBox"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual RibbonTextBoxProperties PropertiesTextBox
		{
			get
			{
				if (properties == null)
					properties = new RibbonTextBoxProperties(this);
				return properties;
			}
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesTextBox;
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.TextBox; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonTextBoxItem item = source as RibbonTextBoxItem;
			if(item != null) {
				PropertiesTextBox.Assign(item.PropertiesTextBox);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesTextBox });
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTextBox" });
		}
	}
	public class RibbonSpinEditItem : RibbonEditItemBase {
		RibbonSpinEditProperties properties;
		public RibbonSpinEditItem() { }
		public RibbonSpinEditItem(string name)
			: base(name) { }
		public RibbonSpinEditItem(string name, string text)
			: base(name, text) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonSpinEditItemPropertiesSpinEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual RibbonSpinEditProperties PropertiesSpinEdit
		{
			get
			{
				if (properties == null)
					properties = new RibbonSpinEditProperties(this);
				return properties;
			}
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesSpinEdit;
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.SpinEdit; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonSpinEditItem item = source as RibbonSpinEditItem;
			if(item != null) {
				PropertiesSpinEdit.Assign(item.PropertiesSpinEdit);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesSpinEdit });
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesSpinEdit" });
		}
	}
	public class RibbonDateEditItem : RibbonEditItemBase, IDateEditIDResolver {
		RibbonDateEditProperties properties;
		public RibbonDateEditItem() { }
		public RibbonDateEditItem(string name)
			: base(name) { }
		public RibbonDateEditItem(string name, string text)
			: base(name, text) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonDateEditItemPropertiesDateEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual RibbonDateEditProperties PropertiesDateEdit
		{
			get
			{
				if (properties == null)
					properties = new RibbonDateEditProperties(this);
				return properties;
			}
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesDateEdit;
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.DateEdit; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonDateEditItem item = source as RibbonDateEditItem;
			if(item != null) {
				PropertiesDateEdit.Assign(item.PropertiesDateEdit);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesDateEdit });
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesDateEdit" });
		}
		string IDateEditIDResolver.GetDateEditIdByDataItemName(string dataItemName) {
			RibbonDateEditItem item = null;
			item = Group.Items.FindByName(dataItemName) as RibbonDateEditItem;
			if(item == null) {
				item = Group.Tab != null ? Group.Tab.Groups.FindItemByName(dataItemName) as RibbonDateEditItem : null;
				if (item == null)
					item = Ribbon.FindItemByName(dataItemName) as RibbonDateEditItem;
			}
			return item != null && item != this ? RibbonHelper.GetDateEditID(item) : "";
		}
		string[] IDateEditIDResolver.GetPossibleDataItemNames() {
			List<string> result = new List<string>();
			foreach (RibbonTab tab in Ribbon.AllTabs)
				foreach (RibbonGroup group in tab.Groups)
					foreach (RibbonItemBase item in group.Items)
						if (item is RibbonDateEditItem && item != this && !string.IsNullOrEmpty(item.Name))
							result.Add(item.Name);
			return result.ToArray();
		}
	}
	public class RibbonComboBoxItem : RibbonEditItemBase {
		RibbonComboBoxProperties properties;
		public RibbonComboBoxItem() { }
		public RibbonComboBoxItem(string name)
			: base(name) { }
		public RibbonComboBoxItem(string name, string text)
			: base(name, text) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonComboBoxItemItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), NotifyParentProperty(true)]
		public virtual ListEditItemCollection Items
		{
			get { return GetComboBoxProperties().Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonComboBoxItemPropertiesComboBox"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual RibbonComboBoxProperties PropertiesComboBox
		{
			get
			{
				if (properties == null)
					properties = new RibbonComboBoxProperties(this);
				return properties;
			}
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesComboBox;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonComboBoxItem item = source as RibbonComboBoxItem;
			if(item != null) {
				PropertiesComboBox.Assign(item.PropertiesComboBox);
				Items.Assign(item.Items);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesComboBox });
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.ComboBox; } }
		protected override IList GetDesignTimeItems() {
			return (IList)this.Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesComboBox", "Items" });
		}
		protected internal virtual Dictionary<int, string> GetHtmlTextItemsDictionary() {
			return null;
		}
		protected internal virtual RibbonComboBoxProperties GetComboBoxProperties() {
			return PropertiesComboBox;
		}
	}
	public class RibbonCheckBoxItem : RibbonEditItemBase {
		RibbonCheckBoxProperties properties;
		public RibbonCheckBoxItem() { }
		public RibbonCheckBoxItem(string name)
			: base(name) { }
		public RibbonCheckBoxItem(string name, string text)
			: base(name, text) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonCheckBoxItemPropertiesCheckBox"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual RibbonCheckBoxProperties PropertiesCheckBox
		{
			get
			{
				if (properties == null)
					properties = new RibbonCheckBoxProperties(this);
				return properties;
			}
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesCheckBox;
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.CheckBox; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonCheckBoxItem item = source as RibbonCheckBoxItem;
			if(item != null) {
				PropertiesCheckBox.Assign(item.PropertiesCheckBox);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesCheckBox });
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesCheckBox" });
		}
	}
	[ControlBuilder(typeof(GalleryItemBuilder))]
	public class RibbonGalleryProperties : RibbonDropDownGalleryProperties {
		private RibbonGalleryGroupCollection groups;
		public RibbonGalleryProperties(IPropertiesOwner owner)
			: base(owner) { }
		[DefaultValue(1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int ColumnCount {
			get { return GetIntProperty("ColumnCount", 1); }
			set { SetIntProperty("ColumnCount", 1, value); }
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool ShowText {
			get { return GetBoolProperty("ShowText", true); }
			set { SetBoolProperty("ShowText", true, value); }
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool AllowSelectItem {
			get { return GetBoolProperty("AllowSelectItem", true); }
			set { SetBoolProperty("AllowSelectItem", true, value); }
		}
		[DefaultValue(ImagePosition.Top), NotifyParentProperty(true), AutoFormatDisable]
		public virtual ImagePosition ImagePosition {
			get { return (ImagePosition)GetEnumProperty("ImagePosition", ImagePosition.Top); }
			set { SetEnumProperty("ImagePosition", ImagePosition.Top, value); }
		}
		[DefaultValue(typeof(Unit), "32"), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit ImageWidth {
			get { return GetUnitProperty("ImageWidth", 32); }
			set {
				CommonUtils.CheckNegativeValue(value.Value, "ImageWidth");
				SetUnitProperty("ImageWidth", 32, value);
			}
		}
		[DefaultValue(typeof(Unit), "32"), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit ImageHeight {
			get { return GetUnitProperty("ImageHeight", 32); }
			set {
				CommonUtils.CheckNegativeValue(value.Value, "ImageHeight");
				SetUnitProperty("ImageHeight", 32, value);
			}
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit MaxTextWidth {
			get { return GetUnitProperty("MaxTextWidth", Unit.Empty); }
			set {
				CommonUtils.CheckNegativeValue(value.Value, "MaxTextWidth");
				SetUnitProperty("MaxTextWidth", Unit.Empty, value);
			}
		}
		[DefaultValue(null), NotifyParentProperty(true), RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(StringToObjectTypeConverter)), AutoFormatDisable]
		public object Value {
			get { return GetObjectProperty("Value", null); }
			set {
				SetObjectProperty("Value", null, value);
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueString {
			get { return ""; }
			set { Value = value; }
		}
		[DefaultValue(typeof(String)), NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get { return (Type)GetObjectProperty("ValueType", typeof(String)); }
			set {
				if(ValueType != value) {
					SetObjectProperty("ValueType", typeof(String), value);
					RefreshValues();
				}
			}
		}
		protected internal RibbonGalleryGroupCollection Groups {
			get {
				if(groups == null)
					groups = new RibbonGalleryGroupCollection((RibbonItemBase)Owner, this);
				return groups;
			}
			protected set {
				groups = value;
			}
		}
		protected void RefreshValues() {
			foreach(RibbonGalleryGroup group in Groups) {
				foreach(RibbonGalleryItem item in group.Items)
					if(item.Value != null && item.Value.GetType() != ValueType)
						item.Value = CommonUtils.ConvertToType(item.Value, ValueType, false);
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			RibbonGalleryProperties item = source as RibbonGalleryProperties;
			if(item != null) {
				Groups.Assign(item.Groups);
				ShowText = item.ShowText;
				ImagePosition = item.ImagePosition;
				Value = item.Value;
				ValueType = item.ValueType;
				ImageWidth = item.ImageWidth;
				ImageHeight = item.ImageHeight;
				MaxTextWidth = item.MaxTextWidth;
				ColumnCount = item.ColumnCount;
				AllowSelectItem = item.AllowSelectItem;
			}
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Groups });
		}
	}
	public class RibbonDropDownGalleryProperties : PropertiesBase, IPropertiesOwner {
		public RibbonDropDownGalleryProperties(IPropertiesOwner owner)
			: base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), NotifyParentProperty(true), AutoFormatDisable]
		private bool AllowFilter {
			get { return GetBoolProperty("AllowFilter", true); }
			set { SetBoolProperty("AllowFilter", true, value); }
		}
		[DefaultValue(1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int RowCount {
			get { return GetIntProperty("RowCount", 1); }
			set { SetIntProperty("RowCount", 1, value); }
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool ShowGroupText {
			get { return GetBoolProperty("ShowGroupText", true); }
			set { SetBoolProperty("ShowGroupText", true, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), NotifyParentProperty(true), AutoFormatDisable]
		private string FilterCaption {
			get { return GetStringProperty("FilterCaption", string.Empty); }
			set { SetStringProperty("FilterCaption", string.Empty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), NotifyParentProperty(true), AutoFormatDisable]
		private object ContextButtons {
			get { return GetObjectProperty("ContextButtons", null); }
			set { SetObjectProperty("ContextButtons", null, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			RibbonDropDownGalleryProperties item = source as RibbonDropDownGalleryProperties;
			if(item != null) {
				ShowGroupText = item.ShowGroupText;
				AllowFilter = item.AllowFilter;
				FilterCaption = item.FilterCaption;
				RowCount = item.RowCount;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	[ControlBuilder(typeof(GalleryItemBuilder))]
	public class RibbonGalleryBarItem : RibbonItemBase {
		private RibbonGalleryProperties propertiesBarGallery;
		private RibbonDropDownGalleryProperties propertiesDropDownGallery;
		private RibbonItemOneLineModeSettings oneLineModeSettings;
		public RibbonGalleryBarItem() { }
		public RibbonGalleryBarItem(string name)
			: base(name) { }
		public RibbonGalleryBarItem(string name, params RibbonGalleryGroup[] groups)
			: base(name) {
			PropertiesBarGallery.Groups.AddRange(groups);
		}
		[DefaultValue(RibbonItemSize.Large), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable]
		public override RibbonItemSize Size {
			get {
				return RibbonItemSize.Large;
			}
		}
		[DefaultValue(null), NotifyParentProperty(true), RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(StringToObjectTypeConverter)), AutoFormatDisable]
		public object Value {
			get { 
				return PropertiesBarGallery.Value; }
			set {
				PropertiesBarGallery.Value = value;
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never), Localizable(false)]
		public string ValueString {
			get { return ""; }
			set { Value = value; }
		}
		[DefaultValue(typeof(String)), NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false)]
		public Type ValueType {
			get {
				return PropertiesBarGallery.ValueType;
			}
			set {
				PropertiesBarGallery.ValueType = value;
			}
		}
		public List<RibbonGalleryItem> GetAllItems() {
			var result = new List<RibbonGalleryItem>();
			foreach(RibbonGalleryGroup group in Groups) {
				foreach(RibbonGalleryItem item in group.Items)
					result.Add(item);
			}
			return result;
		}
		[Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable]
		public virtual RibbonDropDownGalleryProperties PropertiesDropDownGallery {
			get {
				if(propertiesDropDownGallery == null)
					propertiesDropDownGallery = new RibbonDropDownGalleryProperties(this);
				return propertiesDropDownGallery;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual RibbonGalleryGroupCollection Groups {
			get {
				return PropertiesBarGallery.Groups;
			}
		}
		[DefaultValue(1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int MaxColumnCount {
			get { return GetIntProperty("MaxColumnCount", 1); }
			set { SetIntProperty("MaxColumnCount", 1, value); }
		}
		[DefaultValue(1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int MinColumnCount {
			get { return GetIntProperty("MinColumnCount", 1); }
			set { SetIntProperty("MinColumnCount", 1, value); }
		}
		[DefaultValue(1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int RowCount {
			get { return PropertiesBarGallery.RowCount; }
			set { PropertiesBarGallery.RowCount = value; }
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool ShowText {
			get { return PropertiesBarGallery.ShowText; }
			set { PropertiesBarGallery.ShowText = value; }
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool AllowSelectItem {
			get { return PropertiesBarGallery.AllowSelectItem; }
			set { PropertiesBarGallery.AllowSelectItem = value; }
		}
		[DefaultValue(ImagePosition.Top), NotifyParentProperty(true), AutoFormatDisable]
		public virtual ImagePosition ImagePosition {
			get { return PropertiesBarGallery.ImagePosition; }
			set { PropertiesBarGallery.ImagePosition = value; }
		}
		[DefaultValue(typeof(Unit), "32"), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit ImageWidth {
			get { return PropertiesBarGallery.ImageWidth; }
			set { PropertiesBarGallery.ImageWidth = value; }
		}
		[DefaultValue(typeof(Unit), "32"), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit ImageHeight {
			get { return PropertiesBarGallery.ImageHeight; }
			set { PropertiesBarGallery.ImageHeight = value; }
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit MaxTextWidth {
			get { return PropertiesBarGallery.MaxTextWidth; }
			set { PropertiesBarGallery.MaxTextWidth = value; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatDisable]
		public virtual RibbonItemOneLineModeSettings OneLineModeSettings {
			get {
				if(oneLineModeSettings == null)
					oneLineModeSettings = new RibbonItemOneLineModeSettings(this);
				return oneLineModeSettings;
			}
		}
		[Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable]
		protected RibbonGalleryProperties PropertiesBarGallery {
			get {
				if(propertiesBarGallery == null) {
					propertiesBarGallery = new RibbonGalleryProperties(this);
					propertiesBarGallery.ShowGroupText = false;
				}
				return propertiesBarGallery;
			}
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.GalleryBar; } }
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesBarGallery, PropertiesDropDownGallery, OneLineModeSettings });
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Groups;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Groups" });
		}
		protected internal override bool IsButtonMode() {
			return Ribbon != null && Ribbon.OneLineMode;
		}
		protected internal override ItemImagePropertiesBase GetSmallImage() {
			return OneLineModeSettings.Image;
		}
		protected internal override string GetText() {
			return OneLineModeSettings.Text;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonGalleryBarItem item = source as RibbonGalleryBarItem;
			if(item != null) {
				PropertiesDropDownGallery.Assign(item.PropertiesDropDownGallery);
				PropertiesBarGallery.Assign(item.PropertiesBarGallery);
				OneLineModeSettings.Assign(item.OneLineModeSettings);
				MaxColumnCount = item.MaxColumnCount;
				MinColumnCount = item.MinColumnCount;
				RowCount = item.RowCount;
				ShowText = item.ShowText;
				ImagePosition = item.ImagePosition;
				ImageWidth = item.ImageWidth;
				ImageHeight = item.ImageHeight;
				MaxTextWidth = item.MaxTextWidth;
				AllowSelectItem = item.AllowSelectItem;
			}
		}
	}
	public class RibbonGalleryDropDownItem : RibbonButtonItem {
		RibbonGalleryProperties propertiesDropDownGallery;
		public RibbonGalleryDropDownItem() { }
		public RibbonGalleryDropDownItem(string name)
			: base(name) { }
		public RibbonGalleryDropDownItem(string name, RibbonItemSize size)
			: base(name, size) { }
		public RibbonGalleryDropDownItem(string name, string text)
			: base(name, text) { }
		public RibbonGalleryDropDownItem(string name, string text, RibbonItemSize size)
			: base(name, text, size) { }
		public RibbonGalleryDropDownItem(string name, string text, params RibbonGalleryGroup[] groups)
			: base(name, text) {
			PropertiesDropDownGallery.Groups.AddRange(groups);
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual RibbonGalleryGroupCollection Groups {
			get {
				return PropertiesDropDownGallery.Groups;
			}
		}
		[Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable]
		public virtual RibbonGalleryProperties PropertiesDropDownGallery {
			get {
				if(propertiesDropDownGallery == null)
					propertiesDropDownGallery = new RibbonGalleryProperties(this);
				return propertiesDropDownGallery;
			}
		}
		public List<RibbonGalleryItem> GetAllItems() {
			var result = new List<RibbonGalleryItem>();
			foreach(RibbonGalleryGroup group in Groups) {
				foreach(RibbonGalleryItem item in group.Items)
					result.Add(item);
			}
			return result;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonGalleryDropDownItem item = source as RibbonGalleryDropDownItem;
			if(item != null) {
				PropertiesDropDownGallery.Assign(item.PropertiesDropDownGallery);
			}
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Groups;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Groups" });
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesDropDownGallery });
		}
		protected internal override RibbonItemType ItemType { get { return RibbonItemType.GalleryDropDown; } }
	}
	public class RibbonGalleryItemCollection : Collection<RibbonGalleryItem> {
		public RibbonGalleryItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public RibbonGalleryGroup Group { get { return Owner as RibbonGalleryGroup; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			RibbonGalleryItem item = value as RibbonGalleryItem;
			if(item != null && Group != null && Group.Gallery != null) {
				Type type = Group.Gallery.ValueType;
				var convertedValue = CommonUtils.GetConvertedArgumentValue(item.Value, type, "Item[]");
				item.Value = convertedValue;
			}
		}
	}
	[ControlBuilder(typeof(GalleryItemBuilder))]
	public class RibbonGalleryItem : CollectionItem {
		const string DefaultText = "GalleryItem";
		private RibbonItemImageProperties image;
		public RibbonGalleryItem() { }
		public RibbonGalleryItem(string text) {
			Text = text;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		NotifyParentProperty(true), AutoFormatDisable]
		public RibbonGalleryGroup Group {
			get {
				if(Collection is RibbonGalleryItemCollection)
					return ((RibbonGalleryItemCollection)Collection).Group;
				return null;
			}
		}
		[Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public RibbonItemImageProperties Image {
			get {
				if(image == null)
					image = new RibbonItemImageProperties(this);
				return image;
			}
		}
		[DefaultValue(""), Localizable(true), NotifyParentProperty(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", string.Empty); }
			set { SetStringProperty("Text", string.Empty, value); }
		}
		[DefaultValue(""), Localizable(true), NotifyParentProperty(true), AutoFormatDisable]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", string.Empty); }
			set { SetStringProperty("ToolTip", string.Empty, value); }
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				if(value == Visible)
					return;
				SetBoolProperty("Visible", true, value);
				LayoutChanged();
			}
		}
		[DefaultValue(null), NotifyParentProperty(true), RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(StringToObjectTypeConverter))]
		public object Value {
			get { return GetObjectProperty("Value", null); }
			set {
				if(Group != null && Group.Gallery != null)
					value = CommonUtils.GetConvertedArgumentValue(value, Group.Gallery.ValueType, "Value");
				SetObjectProperty("Value", null, value);
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueString {
			get { return ""; }
			set { Value = value; }
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Image });
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			RibbonGalleryItem item = source as RibbonGalleryItem;
			if(item != null) {
				Text = item.Text;
				Visible = item.Visible;
				ToolTip = item.ToolTip;
				Value = item.Value;
				Image.Assign(item.Image);
			}
		}
	}
	public class RibbonGalleryGroupCollection : Collection<RibbonGalleryGroup> {
		private RibbonGalleryProperties gallery;
		public RibbonGalleryGroupCollection(IWebControlObject owner, RibbonGalleryProperties gallery)
			: base(owner) {
				Gallery = gallery;
		}
		public RibbonGalleryGroup Add(string text) {
			RibbonGalleryGroup group = new RibbonGalleryGroup(text);
			Add(group);
			return group;
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
		protected internal RibbonGalleryProperties Gallery {
			get { return gallery; }
			set { gallery = value; }
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			RibbonGalleryGroup group = value as RibbonGalleryGroup;
			if(group != null) {
				foreach(RibbonGalleryItem item in group.Items)
					item.Value = item.Value;
			}
		}
	}
	public class RibbonGalleryGroup : CollectionItem {
		const string DefaultText = "Group";
		RibbonGalleryItemCollection items;
		public RibbonGalleryGroup() { }
		public RibbonGalleryGroup(string text) {
			Text = text;
		}
		public RibbonGalleryGroup(string text, params RibbonGalleryItem[] items) : this(text) {
			Items.AddRange(items);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public RibbonItemBase Owner {
			get {
				if(Collection is RibbonGalleryGroupCollection)
					return ((RibbonItemBase)((RibbonGalleryGroupCollection)Collection).Owner);
				return null;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), NotifyParentProperty(true)]
		public RibbonGalleryItemCollection Items {
			get {
				if(items == null)
					items = new RibbonGalleryItemCollection(this);
				return items;
			}
		}
		[DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set { SetStringProperty("Text", DefaultText, value); }
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected override IList GetDesignTimeItems() {
			return (IList)this.Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Items" });
		}
		protected internal RibbonGalleryProperties Gallery {
			get {
				if(Collection is RibbonGalleryGroupCollection)
					return ((RibbonGalleryGroupCollection)Collection).Gallery;
				return null;
			}
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items });
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var group = source as RibbonGalleryGroup;
			if(group != null) {
				Text = group.Text;
				Items.Assign(group.Items);
			}
		}
	}
	public class RibbonItemOneLineModeSettings : PropertiesBase, IPropertiesOwner {
		RibbonItemImageProperties image;
		public RibbonItemOneLineModeSettings(IPropertiesOwner owner)
			: base(owner) { }
		[DefaultValue(""), Localizable(true), NotifyParentProperty(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", string.Empty); }
			set { SetStringProperty("Text", string.Empty, value); }
		}
		[Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual RibbonItemImageProperties Image {
			get {
				if(image == null)
					image = new RibbonItemImageProperties(this);
				return image;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			RibbonItemOneLineModeSettings item = source as RibbonItemOneLineModeSettings;
			if(item != null) {
				Image.Assign(item.Image);
				Text = item.Text;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class RibbonGroupOneLineModeSettings : PropertiesBase, IPropertiesOwner {
		RibbonItemImageProperties image;
		public RibbonGroupOneLineModeSettings(IPropertiesOwner owner)
			: base(owner) { }
		[Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual RibbonItemImageProperties Image {
			get {
				if(image == null)
					image = new RibbonItemImageProperties(this);
				return image;
			}
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowExpandButtonText {
			get { return GetBoolProperty("ShowExpandButtonText", true); }
			set { SetBoolProperty("ShowExpandButtonText", true, value); }
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Image });
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			RibbonGroupOneLineModeSettings item = source as RibbonGroupOneLineModeSettings;
			if(item != null) {
				ShowExpandButtonText = item.ShowExpandButtonText;
				Image.Assign(item.Image);
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RibbonDropDownButtonCollection : HierarchicalCollection<RibbonDropDownButtonItem> {
		public RibbonDropDownButtonCollection(IWebControlObject owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonDropDownButtonItem Parent { get { return Owner as RibbonDropDownButtonItem; } }
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RibbonTabCollection : Collection<RibbonTab> {
		bool isContext;
		public RibbonTabCollection(IWebControlObject owner, bool isContext = false)
			: base(owner) {
			this.isContext = isContext;
		}
		public RibbonTab Add(string text) {
			RibbonTab tab = new RibbonTab(text);
			Add(tab);
			return tab;
		}
		public RibbonTab FindByName(string name) {
			return this.FirstOrDefault(i => i.Name == name);
		}
		protected internal RibbonItemBase FindItemByName(string name) {
			foreach (RibbonTab tab in this) {
				RibbonItemBase result = tab.Groups.FindItemByName(name);
				if (result != null)
					return result;
			}
			return null;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRibbon Ribbon {
			get {
				return Owner == null || Owner is ASPxRibbon ? Owner as ASPxRibbon : ((RibbonContextTabCategory)Owner).Ribbon;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonContextTabCategory ContextTabCategory {
			get {
				return Owner is RibbonContextTabCategory ? ((RibbonContextTabCategory)Owner) : null;
			}
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
		protected override void OnBeforeAdd(CollectionItem item) {
			var tab = (RibbonTab)item;
			if(isContext)
				tab.ClientVisibleInternal = false;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RibbonContextTabCategoryCollection : Collection<RibbonContextTabCategory> {
		public RibbonContextTabCategoryCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonContextTabCategory Add(string name) {
			var tabGroup = new RibbonContextTabCategory(name);
			Add(tabGroup);
			return tabGroup;
		}
		public RibbonContextTabCategory FindByName(string name) {
			return this.FirstOrDefault(i => i.Name == name);
		}
		protected internal RibbonItemBase FindItemByName(string name) {
			foreach(RibbonContextTabCategory tabGroup in this) {
				RibbonItemBase result = tabGroup.Tabs.FindItemByName(name);
				if(result != null)
					return result;
			}
			return null;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxRibbon Ribbon { get { return Owner as ASPxRibbon; } }
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RibbonGroupCollection : Collection<RibbonGroup> {
		public RibbonGroupCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonGroup Add() {
			RibbonGroup group = new RibbonGroup();
			Add(group);
			return group;
		}
		public RibbonGroup Add(string text) {
			RibbonGroup group = new RibbonGroup(text);
			Add(group);
			return group;
		}
		public RibbonGroup FindByName(string name) {
			return this.FirstOrDefault(i => i.Name == name);
		}
		protected internal RibbonItemBase FindItemByName(string name) {
			foreach (RibbonGroup group in this) {
				RibbonItemBase result = group.Items.FindByName(name);
				if (result != null)
					return result;
			}
			return null;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonTab Tab { get { return Owner as RibbonTab; } }
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class RibbonItemCollection : Collection<RibbonItemBase> {
		public RibbonItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RibbonGroup Group { get { return Owner as RibbonGroup; } }
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
		public RibbonItemBase FindByName(string name) {
			return this.FirstOrDefault(i => i.Name == name);
		}
	}
	public enum RibbonItemSize {
		Large,
		Small
	}
	public class RibbonComboBoxProperties : ComboBoxProperties {
		public RibbonComboBoxProperties(IPropertiesOwner owner)
			:base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new ListEditItemCollection Items {
			get { return base.Items; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string ClientInstanceName {
			get { return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new DefaultBoolean EnableSynchronization {
			get { return base.EnableSynchronization; }
			set { base.EnableSynchronization = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorDecorationStyle InvalidStyle {
			get { return base.InvalidStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new ValidationSettings ValidationSettings {
			get { return base.ValidationSettings; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new DefaultBoolean RenderIFrameForPopupElements {
			get { return base.RenderIFrameForPopupElements; }
			set { base.RenderIFrameForPopupElements = value; }
		}
		[Browsable(false), Themeable(false), AutoFormatDisable, DefaultValue(typeof(string)),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), NotifyParentProperty(true), EditorBrowsable(EditorBrowsableState.Never)]
		public new Type ValueType {
			get { return base.ValueType; }
			set { base.ValueType = value; }
		}
		[DefaultValue(typeof(Unit), ""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ListEditItemsRequestedByFilterConditionEventHandler ItemsRequestedByFilterCondition {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ListEditItemRequestedByValueEventHandler ItemRequestedByValue {
			add { }
			remove { }
		}
	}
	public class RibbonCheckBoxProperties : CheckBoxProperties {
		public RibbonCheckBoxProperties(IPropertiesOwner owner)
			:base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string ClientInstanceName {
			get { return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
	}
	public class RibbonTextBoxProperties : TextBoxProperties {
		public RibbonTextBoxProperties(IPropertiesOwner owner)
			: base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string ClientInstanceName {
			get { return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
	}
	public class RibbonSpinEditProperties : SpinEditProperties {
		public RibbonSpinEditProperties(IPropertiesOwner owner)
			: base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string ClientInstanceName {
			get { return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
	}
	public class RibbonDateEditProperties : DateEditProperties {
		public RibbonDateEditProperties(IPropertiesOwner owner)
			: base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string ClientInstanceName {
			get { return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
	}
}
namespace DevExpress.Web.Internal {
	public class GalleryItemBuilder : ListEditItemBuilder {
	}
}
