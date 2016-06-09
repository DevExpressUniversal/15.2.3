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

using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Internal;
	using System.Collections;
	public abstract class MenuExtensionBase : ExtensionBase {
		public MenuExtensionBase(MenuSettingsBase settings)
			: base(settings) {
		}
		public MenuExtensionBase(MenuSettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxMenuBase Control {
			get { return (ASPxMenuBase)base.Control; }
		}
		protected internal new MenuSettingsBase Settings {
			get { return (MenuSettingsBase)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowSelectItem = Settings.AllowSelectItem;
			Control.AppearAfter = Settings.AppearAfter;
			Control.AutoSeparators = Settings.AutoSeparators;
			Control.BorderBetweenItemAndSubMenu = Settings.BorderBetweenItemAndSubMenu;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.DisappearAfter = Settings.DisappearAfter;
			Control.EnableAnimation = Settings.EnableAnimation;
			Control.EnableHotTrack = Settings.EnableHotTrack;
			Control.ImageUrlField = Settings.ImageUrlField;
			Control.ItemLinkMode = Settings.ItemLinkMode;
			Control.Items.Assign(Settings.Items);
			Control.ItemSubMenuOffset.Assign(Settings.ItemSubMenuOffset);
			Control.MaximumDisplayLevels = Settings.MaximumDisplayLevels;
			Control.NameField = Settings.NameField;
			Control.NavigateUrlField = Settings.NavigateUrlField;
			Control.NavigateUrlFormatString = Settings.NavigateUrlFormatString;
			Control.RenderIFrameForPopupElements = Settings.RenderIFrameForPopupElements;
			Control.RootItemSubMenuOffset.Assign(Settings.RootItemSubMenuOffset);
			Control.SelectParentItem = Settings.SelectParentItem;
			Control.ShowPopOutImages = Settings.ShowPopOutImages;
			Control.SyncSelectionMode = Settings.SyncSelectionMode;
			Control.Target = Settings.Target;
			Control.TextField = Settings.TextField;
			Control.TextFormatString = Settings.TextFormatString;
			Control.ToolTipField = Settings.ToolTipField;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ItemDataBound += Settings.ItemDataBound;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ItemTemplate = ContentControlTemplate<MenuItemTemplateContainer>.Create(
				Settings.ItemTemplateContent, Settings.ItemTemplateContentMethod, typeof(MenuItemTemplateContainer));
			Control.ItemTextTemplate = ContentControlTemplate<MenuItemTemplateContainer>.Create(
				Settings.ItemTextTemplateContent, Settings.ItemTextTemplateContentMethod, typeof(MenuItemTemplateContainer));
			Control.SubMenuTemplate = ContentControlTemplate<MenuItemTemplateContainer>.Create(
				Settings.SubMenuTemplateContent, Settings.SubMenuTemplateContentMethod, typeof(MenuItemTemplateContainer));
			for (int i = 0; i < Control.Items.Count; i++) {
				MVCxMenuItem sourceItem = (i < Settings.Items.Count) ? Settings.Items[i] : null;
				AssignItemTemplates(sourceItem, Control.Items[i]);
			}
		}
		protected void AssignItemTemplates(MVCxMenuItem sourceItem, MenuItem destinationItem) {
			if (sourceItem == null) return;
			destinationItem.Template = ContentControlTemplate<MenuItemTemplateContainer>.Create(
				sourceItem.TemplateContent, sourceItem.TemplateContentMethod, typeof(MenuItemTemplateContainer));
			destinationItem.TextTemplate = ContentControlTemplate<MenuItemTemplateContainer>.Create(
				sourceItem.TextTemplateContent, sourceItem.TextTemplateContentMethod, typeof(MenuItemTemplateContainer));
			destinationItem.SubMenuTemplate = ContentControlTemplate<MenuItemTemplateContainer>.Create(
				sourceItem.SubMenuTemplateContent, sourceItem.SubMenuTemplateContentMethod, typeof(MenuItemTemplateContainer));
			for (int i = 0; i < destinationItem.Items.Count; i++) {
				MVCxMenuItem sourceSubItem = (sourceItem != null && i < sourceItem.Items.Count) ? sourceItem.Items[i] : null;
				AssignItemTemplates(sourceSubItem, destinationItem.Items[i]);
			}
		}
		public MenuExtensionBase Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public MenuExtensionBase BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public MenuExtensionBase BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public MenuExtensionBase BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public MenuExtensionBase BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public MenuExtensionBase BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
	}
	public class MenuExtension : MenuExtensionBase {
		public MenuExtension(MenuSettings settings)
			: base(settings) {
		}
		public MenuExtension(MenuSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxMenu Control {
			get { return (MVCxMenu)base.Control; }
		}
		protected internal new MenuSettings Settings {
			get { return (MenuSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.ClientVisible = Settings.ClientVisible;
			Control.EnableAdaptivity = Settings.EnableAdaptivity;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.EnableSubMenuFullWidth = Settings.EnableSubMenuFullWidth;
			Control.EnableSubMenuScrolling = Settings.EnableSubMenuScrolling;
			Control.Images.CopyFrom(Settings.Images);
			Control.ItemAutoWidth = Settings.ItemAutoWidth;
			Control.ItemImagePosition = Settings.ItemImagePosition;
			Control.Orientation = Settings.Orientation;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.ShowAsToolbar = Settings.ShowAsToolbar;
			base.AssignInitialProperties();
		}
		public new MenuExtension Bind(object dataObject) {
			return (MenuExtension)base.Bind(dataObject);
		}
		public new MenuExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public new MenuExtension BindToSiteMap(string fileName, bool showStartingNode) {
			return (MenuExtension)base.BindToSiteMap(fileName, showStartingNode);
		}
		public new MenuExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public new MenuExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public new MenuExtension BindToXML(string fileName, string xPath, string transformFileName) {
			return (MenuExtension)base.BindToXML(fileName, xPath, transformFileName);
		}
		protected internal override void PrepareControlProperties() {
			base.PrepareControlProperties();
			Control.ValidateProperties();
		}
		public static MenuState GetState(string name) {
			return MVCxMenu.GetState(name);
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxMenu();
		}
	}
	public class PopupMenuExtension : MenuExtensionBase {
		public PopupMenuExtension(PopupMenuSettings settings)
			: base(settings) {
		}
		public PopupMenuExtension(PopupMenuSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxPopupMenu Control {
			get { return (MVCxPopupMenu)base.Control; }
		}
		protected internal new PopupMenuSettings Settings {
			get { return (PopupMenuSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.Images.CopyFrom(Settings.Images);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.CloseAction = Settings.CloseAction;
			Control.EnableScrolling = Settings.EnableScrolling;
			Control.PopupAction = Settings.PopupAction;
			Control.PopupElementID = Settings.PopupElementID;
			Control.PopupHorizontalAlign = Settings.PopupHorizontalAlign;
			Control.PopupVerticalAlign = Settings.PopupVerticalAlign;
			Control.PopupHorizontalOffset = Settings.PopupHorizontalOffset;
			Control.PopupVerticalOffset = Settings.PopupVerticalOffset;
			Control.Left = Settings.Left;
			Control.Top = Settings.Top;
			Control.PopupElementResolve += Settings.PopupElementResolve;
			base.AssignInitialProperties();
		}
		public new PopupMenuExtension Bind(object dataObject) {
			return (PopupMenuExtension)base.Bind(dataObject);
		}
		public new PopupMenuExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public new PopupMenuExtension BindToSiteMap(string fileName, bool showStartingNode) {
			return (PopupMenuExtension)base.BindToSiteMap(fileName, showStartingNode);
		}
		public new PopupMenuExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public new PopupMenuExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public new PopupMenuExtension BindToXML(string fileName, string xPath, string transformFileName) {
			return (PopupMenuExtension)base.BindToXML(fileName, xPath, transformFileName);
		}
		protected internal override void PrepareControlProperties() {
			base.PrepareControlProperties();
			Control.ValidateProperties();
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxPopupMenu();
		}
	}
}
