#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Linq;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.ViewModel {
	public class GridDashboardItemViewModel : DataDashboardItemViewModel {
		public IList<GridColumnViewModel> Columns { get; set; }
		public bool AllowCellMerge { get; set; }
		public bool EnableBandedRows { get; set; }
		public bool ShowHorizontalLines { get; set; }
		public bool ShowVerticalLines { get; set; }
		public bool ShowColumnHeaders { get; set; }
		public bool AllColumnsFixed { get; set; }
		public GridColumnWidthMode ColumnWidthMode { get; set; }
		public bool WordWrap { get; set; }
		public string[] SelectionDataMembers { get; set; }
		public string[] RowIdentificatorDataMembers { get; set; }
		public string ColumnAxisName { get; set; }
		public string SparklineAxisName { get; set; }
		public bool HasDimensionColumns { get; set; }
		public bool ShowFooter { get { return TotalsCount > 0; } }
		public int TotalsCount {
			get {
				int count = 0;
				foreach(GridColumnViewModel col in Columns) {
					int totalsCount = col.Totals.Count;
					if(totalsCount > count)
						count = totalsCount;
				}
				return count;
			}
		}
		public GridDashboardItemViewModel() {
			Columns = new List<GridColumnViewModel>();
		}
		public GridDashboardItemViewModel(GridDashboardItem gridDashboardItem) 
			: base(gridDashboardItem) {
		}
	}
}
