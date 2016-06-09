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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Utils;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public enum DragElement { Header, Window }
	public enum ShowSizeGrip { Auto, True, False }
	public enum LoadContentViaCallback { None, OnFirstShow, OnPageLoad }
	public enum CssOverflow { Visible, Hidden, Auto, Scroll }
	public abstract class ASPxPopupControlBase : ASPxDataWebControl, IRequiresLoadPostDataControl, IEndInitAccessorContainer, ICallbackEventHandler {
		protected internal enum PopupHeaderButton { CloseButton, PinButton, RefreshButton, CollapseButton, MaximizeButton }
		public const int DefaultAppearAfter = 300;
		public const int DefaultDisappearAfter = 500;
		public const bool DefaultShowFooter = false;
		public const bool DefaultShowHeader = true;
		protected internal const string PopupControlScriptResourceName = WebScriptsResourcePath + "PopupControl.js";
		protected internal const string CloseButtonClickHandlerName = "ASPx.PWCBClick(event, '{0}',{1})";
		protected internal const string PinButtonClickHandlerName = "ASPx.PWPBClick(event, '{0}',{1})";
		protected internal const string RefreshButtonClickHandlerName = "ASPx.PWRBClick(event, '{0}',{1})";
		protected internal const string CollapseButtonClickHandlerName = "ASPx.PWMNBClick(event, '{0}',{1})";
		protected internal const string MaximizeButtonClickHandlerName = "ASPx.PWMXBClick(event, '{0}',{1})";
		protected internal const string HeaderButtonMouseDownHandlerName = "return ASPx.PWHMDown(event);";
		protected internal const string DraggingMouseDownHandlerName = "ASPx.PWDGMDown(event,'{0}',{1})";
		protected internal const string PreventDragStartHandlerName = "return ASPx.Evt.PreventDragStart(event)";
		protected internal const string GripMouseDownHandlerName = "return ASPx.PWGripMDown(event,'{0}',{1})";
		protected internal const string WindowMouseDownHandlerName = "ASPx.PWMDown(event,'{0}',{1},{2})";
		protected internal const string WindowMouseMoveHandlerName = "ASPx.PWMMove(event,'{0}',{1})";
		protected internal const string HeaderButtonIdPostfix = "Img";
		protected internal const string WindowsStateKey = "windowsState";
		protected DefaultPopupWindow fDefaultWindow = null;
		protected PopupWindowCollection fWindows = null;
		protected PCControl fPCControl = null;
		private Dictionary<PopupHeaderButton, string> headerButtonsIds;
		private Dictionary<PopupHeaderButton, string> headerButtonsOnClicks;
		private bool fDataBound = false;
		private ITemplate fWindowContentTemplate = null;
		private ITemplate fWindowFooterTemplate = null;
		private ITemplate fWindowHeaderTemplate = null;
		private ITemplate fWindowHeaderContentTemplate = null;
		private ITemplate fWindowFooterContentTemplate = null;
		private static readonly object EventCallback = new object();
		private static readonly object EventCommand = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseAllowDragging"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public virtual bool AllowDragging {
			get { return GetBoolProperty("AllowDragging", false); }
			set { SetBoolProperty("AllowDragging", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseAllowResize"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AllowResize {
			get { return GetBoolProperty("AllowResize", false); }
			set {
				SetBoolProperty("AllowResize", false, value);
				LayoutChanged();
			}
		}
		protected internal int AppearAfterInternal {
			get { return GetIntProperty("AppearAfter", DefaultAppearAfter); }
			set {
				CommonUtils.CheckNegativeValue(value, "AppearAfter");
				SetIntProperty("AppearAfter", DefaultAppearAfter, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseScrollBars"),
#endif
Category("Layout"), DefaultValue(ScrollBars.None), AutoFormatDisable]
		public ScrollBars ScrollBars {
			get { return (ScrollBars)GetEnumProperty("ScrollBars", ScrollBars.None); }
			set {
				SetEnumProperty("ScrollBars", ScrollBars.None, value);
				LayoutChanged();
			}
		}
		protected internal CssOverflow ContentOverflowX {
			get { return (CssOverflow)GetEnumProperty("ContentOverflowX", CssOverflow.Visible); }
			set { SetEnumProperty("ContentOverflowX", CssOverflow.Visible, value); }
		}
		protected internal CssOverflow ContentOverflowY {
			get { return (CssOverflow)GetEnumProperty("ContentOverflowY", CssOverflow.Visible); }
			set { SetEnumProperty("ContentOverflowY", CssOverflow.Visible, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		protected internal CloseAction CloseActionInternal {
			get { return (CloseAction)GetEnumProperty("CloseAction", CloseAction.OuterMouseClick); }
			set { SetEnumProperty("CloseAction", CloseAction.OuterMouseClick, value); }
		}
		protected internal bool CloseOnEscapeInternal {
			get { return GetBoolProperty("CloseOnEscape", false); }
			set { SetBoolProperty("CloseOnEscape", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseContentUrl"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string ContentUrl {
			get { return DefaultWindow.ContentUrl; }
			set {
				DefaultWindow.ContentUrl = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseContentUrlIFrameTitle"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string ContentUrlIFrameTitle {
			get { return DefaultWindow.ContentUrlIFrameTitle; }
			set { DefaultWindow.ContentUrlIFrameTitle = value; }
		}
		protected internal LoadContentViaCallback LoadContentViaCallbackInternal {
			get {
				return string.IsNullOrEmpty(ContentUrl)	?
					(LoadContentViaCallback)GetEnumProperty("LoadContentViaCallback", LoadContentViaCallback.None) 
					: LoadContentViaCallback.None; 
			}
			set { SetEnumProperty("LoadContentViaCallback", LoadContentViaCallback.None, value); }
		}
		protected internal int DisappearAfterInternal {
			get { return GetIntProperty("DisappearAfter", DefaultDisappearAfter); }
			set {
				CommonUtils.CheckNegativeValue(value, "DisappearAfter");
				SetIntProperty("DisappearAfter", DefaultDisappearAfter, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseDragElement"),
#endif
		Category("Behavior"), DefaultValue(DragElement.Header), AutoFormatDisable]
		public DragElement DragElement {
			get { return (DragElement)GetEnumProperty("DragElement", DragElement.Header); }
			set { SetEnumProperty("DragElement", DragElement.Header, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the PopupAnimationType property instead")]
		public bool EnableAnimation {
			get { return PopupAnimationType != AnimationType.None; }
			set {
				if(value)
					PopupAnimationType = AnimationType.Auto;
				else
					PopupAnimationType = AnimationType.None;
			}
		}
		bool ShouldSerializeEnableAnimation() { return false; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBasePopupAnimationType"),
#endif
		Category("Behavior"), DefaultValue(AnimationType.Auto), AutoFormatDisable]
		public AnimationType PopupAnimationType {
			get { return (AnimationType)GetEnumProperty("PopupAnimationType", AnimationType.Auto); }
			set {
				SetEnumProperty("PopupAnimationType", AnimationType.Auto, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseCloseAnimationType"),
#endif
		Category("Behavior"), DefaultValue(AnimationType.None), AutoFormatDisable]
		public AnimationType CloseAnimationType {
			get { return (AnimationType)GetEnumProperty("CloseAnimationType", AnimationType.None); }
			set { SetEnumProperty("CloseAnimationType", AnimationType.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseRenderIFrameForPopupElements"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean RenderIFrameForPopupElements {
			get { return RenderIFrameForPopupElementsInternal; }
			set {
				RenderIFrameForPopupElementsInternal = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseEnableHierarchyRecreation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableHierarchyRecreation {
			get { return EnableHierarchyRecreationInternal; }
			set { EnableHierarchyRecreationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseEnableHotTrack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool EnableHotTrack {
			get { return EnableHotTrackInternal; }
			set { EnableHotTrackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseFooterNavigateUrl"),
#endif
		AutoFormatDisable, Localizable(false), DefaultValue(""),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string FooterNavigateUrl {
			get { return DefaultWindow.FooterNavigateUrl; }
			set {
				DefaultWindow.FooterNavigateUrl = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseHeaderNavigateUrl"),
#endif
		AutoFormatDisable, DefaultValue(""), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string HeaderNavigateUrl {
			get { return DefaultWindow.HeaderNavigateUrl; }
			set {
				DefaultWindow.HeaderNavigateUrl = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseFooterText"),
#endif
		DefaultValue(DefaultPopupWindow.DefaultFooterText), AutoFormatDisable, Localizable(true)]
		public string FooterText {
			get { return DefaultWindow.FooterText; }
			set { DefaultWindow.FooterText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseHeaderText"),
#endif
		DefaultValue(DefaultPopupWindow.DefaultHeaderText), AutoFormatDisable, Localizable(true)]
		public string HeaderText {
			get { return DefaultWindow.HeaderText; }
			set { DefaultWindow.HeaderText = value; }
		}
		[Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseLeft"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int Left {
			get { return DefaultWindow.Left; }
			set { DefaultWindow.Left = value; }
		}
		protected internal bool ModalInternal {
			get { return DefaultWindow.Modal; }
			set {
				DefaultWindow.Modal = value;
				LayoutChanged();
			}
		}
		protected internal bool ShowPageScrollbarWhenModalInternal {
			get { return DefaultWindow.ShowPageScrollbarWhenModal; }
			set { DefaultWindow.ShowPageScrollbarWhenModal = value; }
		}
		protected internal bool AutoUpdatePositionInternal {
			get { return DefaultWindow.AutoUpdatePosition; }
			set { DefaultWindow.AutoUpdatePosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseOpacity"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(AppearanceStyleBase.DefaultOpacity)]
		public int Opacity {
			get { return (ControlStyle as AppearanceStyle).Opacity; }
			set { (ControlStyle as AppearanceStyle).Opacity = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBasePinned"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool Pinned {
			get { return DefaultWindow.Pinned; }
			set { DefaultWindow.Pinned = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseCollapsed"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool Collapsed {
			get { return DefaultWindow.Collapsed; }
			set { DefaultWindow.Collapsed = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseMaximized"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool Maximized {
			get { return DefaultWindow.Maximized; }
			set { DefaultWindow.Maximized = value; }
		}
		protected internal PopupAction PopupActionInternal {
			get { return (PopupAction)GetEnumProperty("PopupAction", PopupAction.LeftMouseClick); }
			set { SetEnumProperty("PopupAction", PopupAction.LeftMouseClick, value); }
		}
		protected internal string PopupElementIDInternal {
			get { return DefaultWindow.PopupElementID; }
			set { DefaultWindow.PopupElementID = value; }
		}
		protected internal PopupHorizontalAlign PopupHorizontalAlignInternal {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.NotSet); }
			set { SetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.NotSet, value); }
		}
		protected internal int PopupHorizontalOffsetInternal {
			get { return (int)GetIntProperty("PopupHorizontalOffset", 0); }
			set { SetIntProperty("PopupHorizontalOffset", 0, value); }
		}
		protected internal PopupVerticalAlign PopupVerticalAlignInternal {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.NotSet); }
			set { SetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.NotSet, value); }
		}
		[DefaultValue(0), AutoFormatDisable]
		protected internal int PopupVerticalOffsetInternal {
			get { return (int)GetIntProperty("PopupVerticalOffset", 0); }
			set { SetEnumProperty("PopupVerticalOffset", 0, value); }
		}
		[DefaultValue(PopupAlignCorrection.Auto), AutoFormatDisable]
		protected internal PopupAlignCorrection PopupAlignCorrectionInternal {
			get { return (PopupAlignCorrection)GetEnumProperty("PopupAlignCorrection", PopupAlignCorrection.Auto); }
			set { SetEnumProperty("PopupAlignCorrection", PopupAlignCorrection.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseResizingMode"),
#endif
		Category("Behavior"), DefaultValue(ResizingMode.Live), AutoFormatDisable]
		public ResizingMode ResizingMode {
			get { return (ResizingMode)GetEnumProperty("ResizingMode", ResizingMode.Live); }
			set { SetEnumProperty("ResizingMode", ResizingMode.Live, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowCloseButton"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowCloseButton {
			get { return GetBoolProperty("ShowCloseButton", true); }
			set {
				SetBoolProperty("ShowCloseButton", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowPinButton"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool ShowPinButton {
			get { return GetBoolProperty("ShowPinButton", false); }
			set {
				SetBoolProperty("ShowPinButton", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowRefreshButton"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool ShowRefreshButton {
			get { return GetBoolProperty("ShowRefreshButton", false); }
			set {
				SetBoolProperty("ShowRefreshButton", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowCollapseButton"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool ShowCollapseButton {
			get { return GetBoolProperty("ShowCollapseButton", false); }
			set {
				SetBoolProperty("ShowCollapseButton", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowMaximizeButton"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatDisable]
		public bool ShowMaximizeButton {
			get { return GetBoolProperty("ShowMaximizeButton", false); }
			set {
				SetBoolProperty("ShowMaximizeButton", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowFooter"),
#endif
		Category("Appearance"), DefaultValue(DefaultShowFooter), AutoFormatDisable]
		public bool ShowFooter {
			get {
				if(DefaultWindow.ShowFooter == DefaultBoolean.Default)
					return DefaultShowFooter;
				return DefaultWindow.ShowFooter == DefaultBoolean.True;
			}
			set {
				if(value == DefaultShowFooter)
					DefaultWindow.ShowFooter = DefaultBoolean.Default;
				else
					DefaultWindow.ShowFooter = value ? DefaultBoolean.True : DefaultBoolean.False;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowHeader"),
#endif
		Category("Appearance"), DefaultValue(DefaultShowHeader), AutoFormatDisable]
		public bool ShowHeader {
			get {
				if(DefaultWindow.ShowHeader == DefaultBoolean.Default)
					return DefaultShowHeader;
				return DefaultWindow.ShowHeader == DefaultBoolean.True;
			}
			set {
				if(value == DefaultShowHeader)
					DefaultWindow.ShowHeader = DefaultBoolean.Default;
				else
					DefaultWindow.ShowHeader = value ? DefaultBoolean.True : DefaultBoolean.False;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowShadow"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowShadow {
			get { return GetBoolProperty("ShowShadow", true); }
			set {
				SetBoolProperty("ShowShadow", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowSizeGrip"),
#endif
		Category("Appearance"), DefaultValue(ShowSizeGrip.Auto), AutoFormatDisable]
		public ShowSizeGrip ShowSizeGrip {
			get { return (ShowSizeGrip)GetEnumProperty("ShowSizeGrip", ShowSizeGrip.Auto); }
			set {
				SetEnumProperty("ShowSizeGrip", ShowSizeGrip.Auto, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseShowOnPageLoad"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public virtual bool ShowOnPageLoad {
			get { return DefaultWindow.ShowOnPageLoad; }
			set { DefaultWindow.ShowOnPageLoad = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseTarget"),
#endif
		DefaultValue(""), Localizable(false), TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return DefaultWindow.Target; }
			set { DefaultWindow.Target = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseText"),
#endif
		Localizable(true), DefaultValue(""), AutoFormatDisable,
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
		public string Text {
			get { return DefaultWindow.Text; }
			set { DefaultWindow.Text = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseTop"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int Top {
			get { return DefaultWindow.Top; }
			set { DefaultWindow.Top = value; }
		}
		protected internal PopupWindowCollection WindowsInternal {
			get {
				if(fWindows == null)
					fWindows = new PopupWindowCollection(this);
				return fWindows;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseMinWidth"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit MinWidth {
			get { return GetUnitProperty("MinWidth", Unit.Empty); }
			set {
				if(!MaxWidth.IsEmpty)
					CommonUtils.CheckGreaterOrEqual(MaxWidth.Value, value.Value, "MaxWidth", "MinWidth");
				SetUnitProperty("MinWidth", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseMinHeight"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit MinHeight {
			get { return GetUnitProperty("MinHeight", Unit.Empty); }
			set {
				if(!MaxHeight.IsEmpty)
					CommonUtils.CheckGreaterOrEqual(MaxHeight.Value, value.Value, "MaxHeight", "MinHeight");
				SetUnitProperty("MinHeight", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseMaxWidth"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit MaxWidth {
			get { return GetUnitProperty("MaxWidth", Unit.Empty); }
			set {
				if(!MinWidth.IsEmpty)
					CommonUtils.CheckGreaterOrEqual(value.Value, MinWidth.Value, "MaxWidth", "MinWidth");
				SetUnitProperty("MaxWidth", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseMaxHeight"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit MaxHeight {
			get { return GetUnitProperty("MaxHeight", Unit.Empty); }
			set {
				if(!MinHeight.IsEmpty)
					CommonUtils.CheckGreaterOrEqual(value.Value, MinHeight.Value, "MaxHeight", "MinHeight");
				SetUnitProperty("MaxHeight", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseRenderMode"),
#endif
		Obsolete("This property is now obsolete. The Lightweight render mode is used."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Category("Layout"), DefaultValue(ControlRenderMode.Lightweight), AutoFormatDisable]
		public virtual ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		protected internal HeaderButtonImageProperties CloseButtonImageInternal {
			get { return DefaultWindow.CloseButtonImage; }
		}
		protected internal HeaderButtonCheckedImageProperties PinButtonImageInternal {
			get { return DefaultWindow.PinButtonImage; }
		}
		protected internal HeaderButtonImageProperties RefreshButtonImageInternal {
			get { return DefaultWindow.RefreshButtonImage; }
		}
		protected internal HeaderButtonCheckedImageProperties CollapseButtonImageInternal {
			get { return DefaultWindow.CollapseButtonImage; }
		}
		protected internal HeaderButtonCheckedImageProperties MaximizeButtonImageInternal {
			get { return DefaultWindow.MaximizeButtonImage; }
		}
		protected internal ImageProperties FooterImageInternal {
			get { return DefaultWindow.FooterImage; }
		}
		protected internal ImageProperties HeaderImageInternal {
			get { return DefaultWindow.HeaderImage; }
		}
		protected internal ImageProperties SizeGripImageInternal {
			get { return DefaultWindow.SizeGripImage; }
		}
		protected internal ImageProperties SizeGripRtlImageInternal {
			get { return DefaultWindow.SizeGripRtlImage; }
		}
		protected internal Dictionary<PopupHeaderButton, string> HeaderButtonsIds {
			get {
				if(headerButtonsIds == null) {
					headerButtonsIds = new Dictionary<PopupHeaderButton, string>();
					headerButtonsIds.Add(PopupHeaderButton.CloseButton, "HCB");
					headerButtonsIds.Add(PopupHeaderButton.PinButton, "HPB");
					headerButtonsIds.Add(PopupHeaderButton.RefreshButton, "HRB");
					headerButtonsIds.Add(PopupHeaderButton.CollapseButton, "HMNB");
					headerButtonsIds.Add(PopupHeaderButton.MaximizeButton, "HMXB");
				}
				return headerButtonsIds;
			}
		}
		protected internal Dictionary<PopupHeaderButton, string> HeaderButtonsOnClicks {
			get {
				if(headerButtonsOnClicks == null) {
					headerButtonsOnClicks = new Dictionary<PopupHeaderButton, string>();
					headerButtonsOnClicks.Add(PopupHeaderButton.CloseButton, CloseButtonClickHandlerName);
					headerButtonsOnClicks.Add(PopupHeaderButton.PinButton, PinButtonClickHandlerName);
					headerButtonsOnClicks.Add(PopupHeaderButton.RefreshButton, RefreshButtonClickHandlerName);
					headerButtonsOnClicks.Add(PopupHeaderButton.CollapseButton, CollapseButtonClickHandlerName);
					headerButtonsOnClicks.Add(PopupHeaderButton.MaximizeButton, MaximizeButtonClickHandlerName);
				}
				return headerButtonsOnClicks;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseWindowCallback"),
#endif
		Category("Action")]
		public event PopupWindowCallbackEventHandler WindowCallback {
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBaseCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlBasePopupWindowCommand"),
#endif
		Category("Action")]
		public event PopupControlCommandEventHandler PopupWindowCommand {
			add { Events.AddHandler(EventCommand, value); }
			remove { Events.RemoveHandler(EventCommand, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxPopupControlBaseClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterTemplate {
			get { return DefaultWindow.FooterTemplate; }
			set {
				DefaultWindow.FooterTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate FooterContentTemplate {
			get { return DefaultWindow.FooterContentTemplate; }
			set {
				DefaultWindow.FooterContentTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderTemplate {
			get { return DefaultWindow.HeaderTemplate; }
			set {
				DefaultWindow.HeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderContentTemplate {
			get { return DefaultWindow.HeaderContentTemplate; }
			set {
				DefaultWindow.HeaderContentTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal virtual ITemplate WindowHeaderContentTemplateInternal {
			get { return fWindowHeaderContentTemplate; }
			set {
				fWindowHeaderContentTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal virtual ITemplate WindowFooterContentTemplateInternal {
			get { return fWindowFooterContentTemplate; }
			set {
				fWindowFooterContentTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal virtual ITemplate WindowContentTemplateInternal {
			get { return fWindowContentTemplate; }
			set {
				fWindowContentTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal virtual ITemplate WindowFooterTemplateInternal {
			get { return fWindowFooterTemplate; }
			set {
				fWindowFooterTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal virtual ITemplate WindowHeaderTemplateInternal {
			get { return fWindowHeaderTemplate; }
			set {
				fWindowHeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty)]
		protected internal virtual PopupWindow DefaultWindow {
			get {
				if(fDefaultWindow == null)
					fDefaultWindow = CreateDefaultWindow();
				return fDefaultWindow;
			}
		}
		[Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ControlCollection Controls {
			get { return DefaultWindow.Controls; }
		}
		[Browsable(false), AutoFormatDisable,
		EditorBrowsableAttribute(EditorBrowsableState.Never),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ContentControlCollection ContentCollection {
			get { return DefaultWindow.ContentCollection; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatEnable]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		protected virtual bool AdjustInnerControlsSizeOnShow {
			get { return true; }
		}
		protected PopupControlImages PopupControlImages {
			get { return (PopupControlImages)ImagesInternal; }
		}
		protected internal PopupControlImages PopupControlRenderImages {
			get { return (PopupControlImages)RenderImagesInternal; }
		}
		protected PopupControlStyles PopupControlStyles {
			get { return (PopupControlStyles)StylesInternal; }
		}
		protected internal PopupControlStyles PopupControlRenderStyles {
			get { return (PopupControlStyles)RenderStylesInternal; }
		}
		protected virtual ControlCollection BaseControls {
			get { return base.Controls; }
		}
		protected internal PCControl MainControl {
			get { return fPCControl; }
		}
		public ASPxPopupControlBase()
			: this(null) {
		}
		protected ASPxPopupControlBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		public new virtual Control FindControl(string id) {
			return DefaultWindow.FindControl(id);
		}
		protected internal Control FindControlBase(string id) {
			return base.FindControl(id);
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if(e is PopupControlCommandEventArgs) {
				OnPopupWindowCommand(e as PopupControlCommandEventArgs);
				return true;
			}
			return false;
		}
		protected override bool IsAnimationScriptNeeded() {
			return base.IsAnimationScriptNeeded() || PopupAnimationType != AnimationType.None || CloseAnimationType != AnimationType.None;
		}
		protected virtual DefaultPopupWindow CreateDefaultWindow() {
			return new DefaultPopupWindow(this);
		}
		protected internal virtual bool HasDefaultWindowInternal() {
			return WindowsInternal.Count == 0;
		}
		public virtual bool HasSizeGrip() {
			return ShowSizeGrip == ShowSizeGrip.True || (ShowSizeGrip == ShowSizeGrip.Auto && AllowResize);
		}
		protected internal virtual bool IsHeaderDragging(PopupWindow window) {
			return (window != null && AllowDragging && DragElement == DragElement.Header &&
				IsHeaderVisible(window));
		}
		protected internal virtual bool IsWindowDragging() {
			return (AllowDragging && DragElement == DragElement.Window);
		}
		protected virtual AnimationType GetActualPopupAnimationType() {
			if(PopupAnimationType == AnimationType.Auto) {
				if(PopupHorizontalAlignInternal == PopupHorizontalAlign.WindowCenter && PopupVerticalAlignInternal == PopupVerticalAlign.WindowCenter)
					return AnimationType.Fade;
				else
					return AnimationType.Slide;
			}
			return PopupAnimationType;
		}
		protected virtual AnimationType GetActualCloseAnimationType() {
			if(CloseAnimationType == AnimationType.Auto) {
				if(PopupHorizontalAlignInternal == PopupHorizontalAlign.WindowCenter && PopupVerticalAlignInternal == PopupVerticalAlign.WindowCenter)
					return AnimationType.Fade;
				else
					return AnimationType.Slide;
			}
			return CloseAnimationType;
		}
		protected internal new bool IsRightToLeft() {
			return base.IsRightToLeft();
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable dataSource) {
			if(!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				WindowsInternal.Clear();
			else if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindPopupWindows(dataSource);
				ResetControlHierarchy();
			}
		}
		private void DataBindPopupWindows(IEnumerable dataSource) {
			WindowsInternal.Clear();
			foreach(object obj in dataSource) {
				PopupWindow window = new PopupWindow();
				WindowsInternal.Add(window);
				DataBindWindowProperties(window, obj);
				DataBindHeaderProperties(window, obj);
				DataBindFooterProperties(window, obj);
				window.SetDataItem(obj);
				OnWindowDataBound(new PopupWindowEventArgs(window));
			}
		}
		private void DataBindWindowProperties(PopupWindow window, object windowObject) {
			if(windowObject is SiteMapNode) {
				SiteMapNode siteMapNode = (SiteMapNode)windowObject;
				window.Text = siteMapNode.Title;
				window.ToolTip = siteMapNode.Description;
				if(siteMapNode["ContentUrl"] != null)
					window.ContentUrl = siteMapNode["ContentUrl"];
				if(siteMapNode["Enabled"] != null)
					window.Enabled = bool.Parse(siteMapNode["Enabled"]);
				if(siteMapNode["Name"] != null)
					window.Name = siteMapNode["Name"];
				if(siteMapNode["PopupElementID"] != null)
					window.PopupElementID = siteMapNode["PopupElementID"];
				if(siteMapNode["ShowHeader"] != null)
					window.ShowHeader = (DefaultBoolean)Enum.Parse(typeof(DefaultBoolean), siteMapNode["ShowHeader"], true);
				if(siteMapNode["ShowFooter"] != null)
					window.ShowFooter = (DefaultBoolean)Enum.Parse(typeof(DefaultBoolean), siteMapNode["ShowFooter"], true);
				if(siteMapNode["Target"] != null)
					window.Target = siteMapNode["Target"];
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(windowObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<string>(windowObject, "ContentUrl", props, value => { window.ContentUrl = value; });
				DataUtils.GetPropertyValue<bool>(windowObject, "Enabled", props, value => { window.Enabled = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "Name", props, value => { window.Name = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "Target", props, value => { window.Target = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "PopupElementID", props, value => { window.PopupElementID = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "Text", props, value => { window.Text = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "ToolTip", props, value => { window.ToolTip = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "ShowHeader", props,
					value => {
						if(value != "")
							window.ShowHeader = (DefaultBoolean)Enum.Parse(typeof(DefaultBoolean), value, true);
					}
				);
				DataUtils.GetPropertyValue<string>(windowObject, "ShowFooter", props,
					value => {
						if(value != "")
							window.ShowFooter = (DefaultBoolean)Enum.Parse(typeof(DefaultBoolean), value, true);
					}
				);
			}
		}
		private void DataBindHeaderProperties(PopupWindow window, object windowObject) {
			if(windowObject is SiteMapNode) {
				SiteMapNode siteMapNode = (SiteMapNode)windowObject;
				window.HeaderNavigateUrl = siteMapNode.Url;
				if(siteMapNode["HeaderNavigateUrl"] != null)
					window.HeaderNavigateUrl = siteMapNode["HeaderNavigateUrl"];
				if(siteMapNode["HeaderText"] != null)
					window.HeaderText = siteMapNode["HeaderText"];
				if(siteMapNode["HeaderImageUrl"] != null)
					window.HeaderImage.Url = siteMapNode["HeaderImageUrl"];
				if(siteMapNode["HeaderImageHeight"] != null)
					window.HeaderImage.Height = Unit.Parse(siteMapNode["HeaderImageHeight"]);
				if(siteMapNode["HeaderImageWidth"] != null)
					window.HeaderImage.Width = Unit.Parse(siteMapNode["HeaderImageWidth"]);
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(windowObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<string>(windowObject, "HeaderNavigateUrl", props, value => { window.HeaderNavigateUrl = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "HeaderText", props, value => { window.HeaderText = value; });
				if(DataUtils.GetPropertyValue<string>(windowObject, "HeaderImageUrl", props, value => { window.HeaderImage.Url = value; })) {
					DataUtils.GetPropertyValue<string>(windowObject, "HeaderImageHeight", props, value => { window.HeaderImage.Height = Unit.Parse(value); });
					DataUtils.GetPropertyValue<string>(windowObject, "HeaderImageWidth", props, value => { window.HeaderImage.Width = Unit.Parse(value); });
				}
			}
		}
		private void DataBindFooterProperties(PopupWindow window, object windowObject) {
			if(windowObject is SiteMapNode) {
				SiteMapNode siteMapNode = (SiteMapNode)windowObject;
				window.FooterNavigateUrl = siteMapNode.Url;
				if(siteMapNode["FooterNavigateUrl"] != null)
					window.FooterNavigateUrl = siteMapNode["FooterNavigateUrl"];
				if(siteMapNode["FooterText"] != null)
					window.FooterText = siteMapNode["FooterText"];
				if(siteMapNode["FooterImageUrl"] != null)
					window.FooterImage.Url = siteMapNode["FooterImageUrl"];
				if(siteMapNode["FooterImageHeight"] != null)
					window.FooterImage.Height = Unit.Parse(siteMapNode["FooterImageHeight"]);
				if(siteMapNode["FooterImageWidth"] != null)
					window.FooterImage.Width = Unit.Parse(siteMapNode["FooterImageWidth"]);
				if(siteMapNode["SizeGripImageUrl"] != null)
					window.SizeGripImage.Url = siteMapNode["SizeGripImageUrl"];
				if(siteMapNode["SizeGripImageHeight"] != null)
					window.SizeGripImage.Height = Unit.Parse(siteMapNode["SizeGripImageHeight"]);
				if(siteMapNode["SizeGripImageWidth"] != null)
					window.SizeGripImage.Width = Unit.Parse(siteMapNode["SizeGripImageWidth"]);
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(windowObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<string>(windowObject, "FooterNavigateUrl", props, value => { window.FooterNavigateUrl = value; });
				DataUtils.GetPropertyValue<string>(windowObject, "FooterText", props, value => { window.FooterText = value; });
				if(DataUtils.GetPropertyValue<string>(windowObject, "FooterImageUrl", props, value => { window.FooterImage.Url = value; })) {
					DataUtils.GetPropertyValue<string>(windowObject, "FooterImageHeight", props, value => { window.FooterImage.Height = Unit.Parse(value); });
					DataUtils.GetPropertyValue<string>(windowObject, "FooterImageWidth", props, value => { window.FooterImage.Width = Unit.Parse(value); });
				}
				if(DataUtils.GetPropertyValue<string>(windowObject, "SizeGripImageUrl", props, value => { window.SizeGripImage.Url = value; })) {
					DataUtils.GetPropertyValue<string>(windowObject, "SizeGripImageHeight", props, value => { window.SizeGripImage.Height = Unit.Parse(value); });
					DataUtils.GetPropertyValue<string>(windowObject, "SizeGripImageWidth", props, value => { window.SizeGripImage.Width = Unit.Parse(value); });
				}
			}
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected override bool HasContent() {
			return IsEnabled() || DesignMode;
		}
		protected override bool HasLoadingPanel() {
			return IsCallBacksEnabled();
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return true;
		}
		protected override void ClearControlFields() {
			fPCControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			fPCControl = new PopupControlLite(this);
			BaseControls.Add(fPCControl);
			base.CreateControlHierarchy();
			ClearChildViewState();
		}
		protected override IDictionary GetDesignModeState() {
			int index = -1;
			object obj = ((IControlDesignerAccessor)this).UserData["ViewWindow"];
			if(obj != null)
				index = (int)obj;
			if(!HasDefaultWindowInternal() && index != -1)
				for(int i = 0; i < WindowsInternal.Count; i++)
					MainControl.GetWindowControl(WindowsInternal[i]).Visible = index == i;
			return base.GetDesignModeState();
		}
		protected internal string GetClientWindowID(PopupWindow window) {
			return "CLW" + window.Index.ToString();
		}
		protected internal string GetHeaderButtonID(PopupHeaderButton ButtonType, PopupWindow window) {
			return HeaderButtonsIds[ButtonType] + window.Index.ToString();
		}
		protected internal string GetHeaderButtonImageID(PopupHeaderButton ButtonType, PopupWindow window) {
			return GetHeaderButtonID(ButtonType, window) + HeaderButtonIdPostfix;
		}
		protected internal string GetContentIFrameID(PopupWindow window) {
			return "CIF" + window.Index.ToString();
		}
		protected internal string GetContentIFrameDivID(PopupWindow window) {
			return "CIFD" + window.Index.ToString();
		}
		protected internal string GetContentScrollDivID(PopupWindow window) {
			return "CSD" + window.Index.ToString();
		}
		protected internal string GetContentTemplateContainerID(PopupWindow window) {
			return CorrectTemplateID("TPCC" + window.Index.ToString());
		}
		protected internal string GetFooterTemplateContainerID(PopupWindow window) {
			return CorrectTemplateID("TPCF" + window.Index.ToString());
		}
		protected internal string GetHeaderContentTemplateContainerID(PopupWindow window) {
			return CorrectTemplateID("TPCHC" + window.Index.ToString());
		}
		protected internal string GetFooterContentTemplateContainerID(PopupWindow window) {
			return CorrectTemplateID("TPCFC" + window.Index.ToString());
		}
		protected internal string GetHeaderTemplateContainerID(PopupWindow window) {
			return CorrectTemplateID("TPCH" + window.Index.ToString());
		}
		protected internal string GetFooterGripCellID(PopupWindow window) {
			return "FGRP" + window.Index.ToString();
		}
		protected internal string GetFirefoxTextCursorFixDivID(PopupWindow window) {
			return "TCFix" + window.Index.ToString();
		}
		protected internal string GetPopupWindowIFrameElementID(PopupWindow window) {
			return "DXPWIF" + window.Index.ToString();
		}
		protected internal string GetPopupWindowModalElementID(PopupWindow window) {
			return "DXPWMB" + window.Index.ToString();
		}
		protected internal string GetPopupWindowContentCellID(PopupWindow window) {
			return "PWC" + window.Index.ToString();
		}
		protected internal string GetPopupWindowHeaderCellID(PopupWindow window) {
			return "PWH" + window.Index.ToString();
		}
		protected internal string GetPopupWindowHeaderTextCellID(PopupWindow window) {
			return GetPopupWindowHeaderCellID(window) + "T";
		}
		protected internal string GetPopupWindowHeaderImageCellID(PopupWindow window) {
			return GetPopupWindowHeaderCellID(window) + "I";
		}
		protected internal string GetPopupWindowFooterCellID(PopupWindow window) {
			return "PWF" + window.Index.ToString();
		}
		protected internal string GetPopupWindowFooterTextCellID(PopupWindow window) {
			return GetPopupWindowFooterCellID(window) + "T";
		}
		protected internal string GetPopupWindowFooterImageCellID(PopupWindow window) {
			return GetPopupWindowFooterCellID(window) + "I";
		}
		protected internal string GetPopupWindowID(PopupWindow window) {
			return "PW" + window.Index.ToString();
		}
		protected internal string GetPopupWindowShadowTableID(PopupWindow window) {
			return "PWST" + window.Index.ToString();
		}
		protected internal bool HasHeaderFooterElementsCellIDs() {
			return IsClientSideAPIEnabled();
		}
		protected internal string GetHeaderButtonOnClick(PopupHeaderButton buttonType, PopupWindow window) {
			return string.Format(HeaderButtonsOnClicks[buttonType], ClientID, window.Index);
		}
		protected internal string GetHeaderButtonOnMouseDown() {
			return HeaderButtonMouseDownHandlerName;
		}
		protected internal string GetPreventOnDragStart() {
			return PreventDragStartHandlerName;
		}
		protected internal string GetGripOnMouseDown(PopupWindow window) {
			return string.Format(GripMouseDownHandlerName, ClientID, window.Index);
		}
		protected internal string GetHeaderOnMouseDown(PopupWindow window) {
			if(IsHeaderDragging(window))
				return string.Format(DraggingMouseDownHandlerName, ClientID, window.Index);
			return "";
		}
		protected internal string GetWindowOnMouseDown(PopupWindow window) {
			return string.Format(WindowMouseDownHandlerName, ClientID, window.Index, IsWindowDragging().ToString().ToLower());
		}
		protected internal string GetWindowOnMouseMove(PopupWindow window) {
			return string.Format(WindowMouseMoveHandlerName, ClientID, window.Index);
		}
		protected internal PopupControlTemplateContainer CreateTemplateContainer(PopupWindow window) {
			return new PopupControlTemplateContainer(window);
		}
		protected internal string GetFooterNavigateUrl(PopupWindow window) {
			return window.FooterNavigateUrl == "" ? DefaultWindow.FooterNavigateUrl : window.FooterNavigateUrl;
		}
		protected internal string GetHeaderNavigateUrl(PopupWindow window) {
			return window.HeaderNavigateUrl == "" ? DefaultWindow.HeaderNavigateUrl : window.HeaderNavigateUrl;
		}
		protected internal string GetContentText(PopupWindow window) {
			return window.Text;
		}
		protected internal string GetFooterText(PopupWindow window) {
			return (window.FooterText != "") ? window.FooterText : FooterText;
		}
		protected internal string GetHeaderText(PopupWindow window) {
			return (window.HeaderText != "") ? window.HeaderText : HeaderText;
		}
		protected internal bool IsFooterVisible(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowFooter, ShowFooter);
		}
		protected internal bool IsHeaderVisible(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowHeader, ShowHeader);
		}
		protected internal string GetTarget(PopupWindow window) {
			return (window.Target != "") ? window.Target : Target;
		}
		protected internal string GetToolTip(PopupWindow window) {
			return (window.ToolTip != "") ? window.ToolTip : ToolTip;
		}
		protected string CorrectTemplateID(string id) {
			return id.Replace('-', 'm');
		}
		protected PopupAction ConvertWindowPopupActionToPopupAction(WindowPopupAction windowPopupAction) {
			return (PopupAction)Enum.Parse(typeof(PopupAction), windowPopupAction.ToString());
		}
		protected CloseAction ConvertWindowCloseActionToCloseAction(WindowCloseAction windowCloseAction) {
			return (CloseAction)Enum.Parse(typeof(CloseAction), windowCloseAction.ToString());
		}
		protected PopupAction GetWindowPopupAction(PopupWindow window) {
			return window.PopupAction == WindowPopupAction.Default ? PopupActionInternal :
				ConvertWindowPopupActionToPopupAction(window.PopupAction);
		}
		protected CloseAction GetWindowCloseAction(PopupWindow window) {
			return window.CloseAction == WindowCloseAction.Default ? CloseActionInternal :
				ConvertWindowCloseActionToCloseAction(window.CloseAction);
		}
		protected bool GetWindowDefaultBooleanProperty(PopupWindow window, Func<PopupWindow, DefaultBoolean> getProperty, bool defaultValue) {
			if(window != null) {
				DefaultBoolean property = getProperty(window);
				return property == DefaultBoolean.Default ? defaultValue : property == DefaultBoolean.True;
			}
			return defaultValue;
		}
		protected bool GetWindowCloseOnEscape(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.CloseOnEscape, CloseOnEscapeInternal);
		}
		protected internal bool GetShowWindowCloseButton(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowCloseButton, ShowCloseButton);
		}
		protected internal bool GetShowWindowPinButton(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowPinButton, ShowPinButton);
		}
		protected internal bool GetShowWindowRefreshButton(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowRefreshButton, ShowRefreshButton);
		}
		protected internal bool GetShowWindowCollapseButton(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowCollapseButton, ShowCollapseButton);
		}
		protected internal bool GetShowWindowMaximizeButton(PopupWindow window) {
			return GetWindowDefaultBooleanProperty(window, w => w.ShowMaximizeButton, ShowMaximizeButton);
		}
		protected internal void WindowsChanged() {
			if(!IsLoading()) {
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { DefaultWindow, WindowsInternal });
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
		}
		protected override bool HasSelectedScripts() {
			return HasCheckedScripts();
		}
		protected bool HasCheckedScripts() {
			for(int i = 0; i < WindowsInternal.Count; i++) {
				if(!GetPinButtonCheckedStyle(WindowsInternal[i]).IsEmpty || !GetCollapseButtonCheckedStyle(WindowsInternal[i]).IsEmpty || !GetMaximizeButtonCheckedStyle(WindowsInternal[i]).IsEmpty)
					return true;
			}
			if(HasDefaultWindowInternal()) {
				if(!GetPinButtonCheckedStyle(DefaultWindow).IsEmpty || !GetCollapseButtonCheckedStyle(DefaultWindow).IsEmpty || !GetMaximizeButtonCheckedStyle(DefaultWindow).IsEmpty)
					return true;
			}
			return false;
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			AddCheckedItems(helper);
		}
		protected void AddCheckedItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < WindowsInternal.Count; i++)
				AddCheckedItem(helper, WindowsInternal[i]);
			if(HasDefaultWindowInternal())
				AddCheckedItem(helper, DefaultWindow);
		}
		protected void AddCheckedItem(StateScriptRenderHelper helper, PopupWindow window) {
			if(GetShowWindowPinButton(window))
				helper.AddStyle(GetPinButtonCheckedCssStyle(window), GetHeaderButtonID(PopupHeaderButton.PinButton, window), new string[0],
					GetPinButtonImageProperties(window).GetCheckedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
			if(GetShowWindowCollapseButton(window))
				helper.AddStyle(GetCollapseButtonCheckedCssStyle(window), GetHeaderButtonID(PopupHeaderButton.CollapseButton, window), new string[0],
					GetCollapseButtonImageProperties(window).GetCheckedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
			if(GetShowWindowMaximizeButton(window))
				helper.AddStyle(GetMaximizeButtonCheckedCssStyle(window), GetHeaderButtonID(PopupHeaderButton.MaximizeButton, window), new string[0],
					GetMaximizeButtonImageProperties(window).GetCheckedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
		}
		protected override bool HasHoverScripts() {
			if(EnableHotTrack) {
				for(int i = 0; i < WindowsInternal.Count; i++) {
					if(!GetCloseButtonHoverStyle(WindowsInternal[i]).IsEmpty && !GetPinButtonHoverStyle(WindowsInternal[i]).IsEmpty && !GetRefreshButtonHoverStyle(WindowsInternal[i]).IsEmpty)
						return true;
				}
				if(HasDefaultWindowInternal()) {
					if(!GetCloseButtonHoverStyle(DefaultWindow).IsEmpty && !GetPinButtonHoverStyle(DefaultWindow).IsEmpty && !GetRefreshButtonHoverStyle(DefaultWindow).IsEmpty)
						return true;
				}
			}
			return false;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < WindowsInternal.Count; i++)
				AddHoverItem(helper, WindowsInternal[i]);
			if(HasDefaultWindowInternal())
				AddHoverItem(helper, DefaultWindow);
		}
		protected void AddHoverItem(StateScriptRenderHelper helper, PopupWindow window) {
			if(GetShowWindowCloseButton(window))
				helper.AddStyle(GetCloseButtonHoverCssStyle(window), GetHeaderButtonID(PopupHeaderButton.CloseButton, window), new string[0],
					GetCloseButtonImageProperties(window).GetHottrackedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
			if(GetShowWindowPinButton(window))
				helper.AddStyle(GetPinButtonHoverCssStyle(window), GetHeaderButtonID(PopupHeaderButton.PinButton, window), new string[0],
					GetPinButtonImageProperties(window).GetHottrackedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
			if(GetShowWindowRefreshButton(window))
				helper.AddStyle(GetRefreshButtonHoverCssStyle(window), GetHeaderButtonID(PopupHeaderButton.RefreshButton, window), new string[0],
					GetRefreshButtonImageProperties(window).GetHottrackedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
			if(GetShowWindowCollapseButton(window))
				helper.AddStyle(GetCollapseButtonHoverCssStyle(window), GetHeaderButtonID(PopupHeaderButton.CollapseButton, window), new string[0],
					GetCollapseButtonImageProperties(window).GetHottrackedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
			if(GetShowWindowMaximizeButton(window))
				helper.AddStyle(GetMaximizeButtonHoverCssStyle(window), GetHeaderButtonID(PopupHeaderButton.MaximizeButton, window), new string[0],
					GetMaximizeButtonImageProperties(window).GetHottrackedScriptObject(Page), HeaderButtonIdPostfix, IsEnabled());
		}
		protected override bool HasFunctionalityScripts() {
			return IsEnabled();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterPopupUtilsScripts();
			RegisterIncludeScript(typeof(ASPxPopupControl), PopupControlScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!AdjustInnerControlsSizeOnShow)
				stb.Append(localVarName + ".adjustInnerControlsSizeOnShow=false;\n");
			if(AppearAfterInternal != DefaultAppearAfter)
				stb.AppendFormat("{0}.appearAfter={1};\n", localVarName, AppearAfterInternal.ToString());
			if(DisappearAfterInternal != DefaultDisappearAfter)
				stb.AppendFormat("{0}.disappearAfter={1};\n", localVarName, DisappearAfterInternal.ToString());
			if(GetActualPopupAnimationType() == AnimationType.None && GetActualCloseAnimationType() == AnimationType.None)
				stb.AppendFormat(localVarName + ".enableAnimation=false;\n");
			if(GetActualPopupAnimationType() != AnimationType.None)
				stb.AppendFormat(localVarName + ".popupAnimationType='" + GetActualPopupAnimationType().ToString().ToLower() + "';\n");
			if(GetActualCloseAnimationType() != AnimationType.None)
				stb.AppendFormat(localVarName + ".closeAnimationType='" + GetActualCloseAnimationType().ToString().ToLower() + "';\n");
			if(AllowResize)
				stb.Append(localVarName + ".allowResize=true;\n");
			if(PopupActionInternal != PopupAction.LeftMouseClick)
				stb.AppendFormat("{0}.popupAction='{1}';\n", localVarName, PopupActionInternal.ToString());
			if(CloseActionInternal != CloseAction.OuterMouseClick)
				stb.AppendFormat("{0}.closeAction='{1}';\n", localVarName, CloseActionInternal.ToString());
			if(CloseOnEscapeInternal)
				stb.AppendFormat(localVarName + ".closeOnEscape=true;\n");
			if(PopupHorizontalAlignInternal != PopupHorizontalAlign.NotSet)
				stb.AppendFormat("{0}.popupHorizontalAlign='{1}';\n", localVarName, PopupHorizontalAlignInternal.ToString());
			if(PopupVerticalAlignInternal != PopupVerticalAlign.NotSet)
				stb.AppendFormat("{0}.popupVerticalAlign='{1}';\n", localVarName, PopupVerticalAlignInternal.ToString());
			if(PopupHorizontalOffsetInternal != 0)
				stb.AppendFormat("{0}.popupHorizontalOffset={1};\n", localVarName, PopupHorizontalOffsetInternal.ToString());
			if(PopupVerticalOffsetInternal != 0)
				stb.AppendFormat("{0}.popupVerticalOffset={1};\n", localVarName, PopupVerticalOffsetInternal.ToString());
			if(PopupAlignCorrectionInternal != PopupAlignCorrection.Auto)
				stb.Append(localVarName + ".isPopupFullCorrectionOn=false;\n");
			if(ResizingMode == ResizingMode.Postponed)
				stb.Append(localVarName + ".isLiveResizingMode=false;\n");
			if(Page != null && !Page.IsPostBack)
				stb.Append(localVarName + ".isPopupPositionCorrectionOn=false;\n");
			if(!ShowShadow)
				stb.Append(localVarName + ".shadowVisible=false;\n");
			if(LoadContentViaCallbackInternal != LoadContentViaCallback.None)
				stb.AppendFormat("{0}.contentLoadingMode='{1}';\n", localVarName, LoadContentViaCallbackInternal.ToString());
			CssOverflow overflowX = GetContentOverflowX(), overflowY = GetContentOverflowY();
			if(overflowX != CssOverflow.Visible)
				stb.AppendFormat("{0}.contentOverflowX=\"{1}\";\n", localVarName, overflowX.ToString());
			if(overflowY != CssOverflow.Visible)
				stb.AppendFormat("{0}.contentOverflowY=\"{1}\";\n", localVarName, overflowY.ToString());
			if(IsWindowDragging())
				stb.AppendFormat("{0}.isWindowDragging=true;\n", localVarName);
			if(AllowDragging)
				stb.AppendFormat("{0}.allowDragging=true;\n", localVarName);
			if(HasDefaultWindowInternal()) {
				if(DefaultWindow.IsDragged)
					stb.AppendFormat("{0}.isDragged={1};\n", localVarName, DefaultWindow.IsDragged.ToString().ToLower());
				if(DefaultWindow.ZIndex != -1)
					stb.AppendFormat("{0}.zIndex={1};\n", localVarName, DefaultWindow.ZIndex.ToString());
				if(Left != 0)
					stb.AppendFormat("{0}.left={1};\n", localVarName, Left.ToString());
				if(Top != 0)
					stb.AppendFormat("{0}.top={1};\n", localVarName, Top.ToString());
				if(DefaultWindow.IsResized)
					stb.AppendFormat("{0}.isResized={1};\n", localVarName, DefaultWindow.IsResized.ToString().ToLower());
				if(!DefaultWindow.Width.IsEmpty && DefaultWindow.Width != 0) {
					stb.AppendFormat("{0}.width={1};\n", localVarName, DefaultWindow.WidthInPixel);
					stb.AppendFormat("{0}.widthFromServer=true;\n", localVarName);
				}
				if(!DefaultWindow.Height.IsEmpty && DefaultWindow.Height != 0)
					stb.AppendFormat("{0}.height={1};\n", localVarName, DefaultWindow.HeightInPixel);
				if(!DefaultWindow.MinHeight.IsEmpty)
					stb.AppendFormat("{0}.minHeight={1};\n", localVarName, UnitUtils.ConvertToPixels(DefaultWindow.MinHeight).Value);
				if(!DefaultWindow.MinWidth.IsEmpty)
					stb.AppendFormat("{0}.minWidth={1};\n", localVarName, UnitUtils.ConvertToPixels(DefaultWindow.MinWidth).Value);
				if(!DefaultWindow.MaxHeight.IsEmpty)
					stb.AppendFormat("{0}.maxHeight={1};\n", localVarName, UnitUtils.ConvertToPixels(DefaultWindow.MaxHeight).Value);
				if(!DefaultWindow.MaxWidth.IsEmpty)
					stb.AppendFormat("{0}.maxWidth={1};\n", localVarName, UnitUtils.ConvertToPixels(DefaultWindow.MaxWidth).Value);
				if(ShowOnPageLoad)
					stb.AppendFormat("{0}.showOnPageLoad={1};\n", localVarName, ShowOnPageLoad.ToString().ToLower());
				if(PopupElementIDInternal != "")
					stb.AppendFormat("{0}.defaultWindowPopupElementIDList={1};\n", localVarName, HtmlConvertor.ToJSON(GetPopupElementIDList(PopupElementIDInternal), true));
				if(ContentUrl != "")
					stb.AppendFormat("{0}.contentUrl='{1}';\n", localVarName, ResolveContentUrl(ContentUrl));
				if(AutoUpdatePositionInternal)
					stb.Append(localVarName + ".autoUpdatePosition=true;\n");
				if(DefaultWindow.Pinned)
					stb.Append(localVarName + ".isPinned=true;\n");
				if(DefaultWindow.Collapsed)
					stb.Append(localVarName + ".isCollapsedInit=true;\n");
				if(DefaultWindow.Maximized)
					stb.Append(localVarName + ".isMaximizedInit=true;\n");
				if(!HideBodyScrollWhenModal())
					stb.Append(localVarName + ".hideBodyScrollWhenModal=false;\n");
			}
			stb.Append(GetIsDraggedArrayScript(localVarName));
			stb.Append(GetIsResizedArrayScript(localVarName));
			stb.Append(GetzIndexArrayScript(localVarName));
			stb.Append(GetLeftArrayScript(localVarName));
			stb.Append(GetTopArrayScript(localVarName));
			stb.Append(GetHeightArrayScript(localVarName));
			stb.Append(GetWidthArrayScript(localVarName));
			stb.Append(GetWidthFromServerScript(localVarName));
			stb.Append(GetMinHeightArrayScript(localVarName));
			stb.Append(GetMinWidthArrayScript(localVarName));
			stb.Append(GetMaxHeightArrayScript(localVarName));
			stb.Append(GetMaxWidthArrayScript(localVarName));
			stb.Append(GetWindowsPopupElementIDListScript(localVarName));
			stb.Append(GetShowOnPageLoadArrayScript(localVarName));
			stb.Append(GetContentURLArrayScript(localVarName));
			stb.Append(GetPopupActionArrayScript(localVarName));
			stb.Append(GetCloseActionArrayScript(localVarName));
			stb.Append(GetCloseOnEscapeArrayScript(localVarName));
			stb.Append(GetAutoUpdatePositionrrayScript(localVarName));
			stb.Append(GetPinnedArrayScript(localVarName));
			stb.Append(GetCollapsedArrayScript(localVarName));
			stb.Append(GetMaximizedArrayScript(localVarName));
			stb.Append(GetHideBodyScrollWhenModalArrayScript(localVarName));
			if(IsClientSideAPIEnabled()) {
				if(IsSSLSecureBlankUrlRequired() && GetSSLSecureBlankUrl() != "")
					stb.AppendFormat("{0}.SSLSecureBlankUrl={1};\n", localVarName, HtmlConvertor.ToScript(GetSSLSecureBlankUrl()));
				stb.Append(GetPopupWindowNameArrayScript(localVarName));
			} else if(WindowsInternal.Count != 0)
				stb.AppendFormat("{0}.windowCount={1};\n", localVarName, WindowsInternal.Count.ToString());
		}
		protected string ResolveContentUrl(string url) {
			return ResolveUrl(url.Replace("'", "%27"));
		}
		protected List<string> GetPopupElementIDList(string ids) {
			ids = ids.Trim();
			if(string.IsNullOrEmpty(ids))
				return null;
			List<string> result = new List<string>();
			foreach(string id in ids.Split(';'))
				result.Add(RenderUtils.GetReferentControlClientID(this, id.Trim(), OnPopupElementResolve));
			return result;
		}
		protected bool GetIsArrayScriptTrivial(IEnumerable items, object defaultValue) {
			foreach(object item in items) {
				if(!object.Equals(item, defaultValue))
					return false;
			}
			return true;
		}
		protected string GetArrayScript<T>(string localVarName, Func<PopupWindow, T> getParam, T defaultValue, string formatString) {
			if(WindowsInternal.Count != 0) {
				List<T> list = new List<T>();
				for(int i = 0; i < WindowsInternal.Count; i++)
					list.Add(getParam(WindowsInternal[i]));
				if(!GetIsArrayScriptTrivial(list, defaultValue))
					return string.Format(formatString, localVarName, HtmlConvertor.ToJSON(list));
			}
			return "";
		}
		protected string GetContentURLArrayScript(string localVarName) {
			return GetArrayScript<string>(localVarName, window => ResolveContentUrl(window.ContentUrl), PopupWindow.DefaultContentUrl, "{0}.contentUrlArray={1};\n");
		}
		protected string GetIsDraggedArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.IsDragged, PopupWindow.DefaultIsDragged, "{0}.isDraggedArray={1};\n");
		}
		protected string GetIsResizedArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.IsResized, PopupWindow.DefaultIsResized, "{0}.isResizedArray={1};\n");
		}
		protected string GetzIndexArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => window.ZIndex, PopupWindow.DefaultZIndex, "{0}.zIndexArray={1};\n");
		}
		protected string GetLeftArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => window.Left, PopupWindow.DefaultLeft, "{0}.leftArray={1};\n");
		}
		protected string GetTopArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => window.Top, PopupWindow.DefaultTop, "{0}.topArray={1};\n");
		}
		protected string GetHeightArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => window.HeightInPixel, 0, "{0}.heightArray={1};\n");
		}
		protected string GetWidthArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => window.WidthInPixel, (int)GetDefaultWindowWidth().Value, "{0}.widthArray={1};\n");
		}
		protected string GetWidthFromServerScript(string localVarName) {
			if(WindowsInternal.Count != 0) {
				List<bool> list = new List<bool>();
				for(int i = 0; i < WindowsInternal.Count; i++)
					list.Add(!WindowsInternal[i].Width.IsEmpty);
				if(!GetIsArrayScriptTrivial(list, false))
					return string.Format("{0}.widthFromServerArray={1};\n", localVarName, HtmlConvertor.ToJSON(list));
			}
			return "";
		}
		protected string GetMinWidthArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => (int)Math.Round(UnitUtils.ConvertToPixels(window.MinWidth).Value), 0, "{0}.minWidthArray={1};\n");
		}
		protected string GetMinHeightArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => (int)Math.Round(UnitUtils.ConvertToPixels(window.MinHeight).Value), 0, "{0}.minHeightArray={1};\n");
		}
		protected string GetMaxWidthArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => (int)Math.Round(UnitUtils.ConvertToPixels(window.MaxWidth).Value), 0, "{0}.maxWidthArray={1};\n");
		}
		protected string GetMaxHeightArrayScript(string localVarName) {
			return GetArrayScript<int>(localVarName, window => (int)Math.Round(UnitUtils.ConvertToPixels(window.MaxHeight).Value), 0, "{0}.maxHeightArray={1};\n");
		}
		protected string GetPopupActionArrayScript(string localVarName) {
			return GetArrayScript<PopupAction>(localVarName, window => GetWindowPopupAction(window), PopupActionInternal, "{0}.popupActionArray={1};\n");
		}
		protected string GetCloseActionArrayScript(string localVarName) {
			return GetArrayScript<CloseAction>(localVarName, window => GetWindowCloseAction(window), CloseActionInternal, "{0}.closeActionArray={1};\n");
		}
		protected string GetCloseOnEscapeArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => GetWindowCloseOnEscape(window), CloseOnEscapeInternal, "{0}.closeOnEscapeArray={1};\n");
		}
		protected string GetShowOnPageLoadArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.ShowOnPageLoad, PopupWindow.DefaultShowOnPageLoad, "{0}.showOnPageLoadArray={1};\n");
		}
		protected string GetAutoUpdatePositionrrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.AutoUpdatePosition, false, "{0}.autoUpdatePositionArray={1};\n");
		}
		protected string GetPinnedArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.Pinned, false, "{0}.isPinnedArray={1};\n");
		}
		protected string GetCollapsedArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.Collapsed, false, "{0}.isCollapsedInitArray={1};\n");
		}
		protected string GetMaximizedArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => window.Maximized, false, "{0}.isMaximizedInitArray={1};\n");
		}
		protected string GetHideBodyScrollWhenModalArrayScript(string localVarName) {
			return GetArrayScript<bool>(localVarName, window => !window.ShowPageScrollbarWhenModal, true, "{0}.hideBodyScrollWhenModalArray={1};\n");
		}
		protected string GetWindowsPopupElementIDListScript(string localVarName) {
			if(WindowsInternal.Count == 0)
				return string.Empty;
			List<object> result = new List<object>();
			foreach(PopupWindow window in WindowsInternal)
				result.Add(GetPopupElementIDList(window.PopupElementID));
			return string.Format("{0}.windowsPopupElementIDList={1};\n", localVarName, HtmlConvertor.ToJSON(result, true));
		}
		protected string GetPopupWindowNameArrayScript(string localVarName) {
			if(WindowsInternal.Count != 0) {
				List<string> list = new List<string>();
				for(int i = 0; i < WindowsInternal.Count; i++)
					list.Add(WindowsInternal[i].Name);
				return string.Format("{0}.CreateWindows({1});\n", localVarName, HtmlConvertor.ToJSON(list));
			}
			return "";
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPopupControl";
		}
		protected virtual bool HideBodyScrollWhenModal() {
			return !ShowPageScrollbarWhenModalInternal;
		}
		protected virtual bool IsSSLSecureBlankUrlRequired() {
			return true;
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new PopupControlStyles(this);
		}
		protected override void RegisterLinkStyles() {
			for(int i = 0; i < WindowsInternal.Count; i++)
				RegisterHeaderFooterLinkStyles(string.Format("{0}_{1}", ClientID, GetPopupWindowID(WindowsInternal[i])));
			if(HasDefaultWindowInternal())
				RegisterHeaderFooterLinkStyles(string.Format("{0}_{1}", ClientID, GetPopupWindowID(DefaultWindow)));
		}
		protected void RegisterHeaderFooterLinkStyles(string windowID) {
			RegisterLinkHoverStyle(LinkStyle.HoverStyle, windowID);
			RegisterLinkVisitedStyle(LinkStyle.VisitedStyle, windowID);
		}
		protected internal AppearanceStyleBase GetDefaultHeaderButtonCellStyle(PopupWindow window) {
			AppearanceStyle cellHeaderButtonStyle = new AppearanceStyle();
			cellHeaderButtonStyle.CopyFrom(PopupControlStyles.GetDefaultHeaderButtonCellStyle());
			return cellHeaderButtonStyle;
		}
		protected internal AppearanceStyleBase GetCloseButtonHoverCssStyle(PopupWindow window) {
			AppearanceStyle hoverStyle = new AppearanceStyle();
			hoverStyle.CopyFrom(GetCloseButtonHoverStyle(window));
			hoverStyle.Paddings.CopyFrom(GetCloseButtonHoverCssStylePaddings(window, hoverStyle));
			return hoverStyle;
		}
		protected internal Paddings GetCloseButtonHoverCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetCloseButtonStyle(window);
			Paddings paddings = GetCloseButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetCloseButtonHoverStyle(PopupWindow window) {
			AppearanceStyleBase hoverStyle = new AppearanceStyleBase();
			hoverStyle.CopyFrom(PopupControlStyles.GetDefaultCloseButtonHoverStyle());
			hoverStyle.CopyFrom(PopupControlRenderStyles.CloseButton.HoverStyle);
			if(window != null)
				hoverStyle.CopyFrom(window.CloseButtonStyle.HoverStyle);
			return hoverStyle;
		}
		protected internal Paddings GetCloseButtonPaddings(PopupWindow window) {
			return GetCloseButtonStyle(window).Paddings;
		}
		protected internal PopupWindowButtonStyle GetCloseButtonStyle(PopupWindow window) {
			PopupWindowButtonStyle style = new PopupWindowButtonStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultCloseButtonStyle());
			style.CopyFrom(PopupControlRenderStyles.CloseButton);
			if(window != null)
				style.CopyFrom(window.CloseButtonStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyleBase GetPinButtonCheckedCssStyle(PopupWindow window) {
			AppearanceStyle checkedStyle = new AppearanceStyle();
			checkedStyle.CopyFrom(GetPinButtonCheckedStyle(window));
			checkedStyle.Paddings.CopyFrom(GetPinButtonCheckedCssStylePaddings(window, checkedStyle));
			return checkedStyle;
		}
		protected internal Paddings GetPinButtonCheckedCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetPinButtonStyle(window);
			Paddings paddings = GetPinButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetPinButtonCheckedStyle(PopupWindow window) {
			AppearanceStyleBase checkedStyle = new AppearanceStyleBase();
			checkedStyle.CopyFrom(PopupControlStyles.GetDefaultPinButtonCheckedStyle());
			checkedStyle.CopyFrom(PopupControlRenderStyles.PinButton.CheckedStyle);
			if(window != null)
				checkedStyle.CopyFrom(window.PinButtonStyle.CheckedStyle);
			return checkedStyle;
		}
		protected internal AppearanceStyleBase GetPinButtonHoverCssStyle(PopupWindow window) {
			AppearanceStyle hoverStyle = new AppearanceStyle();
			hoverStyle.CopyFrom(GetPinButtonHoverStyle(window));
			hoverStyle.Paddings.CopyFrom(GetPinButtonHoverCssStylePaddings(window, hoverStyle));
			return hoverStyle;
		}
		protected internal Paddings GetPinButtonHoverCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetPinButtonStyle(window);
			Paddings paddings = GetPinButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetPinButtonHoverStyle(PopupWindow window) {
			AppearanceStyleBase hoverStyle = new AppearanceStyleBase();
			hoverStyle.CopyFrom(PopupControlStyles.GetDefaultPinButtonHoverStyle());
			hoverStyle.CopyFrom(PopupControlRenderStyles.PinButton.HoverStyle);
			if(window != null)
				hoverStyle.CopyFrom(window.PinButtonStyle.HoverStyle);
			return hoverStyle;
		}
		protected internal Paddings GetPinButtonPaddings(PopupWindow window) {
			return GetPinButtonStyle(window).Paddings;
		}
		protected internal PopupWindowButtonStyle GetPinButtonStyle(PopupWindow window) {
			PopupWindowButtonStyle style = new PopupWindowButtonStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultPinButtonStyle());
			style.CopyFrom(PopupControlRenderStyles.PinButton);
			if(window != null)
				style.CopyFrom(window.PinButtonStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyleBase GetRefreshButtonHoverCssStyle(PopupWindow window) {
			AppearanceStyle hoverStyle = new AppearanceStyle();
			hoverStyle.CopyFrom(GetRefreshButtonHoverStyle(window));
			hoverStyle.Paddings.CopyFrom(GetRefreshButtonHoverCssStylePaddings(window, hoverStyle));
			return hoverStyle;
		}
		protected internal Paddings GetRefreshButtonHoverCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetRefreshButtonStyle(window);
			Paddings paddings = GetRefreshButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetRefreshButtonHoverStyle(PopupWindow window) {
			AppearanceStyleBase hoverStyle = new AppearanceStyleBase();
			hoverStyle.CopyFrom(PopupControlStyles.GetDefaultRefreshButtonHoverStyle());
			hoverStyle.CopyFrom(PopupControlRenderStyles.RefreshButton.HoverStyle);
			if(window != null)
				hoverStyle.CopyFrom(window.RefreshButtonStyle.HoverStyle);
			return hoverStyle;
		}
		protected internal Paddings GetRefreshButtonPaddings(PopupWindow window) {
			return GetRefreshButtonStyle(window).Paddings;
		}
		protected internal PopupWindowButtonStyle GetRefreshButtonStyle(PopupWindow window) {
			PopupWindowButtonStyle style = new PopupWindowButtonStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultRefreshButtonStyle());
			style.CopyFrom(PopupControlRenderStyles.RefreshButton);
			if(window != null)
				style.CopyFrom(window.RefreshButtonStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyleBase GetCollapseButtonCheckedCssStyle(PopupWindow window) {
			AppearanceStyle checkedStyle = new AppearanceStyle();
			checkedStyle.CopyFrom(GetCollapseButtonCheckedStyle(window));
			checkedStyle.Paddings.CopyFrom(GetCollapseButtonCheckedCssStylePaddings(window, checkedStyle));
			return checkedStyle;
		}
		protected internal Paddings GetCollapseButtonCheckedCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetCollapseButtonStyle(window);
			Paddings paddings = GetCollapseButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetCollapseButtonCheckedStyle(PopupWindow window) {
			AppearanceStyleBase checkedStyle = new AppearanceStyleBase();
			checkedStyle.CopyFrom(PopupControlStyles.GetDefaultCollapseButtonCheckedStyle());
			checkedStyle.CopyFrom(PopupControlRenderStyles.CollapseButton.CheckedStyle);
			if(window != null)
				checkedStyle.CopyFrom(window.CollapseButtonStyle.CheckedStyle);
			return checkedStyle;
		}
		protected internal AppearanceStyleBase GetCollapseButtonHoverCssStyle(PopupWindow window) {
			AppearanceStyle hoverStyle = new AppearanceStyle();
			hoverStyle.CopyFrom(GetCollapseButtonHoverStyle(window));
			hoverStyle.Paddings.CopyFrom(GetCollapseButtonHoverCssStylePaddings(window, hoverStyle));
			return hoverStyle;
		}
		protected internal Paddings GetCollapseButtonHoverCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetCollapseButtonStyle(window);
			Paddings paddings = GetCollapseButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetCollapseButtonHoverStyle(PopupWindow window) {
			AppearanceStyleBase hoverStyle = new AppearanceStyleBase();
			hoverStyle.CopyFrom(PopupControlStyles.GetDefaultCollapseButtonHoverStyle());
			hoverStyle.CopyFrom(PopupControlRenderStyles.CollapseButton.HoverStyle);
			if(window != null)
				hoverStyle.CopyFrom(window.CollapseButtonStyle.HoverStyle);
			return hoverStyle;
		}
		protected internal Paddings GetCollapseButtonPaddings(PopupWindow window) {
			return GetCollapseButtonStyle(window).Paddings;
		}
		protected internal PopupWindowButtonStyle GetCollapseButtonStyle(PopupWindow window) {
			PopupWindowButtonStyle style = new PopupWindowButtonStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultCollapseButtonStyle());
			style.CopyFrom(PopupControlRenderStyles.CollapseButton);
			if(window != null)
				style.CopyFrom(window.CollapseButtonStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyleBase GetMaximizeButtonCheckedCssStyle(PopupWindow window) {
			AppearanceStyle checkedStyle = new AppearanceStyle();
			checkedStyle.CopyFrom(GetMaximizeButtonCheckedStyle(window));
			checkedStyle.Paddings.CopyFrom(GetMaximizeButtonCheckedCssStylePaddings(window, checkedStyle));
			return checkedStyle;
		}
		protected internal Paddings GetMaximizeButtonCheckedCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetMaximizeButtonStyle(window);
			Paddings paddings = GetMaximizeButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetMaximizeButtonCheckedStyle(PopupWindow window) {
			AppearanceStyleBase checkedStyle = new AppearanceStyleBase();
			checkedStyle.CopyFrom(PopupControlStyles.GetDefaultMaximizeButtonCheckedStyle());
			checkedStyle.CopyFrom(PopupControlRenderStyles.MaximizeButton.CheckedStyle);
			if(window != null)
				checkedStyle.CopyFrom(window.MaximizeButtonStyle.CheckedStyle);
			return checkedStyle;
		}
		protected internal AppearanceStyleBase GetMaximizeButtonHoverCssStyle(PopupWindow window) {
			AppearanceStyle hoverStyle = new AppearanceStyle();
			hoverStyle.CopyFrom(GetMaximizeButtonHoverStyle(window));
			hoverStyle.Paddings.CopyFrom(GetMaximizeButtonHoverCssStylePaddings(window, hoverStyle));
			return hoverStyle;
		}
		protected internal Paddings GetMaximizeButtonHoverCssStylePaddings(PopupWindow window, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetMaximizeButtonStyle(window);
			Paddings paddings = GetMaximizeButtonPaddings(window);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal AppearanceStyleBase GetMaximizeButtonHoverStyle(PopupWindow window) {
			AppearanceStyleBase hoverStyle = new AppearanceStyleBase();
			hoverStyle.CopyFrom(PopupControlStyles.GetDefaultMaximizeButtonHoverStyle());
			hoverStyle.CopyFrom(PopupControlRenderStyles.MaximizeButton.HoverStyle);
			if(window != null)
				hoverStyle.CopyFrom(window.MaximizeButtonStyle.HoverStyle);
			return hoverStyle;
		}
		protected internal Paddings GetMaximizeButtonPaddings(PopupWindow window) {
			return GetMaximizeButtonStyle(window).Paddings;
		}
		protected internal PopupWindowButtonStyle GetMaximizeButtonStyle(PopupWindow window) {
			PopupWindowButtonStyle style = new PopupWindowButtonStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultMaximizeButtonStyle());
			style.CopyFrom(PopupControlRenderStyles.MaximizeButton);
			if(window != null)
				style.CopyFrom(window.MaximizeButtonStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal PopupWindowContentStyle GetContentStyle(PopupWindow window) {
			PopupWindowContentStyle style = new PopupWindowContentStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultContentStyle());
			style.CopyFrom(PopupControlRenderStyles.Content);
			if(window != null)
				style.CopyFrom(window.ContentStyle);
			return style;
		}
		protected internal string GetContentPaddingsCssClass() {
			return PopupControlStyles.GetDefaultContentPaddingsCssClass();
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(StylesInternal.GetDefaultControlStyle());
			if(style.Cursor == "") {
				if(IsWindowDragging())
					style.Cursor = "move";
				else
					style.Cursor = RenderUtils.GetDefaultCursor();
			}
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, "dxpclW");
			if (Browser.IsIE && Browser.Version > 8)
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, "dxpc-ie");
		}
		protected internal AppearanceStyleBase GetMainDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(PopupControlRenderStyles.Style);
			style.CopyFrom(ControlStyle);
			return style;
		}
		protected internal PopupWindowFooterStyle GetCustomFooterStyle(PopupWindow window) {
			PopupWindowFooterStyle style = new PopupWindowFooterStyle();
			style.CopyFrom(PopupControlRenderStyles.Footer);
			if(window != null)
				style.CopyFrom(window.FooterStyle);
			return style;
		}
		protected internal PopupWindowStyle GetCustomHeaderStyle(PopupWindow window) {
			PopupWindowStyle style = new PopupWindowStyle();
			style.CopyFrom(PopupControlRenderStyles.Header);
			if(window != null)
				style.CopyFrom(window.HeaderStyle);
			return style;
		}
		protected internal PopupWindowFooterStyle GetFooterStyle(PopupWindow window) {
			PopupWindowFooterStyle style = new PopupWindowFooterStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultFooterStyle());
			style.CopyFrom(GetCustomFooterStyle(window));
			return style;
		}
		protected internal PopupWindowStyle GetHeaderStyle(PopupWindow window) {
			PopupWindowStyle style = new PopupWindowStyle();
			style.CopyFrom(PopupControlStyles.GetDefaultHeaderStyle());
			style.CopyFrom(GetCustomHeaderStyle(window));
			if(window != null) {
				if(IsHeaderDragging(window) && style.Cursor == "")
					style.Cursor = "move";
			}
			return style;
		}
		protected internal AppearanceStyleBase GetModalBackgroundStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(PopupControlStyles.GetDefaultModalBackgroundStyle());
			style.CopyFrom(PopupControlRenderStyles.ModalBackground);
			return style;
		}
		protected virtual internal Paddings GetContentPaddings(PopupWindow window) {
			return GetContentStyle(window).Paddings;
		}
		protected virtual internal Paddings GetFooterPaddings(PopupWindow window) {
			return GetFooterStyle(window).Paddings;
		}
		protected virtual internal Paddings GetHeaderPaddings(PopupWindow window) {
			return GetHeaderStyle(window).Paddings;
		}
		protected virtual internal Paddings GetSizeGripPaddings(PopupWindow window) {
			return GetFooterStyle(window).SizeGripPaddings;
		}
		protected virtual internal Unit GetFooterImageSpacing(PopupWindow window) {
			return GetFooterStyle(window).ImageSpacing;
		}
		protected virtual internal Unit GetHeaderImageSpacing(PopupWindow window) {
			return GetHeaderStyle(window).ImageSpacing;
		}
		protected virtual internal Unit GetSizeGripSpacing(PopupWindow window) {
			return GetFooterStyle(window).SizeGripSpacing;
		}
		protected virtual internal Unit GetWindowHeight(PopupWindow window) {
			Unit height = (window == DefaultWindow || window.Height.IsEmpty) ?
				GetMainDivStyle().Height : window.Height;
			Unit minHeight = GetWindowMinHeight(window);
			Unit maxHeight = GetWindowMaxHeight(window);
			Unit windowHeight = height;
			if(!minHeight.IsEmpty && UnitUtils.ConvertToPixels(height).Value < UnitUtils.ConvertToPixels(minHeight).Value)
				windowHeight = minHeight;
			if(!maxHeight.IsEmpty && UnitUtils.ConvertToPixels(height).Value > UnitUtils.ConvertToPixels(maxHeight).Value)
				windowHeight = maxHeight;
			return windowHeight;
		}
		protected virtual internal Unit GetWindowWidth(PopupWindow window) {
			Unit width = (window == DefaultWindow || window.Width.IsEmpty) ?
				GetMainDivStyle().Width : window.Width;
			if(width.IsEmpty || width.Type != UnitType.Pixel)
				width = GetDefaultWindowWidth();
			Unit minWidth = GetWindowMinWidth(window);
			Unit maxWidth = GetWindowMaxWidth(window);
			Unit windowWidth = width;
			if(!minWidth.IsEmpty && UnitUtils.ConvertToPixels(width).Value < UnitUtils.ConvertToPixels(minWidth).Value)
				windowWidth = minWidth;
			if(!maxWidth.IsEmpty && UnitUtils.ConvertToPixels(width).Value > UnitUtils.ConvertToPixels(maxWidth).Value)
				windowWidth = maxWidth;
			return windowWidth;
		}
		protected virtual internal Unit GetWindowMinWidth(PopupWindow window) {
			if(window == DefaultWindow)
				return MinWidth;
			return window.MinWidth.IsEmpty ? MinWidth : window.MinWidth;
		}
		protected virtual internal Unit GetWindowMinHeight(PopupWindow window) {
			if(window == DefaultWindow)
				return MinHeight;
			return window.MinHeight.IsEmpty ? MinHeight : window.MinHeight;
		}
		protected virtual internal Unit GetWindowMaxWidth(PopupWindow window) {
			if(window == DefaultWindow)
				return MaxWidth;
			return window.MaxWidth.IsEmpty ? MaxWidth : window.MaxWidth;
		}
		protected virtual internal Unit GetDefaultWindowWidth() {
			return Unit.Pixel(200);
		}
		protected virtual internal Unit GetWindowMaxHeight(PopupWindow window) {
			if(window == DefaultWindow)
				return MaxHeight;
			return window.MaxHeight.IsEmpty ? MaxHeight : window.MaxHeight;
		}
		protected internal AppearanceStyleBase GetHeaderLinkStyle(PopupWindow window) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFontAndCursorFrom(PopupControlStyles.GetDefaultHeaderStyle());
			style.CopyFontAndCursorFrom(GetCustomHeaderStyle(window));
			style.CopyFrom(LinkStyle.Style);
			return style;
		}
		protected internal AppearanceStyleBase GetFooterLinkStyle(PopupWindow window) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFontAndCursorFrom(PopupControlStyles.GetDefaultFooterStyle());
			style.CopyFontAndCursorFrom(GetCustomFooterStyle(window));
			style.CopyFrom(LinkStyle.Style);
			return style;
		}
		protected override ImagesBase CreateImages() {
			return new PopupControlImages(this);
		}
		protected internal HeaderButtonImageProperties GetCloseButtonImageProperties(PopupWindow window) {
			HeaderButtonImageProperties image = new HeaderButtonImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.CloseButtonImageName));
			image.CopyFrom(CloseButtonImageInternal);
			image.CopyFrom(window.CloseButtonImage);
			return image;
		}
		protected internal HeaderButtonCheckedImageProperties GetPinButtonImageProperties(PopupWindow window) {
			HeaderButtonCheckedImageProperties image = new HeaderButtonCheckedImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.PinButtonImageName));
			image.CopyFrom(PinButtonImageInternal);
			image.CopyFrom(window.PinButtonImage);
			return image;
		}
		protected internal HeaderButtonImageProperties GetRefreshButtonImageProperties(PopupWindow window) {
			HeaderButtonImageProperties image = new HeaderButtonImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.RefreshButtonImageName));
			image.CopyFrom(RefreshButtonImageInternal);
			image.CopyFrom(window.RefreshButtonImage);
			return image;
		}
		protected internal HeaderButtonCheckedImageProperties GetCollapseButtonImageProperties(PopupWindow window) {
			HeaderButtonCheckedImageProperties image = new HeaderButtonCheckedImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.CollapseButtonImageName));
			image.CopyFrom(CollapseButtonImageInternal);
			image.CopyFrom(window.CollapseButtonImage);
			return image;
		}
		protected internal HeaderButtonCheckedImageProperties GetMaximizeButtonImageProperties(PopupWindow window) {
			HeaderButtonCheckedImageProperties image = new HeaderButtonCheckedImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.MaximizeButtonImageName));
			image.CopyFrom(MaximizeButtonImageInternal);
			image.CopyFrom(window.MaximizeButtonImage);
			return image;
		}
		protected internal ImageProperties GetFooterImageProperties(PopupWindow window) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.FooterImageName));
			image.CopyFrom(FooterImageInternal);
			image.CopyFrom(window.FooterImage);
			return image;
		}
		protected internal ImageProperties GetHeaderImageProperties(PopupWindow window) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.HeaderImageName));
			image.CopyFrom(HeaderImageInternal);
			image.CopyFrom(window.HeaderImage);
			return image;
		}
		protected internal ImageProperties GetSizeGripImageProperties(PopupWindow window) {
			ImageProperties image = new ImageProperties();
			if(IsRightToLeft()) {
				image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.SizeGripRtlImageName));
				image.CopyFrom(SizeGripRtlImageInternal);
				image.CopyFrom(window.SizeGripRtlImage);
			} else {
				image.CopyFrom(PopupControlRenderImages.GetImageProperties(Page, PopupControlImages.SizeGripImageName));
				image.CopyFrom(SizeGripImageInternal);
				image.CopyFrom(window.SizeGripImage);
			}
			return image;
		}
		protected virtual bool LoadWindowsState(string state) {
			PopupWindowClientState[] states = GetPopupWindowClientStates(state);
			int stateIndex = 0;
			if(HasDefaultWindowInternal()) {
				PopupWindowClientStateUtils.LoadClientState(DefaultWindow, states[stateIndex]);
				stateIndex++;
			}
			for(; stateIndex < WindowsInternal.Count; stateIndex++) {
				if(stateIndex < states.Length)
					PopupWindowClientStateUtils.LoadClientState(WindowsInternal[stateIndex], states[stateIndex]);
			}
			return false;
		}
		protected internal PopupWindowClientState[] GetPopupWindowClientStates(string state) {
			string[] states = state.Split(new char[] { ';' });
			var popupStates = new List<PopupWindowClientState>();
			foreach(string clientState in states)
				popupStates.Add(new PopupWindowClientState(clientState));
			return popupStates.ToArray();
		}
		protected string SaveWindowsState() {
			StringBuilder state = new StringBuilder();
			if(HasDefaultWindowInternal())
				state.Append(PopupWindowClientStateUtils.SaveClientState(DefaultWindow));
			for(int i = 0; i < WindowsInternal.Count; i++) {
				if(state.Length > 0)
					state.Append(';');
				state.Append(PopupWindowClientStateUtils.SaveClientState(WindowsInternal[i]));
			}
			return state.ToString();
		}
		protected internal override void LoadClientState(string state) {
			EnsureDataBound();
			LoadWindowsState(state);
		}
		protected internal override string SaveClientState() {
			return SaveWindowsState();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected override string GetClientObjectStateInputID() {
			return UniqueID + "State";
		}
		protected internal ITemplate GetContentTemplate(PopupWindow window) {
			if(window == DefaultWindow)
				return null;
			else if(window.ContentTemplate != null)
				return window.ContentTemplate;
			return WindowContentTemplateInternal;
		}
		protected internal ITemplate GetFooterTemplate(PopupWindow window) {
			if(window == DefaultWindow)
				return FooterTemplate;
			else if(window.FooterTemplate != null)
				return window.FooterTemplate;
			return WindowFooterTemplateInternal;
		}
		protected internal ITemplate GetHeaderTemplate(PopupWindow window) {
			if(window == DefaultWindow)
				return HeaderTemplate;
			else if(window.HeaderTemplate != null)
				return window.HeaderTemplate;
			return WindowHeaderTemplateInternal;
		}
		protected internal ITemplate GetHeaderContentTemplate(PopupWindow window) {
			if(window == DefaultWindow)
				return HeaderContentTemplate;
			else if(window.HeaderContentTemplate != null)
				return window.HeaderContentTemplate;
			return WindowHeaderContentTemplateInternal;
		}
		protected internal ITemplate GetFooterContentTemplate(PopupWindow window) {
			if(window == DefaultWindow)
				return FooterContentTemplate;
			else if(window.FooterContentTemplate != null)
				return window.FooterContentTemplate;
			return WindowFooterContentTemplateInternal;
		}
		protected virtual void OnPopupWindowCommand(PopupControlCommandEventArgs e) {
			PopupControlCommandEventHandler handler = (PopupControlCommandEventHandler)Events[EventCommand];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnWindowDataBound(PopupWindowEventArgs e) { }
		protected virtual void OnPopupElementResolve(ControlResolveEventArgs e) { }
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			EnsureDataBound();
			string stateString = GetClientObjectStateValue<string>(WindowsStateKey);
			if(!string.IsNullOrEmpty(stateString))
				return LoadWindowsState(stateString);
			return false;
		}
		int? windowIndexCallbackRequested;
		protected internal int? WindowIndexCallbackRequested { get { return windowIndexCallbackRequested; } }
		protected internal override bool IsCallBacksEnabled() {
			return LoadContentViaCallbackInternal != LoadContentViaCallback.None || IsClientSideAPIEnabled() || ShowRefreshButton;
		}
		protected internal virtual bool GetIsWindowContentVisible(PopupWindow window) {
			return GetIsWindowContentVisibleCore(DesignMode, LoadContentViaCallbackInternal, WindowIndexCallbackRequested, window.Index, GetWindowClientVisible(window));
		}
		protected internal static bool GetIsWindowContentVisibleCore(bool designMode, LoadContentViaCallback contentLoadingMode,
			int? windowIndexCallbackRequested, int index, bool clientContentVisible) {
			return designMode || contentLoadingMode == LoadContentViaCallback.None || clientContentVisible ||
				(windowIndexCallbackRequested.HasValue && windowIndexCallbackRequested.Value == index);
		}
		protected bool GetWindowClientVisible(PopupWindow window){
			if(window.GetIsClientContentVisibleNotKnown())
				TryLoadWindowClientVisibleFromRequest(window);
			return window.GetIsClientContentLoadedOrVisible();
		}
		protected void TryLoadWindowClientVisibleFromRequest(PopupWindow window) {
			if((Page != null || MvcUtils.IsCallback()) && Request != null) {
				if(ClientObjectState == null) return;
				string stateString = GetClientObjectStateValue<string>(WindowsStateKey);
				PopupWindowClientState[] states = GetPopupWindowClientStates(stateString);
				if(states.Length == 0) return;
				if(HasDefaultWindowInternal()) {
					window.SetClientContentVisible(states[0].ClientContentWasLoaded);
				} else if(states.Length > window.Index && states[window.Index].IsValid)
					window.SetClientContentVisible(states[window.Index].ClientContentWasLoaded);
			}
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			int callbackArgumentSeparatorPos = eventArgument.IndexOf(";");
			bool performCallback = callbackArgumentSeparatorPos > -1;
			string windowIndexArgument = performCallback ? eventArgument.Substring(0, callbackArgumentSeparatorPos) : eventArgument;
			this.windowIndexCallbackRequested = int.Parse(windowIndexArgument);
			PopupWindow window = WindowIndexCallbackRequested.Value == -1 ? DefaultWindow : WindowsInternal[WindowIndexCallbackRequested.Value];
			window.SetClientContentVisible(true);
			if(performCallback) {
				string customCallbackArgument = eventArgument.Substring(callbackArgumentSeparatorPos + 1);
				RaiseWindowCallbackEvent(WindowIndexCallbackRequested.Value, customCallbackArgument);
			}
		}
		protected void RaiseWindowCallbackEvent(int index, string arg) {
			PopupWindow window = index == -1 ? DefaultWindow : WindowsInternal[index];
			PopupWindowCallbackArgs args = new PopupWindowCallbackArgs(window, arg);
			OnWindowCallback(args);
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.Html] = GetWindowContentRender(WindowIndexCallbackRequested.Value);
			result[CallbackResultProperties.Index] = WindowIndexCallbackRequested.Value;
			return result;
		}
		protected virtual string GetWindowContentRender(int index) {
			string result = string.Empty;
			PCWindowControlBase contentContainerControl = GetContentContainerControl(index);
			if(contentContainerControl != null) {
				BeginRendering();
				try {
					result = RenderUtils.GetRenderResult(contentContainerControl);
				} finally {
					EndRendering();
				}
			}
			return result;
		}
		protected PCWindowControlBase GetContentContainerControl(int index) {
			PopupWindow window = index == -1 ? DefaultWindow : WindowsInternal[index];
			return window.ContentContainerControl;
		}
		protected override object GetCallbackErrorData() {
			return WindowIndexCallbackRequested;
		}
		protected virtual void OnWindowCallback(PopupWindowCallbackArgs e) {
			PopupWindowCallbackEventHandler handler = Events[EventCallback] as PopupWindowCallbackEventHandler;
			if(handler != null)
				handler(this, e);
		}
		internal void SetContentOverflow(WebControl control) {
			CssOverflow x = GetContentOverflowX();
			CssOverflow y = GetContentOverflowY();
			if(x == y)
				SetOverflow(control, HtmlTextWriterStyle.Overflow, x);
			else {
				SetOverflow(control, HtmlTextWriterStyle.OverflowX, x);
				SetOverflow(control, HtmlTextWriterStyle.OverflowY, y);
			}
		}
		void SetOverflow(WebControl control, HtmlTextWriterStyle styleKey, CssOverflow overflow) {
			if(overflow != CssOverflow.Visible)
				control.Style[styleKey] = overflow.ToString().ToLower();
		}
		CssOverflow GetContentOverflowX() {
			if(ScrollBars == ScrollBars.None)
				return ContentOverflowX;
			switch(ScrollBars) {
				case ScrollBars.Vertical:
					return CssOverflow.Hidden;
				case ScrollBars.Horizontal:
				case ScrollBars.Both:
					return CssOverflow.Scroll;
			}
			return CssOverflow.Auto;
		}
		CssOverflow GetContentOverflowY() {
			if(ScrollBars == ScrollBars.None)
				return ContentOverflowY;
			switch(ScrollBars) {
				case ScrollBars.Horizontal:
					return CssOverflow.Hidden;
				case ScrollBars.Vertical:
				case ScrollBars.Both:
					return CssOverflow.Scroll;
			}
			return CssOverflow.Auto;
		}
	}
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxPopupControl"),
	DefaultProperty("Windows"),
	ToolboxData("<{0}:ASPxPopupControl runat=\"server\"></{0}:ASPxPopupControl>"),
	Designer("DevExpress.Web.Design.ASPxPopupControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxPopupControl.bmp")
	]
	public class ASPxPopupControl : ASPxPopupControlBase, IControlDesigner {
		private static readonly object EventWindowDataBound = new object();
		private static readonly object EventPopupElementResolve = new object();
		public ASPxPopupControl()
			: base() {
		}
		public ASPxPopupControl(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlWindows"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public PopupWindowCollection Windows {
			get { return WindowsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupAction"),
#endif
		DefaultValue(PopupAction.LeftMouseClick), AutoFormatDisable]
		public PopupAction PopupAction {
			get { return PopupActionInternal; }
			set { PopupActionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupHorizontalAlign"),
#endif
		DefaultValue(PopupHorizontalAlign.NotSet), AutoFormatDisable]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return PopupHorizontalAlignInternal; }
			set { PopupHorizontalAlignInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupHorizontalOffset"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int PopupHorizontalOffset {
			get { return PopupHorizontalOffsetInternal; }
			set { PopupHorizontalOffsetInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupVerticalAlign"),
#endif
		DefaultValue(PopupVerticalAlign.NotSet), AutoFormatDisable]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return PopupVerticalAlignInternal; }
			set { PopupVerticalAlignInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupVerticalOffset"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int PopupVerticalOffset {
			get { return PopupVerticalOffsetInternal; }
			set { PopupVerticalOffsetInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupAlignCorrection"),
#endif
		DefaultValue(PopupAlignCorrection.Auto), AutoFormatDisable]
		public PopupAlignCorrection PopupAlignCorrection {
			get { return PopupAlignCorrectionInternal; }
			set { PopupAlignCorrectionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlCloseButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties CloseButtonImage {
			get { return CloseButtonImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPinButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties PinButtonImage {
			get { return PinButtonImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlRefreshButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonImageProperties RefreshButtonImage {
			get { return RefreshButtonImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlCollapseButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties CollapseButtonImage {
			get { return CollapseButtonImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlMaximizeButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderButtonCheckedImageProperties MaximizeButtonImage {
			get { return MaximizeButtonImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlFooterImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FooterImage {
			get { return FooterImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlHeaderImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties HeaderImage {
			get { return HeaderImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlSizeGripImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SizeGripImage {
			get { return SizeGripImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlSizeGripRtlImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SizeGripRtlImage {
			get { return SizeGripRtlImageInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlAutoUpdatePosition"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoUpdatePosition {
			get { return AutoUpdatePositionInternal; }
			set { AutoUpdatePositionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupElementID"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string PopupElementID {
			get { return PopupElementIDInternal; }
			set { PopupElementIDInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlModal"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, Localizable(false)]
		public bool Modal {
			get { return ModalInternal; }
			set { ModalInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlShowPageScrollbarWhenModal"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowPageScrollbarWhenModal {
			get { return ShowPageScrollbarWhenModalInternal; }
			set { ShowPageScrollbarWhenModalInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlLoadContentViaCallback"),
#endif
		Category("Behavior"), DefaultValue(LoadContentViaCallback.None), AutoFormatDisable]
		public LoadContentViaCallback LoadContentViaCallback {
			get { return LoadContentViaCallbackInternal; }
			set { LoadContentViaCallbackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlAppearAfter"),
#endif
		Category("Behavior"), DefaultValue(DefaultAppearAfter), AutoFormatDisable]
		public int AppearAfter {
			get { return AppearAfterInternal; }
			set { AppearAfterInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlDisappearAfter"),
#endif
		Category("Behavior"), DefaultValue(DefaultDisappearAfter), AutoFormatDisable]
		public int DisappearAfter {
			get { return DisappearAfterInternal; }
			set { DisappearAfterInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlCloseAction"),
#endif
		DefaultValue(CloseAction.OuterMouseClick), AutoFormatDisable]
		public CloseAction CloseAction {
			get { return CloseActionInternal; }
			set { CloseActionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlCloseOnEscape"),
#endif
		DefaultValue(false), AutoFormatDisable]
		public bool CloseOnEscape {
			get { return CloseOnEscapeInternal; }
			set { CloseOnEscapeInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public PopupControlClientSideEvents ClientSideEvents {
			get { return (PopupControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlCloseButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowButtonStyle CloseButtonStyle {
			get { return PopupControlStyles.CloseButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPinButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowButtonStyle PinButtonStyle {
			get { return PopupControlStyles.PinButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlRefreshButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowButtonStyle RefreshButtonStyle {
			get { return PopupControlStyles.RefreshButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlCollapseButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowButtonStyle CollapseButtonStyle {
			get { return PopupControlStyles.CollapseButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlMaximizeButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowButtonStyle MaximizeButtonStyle {
			get { return PopupControlStyles.MaximizeButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowContentStyle ContentStyle {
			get { return PopupControlStyles.Content; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlFooterStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowFooterStyle FooterStyle {
			get { return PopupControlStyles.Footer; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupWindowStyle HeaderStyle {
			get { return PopupControlStyles.Header; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlLoadingDivStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingDivStyle LoadingDivStyle {
			get { return base.LoadingDivStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlModalBackgroundStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public PopupControlModalBackgroundStyle ModalBackgroundStyle {
			get { return PopupControlStyles.ModalBackground; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate WindowContentTemplate {
			get { return WindowContentTemplateInternal; }
			set { WindowContentTemplateInternal = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate WindowFooterTemplate {
			get { return WindowFooterTemplateInternal; }
			set { WindowFooterTemplateInternal = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate WindowFooterContentTemplate {
			get { return WindowFooterContentTemplateInternal; }
			set { WindowFooterContentTemplateInternal = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate WindowHeaderTemplate {
			get { return WindowHeaderTemplateInternal; }
			set { WindowHeaderTemplateInternal = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PopupControlTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate WindowHeaderContentTemplate {
			get { return WindowHeaderContentTemplateInternal; }
			set { WindowHeaderContentTemplateInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlWindowDataBound"),
#endif
		Category("Data")]
		public event PopupWindowEventHandler WindowDataBound {
			add { Events.AddHandler(EventWindowDataBound, value); }
			remove { Events.RemoveHandler(EventWindowDataBound, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupControlPopupElementResolve"),
#endif
		Category("Events")]
		public event EventHandler<ControlResolveEventArgs> PopupElementResolve {
			add { Events.AddHandler(EventPopupElementResolve, value); }
			remove { Events.RemoveHandler(EventPopupElementResolve, value); }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PopupControlClientSideEvents();
		}
		public bool HasDefaultWindow() {
			return HasDefaultWindowInternal();
		}
		protected override void OnWindowDataBound(PopupWindowEventArgs e) {
			PopupWindowEventHandler handler = (PopupWindowEventHandler)Events[EventWindowDataBound];
			if(handler != null)
				handler(this, e);
		}
		protected override void OnPopupElementResolve(ControlResolveEventArgs e) {
			EventHandler<ControlResolveEventArgs> handler = (EventHandler<ControlResolveEventArgs>)Events[EventPopupElementResolve];
			if(handler != null)
				handler(this, e);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.PopupControlCommonFormDesigner"; } }
	}
	public class PopupControlContentControlCollection : ContentControlCollection {
		public PopupControlContentControlCollection(Control owner)
			: base(owner) {
		}
		public new PopupControlContentControl this[int i] {
			get {
				return (PopupControlContentControl)base[i];
			}
		}
		protected override Type GetChildType() {
			return typeof(PopupControlContentControl);
		}
	}
	public class PopupControlContentControl : ContentControl {
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		public PopupControlContentControl()
			: base() {
		}
		protected override bool HasRootTag() {
			return DesignMode;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(DesignMode)
				Height = Unit.Percentage(100);
		}
	}
}
