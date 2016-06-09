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

#define DX_THREAD_DEBUGGER
using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using System.IO;
using System.Reflection;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.Threading;
using DevExpress.Data.Selection;
using DevExpress.Utils;
#if !SL
using System.Data;
using System.Windows.Forms;
using DevExpress.Data.Details;
using DevExpress.Data.Summary;
#else
using DevExpress.Xpf.Collections;
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using MethodInvoker = System.Action;
using DevExpress.Data.Details;
#endif
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Collections;
namespace DevExpress.Data {
	public class DataControllerBase : IDisposable, IEvaluatorDataAccess {
		public event EventHandler ListSourceChanged, VisibleRowCountChanged;
		public event ListChangedEventHandler BeforeListChanged, ListChanged;
		public event EventHandler BeforePopulateColumns;
		DataControllerNotificationProviders notificationProviders = null;
		bool disposed = false, allowNotifications = true;
		bool summariesIgnoreNullValues = false;
		bool keepGroupRowsExpandedOnRefresh = true;
		int prevVisibleCount = 0;
		BaseDataControllerHelper helper; 
		DataColumnInfoCollection columns;
		DataColumnInfoCollection detailColumns;
		ValueComparer valueComparer; 
		IDataControllerThreadClient threadClient = NullThreadClient.Default;
		IList listSource;
		IDataControllerData dataClient;
		protected IDataControllerSort fSortClient; 
		Type forcedDataRowType;
		int lockUpdate = 0;
		int listSourceChanging = 0;
		public DataControllerBase() {
			this.listSource = null;
			this.dataClient = null;
			this.fSortClient = null;
			this.columns = new DataColumnInfoCollection();
			this.detailColumns = new DataColumnInfoCollection();
			this.helper = CreateHelper();
			this.valueComparer = CreateValueComparer();
		}
		public virtual void Dispose() {
			this.disposed = true;
			SetListSource(null);
			ListChanged = null;
			BeforeListChanged = null;
			ClearClients();
		}
		protected virtual void ClearClients() {
			this.fSortClient = null;
			this.ThreadClient = null;
			this.dataClient = null;
		}
		public DataControllerNotificationProviders NotificationProviders {
			get { return notificationProviders; }
			set { notificationProviders = value; }
		}
		public DataControllerNotificationProviders GetNotificationProviders() {
			if(notificationProviders == null) return DataControllerNotificationProviders.Default;
			return notificationProviders;
		}
		public virtual bool AllowPartialGrouping {
			get { return false; }
			set {  }
		}
		protected internal virtual bool UseFirstRowTypeWhenPopulatingColumns(Type rowType) { return false; }
		public bool KeepGroupRowsExpandedOnRefresh { get { return keepGroupRowsExpandedOnRefresh; } set { keepGroupRowsExpandedOnRefresh = value; } }
		public Type ForcedDataRowType { get { return forcedDataRowType; } set { forcedDataRowType = value; } }
		public bool IsDisposed { get { return disposed; } }
		public bool SummariesIgnoreNullValues { get { return summariesIgnoreNullValues; } set { summariesIgnoreNullValues = value; } }
		public bool AllowNotifications { get { return allowNotifications; } set { allowNotifications = value; } }
		public IList ListSource {
			get { return listSource; }
		}
		public IDataControllerData DataClient {
			get { return dataClient; }
			set { dataClient = value; }
		}
		public IDataControllerData2 DataClient2 { get { return DataClient as IDataControllerData2; } }
		public IDataControllerSort SortClient {
			get { return fSortClient; }
			set { fSortClient = value; }
		}
		public DataColumnInfoCollection Columns { get { return columns; } protected set { columns = value; } }
		public DataColumnInfoCollection DetailColumns { get { return detailColumns; } }
		public bool ContainsColumn(string fieldName) {
			return FindColumn(fieldName) != null;
		}
		public DataColumnInfo FindColumn(string fieldName) {
			var res = Columns[fieldName];
			if(res == null) res = DetailColumns[fieldName];
			return res;
		}
		public int ListSourceRowCount { get { return IsReady ? Helper.Count : 0; } }
		public virtual void ValidateExpression(CriteriaOperator op) {
			if(!IsReady)
				return;
			if(ReferenceEquals(op, null))
				return;
			CriteriaCompiler.ToLambda(op, new BaseRowStub.DataControllerCriteriaCompilerDescriptor(this));
		}
		public sealed class EvalRowStub: BaseRowStub {
			readonly Delegate TypedDelegate;
			Func<BaseRowStub, object> UnTypedDelegate;
			public static EvalRowStub Create(DataControllerBase _DC, CriteriaOperator op, out Exception exception) {
				exception = null;
				if(!ReferenceEquals(op, null) && _DC.IsReady) {
					try {
						DataControllerCriteriaCompilerDescriptor coreDescriptors = new DataControllerCriteriaCompilerDescriptor(_DC);
						CriteriaCompilerNullRowProofDescriptor hedged = new CriteriaCompilerNullRowProofDescriptor(coreDescriptors);
						CachingCriteriaCompilerDescriptor dscr = new CachingCriteriaCompilerDescriptor(hedged, op);
						LambdaExpression lmbd = CriteriaCompiler.ToLambda(op, dscr);
						return new EvalRowStub(_DC, lmbd.Compile(), dscr.GetStubCleanUpCode());
					} catch(Exception e) {
						exception = e;
					}
				}
				Func<BaseRowStub, object> nullLambda = stub => null;
				return new EvalRowStub(_DC, nullLambda, null); ;
			}
			public EvalRowStub(DataControllerBase _DC, Delegate typedDelegate, Action additionalCleanUp)
				: base(_DC, additionalCleanUp) {
				this.TypedDelegate = typedDelegate;
				this.UnTypedDelegate = TypedDelegate as Func<BaseRowStub, object>;
				if(this.UnTypedDelegate == null) {
					ParameterExpression stubPar = Expression.Parameter(typeof(BaseRowStub), "stub");
					this.UnTypedDelegate = Expression.Lambda<Func<BaseRowStub, object>>(Expression.Convert(Expression.Invoke(Expression.Constant(this.TypedDelegate), stubPar), typeof(object)), stubPar).Compile();
				}
			}
			public object Evaluate(int forRowIndex) {
				if(this.RowIndex >= 0)
					throw new InvalidOperationException("Internal error: EvalRowStub is not clean before next evaluate (may be reenterancy?)");
				GoTo(forRowIndex);
				try {
					return this.UnTypedDelegate(this);
				} finally {
					Reset();
				}
			}
		}
		public virtual ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, bool setDataAccess, out Exception e) {
			e = null;
			if(ReferenceEquals(criteriaOperator, null))
				return null;
			if(!IsReady || Columns.Count == 0)
				return null;
			try {
				ExpressionEvaluator evaluator = new ExpressionEvaluator(GetFilterDescriptorCollection(), criteriaOperator, false); 
				if(setDataAccess)
					evaluator.DataAccess = this;
				return evaluator;
			}
			catch(Exception ex) {
				e = ex;
				return null;
			}
		}
		public virtual EvalRowStub CreateEvalRowStub(CriteriaOperator expression, out Exception ex) {
			return EvalRowStub.Create(this, expression, out ex);
		}
		public virtual void BeginUpdate() {
			Helper.UnsubscribeEvents();
			this.lockUpdate++;
		}
		public virtual void EndUpdate() {
			EndUpdateCore(false);
		}
		public void CancelUpdate() {
			this.lockUpdate--;
			Helper.SubscribeEvents();
		}
		protected virtual void EndUpdateCore(bool sortUpdate) {
			CancelUpdate();
		}
		public void AddThreadClient(IDataControllerThreadClient client) {
			MultiThreadClient mt = ThreadClient as MultiThreadClient;
			if(mt == null) {
				mt = new MultiThreadClient();
				if(ThreadClient != NullThreadClient.Default) mt.Add(ThreadClient);
				ThreadClient = mt;
			}
			mt.Add(client);
		}
		public void RemoveThreadClient(IDataControllerThreadClient client) {
			MultiThreadClient mt = ThreadClient as MultiThreadClient;
			if(mt != null) {
				mt.Remove(client);
				if(mt.Listeners.Count == 0) ThreadClient = null;
			}
			if(ThreadClient == client) ThreadClient = null;
		}
		public virtual void WaitForAsyncEnd() { }
		public virtual IDataControllerThreadClient ThreadClient {
			get { return threadClient; }
			set {
				if(value == null) value = NullThreadClient.Default;
				MultiThreadClient mt = ThreadClient as MultiThreadClient;
				if(mt != null && value == NullThreadClient.Default) mt.Listeners.Clear();
				threadClient = value;
			}
		}
		protected virtual internal PropertyDescriptorCollection GetFilterDescriptorCollection() {
			return Helper.DescriptorCollection;
		}
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			if(descriptor is DevExpress.Data.Access.UnboundPropertyDescriptor) {
				return descriptor.GetValue(theObject);
			}
			DevExpress.Data.Access.DisplayTextPropertyDescriptor da = descriptor as DevExpress.Data.Access.DisplayTextPropertyDescriptor;
			if(da != null) {
				if(da.Info.PropertyDescriptor is DevExpress.Data.Access.UnboundPropertyDescriptor) {
					return da.GetValue((int)theObject, theObject);
				}
				return da.GetValue((int)theObject, Helper.GetRow((int)theObject));
			}
			object row = Helper.GetRow((int)theObject);
			if(row == null || row is NotLoadedObject) return null;
			return descriptor.GetValue(row);
		}
		public int LockUpdate { get { return lockUpdate; } }
		public virtual bool IsUpdateLocked { get { return this.lockUpdate != 0; } }
		public virtual bool IsReady { get { return ListSource != null; } }
		public virtual bool IsColumnValid(int column) {
			if(!IsReady) return false;
			return (column >= 0) && (column < Columns.Count);
		}
		public virtual bool IsDetailColumnValid(int column) {
			if(!IsReady) return false;
			return (column >= 0) && (column < DetailColumns.Count);
		}
		public virtual void RefreshData() {
			DoRefresh();
		}
		public void RePopulateColumns() { RePopulateColumns(true); }
		public virtual void RePopulateColumns(bool allowRefresh) {
			PopulateColumns();
		}
		public virtual void PopulateColumns() {
			Columns.Clear();
			DetailColumns.Clear();
			RaiseBeforePopulateColumns(EventArgs.Empty);
			Helper.PopulateColumns();
			DoRefresh();
		}
		public void DoRefresh() { DoRefresh(!IsListSourceChanging); }
		protected virtual void DoRefresh(bool useRowsKeeper) { }
		public virtual void CollapseDetailRows() { }
		public virtual bool AlwaysUsePrimitiveDataSource { get { return false; } }
		public ValueComparer ValueComparer { get { return valueComparer; } }
		public virtual bool IsGroupRowHandle(int controllerRowHandle) {
			return GroupRowInfo.IsGroupRowHandle(controllerRowHandle);
		}
		public virtual bool IsServerMode { get { return false; } }
		protected virtual void SetListSourceCore(IList value) {
			Helper.UnsubscribeEvents();
			UnsubscribeDataSync();
			listSource = value;
			if(IsDisposed) return;
			OnListSourceChanged();
			Helper.SubscribeEvents();
			SubscribeDataSync();
			RequestDataSyncInitialize();
		}
		protected internal virtual IDataSync DataSync { get { return ListSource as IDataSync; } }
		protected virtual void RequestDataSyncInitialize() { }
		protected virtual void SubscribeDataSync() { }
		protected virtual void UnsubscribeDataSync() { }
		protected virtual void SetListSource(IList value) {
			if(ListSource == value) return;
			SetListSourceCore(value);
		}
		public BaseDataControllerHelper Helper { get { return helper; } }
		protected virtual BaseDataControllerHelper CreateHelper() {
			if(ListSource == null) return new BaseDataControllerHelper(this);
#if !SL && !DXPORTABLE
			if(ListSource is DataView) return new DataViewDataControllerHelper(this);
#if DXWhidbey
			System.Windows.Forms.BindingSource bs = ListSource as System.Windows.Forms.BindingSource;
			try {
				if(bs != null && bs.SyncRoot is DataView) return new BindingSourceDataControllerHelper(this);
			}
			catch { }
#endif
#endif
			return new ListDataControllerHelper(this);
		}
		protected virtual ValueComparer CreateValueComparer() {
			return new ValueComparer();
		}
		protected virtual void Reset() {
		}
		protected virtual void OnListSourceChangeClear() {
			Columns.Clear();
		}
		protected virtual void OnListSourceChanged() {
			if(this.helper != null) this.helper.Dispose();
			Reset();
			this.helper = CreateHelper();
			if(IsDisposed) return;
			this.listSourceChanging++;
			try {
				BeginUpdate();
				try {
					OnListSourceChangeClear();
					CollapseDetailRows();
					PopulateColumns();
					VisualClientRequestSynchronization();
				}
				finally {
					EndUpdate();
				}
			}
			finally {
				this.listSourceChanging--;
			}
			RaiseListSourceChanged();
		}
		protected bool IsListSourceChanging { get { return listSourceChanging != 0; } }
		protected virtual void RaiseListSourceChanged() {
			if(ListSourceChanged != null) ListSourceChanged(this, EventArgs.Empty);
		}
		int suspedVCount = 0;
		protected void SuspendVisibleRowCountChanged() { this.suspedVCount++; }
		protected void ResumeVisibleRowCountChanged() { this.suspedVCount--; }
		protected internal virtual void RaiseVisibleRowCountChanged() {
			if(this.suspedVCount != 0) return;
			if(VisibleRowCountChanged != null) VisibleRowCountChanged(this, EventArgs.Empty);
		}
		protected int PrevVisibleCount { get { return prevVisibleCount; } }
		protected virtual bool StorePrevVisibleCount(int visibleCount) {
			if(this.suspedVCount != 0) return true;
			bool equals = visibleCount == prevVisibleCount;
			prevVisibleCount = visibleCount;
			return equals;
		}
		protected virtual void VisualClientNotifyTotalSummary() { }
		protected internal virtual void VisualClientRequestSynchronization() { }
		protected internal virtual void VisualClientUpdateLayout() { }
		protected internal virtual void OnStartNewItemRow() { }
		protected internal virtual void OnEndNewItemRow() { }
		protected internal virtual void OnItemDeleting(int listSourceRow) { }
		protected internal virtual void OnItemDeleted(int listSourceRow) { }
		protected internal virtual void OnGroupDeleted(GroupRowInfo groupRow) { }
		protected internal virtual void OnGroupsDeleted(List<GroupRowInfo> groups, bool addedToSameGroup) { }
		protected internal virtual void RaiseOnBindingListChanged(ListChangedEventArgs e) {
			RaiseOnBeforeListChanged(e);
			try {
				OnBindingListChanged(e);
			}
			finally {
				RaiseOnListChanged(e);
			}
		}
		protected void RaiseOnListChanged(ListChangedEventArgs e) {
			if(ListChanged != null) ListChanged(this, e);
		}
		protected virtual void RaiseOnBeforeListChanged(ListChangedEventArgs e) {
			if(BeforeListChanged != null) BeforeListChanged(this, e);
		}
		protected virtual void OnBindingListChanged(ListChangedEventArgs e) {
			switch(e.ListChangedType) {
				case ListChangedType.PropertyDescriptorAdded:
				case ListChangedType.PropertyDescriptorDeleted:
					RePopulateColumns();
					break;
				case ListChangedType.PropertyDescriptorChanged:
					SetListSourceCore(ListSource);
					break;
			}
		}
		internal void RaiseBeforePopulateColumns(EventArgs e) {
			if(BeforePopulateColumns != null) BeforePopulateColumns(this, e);
		}
		protected int GetChangedListSourceRow(ListChangedEventArgs e) {
			if(e.OldIndex == -1) return e.NewIndex;
			return e.OldIndex;
		}
		protected virtual DataControllerChangedItemCollection CreateDataControllerChangedItemCollection() {
			return new DataControllerChangedItemCollection();
		}
		#region Make the methods accessible in PivotGridController
		protected int GetColumnIndex(DataColumnInfo column) { return column.ColumnIndex; }
		protected int GetListSourceFromVisibleListSourceRowCollection(VisibleListSourceRowCollection visibleListSourceRowCollection, int controllerRow) {
			return visibleListSourceRowCollection.GetListSourceRow(controllerRow);
		}
		protected void SetVisibleListSourceCollectionCore(VisibleListSourceRowCollection visibleListSourceRowCollection, int[] list, int count) {
			visibleListSourceRowCollection.Init(list, count);
		}
		protected void ResetSortInfoCollectionCore(DataColumnSortInfoCollection sortInfo) {
			sortInfo.Reset();
		}
		#endregion
		public int GetListSourceRowIndex(GroupRowInfoCollection groupInfo, int controllerRow) {
			if(IsGroupRowHandle(controllerRow)) {
				GroupRowInfo group = groupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
				controllerRow = group == null ? DataController.InvalidRow : group.ChildControllerRow;
			}
			if(!IsControllerRowValid(groupInfo.VisibleListSourceRows, controllerRow)) return DataController.InvalidRow;
			return groupInfo.VisibleListSourceRows.GetListSourceRow(controllerRow);
		}
		public virtual bool IsControllerRowValid(VisibleListSourceRowCollection visibleListSourceRows, int controllerRow) {
			return IsReady && controllerRow >= 0 && controllerRow < GetVisibleListSourceRowCount(visibleListSourceRows);
		}
		public int GetVisibleListSourceRowCount(VisibleListSourceRowCollection visibleListSourceRows) {
			return visibleListSourceRows.VisibleRowCount;
		}
		protected internal virtual void DoGroupColumn(DataColumnSortInfoCollection sortInfo, GroupRowInfoCollection groupInfo, int controllerRow, int rowCount, GroupRowInfo parentGroup) {
			byte level = (byte)(parentGroup == null ? 0 : parentGroup.Level + 1);
			DataColumnInfo columnInfo = sortInfo[level].ColumnInfo;
			bool isRunningSummary = sortInfo[level].RunningSummary,
				isCrossGroupRunning = sortInfo[level].CrossGroupRunningSummary;
			int prevStart = controllerRow;
			object prevVal = null, val;
			GroupRowInfo groupRow = null;
			for(int row = 0; row < rowCount + 1; row++) {
				if(row != rowCount) {
					int listSourceRow = GetListSourceRowIndex(groupInfo, controllerRow + row);
					val = Helper.GetRowValue(listSourceRow, columnInfo.Index, null);
					if(row > 0) {
						if(IsEqualGroupValues(prevVal, val, GetListSourceRowIndex(groupInfo, controllerRow + row - 1), listSourceRow, columnInfo)) {
							groupRow.ChildControllerRowCount++;
							continue;
						}
					}
					prevVal = val;
				}
				if(row > 0 && !groupInfo.IsLastLevel(groupRow)) { 
					int childRowCount = groupRow.ChildControllerRowCount;
					if(isRunningSummary) {
						if(isCrossGroupRunning)
							childRowCount = childRowCount - prevStart;
						else
							childRowCount = childRowCount + controllerRow - prevStart;
					}
					if(childRowCount + prevStart > controllerRow + rowCount)
						childRowCount = controllerRow + rowCount - prevStart;
					DoGroupColumn(sortInfo, groupInfo, prevStart, childRowCount, groupRow);
					prevStart = controllerRow + row;
				}
				if(row == rowCount) break;
				int startRow, controllerRowCount;
				if(isRunningSummary) {
					if(isCrossGroupRunning) {
						startRow = 0;
						controllerRowCount = row + controllerRow + 1;
					} else {
						startRow = controllerRow;
						controllerRowCount = ((groupRow == null) ? 0 : groupRow.ChildControllerRowCount) + 1;
					}
				} else {
					startRow = row + controllerRow;
					controllerRowCount = 1;
				}
				groupRow = groupInfo.Add(level, startRow, parentGroup);
				groupRow.ChildControllerRowCount = controllerRowCount;
			}
		}
		protected internal virtual int CompareGroupColumnRows(DataColumnSortInfoCollection sortInfo, GroupRowInfoCollection groupInfo, int controllerRow1, int controllerRow2) {
			int listSourceRow1 = GetListSourceRowIndex(groupInfo, controllerRow1);
			int listSourceRow2 = GetListSourceRowIndex(groupInfo, controllerRow2);
			for(int n = 0; n < sortInfo.GroupCount; n++) {
				object val1 = Helper.GetRowValue(listSourceRow1, sortInfo[n].ColumnInfo.Index, null);
				object val2 = Helper.GetRowValue(listSourceRow2, sortInfo[n].ColumnInfo.Index, null);
				if(!IsEqualGroupValues(val1, val2, listSourceRow1, listSourceRow2, sortInfo[n].ColumnInfo)) {
					return n;
				}
			}
			return -1;
		}
		protected virtual bool IsEqualGroupValues(object val1, object val2, int listSourceRow1, int listSourceRow2, DataColumnInfo columnInfo) {
			if(this.fSortClient != null) {
				var clientEq = this.fSortClient.IsEqualGroupValues(listSourceRow1, listSourceRow2, val1, val2, columnInfo);
				if(clientEq.HasValue)
					return clientEq.Value;
			}
			return IsEqualGroupValues(val1, val2);
		}
		public bool IsEqualGroupValues(object val1, object val2) {
			if(val1 == null || val2 == null || val1 is DBNull || val2 is DBNull) {
				return val1 == val2;
			}
			return IsEqualNonNullValues(val1, val2);
		}
		protected virtual bool IsEqualNonNullValues(object val1, object val2) {
			return val1.Equals(val2);
		}
		public virtual void UpdateTotalSummary(List<SummaryItem> changedItems) { }
		public void UpdateTotalSummary() { UpdateTotalSummary(null); }
		public virtual void UpdateGroupSummary(GroupRowInfo groupRow, DataControllerChangedItemCollection changedItems) { }
		public bool AllowIEnumerableDetails { get; set; }
		public static bool AllowFindNonStringTypesServerMode = true;
		public static bool CatchRowUpdatedExceptions = true;
		protected internal virtual void MakeGroupRowVisible(GroupRowInfo groupRow) {
			groupRow.Expanded = true;
		}
		protected internal virtual void OnColumnPopulated(DataColumnInfo info) { }
		protected internal virtual void SubscribeListChanged(INotificationProvider provider, object list) {
			provider.SubscribeNotifications(new ListChangedEventHandler(OnBindingListChanged));
		}
		protected internal virtual void UnsubscribeListChanged(INotificationProvider provider, object list) {
			provider.UnsubscribeNotifications(new ListChangedEventHandler(OnBindingListChanged));
		}
		bool ignoreNextReset = false;
		protected void OnBindingListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.Reset && ignoreNextReset) {
				ignoreNextReset = false;
				CheckInvalidCurrentRow();
				return;
			}
