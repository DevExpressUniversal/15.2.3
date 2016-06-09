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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ConditionalFormattingTopBottomRuleCommandGroup
	public class ConditionalFormattingTopBottomRuleCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingTopBottomRuleCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingTopBottomRuleCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTopBottomRuleCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTopBottomRuleCommandGroupDescription; } }
		public override string ImageName { get { return "ConditionalFormattingTopBottomRules"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingTop10RuleCommand
	public class ConditionalFormattingTop10RuleCommand : ApplyConditionalFormattingTopBottomRuleCommand {
		public ConditionalFormattingTop10RuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingTop10RuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10RuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10RuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingTop10Items"; } }
		#endregion
		protected override ConditionalFormattingTopBottomRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingTopBottomRuleViewModel result = new ConditionalFormattingTopBottomRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTop10Rule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopBottomRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			result.Rank = RankFormulaConditionalFormatting.DefaultRankValue;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			return new RankFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingRankCondition.TopByRank, viewModel.Rank, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingBottom10RuleCommand
	public class ConditionalFormattingBottom10RuleCommand : ApplyConditionalFormattingTopBottomRuleCommand {
		public ConditionalFormattingBottom10RuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingBottom10RuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10RuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10RuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingBottom10Items"; } }
		#endregion
		protected override ConditionalFormattingTopBottomRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingTopBottomRuleViewModel result = new ConditionalFormattingTopBottomRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottom10Rule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottomRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopBottomRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			result.Rank = RankFormulaConditionalFormatting.DefaultRankValue;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			return new RankFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingRankCondition.BottomByRank, viewModel.Rank, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingTop10PercentRuleCommand
	public class ConditionalFormattingTop10PercentRuleCommand : ApplyConditionalFormattingTopBottomRuleCommand {
		public ConditionalFormattingTop10PercentRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingTop10PercentRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10PercentRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10PercentRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingTop10Percent"; } }
		#endregion
		protected override ConditionalFormattingTopBottomRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingTopBottomRuleViewModel result = new ConditionalFormattingTopBottomRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTop10PercentRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopBottomPercentRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			result.Rank = RankFormulaConditionalFormatting.DefaultRankValue;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			return new RankFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingRankCondition.TopByPercent, viewModel.Rank, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingBottom10PercentRuleCommand
	public class ConditionalFormattingBottom10PercentRuleCommand : ApplyConditionalFormattingTopBottomRuleCommand {
		public ConditionalFormattingBottom10PercentRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingBottom10PercentRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10PercentRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10PercentRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingBottom10Percent"; } }
		#endregion
		protected override ConditionalFormattingTopBottomRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingTopBottomRuleViewModel result = new ConditionalFormattingTopBottomRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottom10PercentRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottomRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopBottomPercentRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			result.Rank = RankFormulaConditionalFormatting.DefaultRankValue;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			return new RankFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingRankCondition.BottomByPercent, viewModel.Rank, ErrorHandler);
		}
	}
	#endregion
	#region ConditionalFormattingAboveAverageRuleCommand
	public class ConditionalFormattingAboveAverageRuleCommand : ApplyConditionalFormattingAverageRuleCommand {
		public ConditionalFormattingAboveAverageRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingAboveAverageRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingAboveAverageRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingAboveAverageRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingAboveAverage"; } }
		#endregion
		protected override ConditionalFormattingAverageRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingAverageRuleViewModel result = new ConditionalFormattingAverageRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingAboveAverageRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingAboveAverageRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingAboveAverageRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingAverageRuleViewModel viewModel) {
			return new AverageFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingAverageCondition.Above, 0);
		}
	}
	#endregion
	#region ConditionalFormattingBelowAverageRuleCommand
	public class ConditionalFormattingBelowAverageRuleCommand : ApplyConditionalFormattingAverageRuleCommand {
		public ConditionalFormattingBelowAverageRuleCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingBelowAverageRuleCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBelowAverageRuleCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBelowAverageRuleCommandDescription; } }
		public override string ImageName { get { return "ConditionalFormattingBelowAverage"; } }
		#endregion
		protected override ConditionalFormattingAverageRuleViewModel CreateViewModel(CellRangeBase range) {
			ConditionalFormattingAverageRuleViewModel result = new ConditionalFormattingAverageRuleViewModel(Control);
			result.FormText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBelowAverageRule_FormText);
			result.LabelHeaderText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBelowAverageRule_LabelHeaderText);
			result.LabelWithText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBelowAverageRule_LabelWithText);
			result.Range = range;
			result.Command = this;
			return result;
		}
		protected override FormulaConditionalFormatting CreateConditionalFormatting(ConditionalFormattingAverageRuleViewModel viewModel) {
			return new AverageFormulaConditionalFormatting(ActiveSheet, viewModel.Range, ConditionalFormattingAverageCondition.Below, 0);
		}
	}
	#endregion
}
