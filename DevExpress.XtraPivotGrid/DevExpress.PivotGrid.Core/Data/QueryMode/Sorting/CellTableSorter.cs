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
using System.Collections;
using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.PivotGrid.QueryMode.Sorting {
	class CellTableSorter<TColumn> : Sorter<TColumn, GroupInfo, IQueryMetadataColumn> where TColumn : QueryColumn {
		public static void FullSort(IQueryContext<TColumn> context, QueryAreas<TColumn> areas) {
			SortFieldValues(context, areas, true);
			SortFieldValues(context, areas, false);
		}
		public static void EnsureSortedByCalculatedMeasure(IQueryContext<TColumn> context, QueryAreas<TColumn> areas, GroupInfo[] groups, TColumn sortedColumn, bool isColumn) {
			if(sortedColumn == null || groups == null)
				return;
			TColumn sortByColumn = (TColumn)sortedColumn.SortBySummary;
			if(sortByColumn == null || areas.ServerSideDataArea.Contains(sortByColumn) || !areas.DataArea.Contains(sortByColumn))
				return;
			foreach(GroupInfo info in groups) {
				List<GroupInfo> a = info.GetChildren();
				if(a == null)
					continue;
				int count = a.Count;
				if(count > 1) {
					int index = 0;
					if(a[count - 1].Member.IsOthers)
						count--;
					if(a[0].IsTotal) {
						index++;
						count--;
					}
					if(count > 1)
						a.Sort(index, count, GetComparer(context.Areas, sortedColumn, isColumn));
				}
			}
		}
		static void SortFieldValues(IQueryContext<TColumn> context, QueryAreas<TColumn> areas, bool isColumn) {
			AreaFieldValues fieldValues = areas.GetFieldValues(isColumn);
			IList<TColumn> area = areas.GetArea(isColumn);
			IComparer<GroupInfo>[] comparers = new IComparer<GroupInfo>[area.Count];
			for(int i = 0; i < area.Count; i++)
				comparers[i] = GetComparer(context.Areas, area[i], isColumn);
			fieldValues.ForEachGroupInfo((a, level) => {
				int count = a.Count;
				if(count > 1) {
					int startIndex = 0;
					if(a[count - 1].Member.IsOthers)
						count--;
					if(a[0].IsTotal) {
						startIndex++;
						count--;
					}
					if(count > 1) {
						try {
							a.Sort(startIndex, count, comparers[level]);
						} catch(Exception raisedException) {
							bool handled = context.HandleException(new QueryHandleableException(raisedException.Message, raisedException));
							if(!handled)
								throw raisedException;
						}
					}
				}
			});
		}
		public static IComparer<GroupInfo> GetComparer(ISortContext<TColumn, GroupInfo, IQueryMetadataColumn> sortContext, TColumn sortedColumn, bool isColumn) {
			return GetSortComparerCore(sortContext, sortedColumn, isColumn);
		}
	}
}
