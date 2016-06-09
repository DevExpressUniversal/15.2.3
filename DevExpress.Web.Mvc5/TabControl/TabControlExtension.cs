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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	public abstract class TabControlExtensionBase: ExtensionBase {
		public TabControlExtensionBase(TabControlSettingsBase settings)
			: base(settings) {
		}
		public TabControlExtensionBase(TabControlSettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxTabControlBase Control {
			get { return (ASPxTabControlBase)base.Control; }
		}
		protected internal new TabControlSettingsBase Settings {
			get { return (TabControlSettingsBase)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.EnableHotTrack = Settings.EnableHotTrack;
			Control.EnableTabScrolling = Settings.EnableTabScrolling;
			Control.TabAlign = Settings.TabAlign;
			Control.TabPosition = Settings.TabPosition;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.SpaceAfterTabsTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.SpaceAfterTabsContent, Settings.SpaceAfterTabsMethod, typeof(TemplateContainerBase));
			Control.SpaceBeforeTabsTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.SpaceBeforeTabsContent, Settings.SpaceBeforeTabsMethod, typeof(TemplateContainerBase));
		}
	}
	public class TabControlExtension: TabControlExtensionBase {
		public TabControlExtension(TabControlSettings settings)
			: base(settings) {
		}
		public TabControlExtension(TabControlSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxTabControl Control {
			get { return (MVCxTabControl)base.Control; }
		}
		protected internal new TabControlSettings Settings {
			get { return (TabControlSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ActiveTabImageUrlField = Settings.ActiveTabImageUrlField;
			Control.Images.CopyFrom(Settings.Images);
			Control.NameField = Settings.NameField;
			Control.NavigateUrlField = Settings.NavigateUrlField;
			Control.NavigateUrlFormatString = Settings.NavigateUrlFormatString;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.SyncSelectionMode = Settings.SyncSelectionMode;
			Control.TabImageUrlField = Settings.TabImageUrlField;
			Control.Target = Settings.Target;
			Control.TextField = Settings.TextField;
			Control.TextFormatString = Settings.TextFormatString;
			Control.ToolTipField = Settings.ToolTipField;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.Tabs.Assign(Settings.Tabs);
			Control.ActiveTabIndex = Settings.ActiveTabIndex;
			Control.TabDataBound += Settings.TabDataBound;
			Control.DataBound += Settings.DataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.TabTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
				Settings.TabTemplateContent, Settings.TabTemplateContentMethod, typeof(TabControlTemplateContainer));
			Control.ActiveTabTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
				Settings.ActiveTabTemplateContent, Settings.ActiveTabTemplateContentMethod, typeof(TabControlTemplateContainer));
			Control.TabTextTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
				Settings.TabTextTemplateContent, Settings.TabTextTemplateContentMethod, typeof(TabControlTemplateContainer));
			Control.ActiveTabTextTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
				Settings.ActiveTabTextTemplateContent, Settings.ActiveTabTextTemplateContentMethod, typeof(TabControlTemplateContainer));
			for (int i = 0; i < Control.Tabs.Count; i++) {
				MVCxTab sourceTab = (i < Settings.Tabs.Count) ? Settings.Tabs[i] : null;
				if (sourceTab == null) continue;
				Control.Tabs[i].TabTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
					sourceTab.TabTemplateContent, sourceTab.TabTemplateContentMethod, 
					typeof(TabControlTemplateContainer));
				Control.Tabs[i].ActiveTabTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
					sourceTab.ActiveTabTemplateContent, sourceTab.ActiveTabTemplateContentMethod, 
					typeof(TabControlTemplateContainer));
				Control.Tabs[i].TabTextTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
					sourceTab.TabTextTemplateContent, sourceTab.TabTextTemplateContentMethod,
					typeof(TabControlTemplateContainer));
				Control.Tabs[i].ActiveTabTextTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
					sourceTab.ActiveTabTextTemplateContent, sourceTab.ActiveTabTextTemplateContentMethod,
					typeof(TabControlTemplateContainer));
			}
		}
		public TabControlExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public TabControlExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public TabControlExtension BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public TabControlExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public TabControlExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public TabControlExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxTabControl();
		}
	}
	public class PageControlExtension: TabControlExtensionBase {
		public PageControlExtension(PageControlSettings settings)
			: base(settings) {
		}
		public PageControlExtension(PageControlSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxPageControl Control {
			get { return (MVCxPageControl)base.Control; }
		}
		protected internal new PageControlSettings Settings {
			get { return (PageControlSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ActivateTabPageAction = Settings.ActivateTabPageAction;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.Images.CopyFrom(Settings.Images);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ShowTabs = Settings.ShowTabs;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.TabPages.Assign(Settings.TabPages);
			Control.ActiveTabIndex = Settings.ActiveTabIndex;
			Control.ClientLayout += Settings.ClientLayout;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.TabTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
				Settings.TabTemplateContent, Settings.TabTemplateContentMethod, typeof(PageControlTemplateContainer));
			Control.ActiveTabTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
				Settings.ActiveTabTemplateContent, Settings.ActiveTabTemplateContentMethod, typeof(PageControlTemplateContainer));
			Control.TabTextTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
				Settings.TabTextTemplateContent, Settings.TabTextTemplateContentMethod, typeof(PageControlTemplateContainer));
			Control.ActiveTabTextTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
				Settings.ActiveTabTextTemplateContent, Settings.ActiveTabTextTemplateContentMethod, typeof(PageControlTemplateContainer));
			for (int i = 0; i < Control.TabPages.Count; i++) {
				MVCxTabPage sourceTabPage = (i < Settings.TabPages.Count) ? Settings.TabPages[i] : null;
				if (sourceTabPage == null) continue;
				Control.TabPages[i].Controls.Add(ContentControl.Create(sourceTabPage.Content, sourceTabPage.ContentMethod));
				Control.TabPages[i].TabTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
					sourceTabPage.TabTemplateContent, sourceTabPage.TabTemplateContentMethod, 
					typeof(PageControlTemplateContainer));
				Control.TabPages[i].ActiveTabTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
					sourceTabPage.ActiveTabTemplateContent, sourceTabPage.ActiveTabTemplateContentMethod, 
					typeof(PageControlTemplateContainer));
				Control.TabPages[i].TabTextTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
					sourceTabPage.TabTextTemplateContent, sourceTabPage.TabTextTemplateContentMethod,
					typeof(PageControlTemplateContainer));
				Control.TabPages[i].ActiveTabTextTemplate = ContentControlTemplate<PageControlTemplateContainer>.Create(
					sourceTabPage.ActiveTabTextTemplateContent, sourceTabPage.ActiveTabTextTemplateContentMethod,
					typeof(PageControlTemplateContainer));
			}
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.EnsureClientStateLoaded();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		public static PageControlState GetState(string name) {
			return MVCxPageControl.GetState(name);
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxPageControl();
		}
	}
}
