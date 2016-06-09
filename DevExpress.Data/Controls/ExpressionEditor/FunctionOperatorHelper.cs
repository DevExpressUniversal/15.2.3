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
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;
using DevExpress.Xpo.DB;
using System.ComponentModel;
using System.Linq;
using DevExpress.Utils;
namespace DevExpress.Data.Filtering.Helpers {
	using System.Collections.Generic;
	using System.Text;
	using DevExpress.Data.Filtering;
	using System.ComponentModel;
#if !CF
	using DevExpress.Data.ExpressionEditor;
	using System.Threading;
#endif
	public class FunctionOperatorHelper {
#if !CF
		static Dictionary<FunctionOperatorType, Dictionary<int, ItemClickHelper.FunctionInfo>> functionInfoStaticDict = new Dictionary<FunctionOperatorType, Dictionary<int, ItemClickHelper.FunctionInfo>>();
		static List<ItemClickHelper.FunctionInfo> aggregateFunctionInfo = new List<ItemClickHelper.FunctionInfo>();
		static FunctionOperatorHelper() {
			Dictionary<int, ItemClickHelper.FunctionInfo> argumentsDict;
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeDayAfterTomorrow()", "LocalDateTimeDayAfterTomorrow.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeDayAfterTomorrow, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeLastWeek()", "LocalDateTimeLastWeek.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeLastWeek, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeNextMonth()", "LocalDateTimeNextMonth.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeNextMonth, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeNextWeek()", "LocalDateTimeNextWeek.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeNextWeek, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeNextYear()", "LocalDateTimeNextYear.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeNextYear, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeNow()", "LocalDateTimeNow.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeNow, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeThisMonth()", "LocalDateTimeThisMonth.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeThisMonth, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeThisWeek()", "LocalDateTimeThisWeek.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeThisWeek, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeThisYear()", "LocalDateTimeThisYear.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeThisYear, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeToday()", "LocalDateTimeToday.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeToday, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeTomorrow()", "LocalDateTimeTomorrow.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeTomorrow, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeTwoWeeksAway()", "LocalDateTimeTwoWeeksAway.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeTwoWeeksAway, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("LocalDateTimeYesterday()", "LocalDateTimeYesterday.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.LocalDateTimeYesterday, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Abs()", "Abs.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Abs, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Acos()", "Acos.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Acos, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddDays(, )", "AddDays.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddDays, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddHours(, )", "AddHours.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddHours, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddMilliSeconds(, )", "AddMilliSeconds.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddMilliSeconds, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddMinutes(, )", "AddMinutes.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddMinutes, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddMonths(, )", "AddMonths.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddMonths, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddSeconds(, )", "AddSeconds.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddSeconds, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddTicks(, )", "AddTicks.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddTicks, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddTimeSpan(, )", "AddTimeSpan.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddTimeSpan, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("IsThisWeek()", "IsThisWeek.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.IsThisWeek, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("IsThisMonth()", "IsThisMonth.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.IsThisMonth, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("IsThisYear()", "IsThisYear.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.IsThisYear, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("AddYears(, )", "AddYears.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.AddYears, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Ascii('')", "Ascii.Description", 2, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Ascii, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Asin()", "Asin.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Asin, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Atn()", "Atn.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Atn, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Atn2(, )", "Atn2.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Atn2, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("BigMul(, )", "BigMul.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.BigMul, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Ceiling()", "Ceiling.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Ceiling, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Char()", "Char.Description", 1, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Char, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("CharIndex('', '')", "CharIndex.Description", 6, FunctionEditorCategory.String));
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("CharIndex('', '', )", "CharIndex3Param.Description", 8, FunctionEditorCategory.String));
			argumentsDict.Add(4, null);
			functionInfoStaticDict.Add(FunctionOperatorType.CharIndex, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, null);
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Concat(, )", "Concat.Description", 3, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Concat, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Cos()", "Cos.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Cos, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Cosh()", "Cosh.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Cosh, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffDay(, )", "DateDiffDay.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffDay, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffHour(, )", "DateDiffHour.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffHour, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffMilliSecond(, )", "DateDiffMilliSecond.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffMilliSecond, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffMinute(, )", "DateDiffMinute.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffMinute, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffMonth(, )", "DateDiffMonth.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffMonth, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffSecond(, )", "DateDiffSecond.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffSecond, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffTick(, )", "DateDiffTick.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffTick, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("DateDiffYear(, )", "DateDiffYear.Description", 3, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.DateDiffYear, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Exp()", "Exp.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Exp, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Floor()", "Floor.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Floor, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetDate()", "GetDate.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetDate, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetDay()", "GetDay.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetDay, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetDayOfWeek()", "GetDayOfWeek.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetDayOfWeek, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetDayOfYear()", "GetDayOfYear.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetDayOfYear, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetHour()", "GetHour.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetHour, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetMilliSecond()", "GetMilliSecond.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetMilliSecond, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetMinute()", "GetMinute.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetMinute, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetMonth()", "GetMonth.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetMonth, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetSecond()", "GetSecond.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetSecond, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetTimeOfDay()", "GetTimeOfDay.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetTimeOfDay, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("GetYear()", "GetYear.Description", 1, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.GetYear, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("Iif(,  , )", "Iif.Description", 6, FunctionEditorCategory.Logical));
			functionInfoStaticDict.Add(FunctionOperatorType.Iif, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("Insert('', , '')", "Insert.Description", 8, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Insert, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("IsNull()", "IsNull.Description", 1, FunctionEditorCategory.Logical));
			argumentsDict.Add(2, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsNull, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("IsNullOrEmpty()", "IsNullOrEmpty.Description", 1, FunctionEditorCategory.Logical));
			functionInfoStaticDict.Add(FunctionOperatorType.IsNullOrEmpty, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Len()", "Len.Description", 1, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Len, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Lower()", "Lower.Description", 1, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Lower, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Log()", "Log.Description", 1, FunctionEditorCategory.Math));
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Log(, )", "Log2Param.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Log, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Log10()", "Log10.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Log10, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Max(, )", "Max.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Max, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Min(, )", "Min.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Min, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("Now()", "Now.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.Now, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("PadLeft(, )", "PadLeft.Description", 3, FunctionEditorCategory.String));
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("PadLeft(, , '')", "PadLeft3Param.Description", 7, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.PadLeft, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("PadRight(, )", "PadRight.Description", 3, FunctionEditorCategory.String));
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("PadRight(, , '')", "PadRight3Param.Description", 7, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.PadRight, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Power(, )", "Power.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Power, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Remove('', )", "Remove2Param.Description", 4, FunctionEditorCategory.String));
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("Remove('', , )", "Remove3Param.Description", 6, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Remove, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("Replace('', '', '')", "Replace.Description", 10, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Replace, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Reverse('')", "Reverse.Description", 2, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Reverse, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("Rnd()", "Rnd.Description", 0, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Rnd, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Round()", "Round.Description", 1, FunctionEditorCategory.Math));
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Round(, )", "Round2Param.Description", 3, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Round, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Sign()", "Sign.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Sign, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Sin()", "Sin.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Sin, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Sinh()", "Sinh.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Sinh, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Sqr()", "Sqr.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Sqr, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(3, new ItemClickHelper.FunctionInfo("Substring('',  , )", "Substring3param.Description", 7, FunctionEditorCategory.String));
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Substring('', )", "Substring2param.Description", 4, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Substring, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Tan()", "Tan.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Tan, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Tanh()", "Tanh.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.Tanh, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("Today()", "Today.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.Today, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("ToInt()", "ToInt.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.ToInt, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("ToLong()", "ToLong.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.ToLong, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("ToFloat()", "ToFloat.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.ToFloat, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("ToDouble()", "ToDouble.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.ToDouble, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("ToDecimal()", "ToDecimal.Description", 1, FunctionEditorCategory.Math));
			functionInfoStaticDict.Add(FunctionOperatorType.ToDecimal, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("ToStr()", "ToStr.Description", 1, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.ToStr, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Trim()", "Trim.Description", 1, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Trim, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(1, new ItemClickHelper.FunctionInfo("Upper()", "Upper.Description", 1, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Upper, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(0, new ItemClickHelper.FunctionInfo("UtcNow()", "UtcNow.Description", 0, FunctionEditorCategory.DateTime));
			functionInfoStaticDict.Add(FunctionOperatorType.UtcNow, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("StartsWith('', '')", "StartsWith.Description", 6, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.StartsWith, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("EndsWith('', '')", "EndsWith.Description", 6, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.EndsWith, argumentsDict);
			argumentsDict = new Dictionary<int, ItemClickHelper.FunctionInfo>();
			argumentsDict.Add(2, new ItemClickHelper.FunctionInfo("Contains('', '')", "Contains.Description", 6, FunctionEditorCategory.String));
			functionInfoStaticDict.Add(FunctionOperatorType.Contains, argumentsDict);
			functionInfoStaticDict.Add(FunctionOperatorType.Custom, null);
			functionInfoStaticDict.Add(FunctionOperatorType.CustomNonDeterministic, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalBeyondThisYear, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalEarlierThisMonth, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalEarlierThisWeek, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalEarlierThisYear, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalLastWeek, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalLaterThisMonth, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalLaterThisWeek, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalLaterThisYear, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalNextWeek, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalPriorThisYear, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalToday, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalTomorrow, null);
			functionInfoStaticDict.Add(FunctionOperatorType.IsOutlookIntervalYesterday, null);
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Avg()", "AvgAggregate.Description", 1, FunctionEditorCategory.Aggregate));
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Count()", "CountAggregate.Description", 0, FunctionEditorCategory.Aggregate));
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Exists()", "ExistsAggregate.Description", 0, FunctionEditorCategory.Aggregate));
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Max()", "MaxAggregate.Description", 1, FunctionEditorCategory.Aggregate));
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Min()", "MinAggregate.Description", 1, FunctionEditorCategory.Aggregate));
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Sum()", "SumAggregate.Description", 1, FunctionEditorCategory.Aggregate));
			aggregateFunctionInfo.Add(new ItemClickHelper.FunctionInfo("Single()", "SingleAggregate.Description", 0, FunctionEditorCategory.Aggregate));
		}
		class FunctionInfoComparer : IComparer<ItemClickHelper.FunctionInfo> {
			int IComparer<ItemClickHelper.FunctionInfo>.Compare(ItemClickHelper.FunctionInfo x, ItemClickHelper.FunctionInfo y) {
				if(x == null && y == null) return 0;
				if(x == null) return 1;
				if(y == null) return -1;
				return string.Compare(x.Function, y.Function);
			}
		}
		public static ItemClickHelper.FunctionInfo GetFunctionInfo(IExpressionEditor editor, FunctionOperatorType functionType, int argumentsCount) {
			Dictionary<int, ItemClickHelper.FunctionInfo> argumentsDict;
			if(!functionInfoStaticDict.TryGetValue(functionType, out argumentsDict)) throw new InvalidOperationException(string.Format("FunctionInfo not found for function type '{0}'", functionType.ToString()));
			if(argumentsDict == null) return null;
			ItemClickHelper.FunctionInfo stubFunctionInfo;
			if(!argumentsDict.TryGetValue(argumentsCount, out stubFunctionInfo)) throw new InvalidOperationException(string.Format("FunctionInfo not found for function type '{0}' and arguments count: {1} ", functionType.ToString(), argumentsCount));
			if(stubFunctionInfo == null) return null;
			return GetLocalizedFunctionInfo(editor, stubFunctionInfo);
		}
		static ItemClickHelper.FunctionInfo GetLocalizedFunctionInfo(IExpressionEditor editor, ItemClickHelper.FunctionInfo functionInfo) {
			return new ItemClickHelper.FunctionInfo(functionInfo.Function, editor.GetResourceString(functionInfo.Description), functionInfo.CursorOffset, functionInfo.Category);
		}
		public static void GetAllCustomFunctionInfo(IExpressionEditor editor, List<ItemClickHelper.FunctionInfo> result) {
			if(CriteriaOperator.CustomFunctionCount == 0) return;
			foreach(ICustomFunctionOperator customFunction in CriteriaOperator.GetCustomFunctions()) {
				ICustomFunctionOperatorBrowsable customFunctionBrowsable = customFunction as ICustomFunctionOperatorBrowsable;
				if(customFunctionBrowsable == null) {
					if(customFunction is LikeCustomFunction)
						continue;
					result.Add(new ItemClickHelper.FunctionInfo(customFunction.Name + "()", string.Empty, 0, FunctionEditorCategory.All));
					continue;
				}
				int minArgs = customFunctionBrowsable.MinOperandCount < 0 ? 0 : customFunctionBrowsable.MinOperandCount;
				int maxArgs = customFunctionBrowsable.MaxOperandCount < 0 ? minArgs : customFunctionBrowsable.MaxOperandCount;
				string description = customFunctionBrowsable.Description;
				FunctionCategory category = customFunctionBrowsable.Category;
				StringBuilder stringBuilder = new StringBuilder();
				for(int i = minArgs; i <= maxArgs; i++) {
					stringBuilder.Append(customFunctionBrowsable.Name + "(");
					int startLength = stringBuilder.Length;
					for(int j = 0; j < i; j++) {
						if(j > 0) stringBuilder.Append(", ");
						if(customFunctionBrowsable.IsValidOperandType(j, i, typeof(string)) && !customFunctionBrowsable.IsValidOperandType(j, i, typeof(object))) {
							if(j == 0) startLength++;
							stringBuilder.Append("''");
						}
					}
					stringBuilder.Append(")");
					int offset = (i == 0) ? 0 : stringBuilder.Length - startLength;
					result.Add(new ItemClickHelper.FunctionInfo(stringBuilder.ToString(), description, offset, ToFunctionEditorCategory(category)));
					stringBuilder.Remove(0, stringBuilder.Length);
				}
			}
		}
		static FunctionEditorCategory ToFunctionEditorCategory(FunctionCategory category) {
			switch(category) {
				case FunctionCategory.All:
					return FunctionEditorCategory.All;
				case FunctionCategory.DateTime:
					return FunctionEditorCategory.DateTime;
				case FunctionCategory.Logical:
					return FunctionEditorCategory.Logical;
				case FunctionCategory.Math:
					return FunctionEditorCategory.Math;
				case FunctionCategory.Text:
					return FunctionEditorCategory.String;
			}
			throw new ArgumentException("category");
		}
		public static ItemClickHelper.FunctionInfo[] GetAllFunctionInfo(IExpressionEditor editor) {
			List<ItemClickHelper.FunctionInfo> result = new List<ItemClickHelper.FunctionInfo>();
			foreach(FunctionOperatorType functionType in DevExpress.Data.Utils.Helpers.GetEnumValues<FunctionOperatorType>()) {
				if(functionType == FunctionOperatorType.None || functionType == FunctionOperatorType.CustomNonDeterministic) continue;
				if(functionType == FunctionOperatorType.Custom) {
					GetAllCustomFunctionInfo(editor, result);
					continue;
				}
				int[] argumentsCount = GetFunctionArgumentsCount(functionType);
				if(argumentsCount == null) throw new InvalidOperationException(string.Format("Arguments count information not found for function type '{0}'", functionType.ToString()));
				for(int i = 0; i < argumentsCount.Length; i++) {
					if(argumentsCount[i] < 0) continue;
					ItemClickHelper.FunctionInfo functionInfo = GetFunctionInfo(editor, functionType, argumentsCount[i]);
					if(functionInfo == null) continue;
					result.Add(functionInfo);
				}
			}
			result.AddRange(aggregateFunctionInfo.ConvertAll(info => GetLocalizedFunctionInfo(editor, info)));
			result.Sort(new FunctionInfoComparer());
			return result.ToArray();
		}
#endif
		public static bool IsValidCustomFunctionArgumentsCount(string functionName, int argumentsCount) {
			if(CriteriaOperator.CustomFunctionCount == 0) return true;
			ICustomFunctionOperatorBrowsable customFunction = CriteriaOperator.GetCustomFunction(functionName) as ICustomFunctionOperatorBrowsable;
			if(customFunction == null) return true;
			return customFunction.IsValidOperandCount(argumentsCount);
		}
		public static int[] GetFunctionArgumentsCount(FunctionOperatorType functionType) {
			switch(functionType) {
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
				case FunctionOperatorType.LocalDateTimeLastWeek:
				case FunctionOperatorType.LocalDateTimeNextMonth:
				case FunctionOperatorType.LocalDateTimeNextWeek:
				case FunctionOperatorType.LocalDateTimeNextYear:
				case FunctionOperatorType.LocalDateTimeNow:
				case FunctionOperatorType.LocalDateTimeThisMonth:
				case FunctionOperatorType.LocalDateTimeThisWeek:
				case FunctionOperatorType.LocalDateTimeThisYear:
				case FunctionOperatorType.LocalDateTimeToday:
				case FunctionOperatorType.LocalDateTimeTomorrow:
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
				case FunctionOperatorType.LocalDateTimeYesterday:
				case FunctionOperatorType.Now:
				case FunctionOperatorType.UtcNow:
				case FunctionOperatorType.Today:
				case FunctionOperatorType.Rnd:
					return new int[] { 0 };
				case FunctionOperatorType.IsNullOrEmpty:
				case FunctionOperatorType.Trim:
				case FunctionOperatorType.Upper:
				case FunctionOperatorType.Lower:
				case FunctionOperatorType.Len:
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.Ascii:
				case FunctionOperatorType.Char:
				case FunctionOperatorType.ToInt:
				case FunctionOperatorType.ToLong:
				case FunctionOperatorType.ToFloat:
				case FunctionOperatorType.ToDouble:
				case FunctionOperatorType.ToDecimal:
				case FunctionOperatorType.ToStr:
				case FunctionOperatorType.Reverse:
				case FunctionOperatorType.Abs:
				case FunctionOperatorType.Sqr:
				case FunctionOperatorType.Cos:
				case FunctionOperatorType.Sin:
				case FunctionOperatorType.Atn:
				case FunctionOperatorType.Exp:
				case FunctionOperatorType.Log10:
				case FunctionOperatorType.Tan:
				case FunctionOperatorType.Sign:
				case FunctionOperatorType.Ceiling:
				case FunctionOperatorType.Floor:
				case FunctionOperatorType.Asin:
				case FunctionOperatorType.Acos:
				case FunctionOperatorType.Cosh:
				case FunctionOperatorType.Sinh:
				case FunctionOperatorType.Tanh:
				case FunctionOperatorType.GetDate:
				case FunctionOperatorType.GetDay:
				case FunctionOperatorType.GetDayOfWeek:
				case FunctionOperatorType.GetDayOfYear:
				case FunctionOperatorType.GetHour:
				case FunctionOperatorType.GetMilliSecond:
				case FunctionOperatorType.GetMinute:
				case FunctionOperatorType.GetMonth:
				case FunctionOperatorType.GetSecond:
				case FunctionOperatorType.GetTimeOfDay:
				case FunctionOperatorType.GetYear:
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
					return new int[] { 1 };
				case FunctionOperatorType.Power:
				case FunctionOperatorType.Atn2:
				case FunctionOperatorType.BigMul:
				case FunctionOperatorType.AddDays:
				case FunctionOperatorType.AddHours:
				case FunctionOperatorType.AddMilliSeconds:
				case FunctionOperatorType.AddMinutes:
				case FunctionOperatorType.AddMonths:
				case FunctionOperatorType.AddSeconds:
				case FunctionOperatorType.AddTicks:
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddYears:
				case FunctionOperatorType.Max:
				case FunctionOperatorType.Min:
				case FunctionOperatorType.DateDiffDay:
				case FunctionOperatorType.DateDiffHour:
				case FunctionOperatorType.DateDiffMilliSecond:
				case FunctionOperatorType.DateDiffMinute:
				case FunctionOperatorType.DateDiffMonth:
				case FunctionOperatorType.DateDiffSecond:
				case FunctionOperatorType.DateDiffTick:
				case FunctionOperatorType.DateDiffYear:
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
					return new int[] { 2 };
				case FunctionOperatorType.Log:
				case FunctionOperatorType.IsNull:
				case FunctionOperatorType.Round:
					return new int[] { 1, 2 };
				case FunctionOperatorType.Insert:
				case FunctionOperatorType.Replace:
					return new int[] { 3 };
				case FunctionOperatorType.PadLeft:
				case FunctionOperatorType.PadRight:
				case FunctionOperatorType.Remove:
				case FunctionOperatorType.Substring:
					return new int[] { 2, 3 };
				case FunctionOperatorType.CharIndex:
					return new int[] { 2, 3, 4 };
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
					return new int[] { -1 };
				case FunctionOperatorType.Concat:
					return new int[] { 1, 2, -1 };
				case FunctionOperatorType.Iif:
					return new int[] { 3, -3 };
				default:
					return null;
			}
		}
	}
}
