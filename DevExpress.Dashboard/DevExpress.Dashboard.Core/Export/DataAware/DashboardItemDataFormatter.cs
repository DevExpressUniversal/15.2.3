#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public static class DashboardItemDataFormatter {
		static bool SupportDateTimeAsInteger(DateTimeFormatViewModel dateTimeFormat) {
			if(dateTimeFormat == null)
				return false;
			switch(dateTimeFormat.GroupInterval) {
				case DateTimeGroupInterval.Day:
				case DateTimeGroupInterval.DayOfYear:
				case DateTimeGroupInterval.WeekOfMonth:
				case DateTimeGroupInterval.WeekOfYear:
				case DateTimeGroupInterval.Minute:
				case DateTimeGroupInterval.Year:
				case DateTimeGroupInterval.Second:
					return true;
				default:
					return false;
			}
		}
		static void PrepareMeasureFormatSettings(FormatSettings settings, NumericFormatViewModel format) {
			settings.FormatString = NumericFormatter.CreateInstance(format).FormatPattern;
			settings.ActualDataType = typeof(float);
		}
		public static FormatSettings CreateValueFormatSettings(ValueFormatViewModel valueFormat) {
			FormatSettings formatSettings = new FormatSettings();
			switch(valueFormat.DataType) {
				case ValueDataType.DateTime: {
					if (SupportFormatSettings(valueFormat.DateTimeFormat)) {
						Type actualDataType;
						switch (valueFormat.DataType) {
							case ValueDataType.DateTime:
								actualDataType = SupportDateTimeAsInteger(valueFormat.DateTimeFormat) ? typeof(int) : typeof(DateTime);
								break;
							case ValueDataType.Numeric:
								actualDataType = typeof(float);
								break;
							default:
								actualDataType = typeof(string);
								break;
						}
						formatSettings.ActualDataType = actualDataType;
						formatSettings.FormatString = FormatterBase.CreateFormatter(valueFormat).FormatPattern;
					}
					break;
				}
				case ValueDataType.Numeric:
					PrepareMeasureFormatSettings(formatSettings, valueFormat.NumericFormat);
					break;
				default:
					formatSettings.ActualDataType = typeof(string);
					break;
			}
			return formatSettings;
		}
		public static FormatSettings CreateNumericFormatSettings(NumericFormatViewModel format) {
			FormatSettings settings = new FormatSettings();
			PrepareMeasureFormatSettings(settings, format);
			return settings;
		}
		public static bool SupportFormatSettings(DateTimeFormatViewModel dateTimeFormat) {
			if (dateTimeFormat == null)
				return false;
			switch (dateTimeFormat.GroupInterval) {
				case DateTimeGroupInterval.Month:
				case DateTimeGroupInterval.Quarter:
				case DateTimeGroupInterval.QuarterYear:
					return false;
				default:
					return true;
			}
		}
		public static NumericFormatViewModel GetDeltaFormat(DeltaDescriptor delta, DeltaValueType valueType) {
			NumericFormatViewModel format;
			switch(valueType) {
				case DeltaValueType.AbsoluteVariation:
					format = delta.InternalDescriptor.AbsoluteVariationFormat;
					break;
				case DeltaValueType.PercentOfTarget:
					format = delta.InternalDescriptor.PercentOfTargetFormat;
					break;
				case DeltaValueType.PercentVariation:
					format = delta.InternalDescriptor.PercentVariationFormat;
					break;
				default:
					format = delta.InternalDescriptor.ActualValueFormat;
					break;
			}
			return format;
		}
		public static FormatSettings CreateDeltaFormatSettings(DeltaDescriptor delta, DeltaValueType valueType) {
			return CreateNumericFormatSettings(GetDeltaFormat(delta, valueType));
		}
	}
}
