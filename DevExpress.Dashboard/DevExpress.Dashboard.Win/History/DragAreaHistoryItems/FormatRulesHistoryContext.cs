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
namespace DevExpress.DashboardWin.Native {
	public class FormatRulesHistoryContext {
		class UndoDataItemRecord {
			public DataItem Item { get; set; }
			public DataItem ItemApplyTo { get; set; }
			public Dimension LevelRow { get; set; }
			public Dimension LevelColumn { get; set; }
			public bool IsEmpty { get { return Item == null && ItemApplyTo == null && LevelRow == null && LevelColumn == null; } }
			public bool Enabled { get; set; }
		}
		readonly DataDashboardItem dashboardItem;
		readonly Dictionary<DashboardItemFormatRule, UndoDataItemRecord> undoDataItemRecords = new Dictionary<DashboardItemFormatRule, UndoDataItemRecord>();
		public FormatRulesHistoryContext(DataDashboardItem dashboardItem) {
			this.dashboardItem = dashboardItem;
			if(dashboardItem.FormatRulesInternal == null) return;
			foreach (DashboardItemFormatRule formatRule in dashboardItem.FormatRulesInternal) {
				UndoDataItemRecord undoRecord = new UndoDataItemRecord();
				CellsItemFormatRule cellsRule = formatRule as CellsItemFormatRule;
				DataItem undoDataItem = null;
				if(cellsRule != null) {
					undoDataItem = cellsRule.DataItem;
					if(ShouldReplaceDataItem(undoDataItem)) {
						undoRecord.Item = undoDataItem;
						cellsRule.DataItem = FindDataItemForReplace(undoDataItem);
						if(cellsRule.DataItem == null) {
							undoRecord.Enabled = cellsRule.Enabled;
							cellsRule.Enabled = false;
						}
					}
					undoDataItem = cellsRule.DataItemApplyTo;
					if(ShouldReplaceDataItem(undoDataItem)) {
						undoRecord.ItemApplyTo = undoDataItem;
						cellsRule.DataItemApplyTo = FindDataItemForReplace(undoDataItem);
					}
				}
				PivotItemFormatRule pivotRule = formatRule as PivotItemFormatRule;
				Dimension undoDimension = null;
				if(pivotRule != null) {
					undoDimension = pivotRule.Level.Row;
					if(ShouldReplaceDataItem(undoDimension)) {
						undoRecord.LevelRow = undoDimension;
						pivotRule.Level.Row = FindDataItemForReplace(undoDimension) as Dimension;
					}
					undoDataItem = pivotRule.Level.Column;
					if(ShouldReplaceDataItem(undoDataItem)) {
						undoRecord.LevelColumn = undoDimension;
						pivotRule.Level.Column = FindDataItemForReplace(undoDimension) as Dimension;
					}
				}
				if(!undoRecord.IsEmpty)
					undoDataItemRecords.Add(formatRule, undoRecord);
			}
		}
		public void Undo() {
			foreach(KeyValuePair<DashboardItemFormatRule, UndoDataItemRecord> pair in undoDataItemRecords) {
				DashboardItemFormatRule formatRule = pair.Key;
				UndoDataItemRecord undoRecord = pair.Value;
				CellsItemFormatRule cellsRule = formatRule as CellsItemFormatRule;
				if(cellsRule != null) {
					if(undoRecord.Item != null) {
						cellsRule.DataItem = undoRecord.Item;
						cellsRule.Enabled = undoRecord.Enabled;
					}
					if(undoRecord.ItemApplyTo != null)
						cellsRule.DataItemApplyTo = undoRecord.ItemApplyTo;
				}
				PivotItemFormatRule pivotRule = formatRule as PivotItemFormatRule;
				if(pivotRule != null) {
					if(undoRecord.LevelRow != null)
						pivotRule.Level.Row = undoRecord.LevelRow;
					if(undoRecord.LevelColumn != null)
						pivotRule.Level.Column = undoRecord.LevelColumn;
				}
			}
		}
		bool ShouldReplaceDataItem(DataItem dataItem) {
			return dataItem != null && !dashboardItem.DataItemRepository.Contains(dataItem);
		}
		DataItem FindDataItemForReplace(DataItem dataItem) {
			return null;
		}
	}
}
