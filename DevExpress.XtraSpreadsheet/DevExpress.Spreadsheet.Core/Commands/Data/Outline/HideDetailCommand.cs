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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region HideDetailCommand
	public class HideDetailCommand :UngroupCommand {
		public HideDetailCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.HideDetail; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_HideDetailCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_HideDetailModeCommand; } }
		public override string ImageName { get { return "HideDetail"; } }
		#endregion
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			ActiveSheet.Workbook.BeginUpdateFromUI();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<CellRangeBase> valueBasedState = state as IValueBasedCommandUIState<CellRangeBase>;
				if (valueBasedState == null || valueBasedState.Value.RangeType == CellRangeType.UnionRange)
					return;
				CloseOutline(valueBasedState.Value as CellRange);
			}
			finally {
				NotifyEndCommandExecution(state);
				ActiveSheet.Workbook.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Group.Collapse);
			ApplyActiveSheetProtection(state);
		}
		void CloseOutline(CellRange range) {
			bool selectedAllSheet = range.IsWholeWorksheetRange();
			if (selectedAllSheet || (ActiveSheet.Properties.FormatProperties.OutlineLevelCol > 0 && !range.IsRowRangeInterval()))
				CollapseColumnGroups(range);
			if (selectedAllSheet || (ActiveSheet.Properties.FormatProperties.OutlineLevelRow > 0 && !range.IsColumnRangeInterval()))
				CollapseRowGroups(range);
		}
		void CollapseColumnGroups(CellRange range) {
			HideColumnDetailCommand command = new HideColumnDetailCommand(ActiveSheet, range);
			if(command.Execute())
				UpdateColumnSelection();
		}
		void CollapseRowGroups(CellRange range) {
			HideRowDetailCommand command = new HideRowDetailCommand(ActiveSheet, range);
			if(command.Execute())
				UpdateRowSelection();
		}
		void UpdateColumnSelection() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			int left = selectedRange.LeftColumnIndex;
			int right = selectedRange.RightColumnIndex;
			bool selectionUpdated = false;
			for (int i = left; i <= right; i++) {
				Column col = ActiveSheet.Columns.TryGetColumn(i);
				if (col == null || !col.IsHidden) {
					selectedRange.LeftColumnIndex = i;
					break;
				}
			}
			for (int i = right; i >= left; i--) {
				Column col = ActiveSheet.Columns.TryGetColumn(i);
				if (col == null || !col.IsHidden) {
					selectedRange.RightColumnIndex = i;
					break;
				}
			}
			if (!selectionUpdated) {
				int i = ActiveSheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight ? right : left;
				Column col = ActiveSheet.Columns.TryGetColumn(i);
				while (col != null && col.IsHidden) {
					i = ActiveSheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight ? i + 1 : i - 1;
					col = ActiveSheet.Columns.TryGetColumn(i);
				}
				selectedRange.RightColumnIndex = i;
				selectedRange.LeftColumnIndex = i;
			}
		}
		void UpdateRowSelection() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			int top = selectedRange.TopRowIndex;
			int bottom = selectedRange.BottomRowIndex;
			bool selectionUpdated = false;
			for (int i = top; i <= bottom; i++) {
				Row row = ActiveSheet.Rows.TryGetRow(i);
				if (row == null || !row.IsHidden) {
					selectedRange.TopRowIndex = i;
					selectionUpdated = true;
					break;
				}
			}
			for (int i = bottom; i >= top; i--) {
				Row row = ActiveSheet.Rows.TryGetRow(i);
				if (row == null || !row.IsHidden) {
					selectedRange.BottomRowIndex = i;
					selectionUpdated = true;
					break;
				}
			}
			if (!selectionUpdated) {
				int i = ActiveSheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow ? bottom : top;
				Row row = ActiveSheet.Rows.TryGetRow(i);
				while (row != null && row.IsHidden) {
					i = ActiveSheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow ? i + 1 : i - 1;
					row = ActiveSheet.Rows.TryGetRow(i);
				}
				selectedRange.BottomRowIndex = i;
				selectedRange.TopRowIndex = i;
			}
		}
	}
	#endregion
}
