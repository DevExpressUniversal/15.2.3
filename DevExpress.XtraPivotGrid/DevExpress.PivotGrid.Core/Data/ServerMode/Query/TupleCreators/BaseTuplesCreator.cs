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

using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode {
	abstract class BaseTuplesCreator : IEnumerable<AreaQueryContext> {
		static int maxMSAccessWhereStatementCount = 99;
		protected List<ServerModeColumn> Area { get { return Context.GetArea(IsColumn); } }
		public static BaseTuplesCreator Create(IServerModeHelpersOwner helpersOwner, MultipleLevelsQueryExecutor executor, IServerQueryContext context, bool isColumn) {
			if(context.ColumnExpand || context.RowExpand)
				if(isColumn && context.ColumnExpand || !isColumn && context.RowExpand)
					return new ThisAreaExpandTuplesCreator(helpersOwner, executor, context, isColumn);
				else
					return new CrossAreaExpandTuplesCreator(helpersOwner, executor, context, isColumn);
			else
				return new FirstLevelTuplesCreator(helpersOwner, executor, context, isColumn);
		}
		protected static QueryTuple CreateGrandTotalTuple(QueryMember totalMember) {
			return new QueryTuple(GroupInfo.GrandTotalGroup, totalMember);
		}
		readonly IServerModeHelpersOwner owner;
		readonly MultipleLevelsQueryExecutor executor;
		readonly IServerQueryContext context;
		readonly bool isColumn;
		readonly bool isNew;
		readonly IEnumerable<AreaQueryContext> tuples;
		protected IServerQueryContext Context { get { return context; } }
		protected bool IsColumn { get { return isColumn; } }
		protected List<QueryMember> GetTopSelectedRowsByColumn(QueryColumn column, QueryTuple tuple) {
			return DXListExtensions.ConvertAll<object, QueryMember>(executor.GetTopSelectedRows(owner, column, tuple, MultipleLevelsQueryExecutor.CreateSortByCriteria(column, context.Areas.ServerSideDataArea)), (value) => new ServerModeMember(column.Metadata, value));
		}
		protected BaseTuplesCreator(IServerModeHelpersOwner owner, MultipleLevelsQueryExecutor executor, IServerQueryContext context, bool isColumn, bool isNew) {
			this.owner = owner;
			this.executor = executor;
			this.context = context;
			this.isColumn = isColumn;
			this.isNew = isNew;
			tuples = new List<AreaQueryContext>(CreateTuplesCore());
		}
		protected bool CalculateTotal(ServerModeColumn column) {
			return column.CalculateTotals && Context.Areas.ServerSideDataArea.Count > 0;
		}
		protected bool CalculateOthers(ServerModeColumn column) {
			return column.SortBySummary != null || Context.Areas.ServerSideDataArea.Count > 0;
		}
		protected IEnumerable<QueryTuple> Enumerate(QueryTuple tuple, IEnumerable<QueryTuple> tuplesList) {
			if(tuple != null)
				yield return tuple;
			foreach(QueryTuple item in tuplesList)
				yield return item;
		}
		protected AreaQueryContext CreateAreaContext(QueryTuple tuple, int areaIndex) {
			if(tuple != null) {
				List<QueryMember> others = new List<QueryMember>();
				foreach(QueryMember member in tuple)
					if(member.IsOthers)
						others.Add(member);
				return CreateAreaContext(new List<QueryTuple>() { tuple }, areaIndex, others);
			} else
				return CreateAreaContext(new List<QueryTuple>(), areaIndex);
		}
		protected AreaQueryContext CreateAreaContext(List<QueryTuple> tuples, int areaIndex, List<QueryMember> others = null) {
			return CreateAreaContext(new QueryTupleListToCriteriaOperatorConverter(tuples), areaIndex, others);
		}
		protected AreaQueryContext CreateAreaContext(IRawCriteriaConvertible conv, int areaIndex, bool isn, IEnumerable<QueryMember> others = null) {
			List<ServerModeColumn> area = IsColumn ? context.Areas.ColumnArea : context.Areas.RowArea;
			List<IGroupCriteriaConvertible> areaList = new List<IGroupCriteriaConvertible>();
			for(int i = 0; i <= areaIndex; i++)
				areaList.Add(area[i]);
			return new AreaQueryContext(areaList, conv, others ?? new QueryMember[0], isn);
		}
		protected AreaQueryContext CreateAreaContext(IRawCriteriaConvertible conv, int areaIndex, IEnumerable<QueryMember> others = null) {
			return CreateAreaContext(conv, areaIndex, isNew, others);
		}
		protected IEnumerable<AreaQueryContext> EnumerateTuplesList(IList<ServerModeColumn> area, List<QueryTuple> tuples, int level, bool enumerateIfEmpty, IRawCriteriaConvertible additionPostFilter) {
			List<QueryTuple> resultTuples = new List<QueryTuple>();
			if(tuples.Count > 0 && !ExpandHelper.CanQueryFullLevel(area, level - 1) || tuples.Count == 1) {
				for(int i = tuples.Count - 1; i >= 0; i--) {
					QueryTuple tuple = tuples[i];
					if(tuple.ContainsOthers())
						yield return CreateAreaContext(tuple, level);
					else
						resultTuples.Add(tuple);
				}
				if(resultTuples.Count > 0 || ExpandHelper.CanQueryFullLevel(area, level - 1) && resultTuples.Count > 1)
					yield return CreateAreaContext(new AndCriteria(additionPostFilter, new QueryTupleListToCriteriaOperatorConverter(resultTuples)), level);
			} else
				if(enumerateIfEmpty)
					foreach(AreaQueryContext context in EnumerateLastLevel(level, true, false, tuples, additionPostFilter))
						yield return context;
		}
		protected IEnumerable<AreaQueryContext> EnumerateLastLevel(int lastLevel, bool calculateOthers, bool calculateTotal, List<QueryTuple> desiredTuples = null, IRawCriteriaConvertible additionPostFilter = null) {
			if(desiredTuples != null && desiredTuples.Count > 0 && desiredTuples.Count * desiredTuples[0].MemberCount < maxMSAccessWhereStatementCount) {
				List<GroupInfo> firstLevel = null;
				(isColumn ? context.ColumnValues : context.RowValues).ForEachGroupInfo((f, b) => firstLevel = f, 0);
				if(firstLevel != null)
					additionPostFilter = new AndCriteria(additionPostFilter, new QueryTupleListToCriteriaOperatorConverter(desiredTuples, firstLevel, maxMSAccessWhereStatementCount));
			}
			IList<ServerModeColumn> area = Context.Areas.GetArea(IsColumn);
			IRawCriteriaConvertible valuesTuples;
			ServerModeColumn lastColumn = lastLevel >= 0 ? area[lastLevel] : null;
			calculateTotal = calculateTotal && lastColumn != null && CalculateTotal(lastColumn);
			calculateOthers = calculateOthers && lastColumn != null && CalculateOthers(lastColumn);
			List<QueryMember> lastLevelMembers = null;
			QueryTuple othersTuple = null;
			QueryTuple totalTuple = null;
			if(lastColumn != null)
				if(lastColumn.TopValueCount > 0) {
					lastLevelMembers = GetTopSelectedRowsByColumn(lastColumn, null);
					if(lastColumn.TopValueShowOthers && lastLevelMembers.Count >= lastColumn.TopValueCount) {
						if(calculateOthers)
							othersTuple = new QueryTuple(new OthersMember(lastColumn.Metadata, lastLevelMembers));
						if(calculateTotal)
							totalTuple = CreateGrandTotalTuple(lastColumn.TotalMember);
					} else
						if(calculateTotal)
							totalTuple = CreateGrandTotalTuple(new TotalMember(lastColumn.Metadata, lastLevelMembers));
				} else
					if(calculateTotal)
						totalTuple = CreateGrandTotalTuple(lastColumn.TotalMember);
			bool isMultiColumnFirstLevel = lastLevel > 0 || lastLevel < area.Count - 1;
			List<QueryTuple> preFilters = new List<QueryTuple>();
			if(isMultiColumnFirstLevel) {
				for(int i = 0; i < lastLevel; i++) {
					ServerModeColumn preColumn = area[i];
					if(preColumn.TopValueCount > 0 && preColumn.TopValueMode != XtraPivotGrid.TopValueMode.ParentFieldValues)
						preFilters.Add(new QueryTuple(GetTopSelectedRowsByColumn(preColumn, null)));
				}
				if(additionPostFilter == null)
					GetPostColumnsFilter(lastLevel + 1, preFilters);
				valuesTuples = new InversedQueryTupleListToCriteriaOperatorConverter(Enumerate(new QueryTuple(lastLevelMembers), preFilters));
			} else
				valuesTuples = new QueryTupleListToCriteriaOperatorConverter(lastLevelMembers != null ? DXListExtensions.ConvertAll<QueryMember, QueryTuple>(lastLevelMembers, (member) => new QueryTuple(member)) : null);
			yield return CreateAreaContext(new AndCriteria(additionPostFilter, valuesTuples), lastLevel);
			if(othersTuple != null)
				yield return CreateAreaContext(new AndCriteria(additionPostFilter, CreateICriteriaOperatorConvertible(othersTuple, preFilters, isMultiColumnFirstLevel)), lastLevel, false, new QueryMember[1] { othersTuple[0] });
			if(totalTuple != null)
				yield return CreateAreaContext(new AndCriteria(additionPostFilter, CreateICriteriaOperatorConvertible(totalTuple, preFilters, isMultiColumnFirstLevel)), lastLevel != -1 ? lastLevel - 1 : -1);
		}
		protected void GetPostColumnsFilter(int startLevel, List<QueryTuple> filters) {
			foreach(QueryTuple tuple in GetPostColumnsFilter(startLevel))
				if(tuple != null)
					filters.Add(tuple);
		}
		protected List<QueryTuple> GetPostColumnsFilter(int startLevel) {
			List<QueryTuple> tuples = new List<QueryTuple>();
			IList<ServerModeColumn> area = Context.Areas.GetArea(IsColumn);
			for(int i = startLevel; i < area.Count; i++) {
				ServerModeColumn postColumn = area[i];
				if(postColumn.TopValueCount > 0 && postColumn.TopValueMode != XtraPivotGrid.TopValueMode.ParentFieldValues && !postColumn.TopValueHiddenOthersShowedInTotal)
					tuples.Add(new QueryTuple(GetTopSelectedRowsByColumn(postColumn, null)));
				else
					tuples.Add(null);
			}
			return tuples;
		}
		IRawCriteriaConvertible CreateICriteriaOperatorConvertible(QueryTuple tuple, List<QueryTuple> list, bool isMultiColumnFirstLevel) {
			if(isMultiColumnFirstLevel)
				return new InversedQueryTupleListToCriteriaOperatorConverter(Enumerate(tuple, list));
			else
				return new QueryTupleListToCriteriaOperatorConverter(tuple);
		}
		protected abstract IEnumerable<AreaQueryContext> CreateTuplesCore();
		IEnumerator<AreaQueryContext> IEnumerable<AreaQueryContext>.GetEnumerator() {
			return tuples.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return tuples.GetEnumerator();
		}
	}
}
