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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChangeChartTypeCommand
	public class ChangeChartTypeCommand : ModifyChartCommandBase {
		InsertChartCommandBase command;
		public ChangeChartTypeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartChangeType; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChangeChartType; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChangeChartTypeDescription; } }
		public override string ImageName { get { return "ChartGroupColumn"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<SpreadsheetCommandId> valueBasedState = state as IValueBasedCommandUIState<SpreadsheetCommandId>;
			if (state == null)
				return;
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (InnerControl.AllowShowingForms) {
					ChangeChartTypeViewModel viewModel = new ChangeChartTypeViewModel(Control);
					viewModel.CommandId = SpreadsheetCommandId.InsertChartColumnClustered2D;
					Control.ShowChangeChartTypeForm(viewModel);
				}
				else
					ChangeChartType(valueBasedState.Value);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public void ChangeChartType(SpreadsheetCommandId commandId) {
			DocumentModel.BeginUpdateFromUI();
			try {
				this.command = Control.CreateCommand(commandId) as InsertChartCommandBase;
				if (command == null)
					return;
				ExecuteCore();
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<SpreadsheetCommandId>();
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
		protected override bool IsChecked(Chart chart) {
			return false;
		}
		protected override void ModifyChart(Chart chart) {
			if (command == null)
				return;
			command.ChangeChartType(chart);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ChangeChartTypeContextMenuItemCommand
	public class ChangeChartTypeContextMenuItemCommand : ChangeChartTypeCommand {
		public ChangeChartTypeContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartChangeTypeContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChangeChartTypeContextMenuItem; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = state.Enabled && ActiveSheet.Selection.SelectedDrawingIndexes.Count == 1;
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
