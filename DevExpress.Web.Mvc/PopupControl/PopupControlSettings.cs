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
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	public abstract class PopupControlSettingsBase : SettingsBase {
		SettingsLoadingPanel settingsLoadingPanel;
		public PopupControlSettingsBase() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			AllowDragging = true;
			AllowResize = false;
			DragElement = DragElement.Header;
			EnableHotTrack = true;
			FooterText = DefaultPopupWindow.DefaultFooterText;
			HeaderText = DefaultPopupWindow.DefaultHeaderText;
			Opacity = AppearanceStyleBase.DefaultOpacity;
			PopupAnimationType = AnimationType.Auto;
			CloseAnimationType = AnimationType.None;
			RenderIFrameForPopupElements = DefaultBoolean.Default;
			ResizingMode = ResizingMode.Live;
			SaveStateToCookies = false;
			SaveStateToCookiesID = String.Empty;
			ScrollBars = ScrollBars.None;
			ShowCloseButton = true;
			ShowPinButton = false;
			ShowRefreshButton = false;
			ShowCollapseButton = false;
			ShowMaximizeButton = false;
			ShowFooter = ASPxPopupControl.DefaultShowFooter;
			ShowHeader = ASPxPopupControl.DefaultShowHeader;
			ShowShadow = true;
			ShowSizeGrip = ShowSizeGrip.Auto;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public bool AllowDragging { get; set; }
		public bool AllowResize { get; set; }
		public object CallbackRouteValues { get; set; }
		public bool Collapsed { get; set; }
		public new AppearanceStyle ControlStyle { get { return (AppearanceStyle)base.ControlStyle; } }
		public string ContentUrl { get; set; }
		public DragElement DragElement { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the PopupAnimationType property instead")]
		public bool EnableAnimation { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public bool EnableHotTrack { get; set; }
		public string FooterNavigateUrl { get; set; }
		public string FooterText { get; set; }
		public string HeaderNavigateUrl { get; set; }
		public string HeaderText { get; set; }
		public PopupControlImages Images { get { return (PopupControlImages)ImagesInternal; } }
		public int Left { get; set; }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public bool Maximized { get; set; }
		public Unit MaxHeight { get; set; }
		public Unit MaxWidth { get; set; }
		public Unit MinHeight { get; set; }
		public Unit MinWidth { get; set; }
		public int Opacity { get; set; }
		public bool Pinned { get; set; }
		public AnimationType PopupAnimationType { get; set; }
		public AnimationType CloseAnimationType { get; set; }
		public DefaultBoolean RenderIFrameForPopupElements { get; set; }
		public ResizingMode ResizingMode { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public ScrollBars ScrollBars { get; set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowCloseButton { get; set; }
		public bool ShowPinButton { get; set; }
		public bool ShowRefreshButton { get; set; }
		public bool ShowCollapseButton { get; set; }
		public bool ShowMaximizeButton { get; set; }
		public bool ShowHeader { get; set; }
		public bool ShowFooter { get; set; }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanel { get { return SettingsLoadingPanel.Enabled; } set { SettingsLoadingPanel.Enabled = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLoadingPanelImage { get { return SettingsLoadingPanel.ShowImage; } set { SettingsLoadingPanel.ShowImage = value; } }
		public bool ShowOnPageLoad { get; set; }
		public bool ShowShadow { get; set; }
		public ShowSizeGrip ShowSizeGrip { get; set; }
		public PopupControlStyles Styles { get { return (PopupControlStyles)StylesInternal; } }
		public string Target { get; set; }
		public string Text { get; set; }
		public int Top { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> HeaderContentTemplateContentMethod { get; set; }
		protected internal string FooterTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> FooterTemplateContentMethod { get; set; }
		protected internal string FooterContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> FooterContentTemplateContentMethod { get; set; }
		protected internal string Content { get; set; }
		protected internal Action ContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			HeaderContentTemplateContentMethod = contentMethod;
		}
		public void SetHeaderContentTemplateContent(string content) {
			HeaderContentTemplateContent = content;
		}
		public void SetFooterTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			FooterTemplateContentMethod = contentMethod;
		}
		public void SetFooterTemplateContent(string content) {
			FooterTemplateContent = content;
		}
		public void SetFooterContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			FooterContentTemplateContentMethod = contentMethod;
		}
		public void SetFooterContentTemplateContent(string content) {
			FooterContentTemplateContent = content;
		}
		public void SetContent(Action contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override ImagesBase CreateImages() {
			return new PopupControlImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new PopupControlStyles(null);
		}
	}
	public class PopupControlSettings : PopupControlSettingsBase {
		MVCxPopupWindowCollection windows;
		public PopupControlSettings() {
			this.windows = new MVCxPopupWindowCollection();
			AppearAfter = ASPxPopupControl.DefaultAppearAfter;
			CloseAction = CloseAction.OuterMouseClick;
			CloseOnEscape = false;
			DisappearAfter = ASPxPopupControl.DefaultDisappearAfter;
			PopupAnimationType = AnimationType.Auto;
			PopupAlignCorrection = PopupAlignCorrection.Auto;
			PopupAction = PopupAction.LeftMouseClick;
		}
		public int AppearAfter { get; set; }
		public bool AutoUpdatePosition { get; set; }
		public PopupControlClientSideEvents ClientSideEvents { get { return (PopupControlClientSideEvents)ClientSideEventsInternal; } }
		public CloseAction CloseAction { get; set; }
		public bool CloseOnEscape { get; set; }
		public int DisappearAfter { get; set; }
		public bool Modal { get; set; }
		public PopupAction PopupAction { get; set; }
		public string PopupElementID { get; set; }
		public PopupHorizontalAlign PopupHorizontalAlign { get; set; }
		public int PopupHorizontalOffset { get; set; }
		public PopupVerticalAlign PopupVerticalAlign { get; set; }
		public int PopupVerticalOffset { get; set; }
		public PopupAlignCorrection PopupAlignCorrection { get; set; }
		public bool ShowPageScrollbarWhenModal { get; set; }
		public MVCxPopupWindowCollection Windows { get { return windows; } }
		public LoadContentViaCallback LoadContentViaCallback { get; set; }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public PopupWindowEventHandler WindowDataBound { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		protected internal string WindowContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> WindowContentTemplateContentMethod { get; set; }
		protected internal string WindowHeaderTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> WindowHeaderTemplateContentMethod { get; set; }
		protected internal string WindowHeaderContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> WindowHeaderContentTemplateContentMethod { get; set; } 
		protected internal string WindowFooterTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> WindowFooterTemplateContentMethod { get; set; }
		protected internal string WindowFooterContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> WindowFooterContentTemplateContentMethod { get; set; } 
		public void SetWindowContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			WindowContentTemplateContentMethod = contentMethod;
		}
		public void SetWindowContentTemplateContent(string content) {
			WindowContentTemplateContent = content;
		}
		public void SetWindowHeaderTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			WindowHeaderTemplateContentMethod = contentMethod;
		}
		public void SetWindowHeaderTemplateContent(string content) {
			WindowHeaderTemplateContent = content;
		}
		public void SetWindowHeaderContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			WindowHeaderContentTemplateContentMethod = contentMethod;
		}
		public void SetWindowHeaderContentTemplateContent(string content) {
			WindowHeaderContentTemplateContent = content;
		}
		public void SetWindowFooterTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			WindowFooterTemplateContentMethod = contentMethod;
		}
		public void SetWindowFooterTemplateContent(string content) {
			WindowFooterTemplateContent = content;
		}
		public void SetWindowFooterContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			WindowFooterContentTemplateContentMethod = contentMethod;
		}
		public void SetWindowFooterContentTemplateContent(string content) {
			WindowFooterContentTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PopupControlClientSideEvents();
		}
	}
}
