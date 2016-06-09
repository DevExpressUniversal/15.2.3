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
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Internal;
	using System.Collections;
	public class NavBarExtension: ExtensionBase {
		public NavBarExtension(NavBarSettings settings)
			: base(settings) {
		}
		public NavBarExtension(NavBarSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxNavBar Control {
			get { return (MVCxNavBar)base.Control; }
		}
		protected internal new NavBarSettings Settings {
			get { return (NavBarSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowExpanding = Settings.AllowExpanding;
			Control.AllowSelectItem = Settings.AllowSelectItem;
			Control.AutoCollapse = Settings.AutoCollapse;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.EnableAnimation = Settings.EnableAnimation;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.EnableHotTrack = Settings.EnableHotTrack;
			Control.ExpandButtonPosition = Settings.ExpandButtonPosition;
			Control.Groups.Assign(Settings.Groups);
			Control.Images.CopyFrom(Settings.Images);
			Control.ItemLinkMode = Settings.ItemLinkMode;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ShowExpandButtons = Settings.ShowExpandButtons;
			Control.ShowGroupHeaders = Settings.ShowGroupHeaders;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.SyncSelectionMode = Settings.SyncSelectionMode;
			Control.Target = Settings.Target;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.GroupDataFields.HeaderImageUrlField = Settings.GroupDataFields.HeaderImageUrlField;
			Control.GroupDataFields.NameField = Settings.GroupDataFields.NameField;
			Control.GroupDataFields.NavigateUrlField = Settings.GroupDataFields.NavigateUrlField;
			Control.GroupDataFields.NavigateUrlFormatString = Settings.GroupDataFields.NavigateUrlFormatString;
			Control.GroupDataFields.TextField = Settings.GroupDataFields.TextField;
			Control.GroupDataFields.TextFormatString = Settings.GroupDataFields.TextFormatString;
			Control.GroupDataFields.ToolTipField = Settings.GroupDataFields.ToolTipField;
			Control.ItemDataFields.ImageUrlField = Settings.ItemDataFields.ImageUrlField;
			Control.ItemDataFields.NameField = Settings.ItemDataFields.NameField;
			Control.ItemDataFields.NavigateUrlField = Settings.ItemDataFields.NavigateUrlField;
			Control.ItemDataFields.NavigateUrlFormatString = Settings.ItemDataFields.NavigateUrlFormatString;
			Control.ItemDataFields.TextField = Settings.ItemDataFields.TextField;
			Control.ItemDataFields.TextFormatString = Settings.ItemDataFields.TextFormatString;
			Control.ItemDataFields.ToolTipField = Settings.ItemDataFields.ToolTipField;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.ClientLayout += Settings.ClientLayout;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.ItemDataBound += Settings.ItemDataBound;
			Control.GroupDataBound += Settings.GroupDataBound;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.GroupHeaderTemplate = ContentControlTemplate<NavBarGroupTemplateContainer>.Create(
				Settings.GroupHeaderTemplateContent, Settings.GroupHeaderTemplateContentMethod, 
				typeof(NavBarGroupTemplateContainer));
			Control.GroupHeaderTemplateCollapsed = ContentControlTemplate<NavBarGroupTemplateContainer>.Create(
				Settings.GroupHeaderTemplateCollapsedContent, Settings.GroupHeaderTemplateCollapsedContentMethod, 
				typeof(NavBarGroupTemplateContainer));
			Control.GroupContentTemplate = ContentControlTemplate<NavBarGroupTemplateContainer>.Create(
				Settings.GroupContentTemplateContent, Settings.GroupContentTemplateContentMethod, 
				typeof(NavBarGroupTemplateContainer));
			Control.ItemTemplate = ContentControlTemplate<NavBarItemTemplateContainer>.Create(
				Settings.ItemTemplateContent, Settings.ItemTemplateContentMethod, typeof(NavBarItemTemplateContainer));
			Control.ItemTextTemplate = ContentControlTemplate<NavBarItemTemplateContainer>.Create(
				Settings.ItemTextTemplateContent, Settings.ItemTextTemplateContentMethod, typeof(NavBarItemTemplateContainer));
			for (int i = 0; i < Control.Groups.Count; i++) {
				MVCxNavBarGroup sourceGroup = (i < Settings.Groups.Count) ? Settings.Groups[i] : null;
				if (sourceGroup == null) continue;
				Control.Groups[i].HeaderTemplate = ContentControlTemplate<NavBarGroupTemplateContainer>.Create(
					sourceGroup.HeaderTemplateContent, sourceGroup.HeaderTemplateContentMethod, 
					typeof(NavBarGroupTemplateContainer));
				Control.Groups[i].HeaderTemplateCollapsed = ContentControlTemplate<NavBarGroupTemplateContainer>.Create(
					sourceGroup.HeaderTemplateCollapsedContent, sourceGroup.HeaderTemplateCollapsedContentMethod, 
					typeof(NavBarGroupTemplateContainer));
				Control.Groups[i].ContentTemplate = ContentControlTemplate<NavBarGroupTemplateContainer>.Create(
					sourceGroup.ContentTemplateContent, sourceGroup.ContentTemplateContentMethod, 
					typeof(NavBarGroupTemplateContainer));
				Control.Groups[i].ItemTemplate = ContentControlTemplate<NavBarItemTemplateContainer>.Create(
					sourceGroup.ItemTemplateContent, sourceGroup.ItemTemplateContentMethod,
					typeof(NavBarItemTemplateContainer));
				Control.Groups[i].ItemTextTemplate = ContentControlTemplate<NavBarItemTemplateContainer>.Create(
					sourceGroup.ItemTextTemplateContent, sourceGroup.ItemTextTemplateContentMethod,
					typeof(NavBarItemTemplateContainer));
				for (int j = 0; j < Control.Groups[i].Items.Count; j++) {
					MVCxNavBarItem sourceItem = (sourceGroup != null && j < sourceGroup.Items.Count) ? sourceGroup.Items[j] : null;
					if (sourceItem == null) continue;
					Control.Groups[i].Items[j].Template = ContentControlTemplate<NavBarItemTemplateContainer>.Create(
						sourceItem.TemplateContent, sourceItem.TemplateContentMethod, typeof(NavBarItemTemplateContainer));
					Control.Groups[i].Items[j].TextTemplate = ContentControlTemplate<NavBarItemTemplateContainer>.Create(
						sourceItem.TextTemplateContent, sourceItem.TextTemplateContentMethod, typeof(NavBarItemTemplateContainer));
				}
			}
		}
		public NavBarExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public NavBarExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public NavBarExtension BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public NavBarExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty);
		}
		public NavBarExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public NavBarExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected internal override void PrepareControlProperties() {
			base.PrepareControlProperties();
			Control.ValidateProperties();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.EnsureClientStateLoaded();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		public static NavBarState GetState(string name) {
			return MVCxNavBar.GetState(name);
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxNavBar();
		}
	}
}
