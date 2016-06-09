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
	public class NavBarSettings: SettingsBase {
		MVCxNavBarGroupCollection groups;
		NavBarGroupDataFields groupDataFields;
		NavBarItemDataFields itemDataFields;
		SettingsLoadingPanel settingsLoadingPanel;
		public NavBarSettings() {
			this.groups = new MVCxNavBarGroupCollection();
			this.groupDataFields = new NavBarGroupDataFields(null);
			this.itemDataFields = new NavBarItemDataFields(null);
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			AllowExpanding = true;
			EnableHotTrack = true;
			ExpandButtonPosition = ExpandButtonPosition.Default;
			ItemLinkMode = ItemLinkMode.ContentBounds;
			ShowExpandButtons = true;
			ShowGroupHeaders = true;
			SyncSelectionMode = SyncSelectionMode.CurrentPathAndQuery;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public bool AllowExpanding { get; set; }
		public bool AllowSelectItem { get; set; }
		public bool AutoCollapse { get; set; }
		public object CallbackRouteValues { get; set; }
		public NavBarClientSideEvents ClientSideEvents { get { return (NavBarClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new NavBarStyle ControlStyle { get { return (NavBarStyle)base.ControlStyle; } }
		public bool EnableAnimation { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public bool EnableHotTrack { get; set; }
		public ExpandButtonPosition ExpandButtonPosition { get; set; }
		public MVCxNavBarGroupCollection Groups { get { return groups; } }
		public NavBarGroupDataFields GroupDataFields { get { return groupDataFields; } }
		public NavBarImages Images { get { return (NavBarImages)ImagesInternal; } }
		public ItemLinkMode ItemLinkMode { get; set; }
		public NavBarItemDataFields ItemDataFields { get { return itemDataFields; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowExpandButtons { get; set; }
		public bool ShowGroupHeaders { get; set; }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanel { get { return SettingsLoadingPanel.Enabled; } set { SettingsLoadingPanel.Enabled = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanelImage { get { return SettingsLoadingPanel.ShowImage; } set { SettingsLoadingPanel.ShowImage = value; } }
		public NavBarStyles Styles { get { return (NavBarStyles)StylesInternal; } }
		public SyncSelectionMode SyncSelectionMode { get; set; }
		public string Target { get; set; }
		public EventHandler BeforeGetCallbackResult { get { return BeforeGetCallbackResultInternal; } set { BeforeGetCallbackResultInternal = value; } }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public NavBarItemEventHandler ItemDataBound { get; set; }
		public NavBarGroupEventHandler GroupDataBound { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		protected internal string GroupContentTemplateContent { get; set; }
		protected internal Action<NavBarGroupTemplateContainer> GroupContentTemplateContentMethod { get; set; }
		protected internal string GroupHeaderTemplateContent { get; set; }
		protected internal Action<NavBarGroupTemplateContainer> GroupHeaderTemplateContentMethod { get; set; }
		protected internal string GroupHeaderTemplateCollapsedContent { get; set; }
		protected internal Action<NavBarGroupTemplateContainer> GroupHeaderTemplateCollapsedContentMethod { get; set; }
		protected internal string ItemTemplateContent { get; set; }
		protected internal Action<NavBarItemTemplateContainer> ItemTemplateContentMethod { get; set; }
		protected internal string ItemTextTemplateContent { get; set; }
		protected internal Action<NavBarItemTemplateContainer> ItemTextTemplateContentMethod { get; set; }
		public void SetGroupContentTemplateContent(Action<NavBarGroupTemplateContainer> contentMethod) {
			GroupContentTemplateContentMethod = contentMethod;
		}
		public void SetGroupContentTemplateContent(string content) {
			GroupContentTemplateContent = content;
		}
		public void SetGroupHeaderTemplateContent(Action<NavBarGroupTemplateContainer> contentMethod) {
			GroupHeaderTemplateContentMethod = contentMethod;
		}
		public void SetGroupHeaderTemplateContent(string content) {
			GroupHeaderTemplateContent = content;
		}
		public void SetGroupHeaderTemplateCollapsedContent(Action<NavBarGroupTemplateContainer> contentMethod) {
			GroupHeaderTemplateCollapsedContentMethod = contentMethod;
		}
		public void SetGroupHeaderTemplateCollapsedContent(string content) {
			GroupHeaderTemplateCollapsedContent = content;
		}
		public void SetItemTemplateContent(Action<NavBarItemTemplateContainer> contentMethod) {
			ItemTemplateContentMethod = contentMethod;
		}
		public void SetItemTemplateContent(string content) {
			ItemTemplateContent = content;
		}
		public void SetItemTextTemplateContent(Action<NavBarItemTemplateContainer> contentMethod) {
			ItemTextTemplateContentMethod = contentMethod;
		}
		public void SetItemTextTemplateContent(string content) {
			ItemTextTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new NavBarClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new NavBarStyle();
		}
		protected override ImagesBase CreateImages() {
			return new NavBarImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new NavBarStyles(null);
		}
	}
}
