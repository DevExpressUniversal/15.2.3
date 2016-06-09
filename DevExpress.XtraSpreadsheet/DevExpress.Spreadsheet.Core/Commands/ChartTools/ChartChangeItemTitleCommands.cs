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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ChartChangeItemTitleContextMenuItemCommandBase (abstract class)
	public abstract class ChartChangeItemTitleContextMenuItemCommandBase : SpreadsheetCommand {
		protected ChartChangeItemTitleContextMenuItemCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (state == null)
				return;
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (InnerControl.AllowShowingForms)
					ShowForm(GetTitle());
				else
					ChangeItemTitle(valueBasedState.Value);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		string GetTitle() {
			Chart chart = GetChart();
			TitleOptions title = GetTitle(chart);
			return title.Text.PlainText;
		}
		protected internal void ChangeItemTitle(string title) {
			DocumentModel.BeginUpdateFromUI();
			try {
				Chart chart = GetChart();
				ModifyTitle(chart, title);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal void ModifyTitle(Chart chart, string title) {
			TitleOptions titleOptions = GetTitle(chart);
			if (String.IsNullOrEmpty(title))
				titleOptions.Text = ChartText.Empty;
			else {
				ChartRichText text = new ChartRichText(chart);
				text.PlainText = title;
				titleOptions.Text = text;
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<string>();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Charts, !InnerControl.IsInplaceEditorActive);
			state.Enabled = state.Enabled && HasTitle();
			state.Visible = state.Enabled;
		}
		bool HasTitle() {
			Chart chart = GetChart();
			if (chart == null)
				return false;
			TitleOptions title = GetTitle(chart);
			if (title == null)
				return false;
			return title.Visible;
		}
		Chart GetChart() {
			List<int> drawingIndexes = ActiveSheet.Selection.SelectedDrawingIndexes;
			if (drawingIndexes.Count != 1)
				return null;
			int index = drawingIndexes[0];
			return ActiveSheet.DrawingObjects[index] as Chart;
		}
		protected internal abstract void ShowForm(string title);
		protected internal abstract TitleOptions GetTitle(Chart chart);
	}
	#endregion
	#region ChartChangeTitleContextMenuItemCommand
	public class ChartChangeTitleContextMenuItemCommand : ChartChangeItemTitleContextMenuItemCommandBase {
		public ChartChangeTitleContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartChangeTitleContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartChangeTitleContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartChangeTitleContextMenuItemDescription; } }
		#endregion
		protected internal override TitleOptions GetTitle(Chart chart) {
			return chart.Title;
		}
		protected internal override void ShowForm(string title) {
			ChangeChartTitleViewModel viewModel = new ChangeChartTitleViewModel();
			viewModel.Command = this;
			viewModel.Title = title;
			Control.ShowChangeChartTitleForm(viewModel);
		}
	}
	#endregion
	#region ChartChangeAxisTitleContextMenuItemCommandBase (abstract class)
	public abstract class ChartChangeAxisTitleContextMenuItemCommandBase : ChartChangeItemTitleContextMenuItemCommandBase {
		protected ChartChangeAxisTitleContextMenuItemCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected internal abstract bool IsHorizontalAxis { get; }
		#endregion
		protected internal override TitleOptions GetTitle(Chart chart) {
			AxisBase axis = ChartModifyPrimaryAxisCommandBase.GetAxis(chart, IsHorizontalAxis);
			if (axis == null)
				return null;
			return axis.Title;
		}
	}
	#endregion
	#region ChartChangeHorizontalAxisTitleContextMenuItemCommand
	public class ChartChangeHorizontalAxisTitleContextMenuItemCommand : ChartChangeAxisTitleContextMenuItemCommandBase {
		public ChartChangeHorizontalAxisTitleContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartChangeHorizontalAxisTitleContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartChangeHorizontalAxisTitleContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartChangeHorizontalAxisTitleContextMenuItemDescription; } }
		protected internal override bool IsHorizontalAxis { get { return true; } }
		#endregion
		protected internal override void ShowForm(string title) {
			ChangeChartHorizontalAxisTitleViewModel viewModel = new ChangeChartHorizontalAxisTitleViewModel();
			viewModel.Command = this;
			viewModel.Title = title;
			Control.ShowChangeChartHorizontalAxisTitleForm(viewModel);
		}
	}
	#endregion
	#region ChartChangeVerticalAxisTitleContextMenuItemCommand
	public class ChartChangeVerticalAxisTitleContextMenuItemCommand : ChartChangeAxisTitleContextMenuItemCommandBase {
		public ChartChangeVerticalAxisTitleContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartChangeVerticalAxisTitleContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartChangeVerticalAxisTitleContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartChangeVerticalAxisTitleContextMenuItemDescription; } }
		protected internal override bool IsHorizontalAxis { get { return false; } }
		#endregion
		protected internal override void ShowForm(string title) {
			ChangeChartVerticalAxisTitleViewModel viewModel = new ChangeChartVerticalAxisTitleViewModel();
			viewModel.Command = this;
			viewModel.Title = title;
			Control.ShowChangeChartVerticalAxisTitleForm(viewModel);
		}
	}
	#endregion
}
