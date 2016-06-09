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
using DevExpress.PivotGrid.DataCalculation;
namespace DevExpress.PivotGrid.QueryMode.Sorting {
	abstract class BySummaryComparer<TObject, TData> : BaseComparer, IComparer<TObject> where TObject : IQueryMemberProvider {
		protected ICalculationSource<TObject, TData> cellTable;
		protected TObject sortBy;
		protected TData data;
		public BySummaryComparer(ICalculationSource<TObject, TData> cellTable, TObject sortByRow, TData data) {
			this.cellTable = cellTable;
			this.sortBy = sortByRow;
			this.data = data;
		}
		int IComparer<TObject>.Compare(TObject x, TObject y) {
			int result;
			try {
				result = Comparer.Default.Compare(
												GetValue(x),
												  GetValue(y)
							   );				
			} catch {
				result = CompareCatched(
										  GetValue(x),
										  GetValue(y)
										   );
			}
			if(result == 0)
				return Comparer.Default.Compare(x.Member.UniqueLevelValue, y.Member.UniqueLevelValue);
			return result;
		}
		protected abstract object GetValue(TObject info);
	}
	class ByRowSummaryComparer<TObject, TData> : BySummaryComparer<TObject, TData>  where TObject : IQueryMemberProvider {
		public ByRowSummaryComparer(ICalculationSource<TObject, TData> cellTable, TObject sortByRow, TData data) : base(cellTable, sortByRow, data) { }
		protected override object GetValue(TObject info) {
			return cellTable.GetValue(info, sortBy, data);
		}
	}
	class ByColumnSummaryComparer<TObject, TData> : BySummaryComparer<TObject, TData>  where TObject : IQueryMemberProvider {
		public ByColumnSummaryComparer(ICalculationSource<TObject, TData> cellTable, TObject sortByColumn, TData data) : base(cellTable, sortByColumn, data) { }
		protected override object GetValue(TObject info) {
			return cellTable.GetValue(sortBy, info, data);
		}
	}
}
