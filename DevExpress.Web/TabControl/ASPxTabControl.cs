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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Design;
	public enum ActivateTabPageAction { Click, MouseOver }
	public enum TabAlign { Left, Center, Right, Justify }
	public enum TabPosition { Top, Bottom, Left, Right }
	public enum TabSpaceTemplatePosition { Before, After }
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxTabControl"),
	DefaultEvent("ActiveTabChanged"),
	Designer("DevExpress.Web.Design.ASPxTabControlDesignerBase, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public abstract class ASPxTabControlBase : ASPxHierarchicalDataWebControl, IRequiresLoadPostDataControl, IControlDesigner {
		protected internal const string TabControlScriptResourceName = WebScriptsResourcePath + "TabControl.js";
		protected internal const string TabClickHandlerName = "return ASPx.TCTClick(event, '{0}', {1})";
		protected internal const string ActiveTabIndexKey = "activeTabIndex";
		protected internal static string[] TabIdPostfixes = new string[] { "", "T" };
		protected internal static string[] ImageIdPostfixes = new string[] { "Img" };
		internal const string
			ScrollDirectionLeft = "L",
			ScrollDirectionRight = "R";
		private TabCollectionBase fTabItems = null;
		private ITemplate fActiveTabTemplate = null;
		private ITemplate fTabTemplate = null;
		private ITemplate activeTabTextTemplate = null;
		private ITemplate tabTextTemplate = null;
		private ITemplate spaceBeforeTabsTemplate = null;
		private ITemplate spaceAfterTabsTemplate = null;
		protected TCControlBase fMainControl = null;
		private static readonly object EventActiveTabChanged = new object();
		private static readonly object EventActiveTabChanging = new object();
		private static readonly object EventTabClick = new object();
		private static readonly object EventTabCommand = new object();
		public ASPxTabControlBase()
			: this(null) {
		}
		protected ASPxTabControlBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseActiveTabIndex"),
#endif
		Category("Behavior"), DefaultValue(-1), AutoFormatDisable]
		public int ActiveTabIndex {
			get {
				int index = InternalActiveTabIndex;
				return IsValidActiveTabIndex(index) ? index : -1;
			}
			set {
				if(IsLoading() || IsValidActiveTabIndex(value))
					InternalActiveTabIndex = value;
				LastSpecifiedActiveTabIndex = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseEnableTabScrolling"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableTabScrolling {
			get { return GetBoolProperty("EnableTabScrolling", false); }
			set {
				SetBoolProperty("EnableTabScrolling", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public TabControlClientSideEvents ClientSideEvents {
			get { return (TabControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseEnableHotTrack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool EnableHotTrack {
			get { return EnableHotTrackInternal; }
			set { EnableHotTrackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabAlign"),
#endif
		Category("Appearance"), DefaultValue(TabAlign.Left), AutoFormatEnable]
		public TabAlign TabAlign {
			get { return (TabAlign)GetEnumProperty("TabAlign", TabAlign.Left); }
			set {
				SetEnumProperty("TabAlign", TabAlign.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabPosition"),
#endif
		Category("Appearance"), DefaultValue(TabPosition.Top), AutoFormatDisable]
		public virtual TabPosition TabPosition {
			get { return (TabPosition)GetEnumProperty("TabPosition", TabPosition.Top); }
			set {
				SetEnumProperty("TabPosition", TabPosition.Top, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseSpriteImageUrl"),
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
	DevExpressWebLocalizedDescription("ASPxTabControlBaseSpriteCssFilePath"),
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
	DevExpressWebLocalizedDescription("ASPxTabControlBaseActiveTabImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties ActiveTabImage {
			get { return Images.ActiveTab; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties TabImage {
			get { return Images.Tab; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseScrollLeftButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ScrollLeftButtonImage {
			get { return Images.ScrollLeftButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseScrollRightButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ScrollRightButtonImage {
			get { return Images.ScrollRightButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((TabControlStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit TabSpacing {
			get { return ((TabControlStyle)ControlStyle).TabSpacing; }
			set {
				((TabControlStyle)ControlStyle).TabSpacing = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseScrollButtonSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ScrollButtonSpacing {
			get { return ((TabControlStyle)ControlStyle).ScrollButtonSpacing; }
			set {
				((TabControlStyle)ControlStyle).ScrollButtonSpacing = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseScrollButtonsIndent"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ScrollButtonsIndent {
			get { return ((TabControlStyle)ControlStyle).ScrollButtonsIndent; }
			set {
				((TabControlStyle)ControlStyle).ScrollButtonsIndent = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseRenderMode"),
#endif
 Category("Layout"),
		Obsolete("This property is now obsolete. The Lightweight render mode is used."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		AutoFormatDisable, DefaultValue(ControlRenderMode.Lightweight)]
		public ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseActiveTabStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle ActiveTabStyle {
			get { return Styles.ActiveTab; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle TabStyle {
			get { return Styles.Tab; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContentStyle ContentStyle {
			get { return Styles.Content; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseScrollButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonStyle ScrollButtonStyle {
			get { return Styles.ScrollButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseSpaceBeforeTabsTemplateStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpaceTabTemplateStyle SpaceBeforeTabsTemplateStyle {
			get { return Styles.SpaceBeforeTabsTemplate; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseSpaceAfterTabsTemplateStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpaceTabTemplateStyle SpaceAfterTabsTemplateStyle {
			get { return Styles.SpaceAfterTabsTemplate; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ActiveTabTemplate {
			get { return fActiveTabTemplate; }
			set {
				fActiveTabTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TabTemplate {
			get { return fTabTemplate; }
			set {
				fTabTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ActiveTabTextTemplate {
			get { return this.activeTabTextTemplate; }
			set {
				this.activeTabTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate TabTextTemplate {
			get { return this.tabTextTemplate; }
			set {
				this.tabTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate SpaceAfterTabsTemplate {
			get { return spaceAfterTabsTemplate; }
			set {
				spaceAfterTabsTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate SpaceBeforeTabsTemplate {
			get { return spaceBeforeTabsTemplate; }
			set {
				spaceBeforeTabsTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseActiveTabChanged"),
#endif
		Category("Behavior")]
		public event TabControlEventHandler ActiveTabChanged
		{
			add { Events.AddHandler(EventActiveTabChanged, value); }
			remove { Events.RemoveHandler(EventActiveTabChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseActiveTabChanging"),
#endif
		Category("Behavior")]
		public event TabControlCancelEventHandler ActiveTabChanging
		{
			add { Events.AddHandler(EventActiveTabChanging, value); }
			remove { Events.RemoveHandler(EventActiveTabChanging, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabClick"),
#endif
		Category("Behavior")]
		public event TabControlCancelEventHandler TabClick
		{
			add { Events.AddHandler(EventTabClick, value); }
			remove { Events.RemoveHandler(EventTabClick, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlBaseTabCommand"),
#endif
		Category("Action")]
		public event TabControlCommandEventHandler TabCommand
		{
			add
			{
				Events.AddHandler(EventTabCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventTabCommand, value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTabControlBaseBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		protected TabControlImages Images {
			get { return (TabControlImages)ImagesInternal; }
		}
		protected internal TabControlStyles Styles {
			get { return (TabControlStyles)StylesInternal; }
		}
		protected internal bool IsTopBottomTabPosition {
			get { return TabPosition == TabPosition.Top || TabPosition == TabPosition.Bottom; }
		}
		protected internal TabBase ActiveTabItem {
			get { return IsValidActiveTabIndex(ActiveTabIndex) ? TabItems[ActiveTabIndex] : null; }
			set {
				if(value != null && value.TabControl == this)
					ActiveTabIndex = value.Index;
			}
		}
		protected int InternalActiveTabIndex {
			get { return GetIntProperty("ActiveTabIndex", -1); }
			set { SetIntProperty("ActiveTabIndex", -1, value); }
		}
		protected int LastSpecifiedActiveTabIndex { get; set; }
		protected internal TCControlBase MainControl {
			get { return fMainControl; }
		}
		protected internal TabCollectionBase TabItems {
			get {
				if(fTabItems == null)
					fTabItems = CreateTabItemsCollection();
				return fTabItems;
			}
		}
		protected internal virtual bool ShowTabsInternal {
			get { return true; }
			set { }
		}
		public override Control FindControl(string id) {
			Control result = null;
			string[] templateIDs = { GetTabsSpaceTemplateID(TabSpaceTemplatePosition.Before), GetTabsSpaceTemplateID(TabSpaceTemplatePosition.After) };
			foreach(string templateID in templateIDs) {
				Control templateContainer = base.FindControl(templateID);
				if(templateContainer != null)
					result = templateContainer.FindControl(id);
				if(result != null) return result; ;
			}
			return base.FindControl(id);
		}
		protected internal TabPosition GetEffectiveTabPosition() {
			if(IsRightToLeft()) {
				if(TabPosition == TabPosition.Left)
					return TabPosition.Right;
				if(TabPosition == TabPosition.Right)
					return TabPosition.Left;
			}
			return TabPosition;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if(e is TabControlCommandEventArgs) {
				OnTabCommand(e as TabControlCommandEventArgs);
				return true;
			}
			return false;
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			ValidateActiveTabIndex();
		}
		protected virtual TabCollectionBase CreateTabItemsCollection() {
			return new TabCollectionBase(this);
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[EventTabClick] != null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TabControlClientSideEvents();
		}
		protected override bool UseClientSideVisibility() {
			return !DesignMode && !AutoPostBackInternal && IsEnabled();
		}
		protected virtual internal bool UseClientSideVisibility(TabBase tab) {
			return UseClientSideVisibility() && GetTabEnabled(tab);
		}
		public bool HasVisibleTabs() {
			return TabItems.GetVisibleTabItemCount() > 0;
		}
		protected internal bool CanTabHotTrack(TabBase tab, bool isActive) {
			return !GetTabHoverStyle(tab, isActive).IsEmpty;
		}
		protected internal bool HasTabCellIDs(TabBase tab) {
			return IsClientSideAPIEnabled();
		}
		protected internal bool HasTabCellImageIDs(TabBase tab, bool isActive) {
			return (CanTabHotTrack(tab, isActive) || IsClientSideAPIEnabled()) && GetTabEnabled(tab);
		}
		protected internal virtual bool IsLoadTabByCallbackInternal() {
			return IsCallBacksEnabled();
		}
		protected internal bool GetTabEnabled(TabBase tab) {
			return tab.Enabled && IsEnabled();
		}
		protected virtual internal ActivateTabPageAction GetActivateTabPageAction() {
			return ActivateTabPageAction.Click;
		}
		protected override bool HasPressedScripts() {
			return EnableTabScrolling;
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			AddScrollButtonsScriptStateItems(helper, GetScrollButtonPressedCssStyle(),
				GetScrollLeftImage().GetPressedScriptObject(Page), GetScrollRightImage().GetPressedScriptObject(Page));
		}
		protected override bool HasHoverScripts() {
			return EnableHotTrack || EnableTabScrolling;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			for(int i = 0; EnableHotTrack && i < TabItems.GetVisibleTabItemCount(); i++) {
				TabBase tab = TabItems.GetVisibleTabItem(i);
				if(!CanTabHotTrack(tab, true) && !CanTabHotTrack(tab, false)) continue;
				AddTabHoverItems(helper, tab, false);
				AddTabHoverItems(helper, tab, true);
			}
			if(EnableTabScrolling) {
				AddScrollButtonsScriptStateItems(helper, GetScrollButtonHoverCssStyle(),
					GetScrollLeftImage().GetHottrackedScriptObject(Page), GetScrollRightImage().GetHottrackedScriptObject(Page));
			}
		}
		protected void AddTabHoverItems(StateScriptRenderHelper helper, TabBase tab, bool isActive) {
			if(CanTabHotTrack(tab, isActive))
				helper.AddStyles(GetTabStyles(GetTabHoverCssStyle(tab, isActive)), GetTabElementID(tab, isActive), TabIdPostfixes,
					GetTabImageObjects(GetTabImage(tab, isActive).GetHottrackedScriptObject(Page)), ImageIdPostfixes, GetTabEnabled(tab));
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < TabItems.GetVisibleTabItemCount(); i++) {
				TabBase tab = TabItems.GetVisibleTabItem(i);
				AddTabDisabledItems(helper, tab, false);
				AddTabDisabledItems(helper, tab, true);
			}
			if(EnableTabScrolling) {
				AddScrollButtonsScriptStateItems(helper, GetScrollButtonDisabledCssStyle(),
					GetScrollLeftImage().GetDisabledScriptObject(Page), GetScrollRightImage().GetDisabledScriptObject(Page));
			}
		}
		protected void AddTabDisabledItems(StateScriptRenderHelper helper, TabBase tab, bool isActive) {
			helper.AddStyles(GetTabStyles(GetTabDisabledCssStyle(tab, isActive)), GetTabElementID(tab, isActive), TabIdPostfixes,
				GetTabImageObjects(GetTabImage(tab, isActive).GetDisabledScriptObject(Page)), ImageIdPostfixes, GetTabEnabled(tab));
		}
		protected void AddScrollButtonsScriptStateItems(StateScriptRenderHelper helper, AppearanceStyleBase style,
			object scrollLeftButtonImageObject, object scrollRightButtonImageObject) {
			helper.AddStyles(new AppearanceStyleBase[] { style }, GetScrollButtonID(ScrollDirectionLeft), new string[0],
				new object[] { scrollLeftButtonImageObject }, ImageIdPostfixes, true);
			helper.AddStyles(new AppearanceStyleBase[] { style }, GetScrollButtonID(ScrollDirectionRight), new string[0],
				new object[] { scrollRightButtonImageObject }, ImageIdPostfixes, true);
		}
		public override bool IsClientSideAPIEnabled() {
			for(int i = 0; i < TabItems.GetVisibleTabItemCount(); i++) {
				TabBase tab = TabItems.GetVisibleTabItem(i);
				if(!tab.ClientEnabled || !tab.ClientVisible) return true;
			}
			return base.IsClientSideAPIEnabled();
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasDisabledScripts() {
			return EnableTabScrolling || base.HasDisabledScripts();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxTabControlBase), TabControlScriptResourceName);
			RegisterScrollUtilsScripts();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(ActiveTabIndex != 0)
				stb.Append(localVarName + ".activeTabIndex=" + ActiveTabIndex.ToString() + ";\n");
			if(Height.IsEmpty)
				stb.Append(localVarName + ".emptyHeight = true;\n");
			if(Width.IsEmpty)
				stb.Append(localVarName + ".emptyWidth = true;\n");
			if(TabAlign != TabAlign.Left)
				stb.Append(localVarName + ".tabAlign='" + TabAlign.ToString() + "';\n");
			if(TabPosition != TabPosition.Top)
				stb.Append(localVarName + ".tabPosition='" + TabPosition.ToString() + "';\n");
			stb.Append(GetClientTabsScript(localVarName));
			if(!UseClientSideVisibility())
				stb.Append(localVarName + ".useClientVisibility=false;\n");
			if(IsLoadTabByCallbackInternal())
				stb.Append(localVarName + ".isLoadTabByCallback = true;");
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && Events[EventTabClick] != null)
				eventNames.Add("TabClick");
		}
		protected string GetClientTabsScript(string localVarName) {
			object[] tabsArray = new object[TabItems.Count];
			for(int i = 0; i < TabItems.Count; i++)
				tabsArray[i] = GetClientTabProperties(TabItems[i]);
			return localVarName + ".CreateTabs(" + HtmlConvertor.ToJSON(tabsArray, true) + ");\n";
		}
		protected virtual List<object> GetClientTabProperties(TabBase tab) {
			List<object> tabProperties = new List<object>();
			tabProperties.Add(tab.Name);
			tabProperties.Add(!tab.Enabled ? (object)false : null);
			tabProperties.Add(!tab.ClientEnabled ? (object)false : null);
			tabProperties.Add(!tab.Visible ? (object)false : null);
			tabProperties.Add(!tab.ClientVisible ? (object)false : null);
			return tabProperties;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTabControlBase";
		}
		protected override bool HasContent() {
			return HasVisibleTabs();
		}
		protected override void ClearControlFields() {
			fMainControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			CreateTabControlHierarchy();
			base.CreateControlHierarchy();
		}
		protected virtual void CreateTabControlHierarchy() {
			fMainControl = CreateTabControl();
			Controls.Add(fMainControl);
			Controls.Add(RenderUtils.CreateClearElement());
		}
		protected virtual TabControlLite CreateTabControl() {
			return DesignMode ? new TabControlLiteDesignMode(this) : new TabControlLite(this);
		}
		protected internal virtual TabControlTemplateContainerBase CreateTemplateContainer(TabBase tab, bool active) {
			return new TabControlTemplateContainerBase(tab, active);
		}
		protected internal virtual string GetTabText(TabBase tab) {
			return tab.Text;
		}
		protected internal string GetTabsCellID() {
			return "TC";
		}
		protected internal string GetTabElementID(TabBase tab, bool isActive) {
			return (isActive ? "AT" : "T") + tab.Index.ToString();
		}
		protected internal string GetTabCellID(TabBase tab, bool isActive) {
			return GetTabElementID(tab, isActive) + TabIdPostfixes[0];
		}
		protected internal string GetTabSeparatorCellID(TabBase tab) {
			return GetTabElementID(tab, false) + "S";
		}
		protected internal string GetTabTextCellID(TabBase tab, bool isActive) {
			return GetTabElementID(tab, isActive) + TabIdPostfixes[1];
		}
		protected internal string GetTabImageCellID(TabBase tab, bool isActive) {
			return GetTabElementID(tab, isActive) + "I";
		}
		protected internal string GetTabImageID(TabBase tab, bool isActive) {
			return GetTabElementID(tab, isActive) + ImageIdPostfixes[0];
		}
		protected internal string GetLeftAlignCellID() {
			return "LAC";
		}
		protected internal string GetRightAlignCellID() {
			return "RAC";
		}
		protected internal string GetContentsCellID() {
			return "CC";
		}
		protected internal string GetContentDivID(TabBase tab) {
			return "C" + tab.Index.ToString();
		}
		protected internal string GetContentControlID(TabBase tab) {
			return "CCTRL" + tab.Index.ToString();
		}
		protected internal string GetTabTemplateContainerID(TabBase tab, bool isActive) {
			return (isActive ? "ATTC" : "TTC") + tab.Index.ToString();
		}
		protected internal string GetTabTextTemplateContainerID(TabBase tab, bool isActive) {
			return (isActive ? "ATTTC" : "TTTC") + tab.Index.ToString();
		}
		protected internal string GetTabsSpaceTemplateID(TabSpaceTemplatePosition position) {
			return "TPTC" + (position == TabSpaceTemplatePosition.Before ? "L" : "R");
		}
		protected internal string GetScrollVisibleAreaID() {
			return "SVA";
		}
		protected internal string GetScrollButtonID(string scrollDirection) {
			return "SB" + scrollDirection;
		}
		protected internal string GetScrollButtonImageID(string scrollDirection) {
			return GetScrollButtonID(scrollDirection) + ImageIdPostfixes[0];
		}
		protected internal bool HasTabCellOnClick(TabBase tab, bool isActive) {
			return GetTabEnabled(tab) &&
				(!isActive || ClientSideEvents.TabClick != "" || HasEvents() && Events[EventTabClick] != null);
		}
		protected internal string GetTabCellOnClick(TabBase tab, bool isActive) {
			if(AutoPostBack && !IsClientSideEventsAssigned() && GetTabNavigateUrl(tab) == "")
				return RenderUtils.GetPostBackEventReference(Page, this, "CLICK:" + tab.Index.ToString());
			else
				return string.Format(TabClickHandlerName, ClientID, tab.Index);
		}
		protected internal virtual string GetTabNavigateUrl(TabBase tabBase) {
			return IsAccessibilityCompliantRender(true) ? RenderUtils.AccessibilityEmptyUrl : string.Empty;
		}
		protected internal virtual string GetTabTarget(TabBase tabBase) {
			return "";
		}
		protected internal virtual bool HasContentArea() {
			return true;
		}
		protected override Style CreateControlStyle() {
			return new TabControlStyle();
		}
		protected override StylesBase CreateStyles() {
			return new TabControlStyles(this);
		}
		protected DisabledStyle GetChildDisabledStyle() {
			return DisabledStyle;
		}
		protected internal Unit GetTabSize(TabBase tab, bool isActive, bool isPrimary) {
			TabStyle style = GetTabStyle(tab, isActive);
			Unit size, tabSize;
			if(IsTopBottomTabPosition && isPrimary || !IsTopBottomTabPosition && !isPrimary) {
				size = Width;
				tabSize = style.Width;
			}
			else {
				size = Height;
				tabSize = style.Height;
			}
			if(tabSize.IsEmpty)
				return Unit.Empty;
			if(isPrimary && TabAlign != TabAlign.Justify && !(size.IsEmpty || size.Type == UnitType.Percentage) && tabSize.Type == UnitType.Percentage) {
				double tabsSize = GetTabsSize();
				if(tabsSize > 0)
					return new Unit(tabsSize * tabSize.Value / 100, size.Type);
			}
			if(tabSize.Type == UnitType.Percentage)
				return Unit.Empty;
			return tabSize;
		}
		protected double GetTabsSize() {
			return IsTopBottomTabPosition ? GetTabsWidth() : GetTabsHeight();
		}
		protected double GetTabsWidth() {
			double tabsWidth = Width.Value;
			Unit tabSpacing = GetTabSpacing();
			if(tabSpacing.Type == Width.Type)
				tabsWidth -= tabSpacing.Value * (TabItems.GetVisibleTabItemCount() - 1);
			if(IsTopBottomTabPosition) {
				Paddings tabsPaddings = GetTabsPaddings();
				Unit leftPadding = tabsPaddings.GetPaddingLeft();
				Unit rightPadding = tabsPaddings.GetPaddingRight();
				if(leftPadding.Type == Width.Type)
					tabsWidth -= leftPadding.Value;
				if(rightPadding.Type == Width.Type)
					tabsWidth -= rightPadding.Value;
			}
			return tabsWidth;
		}
		protected double GetTabsHeight() {
			double tabsHeight = Height.Value;
			Unit tabSpacing = GetTabSpacing();
			if(tabSpacing.Type == Height.Type)
				tabsHeight -= tabSpacing.Value * (TabItems.GetVisibleTabItemCount() - 1);
			if(!IsTopBottomTabPosition) {
				Paddings tabsPaddings = GetTabsPaddings();
				Unit topPadding = tabsPaddings.GetPaddingTop();
				Unit bottomPadding = tabsPaddings.GetPaddingBottom();
				if(topPadding.Type == Height.Type)
					tabsHeight -= topPadding.Value;
				if(bottomPadding.Type == Height.Type)
					tabsHeight -= bottomPadding.Value;
			}
			return tabsHeight;
		}
		protected virtual internal Paddings GetTabsPaddings() {
			return GetControlStyle().Paddings;
		}
		protected virtual internal Unit GetTabSpacing() {
			return GetControlStyle().Spacing;
		}
		protected virtual internal Unit GetTabSpacing(int index) {
			return GetControlStyle().Spacing;
		}
		protected virtual internal Paddings GetTabContentPaddings(TabBase tab, bool isActive) {
			return GetTabStyle(tab, isActive).Paddings;
		}
		protected virtual internal Unit GetTabImageSpacing(TabBase tab, bool isActive) {
			return GetTabStyle(tab, isActive).ImageSpacing;
		}
		protected virtual internal Paddings GetContentPaddings() {
			return new Paddings();
		}
		protected internal AppearanceStyleBase GetTabHoverStyleInternal(TabBase tab, bool isActive) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(isActive ? Styles.GetDefaultActiveTabHoverStyle() : Styles.GetDefaultTabHoverStyle(TabPosition));
			style.CopyFrom(TabStyle.HoverStyle);
			if(tab != null)
				style.CopyFrom(tab.TabStyle.HoverStyle);
			if(isActive) {
				style.CopyFrom(ActiveTabStyle.HoverStyle);
				if(tab != null)
					style.CopyFrom(tab.ActiveTabStyle.HoverStyle);
			}
			return style;
		}
		protected internal AppearanceStyleBase GetTabHoverStyle(TabBase tab, bool isActive) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyBordersFrom(GetTabStyleInternal(tab, isActive));
			style.CopyFrom(GetTabHoverStyleInternal(tab, isActive));
			return style;
		}
		protected internal AppearanceStyle GetTabHoverCssStyle(TabBase tab, bool isActive) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetTabHoverStyle(tab, isActive));
			style.Paddings.CopyFrom(GetTabSelectedPaddings(tab, isActive, style));
			return style;
		}
		protected internal Paddings GetTabSelectedPaddings(TabBase tab, bool isActive, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetTabStyleInternal(tab, isActive);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, GetTabContentPaddings(tab, isActive));
		}
		protected AppearanceStyleBase[] GetTabStyles(AppearanceStyleBase baseStyle) {
			AppearanceStyleBase containerStyle = new AppearanceStyleBase();
			AppearanceStyleBase contentStyle = new AppearanceStyleBase();
			if(baseStyle != null) {
				containerStyle.CopyFrom(baseStyle);
				containerStyle.Paddings.Reset();
				contentStyle.Paddings.Assign(baseStyle.Paddings);
			}
			return new AppearanceStyleBase[] { containerStyle, contentStyle };
		}
		protected TabStyle GetCustomTabStyle(TabBase tab, bool isActive) {
			TabStyle style = new TabStyle();
			style.CopyFrom(TabStyle);
			if(tab != null)
				style.CopyFrom(tab.TabStyle);
			if(isActive) {
				style.CopyFrom(ActiveTabStyle);
				if(tab != null)
					style.CopyFrom(tab.ActiveTabStyle);
			}
			return style;
		}
		protected AppearanceStyleBase GetDefaultTabStyle(bool isActive) {
			return isActive ? Styles.GetDefaultActiveTabStyle(TabPosition) : Styles.GetDefaultTabStyle(TabPosition);
		}
		protected TabStyle GetTabStyleInternal(TabBase tab, bool isActive) {
			TabStyle style = new TabStyle();
			style.CopyFrom(GetDefaultTabStyle(isActive));
			style.CopyFrom(GetCustomTabStyle(tab, isActive));
			return style;
		}
		protected internal TabStyle GetTabStyle(TabBase tab, bool isActive) {
			TabStyle style = new TabStyle();
			style.CopyFrom(GetTabStyleInternal(tab, isActive));
			if(tab != null) {
				if(!isActive)
					CorrectTabStyleBorders(style);
				MergeDisableStyle(style, GetTabEnabled(tab), GetChildDisabledStyle());
			}
			return style;
		}
		protected virtual internal AppearanceStyleBase GetTabSeparatorStyle(int index) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			CorrectTabStyleBorders(style);
			return style;
		}
		protected internal AppearanceStyleBase GetTabSpacerStyle(TabBase tab) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			return style;
		}
		protected internal void CorrectTabStyleBorders(AppearanceStyleBase style) {
			AppearanceStyleBase s = GetContentStyle();
			switch(GetEffectiveTabPosition()) {
				case TabPosition.Top:
					style.BorderBottom.CopyFrom(new Border(s.GetBorderColorTop(), s.GetBorderStyleTop(), s.GetBorderWidthTop()));
					break;
				case TabPosition.Bottom:
					style.BorderTop.CopyFrom(new Border(s.GetBorderColorBottom(), s.GetBorderStyleBottom(), s.GetBorderWidthBottom()));
					break;
				case TabPosition.Left:
					style.BorderRight.CopyFrom(new Border(s.GetBorderColorLeft(), s.GetBorderStyleLeft(), s.GetBorderWidthLeft()));
					break;
				default:
					style.BorderLeft.CopyFrom(new Border(s.GetBorderColorRight(), s.GetBorderStyleRight(), s.GetBorderWidthRight()));
					break;
			}
		}
		protected internal Paddings GetTabStripPaddings() {
			Paddings paddings = new Paddings();
			switch(GetEffectiveTabPosition()) {
				case TabPosition.Left:
					paddings.PaddingLeft = GetTabsPaddings().GetPaddingLeft();
					break;
				case TabPosition.Top:
					paddings.PaddingTop = GetTabsPaddings().GetPaddingTop();
					break;
				case TabPosition.Right:
					paddings.PaddingRight = GetTabsPaddings().GetPaddingRight();
					break;
				case TabPosition.Bottom:
					paddings.PaddingBottom = GetTabsPaddings().GetPaddingBottom();
					break;
			}
			return paddings;
		}
		protected internal DisabledStyle GetTabDisabledCssStyle(TabBase tab, bool isActive) {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			return style;
		}
		protected internal virtual AppearanceStyleBase GetTabLinkStyle(TabBase tab, bool isActive) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFontAndCursorFrom(GetDefaultTabStyle(isActive));
			style.CopyFontAndCursorFrom(GetCustomTabStyle(tab, isActive));
			style.CopyFrom(LinkStyle.Style);
			if (tab != null)
				MergeDisableStyle(style, GetTabEnabled(tab));
			return style;
		}
		protected internal ContentStyle GetContentStyle() {
			ContentStyle style = new ContentStyle();
			style.CopyFrom(Styles.GetDefaultContentStyle());
			style.CopyFrom(ContentStyle);
			MergeDisableStyle(style, GetChildDisabledStyle());
			return style;
		}
		protected virtual string GetContentStyleNamePrefix() {
			return string.Empty;
		}
		protected internal AppearanceStyleBase GetScrollButtonCellStyle() {
			AppearanceStyleBase style = Styles.GetDefaultScrollButtonCellStyle();
			CorrectTabStyleBorders(style);
			return style;
		}
		protected internal AppearanceStyleBase GetScrollButtonSeparatorStyle() {
			AppearanceStyleBase style = Styles.GetDefaultScrollButtonSeparatorStyle();
			CorrectTabStyleBorders(style);
			return style;
		}
		protected internal AppearanceStyleBase GetScrollButtonIndentStyle() {
			AppearanceStyleBase style = Styles.GetDefaultScrollButtonIndentStyle();
			CorrectTabStyleBorders(style);
			return style;
		}
		protected internal ButtonStyle GetScrollButtonStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(Styles.GetDefaultScrollButtonStyle());
			style.CopyFrom(ScrollButtonStyle);
			MergeDisableStyle(style, Enabled, GetScrollButtonDisabledStyle());
			return style;
		}
		protected internal ButtonStyle GetScrollButtonHoverStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(Styles.GetDefaultScrollButtonHoverStyle());
			style.CopyFrom(GetScrollButtonStyle().HoverStyle);
			return style;
		}
		protected internal ButtonStyle GetScrollButtonPressedStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyBordersFrom(GetScrollButtonStyle());
			style.CopyFrom(Styles.GetDefaultScrollButtonPressedStyle());
			style.CopyFrom(GetScrollButtonStyle().PressedStyle);
			return style;
		}
		protected internal ButtonStyle GetScrollButtonDisabledStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(Styles.GetDefaultScrollButtonDisabledStyle());
			style.CopyFrom(ScrollButtonStyle.DisabledStyle);
			return style;
		}
		protected internal ButtonStyle GetScrollButtonHoverCssStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(GetScrollButtonHoverStyle());
			return style;
		}
		protected internal ButtonStyle GetScrollButtonPressedCssStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(GetScrollButtonPressedStyle());
			return style;
		}
		protected internal ButtonStyle GetScrollButtonDisabledCssStyle() {
			ButtonStyle style = new ButtonStyle();
			style.CopyFrom(GetScrollButtonDisabledStyle());
			return style;
		}
		protected override ImagesBase CreateImages() {
			return new TabControlImages(this);
		}
		protected internal TabImageProperties GetTabImage(TabBase tab, bool isActive) {
			TabImageProperties image = new TabImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, TabControlImages.TabImageName));
			if(tab != null)
				image.CopyFrom(tab.TabImage);
			if(isActive) {
				image.CopyFrom(Images.GetImageProperties(Page, TabControlImages.ActiveTabImageName));
				if(tab != null)
					image.CopyFrom(tab.ActiveTabImage);
			}
			return image;
		}
		protected internal ButtonImageProperties GetScrollLeftImage() {
			return GetScrollImageCore(ScrollLeftButtonImage, TabControlImages.ScrollLeftButtonImageName);
		}
		protected internal ButtonImageProperties GetScrollRightImage() {
			return GetScrollImageCore(ScrollRightButtonImage, TabControlImages.ScrollRightButtonImageName);
		}
		protected internal ButtonImageProperties GetScrollImageCore(ButtonImageProperties customProperties, string imageName) {
			ButtonImageProperties result = new ButtonImageProperties();
			result.CopyFrom(Images.GetImageProperties(Page, imageName));
			result.CopyFrom(customProperties);
			if(!Enabled) {
				if(!string.IsNullOrEmpty(result.UrlDisabled))
					result.Url = result.UrlDisabled;
				if(!string.IsNullOrEmpty(result.SpriteProperties.DisabledCssClass)) {
					result.SpriteProperties.CssClass = result.SpriteProperties.DisabledCssClass;
					result.SpriteProperties.Left = result.SpriteProperties.DisabledLeft;
					result.SpriteProperties.Top = result.SpriteProperties.DisabledTop;
				}
			}
			return result;
		}
		protected object[] GetTabImageObjects(object baseObject) {
			return new object[] { baseObject };
		}
		protected internal ITemplate GetTabTemplate(TabBase tab, bool isActive) {
			ITemplate ret = (tab.TabTemplate != null) ? tab.TabTemplate : TabTemplate;
			if(isActive) {
				if(tab.ActiveTabTemplate != null)
					ret = tab.ActiveTabTemplate;
				else if(ActiveTabTemplate != null)
					ret = ActiveTabTemplate;
			}
			return ret;
		}
		protected internal ITemplate GetTabTextTemplate(TabBase tab, bool isActive) {
			ITemplate ret = (tab.TabTextTemplate != null) ? tab.TabTextTemplate : TabTextTemplate;
			if(isActive) {
				if(tab.ActiveTabTextTemplate != null)
					ret = tab.ActiveTabTextTemplate;
				else if(ActiveTabTextTemplate != null)
					ret = ActiveTabTextTemplate;
			}
			return ret;
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { TabItems });
		}
		protected virtual void OnTabClick(ref TabControlCancelEventArgs e) {
			TabControlCancelEventHandler handler = (TabControlCancelEventHandler)Events[EventTabClick];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnActiveTabChanged(TabControlEventArgs e) {
			TabControlEventHandler handler = (TabControlEventHandler)Events[EventActiveTabChanged];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnActiveTabChanging(ref TabControlCancelEventArgs e) {
			TabControlCancelEventHandler handler = (TabControlCancelEventHandler)Events[EventActiveTabChanging];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnTabCommand(TabControlCommandEventArgs e) {
			TabControlCommandEventHandler handler = (TabControlCommandEventHandler)Events[EventTabCommand];
			if(handler != null)
				handler(this, e);
		}
		protected internal void TabsChanged() {
			if(!IsLoading()) {
				ValidateActiveTabIndex();
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected void ChangeActiveTabItem(int newActiveTabIndex) {
			TabBase newActiveTab = (0 <= newActiveTabIndex && newActiveTabIndex < TabItems.Count) ? TabItems[newActiveTabIndex] : null;
			TabControlCancelEventArgs args = new TabControlCancelEventArgs(newActiveTab);
			OnActiveTabChanging(ref args);
			if(!args.Cancel) {
				if(ActiveTabIndex != newActiveTabIndex) {
					ActiveTabIndex = newActiveTabIndex;
					OnActiveTabChanged(new TabControlEventArgs(ActiveTabItem));
				}
			}
		}
		protected bool IsValidActiveTabIndex(int index) {
			if((0 <= index) && (index < TabItems.Count))
				return TabItems[index].Enabled && TabItems[index].Visible;
			return false;
		}
		protected internal void ValidateActiveTabIndex() {
			if(!IsLoading()) {
				int oldActiveTabIndex = InternalActiveTabIndex;
				if(oldActiveTabIndex < -1)
					oldActiveTabIndex = -1;
				if(!IsValidActiveTabIndex(InternalActiveTabIndex)) {
					for(int i = oldActiveTabIndex + 1; i < TabItems.Count; i++) {
						if(IsValidActiveTabIndex(i)) {
							InternalActiveTabIndex = i;
							break;
						}
					}
				}
				if(!IsValidActiveTabIndex(InternalActiveTabIndex)) {
					for(int i = oldActiveTabIndex - 1; i >= 0; i--) {
						if(IsValidActiveTabIndex(i)) {
							InternalActiveTabIndex = i;
							break;
						}
					}
				}
			}
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			int newActiveTabIndex = GetClientObjectStateValue<int>(ActiveTabIndexKey);
			if(ActiveTabIndex != newActiveTabIndex) {
				ActiveTabIndex = newActiveTabIndex;
				return true;
			}
			return false;
		}
		protected override void RaisePostDataChangedEvent() {
			OnActiveTabChanged(new TabControlEventArgs(ActiveTabItem));
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch(arguments[0]) {
				case "ACTIVATE":
					int newActiveTabIndex = int.Parse(arguments[1]);
					ChangeActiveTabItem(newActiveTabIndex);
					break;
				case "CLICK":
					int clickedTabIndex = int.Parse(arguments[1]);
					TabBase tab = TabItems[clickedTabIndex];
					TabControlCancelEventArgs args = new TabControlCancelEventArgs(tab);
					OnTabClick(ref args);
					if(!args.Cancel && (clickedTabIndex != ActiveTabIndex))
						ChangeActiveTabItem(clickedTabIndex);
					break;
			}
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.TabControlTabsOwner"; } }
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free), DefaultProperty("Tabs"),
	ToolboxData("<{0}:ASPxTabControl runat=\"server\"></{0}:ASPxTabControl>"),
	Designer("DevExpress.Web.Design.ASPxTabControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTabControl.bmp")
]
	public class ASPxTabControl : ASPxTabControlBase {
		private bool fDataBound = false;
		private static readonly object EventTabDataBound = new object();
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Tab ActiveTab {
			get { return ActiveTabItem as Tab; }
			set { ActiveTabItem = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlTabs"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public TabCollection Tabs {
			get { return TabItems as TabCollection; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlTarget"),
#endif
		DefaultValue(""), Localizable(false), TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlSyncSelectionWithCurrentPath"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable,
		Obsolete("Use the SyncSelectionMode property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool SyncSelectionWithCurrentPath {
			get { return base.SyncSelectionWithCurrentPath; }
			set { base.SyncSelectionWithCurrentPath = value; }
		}
		protected bool ShouldSerializeSyncSelectionWithCurrentPath() { return false; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlSyncSelectionMode"),
#endif
		Category("Behavior"), DefaultValue(SyncSelectionMode.CurrentPathAndQuery), AutoFormatDisable]
		public new SyncSelectionMode SyncSelectionMode {
			get { return base.SyncSelectionMode; }
			set { base.SyncSelectionMode = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainer))]
		public override ITemplate ActiveTabTemplate {
			get { return base.ActiveTabTemplate; }
			set { base.ActiveTabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TabControlTemplateContainer))]
		public override ITemplate TabTemplate {
			get { return base.TabTemplate; }
			set { base.TabTemplate = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlActiveTabImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ActiveTabImageUrlField {
			get { return GetStringProperty("ActiveTabImageUrlField", ""); }
			set {
				SetStringProperty("ActiveTabImageUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlTabImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TabImageUrlField {
			get { return GetStringProperty("TabImageUrlField", ""); }
			set {
				SetStringProperty("TabImageUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlNameField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NameField {
			get { return GetStringProperty("NameField", ""); }
			set {
				SetStringProperty("NameField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlNavigateUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", ""); }
			set {
				SetStringProperty("NavigateUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set {
				SetStringProperty("TextField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlTextFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string TextFormatString {
			get { return GetStringProperty("TextFormatString", "{0}"); }
			set { SetStringProperty("TextFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlToolTipField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ToolTipField {
			get { return GetStringProperty("ToolTipField", ""); }
			set {
				SetStringProperty("ToolTipField", "", value);
				OnDataFieldChanged();
			}
		}
		public ASPxTabControl()
			: base() {
		}
		protected internal ASPxTabControl(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected override TabCollectionBase CreateTabItemsCollection() {
			return new TabCollection(this);
		}
		protected internal override void PerformDataBinding(string dataHelperName) {
			if(!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				TabItems.Clear();
			else if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindTabs();
				ResetControlHierarchy();
			}
		}
		protected internal void DataBindTabs() {
			TabDataFields dataFields = new TabDataFields();
			dataFields.activeTabImageUrlFieldName = String.IsNullOrEmpty(ActiveTabImageUrlField) ? "ActiveTabImageUrl" : ActiveTabImageUrlField;
			dataFields.tabImageUrlFieldName = String.IsNullOrEmpty(TabImageUrlField) ? "TabImageUrl" : TabImageUrlField;
			dataFields.navigateUrlFieldName = String.IsNullOrEmpty(NavigateUrlField) ? "NavigateUrl" : NavigateUrlField;
			dataFields.nameFieldName = String.IsNullOrEmpty(NameField) ? "Name" : NameField;
			dataFields.textFieldName = String.IsNullOrEmpty(TextField) ? "Text" : TextField;
			dataFields.toolTipFieldName = String.IsNullOrEmpty(ToolTipField) ? "ToolTip" : ToolTipField;
			HierarchicalDataSourceView view = GetData("");
			if(view != null) {
				IHierarchicalEnumerable enumerable = view.Select();
				TabItems.Clear();
				if(enumerable != null) {
					foreach(object obj in enumerable) {
						IHierarchyData data = enumerable.GetHierarchyData(obj);
						Tab tab = new Tab();
						TabItems.Add(tab);
						DataBindTabProperties(tab, obj, dataFields);
						tab.SetDataItem(obj);
						OnTabDataBound(new TabControlEventArgs(tab));
					}
				}
				InternalActiveTabIndex = LastSpecifiedActiveTabIndex;
				ValidateActiveTabIndex();
			}
		}
		protected virtual void DataBindTabProperties(Tab tab, object tabObject, TabDataFields dataFields) {
			if(tabObject is SiteMapNode) {
				SiteMapNode siteMapNode = (SiteMapNode)tabObject;
				tab.Text = siteMapNode.Title;
				tab.ToolTip = siteMapNode.Description;
				tab.NavigateUrl = siteMapNode.Url;
				if(siteMapNode[dataFields.activeTabImageUrlFieldName] != null)
					tab.ActiveTabImage.Url = siteMapNode[dataFields.activeTabImageUrlFieldName];
				if(siteMapNode[dataFields.tabImageUrlFieldName] != null)
					tab.TabImage.Url = siteMapNode[dataFields.tabImageUrlFieldName];
				if(siteMapNode[dataFields.nameFieldName] != null)
					tab.Name = siteMapNode[dataFields.nameFieldName];
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(tabObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<string>(tabObject, dataFields.activeTabImageUrlFieldName, props, value => { tab.ActiveTabImage.Url = value; });
				DataUtils.GetPropertyValue<string>(tabObject, dataFields.tabImageUrlFieldName, props, value => { tab.TabImage.Url = value; });
				DataUtils.GetPropertyValue<string>(tabObject, dataFields.navigateUrlFieldName, props, value => { tab.NavigateUrl = value; });
				DataUtils.GetPropertyValue<string>(tabObject, dataFields.nameFieldName, props, value => { tab.Name = value; });
				if(!DataUtils.GetPropertyValue<string>(tabObject, dataFields.textFieldName, props, value => { tab.Text = value; }))
					tab.Text = tabObject.ToString();
				DataUtils.GetPropertyValue<string>(tabObject, dataFields.toolTipFieldName, props, value => { tab.ToolTip = value; });
			}
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected override void SelectCurrentPath(bool ignoreQueryString) {
			for(int i = 0; i < TabItems.Count; i++) {
				if(UrlUtils.IsCurrentUrl(ResolveUrl(GetTabNavigateUrl(Tabs[i])), ignoreQueryString)) {
					ActiveTabIndex = Tabs[i].Index;
					break;
				}
			}
		}
		protected internal override TabControlTemplateContainerBase CreateTemplateContainer(TabBase tab, bool active) {
			return new TabControlTemplateContainer(tab as Tab, active);
		}
		protected internal override string GetTabText(TabBase tab) {
			return String.Format(TextFormatString, tab.Text);
		}
		protected internal override string GetTabNavigateUrl(TabBase tabBase) {
			string url = string.Format(NavigateUrlFormatString, (tabBase as Tab).NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected internal override string GetTabTarget(TabBase tabBase) {
			Tab tab = tabBase as Tab;
			return (tab.Target != "") ? tab.Target : Target;
		}
		protected internal override bool HasContentArea() {
			if(TabPosition == TabPosition.Left || TabPosition == TabPosition.Right)
				return !Width.IsEmpty && Width.Value > 0;
			else
				return !Height.IsEmpty && Height.Value > 0;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTabControl";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { LinkStyle });
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
			ValidateActiveTabIndex();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTabControlTabDataBound"),
#endif
		Category("Data")]
		public event TabControlEventHandler TabDataBound
		{
			add
			{
				Events.AddHandler(EventTabDataBound, value);
			}
			remove
			{
				Events.RemoveHandler(EventTabDataBound, value);
			}
		}
		protected virtual void OnTabDataBound(TabControlEventArgs e) {
			TabControlEventHandler handler = (TabControlEventHandler)Events[EventTabDataBound];
			if(handler != null)
				handler(this, e);
		}
	}
	public class TabDataFields {
		protected internal string activeTabImageUrlFieldName, tabImageUrlFieldName,
			navigateUrlFieldName, nameFieldName, textFieldName, toolTipFieldName;
	}
}
