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
using System.Diagnostics;
using System.Globalization;
using DevExpress.Office;
using Model = DevExpress.XtraSpreadsheet.Model;
using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
using ModelIVariantArray = DevExpress.XtraSpreadsheet.Model.IVariantArray;
using ModelVariantArray = DevExpress.XtraSpreadsheet.Model.VariantArray;
using ModelVariantValue = DevExpress.XtraSpreadsheet.Model.VariantValue;
using ModelVariantValueType = DevExpress.XtraSpreadsheet.Model.VariantValueType;
using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
namespace DevExpress.Spreadsheet {
	using DevExpress.XtraSpreadsheet.API.Native.Implementation;
	#region DataValidationType
	public enum DataValidationType {
		AnyValue = DevExpress.XtraSpreadsheet.Model.DataValidationType.None,
		WholeNumber = DevExpress.XtraSpreadsheet.Model.DataValidationType.Whole,
		Decimal = DevExpress.XtraSpreadsheet.Model.DataValidationType.Decimal,
		List = DevExpress.XtraSpreadsheet.Model.DataValidationType.List,
		Date = DevExpress.XtraSpreadsheet.Model.DataValidationType.Date,
		Time = DevExpress.XtraSpreadsheet.Model.DataValidationType.Time,
		TextLength = DevExpress.XtraSpreadsheet.Model.DataValidationType.TextLength,
		Custom = DevExpress.XtraSpreadsheet.Model.DataValidationType.Custom
	}
	#endregion
	#region DataValidationErrorStyle
	public enum DataValidationErrorStyle {
		Stop = DevExpress.XtraSpreadsheet.Model.DataValidationErrorStyle.Stop,
		Warning = DevExpress.XtraSpreadsheet.Model.DataValidationErrorStyle.Warning,
		Information = DevExpress.XtraSpreadsheet.Model.DataValidationErrorStyle.Information
	}
	#endregion
	#region DataValidationImeMode
	public enum DataValidationImeMode {
		NoControl = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.NoControl,
		On = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.On,
		Off = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.Off,
		Disabled = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.Disabled,
		Hiragana = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.Hiragana,
		FullKatakana = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.FullKatakana,
		HalfKatakana = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.HalfKatakana,
		FullAlpha = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.FullAlpha,
		HalfAlpha = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.HalfAlpha,
		FullHangul = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.FullHangul,
		HalfHangul = DevExpress.XtraSpreadsheet.Model.DataValidationImeMode.HalfHangul
	}
	#endregion
	#region DataValidationOperator
	public enum DataValidationOperator {
		Between = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.Between,
		NotBetween = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.NotBetween,
		Equal = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.Equal,
		NotEqual = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.NotEqual,
		LessThan = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.LessThan,
		LessThanOrEqual = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.LessThanOrEqual,
		GreaterThan = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.GreaterThan,
		GreaterThanOrEqual = DevExpress.XtraSpreadsheet.Model.DataValidationOperator.GreaterThanOrEqual
	}
	#endregion
	#region DataValidation
	public interface DataValidation {
		Range Range { get; set; }
		DataValidationType ValidationType { get; }
		DataValidationOperator Operator { get; set; }
		bool AllowBlank { get; set; }
		DataValidationImeMode ImeMode { get; set; }
		bool ShowDropDown { get; set; }
		bool ShowInputMessage { get; set; }
		string InputTitle { get; set; }
		string InputMessage { get; set; }
		bool ShowErrorMessage { get; set; }
		DataValidationErrorStyle ErrorStyle { get; set; }
		string ErrorTitle { get; set; }
		string ErrorMessage { get; set; }
		ValueObject Criteria { get; set; }
		ValueObject Criteria2 { get; set; }
	}
	#endregion
	#region DataValidationCollection
	public interface DataValidationCollection : ISimpleCollection<DataValidation> {
		DataValidation Add(Range range, DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria, ValueObject criteria2);
		DataValidation Add(Range range, DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria);
		DataValidation Add(Range range, DataValidationType validationType, ValueObject criteria);
		bool Remove(DataValidation item);
		void RemoveAt(int index);
		void Clear();
		bool Contains(DataValidation item);
		int IndexOf(DataValidation item);
		DataValidation GetDataValidation(Cell cell);
		IList<DataValidation> GetDataValidations(Range range);
		IList<DataValidation> GetDataValidations(DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria, ValueObject criteria2);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Office.Model;
	#region NativeDataValidation
	partial class NativeDataValidation : NativeObjectBase, DataValidation {
		#region ExpressionTypeTables
		static HashSet<Type> rangeExpressionTypeTable = CreateRangeExpressionTypeTable();
		static HashSet<Type> CreateRangeExpressionTypeTable() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(Model.ParsedThingRef));
			result.Add(typeof(Model.ParsedThingRef3d));
			result.Add(typeof(Model.ParsedThingRefRel));
			result.Add(typeof(Model.ParsedThingRef3dRel));
			result.Add(typeof(Model.ParsedThingArea));
			result.Add(typeof(Model.ParsedThingArea3d));
			result.Add(typeof(Model.ParsedThingAreaN));
			result.Add(typeof(Model.ParsedThingArea3dRel));
			result.Add(typeof(Model.ParsedThingRange));
			result.Add(typeof(Model.ParsedThingTable));
			result.Add(typeof(Model.ParsedThingTableExt));
			return result;
		}
		static HashSet<Type> literalExpressionTypeTable = CreateLiteralExpressionTypeTable();
		static HashSet<Type> CreateLiteralExpressionTypeTable() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(Model.ParsedThingInteger));
			result.Add(typeof(Model.ParsedThingNumeric));
			result.Add(typeof(Model.ParsedThingBoolean));
			result.Add(typeof(Model.ParsedThingError));
			result.Add(typeof(Model.ParsedThingStringValue));
			return result;
		}
		#endregion
		readonly Model.DataValidation modelDataValidation;
		readonly NativeWorksheet worksheet;
		public NativeDataValidation(NativeWorksheet worksheet, Model.DataValidation modelDataValidation)
			: base() {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(modelDataValidation, "modelDataValidation");
			this.modelDataValidation = modelDataValidation;
			this.worksheet = worksheet;
		}
		ModelWorkbookDataContext DataContext { get { return worksheet.ModelWorkbook.DataContext; } }
		#region DataValidation Members
		public Range Range {
			get {
				CheckValid();
				return new NativeRange(modelDataValidation.CellRange, worksheet);
			}
			set {
				CheckValid();
				Guard.ArgumentNotNull(value, "Range");
				Model.DataValidationModifyRangeCommand command = new Model.DataValidationModifyRangeCommand(
					worksheet.ModelWorksheet,
					ApiErrorHandler.Instance,
					modelDataValidation,
					worksheet.GetModelRange(value));
				command.Execute();
			}
		}
		public DataValidationType ValidationType {
			get {
				CheckValid();
				return (DataValidationType)modelDataValidation.Type;
			}
			set {
				CheckValid();
				modelDataValidation.Type = (Model.DataValidationType)value;
			}
		}
		public DataValidationOperator Operator {
			get {
				CheckValid();
				return (DataValidationOperator)modelDataValidation.ValidationOperator;
			}
			set {
				CheckValid();
				modelDataValidation.ValidationOperator = (Model.DataValidationOperator)value;
			}
		}
		public bool AllowBlank {
			get {
				CheckValid();
				return modelDataValidation.AllowBlank;
			}
			set {
				CheckValid();
				modelDataValidation.AllowBlank = value;
			}
		}
		public DataValidationImeMode ImeMode {
			get {
				CheckValid();
				return (DataValidationImeMode)modelDataValidation.ImeMode;
			}
			set {
				CheckValid();
				modelDataValidation.ImeMode = (Model.DataValidationImeMode)value;
			}
		}
		public bool ShowDropDown {
			get {
				CheckValid();
				return !modelDataValidation.SuppressDropDown;
			}
			set {
				CheckValid();
				modelDataValidation.SuppressDropDown = !value;
			}
		}
		public bool ShowInputMessage {
			get {
				CheckValid();
				return modelDataValidation.ShowInputMessage;
			}
			set {
				CheckValid();
				modelDataValidation.ShowInputMessage = value;
			}
		}
		public string InputTitle {
			get {
				CheckValid();
				return CheckNullOrEmpty(modelDataValidation.PromptTitle);
			}
			set {
				CheckValid();
				modelDataValidation.PromptTitle = CheckNullOrEmpty(value);
			}
		}
		public string InputMessage {
			get {
				CheckValid();
				return CheckNullOrEmpty(modelDataValidation.Prompt);
			}
			set {
				CheckValid();
				modelDataValidation.Prompt = CheckNullOrEmpty(value);
			}
		}
		public bool ShowErrorMessage {
			get {
				CheckValid();
				return modelDataValidation.ShowErrorMessage;
			}
			set {
				CheckValid();
				modelDataValidation.ShowErrorMessage = value;
			}
		}
		public DataValidationErrorStyle ErrorStyle {
			get {
				CheckValid();
				return (DataValidationErrorStyle)modelDataValidation.ErrorStyle;
			}
			set {
				CheckValid();
				modelDataValidation.ErrorStyle = (Model.DataValidationErrorStyle)value;
			}
		}
		public string ErrorTitle {
			get {
				CheckValid();
				return CheckNullOrEmpty(modelDataValidation.ErrorTitle);
			}
			set {
				CheckValid();
				modelDataValidation.ErrorTitle = CheckNullOrEmpty(value);
			}
		}
		public string ErrorMessage {
			get {
				CheckValid();
				return CheckNullOrEmpty(modelDataValidation.Error);
			}
			set {
				CheckValid();
				modelDataValidation.Error = CheckNullOrEmpty(value);
			}
		}
		public ValueObject Criteria {
			get {
				CheckValid();
				Model.ParsedExpression expression = modelDataValidation.Expression1;
				ValueObject result = CreateValueObject(expression);
				if (result == null) {
					string formula = modelDataValidation.Formula1;
					if (string.IsNullOrEmpty(formula))
						result = ValueObject.Empty;
					else if (expression == null || expression.Count < 1)
						result = new ValueObject(string.Empty, formula);
					else {
						ModelWorkbookDataContext context = DataContext;
						context.PushCulture(CultureInfo.InvariantCulture);
						try {
							result = new ValueObject(formula, modelDataValidation.Formula1);
						}
						finally {
							context.PopCulture();
						}
					}
				}
				return result;
			}
			set {
				CheckValid();
				if (value == null || value.IsEmpty)
					modelDataValidation.Formula1 = string.Empty;
				else if (value.IsFormula) {
					if (string.IsNullOrEmpty(value.Formula)) {
						ModelWorkbookDataContext context = DataContext;
						context.PushCulture(CultureInfo.InvariantCulture);
						try {
							modelDataValidation.Formula1 = value.FormulaInvariant;
						}
						finally {
							context.PopCulture();
						}
					}
					else
						modelDataValidation.Formula1 = value.Formula;
				}
				else {
					Model.OperandDataType dataType = modelDataValidation.Type == Model.DataValidationType.List ? Model.OperandDataType.Reference : Model.OperandDataType.Value;
					Model.ParsedExpression expression = CreateExpression(value.ModelValue, dataType);
					modelDataValidation.BeginUpdate();
					try {
						modelDataValidation.SetFormula1(expression);
					}
					finally {
						modelDataValidation.EndUpdate();
					}
				}
			}
		}
		public ValueObject Criteria2 {
			get {
				CheckValid();
				Model.ParsedExpression expression = modelDataValidation.Expression2;
				ValueObject result = CreateValueObject(expression);
				if (result == null) {
					string formula = modelDataValidation.Formula2;
					if (string.IsNullOrEmpty(formula))
						result = ValueObject.Empty;
					else if (expression == null || expression.Count < 1)
						result = new ValueObject(string.Empty, formula);
					else {
						ModelWorkbookDataContext context = DataContext;
						context.PushCulture(CultureInfo.InvariantCulture);
						try {
							result = new ValueObject(formula, modelDataValidation.Formula2);
						}
						finally {
							context.PopCulture();
						}
					}
				}
				return result;
			}
			set {
				CheckValid();
				if (value == null || value.IsEmpty)
					modelDataValidation.Formula2 = string.Empty;
				else if (value.IsFormula) {
					if (string.IsNullOrEmpty(value.Formula)) {
						ModelWorkbookDataContext context = DataContext;
						context.PushCulture(CultureInfo.InvariantCulture);
						try {
							modelDataValidation.Formula2 = value.FormulaInvariant;
						}
						finally {
							context.PopCulture();
						}
					}
					else
						modelDataValidation.Formula2 = value.Formula;
				}
				else {
					Model.ParsedExpression expression = CreateExpression(value.ModelValue, Model.OperandDataType.Value);
					modelDataValidation.BeginUpdate();
					try {
						modelDataValidation.SetFormula2(expression);
					}
					finally {
						modelDataValidation.EndUpdate();
					}
				}
			}
		}
		#endregion
		string CheckNullOrEmpty(string value) {
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			return value;
		}
		ValueObject CreateValueObject(Model.ParsedExpression expression) {
			ValueObject result = null;
			if (expression != null && expression.Count == 1) {
				Model.IParsedThing ptg = expression[0];
				if (literalExpressionTypeTable.Contains(ptg.GetType())) {
					ModelVariantValue modelValue = expression.Evaluate(DataContext);
					result = new ValueObject(modelValue);
				}
				else if (ptg.DataType == Model.OperandDataType.Reference && rangeExpressionTypeTable.Contains(ptg.GetType())) {
					ModelWorkbookDataContext context = worksheet.ModelWorkbook.DataContext;
					context.PushCurrentCell(modelDataValidation.CellRange.TopLeft);
					try {
						ModelVariantValue modelValue = expression.Evaluate(DataContext);
						if (modelValue.IsCellRange) {
							Model.CellRangeBase modelRangeBase = modelValue.CellRangeValue;
							int sheetIndex = worksheet.ModelWorkbook.Sheets.GetIndexById(modelRangeBase.Worksheet.SheetId);
							result = new ValueObject(modelValue, worksheet.NativeWorkbook.Worksheets[sheetIndex] as NativeWorksheet);
						}
					}
					finally {
						context.PopCurrentCell();
					}
				}
			}
			return result;
		}
		Model.ParsedExpression CreateExpression(ModelVariantValue value, Model.OperandDataType dataType) {
			if (value.IsCellRange) {
				Model.ParsedExpression result = new Model.ParsedExpression();
				Model.CellRangeBase rangeValue = value.CellRangeValue;
				Model.BasicExpressionCreatorParameter parameter = Model.BasicExpressionCreatorParameter.ShouldEncloseUnion;
				if (!object.ReferenceEquals(modelDataValidation.Sheet, rangeValue.Worksheet))
					parameter |= Model.BasicExpressionCreatorParameter.ShouldCreate3d;
				Model.BasicExpressionCreator.CreateCellRangeExpression(result, rangeValue, parameter, dataType, DataContext);
				return result;
			}
			return Model.BasicExpressionCreator.CreateExpressionForVariantValue(value, dataType, DataContext);
		}
		public override bool Equals(object obj) {
			if (!IsValid)
				return false;
			NativeDataValidation other = obj as NativeDataValidation;
			if (other == null || !other.IsValid)
				return false;
			return object.ReferenceEquals(modelDataValidation, other.modelDataValidation);
		}
		public override int GetHashCode() {
			if (!IsValid)
				return -1;
			return modelDataValidation.GetHashCode();
		}
		protected internal bool IsRangeIntersects(Range range) {
			Model.CellRangeBase other = worksheet.GetModelRange(range);
			return modelDataValidation.CellRange.Intersects(other);
		}
	}
	#endregion
	#region NativeDataValidationCollection
	partial class NativeDataValidationCollection : NativeObjectBase, DataValidationCollection {
		#region Fields
		readonly NativeWorksheet nativeWorksheet;
		readonly List<NativeDataValidation> innerList;
		#endregion
		public NativeDataValidationCollection(NativeWorksheet nativeWorksheet) {
			this.nativeWorksheet = nativeWorksheet;
			this.innerList = new List<NativeDataValidation>();
			Initialize();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return InnerList.Count;
			}
		}
		public DataValidation this[int index] {
			get {
				CheckValid();
				return InnerList[index];
			}
		}
		protected internal Model.Worksheet ModelWorksheet { get { return nativeWorksheet.ModelWorksheet; } }
		protected internal List<NativeDataValidation> InnerList { get { return innerList; } }
		Model.DataValidationCollection ModelCollection { get { return ModelWorksheet.DataValidations; } }
		#endregion
		#region Internal
		protected internal void Initialize() {
			ModelCollection.ForEach(RegisterItem);
			SubscribeEvents();
		}
		void RegisterItem(Model.DataValidation item) {
			innerList.Add(new NativeDataValidation(nativeWorksheet, item));
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
		#endregion
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			ModelCollection.OnAdd += OnAdd;
			ModelCollection.OnRemoveAt += OnRemoveAt;
			ModelCollection.OnInsert += OnInsert;
			ModelCollection.OnClear += OnClear;
			ModelCollection.OnAddRange += OnAddRange;
		}
		protected internal void UnsubscribeEvents() {
			ModelCollection.OnAdd -= OnAdd;
			ModelCollection.OnRemoveAt -= OnRemoveAt;
			ModelCollection.OnInsert -= OnInsert;
			ModelCollection.OnClear -= OnClear;
			ModelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.DataValidation> modelArgs = e as UndoableCollectionAddEventArgs<Model.DataValidation>;
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
			UndoableCollectionInsertEventArgs<Model.DataValidation> modelArgs = e as UndoableCollectionInsertEventArgs<Model.DataValidation>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, new NativeDataValidation(nativeWorksheet, modelArgs.Item));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.DataValidation> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.DataValidation>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.DataValidation> collection = modelArgs.Collection;
			foreach (Model.DataValidation modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<DataValidation> Members
		public IEnumerator<DataValidation> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<DataValidation, NativeDataValidation>(innerList.GetEnumerator());
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
		#region DataValidationCollection Members
		public DataValidation Add(Range range, DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria, ValueObject criteria2) {
			CheckValid();
			CheckArguments(range, validationType, validationOperator, criteria, criteria2);
			ModelWorksheet.Workbook.BeginUpdate();
			try {
				Model.DataValidationAddCommand command = new Model.DataValidationAddCommand(
					nativeWorksheet.ModelWorksheet, 
					ApiErrorHandler.Instance, 
					nativeWorksheet.GetModelRange(range),
					(Model.DataValidationType)validationType,
					(Model.DataValidationOperator)validationOperator);
				if (!command.Execute())
					return null;
				DataValidation newItem = InnerList[Count - 1];
				newItem.Criteria = criteria;
				newItem.Criteria2 = criteria2;
				return newItem;
			}
			finally {
				ModelWorksheet.Workbook.EndUpdate();
			}
		}
		public DataValidation Add(Range range, DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria) {
			return Add(range, validationType, validationOperator, criteria, ValueObject.Empty);
		}
		public DataValidation Add(Range range, DataValidationType validationType, ValueObject criteria) {
			return Add(range, validationType, DataValidationOperator.Between, criteria, ValueObject.Empty);
		}
		public bool Remove(DataValidation item) {
			int index = IndexOf(item);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public void RemoveAt(int index) {
			CheckValid();
			ModelCollection.RemoveAt(index);
		}
		public void Clear() {
			CheckValid();
			ModelCollection.Clear();
		}
		public bool Contains(DataValidation item) {
			return IndexOf(item) != -1;
		}
		public int IndexOf(DataValidation item) {
			CheckValid();
			NativeDataValidation nativeItem = item as NativeDataValidation;
			if (nativeItem != null)
				return InnerList.IndexOf(nativeItem);
			return -1;
		}
		public DataValidation GetDataValidation(Cell cell) {
			CheckValid();
			if (cell != null && object.ReferenceEquals(cell.Worksheet, nativeWorksheet)) {
				foreach (NativeDataValidation item in innerList) {
					if (item.IsRangeIntersects(cell))
						return item;
				}
			}
			return null;
		}
		public IList<DataValidation> GetDataValidations(Range range) {
			CheckValid();
			List<DataValidation> result = new List<DataValidation>();
			if (range != null && object.ReferenceEquals(range.Worksheet, nativeWorksheet)) {
				foreach (NativeDataValidation item in innerList) {
					if (item.IsRangeIntersects(range))
						result.Add(item);
				}
			}
			return result;
		}
		public IList<DataValidation> GetDataValidations(DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria, ValueObject criteria2) {
			CheckValid();
			List<DataValidation> result = new List<DataValidation>();
			foreach (NativeDataValidation item in innerList) {
				if (InCriteria(item, validationType, validationOperator, criteria, criteria2))
					result.Add(item);
			}
			return result;
		}
		#endregion
		void CheckArguments(Range range, DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria, ValueObject criteria2) {
			Guard.ArgumentNotNull(range, "range");
			if (!object.ReferenceEquals(range.Worksheet, nativeWorksheet))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
			if (validationType == DataValidationType.AnyValue) {
				CriteriaEmpty(criteria, true);
				CriteriaEmpty(criteria2, false);
			}
			else if (validationType == DataValidationType.List || validationType == DataValidationType.Custom) {
				CriteriaNonEmpty(criteria, true);
				CriteriaEmpty(criteria2, false);
			}
			else if (validationOperator == DataValidationOperator.Between || validationOperator == DataValidationOperator.NotBetween) {
				CriteriaNonEmpty(criteria, true);
				CriteriaNonEmpty(criteria2, false);
			}
			else {
				CriteriaNonEmpty(criteria, true);
				CriteriaEmpty(criteria2, false);
			}
		}
		void CriteriaNonEmpty(ValueObject criteria, bool first) {
			if (ValueObject.IsNullOrEmpty(criteria))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(first ? XtraSpreadsheetStringId.Msg_ErrorFirstDVCriteriaMustNotBeEmpty : XtraSpreadsheetStringId.Msg_ErrorSecondDVCriteriaMustNotBeEmpty));
		}
		void CriteriaEmpty(ValueObject criteria, bool first) {
			if (!ValueObject.IsNullOrEmpty(criteria))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(first ? XtraSpreadsheetStringId.Msg_ErrorFirstDVCriteriaMustBeEmpty : XtraSpreadsheetStringId.Msg_ErrorSecondDVCriteriaMustBeEmpty));
		}
		bool InCriteria(NativeDataValidation validation, DataValidationType validationType, DataValidationOperator validationOperator, ValueObject criteria, ValueObject criteria2) {
			if (!validation.IsValid)
				return false;
			if (validation.ValidationType != validationType)
				return false;
			if (validationType == DataValidationType.AnyValue)
				return true;
			if (validationType == DataValidationType.List || validationType == DataValidationType.Custom)
				return validation.Criteria.Equals(criteria);
			if (validation.Operator != validationOperator)
				return false;
			if (validationOperator == DataValidationOperator.Between || validationOperator == DataValidationOperator.NotBetween)
				return validation.Criteria.Equals(criteria) && validation.Criteria2.Equals(criteria2);
			return validation.Criteria.Equals(criteria);
		}
	}
	#endregion
}
