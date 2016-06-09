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
using System.Security;
using System.Text;
using System.Windows;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.Native;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
#if SL
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#else
#endif
namespace DevExpress.Xpf.Data {
	public interface ISelectionController {
		void BeginSelection();
		void EndSelection();
		int[] GetSelectedRows();
		void SetActuallyChanged();
		void SelectAll();
		void Clear();
		void SetSelected(int rowHandle, bool selected);
		void SetSelected(int rowHandle, bool selected, object selectionObject);
		bool IsSelectionLocked { get; }
		bool GetSelected(int controllerRow);
		object GetSelectedObject(int controllerRow);
		int Count { get; }
	}
	public abstract class DataProviderBase {
		protected static readonly ErrorInfo ValidErrorInfo = new ErrorInfo() { ErrorType = ErrorType.None };
		internal static Type GetListItemPropertyType(IList list, ICollectionView collectionView) {
			if(list == null)
				return null;
			Type listType = null;
			if(collectionView != null && collectionView.SourceCollection != null)
				listType = collectionView.SourceCollection.GetType();
			else
				listType = ListDataControllerHelper.GetListType(list);
			if(list.Count == 0 && listType.IsArray)
				return listType.GetElementType();
			Type itemType = ListDataControllerHelper.GetIndexerPropertyType(listType);
			if(itemType == null) {
				itemType = GenericTypeHelper.GetGenericIListTypeArgument(listType);
				if(itemType == typeof(object))
					itemType = null;
			}
			return itemType;
		}
		protected readonly WeakReference selfReference;
		object dataSource;
		public object DataSource {
			get { return dataSource; }
			set {
				if(ReferenceEquals(DataSource, value)) return;
				dataSource = value;
				OnDataSourceChanged();
			}
		}
		internal Type ItemType { 
			get {
				try {
					return ItemTypeCore;
				} catch(Exception) {
#if DEBUGTEST
					throw;
#else
					return null;
#endif
				}
			} 
		}
		internal virtual IRefreshable RefreshableSource { get { return null; } }
		internal virtual BindingListAdapterBase BindingListAdapter { get { return null; } }
		protected abstract Type ItemTypeCore { get; }
		protected DataProviderBase() {
			this.selfReference = new WeakReference(this);
		}
		#region grouping members
		public abstract bool AutoExpandAllGroups { get; set; }
		public abstract int GroupedColumnCount { get; }
		public abstract void ExpandAll();
		public abstract void CollapseAll();
		public abstract bool IsGroupRowExpanded(int controllerRow);
		public abstract bool IsGroupRowHandle(int rowHandle);
		public abstract bool TryGetGroupSummaryValue(int rowHandle, DevExpress.Xpf.Grid.SummaryItemBase item, out object value);
		public abstract int GetControllerRowByGroupRow(int groupRowHandle);
		public abstract void ChangeGroupExpanded(int controllerRow, bool recursive);
		public abstract void UpdateGroupSummary();
		#endregion
		internal void ThrowNotSupportedExceptionIfInServerMode() {
			if(DataSource is ICollectionView)
				throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.ICollectionViewNotSupported);
			if(IsServerMode || IsAsyncServerMode)
				throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.ServerAndInstantFeedbackModeNotSupported);
		}
		internal virtual bool SubscribeRowChangedForVisibleRows { get { return false; } }
		internal virtual bool IsSelfManagedItemsSource { get { return true; } }
		public abstract ISummaryItemOwner TotalSummaryCore { get; }
		public abstract ISummaryItemOwner GroupSummaryCore { get; }
		public abstract int DataRowCount { get; }
		public abstract bool IsCurrentRowEditing { get; }
		public abstract int VisibleCount { get; }
		public abstract int CurrentControllerRow { get; set; }
		public abstract int CurrentIndex { get; }
		public abstract DataColumnInfoCollection Columns { get; }
		public abstract DataColumnInfoCollection DetailColumns { get; }
		public abstract bool IsReady { get; }
		public ICollectionView CollectionViewSource { get { return DataSource as ICollectionView; } }
		protected internal abstract void OnDataSourceChanged();
		internal protected abstract BaseGridController DataController { get; }
		internal abstract bool IsServerMode { get; }
		internal abstract bool IsICollectionView { get; }
		internal abstract bool IsAsyncServerMode { get; }
		internal abstract bool IsAsyncOperationInProgress { get; set; }
		public abstract CriteriaOperator FilterCriteria { get; set; }
		public abstract bool IsUpdateLocked { get; }
		public abstract ValueComparer ValueComparer { get; }
		public abstract void MakeRowVisible(int rowHandle);
		public abstract int GetChildRowHandle(int rowHandle, int childIndex);
		public abstract int GetChildRowCount(int rowHandle);
		internal abstract void CancelAllGetRows();
		internal abstract void EnsureAllRowsLoaded(int firstRowIndex, int rowsCount);
		internal abstract void EnsureRowLoaded(int rowHandle);
		public abstract ISelectionController Selection { get; }
		public abstract bool IsValidRowHandle(int rowHandle);
		public abstract bool IsRowVisible(int rowHandle);
		public abstract int GetControllerRow(int visibleIndex);
		public abstract int GetRowVisibleIndexByHandle(int rowHandle);
		public abstract bool AllowEdit { get; }
		public bool IsColumnReadonly(string fieldName, bool requireColumn) {
			if(!AllowEdit)
				return true;
			DataColumnInfo info = Columns[fieldName];
			return info != null ? info.ReadOnly : requireColumn;
		}
		public abstract object GetRowValue(int rowHandle, string fieldName);
		public abstract object GetRowValue(int rowHandle, DataColumnInfo info);
		public abstract object GetRowValue(int rowHandle);
		public abstract void SetRowValue(RowHandle rowHandle, DataColumnInfo info, object value);
		public abstract void DoRefresh();
		public abstract void RefreshRow(int rowHandle);
		public abstract void BeginUpdate();
		public abstract void EndUpdate();
		internal abstract void ScheduleAutoPopulateColumns();
		internal abstract DataColumnInfo GetActualColumnInfo(string fieldName);
		public abstract object GetTotalSummaryValue(DevExpress.Xpf.Grid.SummaryItemBase item);
		public abstract int GetListIndexByRowHandle(int rowHandle);
		public abstract int GetRowHandleByListIndex(int listIndex);
		public abstract object GetRowByListIndex(int listSourceRowIndex);
		public abstract object GetCellValueByListIndex(int listSourceRowIndex, string fieldName);
		public abstract int GetRowLevelByControllerRow(int rowHandle);
		public abstract int GetActualRowLevel(int rowHandle, int level);
		public abstract object GetGroupRowValue(int rowHandle);
		public abstract object GetGroupRowValue(int rowHandle, ColumnBase column);
		public abstract int GetParentRowHandle(int rowHandle);
		public abstract DependencyObject GetRowState(int controllerRow, bool createNewIfNotExist);
		public abstract object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime, bool implyNullLikeEmptyStringWhenFiltering);
		public virtual object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime) {
			return GetUniqueColumnValues(column, includeFilteredOut, roundDataTime, false);
		}
		protected internal abstract void ForceClearData();
		public abstract void SynchronizeSummary();
		public abstract void UpdateTotalSummary();
		public abstract CriteriaOperator CalcColumnFilterCriteriaByValue(ColumnBase column, object columnValue);
		public abstract bool IsGroupRow(int visibleIndex);
		public abstract ErrorInfo GetErrorInfo(RowHandle rowHandle);
		public abstract void DeleteRow(RowHandle rowHandle);
		public abstract void CancelCurrentRowEdit();
		public abstract void BeginCurrentRowEdit();
		public abstract bool EndCurrentRowEdit();
