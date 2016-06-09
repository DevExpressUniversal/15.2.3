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
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms.VisualStyles;
using DevExpress.Data.Linq;
using DevExpress.Utils;
using System.Windows.Data;
using System.Windows;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Xpf.Editors.Helpers {
	public class ItemsProvider : IItemsProvider, IDataControllerVisualClient, IEnumerable<object>, IDataControllerAdapter, IWeakEventListener {
		readonly IItemsProviderOwner owner;
		readonly Dictionary<DisplayTextCacheItem, int> editTextToDisplayTextCache = new Dictionary<DisplayTextCacheItem, int>();
		BaseListSourceDataController dataController;
		DataControllerICollectionViewSupport CollectionViewSupport { get; set; }
		public DataControllerItemsCache ItemsCache { get; private set; }
		IEnumerable visibleListSource;
		CriteriaOperator displayFilterCriteria;
		Locker InitializeDataControllerLocker { get; set; }
		string searchText;
		FilterCondition filterCondition;
		public ItemsProvider(IItemsProviderOwner owner) {
			InitializeDataControllerLocker = new Locker();
			this.owner = owner;
			CollectionViewSupport = CreateCollectionViewSupport();
			ItemsCache = new DataControllerItemsCache(this);
			InitializeDataController();
		}
		protected virtual DataControllerICollectionViewSupport CreateCollectionViewSupport() {
			return new DataControllerICollectionViewSupport(this);
		}
		public object CurrentDataViewHandle { get { return null; } }
		public bool IsInLookUpMode { get { return HasValueMember || HasDisplayMember; } }
		public event ItemsProviderChangedEventHandler ItemsProviderChanged;
		public IItemsProviderOwner Owner { get { return owner; } }
		public int Count { get { return DataController.VisibleListSourceRowCount; } }
		public int GetCount(object handle) {
			return Count;
		}
		public CriteriaOperator DisplayFilterCriteria {
			get { return displayFilterCriteria; }
			set {
				if (ReferenceEquals(displayFilterCriteria, value))
					return;
				displayFilterCriteria = value;
				UpdateDisplayFilter();
			}
		}
		void UpdateDisplayFilter() {
			ActualFilterCriteria = CalcActualFilterOperator();
			DataController.FilterCriteria = ActualFilterCriteria;
			ResetCaches();
		}
		public void SetSearchText(string text) {
			this.searchText = text;
			UpdateDisplayFilter();
		}
		public void SetFilterCondition(FilterCondition condition) {
			this.filterCondition = condition;
			UpdateDisplayFilter();
		}
		public CriteriaOperator ActualFilterCriteria { get; private set; }
		public IList ListSource { get; private set; }
		public DataControllerData DataControllerData { get { return DataController.DataClient as DataControllerData; } }
		public IEnumerable VisibleListSource { get { return visibleListSource ?? (visibleListSource = CreateVisibleListSource()); } }
		public IEnumerable<string> AvailableColumns { get { return GetAvailableColumns(); } }
		public bool HasDisplayMember { get { return !string.IsNullOrEmpty(Owner.DisplayMember); } }
		public bool HasValueMember { get { return !string.IsNullOrEmpty(Owner.ValueMember); } }
		public bool IsServerMode { get { return ListSource is IListServer; } }
		protected BaseListSourceDataController DataController {
			get {
				if (dataController == null) {
					dataController = CreateDataController();
					dataController.VisualClient = this;
					dataController.DataClient = new DataControllerData(dataController, Owner);
					dataController.ListChanged += DataController_ListChanged;
					dataController.ListSourceChanged += DataController_ListSourceChanged;
					dataController.Refreshed += DataController_Refreshed;
					InitializeDataController();
				}
				return dataController;
			}
		}
		public void Reset() {
			ResetCaches();
			InitializeDataController();
#if DEBUGTEST
			LogBase.Add(Owner, null, string.Empty);
#endif
		}
		protected void ResetVisibleListSource() {
			visibleListSource = null;
#if DEBUGTEST
			LogBase.Add(Owner, null, "Refreshed");
#endif
		}
		public void ResetCaches() {
			ItemsCache.Reset();
			ResetDisplayTextCache();
#if DEBUGTEST
			LogBase.Add(Owner, null, string.Empty);
#endif
		}
		public void ResetDisplayTextCache() {
			editTextToDisplayTextCache.Clear();
		}
		public object this[int index] {
			get { return GetItemByControllerIndex(index); }
		}
		protected virtual IEnumerable CreateVisibleListSource() {
			return Owner.AllowCollectionView ? CreateHierarchicalSource() : CreatePlainListSource();
		}
		protected virtual IEnumerable CreateHierarchicalSource() {
			return CollectionViewSupport.HasCollectionView ? CollectionViewSupport.CollectionView : CreatePlainListSource();
		}
		protected virtual IEnumerable CreatePlainListSource() {
			Type sourceGenericType = GetSourceGenericType();
			if (sourceGenericType == null)
				sourceGenericType = typeof(object);
			return CreateCollectionWrapper(sourceGenericType);
		}
		protected virtual IEnumerable CreateCollectionWrapper(Type genericType) {
			Type wrapperType = DataController.Helper.TypedList != null ? typeof(TypedListObservableCollection<>) : typeof(ObservableCollection<>);
			Type type = wrapperType.MakeGenericType(new[] { genericType });
			IList list = (IList)Activator.CreateInstance(type);
			foreach (object item in this.Cast<object>())
				list.Add(item);
			return list;
		}
		Type GetSourceGenericType() {
			Type sourceType = null;
			BindingListAdapter adapter = ListSource as BindingListAdapter;
			if (adapter != null && adapter.OriginalDataSource != null)
				sourceType = adapter.OriginalDataSource.GetType();
			else if (ListSource != null)
				sourceType = ListSource.GetType();
			return sourceType != null ? FindGenericType(sourceType) : null;
		}
		public Type FindGenericType(Type sourceType) {
			IEnumerable<Type> typeHierarchy = GetTypeHierarchy(sourceType);
			foreach (Type type in typeHierarchy) {
				Type[] interfaces = type.GetInterfaces();
				Type genericCollectionInterfaceType = GetCollectionLikeGenericTypeFromInterfaces(interfaces);
				if (genericCollectionInterfaceType != null)
					return genericCollectionInterfaceType;
			}
			if (!sourceType.IsGenericType)
				return null;
			Type[] genericTypes = sourceType.GetGenericArguments();
			return genericTypes.Length == 1 ? genericTypes[0] : null;
		}
		Type GetCollectionLikeGenericTypeFromInterfaces(IEnumerable<Type> interfaces) {
			foreach (Type interf in interfaces) {
				if (!interf.IsGenericType)
					continue;
				Type[] arguments = interf.GetGenericArguments();
				if (arguments.Length > 1)
					continue;
				if (typeof(IEnumerable<>).MakeGenericType(arguments) == interf)
					return arguments[0];
			}
			return null;
		}
		IEnumerable<Type> GetTypeHierarchy(Type type) {
			IList<Type> hierarchy = new List<Type>();
			Type currentType = type;
			while (currentType.BaseType != null) {
				hierarchy.Add(currentType);
				currentType = currentType.BaseType;
			}
			return hierarchy.Reverse();
		}
		public object GetValueByIndex(int index, object handle = null) {
			object result = DataController.GetRowByListSourceIndex(index);
			return GetValueFromItem(result);
		}
		public object GetItemByControllerIndex(int controllerIndex, object handle = null) {
			if (controllerIndex > -1)
				return DataController.GetRow(controllerIndex);
			return null;
		}
		public object GetDisplayValueByIndex(int index, object handle) {
			int controllerIndex = GetControllerIndexByIndex(index);
			return GetDisplayValueFromItem(GetItemByControllerIndex(controllerIndex));
		}
		public int GetIndexByItem(object item, object handle = null) {
			return ItemsCache.IndexByItem(item);
		}
		public void DoSort(ColumnSortOrder sortOrder) {
			if (!DataController.IsReady)
				return;
			DataColumnInfo column = GetDisplayColumnInfo();
			if (column != null)
				DataController.SortInfo.ClearAndAddRange(new DataColumnSortInfo(column, sortOrder));
		}
		DataColumnInfo GetDisplayColumnInfo() {
			return DataController.Columns[DataControllerData.DisplayColumnName];
		}
		public DataColumnInfo GetValueColumnInfo() {
			return DataController.Columns[DataControllerData.ValueColumnName];
		}
		public void ClearSort() {
				DataController.SortInfo.Clear();
		}
		public void UpdateDisplayMember() {
			ResetCaches();
			DataController.RePopulateColumns();
		}
		public void UpdateValueMember() {
			ResetCaches();
			DataController.RePopulateColumns();
		}
		public int GetIndexByControllerIndex(int controllerIndex, object handle = null) {
			object item = GetItemByControllerIndex(controllerIndex);
			return ItemsCache.IndexByItem(item);
		}
		public int GetControllerIndexByIndex(int index, object handle = null) {
			return DataController.GetControllerRow(index);
		}
		protected virtual object GetListSourceItemByValue(object editValue) {
			int index = ItemsCache.IndexOfValue(editValue);
			return DataController.GetRowByListSourceIndex(index);
		}
		public virtual object GetDisplayValueByEditValue(object editValue, object handle = null) {
			object item = GetListSourceItemByValue(editValue);
			return IsInLookUpMode ? GetDisplayValueFromItem(item) : GetDisplayValueFromItem(item ?? editValue);
		}
		public object GetItem(object value, object handle = null) {
			int index = IndexOfValue(value);
			int controllerRow = DataController.GetControllerRow(index);
			return index > -1 ? DataController.GetListSourceRow(controllerRow) : null;
		}
		public int IndexOfValue(object value, object handle = null) {
			return ItemsCache.IndexOfValue(value);
		}
		public object GetValueFromItem(object item, object handle = null) {
			object result = GetEditValueCore(item);
			if (LookUpPropertyDescriptor.IsUnsetValue(result))
				result = null;
			return result;
		}
		public object GetDisplayValueFromItem(object item) {
			object result = GetDisplayValueCore(item);
			if (LookUpPropertyDescriptor.IsUnsetValue(result))
				result = null;
			return result;
		}
		CriteriaOperator CalcActualFilterOperator() {
			List<CriteriaOperator> operators = new List<CriteriaOperator>();
			if (!object.Equals(DisplayFilterCriteria, null))
				operators.Add(DisplayFilterCriteria);
			if (!object.Equals(Owner.FilterCriteria, null))
				operators.Add(Owner.FilterCriteria);
			if (operators.Count == 0)
				return null;
			return operators.Count > 1 ? CriteriaOperator.And(operators) : operators[0];
		}
		protected virtual object GetDisplayValueCore(object item) {
			return DataControllerData.DisplayColumnDescriptor.GetValue(item);
		}
		protected virtual object GetEditValueCore(object item) {
			return DataControllerData.ValueColumnDescriptor.GetValue(item);
		}
		protected virtual string GetDisplayTextCore(object displayValue) {
			return Convert.ToString(DataController.GetValueEx(IndexOfValue(displayValue), GetDisplayColumnInfo().Name));
		}
		protected virtual void ReleaseDataController() {
			dataController.ListSourceChanged -= DataController_ListSourceChanged;
			dataController.ListChanged -= DataController_ListChanged;
			dataController.Refreshed -= DataController_Refreshed;
			dataController.VisualClient = null;
			dataController.DataClient = null;
			DataController.Dispose();
			dataController = null;
		}
		public void UpdateFilterCriteria() {
			ActualFilterCriteria = CalcActualFilterOperator();
			DataController.FilterCriteria = ActualFilterCriteria;
		}
		protected void InitializeDataController() {
			InitializeDataControllerLocker.DoLockedActionIfNotLocked(InitializeDataControllerInternal);
		}
		protected virtual void InitializeDataControllerInternal() {
			ListSource = ExtractDataSource();
			VerifyDataController();
			try {
				DataController.BeginUpdate();
				CollectionViewSupport.Release();
				DataController.SetDataSource(ListSource);
				UpdateDisplayFilter();
			}
			finally {
				CollectionViewSupport.SyncWithData(this);
				CollectionViewSupport.Initialize();
				CollectionViewSupport.SyncWithCurrent();
				DataController.EndUpdate();
			}
		}
		void DataController_ListSourceChanged(object sender, EventArgs e) {
		}
		void DataController_Refreshed(object sender, EventArgs e) {
			LogBase.Add(Owner, null, "DataController_Refreshed");
			ClearDisplayCacheAndRaiseListChanged();
		}
		void DataController_ListChanged(object sender, ListChangedEventArgs e) {
			LogBase.Add(Owner, e.ListChangedType, "DataController_ListChanged");
			int listSourceIndex = e.NewIndex;
			ListChangedType listChanged = e.ListChangedType;
			if (listChanged == ListChangedType.ItemChanged) {
				if (!object.Equals(ActualFilterCriteria, null) && e.PropertyDescriptor != null) {
					listChanged = ListChangedType.Reset;
					ClearDisplayCache();
				}
				else {
					ItemsCache.UpdateItem(listSourceIndex);
					if (ShouldUpdateVisibleListSourceOnUpdateEvents(e.PropertyDescriptor))
						UpdateItem(listSourceIndex, e.PropertyDescriptor);
				}
			}
			else if (listChanged == ListChangedType.ItemAdded) {
				ItemsCache.UpdateItemOnAdding(listSourceIndex);
				if (ShouldUpdateVisibleListSourceOnUpdateEvents()) {
					AddItem(listSourceIndex);
				}
			}
			else if (listChanged == ListChangedType.ItemDeleted) {
				ItemsCache.UpdateItemOnDeleting(listSourceIndex);
				if (ShouldUpdateVisibleListSourceOnUpdateEvents()) {
					RemoveItem(listSourceIndex);
				}
			}
			else
				ClearDisplayCache();
			ResetDisplayTextCache();
			RaiseDataChanged(listChanged, listSourceIndex, e.PropertyDescriptor);
		}
		public void UpdateItemsSource() {
			Reset();
		}
		void VerifyDataController() {
			DataControllerData.ResetDescriptors();
			if (DataController.IsServerMode != IsServerMode)
				ReleaseDataController();
		}
		class ItemsProviderServerModeDataController : ServerModeDataController {
			protected override BaseDataControllerHelper CreateHelper() {
				return new ListDataControllerHelper(this);
			}
		}
		class ItemsProviderDataController : DXGridDataController {
			protected override BaseDataControllerHelper CreateHelper() {
				return new ListDataControllerHelper(this);
			}
#if !SL
			protected override System.Windows.Threading.Dispatcher Dispatcher { get { return ((ItemsProvider)VisualClient).Owner.Dispatcher; } }
#endif
		}
		protected virtual BaseListSourceDataController CreateDataController() {
			return IsServerMode
				? (BaseListSourceDataController)new ItemsProviderServerModeDataController()
				: (BaseListSourceDataController)new ItemsProviderDataController();
		}
		public virtual void ProcessSelectionChanged(NotifyItemsProviderSelectionChangedEventArgs e) {
			RaiseSelectionChanged(GetValueFromItem(e.Item), e.IsSelected);
		}
		public virtual void ProcessCollectionChanged(NotifyItemsProviderChangedEventArgs e) {
			if (e.ChangedType == ListChangedType.ItemChanged) {
				ProcessItemChanged(e);
				RaiseDataChanged(ListChangedType.ItemChanged);
			}
			else
				Reset();
		}
		protected virtual void ProcessItemChanged(NotifyItemsProviderChangedEventArgs e) {
			ResetDisplayTextCache();
		}
		protected virtual IList ExtractDataSource() {
			object itemsSource = Owner.ItemsSource ?? Owner.Items;
			if (!Owner.AllowCollectionView) {
				IList source = ExtractSimpleDataSource(itemsSource);
				return source;
			}
			else {
				return ExtractCollectionViewDataSource(itemsSource);
			}
		}
		IList ExtractCollectionViewDataSource(object itemsSource) {
			ICollectionView view = itemsSource as ICollectionView;
			if (view == null) {
				CollectionViewSource source = new CollectionViewSource { Source = itemsSource };
				view = source.View;
			}
			return DataBindingHelper.ExtractDataSource(view);
		}
		IList ExtractSimpleDataSource(object itemsSource) {
			ICollectionView view = itemsSource as ICollectionView;
			if (view != null)
				return DataBindingHelper.ExtractDataSourceFromCollectionView(view);
			return DataBindingHelper.ExtractDataSource(itemsSource);
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
		#region IDataControllerVisualClient Members
		void IDataControllerVisualClient.RequireSynchronization(IDataSync dataSync) {
			dataSync.AllowSyncSortingAndGrouping = false;
			ClearDisplayCacheAndRaiseListChanged();
		}
		ColumnSortOrder ListSortDirectionToColumnSortOrder(ListSortDirection direction) {
			return direction == ListSortDirection.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
		}
		void IDataControllerVisualClient.ColumnsRenewed() {
		}
		bool IDataControllerVisualClient.IsInitializing {
			get { return false; }
		}
		int IDataControllerVisualClient.PageRowCount {
			get { return DataController.VisibleCount; }
		}
		int IDataControllerVisualClient.VisibleRowCount {
			get { return -1; }
		}
		int IDataControllerVisualClient.TopRowIndex {
			get { return 0; }
		}
		void IDataControllerVisualClient.RequestSynchronization() {
		}
		void IDataControllerVisualClient.UpdateColumns() {
		}
		void IDataControllerVisualClient.UpdateScrollBar() {
		}
		void IDataControllerVisualClient.UpdateTotalSummary() {
		}
		void IDataControllerVisualClient.UpdateLayout() {
		}
		void IDataControllerVisualClient.UpdateRow(int controllerRowHandle) {
		}
		bool ShouldUpdateVisibleListSourceOnUpdateEvents(PropertyDescriptor changedProperty = null) {
			if (visibleListSource == null)
				return false;
			if (Owner.AllowCollectionView)
				return false;
			if (changedProperty != null)
				return false;
			return VisibleListSource as IList != null;
		}
		void UpdateItem(int listSourceIndex, PropertyDescriptor changedProperty = null) {
			IList list = (IList)VisibleListSource;
			int controllerIndex = DataController.GetControllerRow(listSourceIndex);
			if (controllerIndex == Data.DataController.InvalidRow)
				return;
			object newValue = DataController.GetRowByListSourceIndex(listSourceIndex);
			if (changedProperty == null && !object.ReferenceEquals(newValue, list[controllerIndex])) {
				list[controllerIndex] = newValue;
				LogBase.Add(Owner, listSourceIndex, "ItemsProvider_UpdateItem");
			}
		}
		void AddItem(int listSourceIndex) {
			IList list = (IList)VisibleListSource;
			int controllerIndex = DataController.GetControllerRow(listSourceIndex);
			if (controllerIndex == Data.DataController.InvalidRow)
				return;
			if (controllerIndex == list.Count)
				list.Add(DataController.GetRowByListSourceIndex(listSourceIndex));
			else
				list.Insert(controllerIndex, DataController.GetRowByListSourceIndex(listSourceIndex));
		}
		void RemoveItem(int listSourceIndex) {
			IList list = (IList)VisibleListSource;
			int controllerIndex = DataController.GetControllerRow(listSourceIndex);
			if (controllerIndex == Data.DataController.InvalidRow)
				return;
			list.RemoveAt(controllerIndex);
		}
		void IDataControllerVisualClient.UpdateRowIndexes(int newTopRowIndex) {
		}
		void IDataControllerVisualClient.UpdateRows(int topRowIndexDelta) {
		}
		void ClearDisplayCacheAndRaiseListChanged() {
			ClearDisplayCache();
			RaiseDataChanged();
		}
		void ClearDisplayCache() {
			ResetCaches();
			ResetVisibleListSource();
		}
		#endregion
		#region IEnumerable Members
		public IEnumerator<object> GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return DataController.GetRow(i);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		public static bool AreEqual(IList list1, IList list2) {
			if (list1 == null && list2 == null)
				return true;
			if (list1 == null)
				return list2.Count == 0;
			if (list2 == null)
				return list1.Count == 0;
			if (list1.Count != list2.Count)
				return false;
			return list1.Cast<object>().All(list2.Contains);
		}
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(ListChangedEventManager)) {
				ClearDisplayCacheAndRaiseListChanged();
				return true;
			}
			return false;
		}
		#endregion
		public void DoRefresh() {
			DataController.DoRefresh();
#if DEBUGTEST
			LogBase.Add(Owner, null);
#endif
		}
		public virtual int FindItemIndexByText(string text, bool isCaseSensitive, bool autoComplete, object handle, int startItemIndex = -1) {
			if (text == null)
				return -1;
			int index;
			string findText = isCaseSensitive ? text : text.ToLower();
			var cacheItem = CreateDisplayTextCacheItem(findText, isCaseSensitive, autoComplete);
			if (!editTextToDisplayTextCache.TryGetValue(cacheItem, out index)) {
				index = FindItemIndexByTextInternal(findText, isCaseSensitive, autoComplete);
				if (!IsAsyncServerMode || index != Data.DataController.OperationInProgress)
					editTextToDisplayTextCache[cacheItem] = index;
			}
			return index;
		}
		DisplayTextCacheItem CreateDisplayTextCacheItem(string text, bool isCaseSensitive, bool autoComplete) {
			return new DisplayTextCacheItem() { DisplayText = text, AutoComplete = autoComplete };
		}
		protected virtual int FindItemIndexByTextInternal(string text, bool isCaseSensitive, bool autoComplete) {
			LogBase.Add(Owner, null, "FindItemIndexByText");
			var compareOperator = GetCompareCriteriaOperator(
				autoComplete && text != string.Empty,
				new OperandProperty(DataControllerData.DisplayColumnName),
				new OperandValue(text));
			ExpressionEvaluator evaluator = new ExpressionEvaluator(
				new PropertyDescriptorCollection(new[] { GetDisplayStringPropertyDescriptor() }),
				compareOperator,
				isCaseSensitive);
			for (int i = 0; i < Count; i++) {
				if ((bool)evaluator.Evaluate(this[i])) {
					return DataController.GetListSourceRowIndex(i);
				}
			}
			return -1;
		}
		PropertyDescriptor GetDisplayStringPropertyDescriptor() {
			return new GetStringFromLookUpValuePropertyDescriptor(DataControllerData.DisplayColumnDescriptor);
		}
		CriteriaOperator GetCompareCriteriaOperator(bool autoComplete, OperandProperty property, OperandValue value) {
			if (autoComplete)
				return new FunctionOperator(FunctionOperatorType.StartsWith, property, value);
			return new BinaryOperator(property, value, BinaryOperatorType.Equal);
		}
		IEnumerable<string> GetAvailableColumns() {
			return from IDataColumnInfo dataColumnInfo in DataController.Columns select dataColumnInfo.FieldName;
		}
		#region IItemsProviderICollectionViewSupport Members
		ICollectionViewHelper IItemsProviderCollectionViewSupport.DataSync { get { return ListSource as ICollectionViewHelper; } }
		ICollectionView IItemsProviderCollectionViewSupport.ListSource {
			get {
				var helper = ListSource as ICollectionViewHelper;
				return helper != null ? helper.Collection : null;
			}
		}
		void IItemsProviderCollectionViewSupport.RaiseCurrentChanged(object currentItem) {
			RaiseCurrentChanged(currentItem);
		}
		void IItemsProviderCollectionViewSupport.SyncWithCurrentItem() {
			CollectionViewSupport.SyncWithCurrent();
		}
		bool IItemsProviderCollectionViewSupport.IsSynchronizedWithCurrentItem { get { return Owner.IsSynchronizedWithCurrentItem; } }
		void IItemsProviderCollectionViewSupport.SetCurrentItem(object currentItem) {
			CollectionViewSupport.SetCurrentItem(currentItem);
		}
		#endregion
		#region IDataControllerAdapter Members
		int IDataControllerAdapter.VisibleRowCount { get { return DataController.VisibleListSourceRowCount; } }
		bool IDataControllerAdapter.IsOwnSearchProcessing { get { return IsAsyncServerMode; } }
		object IDataControllerAdapter.GetRowValue(object item) {
			return GetEditValueCore(item);
		}
		object IDataControllerAdapter.GetRowValue(int index) {
			object item = DataController.GetRowByListSourceIndex(index);
			return GetEditValueCore(item);
		}
		object GetRowValueInternal(DataColumnInfo column, int listSourceIndex) {
			return DataController.GetListSourceRowValue(listSourceIndex, column.Name);
		}
		object IDataControllerAdapter.GetRow(int listSourceIndex) {
			return DataController.GetRowByListSourceIndex(listSourceIndex);
		}
		int IDataControllerAdapter.GetListSourceIndex(object value) {
			throw  new NotImplementedException();
		}
		public CriteriaOperator CreateDisplayFilterCriteria(string searchText, FilterCondition filterCondition) {
			if (string.IsNullOrEmpty(searchText))
				return null;
			FunctionOperatorType functionOperatorType = GetFunctionOperatorType(filterCondition);
			return new FunctionOperator(functionOperatorType, new OperandProperty(GetDisplayColumnInfo().Name), searchText);
		}
		protected virtual FunctionOperatorType GetFunctionOperatorType(FilterCondition filterCondition) {
			return filterCondition == FilterCondition.Contains ? FunctionOperatorType.Contains : FunctionOperatorType.StartsWith;
		}
		#endregion
		#region ServerMode
		public bool IsAsyncServerMode { get { return false; } }
		public ServerModeDataControllerBase GetServerModeDataController() {
			return null;
		}
		#endregion
		#region IItemsProvider
		void IItemsProvider.RegisterSnapshot(object handle) {}
		void IItemsProvider.ReleaseSnapshot(object handle) { }
		void IItemsProvider.SetDisplayFilterCriteria(CriteriaOperator criteria, object handle) { }
		IEnumerable IItemsProvider.GetVisibleListSource(object hadle) { return null; }
		string IItemsProvider.GetDisplayPropertyName(object handle) { return string.Empty; }
		#endregion
	}
	public class DisplayTextCacheItem {
		protected bool Equals(DisplayTextCacheItem other) {
			return StartIndex == other.StartIndex && string.Equals(DisplayText, other.DisplayText) && AutoComplete == other.AutoComplete && SearchNext == other.SearchNext && IgnoreStartIndex == other.IgnoreStartIndex;
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((DisplayTextCacheItem)obj);
		}
		public override int GetHashCode() {
			unchecked {
				int hashCode = StartIndex;
				hashCode = (hashCode * 397) ^ (DisplayText != null ? DisplayText.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ AutoComplete.GetHashCode();
				hashCode = (hashCode * 397) ^ SearchNext.GetHashCode();
				hashCode = (hashCode * 397) ^ IgnoreStartIndex.GetHashCode();
				return hashCode;
			}
		}
		public int StartIndex { get; set; }
		public string DisplayText { get; set; }
		public bool AutoComplete { get; set; }
		public bool SearchNext { get; set; }
		public bool IgnoreStartIndex { get; set; }
	}
	public class TypedListObservableCollection<T> : ObservableCollection<T>, ITypedList {
		public TypedListObservableCollection() : base() { }
		public TypedListObservableCollection(IEnumerable<T> collection) : base(collection) { }
		public TypedListObservableCollection(List<T> list) : base(list) { }
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			ICustomTypeDescriptor obj = GetCustomTypedItem();
			if (obj != null)
				return obj.GetProperties();
			return TypeDescriptor.GetProperties(typeof(T));
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return null;
		}
		ICustomTypeDescriptor GetCustomTypedItem() {
			foreach (object item in this) {
				ICustomTypeDescriptor customTypedItem = item as ICustomTypeDescriptor;
				if (customTypedItem != null)
					return customTypedItem;
			}
			return null;
		}
		#endregion
	}
	public delegate void ItemsProviderChangedEventHandler(object sender, ItemsProviderChangedEventArgs e);
}
