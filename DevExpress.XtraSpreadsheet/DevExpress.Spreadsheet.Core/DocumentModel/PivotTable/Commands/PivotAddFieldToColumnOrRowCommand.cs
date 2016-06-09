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

using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotAddFieldToKeyFieldsCommand
	public class PivotInsertFieldToKeyFieldsCommand : PivotTableTransactedCommand {
		#region Fields
		readonly PivotTableAxis axis;
		readonly int fieldIndex;
		readonly int count;
		int insertIndex;
		#endregion
		public PivotInsertFieldToKeyFieldsCommand(PivotTable pivotTable, int fieldIndex, PivotTableAxis axis, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			if (axis == PivotTableAxis.None || axis == PivotTableAxis.Value)
				Exceptions.ThrowArgumentException("axis", axis);
			this.fieldIndex = fieldIndex;
			this.axis = axis;
			this.count = GetCount(axis);
			this.insertIndex = count;
		}
		public PivotInsertFieldToKeyFieldsCommand(PivotTable pivotTable, int fieldIndex, PivotTableAxis axis, int insertIndex, IErrorHandler errorHandler)
			: this(pivotTable, fieldIndex, axis, errorHandler) {
			ValueChecker.CheckValue(insertIndex, 0, count);
			this.insertIndex = insertIndex;
		}
		int GetCount(PivotTableAxis axis) {
			switch (axis) {
				case PivotTableAxis.Column:
					return PivotTable.ColumnFields.Count;
				case PivotTableAxis.Row:
					return PivotTable.RowFields.Count;
				case PivotTableAxis.Page:
					return PivotTable.PageFields.Count;
				default:
					return 0;
			}
		}
		protected internal override bool Validate() {
			return Validate(PivotTable, fieldIndex, axis, ErrorHandler);
		}
		public static bool Validate(PivotTable pivotTable, int fieldIndex, PivotTableAxis axis, IErrorHandler errorHandler) { 
			if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
				if (axis == PivotTableAxis.Value) {
					if (pivotTable.DataFields.Count == 256)
						if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableTooMuchDataFields)) == ErrorHandlingResult.Abort)
							return false;
				}
				else {
					PivotField field = pivotTable.Fields[fieldIndex];
					if (field.Items.Count > 1048576)
						if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldHasTooMuchItems)) == ErrorHandlingResult.Abort)
							return false;
					if (axis == PivotTableAxis.Column && field.Items.Count > 16384)
						if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldHasTooMuchItems_ColumnField)) == ErrorHandlingResult.Abort)
							return false;
					if (axis == PivotTableAxis.Page && pivotTable.PageFields.Count == 256)
						if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableTooMuchPageFields)) == ErrorHandlingResult.Abort)
							return false;
				}
			}
			else {
				if (pivotTable.DataFields.Count < 2)
					if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableNotEnoughDataFields)) == ErrorHandlingResult.Abort)
						return false;
				if (axis == PivotTableAxis.Value || axis == PivotTableAxis.Page)
					if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldCannotBePlacedOnThatAxis)) == ErrorHandlingResult.Abort)
						return false;
			}
			return true;
		}
		protected internal override void ExecuteCore() {
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				AddValuesField();
			else {
				PivotField field = PivotTable.Fields[fieldIndex];
				if (field.Axis == axis)
					AddOrdinalFieldSameAxis();
				else
					AddOrdinalField(field);
			}
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void AddOrdinalField(PivotField field) {
			switch (field.Axis) {
				case PivotTableAxis.Column:
					PivotTable.ColumnFields.RemoveByFieldIndex(fieldIndex);
					break;
				case PivotTableAxis.Page:
					PivotTable.PageFields.RemoveByFieldIndex(fieldIndex);
					break;
				case PivotTableAxis.Row:
					PivotTable.RowFields.RemoveByFieldIndex(fieldIndex);
					break;
			}
			switch (axis) {
				case PivotTableAxis.Column:
					PivotTable.ColumnFields.Insert(insertIndex, fieldIndex);
					break;
				case PivotTableAxis.Page:
					PivotTable.PageFields.Insert(insertIndex, new PivotPageField(PivotTable, fieldIndex));
					PivotTable.ClearFieldFilters(fieldIndex, PivotFilterClearType.AllExceptManual, ErrorHandler);
					if (!field.MultipleItemSelectionAllowed && PivotTable.FieldHasHiddenItems(fieldIndex))
						field.MultipleItemSelectionAllowed = true;
					break;
				case PivotTableAxis.Row:
					PivotTable.RowFields.Insert(insertIndex, fieldIndex);
					break;
			}
			field.Axis = axis;
		}
		void AddOrdinalFieldSameAxis() {
			switch (axis) {
				case PivotTableAxis.Column:
					AddSameAxisCore(PivotTable.ColumnFields);
					break;
				case PivotTableAxis.Page:
					AddSameAxisCore(PivotTable.PageFields);
					PivotTable.ClearFieldFilters(fieldIndex, PivotFilterClearType.AllExceptManual, ErrorHandler);
					break;
				case PivotTableAxis.Row:
					AddSameAxisCore(PivotTable.RowFields);
					break;
			}
		}
		void AddSameAxisCore<T>(PivotFieldReferenceCollection<T> collection) where T : IPivotFieldReference {
			int sourceIndex = collection.GetIndexElementByFieldIndex(fieldIndex);
			if (sourceIndex < insertIndex)
				--insertIndex;
			if (sourceIndex == insertIndex)
				return;
			collection.Move(sourceIndex, insertIndex);
		}
		void AddValuesField() {
			PivotTableColumnRowFieldIndices collection = axis == PivotTableAxis.Column ? PivotTable.ColumnFields : PivotTable.RowFields;
			if (collection.HasValuesField && collection.ValuesFieldIndex < insertIndex)
				--insertIndex;
			if (collection.ValuesFieldIndex == insertIndex)
				return;
			AddValuesField(PivotTable, collection, insertIndex);
		}
		public static void AddValuesField(PivotTable pivotTable, PivotTableColumnRowFieldIndices collection, int index) {
			pivotTable.RowFields.RemoveValuesField();
			pivotTable.ColumnFields.RemoveValuesField();
			collection.Insert(index, PivotTable.ValuesFieldFakeIndex);
			pivotTable.DataPosition = index == collection.Count - 1 ? -1 : index;
			pivotTable.DataOnRows = object.ReferenceEquals(pivotTable.RowFields, collection);
		}
	}
	#endregion
	#region PivotInsertDataFieldCommand
	public class PivotInsertDataFieldCommand : PivotTableTransactedCommand {
		readonly int fieldIndex;
		readonly string caption;
		readonly PivotDataConsolidateFunction? function;
		readonly int insertIndex;
		public PivotInsertDataFieldCommand(PivotTable pivotTable, int fieldIndex, string caption, int insertIndex, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			if (insertIndex < 0)
				Exceptions.ThrowArgumentException("insertIndex", insertIndex);
			this.fieldIndex = fieldIndex;
			this.caption = caption;
			this.insertIndex = insertIndex;
		}
		public PivotInsertDataFieldCommand(PivotTable pivotTable, int fieldIndex, string caption, PivotDataConsolidateFunction function, int insertIndex, IErrorHandler errorHandler)
			: this(pivotTable, fieldIndex, caption, insertIndex, errorHandler) {
			this.function = function;
		}
		protected internal override bool Validate() {
			IModelErrorInfo error = null;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				error = new ModelErrorInfo(ModelErrorType.PivotTableFieldNameIsInvalid); 
			return HandleError(error);
		}
		protected internal override void ExecuteCore() {
			PivotDataField fieldReference = new PivotDataField(PivotTable, fieldIndex);
			fieldReference.SetSubtotalCore(GenerateFunction());
			fieldReference.GenerateUniqueName(caption);
			PivotTable.DataFields.Insert(insertIndex, fieldReference);
			if (PivotTable.DataFields.Count == 2)
				CreateFakeValuesField();
			IncrementMeasureFilterIndexes(insertIndex);
			PivotTable.Fields[fieldIndex].IsDataField = true;
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void IncrementMeasureFilterIndexes(int dataFieldIndex) {
			PivotFilterCollection filters = PivotTable.Filters;
			for (int i = 0; i < filters.Count; i++) {
				PivotFilter filter = filters[i];
				if (filter.IsMeasureFilter && filter.MeasureFieldIndex.Value >= insertIndex)
					filter.MeasureFieldIndex++;
			}
		}
		PivotDataConsolidateFunction GenerateFunction() {
			if (function.HasValue)
				return function.Value;
			if (PivotTable.Cache.CacheFields[fieldIndex].SharedItems.ContainsOnlyNumbers)
				return PivotDataConsolidateFunction.Sum;
			return PivotDataConsolidateFunction.Count;
		}
		void CreateFakeValuesField() {
			PivotTableColumnRowFieldIndices collection = PivotTable.DataOnRows ? PivotTable.RowFields : PivotTable.ColumnFields;
			int defaultDataPosition = PivotTable.DataPosition;
			if (defaultDataPosition >= 0 && defaultDataPosition <= collection.Count)
				collection.Insert(defaultDataPosition, PivotTable.ValuesFieldFakeIndex);
			else
				collection.Add(PivotTable.ValuesFieldFakeIndex);
		}
	}
	#endregion
	#region PivotRemoveKeyFieldCommand
	public class PivotRemoveKeyFieldCommand : PivotTableTransactedCommand {
		readonly int fieldReferenceIndex;
		readonly PivotTableAxis axis;
		public PivotRemoveKeyFieldCommand(PivotTable pivotTable, int fieldReferenceIndex, PivotTableAxis axis, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.fieldReferenceIndex = fieldReferenceIndex;
			this.axis = axis;
		}
		protected internal override void ExecuteCore() {
			if (axis == PivotTableAxis.Value)
				RemoveFromDataFields();
			else
				RemoveFromKeyFields();
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void RemoveFromDataFields() {
			PivotDataField dataField = PivotTable.DataFields[fieldReferenceIndex];
			PivotTable.DataFields.RemoveAt(fieldReferenceIndex);
			PivotTable.RemoveMeasureFilters(fieldReferenceIndex);
			int fieldIndex = dataField.FieldIndex;
			PivotField field = PivotTable.Fields[fieldIndex];
			System.Diagnostics.Debug.Assert(field.IsDataField);
			field.IsDataField = IsDataField(fieldIndex);
			RemoveValuesFakeFieldIfNotEnoughtDataFields(PivotTable);
		}
		bool IsDataField(int fieldIndex) {
			PivotDataFieldCollection dataFields = PivotTable.DataFields;
			for (int i = 0; i < dataFields.Count; i++)
				if (dataFields[i].FieldIndex == fieldIndex)
					return true;
			return false;
		}
		void RemoveFromKeyFields() {
			IPivotFieldReference fieldReference = null;
			switch (axis) {
				case PivotTableAxis.Column:
					fieldReference = PivotTable.ColumnFields[fieldReferenceIndex];
					PivotTable.ColumnFields.RemoveAt(fieldReferenceIndex);
					break;
				case PivotTableAxis.Page:
					fieldReference = PivotTable.PageFields[fieldReferenceIndex];
					PivotTable.PageFields.RemoveAt(fieldReferenceIndex);
					break;
				case PivotTableAxis.Row:
					fieldReference = PivotTable.RowFields[fieldReferenceIndex];
					PivotTable.RowFields.RemoveAt(fieldReferenceIndex);
					break;
			}
			if (fieldReference.FieldIndex == PivotTable.ValuesFieldFakeIndex)
				RemoveValuesFakeField(PivotTable);
			else {
				PivotField field = PivotTable.Fields[fieldReference.FieldIndex];
				field.Axis = PivotTableAxis.None;
			}
		}
		internal static void RemoveValuesFakeField(PivotTable pivotTable) {
			pivotTable.ColumnFields.RemoveValuesField();
			pivotTable.RowFields.RemoveValuesField();
			for (int i = 0; i < pivotTable.DataFields.Count; ++i)
				pivotTable.Fields[pivotTable.DataFields[i].FieldIndex].IsDataField = false;
			pivotTable.DataFields.Clear();
		}
		internal static void RemoveValuesFakeFieldIfNotEnoughtDataFields(PivotTable pivotTable) {
			if (pivotTable.DataFields.Count < 2) {
				pivotTable.RowFields.RemoveValuesField();
				pivotTable.ColumnFields.RemoveValuesField();
			}
		}
	}
	#endregion
	#region RemoveFieldFromAllKeyFields
	public class PivotRemoveFieldFromKeyFieldsCommand : PivotTableTransactedCommand {
		readonly int fieldIndex;
		public PivotRemoveFieldFromKeyFieldsCommand(PivotTable pivotTable, int fieldIndex, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.fieldIndex = fieldIndex;
		}
		protected internal override void ExecuteCore() {
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex) {
				if (PivotTable.DataFields.Count > 1) 
					PivotRemoveKeyFieldCommand.RemoveValuesFakeField(PivotTable);
				return;
			}
			PivotField field = PivotTable.Fields[fieldIndex];
			switch (field.Axis) {
				case PivotTableAxis.Column:
					PivotTable.ColumnFields.RemoveByFieldIndex(fieldIndex);
					break;
				case PivotTableAxis.Row:
					PivotTable.RowFields.RemoveByFieldIndex(fieldIndex);
					break;
				case PivotTableAxis.Page:
					PivotTable.PageFields.RemoveByFieldIndex(fieldIndex);
					break;
				case PivotTableAxis.Value:
					Exceptions.ThrowInternalException();
					break;
			}
			for (int i = PivotTable.DataFields.Count - 1; i >= 0; --i) {
				if (PivotTable.DataFields[i].FieldIndex == fieldIndex) {
					PivotTable.DataFields.RemoveAt(i);
					PivotTable.RemoveMeasureFilters(i);
				}
			}
			field.IsDataField = false;
			field.Axis = PivotTableAxis.None;
			PivotRemoveKeyFieldCommand.RemoveValuesFakeFieldIfNotEnoughtDataFields(PivotTable);
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
	}
	#endregion
	#region PivotClearKeyFieldsCommand
	public class PivotClearKeyFieldsCommand : PivotTableTransactedCommand {
		readonly PivotTableAxis axisType;
		readonly bool clearAll;
		public PivotClearKeyFieldsCommand(PivotTable pivotTable, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.clearAll = true;
		}
		public PivotClearKeyFieldsCommand(PivotTable pivotTable, PivotTableAxis axisType, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.axisType = axisType;
		}
		protected internal override bool Validate() {
			return axisType != PivotTableAxis.None || clearAll;
		}
		protected internal override void ExecuteCore() {
			if (clearAll) {
				ClearKeyFieldCollection(PivotTable.RowFields);
				ClearKeyFieldCollection(PivotTable.ColumnFields);
				ClearKeyFieldCollection(PivotTable.PageFields);
				ClearDataFields();
			}
			else {
				switch (axisType) {
					case PivotTableAxis.Row:
						RemoveValuesFakeField(PivotTable.RowFields);
						ClearKeyFieldCollection(PivotTable.RowFields);
						break;
					case PivotTableAxis.Column:
						RemoveValuesFakeField(PivotTable.ColumnFields);
						ClearKeyFieldCollection(PivotTable.ColumnFields);
						break;
					case PivotTableAxis.Page:
						ClearKeyFieldCollection(PivotTable.PageFields);
						break;
					case PivotTableAxis.Value:
						ClearDataFields();
						PivotRemoveKeyFieldCommand.RemoveValuesFakeFieldIfNotEnoughtDataFields(PivotTable);
						break;
				}
			}
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void RemoveValuesFakeField(PivotTableColumnRowFieldIndices collection) {
			if (collection.HasValuesField) {
				PivotRemoveKeyFieldCommand.RemoveValuesFakeField(PivotTable);
				ClearMeasureFilters();
			}
		}
		void ClearDataFields() {
			foreach (int keyFieldIndex in PivotTable.DataFields.GetKeyIndicesEnumerable())
				PivotTable.Fields[keyFieldIndex].IsDataField = false;
			ClearMeasureFilters();
			PivotTable.DataFields.Clear();
		}
		void ClearMeasureFilters() {
			PivotFilterCollection filters = PivotTable.Filters;
			for (int i = filters.Count - 1; i >= 0; i--) {
				PivotFilter filter = filters[i];
				if (filter.IsMeasureFilter) {
					PivotTable.Fields[filter.FieldIndex].MeasureFilter = false;
					filters.Remove(filter);
				}
			}
		}
		void ClearKeyFieldCollection<T>(PivotFieldReferenceCollection<T> collection) where T : IPivotFieldReference {
			foreach (int keyFieldIndex in collection.GetKeyIndicesEnumerable())
				PivotTable.Fields[keyFieldIndex].Axis = PivotTableAxis.None;
			collection.Clear();
		}
	}
	#endregion
}
