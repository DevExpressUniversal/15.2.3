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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Design;
	using DevExpress.Web.Internal;
	public enum AutoSeparatorMode { All, RootOnly, None }
	public enum BorderBetweenItemAndSubMenuMode { HideAll, HideRootOnly, ShowAll }
	public enum FirstSubMenuDirection { Auto, RightOrBottom, LeftOrTop }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ItemSubMenuOffset : PropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemSubMenuOffsetFirstItemX"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatEnable]
		public int FirstItemX {
			get { return GetIntProperty("FirstItemX", 0);} 
			set { SetIntProperty("FirstItemX", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemSubMenuOffsetFirstItemY"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatEnable]
		public int FirstItemY {
			get { return GetIntProperty("FirstItemY", 0); }
			set { SetIntProperty("FirstItemY", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemSubMenuOffsetLastItemX"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatEnable]
		public int LastItemX {
			get { return GetIntProperty("LastItemX", 0); }
			set { SetIntProperty("LastItemX", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemSubMenuOffsetLastItemY"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatEnable]
		public int LastItemY {
			get { return GetIntProperty("LastItemY", 0); }
			set { SetIntProperty("LastItemY", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemSubMenuOffsetX"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatEnable]
		public int X {
			get { return GetIntProperty("X", 0); }
			set { SetIntProperty("X", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemSubMenuOffsetY"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatEnable]
		public int Y {
			get { return GetIntProperty("Y", 0); }
			set { SetIntProperty("Y", 0, value); }
		}
		public override void Assign(PropertiesBase source) {
			ItemSubMenuOffset offset = source as ItemSubMenuOffset;
			if(offset != null) {
				FirstItemX = offset.FirstItemX;
				FirstItemY = offset.FirstItemY;
				LastItemX = offset.LastItemX;
				LastItemY = offset.LastItemY;
				X = offset.X;
				Y = offset.Y;
			}
			base.Assign(source);
		}
		public override string ToString() {
			if(X != 0 || Y != 0)
				return X.ToString() + ", " + Y.ToString();
			return "";
		}
	}
	[DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxMenu"), 
	DefaultProperty("Items"), DefaultEvent("ItemClick"),
	Designer("DevExpress.Web.Design.ASPxMenuDesignerBase, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public abstract class ASPxMenuBase : ASPxHierarchicalDataWebControl, IRequiresLoadPostDataControl, IControlDesigner {
		protected const int DefaultAppearAfter = 300;
		protected const int DefaultDisappearAfter = 500;
		protected internal const string MenuScriptResourceName = WebScriptsResourcePath + "Menu.js";
		protected internal const string ItemClickHandlerName = "ASPx.MIClick(event, '{0}', '{1}')";
		protected internal const string ItemDropDownClickHandlerName = "ASPx.MIDDClick(event, '{0}', '{1}')";
		protected internal const string CheckedStateKey = "checkedState";
		protected internal const string SelectedItemIndexPathKey = "selectedItemIndexPath";
		protected internal static string[] ItemIdPostfixes = new string[] { "I", "N", "T", "P" };
		protected internal static string[] ItemImageIdPostfixes = new string[] { "Img", "PImg" };
		protected internal static string[] ScrollButtonIdPostfixes = new string[] { "U", "D" };
		protected internal static string[] ScrollButtonImageIdPostfixes = new string[] { "Img" };
		protected WebControl RootControl { get; private set; }
		protected WebControl MenuControl { get; private set; }
		protected WebControl SubMenuControl { get; private set; }
		private MenuItem RootItemInternal { get; set; }
		private MenuItem SelectedItemInternal { get; set; }
		protected string SampleMenuHTML { get; set; }
		private Dictionary<string, List<MenuItem>> itemCheckedGroups = new Dictionary<string, List<MenuItem>>();
		private Dictionary<string, Control> subMenuContentControls = new Dictionary<string, Control>();
		private RenderHelper renderHelper;
		private ItemSubMenuOffset fItemSubMenuOffset = null;
		private ItemSubMenuOffset fRootItemSubMenuOffset = null;
		private ITemplate fItemTemplate = null;
		private ITemplate fItemTextTemplate = null;
		private ITemplate fSubMenuTemplate = null;
		private ITemplate fRootItemTemplate = null;
		private ITemplate fRootItemTextTemplate = null;
		private bool fDataBound = false;
		private static readonly object EventItemClick = new object();
		private static readonly object EventItemCommand = new object();
		private static readonly object EventItemDataBound = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseAppearAfter"),
#endif
		Category("Behavior"), DefaultValue(DefaultAppearAfter), AutoFormatDisable]
		public int AppearAfter {
			get { return GetIntProperty("AppearAfter", DefaultAppearAfter); }
			set {
				CommonUtils.CheckNegativeValue(value, "AppearAfter");
				SetIntProperty("AppearAfter", DefaultAppearAfter, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseAllowSelectItem"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AllowSelectItem {
			get { return GetBoolProperty("AllowSelectItem", false); }
			set { SetBoolProperty("AllowSelectItem", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseApplyItemStyleToTemplates"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public bool ApplyItemStyleToTemplates {
			get { return GetBoolProperty("ApplyItemStyleToTemplates", false); }
			set { SetBoolProperty("ApplyItemStyleToTemplates", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSelectParentItem"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool SelectParentItem {
			get { return GetBoolProperty("SelectParentItem", false); }
			set { SetBoolProperty("SelectParentItem", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSyncSelectionWithCurrentPath"),
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseSyncSelectionMode"),
#endif
		Category("Behavior"), DefaultValue(SyncSelectionMode.CurrentPathAndQuery), AutoFormatDisable]
		public new SyncSelectionMode SyncSelectionMode {
			get { return base.SyncSelectionMode; }
			set { base.SyncSelectionMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseAutoSeparators"),
#endif
		Category("Behavior"), DefaultValue(AutoSeparatorMode.None), AutoFormatEnable]
		public AutoSeparatorMode AutoSeparators {
			get { return (AutoSeparatorMode)GetEnumProperty("AutoSeparators", AutoSeparatorMode.None); }
			set {
				SetEnumProperty("AutoSeparators", AutoSeparatorMode.None, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public MenuClientSideEvents ClientSideEvents {
			get { return (MenuClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseDisappearAfter"),
#endif
		Category("Behavior"), DefaultValue(DefaultDisappearAfter), AutoFormatDisable]
		public int DisappearAfter {
			get { return GetIntProperty("DisappearAfter", DefaultDisappearAfter); }
			set {
				CommonUtils.CheckNegativeValue(value, "DisappearAfter");
				SetIntProperty("DisappearAfter", DefaultDisappearAfter, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseEnableAnimation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableAnimation {
			get { return GetBoolProperty("EnableAnimation", true); }
			set { SetBoolProperty("EnableAnimation", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseEnableCallBacks"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseEnableHotTrack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool EnableHotTrack {
			get { return EnableHotTrackInternal; }
			set { EnableHotTrackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseRenderIFrameForPopupElements"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean RenderIFrameForPopupElements {
			get { return RenderIFrameForPopupElementsInternal; }
			set { RenderIFrameForPopupElementsInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseGutterBackgroundImage"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage GutterBackgroundImage {
			get { return ((MenuStyle)ControlStyle).GutterBackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseGutterColor"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(Color), ""),
		TypeConverter(typeof(WebColorConverter)),
		Obsolete("Use the GutterBackgroundImage property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color GutterColor {
			get { return Color.Empty; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseGutterCssClass"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(""), Localizable(false)]
		public string GutterCssClass {
			get { return ((MenuStyle)ControlStyle).GutterCssClass; }
			set { ((MenuStyle)ControlStyle).GutterCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseRenderMode"),
#endif
		Obsolete("This property is now obsolete. The Lightweight render mode is used."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Category("Layout"), AutoFormatDisable, DefaultValue(ControlRenderMode.Lightweight)]
		public ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseGutterWidth"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit GutterWidth {
			get { return ((MenuStyle)ControlStyle).GutterWidth; }
			set {
				((MenuStyle)ControlStyle).GutterWidth = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseGutterImageSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), ""),
		Obsolete("Use the ImageSpacing and TextIndent properties instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Unit GutterImageSpacing {
			get { return Unit.Empty; }
			set {  }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseTextIndent"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit TextIndent {
			get { return ((MenuStyle)ControlStyle).TextIndent; }
			set { ((MenuStyle)ControlStyle).TextIndent = value; }
		}
		static object itemLinkModeContentBounds = ItemLinkMode.ContentBounds;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemLinkMode"),
#endif
		Category("Behavior"), DefaultValue(ItemLinkMode.ContentBounds), AutoFormatEnable]
		public ItemLinkMode ItemLinkMode {
			get { return (ItemLinkMode)GetEnumProperty("ItemLinkMode", itemLinkModeContentBounds); }
			set {
				SetEnumProperty("ItemLinkMode", ItemLinkMode.ContentBounds, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ItemSpacing {
			get { return ((MenuStyle)ControlStyle).ItemSpacing; }
			set { ((MenuStyle)ControlStyle).ItemSpacing = value; }
		}
		protected bool EnableScrollingInternal {
			get { return GetBoolProperty("EnableScrollingInternal", false); }
			set {
				SetBoolProperty("EnableScrollingInternal", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseMaximumDisplayLevels"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable]
		public int MaximumDisplayLevels {
			get { return GetIntProperty("MaximumDisplayLevels", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaximumDisplayLevels");
				SetIntProperty("MaximumDisplayLevels", 0, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((MenuStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSeparatorBackgroundImage"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage SeparatorBackgroundImage {
			get { return ((MenuStyle)ControlStyle).SeparatorBackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSeparatorColor"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(typeof(Color), ""),
		TypeConverter(typeof(WebColorConverter))]
		public Color SeparatorColor {
			get { return ((MenuStyle)ControlStyle).SeparatorColor; }
			set { ((MenuStyle)ControlStyle).SeparatorColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSeparatorCssClass"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(""), Localizable(false)]
		public string SeparatorCssClass {
			get { return ((MenuStyle)ControlStyle).SeparatorCssClass; }
			set { ((MenuStyle)ControlStyle).SeparatorCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSeparatorHeight"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit SeparatorHeight {
			get { return ((MenuStyle)ControlStyle).SeparatorHeight; }
			set { ((MenuStyle)ControlStyle).SeparatorHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSeparatorPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings SeparatorPaddings {
			get { return ((MenuStyle)ControlStyle).SeparatorPaddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSeparatorWidth"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit SeparatorWidth {
			get { return ((MenuStyle)ControlStyle).SeparatorWidth; }
			set { ((MenuStyle)ControlStyle).SeparatorWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseBorderBetweenItemAndSubMenu"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(BorderBetweenItemAndSubMenuMode.ShowAll)]
		public BorderBetweenItemAndSubMenuMode BorderBetweenItemAndSubMenu {
			get { return (BorderBetweenItemAndSubMenuMode)GetEnumProperty("BorderBetweenItemAndSubMenu", BorderBetweenItemAndSubMenuMode.ShowAll); }
			set {
				SetEnumProperty("BorderBetweenItemAndSubMenu", BorderBetweenItemAndSubMenuMode.ShowAll, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseShowPopOutImages"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowPopOutImages {
			get { return GetDefaultBooleanProperty("ShowPopOutImages", DefaultBoolean.Default); }
			set {
				SetDefaultBooleanProperty("ShowPopOutImages", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseShowSubMenuShadow"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(true)]
		public bool ShowSubMenuShadow {
			get { return GetBoolProperty("ShowSubMenuShadow", true); }
			set {
				SetBoolProperty("ShowSubMenuShadow", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseTarget"),
#endif
		DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public MenuItemCollection Items {
			get { return RootItem.Items; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual MenuItem RootItem {
			get {
				if(RootItemInternal == null)
					RootItemInternal = new MenuItem(this);
				return RootItemInternal;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MenuItem SelectedItem {
			get { return SelectedItemInternal; }
			set {
				if(value != null && value.Menu != this)
					return;
				if(SelectedItemInternal != value) {
					if(SelectedItemInternal != null)
						SelectedItemInternal.SetSelected(false);
					SelectedItemInternal = value;
					if(SelectedItemInternal != null)
						SelectedItemInternal.SetSelected(true);
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseOpacity"),
#endif
		Category("Appearance"), AutoFormatEnable, DefaultValue(AppearanceStyleBase.DefaultOpacity)]
		public int Opacity {
			get { return ((MenuStyle)ControlStyle).Opacity; }
			set { ((MenuStyle)ControlStyle).Opacity = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSpriteImageUrl"),
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseSpriteCssFilePath"),
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemImageProperties ItemImage {
			get { return Images.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSubMenuItemImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemImageProperties SubMenuItemImage {
			get { return Images.SubMenuItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseHorizontalPopOutImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties HorizontalPopOutImage {
			get { return Images.HorizontalPopOut; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseVerticalPopOutImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties VerticalPopOutImage {
			get { return Images.VerticalPopOut; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseVerticalPopOutRtlImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties VerticalPopOutRtlImage {
			get { return Images.VerticalPopOutRtl; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseScrollUpButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuScrollButtonImageProperties ScrollUpButtonImage {
			get { return Images.ScrollUpButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseScrollDownButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuScrollButtonImageProperties ScrollDownButtonImage {
			get { return Images.ScrollDownButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemSubMenuOffset"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemSubMenuOffset ItemSubMenuOffset {
			get {
				if(fItemSubMenuOffset == null)
					fItemSubMenuOffset = new ItemSubMenuOffset();
				return fItemSubMenuOffset;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseRootItemSubMenuOffset"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemSubMenuOffset RootItemSubMenuOffset {
			get {
				if(fRootItemSubMenuOffset == null)
					fRootItemSubMenuOffset = new ItemSubMenuOffset();
				return fRootItemSubMenuOffset;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuItemStyle ItemStyle {
			get { return Styles.Item; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseScrollButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuScrollButtonStyle ScrollButtonStyle {
			get { return Styles.ScrollButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSubMenuItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuItemStyle SubMenuItemStyle {
			get { return Styles.SubMenuItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseSubMenuStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public MenuStyle SubMenuStyle {
			get { return Styles.SubMenu; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate ItemTemplate {
			get { return fItemTemplate; }
			set {
				fItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate RootItemTemplate {
			get { return fRootItemTemplate; }
			set {
				fRootItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate RootItemTextTemplate {
			get { return fRootItemTextTemplate; }
			set {
				fRootItemTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate ItemTextTemplate {
			get { return fItemTextTemplate; }
			set {
				fItemTextTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DevExpress.Web.MenuItemTemplateContainer))]
		public virtual ITemplate SubMenuTemplate {
			get { return fSubMenuTemplate; }
			set {
				fSubMenuTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseNavigateUrlField"),
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseTextField"),
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseTextFormatString"),
#endif
		Category("Data"), DefaultValue("{0}"), Localizable(true), AutoFormatEnable]
		public string TextFormatString {
			get { return GetStringProperty("TextFormatString", "{0}"); }
			set { SetStringProperty("TextFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseToolTipField"),
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
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", ""); }
			set {
				SetStringProperty("ImageUrlField", "", value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseNameField"),
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
	DevExpressWebLocalizedDescription("ASPxMenuBaseCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemClick"),
#endif
		Category("Action")]
		public event MenuItemEventHandler ItemClick
		{
			add { Events.AddHandler(EventItemClick, value); }
			remove { Events.RemoveHandler(EventItemClick, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemCommand"),
#endif
		Category("Action")]
		public event MenuItemCommandEventHandler ItemCommand
		{
			add { Events.AddHandler(EventItemCommand, value); }
			remove { Events.RemoveHandler(EventItemCommand, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuBaseItemDataBound"),
#endif
		Category("Data")]
		public event MenuItemEventHandler ItemDataBound
		{
			add { Events.AddHandler(EventItemDataBound, value); }
			remove { Events.RemoveHandler(EventItemDataBound, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxMenuBaseIsContentCallback")]
#endif
		public static bool IsContentCallback {
			get {
				string param = HttpUtils.GetRequest().Params[RenderUtils.CallbackControlParamParamName];
				if(!string.IsNullOrEmpty(param))
					return param.Contains("DXMENUCONTENT");
				return false;
			}
		}
		protected Dictionary<string, List<MenuItem>> ItemCheckedGroups {
			get { return itemCheckedGroups; }
		}
		protected MenuImages Images {
			get { return (MenuImages)ImagesInternal; }
		}
		protected internal MenuImages RenderImages {
			get { return (MenuImages)RenderImagesInternal; }
		}
		protected MenuStyles Styles {
			get { return StylesInternal as MenuStyles; }
		}
		protected internal MenuStyles RenderStyles {
			get { return (MenuStyles)RenderStylesInternal; }
		}
		protected internal virtual MenuItem RenderRootItem {
			get { return RootItem; }
		}
		protected internal RenderHelper RenderHelper {
			get {
				if(renderHelper == null)
					renderHelper = new RenderHelper(this);
				return renderHelper;
			}
		}
		public ASPxMenuBase()
			: this(null) {
		}
		protected ASPxMenuBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		public bool IsChildSelected(MenuItem item) {
			if(SelectParentItem && (!IsItemSelectEnabled() || AutoPostBack)) {
				MenuItem selectedItem = SelectedItem;
				while(selectedItem != null) {
					if(selectedItem == item)
						return true;
					else
						selectedItem = selectedItem.Parent;
				}
			}
			return false;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if(e is MenuItemCommandEventArgs) {
				OnItemCommand(e as MenuItemCommandEventArgs);
				return true;
			}
			return false;
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			FillItemCheckedGroups();
			ValidateCheckedState();
			ValidateSelectedItem();
		}
		public bool HasVisibleItems() {
			return Items.GetVisibleItemCount() > 0;
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[EventItemClick] != null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MenuClientSideEvents();
		}
		protected internal override bool IsCallBacksEnabled() {
			return base.IsCallBacksEnabled() && (Page == null || !Page.IsCallback || IsCallback) && !IsOneLevelMenuCore();
		}
		protected bool IsHotTrackEnabled() {
			return EnableHotTrack && (ItemLinkMode == ItemLinkMode.ContentBounds);
		}
		protected bool IsItemSelectEnabled() {
			return AllowSelectItem && (ItemLinkMode == ItemLinkMode.ContentBounds);
		}
		protected bool IsItemCheckEnabled() {
			return !AllowSelectItem && (ItemLinkMode == ItemLinkMode.ContentBounds) && HasCheckedGroups();
		}
		protected virtual bool IsOneLevelMenu() {
			if(ClientSideEvents.IsEmpty() && ItemLinkMode != ItemLinkMode.ContentBounds) 
				return IsOneLevelMenuCore();
			return false;
		}
		protected bool IsOneLevelMenuCore() {
			if(MaximumDisplayLevels == 1)
				return true;
			else {
				for(int i = 0; i < Items.GetVisibleItemCount(); i++) {
					MenuItem item = Items.GetVisibleItem(i);
					if(item.Items.GetVisibleItemCount() > 0)
						return false;
				}
				return true;
			}
		}
		protected int GetItemsMaxDepth() {
			return GetItemsMaxDepth(RootItem) - 1;
		}
		protected int GetItemsMaxDepth(MenuItem item) {
			int childrenMaxDepth = 0;
			for(int i = 0; i < item.Items.Count; i++) {
				int childMaxDepth = GetItemsMaxDepth(item.Items[i]);
				if(childrenMaxDepth < childMaxDepth)
					childrenMaxDepth = childMaxDepth;
			}
			return childrenMaxDepth + 1;
		}
		protected internal bool CanScrollSubMenu(MenuItem item) {
			if(IsMainMenu(item) || !IsVertical(item) || GetMenuTemplate(item) != null)
				return false;
			if(EnableScrollingInternal)
				return true;
			return item.EnableScrolling;
		}
		protected internal bool CanItemHotTrack(MenuItem item) {
			bool hasSubMenu = item.HasChildren || GetMenuTemplate(item) != null || IsAdaptivityMenu(item);
			return !IsOneLevelMenu() && GetItemTemplate(item) == null && (!Browser.Platform.IsWebKitTouchUI || hasSubMenu);
		}
		protected internal bool CanItemSelect(MenuItem item) {
			return IsItemSelectEnabled() && GetItemTemplate(item) == null &&
				(!GetItemImageProperties(item).IsEmptySelected ||
					!item.IsRootItem && !GetItemSelectedStyle(item.Parent, item).IsEmpty);
		}
		protected internal bool CanItemCheck(MenuItem item) {
			return IsItemCheckEnabled() && IsCheckableItem(item) && GetItemTemplate(item) == null &&
				(!GetItemImageProperties(item).IsEmptyChecked ||
					!item.IsRootItem && !GetItemCheckedStyle(item.Parent, item).IsEmpty);
		}
		protected internal bool GetItemEnabled(MenuItem item) {
			return IsEnabled() && (item == null || GetItemEnabledCore(item));
		}
		protected internal bool GetItemEnabledCore(MenuItem item) {
			return item.Enabled && (item.IsRootItem || GetItemEnabledCore(item.Parent));
		}
		protected internal bool IsCurrentItem(MenuItem item) {
			return item.Selected && (!IsItemSelectEnabled() || DesignMode) && GetItemTemplate(item) == null;
		}
		protected internal bool IsCheckedItem(MenuItem item) {
			return item.Checked && (!IsCheckableItem(item) || DesignMode) && GetItemTemplate(item) == null;
		}
		protected internal bool IsCheckableItem(MenuItem item) {
			return !string.IsNullOrEmpty(item.GroupName);
		}
		protected internal override void PerformDataBinding(string dataHelperName) {
			if(!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				Items.Clear();
			else if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindItem(RootItem);
				ResetControlHierarchy();
			}
		}
		private void DataBindItem(MenuItem item) {
			HierarchicalDataSourceView view = GetData(item.DataPath);
			if(view != null) {
				IHierarchicalEnumerable enumerable = view.Select();
				item.Items.Clear();
				if(enumerable != null) {
					DataBindItemRecursive(item, enumerable);
				}
			}
		}
		private void DataBindItemRecursive(MenuItem item, IHierarchicalEnumerable enumerable) {
			MenuDataFields dataFields = new MenuDataFields();
			dataFields.navigateUrlFieldName = String.IsNullOrEmpty(NavigateUrlField) ? "NavigateUrl" : NavigateUrlField;
			dataFields.textFieldName = String.IsNullOrEmpty(TextField) ? "Text" : TextField;
			dataFields.toolTipFieldName = String.IsNullOrEmpty(ToolTipField) ? "ToolTip" : ToolTipField;
			dataFields.imageUrlFieldName = String.IsNullOrEmpty(ImageUrlField) ? "ImageUrl" : ImageUrlField;
			dataFields.nameFieldName = String.IsNullOrEmpty(NameField) ? "Name" : NameField;
			foreach(object obj in enumerable) {
				IHierarchyData data = enumerable.GetHierarchyData(obj);
				MenuItem childItem = new MenuItem();
				DataBindItemProperties(childItem, obj, dataFields);
				item.Items.Add(childItem);
				childItem.SetDataPath(data.Path);
				childItem.SetDataItem(obj);
				OnItemDataBound(new MenuItemEventArgs(childItem));
				if(data.HasChildren)
					DataBindItemRecursive(childItem, data.GetChildren());
			}
		}
		private void DataBindItemProperties(MenuItem item, object itemObject, MenuDataFields dataFields) {
			if(itemObject is SiteMapNode) {
				SiteMapNode siteMapNode = itemObject as SiteMapNode;
				item.NavigateUrl = siteMapNode.Url;
				item.Text = siteMapNode.Title;
				item.ToolTip = siteMapNode.Description;
				if(siteMapNode["BeginGroup"] != null)
					item.BeginGroup = bool.Parse(siteMapNode["BeginGroup"]);
				if(siteMapNode["Enabled"] != null)
					item.Enabled = bool.Parse(siteMapNode["Enabled"]);
				if(siteMapNode[dataFields.imageUrlFieldName] != null)
					item.Image.Url = siteMapNode[dataFields.imageUrlFieldName];
				if(siteMapNode[dataFields.nameFieldName] != null)
					item.Name = siteMapNode[dataFields.nameFieldName];
				if(siteMapNode[dataFields.navigateUrlFieldName] != null)
					item.NavigateUrl = siteMapNode[dataFields.navigateUrlFieldName];
				if(siteMapNode["Selected"] != null)
					item.Selected = bool.Parse(siteMapNode["Selected"]);
				if(siteMapNode["Target"] != null)
					item.Target = siteMapNode["Target"];
				if(siteMapNode["VisibleIndex"] != null)
					item.VisibleIndex = int.Parse(siteMapNode["VisibleIndex"]);
				if(siteMapNode["GroupName"] != null)
					item.GroupName = siteMapNode["GroupName"];
				if(siteMapNode["Checked"] != null)
					item.Checked = bool.Parse(siteMapNode["Checked"]);
			} else {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(itemObject);
				if(props == null)
					return;
				DataUtils.GetPropertyValue<bool>(itemObject, "BeginGroup", props, value => { item.BeginGroup = value; });
				DataUtils.GetPropertyValue<bool>(itemObject, "Enabled", props, value => { item.Enabled = value; });
				DataUtils.GetPropertyValue<string>(itemObject, dataFields.nameFieldName, props, value => { item.Name = value; });
				DataUtils.GetPropertyValue<string>(itemObject, dataFields.imageUrlFieldName, props, value => { item.Image.Url = value; });
				DataUtils.GetPropertyValue<string>(itemObject, dataFields.navigateUrlFieldName, props, value => { item.NavigateUrl = value; });
				DataUtils.GetPropertyValue<bool>(itemObject, "Selected", props, value => { item.Selected = value; });
				DataUtils.GetPropertyValue<string>(itemObject, "Target", props, value => { item.Target = value; });
				if(!DataUtils.GetPropertyValue<string>(itemObject, dataFields.textFieldName, props, value => { item.Text = value; }))
					item.Text = itemObject.ToString();
				DataUtils.GetPropertyValue<string>(itemObject, dataFields.toolTipFieldName, props, value => { item.ToolTip = value; });
				DataUtils.GetPropertyValue<string>(itemObject, "GroupName", props, value => { item.GroupName = value; });
				DataUtils.GetPropertyValue<int>(itemObject, "VisibleIndex", props, value => { item.VisibleIndex = value; });
				DataUtils.GetPropertyValue<bool>(itemObject, "Checked", props, value => { item.Checked = value; });
			}
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		public override bool IsClientSideAPIEnabled() {
			if(IsFocused())
				return true;
			if(IsClientSideAPIEnabled(RootItem))
				return true;
			return base.IsClientSideAPIEnabled();
		}
		protected bool IsClientSideAPIEnabled(MenuItem item) {
			for(int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
				MenuItem childItem = item.Items.GetVisibleItem(i);
				if(!childItem.ClientEnabled || !childItem.ClientVisible) 
					return true;
				if(IsClientSideAPIEnabled(childItem))
					return true;
			}
			return false;
		}
		protected override bool HasClientInitialization() {
			return !IsOneLevelMenu();
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || !IsOneLevelMenu();
		}
		protected string GetCheckedState() {
			return GetCheckedState(RootItem);
		}
		protected string GetCheckedState(MenuItem item) {
			string result = string.Empty;
			for(int i = 0; i < item.Items.Count; i++) {
				if(item.Items[i].Checked)
					result += GetItemIndexPath(item.Items[i]) + ";";
				result += GetCheckedState(item.Items[i]);
			}
			return result;
		}
		protected void ClearCheckedState() {
			ClearCheckedState(RootItem);
		}
		protected void ClearCheckedState(MenuItem item) {
			for(int i = 0; i < item.Items.Count; i++) {
				if(item.Items[i].Checked)
					item.Items[i].Checked = false;
				ClearCheckedState(item.Items[i]);
			}
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterPopupUtilsScripts();
			RegisterScrollUtilsScripts();
			RegisterIncludeScript(typeof(ASPxMenuBase), MenuScriptResourceName);
		}
		protected void GetRenderData(MenuItem parent, Dictionary<string, object> allData) {
			if(parent.Items.GetVisibleItemCount() == 0)
				return;
			string menuIndex = parent.IsRootItem ? "" : GetItemIndexPath(parent);
			List<object> data = new List<object>();
			allData.Add(menuIndex, data);
			for(int i = 0; i < parent.Items.GetVisibleItemCount(); i++) {
				MenuItem item = parent.Items.GetVisibleItem(i);
				if(IsClickableItem(item))
					data.Add(new int[] { item.Index });
				else
					data.Add(item.Index);
				GetRenderData(item, allData);
			}
		}
		protected string GetRenderData() {
			Dictionary<string, object> data = new Dictionary<string, object>();
			GetRenderData(RenderRootItem, data);
			return HtmlConvertor.ToJSON(data, false, false, true);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(NeedCreateItemsOnClientSide())
				stb.Append(localVarName + string.Format(".sampleMenuHTML=\"{0}\";\n", SampleMenuHTML));
			else
				stb.Append(localVarName + string.Format(".renderData={0};\n", GetRenderData()));
			if(NeedRenderIFrameBehindPopupElement())
				stb.Append(localVarName + ".createIFrames=true;\n");
			if(AppearAfter != DefaultAppearAfter)
				stb.Append(localVarName + ".appearAfter=" + AppearAfter.ToString() + ";\n");
			if(DisappearAfter != DefaultDisappearAfter)
				stb.Append(localVarName + ".disappearAfter=" + DisappearAfter.ToString() + ";\n");
			if(ItemSubMenuOffset.FirstItemX != 0)
				stb.Append(localVarName + ".subMenuFIXOffset=" + ItemSubMenuOffset.FirstItemX.ToString() + ";\n");
			if(!EnableAnimation)
				stb.Append(localVarName + ".enableAnimation=false;\n");
			if(ItemSubMenuOffset.FirstItemY != 0)
				stb.Append(localVarName + ".subMenuFIYOffset=" + ItemSubMenuOffset.FirstItemY.ToString() + ";\n");
			if(ItemSubMenuOffset.LastItemX != 0)
				stb.Append(localVarName + ".subMenuLIXOffset=" + ItemSubMenuOffset.LastItemX.ToString() + ";\n");
			if(ItemSubMenuOffset.LastItemY != 0)
				stb.Append(localVarName + ".subMenuLIYOffset=" + ItemSubMenuOffset.LastItemY.ToString() + ";\n");
			if(ItemSubMenuOffset.X != 0)
				stb.Append(localVarName + ".subMenuXOffset=" + ItemSubMenuOffset.X.ToString() + ";\n");
			if(ItemSubMenuOffset.Y != 0)
				stb.Append(localVarName + ".subMenuYOffset=" + ItemSubMenuOffset.Y.ToString() + ";\n");
			if(RootItemSubMenuOffset.FirstItemX != 0)
				stb.Append(localVarName + ".rootSubMenuFIXOffset=" + RootItemSubMenuOffset.FirstItemX.ToString() + ";\n");
			if(RootItemSubMenuOffset.FirstItemY != 0)
				stb.Append(localVarName + ".rootSubMenuFIYOffset=" + RootItemSubMenuOffset.FirstItemY.ToString() + ";\n");
			if(RootItemSubMenuOffset.LastItemX != 0)
				stb.Append(localVarName + ".rootSubMenuLIXOffset=" + RootItemSubMenuOffset.LastItemX.ToString() + ";\n");
			if(RootItemSubMenuOffset.LastItemY != 0)
				stb.Append(localVarName + ".rootSubMenuLIYOffset=" + RootItemSubMenuOffset.LastItemY.ToString() + ";\n");
			if(RootItemSubMenuOffset.X != 0)
				stb.Append(localVarName + ".rootSubMenuXOffset=" + RootItemSubMenuOffset.X.ToString() + ";\n");
			if(RootItemSubMenuOffset.Y != 0)
				stb.Append(localVarName + ".rootSubMenuYOffset=" + RootItemSubMenuOffset.Y.ToString() + ";\n");
			if(IsItemSelectEnabled()) {
				stb.Append(localVarName + ".allowSelectItem=true;\n");
				stb.AppendFormat(localVarName + ".selectedItemIndexPath='{0}';\n", (SelectedItem != null) ? GetItemIndexPath(SelectedItem) : "");
			}
			CreateScrollInfoScript(stb, localVarName);
			if(IsItemCheckEnabled()) {
				stb.Append(localVarName + ".allowCheckItems=true;\n");
				stb.AppendFormat(localVarName + ".checkedState='{0}';\n", GetCheckedState());
				CreateItemCheckedGroupsScript(stb, localVarName);
			}
			if(IsClientSideAPIEnabled())
				stb.Append(GetClientItemsScript(localVarName));
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && Events[EventItemClick] != null)
				eventNames.Add("ItemClick");
		}
		protected string GetClientItemsScript(string localVarName) {
			string script = HtmlConvertor.ToJSON(GetClientItemsScriptObject(RenderRootItem.Items));
			return localVarName + ".CreateItems(" + script + ");\n";
		}
		protected internal object GetClientItemsScriptObject(MenuItemCollection items) {
			Hashtable[] itemsArray = new Hashtable[items.Count];
			for(int i = 0; i < items.Count; i++) {
				Hashtable itemsHashTable = new Hashtable();
				MenuItem item = items[i];
				if(!string.IsNullOrEmpty(item.Name))
					itemsHashTable.Add("name", item.Name);
				if(item.BeginGroup)
					itemsHashTable.Add("beginGroup", true);
				if(!item.ClientEnabled)
					itemsHashTable.Add("clientEnabled", false);
				if(!item.Enabled)
					itemsHashTable.Add("enabled", false);
				if(!item.ClientVisible)
					itemsHashTable.Add("clientVisible", false);
				if(!item.Visible)
					itemsHashTable.Add("visible", false);
				if(item.Items.Count > 0)
					itemsHashTable.Add("items", GetClientItemsScriptObject(item.Items));
				itemsArray[i] = itemsHashTable;
			}
			return itemsArray;
		}
		protected void CreateItemCheckedGroupsScript(StringBuilder stb, string localVarName) {
			List<object> groupsArray = new List<object>();
			foreach(string groupName in ItemCheckedGroups.Keys) {
				List<string> indexPathes = new List<string>();
				for(int i = 0; i < ItemCheckedGroups[groupName].Count; i++)
					indexPathes.Add(GetItemIndexPath(ItemCheckedGroups[groupName][i]));
				groupsArray.Add(indexPathes);
			}
			stb.Append(localVarName + ".itemCheckedGroups=" + HtmlConvertor.ToJSON(groupsArray) + ";\n");
		}
		protected void CreateScrollInfoScript(StringBuilder stb, string localVarName) {
			List<string> scrollInfo = new List<string>();
			CreateItemScrollInfo(scrollInfo, RenderRootItem);
			if(scrollInfo.Count > 0)
				stb.Append(localVarName + ".scrollInfo=" + HtmlConvertor.ToJSON(scrollInfo) + ";\n");
		}
		protected void CreateItemScrollInfo(List<string> scrollInfo, MenuItem item) {
			if(CanScrollSubMenu(item))
				scrollInfo.Add(GetItemIndexPath(item));
			for(int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
				MenuItem childItem = item.Items.GetVisibleItem(i);
				CreateItemScrollInfo(scrollInfo, childItem);
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientMenuBase";
		}
		protected bool ShouldAddItemStateItems(MenuItem item) {
			return GetItemEnabled(item) && item.HasVisibleChildren && GetMenuTemplate(item) == null && CanItemSubMenuRender(item);
		}
		protected AppearanceStyleBase[] GetItemStyles(MenuItem item, AppearanceStyleBase style, AppearanceStyleBase dropDownButtonStyle) {
			AppearanceStyleBase containerStyle = new AppearanceStyleBase();
			AppearanceStyleBase contentStyle = new AppearanceStyleBase();
			if(style != null) {
				containerStyle.CopyFrom(style);
				containerStyle.Paddings.Reset();
				contentStyle.Paddings.Assign(style.Paddings);
			}
			return dropDownButtonStyle != null
				? new AppearanceStyleBase[] { containerStyle, dropDownButtonStyle, contentStyle }
				: new AppearanceStyleBase[] { containerStyle, contentStyle };
		}
		protected string[] GetItemIdPostfixes(MenuItem item) {
			return IsDropDownItemStyle(item) || item.HasPopOutImageCell
				? new string[] { "", "P", "T" }
				: new string[] { "", "T" };
		}
		protected override bool CanHoverScript(){
			return true;
		}
		protected override bool HasHoverScripts() {
			return !IsOneLevelMenu();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			AddItemHoverItems(helper, RenderRootItem);
		}
		protected void AddItemHoverItems(StateScriptRenderHelper helper, MenuItem item) {
			if((item.HasVisibleChildren || GetMenuTemplate(item) != null || IsAdaptivityMenu(item)) && CanItemSubMenuRender(item) && CanItemHotTrack(item)) {
				bool enabled = GetItemEnabled(item);
				if(!IsMainMenu(item)) 
					 helper.AddStyle(null, GetMenuMainElementID(item), enabled);	   
				if(!item.IsRootItem)
					helper.AddStyle(null, GetMenuBorderCorrectorElementID(item), enabled);   
				foreach(MenuItem childItem in item.Items.GetVisibleItems()) {
					if(CanItemHotTrack(childItem)) {
						AppearanceStyleBase style = null, dropDownButtonStyle = null;
						if(IsHotTrackEnabled() && !IsCurrentItem(childItem)) {
							style = GetItemHoverCssStyle(item, childItem);
							if(IsDropDownItemStyle(childItem) || HasLightweightPopOutImageCell(childItem))
								dropDownButtonStyle = GetItemDropDownButtonHoverCssStyle(item, childItem);
						}
						helper.AddStyles(
							GetItemStyles(childItem, style, dropDownButtonStyle),
							GetItemElementID(childItem),
							GetItemIdPostfixes(childItem),
							GetItemHottrackedImages(childItem),
							ItemImageIdPostfixes,
							GetItemEnabled(childItem)
						);
						AddItemHoverItems(helper, childItem);
					}
				}
			}
			if(CanScrollSubMenu(item)) {
				helper.AddStyles(
					new AppearanceStyleBase[] { GetScrollUpButtonHoverCssStyle(item) },
					GetScrollUpButtonID(item),
					new string[0],
					GetScrollUpButtonHottrackedImages(item),
					ScrollButtonImageIdPostfixes,
					GetItemEnabled(item)
				);
				helper.AddStyles(
					new AppearanceStyleBase[] { GetScrollDownButtonHoverCssStyle(item) },
					GetScrollDownButtonID(item),
					new string[0],
					GetScrollDownButtonHottrackedImages(item),
					ScrollButtonImageIdPostfixes,
					GetItemEnabled(item)
				);
			}
		}
		protected override bool HasSelectedScripts() {
			return IsItemCheckEnabled() || IsItemSelectEnabled();
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			if(IsItemCheckEnabled())
				AddItemCheckedItems(helper, RenderRootItem);
			if(IsItemSelectEnabled())
				AddItemSelectedItems(helper, RenderRootItem);
		}
		protected void AddItemCheckedItems(StateScriptRenderHelper helper, MenuItem item) {
			if(!ShouldAddItemStateItems(item)) return;
			for (int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
				MenuItem childItem = item.Items.GetVisibleItem(i);
				if (CanItemCheck(childItem)) {
					AppearanceStyleBase dropDownButtonStyle = IsDropDownItemStyle(childItem) || HasLightweightPopOutImageCell(childItem)
						? GetItemDropDownButtonCheckedCssStyle(item, childItem) : null;
					helper.AddStyles(
						GetItemStyles(childItem, GetItemCheckedCssStyle(item, childItem), dropDownButtonStyle),
						GetItemElementID(childItem),
						GetItemIdPostfixes(childItem),
						GetItemCheckedImages(childItem),
						ItemImageIdPostfixes,
						GetItemEnabled(childItem)
					);
				}
				AddItemCheckedItems(helper, childItem);
			}
		}
		protected void AddItemSelectedItems(StateScriptRenderHelper helper, MenuItem item) {
			if(!ShouldAddItemStateItems(item)) return;
			for(int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
				MenuItem childItem = item.Items.GetVisibleItem(i);
				if (CanItemSelect(childItem)) {
					AppearanceStyleBase dropDownButtonStyle = IsDropDownItemStyle(childItem) || HasLightweightPopOutImageCell(childItem) ? 
						GetItemDropDownButtonSelectedCssStyle(item, childItem) : null;
					helper.AddStyles(
						GetItemStyles(childItem, GetItemSelectedCssStyle(item, childItem), dropDownButtonStyle),
						GetItemElementID(childItem),
						GetItemIdPostfixes(childItem),
						GetItemSelectedImages(childItem),
						ItemImageIdPostfixes,
						GetItemEnabled(childItem)
					);
				}
				AddItemSelectedItems(helper, childItem);
			}
		}
		protected override bool HasPressedScripts() {
			return !IsOneLevelMenu();
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			AddItemPressedItems(helper, RenderRootItem);
		}
		protected void AddItemPressedItems(StateScriptRenderHelper helper, MenuItem item) {
			if(!ShouldAddItemStateItems(item)) return;
			for(int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
				MenuItem childItem = item.Items.GetVisibleItem(i);
				AddItemPressedItems(helper, childItem);
			}
			if(CanScrollSubMenu(item)) {
				helper.AddStyles(new AppearanceStyleBase[] { GetScrollButtonPressedCssStyle(item) }, GetScrollUpButtonID(item),
					new string[0], GetScrollUpButtonPressedImages(item), ScrollButtonImageIdPostfixes, GetItemEnabled(item));
				helper.AddStyles(new AppearanceStyleBase[] { GetScrollButtonPressedCssStyle(item) }, GetScrollDownButtonID(item),
					new string[0], GetScrollDownButtonPressedImages(item), ScrollButtonImageIdPostfixes, GetItemEnabled(item));
			}
		}
		protected override bool HasDisabledScripts() {
			return IsEnabled() && IsClientSideAPIEnabled();
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			AddItemDisabledItems(helper, RenderRootItem);
		}
		protected void AddItemDisabledItems(StateScriptRenderHelper helper, MenuItem item) {
			if(!ShouldAddItemStateItems(item)) return;
			for(int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
				MenuItem childItem = item.Items.GetVisibleItem(i);
				DisabledStyle style = GetItemDisabledCssStyle(item, childItem);
				object[] images = GetItemDisabledImages(childItem);
				helper.AddStyle(style, GetItemElementID(childItem), GetItemIdPostfixes(childItem), images, ItemImageIdPostfixes, GetItemEnabled(childItem));
				AddItemDisabledItems(helper, childItem);
			}
		}
		protected override bool HasContent() {
			return NeedCreateItemsOnClientSide() || HasVisibleItems();
		}
		protected internal virtual bool IsMainMenu(MenuItem item) {
			return (item != null) && item.IsRootItem;
		}
		protected internal virtual bool IsAdaptivityMenu(MenuItem item) {
			return item is AdaptiveMenuItem;
		}
		protected internal virtual bool IsAutoWidthMode(MenuItem item) {
			return false;
		}
		protected internal virtual bool IsWrapPrevented(MenuItem item) {
			return false;
		}
		protected internal virtual ImagePosition GetItemImagePosition(MenuItem item) {
			return ImagePosition.Left;
		}
		protected internal virtual Orientation GetOrientation(MenuItem item) {
			return Orientation.Vertical;
		}
		protected internal virtual bool IsShowAsToolbar() {
			return false;
		}
		protected internal bool IsVertical(MenuItem item) {
			return GetOrientation(item) == Orientation.Vertical;
		}
		protected internal bool IsDropDownItemStyle(MenuItem item) {
			if(IsDropDownMode(item))
				return true;
			if(!item.IsRootItem && IsVertical(item.Parent)) { 
				foreach(MenuItem childItem in item.Parent){
					if(IsDropDownMode(childItem))
						return true;
				}
			}
			return false;
		}
		protected internal bool IsDropDownMode(MenuItem item) {
			return item.DropDownMode && ItemLinkMode == ItemLinkMode.ContentBounds;
		}
		protected internal bool HasLightweightPopOutImageCell(MenuItem item) {
			return item.HasPopOutImageCell; 
		}
		protected internal bool CanItemSubMenuRender(MenuItem item) {
			return (MaximumDisplayLevels == 0 || item.Depth + 1 < MaximumDisplayLevels);
		}
		protected internal bool IsPopupMenuControlVisible(WebControl menuControl) {
			return DesignMode;
		}
		protected override IDictionary GetDesignModeState() {
			int view = 0;
			object obj = ((IControlDesignerAccessor)this).UserData["ViewType"];
			if(obj != null)
				view = (int)obj;
			if(view == 1) {
				if(SubMenuControl != null) {
					SubMenuControl.Visible = true;
					MenuControl.Visible = false;
				} else
					MenuControl.Visible = true;
			}
			return base.GetDesignModeState();
		}
		protected override void ClearControlFields() {
			RootControl = null;
			MenuControl = null;
			SubMenuControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			RootControl = DesignMode ? CreateRootControlDesignMode() : CreateRootControl();
			RootControl.CssClass = RenderStyles.GetRootContainerCssClass();
			if(!DesignMode && NeedCreateItemsOnClientSide())
				CreateSampleMenuAndSerialize();
			MenuControl = CreateItemMenuControl(RenderRootItem);
			base.CreateControlHierarchy();
		}
		private void CreateSampleMenuAndSerialize() {
			CreateItemMenuControl(CreateSampleItems());
			SerializeMenuTemplates();
			RemoveSampleItems();
		}
		protected internal virtual bool NeedCreateItemsOnClientSide() {
			return false;
		}
		protected internal MenuItem CreateSampleItemsCore(MenuItem rootItem) {
			MenuItem item = new MenuItem("SAMPLE_ITEM");
			item.Name = "SAMPLE_ITEM_NAME";
			MenuItem item2 = new MenuItem("ITEM_WITH_SUBMENU");
			item2.Name = "ITEM_WITH_SUBMENU_NAME";
			item2.EnableScrolling = true;
			MenuItem subItem = new MenuItem("SUBITEM");
			subItem.Name = "SUBITEM_NAME";
			item2.Items.Add(subItem);
			MenuItem item3 = new MenuItem("ITEM_WITH_IMAGE");
			item3.Name = "ITEM_WITH_IMAGE_NAME";
			item3.Image.Url = "";
			item3.Image.SpriteProperties.CssClass = "SAMPLE_CSS_CLASS";
			MenuItem item4 = new MenuItem();
			item4.BeginGroup = true;
			rootItem.Items.Add(item);
			rootItem.Items.Add(item2);
			rootItem.Items.Add(item3);
			rootItem.Items.Add(item4);
			return rootItem;
		}
		protected internal virtual MenuItem CreateSampleItems() {
			return null;
		}
		private void SerializeMenuTemplates() {
			SampleMenuHTML = ClearRenderForSamples(RenderUtils.GetRenderResult(RootControl));
		}
		private void RemoveSampleItems() {
			RootControl.Controls.Clear();
		}
		private string ClearRenderForSamples(string render) {
			return render.Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "").Replace("\t", "");
		}
		protected WebControl CreateRootControlDesignMode() {
			InternalTable table = RenderUtils.CreateTable();
			table.Width = Width;
			Controls.Add(table);
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return cell;
		}
		protected WebControl CreateRootControl() {
			WebControl rootControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(rootControl);
			return rootControl;
		}
		protected override void AddRightToLeftAttributes() {
		}
		protected internal override bool NeedRenderIFrameBehindPopupElement() {
			return base.NeedRenderIFrameBehindPopupElement() && !IsOneLevelMenu();
		}
		protected internal override void ResetControlHierarchy() {
			base.ResetControlHierarchy();
		}
		protected WebControl CreateItemMenuControl(MenuItem item) {
			if(!NeedCreateItemsOnClientSide())
				if((!item.HasVisibleChildren && GetMenuTemplate(item) == null && !IsAdaptivityMenu(item)) || !CanItemSubMenuRender(item))
					return null;
			WebControl menuControl = null;
			if(IsMainMenu(item)) {
				menuControl = CreateMainMenuControl(item);
				RootControl.Controls.Add(menuControl);
				RootControl.Controls.Add(RenderUtils.CreateClearElement());
			} else {
				menuControl = CreatePopupMenuControl(item);
				RootControl.Controls.Add(menuControl);
				if(DesignMode)
					menuControl.Visible = false;
			}
			if(GetMenuTemplate(item) == null) {
				for(int i = 0; i < item.Items.GetVisibleItemCount(); i++) {
					MenuItem child = item.Items.GetVisibleItem(i);
					WebControl subMenuControl = CreateItemMenuControl(child);
					if(child.Depth == 0 && SubMenuControl == null)
						SubMenuControl = subMenuControl;
				}
			}
			return menuControl;
		}
		protected virtual WebControl CreateMainMenuControl(MenuItem item) {
			return (WebControl)new MainMenuLite(item);
		}
		protected virtual WebControl CreatePopupMenuControl(MenuItem item) {
			return (WebControl)new PopupMenuLite(item);
		}
		protected internal int GetMenuZIndex(MenuItem item) {
			return RenderUtils.MenuZIndex + item.Depth * 2 + (IsAdaptivityMenu(item) ? 0 : 2);
		}
		protected internal int GetMenuIFrameZIndex() {
			return RenderUtils.MenuZIndex - 2 - 1; 
		}
		protected internal int GetMenuBorderCorrectorZIndex(MenuItem item) {
			return GetMenuZIndex(item) + 1;
		}
		protected internal string GetItemIndexPath(MenuItem item) {
			return item != null ? item.IndexPath : "";
		}
		protected internal MenuItem GetItemByIndexPath(string path) {
			if(string.IsNullOrEmpty(path))
				return null;
			string[] indexes = path.Split(RenderUtils.IndexSeparator);
			MenuItem item = RenderRootItem;
			for(int i = 0; i < indexes.Length; i++) {
				int index = int.Parse(indexes[i]);
				if(0 <= index && index < item.Items.Count)
					item = item.Items[index];
				else {
					item = null;
					break;
				}
			}
			return item;
		}
		protected internal string GetMenuElementID(MenuItem item) {
			return "DXM" + GetItemIndexPath(item) + "_";
		}
		protected internal string GetMenuMainElementID(MenuItem item) {
			return "DXME" + GetItemIndexPath(item) + "_";
		}
		protected internal string GetMenuIFrameElementID(int level) {
			return "DXMIF" + level.ToString();
		}
		protected internal string GetMenuBorderCorrectorElementID(MenuItem item) {
			return "DXMBC" + GetItemIndexPath(item) + "_";
		}
		protected internal string GetScrollButtonID(MenuItem item) {
			return "DXSB" + GetItemIndexPath(item) + "_";
		}
		protected internal string GetScrollUpButtonID(MenuItem item) {
			return GetScrollButtonID(item) + ScrollButtonIdPostfixes[0];
		}
		protected internal string GetScrollDownButtonID(MenuItem item) {
			return GetScrollButtonID(item) + ScrollButtonIdPostfixes[1];
		}
		protected internal string GetScrollAreaID(MenuItem item) {
			return "DXSA" + GetItemIndexPath(item);
		}
		protected internal string GetScrollUpButtonImageID(MenuItem item) {
			return GetScrollUpButtonID(item) + ScrollButtonImageIdPostfixes[0];
		}
		protected internal string GetScrollDownButtonImageID(MenuItem item) {
			return GetScrollDownButtonID(item) + ScrollButtonImageIdPostfixes[0];
		}
		protected internal string GetItemElementID(MenuItem item) {
			return "DXI" + GetItemIndexPath(item) + "_";
		}
		protected internal string GetItemIndentElementID(MenuItem item) {
			return GetItemElementID(item) + "II";
		}
		protected internal string GetItemSeparatorElementID(MenuItem item) {
			return GetItemElementID(item) + "IS";
		}
		protected internal string GetItemTemplateCellID(MenuItem item) {
			return GetItemElementID(item) + "ITC";
		}
		protected internal string GetItemImageCellID(MenuItem item) {
			return GetItemElementID(item) + ItemIdPostfixes[0];
		}
		protected internal string GetItemIndentCellID(MenuItem item) {
			return GetItemElementID(item) + ItemIdPostfixes[1];
		}
		protected internal string GetItemTextCellID(MenuItem item) {
			return GetItemElementID(item) + ItemIdPostfixes[2];
		}
		protected internal string GetItemPopOutImageCellID(MenuItem item) {
			return GetItemElementID(item) + ItemIdPostfixes[3];
		}
		protected internal string GetItemImageID(MenuItem item) {
			return GetItemElementID(item) + ItemImageIdPostfixes[0];
		}
		protected internal string GetItemPopOutImageID(MenuItem item) {
			return GetItemElementID(item) + ItemImageIdPostfixes[1];
		}
		protected internal string GetItemTextTemplateContainerID(MenuItem item) {
			return "ITTCNT" + GetItemIndexPath(item);
		}
		protected internal string GetItemTemplateContainerID(MenuItem item) {
			return "ITCNT" + GetItemIndexPath(item);
		}
		protected internal string GetMenuTemplateContainerID(MenuItem item) {
			return "MTCNT" + GetItemIndexPath(item);
		}
		protected internal bool HasItemCellIDs(MenuItem item) {
			return (CanItemHotTrack(item) || CanItemSelect(item) || CanItemCheck(item) || IsClientSideAPIEnabled()) && GetItemEnabled(item);
		}
		protected bool HasItemServerClick(MenuItem item) {
			return (AutoPostBack || Events[EventItemClick] != null) && !IsClientSideEventsAssigned() && (item.NavigateUrl == "");
		}
		protected internal string GetItemElementOnClick(MenuItem item) {
			if(HasItemServerClick(item))
				return RenderUtils.GetPostBackEventReference(Page, this, "CLICK:" + GetItemIndexPath(item));
			else
				return string.Format(ItemClickHandlerName, ClientID, GetItemIndexPath(item));
		}
		protected internal string GetItemDropDownElementOnClick(MenuItem item) {
			return string.Format(ItemDropDownClickHandlerName, ClientID, GetItemIndexPath(item));
		}
		protected virtual bool IsClickableItem(MenuItem item) {
			return !IsCurrentItem(item) && GetItemEnabled(item) &&
				((ClientSideEvents.ItemClick != "") || (Events[EventItemClick] != null) || AutoPostBack ||
				  (item.NavigateUrl != "" && ItemLinkMode == ItemLinkMode.ContentBounds) ||
				  IsItemSelectEnabled() || IsItemCheckEnabled() ||
				  ((AppearAfter > 0 || Browser.Platform.IsTouchUI) && item.HasVisibleChildren && !IsOneLevelMenu()));
		}
		protected internal bool HasItemElementOnClick(MenuItem item) {
			return IsClickableItem(item) && (ItemLinkMode == ItemLinkMode.ContentBounds);
		}
		protected internal bool HasItemImageCellOnClick(MenuItem item) {
			return HasItemImageLinkOnClickCore(item) && !IsItemNavigateUrl(item);
		}
		protected internal bool HasItemImageLinkOnClick(MenuItem item) {
			return HasItemImageLinkOnClickCore(item) && IsItemNavigateUrl(item);
		}
		protected internal bool HasItemImageLinkOnClickCore(MenuItem item) {
			return ItemLinkMode == ItemLinkMode.TextAndImage && IsClickableItem(item);
		}
		protected internal bool HasItemTextCellOnClick(MenuItem item) {
			return HasItemTextLinkOnClickCore(item) && string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal bool HasItemTextLinkOnClick(MenuItem item) {
			return HasItemTextLinkOnClickCore(item) && !string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal bool HasItemTextLinkOnClickCore(MenuItem item) {
			return IsClickableItem(item) && (ItemLinkMode != ItemLinkMode.ContentBounds);
		}
		protected internal bool HasParentImageCellInternal(MenuItem parent) {
			for(int i = 0; i < parent.Items.GetVisibleItemCount(); i++)
				if(!GetItemImageProperties(parent.Items.GetVisibleItem(i)).IsEmpty)
					return true;
			return false;
		}
		protected internal bool HasImageCell(MenuItem item) {
			return item == null
				? false
				: item.HasImageCell;
		}
		protected internal bool HasPopOutImageCell(MenuItem item) {
			if(item != null && !item.IsRootItem)
				return item.HasPopOutImageCell;
			return false;
		}
		protected internal bool HasPopOutImage(MenuItem item) {
			if(item != null && !item.IsRootItem) {
				if(IsAdaptivityMenu(item))
					return true;
				else if(!CanItemSubMenuRender(item))
					return false;
				else {
					if(item.HasVisibleChildren) {
						if(IsDropDownMode(item))
							return true;
						if(IsVertical(item.Parent))
							return ShowPopOutImages != DefaultBoolean.False;
						else
							return ShowPopOutImages == DefaultBoolean.True;
					}
				}
			}
			return false;
		}
		protected internal bool IsItemNavigateUrl(MenuItem item) {
			return !string.IsNullOrEmpty(GetItemNavigateUrl(item));
		}
		protected internal string GetItemNavigateUrl(MenuItem item) {
			string url = string.Empty;
			if(!IsCurrentItem(item))
				url = String.Format(NavigateUrlFormatString, item.NavigateUrl);
			if(string.IsNullOrEmpty(url) && IsAccessibilityCompliantRender(true))
				url = RenderUtils.AccessibilityEmptyUrl;
			return url;
		}
		protected internal string GetItemText(MenuItem item) {
			string text = string.Format(TextFormatString, item.Text);
			if(IsShowAsToolbar() && IsMainMenu(item.Parent) && text == MenuItem.DefaultText)
				return string.Empty;
			return text;
		}
		protected internal virtual string GetItemAdaptiveText(MenuItem item) {
			return string.Empty;
		}
		protected internal string GetItemTarget(MenuItem item) {
			return (item.Target != "") ? item.Target : Target;
		}
		protected internal string GetItemImageToolTip(MenuItem item) {
			return item.ToolTip;
		}
		protected internal string GetItemLinkToolTip(MenuItem item) {
			return item.ToolTip;
		}
		protected internal string GetItemCellToolTip(MenuItem item) {
			return (ItemLinkMode == ItemLinkMode.ContentBounds) ? item.ToolTip : "";
		}
		protected internal bool HasAutoSeparators(MenuItem parentItem) {
			return (AutoSeparators == AutoSeparatorMode.All) ||
				(AutoSeparators == AutoSeparatorMode.RootOnly && IsMainMenu(parentItem));
		}
		protected internal bool ShowItemSeparator(MenuItem parentItem, MenuItem item) {
			return HasAutoSeparators(parentItem) || IsItemBeginsGroup(parentItem, item);
		}
		protected internal bool IsNextItemBeginsGroup(MenuItem parentItem, MenuItem item) {
			if(item.VisibleIndex < parentItem.Items.GetVisibleItemCount() - 1)
				return ShowItemSeparator(parentItem, parentItem.Items.GetVisibleItem(item.VisibleIndex + 1));
			return HasAutoSeparators(parentItem);
		}
		bool IsItemBeginsGroup(MenuItem parentItem, MenuItem item) {
			if(item.BeginGroup)
				return true;
			for(int i = item.Index - 1; i >= 0; i--) {
				MenuItem previousItem = parentItem.Items[i];
				if(previousItem.Visible)
					return false;
				if(previousItem.BeginGroup)
					return true;
			}
			return false;
		}
		protected override ImagesBase CreateImages() {
			return new MenuImages(this);
		}
		ImagePropertiesCache<MenuItem, ItemImagePropertiesBase> itemImageProperties;
		protected internal ItemImagePropertiesBase GetItemImageProperties(MenuItem item_) {
			if(itemImageProperties == null)
				itemImageProperties = new ImagePropertiesCache<MenuItem, ItemImagePropertiesBase>(delegate(CacheKey<MenuItem> key) {
					MenuItem item = key.Item;
					MenuItemImageProperties ret = new MenuItemImageProperties();
					ret.MergeWith(item.Image);
					ret.MergeWith(GetItemParentImageProperties(item.Parent));
					ret.MergeWith(GetDefaultItemImageProperties(item));
					if(IsCurrentItem(item) || IsChildSelected(item)) {
						if(!string.IsNullOrEmpty(ret.UrlSelected))
							ret.Url = ret.UrlSelected;
						if(!ret.SpriteProperties.SelectedLeft.IsEmpty)
							ret.SpriteProperties.Left = ret.SpriteProperties.SelectedLeft;
						if(!ret.SpriteProperties.SelectedTop.IsEmpty)
							ret.SpriteProperties.Top = ret.SpriteProperties.SelectedTop;
					}
					if(IsCheckedItem(item)) {
						if(!string.IsNullOrEmpty(ret.UrlChecked))
							ret.Url = ret.UrlChecked;
						if(!ret.SpriteProperties.CheckedLeft.IsEmpty)
							ret.SpriteProperties.Left = ret.SpriteProperties.CheckedLeft;
						if(!ret.SpriteProperties.CheckedTop.IsEmpty)
							ret.SpriteProperties.Top = ret.SpriteProperties.CheckedTop;
					}
					return ret;
				});
			return GetItemImage(itemImageProperties, item_);
		}
		ImagePropertiesCache<MenuItem, ItemImagePropertiesBase> itemParentImageProperties;
		static object itemParentImagePropertiesKey = new object();
		protected internal ItemImagePropertiesBase GetItemParentImageProperties(MenuItem parentItem_) {
			if(itemParentImageProperties == null) {
				itemParentImageProperties = new ImagePropertiesCache<MenuItem, ItemImagePropertiesBase>(delegate(CacheKey<MenuItem> key) {
					MenuItemImageProperties ret = new MenuItemImageProperties();
					MenuItem parentItem = key.Item;
					MenuItem menuItem = parentItem;
					while(menuItem != null) {
						ret.MergeWith(menuItem.SubMenuItemImage);
						menuItem = menuItem.Parent;
					}
					if(parentItem != null && !parentItem.IsRootItem)
						ret.MergeWith(RenderImages.SubMenuItem);
					else
						ret.MergeWith(RenderImages.Item);
					return ret;
				});
			}
			return GetItemImage(itemParentImageProperties, parentItem_);
		}
		ImagePropertiesCache<object, MenuItemImageProperties> itemDefaultImageProperties;
		static object itemDefaultImagePropertiesKey = new object();
		protected internal MenuItemImageProperties GetDefaultItemImageProperties(MenuItem item) {
			if(itemDefaultImageProperties == null) {
				itemDefaultImageProperties = new ImagePropertiesCache<object, MenuItemImageProperties>(delegate(CacheKey<object> key) {
					MenuItemImageProperties ret = new MenuItemImageProperties();
					ret.CopyFrom(RenderImages.GetImageProperties(Page, MenuImages.SubMenuItemImageName));
					if(key.ExtraOption == 1) {
						if(!string.IsNullOrEmpty(ret.UrlChecked))
							ret.Url = ret.UrlChecked;
						if(!string.IsNullOrEmpty(ret.SpriteProperties.CheckedCssClass))
							ret.SpriteProperties.CssClass = ret.SpriteProperties.CheckedCssClass;
						if(!ret.SpriteProperties.CheckedLeft.IsEmpty)
							ret.SpriteProperties.Left = ret.SpriteProperties.CheckedLeft;
						if(!ret.SpriteProperties.CheckedTop.IsEmpty)
							ret.SpriteProperties.Top = ret.SpriteProperties.CheckedTop;
					}
					else if(key.ExtraOption == 2) {
						ret.Url = string.Empty;
						ret.SpriteProperties.CssClass = string.Empty;
						ret.SpriteProperties.Left = Unit.Empty;
						ret.SpriteProperties.Top = Unit.Empty;
						ret.UrlChecked = string.Empty;
						ret.SpriteProperties.CheckedCssClass = string.Empty;
						ret.SpriteProperties.CheckedLeft = Unit.Empty;
						ret.SpriteProperties.CheckedTop = Unit.Empty;
					}
					return ret;
				});
			}
			MenuItem parentItem = item.Parent;
			if(parentItem == null)
				return null;
			else if(IsCheckedItem(item) && !IsMainMenu(parentItem))
				return GetItemImage(itemDefaultImageProperties, itemDefaultImagePropertiesKey, 1);
			else if(!IsCheckableItem(item) || IsMainMenu(parentItem))
				return GetItemImage(itemDefaultImageProperties, itemDefaultImagePropertiesKey, 2);
			return GetItemImage(itemDefaultImageProperties, itemDefaultImagePropertiesKey, 0);
		}
		ImagePropertiesCache<MenuItem, ItemImageProperties> horizontalPopOutImageProperties;
		protected internal ItemImageProperties GetHorizontalPopOutImageProperties(MenuItem item_) {
			if(horizontalPopOutImageProperties == null)
				horizontalPopOutImageProperties = new ImagePropertiesCache<MenuItem, ItemImageProperties>(delegate(CacheKey<MenuItem> key) {
					MenuItem item = key.Item;
					ItemImageProperties ret = new ItemImageProperties();
					ret.MergeWith(item.PopOutImage);
					MenuItem menuItem = item.Parent;
					while(menuItem != null) {
						ret.MergeWith(menuItem.SubMenuPopOutImage);
						menuItem = menuItem.Parent;
					}
					ret.MergeWith(RenderImages.GetImageProperties(Page, MenuImages.HorizontalPopOutImageName));
					if(IsCurrentItem(item) || IsChildSelected(item)){
						 if(!string.IsNullOrEmpty(ret.UrlSelected))
							ret.Url = ret.UrlSelected;
						if(!ret.SpriteProperties.SelectedLeft.IsEmpty)
							ret.SpriteProperties.Left = ret.SpriteProperties.SelectedLeft;
						if(!ret.SpriteProperties.SelectedTop.IsEmpty)
							 ret.SpriteProperties.Top = ret.SpriteProperties.SelectedTop;
					}
					return ret;
				});
			return GetItemImage(horizontalPopOutImageProperties, item_);
		}
		ImagePropertiesCache<MenuItem, ItemImageProperties> verticalPopOutImageProperties;
		protected internal ItemImageProperties GetVerticalPopOutImageProperties(MenuItem item_) {
			if(verticalPopOutImageProperties == null)
				verticalPopOutImageProperties = new ImagePropertiesCache<MenuItem, ItemImageProperties>(delegate(CacheKey<MenuItem> key) {
					MenuItem item = key.Item;
					ItemImageProperties ret = new ItemImageProperties();
					ret.MergeWith(item.PopOutImage);
					MenuItem menuItem = item.Parent;
					while(menuItem != null) {
						ret.MergeWith(menuItem.SubMenuPopOutImage);
						menuItem = menuItem.Parent;
					}
					ret.MergeWith(RenderImages.GetImageProperties(Page,  
						IsRightToLeft() ? MenuImages.VerticalPopOutRtlImageName : MenuImages.VerticalPopOutImageName));
					if(IsCurrentItem(item) || IsChildSelected(item)){
						 if(!string.IsNullOrEmpty(ret.UrlSelected))
							ret.Url = ret.UrlSelected;
						if(!ret.SpriteProperties.SelectedLeft.IsEmpty)
							ret.SpriteProperties.Left = ret.SpriteProperties.SelectedLeft;
						if(!ret.SpriteProperties.SelectedTop.IsEmpty)
							ret.SpriteProperties.Top = ret.SpriteProperties.SelectedTop;
					}
					return ret;
				});
			return GetItemImage(verticalPopOutImageProperties, item_);
		}
		ImagePropertiesCache<MenuItem, ItemImageProperties> adaptiveMenuImageProperties;
		protected internal ItemImageProperties GetAdaptiveMenuImageProperties(MenuItem item_) {
			if(adaptiveMenuImageProperties == null)
				adaptiveMenuImageProperties = new ImagePropertiesCache<MenuItem, ItemImageProperties>(delegate(CacheKey<MenuItem> key) {
					ItemImageProperties ret = new ItemImageProperties();
					ret.MergeWith(RenderImages.GetImageProperties(Page, MenuImages.AdaptiveMenuImageName));
					return ret;
				});
			return GetItemImage(adaptiveMenuImageProperties, item_);
		}
		protected internal ItemImageProperties GetPopOutImageProperties(MenuItem item) {
			if(IsAdaptivityMenu(item))
				return GetAdaptiveMenuImageProperties(item);
			if(!item.IsRootItem && GetOrientation(item.Parent) == Orientation.Horizontal)
				return GetHorizontalPopOutImageProperties(item);
			return GetVerticalPopOutImageProperties(item);
		}
		ImagePropertiesCache<MenuItem, MenuScrollButtonImageProperties> scrollUpButtonImageProperties;
		protected internal MenuScrollButtonImageProperties GetScrollUpButtonImageProperties(MenuItem item_) {
			if(scrollUpButtonImageProperties == null)
				scrollUpButtonImageProperties = new ImagePropertiesCache<MenuItem, MenuScrollButtonImageProperties>(delegate(CacheKey<MenuItem> key) {
					MenuItem item = key.Item;
					MenuScrollButtonImageProperties ret = new MenuScrollButtonImageProperties();
					MenuItem menuItem = item;
					while(menuItem != null) {
						ret.MergeWith(menuItem.ScrollUpButtonImage);
						menuItem = menuItem.Parent;
					}
					ret.MergeWith(RenderImages.GetImageProperties(Page, MenuImages.ScrollUpButtonImageName));
					return ret;
				});
			return GetItemImage(scrollUpButtonImageProperties, item_);
		}
		ImagePropertiesCache<MenuItem, MenuScrollButtonImageProperties> scrollDownButtonImageProperties;
		protected internal MenuScrollButtonImageProperties GetScrollDownButtonImageProperties(MenuItem item_) {
			if(scrollDownButtonImageProperties == null)
				scrollDownButtonImageProperties = new ImagePropertiesCache<MenuItem, MenuScrollButtonImageProperties>(delegate(CacheKey<MenuItem> key) {
					MenuItem item = key.Item;
					MenuScrollButtonImageProperties ret = new MenuScrollButtonImageProperties();
					MenuItem menuItem = item;
					while(menuItem != null) {
						ret.MergeWith(menuItem.ScrollDownButtonImage);
						menuItem = menuItem.Parent;
					}
					ret.MergeWith(RenderImages.GetImageProperties(Page, MenuImages.ScrollDownButtonImageName));
					return ret;
				});
			return GetItemImage(scrollDownButtonImageProperties, item_);
		}
		protected object[] GetItemHottrackedImages(MenuItem item) {
			if(HasPopOutImageCell(item))
				return new object[] { GetItemImageProperties(item).GetHottrackedScriptObject(Page), GetPopOutImageProperties(item).GetHottrackedScriptObject(Page) };
			return new object[] { GetItemImageProperties(item).GetHottrackedScriptObject(Page) };
		}
		protected object[] GetItemSelectedImages(MenuItem item) {
			if(HasPopOutImageCell(item))
				return new object[] { GetItemImageProperties(item).GetSelectedScriptObject(Page), GetPopOutImageProperties(item).GetSelectedScriptObject(Page) };
			return new object[] { GetItemImageProperties(item).GetSelectedScriptObject(Page) };
		}
		protected object[] GetItemDisabledImages(MenuItem item) {
			if(HasPopOutImageCell(item))
				return new object[] { GetItemImageProperties(item).GetDisabledScriptObject(Page), GetPopOutImageProperties(item).GetDisabledScriptObject(Page) };
			return new object[] { GetItemImageProperties(item).GetDisabledScriptObject(Page) };
		}
		protected object[] GetItemCheckedImages(MenuItem item) {
			var itemImageProperties = GetItemImageProperties(item);
			if(HasPopOutImageCell(item))
				return new object[] { GetCheckedScriptObject(itemImageProperties), GetCheckedScriptObject(GetPopOutImageProperties(item)) };
			return new object[] { GetCheckedScriptObject(itemImageProperties) };
		}		
		protected object[] GetScrollUpButtonHottrackedImages(MenuItem item) {
			return new object[] { GetScrollUpButtonImageProperties(item).GetHottrackedScriptObject(Page) };
		}
		protected object[] GetScrollDownButtonHottrackedImages(MenuItem item) {
			return new object[] { GetScrollDownButtonImageProperties(item).GetHottrackedScriptObject(Page) };
		}
		protected object[] GetScrollUpButtonPressedImages(MenuItem item) {
			return new object[] { GetScrollUpButtonImageProperties(item).GetPressedScriptObject(Page) };
		}
		protected object[] GetScrollDownButtonPressedImages(MenuItem item) {
			return new object[] { GetScrollDownButtonImageProperties(item).GetPressedScriptObject(Page) };
		}
		protected object[] GetScrollUpButtonDisabledImages(MenuItem item) {
			return new object[] { GetScrollUpButtonImageProperties(item).GetDisabledScriptObject(Page) };
		}
		protected override Style CreateControlStyle() {
			return new MenuStyle();
		}
		protected override StylesBase CreateStyles() {
			return new MenuStyles(this);
		}
		internal string GetToolbarModeCssClassName() {
			return GetCssClassNamePrefix("tb");
		}
		protected override void RegisterLinkStyles() {
			RegisterItemLinkStyles(RootItem);
		}
		protected void RegisterItemLinkStyles(MenuItem item) {
			if(CanItemSubMenuRender(item) && item.Items.GetVisibleItemCount() > 0) {
				string menuID = item.IsRootItem ? ClientID : ClientID + "_" + GetMenuElementID(item);
				RegisterLinkHoverStyle(LinkStyle.HoverStyle, menuID);
				RegisterLinkVisitedStyle(LinkStyle.VisitedStyle, menuID);
				for(int i = 0; i < item.Items.GetVisibleItemCount(); i++)
					RegisterItemLinkStyles(item.Items.GetVisibleItem(i));
			}
		}
		static object customMenuStyle = new object();
		protected MenuStyle GetCustomMenuStyle(MenuItem parentItem) {
			return (MenuStyle)CreateStyle(delegate() {
				MenuStyle style = new MenuStyle();
				if(parentItem != null && (IsMainMenu(parentItem) || parentItem.IsRootItem)) {
					MergeParentSkinOwnerControlStyle(style);
					style.CopyFrom(RenderStyles.Style);
					style.CopyFrom(ControlStyle);
				} else {
					MergeParentSkinOwnerControlStyle(style);
					MenuItem menuItem = parentItem;
					while(menuItem != null) {
						style.MergeWith(menuItem.SubMenuStyle);
						menuItem = menuItem.Parent;
					}
					style.MergeWith(RenderStyles.SubMenu);
					style.MergeFontWith(ControlStyle);
					style.MergeFontWith(RenderStyles.Style);
				}
				return style;
			}, parentItem, customMenuStyle);
		}
		protected MenuStyle GetDefaultMenuStyle(MenuItem parentItem) {
			return parentItem != null && IsMainMenu(parentItem)
				? Styles.GetDefaultMainMenuStyle(IsVertical(parentItem), IsAutoWidthMode(parentItem), IsWrapPrevented(parentItem))
				: Styles.GetDefaultMenuStyle();
		}
		static object menuStyleKey = new object();
		protected internal MenuStyle GetMenuStyle(MenuItem parentItem) {
			return (MenuStyle)CreateStyle(delegate() {
				MenuStyle style = new MenuStyle();
				style.CopyFrom(GetDefaultMenuStyle(parentItem));
				style.CopyFrom(GetCustomMenuStyle(parentItem));
				MergeDisableStyle(style, IsEnabled(), GetDisabledStyle(), MainOwnerControl != null);
				return style;
			}, parentItem, menuStyleKey);
		}
		protected internal AppearanceStyleBase GetMenuBorderCorrectorStyle(MenuItem parentItem) {
			return GetMenuBorderCorrectorStyle(parentItem, true);
		}
		protected internal AppearanceStyleBase GetMenuBorderCorrectorStyle(MenuItem parentItem, bool mergeWithMenuCssClass) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.BackColor = GetMenuStyle(parentItem).BackColor;
			if(mergeWithMenuCssClass)
				style.CssClass = GetMenuStyle(parentItem).CssClass;
			style.CopyFrom(Styles.GetDefaultMenuBorderCorrectorStyle());
			return style;
		}
		static object menuSeparatorStyleKey = new object();
		protected internal AppearanceStyleBase GetMenuSeparatorStyle(MenuItem parentItem) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultMainMenuSeparatorStyle());
			style.CopyFrom(GetCustomMenuStyle(parentItem).SeparatorStyle);
			return style;
			}, parentItem, menuSeparatorStyleKey, false);
		}
		static object menuGutterStyleKey = new object();
		protected internal AppearanceStyleBase GetMenuGutterStyle(MenuItem parentItem) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFrom(Styles.GetDefaultMainMenuGutterStyle());
				style.CopyFrom(GetCustomMenuStyle(parentItem).GutterStyle);
				return style;
			}, parentItem, menuGutterStyleKey);
		}
		protected MenuItemStyle GetCustomItemStyleMenuStylePart(MenuItem parentItem) {
			MenuItemStyle style = new MenuItemStyle();
			MenuStyle menuStyle = GetCustomMenuStyle(parentItem);
			style.MergeFontWith(menuStyle);
			if(menuStyle.Cursor != "")
				style.Cursor = menuStyle.Cursor;
			if(menuStyle.Wrap != DefaultBoolean.Default)
				style.Wrap = menuStyle.Wrap;
			return style;
		}
		static object customItemStyleKey = new object();
		protected MenuItemStyle GetCustomItemStyle(MenuItem parentItem) {
			return (MenuItemStyle)CreateStyle(delegate() {
				MenuItemStyle style = new MenuItemStyle();
				MenuItemStyle menuStyle = GetCustomItemStyleMenuStylePart(parentItem);
				if(parentItem == null || IsMainMenu(parentItem) || parentItem.IsRootItem) {
					style.CopyFrom(menuStyle);
					style.CopyFrom(RenderStyles.Item);
				}
				else {
					MenuItem menuItem = parentItem;
					while(menuItem != null) {
						style.MergeWith(menuItem.SubMenuItemStyle);
						menuItem = menuItem.Parent;
					}
					style.MergeWith(RenderStyles.SubMenuItem);
					style.MergeWith(menuStyle);
				}
				return style;
			}, parentItem, customItemStyleKey);
		}
		protected AppearanceStyleBase GetCustomItemDropDownButtonStyle(MenuItem parentItem) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			if(parentItem == null || IsMainMenu(parentItem) || parentItem.IsRootItem)
				style.CopyFrom(RenderStyles.Item.DropDownButtonStyle);
			else {
				MenuItem menuItem = parentItem;
				while(menuItem != null) {
					style.MergeWith(menuItem.SubMenuItemStyle.DropDownButtonStyle);
					menuItem = menuItem.Parent;
				}
				style.MergeWith(RenderStyles.SubMenuItem.DropDownButtonStyle);
			}
			return style;
		}
		static object defaultItemStyleKey = new object();
		protected MenuItemStyle GetDefaultItemStyle(MenuItem parentItem, bool hasImage, bool hasPopOutImage) {
			return (MenuItemStyle)CreateStyle(delegate() {
				return Styles.GetDefaultMainMenuItemStyle();
			}, parentItem, GetBoolParam(hasImage), GetBoolParam(hasPopOutImage), defaultItemStyleKey);
		}
		static object itemParentInternalStyleKey = new object();
		protected MenuItemStyle GetItemParentStyleInternal(MenuItem parentItem, bool hasImage, bool hasPopOutImage) {
			return (MenuItemStyle)CreateStyle(delegate() {
				MenuItemStyle style = new MenuItemStyle();
				style.CopyFrom(GetDefaultItemStyle(parentItem, hasImage, hasPopOutImage));
				style.CopyFrom(GetCustomItemStyle(parentItem));
				return style;
			}, parentItem, GetBoolParam(hasImage), GetBoolParam(hasPopOutImage), itemParentInternalStyleKey);
		}
		static object itemInternalStyleKey = new object();
		protected MenuItemStyle GetItemStyleInternal(MenuItem item) {
			return (MenuItemStyle)CreateStyle(delegate() {
				if(item != null) {
					MenuItemStyle result = new MenuItemStyle();
					result.CopyFrom(GetItemParentStyleInternal(item.Parent, HasImageCell(item), HasPopOutImageCell(item)));
					result.CopyFrom(item.ItemStyle);
					return result;
				}
				return GetItemParentStyleInternal(null, false, false);
			}, item, itemInternalStyleKey);
		}
		static object itemStyleKey = new object();
		protected internal MenuItemStyle GetItemStyle(MenuItem item) {
			return (MenuItemStyle)CreateStyle(delegate() {
				MenuItemStyle itemStyle = GetItemStyleInternal(item);
				AppearanceStyleBase disabledStyle = GetItemDisabledStyle(item);
				bool addSelectedStyle = item != null && (IsCurrentItem(item) || IsChildSelected(item));
				bool addCheckedStyle = item != null && IsCheckedItem(item);
				bool addDisabledStyle = !disabledStyle.IsEmpty;
				if(addSelectedStyle || addCheckedStyle || addDisabledStyle) {
					MenuItemStyle result = new MenuItemStyle();
					result.CopyFrom(itemStyle);
					if(addSelectedStyle)
						result.CopyFrom(GetItemSelectedStyle(item.Parent, item));
					if(addCheckedStyle)
						result.CopyFrom(GetItemCheckedStyle(item.Parent, item));
					result.CopyFrom(disabledStyle);
					return result;
				}
				return itemStyle;
			}, item, itemStyleKey);
		}
		static object itemGutterStyleKey = new object();
		protected internal MenuItemStyle GetItemGutterStyle(MenuItem item) {
			return (MenuItemStyle)CreateStyle(delegate() {
				MenuItem parentItem = item != null ? item.Parent : null;
				MenuItemStyle itemStyle = GetItemStyle(item);
				if(IsVertical(parentItem)) {
					MenuItemStyle result = new MenuItemStyle();
					result.CopyFrom(GetMenuGutterStyle(parentItem));
					result.CopyFrom(itemStyle);
					return result;
				}
				return itemStyle;
			}, item, itemGutterStyleKey);
		}
		static object itemIndentStyleKey = new object();
		protected internal AppearanceStyleBase GetItemIndentStyle(MenuItem item) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				bool addGutter = IsVertical(item.Parent);
				AppearanceStyleBase gutterStyle = addGutter ? GetMenuGutterStyle(item.Parent) : null;
				AppearanceStyleBase disabledStyle = GetItemDisabledStyle(item);
				bool addDisabledStyle = !disabledStyle.IsEmpty;
				if(addGutter && !addDisabledStyle)
					return gutterStyle;
				else if(addGutter) {
					AppearanceStyleBase result = new AppearanceStyleBase();
					if(addGutter)
						result.CopyFrom(gutterStyle);
					result.CopyFrom(disabledStyle);
					return result;
				}
				return disabledStyle;
			}, item, itemIndentStyleKey);
		}
		static object itemDisabledStyleKey = new object();
		protected internal AppearanceStyleBase GetItemDisabledStyle(MenuItem item) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				return GetItemEnabled(item)
					? new AppearanceStyleBase()
					: GetDisabledStyle();
			}, item, itemDisabledStyleKey);
		}
		static object itemLinkStyleKey = new object();
		protected internal AppearanceStyleBase GetItemLinkStyle(MenuItem parentItem, MenuItem item) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFontAndCursorFrom(GetDefaultItemStyle(parentItem, HasImageCell(item), HasPopOutImageCell(item)));
				style.CopyFontAndCursorFrom(GetCustomItemStyle(parentItem));
				style.CopyFontAndCursorFrom(GetItemStyle(item));
				if(!IsCurrentItem(item))
					style.CopyFrom(LinkStyle.Style);
				style.CopyFrom(GetItemDisabledStyle(item));
				return style;
			}, item, itemLinkStyleKey);
		}
		static object itemDropDownButtonStyleKey = new object();
		protected MenuItemStyleBase GetItemDropDownButtonStyleInternal(MenuItem parentItem, MenuItem item) {
			return (MenuItemStyleBase)CreateStyle(delegate() {
				MenuItemStyleBase style = new MenuItemStyleBase();
				style.CopyFrom(GetCustomItemDropDownButtonStyle(parentItem));
				style.CopyFrom(item.ItemStyle.DropDownButtonStyle);
				return style;
			}, parentItem, itemDropDownButtonStyleKey);
		}
		protected internal MenuItemStyleBase GetItemDropDownButtonStyle(MenuItem parentItem, MenuItem item, bool mergeWithItemStyle) {
			MenuItemStyleBase style = new MenuItemStyleBase();
			if(mergeWithItemStyle)
				style.CopyFrom(GetItemStyle(item));
			style.CopyFrom(GetItemDropDownButtonStyleInternal(parentItem, item));
			return style;
		}
		protected internal MenuItemStyleBase GetItemDropDownButtonStyle(MenuItem parentItem, MenuItem item) {
			return GetItemDropDownButtonStyle(parentItem, item, true);
		}
		protected MenuScrollButtonStyle GetCustomScrollButtonStyle(MenuItem parentItem) {
			MenuScrollButtonStyle style = new MenuScrollButtonStyle();
			MenuItem menuItem = parentItem;
			while(menuItem != null) {
				style.MergeWith(menuItem.ScrollButtonStyle);
				menuItem = menuItem.Parent;
			}
			style.MergeWith(RenderStyles.ScrollButton);
			return style;
		}
		static object scrollUpButtonStyleKey = new object();
		protected internal MenuScrollButtonStyle GetScrollUpButtonStyle(MenuItem parentItem) {
			return (MenuScrollButtonStyle)CreateStyle(delegate() {
				MenuScrollButtonStyle style = new MenuScrollButtonStyle();
				style.CopyFrom(Styles.GetDefaultScrollUpButtonStyle());
				style.CopyFrom(GetCustomScrollButtonStyle(parentItem));
				return style;
			}, parentItem, scrollUpButtonStyleKey);
		}
		static object scrollDownButtonStyleKey = new object();
		protected internal MenuScrollButtonStyle GetScrollDownButtonStyle(MenuItem parentItem) {
			return (MenuScrollButtonStyle)CreateStyle(delegate() {
				MenuScrollButtonStyle style = new MenuScrollButtonStyle();
				style.CopyFrom(Styles.GetDefaultScrollDownButtonStyle());
				style.CopyFrom(GetCustomScrollButtonStyle(parentItem));
				return style;
			}, parentItem, scrollDownButtonStyleKey);
		}
		protected internal AppearanceStyleBase GetScrollAreaStyle(MenuItem parentItem) {
			return Styles.GetDefaultScrollAreaStyle();
		}
		protected Paddings GetItemSelectedCssStylePaddings(MenuItem parentItem, MenuItem item, AppearanceStyleBase selectedStyle) {
			return GetItemSelectedCssStylePaddings(GetItemStyleInternal(item), selectedStyle);
		}
		protected Paddings GetItemSelectedCssStylePaddings(MenuItemStyle itemStyle, AppearanceStyleBase selectedStyle) {
			return UnitUtils.GetSelectedCssStylePaddings(itemStyle, selectedStyle, itemStyle.Paddings);
		}
		protected Paddings GetScrollUpButtonSelectedCssStylePaddings(MenuItem parentItem, AppearanceStyleBase selectedStyle) {
			MenuScrollButtonStyle buttonStyle = GetScrollUpButtonStyle(parentItem);
			Paddings paddings = GetScrollUpButtonContentPaddings(parentItem);
			return UnitUtils.GetSelectedCssStylePaddings(buttonStyle, selectedStyle, paddings);
		}
		protected Paddings GetScrollDownButtonSelectedCssStylePaddings(MenuItem parentItem, AppearanceStyleBase selectedStyle) {
			MenuScrollButtonStyle buttonStyle = GetScrollDownButtonStyle(parentItem);
			Paddings paddings = GetScrollDownButtonContentPaddings(parentItem);
			return UnitUtils.GetSelectedCssStylePaddings(buttonStyle, selectedStyle, paddings);
		}
		static object defaultItemHoverStyleKey = new object();
		protected AppearanceStyleBase GetDefaultItemHoverStyle(MenuItem parentItem, bool hasImage, bool hasPopOutImage) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				return Styles.GetDefaultMainMenuItemHoverStyle();
			}, parentItem, GetBoolParam(hasImage), GetBoolParam(hasPopOutImage), defaultItemHoverStyleKey);
		}
		protected internal AppearanceStyle GetItemHoverCssStyle(MenuItem parentItem, MenuItem item) {
			MenuItemStyle itemStyleInternal = GetItemStyleInternal(item);
			AppearanceStyle style = new AppearanceStyle();
			style.CopyBordersFrom(itemStyleInternal);
			style.CopyFrom(GetDefaultItemHoverStyle(parentItem, HasImageCell(item), HasPopOutImageCell(item)));
			style.CopyFrom(itemStyleInternal.HoverStyle);
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(itemStyleInternal, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetItemDropDownButtonHoverStyle(MenuItem parentItem, MenuItem item) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetItemDropDownButtonStyleInternal(parentItem, item));
			style.CopyFrom(GetItemDropDownButtonStyleInternal(parentItem, item).HoverStyle);
			return style;
		}
		protected internal AppearanceStyle GetItemDropDownButtonHoverCssStyle(MenuItem parentItem, MenuItem item) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemDropDownButtonHoverStyle(parentItem, item));
			style.Paddings.CopyFrom(GetItemDropDownButtonContentPaddings(parentItem, item));
			return style;
		}
		protected internal AppearanceSelectedStyle GetScrollUpButtonHoverStyle(MenuItem parentItem) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetScrollUpButtonStyle(parentItem));
			style.CopyFrom(Styles.GetDefaultScrollButtonHoverStyle());
			style.CopyFrom(GetScrollUpButtonStyle(parentItem).HoverStyle);
			return style;
		}
		protected internal AppearanceStyle GetScrollUpButtonHoverCssStyle(MenuItem parentItem) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetScrollUpButtonHoverStyle(parentItem));
			style.Paddings.CopyFrom(GetScrollUpButtonSelectedCssStylePaddings(parentItem, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetScrollDownButtonHoverStyle(MenuItem parentItem) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetScrollDownButtonStyle(parentItem));
			style.CopyFrom(Styles.GetDefaultScrollButtonHoverStyle());
			style.CopyFrom(GetScrollDownButtonStyle(parentItem).HoverStyle);
			return style;
		}
		protected internal AppearanceStyle GetScrollDownButtonHoverCssStyle(MenuItem parentItem) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetScrollDownButtonHoverStyle(parentItem));
			style.Paddings.CopyFrom(GetScrollDownButtonSelectedCssStylePaddings(parentItem, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetItemSelectedStyle(MenuItem parentItem, MenuItem item) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			MenuItemStyle itemStyle = GetItemStyleInternal(item);
			style.CopyBordersFrom(itemStyle);
			style.CopyFrom(Styles.GetDefaultMainMenuItemSelectedStyle());
			style.CopyFrom(itemStyle.SelectedStyle);
			return style;
		}
		protected internal AppearanceStyle GetItemSelectedCssStyle(MenuItem parentItem, MenuItem item) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemSelectedStyle(parentItem, item));
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(parentItem, item, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetItemDropDownButtonSelectedStyle(MenuItem parentItem, MenuItem item) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetItemDropDownButtonStyleInternal(parentItem, item));
			style.CopyFrom(GetItemDropDownButtonStyleInternal(parentItem, item).SelectedStyle);
			return style;
		}
		protected internal AppearanceStyle GetItemDropDownButtonSelectedCssStyle(MenuItem parentItem, MenuItem item) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemDropDownButtonSelectedStyle(parentItem, item));
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(parentItem, item, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetItemCheckedStyle(MenuItem parentItem, MenuItem item) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			MenuItemStyle itemStyle = GetItemStyleInternal(item);
			style.CopyBordersFrom(itemStyle);
			style.CopyFrom(Styles.GetDefaultMainMenuItemCheckedStyle());
			style.CopyFrom(itemStyle.CheckedStyle);
			return style;
		}
		protected internal AppearanceStyle GetItemCheckedCssStyle(MenuItem parentItem, MenuItem item) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemCheckedStyle(parentItem, item));
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(parentItem, item, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetItemDropDownButtonCheckedStyle(MenuItem parentItem, MenuItem item) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetItemDropDownButtonStyleInternal(parentItem, item));
			style.CopyFrom(GetItemDropDownButtonStyleInternal(parentItem, item).CheckedStyle);
			return style;
		}
		protected internal AppearanceStyle GetItemDropDownButtonCheckedCssStyle(MenuItem parentItem, MenuItem item) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetItemDropDownButtonCheckedStyle(parentItem, item));
			style.Paddings.CopyFrom(GetItemSelectedCssStylePaddings(parentItem, item, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetScrollUpButtonPressedStyle(MenuItem parentItem) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetScrollUpButtonStyle(parentItem));
			style.CopyFrom(Styles.GetDefaultScrollButtonPressedStyle());
			style.CopyFrom(GetScrollUpButtonStyle(parentItem).PressedStyle);
			return style;
		}
		protected internal AppearanceStyle GetScrollUpButtonPressedCssStyle(MenuItem parentItem) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetScrollUpButtonPressedStyle(parentItem));
			style.Paddings.CopyFrom(GetScrollUpButtonSelectedCssStylePaddings(parentItem, style));
			return style;
		}
		protected internal AppearanceSelectedStyle GetScrollDownButtonPressedStyle(MenuItem parentItem) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyBordersFrom(GetScrollDownButtonStyle(parentItem));
			style.CopyFrom(Styles.GetDefaultScrollButtonPressedStyle());
			style.CopyFrom(GetScrollDownButtonStyle(parentItem).PressedStyle);
			return style;
		}
		protected internal AppearanceStyle GetScrollButtonPressedCssStyle(MenuItem parentItem) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetScrollDownButtonPressedStyle(parentItem));
			style.Paddings.CopyFrom(GetScrollDownButtonSelectedCssStylePaddings(parentItem, style));
			return style;
		}
		protected internal DisabledStyle GetItemDisabledCssStyle(MenuItem parentItem, MenuItem item) {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			return style;
		}
		protected internal virtual string GetItemTemplateToolTip(MenuItem item) {
			return string.Empty;
		}
		protected virtual AppearanceStyleBase GetItemTemplateStyle(MenuItem item) {
			if(ApplyItemStyleToTemplates) 
				return GetItemStyle(item);
			return new AppearanceStyleBase();
		}
		protected internal AppearanceStyleBase GetItemTemplateStyleInternal(MenuItem item) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(GetDefaultItemStyle(item.Parent, false, false));
			style.CopyFrom(GetItemTemplateStyle(item));
			return style;
		}
		protected virtual Paddings GetItemTemplatePaddings(MenuItem item) {
			if(ApplyItemStyleToTemplates)
				return GetItemContentPaddings(item.Parent, item);
			return new Paddings();
		}
		protected internal Paddings GetItemTemplatePaddingsInternal(MenuItem item) {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(GetItemTemplatePaddings(item));
			return paddings;
		}
		protected internal Unit GetTextIndent(MenuItem parentItem) {
			return GetMenuStyle(parentItem).TextIndent;
		}
		protected internal Unit GetGutterWidth(MenuItem parentItem) {
			return GetMenuStyle(parentItem).GutterWidth;
		}
		protected internal Unit GetSeparatorWidth(MenuItem parentItem) {
			return GetMenuStyle(parentItem).SeparatorWidth;
		}
		protected internal Unit GetSeparatorHeight(MenuItem parentItem) {
			return GetMenuStyle(parentItem).SeparatorHeight;
		}
		protected internal Unit GetItemHeight(MenuItem item) {
			return GetItemHeight(item, false);
		}
		protected internal Unit GetItemHeight(MenuItem item, bool corrected) {
			MenuItemStyle style = GetItemStyleInternal(item);
			return corrected || Browser.IsIE ? UnitUtils.GetCorrectedHeight(style.Height, style, style.Paddings) : style.Height;
		}
		protected internal Unit GetItemWidth(MenuItem parentItem, MenuItem item) {
			return GetItemStyleInternal(item).Width;
		}
		protected internal Unit GetItemImageSpacing(MenuItem item) {
			return GetItemStyleInternal(item).ImageSpacing;
		}
		static object itemImageSpacingStyleKey = new object();
		protected internal AppearanceStyleBase GetItemImageSpacingStyle(MenuItem parentItem) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFrom(Styles.GetMenuItemImageSpacingStyle(IsMainMenu(parentItem), GetItemImagePosition(parentItem), IsVertical(parentItem)));
				return style;
			}, parentItem, itemImageSpacingStyleKey);
		}
		protected internal Unit GetPopOutImageCellSpacing(MenuItem item) {
			MenuItemStyle itemStyle = GetItemStyleInternal(item);
			if(IsShowAsToolbar()) {
				return IsDropDownItemStyle(item)
					? itemStyle.ToolbarDropDownButtonSpacing
					: itemStyle.ToolbarPopOutImageSpacing;
			}
			else {
				return IsDropDownItemStyle(item)
					? itemStyle.DropDownButtonSpacing
					: itemStyle.PopOutImageSpacing;
			}
		}
		static object menuItemSpacingStyleKey = new object();
		protected internal AppearanceStyleBase GetItemSpacingStyle(MenuItem parentItem, bool hasSeparator) {
			return (AppearanceStyleBase)CreateStyle(delegate() {
				AppearanceStyleBase style = new AppearanceStyleBase();
				style.CopyFrom(Styles.GetMenuItemSpacingStyle(IsMainMenu(parentItem), IsLargeItems(parentItem), IsVertical(parentItem), hasSeparator));
				return style;
			}, parentItem, menuItemSpacingStyleKey, hasSeparator);
		}
		protected internal Paddings GetMenuPaddings(MenuItem parentItem) {
			return GetMenuStyle(parentItem).Paddings;
		}
		protected internal Paddings GetItemContentPaddings(MenuItem parentItem, MenuItem item) {
			Paddings paddings = new Paddings();
			MenuItemStyle itemStyle = GetItemStyleInternal(item);
			paddings.CopyFrom(itemStyle.Paddings);
			if(IsCurrentItem(item) || IsChildSelected(item))
				paddings.CopyFrom(GetItemSelectedCssStylePaddings(itemStyle, GetItemSelectedStyle(parentItem, item)));
			if(IsCheckedItem(item))
				paddings.CopyFrom(GetItemSelectedCssStylePaddings(itemStyle, GetItemCheckedStyle(parentItem, item)));
			return paddings;
		}
		protected Paddings GetItemDropDownButtonContentPaddingsIntenal(MenuItem parentItem, MenuItem item) {
			return GetItemDropDownButtonStyleInternal(parentItem, item).Paddings;
		}
		protected internal Paddings GetItemDropDownButtonContentPaddings(MenuItem parentItem, MenuItem item) {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(GetItemDropDownButtonContentPaddingsIntenal(parentItem, item));
			if(IsCurrentItem(item) || IsChildSelected(item))
				paddings.CopyFrom(GetItemSelectedCssStylePaddings(parentItem, item, GetItemDropDownButtonSelectedStyle(parentItem, item)));
			if(IsCheckedItem(item))
				paddings.CopyFrom(GetItemSelectedCssStylePaddings(parentItem, item, GetItemDropDownButtonCheckedStyle(parentItem, item)));
			return paddings;
		}
		protected internal Paddings GetScrollUpButtonContentPaddings(MenuItem parentItem) {
			return GetScrollUpButtonStyle(parentItem).Paddings;
		}
		protected internal Paddings GetScrollDownButtonContentPaddings(MenuItem parentItem) {
			return GetScrollDownButtonStyle(parentItem).Paddings;
		}
		protected internal bool IsLargeItems(MenuItem parentItem) {
			return GetItemImagePosition(parentItem) == ImagePosition.Top || GetItemImagePosition(parentItem) == ImagePosition.Bottom;
		}
		protected internal void AddItemGroup(MenuItem item) {
			if(!ItemCheckedGroups.ContainsKey(item.GroupName))
				ItemCheckedGroups.Add(item.GroupName, new List<MenuItem>());
			ItemCheckedGroups[item.GroupName].Add(item);
		}
		protected internal void RemoveItemGroup(MenuItem item) {
			string groupName = item.GroupName;
			if(ItemCheckedGroups.ContainsKey(groupName)) {
				ItemCheckedGroups[groupName].Remove(item);
				if(ItemCheckedGroups[groupName].Count == 0)
					ItemCheckedGroups.Remove(groupName);
			}
		}
		protected bool HasCheckedGroups() {
			return ItemCheckedGroups.Count > 0;
		}
		protected bool IsSingleCheckedGroupItem(MenuItem item) {
			return string.IsNullOrEmpty(item.GroupName) || !ItemCheckedGroups.ContainsKey(item.GroupName) || ItemCheckedGroups[item.GroupName].Count == 1;
		}
		protected void FillItemCheckedGroups() {
			ItemCheckedGroups.Clear();
			FillItemCheckedGroups(RootItem);
		}
		protected void FillItemCheckedGroups(MenuItem item) {
			for(int i = 0; i < item.Items.Count; i++) {
				if(!string.IsNullOrEmpty(item.Items[i].GroupName))
					AddItemGroup(item.Items[i]);
				FillItemCheckedGroups(item.Items[i]);
			}
		}
		protected internal void ValidateCheckedState() {
			ValidateCheckedState(RootItem);
		}
		protected internal void ValidateCheckedState(MenuItem item) {
			for(int i = 0; i < item.Items.Count; i++) {
				SetCheckedState(item.Items[i]);
				ValidateCheckedState(item.Items[i]);
			}
		}
		protected internal void SetCheckedState(MenuItem item) {
			if(!string.IsNullOrEmpty(item.GroupName) && item.Checked) {
				if(ItemCheckedGroups.ContainsKey(item.GroupName)) {
					List<MenuItem> items = ItemCheckedGroups[item.GroupName];
					for(int i = 0; i < items.Count; i++) {
						if(items[i] == item)
							continue;
						items[i].SetChecked(false);
					}
				}
			}
		}
		protected override void SelectCurrentPath(bool ignoreQueryString) {
			SelectCurrentPathRecursive(RootItem, ignoreQueryString);
		}
		private bool SelectCurrentPathRecursive(MenuItem rootItem, bool ignoreQueryString) {
			bool ret = false;
			for(int i = 0; i < rootItem.Items.Count; i++) {
				MenuItem item = rootItem.Items[i];
				if(UrlUtils.IsCurrentUrl(ResolveUrl(GetItemNavigateUrl(item)), ignoreQueryString)) {
					item.Selected = true;
					ret = true;
				} else if(SelectCurrentPathRecursive(item, ignoreQueryString))
					ret = true;
			}
			return ret;
		}
		protected void ValidateSelectedItem() {
			ValidateSelectedItemRecursive(RootItem);
			if(SelectedItem != null && SelectedItem.Menu != this) 
				SelectedItem = null;
		}
		private void ValidateSelectedItemRecursive(MenuItem item) {
			for(int i = 0; i < item.Items.Count; i++) {
				if(item.Items[i].Selected && SelectedItem != item.Items[i])
					SelectedItem = item.Items[i];
				ValidateSelectedItemRecursive(item.Items[i]);
			}
		}
		protected internal ITemplate GetMenuTemplate(MenuItem item) {
			if(item.IsRootItem)
				return null;
			if(item.SubMenuTemplate != null)
				return item.SubMenuTemplate;
			return SubMenuTemplate;
		}
		protected internal ITemplate GetItemTemplate(MenuItem item) {
			if(item.Template != null)
				return item.Template;
			if((item.Depth == 0) && (RootItemTemplate != null))
				return RootItemTemplate;
			return ItemTemplate;
		}
		protected internal ITemplate GetItemTextTemplate(MenuItem item) {
			if(item.TextTemplate != null)
				return item.TextTemplate;
			if((item.Depth == 0) && (RootItemTextTemplate != null))
				return RootItemTextTemplate;
			return ItemTextTemplate;
		}
		protected internal virtual void ItemsChanged() {
			if(!IsLoading()) {
				FillItemCheckedGroups();
				ValidateCheckedState();
				ValidateSelectedItem();
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items, ItemSubMenuOffset, RootItemSubMenuOffset });
		}
		protected virtual void OnItemClick(MenuItemEventArgs e) {
			MenuItemEventHandler handler = (MenuItemEventHandler)Events[EventItemClick];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnItemCommand(MenuItemCommandEventArgs e) {
			MenuItemCommandEventHandler handler = (MenuItemCommandEventHandler)Events[EventItemCommand];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnItemDataBound(MenuItemEventArgs e) {
			MenuItemEventHandler handler = (MenuItemEventHandler)Events[EventItemDataBound];
			if(handler != null)
				handler(this, e);
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null))
				fDataBound = true;
			FillItemCheckedGroups();
			ValidateCheckedState();
			ValidateSelectedItem();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			if(IsItemSelectEnabled()) {
				string indexPath = GetClientObjectStateValue<string>(SelectedItemIndexPathKey);
				if(indexPath != null) {
					if(indexPath != string.Empty)
						SelectedItem = GetItemByIndexPath(indexPath);
					else
						SelectedItem = null;
				}
			}
			if(IsItemCheckEnabled()) {
				string checkedState = GetClientObjectStateValue<string>(CheckedStateKey);
				if(checkedState != null) {
					ClearCheckedState();
					if(checkedState != string.Empty) {
						string[] indexPathes = checkedState.Split(';');
						for(int i = 0; i < indexPathes.Length; i++) {
							MenuItem item = GetItemByIndexPath(indexPathes[i]);
							if(item != null)
								item.Checked = true;
						}
					}
				}
			}
			return false;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			EnsureDataBound();
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch(arguments[0]) {
				case "CLICK":
					MenuItem item = GetItemByIndexPath(arguments[1]);
					if(IsItemSelectEnabled() && HasItemServerClick(item))
						SelectedItem = item;
					if(IsItemCheckEnabled() && HasItemServerClick(item)) {
						if(IsCheckableItem(item)) {
							if(IsSingleCheckedGroupItem(item))
								item.Checked = !item.Checked;
							else
								item.Checked = true;
						}
					}
					OnItemClick(new MenuItemEventArgs(item));
					break;
			}
		}
		protected override object GetCallbackResult() {
			EnsureChildControls();
			Hashtable result = new Hashtable();
			BeginRendering();
			try {
				foreach(string indexPath in subMenuContentControls.Keys) {
					string renderResult = RenderUtils.GetRenderResult(subMenuContentControls[indexPath]);
					result.Add(indexPath, renderResult);
				}
			}
			finally {
				EndRendering();
			}
			return result;
		}
		protected internal void RegisterSubMenuContentControl(MenuItem parentItem, Control control) {
			if(IsCallBacksEnabled() && !IsMainMenu(parentItem))
				this.subMenuContentControls.Add(GetItemIndexPath(parentItem), control);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.MenuItemsOwner"; } }
		private object GetCheckedScriptObject(ItemImagePropertiesBase itemImageProperties) {
			if(!string.IsNullOrEmpty(itemImageProperties.IconID)
					|| (!string.IsNullOrEmpty(itemImageProperties.SpritePropertiesInternal.CssClass) && (!itemImageProperties.SpritePropertiesInternal.CssClass.Contains(MenuImages.SubMenuItemImageName) || !itemImageProperties.SpritePropertiesInternal.CheckedCssClass.Contains(MenuImages.SubMenuItemImageName)))
					|| (string.IsNullOrEmpty(itemImageProperties.UrlChecked) && !string.IsNullOrEmpty(itemImageProperties.Url))) {
				return null;
			}
			return itemImageProperties.GetCheckedScriptObject(Page);
		}
	}
	[DXWebToolboxItem(true), ToolboxData("<{0}:ASPxMenu runat=\"server\"></{0}:ASPxMenu>"),
	Designer("DevExpress.Web.Design.ASPxMenuDesignerBase, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxMenu.bmp")
	]
	public class ASPxMenu : ASPxMenuBase {
		private MenuItem AdaptiveItemInternal { get; set; }
		private MenuItem RenderRootItemInternal { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuEnableAdaptivity"),
#endif
		Category("Settings"), DefaultValue(false), AutoFormatDisable]
		public virtual bool EnableAdaptivity {
			get { return GetBoolProperty("EnableAdaptivity", false); }
			set { SetBoolProperty("EnableAdaptivity", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return ((AppearanceStyle)ControlStyle).HorizontalAlign; }
			set { ((AppearanceStyle)ControlStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuItemAutoWidth"),
#endif
		Category("Layout"), DefaultValue(true), AutoFormatEnable]
		public bool ItemAutoWidth {
			get { return GetBoolProperty("ItemAutoWidth", true); }
			set { SetBoolProperty("ItemAutoWidth", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuItemWrap"),
#endif
		Category("Layout"), DefaultValue(true), AutoFormatEnable]
		public bool ItemWrap {
			get { return GetBoolProperty("ItemWrap", true); }
			set { SetBoolProperty("ItemWrap", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuItemImagePosition"),
#endif
		Category("Appearance"), DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition ItemImagePosition {
			get { return (ImagePosition)GetEnumProperty("ItemImagePosition", ImagePosition.Left); }
			set {
				SetEnumProperty("ItemImagePosition", ImagePosition.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuEnableSubMenuFullWidth"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableSubMenuFullWidth {
			get { return GetBoolProperty("EnableSubMenuFullWidth", false); }
			set { SetBoolProperty("EnableSubMenuFullWidth", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuEnableSubMenuScrolling"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableSubMenuScrolling {
			get { return EnableScrollingInternal; }
			set { EnableScrollingInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuShowAsToolbar"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public bool ShowAsToolbar {
			get { return GetBoolProperty("ShowAsToolbar", false); }
			set { SetBoolProperty("ShowAsToolbar", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuOrientation"),
#endif
		Category("Layout"), DefaultValue(Orientation.Horizontal), AutoFormatDisable]
		public Orientation Orientation {
			get { return (Orientation)GetEnumProperty("Orientation", Orientation.Horizontal); }
			set {
				SetEnumProperty("Orientation", Orientation.Horizontal, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuVerticalAlign"),
#endif
		Category("Layout"), DefaultValue(VerticalAlign.NotSet), AutoFormatEnable]
		public VerticalAlign VerticalAlign {
			get { return ((AppearanceStyle)ControlStyle).VerticalAlign; }
			set { ((AppearanceStyle)ControlStyle).VerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuFirstSubMenuDirection"),
#endif
		Category("Layout"), DefaultValue(FirstSubMenuDirection.Auto), AutoFormatDisable]
		public FirstSubMenuDirection FirstSubMenuDirection
		{
			get { return (FirstSubMenuDirection)GetEnumProperty("FirstSubMenuDirection", FirstSubMenuDirection.Auto); }
			set { SetEnumProperty("FirstSubMenuDirection", FirstSubMenuDirection.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxMenuAdaptiveMenuImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties AdaptiveMenuImage {
			get { return Images.AdaptiveMenu; }
		}
		protected internal MenuItem AdaptiveItem {
			get {
				if(AdaptiveItemInternal == null)
					AdaptiveItemInternal = CreateAdaptiveItem();
				return AdaptiveItemInternal;
			}
		}
		protected internal override MenuItem RenderRootItem {
			get {
				if(!IsAdaptivityEnabled())
					return base.RenderRootItem;
				if(RenderRootItemInternal == null)
					RenderRootItemInternal = CreateRenderRootItem();
				return RenderRootItemInternal;
			}
		}
		public ASPxMenu()
			: base() {
		}
		protected ASPxMenu(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected virtual MenuItem CreateRenderRootItem() {
			MenuItem item = new MenuItem(this);
			item.Items.Assign(RootItem.Items);
			item.Items.Add(AdaptiveItem);
			return item;
		}
		protected virtual MenuItem CreateAdaptiveItem() {
			MenuItem item = new AdaptiveMenuItem();
			item.DropDownMode = true;
			return item;
		}
		protected internal override void ItemsChanged() {
			base.ItemsChanged();
			if(!IsLoading()) {
				AdaptiveItemInternal = null;
				RenderRootItemInternal = null;
			}
		}
		protected internal bool IsAdaptivityEnabled() {
			return EnableAdaptivity && !Width.IsEmpty;
		}
		protected internal override string GetItemAdaptiveText(MenuItem item) {
			if(!IsAdaptivityEnabled())
				return string.Empty;
			return string.Format(TextFormatString, item.AdaptiveText);
		}
		protected internal override bool IsAutoWidthMode(MenuItem item) {
			return IsMainMenu(item) && !Width.IsEmpty && ItemAutoWidth && !IsWrapPreventedInternal();
		}
		protected internal override bool IsWrapPrevented(MenuItem item) {
			return IsMainMenu(item) && (IsWrapPreventedInternal() || IsAdaptivityEnabled());
		}
		protected internal bool IsWrapPreventedInternal() {
			return !ItemWrap || GetControlStyle().HorizontalAlign == HorizontalAlign.Center;
		}
		protected internal override ImagePosition GetItemImagePosition(MenuItem item) {
			if(IsMainMenu(item)) 
				return ItemImagePosition;
			return base.GetItemImagePosition(item);
		}
		protected internal override Orientation GetOrientation(MenuItem item) {
			if(IsMainMenu(item))
				return Orientation;
			return base.GetOrientation(item);
		}
		protected internal override bool IsShowAsToolbar() {
			return ShowAsToolbar && Orientation == Orientation.Horizontal;
		}
		protected override bool HasClientInitialization() {
			if(base.HasClientInitialization() || HorizontalAlign == HorizontalAlign.Center)
				return true;
			for(int i = 0; i < RenderRootItem.Items.Count; i++) {
				if(GetItemStyle(RenderRootItem.Items[i]).HorizontalAlign != HorizontalAlign.NotSet)
					return true;
			}
			return false;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Orientation == Orientation.Vertical)
				stb.Append(localVarName + ".isVertical=true;\n");
			if(FirstSubMenuDirection != FirstSubMenuDirection.Auto)
				stb.AppendFormat("{0}.firstSubMenuDirection=\"{1}\";\n", localVarName, FirstSubMenuDirection.ToString());
			if(EnableSubMenuFullWidth)
				stb.AppendFormat("{0}.enableSubMenuFullWidth=true;\n", localVarName);
			if(IsAdaptivityEnabled()) 
				CreateSetAdaptiveModeScript(stb, localVarName);
		}
		protected void CreateSetAdaptiveModeScript(StringBuilder stb, string localVarName) {
			List<MenuItem> items = new List<MenuItem>(RootItem.Items);
			items.Sort((item1, item2) => {
				int result = item2.AdaptivePriority - item1.AdaptivePriority;
				if(result == 0)
					result = item2.VisibleIndex - item1.VisibleIndex;
				return result;
			});
			List<string> prioritiesArray = new List<string>();
			bool renderPriorities = false;
			foreach(MenuItem item in items) {
				prioritiesArray.Add(item.IndexPath);
				if(item.AdaptivePriority > 0)
					renderPriorities = true;
			}
			stb.Append(localVarName + ".SetAdaptiveMode(" + (renderPriorities ? HtmlConvertor.ToJSON(prioritiesArray) : Items.Count.ToString()) + ");\n");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientMenu";
		}
	}
	public class MenuDataFields {
		protected internal string navigateUrlFieldName, textFieldName,
			toolTipFieldName, imageUrlFieldName, nameFieldName;
	}
}
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class ASPxMenuExt : ASPxMenu {
		protected override string GetClientObjectClassName() {
			return "ASPxClientMenuExt";
		}
		protected internal override MenuItem CreateSampleItems() {
			MenuItem rootSubItem = CreateSampleItemsCore(new MenuItem());
			MenuItem rootItem = new MenuItem(this);
			rootItem.Items.Add(rootSubItem);
			return rootItem;
		}
		protected internal override bool NeedCreateItemsOnClientSide() {
			return true;
		}
	}
}
