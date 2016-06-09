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
using System.Linq;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.ServerMode {
	sealed class FullExpandQueryContextCreator : IEnumerable<CrossAreaQueryContext> {
		readonly IServerModeHelpersOwner helpersOwner;
		readonly List<CrossAreaQueryContext> tuples;
		readonly IServerQueryContext serverQueryContext;
		readonly MultipleLevelsQueryExecutor executor;
		public FullExpandQueryContextCreator(IServerModeHelpersOwner helpersOwner, IServerQueryContext serverQueryContext, MultipleLevelsQueryExecutor executor) {
			this.helpersOwner = helpersOwner;
			this.serverQueryContext = serverQueryContext;
			this.executor = executor;
			this.tuples = new List<CrossAreaQueryContext>(Enumerate());
		}
		IEnumerable<CrossAreaQueryContext> Enumerate() {
			List<ColumnState> rowStates = new List<ColumnState>();
			for(int i = 0; i < serverQueryContext.RowArea.Count; i++)
				rowStates.Add(new ColumnState(false, helpersOwner, i == 0 ? null : rowStates[i - 1], serverQueryContext.RowArea[i], executor));
			List<ColumnState> columnStates = new List<ColumnState>();
			for(int i = 0; i < serverQueryContext.ColumnArea.Count; i++)
				columnStates.Add(new ColumnState(true, helpersOwner, i == 0 ? null : columnStates[i - 1], serverQueryContext.ColumnArea[i], executor));
			for(int i = rowStates.Count - 1; i >= -1; i--)
				for(int j = columnStates.Count - 1; j >= -1; j--) {
					if(i == rowStates.Count - 1 && j == columnStates.Count - 1 || ShowTotal(rowStates, i) && ShowTotal(columnStates, j) && helpersOwner.Areas.ServerSideDataArea.Count > 0) {
						IEnumerable<BaseCriteriaHolder> rowHolders = GetCriterias(rowStates, i);
						IEnumerable<BaseCriteriaHolder> columnHolders = GetCriterias(columnStates, j);
						foreach(BaseCriteriaHolder row in rowHolders)
							foreach(BaseCriteriaHolder column in columnHolders)
								yield return new CrossAreaQueryContext(
																	 new AreaQueryContext(GetFieldList(serverQueryContext.RowArea, i), row, row.Others, false),
																	 new AreaQueryContext(GetFieldList(serverQueryContext.ColumnArea, j), column, column.Others, false),
																	 helpersOwner.Areas.ServerSideDataArea
																	 );
					}
				}
			if(helpersOwner.Areas.ServerSideDataArea.Count > 0) {
				foreach(CrossAreaQueryContext context in EnumerateSortBySummaryCore(helpersOwner.Areas.RowArea, rowStates, helpersOwner.Areas.ColumnArea, columnStates, false))
					yield return context;
				foreach(CrossAreaQueryContext context in EnumerateSortBySummaryCore(helpersOwner.Areas.ColumnArea, columnStates, helpersOwner.Areas.RowArea, rowStates, true))
					yield return context;
			}
		}
		IEnumerable<BaseCriteriaHolder> GetCriterias(List<ColumnState> states, int index) {
			if(index == -1) {
				return new List<BaseCriteriaHolder>() { states.Count > 0 ? new EmptyCriteriaHolder(states[0].GetUpstairsCriteria()) : BaseCriteriaHolder.Empty };
			} else {
				return states[index].GetFullBranch();
			}
		}
		bool ShowTotal(List<ColumnState> states, int index) {
			return states.Count == 0 || index == states.Count - 1 || states[index + 1].ShowTotal;
		}
		IEnumerable<CrossAreaQueryContext> EnumerateSortBySummaryCore(List<ServerModeColumn> area, List<ColumnState> areaStates, List<ServerModeColumn> crossArea, List<ColumnState> crossAreaStates, bool isColumn) {
			for(int i = 0; i < area.Count; i++) {
				ServerModeColumn column = area[i];
				if(column.SortBySummary == null || !helpersOwner.Areas.ContainsDataColumn((ServerModeColumn)column.SortBySummary))
					continue;
				int sortByMemberCount = column.SortBySummaryMembers.Count;
				if(ShowTotal(areaStates, i) && ShowTotal(crossAreaStates, sortByMemberCount - 1))
					continue;
				foreach(BaseCriteriaHolder holder in areaStates[i].GetFullBranch()) {
					AreaQueryContext areaContext = new AreaQueryContext(GetFieldList(area, i), holder, holder.Others, false);
					IRawCriteriaConvertible conv = null;
					if(crossArea.Count > column.SortBySummaryMembers.Count)
						conv = crossAreaStates[column.SortBySummaryMembers.Count].GetUpstairsCriteria();
					AreaQueryContext crossAreaContext = new AreaQueryContext(GetFieldList(crossArea, sortByMemberCount - 1),
						new AndCriteria(conv, new QueryTupleListToCriteriaOperatorConverter(new QueryTuple(column.SortBySummaryMembers)))
						, Enumerable.Empty<QueryMember>(), false);
					yield return new CrossAreaQueryContext(isColumn ? crossAreaContext : areaContext, isColumn ? areaContext : crossAreaContext, helpersOwner.Areas.GetUniqueMeasureColumns(column.SortBySummary as ServerModeColumn ?? helpersOwner.Areas.ServerSideDataArea[0]));
				}
			}
		}
		IList<IGroupCriteriaConvertible> GetFieldList(List<ServerModeColumn> list, int endIndex) {
			IList<IGroupCriteriaConvertible> result = new List<IGroupCriteriaConvertible>();
			for(int i = 0; i <= endIndex; i++)
				result.Add(list[i]);
			return result;
		}
		IEnumerator<CrossAreaQueryContext> IEnumerable<CrossAreaQueryContext>.GetEnumerator() {
			return tuples.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return tuples.GetEnumerator();
		}
	}
}
