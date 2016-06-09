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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorkbookDataContext
	public class WorkbookDataContext : IDisposable {
		#region Fields
		internal const DateTimeStyles defaultFlags = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal | DateTimeStyles.NoCurrentDateDefault;
		readonly DocumentModel workbook;
		ExpressionParser expressionParser;
		ComplexNumberParser complexNumberParser;
		readonly DxStack<DefinedNameBase> definedNameProcessingStack;
		readonly DxStack<bool> relativeToCurrentCellStack;
		readonly DxStack<bool> arrayFormulaProcessingStack;
		readonly DxStack<bool> sharedFormulaProcessingStack;
		readonly DxStack<bool> useR1C1ReferenceStyleStack;
		readonly DxStack<bool> importExportModeStack;
		readonly DxStack<bool> setValueShouldAffectSharedFormulaStack;
		readonly DxStack<CellPositionOffset> arrayFormulaOffcetStack;
		readonly DxStack<ICurrentContextData> currentContextDatas;
		readonly DxStack<CurrentContextLimits> currentContextLimits;
		readonly DxStack<CultureInfo> cultureStack;
		readonly CalculationsInfo calculationsInfo;
		IRPNContext rpnContext;
		readonly DateTimeParanoicParser dateTimeParser = new DateTimeParanoicParser();
		#endregion
		public WorkbookDataContext(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
			expressionParser = new ExpressionParser(this);
			relativeToCurrentCellStack = new DxStack<bool>();
			relativeToCurrentCellStack.Push(false);
			definedNameProcessingStack = new DxStack<DefinedNameBase>();
			definedNameProcessingStack.Push(null);
			arrayFormulaProcessingStack = new DxStack<bool>();
			arrayFormulaProcessingStack.Push(false);
			sharedFormulaProcessingStack = new DxStack<bool>();
			sharedFormulaProcessingStack.Push(false);
			useR1C1ReferenceStyleStack = new DxStack<bool>();
			importExportModeStack = new DxStack<bool>();
			importExportModeStack.Push(false);
			setValueShouldAffectSharedFormulaStack = new DxStack<bool>();
			setValueShouldAffectSharedFormulaStack.Push(true);
			cultureStack = new DxStack<CultureInfo>();
			arrayFormulaOffcetStack = new DxStack<CellPositionOffset>();
			arrayFormulaOffcetStack.Push(CellPositionOffset.None);
			currentContextDatas = new DxStack<ICurrentContextData>();
			currentContextDatas.Push(new WorkbookInitialContextData(workbook));
			currentContextLimits = new DxStack<CurrentContextLimits>();
			currentContextLimits.Push(new CurrentContextLimits());
			this.calculationsInfo = new CalculationsInfo();
			this.rpnContext = new RPNContext(this);
		}
		public WorkbookDataContext(WorkbookDataContext sourceContext)
			: this(sourceContext.Workbook, sourceContext) {
		}
		public WorkbookDataContext(DocumentModel workbook, WorkbookDataContext sourceContext) {
			Guard.ArgumentNotNull(sourceContext, "context");
			this.workbook = workbook;
			this.expressionParser = sourceContext.expressionParser;
			this.rpnContext = new RPNContext(this);
			this.calculationsInfo = sourceContext.calculationsInfo;
			this.cultureStack = sourceContext.cultureStack.Clone();
			this.importExportModeStack = sourceContext.importExportModeStack.Clone();
			this.setValueShouldAffectSharedFormulaStack = sourceContext.setValueShouldAffectSharedFormulaStack.Clone();
			this.useR1C1ReferenceStyleStack = sourceContext.useR1C1ReferenceStyleStack.Clone();
			relativeToCurrentCellStack = sourceContext.relativeToCurrentCellStack.Clone();
			definedNameProcessingStack = sourceContext.definedNameProcessingStack.Clone();
			arrayFormulaProcessingStack = sourceContext.arrayFormulaProcessingStack.Clone();
			sharedFormulaProcessingStack = sourceContext.sharedFormulaProcessingStack.Clone();
			arrayFormulaOffcetStack = sourceContext.arrayFormulaOffcetStack.Clone();
			currentContextDatas = sourceContext.currentContextDatas.Clone();
			currentContextLimits = sourceContext.currentContextLimits.Clone();
		}
		#region Properties
		public CalculationChain CalculationChain { get { return workbook.CalculationChain; } }
		public CultureInfo Culture { get { return cultureStack.Count > 0 ? cultureStack.Peek() : Workbook.Culture; } }
		public CellPositionOffset ArrayFormulaOffcet { get { return arrayFormulaOffcetStack.Peek(); } }
		public DocumentModel Workbook { get { return workbook; } }
		public SharedStringTable StringTable { get { return workbook.SharedStringTable; } }
		public IModelWorkbook CurrentWorkbook { get { return currentContextDatas.Peek().Workbook; } }
		public IWorksheet CurrentWorksheet { get { return currentContextDatas.Peek().Worksheet; } }
		public int CurrentRowIndex { get { return currentContextDatas.Peek().RowIndex; } }  
		public int CurrentColumnIndex { get { return currentContextDatas.Peek().ColumnIndex; } }  
		public ICellBase CurrentCell { get { return CurrentWorksheet == null ? null : CurrentWorksheet.GetCell(CurrentColumnIndex, CurrentRowIndex); } }
		public ExpressionParser ExpressionParser { get { return expressionParser; } }
		public ComplexNumberParser ComplexNumberParser {
			get {
				if (complexNumberParser == null)
					complexNumberParser = new ComplexNumberParser(this);
				return complexNumberParser;
			}
		}
		public bool UseR1C1ReferenceStyle {
			get {
				return useR1C1ReferenceStyleStack.Count > 0 ? useR1C1ReferenceStyleStack.Peek()
					: workbook.Properties.UseR1C1ReferenceStyle;
			}
		}
		public bool RelativeToCurrentCell { get { return relativeToCurrentCellStack.Peek(); } }
		public bool ImportExportMode { get { return importExportModeStack.Peek(); } }
		public bool SetValueShouldAffectSharedFormula { get { return setValueShouldAffectSharedFormulaStack.Peek(); } }
		public bool DefinedNameProcessing { get { return definedNameProcessingStack.Peek() != null; } }
		public bool ArrayFormulaProcessing { get { return arrayFormulaProcessingStack.Peek(); } }
		public bool SharedFormulaProcessing { get { return sharedFormulaProcessingStack.Peek(); } }
		public DxStack<DefinedNameBase> DefinedNameProcessingStack { get { return definedNameProcessingStack; } }
		public DateSystem DateSystem { get { return workbook.Properties.CalculationOptions.DateSystem; } }
		public bool TransitionFormulaEvaluation {
			get {
				Worksheet sheet = CurrentWorksheet as Worksheet;
				return sheet != null ? sheet.Properties.TransitionOptions.TransitionFormulaEvaluation : false;
			}
		}
		public CalculationsInfo CalculationsInfo { get { return calculationsInfo; } }
		public IRPNContext RPNContext { get { return rpnContext; } }
		#endregion
		#region CurrentContextData setters
		public void PushCurrentCell(ICell cell) {
			currentContextDatas.Push(new CurrentContextData(currentContextDatas.Peek(), null, null, cell));
		}
		public void PushCurrentCell(int columnIndex, int rowIndex) {
			currentContextDatas.Push(new CurrentContextData(currentContextDatas.Peek(), columnIndex, rowIndex));
		}
		public void PushCurrentCell(CellPosition position) {
			currentContextDatas.Push(new CurrentContextData(currentContextDatas.Peek(), position.Column, position.Row));
		}
		public void PushCurrentWorksheet(IWorksheet worksheet) {
			currentContextDatas.Push(new CurrentContextData(currentContextDatas.Peek(), null, worksheet, null));
		}
		public void PushCurrentWorkbook(IModelWorkbook workbook) {
			PushCurrentWorkbook(workbook, true);
		}
		public void PushCurrentWorkbook(IModelWorkbook workbook, bool dependOnPrevious) {
			if (dependOnPrevious)
				currentContextDatas.Push(new CurrentContextData(currentContextDatas.Peek(), workbook, null, null));
			else
				currentContextDatas.Push(new CurrentContextData(null, workbook, null, null));
		}
		public void PopCurrentCell() {
			PopCurrentContextData();
		}
		public void PopCurrentWorksheet() {
			PopCurrentContextData();
		}
		public void PopCurrentWorkbook() {
			PopCurrentContextData();
		}
		public void PopCurrentContextData() {
			currentContextDatas.Pop();
		}
		#endregion
		#region Current context limits
		public void PushCurrentLimits(int maxColumnCount, int maxRowCount) {
			currentContextLimits.Push(new CurrentContextLimits(maxColumnCount, maxRowCount));
		}
		public void PopCurrentLimits() {
			currentContextLimits.Pop();
		}
		public int MaxColumnCount { get { return currentContextLimits.Peek().MaxColumnCount; } }
		public int MaxRowCount { get { return currentContextLimits.Peek().MaxRowCount; } }
		#endregion
		#region Formula settings override
		protected internal void SetImportExportSettings() {
			SetImportExportSettings(CultureInfo.InvariantCulture, false);
		}
		protected internal void SetImportExportSettings(CultureInfo culture, bool useR1C1) {
			PushUseR1C1(useR1C1);
			PushCulture(culture);
			PushImportExportMode(true);
		}
		protected internal void SetWorkbookDefinedSettings() {
			PopUseR1C1();
			PopCulture();
			PopImportExportMode();
		}
		protected internal void PushImportExportMode(bool isImportExportMode) {
			importExportModeStack.Push(isImportExportMode);
		}
		protected internal void PopImportExportMode() {
			importExportModeStack.Pop();
		}
		protected internal void PushSetValueShouldAffectSharedFormula(bool setValueShouldAffectSharedFormula) {
			setValueShouldAffectSharedFormulaStack.Push(setValueShouldAffectSharedFormula);
		}
		protected internal void PopSetValueShouldAffectSharedFormula() {
			setValueShouldAffectSharedFormulaStack.Pop();
		}
		#region UseR1C1
		protected internal void PushUseR1C1(bool value) {
			useR1C1ReferenceStyleStack.Push(value);
		}
		protected internal void PopUseR1C1() {
			useR1C1ReferenceStyleStack.Pop();
		}
		#endregion
		#region Culture
		protected internal void PushCulture(CultureInfo value) {
			cultureStack.Push(value);
		}
		protected internal void PopCulture() {
			cultureStack.Pop();
		}
		#endregion
		#region ArrayFromulaOffcet
		public void PushArrayFormulaOffcet(CellPositionOffset arrayFormulaOffset) {
			this.arrayFormulaOffcetStack.Push(arrayFormulaOffset);
		}
		public void PopArrayFromulaOffcet() {
			this.arrayFormulaOffcetStack.Pop();
		}
		#endregion
		#region RelativeToCurrentCell
		public void PushRelativeToCurrentCell(bool value) {
			this.relativeToCurrentCellStack.Push(value);
		}
		public void PopRelativeToCurrentCell() {
			this.relativeToCurrentCellStack.Pop();
		}
		#endregion
		#region DefinedNameProcessing
		public void PushDefinedNameProcessing(DefinedNameBase definedName) {
			this.definedNameProcessingStack.Push(definedName);
		}
		public void PopDefinedNameProcessing() {
			this.definedNameProcessingStack.Pop();
		}
		#endregion
		#region ArrayFormulaProcessing
		public void PushArrayFormulaProcessing(bool value) {
			this.arrayFormulaProcessingStack.Push(value);
		}
		public void PopArrayFormulaProcessing() {
			this.arrayFormulaProcessingStack.Pop();
		}
		#endregion
		#region SharedFormulaProcessing
		public void PushSharedFormulaProcessing(bool value) {
			this.sharedFormulaProcessingStack.Push(true);
		}
		public void PopSharedFormulaProcessing() {
			this.sharedFormulaProcessingStack.Pop();
		}
		#endregion
		#endregion
		#region Parser
		public ParsedExpression ParseExpression(string expressionString, OperandDataType highLevelDataType, bool allowsToCreateExternal) {
			if (string.IsNullOrEmpty(expressionString))
				return null;
			if (expressionString[0] != '=')
				expressionString = expressionString.Insert(0, "=");
			if (expressionString.Length <= 1)
				return null;
			ParsedExpression expression = expressionParser.Parse(expressionString, highLevelDataType, allowsToCreateExternal);
			return expression;
		}
		public bool TryParseBoolean(string value, out bool result) {
			result = false;
			if (StringExtensions.CompareInvariantCultureIgnoreCase(value, VariantValue.TrueConstant) == 0) {
				result = true;
				return true;
			}
			if (StringExtensions.CompareInvariantCultureIgnoreCase(value, VariantValue.FalseConstant) == 0) {
				result = false;
				return true;
			}
			return false;
		}
		public static bool TryParseBooleanInvariant(string value, out bool result) {
			result = false;
			if (StringExtensions.CompareInvariantCultureIgnoreCase(value, VariantValue.TrueConstant) == 0) {
				result = true;
				return true;
			}
			if (StringExtensions.CompareInvariantCultureIgnoreCase(value, VariantValue.FalseConstant) == 0) {
				result = false;
				return true;
			}
			return false;
		}
		public static bool StartsFromCellPosition(string value) {
			CellPosition position = CellReferenceParser.TryParse(value);
			if (position.IsValid)
				return true;
			return CellReferenceParser.CompareStringWithRCRef(value);
		}
		public static bool IsIdent(string value) {
			if (!IsValidIndentifier(value))
				return false;
			return !StartsFromCellPosition(value);
		}
		public static bool IsWideIdent(string value) {
			if (string.IsNullOrEmpty(value))
				return true;
			if (!char.IsLetter(value[0]) && value[0] != '_')
				return false;
			for (int i = 1; i < value.Length; i++) {
				char curChar = value[i];
				if (!char.IsLetterOrDigit(curChar) && curChar != '_' && curChar != '.')
					return false;
			}
			return true;
		}
		public static bool IsValidIndentifier(string value) {
			if (String.IsNullOrEmpty(value))
				return false;
			for (int i = 0; i < value.Length; i++)
				if (!IsValidIndentifierChar(value[i], i, value))
					return false;
			return true;
		}
		public static bool IsValidIndentifierChar(char curChar, int index, string value) {
			if (index == 0) {
				if (curChar == '\\') {
					if (value.Length == 1)
						return true;
					if (value[1] != '\\' && value[1] != '.' && value[1] != '?' && value[1] != '_')
						return false;
				}
				else
					if (!char.IsLetter(curChar) && curChar != '_')
						return false;
			}
			return !(!char.IsLetterOrDigit(curChar) && curChar != '_' && curChar != '.' && curChar != '\\' && curChar != '?');
		}
		#endregion
		public int Compare(VariantValue x, VariantValue y) {
			if (x.IsEmpty && y.IsEmpty)
				return 0;
			if (x.IsEmpty) {
				if (y.IsText && TransitionFormulaEvaluation)
					x = 0;
				else
					x.ChangeType(y.Type);
			}
			if (y.IsEmpty) {
				if (x.IsText && TransitionFormulaEvaluation)
					y = 0;
				else
					y.ChangeType(x.Type);
			}
			VariantValueType sortType = x.InversedSortType;
			int result = Comparer<int>.Default.Compare(VariantValueType.Numeric - sortType, VariantValueType.Numeric - y.InversedSortType);
			if (result != 0) {
				if (!TransitionFormulaEvaluation)
					return result;
				if (x.IsText && (y.IsNumeric || y.IsBoolean)) {
					x = 0;
					sortType = y.InversedSortType;
				}
				else if (y.IsText && (x.IsNumeric || x.IsBoolean)) {
					y = 0;
				}
			}
			if (sortType == VariantValueType.Numeric || sortType == VariantValueType.Boolean)
				return DoubleComparer.Compare(x.NumericValue, y.NumericValue);
			if (sortType == VariantValueType.InlineText)
				return String.Compare(x.GetTextValue(StringTable), y.GetTextValue(StringTable), StringComparison.CurrentCultureIgnoreCase);
			if (sortType == VariantValueType.Error)
				return String.Compare(x.ErrorValue.Name, y.ErrorValue.Name, StringComparison.CurrentCultureIgnoreCase);
			return 0;
		}
		public bool AreEqual(VariantValue x, VariantValue y) {
			if (x.IsEmpty && y.IsEmpty)
				return true;
			if (x.IsEmpty) {
				if (y.IsText && TransitionFormulaEvaluation)
					x = 0;
				else
					x.ChangeType(y.Type);
			}
			if (y.IsEmpty) {
				if (x.IsText && TransitionFormulaEvaluation)
					y = 0;
				else
					y.ChangeType(x.Type);
			}
			VariantValueType sortType = x.InversedSortType;
			int result = Comparer<int>.Default.Compare(VariantValueType.Numeric - sortType, VariantValueType.Numeric - y.InversedSortType);
			if (result != 0) {
				if (!TransitionFormulaEvaluation)
					return result == 0;
				if (x.IsText && (y.IsNumeric || y.IsBoolean)) {
					x = 0;
					sortType = y.InversedSortType;
				}
				else if (y.IsText && (x.IsNumeric || x.IsBoolean)) {
					y = 0;
				}
			}
			if (sortType == VariantValueType.Numeric || sortType == VariantValueType.Boolean)
				return DevExpress.XtraSpreadsheet.Utils.DoubleComparer.AreEqual(x.NumericValue, y.NumericValue);
			if (sortType == VariantValueType.InlineText)
				return String.Compare(x.GetTextValue(StringTable), y.GetTextValue(StringTable), StringComparison.CurrentCultureIgnoreCase) == 0;
			if (sortType == VariantValueType.Error)
				return String.Compare(x.ErrorValue.Name, y.ErrorValue.Name, StringComparison.CurrentCultureIgnoreCase) == 0;
			return true;
		}
		#region DefinedNames
		public virtual DefinedNameBase GetDefinedName(string name, SheetDefinition sheetDefinition) {
			DefinedNameBase definedExpression = null;
			if (sheetDefinition != null) {
				if (!string.IsNullOrEmpty(sheetDefinition.SheetNameStart)) {
					IWorksheet sheet = sheetDefinition.GetSheetStart(this);
					definedExpression = GetDefinedNameForWorksheet(name, sheet);
				}
				else {
					IModelWorkbook workbook = sheetDefinition.GetWorkbook(this);
					definedExpression = GetDefinedNameForWorkbook(name, workbook);
				}
			}
			else
				definedExpression = GetDefinedNameForWorksheet(name, CurrentWorksheet);
			return definedExpression;
		}
		public virtual DefinedNameBase GetDefinedName(string name) {
			if (CurrentWorksheet != null)
				return GetDefinedNameForWorksheet(name, CurrentWorksheet);
			return GetDefinedNameForWorkbook(name, CurrentWorkbook);
		}
		public DefinedNameBase GetDefinedNameForWorkbook(string name, IModelWorkbook workbook) {
			if (workbook == null)
				return null;
			DefinedNameBase result;
			workbook.DefinedNames.TryGetItemByName(name, out result);
			return result;
		}
		public DefinedNameBase GetDefinedNameForWorksheet(string name, IWorksheet worksheet) {
			if (worksheet == null)
				return null;
			DefinedNameBase result = null;
			if (worksheet.DefinedNames.TryGetItemByName(name, out result))
				return result;
			return GetDefinedNameForWorkbook(name, worksheet.Workbook);
		}
		#endregion
		#region Tables
		public virtual Table GetDefinedTableRange(string name) {
			Guard.ArgumentIsNotNullOrEmpty(name, "Table name");
			IModelWorkbook currentWorkbook = CurrentWorkbook;
			if (currentWorkbook == null)
				return null;
			return currentWorkbook.GetTableByName(name);
		}
		public Table GetCurrentTable() {
			if (CurrentWorksheet == null)
				return null;
			return CurrentWorksheet.GetTableByCellPosition(CurrentColumnIndex, CurrentRowIndex);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			expressionParser.Dispose();
			currentContextDatas.Clear();
		}
		#endregion
		#region DateTime helpers
		public const double MaxDateTimeSerialNumber
			= 2958465.999999990;  
		public const double MaxDateTimeSerialNumber1904
			= 2957003.999999990;
		public static bool IsErrorDateTimeSerial(double serialNumber, DateSystem dateSystem) {
			if (dateSystem == DateSystem.Date1900)
				return (serialNumber < 0 || serialNumber > MaxDateTimeSerialNumber);
			else
				return (serialNumber < 0 || serialNumber > MaxDateTimeSerialNumber1904);
		}
		public DateTime FromDateTimeSerial(double value) {
			return FromDateTimeSerial(value, DateSystem);
		}
		public static DateTime FromDateTimeSerial(double value, DateSystem dateSystem) {
			if (dateSystem == DateSystem.Date1904)
				return VariantValue.BaseDate1904 + TimeSpan.FromDays(value);
			else {
				if (value > 60)
					return VariantValue.BaseDate + TimeSpan.FromDays(value);
				else
					return VariantValue.BaseDate + TimeSpan.FromDays(value + 1);
			}
		}
		bool ShouldUseMonthYearFormat(string textValue, DateTime dateTime) {
			DateTimeFormatInfo dateTimeFormat = Culture.DateTimeFormat;
			int month = dateTime.Month - 1;
			string longMonth = dateTimeFormat.MonthNames[month];
			string shortMonth = dateTimeFormat.AbbreviatedMonthNames[month];
			if (textValue.IndexOfInvariantCultureIgnoreCase(shortMonth) < 0 && textValue.IndexOfInvariantCultureIgnoreCase(longMonth) < 0)
				return false;
			int index = textValue.IndexOf(longMonth);
			if (index >= 0)
				textValue = textValue.Remove(index, longMonth.Length);
			else {
				index = textValue.IndexOf(shortMonth);
				if (index >= 0)
					textValue = textValue.Remove(index, shortMonth.Length);
			}
			string longYear = dateTime.Year.ToString();
			string shortYear = (dateTime.Year - dateTime.Year / 100 * 100).ToString();
			if (!textValue.Contains(longYear) && !textValue.Contains(shortYear))
				return false;
			index = textValue.IndexOf(longYear);
			if (index >= 0)
				textValue = textValue.Remove(index, longYear.Length);
			else {
				index = textValue.IndexOf(shortYear);
				if (index >= 0)
					textValue = textValue.Remove(index, shortYear.Length);
			}
			return !textValue.Contains(dateTime.Day.ToString("00")) && !textValue.Contains(dateTime.Day.ToString("#0"));
		}
		bool ShouldUseShortDateFormat(string textValue, DateTime dateTime) {
			return textValue.Contains(dateTime.Year.ToString());
		}
		internal FormattedVariantValue TryConvertStringToDateTimeValue(string textValue, bool calculateFormat) {
			string separator;
			if (IsShortNumericDate(textValue, out separator)) {
				string[] shortDatePatterns = Culture.GetAllDateTimePatterns('d');
				if (!IsValidSeparator(shortDatePatterns, separator))
					return TryParseParanoic(textValue);
			}
			return TryConvertStringToDateTimeValueCore(textValue, calculateFormat);
		}
		bool IsShortNumericDate(string textValue, out string separator) {
			char[] digits = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			char[] separators = new char[3] { ',', '-', '.' };
			separator = Culture.GetDateSeparator();
			string trimmedValue = textValue.Trim();
			int count = trimmedValue.Length;
			for (int i = 0; i < count; i++) {
				char current = trimmedValue[i];
				bool isDigit = Array.IndexOf(digits, current) >= 0;
				bool isSeparator = Array.IndexOf(separators, current) >= 0;
				if (!isDigit && !isSeparator)
					return false;
				if (isSeparator)
					separator = current.ToString();
			}
			return true;
		}
		bool IsValidSeparator(string[] datePatterns, string separator) {
			foreach (string pattern in datePatterns)
				if (pattern.IndexOf(separator) > 0)
					return true;
			return false;
		}
		FormattedVariantValue TryConvertStringToDateTimeValueCore(string textValue, bool calculateFormat) {
			DateTime dateTime1;
			DateTime dateTime2;
			bool dateTime1Parsed = DateTimeParanoicParser.DateTimeTryParse(textValue, Culture, defaultFlags, out dateTime1);
			bool dateTime2Parsed = DateTimeUtils.TryParseByParts(textValue, Culture, out dateTime2);
			if (dateTime1Parsed || dateTime2Parsed) {
				DateTime dateTime;
				bool validateDesignator;
				if (dateTime1Parsed && dateTime2Parsed) {
					dateTime = new DateTime(Math.Max(dateTime1.Ticks, dateTime2.Ticks));
					validateDesignator = dateTime1 > dateTime2;
				}
				else if (dateTime1Parsed) {
					dateTime = dateTime1;
					validateDesignator = true;
				}
				else {
					dateTime = dateTime2;
					validateDesignator = false;
				}
				if (validateDesignator && !DateTimeUtils.ValidateDesignator(textValue, dateTime, Culture))
					return new FormattedVariantValue(VariantValue.ErrorInvalidValueInFunction, 0);
				int timeFormat = -1;
				DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				if (date == DateTime.MinValue) {
					timeFormat = CalculateTimeFormatId(textValue, dateTime);
					if (timeFormat <= 0)
						return new FormattedVariantValue(VariantValue.ErrorInvalidValueInFunction, 0);
				}
				if (calculateFormat) {
					int formatIndex = timeFormat >= 0 ? timeFormat : CalculateDateTimeFormatId(textValue, dateTime);
					return new FormattedVariantValue(FromDateTime(dateTime), formatIndex);
				}
				else
					return new FormattedVariantValue(FromDateTime(dateTime), 0);
			}
			return TryParseParanoic(textValue);
		}
		FormattedVariantValue TryParseParanoic(string textValue) {
			DateTimeInfo result = new DateTimeInfo();
			if (dateTimeParser.TryParse(textValue, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, this, out result)) {
				VariantValue value = TryConvertStringToDateTimeValueCore(result.Value);
				string format = result.Format;
				if (IsTimeOnlyFormat(format) && value.IsNumeric && value.NumericValue >= 1)
					format = "[h]:mm:ss";
				return new FormattedVariantValue(value, workbook.Cache.NumberFormatCache.GetItemIndex(new NumberFormat(format)));
			}
			return FormattedVariantValue.Empty;
		}
		bool IsTimeOnlyFormat(string format) {
			return format == "h:mm:ss" || format == "hh:mm:ss" || format == "h:mm" || format == "hh:mm";
		}
		int CalculateDateTimeFormatId(string textValue, DateTime dateTime) {
			DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
			if (date == DateTime.MinValue) 
				return CalculateTimeFormatId(textValue, dateTime);
			else if (dateTime == date)
				return CalculateDateFormatId(textValue, dateTime);
			else
				return 22; 
		}
		int CalculateDateFormatId(string textValue, DateTime dateTime) {
			if (ShouldUseMonthYearFormat(textValue, dateTime))
				return 17; 
			else if (ShouldUseShortDateFormat(textValue, dateTime))
				return 14; 
			else
				return 16; 
		}
		int CalculateTimeFormatId(string textValue, DateTime dateTime) {
			int timeDesignatorIndex = CalculateTimeDesignatorIndex(textValue);
			int hourIndex = CalculateHourIndex(textValue, dateTime.Hour, timeDesignatorIndex > 0);
			if (hourIndex < 0)
				return 0;
			int minuteIndex = IndexOf(textValue, hourIndex, dateTime.Minute.ToString());
			if (minuteIndex < 0)
				return timeDesignatorIndex < 0 ? 0 : 18;
			int secondIndex = IndexOf(textValue, minuteIndex, dateTime.Second.ToString());
			if (timeDesignatorIndex <= 0) { 
				if (secondIndex > minuteIndex)
					return 21;
				else
					return 20;
			}
			else { 
				if (secondIndex > minuteIndex)
					return 19;
				else
					return 18;
			}
		}
		int CalculateHourIndex(string textValue, int hour, bool hasTimeDesignator) {
			if (hasTimeDesignator) {
				if (hour >= 13)
					hour -= 12;
				int index = IndexOf(textValue, 0, hour.ToString());
				if (index >= 0)
					return index;
				if (hour == 0) {
					hour = 12;
					index = IndexOf(textValue, 0, hour.ToString());
					if (index >= 0)
						return index;
				}
				else if (hour == 12) {
					hour = 0;
					index = IndexOf(textValue, 0, hour.ToString());
					if (index >= 0)
						return index;
				}
				return IndexOf(textValue, 0, hour.ToString());
			}
			else {
				int index = IndexOf(textValue, 0, hour.ToString("0"));
				if (index >= 0)
					return index;
				return IndexOf(textValue, 0, hour.ToString("00"));
			}
		}
		int IndexOf(string where, int from, string what) {
			int result = where.IndexOfInvariantCultureIgnoreCase(what, from);
			if (result < 0)
				return result;
			return result + what.Length;
		}
		int CalculateTimeDesignatorIndex(string textValue) {
			DateTimeFormatInfo dateTimeFormat = Culture.DateTimeFormat;
			int index = -1;
			if (!String.IsNullOrEmpty(dateTimeFormat.AMDesignator))
				index = Math.Max(index, IndexOf(textValue, 0, dateTimeFormat.AMDesignator));
			if (!String.IsNullOrEmpty(dateTimeFormat.PMDesignator))
				index = Math.Max(index, IndexOf(textValue, 0, dateTimeFormat.PMDesignator));
			index = Math.Max(index, IndexOf(textValue, 0, "AM"));
			index = Math.Max(index, IndexOf(textValue, 0, "PM"));
			return index;
		}
		VariantValue TryConvertStringToDateTimeValueCore(DateTime dateTime) {
			bool isSmallerThen29Feb = false;
			if (DateSystem == DateSystem.Date1900 && dateTime.Date < VariantValue.BaseDate) {
				TimeSpan dateTimeTicks = TimeSpan.FromTicks(dateTime.Ticks);
				isSmallerThen29Feb = dateTimeTicks < VariantValue.Day29Feb1900;
				dateTime = VariantValue.BaseDate + dateTimeTicks;
				if (dateTimeTicks.Ticks > 0 && dateTime.Date < VariantValue.BaseDate.AddDays(1))
					dateTime = dateTime.AddDays(1);
			}
			if (DateSystem == DateSystem.Date1904 && dateTime.Date < VariantValue.BaseDate1904)
				dateTime = VariantValue.BaseDate1904 + TimeSpan.FromTicks(dateTime.Ticks);
			VariantValue resultValue = FromDateTime(dateTime);
			if (isSmallerThen29Feb && resultValue.IsNumeric)
				resultValue = resultValue.NumericValue + 1;
			return resultValue;
		}
		public double ToDateTimeSerialDouble(DateTime value) {
			return ToDateTimeSerialDouble(value, DateSystem);
		}
		public static double ToDateTimeSerialDouble(DateTime value, DateSystem dateSystem) {
			if (dateSystem == DateSystem.Date1904) {
				TimeSpan difference = value - VariantValue.BaseDate1904;
				return difference.TotalDays;
			}
			else {
				TimeSpan difference = value - VariantValue.BaseDate;
				if (difference > VariantValue.Day29Feb1900)
					return difference.TotalDays;
				else
					return difference.TotalDays - 1;
			}
		}
		public VariantValue ToDateTimeSerial(DateTime value) {
			if (DateSystem == DateSystem.Date1904) {
				TimeSpan difference = value - VariantValue.BaseDate1904;
				if (difference < TimeSpan.Zero)
					return VariantValue.ErrorNumber;
				else
					return difference.TotalDays;
			}
			else {
				TimeSpan difference = value - VariantValue.BaseDate;
				if (difference <= TimeSpan.Zero)
					return VariantValue.ErrorNumber;
				if (difference > VariantValue.Day29Feb1900)
					return difference.TotalDays;
				else
					return difference.TotalDays - 1;
			}
		}
		internal DateTime FromDateTimeSerialForDayOfWeek(double value) {
			if (DateSystem == DateSystem.Date1904)
				return VariantValue.BaseDate1904 + TimeSpan.FromDays(value);
			else
				return VariantValue.BaseDate + TimeSpan.FromDays(value);
		}
		internal DateTime FromDateTimeSerialForMonthName(double value) {
			if (DateSystem == DateSystem.Date1904)
				return VariantValue.BaseDate1904 + TimeSpan.FromDays(value);
			else {
				if (value < 2)
					return VariantValue.BaseDate.AddDays(2);
				if (value >= 60 && value < 61)
					return VariantValue.BaseDate.AddDays(59); 
				if (value < 61)
					return VariantValue.BaseDate + TimeSpan.FromDays(value + 1);
				else
					return VariantValue.BaseDate + TimeSpan.FromDays(value);
			}
		}
		public VariantValue FromDateTime(DateTime value) {
			VariantValue result = new VariantValue();
			result.SetDateTime(value, this);
			return result;
		}
#endregion
#region FindCrossing
		public VariantValue GetArrayFromCellRange(CellRangeBase cellRange) {
			if (cellRange.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			if (cellRange.CellCount == 1)
				return cellRange.GetFirstCellValue();
			return VariantValue.FromArray(new RangeVariantArray((CellRange)cellRange));
		}
		public VariantValue DereferenceValue(VariantValue value, bool dereferenceEmptyValueAsZero) {
			if (value.IsArray )
				value = ArrayFormulaProcessing ? value.ArrayValue.GetValue(ArrayFormulaOffcet.RowOffset, ArrayFormulaOffcet.ColumnOffset) : value.ArrayValue[0];
			if (value.IsCellRange) {
				CellRangeBase cellRange = value.CellRangeValue;
				if (cellRange.CellCount == 1) {
					if (cellRange.Worksheet == null)
						return VariantValue.ErrorReference;
					value = cellRange.GetFirstCellValue();
				}
				else {
					if (cellRange.Worksheet == null)
						return ArrayFormulaProcessing ? VariantValue.ErrorReference : VariantValue.ErrorInvalidValueInFunction;
					value = ArrayFormulaProcessing ? cellRange.GetCellValueRelative(ArrayFormulaOffcet.ColumnOffset, ArrayFormulaOffcet.RowOffset) : FindCrossing(cellRange);
				}
			}
			if (dereferenceEmptyValueAsZero && value.IsEmpty)
				value = 0;
			return value;
		}
		public CellPosition DereferenceToCellPosition(CellRangeBase range) {
			if (range.CellCount == 1)
				return range.TopLeft;
			return FindCrossingCellPosition(range);
		}
		internal CellPosition FindCrossingCellPosition(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange)
				return CellPosition.InvalidValue;
			CellRange cellRange = range.GetFirstInnerCellRange();
			CellPosition topLeft = cellRange.TopLeft;
			int rowIndex;
			int columnIndex;
			if ((cellRange.Width > 1 && cellRange.Height > 1)) {
				if (RowIndexOutOfRange(cellRange, CurrentRowIndex))
					return CellPosition.InvalidValue;
				if (ColumnIndexOutOfRange(cellRange, CurrentColumnIndex))
					return CellPosition.InvalidValue;
				rowIndex = CurrentRowIndex;
				columnIndex = CurrentColumnIndex;
				return new CellPosition(columnIndex, rowIndex);
			}
			if (cellRange.Width == 1) {
				if (RowIndexOutOfRange(cellRange, CurrentRowIndex))
					return CellPosition.InvalidValue;
				rowIndex = CurrentRowIndex;
				columnIndex = topLeft.Column;
			}
			else {
				if (ColumnIndexOutOfRange(cellRange, CurrentColumnIndex))
					return CellPosition.InvalidValue;
				rowIndex = topLeft.Row;
				columnIndex = CurrentColumnIndex;
			}
			return new CellPosition(columnIndex, rowIndex);
		}
		internal VariantValue FindCrossing(CellRangeBase range) {
			CellPosition crossingPosition = FindCrossingCellPosition(range);
			if (!crossingPosition.IsValid)
				return VariantValue.ErrorInvalidValueInFunction;
			CellRange cellRange = range.GetFirstInnerCellRange();
			return cellRange.Worksheet.GetCalculatedCellValue(crossingPosition.Column, crossingPosition.Row);
		}
		bool RowIndexOutOfRange(CellRange cellRange, int currentRowIndex) {
			CellPosition topLeft = cellRange.TopLeft;
			int bottomRightRow = topLeft.Row + cellRange.Height - 1;
			return currentRowIndex < topLeft.Row || currentRowIndex > bottomRightRow;
		}
		bool ColumnIndexOutOfRange(CellRange cellRange, int currentColumnIndex) {
			CellPosition topLeft = cellRange.TopLeft;
			int bottomRightRow = topLeft.Column + cellRange.Width - 1;
			return currentColumnIndex < topLeft.Column || currentColumnIndex > bottomRightRow;
		}
		protected internal VariantValue DereferenceWithoutCrossing(VariantValue value) {
			if (value.IsCellRange && value.CellRangeValue.CellCount > 1)
				return VariantValue.ErrorInvalidValueInFunction;
			value = DereferenceValue(value, true);
			if (value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			return value;
		}
		protected internal VariantValue GetTextComplexValue(VariantValue value) {
			return DereferenceWithoutCrossing(value).ToText(this);
		}
		protected internal VariantValue ToNumericWithoutCrossingCore(VariantValue value) {
			return DereferenceWithoutCrossing(value).ToNumeric(this);
		}
		protected internal VariantValue ToNumericWithoutCrossing(VariantValue value) {
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			return ToNumericWithoutCrossingCore(value);
		}
#endregion
#region Culture sensitive converters
		public string ConvertNumberToText(double value) {
			string result = value.ToString(Culture);
			if (value > 1e+15 && value < 1e+16) {
				try {
					double parsedValue = double.Parse(result, Culture);
					long intValue = (long)parsedValue;
					if (parsedValue == intValue) {
						string integerText = intValue.ToString(Culture);
						if (integerText.Length < result.Length)
							return integerText;
					}
				}
				catch {
				}
			}
			return result;
		}
		public char GetListSeparator() {
			return Culture.TextInfo.ListSeparator[0];
		}
		public char GetDecimalSymbol() {
			return Culture.NumberFormat.NumberDecimalSeparator[0];
		}
#endregion
		public WorkbookDataContext CreateSnapshot() {
			return new WorkbookDataContext(this);
		}
#region SheetDefinition
		internal SheetDefinition GetSheetDefinition(int index) {
			return rpnContext.GetSheetDefinition(index);
		}
		protected internal int RegisterSheetDefinition(SheetDefinition sheetDefinition) {
			return rpnContext.IndexOfSheetDefinition(sheetDefinition);
		}
#endregion
		public void CopyFrom(WorkbookDataContext sourceContext) {
		}
		internal VariantValue ConvertTextToNumericWithCaching(string textValue) {
			return workbook.CalculationChain.CalculationHash.ConvertTextToNumeric(textValue, Culture);
		}
		internal VariantValue ConvertTextToVariantValueWithCaching(string textValue) {
			return workbook.CalculationChain.CalculationHash.ConvertTextToVariantValue(textValue, Culture);
		}
	}
#endregion
#region CurrentContextData
#region ICurrentContextData
	interface ICurrentContextData {
		IModelWorkbook Workbook { get; }
		IWorksheet Worksheet { get; }
		int ColumnIndex { get; }
		int RowIndex { get; }
	}
#endregion
#region WorkbookInitialContextData
	class WorkbookInitialContextData : ICurrentContextData {
		DocumentModel workbook;
		public WorkbookInitialContextData(DocumentModel workbook) {
			this.workbook = workbook;
		}
		public IModelWorkbook Workbook { get { return workbook; } }
		public IWorksheet Worksheet { get { return workbook.ActiveSheet; } }
		public int ColumnIndex { get { return 0; } }
		public int RowIndex { get { return 0; } }
	}
#endregion
#region CurrentContextData
	class CurrentContextData : ICurrentContextData {
#region Fields
		readonly IModelWorkbook workbook;
		readonly IWorksheet worksheet;
		readonly int columnIndex;
		readonly int rowIndex;
#endregion
		public CurrentContextData(ICurrentContextData previousData, IModelWorkbook workbook, IWorksheet worksheet, ICell cell) {
			this.workbook = workbook;
			this.worksheet = worksheet;
			if (cell != null) {
				columnIndex = cell.ColumnIndex;
				rowIndex = cell.RowIndex;
			}
			else {
				columnIndex = 0;
				rowIndex = 0;
			}
			if (cell != null && worksheet == null)
				this.worksheet = cell.Sheet;
			if (this.worksheet != null && this.workbook == null)
				this.workbook = this.worksheet.Workbook;
			if (previousData != null) {
				if (object.ReferenceEquals(previousData.Workbook, this.workbook)) {
					if (this.worksheet == null) {
						this.worksheet = previousData.Worksheet;
						this.columnIndex = previousData.ColumnIndex;
						this.rowIndex = previousData.RowIndex;
					}
					else if (cell == null) {
						this.columnIndex = previousData.ColumnIndex;
						this.rowIndex = previousData.RowIndex;
					}
				}
			}
		}
		public CurrentContextData(ICurrentContextData previousData, int columnIndex, int rowIndex) {
			this.workbook = previousData.Workbook;
			this.worksheet = previousData.Worksheet;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
		}
#region Properties
		public IModelWorkbook Workbook { get { return workbook; } }
		public IWorksheet Worksheet { get { return worksheet; } }
		public int ColumnIndex { get { return columnIndex; } }
		public int RowIndex { get { return rowIndex; } }
#endregion
	}
#endregion
#region CurrentContextLimits
	class CurrentContextLimits {
#region Fields
		int maxColumnCount;
		int maxRowCount;
#endregion
		public CurrentContextLimits() {
			this.maxColumnCount = IndicesChecker.MaxColumnCount;
			this.maxRowCount = IndicesChecker.MaxRowCount;
		}
		public CurrentContextLimits(int maxColumnCount, int maxRowCount) {
			this.maxColumnCount = maxColumnCount;
			this.maxRowCount = maxRowCount;
		}
		public int MaxColumnCount { get { return maxColumnCount; } }
		public int MaxRowCount { get { return maxRowCount; } }
	}
#endregion
#endregion
}
