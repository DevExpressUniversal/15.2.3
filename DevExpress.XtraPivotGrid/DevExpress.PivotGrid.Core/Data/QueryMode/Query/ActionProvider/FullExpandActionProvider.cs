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
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.QueryMode {
	class FullExpandActionProvider<TColumn> : IActionProvider<TColumn, CellSet<TColumn>> where TColumn : QueryColumn {
		QueryAreas<TColumn> areas;
		public FullExpandActionProvider(QueryAreas<TColumn> areas) {
			this.areas = areas;
		}
		List<TColumn> IActionProvider<TColumn, CellSet<TColumn>>.GetColumns(List<TColumn> area, GroupInfo[] groups) {
			return new List<TColumn>(area);
		}
		List<QueryTuple> IActionProvider<TColumn, CellSet<TColumn>>.GetTuples(List<TColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups) {
			return new List<QueryTuple>();
		}
		void IActionProvider<TColumn, CellSet<TColumn>>.ParseFieldValues(CellSet<TColumn> queryResult, IQueryContext<TColumn> context, bool isColumn) {
			new FieldValuesBuilder<TColumn>(context, queryResult, isColumn).Build();
		}
	}
	class FieldValuesBuilder<TColumn> where TColumn : QueryColumn {
		readonly IQueryContext<TColumn> queryContext;
		readonly CellSet<TColumn> queryResult;
		readonly bool isColumn;
		readonly List<TColumn> area;
		readonly int lastLevel;
		readonly List<LevelRecord[]> levelsList = new List<LevelRecord[]>();
		public FieldValuesBuilder(IQueryContext<TColumn> context, CellSet<TColumn> queryResult, bool isColumn) {
			this.queryContext = context;
			this.queryResult = queryResult;
			this.isColumn = isColumn;
			area = context.GetArea(isColumn);
			lastLevel = area.Count - 1;
			for(int i = 0; i <= Math.Max(1, lastLevel + 1); i++)
				levelsList.Add(new LevelRecord[0]);
		}
		public void Build() {
			TuplesIndexedTreeCache<TColumn> cache = isColumn ? queryResult.ColumnIndexes : queryResult.RowIndexes;
			AreaFieldValues fieldValues = isColumn ? queryContext.ColumnValues : queryContext.RowValues;
			fieldValues.Clear();
			fieldValues.Add(GroupInfo.GrandTotalGroup);
			if(area.Count == 0)
				return;
			PreLastLevelRecord rec = cache.Root;
			if(lastLevel > 0) {
				LevelRecord[] recs = levelsList[0];
				int count = rec.BuildLevelList(null, ref recs);
				for(int i = 0; i < count; i++) {
					LevelRecord pair = recs[i];
					GroupInfo group = new GroupInfo(0, pair.Member, null);
					fieldValues.Add(group);
					pair.GroupInfo = group;
					BuildFieldValuesRecursive(pair, group, 1);
				}
			} else {
				LevelRecord[] recs = levelsList[0];
				int count = rec.BuildLevelList(null, ref recs);
				int start = 0;
				if(count > 0 && recs[0].Member.IsTotal) {
					fieldValues.Add(GroupInfo.GrandTotalGroup);
					recs[0].GroupInfo = GroupInfo.GrandTotalGroup;
					start = 1;
				}
				for(int i = start; i < count; i++) {
					LevelRecord record = recs[i];
					GroupInfo info = new GroupInfo(0, record.Member, null);
					fieldValues.Add(info);
					record.GroupInfo = info;
				}
			}
		}
		void BuildFieldValuesRecursive(LevelRecord record, GroupInfo parent, int level) {
			if(level == lastLevel) {
				LevelRecord[] recs = levelsList[level + 1];
				int count = record.BuildLevelList(null, ref recs);
				for(int i = 0; i < count; i++) {
					LevelRecord nRecord = recs[i];
					nRecord.GroupInfo = new GroupInfo(level, nRecord.Member, parent);
				}
			} else {
				PreLastLevelRecord rec = record as PreLastLevelRecord;
				if(rec != null) {
					LevelRecord[] recs = levelsList[level + 1];
					int count = rec.BuildLevelList(null, ref recs);
					for(int i = 0; i < count; i++) {
						LevelRecord pair = recs[i];
						GroupInfo group = new GroupInfo(level, pair.Member, parent);
						pair.GroupInfo = group;
						BuildFieldValuesRecursive(pair, group, level + 1);
					}
					if(rec.Others != null) {
						GroupInfo group = new GroupInfo(level, rec.Others.Member, parent);
						rec.Others.GroupInfo = group;
						BuildFieldValuesRecursive(rec.Others, group, level + 1);
					}
					levelsList[level + 1] = recs;
				}
			}
		}
	}
}
