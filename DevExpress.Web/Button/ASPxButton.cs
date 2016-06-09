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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum ButtonRenderMode { Button = 0, Link = 1 }
	[DXWebToolboxItem(DXToolboxItemKind.Free), 
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxButton"),
	ToolboxData("<{0}:ASPxButton runat=\"server\" Text=\"ASPxButton\"></{0}:ASPxButton>"),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxButton.bmp"),
	Designer("DevExpress.Web.Design.ASPxButtonDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull), 
	DefaultEvent("Click"), DefaultProperty("Text"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon)]
	public class ASPxButton : ASPxWebControl, IButtonControl, IRequiresLoadPostDataControl, IAssociatedControlID {
		protected internal const string ButtonScriptsResourcePath = "DevExpress.Web.Scripts.Editors.";
		protected internal const string ButtonScriptResourceName = ButtonScriptsResourcePath + "Button.js";
		protected internal const string ButtonCellPostfix = "B";
		protected internal const string ButtonLightweightPostfix = "";
		protected internal const string ContentDivPostfix = "CD";
		protected internal const string TextButtonCellPostfix = "TC";
		protected internal const string TextButtonCellFullPostfix = ButtonCellPostfix + TextButtonCellPostfix;
		protected internal const string ButtonPostfixNative = "";
		protected internal const string ButtonImageIdPostfix = "Img";
		protected internal const string ButtonCheckedPostfix = "_CH";
		protected internal const string ButtonInputPostfix = "I";
		private static readonly object EventClick = new object();
		private static readonly object EventCheckedChanged = new object();
		private static readonly object EventCommand = new object();
		private static readonly object EventValidationContainerResolve = new object();
		private ButtonControl buttonControl = null;
		public ASPxButton()
			: base() {
			AutoPostBack = true;
		}
		protected ASPxButton(ASPxWebControl ownerControl)
			: base(ownerControl) {
			AutoPostBack = true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonNative"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonAllowFocus"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool AllowFocus {
			get { return GetBoolProperty("AllowFocus", true); }
			set {
				SetBoolProperty("AllowFocus", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonChecked"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, Bindable(true)]
		public bool Checked {
			get { return GetBoolProperty("Checked", false); }
			set {
				if(value != Checked) {
					SetChecked(value);
					if(value)
						ClearButtonGroupChecked(false);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonGroupName"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string GroupName {
			get { return GetStringProperty("GroupName", ""); }
			set { SetStringProperty("GroupName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonUseSubmitBehavior"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public virtual bool UseSubmitBehavior {
			get { return GetBoolProperty("UseSubmitBehavior", true); }
			set {
				SetBoolProperty("UseSubmitBehavior", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonValidationContainerID"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValidationContainerID {
			get { return GetStringProperty("ValidationContainerID", ""); }
			set { SetStringProperty("ValidationContainerID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonValidateInvisibleEditors"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(false), Localizable(false), AutoFormatDisable]
		public bool ValidateInvisibleEditors {
			get { return GetBoolProperty("ValidateInvisibleEditors", false); }
			set { SetBoolProperty("ValidateInvisibleEditors", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ButtonClientSideEvents ClientSideEvents {
			get { return (ButtonClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[Obsolete("Use the ClientSideEvents.Click property instead."), Category("Client-Side"),
		DefaultValue(""), Localizable(false), AutoFormatDisable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(true)]
		public string OnClientClick {
			get { return ClientSideEvents.Click; }
			set { ClientSideEvents.Click = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty)]
		public CheckedButtonImageProperties Image {
			get { return Images.Image; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonImagePosition"),
#endif
		Category("Appearance"), DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition ImagePosition {
			get { return (ImagePosition)GetEnumProperty("ImagePosition", ImagePosition.Left); }
			set {
				SetEnumProperty("ImagePosition", ImagePosition.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonImageSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ImageSpacing {
			get { return ((AppearanceStyle)ControlStyle).ImageSpacing; }
			set {
				((AppearanceStyle)ControlStyle).ImageSpacing = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[Category("Images"), DefaultValue(""), Localizable(false), AutoFormatDisable, Bindable(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ImageUrl {
			get { return Image.Url; }
			set { Image.Url = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonSpriteImageUrl"),
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
	DevExpressWebLocalizedDescription("ASPxButtonSpriteCssFilePath"),
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
	DevExpressWebLocalizedDescription("ASPxButtonRenderMode"),
#endif
		Category("Layout"), DefaultValue(ButtonRenderMode.Button), AutoFormatDisable]
		public ButtonRenderMode RenderMode {
			get { return (ButtonRenderMode)GetEnumProperty("RenderMode", ButtonRenderMode.Button); }
			set {
				SetEnumProperty("RenderMode", ButtonRenderMode.Button, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonPaddings"),
#endif
		Category("Layout"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Paddings Paddings {
			get { return (ControlStyle as AppearanceStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return ((AppearanceStyle)ControlStyle).HorizontalAlign; }
			set { ((AppearanceStyle)ControlStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonVerticalAlign"),
#endif
		Category("Layout"), DefaultValue(VerticalAlign.NotSet), AutoFormatEnable]
		public VerticalAlign VerticalAlign {
			get { return ((AppearanceStyle)ControlStyle).VerticalAlign; }
			set { ((AppearanceStyle)ControlStyle).VerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonWrap"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean Wrap {
			get { return ((AppearanceStyle)ControlStyle).Wrap; }
			set { ((AppearanceStyle)ControlStyle).Wrap = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCheckedStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual AppearanceSelectedStyle CheckedStyle {
			get { return ((ButtonControlStyle)ControlStyle).CheckedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonPressedStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual AppearanceSelectedStyle PressedStyle {
			get { return ((ButtonControlStyle)ControlStyle).PressedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonHoverStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public virtual AppearanceSelectedStyle HoverStyle {
			get { return ((AppearanceStyle)ControlStyle).HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCausesValidation"),
#endif
	   Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public virtual bool CausesValidation {
			get { return GetBoolProperty("CausesValidation", true) && !IsCheckable(); }
			set { SetBoolProperty("CausesValidation", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCommandArgument"),
#endif
		Category("Behavior"), Localizable(false), DefaultValue(""), Bindable(true), AutoFormatDisable]
		public string CommandArgument {
			get { return GetStringProperty("CommandArgument", ""); }
			set { SetStringProperty("CommandArgument", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCommandName"),
#endif
		Category("Behavior"), Localizable(false), DefaultValue(""), AutoFormatDisable]
		public string CommandName {
			get { return GetStringProperty("CommandName", ""); }
			set { SetStringProperty("CommandName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonPostBackUrl"),
#endif
		UrlProperty(), Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string PostBackUrl {
			get { return GetStringProperty("PostBackUrl", ""); }
			set { SetStringProperty("PostBackUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonText"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable, Bindable(true)]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set {
				SetStringProperty("Text", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonFocusRectPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings FocusRectPaddings {
			get { return Styles.FocusRectStyle.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonFocusRectBorder"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper FocusRectBorder {
			get { return Styles.FocusRectStyle.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonValidationGroup"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValidationGroup {
			get { return GetStringProperty("ValidationGroup", ""); }
			set { SetStringProperty("ValidationGroup", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonClick"),
#endif
		Category("Action")]
		public event EventHandler Click
		{
			add { Events.AddHandler(EventClick, value); }
			remove { Events.RemoveHandler(EventClick, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCheckedChanged"),
#endif
		Category("Action")]
		public event EventHandler CheckedChanged
		{
			add { Events.AddHandler(EventCheckedChanged, value); }
			remove { Events.RemoveHandler(EventCheckedChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonCommand"),
#endif
		Category("Action")]
		public event CommandEventHandler Command
		{
			add { Events.AddHandler(EventCommand, value); }
			remove { Events.RemoveHandler(EventCommand, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonValidationContainerResolve"),
#endif
		Category("Events")]
		public event EventHandler<ControlResolveEventArgs> ValidationContainerResolve
		{
			add { Events.AddHandler(EventValidationContainerResolve, value); }
			remove { Events.RemoveHandler(EventValidationContainerResolve, value); }
		}
		protected ButtonControl ButtonControl {
			get { return this.buttonControl; }
		}
		protected internal ButtonControlStyles RenderStyles {
			get { return (ButtonControlStyles)RenderStylesInternal; }
		}
		protected ButtonControlStyles Styles {
			get { return (ButtonControlStyles)StylesInternal; }
		}
		protected ButtonImages Images {
			get { return (ButtonImages)ImagesInternal; }
		}
		protected override void OnLoad(EventArgs e) {
			if(Page != null && !Page.IsPostBack && !Page.IsCallback && IsCheckable() && Checked)
				ClearButtonGroupChecked(false);
			base.OnLoad(e);
		}
		public override void Focus() {
			if(AllowFocus) base.Focus();
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ButtonClientSideEvents();
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[EventClick] != null;
		}
		protected internal bool IsLink() {
			return RenderMode == ButtonRenderMode.Link;
		}
		protected bool IsCheckable() {
			return !string.IsNullOrEmpty(GroupName);
		}
		protected List<ASPxButton> GetCheckedGroupList() {
			return GetCheckedGroupList(Page);
		}
		protected List<ASPxButton> GetCheckedGroupList(Control control) {
			List<ASPxButton> list = new List<ASPxButton>();
			if(control != null){
			for(int i = 0; i < control.Controls.Count; i++) {
				ASPxButton btn = control.Controls[i] as ASPxButton;
				if(btn != null && btn.GroupName == GroupName)
					list.Add(btn);
				list.AddRange(GetCheckedGroupList(control.Controls[i]));
			}
			}
			return list;
		}
		protected void ClearButtonGroupChecked(bool raiseCheckedChanged){
			if(DesignMode) return;
			List<ASPxButton> list = GetCheckedGroupList();
			foreach(ASPxButton btn in list){
				if(btn != this && btn.Checked) {
					btn.SetChecked(false);
					if(raiseCheckedChanged)
						btn.OnCheckedChanged(EventArgs.Empty);
				}
			}
		}
		protected void SetChecked(bool value) {
			SetBoolProperty("Checked", false, value);
		}
		protected void OnClick(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventClick];
			if(handler != null)
				handler(this, e);
			this.RaiseBubbleEvent(this, e);
		}
		protected void OnCheckedChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventCheckedChanged];
			if(handler != null)
				handler(this, e);
			this.RaiseBubbleEvent(this, e);
		}
		protected void OnCommand(CommandEventArgs e) {
			CommandEventHandler handler = (CommandEventHandler)base.Events[EventCommand];
			if(handler != null)
				handler(this, e);
			base.RaiseBubbleEvent(this, e);
		}
		protected void OnValidationContainerResolve(ControlResolveEventArgs e) {
			EventHandler<ControlResolveEventArgs> handler = (EventHandler<ControlResolveEventArgs>)Events[EventValidationContainerResolve];
			if(handler != null)
				handler(this, e);
		}
		protected override void ClearControlFields() {
			this.buttonControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.buttonControl = new ButtonControl(this);
			Controls.Add(ButtonControl);
		}
		protected internal short GetTabIndex() {
			return AllowFocus ? TabIndex : (short)-1;
		}
		protected internal string GetText() {
			return HtmlEncode(Text);
		}
		protected internal string GetValue() {
			return Text.Replace("<", "&lt;").Replace(">", "&gt;");
		}
		protected internal string GetOnClickScript() {
			string scr = "";
			if(!HasFunctionalityScripts() && Enabled && AutoPostBack)
				scr = GetPostBackEventReference(false);
			return scr;
		}
		protected bool GetUseSubmitBehavior() {
			return IsLink() ? false : UseSubmitBehavior;
		}
		protected string GetPostBackEventReference(bool wrapWithAnonymFunc) {
			string actionUrl = !string.IsNullOrEmpty(PostBackUrl) ? HttpUtility.UrlPathEncode(ResolveClientUrl(PostBackUrl)) : "";
			return RenderUtils.GetPostBackEventReference(this, "", CausesValidation, ValidationGroup, actionUrl, !GetUseSubmitBehavior(), true, false, wrapWithAnonymFunc);
		}
		protected bool HeedRenderPostBackEventReference() {
			if(!string.IsNullOrEmpty(PostBackUrl))
				return true;
			if(AutoPostBack || !ClientSideEvents.IsEmpty()) { 
				if(CausesValidation && (Page == null || Page.GetValidators(ValidationGroup).Count > 0))
					return true;
				if(!GetUseSubmitBehavior())
					return true;
			}
			return false;
		}
		protected internal bool HasIDs() {
			return Enabled;
		}
		protected bool IsClientStateProcessing() {
			return HasFunctionalityScripts() && !DesignMode;
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || Enabled && (CausesValidation || AllowFocus || IsCheckable() && !AutoPostBack) ||
				HasEvents() && Events[EventClick] != null;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxButton), ButtonScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientButton";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(string.IsNullOrEmpty(Text))
				stb.Append(localVarName + ".isTextEmpty = true;\n");
			if(IsEnabled()) {
				if(!AllowFocus)
					stb.Append(String.Format("{0}.allowFocus = false;\n", localVarName));
				else if(!IsNativeRender() && !IsLink())
					AddFocusedItems(stb);
				if(!UseSubmitBehavior)
					stb.Append(localVarName + ".useSubmitBehavior = false;\n");
				if(HeedRenderPostBackEventReference()) {
					string autoPostBackFunctionScript = GetPostBackEventReference(true);
					if(!string.IsNullOrEmpty(autoPostBackFunctionScript))
						stb.AppendFormat("{0}.autoPostBackFunction = {1};\n", localVarName, autoPostBackFunctionScript);
				}
				if(CausesValidation) {
					if(ValidationContainerID != "")
						stb.AppendFormat("{0}.validationContainerID = \"{1}\";\n", localVarName,
							RenderUtils.GetReferentControlClientID(this, ValidationContainerID, OnValidationContainerResolve));
					if(ValidationGroup != "")
						stb.AppendFormat("{0}.validationGroup = \"{1}\";\n", localVarName, ValidationGroup);
					if(ValidateInvisibleEditors)
						stb.Append(localVarName + ".validateInvisibleEditors = true;\n");
				}
				else
					stb.AppendFormat("{0}.causesValidation = false;\n", localVarName);
				if(!IsNativeRender() && !IsLink() && IsClientStateProcessing()) {
					if(IsCheckable()) {
						stb.AppendFormat("{0}.groupName = '{1}';\n", localVarName, GroupName);
						if(Checked)
							stb.AppendFormat("{0}.checked = true;\n", localVarName);
						if(HasCheckedScripts())
							AddCheckedItems(stb);
					}
				}
			}
		}
		protected override string GetClientObjectStateInputID() {
			return UniqueID + "$State";
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && Events[EventClick] != null)
				eventNames.Add("Click");
		}
		protected bool HasCheckedScripts() {
			return (IsCheckable() && !IsNativeRender() && !GetCheckedStyle().IsEmpty || !GetImage().IsEmptyChecked) && IsStateScriptEnabled() && !IsLink();
		}
		protected override bool HasPressedScripts() {
			return (!GetPressedStyle().IsEmpty || !GetImage().IsEmptyPressed) && IsStateScriptEnabled();
		}
		protected override bool HasHoverScripts() {
			return (!GetHoverStyle().IsEmpty || !GetImage().IsEmptyHottracked) && IsStateScriptEnabled();
		}
		protected override bool HasSelectedScripts() {
			return !IsLink(); 
		}
		protected void AddCheckedItems(StringBuilder stb) {
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			helper.AddStyle(GetButtonCheckedCssStyle(), GetStateItemName(),
				GetStatePostfixes(), GetImage().GetCheckedScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			helper.GetCreateSelectedScript(stb);
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetButtonDisabledCssStyle(), GetStateItemName(),
				GetStatePostfixes(), GetImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetButtonPressedCssStyle(), GetStateItemName(),
				GetStatePostfixes(), GetImage().GetPressedScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetButtonHoverCssStyle(), GetStateItemName(),
				GetStatePostfixes(), GetImage().GetHottrackedScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
		}
		protected void AddFocusedItems(StringBuilder stb) {
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			helper.AddStyle(GetButtonContentDivFocusedCssStyle(), ContentDivPostfix, new string[0], IsEnabled());
			helper.GetCreateSelectedScript(stb);
		}
		protected string GetStateItemName() {
			if(IsNativeRender())
				return ButtonPostfixNative;
			else 
				return ButtonLightweightPostfix;
		}
		protected string[] GetStatePostfixes() {
			return new string[] { "", TextButtonCellPostfix };
		}
		protected override ImagesBase CreateImages() {
			return new ButtonImages(this);
		}
		protected internal CheckedButtonImageProperties GetImage() {
			CheckedButtonImageProperties ret = new CheckedButtonImageProperties();
			ret.CopyFrom(Images.GetImageProperties(Page, ButtonImages.ImageName));
			if(!Enabled) {
				if(!string.IsNullOrEmpty(ret.UrlDisabled))
					ret.Url = ret.UrlDisabled;
				if(!ret.SpriteProperties.DisabledLeft.IsEmpty)
					ret.SpriteProperties.Left = ret.SpriteProperties.DisabledLeft;
				if(!ret.SpriteProperties.DisabledTop.IsEmpty)
					ret.SpriteProperties.Top = ret.SpriteProperties.DisabledTop;
			}
			else if(Checked && (!IsClientStateProcessing() || !IsCheckable())) {
				if(!string.IsNullOrEmpty(ret.UrlChecked))
					ret.Url = ret.UrlChecked;
				if(!ret.SpriteProperties.CheckedLeft.IsEmpty)
					ret.SpriteProperties.Left = ret.SpriteProperties.CheckedLeft;
				if(!ret.SpriteProperties.CheckedTop.IsEmpty)
					ret.SpriteProperties.Top = ret.SpriteProperties.CheckedTop;
			}
			return ret;
		}
		protected override Style CreateControlStyle() {
			return new ButtonControlStyle();
		}
		protected override StylesBase CreateStyles() {
			return new ButtonControlStyles(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), ASPxEditBase.EditDefaultCssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), ASPxEditBase.EditSystemCssResourceName);
		}
		protected internal ButtonControlStyle GetButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(GetButtonStyleInternal());
			if(Checked && (!IsClientStateProcessing() || !IsCheckable()))
				style.CopyFrom(GetCheckedStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal AppearanceStyle GetButtonStyleInternal() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			return style;
		}
		protected internal AppearanceStyle GetButtonContentDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonContentDivStyle());
			style.Paddings.CopyFrom(RenderStyles.FocusRectStyle.Paddings);
			if(!Height.IsEmpty) {
				style.Paddings.PaddingTop = Unit.Empty;
				style.Paddings.PaddingBottom = Unit.Empty;
			}
			style.Border.CopyFrom(GetButtonContentDivFocusedStyle().Border);
			if(!style.Border.BorderColor.IsEmpty)
				style.Border.BorderColor = Color.Transparent;
			style.BorderLeft.CopyFrom(GetButtonContentDivFocusedStyle().BorderLeft);
			if(!style.BorderLeft.BorderColor.IsEmpty)
				style.BorderLeft.BorderColor = Color.Transparent;
			style.BorderRight.CopyFrom(GetButtonContentDivFocusedStyle().BorderRight);
			if(!style.BorderRight.BorderColor.IsEmpty)
				style.BorderRight.BorderColor = Color.Transparent;
			style.BorderTop.CopyFrom(GetButtonContentDivFocusedStyle().BorderTop);
			if(!style.BorderTop.BorderColor.IsEmpty)
				style.BorderTop.BorderColor = Color.Transparent;
			style.BorderBottom.CopyFrom(GetButtonContentDivFocusedStyle().BorderBottom);
			if(!style.BorderBottom.BorderColor.IsEmpty)
				style.BorderBottom.BorderColor = Color.Transparent;
			return style;
		}
		protected internal AppearanceStyle GetButtonContentDivFocusedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonContentDivFocusedStyle());
			style.Border.CopyFrom(RenderStyles.FocusRectStyle.Border);
			return style;
		}
		protected internal AppearanceStyle GetCheckedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonCheckedStyle());
			style.CopyFrom(RenderStyles.Style.CheckedStyle);
			style.CopyFrom(CheckedStyle);
			return style;
		}
		protected internal AppearanceStyle GetPressedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonPressedStyle(IsLink()));
			style.CopyFrom(RenderStyles.Style.PressedStyle);
			style.CopyFrom(PressedStyle);
			return style;
		}
		protected internal AppearanceStyle GetHoverStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonHoverStyle(IsLink()));
			style.CopyFrom(RenderStyles.Style.HoverStyle);
			style.CopyFrom(HoverStyle);
			return style;
		}
		protected internal AppearanceStyle GetButtonCheckedCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			AppearanceStyle buttonStyle = GetButtonStyle();
			style.CopyFrom(GetCheckedStyle());
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(buttonStyle, style, buttonStyle.Paddings));
			return style;
		}
		protected AppearanceStyleBase GetButtonPressedCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			AppearanceStyle buttonStyle = GetButtonStyle();
			style.CopyFrom(GetPressedStyle());
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(buttonStyle, style, buttonStyle.Paddings));
			return style;
		}
		protected AppearanceStyleBase GetButtonHoverCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			AppearanceStyle buttonStyle = GetButtonStyle();
			style.CopyFrom(GetHoverStyle());
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(buttonStyle, style, buttonStyle.Paddings));
			return style;
		}
		protected AppearanceStyleBase GetButtonContentDivFocusedCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			AppearanceStyle buttonStyle = GetButtonContentDivStyle();
			style.CopyFrom(GetButtonContentDivFocusedStyle());
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(buttonStyle, style, buttonStyle.Paddings));
			return style;
		}
		protected internal DisabledStyle GetButtonDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(RenderStyles.Style.DisabledStyle);
			style.CopyFrom(GetDisabledStyle());
			return style;
		}
		protected internal Unit GetImageSpacing() {
			return GetButtonStyleInternal().ImageSpacing; 
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			if(CausesValidation) {
				Control validationContainer = RenderUtils.GetReferentControl(this, ValidationContainerID, OnValidationContainerResolve);
				if(validationContainer == null)
					validationContainer = Page;
				ASPxEdit.ValidateEditorsInContainer(validationContainer, ValidationGroup, ValidateInvisibleEditors);
				Page.Validate(ValidationGroup);
			}
			if(!IsCheckedChangedProcessedOnClient(eventArgument) && !string.IsNullOrEmpty(GroupName)) {
				List<ASPxButton> list = GetCheckedGroupList();
				if(list.Count == 1)
					SetChecked(!Checked);
				else {
					SetChecked(true);
					ClearButtonGroupChecked(true);
				}
				OnCheckedChanged(EventArgs.Empty);
			}
			OnCommand(new CommandEventArgs(CommandName, CommandArgument));
			OnClick(EventArgs.Empty);
		}
		private bool IsCheckedChangedProcessedOnClient(string postBackEventArg) {
			return postBackEventArg != "CheckedChanged";
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(GetUseSubmitBehavior() && postCollection[UniqueID] != null && Page != null)
				Page.RegisterRequiresRaiseEvent(this);
			if(ClientObjectState == null)
				return false;
			bool newChecked = GetClientObjectStateValue<bool>("checked");
			bool raiseCheckedChanged = newChecked != Checked;
			SetChecked(newChecked);
			return raiseCheckedChanged;
		}
		protected override void RaisePostDataChangedEvent() {
			OnCheckedChanged(EventArgs.Empty);
		}
		protected override string GetSkinControlName() {
			return "Editors";
		}
		protected override bool IsNativeSupported() {
			return true;
		}
		#region IAssociatedControlID Members
		string IAssociatedControlID.ClientID() {
			return IsNativeRender() ? ClientID : string.Format("{0}_{1}", ClientID, ButtonInputPostfix);
		}
		#endregion
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsLink() && IsAriaSupported() && IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "role", "button");
		}
	}
}
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class ASPxCommandButton : ASPxButton {
		public ASPxCommandButton() {
			ClickArguments = new List<object>();
		}
		public override bool Initialized { get { return DesignMode || base.Initialized; } }
		public override bool EnableViewState { get { return false; } set { } }
		protected override bool AutoPostBackInternal { get { return false; } set { } }
		protected override string GetStartupScript() {
			return null;
		}
		protected List<object> ClickArguments { get; private set; }
		public void AddClickArguments(params object[] clickArgs) {
			if(clickArgs == null)
				return;
			foreach(var arg in clickArgs) {
				ClickArguments.Add(arg);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(EncodeHtml)
				Attributes.Add("data-encodeHtml", HtmlConvertor.ToScript(EncodeHtml));
			if(ClickArguments != null && ClickArguments.Count > 0)
				Attributes.Add("data-args", HtmlConvertor.ToJSON(ClickArguments));
			if(IsNative())
				Attributes.Add("data-isNative", HtmlConvertor.ToJSON(IsNative()));
		}
	}
}
