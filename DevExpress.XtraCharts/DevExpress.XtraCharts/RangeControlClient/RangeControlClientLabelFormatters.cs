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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.XtraCharts.Native {
	public class NumericLabelFormatter : IFormatProvider, ICustomFormatter {
		#region IFormatProvider
		object IFormatProvider.GetFormat(Type formatType) {
			if (formatType == typeof(ICustomFormatter))
				return this;
			return null;
		}
		#endregion
		#region ICustomFormatter
		string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider) {
			if (!Equals(formatProvider))
				return null;
			if (string.IsNullOrWhiteSpace(format)) {
				double value = Convert.ToDouble(arg);
				string autoFormat = "G";
				string suffix = "";
				if ((value >= 1000) && (value < 999999)) {
					autoFormat = "F1";
					suffix = "K";
					value /= 1000;
				}
				else if ((value >= 1000000) && (value < 999999999)) {
					autoFormat = "F1";
					suffix = "M";
					value /= 1000000;
				}
				else if (value >= 1000000000) {
					autoFormat = "F1";
					suffix = "B";
					value /= 1000000000;
				}
				return value.ToString(autoFormat, CultureInfo.InvariantCulture) + suffix;
			}
			try {
				return String.Format("{0:" + format + "}", arg);
			}
			catch {
				return String.Format("{0}", arg);
			}
		}
		#endregion
	}
	public class DateTimeLabelFormatter : IFormatProvider, ICustomFormatter {
		readonly ChartRangeControlClientDateTimeGridOptions gridOptions;
		public DateTimeLabelFormatter(ChartRangeControlClientDateTimeGridOptions gridOptions) {
			this.gridOptions = gridOptions;
		}
		#region IFormatProvider
		object IFormatProvider.GetFormat(Type formatType) {
			if (formatType == typeof(ICustomFormatter))
				return this;
			return null;
		}
		#endregion
		#region ICustomFormatter
		string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider) {
			if (!Equals(formatProvider))
				return null;
			if (string.IsNullOrWhiteSpace(format)) {
				DateTime value = Convert.ToDateTime(arg);
				DateTimeFormatInfo dateTimeInfo = DateTimeFormatInfo.CurrentInfo;
				string dateSeparator = dateTimeInfo.DateSeparator;
				string timeSeparator = dateTimeInfo.TimeSeparator;
				switch (gridOptions.GridAlignment) {
					default:
					case DateTimeGridAlignment.Millisecond:
						string timePattern = DateTimeFormatInfo.CurrentInfo.LongTimePattern;
						timePattern = Regex.Replace(timePattern, "(:ss|:s)", "$1.fff");
						return String.Format("{0:d} {0:" + timePattern + "}", value);
					case DateTimeGridAlignment.Second:
						return String.Format("{0:G}", value);
					case DateTimeGridAlignment.Minute:
						return String.Format("{0:g}", value);
					case DateTimeGridAlignment.Hour:
						return String.Format("{0:g}", value);
					case DateTimeGridAlignment.Day:
					case DateTimeGridAlignment.Week:
						return String.Format("{0:d}", value);
					case DateTimeGridAlignment.Quarter:
					case DateTimeGridAlignment.Month:
						return String.Format("{0:y}", value);
					case DateTimeGridAlignment.Year:
						return String.Format("{0:yyyy}", value);
				}
			}
			try {
				return String.Format("{0:" + format + "}", arg);
			}
			catch {
				return String.Format("{0}", arg);
			}
		}
		#endregion
	}
}
