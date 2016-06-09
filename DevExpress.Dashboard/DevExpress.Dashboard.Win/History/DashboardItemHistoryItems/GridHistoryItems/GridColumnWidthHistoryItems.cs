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

using System;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public class GridColumnFitToContentHistoryItem : ToggleStateHistoryItem<GridDashboardItem> {
		readonly GridColumnBase column;
		readonly GridColumnWidthMode newColumnWidthMode;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable;
		List<double> oldWeights;
		GridColumnWidthMode oldColumnWidthMode;
		GridColumnFixedWidthType oldFixedWidthType;
		protected override DashboardWinStringId CaptionId { get { return NewState ? DashboardWinStringId.HistoryItemEnableGridColumnFitToContent : DashboardWinStringId.HistoryItemDisableGridColumnFitToContent; } }
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), column.DisplayName, DashboardItem.Name); } }
		public GridColumnFitToContentHistoryItem(GridDashboardItem grid, Dictionary<GridColumnBase, int> columnActualWidthsTable, GridColumnBase column)
			: base(grid, column.WidthType == GridColumnFixedWidthType.FitToContent ? false : true) {
			this.column = column;
			this.newColumnWidthMode = GridColumnWidthMode.Manual;
			this.columnActualWidthsTable = columnActualWidthsTable;
		}
		protected override void PerformUndo() {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				DashboardItem.GridOptions.ColumnWidthMode = oldColumnWidthMode;
				column.WidthType = oldFixedWidthType;
				for(int i = 0; i < oldWeights.Count; i++) {
					GridColumnBase col = DashboardItem.Columns[i];
					col.Weight = oldWeights[i];
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
				column.WidthType = NewState ? GridColumnFixedWidthType.FitToContent : GridColumnFixedWidthType.Weight;
				if(column.WidthType == GridColumnFixedWidthType.Weight || oldColumnWidthMode == GridColumnWidthMode.AutoFitToGrid)
					GridDashboardColumnWidthConverter.ConvertActualWidthToWeight(columnActualWidthsTable);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		void SaveInitialState() {
			oldColumnWidthMode = DashboardItem.GridOptions.ColumnWidthMode;
			oldFixedWidthType = column.WidthType;
			oldWeights = new List<double>();
			foreach(GridColumnBase col in DashboardItem.Columns)
				oldWeights.Add(col.Weight);
		}
	}
	public class GridColumnFixedWidthHistoryItem : ToggleStateHistoryItem<GridDashboardItem> {
		readonly GridColumnWidthMode newColumnWidthMode;
		readonly GridColumnBase column;
		List<double> oldWeights;
		GridColumnFixedWidthType oldFixedWidthType;
		GridColumnWidthMode oldColumnWidthMode;
		readonly double newFixedWidth;
		readonly GridColumnFixedWidthType newWidthType;
		double oldFixedWidth;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable;
		protected override DashboardWinStringId CaptionId { get { return NewState ? DashboardWinStringId.HistoryItemEnableGridColumnFixedWidth : DashboardWinStringId.HistoryItemDisableGridColumnFixedWidth; } }
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), column.DisplayName, DashboardItem.Name); } }
		protected GridColumnBase Column { get { return column; } }
		public GridColumnFixedWidthHistoryItem(GridDashboardItem grid, Dictionary<GridColumnBase, int> columnActualWidthsTable, GridColumnBase column, bool newState, double newFixedWidth)
			: base(grid, newState) {
			this.column = column;
			this.columnActualWidthsTable = columnActualWidthsTable;
			this.newFixedWidth = NewState ? newFixedWidth : 0;
			this.newColumnWidthMode = NewState ? GridColumnWidthMode.Manual : DashboardItem.GridOptions.ColumnWidthMode;
			this.newWidthType = NewState ? GridColumnFixedWidthType.FixedWidth : GridColumnFixedWidthType.Weight;
		}
		protected override void PerformUndo() {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				DashboardItem.GridOptions.ColumnWidthMode = oldColumnWidthMode;
				column.FixedWidth = oldFixedWidth;
				column.WidthType = oldFixedWidthType;
				for(int i = 0; i < oldWeights.Count; i++) {
					GridColumnBase col = DashboardItem.Columns[i];
					col.Weight = oldWeights[i];
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
				column.WidthType = newWidthType;
				column.FixedWidth = newFixedWidth;
				if(!NewState || oldColumnWidthMode == GridColumnWidthMode.AutoFitToGrid)
					GridDashboardColumnWidthConverter.ConvertActualWidthToWeight(columnActualWidthsTable);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		void SaveInitialState() {
			oldColumnWidthMode = DashboardItem.GridOptions.ColumnWidthMode;
			oldFixedWidthType = column.WidthType;
			oldFixedWidth = column.FixedWidth;
			oldWeights = new List<double>();
			foreach(GridColumnBase col in DashboardItem.Columns)
				oldWeights.Add(col.Weight);
		}
	}
	public class GridResizeColumnsHistoryItem : DashboardItemHistoryItem<GridDashboardItem> {
		readonly GridColumnBase leftColumn;
		readonly GridColumnBase rightColumn;
		readonly ColumnsWidthOptionsInfo newOptionsInfo;
		ColumnsWidthOptionsInfo oldOptionsInfo;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemGridResizeColumns; } }
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), leftColumn.DisplayName, rightColumn.DisplayName, DashboardItem.Name); } }
		public GridResizeColumnsHistoryItem(GridDashboardItem grid, ColumnsWidthOptionsInfo optionsInfo, GridColumnBase rightColumn, GridColumnBase leftColumn)
			: base(grid) {
			Guard.ArgumentNotNull(optionsInfo, "optionsInfo");
			Guard.ArgumentNotNull(rightColumn, "rightColumn");
			Guard.ArgumentNotNull(leftColumn, "leftColumn");
			this.newOptionsInfo = optionsInfo;
			this.leftColumn = leftColumn;
			this.rightColumn = rightColumn;
		}
		protected override void PerformUndo() {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				ApplyInfo(oldOptionsInfo);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		protected override void PerformRedo() {
			SaveInitialState();
			DashboardItem.Dashboard.BeginUpdate();
			try {
				ApplyInfo(newOptionsInfo);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		void ApplyInfo(ColumnsWidthOptionsInfo optionsInfo) {
			DashboardItem.GridOptions.ColumnWidthMode = optionsInfo.ColumnWidthMode;
			foreach(ColumnWidthOptionsInfo info in optionsInfo.ColumnsInfo) {
				GridColumnBase column = DashboardItem.Columns[info.ActualIndex];
				column.WidthType = info.WidthType;
				column.Weight = info.Weight;
				column.FixedWidth = info.FixedWidth;
			}
		}
		void SaveInitialState() {
			oldOptionsInfo = new ColumnsWidthOptionsInfo();
			oldOptionsInfo.ColumnWidthMode = DashboardItem.GridOptions.ColumnWidthMode;
			for(int i = 0; i < DashboardItem.Columns.Count;i++ ) {
				GridColumnBase col = DashboardItem.Columns[i];
				ColumnWidthOptionsInfo columnInfo = new ColumnWidthOptionsInfo() {
					ActualIndex = i,
					WidthType = col.WidthType,
					Weight = col.Weight,
					FixedWidth = col.FixedWidth
				};
				oldOptionsInfo.ColumnsInfo.Add(columnInfo);
			}
		}
	}
}
