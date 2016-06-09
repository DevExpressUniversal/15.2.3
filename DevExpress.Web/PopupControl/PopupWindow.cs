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
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class PopupWindow : ContentControlCollectionItem {
		protected internal const string DefaultContentUrl = "";
		protected internal const bool DefaultIsDragged = false;
		protected internal const bool DefaultIsResized = false;
		protected internal const int DefaultZIndex = -1;
		protected internal const int DefaultLeft = 0;
		protected internal const int DefaultTop = 0;
		protected internal const bool DefaultShowOnPageLoad = false;
		private object fDataItem = null;
		private bool fIsDragged = false;
		private bool isResized = false;
		private int fZIndex = DefaultZIndex;
		private HeaderButtonImageProperties fCloseButtonImage = null;
		private HeaderButtonCheckedImageProperties pinButtonImage = null;
		private HeaderButtonImageProperties refreshButtonImage = null;
		private HeaderButtonCheckedImageProperties collapseButtonImage = null;
		private HeaderButtonCheckedImageProperties maxButtonImage = null;
		private ImageProperties fFooterImage = null;
		private ImageProperties fHeaderImage = null;
		private ImageProperties fSizeGripImage = null;
		ImageProperties sizeGripRtlImage;
		private PopupWindowContentStyle fContentStyle = null;
		private PopupWindowFooterStyle fFooterStyle = null;
		private PopupWindowStyle fHeaderStyle = null;
		private PopupWindowButtonStyle fCloseButtonStyle = null;
		private PopupWindowButtonStyle fPinButtonStyle = null;
		private PopupWindowButtonStyle fRefreshButtonStyle = null;
		private PopupWindowButtonStyle fCollapseButtonStyle = null;
		private PopupWindowButtonStyle fMaximizeButtonStyle = null;
		private ITemplate fContentTemplate = null;
		private ITemplate fFooterTemplate = null;
		private ITemplate fHeaderTemplate = null;
		private ITemplate fFooterContentTemplate = null;
		private ITemplate fHeaderContentTemplate = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowCloseButton"),
#endif
		DefaultValue(DefaultBoolean.Default), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean ShowCloseButton {
			get { return (DefaultBoolean)GetEnumProperty("ShowCloseButton", DefaultBoolean.Default); }
			set {
				SetEnumProperty("ShowCloseButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowPinButton"),
#endif
		DefaultValue(DefaultBoolean.Default), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean ShowPinButton {
			get { return (DefaultBoolean)GetEnumProperty("ShowPinButton", DefaultBoolean.Default); }
			set {
				SetEnumProperty("ShowPinButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowRefreshButton"),
#endif
		DefaultValue(DefaultBoolean.Default), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean ShowRefreshButton {
			get { return (DefaultBoolean)GetEnumProperty("ShowRefreshButton", DefaultBoolean.Default); }
			set {
				SetEnumProperty("ShowRefreshButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowCollapseButton"),
#endif
		DefaultValue(DefaultBoolean.Default), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean ShowCollapseButton {
			get { return (DefaultBoolean)GetEnumProperty("ShowCollapseButton", DefaultBoolean.Default); }
			set {
				SetEnumProperty("ShowCollapseButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowMaximizeButton"),
#endif
		DefaultValue(DefaultBoolean.Default), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean ShowMaximizeButton {
			get { return (DefaultBoolean)GetEnumProperty("ShowMaximizeButton", DefaultBoolean.Default); }
			set {
				SetEnumProperty("ShowMaximizeButton", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowPinned"),
#endif
		DefaultValue(false), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public bool Pinned {
			get { return GetBoolProperty("Pinned", false); }
			set { SetBoolProperty("Pinned", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCollapsed"),
#endif
		DefaultValue(false), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public bool Collapsed {
			get { return GetBoolProperty("Collapsed", false); }
			set { SetBoolProperty("Collapsed", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMaximized"),
#endif
		DefaultValue(false), Category("Appearance"), NotifyParentProperty(true), AutoFormatDisable]
		public bool Maximized {
			get { return GetBoolProperty("Maximized", false); }
			set { SetBoolProperty("Maximized", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowPopupAction"),
#endif
		DefaultValue(WindowPopupAction.Default), NotifyParentProperty(true), AutoFormatDisable]
		public WindowPopupAction PopupAction {
			get { return (WindowPopupAction)GetEnumProperty("PopupAction", WindowPopupAction.Default); }
			set { SetEnumProperty("PopupAction", WindowPopupAction.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCloseAction"),
#endif
		DefaultValue(WindowCloseAction.Default), NotifyParentProperty(true), AutoFormatDisable]
		public WindowCloseAction CloseAction {
			get { return (WindowCloseAction)GetEnumProperty("CloseAction", WindowCloseAction.Default); }
			set { SetEnumProperty("CloseAction", WindowCloseAction.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCloseOnEscape"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean CloseOnEscape {
			get { return GetDefaultBooleanProperty("CloseOnEscape", DefaultBoolean.Default); }
			set { SetDefaultBooleanProperty("CloseOnEscape", DefaultBoolean.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowContentUrl"),
#endif
		DefaultValue(DefaultContentUrl), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string ContentUrl {
			get { return GetStringProperty("ContentUrl", DefaultContentUrl); }
			set {
				SetStringProperty("ContentUrl", DefaultContentUrl, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowContentUrlIFrameTitle"),
#endif
		NotifyParentProperty(true), Localizable(true), DefaultValue("")]
		public string ContentUrlIFrameTitle {
			get { return GetStringProperty("ContentUrlIFrameTitle", ""); }
			set { SetStringProperty("ContentUrlIFrameTitle", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowEnabled"),
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
	DevExpressWebLocalizedDescription("PopupWindowFooterNavigateUrl"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string FooterNavigateUrl {
			get { return GetStringProperty("FooterNavigateUrl", ""); }
			set {
				SetStringProperty("FooterNavigateUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowHeaderNavigateUrl"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string HeaderNavigateUrl {
			get { return GetStringProperty("HeaderNavigateUrl", ""); }
			set {
				SetStringProperty("HeaderNavigateUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowFooterText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string FooterText {
			get { return GetStringProperty("FooterText", ""); }
			set { SetStringProperty("FooterText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowHeaderText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string HeaderText {
			get { return GetStringProperty("HeaderText", ""); }
			set { SetStringProperty("HeaderText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowLeft"),
#endif
		DefaultValue(DefaultLeft), NotifyParentProperty(true), AutoFormatDisable]
		public int Left {
			get { return GetIntProperty("Left", DefaultLeft); }
			set { SetIntProperty("Left", DefaultLeft, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowModal"),
#endif
		DefaultValue(false), NotifyParentProperty(true), Localizable(false)]
		public bool Modal {
			get { return GetBoolProperty("Modal", false); }
			set {
				SetBoolProperty("Modal", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowPageScrollbarWhenModal"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowPageScrollbarWhenModal {
			get { return GetBoolProperty("ShowPageScrollbarWhenModal", false); }
			set { SetBoolProperty("ShowPageScrollbarWhenModal", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowAutoUpdatePosition"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool AutoUpdatePosition {
			get { return GetBoolProperty("AutoUpdatePosition", false); }
			set { SetBoolProperty("AutoUpdatePosition", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowPopupElementID"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public string PopupElementID {
			get { return GetStringProperty("PopupElementID", ""); }
			set { SetStringProperty("PopupElementID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowOnPageLoad"),
#endif
		DefaultValue(DefaultShowOnPageLoad), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool ShowOnPageLoad {
			get { return GetBoolProperty("ShowOnPageLoad", DefaultShowOnPageLoad); }
			set { 
				SetBoolProperty("ShowOnPageLoad", DefaultShowOnPageLoad, value); 
				if (!value) IsDragged = false;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowFooter"),
#endif
		Category("Appearance"), DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean ShowFooter {
			get { return GetDefaultBooleanProperty("ShowFooter", DefaultBoolean.Default); }
			set {
				SetDefaultBooleanProperty("ShowFooter", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowShowHeader"),
#endif
		Category("Appearance"), DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean ShowHeader {
			get { return GetDefaultBooleanProperty("ShowHeader", DefaultBoolean.Default); }
			set {
				SetDefaultBooleanProperty("ShowHeader", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowTarget"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true),
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set { SetStringProperty("Text", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowTop"),
#endif
		DefaultValue(DefaultTop), NotifyParentProperty(true), AutoFormatDisable]
		public int Top {
			get { return GetIntProperty("Top", DefaultTop); }
			set { SetIntProperty("Top", DefaultTop, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMinWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit MinWidth
		{
			get { return GetUnitProperty("MinWidth", Unit.Empty); }
			set { SetUnitProperty("MinWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMinHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit MinHeight
		{
			get { return GetUnitProperty("MinHeight", Unit.Empty); }
			set { SetUnitProperty("MinHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMaxWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit MaxWidth
		{
			get { return GetUnitProperty("MaxWidth", Unit.Empty); }
			set { SetUnitProperty("MaxWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMaxHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public virtual Unit MaxHeight
		{
			get { return GetUnitProperty("MaxHeight", Unit.Empty); }
			set { SetUnitProperty("MaxHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCloseButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties CloseButtonImage {
			get {
				if (fCloseButtonImage == null)
					fCloseButtonImage = new HeaderButtonImageProperties(this);
				return fCloseButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowPinButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties PinButtonImage {
			get {
				if(pinButtonImage == null)
					pinButtonImage = new HeaderButtonCheckedImageProperties(this);
				return pinButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowRefreshButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties RefreshButtonImage {
			get {
				if(refreshButtonImage == null)
					refreshButtonImage = new HeaderButtonImageProperties(this);
				return refreshButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCollapseButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties CollapseButtonImage {
			get {
				if(collapseButtonImage == null)
					collapseButtonImage = new HeaderButtonCheckedImageProperties(this);
				return collapseButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMaximizeButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties MaximizeButtonImage {
			get {
				if(maxButtonImage == null)
					maxButtonImage = new HeaderButtonCheckedImageProperties(this);
				return maxButtonImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowFooterImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FooterImage {
			get {
				if (fFooterImage == null)
					fFooterImage = new ImageProperties(this);
				return fFooterImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowHeaderImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties HeaderImage {
			get {
				if (fHeaderImage == null)
					fHeaderImage = new ImageProperties(this);
				return fHeaderImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowSizeGripImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SizeGripImage {
			get {
				if (fSizeGripImage == null)
					fSizeGripImage = new ImageProperties(this);
				return fSizeGripImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowSizeGripRtlImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SizeGripRtlImage {
			get {
				if (sizeGripRtlImage == null)
					sizeGripRtlImage = new ImageProperties(this);
				return sizeGripRtlImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCloseButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle CloseButtonStyle {
			get {
				if (fCloseButtonStyle == null)
					fCloseButtonStyle = new PopupWindowButtonStyle();
				return fCloseButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowPinButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle PinButtonStyle {
			get {
				if(fPinButtonStyle == null)
					fPinButtonStyle = new PopupWindowButtonStyle();
				return fPinButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowRefreshButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle RefreshButtonStyle {
			get {
				if(fRefreshButtonStyle == null)
					fRefreshButtonStyle = new PopupWindowButtonStyle();
				return fRefreshButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowCollapseButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle CollapseButtonStyle {
			get {
				if(fCollapseButtonStyle == null)
					fCollapseButtonStyle = new PopupWindowButtonStyle();
				return fCollapseButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowMaximizeButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowButtonStyle MaximizeButtonStyle {
			get {
				if(fMaximizeButtonStyle == null)
					fMaximizeButtonStyle = new PopupWindowButtonStyle();
				return fMaximizeButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowContentStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowContentStyle ContentStyle {
			get {
				if (fContentStyle == null)
					fContentStyle = new PopupWindowContentStyle();
				return fContentStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowFooterStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowFooterStyle FooterStyle {
			get {
				if (fFooterStyle == null)
					fFooterStyle = new PopupWindowFooterStyle();
				return fFooterStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupWindowHeaderStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowStyle HeaderStyle {
			get {
				if (fHeaderStyle == null)
					fHeaderStyle = new PopupWindowStyle();
				return fHeaderStyle;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return fDataItem; }
			set { SetDataItem(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ASPxPopupControlBase PopupControl {
			get { return ((Collection as PopupWindowCollection) != null) ? (Collection as PopupWindowCollection).PopupControl : null; }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ContentTemplate {
			get { return fContentTemplate; }
			set {
				fContentTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterTemplate {
			get { return fFooterTemplate; }
			set {
				fFooterTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterContentTemplate {
			get { return fFooterContentTemplate; }
			set {
				fFooterContentTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderTemplate {
			get { return fHeaderTemplate; }
			set {
				fHeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderContentTemplate {
			get { return fHeaderContentTemplate; }
			set {
				fHeaderContentTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal bool IsDragged {
			get { return fIsDragged; }
			set { fIsDragged = value; }
		}
		protected internal bool IsResized {
			get { return isResized; }
			set { isResized = value; }
		}
		protected internal int HeightInPixel {
			get {
				Unit height = PopupControl.GetWindowHeight(this);
				return height.Type == UnitType.Pixel ? (int)Math.Round(height.Value) : 0;
			}
		}
		protected internal int WidthInPixel {
			get {
				Unit width = PopupControl.GetWindowWidth(this);
				return width.Type == UnitType.Pixel ? (int)Math.Round(width.Value) : 0;
			}
		}
		protected internal int ZIndex {
			get { return fZIndex; }
			set { fZIndex = value; }
		}
		public PopupWindow()
			: base() {
		}
		public PopupWindow(string text)
			: this(text, "", "", "") {
		}
		public PopupWindow(string text, string name)
			: this(text, name, "", "") {
		}
		public PopupWindow(string text, string name, string headerText)
			: this(text, name, headerText, "") {
		}
		public PopupWindow(string text, string name, string headerText, string footerText)
			: base() {
			Text = text;
			Name = name;
			HeaderText = headerText;
			FooterText = footerText;
		}
		public override void Assign(CollectionItem source) {
			if (source is PopupWindow) {
				PopupWindow src = source as PopupWindow;
				Enabled = src.Enabled;
				CloseButtonImage.Url = src.CloseButtonImage.Url;
				PinButtonImage.Url = src.PinButtonImage.Url;
				RefreshButtonImage.Url = src.RefreshButtonImage.Url;
				CollapseButtonImage.Url = src.CollapseButtonImage.Url;
				MaximizeButtonImage.Url = src.MaximizeButtonImage.Url;
				ContentUrl = src.ContentUrl;
				ContentUrlIFrameTitle = src.ContentUrlIFrameTitle;
				FooterImage.Assign(src.FooterImage);
				FooterNavigateUrl = src.FooterNavigateUrl;
				FooterText = src.FooterText;
				HeaderNavigateUrl = src.HeaderNavigateUrl;
				HeaderImage.Assign(src.HeaderImage);
				HeaderText = src.HeaderText;
				Height = src.Height;
				MinWidth = src.MinWidth;
				MinHeight = src.MinHeight;
				MaxWidth = src.MaxWidth;
				MaxHeight = src.MaxHeight;
				Left = src.Left;
				Modal = src.Modal;
				Name = src.Name;
				PopupElementID = src.PopupElementID;
				SizeGripImage.Assign(src.SizeGripImage);
				SizeGripRtlImage.Assign(src.SizeGripRtlImage);
				ShowFooter = src.ShowFooter;
				ShowHeader = src.ShowHeader;
				ShowOnPageLoad = src.ShowOnPageLoad;
				Target = src.Target;
				Text = src.Text;
				ToolTip = src.ToolTip;
				Top = src.Top;
				Width = src.Width;
				ShowPageScrollbarWhenModal = src.ShowPageScrollbarWhenModal;
				AutoUpdatePosition = src.AutoUpdatePosition;
				CloseButtonStyle.Assign(src.CloseButtonStyle);
				PinButtonStyle.Assign(src.PinButtonStyle);
				RefreshButtonStyle.Assign(src.RefreshButtonStyle);
				CollapseButtonStyle.Assign(src.CollapseButtonStyle);
				MaximizeButtonStyle.Assign(src.MaximizeButtonStyle);
				ContentStyle.Assign(src.ContentStyle);
				FooterStyle.Assign(src.FooterStyle);
				HeaderStyle.Assign(src.HeaderStyle);
				CloseButtonImage.Assign(src.CloseButtonImage);
				pinButtonImage.Assign(src.pinButtonImage);
				refreshButtonImage.Assign(src.refreshButtonImage);
				collapseButtonImage.Assign(src.collapseButtonImage);
				maxButtonImage.Assign(src.maxButtonImage);
				HeaderTemplate = src.HeaderTemplate;
				ContentTemplate = src.ContentTemplate;
				FooterTemplate = src.FooterTemplate;
				FooterContentTemplate = src.FooterContentTemplate;
				HeaderContentTemplate = src.HeaderContentTemplate;
				PopupAction = src.PopupAction;
				CloseAction = src.CloseAction;
				CloseOnEscape = src.CloseOnEscape;
				ShowCloseButton = src.ShowCloseButton;
				ShowPinButton = src.ShowPinButton;
				ShowRefreshButton = src.ShowRefreshButton;
				ShowCollapseButton = src.ShowCollapseButton;
				ShowMaximizeButton = src.ShowMaximizeButton;
				Pinned = src.Pinned;
				Collapsed = src.Collapsed;
				Maximized = src.Maximized;
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { CloseButtonImage, PinButtonImage, RefreshButtonImage, CollapseButtonImage, MaximizeButtonImage, HeaderImage, FooterImage, 
				CloseButtonStyle, PinButtonStyle, RefreshButtonStyle, CollapseButtonStyle, MaximizeButtonStyle, FooterStyle, HeaderStyle, ContentStyle, SizeGripImage, SizeGripRtlImage };
		}
		public override string ToString() {
			return (HeaderText != "") ? HeaderText : GetType().Name;
		}
		public Control FindControl(string id) {
			return TemplateContainerBase.FindTemplateControl(PopupControl, PopupControl.GetHeaderTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(PopupControl, PopupControl.GetHeaderContentTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(PopupControl, PopupControl.GetFooterTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(PopupControl, PopupControl.GetFooterContentTemplateContainerID(this), id)
				?? TemplateContainerBase.FindTemplateControl(PopupControl, PopupControl.GetContentTemplateContainerID(this), id)
				?? ContentControl.FindControl(id);			
		}
		protected internal void SetDataItem(object value) {
			fDataItem = value;
		}
		protected override ContentControlCollection CreateContentControlCollection(Control ownerControl) {
			return new PopupControlContentControlCollection(ownerControl);
		}
		protected override ContentControl CreateContentControl() {
			return new PopupControlContentControl();
		}
		PCWindowControlBase contentContainerControl;
		protected internal PCWindowControlBase ContentContainerControl {
			get { return contentContainerControl; }
		}
		protected internal void SetContentContainer(PCWindowControlBase contentContainerControl) {
			this.contentContainerControl = contentContainerControl;
		}
		enum WindowClientVisibleState { Unknown, LoadedOrVisible, UnloadedInvisible };
		WindowClientVisibleState clientContontVisible = WindowClientVisibleState.Unknown;
		protected internal bool GetIsClientContentVisibleNotKnown() {
			return this.clientContontVisible == WindowClientVisibleState.Unknown;
		}
		protected internal bool GetIsClientContentLoadedOrVisible() {
			return this.clientContontVisible == WindowClientVisibleState.LoadedOrVisible;
		}
		protected internal void SetClientContentVisible(bool clientVisible) {
			this.clientContontVisible = clientVisible ? WindowClientVisibleState.LoadedOrVisible : WindowClientVisibleState.UnloadedInvisible;
			if(clientVisible)
				ShowContent();
		}
		private void ShowContent() {
			if (ContentContainerControl != null)
				ContentContainerControl.Visible = true;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class PopupWindowCollection : Collection<PopupWindow> {
		public PopupWindowCollection()
			: base() {
		}
		public PopupWindowCollection(ASPxPopupControlBase popupControl)
			: base(popupControl) {
		}
		protected internal ASPxPopupControlBase PopupControl {
			get { return Owner as ASPxPopupControlBase; }
		}
		protected override void OnChanged() {
			if (PopupControl != null)
				PopupControl.WindowsChanged();
		}
		public PopupWindow Add() {
			return AddInternal(new PopupWindow());
		}
		public PopupWindow Add(string contentText) {
			return Add(contentText, "", "", "");
		}
		public PopupWindow Add(string contentText, string name) {
			return Add(contentText, name, "", "");
		}
		public PopupWindow Add(string contentText, string name, string headerText) {
			return Add(contentText, name, headerText, "");
		}
		public PopupWindow Add(string contentText, string name, string headerText, string footerText) {
			return AddInternal(new PopupWindow(contentText, name, headerText, footerText));
		}
		public PopupWindow FindByName(string name) {
			return FindByIndex(IndexOfName(name));
		}
		public PopupWindow FindByText(string text) {
			return FindByIndex(IndexOfText(text));
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(PopupWindow item) {
				return item.Name == name;
			});
		}
		public int IndexOfText(string text) {
			return IndexOf(delegate(PopupWindow item) {
				return item.Text == text;
			});
		}
	}
}
namespace DevExpress.Web.Internal {
	public class DefaultPopupWindow : PopupWindow {
		public const string DefaultHeaderText = "Header";
		public const string DefaultFooterText = "Footer";
		private ASPxPopupControlBase fPopupControl = null;
		public override ASPxPopupControlBase PopupControl {
			get { return fPopupControl; }
		}
		public override int Index {
			get { return -1; }
		}
		[
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public override Unit Height {
			get { return PopupControl.Height; }
			set { PopupControl.Height = value; }
		}
		[
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public override Unit Width {
			get { return PopupControl.Width; }
			set { PopupControl.Width = value; }
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public override Unit MinHeight {
			get { return PopupControl.MinHeight; }
			set { PopupControl.MinHeight = value; }
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public override Unit MinWidth {
			get { return PopupControl.MinWidth; }
			set { PopupControl.MinWidth = value; }
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public override Unit MaxHeight {
			get { return PopupControl.MaxHeight; }
			set { PopupControl.MaxHeight = value; }
		}
		[DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public override Unit MaxWidth {
			get { return PopupControl.MaxWidth; }
			set { PopupControl.MaxWidth = value; }
		}
		public DefaultPopupWindow(ASPxPopupControlBase popupControl)
			: base() {
			fPopupControl = popupControl;
			HeaderText = DefaultHeaderText;
			FooterText = DefaultFooterText;
		}
		protected override bool IsDeserialization() {
			return PopupControl.IsLoading();
		}
	}
	public class PopupWindowClientState {
		public const char WindowStateSeparator = ':';
		private string clientState;
		public int? Left					 { get; private set; }
		public int? Top					  { get; private set; }
		public Unit? Width				   { get; private set; }
		public Unit? Height				  { get; private set; }
		public int ZIndex				   { get; private set; }
		public bool IsDragged			   { get; private set; }
		public bool ShowOnPageLoad		  { get; private set; }
		public bool IsResized			   { get; private set; }
		public bool ClientContentWasLoaded  { get; private set; }
		public bool Pinned				  { get; private set; }
		public bool Collapsed			   { get; private set; }
		public bool Maximized			   { get; private set; }
		public bool IsValid				 { get; private set; }
		public PopupWindowClientState(string clientState) {
			this.clientState = clientState;
			ParseClientState();
		}
		protected void ParseClientState() {
			string[] position = clientState.Split(new char[] { WindowStateSeparator });
			IsValid = position.Length == 12;
			if(IsValid) {
				ShowOnPageLoad = (position[0] == "1");
				IsDragged = (position[1] == "1");
				ZIndex = int.Parse(position[2]);
				int clientLeft = int.Parse(position[3], CultureInfo.InvariantCulture);
				if(clientLeft != RenderUtils.InvalidDimension)
					Left = clientLeft;
				int clientTop = int.Parse(position[4], CultureInfo.InvariantCulture);
				if(clientTop != RenderUtils.InvalidDimension)
					Top = clientTop;
				IsResized = (position[5] == "1");
				string clientWidth = position[6];
				if(clientWidth != RenderUtils.InvalidDimension.ToString())
					Width = Unit.Parse(clientWidth, CultureInfo.InvariantCulture);
				string clientHeight = position[7];
				if(clientHeight != RenderUtils.InvalidDimension.ToString())
					Height = Unit.Parse(clientHeight, CultureInfo.InvariantCulture);
				ClientContentWasLoaded = position[8] == "1";
				Pinned = position[9] == "1";
				Collapsed = position[10] == "1";
				Maximized = position[11] == "1";
			}
		}
	}
	public static class PopupWindowClientStateUtils {
		public static void LoadClientState(PopupWindow window, PopupWindowClientState popupWindowClientState) {
			if(popupWindowClientState.IsValid) {
				window.ShowOnPageLoad   = popupWindowClientState.ShowOnPageLoad;
				window.IsDragged		= popupWindowClientState.IsDragged;
				window.ZIndex		   = popupWindowClientState.ZIndex;
				if(popupWindowClientState.Left.HasValue)
					window.Left = popupWindowClientState.Left.Value;
				if(popupWindowClientState.Top.HasValue)
					window.Top = popupWindowClientState.Top.Value;
				window.IsResized = popupWindowClientState.IsResized;
				if (popupWindowClientState.Width.HasValue)
					window.Width = popupWindowClientState.Width.Value;
				if (popupWindowClientState.Height.HasValue)
					window.Height = popupWindowClientState.Height.Value;
				window.SetClientContentVisible(popupWindowClientState.ClientContentWasLoaded);
				window.Pinned = popupWindowClientState.Pinned;
				window.Collapsed = popupWindowClientState.Collapsed;
				window.Maximized = popupWindowClientState.Maximized;
			}
		}
		public static bool GetPopupControlWindowContentVisible(PopupWindow window) {
			ASPxPopupControlBase popupControl = window.PopupControl;
			return popupControl != null && popupControl.GetIsWindowContentVisible(window);
		}
		public static string SaveClientState(PopupWindow window) {
			bool contentVisible = GetPopupControlWindowContentVisible(window);
			StringBuilder state = new StringBuilder(window.ShowOnPageLoad ? "1:" : "0:");
			state.Append(window.IsDragged ? "1:" : "0:");
			state.Append(window.ZIndex.ToString()).Append(PopupWindowClientState.WindowStateSeparator);
			state.Append(window.Left != PopupWindow.DefaultLeft ? window.Left.ToString() : RenderUtils.InvalidDimension.ToString()).Append(PopupWindowClientState.WindowStateSeparator);
			state.Append(window.Top != PopupWindow.DefaultTop ? window.Top.ToString() : RenderUtils.InvalidDimension.ToString()).Append(PopupWindowClientState.WindowStateSeparator);
			state.Append(window.IsResized ? "1:" : "0:");
			state.Append(window.WidthInPixel.ToString()).Append(PopupWindowClientState.WindowStateSeparator);
			state.Append(window.HeightInPixel.ToString()).Append(PopupWindowClientState.WindowStateSeparator);
			state.Append(contentVisible ? "1:" : "0:");
			state.Append(window.Pinned ? "1:" : "0:");
			state.Append(window.Collapsed ? "1:" : "0:");
			state.Append(window.Maximized ? "1" : "0");
			return state.ToString();
		}
	}
}
