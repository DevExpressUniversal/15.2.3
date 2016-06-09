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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public class NavBarItem : CollectionItem {
		private const string DefaultText = "Item";
		private object fDataItem = null;
		private ITemplate fTemplate = null;
		private ITemplate fTextTemplate = null;
		private ItemImageProperties fImage = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
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
	DevExpressWebLocalizedDescription("NavBarItemEnabled"),
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
	DevExpressWebLocalizedDescription("NavBarItemImage"),
#endif
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ItemImageProperties Image {
			get {
				if(fImage == null)
					fImage = new ItemImageProperties(this);
				return fImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemNavigateUrl"),
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
	DevExpressWebLocalizedDescription("NavBarItemSelected"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool Selected {
			get { return GetBoolProperty("Selected", false); }
			set {
				if(NavBar != null) {
					if(value)
						NavBar.SelectedItem = this;
					else
						if(NavBar.SelectedItem == this)
							NavBar.SelectedItem = null;
				}
				else
					SetSelected(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemTarget"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true),
		TypeConverter(typeof(TargetConverter))]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set { SetStringProperty("Text", DefaultText, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate Template {
			get { return fTemplate; }
			set {
				fTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TextTemplate {
			get { return fTextTemplate; }
			set {
				fTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxNavBar NavBar {
			get {
				return (Group != null) ? Group.NavBar : null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavBarGroup Group {
			get {
				return (Collection as NavBarItemCollection) != null ? (Collection as NavBarItemCollection).Group : null;
			}
		}
		public NavBarItem()
			: base() {
		}
		public NavBarItem(string text)
			: this(text, "", "", "", "") {
		}
		public NavBarItem(string text, string name)
			: this(text, name, "", "", "") {
		}
		public NavBarItem(string text, string name, string imageUrl)
			: this(text, name, imageUrl, "", "") {
		}
		public NavBarItem(string text, string name, string imageUrl, string navigateUrl)
			: this(text, name, imageUrl, navigateUrl, "") {
		}
		public NavBarItem(string text, string name, string imageUrl, string navigateUrl, string target)
			: base() {
			Text = text;
			Name = name;
			Image.Url = imageUrl;
			NavigateUrl = navigateUrl;
			Target = target;
		}
		public override void Assign(CollectionItem source) {
			if (source is NavBarItem) {
				NavBarItem src = source as NavBarItem;
				ClientEnabled = src.ClientEnabled;
				ClientVisible = src.ClientVisible;
				Enabled = src.Enabled;
				Image.Assign(src.Image);
				Name = src.Name;
				NavigateUrl = src.NavigateUrl;
				Selected = src.Selected;
				Target = src.Target;
				Text = src.Text;
				ToolTip = src.ToolTip;
				Visible = src.Visible;
				Template = src.Template;
				TextTemplate = src.TextTemplate;
			}
			base.Assign(source);
		}
		public Control FindControl(string id) {
			return TemplateContainerBase.FindTemplateControl(NavBar, NavBar.GetItemTextTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(NavBar, NavBar.GetItemTemplateContainerID(this), id);
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected internal void SetDataItem(object value) {
			fDataItem = value;
		}
		protected internal void SetDataPath(string value) {
			SetStringProperty("DataPath", "", value);
		}
		protected internal void SetSelected(bool value) {
			SetBoolProperty("Selected", false, value);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Image };
		}
		protected override IDesignTimeCollectionItem GetDesignTimeItemParent() {
			return Group;
		}
	}
	public class NavBarItemCollection : Collection<NavBarItem> {
		public NavBarItemCollection()
			: base() {
		}
		public NavBarItemCollection(NavBarGroup group)
			: base(group) {
		}
		protected internal NavBarGroup Group {
			get { return Owner as NavBarGroup; }
		}
		protected internal ASPxNavBar NavBar {
			get { return (Group != null) ? Group.NavBar : null; }
		}
		public NavBarItem Add() {
			return AddInternal(new NavBarItem());
		}
		public NavBarItem Add(string text) {
			return Add(text, "", "", "", "");
		}
		public NavBarItem Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public NavBarItem Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public NavBarItem Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public NavBarItem Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			return AddInternal(new NavBarItem(text, name, imageUrl, navigateUrl, target));
		}
		public NavBarItem FindByName(string name) {
			return FindByIndex(IndexOfName(name));
		}
		public NavBarItem FindByText(string text) {
			return FindByIndex(IndexOfText(text));
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(NavBarItem item) {
				return item.Name == name;
			});
		}
		public int IndexOfText(string text) {
			return IndexOf(delegate(NavBarItem item) {
				return item.Text == text;
			});
		}
		protected override void OnChanged() {
			if (NavBar != null)
				NavBar.ItemsChanged();
		}
	}
}
