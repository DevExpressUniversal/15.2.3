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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using DevExpress.Data;
using DevExpress.Data.Async.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.Native;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data.Linq;
using System.IO;
using System.Windows.Forms;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.ChunkList;
namespace DevExpress.Xpf.Data {
	public class GridSelectionController : ISelectionController {
		readonly DataController controller;
		RowStateController Selection { get { return controller.Selection as RowStateController; } }
		public GridSelectionController(DataController controller) {
			this.controller = controller;
		}
		public void BeginSelection() {
			Selection.BeginSelection();
		}
		public void EndSelection() {
			Selection.EndSelection();
		}
		public int[] GetSelectedRows() {
			return Selection.GetSelectedRows();
		}
		public void SetActuallyChanged() {
			Selection.SetActuallyChanged();
		}
		public void SetSelected(int rowHandle, bool selected) {
			Selection.SetSelected(rowHandle, selected);
		}
		public void SetSelected(int rowHandle, bool selected, object selectionObject) {
			Selection.SetSelected(rowHandle, selected, selectionObject);
		}
		public bool IsSelectionLocked {
			get { return Selection.IsSelectionLocked; }
		}
		public void SelectAll() {
			Selection.SelectAllRows();
		}
		public void Clear() {
			Selection.ClearSelection();
		}
		public bool GetSelected(int controllerRow) {
			return Selection.GetSelected(controllerRow);
		}
		public object GetSelectedObject(int controllerRow) {
			return Selection.GetSelectedObject(controllerRow);
		}
		public int Count { get { return Selection.Count; } }
	}
	public abstract class GridDataProviderBase : DataProviderBase {
		GridGroupSummarySortInfoCollection groupSummarySortInfo;
		public GridDataProviderBase() {
			this.groupSummarySortInfo = new GridGroupSummarySortInfoCollection(this);
		}
		public GridGroupSummarySortInfoCollection GroupSummarySortInfo { get { return groupSummarySortInfo; } }
		public abstract GridSummaryItemCollection GroupSummary { get; }
	}
	public class GridDataProvider : GridDataProviderBase, IDataControllerCurrentSupport, IDataControllerData2, IDataControllerSort, IDataControllerValidationSupport, IWeakEventListener {
		static readonly string[] DummyPropertyDescriptors = new string[] { "SimpleListPropertyDescriptor", "NoQueryablePropertyDescriptor",
			"NoEnumerablePropertyDescriptor", "NoSourcePropertyDescriptor", "NoQueryPropertyDescriptor" };
		internal static DataColumnInfo GetActualColumnInfo(DataController dataController, string fieldName) {
			return dataController.Columns[fieldName] ?? dataController.DetailColumns[fieldName];
		}
		internal static object GetRowValueByListIndex(DataController dataController, int listIndex, string fieldName) {
			DataColumnInfo info = GetActualColumnInfo(dataController, fieldName);
			return GetRowValueByListIndex(dataController, listIndex, info);
		}
		internal static object GetRowValueByListIndex(DataController dataController, int listIndex, DataColumnInfo info) {
			if(info == null)
				return null;
			if(dataController.DetailColumns[info.Name] != null)
				return dataController.Helper.GetRowValueDetail(listIndex, info);
			return dataController.GetListSourceRowValue(listIndex, info.Index);
		}
		internal static object GetRowByListIndex(DataController dataController, int listSourceRowIndex) {
			return dataController.GetRowByListSourceIndex(listSourceRowIndex);
		}
		IDataProviderOwner owner;
		DataSourceProvider provider;
		BaseGridController dataController;
		IList list;
		IDataControllerVisualClient visualClient;
		GridSelectionController gridSelection;
		bool isDisplayMemberBindingInitialized;
		bool autoPopulateColumns;
		bool isRepopulateColumnsNeeded;
		bool isDummySource;
		bool isAsyncOperationInProgress;
		public GridDataProvider(IDataProviderOwner owner) {
			this.owner = owner;
			this.VisibleIndicesProvider = new VisibleIndicesProvider(this);
			VisualClient = (IDataControllerVisualClient)owner;
		}
		static NullDataProviderOwner nullOwner;
		static protected NullDataProviderOwner NullOwner {
			get {
				if(nullOwner == null) {
					nullOwner = new NullDataProviderOwner();
				}
				return nullOwner;
			}
		}
		internal IDataProviderOwner Owner { get { return owner != null ? owner : NullOwner; } }
		protected internal IDataProviderEvents Events { get { return Owner as IDataProviderEvents; } }
		public VisibleIndicesProvider VisibleIndicesProvider { get; private set; }
		public override ISelectionController Selection { get { return gridSelection; } }
		internal protected override BaseGridController DataController {
			get {
				if(dataController == null) {
					dataController = CreateDataController();
					gridSelection = new GridSelectionController(dataController);
					dataController.ListSourceChanged += new EventHandler(OnListSourceChanged);
					dataController.CustomSummary += new CustomSummaryEventHandler(OnCustomSummary);
					dataController.CustomSummaryExists += new CustomSummaryExistEventHandler(OnCustomSummaryExists);
					dataController.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
					dataController.VisibleRowCountChanged += OnVisibleRowCountChanged;
					dataController.VisualClient = VisualClient;
					dataController.DataClient = this;
					dataController.CurrentClient = this;
					dataController.ValidationClient = this;
					dataController.GetVisibleIndexes().ExpandedGroupsIncludedInScrollableIndexes = true;
					if(Events != null)
						dataController.SortClient = this;
				}
				return dataController;
			}
		}
		internal GridServerModeDataControllerPrintInfo SubstituteDataControllerForPrinting(IList printingSource, bool expandAllGroups) {
			BaseGridController printingController = CreateDefaultDataController();
			printingController.ByPassFilter = IsServerMode || IsAsyncServerMode;
			printingController.DataClient = new DevExpress.Xpf.Grid.Printing.PrintingDataClient(this);
			printingController.CustomSummary += new CustomSummaryEventHandler(OnCustomSummary);
			printingController.CustomSummaryExists += new CustomSummaryExistEventHandler(OnCustomSummaryExists);
			printingController.VisualClient = new DevExpress.Xpf.Grid.Printing.PrintingVisualClient(VisualClient);
			printingController.SortClient = this;
			BaseGridController oldDataController = DataController;
			dataController = printingController;
			bool shouldUpdateDataController = printingSource.Count != 0;
			if(shouldUpdateDataController)
				printingController.DataSource = printingSource;
			((DataControlBase)Owner).UpdateTotalSummary();
			Dictionary<ColumnBase, string> summaries = ((DataControlBase)Owner).ColumnsCore.Cast<ColumnBase>().ToDictionary(c => c, c => c.TotalSummaryText);
			string fixedLeftSummaryText = ((DataControlBase)Owner).viewCore.GetFixedSummariesLeftString();
			string fixedRightSummaryText = ((DataControlBase)Owner).viewCore.GetFixedSummariesRightString();
			if(shouldUpdateDataController)
				printingController.VisualClient.RequestSynchronization();
			if(expandAllGroups)
				ExpandAll();
			else
				LoadGroupStatesToPrintingController(oldDataController);
			return new GridServerModeDataControllerPrintInfo(printingController, summaries, fixedLeftSummaryText, fixedRightSummaryText);
		}
		internal void ClearPrintingControllerEvents(BaseGridController printingController) {
			printingController.CustomSummary -= new CustomSummaryEventHandler(OnCustomSummary);
			printingController.CustomSummaryExists -= new CustomSummaryExistEventHandler(OnCustomSummaryExists);
		}
		void LoadGroupStatesToPrintingController(BaseGridController oldDataController) {
			using(MemoryStream ms = new MemoryStream()) {
				oldDataController.SaveRowState(ms);
				ms.Seek(0, SeekOrigin.Begin);
				DataController.RestoreRowState(ms);
			}
		}
		public override void InvalidateVisibleIndicesCache() {
			VisibleIndicesProvider.InvalidateCache();
		}
		void OnVisibleRowCountChanged(object sender, EventArgs e) {
			DataControlBase dcb = owner as DataControlBase;
			if(dcb != null) {
				dcb.RaiseVisibleRowCountChanged();
			}
		}
		void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			Owner.OnSelectionChanged(e);
		}
		protected internal override void ForceClearData() {
			if(dataController == null)
				return;
			dataController.ListSourceChanged -= new EventHandler(OnListSourceChanged);
			dataController.CustomSummary -= new CustomSummaryEventHandler(OnCustomSummary);
			dataController.CustomSummaryExists -= new CustomSummaryExistEventHandler(OnCustomSummaryExists);
			dataController.SelectionChanged -= new SelectionChangedEventHandler(OnSelectionChanged);
			dataController.VisibleRowCountChanged -= OnVisibleRowCountChanged;
			dataController.Dispose();
			dataController = null;
		}
		protected virtual void OnListSourceChanged(object sender, EventArgs e) {
			Owner.OnListSourceChanged();
		}
		void OnCustomSummaryExists(object sender, CustomSummaryExistEventArgs e) {
			if(Events != null)
				Events.OnCustomSummaryExists(sender, e);
		}
		void OnCustomSummary(object sender, CustomSummaryEventArgs e) {
			if(Events != null)
				Events.OnCustomSummary(sender, e);
		}
		BaseGridController CreateDefaultDataController() {
			return new StateGridDataController(Owner);
		}
		protected virtual BaseGridController CreateDataController() {
			if(IsAsyncServerMode) {
				return new AsyncServerModeGridDataController() { ThreadClient = new GridDataControllerThreadClient(this) };
			}
			if(IsServerMode) {
				if(IsICollectionView && !IsIPagedCollectionView)
					return new CollectionViewDataController(owner);
				return new StateServerModeDataController(Owner);
			}
			if(Owner.OptimizeSummaryCalculation && List is IListChanging)
				return new ChunkListDataController(Owner);
			return CreateDefaultDataController();
		}
		internal void SetDataController(BaseGridController dataController) {
			this.dataController = dataController;
		}
		bool IsIPagedCollectionView {
			get {
#if SL
				return List is ICollectionViewHelper && ((ICollectionViewHelper)List).Collection is IPagedCollectionView;
#else
				return false;
#endif
			}
		}
		internal override void CancelAllGetRows() {
			if(IsAsyncServerMode) {
				DataController.ScrollingCancelAllGetRows();
			}
		}
		internal override void EnsureAllRowsLoaded(int firstRowIndex, int rowsCount) {
			if(IsAsyncServerMode) {
				for(int i = firstRowIndex; i < firstRowIndex + rowsCount; ++i) {
					object visibleIndex = VisibleIndicesProvider.GetVisibleIndexByScrollIndex(i);
					if(visibleIndex is GroupSummaryRowKey) continue;
					int rowHandle = DataController.GetControllerRowHandle((int)visibleIndex);
					DataController.ScrollingCheckRowLoaded(rowHandle);
				}
			}
		}
		protected override Type ItemTypeCore { get { return DataProviderBase.GetListItemPropertyType(DataController.ListSource, null); } }
		public override ISummaryItemOwner TotalSummaryCore { get { return Owner.TotalSummary; } }
		public override ISummaryItemOwner GroupSummaryCore { get { return GroupSummary; } }
		public override GridSummaryItemCollection GroupSummary { get { return Owner.GroupSummary; } }
		public override ValueComparer ValueComparer { get { return DataController.ValueComparer; } }
		public override CriteriaOperator FilterCriteria {
			get { return DataController.FilterCriteria; }
			set {
				if(object.ReferenceEquals(FilterCriteria, value))
					return;
				DataController.FilterCriteria = value;
			}
		}
		FilterHelper FilterHelper {
			get { return DataController.FilterHelper; }
		}
		internal override bool IsServerMode { get { return List is IListServer; } }
		internal override bool IsICollectionView { get { return List is DevExpress.Data.Linq.ICollectionViewHelper; } }
		internal override bool IsAsyncServerMode { get { return List is DevExpress.Data.Async.IAsyncListServer; } }
		internal override bool IsAsyncOperationInProgress {
			get { return isAsyncOperationInProgress; }
			set {
				isAsyncOperationInProgress = value;
				Owner.UpdateIsAsyncOperationInProgress(value);
			}
		}
		protected internal override bool AllowUpdateFocusedRowData {
			get { return !IsAsyncServerMode; }
		}
		public override bool IsUpdateLocked { get { return DataController.IsUpdateLocked; } }
		internal RowStateController RowStateController { get { return (RowStateController)DataController.Selection; } }
		protected
#if DEBUGTEST
 internal
#endif
 IList List { get { return list; } }
		bool IsRealBindingList { get { return List is IBindingList && !(List is IListWrapper); } }
		internal override IRefreshable RefreshableSource { get { return list as IRefreshable; } }
		internal override BindingListAdapterBase BindingListAdapter { get { return list as BindingListAdapterBase; } }
		internal override bool SubscribeRowChangedForVisibleRows {
			get {
				if(IsServerMode || IsAsyncServerMode)
					return true;
				if(Owner.AllowLiveDataShaping.HasValue)
					return !Owner.AllowLiveDataShaping.Value;
				return !IsRealBindingList;
			}
		}
		internal override bool IsSelfManagedItemsSource {
			get {
				return IsServerMode || IsAsyncServerMode;
			}
		}
		protected internal override void OnDataSourceChanged() {
			this.list = ExtractDataSource(DataSource);
			bool isUpdateLocked = DataController.IsUpdateLocked;
			ForceClearData();
			if(isUpdateLocked) {
				DataController.BeginUpdate();
			}
			if(VisualClient != null && List == null && !IsAsyncServerMode) VisualClient.RequestSynchronization();
			DataController.SetDataSource(List);
			Owner.ValidateMasterDetailConsistency();
			ScheduleRepopulateColumnsIfNeeded();
			ResetValidationAttributes();
		}
		protected DataSourceProvider Provider {
			get { return provider; }
			set {
				if(Provider == value)
					return;
				if(Provider != null)
					DataChangedEventManager.RemoveListener(Provider, this);
				provider = value;
				if(Provider != null)
					DataChangedEventManager.AddListener(Provider, this);
			}
		}
		void RefreshDataProvider() { 
			object ds = DataSource;
			DataSource = null;
			DataSource = ds;
		}
		bool IsLockInit { get { return lockInit != 0; } }
		int lockInit = 0;
		protected virtual IList ExtractDataSource(object dataSource) {
			if(IsLockInit)
				return null;
			this.lockInit++;
			try {
				Provider = dataSource as DataSourceProvider;
				DevExpress.Xpf.Core.Native.ItemPropertyNotificationMode notificationMode = DevExpress.Xpf.Core.Native.ItemPropertyNotificationMode.None;
				if(!SubscribeRowChangedForVisibleRows) {
					notificationMode |= DevExpress.Xpf.Core.Native.ItemPropertyNotificationMode.PropertyChanged;
					if(Owner.OptimizeSummaryCalculation)
						notificationMode |= DevExpress.Xpf.Core.Native.ItemPropertyNotificationMode.PropertyChanging;
				}   
				return DataBindingHelper.ExtractDataSource(dataSource, notificationMode);
			}
			finally {
				this.lockInit--;
			}
		}
		public IDataControllerVisualClient VisualClient {
			get { return visualClient; }
			private set {
				visualClient = value;
				if(DataController.VisualClient == value)
					return;
				DataController.VisualClient = value;
			}
		}
		IDispalyMemberBindingClient DisplayMemberBindingClient {
			get { return VisualClient as IDispalyMemberBindingClient; }
		}
		public virtual bool CanColumnSort(string fieldName) {
			return Owner.CanSortColumn(fieldName);
		}
		public override bool CanColumnSortCore(string fieldName) {
			return DataController.CanSortColumn(fieldName);
		}
		public override int GroupedColumnCount { get { return DataController.GroupedColumnCount; } }
#if DEBUGTEST
		internal int SyncFireCount { get; private set; }
#endif
		public override void Syncronize(IList<GridSortInfo> sortList, int groupCount, CriteriaOperator filterCriteria) {
#if DEBUGTEST
			SyncFireCount++;
#endif
			List<DataColumnSortInfo> infoList = new List<DataColumnSortInfo>();
			foreach(IColumnInfo columnInfo in sortList) {
				if(!CanColumnSort(columnInfo.FieldName))
					continue;
				DataColumnInfo colInfo = Columns[columnInfo.FieldName];
				if(colInfo != null)
					infoList.Add(new DataColumnSortInfo(colInfo, columnInfo.SortOrder, Owner.GetGroupInterval(columnInfo.FieldName)));
			}
			DataController.BeginUpdate();
			try {
				try {
					FilterCriteria = filterCriteria;
				}
				catch {
				}
				DataController.SortInfo.ClearAndAddRange(infoList.ToArray(), groupCount);
				GroupSummarySortInfo.Sync(sortList, groupCount);
				SynchronizeGroupSortInfo();
				SynchronizeSummary();
				DataController.SummarySortInfo.ClearAndAddRange(GetSummarySortInfo());
			}
			finally {
				DataController.EndUpdate();
			}
		}
		void SynchronizeGroupSortInfo() {
			int groupCount;
			IList<IColumnInfo> sortList = GetSortInfo(out groupCount);
			Owner.SynchronizeGroupSortInfo(sortList, groupCount);
		}
		public override void SynchronizeSummary() {
			List<SummaryItem> changedGroupSummaries = SynchronizeSummary(DataController.GroupSummary, GroupSummary);
			List<SummaryItem> changedTotalSummaries = SynchronizeSummary(DataController.TotalSummary, TotalSummaryCore.Concat(Owner.GetServiceSummaries()));
			UpdateGroupSummary(changedGroupSummaries);
			UpdateTotalSummary(changedTotalSummaries);
		}
		void UpdateGroupSummary(List<SummaryItem> changedGroupSummaries = null) {
			DataController.UpdateGroupSummary(changedGroupSummaries);
		}
		void UpdateTotalSummary(List<SummaryItem> changedTotalSummaries = null) {
			DataController.UpdateTotalSummary(changedTotalSummaries);
		}
		public override void UpdateGroupSummary() {
			DataController.UpdateGroupSummary(null);
		}
		public override void UpdateTotalSummary() {
			DataController.UpdateTotalSummary(null);
		}
		public List<IColumnInfo> GetSortInfo(out int groupCount) {
			DataControlBase dataControl = owner as DataControlBase;
			List<IColumnInfo> res = new List<IColumnInfo>();
			if(dataControl != null && dataControl.ActualItemsSource == null) {
				groupCount = dataControl.SortInfoCore.GroupCountCore;
				foreach(GridSortInfo info in dataControl.SortInfoCore) {
					res.Add(new DummyColumnInfo(((IColumnInfo)info).FieldName, ((IColumnInfo)info).SortOrder));
				}
			}
			else {
				groupCount = DataController.SortInfo.GroupCount;
				foreach(DataColumnSortInfo info in DataController.SortInfo) {
					res.Add(new DummyColumnInfo(info.ColumnInfo.Name, info.SortOrder));
				}
			}
			return res;
		}
		protected SummarySortInfo[] GetSummarySortInfo() {
			List<SummarySortInfo> items = new List<SummarySortInfo>();
			foreach(GridGroupSummarySortInfo info in GroupSummarySortInfo) {
				if(info.SummaryItem == null || info.SummaryItem.SummaryType == SummaryItemType.None) continue;
				int summaryIndex = GroupSummary.IndexOf(info.SummaryItem);
				if(summaryIndex < 0 || summaryIndex >= DataController.GroupSummary.Count) continue;
				SummaryItem summary = DataController.GroupSummary[summaryIndex];
				int groupLevel = GetGroupIndex(info.FieldName);
				if(groupLevel < 0) continue;
				items.Add(new SummarySortInfo(summary, groupLevel, info.GetSortOrder()));
			}
			return items.ToArray();
		}
		public override int VisibleCount { get { return DataController.VisibleCount; } }
		public override int DataRowCount { get { return DataController.VisibleListSourceRowCount; } }
		public override DataColumnInfoCollection Columns { get { return DataController.Columns; } }
		public override DataColumnInfoCollection DetailColumns { get { return DataController.DetailColumns; } }
		public override bool IsReady { get { return DataController.IsReady; } }
		public override bool IsCurrentRowEditing { get { return DataController.IsCurrentRowEditing; } }
		public override void BeginUpdate() {
			DataController.BeginUpdate();
		}
		public override void EndUpdate() {
			DataController.EndUpdate();
			UpdateDisplayMemberBindingColumnsIfNeeded();
		}
		public override void CancelCurrentRowEdit() {
			DataController.CancelCurrentRowEdit();
		}
		public override void BeginCurrentRowEdit() {
			DataController.BeginCurrentRowEdit();
		}
		public override bool EndCurrentRowEdit() {
			if(IsCurrentRowEditing)
				return DataController.EndCurrentRowEdit();
			return true;
		}
		public override ErrorInfo GetErrorInfo(RowHandle rowHandle) {
			return DataController.GetErrorInfo(rowHandle.Value);
		}
		public override void ExpandAll() {
			DataController.ExpandAll();
		}
		public override void CollapseAll() {
			DataController.CollapseAll();
		}
		public override int GetListIndexByRowHandle(int rowHandle) {
			return DataController.GetListSourceRowIndex(rowHandle);
		}
		public override RowDetailContainer GetRowDetailContainer(int rowHandle, Func<RowDetailContainer> createContainerDelegate, bool createNewIfNotExist) {
			return IsGroupRowHandle(rowHandle) ? null : RowStateController.GetRowDetailInfo(rowHandle, createContainerDelegate, createNewIfNotExist);
		}
		public override IEnumerable<int> GetRowListIndicesWithExpandedDetails() {
			return RowStateController.GetRowListIndicesWithExpandedDetails();
		}
		public override void ClearDetailInfo() {
			RowStateController.ClearDetailInfo();
		}
		public override DependencyObject GetRowState(int controllerRow, bool createNewIfNotExist) {
			return RowStateController.GetRowState(controllerRow, createNewIfNotExist);
		}
#if DEBUGTEST
		internal static int GetRowHandleByListIndexCallCount;
#endif
		public override int GetRowHandleByListIndex(int listIndex) {
#if DEBUGTEST
			GetRowHandleByListIndexCallCount++;
#endif
			return DataController.GetControllerRow(listIndex);
		}
		public override int GetControllerRow(int visibleIndex) {
			return DataController.GetControllerRowHandle(visibleIndex);
		}
		public override bool IsGroupRowHandle(int rowHandle) {
			if(rowHandle == GridControl.AutoFilterRowHandle) {
				return false;
			}
			return DataController.IsGroupRowHandle(rowHandle);
		}
		public override bool IsValidRowHandle(int rowHandle) {
			return DataController.IsValidControllerRowHandle(rowHandle);
		}
		public override bool IsRowVisible(int rowHandle) {
			if(Owner.NewItemRowPosition == NewItemRowPosition.Bottom && rowHandle == GridControl.NewItemRowHandle)
				return true;
			return DataController.IsRowVisible(rowHandle);
		}
		public override int GetParentRowHandle(int rowHandle) {
			return DataController.GetParentRowHandle(rowHandle);
		}
		public override int GetChildRowCount(int rowHandle) {
			return DataController.GroupInfo.GetChildCount(rowHandle);
		}
		public override int GetChildRowHandle(int rowHandle, int childIndex) {
			return DataController.GroupInfo.GetChildRow(rowHandle, childIndex);
		}
		public object GetWpfRowByListIndex(int listIndex) {
			return new RowTypeDescriptorListIndex(selfReference, listIndex);
		}
		public override bool IsGroupRow(int visibleIndex) { return IsGroupRowHandle(GetControllerRow(visibleIndex)); }
		public override object GetGroupRowValue(int rowHandle) {
			return DataController.GetGroupRowValue(rowHandle);
		}
		public override object GetGroupRowValue(int rowHandle, ColumnBase column) {
			if(column == null) {
				return GetGroupRowValue(rowHandle);
			}
			DataColumnInfo ci = this.DataController.Columns[column.FieldName];
			return DataController.GetGroupRowValue(rowHandle, ci);
		}
		public override bool IsGroupRowExpanded(int controllerRow) {
			return DataController.IsRowExpanded(controllerRow);
		}
		public override int GetRowLevelByControllerRow(int rowHandle) {
			return DataController.GetRowLevel(rowHandle);
		}
		public override int GetActualRowLevel(int rowHandle, int level) {
			if(!DataController.AllowPartialGrouping || IsGroupRowHandle(rowHandle))
				return level;
			int parentRowHandle = GetParentRowHandle(rowHandle);
			while(parentRowHandle != DataControlBase.InvalidRowHandle) {
				if(GetChildRowCount(parentRowHandle) == 1)
					level--;
				else
					return level;
				parentRowHandle = GetParentRowHandle(parentRowHandle);
			}
			return level;
		}
		public override void ChangeGroupExpanded(int controllerRow, bool recursive) {
			if(IsGroupRowExpanded(controllerRow))
				DataController.CollapseRow(controllerRow, recursive);
			else
				DataController.ExpandRow(controllerRow, recursive);
		}
		public override void MakeRowVisible(int rowHandle) {
			DataController.MakeRowVisible(rowHandle);
		}
		public override object GetTotalSummaryValue(DevExpress.Xpf.Grid.SummaryItemBase item) {
			if(item == null)
				return null;
			SummaryItem dcItem = DataController.TotalSummary.GetSummaryItemByKey(item);
			if(dcItem == null)
				return null;
			return dcItem.SummaryValue;
		}
		public override bool TryGetGroupSummaryValue(int rowHandle, DevExpress.Xpf.Grid.SummaryItemBase item, out object value) {
			value = null;
			if(item == null)
				return false;
			IDictionary summary = DataController.GetGroupSummary(rowHandle);
			if(summary == null || !summary.Contains(item))
				return false;
			value = summary[item];
			return true;
		}
		public override object GetRowByListIndex(int listSourceRowIndex) {
			return GetRowByListIndex(DataController, listSourceRowIndex);
		}
		public override object GetCellValueByListIndex(int listSourceRowIndex, string fieldName) {
			return GetRowValueByListIndex(listSourceRowIndex, fieldName);
		}
		public object GetListSourceRowValue(int listSourceRowIndex, string fieldName) {
			return DataController.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
		int GetColumnHandle(ColumnBase column) {
			DataColumnInfo columnInfo = Columns[column.FieldName];
			return columnInfo == null ? -1 : columnInfo.Index;
		}
		public override object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime, bool implyNullLikeEmptyStringWhenFiltering) {
			return GetUniqueColumnValues(column, includeFilteredOut, roundDataTime, null, implyNullLikeEmptyStringWhenFiltering);
		}
		public object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime, OperationCompleted asyncCompleted, bool implyNullLikeEmptyStringWhenFiltering) {
			if(IsAsyncServerMode) {
				if(asyncCompleted == null) {
					asyncCompleted = delegate(object valuesObject) {
						object[] values = valuesObject as object[];
						OnAsyncGetColumnValuesCompleted(column, values);
					};
				}
			}
			return FilterHelper.GetUniqueColumnValues(GetColumnHandle(column), -1, includeFilteredOut, roundDataTime, asyncCompleted, implyNullLikeEmptyStringWhenFiltering);
		}
		void OnAsyncGetColumnValuesCompleted(ColumnBase column, object[] values) {
			column.ColumnFilterInfo.UpdateCurrentPopupData(values);
		}
		public override CriteriaOperator CalcColumnFilterCriteriaByValue(ColumnBase column, object columnValue) {
			return FilterHelper.CalcColumnFilterCriteriaByValue(GetColumnHandle(column), columnValue, true, column.RoundDateTimeForColumnFilter, null);
		}
		protected virtual void OnCurrentChanged() {
			Owner.OnCurrentIndexChanged();
		}
		protected virtual void OnCurrentRowChanged() {
			Owner.OnCurrentRowChanged();
		}
		public override bool AutoExpandAllGroups {
			get { return DataController.AutoExpandAllGroups; }
			set { DataController.AutoExpandAllGroups = value; }
		}
		public override int CurrentControllerRow {
			get { return DataController.CurrentControllerRow; }
			set { DataController.CurrentControllerRow = value; }
		}
		public override int GetControllerRowByGroupRow(int groupRowHandle) {
			return dataController.GetControllerRowByGroupRow(groupRowHandle);
		}
		public override int GetRowVisibleIndexByHandle(int controllerRow) {
			if(Owner.NewItemRowPosition == NewItemRowPosition.Bottom && controllerRow == GridControl.NewItemRowHandle)
				return VisibleCount;
			return DataController.GetVisibleIndexChecked(controllerRow);
		}
		public override int CurrentIndex {
			get { return GetRowVisibleIndexByHandle(CurrentControllerRow); }
		}
		public int GetLastVisibleChild(int visibleIndex) {
			int controllerRow = GetControllerRow(visibleIndex);
			return visibleIndex + GetVisibleRowCount(controllerRow);
		}
		public override void DoRefresh() {
			DataController.DoRefresh();
		}
		public override void RefreshRow(int rowHandle) {
			DataController.RefreshRow(rowHandle);
		}
		public override int FindRowByRowValue(object value) {
#if DEBUGTEST
			FindRowByRowValueCallCount++;
#endif
			return DataController.FindRowByRowValue(value);
		}
		public override int FindRowByValue(string fieldName, object value) {
			return DataController.FindRowByValue(fieldName, value);
		}
		internal override int GetGroupIndex(string fieldName) {
			return DataController.SortInfo.GetGroupIndex(Columns[fieldName]);
		}
		internal override void EnsureRowLoaded(int rowHandle) {
			if(IsAsyncServerMode) {
				DataController.ScrollingCheckRowLoaded(rowHandle);
			}
		}
		int GetVisibleRowCount(int groupRowHandle) {
			int rowCount = 0;
			GroupRowInfo groupRow = DataController.GroupInfo.GetGroupRowInfoByHandle(groupRowHandle);
			if(groupRow == null)
				return 0;
			if(!groupRow.Expanded)
				return 0;
			if(DataController.GroupInfo.IsLastLevel(groupRow))
				return groupRow.ChildControllerRowCount;
			int totalChildrenCount = DataController.GroupInfo.GetTotalChildrenGroupCount(groupRow);
			for(int n = 0; n < totalChildrenCount; n++) {
				GroupRowInfo group = DataController.GroupInfo[groupRow.Index + n + 1];
				if(group.IsVisible)
					rowCount++;
				if(!group.Expanded)
					continue;
				if(DataController.GroupInfo.IsLastLevel(group))
					rowCount += group.ChildControllerRowCount;
			}
			return rowCount;
		}
		List<SummaryItem> SynchronizeSummary(SummaryItemCollection controllerSummaries, IEnumerable<DevExpress.Xpf.Grid.SummaryItemBase> gridSummaries) {
			if(gridSummaries == null) {
				return Enumerable.Empty<SummaryItem>().ToList();
			}
			controllerSummaries.BeginUpdate();
			try {
				RemoveNonVisibleSummariesFromDataController(controllerSummaries);
				RemoveDeletedGridSummariesFromDataController(gridSummaries, controllerSummaries);
				var changedSummaries = new List<SummaryItem>();
				var visibleGridSummaries = gridSummaries.Where(x => x.Visible).ToArray();
				var summariesInDataControllerToUpdate = visibleGridSummaries.Where(x => { 
					return x.IsInDataController(controllerSummaries) 
						&& x.SummaryType != SummaryItemType.None 
						&& !x.EqualsToControllerSummaryItem(controllerSummaries.GetSummaryItemByTag(x)); 
				});
				List<SummaryItem> updatedControllerSummaries = UpdateControllerSummaryItems(summariesInDataControllerToUpdate, controllerSummaries);
				changedSummaries.AddRange(updatedControllerSummaries);
				SummaryItem[] newControllerSummaryItems = visibleGridSummaries
					.Where(x => !x.IsInDataController(controllerSummaries))
					.Select(x => {
						DataColumnInfo column = !string.IsNullOrEmpty(x.FieldName) ? Columns[x.FieldName] : null;
						return new SummaryItem(column, x.SummaryType, x, x.IgnoreNullValues);
					}).ToArray();
				controllerSummaries.AddRange(newControllerSummaryItems);
				changedSummaries.AddRange(newControllerSummaryItems);
				return changedSummaries;
			}
			finally {
				controllerSummaries.CancelUpdate();
			}
		}
		void RemoveNonVisibleSummariesFromDataController(SummaryItemCollection controllerSummaries) {
			for(int i = controllerSummaries.Count - 1; i >= 0; i--) {
				if(!(controllerSummaries[i].Tag as DevExpress.Xpf.Grid.SummaryItemBase).Visible) {
					controllerSummaries.RemoveAt(i);
				}
			}
		}
		void RemoveDeletedGridSummariesFromDataController(IEnumerable<DevExpress.Xpf.Grid.SummaryItemBase> gridSummaries, SummaryItemCollection controllerSummaries) {
			for(int i = controllerSummaries.Count - 1; i >= 0; i--) {
				if(!gridSummaries.Any(gridSummaryItem => gridSummaryItem == controllerSummaries[i].Tag)) {
					controllerSummaries.RemoveAt(i);
				}
			}
		}
		List<SummaryItem> UpdateControllerSummaryItems(IEnumerable<DevExpress.Xpf.Grid.SummaryItemBase> gridSummaries, SummaryItemCollection controllerSummaries) {
			var updated = new List<SummaryItem>();
			foreach(var gridSummaryItem in gridSummaries) {
				SummaryItem controllerSummaryItem = controllerSummaries.GetSummaryItemByTag(gridSummaryItem);
				UpdateControllerSummaryItem(controllerSummaryItem, gridSummaryItem);
				updated.Add(controllerSummaryItem);
			}
			return updated;
		}
		void UpdateControllerSummaryItem(SummaryItem controllerSummaryItem, DevExpress.Xpf.Grid.SummaryItemBase gridSummaryItem) {
			controllerSummaryItem.FieldName = gridSummaryItem.FieldName;
			controllerSummaryItem.SummaryType = gridSummaryItem.SummaryType;
		}
		internal void DisplayMemberBindingInitialize() {
			if(DisplayMemberBindingClient != null)
				DisplayMemberBindingClient.UpdateSimpleBinding();
			if(DataRowCount > 0) {
				if(DisplayMemberBindingClient != null)
					DisplayMemberBindingClient.UpdateColumns();
			}
			else
				isDisplayMemberBindingInitialized = false;
		}
		#region IDataControllerCurrentSupport Members
		void IDataControllerCurrentSupport.OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			OnCurrentChanged();
		}
		void IDataControllerCurrentSupport.OnCurrentControllerRowObjectChanged(DevExpress.Data.CurrentRowChangedEventArgs e) {
			OnCurrentRowChanged();
		}
		#endregion
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(DataChangedEventManager)) {
				RefreshDataProvider();
				return true;
			}
			return false;
		}
		#endregion
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return GetUnboundColumnsCore(Owner.GetColumns().Concat(Owner.GetServiceUnboundColumns()));
		}
		object IDataControllerData.GetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			if(Events == null)
				return value;
			return Events.GetUnboundData(listSourceRow, column.Name, value);
		}
		void IDataControllerData.SetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			if(Events != null)
				Events.SetUnboundData(listSourceRow, column.Name, value);
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties { get { return !Owner.IsDesignTime; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) {
			Events.SubstituteFilter(args);
		}
		bool IDataControllerData2.HasUserFilter { get { return Owner.HasCustomRowFilter(); } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) { return Events.OnCustomRowFilter(listSourceRow, fit); }
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			List<IColumnInfo> columns = Owner.GetColumns();
			if(columns == null || collection == null)
				return collection;
			foreach(IColumnInfo col in columns) {
				if(collection.Find(col.FieldName, false) == null)
					collection.Find(col.FieldName, true);
			}
			return collection;
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			foreach(IColumnInfo column in Owner.GetColumns()) {
				if(column.UnboundType != UnboundColumnType.Bound) continue;
				if(column.FieldName.Contains(".") && DataController.Columns[column.FieldName] == null)
					res.Add(column.FieldName);
			}
			return res;
		}
		#endregion
		#region IDataControllerSort Members
		string[] IDataControllerSort.GetFindByPropertyNames() {
			if(IsServerMode || Owner == null)
				return new string[0];
			return Owner.GetFindToColumnNames();
		}
		string IDataControllerSort.GetDisplayText(int listSourceIndex, DataColumnInfo column, object value, string columnName) { return Owner.GetDisplayText(listSourceIndex, column, value, columnName); }
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) { return Owner.RequireDisplayText(column); }
		ExpressiveSortInfo.Row IDataControllerSort.GetCompareRowsMethodInfo() {
			return null;
		}
		bool? IDataControllerSort.IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo column) {
			if(!Owner.RequireSortCell(column))
				return null;
			var cmp = Events.OnCompareGroupValues(listSourceRow1, listSourceRow2, value1, value2, column);
			if(cmp.HasValue)
				return cmp.Value == 0;
			return null;
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) {
			if(!Owner.RequireSortCell(dataColumnInfo))
				return null;
			return Events.GetSortGroupCellMethodInfo(dataColumnInfo, baseExtractorType);
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			if(!Owner.RequireSortCell(dataColumnInfo))
				return null;
			return Events.GetSortCellMethodInfo(dataColumnInfo, baseExtractorType, order);
		}
		void IDataControllerSort.BeforeGrouping() { Events.OnBeforeGrouping(); }
		void IDataControllerSort.AfterGrouping() { Events.OnAfterGrouping(); }
		void IDataControllerSort.BeforeSorting() { Events.OnBeforeSorting(); }
		void IDataControllerSort.AfterSorting() { Events.OnAfterSorting(); }
		void IDataControllerSort.SubstituteSortInfo(SubstituteSortInfoEventArgs args) {
			Events.SubstituteSortInfo(args);
		}
		#endregion
		internal override DataColumnInfo GetActualColumnInfo(string fieldName) {
			return GetActualColumnInfo(DataController, fieldName);
		}
		public object GetRowValueByListIndex(int listIndex, string fieldName) {
			return GetRowValueByListIndex(DataController, listIndex, fieldName);
		}
		public object GetRowValueByListIndex(int listIndex, DataColumnInfo info) {
			return GetRowValueByListIndex(DataController, listIndex, info);
		}
		public override object GetRowValue(int rowHandle) {
			return DataController.GetRow(rowHandle);
		}
		public override object GetRowValue(int rowHandle, string fieldName) {
			return GetRowValue(rowHandle, GetActualColumnInfo(fieldName));
		}
		public override object GetRowValue(int rowHandle, DataColumnInfo info) {
			return IsGroupRowHandle(rowHandle) ?
				DataController.GetGroupRowValue(rowHandle, info) :
				DataController.GetRowValue(rowHandle, info);
		}
		public override void SetRowValue(RowHandle rowHandle, DataColumnInfo info, object value) {
			DataController.SetRowValue(rowHandle.Value, info, value);
		}
		public override bool AllowEdit { get { return DataController.AllowEdit; } }
		#region IDataControllerValidationSupport Members
		IBoundControl IDataControllerValidationSupport.BoundControl { get { throw new NotImplementedException(); } }
		void IDataControllerValidationSupport.OnEndNewItemRow() { Owner.OnEndNewItemRow(); }
		void IDataControllerValidationSupport.OnStartNewItemRow() {
			if(!DataController.Columns.HasUnboundColumns) {
				TryRePopulateColumns();
			}
			Owner.OnStartNewItemRow();
		}
		void IDataControllerValidationSupport.OnBeginCurrentRowEdit() { }
		void IDataControllerValidationSupport.OnControllerItemChanged(ListChangedEventArgs e) {
			UpdateDisplayMemberBindingColumnsIfNeeded();
			TryRePopulateColumns();
			Owner.OnItemChanged(e);
		}
		void UpdateDisplayMemberBindingColumnsIfNeeded() {
			if(!isDisplayMemberBindingInitialized && DisplayMemberBindingClient != null && DataRowCount > 0) {
				DisplayMemberBindingClient.UpdateColumns();
				isDisplayMemberBindingInitialized = true;
			}
		}
		void IDataControllerValidationSupport.OnCurrentRowUpdated(ControllerRowEventArgs e) {
			Owner.RaiseCurrentRowUpdated(e);
		}
		void IDataControllerValidationSupport.OnPostCellException(ControllerRowCellExceptionEventArgs e) {
			throw e.Exception;
		}
		void IDataControllerValidationSupport.OnPostRowException(ControllerRowExceptionEventArgs e) {
			Owner.OnPostRowException(e);
		}
		void IDataControllerValidationSupport.OnValidatingCurrentRow(ValidateControllerRowEventArgs e) {
			Owner.RaiseValidatingCurrentRow(e);
		}
		#endregion
		public virtual RowPosition GetRowPosition(int visibleIndex) {
			RowPosition rowPosition = RowPosition.Middle;
			if(GetRowLevelByVisibleIndex(visibleIndex) == 0) {
				rowPosition = GetRowPosition(visibleIndex, 0, VisibleCount - 1);
			}
			else {
				GroupRowInfo groupRow = DataController.GetParentGroupRow(GetControllerRow(visibleIndex));
				int groupRowVisibleIndex = DataController.GetVisibleIndex(groupRow.ChildControllerRow);
				rowPosition = GetRowPosition(visibleIndex, groupRowVisibleIndex, groupRowVisibleIndex + groupRow.ChildControllerRowCount - 1);
			}
			return GetRowPositionByCheckingPrevRow(visibleIndex, rowPosition);
		}
		protected RowPosition GetRowPositionByCheckingPrevRow(int visibleIndex, RowPosition rowPosition) {
			int level = GetRowLevelByVisibleIndex(visibleIndex);
			if(rowPosition == RowPosition.Middle && level < DataController.GroupInfo.LevelCount) {
				if(GetRowLevelByVisibleIndex(visibleIndex - 1) > level)
					return RowPosition.Top;
			}
			return rowPosition;
		}
		protected RowPosition GetRowPosition(int visibleIndex, int startLevelIndex, int endLevelIndex) {
			if(startLevelIndex == endLevelIndex)
				return RowPosition.Single;
			if(visibleIndex == startLevelIndex)
				return RowPosition.Top;
			if(visibleIndex == endLevelIndex)
				return RowPosition.Bottom;
			return RowPosition.Middle;
		}
		public virtual void AddNewRow() {
			DataController.AddNewRow();
			if(List != null && List.Count == 0 && CollectionViewSource != null)
				RePopulateColumns();
		}
		public bool IsNewItemRowEditing { get { return DataController.IsNewItemRowEditing; } }
		public override void DeleteRow(RowHandle rowHandle) {
			DataController.DeleteRow(rowHandle.Value);
		}
		internal override void ScheduleAutoPopulateColumns() {
			autoPopulateColumns = true;
		}
		void ScheduleRepopulateColumnsIfNeeded() {
			if(IsDataSourceEmpty()) {
				SetRepopulateColumnsConditions(true);
			}
		}
		void TryRePopulateColumns() {
			if(!isRepopulateColumnsNeeded) return;
			if(IsDataSourceEmpty() && !IsAsyncServerMode) return;
			bool repopulateGridColumns = autoPopulateColumns && ShouldTryRepopulateColumns();
			Owner.RePopulateDataControllerColumns();
			if(repopulateGridColumns)
				Owner.PopulateColumns();
			SetRepopulateColumnsConditions(false);
		}
		IEditableCollectionView EditableView {
			get {
				if(List is ISupportEditableCollectionView)
					return ((ISupportEditableCollectionView)List).IsSupportEditableCollectionView ? (IEditableCollectionView)List : null;
				return List as IEditableCollectionView;
			}
		}
		bool IsDataSourceEmpty() {
			if(List != null && List.Count == 0 && EditableView != null && EditableView.IsAddingNew)
				return false;
			return List != null && List.Count == 0;
		}
		void SetRepopulateColumnsConditions(bool value) {
			if(value) {
				isRepopulateColumnsNeeded = true;
				isDummySource = false;
				if(Columns.Count == 1) {
					Type propertyDescriptorType = Columns[0].PropertyDescriptor.GetType();
					foreach(string dummyPropertyDescriptor in DummyPropertyDescriptors) {
						if(propertyDescriptorType.Name == dummyPropertyDescriptor) {
							isDummySource = true;
							break;
						}
					}
				}
			}
			else {
				isRepopulateColumnsNeeded = false;
				isDummySource = false;
			}
		}
		bool ShouldTryRepopulateColumns() {
			IList<IColumnInfo> ownerColumns = Owner.GetColumns();
			return ownerColumns.Count == 0 || isDummySource;
		}
		internal override int ConvertVisibleIndexToScrollIndex(int visibleIndex, bool allowFixedGroups) {
			int scrollIndex = 0;
			if(Owner.NewItemRowPosition == NewItemRowPosition.Bottom && visibleIndex == VisibleCount)
				scrollIndex = visibleIndex;
			else
				scrollIndex = DataController.GetVisibleIndexes().ConvertIndexToScrollIndex(visibleIndex, allowFixedGroups);
			return scrollIndex + VisibleIndicesProvider.CalcVisibleSummaryRowsCountBeforeRow(visibleIndex, allowFixedGroups);
		}
		internal override int ConvertScrollIndexToVisibleIndex(int scrollIndex, bool allowFixedGroups) {
			scrollIndex = VisibleIndicesProvider.GetVisibleIndexByScrollIndexSafe(scrollIndex);
			return DataController.GetVisibleIndexes().ConvertScrollIndexToIndex(scrollIndex, allowFixedGroups);
		}
		public override object GetVisibleIndexByScrollIndex(int scrollIndex) {
			return VisibleIndicesProvider.GetVisibleIndexByScrollIndex(scrollIndex);
		}
	}
	public class EmptyDataProvider : DataProviderBase {
		GridSummaryItemCollection totalSummary = new GridSummaryItemCollection(null, SummaryItemCollectionType.Total);
		DataColumnInfoCollection columns = new DataColumnInfoCollection();
		DataColumnInfoCollection detailColumns = new DataColumnInfoCollection();
		public override bool AutoExpandAllGroups { get { return false; } set { } }
		public override void BeginCurrentRowEdit() { }
		public override void BeginUpdate() { }
		public override CriteriaOperator CalcColumnFilterCriteriaByValue(ColumnBase column, object columnValue) { return null; }
		public override bool CanColumnSortCore(string fieldName) { return false; }
		internal override void CancelAllGetRows() { }
		public override void CancelCurrentRowEdit() { }
		public override void ChangeGroupExpanded(int controllerRow, bool recursive) { }
		public override void CollapseAll() { }
		public override DataColumnInfoCollection Columns { get { return columns; } }
		public override int CurrentControllerRow { get { return DataControlBase.InvalidRowHandle; } set { } }
		public override int CurrentIndex { get { return 0; } }
		protected internal override BaseGridController DataController { get { return null; } }
		public override int DataRowCount { get { return 0; } }
		protected internal override void OnDataSourceChanged() { }
		public override void DeleteRow(RowHandle rowHandle) { }
		public override DataColumnInfoCollection DetailColumns { get { return detailColumns; } }
		public override void DoRefresh() { }
		public override bool EndCurrentRowEdit() { return true; }
		public override void EndUpdate() { }
		internal override void EnsureAllRowsLoaded(int firstRowIndex, int rowsCount) { }
		public override void ExpandAll() { }
		public override CriteriaOperator FilterCriteria { get { return null; } set { } }
		public override int FindRowByRowValue(object value) { return -1; }
		public override int FindRowByValue(string fieldName, object value) { return -1; }
		protected internal override void ForceClearData() { }
		internal override DataColumnInfo GetActualColumnInfo(string fieldName) { return null; }
		public override int GetChildRowCount(int rowHandle) { return 0; }
		public override int GetChildRowHandle(int rowHandle, int childIndex) { return DataControlBase.InvalidRowHandle; }
		public override int GetControllerRow(int visibleIndex) { return DataControlBase.InvalidRowHandle; }
		public override int GetControllerRowByGroupRow(int groupRowHandle) { return DataControlBase.InvalidRowHandle; }
		public override ErrorInfo GetErrorInfo(RowHandle rowHandle) { return null; }
		internal override int GetGroupIndex(string fieldName) { return 0; }
		public override object GetGroupRowValue(int rowHandle, ColumnBase column) { return null; }
		public override object GetGroupRowValue(int rowHandle) { return null; }
		public override bool TryGetGroupSummaryValue(int rowHandle, Grid.SummaryItemBase item, out object value) {
			value = null;
			return false; 
		}
		public override int GetParentRowHandle(int rowHandle) { return DataControlBase.InvalidRowHandle; }
		public override object GetRowByListIndex(int listSourceRowIndex) { return null; }
		public override object GetCellValueByListIndex(int listSourceRowIndex, string fieldName) { return null; }
		public override int GetRowHandleByListIndex(int listIndex) { return DataControlBase.InvalidRowHandle; }
		public override int GetRowLevelByControllerRow(int rowHandle) { return 0; }
		public override int GetActualRowLevel(int rowHandle, int level) { return level; }
		public override int GetListIndexByRowHandle(int rowHandle) { return 0; }
		public override DependencyObject GetRowState(int controllerRow, bool createNewIfNotExist) { return null; }
		public override object GetRowValue(int rowHandle) { return null; }
		public override object GetRowValue(int rowHandle, DataColumnInfo info) { return null; }
		public override object GetRowValue(int rowHandle, string fieldName) { return null; }
		public override int GetRowVisibleIndexByHandle(int rowHandle) { return 0; }
		public override object GetTotalSummaryValue(Grid.SummaryItemBase item) { return null; }
		public override object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime, bool implyNullLikeEmptyStringWhenFiltering) { return null; }
		public override int GroupedColumnCount { get { return 0; } }
		public override bool AllowEdit { get { return false; } }
		public override bool IsCurrentRowEditing { get { return false; } }
		public override bool IsGroupRow(int visibleIndex) { return false; }
		public override bool IsGroupRowExpanded(int controllerRow) { return false; }
		public override bool IsGroupRowHandle(int rowHandle) { return false; }
		public override bool IsReady { get { return false; } }
		public override bool IsRowVisible(int rowHandle) { return false; }
		internal override bool IsServerMode { get { return false; } }
		internal override bool IsICollectionView { get { return false; } }
		internal override bool IsAsyncServerMode { get { return false; } }
		internal override bool IsAsyncOperationInProgress { get { return false; } set { } }
		public override bool IsUpdateLocked { get { return false; } }
		public override bool IsValidRowHandle(int rowHandle) { return false; }
		public override void MakeRowVisible(int rowHandle) { }
		public override void RefreshRow(int rowHandle) { }
		internal override void ScheduleAutoPopulateColumns() { }
		public override ISelectionController Selection { get { return null; } }
		public override void SetRowValue(RowHandle rowHandle, DataColumnInfo info, object value) { }
		public override void SynchronizeSummary() { }
		public override void Syncronize(IList<GridSortInfo> sortList, int groupCount, CriteriaOperator filterCriteria) { }
		public override ISummaryItemOwner TotalSummaryCore { get { return totalSummary; } }
		public override ISummaryItemOwner GroupSummaryCore { get { return null; } }
		public override void UpdateGroupSummary() { }
		public override void UpdateTotalSummary() { }
		public override int VisibleCount { get { return 0; } }
		internal override void EnsureRowLoaded(int rowHandle) { }
		public override RowDetailContainer GetRowDetailContainer(int controllerRow, Func<RowDetailContainer> createContainerDelegate, bool createNewIfNotExist) {
			throw new NotImplementedException(); 
		}
		public override void RePopulateColumns() { }
		protected override Type ItemTypeCore { get { return null; } }
		public static ValueComparer DefaultValueComparer = new ValueComparer();
		public override ValueComparer ValueComparer { get { return DefaultValueComparer; } }
	}
	public class VisibleIndicesProvider {
		const int DefaultMaxCachedRowCount = 50, DefaultCachedRowsIncrement = 20;
		Dictionary<int, object> visibleIndexToScrollIndexMap = new Dictionary<int, object>();
		Dictionary<int, int> groupRowHandleToSummaryRowCountMap = new Dictionary<int, int>();
		bool isValid = false;
		int visibleIndex = 0;
		int index = 0;
		int currentSummaryRowCount = 0;
		protected int maxCachedRowCount;
		int cachedVisibleGroupSummaryRowCount = -1;
		public VisibleIndicesProvider(GridDataProvider dataProvider) {
			DataProvider = dataProvider;
			ShowGroupSummaryFooterInternal = true;
		}
		protected GridDataProvider DataProvider { get; private set; }
		protected bool IsBottomNewItemRowVisible { get { return DataProvider.Owner.NewItemRowPosition == NewItemRowPosition.Bottom; } }
		public bool IsCacheEmpty { get { return CachedRowCount == 0; } }
		public int CachedRowCount { get { return visibleIndexToScrollIndexMap.Count; } }
		protected bool ShowGroupSummaryFooter { get { return DataProvider.Owner.ShowGroupSummaryFooter && ShowGroupSummaryFooterInternal; } }
		public int VisibleGroupSummaryRowCount { 
			get {
				if(DataProvider.IsServerMode || DataProvider.IsAsyncServerMode) 
					return currentSummaryRowCount;
				ValidateVisibleGroupSummaryRowCountCache();
				return cachedVisibleGroupSummaryRowCount;
			} 
		}
		public void InvalidateCache() {
			this.isValid = false;
			cachedVisibleGroupSummaryRowCount = -1;
			visibleIndexToScrollIndexMap.Clear();
			groupRowHandleToSummaryRowCountMap.Clear();
		}
		protected void ValidateVisibleGroupSummaryRowCountCache() {
			if(cachedVisibleGroupSummaryRowCount >= 0) return;
			cachedVisibleGroupSummaryRowCount = DataProvider.DataController.GroupInfo.Count<GroupRowInfo>(info => {
				return info.IsVisible && (!DataProvider.DataController.AllowPartialGrouping || DataProvider.GetChildRowCount(info.Handle) > 1) && DataProvider.Events.OnShowingGroupFooter(info.Handle, info.Level);
			});
		}
		public virtual object GetVisibleIndexByScrollIndex(int scrollIndex) {
			if(!ShowGroupSummaryFooter) return scrollIndex;
			ValidateCache();
			if(IsCacheEmpty) return scrollIndex;
			UpdateMaxCacheRowCountIfNeeded(scrollIndex);
			if(scrollIndex >= CachedRowCount) return scrollIndex;
			return visibleIndexToScrollIndexMap[scrollIndex];
		}
		public virtual int GetVisibleIndexByScrollIndexSafe(int scrollIndex) {
			if(!ShowGroupSummaryFooter) return scrollIndex;
			ValidateCache();
			if(IsCacheEmpty) return scrollIndex;
			UpdateMaxCacheRowCountIfNeeded(scrollIndex);
			if(scrollIndex >= CachedRowCount) return scrollIndex;
			for(int i = scrollIndex; i >= 0; i--) {
				object current = visibleIndexToScrollIndexMap[i];
				if(current is int)
					return (int)current;
			}
			return scrollIndex;
		}
		protected void ValidateCache() {
			if(this.isValid) return;
			this.visibleIndex = this.index = 0;
			this.currentSummaryRowCount = 0;
			this.maxCachedRowCount = DefaultMaxCachedRowCount;
			BuildCache();
			this.isValid = true;
		}
		protected void UpdateMaxCacheRowCountIfNeeded(int requestedScrollIndex) {
			if(requestedScrollIndex > maxCachedRowCount) {
				maxCachedRowCount = requestedScrollIndex + DefaultCachedRowsIncrement;
				BuildCache();
			}
		}
		bool IsSingleRow(int rowHandle) {
			if(!DataProvider.DataController.AllowPartialGrouping)
				return false;
			int parentRowHandle = DataProvider.GetParentRowHandle(rowHandle);
			return DataProvider.GetChildRowCount(parentRowHandle) == 1;
		}
		protected virtual void BuildCache() {
			int currentRowHandle = DataProvider.GetControllerRow(visibleIndex);
			int currentRowLevel = DataProvider.GetRowLevelByControllerRow(currentRowHandle);
			while(visibleIndex < DataProvider.VisibleCount + (IsBottomNewItemRowVisible ? 1 : 0)) {
				visibleIndexToScrollIndexMap[index] = visibleIndex;
				index++; visibleIndex++;
				int nextRowHandle = DataProvider.GetControllerRow(visibleIndex);
				int nextRowLevel = DataProvider.GetRowLevelByControllerRow(nextRowHandle);
				bool isGroupRow = DataProvider.IsGroupRowHandle(currentRowHandle);
				if(isGroupRow || IsSingleRow(currentRowHandle))
					groupRowHandleToSummaryRowCountMap[currentRowHandle] = currentSummaryRowCount;
				AddSummaryRows(ref index, currentRowHandle, currentRowLevel, nextRowLevel, nextRowHandle, visibleIndex);
				currentRowHandle = nextRowHandle; currentRowLevel = nextRowLevel;
				if(CachedRowCount > maxCachedRowCount)
					break;
			}
			if(IsBottomNewItemRowVisible && visibleIndex == DataProvider.VisibleCount + 1) 
				groupRowHandleToSummaryRowCountMap[DataControlBase.NewItemRowHandle] = currentSummaryRowCount;
		}
		void AddSummaryRows(ref int index, int currentRowHandle, int currentRowLevel, int nextRowLevel, int nextRowHandle, int visibleIndex) {
			if(!DataProvider.IsGroupRowHandle(currentRowHandle)) {
				currentRowHandle = DataProvider.GetParentRowHandle(currentRowHandle);
				currentRowLevel--;
			}
			if(currentRowHandle == DataControlBase.InvalidRowHandle) return;
			if(!DataProvider.IsGroupRowHandle(nextRowHandle) && (!IsSingleRow(nextRowHandle) || DataProvider.GetParentRowHandle(currentRowHandle) == DataProvider.GetParentRowHandle(nextRowHandle))
				&& !DataProvider.IsValidRowHandle(nextRowHandle) && visibleIndex != DataProvider.VisibleCount)
				return;
			for(int i = currentRowLevel; i >= DataProvider.GetActualRowLevel(nextRowHandle, nextRowLevel); i--) {
				if((!DataProvider.DataController.AllowPartialGrouping || DataProvider.GetChildRowCount(currentRowHandle) > 1) && DataProvider.Events.OnShowingGroupFooter(currentRowHandle, i)) {
					visibleIndexToScrollIndexMap[index] = new GroupSummaryRowKey(new Data.RowHandle(currentRowHandle), i);
					currentSummaryRowCount++;
					index++;
				}
				currentRowHandle = DataProvider.GetParentRowHandle(currentRowHandle);
			}
		}
		protected internal int CalcVisibleSummaryRowsCountBeforeRow(int visibleIndex, bool allowFixedGroups) {
			if(!ShowGroupSummaryFooter) return 0;
			ValidateCache();
			if(IsCacheEmpty) return 0;
			while(visibleIndex > this.visibleIndex - 1) {
				maxCachedRowCount += visibleIndex - this.visibleIndex - 1 + DefaultCachedRowsIncrement;
				BuildCache();
			}
			if(IsBottomNewItemRowVisible && visibleIndex == DataProvider.VisibleCount)
				return groupRowHandleToSummaryRowCountMap[DataControlBase.NewItemRowHandle];
			if(allowFixedGroups) {
				int[] indices = DataProvider.DataController.GetVisibleIndexes().ScrollableIndexes;
				int maxVisibleIndex = visibleIndex;
				for(int i = 0; i < indices.Length; i++) {
					int currentVisibleIndex = indices[i];
					maxVisibleIndex = Math.Max(currentVisibleIndex, maxVisibleIndex);
					if(currentVisibleIndex == visibleIndex)
						break;
				}
				visibleIndex = maxVisibleIndex;
			}
			return GetSummaryRowCountBeforeRow(visibleIndex);
		}
		int GetSummaryRowCountBeforeRow(int visibleIndex) {
			int currentRowHandle = DataProvider.GetControllerRow(visibleIndex);
			if(DataProvider.IsGroupRowHandle(currentRowHandle) || IsSingleRow(currentRowHandle))
				return GetCachedSummaryRowCountBeforeRow(currentRowHandle); 
			return GetCachedSummaryRowCountBeforeRow(DataProvider.GetParentRowHandle(currentRowHandle));
		}
		int GetCachedSummaryRowCountBeforeRow(int rowHandle) {
			int result = 0;
			if(rowHandle == DataControlBase.InvalidRowHandle)
				return result;
			while(!groupRowHandleToSummaryRowCountMap.TryGetValue(rowHandle, out result)) {
				maxCachedRowCount += DefaultCachedRowsIncrement;
				BuildCache();
				if(visibleIndex >= DataProvider.VisibleCount) return result;
			}
			return result;
		}
		internal bool ShowGroupSummaryFooterInternal { get; set; }
	}
	internal class GridServerModeDataControllerPrintInfo {
		public GridServerModeDataControllerPrintInfo(BaseGridController controller, Dictionary<ColumnBase, string> summaries, string fixedLeftSummaryText, string fixedRightSummaryText) {
			Controller = controller;
			Summaries = summaries;
			FixedLeftSummaryText = fixedLeftSummaryText;
			FixedRightSummaryText = fixedRightSummaryText;
		}
		public BaseGridController Controller { get; private set; }
		public Dictionary<ColumnBase, string> Summaries { get; private set; }
		public string FixedLeftSummaryText { get; private set; }
		public string FixedRightSummaryText { get; private set; }
	}
}