#if !SL && !DXPORTABLE
			if(e.ListChangedType == ListChangedType.PropertyDescriptorChanged
				&& StackTraceHelper.CheckStackFrame("ResetBindings", typeof(BindingSource))) {
				ignoreNextReset = true;
			}
#endif
			if(!IsDisposed) Helper.OnBindingListChanged(e);
		}
		protected virtual void CheckInvalidCurrentRow() { }
		protected internal virtual object GetGroupRowKeyValueInternal(GroupRowInfo group) { return null; }
		protected internal virtual bool RaiseRowDeleting(int listSourceRowIndex) { return true; }
		protected internal virtual void RaiseRowDeleted(int controllerRow, int listSourceRowIndex, object row) { }
		public virtual int GetControllerRow(int listSourceRow) { return listSourceRow; }
		public bool ComplexUseLinqDescriptors { get; set; }
		protected internal virtual TypeConverter GetActualTypeConverter(TypeConverter converter, PropertyDescriptor property) {
			return converter;
		}
	}
	public class DataController : DataControllerBase {
		public event SelectionChangedEventHandler SelectionChanged;
		public event EventHandler Refreshed;
		public event RowDeletedEventHandler RowDeleted;
		public event RowDeletingEventHandler RowDeleting;
		Dictionary<int, bool> detailEmptyHash;
		FilterHelper filterHelper;
		bool allowRestoreSelection = true;
		bool notifyClientOnNextChange = false;
		string lastErrorText = string.Empty;
		ListSourceRowsKeeper rowsKeeper;
		SelectionController selection; 
		IDataControllerVisualClient visualClient = NullVisualClient.Default; 
		IDataControllerRelationSupport relationSupport = null;
		DataColumnSortInfoCollection sortInfo; 
		VisibleListSourceRowCollection visibleListSourceRows, visibleListSourceRowsFilterCache;
		VisibleIndexCollection visibleIndexes; 
		GroupRowInfoCollection groupInfo; 
		SummaryItemCollection groupSummary; 
		TotalSummaryItemCollection totalSummary; 
#if !SL
		MasterRowInfoCollection expandedMasterRowCollection;
#endif
		SummarySortInfoCollection summarySortInfo; 
		CriteriaOperator filterCriteria; 
		bool autoUpdateTotalSummary = true, immediateUpdateRowPosition = true;
		FilterRowStub _FilterStub = null;
		public const int InvalidRow = Int32.MinValue; 
		public const int OperationInProgress = Int32.MinValue + 10; 
		public event CustomSummaryEventHandler CustomSummary;
		public event CustomSummaryExistEventHandler CustomSummaryExists;
		public bool MaintainVisibleRowBindingOnFilterChange { get; set; }
		CustomSummaryEventArgs summaryCalculateArgs = null; 
		protected internal CustomSummaryEventArgs SummaryCalculateArgs {
			get {
				if(summaryCalculateArgs == null) {
					summaryCalculateArgs = CreateCustomSummaryEventArgs();
					summaryCalculateArgs.controller = this;
				}
				return summaryCalculateArgs;
			}
		}
		public DataController() {
			this.detailEmptyHash = new Dictionary<int, bool>();
			this.rowsKeeper = CreateControllerRowsKeeper();
			this.selection = CreateSelectionController();
			this.sortInfo = new DataColumnSortInfoCollection(this, new CollectionChangeEventHandler(OnSortInfoCollectionChanged));
			this.groupSummary = new SummaryItemCollection(this, new CollectionChangeEventHandler(OnGroupSummaryCollectionChanged));
			this.totalSummary = new TotalSummaryItemCollection(this, new CollectionChangeEventHandler(OnTotalSummaryCollectionChanged));
			this.summarySortInfo = new SummarySortInfoCollection(new CollectionChangeEventHandler(OnSortSummaryCollectionChanged));
#if !SL
			this.expandedMasterRowCollection = CreateMasterRowCollection();
#endif
			this.visibleListSourceRows = new VisibleListSourceRowCollection(this);
			this.groupInfo = CreateGroupRowInfoCollection();
			this.visibleListSourceRowsFilterCache = null;
			this.visibleIndexes = CreateVisibleIndexCollection();
			this.filterHelper = CreateFilterHelper();
			CollapseDetailRowsOnReset = true;
		}
		public override void Dispose() {
			if(IsDisposed) return;
			base.Dispose();
			this.sortInfo.CollectionChanged -= new CollectionChangeEventHandler(OnSortInfoCollectionChanged);
			this.groupSummary.CollectionChanged -= new CollectionChangeEventHandler(OnGroupSummaryCollectionChanged);
			this.totalSummary.CollectionChanged -= new CollectionChangeEventHandler(OnTotalSummaryCollectionChanged);
			this.summarySortInfo.CollectionChanged -= new CollectionChangeEventHandler(OnSortSummaryCollectionChanged);
			this.selection.Dispose();
			this.rowsKeeper.Dispose();
			this.rowsKeeper = null;
			this.selection = null;
			this.VisualClient = null;
			this.relationSupport = null;
			this.RowDeleting = null;
			this.RowDeleted = null;
			summaryCalculateArgs = null;
		}
		protected override void ClearClients() {
			base.ClearClients();
			this.VisualClient = null;
			this.relationSupport = null;
		}
		protected internal override bool RaiseRowDeleting(int listSourceRowIndex) {
			if(RowDeleting == null) return true;
			RowDeletingEventArgs e = new RowDeletingEventArgs(GetControllerRow(listSourceRowIndex), listSourceRowIndex, GetRowByListSourceIndex(listSourceRowIndex));
			RowDeleting(this, e);
			return !e.Cancel;
		}
		protected internal override void RaiseRowDeleted(int controllerRow, int listSourceRowIndex, object row) {
			if(RowDeleted == null) return;
			RowDeletedEventArgs e = new RowDeletedEventArgs(controllerRow, listSourceRowIndex, row);
			RowDeleted(this, e);
		}
		protected virtual CustomSummaryExistEventArgs CreateCustomSummaryExistEventArgs(GroupRowInfo groupRow, object item) { return new CustomSummaryExistEventArgs(groupRow, item); }
		protected virtual CustomSummaryEventArgs CreateCustomSummaryEventArgs() { return new CustomSummaryEventArgs(); }
		protected virtual MasterRowInfoCollection CreateMasterRowCollection() { return new MasterRowInfoCollection(this); }
		protected virtual FilterHelper CreateFilterHelper() { return new DataControllerFilterHelper(this); }
		protected virtual GroupRowInfoCollection CreateGroupRowInfoCollection() { return new DataControllerGroupRowInfoCollection(this); }
		protected virtual SelectionController CreateSelectionController() { return new SelectionController(this); }
		protected virtual VisibleIndexCollection CreateVisibleIndexCollection() { return new DataControllerVisibleIndexCollection(this); }
		public void BeginSortUpdate() {
			BeginUpdate();
		}
		public void EndSortUpdate() {
			EndUpdateCore(true);
		}
		public override void BeginUpdate() {
			if(LockUpdate == 0) {
				if(!IsListSourceChanging) RowsKeeper.Save();
			}
			base.BeginUpdate();
		}
		protected override void EndUpdateCore(bool sortUpdate) {
			base.EndUpdateCore(sortUpdate);
			if(LockUpdate == 0) {
				if(sortUpdate)
					DoSortGroupRefresh();
				else
					DoRefresh();
			}
		}
#if !SL
		public virtual CriteriaOperator CalcColumnFilterCriteriaByValue(string fieldName, object columnValue, bool equal, bool roundDateTime, IFormatProvider provider) {
			DataColumnInfo info = Columns[fieldName];
			if(info == null) return null;
			return FilterHelper.CalcColumnFilterCriteriaByValue(info.Index, columnValue, equal, roundDateTime, provider);
		}
#endif
		public virtual object[] GetUniqueColumnValues(string fieldName, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed) {
			return GetUniqueColumnValues(fieldName, maxCount, includeFilteredOut, roundDataTime, completed, false);
		}
		public virtual object[] GetUniqueColumnValues(string fieldName, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed, bool implyNullLikeEmptyStringWhenFiltering) {
			DataColumnInfo info = Columns[fieldName];
			if(info == null) return null;
			return FilterHelper.GetUniqueColumnValues(info.Index, maxCount, includeFilteredOut, roundDataTime, completed, implyNullLikeEmptyStringWhenFiltering);
		}
		public virtual void LoadRows(int startFrom, int count) { }
		public virtual void ClearInvalidRowsCache() { }
		public virtual void CancelWeakFindIncremental() { }
		public virtual void CancelFindIncremental() { }
		public virtual void ScrollingCancelAllGetRows() { }
		public virtual void ScrollingCheckRowLoaded(int rowHandle) { }
		public virtual void UpdateSortGroup(DataColumnSortInfo[] sortInfo, int groupCount, SummarySortInfo[] summaryInfo) {
			BeginUpdate();
			try {
				if(groupCount == 0 || sortInfo.Length == 0) summaryInfo = new SummarySortInfo[0];
				SortInfo.ClearAndAddRange(sortInfo, groupCount);
				SummarySortInfo.ClearAndAddRange(summaryInfo);
			}
			finally {
				EndUpdate();
			}
		}
		public Dictionary<int, bool> DetailEmptyHash { get { return detailEmptyHash; } }
		public FilterHelper FilterHelper { get { return filterHelper; } }
		public IDataControllerRelationSupport RelationSupport {
			get { return relationSupport; }
			set { relationSupport = value; }
		}
		public bool AutoExpandAllGroups {
			get { return GroupInfo.AutoExpandAllGroups; }
			set { GroupInfo.AutoExpandAllGroups = value; }
		}
		public override bool AllowPartialGrouping {
			get { return GroupInfo.AllowPartialGrouping && SortInfo.GroupCount > 0; }
			set { GroupInfo.AllowPartialGrouping = value; }
		}
		public virtual bool AutoUpdateTotalSummary {
			get { return autoUpdateTotalSummary; }
			set { autoUpdateTotalSummary = value; }
		}
		public virtual bool ImmediateUpdateRowPosition {
			get { return immediateUpdateRowPosition; }
			set { immediateUpdateRowPosition = value; }
		}
		public bool IsImmediateUpdateRowPosition { get { return ImmediateUpdateRowPosition; }} 
		public IDataControllerVisualClient2 VisualClient2 { get { return VisualClient as IDataControllerVisualClient2; } }
		public IDataControllerVisualClient VisualClient {
			get { return visualClient; }
			set {
				if(value == null) value = NullVisualClient.Default;
				visualClient = value; 
			}
		}
		public bool AllowRestoreSelection { get { return allowRestoreSelection; } set { allowRestoreSelection = value; } }
		public bool IsInitializing { get { return VisualClient != null && VisualClient.IsInitializing; } }
		public SelectionController Selection { get { return selection; } }
		public void SaveRowState(Stream stream) {
			RowsKeeper.Save();
			RowsKeeper.GroupHashEx.SaveToStream(stream);
		}
		public void RestoreRowState(Stream stream) {
			if(stream.Position >= stream.Length) return;
			RowsKeeper.GroupHashEx.RestoreFromStream(stream);
			if(RowsKeeper.RestoreStream()) BuildVisibleIndexes();
		}
		public void SaveRowState() {
			SaveRowState(RowsKeeper);
		}
		public void SaveRowState(ListSourceRowsKeeper keeper) {
			keeper.Save();
		}
		public void RestoreRowState() {
			RestoreRowState(RowsKeeper);
		}
		public void RestoreRowState(ListSourceRowsKeeper keeper) {
			if(keeper.Restore()) BuildVisibleIndexes();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetRowsKeeper() { RowsKeeper.Clear(); }
		protected void ResetRowsKeeperEx() {
			if(!IsUpdateLocked && !IsRefreshInProgress)
				RowsKeeper.Clear();
		}
		protected ListSourceRowsKeeper RowsKeeper { get { return rowsKeeper; } }
		void ResetFilterStub() {
			this._FilterStub = null;
		}
		protected FilterRowStub FilterStub {
			get {
				if(_FilterStub == null) {
					Exception e;
					this._FilterStub = CreateFilterRowStub(out e);
				}
				return this._FilterStub;
			}
		}
		[Obsolete("Use FilterDelegate instead", true)]
		protected ExpressionEvaluator FilterExpressionEvaluator {
			get {
				throw new NotSupportedException();
			}
		}
		public CriteriaOperator ClientUserSubstituteFilter(CriteriaOperator criterion) {
			var client = this.DataClient2;
			if(client == null)
				return criterion;
			else {
				var args = new SubstituteFilterEventArgs(criterion);
				client.SubstituteFilter(args);
				return args.Filter;
			}
		}
		public sealed class FilterRowStub: BaseRowStub {
			readonly Func<BaseRowStub, bool> FilterDelegate;
			public static FilterRowStub Create(DataController _DC, out Exception exception) {
				exception = null;
				CriteriaOperator op = _DC.ClientUserSubstituteFilter(_DC.FilterCriteria);
				if(ReferenceEquals(op, null) || !_DC.IsReady)
					return new FilterRowStub(_DC, x => true, null);
				try {
					DataControllerCriteriaCompilerDescriptor coreDescriptors = new DataControllerCriteriaCompilerDescriptor(_DC);
					CachingCriteriaCompilerDescriptor dscr = new CachingCriteriaCompilerDescriptor(coreDescriptors, op);
					Func<BaseRowStub, bool> predicate = CriteriaCompiler.ToPredicate<BaseRowStub>(op, dscr);
					return new FilterRowStub(_DC, predicate, dscr.GetStubCleanUpCode());
				} catch(Exception e) {
					exception = e;
					return new FilterRowStub(_DC, x => false, null);
				}
			}
			public FilterRowStub(DataController _DC, Func<BaseRowStub, bool> _FilterDelegate, Action additionalCleanUp)
				: base(_DC, additionalCleanUp) {
				this.FilterDelegate = _FilterDelegate;
			}
			public bool Filter() {
				return FilterDelegate(this);
			}
		}
		protected virtual FilterRowStub CreateFilterRowStub(out Exception exception) {
			return FilterRowStub.Create(this, out exception);
		}
		protected bool HasUserFilter { get { return (DataClient2 != null && DataClient2.HasUserFilter); } }
		public bool IsFiltered { get { return !ReferenceEquals(FilterCriteria, null) || HasUserFilter; } }
		public CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				if(Equals(FilterCriteria, value))
					return;
				CriteriaOperator oldCriteria = FilterCriteria;
				try {
					filterCriteria = CriteriaOperator.Clone(value);
					OnFilterExpressionChanged();
				}
				catch {
					filterCriteria = oldCriteria;
					OnFilterExpressionChanged();
				}
			}
		}
		public string FilterExpression {
			get { return CriteriaOperator.ToString(FilterCriteria); }
			set { FilterCriteria = CriteriaOperator.Parse(value); }
		}
		public virtual bool AllowNew { get { return Helper.AllowNew; } }
		public bool AllowEdit { get { return Helper.AllowEdit; } }
		public bool AllowRemove { get { return Helper.AllowRemove; } }
		public bool IsSupportMasterDetail { get { return IsReady && RelationCount > 0; } }
		public DataColumnSortInfoCollection SortInfo { get { return sortInfo; } }
		public SummaryItemCollection GroupSummary { get { return groupSummary; } }
		public SummarySortInfoCollection SummarySortInfo { get { return summarySortInfo; } }
		public TotalSummaryItemCollection TotalSummary { get { return totalSummary; } }
		public int VisibleListSourceRowCount {
			get {
				int count = ListSourceRowCount;
				if(count == 0)
					return count;
				return VisibleListSourceRows.VisibleRowCount;
			}
		}
		public int GroupRowCount { get { return GroupInfo.Count; } }
		public int RelationCount {
			get { return GetRelationCount(InvalidRow); }
		}
		public int GetRelationCount(int controllerRow) {
			int count = controllerRow == InvalidRow || Helper.RelationListEx == Helper ? Helper.RelationList.RelationCount : Helper.RelationListEx.GetRelationCount(GetListSourceRowIndex(controllerRow));
			if(RelationSupport != null) count = RelationSupport.GetRelationCount(count, controllerRow);
			return count;
		}
		public int GetRelationIndex(int controllerRow, string relationName) {
			int count = GetRelationCount(controllerRow);
			for(int n = 0; n < count; n++) {
				if(GetRelationName(controllerRow, n) == relationName) return n;
			}
			return InvalidRow;
		}
		public string GetRelationName(int controllerRow, int relationIndex) {
			string name = Helper.RelationList.GetRelationName(GetListSourceRowIndex(controllerRow), relationIndex);
			if(RelationSupport != null) name = RelationSupport.GetRelationName(name, controllerRow, relationIndex);
			return name;
		}
		public string GetRelationDisplayName(int controllerRow, int relationIndex) {
			string name = Helper.RelationListEx.GetRelationDisplayName(GetListSourceRowIndex(controllerRow), relationIndex);
			if(RelationSupport != null ) name = RelationSupport.GetRelationDisplayName(name, controllerRow, relationIndex); 
			return name;
		}
		public int GetPrevSibling(int controllerRow) {
			if(!IsValidControllerRowHandle(controllerRow)) return InvalidRow;
			if(!IsGrouped) {
				if(!IsValidControllerRowHandle(controllerRow - 1)) return controllerRow;
				return controllerRow - 1;
			}
			int res = GetParentRowHandle(controllerRow);
			if(res != InvalidRow) return res;
			GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
			if(group == null) return controllerRow;
			for(int n = group.Index - 1; n >= 0; n--) {
				if(GroupInfo[n].Level == group.Level) return GroupInfo[n].Handle;
			}
			return controllerRow;
		}
		public int GetNextSibling(int controllerRow) {
			if(!IsValidControllerRowHandle(controllerRow)) return InvalidRow;
			if(!IsGrouped) {
				if(!IsValidControllerRowHandle(controllerRow + 1)) return controllerRow;
				return controllerRow + 1;
			}
			GroupRowInfo group = GetParentGroupRow(controllerRow);
			if(group == null) group = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
			if(group == null) return controllerRow;
			for(int n = group.Index + 1; n < GroupInfo.Count; n++) {
				if(GroupInfo[n].Level <= group.Level) return GroupInfo[n].Handle;
			}
			return controllerRow;
		}
		public int GetParentRowHandle(int controllerRow) {
			GroupRowInfo res = GetParentGroupRow(controllerRow);
			return res == null ? InvalidRow : res.Handle;
		}
		public GroupRowInfo GetParentGroupRow(int controllerRow) {
			if(IsGroupRowHandle(controllerRow)) {
				GroupRowInfo res = GroupInfo.GetGroupRowInfoByHandle(controllerRow);
				return res == null ? null : res.ParentGroup;
			}
			return GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
		}
		public virtual void DeleteRow(int controllerRow) {
			DeleteRows(new int[] { controllerRow });
		}
		public virtual void DeleteSelectedRows() {
			if(!AllowRemove || Selection.Count == 0) return;
			int[] rows = Selection.GetSelectedRows();
			DeleteRows(rows);
		}
		public virtual void DeleteRows(int[] controllerRows) {
			if(!AllowRemove) return;
			int[] listRows = GetListSourceRows(controllerRows);
			if(listRows.Length == 0) return;
			if(listRows.Length == 1)
				Helper.DeleteRow(listRows[0]);
			else {
				if(!IsUpdateLocked) RowsKeeper.Save();
				BeginUpdate();
				Selection.BeginSelection();
				Selection.LockAddRemoveAction();
				try {
					for(int i = listRows.Length - 1; i >= 0; i--) {
						Helper.DeleteRow(listRows[i]);
						OnActionItemDeleted(listRows[i]);
					}
					Selection.Clear();
				}
				finally {
					Selection.UnLockAddRemoveAction();
					Selection.CancelSelection();
					CancelUpdate();
					DoRefresh(false);
					RowsKeeper.Clear();
					Selection.RaiseSelectionChanged();
				}
			}
		}
		int[] GetListSourceRows(int[] controllerRows) {
			Dictionary<int, int> list = new Dictionary<int, int>();
			for(int i = 0; i < controllerRows.Length; i++) {
				if(IsGroupRowHandle(controllerRows[i])) {
					GroupRowInfo groupInfo = GroupInfo.GetGroupRowInfoByHandle(controllerRows[i]);
					if(groupInfo != null) {
						for(int n = 0; n < groupInfo.ChildControllerRowCount; n++)
							AddControllerRowToDeletedHashtable(list, groupInfo.ChildControllerRow + n);
					}
					continue;
				}
				AddControllerRowToDeletedHashtable(list, controllerRows[i]);
			}
			int[] listRows = new int[list.Count];
			int j = 0;
			foreach(int value in list.Keys) {
				listRows[j] = value;
				j++;
			}
			Array.Sort(listRows);
			return listRows;
		}
		void AddControllerRowToDeletedHashtable(Dictionary<int, int> list, int controllerRow) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return;
			if(!list.ContainsKey(listSourceRow))
				list[listSourceRow] = 0;
		}
