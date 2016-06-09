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
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Native;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataController {
		bool snapshotsDestroyed = false;
		public readonly object CurrentDataViewHandle = new object();
		public readonly object DefaultDataViewHandle = new object();
		readonly Locker endInitLocker = new Locker();
		public IItemsProviderOwner Owner { get; private set; }
		public IList ListSource { get; private set; }
		public object VisibleList { get { return CurrentDataView.VisibleList; } }
		public DefaultDataView DefaultDataView { get; private set; }
		public CurrentDataView CurrentDataView { get { return Views[CurrentDataViewHandle]; } }
		public CurrentDataView GetCurrentDataView(object handle) { return Views[handle]; }
		PostponedAction EndInitPostponedAction { get; set; }
		public event ListChangedEventHandler ListChanged;
		public event EventHandler Refreshed;
		public event EventHandler<ItemsProviderCurrentChangedEventArgs> CurrentChanged;
		public event EventHandler<ItemsProviderRowLoadedEventArgs> RowLoaded;
		public event EventHandler<ItemsProviderOnBusyChangedEventArgs> BusyChanged;
		public event EventHandler<ItemsProviderViewRefreshedEventArgs> ViewRefreshed;
		public event EventHandler<ItemsProviderFindIncrementalCompletedEventArgs> FindIncrementalCompleted;
		ListChangedWeakEventHandler<DataController> ListChangedHandler { get; set; }
		readonly HashSet<object> busyHandlers = new HashSet<object>();
		readonly Locker findIncrementalLocker = new Locker();
		public bool NeedsRefresh { get { return this.snapshotsDestroyed; } }
		readonly Dictionary<object, DataControllerSnapshotDescriptor> Snapshots = new Dictionary<object, DataControllerSnapshotDescriptor>();
		internal readonly Dictionary<object, CurrentDataView> Views = new Dictionary<object, CurrentDataView>();
		Dispatcher Dispatcher { get { return Owner.Dispatcher; } }
		public DataController(IItemsProviderOwner owner) {
			Owner = owner;
			EndInitPostponedAction = new PostponedAction(() => endInitLocker.IsLocked);
			ListChangedHandler = new ListChangedWeakEventHandler<DataController>(this, (controller, o, e) => controller.ItemsSourceListChanged(o, e));
			InitializeCurrentDataView();
			UpdateItemsSource();
		}
		void InitializeCurrentDataView() {
			var currentDataViewDescriptor = new DataControllerSnapshotDescriptor(CurrentDataViewHandle);
			currentDataViewDescriptor.Refreshed += DescriptorRefreshed;
			Snapshots.Add(CurrentDataViewHandle, currentDataViewDescriptor);
		}
		void DescriptorRefreshed(object sender, EventArgs eventArgs) {
			if (endInitLocker.IsLocked)
				return;
			var descriptor = (DataControllerSnapshotDescriptor)sender;
			UpdateSnapshot(descriptor);
			RaiseRefreshed();
		}
		public virtual void BeginInit() {
			endInitLocker.Lock();
		}
		public virtual void EndInit() {
			UpdateDefaultView();
			ResetSnapshots();
			UpdateSnapshots();
			endInitLocker.Unlock();
		}
		public virtual DataControllerSnapshotDescriptor GetSnapshot(object handle) {
			return Snapshots.GetValueOrDefault(handle);
		}
		public virtual void RegisterSnapshot(DataControllerSnapshotDescriptor descriptor) {
			Snapshots.Add(descriptor.Handle, descriptor);
			descriptor.Refreshed += DescriptorRefreshed;
			if (!endInitLocker.IsLocked)
				UpdateSnapshot(descriptor);
		}
		public virtual void ReleaseSnapshot(DataControllerSnapshotDescriptor descriptor) {
			Snapshots.Remove(descriptor.Handle);
			descriptor.Refreshed -= DescriptorRefreshed;
			var view = Views.GetValueOrDefault(descriptor.Handle);
			view.Do(x => x.Release());
			Views.Remove(descriptor.Handle);
		}
		void UpdateDefaultView() {
			DefaultDataView.Do(UnsubscribeDefaultDataView);
			DefaultDataView.Do(x => x.Dispose());
			DefaultDataView = CreateDefaultDataView();
			SubscribeDefaultDataView(DefaultDataView);
		}
		void SubscribeDefaultDataView(DefaultDataView view) {
			view.InconsistencyDetected += DefaultViewInconsistencyDetected;
			view.RefreshNeeded += DefaultViewRefreshNeeded;
			view.ListChanged += DefaultViewListChanged;
			view.RowLoaded += DefaultViewRowLoaded;
			view.BusyChanged += DefaultViewOnBusyChanged;
			view.FindIncrementalCompleted += DefaultViewFindIncrementalCompleted;
			view.Initialize();
			view.SubscribeToEvents();
			(view.ListSource as IBindingList).Do(x => x.ListChanged += ListChangedHandler.Handler);
			(view as CollectionViewDefaultDataView).Do(x => x.CurrentChanged += CollectionViewCurrentChanged);
		}
		void DefaultViewFindIncrementalCompleted(object sender, ItemsProviderChangedEventArgs e) {
			ProcessFindIncrementalCompletedForSnapshots(e);
			RaiseFindIncrementalCompleted(this, (ItemsProviderFindIncrementalCompletedEventArgs)e);
		}
		void ProcessFindIncrementalCompletedForSnapshots(ItemsProviderChangedEventArgs e) {
			findIncrementalLocker.DoLockedActionIfNotLocked(() => {
				var args = (ItemsProviderFindIncrementalCompletedEventArgs)e;
				foreach (var pair in Snapshots)
					ProcessFindIncrementalCompletedForSnapshot(Views[pair.Key], args.Text, args.StartIndex, args.SearchNext, args.IgnoreStartIndex, args.Value);
			});
		}
		void ProcessFindIncrementalCompletedForSnapshot(CurrentDataView view, string text, int startIndex, bool searchNext, bool ignoreStartIndex, object value) {
			view.ProcessFindIncrementalCompleted(text, startIndex, searchNext, ignoreStartIndex, value);
		}
		void DefaultViewOnBusyChanged(object sender, ItemsProviderChangedEventArgs e) {
			bool isBusy = ((ItemsProviderOnBusyChangedEventArgs)e).IsBusy;
			ProcessBusyChanged(DefaultDataViewHandle, isBusy);
		}
		void ProcessBusyChanged(object handle, bool isBusy) {
			bool totalBusy = !this.busyHandlers.IsEmpty();
			if (isBusy)
				this.busyHandlers.Add(handle);
			else {
				this.busyHandlers.Remove(handle);
			}
			bool newTotalBusy = !this.busyHandlers.IsEmpty();
			if (newTotalBusy != totalBusy)
				RaiseOnBusyChanged(new ItemsProviderOnBusyChangedEventArgs(newTotalBusy));
		}
		void DefaultViewRowLoaded(object sender, ItemsProviderChangedEventArgs e) {
			var rowLoadedArgs = (ItemsProviderRowLoadedEventArgs)e;
			ProcessRowLoadedForSnapshots(rowLoadedArgs.Value);
			RaiseRowLoaded(e);
		}
		void RaiseRowLoaded(ItemsProviderChangedEventArgs e) {
			if (RowLoaded != null)
				RowLoaded(this, (ItemsProviderRowLoadedEventArgs)e);
		}
		void ProcessRowLoadedForSnapshots(object value) {
			foreach (var pair in Snapshots)
				ProcessRowLoadedForSnapshot(pair.Value, value);
		}
		void ProcessRowLoadedForSnapshot(DataControllerSnapshotDescriptor descriptor, object value) {
			var currentDataView = Views[descriptor.Handle];
			currentDataView.ProcessRowLoaded(value);
		}
		void DefaultViewListChanged(object sender, ListChangedEventArgs e) {
			ProcessListChangedForSnapshots(e);
			RaiseListChanged(this, e);
		}
		void ProcessListChangedForSnapshots(ListChangedEventArgs e) {
			foreach (var pair in this.Snapshots)
				ProcessListChangedForSnapshot(pair.Value, e);
		}
		void ProcessListChangedForSnapshot(DataControllerSnapshotDescriptor descriptor, ListChangedEventArgs e) {
			var currentDataView = Views[descriptor.Handle];
			if (!currentDataView.ProcessChangeSource(e))
				UpdateSnapshot(descriptor);
		}
		void DefaultViewRefreshNeeded(object sender, EventArgs e) {
			ProcessRefreshedForSnapshots();
			RaiseRefreshed();
		}
		void UnsubscribeDefaultDataView(DefaultDataView view) {
			view.RefreshNeeded -= DefaultViewRefreshNeeded;
			view.InconsistencyDetected -= DefaultViewInconsistencyDetected;
			view.ListChanged -= DefaultViewListChanged;
			view.RowLoaded -= DefaultViewRowLoaded;
			view.BusyChanged -= DefaultViewOnBusyChanged;
			view.FindIncrementalCompleted -= DefaultViewFindIncrementalCompleted;
			(view.ListSource as IBindingList).Do(x => x.ListChanged -= ListChangedHandler.Handler);
			(view as CollectionViewDefaultDataView).Do(x => x.CurrentChanged -= CollectionViewCurrentChanged);
		}
		void CollectionViewCurrentChanged(object sender, ItemsProviderCurrentChangedEventArgs e) {
			RaiseCurrentChanged(e);
		}
		void DefaultViewInconsistencyDetected(object sender, EventArgs e) {
			if (endInitLocker.IsLocked)
				return;
			ProcessInconsistencyDetectedForSnapshots();
			RaiseRefreshed();
		}
		void ProcessInconsistencyDetectedForSnapshots() {
			if (!DefaultDataView.ProcessInconsistencyDetected()) {
				UpdateDefaultView();
				ResetSnapshots();
				UpdateSnapshots();
				return;
			}
			foreach (var pair in Snapshots)
				ProcessInconsistencyDetectedForSnapshot(pair.Value);
		}
		void ProcessInconsistencyDetectedForSnapshot(DataControllerSnapshotDescriptor descriptor) {
			var currentDataView = Views[descriptor.Handle];
			if (!currentDataView.ProcessInconsistencyDetected())
				UpdateSnapshot(descriptor);
		}
		public void UpdateItemsSource() {
			if (snapshotsDestroyed)
				ListSource = Enumerable.Empty<object>().ToList();
			else 
				ListSource = ExtractDataSource(Owner.ItemsSource) ?? ExtractDataSource(Owner.Items) ?? Enumerable.Empty<object>().ToList();
			Reset();
		}
		void ItemsSourceListChanged(object sender, ListChangedEventArgs e) {
			if (Dispatcher == null || Dispatcher.CheckAccess()) {
				ProcessItemsSourceListChanged(e);
			}
			else {
#pragma warning disable 0618
				if (DXGridDataController.DisableThreadingProblemsDetection)
#pragma warning restore 0618
					Dispatcher.BeginInvoke(new Action(() => ProcessItemsSourceListChanged(e)));
				else {
					Dispatcher.BeginInvoke(new Action(ThrowCrossThreadException));
					ThrowCrossThreadException();
				}
			}
		}
		static void ThrowCrossThreadException() {
			throw new InvalidOperationException("Cross thread operation detected. To suppress this exception, set DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true");
		}
		void ProcessItemsSourceListChanged(ListChangedEventArgs e) {
			DefaultDataView.ProcessListChanged(e);
		}
		protected internal void ProcessChangeItem(ListChangedEventArgs e) {
			DefaultDataView.ProcessChangeSource(e);
			ProcessListChangedForSnapshots(e);
		}
		public void SetFilterCriteria(string criteria) {
			if (Equals(FilterCriteria, criteria))
				return;
			FilterCriteria = criteria;
			if (endInitLocker.IsLocked)
				return;
			UpdateFilterCriteria();
		}
		void UpdateFilterCriteria() {
			if (!DefaultDataView.ProcessChangeFilter(FilterCriteria)) {
				UpdateDefaultView();
				ResetSnapshots();
			}
			UpdateSnapshots();
		}
		void ResetSnapshots() {
			foreach (var pair in Snapshots)
				ResetSnapshot(pair.Value);
		}
		void ResetSnapshot(DataControllerSnapshotDescriptor descriptor) {
			CurrentDataView currentView;
			var handle = descriptor.Handle;
			if (Views.TryGetValue(handle, out currentView)) {
				Views.Remove(handle);
				currentView.Dispose();
			}
		}
		void ProcessRefreshedForSnapshots() {
			foreach (var pair in Snapshots)
				ProcessRefreshedForSnapshot(pair.Value);
		}
		void ProcessRefreshedForSnapshot(DataControllerSnapshotDescriptor descriptor) {
			var currentDataView = Views[descriptor.Handle];
			currentDataView.ProcessRefreshed();
		}
		void UpdateSnapshots() {
			foreach (var pair in Snapshots)
				UpdateSnapshot(pair.Value);
			RaiseRefreshed();
		}
		void UpdateSnapshot(DataControllerSnapshotDescriptor descriptor) {
			CurrentDataView currentView;
			object handle = descriptor.Handle;
			string actualFilter = CalcActualFilterString(FilterCriteria, descriptor.FilterCriteria);
			if (Views.TryGetValue(handle, out currentView)) {
				if (currentView.ProcessChangeSortFilter(descriptor.Groups, descriptor.Sorts, actualFilter, descriptor.DisplayFilterCriteria)) {
					return;
				}
				UnsubscribeSnapshot(currentView);
				currentView.Dispose();
				Views.Remove(handle);
			}
			currentView = DefaultDataView.CreateCurrentDataView(descriptor.Handle, descriptor.Groups, descriptor.Sorts, actualFilter, descriptor.DisplayFilterCriteria);
			currentView.Initialize();
			SubscribeSnapshot(currentView);
			Views[handle] = currentView;
		}
		void UnsubscribeSnapshot(CurrentDataView view) {
			view.RefreshNeeded -= SnapshotRefreshNeeded;
			view.RowLoaded -= SnapshotRowLoaded;
			view.BusyChanged -= SnapshotBusyChanged;
			view.FindIncrementalCompleted -= SnapshotFindIncrementalCompleted;
			view.InconsistencyDetected -= SnapshotInconsistencyDetected;
		}
		void SubscribeSnapshot(CurrentDataView view) {
			view.RefreshNeeded += SnapshotRefreshNeeded;
			view.RowLoaded += SnapshotRowLoaded;
			view.BusyChanged += SnapshotBusyChanged;
			view.FindIncrementalCompleted += SnapshotFindIncrementalCompleted;
			view.InconsistencyDetected += SnapshotInconsistencyDetected;
		}
		void SnapshotInconsistencyDetected(object sender, EventArgs e) {
			RaiseRefreshed();
		}
		void SnapshotBusyChanged(object sender, ItemsProviderChangedEventArgs e) {
			var view = (CurrentDataView)sender;
			object handle = view.Handle;
			var args = (ItemsProviderOnBusyChangedEventArgs)e;
			ProcessBusyChanged(handle, args.IsBusy);
		}
		void SnapshotFindIncrementalCompleted(object sender, ItemsProviderChangedEventArgs e) {
			var findIncremental = (ItemsProviderFindIncrementalCompletedEventArgs)e;
			DefaultDataView.ProcessFindIncremental(findIncremental);
		}
		void SnapshotRowLoaded(object sender, ItemsProviderChangedEventArgs args) {
			var currentView = (CurrentDataView)sender;
			RaiseRowLoaded(args);
		}
		void SnapshotRefreshNeeded(object sender, EventArgs e) {
			var currentView = (CurrentDataView)sender;
			RaiseViewRefreshed(currentView.Handle);
		}
		void RaiseFindIncrementalCompleted(object sender, ItemsProviderFindIncrementalCompletedEventArgs e) {
			if (FindIncrementalCompleted != null)
				FindIncrementalCompleted(sender, e);
		}
		string CalcActualFilterString(string filterCriteria, string displayFilterCriteria) {
			bool hasFilterCriteria = !string.IsNullOrEmpty(filterCriteria);
			bool hasDisplayFilterCriteria = !string.IsNullOrEmpty(displayFilterCriteria);
			if (hasFilterCriteria)
				return hasDisplayFilterCriteria ? CriteriaOperator.And(CriteriaOperator.Parse(filterCriteria), CriteriaOperator.Parse(displayFilterCriteria)).ToString() : filterCriteria;
			return hasDisplayFilterCriteria ? displayFilterCriteria : string.Empty;
		}
		void RaiseCurrentChanged(ItemsProviderCurrentChangedEventArgs e) {
			CurrentChanged.Do(x => x(this, e));
		}
		void RaiseOnBusyChanged(ItemsProviderOnBusyChangedEventArgs e) {
			BusyChanged.Do(x => x(this, e));
		}
		void RaiseListChanged(object sender, ListChangedEventArgs e) {
			ListChanged.Do(x => x(sender, e));
		}
		void RaiseRefreshed() {
			Refreshed.Do(x => x(this, EventArgs.Empty));
		}
		void RaiseViewRefreshed(object handle) {
			var viewRefreshed = ViewRefreshed;
			if (viewRefreshed != null)
				viewRefreshed(this, new ItemsProviderViewRefreshedEventArgs(handle));
		}
		DefaultDataView CreateDefaultDataView() {
			return ListSource.If(x => x is IListServer || x is IAsyncListServer)
				.Return(x => CreateServerModeDefaultDataView(), CreateLocalDefaultDataView);
		}
		PlainListDataView CreateLocalDefaultDataView() {
			return new PlainListDataView(ListSource, Owner.ValueMember, Owner.DisplayMember,
				Enumerable.Empty<GroupingInfo>(), Enumerable.Empty<SortingInfo>(), FilterCriteria.With(x => x.ToString()));
		}
		DefaultDataView CreateServerModeDefaultDataView() {
			object source = ListSource;
			if (source is ICollectionViewHelper)
				return new CollectionViewDefaultDataView(Owner.AllowCollectionView, () => Owner.IsSynchronizedWithCurrentItem, (IListServer)source, Owner.ValueMember, Owner.DisplayMember,
					Enumerable.Empty<GroupingInfo>(), Enumerable.Empty<SortingInfo>(), FilterCriteria.With(fc => fc.ToString()));
			if (source is IAsyncListServer)
				return new AsyncListServerDefaultDataView((IAsyncListServer)source, Owner.ValueMember, Owner.DisplayMember, Enumerable.Empty<GroupingInfo>(), Enumerable.Empty<SortingInfo>(), FilterCriteria.With(x => x.ToString()));
			if (source is IListServer)
				return new SyncListServerDefaultDataView((IListServer)source, Owner.ValueMember, Owner.DisplayMember, Enumerable.Empty<GroupingInfo>(), Enumerable.Empty<SortingInfo>(), FilterCriteria.With(x => x.ToString()));
			return new ListServerDataView((IListServer)source, Owner.ValueMember, Owner.DisplayMember, Enumerable.Empty<GroupingInfo>(), Enumerable.Empty<SortingInfo>(), FilterCriteria.With(fc => fc.ToString()));
		}
		public virtual IList ExtractDataSource(object itemsSource) {
			if (Owner.AllowCollectionView) {
				ICollectionView view = itemsSource as ICollectionView;
				if (view == null) {
					CollectionViewSource source = new CollectionViewSource { Source = itemsSource };
					view = source.View;
				}
				return DataBindingHelper.ExtractDataSource(view, Owner.AllowLiveDataShaping);
			}
			return DataBindingHelper.ExtractDataSource(itemsSource, Owner.AllowLiveDataShaping);
		}
		public string FilterCriteria { get; private set; }
		public bool IsAsyncServerMode { get { return DefaultDataView.GetIsAsyncServerMode(); } }
		public bool IsSyncServerMode { get { return !IsAsyncServerMode && DefaultDataView.GetIsOwnSearchProcessing(); } }
		public object GetItemByListSourceIndex(int index, object handle = null) {
			int visibleIndex = GetVisibleIndexByListSourceIndex(index, handle);
			var view = Views[handle ?? CurrentDataViewHandle];
			var proxy = view.GetProxyByIndex(visibleIndex);
			if (proxy == null)
				return null;
			return view.GetItemByProxy(proxy);
		}
		public int GetVisibleIndexByListSourceIndex(int index, object handle = null) {
			if (index < 0 || index >= DefaultDataView.VisibleRowCount)
				return -1;
			var proxy = DefaultDataView[index];
			if (proxy == null)
				return -1;
			object value = DefaultDataView.GetValueFromProxy(proxy);
			var view = Views[handle ?? CurrentDataViewHandle];
			return view.IndexOfValue(value);
		}
		public int GetListSourceIndexByVisibleIndex(int visibleIndex, object handle) {
			if (visibleIndex < 0)
				return -1;
			var view = Views[handle ?? CurrentDataViewHandle];
			if (visibleIndex >= view.VisibleRowCount)
				return -1;
			var proxy = view[visibleIndex];
			if (proxy == null)
				return -1;
			object value = view.GetValueFromProxy(proxy);
			return DefaultDataView.IndexOfValue(value);
		}
		public int IndexOfValue(object value, object handle = null) {
			CurrentDataView view = this.Views[handle ?? this.CurrentDataViewHandle];
			int currentViewIndex = view.IndexOfValue(value);
			if (currentViewIndex == -1)
				return -1;
			return DefaultDataView.IndexOfValue(value);
		}
		public void Reset() {
			BeginInit();
			EndInit();
		}
		public int FindItemIndexByText(string text, bool isCaseSensitiveSearch, bool allowTextInputSuggestions, object handle = null, int startItemIndex = -1, bool searchNext = true, bool ignoreStartIndex = false) {
			var view = Views[handle ?? CurrentDataViewHandle];
			int currentIndex = startItemIndex == -1
				? startItemIndex
				: GetVisibleIndexByListSourceIndex(startItemIndex, handle);
			int currentViewIndex = view.FindItemIndexByText(text, isCaseSensitiveSearch, allowTextInputSuggestions, currentIndex, searchNext, ignoreStartIndex);
			if (currentViewIndex == -1)
				return -1;
			var proxy = view.GetProxyByIndex(currentViewIndex);
			if (proxy == null)
				return -1;
			object value = view.GetValueFromProxy(proxy);
			return DefaultDataView.IndexOfValue(value);
		}
		public object GetValue(object item, bool fromObject, object handle = null) {
			object value;
			var cdv = Views[handle ?? CurrentDataViewHandle];
			if (fromObject) {
				value = cdv.GetValueFromProxy(cdv.CreateProxy(item, -1));
			}
			else {
				int listSourceIndex = DefaultDataView.IndexOf(item);
				int visibleIndex = GetVisibleIndexByListSourceIndex(listSourceIndex);
				var proxy = cdv.GetProxyByIndex(visibleIndex);
				value = proxy != null ? cdv.GetValueFromProxy(proxy) : null;
			}
			return LookUpPropertyDescriptorBase.IsUnsetValue(value) ? null : value;
		}
		public void ResetDisplayTextCache() {
			foreach (var view in Views.Values)
				view.ResetDisplayTextCache();
		}
		public int IndexOf(object item, object handle = null) {
			int listSourceIndex = DefaultDataView.IndexOf(item);
			return GetVisibleIndexByListSourceIndex(listSourceIndex, handle);
		}
		public object GetDisplayValueByListSourceIndex(int listSourceIndex, object handle = null) {
			if (listSourceIndex < 0)
				return null;
			int visibleIndex = GetVisibleIndexByListSourceIndex(listSourceIndex, handle);
			var view = Views[handle ?? CurrentDataViewHandle];
			var proxy = view.GetProxyByIndex(visibleIndex);
			if (proxy == null)
				return null;
			return view.GetDisplayValueFromProxy(proxy);
		}
		public object GetValueByListSourceIndex(int listSourceIndex, object handle = null) {
			if (listSourceIndex < 0)
				return null;
			int visibleIndex = GetVisibleIndexByListSourceIndex(listSourceIndex, handle);
			var view = Views[handle ?? CurrentDataViewHandle];
			var proxy = view.GetProxyByIndex(visibleIndex);
			if (proxy == null)
				return null;
			return view.GetValueFromProxy(proxy);
		}
		public IEnumerable GetVisibleList(object handle) {
			var view = Views[handle ?? CurrentDataViewHandle];
			return view.Return(x => x.VisibleList as IEnumerable, () => null);
		}
		public void ResetVisibleList(object handle) {
			var view = Views[handle ?? CurrentDataViewHandle];
			view.Do(x => x.ResetVisibleList());
		}
		public void CancelAsyncOperations(object handle) {
			var view = Views[handle ?? CurrentDataViewHandle];
			view.Do(x => x.CancelAsyncOperations());
		}
		public string GetDisplayPropertyName(object handle = null) {
			var view = Views[handle ?? CurrentDataViewHandle];
			return view.DataAccessor.DisplayPropertyName;
		}
		public string GetValuePropertyName(object handle) {
			var view = Views[handle ?? CurrentDataViewHandle];
			return view.DataAccessor.ValuePropertyName;
		}
		public int GetVisibleRowCount(object handle) {
			return Views[handle ?? CurrentDataViewHandle].VisibleRowCount;
		}
		public void Refresh() {
			if (!Owner.IsLoaded) {
				snapshotsDestroyed = true;
				UpdateItemsSource();
				return;
			}
			if (Owner.IsLoaded && snapshotsDestroyed) {
				snapshotsDestroyed = false;
				UpdateItemsSource();
				return;
			}
			if (!DefaultDataView.ProcessRefresh()) {
				Reset();
				return;
			}
			RefreshSnapshots();
		}
		void RefreshSnapshots() {
			foreach (var pair in Snapshots)
				RefreshSnapshot(pair.Value);
		}
		void RefreshSnapshot(DataControllerSnapshotDescriptor descriptor) {
			CurrentDataView currentView;
			object handle = descriptor.Handle;
			bool result = false;
			if (Views.TryGetValue(handle, out currentView)) {
				result = currentView.ProcessRefresh();
			}
			if (!result)
				UpdateSnapshot(descriptor);
		}
	}
}
