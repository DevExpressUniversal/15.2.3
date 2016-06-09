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
using System.Linq;
using System.Windows;
using DevExpress.Data;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.XtraPivotGrid.Data;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public enum SyncState { Default, ByActions }
	public enum ActionState { Read, Write }
	public class PivotClipboardAccessor : IPivotClipboardAccessor {
		public void SetDataObject(string clipbloarObject) {
			try {
				Clipboard.SetText(clipbloarObject);
			} catch(System.Runtime.InteropServices.ExternalException) { }
		}
	}
	public class PivotGridWpfData : PivotGridDataAsync, IFilteredComponent {
		public delegate void CellMadeVisibleDelegate(PivotGridWpfData data, CellMadeVisibleEventArgs cell);
		WeakReference pivotGridReference;
		PivotGridFieldCollection fields;
		PivotGridGroupCollection groups;
		PivotGridDataField dataField;
		BestWidthCalculator bestWidthCalculator;
		BestHeightCalculator bestHeightCalculator;
		BestWidthCalculator.BestFitCacheHelper bestFitCacheHelper;
		FieldListFields fieldListFields;
		ActionsInvoker actionsInvoker;
		SyncState syncState;
		public static void DataRelatedDPChanged(PivotGridControl pivot, PropertyChangedCallback baseCallback, DependencyObject d, DependencyPropertyChangedEventArgs e) {
#if SL
			if(pivot != null && pivot.HasData && pivot.Data.IsForcedAsyncMode && !pivot.Data.IsLockUpdate)
				pivot.Data.ActionsDelayer.EnqueueAction(delegate() {
					baseCallback.Invoke(d, e);
				});
			else
#endif
			baseCallback.Invoke(d, e);
		}
		public PivotGridWpfData(PivotGridControl pivotGrid)
			: base() {
			PivotGrid = pivotGrid;
			this.fields = CreateFields();
			this.groups = CreateGroups();
			this.fieldListFields = CreateFieldListFields();
			this.dataField = CreateDataFieldWrapper();
			this.bestFitCacheHelper = new BestWidthCalculator.BestFitCacheHelper();
			this.bestWidthCalculator = CreateBestWidthCalculator(bestFitCacheHelper);
			this.bestHeightCalculator = CreateBestHeightCalculator(bestFitCacheHelper);
		}
		public PivotGridControl PivotGrid {
			get { return pivotGridReference != null ? (PivotGridControl)pivotGridReference.Target : null; }
			set {
				this.pivotGridReference = new WeakReference(value);
				base.EventsImplementor = PivotGrid;
			}
		}
		public override IPivotGridEventsImplementorBase EventsImplementor {
			get { return base.EventsImplementor; }
			set {
				base.EventsImplementor = value;
				PivotGrid = null;
			}
		}
		protected IPivotGridEventsImplementor EventsImplementorEx {
			get { return (IPivotGridEventsImplementor)EventsImplementor; }
		}
		public PivotGridPopupMenu GridMenu { get { return PivotGrid.GridMenu; } }
		public BestWidthCalculator BestWidthCalculator { get { return bestWidthCalculator; } }
		public BestWidthCalculator BestHeightCalculator { get { return bestHeightCalculator; } }
		public new PivotGridFieldCollection Fields { get { return fields; } }
		public new PivotGridGroupCollection Groups { get { return groups; } }
		public FieldListFields FieldListFields { get { return fieldListFields; } }
		public override CoreXtraPivotGrid.Customization.CustomizationFormFields GetCustomizationFormFields() {
			return FieldListFields;
		}
		public new PivotGridDataField DataField { get { return dataField; } }
		protected PivotGridInternalFieldCollection InternalFields { get { return (PivotGridInternalFieldCollection)base.Fields; } }
		protected PivotGridInternalField InternalDataField { get { return (PivotGridInternalField)base.DataField; } }
		public new PivotVisualItems VisualItems {
			get { return (PivotVisualItems)base.VisualItems; }
		}
		internal PivotGridField RowTreeField { get { return ((PivotVisualItems)base.VisualItemsInternal).RowTreeField; } }
		internal PivotWpfChartDataSource ChartDataSource { get { return (PivotWpfChartDataSource)ChartDataSourceInternal; } }
		public override bool IsLoading {
			get { return base.IsLoading || (PivotGrid != null && PivotGrid.IsLoading); }
		}
		public override bool IsDesignMode {
			get { return base.IsDesignMode || (PivotGrid != null && PivotGrid.IsDesignMode); }
		}
		public override bool IsControlReady { get { return !IsLoading; } }
		public bool IsRowTree { get { return OptionsView.RowTotalsLocation == CoreXtraPivotGrid.PivotRowTotalsLocation.Tree; } }
		public event CellMadeVisibleDelegate CellMadeVisible;
		public override bool AllowHideFields {
			get {
				if(PivotGrid == null)
					return base.AllowHideFields;
				switch(PivotGrid.AllowHideFields) {
					case AllowHideFieldsType.Never:
						return false;
					case AllowHideFieldsType.Always:
						return true;
					case AllowHideFieldsType.WhenFieldListVisible:
						return PivotGrid.IsFieldListVisible || PivotGrid.ExternalFieldListCount > 0;
				}
				throw new ArgumentException("PivotGrid.AllowHideFields");
			}
		}
		protected override IPivotClipboardAccessor CreateClipboardAccessor() {
			return new PivotClipboardAccessor();
		}
		protected virtual PivotGridFieldCollection CreateFields() {
			return new PivotGridFieldCollection(InternalFields);
		}
		protected virtual PivotGridGroupCollection CreateGroups() {
			return new PivotGridGroupCollection(this, base.Groups);
		}
		protected override CoreXtraPivotGrid.PivotGridGroupCollection CreateGroupCollection() {
			return new PivotGridInternalGroupCollection(this);
		}
		protected virtual FieldListFields CreateFieldListFields() {
			return new FieldListFields(this);
		}
		protected override CoreXtraPivotGrid.PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridInternalFieldCollection(this);
		}
		protected override CoreXtraPivotGrid.PivotGridFieldBase CreateDataField() {
			return new PivotGridInternalField(this);
		}
		protected virtual PivotGridDataField CreateDataFieldWrapper() {
			return new PivotGridDataField(this, InternalDataField);
		}
		protected override PivotVisualItemsBase CreateVisualItems() {
			return new PivotVisualItems(this);
		}
		protected virtual BestWidthCalculator CreateBestWidthCalculator(BestWidthCalculator.BestFitCacheHelper helper) {
			return new BestWidthCalculator(this, helper);
		}
		protected virtual BestHeightCalculator CreateBestHeightCalculator(BestWidthCalculator.BestFitCacheHelper helper) {
			return new BestHeightCalculator(this, helper);
		}
		protected override IPivotListDataSource CreateListDataSource() {
			return new WpfNativeDataSource(this);
		}
		protected override CoreXtraPivotGrid.PivotChartDataSourceBase CreateChartDataSource() {
			return new PivotWpfChartDataSource(this);
		}
		protected override PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new CoreXtraPivotGrid.PivotGridOptionsChartDataSource(this);
		}
		protected override PivotGridData CreateEmptyInstance() {
			return PivotGrid.CreateEmptyData();
		}
		protected override CoreXtraPivotGrid.Events.PivotGridEventRaiserBase CreateEventRaiser(IPivotGridEventsImplementorBase eventsImplementor) {
			return eventsImplementor == null ? null : new PivotGridEventRaiser(eventsImplementor);
		}
		protected override CoreXtraPivotGrid.PivotSummaryDataSource CreateSummaryDataSourceCore(int columnIndex, int rowIndex) {
			return new PivotSummaryDataSource(this, columnIndex, rowIndex);
		}
		public new PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex) {
			return base.CreateSummaryDataSource(columnIndex, rowIndex) as PivotSummaryDataSource;
		}
		public new PivotSummaryDataSource CreateSummaryDataSource() {
			return base.CreateSummaryDataSource() as PivotSummaryDataSource;
		}
		internal PivotDrillDownDataSource CreateDrillDownDataSourceWrapper(CoreXtraPivotGrid.PivotDrillDownDataSource dataSource) {
			return new PivotDrillDownDataSource(dataSource);
		}
		public new PivotDrillDownDataSource GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows) {
			return CreateDrillDownDataSourceWrapper(base.GetDrillDownDataSource(groupRow, visibleListSourceRows));
		}
		public new PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex) {
			return CreateDrillDownDataSourceWrapper(base.GetDrillDownDataSource(columnIndex, rowIndex,
				dataIndex, OptionsData.DrillDownMaxRowCount));
		}
		public new PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			return CreateDrillDownDataSourceWrapper(base.GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount));
		}
		public new PivotDrillDownDataSource GetQueryModeDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex,
			int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSourceWrapper(base.GetQueryModeDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns));
		}
		public override void RetrieveFields(CoreXtraPivotGrid.PivotArea area, bool visible) {
			BeginUpdate();
			try {
				Fields.Clear();
				base.RetrieveFields(area, visible);
			} finally {
				EndUpdate();
			}
		}
		protected override void RetrieveFieldCore(CoreXtraPivotGrid.PivotArea area, string fieldName, string caption, string displayFolder, bool visible) {
			base.RetrieveFieldCore(area, fieldName, caption, displayFolder, visible);
			PivotGridInternalField internalField = InternalFields[InternalFields.Count - 1];
			new PropertiesSynchronizer().SyncProperty(this, true, false, () => { }, () => {
				BeginUpdate();
				Fields.Add(internalField);
				CancelUpdate();
			});
		}
		public override void SetControlDataSource(System.Collections.IList ds) {
			if(PivotGrid != null)
				PivotGrid.DataSource = ds;
		}
		public List<PivotGridField> GetFieldsByArea(FieldArea area, bool includeDataField) {
			List<CoreXtraPivotGrid.PivotGridFieldBase> fields = base.GetFieldsByArea(area.ToPivotArea(), includeDataField);
			List<PivotGridField> res = new List<PivotGridField>(fields.Count);
			for(int i = 0; i < fields.Count; i++) {
				PivotGridInternalField field = (PivotGridInternalField)fields[i];
				res.Add(field.Wrapper);
			}
			return res;
		}
		public new PivotGridField GetFieldByArea(CoreXtraPivotGrid.PivotArea area, int index) {
			PivotGridInternalField field = base.GetFieldByArea(area, index) as PivotGridInternalField;
			return field.GetWrapper();
		}
		public new PivotGridField GetFieldByLevel(bool isColumn, int level) {
			PivotGridInternalField field = base.GetFieldByLevel(isColumn, level) as PivotGridInternalField;
			return field.GetWrapper();
		}
		public object GetCellValue(int columnIndex, int rowIndex, PivotGridField field) {
			return GetCellValue(columnIndex, rowIndex, field.InternalField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField field) {
			return GetCellValue(columnValues, rowValues, field.InternalField);
		}
		public override void OnFieldSizeChanged(CoreXtraPivotGrid.PivotGridFieldBase field, bool widthChanged, bool heightChanged) {
			base.OnFieldSizeChanged(field, widthChanged, heightChanged);
			PivotGridField wrapper = field.GetWrapper();
			if(wrapper == DataField)
				SyncDataField();
		}
		protected override void AfterFieldFilteringChanged(CoreXtraPivotGrid.PivotGridFieldBase field) {
			PivotGridInternalField internalField = (PivotGridInternalField)field;
			base.AfterFieldFilteringChanged(internalField);
			internalField.AfterFilteredValueChanged();
		}
		protected override void AfterGroupFilteringChanged(CoreXtraPivotGrid.PivotGridGroup group) {
			PivotGridInternalGroup internalGroup = (PivotGridInternalGroup)group;
			base.AfterGroupFilteringChanged(internalGroup);
			if(internalGroup.Count > 0)
				internalGroup.Wrapper[0].OnInternalFieldChanged(null, new FieldSyncPropertyEventArgs(FieldSyncProperty.Filtered));
		}
		public override void CellSelectionChanged() {
			base.CellSelectionChanged();
			if(EventsImplementorEx != null)
				EventsImplementorEx.CellSelectionChanged();
		}
		public override void FocusedCellChanged(System.Drawing.Point oldValue, System.Drawing.Point newValue) {
			base.FocusedCellChanged(oldValue, newValue);
			if(EventsImplementorEx != null)
				EventsImplementorEx.FocusedCellChanged();
		}
		public new PivotGridField GetField(PivotFieldItemBase item) {
			return base.GetField(item).GetWrapper();
		}
		public PivotFieldItem GetFeldItem(PivotGridField field) {
			return (PivotFieldItem)base.GetFieldItem(field.GetInternalField());
		}
		public PivotFieldItem GetFeldItem(PivotGridInternalField field) {
			return (PivotFieldItem)base.GetFieldItem(field);
		}
		public PivotGridInternalField GetInternalField(PivotFieldItem item) {
			return (PivotGridInternalField)base.GetField(item);
		}
		public PivotGridInternalField GetInternalField(PivotGridField field) {
			return field.InternalField;
		}
		public override void Invalidate() {
			base.Invalidate();
			SyncDataField();
		}
		public ActionsInvoker ActionsInvoker {
			get {
				if(this.actionsInvoker == null)
					this.actionsInvoker = new ActionsInvoker();
				return this.actionsInvoker;
			}
		}
#if SL
		ActionsDelayer actionsDelayer;
		public ActionsDelayer ActionsDelayer {
			get {
				if(actionsDelayer == null)
					actionsDelayer = new ActionsDelayer(PivotGrid);
				return actionsDelayer;
			}
		}
#endif
		public SyncState SyncState {
			get { return syncState; }
			private set { syncState = value; }
		}
		#region async
		protected override void ShowLoadingPanelInternal() {
			base.ShowLoadingPanelInternal();
			SetLoadingPanelVisibility(true);
		}
		protected override void HideLoadingPanelInternal() {
			base.HideLoadingPanelInternal();
			SetLoadingPanelVisibility(false);
		}
		void SetLoadingPanelVisibility(bool visible) {
			switch(LoadingPanelType) {
				case PivotLoadingPanelType.MainLoadingPanel:
					PivotGrid.IsMainWaitIndicatorVisible = visible;
					break;
				case PivotLoadingPanelType.FilterPopupLoadingPanel:
					PivotGrid.IsFilterPopupWaitIndicatorVisible = visible;
					break;
				default:
					throw new Exception("Incorrect loading panel type");
			}
		}
		protected override void AsyncProcessStarting() {
			base.AsyncProcessStarting();
			ApplyControlAsyncProcess(false);
			if(EventsImplementorEx != null)
				EventsImplementorEx.AsyncOperationStarting();
		}
		protected override void AsyncProcessFinishing() {
			base.AsyncProcessFinishing();
			ApplyControlAsyncProcess(true);
			if(EventsImplementorEx != null)
				EventsImplementorEx.AsyncOperationCompleted();
		}
		protected override void DoBeforeAsyncProcessStarted() {
			base.DoBeforeAsyncProcessStarted();
			GetAggregations(true);
			SyncState = SyncState.ByActions;
		}
		protected override void DoBeforeAsyncProcessCompleted() {
			SyncState = SyncState.Default;
			ActionsInvoker.ExecuteActions(ActionState.Read, true);
			base.DoBeforeAsyncProcessCompleted();
		}
		void ApplyControlAsyncProcess(bool finished) {
			if(PivotGrid == null)
				return;
			PivotGrid.IsEnabled = finished;
			PivotGrid.InitIsDesignMode(finished);
		}
		protected override void InvokeInMainThread(AsyncCompletedInternal internalCompleted) {
#if !SL
			PivotGrid.Dispatcher.Invoke(internalCompleted);
#else
			PivotGrid.Dispatcher.BeginInvoke(internalCompleted);
#endif
		}
		protected override bool IsInMainThread {
			get {
				if(PivotGrid == null)
					return true;
				return PivotGrid.Dispatcher.CheckAccess();
			}
		}
		public bool IsForcedAsyncMode { get { return IsSL && IsOLAP; } }
		public bool IsSL {
			get {
#if SL
				return true;
#else
				return false;
#endif
			}
		}
		void InvokeInMainThreadBase(AsyncCompletedInternal internalCompleted) {
			base.InvokeInMainThread(internalCompleted);
		}
		public void SetFieldSortingAsync(PivotGridField field, FieldSortOrder sortOrder, bool forceAsync) {
			SetFieldSortingAsync(field, sortOrder, forceAsync, AsyncModeHelper.DoEmptyComplete);
		}
		public void SetFieldSortingAsync(PivotGridField field, FieldSortOrder sortOrder, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			base.SetFieldSortingAsync(field.InternalField, sortOrder.ToPivotSortOrder(), DevExpress.XtraPivotGrid.PivotSortMode.DisplayText, null, true, forceAsync, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ClearFieldSortingAsync(PivotGridField field, bool forceAsync) {
			ClearFieldSortingAsync(field, forceAsync, AsyncModeHelper.DoEmptyComplete);
		}
		public void ClearFieldSortingAsync(PivotGridField field, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			base.ClearFieldSortingAsync(field.InternalField, forceAsync, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ChangeFieldSortOrderAsync(PivotGridField field, bool forceAsync) {
			ChangeFieldSortOrderAsync(field, forceAsync, DoEmptyComplete);
		}
		public void ChangeFieldSortOrderAsync(PivotGridField field, bool forceAsync, CoreXtraPivotGrid.AsyncCompletedHandler asyncCompleted) {
			base.ChangeFieldSortOrderAsync(field.InternalField, forceAsync, DoEmptyComplete);
		}
		#endregion
		public void SetFieldAreaPosition(PivotGridField field, FieldArea newArea, int newAreaIndex) {
			base.SetFieldAreaPosition(field.InternalField, newArea.ToPivotArea(), newAreaIndex);
		}
		public int GetFieldCountByArea(FieldArea area) {
			return base.GetFieldCountByArea(area.ToPivotArea());
		}
		public override void MakeCellVisible(System.Drawing.Point cell) {
			base.MakeCellVisible(cell);
			if(CellMadeVisible != null)
				CellMadeVisible(this, new CellMadeVisibleEventArgs(cell));
		}
		public void SyncDataField() {
			if(base.DataField != null && base.DataField.Visible)
				DataField.SyncFieldAll(true, false);
			if(PivotGrid != null) {
				PivotGrid.DataFieldAreaIndex = OptionsDataField.AreaIndex;
				PivotGrid.DataFieldArea = OptionsDataField.Area.ToDataFieldAreaType();
			}
		}
		#region IFilteredComponent Members
		IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			List<FilterColumn> res = new List<FilterColumn>();
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].ShowInPrefilter && (OptionsDataField.EnableFilteringByData || Fields[i].Area != FieldArea.DataArea))
					res.Add(Fields[i].CreateFilterColumn());
			return res;
		}
		#endregion
		public override void SaveFieldsToStreamCore(System.IO.Stream stream) {
			PivotGrid.SaveLayoutToStream(stream);
		}
		public override void LoadFieldsFromStreamCore(System.IO.MemoryStream layoutStream) {
			PivotGrid.RestoreLayoutFromStream(layoutStream);
		}
		public void StartRecordingInFieldSync() {
			if(EventRaiser == null)
				return;
			((PivotGridEventRaiser)EventRaiser).StartRecordingInFieldSync();
		}
		public void FinishRecordingInFieldSync() {
			if(EventRaiser == null)
				return;
			((PivotGridEventRaiser)EventRaiser).FinishRecordingInFieldSync();
		}
		internal object GetAggregation(string fieldName, PivotGridCellItem item, Core.ConditionalFormatting.Native.ConditionalFormatSummaryType summaryType) {
			bool topBottomRule = false;
			SummaryItemTypeEx ex;
			switch(summaryType) {
				case Core.ConditionalFormatting.Native.ConditionalFormatSummaryType.Average:
					ex = SummaryItemTypeEx.Average;
					break;
				case Core.ConditionalFormatting.Native.ConditionalFormatSummaryType.Max:
					ex = SummaryItemTypeEx.Max;
					break;
				case Core.ConditionalFormatting.Native.ConditionalFormatSummaryType.Min:
					ex = SummaryItemTypeEx.Min;
					break;
				case Core.ConditionalFormatting.Native.ConditionalFormatSummaryType.SortedList:
					ex = SummaryItemTypeEx.Top;
					topBottomRule = true;
					break;
				default:
					throw new NotImplementedException(summaryType.ToString());
			}
			DevExpress.XtraPivotGrid.PivotGridFieldBase field = GetFieldByNameOrDataControllerColumnName(fieldName);
			if(field.Area != CoreXtraPivotGrid.PivotArea.DataArea || !field.Visible)
				return null;
			return GetAggregationCore(item, topBottomRule, ex, field, calcs0) ?? GetAggregationCore(item, topBottomRule, ex, field, calcs1);
		}
		static object GetAggregationCore(PivotGridCellItem item, bool topBottomRule, SummaryItemTypeEx ex, DevExpress.XtraPivotGrid.PivotGridFieldBase field, List<AggregationLevel> aggs) {
			int rowLevel = item.RowField != null ? item.RowField.AreaIndex : 0;
			int columnLevel = item.ColumnField != null ? item.ColumnField.AreaIndex : 0;
			AggregationLevel level = aggs.Find((l) => l.Row == rowLevel && l.Column == columnLevel);
			if(level == null)
				return null;
			AggregationCalculation dataLevel = level.Find((dl) => dl.Index == field.AreaIndex);
			if(dataLevel == null)
				return null;
			if(topBottomRule)
				return dataLevel;
			AggregationItemValueStorage agg = (AggregationItemValueStorage)dataLevel.Find((f) => f.SummaryType == ex);
			return agg == null ? null : agg.Result;
		}
		List<AggregationLevel> calcs0;
		List<AggregationLevel> calcs1;
		public override IList<AggregationLevel> GetAggregations(bool datasourceLevel) {
			if(!IsInMainThread) {
				return datasourceLevel ? calcs0 : calcs1;
			}
			List<AggregationLevel> result = new List<AggregationLevel>();
			if(PivotGrid != null && PivotGrid.FormatConditions != null) {
				foreach(FormatConditionBase cond in PivotGrid.FormatConditions) {
					List<AggregationItemValueStorage> sums = new List<AggregationItemValueStorage>(cond.GetSummaries());
					if(sums.Count == 0)
						continue;
					DevExpress.XtraPivotGrid.PivotGridFieldBase field = GetFieldByNameOrDataControllerColumnName(cond.MeasureName);
					if(field != null && field.Area == CoreXtraPivotGrid.PivotArea.DataArea && field.Visible) {
						if(datasourceLevel != (field.SummaryDisplayType == DevExpress.Data.PivotGrid.PivotSummaryDisplayType.Default))
							continue;
						DevExpress.XtraPivotGrid.PivotGridFieldBase rowField = GetFieldByNameOrDataControllerColumnName(cond.RowName);
						DevExpress.XtraPivotGrid.PivotGridFieldBase columnField = GetFieldByNameOrDataControllerColumnName(cond.ColumnName);
						if(rowField != null && (!rowField.Visible || rowField.Area != CoreXtraPivotGrid.PivotArea.RowArea) ||
						   columnField != null && (!columnField.Visible || columnField.Area != CoreXtraPivotGrid.PivotArea.ColumnArea))
							continue;
						int rowLevel = rowField == null ? 0 : rowField.AreaIndex;
						int columnLevel = columnField == null ? 0 : columnField.AreaIndex;
						AggregationLevel level = result.Find((l) => l.Row == rowLevel && l.Column == columnLevel);
						if(level == null) {
							level = new AggregationLevel(Enumerable.Empty<AggregationCalculation>(), rowLevel, columnLevel);
							result.Add(level);
						}
						AggregationCalculation dataLevel = level.Find((dl) => dl.Index == field.AreaIndex);
						if(dataLevel == null) {
							dataLevel = new AggregationCalculation(Enumerable.Empty<AggregationItemValue>(), field.AreaIndex);
							level.Add(dataLevel);
						}
						foreach(AggregationItemValueStorage sum in sums)
							dataLevel.Add(sum);
					}
				}
			}
			if(datasourceLevel)
				calcs0 = result;
			else
				calcs1 = result;
			return result;
		}
	}
	public class ActionsInvoker {
		List<Action> readActions;
		List<Action> writeActions;
		public ActionsInvoker() { }
		public void AddAction(ActionState state, Action action) {
			GetActions(state).Add(action);
		}
		public void ExecuteActions(ActionState state, bool clear) {
			if(GetActionsInternal(state) == null)
				return;
			foreach(Action action in GetActions(state)) {
				action.Invoke();
			}
			if(clear)
				ClearActions(state);
		}
		void ClearActions(ActionState state) {
			if(GetActionsInternal(state) == null)
				return;
			GetActions(state).Clear();
		}
		List<Action> GetActions(ActionState state) {
			return state == ActionState.Read ? ReadActions : WriteActions;
		}
		List<Action> GetActionsInternal(ActionState state) {
			return state == ActionState.Read ? this.readActions : this.writeActions;
		}
		List<Action> ReadActions {
			get {
				if(this.readActions == null)
					this.readActions = new List<Action>();
				return this.readActions;
			}
		}
		List<Action> WriteActions {
			get {
				if(this.writeActions == null)
					this.writeActions = new List<Action>();
				return this.writeActions;
			}
		}
	}
	public class PropertiesSynchronizer {
		bool isSynchronizing;
		public PropertiesSynchronizer() {
			this.isSynchronizing = false;
		}
		public bool IsSynchronizing {
			get { return isSynchronizing; }
			set { isSynchronizing = value; }
		}
		public void SyncProperty(PivotGridWpfData data, bool read, bool write, Action writeProperties, Action readProperties) {
			if(isSynchronizing)
				return;
			isSynchronizing = true;
			try {
				InvokeAction(data, writeProperties, write, ActionState.Write);
				InvokeAction(data, readProperties, read, ActionState.Read);
			} finally {
				isSynchronizing = false;
			}
		}
		void InvokeAction(PivotGridWpfData data, Action action, bool invokeRequired, ActionState state) {
			if(invokeRequired && action != null) {
				if(data != null && data.SyncState == SyncState.ByActions) {
					data.ActionsInvoker.AddAction(state, action);
				} else {
					action();
				}
			}
		}
	}
	public class CellMadeVisibleEventArgs : EventArgs {
		System.Drawing.Point cell;
		public CellMadeVisibleEventArgs(System.Drawing.Point cell) {
			this.cell = cell;
		}
		public System.Drawing.Point Cell { get { return cell; } }
	}
	public class PivotGridDataField : PivotGridField {
		PivotGridWpfData data;
		public PivotGridDataField(PivotGridWpfData data, PivotGridInternalField internalField)
			: base(internalField, true) {
			this.data = data;
		}
		protected internal override PivotGridWpfData Data { get { return data != null ? data : base.Data; } }
	}
	public interface IPivotGridEventsImplementor : IPivotGridEventsImplementorBase {
		void CellSelectionChanged();
		void FocusedCellChanged();
		void AsyncOperationStarting();
		void AsyncOperationCompleted();
	}
}
