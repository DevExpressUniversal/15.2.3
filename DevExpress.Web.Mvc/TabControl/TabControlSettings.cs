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
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Mvc {
	public abstract class TabControlSettingsBase: SettingsBase {
		public TabControlSettingsBase()
			: base() {
			EnableHotTrack = true;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public int ActiveTabIndex { get; set; }
		public TabControlClientSideEvents ClientSideEvents { get { return (TabControlClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new TabControlStyle ControlStyle { get { return (TabControlStyle)base.ControlStyle; } }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public bool EnableHotTrack { get; set; }
		public bool EnableTabScrolling { get; set; }
		public TabControlImages Images { get { return (TabControlImages)ImagesInternal; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public TabControlStyles Styles { get { return (TabControlStyles)StylesInternal; } }
		public TabAlign TabAlign { get; set; }
		public TabPosition TabPosition { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected internal string SpaceAfterTabsContent { get; set; }
		protected internal Action<TemplateContainerBase> SpaceAfterTabsMethod { get; set; }
		protected internal string SpaceBeforeTabsContent { get; set; }
		protected internal Action<TemplateContainerBase> SpaceBeforeTabsMethod { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TabControlClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new TabControlStyle();
		}
		protected override ImagesBase CreateImages() {
			return new TabControlImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new TabControlStyles(null);
		}
		public void SetSpaceAfterTabsContent(Action<TemplateContainerBase> contentMethod) {
			SpaceAfterTabsMethod = contentMethod;
		}
		public void SetSpaceAfterTabsContent(string content) {
			SpaceAfterTabsContent = content;
		}
		public void SetSpaceBeforeTabsContent(Action<TemplateContainerBase> contentMethod) {
			SpaceBeforeTabsMethod = contentMethod;
		}
		public void SetSpaceBeforeTabsContent(string content) {
			SpaceBeforeTabsContent = content;
		}
	}
	public class TabControlSettings: TabControlSettingsBase {
		MVCxTabCollection tabs;
		public TabControlSettings() {
			this.tabs = new MVCxTabCollection();
			SyncSelectionMode = SyncSelectionMode.CurrentPathAndQuery;
		}
		public string ActiveTabImageUrlField { get; set; }
		public string NameField { get; set; }
		public string NavigateUrlField { get; set; }
		public string NavigateUrlFormatString { get; set; }
		public SyncSelectionMode SyncSelectionMode { get; set; }
		public string TabImageUrlField { get; set; }
		public MVCxTabCollection Tabs { get { return tabs; } }
		public string Target { get; set; }
		public string TextField { get; set; }
		public string TextFormatString { get; set; }
		public string ToolTipField { get; set; }
		public TabControlEventHandler TabDataBound { get; set; }
		public EventHandler DataBound { get; set; }
		protected internal string ActiveTabTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> ActiveTabTemplateContentMethod { get; set; }
		protected internal string TabTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> TabTemplateContentMethod { get; set; }
		protected internal string ActiveTabTextTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> ActiveTabTextTemplateContentMethod { get; set; }
		protected internal string TabTextTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> TabTextTemplateContentMethod { get; set; }
		public void SetActiveTabTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			ActiveTabTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTemplateContent(string content) {
			ActiveTabTemplateContent = content;
		}
		public void SetTabTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			TabTemplateContentMethod = contentMethod;
		}
		public void SetTabTemplateContent(string content) {
			TabTemplateContent = content;
		}
		public void SetActiveTabTextTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			ActiveTabTextTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTextTemplateContent(string content) {
			ActiveTabTextTemplateContent = content;
		}
		public void SetTabTextTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			TabTextTemplateContentMethod = contentMethod;
		}
		public void SetTabTextTemplateContent(string content) {
			TabTextTemplateContent = content;
		}
	}
	public class PageControlSettings: TabControlSettingsBase {
		MVCxTabPageCollection tabPages;
		SettingsLoadingPanel settingsLoadingPanel;
		public PageControlSettings() {
			this.tabPages = new MVCxTabPageCollection();
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			ActivateTabPageAction = ActivateTabPageAction.Click;
			ShowTabs = true;
		}
		public ActivateTabPageAction ActivateTabPageAction { get; set; }
		public object CallbackRouteValues { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanel { get { return SettingsLoadingPanel.Enabled; } set { SettingsLoadingPanel.Enabled = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanelImage { get { return SettingsLoadingPanel.ShowImage; } set { SettingsLoadingPanel.ShowImage = value; } }
		public bool ShowTabs { get; set; }
		public MVCxTabPageCollection TabPages { get { return tabPages; } }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public EventHandler BeforeGetCallbackResult { get { return BeforeGetCallbackResultInternal; } set { BeforeGetCallbackResultInternal = value; } }
		protected internal string ActiveTabTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> ActiveTabTemplateContentMethod { get; set; }
		protected internal string TabTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> TabTemplateContentMethod { get; set; }
		protected internal string ActiveTabTextTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> ActiveTabTextTemplateContentMethod { get; set; }
		protected internal string TabTextTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> TabTextTemplateContentMethod { get; set; }
		public void SetActiveTabTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			ActiveTabTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTemplateContent(string content) {
			ActiveTabTemplateContent = content;
		}
		public void SetTabTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			TabTemplateContentMethod = contentMethod;
		}
		public void SetTabTemplateContent(string content) {
			TabTemplateContent = content;
		}
		public void SetActiveTabTextTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			ActiveTabTextTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTextTemplateContent(string content) {
			ActiveTabTextTemplateContent = content;
		}
		public void SetTabTextTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			TabTextTemplateContentMethod = contentMethod;
		}
		public void SetTabTextTemplateContent(string content) {
			TabTextTemplateContent = content;
		}
	}
}
