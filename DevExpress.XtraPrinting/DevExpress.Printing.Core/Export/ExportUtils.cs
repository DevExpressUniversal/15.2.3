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
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Export;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraExport {
	public static class ExportUtils {
		static string dateTimeFormat;
		static string DateTimeFormat {
			get {
				if(string.IsNullOrEmpty(dateTimeFormat))
					dateTimeFormat = GetDateTimeFormat(DateTimeFormatInfo.CurrentInfo);
				return dateTimeFormat;
			}
		}
		static string GetDateTimeFormat(DateTimeFormatInfo formatInfo) {
			string s =  formatInfo.ShortDatePattern.ToLower() + string.Format(" h{0}mm{0}ss", formatInfo.TimeSeparator);
			if(!string.IsNullOrEmpty(formatInfo.AMDesignator))
				s += " AM/PM";
			s += " [${0}{1:00}" + formatInfo.TimeSeparator + "{2:00}]";
			return s;
		}
		public static string GetDateTimeFormatString(TimeSpan timeSpan) {
			return string.Format(DateTimeFormat, ToSign(timeSpan.Ticks), timeSpan.Hours, timeSpan.Minutes);
		}
		static string ToSign(long value) {
			return value >= 0 ? "+" : "-";
		} 
		public static double ToOADate(DateTime value) {
			try {
				return value.ToOADate();
			} catch(OverflowException) {
				return new DateTime().ToOADate();
			}
		}
		public static DateTime TimeSpanStartDate { get { return new DateTime(1899, 12, 30); } }
		public static bool IsIntegerValue(object data) {
			return
				data is System.Int16 ||
				data is System.UInt16 ||
				data is System.Int32 ||
				data is System.UInt32 ||
				data is System.Int64 ||
				data is System.UInt64 ||
				data is System.SByte ||
				data is System.Byte;
		}
		public static bool IsDoubleValue(object data) {
			return
				data is System.Single || 
				data is System.Double || 
				data is System.Decimal;
		}
		public static bool AllowNewExcelExportEx(IDataAwareExportOptions options,ExportTarget target) {
			bool allowNewExport = options != null ? options.ExportType == ExportType.DataAware : ExportSettings.DefaultExportType == ExportType.DataAware;
			switch(target){
				case ExportTarget.Xlsx:
				case ExportTarget.Xls:
				case ExportTarget.Csv:
					return allowNewExport;
				default:
					return false;
			}
		}
	}
}
