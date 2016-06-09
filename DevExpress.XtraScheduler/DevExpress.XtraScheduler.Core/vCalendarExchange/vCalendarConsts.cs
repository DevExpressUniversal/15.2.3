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
namespace DevExpress.XtraScheduler.VCalendar {
	public static class VCalendarTag {
		public const string Begin = "BEGIN:";
		public const string End = "END:";
		public const string Calendar = "VCALENDAR";
		public const string BeginCalendar = Begin + Calendar;
		public const string EndCalendar = End + Calendar;
		public const string Event = "VEVENT";
		public const string BeginEvent = Begin + Event;
		public const string EndEvent = End + Event;
	}
	public static class VCalendarPropertyNames {
		public const string DaylightSavingsRule = "DAYLIGHT";
		public const string GeographicPosition = "GEO";
		public const string ProductIdentifier = "PRODID";
		public const string TimeZone = "TZ";
		public const string Version = "VERSION";
	}
	public static class VEventPropertyNames {
		public const string ExtensionPrefix = "X-";
		public const string DtEnd = "DTEND";
		public const string DtStart = "DTSTART";
		public const string Summary = "SUMMARY";
		public const string Location = "LOCATION";
		public const string Description = "DESCRIPTION";
		public const string TimeTransparency = "TRANSP";
		public const string Priority = "PRIORITY";
		public const string RecurrenceRule = "RRULE";
		public const string ExceptionRule = "EXRULE";
		public const string ExceptionDateTimes = "EXDATE";
	}
	public static class VPropertyParameterNames {
		public const string Encoding = "ENCODING";
		public const string Charset = "CHARSET";
		public const string Language = "LANGUAGE";
	}
	public static class VEncodingParameterValues {
		public const string Default = "bit-7"; 
		public const string QuotedPrintable = "QUOTED-PRINTABLE";
		public const string Base64 = "BASE64";
		public const string Bit8 = "8-bit";
	}
	public static class VRecurrenceFrequencyTag {
		public const string Daily = "D";
		public const string Weekly = "W";
		public const string MonthlyByPosition = "MP";
		public const string MonthlyByDay = "MD";
		public const string YearlyByMonth = "YM";
		public const string YearlyByDay = "YD";
	}
}
