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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public enum RenderMode { Div = 0, Table = 1 }
	public abstract class ASPxPanelContainerBase : ASPxWebControl, IEndInitAccessorContainer {
		private PanelCollection fPanelCollection = null;
		public ASPxPanelContainerBase()
			: base() {
		}
		protected ASPxPanelContainerBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxPanelContainerBaseControls")]
#endif
		public new ControlCollection Controls {
			get { return PanelContent.Controls; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelContainerBaseEnableHierarchyRecreation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableHierarchyRecreation {
			get { return EnableHierarchyRecreationInternal; }
			set { EnableHierarchyRecreationInternal = value; }
		}
		[Browsable(false), AutoFormatDisable,
		EditorBrowsableAttribute(EditorBrowsableState.Never),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelCollection PanelCollection {
			get {
				if(fPanelCollection == null)
					fPanelCollection = new PanelCollection(this);
				return fPanelCollection;
			}
		}
		protected internal PanelContent PanelContent {
			get {
				if(PanelCollection.Count == 0)
					PanelCollection.Add(new PanelContent());
				return PanelCollection[0];
			}
		}
		protected internal ControlCollection ControlsBase {
			get { return base.Controls; }
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return true;
		}
		protected override bool HasRenderCssFile() {
			return false;
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
	}
	public abstract class ASPxPanelBase : ASPxPanelContainerBase {
		protected internal const string PanelScriptResourceName = WebScriptsResourcePath + "Panel.js";
		private static readonly object EventDefaultButtonResolve = new object();
		public ASPxPanelBase()
			: base() {
		}
		protected ASPxPanelBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseDefaultButtonResolve"),
#endif
		Category("Events")]
		public event EventHandler<ControlResolveEventArgs> DefaultButtonResolve
		{
			add { Events.AddHandler(EventDefaultButtonResolve, value); }
			remove { Events.RemoveHandler(EventDefaultButtonResolve, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseDefaultButton"),
#endif
		Category("Behavior"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string DefaultButton {
			get { return GetStringProperty("DefaultButton", ""); }
			set { SetStringProperty("DefaultButton", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public PanelClientSideEvents ClientSideEvents {
			get { return (PanelClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelBaseJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		protected void OnDefaultButtonResolve(ControlResolveEventArgs e) {
			EventHandler<ControlResolveEventArgs> handler = (EventHandler<ControlResolveEventArgs>)Events[EventDefaultButtonResolve];
			if(handler != null)
				handler(this, e);
		}
		protected internal bool IsDefaultButtonAssigned() {
			return DefaultButton != "";
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PanelClientSideEvents();
		}
		protected virtual bool HasPanelFunctionalityScripts() {
			return base.HasFunctionalityScripts();
		}
		protected override sealed bool HasFunctionalityScripts() {
			return IsDefaultButtonAssigned() || HasPanelFunctionalityScripts();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxPanelBase), PanelScriptResourceName, HasPanelFunctionalityScripts());
		}
		protected internal void AddDefaultButtonScript(WebControl control) {
			if (DesignMode || (Page == null && !IsMvcRender()))
				return;
			RenderUtils.SetStringAttribute(control, "onkeypress",
				string.Format(ASPxWebControl.FireDefaultButtonHandlerName, RenderUtils.GetReferentControlClientID(PanelContent, DefaultButton, OnDefaultButtonResolve)));
		}
		protected override string GetStartupScript() {
			return HasPanelFunctionalityScripts() ? base.GetStartupScript() : string.Empty;
		}
	}
	public class PanelCollection : ContentControlCollection {
		public PanelCollection(Control owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("PanelCollectionItem")]
#endif
		public new PanelContent this[int i] {
			get {
				return (PanelContent)base[i];
			}
		}
		internal sealed override bool IsChildTypeValid(Control child) {
			return typeof(PanelContent).IsAssignableFrom(child.GetType());
		}
		protected override Type GetChildType() {
			return typeof(PanelContent);
		}
	}
	public class PanelContent : ContentControl {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string AccessKey {
			get { return base.AccessKey; }
			set { base.AccessKey = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color BorderColor {
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderStyle BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit BorderWidth {
			get { return base.BorderWidth; }
			set { base.BorderWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssClass {
			get { return base.CssClass; }
			set { base.CssClass = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableTheming {
			get { return base.EnableTheming; }
			set { base.EnableTheming = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState {
			get { return base.EnableViewState; }
			set { base.EnableViewState = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string SkinID {
			get { return base.SkinID; }
			set { base.SkinID = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override short TabIndex {
			get { return base.TabIndex; }
			set { base.TabIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		} 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ViewStateMode ViewStateMode {
			get { return base.ViewStateMode; }
			set { base.ViewStateMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return DesignMode;
		}
	}
	public abstract class ASPxCollapsiblePanel : ASPxPanelBase {
		internal const string ExpandBarID = "EB";
		internal const string ExpandBarTemplateID = "EBT";
		internal const string ExpandButtonID = "EBB";
		internal const string ExpandButtonImageIDPostFix = "I";
		internal const string ExpandButtonImageID = ExpandButtonID + ExpandButtonImageIDPostFix;
		internal const string ContentContainerID = "CC";
		internal const int FixedPositionOverlapZIndex = 1004;
		internal const int TopBottomFixedPositionOverlapZIndex = 1006;
		static readonly Dictionary<PanelExpandButtonGlyphType, string> ExpandGlyphImageNames = new Dictionary<PanelExpandButtonGlyphType, string>();
		CollapsiblePanelControl panelControl = null;
		PanelAdaptivitySettings settingsAdaptivity;
		PanelCollapsingSettings settingsCollapsing;
		ITemplate expandBarTemplate = null;
		ITemplate expandButtonTemplate = null;
		ITemplate expandedPanelTemplate = null;
		static ASPxCollapsiblePanel() { 
			ExpandGlyphImageNames.Add(PanelExpandButtonGlyphType.Strips, PanelImages.ExpandButtonImageName);
			ExpandGlyphImageNames.Add(PanelExpandButtonGlyphType.ArrowTop, PanelImages.ExpandButtonArrowTopImageName);
			ExpandGlyphImageNames.Add(PanelExpandButtonGlyphType.ArrowBottom, PanelImages.ExpandButtonArrowBottomImageName);
			ExpandGlyphImageNames.Add(PanelExpandButtonGlyphType.ArrowLeft, PanelImages.ExpandButtonArrowLeftImageName);
			ExpandGlyphImageNames.Add(PanelExpandButtonGlyphType.ArrowRight, PanelImages.ExpandButtonArrowRightImageName);
		}
		public ASPxCollapsiblePanel()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelCollapsible"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool Collapsible {
			get { return GetBoolProperty("Collapsible", false); }
			set {
				SetBoolProperty("Collapsible", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelFixedPosition"),
#endif
		Category("Layout"), DefaultValue(PanelFixedPosition.None), AutoFormatDisable]
		public PanelFixedPosition FixedPosition {
			get { return (PanelFixedPosition)GetEnumProperty("FixedPosition", PanelFixedPosition.None); }
			set {
				SetEnumProperty("FixedPosition", PanelFixedPosition.None, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelFixedPositionOverlap"),
#endif
		Category("Layout"), DefaultValue(false), AutoFormatDisable]
		public bool FixedPositionOverlap {
			get { return GetBoolProperty("FixedPositionOverlap", false); }
			set { SetBoolProperty("FixedPositionOverlap", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelRenderMode"),
#endif
		Category("Layout"), DefaultValue(RenderMode.Div), AutoFormatDisable]
		public RenderMode RenderMode {
			get { return (RenderMode)GetEnumProperty("RenderMode", RenderMode.Div); }
			set {
				SetEnumProperty("RenderMode", RenderMode.Div, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelScrollBars"),
#endif
		Category("Layout"), DefaultValue(ScrollBars.None), AutoFormatDisable]
		public ScrollBars ScrollBars {
			get { return (ScrollBars)GetEnumProperty("ScrollBars", ScrollBars.None); }
			set {
				SetEnumProperty("ScrollBars", ScrollBars.None, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelSettingsAdaptivity"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		Category("Settings"), AutoFormatDisable]
		public PanelAdaptivitySettings SettingsAdaptivity {
			get {
				if(settingsAdaptivity == null)
					settingsAdaptivity = new PanelAdaptivitySettings(this);
				return settingsAdaptivity;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		Category("Settings"), AutoFormatDisable]
		public PanelCollapsingSettings SettingsCollapsing {
			get {
				if(settingsCollapsing == null)
					settingsCollapsing = new PanelCollapsingSettings(this);
				return settingsCollapsing;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelImageFolder"),
#endif
		Category("Images"), DefaultValue(""), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty, Localizable(false),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Images.ImageFolder property instead.")]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Images.SpriteImageUrl property instead.")]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Images.SpriteCssFilePath property instead.")]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCollapsiblePanelPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatUrlProperty, UrlProperty]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ExpandBarTemplate {
			get { return expandBarTemplate; }
			set {
				expandBarTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ExpandButtonTemplate {
			get { return expandButtonTemplate; }
			set {
				expandButtonTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ExpandedPanelTemplate {
			get { return expandedPanelTemplate; }
			set {
				expandedPanelTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal PanelImages Images {
			get { return (PanelImages)ImagesInternal; }
		}
		protected internal PanelStyles Styles {
			get { return (PanelStyles)StylesInternal; }
		}
		internal static Dictionary<PanelFixedPosition, ASPxCollapsiblePanel> FixedPanels {
			get { return HttpUtils.GetContextObject<Dictionary<PanelFixedPosition, ASPxCollapsiblePanel>>("DXFixedPanels"); }
		}
		protected CollapsiblePanelControl PanelControl {
			get { return panelControl; }
		}
		protected internal bool HasCollapsingAdaptivity() {
			return Collapsible && (SettingsAdaptivity.CollapseAtWindowInnerHeight > 0 || SettingsAdaptivity.CollapseAtWindowInnerWidth > 0);
		}
		protected internal bool HasVisibilityAdaptivity() {
			return SettingsAdaptivity.HideAtWindowInnerHeight > 0 || SettingsAdaptivity.HideAtWindowInnerWidth > 0;
		}
		protected AnimationType GetAnimationType() {
			if(SettingsCollapsing.AnimationType == AnimationType.Auto) {
				if(GetExpandEffect() == PanelExpandEffect.Slide)
					return AnimationType.Slide;
				else 
					return AnimationType.Fade;
			}
			return SettingsCollapsing.AnimationType;
		}
		protected internal PanelExpandEffect GetExpandEffect() {
			if(SettingsCollapsing.ExpandEffect == PanelExpandEffect.Auto) {
				switch(FixedPosition) {
					case PanelFixedPosition.WindowTop:
						return PanelExpandEffect.PopupToBottom;
					case PanelFixedPosition.WindowBottom:
						return PanelExpandEffect.PopupToTop;
				}
				return PanelExpandEffect.Slide;
			}
			else if(SettingsCollapsing.ExpandEffect != PanelExpandEffect.Slide && FixedPosition != PanelFixedPosition.None) {
				switch(FixedPosition) {
					case PanelFixedPosition.WindowTop:
						return PanelExpandEffect.PopupToBottom;
					case PanelFixedPosition.WindowBottom:
						return PanelExpandEffect.PopupToTop;
					case PanelFixedPosition.WindowLeft:
						return PanelExpandEffect.PopupToRight;
					case PanelFixedPosition.WindowRight:
						return PanelExpandEffect.PopupToLeft;
				}
			}
			return SettingsCollapsing.ExpandEffect;
		}
		protected internal PanelExpandButtonPosition GetExpandButtonPosition() {
			if(SettingsCollapsing.ExpandButton.Position != PanelExpandButtonPosition.Auto)
				return SettingsCollapsing.ExpandButton.Position;
			if(FixedPosition == PanelFixedPosition.WindowTop || FixedPosition == PanelFixedPosition.WindowBottom)
				return PanelExpandButtonPosition.Far;
			if(FixedPosition == PanelFixedPosition.WindowLeft || FixedPosition == PanelFixedPosition.WindowRight)
				return PanelExpandButtonPosition.Near;
			return PanelExpandButtonPosition.Center;
		}
		protected internal bool IsExpandButtonVisible() {
			return SettingsCollapsing.ExpandButton.Visible;
		}
		protected PanelExpandButtonGlyphType GetExpandButtonGlyphType() {
			if(SettingsCollapsing.ExpandButton.GlyphType != PanelExpandButtonGlyphType.Auto)
				return SettingsCollapsing.ExpandButton.GlyphType;
			if(IsPositionFixed())
				return PanelExpandButtonGlyphType.Strips;
			else {
				switch(GetExpandEffect()) { 
					case PanelExpandEffect.PopupToTop:
						return PanelExpandButtonGlyphType.ArrowTop;
					case PanelExpandEffect.PopupToBottom:
						return PanelExpandButtonGlyphType.ArrowBottom;
					case PanelExpandEffect.PopupToLeft:
						return PanelExpandButtonGlyphType.ArrowLeft;
					case PanelExpandEffect.PopupToRight:
						return PanelExpandButtonGlyphType.ArrowRight;
				}
				return PanelExpandButtonGlyphType.ArrowBottom;
			}
		}
		protected internal bool NeedExpandOnPageLoad() {
			return SettingsCollapsing.ExpandOnPageLoad;
		}
		protected internal bool IsPositionFixed() {
			return FixedPosition != PanelFixedPosition.None;
		}
		protected internal int GetFixedPositionOverlapZIndex(bool forBar) {
			if(IsPositionFixed() && FixedPositionOverlap) {
				int zIndex = (FixedPosition == PanelFixedPosition.WindowTop || FixedPosition == PanelFixedPosition.WindowBottom) ?  
					TopBottomFixedPositionOverlapZIndex : FixedPositionOverlapZIndex;
				if(!forBar)
					zIndex++;
				return zIndex;
			}
			return 0;
		}
		protected override bool HasRenderCssFile() {
			return IsPositionFixed() || Collapsible;
		}
		protected override void BeforeRender() {
			if(IsPositionFixed()) {
				if(FixedPanels.ContainsKey(FixedPosition))
					throw new Exception("The page can contain only one panel with FixedPosition='" + FixedPosition.ToString() + "'.");
				FixedPanels.Add(FixedPosition, this);
			}
			base.BeforeRender();
		}
		protected override void ClearControlFields() {
			this.panelControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			if(!IsAutoFormatPreview) {
				this.panelControl = new CollapsiblePanelControl(this);
				ControlsBase.Add(PanelControl);
			}
			base.CreateControlHierarchy();
		}
		protected internal bool HasScrollableContainer() {
			return ScrollBars != ScrollBars.None && IsPositionFixed();
		}
		protected internal bool HasAnimationContainer() {
			return Collapsible && GetAnimationType() == AnimationType.Slide;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterPopupUtilsScripts(HasPanelFunctionalityScripts());
			RegisterAnimationScript(SettingsCollapsing.AnimationType != AnimationType.None);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPanel";
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(HasCollapsingAdaptivity()) {
				if(SettingsAdaptivity.CollapseAtWindowInnerWidth > 0)
					stb.AppendFormat("{0}.collapseWindowWidth = {1};\n", localVarName, SettingsAdaptivity.CollapseAtWindowInnerWidth);
				if(SettingsAdaptivity.CollapseAtWindowInnerHeight > 0)
					stb.AppendFormat("{0}.collapseWindowHeight = {1};\n", localVarName, SettingsAdaptivity.CollapseAtWindowInnerHeight);
			}
			if(HasVisibilityAdaptivity()) {
				if(SettingsAdaptivity.HideAtWindowInnerWidth > 0)
					stb.AppendFormat("{0}.hideWindowWidth = {1};\n", localVarName, SettingsAdaptivity.HideAtWindowInnerWidth);
				if(SettingsAdaptivity.HideAtWindowInnerHeight > 0)
					stb.AppendFormat("{0}.hideWindowHeight = {1};\n", localVarName, SettingsAdaptivity.HideAtWindowInnerHeight);
			}
			if(IsPositionFixed()) { 
				if(FixedPositionOverlap)
					stb.AppendFormat(localVarName + ".fixedPositionOverlap=true;\n");
			}
			if(Collapsible) {
				if(GetAnimationType() != AnimationType.None)
					stb.AppendFormat(localVarName + ".animationType='" + GetAnimationType().ToString().ToLower() + "';\n");
				if(GetExpandEffect() != PanelExpandEffect.Slide)
					stb.AppendFormat("{0}.expandEffect = {1};\n", localVarName, HtmlConvertor.ToScript(GetExpandEffect()));
				if(!string.IsNullOrEmpty(SettingsCollapsing.GroupName))
					stb.AppendFormat("{0}.groupName = {1};\n", localVarName, HtmlConvertor.ToScript(SettingsCollapsing.GroupName));
				if(NeedExpandOnPageLoad())
					stb.AppendFormat("{0}.expandOnPageLoad = true;\n", localVarName);
			}
		}
		protected override bool HasPanelFunctionalityScripts() {
			return base.HasPanelFunctionalityScripts() || RequiresScriptForScrolling() || HasVisibilityAdaptivity() || HasCollapsingAdaptivity() ||
				IsPositionFixed() || Collapsible;
		}
		protected bool RequiresScriptForScrolling() {
			return Browser.Platform.IsWebKitTouchUI && RenderMode == RenderMode.Div && ScrollBars != ScrollBars.None;
		}
		protected override bool HasHoverScripts() {
			return Collapsible && IsExpandButtonVisible() && !GetExpandButtonHoverStyle().IsEmpty;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetExpandButtonHoverStyle(), ExpandButtonID, true);
		}
		protected override bool HasPressedScripts() {
			return Collapsible && IsExpandButtonVisible() && !GetExpandButtonPressedStyle().IsEmpty;
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetExpandButtonPressedStyle(), ExpandButtonID, true);
		}
		protected override bool HasSelectedScripts() {
			return Collapsible;
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetExpandedPanelStyle(), "", true);
			helper.AddStyle(GetExpandBarExpandedStyle(), ExpandBarID, true);
			if(IsExpandButtonVisible() && !GetExpandButtonSelectedStyle().IsEmpty)
				helper.AddStyle(GetExpandButtonSelectedStyle(), ExpandButtonID, new string[0], GetExpandButtonSelectedImage().GetScriptObject(Page), ExpandButtonImageIDPostFix, true);
		}
		protected internal ImageProperties GetExpandButtonImage() {
			string imageName = ExpandGlyphImageNames[GetExpandButtonGlyphType()];
			return string.IsNullOrEmpty(imageName) ? ImageProperties.Empty : Images.GetImageProperties(Page, imageName);
		}
		protected ImageProperties GetExpandButtonSelectedImage() {
			switch(GetExpandButtonGlyphType()) {
				case PanelExpandButtonGlyphType.Strips:
					return Images.CollapseButton;
				case PanelExpandButtonGlyphType.ArrowBottom:
					return Images.ExpandButtonArrowTop;
				case PanelExpandButtonGlyphType.ArrowTop:
					return Images.ExpandButtonArrowBottom;
				case PanelExpandButtonGlyphType.ArrowRight:
					return Images.ExpandButtonArrowLeft;
				case PanelExpandButtonGlyphType.ArrowLeft:
					return Images.ExpandButtonArrowRight;
			}
			return ImageProperties.Empty;
		}
		protected override StylesBase CreateStyles() {
			return new PanelStyles(this);
		}
		protected internal AppearanceStyle GetPanelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetControlStyle());
			style.CopyFrom(Styles.Panel);
			return style;
		}
		protected internal Paddings GetPanelPaddings() {
			return GetPanelStyle().Paddings;
		}
		protected internal Unit GetPanelHeight() {
			Unit height = Height;
			if(RenderMode == RenderMode.Div)
				height = UnitUtils.GetCorrectedHeight(height, GetPanelStyle(), GetPanelPaddings());
			return height;
		}
		protected internal Unit GetPanelWidth() {
			Unit width = Width;
			if(RenderMode == RenderMode.Div)
				width = UnitUtils.GetCorrectedWidth(width, GetPanelStyle(), GetPanelPaddings());
			return width;
		}
		protected internal AppearanceStyle GetExpandedPanelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.Panel);
			style.CopyFrom(Styles.GetDefaultExpandedStyle());
			style.CopyFrom(Styles.ExpandedPanel);
			return style;
		}
		protected internal AppearanceStyle GetExpandBarStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetControlStyle());
			style.Width = Unit.Empty;
			style.Height = Unit.Empty;
			style.CopyFrom(Styles.ExpandBar);
			return style;
		}
		protected internal AppearanceStyle GetExpandBarExpandedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultExpandedStyle());
			style.CopyFrom(Styles.ExpandedExpandBar);
			return style;
		}
		protected internal AppearanceStyle GetExpandButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultExpandButtonStyle());
			style.CopyFrom(Styles.ExpandButton);
			return style;
		}
		protected internal AppearanceStyle GetExpandButtonHoverStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultExpandButtonHoverStyle());
			style.CopyFrom(Styles.ExpandButton.HoverStyle);
			return style;
		}
		protected internal AppearanceStyle GetExpandButtonPressedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultExpandButtonPressedStyle());
			style.CopyFrom(Styles.ExpandButton.PressedStyle);
			return style;
		}
		protected internal AppearanceStyle GetExpandButtonSelectedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultExpandButtonSelectedStyle());
			style.CopyFrom(Styles.ExpandButton.SelectedStyle);
			return style;
		}
		protected override ImagesBase CreateImages() {
			return new PanelImages(this);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { SettingsAdaptivity, SettingsCollapsing });
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxPanel"),
	ToolboxData("<{0}:ASPxPanel Width=\"200px\" runat=\"server\"></{0}:ASPxPanel>"),
	Designer("DevExpress.Web.Design.ASPxPanelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxPanel.bmp")
]
	public class ASPxPanel : ASPxCollapsiblePanel {
		public ASPxPanel()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PanelImages Images {
			get { return (PanelImages)ImagesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPanelStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PanelStyles Styles {
			get { return (PanelStyles)StylesInternal; }
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
	}
}
