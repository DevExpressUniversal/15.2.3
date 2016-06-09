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
using System.Windows;
using System.Windows.Input;
using DevExpress.Data.PivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Data;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid {
	public partial class PivotGridControl {
		[Category(Categories.Events)]
		public event RoutedEventHandler GridLayout {
			add { AddHandler(GridLayoutEvent, value); }
			remove { RemoveHandler(GridLayoutEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler BeginRefresh {
			add { AddHandler(BeginRefreshEvent, value); }
			remove { RemoveHandler(BeginRefreshEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler EndRefresh {
			add { AddHandler(EndRefreshEvent, value); }
			remove { RemoveHandler(EndRefreshEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler DataSourceChanged {
			add { AddHandler(DataSourceChangedEvent, value); }
			remove { RemoveHandler(DataSourceChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler ShownFieldList {
			add { AddHandler(ShownFieldListEvent, value); }
			remove { RemoveHandler(ShownFieldListEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler HiddenFieldList {
			add { AddHandler(HiddenFieldListEvent, value); }
			remove { RemoveHandler(HiddenFieldListEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler OlapDataLoaded {
			add { AddHandler(OlapDataLoadedEvent, value); }
			remove { RemoveHandler(OlapDataLoadedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotLayoutAllowEventHandler BeforeLoadLayout {
			add { AddHandler(BeforeLoadLayoutEvent, value); }
			remove { RemoveHandler(BeforeLoadLayoutEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotLayoutUpgradeEventHandler LayoutUpgrade {
			add { AddHandler(LayoutUpgradeEvent, value); }
			remove { RemoveHandler(LayoutUpgradeEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler OlapQueryTimeout {
			add { AddHandler(OlapQueryTimeoutEvent, value); }
			remove { RemoveHandler(OlapQueryTimeoutEvent, value); }
		}
		[Category(Categories.Events), Obsolete("The OlapException event is obsolete now. Use the QueryException event instead."), Browsable(false)]
		public event PivotOlapExceptionEventHandler OlapException {
			add { AddHandler(OlapExceptionEvent, value); }
			remove { RemoveHandler(OlapExceptionEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotQueryExceptionEventHandler QueryException {
			add { AddHandler(QueryExceptionEvent, value); }
			remove { RemoveHandler(QueryExceptionEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler PrefilterCriteriaChanged {
			add { AddHandler(PrefilterCriteriaChangedEvent, value); }
			remove { RemoveHandler(PrefilterCriteriaChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldValueEventHandler FieldValueCollapsed {
			add { AddHandler(FieldValueCollapsedEvent, value); }
			remove { RemoveHandler(FieldValueCollapsedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldValueEventHandler FieldValueExpanded {
			add { AddHandler(FieldValueExpandedEvent, value); }
			remove { RemoveHandler(FieldValueExpandedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldValueEventHandler FieldValueNotExpanded {
			add { AddHandler(FieldValueNotExpandedEvent, value); }
			remove { RemoveHandler(FieldValueNotExpandedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldValueCancelEventHandler FieldValueCollapsing {
			add { AddHandler(FieldValueCollapsingEvent, value); }
			remove { RemoveHandler(FieldValueCollapsingEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldValueCancelEventHandler FieldValueExpanding {
			add { AddHandler(FieldValueExpandingEvent, value); }
			remove { RemoveHandler(FieldValueExpandingEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCustomSummaryEventHandler CustomSummary {
			add { customSummary += value; }
			remove { customSummary -= value; }
		}
		[Category(Categories.Events)]
		public event PivotCellDisplayTextEventHandler CustomCellDisplayText {
			add { AddHandler(CustomCellDisplayTextEvent, value); }
			remove { RemoveHandler(CustomCellDisplayTextEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCellValueEventHandler CustomCellValue {
			add { AddHandler(CustomCellValueEvent, value); }
			remove { RemoveHandler(CustomCellValueEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCustomCellAppearanceEventHandler CustomCellAppearance {
			add { AddHandler(CustomCellAppearanceEvent, value); }
			remove { RemoveHandler(CustomCellAppearanceEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCustomValueAppearanceEventHandler CustomValueAppearance {
			add { AddHandler(CustomValueAppearanceEvent, value); }
			remove { RemoveHandler(CustomValueAppearanceEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCustomGroupIntervalEventHandler CustomGroupInterval {
			add { customGroupInterval += value; }
			remove { customGroupInterval -= value; }
		}
		[Category(Categories.Events)]
		public event CustomPrefilterDisplayTextEventHandler CustomPrefilterDisplayText {
			add { AddHandler(CustomPrefilterDisplayTextEvent, value); }
			remove { RemoveHandler(CustomPrefilterDisplayTextEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotGroupEventHandler GroupFilterChanged {
			add { AddHandler(GroupFilterChangedEvent, value); }
			remove { RemoveHandler(GroupFilterChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldAreaChangingEventHandler FieldAreaChanging {
			add { AddHandler(FieldAreaChangingEvent, value); }
			remove { RemoveHandler(FieldAreaChangingEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldAreaChanged {
			add { AddHandler(FieldAreaChangedEvent, value); }
			remove { RemoveHandler(FieldAreaChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldExpandedInGroupChanged {
			add { AddHandler(FieldExpandedInGroupChangedEvent, value); }
			remove { RemoveHandler(FieldExpandedInGroupChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldFilterChangingEventHandler FieldFilterChanging {
			add { AddHandler(FieldFilterChangingEvent, value); }
			remove { RemoveHandler(FieldFilterChangingEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldFilterChanged {
			add { AddHandler(FieldFilterChangedEvent, value); }
			remove { RemoveHandler(FieldFilterChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldUnboundExpressionChanged {
			add { AddHandler(FieldUnboundExpressionChangedEvent, value); }
			remove { RemoveHandler(FieldUnboundExpressionChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldVisibleChanged {
			add { AddHandler(FieldVisibleChangedEvent, value); }
			remove { RemoveHandler(FieldVisibleChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldAreaIndexChanged {
			add { AddHandler(FieldAreaIndexChangedEvent, value); }
			remove { RemoveHandler(FieldAreaIndexChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldPropertyChangedEventHandler FieldPropertyChanged {
			add { AddHandler(FieldPropertyChangedEvent, value); }
			remove { RemoveHandler(FieldPropertyChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFieldDisplayTextEventHandler FieldValueDisplayText {
			add { fieldValueDisplayText += value; }
			remove { fieldValueDisplayText -= value; }
		}
		[Category(Categories.Events)]
		public event PivotFieldEventHandler FieldSizeChanged {
			add { AddHandler(FieldSizeChangedEvent, value); }
			remove { RemoveHandler(FieldSizeChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCellEventHandler CellClick {
			add { AddHandler(CellClickEvent, value); }
			remove { RemoveHandler(CellClickEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCellEventHandler CellDoubleClick {
			add { AddHandler(CellDoubleClickEvent, value); }
			remove { RemoveHandler(CellDoubleClickEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler CellSelectionChanged {
			add { AddHandler(CellSelectionChangedEvent, value); }
			remove { RemoveHandler(CellSelectionChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler FocusedCellChanged {
			add { AddHandler(FocusedCellChangedEvent, value); }
			remove { RemoveHandler(FocusedCellChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCustomFieldSortEventHandler CustomFieldSort {
			add { customFieldSort += value; }
			remove { customFieldSort -= value; }
		}
		[Category(Categories.Events)]
		public event EventHandler<CustomServerModeSortEventArgs> CustomServerModeSort {
			add { customServerModeSort += value; }
			remove { customServerModeSort -= value; }
		}
		[Category(Categories.Events)]
		public event PivotCustomFieldDataEventHandler CustomUnboundFieldData {
			add { customUnboundFieldData += value; }
			remove { customUnboundFieldData -= value; }
		}
		[Category(Categories.Events)]
		public event PivotCustomChartDataSourceDataEventHandler CustomChartDataSourceData {
			add { customChartDataSourceData += value; }
			remove { customChartDataSourceData -= value; }
		}
		[Category(Categories.Events)]
		public event PivotCustomChartDataSourceRowsEventHandler CustomChartDataSourceRows {
			add { customChartDataSourceRows += value; }
			remove { customChartDataSourceRows -= value; }
		}
		[Category(Categories.Events)]
		public event PivotCustomFilterPopupItemsEventHandler CustomFilterPopupItems {
			add { AddHandler(CustomFilterPopupItemsEvent, value); }
			remove { RemoveHandler(CustomFilterPopupItemsEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotCustomFieldValueCellsEventHandler CustomFieldValueCells {
			add { AddHandler(CustomFieldValueCellsEvent, value); }
			remove { RemoveHandler(CustomFieldValueCellsEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotUnboundExpressionEditorEventHandler UnboundExpressionEditorCreated {
			add { AddHandler(UnboundExpressionEditorCreatedEvent, value); }
			remove { RemoveHandler(UnboundExpressionEditorCreatedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFilterEditorEventHandler PrefilterEditorCreated {
			add { AddHandler(PrefilterEditorCreatedEvent, value); }
			remove { RemoveHandler(PrefilterEditorCreatedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotFilterEditorEventHandler PrefilterEditorHiding {
			add { AddHandler(PrefilterEditorHidingEvent, value); }
			remove { RemoveHandler(PrefilterEditorHidingEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotPropertyChangedEventHandler PropertyChanged {
			add { AddHandler(PropertyChangedEvent, value); }
			remove { RemoveHandler(PropertyChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PivotBrushChangedEventHandler BrushChanged {
			add { AddHandler(BrushChangedEvent, value); }
			remove { RemoveHandler(BrushChangedEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler AsyncOperationStarting {
			add { AddHandler(AsyncOperationStartingEvent, value); }
			remove { RemoveHandler(AsyncOperationStartingEvent, value); }
		}
		[Category(Categories.Events)]
		public event RoutedEventHandler AsyncOperationCompleted {
			add { AddHandler(AsyncOperationCompletedEvent, value); }
			remove { RemoveHandler(AsyncOperationCompletedEvent, value); }
		}
		[Category(Categories.Events)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { AddHandler(PopupMenuShowingEvent, value); }
			remove { RemoveHandler(PopupMenuShowingEvent, value); }
		}
		#region internal events
		internal event RoutedEventHandler ShowHeadersPropertyChanged {
			add { AddHandler(ShowHeadersPropertyChangedEvent, value); }
			remove { RemoveHandler(ShowHeadersPropertyChangedEvent, value); }
		}
		#endregion
		#region IPivotGridEventsImplementor Members
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(item.IsCollapsed)
				RaiseFieldValueExpanded(item);
			else
				RaiseFieldValueCollapsed(item);
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, CoreXtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldValueNotExpanded(item, field.GetWrapper());
		}
		bool IPivotGridEventsImplementorBase.BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(item.IsCollapsed)
				return RaiseFieldValueExpanding(item);
			else
				return RaiseFieldValueCollapsing(item);
		}
		void IPivotGridEventsImplementorBase.BeginRefresh() {
			RaiseBeginRefresh();
		}
		void IPivotGridEventsImplementorBase.CalcCustomSummary(DevExpress.XtraPivotGrid.PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {
			RaiseCustomSummary(field, customSummaryInfo);
		}
		string IPivotGridEventsImplementorBase.CustomCellDisplayText(PivotGridCellItem cellItem) {
			return RaiseCellDisplayText(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomCellValue(PivotGridCellItem cellItem) {
			return RaiseCellValue(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomGroupInterval(DevExpress.XtraPivotGrid.PivotGridFieldBase field, object value) {
			return RaiseCustomGroupInterval(field.GetWrapper(), value);
		}
		void IPivotGridEventsImplementorBase.DataSourceChanged() {
			RaiseDataSourceChanged();
		}
		void IPivotGridEventsImplementorBase.EndRefresh() {
			RaiseEndRefresh();
		}
		void IPivotGridEventsImplementorBase.GroupFilterChanged(DevExpress.XtraPivotGrid.PivotGridGroup group) {
			RaiseGroupFilterChanged(group.GetWrapper());
		}
		void IPivotGridEventsImplementorBase.FieldAreaChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldAreaChanged(field.GetWrapper());
		}
		bool IPivotGridEventsImplementorBase.FieldAreaChanging(DevExpress.XtraPivotGrid.PivotGridFieldBase field, DevExpress.XtraPivotGrid.PivotArea newArea, int newAreaIndex) {
			return RaiseFieldAreaChanging(field.GetWrapper(), newArea.ToFieldArea(), newAreaIndex);
		}
		void IPivotGridEventsImplementorBase.FieldExpandedInFieldsGroupChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldExpandedInFieldsGroupChanged(field.GetWrapper());
		}
		bool IPivotGridEventsImplementorBase.FieldFilterChanging(DevExpress.XtraPivotGrid.PivotGridFieldBase field, CoreXtraPivotGrid.PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return RaiseFieldFilterChanging(field.GetWrapper(), filterType.ToFieldFilterType(), showBlanks, values);
		}
		void IPivotGridEventsImplementorBase.FieldFilterChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldFilterChanged(field.GetWrapper());
		}
		void IPivotGridEventsImplementorBase.FieldUnboundExpressionChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldUnboundExpressionChanged(field.GetWrapper());
		}
		void IPivotGridEventsImplementorBase.FieldVisibleChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldVisibleChanged(field.GetWrapper());
		}
		void IPivotGridEventsImplementorBase.FieldAreaIndexChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldAreaIndexChanged(field.GetWrapper());
		}
		void IPivotGridEventsImplementorBase.FieldPropertyChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field, CoreXtraPivotGrid.PivotFieldPropertyName propertyName) {
			RaiseFieldPropertyChanged(field.GetWrapper(), propertyName.ToFieldPropertyName());
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			return RaiseFieldValueDisplayText(item, defaultText);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(DevExpress.XtraPivotGrid.PivotGridFieldBase field, object value) {
			return RaiseFieldValueDisplayText(field.GetWrapper(), value);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(DevExpress.XtraPivotGrid.PivotGridFieldBase field, DevExpress.XtraPivotGrid.IOLAPMember member) {
			return RaiseFieldValueDisplayText(field.GetWrapper(), member);
		}
		void IPivotGridEventsImplementorBase.FieldWidthChanged(DevExpress.XtraPivotGrid.PivotGridFieldBase field) {
			RaiseFieldSizeChanged(field.GetWrapper());
		}
		int? IPivotGridEventsImplementorBase.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2,
				DevExpress.XtraPivotGrid.PivotGridFieldBase field, DevExpress.XtraPivotGrid.PivotSortOrder sortOrder) {
			return RaiseCustomFieldSort(listSourceRow1, listSourceRow2, value1, value2,
				field.GetWrapper(), sortOrder.ToFieldSortOrder());
		}
		int? IPivotGridEventsImplementorBase.QuerySorting(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, DevExpress.XtraPivotGrid.PivotGridFieldBase field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			return RaiseCustomQuerySort(value0, value1, field.GetWrapper(), helper);
		}
		object IPivotGridEventsImplementorBase.GetUnboundValue(DevExpress.XtraPivotGrid.PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return RaiseCustomUnboundFieldData(field.GetWrapper(), listSourceRowIndex, expValue);
		}
		void IPivotGridEventsImplementorBase.LayoutChanged() {
			RaiseGridLayout();
			PeerCache = null;
		}
		void IPivotGridEventsImplementorBase.OLAPQueryTimeout() {
			RaiseOlapQueryTimeout();
		}
		bool IPivotGridEventsImplementorBase.QueryException(System.Exception ex) {
			return RaiseQueryException(ex);
		}
		void IPivotGridEventsImplementorBase.PrefilterCriteriaChanged() {
			RaisePrefilterCriteriaChanged();
		}
		object IPivotGridEventsImplementorBase.CustomChartDataSourceData(CoreXtraPivotGrid.PivotChartItemType itemType, CoreXtraPivotGrid.PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			return RaiseCustomChartDataSourceData(itemType.ToXpfPivotChartItemType(), itemDataMember.ToXpfPivotChartItemDataMember(), fieldValueItem, cellItem, value);
		}
		void IPivotGridEventsImplementorBase.CustomChartDataSourceRows(IList<CoreXtraPivotGrid.PivotChartDataSourceRowBase> rows) {
			RaiseCustomChartDataSourceRows(Data.ChartDataSource, rows);
		}
		bool IPivotGridEventsImplementorBase.CustomFilterPopupItems(PivotGridFilterItems items) {
			return RaiseCustomFilterPopupItems(items);
		}
		bool IPivotGridEventsImplementorBase.CustomFieldValueCells(PivotVisualItemsBase items) {
			return RaiseCustomFieldValueCells(items);
		}
		void IPivotGridEventsImplementor.CellSelectionChanged() {
			Selection = VisualItems.Selection;
			RaiseCellSelectionChanged();
		}
		void IPivotGridEventsImplementor.FocusedCellChanged() {
			FocusedCell = VisualItems.FocusedCell;
			RaiseFocusedCellChanged();
		}
		void IPivotGridEventsImplementor.AsyncOperationStarting() {
			RaiseAsyncOperationStarting();
		}
		void IPivotGridEventsImplementor.AsyncOperationCompleted() {
			RaiseAsyncOperationCompleted();
		}
		#endregion
		#region RaiseXXX methods
		protected virtual void RaiseGridLayout() {
			RaiseEvent(new RoutedEventArgs(GridLayoutEvent));
		}
		protected virtual void RaiseFieldValueCollapsed(PivotFieldValueItem item) {
			RaiseEvent(new PivotFieldValueCollapseEventArgs(FieldValueCollapsedEvent, item));
		}
		protected virtual void RaiseFieldValueExpanded(PivotFieldValueItem item) {
			RaiseEvent(new PivotFieldValueExpandEventArgs(FieldValueExpandedEvent, item));
		}
		protected virtual void RaiseFieldValueNotExpanded(PivotFieldValueItem item, PivotGridField field) {
			if(item != null)
				RaiseEvent(new PivotFieldValueEventArgs(FieldValueNotExpandedEvent, item));
			else
				RaiseEvent(new PivotFieldValueEventArgs(FieldValueNotExpandedEvent, field));
		}
		protected virtual bool RaiseFieldValueCollapsing(PivotFieldValueItem item) {
			PivotFieldValueCancelEventArgs e = new PivotFieldValueCancelEventArgs(FieldValueCollapsingEvent, item);
			RaiseEvent(e);
			return !e.Cancel;
		}
		protected virtual bool RaiseFieldValueExpanding(PivotFieldValueItem item) {
			PivotFieldValueCancelEventArgs e = new PivotFieldValueCancelEventArgs(FieldValueExpandingEvent, item);
			RaiseEvent(e);
			return !e.Cancel;
		}
		protected virtual void RaiseBeginRefresh() {
			RaiseEvent(new RoutedEventArgs(BeginRefreshEvent));
		}
		protected virtual void RaiseEndRefresh() {
			RaiseEvent(new RoutedEventArgs(EndRefreshEvent));
		}
		protected virtual void RaiseDataSourceChanged() {
			RaiseEvent(new RoutedEventArgs(DataSourceChangedEvent));
		}
		protected virtual void RaiseShownFieldList() {
			RaiseEvent(new RoutedEventArgs(ShownFieldListEvent));
		}
		protected virtual void RaiseHiddenFieldList() {
			RaiseEvent(new RoutedEventArgs(HiddenFieldListEvent));
		}
		protected virtual void RaiseOlapDataLoaded() {
			RaiseEvent(new RoutedEventArgs(OlapDataLoadedEvent));
		}
		protected internal virtual void RaiseLayoutUpgrade(DevExpress.Utils.LayoutUpgradeEventArgs e) {
			RaiseEvent(new PivotLayoutUpgradeEventArgs(LayoutUpgradeEvent, e));
		}
		protected internal virtual void RaiseBeforeLoadLayout(PivotLayoutAllowEventArgs e) {
			RaiseEvent(e);
		}
		protected virtual void RaiseOlapQueryTimeout() {
			RaiseEvent(new RoutedEventArgs(OlapQueryTimeoutEvent));
		}
		protected virtual bool RaiseQueryException(System.Exception ex) {
			PivotOlapExceptionEventArgs e = new PivotOlapExceptionEventArgs(OlapExceptionEvent, this, ex);
			RaiseEvent(e);
			PivotQueryExceptionEventArgs e2 = new PivotQueryExceptionEventArgs(QueryExceptionEvent, this, ex);
			RaiseEvent(e2);
			return e.Handled || e2.Handled;
		}
		protected virtual void RaisePrefilterCriteriaChanged() {
			RaiseEvent(new RoutedEventArgs(PrefilterCriteriaChangedEvent));
			if(!object.Equals(null, PrefilterCriteria))
				UpdatePrefilterPanel();
		}
		protected virtual void RaiseCustomSummary(CoreXtraPivotGrid.PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			if(customSummary != null) {
				PivotCustomSummaryEventArgs e = new PivotCustomSummaryEventArgs(Data, field.GetWrapper(), customSummaryInfo);
				customSummary(this, e);
			}
		}
		protected virtual string RaiseCellDisplayText(PivotGridCellItem cellItem) {
			PivotCellDisplayTextEventArgs e = new PivotCellDisplayTextEventArgs(CustomCellDisplayTextEvent, cellItem);
			RaiseEvent(e);
			return e.DisplayText;
		}
		protected virtual object RaiseCellValue(PivotGridCellItem cellItem) {
			PivotCellValueEventArgs e = new PivotCellValueEventArgs(CustomCellValueEvent, cellItem);
			RaiseEvent(e);
			return e.Value;
		}
		protected internal virtual PivotCustomCellAppearanceEventArgs RaiseCustomCellAppearance(PivotGridCellItem cellItem, bool isExporting) {
			PivotCustomCellAppearanceEventArgs e = new PivotCustomCellAppearanceEventArgs(CustomCellAppearanceEvent, cellItem, isExporting);
			RaiseEvent(e);
			return e;
		}
		protected internal virtual PivotCustomValueAppearanceEventArgs RaiseCustomValueAppearance(PivotFieldValueItem valueItem, bool isExporting) {
			PivotCustomValueAppearanceEventArgs e = new PivotCustomValueAppearanceEventArgs(CustomValueAppearanceEvent, valueItem, isExporting);
			RaiseEvent(e);
			return e;
		}
		protected virtual object RaiseCustomGroupInterval(PivotGridField field, object value) {
			if(customGroupInterval != null) {
				PivotCustomGroupIntervalEventArgs e = new PivotCustomGroupIntervalEventArgs(field, value);
				customGroupInterval(this, e);
				return e.GroupValue;
			} else
				return value;
		}
		protected virtual bool RaiseFieldAreaChanging(PivotGridField field, FieldArea newArea, int newAreaIndex) {
			PivotFieldAreaChangingEventArgs e = new PivotFieldAreaChangingEventArgs(FieldAreaChangingEvent, field,
				newArea, newAreaIndex);
			RaiseEvent(e);
			return e.Allow;
		}
		protected virtual void RaiseGroupFilterChanged(PivotGridGroup group) {
			RaiseEvent(new PivotGroupEventArgs(GroupFilterChangedEvent, group));
		}
		protected virtual void RaiseFieldAreaChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldAreaChangedEvent, field));
		}
		protected virtual void RaiseFieldExpandedInFieldsGroupChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldExpandedInGroupChangedEvent, field));
		}
		protected virtual void RaiseFieldFilterChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldFilterChangedEvent, field));
		}
		protected virtual bool RaiseFieldFilterChanging(PivotGridField field, FieldFilterType filterType, bool showBlanks, IList<object> values) {
			PivotFieldFilterChangingEventArgs e = new PivotFieldFilterChangingEventArgs(FieldFilterChangingEvent, field, filterType, values);
			RaiseEvent(e);
			return e.Cancel;
		}
		protected virtual void RaiseFieldAreaIndexChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldAreaIndexChangedEvent, field));
		}
		protected virtual void RaiseFieldVisibleChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldVisibleChangedEvent, field));
		}
		protected virtual void RaiseFieldUnboundExpressionChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldUnboundExpressionChangedEvent, field));
		}
		protected virtual void RaiseFieldPropertyChanged(PivotGridField field, FieldPropertyName propertyName) {
			RaiseEvent(new PivotFieldPropertyChangedEventArgs(FieldPropertyChangedEvent, field, propertyName));
		}
		protected virtual void RaiseFieldSizeChanged(PivotGridField field) {
			RaiseEvent(new PivotFieldEventArgs(FieldSizeChangedEvent, field));
		}
		protected virtual string RaiseFieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			if(fieldValueDisplayText != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(null, item, defaultText);
				fieldValueDisplayText(this, e);
				return e.DisplayText;
			} else
				return defaultText;
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridField field, object value) {
			if(fieldValueDisplayText != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(null, field, value);
				fieldValueDisplayText(this, e);
				return e.DisplayText;
			} else
				return field.GetValueText(value);
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridField field, DevExpress.XtraPivotGrid.IOLAPMember member) {
			if(fieldValueDisplayText != null) {
				PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(null, field, member);
				fieldValueDisplayText(this, e);
				return e.DisplayText;
			} else
				return field.GetValueText(member);
		}
		protected virtual void RaiseCustomPrefilterDisplayText(CustomPrefilterDisplayTextEventArgs e) {
			if(Data.Prefilter.State == CoreXtraPivotGrid.PrefilterState.Invalid) {
				e.Value = CoreXtraPivotGrid.PrefilterBase.InvalidCriteriaDisplayText;
			}
			RaiseEvent(e);
		}
		PivotCustomFieldSortEventArgs fieldSortEventArgs = null;
		protected virtual int? RaiseCustomFieldSort(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridField field,
				FieldSortOrder sortOrder) {
			if(customFieldSort != null) {
				if(fieldSortEventArgs != null && (fieldSortEventArgs.Field != field || fieldSortEventArgs.Data != Data)) {
					fieldSortEventArgs = null;
				}
				if(fieldSortEventArgs == null) {
					fieldSortEventArgs = new PivotCustomFieldSortEventArgs(Data, field);
				}
				fieldSortEventArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
				customFieldSort(this, fieldSortEventArgs);
				return fieldSortEventArgs.GetSortResult();
			} else
				return null;
		}
		CustomServerModeSortEventArgs fieldQuerySortEventArgs = null;
		protected virtual int? RaiseCustomQuerySort(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridField field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			if(customServerModeSort != null) {
				if(fieldQuerySortEventArgs == null)
					fieldQuerySortEventArgs = new CustomServerModeSortEventArgs();
				fieldQuerySortEventArgs.SetArgs(value0, value1, field, helper);
				customServerModeSort(this, fieldQuerySortEventArgs);
				return fieldQuerySortEventArgs.Result;
			} else
				return null;
		}
		protected virtual object RaiseCustomUnboundFieldData(PivotGridField field, int listSourceRowIndex, object expValue) {
			if(customUnboundFieldData != null) {
				PivotCustomFieldDataEventArgs e = new PivotCustomFieldDataEventArgs(Data, field, listSourceRowIndex, expValue);
				customUnboundFieldData(this, e);
				return e.Value;
			} else
				return expValue;
		}
		protected virtual object RaiseCustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			if(customChartDataSourceData != null) {
				PivotCustomChartDataSourceDataEventArgs e = new PivotCustomChartDataSourceDataEventArgs(itemType, itemDataMember, fieldValueItem, cellItem, value);
				customChartDataSourceData(this, e);
				return e.Value;
			} else
				return value;
		}
		void RaiseCustomChartDataSourceRows(PivotWpfChartDataSource ds, IList<CoreXtraPivotGrid.PivotChartDataSourceRowBase> rows) {
			if(customChartDataSourceRows != null) {
				PivotCustomChartDataSourceRowsEventArgs e = new PivotCustomChartDataSourceRowsEventArgs(ds, rows);
				customChartDataSourceRows(this, e);
			}
		}
		protected virtual bool RaiseCustomFilterPopupItems(PivotGridFilterItems items) {
			PivotCustomFilterPopupItemsEventArgs e = new PivotCustomFilterPopupItemsEventArgs(CustomFilterPopupItemsEvent, items);
			RaiseEvent(e);
			return true;
		}
		protected virtual bool RaiseCustomFieldValueCells(PivotVisualItemsBase items) {
			PivotCustomFieldValueCellsEventArgs e = new PivotCustomFieldValueCellsEventArgs(CustomFieldValueCellsEvent, items);
			RaiseEvent(e);
			return e.IsUpdateRequired;
		}
		protected internal virtual bool RaiseCellClick(UIElement cellElement, PivotGridCellItem cellItem, MouseButton button) {
			PivotCellEventArgs e = new PivotCellEventArgs(CellClickEvent, cellElement, cellItem, button);
			RaiseEvent(e);
			return e.Handled;
		}
		protected internal virtual bool RaiseCellDblClick(UIElement cellElement, PivotGridCellItem cellItem, MouseButton button) {
			PivotCellEventArgs e = new PivotCellEventArgs(CellDoubleClickEvent, cellElement, cellItem, button);
			RaiseEvent(e);
			return e.Handled;
		}
		protected virtual void RaiseCellSelectionChanged() {
			RoutedEventArgs e = new RoutedEventArgs(CellSelectionChangedEvent);
			RaiseEvent(e);
		}
		protected virtual void RaiseFocusedCellChanged() {
			RoutedEventArgs e = new RoutedEventArgs(FocusedCellChangedEvent);
			RaiseEvent(e);
		}
		protected virtual void RaiseShowHeadersPropertyChanged() {
			RaiseEvent(new RoutedEventArgs(ShowHeadersPropertyChangedEvent, this));
		}
		protected internal virtual void RaisePropertyChanged(DependencyPropertyChangedEventArgs args, object source) {
			RaiseEvent(new PivotPropertyChangedEventArgs(PropertyChangedEvent, args, source));
		}
		protected internal virtual void RaisePivotBrushesChanged(PivotBrushType type) {
			RaiseEvent(new PivotBrushChangedEventArgs(BrushChangedEvent, this, type));
		}
		protected virtual void RaiseAsyncOperationStarting() {
			RaiseEvent(new RoutedEventArgs(AsyncOperationStartingEvent));
		}
		protected virtual void RaiseAsyncOperationCompleted() {
			RaiseEvent(new RoutedEventArgs(AsyncOperationCompletedEvent));
		}
		protected internal virtual bool RaiseShowMenu() {
			PopupMenuShowingEventArgs e = new PopupMenuShowingEventArgs(GridMenu) { RoutedEvent = PivotGridControl.PopupMenuShowingEvent };
			RaiseEvent(e);
			return !e.Handled;
		}
		#endregion
	}
}
