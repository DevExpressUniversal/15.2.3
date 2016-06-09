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
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.Data.Filtering.Helpers;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using System.Collections.Specialized;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public interface IFilterClient {
		object GetObject(RowHandle handle);
		object GetDefaultObject();
	}
	public interface ISortClient {
		IEnumerable<RowHandle> GetSortedChildren(RowHandle parent, IEnumerable<RowHandle> children);
		IEnumerable<RowHandle> GetMappings(RowHandle parent, Func<string, string, RowHandle> addMappingSourceToTarget);
	}
	public class DataViewBase {
		readonly IFilterClient DefaultFilterClient;
		readonly ISortClient DefaultSortClient;
		DataController dataController;
		CriteriaOperator filterCriteria;
		CriteriaOperator searchCriteria;
		Dictionary<RowHandle, IEnumerable<RowHandle>> childrenCache;
		IFilterClient filterClient;
		ISortClient sortClient;
		Lazy<IList<RowHandle>> visibleHandles;
		Dictionary<object, Mapping> mappings;
		DataControllerSourceChangedEventHandler sourceChangedEventHandler;
		DataControllerSourceChangedEventHandler sourceChangingEventHandler;
		protected DataViewBase(DataController dataController) {
			DefaultFilterClient =  new PropertyNameFilterClient(this);
			DefaultSortClient = new SortClient();
			this.dataController = dataController;
			sourceChangedEventHandler = new DataControllerSourceChangedEventHandler(this, (d, o, e) => d.SourceChanged(o, e));
			sourceChangingEventHandler = new DataControllerSourceChangedEventHandler(this, (d, o, e) => d.SourceChanging(o, e));
			dataController.SourceChanged += sourceChangedEventHandler.OnEvent;
			dataController.SourceChanging += sourceChangingEventHandler.OnEvent;
			ResetVisibleHandles();
		}
		protected Dictionary<object, Mapping> Mappings {
			get {
				if (mappings == null) {
					mappings = new Dictionary<object, Mapping>();
				}
				return mappings;
			}
		}
		List<RowHandle> oldHandles = null;
		int startIndex = -1;
		void SourceChanging(object o, EventArgs e) {
			var ee = e as RowHandleChangedEventArgs;
			if (ee != null) {
				if (ee.ChangeType == RowHandleChangeType.Reset)
					return;
				this.startIndex = VisibleHandles.ToList().IndexOf(ee.Handle);
				if (this.startIndex == -1)
					return;
				this.oldHandles = GetVisibleHandles(ee.Handle).ToList();
			}
		}
		void SourceChanged(object o, EventArgs e) {
			var ee = e as RowHandleChangedEventArgs;
			if (ee != null ) {
				ResetVisibleHandles();
			}
			InvalidateFilter();
			if (ee != null) {
				if (ee.ChangeType == RowHandleChangeType.Reset) {
					VisibleHandlesChanged.Do(h => h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
				}
				if (ee.ChangeType == RowHandleChangeType.Collapse) {
					if (this.startIndex == -1)
						return;
					VisibleHandlesChanged.Do(h => h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this.oldHandles.ToList(), this.startIndex + 1)));
				}
				if (ee.ChangeType == RowHandleChangeType.Expand) {
					if (this.startIndex == -1)
						return;
					var newHandles = GetVisibleHandles(ee.Handle);
					VisibleHandlesChanged.Do(h => h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newHandles.ToList(), this.startIndex + 1)));
				}
				if (ee.ChangeType == RowHandleChangeType.Replace) {
					if (this.startIndex == -1)
						return;
					var newHandles = new List<RowHandle> { ee.Handle };
					newHandles.AddRange(GetVisibleHandles(ee.Handle));
					this.oldHandles.Insert(0, ee.Handle);
					VisibleHandlesChanged.Do(h => h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newHandles, this.oldHandles, this.startIndex)));
				}
				this.oldHandles = null;
				this.startIndex = -1;
			}
		}
		void ResetVisibleHandles() {
			this.visibleHandles = new Lazy<IList<RowHandle>>(() => GetVisibleHandles(RowHandle.Root));
			this.mappings = null;
		}
		System.Collections.ObjectModel.ReadOnlyCollection<RowHandle> emptyHandles = new System.Collections.ObjectModel.ReadOnlyCollection<RowHandle>(new List<RowHandle>());
		IList<RowHandle> GetVisibleHandles(RowHandle handle) {
			if (!IsExpanded(handle))
				return emptyHandles;
			var resultHandles = new List<RowHandle>();
			IEnumerable<RowHandle> rootHandles = GetChildren(handle);
			foreach (RowHandle childHandle in rootHandles) {
				resultHandles.Add(childHandle);
				resultHandles.AddRange(GetVisibleHandles(childHandle));
			}
			return resultHandles;
		}
		public event NotifyCollectionChangedEventHandler VisibleHandlesChanged;
		public IList<RowHandle> VisibleHandles {
			get { return visibleHandles.Value; }
		}
		public bool IsKeySensitive { get; set; }
		public CriteriaOperator SearchCriteria {
			get { return searchCriteria; }
			set {
				if (CriteriaOperator.Equals(searchCriteria, value))
					return;
				searchCriteria = value;
				FilterCriteriaChanged();
			}
		}
		public CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				if (CriteriaOperator.Equals(filterCriteria, value))
					return;
				filterCriteria = value;
				FilterCriteriaChanged();
			}
		}
