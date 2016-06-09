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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class GridColumnWidthModeHistoryItem : DashboardItemHistoryItem<GridDashboardItem> {
		List<double> oldWeight;
		List<GridColumnFixedWidthType> oldWidthType;
		GridColumnWidthMode oldColumnWidthMode;
		readonly GridColumnWidthMode newColumnWidthMode;
		readonly GridColumnFixedWidthType newFixedWidthType;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable = new Dictionary<GridColumnBase, int>();
		protected override DashboardWinStringId CaptionId { 
			get {
				switch(newColumnWidthMode) {
					case GridColumnWidthMode.AutoFitToContents:
						return DashboardWinStringId.HistoryItemGridAutoFitToContentsColumnWidthMode;
					case GridColumnWidthMode.AutoFitToGrid:
						return DashboardWinStringId.HistoryItemGridAutoFitToGridColumnWidthMode;
					default:
						return DashboardWinStringId.HistoryItemGridManualColumnWidthMode;
				}
			} 
		}
		public GridColumnWidthModeHistoryItem(GridDashboardItem grid, ColumnsWidthOptionsInfo optionsInfo, GridColumnWidthMode newColumnWidthMode)
			: base(grid) {
			this.newColumnWidthMode = newColumnWidthMode;
			this.newFixedWidthType = GridColumnFixedWidthType.Weight;
			foreach(ColumnWidthOptionsInfo info in optionsInfo.ColumnsInfo)
				this.columnActualWidthsTable.Add(grid.Columns[info.ActualIndex], info.ActualWidth);
		}
		protected override void PerformUndo() {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				DashboardItem.GridOptions.ColumnWidthMode = oldColumnWidthMode;
				for(int i = 0; i < oldWeight.Count; i++) {
					GridColumnBase column = DashboardItem.Columns[i];
					column.Weight = oldWeight[i];
					column.WidthType = oldWidthType[i];
				}
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		protected override void PerformRedo() {
			SaveInitialState();
			DashboardItem.Dashboard.BeginUpdate();
			try {
				DashboardItem.GridOptions.ColumnWidthMode = newColumnWidthMode;
				foreach(GridColumnBase column in DashboardItem.Columns)
					column.WidthType = newFixedWidthType;
				if(newColumnWidthMode == GridColumnWidthMode.Manual)
					GridDashboardColumnWidthConverter.ConvertActualWidthToWeight(columnActualWidthsTable);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		void SaveInitialState() {
			oldWeight = new List<double>();
			oldWidthType = new List<GridColumnFixedWidthType>();
			foreach(GridColumnBase column in DashboardItem.Columns) {
				oldWeight.Add(column.Weight);
				oldWidthType.Add(column.WidthType);
			}
			oldColumnWidthMode = DashboardItem.GridOptions.ColumnWidthMode;
		}
	}
}
