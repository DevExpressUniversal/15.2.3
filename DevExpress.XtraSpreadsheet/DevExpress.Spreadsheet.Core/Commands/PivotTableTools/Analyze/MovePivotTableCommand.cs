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

using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region MovePivotTableCommand
	public class MovePivotTableCommand : PivotTableCommandBase {
		public MovePivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotTableDescription; } }
		public override string ImageName { get { return "MovePivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			if (InnerControl.AllowShowingForms)
				Control.ShowMovePivotTableForm(CreateViewModel(table));
		}
		MovePivotTableViewModel CreateViewModel(PivotTable table) {
			MovePivotTableViewModel viewModel = new MovePivotTableViewModel(Control);
			CellPosition topLeftLocation = table.Location.Range.TopLeft;
			viewModel.PivotTable = table;
			viewModel.Source = null;
			viewModel.NewWorksheet = false;
			viewModel.Location = PivotTableCommandBase.CreateReferenceString(ActiveSheet, new CellRange(ActiveSheet, topLeftLocation, topLeftLocation));
			return viewModel;
		}
		protected internal bool Validate(MovePivotTableViewModel viewModel) {
			if (!ValidateReference(viewModel))
				return false;
			return true;
		}
		bool ValidateReference(MovePivotTableViewModel viewModel) {
			WorkbookDataContext dataContext = Control.Document.Model.DocumentModel.DataContext;
			viewModel.LocationRange = CellRangeBase.TryParse(viewModel.Location, dataContext) as CellRange;
			if (viewModel.LocationRange == null)
				if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableDestinationReferenceNotValid)))
					return false;
			return true;
		}
		protected internal void ApplyChanges(MovePivotTableViewModel viewModel) {
			DocumentModel.BeginUpdate();
			try {
				if (viewModel.NewWorksheet) {
					InsertSheetCommandCreate().ExecuteCore();
					GetPivotTableLocationFromNewSheet(viewModel);
				}
				else
					ActiveSheet.Selection.SetSelection(viewModel.LocationRange.TopLeft);
				MovePivotCommandCreate(viewModel).Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		InsertSheetCommand InsertSheetCommandCreate() {
			return new InsertSheetCommand(Control);
		}
		MovePivotCommand MovePivotCommandCreate(MovePivotTableViewModel viewModel) {
			return new MovePivotCommand(viewModel.PivotTable, viewModel.LocationRange, ErrorHandler);
		}
		void GetPivotTableLocationFromNewSheet(InsertPivotTableViewModel viewModel) {
			CellPosition activeCellPosition = new CellPosition(0, 2);
			ActiveSheet.Selection.SetSelection(activeCellPosition);
			viewModel.Location = PivotTableCommandBase.CreateReferenceString(ActiveSheet, new CellRange(ActiveSheet, activeCellPosition, activeCellPosition));
			viewModel.LocationRange = CellRangeBase.TryParse(viewModel.Location, Control.Document.Model.DocumentModel.DataContext) as CellRange;
		}
	}
	#endregion
}
