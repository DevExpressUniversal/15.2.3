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
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ConditionalFormattingHighlightCellsRuleCommandGroup
	public class ConditionalFormattingHighlightCellsRuleCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingHighlightCellsRuleCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingHighlightCellsRuleCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingHighlightCellsRuleCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingHighlightCellsRuleCommandGroupDescription; } }
		public override string ImageName { get { return "ConditionalFormattingHighlightCellsRules"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingGreaterThanRuleCommand
	public class ConditionalFormattingGreaterThanRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommand<ConditionalFormattingHighlightCellsRuleViewModel> {
		public ConditionalFormattingGreaterThanRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingGreaterThanRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingGreaterThanRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingGreaterThanRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGreaterThan"; } }
		#endregion
		protected override ConditionalFormattingHighlightCellsRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingHighlightCellsRuleViewModel result = new ConditionalFormattingHighlightCellsRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingGreaterThanRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingGreaterThanRule_LabelHeaderText);
			result.Range = range;
			result.Command = this;
			result.Value = GetAverageValueText();
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
			return new ExpressionFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingExpressionCondition.GreaterThan, viewModel.Value, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingLessThanRuleCommand
	public class ConditionalFormattingLessThanRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommand<ConditionalFormattingHighlightCellsRuleViewModel> {
		public ConditionalFormattingLessThanRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingLessThanRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingLessThanRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingLessThanRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingLessThan"; } }
		#endregion
		protected override ConditionalFormattingHighlightCellsRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingHighlightCellsRuleViewModel result = new ConditionalFormattingHighlightCellsRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingLessThanRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingLessThanRule_LabelHeaderText);
			result.Range = range;
			result.Command = this;
			result.Value = GetAverageValueText();
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
			return new ExpressionFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingExpressionCondition.LessThan, viewModel.Value, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingEqualToRuleCommand
	public class ConditionalFormattingEqualToRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommand<ConditionalFormattingHighlightCellsRuleViewModel> {
		public ConditionalFormattingEqualToRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingEqualToRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingEqualToRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingEqualToRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingEqualTo"; } }
		#endregion
		protected override ConditionalFormattingHighlightCellsRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingHighlightCellsRuleViewModel result = new ConditionalFormattingHighlightCellsRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingEqualToRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingEqualToRule_LabelHeaderText);
			result.Range = range;
			result.Command = this;
			result.Value = GetAverageValueText();
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
			return new ExpressionFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingExpressionCondition.EqualTo, viewModel.Value, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingTextContainsRuleCommand
	public class ConditionalFormattingTextContainsRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommandBase<ConditionalFormattingTextRuleViewModel> {
		public ConditionalFormattingTextContainsRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingTextContainsRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTextContainsRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTextContainsRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingTextThatContains"; } }
		#endregion
		protected override ConditionalFormattingTextRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingTextRuleViewModel result = new ConditionalFormattingTextRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTextContainsRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTextContainsRule_LabelHeaderText);
			result.Range = range;
			result.Command = this;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingTextRuleViewModel viewModel) {
			return new TextFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingTextCondition.Contains, viewModel.Value, ErrorHandler);
		}
		protected override void ShowForm(ConditionalFormattingTextRuleViewModel viewModel) {
			Control.ShowConditionalFormattingTextRuleForm(viewModel);
		}
	}
	#endregion
	#region ConditionalFormattingDuplicateValuesRuleCommand
	public class ConditionalFormattingDuplicateValuesRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommandBase<ConditionalFormattingDuplicateValuesRuleViewModel> {
		public ConditionalFormattingDuplicateValuesRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDuplicateValuesRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDuplicateValuesRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDuplicateValuesRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingDuplicateValues"; } }
		#endregion
		protected override ConditionalFormattingDuplicateValuesRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingDuplicateValuesRuleViewModel result = new ConditionalFormattingDuplicateValuesRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDuplicateValuesRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDuplicateValuesRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDuplicateValuesRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			result.Condition = ConditionalFormattingDuplicateValuesCondition.Duplicate;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingDuplicateValuesRuleViewModel viewModel) {
			ConditionalFormattingSpecialCondition condition = viewModel.Condition == ConditionalFormattingDuplicateValuesCondition.Duplicate ? ConditionalFormattingSpecialCondition.ContainDuplicateValue : ConditionalFormattingSpecialCondition.ContainUniqueValue;
			return new SpecialFormulaConditionalFormatting(ActiveSheet, viewModel.Range, condition);
		}
		protected override void ShowForm(ConditionalFormattingDuplicateValuesRuleViewModel viewModel) {
			Control.ShowConditionalFormattingDuplicateValuesRuleForm(viewModel);
		}
	}
	#endregion
	#region ConditionalFormattingDateOccurringRuleCommand
	public class ConditionalFormattingDateOccurringRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommandBase<ConditionalFormattingDateOccurringRuleViewModel> {
		public ConditionalFormattingDateOccurringRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDateOccurringRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDateOccurringRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDateOccurringRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingADateOccurring"; } }
		#endregion
		protected override ConditionalFormattingDateOccurringRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingDateOccurringRuleViewModel result = new ConditionalFormattingDateOccurringRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDateOccurringRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDateOccurringRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDateOccurringRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			result.Condition = ConditionalFormattingDateOccurringRuleViewModel.TimePeriodToString(ConditionalFormattingTimePeriod.Yesterday);
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingDateOccurringRuleViewModel viewModel) {
			ConditionalFormattingTimePeriod condition = ConditionalFormattingDateOccurringRuleViewModel.StringToTimePeriod(viewModel.Condition);
			return new TimePeriodFormulaConditionalFormatting(ActiveSheet, viewModel.Range, condition);
		}
		protected override void ShowForm(ConditionalFormattingDateOccurringRuleViewModel viewModel) {
			Control.ShowConditionalFormattingDateOccurringRuleForm(viewModel);
		}
	}
	#endregion
	#region ConditionalFormattingBetweenRuleCommand
	public class ConditionalFormattingBetweenRuleCommand : ApplyConditionalFormattingHighlightCellsRuleCommandBase<ConditionalFormattingBetweenRuleViewModel> {
		public ConditionalFormattingBetweenRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingBetweenRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBetweenRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBetweenRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingBetween"; } }
		#endregion
		protected override ConditionalFormattingBetweenRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingBetweenRuleViewModel result = new ConditionalFormattingBetweenRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBetweenRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBetweenRule_LabelHeaderText);
			result.Range = range;
			result.Command = this;
			MinMaxText minMax = CalculateMinMaxText();
			result.LowValue = minMax.Min;
			result.HighValue = minMax.Max;
			return result;
		}
		struct MinMaxText {
			public string Min { get; set; }
			public string Max { get; set; }
		}
		MinMaxText CalculateMinMaxText() {
			MinMaxValue minMax = CalculateMinMaxValue();
			int quarter = (int)Math.Round((minMax.Max - minMax.Min) / 4);
			MinMaxText result = new MinMaxText();
			if (minMax.Min == Double.MaxValue && minMax.Max == Double.MinValue) {
				result.Min = String.Empty;
				result.Max = String.Empty;
			}
			else {
				result.Min = Math.Round(minMax.Min + quarter).ToString(DocumentModel.DataContext.Culture);
				result.Max = Math.Round(minMax.Max - quarter).ToString(DocumentModel.DataContext.Culture);
			}
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingBetweenRuleViewModel viewModel) {
			return new RangeFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingRangeCondition.Inside, viewModel.LowValue, viewModel.HighValue, ErrorHandler);
		}
		protected override void ShowForm(ConditionalFormattingBetweenRuleViewModel viewModel) {
			Control.ShowConditionalFormattingBetweenRuleForm(viewModel);
		}
	}
	#endregion
}
