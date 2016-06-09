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
using System.IO;
using System.Text;
using DevExpress.Export.Xl;
namespace DevExpress.Export.Xl {
	using DevExpress.XtraExport.Implementation;
	public static class XlFunc {
		public static IXlFormulaParameter Param(XlVariantValue value) {
			return new XlFormulaParameter(value);
		}
		public static IXlFormulaParameter Subtotal(XlCellRange range, XlSummary summary, bool ignoreHidden) {
			return XlSubtotalFunction.Create(range, summary, ignoreHidden);
		}
		public static IXlFormulaParameter Subtotal(IList<XlCellRange> ranges, XlSummary summary, bool ignoreHidden) {
			return XlSubtotalFunction.Create(ranges, summary, ignoreHidden);
		}
		public static IXlFormulaParameter VLookup(XlVariantValue lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return XlVLookupFunction.Create(lookupValue, table, columnIndex, rangeLookup);
		}
		public static IXlFormulaParameter VLookup(IXlFormulaParameter lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return XlVLookupFunction.Create(lookupValue, table, columnIndex, rangeLookup);
		}
		public static IXlFormulaParameter Text(XlVariantValue value, string netFormatString, bool isDateTimeFormatString) {
			return XlTextFunction.Create(value, netFormatString, isDateTimeFormatString);
		}
		public static IXlFormulaParameter Text(IXlFormulaParameter formula, string netFormatString, bool isDateTimeFormatString) {
			return XlTextFunction.Create(formula, netFormatString, isDateTimeFormatString);
		}
		public static IXlFormulaParameter Text(XlVariantValue value, XlNumberFormat numberFormat) {
			return XlTextFunction.Create(value, numberFormat);
		}
		public static IXlFormulaParameter Text(IXlFormulaParameter formula, XlNumberFormat numberFormat) {
			return XlTextFunction.Create(formula, numberFormat);
		}
		public static IXlFormulaParameter Concatenate(params IXlFormulaParameter[] parameters) {
			return new XlConcatenateFunction(parameters);
		}
		public static IXlFormulaParameter Count(params IXlFormulaParameter[] parameters) {
			return new XlCountFunction(parameters);
		}
		public static IXlFormulaParameter Sum(params IXlFormulaParameter[] parameters) {
			return new XlSumFunction(parameters);
		}
		public static IXlFormulaParameter Average(params IXlFormulaParameter[] parameters) {
			return new XlAverageFunction(parameters);
		}
		public static IXlFormulaParameter Min(params IXlFormulaParameter[] parameters) {
			return new XlMinFunction(parameters);
		}
		public static IXlFormulaParameter Max(params IXlFormulaParameter[] parameters) {
			return new XlMaxFunction(parameters);
		}
	}
}
namespace DevExpress.XtraExport.Implementation {
	using DevExpress.Utils;
	#region XlSubtotalFunction
	internal class XlSubtotalFunction : IXlFormulaParameter {
		public static XlSubtotalFunction Create(XlCellRange range, XlSummary summary, bool ignoreHidden) {
			XlSubtotalFunction result = new XlSubtotalFunction();
			result.Ranges = new List<XlCellRange>();
			result.Ranges.Add(range);
			result.Summary = summary;
			result.IgnoreHidden = ignoreHidden;
			return result;
		}
		public static XlSubtotalFunction Create(IList<XlCellRange> ranges, XlSummary summary, bool ignoreHidden) {
			XlSubtotalFunction result = new XlSubtotalFunction();
			result.Ranges = new List<XlCellRange>(ranges);
			result.Summary = summary;
			result.IgnoreHidden = ignoreHidden;
			return result;
		}
		public IList<XlCellRange> Ranges { get; set; }
		public XlSummary Summary { get; set; }
		public bool IgnoreHidden { get; set; }
		public string ToString(CultureInfo culture) {
			int code = (int)Summary;
			if(IgnoreHidden)
				code += 100;
			StringBuilder sb = new StringBuilder();
			int subtotalCount = 0;
			int count = 0;
			foreach(XlCellRange range in Ranges) {
				if(count == 0) {
					if(subtotalCount > 0)
						sb.Append(",");
					sb.Append("SUBTOTAL(");
					sb.Append(code.ToString());
				}
				sb.Append(",");
				sb.Append(range.ToString());
				count++;
				if(count >= 254) {
					sb.Append(")");
					subtotalCount++;
					count = 0;
				}
			}
			if(count > 0)
				sb.Append(")");
			if(subtotalCount == 0)
				return sb.ToString();
			if(Summary == XlSummary.Average)
				return string.Format("AVERAGE({0})", sb.ToString());
			if(Summary == XlSummary.Min)
				return string.Format("MIN({0})", sb.ToString());
			if(Summary == XlSummary.Max)
				return string.Format("MAX({0})", sb.ToString());
			return string.Format("SUM({0})", sb.ToString());
		}
	}
	#endregion
	#region XlVLookupFunction
	internal class XlVLookupFunction : IXlFormulaParameter {
		public static XlVLookupFunction Create(XlVariantValue lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return Create(new XlFormulaParameter(lookupValue), table, columnIndex, rangeLookup);
		}
		public static XlVLookupFunction Create(IXlFormulaParameter lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			XlVLookupFunction result = new XlVLookupFunction();
			result.LookupValue = lookupValue;
			result.Table = table;
			result.ColumnIndex = columnIndex;
			result.RangeLookup = rangeLookup;
			return result;
		}
		public IXlFormulaParameter LookupValue { get; set; }
		public XlCellRange Table { get; set; }
		public int ColumnIndex { get; set; }
		public bool RangeLookup { get; set; }
		public string ToString(CultureInfo culture) {
			XlVariantValue rangeLookupValue = new XlVariantValue() { BooleanValue = RangeLookup };
			string lookupValueText;
			if(LookupValue != null)
				lookupValueText = LookupValue.ToString(culture);
			else
				lookupValueText = "#VALUE!";
			return string.Format("VLOOKUP({0}, {1}, {2}, {3})", lookupValueText, Table.ToString(), ColumnIndex, rangeLookupValue.ToText().TextValue);
		}
	}
	#endregion
	#region XlTextFunction
	internal class XlTextFunction : IXlFormulaParameter {
		public static XlTextFunction Create(XlVariantValue value, string netFormatString, bool isDateTimeFormatString) {
			return Create(new XlFormulaParameter(value), netFormatString, isDateTimeFormatString);
		}
		public static XlTextFunction Create(IXlFormulaParameter formula, string netFormatString, bool isDateTimeFormatString) {
			XlTextFunction result = new XlTextFunction();
			result.Value = formula;
			result.NetFormatString = netFormatString;
			result.IsDateTimeFormatString = isDateTimeFormatString;
			return result;
		}
		public static XlTextFunction Create(XlVariantValue value, XlNumberFormat numberFormat) {
			return Create(new XlFormulaParameter(value), numberFormat);
		}
		public static XlTextFunction Create(IXlFormulaParameter formula, XlNumberFormat numberFormat) {
			XlTextFunction result = new XlTextFunction();
			result.Value = formula;
			result.NumberFormat = numberFormat;
			return result;
		}
		public IXlFormulaParameter Value { get; set; }
		public string NetFormatString { get; set; }
		public bool IsDateTimeFormatString { get; set; }
		public XlNumberFormat NumberFormat { get; set; }
		public string ToString(CultureInfo culture) {
			if(culture == null)
				culture = CultureInfo.InvariantCulture;
			string valueText = Value != null ? Value.ToString(culture) : "#VALUE!";
			if(NumberFormat != null)
				return string.Format("TEXT({0}, \"{1}\")", valueText, NumberFormat.GetLocalizedFormatCode(culture));
			XlExportNumberFormatConverter numberFormatConverter = new XlExportNumberFormatConverter();
			ExcelNumberFormat numberFormat = numberFormatConverter.Convert(NetFormatString, IsDateTimeFormatString, culture);
			string localFormatString = IsDateTimeFormatString ?
				numberFormatConverter.GetLocalDateFormatString(numberFormat != null ? numberFormat.FormatString : string.Empty, culture) :
				numberFormatConverter.GetLocalFormatString(numberFormat != null ? numberFormat.FormatString : string.Empty, culture);
			return string.Format("TEXT({0}, \"{1}\")", valueText, localFormatString);
		}
	}
	#endregion
	#region XlFormulaParameter
	internal class XlFormulaParameter : IXlFormulaParameter {
		public XlFormulaParameter(XlVariantValue value) {
			Value = value;
		}
		public XlVariantValue Value { get; set; }
		public string ToString(CultureInfo culture) {
			string result = Value.ToText().TextValue;
			if(string.IsNullOrEmpty(result))
				result = string.Empty;
			if(Value.IsText)
				result = "\"" + result + "\"";
			return result;
		}
	}
	#endregion
	#region XlFunctionBase
	internal abstract class XlFunctionBase : IXlFormulaParameter {
		readonly List<IXlFormulaParameter> parameters;
		protected XlFunctionBase(IEnumerable<IXlFormulaParameter> parameters) {
			this.parameters = new List<IXlFormulaParameter>(parameters);
		}
		public IList<IXlFormulaParameter> Parameters { get { return parameters; } }
		public abstract XlPtgDataType ParamType { get; }
		public abstract int FunctionCode { get; }
		protected abstract string FunctionName { get; }
		public string ToString(CultureInfo culture) {
			StringBuilder sb = new StringBuilder();
			sb.Append(FunctionName);
			sb.Append("(");
			if(Parameters.Count == 0)
				sb.Append("#VALUE!");
			else {
				for(int i = 0; i < Parameters.Count; i++) {
					if(i > 0)
						sb.Append(",");
					sb.Append(Parameters[i].ToString(culture));
				}
			}
			sb.Append(")");
			return sb.ToString();
		}
	}
	#endregion
	#region XlConcatenateFunction
	internal class XlConcatenateFunction : XlFunctionBase {
		public XlConcatenateFunction(IEnumerable<IXlFormulaParameter> parameters) :
			base(parameters) {
		}
		public override XlPtgDataType ParamType {
			get { return XlPtgDataType.Value; }
		}
		public override int FunctionCode {
			get { return 0x0150; }
		}
		protected override string FunctionName {
			get { return "CONCATENATE"; }
		}
	}
	#endregion
	#region XlCountFunction
	internal class XlCountFunction : XlFunctionBase {
		public XlCountFunction(IEnumerable<IXlFormulaParameter> parameters) 
			: base(parameters) {
		}
		public override XlPtgDataType ParamType {
			get { return XlPtgDataType.Reference; }
		}
		public override int FunctionCode {
			get { return 0x0000; }
		}
		protected override string FunctionName {
			get { return "COUNT"; }
		}
	}
	#endregion
	#region XlSumFunction
	internal class XlSumFunction : XlFunctionBase {
		public XlSumFunction(IEnumerable<IXlFormulaParameter> parameters)
			: base(parameters) {
		}
		public override XlPtgDataType ParamType {
			get { return XlPtgDataType.Reference; }
		}
		public override int FunctionCode {
			get { return 0x0004; }
		}
		protected override string FunctionName {
			get { return "SUM"; }
		}
	}
	#endregion
	#region XlAverageFunction
	internal class XlAverageFunction : XlFunctionBase {
		public XlAverageFunction(IEnumerable<IXlFormulaParameter> parameters)
			: base(parameters) {
		}
		public override XlPtgDataType ParamType {
			get { return XlPtgDataType.Reference; }
		}
		public override int FunctionCode {
			get { return 0x0005; }
		}
		protected override string FunctionName {
			get { return "AVERAGE"; }
		}
	}
	#endregion
	#region XlMinFunction
	internal class XlMinFunction : XlFunctionBase {
		public XlMinFunction(IEnumerable<IXlFormulaParameter> parameters)
			: base(parameters) {
		}
		public override XlPtgDataType ParamType {
			get { return XlPtgDataType.Reference; }
		}
		public override int FunctionCode {
			get { return 0x0006; }
		}
		protected override string FunctionName {
			get { return "MIN"; }
		}
	}
	#endregion
	#region XlMaxFunction
	internal class XlMaxFunction : XlFunctionBase {
		public XlMaxFunction(IEnumerable<IXlFormulaParameter> parameters)
			: base(parameters) {
		}
		public override XlPtgDataType ParamType {
			get { return XlPtgDataType.Reference; }
		}
		public override int FunctionCode {
			get { return 0x0007; }
		}
		protected override string FunctionName {
			get { return "MAX"; }
		}
	}
	#endregion
}
