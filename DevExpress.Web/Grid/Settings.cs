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
using DevExpress.Web.Localization;
using DevExpress.Web.Internal;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
namespace DevExpress.Web {
	public enum GridViewPagerMode { ShowAllRecords, ShowPager, EndlessPaging }
	public enum GridViewVerticalScrollBarStyle { Standard, Virtual, VirtualSmooth }
	public enum HeaderFilterMode { List, CheckedList }
	public enum GridHeaderFilterMode { Default, List, CheckedList, DateRangePicker, DateRangeCalendar }
	public enum GridViewFilterRowMode { Auto, OnClick }
	public enum GridViewSelectionStoringMode { PerformanceOptimized, DataIntegrityOptimized };
	public enum GridViewStatusBarMode { Auto, Hidden, Visible }
	public enum GridViewGroupFooterMode { Hidden, VisibleIfExpanded, VisibleAlways };
	public enum GridViewNewItemRowPosition { Top, Bottom }
	public enum GridViewEditingMode { Inline, EditForm, EditFormAndDisplayRow, PopupEditForm, Batch }
	public enum GridViewBatchEditMode { Cell, Row }
	public enum GridViewBatchStartEditAction { Click, DblClick }
	public enum GridViewLoadingPanelMode { Disabled, ShowAsPopup, ShowOnStatusBar, Default }
	public enum GridViewDetailExportMode { None, Expanded, All }
	public enum GridCommandButtonRenderMode { Default, Link, Image, Button }
	public enum GridViewSearchPanelGroupOperator { And, Or }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxGridSettingsBase : PropertiesBase {
		public ASPxGridSettingsBase(ASPxGridBase grid)
			: base(grid) {
		}
		protected ASPxGridBase Grid { get { return (ASPxGridBase)Owner; } }
	}
	public class ASPxGridPagerSettings : PagerSettingsEx {
		public ASPxGridPagerSettings(ASPxGridBase grid)
			: base(grid) {
			Position = PagerPosition.Bottom;
		}
		protected ASPxGridBase Grid { get { return (ASPxGridBase)Owner; } }
		public override SEOFriendlyMode SEOFriendly {
			get { return base.SEOFriendly; }
			set {
				if(base.SEOFriendly == value)
					return;
				base.SEOFriendly = value;
				Changed();
			}
		}
		protected internal GridViewPagerMode Mode {
			get { return (GridViewPagerMode)GetIntProperty("Mode", (int)GridViewPagerMode.ShowPager); }
			set {
				if (value == Mode) return;
				SetIntProperty("Mode", (int)GridViewPagerMode.ShowPager, (int)value);
				Changed();
			}
		}
		protected internal virtual int PageSize { 
			get {
				int pageSize = GetIntProperty("PageSize", 10);
				if(Grid != null)
					pageSize = Grid.CallbackState.Get<int>("PageSize", 10);
				return pageSize;
			}
			set {
				if (value < 0) return;
				if (value == PageSize) return;
				SetIntProperty("PageSize", 10, value);
				if (Grid != null) {
					Grid.CallbackState.Put("PageSize", value);
					Grid.DataProxy.ClearStoredPageSelectionResult(); 
				}
				Changed();
			}
		}
		protected internal bool AlwaysShowPager {
			get { return GetBoolProperty("AlwaysShowPager", false); }
			set {
				if (value == AlwaysShowPager) return;
				SetBoolProperty("AlwaysShowPager", false, value);
				Changed();
			}
		}
		protected internal bool ShowEmptyGridItems {
			get { return GetBoolProperty("ShowEmptyGridItems", false); }
			set {
				if(value == ShowEmptyGridItems) return;
				SetBoolProperty("ShowEmptyGridItems", false, value);
				Changed();
			}
		}
		protected bool ShouldSerializeRenderMode() { return false; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridPagerSettings;
				if (src != null) {
					Mode = src.Mode;
					PageSize = src.PageSize;
					AlwaysShowPager = src.AlwaysShowPager;
					ShowEmptyGridItems = src.ShowEmptyGridItems;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxGridBehaviorSettings : ASPxGridSettingsBase {
		public ASPxGridBehaviorSettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal bool AllowDragDrop {
			get { return GetBoolProperty("AllowDragDrop", true); }
			set {
				if (value == AllowDragDrop) return;
				SetBoolProperty("AllowDragDrop", true, value);
				Changed();
			}
		}
		protected internal bool AllowSort {
			get { return GetBoolProperty("AllowSort", true); }
			set {
				if (value == AllowSort) return;
				SetBoolProperty("AllowSort", true, value);
				Changed();
			}
		}
		protected internal bool AllowEllipsisInText {
			get { return GetBoolProperty("AllowEllipsisInText", false); }
			set {
				if(value == AllowEllipsisInText) return;
				SetBoolProperty("AllowEllipsisInText", false, value);
				Changed();
			}
		}
		protected internal GridViewSelectionStoringMode SelectionStoringMode {
			get { return (GridViewSelectionStoringMode)GetEnumProperty("SelectionStoringMode", GridViewSelectionStoringMode.DataIntegrityOptimized); }
			set { SetEnumProperty("SelectionStoringMode", GridViewSelectionStoringMode.DataIntegrityOptimized, value); }
		}
		protected internal bool ConfirmDelete {
			get { return GetBoolProperty("ConfirmDelete", false); }
			set {
				if (value == ConfirmDelete) return;
				SetBoolProperty("ConfirmDelete", false, value);
				Changed();
			}
		}
		protected internal bool EncodeErrorHtml {
			get { return GetBoolProperty("EncodeErrorHtml", false); }
			set {
				if (value == EncodeErrorHtml) return;
				SetBoolProperty("EncodeErrorHtml", false, value);
				Changed();
			}
		}
		protected internal bool ProcessSelectionChangedOnServer {
			get { return GetBoolProperty("ProcessSelectionChangedOnServer", false); }
			set {
				if (value == ProcessSelectionChangedOnServer) return;
				SetBoolProperty("ProcessSelectionChangedOnServer", false, value);
				Changed();
			}
		}
		protected internal int HeaderFilterMaxRowCount {
			get { return GetIntProperty("HeaderFilterMaxRowCount", -1); }
			set {
				if(value < 0) value = -1;
				if(value == HeaderFilterMaxRowCount) return;
				SetIntProperty("HeaderFilterMaxRowCount", -1, value);
				Changed();
			}
		}
		protected internal ColumnSortMode SortMode {
			get { return (ColumnSortMode)GetEnumProperty("SortMode", ColumnSortMode.Default); }
			set {
				if (value == SortMode) return;
				SetEnumProperty("SortMode", ColumnSortMode.Default, value);
				Changed();
			}
		}
		protected internal bool AllowClientEventsOnLoad {
			get { return GetBoolProperty("AllowClientEventsOnLoad", true); }
			set { SetBoolProperty("AllowClientEventsOnLoad", true, value); }
		}
		protected internal bool EnableCustomizationWindow { 
			get { return GetBoolProperty("EnableCustomizationWindow", false); }
			set {
				if(EnableCustWindowPropertyChanged && value == EnableCustomizationWindow)
					return;
				SetBoolProperty("EnableCustomizationWindow", false, value);
				EnableCustWindowPropertyChanged = true;
				Changed();
			}
		}
		protected internal Unit HeaderFilterHeightInternal {
			get { return (Unit)GetObjectProperty("HeaderFilterHeightInternal", Unit.Empty); }
			set {
				if(value.Equals(HeaderFilterHeightInternal)) return;
				SetObjectProperty("HeaderFilterHeightInternal", Unit.Empty, value);
				Changed();
			}
		}
		protected internal bool EnableCustWindowPropertyChanged {
			get { return GetBoolProperty("EnableCustWindowPropertyChanged", false); }
			set { SetBoolProperty("EnableCustWindowPropertyChanged", false, value); }
		}
		protected internal bool AllowFocusedItem {
			get { return GetBoolProperty("AllowFocusedItem", false); }
			set {
				if(value == AllowFocusedItem) return;
				SetBoolProperty("AllowFocusedItem", false, value);
				if(IsRaiseOnFocusedRowChanged()) {
					Grid.DataProxy.OnFocusedRowChanged();
				}
				Changed();
			}
		}
		protected internal bool AllowSelectByItemClick {
			get { return GetBoolProperty("AllowSelectByItemClick", false); }
			set {
				if(value == AllowSelectByItemClick) return;
				SetBoolProperty("AllowSelectByItemClick", false, value);
				Changed();
			}
		}
		protected internal bool AllowSelectSingleItemOnly {
			get { return GetBoolProperty("AllowSelectSingleItemOnly", false); }
			set {
				if(value == AllowSelectSingleItemOnly) return;
				SetBoolProperty("AllowSelectSingleItemOnly", false, value);
				Changed();
			}
		}
		protected internal bool ProcessFocusedItemChangedOnServer {
			get { return GetBoolProperty("ProcessFocusedItemChangedOnServer", false); }
			set {
				if(value == ProcessFocusedItemChangedOnServer) return;
				SetBoolProperty("ProcessFocusedItemChangedOnServer", false, value);
				Changed();
			}
		}
		protected internal bool EnableItemHotTrack { 
			get { return GetBoolProperty("EnableItemHotTrack", false); } 
			set { SetBoolProperty("EnableItemHotTrack", false, value); } 
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridBehaviorSettings;
				if (src != null) {
					AllowDragDrop = src.AllowDragDrop;
					AllowSort = src.AllowSort;
					AllowEllipsisInText = src.AllowEllipsisInText;
					SelectionStoringMode = src.SelectionStoringMode;
					ConfirmDelete = src.ConfirmDelete;
					EncodeErrorHtml = src.EncodeErrorHtml;
					ProcessSelectionChangedOnServer = src.ProcessSelectionChangedOnServer;
					HeaderFilterHeightInternal = src.HeaderFilterHeightInternal;
					HeaderFilterMaxRowCount = src.HeaderFilterMaxRowCount;
					SortMode = src.SortMode;
					AllowClientEventsOnLoad = src.AllowClientEventsOnLoad;
					if(EnableCustomizationWindow != src.EnableCustomizationWindow)
						EnableCustomizationWindow = src.EnableCustomizationWindow;
					if(EnableCustWindowPropertyChanged != src.EnableCustWindowPropertyChanged)
						EnableCustWindowPropertyChanged = src.EnableCustWindowPropertyChanged;
					AllowFocusedItem = src.AllowFocusedItem;
					AllowSelectByItemClick = src.AllowSelectByItemClick;
					AllowSelectSingleItemOnly = src.AllowSelectSingleItemOnly;
					ProcessFocusedItemChangedOnServer = src.ProcessFocusedItemChangedOnServer;
					EnableItemHotTrack = src.EnableItemHotTrack;
				}
			} finally {
				EndUpdate();
			}
		}
		protected virtual bool IsRaiseOnFocusedRowChanged() {
			return Grid != null && !Grid.IsLoading();
		}
	}
	public class ASPxGridSettings : ASPxGridSettingsBase {
		public ASPxGridSettings(ASPxGridBase grid) : base(grid) { }
		protected internal bool EnableFilterControlPopupMenuScrolling {
			get { return GetBoolProperty("EnableFilterControlPopupMenuScrolling", false); }
			set {
				if (EnableFilterControlPopupMenuScrolling == value)
					return;
				SetBoolProperty("EnableFilterControlPopupMenuScrolling", false, value);
				Changed();
			}
		}
		protected internal bool ShowTitlePanel {
			get { return GetBoolProperty("ShowTitlePanel", false); }
			set {
				if (value == ShowTitlePanel) return;
				SetBoolProperty("ShowTitlePanel", false, value);
				Changed();
			}
		}
		protected internal bool ShowHeaderFilterButton {
			get { return GetBoolProperty("ShowHeaderFilterButton", false); }
			set {
				if (value == ShowHeaderFilterButton) return;
				SetBoolProperty("ShowHeaderFilterButton", false, value);
				Changed();
			}
		}
		protected internal bool ShowHeaderFilterBlankItems {
			get { return GetBoolProperty("ShowHeaderFilterBlankItems", true); }
			set {
				if (value == ShowHeaderFilterBlankItems) return;
				SetBoolProperty("ShowHeaderFilterBlankItems", true, value);
				Changed();
			}
		}
		protected internal int VerticalScrollableHeight {
			get { return GetIntProperty("VerticalScrollableHeight", 200); }
			set {
				value = Math.Max(50, value);
				if (value == VerticalScrollableHeight) return;
				SetIntProperty("VerticalScrollableHeight", 200, value);
				Changed();
			}
		}
		protected internal GridViewVerticalScrollBarStyle VerticalScrollBarStyle {
			get { return (GridViewVerticalScrollBarStyle)GetIntProperty("VerticalScrollBarStyle", (int)GridViewVerticalScrollBarStyle.Standard); }
			set {
				if (value == VerticalScrollBarStyle) return;
				SetIntProperty("VerticalScrollBarStyle", (int)GridViewVerticalScrollBarStyle.Standard, (int)value);
				Changed();
			}
		}
		protected internal GridViewStatusBarMode ShowStatusBar {
			get { return (GridViewStatusBarMode)GetIntProperty("ShowStatusBar", (int)GridViewStatusBarMode.Auto); }
			set {
				if (value == ShowStatusBar) return;
				SetIntProperty("ShowStatusBar", (int)GridViewStatusBarMode.Auto, (int)value);
				Changed();
			}
		}
		protected internal GridViewStatusBarMode ShowFilterBar {
			get { return (GridViewStatusBarMode)GetIntProperty("ShowFilterBar", (int)GridViewStatusBarMode.Hidden); }
			set {
				if (value == ShowFilterBar) return;
				SetIntProperty("ShowFilterBar", (int)GridViewStatusBarMode.Hidden, (int)value);
				Changed();
			}
		}
		protected internal ScrollBarMode HorizontalScrollBarMode {
			get { return (ScrollBarMode)GetEnumProperty("HorizontalScrollBarMode", ScrollBarMode.Hidden); }
			set {
				if(HorizontalScrollBarMode == value)
					return;
				SetEnumProperty("HorizontalScrollBarMode", ScrollBarMode.Hidden, value);
				Changed();
			}
		}
		protected internal ScrollBarMode VerticalScrollBarMode {
			get { return (ScrollBarMode)GetEnumProperty("VerticalScrollBarMode", ScrollBarMode.Hidden); }
			set {
				if(VerticalScrollBarMode == value) return;
				SetEnumProperty("VerticalScrollBarMode", ScrollBarMode.Hidden, value);
				Changed();
			}
		}
		protected internal bool ShowVerticalScrollBarInternal {
			get { return GetBoolProperty("ShowVerticalScrollBar", false); }
			set {
				if(value == ShowVerticalScrollBarInternal)
					return;
				SetBoolProperty("ShowVerticalScrollBar", false, value);
				Changed();
			}
		}
		protected internal bool ShowHorizontalScrollBarInternal {
			get { return GetBoolProperty("ShowHorizontalScrollBar", false); }
			set {
				if(value == ShowHorizontalScrollBarInternal)
					return;
				SetBoolProperty("ShowHorizontalScrollBar", false, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridSettings;
				if(src != null) {
					EnableFilterControlPopupMenuScrolling = src.EnableFilterControlPopupMenuScrolling;
					ShowTitlePanel = src.ShowTitlePanel;
					ShowHeaderFilterButton = src.ShowHeaderFilterButton;
					ShowHeaderFilterBlankItems = src.ShowHeaderFilterBlankItems;
					ShowVerticalScrollBarInternal = src.ShowVerticalScrollBarInternal;
					ShowHorizontalScrollBarInternal = src.ShowHorizontalScrollBarInternal;
					VerticalScrollableHeight = src.VerticalScrollableHeight;
					VerticalScrollBarStyle = src.VerticalScrollBarStyle;
					ShowStatusBar = src.ShowStatusBar;
					ShowFilterBar = src.ShowFilterBar;
					HorizontalScrollBarMode = src.HorizontalScrollBarMode;
					VerticalScrollBarMode = src.VerticalScrollBarMode;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public abstract class ASPxGridEditingSettings : ASPxGridSettingsBase {
		GridBatchEditSettings batchEditSettings;
		public ASPxGridEditingSettings(ASPxGridBase grid) : base(grid) { }
		protected internal GridBatchEditSettings BatchEditSettings {
			get {
				if(batchEditSettings == null)
					batchEditSettings = CreateBatchEditSettings();
				return batchEditSettings;
			}
		}
		protected internal bool UseFormLayout {
			get { return GetBoolProperty("UseFormLayout", true); }
			set {
				SetBoolProperty("UseFormLayout", true, value);
				Changed();
			}
		}
		protected internal GridViewNewItemRowPosition NewItemPosition {
			get { return (GridViewNewItemRowPosition)GetEnumProperty("NewItemPosition", GridViewNewItemRowPosition.Top); }
			set {
				if(value == NewItemPosition) return;
				SetEnumProperty("NewItemPosition", GridViewNewItemRowPosition.Top, value);
				Changed();
			}
		}
		protected virtual GridBatchEditSettings CreateBatchEditSettings() {
			return new GridBatchEditSettings(Grid);
		}
		protected internal abstract bool DisplayEditingRow { get; }
		protected internal abstract bool IsPopupEditForm { get; }
		protected internal abstract bool IsBatchEdit { get; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridEditingSettings;
				if(src != null) {
					NewItemPosition = src.NewItemPosition;
					UseFormLayout = src.UseFormLayout;
					BatchEditSettings.Assign(src.BatchEditSettings);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] { BatchEditSettings });
		}
	}
	public class GridBatchEditSettings : ASPxGridSettingsBase {
		public GridBatchEditSettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal GridViewBatchEditMode EditMode {
			get { return (GridViewBatchEditMode)GetEnumProperty("EditMode", GridViewBatchEditMode.Cell); }
			set {
				if(value == EditMode) return;
				SetEnumProperty("EditMode", GridViewBatchEditMode.Cell, value);
				Changed();
			}
		}
		protected internal GridViewBatchStartEditAction StartEditAction {
			get { return (GridViewBatchStartEditAction)GetEnumProperty("StartEditAction", GridViewBatchStartEditAction.Click); }
			set {
				if(value == StartEditAction) return;
				SetEnumProperty("StartEditAction", GridViewBatchStartEditAction.Click, value);
				Changed();
			}
		}
		protected internal bool ShowConfirmOnLosingChanges {
			get { return GetBoolProperty("ShowConfirmOnLosingChanges", true); }
			set {
				if(value == ShowConfirmOnLosingChanges) return;
				SetBoolProperty("ShowConfirmOnLosingChanges", true, value);
				Changed();
			}
		}
		protected internal bool AllowValidationOnEndEdit {
			get { return GetBoolProperty("AllowValidationOnEndEdit", true); }
			set {
				if(value == AllowValidationOnEndEdit) return;
				SetBoolProperty("AllowValidationOnEndEdit", true, value);
				Changed();
			}
		}
		protected internal bool AllowEndEditOnValidationError {
			get { return GetBoolProperty("AllowEndEditOnValidationError", true); }
			set {
				if(value == AllowEndEditOnValidationError) return;
				SetBoolProperty("AllowEndEditOnValidationError", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as GridBatchEditSettings;
				if(src != null) {
					EditMode = src.EditMode;
					StartEditAction = src.StartEditAction;
					ShowConfirmOnLosingChanges = src.ShowConfirmOnLosingChanges;
					AllowValidationOnEndEdit = src.AllowValidationOnEndEdit;
					AllowEndEditOnValidationError = src.AllowEndEditOnValidationError;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ASPxGridTextSettings : ASPxGridSettingsBase {
		public ASPxGridTextSettings(ASPxGridBase grid) : base(grid) { }
		protected internal string Title {
			get { return GetStringProperty("Title", string.Empty); }
			set {
				if (value == Title) return;
				SetStringProperty("Title", string.Empty, value);
				Changed();
			}
		}
		protected string GroupPanel {
			get { return GetStringProperty("GroupPanel", string.Empty); }
			set {
				if (value == GroupPanel) return;
				SetStringProperty("GroupPanel", string.Empty, value);
				Changed();
			}
		}
		protected string ConfirmDelete {
			get { return GetStringProperty("ConfirmDelete", string.Empty); }
			set {
				if (value == ConfirmDelete) return;
				SetStringProperty("ConfirmDelete", string.Empty, value);
				Changed();
			}
		}
		protected string CustomizationWindowCaption {
			get { return GetStringProperty("CustomizationWindowCaption", string.Empty); }
			set {
				if (value == CustomizationWindowCaption) return;
				SetStringProperty("CustomizationWindowCaption", string.Empty, value);
				Changed();
			}
		}
		protected string PopupEditFormCaption {
			get { return GetStringProperty("PopupEditFormCaption", string.Empty); }
			set {
				if (value == PopupEditFormCaption) return;
				SetStringProperty("PopupEditFormCaption", string.Empty, value);
				Changed();
			}
		}
		protected string EmptyHeaders {
			get { return GetStringProperty("EmptyHeaders", string.Empty); }
			set {
				if (value == EmptyHeaders) return;
				SetStringProperty("EmptyHeaders", string.Empty, value);
				Changed();
			}
		}
		protected string GroupContinuedOnNextPage {
			get { return GetStringProperty("GroupContinuedOnNextPage", string.Empty); }
			set {
				if (value == GroupContinuedOnNextPage) return;
				SetStringProperty("GroupContinuedOnNextPage", string.Empty, value);
				Changed();
			}
		}
		protected string EmptyDataRow {
			get { return GetStringProperty("DataEmptyRow", string.Empty); }
			set {
				if (value == EmptyDataRow) return;
				SetStringProperty("DataEmptyRow", string.Empty, value);
				Changed();
			}
		}
		protected string CommandEdit {
			get { return GetStringProperty("CommandEdit", string.Empty); }
			set {
				if (value == CommandEdit) return;
				SetStringProperty("CommandEdit", string.Empty, value);
				Changed();
			}
		}
		protected string CommandNew {
			get { return GetStringProperty("CommandNew", string.Empty); }
			set {
				if (value == CommandNew) return;
				SetStringProperty("CommandNew", string.Empty, value);
				Changed();
			}
		}
		protected string CommandDelete {
			get { return GetStringProperty("CommandDelete", string.Empty); }
			set {
				if (value == CommandDelete) return;
				SetStringProperty("CommandDelete", string.Empty, value);
				Changed();
			}
		}
		protected string CommandSelect {
			get { return GetStringProperty("CommandSelect", string.Empty); }
			set {
				if (value == CommandSelect) return;
				SetStringProperty("CommandSelect", string.Empty, value);
				Changed();
			}
		}
		protected string CommandCancel {
			get { return GetStringProperty("CommandCancel", string.Empty); }
			set {
				if (value == CommandCancel) return;
				SetStringProperty("CommandCancel", string.Empty, value);
				Changed();
			}
		}
		protected string CommandUpdate {
			get { return GetStringProperty("CommandUpdate", string.Empty); }
			set {
				if (value == CommandUpdate) return;
				SetStringProperty("CommandUpdate", string.Empty, value);
				Changed();
			}
		}
		protected string CommandClearFilter {
			get { return GetStringProperty("CommandClearFilter", string.Empty); }
			set {
				if (value == CommandClearFilter) return;
				SetStringProperty("CommandClearFilter", string.Empty, value);
				Changed();
			}
		}
		protected string CommandApplyFilter {
			get { return GetStringProperty("CommandApplyFilter", string.Empty); }
			set {
				if (value == CommandApplyFilter) return;
				SetStringProperty("CommandApplyFilter", string.Empty, value);
				Changed();
			}
		}
		protected string CommandBatchEditUpdate {
			get { return GetStringProperty("CommandBatchEditUpdate", string.Empty); }
			set {
				if(value == CommandBatchEditUpdate) return;
				SetStringProperty("CommandBatchEditUpdate", string.Empty, value);
				Changed();
			}
		}
		protected string CommandBatchEditCancel {
			get { return GetStringProperty("CommandBatchEditCancel", string.Empty); }
			set {
				if(value == CommandBatchEditCancel) return;
				SetStringProperty("CommandBatchEditCancel", string.Empty, value);
				Changed();
			}
		}
		protected string ConfirmOnLosingBatchChanges {
			get { return GetStringProperty("ConfirmOnLosingBatchChanges", string.Empty); }
			set {
				if(value == ConfirmOnLosingBatchChanges) return;
				SetStringProperty("ConfirmOnLosingBatchChanges", string.Empty, value);
				Changed();
			}
		}
		protected string CommandApplySearchPanelFilter {
			get { return GetStringProperty("CommandApplySearchPanelFilter", string.Empty); }
			set {
				if(value == CommandApplySearchPanelFilter) return;
				SetStringProperty("CommandApplySearchPanelFilter", string.Empty, value);
				Changed();
			}
		}
		protected string CommandClearSearchPanelFilter {
			get { return GetStringProperty("CommandClearSearchPanelFilter", string.Empty); }
			set {
				if(value == CommandClearSearchPanelFilter) return;
				SetStringProperty("CommandClearSearchPanelFilter", string.Empty, value);
				Changed();
			}
		}
		protected string CommandShowAdaptiveDetail {
			get { return GetStringProperty("CommandShowAdaptiveDetail", string.Empty); }
			set {
				if(value == CommandShowAdaptiveDetail)
					return;
				SetStringProperty("CommandShowAdaptiveDetail", string.Empty, value);
				Changed();
			}
		}
		protected string CommandHideAdaptiveDetail {
			get { return GetStringProperty("CommandHideAdaptiveDetail", string.Empty); }
			set {
				if(value == CommandHideAdaptiveDetail)
					return;
				SetStringProperty("CommandHideAdaptiveDetail", string.Empty, value);
				Changed();
			}
		}
		protected string SearchPanelEditorNullText {
			get { return GetStringProperty("SearchPanelEditorNullText", string.Empty); }
			set {
				if(value == SearchPanelEditorNullText) return;
				SetStringProperty("SearchPanelEditorNullText", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterShowAll {
			get { return GetStringProperty("HeaderFilterShowAll", string.Empty); }
			set {
				if (value == HeaderFilterShowAll) return;
				SetStringProperty("HeaderFilterShowAll", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterShowBlanks {
			get { return GetStringProperty("HeaderFilterShowBlanks", string.Empty); }
			set {
				if (value == HeaderFilterShowBlanks) return;
				SetStringProperty("HeaderFilterShowBlanks", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterShowNonBlanks {
			get { return GetStringProperty("HeaderFilterShowNonBlanks", string.Empty); }
			set {
				if (value == HeaderFilterShowNonBlanks) return;
				SetStringProperty("HeaderFilterShowNonBlanks", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterSelectAll {
			get { return GetStringProperty("HeaderFilterSelectAll", string.Empty); }
			set {
				if(value == HeaderFilterSelectAll) return;
				SetStringProperty("HeaderFilterSelectAll", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterYesterday {
			get { return GetStringProperty("HeaderFilterYesterday", string.Empty); }
			set {
				if(value == HeaderFilterYesterday) return;
				SetStringProperty("HeaderFilterYesterday", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterToday {
			get { return GetStringProperty("HeaderFilterToday", string.Empty); }
			set {
				if(value == HeaderFilterToday) return;
				SetStringProperty("HeaderFilterToday", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterTomorrow {
			get { return GetStringProperty("HeaderFilterTomorrow", string.Empty); }
			set {
				if(value == HeaderFilterTomorrow) return;
				SetStringProperty("HeaderFilterTomorrow", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterLastWeek {
			get { return GetStringProperty("HeaderFilterLastWeek", string.Empty); }
			set {
				if(value == HeaderFilterLastWeek) return;
				SetStringProperty("HeaderFilterLastWeek", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterThisWeek {
			get { return GetStringProperty("HeaderFilterThisWeek", string.Empty); }
			set {
				if(value == HeaderFilterThisWeek) return;
				SetStringProperty("HeaderFilterThisWeek", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterNextWeek {
			get { return GetStringProperty("HeaderFilterNextWeek", string.Empty); }
			set {
				if(value == HeaderFilterNextWeek) return;
				SetStringProperty("HeaderFilterNextWeek", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterLastMonth {
			get { return GetStringProperty("HeaderFilterLastMonth", string.Empty); }
			set {
				if(value == HeaderFilterLastMonth) return;
				SetStringProperty("HeaderFilterLastMonth", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterThisMonth {
			get { return GetStringProperty("HeaderFilterThisMonth", string.Empty); }
			set {
				if(value == HeaderFilterThisMonth) return;
				SetStringProperty("HeaderFilterThisMonth", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterNextMonth {
			get { return GetStringProperty("HeaderFilterNextMonth", string.Empty); }
			set {
				if(value == HeaderFilterNextMonth) return;
				SetStringProperty("HeaderFilterNextMonth", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterLastYear {
			get { return GetStringProperty("HeaderFilterLastYear", string.Empty); }
			set {
				if(value == HeaderFilterLastYear) return;
				SetStringProperty("HeaderFilterLastYear", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterThisYear {
			get { return GetStringProperty("HeaderFilterThisYear", string.Empty); }
			set {
				if(value == HeaderFilterThisYear) return;
				SetStringProperty("HeaderFilterThisYear", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterNextYear {
			get { return GetStringProperty("HeaderFilterNextYear", string.Empty); }
			set {
				if(value == HeaderFilterNextYear) return;
				SetStringProperty("HeaderFilterNextYear", string.Empty, value);
				Changed();
			}
		}
		protected internal string FilterControlPopupCaption {
			get { return GetStringProperty("FilterControlPopupCaption", string.Empty); }
			set {
				if (value == FilterControlPopupCaption) return;
				SetStringProperty("FilterControlPopupCaption", string.Empty, value);
				Changed();
			}
		}
		protected internal string FilterBarClear {
			get { return GetStringProperty("FilterBarClear", string.Empty); }
			set {
				if (value == FilterBarClear) return;
				SetStringProperty("FilterBarClear", string.Empty, value);
				Changed();
			}
		}
		protected internal string FilterBarCreateFilter {
			get { return GetStringProperty("FilterBarCreateFilter", string.Empty); }
			set {
				if (value == FilterBarCreateFilter) return;
				SetStringProperty("FilterBarCreateFilter", string.Empty, value);
				Changed();
			}
		}
		protected string HeaderFilterOkButton {
			get { return GetStringProperty("HeaderFilterOkButton", string.Empty); }
			set { SetStringProperty("HeaderFilterOkButton", string.Empty, value); }
		}
		protected string HeaderFilterCancelButton {
			get { return GetStringProperty("HeaderFilterCancelButton", string.Empty); }
			set { SetStringProperty("HeaderFilterCancelButton", string.Empty, value); }
		}
		protected string ContextMenuFullExpand {
			get { return GetStringProperty("ContextMenuFullExpand", string.Empty); }
			set { SetStringProperty("ContextMenuFullExpand", string.Empty, value); }
		}
		protected string ContextMenuFullCollapse {
			get { return GetStringProperty("ContextMenuFullCollapse", string.Empty); }
			set { SetStringProperty("ContextMenuFullCollapse", string.Empty, value); }
		}
		protected string ContextMenuSortAscending {
			get { return GetStringProperty("ContextMenuSortAscending", string.Empty); }
			set { SetStringProperty("ContextMenuSortAscending", string.Empty, value); }
		}
		protected string ContextMenuSortDescending {
			get { return GetStringProperty("ContextMenuSortDescending", string.Empty); }
			set { SetStringProperty("ContextMenuSortDescending", string.Empty, value); }
		}
		protected string ContextMenuClearSorting {
			get { return GetStringProperty("ContextMenuClearSorting", string.Empty); }
			set { SetStringProperty("ContextMenuClearSorting", string.Empty, value); }
		}
		protected string ContextMenuClearFilter {
			get { return GetStringProperty("ContextMenuClearFilter", string.Empty); }
			set { SetStringProperty("ContextMenuClearFilter", string.Empty, value); }
		}
		protected string ContextMenuShowFilterEditor {
			get { return GetStringProperty("ContextMenuShowFilterEditor", string.Empty); }
			set { SetStringProperty("ContextMenuShowFilterEditor", string.Empty, value); }
		}
		protected string ContextMenuShowFilterRow {
			get { return GetStringProperty("ContextMenuShowFilterRow", string.Empty); }
			set { SetStringProperty("ContextMenuShowFilterRow", string.Empty, value); }
		}
		protected string ContextMenuShowFilterRowMenu { 
			get { return GetStringProperty("ContextMenuShowFilterRowMenu", string.Empty); }
			set { SetStringProperty("ContextMenuShowFilterRowMenu", string.Empty, value); }
		}
		protected string ContextMenuShowFooter {
			get { return GetStringProperty("ContextMenuShowFooter", string.Empty); }
			set { SetStringProperty("ContextMenuShowFooter", string.Empty, value); }
		}
		protected string ContextMenuGroupByColumn {
			get { return GetStringProperty("ContextMenuGroupByColumn", string.Empty); }
			set { SetStringProperty("ContextMenuGroupByColumn", string.Empty, value); }
		}
		protected string ContextMenuUngroupColumn {
			get { return GetStringProperty("ContextMenuUngroupColumn", string.Empty); }
			set { SetStringProperty("ContextMenuUngroupColumn", string.Empty, value); }
		}
		protected string ContextMenuClearGrouping {
			get { return GetStringProperty("ContextMenuClearGrouping", string.Empty); }
			set { SetStringProperty("ContextMenuClearGrouping", string.Empty, value); }
		}
		protected string ContextMenuShowGroupPanel {
			get { return GetStringProperty("ContextMenuShowGroupPanel", string.Empty); }
			set { SetStringProperty("ContextMenuShowGroupPanel", string.Empty, value); }
		}
		protected string ContextMenuShowSearchPanel {
			get { return GetStringProperty("ContextMenuShowSearchPanel", string.Empty); }
			set { SetStringProperty("ContextMenuShowSearchPanel", string.Empty, value); }
		}
		protected string ContextMenuShowColumn {
			get { return GetStringProperty("ContextMenuShowColumn", string.Empty); }
			set { SetStringProperty("ContextMenuShowColumn", string.Empty, value); }
		}
		protected string ContextMenuHideColumn {
			get { return GetStringProperty("ContextMenuHideColumn", string.Empty); }
			set { SetStringProperty("ContextMenuHideColumn", string.Empty, value); }
		}
		protected string ContextMenuShowCustomizationWindow { 
			get { return GetStringProperty("ContextMenuShowCustomizationWindow", string.Empty); }
			set { SetStringProperty("ContextMenuShowCustomizationWindow", string.Empty, value); }
		}
		protected string ContextMenuExpandRow {
			get { return GetStringProperty("ContextMenuExpandRow", string.Empty); }
			set { SetStringProperty("ContextMenuExpandRow", string.Empty, value); }
		}
		protected string ContextMenuCollapseRow {
			get { return GetStringProperty("ContextMenuCollapseRow", string.Empty); }
			set { SetStringProperty("ContextMenuCollapseRow", string.Empty, value); }
		}
		protected string ContextMenuExpandDetailRow {
			get { return GetStringProperty("ContextMenuExpandDetailRow", string.Empty); }
			set { SetStringProperty("ContextMenuExpandDetailRow", string.Empty, value); }
		}
		protected string ContextMenuCollapseDetailRow {
			get { return GetStringProperty("ContextMenuCollapseDetailRow", string.Empty); }
			set { SetStringProperty("ContextMenuCollapseDetailRow", string.Empty, value); }
		}
		protected string ContextMenuRefresh {
			get { return GetStringProperty("ContextMenuRefresh", string.Empty); }
			set { SetStringProperty("ContextMenuRefresh", string.Empty, value); }
		}
		protected string ContextMenuSummarySum {
			get { return GetStringProperty("ContextMenuSummarySum", string.Empty); }
			set { SetStringProperty("ContextMenuSummarySum", string.Empty, value); }
		}
		protected string ContextMenuSummaryMin {
			get { return GetStringProperty("ContextMenuSummaryMin", string.Empty); }
			set { SetStringProperty("ContextMenuSummaryMin", string.Empty, value); }
		}
		protected string ContextMenuSummaryMax {
			get { return GetStringProperty("ContextMenuSummaryMax", string.Empty); }
			set { SetStringProperty("ContextMenuSummaryMax", string.Empty, value); }
		}
		protected string ContextMenuSummaryAverage {
			get { return GetStringProperty("ContextMenuSummaryAverage", string.Empty); }
			set { SetStringProperty("ContextMenuSummaryAverage", string.Empty, value); }
		}
		protected string ContextMenuSummaryCount {
			get { return GetStringProperty("ContextMenuSummaryCount", string.Empty); }
			set { SetStringProperty("ContextMenuSummaryCount", string.Empty, value); }
		}
		protected string ContextMenuSummaryNone {
			get { return GetStringProperty("ContextMenuSummaryNone", string.Empty); }
			set { SetStringProperty("ContextMenuSummaryNone", string.Empty, value); }
		}
		protected string ContextMenuNewRow {
			get { return GetStringProperty("ContextMenuNewRow", string.Empty); }
			set { SetStringProperty("ContextMenuNewRow", string.Empty, value); }
		}
		protected string ContextMenuEditRow {
			get { return GetStringProperty("ContextMenuEditRow", string.Empty); }
			set { SetStringProperty("ContextMenuEditRow", string.Empty, value); }
		}
		protected string ContextMenuDeleteRow {
			get { return GetStringProperty("ContextMenuDeleteRow", string.Empty); }
			set { SetStringProperty("ContextMenuDeleteRow", string.Empty, value); }
		}
		protected string CommandSelectAllOnPage {
			get { return GetStringProperty("CommandSelectAllOnPage", string.Empty); }
			set { SetStringProperty("CommandSelectAllOnPage", string.Empty, value); }
		}
		protected string CommandSelectAllOnAllPages {
			get { return GetStringProperty("CommandSelectAllOnAllPages", string.Empty); }
			set { SetStringProperty("CommandSelectAllOnAllPages", string.Empty, value); }
		}
		protected string CommandDeselectAllOnPage {
			get { return GetStringProperty("CommandDeselectAllOnPage", string.Empty); }
			set { SetStringProperty("CommandDeselectAllOnPage", string.Empty, value); }
		}
		protected string CommandDeselectAllOnAllPages {
			get { return GetStringProperty("CommandDeselectAllOnAllPages", string.Empty); }
			set { SetStringProperty("CommandDeselectAllOnAllPages", string.Empty, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridTextSettings;
				if (src != null) {
					Title = src.Title;
					GroupPanel = src.GroupPanel;
					ConfirmDelete = src.ConfirmDelete;
					CustomizationWindowCaption = src.CustomizationWindowCaption;
					PopupEditFormCaption = src.PopupEditFormCaption;
					EmptyHeaders = src.EmptyHeaders;
					GroupContinuedOnNextPage = src.GroupContinuedOnNextPage;
					EmptyDataRow = src.EmptyDataRow;
					CommandEdit = src.CommandEdit;
					CommandNew = src.CommandNew;
					CommandDelete = src.CommandDelete;
					CommandSelect = src.CommandSelect;
					CommandCancel = src.CommandCancel;
					CommandUpdate = src.CommandUpdate;
					CommandClearFilter = src.CommandClearFilter;
					CommandApplyFilter = src.CommandApplyFilter;
					CommandBatchEditUpdate = src.CommandBatchEditUpdate;
					CommandBatchEditCancel = src.CommandBatchEditCancel;
					CommandApplySearchPanelFilter = src.CommandApplySearchPanelFilter;
					CommandClearSearchPanelFilter = src.CommandClearSearchPanelFilter;
					CommandShowAdaptiveDetail = src.CommandShowAdaptiveDetail;
					CommandHideAdaptiveDetail = src.CommandHideAdaptiveDetail;
					ConfirmOnLosingBatchChanges = src.ConfirmOnLosingBatchChanges;
					SearchPanelEditorNullText = src.SearchPanelEditorNullText;
					HeaderFilterShowAll = src.HeaderFilterShowAll;
					HeaderFilterShowBlanks = src.HeaderFilterShowBlanks;
					HeaderFilterShowNonBlanks = src.HeaderFilterShowNonBlanks;
					HeaderFilterYesterday = src.HeaderFilterYesterday;
					HeaderFilterToday = src.HeaderFilterToday;
					HeaderFilterTomorrow = src.HeaderFilterTomorrow;
					HeaderFilterLastWeek = src.HeaderFilterLastWeek;
					HeaderFilterThisWeek = src.HeaderFilterThisWeek;
					HeaderFilterNextWeek = src.HeaderFilterNextWeek;
					HeaderFilterLastMonth = src.HeaderFilterLastMonth;
					HeaderFilterThisMonth = src.HeaderFilterThisMonth;
					HeaderFilterNextMonth = src.HeaderFilterNextMonth;
					HeaderFilterLastYear = src.HeaderFilterLastYear;
					HeaderFilterThisYear = src.HeaderFilterThisYear;
					HeaderFilterNextYear = src.HeaderFilterNextYear;
					FilterControlPopupCaption = src.FilterControlPopupCaption;
					FilterBarClear = src.FilterBarClear;
					FilterBarCreateFilter = src.FilterBarCreateFilter;
					HeaderFilterOkButton = src.HeaderFilterOkButton;
					HeaderFilterCancelButton = src.HeaderFilterCancelButton;
					HeaderFilterSelectAll = src.HeaderFilterSelectAll;
					ContextMenuFullExpand = src.ContextMenuFullExpand;
					ContextMenuFullCollapse = src.ContextMenuFullCollapse;
					ContextMenuSortAscending = src.ContextMenuSortAscending;
					ContextMenuSortDescending = src.ContextMenuSortDescending;
					ContextMenuClearSorting = src.ContextMenuClearSorting;
					ContextMenuClearFilter = src.ContextMenuClearFilter;
					ContextMenuShowFilterEditor = src.ContextMenuShowFilterEditor;
					ContextMenuShowFilterRow = src.ContextMenuShowFilterRow;
					ContextMenuShowFilterRowMenu = src.ContextMenuShowFilterRowMenu;
					ContextMenuGroupByColumn = src.ContextMenuGroupByColumn;
					ContextMenuUngroupColumn = src.ContextMenuUngroupColumn;
					ContextMenuClearGrouping = src.ContextMenuClearGrouping;
					ContextMenuShowGroupPanel = src.ContextMenuShowGroupPanel;
					ContextMenuShowSearchPanel = src.ContextMenuShowSearchPanel;
					ContextMenuShowColumn = src.ContextMenuShowColumn;
					ContextMenuHideColumn = src.ContextMenuHideColumn;
					ContextMenuShowCustomizationWindow = src.ContextMenuShowCustomizationWindow;
					ContextMenuExpandRow = src.ContextMenuExpandRow;
					ContextMenuCollapseRow = src.ContextMenuCollapseRow;
					ContextMenuExpandDetailRow = src.ContextMenuExpandDetailRow;
					ContextMenuCollapseDetailRow = src.ContextMenuCollapseDetailRow;
					ContextMenuSummarySum = src.ContextMenuSummarySum;
					ContextMenuSummaryMin = src.ContextMenuSummaryMin;
					ContextMenuSummaryMax = src.ContextMenuSummaryMax;
					ContextMenuSummaryAverage = src.ContextMenuSummaryAverage;
					ContextMenuSummaryCount = src.ContextMenuSummaryCount;
					ContextMenuSummaryNone = src.ContextMenuSummaryNone;
					ContextMenuRefresh = src.ContextMenuRefresh;
					ContextMenuNewRow = src.ContextMenuNewRow;
					ContextMenuEditRow = src.ContextMenuEditRow;
					ContextMenuDeleteRow = src.ContextMenuDeleteRow;
					ContextMenuShowFooter = src.ContextMenuShowFooter;
					CommandSelectAllOnPage = src.CommandSelectAllOnPage;
					CommandSelectAllOnAllPages = src.CommandSelectAllOnAllPages;
					CommandDeselectAllOnPage = src.CommandDeselectAllOnPage;
					CommandDeselectAllOnAllPages = src.CommandDeselectAllOnAllPages;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal string GetGroupPanel() {
			if (!string.IsNullOrEmpty(GroupPanel)) return GroupPanel;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.GroupPanel);
		}
		protected internal virtual string GetCustomizationWindowCaption() {
			if (!string.IsNullOrEmpty(CustomizationWindowCaption)) return CustomizationWindowCaption;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CustomizationWindowCaption);
		}
		protected internal string GetPopupEditFormCaption() {
			if (!string.IsNullOrEmpty(PopupEditFormCaption)) return PopupEditFormCaption;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.PopupEditFormCaption);
		}
		protected internal string GetConfirmDelete() {
			if (!string.IsNullOrEmpty(ConfirmDelete)) return ConfirmDelete;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ConfirmDelete);
		}
		protected internal string GetConfirmBatchUpdate() {
			if(!string.IsNullOrEmpty(ConfirmOnLosingBatchChanges)) return ConfirmOnLosingBatchChanges;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ConfirmOnLosingBatchChanges);
		}
		protected internal string GetEmptyHeaders() {
			if (!string.IsNullOrEmpty(EmptyHeaders)) return EmptyHeaders;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.EmptyHeaders);
		}
		protected internal string GetGroupContinuedOnNextPage() {
			if (!string.IsNullOrEmpty(GroupContinuedOnNextPage)) return GroupContinuedOnNextPage;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.GroupContinuedOnNextPage);
		}
		protected internal string GetEmptyDataRow() {
			if (!string.IsNullOrEmpty(EmptyDataRow)) return EmptyDataRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.EmptyDataRow);
		}
		protected internal string GetSearchPanelEditorNullText() {
			if(!string.IsNullOrEmpty(SearchPanelEditorNullText)) return SearchPanelEditorNullText;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.SearchPanelEditorNullText);
		}
		protected internal string GetHeaderFilterShowAll() {
			if (!string.IsNullOrEmpty(HeaderFilterShowAll)) return HeaderFilterShowAll;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowAllItem);
		}
		protected internal string GetHeaderFilterShowBlanks() {
			if (!string.IsNullOrEmpty(HeaderFilterShowBlanks)) return HeaderFilterShowBlanks;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowBlanksItem);
		}
		protected internal string GetHeaderFilterShowNonBlanks() {
			if (!string.IsNullOrEmpty(HeaderFilterShowNonBlanks)) return HeaderFilterShowNonBlanks;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowNonBlanksItem);
		}
		protected internal string GetHeaderFilterSelectAll() {
			if(!string.IsNullOrEmpty(HeaderFilterSelectAll)) return HeaderFilterSelectAll;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterSelectAll);
		}
		protected internal string GetHeaderFilterYesterday() {
			if(!string.IsNullOrEmpty(HeaderFilterYesterday)) return HeaderFilterYesterday;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterYesterday);
		}
		protected internal string GetHeaderFilterToday() {
			if(!string.IsNullOrEmpty(HeaderFilterToday)) return HeaderFilterToday;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterToday);
		}
		protected internal string GetHeaderFilterTomorrow() {
			if(!string.IsNullOrEmpty(HeaderFilterTomorrow)) return HeaderFilterTomorrow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterTomorrow);
		}
		protected internal string GetHeaderFilterLastWeek() {
			if(!string.IsNullOrEmpty(HeaderFilterLastWeek)) return HeaderFilterLastWeek;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterLastWeek);
		}
		protected internal string GetHeaderFilterThisWeek() {
			if(!string.IsNullOrEmpty(HeaderFilterThisWeek)) return HeaderFilterThisWeek;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterThisWeek);
		}
		protected internal string GetHeaderFilterNextWeek() {
			if(!string.IsNullOrEmpty(HeaderFilterNextWeek)) return HeaderFilterNextWeek;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterNextWeek);
		}
		protected internal string GetHeaderFilterLastMonth() {
			if(!string.IsNullOrEmpty(HeaderFilterLastMonth)) return HeaderFilterLastMonth;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterLastMonth);
		}
		protected internal string GetHeaderFilterThisMonth() {
			if(!string.IsNullOrEmpty(HeaderFilterThisMonth)) return HeaderFilterThisMonth;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterThisMonth);
		}
		protected internal string GetHeaderFilterNextMonth() {
			if(!string.IsNullOrEmpty(HeaderFilterNextMonth)) return HeaderFilterNextMonth;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterNextMonth);
		}
		protected internal string GetHeaderFilterLastYear() {
			if(!string.IsNullOrEmpty(HeaderFilterLastYear)) return HeaderFilterLastYear;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterLastYear);
		}
		protected internal string GetHeaderFilterThisYear() {
			if(!string.IsNullOrEmpty(HeaderFilterThisYear)) return HeaderFilterThisYear;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterThisYear);
		}
		protected internal string GetHeaderFilterNextYear() {
			if(!string.IsNullOrEmpty(HeaderFilterNextYear)) return HeaderFilterNextYear;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterNextYear);
		}
		protected internal string GetHeaderFilterOkButton() {
			if(!string.IsNullOrEmpty(HeaderFilterOkButton)) return HeaderFilterOkButton;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterOkButton);
		}
		protected internal string GetHeaderFilterCancelButton() {
			if(!string.IsNullOrEmpty(HeaderFilterCancelButton)) return HeaderFilterCancelButton;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterCancelButton);
		}
		protected internal string GetContextMenuFullExpand() {
			if(!string.IsNullOrEmpty(ContextMenuFullExpand)) return ContextMenuFullExpand;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_FullExpand);
		}
		protected internal string GetContextMenuFullCollapse() {
			if(!string.IsNullOrEmpty(ContextMenuFullCollapse)) return ContextMenuFullCollapse;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_FullCollapse);
		}
		protected internal string GetContextMenuSortAscending() {
			if(!string.IsNullOrEmpty(ContextMenuSortAscending)) return ContextMenuSortAscending;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SortAscending);
		}
		protected internal string GetContextMenuSortDescending() {
			if(!string.IsNullOrEmpty(ContextMenuSortDescending)) return ContextMenuSortDescending;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SortDescending);
		}
		protected internal string GetContextMenuClearSorting() {
			if(!string.IsNullOrEmpty(ContextMenuClearSorting)) return ContextMenuClearSorting;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ClearSorting);
		}
		protected internal string GetContextMenuClearFilter() {
			if(!string.IsNullOrEmpty(ContextMenuClearFilter)) return ContextMenuClearFilter;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ClearFilter);
		}
		protected internal string GetContextMenuShowFilterEditor() {
			if(!string.IsNullOrEmpty(ContextMenuShowFilterEditor)) return ContextMenuShowFilterEditor;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowFilterEditor);
		}
		protected internal string GetContextMenuShowFilterRow() {
			if(!string.IsNullOrEmpty(ContextMenuShowFilterRow)) return ContextMenuShowFilterRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowFilterRow);
		}
		protected internal string GetContextMenuShowFilterRowMenu() {
			if(!string.IsNullOrEmpty(ContextMenuShowFilterRowMenu)) return ContextMenuShowFilterRowMenu;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowFilterRowMenu);
		}
		protected internal string GetContextMenuShowFooter() { 
			if(!string.IsNullOrEmpty(ContextMenuShowFooter)) return ContextMenuShowFooter;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowFooter);
		}
		protected internal string GetContextMenuGroupByColumn() {
			if(!string.IsNullOrEmpty(ContextMenuGroupByColumn)) return ContextMenuGroupByColumn;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_GroupByColumn);
		}
		protected internal string GetContextMenuUngroupColumn() {
			if(!string.IsNullOrEmpty(ContextMenuUngroupColumn)) return ContextMenuUngroupColumn;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_UngroupColumn);
		}
		protected internal string GetContextMenuClearGrouping() {
			if(!string.IsNullOrEmpty(ContextMenuClearGrouping)) return ContextMenuClearGrouping;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ClearGrouping);
		}
		protected internal string GetContextMenuShowGroupPanel() {
			if(!string.IsNullOrEmpty(ContextMenuShowGroupPanel)) return ContextMenuShowGroupPanel;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowGroupPanel);
		}
		protected internal string GetContextMenuShowSearchPanel() {
			if(!string.IsNullOrEmpty(ContextMenuShowSearchPanel)) return ContextMenuShowSearchPanel;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowSearchPanel);
		}
		protected internal string GetContextMenuShowColumn() {
			if(!string.IsNullOrEmpty(ContextMenuShowColumn)) return ContextMenuShowColumn;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowColumn);
		}
		protected internal string GetContextMenuHideColumn() {
			if(!string.IsNullOrEmpty(ContextMenuHideColumn)) return ContextMenuHideColumn;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_HideColumn);
		}
		protected internal string GetContextMenuShowCustomizationWindow() {
			if(!string.IsNullOrEmpty(ContextMenuShowCustomizationWindow)) return ContextMenuShowCustomizationWindow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ShowCustomizationWindow);
		}
		protected internal string GetContextMenuExpandRow() {
			if(!string.IsNullOrEmpty(ContextMenuExpandRow)) return ContextMenuExpandRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ExpandRow);
		}
		protected internal string GetContextMenuCollapseRow() {
			if(!string.IsNullOrEmpty(ContextMenuCollapseRow)) return ContextMenuCollapseRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_CollapseRow);
		}
		protected internal string GetContextMenuExpandDetailRow() {
			if(!string.IsNullOrEmpty(ContextMenuExpandDetailRow)) return ContextMenuExpandDetailRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_ExpandDetailRow);
		}
		protected internal string GetContextMenuCollapseDetailRow() {
			if(!string.IsNullOrEmpty(ContextMenuCollapseDetailRow)) return ContextMenuCollapseDetailRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_CollapseDetailRow);
		}
		protected internal string GetContextMenuRefresh() {
			if(!string.IsNullOrEmpty(ContextMenuRefresh)) return ContextMenuRefresh;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_Refresh);
		}
		protected internal string GetContextMenuSummarySum() {
			if(!string.IsNullOrEmpty(ContextMenuSummarySum)) return ContextMenuSummarySum;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SummarySum);
		}
		protected internal string GetContextMenuSummaryMin() {
			if(!string.IsNullOrEmpty(ContextMenuSummaryMin)) return ContextMenuSummaryMin;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SummaryMin);
		}
		protected internal string GetContextMenuSummaryMax() {
			if(!string.IsNullOrEmpty(ContextMenuSummaryMax)) return ContextMenuSummaryMax;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SummaryMax);
		}
		protected internal string GetContextMenuSummaryAverage() {
			if(!string.IsNullOrEmpty(ContextMenuSummaryAverage)) return ContextMenuSummaryAverage;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SummaryAverage);
		}
		protected internal string GetContextMenuSummaryCount() {
			if(!string.IsNullOrEmpty(ContextMenuSummaryCount)) return ContextMenuSummaryCount;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SummaryCount);
		}
		protected internal string GetContextMenuSummaryNone() {
			if(!string.IsNullOrEmpty(ContextMenuSummaryNone)) return ContextMenuSummaryNone;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_SummaryNone);
		}
		protected internal string GetContextMenuNewRow() {
			if(!string.IsNullOrEmpty(ContextMenuNewRow)) return ContextMenuNewRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_NewRow);
		}
		protected internal string GetContextMenuEditRow() {
			if(!string.IsNullOrEmpty(ContextMenuEditRow)) return ContextMenuEditRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_EditRow);
		}
		protected internal string GetContextMenuDeleteRow() {
			if(!string.IsNullOrEmpty(ContextMenuDeleteRow)) return ContextMenuDeleteRow;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.ContextMenu_DeleteRow);
		}
		protected internal string GetCommandButtonText(GridCommandButtonType buttonType) {
			return GetCommandButtonText(buttonType, false);
		}
		protected internal virtual string GetCommandButtonText(GridCommandButtonType buttonType, bool isBatchEditMode) {
			Dictionary<string, string> values = new Dictionary<string, string>();
			values[GridCommandButtonType.Edit.ToString()] = CommandEdit;
			values[GridCommandButtonType.New.ToString()] = CommandNew;
			values[GridCommandButtonType.Delete.ToString()] = CommandDelete;
			values[GridCommandButtonType.Select.ToString()] = CommandSelect;
			values[GridCommandButtonType.Cancel.ToString()] = CommandCancel;
			values[GridCommandButtonType.Update.ToString()] = CommandUpdate;
			values[GridCommandButtonType.ClearFilter.ToString()] = CommandClearFilter;
			values[GridCommandButtonType.ApplyFilter.ToString()] = CommandApplyFilter;
			values[GridCommandButtonType.Update.ToString() + "B"] = CommandBatchEditUpdate;
			values[GridCommandButtonType.Cancel.ToString() + "B"] = CommandBatchEditCancel;
			values[GridCommandButtonType.ApplySearchPanelFilter.ToString()] = CommandApplySearchPanelFilter;
			values[GridCommandButtonType.ClearSearchPanelFilter.ToString()] = CommandClearSearchPanelFilter;
			values[GridCommandButtonType.ShowAdaptiveDetail.ToString()] = CommandShowAdaptiveDetail;
			values[GridCommandButtonType.HideAdaptiveDetail.ToString()] = CommandHideAdaptiveDetail;
			var key = buttonType.ToString();
			if(isBatchEditMode && (buttonType == GridCommandButtonType.Update || buttonType == GridCommandButtonType.Cancel))
				key += "B";
			if(values.ContainsKey(key) && !string.IsNullOrEmpty(values[key]))
				return values[key];
			return GetCommandButtonDefaultText(buttonType, isBatchEditMode);
		}
		protected static internal string GetCommandButtonDefaultText(GridCommandButtonType buttonType) {
			return GetCommandButtonDefaultText(buttonType, false);
		}
		protected static internal string GetCommandButtonDefaultText(GridCommandButtonType buttonType, bool isBatchEditMode) {
			switch (buttonType) {
				case GridCommandButtonType.ShowAdaptiveDetail:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandShowAdaptiveDetail);
				case GridCommandButtonType.HideAdaptiveDetail:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandHideAdaptiveDetail);
				case GridCommandButtonType.ClearFilter:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandClearFilter);
				case GridCommandButtonType.ApplyFilter:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandApplyFilter);
				case GridCommandButtonType.Delete:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandDelete);
				case GridCommandButtonType.Edit:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandEdit);
				case GridCommandButtonType.New:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandNew);
				case GridCommandButtonType.Select:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandSelect);
				case GridCommandButtonType.Update:
					return ASPxGridViewLocalizer.GetString(isBatchEditMode ? ASPxGridViewStringId.CommandBatchEditUpdate : ASPxGridViewStringId.CommandUpdate);
				case GridCommandButtonType.Cancel:
					return ASPxGridViewLocalizer.GetString(isBatchEditMode ? ASPxGridViewStringId.CommandBatchEditCancel : ASPxGridViewStringId.CommandCancel);
				case GridCommandButtonType.ApplySearchPanelFilter:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandApplySearchPanelFilter);
				case GridCommandButtonType.ClearSearchPanelFilter:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandClearSearchPanelFilter);
			}
			return string.Empty;
		}
		protected internal string GetSelectAllCheckboxTooltip(GridViewSelectAllCheckBoxMode mode) {
			switch(mode) {
				case GridViewSelectAllCheckBoxMode.Page:
					return GetCommandSelectAllOnPage();
				case GridViewSelectAllCheckBoxMode.AllPages:
					return GetCommandSelectAllOnAllPages();
			}
			return string.Empty;
		}
		protected internal string GetUnselectAllCheckboxTooltip(GridViewSelectAllCheckBoxMode mode) {
			switch(mode) {
				case GridViewSelectAllCheckBoxMode.Page:
					return GetCommandDeselectAllOnPage();
				case GridViewSelectAllCheckBoxMode.AllPages:
					return GetCommandDeselectAllOnAllPages();
			}
			return string.Empty;
		}
		string GetCommandSelectAllOnPage() {
			if(!string.IsNullOrEmpty(CommandSelectAllOnPage)) return CommandSelectAllOnPage;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandSelectAllOnPage);
		}
		string GetCommandSelectAllOnAllPages() {
			if(!string.IsNullOrEmpty(CommandSelectAllOnAllPages)) return CommandSelectAllOnAllPages;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandSelectAllOnAllPages);
		}
		string GetCommandDeselectAllOnPage() {
			if(!string.IsNullOrEmpty(CommandDeselectAllOnPage)) return CommandDeselectAllOnPage;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandDeselectAllOnPage);
		}
		string GetCommandDeselectAllOnAllPages() {
			if(!string.IsNullOrEmpty(CommandDeselectAllOnAllPages)) return CommandDeselectAllOnAllPages;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CommandDeselectAllOnAllPages);
		}
	}
	public class ASPxGridLoadingPanelSettings : SettingsLoadingPanel {
		public ASPxGridLoadingPanelSettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal GridViewLoadingPanelMode Mode {
			get { return (GridViewLoadingPanelMode)GetEnumProperty("Mode", GridViewLoadingPanelMode.Default); }
			set {
				SetEnumProperty("Mode", GridViewLoadingPanelMode.Default, value);
				Changed();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxGridLoadingPanelSettings;
			if(src != null)
				Mode = src.Mode;
		}
	}
	public class ASPxGridCookiesSettings : ASPxGridSettingsBase {
		public ASPxGridCookiesSettings(ASPxGridBase grid) 
			: base(grid) { 
		}
		protected internal bool Enabled {
			get { return GetBoolProperty("Enabled", false); }
			set {
				SetBoolProperty("Enabled", false, value);
				Changed();
			}
		}
		protected internal string CookiesID {
			get { return GetStringProperty("CookiesID", string.Empty); }
			set {
				SetStringProperty("CookiesID", string.Empty, value);
				Changed();
			}
		}
		protected internal string Version {
			get { return GetStringProperty("Version", string.Empty); }
			set {
				SetStringProperty("Version", string.Empty, value);
				Changed();
			}
		}
		protected internal bool StorePaging {
			get { return GetBoolProperty("StorePaging", true); }
			set {
				SetBoolProperty("StorePaging", true, value);
				Changed();
			}
		}
		protected internal bool StoreFiltering {
			get { return GetBoolProperty("StoreFiltering", true); }
			set {
				SetBoolProperty("StoreFiltering", true, value);
				Changed();
			}
		}
		protected internal bool StoreSearchPanelFiltering {
			get { return GetBoolProperty("StoreSearchPanelFiltering", true); }
			set {
				SetBoolProperty("StoreSearchPanelFiltering", true, value);
				Changed();
			}
		}
		protected internal bool StoreGroupingAndSortingInternal {
			get { return GetBoolProperty("StoreGroupingAndSortingInternal", true); }
			set {
				SetBoolProperty("StoreGroupingAndSortingInternal", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxGridCookiesSettings;
			if (src != null) {
				Enabled = src.Enabled;
				CookiesID = src.CookiesID;
				Version = src.Version;
				StorePaging = src.StorePaging;
				StoreGroupingAndSortingInternal = src.StoreGroupingAndSortingInternal;
				StoreFiltering = src.StoreFiltering;
				StoreSearchPanelFiltering = src.StoreSearchPanelFiltering;
			}
		}
	}
	public class ASPxGridCommandButtonSettings : ASPxGridSettingsBase {
		GridCommandButtonSettings newButton, editButton, updateButton, cancelButton, deleteButton,
			selectButton, searchPanelApplyButton, searchPanelClearButton;
		public ASPxGridCommandButtonSettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal GridCommandButtonSettings NewButton {
			get {
				if(newButton == null)
					newButton = CreateButtonSettings();
				return newButton;
			}
		}
		protected internal GridCommandButtonSettings UpdateButton {
			get {
				if(updateButton == null)
					updateButton = CreateButtonSettings();
				return updateButton;
			}
		}
		protected internal GridCommandButtonSettings CancelButton {
			get {
				if(cancelButton == null)
					cancelButton = CreateButtonSettings();
				return cancelButton;
			}
		}
		protected internal GridCommandButtonSettings EditButton {
			get {
				if(editButton == null)
					editButton = CreateButtonSettings();
				return editButton;
			}
		}
		protected internal GridCommandButtonSettings DeleteButton {
			get {
				if(deleteButton == null)
					deleteButton = CreateButtonSettings();
				return deleteButton;
			}
		}
		protected internal GridCommandButtonSettings SelectButton {
			get {
				if(selectButton == null)
					selectButton = CreateButtonSettings();
				return selectButton;
			}
		}
		protected internal GridCommandButtonSettings SearchPanelApplyButton {
			get {
				if(searchPanelApplyButton == null)
					searchPanelApplyButton = CreateButtonSettings();
				return searchPanelApplyButton;
			}
		}
		protected internal GridCommandButtonSettings SearchPanelClearButton {
			get {
				if(searchPanelClearButton == null)
					searchPanelClearButton = CreateButtonSettings();
				return searchPanelClearButton;
			}
		}
		protected internal bool EncodeHtml {
			get { return GetBoolProperty("EncodeHtml", false); }
			set { 
				if(EncodeHtml != value)
					SetBoolProperty("EncodeHtml", false, value); 
			}
		}
		protected virtual GridCommandButtonSettings CreateButtonSettings() {
			return new GridCommandButtonSettings(Grid);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridCommandButtonSettings;
				if(src != null) {
					UpdateButton.Assign(src.UpdateButton);
					CancelButton.Assign(src.CancelButton);
					NewButton.Assign(src.NewButton);
					EditButton.Assign(src.EditButton);
					DeleteButton.Assign(src.DeleteButton);
					SelectButton.Assign(src.SelectButton);
					SearchPanelApplyButton.Assign(src.SearchPanelApplyButton);
					SearchPanelClearButton.Assign(src.SearchPanelClearButton);
					EncodeHtml = src.EncodeHtml;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				NewButton,
				UpdateButton,
				CancelButton,
				EditButton,
				DeleteButton,
				SelectButton,
				SearchPanelApplyButton,
				SearchPanelClearButton
			});
		}
	}
	public class ASPxGridDataSecuritySettings : ASPxGridSettingsBase {
		public ASPxGridDataSecuritySettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal bool AllowInsert {
			get { return GetBoolProperty("AllowInsert", true); }
			set {
				if(AllowInsert == value) return;
				SetBoolProperty("AllowInsert", true, value);
				Changed();
			}
		}
		protected internal bool AllowEdit {
			get { return GetBoolProperty("AllowEdit", true); }
			set {
				if(AllowEdit == value) return;
				SetBoolProperty("AllowEdit", true, value);
				Changed();
			}
		}
		protected internal bool AllowDelete {
			get { return GetBoolProperty("AllowDelete", true); }
			set {
				if(AllowDelete == value) return;
				SetBoolProperty("AllowDelete", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxGridDataSecuritySettings;
			if(src != null) {
				AllowInsert = src.AllowInsert;
				AllowEdit = src.AllowEdit;
				AllowDelete = src.AllowDelete;
			}
		}
	}
	public class GridCommandButtonSettings : ASPxGridSettingsBase {
		public GridCommandButtonSettings(ASPxGridBase grid)
			: base(grid) {
			Image = new ImageProperties(Grid);
			Styles = new ButtonControlStyles(Grid);
		}
		protected internal GridCommandButtonRenderMode ButtonType {
			get { return (GridCommandButtonRenderMode)GetEnumProperty("ButtonType", DefaultButtonType); }
			set {
				if(ButtonType == value) return;
				SetEnumProperty("ButtonType", DefaultButtonType, value);
				Changed();
			}
		}
		protected internal string Text {
			get { return GetStringProperty("Text", string.Empty); }
			set {
				if(value == Text) return;
				SetStringProperty("Text", string.Empty, value);
				Changed();
			}
		}
		protected internal ImageProperties Image { get; private set; }
		protected internal ButtonControlStyles Styles { get; private set; }
		protected virtual GridCommandButtonRenderMode DefaultButtonType { get { return GridCommandButtonRenderMode.Default; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as GridCommandButtonSettings;
			if(src != null) {
				ButtonType = src.ButtonType;
				Text = src.Text;
				Image.Assign(src.Image);
				Styles.CopyFrom(src.Styles);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() { return new IStateManager[] { Image, Styles }; }
	}
	public class ASPxGridPopupControlSettings : ASPxGridSettingsBase {
		GridEditFormPopupSettings editForm;
		GridCustomizationWindowPopupSettings customizationWindow;
		GridHeaderFilterPopupSettings headerFilter;
		public ASPxGridPopupControlSettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal GridEditFormPopupSettings EditForm {
			get {
				if(editForm == null)
					editForm = CreateEditFormSettings();
				return editForm;
			}
		}
		protected internal GridCustomizationWindowPopupSettings CustomizationWindow {
			get {
				if(customizationWindow == null)
					customizationWindow = CreateCustomizationPopupSettings();
				return customizationWindow;
			}
		}
		protected internal GridHeaderFilterPopupSettings HeaderFilter {
			get {
				if(headerFilter == null)
					headerFilter = CreateHeaderFilterPopupSettings();
				return headerFilter;
			}
		}
		protected virtual GridEditFormPopupSettings CreateEditFormSettings() {
			return new GridEditFormPopupSettings(Grid);
		}
		protected virtual GridCustomizationWindowPopupSettings CreateCustomizationPopupSettings() {
			return new GridCustomizationWindowPopupSettings(Grid);
		}
		protected virtual GridHeaderFilterPopupSettings CreateHeaderFilterPopupSettings() {
			return new GridHeaderFilterPopupSettings(Grid);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxGridPopupControlSettings;
				if(src != null) {
					EditForm.Assign(src.EditForm);
					CustomizationWindow.Assign(src.CustomizationWindow);
					HeaderFilter.Assign(src.HeaderFilter);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(EditForm);
			list.Add(CustomizationWindow);
			list.Add(HeaderFilter);
			return list.ToArray();
		}
	}
	public class GridPopupSettingsBase : ASPxGridSettingsBase {
		protected PopupHorizontalAlign DefaultHorizontalAlign = PopupHorizontalAlign.RightSides;
		protected PopupVerticalAlign DefaultVerticalAlign = PopupVerticalAlign.Below;
		protected int DefaultHorizontalOffset = 0;
		protected int DefaultVerticalOffset = 0;
		protected ResizingMode DefaultResizingMode = ResizingMode.Live;
		public GridPopupSettingsBase(ASPxGridBase grid)
			: base(grid) {
		}
		protected Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		protected Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
		protected Unit MinWidth {
			get { return GetUnitProperty("MinWidth", Unit.Empty); }
			set {
				if(value == MinWidth)
					return;
				SetUnitProperty("MinWidth", Unit.Empty, value);
				PropertyChanged("MinWidth");
			}
		}
		protected Unit MinHeight {
			get { return GetUnitProperty("MinHeight", Unit.Empty); }
			set {
				if(value == MinHeight)
					return;
				SetUnitProperty("MinHeight", Unit.Empty, value);
				PropertyChanged("MinHeight");
			}
		}
		protected PopupHorizontalAlign HorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("HorizontalAlign", DefaultHorizontalAlign); }
			set {
				if(IsPropertyChanged("HorizontalAlign") && value == HorizontalAlign)
					return;
				SetEnumProperty("HorizontalAlign", DefaultHorizontalAlign, value);
				PropertyChanged("HorizontalAlign");
			}
		}
		protected PopupVerticalAlign VerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("VerticalAlign", DefaultVerticalAlign); }
			set {
				if(IsPropertyChanged("VerticalAlign") && value == VerticalAlign) 
					return;
				SetEnumProperty("VerticalAlign", DefaultVerticalAlign, value);
				PropertyChanged("VerticalAlign");
			}
		}
		protected int HorizontalOffset {
			get { return GetIntProperty("HorizontalOffset", DefaultHorizontalOffset); }
			set {
				if(IsPropertyChanged("HorizontalOffset") && value == HorizontalOffset)
					return;
				SetIntProperty("HorizontalOffset", DefaultHorizontalOffset, value);
				PropertyChanged("HorizontalOffset");
			}
		}
		protected int VerticalOffset {
			get { return GetIntProperty("VerticalOffset", DefaultVerticalOffset); }
			set {
				if(IsPropertyChanged("VerticalOffset") && value == VerticalOffset)
					return;
				SetIntProperty("VerticalOffset", DefaultVerticalOffset, value);
				PropertyChanged("VerticalOffset");
			}
		}
		protected ResizingMode ResizingMode {
			get { return (ResizingMode)GetEnumProperty("ResizingMode", DefaultResizingMode); }
			set {
				if(IsPropertyChanged("ResizingMode") && value == ResizingMode)
					return;
				SetEnumProperty("ResizingMode", DefaultResizingMode, value);
				PropertyChanged("ResizingMode");
			}
		}
		protected bool AllowResize {
			get { return GetBoolProperty("AllowResize", false); }
			set {
				if(IsPropertyChanged("AllowResize") && value == AllowResize)
					return;
				SetBoolProperty("AllowResize", false, value);
				PropertyChanged("AllowResize");
			}
		}
		protected bool ShowHeader {
			get { return GetBoolProperty("ShowHeader", true); }
			set {
				if(IsPropertyChanged("ShowHeader") && value == ShowHeader)
					return;
				SetBoolProperty("ShowHeader", true, value);
				PropertyChanged("ShowHeader");
			}
		}
		protected bool Modal {
			get { return GetBoolProperty("Modal", false); }
			set {
				if(IsPropertyChanged("Modal") && value == Modal)
					return;
				SetBoolProperty("Modal", false, value);
				PropertyChanged("Modal");
			}
		}
		protected AutoBoolean CloseOnEscape {
			get { return (AutoBoolean)GetEnumProperty("CloseOnEscape", AutoBoolean.Auto); }
			set {
				if(IsPropertyChanged("CloseOnEscape") && value == CloseOnEscape)
					return;
				SetEnumProperty("CloseOnEscape", AutoBoolean.Auto, value);
				PropertyChanged("CloseOnEscape");
			}
		}
		protected List<string> ChangedProperties {
			get { return (List<string>)GetObjectProperty("ChangedProperties", null); }
			set { SetObjectProperty("ChangedProperties", null, value); }
		}
		protected void PropertyChanged(string propertyName) {
			if(!IsPropertyChanged(propertyName))
				ChangedProperties.Add(propertyName);
			Changed();
		}
		protected internal bool IsPropertyChanged(string propertyName) {
			if(ChangedProperties == null)
				ChangedProperties = new List<string>();
			return ChangedProperties.Contains(propertyName);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as GridPopupSettingsBase;
				if(src != null) {
					if(Width != src.Width)
						Width = src.Width;
					if(Height != src.Height)
						Height = src.Height;
					if(MinWidth != src.MinWidth)
						MinWidth = src.MinWidth;
					if(MinHeight != src.MinHeight)
						MinHeight = src.MinHeight;
					if(HorizontalAlign != src.HorizontalAlign)
						HorizontalAlign = src.HorizontalAlign;
					if(VerticalAlign != src.VerticalAlign)
						VerticalAlign = src.VerticalAlign;
					if(HorizontalOffset != src.HorizontalOffset)
						HorizontalOffset = src.HorizontalOffset;
					if(VerticalOffset != src.VerticalOffset)
						VerticalOffset = src.VerticalOffset;
					if(ResizingMode != src.ResizingMode)
						ResizingMode = src.ResizingMode;
					if(AllowResize != src.AllowResize)
						AllowResize = src.AllowResize;
					if(ShowHeader != src.ShowHeader)
						ShowHeader = src.ShowHeader;
					if(Modal != src.Modal)
						Modal = src.Modal;
					if(CloseOnEscape != src.CloseOnEscape)
						CloseOnEscape = src.CloseOnEscape;
					if(!AreListEquals(ChangedProperties, src.ChangedProperties))
						ChangedProperties = src.ChangedProperties;
				}
			} finally {
				EndUpdate();
			}
		}
		bool AreListEquals(List<string> list1, List<string> list2) {
			if(list1 == null || list2 == null)
				return false;
			if(list1.Count != list2.Count)
				return false;
			foreach(string item in list1)
				if(!list2.Contains(item))
					return false;
			return true;
		}
	}
	public class GridEditFormPopupSettings : GridPopupSettingsBase {
		public GridEditFormPopupSettings(ASPxGridBase grid)
			: base(grid) {
			DefaultVerticalOffset = -1;
		}
		protected internal new Unit Width { get { return base.Width; } set { base.Width = value; } }
		protected internal new Unit Height { get { return base.Height; } set { base.Height = value; } }
		protected internal new Unit MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		protected internal new Unit MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		protected internal new PopupHorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		protected internal new PopupVerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		protected internal new int HorizontalOffset { get { return base.HorizontalOffset; } set { base.HorizontalOffset = value; } }
		protected internal new int VerticalOffset { get { return base.VerticalOffset; } set { base.VerticalOffset = value; } }
		protected internal new bool ShowHeader { get { return base.ShowHeader; } set { base.ShowHeader = value; } }
		protected internal new bool AllowResize { get { return base.AllowResize; } set { base.AllowResize = value; } }
		protected internal new ResizingMode ResizingMode { get { return base.ResizingMode; } set { base.ResizingMode = value; } }
		protected internal new bool Modal { get { return base.Modal; } set { base.Modal = value; } }
		protected internal new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class GridCustomizationWindowPopupSettings : GridPopupSettingsBase {
		internal readonly static Unit 
			DefaultWidth = 150, DefaultHeight = 170;
		public GridCustomizationWindowPopupSettings(ASPxGridBase grid)
			: base(grid) {
			DefaultVerticalAlign = PopupVerticalAlign.BottomSides;
		}
		protected internal new Unit Width { get { return base.Width; } set { base.Width = value; } }
		protected internal new Unit Height { get { return base.Height; } set { base.Height = value; } }
		protected internal new PopupHorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		protected internal new PopupVerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		protected internal new int HorizontalOffset { get { return base.HorizontalOffset; } set { base.HorizontalOffset = value; } }
		protected internal new int VerticalOffset { get { return base.VerticalOffset; } set { base.VerticalOffset = value; } }
		protected internal new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class GridHeaderFilterPopupSettings : GridPopupSettingsBase {
		public GridHeaderFilterPopupSettings(ASPxGridBase grid)
			: base(grid) {
			CalendarProperties = new CalendarProperties();
		}
		protected internal new Unit Width { get { return base.Width; } set { base.Width = value; } }
		protected internal new Unit Height { get { return base.Height; } set { base.Height = value; } }
		protected internal new Unit MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		protected internal new Unit MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		protected internal new ResizingMode ResizingMode { get { return base.ResizingMode; } set { base.ResizingMode = value; } }
		protected internal new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
		protected internal CalendarProperties CalendarProperties { get; private set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as GridHeaderFilterPopupSettings;
			if(settings != null)
				CalendarProperties.Assign(settings.CalendarProperties);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] { CalendarProperties });
		}
	}
	public class ASPxGridSearchPanelSettings : ASPxGridSettingsBase {
		public const int DefaultInputDelay = 1200;
		public ASPxGridSearchPanelSettings(ASPxGridBase grid)
			: base(grid) {
		}
		protected internal bool Visible {
			get { 
				if(Grid != null)
					return Grid.CallbackState.Get<bool>("SearchPanelVisible");
				return GetBoolProperty("SearchPanelVisible", false);
			}
			set {
				if(value == Visible)
					return;
				SetBoolProperty("SearchPanelVisible", false, value);
				if(Grid != null)
					Grid.CallbackState.Put("SearchPanelVisible", value);
				Changed();
			}
		}
		protected internal bool HighlightResults {
			get { return GetBoolProperty("HighlightResults", true); }
			set {
				if(value == HighlightResults) return;
				SetBoolProperty("HighlightResults", true, value);
				Changed();
			}
		}
		protected internal string ColumnNames {
			get { return GetStringProperty("ColumnNames", "*"); }
			set {
				if(value == ColumnNames) return;
				SetStringProperty("ColumnNames", "*", value);
				Changed();
				FilterChanged();
			}
		}
		protected internal int Delay { 
			get { return GetIntProperty("Delay", DefaultInputDelay); } 
			set { SetIntProperty("Delay", DefaultInputDelay, value); } 
		}
		protected internal bool ShowApplyButton {
			get { return GetBoolProperty("ShowApplyButton", false); }
			set {
				if(value == ShowApplyButton) return;
				SetBoolProperty("ShowApplyButton", false, value);
				Changed();
			}
		}
		protected internal bool ShowClearButton {
			get { return GetBoolProperty("ShowClearButton", false); }
			set {
				if(value == ShowClearButton) return;
				SetBoolProperty("ShowClearButton", false, value);
				Changed();
			}
		}
		protected internal virtual string CustomEditorID {
			get { return GetStringProperty("CustomEditorID", string.Empty); }
			set {
				if(value == CustomEditorID) return;
				SetStringProperty("CustomEditorID", string.Empty, value);
				Changed();
			}
		}
		protected internal bool AllowTextInputTimer {
			get { return GetBoolProperty("AllowTextInputTimer", true); }
			set { SetBoolProperty("AllowTextInputTimer", true, value); }
		}
		protected internal GridViewSearchPanelGroupOperator GroupOperator {
			get { return (GridViewSearchPanelGroupOperator)GetEnumProperty("GroupOperator", GridViewSearchPanelGroupOperator.And); }
			set {
				if(value == GroupOperator) return;
				SetEnumProperty("GroupOperator", GridViewSearchPanelGroupOperator.And, value);
				Changed();
				FilterChanged();
			}
		}
		protected void FilterChanged() {
			if(Grid != null && Grid.Initialized)
				Grid.OnFilterChanged();
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxGridSearchPanelSettings;
			if(src != null) {
				Visible = src.Visible;
				HighlightResults = src.HighlightResults;
				ColumnNames = src.ColumnNames;
				Delay = src.Delay;
				ShowApplyButton = src.ShowApplyButton;
				ShowClearButton = src.ShowClearButton;
				CustomEditorID = src.CustomEditorID;
				AllowTextInputTimer = src.AllowTextInputTimer;
				GroupOperator = src.GroupOperator;
			}
		}
	}
	public class ASPxGridFilterControlSettings : FilterControlSettings {
		public ASPxGridFilterControlSettings(ASPxGridBase grid)
			: base(grid) {
		}
	}
}
