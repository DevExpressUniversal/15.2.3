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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class SyncVisibleListWrapper : VisibleListWrapper, ICollectionView, IListServerDataView, IListServer, IListServerHints, IDataControllerAdapter, ITypedList {
		readonly SyncServerModeCurrentDataView dataView;
		readonly SyncListWrapper wrapper;
		readonly bool isClonedServer;
		IListServer Server { get { return wrapper.Server; } }
		SyncDataProxyViewCache ItemsCache { get; set; }
		DataControllerItemsCache View { get; set; }
		public DataAccessor DataAccessor { get { return dataView.DataAccessor; } }
		public SyncListWrapper Wrapper { get { return wrapper; } }
		bool initialized = false;
		public SyncVisibleListWrapper(SyncServerModeCurrentDataView dataView, SyncListWrapper wrapper = null) {
			isClonedServer = wrapper == null;
			this.wrapper = wrapper ?? new SyncListWrapper(dataView.Wrapper.Server);
			this.dataView = dataView;
			dataView.ListChanged += DataViewListChanged;
			ItemsCache = new SyncDataProxyViewCache(this);
			View = new DataControllerItemsCache(this);
		}
		void DataViewListChanged(object sender, ListChangedEventArgs e) {
			RaiseListChanged(e);
		}
		protected override IEnumerator GetEnumeratorInternal() {
			int count = wrapper.Count;
			for (int i = 0; i < count; i++) {
				yield return GetRow(i);
			}
		}
		#region IDataControllerAdapter
		int IDataControllerAdapter.VisibleRowCount { get { return ItemsCache.Count; } }
		bool IDataControllerAdapter.IsOwnSearchProcessing { get { return true; } }
		object IDataControllerAdapter.GetRowValue(object item) {
			DataProxy proxy = DataAccessor.CreateProxy(item, -1);
			object value = GetValueFromProxy(proxy);
			return IndexOf(value);
		}
		object IDataControllerAdapter.GetRowValue(int listSourceIndex) {
			var item = ItemsCache[listSourceIndex];
			return GetValueFromProxy(item);
		}
		object IDataControllerAdapter.GetRow(int listSourceIndex) {
			return listSourceIndex > -1 ? ItemsCache[listSourceIndex].f_component : null;
		}
		int IDataControllerAdapter.GetListSourceIndex(object value) {
			return ItemsCache.FindIndexByValue(ListServerDataViewBase.CreateCriteriaForValueColumn(DataAccessor), value);
		}
		public object GetValueFromItem(object item) {
			int index = View.IndexByItem(item);
			if (index < 0)
				return null;
			var proxy = ItemsCache[index];
			if (proxy == null)
				return null;
			return GetValueFromProxy(proxy);
		}
		public object GetValueFromProxy(DataProxy proxy) {
			return DataAccessor.GetValue(proxy);
		}
		#endregion
		object GetRow(int index) {
			return ItemsCache[index];
		}
		protected override void CopyToInternal(Array array, int index) {
			int count = Count;
			for (int i = index; i < count; i++)
				array.SetValue(GetRow(i), i);
		}
		protected override int GetCountInternal() {
			return wrapper.Count;
		}
		protected override bool ContainsInternal(object value) {
			return IndexOf(value) > -1;
		}
		protected override int IndexOfInternal(object value) {
			int index = View.IndexOfValue(value);
			return index;
		}
		protected override object IndexerGetInternal(int index) {
			return GetRow(index);
		}
		protected override void DisposeInternal() {
			dataView.ListChanged -= DataViewListChanged;
		}
		public void ApplyGroupSortFilter(int groupCount, IList<SortingInfo> sorts, string filterCriteria) {
			ApplyGroupSortFilterInternal(CriteriaOperator.Parse(filterCriteria),
				sorts.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount);
		}
		#region CollectionView
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args) {
			var collectionChanged = CollectionChanged;
			if (collectionChanged != null)
				collectionChanged(this, args);
		}
		void IListServer.Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo,
			ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			ApplyGroupSortFilterInternal(filterCriteria, sortInfo, groupCount, groupSummaryInfo, totalSummaryInfo);
		}
		void ApplyGroupSortFilterInternal(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo = null,
			ICollection<ServerModeSummaryDescriptor> totalSummaryInfo = null) {
			this.wrapper.ApplySortGroupFilter(filterCriteria, sortInfo, groupCount, groupSummaryInfo, totalSummaryInfo);
		}
		public void Reset() {
			ItemsCache.Reset();
			View.Reset();
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		List<ListSourceGroupInfo> IListServer.GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return Server.GetGroupInfo(parentGroup);
		}
		object[] IListServer.GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return Server.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
		}
		List<object> IListServer.GetTotalSummary() {
			return Server.GetTotalSummary();
		}
		object IListServer.GetRowKey(int index) {
			return Server.GetRowKey(index);
		}
		int IListServer.GetRowIndexByKey(object key) {
			return Server.GetRowIndexByKey(key);
		}
		int IListServer.FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow,
			bool allowLoop) {
			return Server.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop);
		}
		int IListServer.LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return Server.LocateByValue(expression, value, startIndex, searchUp);
		}
		IList IListServer.GetAllFilteredAndSortedRows() {
			return Server.GetAllFilteredAndSortedRows();
		}
		bool IListServer.PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return Server.PrefetchRows(groupsToPrefetch, cancellationToken);
		}
		void IListServer.Refresh() {
			Server.Refresh();
			ItemsCache.Reset();
		}
		protected override void RefreshInternal() {
			wrapper.Refresh();
			Reset();
		}
		void IListServerHints.HintGridIsPaged(int pageSize) {
			(Server as IListServerHints).Do(x => x.HintGridIsPaged(pageSize));
		}
		void IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
			(Server as IListServerHints).Do(x => x.HintMaxVisibleRowsInGrid(rowsInGrid));
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		void RaiseInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs args) {
			var id = InconsistencyDetected;
			if (id != null)
				id(this, args);
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		void RaiseExceptionThrown(ServerModeExceptionThrownEventArgs args) {
			var id = ExceptionThrown;
			if (id != null)
				id(this, args);
		}
		void IListServerDataView.NotifyInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			Reset();
			RaiseInconsistencyDetected(e);
		}
		void IListServerDataView.NotifyExceptionThrown(ServerModeExceptionThrownEventArgs e) {
			RaiseExceptionThrown(e);
		}
		void ICollectionView.Refresh() {
		}
		IDisposable ICollectionView.DeferRefresh() {
			return new DeferHelper(this);
		}
		void EndDefer() {
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		bool ICollectionView.MoveCurrentToFirst() {
			return false;
		}
		bool ICollectionView.MoveCurrentToLast() {
			return false;
		}
		bool ICollectionView.MoveCurrentToNext() {
			return false;
		}
		bool ICollectionView.MoveCurrentToPrevious() {
			return false;
		}
		bool ICollectionView.MoveCurrentTo(object item) {
			return false;
		}
		bool ICollectionView.MoveCurrentToPosition(int position) {
			return false;
		}
		CultureInfo ICollectionView.Culture {
			get { return CultureInfo.CurrentCulture; }
			set {; }
		}
		IEnumerable ICollectionView.SourceCollection { get { return this; } }
		void SetupWrapper() {
			if (!isClonedServer)
				return;
			this.wrapper.Reset();
		}
		public void Initialize() {
			if (!initialized) {
				wrapper.Initialize(this);
				SetupWrapper();
				initialized = true;
			}
			dataView.InitializeVisibleList();
		}
		public void ProcessInconsistencyDetected() {
			Reset();
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		Predicate<object> ICollectionView.Filter {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool ICollectionView.CanFilter {
			get { return false; }
		}
		SortDescriptionCollection ICollectionView.SortDescriptions {
			get { return new SortDescriptionCollection(); }
		}
		bool ICollectionView.CanSort {
			get { return false; }
		}
		bool ICollectionView.CanGroup {
			get { return false; }
		}
		ObservableCollection<GroupDescription> ICollectionView.GroupDescriptions {
			get { return new ObservableCollection<GroupDescription>(); }
		}
		ReadOnlyObservableCollection<object> ICollectionView.Groups {
			get { throw new NotImplementedException(); }
		}
		bool ICollectionView.IsEmpty {
			get { return dataView.Wrapper.Count == 0; }
		}
		object ICollectionView.CurrentItem {
			get { return null; }
		}
		int ICollectionView.CurrentPosition {
			get { return -1; }
		}
		bool ICollectionView.IsCurrentAfterLast {
			get { return false; }
		}
		bool ICollectionView.IsCurrentBeforeFirst {
			get { return false; }
		}
		public event CurrentChangingEventHandler CurrentChanging;
		void RaiseCurrentChanging() {
			if (CurrentChanging != null)
				CurrentChanging(this, new CurrentChangingEventArgs());
		}
		public event EventHandler CurrentChanged;
		void RaiseCurrentChanged() {
			if (CurrentChanged != null)
				CurrentChanged(this, EventArgs.Empty);
		}
		#endregion
		class DeferHelper : IDisposable {
			public DeferHelper(SyncVisibleListWrapper collectionView) {
				this.collectionView = collectionView;
			}
			public void Dispose() {
				if (collectionView != null) {
					collectionView.EndDefer();
					collectionView = null;
				}
				GC.SuppressFinalize(this);
			}
			SyncVisibleListWrapper collectionView;
		}
		#region ITypedList
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ((ITypedList)this.Server).GetItemProperties(listAccessors);
		}
		#endregion
	}
}
