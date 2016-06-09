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
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.QueryMode {
	public class ExpandActionProvider<TColumn> : IActionProvider<TColumn, CellSet<TColumn>> where TColumn : QueryColumn {
		List<TColumn> IActionProvider<TColumn, CellSet<TColumn>>.GetColumns(List<TColumn> area, GroupInfo[] groups) {
			List<TColumn> result = new List<TColumn>();
			if(groups[0].Level >= area.Count - 1)
				throw new QueryException("Cannot expand last level");
			result.Add(area[groups[0].Level + 1]);
			return result;
		}
		List<QueryTuple> IActionProvider<TColumn, CellSet<TColumn>>.GetTuples(List<TColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups) {
			return ActionProviderHelper.GetTuples<TColumn>(fieldValues, groups, area);
		}
		void IActionProvider<TColumn, CellSet<TColumn>>.ParseFieldValues(CellSet<TColumn> cellSet, IQueryContext<TColumn> context, bool isColumn) {
			TuplesIndexedTreeCache<TColumn> treeCache = isColumn ? cellSet.ColumnIndexes : cellSet.RowIndexes;
			IComparer<LevelRecord> sorter = CellSetSorter<TColumn>.GetSortComparer(context, cellSet, isColumn);
			LevelRecord[] list = new LevelRecord[0];
			LevelRecord levelRecord;
			foreach(QueryTuple contextTuple in context.GetTuples(isColumn)) {
				int count = treeCache.BuildLastLevel(contextTuple, sorter, context.GetArea(isColumn)[0].SortByLastEmpty, ref list);
				for(int i = 0; i < count; i++) {
					levelRecord = list[i];
					levelRecord.GroupInfo = new GroupInfo(levelRecord.Member, contextTuple.BaseGroup, count);
				}
			}
		}
	}
}
