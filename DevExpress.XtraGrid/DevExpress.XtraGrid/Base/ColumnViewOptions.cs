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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.EditForm;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Localization;
namespace DevExpress.XtraGrid.Columns {
	public class OptionsColumn : ViewBaseOptions {
		bool allowMove, allowSize, readOnly, allowFocus, fixedWidth, allowShowHide,
			showInCustomizationForm, showInExpressionEditor, allowEdit, showCaption, allowIncrementalSearch, tabStop;
		DefaultBoolean allowMerge, allowSort, allowGroup, printable, immediateUpdateRowPosition;
		public OptionsColumn() {
			this.tabStop = true;
			this.allowShowHide = true;
			this.showCaption = true;
			this.allowIncrementalSearch = true;
			this.allowEdit = true;
			this.allowMove = true;
			this.allowSize = true;
			this.allowFocus = true;
			this.readOnly = false;
			this.fixedWidth = false;
			this.showInCustomizationForm = true;
			this.showInExpressionEditor = true;
			this.allowMerge = DefaultBoolean.Default;
			this.allowGroup = DefaultBoolean.Default;
			this.allowSort = DefaultBoolean.Default;
			this.printable = DefaultBoolean.Default;
			this.immediateUpdateRowPosition = DefaultBoolean.Default;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnShowCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowCaption {
			get { return showCaption; }
			set {
				if(ShowCaption == value) return;
				bool prevValue = ShowCaption;
				showCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCaption", prevValue, ShowCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnPrintable"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean Printable {
			get { return printable; }
			set {
				if(Printable == value) return;
				DefaultBoolean prevValue = Printable;
				printable = value;
				OnChanged(new BaseOptionChangedEventArgs("Printable", prevValue, Printable));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnImmediateUpdateRowPosition"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean ImmediateUpdateRowPosition {
			get { return immediateUpdateRowPosition; }
			set {
				if(ImmediateUpdateRowPosition == value) return;
				DefaultBoolean prevValue = ImmediateUpdateRowPosition;
				immediateUpdateRowPosition = value;
				OnChanged(new BaseOptionChangedEventArgs("ImmediateUpdateRowPosition", prevValue, ImmediateUpdateRowPosition));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnTabStop"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool TabStop {
			get { return tabStop; }
			set {
				if(TabStop == value) return;
				bool prevValue = TabStop;
				tabStop = value;
				OnChanged(new BaseOptionChangedEventArgs("TabStop", prevValue, TabStop));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowEdit"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowEdit {
			get { return allowEdit; }
			set {
				if(AllowEdit == value) return;
				bool prevValue = AllowEdit;
				allowEdit = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowEdit", prevValue, AllowEdit));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowIncrementalSearch"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowIncrementalSearch {
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
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowMove"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowMove {
			get { return allowMove; }
			set {
				if(AllowMove == value) return;
				bool prevValue = AllowMove;
				allowMove = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowMove", prevValue, AllowMove));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowShowHide"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowShowHide {
			get { return allowShowHide; }
			set {
				if(AllowShowHide == value) return;
				bool prevValue = AllowShowHide;
				allowShowHide = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowShowHide", prevValue, AllowShowHide));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowGroup"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowGroup {
			get { return allowGroup; }
			set {
				if(!Enum.IsDefined(typeof(DefaultBoolean), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(DefaultBoolean));
				}
				if(AllowGroup == value) return;
				DefaultBoolean prevValue = AllowGroup;
				allowGroup = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowGroup", prevValue, AllowGroup));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowSort"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowSort {
			get { return allowSort; }
			set {
				if(!Enum.IsDefined(typeof(DefaultBoolean), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(DefaultBoolean));
				}
				if(AllowSort == value) return;
				DefaultBoolean prevValue = AllowSort;
				allowSort = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowSort", prevValue, AllowSort));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowMerge"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowMerge {
			get { return allowMerge; }
			set {
				if(!Enum.IsDefined(typeof(DefaultBoolean), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(DefaultBoolean));
				}
				if(AllowMerge == value) return;
				DefaultBoolean prevValue = AllowMerge;
				allowMerge = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowMerge", prevValue, AllowMerge));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowSize"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowSize {
			get { return allowSize; }
			set {
				if(AllowSize == value) return;
				bool prevValue = AllowSize;
				allowSize = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowSize", prevValue, AllowSize));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnAllowFocus"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowFocus {
			get { return allowFocus; }
			set {
				if(AllowFocus == value) return;
				bool prevValue = AllowFocus;
				allowFocus = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFocus", prevValue, AllowFocus));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnReadOnly"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if(ReadOnly == value) return;
				bool prevValue = ReadOnly;
				readOnly = value;
				OnChanged(new BaseOptionChangedEventArgs("ReadOnly", prevValue, ReadOnly));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFixedWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool FixedWidth {
			get { return fixedWidth; }
			set {
				if(FixedWidth == value) return;
				bool prevValue = FixedWidth;
				fixedWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("FixedWidth", prevValue, FixedWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnShowInCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowInCustomizationForm {
			get { return showInCustomizationForm; }
			set {
				if(ShowInCustomizationForm == value) return;
				bool prevValue = ShowInCustomizationForm;
				showInCustomizationForm = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowInCustomizationForm", prevValue, ShowInCustomizationForm));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnShowInExpressionEditor"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowInExpressionEditor {
			get { return showInExpressionEditor; }
			set {
				if(ShowInExpressionEditor == value) return;
				bool prevValue = ShowInExpressionEditor;
				showInExpressionEditor = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowInExpressionEditor", prevValue, ShowInExpressionEditor));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				OptionsColumn opt = options as OptionsColumn;
				if(opt == null) return;
				this.printable = opt.Printable;
				this.immediateUpdateRowPosition = opt.ImmediateUpdateRowPosition;
				this.allowShowHide = opt.AllowShowHide;
				this.showCaption = opt.ShowCaption;
				this.allowMerge = opt.allowMerge;
				this.allowEdit = opt.AllowEdit;
				this.allowIncrementalSearch = opt.AllowIncrementalSearch;
				this.allowMove = opt.AllowMove;
				this.allowGroup = opt.AllowGroup;
				this.allowSize = opt.AllowSize;
				this.allowSort = opt.AllowSort;
				this.readOnly = opt.ReadOnly;
				this.allowFocus = opt.AllowFocus;
				this.fixedWidth = opt.FixedWidth;
				this.tabStop = opt.TabStop;
				this.showInCustomizationForm = opt.ShowInCustomizationForm;
				this.showInExpressionEditor = opt.ShowInExpressionEditor;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class OptionsColumnEditForm : ViewBaseOptions {
		int columnSpan = 1, rowSpan = 1, visibleIndex = 0;
		EditFormColumnCaptionLocation captionLocation = EditFormColumnCaptionLocation.Default;
		DefaultBoolean visible = DefaultBoolean.Default;
		string caption = "";
		bool useEditorColRowSpan = true, startNewRow = false;
		public OptionsColumnEditForm() {
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				OptionsColumnEditForm opt = options as OptionsColumnEditForm;
				if(opt == null) return;
				this.startNewRow = opt.StartNewRow;
				this.useEditorColRowSpan = opt.UseEditorColRowSpan;
				this.columnSpan = opt.ColumnSpan;
				this.rowSpan = opt.RowSpan;
				this.captionLocation = opt.CaptionLocation;
				this.visible = opt.Visible;
				this.visibleIndex = opt.VisibleIndex;
				this.caption = opt.Caption;
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormUseEditorColRowSpan"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseEditorColRowSpan {
			get { return useEditorColRowSpan; }
			set {
				if(UseEditorColRowSpan == value) return;
				bool prevValue = UseEditorColRowSpan;
				useEditorColRowSpan = value;
				OnChanged(new BaseOptionChangedEventArgs("UseEditorColRowSpan", prevValue, UseEditorColRowSpan));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormStartNewRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool StartNewRow {
			get { return startNewRow; }
			set {
				if(StartNewRow == value) return;
				bool prevValue = StartNewRow;
				startNewRow = value;
				OnChanged(new BaseOptionChangedEventArgs("StartNewRow", prevValue, StartNewRow));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormVisibleIndex"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int VisibleIndex {
			get { return visibleIndex; }
			set {
				if(VisibleIndex == value) return;
				int prevValue = VisibleIndex;
				visibleIndex = value;
				OnChanged(new BaseOptionChangedEventArgs("VisibleIndex", prevValue, VisibleIndex));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormColumnSpan"),
#endif
 DefaultValue(1), XtraSerializableProperty()]
		public virtual int ColumnSpan {
			get { return columnSpan; }
			set {
				if(ColumnSpan == value) return;
				int prevValue = ColumnSpan;
				columnSpan = value;
				OnChanged(new BaseOptionChangedEventArgs("ColumnSpan", prevValue, ColumnSpan));
			}
		}
		[ DefaultValue(1), XtraSerializableProperty()]
		public virtual int RowSpan {
			get { return rowSpan; }
			set {
				if(RowSpan == value) return;
				int prevValue = RowSpan;
				rowSpan = value;
				OnChanged(new BaseOptionChangedEventArgs("RowSpan", prevValue, RowSpan));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormCaptionLocation"),
#endif
 DefaultValue(EditFormColumnCaptionLocation.Default), XtraSerializableProperty()]
		public virtual EditFormColumnCaptionLocation CaptionLocation {
			get { return captionLocation; }
			set {
				if(CaptionLocation == value) return;
				EditFormColumnCaptionLocation prevValue = CaptionLocation;
				captionLocation = value;
				OnChanged(new BaseOptionChangedEventArgs("CaptionLocation", prevValue, CaptionLocation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty()]
		public virtual string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				string prevValue = Caption;
				caption = value;
				OnChanged(new BaseOptionChangedEventArgs("Caption", prevValue, Caption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnEditFormVisible"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				DefaultBoolean prevValue = Visible;
				visible = value;
				OnChanged(new BaseOptionChangedEventArgs("Visible", prevValue, Visible));
			}
		}
	}
	public class OptionsColumnFilter : ViewBaseOptions {
		DefaultBoolean immediateUpdatePopupDateFilterOnCheck, immediateUpdatePopupDateFilterOnDateChange, allowFilterModeChanging,
			filterBySortField, showBlanksFilterItems;
		bool allowFilter, allowAutoFilter, immediateUpdateAutoFilter, showEmptyDateFilter;
		AutoFilterCondition autoFilterCondition;
		FilterPopupMode filterPopupMode;
		public OptionsColumnFilter() {
			this.filterBySortField = DefaultBoolean.Default;
			this.showEmptyDateFilter = false;
			this.autoFilterCondition = AutoFilterCondition.Default;
			this.filterPopupMode = FilterPopupMode.Default;
			this.allowFilter = true;
			this.immediateUpdatePopupDateFilterOnDateChange = DefaultBoolean.Default;
			this.immediateUpdatePopupDateFilterOnCheck = DefaultBoolean.Default;
			this.allowAutoFilter = true;
			this.immediateUpdateAutoFilter = true;
			this.allowFilterModeChanging = DefaultBoolean.Default;
			this.showBlanksFilterItems = DefaultBoolean.Default;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				OptionsColumnFilter opt = options as OptionsColumnFilter;
				if(opt == null) return;
				this.filterBySortField = opt.FilterBySortField;
				this.showEmptyDateFilter = opt.ShowEmptyDateFilter;
				this.immediateUpdatePopupDateFilterOnDateChange = opt.ImmediateUpdatePopupDateFilterOnDateChange;
				this.immediateUpdatePopupDateFilterOnCheck = opt.ImmediateUpdatePopupDateFilterOnCheck;
				this.autoFilterCondition = opt.AutoFilterCondition;
				this.filterPopupMode = opt.FilterPopupMode;
				this.allowFilter = opt.AllowFilter;
				this.allowAutoFilter = opt.AllowAutoFilter;
				this.immediateUpdateAutoFilter = opt.ImmediateUpdateAutoFilter;
				this.allowFilterModeChanging = opt.AllowFilterModeChanging;
				this.showBlanksFilterItems = opt.ShowBlanksFilterItems;
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterAllowFilter"),
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
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterAutoFilterCondition"),
#endif
 DefaultValue(AutoFilterCondition.Default), XtraSerializableProperty()]
		public virtual AutoFilterCondition AutoFilterCondition {
			get { return autoFilterCondition; }
			set {
				if(AutoFilterCondition == value) return;
				AutoFilterCondition prevValue = AutoFilterCondition;
				autoFilterCondition = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoFilterCondition", prevValue, AutoFilterCondition));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterAllowAutoFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowAutoFilter {
			get { return allowAutoFilter; }
			set {
				if(AllowAutoFilter == value) return;
				bool prevValue = AllowAutoFilter;
				allowAutoFilter = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAutoFilter", prevValue, AllowAutoFilter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterImmediateUpdateAutoFilter"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ImmediateUpdateAutoFilter {
			get { return immediateUpdateAutoFilter; }
			set {
				if(ImmediateUpdateAutoFilter == value) return;
				bool prevValue = ImmediateUpdateAutoFilter;
				immediateUpdateAutoFilter = value;
				OnChanged(new BaseOptionChangedEventArgs("ImmediateUpdateAutoFilter", prevValue, ImmediateUpdateAutoFilter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterShowBlanksFilterItems"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ShowBlanksFilterItems {
			get { return showBlanksFilterItems; }
			set {
				if(ShowBlanksFilterItems == value) return;
				DefaultBoolean prevValue = ShowBlanksFilterItems;
				showBlanksFilterItems = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowBlanksFilterItems", prevValue, ShowBlanksFilterItems));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterFilterBySortField"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean FilterBySortField {
			get { return filterBySortField; }
			set {
				if(FilterBySortField == value) return;
				DefaultBoolean prevValue = FilterBySortField;
				filterBySortField = value;
				OnChanged(new BaseOptionChangedEventArgs("FilterBySortField", prevValue, FilterBySortField));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterImmediateUpdatePopupDateFilterOnCheck"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ImmediateUpdatePopupDateFilterOnCheck {
			get { return immediateUpdatePopupDateFilterOnCheck; }
			set {
				if(ImmediateUpdatePopupDateFilterOnCheck == value) return;
				DefaultBoolean prevValue = ImmediateUpdatePopupDateFilterOnCheck;
				immediateUpdatePopupDateFilterOnCheck = value;
				OnChanged(new BaseOptionChangedEventArgs("ImmediateUpdatePopupDateFilterOnCheck", prevValue, ImmediateUpdatePopupDateFilterOnCheck));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterImmediateUpdatePopupDateFilterOnDateChange"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean ImmediateUpdatePopupDateFilterOnDateChange {
			get { return immediateUpdatePopupDateFilterOnDateChange; }
			set {
				if(ImmediateUpdatePopupDateFilterOnDateChange == value) return;
				DefaultBoolean prevValue = ImmediateUpdatePopupDateFilterOnDateChange;
				immediateUpdatePopupDateFilterOnDateChange = value;
				OnChanged(new BaseOptionChangedEventArgs("ImmediateUpdatePopupDateFilterOnDateChange", prevValue, ImmediateUpdatePopupDateFilterOnDateChange));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterFilterPopupMode"),
#endif
 DefaultValue(FilterPopupMode.Default), XtraSerializableProperty()]
		public virtual FilterPopupMode FilterPopupMode {
			get { return filterPopupMode; }
			set {
				if(FilterPopupMode == value) return;
				FilterPopupMode prevValue = FilterPopupMode;
				filterPopupMode = value;
				OnChanged(new BaseOptionChangedEventArgs("FilterPopupMode", prevValue, FilterPopupMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterShowEmptyDateFilter"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowEmptyDateFilter {
			get { return showEmptyDateFilter; }
			set {
				if(ShowEmptyDateFilter == value) return;
				bool prevValue = ShowEmptyDateFilter;
				showEmptyDateFilter = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowEmptyDateFilter", prevValue, ShowEmptyDateFilter));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("OptionsColumnFilterAllowFilterModeChanging"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowFilterModeChanging {
			get { return allowFilterModeChanging; }
			set {
				if(AllowFilterModeChanging == value) return;
				DefaultBoolean prevValue = AllowFilterModeChanging;
				allowFilterModeChanging = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFilterModeChanging", prevValue, AllowFilterModeChanging));
			}
		}
	}
}
namespace DevExpress.XtraGrid.Views.Base {
	public class ColumnViewOptionsFind : ViewBaseOptions {
		int findDelay;
		bool allowFindPanel, showCloseButton, alwaysVisible, clearFindOnClose, highlightFindResults, showClearButton, showFindButton;
		FindMode findMode;
		string findFilterColumns;
		string findNullPrompt;
		public ColumnViewOptionsFind() {
			this.highlightFindResults = true;
			this.clearFindOnClose = true;
			this.findDelay = 1000;
			this.alwaysVisible = false;
			this.allowFindPanel = true;
			this.showClearButton = this.showFindButton = this.showCloseButton = true;
			this.findFilterColumns = "*";
			this.findNullPrompt = defaultFindNullPrompt;
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindFindMode"),
#endif
 DefaultValue(FindMode.Default)]
		public FindMode FindMode {
			get { return findMode; }
			set {
				if(FindMode == value) return;
				FindMode prevValue = FindMode;
				findMode = value;
				OnChanged(new BaseOptionChangedEventArgs("FindMode", prevValue, FindMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindFindDelay"),
#endif
 DefaultValue(1000), XtraSerializableProperty()]
		public virtual int FindDelay {
			get { return findDelay; }
			set {
				if(value < 100) value = 100;
				if(FindDelay == value) return;
				int prevValue = FindDelay;
				findDelay = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFindFilter", prevValue, FindDelay));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindAllowFindPanel"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFindPanel {
			get { return allowFindPanel; }
			set {
				if(AllowFindPanel == value) return;
				bool prevValue = AllowFindPanel;
				allowFindPanel = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFindPanel", prevValue, AllowFindPanel));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindHighlightFindResults"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool HighlightFindResults {
			get { return highlightFindResults; }
			set {
				if(HighlightFindResults == value) return;
				bool prevValue = HighlightFindResults;
				highlightFindResults = value;
				OnChanged(new BaseOptionChangedEventArgs("HighlightFindResults", prevValue, HighlightFindResults));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindClearFindOnClose"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ClearFindOnClose {
			get { return clearFindOnClose; }
			set {
				if(ClearFindOnClose == value) return;
				bool prevValue = ClearFindOnClose;
				clearFindOnClose = value;
				OnChanged(new BaseOptionChangedEventArgs("ClearFindOnClose", prevValue, ClearFindOnClose));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindShowCloseButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value) return;
				bool prevValue = ShowCloseButton;
				showCloseButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCloseButton", prevValue, ShowCloseButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindShowFindButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFindButton {
			get { return showFindButton; }
			set {
				if(ShowFindButton == value) return;
				bool prevValue = ShowFindButton;
				showFindButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFindButton", prevValue, ShowFindButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindShowClearButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowClearButton {
			get { return showClearButton; }
			set {
				if(ShowClearButton == value) return;
				bool prevValue = ShowClearButton;
				showClearButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowClearButton", prevValue, ShowClearButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindAlwaysVisible"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AlwaysVisible {
			get { return alwaysVisible; }
			set {
				if(AlwaysVisible == value) return;
				bool prevValue = AlwaysVisible;
				alwaysVisible = value;
				OnChanged(new BaseOptionChangedEventArgs("AlwaysVisible", prevValue, AlwaysVisible));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindFindFilterColumns"),
#endif
 DefaultValue("*"), XtraSerializableProperty()]
		public virtual string FindFilterColumns {
			get { return findFilterColumns; }
			set {
				if(value == null) value = "";
				if(FindFilterColumns == value) return;
				string prevValue = FindFilterColumns;
				findFilterColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("FindFilterColumns", prevValue, FindFilterColumns));
			}
		}
		string defaultFindNullPrompt { get { return GridLocalizer.Active.GetLocalizedString(GridStringId.FindNullPrompt); } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFindFindNullPrompt"),
#endif
 XtraSerializableProperty()]
		public virtual string FindNullPrompt {
			get { return findNullPrompt; }
			set {
				if(value == null) value = "";
				if(FindNullPrompt == value) return;
				string prevValue = FindNullPrompt;
				findNullPrompt = value;
				OnChanged(new BaseOptionChangedEventArgs("FindNullPrompt", prevValue, FindNullPrompt));
			}
		}
		bool ShouldSerializeFindNullPrompt() { return FindNullPrompt != defaultFindNullPrompt; }
		void ResetFindNullPrompt() { FindNullPrompt = defaultFindNullPrompt; }
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ColumnViewOptionsFind opt = options as ColumnViewOptionsFind;
				if(opt == null) return;
				this.findNullPrompt = opt.FindNullPrompt;
				this.findMode = opt.FindMode;
				this.alwaysVisible = opt.AlwaysVisible;
				this.allowFindPanel = opt.AllowFindPanel;
				this.showCloseButton = opt.ShowCloseButton;
				this.showFindButton = opt.ShowFindButton;
				this.showClearButton = opt.ShowClearButton;
				this.findFilterColumns = opt.FindFilterColumns;
				this.highlightFindResults = opt.HighlightFindResults;
				this.findDelay = opt.FindDelay;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ColumnViewOptionsFilter : ViewBaseOptions {
		bool allowMRUFilterList, allowColumnMRUFilterList, useNewCustomFilterDialog, showAllTableValuesInFilterPopup,
			allowFilterEditor, allowFilterIncrementalSearch, showAllTableValuesInCheckedFilterPopup,
			allowMultiSelectInCheckedFilterPopup, filterEditorUseMenuForOperandsAndOperators;
		int mruFilterListCount, mruFilterListPopupCount, mruColumnFilterListCount, columnFilterPopupMaxRecordsCount,
			columnFilterPopupRowCount, maxCheckedListItemCount;
		FilterEditorViewMode defaultFilterEditorView;
		FilterControlAllowAggregateEditing filterEditorAggregateEditing;
		public ColumnViewOptionsFilter() {
			this.columnFilterPopupMaxRecordsCount = -1;
			this.columnFilterPopupRowCount = 20;
			this.allowMRUFilterList = allowColumnMRUFilterList = allowFilterEditor = true;
			this.allowFilterIncrementalSearch = true;
			this.allowMultiSelectInCheckedFilterPopup = true;
			this.mruFilterListCount = 10;
			this.mruFilterListPopupCount = 7;
			this.mruColumnFilterListCount = 5;
			this.showAllTableValuesInFilterPopup = false;
			this.showAllTableValuesInCheckedFilterPopup = true;
			this.useNewCustomFilterDialog = false;
			this.maxCheckedListItemCount = 25;
			this.defaultFilterEditorView = FilterEditorViewMode.Visual;
			this.filterEditorAggregateEditing = FilterControlAllowAggregateEditing.No;
			this.filterEditorUseMenuForOperandsAndOperators = true;
		}
		[Browsable(false), 
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterFilterEditorAggregateEditing"),
#endif
 DefaultValue(FilterControlAllowAggregateEditing.No), XtraSerializableProperty()]
		public virtual FilterControlAllowAggregateEditing FilterEditorAggregateEditing {
			get { return filterEditorAggregateEditing; }
			set {
				if(FilterEditorAggregateEditing == value) return;
				FilterControlAllowAggregateEditing prevValue = FilterEditorAggregateEditing;
				filterEditorAggregateEditing = value;
				OnChanged(new BaseOptionChangedEventArgs("FilterEditorAggregateEditing", prevValue, FilterEditorAggregateEditing));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterFilterEditorUseMenuForOperandsAndOperators"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool FilterEditorUseMenuForOperandsAndOperators {
			get { return filterEditorUseMenuForOperandsAndOperators; }
			set {
				if(FilterEditorUseMenuForOperandsAndOperators == value) return;
				bool prevValue = FilterEditorUseMenuForOperandsAndOperators;
				filterEditorUseMenuForOperandsAndOperators = value;
				OnChanged(new BaseOptionChangedEventArgs("FilterEditorUseMenuForOperandsAndOperators", prevValue, FilterEditorUseMenuForOperandsAndOperators));
			}
		}
		[ DefaultValue(FilterEditorViewMode.Visual), XtraSerializableProperty()]
		public virtual FilterEditorViewMode DefaultFilterEditorView {
			get { return defaultFilterEditorView; }
			set {
				if(DefaultFilterEditorView == value) return;
				FilterEditorViewMode prevValue = DefaultFilterEditorView;
				defaultFilterEditorView = value;
				OnChanged(new BaseOptionChangedEventArgs("DefaultFilterView", prevValue, DefaultFilterEditorView));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterShowAllTableValuesInCheckedFilterPopup"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowAllTableValuesInCheckedFilterPopup {
			get { return showAllTableValuesInCheckedFilterPopup; }
			set {
				if(ShowAllTableValuesInCheckedFilterPopup == value) return;
				bool prevValue = ShowAllTableValuesInCheckedFilterPopup;
				showAllTableValuesInCheckedFilterPopup = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAllTableValuesInCheckedFilterPopup", prevValue, ShowAllTableValuesInCheckedFilterPopup));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterShowAllTableValuesInFilterPopup"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowAllTableValuesInFilterPopup {
			get { return showAllTableValuesInFilterPopup; }
			set {
				if(ShowAllTableValuesInFilterPopup == value) return;
				bool prevValue = ShowAllTableValuesInFilterPopup;
				showAllTableValuesInFilterPopup = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAllTableValuesInFilterPopup", prevValue, ShowAllTableValuesInFilterPopup));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterUseNewCustomFilterDialog"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool UseNewCustomFilterDialog {
			get { return useNewCustomFilterDialog; }
			set {
				if(UseNewCustomFilterDialog == value) return;
				bool prevValue = UseNewCustomFilterDialog;
				useNewCustomFilterDialog = value;
				OnChanged(new BaseOptionChangedEventArgs("UseNewCustomFilterDialog", prevValue, UseNewCustomFilterDialog));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterColumnFilterPopupMaxRecordsCount"),
#endif
 DefaultValue(-1), XtraSerializableProperty()]
		public virtual int ColumnFilterPopupMaxRecordsCount {
			get { return columnFilterPopupMaxRecordsCount; }
			set {
				if(value < 0) value = -1;
				if(ColumnFilterPopupMaxRecordsCount == value) return;
				int prevValue = ColumnFilterPopupMaxRecordsCount;
				columnFilterPopupMaxRecordsCount = value;
				OnChanged(new BaseOptionChangedEventArgs("ColumnFilterPopupMaxRecordsCount", prevValue, ColumnFilterPopupMaxRecordsCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterColumnFilterPopupRowCount"),
#endif
 DefaultValue(20), XtraSerializableProperty()]
		public virtual int ColumnFilterPopupRowCount {
			get { return columnFilterPopupRowCount; }
			set {
				if(value < 4) value = 4;
				if(value > 100) value = 100;
				if(ColumnFilterPopupRowCount == value) return;
				int prevValue = ColumnFilterPopupRowCount;
				columnFilterPopupRowCount = value;
				OnChanged(new BaseOptionChangedEventArgs("ColumnFilterPopupRowCount", prevValue, ColumnFilterPopupRowCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterMRUFilterListCount"),
#endif
 DefaultValue(10), XtraSerializableProperty()]
		public virtual int MRUFilterListCount {
			get { return mruFilterListCount; }
			set {
				if(value < 0) value = 0;
				if(MRUFilterListCount == value) return;
				int prevValue = MRUFilterListCount;
				mruFilterListCount = value;
				OnChanged(new BaseOptionChangedEventArgs("MRUFilterListCount", prevValue, MRUFilterListCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterMRUColumnFilterListCount"),
#endif
 DefaultValue(5), XtraSerializableProperty()]
		public virtual int MRUColumnFilterListCount {
			get { return mruColumnFilterListCount; }
			set {
				if(value < 0) value = 0;
				if(MRUColumnFilterListCount == value) return;
				int prevValue = MRUColumnFilterListCount;
				mruColumnFilterListCount = value;
				OnChanged(new BaseOptionChangedEventArgs("MRUColumnFilterListCount", prevValue, MRUColumnFilterListCount));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterMaxCheckedListItemCount"),
#endif
 DefaultValue(25), 
		Obsolete("The MaxCheckedListItemCount property is now obsolete and isn't used at all. Instead, use the View.OptionsFilter.ColumnFilterPopupMaxRecordsCount property.")]
		public virtual int MaxCheckedListItemCount {
			get { return maxCheckedListItemCount; }
			set {
				if(value < 1) value = 1;
				if(value > 10000) value = 10000;
				if(MaxCheckedListItemCount == value) return;
				int prevValue = MaxCheckedListItemCount;
				maxCheckedListItemCount = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxCheckedListItemCount", prevValue, MaxCheckedListItemCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterMRUFilterListPopupCount"),
#endif
 DefaultValue(7), XtraSerializableProperty()]
		public virtual int MRUFilterListPopupCount {
			get { return mruFilterListPopupCount; }
			set {
				if(value < 0) value = 0;
				if(MRUFilterListPopupCount == value) return;
				int prevValue = MRUFilterListPopupCount;
				mruFilterListPopupCount = value;
				OnChanged(new BaseOptionChangedEventArgs("MRUFilterListPopupCount", prevValue, MRUFilterListPopupCount));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterAllowColumnMRUFilterList"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowColumnMRUFilterList {
			get { return allowColumnMRUFilterList; }
			set {
				if(AllowColumnMRUFilterList == value) return;
				bool prevValue = AllowColumnMRUFilterList;
				allowColumnMRUFilterList = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowColumnMRUFilterList", prevValue, AllowColumnMRUFilterList));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterAllowMRUFilterList"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowMRUFilterList {
			get { return allowMRUFilterList; }
			set {
				if(AllowMRUFilterList == value) return;
				bool prevValue = AllowMRUFilterList;
				allowMRUFilterList = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowMRUFilterList", prevValue, AllowMRUFilterList));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterAllowFilterEditor"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFilterEditor {
			get { return allowFilterEditor; }
			set {
				if(AllowFilterEditor == value) return;
				bool prevValue = AllowFilterEditor;
				allowFilterEditor = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFilterEditor", prevValue, AllowFilterEditor));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterAllowFilterIncrementalSearch"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowFilterIncrementalSearch {
			get { return allowFilterIncrementalSearch; }
			set {
				if(AllowFilterIncrementalSearch == value) return;
				bool prevValue = AllowFilterIncrementalSearch;
				allowFilterIncrementalSearch = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFilterIncrementalSearch", prevValue, AllowFilterIncrementalSearch));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilterAllowMultiSelectInCheckedFilterPopup"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowMultiSelectInCheckedFilterPopup {
			get { return allowMultiSelectInCheckedFilterPopup; }
			set {
				if(AllowMultiSelectInCheckedFilterPopup == value) return;
				bool prevValue = AllowMultiSelectInCheckedFilterPopup;
				allowMultiSelectInCheckedFilterPopup = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowMultiSelectInCheckedFilterPopup", prevValue, AllowMultiSelectInCheckedFilterPopup));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ColumnViewOptionsFilter opt = options as ColumnViewOptionsFilter;
				if(opt == null) return;
				this.useNewCustomFilterDialog = opt.UseNewCustomFilterDialog;
				this.showAllTableValuesInFilterPopup = opt.ShowAllTableValuesInFilterPopup;
				this.showAllTableValuesInCheckedFilterPopup = opt.ShowAllTableValuesInCheckedFilterPopup;
				this.allowColumnMRUFilterList = opt.AllowColumnMRUFilterList;
				this.allowMRUFilterList = opt.AllowMRUFilterList;
				this.allowFilterEditor = opt.AllowFilterEditor;
				this.allowFilterIncrementalSearch = opt.AllowFilterIncrementalSearch;
				this.allowMultiSelectInCheckedFilterPopup = opt.AllowMultiSelectInCheckedFilterPopup;
				this.mruColumnFilterListCount = opt.MRUColumnFilterListCount;
				this.mruFilterListCount = opt.MRUFilterListCount;
				this.mruFilterListPopupCount = opt.MRUFilterListPopupCount;
				this.columnFilterPopupMaxRecordsCount = opt.ColumnFilterPopupMaxRecordsCount;
				this.columnFilterPopupRowCount = opt.ColumnFilterPopupRowCount;
				this.defaultFilterEditorView = opt.DefaultFilterEditorView;
				this.filterEditorAggregateEditing = opt.FilterEditorAggregateEditing;
				this.filterEditorUseMenuForOperandsAndOperators = opt.FilterEditorUseMenuForOperandsAndOperators;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ColumnViewOptionsSelection : ViewBaseOptions {
		bool multiSelect;
		public ColumnViewOptionsSelection() {
			this.multiSelect = false;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsSelectionMultiSelect"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool MultiSelect {
			get { return multiSelect; }
			set {
				if(MultiSelect == value) return;
				bool prevValue = MultiSelect;
				multiSelect = value;
				OnChanged(new BaseOptionChangedEventArgs("MultiSelect", prevValue, MultiSelect));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ColumnViewOptionsSelection opt = options as ColumnViewOptionsSelection;
				if(opt == null) return;
				this.multiSelect = opt.MultiSelect;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ColumnViewOptionsView : ViewBaseOptions {
		ShowFilterPanelMode showFilterPanelMode;
		GridAnimationType animationType;
		bool showViewCaption;
		ShowButtonModeEnum showButtonMode = ShowButtonModeEnum.Default;
		internal bool AllowAssignSplitOptions = true;
		public ColumnViewOptionsView() {
			this.showFilterPanelMode = ShowFilterPanelMode.Default;
			this.showViewCaption = false;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ColumnViewOptionsView opt = options as ColumnViewOptionsView;
				if(opt == null) return;
				if(AllowAssignSplitOptions) {
					this.showViewCaption = opt.ShowViewCaption;
					this.showFilterPanelMode = opt.ShowFilterPanelMode;
				}
				this.showButtonMode = opt.ShowButtonMode;
				this.animationType = opt.AnimationType;
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsViewShowButtonMode"),
#endif
 DefaultValue(ShowButtonModeEnum.Default), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public ShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(ShowButtonMode == value) return;
				ShowButtonModeEnum prevValue = ShowButtonMode;
				showButtonMode = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowButtonMode", prevValue, ShowButtonMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsViewShowViewCaption"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowViewCaption {
			get { return showViewCaption; }
			set {
				if(ShowViewCaption == value) return;
				bool prevValue = ShowViewCaption;
				showViewCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowViewCaption", prevValue, ShowViewCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsViewShowFilterPanelMode"),
#endif
 DefaultValue(ShowFilterPanelMode.Default), XtraSerializableProperty()]
		public ShowFilterPanelMode ShowFilterPanelMode {
			get { return showFilterPanelMode; }
			set {
				if(!Enum.IsDefined(typeof(ShowFilterPanelMode), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ShowFilterPanelMode));
				}
				if(ShowFilterPanelMode == value) return;
				ShowFilterPanelMode prevValue = ShowFilterPanelMode;
				showFilterPanelMode = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFilterPanelMode", prevValue, ShowFilterPanelMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsViewAnimationType"),
#endif
 DefaultValue(GridAnimationType.Default), XtraSerializableProperty()]
		public GridAnimationType AnimationType {
			get { return animationType; }
			set {
				object prevValue = AnimationType;
				animationType = value;
				OnChanged(new BaseOptionChangedEventArgs("AnimationType", prevValue, AnimationType));
			}
		}
		public GridAnimationType GetAnimationType() {
			if(AnimationType == GridAnimationType.Default) return GridAnimationType.AnimateFocusedItem;
			return AnimationType;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete(ObsoleteText.SRColumnView_OptionsViewShowFilterPanel)]
		public virtual bool ShowFilterPanel {
			get { return ShowFilterPanelMode != ShowFilterPanelMode.Never; }
			set {
				if(ShowFilterPanel == value) return;
				ShowFilterPanelMode = value ? ShowFilterPanelMode.Default : ShowFilterPanelMode.Never;
			}
		}
	}
	public class ColumnViewOptionsBehavior : ViewBaseOptions {
		EditorShowMode editorShowMode;
		DefaultBoolean allowDeleteRows, allowAddRows;
		bool autoSelectAllInEditor, editable, readOnly,
			immediateUpdateRowPosition, keepFocusedRowOnUpdate,
			autoPopulateColumns, focusLeaveOnTab;
		CacheRowValuesMode cacheValuesOnRowUpdating;
		ColumnView view;
		public ColumnViewOptionsBehavior(ColumnView view) {
			this.editorShowMode = EditorShowMode.Default;
			this.view = view;
			this.cacheValuesOnRowUpdating = CacheRowValuesMode.CacheAll;
			this.autoPopulateColumns = true;
			this.keepFocusedRowOnUpdate = true;
			this.immediateUpdateRowPosition = true;
			this.autoSelectAllInEditor = true;
			this.editable = true;
			this.readOnly = false;
			this.focusLeaveOnTab = false;
			this.allowDeleteRows = this.allowAddRows = DefaultBoolean.Default;
		}
		public ColumnViewOptionsBehavior() : this(null) { }
		protected ColumnView View { get { return view; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRColumnView_OptionsBehaviorShowEditorOnMouseUp)]
		public virtual bool ShowEditorOnMouseUp {
			get { return EditorShowMode == DevExpress.Utils.EditorShowMode.MouseUp; }
			set {
				if(ShowEditorOnMouseUp == value) return;
				EditorShowMode = value ? DevExpress.Utils.EditorShowMode.MouseUp : DevExpress.Utils.EditorShowMode.Default;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorCacheValuesOnRowUpdating"),
#endif
 DefaultValue(CacheRowValuesMode.CacheAll), XtraSerializableProperty()]
		public virtual CacheRowValuesMode CacheValuesOnRowUpdating {
			get { return cacheValuesOnRowUpdating; }
			set {
				if(CacheValuesOnRowUpdating == value) return;
				CacheRowValuesMode prevValue = CacheValuesOnRowUpdating;
				cacheValuesOnRowUpdating = value;
				OnChanged(new BaseOptionChangedEventArgs("CacheValuesOnRowUpdating", prevValue, CacheValuesOnRowUpdating));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorFocusLeaveOnTab"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool FocusLeaveOnTab {
			get { return focusLeaveOnTab; }
			set {
				if(focusLeaveOnTab == value) return;
				bool prevFocusLeaveOnTab = FocusLeaveOnTab;
				focusLeaveOnTab = value;
				OnChanged(new BaseOptionChangedEventArgs("FocusLeaveOnTab", prevFocusLeaveOnTab, FocusLeaveOnTab));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorAllowDeleteRows"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowDeleteRows {
			get { return allowDeleteRows; }
			set {
				if(AllowDeleteRows == value) return;
				DefaultBoolean prevAllowDeleteRows = AllowDeleteRows;
				allowDeleteRows = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDeleteRows", prevAllowDeleteRows, AllowDeleteRows));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorAllowAddRows"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DefaultBoolean AllowAddRows {
			get { return allowAddRows; }
			set {
				if(AllowAddRows == value) return;
				DefaultBoolean prevAllowAddRows = AllowAddRows;
				allowAddRows = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAddRows", prevAllowAddRows, AllowAddRows));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorEditorShowMode"),
#endif
 DefaultValue(EditorShowMode.Default), XtraSerializableProperty()]
		public virtual EditorShowMode EditorShowMode {
			get { return editorShowMode; }
			set {
				if(EditorShowMode == value) return;
				EditorShowMode prevValue = EditorShowMode;
				editorShowMode = value;
				OnChanged(new BaseOptionChangedEventArgs("EditorShowMode", prevValue, EditorShowMode));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorImmediateUpdateRowPosition"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ImmediateUpdateRowPosition {
			get { return immediateUpdateRowPosition; }
			set {
				if(ImmediateUpdateRowPosition == value) return;
				bool prevValue = ImmediateUpdateRowPosition;
				immediateUpdateRowPosition = value;
				OnChanged(new BaseOptionChangedEventArgs("ImmediateUpdateRowPosition", prevValue, ImmediateUpdateRowPosition));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorKeepFocusedRowOnUpdate"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool KeepFocusedRowOnUpdate {
			get { return keepFocusedRowOnUpdate; }
			set {
				if(KeepFocusedRowOnUpdate == value) return;
				bool prevValue = KeepFocusedRowOnUpdate;
				keepFocusedRowOnUpdate = value;
				OnChanged(new BaseOptionChangedEventArgs("KeepFocusedRowOnUpdate", prevValue, KeepFocusedRowOnUpdate));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorAutoSelectAllInEditor"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoSelectAllInEditor {
			get { return autoSelectAllInEditor; }
			set {
				if(AutoSelectAllInEditor == value) return;
				bool prevValue = AutoSelectAllInEditor;
				autoSelectAllInEditor = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoSelectAllInEditor", prevValue, AutoSelectAllInEditor));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorAutoPopulateColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AutoPopulateColumns {
			get { return autoPopulateColumns; }
			set {
				if(AutoPopulateColumns == value) return;
				bool prevValue = AutoPopulateColumns;
				autoPopulateColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoPopulateColumns", prevValue, AutoPopulateColumns));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorEditable"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool Editable {
			get { return editable; }
			set {
				if(Editable == value) return;
				bool prevValue = Editable;
				editable = value;
				OnChanged(new BaseOptionChangedEventArgs("Editable", prevValue, Editable));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehaviorReadOnly"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ReadOnly {
			get { return readOnly; }
			set {
				if(ReadOnly == value) return;
				bool prevValue = ReadOnly;
				readOnly = value;
				OnChanged(new BaseOptionChangedEventArgs("ReadOnly", prevValue, ReadOnly));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRColumnView_OptionsBehaviorShowAllTableValuesInFilterPopup)]
		public virtual bool ShowAllTableValuesInFilterPopup {
			get { return view.OptionsFilter.ShowAllTableValuesInFilterPopup; }
			set { view.OptionsFilter.ShowAllTableValuesInFilterPopup = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRColumnView_OptionsBehaviorUseNewCustomFilterDialog)]
		public virtual bool UseNewCustomFilterDialog {
			get { return view.OptionsFilter.UseNewCustomFilterDialog; }
			set { view.OptionsFilter.UseNewCustomFilterDialog = value; }
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ColumnViewOptionsBehavior opt = options as ColumnViewOptionsBehavior;
				if(opt == null) return;
				this.cacheValuesOnRowUpdating = opt.CacheValuesOnRowUpdating;
				this.editorShowMode = opt.EditorShowMode;
				this.autoPopulateColumns = opt.AutoPopulateColumns;
				this.keepFocusedRowOnUpdate = opt.KeepFocusedRowOnUpdate;
				this.immediateUpdateRowPosition = opt.ImmediateUpdateRowPosition;
				this.autoSelectAllInEditor = opt.AutoSelectAllInEditor;
				this.editable = opt.Editable;
				this.allowAddRows = opt.AllowAddRows;
				this.allowDeleteRows = opt.AllowDeleteRows;
				this.readOnly = opt.ReadOnly;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
