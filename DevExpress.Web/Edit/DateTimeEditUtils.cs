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
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public static class DateTimeEditUtils {
		static readonly DateTime UnixEpochStart = new DateTime(1970, 1, 1);
		public static string GetTextProperty(ASPxTextEdit editor, string dateFormat, string baseText) {
			if(editor.Value is DateTime) {
				DateTime date = (DateTime)editor.Value;
				if(date == DateTime.MinValue)
					return "";
				return date.ToString(dateFormat);
			}
			return baseText;
		}
		public static void SetTextProperty(ASPxTextEdit editor, string text, string dateFormat) {
			DateTime date;
			if(!DateTime.TryParseExact(text, dateFormat, null, DateTimeStyles.None, out date))
				date = DateTime.MinValue;
			editor.Value = date;
		}
		public static DateTime GetDateProperty(ASPxTextEdit editor) {
			if(editor.Value is DateTime)
				return (DateTime)editor.Value;
			return DateTime.MinValue;
		}
		public static void SetDateProperty(ASPxTextEdit editor, DateTime date) {
			if(date == DateTime.MinValue)
				editor.Value = null;
			else
				editor.Value = date;
		}
		public static object PreprocessValueProperty(object value) {			
			if(value is DateTime)
				return value;
			DateTime date;
			if(value != null && DateTime.TryParseExact(value.ToString(), "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
				return date;			
			return value;
		}
		public static void NotifyOwnerComponentChanged(IPropertiesOwner owner) {
			IComponent component = owner as IComponent;
			if(component == null) {
				CollectionItem item = owner as CollectionItem;
				if(item != null && item.Collection != null)
					component = item.Collection.Owner as IComponent;
			}
			if(component != null && component.Site != null)
				DesignServices.ComponentChanged(component.Site, component);
		}
		static string ExpandSingleCharFormat(string format) {
			DateTimeFormatInfo info = CultureInfo.CurrentCulture.DateTimeFormat;
			switch(format) {
				case "d":
					return info.ShortDatePattern;
				case "D":
					return info.LongDatePattern;
				case "t":
					return info.ShortTimePattern;
				case "T":
					return info.LongTimePattern;
				case "f":
					return info.LongDatePattern + " " + info.ShortTimePattern;
				case "F":
					return info.FullDateTimePattern;
				case "g":
					return info.ShortDatePattern + " " + info.ShortTimePattern;
				case "G":
					return info.ShortDatePattern + " " + info.LongTimePattern;
				case "M":
				case "m":
					return info.MonthDayPattern;
				case "r":
				case "R":
					return info.RFC1123Pattern;
			}
			return format;
		}
		public static string ToJsMilliseconds(DateTime time) {
			return new TimeSpan(time.Ticks - UnixEpochStart.Ticks).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
		}
		public static DateTime FromJsMilliseconds(string value) {
			return UnixEpochStart.AddMilliseconds(Math.Floor(double.Parse(value, CultureInfo.InvariantCulture)));
		}
		public static string GetRawValue(DateTime date){
			if(date > DateTime.MinValue)
				return DateTimeEditUtils.ToJsMilliseconds(date);
			return "N";
		}
		public static object ParseRawInputValue(string value) {
			if(value == "N")
				return null;
			return FromJsMilliseconds(value);
		}
		public static string GetDateFormatString(EditFormat editFormat, string editFormatString, string defaultFormat) {
			string format = null;
			if(!string.IsNullOrEmpty(editFormatString)) {
				format = editFormatString;
			} else {
				switch(editFormat) {
					case EditFormat.Date:
						format = "d";
						break;
					case EditFormat.DateTime:
						format = "g";
						break;
					case EditFormat.Time:
						format = "t";
						break;
				}
			}
			if(string.IsNullOrEmpty(format))
				format = defaultFormat;
			else if(format.Length == 1)
				format = ExpandSingleCharFormat(format);
			return format;
		}
	}
}
