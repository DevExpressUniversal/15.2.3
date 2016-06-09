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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Design;
	using DevExpress.Web.Internal;
	public enum ExpandGroupAction { Click, MouseOver }
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxNavBar"),
	DefaultProperty("Groups"), DefaultEvent("ItemClick"),
	ToolboxData("<{0}:ASPxNavBar runat=\"server\"></{0}:ASPxNavBar>"),
	Designer("DevExpress.Web.Design.ASPxNavBarDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxNavBar.bmp")
	]
	public class ASPxNavBar : ASPxHierarchicalDataWebControl, IRequiresLoadPostDataControl, IControlDesigner {
		protected internal const string NavBarScriptResourceName = WebScriptsResourcePath + "NavBar.js";
		protected internal const string GroupHeaderClickHandlerName = "ASPx.NBHClick(event, '{0}', {1})";
		protected internal const string GroupHeaderMouseMoveHandlerName = "ASPx.NBHMMove(event, '{0}', {1})";
		protected internal const string ItemClickHandlerName = "ASPx.NBIClick(event, '{0}', {1}, {2})";
		protected internal const string GroupsExpandingStateKey = "groupsExpanding";
		protected internal const string SelectedItemIndexPathKey = "selectedItemIndexPath";
		protected internal static string[] ItemIdPostfixes = new string[] { "I", "T" };
		protected internal static string[] ItemImageIdPostfixes = new string[] { "Img" };
		private NavBarGroupDataFields groupDataField = null;
		private NavBarItemDataFields itemDataField = null;
		private NavBarGroupCollection fGroups = null;
		private NavBarItems fItems = null;
		private NavBarItem fSelectedItem = null;
		private bool fLockAutoCollapse = false;
		private List<NavBarGroup> fExpandedChangedGroups = null;
		private int callbackGroupIndex = -1;
		private bool fDataBound = false;
		private ITemplate fGroupContentTemplate = null;
		private ITemplate fGroupHeaderTemplate = null;
		private ITemplate fGroupHeaderTemplateCollapsed = null;
		private ITemplate fItemTemplate = null;
		private ITemplate fItemTextTemplate = null;
		private NavBarControlBase fNavBarControl = null;
		private static readonly object EventItemClick = new object();
		private static readonly object EventExpandedChanged = new object();
		private static readonly object EventExpandedChanging = new object();
		private static readonly object EventHeaderClick = new object();
		private static readonly object EventGroupCommand = new object();
		private static readonly object EventItemCommand = new object();
		private static readonly object EventGroupDataBound = new object();
		private static readonly object EventItemDataBound = new object();
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavBarGroup ActiveGroup {
			get { return GetActiveGroup(); }
			set { SetActiveGroup(value); }
		}
		[Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		protected bool AllowDragging { 
			get { return GetBoolProperty("AllowDragging", false); }
			set { SetBoolProperty("AllowDragging", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarAllowExpanding"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowExpanding {
			get { return GetBoolProperty("AllowExpanding", true); }
			set { SetBoolProperty("AllowExpanding", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarAllowSelectItem"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AllowSelectItem {
			get { return GetBoolProperty("AllowSelectItem", false); }
			set { SetBoolProperty("AllowSelectItem", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarAutoCollapse"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoCollapse {
			get { return GetBoolProperty("AutoCollapse", false); }
			set {
				SetBoolProperty("AutoCollapse", false, value);
				ValidateAutoCollapse(null);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarExpandGroupAction"),
#endif
	   Category("Behavior"), DefaultValue(ExpandGroupAction.Click), AutoFormatDisable]
		public ExpandGroupAction ExpandGroupAction {
			get { return (ExpandGroupAction)GetEnumProperty("ExpandGroupAction", ExpandGroupAction.Click); }
			set { SetEnumProperty("ExpandGroupAction", ExpandGroupAction.Click, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public NavBarClientSideEvents ClientSideEvents {
			get { return (NavBarClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarEnableAnimation"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableAnimation {
			get { return GetBoolProperty("EnableAnimation", false); }
			set { SetBoolProperty("EnableAnimation", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarEnableCallBacks"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarEnableHotTrack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool EnableHotTrack {
			get { return EnableHotTrackInternal; }
			set { EnableHotTrackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarExpandButtonPosition"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(ExpandButtonPosition.Default)]
		public ExpandButtonPosition ExpandButtonPosition {
			get { return (ExpandButtonPosition)GetEnumProperty("ExpandButtonPosition ", ExpandButtonPosition.Default); }
			set {
				SetEnumProperty("ExpandButtonPosition ", ExpandButtonPosition.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroups"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public NavBarGroupCollection Groups {
			get {
				if(fGroups == null)
					fGroups = new NavBarGroupCollection(this);
				return fGroups;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemLinkMode"),
#endif
		Category("Behavior"), AutoFormatEnable, DefaultValue(ItemLinkMode.ContentBounds)]
		public ItemLinkMode ItemLinkMode {
			get { return (ItemLinkMode)GetEnumProperty("ItemLinkMode", ItemLinkMode.ContentBounds); }
			set {
				SetEnumProperty("ItemLinkMode", ItemLinkMode.ContentBounds, value);
				LayoutChanged();
			}
		}
		[Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavBarItems Items {
			get {
				if(fItems == null)
					fItems = new NavBarItems(this);
				return fItems;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavBarItem SelectedItem {
			get { return fSelectedItem; }
			set {
				if(value != null && value.NavBar != this)
					return;
				if(fSelectedItem != value) {
					if(fSelectedItem != null)
						fSelectedItem.SetSelected(false);
					fSelectedItem = value;
					if(fSelectedItem != null)
						fSelectedItem.SetSelected(true);
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarShowExpandButtons"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowExpandButtons {
			get { return GetBoolProperty("ShowExpandButtons", true); }
			set {
				SetBoolProperty("ShowExpandButtons", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarShowGroupHeaders"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowGroupHeaders {
			get { return GetBoolProperty("ShowGroupHeaders", true); }
			set {
				SetBoolProperty("ShowGroupHeaders", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarSyncSelectionWithCurrentPath"),
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
	DevExpressWebLocalizedDescription("ASPxNavBarSyncSelectionMode"),
#endif
		Category("Behavior"), DefaultValue(SyncSelectionMode.CurrentPathAndQuery), AutoFormatDisable]
		public new SyncSelectionMode SyncSelectionMode {
			get { return base.SyncSelectionMode; }
			set { base.SyncSelectionMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarTarget"),
#endif
		DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarSettingsLoadingPanel"),
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
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarSpriteImageUrl"),
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
	DevExpressWebLocalizedDescription("ASPxNavBarSpriteCssFilePath"),
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
	DevExpressWebLocalizedDescription("ASPxNavBarCollapseImage"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties CollapseImage {
			get { return Images.Collapse; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarExpandImage"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties ExpandImage {
			get { return Images.Expand; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupHeaderImage"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties GroupHeaderImage {
			get { return Images.GroupHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupHeaderImageCollapsed"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties GroupHeaderImageCollapsed {
			get { return Images.GroupHeaderCollapsed; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemImage"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ItemImageProperties ItemImage {
			get { return Images.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit GroupSpacing {
			get { return ((NavBarStyle)ControlStyle).GroupSpacing; }
			set { ((NavBarStyle)ControlStyle).GroupSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((NavBarStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarRenderMode"),
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
	DevExpressWebLocalizedDescription("ASPxNavBarRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public NavBarGroupHeaderStyle GroupHeaderStyle {
			get { return Styles.GroupHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupHeaderStyleCollapsed"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public NavBarGroupHeaderStyle GroupHeaderStyleCollapsed {
			get { return Styles.GroupHeaderCollapsed; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public NavBarGroupContentStyle GroupContentStyle {
			get { return Styles.GroupContent; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public NavBarItemStyle ItemStyle {
			get { return Styles.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarGroupTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupContentTemplate {
			get { return fGroupContentTemplate; }
			set {
				fGroupContentTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarGroupTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupHeaderTemplate {
			get { return fGroupHeaderTemplate; }
			set {
				fGroupHeaderTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarGroupTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupHeaderTemplateCollapsed {
			get { return fGroupHeaderTemplateCollapsed; }
			set {
				fGroupHeaderTemplateCollapsed = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTemplate {
			get { return fItemTemplate; }
			set {
				fItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NavBarItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTextTemplate {
			get { return fItemTextTemplate; }
			set {
				fItemTextTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupDataFields"),
#endif
		Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavBarGroupDataFields GroupDataFields {
			get {
				if(groupDataField == null)
					groupDataField = new NavBarGroupDataFields(this);
				return groupDataField;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemDataFields"),
#endif
		Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavBarItemDataFields ItemDataFields {
			get {
				if(itemDataField == null)
					itemDataField = new NavBarItemDataFields(this);
				return itemDataField;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemClick"),
#endif
		Category("Action")]
		public event NavBarItemEventHandler ItemClick
		{
			add
			{
				Events.AddHandler(EventItemClick, value);
			}
			remove
			{
				Events.RemoveHandler(EventItemClick, value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxNavBarClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarExpandedChanged"),
#endif
		Category("Behavior")]
		public event NavBarGroupEventHandler ExpandedChanged
		{
			add
			{
				Events.AddHandler(EventExpandedChanged, value);
			}
			remove
			{
				Events.RemoveHandler(EventExpandedChanged, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarExpandedChanging"),
#endif
		Category("Behavior")]
		public event NavBarGroupCancelEventHandler ExpandedChanging
		{
			add
			{
				Events.AddHandler(EventExpandedChanging, value);
			}
			remove
			{
				Events.RemoveHandler(EventExpandedChanging, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarHeaderClick"),
#endif
		Category("Action")]
		public event NavBarGroupCancelEventHandler HeaderClick
		{
			add
			{
				Events.AddHandler(EventHeaderClick, value);
			}
			remove
			{
				Events.RemoveHandler(EventHeaderClick, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupCommand"),
#endif
		Category("Action")]
		public event NavBarGroupCommandEventHandler GroupCommand
		{
			add
			{
				Events.AddHandler(EventGroupCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventGroupCommand, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemCommand"),
#endif
		Category("Action")]
		public event NavBarItemCommandEventHandler ItemCommand
		{
			add
			{
				Events.AddHandler(EventItemCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventItemCommand, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarItemDataBound"),
#endif
		Category("Data")]
		public event NavBarItemEventHandler ItemDataBound
		{
			add
			{
				Events.AddHandler(EventItemDataBound, value);
			}
			remove
			{
				Events.RemoveHandler(EventItemDataBound, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxNavBarGroupDataBound"),
#endif
		Category("Data")]
		public event NavBarGroupEventHandler GroupDataBound
		{
			add
			{
				Events.AddHandler(EventGroupDataBound, value);
			}
			remove
			{
				Events.RemoveHandler(EventGroupDataBound, value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxNavBarBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		protected int CallbackGroupIndex {
			get { return callbackGroupIndex; }
		}
		protected NavBarImages Images {
			get { return (NavBarImages)ImagesInternal; }
		}
		protected internal NavBarStyles Styles {
			get { return (NavBarStyles)StylesInternal; }
		}
		public ASPxNavBar()
			: base() {
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if(e is NavBarGroupCommandEventArgs) {
				OnGroupCommand(e as NavBarGroupCommandEventArgs);
				return true;
			}
			if(e is NavBarItemCommandEventArgs) {
				OnItemCommand(e as NavBarItemCommandEventArgs);
				return true;
			}
			return false;
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			ValidateAutoCollapse(null);
			ValidateSelectedItem();
		}
		protected internal override void PerformDataBinding(string dataHelperName) {
			if(!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				Groups.Clear();
			else if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindGroups();
				ResetControlHierarchy();
			}
		}
		public void OnDataFieldChangedInternal() {
			OnDataFieldChanged();
		}
		private void DataBindGroups() {
			GroupDataFields.SpecifyDataFields();
			HierarchicalDataSourceView view = GetData("");
			if(view != null) {
				IHierarchicalEnumerable enumerable = view.Select();
				Groups.Clear();
				if(enumerable != null) {
					foreach(object obj in enumerable) {
						IHierarchyData data = enumerable.GetHierarchyData(obj);
						NavBarGroup group = new NavBarGroup();
						Groups.Add(group);
						DataBindGroupProperties(group, obj);
						if(data.HasChildren)
							DataBindGroupItems(group, data.GetChildren());
						group.SetDataPath(data.Path);
						group.SetDataItem(obj);
						OnGroupDataBound(new NavBarGroupEventArgs(group));
					}
				}
			}
		}
		private void DataBindGroupProperties(NavBarGroup group, object groupObject) {
			if(groupObject is SiteMapNode) {
				SiteMapNode siteMapNode = (SiteMapNode)groupObject;
				group.NavigateUrl = siteMapNode.Url;
				group.Text = siteMapNode.Title;
				group.ToolTip = siteMapNode.Description;
				if(siteMapNode["Enabled"] != null)
					group.Enabled = bool.Parse(siteMapNode["Enabled"]);
				if(siteMapNode["Expanded"] != null)
					group.Expanded = bool.Parse(siteMapNode["Expanded"]);
				if(siteMapNode[GroupDataFields.headerImageUrlFieldName] != null)
					group.HeaderImage.Url = siteMapNode[GroupDataFields.headerImageUrlFieldName];
				if(siteMapNode[GroupDataFields.nameFieldName] != null)
					group.Name = siteMapNode[GroupDataFields.nameFieldName];
				if(siteMapNode[GroupDataFields.navigateUrlFieldName] != null)
					group.NavigateUrl = siteMapNode[GroupDataFields.navigateUrlFieldName];
				if(siteMapNode["Target"] != null)
					group.Target = siteMapNode["Target"];
				if(siteMapNode["VisibleIndex"] != null)
					group.VisibleIndex = int.Parse(siteMapNode["VisibleIndex"]);
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(groupObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<bool>(groupObject, "Enabled", props, value => { group.Enabled = value; });
				DataUtils.GetPropertyValue<bool>(groupObject, "Expanded", props, value => { group.Expanded = value; });
				DataUtils.GetPropertyValue<string>(groupObject, GroupDataFields.headerImageUrlFieldName, props, value => { group.HeaderImage.Url = value; });
				DataUtils.GetPropertyValue<string>(groupObject, GroupDataFields.nameFieldName, props, value => { group.Name = value; });
				DataUtils.GetPropertyValue<string>(groupObject, GroupDataFields.navigateUrlFieldName, props, value => { group.NavigateUrl = value; });
				DataUtils.GetPropertyValue<string>(groupObject, "Target", props, value => { group.Target = value; });
				if(!DataUtils.GetPropertyValue<string>(groupObject, GroupDataFields.textFieldName, props, value => { group.Text = value; }))
					group.Text = groupObject.ToString();
				DataUtils.GetPropertyValue<string>(groupObject, GroupDataFields.toolTipFieldName, props, value => { group.ToolTip = value; });
				DataUtils.GetPropertyValue<int>(groupObject, "VisibleIndex", props, value => { group.VisibleIndex = value; });
			}
		}
		private void DataBindGroupItems(NavBarGroup group, IHierarchicalEnumerable enumerable) {
			ItemDataFields.SpecifyDataFields();
			foreach(object obj in enumerable) {
				IHierarchyData data = enumerable.GetHierarchyData(obj);
				NavBarItem item = new NavBarItem();
				group.Items.Add(item);
				DataBindItemProperties(item, obj);
				item.SetDataPath(data.Path);
				item.SetDataItem(obj);
				OnItemDataBound(new NavBarItemEventArgs(item));
			}
		}
		private void DataBindItemProperties(NavBarItem item, object itemObject) {
			if(itemObject is SiteMapNode) {
				SiteMapNode siteMapNode = itemObject as SiteMapNode;
				item.NavigateUrl = siteMapNode.Url;
				item.Text = siteMapNode.Title;
				item.ToolTip = siteMapNode.Description;
				if(siteMapNode["Enabled"] != null)
					item.Enabled = bool.Parse(siteMapNode["Enabled"]);
				if(siteMapNode[ItemDataFields.imageUrlFieldName] != null)
					item.Image.Url = siteMapNode[ItemDataFields.imageUrlFieldName];
				if(siteMapNode[ItemDataFields.nameFieldName] != null)
					item.Name = siteMapNode[ItemDataFields.nameFieldName];
				if(siteMapNode[ItemDataFields.navigateUrlFieldName] != null)
					item.NavigateUrl = siteMapNode[ItemDataFields.navigateUrlFieldName];
				if(siteMapNode["Selected"] != null)
					item.Selected = bool.Parse(siteMapNode["Selected"]);
				if(siteMapNode["Target"] != null)
					item.Target = siteMapNode["Target"];
				if(siteMapNode["VisibleIndex"] != null)
					item.VisibleIndex = int.Parse(siteMapNode["VisibleIndex"]);
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(itemObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<bool>(itemObject, "Enabled", props, value => { item.Enabled = value; });
				DataUtils.GetPropertyValue<bool>(itemObject, "Selected", props, value => { item.Selected = value; });
				DataUtils.GetPropertyValue<string>(itemObject, ItemDataFields.imageUrlFieldName, props, value => { item.Image.Url = value; });
				DataUtils.GetPropertyValue<string>(itemObject, ItemDataFields.nameFieldName, props, value => { item.Name = value; });
				DataUtils.GetPropertyValue<string>(itemObject, ItemDataFields.navigateUrlFieldName, props, value => { item.NavigateUrl = value; });
				DataUtils.GetPropertyValue<string>(itemObject, "Target", props, value => { item.Target = value; });
				if(!DataUtils.GetPropertyValue<string>(itemObject, ItemDataFields.textFieldName, props, value => { item.Text = value; }))
					item.Text = itemObject.ToString();
				DataUtils.GetPropertyValue<string>(itemObject, ItemDataFields.toolTipFieldName, props, value => { item.ToolTip = value; });
				DataUtils.GetPropertyValue<int>(itemObject, "VisibleIndex", props, value => { item.VisibleIndex = value; });
			}
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Groups });
		}
		protected override ImagesBase CreateImages() {
			return new NavBarImages(this);
		}
		ImagePropertiesCache<NavBarGroup, ImageProperties> collapseImageProperties;
		protected internal ImageProperties GetCollapseImageProperties(NavBarGroup group_) {
			if(collapseImageProperties == null)
				collapseImageProperties = new ImagePropertiesCache<NavBarGroup, ImageProperties>(delegate(CacheKey<NavBarGroup> key) {
					NavBarGroup group = key.Item;
					ImageProperties ret = new ImageProperties();
					ret.CopyFrom(Images.GetImageProperties(Page, NavBarImages.CollapseImageName));
					ret.CopyFrom(group.CollapseImage);
					return ret;
				});
			return GetItemImage(collapseImageProperties, group_);
		}
		ImagePropertiesCache<NavBarGroup, ImageProperties> expandImageProperties;
		protected internal ImageProperties GetExpandImageProperties(NavBarGroup group_) {
			if(expandImageProperties == null)
				expandImageProperties = new ImagePropertiesCache<NavBarGroup, ImageProperties>(delegate(CacheKey<NavBarGroup> key) {
					NavBarGroup group = key.Item;
					ImageProperties ret = new ImageProperties();
					ret.CopyFrom(Images.GetImageProperties(Page, NavBarImages.ExpandImageName));
					ret.CopyFrom(group.ExpandImage);
					return ret;
				});
			return GetItemImage(expandImageProperties, group_);
		}
		protected internal ExpandButtonPosition GetExpandButtonPosition(NavBarGroup group) {
			if(group.ExpandButtonPosition != ExpandButtonPosition.Default)
				return group.ExpandButtonPosition;
			else
				if(ExpandButtonPosition != ExpandButtonPosition.Default)
					return ExpandButtonPosition;
				else
					return ExpandButtonPosition.Right;
		}
		protected internal ImageProperties GetHeaderButtonImageProperties(NavBarGroup group, bool expanded) {
			return expanded ? GetCollapseImageProperties(group) : GetExpandImageProperties(group);
		}
		ImagePropertiesCache<NavBarGroup, ImageProperties> groupHeaderImageProperties;
		protected internal ImageProperties GetGroupHeaderImageProperties(NavBarGroup group_, bool expanded) {
			if(groupHeaderImageProperties == null)
				groupHeaderImageProperties = new ImagePropertiesCache<NavBarGroup, ImageProperties>(delegate(CacheKey<NavBarGroup> key) {
					NavBarGroup group = key.Item;
					ImageProperties ret = new ImageProperties();
					ret.CopyFrom(Images.GetImageProperties(Page, NavBarImages.GroupHeaderImageName));
					ret.CopyFrom(group.HeaderImage);
					if(key.ExtraOption != 0) {
						ret.CopyFrom(Images.GetImageProperties(Page, NavBarImages.GroupHeaderImageCollapsedName));
						ret.CopyFrom(group.HeaderImageCollapsed);
					}
					return ret;
				});
			return GetItemImage(groupHeaderImageProperties, group_, expanded ? 1 : 0);
		}
		ImagePropertiesCache<NavBarItem, ItemImageProperties> itemImageProperties;
		protected ItemImageProperties GetItemImagePropertiesInternal(NavBarItem item_) {
			if(itemImageProperties == null)
				itemImageProperties = new ImagePropertiesCache<NavBarItem, ItemImageProperties>(delegate(CacheKey<NavBarItem> key) {
					NavBarItem item = key.Item;
					ItemImageProperties ret = new ItemImageProperties();
					ret.CopyFrom(Images.GetImageProperties(Page, NavBarImages.ItemImageName));
					ret.CopyFrom(item.Group.ItemImage);
					ret.CopyFrom(item.Image);
					return ret;
				});
			return GetItemImage(itemImageProperties, item_);
		}
		protected internal ItemImageProperties GetItemImageProperties(NavBarItem item) {
			ItemImageProperties ret = GetItemImagePropertiesInternal(item);
			if(IsCurrentItem(item)) {
				if(!string.IsNullOrEmpty(ret.UrlSelected))
					ret.Url = ret.UrlSelected;
				if(!ret.SpriteProperties.SelectedLeft.IsEmpty)
					ret.SpriteProperties.Left = ret.SpriteProperties.SelectedLeft;
				if(!ret.SpriteProperties.SelectedTop.IsEmpty)
					ret.SpriteProperties.Top = ret.SpriteProperties.SelectedTop;
			}
			return ret;
		}
		protected override bool HasHoverScripts() {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				if(CanGroupHeaderHotTrack(group, true) || CanGroupHeaderHotTrack(group, false) ||
					CanItemHotTrack(group))
					return true;
			}
			return false;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				AddGroupHeaderHoverItems(helper, group, true);
				AddGroupHeaderHoverItems(helper, group, false);
				if(group.HasVisibleItems() && CanItemHotTrack(group)) {
					AppearanceStyleBase style = GetItemHoverCssStyle(group);
					if(IsBulletMode(group))
						helper.AddStyle(style, GetItemElementIDPrefix(group), GetGroupEnabled(group));
					else {
						for(int j = 0; j < group.Items.GetVisibleItemCount(); j++) {
							NavBarItem item = group.Items.GetVisibleItem(j);
							if(CanItemHotTrack(item)) {
								helper.AddStyles(GetItemStyles(item, style), GetItemElementID(item), GetItemIdPostfixes(item),
									GetItemImageObjects(GetItemImageProperties(item).GetHottrackedScriptObject(Page)), ItemImageIdPostfixes, GetItemEnabled(item));
							}
						}
					}
				}
			}
		}
		protected void AddGroupHeaderHoverItems(StateScriptRenderHelper helper, NavBarGroup group, bool expanded) {
			if(CanGroupHeaderHotTrack(group, expanded))
				helper.AddStyle(GetGroupHeaderHoverCssStyle(group, expanded), GetGroupHeaderCellID(group, expanded), GetGroupEnabled(group));
		}
		protected override bool HasSelectedScripts() {
			return IsItemSelectEnabled();
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				if(group.HasVisibleItems() && CanItemSelect(group)) {
					AppearanceStyleBase style = GetItemSelectedCssStyle(group);
					if(IsBulletMode(group))
						helper.AddStyle(style, GetItemElementIDPrefix(group), GetGroupEnabled(group));
					else {
						for(int j = 0; j < group.Items.GetVisibleItemCount(); j++) {
							NavBarItem item = group.Items.GetVisibleItem(j);
							if(CanItemSelect(item)) {
								helper.AddStyles(GetItemStyles(item, style), GetItemElementID(item), GetItemIdPostfixes(item),
									GetItemImageObjects(GetItemImageProperties(item).GetSelectedScriptObject(Page)), ItemImageIdPostfixes, GetItemEnabled(item));
							}
						}
					}
				}
			}
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				if(group.HasVisibleItems()) {
					AppearanceStyleBase style = GetItemDisabledCssStyle(group);
					for(int j = 0; j < group.Items.GetVisibleItemCount(); j++) {
						NavBarItem item = group.Items.GetVisibleItem(j);
						if(IsBulletMode(group))
							helper.AddStyle(style, GetItemElementID(item), GetItemEnabled(item));
						else {
							helper.AddStyles(GetItemStyles(item, style), GetItemElementID(item), GetItemIdPostfixes(item),
								GetItemImageObjects(GetItemImageProperties(item).GetDisabledScriptObject(Page)), ItemImageIdPostfixes, GetItemEnabled(item));
						}
					}
				}
			}
		}
		protected override bool IsAnimationScriptNeeded() {
			return EnableAnimation || EnableCallbackAnimation;
		}
		public override bool IsClientSideAPIEnabled() {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				if(!group.ClientVisible) return true;
				for(int j = 0; j < group.Items.GetVisibleItemCount(); j++) {
					NavBarItem item = group.Items.GetVisibleItem(j);
					if(!item.ClientEnabled || !item.ClientVisible)
						return true;
				}
			}
			return base.IsClientSideAPIEnabled();
		}
		protected override bool HasFunctionalityScripts() {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				if((GetItemLinkMode(group) == ItemLinkMode.ContentBounds) ||
					ClientSideEvents.ItemClick != "" ||
					(ClientSideEvents.ExpandedChanging != "" && CanGroupExpanding(group, false)) ||
					(!AutoPostBack &&
					 (CanGroupDragging(group) || CanGroupExpanding(group, false) || CanItemSelect(group))))
					return true;
			}
			return base.HasFunctionalityScripts();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxNavBar), NavBarScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(AutoCollapse)
				stb.Append(localVarName + ".autoCollapse=true;\n");
			if(!AllowExpanding)
				stb.Append(localVarName + ".allowExpanding=false;\n");
			if(EnableAnimation)
				stb.Append(localVarName + ".enableAnimation=true;\n");
			if(IsItemSelectEnabled()) {
				stb.Append(localVarName + ".allowSelectItem=true;\n");
				stb.AppendFormat(localVarName + ".selectedItemIndexPath='{0}';\n", (SelectedItem != null) ? GetItemIndexPath(SelectedItem) : "");
			}
			stb.Append(GetClientExpandingScript(localVarName));
			stb.Append(GetClientGroupsScript(localVarName));
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && Events[EventItemClick] != null)
				eventNames.Add("ItemClick");
			if(HasEvents() && Events[EventHeaderClick] != null)
				eventNames.Add("HeaderClick");
		}
		protected string GetClientExpandingScript(string localVarName) {
			object[] expandingArray = new object[Groups.Count];
			for(int i = 0; i < Groups.Count; i++)
				expandingArray[i] = Groups[i].HasContent ? Groups[i].Expanded : false;
			return localVarName + ".groupsExpanding=" + HtmlConvertor.ToJSON(expandingArray) + ";\n";
		}
		protected string GetClientGroupsScript(string localVarName) {
			object[] groupsArray = new object[Groups.Count];
			for(int i = 0; i < Groups.Count; i++) {
				object[] groupProperties = new object[5];
				groupProperties[0] = Groups[i].Name;
				if(!Groups[i].Enabled)
					groupProperties[1] = false;
				if(!Groups[i].Visible)
					groupProperties[2] = false;
				if(!Groups[i].ClientVisible)
					groupProperties[3] = false;
				object[] itemsArray = new object[Groups[i].Items.Count];
				for(int j = 0; j < Groups[i].Items.Count; j++) {
					object[] itemProperties = new object[5];
					itemProperties[0] = Groups[i].Items[j].Name;
					if(!Groups[i].Items[j].Enabled)
						itemProperties[1] = false;
					if(!Groups[i].Items[j].ClientEnabled)
						itemProperties[2] = false;
					if(!Groups[i].Items[j].Visible)
						itemProperties[3] = false;
					if(!Groups[i].Items[j].ClientVisible)
						itemProperties[4] = false;
					itemsArray[j] = itemProperties;
				}
				groupProperties[4] = itemsArray;
				groupsArray[i] = groupProperties;
			}
			return localVarName + ".CreateGroups(" + HtmlConvertor.ToJSON(groupsArray, true) + ");\n";
		}
		protected AppearanceStyleBase[] GetItemStyles(NavBarItem item, AppearanceStyleBase baseStyle) {
			if(IsItemLinkBlock(item)) {
				AppearanceStyleBase containerStyle = new AppearanceStyleBase();
				AppearanceStyleBase contentStyle = new AppearanceStyleBase();
				if(baseStyle != null) {
					containerStyle.CopyFrom(baseStyle);
					containerStyle.Paddings.Reset();
					contentStyle.Paddings.Assign(baseStyle.Paddings);
				}
				return new AppearanceStyleBase[] { containerStyle, contentStyle };
			}
			return new AppearanceStyleBase[] { baseStyle };
		}
		protected string[] GetItemIdPostfixes(NavBarItem item) {
			if(IsItemLinkBlock(item))
				return new string[] { "", ItemIdPostfixes[1] };
			return new string[] { };
		}
		protected object[] GetItemImageObjects(object baseObject) {
			return new object[] { baseObject };
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientNavBar";
		}
		protected internal NavBarControlBase GetGroupContentControl(int groupIndex) {
			return FindControl(GetGroupContentCellID(Groups[groupIndex])) as NavBarControlBase;
		}
		protected override bool HasContent() {
			return HasVisibleGroups();
		}
		protected override void ClearControlFields() {
			fNavBarControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			fNavBarControl = new NavBarControlLite(this);
			Controls.Add(fNavBarControl);
			base.CreateControlHierarchy();
		}
		protected internal string GetGroupHeaderCellID(NavBarGroup group, bool expanded) {
			return (expanded ? "GHE" : "GHC") + group.Index.ToString();
		}
		protected internal string GetGroupHeaderExpandButtonID(NavBarGroup group, bool expanded) {
			return (expanded ? "GHEBE" : "GHEBC") + group.Index.ToString();
		}
		protected internal string GetGroupRowID(NavBarGroup group) {
			return "GR" + group.Index.ToString();
		}
		protected internal string GetGroupSeparatorRowID(NavBarGroup group) {
			return "GSR" + group.Index.ToString();
		}
		protected internal string GetGroupContentCellID(NavBarGroup group) {
			return "GC" + group.Index.ToString();
		}
		protected internal string GetGroupContentAnimationControlID(NavBarGroup group) {
			return "GCA" + group.Index.ToString();
		}
		protected internal string GetItemElementIDPrefix(NavBarGroup group) {
			return "I" + group.Index.ToString() + RenderUtils.IndexSeparator;
		}
		protected internal string GetItemElementID(NavBarItem item) {
			return GetItemElementIDPrefix(item.Group) + item.Index.ToString() + "_";
		}
		protected internal string GetItemSeparatorElementID(NavBarItem item) {
			return GetItemElementIDPrefix(item.Group) + item.Index.ToString() + "S";
		}
		protected internal string GetItemTemplateCellID(NavBarItem item) {
			return GetItemElementID(item) + "ITC";
		}
		protected internal string GetItemImageCellID(NavBarItem item) {
			return GetItemElementID(item) + ItemIdPostfixes[0];
		}
		protected internal string GetItemTextCellID(NavBarItem item) {
			return GetItemElementID(item) + ItemIdPostfixes[1];
		}
		protected internal string GetItemImageID(NavBarItem item) {
			return GetItemElementID(item) + ItemImageIdPostfixes[0];
		}
		protected internal bool HasItemCellIDs(NavBarItem item) {
			return (CanItemSelect(item) || IsClientSideAPIEnabled() || CanItemHotTrack(item)) && GetItemEnabled(item);
		}
		protected internal bool HasItemSeparatorID(NavBarItem item) {
			return IsClientSideAPIEnabled();
		}
		protected internal bool HasGroupRowIDs(NavBarGroup group) {
			return IsClientSideAPIEnabled();
		}
		protected internal bool HasGroupHeaderCellOnClick(NavBarGroup group, bool expanded) {
			return CanGroupExpanding(group, expanded) || (GetGroupEnabled(group) &&
				(ClientSideEvents.HeaderClick != "" || HasEvents() && Events[EventHeaderClick] != null));
		}
		protected internal string GetGroupHeaderCellOnClick(NavBarGroup group) {
			if(AutoPostBack && !IsClientSideEventsAssigned())
				return RenderUtils.GetPostBackEventReference(Page, this, "HEADERCLICK:" + group.Index.ToString());
			else
				return string.Format(GroupHeaderClickHandlerName, ClientID, group.Index);
		}
		protected internal string GetGroupHeaderCellOnMouseMove(NavBarGroup group) {
			return string.Format(GroupHeaderMouseMoveHandlerName, ClientID, group.Index);
		}
		protected internal string GetItemElementOnClick(NavBarItem item) {
			if(AutoPostBack && !IsClientSideEventsAssigned() && GetItemNavigateUrl(item) == "")
				return RenderUtils.GetPostBackEventReference(Page, this,
					"CLICK:" + GetItemIndexPath(item));
			else
				return string.Format(ItemClickHandlerName, ClientID, item.Group.Index, item.Index);
		}
		protected bool IsClickableItem(NavBarItem item) {
			return GetItemEnabled(item) && !IsCurrentItem(item) &&
				((ClientSideEvents.ItemClick != "") || (HasEvents() && Events[EventItemClick] != null) || AutoPostBack ||
				  (item.NavigateUrl != "" && GetItemLinkMode(item) == ItemLinkMode.ContentBounds) ||
				  IsItemSelectEnabled(item.Group));
		}
		protected internal bool HasItemElementOnClick(NavBarItem item) {
			return IsClickableItem(item) && (GetItemLinkMode(item) == ItemLinkMode.ContentBounds);
		}
		protected internal bool HasItemImageElementOnClick(NavBarItem item) {
			return HasItemImageLinkOnClickCore(item) && string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal bool HasItemImageLinkOnClickCore(NavBarItem item) {
			return IsClickableItem(item) && (GetItemLinkMode(item) == ItemLinkMode.TextAndImage);
		}
		protected internal bool HasItemTextElementOnClick(NavBarItem item) {
			return HasItemTextLinkOnClickCore(item) && string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal bool HasItemTextLinkOnClick(NavBarItem item) {
			return HasItemTextLinkOnClickCore(item) && !string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal bool HasItemTextLinkOnClickCore(NavBarItem item) {
			return IsClickableItem(item) && (GetItemLinkMode(item) != ItemLinkMode.ContentBounds);
		}
		protected internal bool IsVisibleExpandButton(NavBarGroup group) {
			return (ShowExpandButtons && group.ShowExpandButton != DefaultBoolean.False && group.HasContent) || group.ShowExpandButton == DefaultBoolean.True;
		}
		protected internal bool IsGroupNavigateUrl(NavBarGroup group) {
			return !string.IsNullOrEmpty(GetGroupNavigateUrl(group));
		}
		protected internal bool IsItemNavigateUrl(NavBarItem item) {
			return !string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal bool IsItemLinkBlock(NavBarItem item) {
			return IsItemNavigateUrl(item) && GetItemLinkMode(item) == ItemLinkMode.ContentBounds && GetItemTemplate(item) == null && GetItemTextTemplate(item) == null; 
		}
		protected internal string GetGroupNavigateUrl(NavBarGroup group) {
			string url = String.Format(GroupDataFields.NavigateUrlFormatString, group.NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected internal string GetGroupText(NavBarGroup group) {
			return String.Format(GroupDataFields.TextFormatString, group.Text);
		}
		protected internal string GetItemNavigateUrl(NavBarItem item) {
			string url = string.Empty;
			if(!IsCurrentItem(item))
				url = String.Format(ItemDataFields.NavigateUrlFormatString, item.NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected internal string GetItemText(NavBarItem item) {
			return String.Format(ItemDataFields.TextFormatString, item.Text);
		}
		protected internal bool GetItemEnabled(NavBarItem item) {
			return item.Enabled && GetGroupEnabled(item.Group);
		}
		protected internal bool GetGroupEnabled(NavBarGroup group) {
			return group.Enabled && IsEnabled();
		}
		protected internal string GetGroupTarget(NavBarGroup group) {
			return (group.Target != "") ? group.Target : Target;
		}
		protected internal string GetItemTarget(NavBarItem item) {
			return (item.Target != "") ? item.Target : Target;
		}
		protected internal string GetGroupHeaderTemplateContainerID(NavBarGroup group, bool expanded) {
			return (expanded ? "GHTCE" : "GHTCC") + group.Index.ToString();
		}
		protected internal string GetGroupContentTemplateContainerID(NavBarGroup group) {
			return "GCTC" + group.Index.ToString();
		}
		protected internal string GetItemTextTemplateContainerID(NavBarItem item) {
			return "ITTC" + GetItemIndexPath(item);
		}
		protected internal string GetItemTemplateContainerID(NavBarItem item) {
			return "ITC" + GetItemIndexPath(item);
		}
		protected internal string GetItemIndexPath(NavBarItem item) {
			return item.Group.Index.ToString() + RenderUtils.IndexSeparator + item.Index.ToString();
		}
		protected internal NavBarItem GetItemByIndexPath(string path) {
			string[] indexes = path.Split(RenderUtils.IndexSeparator);
			int groupIndex = int.Parse(indexes[0]);
			if(0 <= groupIndex && groupIndex < Groups.Count) {
				NavBarGroup group = Groups[groupIndex];
				int itemIndex = int.Parse(indexes[1]);
				if(0 <= itemIndex && itemIndex < group.Items.Count)
					return group.Items[itemIndex];
			}
			return null;
		}
		protected internal string GetItemImageToolTip(NavBarItem item) {
			return item.ToolTip;
		}
		protected internal string GetItemLinkToolTip(NavBarItem item) {
			return (GetItemLinkMode(item) != ItemLinkMode.ContentBounds) ? item.ToolTip : "";
		}
		protected internal string GetItemContentToolTip(NavBarItem item) {
			return (GetItemLinkMode(item) == ItemLinkMode.ContentBounds) ? item.ToolTip : "";
		}
		protected internal string GetGroupContentToolTip(NavBarGroup group) {
			return !IsGroupNavigateUrl(group) ? group.ToolTip : "";
		}
		protected override Style CreateControlStyle() {
			return new NavBarStyle();
		}
		protected override StylesBase CreateStyles() {
			 return new NavBarStyles(this);
		}
		protected DisabledStyle GetChildDisabledStyle() {
			return DisabledStyle;
		}
		protected internal Unit GetGroupSpacing() {
			return GetControlStyle().Spacing;
		}
		protected internal Paddings GetGroupHeaderContentPaddings(NavBarGroup group, bool expanded) {
			return GetGroupHeaderStyle(group, expanded).Paddings;
		}
		protected Unit GetGroupHeaderHeightInternal(NavBarGroup group, bool expanded) {
			return GetGroupHeaderStyle(group, expanded).Height;
		}
		protected internal Unit GetGroupHeaderHeight(NavBarGroup group, bool expanded) {
			Unit height = GetGroupHeaderHeightInternal(group, expanded);
			if(Browser.IsIE) {
				NavBarGroupHeaderStyle style = GetGroupHeaderStyle(group, expanded);
				Paddings paddings = GetGroupHeaderContentPaddings(group, expanded);
				height = UnitUtils.GetCorrectedHeight(height, style, paddings);
			}
			return height;
		}
		protected internal Unit GetGroupHeaderImageSpacing(NavBarGroup group, bool expanded) {
			return GetGroupHeaderStyle(group, expanded).ImageSpacing;
		}
		protected internal Paddings GetGroupContentPaddings(NavBarGroup group) {
			return GetGroupContentStyle(group).Paddings;
		}
		protected Paddings GetItemContentPaddingsInternal(NavBarGroup group) {
			return GetItemStyleInternal(group).Paddings;
		}
		protected internal Paddings GetItemContentPaddings(NavBarItem item) {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(GetItemContentPaddingsInternal(item.Group));
			if(IsCurrentItem(item))
				paddings.CopyFrom(GetItemSelectedCssStylePaddings(item.Group, GetItemSelectedStyle(item.Group)));
			return paddings;
		}
		protected internal Unit GetBulletIndent(NavBarGroup group) {
			return Styles.GetBulletIndent();
		}
		protected Unit GetItemHeightInternal(NavBarItem item) {
			return GetItemStyleInternal(item.Group).Height;
		}
		protected internal Unit GetItemHeightCorrected(NavBarItem item) {
			NavBarItemStyle style = GetItemStyleInternal(item.Group);
			Paddings paddings = GetItemContentPaddingsInternal(item.Group);
			return UnitUtils.GetCorrectedHeight(GetItemHeightInternal(item), style, paddings);
		}
		protected internal Unit GetItemHeight(NavBarItem item) {
			return Browser.IsIE ? GetItemHeightCorrected(item) : GetItemHeightInternal(item);
		}
		protected internal Unit GetItemSpacing(NavBarGroup group) {
			return GetGroupContentStyle(group).ItemSpacing;
		}
		protected internal Unit GetItemImageSpacing(NavBarGroup group) {
			return GetItemStyleInternal(group).ImageSpacing;
		}
		protected NavBarGroupHeaderStyle GetCustomGroupHeaderStyle(NavBarGroup group, bool expanded) {
			NavBarGroupHeaderStyle style = new NavBarGroupHeaderStyle();
			style.CopyFrom(GroupHeaderStyle);
			if(group != null)
				style.CopyFrom(group.HeaderStyle);
			if(!expanded) {
				style.CopyFrom(GroupHeaderStyleCollapsed);
				if(group != null)
					style.CopyFrom(group.HeaderStyleCollapsed);
			}
			return style;
		}
		protected AppearanceStyleBase GetDefaultGroupHeaderStyle(bool expanded) {
			return Styles.GetDefaultGroupHeaderStyle(expanded);
		}
		static object headerInternalStyleKey = new object();
		protected internal NavBarGroupHeaderStyle GetGroupHeaderStyleInternal(NavBarGroup group, bool expanded) {
			return (NavBarGroupHeaderStyle)CreateStyle(delegate() {
				NavBarGroupHeaderStyle style = new NavBarGroupHeaderStyle();
				style.CopyFrom(GetDefaultGroupHeaderStyle(expanded));
				style.CopyFrom(GetCustomGroupHeaderStyle(group, expanded));
				return style;
			}, group, GetBoolParam(expanded), headerInternalStyleKey);
		}
		static object headerStyleKey = new object();
		protected internal NavBarGroupHeaderStyle GetGroupHeaderStyle(NavBarGroup group, bool expanded) {
			return (NavBarGroupHeaderStyle)CreateStyle(delegate() {
				NavBarGroupHeaderStyle style = new NavBarGroupHeaderStyle();
				style.CopyFrom(GetGroupHeaderStyleInternal(group, expanded));
				if(group != null) {
					bool needPointerCursor = CanGroupDragging(group) || HasGroupHeaderCellOnClick(group, expanded);
					if(!needPointerCursor)
						style.Cursor = RenderUtils.GetDefaultCursor();
					MergeDisableStyle(style, GetGroupEnabled(group), GetChildDisabledStyle());
				}
				return style;
			}, group, GetBoolParam(expanded), headerStyleKey);
		}
		static object headerLinkStyleKey = new object();
		protected internal AppearanceStyleBase GetGroupHeaderLinkStyle(NavBarGroup group, bool expanded) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFontAndCursorFrom(GetDefaultGroupHeaderStyle(expanded));
				style.CopyFontAndCursorFrom(GetCustomGroupHeaderStyle(group, expanded));
				style.CopyFrom(LinkStyle.Style);
				MergeDisableStyle(style, GetGroupEnabled(group), GetChildDisabledStyle());
				return style;
			}, group, GetBoolParam(expanded), headerLinkStyleKey);
		}
		protected AppearanceStyleBase GetGroupHeaderHoverStyleInternal(NavBarGroup group, bool expanded) {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultGroupHeaderHoverStyle(expanded));
			style.CopyFrom(GroupHeaderStyle.HoverStyle);
			style.CopyFrom(group.HeaderStyle.HoverStyle);
			if(!expanded) {
				style.CopyFrom(GroupHeaderStyleCollapsed.HoverStyle);
				style.CopyFrom(group.HeaderStyleCollapsed.HoverStyle);
			}
			return style;
		}
		protected internal AppearanceStyleBase GetGroupHeaderHoverStyle(NavBarGroup group, bool expanded) {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CopyBordersFrom(GetGroupHeaderStyleInternal(group, expanded));
			style.CopyFrom(GetGroupHeaderHoverStyleInternal(group, expanded));
			return style;
		}
		protected internal AppearanceStyle GetGroupHeaderHoverCssStyle(NavBarGroup group, bool expanded) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetGroupHeaderHoverStyle(group, expanded));
			style.Paddings.CopyFrom(GetGroupHeaderHoverStylePaddings(group, expanded));
			return style;
		}
		protected Paddings GetGroupHeaderHoverStylePaddings(NavBarGroup group, bool expanded) {
			Paddings paddings = GetGroupHeaderContentPaddings(group, expanded);
			AppearanceStyle style = GetGroupHeaderStyle(group, expanded);
			AppearanceStyleBase selectedStyle = GetGroupHeaderHoverStyle(group, expanded);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		static object contentStyleKey = new object();
		protected internal NavBarGroupContentStyle GetGroupContentStyle(NavBarGroup group) {
			return (NavBarGroupContentStyle)CreateStyle(delegate() {
				NavBarGroupContentStyle style = new NavBarGroupContentStyle();
				style.CopyFrom(Styles.GetDefaultGroupContentStyle(IsBulletMode(group), IsLargeItems(group)));
				style.CopyFrom(GroupContentStyle);
				if(group != null)
					style.CopyFrom(group.ContentStyle);
				return style;
			}, group, contentStyleKey);
		}
		protected NavBarItemStyle GetCustomItemStyle(NavBarGroup group) {
			NavBarItemStyle style = new NavBarItemStyle();
			AppearanceStyleBase contentStyle = GetGroupContentStyle(group);
			if(contentStyle.Cursor != "")
				style.Cursor = contentStyle.Cursor;
			if(contentStyle.HorizontalAlign != HorizontalAlign.NotSet)
				style.HorizontalAlign = contentStyle.HorizontalAlign;
			if(contentStyle.Wrap != DefaultBoolean.Default)
				style.Wrap = contentStyle.Wrap;
			style.CopyFrom(ItemStyle);
			if(group != null)
				style.CopyFrom(group.ItemStyle);
			return style;
		}
		protected AppearanceStyleBase GetDefaultItemStyle(NavBarGroup group) {
			return Styles.GetDefaultItemStyle(IsBulletMode(group), IsLargeItems(group));
		}
		static object itemInternalStyleKey = new object();
		protected NavBarItemStyle GetItemStyleInternal(NavBarGroup group) {
			return (NavBarItemStyle)CreateStyle(delegate() {
				NavBarItemStyle style = new NavBarItemStyle();
				style.CopyFrom(GetDefaultItemStyle(group));
				style.CopyFrom(GetCustomItemStyle(group));
				return style;
			}, group, itemInternalStyleKey);
		}
		static object itemStyleKey = new object();
		protected internal NavBarItemStyle GetItemStyle(NavBarItem item) {
			return (NavBarItemStyle)CreateStyle(delegate() {
				NavBarItemStyle style = new NavBarItemStyle();
				style.CopyFrom(GetItemStyleInternal(item != null ? item.Group : null));
				if(item != null) {
					if(!IsBulletMode(item.Group) && IsLargeItems(item.Group) && (style.HorizontalAlign == HorizontalAlign.NotSet))
						style.HorizontalAlign = HorizontalAlign.Center;
					if(IsCurrentItem(item))
						style.CopyFrom(GetItemSelectedStyle(item.Group));
					MergeDisableStyle(style, GetItemEnabled(item), GetChildDisabledStyle(), false);
				}
				return style;
			}, item, itemStyleKey);
		}
		static object itemLinkStyleKey = new object();
		protected internal AppearanceStyleBase GetItemLinkStyle(NavBarItem item) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFontAndCursorFrom(GetDefaultItemStyle(item.Group));
				style.CopyFontAndCursorFrom(GetCustomItemStyle(item.Group));
				if(!IsCurrentItem(item))
					style.CopyFrom(LinkStyle.Style);
				MergeDisableStyle(style, GetItemEnabled(item), GetChildDisabledStyle());
				return style;
			}, item, itemLinkStyleKey);
		}
		object itemHoverStyleKey = new object();
		protected AppearanceStyleBase GetItemHoverStyleInternal(NavBarGroup group) {
			return CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFrom(Styles.GetDefaultItemHoverStyle(IsBulletMode(group), IsLargeItems(group)));
				style.CopyFrom(ItemStyle.HoverStyle);
				style.CopyFrom(group.ItemStyle.HoverStyle);
				return style;
			}, group, itemHoverStyleKey);
		}
		protected internal AppearanceStyleBase GetItemHoverStyle(NavBarGroup group) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyBordersFrom(GetItemStyleInternal(group));
			style.CopyFrom(GetItemHoverStyleInternal(group));
			return style;
		}
		protected internal AppearanceStyleBase GetItemHoverCssStyle(NavBarGroup group) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemHoverStyle(group));
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(group, style));
			return style;
		}
		protected internal AppearanceStyleBase GetItemSelectedStyle(NavBarGroup group) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyBordersFrom(GetItemStyleInternal(group));
			style.CopyFrom(Styles.GetDefaultItemSelectedStyle(IsBulletMode(group), IsLargeItems(group)));
			style.CopyFrom(ItemStyle.SelectedStyle);
			style.CopyFrom(group.ItemStyle.SelectedStyle);
			return style;
		}
		protected internal AppearanceStyle GetItemSelectedCssStyle(NavBarGroup group) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemSelectedStyle(group));
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(group, style));
			return style;
		}
		protected Paddings GetItemSelectedCssStylePaddings(NavBarGroup group, AppearanceStyleBase selectedStyle) {
			AppearanceStyle style = GetItemStyleInternal(group);
			Paddings paddings = GetItemContentPaddingsInternal(group);
			return UnitUtils.GetSelectedCssStylePaddings(style, selectedStyle, paddings);
		}
		protected internal DisabledStyle GetItemDisabledCssStyle(NavBarGroup group) {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(Styles.GetDefaultItemDisabledStyle());
			style.CopyFrom(DisabledStyle);
			return style;
		}
		protected internal bool IsBulletMode(NavBarGroup group) {
			if(group == null || group.ItemBulletStyle == ItemBulletStyle.None) return false;
			foreach(NavBarItem item in group.Items)
				if(!GetItemImagePropertiesInternal(item).IsEmpty)
					return false;
			return true;
		}
		protected internal bool IsLargeItems(NavBarGroup group) {
			if(group == null) return false;
			return group.ItemImagePosition == ImagePosition.Top ||
				group.ItemImagePosition == ImagePosition.Bottom;
		}
		public bool HasVisibleGroups() {
			return Groups.GetVisibleItemCount() > 0;
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[EventItemClick] != null || Events[EventHeaderClick] != null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new NavBarClientSideEvents();
		}
		protected internal bool UseClientSideVisibility(NavBarGroup group) {
			return UseClientSideVisibility() && CanGroupExpanding(group);
		}
		protected internal override bool IsCallBacksEnabled() {
			return EnableCallBacks && !AutoPostBack;
		}
		protected bool IsGroupHeaderHotTrackEnabled(NavBarGroup group) {
			return EnableHotTrack;
		}
		protected bool IsItemHotTrackEnabled(NavBarGroup group) {
			return EnableHotTrack && (GetItemLinkMode(group) == ItemLinkMode.ContentBounds);
		}
		protected bool IsItemSelectEnabled(NavBarGroup group) {
			return AllowSelectItem && (GetItemLinkMode(group) == ItemLinkMode.ContentBounds);
		}
		protected bool IsItemSelectEnabled() {
			for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = Groups.GetVisibleItem(i);
				if(IsItemSelectEnabled(group))
					return true;
			}
			return false;
		}
		protected internal bool IsCurrentItem(NavBarItem item) {
			return item.Selected && (!IsItemSelectEnabled() || DesignMode) && GetItemTemplate(item) == null;
		}
		protected internal bool CanGroupDragging(NavBarGroup group) {
			return GetGroupEnabled(group) && group.AllowDragging && AllowDragging;
		}
		protected internal bool CanGroupExpanding(NavBarGroup group) {
			return GetGroupEnabled(group) && group.AllowExpanding && group.HasContent && AllowExpanding;
		}
		protected internal bool CanGroupExpanding(NavBarGroup group, bool expanded) {
			return CanGroupExpanding(group) && !(expanded && AutoCollapse);
		}
		protected internal bool CanGroupHeaderHotTrack(NavBarGroup group, bool expanded) {
			return IsGroupHeaderHotTrackEnabled(group) && !GetGroupHeaderHoverStyleInternal(group, expanded).IsEmpty;
		}
		protected internal bool CanItemHotTrack(NavBarGroup group) {
			if(!IsItemHotTrackEnabled(group) || GetGroupContentTemplate(group) != null)
				return false;
			if(!GetItemHoverStyleInternal(group).IsEmpty)
				return true;
			for(int j = 0; j < group.Items.GetVisibleItemCount(); j++) {
				NavBarItem item = group.Items.GetVisibleItem(j);
				if(!GetItemImagePropertiesInternal(item).IsEmptyHottracked)
					return true;
			}
			return false;
		}
		protected internal bool CanItemHotTrack(NavBarItem item) {
			return !IsCurrentItem(item) && GetItemTemplate(item) == null && CanItemHotTrack(item.Group);
		}
		protected internal bool CanItemSelect(NavBarGroup group) {
			if(!IsItemSelectEnabled(group) || !GetGroupEnabled(group) ||
				GetGroupContentTemplate(group) != null)
				return false;
			if(!GetItemSelectedStyle(group).IsEmpty)
				return true;
			for(int j = 0; j < group.Items.GetVisibleItemCount(); j++) {
				NavBarItem item = group.Items.GetVisibleItem(j);
				if(!GetItemImagePropertiesInternal(item).IsEmptySelected) {
					return true;
				}
			}
			return false;
		}
		protected internal bool CanItemSelect(NavBarItem item) {
			return GetItemEnabled(item) && CanItemSelect(item.Group) &&
				GetItemTemplate(item) == null;
		}
		protected internal ItemLinkMode GetItemLinkMode(NavBarGroup group) {
			if(IsBulletMode(group))
				return ItemLinkMode.TextOnly;
			else
				if(group.ItemLinkMode != GroupItemLinkMode.Default) {
					ItemLinkMode itemLinkMode = ItemLinkMode.ContentBounds;
					switch(group.ItemLinkMode) {
						case GroupItemLinkMode.TextOnly:
							itemLinkMode = ItemLinkMode.TextOnly;
							break;
						case GroupItemLinkMode.TextAndImage:
							itemLinkMode = ItemLinkMode.TextAndImage;
							break;
					}
					return itemLinkMode;
				} else
					return ItemLinkMode;
		}
		protected internal ItemLinkMode GetItemLinkMode(NavBarItem item) {
			return GetItemLinkMode(item.Group);
		}
		protected void LoadGroupsState(string state) {
			LoadGroupsState(state, new List<NavBarGroup>());
		}
		protected void LoadGroupsState(string state, List<NavBarGroup> changedGroups) {
			string[] states = state.Split(new char[] { ';' });
			for(int i = 0; i < Groups.Count; i++) {
				if(i < states.Length) {
					bool newExpanded = (states[i] == "1");
					if(newExpanded != Groups[i].Expanded) {
						Groups[i].Expanded = newExpanded;
						changedGroups.Add(Groups[i]);
					}
				}
			}
		}
		protected string SaveGroupsState() {
			string state = "";
			for(int i = 0; i < Groups.Count; i++) {
				state += Groups[i].Expanded ? "1" : "0";
				if(i < Groups.Count - 1) state += ";";
			}
			return state;
		}
		protected internal override void LoadClientState(string state) {
			EnsureDataBound();
			LoadGroupsState(state);
		}
		protected internal override string SaveClientState() {
			return SaveGroupsState();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected override void SelectCurrentPath(bool ignoreQueryString) {
			for(int i = 0; i < Items.Count; i++) {
				NavBarItem item = Items[i];
				if(UrlUtils.IsCurrentUrl(ResolveUrl(GetItemNavigateUrl(item)), ignoreQueryString)) {
					item.Selected = true;
					item.Group.Expanded = true;
					break;
				}
			}
		}
		protected void ValidateSelectedItem() {
			for(int i = 0; i < Groups.Count; i++) {
				for(int j = 0; j < Groups[i].Items.Count; j++) {
					NavBarItem item = Groups[i].Items[j];
					if(item.Selected && SelectedItem != item)
						SelectedItem = item;
				}
			}
			if(SelectedItem != null && SelectedItem.NavBar != this) 
				SelectedItem = null;
		}
		protected internal ITemplate GetGroupContentTemplate(NavBarGroup group) {
			return (group.ContentTemplate != null) ? group.ContentTemplate : GroupContentTemplate;
		}
		protected internal ITemplate GetGroupHeaderTemplate(NavBarGroup group, bool expanded) {
			ITemplate template = (group.HeaderTemplate != null) ? group.HeaderTemplate : GroupHeaderTemplate;
			if(!expanded) {
				if(group.HeaderTemplateCollapsed != null)
					template = group.HeaderTemplateCollapsed;
				else
					if(GroupHeaderTemplateCollapsed != null)
						template = GroupHeaderTemplateCollapsed;
			}
			return template;
		}
		protected internal ITemplate GetItemTemplate(NavBarItem item) {
			if(item.Template != null)
				return item.Template;
			else if(item.Group.ItemTemplate != null)
				return item.Group.ItemTemplate;
			else
				return ItemTemplate;
		}
		protected internal ITemplate GetItemTextTemplate(NavBarItem item) {
			if(item.TextTemplate != null)
				return item.TextTemplate;
			else if(item.Group.ItemTextTemplate != null)
				return item.Group.ItemTextTemplate;
			else
				return ItemTextTemplate;
		}
		protected virtual void OnItemClick(NavBarItemEventArgs e) {
			NavBarItemEventHandler handler = (NavBarItemEventHandler)Events[EventItemClick];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnExpandedChanged(NavBarGroupEventArgs e) {
			NavBarGroupEventHandler handler = (NavBarGroupEventHandler)Events[EventExpandedChanged];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnExpandedChanging(ref NavBarGroupCancelEventArgs e) {
			NavBarGroupCancelEventHandler handler = (NavBarGroupCancelEventHandler)Events[EventExpandedChanging];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnHeaderClick(ref NavBarGroupCancelEventArgs e) {
			NavBarGroupCancelEventHandler handler = (NavBarGroupCancelEventHandler)Events[EventHeaderClick];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnGroupCommand(NavBarGroupCommandEventArgs e) {
			NavBarGroupCommandEventHandler handler = (NavBarGroupCommandEventHandler)Events[EventGroupCommand];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnItemCommand(NavBarItemCommandEventArgs e) {
			NavBarItemCommandEventHandler handler = (NavBarItemCommandEventHandler)Events[EventItemCommand];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnItemDataBound(NavBarItemEventArgs e) {
			NavBarItemEventHandler handler = (NavBarItemEventHandler)Events[EventItemDataBound];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnGroupDataBound(NavBarGroupEventArgs e) {
			NavBarGroupEventHandler handler = (NavBarGroupEventHandler)Events[EventGroupDataBound];
			if(handler != null)
				handler(this, e);
		}
		protected internal void GroupsChanged() {
			if(!IsLoading()) {
				ValidateAutoCollapse(null);
				Items.Reset();
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected internal void ItemsChanged() {
			if(!IsLoading()) {
				Items.Reset();
				ValidateSelectedItem();
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected void ChangeGroupExpanding(NavBarGroup group, bool expanded) {
			NavBarGroupCancelEventArgs args = new NavBarGroupCancelEventArgs(group);
			OnExpandedChanging(ref args);
			if(!args.Cancel) {
				group.Expanded = expanded;
				OnExpandedChanged(new NavBarGroupEventArgs(group));
			}
		}
		protected NavBarGroup GetActiveGroup() {
			if(AutoCollapse) {
				foreach(NavBarGroup group in Groups) {
					if(group.Expanded)
						return group;
				}
			}
			return null;
		}
		protected void SetActiveGroup(NavBarGroup group) {
			if(AutoCollapse && group != null)
				group.Expanded = true;
		}
		protected bool IsValidActiveGroupIndex(int index) {
			if((0 <= index) && (index < Groups.Count))
				return Groups[index].Enabled && Groups[index].Visible;
			return false;
		}
		protected internal void ValidateAutoCollapse(NavBarGroup activeGroup) {
			if(!fLockAutoCollapse && HasVisibleGroups() && !IsLoading() && AutoCollapse) {
				fLockAutoCollapse = true;
				try {
					if(activeGroup != null) {
						Groups.CollapseAll(activeGroup);
						if(!activeGroup.Expanded)
							activeGroup.Expanded = true;
					} else {
						NavBarGroup expandedGroup = null;
						for(int i = 0; i < Groups.GetVisibleItemCount(); i++) {
							NavBarGroup group = Groups.GetVisibleItem(i);
							if(group.HasContent && group.Expanded) {
								expandedGroup = group;
								break;
							}
						}
						if(expandedGroup != null)
							Groups.CollapseAll(expandedGroup);
						else
							Groups.GetVisibleItem(0).Expanded = true;
					}
				} finally {
					fLockAutoCollapse = false;
				}
			}
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
			ValidateSelectedItem();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			EnsureDataBound();
			if(IsItemSelectEnabled()) {
				string indexPath = GetClientObjectStateValue<string>(SelectedItemIndexPathKey);
				if(indexPath != null) {
					if(indexPath != string.Empty)
						SelectedItem = GetItemByIndexPath(indexPath);
					else
						SelectedItem = null;
				}
			}
			fExpandedChangedGroups = new List<NavBarGroup>();
			string stateString = GetClientObjectStateValue<string>(GroupsExpandingStateKey);
			if(!string.IsNullOrEmpty(stateString))
				LoadGroupsState(stateString, fExpandedChangedGroups);
			return fExpandedChangedGroups.Count > 0;
		}
		protected override void RaisePostDataChangedEvent() {
			for(int i = 0; i < fExpandedChangedGroups.Count; i++)
				OnExpandedChanged(new NavBarGroupEventArgs(fExpandedChangedGroups[i]));
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			EnsureDataBound();
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch(arguments[0]) {
				case "EXPAND":
				case "HEADERCLICK":
					NavBarGroup group = Groups[int.Parse(arguments[1])];
					NavBarGroupCancelEventArgs args = new NavBarGroupCancelEventArgs(group);
					if(arguments[0] == "HEADERCLICK")
						OnHeaderClick(ref args);
					if(CanGroupExpanding(group) && !args.Cancel)
						ChangeGroupExpanding(group, !group.Expanded);
					break;
				case "CLICK":
					NavBarItem item = GetItemByIndexPath(arguments[1]);
					if(IsItemSelectEnabled())
						SelectedItem = item;
					OnItemClick(new NavBarItemEventArgs(item));
					break;
			}
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.Html] = GetCallbackResultHtml();
			result[CallbackResultProperties.Index] = CallbackGroupIndex;
			return result;
		}
		protected virtual string GetCallbackResultHtml() {
			string result = string.Empty;
			NavBarControlBase groupControl = GetCallbackResultControl();
			if(groupControl != null) {
				groupControl.Visible = true;
				BeginRendering();
				try {
					result = RenderUtils.GetRenderResult(groupControl);
				} finally {
					EndRendering();
				}
			}
			return result;
		}
		protected virtual NavBarControlBase GetCallbackResultControl() {
			return GetGroupContentControl(CallbackGroupIndex);
		}
		protected override object GetCallbackErrorData() {
			return CallbackGroupIndex;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			this.callbackGroupIndex = int.Parse(eventArgument);
		}
		static internal string GetItemBulletStyleAttribute(ItemBulletStyle bulletStyle) {
			string result = Regex.Replace(bulletStyle.ToString(), "([a-z])([A-Z])", "$1-$2");
			return result.ToLower();
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.NavBarItemsOwner"; } }
	}
	public class NavBarItems {
		private ASPxNavBar fNavBar = null;
		private List<NavBarItem> fItems = null;
#if !SL
	[DevExpressWebLocalizedDescription("NavBarItemsItem")]
#endif
		public NavBarItem this[int index] {
			get { return ItemList[index]; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("NavBarItemsCount")]
#endif
		public int Count {
			get { return ItemList.Count; }
		}
		protected ASPxNavBar NavBar {
			get { return fNavBar; }
		}
		protected List<NavBarItem> ItemList {
			get {
				if(NavBar.IsLoading())
					throw new IndexOutOfRangeException(StringResources.NavBar_InvalidAccessItemsWhileLoading);
				CheckItemList();
				return fItems;
			}
		}
		public NavBarItems(ASPxNavBar navBar) {
			fNavBar = navBar;
		}
		public NavBarItem FindByName(string name) {
			for(int i = 0; i < NavBar.Groups.Count; i++) {
				NavBarItem item = NavBar.Groups[i].Items.FindByName(name);
				if(item != null)
					return item;
			}
			return null;
		}
		public NavBarItem FindByText(string text) {
			for(int i = 0; i < NavBar.Groups.Count; i++) {
				NavBarItem item = NavBar.Groups[i].Items.FindByText(text);
				if(item != null)
					return item;
			}
			return null;
		}
		protected void CheckItemList() {
			if(fItems == null) {
				fItems = new List<NavBarItem>();
				for(int i = 0; i < NavBar.Groups.Count; i++) {
					for(int j = 0; j < NavBar.Groups[i].Items.Count; j++) {
						fItems.Add(NavBar.Groups[i].Items[j]);
					}
				}
			}
		}
		protected internal void Reset() {
			fItems = null;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class NavBarDataFields : StateManager, IDataSourceViewSchemaAccessor {
		private ASPxNavBar navBar = null;
		protected internal string nameFieldName, navigateUrlFieldName, textFieldName, toolTipFieldName;
		public NavBarDataFields(ASPxNavBar navBar) {
			this.navBar = navBar;
		}
		[Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxNavBar NavBar {
			get { return navBar; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("NavBarDataFieldsNameField")]
#endif
		public abstract string NameField { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("NavBarDataFieldsNavigateUrlField")]
#endif
		public abstract string NavigateUrlField { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("NavBarDataFieldsNavigateUrlFormatString")]
#endif
		public abstract string NavigateUrlFormatString { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("NavBarDataFieldsTextField")]
#endif
		public abstract string TextField { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("NavBarDataFieldsTextFormatString")]
#endif
		public abstract string TextFormatString { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("NavBarDataFieldsToolTipField")]
#endif
		public abstract string ToolTipField { get; set; }
		protected void Changed() {
			if(NavBar != null)
				NavBar.OnDataFieldChangedInternal();
		}
		protected internal virtual void SpecifyDataFields() {
			nameFieldName = String.IsNullOrEmpty(NameField) ? "Name" : NameField;
			navigateUrlFieldName = String.IsNullOrEmpty(NavigateUrlField) ? "NavigateUrl" : NavigateUrlField;
			textFieldName = String.IsNullOrEmpty(TextField) ? "Text" : TextField;
			toolTipFieldName = String.IsNullOrEmpty(ToolTipField) ? "ToolTip" : ToolTipField;
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get { return ((IDataSourceViewSchemaAccessor)NavBar).DataSourceViewSchema; }
			set { ((IDataSourceViewSchemaAccessor)NavBar).DataSourceViewSchema = value; }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class NavBarGroupDataFields : NavBarDataFields {
		protected internal string headerImageUrlFieldName;
		public NavBarGroupDataFields(ASPxNavBar navBar)
			: base(navBar) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsNameField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public override string NameField {
			get { return GetStringProperty("NameField", ""); }
			set {
				SetStringProperty("NameField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsNavigateUrlField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public override string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", ""); }
			set {
				SetStringProperty("NavigateUrlField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), NotifyParentProperty(true),
		AutoFormatEnable]
		public override string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsTextField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public override string TextField {
			get { return GetStringProperty("TextField", ""); }
			set {
				SetStringProperty("TextField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsTextFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), NotifyParentProperty(true),
		AutoFormatEnable]
		public override string TextFormatString {
			get { return GetStringProperty("TextFormatString", "{0}"); }
			set { SetStringProperty("TextFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsToolTipField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public override string ToolTipField {
			get { return GetStringProperty("ToolTipField", ""); }
			set {
				SetStringProperty("ToolTipField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarGroupDataFieldsHeaderImageUrlField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string HeaderImageUrlField {
			get { return GetStringProperty("HeaderImageUrlField", ""); }
			set {
				SetStringProperty("HeaderImageUrlField", "", value);
				Changed();
			}
		}
		protected internal override void SpecifyDataFields() {
			base.SpecifyDataFields();
			headerImageUrlFieldName = String.IsNullOrEmpty(HeaderImageUrlField) ? "HeaderImageUrl" : HeaderImageUrlField;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class NavBarItemDataFields : NavBarDataFields {
		protected internal string imageUrlFieldName;
		public NavBarItemDataFields(ASPxNavBar navBar)
			: base(navBar) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsNameField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter("DevExpress.Web.Design.DataSourceViewChildSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public override string NameField {
			get { return GetStringProperty("NameField", ""); }
			set {
				SetStringProperty("NameField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsNavigateUrlField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter("DevExpress.Web.Design.DataSourceViewChildSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public override string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", ""); }
			set {
				SetStringProperty("NavigateUrlField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), NotifyParentProperty(true),
		AutoFormatEnable]
		public override string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsTextField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter("DevExpress.Web.Design.DataSourceViewChildSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public override string TextField {
			get { return GetStringProperty("TextField", ""); }
			set {
				SetStringProperty("TextField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsTextFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), NotifyParentProperty(true),
		AutoFormatEnable]
		public override string TextFormatString {
			get { return GetStringProperty("TextFormatString", "{0}"); }
			set { SetStringProperty("TextFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsToolTipField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter("DevExpress.Web.Design.DataSourceViewChildSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public override string ToolTipField {
			get { return GetStringProperty("ToolTipField", ""); }
			set {
				SetStringProperty("ToolTipField", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarItemDataFieldsImageUrlField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter("DevExpress.Web.Design.DataSourceViewChildSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", ""); }
			set {
				SetStringProperty("ImageUrlField", "", value);
				Changed();
			}
		}
		protected internal override void SpecifyDataFields() {
			base.SpecifyDataFields();
			imageUrlFieldName = String.IsNullOrEmpty(ImageUrlField) ? "ImageUrl" : ImageUrlField;
		}
	}
}
