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
	public class FirstLevelActionProvider<TColumn> : IActionProvider<TColumn, CellSet<TColumn>> where TColumn : QueryColumn {
		List<TColumn> IActionProvider<TColumn, CellSet<TColumn>>.GetColumns(List<TColumn> area, GroupInfo[] groups) {
			List<TColumn> result = new List<TColumn>();
			if(area.Count > 0)
				result.Add(area[0]);
			return result;
		}
		List<QueryTuple> IActionProvider<TColumn, CellSet<TColumn>>.GetTuples(List<TColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups) {
			return new List<QueryTuple>();
		}
		void IActionProvider<TColumn, CellSet<TColumn>>.ParseFieldValues(CellSet<TColumn> cellSet, IQueryContext<TColumn> context, bool isColumn) {
			IList<TColumn> area = context.GetArea(isColumn);
			AreaFieldValues fieldValues = isColumn ? context.ColumnValues : context.RowValues;
			TuplesIndexedTreeCache<TColumn> treeCache = isColumn ? cellSet.ColumnIndexes : cellSet.RowIndexes;
			if(area.Count == 0) {
				fieldValues.Clear(1);
				fieldValues.Add(GroupInfo.GrandTotalGroup);
				return;
			}
			if(area.Count != 1)
				throw new QueryException("Invalid area column count");
			LevelRecord[] list = new LevelRecord[0];
			int count = treeCache.BuildLastLevel(new QueryTuple(new QueryMember[0]), CellSetSorter<TColumn>.GetSortComparer(context, cellSet, isColumn), context.GetArea(isColumn)[0].SortByLastEmpty, ref list);
			fieldValues.Clear(count);
			foreach(LevelRecord record in list) {
				GroupInfo groupInfo = record.GroupInfo;
				if(groupInfo == null) {
					QueryMember member = record.Member;
					groupInfo = member.IsTotal ? GroupInfo.GrandTotalGroup : new GroupInfo(0, member, null);
					record.GroupInfo = groupInfo;
				}
				fieldValues.Add(groupInfo);
			}
		}
	}
}
