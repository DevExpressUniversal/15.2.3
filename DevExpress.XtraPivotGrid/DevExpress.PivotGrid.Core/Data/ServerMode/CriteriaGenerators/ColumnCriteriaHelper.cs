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

using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB;
namespace DevExpress.PivotGrid.ServerMode {
	public static class ColumnCriteriaHelper {
		public static CriteriaOperator GetDateHour(CriteriaOperator date) {
			return new FunctionOperator(
				FunctionOperatorType.AddHours,
				new FunctionOperator(FunctionOperatorType.GetDate, date),
				new FunctionOperator(FunctionOperatorType.GetHour, date)
			);
		}
		public static CriteriaOperator GetDateHourMinute(CriteriaOperator date) {
			return new FunctionOperator(
				FunctionOperatorType.AddMinutes,
				GetDateHour(date),
				new FunctionOperator(FunctionOperatorType.GetMinute, date)
			);
		}
		public static CriteriaOperator GetDateMonthYear(CriteriaOperator date) {
			return new FunctionOperator(
				FunctionOperatorType.AddDays,
				new FunctionOperator(FunctionOperatorType.GetDate, date),
				new BinaryOperator(
					new ConstantValue(1),
					new FunctionOperator(FunctionOperatorType.GetDay, date),
					BinaryOperatorType.Minus
				)
			);
		}
		static CriteriaOperator GetCorrectedDayOfWeekNumber(CriteriaOperator dayOfWeekNumber) {
			return new BinaryOperator(
				new BinaryOperator(
						new BinaryOperator(
								dayOfWeekNumber,
								(int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek,
								BinaryOperatorType.Minus
							),
						new ConstantValue(7),
						BinaryOperatorType.Plus
					),
				new ConstantValue(7),
				BinaryOperatorType.Modulo
			);
		}
		static CriteriaOperator GetDayOfWeekNumber(CriteriaOperator day) {
			return new FunctionOperator(
					FunctionOperatorType.ToInt,
					new FunctionOperator(
						FunctionOperatorType.GetDayOfWeek,
						day
					)
				);
		}
		public static CriteriaOperator GetWeekOfMonthExpression(CriteriaOperator date) {
			return GetWeekNumberExpression(new FunctionOperator(FunctionOperatorType.GetDay, date), date);
		}
		public static CriteriaOperator GetWeekNumberExpression(CriteriaOperator dayNumber, CriteriaOperator day) {
			CriteriaOperator firstDay = new FunctionOperator(
					FunctionOperatorType.AddDays,
					day,
					new BinaryOperator(
						new ConstantValue(1),
						dayNumber,
						BinaryOperatorType.Minus
					)
				);
			CriteriaOperator firstDayDayOfWeekNumber = GetDayOfWeekNumber(firstDay);
			CriteriaOperator dayOfWeekNumber = GetDayOfWeekNumber(day);
			firstDayDayOfWeekNumber = GetCorrectedDayOfWeekNumber(firstDayDayOfWeekNumber);
			dayOfWeekNumber = GetCorrectedDayOfWeekNumber(dayOfWeekNumber);
			return new BinaryOperator(
				new FunctionOperator(
					FunctionOperatorType.Floor,
					new BinaryOperator(
						new BinaryOperator(
							new BinaryOperator(
									dayNumber,
									dayOfWeekNumber,
									BinaryOperatorType.Minus
								),
							new BinaryOperator(
									firstDayDayOfWeekNumber,
									new ConstantValue(1),
									BinaryOperatorType.Minus
								),
							BinaryOperatorType.Plus
						),
						new ConstantValue(7),
						BinaryOperatorType.Divide
					)
				),
				new ConstantValue(1),
				BinaryOperatorType.Plus
			);
		}
		public static CriteriaOperator GetTruncatedFractionalPartExpression(CriteriaOperator value) {
			return new FunctionOperator(
				FunctionOperatorType.Iif,
				new BinaryOperator(
					value,
					new ConstantValue(0),
					BinaryOperatorType.GreaterOrEqual
				),
				new FunctionOperator(
					FunctionOperatorType.Floor,
					value
				),
				new FunctionOperator(
					FunctionOperatorType.Ceiling,
					value
				)
			);
		}
		public static CriteriaOperator GetDateDiffFullDays(CriteriaOperator start, CriteriaOperator end) {
			return new BinaryOperator(
				new FunctionOperator(FunctionOperatorType.DateDiffDay, start, end),
				new FunctionOperator(
					FunctionOperatorType.Iif,
					new GroupOperator(
						GroupOperatorType.And,
						new BinaryOperator(end, start, BinaryOperatorType.Greater),
						new BinaryOperator(
							new FunctionOperator(FunctionOperatorType.GetTimeOfDay, end),
							new FunctionOperator(FunctionOperatorType.GetTimeOfDay, start),
							BinaryOperatorType.Less
						)
					),
					new ConstantValue(-1),
					new FunctionOperator(
						FunctionOperatorType.Iif,
						new GroupOperator(
							GroupOperatorType.And,
							new BinaryOperator(end, start, BinaryOperatorType.Less),
							new BinaryOperator(
								new FunctionOperator(FunctionOperatorType.GetTimeOfDay, end),
								new FunctionOperator(FunctionOperatorType.GetTimeOfDay, start),
								BinaryOperatorType.Greater
							)
						),
						new ConstantValue(1),
						new ConstantValue(0)
					)
				),
				BinaryOperatorType.Plus
			);
		}
		public static CriteriaOperator GetDateDiffFullMonths(CriteriaOperator start, CriteriaOperator end) {
			return new BinaryOperator(
				new FunctionOperator(FunctionOperatorType.DateDiffMonth, start, end),
				new FunctionOperator(
					FunctionOperatorType.Iif,
					new GroupOperator(
						GroupOperatorType.And,
						new BinaryOperator(end, start, BinaryOperatorType.Greater),
						new BinaryOperator(
							new FunctionOperator(FunctionOperatorType.GetDay, end),
							new FunctionOperator(FunctionOperatorType.GetDay, start),
							BinaryOperatorType.Less
						)
					),
					new ConstantValue(-1),
					new FunctionOperator(
						FunctionOperatorType.Iif,
						new GroupOperator(
							GroupOperatorType.And,
							new BinaryOperator(end, start, BinaryOperatorType.Less),
							new BinaryOperator(
								new FunctionOperator(FunctionOperatorType.GetDay, end),
								new FunctionOperator(FunctionOperatorType.GetDay, start),
								BinaryOperatorType.Greater
							)
						),
						new ConstantValue(1),
						new ConstantValue(0)
					)
				),
				BinaryOperatorType.Plus
			);
		}
		public static CriteriaOperator GetDateDiffFullYears(CriteriaOperator start, CriteriaOperator end) {
			return new BinaryOperator(
				new FunctionOperator(FunctionOperatorType.DateDiffYear, start, end),
				new FunctionOperator(
					FunctionOperatorType.Iif,
					new GroupOperator(
						GroupOperatorType.And,
						new BinaryOperator(end, start, BinaryOperatorType.Greater),
						new GroupOperator(
							GroupOperatorType.Or,
							new BinaryOperator(
								new FunctionOperator(FunctionOperatorType.GetMonth, end),
								new FunctionOperator(FunctionOperatorType.GetMonth, start),
								BinaryOperatorType.Less
							),
							new GroupOperator(
								GroupOperatorType.And,
								new BinaryOperator(
									new FunctionOperator(FunctionOperatorType.GetMonth, end),
									new FunctionOperator(FunctionOperatorType.GetMonth, start),
									BinaryOperatorType.Equal
								),
								new BinaryOperator(
									new FunctionOperator(FunctionOperatorType.GetDay, end),
									new FunctionOperator(FunctionOperatorType.GetDay, start),
									BinaryOperatorType.Less
								)
							)
						)
					),
					new ConstantValue(-1),
					new FunctionOperator(
						FunctionOperatorType.Iif,
						new GroupOperator(
							GroupOperatorType.And,
							new BinaryOperator(end, start, BinaryOperatorType.Less),
							new GroupOperator(
								GroupOperatorType.Or,
								new BinaryOperator(
									new FunctionOperator(FunctionOperatorType.GetMonth, end),
									new FunctionOperator(FunctionOperatorType.GetMonth, start),
									BinaryOperatorType.Greater
								),
								new GroupOperator(
									GroupOperatorType.And,
									new BinaryOperator(
										new FunctionOperator(FunctionOperatorType.GetMonth, end),
										new FunctionOperator(FunctionOperatorType.GetMonth, start),
										BinaryOperatorType.Equal
									),
									new BinaryOperator(
										new FunctionOperator(FunctionOperatorType.GetDay, end),
										new FunctionOperator(FunctionOperatorType.GetDay, start),
										BinaryOperatorType.Greater
									)
								)
							)
						),
						new ConstantValue(1),
						new ConstantValue(0)
					)
				),
				BinaryOperatorType.Plus
			);
		}
		public static CriteriaOperator GetNumericRangeGroupIntervalExpression(CriteriaOperator value, int numericRangeInterval) {
			return new FunctionOperator(
				FunctionOperatorType.Floor,
				new BinaryOperator(
					value,
					new ConstantValue(numericRangeInterval),
					BinaryOperatorType.Divide
				)
			);
		}
		public static CriteriaOperator GetDoubleSquareSum(CriteriaOperator criteria, bool isServerCriteria) {
			return new FunctionOperator(FunctionOperatorType.ToDouble, GetAggregate(new BinaryOperator(criteria, criteria, BinaryOperatorType.Multiply), Aggregate.Sum, isServerCriteria));
		}
		public static CriteriaOperator GetAggregate(CriteriaOperator criteria, Aggregate aggregate, bool isServerCriteria) {
			return isServerCriteria ?
				(CriteriaOperator)new QuerySubQueryContainer(null, criteria, aggregate) :
				new AggregateOperand(null, criteria, aggregate, null);
		}
		public static CriteriaOperator WrapToType(CriteriaOperator criteria, UnboundColumnType type) {
			switch(type) {
				case Data.UnboundColumnType.Boolean: {
						if(IsLogicalCriteriaChecker.GetBooleanState(criteria) == BooleanCriteriaState.Value)
							criteria = new BinaryOperator(criteria, new ConstantValue(true), BinaryOperatorType.Equal);
						return new FunctionOperator(FunctionOperatorType.Iif, criteria, new ConstantValue(true), new ConstantValue(false));
					}
				case Data.UnboundColumnType.DateTime:
					return new FunctionOperator(FunctionOperatorType.AddDays, criteria, 0);
				case Data.UnboundColumnType.Decimal:
					return new FunctionOperator(FunctionOperatorType.ToDecimal, criteria);
				case Data.UnboundColumnType.Integer:
					return new FunctionOperator(FunctionOperatorType.ToInt, criteria);
				case Data.UnboundColumnType.String:
					return new FunctionOperator(FunctionOperatorType.ToStr, criteria);
				case Data.UnboundColumnType.Bound:
				case Data.UnboundColumnType.Object:
				default:
					return criteria;
			}
		}
	}
}