#if !SL
		public DetailInfo ExpandDetailRow(int controllerRow, int relationIndex) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return null;
			MasterRowInfo masterRow = ExpandedMasterRowCollection.Find(listSourceRow);
			DetailInfo info = masterRow == null ? null : masterRow.FindDetail(relationIndex);
			if(info == null) {
				IList list = GetDetailList(controllerRow, relationIndex);
				if(list == null) return null;
				if(masterRow == null)
					masterRow = ExpandedMasterRowCollection.CreateRow(this, listSourceRow, Helper.GetRowKey(listSourceRow));
				info = masterRow.CreateDetail(list, relationIndex);
			}
			if(DetailEmptyHash.ContainsKey(listSourceRow)) DetailEmptyHash.Remove(listSourceRow);
			masterRow.MakeDetailVisible(info);
			return info;
		}
		public IList GetDetailList(int controllerRow, int relationIndex) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return null;
			IList list = RelationSupport == null ? null : RelationSupport.GetDetailList(controllerRow, relationIndex);
			if(list == null) list = Helper.RelationList.GetDetailList(listSourceRow, relationIndex);
			return list;
		}
		public void CollapseDetailRow(int controllerRow, int relationIndex) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return;
			MasterRowInfo masterRow = ExpandedMasterRowCollection.Find(listSourceRow);
			DetailInfo info = masterRow == null ? null : masterRow.FindDetail(relationIndex);
			if(DetailEmptyHash.ContainsKey(listSourceRow)) DetailEmptyHash.Remove(listSourceRow);
			CollapseDetail(info);
		}
		public void CollapseDetail(DetailInfo detail) {
			if(detail == null) return;
			MasterRowInfo masterRow = detail.MasterRow;
			masterRow.Remove(detail);
			if(masterRow.Count == 0)
				ExpandedMasterRowCollection.Remove(masterRow);
		}
		bool collapsingRows = false;
		public override void CollapseDetailRows() {
			if(this.collapsingRows) return;
			this.collapsingRows = true;
			try {
				for(int n = ExpandedMasterRowCollection.Count - 1; n >= 0; n--) {
					MasterRowInfo masterRow = ExpandedMasterRowCollection[n];
					masterRow.Clear();
					ExpandedMasterRowCollection.RemoveAt(n);
				}
			}
			finally {
				this.collapsingRows = false;
			}
			DetailEmptyHash.Clear();
		}
#endif
		public Hashtable GetGroupSummary(int groupRowHandle) {
			GroupRowInfo group = GroupInfo.GetGroupRowInfoByHandle(groupRowHandle);
			if(group == null) return null;
			return GetGroupSummaryCore(group);
		}
		protected virtual Hashtable GetGroupSummaryCore(GroupRowInfo group) { return group.Summary; }
