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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.PivotGrid.ServerMode.Queryable;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.ExpressionEditor.Native;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.PivotGrid.Printing;
using DevExpress.Xpf.PivotGrid.Serialization;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using PivotGridCommands_ = DevExpress.Xpf.PivotGrid.Internal.PivotGridCommandsWrapper;
namespace DevExpress.Xpf.PivotGrid {
	[
	DXToolboxBrowsable,
	TemplatePart(Name = TemplatePartScroller, Type = typeof(PivotGridScroller)),
	TemplatePart(Name = TemplatePartThemeLoader, Type = typeof(DXPivotGridThemesLoader)),
	]
	[DevExpress.Utils.Design.DataAccess.OLAPDataAccessMetadata("All", SupportedProcessingModes = "Pivot", OLAPConnectionStringProperty = "OlapConnectionString", OLAPDataProviderProperty = "OlapDataProvider", DataSourceProperty = "DataSource")]	
	[ComplexBindingProperties("DataSource")]
	public partial class PivotGridControl : Control, IPivotGridEventsImplementor, ILogicalOwner, IWeakEventListener, ISupportInitialize, IFormatConditionCollectionOwner {
		internal static string ExcelFieldListTemplatePropertyName = "ExcelFieldListTemplate";
		internal static string FieldListTemplatePropertyName = "FieldListTemplate";
		internal static string PrefilterCriteriaPropertyName = "PrefilterCriteria";
		internal static double RowTotalsHeightFactorPropertyDefaultValue = 7.0 / 5.0;
		const string SerializerAppName = "PivotGrid",
			TemplatePartScroller = "PART_Scroller",
			TemplatePartThemeLoader = "PART_ThemesLoader";
		const int DefaultRowTreeOffset = 35;
#if !SL
		public static bool AllowInfiniteGridSize = false;
#endif
		Locker loadingLocker;
		Locker applyFieldListStateLocker;
		IColumnChooser actualFieldList;
		PivotGridPopupMenu gridMenu;
		BestFitter bestFitter;
		PivotPrintHelper printHelper;
		Lazy<BarManagerMenuController> headersAreaMenuController;
		Lazy<BarManagerMenuController> headerMenuController;
		Lazy<BarManagerMenuController> fieldValueMenuController;
		Lazy<BarManagerMenuController> fieldListMenuController;
		Lazy<BarManagerMenuController> filterPopupMenuController;
		Lazy<BarManagerMenuController> cellMenuController;
		PivotSerializationController serializationController;
		PivotCustomSummaryEventHandler customSummary;
		PivotCustomGroupIntervalEventHandler customGroupInterval;
		PivotFieldDisplayTextEventHandler fieldValueDisplayText;
		PivotCustomFieldSortEventHandler customFieldSort;
		EventHandler<CustomServerModeSortEventArgs> customServerModeSort;
		PivotCustomFieldDataEventHandler customUnboundFieldData;
		PivotCustomChartDataSourceDataEventHandler customChartDataSourceData;
		PivotCustomChartDataSourceRowsEventHandler customChartDataSourceRows;
		ObservableCollection<CriteriaOperatorInfo> mruFiltersInternal;
		ReadOnlyObservableCollection<CriteriaOperatorInfo> mruFilters;
		WeakReference dataReference;
		bool? isDesignMode;
		FormatConditionCollection formatConditions;
		public PivotGridControl() {
			this.SetValue(PivotGridProperty, this);
			this.loadingLocker = new Locker();
			this.applyFieldListStateLocker = new Locker();
			loadingLocker.Lock();
			this.CoerceValue(PivotGridControl.RowTreeWidthProperty);
			this.CoerceValue(PivotGridControl.RowTreeHeightProperty);
			this.CoerceValue(PivotGridControl.DataFieldHeightProperty);
			this.CoerceValue(PivotGridControl.DataFieldWidthProperty);
			Data = CreateData();
			loadingLocker.Unlock();
			UpdatePrefilterPanel();
			SetDefaultOptionsView();
			this.headersAreaMenuController = CreateMenuController();
			this.headerMenuController = CreateMenuController();
			this.fieldValueMenuController = CreateMenuController();
			this.serializationController = CreateSerializationController();
			this.fieldListMenuController = CreateMenuController();
			this.filterPopupMenuController = CreateMenuController();
			this.cellMenuController = CreateMenuController();
#if !SL            			
			IsVisibleChanged += OnIsVisibleChanged;
#endif
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			this.CoerceValue(GroupFieldsInFieldListProperty); 
			this.SetDefaultStyleKey(typeof(PivotGridControl));
			SetValue(DXSerializer.SerializationProviderProperty, new PivotSerializationProvider() { PivotGrid = this });
			SetValue(DXSerializer.SerializationIDDefaultProperty, PivotSerializationController.DefaultID);
#if SL
			doubleClickImplementer = new DoubleClickImplementer(this);
			Type ignore = typeof(LayoutControl.LayoutControl);
			if(ignore.Name.Contains("C")) {
			}
			Data.OptionsBehavior.UseAsyncMode = UseAsyncMode;
#endif
			UpdateShowColumnsBorder();
			DragDropManager = new PivotDragDropManager();
#if DEBUGTEST && SL
			DevExpress.Xpf.PivotGrid.Tests.MemoryLeaksHelper.Add(this);
#endif
			SetFieldHeight();
			SetFieldWidth();
			formatConditions = new FormatConditionCollection(this);
		}
		internal bool HasData {
			get { return dataReference != null; }
		}
		PivotGridCommands_ commands;
#if !SL
		internal
#else
		public
#endif
		PivotGridCommands_ Commands {
			get {
				if(commands == null)
					commands = new PivotGridCommands_(this);
				return commands;
			}
		}
		protected internal PivotGridWpfData Data {
			get { return (PivotGridWpfData)dataReference.Target; }
			private set {
				dataReference = new WeakReference(value);
				this.SetValue(DataPropertyKey, value);
			}
		}
		protected internal PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		protected internal PivotSerializationController SerializationController { get { return serializationController; } }
		protected internal BarManagerMenuController HeadersAreaMenuController { 
			get { return headersAreaMenuController.Value; } 
		}
		protected internal BarManagerMenuController HeaderMenuController {
			get { return headerMenuController.Value; }
		}
		protected internal BarManagerMenuController FieldValueMenuController {
			get { return fieldValueMenuController.Value; }
		}
		protected internal BarManagerMenuController FieldListMenuController {
			get { return fieldListMenuController.Value; }
		}
		protected internal BarManagerMenuController FilterPopupMenuController {
			get { return filterPopupMenuController.Value; }
		}
		protected internal BarManagerMenuController CellMenuController {
			get { return cellMenuController.Value; }
		}
		protected internal IColumnChooser ActualFieldList {
			get {
				if(actualFieldList == null)
					ActualFieldList = CreateFieldList();
				return actualFieldList;
			}
			private set {
				if(actualFieldList == value)
					return;
				if(actualFieldList != null) actualFieldList.Destroy();
				actualFieldList = value;
			}
		}
		protected internal int ExternalFieldListCount { get; set; }
		PivotGridScroller pivotGridScroller;
		protected internal PivotGridScroller PivotGridScroller {
			get {
				if(pivotGridScroller == null)
					pivotGridScroller = GetPivotGridScroller();
				return pivotGridScroller;
			}
		}
		protected BestFitter BestFitter {
			get {
				if(bestFitter == null)
					bestFitter = CreateBestFitter();
				return bestFitter;
			}
		}
		protected virtual PivotGridWpfData CreateData() {
			return new PivotGridWpfData(this);
		}
		protected internal PivotGridWpfData CreateEmptyData() {
			return CreateData();
		}
		protected virtual BestFitter CreateBestFitter() {
			return new BestFitter(Data);
		}
		protected virtual IColumnChooser CreateFieldList() {
			IColumnChooser result = FieldListFactory.Create(this);
			NullColumnChooserException.CheckColumnChooserNotNull(result);
			result.ApplyState(FieldListStyle == FieldListStyle.Excel2007 ? ExcelFieldListState : FieldListState);
			return result;
		}
		protected virtual Lazy<BarManagerMenuController> CreateMenuController() {
			return new Lazy<BarManagerMenuController>(() => GridMenuInfoBase.CreateMenuController(GridMenu));
		}
		protected virtual PivotSerializationController CreateSerializationController() {
			return new PivotSerializationController(this);
		}
		public virtual void ShowFieldList() {
			IsFieldListVisible = true;
		}
		public virtual void HideFieldList() {
			IsFieldListVisible = false;
		}
		public virtual void ShowUnboundExpressionEditor(PivotGridField field) {
			if(ExpressionEditorContainer != null) HideUnboundExpressionEditor();
			if(!Data.IsCapabilitySupported(PivotDataSourceCaps.UnboundColumns))
				return;
			ExpressionEditorControl expressionEditorControl = new ExpressionEditorControl(field.InternalField);
			DialogClosedDelegate closedHandler = delegate(bool? dialogResult) {
				if(dialogResult == true) {
					Data.BeginUpdate();
					field.UnboundExpression = expressionEditorControl.Expression;
					Data.CancelUpdate();
					if(!IsDesignMode)
						Data.ReloadDataAsync(false);
				}
				ExpressionEditorContainer = null;
				if(DesignTimeAdorner != null)
					DesignTimeAdorner.PerformChangeUnboundExpression(field);
				UserAction = UserAction.None;
			};
			RoutedEventArgs e = new PivotUnboundExpressionEditorEventArgs(UnboundExpressionEditorCreatedEvent,
				expressionEditorControl, field);
			RaiseEvent(e);
			if(e.Handled)
				return;
			UserAction = UserAction.FieldUnboundExpression;
			ExpressionEditorContainer = ExpressionEditorHelper.ShowExpressionEditor(expressionEditorControl, this, closedHandler);
		}
		public virtual void HideUnboundExpressionEditor() {
			if(ExpressionEditorContainer == null) return;
			if(ExpressionEditorContainer != null && IsExpressionEditorContainerOpen) {
				IsExpressionEditorContainerOpen = false;
				ExpressionEditorContainer = null;
			}
		}
		public virtual void ShowPrefilter() {
			if(!Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter))
				return;
			IsPrefilterVisible = true;
		}
		public virtual void HidePrefilter() {
			if(!Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter))
				return;
			IsPrefilterVisible = false;
		}
		public virtual void ResetPrefilter() {
			PrefilterCriteria = null;
			IsPrefilterEnabled = true;
		}
		CoreXtraPivotGrid.PrefilterBase Prefilter {
			get {
				return Data.Prefilter;
			}
		}
		public PivotGridField GetFieldByArea(FieldArea area, int index) {
			return Data.GetFieldByArea(area.ToPivotArea(), index);
		}
		public List<PivotGridField> GetFieldsByArea(FieldArea area) {
			return Data.GetFieldsByArea(area, false);
		}
		public int GetFieldCountByArea(FieldArea area) {
			return Data.GetFieldCountByArea(area.ToPivotArea());
		}
		[
		Description("")
		]
		public void CopySelectionToClipboard() { 
			VisualItems.CopySelectionToClipboard(); 
		}
		public void SetSelectionByFieldValues(bool isColumn, object[] values) {
			VisualItems.SetSelectionByFieldValues(isColumn, values);
		}
		public void SetSelectionByFieldValues(bool isColumn, object[] values, PivotGridField dataField) {
			VisualItems.SetSelectionByFieldValues(isColumn, values, Data.GetFeldItem(dataField));
		}
		public override void BeginInit() {
			base.BeginInit();
			loadingLocker.Lock();
		}
		public override void EndInit() {
			Fields.OnInitialized();
			loadingLocker.Unlock();
			base.EndInit();
			Data.BeginUpdate();
			SetFieldWidth();
			SetFieldHeight();
			Data.CancelUpdate();
			OnDataSourceChanged(null, DataSource);
			if(OlapConnectionString != null)
				OnOlapSourceChanged(null, OlapConnectionString);
			else
				Data.DoRefresh();
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool IsLoading { get { return loadingLocker.IsLocked; } }
		public void BeginUpdate() {
			Data.BeginUpdate();
		}
		public void EndUpdate() {
			Data.EndUpdate();
		}
		public bool IsUpdateLocked { get { return Data.IsLockUpdate; } }
		public void CollapseAll() {
			Data.ChangeExpandedAll(false);
		}
		public void CollapseAllRows() {
			Data.ChangeExpandedAll(false, false);
		}
		public void CollapseAllColumns() {
			Data.ChangeExpandedAll(true, false);
		}
		public void ExpandAll() {
			Data.ChangeExpandedAll(true);
		}
		public void ExpandAllRows() {
			Data.ChangeExpandedAll(false, true);
		}
		public void ExpandAllColumns() {
			Data.ChangeExpandedAll(true, true);
		}
		public void CollapseValue(bool isColumn, params object[] values) {
			Data.ChangeExpanded(isColumn, values, false);
		}
		public void ExpandValue(bool isColumn, params object[] values) {
			Data.ChangeExpanded(isColumn, values, true);
		}
		public int GetColumnIndex(params object[] values) {
			return Data.GetFieldValueIndex(true, values);
		}
		public int GetRowIndex(params object[] values) {
			return Data.GetFieldValueIndex(false, values);
		}
		public int GetColumnIndex(PivotGridField field, params object[] values) {
			return Data.GetFieldValueIndex(true, values, field);
		}
		public int GetRowIndex(PivotGridField field, params object[] values) {
			return Data.GetFieldValueIndex(false, values, field);
		}
		public bool IsFieldValueCollapsed(PivotGridField field, int lastLevelIndex) {
			return VisualItems.IsObjectCollapsed(field, lastLevelIndex);
		}
		public bool IsFieldValueCollapsed(bool isColumn, params object[] values) {
			return Data.IsObjectCollapsed(isColumn, values);
		}
		public object GetFieldValue(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetFieldValue(field, lastLevelIndex);
		}
		public object GetFieldValue(bool isColumn, int lastLevelIndex, int level) {
			return VisualItems.GetFieldValue(isColumn, lastLevelIndex, level);
		}
		public FieldValueType GetFieldValueType(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetFieldValueType(field, lastLevelIndex);
		}
		public FieldValueType GetFieldValueType(bool isColumn, int lastLevelIndex) {
			return VisualItems.GetLastLevelItem(isColumn, lastLevelIndex).ValueType.ToFieldValueType();
		}
		public PivotOlapMember GetFieldValueOlapMember(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetOlapMember(field, lastLevelIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues) {
			if(GetFieldCountByArea(FieldArea.DataArea) != 1)
				throw new Exception("This method can be used if there is just one field in the data area only.");
			PivotGridField dataField = GetFieldByArea(FieldArea.DataArea, 0);
			return GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public PivotCellBaseEventArgs GetCellInfo(int columnIndex, int rowIndex) {
			PivotGridCellItem cellItem = VisualItems.CreateCellItem(columnIndex, rowIndex);
			return new PivotCellBaseEventArgs(null, cellItem);
		}
		public PivotCellBaseEventArgs GetFocusedCellInfo() {
			return GetCellInfo(FocusedCell.X, FocusedCell.Y);
		}
		public List<string> GetOlapKpiList() {
			return Data.GetOLAPKPIList();
		}
		public PivotOlapKpiMeasures GetOlapKpiMeasures(string kpiName) {
			return PivotOlapKpiMeasures.Create(Data.GetOLAPKPIMeasures(kpiName));
		}
		public PivotOlapKpiValue GetOlapKpiValue(string kpiName) {
			return PivotOlapKpiValue.Create(Data.GetOLAPKPIValue(kpiName));
		}
		public PivotKpiGraphic GetOlapKpiServerGraphic(string kpiName, PivotKpiType kpiType) {
			return Data.GetOLAPKPIServerGraphic(kpiName, kpiType.ToXtraType()).ToKpiGraphic();
		}
		public BitmapImage GetKpiBitmap(PivotKpiGraphic graphic, int state) {
			return DevExpress.Xpf.PivotGrid.Internal.ImageHelper.GetImage(graphic, state);
		}
		public void ReloadData() {
			Data.ReloadData();
		}
		public void RefreshData() {
			Data.DoRefresh();
		}
		public void LayoutChanged() {
			Data.LayoutChanged();
		}
		public void RetrieveFields() {
			Data.RetrieveFields();
		}
		public string[] GetFieldList() {
			return Data.GetFieldList();
		}
		public void RetrieveFields(FieldArea area, bool visible) {
			Data.RetrieveFields(area.ToPivotArea(), visible);
		}
		#region async
		public virtual bool IsAsyncInProgress { get { return Data.IsLocked; } }
		public void EndUpdateAsync() {
			EndUpdateAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void EndUpdateAsync(AsyncCompletedHandler asyncCompleted) {
			Data.EndUpdateAsync(true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void SetDataSourceAsync(object dataSource) {
			SetDataSourceAsync(dataSource, AsyncModeHelper.DoEmptyComplete);
		}
		public void SetDataSourceAsync(object dataSource, AsyncCompletedHandler asyncCompleted) {
			BeginUpdate();
			DataSource = dataSource;
			EndUpdateAsync(asyncCompleted);
		}
		public void SetOlapConnectionStringAsync(string olapConnectionString) {
			SetOlapConnectionStringAsync(olapConnectionString, AsyncModeHelper.DoEmptyComplete);
		}
		public void SetOlapConnectionStringAsync(string olapConnectionString, AsyncCompletedHandler asyncCompleted) {
			BeginUpdate();
			try {
				OlapConnectionString = olapConnectionString;
			} catch {
				Data.CancelUpdate();
				throw;
			}
			EndUpdateAsync(asyncCompleted);
		}
		public void CollapseAllAsync() {
			CollapseAllAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void CollapseAllAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(false, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void CollapseAllRowsAsync() {
			CollapseAllRowsAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void CollapseAllRowsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(false, false, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void CollapseAllColumnsAsync() {
			CollapseAllColumnsAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void CollapseAllColumnsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(true, false, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ExpandAllAsync() {
			ExpandAllAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void ExpandAllAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(true, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ExpandAllRowsAsync() {
			ExpandAllRowsAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void ExpandAllRowsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(false, true, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ExpandAllColumnsAsync() {
			ExpandAllColumnsAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void ExpandAllColumnsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(true, true, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void CollapseValueAsync(bool isColumn, object[] values) {
			CollapseValueAsync(isColumn, values, AsyncModeHelper.DoEmptyComplete);
		}
		public void CollapseValueAsync(bool isColumn, object[] values, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAsync(isColumn, values, false, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ExpandValueAsync(bool isColumn, object[] values) {
			ExpandValueAsync(isColumn, values, AsyncModeHelper.DoEmptyComplete);
		}
		public void ExpandValueAsync(bool isColumn, object[] values, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAsync(isColumn, values, true, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ChangeFieldExpandedAsync(PivotGridField field, bool expand) {
			ChangeFieldExpandedAsync(field, expand, AsyncModeHelper.DoEmptyComplete);
		}
		public void ChangeFieldExpandedAsync(PivotGridField field, bool expand, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeFieldExpandedAsync(field.InternalField, expand, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void SetFieldSortingAsync(PivotGridField field, FieldSortOrder sortOrder) {
			SetFieldSortingAsync(field, sortOrder, AsyncModeHelper.DoEmptyComplete);
		}
		public void SetFieldSortingAsync(PivotGridField field, FieldSortOrder sortOrder, AsyncCompletedHandler asyncCompleted) {
			Data.SetFieldSortingAsync(field, sortOrder, true, asyncCompleted);
		}
		public void ClearFieldSortingAsync(PivotGridField field) {
			Data.ClearFieldSortingAsync(field, true, AsyncModeHelper.DoEmptyComplete);
		}
		public void ClearFieldSortingAsync(PivotGridField field, AsyncCompletedHandler asyncCompleted) {
			Data.ClearFieldSortingAsync(field, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void ChangeFieldSortOrderAsync(PivotGridField field) {
			ChangeFieldSortOrderAsync(field, AsyncModeHelper.DoEmptyComplete);
		}
		public void ChangeFieldSortOrderAsync(PivotGridField field, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeFieldSortOrderAsync(field, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void CreateDrillDownDataSourceAsync(AsyncCompletedHandler asyncCompleted) {
			Data.GetDrillDownDataSourceAsync(-1, -1, 0, true, delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
				asyncCompleted.Invoke(new AsyncDrillDownResult(Data, result));
			});
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, AsyncCompletedHandler asyncCompleted) {
			Data.CreateDrillDownDataSourceAsync(columnIndex, rowIndex, true, delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
				asyncCompleted.Invoke(new AsyncDrillDownResult(Data, result));
			});
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, AsyncCompletedHandler asyncCompleted) {
			Data.CreateDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, true, delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
				asyncCompleted.Invoke(new AsyncDrillDownResult(Data, result));
			});
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, -1, customColumns, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			Data.CreateQueryModeDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, true, delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
				asyncCompleted.Invoke(new AsyncDrillDownResult(Data, result));
			});
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateOlapDrillDownDataSourceAsync(int columnIndex, int rowIndex, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, customColumns, asyncCompleted);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateOlapDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, asyncCompleted);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateServerModeDrillDownDataSourceAsync(int columnIndex, int rowIndex, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, -1, customColumns, asyncCompleted);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateServerModeDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, asyncCompleted);
		}
		public void ReloadDataAsync() {
			ReloadDataAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void ReloadDataAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ReloadDataAsync(true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void RetrieveFieldsAsync() {
			RetrieveFieldsAsync(AsyncModeHelper.DoEmptyComplete);
		}
		public void RetrieveFieldsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.RetrieveFieldsAsync(true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void RetrieveFieldsAsync(FieldArea area, bool visible) {
			RetrieveFieldsAsync(area, visible, AsyncModeHelper.DoEmptyComplete);
		}
		public void RetrieveFieldsAsync(FieldArea area, bool visible, AsyncCompletedHandler asyncCompleted) {
			Data.RetrieveFieldsAsync(area.ToPivotArea(), visible, true, asyncCompleted.ToCoreAsyncCompleted());
		}
		public void RestoreLayoutFromStreamAsync(Stream stream, AsyncCompletedHandler asyncCompleted) {
			SerializationController.RestoreLayoutAsync(stream, asyncCompleted);
		}
		public void RestoreLayoutFromXmlAsync(string fileName, AsyncCompletedHandler asyncCompleted) {
			SerializationController.RestoreLayoutAsync(fileName, asyncCompleted);
		}
		public void RestoreCollapsedStateFromStreamAsync(Stream stream) {
			RestoreCollapsedStateFromStreamAsync(stream, AsyncModeHelper.DoEmptyComplete);
		}
		public void RestoreCollapsedStateFromStreamAsync(Stream stream, AsyncCompletedHandler asyncCompleted) {
			Data.LoadCollapsedStateFromStreamAsync(stream, true, delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
				LayoutChanged();
				asyncCompleted.ToCoreAsyncCompleted().Invoke(result);
			});
		}
		#endregion
		public void ScrollToPixels(Point point) {
			ScrollToPixelsCore(point.X, point.Y);
		}
		public void ScrollToPixels(System.Drawing.Point point) {
			ScrollToPixelsCore(point.X, point.Y);
		}
		void ScrollToPixelsCore(double x, double y) {
			if(PivotGridScroller == null || ScrollingMode == ScrollingMode.Line)
				return;
			PivotGridScroller.ScrollToHorizontalOffset(x);
			PivotGridScroller.ScrollToVerticalOffset(y);
		}
		public void ScrollTo(System.Drawing.Point cell) {
			if(PivotGridScroller == null)
				return;
			PivotGridScroller.ScrollToHorizontalOffset(VisualItems.CellSizeProvider.GetWidthDifference(true, 0, cell.X));
			PivotGridScroller.ScrollToVerticalOffset(VisualItems.CellSizeProvider.GetHeightDifference(false, 0, cell.Y));
		}
		public void MakeCellVisible(System.Drawing.Point cell) {
			if(PivotGridScroller == null) return;
			PivotGridScroller.MakeCellVisible(cell);
		}
		#region best fit        
		public void BestFitColumn(int columnIndex) {
			if(!CanBestFit)
				return;
			BestFitter.BestFitColumn(columnIndex);
		}
		public void BestFitRow(int rowIndex) {
			if(!CanBestFit)
				return;
			BestFitter.BestFitRow(rowIndex);
		}
		public void BestFit() {
			BestFit(true, true);
		}
		public void BestFit(bool fitWidth, bool fitHeight) {
			if(!CanBestFit)
				return;
			BestFitter.BestFit(RowTotalsLocation == FieldRowTotalsLocation.Tree, fitWidth, fitHeight);
		}
		public void BestFit(FieldArea area) {
			BestFit(area, true, true);
		}
		public void BestFit(FieldArea area, bool fitWidth, bool fitHeight) {
			if(!CanBestFit)
				return;
			BestFitter.BestFit(area, RowTotalsLocation == FieldRowTotalsLocation.Tree, fitWidth, fitHeight);
		}
		public void BestFit(PivotGridField field) {
			BestFit(field, true, true);
		}
		public void BestFit(PivotGridField field, bool fitWidth, bool fitHeight) {
			if(!CanBestFit)
				return;
			BestFitter.BestFit(field, fitWidth, fitHeight);
		}
		internal void BestFit(bool isColumn, int index) {
			if(isColumn)
				BestFitColumn(index);
			else
				BestFitRow(index);
		}
		bool CanBestFit {
			get { return Data.BestHeightCalculator.CellsDecorator != null && !IsAsyncInProgress && !Data.IsRefreshing && UIElementHelper.IsVisibleInTree(Data.BestHeightCalculator.CellsDecorator); }
		}
		#endregion
		void ReadOptionsView() {
			ShowColumnGrandTotals = Data.OptionsView.ShowColumnGrandTotals;
			ShowColumnHeaders = Data.OptionsView.ShowColumnHeaders;
			ShowColumnTotals = Data.OptionsView.ShowColumnTotals;
			ShowCustomTotalsForSingleValues = Data.OptionsView.ShowCustomTotalsForSingleValues;
			ShowDataHeaders = Data.OptionsView.ShowDataHeaders;
			ShowFilterHeaders = Data.OptionsView.ShowFilterHeaders;
			ShowGrandTotalsForSingleValues = Data.OptionsView.ShowGrandTotalsForSingleValues;
			ShowRowGrandTotals = Data.OptionsView.ShowRowGrandTotals;
			ShowRowHeaders = Data.OptionsView.ShowRowHeaders;
			ShowRowTotals = Data.OptionsView.ShowRowTotals;
			ShowTotalsForSingleValues = Data.OptionsView.ShowTotalsForSingleValues;
		}
		public bool GetShowHeaders(FieldArea area) {
			return Data.OptionsView.GetShowHeaders(area.ToPivotArea());
		}
		public void HideAllTotals() {
			Data.OptionsView.HideAllTotals();
			ReadOptionsView();
		}
		public void ShowAllTotals() {
			Data.OptionsView.ShowAllTotals();
			ReadOptionsView();
		}
		void SetDefaultOptionsView() {
			Data.OptionsView.ShowAllTotals();
			Data.OptionsView.ShowTotalsForSingleValues = true;
			Data.OptionsView.RowTotalsLocation = CoreXtraPivotGrid.PivotRowTotalsLocation.Tree;
			Data.OptionsView.RowTreeOffset = DefaultRowTreeOffset;
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource();
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(-1, -1, 0);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return VisualItems.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOlapDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOlapDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			 return CreateDrillDownDataSource(columnIndex, rowIndex, customColumns);
		}
		protected virtual void OnDataSourceChanged(object oldValue, object newValue) {
			if(IsLoading) return;
			if(newValue != null)
				OlapConnectionString = null;
			UnsubscribeDataSourceEvents(oldValue);
			IPivotGridDataSource pivotDataSource = newValue as IPivotGridDataSource;
			if(pivotDataSource != null) {
				Data.ListSource = null;
				Data.PivotDataSource = pivotDataSource;
			} else if(IsQueryableDataSource(newValue)) {
				Data.ListSource = null;
				SetQueryablePivotDataSource(GetQueryable(newValue));
				IListSource listSource = newValue as IListSource;
				if(listSource != null) {
					IBindingList queryableDataSourceBindingList = (IBindingList)listSource.GetList();
					queryableDataSourceBindingList.ListChanged += OnQueryableChanged;
				}
			} else {
#if SL && DEBUGTEST
				if(newValue is DevExpress.Data.Test.DataTable)
					newValue = ((DevExpress.Data.Test.DataTable)newValue).DefaultView;
#endif
				Data.ListSource = DataBindingHelper.ExtractDataSource(newValue, new PivotComplexListExtractionAlgorithm(this));
			}
			DoAfterDataSourceIsChanged();
		}
		void UnsubscribeDataSourceEvents(object dataSource) {
			if(IsQueryableDataSource(dataSource)) {
				IListSource listSource = dataSource as IListSource;
				if(listSource != null) {
					IBindingList queryableDataSourceBindingList = (IBindingList)listSource.GetList();
					queryableDataSourceBindingList.ListChanged -= OnQueryableChanged;
				}
			}
		}
		bool IsQueryableDataSource(object dataSource) {
			if(dataSource is DevExpress.Xpf.Core.ServerMode.LinqServerModeDataSource || dataSource is DevExpress.Xpf.Core.ServerMode.EntityServerModeDataSource)
				return true;
			if(!(dataSource is ILinqServerModeFrontEndOwner))
				return false;
			IListSource listSource = dataSource as IListSource;
			if(listSource == null)
				return false;
			return listSource.GetList() is IBindingList;
		}
		IQueryable GetQueryable(object queryableDataSource) {
			ILinqServerModeFrontEndOwner owner = queryableDataSource as ILinqServerModeFrontEndOwner;
			if(owner != null)
				return owner.QueryableSource;
			DevExpress.Xpf.Core.ServerMode.LinqServerModeDataSource lq = queryableDataSource as DevExpress.Xpf.Core.ServerMode.LinqServerModeDataSource;
			if(lq != null)
				return lq.QueryableSource;
			return ((DevExpress.Xpf.Core.ServerMode.EntityServerModeDataSource)queryableDataSource).QueryableSource;
		}
		void SetQueryablePivotDataSource(IQueryable queryable) {
			Data.PivotDataSource = new ServerModeDataSource(new QueryableQueryExecutor(queryable));
		}
		void OnQueryableChanged(object sender, ListChangedEventArgs e) {
			if(IsLoading)
				return;
			SetQueryablePivotDataSource(GetQueryable(DataSource));
			DoAfterDataSourceIsChanged();
		}
		protected virtual void OnOlapSourceChanged(string oldValue, string newValue) {
			if(IsLoading || IsDesignMode) return;
#if SL
			bool locked = Data.IsLockUpdate;
			if(!locked)
				BeginUpdate();
#endif
			if(!string.IsNullOrEmpty(newValue))
				DataSource = null;
			Data.OLAPConnectionString = newValue;
			DoAfterDataSourceIsChanged();
#if SL
			if(!locked)
				EndUpdateAsync((result) => {
					OnOlapDataLoaded();
				});
#endif
		}
		void DoAfterDataSourceIsChanged() {
			OnFocusedCellChanged(FocusedCell);
			OnSelectionChanged(Selection);
			this.CoerceValue(GroupFieldsInFieldListProperty);
		}
		protected virtual void OnOlapDataLoaded() {
			RaiseOlapDataLoaded();
		}
		protected virtual void OnPrefilterStringChanged(string oldValue, string newValue) {
			if(Prefilter.CriteriaString == newValue) return;
			Prefilter.CriteriaString = newValue;
			PrefilterCriteria = Prefilter.Criteria;
			PrefilterString = Prefilter.CriteriaString;
		}
		protected virtual void OnPrefilterCriteriaChanged(CriteriaOperator oldValue, CriteriaOperator newValue) {
			if(!object.ReferenceEquals(newValue, Prefilter.Criteria)) {
				Prefilter.Criteria = newValue;
				PrefilterString = Prefilter.CriteriaString;
			}
			if(Data.Prefilter.State != CoreXtraPivotGrid.PrefilterState.Invalid) {
				AddMRUFilter(oldValue);
				RemoveMRUFilter(Prefilter.Criteria);
			}
			UpdatePrefilterPanel();
		}
		protected virtual void OnFocusedCellChanged(System.Drawing.Point pt) {
			if(IsDataSourceActive)
				VisualItems.FocusedCell = pt;
		}
		protected virtual void OnSelectionChanged(System.Drawing.Rectangle selection) {
			if(IsDataSourceActive)
				VisualItems.Selection = selection;
		}
		protected virtual void OnFieldListStateChanged() {
			if(!applyFieldListStateLocker.IsLocked && FieldListStyle == FieldListStyle.Simple)
				ActualFieldList.ApplyState(FieldListState);
		}
		protected virtual void OnExcelFieldListStateChanged() {
			if(!applyFieldListStateLocker.IsLocked && FieldListStyle == FieldListStyle.Excel2007)
				ActualFieldList.ApplyState(ExcelFieldListState);
		}
		protected virtual void OnIsFieldListVisibleChanged() {
			if(IsDesignMode) return;
			if(IsFieldListVisible) {
				SetFieldListSize();
				ActualFieldList.Show();
				FrameworkElement topContainer = ActualFieldList.TopContainer as FrameworkElement;
				if(topContainer != null)
					topContainer.DataContext = this;
				RaiseShownFieldList();
			} else {
				if(actualFieldList != null)
					ActualFieldList.Hide();
				SaveCurrentFieldListState();
				RaiseHiddenFieldList();
			}
		}
		protected virtual void OnMRUFilterListCountChanged() {
			if(MRUFiltersInternal.Count <= MRUFilterListCount)return;
			for(int i = MRUFiltersInternal.Count - 1; i > MRUFilterListCount; i--) {
				MRUFiltersInternal.RemoveAt(i);
			}
		}
		protected virtual void OnIsPrefilterVisibleChanged() {
			UserAction = IsPrefilterVisible ? UserAction.Prefilter : UserAction.None;
			if(IsDesignMode) return;
			if(IsPrefilterVisible) {
				if(IsLoaded)
					ShowPrefilterCore();
			} else
				HidePrefilterCore();			
		}
		protected virtual void ShowPrefilterCore() {
			if(PrefilterControl != null) HidePrefilterCore();
			PrefilterControl = CreateFilterControl(Data);
			PrefilterControl.ShowBorder = false;
			Binding bindingFilterCriteria = new Binding() {
				Source = this, Path = new PropertyPath(PrefilterCriteriaPropertyName), Mode = BindingMode.TwoWay
			};
			PrefilterControl.SetBinding(FilterControl.FilterCriteriaProperty, bindingFilterCriteria);
			RoutedEventArgs args = new PivotFilterEditorEventArgs(PrefilterControl) { RoutedEvent = PrefilterEditorCreatedEvent };
			RaiseEvent(args);
			if(args.Handled)
				return;
#if !SL
			PrefilterContainer = 
#endif
			FloatingContainer.ShowDialogContent(PrefilterControl, this, new Size(500, 350), new FloatingContainerParameters() {
				Title = PivotGridLocalizer.GetString(PivotGridStringId.PrefilterFormCaption),
				AllowSizing = true,
				ShowApplyButton = true,
				CloseOnEscape = false,
#if !SL
				ShowModal = false,
#endif
				ClosedDelegate = OnPrefilterClosed
			});
#if SL
			PrefilterContainer = PrefilterControl.GetValue(FloatingContainer.DialogOwnerProperty) as DXDialog;
#endif
		}
		protected virtual FilterControl CreateFilterControl(object sourceControl) {
			return new PivotFilterControl() { SourceControl = sourceControl };
		}
		protected virtual void HidePrefilterCore() {
			if(PrefilterControl == null) return;
			RoutedEventArgs args = new PivotFilterEditorEventArgs(PrefilterControl) { RoutedEvent = PrefilterEditorHidingEvent };
			RaiseEvent(args);
			if(args.Handled)
				return;
#if !SL
			if(PrefilterContainer != null && PrefilterContainer.IsOpen) {
				PrefilterContainer.Close();
				PrefilterContainer = null;
			}
#else
			if(PrefilterContainer != null && PrefilterContainer.IsVisible) {
				PrefilterContainer.Hide();
				PrefilterContainer = null;
			}
#endif
			PrefilterControl = null;
		}
		protected virtual void OnPrefilterClosed(bool? dialogResult) {
			IsPrefilterVisible = false;
		}
		protected virtual void OnFieldListTemplateChanged(ControlTemplate controlTemplate) { }
		protected virtual void OnExcelFieldListTemplateChanged(ControlTemplate controlTemplate) { }
		protected virtual object CoerceFieldListFactory(IColumnChooserFactory baseValue) {
			return baseValue ?? DefaultFieldListFactory.Instance;
		}
		protected virtual void OnFieldListFactoryChanged() {
			IsFieldListVisible = false;
			ActualFieldList = CreateFieldList();
		}
		protected virtual void OnFieldListStyleChanged(FieldListStyle fieldListStyle) {
			applyFieldListStateLocker.Lock();
			IsFieldListVisible = false;
			SaveCurrentFieldListState(true);
			SetFieldListSize();
			applyFieldListStateLocker.Unlock();
		}
		protected virtual void SetFieldListSize() {
			DefaultColumnChooserState newState;
			SetDefaultFieldListState();
			if(FieldListStyle == FieldListStyle.Excel2007) {
				newState = ExcelFieldListState as DefaultColumnChooserState;
			} else {
				newState = FieldListState as DefaultColumnChooserState;
			}
			if(ActualFieldList != null) {
				DefaultFieldList fieldList = ActualFieldList as DefaultFieldList;
				if(fieldList == null || fieldList.Container == null)
					return;
				fieldList.ApplyState(newState);
			}
		}
		protected virtual void SaveCurrentFieldListState() {
			SaveCurrentFieldListState(false);
		}
		protected virtual void SaveCurrentFieldListState(bool inverse) {
			if(applyFieldListStateLocker.IsLocked != inverse)
				return;
			SetDefaultFieldListState();
			DefaultColumnChooserState actualState;
			if((FieldListStyle == FieldListStyle.Excel2007) != inverse) {
				actualState = ExcelFieldListState as DefaultColumnChooserState;
			} else {
				actualState = FieldListState as DefaultColumnChooserState;
			}
			if(actualFieldList != null) {
				DefaultFieldList fieldList = ActualFieldList as DefaultFieldList;
				if(fieldList == null || fieldList.Container == null)
					return;
				fieldList.SaveState(actualState);
				FloatingWindowContainer container = fieldList.Container as FloatingWindowContainer;
				if(container != null && container.Window != null && !double.IsNaN(container.Window.Width) && !double.IsNaN(container.Window.Height) && container.Window.Width != 0 && container.Window.Height != 0)
					actualState.Size = new Size(container.Window.Width, container.Window.Height);
			}
		}
		void SetDefaultFieldListState() {
			if(FieldListState == null)
				FieldListState = new DefaultColumnChooserState() { Size = GetCorrectedFieldListDefaultSize((Size)DefaultColumnChooserState.SizeProperty.GetMetadata(typeof(DefaultColumnChooserState)).DefaultValue) };
			if(ExcelFieldListState == null)
				ExcelFieldListState = new DefaultColumnChooserState() { Size = GetCorrectedFieldListDefaultSize(DefaultExcelListSize) };
		}
		Size GetCorrectedFieldListDefaultSize(Size size) {
			string themeName = DevExpress.Xpf.Editors.Helpers.ThemeHelper.GetEditorThemeName(this);
			string coreThemeName = DevExpress.Xpf.Editors.Helpers.ThemeHelper.GetThemeName(this);
			if(themeName == Theme.TouchlineDarkName || coreThemeName != null && coreThemeName.EndsWith(";Touch")) {
				size.Width *= 1.5;
				size.Height *= 1.5;
			}
			return size;
		}
		protected virtual void OnAllowDragChanged(bool newValue) {
			Data.OptionsCustomization.AllowDrag = newValue;
		}
		protected virtual void OnAllowDragInCustomizationFormChanged(bool newValue) {
			Data.OptionsCustomization.AllowDragInCustomizationForm = newValue;
		}
		protected virtual void OnFieldListLayoutChanged(FieldListLayout layout) { }
		protected virtual void OnFieldListAllowedLayoutsChanged(FieldListAllowedLayouts allowedLayouts) {
			if(actualFieldList == null || FieldListStyle == FieldListStyle.Simple || !IsFieldListVisible)
				return;
			DefaultFieldList defaultFieldList = ActualFieldList as DefaultFieldList;
			if(defaultFieldList == null)
				return;
			PivotExcelFieldListControl excelFieldListControl = defaultFieldList.ContentControl.GetElementByName("ExcelFieldList") as PivotExcelFieldListControl;
			if(excelFieldListControl != null)
				excelFieldListControl.UpdateLayout();
		}
		protected virtual void OnShowFilterHeadersChanged(bool newValue) {
			Data.OptionsView.ShowFilterHeaders = newValue;
			RaiseShowHeadersPropertyChanged();
		}
		protected virtual void OnShowDataHeadersChanged(bool newValue) {
			Data.OptionsView.ShowDataHeaders = newValue;
			RaiseShowHeadersPropertyChanged();
		}
		protected virtual void OnShowColumnHeadersChanged(bool newValue) {
			Data.OptionsView.ShowColumnHeaders = newValue;
			RaiseShowHeadersPropertyChanged();
		}
		protected virtual void OnShowRowHeadersChanged(bool newValue) {
			Data.OptionsView.ShowRowHeaders = newValue;
			RaiseShowHeadersPropertyChanged();
		}
#if !SL
		protected virtual void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(!(bool)e.NewValue) {
				HidePopups();
			}
		}
#endif
		void HidePopups() {
			HideFieldList();
			HidePrefilter();
			HideUnboundExpressionEditor();
		}
		protected virtual void OnGroupFilterModeChanged(GroupFilterMode value) {
			Data.OptionsFilter.GroupFilterMode = value.ToGroupFilterMode();
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].Group != null)
					Fields[i].OnInternalFieldChanged(Fields[i], new FieldSyncPropertyEventArgs(FieldSyncProperty.Filtered));
		}
		protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			if(IsPrefilterVisible)
				ShowPrefilterCore();
			GridMenu.Init();
			RegisteredFieldListControl(this);
		}
		protected virtual void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			GridMenu.Reset();
#if SL
			HidePopups();
#endif
			ActualFieldList = null;
			UnregisteredFieldListControl(this);
		}
		protected void UpdateFieldAppearances() {
			UpdateColumns((field) => field.UpdateAppearance());
		}
		void UpdateColumns(UpdateFieldDelegate updateFieldDelegate) {
			foreach(PivotGridField field in Fields) {
				updateFieldDelegate(field);
			}
			updateFieldDelegate(Data.DataField);
		}
		List<object> logicalChildren = new List<object>();
		internal void AddChild(object child) {
			Action action = delegate() {
				AddLogicalChild(child);
				logicalChildren.Add(child);
			};
			InvokeAction(action, this);
		}
		internal void RemoveChild(object child) {
			Action action = delegate() {
				logicalChildren.Remove(child);
				RemoveLogicalChild(child);
			};
			InvokeAction(action, this);
		}
		internal static void InvokeAction(Action action, PivotGridControl owner) {
			if(owner == null || !owner.IsAsyncInProgress)
				action.Invoke();
			else
				new PropertiesSynchronizer().SyncProperty(owner.Data, true, false, null, action);
		}
#if SL
		internal event MouseButtonEventHandler MouseDoubleClick;
		DoubleClickImplementer doubleClickImplementer;
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			doubleClickImplementer.OnMouseLeftButtonUpDoubleClickForce(e, (sender, args) => OnMouseDoubleClick(args), MouseDoubleClick);
			base.OnMouseLeftButtonUp(e);
		}
		protected virtual void OnMouseDoubleClick(MouseButtonEventArgs e) {
		}
#endif
#if SL
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add { } remove { } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
#else
		internal void AddVisualchild(System.Windows.Media.Visual element) {
			AddVisualChild(element);
		}
		internal void RemoveVisualchild(System.Windows.Media.Visual element) {
			RemoveVisualChild(element);
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return logicalChildren.GetEnumerator(); }
		}
#endif
		protected override bool HandlesScrolling {
			get { return true; }
		}
		protected internal virtual bool OnKeyDown(Key keyCode) {
			switch(keyCode) {
				case Key.Escape:
					return true;
				case Key.Insert:
				case Key.C:
					if(VisualItems.IsControlDown) {
						VisualItems.CopySelectionToClipboard();
						return true;
					}
					break;
			}
			if(FlowDirection == System.Windows.FlowDirection.RightToLeft) {
				if(keyCode == Key.Right)
					keyCode = Key.Left;
				else
					if(keyCode == Key.Left)
						keyCode = Key.Right;
			}
#if !SL
			int virtualKey = KeyInterop.VirtualKeyFromKey(keyCode);
#else
			int virtualKey = (int)keyCode;
#endif
			return VisualItems.OnKeyDown(virtualKey, VisualItems.IsControlDown, VisualItems.IsShiftDown);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			EnsureFocus();
			base.OnMouseLeftButtonDown(e);
		}
		public override void OnApplyTemplate() {
			pivotGridScroller = GetPivotGridScroller();
		}
#if !SL
		protected override Size MeasureOverride(Size constraint) {
			if(!IsDesignMode) 
				InfinitePivotGridSizeException.ValidateDefineSize(constraint);
			return base.MeasureOverride(constraint);
		}
#endif
#if !SL //TODO
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			OnFocusChanged();
		}		
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			OnFocusChanged();
		}
#else
		protected override void OnGotFocus(System.Windows.RoutedEventArgs e) {
			base.OnGotFocus(e);
			if(FocusManager.GetFocusedElement() == this)
				EnsureFocus();
		}
#endif
		protected virtual void OnFocusChanged() {
			VisualItems.RaiseFocusedCellChanged(VisualItems.FocusedCell, VisualItems.FocusedCell);
		}
		protected internal virtual void EnsureFocus() {
#if !SL
			if(Focusable && !IsFocused)
				Focus();;
#else
			DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
			if(Focusable && (!IsFocused || IsFocused && focusedElement == this) && (focusedElement == null || !LayoutHelper.IsChildElement(this, focusedElement) || focusedElement == this)) {
				if(PivotGridScroller != null && PivotGridScroller.Scroller != null) {
					DependencyObject newFocusedElement = pivotGridScroller.Cells;
					if(IsDesignMode && !LayoutHelper.IsChildElement(this,pivotGridScroller.Cells))
						return;
					while((!(newFocusedElement is System.Windows.Controls.Control) || !((System.Windows.Controls.Control)newFocusedElement).Focus()) && newFocusedElement != this)
						newFocusedElement = System.Windows.Media.VisualTreeHelper.GetParent(newFocusedElement);
				} else
					Focus();
			}
#endif
		}
		protected virtual bool OnReceiveWeakEvent(Type managerType, object sender, EventArgs e) {			
			return false;
		}
		public void SaveLayoutToXml(string fileName) {
			SerializationController.SaveLayout(fileName);
		}
		public void SaveLayoutToStream(Stream stream) {
			SerializationController.SaveLayout(stream);
		}
		public void RestoreLayoutFromXml(string fileName) {
			SerializationController.RestoreLayout(fileName);
		}
		public void RestoreLayoutFromStream(Stream stream) {
			SerializationController.RestoreLayout(stream);
		}
#if !SL
		public void SavePivotGridToFile(string path) {
			SavePivotGridToFile(path, false);
		}
		public void SavePivotGridToFile(string path, bool compress) {
			Data.SavePivotGridToFile(path, compress);
		}
		public void SavePivotGridToStream(Stream stream) {
			SavePivotGridToStream(stream, false);
		}
		public void SavePivotGridToStream(Stream stream, bool compress) {
			Data.SavePivotGridToStream(stream, compress);
		}
		public void RestorePivotGridFromFile(string path) {
			DataSource = new PivotFileDataSource(path);
		}
		public void RestorePivotGridFromStream(Stream stream) {
			DataSource = new PivotFileDataSource(stream);
		}
#endif
		public void SaveCollapsedStateToStream(Stream stream) {
			Data.SaveCollapsedStateToStream(stream);
		}
		public void SaveCollapsedStateToFile(string path) {
			using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				SaveCollapsedStateToStream(stream);
			}
		}
		public void RestoreCollapsedStateFromStream(Stream stream) {
			Data.LoadCollapsedStateFromStream(stream);
			LayoutChanged();
		}
		public void RestoreCollapsedStateFromFile(string path) {
			FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			RestoreCollapsedStateFromStream(stream);
			stream.Close();
		}
		protected internal virtual void UpdatePrefilterPanel() {
			if(Data == null) return;
			ShowPrefilterPanel = GetActualShowPrefilterPanel();
			CriteriaOperatorInfo filterInfo = CreateCriteriaOperatorInfo(Prefilter.Criteria, true);
			PrefilterPanelText = filterInfo.FilterText;
			ActiveFilterInfo = filterInfo;
			IsPrefilterEnabled = Prefilter.Enabled;
			CanEnablePrefilter = !Prefilter.IsEmpty && (Prefilter.State != CoreXtraPivotGrid.PrefilterState.Invalid);
		}
		protected virtual void UpdateFieldsButtonsState() {
			foreach(PivotGridField field in Fields) {
				field.ReadFromFieldButtonsState();
			}
		}
		protected internal virtual void UpdateScrollbars() {
			if(pivotGridScroller != null)
				pivotGridScroller.Cells.EnsureTopLeft(pivotGridScroller.Cells.ActualWidth, pivotGridScroller.Cells.ActualHeight);
			if(PivotGridScroller != null) {
				pivotGridScroller.InvalidateMeasure();
#if SL
				ScrollViewer sv = PivotGridScroller.Content as ScrollViewer;
				if(sv != null) {
					sv.InvalidateScrollInfo();
					sv.InvalidateMeasure();
				}
#endif
			}
		}
		protected virtual PivotGridScroller GetPivotGridScroller() {
			DXPivotGridThemesLoader loader = (DXPivotGridThemesLoader)GetTemplateChild(TemplatePartThemeLoader);
			if(loader == null) return null;
			return (PivotGridScroller)loader.Content;
		}
		internal bool CanSwapAreas() {
			bool flag = true;
			List<PivotGridField> columns = Data.GetFieldsByArea(FieldArea.ColumnArea, true);
			List<PivotGridField> rows = Data.GetFieldsByArea(FieldArea.RowArea, true);
			for(int i = 0; i < columns.Count; i++)
				flag |= Data.OnFieldAreaChanging(columns[i].InternalField, XtraPivotGrid.PivotArea.RowArea, columns[i].AreaIndex);
			for(int i = 0; i < rows.Count; i++)
				flag |= Data.OnFieldAreaChanging(rows[i].InternalField, XtraPivotGrid.PivotArea.ColumnArea, rows[i].AreaIndex);
			return AllowDrag && flag;
		}
	   internal  void SwapAreas() {
			if(UseAsyncMode && IsAsyncInProgress)
				return;
			BeginUpdate();
			List<PivotGridField> columns = Data.GetFieldsByArea(FieldArea.ColumnArea, true);
			List<PivotGridField> rows = Data.GetFieldsByArea(FieldArea.RowArea, true);
			for(int i = 0; i < columns.Count; i++)
				columns[i].Area = FieldArea.RowArea;
			for(int i = 0; i < rows.Count; i++)
				rows[i].Area = FieldArea.ColumnArea;
			if(UseAsyncMode) 
				EndUpdateAsync();
			else
				EndUpdate();
		}
		protected virtual bool GetActualShowPrefilterPanel() {
			if(ShowPrefilterPanelMode == ShowPrefilterPanelMode.Never)
				return false;
			if(ShowPrefilterPanelMode == ShowPrefilterPanelMode.ShowAlways)
				return true;
			return !Prefilter.IsEmpty;
		}
		protected virtual string GetPrefilterPanelText(object op) {
			if(op == null)
				return string.Empty;
			CriteriaOperator criteria = op as CriteriaOperator;
			if(!object.Equals(null, criteria))
				return LocalaizableCriteriaToStringProcessor.Process(criteria);
			return op.ToString();
		}
		protected virtual void AddMRUFilter(CriteriaOperatorInfo filter) {
			if(filter == null) return;
			if(MRUFiltersInternal.Contains(filter)) {
				MRUFiltersInternal.Remove(filter);
			}
			MRUFiltersInternal.Insert(0, filter);
			if(MRUFiltersInternal.Count > MRUFilterListCount) {
				MRUFiltersInternal.RemoveAt(MRUFilterListCount);
			}
		}
		protected virtual void RemoveMRUFilter(CriteriaOperatorInfo filter) {
			MRUFiltersInternal.Remove(filter);
		}
		class CaptionEqualityCriteriaOperatorInfo : CriteriaOperatorInfo {
			public CaptionEqualityCriteriaOperatorInfo(CriteriaOperator filterOperator, string filterText) : base(filterOperator, filterText) {
			}
			public override bool Equals(object obj) {
				return base.Equals(obj) && string.Equals(this.FilterText, ((CriteriaOperatorInfo)obj).FilterText);
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
			public override string ToString() {
				return base.ToString();
			}
		}
		protected virtual CriteriaOperatorInfo CreateCriteriaOperatorInfo(CriteriaOperator criteriaOperator, bool captionEquality) {
			CriteriaOperator op = DisplayCriteriaGenerator.Process(new DisplayCriteriaHelper(this), criteriaOperator);
			CustomPrefilterDisplayTextEventArgs e = new CustomPrefilterDisplayTextEventArgs(op) {
				RoutedEvent = PivotGridControl.CustomPrefilterDisplayTextEvent
			};
			RaiseCustomPrefilterDisplayText(e);
			string displayText = GetPrefilterPanelText(e.Value);
			return captionEquality ? new CaptionEqualityCriteriaOperatorInfo(criteriaOperator, displayText) : new CriteriaOperatorInfo(criteriaOperator, displayText);
		}
		public void AddMRUFilter(CriteriaOperator filterCriteria) {
			if(ReferenceEquals(filterCriteria, null)) return;
			CriteriaOperatorInfo filter = CreateCriteriaOperatorInfo(filterCriteria, false);
			AddMRUFilter(filter);
		}
		public void RemoveMRUFilter(CriteriaOperator filterCriteria) {
			CriteriaOperatorInfo filter = CreateCriteriaOperatorInfo(filterCriteria, false);
			RemoveMRUFilter(filter);
		}		
		public void ClearMRUFilter() {
			MRUFiltersInternal.Clear();
		}
		#region command handlers
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			RaisePropertyChanged(e, this);
			if (e.Property == PivotGridControl.ShowColumnHeadersProperty || e.Property == PivotGridControl.ShowFilterHeadersProperty || e.Property == ShowDataHeadersProperty)
				UpdateShowColumnsBorder();
		}
#if !SL
		protected virtual void OnCanShowFieldList(CanExecuteRoutedEventArgs e) {
			e.CanExecute = !IsFieldListVisible;
		}
		protected virtual void OnCanHideFieldList(CanExecuteRoutedEventArgs e) {
			e.CanExecute = IsFieldListVisible;
		}
		protected virtual void OnCanHideField(CanExecuteRoutedEventArgs e) {
			PivotGridField field = e.Parameter as PivotGridField;
			e.CanExecute = field != null ? field.CanHide : false;
		}
		protected virtual void OnIsValidExpandCollapseField(CanExecuteRoutedEventArgs e) {
			PivotGridField field = e.Parameter as PivotGridField;
			e.CanExecute = !(field == null || !field.Area.IsColumnOrRow() || field.PivotGrid == null || field.AreaIndex == field.PivotGrid.GetFieldCountByArea(field.Area) - 1);
		}
		protected virtual void OnChangeFieldSortOrder(ExecutedRoutedEventArgs e) {
			Data.ChangeFieldSortOrderAsync(((PivotGridField)e.Parameter), false);
			e.Handled = true;
			if(DesignTimeAdorner != null)
				DesignTimeAdorner.PerformChangeSortOrder((PivotGridField)e.Parameter);
		}
		protected internal virtual void OnShowUnboundExpressionEditor(ExecutedRoutedEventArgs e) {
			PivotGridField field = e.Parameter as PivotGridField;
			if(OnCanShowUnboundExpressionEditorForColumn(field))
				ShowUnboundExpressionEditor(field);
		}
		protected virtual void OnCanShowUnboundExpressionEditor(CanExecuteRoutedEventArgs e) {
			e.CanExecute = OnCanShowUnboundExpressionEditorForColumn((PivotGridField)e.Parameter) && Data.IsCapabilitySupported(PivotDataSourceCaps.UnboundColumns);
		}
		protected virtual void OnCanShowPrefilter(CanExecuteRoutedEventArgs e) {
			e.CanExecute = AllowPrefilter && Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter);
		}
		protected virtual void OnCanHidePrefilter(CanExecuteRoutedEventArgs e) {
			e.CanExecute = IsPrefilterVisible && Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter);
		}
#else
		protected internal virtual void OnChangeFieldSortOrder(PivotGridField e) {
			Data.ChangeFieldSortOrderAsync(e, false);
		}
		protected internal virtual bool OnCanShowFieldList() {
			return !IsFieldListVisible;
		}
		protected internal virtual bool OnCanHideFieldList() {
			return IsFieldListVisible;
		}
		protected internal virtual bool OnCanHideField(PivotGridField field) {
			return field != null ? field.CanHide : false;
		}
		protected internal virtual void OnShowUnboundExpressionEditor(PivotGridField field) {
			if(OnCanShowUnboundExpressionEditorForColumn(field))
				ShowUnboundExpressionEditor(field);
		}
		protected internal virtual bool OnCanShowUnboundExpressionEditor(PivotGridField field) {
			return OnCanShowUnboundExpressionEditorForColumn(field) && Data.IsCapabilitySupported(PivotDataSourceCaps.UnboundColumns);
		}
		protected internal virtual bool OnCanShowPrefilter() {
			return AllowPrefilter && Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter);
		}
		protected virtual bool OnCanHidePrefilter() {
			return IsPrefilterVisible && Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter);
		}
#endif
#if !SL
		protected virtual void OnChangeFieldValueExpanded(ExecutedRoutedEventArgs e) {
			PivotFieldValueItem item = (PivotFieldValueItem)e.Parameter;
#else
		protected internal virtual void OnChangeFieldValueExpanded(PivotFieldValueItem item) {
#endif
			PivotFieldValueItem prevItem = item.Field != null ? VisualItems.GetItem(item.Field, item.MinLastLevelIndex - 1) : null;
			if(prevItem == null || !object.Equals(prevItem.Value, item.Value))
				prevItem = item;
			Data.ChangeExpandedAsync(item, false);
			if(PivotGridScroller == null)
				return;
			if(!item.IsColumn) {
				if(prevItem.MinLastLevelIndex < PivotGridScroller.Cells.Top)
					PivotGridScroller.Cells.Top = prevItem.MinLastLevelIndex;
			} else {
				if(prevItem.MinLastLevelIndex < PivotGridScroller.Cells.Left)
					PivotGridScroller.Cells.Left = prevItem.MinLastLevelIndex;
			}
		}
		protected virtual bool OnCanShowUnboundExpressionEditorForColumn(PivotGridField field) {
			return field != null && field.AllowUnboundExpressionEditor && Data.IsCapabilitySupported(PivotDataSourceCaps.UnboundColumns);
		}
		#endregion
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveChild(child);
		}
		#endregion
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return OnReceiveWeakEvent(managerType, sender, e);
		}
		#endregion        
	   internal bool GetPrintHeaders(CoreXtraPivotGrid.PivotArea area) {
		   switch (area){
			   case CoreXtraPivotGrid.PivotArea.ColumnArea:
				   return PrintColumnHeaders;
			   case CoreXtraPivotGrid.PivotArea.DataArea:
				   return PrintDataHeaders;
			   case CoreXtraPivotGrid.PivotArea.FilterArea:
				   return PrintFilterHeaders;
			   case CoreXtraPivotGrid.PivotArea.RowArea:
				   return PrintRowHeaders;
			   default:
				   throw new InvalidOperationException(area.ToString());
		   }
	   }
	   void UpdateShowColumnsBorder() {
		   ShowColumnsBorder = ShowColumnHeaders || ShowFilterHeaders || ShowDataHeaders;
	   }
	   internal bool IsDesignMode { 
		   get {
			   if(isDesignMode.HasValue) return isDesignMode.Value;
			   return DesignerProperties.GetIsInDesignMode(this);
		   }
	   }
	   internal void InitIsDesignMode(bool makeDefault) {
		   isDesignMode = makeDefault ? null : (bool?)IsDesignMode;
	   }
	   internal IEnumerable<UIElement> GetTopLevelDropContainers() {
		   return DragDropManager.GetTopLevelDropContainers();
	   }
	   protected PivotDragDropManager DragDropManager { get; private set; }
	   protected internal void UnregisteredFieldListControl(Control fieldList) {
		   DragDropManager.UnregisteredFieldListControl(fieldList);
	   }
	   protected internal void RegisteredFieldListControl(Control fieldList) {
		   DragDropManager.RegisteredFieldListControl(fieldList);
	   }
	   protected void OnOlapFilterByUniqueNamePropertyChanged() {
		   Data.OptionsOLAP.FilterByUniqueName = OlapFilterByUniqueName;
	   }
#if !SL
	   protected void OlapDataProviderPropertyChanged() {
		   Data.OLAPDataProvider = OlapDataProvider.ToOLAPDataProvider();
	   }
#endif
	   protected override void OnInitialized(EventArgs e) {
		   base.OnInitialized(e);
		   this.CoerceValue(PivotGridControl.GroupFieldsInFieldListProperty);
	   }
#if SL
	   Decorator IPopupContainer.PopupContainer { get; set; }
#endif
	   internal bool IsDragging { get; set; }
	   internal FieldHeaderBase.PivotDragDropElementHelper CurrenDragDropElementHelper { get; set; }
#if DEBUGTEST
	   internal IColumnChooser GetActualFieldList(){
		   return ActualFieldList;
		}
#endif
	   internal void OnEndDeserializing() {
		   this.CoerceValue(PivotGridControl.DataFieldHeightProperty);
		   this.CoerceValue(PivotGridControl.DataFieldWidthProperty);
	   }
	   internal void SetLeftTopCoord(System.Drawing.Point point, Point offset) {
		   LeftTopCoord = point;
		   LeftTopPixelCoord = new Point(VisualItems.CellSizeProvider.GetWidthDifference(true, 0, point.X) + offset.X,
								VisualItems.CellSizeProvider.GetHeightDifference(false, 0, point.Y) + offset.Y); 
	   }
	   #region format rules
	   void IFormatConditionCollectionOwner.OnFormatConditionCollectionChanged(FormatConditionChangeType changeType) {
		   Data.DoRefresh();
	   }
	   void IFormatConditionCollectionOwner.SyncFormatCondtitionPropertyWithDetails(FormatConditionBase item, DependencyPropertyChangedEventArgs e) {
		   Data.DoRefresh();
	   }
	   void IFormatConditionCollectionOwner.SyncFormatCondtitionCollectionWithDetails(NotifyCollectionChangedEventArgs e) {
		   Data.DoRefresh();
	   }
	   public void AddFormatCondition(FormatConditionBase formatCondition) {
		   if(formatCondition == null)
			   return;
		   IModelItem dataControl = GetParentModel();
		   using(IModelEditingScope scope = dataControl.BeginEdit("Add format condition")) {
			   IModelItemCollection formatConditions = GetConditions().Collection;
			   if(formatCondition is IndicatorFormatConditionBase) {
				   var formatConditionsToRemove = formatConditions
					   .Where(x => x.ItemType == formatCondition.GetType() &&
									   (x.Properties[FormatConditionBase.MeasureNameProperty.Name].ComputedValue as string) == formatCondition.MeasureName &&
									   (x.Properties[FormatConditionBase.ColumnNameProperty.Name].ComputedValue as string) == formatCondition.ColumnName &&
									   (x.Properties[FormatConditionBase.RowNameProperty.Name].ComputedValue as string) == formatCondition.RowName
						   )
					   .ToArray();
				   foreach(var condition in formatConditionsToRemove) {
					   formatConditions.Remove(condition);
				   }
			   }
			   formatConditions.Add(CreateModelItem(formatCondition));
			   scope.Complete();
		   }
	   }
	   internal IModelItem GetParentModel() {
		   if(DesignTimeAdorner == null)
			   return new RuntimeEditingContext(this).GetRoot();
		   else
			   return DesignTimeAdorner.GetPivotGridModelItem();	
	   }
	   IModelProperty GetConditions() {
		   return GetParentModel().Properties["FormatConditions"];
	   }
	   IModelItem CreateModelItem(object obj) {
		   EditingContextBase context = GetParentModel().Context as EditingContextBase;
		   return context != null ? context.CreateModelItem(obj, GetParentModel()) : null;
	   }
	   FormatConditionDialogType[] intersectionRules = new FormatConditionDialogType[] { 
																		 FormatConditionDialogType.Top10Items, FormatConditionDialogType.Bottom10Items,
																		 FormatConditionDialogType.Top10Percent, FormatConditionDialogType.Bottom10Percent, 
																		 FormatConditionDialogType.AboveAverage, FormatConditionDialogType.BelowAverage };
	   public void ShowFormatConditionDialog(PivotGridField measure, PivotGridField row, PivotGridField column, FormatConditionDialogType dialogKind) {
		   DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<FrameworkElement, IDialogService>.DoServiceAction(this, FormatConditionDialogServiceTemplate, service => {
			   ConditionalFormattingDialogViewModel viewModel = GetViewModelFactory(dialogKind)(this);
			   viewModel.ApplyFormatToWholeRowText = PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesIntersectionOnly);
			   viewModel.ApplyFormatToWholeRowEnabled = !intersectionRules.Contains(dialogKind);
			   viewModel.ApplyFormatToWholeRow = intersectionRules.Contains(dialogKind);
			   viewModel.Initialize(new DataControlDialogContext(measure, new FormatConditionCommandParameters() { Measure = measure, Row = row, Column = column }));
			   List<UICommand> commands = UICommand.GenerateFromMessageBoxButton(MessageBoxButton.OKCancel, new DXDialogWindowMessageBoxButtonLocalizer());
			   commands[0].Command = new DelegateCommand<CancelEventArgs>(x => x.Cancel = !viewModel.TryClose());
			   var result = service.ShowDialog(commands, viewModel.Title, viewModel);
			   if(result == commands[0]) {
				   IModelItem dataControl = GetParentModel();
				   IModelItemCollection formatConditions = GetConditions().Collection;
				   IModelItem formatCondition = viewModel.CreateCondition(dataControl.Context, measure.Name);
				   viewModel.SetFormatProperty(formatCondition);
				   formatConditions.Add(formatCondition);
			   }
		   });
	   }
	   string GetFieldName(PivotGridField field) {
		   return field == null ? null : field.Name;
	   }
	   Func<IFormatsOwner, ConditionalFormattingDialogViewModel> GetViewModelFactory(FormatConditionDialogType dialogKind) {
		   switch(dialogKind) {
			   case FormatConditionDialogType.GreaterThan:
				   return GreaterThanConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.LessThan:
				   return LessThanConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.Between:
				   return BetweenConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.EqualTo:
				   return EqualToConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.TextThatContains:
				   return TextThatContainsConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.ADateOccurring:
				   return DateOccurringConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.CustomCondition:
				   return CustomConditionConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.Top10Items:
				   return Top10ItemsConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.Bottom10Items:
				   return Bottom10ItemsConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.Top10Percent:
				   return Top10PercentConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.Bottom10Percent:
				   return Bottom10PercentConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.AboveAverage:
				   return AboveAverageConditionalFormattingDialogViewModel.Factory;
			   case FormatConditionDialogType.BelowAverage:
				   return BelowAverageConditionalFormattingDialogViewModel.Factory;
			   default:
				   throw new InvalidOperationException();
		   }
	   }
	   public void ClearFormatConditionsFromAllMeasures() {
		   GetConditions().Collection.Clear();
	   }
	   public void ClearFormatConditionsFromMeasure(PivotGridField measure) {
		   if(measure == null)
			   return;
		   IModelItemCollection formatConditions = GetConditions().Collection;
		   var columnConditions = formatConditions.Where(x => ((FormatConditionBase)x.GetCurrentValue()).MeasureName == measure.Name).ToArray();
		   if(columnConditions.Length == 0)
			   return;
		   FormatConditions.BeginUpdate();
		   try {
			   foreach(var item in columnConditions) {
				   formatConditions.Remove(item);
			   }
		   } finally {
			   FormatConditions.EndUpdate();
		   }
	   }
	   public void ClearFormatConditionsFromIntersection(PivotGridField rowField, PivotGridField columnField) {
		   string rowFieldName = rowField == null ? null : rowField.Name;
		   string columnFieldName = columnField == null ? null : columnField.Name;
		   IModelItemCollection formatConditions = GetConditions().Collection;
		   var columnConditions = formatConditions.Where(x => {
			   FormatConditionBase infoBase = (FormatConditionBase)x.GetCurrentValue();
			   return infoBase.RowName == rowFieldName && infoBase.ColumnName == columnFieldName;
		   }).ToArray();
		   if(columnConditions.Length == 0)
			   return;
		   FormatConditions.BeginUpdate();
		   try {
			   foreach(var item in columnConditions) {
				   formatConditions.Remove(item);
			   }
		   } finally {
			   FormatConditions.EndUpdate();
		   }
	   }
	   public void ShowConditionalFormattingManager(PivotGridField measure) {
		   DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<FrameworkElement, IDialogService>.DoServiceAction(this, ConditionalFormattingManagerServiceTemplate, service => {
			   var viewModel = DevExpress.Xpf.Core.ConditionalFormattingManager.ManagerViewModel.Factory(new DataControlDialogContext(measure, new FormatConditionCommandParameters(true) { Measure = measure }));
			   List<UICommand> commands = UICommand.GenerateFromMessageBoxButton(MessageBoxButton.OKCancel, new DXDialogWindowMessageBoxButtonLocalizer());
			   UICommand applyCommand = new UICommand() {
				   Caption = ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Apply),
				   IsCancel = false,
				   IsDefault = false,
				   Command = new DelegateCommand<CancelEventArgs>(e => {
					   e.Cancel = true;
					   viewModel.ApplyChanges();
				   },
				   x => viewModel.CanApply),
			   };
			   commands.Add(applyCommand);
			   UICommand result = service.ShowDialog(commands, viewModel.Description, viewModel);
			   if(result == commands[0])
				   viewModel.ApplyChanges();
		   });
	   }
	   #endregion
	}
}
