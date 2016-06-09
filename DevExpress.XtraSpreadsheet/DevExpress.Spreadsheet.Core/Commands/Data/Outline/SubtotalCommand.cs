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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SubtotalCommand
	public class SubtotalCommand : SpreadsheetMenuItemSimpleCommand {
		#region fields
		CellRange selectedRange;
		#endregion
		public SubtotalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.Subtotal; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_SubtotalCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_SubtotalCommand; } }
		public override string ImageName { get { return "Subtotal"; } }
		#endregion
		protected internal override void ExecuteCore() {
			ActiveSheet.Workbook.BeginUpdateFromUI();
			try {
				if (selectedRange == null) {
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorSubtotalUncomplete));
					return;
				}
				CellRange firstSelectedRow = selectedRange.GetSubRowRange(0, 0);
				if (!ValidateColumnNames(firstSelectedRow)) {
					if (!Control.ShowOkCancelMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_SubtotalNeedColumnNames)))
						return;
				}
				if (InnerControl.AllowShowingForms) {
					List<string> columnNames = GetColumnsNames(firstSelectedRow);
					OutlineSubtotalViewModel vieModel = new OutlineSubtotalViewModel(Control, selectedRange.GetSubRowRange(1, selectedRange.Height - 1), ActiveSheet, columnNames);
					Control.ShowOutlineSubtotalForm(vieModel);
				}
			}
			finally {
				ActiveSheet.Workbook.EndUpdateFromUI();
			}
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<CellRange> valueBasedState = state as IValueBasedCommandUIState<CellRange>;
				if (valueBasedState == null)
					return;
				CellRange currentSelectedRange = ActiveSheet.Selection.SelectedRanges[0].Clone();
				if (currentSelectedRange.Height == 1)
					currentSelectedRange = ActiveSheet.Selection.GetResultRange(currentSelectedRange, currentSelectedRange);
				while (!ActiveSheet.RangeContainsNotEmptyCell(currentSelectedRange.GetSubRowRange(0, 0)) && currentSelectedRange.Height != 1)
					currentSelectedRange.TopRowIndex++;
				selectedRange = currentSelectedRange.Height == 1 ? null : currentSelectedRange;
				string errorMessage = GetErrorMessage();
				if (!string.IsNullOrEmpty(errorMessage)) {
					Control.ShowWarningMessage(errorMessage);
					return;
				}
				ExecuteCore();
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<CellRange>();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Group.Group, GetEnabled());
			ApplyActiveSheetProtection(state);
		}
		bool GetEnabled() {
			bool result = ActiveSheet.Tables.CanInsertSubtotal(ActiveSheet.Selection.SelectedRanges[0]);
			return result & !ActiveSheet.PivotTables.ContainsItemsInRange(ActiveSheet.Selection.ActiveRange, true);
		}
		public void ApplyChanges(OutlineSubtotalViewModel viewModel) {
			bool needInsertTextColumn = true;
			if (viewModel.ReplaceCurrentSubtotals)
				needInsertTextColumn = !RemoveAllSubtotals(viewModel.Range);
			ActiveSheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow = viewModel.ShowRowSumsBelow;
			SubtotalModelCommand command = new SubtotalModelCommand(ActiveSheet, this.ErrorHandler, viewModel.Range);
			command.ChangedColumnIndex = viewModel.ChangedColumnIndex;
			command.SubTotalColumnIndices = viewModel.SubtotalColumnIndices;
			command.FunctionType = viewModel.FunctionIndex;
			command.Text = viewModel.FunctionText;
			command.PageBreakBeetwenGroups = viewModel.PageBreakBeetwenGroups;
			command.NeedInsertTextColumn = needInsertTextColumn;
			command.Execute();
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (selectedRange.Height > 1) {
				selectedRange.BottomRowIndex += command.InsertedRowsCount;
				if (command.NeedInsertTextColumn)
					selectedRange.RightColumnIndex++;
			}
		}
		public bool RemoveAllSubtotals(CellRange range) {
			SubtotalRemoveCommand command = new SubtotalRemoveCommand(ActiveSheet, range, ErrorHandler);
			if (command.Execute())
				return command.HasTextColumn;
			return false;
		}
		bool ValidateColumnNames(CellRange firstRow) {
			foreach (ICell cell in firstRow.GetExistingCellsEnumerable())
				if (cell.Value.Type == VariantValueType.SharedString || cell.Value.Type == VariantValueType.InlineText)
					return true;
			return false;
		}
		List<string> GetColumnsNames(CellRange firstRow) {
			List<string> result = new List<string>();
			for(int i = firstRow.LeftColumnIndex; i<= firstRow.RightColumnIndex; i++){
				ICell cell = ActiveSheet.GetRegisteredCell(i, firstRow.TopRowIndex);
				if(cell == null || string.IsNullOrEmpty(cell.Text)){
					result.Add(string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Subtotal_ColumnHeader), CellReferenceParser.ColumnIndexToString(i)));
					continue;
				}
				string currentValue = cell.Text;
				if (result.Contains(currentValue))
					result.Add(GetNewColumnName(result, currentValue));
				else
					result.Add(currentValue);
			}
			return result;
		}
		string GetNewColumnName(List<string> names, string name) {
			int i = 1;
			while (names.Contains(string.Format("({0}) {1}", i, name)))
				i++;
			return string.Format("({0}) {1}", i, name);
		}
		string GetErrorMessage() {
			if (ActiveSheet.ArrayFormulaRanges.HasIntersectedSubtotalArrays(selectedRange))
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorCannotChangingPartOfAnArray);
			return string.Empty;
		}
	}
	#endregion
}
