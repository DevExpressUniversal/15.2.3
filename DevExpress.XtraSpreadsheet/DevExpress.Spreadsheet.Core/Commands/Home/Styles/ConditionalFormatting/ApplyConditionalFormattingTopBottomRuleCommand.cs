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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	public interface IConditionalFormattingCommand {
		bool ApplyChanges(ConditionalFormattingViewModelBase viewModel);
	}
	#region ApplyFormulaConditionalFormattingCommand (abstract class)
	public abstract class ApplyFormulaConditionalFormattingCommand<TViewModel> : SpreadsheetCommand, IConditionalFormattingCommand
		where TViewModel : ConditionalFormattingViewModelBase {
		protected ApplyFormulaConditionalFormattingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				List<CellRangeBase> ranges = new List<CellRangeBase>();
				foreach (CellRangeBase range in ActiveSheet.Selection.SelectedRanges)
					ranges.Add(range);
				CellRangeBase modelRange = new CellUnion(ranges);
				if (InnerControl.AllowShowingForms)
					ShowForm(CreateViewModel(modelRange));
				else
					ApplyChanges(CreateViewModel(modelRange));
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
			state.Visible = true;
			state.Checked = false;
		}
		public bool ApplyChanges(ConditionalFormattingViewModelBase viewModel) {
			DocumentModel.BeginUpdateFromUI();
			try {
				FormulaConditionalFormatting result = CreateConditionalFormatting((TViewModel)viewModel);
				ConditionalFormattingStyle style = viewModel.Style;
				if (style != null) {
					if (result.Font.Color != style.FontColor)
						result.Font.Color = style.FontColor;
					if (result.Fill.BackColor != style.FillBackColor) {
						result.Fill.BackColor = style.FillBackColor;
						result.Fill.PatternType = style.FillPatternType;
					}
					if (result.Fill.ForeColor != style.FillForeColor) {
						result.Fill.ForeColor = style.FillForeColor;
						result.Fill.PatternType = style.FillPatternType;
					}
					if (result.Border.TopColor != style.BorderColor) {
						result.Border.TopColor = style.BorderColor;
						result.Border.TopLineStyle = style.BorderLineStyle;
					}
					if (result.Border.BottomColor != style.BorderColor) {
						result.Border.BottomColor = style.BorderColor;
						result.Border.BottomLineStyle = style.BorderLineStyle;
					}
					if (result.Border.LeftColor != style.BorderColor) {
						result.Border.LeftColor = style.BorderColor;
						result.Border.LeftLineStyle = style.BorderLineStyle;
					}
					if (result.Border.RightColor != style.BorderColor) {
						result.Border.RightColor = style.BorderColor;
						result.Border.RightLineStyle = style.BorderLineStyle;
					}
				}
				ActiveSheet.InsertConditionalFormattingWithHistoryAndNotification(result);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
			return true;
		}
		protected abstract FormulaConditionalFormatting CreateConditionalFormatting(TViewModel viewModel);
		protected abstract TViewModel CreateViewModel(CellRangeBase range);
		protected abstract void ShowForm(TViewModel viewModel);
	}
	#endregion
	#region ApplyConditionalFormattingTopBottomRuleCommand (abstract class)
	public abstract class ApplyConditionalFormattingTopBottomRuleCommand : ApplyFormulaConditionalFormattingCommand<ConditionalFormattingTopBottomRuleViewModel> {
		protected ApplyConditionalFormattingTopBottomRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void ShowForm(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			Control.ShowConditionalFormattingTop10RuleForm(viewModel);
		}
	}
	#endregion
	#region ApplyConditionalFormattingAverageRuleCommand (abstract class)
	public abstract class ApplyConditionalFormattingAverageRuleCommand : ApplyFormulaConditionalFormattingCommand<ConditionalFormattingAverageRuleViewModel> {
		protected ApplyConditionalFormattingAverageRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void ShowForm(ConditionalFormattingAverageRuleViewModel viewModel) {
			Control.ShowConditionalFormattingAverageRuleForm(viewModel);
		}
	}
	#endregion
	#region ApplyConditionalFormattingHighlightCellsRuleCommandBase (abstract class)
	public abstract class ApplyConditionalFormattingHighlightCellsRuleCommandBase<TViewModel> : ApplyFormulaConditionalFormattingCommand<TViewModel>
		where TViewModel : ConditionalFormattingViewModelBase {
		protected ApplyConditionalFormattingHighlightCellsRuleCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected struct MinMaxValue {
			public double Min { get; set; }
			public double Max { get; set; }
		}
		protected MinMaxValue CalculateMinMaxValue() {
			IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
			double min = Double.MaxValue;
			double max = Double.MinValue;
			for (int i = 0; i < selectedRanges.Count; i++) {
				foreach (VariantValue value in new Enumerable<VariantValue>(selectedRanges[i].GetExistingValuesEnumerator())) {
					if (!value.IsNumeric)
						continue;
					min = Math.Min(min, Math.Floor(value.NumericValue));
					max = Math.Max(max, Math.Ceiling(value.NumericValue));
				}
			}
			MinMaxValue result = new MinMaxValue();
			result.Min = min;
			result.Max = max;
			return result;
		}
		protected string GetAverageValueText() {
			return GetAverageValue().ToString(DocumentModel.DataContext.Culture);
		}
		protected double GetAverageValue() {
			MinMaxValue minMax = CalculateMinMaxValue();
			return minMax.Max != Double.MinValue && minMax.Min != Double.MaxValue ? Math.Round((minMax.Min + minMax.Max) / 2, 2) : 0;
		}
	}
	#endregion
	#region ApplyConditionalFormattingHighlightCellsRuleCommand (abstract class)
	public abstract class ApplyConditionalFormattingHighlightCellsRuleCommand<TViewModel> : ApplyConditionalFormattingHighlightCellsRuleCommandBase<TViewModel>
		where TViewModel : ConditionalFormattingHighlightCellsRuleViewModel {
		protected ApplyConditionalFormattingHighlightCellsRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void ShowForm(TViewModel viewModel) {
			Control.ShowConditionalFormattingExpressionRuleForm(viewModel);
		}
	}
	#endregion
}
