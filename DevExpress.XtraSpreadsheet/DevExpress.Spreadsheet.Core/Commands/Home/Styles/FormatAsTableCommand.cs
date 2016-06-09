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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatAsTableCommand
	public class FormatAsTableCommand : InsertTableUICommand {
		public FormatAsTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		ITableBase ActiveTable { get; set; }
		string StyleName { get; set; }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAsTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAsTableDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAsTable; } }
		public override string ImageName { get { return "FormatAsTable"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			if (ActiveTable == null) {
				IValueBasedCommandUIState<InsertTableViewModel> valueBasedState = state as IValueBasedCommandUIState<InsertTableViewModel>;
				if (valueBasedState == null)
					return;
				StyleName = valueBasedState.Value.Style;
				base.ForceExecute(state);
			}
			else {
				IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
				if (valueBasedState == null)
					return;
				StyleName = valueBasedState.Value;
				DocumentModel.ActiveSheet.ApplyStyleToActiveTableBases(StyleName, true);
			}
		}
		protected internal override InsertTableCommand CreateCreationCommand(InsertTableViewModel viewModel) {
			InsertTableWithStyleCommand command = new InsertTableWithStyleCommand(ErrorHandler, ActiveSheet, string.Empty, viewModel.HasHeaders, true, StyleName);
			command.Reference = viewModel.Reference;
			command.Style = viewModel.Style;
			return command;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool additionalEnabled = !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive;
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, additionalEnabled);
			ApplyActiveSheetProtection(state);
			if (state.Enabled && state.Visible) {
				ActiveTable = ActiveSheet.TryGetActiveTableBase();
				if (ActiveTable == null && ActiveSheet.TryGetSelectedTableBases(true).Count > 0) {
					state.Enabled = false;
					return;
				}
				if (ActiveTable == null) {
					IValueBasedCommandUIState<InsertTableViewModel> valueBasedState = state as IValueBasedCommandUIState<InsertTableViewModel>;
					if (valueBasedState != null)
						valueBasedState.Value = CreateViewModel();
				} else {
					IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
					if (valueBasedState != null)
						valueBasedState.Value = GetStyleName();
				}
			}
		}
		protected internal override InsertTableViewModel CreateViewModel() {
			FormatAsTableViewModel viewModel = new FormatAsTableViewModel(Control);
			viewModel.Reference = GetReferenceCommon();
			viewModel.Style = StyleName;
			viewModel.HasHeaders = false;
			return viewModel;
		}
		string GetStyleName() {
			TableStyle style = ActiveTable.Style;
			return style != null ? style.Name.Name : DocumentModel.StyleSheet.TableStyles.GetDefaultStyleName(ActiveTable is Table);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			if (ActiveTable == null) 
				return base.CreateDefaultCommandUIState();
			else {
				DefaultValueBasedCommandUIState<string> result = new DefaultValueBasedCommandUIState<string>();
				result.Value = GetStyleName();
				return result;
			}
		}
	}
	#endregion
}