#if !SL
		public void CollapseDetailRow(int controllerRow) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return;
			MasterRowInfo masterRow = ExpandedMasterRowCollection.Find(listSourceRow);
			if(masterRow != null) {
				masterRow.Clear();
				ExpandedMasterRowCollection.Remove(masterRow);
			}
		}
		public void CollapseDetailRowByKey(object rowKey) {
			MasterRowInfo masterRow = ExpandedMasterRowCollection.FindByKey(rowKey);
			if(masterRow != null) {
				masterRow.Clear();
				ExpandedMasterRowCollection.Remove(masterRow);
			}
		}
		public void CollapseDetailRowByOwner(object detailOwner) {
			DetailInfo info = ExpandedMasterRowCollection.FindOwner(detailOwner);
			CollapseDetail(info);
		}
		public MasterRowInfo FindRowDetailInfo(object rowKey) {
			return ExpandedMasterRowCollection.FindByKey(rowKey);
		}
		public MasterRowInfo GetRowDetailInfo(int controllerRow) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return null;
			return ExpandedMasterRowCollection.Find(listSourceRow);
		}
		public bool IsDetailRowExpanded(int controllerRow) {
			return GetRowDetailInfo(controllerRow) != null;
		}
		public bool IsDetailRowExpanded(int controllerRow, int relationIndex) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return false;
			MasterRowInfo masterRow = ExpandedMasterRowCollection.Find(listSourceRow);
			return (masterRow == null ? null : masterRow.FindDetail(relationIndex)) != null;
		}
		public bool IsDetailRowEmptyCached(int controllerRow) { return IsDetailRowEmptyCached(controllerRow, -1); }
		public bool IsDetailRowEmptyCached(int controllerRow, int relationIndex) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return false;
			if(DetailEmptyHash.ContainsKey(listSourceRow)) return DetailEmptyHash[listSourceRow];
			bool res = true;
			if(relationIndex == -1) {
				int count = GetRelationCount(controllerRow);
				for(int n = 0; n < count; n++) {
					if(!IsDetailRowEmpty(controllerRow, n)) {
						res = false;
						break;
					}
				}
			}
			else
				res = IsDetailRowEmpty(controllerRow, relationIndex);
			DetailEmptyHash[listSourceRow] = res;
			return res;
		}
		public bool IsDetailRowEmpty(int controllerRow, int relationIndex) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return false;
			bool isEmpty = Helper.RelationList.IsMasterRowEmpty(listSourceRow, relationIndex);
			if(RelationSupport != null) isEmpty = RelationSupport.IsMasterRowEmpty(isEmpty, controllerRow, relationIndex);
			return isEmpty;
		}
		public bool IsDetailRow(int controllerRow) {
			if(IsGroupRowHandle(controllerRow) || !IsValidControllerRowHandle(controllerRow)) return false;
			return IsSupportMasterDetail;
		}
