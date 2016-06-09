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

using DevExpress.Utils;
using System;
using System.Diagnostics;
using System.Globalization;
namespace DevExpress.Export.Xl {
	#region VariantValueType
	public enum XlVariantValueType {
		None,
		Boolean,
		Text,
		Numeric,
		DateTime,
		Error,
	}
	#endregion
	#region XlVariantValue
	public struct XlVariantValue {
		const double MaxDateTimeSerialNumber = 2958465.999999990;  
		const double MaxDateTimeSerialNumber1904 = 2957003.999999990;
		const string trueConstant = "TRUE";
		const string falseConstant = "FALSE";
		static readonly DateTime baseDate = new DateTime(1899, 12, 30);
		static readonly DateTime baseDate1 = new DateTime(1899, 12, 31);
		static readonly TimeSpan day29Feb1900 = TimeSpan.FromDays(60);
		static readonly DateTime baseDate1904 = new DateTime(1904, 1, 1);
		public static DateTime BaseDate { get { return baseDate; } }
		public static TimeSpan Day29Feb1900 { get { return day29Feb1900; } }
		public static string TrueConstant { get { return trueConstant; } }
		public static string FalseConstant { get { return falseConstant; } }
		public static readonly XlVariantValue Empty = new XlVariantValue();
		public static readonly XlVariantValue ErrorInvalidValueInFunction = XlVariantValue.FromError(InvalidValueInFunctionError.Instance);
		public static readonly XlVariantValue ErrorDivisionByZero = XlVariantValue.FromError(DivisionByZeroError.Instance);
		public static readonly XlVariantValue ErrorNumber = XlVariantValue.FromError(NumberError.Instance);
		public static readonly XlVariantValue ErrorReference = XlVariantValue.FromError(ReferenceError.Instance);
		public static readonly XlVariantValue ErrorValueNotAvailable = XlVariantValue.FromError(ValueNotAvailableError.Instance);
		public static readonly XlVariantValue ErrorNullIntersection = XlVariantValue.FromError(NullIntersectionError.Instance);
		public static readonly XlVariantValue ErrorName = XlVariantValue.FromError(NameError.Instance);
		double numericValue; 
		object referenceValue;
		XlVariantValueType type; 
		public XlVariantValueType Type { [DebuggerStepThrough] get { return type; } }
		public bool IsEmpty { [DebuggerStepThrough] get { return type == XlVariantValueType.None; } }
		public bool IsNumeric { [DebuggerStepThrough] get { return type == XlVariantValueType.Numeric || type == XlVariantValueType.DateTime; } }
		public bool IsBoolean { [DebuggerStepThrough] get { return type == XlVariantValueType.Boolean; } }
		public bool IsText { [DebuggerStepThrough] get { return type == XlVariantValueType.Text; } }
		public bool IsError { [DebuggerStepThrough] get { return type == XlVariantValueType.Error; } }
		public double NumericValue {
			[DebuggerStepThrough]
			get { return numericValue; }
			[DebuggerStepThrough]
			set {
				this.type = XlVariantValueType.Numeric;
				this.numericValue = value;
				this.referenceValue = null;
			}
		}
		public DateTime DateTimeValue {
			[DebuggerStepThrough]
			get { return FromDateTimeSerial(numericValue); }
			[DebuggerStepThrough]
			set {
				SetDateTime(value);
			}
		}
		public bool BooleanValue {
			[DebuggerStepThrough]
			get { return this.numericValue != 0; }
			[DebuggerStepThrough]
			set {
				this.type = XlVariantValueType.Boolean;
				this.numericValue = value ? 1 : 0;
				this.referenceValue = null;
			}
		}
		public IXlCellError ErrorValue {
			[DebuggerStepThrough]
			get { return this.referenceValue as IXlCellError; }
			[DebuggerStepThrough]
			set {
				this.type = XlVariantValueType.Error;
				this.numericValue = 0;
				this.referenceValue = value;
			}
		}
		void SetDateTime(DateTime value) {
			bool onlyTimeSpecified = value.Year == 1 && value.Month == 1 && value.Day == 1;
			if (onlyTimeSpecified) {
				value = new DateTime(1899, 12, 31).AddTicks(value.Ticks);
			}
			else {
				if (value <= XlVariantValue.baseDate.AddDays(1))
					value = XlVariantValue.baseDate.AddDays(1);
			}
			this.type = XlVariantValueType.Numeric;
			this.numericValue = ToDateTimeSerialDouble(value);
			this.referenceValue = null;
		}
		public string TextValue {
			[DebuggerStepThrough]
			get { return referenceValue as string; }
			set {
				if (value == null)
					value = String.Empty;
				this.type = XlVariantValueType.Text;
				this.numericValue = 0;
				this.referenceValue = value;
			}
		}
		[DebuggerStepThrough]
		public override int GetHashCode() {
			if (this.referenceValue == null)
				return ((int)type << 29) ^ this.numericValue.GetHashCode();
			else
				return ((int)type << 29) ^ this.numericValue.GetHashCode() ^ this.referenceValue.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!(obj is XlVariantValue))
				return false;
			XlVariantValue value = (XlVariantValue)obj;
			if (value.type == type && value.numericValue == this.numericValue) {
				if(type == XlVariantValueType.Text) {
					return Object.Equals(this.referenceValue as string, value.referenceValue as string);
				}
				return Object.ReferenceEquals(value.referenceValue, this.referenceValue);
			}
			else
				return false;
		}
		[DebuggerStepThrough]
		public static bool operator ==(XlVariantValue first, XlVariantValue second) {
			if (first.type == second.type && first.numericValue == second.numericValue) {
				if(first.Type == XlVariantValueType.Text) {
					return (first.referenceValue as string) == (second.referenceValue as string);
				}
				return Object.ReferenceEquals(first.referenceValue, second.referenceValue);
			}
			else
				return false;
		}
		[DebuggerStepThrough]
		public static bool operator !=(XlVariantValue first, XlVariantValue second) {
			if (first.type != second.type || first.numericValue != second.numericValue)
				return true;
			if(first.Type == XlVariantValueType.Text) {
				return (first.referenceValue as string) != (second.referenceValue as string);
			}
			return !Object.ReferenceEquals(first.referenceValue, second.referenceValue);
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(double value) {
			XlVariantValue result = new XlVariantValue();
			if (double.IsNaN(value) || double.IsInfinity(value))
				result.NumericValue = ushort.MaxValue;
			else if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(value))
				result.NumericValue = 0;
			else
				result.NumericValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(float value) {
			XlVariantValue result = new XlVariantValue();
			if (float.IsNaN(value) || float.IsInfinity(value))
				result.NumericValue = ushort.MaxValue;
			else if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(value))
				result.NumericValue = 0;
			else
				result.NumericValue = ConvertFloat(value);
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(DateTime value) {
			XlVariantValue result = new XlVariantValue();
			result.DateTimeValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(TimeSpan value) {
			XlVariantValue result = new XlVariantValue();
			result.DateTimeValue = baseDate1.Add(value);
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(char value) {
			XlVariantValue result = new XlVariantValue();
			result.TextValue = char.ToString(value);
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(string value) {
			if (value == null)
				return XlVariantValue.Empty;
			XlVariantValue result = new XlVariantValue();
			result.TextValue = value;
			return result;
		}
		[DebuggerStepThrough]
		public static implicit operator XlVariantValue(bool value) {
			XlVariantValue result = new XlVariantValue();
			result.BooleanValue = value;
			return result;
		}
		public XlVariantValue ToText() {
			switch (type) {
				case XlVariantValueType.None:
				case XlVariantValueType.Text:
					return TextValue;
				case XlVariantValueType.DateTime:
				case XlVariantValueType.Numeric:
					return ConvertNumberToText(NumericValue);
				case XlVariantValueType.Boolean:
					return BooleanValue ? trueConstant : falseConstant;
				case XlVariantValueType.Error:
					return ErrorValue.Name;
				default:
					return this;
			}
		}
		public XlVariantValue ToText(CultureInfo culture) {
			switch (type) {
				case XlVariantValueType.None:
				case XlVariantValueType.Text:
					return TextValue;
				case XlVariantValueType.DateTime:
				case XlVariantValueType.Numeric:
					return ConvertNumberToText(NumericValue, culture);
				case XlVariantValueType.Boolean:
					return BooleanValue ? trueConstant : falseConstant;
				case XlVariantValueType.Error:
					return ErrorValue.Name;
				default:
					return this;
			}
		}
		string ConvertNumberToText(double value) {
			string result = value.ToString(CultureInfo.InvariantCulture);
			if (value > 1e+15 && value < 1e+16) {
				try {
					double parsedValue = double.Parse(result, CultureInfo.InvariantCulture);
					long intValue = (long)parsedValue;
					if (parsedValue == intValue) {
						string integerText = intValue.ToString(CultureInfo.InvariantCulture);
						if (integerText.Length < result.Length)
							return integerText;
					}
				}
				catch {
				}
			}
			return result;
		}
		string ConvertNumberToText(double value, CultureInfo culture) {
			string result = value.ToString(culture);
			if (value > 1e+15 && value < 1e+16) {
				try {
					double parsedValue = double.Parse(result, culture);
					long intValue = (long)parsedValue;
					if (parsedValue == intValue) {
						string integerText = intValue.ToString(culture);
						if (integerText.Length < result.Length)
							return integerText;
					}
				}
				catch {
				}
			}
			return result;
		}
		internal static bool IsErrorDateTimeSerial(double serialNumber, bool date1904) {
			if (date1904)
				return (serialNumber < 0 || serialNumber > MaxDateTimeSerialNumber1904);
			else
				return (serialNumber < 0 || serialNumber > MaxDateTimeSerialNumber);
		}
		static DateTime FromDateTimeSerial(double value) {
			if (value > 60)
				return XlVariantValue.BaseDate + TimeSpan.FromDays(value);
			else
				return XlVariantValue.BaseDate + TimeSpan.FromDays(value + 1);
		}
		static DateTime FromDateTimeSerial(double value, bool date1904) {
			if (date1904)
				return baseDate1904 + TimeSpan.FromDays(value);
			return FromDateTimeSerial(value);
		}
		static double ToDateTimeSerialDouble(DateTime value) {
			TimeSpan difference = value - XlVariantValue.BaseDate;
			if (difference > XlVariantValue.Day29Feb1900)
				return difference.TotalDays;
			else
				return difference.TotalDays - 1;
		}
		internal DateTime GetDateTime() {
			if (numericValue > 60)
				return XlVariantValue.BaseDate + TimeSpan.FromDays(numericValue);
			else if (numericValue >= 1)
				return XlVariantValue.BaseDate + TimeSpan.FromDays(numericValue + 1);
			else if (numericValue > 0)
				return DateTime.MinValue + +TimeSpan.FromDays(numericValue);
			return DateTime.MinValue;
		}
		public void SetDateTimeSerial(double value, bool date1904) {
			if (double.IsNaN(value) || double.IsInfinity(value))
				value = ushort.MaxValue;
			else if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(value))
				value = 0;
			if (value < 0 || IsErrorDateTimeSerial(value, date1904))
				NumericValue = value;
			else {
				DateTime dateTimeValue = FromDateTimeSerial(value, date1904);
				SetDateTime(dateTimeValue);
				this.type = XlVariantValueType.DateTime;
			}
		}
		public static XlVariantValue FromObject(object value) {
			if (value == null)
				return XlVariantValue.Empty;
			if (DXConvert.IsDBNull(value))
				return XlVariantValue.Empty;
			Type type = value.GetType();
			if (type == typeof(string))
				return (string)value;
			if (type == typeof(DateTime))
				return (DateTime)value;
			if (type == typeof(bool))
				return (bool)value;
			if (type == typeof(double))
				return Convert.ToDouble(value);
			if (type == typeof(int))
				return Convert.ToDouble(value);
			if (type == typeof(long))
				return Convert.ToDouble(value);
			if (type == typeof(decimal))
				return Convert.ToDouble(value);
			if (type == typeof(float)) {
				float floatValue = Convert.ToSingle(value);
				if (float.IsNaN(floatValue) || float.IsInfinity(floatValue))
					return ushort.MaxValue;
				if (DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(floatValue))
					floatValue = 0;
				return ConvertFloat(floatValue);
			}
			if (type == typeof(short))
				return Convert.ToDouble(value);
			if (type == typeof(byte))
				return Convert.ToDouble(value);
			if (type == typeof(ushort))
				return Convert.ToDouble(value);
			if (type == typeof(uint))
				return Convert.ToDouble(value);
			if (type == typeof(ulong))
				return Convert.ToDouble(value);
			if (type == typeof(TimeSpan))
				return (TimeSpan)value;
			return XlVariantValue.Empty;
		}
		static double ConvertFloat(float value) {
			if (value > (float)decimal.MaxValue)
				return (double)value;
			if (value < (float)decimal.MinValue)
				return (double)value;
			return (double)((decimal)value);
		}
		static XlVariantValue FromError(IXlCellError value) {
			XlVariantValue result = new XlVariantValue();
			result.type = XlVariantValueType.Error;
			result.referenceValue = value;
			return result;
		}
		internal DateTime GetDateTimeForMonthName() {
			double value = NumericValue;
			if (value < 2)
				return XlVariantValue.BaseDate.AddDays(2);
			if (value >= 60 && value < 61)
				return XlVariantValue.BaseDate.AddDays(59); 
			if (value < 61)
				return XlVariantValue.BaseDate + TimeSpan.FromDays(value + 1);
			else
				return XlVariantValue.BaseDate + TimeSpan.FromDays(value);
		}
		internal DateTime GetDateTimeForDayOfWeek() {
			return XlVariantValue.BaseDate + TimeSpan.FromDays(NumericValue);
		}
	}
	#endregion
}
