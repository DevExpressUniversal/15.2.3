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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatNumberCommand
	public class FormatNumberCommand : SpreadsheetCommand {
		static List<SpreadsheetCommandId> commandIds = CreateCommandIds();
		static List<SpreadsheetCommandId> CreateCommandIds() {
			List<SpreadsheetCommandId> result = new List<SpreadsheetCommandId>();
			result.Add(SpreadsheetCommandId.FormatNumberGeneral);
			result.Add(SpreadsheetCommandId.FormatNumberDecimal);
			result.Add(SpreadsheetCommandId.FormatNumberAccountingCurrency);
			result.Add(SpreadsheetCommandId.FormatNumberAccountingRegular);
			result.Add(SpreadsheetCommandId.FormatNumberShortDate);
			result.Add(SpreadsheetCommandId.FormatNumberLongDate);
			result.Add(SpreadsheetCommandId.FormatNumberTime);
			result.Add(SpreadsheetCommandId.FormatNumberPercentage);
			result.Add(SpreadsheetCommandId.FormatNumberFraction);
			result.Add(SpreadsheetCommandId.FormatNumberScientific);
			result.Add(SpreadsheetCommandId.FormatNumberText);
			return result;
		}
		public FormatNumberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatNumber; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatNumber; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatNumberDescription; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				DefaultValueBasedCommandUIState<string> valueBasedState = state as DefaultValueBasedCommandUIState<string>;
				if (valueBasedState == null)
					return;
				string numberFormatName = valueBasedState.Value;
				if (!String.IsNullOrEmpty(numberFormatName))
					ExecuteCore(numberFormatName);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal void ExecuteCore(string numberFormatName) {
			SpreadsheetCommand command = GetCommandByFormatName(numberFormatName);
			if (command != null)
				command.Execute();
		}
		SpreadsheetCommand GetCommandByFormatName(string numberFormatName) {
			foreach (SpreadsheetCommandId commandId in commandIds) {
				SpreadsheetCommand command = InnerControl.CreateCommand(commandId);
				if (StringExtensions.CompareInvariantCultureIgnoreCase(command.MenuCaption, numberFormatName) == 0)
					return command;
			}
			return null;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			bool conditions = !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly;
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, conditions);
			if (!state.Visible || !state.Enabled)
				return;
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState == null)
				return;
			CellPosition activeCellPosition = ActiveSheet.Selection.ActiveCell;
			ICell activeCell = ActiveSheet.GetCellForFormatting(activeCellPosition.Column, activeCellPosition.Row);
			valueBasedState.Value = FindNumberFormatName(activeCell.ActualFormatString);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<string>();
		}
		string FindNumberFormatName(string formatCode) {
			foreach (SpreadsheetCommandId commandId in commandIds) {
				ChangeSelectedCellsNumberFormatCommand command = InnerControl.CreateCommand(commandId) as ChangeSelectedCellsNumberFormatCommand;
				if (command == null)
					continue;
				if (StringExtensions.CompareInvariantCultureIgnoreCase(command.FormatString, formatCode) == 0)
					return command.MenuCaption;
			}
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberCustom);
		}
	}
	#endregion
}
