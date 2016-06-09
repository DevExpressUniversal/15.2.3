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
using DevExpress.Web;
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public class MenuSettingsBase : SettingsBase {
		MVCxMenuItemCollection items;
		ItemSubMenuOffset itemSubMenuOffset;
		ItemSubMenuOffset rootItemSubMenuOffset;
		public MenuSettingsBase() {
			this.items = new MVCxMenuItemCollection();
			this.itemSubMenuOffset = new ItemSubMenuOffset();
			this.rootItemSubMenuOffset = new ItemSubMenuOffset();
			AppearAfter = 300;
			AutoSeparators = AutoSeparatorMode.None;
			BorderBetweenItemAndSubMenu = BorderBetweenItemAndSubMenuMode.ShowAll;
			DisappearAfter = 500;
			EnableHotTrack = true;
			ItemLinkMode = ItemLinkMode.ContentBounds;
			MaximumDisplayLevels = 0;
			RenderIFrameForPopupElements = DefaultBoolean.Default;
			ShowPopOutImages = DefaultBoolean.Default;
			SyncSelectionMode = SyncSelectionMode.CurrentPathAndQuery;
			EnableScrollingInternal = false;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public bool AllowSelectItem { get; set; }
		public int AppearAfter { get; set; }
		public AutoSeparatorMode AutoSeparators { get; set; }
		public BorderBetweenItemAndSubMenuMode BorderBetweenItemAndSubMenu { get; set; }
		public MenuClientSideEvents ClientSideEvents { get { return (MenuClientSideEvents)ClientSideEventsInternal; } }
		public new MenuStyle ControlStyle { get { return (MenuStyle)base.ControlStyle; } }
		public int DisappearAfter { get; set; }
		public bool EnableAnimation { get; set; }
		public bool EnableHotTrack { get; set; }
		protected internal bool EnableScrollingInternal { get; set; }
		public MenuImages Images { get { return (MenuImages)ImagesInternal; } }
		public string ImageUrlField { get; set; }
		public ItemLinkMode ItemLinkMode { get; set; }
		public MVCxMenuItemCollection Items { get { return items; } }
		public ItemSubMenuOffset ItemSubMenuOffset { get { return itemSubMenuOffset; } }
		public int MaximumDisplayLevels { get; set; }
		public string NameField { get; set; }
		public string NavigateUrlField { get; set; }
		public string NavigateUrlFormatString { get; set; }
		public DefaultBoolean RenderIFrameForPopupElements { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public ItemSubMenuOffset RootItemSubMenuOffset { get { return rootItemSubMenuOffset; } }
		public bool SelectParentItem { get; set; }
		public DefaultBoolean ShowPopOutImages { get; set; }
		public MenuStyles Styles { get { return (MenuStyles)StylesInternal; } }
		public SyncSelectionMode SyncSelectionMode { get; set; }
		public string Target { get; set; }
		public string TextField { get; set; }
		public string TextFormatString { get; set; }
		public string ToolTipField { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public MenuItemEventHandler ItemDataBound { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		protected internal string ItemTemplateContent { get; set; }
		protected internal Action<MenuItemTemplateContainer> ItemTemplateContentMethod { get; set; }
		protected internal string ItemTextTemplateContent { get; set; }
		protected internal Action<MenuItemTemplateContainer> ItemTextTemplateContentMethod { get; set; }
		protected internal string SubMenuTemplateContent { get; set; }
		protected internal Action<MenuItemTemplateContainer> SubMenuTemplateContentMethod { get; set; }
		public void SetItemTemplateContent(Action<MenuItemTemplateContainer> contentMethod) {
			ItemTemplateContentMethod = contentMethod;
		}
		public void SetItemTemplateContent(string content) {
			ItemTemplateContent = content;
		}
		public void SetItemTextTemplateContent(Action<MenuItemTemplateContainer> contentMethod) {
			ItemTextTemplateContentMethod = contentMethod;
		}
		public void SetItemTextTemplateContent(string content) {
			ItemTextTemplateContent = content;
		}
		public void SetSubMenuTemplateContent(Action<MenuItemTemplateContainer> contentMethod) {
			SubMenuTemplateContentMethod = contentMethod;
		}
		public void SetSubMenuTemplateContent(string content) {
			SubMenuTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MenuClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new MenuStyle();
		}
		protected override ImagesBase CreateImages() {
			return new MenuImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new MenuStyles(null);
		}
	}
	public class MenuSettings : MenuSettingsBase {
		public MenuSettings()
			: base() {
			Orientation = System.Web.UI.WebControls.Orientation.Horizontal;
		}
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public bool EnableAdaptivity { get; set; }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public bool EnableSubMenuFullWidth { get; set; }
		public bool EnableSubMenuScrolling { get; set; }
		public bool ItemAutoWidth { get; set; }
		public ImagePosition ItemImagePosition { get; set; }
		public System.Web.UI.WebControls.Orientation Orientation { get; set; }
		public bool ShowAsToolbar { get; set; }
	}
	public class PopupMenuSettings : MenuSettingsBase {
		public PopupMenuSettings()
			: base() {
			CloseAction = PopupMenuCloseAction.OuterMouseClick;
			EnableScrolling = false;
			PopupAction = PopupAction.RightMouseClick;
			PopupElementID = "";
			PopupHorizontalAlign = PopupHorizontalAlign.NotSet;
			PopupVerticalAlign = PopupVerticalAlign.NotSet;
			PopupHorizontalOffset = 0;
			PopupVerticalOffset = 0;
			Left = 0;
			Top = 0;
		}
		public PopupMenuCloseAction CloseAction { get; set; }
		public bool EnableScrolling {
			get { return EnableScrollingInternal; }
			set { EnableScrollingInternal = value; }
		}
		public PopupAction PopupAction { get; set; }
		public string PopupElementID { get; set; }
		public PopupHorizontalAlign PopupHorizontalAlign { get; set; }
		public int PopupHorizontalOffset { get; set; }
		public PopupVerticalAlign PopupVerticalAlign { get; set; }
		public int PopupVerticalOffset { get; set; }
		public int Left { get; set; }
		public int Top { get; set; }
		public EventHandler<ControlResolveEventArgs> PopupElementResolve { get; set; }
	}
}
