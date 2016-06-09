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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon {
	public enum NodeType { Category, Page, Group, Item }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonCustomizationFormOptions : RibbonOptionsBase {
		public RibbonCustomizationFormOptions(RibbonControl ribbon) : base(ribbon) {
			this.FormIcon = null;
			this.AllowEditBarItemPopups = false;
			this.AllowLinkCustomization = false;
		}
		[Category(CategoryName.Appearance),  DefaultValue(null)]
		public virtual Icon FormIcon {
			get;
			set;
		}
		[Category(CategoryName.Appearance),  DefaultValue(false), XtraSerializableProperty]
		public virtual bool AllowEditBarItemPopups {
			get;
			set;
		}
		[Category(CategoryName.Behavior),  DefaultValue(false), XtraSerializableProperty]
		public virtual bool AllowLinkCustomization {
			get;
			set;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonOptionsBase : BaseOptions {
		public RibbonOptionsBase(RibbonControl ribbon) {
			Ribbon = ribbon;
		}
		[Browsable(false)]
		public RibbonControl Ribbon { get; private set; }
		protected virtual void OnPropertiesChanged(string propName) {
			Ribbon.OnPropertiesChanged(this, propName);
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonOptionsTouch : RibbonOptionsBase {
		public RibbonOptionsTouch(RibbonControl ribbon) : base(ribbon) { }
		DefaultBoolean touchUI = DefaultBoolean.Default;
		Image touchMouseModeSelectorGlyph;
		Image mouseModeGlyph;
		Image touchModeGlyph;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty]
		public DefaultBoolean TouchUI {
			get { return touchUI; }
			set {
				if(TouchUI == value)
					return;
				touchUI = value;
				OnPropertiesChanged("TouchUI");
			}
		}
		protected internal bool GetTouchUI() { return TouchUI == DefaultBoolean.True ? true: false; }
		bool showTouchUISelectorInQAT = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowTouchUISelectorInQAT {
			get { return showTouchUISelectorInQAT; }
			set {
				if(ShowTouchUISelectorInQAT == value)
					return;
				showTouchUISelectorInQAT = value;
				OnPropertiesChanged("ShowTouchUISelectorInQAT");
			}
		}
		bool showTouchUISelectorVisibilityItemInQATMenu = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowTouchUISelectorVisibilityItemInQATMenu {
			get { return showTouchUISelectorVisibilityItemInQATMenu; }
			set {
				if(ShowTouchUISelectorVisibilityItemInQATMenu == value)
					return;
				showTouchUISelectorVisibilityItemInQATMenu = value;
				OnPropertiesChanged("ShowTouchUISelectorVisibilityItemInQATMenu");
			}
		}
		bool affectOnlyRibbon = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool AffectOnlyRibbon {
			get { return affectOnlyRibbon; }
			set {
				if(AffectOnlyRibbon == value)
					return;
				affectOnlyRibbon = value;
				OnPropertiesChanged("AffectOnlyRibbon");
			}
		}
		public override void Assign(BaseOptions options) {
			RibbonOptionsTouch touch = options as RibbonOptionsTouch;
			if(touch == null)
				return;
			this.touchUI = touch.TouchUI;
			this.showTouchUISelectorInQAT = touch.ShowTouchUISelectorInQAT;
			this.showTouchUISelectorVisibilityItemInQATMenu = touch.ShowTouchUISelectorVisibilityItemInQATMenu;
			this.touchMouseModeSelectorGlyph = touch.TouchMouseModeSelectorGlyph;
			this.mouseModeGlyph = touch.mouseModeGlyph;
			this.touchModeGlyph = touch.touchModeGlyph;
			this.affectOnlyRibbon = touch.AffectOnlyRibbon;
		}
		[DefaultValue(null)]
		public Image TouchMouseModeSelectorGlyph {
			get { return touchMouseModeSelectorGlyph; }
			set {
				if(touchMouseModeSelectorGlyph == value)
					return;
				touchMouseModeSelectorGlyph = value;
				OnPropertiesChanged("TouchUISelectorIcon");
			}
		}
		[DefaultValue(null)]
		public Image MouseModeGlyph {
			get { return mouseModeGlyph; }
			set {
				if(mouseModeGlyph == value)
					return;
				mouseModeGlyph = value;
				OnPropertiesChanged("TouchUISelectorIcon");
			}
		}
		[DefaultValue(null)]
		public Image TouchModeGlyph {
			get { return touchModeGlyph; }
			set {
				if(touchModeGlyph == value)
					return;
				touchModeGlyph = value;
				OnPropertiesChanged("TouchUISelectorIcon");
			}
		}
	}
	public class RibbonCustomizationModel {
		public RibbonCustomizationModel(RibbonControl control) {
			this.RibbonControl = control;
			this.Categories = new RibbonPageCategoryInfoCollection();
			this.CommonSettings = new RibbonCommonSettings();
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public RibbonPageCategoryInfoCollection Categories { get; private set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RibbonCommonSettings CommonSettings { get; set; }
		public bool IsModelValid(RibbonControl ribbon) {
			if(CommonSettings.HashCode == RibbonCommonSettings.DefaultHashCode)
				return true;
			RibbonStateInfo si = RibbonSourceStateInfo.Instance[ribbon];
			return si.ItemsCount == CommonSettings.HashCode;
		}
		protected RibbonControl RibbonControl { get; private set; }
		#region Serialization
		public object XtraFindCategoriesItem(XtraItemEventArgs e) {
			return null;
		}
		public object XtraCreateCategoriesItem(XtraItemEventArgs e) {
			RibbonPageCategory category;
			IdInfoContainer idInfo = new IdInfoContainer();
			IdInfoContainer xmlIdInfo = RibbonCustomizationSerializationHelper.InfoFromItemEventArgs(e);
			if(RibbonCustomizationSerializationHelper.IsCustomValueFromItemEventArgs(e)) {
				idInfo.IdInfo = IdGenerator.Instance.Generate(RibbonControl);
				idInfo.SourceIdInfo = IdInfo.Empty;
				category = new RibbonPageCategory() { Text = RibbonCustomizationSerializationHelper.TextValueFromItemEventArgs(e) };
			}
			else {
				idInfo.Assign(xmlIdInfo);
				category = RibbonCustomizationSerializationHelper.GetEntry<RibbonPageCategory>(RibbonControl, xmlIdInfo, e);
			}
			if(category == null)
				return null;
			RibbonPageCategoryInfo categoryInfo = new RibbonPageCategoryInfo(category, idInfo.IdInfo, idInfo.SourceIdInfo, RibbonControl);
			categoryInfo.IsVisible = RibbonCustomizationSerializationHelper.IsVisibleValueFromItemEventArgs(e);
			Categories.Add(categoryInfo);
			return categoryInfo;
		}
		#endregion
	}
	public class RibbonCommonSettings {
		public RibbonCommonSettings() {
			this.Minimized = false;
			this.ToolbarLocation = RibbonQuickAccessToolbarLocation.Default;
			this.IsReady = false;
			this.HashCode = DefaultHashCode;
		}
		[XtraSerializableProperty]
		public bool Minimized { get; set; }
		[XtraSerializableProperty]
		public RibbonQuickAccessToolbarLocation ToolbarLocation { get; set; }
		[XtraSerializableProperty]
		public bool IsReady { get; set; }
		[XtraSerializableProperty]
		public int HashCode { get; set; }
		public static readonly int DefaultHashCode = 0;
	}
	public abstract class CustomizationItemInfoBase : ICloneable, IIdInfoContainer {
		public CustomizationItemInfoBase(IdInfo idInfo, IdInfo sourceIdInfo, RibbonControl control) {
			this.IdInfo = idInfo;
			this.SourceIdInfo = sourceIdInfo;
			this.RibbonControl = control;
			this.IsVisible = true;
		}
		[XtraSerializableProperty]
		public bool IsCustom { get { return IsCustomCore(); } }
		string aliasCore = string.Empty;
		[XtraSerializableProperty]
		public string Alias {
			get {
				if(string.IsNullOrEmpty(this.aliasCore))
					return Text;
				return this.aliasCore;
			}
			set { this.aliasCore = value; }
		}
		[XtraSerializableProperty]
		public string Name {
			get { return GetNameCore(); }
			set { SetNameCore(value); }
		}
		protected RibbonControl RibbonControl { get; private set; }
		protected virtual bool IsCustomCore() {
			return !CustomizationItemStateResolver.Contains(RibbonControl, this);
		}
		#region IIdInfoContainer
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IdInfo IdInfo { get; protected set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IdInfo SourceIdInfo { get; protected set; }
		#endregion
		#region Abstract Members
		public abstract void ApplyAlias();
		public abstract object Clone();
		public abstract NodeType ItemType { get; }
		public abstract object InternalRibbonItem { get; }
		protected abstract string GetNameCore();
		protected abstract void SetNameCore(string value);
		[XtraSerializableProperty]
		public abstract string Text { get; }
		[XtraSerializableProperty]
		public abstract bool IsVisible { get; set; }
		#endregion
	}
	class CustomizationRibbonPageCategoryCollection : RibbonPageCategoryCollection {
		public CustomizationRibbonPageCategoryCollection(RibbonControl ribbon) : base(ribbon) { }
		protected override void OnInsert(int index, object value) {
		}
		protected override bool ShouldRaiseRibbonEvents { get { return false; } }
	}
	public class RibbonPageCategoryInfo : CustomizationItemInfoBase {
		public RibbonPageCategoryInfo(RibbonPageCategory category, IdInfo idInfo, IdInfo sourceIdInfo, RibbonControl control) : base(idInfo, sourceIdInfo, control) {
			this.Category = category;
			this.Pages = new RibbonPageInfoCollection();
		}
		public override NodeType ItemType {
			get { return NodeType.Category; }
		}
		public override object InternalRibbonItem {
			get { return Category; }
		}
		public override string Text {
			get { return Category.Text; }
		}
		public override void ApplyAlias() {
			if(!IsCustom) {
				Category.Text = Alias;
				return;
			}
			Category.Text = CustomizationHelperBase.ExcludeInternalSuffix(Alias);
		}
		public override bool IsVisible {
			get;
			set;
		}
		public override object Clone() {
			return new RibbonPageCategoryInfo(Category.Clone() as RibbonPageCategory, IdGenerator.Instance.Generate(RibbonControl), IdInfo.Empty, RibbonControl);
		}
		public RibbonPageCategory Category { get; internal set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public RibbonPageInfoCollection Pages { get; private set; }
		#region Serialization
		public object XtraFindPagesItem(XtraItemEventArgs e) {
			return null;
		}
		public object XtraCreatePagesItem(XtraItemEventArgs e) {
			RibbonPage page;
			IdInfoContainer idInfo = new IdInfoContainer();
			IdInfoContainer xmlIdInfo = RibbonCustomizationSerializationHelper.InfoFromItemEventArgs(e);
			if(RibbonCustomizationSerializationHelper.IsCustomValueFromItemEventArgs(e)) {
				idInfo.IdInfo = IdGenerator.Instance.Generate(RibbonControl);
				idInfo.SourceIdInfo = IdInfo.Empty;
				page = new RibbonPage() { Text = RibbonCustomizationSerializationHelper.TextValueFromItemEventArgs(e) };
			}
			else {
				idInfo.Assign(xmlIdInfo);
				page = RibbonCustomizationSerializationHelper.GetEntry<RibbonPage>(RibbonControl, xmlIdInfo, e);
			}
			if(page == null)
				return null;
			RibbonPageInfo pageInfo = new RibbonPageInfo(page, idInfo.IdInfo, idInfo.SourceIdInfo, RibbonControl);
			pageInfo.IsVisible = RibbonCustomizationSerializationHelper.IsVisibleValueFromItemEventArgs(e);
			Pages.Add(pageInfo);
			return pageInfo;
		}
		#endregion
		protected override string GetNameCore() {
			return Category != null ? Category.Name : string.Empty;
		}
		protected override void SetNameCore(string value) {
			if(Category != null) Category.Name = value;
		}
	}
	public class RibbonPageCategoryInfoCollection : Collection<RibbonPageCategoryInfo> {
		public void Assign(RibbonPageCategoryInfoCollection categories) {
			Clear();
			foreach(RibbonPageCategoryInfo item in categories) {
				Add(item);
			}
		}
		public bool ContainsCategory(RibbonPageCategory pageCategory) {
			return GetCategoryInfo(pageCategory) != null;
		}
		public RibbonPageCategoryInfo GetCategoryInfo(RibbonPageCategory pageCategory) {
			foreach(RibbonPageCategoryInfo categoryInfo in this) {
				if(object.ReferenceEquals(pageCategory, categoryInfo.Category))
					return categoryInfo;
			}
			return null;
		}
	}
	public class RibbonPageInfo : CustomizationItemInfoBase {
		public RibbonPageInfo(RibbonPage page, IdInfo idInfo, IdInfo sourceIdInfo, RibbonControl control) : base(idInfo, sourceIdInfo, control) {
			this.Page = page;
			this.Groups = new RibbonGroupInfoCollection();
		}
		public override NodeType ItemType {
			get { return NodeType.Page; }
		}
		public override object InternalRibbonItem {
			get { return Page; }
		}
		public override string Text {
			get { return Page.Text; }
		}
		public override void ApplyAlias() {
			if(!IsCustom) {
				Page.Text = Alias;
				return;
			}
			Page.Text = CustomizationHelperBase.ExcludeInternalSuffix(Alias);
		}
		public override bool IsVisible {
			get;
			set;
		}
		public override object Clone() {
			return new RibbonPageInfo(Page.Clone() as RibbonPage, IdGenerator.Instance.Generate(RibbonControl), IdInfo.Empty, RibbonControl);
		}
		public RibbonPage Page { get; private set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public RibbonGroupInfoCollection Groups { get; private set; }
		#region Serialization
		public object XtraFindGroupsItem(XtraItemEventArgs e) {
			return null;
		}
		public object XtraCreateGroupsItem(XtraItemEventArgs e) {
			RibbonPageGroup group;
			IdInfoContainer idInfo = new IdInfoContainer();
			IdInfoContainer xmlIdInfo = RibbonCustomizationSerializationHelper.InfoFromItemEventArgs(e);
			if(RibbonCustomizationSerializationHelper.IsCustomValueFromItemEventArgs(e)) {
				idInfo.IdInfo = IdGenerator.Instance.Generate(RibbonControl);
				idInfo.SourceIdInfo = IdInfo.Empty;
				group = new RibbonPageGroup() { Text = RibbonCustomizationSerializationHelper.TextValueFromItemEventArgs(e) };
			}
			else {
				idInfo.Assign(xmlIdInfo);
				group = RibbonCustomizationSerializationHelper.GetEntry<RibbonPageGroup>(RibbonControl, xmlIdInfo, e);
			}
			if(group == null)
				return null;
			RibbonGroupInfo groupInfo = new RibbonGroupInfo(group, idInfo.IdInfo, idInfo.SourceIdInfo, RibbonControl);
			Groups.Add(groupInfo);
			return groupInfo;
		}
		#endregion
		protected override string GetNameCore() {
			return Page != null ? Page.Name : string.Empty;
		}
		protected override void SetNameCore(string value) {
			if(Page != null) Page.Name = value;
		}
	}
	public class RibbonPageInfoCollection : Collection<RibbonPageInfo> {
		public void Assign(RibbonPageInfoCollection pages) {
			Clear();
			foreach(RibbonPageInfo pageInfo in pages) {
				Add(pageInfo);
			}
		}
		public bool ContainsPage(RibbonPage page) {
			foreach(RibbonPageInfo pageInfo in this) {
				if(object.ReferenceEquals(pageInfo.Page, page))
					return true;
			}
			return false;
		}
		public int IndexOfPage(RibbonPage page) {
			for (int i = 0; i < Count; i++) {
				RibbonPageInfo pageInfo = this[i];
				if(object.ReferenceEquals(pageInfo.Page, page))
					return i;
			}
			return -1;
		}
	}
	public class RibbonGroupInfo : CustomizationItemInfoBase {
		public RibbonGroupInfo(RibbonPageGroup group, IdInfo idInfo, IdInfo sourceIdInfo, RibbonControl control) : base(idInfo, sourceIdInfo, control) {
			this.Group = group;
			this.Items = new RibbonItemLinkInfoCollection();
		}
		public override NodeType ItemType {
			get { return NodeType.Group; }
		}
		public override object InternalRibbonItem {
			get { return Group; }
		}
		public override string Text {
			get { return Group.Text; }
		}
		public override void ApplyAlias() {
			if(!IsCustom) {
				Group.Text = Alias;
				return;
			}
			Group.Text = CustomizationHelperBase.ExcludeInternalSuffix(Alias);
		}
		public override bool IsVisible {
			get { return true; }
			set { }
		}
		public override object Clone() {
			return new RibbonGroupInfo(Group.Clone() as RibbonPageGroup, IdGenerator.Instance.Generate(RibbonControl), IdInfo.Empty, RibbonControl);
		}
		public RibbonPageGroup Group { get; private set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public RibbonItemLinkInfoCollection Items { get; private set; }
		#region Serialization
		public object XtraFindItemsItem(XtraItemEventArgs e) {
			return null;
		}
		public object XtraCreateItemsItem(XtraItemEventArgs e) {
			IdInfo sourceIdInfo = IdInfo.Empty;
			IdInfoContainer xmlIdInfo = RibbonCustomizationSerializationHelper.InfoFromItemEventArgs(e);
			bool isCustom = RibbonCustomizationSerializationHelper.IsCustomValueFromItemEventArgs(e);
			if(isCustom)
				xmlIdInfo.SourceIdInfo = RibbonCustomizationSerializationHelper.ItemSourceIdInfoValueFromItemEventArgs(e);
			RibbonItemLinkInfo itemLinkInfo;
			BarItemLink link = RibbonCustomizationSerializationHelper.GetEntry<BarItemLink>(RibbonControl, xmlIdInfo, e);
			if(link == null)
				return null;
			if(!isCustom)
				itemLinkInfo = new RibbonItemLinkInfo(link, xmlIdInfo.IdInfo, xmlIdInfo.SourceIdInfo, RibbonControl);
			else {
				BarItemLink cloneLink = ((ICloneable)link).Clone() as BarItemLink;
				itemLinkInfo = new RibbonItemLinkInfo(cloneLink, IdGenerator.Instance.Generate(RibbonControl), IdInfo.Empty, RibbonControl);
			}
			Items.Add(itemLinkInfo);
			return itemLinkInfo;
		}
		#endregion
		protected override string GetNameCore() {
			return Group != null ? Group.Name : string.Empty;
		}
		protected override void SetNameCore(string value) {
			if(Group != null) Group.Name = value;
		}
	}
	public class RibbonGroupInfoCollection : Collection<RibbonGroupInfo> {
		public void Assign(RibbonGroupInfoCollection groups) {
			Clear();
			foreach(RibbonGroupInfo groupInfo in groups) {
				Add(groupInfo);
			}
		}
		public bool ContainsGroup(RibbonPageGroup group) {
			foreach(RibbonGroupInfo groupInfo in this) {
				if(object.ReferenceEquals(groupInfo.Group, group))
					return true;
			}
			return false;
		}
		public int IndexOfPage(RibbonPageGroup group) {
			for(int i = 0; i < Count; i++) {
				RibbonGroupInfo groupInfo = this[i];
				if(object.ReferenceEquals(groupInfo.Group, group))
					return i;
			}
			return -1;
		}
	}
	public class RibbonItemLinkInfo : CustomizationItemInfoBase, IImageProvider {
		public RibbonItemLinkInfo(BarItemLink link, IdInfo idInfo, IdInfo sourceIdInfo, RibbonControl control) : base(idInfo, sourceIdInfo, control) {
			this.ItemLink = link;
			this.ItemSourceIdInfo = new IdInfo();
			this.Image = GetItemImage();
		}
		public override NodeType ItemType {
			get { return NodeType.Item; }
		}
		public override object InternalRibbonItem {
			get { return ItemLink; }
		}
		public override string Text {
			get { return ItemLink.Caption; }
		}
		public override void ApplyAlias() {
			if(ItemLink.Caption == Alias)
				return;
			if(ShouldInitItemCaption(ItemLink)) {
				ItemLink.Item.Caption = Alias;
			}
			else ItemLink.Caption = Alias;
		}
		protected virtual bool ShouldInitItemCaption(BarItemLink link) {
			return ShouldInitItem(link, () => {
				var links = ItemLink.Item.Links;
				string linkCaption = links[0].Caption;
				for(int i = 1; i < links.Count; i++) {
					if(links[i].Caption != linkCaption) return false;
				}
				return true;
			});
		}
		protected virtual bool ShouldInitItemVisibility(BarItemLink link) {
			return ShouldInitItem(link, () => {
				var links = ItemLink.Item.Links;
				bool linkVisible = links[0].Visible;
				for(int i = 1; i < links.Count; i++) {
					if(links[i].Visible != linkVisible) return false;
				}
				return true;
			});
		}
		protected virtual bool ShouldInitItem(BarItemLink link, Func<bool> callback) {
			if(RibbonControl.OptionsCustomizationForm.AllowLinkCustomization || link.Item == null) return false;
			if(ItemLink.Item.Links.Count <= 1) return true;
			return callback();
		}
		public virtual void ApplyVisibility() {
			if(ShouldInitItemVisibility(ItemLink)) {
				SetItemVisibility(IsVisible);
			}
			else ItemLink.Visible = IsVisible;
		}
		public void SetItemVisibility(bool visible) {
			ItemLink.Item.Visibility = visible ? BarItemVisibility.Always : BarItemVisibility.Never;
		}
		public override bool IsVisible {
			get;
			set;
		}
		public override object Clone() {
			BarItemLink clonedLink = ((ICloneable)ItemLink).Clone() as BarItemLink;
			RibbonItemLinkInfo res = new RibbonItemLinkInfo(clonedLink, IdGenerator.Instance.Generate(RibbonControl), IdInfo.Empty, RibbonControl);
			res.ItemSourceIdInfo.Assign(SourceIdInfo);
			return res;
		}
		protected virtual Image GetItemImage() {
			if(!IsItemImageExist(ItemLink))
				return null;
			return GetLinkImage(ItemLink);
		}
		protected virtual bool IsItemImageExist(BarItemLink link) {
			return link != null && (link.IsImageExist || link.IsLargeImageExist);
		}
		protected virtual Image GetLinkImage(BarItemLink link) {
			BarItem item = link.Item;
			if(item.Glyph != null)
				return item.Glyph;
			if(item.LargeGlyph != null)
				return item.LargeGlyph;
			if(item.ImageUri.HasImage)
				return item.ImageUri.GetImage();
			if(item.ImageUri.HasLargeImage)
				return item.ImageUri.GetLargeImage();
			if(link.LinkViewInfo != null)
				return link.LinkViewInfo.GetLinkImage(BarLinkState.Normal);
			if(link.IsImageExist)
				return ImageCollection.GetImageListImage(item.Images, item.ImageIndex);
			if(link.IsLargeImageExist)
				return ImageCollection.GetImageListImage(item.LargeImages, item.LargeImageIndex);
			return null;
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IdInfo ItemSourceIdInfo { get; set; }
		public BarItemLink ItemLink { get; private set; }
		#region IImageProvider
		public Image Image {
			get;
			set;
		}
		public bool IsImageExist {
			get { return Image != null; }
		}
		#endregion
		protected override string GetNameCore() {
			return string.Empty;
		}
		protected override void SetNameCore(string value) {
		}
	}
	public class RibbonItemLinkInfoCollection : Collection<RibbonItemLinkInfo> {
		public void Assign(RibbonItemLinkInfoCollection items) {
			Clear();
			foreach(RibbonItemLinkInfo itemInfo in items) {
				Add(itemInfo);
			}
		}
		public bool ContainsItemLink(BarItemLink link) {
			foreach(RibbonItemLinkInfo linkInfo in this) {
				if(object.ReferenceEquals(linkInfo.ItemLink, link))
					return true;
			}
			return false;
		}
		public int IndexOfPage(BarItemLink itemLink) {
			for(int i = 0; i < Count; i++) {
				RibbonItemLinkInfo linkInfo = this[i];
				if(object.ReferenceEquals(linkInfo.ItemLink, itemLink))
					return i;
			}
			return -1;
		}
	}
	public class ResultModelCreator {
		#region Signleton
		static ResultModelCreator() {
			Instance = new ResultModelCreator();
		}
		protected ResultModelCreator() { }
		public static ResultModelCreator Instance { get; private set; }
		#endregion
		public RibbonCustomizationModel Create(RunTimeRibbonTreeView treeView, RibbonControl control) {
			RibbonCustomizationModel model = new RibbonCustomizationModel(control);
			foreach(TreeNode categoryNode in treeView.Nodes) {
				RibbonPageCategoryInfo categoryInfo = categoryNode.Tag as RibbonPageCategoryInfo;
				categoryInfo.Pages.Assign(CreatePageInfoCollection(categoryNode));
				categoryInfo.IsVisible = categoryNode.Checked;
				model.Categories.Add(categoryInfo);
			}
			CreateCommonSettings(control, model);
			return model;
		}
		protected void CreateCommonSettings(RibbonControl control, RibbonCustomizationModel model) {
			RibbonCommonSettings settings = model.CommonSettings;
			settings.Minimized = control.Minimized;
			settings.ToolbarLocation = control.ToolbarLocation;
			settings.IsReady = true;
			RibbonStateInfo si = RibbonSourceStateInfo.Instance[control];
			settings.HashCode = si.ItemsCount;
		}
		protected RibbonPageInfoCollection CreatePageInfoCollection(TreeNode categoryNode) {
			RibbonPageInfoCollection pages = new RibbonPageInfoCollection();
			foreach(TreeNode pageNode in categoryNode.Nodes) {
				RibbonPageInfo pageInfo = pageNode.Tag as RibbonPageInfo;
				pageInfo.Groups.Assign(CreateGroupsInfoCollection(pageNode));
				pageInfo.IsVisible = pageNode.Checked;
				pages.Add(pageInfo);
			}
			return pages;
		}
		protected RibbonGroupInfoCollection CreateGroupsInfoCollection(TreeNode pageNode) {
			RibbonGroupInfoCollection groups = new RibbonGroupInfoCollection();
			foreach(TreeNode groupNode in pageNode.Nodes) {
				RibbonGroupInfo groupInfo = groupNode.Tag as RibbonGroupInfo;
				groupInfo.Items.Assign(CreateItemLinksInfoCollection(groupNode));
				groupInfo.Group.Text = groupNode.Text;
				groups.Add(groupInfo);
			}
			return groups;
		}
		protected RibbonItemLinkInfoCollection CreateItemLinksInfoCollection(TreeNode groupNode) {
			RibbonItemLinkInfoCollection items = new RibbonItemLinkInfoCollection();
			foreach(TreeNode linkNode in groupNode.Nodes) {
				RibbonItemLinkInfo itemInfo = linkNode.Tag as RibbonItemLinkInfo;
				itemInfo.IsVisible = GetItemInfoVisibility(itemInfo);
				items.Add(itemInfo);
			}
			return items;
		}
		protected virtual bool GetItemInfoVisibility(RibbonItemLinkInfo itemInfo) {
			BarItem item = itemInfo.ItemLink.Item;
			if(item == null) return itemInfo.ItemLink.Visible;
			if(item.Visibility == BarItemVisibility.Never || item.Visibility == BarItemVisibility.OnlyInCustomizing) return false;
			return itemInfo.ItemLink.Visible;
		}
	}
	public class CustomizationItemStateResolver {
		public static bool Contains(RibbonControl control, IIdInfoContainer idInfoContainer) {
			IdLinkTable table = RibbonSourceStateInfo.Instance.GetLinkTable(control);
			return table.ContainsId(idInfoContainer.SourceIdInfo);
		}
	}
	public class RibbonSourceStateInfo {
		protected RibbonSourceStateInfo(RibbonControl ribbonControl) {
			this.Info = new Dictionary<RibbonControl, RibbonStateInfo>();
		}
		#region Public
		public RibbonPageCategoryCollection GetCategories(RibbonControl control) {
			if(!Info.ContainsKey(control))
				return null;
			return Info[control].RibbonPageCategoryCollection;
		}
		public IdLinkTable GetLinkTable(RibbonControl control) {
			if(!Info.ContainsKey(control))
				return null;
			return Info[control].IdLinkTable;
		}
		public RibbonStateInfo this[RibbonControl ribbon] {
			get { return Info[ribbon]; }
		}
		public void Clear() {
			Info.Clear();
		}
		#endregion
		#region Init
		protected virtual void Init(RibbonControl control) {
			if(Info.ContainsKey(control))
				return;
			RibbonStateInfo stateInfo = CreateRibbonStateInfo(control);
			Info.Add(control, stateInfo);
		}
		protected virtual void ReCreateCore(RibbonControl control) {
			if(!Info.ContainsKey(control))
				return;
			Info.Remove(control);
			IdGenerator.Instance.Reset(control);
			Init(control);
		}
		protected virtual RibbonStateInfo CreateRibbonStateInfo(RibbonControl control) {
			RibbonStateInfo stateInfo = new RibbonStateInfo();
			stateInfo.RibbonPageCategoryCollection = GetSourceCatigories(control);
			stateInfo.IdLinkTable = CreateLinkTable(control);
			return stateInfo;
		}
		protected virtual RibbonPageCategoryCollection GetSourceCatigories(RibbonControl control) {
			CustomizationRibbonPageCategoryCollection res = new CustomizationRibbonPageCategoryCollection(control);
			res.Add(control.DefaultPageCategory);
			foreach(RibbonPageCategory pageCategory in control.PageCategories)
				res.Add(pageCategory);
			return res;
		}
		protected virtual IdLinkTable CreateLinkTable(RibbonControl control) {
			IdLinkTable linkTable = new IdLinkTable();
			RibbonPageCategoryCollection categories = control.PageCategories;
			CreateLinkTableCategory(control.DefaultPageCategory, linkTable);
			foreach(RibbonPageCategory category in categories)
				CreateLinkTableCategory(category, linkTable);
			return linkTable;
		}
		protected virtual void CreateLinkTableCategory(RibbonPageCategory category, IdLinkTable linkTable) {
			IdInfo categoryIdInfo = IdGenerator.Instance.Generate(category.Ribbon);
			linkTable.Add(categoryIdInfo, category, IdInfo.Empty, new RibbonElementSettings(category.Visible));
			foreach(RibbonPage page in category.Pages) {
				IdInfo pageIdInfo = IdGenerator.Instance.Generate(category.Ribbon);
				linkTable.Add(pageIdInfo, page, categoryIdInfo, new RibbonElementSettings(page.Visible));
				foreach(RibbonPageGroup group in page.Groups) {
					IdInfo groupIdInfo = IdGenerator.Instance.Generate(category.Ribbon);
					linkTable.Add(groupIdInfo, group, pageIdInfo, new RibbonElementSettings(group.Visible));
					CreateLinkTableItems(group, groupIdInfo, linkTable);
				}
			}
		}
		protected virtual void CreateLinkTableItems(RibbonPageGroup group, IdInfo groupIdInfo, IdLinkTable linkTable) {
			foreach(BarItemLink link in group.ItemLinks) {
				IdInfo linkIdInfo = IdGenerator.Instance.Generate(group.Ribbon);
				linkTable.Add(linkIdInfo, link, groupIdInfo, new RibbonElementSettings(link.Visible));
				if(CustomizationHelperBase.ShouldProcessPopupItems(group.Ribbon.OptionsCustomizationForm, link)) {
					RibbonCustomizationFormOptions options = group.Ribbon.OptionsCustomizationForm;
					BarItemLinkCollection links = CustomizationHelperBase.GetPopupItemLinks(options, link);
					foreach (BarItemLink popupLink in links) {
						linkTable.Add(IdGenerator.Instance.Generate(group.Ribbon), popupLink, linkIdInfo, new RibbonElementSettings(popupLink.Visible));
					}
				}
				BarButtonGroupLink buttonLink = link as BarButtonGroupLink;
				if(buttonLink == null)
					continue;
				foreach(BarItemLink subLink in buttonLink.Item.ItemLinks) {
					linkTable.Add(IdGenerator.Instance.Generate(group.Ribbon), subLink, linkIdInfo, new RibbonElementSettings(subLink.Visible));
				}
			}
		}
		#endregion
		public static RibbonSourceStateInfo Create(RibbonControl control) {
			if(Instance == null)
				Instance = new RibbonSourceStateInfo(control);
			Instance.Init(control);
			return Instance;
		}
		public static void ReCreate(RibbonControl control) {
			Instance.ReCreateCore(control);
		}
		public static void Remove(RibbonControl control) {
			if(Instance == null)
				return;
			Instance.Info.Remove(control);
			IdGenerator.Instance.Release(control);
		}
		public static void Destroy() {
			if(Instance == null)
				return;
			Instance.Clear();
			Instance = null;
		}
		public static RibbonSourceStateInfo Instance { get; private set; }
		protected internal Dictionary<RibbonControl, RibbonStateInfo> Info { get; private set; }
	}
	public class IdInfoContainer : IIdInfoContainer {
		public IdInfoContainer() : this(null, null) { }
		public IdInfoContainer(IdInfo info, IdInfo sourceInfo) {
			this.IdInfo = info;
			this.SourceIdInfo = sourceInfo;
		}
		public void Assign(IdInfoContainer info) {
			IdInfo = info.IdInfo;
			SourceIdInfo = info.SourceIdInfo;
		}
		public IdInfo IdInfo { get; set; }
		public IdInfo SourceIdInfo { get; set; }
	}
	public interface IIdInfoContainer {
		IdInfo IdInfo { get; }
		IdInfo SourceIdInfo { get; }
	}
	public class RibbonStateInfo {
		public RibbonStateInfo() {
			this.IdLinkTable = null;
			this.RibbonPageCategoryCollection = null;
		}
		public IdLinkTable IdLinkTable { get; set; }
		public RibbonPageCategoryCollection RibbonPageCategoryCollection { get; set; }
		public bool IsReady {
			get { return IdLinkTable != null; }
		}
		public int ItemsCount {
			get { return IsReady ? IdLinkTable.Count : 0; }
		}
	}
	public class IdLinkTable {
		public IdLinkTable() {
			this.Table = new Dictionary<IdInfo, object>();
			this.NamesTable = new Dictionary<IdInfo, string>();
			this.ParentsTable = new Dictionary<IdInfo, IdInfo>();
			this.OriginalSettings = new Dictionary<IdInfo, RibbonElementSettings>();
		}
		public bool ContainsId(IdInfo idInfo) {
			if(idInfo.IsEmpty)
				return false;
			return Table.ContainsKey(idInfo);
		}
		public void Add(IdInfo idInfo, object value, IdInfo parentIdInfo, RibbonElementSettings settings) {
			Table.Add(idInfo, value);
			NamesTable.Add(idInfo, GetEntryNameCore(value));
			ParentsTable.Add(idInfo, parentIdInfo);
			OriginalSettings.Add(idInfo, settings);
		}
		public object GetEntry(IdInfo idInfo) {
			return Table[idInfo];
		}
		public string GetEntryName(IdInfo idInfo) {
			return NamesTable[idInfo];
		}
		public IdInfo GetParentIdInfo(IdInfo idInfo) {
			return ParentsTable[idInfo];
		}
		public RibbonElementSettings GetOriginalSettings(IdInfo idInfo) {
			return OriginalSettings[idInfo];
		}
		public IdInfo GetId(object entry) {
			foreach(KeyValuePair<IdInfo, object> pair in Table) {
				if(object.ReferenceEquals(entry, pair.Value))
					return pair.Key;
			}
			return null;
		}
		public int Count { get { return Table.Count; } }
		string GetEntryNameCore(object entry) {
			RibbonPageCategory category = entry as RibbonPageCategory;
			if(category != null)
				return category.Text;
			RibbonPage page = entry as RibbonPage;
			if(page != null)
				return page.Text;
			RibbonPageGroup group = entry as RibbonPageGroup;
			if(group != null)
				return group.Text;
			BarItemLink link = entry as BarItemLink;
			if(link != null)
				return link.Caption;
			throw new ArgumentException("Invalid entry type");
		}
		protected Dictionary<IdInfo, object> Table { get; set; }
		protected Dictionary<IdInfo, string> NamesTable { get; set; }
		protected Dictionary<IdInfo, IdInfo> ParentsTable { get; set; }
		protected Dictionary<IdInfo, RibbonElementSettings> OriginalSettings { get; set; }
	}
	public class IdInfo {
		static IdInfo() {
			Empty = new IdInfo(int.MaxValue);
		}
		public IdInfo() : this(int.MaxValue) { }
		public IdInfo(int id) {
			this.Id = id;
		}
		public void Assign(IdInfo info) {
			Id = info.Id;
		}
		[XtraSerializableProperty()]
		public int Id { get; private set; }
		public bool IsEmpty { get { return Equals(IdInfo.Empty); } }
		public override bool Equals(object obj) {
			IdInfo sample = obj as IdInfo;
			return sample.Id == Id;
		}
		public override int GetHashCode() {
			return Id.GetHashCode();
		}
		public override string ToString() {
			return Id.ToString();
		}
		#region Constant Members
		public static IdInfo Empty { get; private set; }
		#endregion
	}
	public class IdGenerator {
		Dictionary<RibbonControl, int> dict;
		public IdGenerator() {
			this.dict = new Dictionary<RibbonControl, int>();
		}
		public IdInfo Generate(RibbonControl ribbon) {
			if(!dict.ContainsKey(ribbon)) {
				dict.Add(ribbon, 0);
			}
			IdInfo res = new IdInfo(dict[ribbon]);
			dict[ribbon]++;
			return res;
		}
		public void Release(RibbonControl ribbon) {
			if(!dict.ContainsKey(ribbon)) return;
			dict.Remove(ribbon);
		}
		public void Reset(RibbonControl ribbon) {
			if(!dict.ContainsKey(ribbon))
				return;
			dict[ribbon] = 0;
		}
		[ThreadStatic]
		static IdGenerator instanceCore = null;
		public static IdGenerator Instance {
			get {
				if(instanceCore == null) instanceCore = new IdGenerator();
				return instanceCore;
			}
		}
	}
	public class RibbonElementSettings {
		public RibbonElementSettings(bool isVisible) {
			this.IsVisible = isVisible;
		}
		public bool IsVisible { get; set; }
	}
	public class CustomizationHelperBase {
		public static bool IsCategory(TreeNode node) {
			return FromNode(node).ItemType == NodeType.Category;
		}
		public static bool IsPage(TreeNode node) {
			return FromNode(node).ItemType == NodeType.Page;
		}
		public static bool IsGroup(TreeNode node) {
			return FromNode(node).ItemType == NodeType.Group;
		}
		public static bool IsItem(TreeNode node) {
			return FromNode(node).ItemType == NodeType.Item;
		}
		public static int GetImageListIndex(NodeType type) {
			int res = -1;
			switch(type) {
				case NodeType.Category: res = 0; break;
				case NodeType.Page: res = 1; break;
				case NodeType.Group: res = 2; break;
				case NodeType.Item: res = 3; break;
			}
			return res;
		}
		public static CustomizationItemInfoBase FromNode(TreeNode node) {
			if(node == null) return null;
			return node.Tag as CustomizationItemInfoBase;
		}
		public static TreeNode CreateSourceNode(IdInfo idSourceInfo, RibbonControl control) {
			TreeNode node;
			using(RunTimeRibbonTreeViewOriginalView tree = CreateSourceTreeCore(control)) {
				node = FindTreeNodeCore(tree.Nodes, idSourceInfo).Clone() as TreeNode;
			}
			return node;
		}
		public static TreeNode FindTreeNodeCore(TreeNodeCollection nodes, IdInfo info) {
			TreeNode res = null;
			foreach(TreeNode node in nodes) {
				if(FromNode(node).SourceIdInfo.Equals(info))
					return node;
				res = FindTreeNodeCore(node.Nodes, info);
				if(res != null) return res;
			}
			return null;
		}
		public static List<TreeNode> GetSourceTreeNodes(RibbonControl control) {
			List<TreeNode> nodes = new List<TreeNode>();
			using(RunTimeRibbonTreeViewOriginalView tree = CreateSourceTreeCore(control)) {
				foreach(TreeNode node in tree.Nodes) {
					nodes.Add(node.Clone() as TreeNode);
				}
			}
			return nodes;
		}
		static RunTimeRibbonTreeViewOriginalView CreateSourceTreeCore(RibbonControl control) {
			RunTimeRibbonTreeViewOriginalView tree = new RunTimeRibbonTreeViewOriginalView(true) { Ribbon = control };
			tree.CreateTree();
			return tree;
		}
		internal static RunTimeRibbonTreeView CreateCustomizationTreeCore(RibbonControl control) {
			RunTimeRibbonTreeView tree = new RunTimeRibbonTreeView() { Ribbon = control };
			tree.CreateTree();
			return tree;
		}
		public static bool IsBarButtonGroupChildNode(TreeNode node) {
			TreeNode parent = node.Parent;
			if(parent == null)
				return false;
			CustomizationItemInfoBase info = FromNode(parent);
			if(info.ItemType != NodeType.Item)
				return false;
			return info.InternalRibbonItem is BarButtonGroupLink;
		}
		public static string AddInternalSuffixToText(string text) {
			return string.Concat(text, StandardCustomItemSuffix);
		}
		public static string ExcludeInternalSuffix(string text) {
			int pos = text.LastIndexOf(StandardCustomItemSuffix);
			if(pos == -1)
				return text;
			return text.Substring(0, pos);
		}
		public static bool ShouldProcessSuffix(TreeNode sourceNode) {
			CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(sourceNode);
			return info.IsCustom && info.ItemType != NodeType.Item;
		}
		public static string DefaultRibbonPageText {
			get {
				string baseName = BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationNewTabDefaultAlias);
				return string.Concat(baseName, StandardCustomItemSuffix);
			}
		}
		public static string DefaultRibbonPageGroupText {
			get {
				string baseName = BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationNewGroupDefaultAlias);
				return string.Concat(baseName, StandardCustomItemSuffix);
			}
		}
		public static string DefaultRibbonCategoryText {
			get {
				string baseName = BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationNewCategoryDefaultAlias);
				return string.Concat(baseName, StandardCustomItemSuffix);
			}
		}
		public static bool ShouldProcessPopupItems(RibbonCustomizationFormOptions options, BarItemLink link) {
			if(!options.AllowEditBarItemPopups)
				return false;
			return ContainsPopupItems(link);
		}
		public static bool ContainsPopupItems(BarItemLink link) {
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			if(buttonLink == null || buttonLink.Item.ButtonStyle != BarButtonStyle.DropDown)
				return false;
			return buttonLink.Item.DropDownControl is BarLinksHolder;
		}
		public static bool IsPopupItem(TreeNode node) {
			TreeNode parentNode = node.Parent;
			if(parentNode == null) return false;
			CustomizationItemInfoBase info = FromNode(parentNode);
			if(info == null) return false;
			BarButtonItemLink link = info.InternalRibbonItem as BarButtonItemLink;
			if(link == null) return false;
			return ContainsPopupItems(link);
		}
		public static BarItemLinkCollection GetPopupItemLinks(RibbonCustomizationFormOptions options, BarItemLink link) {
			if(!ShouldProcessPopupItems(options, link))
				return null;
			BarLinksHolder linksHolder = (link as BarButtonItemLink).Item.DropDownControl as BarLinksHolder;
			return linksHolder.ItemLinks;
		}
		public static void ForEachNode(TreeView treeView, Action<TreeNode> callback) {
			ForEachNodeCore(treeView.Nodes, callback);
		}
		static void ForEachNodeCore(TreeNodeCollection nodes, Action<TreeNode> callback) {
			foreach(TreeNode node in nodes) {
				callback(node);
				ForEachNodeCore(node.Nodes, callback);
			}
		}
		static string StandardCustomItemSuffix {
			get {
				string suffix = BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationStandardCustomItemSuffix);
				return string.Format(" ({0})", suffix);
			}
		}
	}
	public class RibbonProcessor {
		public RibbonProcessor(RibbonControl control) {
			this.Model = null;
			this.RibbonControl = control;
		}
		public void Process(RibbonCustomizationModel model) {
			Model = model;
			ProcessCategories(model);
			ProcessCommonSettings(model);
		}
		#region Common
		protected virtual void ProcessCommonSettings(RibbonCustomizationModel model) {
			if(!model.CommonSettings.IsReady) return;
			RibbonControl.Minimized = model.CommonSettings.Minimized;
			RibbonControl.ToolbarLocation = model.CommonSettings.ToolbarLocation;
		}
		#endregion
		#region Categories Processing
		protected virtual void ProcessCategories(RibbonCustomizationModel model) {
			foreach(RibbonPageCategoryInfo categoryInfo in model.Categories) {
				if(ShouldAddCategory(categoryInfo)) {
					RibbonControl.PageCategories.Add(categoryInfo.Category);
				}
				ProcessPages(categoryInfo.Pages, categoryInfo.Category);
			}
			RemoveCategories(model.Categories);
			RefreshCategoriesNames(model.Categories);
			RefreshCategoriesVisibility(model.Categories);
		}
		protected virtual bool ShouldAddCategory(RibbonPageCategoryInfo categoryInfo) {
			return categoryInfo.IsCustom && !RibbonControl.PageCategories.Contains(categoryInfo.Category);
		}
		protected virtual void RemoveCategories(RibbonPageCategoryInfoCollection categoriesInfo) {
			for(int i = 0; i < RibbonControl.PageCategories.Count; i++) {
				RibbonPageCategory pageCategory = RibbonControl.PageCategories[i];
				if(categoriesInfo.ContainsCategory(pageCategory))
					continue;
				if(RemoveCategoryCore(pageCategory)) --i;
			}
		}
		protected virtual bool RemoveCategoryCore(RibbonPageCategory pageCategory) {
			if(IsSourceEntry(pageCategory))
				return false;
			RibbonControl.PageCategories.Remove(pageCategory);
			return true;
		}
		protected virtual void RefreshCategoriesNames(RibbonPageCategoryInfoCollection categoriesInfo) {
			foreach(RibbonPageCategoryInfo categoryInfo in categoriesInfo)
				categoryInfo.ApplyAlias();
		}
		protected virtual void RefreshCategoriesVisibility(RibbonPageCategoryInfoCollection categoriesInfo) {
			foreach(RibbonPageCategoryInfo categoryInfo in categoriesInfo)
				categoryInfo.Category.Visible = categoryInfo.IsVisible;
		}
		#endregion
		#region Pages
		protected virtual void ProcessPages(RibbonPageInfoCollection pagesInfo, RibbonPageCategory category) {
			foreach(RibbonPageInfo pageInfo in pagesInfo) {
				if(ShouldAddPage(pageInfo, category)) {
					category.Pages.Add(pageInfo.Page);
				}
				ProcessGroups(pageInfo.Groups, pageInfo.Page);
			}
			RemovePages(pagesInfo, category);
			ReorderPages(pagesInfo, category);
			RefreshPageNames(pagesInfo);
			RefreshPageVisibility(pagesInfo);
		}
		protected virtual bool ShouldAddPage(RibbonPageInfo pageInfo, RibbonPageCategory category) {
			return pageInfo.IsCustom && !category.Pages.Contains(pageInfo.Page);
		}
		protected virtual void RemovePages(RibbonPageInfoCollection pagesInfo, RibbonPageCategory category) {
			for(int i = 0; i < category.Pages.Count; i++) {
				RibbonPage page = category.Pages[i];
				if(pagesInfo.ContainsPage(page))
					continue;
				if(RemovePageCore(page, category)) --i;
			}
		}
		protected virtual bool RemovePageCore(RibbonPage page, RibbonPageCategory category) {
			if(IsSourceEntry(page))
				return false;
			category.Pages.Remove(page);
			return true;
		}
		protected virtual void ReorderPages(RibbonPageInfoCollection pagesInfo, RibbonPageCategory category) {
			foreach(RibbonPageInfo pageInfo in pagesInfo) {
				int newPos = pagesInfo.IndexOfPage(pageInfo.Page);
				if(GetEntryIndex(category.Pages, pageInfo.Page) == newPos)
					continue;
				SetEntryPosition(category.Pages, pageInfo.Page, newPos);
			}
		}
		protected virtual void RefreshPageNames(RibbonPageInfoCollection pagesInfo) {
			foreach(RibbonPageInfo pageInfo in pagesInfo)
				pageInfo.ApplyAlias();
		}
		protected virtual void RefreshPageVisibility(RibbonPageInfoCollection pagesInfo) {
			foreach(RibbonPageInfo pageInfo in pagesInfo)
				pageInfo.Page.Visible = pageInfo.IsVisible;
		}
		#endregion
		#region Groups
		protected virtual void ProcessGroups(RibbonGroupInfoCollection groupsInfo, RibbonPage page) {
			foreach(RibbonGroupInfo groupInfo in groupsInfo) {
				if(ShouldAddGroup(groupInfo, page)) {
					page.Groups.Add(groupInfo.Group);
				}
				ProcessItemLinks(groupInfo.Items, groupInfo.Group);
			}
			RemoveGroups(groupsInfo, page);
			ReorderGroups(groupsInfo, page);
			RefreshGroupsNames(groupsInfo);
		}
		protected virtual bool ShouldAddGroup(RibbonGroupInfo groupInfo, RibbonPage page) {
			return !page.Groups.Contains(groupInfo.Group);
		}
		protected virtual void RemoveGroups(RibbonGroupInfoCollection groupsInfo, RibbonPage page) {
			for(int i = 0; i < page.Groups.Count; i++) {
				RibbonPageGroup group = page.Groups[i];
				if(groupsInfo.ContainsGroup(group)) {
					if(!group.Visible) group.Visible = true;
					continue;
				}
				if(RemoveGroupCore(group, page)) --i;
			}
		}
		protected virtual bool RemoveGroupCore(RibbonPageGroup group, RibbonPage page) {
			if(ShouldPurgeGroup(group, page)) {
				page.Groups.Remove(group);
				return true;
			}
			group.Visible = false;
			return false;
		}
		protected virtual bool ShouldPurgeGroup(RibbonPageGroup group, RibbonPage page) {
			if(!IsSourceEntry(group))
				return true;
			bool exist = false;
			RibbonPageInfoCollection pagesInfo = GetPageInfoCollection(page);
			foreach(RibbonPageInfo pageInfo in pagesInfo) {
				if(object.ReferenceEquals(pageInfo.Page, page))
					continue;
				if(pageInfo.Groups.ContainsGroup(group)) {
					exist = true;
					break;
				}
			}
			return exist;
		}
		protected RibbonPageInfoCollection GetPageInfoCollection(RibbonPage page) {
			RibbonPageCategoryInfo categoryInfo = Model.Categories.GetCategoryInfo(page.Category);
			return categoryInfo.Pages;
		}
		protected virtual void ReorderGroups(RibbonGroupInfoCollection groupsInfo, RibbonPage page) {
			foreach(RibbonGroupInfo groupInfo in groupsInfo) {
				int newPos = groupsInfo.IndexOfPage(groupInfo.Group);
				if(GetEntryIndex(page.Groups, groupInfo.Group) == newPos)
					continue;
				SetEntryPosition(page.Groups, groupInfo.Group, newPos);
			}
		}
		protected virtual void RefreshGroupsNames(RibbonGroupInfoCollection groupsInfo) {
			foreach(RibbonGroupInfo groupInfo in groupsInfo) {
				groupInfo.ApplyAlias();
			}
		}
		#endregion
		#region Items
		protected virtual void ProcessItemLinks(RibbonItemLinkInfoCollection items, RibbonPageGroup group) {
			foreach(RibbonItemLinkInfo linkInfo in items) {
				if(!ShouldAddItemLink(linkInfo, group))
					continue;
				BarItemLink link = linkInfo.ItemLink;
				group.ItemLinks.Add(link);
				if(link.Item != null) {
					link.Item.Links.Add(link);
				}
			}
			RemoveItemLinks(items, group);
			ReorderItemLinks(items, group);
			RefreshItemLinksNames(items);
			RefreshItemLinksVisibility(items);
		}
		protected virtual bool ShouldAddItemLink(RibbonItemLinkInfo linkInfo, RibbonPageGroup group) {
			return linkInfo.IsCustom && !group.ItemLinks.Contains(linkInfo.ItemLink);
		}
		protected virtual void RemoveItemLinks(RibbonItemLinkInfoCollection items, RibbonPageGroup group) {
			for(int i = 0; i < group.ItemLinks.Count; i++) {
				BarItemLink link = group.ItemLinks[i];
				if(items.ContainsItemLink(link))
					continue;
				if(RemoveGroupCore(link, group)) --i;
			}
		}
		protected virtual bool RemoveGroupCore(BarItemLink link, RibbonPageGroup group) {
			if(IsSourceEntry(link))
				return false;
			group.ItemLinks.Remove(link);
			return true;
		}
		protected virtual void ReorderItemLinks(RibbonItemLinkInfoCollection items, RibbonPageGroup group) {
			foreach(RibbonItemLinkInfo itemInfo in items) {
				if(!itemInfo.IsCustom)
					continue;
				int newPos = items.IndexOfPage(itemInfo.ItemLink);
				if(GetEntryIndex(group.ItemLinks, itemInfo.ItemLink) == newPos)
					continue;
				SetBarItemLinkPosition(group.ItemLinks, itemInfo.ItemLink, newPos);
			}
		}
		protected void SetBarItemLinkPosition(RibbonPageGroupItemLinkCollection links, BarItemLink link, int pos) {
			BarItemLink newLink = (BarItemLink)((ICloneable)link).Clone();
			links.Remove(link);
			links.Insert(pos, newLink);
		}
		protected virtual void RefreshItemLinksNames(RibbonItemLinkInfoCollection items) {
			foreach(RibbonItemLinkInfo itemInfo in items)
				itemInfo.ApplyAlias();
		}
		protected virtual void RefreshItemLinksVisibility(RibbonItemLinkInfoCollection items) {
			foreach(RibbonItemLinkInfo itemInfo in items) {
				itemInfo.ApplyVisibility();
			}
		}
		#endregion
		#region Helpers
		protected int GetEntryIndex(IList list, object entry) {
			return list.IndexOf(entry);
		}
		protected bool IsSourceEntry(object entry) {
			IdLinkTable linkTable = RibbonSourceStateInfo.Instance.GetLinkTable(RibbonControl);
			return linkTable.GetId(entry) != null;
		}
		protected void SetEntryPosition(IList list, object entry, int pos) {
			list.Remove(entry);
			list.Insert(pos, entry);
		}
		#endregion
		protected RibbonControl RibbonControl { get; private set; }
		protected RibbonCustomizationModel Model { get; private set; }
	}
	#region Drag and Drop support
	public abstract class DragDropSupportBase {
		public DragDropSupportBase(RunTimeRibbonTreeView tree) {
			this.TreeView = tree;
		}
		public enum DragDropAction { Move, Copy }
		public static DragDropSupportBase Create(RunTimeRibbonTreeView tree, DragDropAction cmd) {
			if(cmd == DragDropAction.Move)
				return new DragDropMove(tree);
			if(cmd == DragDropAction.Copy)
				return new DragDropCopy(tree);
			throw new NotSupportedException();
		}
		protected RunTimeRibbonTreeView TreeView { get; private set; }
		public abstract bool ShouldProcessDragDrop(TreeNode source, TreeNode target);
		public abstract void ProcessDragDrop(TreeNode source, TreeNode target);
	}
	public class DragDropMove : DragDropSupportBase, ICustomizationInfoProvider {
		public DragDropMove(RunTimeRibbonTreeView tree) : base(tree) { }
		public override bool ShouldProcessDragDrop(TreeNode source, TreeNode target) {
			MovingInfo info = CalcMovingInfo(source, target);
			if(info == null)
				return false;
			Info = info;
			return true;
		}
		public override void ProcessDragDrop(TreeNode source, TreeNode target) {
			if(!Info.IsGroupToPageMoving) ProcessDragDropCore(source, target);
			else ProcessDragDropGroupToPageCore(source, target);
		}
		#region Calc Moving Info
		protected MovingInfo CalcMovingInfo(TreeNode source, TreeNode target) {
			if(source == null || target == null)
				return null;
			if(IsMovingWithinParentNode(source, target))
				return CalcMovingInfoCore(source, target);
			if(IsMovingGroupToPage(source, target))
				return CalcMovingInfoGroupToPage(source, target);
			return null;
		}
		protected bool IsMovingWithinParentNode(TreeNode source, TreeNode target) {
			return GetChildLevel(source) == GetChildLevel(target);
		}
		protected bool IsMovingGroupToPage(TreeNode source, TreeNode target) {
			if(CustomizationHelperBase.FromNode(source).ItemType != NodeType.Group || CustomizationHelperBase.FromNode(target).ItemType != NodeType.Page)
				return false;
			if(object.ReferenceEquals(source.Parent, target))
				return false;
			return object.ReferenceEquals(source.Parent.Parent, target.Parent);
		}
		protected MovingInfo CalcMovingInfoCore(TreeNode source, TreeNode target) {
			MovingInfo info = new MovingInfo();
			List<TreeNode> nodes = GetNodesLevelList(GetChildLevel(source));
			int sourcePos = nodes.IndexOf(source), targetPos = nodes.IndexOf(target);
			int offset = sourcePos > targetPos ? -1 : 1;
			info.Direction = sourcePos > targetPos ? Direction.Up : Direction.Down;
			info.StepsCount = Math.Abs(sourcePos - targetPos);
			for(int i = sourcePos; i != targetPos; i += offset)
				info.Items.Add(nodes[i]);
			CustomizationStrategyMoveItemBase strategy = CreateCustomizationStrategy(info);
			foreach(TreeNode node in info.Items) {
				if(!strategy.ShouldProcessCommand(node, null)) return null;
			}
			return info;
		}
		protected MovingInfo CalcMovingInfoGroupToPage(TreeNode source, TreeNode target) {
			MovingInfo info = new MovingInfo(true);
			info.GroupNode = source;
			info.PageNode = target;
			return info;
		}
		protected List<TreeNode> GetNodesLevelList(int level) {
			List<TreeNode> res = new List<TreeNode>();
			GetNodesLevelListCore(TreeView.Nodes, level, res);
			return res;
		}
		protected void GetNodesLevelListCore(TreeNodeCollection nodes, int level, List<TreeNode> res) {
			foreach(TreeNode node in nodes) {
				GetNodesLevelListCore(node.Nodes, level, res);
				if(GetChildLevel(node) == level) res.Add(node);
			}
		}
		protected int GetChildLevel(TreeNode node) {
			int res = 0;
			TreeNode currNode = node;
			while(currNode.Parent != null) {
				res++;
				currNode = currNode.Parent;
			}
			return res;
		}
		#endregion
		#region Process Drag Drop
		protected virtual void ProcessDragDropCore(TreeNode source, TreeNode target) {
			CustomizationStrategyBase strategy = CreateCustomizationStrategy(Info);
			for(int i = 0; i < Info.StepsCount; i++) strategy.Customize();
		}
		protected virtual void ProcessDragDropGroupToPageCore(TreeNode source, TreeNode target) {
			source.Remove();
			target.Nodes.Add(source);
			if(!target.IsExpanded) target.Expand();
		}
		#endregion
		protected MovingInfo Info { get; private set; }
		protected virtual CustomizationStrategyMoveItemBase CreateCustomizationStrategy(MovingInfo info) {
			if(info.Direction == Direction.Up)
				return CustomizationStrategyMoveItemUpBase.Create(this);
			return CustomizationStrategyMoveItemDownBase.Create(this);
		}
		#region Data
		protected enum Direction { Up, Down, Unk }
		protected class MovingInfo {
			public MovingInfo() : this(-1, Direction.Unk) { }
			public MovingInfo(bool groupToPage) : this() {
				this.IsGroupToPageMoving = groupToPage;
			}
			public MovingInfo(int stepsCount, Direction direction) {
				this.StepsCount = stepsCount;
				this.Direction = direction;
				this.Items = new List<TreeNode>();
				this.IsGroupToPageMoving = false;
				this.PageNode = this.GroupNode = null;
			}
			public int StepsCount { get; set; }
			public Direction Direction { get; set; }
			public List<TreeNode> Items { get; set; }
			public bool IsGroupToPageMoving { get; set; }
			public TreeNode GroupNode { get; set; }
			public TreeNode PageNode { get; set; }
		}
		#endregion
		#region ICustomizationInfoProvider
		public bool ShouldRenameItem(TreeNode node) {
			return false;
		}
		public void ReloadForm() {
		}
		public void ResetRibbon() {
		}
		public RibbonControl RibbonControl {
			get { return TreeView.Ribbon; }
		}
		public new RunTimeRibbonTreeView TreeView {
			get { return base.TreeView; }
		}
		public RunTimeRibbonTreeView SourceTreeView {
			get { return null; }
		}
		public TreeNode SelectedNode {
			get { return TreeView.SelectedNode; }
		}
		public TreeNode SourceNode {
			get { return null; }
		}
		#endregion
	}
	public class DragDropCopy : DragDropSupportBase, ICustomizationInfoProvider {
		public DragDropCopy(RunTimeRibbonTreeView tree) : base(tree) {
			this.SourceNode = this.TargetNode = null;
			this.CustomizationHelper = null;
		}
		public override bool ShouldProcessDragDrop(TreeNode source, TreeNode target) {
			if(source == null || target == null)
				return false;
			SourceNode = source;
			TargetNode = target;
			CustomizationHelper = CreateCustomizationStrategy();
			return CustomizationHelper.ShouldProcessCommand(target, source);
		}
		public override void ProcessDragDrop(TreeNode source, TreeNode target) {
			CustomizationHelper.Customize();
		}
		protected TreeNode SourceNode { get; private set; }
		protected TreeNode TargetNode { get; private set; }
		protected CustomizationStrategyMoveToRightBase CustomizationHelper { get; private set; }
		#region Helpers
		protected virtual CustomizationStrategyMoveToRightBase CreateCustomizationStrategy() {
			return CustomizationStrategyMoveToRightBase.Create(this);
		}
		#endregion
		#region ICustomizationInfoProvider
		public bool ShouldRenameItem(TreeNode node) {
			return false;
		}
		public void ReloadForm() {
		}
		public void ResetRibbon() {
		}
		public RibbonControl RibbonControl {
			get { return TreeView.Ribbon; }
		}
		public new RunTimeRibbonTreeView TreeView {
			get { return TargetNode.TreeView as RunTimeRibbonTreeView; }
		}
		public RunTimeRibbonTreeView SourceTreeView {
			get { return SourceNode.TreeView as RunTimeRibbonTreeView; }
		}
		public TreeNode SelectedNode {
			get { return TargetNode; }
		}
		TreeNode ICustomizationInfoProvider.SourceNode {
			get { return SourceNode; }
		}
		#endregion
	}
	#endregion
}
