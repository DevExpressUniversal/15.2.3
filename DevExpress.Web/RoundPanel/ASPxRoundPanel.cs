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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum View { Standard, GroupBox };
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxRoundPanel"),
	ToolboxData("<{0}:ASPxRoundPanel Width=\"200px\" ShowCollapseButton=\"true\" runat=\"server\"></{0}:ASPxRoundPanel>"),
	Designer("DevExpress.Web.Design.ASPxRoundPanelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxRoundPanel.bmp")
]
	public class ASPxRoundPanel : DevExpress.Web.ASPxPanelBase, IRequiresLoadPostDataControl {
		private const string TopEdgeCssClassName = "TE";
		private const string NoHeaderTopEdgeCssClassName = "NHTE";
		private const string HeaderLeftEdgeCssClassName = "HLE";
		private const string HeaderRightEdgeCssClassName = "HRE";
		private const string LeftEdgeCssClassName = "LE";
		private const string RightEdgeCssClassName = "RE";
		private const string BottomEdgeCssClassName = "BE";
		private const string ContentCellCssClassName = "content";
		private const string ContentRowCssClassName = "CR";
		private const string HeaderImageCssClassName = "HI";
		private const string HeaderImageRtlCssClassName = "HIR";
		private const string ContentBottomRowCssClassName = "CBR";
		private const string HeaderTextCssClassName = "HT";
		private const string CustomCornerImageClassName = "CI";
		private const string CollapsedCssClassName = "Collapsed";
		private const string CollapseButtonCssClassName = "CollapseButton";
		private const string CollapseButtonRightToLeftCssClassName = "CollapseButtonRtl";
		private const string ContentWrapperCssClassName = "CW";
		private const string AnimationWrapperCssClassName = "AW";
		private const string HeaderSeparatorCssClassName = "HS";
		private const string HasDefaultImagesCssClassName = "-hasDefaultImages";
		private const string HasNotCollapsingCssClassName = "-noCollapsing";
		private const string HeaderContentWrapperCssClassName = "HCW";
		protected internal const string CollapseButtonIdPostfix = "Img";
		protected internal const string RoundPanelScriptResourceName = WebScriptsResourcePath + "RoundPanel.js";
		protected internal const string HeaderTextContainerID = "RPHT";
		protected internal const string HeaderElementContainerId = "HC";
		protected internal const string RoundPanelContentID = "RPC";
		protected internal const string GroupBoxCaptionID = "GBC";
		protected internal const string HeaderTemplateContainerID = "HTC";
		protected internal const string HeaderContentTemplateContainerID = "HCTC";
		protected internal const string CollapseButtonId = "CB";
		protected internal const string CallbackResultControlId = "CRC";
		internal RPRoundPanelControl roundPanelControl = null;
		private RoundPanelParts parts = null;
		private PanelCornerPart emptyPanelCornerPart;
		private ITemplate headerTemplate = null;
		private ITemplate headerContentTemplate = null;
		private Paddings contentPaddings = null;
		public ASPxRoundPanel()
			: base() {
		}
		protected ASPxRoundPanel(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelLoadContentViaCallback"),
#endif
		Category("Behavior"), DefaultValue(false)]
		public virtual bool LoadContentViaCallback {
			get {
				return GetBoolProperty("LoadContentViaCallback", false);
			}
			set {
				SetBoolProperty("LoadContentViaCallback", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelAllowCollapsingByHeaderClick"),
#endif
		Category("Behavior"), DefaultValue(false)]
		public virtual bool AllowCollapsingByHeaderClick {
			get {
				return GetBoolProperty("AllowCollapsingByHeaderClick", false);
			}
			set {
				SetBoolProperty("AllowCollapsingByHeaderClick", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelContentPaddings"),
#endif
		Category("Layout"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings ContentPaddings {
			get {
				if(contentPaddings == null)
					contentPaddings = new Paddings();
				return contentPaddings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelGroupBoxCaptionOffsetX"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), "")]
		public Unit GroupBoxCaptionOffsetX {
			get {
				return GetUnitProperty("GroupBoxCaptionOffsetX", Unit.Empty);
			}
			set {
				SetUnitProperty("GroupBoxCaptionOffsetX", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelGroupBoxCaptionOffsetY"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), "")]
		public Unit GroupBoxCaptionOffsetY {
			get {
				return GetUnitProperty("GroupBoxCaptionOffsetY", Unit.Empty);
			}
			set {
				SetUnitProperty("GroupBoxCaptionOffsetY", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeight"),
#endif
		AutoFormatDisable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderNavigateUrl"),
#endif
		DefaultValue(""), UrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), AutoFormatDisable]
		public string HeaderNavigateUrl {
			get {
				return GetStringProperty("HeaderNavigateUrl", "");
			}
			set {
				SetStringProperty("HeaderNavigateUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderText"),
#endif
		DefaultValue("Header"), Localizable(true), AutoFormatDisable]
		public string HeaderText {
			get { return GetStringProperty("HeaderText", "Header"); }
			set { SetStringProperty("HeaderText", "Header", value); }
		}
		private static readonly object notSetHorizontalAlign = HorizontalAlign.NotSet;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet)]
		public HorizontalAlign HorizontalAlign {
			get { return ((AppearanceStyle)ControlStyle).HorizontalAlign; }
			set { ((AppearanceStyle)ControlStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelContentHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), "")]
		public Unit ContentHeight {
			get { return GetUnitProperty("ContentHeight", Unit.Empty); }
			set { SetUnitProperty("ContentHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelShowHeader"),
#endif
		Category("Appearance"), DefaultValue(true)]
		public virtual bool ShowHeader {
			get {
				return GetBoolProperty("ShowHeader", true);
			}
			set {
				SetBoolProperty("ShowHeader", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelShowCollapseButton"),
#endif
		Category("Appearance"), DefaultValue(false)]
		public virtual bool ShowCollapseButton {
			get {
				return GetBoolProperty("ShowCollapseButton", false);
			}
			set {
				SetBoolProperty("ShowCollapseButton", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelEnableAnimation"),
#endif
		Category("Appearance"), DefaultValue(true)]
		public virtual bool EnableAnimation {
			get {
				return GetBoolProperty("EnableAnimation", true);
			}
			set {
				SetBoolProperty("EnableAnimation", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelCollapsed"),
#endif
		Category("Appearance"), DefaultValue(false)]
		public virtual bool Collapsed {
			get {
				return GetBoolProperty("Collapsed", false);
			}
			set {
				if(Collapsed == value)
					return;
				SetBoolProperty("Collapsed", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelTarget"),
#endif
		DefaultValue(""), TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get {
				return GetStringProperty("Target", "");
			}
			set {
				SetStringProperty("Target", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelView"),
#endif
		Category("Appearance"), DefaultValue(typeof(View), "Standard")]
		public View View {
			get {
				return (View)GetEnumProperty("View", View.Standard);
			}
			set {
				SetEnumProperty("View", View.Standard, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelWidth"),
#endif
		AutoFormatDisable]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelImageFolder"),
#endif
		Category("Images"), DefaultValue(""), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelSpriteImageUrl"),
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
	DevExpressWebLocalizedDescription("ASPxRoundPanelSpriteCssFilePath"),
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
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderImage"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties HeaderImage {
			get { return Images.Header; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelCollapseButtonImage"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CollapseButtonImageProperties CollapseButtonImage {
			get { return Images.CollapseButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelExpandButtonImage"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandButtonImage {
			get { return Images.ExpandButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelCornerRadius"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), "")]
		public Unit CornerRadius {
			get { return GetUnitProperty("CornerRadius", Unit.Empty); }
			set {
				if(CornerRadius == value)
					return;
				CommonUtils.CheckNegativeValue(value.Value, "CornerRadius");
				SetUnitProperty("CornerRadius", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderStyle HeaderStyle {
			get { return Styles.Header; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelCollapseButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CollapseButtonStyle CollapseButtonStyle {
			get { return Styles.CollapseButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelGroupBoxHeaderStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GroupBoxHeaderStyle GroupBoxHeaderStyle {
			get { return Styles.GroupBoxHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelLinkStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelContent"),
#endif
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart Content {
			get { return Parts.Content; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderContent"),
#endif
		Category("Parts"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart HeaderContent {
			get { return Parts.HeaderContent; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new RoundPanelClientSideEvents ClientSideEvents {
			get { return (RoundPanelClientSideEvents)base.ClientSideEventsInternal; }
		}
		[Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelContentCallback"),
#endif
		Category("Action")]
		public event RoundPanelContentCallbackEventHandler ContentCallback {
			add { Events.AddHandler(CallbackEvent, value); }
			remove { Events.RemoveHandler(CallbackEvent, value); }
		}
		private static readonly object CallbackEvent = new object();
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[CallbackEvent] != null;
		}
		[Browsable(false), DefaultValue(null), TemplateContainer(typeof(RoundPanelHeaderTemplateContainer)),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderTemplate {
			get {
				return headerTemplate;
			}
			set {
				headerTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), TemplateContainer(typeof(RoundPanelHeaderContentTemplateContainer)),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderContentTemplate {
			get {
				return headerContentTemplate;
			}
			set {
				headerContentTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RoundPanelParts Parts {
			get {
				if(this.parts == null)
					this.parts = new RoundPanelParts(this);
				return this.parts;
			}
		}
		protected RoundPanelImages Images {
			get { return (RoundPanelImages)ImagesInternal; }
		}
		protected RoundPanelStyles Styles {
			get { return (RoundPanelStyles)StylesInternal; }
		}
		protected internal bool IsGroupBox {
			get {
				return View == View.GroupBox;
			}
		}
		public new virtual Control FindControl(string id) {
			string templateContainerId =
				HeaderTemplate != null ? HeaderTemplateContainerID : (HeaderContentTemplate != null ? HeaderContentTemplateContainerID : null);
			if(!string.IsNullOrEmpty(templateContainerId)) {
				Control control = TemplateContainerBase.FindTemplateControl(this, templateContainerId, id);
				if(control != null)
					return control;
			}
			return base.FindControl(id);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRoundPanel";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(string.IsNullOrEmpty(HeaderText))
				stb.Append(localVarName + ".headerTextEmpty = true;\n");
			if(!EnableAnimation)
				stb.Append(localVarName + ".enableAnimation = false;\n");
			if(IsCallBacksEnabled())
				stb.Append(localVarName + ".loadContentViaCallback = true;\n");
			if(View == View.GroupBox)
				stb.Append(localVarName + ".isGroupBox = true;\n");
			if(Collapsed)
				stb.Append(localVarName + ".collapsed = true;\n");
			if(AllowCollapsingByHeaderClick)
				stb.Append(localVarName + ".allowCollapsingByHeaderClick = true;\n");
			if(CanRenderCollapseButton()) {
				HeaderStyle headerStyle = GetHeaderStyle();
				if(headerStyle.VerticalAlign != VerticalAlign.NotSet)
					stb.AppendFormat("{0}.headerVerticalAlign = {1};\n", localVarName, (int)headerStyle.VerticalAlign);
				AppendButtonImageState(stb, localVarName, "expandImageProperties", GetExpandButtonImageProperties());
				AppendButtonImageState(stb, localVarName, "collapseImageProperties", GetCollapseButtonImageProperties());
			}
		}
		void AppendButtonImageState(StringBuilder stb, string localVarName, string fieldName, ImagePropertiesBase imageProperties) {
			Dictionary<string, object> state = new Dictionary<string, object>();
			AddNonEmptyValue(state, "s", imageProperties.Url);
			AddNonEmptyValue(state, "a", imageProperties.AlternateText);
			AddNonEmptyValue(state, "t", imageProperties.ToolTip);
			AddNonEmptyValue(state, "w", imageProperties.Width);
			AddNonEmptyValue(state, "h", imageProperties.Height);
			AddNonEmptyValue(state, "so", imageProperties.GetScriptObject(Page));
			stb.AppendFormat("{0}.{1} = {2};\r\n", localVarName, fieldName, HtmlConvertor.ToJSON(state));
		}
		void AddNonEmptyValue(Dictionary<string, object> dictionary, string key, string value) {
			if(!string.IsNullOrEmpty(value))
				dictionary.Add(key, value);
		}
		void AddNonEmptyValue(Dictionary<string, object> dictionary, string key, object value) {
			if(value != null)
				dictionary.Add(key, value);
		}
		void AddNonEmptyValue(Dictionary<string, object> dictionary, string key, Unit value) {
			if(!value.IsEmpty)
				dictionary.Add(key, value);
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxRoundPanel), RoundPanelScriptResourceName, HasPanelFunctionalityScripts());
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RoundPanelClientSideEvents();
		}
		protected override bool IsAnimationScriptNeeded() {
			return IsCollapsingAvailable() && EnableAnimation;
		}
		public override bool IsClientSideAPIEnabled() {
			return IsCollapsingEnabled() || base.IsClientSideAPIEnabled();
		}
		protected internal virtual bool CanBeCollapsed() {
			return ShowHeader && View != View.GroupBox;
		}
		protected internal virtual bool IsCollapsingAvailable() {
			return IsEnabled() && CanBeCollapsed();
		}
		protected internal virtual bool CanRenderCollapseButton() {
			return ShowHeader && View != View.GroupBox && HeaderTemplate == null && ShowCollapseButton;
		}
		protected internal virtual bool IsCollapsingEnabled() {
			return IsCollapsingAvailable() && (CanRenderCollapseButton() || AllowCollapsingByHeaderClick || Collapsed);
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			base.AddHoverItems(helper);
			AddItems(helper, GetCollapseButtonHoverCssStyle(), GetCollapseButtonImageProperties().GetHottrackedScriptObject(Page));
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			helper.AddStyle(GetHeaderDisabledStyle(), HeaderElementContainerId, IsEnabled());
			helper.AddStyle(GetDisabledStyle(), "", IsEnabled());
			AddItems(helper, CollapseButtonStyle.GetDisabledStyle(), GetCollapseButtonImageProperties().GetDisabledScriptObject(Page));
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			base.AddPressedItems(helper);
			AddItems(helper, GetCollapseButtonPressedCssStyle(), GetCollapseButtonImageProperties().GetPressedScriptObject(Page));
		}
		protected internal DisabledStyle GetHeaderDisabledStyle() {
			return new DisabledStyle();
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			base.AddSelectedItems(helper);
			AddItems(helper, GetCollapseButtonCheckedCssStyle(), GetCollapseButtonImageProperties().GetCheckedScriptObject(Page));
		}
		protected void AddItems(StateScriptRenderHelper helper, AppearanceStyleBase style, object scriptObject) {
			helper.AddStyle(style, GetCollapseButtonId(), new string[0], scriptObject, CollapseButtonIdPostfix, IsEnabled());
		}
		protected override bool HasHoverScripts() {
			return HasStateManagerScripts(GetCollapseButtonHoverCssStyle());
		}
		protected override bool HasSelectedScripts() {
			return HasStateManagerScripts(GetCollapseButtonCheckedCssStyle());
		}
		protected override bool HasPressedScripts() {
			return HasStateManagerScripts(GetCollapseButtonPressedCssStyle());
		}
		private bool HasStateManagerScripts(AppearanceStyleBase style) {
			return CanRenderCollapseButton() && !style.IsEmpty;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.roundPanelControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.roundPanelControl = new RPRoundPanelControl(this);
			ControlsBase.Add(this.roundPanelControl);
			base.CreateControlHierarchy();
		}
		protected internal string GetInnerControlId(string id) {
			if(MvcUtils.RenderMode == MvcRenderMode.RenderWithSimpleIDs)
				return string.Empty;
			return id;
		}
		protected internal string GetHeaderTemplateContainerID() {
			return GetInnerControlId(HeaderTemplateContainerID);
		}
		protected internal string GetHeaderContentTemplateContainerID() {
			return GetInnerControlId(HeaderContentTemplateContainerID);
		}
		protected internal string GetHeaderTextContainerID() {
			return GetInnerControlId(HeaderTextContainerID);
		}
		protected internal string GetHeaderElementContainerId() {
			return GetInnerControlId(HeaderElementContainerId);
		}
		protected internal string GetRoundPanelContentID() {
			return GetInnerControlId(RoundPanelContentID);
		}
		protected internal string GetCollapseButtonId() {
			return GetInnerControlId(CollapseButtonId);
		}
		protected internal string GetCollapseButtonImageId() {
			return GetInnerControlId(GetCollapseButtonId() + CollapseButtonIdPostfix);
		}
		protected internal string GetGroupBoxCaptionID() {
			return GetInnerControlId(GroupBoxCaptionID);
		}
		protected internal string GetCallbackResultControlId() {
			return GetInnerControlId(CallbackResultControlId);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Parts, ContentPaddings });
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			bool newCollapsed = GetClientObjectStateValue<bool>("collapsed");
			if(Collapsed != newCollapsed) { 
				Collapsed = newCollapsed;
				return true;
			}
			return false;
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected override bool HasRenderCssFile() {
			return true;
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new RoundPanelStyles(this);
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(Styles.GetDefaultControlStyle(ShowHeader, IsGroupBox));
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(ControlStyle);
			if(!style.BackgroundImage.IsEmpty)
				style.BackColor = Color.Transparent;
			if(!ShowHeader)
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, Styles.GetWithoutHeaderControlStyle().CssClass);
		}
		protected internal AppearanceStyleBase GetHeaderLinkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFontAndCursorFrom(GetHeaderStyle());
			style.CopyFrom(LinkStyle.Style);
			return style;
		}
		protected internal HeaderStyle GetHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			AppearanceStyleBase controlStyle = GetControlStyle();
			style.VerticalAlign = controlStyle.VerticalAlign;
			style.CopyFontAndCursorFrom(controlStyle);
			style.CopyFrom(Styles.GetDefaultHeaderStyle());
			style.CopyFrom(IsGroupBox ? GroupBoxHeaderStyle as AppearanceStyleBase : HeaderStyle as AppearanceStyleBase);
			if(IsGroupBox && (GroupBoxHeaderStyle.BackColor.IsEmpty ||
				GroupBoxHeaderStyle.BackColor == Color.Transparent))
				style.BackColor = controlStyle.BackColor;
			MergeDisableStyle(style);
			return style;
		}
		protected internal Paddings GetHeaderPaddings() {
			return GetHeaderStyle().Paddings;
		}
		protected internal ImagePropertiesBase GetButtonImageProperties() {
			return Collapsed ? (ImagePropertiesBase)GetExpandButtonImageProperties() : (ImagePropertiesBase)GetCollapseButtonImageProperties();
		}
		protected CollapseButtonImageProperties GetCollapseButtonImageProperties() {
			return CreateButtonImageProperties<CollapseButtonImageProperties>(RoundPanelImages.CollapseButtonImageName, CollapseButtonImage);
		}
		protected ImageProperties GetExpandButtonImageProperties() {
			return CreateButtonImageProperties<ImageProperties>(RoundPanelImages.ExpandButtonImageName, ExpandButtonImage);
		}
		protected T CreateButtonImageProperties<T>(string imageName, params ImagePropertiesBase[] sources) where T : ImagePropertiesBase, new() {
			T result = new T();
			result.CopyFrom(Images.GetImageProperties(Page, imageName));
			foreach(var source in sources)
				result.CopyFrom(source);
			return result;
		}
		protected internal AppearanceStyleBase GetCollapseButtonStyles() {
			AppearanceStyleBase styles = new AppearanceStyleBase();
			styles.CopyFrom(CollapseButtonStyle);
			return styles;
		}
		protected internal Paddings GetCollapseButtonPaddings() {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Styles.CollapseButton.Paddings);
			return paddings;
		}
		protected internal AppearanceStyleBase GetCollapseButtonHoverCssStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CollapseButtonStyle.GetDefaultHoverStyle());
			style.CopyFrom(CollapseButtonStyle.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetCollapseButtonCheckedCssStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CollapseButtonStyle.GetDefaultCheckedStyle());
			style.CopyFrom(CollapseButtonStyle.CheckedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetCollapseButtonPressedCssStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CollapseButtonStyle.GetDefaultPressedStyle());
			style.CopyFrom(CollapseButtonStyle.PressedStyle);
			return style;
		}
		protected internal Unit GetImageSpacing() {
			return GetHeaderStyle().ImageSpacing;
		}
		protected internal Paddings GetContentPaddings() {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(ContentPaddings);
			return paddings;
		}
		protected override ImagesBase CreateImages() {
			return new RoundPanelImages(this);
		}
		protected void AddSpecificBorderToList(BorderType type, AppearanceStyleBase style,
			List<Border> borders) {
			switch(type) {
				case BorderType.Top:
					borders.Add(style.BorderTop);
					break;
				case BorderType.Bottom:
					borders.Add(style.BorderBottom);
					break;
				case BorderType.Left:
					borders.Add(style.BorderLeft);
					break;
				case BorderType.Right:
					borders.Add(style.BorderRight);
					break;
			}
		}
		protected internal Border GetMergedBorder(List<Border> borders) {
			Border border = new Border();
			for(int i = 0; i < borders.Count; i++)
				border.CopyFrom(borders[i]);
			return border;
		}
		protected internal Border GetBorder(BorderType type) {
			List<Border> borders = new List<Border>();
			borders.Add((ControlStyle as AppearanceStyleBase).Border.GetBorder());
			AddSpecificBorderToList(type, ControlStyle as AppearanceStyle, borders);
			return GetMergedBorder(borders);
		}
		protected internal PanelCornerPart GetCornerPart(PanelPartType part) {
			bool dummy;
			return GetCornerPart(part, out dummy);
		}
		private bool IsCornersAssigned {
			get {
				return !Parts.TopLeftCornerInternal.IsEmpty || !Parts.NoHeaderTopLeftCornerInternal.IsEmpty || !Parts.TopRightCornerInternal.IsEmpty ||
					!Parts.NoHeaderTopRightCornerInternal.IsEmpty || !Parts.BottomLeftCornerInternal.IsEmpty || !Parts.BottomRightCornerInternal.IsEmpty;
			}
		}
		protected internal PanelCornerPart GetCornerPart(PanelPartType part, out bool isResource) {
			isResource = true;
			PanelCornerPart ret = new PanelCornerPart();
			switch(part) {
				case PanelPartType.TopLeftCorner:
					ret.CopyFrom(GetTopLeftCorner(ref isResource));
					break;
				case PanelPartType.TopRightCorner:
					ret.CopyFrom(GetTopRightCorner(ref isResource));
					break;
				case PanelPartType.BottomLeftCorner:
					ret.CopyFrom(Parts.BottomLeftCornerInternal);
					isResource = Parts.BottomLeftCornerInternal.IsEmpty && !IsCornersAssigned;
					break;
				case PanelPartType.BottomRightCorner:
					ret.CopyFrom(Parts.BottomRightCornerInternal);
					isResource = Parts.BottomRightCornerInternal.IsEmpty && !IsCornersAssigned;
					break;
				default:
					throw new ArgumentException("GetCornerPart: invalid corner part type.");
			}
			return ret;
		}
		private PanelCornerPart GetTopLeftCorner(ref bool isResource) {
			return GetTopCorner(Parts.NoHeaderTopLeftCornerInternal, Parts.TopLeftCornerInternal, ref isResource);
		}
		private PanelCornerPart GetTopRightCorner(ref bool isResource) {
			return GetTopCorner(Parts.NoHeaderTopRightCornerInternal, Parts.TopRightCornerInternal, ref isResource);
		}
		private PanelCornerPart GetTopCorner(PanelCornerPart noHeaderTopCorner, PanelCornerPart topCorner, ref bool isResource) {
			PanelCornerPart corner = GetTopCorner(noHeaderTopCorner, topCorner);
			isResource = corner.IsEmpty && !IsCornersAssigned;
			return corner;
		}
		internal PanelCornerPart GetTopCorner(PanelCornerPart noHeaderTopCorner, PanelCornerPart topCorner) {
			return !noHeaderTopCorner.IsEmpty && (!ShowHeader || IsGroupBox) ? noHeaderTopCorner : topCorner;
		}
		protected internal PanelCornerPart GetEmptyPanelCornerPart() {
			if(emptyPanelCornerPart == null)
				emptyPanelCornerPart = new PanelCornerPart();
			return emptyPanelCornerPart;
		}
		protected internal Unit GetGroupBoxCaptionOffsetX() {
			return GroupBoxCaptionOffsetX;
		}
		protected internal Unit GetGroupBoxCaptionOffsetY() {
			return GroupBoxCaptionOffsetY;
		}
		protected internal Border GetHeaderBorder(BorderType type, View view) {
			List<Border> borders = new List<Border>();
			HeaderStyle headerStyle = Styles.GetDefaultHeaderStyle();
			borders.Add(headerStyle.Border.GetBorder());
			if(view == View.Standard)
				AddSpecificBorderToList(type, headerStyle, borders);
			if(view == View.Standard && type != BorderType.Bottom)
				borders.Add((ControlStyle as AppearanceStyleBase).Border.GetBorder());
			AppearanceStyleBase style = IsGroupBox ? GroupBoxHeaderStyle as AppearanceStyleBase
				: HeaderStyle as AppearanceStyleBase;
			borders.Add(style.Border.GetBorder());
			if(view == View.Standard && type != BorderType.Bottom)
				AddSpecificBorderToList(type, ControlStyle as AppearanceStyleBase, borders);
			AddSpecificBorderToList(type, style, borders);
			return GetMergedBorder(borders);
		}
		protected internal void UpdateProperty(string propname) {
			PropertyChanged(this, propname, null, null);
		}
		internal string GetContentCellCssClassName() {
			return string.Format("{0} {0}{1}", GetCssClassNamePrefix(), ContentCellCssClassName);
		}
		internal string GetContentRowCssClassName() {
			return GetCssClassNamePrefix() + ContentRowCssClassName;
		}
		internal string GetHeaderImageCssClassName() {
			return GetCssClassNamePrefix() + (IsRightToLeft() ? HeaderImageRtlCssClassName : HeaderImageCssClassName);
		}
		internal string GetContentBottomRowCssClassName() {
			return GetCssClassNamePrefix() + ContentBottomRowCssClassName;
		}
		internal string GetCollapsedCssClassName() {
			return GetCssClassNamePrefix() + CollapsedCssClassName;
		}
		internal string GetCollapseButtonCssClassName() {
			return GetCssClassNamePrefix() + (IsRightToLeft() ? CollapseButtonRightToLeftCssClassName : CollapseButtonCssClassName);
		}
		internal string GetContentWrapperCssClassName() {
			return GetCssClassNamePrefix() + ContentWrapperCssClassName;
		}
		internal string GetAnimationWrapperCssClassName() {
			return GetCssClassNamePrefix() + AnimationWrapperCssClassName;
		}
		internal string GetHeaderSeparatorCssClassName() {
			return GetCssClassNamePrefix() + HeaderSeparatorCssClassName;
		}
		internal string GetHasNotCollapsingCssClassName() {
			return GetCssClassNamePrefix() + HasNotCollapsingCssClassName;
		}
		internal string GetHeaderContentWrapperCssClassName() {
			return GetCssClassNamePrefix() + HeaderContentWrapperCssClassName;
		}
		internal string GetHasDefaultImagesCssClassName() {
			return GetCssClassNamePrefix() + HasDefaultImagesCssClassName;
		}
		internal string GetHeaderTextCssClassName() {
			return GetCssClassNamePrefix() + HeaderTextCssClassName;
		}
		internal string GetCustomCornerImageCssClassName() {
			return GetCssClassNamePrefix() + CustomCornerImageClassName;
		}
		internal string GetCssClassName(PanelPartType type) {
			string result = GetCssClassNamePrefix();
			switch(type) {
				case PanelPartType.TopEdge:
					result += TopEdgeCssClassName;
					break;
				case PanelPartType.NoHeaderTopEdge:
					result += NoHeaderTopEdgeCssClassName;
					break;
				case PanelPartType.HeaderLeftEdge:
					result += HeaderLeftEdgeCssClassName;
					break;
				case PanelPartType.HeaderRightEdge:
					result += HeaderRightEdgeCssClassName;
					break;
				case PanelPartType.LeftEdge:
					result += LeftEdgeCssClassName;
					break;
				case PanelPartType.RightEdge:
					result += RightEdgeCssClassName;
					break;
				case PanelPartType.BottomEdge:
					result += BottomEdgeCssClassName;
					break;
				default:
					throw new ArgumentException(type.ToString());
			}
			return result;
		}
		protected internal DivButtonControl GetCollapseButtonControl() {
			if(roundPanelControl != null)
				return roundPanelControl.GetCollapseButtonControl();
			return null;
		}
		protected internal override bool IsCallBacksEnabled() {
			return LoadContentViaCallback && IsCollapsingAvailable();
		}
		protected bool IsChildCallback(Control rootControl) {
			return rootControl.Controls.OfType<ASPxWebControl>().Any(x => x.IsCallback)
				|| rootControl.Controls.OfType<Control>().Any(IsChildCallback);
		}
		protected internal virtual void UpdatePanelContentVisibility() {
			PanelContent.Visible = !IsCallBacksEnabled() || !Collapsed || IsCallback || IsChildCallback(PanelContent);
		}
		protected override object GetCallbackResult() {
			Control content = GetCallbackResultControl();
			if(content == null)
				return string.Empty;
			BeginRendering();
			try {
				return RenderUtils.GetControlChildrenRenderResult(content);
			} finally {
				EndRendering();
			}
		}
		protected virtual Control GetCallbackResultControl() {
			return FindControl(GetCallbackResultControlId());
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			RoundPanelContentCallbackEventArgs args = new RoundPanelContentCallbackEventArgs(eventArgument);
			OnCallback(args);
		}
		protected virtual void OnCallback(RoundPanelContentCallbackEventArgs e) {
			RoundPanelContentCallbackEventHandler handler = Events[CallbackEvent] as RoundPanelContentCallbackEventHandler;
			if(handler != null)
				handler(this, e);
		}
		#region obsolete properties
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelTopLeftCorner"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCornerPart TopLeftCorner {
			get { return Parts.TopLeftCornerInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelNoHeaderTopLeftCorner"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCornerPart NoHeaderTopLeftCorner {
			get { return Parts.NoHeaderTopLeftCornerInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelTopRightCorner"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCornerPart TopRightCorner {
			get { return Parts.TopRightCornerInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelNoHeaderTopRightCorner"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCornerPart NoHeaderTopRightCorner {
			get { return Parts.NoHeaderTopRightCornerInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelBottomRightCorner"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCornerPart BottomRightCorner {
			get { return Parts.BottomRightCornerInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelBottomLeftCorner"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCornerPart BottomLeftCorner {
			get { return Parts.BottomLeftCornerInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderLeftEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart HeaderLeftEdge {
			get { return Parts.HeaderLeftEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelHeaderRightEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart HeaderRightEdge {
			get { return Parts.HeaderRightEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelNoHeaderTopEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart NoHeaderTopEdge {
			get { return Parts.NoHeaderTopEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelTopEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart TopEdge {
			get { return Parts.TopEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelRightEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart RightEdge {
			get { return Parts.RightEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelBottomEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart BottomEdge {
			get { return Parts.BottomEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelLeftEdge"),
#endif
 Browsable(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelPart LeftEdge {
			get { return Parts.LeftEdgeInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelShowDefaultImages"),
#endif
 DefaultValue(false),
		Obsolete("Use the ASPxRoundPanel.CornerRadius property to specify the radius of round panel corners."), Browsable(false)]
		public bool ShowDefaultImages {
			get { return false; }
			set { }
		}
		#endregion
	}
}
namespace DevExpress.Web.Internal {
	public enum BorderType { Top, Bottom, Left, Right };
}
