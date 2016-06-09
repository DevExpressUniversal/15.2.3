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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	public interface PivotFilterCollection : ISimpleCollection<PivotFilter> {
		PivotFilter Add(PivotField field, PivotFilterType filterType);
		PivotFilter Add(PivotField field, PivotFilterType filterType, FilterValue value);
		PivotFilter Add(PivotField field, PivotFilterType filterType, FilterValue firstValue, FilterValue secondValue);
		PivotFilter Add(PivotField field, PivotDataField measureField, PivotFilterType filterType, FilterValue value);
		PivotFilter Add(PivotField field, PivotDataField measureField, PivotFilterType filterType, FilterValue firstValue, FilterValue secondValue);
		void Remove(PivotFilter filter);
		void RemoveAt(int index);
		void Clear();
		bool Contains(PivotFilter filter);
		int IndexOf(PivotFilter filter);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativePivotFilterCollection
	partial class NativePivotFilterCollection : NativeObjectBase, PivotFilterCollection {
		#region Fields
		readonly List<NativePivotFilter> innerList;
		readonly NativePivotTable parentPivotTable;
		#endregion
		public NativePivotFilterCollection(NativePivotTable parentPivotTable) {
			Guard.ArgumentNotNull(parentPivotTable, "parentPivotTable");
			this.innerList = new List<NativePivotFilter>();
			this.parentPivotTable = parentPivotTable;
			Populate();
			SubscribeEvents();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return innerList.Count;
			}
		}
		public PivotFilter this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		protected internal NativePivotTable Parent { get { return parentPivotTable; } }
		protected internal Model.PivotFilterCollection ModelCollection { get { return parentPivotTable.ModelItem.Filters; } }
		#endregion
		#region Internal
		void Populate() {
			ModelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(Model.PivotFilter item) {
			innerList.Add(CreateNativeObject(item));
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value) {
				InvalidateItems();
				UnsubscribeEvents();
			}
		}
		void InvalidateItems() {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				innerList[i].IsValid = false;
		}
		NativePivotFilter CreateNativeObject(Model.PivotFilter item) {
			return new NativePivotFilter(item, parentPivotTable);
		}
		#endregion
		#region SubscribeEvents
		void SubscribeEvents() {
			ModelCollection.OnAdd += OnAdd;
			ModelCollection.OnRemoveAt += OnRemoveAt;
			ModelCollection.OnInsert += OnInsert;
			ModelCollection.OnClear += OnClear;
			ModelCollection.OnAddRange += OnAddRange;
		}
		void UnsubscribeEvents() {
			ModelCollection.OnAdd -= OnAdd;
			ModelCollection.OnRemoveAt -= OnRemoveAt;
			ModelCollection.OnInsert -= OnInsert;
			ModelCollection.OnClear -= OnClear;
			ModelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.PivotFilter> modelArgs = e as UndoableCollectionAddEventArgs<Model.PivotFilter>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				innerList[index].IsValid = false;
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<Model.PivotFilter> modelArgs = e as UndoableCollectionInsertEventArgs<Model.PivotFilter>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, CreateNativeObject(modelArgs.Item));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.PivotFilter> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.PivotFilter>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.PivotFilter> collection = modelArgs.Collection;
			foreach (Model.PivotFilter modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<PivotFilter> Members
		public IEnumerator<PivotFilter> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<PivotFilter, NativePivotFilter>(innerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return innerList.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		bool ICollection.IsSynchronized {
			get {
				CheckValid();
				return ((IList)this.innerList).IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				CheckValid();
				return ((IList)this.innerList).SyncRoot;
			}
		}
		#endregion
		#region PivotFieldCollection Members
		#region Add
		public PivotFilter Add(PivotField field, PivotFilterType filterType) {
			CheckValid();
			return AddCore(new PivotAddFilterHelper(parentPivotTable, field, filterType));
		}
		public PivotFilter Add(PivotField field, PivotFilterType filterType, FilterValue value) {
			CheckValid();
			PivotAddFilterHelper helper = new PivotAddFilterHelper(parentPivotTable, field, filterType);
			helper.FirstValue = value;
			return AddCore(helper);
		}
		public PivotFilter Add(PivotField field, PivotFilterType filterType, FilterValue firstValue, FilterValue secondValue) {
			CheckValid();
			PivotAddFilterHelper helper = new PivotAddFilterHelper(parentPivotTable, field, filterType);
			helper.FirstValue = firstValue;
			helper.SecondValue = secondValue;
			return AddCore(helper);
		}
		public PivotFilter Add(PivotField field, PivotDataField measureField, PivotFilterType filterType, FilterValue value) {
			CheckValid();
			PivotAddFilterHelper helper = new PivotAddFilterHelper(parentPivotTable, field, filterType);
			helper.FirstValue = value;
			helper.MeasureField = measureField;
			return AddCore(helper);
		}
		public PivotFilter Add(PivotField field, PivotDataField measureField, PivotFilterType filterType, FilterValue firstValue, FilterValue secondValue) {
			CheckValid();
			PivotAddFilterHelper helper = new PivotAddFilterHelper(parentPivotTable, field, filterType);
			helper.FirstValue = firstValue;
			helper.SecondValue = secondValue;
			helper.MeasureField = measureField;
			return AddCore(helper);
		}
		PivotFilter AddCore(PivotAddFilterHelper helper) {
			if (!helper.Execute())
				return null;
			return innerList[Count - 1];
		}
		#endregion
		#region Remove
		public void Remove(PivotFilter filter) {
			if (filter == null)
				ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.UsingInvalidObject);
			int index = IndexOf(filter);
			if (index != -1)
				RemoveAt(index);
		}
		public void RemoveAt(int index) {
			CheckValid();
			ApiValueChecker.CheckIndex(index, innerList.Count - 1);
			NativePivotFilter filter = innerList[index];
			int fieldIndex = ((NativePivotField)filter.Field).FieldIndex;
			Model.PivotFilterClearType clearType = filter.ModelItem.IsMeasureFilter ? Model.PivotFilterClearType.Value : Model.PivotFilterClearType.Label;
			parentPivotTable.ModelItem.ClearFieldFilters(fieldIndex, clearType, ApiErrorHandler.Instance);
		}
		#endregion
		public void Clear() {
			CheckValid();
			parentPivotTable.ModelItem.ClearFilters(false, ApiErrorHandler.Instance);
		}
		public bool Contains(PivotFilter field) {
			return IndexOf(field) != -1;
		}
		public int IndexOf(PivotFilter field) {
			CheckValid();
			NativePivotFilter nativeItem = (NativePivotFilter)field;
			if (nativeItem != null)
				return innerList.IndexOf(nativeItem);
			return -1;
		}
		#endregion
	}
	#endregion
	#region PivotAddFilterHelper
	partial class PivotAddFilterHelper {
		#region Fields
		readonly NativePivotTable pivotTable;
		readonly PivotField field;
		readonly PivotFilterType filterType;
		Model.PivotAddFilterCommand command;
		PivotDataField measureField;
		FilterValue firstValue;
		FilterValue secondValue;
		#endregion
		public PivotAddFilterHelper(NativePivotTable pivotTable, PivotField field, PivotFilterType filterType) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			this.pivotTable = pivotTable;
			this.field = field;
			this.filterType = filterType;
		}
		#region Properties
		internal PivotDataField MeasureField { get { return measureField; } set { measureField = value; } }
		internal FilterValue FirstValue { get { return firstValue; } set { firstValue = value; } }
		internal FilterValue SecondValue { get { return secondValue; } set { secondValue = value; } }
		int FieldIndex { get { return ((NativePivotField)field).FieldIndex; } }
		bool IsValueFilter { get { return Model.PivotFilter.GetIsMeasureFilter((Model.PivotFilterType)filterType); } }
		bool IsTop10Filter { get { return filterType >= PivotFilterType.Count && filterType <= PivotFilterType.Sum; } }
		bool IsDateFilter { get { return filterType >= PivotFilterType.DateEqual && filterType <= PivotFilterType.DateNotBetween; } }
		bool CacheFieldContainsNonDateItems { get { return pivotTable.ModelItem.Cache.CacheFields[FieldIndex].SharedItems.ContainsNonDate; } }
		Model.WorkbookDataContext DataContext { get { return pivotTable.Workbook.Model.DocumentModel.DataContext; } }
		#endregion
		public bool Execute() {
			ValidateField();
			command = new Model.PivotAddFilterCommand(pivotTable.ModelItem, FieldIndex, (Model.PivotFilterType)filterType, ApiErrorHandler.Instance);
			AssignArguments();
			return command.Execute();
		}
		#region Assign
		void AssignArguments() {
			switch (filterType) {
				case PivotFilterType.ValueEqual:
				case PivotFilterType.ValueNotEqual:
				case PivotFilterType.ValueGreaterThan:
				case PivotFilterType.ValueGreaterThanOrEqual:
				case PivotFilterType.ValueLessThan:
				case PivotFilterType.ValueLessThanOrEqual:
				case PivotFilterType.Percent:
				case PivotFilterType.Sum:
				case PivotFilterType.Count:
					AssignFirstValue();
					AssignMeasureField();
					break;
				case PivotFilterType.ValueBetween:
				case PivotFilterType.ValueNotBetween:
					AssignFirstValue();
					AssignSecondValue();
					AssignMeasureField();
					break;
				case PivotFilterType.CaptionEqual:
				case PivotFilterType.CaptionNotEqual:
				case PivotFilterType.CaptionBeginsWith:
				case PivotFilterType.CaptionNotBeginsWith:
				case PivotFilterType.CaptionEndsWith:
				case PivotFilterType.CaptionNotEndsWith:
				case PivotFilterType.CaptionContains:
				case PivotFilterType.CaptionNotContains:
				case PivotFilterType.CaptionGreaterThan:
				case PivotFilterType.CaptionGreaterThanOrEqual:
				case PivotFilterType.CaptionLessThan:
				case PivotFilterType.CaptionLessThanOrEqual:
				case PivotFilterType.DateEqual:
				case PivotFilterType.DateNotEqual:
				case PivotFilterType.DateOlderThan:
				case PivotFilterType.DateOlderThanOrEqual:
				case PivotFilterType.DateNewerThan:
				case PivotFilterType.DateNewerThanOrEqual:
					AssignFirstValue();
					break;
				case PivotFilterType.CaptionBetween:
				case PivotFilterType.CaptionNotBetween:
				case PivotFilterType.DateBetween:
				case PivotFilterType.DateNotBetween:
					AssignFirstValue();
					AssignSecondValue();
					break;
			}
		}
		void AssignFirstValue() {
			if (firstValue == null)
				HandleError(Model.ModelErrorType.PivotFilterRequiresValue);
			FilterValue value = GetValidatedValue(firstValue);
			command.FirstValue = value.VariantValue;
			command.FirstValueIsDate = value.IsDateTime;
		}
		void AssignSecondValue() {
			if (secondValue == null)
				HandleError(Model.ModelErrorType.PivotFilterRequiresSecondValue);
			FilterValue value = GetValidatedValue(secondValue);
			command.SecondValue = value.VariantValue;
			command.SecondValueIsDate = value.IsDateTime;
		}
		void AssignMeasureField() {
			command.MeasureFieldIndex = GetValidatedMeasureField().ReferenceIndex;
		}
		#endregion
		#region Validate
		void ValidateField() {
			if (field == null)
				HandleError(Model.ModelErrorType.UsingInvalidObject);
			NativePivotField nativeField = (NativePivotField)field;
			if (!Object.ReferenceEquals(nativeField.ParentTable, pivotTable))
				HandleError(Model.ModelErrorType.UsingInvalidObject);
			if (nativeField.Axis == PivotAxisType.Page)
				HandleError(Model.ModelErrorType.PivotFilterCannotAddFilterToPageField);
		}
		NativePivotDataField GetValidatedMeasureField() {
			if (measureField == null)
				HandleError(Model.ModelErrorType.PivotFilterRequiresMeasureField);
			NativePivotDataField nativeDataField = (NativePivotDataField)measureField;
			if (!Object.ReferenceEquals(nativeDataField.ParentTable, pivotTable))
				HandleError(Model.ModelErrorType.UsingInvalidObject);
			return nativeDataField;
		}
		FilterValue GetValidatedValue(FilterValue value) {
			if (value.IsArray || value.IsEmpty)
				HandleError(Model.ModelErrorType.ErrorInvalidFilterArgument);
			if (IsDateFilter)
				return GetValidatedDateValue(value);
			if (IsValueFilter) {
				Model.VariantValue modelValue = value.VariantValue;
				if (!modelValue.IsNumeric)
					value = new FilterValue(ToNumeric(modelValue), false);
				if (IsTop10Filter)
					CheckTop10Filter(value.VariantValue.NumericValue);
			}
			return value;
		}
		Model.VariantValue ToNumeric(Model.VariantValue modelValue) {
			Model.VariantValue result = modelValue.ToNumeric(DataContext);
			if (!result.IsNumeric)
				HandleError(Model.ModelErrorType.ErrorInvalidFilterArgument);
			return result;
		}
		FilterValue GetValidatedDateValue(FilterValue value) {
			if (CacheFieldContainsNonDateItems)
				HandleError(Model.ModelErrorType.PivotCacheFieldContainsNonDateItems);
			if (value.IsDateTime)
				return value;
			Model.VariantValue modelValue = ToNumeric(value.VariantValue);
			if (Model.WorkbookDataContext.IsErrorDateTimeSerial(modelValue.NumericValue, DataContext.DateSystem))
				HandleError(Model.ModelErrorType.ErrorInvalidFilterArgument);
			return new FilterValue(modelValue, true);
		}
		void CheckTop10Filter(double numericValue) {
			System.Globalization.CultureInfo culture = DataContext.Culture;
			if (filterType == PivotFilterType.Percent)
				ApiValueChecker.CheckValue(numericValue, 0, 100, culture);
			else if (filterType == PivotFilterType.Sum)
				ApiValueChecker.CheckValue(numericValue, 0, Double.MaxValue, culture);
			else {
				ApiValueChecker.CheckValue(numericValue, 1, 2147483647, culture);
				if (numericValue - Math.Truncate(numericValue) != 0)
					HandleError(Model.ModelErrorType.PivotFilterTop10CountMustBeInteger);
			}
		}
		void HandleError(Model.ModelErrorType errorType) {
			ApiErrorHandler.Instance.HandleError(errorType);
		}
		#endregion
	}
	#endregion
}
