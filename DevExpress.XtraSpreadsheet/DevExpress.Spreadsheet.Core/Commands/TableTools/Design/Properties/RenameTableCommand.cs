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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region TableToolsRenameTableCommand
	public class TableToolsRenameTableCommand : ModifyTableStyleOptionsCommandBase {
		public TableToolsRenameTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public string TableName { get; set; }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_TableToolsRenameTableCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_TableToolsRenameTableCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.TableToolsRenameTable; } }
		protected override bool ProtectionOption { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState == null)
				return;
			TableName = valueBasedState.Value;
			base.ForceExecute(state);
		}
		protected override bool IsChecked(Table table) {
			return table.Name == TableName;
		}
		protected override void ModifyTable(Table table) {
			string errorMessage = ValidateTableName();
			if (!String.IsNullOrEmpty(errorMessage)) {
				Control.ShowWarningMessage(errorMessage);
				return;
			}
			table.Name = TableName;
		}
		protected override void ModifyState(Table activeTable, ICommandUIState state) {
			state.EditValue = activeTable.Name;
		}
		string ValidateTableName() {
			if (!WorkbookDataContext.IsIdent(TableName))
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorTableNameIsNotValid);
			DocumentModel workbook = Control.InnerControl.DocumentModel;
			if (workbook.DefinedNames.Contains(TableName))
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorTableAlreadyExists);
			foreach (Worksheet sheet in workbook.Sheets)
				if (sheet.Tables.Contains(TableName) || sheet.DefinedNames.Contains(TableName))
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorTableAlreadyExists);
			return String.Empty;
		}
	}
	#endregion
	#region TableNameCommand
	public class TableNameCommand : ErrorHandledWorksheetCommand {
		public TableNameCommand(ISpreadsheetControl control)
			: base(control.InnerDocumentServer.DocumentModel, control.InnerControl.ErrorHandler) {
		}
		public bool Validate(DefineNameViewModel viewModel) {
			if (viewModel.Name == viewModel.OriginalName)
				return true;
			IModelErrorInfo error = DocumentModel.CheckTableName(viewModel.Name);
			if (error == null)
				return true;
			HandleError(error);
			return false;
		}
		public void ApplyChanges(DefineNameViewModel viewModel) {
			Table table = DocumentModel.GetTableByName(viewModel.OriginalName);
			if (table == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				table.Comment = viewModel.Comment;
				if (viewModel.Name != viewModel.OriginalName) {
					TableRenameCommand command = new TableRenameCommand(table, viewModel.Name);
					command.Execute();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void ExecuteCore() {
		}
	}
	#endregion
}
