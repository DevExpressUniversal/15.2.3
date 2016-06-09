#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.ExpressApp.Kpi {
	public static class DateTimeHelper {
		public enum RoundTo {
			Second, Minute, Hour, Day
		}
		public static DayOfWeek WeekStartDay = DayOfWeek.Monday;
		public static int GetDayOfWeek(DayOfWeek dayOfWeek) {
			int result = (int)dayOfWeek;
			if(dayOfWeek >= WeekStartDay) {
				result = result - (int)WeekStartDay;
			}
			else {
				result = result + 7 - (int)WeekStartDay;
			}
			return result;
		}
		public static DateTime Round(DateTime d, RoundTo rt) {
			DateTime dtRounded = new DateTime();
			switch(rt) {
				case RoundTo.Second:
					dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
					if(d.Millisecond >= 500) dtRounded = dtRounded.AddSeconds(1);
					break;
				case RoundTo.Minute:
					dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
					if(d.Second >= 30) dtRounded = dtRounded.AddMinutes(1);
					break;
				case RoundTo.Hour:
					dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0);
					if(d.Minute >= 30) dtRounded = dtRounded.AddHours(1);
					break;
				case RoundTo.Day:
					dtRounded = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
					if(d.Hour >= 12) dtRounded = dtRounded.AddDays(1);
					break;
			}
			return dtRounded;
		}
	}
}
