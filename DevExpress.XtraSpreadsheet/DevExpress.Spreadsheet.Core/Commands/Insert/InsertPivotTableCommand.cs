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
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertPivotTableCommand
	public class InsertPivotTableCommand : PivotTableCommandBase {
		public InsertPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertPivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertPivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertPivotTableDescription; } }
		public override string ImageName { get { return "InsertPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (InnerControl.AllowShowingForms)
				Control.ShowInsertPivotTableForm(CreateViewModel());
		}
		protected internal virtual InsertPivotTableViewModel CreateViewModel() {
			InsertPivotTableViewModel viewModel = new InsertPivotTableViewModel(Control);
			viewModel.Source = PivotTableCommandBase.CreateReferenceString(ActiveSheet, ActiveSheet.Selection.SelectedRanges[0]);
			viewModel.NewWorksheet = true;
			viewModel.Location = String.Empty;
			return viewModel;
		}
		protected internal bool Validate(InsertPivotTableViewModel viewModel) {
			if (!ValidateReferences(viewModel))
				return false;
			if (!PivotCacheCreateCommand.Validate(viewModel.PivotSource, DocumentModel.DataContext, ErrorHandler))
				return false;
			return true;
		}
		bool ValidateReferences(InsertPivotTableViewModel viewModel) {
			WorkbookDataContext dataContext = Control.Document.Model.DocumentModel.DataContext;
			viewModel.PivotSource = PivotCacheSourceWorksheet.CreateInstance(viewModel.Source, dataContext);
			viewModel.LocationRange = CellRangeBase.TryParse(viewModel.Location, dataContext) as CellRange;
			if (!viewModel.NewWorksheet) {
				if ((viewModel.PivotSource == null || viewModel.PivotSource.GetRange(dataContext) == null) && viewModel.LocationRange == null) {
					if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableDataSourceAndDestinationReferencesAreBothInvalid)))
						return false;
				}
				else if (viewModel.LocationRange == null)
					if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableDestinationReferenceNotValid)))
						return false;
			}
			if ((viewModel.PivotSource == null || viewModel.PivotSource.GetRange(dataContext) == null))
				if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableDataSourceReferenceNotValid)))
					return false;
			return true;
		}
		protected internal void ApplyChanges(InsertPivotTableViewModel viewModel) {
			DocumentModel.BeginUpdate();
			try {
				if (viewModel.NewWorksheet) {
					new InsertSheetCommand(Control).ExecuteCore();
					GetPivotTableLocationFromNewSheet(viewModel);
				}
				else
					ActiveSheet.Selection.SetSelection(viewModel.LocationRange.TopLeft);
				PivotCreateCommand commandPivotCreate = new PivotCreateCommand(ErrorHandler, viewModel.PivotSource, viewModel.LocationRange);
				commandPivotCreate.Execute();
				if (commandPivotCreate.Result != null)
					(commandPivotCreate.Result as PivotTable).StyleInfo.StyleName = "PivotStyleLight16";
				InnerControl.ResetPivotTableFieldsPanelVisibility();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void GetPivotTableLocationFromNewSheet(InsertPivotTableViewModel viewModel) {
			CellPosition activeCellPosition = new CellPosition(0, 2);
			ActiveSheet.Selection.SetSelection(activeCellPosition);
			viewModel.Location = PivotTableCommandBase.CreateReferenceString(ActiveSheet, new CellRange(ActiveSheet, activeCellPosition, activeCellPosition));
			viewModel.LocationRange = CellRangeBase.TryParse(viewModel.Location, Control.Document.Model.DocumentModel.DataContext) as CellRange;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			bool additionalEnabled = !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive && GetEnabled();
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, additionalEnabled);
			ApplyActiveSheetProtection(state);
		}
		bool GetEnabled() {
			return TryGetPivotTable() == null ? true : false;
		}
	}
	#endregion
}
