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
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class AllGroupsActionProvider<TColumn> : IActionProvider<TColumn, CellSet<TColumn>> where TColumn : QueryColumn {
		List<TColumn> IActionProvider<TColumn, CellSet<TColumn>>.GetColumns(List<TColumn> area, GroupInfo[] groups) {
			List<TColumn> result = new List<TColumn>();
			if(area.Count == 0)
				return result;
			int maxLevel = AreaFieldValues.GetMaxLevel(groups);
			for(int i = 0; i <= maxLevel; i++)
				result.Add(area[i]);
			return result;
		}
		List<QueryTuple> IActionProvider<TColumn, CellSet<TColumn>>.GetTuples(List<TColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups) {
			return GetTuples(area, fieldValues, groups);
		}
		protected abstract List<QueryTuple> GetTuples(List<TColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups);
		void IActionProvider<TColumn, CellSet<TColumn>>.ParseFieldValues(CellSet<TColumn> cellSet, IQueryContext<TColumn> context, bool isColumn) {
			TuplesIndexedTreeCache<TColumn> cache = isColumn ? cellSet.ColumnIndexes : cellSet.RowIndexes;
			foreach(QueryTuple parentTuple in context.GetTuples(isColumn))
				cache.SetTupleGroup(parentTuple, parentTuple.BaseGroup);
		}
	}
}
