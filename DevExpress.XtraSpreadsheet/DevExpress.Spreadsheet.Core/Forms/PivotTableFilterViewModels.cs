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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region PivotTableFiltersViewModelBase
	public abstract class PivotTableFiltersViewModelBase : ViewModelBase {
		#region Fields
		readonly ISpreadsheetControl control;
		VariantValue firstValue = VariantValue.Missing;
		VariantValue secondValue = VariantValue.Missing;
		bool firstValueIsDate;
		bool secondValueIsDate;
		string firstStringValue;
		string secondStringValue;
		string filterTypeValue;
		#endregion
		protected PivotTableFiltersViewModelBase(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			Initialize();
		}
		#region Properties
		public PivotTableFiltersCommandBase Command { get; set; }
		protected ISpreadsheetControl Control { get { return control; } }
		protected abstract Dictionary<string, PivotFilterType> FilterTypesTable { get; }
		protected virtual XtraSpreadsheetStringId DefaultFilterTypeId { get { return XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals; } }
		public bool IsOneValueFilter { get { return !IsTwoValueFilter; } }
		public abstract bool IsTwoValueFilter { get; }
		public IEnumerable<string> FilterTypesDataSource { get { return FilterTypesTable.Keys; } }
		public VariantValue FirstValue { get { return firstValue; } protected set { firstValue = value; } }
		public VariantValue SecondValue { get { return secondValue; } protected set { secondValue = value; } }
		public bool FirstValueIsDate { get { return firstValueIsDate; } protected set { firstValueIsDate = value; } }
		public bool SecondValueIsDate { get { return secondValueIsDate; } protected set { secondValueIsDate = value; } }
		public WorkbookDataContext Context { get { return Control.InnerControl.DocumentModel.DataContext; } }
		public string FieldName { get; set; }
		public string FirstStringValue {
			get { return firstStringValue; }
			set {
				if (FirstStringValue == value)
					return;
				firstStringValue = value;
				OnPropertyChanged("FirstStringValue");
			}
		}
		public string SecondStringValue {
			get { return secondStringValue; }
			set {
				if (SecondStringValue == value)
					return;
				secondStringValue = value;
				OnPropertyChanged("SecondStringValue");
			}
		}
		public PivotFilterType FilterType {
			get { return FilterTypesTable[filterTypeValue]; }
			set {
				if (FilterType == value)
					return;
				filterTypeValue = GetFilterTypeValue(value);
				OnPropertyChanged("FilterTypeValue");
			}
		}
		public string FilterTypeValue {
			get { return filterTypeValue; }
			set {
				if (FilterTypeValue == value)
					return;
				filterTypeValue = value;
				OnPropertyChanged("FilterTypeValue");
				OnPropertyChanged("IsOneValueFilter");
				OnPropertyChanged("IsTwoValueFilter");
			}
		}
		#endregion
		void Initialize() {
			filterTypeValue = XtraSpreadsheetLocalizer.GetString(DefaultFilterTypeId);
			firstStringValue = String.Empty;
			secondStringValue = String.Empty;
			firstValueIsDate = false;
			secondValueIsDate = false;
		}
		string GetFilterTypeValue(PivotFilterType value) {
			foreach (string key in FilterTypesTable.Keys)
				if (FilterTypesTable[key] == value)
					return key;
			return String.Empty;
		}
		public abstract void ApplyChanges();
		public abstract bool Validate();
		protected bool ShowErrorMessage(IModelErrorInfo error) {
			Control.InnerControl.ErrorHandler.HandleError(error);
			return false;
		}
		protected bool ShowErrorMessage(ModelErrorType errorType) {
			return ShowErrorMessage(new ModelErrorInfo(errorType));
		}
	}
	#endregion
	#region PivotTableDateFiltersViewModel
	public class PivotTableDateFiltersViewModel : PivotTableFiltersViewModelBase {
		#region Static
		readonly static Dictionary<string, PivotFilterType> filterTypesTable = CreateFilterTypesTable();
		static Dictionary<string, PivotFilterType> CreateFilterTypesTable() {
			Dictionary<string, PivotFilterType> result = new Dictionary<string, PivotFilterType>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals), PivotFilterType.DateEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEqual), PivotFilterType.DateNotEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBefore), PivotFilterType.DateOlderThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeforeOrEqual), PivotFilterType.DateOlderThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorAfter), PivotFilterType.DateNewerThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorAfterOrEqual), PivotFilterType.DateNewerThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBetween), PivotFilterType.DateBetween);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNotBetween), PivotFilterType.DateNotBetween);
			return result;
		}
		#endregion
		#region Fields
		DateTime firstDate;
		DateTime secondDate;
		#endregion
		public PivotTableDateFiltersViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected override Dictionary<string, PivotFilterType> FilterTypesTable { get { return filterTypesTable; } }
		public override bool IsTwoValueFilter { get { return FilterType == PivotFilterType.DateBetween || FilterType == PivotFilterType.DateNotBetween; } }
		public DateTime FirstDate {
			get { return firstDate; }
			set {
				if (FirstDate == value)
					return;
				firstDate = value;
				OnPropertyChanged("FirstDate");
			}
		}
		public DateTime SecondDate {
			get { return secondDate; }
			set {
				if (SecondDate == value)
					return;
				secondDate = value;
				OnPropertyChanged("SecondDate");
			}
		}
		#endregion
		public override void ApplyChanges() {
			if (Command != null)
				(Command as PivotTableCustomFiltersCommandBase<PivotTableDateFiltersViewModel>).ApplyChanges(this);
		}
		public override bool Validate() {
			if (String.IsNullOrEmpty(FirstStringValue))
				return ShowErrorMessage(ModelErrorType.PivotFilterInvalidDate);
			VariantValue value1 = Context.ToDateTimeSerial(firstDate);
			if (IsTwoValueFilter) {
				if (String.IsNullOrEmpty(SecondStringValue))
					return ShowErrorMessage(ModelErrorType.PivotFilterInvalidDate);
				VariantValue value2 = Context.ToDateTimeSerial(secondDate);
				if (value1.NumericValue >= value2.NumericValue)
					return ShowErrorMessage(ModelErrorType.PivotFilterEndNumberMustBeGreaterThanStartNumber);
				SecondValue = value2;
				SecondValueIsDate = true;
			}
			FirstValue = value1;
			FirstValueIsDate = true;
			return true;
		}
	}
	#endregion
	#region PivotTableLabelFiltersViewModel
	public class PivotTableLabelFiltersViewModel : PivotTableFiltersViewModelBase {
		#region Static
		readonly static Dictionary<string, PivotFilterType> filterTypesTable = CreateFilterTypesTable();
		static Dictionary<string, PivotFilterType> CreateFilterTypesTable() {
			Dictionary<string, PivotFilterType> result = new Dictionary<string, PivotFilterType>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals), PivotFilterType.CaptionEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEqual), PivotFilterType.CaptionNotEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreater), PivotFilterType.CaptionGreaterThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreaterOrEqual), PivotFilterType.CaptionGreaterThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLess), PivotFilterType.CaptionLessThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLessOrEqual), PivotFilterType.CaptionLessThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeginsWith), PivotFilterType.CaptionBeginsWith);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotBeginWith), PivotFilterType.CaptionNotBeginsWith);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEndsWith), PivotFilterType.CaptionEndsWith);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEndWith), PivotFilterType.CaptionNotEndsWith);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorContains), PivotFilterType.CaptionContains);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotContain), PivotFilterType.CaptionNotContains);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBetween), PivotFilterType.CaptionBetween);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNotBetween), PivotFilterType.CaptionNotBetween);
			return result;
		}
		#endregion
		public PivotTableLabelFiltersViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected override Dictionary<string, PivotFilterType> FilterTypesTable { get { return filterTypesTable; } }
		public override bool IsTwoValueFilter { get { return FilterType == PivotFilterType.CaptionBetween || FilterType == PivotFilterType.CaptionNotBetween; } }
		public bool ContainsOnlyNumbers { get; set; }
		#endregion
		public override void ApplyChanges() {
			if (Command != null)
				(Command as PivotTableCustomFiltersCommandBase<PivotTableLabelFiltersViewModel>).ApplyChanges(this);
		}
		public override bool Validate() {
			if (ContainsOnlyNumbers) {
				bool isDate;
				FirstValue = ParseStringValue(FirstStringValue, out isDate);
				FirstValueIsDate = isDate;
				if (IsTwoValueFilter) {
					SecondValue = ParseStringValue(SecondStringValue, out isDate);
					SecondValueIsDate = isDate;
				}
			}
			else {
				FirstValue = FirstStringValue;
				if (IsTwoValueFilter)
					SecondValue = SecondStringValue;
			}
			return true;
		}
		VariantValue ParseStringValue(string stringValue, out bool isDate) {
			isDate = false;
			FormattedVariantValue dateValue = Context.TryConvertStringToDateTimeValue(stringValue, false);
			if (!dateValue.IsEmpty && dateValue.Value.IsNumeric) {
				isDate = true;
				return dateValue.Value;
			}
			VariantValue value = stringValue;
			value = value.ToNumeric(Context);
			return value.IsNumeric ? value : stringValue;
		}
	}
	#endregion
	#region PivotTableValueFiltersViewModelBase
	public abstract class PivotTableValueFiltersViewModelBase : PivotTableFiltersViewModelBase {
		#region Fields
		readonly List<string> dataFieldNames;
		string currentDataFieldName;
		#endregion
		protected PivotTableValueFiltersViewModelBase(ISpreadsheetControl control)
			: base(control) {
			this.dataFieldNames = PopulateDataFieldNames();
			this.currentDataFieldName = dataFieldNames[0];
		}
		#region Properties
		public List<string> DataFieldNames { get { return dataFieldNames; } }
		public int MeasureFieldIndex {
			get { return dataFieldNames.IndexOf(currentDataFieldName); }
			set {
				if (MeasureFieldIndex == value)
					return;
				currentDataFieldName = dataFieldNames[value];
				OnPropertyChanged("CurrentDataFieldName");
			}
		}
		public string CurrentDataFieldName {
			get { return currentDataFieldName; }
			set {
				if (CurrentDataFieldName == value)
					return;
				currentDataFieldName = value;
				OnPropertyChanged("CurrentDataFieldName");
			}
		}
		#endregion
		List<string> PopulateDataFieldNames() {
			List<string> result = new List<string>();
			Worksheet activeSheet = Control.InnerControl.DocumentModel.ActiveSheet;
			PivotTableStaticInfo info = activeSheet.PivotTableStaticInfo;
			PivotTable pivotTable = activeSheet.PivotTables[info.TableIndex];
			foreach (PivotDataField dataField in pivotTable.DataFields)
				result.Add(dataField.Name);
			return result;
		}
		protected bool ShowErrorMessageValueBetween(XtraSpreadsheetStringId numberTypeId, object minValue, object maxValue) {
			string numberTypeString = XtraSpreadsheetLocalizer.GetString(numberTypeId);
			ModelErrorInfoWithArgs error = new ModelErrorInfoWithArgs(ModelErrorType.PivotFilterValueMustBeBetween, new object[] { numberTypeString, minValue, maxValue });
			return ShowErrorMessage(error);
		}
	}
	#endregion
	#region PivotTableValueFiltersViewModel
	public class PivotTableValueFiltersViewModel : PivotTableValueFiltersViewModelBase {
		#region Static
		readonly static Dictionary<string, PivotFilterType> filterTypesTable = CreateFilterTypesTable();
		static Dictionary<string, PivotFilterType> CreateFilterTypesTable() {
			Dictionary<string, PivotFilterType> result = new Dictionary<string, PivotFilterType>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals), PivotFilterType.ValueEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEqual), PivotFilterType.ValueNotEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreater), PivotFilterType.ValueGreaterThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreaterOrEqual), PivotFilterType.ValueGreaterThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLess), PivotFilterType.ValueLessThan);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLessOrEqual), PivotFilterType.ValueLessThanOrEqual);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBetween), PivotFilterType.ValueBetween);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNotBetween), PivotFilterType.ValueNotBetween);
			return result;
		}
		#endregion
		public PivotTableValueFiltersViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected override Dictionary<string, PivotFilterType> FilterTypesTable { get { return filterTypesTable; } }
		public override bool IsTwoValueFilter { get { return FilterType == PivotFilterType.ValueBetween || FilterType == PivotFilterType.ValueNotBetween; } }
		#endregion
		public override void ApplyChanges() {
			if (Command != null)
				(Command as PivotTableCustomFiltersCommandBase<PivotTableValueFiltersViewModel>).ApplyChanges(this);
		}
		public override bool Validate() {
			return IsTwoValueFilter ? ValidateTwoValues() : ValidateOneValue();
		}
		bool ValidateOneValue() {
			VariantValue value = ToNumeric(FirstStringValue);
			if (!value.IsNumeric)
				return ShowErrorMessage(XtraSpreadsheetStringId.Msg_PivotFilterNumber);
			FirstValue = value;
			return true;
		}
		bool ValidateTwoValues() {
			VariantValue value1 = ToNumeric(FirstStringValue);
			VariantValue value2 = ToNumeric(SecondStringValue);
			if (!value1.IsNumeric || !value2.IsNumeric)
				return ShowErrorMessage(XtraSpreadsheetStringId.Msg_PivotFilterNumbers);
			if (value1.NumericValue >= value2.NumericValue)
				return ShowErrorMessage(ModelErrorType.PivotFilterEndNumberMustBeGreaterThanStartNumber);
			FirstValue = value1;
			SecondValue = value2;
			return true;
		}
		VariantValue ToNumeric(string stringValue) {
			VariantValue result = stringValue;
			return result.ToNumeric(Context);
		}
		bool ShowErrorMessage(XtraSpreadsheetStringId numberType) {
			return ShowErrorMessageValueBetween(numberType, "-1,7977E+308", "1,7977E+308");
		}
	}
	#endregion
	#region PivotTableTop10FiltersViewModel
	public class PivotTableTop10FiltersViewModel : PivotTableValueFiltersViewModelBase {
		#region Static
		readonly static Dictionary<string, PivotFilterType> filterTypesTable = CreateFilterTypesTable();
		static Dictionary<string, PivotFilterType> CreateFilterTypesTable() {
			Dictionary<string, PivotFilterType> result = new Dictionary<string, PivotFilterType>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterTypeItems), PivotFilterType.Count);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterTypePercent), PivotFilterType.Percent);
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterTypeSum), PivotFilterType.Sum);
			return result;
		}
		#endregion
		#region Fields
		readonly List<Top10OrderItem> topBottomList;
		bool filterByTop;
		double value;
		#endregion
		public PivotTableTop10FiltersViewModel(ISpreadsheetControl control)
			: base(control) {
			this.topBottomList = PopulateTopBottomList();
			this.value = 10;
			this.filterByTop = true;
		}
		#region Properties
		protected override Dictionary<string, PivotFilterType> FilterTypesTable { get { return filterTypesTable; } }
		protected override XtraSpreadsheetStringId DefaultFilterTypeId { get { return XtraSpreadsheetStringId.Caption_Top10FilterTypeItems; } }
		public IList<Top10OrderItem> TopBottomList { get { return topBottomList; } }
		public override bool IsTwoValueFilter { get { return false; } }
		public double Value {
			get { return value; }
			set {
				if (Value == value)
					return;
				this.value = value;
				OnPropertyChanged("Value");
			}
		}
		public bool FilterByTop { 
			get { return filterByTop; } 
			set {
				if (FilterByTop == value)
					return;
				filterByTop = value;
				OnPropertyChanged("FilterByTop");
			} 
		}
		#endregion
		public override void ApplyChanges() {
			if (Command != null)
				(Command as PivotTableCustomFiltersCommandBase<PivotTableTop10FiltersViewModel>).ApplyChanges(this);
		}
		public override bool Validate() {
			double minValue;
			double maxValue;
			XtraSpreadsheetStringId numberTypeId = XtraSpreadsheetStringId.Msg_PivotFilterNumber;
			if (FilterType == PivotFilterType.Percent) {
				minValue = 0;
				maxValue = 100;
			}
			else if (FilterType == PivotFilterType.Sum) {
				minValue = 0;
				maxValue = Double.MaxValue;
			}
			else {
				minValue = 1;
				maxValue = 2147483647;
				numberTypeId = XtraSpreadsheetStringId.Msg_PivotFilterInteger;
				if (value - Math.Truncate(value) != 0)
					return ShowErrorMessageValueBetween(numberTypeId, minValue, maxValue);
			}
			if (value < minValue || value > maxValue)
				return ShowErrorMessageValueBetween(numberTypeId, minValue, maxValue);
			FirstValue = value;
			return true;
		}
		List<Top10OrderItem> PopulateTopBottomList() {
			List<Top10OrderItem> result = new List<Top10OrderItem>();
			result.Add(new Top10OrderItem() { Value = true, Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterOrderTop) });
			result.Add(new Top10OrderItem() { Value = false, Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterOrderBottom) });
			return result;
		}
	}
	#endregion
}