#endif
		public object GetRowKey(int controllerRow) {
			if(IsGroupRowHandle(controllerRow)) {
				return GroupInfo.GetGroupRowInfoByHandle(controllerRow);
			}
			int listSource = GetListSourceRowIndex(controllerRow);
			if(listSource == InvalidRow) return null;
			return Helper.GetRowKey(listSource);
		}
		public object GetGroupRowValue(int controllerRow) {
			GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
			if(group == null) return null;
			return GetGroupRowValue(group);
		}
		public object GetGroupRowValue(int controllerRow, DataColumnInfo column) {
			if(column == null) return null;
			GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
			if(group == null) return null;
			return GetGroupRowValue(group, column.Index);
		}
		int GetColumnByGroupLevel(GroupRowInfo group) {
			if(sortInfo.GroupCount < group.Level + 1) return -1;
			return SortInfo[group.Level].ColumnInfo.Index;
		}
		public virtual object GetGroupRowValue(GroupRowInfo group) {
			return GetGroupRowValue(group, -1);
		}
		protected virtual object GetGroupRowValue(GroupRowInfo group, int column) {
			int columnIndex = GetColumnByGroupLevel(group);
			if(group.GroupValue != null) {
				if(column < 0 || column == columnIndex) return group.GroupValue;
			}
			if(column < 0) return null;
			return GetRowValue(group.ChildControllerRow, column);
		}
		protected internal override object GetGroupRowKeyValueInternal(GroupRowInfo group) {
			int columnIndex = GetColumnByGroupLevel(group);
			if(columnIndex < 0) return null;
			return GetRowValue(group.ChildControllerRow, columnIndex);
		}
		public int GetRowLevel(int controllerRow) {
			if(!IsGrouped || controllerRow == InvalidRow) return 0;
			if(!IsGroupRowHandle(controllerRow)) {
				if(controllerRow == BaseListSourceDataController.NewItemRow) return 0;
				return GroupedColumnCount;
			}
			GroupRowInfo groupInfo = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
			if(groupInfo == null) return 0;
			return groupInfo.Level;
		}
		public object GetRow(int controllerRow) {
			return GetRow(controllerRow, null);
		}
		public virtual object GetRow(int controllerRow, OperationCompleted completed) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return null;
			if(listSourceRow < 0 || listSourceRow >= ListSourceRowCount) return null;
			return Helper.GetRow(listSourceRow, completed);
		}
		public object GetListSourceRow(int controllerRow) {
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return null;
			return Helper.GetRow(listSourceRow);
		}
		public object GetRowByListSourceIndex(int listSourceRow) {
			if(listSourceRow < 0) return null;
			return Helper.GetRow(listSourceRow);
		}
		public string GetRowDisplayText(int controllerRow, int column) {
			object value = GetRowValue(controllerRow, column);
			return (value == null ? "" : value.ToString());
		}
		public virtual int FindIncremental(string text, int columnHandle, int startRowHandle, bool down, bool ignoreStartRow, bool allowLoop, CompareIncrementalValue compareValue,  params OperationCompleted[] completed) {
			if(VisibleListSourceRowCount == 0) return InvalidRow;
			DataColumnInfo column = Columns[columnHandle];
			if(column == null) return InvalidRow;
			if(compareValue == null) compareValue = new CompareIncrementalValue(CompareValueDefault);
			int curRow = startRowHandle;
			bool firstRow = true;
			while(true) {
				if(firstRow && ignoreStartRow) {
					firstRow = false;
				}
				else {
					object val = GetRowValue(curRow, column);
					if(compareValue(curRow, val, text)) return curRow;
				}
				if(down) {
					curRow++;
					if(curRow >= VisibleListSourceRowCount) {
						if(!allowLoop) return InvalidRow;
						curRow = 0;
					}
				}
				else {
					curRow--;
					if(curRow == -1) {
						if(!allowLoop) return InvalidRow;
						curRow = VisibleListSourceRowCount - 1;
					}
				}
				if(curRow == startRowHandle) return InvalidRow;
			}
		}
		bool CompareValueDefault(int controllerRow, object val, string text) {
			string valText = (val == null || val == DBNull.Value) ? string.Empty : val.ToString();
			return StringStartsWith(valText, text);
		}
		public static bool StringStartsWith(string source, string part) {
			if(part.Length > source.Length) return false;
			for(int n = 0; n < part.Length; n++) {
				char c1 = source[n], c2 = part[n];
				if(c1 == c2) continue;
				if(Char.IsWhiteSpace(c1) && Char.IsWhiteSpace(c2)) continue;
				return false;
			}
			return true;
		}
		public virtual int FindRowByBeginWith(string columnName, string text) {
			return FindIncremental(text, Columns[columnName].Index, 0, true, false, false, null);
		}
		public virtual int FindRowByValue(string columnName, object value, params OperationCompleted[] completed) {
			if(!IsReady) return InvalidRow;
			DataColumnInfo column = Columns[columnName];
			if(column == null) return FindRowByRowValue(value);
			try {
				value = column.ConvertValue(value);
			}
			catch {
				return InvalidRow;
			}
			int count = VisibleListSourceRowCount;
			for(int n = 0; n < count; n++) {
				if(ValueComparer.Equals(GetRowValue(n, column), value)) return n;
			}
			return InvalidRow;
		}
		public int FindRowByValues(Dictionary<string, object> columnValues) {
			if(!IsReady) return InvalidRow;
			return FindRowByValues(Columns.ConvertValues(columnValues));
		}
		public virtual int FindRowByValues(Dictionary<DataColumnInfo, object> values) {
			if(!IsReady) return InvalidRow;
			if(values == null) return InvalidRow;
			int count = VisibleListSourceRowCount;
			for(int n = 0; n < count; n++) {
				if(IsRowEquals(values, n)) return n;
			}
			return InvalidRow;
		}
		bool IsRowEquals(Dictionary<DataColumnInfo, object> values, int listSourceIndex) {
			if(values.Count == 0) return false;
			foreach(DataColumnInfo column in values.Keys) {
				if(!ValueComparer.Equals(GetRowValue(listSourceIndex, column), values[column])) return false;
			}
			return true;
		}
		public virtual int FindRowByRowValue(object value) {
			if(!IsReady) return InvalidRow;
			int count = VisibleListSourceRowCount;
			for(int n = 0; n < count; n++) {
				if(object.Equals(GetRow(n), value)) return n;
			}
			return InvalidRow;
		}
		public virtual bool IsRowLoaded(int controllerRow) { return true; }
		public virtual void EnsureRowLoaded(int controllerRow, OperationCompleted completed) {
			if(completed != null) completed(GetRow(controllerRow));
		}
		public virtual void LoadRowHierarchy(int rowHandle, OperationCompleted completed) {
			if(completed != null) completed(true);
		}
		public virtual void LoadRow(int controllerRow) { GetRowValue(controllerRow, 0); }
		public object GetRowValue(int controllerRow, int column) { return GetRowValue(controllerRow, column, null); }
		public object GetRowValue(int controllerRow, string columnName) { return GetRowValue(controllerRow, columnName, null); }
		public virtual object GetRowValue(int controllerRow, int column, OperationCompleted completed) {
			if(!IsControllerCellValid(controllerRow, column)) return null;
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow < 0 || listSourceRow >= ListSourceRowCount) return null;
			return Helper.GetRowValue(listSourceRow, column, completed);
		}
		public object GetRowValue(int controllerRow, string columnName, OperationCompleted completed) {
			DataColumnInfo column = Columns[columnName];
			if(column == null) {
				column = DetailColumns[columnName];
				if(column != null) return GetRowValueDetail(controllerRow, column);
			}
			return GetRowValue(controllerRow, column, completed);
		}
		protected virtual object GetRowValueDetail(int controllerRow, DataColumnInfo column) {
			if(column == null) return null;
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow < 0 || listSourceRow >= ListSourceRowCount) return null;
			return Helper.GetRowValueDetail(listSourceRow, column);
		}
		public object GetRowValue(int controllerRow, DataColumnInfo column) { return GetRowValue(controllerRow, column, null); }
		public object GetRowValue(int controllerRow, DataColumnInfo column, OperationCompleted completed) {
			if(column == null) return null;
			if(column.owner == DetailColumns) return GetRowValueDetail(controllerRow, column);
			return GetRowValue(controllerRow, column.Index, completed);
		}
		public object GetListSourceRowValue(int listSourceRowIndex, string columnName) {
			DataColumnInfo column = Columns[columnName];
			if(column == null || listSourceRowIndex < 0) return null;
			return Helper.GetRowValue(listSourceRowIndex, column.Index, null);
		}
		public object GetListSourceRowValue(int listSourceRowIndex, int column) {
			if(!IsColumnValid(column) || listSourceRowIndex < 0) return null;
			return Helper.GetRowValue(listSourceRowIndex, column, null);
		}
		public void SetRowValue(int controllerRow, int column, object val) {
			SetRowValueCore(controllerRow, column, val);
		}
		public void RefreshRow(int rowHandle) {
			Helper.OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, GetListSourceRowIndex(rowHandle)));
		}
		public object GetValueEx(int controllerRow, string columnName) { return GetValueEx(controllerRow, columnName, null); }
		public virtual object GetValueEx(int controllerRow, string columnName, OperationCompleted completed) {
			if(!IsReady || !IsValidControllerRowHandle(controllerRow)) return null;
			DataColumnInfo column = Columns[columnName];
			if(column == null) return GetRow(controllerRow);
			return GetRowValue(controllerRow, column, completed);
		}
		protected virtual void SetRowValueCore(int controllerRow, int column, object val) {
			if(!IsControllerCellValid(controllerRow, column)) return;
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			Helper.SetRowValue(listSourceRow, column, val);
		}
		public void SetRowValue(int controllerRow, string columnName, object val) {
			SetRowValue(controllerRow, Columns[columnName], val);
		}
		public void SetRowValue(int controllerRow, DataColumnInfo column, object val) {
			if(column == null) return;
			SetRowValue(controllerRow, column.Index, val);
		}
		public virtual bool CanSort { get { return true; } }
		public virtual bool CanGroup { get { return true; } }
		public virtual bool CanFilter { get { return true; } }
		public bool CanSortColumn(string fieldName) {
			return CanSortColumn(Columns[fieldName]);
		}
		public bool CanSortColumn(int column) {
			if(!IsColumnValid(column)) return false;
			return CanSortColumn(Columns[column]);
		}
		public bool CanSortColumn(DataColumnInfo column) {
			return column != null && CanSortColumnCore(column);
		}
		protected virtual bool CanSortColumnCore(DataColumnInfo column) {
			return true;
		}
		protected virtual bool CanFindUnboundColumn(DataColumnInfo column) {
			return false;
		}
		public virtual bool CanFindColumn(DataColumnInfo column) {
			if(column == null) return false;
			if(!IsServerMode) return true;
			if(column.Unbound && !CanFindUnboundColumn(column)) return false;
			if(column.Type.IsEnum()) return false;
#if !SL
			if(!DataController.AllowFindNonStringTypesServerMode) {
				if(SummaryItemTypeHelper.IsNumericalType(column.Type)) return false;
				if(SummaryItemTypeHelper.IsDateTime(column.Type)) return false;
				if(SummaryItemTypeHelper.IsBool(column.Type)) return false;
			}
#endif
			return true;
		}
		public int GroupedColumnCount { get { return SortInfo.GroupCount; } }
		public bool IsSorted { get { return SortInfo.Count > 0; } }
		public bool IsGrouped { get { return SortInfo.GroupCount > 0; } }
		public int GetControllerRowByGroupRow(int controllerRow) {
			if(IsGroupRowHandle(controllerRow)) {
				GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
				controllerRow = group == null ? InvalidRow : group.ChildControllerRow;
			}
			return controllerRow;
		}
		public virtual int GetListSourceRowIndex(int controllerRow) {
			if(IsGroupRowHandle(controllerRow)) {
				GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow);
				controllerRow = group == null ? InvalidRow : group.ChildControllerRow;
			}
			if(!IsControllerRowValid(controllerRow)) return InvalidRow;
			return VisibleListSourceRows.GetListSourceRow(controllerRow);
		}
		public virtual bool IsControllerRowValid(int controllerRow) {
			return IsReady && controllerRow >= 0 && controllerRow < VisibleListSourceRowCount;
		}
		public bool IsControllerCellValid(int controllerRow, int column) {
			return IsControllerRowValid(controllerRow) && IsColumnValid(column);
		}
		protected override void RequestDataSyncInitialize() {
			if(DataSync != null) {
				DataSync.Initialize();
			}
		}
		protected override void SubscribeDataSync() {
			if(DataSync != null) DataSync.FilterSortGroupInfoChanged += new CollectionViewFilterSortGroupInfoChangedEventHandler(OnDataSync_FilterSortGroupInfoChanged);
		}
		protected override void UnsubscribeDataSync() {
			if(DataSync != null) DataSync.FilterSortGroupInfoChanged -= new CollectionViewFilterSortGroupInfoChangedEventHandler(OnDataSync_FilterSortGroupInfoChanged);
		}
		bool dataSyncInProgress = false;
		protected virtual bool IsDataSyncInProgress { get { return dataSyncInProgress; } }
		protected virtual void OnDataSync_FilterSortGroupInfoChanged(object sender, CollectionViewFilterSortGroupInfoChangedEventArgs e) {
			if(dataSyncInProgress || IsUpdateLocked || IsRefreshInProgress || DataSync == null || VisualClient == null) return;
			this.dataSyncInProgress = true;
			try {
				if(e.NeedRefresh)
					VisualClient.RequireSynchronization(DataSync);
				else {
					VisualClient.RequestSynchronization();
					OnRefreshed();
				}
			}
			finally {
				this.dataSyncInProgress = false;
			}
		}
		public override void RePopulateColumns(bool allowRefresh) {
			IList<DataColumnInfo> unusedColumns = Helper.RePopulateColumns();
			VisualClient.UpdateColumns();
			ResetFilterStub();
			bool prevHasUnbound = Columns.HasUnboundColumns;
			if(unusedColumns.Count > 0) {
				bool changed = SortInfo.RemoveUnusedColumns(unusedColumns);
				changed |= GroupSummary.RemoveUnusedColumns(unusedColumns);
				changed |= TotalSummary.RemoveUnusedColumns(unusedColumns);
				if(changed && allowRefresh) DoRefresh();
			}
			else {
				if(Columns.HasUnboundColumns || prevHasUnbound) {
					if(allowRefresh) DoRefresh();
				}
			}
		}
		protected bool IsRefreshInProgress { get { return refreshInProgress != 0; } }
		int refreshInProgress = 0;
		public GroupRowInfoCollection GroupInfo { get { return groupInfo; } }
		protected override void RaiseOnBeforeListChanged(ListChangedEventArgs e) {
			base.RaiseOnBeforeListChanged(e);
			switch(e.ListChangedType) {
				case ListChangedType.ItemDeleted:
				case ListChangedType.Reset:
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemMoved:
					break;
			}
		}
		protected void DoRefreshCore(bool useRowsKeeper) {
			OnPreRefresh(useRowsKeeper);
			OnRefresh(useRowsKeeper);
			OnPostRefresh(useRowsKeeper);
		}
		int prevSelectionCount = 0;
		protected virtual void OnPreRefresh(bool useRowsKeeper) {
			if(useRowsKeeper) RowsKeeper.Save();
			this.prevSelectionCount = Selection.InternalClear();
		}
		protected virtual bool AllowRebuildVisubleIndexesOnRefresh { get { return true; } }
		protected virtual void ClearVisibleInfoOnRefresh() {
			VisibleListSourceRows.Clear();
			VisibleIndexes.Clear();
			GroupInfo.Clear();
		}
		protected virtual void OnRefresh(bool useRowsKeeper) {
			DetailEmptyHash.Clear();
			ClearVisibleInfoOnRefresh();
			DoFilterRows();
			DoSortRows();
			DoGroupRows();
			CalcSummary();
			DoSortSummary();
		}
		protected virtual void OnPostRefresh(bool useRowsKeeper) {
			if(!IsListSourceChanging) {
				if(!useRowsKeeper) {
					if(RowsKeeper.RestoreIncremental()) BuildVisibleIndexes();
				}
				else {
					RestoreRowState();
				}
			}
			OnRefreshed();
			OnPostRefreshUpdateSelection();
		}
		protected void OnPostRefreshUpdateSelection() {
			if(Selection.Count != this.prevSelectionCount && Selection.Count == 0) Selection.RaiseSelectionChanged();
		}
		protected virtual void BeginInvoke(Delegate method) {
			method.DynamicInvoke();
		}
		protected override void OnListSourceChangeClear() {
			base.OnListSourceChangeClear();
			Selection.InternalClear();
			VisibleListSourceRows.Clear();
			VisibleIndexes.Clear();
			GroupInfo.Clear();
		}
		bool prevGrouped = false;
		protected override void DoRefresh(bool useRowsKeeper) {
			if(IsUpdateLocked) return;
			int prevVCount = IsListSourceChanging ? 0 : VisibleCount;
			this.refreshInProgress++;
			try {
				try {
					if(SortClient != null && IsSorted) SortClient.BeforeSorting();
					try {
						if(SortClient != null && (IsGrouped || this.prevGrouped)) SortClient.BeforeGrouping();
						DoRefreshCore(useRowsKeeper);
					}
					finally {
						if(SortClient != null && (IsGrouped || this.prevGrouped)) SortClient.AfterGrouping();
					}
				}
				finally {
					if(SortClient != null && IsSorted) SortClient.AfterSorting();
				}
			}
			finally {
				this.refreshInProgress--;
				this.prevGrouped = IsGrouped;
				VisualClientUpdateLayout();
				CheckRaiseVisibleCountChanged(prevVCount);
			}
		}
		protected virtual void CheckRaiseVisibleCountChanged(int prevVCount) {
			if(!StorePrevVisibleCount(VisibleCount) || IsListSourceChanging || prevVCount != VisibleCount) RaiseVisibleRowCountChanged();
		}
		protected virtual void OnRefreshed() {
			if(Refreshed != null) Refreshed(this, EventArgs.Empty);
		}
		public virtual void DoSortGroupRefresh() {
			if(IsUpdateLocked) return;
			SaveFilterCache();
			try {
				DoRefresh();
			}
			finally {
				ClearFilterCache();
			}
		}
		public string LastErrorText {
			get { return lastErrorText; }
			set {
				if(value == null) value = string.Empty;
				if(LastErrorText == value) return;
				lastErrorText = value;
				VisualClientUpdateLayout();
			}
		}
		public virtual bool PrefetchAllData(Function<bool> callbackMethod) {
			return true;
		}
		public IList GetAllFilteredAndSortedRows() { return GetAllFilteredAndSortedRows(null); }
		public virtual IList GetAllFilteredAndSortedRows(Function<bool> callbackMethod) {
			List<object> list = new List<object>();
			int vcount = VisibleListSourceRowCount;
			for(int n = 0; n < vcount; n++) {
				list.Add(GetRow(n));
				if(n % 1000 == 0 && callbackMethod != null) {
					if(callbackMethod()) return null;
				}
			}
			return list;
		}
		protected virtual void ClearFilterCache() {
			this.visibleListSourceRowsFilterCache = null;
		}
		protected virtual void SaveFilterCache() {
			this.visibleListSourceRowsFilterCache = VisibleListSourceRows.ClonePersistent();
		}
		protected override void VisualClientNotifyTotalSummary() {
			if(VisualClient2 != null) VisualClient2.TotalSummaryCalculated();
		}
		protected internal override void VisualClientUpdateLayout() {
			if(IsUpdateLocked) return;
			VisualClient.UpdateLayout();
		}
		public void ChangeGroupSorting(int groupLevel) {
			if(IsUpdateLocked) return;
			if(groupLevel < 0 || groupLevel >= GroupInfo.LevelCount) return;
			SortInfo.ChangeGroupSorting(groupLevel);
			GroupInfo.ReverseLevel(groupLevel);
			BuildVisibleIndexes();
			VisualClient.UpdateLayout();
		}
		public VisibleIndexCollection GetVisibleIndexes() { return VisibleIndexes; } 
		public int VisibleCount { get { return VisibleIndexes.IsEmpty ? VisibleListSourceRowCount : VisibleIndexes.Count; } }
		public override int GetControllerRow(int listSourceRow) {
			int? res = VisibleListSourceRows.GetVisibleRow(listSourceRow);
			return res ?? InvalidRow;
		}
		public int GetControllerRowHandle(int visibleIndex) {
			if(!IsReady || visibleIndex < 0 || visibleIndex >= VisibleCount) return InvalidRow;
			return VisibleIndexes.GetHandle(visibleIndex);
		}
		public int GetVisibleIndex(int controllerRowHandle) {
			if(VisibleIndexes.IsEmpty) return controllerRowHandle;
			return VisibleIndexes.IndexOf(controllerRowHandle);
		}
		public int GetVisibleIndexChecked(int controllerRowHandle) {
			if(VisibleIndexes.IsEmpty) {
				if(IsValidControllerRowHandle(controllerRowHandle)) return controllerRowHandle;
				return InvalidRow;
			}
			return VisibleIndexes.IndexOf(controllerRowHandle);
		}
		public virtual bool IsValidControllerRowHandle(int controllerRowHandle) {
			if(controllerRowHandle == InvalidRow) return false;
			if(IsGroupRowHandle(controllerRowHandle)) {
				return GroupInfo.GetGroupRowInfoByHandle(controllerRowHandle) != null;
			}
			return controllerRowHandle < VisibleListSourceRowCount;
		}
		public void MakeRowVisible(int controllerRowHandle) {
			if(!IsValidControllerRowHandle(controllerRowHandle)) return;
			GroupRowInfo groupRow = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRowHandle);
			if(groupRow == null) return;
			if(GroupInfo.MakeVisible(groupRow, !IsGroupRowHandle(controllerRowHandle))) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public bool IsRowVisible(int controllerRowHandle) {
			if(!IsValidControllerRowHandle(controllerRowHandle)) return false;
			if(VisibleIndexes.IsEmpty) return true;
			return VisibleIndexes.Contains(controllerRowHandle);
		}
		public void ExpandAll() {
			ChangeAllExpanded(true);
		}
		public bool IsRowExpanded(int groupRowHandle) {
			GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(groupRowHandle);
			return group == null ? false : group.Expanded;
		}
		public void CollapseAll() {
			ChangeAllExpanded(false);
		}
		public void ExpandRow(int groupRowHandle) {
			ExpandRow(groupRowHandle, false);
		}
		public void ExpandLevel(int groupLevel, bool recursive) {
			ChangeExpandedLevel(groupLevel, true, recursive);
		}
		public void CollapseLevel(int groupLevel, bool recursive) {
			ChangeExpandedLevel(groupLevel, false, recursive);
		}
		public void ExpandRow(int groupRowHandle, bool recursive) {
			ChangeExpanded(groupRowHandle, true, recursive);
		}
		public void CollapseRow(int groupRowHandle) {
			CollapseRow(groupRowHandle, false);
		}
		public void CollapseRow(int groupRowHandle, bool recursive) {
			ChangeExpanded(groupRowHandle, false, recursive);
		}
		protected virtual IDataErrorInfo GetRowErrorInfo(int controllerRow) {
			if(!IsControllerRowValid(controllerRow)) return null;
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			return Helper.GetRowErrorInfo(listSourceRow);
		}
		protected virtual DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo GetRowDXErrorInfo(int controllerRow) {
			if(!IsControllerRowValid(controllerRow)) return null;
			int listSourceRow = GetListSourceRowIndex(controllerRow);
			if(listSourceRow == InvalidRow) return null;
			return Helper.GetRowDXErrorInfo(listSourceRow);
		}
		public string GetErrorText(int controllerRow, string columnName) {
			return GetErrorText(controllerRow, Columns[columnName]);
		}
		public string GetErrorText(int controllerRow, DataColumnInfo column) {
			if(column == null) return "";
			return GetErrorText(controllerRow, column.Index);
		}
		public virtual string GetErrorText(int controllerRow) {
			return GetErrorInfo(controllerRow).ErrorText;
		}
		public DevExpress.XtraEditors.DXErrorProvider.ErrorType GetErrorType(int controllerRow) {
			DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info = GetErrorInfo(controllerRow);
			if(!string.IsNullOrEmpty(info.ErrorText)) return info.ErrorType;
			return DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorInfo GetErrorInfo(int controllerRow) {
			DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info = new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo();
			IDataErrorInfo errorInfo = GetRowErrorInfo(controllerRow);
			DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo dxErrorInfo = GetRowDXErrorInfo(controllerRow);
			if(errorInfo != null && dxErrorInfo == null) {
				string error = errorInfo.Error;
				if(!string.IsNullOrEmpty(error)) {
					info.ErrorText = error;
				}
			}
			if(dxErrorInfo != null) {
				dxErrorInfo.GetError(info);
			}
			return info;
		}
		public virtual string GetErrorText(int controllerRow, int column) {
			return GetErrorInfo(controllerRow, column).ErrorText;
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetErrorType(int controllerRow, int column) {
			DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info = GetErrorInfo(controllerRow, column);
			if(!string.IsNullOrEmpty(info.ErrorText)) return info.ErrorType;
			return DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorInfo GetErrorInfo(int controllerRow, int column) {
			DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info = new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo();
			if(column < 0 || column >= Columns.Count) return info;
			DataColumnInfo ci = Columns[column];
			if(ci.Unbound) return info;
			IDataErrorInfo errorInfo = GetRowErrorInfo(controllerRow);
			DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo dxErrorInfo = GetRowDXErrorInfo(controllerRow);
			if(errorInfo != null && dxErrorInfo == null) {
				string error = errorInfo[ci.Name];
				if(!string.IsNullOrEmpty(error)) {
					info.ErrorText = error;
				}
			}
			if(dxErrorInfo != null) {
				dxErrorInfo.GetPropertyError(ci.Name, info);
			}
			return info;
		}
#if !SL
		public MasterRowInfoCollection ExpandedMasterRowCollection { get { return expandedMasterRowCollection; } }
#endif
		protected internal VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		protected internal VisibleIndexCollection VisibleIndexes { get { return visibleIndexes; } }
		protected virtual void ChangeExpandedLevel(int groupLevel, bool expanded, bool recursive) {
			if(GroupInfo.ChangeExpandedLevel(groupLevel, expanded, recursive)) {
				ResetRowsKeeperEx();
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		protected virtual void ChangeExpanded(int groupRowHandle, bool expanded, bool recursive) {
			if(GroupInfo.ChangeExpanded(groupRowHandle, expanded, recursive)) {
				ResetRowsKeeperEx();
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
				RaiseVisibleRowCountChanged();				
			}
		}
		protected virtual void ChangeAllExpanded(bool expanded) {
			if(GroupInfo.ChangeAllExpanded(expanded)) {
				ResetRowsKeeperEx();
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
				RaiseVisibleRowCountChanged();
			}
		}
		protected virtual void BuildVisibleIndexes() { VisibleIndexes.BuildVisibleIndexes(VisibleCount, false, false); }
		protected virtual void OnFilterExpressionChanged() {
			ResetFilterStub();
			if(!IsUpdateLocked) {
				RowsKeeper.SaveIncremental();
			}
			VisibleListSourceRows.Clear();
			ClearFilterCache();
			DoRefresh(false);
			if(!IsUpdateLocked) RowsKeeper.Clear();
		}
		protected internal override void OnGroupDeleted(GroupRowInfo groupInfo) {
			Selection.OnGroupDeleted(groupInfo);
		}
		protected internal override void OnGroupsDeleted(List<GroupRowInfo> groups, bool addedToSameGroup) {
			foreach(GroupRowInfo info in groups) {
				if(!addedToSameGroup)
					OnGroupDeleted(info);
				else {
					Selection.OnReplaceGroupSelection(info, GroupInfo.GetGroupRowInfoByHandle(info.Handle));
				}
			}
		}
		protected internal void RaiseSelectionChanged(SelectionChangedEventArgs e) {
			if(SelectionChanged != null) SelectionChanged(this, e);
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			base.OnBindingListChanged(e);
			DetailEmptyHash.Clear();
			if(IsUpdateLocked) return;
			int prevVCount = VisibleCount;
			try {
				SuspendVisibleRowCountChanged();
				ClearFilterCache();
				OnBindingListChangedCore(e);
			}
			finally {
				ResumeVisibleRowCountChanged();
				if(!StorePrevVisibleCount(VisibleCount) || prevVCount != VisibleCount) RaiseVisibleRowCountChanged();
			}
		}
		protected virtual void OnBindingListChangedCore(ListChangedEventArgs e) {
			if(IsRefreshInProgress) return;
			DataControllerChangedItemCollection changedItems = CreateDataControllerChangedItemCollection();
			OnBindingListChangingStart();
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
					VisibleIndexes.SetDirty();
					OnItemAdded(e, changedItems);
					break;
				case ListChangedType.ItemDeleted:
					VisibleIndexes.SetDirty();
					OnItemDeleted(e, changedItems);
					break;
				case ListChangedType.ItemChanged:
					OnItemChanged(e, changedItems);
					break;
				case ListChangedType.ItemMoved:
					VisibleIndexes.SetDirty();
					OnItemMoved(e, changedItems);
					break;
				case ListChangedType.Reset:
					if(CollapseDetailRowsOnReset) CollapseDetailRows();
					VisibleIndexes.SetDirty();
					DoRefresh();
					break;
			}
			changedItems.UpdateVisibleIndexes(VisibleIndexes, false);
			if(VisibleIndexes.IsDirty) BuildVisibleIndexes();
			changedItems.UpdateVisibleIndexes(VisibleIndexes, true);
			OnVisibleIndexesUpdated();
			changedItems.AlwaysNotifyVisualClient = NotifyClientOnNextChange;
			changedItems.NotifyVisualClient(this, VisualClient);
			NotifyClientOnNextChange = false;
			OnBindingListChangingEnd();
		}
		protected virtual void OnVisibleIndexesUpdated() { }
		public virtual bool CollapseDetailRowsOnReset { get; set; }
		protected bool NotifyClientOnNextChange { get { return notifyClientOnNextChange; } set { notifyClientOnNextChange = value; } }
		protected virtual void OnBindingListChangingEnd() { }
		protected virtual void OnBindingListChangingStart() { }
		protected virtual void OnItemMoved(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			if(e.OldIndex < 0 && e.NewIndex < 0) return;
			if(e.OldIndex < 0) {
				OnItemAdded(new ListChangedEventArgs(ListChangedType.ItemAdded, e.NewIndex, -1), changedItems);
				return;
			}
			if(e.NewIndex < 0) {
				OnItemDeleted(new ListChangedEventArgs(ListChangedType.ItemDeleted, e.NewIndex, e.OldIndex), changedItems);
				return;
			}
			if(VisibleListSourceRows.Contains(e.OldIndex)) {
				VisibleListSourceRows.MoveSourcePosition(e.OldIndex, e.NewIndex);
			}
			OnActionItemMoved(e.OldIndex, e.NewIndex);
			changedItems.AddItem(e.OldIndex, NotifyChangeType.ItemDeleted, null);
			changedItems.AddItem(e.NewIndex, NotifyChangeType.ItemAdded, null);
		}
		protected virtual void OnActionItemMoved(int oldIndex, int newIndex) {
#if !SL
			ExpandedMasterRowCollection.OnItemMoved(oldIndex, newIndex);
#endif
			Selection.OnItemMoved(oldIndex, newIndex);
		}
		protected virtual void OnActionItemDeleted(int index) {
#if !SL
			ExpandedMasterRowCollection.OnItemDeleted(index);
#endif
			Selection.OnItemDeleted(index);
		}
		protected virtual void OnActionItemAdded(int index) {
#if !SL
			ExpandedMasterRowCollection.OnItemAdded(index);
#endif
			Selection.OnItemAdded(index);
		}
		bool OnFilteredItemChanged(int listSourceRow, DataControllerChangedItemCollection changedItems) {
			if(!IsFiltered) return false;
			bool wasInList = VisibleListSourceRows.Contains(listSourceRow);
			if(!IsImmediateUpdateRowPosition) return !wasInList;
			Selection.LockAddRemoveAction();
			try {
			bool fit = IsRowFit(listSourceRow);
			if(!wasInList) {
				if(fit) {
					OnItemAddedCore(listSourceRow, changedItems, false);
				}
				VisibleIndexes.SetDirty();
				return true;
			}
			if(!fit) {
				int controllerRow = VisibleListSourceRows.HideSourceRow(listSourceRow);
				OnItemDeletedCore(controllerRow, changedItems);
				UpdateTotalSummaryOnItemFilteredOut(listSourceRow);
				OnActionItemDeleted(listSourceRow);
					Selection.OnItemFilteredOut(listSourceRow);
				VisibleIndexes.SetDirty();
				return true;
			}
			}
			finally {
				Selection.UnLockAddRemoveAction();
			}
			return false;
		}
		protected virtual void UpdateTotalSummaryOnItemFilteredOut(int listSourceRow) {
		}
		protected virtual void OnItemChanged(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			int listSourceRow = GetChangedListSourceRow(e);
			if(listSourceRow < 0) return;
			if(OnFilteredItemChanged(listSourceRow, changedItems)) return;
			int oldControllerRow = VisibleListSourceRows.GetVisibleRow(listSourceRow) ?? InvalidRow;
			bool immediateUpdateRowPosition = CheckImmediateUpdateRowPosition(e);
			if(immediateUpdateRowPosition && IsSortBySummary) immediateUpdateRowPosition = false;
			if(immediateUpdateRowPosition) {
				int newControllerRow = FindControllerRowForInsert(listSourceRow, oldControllerRow);
				VisibleListSourceRowMove(oldControllerRow, ref newControllerRow, changedItems, false);
				GroupInfo.DoRowChanged(VisibleIndexes, oldControllerRow, newControllerRow, changedItems);
			}
			else {
				if(IsGrouped && GroupSummary.Count > 0) {
					GroupRowInfo groupRowInfo = GroupInfo.GetGroupRowInfoByControllerRowHandle(oldControllerRow);
					UpdateGroupSummary(groupRowInfo, changedItems);
				}
				if(changedItems != null) changedItems.AddItem(oldControllerRow, NotifyChangeType.ItemChanged, null);
			}
			UpdateTotalSummaryOnItemChanged(listSourceRow, e.PropertyDescriptor != null ? e.PropertyDescriptor.Name : string.Empty);
		}
		protected virtual void UpdateTotalSummaryOnItemChanged(int listSourceRow, string propertName) {
			CheckUpdateTotalSummary();
		}
		protected internal virtual bool CheckImmediateUpdateRowPosition(ListChangedEventArgs e) {
			if(!IsImmediateUpdateRowPosition) return false;
			if(e.PropertyDescriptor == null) return true;
			if(SortInfo.Count == 0) return false;
			if(!SortInfo.Contains(e.PropertyDescriptor.Name)) return false;
			return true;
		}
		protected virtual void VisibleListSourceRowMove(int oldControllerRow, ref int newControllerRow, DataControllerChangedItemCollection changedItems, bool isAdding) {
			if(oldControllerRow == DataController.InvalidRow) return;
			if(newControllerRow > oldControllerRow)
				newControllerRow--;
			if(oldControllerRow == newControllerRow || VisibleListSourceRows.VisibleRowCount < 2) {
				if(isAdding)
					MakeVisibleIndexesDirty(visibleIndexes);
				if(changedItems != null)
					changedItems.AddItem(oldControllerRow, isAdding ? NotifyChangeType.ItemAdded : NotifyChangeType.ItemChanged, null);
				return;
			}
			MakeVisibleIndexesDirty(visibleIndexes);
			VisibleListSourceRows.MoveVisiblePosition(oldControllerRow, newControllerRow);
			if(changedItems != null) {
				if(!isAdding)
					changedItems.AddItem(oldControllerRow, NotifyChangeType.ItemDeleted, null);
				changedItems.AddItem(newControllerRow, NotifyChangeType.ItemAdded, null);
			}
		}
		static void MakeVisibleIndexesDirty(VisibleIndexCollection visibleIndexes) {
			if(visibleIndexes != null)
				visibleIndexes.SetDirty();
		}
		void OnItemDeleted(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			int listSourceRow = GetChangedListSourceRow(e);
			OnItemDeletedCore(VisibleListSourceRows.RemoveSourceRow(listSourceRow), changedItems);
			OnActionItemDeleted(listSourceRow);
		}
		protected virtual void OnItemDeletedCore(int controllerRow, DataControllerChangedItemCollection changedItems) {
			if(controllerRow >= 0) {
				changedItems.AddItem(controllerRow, NotifyChangeType.ItemDeleted, GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRow));
				GroupInfo.DoRowDeleted(controllerRow, changedItems);
			}
			UpdateTotalSummaryOnItemDeleted(controllerRow);
		}
		protected virtual void UpdateTotalSummaryOnItemDeleted(int controllerRow) {
			CheckUpdateTotalSummary();
		}
		void OnItemAdded(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			int listSourceRow = GetChangedListSourceRow(e);
			OnItemAddedCore(listSourceRow, changedItems, true);
			OnActionItemAdded(listSourceRow);
		}
		protected virtual bool CanFilterAddedRow(int listSourceRow) { return true; }
		protected virtual void OnItemAddedCore(int listSourceRow, DataControllerChangedItemCollection changedItems, bool rowInserted) {
			if(CanFilterAddedRow(listSourceRow)) {
				if(!IsRowFit(listSourceRow)) {
					VisibleListSourceRows.InsertHiddenRow(listSourceRow);
					return;
				}
			}
			if(IsSorted || IsFiltered) {
				GroupInfo.DoRowAdded(ShowRow(listSourceRow, changedItems, rowInserted), changedItems);
			}
			else changedItems.AddItem(listSourceRow, NotifyChangeType.ItemAdded, null);
			UpdateTotalSummaryOnItemAdded(listSourceRow);
		}
		protected virtual void UpdateTotalSummaryOnItemAdded(int listSourceRow) {
			CheckUpdateTotalSummary();
		}
		int ShowRow(int dataIndex, DataControllerChangedItemCollection changedItems, bool rowInserted) {
			if(rowInserted)
				VisibleListSourceRows.InsertVisibleRow(dataIndex, VisibleListSourceRows.VisibleRowCount);
			else
				VisibleListSourceRows.ShowRow(dataIndex, VisibleListSourceRows.VisibleRowCount);
			int newControllerRow = VisibleListSourceRows.VisibleRowCount - 1;
			if(IsImmediateUpdateRowPosition)
				newControllerRow = FindControllerRowForInsert(dataIndex);
			VisibleListSourceRowMove(VisibleListSourceRows.VisibleRowCount - 1, ref newControllerRow, changedItems, true);
			return newControllerRow;
		}
		int FindControllerRowForInsert(int listSourceRow, int? oldControllerRow = null) {
			return VisibleListSourceRows.FindControllerRowForInsert(GetInterceptSortInfo(), listSourceRow, oldControllerRow);
		}
		bool? IsRowUserFit(int listSourceRow, bool fit) {
			return DataClient2 == null ? null : DataClient2.IsRowFit(listSourceRow, fit);
		}
		protected virtual bool IsRowFit(int listSourceRow) {
			FilterRowStub stub = FilterStub;
			stub.GoTo(listSourceRow);
			try {
				bool fit = stub.Filter();
				bool? user = IsRowUserFit(listSourceRow, fit);
				return user ?? fit;
			}
			finally {
				stub.Reset();
			}
		}
		[Browsable(false)]
		public bool ByPassFilter = false;
		protected virtual void DoFilterRows() {
			if(ByPassFilter) return;
			if(!IsFiltered) return;
			if(this.visibleListSourceRowsFilterCache != null &&
				this.visibleListSourceRowsFilterCache.AppliedFilterExpression == FilterExpression &&
				this.visibleListSourceRowsFilterCache.HasUserFilter == HasUserFilter) {
				if(ListSourceRowCount >= this.visibleListSourceRowsFilterCache.VisibleRowCount) {
					this.visibleListSourceRows = this.visibleListSourceRowsFilterCache.CloneThatWouldBeForSureModifiedAndOrForgottenBeforeAnythingHappensToOriginal();
					return;
				}
				ClearFilterCache();
			} else {
				ClearFilterCache();
			}
			int fitCount = 0;
			int[] fitList = new int[ListSourceRowCount];
			FilterRowStub stub = FilterStub;
			try {
				for(int i = 0; i < fitList.Length; i++) {
					stub.GoTo(i);
					bool completeFit;
					try {
						bool fit = stub.Filter();
						bool? user = IsRowUserFit(i, fit);
						completeFit = user ?? fit;
					} catch {
						completeFit = false;
					}
					if(!completeFit)
						continue;
					fitList[fitCount++] = i;
				}
			} finally {
				stub.Reset();
			}
			VisibleListSourceRows.Init(fitList, fitCount, FilterExpression, HasUserFilter);
		}
		protected override internal PropertyDescriptorCollection GetFilterDescriptorCollection() {
			if(SortClient == null) return Helper.DescriptorCollection;
			Dictionary<string, PropertyDescriptor> pds = new Dictionary<string, PropertyDescriptor>();
			for(int n = 0; n < Columns.Count; n++) {
				if(SortClient.RequireDisplayText(Columns[n]))
					AddPropertyDescriptorToDictionary(pds, new DevExpress.Data.Access.DisplayTextPropertyDescriptor(this, Columns[n]));
				else
					AddPropertyDescriptorToDictionary(pds, Columns[n].PropertyDescriptor);
			}
			if(!IsServerMode) {
				foreach(string name in SortClient.GetFindByPropertyNames()) {
					DataColumnInfo col = Columns[name];
					if(col == null) continue;
					string pname = DevExpress.Data.Access.DisplayTextPropertyDescriptor.DxFtsPropertyPrefix + name;
					AddPropertyDescriptorToDictionary(pds, new DevExpress.Data.Access.DisplayTextPropertyDescriptor(this, col, pname));
				}
			}
			foreach(PropertyDescriptor pd in Helper.DescriptorCollection) {
				AddPropertyDescriptorToDictionary(pds, pd);
			}
			PropertyDescriptor[] array = new PropertyDescriptor[pds.Count];
			pds.Values.CopyTo(array, 0);
			return new PropertyDescriptorCollection(array);
		}
		void AddPropertyDescriptorToDictionary(Dictionary<string, PropertyDescriptor> dictionary, PropertyDescriptor pd) {
			if(dictionary.ContainsKey(pd.Name)) return;
			dictionary.Add(pd.Name, pd);
		}
		SubstituteSortInfoEventArgs sortInfoInterceptor;
		internal DataColumnSortInfo[] GetInterceptSortInfo() {
			var rv = this.SortInfo.ToArray();
			if(SortInfo.Count == 0)
				return rv;
			ValidateSortInfosBeforeIntercept(rv);
			if(SortClient == null)
				return rv;
			if(sortInfoInterceptor == null)
				sortInfoInterceptor = new SubstituteSortInfoEventArgs();
			sortInfoInterceptor.SortInfo = rv;
			sortInfoInterceptor.GroupCount = this.SortInfo.GroupCount;
			DataColumnSortInfo[] patched;
			try {
				SortClient.SubstituteSortInfo(sortInfoInterceptor);
				patched = sortInfoInterceptor.SortInfo;
			} finally {
				sortInfoInterceptor.SortInfo = null;
				sortInfoInterceptor.GroupCount = 0;
			}
			ValidateSortInfosAfterIntercept(patched);
			return patched;
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		void ValidateSortInfosAfterIntercept(DataColumnSortInfo[] patched) {
			ValidateSortInfos(patched);
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		void ValidateSortInfosBeforeIntercept(DataColumnSortInfo[] rv) {
			ValidateSortInfos(rv);
		}
		void ValidateSortInfos(DataColumnSortInfo[] infos) {
			foreach(var si in infos)
				Columns.ValidateColumnInfo(si.ColumnInfo);
		}
		protected virtual void DoSortRows() {
			if(!IsSorted) return;
			VisibleListSourceRows.SortRows(GetInterceptSortInfo());
		}
		protected virtual void DoGroupRows() {
			if(!IsGrouped) return;
			DoGroupColumn(SortInfo.Clone(), GroupInfo, 0, VisibleListSourceRowCount, null);
			GroupInfo.UpdateIndexes();
			BuildVisibleIndexes();
		}
		protected virtual bool IsSortBySummary {
			get {
				if(!IsGrouped || SummarySortInfo.Count == 0 || GroupInfo.Count == 0) return false;
				return true;
			}
		}
		protected virtual void DoSortSummary() {
			if(!IsSortBySummary) return;
			GroupRowInfoCollection groups = CreateGroupRowInfoCollection();
			DoSortSummary(groups, null);
			this.groupInfo = groups;
			this.groupInfo.UpdateIndexes();
			RebuildVisibleListSourceRows(GroupInfo);
			BuildVisibleIndexes();
		}
		void RebuildVisibleListSourceRows(GroupRowInfoCollection groupInfo) {
			int[] records = new int[VisibleListSourceRows.VisibleRowCount];
			int record = 0;
			for(int n = 0; n < groupInfo.Count; n++) {
				if(record == records.Length) break; 
				var group = groupInfo[n];
				int currentRow = group.ChildControllerRow;
				group.ChildControllerRow = record;
				if(!groupInfo.IsLastLevel(group)) continue;
				for(int r = 0; r < group.ChildControllerRowCount; r++) {
					records[record++] = VisibleListSourceRows.GetListSourceRow(r + currentRow);
				}
			}
			VisibleListSourceRows.Assign(records);
		}
		protected virtual void DoSortSummary(GroupRowInfoCollection groups, GroupRowInfo parentGroup) {
			List<GroupRowInfo> list = new List<GroupRowInfo>();
			SummarySortInfo sortInfo = SummarySortInfo.GetByLevel(parentGroup == null ? 0 : parentGroup.Level + 1);
			GroupInfo.GetChildrenGroups(parentGroup, list);
			if(sortInfo != null) {
				foreach(GroupRowInfo rowInfo in list) {
					RequestSummary(rowInfo);
				}
				list.Sort(new GroupSummaryComparer(this, sortInfo));
			}
			for(int n = 0; n < list.Count; n++) {
				GroupRowInfo row = list[n] as GroupRowInfo;
				groups.Add(row);
				if(!GroupInfo.IsLastLevel(row)) DoSortSummary(groups, row);
			}
		}
		protected virtual GroupRowInfo RequestSummary(GroupRowInfo rowInfo) { return rowInfo; }
		protected void CheckUpdateTotalSummary() {
			if(!AutoUpdateTotalSummary) return;
			TotalSummary.SetDirty();
		}
		public override void UpdateTotalSummary(List<SummaryItem> changedItems) {
			if(IsUpdateLocked) return;
			TotalSummary.IsDirty = false;
			IList summary = changedItems == null ? (IList)TotalSummary : (IList)changedItems;
			for(int n = 0; n < summary.Count; n++) {
				CalcTotalSummaryItem(summary[n] as SummaryItem);
			}
			VisualClientNotifyTotalSummary();
		}
		protected virtual void CalcTotalSummaryItem(SummaryItem summary) {
			bool valid = true;
			summary.SummaryValue = CalcSummaryInfo(null, summary, ref valid);
			summary.Exists = valid;
		}
		protected virtual SelectedRowsKeeper CreateSelectionKeeper() { return new SelectedRowsKeeper(this, true); }
		public virtual ListSourceRowsKeeper CreateControllerRowsKeeper() { return new ListSourceRowsKeeper(this, CreateSelectionKeeper()); }
		protected override void Reset() {
			this.DetailEmptyHash.Clear();
			this.TotalSummary.Reset();
			this.GroupSummary.Reset();
			this.SortInfo.Reset();
			this.filterCriteria = null;
			ResetFilterStub();
			RowsKeeper.Clear();
			ClearFilterCache();
		}
		protected internal override void VisualClientRequestSynchronization() {
			VisualClient.RequestSynchronization();
		}
		protected void OnSortSummaryCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(IsUpdateLocked) return;
			if(SummarySortInfo.Count == 0)
				DoRefresh();
			else {
				DoSortSummary();
				VisualClientUpdateLayout();
			}
		}
		protected virtual void OnGroupSummaryCollectionChanged(object sender, CollectionChangeEventArgs e) {
			UpdateGroupSummary();
			if(e.Action != CollectionChangeAction.Add && SummarySortInfo.CheckSummaryCollection(GroupSummary))
				DoRefresh();
			else {
				VisualClientUpdateLayout();
			}
		}
		protected void OnSortInfoCollectionChanged(object sender, CollectionChangeEventArgs e) {
			DoRefresh();
		}
		protected virtual void OnTotalSummaryCollectionChanged(object sender, CollectionChangeEventArgs e) {
			UpdateTotalSummary();
			VisualClientUpdateLayout();
		}
		public void CalcSummary() {
			CalcGroupSummary();
			CalcTotalSummary();
		}
		protected virtual void CalcTotalSummary() {
			UpdateTotalSummary();
		}
		protected virtual void CalcGroupSummary() {
			if(IsUpdateLocked) return;
			GroupInfo.ClearSummary();
			for(int n = 0; n < GroupSummary.Count; n++) {
				CalcGroupSummaryItem(GroupSummary[n]);
			}
		}
		public void UpdateGroupSummary() { UpdateGroupSummary(null); }
		public virtual void UpdateGroupSummary(List<SummaryItem> changedItems) {
			CalcGroupSummary();
		}
		protected virtual void CalcGroupSummaryItem(SummaryItem summary) {
			IEnumerable<GroupRowInfo> groups;
			if(this.SortInfo.GroupCount <= 1)
				groups = GroupInfo;
			else
				groups = GroupInfo.OrderByDescending(g => g.Level).ThenBy(g => g.Index);
			foreach(var g in groups)
				CalcGroupSummaryItem(summary, g);
		}
		protected virtual void CalcGroupSummaryItem(SummaryItem summary, GroupRowInfo groupRow) {
			bool valid = true;
			object val = CalcSummaryInfo(groupRow, summary, ref valid);
			if(valid)
				groupRow.SetSummaryValue(summary, val);
			else
				groupRow.ClearSummaryItem(summary);
		}
		public override void UpdateGroupSummary(GroupRowInfo groupRow, DataControllerChangedItemCollection changedItems) {
			if(GroupSummary.Count == 0) return;
			while(groupRow != null) {
				for(int i = 0; i < GroupSummary.Count; i++) {
					CalcGroupSummaryItem(GroupSummary[i], groupRow);
				}
				if(changedItems != null) changedItems.AddItem(groupRow.Handle, NotifyChangeType.ItemChanged, groupRow.ParentGroup);
				groupRow = groupRow.ParentGroup;
			}
		}
		static IEnumerable<string> GetSummaryAuxInfo(SummaryItem summaryItem, bool ignoreNullValues) {
			yield return "ignoreNullValues: " + ignoreNullValues;
			yield return "summaryItem SummaryType: " + summaryItem.SummaryType;
			yield return "summaryItem FieldName: '" + summaryItem.FieldName + "'";
			if(summaryItem.ColumnInfo == null) {
				yield return "summaryItem ColumnInfo: none";
			} else {
				foreach(var str in BaseListDataControllerHelper.GetColumnAuxInfo(summaryItem.ColumnInfo))
					yield return str;
			}
		}
		protected virtual object CalcSummaryValue(SummaryItem summaryItem, GroupRowInfo groupRow) {
			int controllerRow, rowCount;
			if(groupRow == null) {
				controllerRow = 0;
				rowCount = VisibleListSourceRowCount;
			} else {
				controllerRow = groupRow.ChildControllerRow;
				rowCount = groupRow.ChildControllerRowCount;
			}
			Func<int, int> fastGetListSourceRowIndex =  
				visibleIndex => VisibleListSourceRows.GetListSourceRow(visibleIndex);
			if(summaryItem.IsCustomSummary) {
				if(CustomSummary == null)
					return null;
				var valueGetter = (Func<int, object>)Helper.GetGetRowValue(summaryItem.ColumnInfo, typeof(object));
				for(int n = controllerRow; n < controllerRow + rowCount; ++n) {
					var val = valueGetter(fastGetListSourceRowIndex(n));
					SummaryCalculateArgs.SetupCell(n, val);
					CustomSummary(summaryItem, SummaryCalculateArgs);
				}
				return SummaryCalculateArgs.TotalValue;
			} else {
				var ignoreNullValues = summaryItem.IgnoreNullValues(SummariesIgnoreNullValues);
				var associativeSummaryShortcut = GetShortcutSummaryEnumerable(summaryItem, groupRow);
				if(associativeSummaryShortcut != null) {
					Func<string[]> exceptionAuxInfoGetter = () => new string[] { "associativeSummaryShortcut: exists" }.Concat(GetSummaryAuxInfo(summaryItem, ignoreNullValues)).ToArray();
					return CalcSummaryValue(summaryItem, summaryItem.SummaryType, ignoreNullValues, typeof(object), associativeSummaryShortcut, exceptionAuxInfoGetter, groupRow);
				} else {
					var valueType = ExpressiveSortHelpers.GetCompareType(summaryItem.ColumnInfo);
					var valueGetter = Helper.GetGetRowValue(summaryItem.ColumnInfo, valueType);
					var rowHandles = Enumerable.Range(controllerRow, rowCount);
					var listSourceIndices = rowHandles.Select(fastGetListSourceRowIndex);
					var valuesEnumerable = listSourceIndices.ApplySelect(valueGetter, valueType);
					Func<string[]> exceptionAuxInfoGetter = () => new string[] { "controllerRow: " + controllerRow, "rowCount: " + rowCount, "valueType: " + valueType.Name }.Concat(GetSummaryAuxInfo(summaryItem, ignoreNullValues)).ToArray();
					return CalcSummaryValue(summaryItem, summaryItem.SummaryType, ignoreNullValues, valueType, valuesEnumerable, exceptionAuxInfoGetter, groupRow);
				}
			}
		}
		protected virtual object CalcSummaryValue(SummaryItem summaryItem, SummaryItemType summaryType, bool ignoreNullValues, Type valueType, IEnumerable valuesEnumerable, Func<string[]> exceptionAuxInfoGetter, GroupRowInfo groupRow) {
			return SummaryValueExpressiveCalculator.Calculate(summaryType, valuesEnumerable, valueType, ignoreNullValues, summaryItem.ColumnInfo.CustomComparer, exceptionAuxInfoGetter);
		}
		protected virtual bool IsAssociativeSummary(SummaryItemType summaryType) {
			switch(summaryType) {
				case SummaryItemType.Max:
				case SummaryItemType.Min:
				case SummaryItemType.Sum:
					return true;
				default:
					return false;
			}
		}
		IEnumerable<SummaryItem> FindMatchingGroupSummaries(SummaryItem summaryItem) {
			foreach(SummaryItem si in GroupSummary) {
				if(ReferenceEquals(si, summaryItem)) {
					yield return si;
					break;
				}
			}
			foreach(SummaryItem si in GroupSummary) {
				if(ReferenceEquals(si, summaryItem))
					continue;
				if(si.SummaryType != summaryItem.SummaryType)
					continue;
				if(!ReferenceEquals(si.ColumnInfo, summaryItem.ColumnInfo))
					continue;
				if(si.IgnoreNullValues(SummariesIgnoreNullValues) != summaryItem.IgnoreNullValues(SummariesIgnoreNullValues))
					continue;
				yield return si;
			}
		}
		IEnumerable<object> GetShortcutSummaryEnumerable(SummaryItem summaryItem, GroupRowInfo groupRow) {
			if(this.VisibleListSourceRowCount == 0)
				return null;
			if(!IsAssociativeSummary(summaryItem.SummaryType))
				return null;
			int currentLevel = groupRow != null ? groupRow.Level : -1;
			if(this.SortInfo.GroupCount <= currentLevel + 1)
				return null;
			List<GroupRowInfo> subGroups = null;
			foreach(SummaryItem matchingSi in FindMatchingGroupSummaries(summaryItem)) {
				if(subGroups == null) {
					subGroups = new List<GroupRowInfo>();
					this.GroupInfo.GetChildrenGroups(groupRow, subGroups);
					if(subGroups.Count == 0)
						return null;
				}
				List<object> subAggregates = null;
				foreach(GroupRowInfo gri in subGroups) {
					bool isValid;
					var subValue = GetSummaryShortcut(gri, matchingSi, out isValid);
					if(!isValid) {
						subAggregates = null;
						break;
					}
					if(subAggregates == null)
						subAggregates = new List<object>(subGroups.Count);
					subAggregates.Add(subValue);
				}
				if(subAggregates != null)
					return subAggregates;
			}
			return null;
		}
		protected virtual object GetSummaryShortcut(GroupRowInfo groupRowInfo, SummaryItem summaryItem, out bool isValid) {
			return groupRowInfo.GetSummaryValue(summaryItem, out isValid);
		}
		protected bool IsSummaryExists(GroupRowInfo groupRow, SummaryItem summaryItem) {
			if(CustomSummaryExists != null) {
				CustomSummaryExistEventArgs existArgs = CreateCustomSummaryExistEventArgs(groupRow, summaryItem.Key);
				CustomSummaryExists(this, existArgs);
				if(!existArgs.Exists) {
					return false;
				}
			}
			return true;
		}
		protected virtual object CalcSummaryInfo(GroupRowInfo groupRow, SummaryItem summaryItem, ref bool validResult) {
			validResult = true;
			if(summaryItem.IsNoneSummary)
				return null;
			if(!summaryItem.AllowCalculate)
				return null;
			if(!IsSummaryExists(groupRow, summaryItem)) {
				validResult = false;
				return null;
			}
			if(summaryItem.SummaryType == SummaryItemType.Count) {
				if(groupRow == null)
					return this.VisibleListSourceRowCount;
				else
					return groupRow.ChildControllerRowCount;
			}
			SummaryCalculateArgs.Setup(0, null, null, groupRow, CustomSummaryProcess.Start, summaryItem.Key);
			if(summaryItem.IsListBasedSummary) CalcListBasedSummary(summaryItem, SummaryCalculateArgs);
			if(summaryItem.IsCustomSummary && CustomSummary != null) CustomSummary(this, SummaryCalculateArgs);
			object result = null;
			if(!SummaryCalculateArgs.TotalValueReady && summaryItem.ColumnInfo != null) {
				if(!IsServerMode) {
					 SummaryCalculateArgs.SetupSummaryProcess(CustomSummaryProcess.Calculate);
					 result = CalcSummaryValue(summaryItem, groupRow);
				}
			}
			else
				result = SummaryCalculateArgs.TotalValue;
			if(summaryItem.IsCustomSummary && CustomSummary != null) {
				SummaryCalculateArgs.SetupSummaryProcess(CustomSummaryProcess.Finalize);
				SummaryCalculateArgs.TotalValue = result;
				CustomSummary(this, SummaryCalculateArgs);
				result = SummaryCalculateArgs.TotalValue;
			}
			return result;
		}
		protected virtual void CalcListBasedSummary(SummaryItem summaryItem, CustomSummaryEventArgs e) {
			e.TotalValue = null;
			e.TotalValueReady = true;
			if(summaryItem.ColumnInfo == null || !CanSortColumn(summaryItem.ColumnInfo)) return;
			if(summaryItem.SummaryArgument <= 0 && (summaryItem.SummaryTypeEx != SummaryItemTypeEx.Duplicate && summaryItem.SummaryTypeEx != SummaryItemTypeEx.Unique)) return;
			using(VisibleListSourceRowCollection list = VisibleListSourceRows.CloneThatWouldBeForSureModifiedAndOrForgottenBeforeAnythingHappensToOriginal()) {
				List<object> values = ProcessListBasedSummary(list, summaryItem);
				e.TotalValue = values;
			}
		}
		List<object> ProcessListBasedSummary(VisibleListSourceRowCollection list, SummaryItem summaryItem) {
			if(summaryItem.SummaryTypeEx == SummaryItemTypeEx.Duplicate || summaryItem.SummaryTypeEx == SummaryItemTypeEx.Unique) {
				return ProcessListBasedSummaryDupUni(list, summaryItem);
			}
			var sortInfo = new DataColumnSortInfo[] {
				new DataColumnSortInfo(summaryItem.ColumnInfo, summaryItem.SummaryTypeEx == SummaryItemTypeEx.Top || summaryItem.SummaryTypeEx == SummaryItemTypeEx.TopPercent ? ColumnSortOrder.Descending : ColumnSortOrder.Ascending)
			};
			list.SortRows(sortInfo);
			int targetCount = (int)summaryItem.SummaryArgument;
			if(summaryItem.IsPercentArgument) {
				targetCount = (int)(((decimal)list.VisibleRowCount / 100) * summaryItem.SummaryArgument);
			}
			List<object> values = new List<object>();
			for(int n = 0; n < Math.Min(targetCount, list.VisibleRowCount); n++) {
				values.Add(GetListSourceRowValue(list.GetListSourceRow(n), summaryItem.ColumnInfo.Index));
			}
			return values;
		}
		List<object> ProcessListBasedSummaryDupUni(VisibleListSourceRowCollection list, SummaryItem summaryItem) {
			List<object> values = new List<object>();
			for(int n = 0; n < list.VisibleRowCount; n++) {
				object v = GetListSourceRowValue(list.GetListSourceRow(n), summaryItem.ColumnInfo.Index);
				if(object.ReferenceEquals(v, DBNull.Value)) v = null;
				values.Add(v);
			}
			if(summaryItem.SummaryTypeEx == SummaryItemTypeEx.Duplicate) {
				return values.GroupBy(x => x)
								 .Where(g => g.Count() > 1)
								 .Select(g => g.Key)
								 .ToList();
			}
			else {
				return values.GroupBy(x => x)
								 .Where(g => g.Count() == 1)
								 .Select(g => g.Key)
								 .ToList();
			}
		}
		protected internal virtual void RestoreGroupExpanded(GroupRowInfo group) {
			group.Expanded = true;
		}
	}
	public class GroupSummaryComparer : IComparer<GroupRowInfo>, IComparer {
		DataController controller;
		SummarySortInfo sortInfo;
		public GroupSummaryComparer(DataController controller, SummarySortInfo sortInfo) {
			this.controller = controller;
			this.sortInfo = sortInfo;
		}
		protected DataController Controller { get { return controller; } }
		protected ValueComparer ValueComparer { get { return Controller.ValueComparer; } }
		public int Compare(GroupRowInfo x, GroupRowInfo y) {
			return CompareCore(x, y);
		}
		public int Compare(object x, object y) {
			GroupRowInfo groupRow1 = (GroupRowInfo)x, groupRow2 = (GroupRowInfo)y;
			return CompareCore(groupRow1, groupRow2);
		}
		int CompareCore(GroupRowInfo groupRow1, GroupRowInfo groupRow2) {
			int res = 0;
			if(this.sortInfo != null) {
				res = Compare(groupRow1, groupRow2, this.sortInfo.SummaryItem);
			}
			if(res == 0) res = groupRow1.Index.CompareTo(groupRow2.Index);
			if(this.sortInfo == null || this.sortInfo.SortOrder == ColumnSortOrder.Ascending) return res;
			res = (res > 0 ? -1 : 1);
			return res;
		}
		public int Compare(GroupRowInfo groupRow1, GroupRowInfo groupRow2, SummaryItem item) {
			return ValueComparer.Compare(groupRow1.GetSummaryValue(item), groupRow2.GetSummaryValue(item));
		}
	}
	public class DataControllerGroupRowInfoCollection : GroupRowInfoCollection {
		public DataControllerGroupRowInfoCollection(DataController controller) : base(controller, controller.SortInfo, controller.VisibleListSourceRows) { }
		protected override DataColumnSortInfoCollection SortInfo { get { return Controller.SortInfo; } }
		public override VisibleListSourceRowCollection VisibleListSourceRows { get { return Controller.VisibleListSourceRows; } } 
		protected new DataController Controller { get { return base.Controller as DataController; } }
	}
	public class DataControllerFilterHelper : FilterHelper {
		public DataControllerFilterHelper(DataController controller) : base(controller, controller.VisibleListSourceRows) { }
		public override VisibleListSourceRowCollection VisibleListSourceRows { get { return Controller.VisibleListSourceRows; } } 
		public new DataController Controller { get { return base.Controller as DataController; } }
	}
	public class DataControllerVisibleIndexCollection : VisibleIndexCollection {
		public DataControllerVisibleIndexCollection(DataController controller) : base(controller, controller.GroupInfo) { }
		protected new DataController Controller { get { return base.Controller as DataController; } }
		protected internal override GroupRowInfoCollection GroupInfo { get { return Controller.GroupInfo; } }
	}
	public class ServerModeDataControllerVisibleIndexCollection : DataControllerVisibleIndexCollection {
		public ServerModeDataControllerVisibleIndexCollection(DataController controller) : base(controller) { }
		protected override int GetMaxCount() { return 1000; }
	}
	public delegate bool CompareIncrementalValue(int controllerRow, object value, string text);
	public delegate void OperationCompleted(object arguments);
	public abstract class BaseRowStub {
		readonly DataControllerBase DC;
		readonly Action AdditionalCleanUp;
		int _RowIndex = -1;
		object _Row;
		bool RowGot;
		protected BaseRowStub(DataControllerBase _DC, Action additionalCleanUp) {
			this.DC = _DC;
			this.AdditionalCleanUp = additionalCleanUp;
		}
		public int RowIndex {
			get { return _RowIndex; }
		}
		public void GoTo(int rowIndex) {
			RowGot = false;
			_Row = null;
			_RowIndex = rowIndex;
			if(AdditionalCleanUp != null)
				AdditionalCleanUp();
		}
		public void Reset() {
			GoTo(-1);
		}
		public object Row {
			get {
				if(!RowGot) {
					RowGot = true;
					object row = DC.Helper.GetRow(RowIndex);
					if(row is NotLoadedObject)
						row = null;
					_Row = row;
				}
				return _Row;
			}
		}
		public class CachingCriteriaCompilerDescriptor: CriteriaCompilerDescriptor {
			readonly Dictionary<string, ValueCacheBase> CachesCache = new Dictionary<string, ValueCacheBase>();
			readonly CriteriaCompilerDescriptor FlatDescriptor;
			readonly Func<string, bool> IsCacheable;
			CachingCriteriaCompilerDescriptor(CriteriaCompilerDescriptor flatDescriptor, Func<string, bool> isCacheable) {
				this.FlatDescriptor = flatDescriptor;
				this.IsCacheable = isCacheable;
			}
			public CachingCriteriaCompilerDescriptor(CriteriaCompilerDescriptor flatDescriptor, CriteriaOperator op)
				: this(flatDescriptor, CacheInfoCollector.GetIsCacheablePropertyFunc(op)) {
			}
			public override Type ObjectType {
				get { return typeof(BaseRowStub); }
			}
			public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
				if(!IsCacheable(propertyPath)) {
					return FlatDescriptor.MakePropertyAccess(baseExpression, propertyPath);
				}
				if(propertyPath == null)
					throw new InvalidOperationException("Internal error: propertyPath is null");
				ValueCacheBase vcb;
				if(!CachesCache.TryGetValue(propertyPath, out vcb)) {
					ParameterExpression stubParam = Expression.Parameter(this.ObjectType, "stub");
					Expression stubBody = FlatDescriptor.MakePropertyAccess(stubParam, propertyPath);
					LambdaExpression l = Expression.Lambda(stubBody, stubParam);
					vcb = (ValueCacheBase)Activator.CreateInstance(typeof(ValueCache<>).MakeGenericType(stubBody.Type), l.Compile());
					CachesCache.Add(propertyPath, vcb);
				}
				return Expression.Call(Expression.Constant(vcb), "GetValue", null, baseExpression);
			}
			public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionPropertyPath) {
				return FlatDescriptor.DiveIntoCollectionProperty(baseExpression, collectionPropertyPath);
			}
			public override LambdaExpression MakeFreeJoinLambda(string joinTypeName, CriteriaOperator condition, OperandParameter[] conditionParameters, Aggregate aggregateType, CriteriaOperator aggregateExpression, OperandParameter[] aggregateExpresssionParameters, Type[] invokeTypes) {
				return FlatDescriptor.MakeFreeJoinLambda(joinTypeName, condition, conditionParameters, aggregateType, aggregateExpression, aggregateExpresssionParameters, invokeTypes);
			}
			public Action GetStubCleanUpCode() {
				var caches = CachesCache.Values.ToArray();
				if(caches.Length == 0)
					return null;
				if(caches.Length == 1) {
					var singleCache = caches[0];
					return () => singleCache.Reset();
				}
				return () => {
					for(int i = 0; i < caches.Length; ++i) {
						caches[i].Reset();
					}
				};
			}
			public class CacheInfoCollector: IClientCriteriaVisitor {
				CacheInfoCollector() { }
				Dictionary<string, int> Counter = new Dictionary<string, int>();
				public void Visit(AggregateOperand theOperand) {
				}
				public void Visit(OperandProperty theOperand) {
					if(string.IsNullOrEmpty(theOperand.PropertyName) || EvaluatorProperty.GetIsThisProperty(theOperand.PropertyName))
						return;
					int cnt;
					if(!Counter.TryGetValue(theOperand.PropertyName, out cnt)) {
						Counter.Add(theOperand.PropertyName, 1);
					} else {
						Counter[theOperand.PropertyName] = cnt + 1;
					}
				}
				public void Visit(JoinOperand theOperand) {
				}
				public void Visit(BetweenOperator theOperator) {
					Process(theOperator.TestExpression);
					Process(theOperator.BeginExpression);
					Process(theOperator.EndExpression);
				}
				public void Visit(BinaryOperator theOperator) {
					Process(theOperator.LeftOperand);
					Process(theOperator.RightOperand);
				}
				public void Visit(UnaryOperator theOperator) {
					Process(theOperator.Operand);
				}
				public void Visit(InOperator theOperator) {
					Process(theOperator.LeftOperand);
					Process(theOperator.Operands);
				}
				public void Visit(GroupOperator theOperator) {
					Process(theOperator.Operands);
				}
				public void Visit(OperandValue theOperand) {
				}
				public void Visit(FunctionOperator theOperator) {
					Process(theOperator.Operands);
				}
				void Process(CriteriaOperator op) {
					if(ReferenceEquals(op, null))
						return;
					op.Accept(this);
				}
				void Process(IEnumerable<CriteriaOperator> ops) {
					foreach(var op in ops)
						Process(op);
				}
				public static Func<string, bool> GetIsCacheablePropertyFunc(CriteriaOperator op) {
					CacheInfoCollector collector = new CacheInfoCollector();
					collector.Process(op);
					Dictionary<string, int> counter = collector.Counter;
					var cacheNames = counter.Where(kvp => kvp.Value > 1).Select(kvp => kvp.Key).ToArray();
					if(cacheNames.Length == 0)
						return x => false;
					if(cacheNames.Length == 1) {
						string nm = cacheNames[0];
						return x => x == nm;
					}
					Dictionary<string, string> rv = cacheNames.ToDictionary(s => s, s => s);
					return x => rv.ContainsKey(x);
				}
			}
		}
		public class DataControllerCriteriaCompilerDescriptor: CriteriaCompilerDescriptor {
			readonly Func<PropertyDescriptorCollection> PropertyDescriptorsSource;
			PropertyDescriptorCollection _Props;
			public DataControllerCriteriaCompilerDescriptor(DataControllerBase dc) : this(() => dc.GetFilterDescriptorCollection()) { }
			public DataControllerCriteriaCompilerDescriptor(Func<PropertyDescriptorCollection> propertyDescriptorsSource) {
				if(propertyDescriptorsSource == null)
					throw new ArgumentNullException("propertyDescriptorsSource");
				this.PropertyDescriptorsSource = propertyDescriptorsSource;
			}
			protected PropertyDescriptorCollection Props {
				get {
					if(_Props == null)
						_Props = this.PropertyDescriptorsSource();
					return _Props;
				}
			}
			CriteriaCompilerDescriptor _WorkHorseContext;
			protected CriteriaCompilerDescriptor WorkHorseContext {
				get {
					if(_WorkHorseContext == null)
						_WorkHorseContext = CriteriaCompilerDescriptor.Get(Props);
					return _WorkHorseContext;
				}
			}
			public override Type ObjectType {
				get { return typeof(BaseRowStub); }
			}
			public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
				PropertyDescriptor pd = Props.Find(propertyPath, false) ?? Props.Find(propertyPath, true);
				if(pd != null) {
					{
						var upd = pd as DevExpress.Data.Access.UnboundPropertyDescriptor;
						if(upd != null) {
							Func<BaseRowStub, object> fn = stub => upd.GetValueFromRowNumber(stub.RowIndex);
							return Expression.Invoke(Expression.Constant(fn), baseExpression);
						}
					}
					{
						var dtpd = pd as DevExpress.Data.Access.DisplayTextPropertyDescriptor;
						if(dtpd != null) {
							Func<BaseRowStub, string> fn;
							var nestedUpd = dtpd.Info.PropertyDescriptor as DevExpress.Data.Access.UnboundPropertyDescriptor;
							if(nestedUpd != null) {
								fn = stub => dtpd.GetDisplayText(stub.RowIndex, nestedUpd.GetValueFromRowNumber(stub.RowIndex));
							} else {
								var capturedPD = dtpd.Info.PropertyDescriptor;
								fn = stub => dtpd.GetDisplayText(stub.RowIndex, CriteriaCompilerContextDescriptorDefaultBase.KillDBNull(capturedPD.GetValue(stub.Row)));
							}
							return Expression.Invoke(Expression.Constant(fn), baseExpression);
						}
					}
				}
				ParameterExpression rowParameter = Expression.Parameter(typeof(object), "row");
				var accessor = WorkHorseContext.MakePropertyAccess(rowParameter, propertyPath);
				if(!NullableHelpers.CanAcceptNull(accessor.Type))
					accessor = Expression.Convert(accessor, NullableHelpers.GetUnBoxedType(accessor.Type));
				Expression body = Expression.Condition(Expression.Call(typeof(object), "ReferenceEquals", null, rowParameter, Expression.Constant(null)), Expression.Constant(null, accessor.Type), accessor);
				LambdaExpression lmbd = Expression.Lambda(body, rowParameter);
				Expression rv = Expression.Invoke(lmbd, Expression.PropertyOrField(baseExpression, "Row"));
				if(typeof(IList).IsAssignableFrom(rv.Type) && typeof(ITypedList).IsAssignableFrom(rv.Type)) {
					Func<IList, object> refFromCollection = collection => {
						if(collection == null || collection.Count == 0)
							return null;
						else if(collection.Count == 1)
							return collection[0];
						else
							throw new ArgumentException("single row expected at '" + propertyPath + "', provided: " + collection.Count.ToString());	
					};
					rv = Expression.Invoke(Expression.Constant(refFromCollection), rv);
				}
				return rv;
			}
			public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionPropertyPath) {
				return WorkHorseContext.DiveIntoCollectionProperty(Expression.PropertyOrField(baseExpression, "Row"), collectionPropertyPath);
			}
		}
		public class CriteriaCompilerNullRowProofDescriptor: CriteriaCompilerDescriptor {
			readonly CriteriaCompilerDescriptor Nested;
			public CriteriaCompilerNullRowProofDescriptor(CriteriaCompilerDescriptor _Nested) {
				this.Nested = _Nested;
			}
			public override Type ObjectType {
				get { return typeof(BaseRowStub); }
			}
			public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
				ParameterExpression rowParameter = Expression.Parameter(ObjectType, "row");
				Expression value = Nested.MakePropertyAccess(rowParameter, propertyPath);
				Type rvType = NullableHelpers.GetUnBoxedType(value.Type);
				if(value.Type != rvType)
					value = Expression.Convert(value, rvType);
				Expression body = Expression.Condition(Expression.Call(typeof(object), "ReferenceEquals", null, Expression.PropertyOrField(rowParameter, "Row"), Expression.Constant(null)), Expression.Constant(null, rvType), value);
				var lambda = Expression.Lambda(body, rowParameter);
				return Expression.Invoke(lambda, baseExpression);
			}
			public override CriteriaCompilerRefResult DiveIntoCollectionProperty(Expression baseExpression, string collectionPropertyPath) {
				return Nested.DiveIntoCollectionProperty(baseExpression, collectionPropertyPath);
			}
			public override LambdaExpression MakeFreeJoinLambda(string joinTypeName, CriteriaOperator condition, OperandParameter[] conditionParameters, Aggregate aggregateType, CriteriaOperator aggregateExpression, OperandParameter[] aggregateExpresssionParameters, Type[] invokeTypes) {
				return Nested.MakeFreeJoinLambda(joinTypeName, condition, conditionParameters, aggregateType, aggregateExpression, aggregateExpresssionParameters, invokeTypes);
			}
		}
		public abstract class ValueCacheBase {
			public abstract void Reset();
			public abstract Type Type { get; }
		}
		public sealed class ValueCache<T>: ValueCacheBase {
			readonly Func<BaseRowStub, T> ValueGetter;
			public ValueCache(Func<BaseRowStub, T> _ValueGetter) {
				this.ValueGetter = _ValueGetter;
			}
			bool ValueGot;
			T Value;
			public override void Reset() {
				ValueGot = false;
				Value = default(T);
			}
			public T GetValue(BaseRowStub arg) {
				if(!ValueGot) {
					ValueGot = true;
					Value = ValueGetter(arg);
				}
				return Value;
			}
			public override Type Type {
				get { return typeof(T); }
			}
		}
	}
}
