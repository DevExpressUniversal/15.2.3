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
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class DefineNameCommand : SpreadsheetMenuItemSimpleCommand {
		public DefineNameCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormulasDefineNameCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DefineNameCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DefineNameCommandDescription; } }
		public override string ImageName { get { return "DefineName"; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.ShowDefineNameForm(CreateViewModel());
		}
		public DefineNameViewModel CreateViewModel() {
			DefineNameViewModel viewModel = new DefineNameViewModel(Control);
			viewModel.IsScopeChangeAllowed = true;
			viewModel.NewNameMode = true;
			viewModel.ScopeIndex = 0;
			viewModel.Scope = viewModel.ScopeDataSource[0];
			viewModel.Reference = CreateSelectionReferenceString(ActiveSheet);
			return viewModel;
		}
		public static string CreateSelectionReferenceString(Worksheet sheet) {
			WorkbookDataContext context = sheet.Workbook.DataContext;
			CellRange range = ConvertToAbsoluteRange(sheet.Selection.SelectedRanges[0]);
			return "=" + CellRangeToString.GetReferenceCommon(range, context.UseR1C1ReferenceStyle, range.TopLeft, PositionType.Absolute, PositionType.Absolute, true);
		}
		public static CellRange ConvertToAbsoluteRange(CellRange range) {
			if (range.IsColumnRangeInterval())
				return CellIntervalRange.CreateColumnInterval(range.Worksheet, range.TopLeft.Column, PositionType.Absolute, range.BottomRight.Column, PositionType.Absolute);
			if (range.IsRowRangeInterval())
				return CellIntervalRange.CreateRowInterval(range.Worksheet, range.TopLeft.Row, PositionType.Absolute, range.BottomRight.Row, PositionType.Absolute);
			return new CellRange(range.Worksheet, range.TopLeft.AsAbsolute(), range.BottomRight.AsAbsolute());
		}
		public bool Validate(DefineNameViewModel viewModel) {
			if (Control.InnerControl.RaiseDefinedNameValidating(viewModel.Name, viewModel.OriginalName, viewModel.Scope, viewModel.ScopeIndex, viewModel.Reference, viewModel.Comment))
				return false;
			DefinedName defName = new DefinedName(DocumentModel, viewModel.Reference, viewModel.Reference, viewModel.ScopeIndex);
			string errorMessage = DocumentModel.CheckDefinedNameCore(viewModel.Name, viewModel.SheetId, viewModel.OriginalName);
			CellPosition activeCell = DocumentModel.ActiveSheet.Selection.ActiveCell;
			if (String.IsNullOrEmpty(defName.GetReference(activeCell.Column, activeCell.Row))) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidFormula));
				return false;
			}
			if (!String.IsNullOrEmpty(errorMessage)) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefinedNameInvalidName));
				return false;
			}
			return true;
		}
		public void ApplyChanges(DefineNameViewModel viewModel) {
			if (viewModel.NewNameMode)
				CreateNewDefinedName(viewModel);
			else
				ModifyExistingDefinedName(viewModel);
		}
		void CreateNewDefinedName(DefineNameViewModel viewModel) {
			WorkbookDataContext dataContext = DocumentModel.DataContext;
			dataContext.PushCurrentCell(DocumentModel.ActiveSheet.Selection.ActiveCell);
			DocumentModel.BeginUpdate();
			try {
				int sheetId = viewModel.SheetId;
				DefinedName definedName;
				if (sheetId < 0) {
					DefinedNameWorkbookCreateCommand command = new DefinedNameWorkbookCreateCommand(DocumentModel, viewModel.Name, ValidateReference(viewModel.Reference));
					command.Execute();
					definedName = command.Result as DefinedName;
				}
				else {
					Worksheet sheet = DocumentModel.Sheets.GetById(sheetId);
					DefinedNameWorksheetCreateCommand command = new DefinedNameWorksheetCreateCommand(sheet, viewModel.Name, ValidateReference(viewModel.Reference));
					command.Execute();
					definedName = command.Result as DefinedName;
				}
				if (definedName != null)
					definedName.Comment = viewModel.Comment;
			}
			finally {
				DocumentModel.EndUpdate();
				dataContext.PopCurrentCell();
			}
		}
		void ModifyExistingDefinedName(DefineNameViewModel viewModel) {
			DefinedName definedName = FindDefinedName(viewModel);
			if (definedName == null)
				return;
			WorkbookDataContext dataContext = DocumentModel.DataContext;
			dataContext.PushCurrentCell(DocumentModel.ActiveSheet.Selection.ActiveCell);
			DocumentModel.BeginUpdate();
			try {
				definedName.Comment = viewModel.Comment;
				definedName.SetReference(ValidateReference(viewModel.Reference));
				if (definedName.Name != viewModel.Name) {
					DefinedNameRenamedCommand command = new DefinedNameRenamedCommand(definedName, viewModel.Name);
					command.Execute();
				}
			}
			finally {
				DocumentModel.EndUpdate();
				dataContext.PopCurrentCell();
			}
			DocumentModel.RaiseSchemaChanged();
		}
		protected internal string ValidateReference(string reference) {
			if (!string.IsNullOrEmpty(reference)) {
				if (reference[0] == '=')
					return reference.Substring(1);
				FormattedVariantValue formattedValue = CellValueFormatter.GetValue(VariantValue.Empty, reference, DocumentModel.DataContext, false);
				if (formattedValue.Value.IsInlineText) {
					StringBuilder sb = new StringBuilder();
					sb.Append("\"");
					sb.Append(reference.Replace("\"", "\"\""));
					sb.Append("\"");
					return sb.ToString();
				}
			}
			return reference;
		}
		protected internal DefinedName FindDefinedName(DefineNameViewModel viewModel) {
			DefinedNameBase result;
			int sheetId = viewModel.SheetId;
			if (sheetId < 0)
				DocumentModel.DefinedNames.TryGetItemByName(viewModel.OriginalName, out result);
			else
				DocumentModel.Sheets.GetById(sheetId).DefinedNames.TryGetItemByName(viewModel.OriginalName, out result);
			return result as DefinedName;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state);
		}
	}
}
