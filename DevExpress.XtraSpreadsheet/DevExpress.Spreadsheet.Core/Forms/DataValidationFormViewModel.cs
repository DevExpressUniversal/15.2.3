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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region DataValidationViewModel
	public class DataValidationViewModel : ReferenceEditViewModel {
		#region Translation Tables
		readonly static Dictionary<string, DataValidationType> typeValues = CreateTypeValuesTable();
		readonly static Dictionary<string, DataValidationOperator> operatorValues = CreateOperatorValuesTable();
		readonly static Dictionary<string, DataValidationErrorStyle> errorStyleValues = CreateErrorStyleValuesTable();
		readonly static Dictionary<DataValidationType, FormulaHelperBase> formulaHelpers = CreateFormulaHelpersTable();
		static Dictionary<string, DataValidationType> CreateTypeValuesTable() {
			Dictionary<string, DataValidationType> result = new Dictionary<string, DataValidationType>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationAnyValue), DataValidationType.None);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationWholeNumber), DataValidationType.Whole);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationDecimal), DataValidationType.Decimal);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationList), DataValidationType.List);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationDate), DataValidationType.Date);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationTime), DataValidationType.Time);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationTextLength), DataValidationType.TextLength);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationCustom), DataValidationType.Custom);
			return result;
		}
		static Dictionary<string, DataValidationOperator> CreateOperatorValuesTable() {
			Dictionary<string, DataValidationOperator> result = new Dictionary<string, DataValidationOperator>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationBetween), DataValidationOperator.Between);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationNotBetween), DataValidationOperator.NotBetween);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationEqual), DataValidationOperator.Equal);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationNotEqual), DataValidationOperator.NotEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationGreaterThan), DataValidationOperator.GreaterThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationLessThan), DataValidationOperator.LessThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationGreaterThanOrEqual), DataValidationOperator.GreaterThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationLessThanOrEqual), DataValidationOperator.LessThanOrEqual);
			return result;
		}
		static Dictionary<string, DataValidationErrorStyle> CreateErrorStyleValuesTable() {
			Dictionary<string, DataValidationErrorStyle> result = new Dictionary<string, DataValidationErrorStyle>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationStop), DataValidationErrorStyle.Stop);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationWarning), DataValidationErrorStyle.Warning);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationInformation), DataValidationErrorStyle.Information);
			return result;
		}
		static Dictionary<DataValidationType, FormulaHelperBase> CreateFormulaHelpersTable() {
			Dictionary<DataValidationType, FormulaHelperBase> result = new Dictionary<DataValidationType, FormulaHelperBase>();
			result.Add(DataValidationType.Whole, new WholeNumberFormulaHelper());
			result.Add(DataValidationType.Decimal, new DecimalFormulaHelper());
			result.Add(DataValidationType.Date, new DateFormulaHelper());
			result.Add(DataValidationType.Time, new TimeFormulaHelper());
			result.Add(DataValidationType.TextLength, new TextLengthFormulaHelper());
			result.Add(DataValidationType.List, new ListFormulaHelper());
			result.Add(DataValidationType.Custom, new CustomFormulaHelper());
			return result;
		}
		#endregion
		#region Fields
		CellPosition activeCell;
		CellRangeBase activeRange = null;
		CellRangeBase dataValidationRange = null;
		bool ignoreBlank;
		bool inCellDropDown;
		string typeValue;
		string operatorValue;
		string formula1;
		string formula2;
		bool showMessage;
		string messageTitle;
		string message;
		bool showErrorMessage;
		string errorTitle;
		string errorMessage;
		string errorStyleValue;
		#endregion
		public DataValidationViewModel(ISpreadsheetControl control)
			: base(control) {
			Initialize();
		}
		#region Properties
		public Worksheet Worksheet { get { return (Worksheet)activeRange.Worksheet; } }
		public IEnumerable<string> TypeDataSource { get { return typeValues.Keys; } }
		public IEnumerable<string> OperatorDataSource { get { return operatorValues.Keys; } }
		public IEnumerable<string> ErrorStyleDataSource { get { return errorStyleValues.Keys; } }
		public CellPosition ActiveCell { get { return activeCell; } set { activeCell = value; } }
		public CellRangeBase ActiveRange { get { return activeRange; } set { activeRange = value; } }
		public CellRangeBase DataValidationRange { get { return dataValidationRange; } set { dataValidationRange = value; } }
		public bool Formula1Visible { get { return Type != DataValidationType.None; } }
		public bool InCellDropDownVisible { get { return Type == DataValidationType.List; } }
		public bool CanApplyChangesToAllCells { get { return dataValidationRange != null; } }
		public bool CanChangeOperator { get { return Formula1Visible && !InCellDropDownVisible && Type != DataValidationType.Custom; } }
		public bool Formula2Visible { get { return CanChangeOperator && (Operator == DataValidationOperator.Between || Operator == DataValidationOperator.NotBetween); } }
		public bool StopIconVisible { get { return showErrorMessage && ErrorStyle == DataValidationErrorStyle.Stop; } }
		public bool InfoIconVisible { get { return showErrorMessage && ErrorStyle == DataValidationErrorStyle.Information; } }
		public bool WarningIconVisible { get { return showErrorMessage && ErrorStyle == DataValidationErrorStyle.Warning; } }
		public bool IsDefault {
			get {
				return Type == DataValidationType.None && Operator == DataValidationOperator.Between &&
					   ignoreBlank && inCellDropDown && showMessage && String.IsNullOrEmpty(messageTitle) &&
					   String.IsNullOrEmpty(message) && showErrorMessage && String.IsNullOrEmpty(errorTitle) &&
					   String.IsNullOrEmpty(errorMessage) && ErrorStyle == DataValidationErrorStyle.Stop;
			}
		}
		#region Settings Tab
		public string Formula1Caption { get { return GetFormula1Caption(); } }
		public string Formula2Caption { get { return GetFormula2Caption(); } }
		public bool IgnoreBlank {
			get { return ignoreBlank; }
			set {
				if (IgnoreBlank == value)
					return;
				this.ignoreBlank = value;
				OnPropertyChanged("IgnoreBlank");
			}
		}
		public bool InCellDropDown {
			get { return inCellDropDown; }
			set {
				if (InCellDropDown == value)
					return;
				this.inCellDropDown = value;
				OnPropertyChanged("InCellDropDown");
			}
		}
		public DataValidationType Type {
			get { return typeValues[typeValue]; }
			set {
				if (Type == value)
					return;
				SetTypeCore(GetType(value));
			}
		}
		public string TypeValue {
			get { return typeValue; }
			set {
				if (TypeValue == value)
					return;
				SetTypeCore(value);
			}
		}
		void SetTypeCore(string value) {
			typeValue = value;
			OnPropertyChanged("TypeValue");
			OnPropertyChanged("CanChangeOperator");
			OnPropertyChanged("InCellDropDownVisible");
			NotifyFormulaVisibility();
		}
		public DataValidationOperator Operator {
			get { return operatorValues[operatorValue]; }
			set {
				if (Operator == value)
					return;
				SetOperatorCore(GetOperator(value));
			}
		}
		public string OperatorValue {
			get { return operatorValue; }
			set {
				if (OperatorValue == value)
					return;
				SetOperatorCore(value);
			}
		}
		void SetOperatorCore(string value) {
			operatorValue = value;
			OnPropertyChanged("OperatorValue");
			NotifyFormulaVisibility();
		}
		public string Formula1 {
			get { return formula1; }
			set {
				if (Formula1 == value)
					return;
				this.formula1 = value;
				OnPropertyChanged("Formula1");
			}
		}
		public string Formula2 {
			get { return formula2; }
			set {
				if (Formula2 == value)
					return;
				this.formula2 = value;
				OnPropertyChanged("Formula2");
			}
		}
		void NotifyFormulaVisibility() {
			OnPropertyChanged("Formula1Visible");
			OnPropertyChanged("Formula2Visible");
			OnPropertyChanged("Formula1Caption");
			OnPropertyChanged("Formula2Caption");
		}
		#endregion
		#region Input Message Tab
		public bool ShowMessage {
			get { return showMessage; }
			set {
				if (ShowMessage == value)
					return;
				this.showMessage = value;
				OnPropertyChanged("ShowMessage");
			}
		}
		public string MessageTitle {
			get { return messageTitle; }
			set {
				if (MessageTitle == value)
					return;
				this.messageTitle = value;
				OnPropertyChanged("MessageTitle");
			}
		}
		public string Message {
			get { return message; }
			set {
				if (Message == value)
					return;
				this.message = value;
				OnPropertyChanged("Message");
			}
		}
		#endregion
		#region Error Alert Tab
		public bool ShowErrorMessage {
			get { return showErrorMessage; }
			set {
				if (ShowErrorMessage == value)
					return;
				this.showErrorMessage = value;
				OnPropertyChanged("ShowErrorMessage");
				NotifyIconVisibility();
			}
		}
		public string ErrorTitle {
			get { return errorTitle; }
			set {
				if (ErrorTitle == value)
					return;
				this.errorTitle = value;
				OnPropertyChanged("ErrorTitle");
			}
		}
		public string ErrorMessage {
			get { return errorMessage; }
			set {
				if (ErrorMessage == value)
					return;
				this.errorMessage = value;
				OnPropertyChanged("ErrorMessage");
			}
		}
		public DataValidationErrorStyle ErrorStyle {
			get { return errorStyleValues[errorStyleValue]; }
			set { 
				if (ErrorStyle == value)
					return;
				SetErrorStyleCore(GetErrorStyle(value));
			}
		}
		public string ErrorStyleValue {
			get { return errorStyleValue; }
			set {
				if (ErrorStyleValue == value)
					return;
				SetErrorStyleCore(value);
			}
		}
		void SetErrorStyleCore(string value) {
			errorStyleValue = value;
			OnPropertyChanged("ErrorStyleValue");
			NotifyIconVisibility();
		}
		void NotifyIconVisibility() {
			OnPropertyChanged("InfoIconVisible");
			OnPropertyChanged("WarningIconVisible");
			OnPropertyChanged("StopIconVisible");
		}
		#endregion
		#endregion
		#region GetString
		string GetType(DataValidationType value) {
			foreach (string key in typeValues.Keys)
				if (typeValues[key] == value)
					return key;
			return String.Empty;
		}
		string GetOperator(DataValidationOperator value) {
			foreach (string key in operatorValues.Keys)
				if (operatorValues[key] == value)
					return key;
			return String.Empty;
		}
		string GetErrorStyle(DataValidationErrorStyle value) {
			foreach (string key in errorStyleValues.Keys)
				if (errorStyleValues[key] == value)
					return key;
			return String.Empty;
		}
		#endregion
		#region GetFormulaCaption
		string GetFormula1Caption() {
			if (Type == DataValidationType.None)
				return String.Empty;
			return GetFormulaCaptionCore(CreateHelper().Formula1Caption);
		}
		string GetFormula2Caption() {
			if (Type == DataValidationType.None)
				return String.Empty;
			return GetFormulaCaptionCore(CreateHelper().Formula2Caption);
		}
		string GetFormulaCaptionCore(string formula) {
			return String.IsNullOrEmpty(formula) ? formula : formula + ":";
		}
		#endregion
		#region ResetToDefaults
		void Initialize() {
			ignoreBlank = true;
			inCellDropDown = true;
			typeValue = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationAnyValue);
			operatorValue = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationBetween);
			formula1 = String.Empty;
			formula2 = String.Empty;
			showMessage = true;
			messageTitle = String.Empty;
			message = String.Empty;
			showErrorMessage = true;
			errorTitle = String.Empty;
			errorMessage = String.Empty;
			errorStyleValue = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationStop);
		}
		public void ResetToDefaults() {
			Initialize();
			OnPropertyChanged("IgnoreBlank");
			OnPropertyChanged("TypeValue");
			OnPropertyChanged("OperatorValue");
			OnPropertyChanged("CanChangeOperator");
			OnPropertyChanged("InCellDropDown");
			OnPropertyChanged("InCellDropDownVisible");
			OnPropertyChanged("Formula1");
			OnPropertyChanged("Formula2");
			NotifyFormulaVisibility();
			OnPropertyChanged("ShowMessage");
			OnPropertyChanged("MessageTitle");
			OnPropertyChanged("Message");
			OnPropertyChanged("ShowErrorMessage");
			OnPropertyChanged("ErrorTitle");
			OnPropertyChanged("ErrorMessage");
			OnPropertyChanged("ErrorStyleValue");
			NotifyIconVisibility();
		}
		#endregion
		public void SetSelection(bool changeToDataValidationRange) {
			SheetViewSelection selection = Worksheet.Selection;
			if (changeToDataValidationRange)
				selection.SetSelection(dataValidationRange);
			else {
				selection.SetSelection(activeRange);
				selection.SetActiveCellCore(activeCell);
			}
		}
		public void Refresh() {
			OnPropertyChanged("IgnoreBlank");
		}
		public bool Validate() {
			if (Type == DataValidationType.None)
				return true;
			IModelErrorInfo error = GetError();
			if (error == null)
				return true;
			Control.InnerControl.ErrorHandler.HandleError(error);
			return false;
		}
		protected internal IModelErrorInfo GetError() {
			return CreateHelper().GetError(formula1, formula2);
		}
		public void ApplyChanges() {
			DataValidationCommand command = new DataValidationCommand(Control);
			command.ApplyChanges(this);
		}
		public void PrepareDateTimeFormulas() {
			if (Type == DataValidationType.Date || Type == DataValidationType.Time) {
				FormulaHelperBase helper = CreateHelper();
				Formula1 = helper.TryParseDateTimeFormula(Formula1);
				Formula2 = helper.TryParseDateTimeFormula(Formula2);
			}
		}
		FormulaHelperBase CreateHelper() {
			FormulaHelperBase result = formulaHelpers[Type];
			result.Context = Worksheet.Workbook.DataContext;
			result.ActiveRangeTopLeft = Worksheet.Selection.ActiveRange.TopLeft;
			result.Operator = Operator;
			return result;
		}
		#region Inner Helpers
		#region FormulaHelperBase
		abstract class FormulaHelperBase {
			#region Properties
			public WorkbookDataContext Context { get; set; }
			public CellPosition ActiveRangeTopLeft { get; set; }
			public DataValidationOperator Operator { get; set; }
			protected CultureInfo Culture { get { return Context.Culture; } }
			protected abstract XtraSpreadsheetStringId Type { get; }
			protected virtual XtraSpreadsheetStringId CaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationValue; } }
			protected virtual XtraSpreadsheetStringId MinCaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationMinimum; } }
			protected virtual XtraSpreadsheetStringId MaxCaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationMaximum; } }
			public virtual string Formula1Caption { get { return GetFormula1Caption(); } }
			public virtual string Formula2Caption { get { return GetFormula2Caption(); } }
			string[] MaxMinCaption { get { return new string[2] { GetLocalized(MaxCaptionId), GetLocalized(MinCaptionId) }; } }
			protected virtual bool HasSecondFormula { get { return Operator == DataValidationOperator.Between || Operator == DataValidationOperator.NotBetween; } }
			bool IsMaxOperator { get { return Operator == DataValidationOperator.LessThan || Operator == DataValidationOperator.LessThanOrEqual; } }
			bool IsMinOperator {
				get {
					return Operator == DataValidationOperator.Between ||
						   Operator == DataValidationOperator.NotBetween ||
						   Operator == DataValidationOperator.GreaterThan ||
						   Operator == DataValidationOperator.GreaterThanOrEqual;
				}
			}
			#endregion
			#region GetFormulaCaption
			string GetFormula1Caption() {
				XtraSpreadsheetStringId id;
				if (IsMinOperator)
					id = MinCaptionId;
				else if (IsMaxOperator)
					id = MaxCaptionId;
				else
					id = CaptionId;
				return GetLocalized(id);
			}
			string GetFormula2Caption() {
				return IsMinOperator ? GetLocalized(MaxCaptionId) : String.Empty;
			}
			#endregion
			#region GetError
			public IModelErrorInfo GetError(string formula1, string formula2) {
				if (String.IsNullOrEmpty(formula1)) {
					if (HasSecondFormula)
						return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationBothFormulasAreEmpty, MaxMinCaption);
					else
						return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationFormulaIsEmpty, Formula1Caption);
				}
				ParsedExpression expression1 = PrepareExpression(formula1);
				if (expression1 == null)
					return new ModelErrorInfo(ModelErrorType.InvalidFormula);
				VariantValue value1 = expression1.Evaluate(Context);
				IModelErrorInfo error1 = GetErrorCore(value1, expression1, Formula1Caption);
				if (error1 != null)
					return error1;
				if (HasSecondFormula) {
					if (String.IsNullOrEmpty(formula2))
						return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationBothFormulasAreEmpty, MaxMinCaption);
					ParsedExpression expression2 = PrepareExpression(formula2);
					if (expression2 == null)
						return new ModelErrorInfo(ModelErrorType.InvalidFormula);
					VariantValue value2 = expression2.Evaluate(Context);
					IModelErrorInfo error2 = GetErrorCore(value2, expression2, Formula2Caption);
					if (error2 != null)
						return error2;
					if (IsSingleValue(expression1) && value1.IsNumeric && IsSingleValue(expression2) && value2.IsNumeric && value2.NumericValue < value1.NumericValue)
						return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationMinGreaterThanMax, MaxMinCaption);
				}
				return null;
			}
			protected virtual IModelErrorInfo GetErrorCore(VariantValue value, ParsedExpression expression, string formulaCaption) {
				if (IsSingleValue(expression))
					return GetSingleValueError(value, formulaCaption);
				else if (expression[0] is ParsedThingName) {
					if (expression[0] is ParsedThingNameX)
						return new ModelErrorInfo(ModelErrorType.DataValidationInvalidReference);
					if (value.IsError && value.ErrorValue == NameError.Instance)
						return new ModelErrorInfo(ModelErrorType.DataValidationDefinedNameNotFound);
				}
				else if (value.IsArray)
					return new ModelErrorInfo(ModelErrorType.DataValidationUnionRangeNotAllowed);
				else if (value.IsCellRange) {
					CellRangeBase cellRange = value.CellRangeValue;
					if (cellRange.RangeType == CellRangeType.UnionRange)
						return new ModelErrorInfo(ModelErrorType.DataValidationUnionRangeNotAllowed);
					if (cellRange.CellCount > 1) {
						string sumFormula = "=" + XtraSpreadsheetFunctionNameLocalizer.GetString(XtraSpreadsheetFunctionNameStringId.Sum) + "(A1:E5)";
						return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationMoreThanOneCellInRange, sumFormula);
					}
				}
				return null;
			}
			bool IsSingleValue(ParsedExpression expression) {
				return expression.Count == 1 && expression[0].DataType == OperandDataType.None;
			}
			protected virtual IModelErrorInfo GetSingleValueError(VariantValue value, string formulaCaption) {
				return null;
			}
			ParsedExpression PrepareExpression(string formula) {
				ParsedExpression expression = new ParsedExpression();
				if (!formula.StartsWith("=", StringComparison.Ordinal))
					SetupValueExpression(formula, expression);
				else {
					Context.PushCurrentCell(ActiveRangeTopLeft);
					try {
						expression = Context.ParseExpression(formula, OperandDataType.Default, true);
					}
					finally {
						Context.PopCurrentCell();
					}
				}
				return expression;
			}
			protected virtual void SetupValueExpression(string formula, ParsedExpression expression) {
				VariantValue value = Context.ConvertTextToVariantValueWithCaching(formula);
				expression.Add(ValueParsedThing.CreateInstance(value, Context));
			}
			#endregion
			protected string GetLocalized(XtraSpreadsheetStringId id) {
				return XtraSpreadsheetLocalizer.GetString(id);
			}
			public string TryParseDateTimeFormula(string formula) {
				if (String.IsNullOrEmpty(formula))
					return formula;
				ParsedExpression expression = PrepareExpression(formula);
				if (expression == null)
					return formula;
				VariantValue value = expression.Evaluate(Context);
				if (IsSingleValue(expression) && GetSingleValueError(value, String.Empty) == null)
					return DateToString(value.ToDateTime(Context));
				return formula;
			}
			protected virtual string DateToString(DateTime date) {
				return String.Empty;
			}
			protected string TimeToString(DateTime date) {
				return date.ToString(Culture.DateTimeFormat.LongTimePattern, Culture);
			}
		}
		#endregion
		#region WholeNumberFormulaHelper
		class WholeNumberFormulaHelper : FormulaHelperBase {
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationWholeNumber; } }
			protected override IModelErrorInfo GetSingleValueError(VariantValue value, string formulaCaption) {
				if (!value.IsNumeric)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidNonnumericValue, formulaCaption);
				double numericValue = value.NumericValue;
				if (numericValue != Math.Truncate(numericValue))
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidDecimalValue, GetLocalized(Type));
				return null;
			}
		}
		#endregion
		#region DecimalFormulaHelper
		class DecimalFormulaHelper : FormulaHelperBase {
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationDecimal; } }
			protected override IModelErrorInfo GetSingleValueError(VariantValue value, string formulaCaption) {
				if (!value.IsNumeric)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidNonnumericValue, formulaCaption);
				return null;
			}
		}
		#endregion
		#region DateFormulaHelper
		class DateFormulaHelper : FormulaHelperBase {
			#region Properties
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationDate; } }
			protected override XtraSpreadsheetStringId CaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationDate; } }
			protected override XtraSpreadsheetStringId MinCaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationStartDate; } }
			protected override XtraSpreadsheetStringId MaxCaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationEndDate; } }
			#endregion
			protected override IModelErrorInfo GetSingleValueError(VariantValue value, string formulaCaption) {
				if (value.IsError)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidNonnumericValue, formulaCaption);
				if (!value.IsNumeric)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidDate, formulaCaption);
				double numericValue = value.NumericValue;
				if (numericValue < 1 || WorkbookDataContext.IsErrorDateTimeSerial(numericValue, Context.DateSystem))
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidDate, formulaCaption);
				return null;
			}
			protected override string DateToString(DateTime date) {
				string shortDate = date.ToString(Culture.DateTimeFormat.ShortDatePattern, Culture);
				if (date.Hour > 0 || date.Minute > 0 || date.Second > 0)
					shortDate += " " + TimeToString(date);
				return shortDate;
			}
		}
		#endregion 
		#region TimeFormulaHelper
		class TimeFormulaHelper : FormulaHelperBase {
			#region Properties
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationTime; } }
			protected override XtraSpreadsheetStringId CaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationTime; } }
			protected override XtraSpreadsheetStringId MinCaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationStartTime; } }
			protected override XtraSpreadsheetStringId MaxCaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationEndTime; } }
			#endregion
			protected override IModelErrorInfo GetSingleValueError(VariantValue value, string formulaCaption) {
				if (value.IsError)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidNonnumericValue, formulaCaption);
				if (!value.IsNumeric)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidTime, formulaCaption);
				double numericValue = value.NumericValue;
				if (numericValue < 0 || numericValue >= 1)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidTime, formulaCaption);
				return null;
			}
			protected override string DateToString(DateTime date) {
				return TimeToString(date);
			}
		}
		#endregion
		#region TextLengthFormulaHelper
		class TextLengthFormulaHelper : WholeNumberFormulaHelper {
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationTextLength; } }
			protected override XtraSpreadsheetStringId CaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationLength; } }
			protected override IModelErrorInfo GetSingleValueError(VariantValue value, string formulaCaption) {
				IModelErrorInfo error = base.GetSingleValueError(value, formulaCaption);
				if (error != null)
					return error;
				if (value.NumericValue < 0)
					return new ModelErrorInfoWithArgs(ModelErrorType.DataValidationInvalidNegativeValue, GetLocalized(Type));
				return null;
			}
		}
		#endregion
		#region ListFormulaHelper
		class ListFormulaHelper : FormulaHelperBase {
			#region Properties
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationList; } }
			protected override XtraSpreadsheetStringId CaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationSource; } }
			protected override bool HasSecondFormula { get { return false; } }
			public override string Formula1Caption { get { return GetLocalized(CaptionId); } }
			public override string Formula2Caption { get { return String.Empty; } }
			#endregion
			protected override IModelErrorInfo GetErrorCore(VariantValue value, ParsedExpression expression, string formulaCaption) {
				if (value.IsNumeric)
					return new ModelErrorInfo(ModelErrorType.DataValidationMustBeRowOrColumnRange);
				else if (expression[0] is ParsedThingName && value.IsError && value.ErrorValue == NameError.Instance)
					return new ModelErrorInfo(ModelErrorType.DataValidationDefinedNameNotFound);
				else if (value.IsArray)
					return new ModelErrorInfo(ModelErrorType.DataValidationUnionRangeNotAllowed);
				else if (value.IsCellRange) {
					CellRangeBase cellRange = value.CellRangeValue;
					if (cellRange.RangeType == CellRangeType.UnionRange)
						return new ModelErrorInfo(ModelErrorType.DataValidationUnionRangeNotAllowed);
					if ((cellRange.Height > 1 && cellRange.Width != 1) || (cellRange.Width > 1 && cellRange.Height != 1))
						return new ModelErrorInfo(ModelErrorType.DataValidationMustBeRowOrColumnRange);
				}
				return null;
			}
			protected override void SetupValueExpression(string formula, ParsedExpression expression) {
				expression.Add(ValueParsedThing.CreateInstance(formula, Context));
			}
		}
		#endregion
		#region CustomFormulaHelper
		class CustomFormulaHelper : FormulaHelperBase {
			protected override XtraSpreadsheetStringId Type { get { return XtraSpreadsheetStringId.Caption_DataValidationCustom; } }
			protected override XtraSpreadsheetStringId CaptionId { get { return XtraSpreadsheetStringId.Caption_DataValidationFormula; } }
			protected override bool HasSecondFormula { get { return false; } }
			public override string Formula1Caption { get { return GetLocalized(CaptionId); } }
			public override string Formula2Caption { get { return String.Empty; } }
		}
		#endregion
		#endregion
	}
	#endregion
}
