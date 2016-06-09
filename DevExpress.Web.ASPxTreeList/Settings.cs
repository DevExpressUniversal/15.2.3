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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web.ASPxTreeList.Localization;
using DevExpress.Web.ASPxTreeList.Internal;
namespace DevExpress.Web.ASPxTreeList {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class TreeListSettingsBase : StateManager {
		ASPxTreeList treeList;
		internal TreeListSettingsBase(ASPxTreeList treeList) 
			: base() {
			this.treeList = treeList;			
		}
		protected ASPxTreeList TreeList { get { return treeList; } }
		protected void LayoutChanged() {
			IWebControlObject owner = TreeList as IWebControlObject;
			if(owner != null)
				owner.LayoutChanged();
		}
		protected void ResetVisibleData() {
			if(TreeList != null)
				TreeList.TreeDataHelper.ResetVisibleData();
		}
		public virtual void Assign(TreeListSettingsBase source) {
		}
	}
	public class TreeListSettings : TreeListSettingsBase {
		protected internal TreeListSettings(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsShowPreview"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool ShowPreview {
			get { return GetBoolProperty("ShowPreview", false); }
			set {
				if(value != ShowPreview) {
					SetBoolProperty("ShowPreview", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsShowTreeLines"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowTreeLines {
			get { return GetBoolProperty("ShowTreeLines", true); }
			set {
				if(value != ShowTreeLines) {
					SetBoolProperty("ShowTreeLines", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsShowRoot"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowRoot {
			get { return GetBoolProperty("ShowRoot", true); }
			set {
				if(value != ShowRoot) {
					SetBoolProperty("ShowRoot", true, value);
					ResetVisibleData(); 
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsShowColumnHeaders"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowColumnHeaders {
			get { return GetBoolProperty("ShowColumnHeaders", true); }
			set {
				if(value != ShowColumnHeaders) {
					SetBoolProperty("ShowColumnHeaders", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsShowGroupFooter"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool ShowGroupFooter {
			get { return GetBoolProperty("ShowGroupFooter", false); }
			set {
				if(value != ShowGroupFooter) {
					SetBoolProperty("ShowGroupFooter", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsShowFooter"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool ShowFooter {
			get { return GetBoolProperty("ShowFooter", false); }
			set {
				if(value != ShowFooter) {
					SetBoolProperty("ShowFooter", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsGridLines"),
#endif
		NotifyParentProperty(true), DefaultValue(GridLines.None), AutoFormatDisable]
		public GridLines GridLines {
			get { return (GridLines)GetEnumProperty("GridLines", GridLines.None); }
			set {
				if(value != GridLines) {
					SetEnumProperty("GridLines", GridLines.None, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsSuppressOuterGridLines"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public bool SuppressOuterGridLines {
			get { return GetBoolProperty("SuppressOuterGridLines", false); }
			set {
				if(value != SuppressOuterGridLines) {
					SetEnumProperty("SuppressOuterGridLines", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsHorizontalScrollBarMode"),
#endif
 NotifyParentProperty(true), DefaultValue(ScrollBarMode.Hidden), AutoFormatDisable]
		public ScrollBarMode HorizontalScrollBarMode {
			get { return (ScrollBarMode)GetEnumProperty("HorizontalScrollBarMode", ScrollBarMode.Hidden); }
			set {
				if(HorizontalScrollBarMode != value) {
					SetEnumProperty("HorizontalScrollBarMode", ScrollBarMode.Hidden, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsVerticalScrollBarMode"),
#endif
 NotifyParentProperty(true), DefaultValue(ScrollBarMode.Hidden), AutoFormatDisable]
		public ScrollBarMode VerticalScrollBarMode {
			get { return (ScrollBarMode)GetEnumProperty("VerticalScrollBarMode", ScrollBarMode.Hidden); }
			set {
				if(VerticalScrollBarMode != value) {
					SetEnumProperty("VerticalScrollBarMode", ScrollBarMode.Hidden, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsScrollableHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(200), AutoFormatDisable]
		public int ScrollableHeight {
			get { return GetIntProperty("ScrollableHeight", 200); }
			set {
				int val = Math.Max(50, value);
				SetIntProperty("ScrollableHeight", 200, val);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsColumnMinWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatDisable]
		public int ColumnMinWidth {
			get { return GetIntProperty("ColumnMinWidth", 0); }
			set { SetIntProperty("ColumnMinWidth", 0, Math.Max(0, value)); }
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettings src = source as TreeListSettings;
			if(src != null) {
				GridLines = src.GridLines;
				ShowColumnHeaders = src.ShowColumnHeaders;
				ShowFooter = src.ShowFooter;
				ShowGroupFooter = src.ShowGroupFooter;
				ShowPreview = src.ShowPreview;
				ShowRoot = src.ShowRoot;
				ShowTreeLines = src.ShowTreeLines;
				SuppressOuterGridLines = src.SuppressOuterGridLines;
				ScrollableHeight = src.ScrollableHeight;
				HorizontalScrollBarMode = src.HorizontalScrollBarMode;
				VerticalScrollBarMode = src.VerticalScrollBarMode;
				ColumnMinWidth = src.ColumnMinWidth;
			}
		}
	}
	public class TreeListSettingsBehavior : TreeListSettingsBase {
		protected internal TreeListSettingsBehavior(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorAutoExpandAllNodes"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AutoExpandAllNodes {
			get { return GetBoolProperty("AutoExpandAllNodes", false); }
			set {
				if(value != AutoExpandAllNodes) {
					SetBoolProperty("AutoExpandAllNodes", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorAllowSort"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool AllowSort {
			get { return GetBoolProperty("AllowSort", true); }
			set {
				if(value != AllowSort) {
					SetBoolProperty("AllowSort", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorAllowFocusedNode"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AllowFocusedNode {
			get { return GetBoolProperty("AllowFocusedNode", false); }
			set {
				if(value != AllowFocusedNode) {
					SetBoolProperty("AllowFocusedNode", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorDisablePartialUpdate"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool DisablePartialUpdate {
			get { return GetBoolProperty("DisablePartialUpdate", false); }
			set { SetBoolProperty("DisablePartialUpdate", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorAllowDragDrop"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool AllowDragDrop {
			get { return GetBoolProperty("AllowDragDrop", true); }
			set {
				if(value != AllowDragDrop) {
					SetBoolProperty("AllowDragDrop", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorProcessFocusedNodeChangedOnServer"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool ProcessFocusedNodeChangedOnServer {
			get { return GetBoolProperty("ProcessFocusedNodeChangedOnServer", false); }
			set { SetBoolProperty("ProcessFocusedNodeChangedOnServer", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorProcessSelectionChangedOnServer"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool ProcessSelectionChangedOnServer {
			get { return GetBoolProperty("ProcessSelectionChangedOnServer", false); }
			set { SetBoolProperty("ProcessSelectionChangedOnServer", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorExpandCollapseAction"),
#endif
		NotifyParentProperty(true), DefaultValue(TreeListExpandCollapseAction.Button)]
		public TreeListExpandCollapseAction ExpandCollapseAction {
			get { return (TreeListExpandCollapseAction)GetEnumProperty("ExpandCollapseAction", TreeListExpandCollapseAction.Button); }
			set { SetEnumProperty("ExpandCollapseAction", TreeListExpandCollapseAction.Button, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorFocusNodeOnExpandButtonClick"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool FocusNodeOnExpandButtonClick {
			get { return GetBoolProperty("FocusNodeOnExpandButtonClick", true); }
			set { SetBoolProperty("FocusNodeOnExpandButtonClick", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorFocusNodeOnLoad"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool FocusNodeOnLoad {
			get { return GetBoolProperty("FocusNodeOnLoad", true); }
			set {
				if(value != FocusNodeOnLoad) {
					SetBoolProperty("FocusNodeOnLoad", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsBehaviorColumnResizeMode"),
#endif
 DefaultValue(ColumnResizeMode.Disabled), NotifyParentProperty(true)]
		public ColumnResizeMode ColumnResizeMode {
			get { return (ColumnResizeMode)GetEnumProperty("ColumnResizeMode", ColumnResizeMode.Disabled); }
			set {
				if(value != ColumnResizeMode) {
					SetEnumProperty("ColumnResizeMode", ColumnResizeMode.Disabled, value);
					LayoutChanged();
				}
			}
		}
		[NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool AllowEllipsisInText {
			get { return GetBoolProperty("AllowEllipsisInText", false); }
			set { SetBoolProperty("AllowEllipsisInText", false, value); }
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsBehavior src = source as TreeListSettingsBehavior;
			if(src != null) {
				AllowDragDrop = src.AllowDragDrop;
				AllowFocusedNode = src.AllowFocusedNode;
				AllowSort = src.AllowSort;
				AutoExpandAllNodes = src.AutoExpandAllNodes;
				DisablePartialUpdate = src.DisablePartialUpdate;
				ExpandCollapseAction = src.ExpandCollapseAction;
				FocusNodeOnExpandButtonClick = src.FocusNodeOnExpandButtonClick;
				FocusNodeOnLoad = src.FocusNodeOnLoad;
				ProcessFocusedNodeChangedOnServer = src.ProcessFocusedNodeChangedOnServer;
				ProcessSelectionChangedOnServer = src.ProcessSelectionChangedOnServer;
				ColumnResizeMode = src.ColumnResizeMode;
				AllowEllipsisInText = src.AllowEllipsisInText;
			}
		}
	}
	public class TreeListSettingsPager : PagerSettingsEx {
		protected internal TreeListSettingsPager(IPropertiesOwner owner) 
			: base(owner) {
			Position = PagerPosition.Bottom;
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPagerMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(TreeListPagerMode.ShowAllNodes)]
		public TreeListPagerMode Mode {
			get { return (TreeListPagerMode)GetEnumProperty("Mode", TreeListPagerMode.ShowAllNodes); }
			set {
				if(value != Mode) {
					SetEnumProperty("Mode", TreeListPagerMode.ShowAllNodes, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPagerAlwaysShowPager"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(false)]
		public bool AlwaysShowPager {
			get { return GetBoolProperty("AlwaysShowPager", false); }
			set {
				if(value != AlwaysShowPager) {
					SetBoolProperty("AlwaysShowPager", false, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPagerPosition"),
#endif
		DefaultValue(PagerPosition.Bottom)]
		public override PagerPosition Position {
			get { return base.Position; }
			set {
				if(value != Position)
					base.Position = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPagerPageSize"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(10)]
		public int PageSize {
			get { return GetIntProperty("PageSize", 10); }
			set {
				if(value != PageSize) {
					CommonUtils.CheckNegativeOrZeroValue(value, "PageSize");
					SetIntProperty("PageSize", 10, value);
					Changed();
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TreeListSettingsPager src = source as TreeListSettingsPager;
			if(src != null) {
				AlwaysShowPager = src.AlwaysShowPager;
				Mode = src.Mode;
				PageSize = src.PageSize;
			}
		}
	}
	public class TreeListSettingsCustomizationWindow : TreeListSettingsBase {
		const int 
			DefaultWidth = 150,
			DefaultHeight = 170;		
		const PopupHorizontalAlign DefaultHorizaontalAlign = PopupHorizontalAlign.OutsideRight;
		const PopupVerticalAlign DefaultVerticalAlign = PopupVerticalAlign.TopSides;
		protected internal TreeListSettingsCustomizationWindow(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowEnabled"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set {
				if(value != Enabled)
					SetBoolProperty("Enabled", false, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowPopupHorizontalAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultHorizaontalAlign)]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupHorizontalAlign", DefaultHorizaontalAlign); }
			set {
				if(value != PopupHorizontalAlign)
					SetEnumProperty("PopupHorizontalAlign", DefaultHorizaontalAlign, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowPopupVerticalAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultVerticalAlign)]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupVerticalAlign", DefaultVerticalAlign); }
			set {
				if(value != PopupVerticalAlign)
					SetEnumProperty("PopupVerticalAlign", DefaultVerticalAlign, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowPopupHorizontalOffset"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int PopupHorizontalOffset {
			get { return GetIntProperty("PopupHorizontalOffset", 0); }
			set {
				if(value != PopupHorizontalOffset)
					SetIntProperty("PopupHorizontalOffset", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowPopupVerticalOffset"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int PopupVerticalOffset {
			get { return GetIntProperty("PopupVerticalOffset", 0); }
			set {
				if(value != PopupVerticalOffset)
					SetIntProperty("PopupVerticalOffset", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowPopupWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit PopupWidth {
			get { return GetUnitProperty("PopupWidth", Unit.Empty); }
			set {
				if(value != PopupWidth)
					SetUnitProperty("PopupWidth", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowPopupHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit PopupHeight {
			get { return GetUnitProperty("PopupHeight", Unit.Empty); }
			set {
				if(value != PopupHeight)
					SetUnitProperty("PopupHeight", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCustomizationWindowCaption"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.CustomizationWindowCaption), Localizable(true)]
		public string Caption {
			get { return GetStringProperty("Caption", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CustomizationWindowCaption)); }
			set {
				if(value != Caption)
					SetStringProperty("Caption", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CustomizationWindowCaption), value);
			}
		}
		protected internal void AssignToPopupControl(ASPxPopupControl popup) {
			popup.Width = PopupWidth.IsEmpty ? DefaultWidth : PopupWidth; ;
			popup.Height = PopupHeight.IsEmpty ? DefaultHeight : PopupHeight;
			popup.PopupHorizontalAlign = PopupHorizontalAlign;
			popup.PopupVerticalAlign = PopupVerticalAlign;
			popup.PopupHorizontalOffset = PopupHorizontalOffset;
			popup.PopupVerticalOffset = PopupVerticalOffset;
			popup.HeaderText = Caption;
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsCustomizationWindow src = source as TreeListSettingsCustomizationWindow;
			if(src != null) {
				Caption = src.Caption;
				Enabled = src.Enabled;
				PopupHeight = src.PopupHeight;
				PopupHorizontalAlign = src.PopupHorizontalAlign;
				PopupHorizontalOffset = src.PopupHorizontalOffset;
				PopupVerticalAlign = src.PopupVerticalAlign;
				PopupVerticalOffset = src.PopupVerticalOffset;
				PopupWidth = src.PopupWidth;
			}
		}
	}
	public class TreeListSettingsSelection : TreeListSettingsBase {
		protected internal TreeListSettingsSelection(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsSelectionEnabled"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set {
				if(value != Enabled) {
					SetBoolProperty("Enabled", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsSelectionAllowSelectAll"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AllowSelectAll {
			get { return GetBoolProperty("AllowSelectAll", false); }
			set {
				if(value != AllowSelectAll) {
					SetBoolProperty("AllowSelectAll", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsSelectionRecursive"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Recursive {
			get { return GetBoolProperty("Recursive", false); }
			set {
				if(value != Recursive) {
					SetBoolProperty("Recursive", false, value);
					LayoutChanged();
				}
			}
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsSelection src = source as TreeListSettingsSelection;
			if(src != null) {
				AllowSelectAll = src.AllowSelectAll;
				Enabled = src.Enabled;
				Recursive = src.Recursive;
			}
		}
	}
	public class TreeListSettingsCookies : TreeListSettingsBase {
		protected internal TreeListSettingsCookies(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesEnabled"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set { SetBoolProperty("Enabled", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesCookiesID"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string CookiesID {
			get { return GetStringProperty("CookiesID", ""); }
			set { SetStringProperty("CookiesID", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesStoreExpandedNodes"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool StoreExpandedNodes {
			get { return GetBoolProperty("StoreExpandedNodes", false); }
			set { SetBoolProperty("StoreExpandedNodes", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesStoreSelection"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool StoreSelection {
			get { return GetBoolProperty("StoreSelection", false); }
			set { SetBoolProperty("StoreSelection", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesStoreSorting"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool StoreSorting {
			get { return GetBoolProperty("StoreSorting", false); }
			set { SetBoolProperty("StoreSorting", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesStoreColumnsVisiblePosition"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool StoreColumnsVisiblePosition {
			get { return GetBoolProperty("StoreColumnsVisiblePosition", false); }
			set { SetBoolProperty("StoreColumnsVisiblePosition", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesStoreColumnsWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool StoreColumnsWidth {
			get { return GetBoolProperty("StoreColumnsWidth", false); }
			set { SetBoolProperty("StoreColumnsWidth", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesStorePaging"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool StorePaging {
			get { return GetBoolProperty("StorePaging", false); }
			set { SetBoolProperty("StorePaging", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsCookiesVersion"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string Version {
			get { return GetStringProperty("Version", String.Empty); }
			set { SetStringProperty("Version", String.Empty, value); }
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsCookies src = source as TreeListSettingsCookies;
			if(src != null) {
				CookiesID = src.CookiesID;
				Enabled = src.Enabled;
				StoreColumnsVisiblePosition = src.StoreColumnsVisiblePosition;
				StoreColumnsWidth = src.StoreColumnsWidth;
				StoreExpandedNodes = src.StoreExpandedNodes;
				StorePaging = src.StorePaging;
				StoreSelection = src.StoreSelection;
				StoreSorting = src.StoreSorting;
				Version = src.Version;
			}
		}
	}
	public class TreeListSettingsLoadingPanel : SettingsLoadingPanel {
		protected internal TreeListSettingsLoadingPanel(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsLoadingPanelShowOnPostBacks"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(false)]
		public bool ShowOnPostBacks {
			get { return GetBoolProperty("ShowOnPostBacks", false); }
			set { SetBoolProperty("ShowOnPostBacks", false, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TreeListSettingsLoadingPanel src = source as TreeListSettingsLoadingPanel;
			if(src != null) {
				ShowOnPostBacks = src.ShowOnPostBacks;
			}
		}
	}
	public class TreeListSettingsEditing : TreeListSettingsBase {
		protected internal TreeListSettingsEditing(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsEditingEditFormColumnCount"),
#endif
		NotifyParentProperty(true), DefaultValue(2)]
		public int EditFormColumnCount {
			get { return GetIntProperty("EditFormColumnCount", 2); }
			set {
				if(value < 1) value = 1;
				SetIntProperty("EditFormColumnCount", 2, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsEditingMode"),
#endif
		NotifyParentProperty(true), DefaultValue(TreeListEditMode.Inline)]
		public TreeListEditMode Mode {
			get { return (TreeListEditMode)GetEnumProperty("Mode", TreeListEditMode.Inline); }
			set {
				if(value != Mode) {
					SetEnumProperty("Mode", TreeListEditMode.Inline, value);
					if(TreeList != null) {
						TreeList.TreeDataHelper.RefreshFakeNewNode();
						LayoutChanged();
					}
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsEditingAllowNodeDragDrop"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AllowNodeDragDrop {
			get { return GetBoolProperty("AllowNodeDragDrop", false); }
			set {
				if(value != AllowNodeDragDrop) {
					SetBoolProperty("AllowNodeDragDrop", false, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsEditingConfirmDelete"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ConfirmDelete {
			get { return GetBoolProperty("ConfirmDelete", true); }
			set { SetBoolProperty("ConfirmDelete", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsEditingAllowRecursiveDelete"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AllowRecursiveDelete {
			get { return GetBoolProperty("AllowRecursiveDelete", false); }
			set { SetBoolProperty("AllowRecursiveDelete", false, value); }
		}
		protected internal bool IsPopupEditForm { get { return Mode == TreeListEditMode.PopupEditForm; } }
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsEditing src = source as TreeListSettingsEditing;
			if(src != null) {
				AllowNodeDragDrop = src.AllowNodeDragDrop;
				AllowRecursiveDelete = src.AllowRecursiveDelete;
				ConfirmDelete = src.ConfirmDelete;
				EditFormColumnCount = src.EditFormColumnCount;
				Mode = src.Mode;
			}
		}
	}
	public class TreeListSettingsPopupEditForm : TreeListSettingsBase {
		protected internal TreeListSettingsPopupEditForm(ASPxTreeList treeList)
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormMinWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit MinWidth {
			get { return GetUnitProperty("MinWidth", Unit.Empty); }
			set { SetUnitProperty("MinWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormMinHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit MinHeight {
			get { return GetUnitProperty("MinHeight", Unit.Empty); }
			set { SetUnitProperty("MinHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormShowHeader"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowHeader {
			get { return GetBoolProperty("ShowHeader", true); }
			set {
				if(value == ShowHeader)
					return;
				SetBoolProperty("ShowHeader", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormModal"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Modal {
			get { return GetBoolProperty("Modal", false); }
			set {
				if(value == Modal)
					return;
				SetBoolProperty("Modal", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormAllowResize"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AllowResize {
			get { return GetBoolProperty("AllowResize", false); }
			set {
				if(value == AllowResize)
					return;
				SetBoolProperty("AllowResize", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormPopupElementID"),
#endif
		NotifyParentProperty(true), DefaultValue("")]
		public string PopupElementID {
			get { return GetStringProperty("PopupElementID", string.Empty); }
			set { SetStringProperty("PopupElementID", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormHorizontalAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public PopupHorizontalAlign HorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("HorizontalAlign", PopupHorizontalAlign.RightSides); }
			set { SetEnumProperty("HorizontalAlign", PopupHorizontalAlign.RightSides, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormVerticalAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.Below)]
		public PopupVerticalAlign VerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("VerticalAlign", PopupVerticalAlign.Below); }
			set { SetEnumProperty("VerticalAlign", PopupVerticalAlign.Below, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormHorizontalOffset"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int HorizontalOffset {
			get { return GetIntProperty("HorizontalOffset", 0); }
			set { SetIntProperty("HorizontalOffset", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormVerticalOffset"),
#endif
		NotifyParentProperty(true), DefaultValue(0)]
		public int VerticalOffset {
			get { return GetIntProperty("VerticalOffset", 0); }
			set { SetIntProperty("VerticalOffset", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsPopupEditFormCaption"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.PopupEditFormCaption), Localizable(true)]
		public string Caption {
			get { return GetStringProperty("Caption", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.PopupEditFormCaption)); }
			set { SetStringProperty("Caption", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.PopupEditFormCaption), value); }
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsPopupEditForm src = source as TreeListSettingsPopupEditForm;
			if(src != null) {
				AllowResize = src.AllowResize;
				Caption = src.Caption;
				Height = src.Height;
				HorizontalAlign = src.HorizontalAlign;
				HorizontalOffset = src.HorizontalOffset;
				Modal = src.Modal;
				PopupElementID = src.PopupElementID;
				ShowHeader = src.ShowHeader;
				VerticalAlign = src.VerticalAlign;
				VerticalOffset = src.VerticalOffset;
				Width = src.Width;
				MinWidth = src.MinWidth;
				MinHeight = src.MinHeight;
			}
		}
	}
	public class TreeListSettingsDataSecurity : TreeListSettingsBase {
		public TreeListSettingsDataSecurity(ASPxTreeList treeList)
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsDataSecurityAllowInsert"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowInsert {
			get { return GetBoolProperty("AllowInsert", true); }
			set {
				if(AllowInsert == value) return;
				SetBoolProperty("AllowInsert", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsDataSecurityAllowEdit"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowEdit {
			get { return GetBoolProperty("AllowEdit", true); }
			set {
				if(AllowEdit == value) return;
				SetBoolProperty("AllowEdit", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsDataSecurityAllowDelete"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowDelete {
			get { return GetBoolProperty("AllowDelete", true); }
			set {
				if(AllowDelete == value) return;
				SetBoolProperty("AllowDelete", true, value);
				LayoutChanged();
			}
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			var src = source as TreeListSettingsDataSecurity;
			if(src != null) {
				AllowInsert = src.AllowInsert;
				AllowEdit = src.AllowEdit;
				AllowDelete = src.AllowDelete;
			}
		}
	}
	public class TreeListSettingsText : TreeListSettingsBase {
		protected internal TreeListSettingsText(ASPxTreeList treeList) 
			: base(treeList) {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextConfirmDelete"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.DataEditing_ConfirmDelete), Localizable(true)]
		public string ConfirmDelete {
			get { return GetStringProperty("ConfirmDelete", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.ConfirmDelete)); }
			set { SetStringProperty("ConfirmDelete", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.ConfirmDelete), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextCommandEdit"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.DataEditing_CommandEdit), Localizable(true)]
		public string CommandEdit {
			get { return GetStringProperty("CommandEdit", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandEdit)); }
			set { SetStringProperty("CommandEdit", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandEdit), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextCommandNew"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.DataEditing_CommandNew), Localizable(true)]
		public string CommandNew {
			get { return GetStringProperty("CommandNew", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandNew)); }
			set { SetStringProperty("CommandNew", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandNew), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextCommandDelete"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.DataEditing_CommandDelete), Localizable(true)]
		public string CommandDelete {
			get { return GetStringProperty("CommandDelete", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandDelete)); }
			set { SetStringProperty("CommandDelete", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandDelete), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextCommandUpdate"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.DataEditing_CommandUpdate), Localizable(true)]
		public string CommandUpdate {
			get { return GetStringProperty("CommandUpdate", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandUpdate)); }
			set { SetStringProperty("CommandUpdate", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandUpdate), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextCommandCancel"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.DataEditing_CommandCancel), Localizable(true)]
		public string CommandCancel {
			get { return GetStringProperty("CommandCancel", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandCancel)); }
			set { SetStringProperty("CommandCancel", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.CommandCancel), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextRecursiveDeleteError"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.TreeList_RecursiveDeleteError), Localizable(true)]
		public string RecursiveDeleteError {
			get { return GetStringProperty("RecursiveDeleteError", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.RecursiveDeleteError)); }
			set { SetStringProperty("RecursiveDeleteError", ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.RecursiveDeleteError), value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextCustomizationWindowCaption"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.CustomizationWindowCaption), Localizable(false)]
		public string CustomizationWindowCaption {
			get { return TreeList.SettingsCustomizationWindow.Caption; }
			set { TreeList.SettingsCustomizationWindow.Caption = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSettingsTextLoadingPanelText"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.LoadingPanelText), Localizable(false)]
		public string LoadingPanelText {
			get { return TreeList.SettingsLoadingPanel.Text; }
			set { TreeList.SettingsLoadingPanel.Text = value; }
		}
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			TreeListSettingsText src = source as TreeListSettingsText;
			if(src != null) {
				CommandCancel = src.CommandCancel;
				CommandDelete = src.CommandDelete;
				CommandEdit = src.CommandEdit;
				CommandNew = src.CommandNew;
				CommandUpdate = src.CommandUpdate;
				ConfirmDelete = src.ConfirmDelete;
				RecursiveDeleteError = src.RecursiveDeleteError;
			}
		}
	}
}
