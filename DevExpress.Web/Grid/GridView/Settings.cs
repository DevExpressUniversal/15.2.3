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
using DevExpress.Web.Internal;
using DevExpress.XtraGrid;
namespace DevExpress.Web {
	public class ASPxGridViewPagerSettings : ASPxGridPagerSettings {
		public ASPxGridViewPagerSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPagerSettingsMode"),
#endif
 DefaultValue(GridViewPagerMode.ShowPager), NotifyParentProperty(true), AutoFormatDisable]
		public new GridViewPagerMode Mode { get { return base.Mode; } set { base.Mode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPagerSettingsPageSize"),
#endif
 DefaultValue(10), NotifyParentProperty(true), AutoFormatDisable]
		public new int PageSize { get { return base.PageSize; } set { base.PageSize = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPagerSettingsPosition"),
#endif
 NotifyParentProperty(true), DefaultValue(PagerPosition.Bottom)]
		public override PagerPosition Position { get { return base.Position; } set { base.Position = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPagerSettingsSEOFriendly"),
#endif
 Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable]
		public override SEOFriendlyMode SEOFriendly { get { return base.SEOFriendly; } set { base.SEOFriendly = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPagerSettingsAlwaysShowPager"),
#endif
 DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public new bool AlwaysShowPager { get { return base.AlwaysShowPager; } set { base.AlwaysShowPager = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPagerSettingsShowEmptyDataRows"),
#endif
 DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowEmptyDataRows { get { return ShowEmptyGridItems; } set { ShowEmptyGridItems = value; } }
	}
	public class ASPxGridViewBehaviorSettings : ASPxGridBehaviorSettings {
		public ASPxGridViewBehaviorSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowGroup"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowGroup {
			get { return GetBoolProperty("AllowGroup", true); }
			set {
				if(value == AllowGroup)
					return;
				SetBoolProperty("AllowGroup", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowFixedGroups"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowFixedGroups {
			get { return GetBoolProperty("AllowFixedGroups", false); }
			set {
				if(value == AllowFixedGroups)
					return;
				SetBoolProperty("AllowFixedGroups", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAutoExpandAllGroups"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AutoExpandAllGroups {
			get { return GetBoolProperty("AutoExpandAllGroups", false); }
			set {
				if(value == AutoExpandAllGroups)
					return;
				SetBoolProperty("AutoExpandAllGroups", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsFilterRowMode"),
#endif
 DefaultValue(GridViewFilterRowMode.Auto), NotifyParentProperty(true)]
		public GridViewFilterRowMode FilterRowMode {
			get { return (GridViewFilterRowMode)GetIntProperty("FilterRowMode", (int)GridViewFilterRowMode.Auto); }
			set {
				if(value == FilterRowMode)
					return;
				SetIntProperty("FilterRowMode", (int)GridViewFilterRowMode.Auto, (int)value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsColumnResizeMode"),
#endif
 DefaultValue(ColumnResizeMode.Disabled), NotifyParentProperty(true)]
		public ColumnResizeMode ColumnResizeMode {
			get { return (ColumnResizeMode)GetIntProperty("ColumnResizeMode", (int)ColumnResizeMode.Disabled); }
			set {
				if(value == ColumnResizeMode)
					return;
				SetIntProperty("ColumnResizeMode", (int)ColumnResizeMode.Disabled, (int)value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAutoFilterRowInputDelay"),
#endif
 DefaultValue(1200), NotifyParentProperty(true)]
		public int AutoFilterRowInputDelay {
			get { return GetIntProperty("AutoFilterRowInputDelay", 1200); }
			set {
				if(value == AutoFilterRowInputDelay)
					return;
				SetIntProperty("AutoFilterRowInputDelay", 1200, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowFocusedRow"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowFocusedRow { get { return AllowFocusedItem; } set { AllowFocusedItem = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowSelectByRowClick"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowSelectByRowClick { get { return AllowSelectByItemClick; } set { AllowSelectByItemClick = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowSelectSingleRowOnly"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowSelectSingleRowOnly { get { return AllowSelectSingleItemOnly; } set { AllowSelectSingleItemOnly = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsProcessFocusedRowChangedOnServer"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ProcessFocusedRowChangedOnServer { get { return ProcessFocusedItemChangedOnServer; } set { ProcessFocusedItemChangedOnServer = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsEnableRowHotTrack"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool EnableRowHotTrack { get { return EnableItemHotTrack; } set { EnableItemHotTrack = value; } }
		[Obsolete("Use the SettingsPopup.HeaderFilter.Height property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsHeaderFilterDefaultHeight"),
#endif
 DefaultValue(typeof(Unit), ""), NotifyParentProperty(true)]
		public Unit HeaderFilterDefaultHeight { get { return HeaderFilterHeightInternal; } set { HeaderFilterHeightInternal = value; } }
		[Obsolete("Use the AllowSelectByRowClick property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowMultiSelection { get { return AllowSelectByRowClick; } set { AllowSelectByRowClick = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowDragDrop"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowDragDrop { get { return base.AllowDragDrop; } set { base.AllowDragDrop = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowSort"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowSort { get { return base.AllowSort; } set { base.AllowSort = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsSelectionStoringMode"),
#endif
 DefaultValue(GridViewSelectionStoringMode.DataIntegrityOptimized), NotifyParentProperty(true)]
		public new GridViewSelectionStoringMode SelectionStoringMode { get { return base.SelectionStoringMode; } set { base.SelectionStoringMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsConfirmDelete"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ConfirmDelete { get { return base.ConfirmDelete; } set { base.ConfirmDelete = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsEncodeErrorHtml"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool EncodeErrorHtml { get { return base.EncodeErrorHtml; } set { base.EncodeErrorHtml = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsProcessSelectionChangedOnServer"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ProcessSelectionChangedOnServer { get { return base.ProcessSelectionChangedOnServer; } set { base.ProcessSelectionChangedOnServer = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsHeaderFilterMaxRowCount"),
#endif
 DefaultValue(-1), NotifyParentProperty(true)]
		public new int HeaderFilterMaxRowCount { get { return base.HeaderFilterMaxRowCount; } set { base.HeaderFilterMaxRowCount = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsSortMode"),
#endif
 DefaultValue(ColumnSortMode.Default), NotifyParentProperty(true)]
		public new ColumnSortMode SortMode { get { return base.SortMode; } set { base.SortMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsAllowClientEventsOnLoad"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowClientEventsOnLoad { get { return base.AllowClientEventsOnLoad; } set { base.AllowClientEventsOnLoad = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewBehaviorSettingsEnableCustomizationWindow"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool EnableCustomizationWindow { get { return base.EnableCustomizationWindow; } set { base.EnableCustomizationWindow = value; } }
		[DefaultValue(false), NotifyParentProperty(true)]
		public new bool AllowEllipsisInText { get { return base.AllowEllipsisInText; } set { base.AllowEllipsisInText = value; } }
		protected bool ShouldSerializeAllowMultiSelection() { return false; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridViewBehaviorSettings;
				if(src != null) {
					AllowGroup = src.AllowGroup;
					AllowFixedGroups = src.AllowFixedGroups;
					FilterRowMode = src.FilterRowMode;
					ColumnResizeMode = src.ColumnResizeMode;
					AutoFilterRowInputDelay = src.AutoFilterRowInputDelay;
					AutoExpandAllGroups = src.AutoExpandAllGroups;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxGridViewSettings : ASPxGridSettings {
		public ASPxGridViewSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowFilterRow"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFilterRow {
			get {
				if(Grid != null)
					return Grid.CallbackState.Get<bool>("ShowFilterRow");
				return GetBoolProperty("ShowFilterRow", false);
			}
			set {
				if(value == ShowFilterRow)
					return;
				SetBoolProperty("ShowFilterRow", false, value);
				if(Grid != null)
					Grid.CallbackState.Put("ShowFilterRow", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowFilterRowMenu"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFilterRowMenu {
			get {
				if(Grid != null)
					return Grid.CallbackState.Get<bool>("ShowFilterRowMenu");
				return GetBoolProperty("ShowFilterRowMenu", false);
			}
			set {
				if(value == ShowFilterRowMenu)
					return;
				SetBoolProperty("ShowFilterRowMenu", false, value);
				if(Grid != null)
					Grid.CallbackState.Put("ShowFilterRowMenu", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowFilterRowMenuLikeItem"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFilterRowMenuLikeItem {
			get { return GetBoolProperty("ShowFilterRowMenuLikeItem", false); }
			set {
				if(value == ShowFilterRowMenuLikeItem)
					return;
				SetBoolProperty("ShowFilterRowMenuLikeItem", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowGroupPanel"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowGroupPanel {
			get {
				if(Grid != null)
					return Grid.CallbackState.Get<bool>("ShowGroupPanel");
				return GetBoolProperty("ShowGroupPanel", false);
			}
			set {
				if(value == ShowGroupPanel)
					return;
				SetBoolProperty("ShowGroupPanel", false, value);
				if(Grid != null)
					Grid.CallbackState.Put("ShowGroupPanel", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowGroupButtons"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowGroupButtons {
			get { return GetBoolProperty("ShowGroupButtons", true); }
			set {
				if(value == ShowGroupButtons)
					return;
				SetBoolProperty("ShowGroupButtons", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowFooter"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFooter {
			get {
				if(Grid != null)
					return Grid.CallbackState.Get<bool>("ShowFooter");
				return GetBoolProperty("ShowFooter", false);
			}
			set {
				if(value == ShowFooter)
					return;
				SetBoolProperty("ShowFooter", false, value);
				if(Grid != null)
					Grid.CallbackState.Put("ShowFooter", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowGroupFooter"),
#endif
 DefaultValue(GridViewGroupFooterMode.Hidden), NotifyParentProperty(true)]
		public GridViewGroupFooterMode ShowGroupFooter {
			get { return (GridViewGroupFooterMode)GetEnumProperty("ShowGroupFooter", GridViewGroupFooterMode.Hidden); }
			set {
				if(value == ShowGroupFooter)
					return;
				SetEnumProperty("ShowGroupFooter", GridViewGroupFooterMode.Hidden, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowPreview"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowPreview {
			get { return GetBoolProperty("ShowPreview", false); }
			set {
				if(value == ShowPreview)
					return;
				SetBoolProperty("ShowPreview", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowColumnHeaders"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowColumnHeaders {
			get { return GetBoolProperty("ShowColumnHeaders", true); }
			set {
				if(value == ShowColumnHeaders)
					return;
				SetBoolProperty("ShowColumnHeaders", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowGroupedColumns"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowGroupedColumns {
			get { return GetBoolProperty("ShowGroupedColumns", false); }
			set {
				if(value == ShowGroupedColumns)
					return;
				SetBoolProperty("ShowGroupedColumns", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsGroupFormat"),
#endif
 DefaultValue("{0}: {1} {2}"), NotifyParentProperty(true), Localizable(true)]
		public string GroupFormat {
			get { return GetStringProperty("GroupFormat", "{0}: {1} {2}"); }
			set {
				if(value == null)
					value = string.Empty;
				if(value == GroupFormat)
					return;
				SetStringProperty("GroupFormat", "{0}: {1} {2}", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsGroupSummaryTextSeparator"),
#endif
 DefaultValue(", "), NotifyParentProperty(true), Localizable(true)]
		public string GroupSummaryTextSeparator {
			get { return GetStringProperty("GroupSummaryTextSeparator", ", "); }
			set {
				value = value ?? String.Empty;
				if(value == GroupSummaryTextSeparator)
					return;
				SetStringProperty("GroupSummaryTextSeparator", ", ", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsGridLines"),
#endif
 DefaultValue(GridLines.Both), NotifyParentProperty(true)]
		public GridLines GridLines {
			get { return (GridLines)GetIntProperty("GridLines", (int)GridLines.Both); }
			set {
				if(value == GridLines)
					return;
				SetIntProperty("GridLines", (int)GridLines.Both, (int)value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsUseFixedTableLayout"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool UseFixedTableLayout {
			get { return GetBoolProperty("UseFixedTableLayout", false); }
			set {
				if(value == UseFixedTableLayout)
					return;
				SetBoolProperty("UseFixedTableLayout", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsColumnMinWidth"),
#endif
 DefaultValue(0), NotifyParentProperty(true)]
		public int ColumnMinWidth {
			get { return GetIntProperty("ColumnMinWidth", 0); }
			set { SetIntProperty("ColumnMinWidth", 0, Math.Max(0, value)); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsEnableFilterControlPopupMenuScrolling"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool EnableFilterControlPopupMenuScrolling { get { return base.EnableFilterControlPopupMenuScrolling; } set { base.EnableFilterControlPopupMenuScrolling = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowTitlePanel"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowTitlePanel { get { return base.ShowTitlePanel; } set { base.ShowTitlePanel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowHeaderFilterButton"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowHeaderFilterButton { get { return base.ShowHeaderFilterButton; } set { base.ShowHeaderFilterButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowHeaderFilterBlankItems"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowHeaderFilterBlankItems { get { return base.ShowHeaderFilterBlankItems; } set { base.ShowHeaderFilterBlankItems = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsVerticalScrollableHeight"),
#endif
 DefaultValue(200), NotifyParentProperty(true)]
		public new int VerticalScrollableHeight { get { return base.VerticalScrollableHeight; } set { base.VerticalScrollableHeight = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsVerticalScrollBarStyle"),
#endif
 DefaultValue(GridViewVerticalScrollBarStyle.Standard), NotifyParentProperty(true)]
		public new GridViewVerticalScrollBarStyle VerticalScrollBarStyle { get { return base.VerticalScrollBarStyle; } set { base.VerticalScrollBarStyle = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowStatusBar"),
#endif
 DefaultValue(GridViewStatusBarMode.Auto), NotifyParentProperty(true)]
		public new GridViewStatusBarMode ShowStatusBar { get { return base.ShowStatusBar; } set { base.ShowStatusBar = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowFilterBar"),
#endif
 DefaultValue(GridViewStatusBarMode.Hidden), NotifyParentProperty(true)]
		public new GridViewStatusBarMode ShowFilterBar { get { return base.ShowFilterBar; } set { base.ShowFilterBar = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsHorizontalScrollBarMode"),
#endif
 DefaultValue(ScrollBarMode.Hidden), NotifyParentProperty(true)]
		public new ScrollBarMode HorizontalScrollBarMode { get { return base.HorizontalScrollBarMode; } set { base.HorizontalScrollBarMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsVerticalScrollBarMode"),
#endif
 DefaultValue(ScrollBarMode.Hidden), NotifyParentProperty(true)]
		public new ScrollBarMode VerticalScrollBarMode { get { return base.VerticalScrollBarMode; } set { base.VerticalScrollBarMode = value; } }
		[Obsolete("Use the Settings.VerticalScrollBarMode property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowVerticalScrollBar"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowVerticalScrollBar { get { return ShowVerticalScrollBarInternal; } set { ShowVerticalScrollBarInternal = value; } }
		[Obsolete("Use the Settings.HorizontalScrollBarMode property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSettingsShowHorizontalScrollBar"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowHorizontalScrollBar { get { return ShowHorizontalScrollBarInternal; } set { ShowHorizontalScrollBarInternal = value; } }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridViewSettings;
				if(src != null) {
					ShowFilterRow = src.ShowFilterRow;
					ShowFilterRowMenu = src.ShowFilterRowMenu;
					ShowFilterRowMenuLikeItem = src.ShowFilterRowMenuLikeItem;
					ShowGroupPanel = src.ShowGroupPanel;
					ShowGroupButtons = src.ShowGroupButtons;
					ShowFooter = src.ShowFooter;
					ShowGroupFooter = src.ShowGroupFooter;
					ShowPreview = src.ShowPreview;
					ShowColumnHeaders = src.ShowColumnHeaders;
					ShowGroupedColumns = src.ShowGroupedColumns;
					GroupFormat = src.GroupFormat;
					GroupSummaryTextSeparator = src.GroupSummaryTextSeparator;
					GridLines = src.GridLines;
					UseFixedTableLayout = src.UseFixedTableLayout;
					ColumnMinWidth = src.ColumnMinWidth;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxGridViewEditingSettings : ASPxGridEditingSettings {
		public ASPxGridViewEditingSettings(ASPxGridView grid) : base(grid) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsNewItemRowPosition"),
#endif
 DefaultValue(GridViewNewItemRowPosition.Top), NotifyParentProperty(true)]
		public GridViewNewItemRowPosition NewItemRowPosition { get { return NewItemPosition; } set { NewItemPosition = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsEditFormColumnCount"),
#endif
 DefaultValue(2), NotifyParentProperty(true)]
		public int EditFormColumnCount {
			get { return GetIntProperty("EditFormColumnCount", 2); }
			set {
				if(value == EditFormColumnCount)
					return;
				SetIntProperty("EditFormColumnCount", 2, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsMode"),
#endif
 DefaultValue(GridViewEditingMode.EditFormAndDisplayRow), NotifyParentProperty(true)]
		public GridViewEditingMode Mode {
			get { return (GridViewEditingMode)GetIntProperty("Mode", (int)GridViewEditingMode.EditFormAndDisplayRow); }
			set {
				if(value == Mode) return;
				SetIntProperty("Mode", (int)GridViewEditingMode.EditFormAndDisplayRow, (int)value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsBatchEditSettings"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewBatchEditSettings BatchEditSettings { get { return base.BatchEditSettings as GridViewBatchEditSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsUseFormLayout"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool UseFormLayout { get { return base.UseFormLayout; } set { base.UseFormLayout = value; } }
		#region Obsolete
		[Obsolete("Use the SettingsPopup.EditForm.Width property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit PopupEditFormWidth { get { return Width; } set { Width = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.Height property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit PopupEditFormHeight { get { return Height; } set { Height = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.ShowHeader property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormShowHeader"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public bool PopupEditFormShowHeader { get { return ShowHeader; } set { ShowHeader = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.AllowResize property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormAllowResize"),
#endif
 NotifyParentProperty(true), DefaultValue(false)]
		public bool PopupEditFormAllowResize { get { return AllowResize; } set { AllowResize = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.Modal property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormModal"),
#endif
 NotifyParentProperty(true), DefaultValue(false)]
		public bool PopupEditFormModal { get { return Modal; } set { Modal = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.HorizontalAlign property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormHorizontalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public PopupHorizontalAlign PopupEditFormHorizontalAlign { get { return HorizontalAlign; } set { HorizontalAlign = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.VerticalAlign property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormVerticalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.Below)]
		public PopupVerticalAlign PopupEditFormVerticalAlign { get { return VerticalAlign; } set { VerticalAlign = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.HorizontalOffset property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormHorizontalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public int PopupEditFormHorizontalOffset { get { return HorizontalOffset; } set { HorizontalOffset = value; } }
		[Obsolete("Use the SettingsPopup.EditForm.VerticalOffset property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewEditingSettingsPopupEditFormVerticalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(-1)]
		public int PopupEditFormVerticalOffset { get { return VerticalOffset; } set { VerticalOffset = value; } }
		protected internal Unit Width {
			get { return (Unit)GetObjectProperty("Width", Unit.Empty); }
			set {
				if(value.Equals(Width))
					return;
				SetObjectProperty("Width", Unit.Empty, value);
				Changed();
			}
		}
		protected internal Unit Height {
			get { return (Unit)GetObjectProperty("Height", Unit.Empty); }
			set {
				if(value.Equals(Height))
					return;
				SetObjectProperty("Height", Unit.Empty, value);
				Changed();
			}
		}
		protected internal bool ShowHeader {
			get { return GetBoolProperty("ShowHeader", true); }
			set {
				if(value == ShowHeader)
					return;
				SetBoolProperty("ShowHeader", true, value);
				Changed();
			}
		}
		protected internal bool AllowResize {
			get { return GetBoolProperty("AllowResize", false); }
			set {
				if(value == AllowResize)
					return;
				SetBoolProperty("AllowResize", false, value);
				Changed();
			}
		}
		protected internal bool Modal {
			get { return GetBoolProperty("Modal", false); }
			set {
				if(value == Modal)
					return;
				SetBoolProperty("Modal", false, value);
				Changed();
			}
		}
		protected internal PopupHorizontalAlign HorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("HorizontalAlign", PopupHorizontalAlign.RightSides); }
			set {
				if(value == HorizontalAlign)
					return;
				SetEnumProperty("HorizontalAlign", PopupHorizontalAlign.RightSides, value);
				Changed();
			}
		}
		protected internal PopupVerticalAlign VerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("VerticalAlign", PopupVerticalAlign.Below); }
			set {
				if(value == VerticalAlign)
					return;
				SetEnumProperty("VerticalAlign", PopupVerticalAlign.Below, value);
				Changed();
			}
		}
		protected internal int HorizontalOffset {
			get { return GetIntProperty("HorizontalOffset", 0); }
			set {
				if(value == HorizontalOffset)
					return;
				SetIntProperty("HorizontalOffset", 0, value);
				Changed();
			}
		}
		protected internal int VerticalOffset {
			get { return GetIntProperty("VerticalOffset", -1); }
			set {
				if(value == VerticalOffset)
					return;
				SetIntProperty("VerticalOffset", -1, value);
				Changed();
			}
		}
		#endregion
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected override GridBatchEditSettings CreateBatchEditSettings() {
			return new GridViewBatchEditSettings(Grid);
		}
		protected internal override bool DisplayEditingRow { get { return Mode == GridViewEditingMode.EditFormAndDisplayRow || Mode == GridViewEditingMode.PopupEditForm; } }
		protected internal override bool IsPopupEditForm { get { return Mode == GridViewEditingMode.PopupEditForm; } }
		protected internal override bool IsBatchEdit { get { return Mode == GridViewEditingMode.Batch; } }
		protected internal bool IsEditForm { get { return Mode == GridViewEditingMode.EditForm || Mode == GridViewEditingMode.EditFormAndDisplayRow; } }
		protected internal bool IsInline { get { return Mode == GridViewEditingMode.Inline; } }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxGridViewEditingSettings src = source as ASPxGridViewEditingSettings;
				if(src != null) {
					EditFormColumnCount = src.EditFormColumnCount;
					Mode = src.Mode;
					Width = src.Width;
					Height = src.Height;
					ShowHeader = src.ShowHeader;
					AllowResize = src.AllowResize;
					Modal = src.Modal;
					HorizontalAlign = src.HorizontalAlign;
					VerticalAlign = src.VerticalAlign;
					HorizontalOffset = src.HorizontalOffset;
					VerticalOffset = src.VerticalOffset;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class GridViewBatchEditSettings : GridBatchEditSettings {
		public GridViewBatchEditSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBatchEditSettingsEditMode"),
#endif
 DefaultValue(GridViewBatchEditMode.Cell), NotifyParentProperty(true)]
		public new GridViewBatchEditMode EditMode { get { return base.EditMode; } set { base.EditMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBatchEditSettingsStartEditAction"),
#endif
 DefaultValue(GridViewBatchStartEditAction.Click), NotifyParentProperty(true)]
		public new GridViewBatchStartEditAction StartEditAction { get { return base.StartEditAction; } set { base.StartEditAction = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBatchEditSettingsShowConfirmOnLosingChanges"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowConfirmOnLosingChanges { get { return base.ShowConfirmOnLosingChanges; } set { base.ShowConfirmOnLosingChanges = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBatchEditSettingsAllowValidationOnEndEdit"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowValidationOnEndEdit { get { return base.AllowValidationOnEndEdit; } set { base.AllowValidationOnEndEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBatchEditSettingsAllowEndEditOnValidationError"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowEndEditOnValidationError { get { return base.AllowEndEditOnValidationError; } set { base.AllowEndEditOnValidationError = value; } }
	}
	public class ASPxGridViewCustomizationWindowSettings : ASPxGridSettingsBase {
		public ASPxGridViewCustomizationWindowSettings(ASPxGridView grid) : base(grid) { }
		[Obsolete("Use the SettingsBehavior.EnableCustomizationWindow property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsEnabled"),
#endif
 DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Enabled { get { return EnabledInternal; } set { EnabledInternal = value; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow.Width property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsWidth"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit Width { get { return WidthInternal; } set { WidthInternal = value; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow.Height property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsHeight"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit Height { get { return HeightInternal; } set { HeightInternal = value; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow.HorizontalAlign property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsPopupHorizontalAlign"),
#endif
 DefaultValue(PopupHorizontalAlign.RightSides), AutoFormatDisable, NotifyParentProperty(true)]
		public PopupHorizontalAlign PopupHorizontalAlign { get { return HorizontalAlign; } set { HorizontalAlign = value; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow.VerticalAlign property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsPopupVerticalAlign"),
#endif
 DefaultValue(PopupVerticalAlign.BottomSides), AutoFormatDisable, NotifyParentProperty(true)]
		public PopupVerticalAlign PopupVerticalAlign { get { return VerticalAlign; } set { VerticalAlign = value; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow.HorizontalOffset property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsPopupHorizontalOffset"),
#endif
 DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public int PopupHorizontalOffset { get { return HorizontalOffset; } set { HorizontalOffset = value; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow.VerticalOffset property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCustomizationWindowSettingsPopupVerticalOffset"),
#endif
 DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public int PopupVerticalOffset { get { return VerticalOffset; } set { VerticalOffset = value; } }
		protected internal bool EnabledInternal {
			get { return GetBoolProperty("EnabledInternal", false); }
			set {
				SetBoolProperty("EnabledInternal", false, value);
				Changed();
			}
		}
		protected internal Unit WidthInternal {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		protected internal Unit HeightInternal {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
		protected internal PopupHorizontalAlign HorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("HorizontalAlign", PopupHorizontalAlign.RightSides); }
			set {
				SetEnumProperty("HorizontalAlign", PopupHorizontalAlign.RightSides, value);
				Changed();
			}
		}
		protected internal PopupVerticalAlign VerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("VerticalAlign", PopupVerticalAlign.BottomSides); }
			set {
				SetEnumProperty("VerticalAlign", PopupVerticalAlign.BottomSides, value);
				Changed();
			}
		}
		protected internal int HorizontalOffset {
			get { return GetIntProperty("HorizontalOffset", 0); }
			set {
				SetIntProperty("HorizontalOffset", 0, value);
				Changed();
			}
		}
		protected internal int VerticalOffset {
			get { return GetIntProperty("VerticalOffset", 0); }
			set {
				SetIntProperty("VerticalOffset", 0, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxGridViewCustomizationWindowSettings src = source as ASPxGridViewCustomizationWindowSettings;
				if(src != null) {
					EnabledInternal = src.EnabledInternal;
					WidthInternal = src.WidthInternal;
					HeightInternal = src.HeightInternal;
					HorizontalAlign = src.HorizontalAlign;
					VerticalAlign = src.VerticalAlign;
					HorizontalOffset = src.HorizontalOffset;
					VerticalOffset = src.VerticalOffset;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxGridViewLoadingPanelSettings : ASPxGridLoadingPanelSettings {
		public ASPxGridViewLoadingPanelSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewLoadingPanelSettingsMode"),
#endif
 DefaultValue(GridViewLoadingPanelMode.Default), AutoFormatEnable, NotifyParentProperty(true)]
		public new GridViewLoadingPanelMode Mode { get { return base.Mode; } set { base.Mode = value; } }
	}
	public class ASPxGridViewCookiesSettings : ASPxGridCookiesSettings {
		public ASPxGridViewCookiesSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStoreColumnsWidth"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreColumnsWidth {
			get { return GetBoolProperty("StoreColumnsWidth", true); }
			set {
				SetBoolProperty("StoreColumnsWidth", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStoreColumnsVisiblePosition"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreColumnsVisiblePosition { 
			get { return GetBoolProperty("StoreColumnsVisiblePosition", true); }
			set {
				SetBoolProperty("StoreColumnsVisiblePosition", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStoreControlWidth"),
#endif
 DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreControlWidth {
			get { return GetBoolProperty("StoreControlWidth", false); }
			set {
				SetBoolProperty("StoreControlWidth", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStoreGroupingAndSorting"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreGroupingAndSorting { get { return StoreGroupingAndSortingInternal; } set { StoreGroupingAndSortingInternal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsEnabled"),
#endif
 DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsCookiesID"),
#endif
 DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public new string CookiesID { get { return base.CookiesID; } set { base.CookiesID = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsVersion"),
#endif
 DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public new string Version { get { return base.Version; } set { base.Version = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStorePaging"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool StorePaging { get { return base.StorePaging; } set { base.StorePaging = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStoreFiltering"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool StoreFiltering { get { return base.StoreFiltering; } set { base.StoreFiltering = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCookiesSettingsStoreSearchPanelFiltering"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool StoreSearchPanelFiltering { get { return base.StoreSearchPanelFiltering; } set { base.StoreSearchPanelFiltering = value; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxGridViewCookiesSettings;
			if(src != null) {
				StoreColumnsWidth = src.StoreColumnsWidth;
				StoreColumnsVisiblePosition = src.StoreColumnsVisiblePosition;
				StoreControlWidth = src.StoreControlWidth;
			}
		}
	}
	public class ASPxGridViewDetailSettings : ASPxGridSettingsBase {
		public ASPxGridViewDetailSettings(ASPxGridView grid) : base(grid) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailSettingsShowDetailRow"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ShowDetailRow {
			get { return GetBoolProperty("ShowDetailRow", false); }
			set {
				SetBoolProperty("ShowDetailRow", false, value);
				Changed();
			}
		}
		[Obsolete("This property is no longer required"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailSettingsIsDetailGrid"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool IsDetailGrid {
			get { return GetBoolProperty("IsDetailGrid", false); }
			set {
				SetBoolProperty("IsDetailGrid", false, value);
				Changed();
			}
		}
		protected bool ShouldSerializeIsDetailGrid() { return false; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailSettingsShowDetailButtons"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ShowDetailButtons {
			get { return GetBoolProperty("ShowDetailButtons", true); }
			set {
				SetBoolProperty("ShowDetailButtons", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailSettingsAllowOnlyOneMasterRowExpanded"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool AllowOnlyOneMasterRowExpanded {
			get { return GetBoolProperty("AllowOnlyOneMasterRowExpanded", false); }
			set {
				SetBoolProperty("AllowOnlyOneMasterRowExpanded", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailSettingsExportMode"),
#endif
		DefaultValue(GridViewDetailExportMode.None), AutoFormatDisable, NotifyParentProperty(true)]
		public GridViewDetailExportMode ExportMode {
			get { return (GridViewDetailExportMode)GetEnumProperty("ExportMode", GridViewDetailExportMode.None); }
			set {
				SetEnumProperty("ExportMode", GridViewDetailExportMode.None, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDetailSettingsExportIndex"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int ExportIndex {
			get { return GetIntProperty("ExportIndex", 0); }
			set {
				SetIntProperty("ExportIndex", 0, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ASPxGridViewDetailSettings src = source as ASPxGridViewDetailSettings;
			if(src != null) {
				ShowDetailRow = src.ShowDetailRow;
				ShowDetailButtons = src.ShowDetailButtons;
				AllowOnlyOneMasterRowExpanded = src.AllowOnlyOneMasterRowExpanded;
				ExportMode = src.ExportMode;
				ExportIndex = src.ExportIndex;
			}
		}
	}
	public class ASPxGridViewCommandButtonSettings : ASPxGridCommandButtonSettings {
		GridViewCommandButtonSettings applyFilterButton, clearFilterButton;
		GridViewAdaptiveCommandButtonSettings showAdaptiveDetailButton, hideAdaptiveDetailButton;
		public ASPxGridViewCommandButtonSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsShowAdaptiveDetailButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewAdaptiveCommandButtonSettings ShowAdaptiveDetailButton {
			get {
				if(showAdaptiveDetailButton == null)
					showAdaptiveDetailButton = CreateAdaptiveButtonSettings() as GridViewAdaptiveCommandButtonSettings;
				return showAdaptiveDetailButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsHideAdaptiveDetailButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewAdaptiveCommandButtonSettings HideAdaptiveDetailButton {
			get {
				if(hideAdaptiveDetailButton == null)
					hideAdaptiveDetailButton = CreateAdaptiveButtonSettings() as GridViewAdaptiveCommandButtonSettings;
				return hideAdaptiveDetailButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsApplyFilterButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCommandButtonSettings ApplyFilterButton {
			get {
				if(applyFilterButton == null)
					applyFilterButton = CreateButtonSettings() as GridViewCommandButtonSettings;
				return applyFilterButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsClearFilterButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCommandButtonSettings ClearFilterButton {
			get {
				if(clearFilterButton == null)
					clearFilterButton = CreateButtonSettings() as GridViewCommandButtonSettings;
				return clearFilterButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsNewButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings NewButton { get { return base.NewButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsUpdateButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings UpdateButton { get { return base.UpdateButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsCancelButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings CancelButton { get { return base.CancelButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsEditButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings EditButton { get { return base.EditButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsDeleteButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings DeleteButton { get { return base.DeleteButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsSelectButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings SelectButton { get { return base.SelectButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsSearchPanelApplyButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings SearchPanelApplyButton { get { return base.SearchPanelApplyButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsSearchPanelClearButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCommandButtonSettings SearchPanelClearButton { get { return base.SearchPanelClearButton as GridViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewCommandButtonSettingsEncodeHtml"),
#endif
 DefaultValue(false), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected override GridCommandButtonSettings CreateButtonSettings() {
			return new GridViewCommandButtonSettings(Grid);
		}
		protected virtual GridCommandButtonSettings CreateAdaptiveButtonSettings() {
			return new GridViewAdaptiveCommandButtonSettings(Grid);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridViewCommandButtonSettings;
				if(src != null) {
					ShowAdaptiveDetailButton.Assign(src.ShowAdaptiveDetailButton);
					HideAdaptiveDetailButton.Assign(src.HideAdaptiveDetailButton);
					ApplyFilterButton.Assign(src.ApplyFilterButton);
					ClearFilterButton.Assign(src.ClearFilterButton);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), 
				new IStateManager[] { ShowAdaptiveDetailButton, HideAdaptiveDetailButton, ApplyFilterButton, ClearFilterButton });
		}
	}
	public class ASPxGridViewDataSecuritySettings : ASPxGridDataSecuritySettings {
		public ASPxGridViewDataSecuritySettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDataSecuritySettingsAllowInsert"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowInsert { get { return base.AllowInsert; } set { base.AllowInsert = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDataSecuritySettingsAllowEdit"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowEdit { get { return base.AllowEdit; } set { base.AllowEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewDataSecuritySettingsAllowDelete"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowDelete { get { return base.AllowDelete; } set { base.AllowDelete = value; } }
	}
	public class GridViewCommandButtonSettings : GridCommandButtonSettings {
		public GridViewCommandButtonSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandButtonSettingsButtonType"),
#endif
 DefaultValue(GridCommandButtonRenderMode.Default), NotifyParentProperty(true)]
		public new GridCommandButtonRenderMode ButtonType { get { return base.ButtonType; } set { base.ButtonType = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandButtonSettingsText"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandButtonSettingsImage"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties Image { get { return base.Image; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandButtonSettingsStyles"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ButtonControlStyles Styles { get { return base.Styles; } }
	}
	public class GridViewAdaptiveCommandButtonSettings : GridViewCommandButtonSettings {
		public GridViewAdaptiveCommandButtonSettings(ASPxGridView grid)
			: base(grid) {
		}
		[ DefaultValue(GridCommandButtonRenderMode.Image), NotifyParentProperty(true)]
		public new GridCommandButtonRenderMode ButtonType { get { return base.ButtonType; } set { base.ButtonType = value; } }
		protected override GridCommandButtonRenderMode DefaultButtonType { get { return GridCommandButtonRenderMode.Image; } }
	}
	public class ASPxGridViewContextMenuSettings : ASPxGridSettingsBase {
		GridViewGroupPanelMenuItemVisibility groupPanelMenuItemVisibility;
		GridViewColumnMenuItemVisibility columnMenuItemVisibility;
		GridViewRowMenuItemVisibility rowMenuItemVisibility;
		GridViewFooterMenuItemVisibility footerMenuItemVisibility;
		public ASPxGridViewContextMenuSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsGroupPanelMenuItemVisibility"),
#endif
 AutoFormatDisable, NotifyParentProperty(true),
			PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewGroupPanelMenuItemVisibility GroupPanelMenuItemVisibility {
			get {
				if(groupPanelMenuItemVisibility == null)
					groupPanelMenuItemVisibility = new GridViewGroupPanelMenuItemVisibility(Grid);
				return groupPanelMenuItemVisibility;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsColumnMenuItemVisibility"),
#endif
 AutoFormatDisable, NotifyParentProperty(true),
			PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewColumnMenuItemVisibility ColumnMenuItemVisibility {
			get {
				if(columnMenuItemVisibility == null)
					columnMenuItemVisibility = new GridViewColumnMenuItemVisibility(Grid);
				return columnMenuItemVisibility;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsRowMenuItemVisibility"),
#endif
 AutoFormatDisable, NotifyParentProperty(true),
			PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowMenuItemVisibility RowMenuItemVisibility {
			get {
				if(rowMenuItemVisibility == null)
					rowMenuItemVisibility = new GridViewRowMenuItemVisibility(Grid);
				return rowMenuItemVisibility;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsFooterMenuItemVisibility"),
#endif
 AutoFormatDisable, NotifyParentProperty(true),
			PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewFooterMenuItemVisibility FooterMenuItemVisibility {
			get {
				if(footerMenuItemVisibility == null)
					footerMenuItemVisibility = new GridViewFooterMenuItemVisibility(Grid);
				return footerMenuItemVisibility;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsEnabled"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set {
				if(value != Enabled) {
					SetBoolProperty("Enabled", false, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsEnableGroupPanelMenu"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean EnableGroupPanelMenu {
			get { return (DefaultBoolean)GetEnumProperty("EnableGroupPanelMenu", DefaultBoolean.Default); }
			set {
				if(value != EnableGroupPanelMenu) {
					SetEnumProperty("EnableGroupPanelMenu", DefaultBoolean.Default, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsEnableColumnMenu"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean EnableColumnMenu {
			get { return (DefaultBoolean)GetEnumProperty("EnableColumnMenu", DefaultBoolean.Default); }
			set {
				if(value != EnableColumnMenu) {
					SetEnumProperty("EnableColumnMenu", DefaultBoolean.Default, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsEnableRowMenu"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean EnableRowMenu {
			get { return (DefaultBoolean)GetEnumProperty("EnableRowMenu", DefaultBoolean.Default); }
			set {
				if(value != EnableRowMenu) {
					SetEnumProperty("EnableRowMenu", DefaultBoolean.Default, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewContextMenuSettingsEnableFooterMenu"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean EnableFooterMenu {
			get { return (DefaultBoolean)GetEnumProperty("EnableFooterMenu", DefaultBoolean.Default); }
			set {
				if(value != EnableFooterMenu) {
					SetEnumProperty("EnableFooterMenu", DefaultBoolean.Default, value);
					Changed();
				}
			}
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxGridViewContextMenuSettings src = source as ASPxGridViewContextMenuSettings;
				if(src != null) {
					GroupPanelMenuItemVisibility.Assign(src.GroupPanelMenuItemVisibility);
					ColumnMenuItemVisibility.Assign(src.ColumnMenuItemVisibility);
					RowMenuItemVisibility.Assign(src.RowMenuItemVisibility);
					FooterMenuItemVisibility.Assign(src.FooterMenuItemVisibility);
					Enabled = src.Enabled;
					EnableGroupPanelMenu = src.EnableGroupPanelMenu;
					EnableColumnMenu = src.EnableColumnMenu;
					EnableRowMenu = src.EnableRowMenu;
					EnableFooterMenu = src.EnableFooterMenu;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(GroupPanelMenuItemVisibility);
			list.Add(ColumnMenuItemVisibility);
			list.Add(RowMenuItemVisibility);
			list.Add(FooterMenuItemVisibility);
			return list.ToArray();
		}
	}
	public class GridViewGroupPanelMenuItemVisibility : ASPxGridSettingsBase {
		public GridViewGroupPanelMenuItemVisibility(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewGroupPanelMenuItemVisibilityFullExpand"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool FullExpand {
			get { return GetBoolProperty("ShowFullExpand", true); }
			set {
				if(value != FullExpand) {
					SetBoolProperty("ShowFullExpand", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewGroupPanelMenuItemVisibilityFullCollapse"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool FullCollapse {
			get { return GetBoolProperty("ShowFullCollapse", true); }
			set {
				if(value != FullCollapse) {
					SetBoolProperty("ShowFullCollapse", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewGroupPanelMenuItemVisibilityClearGrouping"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ClearGrouping {
			get { return GetBoolProperty("ClearGrouping", true); }
			set {
				if(value != ClearGrouping) {
					SetBoolProperty("ClearGrouping", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewGroupPanelMenuItemVisibilityShowGroupPanel"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowGroupPanel {
			get { return GetBoolProperty("ShowGroupPanel", true); }
			set {
				if(value != ShowGroupPanel) {
					SetBoolProperty("ShowGroupPanel", true, value);
					Changed();
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				GridViewGroupPanelMenuItemVisibility src = source as GridViewGroupPanelMenuItemVisibility;
				if(src != null) {
					FullExpand = src.FullExpand;
					FullCollapse = src.FullCollapse;
					ClearGrouping = src.ClearGrouping;
					ShowGroupPanel = src.ShowGroupPanel;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class GridViewColumnMenuItemVisibility : ASPxGridSettingsBase {
		public GridViewColumnMenuItemVisibility(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityFullExpand"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool FullExpand {
			get { return GetBoolProperty("ShowFullExpand", true); }
			set {
				if(value != FullExpand) {
					SetBoolProperty("ShowFullExpand", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityFullCollapse"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool FullCollapse {
			get { return GetBoolProperty("ShowFullCollapse", true); }
			set {
				if(value != FullCollapse) {
					SetBoolProperty("ShowFullCollapse", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilitySortAscending"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SortAscending {
			get { return GetBoolProperty("SortAscending", true); }
			set {
				if(value != SortAscending) {
					SetBoolProperty("SortAscending", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilitySortDescending"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SortDescending {
			get { return GetBoolProperty("SortDescending", true); }
			set {
				if(value != SortDescending) {
					SetBoolProperty("SortDescending", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityClearSorting"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ClearSorting {
			get { return GetBoolProperty("ClearSorting", true); }
			set {
				if(value != ClearSorting) {
					SetBoolProperty("ClearSorting", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityGroupByColumn"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool GroupByColumn {
			get { return GetBoolProperty("GroupByColumn", true); }
			set {
				if(value != GroupByColumn) {
					SetBoolProperty("GroupByColumn", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityUngroupColumn"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool UngroupColumn {
			get { return GetBoolProperty("UngroupColumn", true); }
			set {
				if(value != UngroupColumn) {
					SetBoolProperty("UngroupColumn", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowGroupPanel"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowGroupPanel {
			get { return GetBoolProperty("ShowGroupPanel", true); }
			set {
				if(value != ShowGroupPanel) {
					SetBoolProperty("ShowGroupPanel", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowSearchPanel"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowSearchPanel {
			get { return GetBoolProperty("ShowSearchPanel", true); }
			set {
				if(value != ShowSearchPanel) {
					SetBoolProperty("ShowSearchPanel", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowColumn"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowColumn {
			get { return GetBoolProperty("ShowColumn", true); }
			set {
				if(value != HideColumn) {
					SetBoolProperty("ShowColumn", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityHideColumn"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool HideColumn {
			get { return GetBoolProperty("HideColumn", true); }
			set {
				if(value != HideColumn) {
					SetBoolProperty("HideColumn", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowCustomizationWindow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowCustomizationWindow {
			get { return GetBoolProperty("ShowCustomizationWindow", true); }
			set {
				if(value != ShowCustomizationWindow) {
					SetBoolProperty("ShowCustomizationWindow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityClearFilter"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ClearFilter {
			get { return GetBoolProperty("ClearFilter", true); }
			set {
				if(value != ClearFilter) {
					SetBoolProperty("ClearFilter", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowFilterEditor"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowFilterEditor {
			get { return GetBoolProperty("ShowFilterEditor", true); }
			set {
				if(value != ShowFilterEditor) {
					SetBoolProperty("ShowFilterEditor", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowFilterRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowFilterRow {
			get { return GetBoolProperty("ShowFilterRow", true); }
			set {
				if(value != ShowFilterRow) {
					SetBoolProperty("ShowFilterRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowFilterRowMenu"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowFilterRowMenu {
			get { return GetBoolProperty("ShowFilterRowMenu", true); }
			set {
				if(value != ShowFilterRowMenu) {
					SetBoolProperty("ShowFilterRowMenu", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMenuItemVisibilityShowFooter"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowFooter {
			get { return GetBoolProperty("ShowFooter", true); }
			set {
				if(value != ShowFooter) {
					SetBoolProperty("ShowFooter", true, value);
					Changed();
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				GridViewColumnMenuItemVisibility src = source as GridViewColumnMenuItemVisibility;
				if(src != null) {
					FullExpand = src.FullExpand;
					FullCollapse = src.FullCollapse;
					SortAscending = src.SortAscending;
					SortDescending = src.SortDescending;
					ClearSorting = src.ClearSorting;
					GroupByColumn = src.GroupByColumn;
					UngroupColumn = src.UngroupColumn;
					ShowGroupPanel = src.ShowGroupPanel;
					ShowSearchPanel = src.ShowSearchPanel;
					ShowColumn = src.ShowColumn;
					HideColumn = src.HideColumn;
					ShowCustomizationWindow = src.ShowCustomizationWindow;
					ClearFilter = src.ClearFilter;
					ShowFilterEditor = src.ShowFilterEditor;
					ShowFilterRow = src.ShowFilterRow;
					ShowFilterRowMenu = src.ShowFilterRowMenu;
					ShowFooter = src.ShowFooter;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class GridViewRowMenuItemVisibility : ASPxGridSettingsBase {
		public GridViewRowMenuItemVisibility(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityNewRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool NewRow {
			get { return GetBoolProperty("NewRow", true); }
			set {
				if(value != NewRow) {
					SetBoolProperty("NewRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityEditRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool EditRow {
			get { return GetBoolProperty("EditRow", true); }
			set {
				if(value != EditRow) {
					SetBoolProperty("EditRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityDeleteRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool DeleteRow {
			get { return GetBoolProperty("DeleteRow", true); }
			set {
				if(value != DeleteRow) {
					SetBoolProperty("DeleteRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityExpandRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ExpandRow {
			get { return GetBoolProperty("ExpandRow", true); }
			set {
				if(value != ExpandRow) {
					SetBoolProperty("ExpandRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityCollapseRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool CollapseRow {
			get { return GetBoolProperty("CollapseRow", true); }
			set {
				if(value != CollapseRow) {
					SetBoolProperty("CollapseRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityExpandDetailRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ExpandDetailRow {
			get { return GetBoolProperty("ExpandDetailRow", true); }
			set {
				if(value != ExpandDetailRow) {
					SetBoolProperty("ExpandDetailRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityCollapseDetailRow"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool CollapseDetailRow {
			get { return GetBoolProperty("CollapseDetailRow", true); }
			set {
				if(value != CollapseDetailRow) {
					SetBoolProperty("CollapseDetailRow", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewRowMenuItemVisibilityRefresh"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool Refresh {
			get { return GetBoolProperty("Refresh", true); }
			set {
				if(value != Refresh) {
					SetBoolProperty("Refresh", true, value);
					Changed();
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				GridViewRowMenuItemVisibility src = source as GridViewRowMenuItemVisibility;
				if(src != null) {
					NewRow = src.NewRow;
					EditRow = src.EditRow;
					DeleteRow = src.DeleteRow;
					ExpandRow = src.ExpandRow;
					CollapseRow = src.CollapseRow;
					ExpandDetailRow = src.ExpandDetailRow;
					CollapseDetailRow = src.CollapseDetailRow;
					Refresh = src.Refresh;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class GridViewFooterMenuItemVisibility : ASPxGridSettingsBase {
		public GridViewFooterMenuItemVisibility(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFooterMenuItemVisibilitySummarySum"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SummarySum {
			get { return GetBoolProperty("SummarySum", true); }
			set {
				if(value != SummarySum) {
					SetBoolProperty("SummarySum", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFooterMenuItemVisibilitySummaryMin"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SummaryMin {
			get { return GetBoolProperty("SummaryMin", true); }
			set {
				if(value != SummaryMin) {
					SetBoolProperty("SummaryMin", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFooterMenuItemVisibilitySummaryMax"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SummaryMax {
			get { return GetBoolProperty("SummaryMax", true); }
			set {
				if(value != SummaryMax) {
					SetBoolProperty("SummaryMax", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFooterMenuItemVisibilitySummaryCount"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SummaryCount {
			get { return GetBoolProperty("SummaryCount", true); }
			set {
				if(value != SummaryCount) {
					SetBoolProperty("SummaryCount", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFooterMenuItemVisibilitySummaryAverage"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SummaryAverage {
			get { return GetBoolProperty("SummaryAverage", true); }
			set {
				if(value != SummaryAverage) {
					SetBoolProperty("SummaryAverage", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewFooterMenuItemVisibilitySummaryNone"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool SummaryNone {
			get { return GetBoolProperty("SummaryNone", true); }
			set {
				if(value != SummaryNone) {
					SetBoolProperty("SummaryNone", true, value);
					Changed();
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				GridViewFooterMenuItemVisibility src = source as GridViewFooterMenuItemVisibility;
				if(src != null) {
					SummarySum = src.SummarySum;
					SummaryMin = src.SummaryMin;
					SummaryMax = src.SummaryMax;
					SummaryCount = src.SummaryCount;
					SummaryAverage = src.SummaryAverage;
					SummaryNone = src.SummaryNone;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxGridViewPopupControlSettings : ASPxGridPopupControlSettings {
		public ASPxGridViewPopupControlSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPopupControlSettingsEditForm"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewEditFormPopupSettings EditForm { get { return base.EditForm as GridViewEditFormPopupSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPopupControlSettingsCustomizationWindow"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewCustomizationWindowPopupSettings CustomizationWindow { get { return base.CustomizationWindow as GridViewCustomizationWindowPopupSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewPopupControlSettingsHeaderFilter"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewHeaderFilterPopupSettings HeaderFilter { get { return base.HeaderFilter as GridViewHeaderFilterPopupSettings; } }
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected override GridEditFormPopupSettings CreateEditFormSettings() {
			return new GridViewEditFormPopupSettings(Grid);
		}
		protected override GridCustomizationWindowPopupSettings CreateCustomizationPopupSettings() {
			return new GridViewCustomizationWindowPopupSettings(Grid);
		}
		protected override GridHeaderFilterPopupSettings CreateHeaderFilterPopupSettings() {
			return new GridViewHeaderFilterPopupSettings(Grid);
		}
	}
	public class GridViewEditFormPopupSettings : GridEditFormPopupSettings {
		public GridViewEditFormPopupSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsMinWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsMinHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsHorizontalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public new PopupHorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsVerticalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.Below)]
		public new PopupVerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsHorizontalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public new int HorizontalOffset { get { return base.HorizontalOffset; } set { base.HorizontalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsVerticalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(-1)]
		public new int VerticalOffset { get { return base.VerticalOffset; } set { base.VerticalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsShowHeader"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public new bool ShowHeader { get { return base.ShowHeader; } set { base.ShowHeader = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsAllowResize"),
#endif
 NotifyParentProperty(true), DefaultValue(false)]
		public new bool AllowResize { get { return base.AllowResize; } set { base.AllowResize = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsResizingMode"),
#endif
 NotifyParentProperty(true), DefaultValue(ResizingMode.Live)]
		public new ResizingMode ResizingMode { get { return base.ResizingMode; } set { base.ResizingMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsModal"),
#endif
 NotifyParentProperty(true), DefaultValue(false)]
		public new bool Modal { get { return base.Modal; } set { base.Modal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewEditFormPopupSettingsCloseOnEscape"),
#endif
 NotifyParentProperty(true), DefaultValue(AutoBoolean.Auto)]
		public new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class GridViewCustomizationWindowPopupSettings : GridCustomizationWindowPopupSettings {
		public GridViewCustomizationWindowPopupSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsHorizontalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public new PopupHorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsVerticalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.BottomSides)]
		public new PopupVerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsHorizontalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public new int HorizontalOffset { get { return base.HorizontalOffset; } set { base.HorizontalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsVerticalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public new int VerticalOffset { get { return base.VerticalOffset; } set { base.VerticalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCustomizationWindowPopupSettingsCloseOnEscape"),
#endif
 NotifyParentProperty(true), DefaultValue(AutoBoolean.Auto)]
		public new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class GridViewHeaderFilterPopupSettings : GridHeaderFilterPopupSettings {
		public GridViewHeaderFilterPopupSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupSettingsWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupSettingsHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupSettingsMinWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupSettingsMinHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupSettingsResizingMode"),
#endif
 NotifyParentProperty(true), DefaultValue(ResizingMode.Live)]
		public new ResizingMode ResizingMode { get { return base.ResizingMode; } set { base.ResizingMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewHeaderFilterPopupSettingsCloseOnEscape"),
#endif
 NotifyParentProperty(true), DefaultValue(AutoBoolean.Auto)]
		public new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CalendarProperties CalendarProperties { get { return base.CalendarProperties; } }
	}
	public class ASPxGridViewSearchPanelSettings : ASPxGridSearchPanelSettings {
		public ASPxGridViewSearchPanelSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsVisible"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsHighlightResults"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool HighlightResults { get { return base.HighlightResults; } set { base.HighlightResults = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsColumnNames"),
#endif
 DefaultValue("*"), NotifyParentProperty(true)]
		public new string ColumnNames { get { return base.ColumnNames; } set { base.ColumnNames = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsDelay"),
#endif
 DefaultValue(ASPxGridViewSearchPanelSettings.DefaultInputDelay), NotifyParentProperty(true)]
		public new int Delay { get { return base.Delay; } set { base.Delay = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsShowApplyButton"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowApplyButton { get { return base.ShowApplyButton; } set { base.ShowApplyButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsShowClearButton"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowClearButton { get { return base.ShowClearButton; } set { base.ShowClearButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsCustomEditorID"),
#endif
 DefaultValue(""), NotifyParentProperty(true)]
		public new string CustomEditorID { get { return base.CustomEditorID; } set { base.CustomEditorID = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsAllowTextInputTimer"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowTextInputTimer { get { return base.AllowTextInputTimer; } set { base.AllowTextInputTimer = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewSearchPanelSettingsGroupOperator"),
#endif
 DefaultValue(GridViewSearchPanelGroupOperator.And), NotifyParentProperty(true)]
		public new GridViewSearchPanelGroupOperator GroupOperator { get { return base.GroupOperator; } set { base.GroupOperator = value; } }
	}
	public class ASPxGridViewFilterControlSettings : ASPxGridFilterControlSettings {
		public ASPxGridViewFilterControlSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlSettingsViewMode"),
#endif
 DefaultValue(FilterControlViewMode.Visual), NotifyParentProperty(true)]
		public new FilterControlViewMode ViewMode { get { return base.ViewMode; } set { base.ViewMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlSettingsAllowHierarchicalColumns"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool AllowHierarchicalColumns { get { return base.AllowHierarchicalColumns; } set { base.AllowHierarchicalColumns = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowAllDataSourceColumns { get { return base.ShowAllDataSourceColumns; } set { base.ShowAllDataSourceColumns = value; } }
		[ DefaultValue(ASPxGridFilterControlSettings.DefaultMaxHierarchyDepth), NotifyParentProperty(true)]
		public new int MaxHierarchyDepth { get { return base.MaxHierarchyDepth; } set { base.MaxHierarchyDepth = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowOperandTypeButton { get { return base.ShowOperandTypeButton; } set { base.ShowOperandTypeButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewFilterControlSettingsGroupOperationsVisibility"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterControlGroupOperationsVisibility GroupOperationsVisibility { get { return base.GroupOperationsVisibility; } }
	}
	public class ASPxGridViewTextSettings : ASPxGridTextSettings {
		public ASPxGridViewTextSettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsTitle"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string Title { get { return base.Title; } set { base.Title = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsGroupPanel"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string GroupPanel { get { return base.GroupPanel; } set { base.GroupPanel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsConfirmDelete"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ConfirmDelete { get { return base.ConfirmDelete; } set { base.ConfirmDelete = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCustomizationWindowCaption"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CustomizationWindowCaption { get { return base.CustomizationWindowCaption; } set { base.CustomizationWindowCaption = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsPopupEditFormCaption"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string PopupEditFormCaption { get { return base.PopupEditFormCaption; } set { base.PopupEditFormCaption = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsEmptyHeaders"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string EmptyHeaders { get { return base.EmptyHeaders; } set { base.EmptyHeaders = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsGroupContinuedOnNextPage"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string GroupContinuedOnNextPage { get { return base.GroupContinuedOnNextPage; } set { base.GroupContinuedOnNextPage = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsEmptyDataRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string EmptyDataRow { get { return base.EmptyDataRow; } set { base.EmptyDataRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandEdit"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandEdit { get { return base.CommandEdit; } set { base.CommandEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandNew"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandNew { get { return base.CommandNew; } set { base.CommandNew = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandDelete"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandDelete { get { return base.CommandDelete; } set { base.CommandDelete = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandSelect"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandSelect { get { return base.CommandSelect; } set { base.CommandSelect = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandCancel"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandCancel { get { return base.CommandCancel; } set { base.CommandCancel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandUpdate"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandUpdate { get { return base.CommandUpdate; } set { base.CommandUpdate = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandClearFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandClearFilter { get { return base.CommandClearFilter; } set { base.CommandClearFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandApplyFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandApplyFilter { get { return base.CommandApplyFilter; } set { base.CommandApplyFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandBatchEditUpdate"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandBatchEditUpdate { get { return base.CommandBatchEditUpdate; } set { base.CommandBatchEditUpdate = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandBatchEditCancel"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandBatchEditCancel { get { return base.CommandBatchEditCancel; } set { base.CommandBatchEditCancel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsConfirmOnLosingBatchChanges"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ConfirmOnLosingBatchChanges { get { return base.ConfirmOnLosingBatchChanges; } set { base.ConfirmOnLosingBatchChanges = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandApplySearchPanelFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandApplySearchPanelFilter { get { return base.CommandApplySearchPanelFilter; } set { base.CommandApplySearchPanelFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandClearSearchPanelFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandClearSearchPanelFilter { get { return base.CommandClearSearchPanelFilter; } set { base.CommandClearSearchPanelFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandShowAdaptiveDetail"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandShowAdaptiveDetail { get { return base.CommandShowAdaptiveDetail; } set { base.CommandShowAdaptiveDetail = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandHideAdaptiveDetail"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandHideAdaptiveDetail { get { return base.CommandHideAdaptiveDetail; } set { base.CommandHideAdaptiveDetail = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsSearchPanelEditorNullText"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string SearchPanelEditorNullText { get { return base.SearchPanelEditorNullText; } set { base.SearchPanelEditorNullText = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterShowAll"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterShowAll { get { return base.HeaderFilterShowAll; } set { base.HeaderFilterShowAll = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterShowBlanks"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterShowBlanks { get { return base.HeaderFilterShowBlanks; } set { base.HeaderFilterShowBlanks = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterShowNonBlanks"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterShowNonBlanks { get { return base.HeaderFilterShowNonBlanks; } set { base.HeaderFilterShowNonBlanks = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterSelectAll"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterSelectAll { get { return base.HeaderFilterSelectAll; } set { base.HeaderFilterSelectAll = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterYesterday"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterYesterday { get { return base.HeaderFilterYesterday; } set { base.HeaderFilterYesterday = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterToday"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterToday { get { return base.HeaderFilterToday; } set { base.HeaderFilterToday = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterTomorrow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterTomorrow { get { return base.HeaderFilterTomorrow; } set { base.HeaderFilterTomorrow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterLastWeek"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterLastWeek { get { return base.HeaderFilterLastWeek; } set { base.HeaderFilterLastWeek = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterThisWeek"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterThisWeek { get { return base.HeaderFilterThisWeek; } set { base.HeaderFilterThisWeek = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterNextWeek"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterNextWeek { get { return base.HeaderFilterNextWeek; } set { base.HeaderFilterNextWeek = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterLastMonth"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterLastMonth { get { return base.HeaderFilterLastMonth; } set { base.HeaderFilterLastMonth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterThisMonth"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterThisMonth { get { return base.HeaderFilterThisMonth; } set { base.HeaderFilterThisMonth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterNextMonth"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterNextMonth { get { return base.HeaderFilterNextMonth; } set { base.HeaderFilterNextMonth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterLastYear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterLastYear { get { return base.HeaderFilterLastYear; } set { base.HeaderFilterLastYear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterThisYear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterThisYear { get { return base.HeaderFilterThisYear; } set { base.HeaderFilterThisYear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterNextYear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterNextYear { get { return base.HeaderFilterNextYear; } set { base.HeaderFilterNextYear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsFilterControlPopupCaption"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string FilterControlPopupCaption { get { return base.FilterControlPopupCaption; } set { base.FilterControlPopupCaption = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsFilterBarClear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string FilterBarClear { get { return base.FilterBarClear; } set { base.FilterBarClear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsFilterBarCreateFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string FilterBarCreateFilter { get { return base.FilterBarCreateFilter; } set { base.FilterBarCreateFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterOkButton"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterOkButton { get { return base.HeaderFilterOkButton; } set { base.HeaderFilterOkButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsHeaderFilterCancelButton"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterCancelButton { get { return base.HeaderFilterCancelButton; } set { base.HeaderFilterCancelButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuFullExpand"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuFullExpand { get { return base.ContextMenuFullExpand; } set { base.ContextMenuFullExpand = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuFullCollapse"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuFullCollapse { get { return base.ContextMenuFullCollapse; } set { base.ContextMenuFullCollapse = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSortAscending"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSortAscending { get { return base.ContextMenuSortAscending; } set { base.ContextMenuSortAscending = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSortDescending"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSortDescending { get { return base.ContextMenuSortDescending; } set { base.ContextMenuSortDescending = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuClearSorting"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuClearSorting { get { return base.ContextMenuClearSorting; } set { base.ContextMenuClearSorting = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuClearFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuClearFilter { get { return base.ContextMenuClearFilter; } set { base.ContextMenuClearFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowFilterEditor"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowFilterEditor { get { return base.ContextMenuShowFilterEditor; } set { base.ContextMenuShowFilterEditor = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowFilterRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowFilterRow { get { return base.ContextMenuShowFilterRow; } set { base.ContextMenuShowFilterRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowFilterRowMenu"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowFilterRowMenu { get { return base.ContextMenuShowFilterRowMenu; } set { base.ContextMenuShowFilterRowMenu = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowFooter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowFooter { get { return base.ContextMenuShowFooter; } set { base.ContextMenuShowFooter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuGroupByColumn"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuGroupByColumn { get { return base.ContextMenuGroupByColumn; } set { base.ContextMenuGroupByColumn = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuUngroupColumn"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuUngroupColumn { get { return base.ContextMenuUngroupColumn; } set { base.ContextMenuUngroupColumn = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuClearGrouping"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuClearGrouping { get { return base.ContextMenuClearGrouping; } set { base.ContextMenuClearGrouping = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowGroupPanel"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowGroupPanel { get { return base.ContextMenuShowGroupPanel; } set { base.ContextMenuShowGroupPanel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowSearchPanel"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowSearchPanel { get { return base.ContextMenuShowSearchPanel; } set { base.ContextMenuShowSearchPanel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowColumn"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowColumn { get { return base.ContextMenuShowColumn; } set { base.ContextMenuShowColumn = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuHideColumn"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuHideColumn { get { return base.ContextMenuHideColumn; } set { base.ContextMenuHideColumn = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuShowCustomizationWindow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuShowCustomizationWindow { get { return base.ContextMenuShowCustomizationWindow; } set { base.ContextMenuShowCustomizationWindow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuExpandRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuExpandRow { get { return base.ContextMenuExpandRow; } set { base.ContextMenuExpandRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuCollapseRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuCollapseRow { get { return base.ContextMenuCollapseRow; } set { base.ContextMenuCollapseRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuExpandDetailRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuExpandDetailRow { get { return base.ContextMenuExpandDetailRow; } set { base.ContextMenuExpandDetailRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuCollapseDetailRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuCollapseDetailRow { get { return base.ContextMenuCollapseDetailRow; } set { base.ContextMenuCollapseDetailRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuRefresh"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuRefresh { get { return base.ContextMenuRefresh; } set { base.ContextMenuRefresh = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSummarySum"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSummarySum { get { return base.ContextMenuSummarySum; } set { base.ContextMenuSummarySum = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSummaryMin"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSummaryMin { get { return base.ContextMenuSummaryMin; } set { base.ContextMenuSummaryMin = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSummaryMax"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSummaryMax { get { return base.ContextMenuSummaryMax; } set { base.ContextMenuSummaryMax = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSummaryAverage"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSummaryAverage { get { return base.ContextMenuSummaryAverage; } set { base.ContextMenuSummaryAverage = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSummaryCount"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSummaryCount { get { return base.ContextMenuSummaryCount; } set { base.ContextMenuSummaryCount = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuSummaryNone"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuSummaryNone { get { return base.ContextMenuSummaryNone; } set { base.ContextMenuSummaryNone = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuNewRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuNewRow { get { return base.ContextMenuNewRow; } set { base.ContextMenuNewRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuEditRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuEditRow { get { return base.ContextMenuEditRow; } set { base.ContextMenuEditRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsContextMenuDeleteRow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ContextMenuDeleteRow { get { return base.ContextMenuDeleteRow; } set { base.ContextMenuDeleteRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandSelectAllOnPage"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandSelectAllOnPage { get { return base.CommandSelectAllOnPage; } set { base.CommandSelectAllOnPage = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandSelectAllOnAllPages"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandSelectAllOnAllPages { get { return base.CommandSelectAllOnAllPages; } set { base.CommandSelectAllOnAllPages = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandDeselectAllOnPage"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandDeselectAllOnPage { get { return base.CommandDeselectAllOnPage; } set { base.CommandDeselectAllOnPage = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewTextSettingsCommandDeselectAllOnAllPages"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandDeselectAllOnAllPages { get { return base.CommandDeselectAllOnAllPages; } set { base.CommandDeselectAllOnAllPages = value; } }
	}
	public enum GridViewAdaptivityMode { Off, HideDataCells, HideDataCellsWindowLimit }
	public enum GridViewAdaptiveColumnPosition { None, Left, Right }
	public class ASPxGridViewAdaptivitySettings : ASPxGridSettingsBase {
		GridViewFormLayoutProperties adaptiveDetailLayoutProperties;
		public ASPxGridViewAdaptivitySettings(ASPxGridView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAdaptivitySettingsAdaptivityMode"),
#endif
		DefaultValue(GridViewAdaptivityMode.Off), NotifyParentProperty(true), AutoFormatEnable]
		public GridViewAdaptivityMode AdaptivityMode {
			get { return (GridViewAdaptivityMode)GetEnumProperty("AdaptivityMode", GridViewAdaptivityMode.Off); }
			set {
				SetEnumProperty("AdaptivityMode", GridViewAdaptivityMode.Off, value);
				Changed();
			}
		}
		[
		DefaultValue(GridViewAdaptiveColumnPosition.Right), NotifyParentProperty(true), AutoFormatEnable]
		public GridViewAdaptiveColumnPosition AdaptiveColumnPosition {
			get { return (GridViewAdaptiveColumnPosition)GetEnumProperty("AdaptiveColumnPosition", GridViewAdaptiveColumnPosition.Right); }
			set {
				SetEnumProperty("AdaptiveColumnPosition", GridViewAdaptiveColumnPosition.Right, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAdaptivitySettingsAdaptiveDetailColumnCount"),
#endif
		DefaultValue(1), NotifyParentProperty(true), AutoFormatEnable]
		public int AdaptiveDetailColumnCount {
			get { return GetIntProperty("AdaptiveDetailColumnCount", 1); }
			set {
				SetIntProperty("AdaptiveDetailColumnCount", 1, value);
				CommonUtils.CheckNegativeOrZeroValue(value, "AdaptiveDetailColumnCount");
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAdaptivitySettingsAdaptiveDetailLayoutProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public GridViewFormLayoutProperties AdaptiveDetailLayoutProperties { 
			get {
				if(adaptiveDetailLayoutProperties == null)
					adaptiveDetailLayoutProperties = CreateEditFormLayoutProperties();
				return adaptiveDetailLayoutProperties; 
			} 
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAdaptivitySettingsAllowOnlyOneAdaptiveDetailExpanded"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool AllowOnlyOneAdaptiveDetailExpanded {
			get { return GetBoolProperty("AllowOnlyOneAdaptiveDetailExpanded", false); }
			set { SetBoolProperty("AllowOnlyOneAdaptiveDetailExpanded", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewAdaptivitySettingsHideDataCellsAtWindowInnerWidth"),
#endif
		DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public int HideDataCellsAtWindowInnerWidth {
			get { return GetIntProperty("HideDataCellsAtWindowInnerWidth", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "HideDataCellsAtWindowInnerWidth");
				SetIntProperty("HideDataCellsAtWindowInnerWidth", 0, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridViewAdaptivitySettings;
				if(src != null) {
					AdaptivityMode = src.AdaptivityMode;
					AdaptiveColumnPosition = src.AdaptiveColumnPosition;
					AdaptiveDetailColumnCount = src.AdaptiveDetailColumnCount;
					AdaptiveDetailLayoutProperties.Assign(src.AdaptiveDetailLayoutProperties);
					AllowOnlyOneAdaptiveDetailExpanded = src.AllowOnlyOneAdaptiveDetailExpanded;
					HideDataCellsAtWindowInnerWidth = src.HideDataCellsAtWindowInnerWidth;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected GridViewFormLayoutProperties CreateEditFormLayoutProperties() {
			GridViewFormLayoutProperties properties = new GridViewFormLayoutProperties();
			properties.IsStandalone = false;
			properties.DataOwner = Owner as IFormLayoutOwner;
			return properties;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { AdaptiveDetailLayoutProperties });
		}
	}
}
