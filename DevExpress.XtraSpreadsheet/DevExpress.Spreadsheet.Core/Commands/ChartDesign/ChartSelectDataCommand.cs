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
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartSelectDataCommand
	public class ChartSelectDataCommand : SpreadsheetCommand {
		public ChartSelectDataCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartSelectData; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartSelectData; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartSelectDataDescription; } }
		public override string ImageName { get { return "ChartSelectData"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<IChartSelectDataInfo> valueBasedState = state as IValueBasedCommandUIState<IChartSelectDataInfo>;
			if (state == null)
				return;
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (InnerControl.AllowShowingForms) {
					ChartSelectDataViewModel viewModel = new ChartSelectDataViewModel(Control);
					viewModel.Chart = ActiveSheet.Selection.SelectedChart;
					if (viewModel.Chart == null)
						return;
					viewModel.Reference = GetReference(viewModel.Chart);
					viewModel.Sheet = ActiveSheet;
					Control.ShowChartSelectDataForm(viewModel);
				}
				else {
					ModifyChartRangesCommand command = CreateModelCommand(ActiveSheet, valueBasedState.Value.Chart, valueBasedState.Value.Reference);
					command.Execute();
				}
			} finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal bool ApplyViewModelParameters(ChartSelectDataViewModel viewModel) {
			ModifyChartRangesCommand command = CreateModelCommand(viewModel.Sheet, viewModel.Chart, viewModel.Reference);
			if (!command.Validate())
				return false;
			DocumentModel.BeginUpdateFromUI();
			try {
				command.Execute();
				return true;
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		ModifyChartRangesCommand CreateModelCommand(Worksheet sheet, Chart chart, string reference) {
			ModifyChartRangesCommand result = new ModifyChartRangesCommand(sheet, Control.InnerControl.ErrorHandler);
			result.Chart = chart;
			result.Reference = reference;
			return result;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Charts, !InnerControl.IsInplaceEditorActive);
			Chart selectedChart = ActiveSheet.Selection.SelectedChart;
			if (selectedChart == null) {
				state.Enabled = false;
				return;
			}
			state.Visible = true;
			state.Checked = false;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			ChartSelectDataInfo info = CreateDefaultChartSelectDataInfo();
			if (info == null)
				return new DefaultCommandUIState();
			DefaultValueBasedCommandUIState<IChartSelectDataInfo> result = new DefaultValueBasedCommandUIState<IChartSelectDataInfo>();
			result.Value = info;
			return result;
		}
		ChartSelectDataInfo CreateDefaultChartSelectDataInfo() {
			Chart selectedChart = ActiveSheet.Selection.SelectedChart;
			if (selectedChart == null)
				return null;
			string reference = GetReference(selectedChart);
			return new ChartSelectDataInfo(selectedChart, reference);
		}
		string GetReference(Chart selectedChart) {
			ChartReferencedRanges dataReferencedRanges = selectedChart.GetReferencedRanges();
			CellRangeBase dataRange = dataReferencedRanges.GetDataRange();
			if (dataRange == null)
				return string.Empty;
			dataRange = dataRange.GetWithModifiedPositionType(PositionType.Absolute);
			ParsedExpression rangeExpression = new ParsedExpression();
			WorkbookDataContext context = DocumentModel.DataContext;
			BasicExpressionCreator.CreateCellRangeExpression(rangeExpression, dataRange, BasicExpressionCreatorParameter.ShouldCreate3d, OperandDataType.Default, context);
			return "=" + rangeExpression.BuildExpressionString(context);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ChartSelectDataContextMenuItemCommand
	public class ChartSelectDataContextMenuItemCommand : ChartSelectDataCommand {
		public ChartSelectDataContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartSelectDataContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartSelectDataContextMenuItem; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = state.Enabled && ActiveSheet.Selection.SelectedDrawingIndexes.Count == 1;
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
