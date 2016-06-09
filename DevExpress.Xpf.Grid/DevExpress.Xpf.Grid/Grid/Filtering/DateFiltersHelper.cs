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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
namespace DevExpress.Xpf.Grid.Filtering {
	public static class DateFiltersHelper {
		readonly static Interval[] Intervals;
		readonly static Dictionary<FilterDateType, Interval> FilterToIntervalMap;
		readonly static Dictionary<FunctionOperatorType, FilterDateType> IntervalToFilterMap;
		readonly static FilterDateType[] EmptyFilters = new FilterDateType[] { };
		readonly static Tuple<DateTime?, DateTime?>[] EmptyDates = new Tuple<DateTime?, DateTime?>[] { };
		static DateFiltersHelper() {
			Intervals = new[] {
				new Interval(null, FunctionOperatorType.LocalDateTimeThisYear, FunctionOperatorType.IsOutlookIntervalPriorThisYear),
				new Interval(FunctionOperatorType.LocalDateTimeThisYear, FunctionOperatorType.LocalDateTimeThisMonth, FunctionOperatorType.IsOutlookIntervalEarlierThisYear),
				new Interval(FunctionOperatorType.LocalDateTimeThisMonth, FunctionOperatorType.LocalDateTimeLastWeek, FunctionOperatorType.IsOutlookIntervalEarlierThisMonth),
				new Interval(FunctionOperatorType.LocalDateTimeLastWeek, FunctionOperatorType.LocalDateTimeThisWeek, FunctionOperatorType.IsOutlookIntervalLastWeek),
				new Interval(FunctionOperatorType.LocalDateTimeThisWeek, FunctionOperatorType.LocalDateTimeYesterday, FunctionOperatorType.IsOutlookIntervalEarlierThisWeek),
				new Interval(FunctionOperatorType.LocalDateTimeYesterday, FunctionOperatorType.LocalDateTimeToday, FunctionOperatorType.IsOutlookIntervalYesterday),
				new Interval(FunctionOperatorType.LocalDateTimeToday, FunctionOperatorType.LocalDateTimeTomorrow, FunctionOperatorType.IsOutlookIntervalToday),
				new Interval(FunctionOperatorType.LocalDateTimeTomorrow, FunctionOperatorType.LocalDateTimeDayAfterTomorrow, FunctionOperatorType.IsOutlookIntervalTomorrow),
				new Interval(FunctionOperatorType.LocalDateTimeDayAfterTomorrow, FunctionOperatorType.LocalDateTimeNextWeek, FunctionOperatorType.IsOutlookIntervalLaterThisWeek),
				new Interval(FunctionOperatorType.LocalDateTimeNextWeek, FunctionOperatorType.LocalDateTimeTwoWeeksAway, FunctionOperatorType.IsOutlookIntervalNextWeek),
				new Interval(FunctionOperatorType.LocalDateTimeTwoWeeksAway, FunctionOperatorType.LocalDateTimeNextMonth, FunctionOperatorType.IsOutlookIntervalLaterThisMonth),
				new Interval(FunctionOperatorType.LocalDateTimeNextMonth, FunctionOperatorType.LocalDateTimeNextYear, FunctionOperatorType.IsOutlookIntervalLaterThisYear),
				new Interval(FunctionOperatorType.LocalDateTimeNextYear, null, FunctionOperatorType.IsOutlookIntervalBeyondThisYear)
			};
			FilterToIntervalMap = new Dictionary<FilterDateType, Interval> {
				{ FilterDateType.PriorThisYear, Intervals[0] },
				{ FilterDateType.EarlierThisYear, Intervals[1] },
				{ FilterDateType.EarlierThisMonth, Intervals[2] },
				{ FilterDateType.LastWeek, Intervals[3] },
				{ FilterDateType.EarlierThisWeek, Intervals[4] },
				{ FilterDateType.Yesterday, Intervals[5] },
				{ FilterDateType.Today, Intervals[6] },
				{ FilterDateType.Tomorrow, Intervals[7] },
				{ FilterDateType.LaterThisWeek, Intervals[8] },
				{ FilterDateType.NextWeek, Intervals[9] },
				{ FilterDateType.LaterThisMonth, Intervals[10] },
				{ FilterDateType.LaterThisYear, Intervals[11] },
				{ FilterDateType.BeyondThisYear, Intervals[12] },
			};
			IntervalToFilterMap = FilterToIntervalMap
				.ToDictionary<KeyValuePair<FilterDateType, Interval>, FunctionOperatorType, FilterDateType>(
					keySelector: kv => kv.Value.Parts[0],
					elementSelector: kv => kv.Key);
		}
		static DateTime Today {
			get { return DateTime.Today; }
		}
		public static CriteriaOperator ToCriteria(this IEnumerable<FilterDateType> filters, string fieldName) {
			Guard.ArgumentNotNull(fieldName, "fieldName");
			var property = new OperandProperty(fieldName);
			return ToCriteria(filters, property) | ToCriteriaAlt(filters, property) | ToNullCriteria(filters, property);
		}
		static CriteriaOperator ToCriteria(IEnumerable<FilterDateType> filters, OperandProperty property) {
			var intervals = filters.Where(x => x >= FilterDateType.PriorThisYear && x <= FilterDateType.BeyondThisYear).OrderBy(x => x).Select(x => FilterToIntervalMap[x]);
			var merged = Merge(intervals);
			return ToCriteria(merged, property);
		}
		static Interval[] Merge(IEnumerable<Interval> intervals) { 
			if(!intervals.Any()) return new[] { Interval.Empty };
			var result = new Stack<Interval>();
			var first = intervals.First();
			var rest = intervals.Skip(1);
			result.Push(first);
			foreach(var current in rest) {
				var top = result.Peek();
				if(!top.IsAdjacent(current)) {
					result.Push(current);
					continue;
				}
				Interval merged = top.Merge(current);
				result.Pop();
				result.Push(merged);
			}
			return result.Reverse().ToArray();
		}
		static CriteriaOperator ToCriteria(Interval[] intervals, OperandProperty property) { 
			return intervals
				.Select(x => {
					var filters = x.Parts;
					if(filters.Length == 1) {
						return new FunctionOperator(filters[0], property);
					}
					if(filters.Length == 2 && CanCreateOrFilter(filters)) {
						return new FunctionOperator(filters[0], property) | new FunctionOperator(filters[1], property);
					}
					return ToLeftClosedInterval(x, property);
				}).AggregateWithOr();
		}
		static bool CanCreateOrFilter(FunctionOperatorType[] filters) {
			return !AreFirstTwoFilters(filters) && !AreLastTwoFilters(filters);
		}
		static bool AreFirstTwoFilters(FunctionOperatorType[] filters) {
			return filters[0] == FunctionOperatorType.IsOutlookIntervalPriorThisYear && filters[1] == FunctionOperatorType.IsOutlookIntervalEarlierThisYear;
		}
		static bool AreLastTwoFilters(FunctionOperatorType[] filters) {
			return filters[0] == FunctionOperatorType.IsOutlookIntervalLaterThisYear && filters[1] == FunctionOperatorType.IsOutlookIntervalBeyondThisYear;
		}
		static CriteriaOperator ToLeftClosedInterval(Interval x, OperandProperty property) {
			var start = x.Start;
			var end = x.End;
			CriteriaOperator op1 = null;
			CriteriaOperator op2 = null;
			if(start.HasValue) {
				op1 = property >= new FunctionOperator(start.Value);
			}
			if(end.HasValue) {
				op2 = property < new FunctionOperator(end.Value);
			}
			return op1 & op2;
		}
		static CriteriaOperator ToCriteriaAlt(IEnumerable<FilterDateType> filters, OperandProperty property) {
			var alt = filters.Where(x => x >= FilterDateType.Earlier && x <= FilterDateType.Beyond).OrderBy(x => x);
			if(filters.Count() == 0) return null;
			return filters.Select(x => ToCriteriaAlt(x, property)).AggregateWithOr();
		}
		static CriteriaOperator ToCriteriaAlt(FilterDateType filter, OperandProperty property) {
			if(filter == FilterDateType.Earlier) {
				return property < Today.SixMonthsAgo();
			}
			if(filter == FilterDateType.MonthAgo6) {
				return property >= Today.SixMonthsAgo() & property < Today.FiveMonthsAgo();
			}
			if(filter == FilterDateType.MonthAgo5) {
				return property >= Today.FiveMonthsAgo() & property < Today.FourMonthsAgo();
			}
			if(filter == FilterDateType.MonthAgo4) {
				return property >= Today.FourMonthsAgo() & property < Today.ThreeMonthsAgo();
			}
			if(filter == FilterDateType.MonthAgo3) {
				return property >= Today.ThreeMonthsAgo() & property < Today.TwoMonthsAgo();
			}
			if(filter == FilterDateType.MonthAgo2) {
				return property >= Today.TwoMonthsAgo() & property < Today.MonthAgo();
			}
			if(filter == FilterDateType.MonthAgo1) {
				return property >= Today.MonthAgo() & property < Today.BeginningOfMonth();
			}
			if(filter == FilterDateType.ThisMonth) {
				return property >= Today.BeginningOfMonth() & property < Today.MonthAfter();
			}
			if(filter == FilterDateType.ThisWeek) {
				return property >= Today.WeekStart() & property < Today.WeekAfter().WeekStart();
			}
			if(filter == FilterDateType.MonthAfter1) {
				return property >= Today.MonthAfter() & property < Today.TwoMonthsAfter();
			}
			if(filter == FilterDateType.MonthAfter2) {
				return property >= Today.TwoMonthsAfter() & property < Today.ThreeMonthsAfter();
			}
			if(filter == FilterDateType.Beyond) {
				return property >= Today.ThreeMonthsAfter();
			}
			return null;
		}
		static CriteriaOperator ToNullCriteria(IEnumerable<FilterDateType> filters, OperandProperty property) {
			if(!filters.Contains(FilterDateType.Empty)) return null;
			return new NullOperator(property);
		}
		static CriteriaOperator AggregateWithOr(this IEnumerable<CriteriaOperator> source) {
			return source.Aggregate((acc, cur) => acc | cur);
		}
		public static FilterDateType[] ToFilters(this CriteriaOperator criteria, string fieldName) {
			Guard.ArgumentNotNull(fieldName, "fieldName");
			var property = new OperandProperty(fieldName);
			var filters = ToFilters(criteria, property);
			var altFilters = ToFiltersAlt(criteria, property);
			var emptyFilter = ToNullFilter(criteria, property);
			return filters.Concat(altFilters).Concat(emptyFilter).Distinct().OrderBy(x => x).ToArray();
		}
		static FilterDateType[] ToFilters(CriteriaOperator criteria, OperandProperty property) {
			var group = criteria as GroupOperator;
			if(!ReferenceEquals(group, null) && group.OperatorType == GroupOperatorType.Or) {
				return group.Operands.SelectMany(op => ToFilters(op, property)).ToArray();
			}
			return ToFiltersCore(criteria, property);
		}
		static FilterDateType[] ToFiltersCore(CriteriaOperator criteria, OperandProperty property) {
			if(criteria is FunctionOperator) {
				return ExtractFromFunc(criteria, property);
			}
			else if(criteria is BinaryOperator) {
				return ExtractFromBinary(criteria, property);
			}
			else if(criteria is GroupOperator) {
				return ExtractFromGroup(criteria, property);
			}
			return EmptyFilters;
		}
		static FilterDateType[] ExtractFromFunc(CriteriaOperator criteria, OperandProperty property) {
			var func = criteria as FunctionOperator;
			if(ReferenceEquals(func, null) || func.Operands.Count != 1 || !Equals(func.Operands[0], property)) return EmptyFilters;
			int? ix = ExtractIndex(func, x => x.Parts[0]);
			return ix.HasValue ? GetIntervals((int)ix, (int)ix) : EmptyFilters;
		}
		static FilterDateType[] ExtractFromBinary(CriteriaOperator criteria, OperandProperty property) {
			var binary = criteria as BinaryOperator;
			if(ReferenceEquals(binary, null)) return EmptyFilters;
			int? from = ExtractGreaterOrEqual(binary, property);
			if(from.HasValue) return GetIntervals((int)from, Intervals.Length - 1);
			int? to = ExtractLess(binary, property);
			if(to.HasValue) return GetIntervals(0, (int)to);
			return EmptyFilters;
		}
		static FilterDateType[] ExtractFromGroup(CriteriaOperator criteria, OperandProperty property) {
			var group = criteria as GroupOperator;
			if(ReferenceEquals(group, null)) return EmptyFilters;
			int? from = ExtractGreaterOrEqual(group.Operands[0] as BinaryOperator, property);
			int? to = ExtractLess(group.Operands[1] as BinaryOperator, property);
			if(!(from.HasValue && to.HasValue)) return EmptyFilters;
			return GetIntervals((int)from, (int)to);
		}
		static int? ExtractGreaterOrEqual(BinaryOperator binary, OperandProperty property) {
			if(!IsFilterCriteria(binary, property)) return null;
			if(binary.OperatorType != BinaryOperatorType.GreaterOrEqual) return null;
			return ExtractIndex(binary.RightOperand, x => x.Start);
		}
		static int? ExtractLess(BinaryOperator binary, OperandProperty property) {
			if(!IsFilterCriteria(binary, property)) return null;
			if(binary.OperatorType != BinaryOperatorType.Less) return null;
			return ExtractIndex(binary.RightOperand, x => x.End);
		}
		static bool IsFilterCriteria(BinaryOperator binary, OperandProperty property) {
			return !ReferenceEquals(binary, null) && Equals(binary.LeftOperand, property);
		}
		static int? ExtractIndex(CriteriaOperator criteria, Func<Interval, FunctionOperatorType?> type) {
			var func = criteria as FunctionOperator;
			if(ReferenceEquals(func, null)) return null;
			int ix = Array.FindIndex(Intervals, x => func.OperatorType == type(x));
			if(ix != -1) return ix;
			return null;
		}
		static FilterDateType[] GetIntervals(int from, int to) {
			return Enumerable.Range(from, to + 1 - from)
				.Select(i => Intervals[i])
				.Select(x => IntervalToFilterMap[x.Parts[0]])
				.ToArray();
		}
		static FilterDateType[] ToFiltersAlt(this CriteriaOperator criteria, OperandProperty property) {
			var group = criteria as GroupOperator;
			if(!ReferenceEquals(group, null) && group.OperatorType == GroupOperatorType.Or) {
				return group.Operands.SelectMany(x => x.ToFiltersAltCore(property)).ToArray();
			}
			return criteria.ToFiltersAltCore(property);
		}
		static FilterDateType[] ToFiltersAltCore(this CriteriaOperator criteria, OperandProperty property) {
			if(criteria is BinaryOperator) {
				return ExtractFromBinaryAlt(criteria, property);
			}
			if(criteria is GroupOperator) {
				return ExtractFromGroupAlt(criteria, property);
			}
			return EmptyFilters;
		}
		static FilterDateType[] ExtractFromBinaryAlt(CriteriaOperator criteria, OperandProperty property) {
			DateTime? date = null;
			date = ExtractDate(criteria, BinaryOperatorType.Less, property);
			if(date.HasValue && IsEarlier(date.Value)) return new[] { FilterDateType.Earlier };
			date = ExtractDate(criteria, BinaryOperatorType.GreaterOrEqual, property);
			if(date.HasValue && IsBeyond(date.Value)) return new[] { FilterDateType.Beyond };
			return EmptyFilters;
		}
		static FilterDateType[] ExtractFromGroupAlt(CriteriaOperator criteria, OperandProperty property) {
			var spans = ToDateSpansCore(criteria, property);
			if(spans.Length == 0) return EmptyFilters;
			var dates = spans[0];
			if(IsMonthAgo6(dates)) return new[] { FilterDateType.MonthAgo6 };
			if(IsMonthAgo5(dates)) return new[] { FilterDateType.MonthAgo5 };
			if(IsMonthAgo4(dates)) return new[] { FilterDateType.MonthAgo4 };
			if(IsMonthAgo3(dates)) return new[] { FilterDateType.MonthAgo3 };
			if(IsMonthAgo2(dates)) return new[] { FilterDateType.MonthAgo2 };
			if(IsMonthAgo1(dates)) return new[] { FilterDateType.MonthAgo1 };
			if(IsThisMonth(dates)) return new[] { FilterDateType.ThisMonth };
			if(IsThisWeek(dates)) return new[] { FilterDateType.ThisWeek };
			if(IsMonthAfter(dates)) return new[] { FilterDateType.MonthAfter1 };
			if(IsTwoMonthAfter(dates)) return new[] { FilterDateType.MonthAfter2 };
			return EmptyFilters;
		}
		static bool IsEarlier(DateTime date) {
			return date == Today.SixMonthsAgo();
		}
		static bool IsMonthAgo6(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.SixMonthsAgo(), end: Today.FiveMonthsAgo());
		}
		static bool IsMonthAgo5(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.FiveMonthsAgo(), end: Today.FourMonthsAgo());
		}
		static bool IsMonthAgo4(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.FourMonthsAgo(), end: Today.ThreeMonthsAgo());
		}
		static bool IsMonthAgo3(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.ThreeMonthsAgo(), end: Today.TwoMonthsAgo());
		}
		static bool IsMonthAgo2(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.TwoMonthsAgo(), end: Today.MonthAgo());
		}
		static bool IsMonthAgo1(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.MonthAgo(), end: Today.BeginningOfMonth());
		}
		static bool IsThisMonth(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.BeginningOfMonth(), end: Today.MonthAfter());
		}
		static bool IsThisWeek(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.WeekStart(), end: Today.WeekAfter().WeekStart());
		}
		static bool IsMonthAfter(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.MonthAfter(), end: Today.TwoMonthsAfter());
		}
		static bool IsTwoMonthAfter(Tuple<DateTime?, DateTime?> pair) {
			return IsDateSpan(pair, start: Today.TwoMonthsAfter(), end: Today.ThreeMonthsAfter());
		}
		static bool IsBeyond(DateTime date) {
			return date == Today.ThreeMonthsAfter();
		}
		static bool IsDateSpan(Tuple<DateTime?, DateTime?> pair, DateTime start, DateTime end) {
			var actualStart = pair.Item1;
			var actualEnd = pair.Item2;
			if(!actualStart.HasValue || !actualEnd.HasValue) return false;
			return actualStart == start && actualEnd == end;
		}
		static FilterDateType[] ToNullFilter(this CriteriaOperator criteria, OperandProperty property) {
			var group = criteria as GroupOperator;
			if(!ReferenceEquals(group, null) && group.OperatorType == GroupOperatorType.Or) {
				return group.Operands.SelectMany(x => x.ToNullFilterCore(property)).ToArray();
			}
			return criteria.ToNullFilterCore(property);
		}
		static FilterDateType[] ToNullFilterCore(this CriteriaOperator criteria, OperandProperty property) {
			if(criteria is NullOperator) {
				return new[] { FilterDateType.Empty };
			}
			return EmptyFilters;
		}
		public static DateTime[] ToDates(this CriteriaOperator criteria, string fieldName) {
			Guard.ArgumentNotNull(fieldName, "fieldName");
			var spans = ToDateSpans(criteria, fieldName);
			return spans.SelectMany(span => GetDaysInSpan(span)).ToArray();
		}
		static IEnumerable<DateTime> GetDaysInSpan(Tuple<DateTime?, DateTime?> pair) {
			var start = (DateTime)pair.Item1;
			var end = (DateTime)pair.Item2;
			return GetDaysInSpan(start, end);
		}
		internal static IEnumerable<DateTime> GetDaysInSpan(DateTime start, DateTime end) {
			var span = end.Subtract(start);
			return Enumerable.Range(0, span.Days).Select(d => start.AddDays(d));
		}
		static Tuple<DateTime?, DateTime?>[] ToDateSpans(this CriteriaOperator criteria, string fieldName) {
			var property = new OperandProperty(fieldName);
			var group = criteria as GroupOperator;
			if(!ReferenceEquals(group, null) && group.OperatorType == GroupOperatorType.Or) {
				return group.Operands
					.SelectMany(op => ToDateSpansCore(op, property))
					.Where(x => x.Item1 != null && x.Item2 != null)
					.ToArray();
			}
			return ToDateSpansCore(criteria, property);
		}
		static Tuple<DateTime?, DateTime?>[] ToDateSpansCore(CriteriaOperator criteria, OperandProperty property) {
			var group = criteria as GroupOperator;
			if(ReferenceEquals(group, null) || group.OperatorType != GroupOperatorType.And || group.Operands.Count != 2) return EmptyDates;
			DateTime? start = ExtractDate(group.Operands[0], BinaryOperatorType.GreaterOrEqual, property);
			DateTime? end = ExtractDate(group.Operands[1], BinaryOperatorType.Less, property);
			if(!(start.HasValue & end.HasValue)) return EmptyDates;
			return new[] { Tuple.Create(start, end) };
		}
		static DateTime? ExtractDate(CriteriaOperator criteria, BinaryOperatorType type, OperandProperty property) {
			var binary = criteria as BinaryOperator;
			if(ReferenceEquals(binary, null) || binary.OperatorType != type || !Equals(binary.LeftOperand, property)) return null;
			var value = binary.RightOperand as OperandValue;
			if(ReferenceEquals(value, null) || !(value.Value is DateTime)) return null;
			return (DateTime)value.Value;
		}
		public static bool IsFilterValid(this FilterDateType filter) {
			if(filter == FilterDateType.None || !FilterToIntervalMap.ContainsKey(filter)) return false;
			var interval = FilterToIntervalMap[filter];
			if(!interval.Start.HasValue || !interval.End.HasValue) return true;
			var start = EvalHelpers.EvaluateLocalDateTime(interval.Start.Value);
			var end = EvalHelpers.EvaluateLocalDateTime(interval.End.Value);
			return end > start;
		}
		public static CriteriaOperator ExpandDates(this CriteriaOperator criteria) {
			return ActualDatesProcessor.Do(criteria);
		}
		public static FunctionOperatorType ToFunctionType(this FilterDateType filter) {
			if(filter == FilterDateType.None) return FunctionOperatorType.None;
			return FilterToIntervalMap[filter].Parts[0];
		}
		class Interval : IEquatable<Interval> {
			internal static Interval Empty {
				get { return new Interval(null, null); }
			}
			internal readonly FunctionOperatorType? Start;
			internal readonly FunctionOperatorType? End;
			internal readonly FunctionOperatorType[] Parts;
			internal Interval(FunctionOperatorType? start, FunctionOperatorType? end, params FunctionOperatorType[] parts) {
				Start = start;
				End = end;
				Parts = parts ?? new FunctionOperatorType[] { };
			}
			internal bool IsAdjacent(Interval other) {
				return End == other.Start;
			}
			internal Interval Merge(Interval other) {
				return new Interval(Start, other.End, Parts.Concat(other.Parts).ToArray());
			}
			public override bool Equals(object obj) {
				if(obj == null || GetType() != obj.GetType()) return false;
				return base.Equals((Interval)obj);
			}
			public bool Equals(Interval other) {
				return Start == other.Start && End == other.End;
			}
			public override int GetHashCode() {
				return ToString().GetHashCode();
			}
			public override string ToString() {
				return string.Format("[{0} {1}]", Start, End);
			}
		}
		class ActualDatesProcessor : IClientCriteriaVisitor<CriteriaOperator> {
			public CriteriaOperator Visit(OperandProperty theOperand) {
				return theOperand;
			}
			public CriteriaOperator Visit(AggregateOperand theOperand) {
				return new AggregateOperand(theOperand.CollectionProperty, Process(theOperand.AggregatedExpression), theOperand.AggregateType, Process(theOperand.Condition));
			}
			public CriteriaOperator Visit(JoinOperand theOperand) {
				return new JoinOperand(theOperand.JoinTypeName, Process(theOperand.Condition), theOperand.AggregateType, Process(theOperand.AggregatedExpression));
			}
			public CriteriaOperator Visit(FunctionOperator theOperator) {
				switch(theOperator.OperatorType) {
					case FunctionOperatorType.LocalDateTimeThisYear:
					case FunctionOperatorType.LocalDateTimeThisMonth:
					case FunctionOperatorType.LocalDateTimeLastWeek:
					case FunctionOperatorType.LocalDateTimeThisWeek:
					case FunctionOperatorType.LocalDateTimeYesterday:
					case FunctionOperatorType.LocalDateTimeToday:
					case FunctionOperatorType.LocalDateTimeNow:
					case FunctionOperatorType.LocalDateTimeTomorrow:
					case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
					case FunctionOperatorType.LocalDateTimeNextWeek:
					case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
					case FunctionOperatorType.LocalDateTimeNextMonth:
					case FunctionOperatorType.LocalDateTimeNextYear:
						return new OperandValue(EvalHelpers.EvaluateLocalDateTime(theOperator.OperatorType));
					case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
					case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
					case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
					case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
					case FunctionOperatorType.IsOutlookIntervalNextWeek:
					case FunctionOperatorType.IsOutlookIntervalTomorrow:
					case FunctionOperatorType.IsOutlookIntervalToday:
					case FunctionOperatorType.IsOutlookIntervalYesterday:
					case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
					case FunctionOperatorType.IsOutlookIntervalLastWeek:
					case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
					case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
					case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
						return Process(EvalHelpers.ExpandIsOutlookInterval(theOperator));
					default:
						return new FunctionOperator(theOperator.OperatorType, Process(theOperator.Operands));
				}
			}
			public CriteriaOperator Visit(OperandValue theOperand) {
				return theOperand;
			}
			public CriteriaOperator Visit(GroupOperator theOperator) {
				return GroupOperator.Combine(theOperator.OperatorType, Process(theOperator.Operands));
			}
			public CriteriaOperator Visit(InOperator theOperator) {
				return new InOperator(Process(theOperator.LeftOperand), Process(theOperator.Operands));
			}
			public CriteriaOperator Visit(UnaryOperator theOperator) {
				return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
			}
			public CriteriaOperator Visit(BinaryOperator theOperator) {
				return new BinaryOperator(Process(theOperator.LeftOperand), Process(theOperator.RightOperand), theOperator.OperatorType);
			}
			public CriteriaOperator Visit(BetweenOperator theOperator) {
				return new BetweenOperator(Process(theOperator.TestExpression), Process(theOperator.BeginExpression), Process(theOperator.EndExpression));
			}
			CriteriaOperator Process(CriteriaOperator op) {
				if(ReferenceEquals(op, null))
					return null;
				return op.Accept(this);
			}
			CriteriaOperator[] Process(ICollection<CriteriaOperator> ops) {
				return ops.Select(Process).ToArray();
			}
			public static CriteriaOperator Do(CriteriaOperator op) {
				return new ActualDatesProcessor().Process(op);
			}
		}
		internal static DateTime SixMonthsAgo(this DateTime now) {
			return now.MonthsAgo(6);
		}
		internal static DateTime FiveMonthsAgo(this DateTime now) {
			return now.MonthsAgo(5);
		}
		internal static DateTime FourMonthsAgo(this DateTime now) {
			return now.MonthsAgo(4);
		}
		internal static DateTime ThreeMonthsAgo(this DateTime now) {
			return now.MonthsAgo(3);
		}
		internal static DateTime TwoMonthsAgo(this DateTime now) {
			return now.MonthsAgo(2);
		}
		internal static DateTime MonthAgo(this DateTime now) {
			return now.MonthsAgo(1);
		}
		internal static DateTime MonthAfter(this DateTime now) {
			return now.MonthsAfter(1);
		}
		internal static DateTime TwoMonthsAfter(this DateTime now) {
			return now.MonthsAfter(2);
		}
		internal static DateTime ThreeMonthsAfter(this DateTime now) {
			return now.MonthsAfter(3);
		}
		static DateTime MonthsAgo(this DateTime now, int months) {
			return now.AddMonthsAndGetBeginningOfMonth(-months);
		}
		static DateTime MonthsAfter(this DateTime now, int months) {
			return now.AddMonthsAndGetBeginningOfMonth(months);
		}
		static DateTime AddMonthsAndGetBeginningOfMonth(this DateTime now, int months) {
			return now.AddMonths(months).BeginningOfMonth();
		}
		internal static DateTime BeginningOfMonth(this DateTime now) {
			return new DateTime(now.Year, now.Month, 1);
		}
		internal static DateTime WeekStart(this DateTime now) {
			return now.WeekStart(CultureInfo.CurrentCulture);
		}
		internal static DateTime WeekStart(this DateTime now, CultureInfo culture) {
			var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
			var dayOfWeek = now.DayOfWeek;
			int delta = (dayOfWeek - firstDayOfWeek + 7) % 7;
			return now.AddDays(-delta).Date;
		}
		internal static DateTime WeekAfter(this DateTime now) {
			return now.AddDays(7);
		}
		internal static string SixMonthsAgoFilterName {
			get { return GetFilterNameFromDate(Today.SixMonthsAgo()); }
		}
		internal static string FiveMonthsAgoFilterName {
			get { return GetFilterNameFromDate(Today.FiveMonthsAgo()); }
		}
		internal static string FourMonthsAgoFilterName {
			get { return GetFilterNameFromDate(Today.FourMonthsAgo()); }
		}
		internal static string ThreeMonthsAgoFilterName {
			get { return GetFilterNameFromDate(Today.ThreeMonthsAgo()); }
		}
		internal static string TwoMonthsAgoFilterName {
			get { return GetFilterNameFromDate(Today.TwoMonthsAgo()); }
		}
		internal static string MonthAgoFilterName {
			get { return GetFilterNameFromDate(Today.MonthAgo()); }
		}
		internal static string MonthAfterFilterName {
			get { return GetFilterNameFromDate(Today.MonthAfter()); }
		}
		internal static string TwoMonthsAfterFilterName {
			get { return GetFilterNameFromDate(Today.TwoMonthsAfter()); }
		}
		static string GetFilterNameFromDate(DateTime date) {
			return date.ToString("yyyy, MMMM");
		}
	}
}
