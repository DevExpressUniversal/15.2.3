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

using System;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FindCommand
	public class FindCommand : FindReplaceCommandBase {
		public FindCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditingFind; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingFind; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingFindDescription; } }
		protected override bool UseOfficeImage { get { return true; } }
		public override string ImageName { get { return "Find"; } }
		protected internal override bool IsReplaceCommand { get { return false; } }
		#endregion
		protected internal override void PerformDefaultAction(FindReplaceViewModel viewModel) {
			FindNext(viewModel);
		}
		protected internal void FindNext(FindReplaceViewModel viewModel) {
			if (!PerformFindNext(viewModel))
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_SearchCantFindData));
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive;
			state.Visible = true;
		}
	}
	#endregion
	#region FindReplaceCommandBase (abstract class)
	public abstract class FindReplaceCommandBase : SpreadsheetSelectionCommand {
		protected FindReplaceCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal abstract bool IsReplaceCommand { get; }
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<FindReplaceViewModel> valueBasedState = state as IValueBasedCommandUIState<FindReplaceViewModel>;
				if (valueBasedState == null)
					return;
				if (InnerControl.AllowShowingForms)
					Control.ShowFindReplaceForm(valueBasedState.Value);
				else {
					PerformDefaultAction(valueBasedState.Value);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<FindReplaceViewModel> state = new DefaultValueBasedCommandUIState<FindReplaceViewModel>();
			state.Value = CreateViewModel();
			return state;
		}
		protected internal virtual FindReplaceViewModel CreateViewModel() {
			FindReplaceViewModel viewModel = new FindReplaceViewModel(Control);
			viewModel.ReplaceMode = IsReplaceCommand;
			return viewModel;
		}
		protected internal override bool ChangeSelection() {
			return true;
		}
		protected bool PerformFindNext(FindReplaceViewModel viewModel) {
			DocumentLayout documentLayout = this.InnerControl.DesignDocumentLayout;
			DocumentModel.BeginUpdateFromUI();
			try {
				bool found = ChangeSelection(viewModel);
				RecalculateLayoutEnsureActiveCellVisible(documentLayout);
				return found;
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected virtual bool ChangeSelection(FindReplaceViewModel viewModel) {
			if (ActiveSheet.Selection.SelectedRanges.Count > 1)
				return SpreadsheetSearchHelper.SearchInMultiSelection(ActiveSheet, viewModel);
			else
				return SpreadsheetSearchHelper.SearchInSingleSelection(ActiveSheet, viewModel);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
		}
		protected internal abstract void PerformDefaultAction(FindReplaceViewModel viewModel);
	}
	#endregion
}
