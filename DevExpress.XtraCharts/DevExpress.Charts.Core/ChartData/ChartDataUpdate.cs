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
using System.Globalization;
namespace DevExpress.Charts.Native {
	public enum ChartCollectionOperation {
		Clear,
		Reset,
		InsertItem,
		RemoveItem,
		UpdateItem,
		MoveItem,
		SwapItem
	}
	public static class DataItemsHelper {
		public const ActualScaleType DefaultScaleType = ActualScaleType.Numerical;
		public static bool IsNumericalType(Type type) {
			return type == typeof(double) || type == typeof(float) || type == typeof(int) || type == typeof(uint)
				|| type == typeof(long) || type == typeof(ulong) || type == typeof(decimal)
				|| type == typeof(short) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte);
		}
		public static Type GetTypeByScaleType(ActualScaleType scaleType) {
			switch (scaleType) {
				case ActualScaleType.Numerical:
					return typeof(double);
				case ActualScaleType.DateTime:
					return typeof(DateTime);
				default:
					return typeof(string);
			}
		}
		public static ActualScaleType GetScaleTypeByType(Type type) {
			if (type == typeof(DateTime))
				return ActualScaleType.DateTime;
			else if (IsNumericalType(type))
				return ActualScaleType.Numerical;
			return ActualScaleType.Qualitative;
		}
		public static Scale ParseValue(object value, out double numericalValue, out DateTime dateTimeValue, out string qualitativeValue) {
			if (value == null) {
				numericalValue = default(double);
				dateTimeValue = default(DateTime);
				qualitativeValue = default(string);
				return Scale.Auto;
			}
			Type type = value.GetType();
			if (type == typeof(DateTime)) {
				dateTimeValue = (DateTime)value;
				qualitativeValue = dateTimeValue.ToString(CultureInfo.InvariantCulture);
				numericalValue = default(double);
				return Scale.DateTime;
			} else {
				if (IsNumericalType(type)) {
					numericalValue = Convert.ToDouble(value);
					qualitativeValue = numericalValue.ToString(CultureInfo.InvariantCulture);
					dateTimeValue = default(DateTime);
					return Scale.Numerical;
				}
				qualitativeValue = value.ToString();
				return ParseValue(qualitativeValue, out numericalValue, out dateTimeValue);
			}
		}
		public static Scale ParseValue(string value, out double numericalValue, out DateTime dateTimeValue) {
			if (value == null) {
				numericalValue = default(double);
				dateTimeValue = default(DateTime);
				return Scale.Auto;
			}
			if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out numericalValue)) {
				dateTimeValue = default(DateTime);
				return Scale.Numerical;
			}
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue)) {
				numericalValue = default(double);
				return Scale.DateTime;
			}
			return Scale.Qualitative;
		}
		public static double? ParseNumerical(object value) {
			if (value != null) {
				if (IsNumericalType(value.GetType()))
					return Convert.ToDouble(value);
				double numericalValue;
				if (double.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out numericalValue))
					return numericalValue;
			}
			return null;
		}
		public static DateTime? ParseDateTime(object value) {
			if (value != null) {
				if (value is DateTime)
					return (DateTime)value;
				DateTime dateTimeValue;
				if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue))
					return dateTimeValue;
			}
			return null;
		}
		public static double ConvertToDouble(object value) {
			if (value != null) {
				if (IsNumericalType(value.GetType()))
					return Convert.ToDouble(value);
			}
			return double.NaN;
		}
		public static DateTime ConvertToDateTime(object value) {
			if (value != null && value is DateTime)
				return (DateTime)value;
			return DateTime.MinValue;
		}
		public static ActualScaleType ConvertToActualScaleType(Scale scaleType) {
			switch (scaleType) {
				case Scale.DateTime:
					return ActualScaleType.DateTime;
				case Scale.Qualitative:
					return ActualScaleType.Qualitative;
				default:
					return ActualScaleType.Numerical;
			}
		}
		public static Scale ConvertToScaleType(ActualScaleType scaleType) {
			switch (scaleType) {
				case ActualScaleType.DateTime:
					return Scale.DateTime;
				case ActualScaleType.Qualitative:
					return Scale.Qualitative;
				default:
					return Scale.Numerical;
			}
		}
	}
}