#if DEBUGTEST
		string filterString = null;
		internal string FilterString {
			get { return filterString; }
			set {
				if (filterString == value)
					return;
				filterString = value;
				if (string.IsNullOrEmpty(value))
					SearchCriteria = null;
				else
					SearchCriteria = new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty("Name"), new OperandValue(FilterString));
			}
		}
#endif
		public IFilterClient FilterClient {
			get {
				if (filterClient == null) {
					filterClient = DefaultFilterClient;
				}
				return filterClient;
			}
			set {
				if (FilterClient == value)
					return;
				filterClient = value;
			}
		}
		public ISortClient SortClient {
			get {
				if (sortClient == null) {
					sortClient = DefaultSortClient;
				}
				return sortClient;
			}
			set {
				if (SortClient == value)
					return;
				sortClient = value;
			}
		}
		protected DataController DataController { get { return dataController; } }
		protected ExpressionEvaluator ExpressionEvaluator {
			get {
				return new ExpressionEvaluator(
						GetFilterProperties(),
						GetFilterCriteria(),
						IsKeySensitive);
			}
		}
		bool InFilterMode { get { return !CriteriaOperator.Equals(FilterCriteria, null); } }
		bool InSearchMode { get { return !CriteriaOperator.Equals(SearchCriteria, null); } }
		bool CanFilter { get { return InFilterMode || InSearchMode; } }
		Dictionary<RowHandle, IEnumerable<RowHandle>> ChildrenCache {
			get {
				if (childrenCache == null) {
					childrenCache = CreateFilterCache();
				}
				return childrenCache;
			}
		}
		public void Invalidate(RowHandle handle) {
			DataController.Invalidate(handle);
		}
		public void Update() {
			DataController.Update();
		}
		public bool IsExpanded(RowHandle handle) {
			bool isLoaded = DataController.IsLoaded(handle);
			if (isLoaded) {
				if (InSearchMode) {
					return SearchIsExpanded(handle);
				}
			}
			return DataController.IsExpanded(handle);
		}
		Dictionary<RowHandle, bool> searchExpandedState;
		void ResetSearchExpandedState() {
			searchExpandedState = new Dictionary<RowHandle, bool>();
		}
		bool SearchIsExpanded(RowHandle handle) {
			bool isExpanded;
			if (!searchExpandedState.TryGetValue(handle, out isExpanded)) {
				isExpanded = true;
				searchExpandedState[handle] = isExpanded;
			}
			return isExpanded;
		}
		void SearchSetIsExpanded(RowHandle handle, bool value) {
			bool? isExpanded = DataController.IsExpanded(handle);
			searchExpandedState[handle] = value;
			if (!isExpanded.HasValue) {
				DataController.SetIsExpanded(handle, false);
			}
		}
		public bool IsReadOnly(RowHandle rowHandle) {
			return DataController.IsReadOnly(rowHandle);
		}
		public bool ShouldRenderReadOnly(RowHandle rowHandle) {
			return DataController.ShouldRenderReadOnly(rowHandle);
		}
		public IEnumerable<string> GetValidationError(RowHandle handle) {
			return DataController.GetValidationError(handle);
		}
		public Type GetPropertyType(RowHandle handle) {
			return DataController.GetPropertyType(handle);
		}
		public object GetSelectedCollectionNewItem(RowHandle handle) {
			return DataController.GetCollectionHelper(handle).Return(x => x.SelectedItem, () => null);
		}
		public bool CanAddCollectionNewItem(RowHandle handle) {
			return DataController.GetCollectionHelper(handle).Return(x => x.CanAddNewItem, () => false);
		}
		public IEnumerable GetCollectionNewItemValues(RowHandle handle) {
			return DataController.GetCollectionHelper(handle).Return(x => x.NewItemValues, () => null);
		}
		public void AddCollectionNewItem(RowHandle handle, object item) {
			DataController.GetCollectionHelper(handle).Do(x => x.AddNewItem(item));
		}
		public Exception SetValue(RowHandle handle, object value) {
			return DataController.SetValue(handle, value);
		}
		public bool CanResetValue(RowHandle handle) {
			return DataController.CanResetValue(handle);
		}
		public bool CanRemoveCollectionItem(RowHandle handle) {
			DescriptorContext context = DataController.GetDescriptorContext(handle).With(x => x.ParentContext);
			if (context == null)
				return false;
			return DataController.GetCollectionHelper(context.RowHandle).Return(x => x.CanRemoveCollectionItem, () => false);
		}
		public void RemoveCollectionItem(RowHandle handle) {
			DescriptorContext context = DataController.GetDescriptorContext(handle).With(x => x.ParentContext);
			if (context == null)
				return;
			DataController.GetCollectionHelper(context.RowHandle).Do(x => x.RemoveCollectionItem(handle));
		}
		public Exception ResetValue(RowHandle handle) {
			return DataController.ResetValue(handle);
		}
		public bool ShouldSerializeValue(RowHandle handle) {
			return DataController.ShouldSerializeValue(handle);
		}
		public void SetIsExpanded(RowHandle handle, bool isExpanded) {
			if (InSearchMode)
				SearchSetIsExpanded(handle, isExpanded);
			else
				DataController.SetIsExpanded(handle, isExpanded);
		}
		public bool CanUseCollectionEditor(RowHandle handle) {
			return DataController.CanUseCollectionEditor(handle);
		}
		internal bool IsCollectionHandle(RowHandle handle) {
			return DataController.IsCollectionHandle(handle);
		}
		public bool IsGroupRowHandle(RowHandle handle) {
			return DataController.IsGroupRowHandle(handle);
		}
		public bool IsNewInstanceInitializer(RowHandle handle) {
			return DataController.IsNewInstanceInitializer(handle);
		}
		public IEnumerable GetStandardValues(RowHandle handle) {
			return DataController.GetStandardValues(handle);
		}
		public bool GetIsStandardValuesSupported(RowHandle handle) {
			return DataController.GetIsStandardValuesSupported(handle);
		}
		public bool GetIsStandardValuesExclusive(RowHandle handle) {
			return DataController.GetIsStandardValuesExclusive(handle);
		}
		public bool CanConvertToString(RowHandle handle) {
			return DataController.CanConvertToString(handle);
		}
		public bool CanConvertFromString(RowHandle handle) {
			return DataController.CanConvertFromString(handle);
		}
		public string ConvertToString(RowHandle handle, object value) {
			string result = null;
			try {
				result = DataController.ConvertToString(handle, value);
			} catch {
				result = Convert.ToString(value);
			}
			return result;
		}
		public bool CanExpand(RowHandle handle) {
			bool canExpand = DataController.CanExpand(handle);
			return canExpand && (DataController.IsLoaded(handle) ? GetChildren(handle).Any() : true);
		}
		public object GetValue(RowHandle handle) {
			return dataController.GetValue(handle);
		}
		public string GetNameByHandle(RowHandle handle) {
			if (handle == RowHandle.Invalid)
				return null;
			return dataController.GetNameByHandle(handle);
		}
		public string GetFieldNameByHandle(RowHandle handle) {
			if (handle == RowHandle.Invalid)
				return null;
			Mapping mapping;
			if (Mappings.TryGetValue(handle, out mapping)) {
				return mapping.TargetFieldName;
			}
			return dataController.GetFieldNameByHandle(handle);
		}
		public RowHandle GetHandleByFieldName(string fieldName) {
			if (fieldName == null)
				return RowHandle.Invalid;
			Mapping mapping;
			if (Mappings.TryGetValue(fieldName, out mapping)) {
				return mapping.Handle;
			}
			return dataController.GetHandleByFieldName(fieldName).Return(x => x, () => RowHandle.Invalid);
		}
		public PropertyDescriptor GetDescriptor(RowHandle handle) {
			return dataController.GetDescriptorContext(handle).With(x => x.PropertyDescriptor);
		}
		public IEnumerable<RowHandle> GetChildren(RowHandle handle) {
			IEnumerable<RowHandle> handles;
			if (!ChildrenCache.TryGetValue(handle, out handles)) {
				var unsortedHandles = CanFilter ? GetFilteredChildren(handle) : GetChildrenInternal(handle);
				handles = SortClient.GetSortedChildren(handle, unsortedHandles);
				ChildrenCache[handle] = handles;
			}
			return handles;
		}
		public string GetDescription(RowHandle handle) {
			return DataController.GetDescription(handle);
		}
		public string GetDisplayName(RowHandle handle) {
			return DataController.GetDisplayName(handle);
		}
		public virtual RowHandle GetParent(RowHandle handle) {
			if (handle == null || handle.IsInvalid)
				return null;
			Mapping mapping;
			if (Mappings.TryGetValue(handle, out mapping)) {
				return mapping.ParentHandle;
			}
			return DataController.GetParent(handle).Return(x => x.RowHandle, () => null);
		}
		public bool IsAttachedProperty(RowHandle handle) {
			return DataController.IsAttachedProperty(handle);
		}
		Dictionary<RowHandle, IEnumerable<RowHandle>> CreateFilterCache() {
			return new Dictionary<RowHandle, IEnumerable<RowHandle>>();
		}
		PropertyDescriptorCollection GetFilterProperties() {
			object filterObject = FilterClient.GetDefaultObject();
			return TypeDescriptor.GetProperties(filterObject, null, false);
		}
		CriteriaOperator GetFilterCriteria() {
			if (InSearchMode && InFilterMode)
				return CriteriaOperator.And(SearchCriteria, FilterCriteria);
			if (InSearchMode)
				return SearchCriteria;
			return FilterCriteria;
		}
		void FilterCriteriaChanged() {
			InvalidateFilter();
			ResetVisibleHandles();
			VisibleHandlesChanged.Do(h => h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
		}
		void InvalidateFilter() {
			childrenCache = null;
			ResetSearchExpandedState();
		}
		protected virtual IEnumerable<RowHandle> GetFilteredChildren(RowHandle handle) {
			IEnumerable<RowHandle> children = GetChildrenInternal(handle) ?? EmptyEnumerable<RowHandle>.Instance;
			foreach (RowHandle child in children) {
				if (IsVisible(child)) {
					yield return child;
					continue;
				}
				if (HasVisibleChildren(child)) {
					yield return child;
					continue;
				}
			}
		}
		bool HasVisibleChildren(RowHandle handle) {
			if (!DataController.IsLoaded(handle))
				return false;
			return GetFilteredChildren(handle).Any();
		}
		bool IsVisible(RowHandle handle) {
			return (bool)ExpressionEvaluator.Evaluate(FilterClient.GetObject(handle));
		}
		protected virtual IEnumerable<RowHandle> GetChildrenInternal(RowHandle handle) {
			IEnumerable<RowHandle> handles = handle.IsRoot ? GetRootRowHandlesInternal() : DataController.GetChildHandles(handle);
			return handles.Union(SortClient.GetMappings(handle, AddMappingSourceToTarget));
		}
		protected virtual RowHandle AddMappingSourceToTarget(string sourceFieldName, string targetFieldName) {
			RowHandle parentHandle = GetHandleByFieldName(FieldNameHelper.GetParentFieldName(targetFieldName));
			RowHandle handle = GetHandleByFieldName(sourceFieldName);
			Mapping mapping = new Mapping { Handle = handle, ParentHandle = parentHandle, SourceFieldName = sourceFieldName, TargetFieldName = targetFieldName };
			Mappings.Add(handle, mapping);
			Mappings.Add(targetFieldName, mapping);
			return handle;
		}
		protected virtual IEnumerable<RowHandle> GetRootRowHandlesInternal() {
			return DataController.GetChildHandles(RowHandle.Root).ToList();
		}
		public void Dispose() {
			FilterClient = null;
			SortClient = null;
			if (DataController != null) {
				DataController.SourceChanged -= sourceChangedEventHandler.OnEvent;
				DataController.SourceChanging -= sourceChangingEventHandler.OnEvent;
			}
			dataController = null;
		}
	}
	public class Mapping {
		public RowHandle Handle { get; set; }
		public RowHandle ParentHandle { get; set; }
		public string SourceFieldName { get; set; }
		public string TargetFieldName { get; set; }
	}
}
