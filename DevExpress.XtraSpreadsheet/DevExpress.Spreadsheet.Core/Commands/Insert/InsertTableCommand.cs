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

using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using System;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertTableUICommand
	public class InsertTableUICommand : SpreadsheetCommand {
		#region Static Members
		public static string GetReferenceCommon(CellRange range, WorkbookDataContext context) {
			return "=" + CellRangeToString.GetReferenceCommon(range, context.UseR1C1ReferenceStyle, CellPosition.InvalidValue, PositionType.Absolute, PositionType.Absolute, false);
		}
		#endregion
		public InsertTableUICommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertTable; } }
		public override Localization.XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertTableDescription; } }
		public override string ImageName { get { return "InsertTable"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<InsertTableViewModel> valueBasedState = state as IValueBasedCommandUIState<InsertTableViewModel>;
				if (valueBasedState == null)
					return;
				if (InnerControl.AllowShowingForms) {
					InsertTableViewModel viewModel = CreateViewModel();
					Control.ShowTableInsertForm(viewModel);
				}
				else {
					InsertTableCommand command = CreateCreationCommand(valueBasedState.Value);
					command.Execute();
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual InsertTableCommand CreateCreationCommand(InsertTableViewModel viewModel) {
			InsertTableWithDefaultStyleCommand command = new InsertTableWithDefaultStyleCommand(ErrorHandler, ActiveSheet, string.Empty, viewModel.HasHeaders, true);
			command.Reference = viewModel.Reference;
			command.Style = viewModel.Style;
			return command;
		}
		protected internal bool Validate(InsertTableViewModel viewModel) {
			InsertTableCommand command = CreateCreationCommand(viewModel);
			return command.Validate();
		}
		protected internal void ApplyChanges(InsertTableViewModel viewModel) {
			DocumentModel.BeginUpdateFromUI();
			try {
				InsertTableCommand command = CreateCreationCommand(viewModel);
				if (command.Validate())
					command.ExecuteCore();
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal void ShowTableInsertFormCallback(InsertTableCommand command) {
			DocumentModel.BeginUpdateFromUI();
			try {
				command.ExecuteCore();
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected string GetReferenceCommon() {
			return GetReferenceCommon(GetActiveRange(), DocumentModel.DataContext);
		}
		CellRange GetActiveRange() {
			SheetViewSelection selection = ActiveSheet.Selection;
			CellRange activeRange = selection.ActiveRange;
			SheetAutoFilter filter = ActiveSheet.AutoFilter;
			if (filter.Enabled) {
				CellRange filterRange = filter.Range;
				if (activeRange.CellCount == 1 && filterRange.ContainsRange(activeRange)) {
					selection.SetSelection(filterRange);
					activeRange = filterRange.Clone();
				}
			}
			return activeRange;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<InsertTableViewModel>();
		}
		protected internal virtual InsertTableViewModel CreateViewModel() {
			InsertTableViewModel viewModel = new InsertTableViewModel(Control);
			viewModel.Reference = GetReferenceCommon();
			viewModel.Style = String.Empty;
			viewModel.HasHeaders = false;
			return viewModel;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool additionalEnabled = !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive && ActiveSheet.Selection.GetActiveTables().Count == 0;
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, additionalEnabled);
			ApplyActiveSheetProtection(state);
			if (ActiveSheet.TryGetSelectedTableBases(true).Count > 0)
				state.Enabled = false;
		}
	}
	#endregion
}
