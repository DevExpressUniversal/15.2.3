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
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.QueryMode.Sorting {
	abstract class Sorter<TColumn, TMember, TData> where TMember : IQueryMemberProvider where TColumn : QueryColumn {
		protected static IComparer<TMember> GetSortComparerCore(ISortContext<TColumn, TMember, TData> sortContext, TColumn sortedColumn, bool isColumn) {
			List<TColumn> dataArea = sortContext.GetDataArea();
			TColumn sortByColumn = sortedColumn.SortBySummary as TColumn;
			IComparer<TMember> comparer;
			TData data = sortContext.GetData(sortByColumn);
			if(sortContext.IsValidData(data)) {
				if(sortedColumn.SortBySummaryMembersExpanded) {
					if(isColumn)
						comparer = new ByRowSummaryComparer<TMember, TData>(sortContext.GetValueProvider(), sortContext.GetSortByObject(sortedColumn.SortBySummaryMembers, !isColumn), data);
					else
						comparer = new ByColumnSummaryComparer<TMember, TData>(sortContext.GetValueProvider(), sortContext.GetSortByObject(sortedColumn.SortBySummaryMembers, !isColumn), data);
				} else
					comparer = null;
			} else {
				comparer = sortedColumn.GetByMemberComparer<TMember>(sortContext.GetCustomFieldText(sortedColumn));
			}
			return SortHelper.GetResultComparer(sortContext.GetCustomFieldSort(sortedColumn), sortedColumn, comparer, (a, b, c) => new CustomComparer<TColumn, TMember>(a, b, c));
		}
	}
	class SortHelper {
		public static IComparer<TMember> GetResultComparer<TColumn, TMember>(Func<IQueryMemberProvider, IQueryMemberProvider, int?> customFieldSort, TColumn sortedColumn, IComparer<TMember> comparer, Func<Func<IQueryMemberProvider, IQueryMemberProvider, int?>, TColumn, IComparer<TMember>, IComparer<TMember>> cc)
			where TColumn : QueryColumn {
			if(comparer == null)
				if(sortedColumn.SortMode == PivotSortMode.Custom)
					comparer = new EmptyComparer<TMember>();
				else
					return null;
			if(sortedColumn.SortOrder == PivotSortOrder.Descending)
				comparer = new InversedComparer<TMember>(comparer);
			if(sortedColumn.SortMode == PivotSortMode.Custom)
				return cc(customFieldSort, sortedColumn, comparer);
			else
				return comparer;
		}		
	}
}
