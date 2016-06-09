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
using System.Web.UI;
using System.Web.Mvc;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public abstract class PopupControlExtensionBase : ExtensionBase {
		public PopupControlExtensionBase(PopupControlSettingsBase settings)
			: base(settings) {
		}
		public PopupControlExtensionBase(PopupControlSettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxPopupControlBase Control {
			get { return (ASPxPopupControlBase)base.Control; }
		}
		protected internal new PopupControlSettingsBase Settings {
			get { return (PopupControlSettingsBase)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowDragging = Settings.AllowDragging;
			Control.AllowResize = Settings.AllowResize;
			Control.Collapsed = Settings.Collapsed;
			Control.ContentUrl = Settings.ContentUrl;
			Control.DragElement = Settings.DragElement;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.EnableHotTrack = Settings.EnableHotTrack;
			Control.FooterNavigateUrl = Settings.FooterNavigateUrl;
			Control.FooterText = Settings.FooterText;
			Control.HeaderNavigateUrl = Settings.HeaderNavigateUrl;
			Control.HeaderText = Settings.HeaderText;
			Control.Left = Settings.Left;
			Control.Maximized = Settings.Maximized;
			Control.MaxHeight = Settings.MaxHeight;
			Control.MaxWidth = Settings.MaxWidth;
			Control.MinHeight = Settings.MinHeight;
			Control.MinWidth = Settings.MinWidth;
			Control.Opacity = Settings.Opacity;
			Control.Pinned = Settings.Pinned;
			Control.PopupAnimationType = Settings.PopupAnimationType;
			Control.CloseAnimationType = Settings.CloseAnimationType;
			Control.RenderIFrameForPopupElements = Settings.RenderIFrameForPopupElements;
			Control.ResizingMode = Settings.ResizingMode;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ShowCloseButton = Settings.ShowCloseButton;
			Control.ShowPinButton = Settings.ShowPinButton;
			Control.ShowRefreshButton = Settings.ShowRefreshButton;
			Control.ShowCollapseButton = Settings.ShowCollapseButton;
			Control.ShowMaximizeButton = Settings.ShowMaximizeButton;
			Control.ShowHeader = Settings.ShowHeader;
			Control.ShowFooter = Settings.ShowFooter;
			Control.ShowOnPageLoad = Settings.ShowOnPageLoad;
			Control.ShowShadow = Settings.ShowShadow;
			Control.ShowSizeGrip = Settings.ShowSizeGrip;
			Control.Target = Settings.Target;
			Control.Text = Settings.Text;
			Control.Top = Settings.Top;
			Control.ScrollBars = Settings.ScrollBars;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.HeaderTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.HeaderTemplateContent, Settings.HeaderTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.FooterTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.FooterTemplateContent, Settings.FooterTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.HeaderContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.HeaderContentTemplateContent, Settings.HeaderContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.FooterContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.FooterContentTemplateContent, Settings.FooterContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.Controls.Add(DevExpress.Web.Mvc.Internal.ContentControl.Create(Settings.Content, Settings.ContentMethod));
		}
	}
	public class PopupControlExtension : PopupControlExtensionBase {
		public PopupControlExtension(PopupControlSettings settings)
			: base(settings) {
		}
		public PopupControlExtension(PopupControlSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxPopupControl Control {
			get { return (MVCxPopupControl)base.Control; }
		}
		protected internal new PopupControlSettings Settings {
			get { return (PopupControlSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.AppearAfter = Settings.AppearAfter;
			Control.AutoUpdatePosition = Settings.AutoUpdatePosition;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.CloseAction = Settings.CloseAction;
			Control.CloseOnEscape = Settings.CloseOnEscape;
			Control.DisappearAfter = Settings.DisappearAfter;
			Control.Images.CopyFrom(Settings.Images);
			Control.Modal = Settings.Modal;
			Control.PopupAction = Settings.PopupAction;
			Control.PopupElementID = Settings.PopupElementID;
			Control.PopupHorizontalAlign = Settings.PopupHorizontalAlign;
			Control.PopupHorizontalOffset = Settings.PopupHorizontalOffset;
			Control.PopupVerticalAlign = Settings.PopupVerticalAlign;
			Control.PopupVerticalOffset = Settings.PopupVerticalOffset;
			Control.PopupAlignCorrection = Settings.PopupAlignCorrection;
			Control.ShowPageScrollbarWhenModal = Settings.ShowPageScrollbarWhenModal;
			Control.ShowShadow = Settings.ShowShadow;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.Windows.Assign(Settings.Windows);
			Control.LoadContentViaCallback = Settings.LoadContentViaCallback;
			Control.ClientLayout += Settings.ClientLayout;
			Control.WindowDataBound += Settings.WindowDataBound;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
			base.AssignInitialProperties();
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.WindowContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.WindowContentTemplateContent, Settings.WindowContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.WindowHeaderTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.WindowHeaderTemplateContent, Settings.WindowHeaderTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.WindowHeaderContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.WindowHeaderContentTemplateContent, Settings.WindowHeaderContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.WindowFooterTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.WindowFooterTemplateContent, Settings.WindowFooterTemplateContentMethod, typeof(PopupControlTemplateContainer));
			Control.WindowFooterContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
				Settings.WindowFooterContentTemplateContent, Settings.WindowFooterContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
			for(int i = 0; i < Control.Windows.Count; i++) {
				MVCxPopupWindow sourceWindow = (i < Settings.Windows.Count) ? Settings.Windows[i] : null;
				if(sourceWindow == null) continue;
				Control.Windows[i].ContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
					sourceWindow.ContentTemplateContent, sourceWindow.ContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
				Control.Windows[i].HeaderTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
					sourceWindow.HeaderTemplateContent, sourceWindow.HeaderTemplateContentMethod, typeof(PopupControlTemplateContainer));
				Control.Windows[i].HeaderContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
					sourceWindow.HeaderContentTemplateContent, sourceWindow.HeaderContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
				Control.Windows[i].FooterTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
					sourceWindow.FooterTemplateContent, sourceWindow.FooterTemplateContentMethod, typeof(PopupControlTemplateContainer));
				Control.Windows[i].FooterContentTemplate = ContentControlTemplate<PopupControlTemplateContainer>.Create(
					sourceWindow.FooterContentTemplateContent, sourceWindow.FooterContentTemplateContentMethod, typeof(PopupControlTemplateContainer));
				Control.Windows[i].Controls.Add(DevExpress.Web.Mvc.Internal.ContentControl.Create(sourceWindow.Content, sourceWindow.ContentMethod));
			}
		}
		public PopupControlExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public PopupControlExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public PopupControlExtension BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public PopupControlExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty);
		}
		public PopupControlExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public PopupControlExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.EnsureClientStateLoaded();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxPopupControl();
		}
	}
}
