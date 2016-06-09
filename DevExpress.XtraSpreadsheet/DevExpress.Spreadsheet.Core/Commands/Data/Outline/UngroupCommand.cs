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
	#region UngroupCommand
	public class UngroupCommand :SpreadsheetMenuItemSimpleCommand {
		#region fields
		CellRangeBase selectedRange;
		#endregion
		public UngroupCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.UngroupOutline; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UngroupCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UngroupCommand; } }
		public override string ImageName { get { return "Ungroup"; } }
		#endregion
		protected internal override void ExecuteCore() {
			ActiveSheet.Workbook.BeginUpdateFromUI();
			try {
				if (selectedRange.RangeType == CellRangeType.UnionRange) {
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorCommandCannotPerformedWithMultipleSelections));
					return;
				}
				CellRange range = selectedRange as CellRange;
				if (!NeedShowForm(range)) {
					bool rows = NeedGroupRows(range);
					ApplyChanges(rows, range);
				}
				else
					if (InnerControl.AllowShowingForms)
						ShowForm(range);
			}
			finally {
				ActiveSheet.Workbook.EndUpdateFromUI();
			}
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<CellRangeBase> valueBasedState = state as IValueBasedCommandUIState<CellRangeBase>;
				if (valueBasedState != null) {
					selectedRange = valueBasedState.Value;
					ExecuteCore();
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected virtual bool NeedShowForm(CellRange selectedRange) {
			return selectedRange.RangeType != CellRangeType.IntervalRange
				&& ActiveSheet.Properties.FormatProperties.OutlineLevelCol != 0
				&& ActiveSheet.Properties.FormatProperties.OutlineLevelRow != 0;
		}
		protected virtual bool NeedGroupRows(CellRange selectedRange) {
			return selectedRange.IsRowRangeInterval() || ActiveSheet.Properties.FormatProperties.OutlineLevelCol == 0;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Group.Ungroup, GetEnabled());
			ApplyActiveSheetProtection(state);
		}
		bool GetEnabled() {
			return ActiveSheet.Properties.FormatProperties.OutlineLevelCol > 0 || ActiveSheet.Properties.FormatProperties.OutlineLevelRow > 0;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			CellRangeBase selectedRange = ActiveSheet.Selection.AsRange();
			DefaultValueBasedCommandUIState<CellRangeBase> state = new DefaultValueBasedCommandUIState<CellRangeBase>();
			state.Value = selectedRange;
			return state;
		}
		protected virtual void ShowForm(CellRange range) {
			UngroupViewModel viewModel = new UngroupViewModel(Control, range);
			Control.ShowGroupUngroupForm(viewModel);
		}
		public virtual void ApplyChanges(bool rows, CellRange range) {
			UngroupCommandBase command = UngroupCommandBase.CreateUngroupInstance(rows, ActiveSheet, range);
			command.Collapse = false;
			command.UnhideCollapsed = true;
			command.Execute();
		}
	}
	#endregion
}
