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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataReferenceType
	public enum DataReferenceType {
		None,
		Number,
		NumberLiteral,
		String,
		StringLiteral,
		MultiLevelString,
	}
	#endregion
	#region DataReferenceValueType
	public enum DataReferenceValueType {
		Number,
		String,
		DateTime,
	}
	#endregion
	#region IDataReference
	public interface IDataReference : IDisposable {
		string FormulaBody { get; set; }
		bool IsNumber { get; }
		DataReferenceValueType ValueType { get; set; }
		bool Equals(IDataReference value);
		IDataReference CloneTo(DocumentModel documentModel);
		void Visit(IDataReferenceVisitor visitor);
		DataReferenceType GetReferenceType();
		DataReferenceType GetReferenceType(bool isNumber);
		object this[int index] { get; }
		long ValuesCount { get; }
		string FormatCode { get; set; }
		ChartNumberFormat NumberFormat { get; set; }
		void OnContentVersionChanged();
		void ObtainReferencedRanges(FormulaReferencedRanges where);
		void ResetCachedValue();
		void DetectValueType();
		void DetectValueType(bool suppressDateTime);
		void OnRangeInserting(InsertRangeNotificationContext context);
		void OnRangeRemoving(RemoveRangeNotificationContext context);
		string GetDisplayText(int index);
	}
	#endregion
	#region IDataReferenceVisitor
	public interface IDataReferenceVisitor {
		void Visit(ChartDataReference item);
		void Visit(DataReference item);
	}
	#endregion
	#region DataReference
	public class DataReference : IDataReference {
		static IDataReference empty = new DataReference();
		public static IDataReference Empty { get { return empty; } }
		#region IDataReference Members
		public string FormulaBody { get { return string.Empty; } set { } }
		public DataReferenceValueType ValueType { get { return DataReferenceValueType.Number; } set { } }
		public bool Equals(IDataReference value) {
			if (value == null)
				return false;
			return object.ReferenceEquals(value, empty);
		}
		public IDataReference CloneTo(DocumentModel documentModel) {
			return DataReference.Empty;
		}
		public void Visit(IDataReferenceVisitor visitor) {
			visitor.Visit(this);
		}
		public bool IsNumber { get { return false; } }
		public DataReferenceType GetReferenceType() {
			return DataReferenceType.None;
		}
		public DataReferenceType GetReferenceType(bool isNumber) {
			return DataReferenceType.None;
		}
		public object this[int index] {
			get { return index + 1; }
		}
		public long ValuesCount { get { return 0; } }
		public string FormatCode {
			get { return string.Empty; }
			set { 
			}
		}
		public ChartNumberFormat NumberFormat {
			get { return null; }
			set {
			}
		}
		public void OnContentVersionChanged() {
		}
		public void ObtainReferencedRanges(FormulaReferencedRanges where) {
		}
		public void ResetCachedValue() {
		}
		public void DetectValueType() {
		}
		public void DetectValueType(bool suppressDateTime) {
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		#endregion
		public string GetDisplayText(int index) {
			int result = index + 1;
			return result.ToString();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	#endregion
	#region ChartDataReference
	public class ChartDataReference : IDataReference, IDisposable, ISupportsInvalidate {
		#region Fields
		readonly DocumentModel documentModel;
		readonly FormulaData formulaData;
		readonly bool isNumber;
		ChartViewSeriesDirection seriesDirection;
		IChartDataConverter converter;
		string formatCode = string.Empty;
		FormulaReferencedRanges referencedRanges;
		DataReferenceValueType valueType;
		ChartNumberFormat numberFormat = null;
		IChartDataConverter numberFormatConverter = null;
		#endregion
		public ChartDataReference(DocumentModel documentModel, ChartViewSeriesDirection seriesDirection, bool isNumber) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.seriesDirection = seriesDirection;
			this.isNumber = isNumber;
			valueType = isNumber ? DataReferenceValueType.Number : DataReferenceValueType.String;
			formulaData = new FormulaData(documentModel, new ChartDataReferenceExpressionFilter());
			formulaData.Parent = this;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public FormulaData FormulaData { get { return formulaData; } }
		internal FormulaReferencedRanges ReferencedRanges { get { return referencedRanges; } }
		public ChartViewSeriesDirection SeriesDirection { get { return seriesDirection; } set { seriesDirection = value; } }
		public bool IsNumber { get { return isNumber; } }
		public string FormulaBody {
			get { return formulaData.FormulaBody; }
			set {
				formulaData.FormulaBody = value;
				ResetReferencedRanges();
			}
		}
		public string FormatCode {
			get { return formatCode; }
			set {
				if (value == null)
					value = string.Empty;
				if (formatCode == value)
					return;
				SetFormatCode(value);
			}
		}
		public ChartNumberFormat NumberFormat {
			get { return numberFormat; }
			set {
				numberFormat = value;
				this.numberFormatConverter = null;
			}
		}
		public VariantValue CachedValue { get { return formulaData.CachedValue; } set { formulaData.CachedValue = value; } }
		public ParsedExpression Expression { get { return formulaData.Expression; } set { formulaData.Expression = value; } }
		public IChartDataConverter Converter {
			get {
				if (converter == null)
					converter = CreateConverter(IsNumber, ValueType);
				return converter;
			}
		}
		public DataReferenceValueType ValueType { get { return valueType; } set { valueType = value; } }
		IChartDataConverter CreateConverter(bool isNumber, DataReferenceValueType valueType) {
			if (valueType == DataReferenceValueType.DateTime)
				return new ChartDataDateConverter(DocumentModel.DataContext);
			if (isNumber)
				return new ChartDataNumericConverter(DocumentModel.DataContext);
			return new ChartDataNumberFormatConverter(DocumentModel.DataContext, GetActualNumberFormatString());
		}
		private string GetActualNumberFormatString() {
			string numberFormatString = this.formatCode;
			if (NumberFormat != null)
				numberFormatString = NumberFormat.SourceLinked ? GetNumberFormatString() : NumberFormat.FormatCode;
			return numberFormatString;
		}
		#endregion
		#region FormatCode
		void SetFormatCode(string value) {
			ChartDataFormatCodePropertyChangedHistoryItem historyItem = new ChartDataFormatCodePropertyChangedHistoryItem(DocumentModel, this, formatCode, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetFormatCodeCore(string value) {
			this.formatCode = value;
			this.numberFormatConverter = null;
		}
		#endregion
		#region Expression
		public void SetRange(CellRangeBase cellRange) {
			formulaData.SetRange(cellRange);
		}
		public void SetVariantValue(VariantValue value) {
			VariantValue arrayValue = ConvertToArrayValue(value);
			FormulaData.SetVariantValue(arrayValue);
		}
		public void SetVariantValueWithoutHistory(VariantValue value) {
			VariantValue arrayValue = ConvertToArrayValue(value);
			FormulaData.SetVariantValueWithoutHistory(arrayValue);
		}
		VariantValue ConvertToArrayValue(VariantValue value) {
			if (value.IsArray || value.IsCellRange)
				return value;
			VariantArray array = VariantArray.Create(1, 1);
			array.SetValue(0, 0, value);
			return VariantValue.FromArray(array);
		}
		#endregion
		protected IChartDataConverter NumberFormatConverter {
			get {
				if (numberFormatConverter == null) {
					bool sourceLinked = NumberFormat != null ? NumberFormat.SourceLinked : false;
					numberFormatConverter = new ChartDataNumberFormatConverter(DocumentModel.DataContext, sourceLinked ? GetNumberFormatString() : this.formatCode);
				}
				return numberFormatConverter;
			}
		}
		#region IDataReference Members
		public object this[int index] { get { return GetValueByIndex(index); } }
		public long ValuesCount { get { return GetValuesCount(); } }
		public bool Equals(IDataReference value) {
			if (value == null)
				return false;
			ChartDataReference otherReference = value as ChartDataReference;
			if (otherReference == null)
				return false;
			return seriesDirection == otherReference.seriesDirection && IsNumber == value.IsNumber && FormulaData.Equals(otherReference.FormulaData);
		}
		public IDataReference CloneTo(DocumentModel documentModel) {
			ChartDataReference result = new ChartDataReference(documentModel, seriesDirection, IsNumber);
			result.FormatCode = FormatCode;
			result.NumberFormat = ChartNumberFormat.Clone(NumberFormat);
			if (Object.ReferenceEquals(documentModel, DocumentModel))
				result.FormulaData.FormulaBody = FormulaData.FormulaBody;
			else {
				result.FormulaData.Expression = FormulaData.Expression.Clone();
				result.CachedValue = VariantValue.Empty;
			}
			return result;
		}
		public void Visit(IDataReferenceVisitor visitor) {
			visitor.Visit(this);
		}
		public DataReferenceType GetReferenceType() {
			return GetReferenceType(IsNumber);
		}
		public DataReferenceType GetReferenceType(bool isNumber) {
			VariantValue cachedValue = CachedValue;
			bool isLiteral = cachedValue.IsArray;
			if (isNumber)
				return isLiteral ? DataReferenceType.NumberLiteral : DataReferenceType.Number;
			if (cachedValue.IsCellRange && IsMultilevel(cachedValue.CellRangeValue))
				return DataReferenceType.MultiLevelString;
			if (DetectCacheContainsString())
				return isLiteral ? DataReferenceType.StringLiteral : DataReferenceType.String;
			return isLiteral ? DataReferenceType.NumberLiteral : DataReferenceType.Number;
		}
		long GetValuesCount() {
			VariantValue cachedValue = CachedValue;
			if (cachedValue.IsArray)
				return cachedValue.ArrayValue.Count;
			if (cachedValue.IsCellRange) {
				CellRangeBase cellRangeBaseValue = cachedValue.CellRangeValue;
				bool isMultilevel = IsMultilevel(cellRangeBaseValue);
				if (!isMultilevel)
					return cellRangeBaseValue.CellCount;
				if (seriesDirection == ChartViewSeriesDirection.Vertical)
					return cellRangeBaseValue.GetMaxHeight();
				else
					return cellRangeBaseValue.GetMaxWidth();
			}
			return 1;
		}
		bool IsMultilevel(CellRangeBase cellRangeBaseValue) {
			return cellRangeBaseValue.GetMaxWidth() > 1 && cellRangeBaseValue.GetMaxHeight() > 1;
		}
		#region GetValue (VariantValue)
		protected internal VariantValue GetValue(int index) {
			VariantValue cachedValue = CachedValue;
			if (cachedValue.IsArray)
				return GetValueFromArray(index, cachedValue);
			else if (cachedValue.IsCellRange)
				return GetValueFromCellRange(index, cachedValue);
			else {
				if (index == 0)
					return cachedValue;
			}
			return VariantValue.Empty;
		}
		VariantValue GetValueFromArray(int index, VariantValue cachedValue) {
			if (index < cachedValue.ArrayValue.Count)
				return cachedValue.ArrayValue[index];
			return VariantValue.Empty;
		}
		VariantValue GetValueFromCellRange(int index, VariantValue cachedValue) {
			CellRangeBase cellRangeBase = cachedValue.CellRangeValue;
			if (!IsMultilevel(cellRangeBase)) {
				if (index < cellRangeBase.CellCount)
					return cellRangeBase.GetCellValueByZOrder(index);
			}
			else {
				if (isNumber)
					return index;
				else
					return GetMultilevelValue(cellRangeBase, index);
			}
			return VariantValue.Empty;
		}
		#endregion
		#region GetValue
		object GetValueByIndex(int index) {
			WorkbookDataContext context = DocumentModel.DataContext;
			VariantValue cachedValue = CachedValue;
			object result = null;
			if (cachedValue.IsArray)
				result = GetValueFromArray(index, cachedValue, context);
			else if (cachedValue.IsCellRange)
				result = GetValueFromCellRange(index, cachedValue, context);
			else {
				if (index > 0)
					return null;
				result = ConvertSingleValue(cachedValue, context);
			}
			return result;
		}
		object GetValueFromArray(int index, VariantValue cachedValue, WorkbookDataContext context) {
			object result = null;
			if (index < cachedValue.ArrayValue.Count)
				result = ConvertSingleValue(cachedValue.ArrayValue[index], context);
			return result;
		}
		object GetValueFromCellRange(int index, VariantValue cachedValue, WorkbookDataContext context) {
			object result = null;
			CellRangeBase cellRangeBase = cachedValue.CellRangeValue;
			if (!IsMultilevel(cellRangeBase)) {
				if (index < cellRangeBase.CellCount) {
					VariantValue cellValue = cellRangeBase.GetCellValueByZOrder(index);
					result = ConvertSingleValue(cellValue, context);
				}
			}
			else {
				if (isNumber)
					result = index;
				else {
					VariantValue mlResult = GetMultilevelValue(cellRangeBase, index);
					if (!mlResult.IsEmpty)
						result = ConvertSingleValue(mlResult, context);
					else
						result = null;
				}
			}
			return result;
		}
		#region GetMultilevelValue
		VariantValue GetMultilevelValue(CellRangeBase cellRangeBase, int index) {
			StringBuilder builder = new StringBuilder();
			if (!CollectValuesFromCellRangeBase(builder, cellRangeBase, index))
				return VariantValue.Empty;
			if (builder.Length >= 1)
				builder.Remove(builder.Length - 1, 1);
			return builder.ToString();
		}
		bool CollectValuesFromCellRangeBase(StringBuilder builder, CellRangeBase range, int index) {
			if (range.RangeType == CellRangeType.UnionRange)
				return CollectValuesFromCellUnion(builder, (CellUnion)range, index);
			else
				return CollectValuesFromCellRange(builder, (CellRange)range, index);
		}
		bool CollectValuesFromCellUnion(StringBuilder builder, CellUnion range, int index) {
			bool result = false;
			foreach (CellRangeBase innerRange in range.InnerCellRanges) {
				int valuesCount = seriesDirection == ChartViewSeriesDirection.Vertical ? innerRange.Height : innerRange.Width;
				if (index < valuesCount) {
					result |= CollectValuesFromCellRange(builder, (CellRange)innerRange, index);
					break;
				}
				else
					index -= valuesCount;
			}
			return result;
		}
		bool CollectValuesFromCellRange(StringBuilder builder, CellRange range, int index) {
			WorkbookDataContext context = DocumentModel.DataContext;
			int valuesCount;
			int levelCount;
			if (seriesDirection == ChartViewSeriesDirection.Vertical) {
				valuesCount = range.Height;
				levelCount = range.Width;
			}
			else {
				valuesCount = range.Width;
				levelCount = range.Height;
			}
			if (index >= valuesCount)
				return false;
			for (int i = levelCount - 1; i >= 0; i--) {
				VariantValue value = seriesDirection == ChartViewSeriesDirection.Vertical ? range.GetCellValueRelative(i, index) : range.GetCellValueRelative(index, i);
				string stringValue = ConvertSingleValue(value, context).ToString();
				if (!string.IsNullOrEmpty(stringValue)) {
					builder.Append(stringValue);
					builder.Append(" ");
				}
			}
			return true;
		}
		#endregion
		object ConvertSingleValue(VariantValue value, WorkbookDataContext context) {
			if (value.IsSharedString)
				value = value.GetTextValue(documentModel.SharedStringTable);
			return Converter.ConvertSingleValue(value);
		}
		#endregion
		public void OnContentVersionChanged() {
			if (!string.IsNullOrEmpty(FormulaData.TempBody))
				FormulaData.ParseTemporarilySavedBody();
		}
		public void ObtainReferencedRanges(FormulaReferencedRanges where) {
			if (referencedRanges != null) {
				where.AddRange(referencedRanges);
				return;
			}
			if (CachedValue.IsCellRange) {
				ParsedExpression expression = null;
				DocumentModel.DataContext.SetImportExportSettings();
				try {
					expression = DocumentModel.DataContext.ParseExpression("=" + this.FormulaBody, OperandDataType.Default, false);
				}
				finally {
					DocumentModel.DataContext.SetWorkbookDefinedSettings();
				}
				if (expression == null)
					return;
				VariantValue value = expression.Evaluate(DocumentModel.DataContext);
				if (value.IsCellRange) {
					referencedRanges = new FormulaReferencedRanges();
					foreach (CellRange range in value.CellRangeValue.GetAreasEnumerable()) {
						FormulaReferencedRange formulaReferencedRange = new FormulaReferencedRange(range, 0, 0, true);
						referencedRanges.Add(formulaReferencedRange);
					}
					where.AddRange(referencedRanges);
				}
			}
		}
		public void ResetCachedValue() {
			if (Expression != null)
				CachedValue = VariantValue.Empty;
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			formulaData.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			formulaData.OnRangeRemoving(context);
		}
		#endregion
		public string GetDisplayText(int index) {
			VariantValue value = GetValue(index);
			object text = NumberFormatConverter.ConvertSingleValue(value);
			if (text == null)
				return string.Empty;
			return text.ToString();
		}
		#endregion
		#region ISupportsInvalidate Members
		public void Invalidate() {
			ResetReferencedRanges();
		}
		#endregion
		void ResetReferencedRanges() {
			this.referencedRanges = null;
		}
		bool DetectCacheContainsString() {
			VariantValue cachedValue = CachedValue;
			if (cachedValue.IsText)
				return true;
			else if (cachedValue.IsCellRange) {
				foreach (VariantValue value in new Enumerable<VariantValue>(cachedValue.CellRangeValue.GetExistingValuesEnumerator())) {
					if (value.IsText)
						return true;
				}
			}
			else if (cachedValue.IsArray) {
				IVariantArray array = cachedValue.ArrayValue;
				for (int i = 0; i < array.Count; i++)
					if (array[i].IsText)
						return true;
			}
			return false;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void DetectValueType() {
			DetectValueType(false);
		}
		public void DetectValueType(bool suppressDateTime) {
			VariantValue cachedValue = CachedValue;
			DataReferenceValueType newType;
			if (!cachedValue.IsCellRange)
				newType = isNumber ? DataReferenceValueType.Number : DataReferenceValueType.String;
			else {
				newType = isNumber ? DataReferenceValueType.Number : DataReferenceValueType.String;
				if (!suppressDateTime) {
					CellRangeBase cellRangeValue = cachedValue.CellRangeValue;
					bool isDateTimeEverywhere = true;
					bool isAllCellsEmpty = true;
					foreach (ICellBase cell in cellRangeValue.GetExistingCellsEnumerable()) {
						VariantValue cellValue = cell.Value;
						if (cellValue.IsEmpty || cellValue.IsError)
							continue;
						isAllCellsEmpty = false;
						IFormatStringAccessor formatStringAccessor = cell as IFormatStringAccessor;
						if (formatStringAccessor != null) {
							NumberFormat format = new NumberFormat(0, formatStringAccessor.FormatString);
							if (!format.IsDateTime || !cellValue.IsNumeric) {
								isDateTimeEverywhere = false;
								break;
							}
						}
					}
					if (isDateTimeEverywhere && !isAllCellsEmpty)
						newType = DataReferenceValueType.DateTime;
				}
			}
			if (newType != valueType) {
				valueType = newType;
				converter = null;
			}
		}
		protected internal string GetNumberFormatString() {
			VariantValue cachedValue = CachedValue;
			if (!cachedValue.IsCellRange)
				return string.Empty;
			CellRangeBase cellRangeValue = cachedValue.CellRangeValue;
			ICellBase cell = cellRangeValue.GetFirstCellUnsafe();
			if (cell == null)
				return string.Empty;
			IFormatStringAccessor formatStringAccessor = cell as IFormatStringAccessor;
			return (formatStringAccessor != null) ? formatStringAccessor.FormatString : string.Empty;
		}
	}
	#endregion
	#region ChartDataReferenceExpressionFilter
	public class ChartDataReferenceExpressionFilter : IExpressionFilter {
		#region allowedExpressionTypeTable
		static Dictionary<Type, bool> allowedExpressionTypeTable = CreateAllowedExpressionTypeTable();
		static Dictionary<Type, bool> CreateAllowedExpressionTypeTable() {
			Dictionary<Type, bool> result = new Dictionary<Type, bool>();
			result.Add(typeof(ParsedThingRef3d), true);
			result.Add(typeof(ParsedThingArea3d), true);
			result.Add(typeof(ParsedThingNameX), true);
			result.Add(typeof(ParsedThingTable), true);
			result.Add(typeof(ParsedThingAttrSpace), true);
			result.Add(typeof(ParsedThingAttrSpaceSemi), true);
			result.Add(typeof(ParsedThingParentheses), true);
			result.Add(typeof(ParsedThingUnion), true);
			result.Add(typeof(ParsedThingErr3d), true);
			result.Add(typeof(ParsedThingRefErr), true);
			result.Add(typeof(ParsedThingArray), true);
			return result;
		}
		#endregion
		#region IExpressionFilter Members
		public bool CheckExpression(ParsedExpression expression) {
			for (int i = 0; i < expression.Count; i++) {
				IParsedThing currentThing = expression[i];
				if (!(currentThing is Model.ParsedThingAttrBase) && !(currentThing is Model.ParsedThingMemBase) && !allowedExpressionTypeTable.ContainsKey(currentThing.GetType()))
					return false;
			}
			return true;
		}
		#endregion
	}
	#endregion
	#region IChartDataConverter
	public interface IChartDataConverter {
		object ConvertSingleValue(VariantValue value);
	}
	#endregion
	#region ChartDataNumericConverter
	public class ChartDataNumericConverter : IChartDataConverter {
		readonly WorkbookDataContext context;
		public ChartDataNumericConverter(WorkbookDataContext context) {
			this.context = context;
		}
		#region IChartDataConverter Members
		object IChartDataConverter.ConvertSingleValue(VariantValue value) {
			if (value.IsEmpty || value == VariantValue.ErrorValueNotAvailable)
				return null;
			if (!value.IsText)
				return value.ToNumeric(context).NumericValue;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region ChartDataStringConverter
	public class ChartDataStringConverter : IChartDataConverter {
		readonly WorkbookDataContext context;
		public ChartDataStringConverter(WorkbookDataContext context) {
			this.context = context;
		}
		#region IChartDataConverter Members
		object IChartDataConverter.ConvertSingleValue(VariantValue value) {
			if (value.IsError)
				return CellErrorFactory.GetErrorName(value.ErrorValue, context);
			return value.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
		}
		#endregion
	}
	#endregion
	#region ChartDataNumberFormatConverter
	public class ChartDataNumberFormatConverter : IChartDataConverter {
		readonly WorkbookDataContext context;
		readonly NumberFormat formatter;
		public ChartDataNumberFormatConverter(WorkbookDataContext context, string numberFormatString) {
			this.context = context;
			if (string.IsNullOrEmpty(numberFormatString) || StringExtensions.CompareInvariantCultureIgnoreCase("General", numberFormatString) == 0)
				formatter = NumberFormat.Generic;
			else {
				NumberFormat format = NumberFormatParser.Parse(numberFormatString);
				int index = context.Workbook.Cache.NumberFormatCache.AddItem(format);
				formatter = context.Workbook.Cache.NumberFormatCache[index];
			}
		}
		#region IChartDataConverter Members
		object IChartDataConverter.ConvertSingleValue(VariantValue value) {
			if (value.IsError)
				return CellErrorFactory.GetErrorName(value.ErrorValue, context);
			if (!value.IsNumeric)
				return value.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
			return formatter.Format(value, context).Text;
		}
		#endregion
	}
	#endregion
	#region ChartDataDateConverter
	public class ChartDataDateConverter : IChartDataConverter {
		readonly WorkbookDataContext context;
		public ChartDataDateConverter(WorkbookDataContext context) {
			this.context = context;
		}
		#region IChartDataConverter Members
		object IChartDataConverter.ConvertSingleValue(VariantValue value) {
			if (value.IsEmpty || value == VariantValue.ErrorValueNotAvailable)
				return null;
			return value.ToDateTime(context);
		}
		#endregion
	}
	#endregion
}
