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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode;
using System.Linq;
namespace DevExpress.PivotGrid.ServerMode {
	class QueryExecutor : MultipleLevelsQueryExecutor {
		static List<SummaryItemTypeEx> aditionalTypes = new List<SummaryItemTypeEx>() { SummaryItemTypeEx.Top, SummaryItemTypeEx.Bottom, SummaryItemTypeEx.TopPercent, SummaryItemTypeEx.BottomPercent };
		IPivotQueryExecutor exec;
		public QueryExecutor(IPivotQueryExecutor exec, QueryMetadata<ServerModeColumn> metadata)
			: base(metadata) {
			this.exec = exec;
		}
		protected override void QueryAggregationsCalculation(CriteriaOperator selectCriteria, List<CriteriaOperator> groupingCriteria, IEnumerable<AggregationItemValue> items, CriteriaOperator filter) {
			List<Action<object>> setters = new List<Action<object>>();
			List<AggregationItem> summaryTypes = new List<AggregationItem>();
			List<AggregationItemValue> additionalSummaries = new List<AggregationItemValue>();
			int countIndex = -1;
			foreach(AggregationItemValue item in items) {
				if(aditionalTypes.Contains(item.SummaryType))
					additionalSummaries.Add(item);
				else {
					if(item.SummaryType == SummaryItemTypeEx.Count)
						countIndex = summaryTypes.Count;
					summaryTypes.Add(item);
					setters.Add(item.SetValue);
				}
			}
			if(countIndex < 0 && additionalSummaries.Any((rule) => rule.SummaryType.IsTopPercentage())) {
				countIndex = summaryTypes.Count;
				summaryTypes.Add(new AggregationItem(SummaryItemTypeEx.Count, 0));
				setters.Add((val) => { });
			}
			List<object> result = null;
			if(summaryTypes.Count > 0) {
				result = exec.GetQueryResult(new NestedSummaryServerModeQueryCriteria(groupingCriteria, CurrentOwner.FilterHelper.Criteria, selectCriteria, summaryTypes));
				for(int i = 0; i < setters.Count; i++)
					setters[i](result[i]);
			}
			foreach(AggregationItemValue rule in additionalSummaries) {
				switch(rule.SummaryType) {
					case SummaryItemTypeEx.Top:
					case SummaryItemTypeEx.Bottom: {
							object[] retresult = new object[Convert.ToInt32(rule.SummaryArgument)];
							retresult[retresult.Length - 1] = QueryTopBottomAggregations(rule.SummaryType.IsTop(), Convert.ToInt32(rule.SummaryArgument), groupingCriteria, selectCriteria, filter);
							rule.SetValue(retresult);
							break;
						}
					case SummaryItemTypeEx.TopPercent:
					case SummaryItemTypeEx.BottomPercent: {
							rule.SetValue(new object[] { 
									   QueryTopBottomAggregations(rule.SummaryType.IsTop(), rule.GetUnpercentedValue(result[countIndex]), groupingCriteria, selectCriteria, filter) });
							break;
						}
					default:
						throw new ArgumentException(string.Format("SummaryItemTypeEx: {0}", rule.SummaryType.ToString()));
				}
			}
		}
		protected override bool SupporsNestedSummaries { get { return exec != null && ((exec.CriteriaSyntax & CriteriaSyntax.SupportsNestedSummaries) != 0); } }
		protected override IPivotQueryResult QueryDataCore(List<ISelectCriteriaConvertible> operands, List<IGroupCriteriaConvertible> grouping, int aggregatesStart, List<IRawCriteriaConvertible> filters,
																	bool isTopPercentage = false, SortCriteria sorting = null, bool detalizeColumns = false, int maxRowCount = 0) {
			ServerModeQueryCriteria criteria = new ServerModeQueryCriteria(aggregatesStart);
			if(detalizeColumns) {
				foreach(IRawCriteriaConvertible conv in operands) {
					criteria.Operands.Add(conv.GetRawCriteria());
				}
			} else {
				foreach(ISelectCriteriaConvertible conv in operands)
					criteria.Operands.Add(conv.GetSelectCriteria());
				foreach(IGroupCriteriaConvertible conv in grouping) {
					CriteriaOperator op = conv.GetGroupCriteria();
					if(HaveOperandValue(op))
						criteria.Grouping.Add(op);
				}
			}
			if(filters != null)
				foreach(IRawCriteriaConvertible conv in filters)
					criteria.Filter = CriteriaOperator.And(criteria.Filter, conv.GetRawCriteria());
			if(maxRowCount > 0) {
				criteria.TopSelectedRecords = maxRowCount;
				criteria.TopValuePercent = maxRowCount > 0 && maxRowCount < 100 && isTopPercentage;
			}
			if(criteria.Grouping.Count == 0 && criteria.Operands.TrueForAll((op) => !HaveOperandValue(op)))
				criteria.TopSelectedRecords = 1;
			if(sorting != null)
				criteria.Sort.Add(sorting.GetSortingColumn());
			if(criteria.Operands.Count == 0)
				return PivotQueryResult.Empty;
			try {
				return exec.GetQueryResult(criteria, CurrentOwner.CancellationToken);
			} catch(Exception ex) {
				if(TryHandleException(new QueryHandleableException("Query error", ex, true)))
					return PivotQueryResult.Empty;
				throw ex;
			}
		}
		protected override object QueryTopBottomAggregations(bool isTop, int number, List<CriteriaOperator> grouping, CriteriaOperator select, CriteriaOperator filter) {
			ServerModeQueryCriteria criteria = new ServerModeQueryCriteria(-1);
			criteria.SkipSelectedRecords = number > 1 ? number - 1 : 0;
			criteria.TopSelectedRecords = 1;
			criteria.Filter = CriteriaOperator.And(filter, new CriteriaOperatorToServerModeCriteria(CurrentOwner.FilterHelper.Criteria, null, CurrentOwner, true).PatchedCriteria);
			criteria.Grouping.AddRange(grouping);
			criteria.Operands.Add(select);
			criteria.Sort.Add(new Xpo.DB.SortingColumn(select, isTop ? DevExpress.Xpo.DB.SortingDirection.Descending : Xpo.DB.SortingDirection.Ascending));
			try {
				return exec.GetQueryResult(criteria, CurrentOwner.CancellationToken).Data.First().First();
			} catch {
				return DevExpress.XtraPivotGrid.Data.PivotCellValue.ErrorValue.Value;
			}
		}
		bool HaveOperandValue(CriteriaOperator op) {
			if(object.ReferenceEquals(null, op))
				return true;
			return IsNotContantChecker.Check(op);
		}
	}
}
