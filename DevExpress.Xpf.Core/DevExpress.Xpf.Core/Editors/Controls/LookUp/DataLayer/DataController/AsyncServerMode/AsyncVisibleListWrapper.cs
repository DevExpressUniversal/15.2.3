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
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncVisibleListWrapper : VisibleListWrapper, ICollectionView, IAsyncListServer, IAsyncListServerDataView, IDataControllerAdapter, IAsyncResultReceiverBusyChangedListener, ITypedList {
		readonly AsyncServerModeCurrentDataView dataView;
		readonly AsyncListWrapper wrapper;
		AsyncSharedResultReceiver sharedReceiver;
		readonly bool isClonedServer;
		IAsyncListServer Server { get { return wrapper.Server; } }
		AsyncDataProxyViewCache ItemsCache { get; set; }
		DataControllerItemsCache View { get; set; }
		DataAccessor DataAccessor { get { return dataView.DataAccessor; } }
		public object TempProxy { get { return ItemsCache.TempProxy; } }
		bool initialized = false;
		public AsyncVisibleListWrapper(AsyncServerModeCurrentDataView dataView, AsyncListWrapper wrapper = null) {
			isClonedServer = wrapper == null;
			this.wrapper = wrapper ?? new AsyncListWrapper(dataView.Wrapper.Server);
			this.dataView = dataView;
			dataView.InconsistencyDetected += DataViewOnInconsistencyDetected;
			dataView.ListChanged += DataViewListChanged;
			ItemsCache = new AsyncDataProxyViewCache(this);
			View = new DataControllerItemsCache(this);
		}
		void DataViewOnInconsistencyDetected(object sender, EventArgs eventArgs) {
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
		#region IAsyncResultReceiverBusyChangedListener
		void IAsyncResultReceiverBusyChangedListener.ProcessBusyChanged(bool busyChanged) {
		}
		#endregion
		#region IDataControllerAdapter
		int IDataControllerAdapter.VisibleRowCount { get { return ItemsCache.Count; } }
		bool IDataControllerAdapter.IsOwnSearchProcessing { get { return true; } }
		object IDataControllerAdapter.GetRowValue(object item) {
			return GetValueFromItem(item);
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
			var proxy = ItemsCache[index];
			return GetValueFromProxy(proxy);
		}
		public object GetValueFromProxy(DataProxy proxy) {
			return DataAccessor.GetValue(proxy);
		}
		#endregion
		object GetRow(int index) {
			if (!wrapper.IsRowLoaded(index))
				return ItemsCache.TempProxy;
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
			return false;
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
		public void FetchItem(int visibleIndex) {
			wrapper.GetRow(visibleIndex, FetchItemCompleted);
		}
		void FetchItemCompleted(object arguments) {
			CommandGetRow command = (CommandGetRow)arguments;
			ProcessRowLoaded(command.Index);
		}
		public void CancelItem(int visibleIndex) {
			wrapper.CancelRow(visibleIndex);
		}
		public bool IsTempItem(int visibleIndex) {
			return !wrapper.IsRowLoaded(visibleIndex);
		}
		public void ApplyGroupSortFilter(int groupCount, IList<SortingInfo> sorts, string filterCriteria) {
			wrapper.ApplySortGroupFilter(CriteriaOperator.Parse(filterCriteria),
				sorts.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount);
		}
		protected override void RefreshInternal() {
		}
		#region CollectionView
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args) {
			var collectionChanged = CollectionChanged;
			if (collectionChanged != null)
				collectionChanged(this, args);
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
			sharedReceiver = new AsyncSharedResultReceiver(wrapper);
			sharedReceiver.AddReceiver(new AsyncNotifyBusyChangedResultReceiver(this));
			this.wrapper.SetReceiver(sharedReceiver);
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
			public DeferHelper(AsyncVisibleListWrapper collectionView) {
				this.collectionView = collectionView;
			}
			public void Dispose() {
				if (collectionView != null) {
					collectionView.EndDefer();
					collectionView = null;
				}
				GC.SuppressFinalize(this);
			}
			AsyncVisibleListWrapper collectionView;
		}
		CommandGetTotals IAsyncListServer.GetTotals(params DictionaryEntry[] tags) {
			return Server.GetTotals(tags);
		}
		CommandGetRow IAsyncListServer.GetRow(int index, params DictionaryEntry[] tags) {
			return Server.GetRow(index, tags);
		}
		CommandGetGroupInfo IAsyncListServer.GetGroupInfo(ListSourceGroupInfo parentGroup, params DictionaryEntry[] tags) {
			return Server.GetGroupInfo(parentGroup, tags);
		}
		CommandGetRowIndexByKey IAsyncListServer.GetRowIndexByKey(object key, params DictionaryEntry[] tags) {
			return Server.GetRowIndexByKey(key, tags);
		}
		CommandGetUniqueColumnValues IAsyncListServer.GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut, params DictionaryEntry[] tags) {
			return Server.GetUniqueColumnValues(expression, maxCount, includeFilteredOut, tags);
		}
		CommandFindIncremental IAsyncListServer.FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop, params DictionaryEntry[] tags) {
			return Server.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop, tags);
		}
		CommandLocateByValue IAsyncListServer.LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp, params DictionaryEntry[] tags) {
			return Server.LocateByValue(expression, value, startIndex, searchUp, tags);
		}
		CommandApply IAsyncListServer.Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> summaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo, params DictionaryEntry[] tags) {
			return Server.Apply(filterCriteria, sortInfo, groupCount, summaryInfo, totalSummaryInfo, tags);
		}
		CommandRefresh IAsyncListServer.Refresh(params DictionaryEntry[] tags) {
			return Server.Refresh(tags);
		}
		CommandGetAllFilteredAndSortedRows IAsyncListServer.GetAllFilteredAndSortedRows(params DictionaryEntry[] tags) {
			return Server.GetAllFilteredAndSortedRows(tags);
		}
		CommandPrefetchRows IAsyncListServer.PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, params DictionaryEntry[] tags) {
			return Server.PrefetchRows(groupsToPrefetch, tags);
		}
		void IAsyncListServer.Cancel(Command command) {
			Server.Cancel(command);
		}
		void IAsyncListServer.Cancel<T>() {
			Server.Cancel<T>();
		}
		void IAsyncListServer.WeakCancel<T>() {
			Server.WeakCancel<T>();
		}
		void IAsyncListServer.SetReceiver(IAsyncResultReceiver receiver) {
			sharedReceiver.AddReceiver(receiver);
		}
		T IAsyncListServer.PullNext<T>() {
			return Server.PullNext<T>();
		}
		bool IAsyncListServer.WaitFor(Command command) {
			return Server.WaitFor(command);
		}
		AsyncListWrapper IAsyncListServerDataView.Wrapper {
			get { return wrapper; }
		}
		DataAccessor IAsyncListServerDataView.DataAccessor {
			get { return dataView.DataAccessor; }
		}
		void IAsyncListServerDataView.BusyChanged(bool busy) {
			ProcessBusyChanged(busy);
		}
		void IAsyncListServerDataView.NotifyCountChanged() {
			ProcessCountChanged();
		}
		void IAsyncListServerDataView.NotifyLoaded(int listSourceIndex) {
			ProcessRowLoaded(listSourceIndex);
		}
		void IAsyncListServerDataView.NotifyFindIncrementalCompleted(string text, int startIndex, bool searchNext, bool ignoreStartIndex, int controllerIndex) {
			throw new NotImplementedException();
		}
		void IAsyncListServerDataView.NotifyApply() {
			ProcessApply();
		}
		object IAsyncListServerDataView.GetValueFromProxy(DataProxy proxy) {
			return dataView.GetValueFromProxy(proxy);
		}
		void IAsyncListServerDataView.NotifyInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			dataView.ProcessInconsistencyDetected();
		}
		void IAsyncListServerDataView.NotifyExceptionThrown(ServerModeExceptionThrownEventArgs e) {
		}
		void ProcessBusyChanged(bool busy) {
			dataView.ProcessVisibleListBusyChanged(busy);
		}
		void ProcessCountChanged() {
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		void ProcessRowLoaded(int index) {
			View.UpdateItem(index);
			var row = GetRow(index);
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, row, ItemsCache.TempProxy, index));
		}
		void ProcessApply() {
			ItemsCache.Reset();
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
#if DEBUGTEST
		public void PumpAll() {
			wrapper.PumpAll();
		}
#endif
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
