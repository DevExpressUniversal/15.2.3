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

using System.Collections;
using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.ServerMode {
	class ThisAreaExpandTuplesCreator : BaseTuplesCreator {
		InversedQueryTupleListToCriteriaOperatorConverter postColumnsTopNFiltersConv;
		List<QueryTuple> AreaTuples { get { return Context.GetTuples(IsColumn); } }
		IList<ServerModeColumn> AreaColumns { get { return Context.Areas.GetArea(IsColumn); } }
		ServerModeColumn Column { get { return Area[0]; } }
		public ThisAreaExpandTuplesCreator(IServerModeHelpersOwner helpersOwner, MultipleLevelsQueryExecutor executor, IServerQueryContext context, bool isColumn)
			: base(helpersOwner, executor, context, isColumn, true) {
		}
		protected override IEnumerable<AreaQueryContext> CreateTuplesCore() {
			List<QueryTuple> postColumnsTopNFilters = new List<QueryTuple>();
			GetPostColumnsFilter(AreaColumns.IndexOf(Column) + 1, postColumnsTopNFilters);
			if(postColumnsTopNFilters.Count != 0)
				postColumnsTopNFiltersConv = new InversedQueryTupleListToCriteriaOperatorConverter(postColumnsTopNFilters);
			if(Column.TopValueCount == 0)
				return EnumerateOneLevelTuples();
			else
				return EnumerateTopValues();
		}
		IEnumerable<AreaQueryContext> EnumerateOneLevelTuples() {
			if(AreaTuples.Count == 1)
				yield return CreateAreaContext(AreaTuples, true);
			else
				foreach(AreaQueryContext context in EnumerateTuplesList(AreaColumns, AreaTuples, AreaColumns.IndexOf(Column), true, postColumnsTopNFiltersConv))
					yield return context;
		}
		protected AreaQueryContext CreateAreaContext(List<QueryTuple> tuples, bool ensureOthers) {
			List<QueryMember> others = new List<QueryMember>();
			if(ensureOthers)
				foreach(QueryTuple tuple in tuples)
					foreach(QueryMember member in tuple)
						if(member.IsOthers && !others.Contains(member))
							others.Add(member);
			int index = -1;
			List<ServerModeColumn> area = Context.GetArea(IsColumn);
			if(area.Count > 0)
				index = Context.Areas.GetArea(IsColumn).IndexOf(area[area.Count - 1]);
			return CreateAreaContext(new AndCriteria(postColumnsTopNFiltersConv, new QueryTupleListToCriteriaOperatorConverter(tuples)), index, others);
		}
		IEnumerable<AreaQueryContext> EnumerateTopValues() {
			ServerModeColumn column = Column;
			if(column.TopValueMode == TopValueMode.ParentFieldValues) {
				List<QueryTuple> all = new List<QueryTuple>();
				foreach(QueryTuple tuple in AreaTuples) {
					List<QueryMember> topRows = GetTopSelectedRowsByColumn(column, tuple);
					if(tuple.ContainsOthers())
						yield return CreateAreaContext(Append(tuple, topRows), true);
					else
						all.AddRange(Append(tuple, topRows));
					if(column.TopValueShowOthers && column.TopValueCount >= topRows.Count)
						yield return CreateAreaContext(new List<QueryTuple>() { new QueryTuple(EnumerateMembers(tuple, new OthersMember(column.Metadata, topRows))) }, true);
				}
				yield return CreateAreaContext(all, false);
			} else {
				List<QueryTuple> all = new List<QueryTuple>();
				List<QueryMember> topRows = GetTopSelectedRowsByColumn(column, null);
				foreach(QueryTuple tuple in AreaTuples) {
					if(tuple.ContainsOthers())
						yield return CreateAreaContext(Append(tuple, topRows), true);
					else
						all.AddRange(Append(tuple, topRows)); 
					if(column.TopValueShowOthers && column.TopValueCount >= topRows.Count)
						yield return CreateAreaContext(new List<QueryTuple>() { new QueryTuple(EnumerateMembers(tuple, new OthersMember(column.Metadata, topRows))) }, true);
				}
				if(all.Count > 0)
					yield return CreateAreaContext(all, false);
			}
		}
		List<QueryTuple> Append(QueryTuple tuple, List<QueryMember> members) {
			List<QueryTuple> list = new List<QueryTuple>();
			foreach(QueryMember member in members)
				list.Add(new QueryTuple(EnumerateMembers(tuple, member)));
			return list;
		}
		IEnumerable<QueryMember> EnumerateMembers(IEnumerable list, QueryMember member) {
			if(list != null)
				foreach(QueryMember lMember in list)
					yield return lMember;
			if(member != null)
				yield return member;
		}
	}
}
