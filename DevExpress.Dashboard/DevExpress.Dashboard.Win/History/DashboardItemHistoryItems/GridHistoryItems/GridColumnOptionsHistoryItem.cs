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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public abstract class ChangeGridColumnHistoryItemBase : DashboardItemHistoryItem<GridDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemGridColumnOptions; } }
		protected ChangeGridColumnHistoryItemBase(GridDashboardItem gridDashboardItem)
			: base(gridDashboardItem) {
		}
	}
	public class CreateGridColumnHistoryItem : ChangeGridColumnHistoryItemBase {
		readonly GridColumnDragSection section;
		readonly int groupIndex;
		readonly GridColumnBase initialColumn;
		readonly GridColumnBase currentColumn;
		FormatRulesHistoryContext formatRulesHistoryContext;
		GridColumnCollection Columns { get { return DashboardItem.Columns; } }
		public CreateGridColumnHistoryItem(GridColumnDragSection section, int groupIndex, GridDashboardItem gridDashboardItem, 
			GridColumnBase initialColumn, GridColumnBase currentColumn) 
			: base(gridDashboardItem) {
			this.section = section;
			this.initialColumn = initialColumn;
			this.groupIndex = groupIndex;
			this.currentColumn = currentColumn;
		}
		void UpdateGroup(GridColumnBase removeColumn, GridColumnBase addColumn) {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				if(groupIndex < 0) {
					section.RemoveGroup(removeColumn);
					section.InsertGroup(groupIndex, addColumn);
				}
				else {
					int index = Columns.IndexOf(removeColumn);
					Columns.Remove(removeColumn);
					Columns.Insert(index, addColumn);
				}
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		protected override void PerformUndo() {	
			UpdateGroup(currentColumn, initialColumn);
			if(formatRulesHistoryContext != null)
				formatRulesHistoryContext.Undo();
		}
		 protected override void PerformRedo() {
			UpdateGroup(initialColumn, currentColumn);
			formatRulesHistoryContext = new FormatRulesHistoryContext(DashboardItem);
		}
	}
	public class ChangeGridMeasureColumnHistoryItem : ChangeGridColumnHistoryItemBase {
		readonly GridMeasureColumn column;
		readonly GridMeasureColumnDisplayMode initialDisplayMode;
		readonly GridMeasureColumnDisplayMode currentDisplayMode;
		readonly bool initialShowZeroLevel;
		readonly bool currentShowZeroLevel;
		public ChangeGridMeasureColumnHistoryItem(GridDashboardItem gridDashboardItem, GridMeasureColumn column, GridMeasureColumnDisplayMode currentDisplayMode, bool currentShowZeroLevel)
			: base(gridDashboardItem) {
			this.column = column;
			this.initialDisplayMode = column.DisplayMode;
			this.currentDisplayMode = currentDisplayMode;
			this.initialShowZeroLevel = column.AlwaysShowZeroLevel;
			this.currentShowZeroLevel = currentShowZeroLevel;
		}
		protected override void PerformUndo() {
			column.DisplayMode = initialDisplayMode;
			column.AlwaysShowZeroLevel = initialShowZeroLevel;
		}
		protected override void PerformRedo() {
			column.DisplayMode = currentDisplayMode;
			column.AlwaysShowZeroLevel = currentShowZeroLevel;
		}
	}
	public class ChangeGridDeltaColumnHistoryItem : ChangeGridColumnHistoryItemBase {
		readonly GridDeltaColumn column;
		readonly GridDeltaColumnDisplayMode initialDisplayMode;
		readonly GridDeltaColumnDisplayMode currentDisplayMode;
		readonly bool initialShowZeroLevel;
		readonly bool currentShowZeroLevel;
		readonly DeltaOptions initialDeltaOptions;
		readonly DeltaOptions currentDeltaOptions;
		public ChangeGridDeltaColumnHistoryItem(GridDashboardItem gridDashboardItem, GridDeltaColumn column, 
			GridDeltaColumnDisplayMode currentDisplayMode, bool currentShowZeroLevel, DeltaOptions currentDeltaOptions)
			: base(gridDashboardItem) {
			this.column = column;
			this.initialDisplayMode = column.DisplayMode;
			this.currentDisplayMode = currentDisplayMode;
			this.initialShowZeroLevel = column.AlwaysShowZeroLevel;
			this.currentShowZeroLevel = currentShowZeroLevel;
			this.initialDeltaOptions = column.DeltaOptions.Clone();
			this.currentDeltaOptions = currentDeltaOptions.Clone();
		}
		protected override void PerformUndo() {
			column.DisplayMode = initialDisplayMode;
			column.AlwaysShowZeroLevel = initialShowZeroLevel;
			column.DeltaOptions.Assign(initialDeltaOptions);
		}
		protected override void PerformRedo() {
			column.DisplayMode = currentDisplayMode;
			column.AlwaysShowZeroLevel = currentShowZeroLevel;
			column.DeltaOptions.Assign(currentDeltaOptions);
		}
	}
	public class ChangeGridSparklineColumnHistoryItem : ChangeGridColumnHistoryItemBase {
		readonly GridSparklineColumn column;
		readonly bool initialShowStartEndValues;
		readonly bool currentShowStartEndValues;
		readonly SparklineOptions initialSparklineOptions;
		readonly SparklineOptions currentSparklineOptions;
		public ChangeGridSparklineColumnHistoryItem(GridDashboardItem gridDashboardItem, GridSparklineColumn column, bool currentShowStartEndValues, SparklineOptions currentSparklineOptions)
			: base(gridDashboardItem) {
			this.column = column;
			this.initialShowStartEndValues = column.ShowStartEndValues;
			this.currentShowStartEndValues = currentShowStartEndValues;
			this.initialSparklineOptions = column.SparklineOptions.Clone();
			this.currentSparklineOptions = currentSparklineOptions.Clone();
		}
		protected override void PerformUndo() {
			column.ShowStartEndValues = initialShowStartEndValues;
			column.SparklineOptions.Assign(initialSparklineOptions);
		}
		protected override void PerformRedo() {
			column.ShowStartEndValues = currentShowStartEndValues;
			column.SparklineOptions.Assign(currentSparklineOptions);
		}
	}
}
