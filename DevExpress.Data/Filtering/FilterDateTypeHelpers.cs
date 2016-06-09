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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using System.Globalization;
namespace DevExpress.XtraEditors.Helpers {
	public class DateFilterResult {
		const FilterDateType DefaultFilterType = FilterDateType.None;
		FilterDateType filterType = DefaultFilterType;
		string filterDisplayText = string.Empty;
		DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;
		CriteriaOperator filterCriteria;
		List<CriteriaOperator> userFilters;
		public DateFilterResult() : this(DefaultFilterType) {
		}
		public DateFilterResult(FilterDateType filterType) {
			this.filterCriteria = null;
			this.filterType = filterType;
			this.userFilters = new List<CriteriaOperator>();
		}
		public List<CriteriaOperator> UserFilters { get { return userFilters; } }
		public FilterDateType FilterType { get { return filterType; } internal set { filterType = value; } }
		public string FilterDisplayText { get { return filterDisplayText; } internal set { filterDisplayText = value; } }
		public DateTime StartDate { get { return startDate; } internal set { startDate = value; } }
		public DateTime EndDate { get { return endDate; } internal set { endDate = value; } }
		public CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				filterCriteria = value;
				if(object.ReferenceEquals(filterCriteria, null)) FilterType = DefaultFilterType;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetDates(DateTime startDate, DateTime endDate) {
			this.startDate = startDate;
			this.endDate = endDate;
		}
	}
}
namespace DevExpress.XtraEditors {
	using DevExpress.XtraEditors.Helpers;
	[Flags]
	public enum FilterDateType {
		None = 0,
		SpecificDate = 0x1,
		BeyondThisYear = 0x2,
		LaterThisYear = 0x4,
		LaterThisMonth = 0x8,
		LaterThisWeek = 0x10,
		NextWeek = 0x20,
		Tomorrow = 0x40,
		Today = 0x80,
		Yesterday = 0x100,
		EarlierThisWeek = 0x200,
		LastWeek = 0x400,
		EarlierThisMonth = 0x800,
		EarlierThisYear = 0x1000,
		PriorThisYear = 0x2000,
		Empty = 0x4000,
		User = 0x10000,
		Beyond = 0x20000,
		ThisWeek = 0x40000,
		ThisMonth = 0x80000,
		MonthAfter1 = 0x100000,
		MonthAfter2 = 0x200000,
		MonthAgo1 = 0x400000,
		MonthAgo2 = 0x800000,
		MonthAgo3 = 0x1000000,
		MonthAgo4 = 0x2000000,
		MonthAgo5 = 0x4000000,
		MonthAgo6 = 0x8000000,
		Earlier = 0x10000000
	}
	public static class FilterDateTypeHelper {
		class IntervalTriplet {
			public readonly FunctionOperatorType Interval;
			public readonly FunctionOperatorType? Start;
			public readonly FunctionOperatorType? End;
			public IntervalTriplet(FunctionOperatorType interval, FunctionOperatorType? start, FunctionOperatorType? end) {
				this.Interval = interval;
				this.Start = start;
				this.End = end;
			}
		}
		static readonly List<IntervalTriplet> Intervals;
		static readonly Dictionary<FilterDateType, FunctionOperatorType> Mappings;
		static readonly Dictionary<FunctionOperatorType, FilterDateType> ReverseMappings;
		static FilterDateTypeHelper() {
			Intervals = new List<IntervalTriplet>();
			FunctionOperatorType? nextIntervalStart = null;
			PushInterval(FunctionOperatorType.IsOutlookIntervalPriorThisYear, FunctionOperatorType.LocalDateTimeThisYear, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalEarlierThisYear, FunctionOperatorType.LocalDateTimeThisMonth, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalEarlierThisMonth, FunctionOperatorType.LocalDateTimeLastWeek, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalLastWeek, FunctionOperatorType.LocalDateTimeThisWeek, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalEarlierThisWeek, FunctionOperatorType.LocalDateTimeYesterday, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalYesterday, FunctionOperatorType.LocalDateTimeToday, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalToday, FunctionOperatorType.LocalDateTimeTomorrow, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalTomorrow, FunctionOperatorType.LocalDateTimeDayAfterTomorrow, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalLaterThisWeek, FunctionOperatorType.LocalDateTimeNextWeek, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalNextWeek, FunctionOperatorType.LocalDateTimeTwoWeeksAway, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalLaterThisMonth, FunctionOperatorType.LocalDateTimeNextMonth, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalLaterThisYear, FunctionOperatorType.LocalDateTimeNextYear, ref nextIntervalStart);
			PushInterval(FunctionOperatorType.IsOutlookIntervalBeyondThisYear, null, ref nextIntervalStart);
			Mappings = new Dictionary<FilterDateType, FunctionOperatorType>();
			Mappings.Add(FilterDateType.BeyondThisYear, FunctionOperatorType.IsOutlookIntervalBeyondThisYear);
			Mappings.Add(FilterDateType.LaterThisYear, FunctionOperatorType.IsOutlookIntervalLaterThisYear);
			Mappings.Add(FilterDateType.LaterThisMonth, FunctionOperatorType.IsOutlookIntervalLaterThisMonth);
			Mappings.Add(FilterDateType.LaterThisWeek, FunctionOperatorType.IsOutlookIntervalLaterThisWeek);
			Mappings.Add(FilterDateType.NextWeek, FunctionOperatorType.IsOutlookIntervalNextWeek);
			Mappings.Add(FilterDateType.Tomorrow, FunctionOperatorType.IsOutlookIntervalTomorrow);
			Mappings.Add(FilterDateType.Today, FunctionOperatorType.IsOutlookIntervalToday);
			Mappings.Add(FilterDateType.Yesterday, FunctionOperatorType.IsOutlookIntervalYesterday);
			Mappings.Add(FilterDateType.EarlierThisWeek, FunctionOperatorType.IsOutlookIntervalEarlierThisWeek);
			Mappings.Add(FilterDateType.LastWeek, FunctionOperatorType.IsOutlookIntervalLastWeek);
			Mappings.Add(FilterDateType.EarlierThisMonth, FunctionOperatorType.IsOutlookIntervalEarlierThisMonth);
			Mappings.Add(FilterDateType.EarlierThisYear, FunctionOperatorType.IsOutlookIntervalEarlierThisYear);
			Mappings.Add(FilterDateType.PriorThisYear, FunctionOperatorType.IsOutlookIntervalPriorThisYear);
			ReverseMappings = new Dictionary<FunctionOperatorType, FilterDateType>();
			foreach(KeyValuePair<FilterDateType, FunctionOperatorType> p in Mappings) {
				ReverseMappings.Add(p.Value, p.Key);
			}
		}
		static void PushInterval(FunctionOperatorType interval, FunctionOperatorType? intervalEnd, ref FunctionOperatorType? intervalStart) {
			Intervals.Add(new IntervalTriplet(interval, intervalStart, intervalEnd));
			intervalStart = intervalEnd;
		}
		public static DateTime GetMonthAgo(DateTime date, FilterDateType monthAgo) {
			return GetMonthAgo(date, GetMonthAgo(monthAgo));
		}
		public static DateTime GetMonthAgo(DateTime date, int month) {
			date = date.AddMonths(-month);
			return new DateTime(date.Year, date.Month, 1);
		}
		public static CriteriaOperator ToCriteria(CriteriaOperator property, FilterDateType heap) {
			CriteriaOperator res = ToCriteriaCore(property, heap);
			return res | ToCriteriaAlt(property, heap);
		}
		static CriteriaOperator ToCriteriaAlt(CriteriaOperator property, FilterDateType heap) {
			DateTime filterDate = DateTime.Now;
			filterDate = new DateTime(filterDate.Year, filterDate.Month, filterDate.Day);
			CriteriaOperator res = null;
			foreach(FilterDateType type in Enum.GetValues(typeof(FilterDateType))) {
				if((type & heap) != type) continue;
				CriteriaOperator current = null;
				switch(type) {
					case FilterDateType.Empty:
						current = new NullOperator(property);
						break;
					case FilterDateType.MonthAfter1:
					case FilterDateType.MonthAfter2:
					case FilterDateType.MonthAgo1:
					case FilterDateType.MonthAgo2:
					case FilterDateType.MonthAgo3:
					case FilterDateType.MonthAgo4:
					case FilterDateType.MonthAgo5:
					case FilterDateType.MonthAgo6:
						current = property >= GetMonthAgo(filterDate, GetMonthAgo(type)) & property < GetMonthAgo(filterDate, GetMonthAgo(type) - 1);
						break;
					case FilterDateType.ThisMonth:
						current = property >= GetMonthAgo(filterDate, 0) & property < GetMonthAgo(filterDate, -1);
						break;
					case FilterDateType.Earlier:
						current = property < GetMonthAgo(filterDate, 6);
						break;
					case FilterDateType.Beyond:
						current = property >= GetMonthAgo(filterDate, -3);
						break;
					case FilterDateType.ThisWeek:
						current = property >= OutlookDateHelper.GetWeekStart(filterDate, CultureInfo.CurrentCulture.DateTimeFormat) &
							property < OutlookDateHelper.GetWeekStart(filterDate, CultureInfo.CurrentCulture.DateTimeFormat).AddDays(7);
						break;
				}
				if(!object.ReferenceEquals(current, null)) res |= current;
			}
			return res;
		}
		static int GetMonthAgo(FilterDateType heap) {
			switch(heap) {
				case FilterDateType.MonthAfter2: return -2;
				case FilterDateType.MonthAfter1: return -1;
				case FilterDateType.MonthAgo1: return 1;
				case FilterDateType.MonthAgo2: return 2;
				case FilterDateType.MonthAgo3: return 3;
				case FilterDateType.MonthAgo4: return 4;
				case FilterDateType.MonthAgo5: return 5;
				case FilterDateType.MonthAgo6: return 6;
			}
			return 0;
		}
		public static bool IsFilterValid(FilterDateType filterType) {
			for(int i = 0; i < Intervals.Count; ++i) {
				IntervalTriplet triplet = Intervals[i];
				FilterDateType type = ReverseMappings[triplet.Interval];
				if((filterType & type) != type)
					continue;
				if(!triplet.Start.HasValue)
					return true;
				if(!triplet.End.HasValue)
					return true;
				DateTime start = EvalHelpers.EvaluateLocalDateTime(triplet.Start.Value);
				DateTime end = EvalHelpers.EvaluateLocalDateTime(triplet.End.Value);
				if(end > start)
					return true;
			}
			return false;
		}
		static CriteriaOperator ToCriteriaCore(CriteriaOperator property, FilterDateType heap) {
			bool[] arra = new bool[Intervals.Count + 1];
			for(int i = 0; i < Intervals.Count; ++i) {
				FunctionOperatorType interval = Intervals[i].Interval;
				FilterDateType type = ReverseMappings[interval];
				if((heap & type) == type)
					arra[i] = true;
			}
			System.Diagnostics.Debug.Assert(arra[arra.Length - 1] == false);
			CriteriaOperator rv = null;
			int? intervalStart = null;
			for(int i = 0; i < arra.Length; ++i) {
				if(arra[i] == true) {
					if(!intervalStart.HasValue)
						intervalStart = i;
				} else {
					if(intervalStart.HasValue) {
						int intervalEnd = i - 1;
						if(intervalEnd == intervalStart.Value
							||
							(intervalEnd == intervalStart.Value + 1
							&& intervalStart.Value != 0
							&& intervalEnd != Intervals.Count - 1)
						) {
							for(int j = intervalStart.Value; j <= intervalEnd; ++j) {
								rv |= new FunctionOperator(Intervals[j].Interval, property);
							}
						} else {
							CriteriaOperator startCriteria;
							FunctionOperatorType? startLocal = Intervals[intervalStart.Value].Start;
							if(startLocal.HasValue)
								startCriteria = property >= new FunctionOperator(startLocal.Value);
							else
								startCriteria = null;
							CriteriaOperator endCriteria;
							FunctionOperatorType? endLocal = Intervals[intervalEnd].End;
							if(endLocal.HasValue)
								endCriteria = property < new FunctionOperator(endLocal.Value);
							else
								endCriteria = null;
							rv |= (startCriteria & endCriteria);
						}
						intervalStart = null;
					}
				}
			}
			return rv;
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
				List<CriteriaOperator> rv = new List<CriteriaOperator>(ops.Count);
				foreach(CriteriaOperator op in ops)
					rv.Add(Process(op));
				return rv.ToArray();
			}
			public static CriteriaOperator Do(CriteriaOperator op) {
				return new ActualDatesProcessor().Process(op);
			}
		}
		public static CriteriaOperator ToTooltipCriteria(CriteriaOperator property, FilterDateType heap) {
			CriteriaOperator op = ToCriteria(property, heap);
			CriteriaOperator expanded = ActualDatesProcessor.Do(op);
			return expanded;
		}
		static void FromCriteriaRoot(CriteriaOperator currentFilter, CriteriaOperator operand, bool[] arra, ref DateTime? startDate, ref DateTime? endDate, ref bool hasNullValue) {
			GroupOperator gror = currentFilter as GroupOperator;
			if(!ReferenceEquals(gror, null) && (gror.OperatorType == GroupOperatorType.Or || gror.Operands.Count <= 1)) {
				foreach(CriteriaOperator op in gror.Operands) {
					FromCriteriaRoot(op, operand, arra, ref startDate, ref endDate, ref hasNullValue);
				}
			} else {
				FromCriteriaCore(currentFilter, operand, arra, ref startDate, ref endDate, ref hasNullValue);
			}
		}
		static void FromCriteriaCore(CriteriaOperator currentFilter, CriteriaOperator operand, bool[] arra, ref DateTime? startDate, ref DateTime? endDate, ref bool hasNullValue) {
			FunctionOperator fop = currentFilter as FunctionOperator;
			if(!ReferenceEquals(fop, null)) {
				if(fop.Operands.Count != 1)
					return;
				if(!Equals(operand, fop.Operands[0]))
					return;
				int index = ExtractIntervalIndex(fop.OperatorType);
				if(index >= 0)
					arra[index] = true;
				return;
			}
			BinaryOperator bop = currentFilter as BinaryOperator;
			if(!ReferenceEquals(bop, null)) {
				if(!Equals(operand, bop.LeftOperand))
					return;
				if(bop.OperatorType == BinaryOperatorType.GreaterOrEqual) {
					DateTime? dt;
					int? index;
					ExtractGreaterOrEquals(bop.RightOperand, out dt, out index);
					if(dt.HasValue) {
						startDate = dt.Value;
					} else if(index.HasValue) {
						for(int i = index.Value; i < arra.Length; ++i)
							arra[i] = true;
					}
					return;
				} else if(bop.OperatorType == BinaryOperatorType.Less) {
					DateTime? dt;
					int? index;
					ExtractLess(bop.RightOperand, out dt, out index);
					if(dt.HasValue) {
						endDate = dt.Value;
					} else if(index.HasValue) {
						for(int i = index.Value; i >= 0; --i)
							arra[i] = true;
					}
					return;
				}
				return;
			}
			GroupOperator grand = currentFilter as GroupOperator;
			if(!ReferenceEquals(grand, null)) {
				System.Diagnostics.Debug.Assert(grand.OperatorType == GroupOperatorType.And);
				if(grand.Operands.Count != 2)
					return;
				BinaryOperator f = grand.Operands[0] as BinaryOperator;
				if(ReferenceEquals(f, null))
					return;
				BinaryOperator s = grand.Operands[1] as BinaryOperator;
				if(ReferenceEquals(s, null))
					return;
				if(f.OperatorType != BinaryOperatorType.GreaterOrEqual)
					return;
				if(s.OperatorType != BinaryOperatorType.Less)
					return;
				if(!Equals(operand, f.LeftOperand))
					return;
				if(!Equals(operand, s.LeftOperand))
					return;
				DateTime? dts, dte;
				int? indexs, indexe;
				ExtractGreaterOrEquals(f.RightOperand, out dts, out indexs);
				ExtractLess(s.RightOperand, out dte, out indexe);
				if(dts.HasValue && dte.HasValue) {
					startDate = dts.Value;
					endDate = dte.Value;
				} else if(indexs.HasValue && indexe.HasValue) {
					for(int i = indexs.Value; i <= indexe.Value; ++i)
						arra[i] = true;
				}
				return;
			}
			UnaryOperator un = currentFilter as UnaryOperator;
			if(!ReferenceEquals(un, null)) {
				if(object.Equals(un.Operand, operand) && un.OperatorType == UnaryOperatorType.IsNull) {
					hasNullValue = true;
					return;
				}
			}
		}
		static void ExtractLess(CriteriaOperator criteriaOperator, out DateTime? dte, out int? indexe) {
			dte = null;
			indexe = null;
			OperandValue v = criteriaOperator as OperandValue;
			if(!ReferenceEquals(v, null)) {
				if(v.Value is DateTime) {
					dte = (DateTime)v.Value;
				}
				return;
			}
			FunctionOperator f = criteriaOperator as FunctionOperator;
			if(!ReferenceEquals(f, null)) {
				if(f.Operands.Count != 0)
					return;
				FunctionOperatorType type = f.OperatorType;
				for(int i = 0; i < Intervals.Count; ++i) {
					if(type == Intervals[i].End) {
						indexe = i;
						break;
					}
				}
			}
		}
		static void ExtractGreaterOrEquals(CriteriaOperator criteriaOperator, out DateTime? dts, out int? indexs) {
			dts = null;
			indexs = null;
			OperandValue v = criteriaOperator as OperandValue;
			if(!ReferenceEquals(v, null)) {
				if(v.Value is DateTime) {
					dts = (DateTime)v.Value;
				}
				return;
			}
			FunctionOperator f = criteriaOperator as FunctionOperator;
			if(!ReferenceEquals(f, null)) {
				if(f.Operands.Count != 0)
					return;
				FunctionOperatorType type = f.OperatorType;
				for(int i = 0; i < Intervals.Count; ++i) {
					if(type == Intervals[i].Start) {
						indexs = i;
						break;
					}
				}
			}
		}
		static int ExtractIntervalIndex(FunctionOperatorType interval) {
			for(int i = 0; i < Intervals.Count; ++i)
				if(Intervals[i].Interval == interval)
					return i;
			return -1;
		}
		public static DateFilterResult FromCriteria(CriteriaOperator currentFilter, string fieldName) {
			bool[] arra = new bool[Intervals.Count];
			DateTime? dtStart = null, dtEnd = null;
			bool dtNull = false;
			FromCriteriaRoot(currentFilter, new OperandProperty(fieldName), arra, ref dtStart, ref dtEnd, ref dtNull);
			FilterDateType rv = FilterDateType.None;
			for(int i = 0; i < arra.Length; ++i) {
				if(arra[i] == true) {
					rv |= ReverseMappings[Intervals[i].Interval];
				}
			}
			DateFilterResult result = new DateFilterResult();
			if(dtStart.HasValue) {
				result.StartDate = dtStart.Value;
				rv |= FilterDateType.SpecificDate;
			}
			if(dtEnd.HasValue) {
				result.EndDate = dtEnd.Value;
				rv |= FilterDateType.SpecificDate;
			}
			if(dtNull) {
				rv |= FilterDateType.Empty;
			}
			result.FilterType = rv;
			return result;
		}
	}
	public class FilterDateElement {
		string caption, tooltip;
		CriteriaOperator criteria;
		bool _checked;
		FilterDateType filterType;
		public FilterDateElement(string caption, string tooltip, CriteriaOperator criteria) {
			this._checked = false;
			this.tooltip = tooltip;
			this.caption = caption;
			this.criteria = criteria;
			this.filterType = FilterDateType.User;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FilterDateElement(string caption, string tooltip, CriteriaOperator criteria, FilterDateType filterType)
			: this(caption, tooltip, criteria) {
			this.filterType = filterType;
		}
		public FilterDateType FilterType { get { return filterType; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string Tooltip { get { return tooltip; } set { tooltip = value; } }
		public CriteriaOperator Criteria { get { return criteria; } }
		public bool Checked { get { return _checked; } set { _checked = value; } }
	}
}
