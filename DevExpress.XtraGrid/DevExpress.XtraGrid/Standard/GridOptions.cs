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
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Export;
namespace DevExpress.XtraGrid.Views.Grid {
	public class GridViewOptionsFind : ColumnViewOptionsFind {
		bool searchInPreview = false;
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool SearchInPreview {
			get { return searchInPreview; }
			set {
				if(SearchInPreview == value) return;
				bool prevValue = SearchInPreview;
				searchInPreview = value;
				OnChanged(new BaseOptionChangedEventArgs("SearchInPreview", prevValue, SearchInPreview));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridViewOptionsFind opt = options as GridViewOptionsFind;
				if(opt == null) return;
				this.searchInPreview = opt.SearchInPreview;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsBehavior : ColumnViewOptionsBehavior {
		bool autoUpdateTotalSummary, 
			keepGroupExpandedOnSorting, smartVertScrollBar, allowIncrementalSearch,
			autoExpandAllGroups, allowPartialRedrawOnScrolling, summariesIgnoreNullValues;
		DefaultBoolean allowFixedGroups, allowPixelScrolling, allowPartialGroups, alignGroupSummaryInGroupRow;
		GridEditingMode editingMode = GridEditingMode.Default;
		public GridOptionsBehavior(ColumnView view) : base(view) {
			this.alignGroupSummaryInGroupRow = DefaultBoolean.Default;
			this.allowPartialGroups = DefaultBoolean.Default;
			this.allowPixelScrolling = DefaultBoolean.Default;
			this.allowFixedGroups = DefaultBoolean.Default;
			this.summariesIgnoreNullValues = false;
			this.allowPartialRedrawOnScrolling = true;
			this.autoUpdateTotalSummary = true;
			this.autoExpandAllGroups = false;
			this.keepGroupExpandedOnSorting = true;
			this.smartVertScrollBar = true;
			this.allowIncrementalSearch = false;
		}
		public GridOptionsBehavior() : this(null) { }
		protected new GridView View { get { return (GridView)base.View; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRGridView_OptionsBehaviorAllowOnlyOneMasterRowExpanded)]
		public virtual bool AllowOnlyOneMasterRowExpanded {
			get { return View.OptionsDetail.AllowOnlyOneMasterRowExpanded; }
			set { View.OptionsDetail.AllowOnlyOneMasterRowExpanded = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRGridView_CopyToClipboardWithColumnHeaders)]
		public virtual bool CopyToClipboardWithColumnHeaders {
			get { return View.OptionsClipboard.CopyColumnHeaders == DefaultBoolean.False ? false : true; }
			set { View.OptionsClipboard.CopyColumnHeaders = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorEditingMode"),
#endif
 DefaultValue(GridEditingMode.Default), XtraSerializableProperty()]
		public virtual GridEditingMode EditingMode {
			get { return editingMode; }
			set {
				if(EditingMode == value) return;
				GridEditingMode prevValue = EditingMode;
				editingMode = value;
				OnChanged(new BaseOptionChangedEventArgs("EditingMode", prevValue, EditingMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAllowPartialGroups"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowPartialGroups {
			get { return allowPartialGroups; }
			set {
				if(AllowPartialGroups == value) return;
				DefaultBoolean prevValue = AllowPartialGroups;
				allowPartialGroups = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowPartialGroups", prevValue, AllowPartialGroups));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAlignGroupSummaryInGroupRow"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AlignGroupSummaryInGroupRow {
			get { return alignGroupSummaryInGroupRow; }
			set {
				if(AlignGroupSummaryInGroupRow == value) return;
				DefaultBoolean prevValue = AlignGroupSummaryInGroupRow;
				alignGroupSummaryInGroupRow = value;
				OnChanged(new BaseOptionChangedEventArgs("AlignGroupSummaryInGroupRow", prevValue, AlignGroupSummaryInGroupRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAllowFixedGroups"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowFixedGroups {
			get { return allowFixedGroups; }
			set {
				if(AllowFixedGroups == value) return;
				DefaultBoolean prevValue = AllowFixedGroups;
				allowFixedGroups = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFixedGroups", prevValue, AllowFixedGroups));
			}
		}
		int pixelScrollingCounter = 0;
		bool resetMasterDetail = false;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAllowPixelScrolling"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowPixelScrolling {
			get { return allowPixelScrolling; }
			set {
				if(AllowPixelScrolling == value) return;
				if(value == DefaultBoolean.True) {
					if(View.IsDesignMode && !View.IsLoading && pixelScrollingCounter++ == 0) {
						if(View.OptionsDetail.EnableMasterViewMode) {
							MessageBox.Show("The OptionsDetail.EnableMasterViewMode is enabled. When AllowPixelScrolling is set to true, Master-Detail mode will be disabled", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							View.OptionsDetail.EnableMasterViewMode = false;
							resetMasterDetail = true;
						}
					}
				}
				if(value == DefaultBoolean.Default && resetMasterDetail) {
					View.OptionsDetail.EnableMasterViewMode = true;
					resetMasterDetail = false;
				}
				DefaultBoolean prevValue = AllowPixelScrolling;
				allowPixelScrolling = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowPixelScrolling", prevValue, AllowPixelScrolling));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAllowIncrementalSearch"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowIncrementalSearch {
			get { return allowIncrementalSearch; }
			set {
				if(AllowIncrementalSearch == value) return;
				bool prevValue = AllowIncrementalSearch;
				allowIncrementalSearch = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowIncrementalSearch", prevValue, AllowIncrementalSearch));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorSummariesIgnoreNullValues"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool SummariesIgnoreNullValues {
			get { return summariesIgnoreNullValues; }
			set {
				if(SummariesIgnoreNullValues == value) return;
				bool prevValue = SummariesIgnoreNullValues;
				summariesIgnoreNullValues = value;
				OnChanged(new BaseOptionChangedEventArgs("SummariesIgnoreNullValues", prevValue, SummariesIgnoreNullValues));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAllowPartialRedrawOnScrolling"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowPartialRedrawOnScrolling {
			get { return allowPartialRedrawOnScrolling; }
			set {
				if(AllowPartialRedrawOnScrolling == value) return;
				bool prevValue = AllowPartialRedrawOnScrolling;
				allowPartialRedrawOnScrolling = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowPartialRedrawOnScrolling", prevValue, AllowPartialRedrawOnScrolling));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAutoUpdateTotalSummary"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoUpdateTotalSummary {
			get { return autoUpdateTotalSummary; }
			set {
				if(AutoUpdateTotalSummary == value) return;
				bool prevValue = AutoUpdateTotalSummary;
				autoUpdateTotalSummary = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoUpdateTotalSummary", prevValue, AutoUpdateTotalSummary));
			}
		}
		internal const string AutoExpandAllGroupsName = "AutoExpandAllGroups";
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorAutoExpandAllGroups"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoExpandAllGroups {
			get { return autoExpandAllGroups; }
			set {
				if(AutoExpandAllGroups == value) return;
				bool prevValue = AutoExpandAllGroups;
				autoExpandAllGroups = value;
				OnChanged(new BaseOptionChangedEventArgs(AutoExpandAllGroupsName, prevValue, AutoExpandAllGroups));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorKeepGroupExpandedOnSorting"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool KeepGroupExpandedOnSorting {
			get { return keepGroupExpandedOnSorting; }
			set {
				if(KeepGroupExpandedOnSorting == value) return;
				bool prevValue = KeepGroupExpandedOnSorting;
				keepGroupExpandedOnSorting = value;
				OnChanged(new BaseOptionChangedEventArgs("KeepGroupExpandedOnSorting", prevValue, KeepGroupExpandedOnSorting));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsBehaviorSmartVertScrollBar"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool SmartVertScrollBar {
			get { return smartVertScrollBar; }
			set {
				if(SmartVertScrollBar == value) return;
				bool prevValue = SmartVertScrollBar;
				smartVertScrollBar = value;
				OnChanged(new BaseOptionChangedEventArgs("SmartVertScrollBar", prevValue, SmartVertScrollBar));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsBehavior opt = options as GridOptionsBehavior;
				if(opt == null) return;
				this.alignGroupSummaryInGroupRow = opt.AlignGroupSummaryInGroupRow;
				this.editingMode = opt.EditingMode;
				this.allowPixelScrolling = opt.AllowPixelScrolling;
				this.summariesIgnoreNullValues = opt.SummariesIgnoreNullValues;
				this.autoExpandAllGroups = opt.AutoExpandAllGroups;
				this.allowIncrementalSearch = opt.AllowIncrementalSearch;
				this.autoUpdateTotalSummary = opt.AutoUpdateTotalSummary;
				this.keepGroupExpandedOnSorting = opt.KeepGroupExpandedOnSorting;
				this.smartVertScrollBar = opt.SmartVertScrollBar;
				this.allowPartialRedrawOnScrolling = opt.AllowPartialRedrawOnScrolling;
				this.allowFixedGroups = opt.AllowFixedGroups;
				this.allowPartialGroups = opt.AllowPartialGroups;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public enum EditFormBindingMode { Default, Direct, Cached }
	public enum GridEditingMode { Default, Inplace, EditFormInplace, EditFormInplaceHideCurrentRow, EditForm }
	public enum EditFormModifiedAction { Default, Save, Cancel, Nothing }
	public class GridOptionsEditForm : ViewBaseOptions {
		int editFormColumnCount = 3;
		int popupEditFormWidth = 800;
		string formCaptionFormat = "";
		UserControl customEditFormLayout;
		DefaultBoolean showOnEnterKey = DefaultBoolean.Default;
		DefaultBoolean showOnF2Key = DefaultBoolean.Default;
		DefaultBoolean showOnDoubleClick = DefaultBoolean.Default;
		DefaultBoolean showUpdateCancelPanel = DefaultBoolean.Default;
		EditFormModifiedAction actionOnModifiedRowChange = EditFormModifiedAction.Default;
		EditFormBindingMode bindingMode;
		public GridOptionsEditForm() {
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormCustomEditFormLayout"),
#endif
 DefaultValue(null), Browsable(false)]
		public virtual UserControl CustomEditFormLayout {
			get { return customEditFormLayout; }
			set {
				if(CustomEditFormLayout == value) return;
				UserControl prevValue = CustomEditFormLayout;
				customEditFormLayout = value;
				OnChanged(new BaseOptionChangedEventArgs("CustomEditFormLayout", prevValue, CustomEditFormLayout));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormPopupEditFormWidth"),
#endif
 DefaultValue(800), XtraSerializableProperty()]
		public virtual int PopupEditFormWidth {
			get { return popupEditFormWidth; }
			set {
				if(value < 100) value = 100;
				if(PopupEditFormWidth == value) return;
				int prevValue = PopupEditFormWidth;
				popupEditFormWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("PopupEditFormWidth", prevValue, PopupEditFormWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormBindingMode"),
#endif
 DefaultValue(EditFormBindingMode.Default), XtraSerializableProperty()]
		public virtual EditFormBindingMode BindingMode {
			get { return bindingMode; }
			set {
				if(BindingMode == value) return;
				EditFormBindingMode prevValue = BindingMode;
				bindingMode = value;
				OnChanged(new BaseOptionChangedEventArgs("BindingMode", prevValue, BindingMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormEditFormColumnCount"),
#endif
 DefaultValue(3), XtraSerializableProperty()]
		public virtual int EditFormColumnCount {
			get { return editFormColumnCount; }
			set {
				if(EditFormColumnCount == value) return;
				int prevValue = EditFormColumnCount;
				editFormColumnCount = value;
				OnChanged(new BaseOptionChangedEventArgs("EditFormColumnCount", prevValue, EditFormColumnCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormActionOnModifiedRowChange"),
#endif
 DefaultValue(EditFormModifiedAction.Default), XtraSerializableProperty()]
		public virtual EditFormModifiedAction ActionOnModifiedRowChange {
			get { return actionOnModifiedRowChange; }
			set {
				if(ActionOnModifiedRowChange == value) return;
				EditFormModifiedAction prevValue = ActionOnModifiedRowChange;
				actionOnModifiedRowChange = value;
				OnChanged(new BaseOptionChangedEventArgs("ActionOnModifiedRowChange", prevValue, ActionOnModifiedRowChange));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormShowOnEnterKey"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowOnEnterKey {
			get { return showOnEnterKey; }
			set {
				if(ShowOnEnterKey == value) return;
				DefaultBoolean prevValue = ShowOnEnterKey;
				showOnEnterKey = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowOnEnterKey", prevValue, ShowOnEnterKey));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormShowOnDoubleClick"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowOnDoubleClick {
			get { return showOnDoubleClick; }
			set {
				if(ShowOnDoubleClick == value) return;
				DefaultBoolean prevValue = ShowOnDoubleClick;
				showOnDoubleClick = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowOnDoubleClick", prevValue, ShowOnDoubleClick));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormShowUpdateCancelPanel"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowUpdateCancelPanel {
			get { return showUpdateCancelPanel; }
			set {
				if(ShowUpdateCancelPanel == value) return;
				DefaultBoolean prevValue = ShowUpdateCancelPanel;
				showUpdateCancelPanel = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowUpdateCancelPanel", prevValue, ShowUpdateCancelPanel));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormShowOnF2Key"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowOnF2Key {
			get { return showOnF2Key; }
			set {
				if(ShowOnF2Key == value) return;
				DefaultBoolean prevValue = ShowOnF2Key;
				showOnF2Key = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowOnF2Key", prevValue, ShowOnF2Key));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsEditFormFormCaptionFormat"),
#endif
 DefaultValue(""), XtraSerializableProperty()]
		public virtual string FormCaptionFormat {
			get { return formCaptionFormat; }
			set {
				if(FormCaptionFormat == value) return;
				string prevValue = FormCaptionFormat;
				formCaptionFormat = value;
				OnChanged(new BaseOptionChangedEventArgs("FormCaptionFormat", prevValue, FormCaptionFormat));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsEditForm opt = options as GridOptionsEditForm;
				if(opt == null) return;
				this.showUpdateCancelPanel = opt.ShowUpdateCancelPanel;
				this.popupEditFormWidth = opt.PopupEditFormWidth;
				this.actionOnModifiedRowChange = opt.ActionOnModifiedRowChange;
				this.bindingMode = opt.BindingMode;
				this.showOnDoubleClick = opt.ShowOnDoubleClick;
				this.showOnEnterKey = opt.ShowOnEnterKey;
				this.showOnF2Key = opt.ShowOnF2Key;
				this.editFormColumnCount = opt.EditFormColumnCount;
				this.formCaptionFormat = opt.FormCaptionFormat;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public enum DetailExpandButtonMode { Default, AlwaysEnabled, CheckDefaultDetail, CheckAllDetails }
	public class GridOptionsDetail : ViewBaseOptions {
		bool allowExpandEmptyDetails, allowZoomDetail, enableMasterViewMode, autoZoomDetail, 
			enableDetailToolTip, showDetailTabs, smartDetailHeight, smartDetailExpand, allowOnlyOneMasterRowExpanded;
		DetailExpandButtonMode smartDetailExpandButtonMode;
		public GridOptionsDetail() {
			this.smartDetailExpand = true;
			this.enableMasterViewMode = true;
			this.smartDetailExpandButtonMode = DetailExpandButtonMode.Default;
			this.allowExpandEmptyDetails = false;
			this.allowZoomDetail = true;
			this.autoZoomDetail = false;
			this.enableDetailToolTip = false;
			this.showDetailTabs = true;
			this.smartDetailHeight = false;
			this.allowOnlyOneMasterRowExpanded = false;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailAllowOnlyOneMasterRowExpanded"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowOnlyOneMasterRowExpanded {
			get { return allowOnlyOneMasterRowExpanded; }
			set {
				if(AllowOnlyOneMasterRowExpanded == value) return;
				bool prevValue = AllowOnlyOneMasterRowExpanded;
				allowOnlyOneMasterRowExpanded = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowOnlyOneMasterRowExpanded", prevValue, AllowOnlyOneMasterRowExpanded));
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridOptionsDetail_SmartDetailExpandButton)]
		public virtual bool SmartDetailExpandButton {
			get { return GetSmartDetailExpandButtonMode() != DetailExpandButtonMode.AlwaysEnabled; }
			set {
				if(SmartDetailExpandButton == value) return;
				SmartDetailExpandButtonMode = value ? DetailExpandButtonMode.Default : DetailExpandButtonMode.CheckDefaultDetail;
			}
		}
		protected internal virtual DetailExpandButtonMode GetSmartDetailExpandButtonMode() {
			DetailExpandButtonMode res = SmartDetailExpandButtonMode;
			if(res == DetailExpandButtonMode.Default) return DetailExpandButtonMode.CheckDefaultDetail;
			return res;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailSmartDetailExpandButtonMode"),
#endif
 DefaultValue(DetailExpandButtonMode.Default), XtraSerializableProperty()]
		public virtual DetailExpandButtonMode SmartDetailExpandButtonMode {
			get { return smartDetailExpandButtonMode; }
			set {
				if(SmartDetailExpandButtonMode == value) return;
				DetailExpandButtonMode prevValue = SmartDetailExpandButtonMode;
				smartDetailExpandButtonMode = value;
				OnChanged(new BaseOptionChangedEventArgs("SmartDetailExpandButtonMode", prevValue, SmartDetailExpandButtonMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailSmartDetailExpand"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool SmartDetailExpand {
			get { return smartDetailExpand; }
			set {
				if(SmartDetailExpand == value) return;
				bool prevValue = SmartDetailExpand;
				smartDetailExpand = value;
				OnChanged(new BaseOptionChangedEventArgs("SmartDetailExpand", prevValue, SmartDetailExpand));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailAllowExpandEmptyDetails"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowExpandEmptyDetails {
			get { return allowExpandEmptyDetails; }
			set {
				if(AllowExpandEmptyDetails == value) return;
				bool prevValue = AllowExpandEmptyDetails;
				allowExpandEmptyDetails = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowExpandEmptyDetails", prevValue, AllowExpandEmptyDetails));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailAllowZoomDetail"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowZoomDetail {
			get { return allowZoomDetail; }
			set {
				if(AllowZoomDetail == value) return;
				bool prevValue = AllowZoomDetail;
				allowZoomDetail = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowZoomDetail", prevValue, AllowZoomDetail));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailAutoZoomDetail"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoZoomDetail {
			get { return autoZoomDetail; }
			set {
				if(AutoZoomDetail == value) return;
				bool prevValue = AutoZoomDetail;
				autoZoomDetail = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoZoomDetail", prevValue, AutoZoomDetail));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailEnableMasterViewMode"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableMasterViewMode {
			get { return enableMasterViewMode; }
			set {
				if(EnableMasterViewMode == value) return;
				bool prevValue = EnableMasterViewMode;
				enableMasterViewMode = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableMasterViewMode", prevValue, EnableMasterViewMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailEnableDetailToolTip"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableDetailToolTip {
			get { return enableDetailToolTip; }
			set {
				if(EnableDetailToolTip == value) return;
				bool prevValue = EnableDetailToolTip;
				enableDetailToolTip = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableDetailToolTip", prevValue, EnableDetailToolTip));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailShowDetailTabs"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowDetailTabs {
			get { return showDetailTabs; }
			set {
				if(ShowDetailTabs == value) return;
				bool prevValue = ShowDetailTabs;
				showDetailTabs = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowDetailTabs", prevValue, ShowDetailTabs));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsDetailSmartDetailHeight"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool SmartDetailHeight {
			get { return smartDetailHeight; }
			set {
				if(SmartDetailHeight == value) return;
				bool prevValue = SmartDetailHeight;
				smartDetailHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("SmartDetailHeight", prevValue, SmartDetailHeight));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsDetail opt = options as GridOptionsDetail;
				if(opt == null) return;
				this.allowOnlyOneMasterRowExpanded = opt.AllowOnlyOneMasterRowExpanded;
				this.smartDetailExpandButtonMode = opt.SmartDetailExpandButtonMode;
				this.allowExpandEmptyDetails = opt.AllowExpandEmptyDetails;
				this.allowZoomDetail = opt.AllowZoomDetail;
				this.autoZoomDetail = opt.AutoZoomDetail;
				this.enableDetailToolTip = opt.EnableDetailToolTip;
				this.enableMasterViewMode = opt.EnableMasterViewMode;
				this.showDetailTabs = opt.ShowDetailTabs;
				this.smartDetailHeight = opt.SmartDetailHeight;
				this.smartDetailExpand = opt.SmartDetailExpand;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsCustomization : ViewBaseOptions {
		bool allowFilter, allowGroup, allowRowSizing, allowSort, allowColumnMoving, allowColumnResizing, allowQuickHideColumns, customizationFormSearchBoxVisible;
		public GridOptionsCustomization() {
			this.allowFilter = true;
			this.allowGroup = true;
			this.allowRowSizing = false;
			this.allowSort = true;
			this.allowColumnMoving = true;
			this.allowColumnResizing = true;
			this.allowQuickHideColumns = true;
			this.customizationFormSearchBoxVisible = false;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFilter {
			get { return allowFilter; }
			set {
				if(AllowFilter == value) return;
				bool prevValue = AllowFilter;
				allowFilter = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFilter", prevValue, AllowFilter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowQuickHideColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowQuickHideColumns {
			get { return allowQuickHideColumns; }
			set {
				if(AllowQuickHideColumns == value) return;
				bool prevValue = AllowQuickHideColumns;
				allowQuickHideColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowQuickHideColumns", prevValue, AllowQuickHideColumns));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowColumnResizing"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowColumnResizing {
			get { return allowColumnResizing; }
			set {
				if(AllowColumnResizing == value) return;
				bool prevValue = AllowColumnResizing;
				allowColumnResizing = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowColumnResizing", prevValue, AllowColumnResizing));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowColumnMoving"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowColumnMoving {
			get { return allowColumnMoving; }
			set {
				if(AllowColumnMoving == value) return;
				bool prevValue = AllowColumnMoving;
				allowColumnMoving = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowColumnMoving", prevValue, AllowColumnMoving));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowGroup"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowGroup {
			get { return allowGroup; }
			set {
				if(AllowGroup == value) return;
				bool prevValue = AllowGroup;
				allowGroup = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowGroup", prevValue, AllowGroup));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowRowSizing"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowRowSizing {
			get { return allowRowSizing; }
			set {
				if(AllowRowSizing == value) return;
				bool prevValue = AllowRowSizing;
				allowRowSizing = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowRowSizing", prevValue, AllowRowSizing));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationAllowSort"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowSort {
			get { return allowSort; }
			set {
				if(AllowSort == value) return;
				bool prevValue = AllowSort;
				allowSort = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowSort", prevValue, AllowSort));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsCustomizationCustomizationFormSearchBoxVisible"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool CustomizationFormSearchBoxVisible {
			get { return customizationFormSearchBoxVisible; }
			set {
				if(CustomizationFormSearchBoxVisible == value) return;
				bool prevValue = CustomizationFormSearchBoxVisible;
				customizationFormSearchBoxVisible = value;
				OnChanged(new BaseOptionChangedEventArgs("CustomizationFormSearchBoxVisible", prevValue, CustomizationFormSearchBoxVisible));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsCustomization opt = options as GridOptionsCustomization;
				if(opt == null) return;
				this.allowColumnResizing = opt.AllowColumnResizing;
				this.allowFilter = opt.AllowFilter;
				this.allowGroup = opt.AllowGroup;
				this.allowRowSizing = opt.AllowRowSizing;
				this.allowSort = opt.AllowSort;
				this.allowColumnMoving = opt.AllowColumnMoving;
				this.allowQuickHideColumns = opt.AllowQuickHideColumns;
				this.customizationFormSearchBoxVisible = opt.CustomizationFormSearchBoxVisible;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsNavigation : ViewBaseOptions {
		bool autoFocusNewRow, autoMoveRowFocus, enterMoveNextColumn, useOfficePageNavigation,
			useTabKey;
		public GridOptionsNavigation() {
			this.useOfficePageNavigation = true;
			this.autoFocusNewRow = false;
			this.autoMoveRowFocus = true;
			this.enterMoveNextColumn = false;
			this.useTabKey = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsNavigationUseOfficePageNavigation"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseOfficePageNavigation {
			get { return useOfficePageNavigation; }
			set {
				if(UseOfficePageNavigation == value) return;
				bool prevValue = UseOfficePageNavigation;
				useOfficePageNavigation = value;
				OnChanged(new BaseOptionChangedEventArgs("UseOfficePageNavigation", prevValue, UseOfficePageNavigation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsNavigationAutoFocusNewRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoFocusNewRow {
			get { return autoFocusNewRow; }
			set {
				if(AutoFocusNewRow == value) return;
				bool prevValue = AutoFocusNewRow;
				autoFocusNewRow = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoFocusNewRow", prevValue, AutoFocusNewRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsNavigationAutoMoveRowFocus"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoMoveRowFocus {
			get { return autoMoveRowFocus; }
			set {
				if(AutoMoveRowFocus == value) return;
				bool prevValue = AutoMoveRowFocus;
				autoMoveRowFocus = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoMoveRowFocus", prevValue, AutoMoveRowFocus));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsNavigationEnterMoveNextColumn"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnterMoveNextColumn {
			get { return enterMoveNextColumn; }
			set {
				if(EnterMoveNextColumn == value) return;
				bool prevValue = EnterMoveNextColumn;
				enterMoveNextColumn = value;
				OnChanged(new BaseOptionChangedEventArgs("EnterMoveNextColumn", prevValue, EnterMoveNextColumn));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsNavigationUseTabKey"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseTabKey {
			get { return useTabKey; }
			set {
				if(UseTabKey == value) return;
				bool prevValue = UseTabKey;
				useTabKey = value;
				OnChanged(new BaseOptionChangedEventArgs("UseTabKey", prevValue, UseTabKey));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsNavigation opt = options as GridOptionsNavigation;
				if(opt == null) return;
				this.autoFocusNewRow = opt.AutoFocusNewRow;
				this.autoMoveRowFocus = opt.AutoMoveRowFocus;
				this.enterMoveNextColumn = opt.EnterMoveNextColumn;
				this.useTabKey = opt.UseTabKey;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public enum GridMultiSelectMode { RowSelect, CellSelect, CheckBoxRowSelect }
	public class GridOptionsSelection : ColumnViewOptionsSelection {
		bool invertSelection, enableAppearanceFocusedRow, enableAppearanceFocusedCell, enableAppearanceHideSelection, useIndicatorForSelection,
			resetSelectionClickOutsideCheckboxSelector;
		GridMultiSelectMode multiSelectMode;
		DefaultBoolean showCheckBoxSelectorInColumnHeader = DefaultBoolean.Default;
		DefaultBoolean showCheckBoxSelectorInGroupRow = DefaultBoolean.Default;
		DefaultBoolean showCheckBoxSelectorChangesSelectionNavigation = DefaultBoolean.Default,
			showCheckBoxSelectorInPrintExport = DefaultBoolean.Default;
		int checkBoxSelectorColumnWidth;
		string checkBoxSelectorField = "";
		public GridOptionsSelection() {
			this.multiSelectMode = GridMultiSelectMode.RowSelect;
			this.invertSelection = false;
			this.enableAppearanceHideSelection = true;
			this.enableAppearanceFocusedRow = true;
			this.enableAppearanceFocusedCell = true;
			this.useIndicatorForSelection = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionShowCheckBoxSelectorChangesSelectionNavigation"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), Browsable(false)]
		public virtual DefaultBoolean ShowCheckBoxSelectorChangesSelectionNavigation {
			get { return showCheckBoxSelectorChangesSelectionNavigation; }
			set {
				if(ShowCheckBoxSelectorChangesSelectionNavigation == value) return;
				DefaultBoolean prevValue = ShowCheckBoxSelectorChangesSelectionNavigation;
				showCheckBoxSelectorChangesSelectionNavigation = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCheckBoxSelectorChangesSelectionNavigation", prevValue, ShowCheckBoxSelectorChangesSelectionNavigation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionShowCheckBoxSelectorInPrintExport"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowCheckBoxSelectorInPrintExport {
			get { return showCheckBoxSelectorInPrintExport; }
			set {
				if(ShowCheckBoxSelectorInPrintExport == value) return;
				DefaultBoolean prevValue = ShowCheckBoxSelectorInPrintExport;
				showCheckBoxSelectorInPrintExport = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCheckBoxSelectorInPrintExport", prevValue, ShowCheckBoxSelectorInPrintExport));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionShowCheckBoxSelectorInColumnHeader"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowCheckBoxSelectorInColumnHeader {
			get { return showCheckBoxSelectorInColumnHeader; }
			set {
				if(ShowCheckBoxSelectorInColumnHeader == value) return;
				DefaultBoolean prevValue = ShowCheckBoxSelectorInColumnHeader;
				showCheckBoxSelectorInColumnHeader = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCheckBoxSelectorInColumnHeader", prevValue, ShowCheckBoxSelectorInColumnHeader));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionShowCheckBoxSelectorInGroupRow"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowCheckBoxSelectorInGroupRow {
			get { return showCheckBoxSelectorInGroupRow; }
			set {
				if(ShowCheckBoxSelectorInGroupRow == value) return;
				DefaultBoolean prevValue = ShowCheckBoxSelectorInGroupRow;
				showCheckBoxSelectorInGroupRow = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCheckBoxSelectorInGroupRow", prevValue, ShowCheckBoxSelectorInGroupRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionCheckBoxSelectorColumnWidth"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int CheckBoxSelectorColumnWidth {
			get { return checkBoxSelectorColumnWidth; }
			set {
				if(value < 15) value = 0;
				if(value > 300) value = 300;
				if(CheckBoxSelectorColumnWidth == value) return;
				int prevValue = CheckBoxSelectorColumnWidth;
				checkBoxSelectorColumnWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("CheckBoxSelectorColumnWidth", prevValue, CheckBoxSelectorColumnWidth));
			}
		}
		protected internal virtual string CheckBoxSelectorField {
			get { return checkBoxSelectorField; }
			set {
				if(value == null) value = "";
				if(CheckBoxSelectorField == value) return;
				string prevValue = CheckBoxSelectorField;
				checkBoxSelectorField = value;
				OnChanged(new BaseOptionChangedEventArgs("CheckBoxSelectorField", prevValue, CheckBoxSelectorField));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionUseIndicatorForSelection"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseIndicatorForSelection {
			get { return useIndicatorForSelection; }
			set {
				if(UseIndicatorForSelection == value) return;
				bool prevValue = useIndicatorForSelection;
				useIndicatorForSelection = value;
				OnChanged(new BaseOptionChangedEventArgs("UseIndicatorForSelection", prevValue, UseIndicatorForSelection));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionMultiSelectMode"),
#endif
 DefaultValue(GridMultiSelectMode.RowSelect), XtraSerializableProperty()]
		public virtual GridMultiSelectMode MultiSelectMode {
			get { return multiSelectMode; }
			set {
				if(MultiSelectMode == value) return;
				GridMultiSelectMode prevValue = multiSelectMode;
				multiSelectMode = value;
				OnChanged(new BaseOptionChangedEventArgs("MultiSelectMode", prevValue, MultiSelectMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionResetSelectionClickOutsideCheckboxSelector"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ResetSelectionClickOutsideCheckboxSelector { 
			get { return resetSelectionClickOutsideCheckboxSelector; }
			set {
				if(ResetSelectionClickOutsideCheckboxSelector == value) return;
				bool prevValue = ResetSelectionClickOutsideCheckboxSelector;
				resetSelectionClickOutsideCheckboxSelector = value;
				OnChanged(new BaseOptionChangedEventArgs("ResetSelectionClickOutsideCheckboxSelector", prevValue, ResetSelectionClickOutsideCheckboxSelector));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionEnableAppearanceFocusedCell"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableAppearanceFocusedCell {
			get { return enableAppearanceFocusedCell; }
			set {
				if(EnableAppearanceFocusedCell == value) return;
				bool prevValue = EnableAppearanceFocusedCell;
				enableAppearanceFocusedCell = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceFocusedCell", prevValue, EnableAppearanceFocusedCell));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionEnableAppearanceFocusedRow"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableAppearanceFocusedRow {
			get { return enableAppearanceFocusedRow; }
			set {
				if(EnableAppearanceFocusedRow == value) return;
				bool prevValue = EnableAppearanceFocusedRow;
				enableAppearanceFocusedRow = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceFocusedRow", prevValue, EnableAppearanceFocusedRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionEnableAppearanceHideSelection"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableAppearanceHideSelection {
			get { return enableAppearanceHideSelection; }
			set {
				if(EnableAppearanceHideSelection == value) return;
				bool prevValue = EnableAppearanceHideSelection;
				enableAppearanceHideSelection = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceHideSelection", prevValue, EnableAppearanceHideSelection));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsSelectionInvertSelection"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool InvertSelection {
			get { return invertSelection; }
			set {
				if(InvertSelection == value) return;
				bool prevValue = InvertSelection;
				invertSelection = value;
				OnChanged(new BaseOptionChangedEventArgs("InvertSelection", prevValue, InvertSelection));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsSelection opt = options as GridOptionsSelection;
				if(opt == null) return;
				this.resetSelectionClickOutsideCheckboxSelector = opt.ResetSelectionClickOutsideCheckboxSelector;
				this.enableAppearanceHideSelection = opt.EnableAppearanceHideSelection;
				this.multiSelectMode = opt.MultiSelectMode;
				this.useIndicatorForSelection = opt.UseIndicatorForSelection;
				this.invertSelection = opt.InvertSelection;
				this.enableAppearanceFocusedCell = opt.EnableAppearanceFocusedCell;
				this.enableAppearanceFocusedRow = opt.EnableAppearanceFocusedRow;
				this.showCheckBoxSelectorChangesSelectionNavigation = opt.ShowCheckBoxSelectorChangesSelectionNavigation;
				this.showCheckBoxSelectorInGroupRow = opt.ShowCheckBoxSelectorInGroupRow;
				this.showCheckBoxSelectorInColumnHeader = opt.ShowCheckBoxSelectorInColumnHeader;
				this.showCheckBoxSelectorInPrintExport = opt.ShowCheckBoxSelectorInPrintExport;
				this.checkBoxSelectorColumnWidth = opt.CheckBoxSelectorColumnWidth;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsView : ColumnViewOptionsView {
		bool autoCalcPreviewLineCount, allowCellMerge, columnAutoWidth, rowAutoHeight, 
			showChildrenInGroupPanel, showColumnHeaders, showDetailButtons, showGroupExpandCollapseButtons, 
			showFooter, showGroupedColumns, showGroupPanel, showGroupPanelColumnsAsSingleRow,
			showIndicator, showPreview,
			showAutoFilterRow, enableAppearanceEvenRow, enableAppearanceOddRow, allowHtmlDrawHeaders, allowHtmlDrawGroups, allowGlyphSkinning;
		NewItemRowPosition newItemRowPosition;
		GroupDrawMode groupDrawMode;
		FilterButtonShowMode headerFilterButtonShowMode;
		WaitAnimationOptions waitAnimationOptions;
		DefaultBoolean showPreviewRowLines, showHorizontalLines, showVerticalLines;
		GroupFooterShowMode groupFooterShowMode;
		int bestFitMaxRowCount;
		DefaultBoolean bestFitUseErrorInfo, columnHeaderAutoHeight;
		public GridOptionsView() {
			this.showGroupPanelColumnsAsSingleRow = false;
			this.columnHeaderAutoHeight = DefaultBoolean.Default;
			this.showPreviewRowLines = this.showHorizontalLines = this.showVerticalLines = DefaultBoolean.Default;
			this.groupFooterShowMode = GroupFooterShowMode.VisibleIfExpanded;
			this.allowHtmlDrawGroups = true;
			this.waitAnimationOptions = WaitAnimationOptions.Default;
			this.showGroupExpandCollapseButtons = true;
			this.allowHtmlDrawHeaders = false;
			this.headerFilterButtonShowMode = FilterButtonShowMode.Default;
			this.enableAppearanceEvenRow = this.enableAppearanceOddRow = false;
			this.allowCellMerge = false;
			this.groupDrawMode = GroupDrawMode.Default;
			this.newItemRowPosition = NewItemRowPosition.None;
			this.autoCalcPreviewLineCount = false;
			this.columnAutoWidth = true;
			this.rowAutoHeight = false;
			this.showChildrenInGroupPanel = false;
			this.showColumnHeaders = true;
			this.showDetailButtons = true;
			this.showAutoFilterRow = false;
			this.showFooter = false;
			this.showGroupedColumns = false;
			this.showGroupPanel = true;
			this.showIndicator = true;
			this.showPreview = false;
			this.bestFitUseErrorInfo = DefaultBoolean.Default;
			this.bestFitMaxRowCount = -1;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewBestFitMaxRowCount"),
#endif
 DefaultValue(-1), XtraSerializableProperty()]
		public virtual int BestFitMaxRowCount {
			get { return bestFitMaxRowCount; }
			set {
				if(value < 0) value = -1;
				if(BestFitMaxRowCount == value) return;
				int prevValue = BestFitMaxRowCount;
				bestFitMaxRowCount = value;
				OnChanged(new BaseOptionChangedEventArgs("BestFitMaxRowCount", prevValue, BestFitMaxRowCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewBestFitUseErrorInfo"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean BestFitUseErrorInfo {
			get { return bestFitUseErrorInfo; }
			set {
				if(BestFitUseErrorInfo == value) return;
				DefaultBoolean prevValue = BestFitUseErrorInfo;
				bestFitUseErrorInfo = value;
				OnChanged(new BaseOptionChangedEventArgs("BestFitUseErrorInfo", prevValue, BestFitUseErrorInfo));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewHeaderFilterButtonShowMode"),
#endif
 DefaultValue(FilterButtonShowMode.Default), XtraSerializableProperty()]
		public virtual FilterButtonShowMode HeaderFilterButtonShowMode {
			get { return headerFilterButtonShowMode; }
			set {
				if(HeaderFilterButtonShowMode == value) return;
				FilterButtonShowMode prevValue = HeaderFilterButtonShowMode;
				headerFilterButtonShowMode = value;
				OnChanged(new BaseOptionChangedEventArgs("HeaderFilterButtonShowMode", prevValue, HeaderFilterButtonShowMode));
			}
		}
		protected internal virtual FilterButtonShowMode GetHeaderFilterButtonShowMode() {
			return HeaderFilterButtonShowMode == FilterButtonShowMode.Default ? FilterButtonShowMode.SmartTag : HeaderFilterButtonShowMode;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewGroupFooterShowMode"),
#endif
 DefaultValue(GroupFooterShowMode.VisibleIfExpanded), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public GroupFooterShowMode GroupFooterShowMode {
			get { return groupFooterShowMode; }
			set {
				if(value == GroupFooterShowMode) return;
				GroupFooterShowMode prevValue = GroupFooterShowMode;
				groupFooterShowMode = value;
				OnChanged(new BaseOptionChangedEventArgs("GroupFooterShowMode", prevValue, GroupFooterShowMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewGroupDrawMode"),
#endif
 DefaultValue(GroupDrawMode.Default), XtraSerializableProperty()]
		public virtual GroupDrawMode GroupDrawMode {
			get { return groupDrawMode; }
			set {
				if(GroupDrawMode == value) return;
				DevExpress.XtraGrid.Views.Grid.GroupDrawMode prevValue = GroupDrawMode;
				groupDrawMode = value;
				OnChanged(new BaseOptionChangedEventArgs("GroupDrawMode", prevValue, GroupDrawMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewWaitAnimationOptions"),
#endif
 DefaultValue(WaitAnimationOptions.Default), XtraSerializableProperty()]
		public virtual WaitAnimationOptions WaitAnimationOptions {
			get { return waitAnimationOptions; }
			set {
				if(WaitAnimationOptions == value) return;
				WaitAnimationOptions prevValue = WaitAnimationOptions;
				waitAnimationOptions = value;
				OnChanged(new BaseOptionChangedEventArgs("WaitAnimationOptions", prevValue, WaitAnimationOptions));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewNewItemRowPosition"),
#endif
 DefaultValue(NewItemRowPosition.None), XtraSerializableProperty()]
		public virtual NewItemRowPosition NewItemRowPosition {
			get { return newItemRowPosition; }
			set {
				if(NewItemRowPosition == value) return;
				DevExpress.XtraGrid.Views.Grid.NewItemRowPosition prevValue = NewItemRowPosition;
				newItemRowPosition = value;
				OnChanged(new BaseOptionChangedEventArgs("NewItemRowPosition", prevValue, NewItemRowPosition));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewAutoCalcPreviewLineCount"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoCalcPreviewLineCount {
			get { return autoCalcPreviewLineCount; }
			set {
				if(AutoCalcPreviewLineCount == value) return;
				bool prevValue = AutoCalcPreviewLineCount;
				autoCalcPreviewLineCount = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoCalcPreviewLineCount", prevValue, AutoCalcPreviewLineCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewAllowHtmlDrawHeaders"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowHtmlDrawHeaders {
			get { return allowHtmlDrawHeaders; }
			set {
				if(AllowHtmlDrawHeaders == value) return;
				bool prevValue = AllowHtmlDrawHeaders;
				allowHtmlDrawHeaders = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowHtmlDrawHeaders", prevValue, AllowHtmlDrawHeaders));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewAllowGlyphSkinning"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				bool prevValue = AllowGlyphSkinning;
				allowGlyphSkinning = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowGlyphSkinning", prevValue, AllowGlyphSkinning));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewAllowHtmlDrawGroups"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowHtmlDrawGroups {
			get { return allowHtmlDrawGroups; }
			set {
				if(AllowHtmlDrawGroups == value) return;
				bool prevValue = AllowHtmlDrawGroups;
				allowHtmlDrawGroups = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowHtmlDrawGroups", prevValue, AllowHtmlDrawGroups));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewColumnAutoWidth"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ColumnAutoWidth {
			get { return columnAutoWidth; }
			set {
				if(ColumnAutoWidth == value) return;
				bool prevValue = ColumnAutoWidth;
				columnAutoWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("ColumnAutoWidth", prevValue, ColumnAutoWidth));
			}
		}
		[ DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ColumnHeaderAutoHeight {
			get { return columnHeaderAutoHeight; }
			set {
				if(ColumnHeaderAutoHeight == value) return;
				DefaultBoolean prevValue = ColumnHeaderAutoHeight;
				columnHeaderAutoHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("ColumnAutoHeight", prevValue, ColumnHeaderAutoHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewAllowCellMerge"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowCellMerge {
			get { return allowCellMerge; }
			set {
				if(AllowCellMerge == value) return;
				bool prevValue = AllowCellMerge;
				allowCellMerge = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowCellMerge", prevValue, AllowCellMerge));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewRowAutoHeight"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool RowAutoHeight {
			get { return rowAutoHeight; }
			set {
				if(RowAutoHeight == value) return;
				bool prevValue = RowAutoHeight;
				rowAutoHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("RowAutoHeight", prevValue, RowAutoHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowChildrenInGroupPanel"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowChildrenInGroupPanel {
			get { return showChildrenInGroupPanel; }
			set {
				if(ShowChildrenInGroupPanel == value) return;
				bool prevValue = ShowChildrenInGroupPanel;
				showChildrenInGroupPanel = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowChildrenInGroupPanel", prevValue, ShowChildrenInGroupPanel));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowColumnHeaders"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowColumnHeaders {
			get { return showColumnHeaders; }
			set {
				if(ShowColumnHeaders == value) return;
				bool prevValue = ShowColumnHeaders;
				showColumnHeaders = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowColumnHeaders", prevValue, ShowColumnHeaders));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowAutoFilterRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowAutoFilterRow {
			get { return showAutoFilterRow; }
			set {
				if(ShowAutoFilterRow == value) return;
				bool prevValue = ShowAutoFilterRow;
				showAutoFilterRow = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAutoFilterRow", prevValue, ShowAutoFilterRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowDetailButtons"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowDetailButtons {
			get { return showDetailButtons; }
			set {
				if(ShowDetailButtons == value) return;
				bool prevValue = ShowDetailButtons;
				showDetailButtons = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowDetailButtons", prevValue, ShowDetailButtons));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowGroupExpandCollapseButtons"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowGroupExpandCollapseButtons {
			get { return showGroupExpandCollapseButtons; }
			set {
				if(ShowGroupExpandCollapseButtons == value) return;
				bool prevValue = ShowGroupExpandCollapseButtons;
				showGroupExpandCollapseButtons = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupExpandCollapseButtons", prevValue, ShowGroupExpandCollapseButtons));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowFooter"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowFooter {
			get { return showFooter; }
			set {
				if(ShowFooter == value) return;
				bool prevValue = ShowFooter;
				showFooter = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFooter", prevValue, ShowFooter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowGroupedColumns"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowGroupedColumns {
			get { return showGroupedColumns; }
			set {
				if(ShowGroupedColumns == value) return;
				bool prevValue = ShowGroupedColumns;
				showGroupedColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupedColumns", prevValue, ShowGroupedColumns));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowGroupPanel"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowGroupPanel {
			get { return showGroupPanel; }
			set {
				if(ShowGroupPanel == value) return;
				bool prevValue = ShowGroupPanel;
				showGroupPanel = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupPanel", prevValue, ShowGroupPanel));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowGroupPanelColumnsAsSingleRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowGroupPanelColumnsAsSingleRow {
			get { return showGroupPanelColumnsAsSingleRow; }
			set {
				if(ShowGroupPanelColumnsAsSingleRow == value) return;
				bool prevValue = ShowGroupPanelColumnsAsSingleRow;
				showGroupPanelColumnsAsSingleRow = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupPanelColumnsAsSingleRow", prevValue, ShowGroupPanelColumnsAsSingleRow));
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridView_OptionsViewShowHorzLines)]
		public virtual bool ShowHorzLines {
			get { return ShowHorizontalLines != DefaultBoolean.False; }
			set { ShowHorizontalLines = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridView_OptionsViewShowPreviewLines)]
		public virtual bool ShowPreviewLines {
			get { return ShowPreviewRowLines != DefaultBoolean.False; }
			set { ShowPreviewRowLines = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowHorizontalLines"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowHorizontalLines {
			get { return showHorizontalLines; }
			set {
				if(ShowHorizontalLines == value) return;
				DefaultBoolean prevValue = ShowHorizontalLines;
				showHorizontalLines = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowHorizontalLines", prevValue, ShowHorizontalLines));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowVerticalLines"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowVerticalLines {
			get { return showVerticalLines; }
			set {
				if(ShowVerticalLines == value) return;
				DefaultBoolean prevValue = ShowVerticalLines;
				showVerticalLines = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowVerticalLines", prevValue, ShowVerticalLines));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowPreviewRowLines"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowPreviewRowLines {
			get { return showPreviewRowLines; }
			set {
				if(ShowPreviewRowLines == value) return;
				DefaultBoolean prevValue = ShowPreviewRowLines;
				showPreviewRowLines = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowPreviewRowLines", prevValue, ShowPreviewRowLines));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowIndicator"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowIndicator {
			get { return showIndicator; }
			set {
				if(ShowIndicator == value) return;
				bool prevValue = ShowIndicator;
				showIndicator = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowIndicator", prevValue, ShowIndicator));
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridOptionsView_ShowNewItemRow)]
		public bool ShowNewItemRow {
			get { return NewItemRowPosition != DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None; }
			set {
				NewItemRowPosition = (value ? DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom : DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewShowPreview"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowPreview {
			get { return showPreview; }
			set {
				if(ShowPreview == value) return;
				bool prevValue = ShowPreview;
				showPreview = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowPreview", prevValue, ShowPreview));
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete(ObsoleteText.SRGridView_OptionsViewShowVertLines)]
		public virtual bool ShowVertLines {
			get { return ShowVerticalLines != DefaultBoolean.False; }
			set { ShowVerticalLines = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewEnableAppearanceOddRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableAppearanceOddRow {
			get { return enableAppearanceOddRow; }
			set {
				if(EnableAppearanceOddRow == value) return;
				bool prevValue = EnableAppearanceOddRow;
				enableAppearanceOddRow = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceOddRow", prevValue, EnableAppearanceOddRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsViewEnableAppearanceEvenRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableAppearanceEvenRow {
			get { return enableAppearanceEvenRow; }
			set {
				if(EnableAppearanceEvenRow == value) return;
				bool prevValue = EnableAppearanceEvenRow;
				enableAppearanceEvenRow = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceEvenRow", prevValue, EnableAppearanceEvenRow));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsView opt = options as GridOptionsView;
				if(opt == null) return;
				if(AllowAssignSplitOptions) {
					this.showAutoFilterRow = opt.ShowAutoFilterRow;
					this.showColumnHeaders = opt.ShowColumnHeaders;
					this.showFooter = opt.ShowFooter;
					this.showGroupPanel = opt.ShowGroupPanel;
					this.newItemRowPosition = opt.NewItemRowPosition;
					this.showChildrenInGroupPanel = opt.ShowChildrenInGroupPanel;
				}
				this.columnHeaderAutoHeight = opt.ColumnHeaderAutoHeight;
				this.bestFitMaxRowCount = opt.BestFitMaxRowCount;
				this.bestFitUseErrorInfo = opt.BestFitUseErrorInfo;
				this.groupFooterShowMode = opt.GroupFooterShowMode;
				this.allowHtmlDrawGroups = opt.AllowHtmlDrawGroups;
				this.showGroupExpandCollapseButtons = opt.ShowGroupExpandCollapseButtons;
				this.headerFilterButtonShowMode = opt.HeaderFilterButtonShowMode;
				this.enableAppearanceEvenRow = opt.EnableAppearanceEvenRow;
				this.enableAppearanceOddRow = opt.EnableAppearanceOddRow;
				this.allowCellMerge = opt.AllowCellMerge;
				this.autoCalcPreviewLineCount = opt.AutoCalcPreviewLineCount;
				this.columnAutoWidth = opt.ColumnAutoWidth;
				this.rowAutoHeight = opt.RowAutoHeight;
				this.showDetailButtons = opt.ShowDetailButtons;
				this.showGroupedColumns = opt.ShowGroupedColumns;
				this.showVerticalLines = opt.ShowVerticalLines;
				this.showHorizontalLines = opt.ShowHorizontalLines;
				this.showPreviewRowLines = opt.ShowPreviewRowLines;
				this.showIndicator = opt.ShowIndicator;
				this.showPreview = opt.ShowPreview;
				this.groupDrawMode = opt.GroupDrawMode;
				this.waitAnimationOptions = opt.WaitAnimationOptions;
				this.allowHtmlDrawHeaders = opt.AllowHtmlDrawHeaders;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsPrint : ViewPrintOptionsBase {
		bool autoWidth, expandAllDetails, expandAllGroups, printDetails, 
			printFilterInfo, printFooter, printGroupFooter, printHeader, 
			printHorzLines, printPreview, printVertLines, usePrintStyles, allowMultilineHeaders,
			enableAppearanceEvenRow, enableAppearanceOddRow, splitCellPreviewAcrossPages,
			printSelectedRowsOnly;
		int maxMergedCellHeight = 600;
		public GridOptionsPrint() {
			this.printSelectedRowsOnly = false;
			this.enableAppearanceEvenRow = false;
			this.enableAppearanceOddRow = false;
			this.autoWidth = true;
			this.expandAllDetails = false;
			this.expandAllGroups = true;
			this.printDetails = false;
			this.printFilterInfo = false;
			this.printFooter = true;
			this.printGroupFooter = true;
			this.printHeader = true;
			this.printHorzLines = true;
			this.printPreview = false;
			this.printVertLines = true;
			this.usePrintStyles = true;
			this.splitCellPreviewAcrossPages = false;
		}
		[DefaultValue(600), XtraSerializableProperty()]
		public virtual int MaxMergedCellHeight {
			get { return maxMergedCellHeight; }
			set {
				if(MaxMergedCellHeight == value) return;
				int prevValue = maxMergedCellHeight;
				maxMergedCellHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxMergedCellHeight", prevValue, MaxMergedCellHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintSplitCellPreviewAcrossPages"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool SplitCellPreviewAcrossPages {
			get { return splitCellPreviewAcrossPages; }
			set {
				if(SplitCellPreviewAcrossPages == value) return;
				bool prevValue = splitCellPreviewAcrossPages;
				splitCellPreviewAcrossPages = value;
				OnChanged(new BaseOptionChangedEventArgs("SplitCellPreviewAcrossPages", prevValue, SplitCellPreviewAcrossPages));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintEnableAppearanceOddRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableAppearanceOddRow {
			get { return enableAppearanceOddRow; }
			set {
				if(EnableAppearanceOddRow == value) return;
				bool prevValue = EnableAppearanceOddRow;
				enableAppearanceOddRow = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceOddRow", prevValue, EnableAppearanceOddRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintEnableAppearanceEvenRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableAppearanceEvenRow {
			get { return enableAppearanceEvenRow; }
			set {
				if(EnableAppearanceEvenRow == value) return;
				bool prevValue = EnableAppearanceEvenRow;
				enableAppearanceEvenRow = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableAppearanceEvenRow", prevValue, EnableAppearanceEvenRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintAutoWidth"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoWidth {
			get { return autoWidth; }
			set {
				if(AutoWidth == value) return;
				bool prevValue = AutoWidth;
				autoWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoWidth", prevValue, AutoWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintExpandAllDetails"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ExpandAllDetails {
			get { return expandAllDetails; }
			set {
				if(ExpandAllDetails == value) return;
				bool prevValue = ExpandAllDetails;
				expandAllDetails = value;
				OnChanged(new BaseOptionChangedEventArgs("ExpandAllDetails", prevValue, ExpandAllDetails));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintExpandAllGroups"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ExpandAllGroups {
			get { return expandAllGroups; }
			set {
				if(ExpandAllGroups == value) return;
				bool prevValue = ExpandAllGroups;
				expandAllGroups = value;
				OnChanged(new BaseOptionChangedEventArgs("ExpandAllGroups", prevValue, ExpandAllGroups));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintDetails"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintDetails {
			get { return printDetails; }
			set {
				if(PrintDetails == value) return;
				bool prevValue = PrintDetails;
				printDetails = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintDetails", prevValue, PrintDetails));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintFilterInfo"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintFilterInfo {
			get { return printFilterInfo; }
			set {
				if(PrintFilterInfo == value) return;
				bool prevValue = PrintFilterInfo;
				printFilterInfo = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintFilterInfo", prevValue, PrintFilterInfo));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintFooter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintFooter {
			get { return printFooter; }
			set {
				if(PrintFooter == value) return;
				bool prevValue = PrintFooter;
				printFooter = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintFooter", prevValue, PrintFooter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintGroupFooter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintGroupFooter {
			get { return printGroupFooter; }
			set {
				if(PrintGroupFooter == value) return;
				bool prevValue = PrintGroupFooter;
				printGroupFooter = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintGroupFooter", prevValue, PrintGroupFooter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintHeader"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintHeader {
			get { return printHeader; }
			set {
				if(PrintHeader == value) return;
				bool prevValue = PrintHeader;
				printHeader = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintHeader", prevValue, PrintHeader));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintHorzLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintHorzLines {
			get { return printHorzLines; }
			set {
				if(PrintHorzLines == value) return;
				bool prevValue = PrintHorzLines;
				printHorzLines = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintHorzLines", prevValue, PrintHorzLines));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintPreview"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintPreview {
			get { return printPreview; }
			set {
				if(PrintPreview == value) return;
				bool prevValue = PrintPreview;
				printPreview = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintPreview", prevValue, PrintPreview));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintSelectedRowsOnly"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintSelectedRowsOnly {
			get { return printSelectedRowsOnly; }
			set {
				if(PrintSelectedRowsOnly == value) return;
				bool prevValue = PrintSelectedRowsOnly;
				printSelectedRowsOnly = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintSelectedRowsOnly", prevValue, PrintSelectedRowsOnly));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintPrintVertLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintVertLines {
			get { return printVertLines; }
			set {
				if(PrintVertLines == value) return;
				bool prevValue = PrintVertLines;
				printVertLines = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintVertLines", prevValue, PrintVertLines));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsPrintUsePrintStyles"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UsePrintStyles {
			get { return usePrintStyles; }
			set {
				if(UsePrintStyles == value) return;
				bool prevValue = UsePrintStyles;
				usePrintStyles = value;
				OnChanged(new BaseOptionChangedEventArgs("UsePrintStyles", prevValue, UsePrintStyles));
			}
		}
		[ DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowMultilineHeaders {
			get { return allowMultilineHeaders; }
			set {
				if(AllowMultilineHeaders == value) return;
				bool prevValue = AllowMultilineHeaders;
				allowMultilineHeaders = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowMultiLineHeaders", prevValue, AllowMultilineHeaders));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsPrint opt = options as GridOptionsPrint;
				if(opt == null) return;
				this.printSelectedRowsOnly = opt.PrintSelectedRowsOnly;
				this.enableAppearanceOddRow = opt.EnableAppearanceOddRow;
				this.enableAppearanceEvenRow = opt.EnableAppearanceEvenRow;
				this.autoWidth = opt.AutoWidth;
				this.expandAllDetails = opt.ExpandAllDetails;
				this.expandAllGroups = opt.ExpandAllGroups;
				this.printDetails = opt.PrintDetails;
				this.printFilterInfo = opt.PrintFilterInfo;
				this.printFooter = opt.PrintFooter;
				this.printGroupFooter = opt.PrintGroupFooter;
				this.printHeader = opt.PrintHeader;
				this.printHorzLines = opt.PrintHorzLines;
				this.printPreview = opt.PrintPreview;
				this.printVertLines = opt.PrintVertLines;
				this.usePrintStyles = opt.UsePrintStyles;
				this.maxMergedCellHeight = opt.MaxMergedCellHeight;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsMenu : ViewBaseOptions {
		bool enableColumnMenu, enableFooterMenu, enableGroupPanelMenu, showGroupSortSummaryItems,
			showDateTimeGroupIntervalItems, showGroupSummaryEditorItem, showAutoFilterRowItem, showSplitItem, showConditionalFormattingItem;
		FormBorderEffect dialogFormBorderEffect;
		DefaultBoolean showAddNewSummaryItem;
		public GridOptionsMenu() {
			this.enableColumnMenu = true;
			this.enableFooterMenu = true;
			this.enableGroupPanelMenu = true;
			this.showGroupSortSummaryItems = true;
			this.showDateTimeGroupIntervalItems = true;
			this.showGroupSummaryEditorItem = false;
			this.showAutoFilterRowItem = true;
			this.showAddNewSummaryItem = DefaultBoolean.Default;
			this.showSplitItem = true;
			this.showConditionalFormattingItem = false;
			this.dialogFormBorderEffect = FormBorderEffect.Default;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuEnableColumnMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableColumnMenu {
			get { return enableColumnMenu; }
			set {
				if(EnableColumnMenu == value) return;
				bool prevValue = EnableColumnMenu;
				enableColumnMenu = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableColumnMenu", prevValue, EnableColumnMenu));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowGroupSortSummaryItems"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowGroupSortSummaryItems {
			get { return showGroupSortSummaryItems; }
			set {
				if(ShowGroupSortSummaryItems == value) return;
				bool prevValue = showGroupSortSummaryItems;
				showGroupSortSummaryItems = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupSortSummaryItems", prevValue, ShowGroupSortSummaryItems));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowDateTimeGroupIntervalItems"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowDateTimeGroupIntervalItems {
			get { return showDateTimeGroupIntervalItems; }
			set {
				if(ShowDateTimeGroupIntervalItems == value) return;
				bool prevValue = ShowDateTimeGroupIntervalItems;
				showDateTimeGroupIntervalItems = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowDateTimeGroupIntervalItems", prevValue, ShowDateTimeGroupIntervalItems));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowGroupSummaryEditorItem"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowGroupSummaryEditorItem {
			get { return showGroupSummaryEditorItem; }
			set {
				if(ShowGroupSummaryEditorItem == value) return;
				bool prevValue = ShowGroupSummaryEditorItem;
				showGroupSummaryEditorItem = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowGroupSummaryEditorItem", prevValue, ShowGroupSummaryEditorItem));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowAutoFilterRowItem"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowAutoFilterRowItem {
			get { return showAutoFilterRowItem; }
			set {
				if(ShowAutoFilterRowItem == value) return;
				bool prevValue = ShowAutoFilterRowItem;
				showAutoFilterRowItem = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAutoFilterRowItem", prevValue, ShowAutoFilterRowItem));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowAddNewSummaryItem"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowAddNewSummaryItem {
			get { return showAddNewSummaryItem; }
			set {
				if(ShowAddNewSummaryItem == value) return;
				DefaultBoolean prevValue = ShowAddNewSummaryItem;
				showAddNewSummaryItem = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAddNewSummaryItem", prevValue, ShowAddNewSummaryItem));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowSplitItem"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowSplitItem {
			get { return showSplitItem; }
			set {
				if(ShowSplitItem == value) return;
				bool prevValue = ShowSplitItem;
				showSplitItem = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowSplitItem", prevValue, ShowSplitItem));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuShowConditionalFormattingItem"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowConditionalFormattingItem {
			get { return showConditionalFormattingItem; }
			set {
				if(ShowConditionalFormattingItem == value) return;
				bool prevValue = ShowConditionalFormattingItem;
				showConditionalFormattingItem = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowConditionalFormattingItem", prevValue, ShowConditionalFormattingItem));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuEnableFooterMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableFooterMenu {
			get { return enableFooterMenu; }
			set {
				if(EnableFooterMenu == value) return;
				bool prevValue = EnableFooterMenu;
				enableFooterMenu = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableFooterMenu", prevValue, EnableFooterMenu));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuEnableGroupPanelMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool EnableGroupPanelMenu {
			get { return enableGroupPanelMenu; }
			set {
				if(EnableGroupPanelMenu == value) return;
				bool prevValue = EnableGroupPanelMenu;
				enableGroupPanelMenu = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableGroupPanelMenu", prevValue, EnableGroupPanelMenu));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsMenuDialogFormBorderEffect"),
#endif
 DefaultValue(FormBorderEffect.Default), XtraSerializableProperty()]
		public virtual FormBorderEffect DialogFormBorderEffect {
			get { return dialogFormBorderEffect; }
			set {
				if(DialogFormBorderEffect == value) return;
				FormBorderEffect prevValue = DialogFormBorderEffect;
				dialogFormBorderEffect = value;
				OnChanged(new BaseOptionChangedEventArgs("DialogFormBorderEffect", prevValue, DialogFormBorderEffect));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsMenu opt = options as GridOptionsMenu;
				if(opt == null) return;
				this.showGroupSortSummaryItems = opt.ShowGroupSortSummaryItems;
				this.showDateTimeGroupIntervalItems = opt.ShowDateTimeGroupIntervalItems;
				this.showGroupSummaryEditorItem = opt.ShowGroupSummaryEditorItem;
				this.enableColumnMenu = opt.EnableColumnMenu;
				this.enableFooterMenu = opt.EnableFooterMenu;
				this.enableGroupPanelMenu = opt.EnableGroupPanelMenu;
				this.showAutoFilterRowItem = opt.ShowAutoFilterRowItem;
				this.showAddNewSummaryItem = opt.ShowAddNewSummaryItem;
				this.showSplitItem = opt.ShowSplitItem;
				this.showConditionalFormattingItem = opt.ShowConditionalFormattingItem;
				this.dialogFormBorderEffect = opt.DialogFormBorderEffect;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class GridOptionsHint : ViewBaseOptions {
		bool showCellHints, showColumnHeaderHints, showFooterHints;
		public GridOptionsHint() {
			this.showCellHints = true;
			this.showColumnHeaderHints = true;
			this.showFooterHints = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsHintShowCellHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCellHints {
			get { return showCellHints; }
			set {
				if(ShowCellHints == value) return;
				bool prevValue = ShowCellHints;
				showCellHints = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCellHints", prevValue, ShowCellHints));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsHintShowFooterHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFooterHints {
			get { return showFooterHints; }
			set {
				if(ShowFooterHints == value) return;
				bool prevValue = ShowFooterHints;
				showFooterHints = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFooterHints", prevValue, ShowFooterHints));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsHintShowColumnHeaderHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowColumnHeaderHints {
			get { return showColumnHeaderHints; }
			set {
				if(ShowColumnHeaderHints == value) return;
				bool prevValue = ShowColumnHeaderHints;
				showColumnHeaderHints = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowColumnHeaderHints", prevValue, ShowColumnHeaderHints));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GridOptionsHint opt = options as GridOptionsHint;
				if(opt == null) return;
				this.showFooterHints = opt.ShowFooterHints;
				this.showCellHints = opt.ShowCellHints;
				this.showColumnHeaderHints = opt.ShowColumnHeaderHints;
			}
			finally {
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridViewOptionsImageLoad : BaseOptions {
		bool asyncLoad, randowShow;
		Size desiredThumbnailSize;
		ImageContentAnimationType animationType;
		public GridViewOptionsImageLoad() {
			this.asyncLoad = false;
			this.randowShow = true;
			this.animationType = ImageContentAnimationType.None;
			this.desiredThumbnailSize = Size.Empty;
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
		[ DefaultValue(false)]
		public bool AsyncLoad {
			get { return asyncLoad; }
			set { asyncLoad = value; }
		}
		[ DefaultValue(ImageContentAnimationType.None)]
		public ImageContentAnimationType AnimationType {
			get { return animationType; }
			set { animationType = value; }
		}
		[ DefaultValue(true)]
		public bool RandomShow {
			get { return randowShow; }
			set { randowShow = value; }
		}
		void ResetDesiredThumbnailSize() { DesiredThumbnailSize = Size.Empty; }
		bool ShouldSerializeDesiredThumbnailSize() { return DesiredThumbnailSize != Size.Empty; }
		public Size DesiredThumbnailSize {
			get { return desiredThumbnailSize; }
			set { desiredThumbnailSize = value; }
		}
		public override void Assign(BaseOptions options) {
			GridViewOptionsImageLoad optionsImageLoad = options as GridViewOptionsImageLoad;
			if(optionsImageLoad == null)
				return;
			this.animationType = optionsImageLoad.animationType;
			this.asyncLoad = optionsImageLoad.asyncLoad;
			this.randowShow = optionsImageLoad.randowShow;
			this.desiredThumbnailSize = optionsImageLoad.desiredThumbnailSize;
		}
	}
	public class GridOptionsClipboard :ClipboardOptions {
		DefaultBoolean allowCopy = DefaultBoolean.Default;
		public GridOptionsClipboard(bool allowFormattedMode = true) : base(allowFormattedMode) {
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridOptionsClipboardAllowCopy"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowCopy {
			get { return allowCopy; }
			set {
				if(AllowCopy == value) return;
				allowCopy = value;
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			GridOptionsClipboard op = options as GridOptionsClipboard;
			if(op == null) return;
			allowCopy = op.AllowCopy;
		}
	}
}
