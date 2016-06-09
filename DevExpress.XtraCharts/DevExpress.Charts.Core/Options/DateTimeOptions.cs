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
using DevExpress.Utils;
namespace DevExpress.Charts.Native {
	public enum DateTimeOptionsFormat {
		ShortDate,
		LongDate,
		ShortTime,
		LongTime,
		General,
		MonthAndDay,
		MonthAndYear,
		QuarterAndYear,
		Custom
	}
	public interface IDateTimeOptions {
		DateTimeOptionsFormat Format { get; }
		string FormatString { get; }
		string QuarterFormat { get; }
	}
	public static class DateTimeOptionsHelper {
		public static string GetValueText(DateTime dateTime, IDateTimeOptions options) {
			DateTimeOptionsFormat format = options == null ? DateTimeOptionsFormat.General : options.Format;
			switch (format) {
				case DateTimeOptionsFormat.ShortDate:
					return dateTime.ToShortDateString();
				case DateTimeOptionsFormat.LongDate:
					return dateTime.ToLongDateString();
				case DateTimeOptionsFormat.ShortTime:
					return dateTime.ToShortTimeString();
				case DateTimeOptionsFormat.LongTime:
					return dateTime.ToLongTimeString();
				case DateTimeOptionsFormat.General:
					return dateTime.ToString();
				case DateTimeOptionsFormat.MonthAndDay:
					return dateTime.ToString("m");
				case DateTimeOptionsFormat.MonthAndYear:
					return dateTime.ToString("y");
				case DateTimeOptionsFormat.QuarterAndYear:
					return dateTime.ToString(QuarterFormatter.FormatDateTime(dateTime, "QQ ", options.QuarterFormat) + "yyyy");	  
				case DateTimeOptionsFormat.Custom: 
					try {
						return dateTime.ToString(QuarterFormatter.FormatDateTime(dateTime, options.FormatString, options.QuarterFormat), 
							CultureInfo.CurrentCulture);
					} 
					catch(FormatException) {
						return dateTime.ToString(String.Empty, CultureInfo.CurrentCulture);
					}
				default:
					ChartDebug.Fail("Incorrect format.");
					return dateTime.ToString(String.Empty, CultureInfo.CurrentCulture);
			}
		}
		public static string GetFormatString(IDateTimeOptions options) {
			DateTimeOptionsFormat format = options == null ? DateTimeOptionsFormat.General : options.Format;
			switch (format) {
				case DateTimeOptionsFormat.ShortDate:
					return "d";
				case DateTimeOptionsFormat.LongDate:
					return "D";
				case DateTimeOptionsFormat.ShortTime:
					return "t";
				case DateTimeOptionsFormat.LongTime:
					return "T";
				case DateTimeOptionsFormat.MonthAndDay:
					return "m";
				case DateTimeOptionsFormat.MonthAndYear:
					return "y";
				case DateTimeOptionsFormat.Custom:
					return options.FormatString;
				case DateTimeOptionsFormat.QuarterAndYear:
					return "q";
				case DateTimeOptionsFormat.General:
				default:
					return "g";
			}
		}
	}
}
