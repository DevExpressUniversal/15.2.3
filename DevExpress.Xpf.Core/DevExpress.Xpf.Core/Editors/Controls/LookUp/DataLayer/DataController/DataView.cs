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
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class DataView : IEnumerable<DataProxy>, IDataControllerAdapter, IDisposable {
		public static bool GetIsCurrentViewFIltered(IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria) {
			return sorts.If(x => x.Count > 0).ReturnSuccess() || groups.If(x => x.Count > 0).ReturnSuccess() || !string.IsNullOrEmpty(filterCriteria);
		}
		public event EventHandler InconsistencyDetected;
		public event EventHandler RefreshNeeded;
		public event ListChangedEventHandler ListChanged;
		public event ItemsProviderChangedEventHandler RowLoaded;
		public event ItemsProviderChangedEventHandler BusyChanged;
		public event ItemsProviderChangedEventHandler FindIncrementalCompleted;
		DataProxyViewCache view;
		readonly object listSource;
		readonly string valueMember;
		readonly string displayMember;
		readonly DataControllerItemsCache itemsCache;
		public int VisibleRowCount { get { return view.Count; } }
		protected internal DataProxyViewCache View { get { return view; } }
		protected Type ElementType { get { return dataAccessor.ElementType; } }
		DataAccessor dataAccessor;
		public DataAccessor DataAccessor { get { return dataAccessor; } }
		public object ListSource { get { return listSource; } }
		protected virtual object OriginalSource { get { return listSource; } }
		public DataControllerItemsCache ItemsCache { get { return itemsCache; } }
		bool disposed;
		protected DataView(object listSource, string valueMember, string displayMember) {
			this.listSource = listSource;
			this.valueMember = valueMember;
			this.displayMember = displayMember;
			itemsCache = CreateItemsCache();
		}
		protected virtual DataAccessor CreateEditorsDataAccessor() {
			var accessor = CreateEditorsDataAccessorInstance();
			FetchDescriptors(accessor);
			return accessor;
		}
		protected virtual DataControllerItemsCache CreateItemsCache() {
			return new DataControllerItemsCache(this);
		}
		protected virtual DataAccessor CreateEditorsDataAccessorInstance() {
			return new DataAccessor();
		}
		void FetchDescriptors(DataAccessor accessor) {
			accessor.BeginInit();
			FetchDescriptorsInternal(accessor);
			accessor.EndInit();
		}
		protected virtual void FetchDescriptorsInternal(DataAccessor accessor) {
			accessor.ResetDescriptors();
			accessor.GenerateDefaultDescriptors(valueMember, displayMember, GetDescriptorFromCollection);
		}
		protected virtual PropertyDescriptor GetDescriptorFromCollection(string name) {
			ITypedList typedList = OriginalSource as ITypedList;
			if (typedList != null) {
				PropertyDescriptor result = typedList.GetItemProperties(null).Find(name, true);
				if (result != null)
					return result;
			}
			object originalSource = CalcDescriptorsFromOriginalSource(OriginalSource);
			if (originalSource is IEnumerable) {
				var enumerableGenericParameter = originalSource.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).FirstOrDefault();
				if (enumerableGenericParameter != null) {
					var result = TypeDescriptor.GetProperties(enumerableGenericParameter).Find(name, false);
					if (result != null)
						return result;
				}
			}
			return null;
		}
		object CalcDescriptorsFromOriginalSource(object originalSource) {
			var adapter = originalSource as BindingListAdapter;
			if (adapter != null)
				return adapter.OriginalDataSource;
			var defaultDataView = originalSource as DefaultDataView;
			if (defaultDataView != null)
				return CalcDescriptorsFromOriginalSource(defaultDataView.OriginalSource);
			return originalSource;
		}
		public virtual void Initialize() {
			UpdateEditorsDataAccessor();
			InitializeView(listSource);
		}
		public virtual void Release() {
			ReleaseView(ListSource);
		}
		protected virtual void InitializeView(object source) {
		}
		protected internal virtual void SubscribeToEvents() {
		}
		protected virtual void UnsubscribeFromEvents() {
		}
		protected virtual void ReleaseView(object source) {
		}
		protected void UpdateEditorsDataAccessor() {
			dataAccessor = CreateEditorsDataAccessor();
		}
		protected void SetView(DataProxyViewCache view) {
			this.view = view;
		}
		public virtual IEnumerator<DataProxy> GetEnumerator() {
			return view.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public object GetValueFromProxy(DataProxy proxy) {
			return DataAccessor.GetValue(proxy);
		}
		public object GetDisplayValueFromProxy(DataProxy proxy) {
			return DataAccessor.GetDisplayValue(proxy);
		}
		public object GetValueFromItem(object item) {
			int index = itemsCache.IndexByItem(item);
			var proxy = GetProxyByIndex(index);
			return GetValueFromProxy(proxy);
		}
		public object GetDisplayValueFromItem(object item) {
			int index = itemsCache.IndexByItem(item);
			var proxy = GetProxyByIndex(index);
			return GetDisplayValueFromProxy(proxy);
		}
		public DataProxy GetProxyByIndex(int index) {
			if (index < 0 || index >= view.Count)
				return null;
			return view[index];
		}
		public object GetItemByProxy(DataProxy proxy) {
			return proxy.f_component;
		}
		public int GetListSourceIndexByValue(object value) {
			return itemsCache.IndexOfValue(value);
		}
		public int IndexOfValue(object value) {
			return itemsCache.IndexOfValue(value);
		}
		public int IndexOf(object item) {
			return itemsCache.IndexByItem(item);
		}
		public DataProxy CreateProxy(object item, int index) {
			return DataAccessor.CreateProxy(item, index);
		}
		#region IDataControllerAdapter
		bool IDataControllerAdapter.IsOwnSearchProcessing {
			get { return GetIsOwnSearchProcessing(); }
		}
		protected internal virtual bool GetIsOwnSearchProcessing() {
			return false;
		}
		protected internal virtual bool GetIsAsyncServerMode() {
			return false;
		}
		object IDataControllerAdapter.GetRowValue(object item) {
			DataProxy proxy = DataAccessor.CreateProxy(item, -1);
			return GetValueFromProxy(proxy);
		}
		object IDataControllerAdapter.GetRowValue(int visibleIndex) {
			var item = GetProxyByIndex(visibleIndex);
			return GetValueFromProxy(item);
		}
		object IDataControllerAdapter.GetRow(int listSourceIndex) {
			return DataControllerAdapterGetRowInternal(listSourceIndex);
		}
		int IDataControllerAdapter.GetListSourceIndex(object value) {
			return FindListSourceIndexByValue(value);
		}
		protected virtual object DataControllerAdapterGetRowInternal(int visibleIndex) {
			return visibleIndex > -1 ? view[visibleIndex].f_component : null;
		}
		protected virtual int FindListSourceIndexByValue(object value) {
			return -1;
		}
		#endregion
		#region IList
		public DataProxy this[int index] {
			get { return view[index]; }
		}
		#endregion
		protected internal virtual bool ProcessChangeSource(ListChangedEventArgs e) {
			bool result = false;
			if (e.ListChangedType == ListChangedType.ItemAdded)
				result = ProcessAddItem(e);
			else if (e.ListChangedType == ListChangedType.ItemChanged)
				result = ProcessChangeItem(e);
			else if (e.ListChangedType == ListChangedType.ItemDeleted)
				result = ProcessDeleteItem(e);
			else if (e.ListChangedType == ListChangedType.ItemMoved)
				result = ProcessMoveItem(e);
			else if (e.ListChangedType == ListChangedType.PropertyDescriptorAdded || e.ListChangedType == ListChangedType.PropertyDescriptorDeleted ||
					 e.ListChangedType == ListChangedType.PropertyDescriptorChanged || e.ListChangedType == ListChangedType.Reset)
				result = ProcessResetItem(e);
			return result;
		}
		bool ProcessResetItem(ListChangedEventArgs e) {
			return ProcessReset();
		}
		public virtual bool ProcessReset() {
			return false;
		}
		bool ProcessMoveItem(ListChangedEventArgs e) {
			return ProcessMoveItem(e.OldIndex, e.NewIndex);
		}
		bool ProcessDeleteItem(ListChangedEventArgs e) {
			return ProcessDeleteItem(e.NewIndex);
		}
		protected internal bool ProcessChangeItem(ListChangedEventArgs e) {
			return ProcessChangeItem(e.NewIndex);
		}
		bool ProcessAddItem(ListChangedEventArgs e) {
			return ProcessAddItem(e.NewIndex);
		}
		public virtual bool ProcessAddItem(int index) {
			return false;
		}
		public virtual bool ProcessChangeItem(int index) {
			return false;
		}
		public virtual bool ProcessDeleteItem(int index) {
			return false;
		}
		public virtual bool ProcessMoveItem(int oldIndex, int newIndex) {
			return false;
		}
		public virtual bool ProcessRefresh() {
			return false;
		}
		public void Dispose() {
			if (!disposed)
				DisposeInternal();
			disposed = true;
		}
		protected virtual void DisposeInternal() {
		}
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if (ListChanged != null)
				ListChanged(this, e);
		}
		protected virtual void RaiseOnBusyChanged(ItemsProviderOnBusyChangedEventArgs e) {
			if (BusyChanged != null)
				BusyChanged(this, e);
		}
		protected virtual void RaiseFindIncrementalCompleted(ItemsProviderFindIncrementalCompletedEventArgs e) {
			var findIncrementalCompleted = FindIncrementalCompleted;
			if (findIncrementalCompleted != null)
				findIncrementalCompleted(this, e);
		}
		protected virtual void RaiseRowLoaded(ItemsProviderRowLoadedEventArgs e) {
			if (RowLoaded != null)
				RowLoaded(this, e);
		}
		public virtual bool ProcessChangeSortFilter(IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			return false;
		}
		protected void RaiseInconsistencyDetected() {
			InconsistencyDetected.Do(x => x(this, EventArgs.Empty));
		}
		protected void RaiseRefreshNeeded() {
			RefreshNeeded.Do(x => x(this, EventArgs.Empty));
		}
	}
}
