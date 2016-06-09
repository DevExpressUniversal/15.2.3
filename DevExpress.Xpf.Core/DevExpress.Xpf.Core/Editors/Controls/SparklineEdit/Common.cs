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
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.Xpf.Editors.Internal {
	public interface ISupportNegativePointsControl {
		bool HighlightNegativePoints { get; set; }
	}
	public static class SparklineMathUtils {
		public static bool IsValidDouble(double value) {
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}
		public static int Round(double value) {
			return (int)Math.Round(value);
		}
		public static double? ConvertToDouble(object value, out SparklineScaleType scaleType) {
			if (value == null) {
				scaleType = SparklineScaleType.Unknown;
				return null;
			}
			Type valueType = value.GetType();
			if (valueType == typeof(string)) {
				double numericValue;
				DateTime dateTimeValue;
				if (double.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out numericValue)) {
					scaleType = SparklineScaleType.Numeric;
					return numericValue;
				}
				else if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue)) {
					scaleType = SparklineScaleType.DateTime;
					return dateTimeValue.Ticks;
				}
			}
			if (valueType == typeof(DateTime)) {
				scaleType = SparklineScaleType.DateTime;
				return ((DateTime)value).Ticks;
			}
			else if (valueType == typeof(char)) {
				scaleType = SparklineScaleType.Numeric;
				return Convert.ToInt32(value);
			}
			else if (valueType == typeof(double) || valueType == typeof(float) || valueType == typeof(int) ||
				valueType == typeof(uint) || valueType == typeof(long) || valueType == typeof(ulong) ||
				valueType == typeof(decimal) || valueType == typeof(short) || valueType == typeof(ushort) ||
				valueType == typeof(byte) || valueType == typeof(sbyte)) {
				scaleType = SparklineScaleType.Numeric;
				return Convert.ToDouble(value);
			}
			scaleType = SparklineScaleType.Unknown;
			return null;
		}
		public static object ConvertToNative(double value, SparklineScaleType scaleType) {
			switch (scaleType) {
				case SparklineScaleType.DateTime:
					return new DateTime(Convert.ToInt64(value));
				case SparklineScaleType.Numeric:
				case SparklineScaleType.Unknown:
				default:
					return value;
			}
		}
	}
}
namespace DevExpress.Xpf.Editors {
	public enum SparklineViewType {
		Line,
		Area,
		Bar,
		WinLoss
	}
	public class StringToObjectConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return value;
		}
	}
}
