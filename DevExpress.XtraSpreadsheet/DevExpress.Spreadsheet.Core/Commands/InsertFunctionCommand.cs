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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertFunctionCommand
	public class InsertFunctionCommand : SpreadsheetMenuItemSimpleCommand {
		#region fields
		#endregion
		public InsertFunctionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertFunction; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IFormulaBarControl formulaBar = (IFormulaBarControl)Control.GetService(typeof(IFormulaBarControl));
				if (formulaBar != null && InnerControl.AllowShowingForms) {
					if(!InnerControl.IsInplaceEditorActive) {
						if (!InnerControl.TryEditActiveCellContent())
							return;
						InnerControl.ActivateCellInplaceEditor(ActiveSheet.Selection.ActiveCell, null, int.MaxValue, CellEditorMode.EditInFormulaBar);
					}
					if(ShouldExecuteEditFunctionArgumentsCommand()) {
						ExecuteEditFunctionArgsCommand();
					}
					else {
						InsertFunctionViewModel viewModel = new InsertFunctionViewModel(Control);
						Control.ShowInsertFunctionForm(viewModel);
					}
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal override void ExecuteCore() { throw new System.InvalidOperationException(); }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !ActiveSheet.Selection.IsDrawingSelected && IsContentEditable;
			state.Visible = true;
			state.Checked = false;
		}
		public void ApplyChanges(string function, int selectionStart, int selectionLength) {
			if(!InnerControl.IsInplaceEditorActive) {
				InnerControl.ActivateCellInplaceEditor(ActiveSheet.Selection.ActiveCell, "", CellEditorMode.EditInFormulaBar);
			}
			this.InnerControl.InplaceEditor.InsertFunction(function, selectionStart, selectionLength);
			ExecuteEditFunctionArgsCommand();
		}
		void ExecuteEditFunctionArgsCommand() {
			EditFunctionArgumentsCommand command = new EditFunctionArgumentsCommand(Control);
			command.Execute();
		}
		bool ShouldExecuteEditFunctionArgumentsCommand() {
			if(!InnerControl.IsInplaceEditorActive) {
				return false;
			}
			InnerCellInplaceEditor editor = InnerControl.InplaceEditor;
			string formulaBody = editor.Text;
			int selectionStart = editor.SelectionStart;
			int selectionLength = editor.SelectionLength;
			FunctionArgumentsViewModel viewModel = new FunctionArgumentsViewModel(Control, formulaBody, selectionStart, selectionLength);
			return viewModel.MayExecute();
		}
	}
	#endregion
	#region EditFunctionArgumentsCommand
	public class EditFunctionArgumentsCommand : SpreadsheetMenuItemSimpleCommand {
		#region Fields
		FunctionArgumentsViewModel viewModel;
		#endregion
		public EditFunctionArgumentsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IFormulaBarControl formulaBar = (IFormulaBarControl)Control.GetService(typeof(IFormulaBarControl));
				if (formulaBar!=null && InnerControl.AllowShowingForms) {
					IFormulaBarControllerOwner formulaBarControllerOwner = formulaBar as IFormulaBarControllerOwner;
					if(formulaBarControllerOwner != null) {
						if(formulaBarControllerOwner.Controller.EditFunctionArgumentsCommand == null)
							formulaBarControllerOwner.Controller.EditFunctionArgumentsCommand = this;
					}
					InnerControl.InplaceEditor.DeactivateHandlers();
					string formulaBody = InnerControl.InplaceEditor.Text;
					int selectionStart = InnerControl.InplaceEditor.SelectionStart;
					int selectionLength = InnerControl.InplaceEditor.SelectionLength;
					formulaBar.InplaceEditor.SetFocus();
					formulaBar.InplaceEditor.SetSelection(selectionStart, selectionLength);
					viewModel = new FunctionArgumentsViewModel(Control, formulaBody, selectionStart, selectionLength);
					Control.ShowFunctionArgumentsForm(viewModel);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal override void ExecuteCore() { throw new System.InvalidOperationException(); }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !ActiveSheet.Selection.IsDrawingSelected && IsContentEditable;
			state.Visible = true;
			state.Checked = false;
		}
		public void ApplyChanges(FunctionArgumentsViewModel functionArgumentsViewModel) {
			InnerControl.InplaceEditor.ActivateHandlers();
			if(functionArgumentsViewModel.KeepChanges) {
				this.InnerControl.InplaceEditor.InsertFunction(functionArgumentsViewModel.ResultFormula);
				InplaceEndEditCommand command = new InplaceEndEditCommand(Control);
				command.Execute();
			}
			else {
				InplaceCancelEditCommand command = new InplaceCancelEditCommand(Control);
				command.Execute();
			}
		}
		public void UpdateViewModel() {
			string text = InnerControl.InplaceEditor.Text;
			int selectionStart = InnerControl.InplaceEditor.SelectionStart;
			int selectionLength = InnerControl.InplaceEditor.SelectionLength;
			if(viewModel.Update(text, selectionStart, selectionLength))
				SetFocusToFormulaBar();
		}
		public void UpdateModel(string text) {
			this.InnerControl.InplaceEditor.InsertFunction(text);
		}
		public void UnSubscribeCommand() {
			IFormulaBarControl formulaBar = (IFormulaBarControl)Control.GetService(typeof(IFormulaBarControl));
			IFormulaBarControllerOwner formulaBarControllerOwner = formulaBar as IFormulaBarControllerOwner;
			if (formulaBarControllerOwner != null) {
				formulaBarControllerOwner.Controller.EditFunctionArgumentsCommand = null;
			}
		}
		public void SetFocusToFormulaBar() {
			IFormulaBarControl formulaBar = (IFormulaBarControl)Control.GetService(typeof(IFormulaBarControl));
			if (formulaBar==null)
				return;
			formulaBar.InplaceEditor.SetFocus();
		}
	}
	#endregion
}
