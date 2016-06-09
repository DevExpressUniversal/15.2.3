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
using System.Linq;
using System.Text;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using System.Resources;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public class DataControlStringIdExtension : System.Windows.Markup.MarkupExtension {
		public ConditionalFormattingStringId StringId { get; set; }
		public DataControlStringIdExtension(ConditionalFormattingStringId stringId) {
			StringId = stringId;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return ConditionalFormattingLocalizer.GetString(StringId);
		}
	}
	public enum ConditionalFormattingStringId {
		#region menu
		MenuColumnConditionalFormatting,
		MenuColumnConditionalFormatting_HighlightCellsRules,
		MenuColumnConditionalFormatting_HighlightCellsRules_GreaterThan,
		MenuColumnConditionalFormatting_HighlightCellsRules_LessThan,
		MenuColumnConditionalFormatting_HighlightCellsRules_Between,
		MenuColumnConditionalFormatting_HighlightCellsRules_EqualTo,
		MenuColumnConditionalFormatting_HighlightCellsRules_TextThatContains,
		MenuColumnConditionalFormatting_HighlightCellsRules_ADateOccurring,
		MenuColumnConditionalFormatting_HighlightCellsRules_CustomCondition,
		MenuColumnConditionalFormatting_TopBottomRules,
		MenuColumnConditionalFormatting_TopBottomRules_Top10Items,
		MenuColumnConditionalFormatting_TopBottomRules_Bottom10Items,
		MenuColumnConditionalFormatting_TopBottomRules_Top10Percent,
		MenuColumnConditionalFormatting_TopBottomRules_Bottom10Percent,
		MenuColumnConditionalFormatting_TopBottomRules_AboveAverage,
		MenuColumnConditionalFormatting_TopBottomRules_BelowAverage,
		MenuColumnConditionalFormatting_DataBars,
		MenuColumnConditionalFormatting_ColorScales,
		MenuColumnConditionalFormatting_IconSets,
		MenuColumnConditionalFormatting_ClearRules,
		MenuColumnConditionalFormatting_ClearRules_FromAllColumns,
		MenuColumnConditionalFormatting_ClearRules_FromCurrentColumns,
		MenuColumnConditionalFormatting_ManageRules,
		#endregion
		#region predefined formats
		ConditionalFormatting_PredefinedFormat_LightRedFillWithDarkRedText,
		ConditionalFormatting_PredefinedFormat_YellowFillWithDarkYellowText,
		ConditionalFormatting_PredefinedFormat_GreenFillWithDarkGreenText,
		ConditionalFormatting_PredefinedFormat_LightRedFill,
		ConditionalFormatting_PredefinedFormat_LightGreenFill,
		ConditionalFormatting_PredefinedFormat_RedText,
		ConditionalFormatting_PredefinedFormat_GreenText,
		ConditionalFormatting_PredefinedFormat_BoldText,
		ConditionalFormatting_PredefinedFormat_ItalicText,
		ConditionalFormatting_PredefinedFormat_StrikethroughText,
		ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowRedColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_RedYellowGreenColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteRedColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteGreenColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_BlueWhiteRedColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteBlueColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_WhiteRedColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_WhiteGreenColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_YellowGreenColorScale,
		ConditionalFormatting_PredefinedColorScaleFormat_Description,
		ConditionalFormatting_PredefinedDataBarFormat_BlueSolidDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_GreenSolidDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_RedSolidDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_OrangeSolidDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_LightBlueSolidDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_PurpleSolidDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_BlueGradientDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_GreenGradientDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_RedGradientDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_OrangeGradientDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_LightBlueGradientDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_PurpleGradientDataBar,
		ConditionalFormatting_PredefinedDataBarFormat_Description,
		ConditionalFormatting_PredefinedDataBarFormat_SolidFillGroup,
		ConditionalFormatting_PredefinedDataBarFormat_GradientFillGroup,
		ConditionalFormatting_PredefinedIconSetFormat_Arrows3ColoredIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Arrows4ColoredIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Arrows5ColoredIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Arrows3GrayIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Arrows4GrayIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Arrows5GrayIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Boxes5IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Flags3IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Quarters5IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Ratings4IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Ratings5IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_RedToBlackIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Signs3IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Stars3IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Symbols3CircledIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Symbols3UncircledIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3UnrimmedIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3RimmedIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_TrafficLights4IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Triangles3IconSet,
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeTrianglesIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsGrayIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsColoredIconSet,
		ConditionalFormatting_PredefinedIconSetFormat_Description,
		ConditionalFormatting_PredefinedIconSetFormat_DirectionalGroup,
		ConditionalFormatting_PredefinedIconSetFormat_ShapesGroup,
		ConditionalFormatting_PredefinedIconSetFormat_IndicatorsGroup,
		ConditionalFormatting_PredefinedIconSetFormat_RatingsGroup,
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeGroup,
		#endregion
		#region dialogs
		ConditionalFormatting_GreaterThanDialog_Title,
		ConditionalFormatting_LessThanDialog_Title,
		ConditionalFormatting_BetweenDialog_Title,
		ConditionalFormatting_EqualToDialog_Title,
		ConditionalFormatting_TextThatContainsDialog_Title,
		ConditionalFormatting_DateOccurringDialog_Title,
		ConditionalFormatting_CustomConditionDialog_Title,
		ConditionalFormatting_Top10ItemsDialog_Title,
		ConditionalFormatting_Bottom10ItemsDialog_Title,
		ConditionalFormatting_Top10PercentDialog_Title,
		ConditionalFormatting_Bottom10PercentDialog_Title,
		ConditionalFormatting_AboveAverageDialog_Title,
		ConditionalFormatting_BelowAverageDialog_Title,
		ConditionalFormatting_GreaterThanDialog_Description,
		ConditionalFormatting_LessThanDialog_Description,
		ConditionalFormatting_BetweenDialog_Description,
		ConditionalFormatting_EqualToDialog_Description,
		ConditionalFormatting_TextThatContainsDialog_Description,
		ConditionalFormatting_DateOccurringDialog_Description,
		ConditionalFormatting_CustomConditionDialog_Description,
		ConditionalFormatting_Top10ItemsDialog_Description,
		ConditionalFormatting_Bottom10ItemsDialog_Description,
		ConditionalFormatting_Top10PercentDialog_Description,
		ConditionalFormatting_Bottom10PercentDialog_Description,
		ConditionalFormatting_AboveAverageDialog_Description,
		ConditionalFormatting_BelowAverageDialog_Description,
		ConditionalFormatting_Dialog_ApplyFormatToWholeRowText,
		ConditionalFormatting_BetweenDialog_RangeValuesConnector,
		ConditionalFormatting_CustomConditionEditor_Title,
		ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessage,
		ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageEx,
		ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageTitle,
		ConditionalFormatting_GreaterThanDialog_Connector,
		ConditionalFormatting_LessThanDialog_Connector,
		ConditionalFormatting_BetweenDialog_Connector,
		ConditionalFormatting_EqualToDialog_Connector,
		ConditionalFormatting_TextThatContainsDialog_Connector,
		ConditionalFormatting_CustomConditionDialog_Connector,
		ConditionalFormatting_DateOccurringDialog_Connector,
		ConditionalFormatting_BelowAverageDialog_Connector,
		ConditionalFormatting_AboveAverageDialog_Connector,
		ConditionalFormatting_Bottom10PercentDialog_Connector,
		ConditionalFormatting_Top10PercentDialog_Connector,
		ConditionalFormatting_Bottom10ItemsDialog_Connector,
		ConditionalFormatting_Top10ItemsDialog_Connector,
		ConditionalFormatting_DateOccurringDialog_IntervalToday,
		ConditionalFormatting_DateOccurringDialog_IntervalYesterday,
		ConditionalFormatting_DateOccurringDialog_IntervalTomorrow,
		ConditionalFormatting_DateOccurringDialog_IntervalInTheLast7Days,
		ConditionalFormatting_DateOccurringDialog_IntervalLastWeek,
		ConditionalFormatting_DateOccurringDialog_IntervalThisWeek,
		ConditionalFormatting_DateOccurringDialog_IntervalNextWeek,
		ConditionalFormatting_DateOccurringDialog_IntervalLastMonth,
		ConditionalFormatting_DateOccurringDialog_IntervalThisMonth,
		ConditionalFormatting_DateOccurringDialog_IntervalNextMonth,
		#endregion
		#region conditional formatting manager
		ConditionalFormatting_Manager_Title,
		ConditionalFormatting_Manager_NewFormattingRule,
		ConditionalFormatting_Manager_EditFormattingRule,
		ConditionalFormatting_Manager_CellsContainDescription,
		ConditionalFormatting_Manager_DatesOccurring,
		ConditionalFormatting_Manager_CellValue,
		ConditionalFormatting_Manager_Blanks,
		ConditionalFormatting_Manager_NoBlanks,
		ConditionalFormatting_Manager_Containing,
		ConditionalFormatting_Manager_NotContaining,
		ConditionalFormatting_Manager_BeginningWith,
		ConditionalFormatting_Manager_EndingWith,
		ConditionalFormatting_Manager_SpecificText,
		ConditionalFormatting_Manager_Between,
		ConditionalFormatting_Manager_NotBetween,
		ConditionalFormatting_Manager_EqualTo,
		ConditionalFormatting_Manager_NotEqualTo,
		ConditionalFormatting_Manager_GreaterThan,
		ConditionalFormatting_Manager_LessThan,
		ConditionalFormatting_Manager_GreaterOrEqual,
		ConditionalFormatting_Manager_LessOrEqual,
		ConditionalFormatting_Manager_FormatCells,
		ConditionalFormatting_Manager_Font,
		ConditionalFormatting_Manager_Fill,
		ConditionalFormatting_Manager_FormulaDescription,
		ConditionalFormatting_Manager_GradedColorScale,
		ConditionalFormatting_Manager_DataBar,
		ConditionalFormatting_Manager_IconSet,
		ConditionalFormatting_Manager_Above,
		ConditionalFormatting_Manager_Below,
		ConditionalFormatting_Manager_Top,
		ConditionalFormatting_Manager_Bottom,
		ConditionalFormatting_Manager_TopBottomDescription,
		ConditionalFormatting_Manager_AboveBelowDescription,
		ConditionalFormatting_Manager_ValueBasedDescription,
		ConditionalFormatting_Manager_2ColorScaleDescription,
		ConditionalFormatting_Manager_3ColorScaleDescription,
		ConditionalFormatting_Manager_IconDescription,
		ConditionalFormatting_Manager_IconDescriptionConnector,
		ConditionalFormatting_Manager_IconDescriptionCondition,
		ConditionalFormatting_Manager_IconDescriptionValueCondition,
		ConditionalFormatting_Manager_SelectRuleType,
		ConditionalFormatting_Manager_EditRuleDescription,
		ConditionalFormatting_Manager_ContainTitle,
		ConditionalFormatting_Manager_AboveBelowTitle,
		ConditionalFormatting_Manager_TopBottomTitle,
		ConditionalFormatting_Manager_FormulaTitle,
		ConditionalFormatting_Manager_Preview,
		ConditionalFormatting_Manager_FormatButtonDescription,
		ConditionalFormatting_Manager_ClearButtonDescription,
		ConditionalFormatting_Manager_FormatBackgroundColor,
		ConditionalFormatting_Manager_FormatStrikethrough,
		ConditionalFormatting_Manager_FormatUnderline,
		ConditionalFormatting_Manager_FormatFontFamily,
		ConditionalFormatting_Manager_FormatFontStyle,
		ConditionalFormatting_Manager_FormatFontWeight,
		ConditionalFormatting_Manager_FormatFontSize,
		ConditionalFormatting_Manager_FormatColor,
		ConditionalFormatting_Manager_NoFormatSet,
		ConditionalFormatting_Manager_ShowRulesFor,
		ConditionalFormatting_Manager_NewButtonDescription,
		ConditionalFormatting_Manager_EditButtonDescription,
		ConditionalFormatting_Manager_DeleteButtonDescription,
		ConditionalFormatting_Manager_Minimum,
		ConditionalFormatting_Manager_Midpoint,
		ConditionalFormatting_Manager_Maximum,
		ConditionalFormatting_Manager_DataBarPositive,
		ConditionalFormatting_Manager_DataBarNegative,
		ConditionalFormatting_Manager_DataBarFill,
		ConditionalFormatting_Manager_DataBarColor,
		ConditionalFormatting_Manager_DataBarBorder,
		ConditionalFormatting_Manager_DataBarNoBorder,
		ConditionalFormatting_Manager_ApplySameAppearance,
		ConditionalFormatting_Manager_IconSetStyle,
		ConditionalFormatting_Manager_ReverseButtonDescription,
		ConditionalFormatting_Manager_IconSetValueType,
		ConditionalFormatting_Manager_IconSetRulesTitle,
		ConditionalFormatting_Manager_ValueBasedTitle,
		ConditionalFormatting_Manager_ValueBasedStyle,
		ConditionalFormatting_Manager_ValueBasedType,
		ConditionalFormatting_Manager_ValueBasedExpression,
		ConditionalFormatting_Manager_AboveBelowRangeDescription,
		ConditionalFormatting_Manager_TopBottomRangeDescription,
		ConditionalFormatting_Manager_ShowModeAll,
		ConditionalFormatting_Manager_ShowModeCurrent,
		ConditionalFormatting_Manager_FieldNameMode,
		ConditionalFormatting_Manager_ExpressionMode,
		ConditionalFormatting_Manager_FillModeSolid,
		ConditionalFormatting_Manager_FillModeGradient,
		ConditionalFormatting_Manager_AboveMode,
		ConditionalFormatting_Manager_BelowMode,
		ConditionalFormatting_Manager_PercentMode,
		ConditionalFormatting_Manager_NumberMode,
		ConditionalFormatting_Manager_Apply,
		ConditionalFormatting_Manager_Rule,
		ConditionalFormatting_Manager_AppliesTo,
		ConditionalFormatting_Manager_ApplyToRow,
		ConditionalFormatting_Manager_Format,
		ConditionalFormatting_Manager_MinValue,
		ConditionalFormatting_Manager_MaxValue,
		ConditionalFormatting_Manager_MinMaxValueType,
		ConditionalFormatting_Manager_Auto,
		ConditionalFormatting_Manager_Numeric,
		ConditionalFormatting_Manager_Date,
		ConditionalFormatting_Manager_Icon,
		ConditionalFormatting_Manager_CustomIconGroup,
		ConditionalFormatting_Manager_FormatIconDescription,
		ConditionalFormatting_Manager_CustomFormat,
		ConditionalFormatting_Manager_Up,
		ConditionalFormatting_Manager_Down,
		ConditionalFormatting_Manager_FilterAll, 
		#endregion
	}
	public class ConditionalFormattingLocalizer : DXLocalizer<ConditionalFormattingStringId> {
		#region populate table
		protected override void PopulateStringTable() {
			#region Conditional Formatting
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting, "Conditional Formatting");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules, "Highlight Cells Rules");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_GreaterThan, "Greater Than...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_LessThan, "Less Than...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_Between, "Between...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_EqualTo, "Equal To...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_TextThatContains, "Text that Contains...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_ADateOccurring, "A Date Occurring...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_CustomCondition, "Custom Condition...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules, "Top/Bottom Rules");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Top10Items, "Top 10 Items...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Bottom10Items, "Bottom 10 Items...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Top10Percent, "Top 10 %...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Bottom10Percent, "Bottom 10 %...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_AboveAverage, "Above Average...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_BelowAverage, "Below Average...");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_DataBars, "Data Bars");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ColorScales, "Color Scales");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_IconSets, "Icon Sets");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules, "Clear Rules");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules_FromAllColumns, "Clear Rules from All Columns");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules_FromCurrentColumns, "Clear Rules from This Column");
			AddString(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ManageRules, "Manage Rules");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_LightRedFillWithDarkRedText, "Light Red Fill with Dark Red Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_YellowFillWithDarkYellowText, "Yellow Fill with Dark Yellow Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_GreenFillWithDarkGreenText, "Green Fill with Dark Green Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_LightRedFill, "Light Red Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_LightGreenFill, "Light Green Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_RedText, "Red Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_GreenText, "Green Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_BoldText, "Bold Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_ItalicText, "Italic Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedFormat_StrikethroughText, "Strikethrough Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowRedColorScale, "Green - Yellow - Red Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedYellowGreenColorScale, "Red - Yellow - Green Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteRedColorScale, "Green - White - Red Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteGreenColorScale, "Red - White - Green Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_BlueWhiteRedColorScale, "Blue - White - Red Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteBlueColorScale, "Red - White - Blue Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_WhiteRedColorScale, "White - Red Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteColorScale, "Red - White Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteColorScale, "Green - White Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_WhiteGreenColorScale, "White - Green Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowColorScale, "Green - Yellow Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_YellowGreenColorScale, "Yellow - Green Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_Description, "Apply a color gradient to a range of\r\ncells in this column. The color indicates\r\nwhere each cell falls within that range.");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_BlueSolidDataBar, "Blue Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_GreenSolidDataBar, "Green Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_RedSolidDataBar, "Red Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_OrangeSolidDataBar, "Orange Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_LightBlueSolidDataBar, "Light Blue Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_PurpleSolidDataBar, "Purple Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_BlueGradientDataBar, "Blue Data Bar Gradient");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_GreenGradientDataBar, "Green Data Bar Gradient");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_RedGradientDataBar, "Red Data Bar Gradient");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_OrangeGradientDataBar, "Orange Data Bar Gradient");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_LightBlueGradientDataBar, "Light Blue Data Bar Gradient");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_PurpleGradientDataBar, "Purple Data Bar Gradient");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_Description, "Add a colored data bar to represent\r\nthe value in a cell. The higher the\r\nvalue, the longer the bar.");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_SolidFillGroup, "Solid Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_GradientFillGroup, "Gradient Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows3ColoredIconSet, "3 Arrows (Colored)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows3GrayIconSet, "3 Arrows (Gray)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Triangles3IconSet, "3 Triangles");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows4GrayIconSet, "4 Arrows (Gray)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows4ColoredIconSet, "4 Arrows (Colored)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows5GrayIconSet, "5 Arrows (Gray)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows5ColoredIconSet, "5 Arrows (Colored)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3UnrimmedIconSet, "3 Traffic Lights (Unrimmed)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3RimmedIconSet, "3 Traffic Lights (Rimmed)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Signs3IconSet, "3 Signs");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_TrafficLights4IconSet, "4 Traffic Lights");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_RedToBlackIconSet, "Red To Black");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Symbols3CircledIconSet, "3 Symbols (Circled)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Symbols3UncircledIconSet, "3 Symbols (Uncircled)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Flags3IconSet, "3 Flags");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Stars3IconSet, "3 Stars");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Ratings4IconSet, "4 Ratings");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Quarters5IconSet, "5 Quarters");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Ratings5IconSet, "5 Ratings");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Boxes5IconSet, "5 Boxes");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsColoredIconSet, "Arrows (Colored)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsGrayIconSet, "Arrows (Gray)");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeTrianglesIconSet, "Triangles");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Description, "Choose a set of icons to represent\r\nthe values in this column.");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_DirectionalGroup, "Directional");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_ShapesGroup, "Shapes");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_IndicatorsGroup, "Indicators");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_RatingsGroup, "Ratings");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeGroup, "Positive/Negative");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_GreaterThanDialog_Title, "Greater Than");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_LessThanDialog_Title, "Less Than");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_Title, "Between");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_EqualToDialog_Title, "Equal To");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_TextThatContainsDialog_Title, "Text That Contains");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_Title, "A Date Occurring");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_Title, "Custom Condition");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Top10ItemsDialog_Title, "Top 10 Items");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Bottom10ItemsDialog_Title, "Bottom 10 Items");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Top10PercentDialog_Title, "Top 10%");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Bottom10PercentDialog_Title, "Bottom 10%");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_AboveAverageDialog_Title, "Above Average");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BelowAverageDialog_Title, "Below Average");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_GreaterThanDialog_Description, "Format cells that are GREATER THAN:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_LessThanDialog_Description, "Format cells that are LESS THAN:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_Description, "Format cells that are BETWEEN:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_EqualToDialog_Description, "Format cells that are EQUAL TO:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_TextThatContainsDialog_Description, "Format cells that contain the text:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_Description, "Format cells that contain a date occurring:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_Description, "Format cells that match the following condition:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Top10ItemsDialog_Description, "Format cells that rank in the TOP:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Bottom10ItemsDialog_Description, "Format cells that rank in the BOTTOM:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Top10PercentDialog_Description, "Format cells that rank in the TOP:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Bottom10PercentDialog_Description, "Format cells that rank in the BOTTOM:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_AboveAverageDialog_Description, "Format cells that are ABOVE AVERAGE:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BelowAverageDialog_Description, "Format cells that are BELOW AVERAGE:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_GreaterThanDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_LessThanDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_EqualToDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_TextThatContainsDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Top10ItemsDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Bottom10ItemsDialog_Connector, "with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Top10PercentDialog_Connector, "% with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Bottom10PercentDialog_Connector, "% with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_AboveAverageDialog_Connector, "for this column with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BelowAverageDialog_Connector, "for this column with");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Dialog_ApplyFormatToWholeRowText, "Apply format to the entire row");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_RangeValuesConnector, "and");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionEditor_Title, "Custom Condition Editor");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessage, "The specified expression contains invalid symbols (character {0}).");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageEx, "The specified expression is invalid.");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageTitle, "Error");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalYesterday, "Yesterday");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalToday, "Today");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalTomorrow, "Tomorrow");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalInTheLast7Days, "In the last 7 days");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalLastWeek, "Last week");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalThisWeek, "This week");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalNextWeek, "Next week");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalLastMonth, "Last month");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalThisMonth, "This month");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalNextMonth, "Next month");
			#region conditional formatting manager
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Title, "Conditional Formatting Rules Manager");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NewFormattingRule, "New Formatting Rule");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EditFormattingRule, "Edit Formatting Rule");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CellsContainDescription, "Format only cells that contain");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DatesOccurring, "Dates Occurring");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CellValue, "Cell Value");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Blanks, "Blanks");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NoBlanks, "No Blanks");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Containing, "Containing");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NotContaining, "Not Containing");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_BeginningWith, "Beginning With");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EndingWith, "Ending With");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_SpecificText, "Specific Text");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Between, "Between");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NotBetween, "Not Between");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EqualTo, "Equal To");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NotEqualTo, "Not Equal To");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_GreaterThan, "Greater Than");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_LessThan, "Less Than");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_GreaterOrEqual, "Greater Than Or Equal To");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_LessOrEqual, "Less Than Or Equal To");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatCells, "Format Cells");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Font, "Font");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Fill, "Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormulaDescription, "Use a formula to determine which cells to format");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_GradedColorScale, "Graded Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBar, "Data Bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconSet, "Icon Set");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Above, "Above Average");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Below, "Below Average");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Top, "Top");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Bottom, "Bottom");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_TopBottomDescription, "Format only top or bottom ranked values");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AboveBelowDescription, "Format only values that are above or below average");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ValueBasedDescription, "Format all cells based on their values");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_2ColorScaleDescription, "2-Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_3ColorScaleDescription, "3-Color Scale");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescription, "Icon Sets");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescriptionConnector, " and");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescriptionCondition, "when");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescriptionValueCondition, "when value is");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_SelectRuleType, "Select a Rule Type:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EditRuleDescription, "Edit the Rule Description:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ContainTitle, "Format only cells with:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AboveBelowTitle, "Format values that are:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_TopBottomTitle, "Format values that rank in the:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormulaTitle, "Format values where this formula is true:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Preview, "Preview:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatButtonDescription, "Format...");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ClearButtonDescription, "Clear");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatBackgroundColor, "Background Color:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatStrikethrough, "Strikethrough");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatUnderline, "Underline");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatFontFamily, "Font:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatFontStyle, "Font style:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatFontWeight, "Font weight:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatFontSize, "Size:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatColor, "Color:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NoFormatSet, "No Format Set");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ShowRulesFor, "Show formatting rules for:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NewButtonDescription, "New Rule...");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EditButtonDescription, "Edit Rule...");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DeleteButtonDescription, "Delete Rule");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Minimum, "Minimum");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Midpoint, "Midpoint");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Maximum, "Maximum");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBarPositive, "Positive Bar Appearance:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBarNegative, "Negative Bar Appearance:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBarFill, "Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBarColor, "Color");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBarBorder, "Border");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBarNoBorder, "No Border");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ApplySameAppearance, "Apply same appearance as positive bar");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconSetStyle, "Icon Style:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ReverseButtonDescription, "Reverse Icon Order");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconSetValueType, "Value Type:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconSetRulesTitle, "Display each icon according to these rules:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ValueBasedTitle, "Format all cells based on their values:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ValueBasedStyle, "Format Style:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ValueBasedType, "Type:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ValueBasedExpression, "Value:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AboveBelowRangeDescription, "the average of the column's cell values");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_TopBottomRangeDescription, "% of the column's cell values");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ShowModeAll, "All");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ShowModeCurrent, "Current Column");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FieldNameMode, "Field Name");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ExpressionMode, "Expression");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FillModeSolid, "Solid Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FillModeGradient, "Gradient Fill");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AboveMode, "Above");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_BelowMode, "Below");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_PercentMode, "Percent");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NumberMode, "Number");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Apply, "Apply");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Rule, "Rule");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AppliesTo, "Column");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ApplyToRow, "Apply to the row");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Format, "Format");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_MinValue, "Min Value:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_MaxValue, "Max Value:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_MinMaxValueType, "Min/Max Value Type:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Auto, "<Auto>");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Numeric, "Numeric");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Date, "Date");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Icon, "Icon");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CustomIconGroup, "Custom");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatIconDescription, "Select the Icon:");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CustomFormat, "Custom Format...");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Up, "Up");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Down, "Down");
			AddString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FilterAll, "(All)");
			#endregion
			#endregion
		}
		#endregion
		static ConditionalFormattingLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<ConditionalFormattingStringId>(CreateDefaultLocalizer()));
		}
		public static string GetString(ConditionalFormattingStringId id) {
			return Active.GetLocalizedString(id);
		}
		internal static string GetString(string stringId) {
			return GetString((ConditionalFormattingStringId)Enum.Parse(typeof(ConditionalFormattingStringId), stringId, false));
		}
		public override XtraLocalizer<ConditionalFormattingStringId> CreateResXLocalizer() {
			return new ConditionalFormattingResXLocalizer();
		}
		public static XtraLocalizer<ConditionalFormattingStringId> CreateDefaultLocalizer() {
			return new ConditionalFormattingResXLocalizer();
		}
	}
	public class ConditionalFormattingResXLocalizer : DXResXLocalizer<ConditionalFormattingStringId> {
		public ConditionalFormattingResXLocalizer() : base(new ConditionalFormattingLocalizer()) { }
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Core.Core.ConditionalFormatting.LocalizationRes", typeof(ConditionalFormattingResXLocalizer).Assembly);
		}
	}
}
