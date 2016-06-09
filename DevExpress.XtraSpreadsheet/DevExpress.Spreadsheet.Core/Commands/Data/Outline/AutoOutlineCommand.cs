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
	#region AutoOutlineCommand
	public class AutoOutlineCommand : UngroupCommand {
		public AutoOutlineCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.AutoOutline; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_AutoOutlineCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_AutoOutlineCommand; } }
		public override string ImageName { get { return string.Empty; } }
		#endregion
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<CellRangeBase> valueBasedState = state as IValueBasedCommandUIState<CellRangeBase>;
				if (valueBasedState == null || valueBasedState.Value.RangeType == CellRangeType.UnionRange)
					return;
				if (InnerControl.AllowShowingForms && HasOutline() && !Control.ShowYesNoMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ConfirmModifyExistingOutline)))
					return;
				CellRange processedRange = valueBasedState.Value as CellRange;
				if (processedRange == null || (processedRange.Width == 1 && processedRange.Height == 1))
					processedRange = ActiveSheet.GetUsedRange();
				AutoCreateOutlineCommand command = new AutoCreateOutlineCommand(ActiveSheet, processedRange);
				if (!command.Execute() && InnerControl.AllowShowingForms)
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CannotCreateOutline));
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Group.Group);
			ApplyActiveSheetProtection(state);
		}
		bool HasOutline() {
			return ActiveSheet.Properties.FormatProperties.OutlineLevelCol > 0 || ActiveSheet.Properties.FormatProperties.OutlineLevelRow > 0;
		}
	}
	#endregion
}
