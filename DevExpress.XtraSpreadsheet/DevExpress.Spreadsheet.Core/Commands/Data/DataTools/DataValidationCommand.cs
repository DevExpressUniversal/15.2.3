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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Office.Services.Implementation;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region DataValidationCommand
	public class DataValidationCommand : SpreadsheetCommand {
		char invariantListSeparator = CultureInfo.InvariantCulture.TextInfo.ListSeparator[0];
		public DataValidationCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataToolsDataValidation; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataValidation; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataValidationDescription; } }
		public override string ImageName { get { return "DataValidation"; } }
		DataValidationCollection DataValidations { get { return ActiveSheet.DataValidations; } }
		SheetViewSelection Selection { get { return ActiveSheet.Selection; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<DataValidationViewModel> valueBasedState = state as IValueBasedCommandUIState<DataValidationViewModel>;
				if (valueBasedState == null || valueBasedState.Value == null)
					return;
				if (ActiveSheet.PivotTables.ContainsItemsInRange(Selection.AsRange(), true)) {
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PivotTableCanNotApplyDataValidation));
					return;
				}
				if (InnerControl.AllowShowingForms)
					Control.ShowDataValidationForm(valueBasedState.Value);
				else
					ApplyChanges(valueBasedState.Value);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public DataValidationCommandParameters CalculateCommandParameters() {
			CellRangeBase activeRange = Selection.AsRange();
			CellRangeBase remainingRange = activeRange.Clone();
			int intersectionsCount = 0;
			int intersectedItemIndex = 0;
			int count = DataValidations.Count;
			for (int i = 0; i < count; i++) {
				CellRangeBase dataValidationRange = DataValidations[i].CellRange;
				if (dataValidationRange.Intersects(activeRange)) {
					remainingRange = remainingRange.ExcludeRange(dataValidationRange);
					intersectedItemIndex = i;
					intersectionsCount++;
				}
				if (intersectionsCount > 1)
					break;
			}
			DataValidationCommandParameters result = new DataValidationCommandParameters();
			result.IntersectionCount = intersectionsCount;
			result.ActiveRange = activeRange;
			if (intersectionsCount == 1) {
				result.RemainingRange = remainingRange;
				result.ActiveDataValidation = DataValidations[intersectedItemIndex];
			}
			return result;
		}
		internal DataValidationViewModel PrepareViewModel() {
			DataValidationCommandParameters parameters = CalculateCommandParameters();
			int intersectionCount = parameters.IntersectionCount;
			CellRangeBase activeRange = parameters.ActiveRange;
			CellRangeBase remainingRange = parameters.RemainingRange;
			if (intersectionCount == 0 || (intersectionCount > 1 && ShouldChangeIntersected()))
				return CreateDefaultViewModel(activeRange);
			else if (intersectionCount == 1)
				return OneIntersectionCase(activeRange, remainingRange, parameters.ActiveDataValidation);
			return null;
		}
		bool ShouldChangeIntersected() {
			return Control.ShowOkCancelMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_SelectionContainsMoreThanOneDataValidation));
		}
		DataValidationViewModel OneIntersectionCase(CellRangeBase activeRanges, CellRangeBase remainingRanges, DataValidation intersectedDataValidation) {
			if (remainingRanges == null)
				return CreateViewModel(intersectedDataValidation, activeRanges);
			DialogResult result = Control.ShowYesNoCancelMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_SelectionContainsCellsWithoutDataValidation));
			if (result == DialogResult.Yes)
				return CreateViewModel(intersectedDataValidation, activeRanges);
			if (result == DialogResult.No)
				return CreateDefaultViewModel(activeRanges);
			return null;
		}
		public void ApplyChanges(DataValidationViewModel viewModel) {
			Worksheet sheet = viewModel.Worksheet;
			DocumentModel.BeginUpdate();
			try {
				DataValidationCommandBase command;
				UIErrorHandler errorHandler = Control.InnerControl.ErrorHandler;
				if (viewModel.IsDefault)
					command = new DataValidationClearCommand(sheet, errorHandler, sheet.Selection.AsRange());
				else {
					DataValidation dataValidation = CreateDataValidation(viewModel);
					command = new DataValidationUIAddCommand(sheet, errorHandler, dataValidation);
					UpdateInvalidDataCircles(dataValidation);
				}
				command.Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#region UpdateInvalidDataCircles
		void UpdateInvalidDataCircles(DataValidation dataValidation) {
			if (dataValidation.Sheet.ShowInvalidDataCircles) {
				if (!dataValidation.UpdateInvalidDataCircles())
					InvokeShowWarningMessage();
				DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw);
			}
		}
		void InvokeShowWarningMessage() {
			IThreadSyncService service = Control.InnerControl.GetService<IThreadSyncService>();
			if (service != null)
				service.EnqueueInvokeInUIThread(new Action(delegate() { ShowMessage(); }));
		}
		void ShowMessage() {
			Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_MoreThan255InvalidDataCircles));
		}
		#endregion
		#region Create methods
		internal DataValidationViewModel CreateDefaultViewModel(CellRangeBase activeRanges) {
			DataValidationViewModel result = new DataValidationViewModel(Control);
			result.ActiveCell = Selection.ActiveCell;
			result.ActiveRange = activeRanges;
			return result;
		}
		internal DataValidationViewModel CreateViewModel(DataValidation dataValidation, CellRangeBase activeRanges) {
			DataValidationViewModel result = CreateDefaultViewModel(activeRanges);
			CellPosition activeCell = result.ActiveCell;
			result.DataValidationRange = GetDataValidationRange(dataValidation.CellRange, activeCell);
			result.Type = dataValidation.Type;
			result.Operator = dataValidation.ValidationOperator;
			result.IgnoreBlank = dataValidation.AllowBlank;
			result.InCellDropDown = !dataValidation.SuppressDropDown;
			if (dataValidation.Type == DataValidationType.List) {
				string formula = dataValidation.GetFormula1(activeCell);
				if (!formula.StartsWith("=", StringComparison.Ordinal))
					formula = formula.Replace(invariantListSeparator, DocumentModel.DataContext.GetListSeparator());
				result.Formula1 = formula;
				result.Formula2 = string.Empty;
			}
			else {
				result.Formula1 = dataValidation.GetFormula1(activeCell);
				result.Formula2 = dataValidation.GetFormula2(activeCell);
			}
			result.ShowMessage = dataValidation.ShowInputMessage;
			result.MessageTitle = UpdateTextLineBreaks(dataValidation.PromptTitle);
			result.Message = UpdateTextLineBreaks(dataValidation.Prompt);
			result.ShowErrorMessage = dataValidation.ShowErrorMessage;
			result.ErrorTitle = UpdateTextLineBreaks(dataValidation.ErrorTitle);
			result.ErrorMessage = UpdateTextLineBreaks(dataValidation.Error);
			result.ErrorStyle = dataValidation.ErrorStyle;
			return result;
		}
		string UpdateTextLineBreaks(string text) {
			return Regex.Replace(text, "(?<!\r)\n", Environment.NewLine);
		}
		CellRangeBase GetDataValidationRange(CellRangeBase dataValidationRange, CellPosition activeCell) {
			CellRange activeCellRange = new CellRange(ActiveSheet, activeCell, activeCell);
			return dataValidationRange.Includes(activeCellRange) ? dataValidationRange : activeCellRange;
		}
		DataValidation CreateDataValidation(DataValidationViewModel viewModel) {
			DataValidation result = new DataValidation(viewModel.Worksheet.Selection.AsRange(), viewModel.Worksheet);
			result.BeginUpdate();
			try {
				result.Type = viewModel.Type;
				result.ValidationOperator = viewModel.Operator;
				if (viewModel.Type == DataValidationType.List) {
					string formula = viewModel.Formula1;
					if (!formula.StartsWith("=", StringComparison.Ordinal))
						formula = formula.Replace(viewModel.Worksheet.DataContext.GetListSeparator(), invariantListSeparator);
					result.Formula1 = formula;
				}
				else {
					if (viewModel.Formula1Visible)
						result.Formula1 = viewModel.Formula1;
					if (viewModel.Formula2Visible)
						result.Formula2 = viewModel.Formula2;
				}
				result.SuppressDropDown = !viewModel.InCellDropDown;
				result.AllowBlank = viewModel.IgnoreBlank;
				result.ShowInputMessage = viewModel.ShowMessage;
				result.PromptTitle = viewModel.MessageTitle;
				result.Prompt = viewModel.Message;
				result.ShowErrorMessage = viewModel.ShowErrorMessage;
				result.ErrorTitle = viewModel.ErrorTitle;
				result.ErrorStyle = viewModel.ErrorStyle;
				result.Error = viewModel.ErrorMessage;
			}
			finally {
				result.EndUpdate();
			}
			return result;
		}
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			IValueBasedCommandUIState<DataValidationViewModel> state = new DefaultValueBasedCommandUIState<DataValidationViewModel>();
			if (InnerControl.AllowShowingForms)
				state.Value = PrepareViewModel();
			return state;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, IsEnabled());
			ApplyActiveSheetProtection(state);
		}
		bool IsEnabled() {
			return !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly && !Selection.IsDrawingSelected && !Selection.IsChartSelected;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region MoveOrCopySheetCommandParameters
	public struct DataValidationCommandParameters {
		#region Properties
		public int IntersectionCount { get; set; }
		public CellRangeBase ActiveRange { get; set; }
		public CellRangeBase RemainingRange { get; set; }
		public DataValidation ActiveDataValidation { get; set; }
		#endregion
	}
	#endregion
}
