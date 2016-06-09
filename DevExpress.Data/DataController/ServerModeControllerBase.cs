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
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Async;
using DevExpress.Data.Async.Helpers;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Data {
	public abstract class ServerModeDataControllerBase : BaseGridControllerEx {
		public override bool IsServerMode { get { return true; } }
		public override bool AutoUpdateTotalSummary {
			get { return false; }
			set { }
		}
		public override bool ImmediateUpdateRowPosition {
			get { return false; }
			set { }
		}
		protected internal virtual bool AllowSortUnbound { get { return true; } }
		protected internal override void OnColumnPopulated(DataColumnInfo info) {
			base.OnColumnPopulated(info);
			if(info.Unbound) {
				if(AllowSortUnbound && info.UnboundWithExpression) return;
				info.AllowSort = false;
			}
		}
		protected override bool CanFindUnboundColumn(DataColumnInfo column) {
			if(AllowSortUnbound && column.UnboundWithExpression) return true;
			return false;
		}
		protected override bool AllowServerAction(string fieldName, ColumnServerActionType action) {
			if(IsServerMode) {
				DataColumnInfo info = Columns[fieldName];
				if(info != null && info.Unbound) return info.UnboundWithExpression && AllowSortUnbound;
			}
			bool res = false;
			if(ProcessListServerAction(fieldName, action, out res)) return res;
			IColumnsServerActions serverActions = DataSource as IColumnsServerActions;
			if(serverActions != null) return serverActions.AllowAction(fieldName, action);
			return true;
		}
		protected virtual bool ProcessListServerAction(string fieldName, ColumnServerActionType action, out bool res) {
			res = true;
			return false;
		}
		protected override void CalcGroupSummaryItem(SummaryItem summary) {
			if(!summary.GetAllowExternalCalculate(AllowSortUnbound)) {
				base.CalcGroupSummaryItem(summary);
				return;
			}
		}
		protected override object GetGroupRowValue(GroupRowInfo group, int column) {
			if(SortInfo.GroupCount > group.Level) {
				DataColumnSortInfo sortInfo = SortInfo[group.Level];
				ListSourceGroupInfo sgroup = GetListSourceGroupInfo(group);
				if(sgroup != null) {
					if(sortInfo.ColumnInfo.Index == column) return sgroup.GroupValue;
					if(sortInfo.AuxColumnInfo != null && sortInfo.AuxColumnInfo.Index == column) {
						if(sgroup.AuxValue != null) return sgroup.AuxValue;
					}
				}
			}
			return base.GetGroupRowValue(group, column);
		}
		internal ListSourceGroupInfo GetListSourceGroupInfo(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = group as ServerModeGroupRowInfo;
			if(sgroup == null || SortInfo.GroupCount < group.Level + 1) return null;
			return sgroup.ListGroupInfo;
		}
		public override object GetGroupRowValue(GroupRowInfo group) {
			ListSourceGroupInfo sgroup = GetListSourceGroupInfo(group);
			if(sgroup != null) return sgroup.GroupValue;
			DataColumnSortInfo info = group.Level >= SortInfo.Count ? null : SortInfo[group.Level];
			if(info == null) return null;
			return GetRowValue(group.ChildControllerRow, info.ColumnInfo.Index);
		}
		protected override Hashtable GetGroupSummaryCore(GroupRowInfo group) {
			group = RequestSummary(group);
			return group == null ? null : group.Summary;
		}
		public override void UpdateTotalSummary(List<SummaryItem> changedItems) {
			DoRefresh();
			VisualClientNotifyTotalSummary();
		}
		public virtual bool IsBusy { get { return false; } }
		protected override void DoSortSummary() { }
		protected override void DoFilterRows() { }
		protected override void OnBindingListChangedCore(ListChangedEventArgs e) {
			if(IsRefreshInProgress || IsBusy) return;
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemDeleted:
				case ListChangedType.ItemMoved:
				case ListChangedType.Reset:
					DoRefresh();
					break;
				case ListChangedType.ItemChanged:
					VisualClientUpdateLayout();
					break;
			}
		}
		protected abstract IList GetListSource();
		protected override void OnDataSourceChanged() {
			SetListSource(GetListSource());
			if(IsReady)
				ResetCurrentPosition();
		}
		protected override bool RequireEndEditOnGroupRows { get { return false; } }
		#region Static Core
		public static CriteriaOperator DescriptorToCriteria(DataColumnInfo column) {
			if(column == null) return null;
			if(!column.Unbound) return DescriptorToCriteria(column.PropertyDescriptor);
			if(!column.UnboundWithExpression) return null;
			try {
				return UnboundCriteriaInliner.Process(DescriptorToCriteria(column.PropertyDescriptor), column.owner);
			}
			catch { }
			return null;
		}
		static CriteriaOperator DescriptorToCriteria(PropertyDescriptor pd) {
			if(pd == null)
				return null;
			if(string.IsNullOrEmpty(pd.Name))
				return null;
			return new OperandProperty(pd.Name);
		}
		static readonly CriteriaOperator CurrentYearBeginFunction = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisYear);
		public static CriteriaOperator GetColumnGroupIntervalCriteria(CriteriaOperator plainCriteria, DevExpress.XtraGrid.ColumnGroupInterval groupType, out bool isGroupInterval) {
			isGroupInterval = true;
			switch(groupType) {
				case DevExpress.XtraGrid.ColumnGroupInterval.Alphabetical:
					return new FunctionOperator(FunctionOperatorType.Substring, plainCriteria, new ConstantValue(0), new ConstantValue(1));
				case DevExpress.XtraGrid.ColumnGroupInterval.Date:
					return new FunctionOperator(FunctionOperatorType.GetDate, plainCriteria);
				case DevExpress.XtraGrid.ColumnGroupInterval.DateMonth:
					return new FunctionOperator(FunctionOperatorType.AddMonths, CurrentYearBeginFunction, new FunctionOperator(FunctionOperatorType.DateDiffMonth, CurrentYearBeginFunction, plainCriteria));
				case DevExpress.XtraGrid.ColumnGroupInterval.DateRange:
					return GetColumnGroupIntervalCriteriaDateRange(plainCriteria);
				case DevExpress.XtraGrid.ColumnGroupInterval.DateYear:
					return new FunctionOperator(FunctionOperatorType.AddYears, CurrentYearBeginFunction, new FunctionOperator(FunctionOperatorType.DateDiffYear, CurrentYearBeginFunction, plainCriteria));
				case DevExpress.XtraGrid.ColumnGroupInterval.Default:
				case DevExpress.XtraGrid.ColumnGroupInterval.Value:
					isGroupInterval = false;
					return plainCriteria;
				default:
					throw new InvalidOperationException("ColumnGroupInterval." + groupType.ToString() + " not expected here...");
			}
		}
		static CriteriaOperator GetColumnGroupIntervalCriteriaDateRange(CriteriaOperator plainCriteria) {
			return Iif(BinaryEqual(GetDate(plainCriteria), GetDateByInterval(OutlookInterval.Today)), GetDateByInterval(OutlookInterval.Today),
				Iif(BinaryGreater(plainCriteria, GetDateByInterval(OutlookInterval.Today))
				,
					Iif(BinaryEqual(GetDateByInterval(OutlookInterval.Tomorrow), GetDate(plainCriteria)), GetDateByInterval(OutlookInterval.Tomorrow),
						Iif(BinaryGreaterOrEq(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(24 * 7)),
							Iif(BinaryGreater(DiffMonth(GetDateByInterval(OutlookInterval.Today), plainCriteria), new ConstantValue(1)), GetDateByInterval(OutlookInterval.BeyondNextMonth),
								Iif(BinaryGreaterOrEq(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(24 * 7 * 4)),
									 Iif(BinaryEqual(GetMonth(GetDateByInterval(OutlookInterval.Today)), GetMonth(plainCriteria)), GetDateByInterval(OutlookInterval.LaterThisMonth), GetDateByInterval(OutlookInterval.NextMonth))
									 ,
									 Iif(BinaryGreaterOrEq(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(24 * 7 * 3)), GetDateByInterval(OutlookInterval.ThreeWeeksAway),
										Iif(BinaryGreaterOrEq(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(24 * 7 * 2)), GetDateByInterval(OutlookInterval.TwoWeeksAway), GetDateByInterval(OutlookInterval.NextWeek))
									 )
								)
							)
						,
							GetDate(plainCriteria) 
						)
					)
				,
					Iif(BinaryEqual(GetDateByInterval(OutlookInterval.Yesterday), GetDate(plainCriteria)), GetDateByInterval(OutlookInterval.Yesterday),
						Iif(BinaryGreater(GetWeekStart(), GetDate(plainCriteria)),
							Iif(BinaryGreater(DiffMonth(plainCriteria, GetDateByInterval(OutlookInterval.Today)), new ConstantValue(1)), GetDateByInterval(OutlookInterval.Older),
								Iif(BinaryLess(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(-(24 * 7 * 3))),
									 Iif(BinaryEqual(GetMonth(GetDateByInterval(OutlookInterval.Today)), GetMonth(plainCriteria)), GetDateByInterval(OutlookInterval.EarlierThisMonth), GetDateByInterval(OutlookInterval.LastMonth))
									 ,
									 Iif(BinaryLess(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(-(24 * 7 * 2))), GetDateByInterval(OutlookInterval.ThreeWeeksAgo),
										Iif(BinaryLess(DiffHour(GetWeekStart(), GetDate(plainCriteria)), new ConstantValue(-(24 * 7))), GetDateByInterval(OutlookInterval.TwoWeeksAgo), GetDateByInterval(OutlookInterval.LastWeek))
									 )
								)
							)
						,
							GetDate(plainCriteria) 
						)
					)
				)
		);
		}
		static CriteriaOperator DiffHour(CriteriaOperator start, CriteriaOperator end) {
			return new FunctionOperator(FunctionOperatorType.DateDiffHour, start, end);
		}
		static CriteriaOperator DiffDay(CriteriaOperator start, CriteriaOperator end) {
			return new FunctionOperator(FunctionOperatorType.DateDiffDay, start, end);
		}
		static CriteriaOperator DiffMonth(CriteriaOperator start, CriteriaOperator end) {
			return new FunctionOperator(FunctionOperatorType.DateDiffMonth, start, end);
		}
		static CriteriaOperator BinaryEqual(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Equal);
		}
		static CriteriaOperator BinaryGreater(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Greater);
		}
		static CriteriaOperator BinaryGreaterOrEq(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.GreaterOrEqual);
		}
		static CriteriaOperator BinaryLess(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.Less);
		}
		static CriteriaOperator BinaryLessOrEq(CriteriaOperator left, CriteriaOperator right) {
			return new BinaryOperator(left, right, BinaryOperatorType.LessOrEqual);
		}
		static CriteriaOperator GetDate(CriteriaOperator criteria) {
			return new FunctionOperator(FunctionOperatorType.GetDate, criteria);
		}
		static CriteriaOperator GetMonth(CriteriaOperator criteria) {
			return new FunctionOperator(FunctionOperatorType.GetMonth, criteria);
		}
		static CriteriaOperator Iif(CriteriaOperator condition, CriteriaOperator trueResult, CriteriaOperator falseResult) {
			return new FunctionOperator(FunctionOperatorType.Iif, condition, trueResult, falseResult);
		}
		static CriteriaOperator GetDateByInterval(OutlookInterval interval) {
			switch(interval) {
				case OutlookInterval.Older:
					return new FunctionOperator(FunctionOperatorType.AddMonths, new FunctionOperator(FunctionOperatorType.LocalDateTimeToday), new ConstantValue(-2));
				case OutlookInterval.BeyondNextMonth:
					return new FunctionOperator(FunctionOperatorType.AddMonths, new FunctionOperator(FunctionOperatorType.LocalDateTimeToday), new ConstantValue(2));
				case OutlookInterval.Today:
					return new FunctionOperator(FunctionOperatorType.LocalDateTimeToday);
				case OutlookInterval.Tomorrow:
					return new FunctionOperator(FunctionOperatorType.LocalDateTimeTomorrow);
				case OutlookInterval.Yesterday:
					return new FunctionOperator(FunctionOperatorType.LocalDateTimeYesterday);
				case OutlookInterval.LaterThisMonth:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), 7 * 4);
				case OutlookInterval.ThreeWeeksAway:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), 7 * 3);
				case OutlookInterval.TwoWeeksAway:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), 7 * 2);
				case OutlookInterval.NextWeek:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), 8);
				case OutlookInterval.NextMonth:
					return new FunctionOperator(FunctionOperatorType.AddDays, new FunctionOperator(FunctionOperatorType.AddMonths, new FunctionOperator(FunctionOperatorType.LocalDateTimeThisMonth), new ConstantValue(2)), new ConstantValue(-1));
				case OutlookInterval.EarlierThisMonth:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), -(7 * 3) - 1);
				case OutlookInterval.ThreeWeeksAgo:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), -(7 * 2) - 1);
				case OutlookInterval.TwoWeeksAgo:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), -8);
				case OutlookInterval.LastWeek:
					return new FunctionOperator(FunctionOperatorType.AddDays, GetWeekStart(), -2);
				case OutlookInterval.LastMonth:
					return new FunctionOperator(FunctionOperatorType.AddMonths, new FunctionOperator(FunctionOperatorType.LocalDateTimeThisMonth), new ConstantValue(-1));
			}
			throw new NotSupportedException();
		}
		static CriteriaOperator GetWeekStart() {
			return new FunctionOperator(FunctionOperatorType.LocalDateTimeThisWeek);
		}
		internal static List<ServerModeOrderDescriptor> GetSortCollection(DataController controller) {
			if(!controller.IsSorted)
				return null;
			return GetSortCollection(controller.GetInterceptSortInfo(), controller.SortInfo.GroupCount);
		}
		static List<ServerModeOrderDescriptor> GetSortCollection(DataColumnSortInfo[] sortInfo, int groupCount) {
			List<ServerModeOrderDescriptor> rv = new List<ServerModeOrderDescriptor>();
			for(int n = 0; n < sortInfo.Length; n++) {
				DataColumnSortInfo item = sortInfo[n];
				CriteriaOperator criteria = DescriptorToCriteria(item.ColumnInfo);
				bool isGroupItem = n < groupCount;
				bool isGroupInterval = false;
				if(isGroupItem) {
					criteria = GetColumnGroupIntervalCriteria(criteria, item.GroupInterval, out isGroupInterval);
				}
				rv.Add(new ServerModeOrderDescriptor(criteria,
					item.SortOrder == ColumnSortOrder.Descending, item.AuxColumnInfo != null ? DescriptorToCriteria(item.AuxColumnInfo) : null));
				if(isGroupInterval) {
					if(n == sortInfo.Length - 1) {
						rv.Add(new ServerModeOrderDescriptor(DescriptorToCriteria(item.ColumnInfo),
							item.SortOrder == ColumnSortOrder.Descending, item.AuxColumnInfo != null ? DescriptorToCriteria(item.AuxColumnInfo) : null));
					}
				}
			}
			return rv;
		}
		internal static ICollection<ServerModeSummaryDescriptor> ListSourceSummaryItemsToServerModeSummaryDescriptors(ICollection<ListSourceSummaryItem> src) {
			if(src == null)
				return null;
			List<ServerModeSummaryDescriptor> rv = new List<ServerModeSummaryDescriptor>(src.Count);
			foreach(ListSourceSummaryItem s in src) {
				Aggregate agg;
				switch(s.SummaryType) {
					default:
					case SummaryItemType.Count:
						agg = Aggregate.Count;
						break;
					case SummaryItemType.Sum:
						agg = Aggregate.Sum;
						break;
					case SummaryItemType.Min:
						agg = Aggregate.Min;
						break;
					case SummaryItemType.Max:
						agg = Aggregate.Max;
						break;
					case SummaryItemType.Average:
						agg = Aggregate.Avg;
						break;
				}
				rv.Add(new ServerModeSummaryDescriptor(DescriptorToCriteria(s.Info), agg));
			}
			return rv;
		}
		#endregion
		public override void ValidateExpression(CriteriaOperator op) {
			try {
				base.ValidateExpression(op);
			} catch {
			}
		}
	}
}
