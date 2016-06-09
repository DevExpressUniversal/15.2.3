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
namespace DevExpress.Web {
	public class TabBase : ContentControlCollectionItem {
		private const string DefaultText = "Tab";
		private TabImageProperties fActiveTabImage = null;
		private TabImageProperties fTabImage = null;
		private TabStyle fActiveTabStyle = null;
		private TabStyle fTabStyle = null;
		private ITemplate fActiveTabTemplate = null;
		private ITemplate fTabTemplate = null;
		ITemplate activeTabTextTemplate = null;
		ITemplate tabTextTemplate = null;
		private object fDataItem = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseEnabled"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				SetBoolProperty("Enabled", true, value);
				ValidateActiveGroupIndex();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set { SetStringProperty("Text", DefaultText, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseActiveTabImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties ActiveTabImage {
			get {
				if(fActiveTabImage == null)
					fActiveTabImage = new TabImageProperties(this);
				return fActiveTabImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseTabImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties TabImage {
			get {
				if(fTabImage == null)
					fTabImage = new TabImageProperties(this);
				return fTabImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseActiveTabStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle ActiveTabStyle {
			get {
				if(fActiveTabStyle == null)
					fActiveTabStyle = new TabStyle();
				return fActiveTabStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseTabStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle TabStyle {
			get {
				if(fTabStyle == null)
					fTabStyle = new TabStyle();
				return fTabStyle;
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ActiveTabTemplate {
			get { return fActiveTabTemplate; }
			set {
				fActiveTabTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TabTemplate {
			get { return fTabTemplate; }
			set {
				fTabTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ActiveTabTextTemplate {
			get { return this.activeTabTextTemplate; }
			set {
				this.activeTabTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TabTextTemplate {
			get { return this.tabTextTemplate; }
			set {
				this.tabTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object DataItem {
			get { return fDataItem; }
			set { SetDataItem(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabBaseNewLine"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool NewLine {
			get { return GetBoolProperty("NewLine", false); }
			set {
				SetBoolProperty("NewLine", false, value);
				LayoutChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsActive {
			get { return (TabControl != null) && (TabControl.ActiveTabIndex == Index); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ASPxTabControlBase TabControl {
			get { return ((Collection as TabCollectionBase) != null) ? (Collection as TabCollectionBase).TabControl : null; }
		}
		public TabBase()
			: base() {
		}
		public TabBase(string text)
			: this(text, "", "") {
		}
		public TabBase(string text, string name)
			: this(text, name, "") {
		}
		public TabBase(string text, string name, string imageUrl)
			: base() {
			Text = text;
			Name = name;
			TabImage.Url = imageUrl;
		}
		public override void Assign(CollectionItem source) {
			if (source is TabBase) {
				TabBase src = source as TabBase;
				ClientEnabled = src.ClientEnabled;
				ClientVisible = src.ClientVisible;
				Enabled = src.Enabled;
				ActiveTabImage.Assign(src.ActiveTabImage);
				TabImage.Assign(src.TabImage);
				Name = src.Name;
				Text = src.Text;
				ToolTip = src.ToolTip;
				Visible = src.Visible;
				ActiveTabTemplate = src.ActiveTabTemplate;
				TabTemplate = src.TabTemplate;
				ActiveTabTextTemplate = src.ActiveTabTextTemplate;
				TabTextTemplate = src.TabTextTemplate;
				NewLine = src.NewLine;
				ActiveTabStyle.Assign(src.ActiveTabStyle);
				TabStyle.Assign(src.TabStyle);
			}
			base.Assign(source);
		}
		public virtual Control FindControl(string id) {
			return TemplateContainerBase.FindTemplateControl(TabControl, TabControl.GetTabTemplateContainerID(this, IsActive), id)
				?? TemplateContainerBase.FindTemplateControl(TabControl, TabControl.GetTabTemplateContainerID(this, !IsActive), id);
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected void ValidateActiveGroupIndex() {
			if (TabControl != null)
				TabControl.ValidateActiveTabIndex();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ActiveTabStyle, TabStyle, ActiveTabImage, TabImage };
		}
		protected internal void SetDataItem(object value) {
			fDataItem = value;
		}
	}
	public class TabCollectionBase : Collection {
		protected internal TabBase this[int index] {
			get { return (GetItem(index) as TabBase); }
		}
		protected internal ASPxTabControlBase TabControl {
			get { return Owner as ASPxTabControlBase; }
		}
		public TabCollectionBase()
			: base() {
		}
		public TabCollectionBase(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		public int IndexOfName(string name) {
			for (int i = 0; i < Count; i++)
				if ((GetItem(i) as TabBase).Name.Equals(name))
					return i;
			return -1;
		}
		public int IndexOfText(string text) {
			for (int i = 0; i < Count; i++)
				if ((GetItem(i) as TabBase).Text == text)
					return i;
			return -1;
		}
		protected internal TabBase GetVisibleTabItem(int index) {
			return GetVisibleItem(index) as TabBase;
		}
		protected internal int GetVisibleTabItemCount() {
			return GetVisibleItemCount();
		}
		protected override Type GetKnownType() {
			return typeof(TabBase);
		}
		protected override void OnChanged() {
			if (TabControl != null)
				TabControl.TabsChanged();
		}
	}
	public class Tab : TabBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("TabNavigateUrl"),
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
	DevExpressWebLocalizedDescription("TabTarget"),
#endif
		DefaultValue(""), Localizable(false),
		NotifyParentProperty(true), TypeConverter(typeof(TargetConverter))]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainer))]
		public override ITemplate ActiveTabTemplate {
			get { return base.ActiveTabTemplate; }
			set { base.ActiveTabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainer))]
		public override ITemplate TabTemplate {
			get { return base.TabTemplate; }
			set { base.TabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainer))]
		public override ITemplate ActiveTabTextTemplate {
			get { return base.ActiveTabTextTemplate; }
			set { base.ActiveTabTextTemplate = value; }
		}
		[Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainer))]
		public override ITemplate TabTextTemplate {
			get { return base.TabTextTemplate; }
			set { base.TabTextTemplate = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ControlCollection Controls {
			get { return base.Controls; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ContentControlCollection ContentCollection {
			get { return base.ContentCollection; }
		}
		public Tab()
			: base() {
		}
		public Tab(string text)
			: this(text, "", "", "", "") {
		}
		public Tab(string text, string name)
			: this(text, name, "", "", "") {
		}
		public Tab(string text, string name, string imageUrl)
			: this(text, name, imageUrl, "", "") {
		}
		public Tab(string text, string name, string imageUrl, string navigateUrl)
			: this(text, name, imageUrl, navigateUrl, "") {
		}
		public Tab(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl) {
			NavigateUrl = navigateUrl;
			Target = target;
		}
		public override void Assign(CollectionItem source) {
			if (source is Tab) {
				Tab src = source as Tab;
				NavigateUrl = src.NavigateUrl;
				Target = src.Target;
			}
			base.Assign(source);
		}
	}
	public class TabCollection : TabCollectionBase {
#if !SL
	[DevExpressWebLocalizedDescription("TabCollectionItem")]
#endif
		public new Tab this[int index] {
			get { return (GetItem(index) as Tab); }
		}
		public TabCollection()
			: base() {
		}
		public TabCollection(ASPxTabControl tabControl)
			: base(tabControl) {
		}
		public void Add(Tab tab) {
					base.Add(tab);
		}
		public Tab Add() {
			Tab tab = new Tab();
			Add(tab);
			return tab;
		}
		public Tab Add(string text) {
			return Add(text, "", "", "", "");
		}
		public Tab Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public Tab Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public Tab Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public Tab Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			Tab tab = new Tab(text, name, imageUrl, navigateUrl, target);
			Add(tab);
			return tab;
		}
		public Tab GetVisibleTab(int index) {
			return GetVisibleItem(index) as Tab;
		}
		public int GetVisibleTabCount() {
			return GetVisibleItemCount();
		}
		public int IndexOf(Tab tab) {
					return base.IndexOf(tab);
		}
		public void Insert(int index, Tab tab) {
					base.Insert(index, tab);
		}
		public void Remove(Tab tab) {
					base.Remove(tab);
		}
		public Tab FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public Tab FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(Tab);
		}
	}
}
