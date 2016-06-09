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
	class CrossAreaQueryContextCreator : IEnumerable<CrossAreaQueryContext> {
		readonly IServerModeHelpersOwner helpersOwner;
		readonly IEnumerable<AreaQueryContext> rows;
		readonly IEnumerable<AreaQueryContext> columns;
		readonly List<CrossAreaQueryContext> tuples;
		public CrossAreaQueryContextCreator(IServerModeHelpersOwner helpersOwner, IServerQueryContext serverQueryContext, MultipleLevelsQueryExecutor executor) {
			this.helpersOwner = helpersOwner;
			this.rows = BaseTuplesCreator.Create(helpersOwner, executor, serverQueryContext, false).OrderByDescending((c) => c.Grouping.Count());
			this.columns = BaseTuplesCreator.Create(helpersOwner, executor, serverQueryContext, true).OrderByDescending((c) => c.Grouping.Count());
			this.tuples = new List<CrossAreaQueryContext>(Enumerate());
		}
		IEnumerable<CrossAreaQueryContext> Enumerate() {
			foreach(AreaQueryContext row in rows)
				foreach(AreaQueryContext column in columns)
					yield return new CrossAreaQueryContext(row, column, helpersOwner.Areas.ServerSideDataArea);
			foreach(CrossAreaQueryContext context in EnumerateSortBy(rows, columns, false))
				yield return context;
			foreach(CrossAreaQueryContext context in EnumerateSortBy(columns, rows, true))
				yield return context;
		}
		IEnumerable<CrossAreaQueryContext> EnumerateSortBy(IEnumerable<AreaQueryContext> area, IEnumerable<AreaQueryContext> crossArea, bool isColumn) {
			foreach(AreaQueryContext context in area) {
				if(!context.IsNew || context.Grouping.Count == 0)
					continue;
				ServerModeColumn col = helpersOwner.Areas.GetArea(isColumn)[context.Grouping.Count - 1];
				if(col.SortBySummary == null)
					continue;
				if(!crossArea.Any((c) => c.Grouping.Count == 0)) {
					AreaQueryContext areaContext = new AreaQueryContext(new List<IGroupCriteriaConvertible>(), new QueryTupleListToCriteriaOperatorConverter(new QueryTuple[0]), new QueryMember[0], false);
					if(isColumn)
						yield return new CrossAreaQueryContext(areaContext, context, helpersOwner.Areas.ServerSideDataArea);
					else
						yield return new CrossAreaQueryContext(context, areaContext, helpersOwner.Areas.ServerSideDataArea);
				}
			}
		}
		IEnumerator<CrossAreaQueryContext> IEnumerable<CrossAreaQueryContext>.GetEnumerator() {
			return tuples.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return tuples.GetEnumerator();
		}
	}
}
