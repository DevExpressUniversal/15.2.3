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
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Editors.Helpers {
	public class ItemsProvider2 : IItemsProvider2 {
		public IEnumerable VisibleList { get { return (IEnumerable)DataController.VisibleList; }}
		public DataController DataController { get; private set; }
		protected IItemsProviderOwner Owner { get; private set; }
		public object CurrentDataViewHandle { get { return DataController.CurrentDataViewHandle; } }
		IItemsProviderCollectionViewSupport CollectionViewSupport { get { return DataController.DefaultDataView as IItemsProviderCollectionViewSupport; } }
		public ItemsProvider2(IItemsProviderOwner owner) {
			Owner = owner;
			DataController = CreateDataController();
			DataController.ListChanged += DataControllerOnListChanged;
			DataController.Refreshed += DataControllerRefreshed;
			DataController.RowLoaded += DataControllerRowLoaded;
			DataController.CurrentChanged += DataControllerCurrentChanged;
			DataController.BusyChanged += DataControllerBusyChanged;
			DataController.ViewRefreshed += DataControllerViewRefreshed;
			DataController.FindIncrementalCompleted += DataControllerFindIncrementalCompleted;
		}
		void DataControllerFindIncrementalCompleted(object sender, ItemsProviderFindIncrementalCompletedEventArgs e) {
			RaiseFindIncrementalCompleted(e);
		}
		void DataControllerViewRefreshed(object sender, ItemsProviderViewRefreshedEventArgs e) {
			RaiseViewRefreshed(e);
		}
		void DataControllerBusyChanged(object sender, ItemsProviderOnBusyChangedEventArgs e) {
			RaiseBusyChanged(e);
		}
		void DataControllerRowLoaded(object sender, ItemsProviderRowLoadedEventArgs e) {
			RaiseRowLoaded(e);
		}
		void DataControllerCurrentChanged(object sender, ItemsProviderCurrentChangedEventArgs e) {
			RaiseCurrentChanged(e.CurrentItem);
		}
		void DataControllerRefreshed(object sender, EventArgs e) {
			Refreshed();
		}
		public void ResetVisibleList(object handle) {
			DataController.ResetVisibleList(handle);			
		}
		public void CancelAsyncOperations(object handle) {
			DataController.CancelAsyncOperations(handle);
		}
		protected virtual DataController CreateDataController() {
			return new DataController(Owner);
		}
		public int GetCount(object handle) {
			return DataController.GetVisibleRowCount(handle);
		}
		public int IndexOfValue(object value, object handle = null) {
			return DataController.IndexOfValue(value, handle);
		}
		public object GetItem(object value, object handle = null) {
			int index = IndexOfValue(value, handle);
			return index < 0 ? null : DataController.GetItemByListSourceIndex(index, handle);
		}
		public object GetValueByIndex(int index, object handle = null) {
			if (index < 0)
				return null;
			return DataController.GetValueByListSourceIndex(index, handle);
		}
		public object GetDisplayValueByIndex(int index, object handle) {
			if (index < 0)
				return null;
			return DataController.GetDisplayValueByListSourceIndex(index, handle);
		}
		public event ItemsProviderChangedEventHandler ItemsProviderChanged;
		public int Count { get { return DataController.CurrentDataView.VisibleRowCount; } }
		public IEnumerable<string> AvailableColumns { get { return DataController.DefaultDataView.AvailableColumns; }}
		public CriteriaOperator ActualFilterCriteria { get { return CalcActualFilterCriteria(); } }
		CriteriaOperator CalcActualFilterCriteria() {
			return CriteriaOperator.Parse(DataController.GetSnapshot(DataController.CurrentDataViewHandle).FilterCriteria);
		}
		public bool IsAsyncServerMode { get { return DataController.IsAsyncServerMode; } }
		public bool IsSyncServerMode { get { return DataController.IsSyncServerMode; } }
		public bool IsServerMode { get { return IsSyncServerMode || IsAsyncServerMode; } }
		public bool NeedsRefresh { get { return DataController.NeedsRefresh; } }
		public bool IsInLookUpMode { get; private set; }
		public bool HasValueMember { get { return !string.IsNullOrEmpty(Owner.ValueMember); } }
		public IEnumerable VisibleListSource { get { return VisibleList; } }
		public void UpdateDisplayMember() {
			DataController.Reset();
			UpdateIsInLookUpMode();
		}
		void UpdateIsInLookUpMode() {
			IsInLookUpMode = Owner.AllowRejectUnknownValues || (!string.IsNullOrEmpty(Owner.DisplayMember) || !string.IsNullOrEmpty(Owner.ValueMember));
		}
		public void UpdateValueMember() {
			UpdateIsInLookUpMode();
			DataController.Reset();
		}
		public void ProcessSelectionChanged(NotifyItemsProviderSelectionChangedEventArgs e) {
			RaiseSelectionChanged(GetValueFromItem(e.Item), e.IsSelected);
		}
		public void ProcessCollectionChanged(NotifyItemsProviderChangedEventArgs e) {
			if (e.ChangedType == ListChangedType.ItemChanged) {
				ProcessItemChanged(e);
				RaiseDataChanged(ListChangedType.ItemChanged);
			}
			else
				Reset();
		}
		void ProcessItemChanged(NotifyItemsProviderChangedEventArgs e) {
			int index = DataController.IndexOf(e.Item);
			if (index < 0)
				return;
			DataController.ProcessChangeItem(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			ResetDisplayTextCache();
		}
		public object GetValueByRowKey(object rowKey, object handle) {
			int listSourceIndex = IsServerMode
				? string.IsNullOrEmpty(Owner.ValueMember) ? DataController.IndexOf(rowKey, handle) : DataController.IndexOfValue(rowKey, handle)
				: DataController.IndexOf(rowKey, handle);
			return DataController.GetValueByListSourceIndex(listSourceIndex, handle);
		}
		public object GetValueFromItem(object item, object handle = null) {
			return DataController.GetValue(item, !IsInLookUpMode, handle);
		}
		public void Reset() {
			DataController.Reset();
		}
		void Refreshed() {
			LogBase.Add(Owner, null, "Refreshed");
			RaiseRefreshed();
		}
		public object this[int index] {
			get { return GetItemByControllerIndex(index); }
		}
		public int GetIndexByItem(object item, object handle = null) {
			return DataController.IndexOf(item, handle);
		}
		public void DoRefresh() {
			DataController.Refresh();
		}
		public void UpdateItemsSource() {
			DataController.UpdateItemsSource();
		}
		public int FindItemIndexByText(string text, bool isCaseSensitiveSearch, bool allowTextInputSuggestions, object handle, int startItemIndex = -1, bool searchNext = true, bool ignoreStartIndex = false) {
			return DataController.FindItemIndexByText(text, isCaseSensitiveSearch, allowTextInputSuggestions,  handle, startItemIndex, searchNext, ignoreStartIndex);
		}
		public object GetDisplayValueByEditValue(object editValue, object handle = null) {
			int index = DataController.IndexOfValue(editValue, handle);
			if (index < 0)
				return IsInLookUpMode ? null : editValue;
			return DataController.GetDisplayValueByListSourceIndex(index, handle);
		}
		public int GetControllerIndexByIndex(int index, object handle = null) {
			return DataController.GetVisibleIndexByListSourceIndex(index, handle);
		}
		public int GetIndexByControllerIndex(int controllerIndex, object handle = null) {
			return DataController.GetListSourceIndexByVisibleIndex(controllerIndex, handle);
		}
		public object GetItemByControllerIndex(int visibleIndex, object handle = null) {
			int index = DataController.GetListSourceIndexByVisibleIndex(visibleIndex, handle);
			return index < 0 ? null : DataController.GetItemByListSourceIndex(index, handle);
		}
		public void ResetDisplayTextCache() {
			DataController.ResetDisplayTextCache();
		}
		public void UpdateFilterCriteria() {
			DataController.SetFilterCriteria(Owner.FilterCriteria.With(x => x.ToString()));
		}
		public CriteriaOperator CreateDisplayFilterCriteria(string searchText, FilterCondition filterCondition) {
			if (string.IsNullOrEmpty(searchText))
				return null;
			FunctionOperatorType functionOperatorType = GetFunctionOperatorType(filterCondition);
			return new FunctionOperator(functionOperatorType, new OperandProperty(DataController.GetDisplayPropertyName()), searchText);
		}
		protected virtual FunctionOperatorType GetFunctionOperatorType(FilterCondition filterCondition) {
			return filterCondition == FilterCondition.Contains ? FunctionOperatorType.Contains : FunctionOperatorType.StartsWith;
		}
#if DEBUGTEST
		public DataControllerItemsCache ItemsCache { get { return DataController.CurrentDataView.ItemsCache; } }
#endif
		void DataControllerOnListChanged(object sender, ListChangedEventArgs e) {
			LogBase.Add(Owner, e.ListChangedType, "DataController_ListChanged");
			int listSourceIndex = e.NewIndex;
			ListChangedType listChanged = e.ListChangedType;
			RaiseDataChanged(listChanged, listSourceIndex, e.PropertyDescriptor);
		}
		void RaiseRefreshed() {
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, ItemsProviderRefreshedEventArgs.Instance);
		}
		void RaiseFindIncrementalCompleted(ItemsProviderFindIncrementalCompletedEventArgs args) {
			Guard.ArgumentNotNull(args, "args");
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, args);
		}
		void RaiseViewRefreshed(ItemsProviderViewRefreshedEventArgs args) {
			Guard.ArgumentNotNull(args, "args");
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, args);
		}
		public void RaiseBusyChanged(ItemsProviderOnBusyChangedEventArgs args) {
			Guard.ArgumentNotNull(args, "args");
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, args);
		}
		public void RaiseRowLoaded(ItemsProviderRowLoadedEventArgs args) {
			Guard.ArgumentNotNull(args, "args");
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, args);
		}
		public void RaiseDataChanged(ItemsProviderDataChangedEventArgs args) {
			Guard.ArgumentNotNull(args, "args");
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, args);
		}
		public void RaiseSelectionChanged(object editValue, bool isSelected) {
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, new ItemsProviderSelectionChangedEventArgs(editValue, isSelected));
		}
		public void RaiseDataChanged(ListChangedType changedType = ListChangedType.Reset, int newIndex = -1, PropertyDescriptor descriptor = null) {
			RaiseDataChanged(new ItemsProviderDataChangedEventArgs(changedType, newIndex, descriptor));
		}
		public void RaiseCurrentChanged(object currentItem) {
			if (ItemsProviderChanged != null)
				ItemsProviderChanged(this, new ItemsProviderCurrentChangedEventArgs(currentItem));
		}
		ICollectionViewHelper IItemsProviderCollectionViewSupport.DataSync { get { return null; } }
		ICollectionView IItemsProviderCollectionViewSupport.ListSource { get { return null; } }
		bool IItemsProviderCollectionViewSupport.IsSynchronizedWithCurrentItem { get { return false; } }
		void IItemsProviderCollectionViewSupport.RaiseCurrentChanged(object currentItem) { }
		public void SetCurrentItem(object currentItem) {
			CollectionViewSupport.Do(x => x.SetCurrentItem(currentItem));
		}
		public void SyncWithCurrentItem() {
			CollectionViewSupport.Do(x => x.SyncWithCurrentItem());
		}
		public void RegisterSnapshot(object handle) {
			DataController.RegisterSnapshot(new DataControllerSnapshotDescriptor(handle));
		}
		public void ReleaseSnapshot(object handle) {
			var snapshot = DataController.GetSnapshot(handle);
			if (snapshot != null)
				DataController.ReleaseSnapshot(snapshot);
		}
		public void SetFilterCriteria(CriteriaOperator criteria, object handle) {
			var snapshot = DataController.GetSnapshot(handle);
			if (snapshot != null) {
				snapshot.SetFilterCriteria(criteria);
			}
		}
		public void SetDisplayFilterCriteria(CriteriaOperator criteria, object handle) {
			var snapshot = DataController.GetSnapshot(handle);
			if (snapshot != null) {
				snapshot.SetDisplayFilterCriteria(criteria);
			}
		}
		public IEnumerable GetVisibleListSource(object handle) {
			return DataController.GetVisibleList(handle);
		}
		public string GetDisplayPropertyName(object handle) {
			return DataController.GetDisplayPropertyName(handle);
		}
		public string GetValuePropertyName(object handle) {
			return DataController.GetValuePropertyName(handle);
		}
	}
}