#if DEBUGTEST
		protected internal int FindRowByRowValueCallCount { get; protected set; }
#endif
		public abstract int FindRowByRowValue(object value);
		public abstract int FindRowByValue(string fieldName, object value);
		public abstract bool CanColumnSortCore(string fieldName);
		internal abstract int GetGroupIndex(string fieldName);
		public abstract void Syncronize(IList<GridSortInfo> sortList, int groupCount, CriteriaOperator filterCriteria);
		protected internal virtual bool AllowUpdateFocusedRowData { get { return true; } }
		public bool CanSortCollectionView() {
			return CollectionViewSource != null ? CollectionViewSource.CanSort : true;
		}
		public bool CanGroupCollectionView() {
			return CollectionViewSource != null ? CollectionViewSource.CanGroup : true;
		}
		public virtual object GetNodeIdentifier(int rowHandle) {
			return GetListIndexByRowHandle(rowHandle);
		}
		static readonly int[] emptyIndices = new int[0];
		public virtual IEnumerable<int> GetRowListIndicesWithExpandedDetails() {
			return emptyIndices;
		}
		public virtual void ClearDetailInfo() { }
		public abstract RowDetailContainer GetRowDetailContainer(int controllerRow, Func<RowDetailContainer> createContainerDelegate, bool createNewIfNotExist);
		public void SetRowValue(int rowHandle, string fieldName, object value) {
			SetRowValue(new RowHandle(rowHandle), Columns[fieldName], value);
		}
		public int GetRowLevelByVisibleIndex(int visibleIndex) {
			return GetRowLevelByControllerRow(GetControllerRow(visibleIndex));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Instead use the GetListIndexByRowHandle method. For detailed information, see the list of breaking changes in DXperience v2012 vol 1.")]
		public int GetRowListIndex(int rowHandle) {
			return GetListIndexByRowHandle(rowHandle);
		}
		public object GetWpfRow(RowHandle rowHandle, int listSourceRowIndex = DataControlBase.InvalidRowIndex) {
			return new RowTypeDescriptor(selfReference, rowHandle, listSourceRowIndex);
		}
		public int GetParentRowIndex(int visibleIndex) {
			int controllerRow = GetParentRowHandle(GetControllerRow(visibleIndex));
			return GetRowVisibleIndexByHandle(controllerRow);
		}
		PropertyDescriptorCollection properties;
		internal void InvalidateRowPropertyDescriptors() {
			properties = null;
		}
		internal PropertyDescriptorCollection GetProperties() {
			if(properties == null) {
				List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
				for(int n = 0; n < Columns.Count; n++) {
					DataColumnInfo info = Columns[n];
					pds.Add(new RowPropertyDescriptor(info));
				}
				for(int n = 0; n < DetailColumns.Count; n++) {
					DataColumnInfo info = DetailColumns[n];
					pds.Add(new RowPropertyDescriptor(info));
				}
				properties = new PropertyDescriptorCollection(pds.ToArray());
			}
			return properties;
		}
		internal virtual int ConvertVisibleIndexToScrollIndex(int visibleIndex, bool allowFixedGroups) {
			return visibleIndex;
		}
		internal virtual int ConvertScrollIndexToVisibleIndex(int scrollIndex, bool allowFixedGroups) {
			return scrollIndex;
		}
		internal void OnAsyncTotalsReceived() {
			if(!IsValidRowHandle(CurrentControllerRow)) CurrentControllerRow = GetControllerRow(0);
		}
		#region ValidateAttribure
		IValidationAttributeOwner ValidationOwner { get { return DataController.VisualClient as IValidationAttributeOwner; } }
		Dictionary<DataColumnInfo, PropertyValidator> validationAttributes;
		int propertyValidatorsCount = 0;
		public virtual void RePopulateColumns() {
			ResetValidationAttributes();
			DataController.RePopulateColumns(false);
		}
		public void ResetValidationAttributes() {
			validationAttributes = null;
			propertyValidatorsCount = 0;
		}
		public bool HasValidationAttributes() {
			InitializeValidationAttributes();
			return propertyValidatorsCount > 0;
		}
#if DEBUGTEST
		internal int InitializeValidationAttributesCallCount { get; set; }
#endif
		void InitializeValidationAttributes() {
			if(validationAttributes != null) return;
			validationAttributes = new Dictionary<DataColumnInfo, PropertyValidator>();
#if DEBUGTEST
			InitializeValidationAttributesCallCount++;
#endif
			propertyValidatorsCount = 0;
			Type listItemType = ItemType; 
			foreach(DataColumnInfo columnInfo in Columns) {
				var validator = DataColumnAttributesExtensions.CreatePropertyValidator(columnInfo.PropertyDescriptor, listItemType);
				validationAttributes.Add(columnInfo, validator);
				if(validator != null)
					propertyValidatorsCount++;
			}
		}
		public virtual ErrorInfo GetErrorInfo(RowHandle rowHandle, string fieldName) {
			DataColumnInfo column = Columns[fieldName];
			if(column == null)
				return ValidErrorInfo;
			int index = column.Index;
			return GetErrorInfo(rowHandle.Value, index);
		}
		protected virtual ErrorInfo GetErrorInfo(int controllerRow, int column) {
			ErrorInfo result = DataController.GetErrorInfo(controllerRow, column);
			DataColumnInfo ci = Columns[column];
			if(ValidationOwner != null && !ValidationOwner.CalculateValidationAttribute(ci.Name, controllerRow)) {
				return result;
			}
			string errorText = GetValidationAttributesErrorText(controllerRow, ci);
			if(!string.IsNullOrEmpty(errorText))
				result.ErrorText = errorText;
			return result;
		}
		public string GetValidationAttributesErrorText(object value, int controllerRow, string columnName) {
			DataColumnInfo columnInfo = Columns[columnName];
			if(!ColumnSupportsValidation(columnInfo))
				return string.Empty;
			return GetValidationAttributesErrorTextCore(controllerRow, columnInfo, value);
		}
		public string GetValidationAttributesErrorText(int controllerRow, DataColumnInfo columnInfo) {
			if(!ColumnSupportsValidation(columnInfo))
				return string.Empty;
			return GetValidationAttributesErrorTextCore(controllerRow, columnInfo, GetRowValueForValidationAttribute(controllerRow, columnInfo.Name));
		}
		protected virtual object GetRowValueForValidationAttribute(int controllerRow, string columnName) {
			return DataController.GetRowValue(controllerRow, columnName);
		}
		string GetValidationAttributesErrorTextCore(int controllerRow, DataColumnInfo ci, object value) {
			object instance = GetRowValue(controllerRow);
			if(instance == null)
				return string.Empty;
			object convertedValue = value;
			try {
				convertedValue = ci.ConvertValue(value, true);
			}
			catch {
				return string.Empty;
			}
			return GetValidationAttributesErrorTextCore(convertedValue, instance, ci);
		}
		public string GetValidationAttributesErrorTextCore(object value, object instance, DataColumnInfo ci) {
			if(!ColumnSupportsValidation(ci) || DevExpress.Data.AsyncServerModeDataController.IsNoValue(instance))
				return string.Empty;
			DevExpress.Data.Access.ComplexPropertyDescriptor complexPropertyDescriptor = ci.PropertyDescriptor as DevExpress.Data.Access.ComplexPropertyDescriptor;
			if(complexPropertyDescriptor != null) {
				instance = complexPropertyDescriptor.GetOwnerOfLast(instance);
				if(instance == null)
					return string.Empty;
			}
			return validationAttributes[ci].GetErrorText(value, instance);
		}
		bool ColumnSupportsValidation(DataColumnInfo ci) {
			if(ci == null || ci.PropertyDescriptor == null || ci.Unbound)
				return false;
			if((validationAttributes != null) && !validationAttributes.ContainsKey(ci)) {
				ResetValidationAttributes();
			}
			InitializeValidationAttributes();
			return validationAttributes[ci] != null;
		}
		#endregion
		public virtual void InvalidateVisibleIndicesCache() {
		}
		public virtual object GetVisibleIndexByScrollIndex(int scrollIndex) { return scrollIndex; }
		protected UnboundColumnInfoCollection GetUnboundColumnsCore(IEnumerable<IColumnInfo> columns) {
			UnboundColumnInfoCollection res = new UnboundColumnInfoCollection();
			foreach(IColumnInfo col in columns) {
				if(col.UnboundType == UnboundColumnType.Bound || string.IsNullOrEmpty(col.FieldName))
					continue;
#if !SL
				bool isVisible = !(col is ConditionalFormattingColumnInfo);
#else
				bool isVisible = true;
#endif
				res.Add(new UnboundColumnInfo(col.FieldName, col.UnboundType, false, col.UnboundExpression, isVisible));
			}
			if(res.Count > 0)
				return res;
			return null;
		}
		protected internal virtual object GetFormatInfoCellValueByListIndex(int listIndex, string fieldName) {
			return GetCellValueByListIndex(listIndex, fieldName);
		}
	}
	public interface IValidationAttributeOwner {
		bool CalculateValidationAttribute(string columnName, int controllerRow);
	}
}
