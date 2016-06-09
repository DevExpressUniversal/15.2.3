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

namespace DevExpress.XtraSpreadsheet.Model.History {
	#region PivotTableStyleInfoHistoryItems
	public class ChangePivotTableStyleInfoStyleNameHistoryItem : SpreadsheetStringHistoryItem {
		readonly PivotTableStyleInfo info;
		public ChangePivotTableStyleInfoStyleNameHistoryItem(PivotTableStyleInfo info, string oldName, string newName)
			: base(info.Table.Worksheet, oldName, newName) {
			this.info = info;
		}
		protected override void UndoCore() {
			info.SetStyleNameCore(OldValue);
		}
		protected override void RedoCore() {
			info.SetStyleNameCore(NewValue);
		}
	}
	public class ChangePivotTableStyleInfoBooleanHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly PivotTableStyleInfo info;
		readonly byte mask;
		public ChangePivotTableStyleInfoBooleanHistoryItem(PivotTableStyleInfo info, bool oldValue, bool newValue, byte mask)
			: base(info.Table.Worksheet, oldValue, newValue) {
			this.info = info;
			this.mask = mask;
		}
		protected override void UndoCore() {
			info.SetBooleanValueCore(mask, OldValue);
		}
		protected override void RedoCore() {
			info.SetBooleanValueCore(mask, NewValue);
		}
	}
	#endregion
	#region MarkupPivotTablesForInvalidateFormatCacheHistoryItem
	public class MarkupPivotTablesForInvalidateFormatCacheHistoryItem : SpreadsheetHistoryItem {
		readonly PivotTableCollection pivotTables;
		readonly CellRange range;
		public MarkupPivotTablesForInvalidateFormatCacheHistoryItem(PivotTableCollection pivotTables, CellRange range)
			: base(pivotTables.DocumentModelPart) {
			this.pivotTables = pivotTables;
			this.range = range;
		}
		protected override void RedoCore() {
			pivotTables.InvalidateFormatCache(range);
		}
		protected override void UndoCore() {
			pivotTables.InvalidateFormatCache(range);
		}
	}
	#endregion
	#region PivotTableStateHistoryItem
	public class PivotTableStateHistoryItem : SpreadsheetHistoryItem {
		readonly PivotTable pivotTable;
		readonly PivotTableOutOfDateState state;
		public PivotTableStateHistoryItem(PivotTable pivotTable, PivotTableOutOfDateState state)
			: base(pivotTable.Worksheet) {
			this.pivotTable = pivotTable;
			this.state = state;
		}
		protected override void UndoCore() {
			pivotTable.CalculationInfo.State = state;
		}
		protected override void RedoCore() {
			pivotTable.CalculationInfo.State = state;
		}
	}
	#endregion
}
