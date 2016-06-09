#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors.Controls;
using FilterDateType = DevExpress.XtraEditors.FilterDateType;
namespace DevExpress.DashboardWin.Native {
	static class GridColumnTotalExtensions {
		public static string Localize(this GridColumnTotalType summaryType) {
			DashboardWinStringId id;
			switch(summaryType) {
				case GridColumnTotalType.Avg:
					id = DashboardWinStringId.GridColumnTotalAvgBarItemCaption;
					break;
				case GridColumnTotalType.Max:
					id = DashboardWinStringId.GridColumnTotalMaxBarItemCaption;
					break;
				case GridColumnTotalType.Min:
					id = DashboardWinStringId.GridColumnTotalMinBarItemCaption;
					break;
				case GridColumnTotalType.Sum:
					id = DashboardWinStringId.GridColumnTotalSumBarItemCaption;
					break;
				case GridColumnTotalType.Count:
					id = DashboardWinStringId.GridColumnTotalCountBarItemCaption;
					break;
				case GridColumnTotalType.Auto:
					id = DashboardWinStringId.GridColumnTotalAutoBarItemCaption;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
	}
	static class FormatRuleExtensions {
		public static string Caption(this DashboardItemFormatRule rule) {
			if(!rule.IsValid) 
				return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleInvalidRule);
			FormatConditionBase condition = rule.Condition;
			FormatConditionValue v = condition as FormatConditionValue;
			if(v != null) {
				if(v.Value2 == null)
					return v.Condition.Caption(v.Value1);
				else 
					return v.Condition.Caption(v.Value1, v.Value2);
			}
			FormatConditionTopBottom tb = condition as FormatConditionTopBottom;
			if(tb != null) {
				string template = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionTopBottomTemplate);
				string percentValue = string.Empty;
				if(tb.RankType == DashboardFormatConditionValueType.Percent)
					percentValue = DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardFormatConditionPercentValue);
				return string.Format(template, tb.TopBottom.Localize(), tb.Rank, percentValue);
			}
			FormatConditionAverage avg = condition as FormatConditionAverage;
			if(avg != null)
				return avg.AverageType.Localize();
			FormatConditionDateOccuring date = condition as FormatConditionDateOccuring;
			if(date != null)
				return date.DateType.Localize();
			FormatConditionExpression expr = condition as FormatConditionExpression;
			if(expr != null)
				return expr.Expression;
			FormatConditionRangeSet rSet = condition as FormatConditionRangeSet;
			if(rSet != null)
				return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeSetTemplate), rSet.RangeSet.Count);
			FormatConditionBar bar = condition as FormatConditionBar;
			if(bar != null)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionBarTemplate);
			FormatConditionColorRangeBar colorRangeBar = condition as FormatConditionColorRangeBar;
			if(colorRangeBar != null)
				return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionColorRangeBarTemplate), colorRangeBar.RangeSet.Count);
			FormatConditionGradientRangeBar gradientRangeBar = condition as FormatConditionGradientRangeBar;
			if(gradientRangeBar != null)
				return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionGradientRangeBarTemplate), gradientRangeBar.SegmentCount);
			FormatConditionRangeGradient rGradient = condition as FormatConditionRangeGradient;
			if(rGradient != null)
				return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeGradientTemplate), rGradient.SegmentCount);
			throw new ArgumentException(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionUndefinedConditionException));
		}
		static string Caption(this DashboardFormatCondition type, object value) {
			switch(type) {
				case DashboardFormatCondition.Equal:
				case DashboardFormatCondition.NotEqual:
				case DashboardFormatCondition.Less:
				case DashboardFormatCondition.LessOrEqual:
				case DashboardFormatCondition.Greater:
				case DashboardFormatCondition.GreaterOrEqual:
				case DashboardFormatCondition.ContainsText:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardFormatConditionTemplate), type.Localize(), type.ToSign(), value);
				default:
					return string.Empty;
			}
		}
		static string Caption(this DashboardFormatCondition type, object value1, object value2) {
			switch(type) {			 
				case DashboardFormatCondition.Between:
				case DashboardFormatCondition.NotBetween:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardFormatConditionBetweenTemplate), type.Localize(), value1, value2);
				case DashboardFormatCondition.BetweenOrEqual:
				case DashboardFormatCondition.NotBetweenOrEqual:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardFormatConditionBetweenOrEqualTemplate), type.Localize(), value1, value2);
				default:
					return string.Empty;
			}
		}
		public static string Localize(this DashboardFormatCondition type) {
			DashboardWinStringId id;
			switch(type) {
				case DashboardFormatCondition.Equal:
					id = DashboardWinStringId.CommandFormatRuleEqualTo;
					break;
				case DashboardFormatCondition.NotEqual:
					id = DashboardWinStringId.CommandFormatRuleNotEqualTo;
					break;
				case DashboardFormatCondition.Between:
					id = DashboardWinStringId.CommandFormatRuleBetween;
					break;
				case DashboardFormatCondition.NotBetween:
					id = DashboardWinStringId.CommandFormatRuleNotBetween;
					break;
				case DashboardFormatCondition.BetweenOrEqual:
					id = DashboardWinStringId.CommandFormatRuleBetweenOrEqual;
					break;
				case DashboardFormatCondition.NotBetweenOrEqual:
					id = DashboardWinStringId.CommandFormatRuleNotBetweenOrEqual;
					break;
				case DashboardFormatCondition.Less:
					id = DashboardWinStringId.CommandFormatRuleLessThan;
					break;
				case DashboardFormatCondition.LessOrEqual:
					id = DashboardWinStringId.CommandFormatRuleLessThanOrEqualTo;
					break;
				case DashboardFormatCondition.Greater:
					id = DashboardWinStringId.CommandFormatRuleGreaterThan;
					break;
				case DashboardFormatCondition.GreaterOrEqual:
					id = DashboardWinStringId.CommandFormatRuleGreaterThanOrEqualTo;
					break;
				case DashboardFormatCondition.ContainsText:
					id = DashboardWinStringId.CommandFormatRuleContains;
					break;
				default: 
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static string Descript(this DashboardFormatCondition type) {
			DashboardWinStringId id;
			switch(type) {
				case DashboardFormatCondition.Equal:
					id = DashboardWinStringId.CommandFormatRuleEqualToDescription;
					break;
				case DashboardFormatCondition.NotEqual:
					id = DashboardWinStringId.CommandFormatRuleNotEqualToDescription;
					break;
				case DashboardFormatCondition.Between:
					id = DashboardWinStringId.CommandFormatRuleBetweenDescription;
					break;
				case DashboardFormatCondition.NotBetween:
					id = DashboardWinStringId.CommandFormatRuleNotBetweenDescription;
					break;
				case DashboardFormatCondition.BetweenOrEqual:
					id = DashboardWinStringId.CommandFormatRuleBetweenOrEqualDescription;
					break;
				case DashboardFormatCondition.NotBetweenOrEqual:
					id = DashboardWinStringId.CommandFormatRuleNotBetweenOrEqualDescription;
					break;
				case DashboardFormatCondition.Less:
					id = DashboardWinStringId.CommandFormatRuleLessThanDescription;
					break;
				case DashboardFormatCondition.LessOrEqual:
					id = DashboardWinStringId.CommandFormatRuleLessThanOrEqualToDescription;
					break;
				case DashboardFormatCondition.Greater:
					id = DashboardWinStringId.CommandFormatRuleGreaterThanDescription;
					break;
				case DashboardFormatCondition.GreaterOrEqual:
					id = DashboardWinStringId.CommandFormatRuleGreaterThanOrEqualToDescription;
					break;
				case DashboardFormatCondition.ContainsText:
					id = DashboardWinStringId.CommandFormatRuleContainsDescription;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static Image Icon(this DashboardFormatCondition type) {
			switch(type) {
				case DashboardFormatCondition.Equal:
					return GetIconImage("Equal");
				case DashboardFormatCondition.NotEqual:
					return GetIconImage("NotEqual");
				case DashboardFormatCondition.Between:
					return GetIconImage("Between");
				case DashboardFormatCondition.NotBetween:
					return GetIconImage("NotBetween");
				case DashboardFormatCondition.BetweenOrEqual:
					return GetIconImage("BetweenOrEqual");
				case DashboardFormatCondition.NotBetweenOrEqual:
					return GetIconImage("NotBetweenOrEqual");
				case DashboardFormatCondition.Less:
					return GetIconImage("Less");
				case DashboardFormatCondition.LessOrEqual:
					return GetIconImage("LessOrEqual");
				case DashboardFormatCondition.Greater:
					return GetIconImage("Greater");
				case DashboardFormatCondition.GreaterOrEqual:
					return GetIconImage("GreaterOrEqual");
				case DashboardFormatCondition.ContainsText:
					return GetIconImage("ContainsText");
				default:
					return null;
			}
		}
		public static string Localize(this DashboardFormatConditionTopBottomType type) {
			DashboardWinStringId id;
			switch(type) {
				case DashboardFormatConditionTopBottomType.Top:
					id = DashboardWinStringId.CommandFormatRuleTopN;
					break;
				case DashboardFormatConditionTopBottomType.Bottom:
					id = DashboardWinStringId.CommandFormatRuleBottomN;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static string Descript(this DashboardFormatConditionTopBottomType type) {
			DashboardWinStringId id;
			switch(type) {
				case DashboardFormatConditionTopBottomType.Top:
					id = DashboardWinStringId.CommandFormatRuleTopNDescription;
					break;
				case DashboardFormatConditionTopBottomType.Bottom:
					id = DashboardWinStringId.CommandFormatRuleBottomNDescription;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static Image Icon(this DashboardFormatConditionTopBottomType type) {
			switch(type) {
				case DashboardFormatConditionTopBottomType.Top:
					return GetIconImage("Top");
				case DashboardFormatConditionTopBottomType.Bottom:
					return GetIconImage("Bottom");
				default:
					return null;
			}
		}
		public static string Localize(this DashboardFormatConditionAboveBelowType type) {
			DashboardWinStringId id;
			switch(type) {
				case DashboardFormatConditionAboveBelowType.Above:
					id = DashboardWinStringId.CommandFormatRuleAboveAverage;
					break;
				case DashboardFormatConditionAboveBelowType.Below:
					id = DashboardWinStringId.CommandFormatRuleBelowAverage;
					break;
				case DashboardFormatConditionAboveBelowType.AboveOrEqual:
					id = DashboardWinStringId.CommandFormatRuleAboveOrEqualAverage;
					break;
				case DashboardFormatConditionAboveBelowType.BelowOrEqual:
					id = DashboardWinStringId.CommandFormatRuleBelowOrEqualAverage;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static string Descript(this DashboardFormatConditionAboveBelowType type) {
			switch(type) {
				case DashboardFormatConditionAboveBelowType.Above:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleAboveAverageDescription);
				case DashboardFormatConditionAboveBelowType.Below:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleBelowAverageDescription);
				case DashboardFormatConditionAboveBelowType.AboveOrEqual:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleAboveOrEqualAverageDescription);
				case DashboardFormatConditionAboveBelowType.BelowOrEqual:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleBelowOrEqualAverageDescription);
				default:
					return string.Empty;
			}
		}
		public static Image Icon(this DashboardFormatConditionAboveBelowType type) {
			switch(type) {
				case DashboardFormatConditionAboveBelowType.Above:
					return GetIconImage("Greater");
				case DashboardFormatConditionAboveBelowType.AboveOrEqual:
					return GetIconImage("GreaterOrEqual");
				case DashboardFormatConditionAboveBelowType.Below:
					return GetIconImage("Less");
				case DashboardFormatConditionAboveBelowType.BelowOrEqual:
					return GetIconImage("LessOrEqual");
				default:
					return null;
			}
		}
		public static string Localize(this FormatConditionRangeSetTypeGroups type) {
			switch(type) {
				case FormatConditionRangeSetTypeGroups.Ranges2:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeSetRanges2Caption);
				case FormatConditionRangeSetTypeGroups.Ranges3:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeSetRanges3Caption);
				case FormatConditionRangeSetTypeGroups.Ranges4:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeSetRanges4Caption);
				case FormatConditionRangeSetTypeGroups.Ranges5:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeSetRanges5Caption);
				default:
					return null;
			}
		}
		public static string Localize(this FormatConditionRangeGradientTypeGroups type) {
			switch(type) {
				case FormatConditionRangeGradientTypeGroups.TwoColors:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeGradientTwoColorsCaption);
				case FormatConditionRangeGradientTypeGroups.ThreeColors:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionRangeGradientThreeColorsCaption);
				default:
					return null;
			}
		}
		public static string Localize(this FormatConditionRangeSetPredefinedType rangeSetType) {
			switch(rangeSetType) {
				case FormatConditionRangeSetPredefinedType.Arrows2:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsColored), 2);
				case FormatConditionRangeSetPredefinedType.Arrows3:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsColored), 3);
				case FormatConditionRangeSetPredefinedType.Arrows4:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsColored), 4);
				case FormatConditionRangeSetPredefinedType.Arrows5:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsColored), 5);
				case FormatConditionRangeSetPredefinedType.ArrowsGray2:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsGray), 2);
				case FormatConditionRangeSetPredefinedType.ArrowsGray3:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsGray), 3);
				case FormatConditionRangeSetPredefinedType.ArrowsGray4:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsGray), 4);
				case FormatConditionRangeSetPredefinedType.ArrowsGray5:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconArrowsGray), 5);
				case FormatConditionRangeSetPredefinedType.PositiveNegative3:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconPositiveNegativeTriangles);
				case FormatConditionRangeSetPredefinedType.Stars3:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconStars);
				case FormatConditionRangeSetPredefinedType.Quarters5:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconQuarters);
				case FormatConditionRangeSetPredefinedType.Bars4:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconBars), 4);
				case FormatConditionRangeSetPredefinedType.Bars5:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconBars), 5);
				case FormatConditionRangeSetPredefinedType.Boxes5:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconBoxes);
				case FormatConditionRangeSetPredefinedType.TrafficLights3:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconTrafficLights);
				case FormatConditionRangeSetPredefinedType.Circles2:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconCircles), 2);
				case FormatConditionRangeSetPredefinedType.Circles3:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconCircles), 3);
				case FormatConditionRangeSetPredefinedType.Circles4:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconCircles), 4);
				case FormatConditionRangeSetPredefinedType.CirclesRedToBlack4:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconCirclesRedToBlack);
				case FormatConditionRangeSetPredefinedType.Signs3:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconSigns);
				case FormatConditionRangeSetPredefinedType.Symbols2:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconSymbolsUncircled), 2);
				case FormatConditionRangeSetPredefinedType.Symbols3:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconSymbolsUncircled), 3);
				case FormatConditionRangeSetPredefinedType.SymbolsCircled2:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconSymbolsCircled), 2);
				case FormatConditionRangeSetPredefinedType.SymbolsCircled3:
					return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconSymbolsCircled), 3);
				case FormatConditionRangeSetPredefinedType.Flags3:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetIconFlags);
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsPaleRedGreen);
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedGreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsPaleRedGreenBlue);
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedYellowGreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsPaleRedYellowGreenBlue);
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedOrangeYellowGreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsPaleRedOrangeYellowGreenBlue);
				case FormatConditionRangeSetPredefinedType.ColorsRedGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsRedGreen);
				case FormatConditionRangeSetPredefinedType.ColorsRedGreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsRedGreenBlue);
				case FormatConditionRangeSetPredefinedType.ColorsRedYellowGreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsRedYellowGreenBlue);
				case FormatConditionRangeSetPredefinedType.ColorsRedOrangeYellowGreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetColorsRedOrangeYellowGreenBlue);
				default:
					return string.Empty;
			}
		}
		public static string Descript(this FormatConditionRangeSetPredefinedType iconRangeSetType) {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeSetDescription);
		}
		public static string Localize(this FormatConditionRangeGradientPredefinedType gradientType) {
			switch(gradientType) {
				case FormatConditionRangeGradientPredefinedType.GreenWhite:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenWhite);
				case FormatConditionRangeGradientPredefinedType.WhiteGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientWhiteGreen);
				case FormatConditionRangeGradientPredefinedType.RedWhite:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedWhite);
				case FormatConditionRangeGradientPredefinedType.WhiteRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientWhiteRed);
				case FormatConditionRangeGradientPredefinedType.GreenYellow:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenYellow);
				case FormatConditionRangeGradientPredefinedType.YellowGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientYellowGreen);
				case FormatConditionRangeGradientPredefinedType.RedYellow:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedYellow);
				case FormatConditionRangeGradientPredefinedType.YellowRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientYellowRed);
				case FormatConditionRangeGradientPredefinedType.BlueWhite:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueWhite);
				case FormatConditionRangeGradientPredefinedType.WhiteBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientWhiteBlue);
				case FormatConditionRangeGradientPredefinedType.BlueRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueRed);
				case FormatConditionRangeGradientPredefinedType.RedBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedBlue);
				case FormatConditionRangeGradientPredefinedType.BlueYellow:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueYellow);
				case FormatConditionRangeGradientPredefinedType.YellowBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientYellowBlue);
				case FormatConditionRangeGradientPredefinedType.BlueGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueGreen);
				case FormatConditionRangeGradientPredefinedType.GreenBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenBlue);
				case FormatConditionRangeGradientPredefinedType.GreenWhiteBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenWhiteBlue);
				case FormatConditionRangeGradientPredefinedType.BlueWhiteGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueWhiteGreen);
				case FormatConditionRangeGradientPredefinedType.RedWhiteBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedWhiteBlue);
				case FormatConditionRangeGradientPredefinedType.BlueWhiteRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueWhiteRed);
				case FormatConditionRangeGradientPredefinedType.GreenWhiteRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenWhiteRed);
				case FormatConditionRangeGradientPredefinedType.RedWhiteGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedWhiteGreen);
				case FormatConditionRangeGradientPredefinedType.GreenYellowRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenYellowRed);
				case FormatConditionRangeGradientPredefinedType.RedYellowGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedYellowGreen);
				case FormatConditionRangeGradientPredefinedType.BlueYellowRed:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueYellowRed);
				case FormatConditionRangeGradientPredefinedType.RedYellowBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientRedYellowBlue);
				case FormatConditionRangeGradientPredefinedType.GreenYellowBlue:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientGreenYellowBlue);
				case FormatConditionRangeGradientPredefinedType.BlueYellowGreen:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientBlueYellowGreen);
				default:
					return string.Empty;
			}
		}
		public static string Descript(this FormatConditionRangeGradientPredefinedType gradientType) {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.RangeGradientDescription);
		}
		public static string Localize(this FormatConditionAppearanceType appearanceType) {
			DashboardWinStringId id;
			switch(appearanceType) {
				case FormatConditionAppearanceType.Custom:
					id = DashboardWinStringId.FormatConditionAppearanceCustom;
					break;
				case FormatConditionAppearanceType.PaleRed:
					id = DashboardWinStringId.FormatConditionAppearancePaleRed;
					break;
				case FormatConditionAppearanceType.PaleYellow:
					id = DashboardWinStringId.FormatConditionAppearancePaleYellow;
					break;
				case FormatConditionAppearanceType.PaleGreen:
					id = DashboardWinStringId.FormatConditionAppearancePaleGreen;
					break;
				case FormatConditionAppearanceType.PaleBlue:
					id = DashboardWinStringId.FormatConditionAppearancePaleBlue;
					break;
				case FormatConditionAppearanceType.PalePurple:
					id = DashboardWinStringId.FormatConditionAppearancePalePurple;
					break;
				case FormatConditionAppearanceType.PaleCyan:
					id = DashboardWinStringId.FormatConditionAppearancePaleCyan;
					break;
				case FormatConditionAppearanceType.PaleOrange:
					id = DashboardWinStringId.FormatConditionAppearancePaleOrange;
					break;
				case FormatConditionAppearanceType.PaleGray:
					id = DashboardWinStringId.FormatConditionAppearancePaleGray;
					break;
				case FormatConditionAppearanceType.Red:
					id = DashboardWinStringId.FormatConditionAppearanceRed;
					break;
				case FormatConditionAppearanceType.Yellow:
					id = DashboardWinStringId.FormatConditionAppearanceYellow;
					break;
				case FormatConditionAppearanceType.Green:
					id = DashboardWinStringId.FormatConditionAppearanceGreen;
					break;
				case FormatConditionAppearanceType.Blue:
					id = DashboardWinStringId.FormatConditionAppearanceBlue;
					break;
				case FormatConditionAppearanceType.Purple:
					id = DashboardWinStringId.FormatConditionAppearancePurple;
					break;
				case FormatConditionAppearanceType.Cyan:
					id = DashboardWinStringId.FormatConditionAppearanceCyan;
					break;
				case FormatConditionAppearanceType.Orange:
					id = DashboardWinStringId.FormatConditionAppearanceOrange;
					break;
				case FormatConditionAppearanceType.Gray:
					id = DashboardWinStringId.FormatConditionAppearanceGray;
					break;
				case FormatConditionAppearanceType.GradientRed:
					id = DashboardWinStringId.FormatConditionAppearanceGradientRed;
					break;
				case FormatConditionAppearanceType.GradientYellow:
					id = DashboardWinStringId.FormatConditionAppearanceGradientYellow;
					break;
				case FormatConditionAppearanceType.GradientGreen:
					id = DashboardWinStringId.FormatConditionAppearanceGradientGreen;
					break;
				case FormatConditionAppearanceType.GradientBlue:
					id = DashboardWinStringId.FormatConditionAppearanceGradientBlue;
					break;
				case FormatConditionAppearanceType.GradientPurple:
					id = DashboardWinStringId.FormatConditionAppearanceGradientPurple;
					break;
				case FormatConditionAppearanceType.GradientCyan:
					id = DashboardWinStringId.FormatConditionAppearanceGradientCyan;
					break;
				case FormatConditionAppearanceType.GradientOrange:
					id = DashboardWinStringId.FormatConditionAppearanceGradientOrange;
					break;
				case FormatConditionAppearanceType.GradientTransparent:
					id = DashboardWinStringId.FormatConditionAppearanceGradientTransparent;
					break;
				case FormatConditionAppearanceType.FontBold:
					id = DashboardWinStringId.FormatConditionAppearanceFontBold;
					break;
				case FormatConditionAppearanceType.FontItalic:
					id = DashboardWinStringId.FormatConditionAppearanceFontItalic;
					break;
				case FormatConditionAppearanceType.FontUnderline:
					id = DashboardWinStringId.FormatConditionAppearanceFontUnderline;
					break;
				case FormatConditionAppearanceType.FontGrayed:
					id = DashboardWinStringId.FormatConditionAppearanceFontGrayed;
					break;
				case FormatConditionAppearanceType.FontRed:
					id = DashboardWinStringId.FormatConditionAppearanceFontRed;
					break;
				case FormatConditionAppearanceType.FontYellow:
					id = DashboardWinStringId.FormatConditionAppearanceFontYellow;
					break;
				case FormatConditionAppearanceType.FontGreen:
					id = DashboardWinStringId.FormatConditionAppearanceFontGreen;
					break;
				case FormatConditionAppearanceType.FontBlue:
					id = DashboardWinStringId.FormatConditionAppearanceFontBlue;
					break;
				default:
					id = DashboardWinStringId.FormatConditionAppearanceNone;
					break;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static string Localize(this FilterDateType type) {
			StringId id;
			switch(type) {
				case FilterDateType.BeyondThisYear:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalBeyondThisYear;
					break;
				case FilterDateType.LaterThisYear:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisYear;
					break;
				case FilterDateType.LaterThisMonth:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisMonth;
					break;
				case FilterDateType.LaterThisWeek:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisWeek;
					break;
				case FilterDateType.NextWeek:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalNextWeek;
					break;
				case FilterDateType.Tomorrow:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalTomorrow;
					break;
				case FilterDateType.Today:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalToday;
					break;
				case FilterDateType.Yesterday:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalYesterday;
					break;
				case FilterDateType.EarlierThisWeek:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisWeek;
					break;
				case FilterDateType.LastWeek:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLastWeek;
					break;
				case FilterDateType.EarlierThisMonth:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisMonth;
					break;
				case FilterDateType.EarlierThisYear:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisYear;
					break;
				case FilterDateType.PriorThisYear:
					id = StringId.FilterCriteriaToStringFunctionIsOutlookIntervalPriorThisYear;
					break;
				case FilterDateType.None:
					id = StringId.FilterCriteriaToStringFunctionNone;
					break;
				case FilterDateType.SpecificDate:
				case FilterDateType.User:
					return string.Empty;
				default:
					return type.ToString();
			}
			return Localizer.Active.GetLocalizedString(id);
		}
		public static string Localize(this FormatConditionIntersectionLevelMode type) {
			DashboardWinStringId id;
			switch(type) {
				case FormatConditionIntersectionLevelMode.Auto:
					id = DashboardWinStringId.IntersectionLevelModeAuto;
					break;
				case FormatConditionIntersectionLevelMode.FirstLevel:
					id = DashboardWinStringId.IntersectionLevelModeFirst;
					break;
				case FormatConditionIntersectionLevelMode.LastLevel:
					id = DashboardWinStringId.IntersectionLevelModeLast;
					break;
				case FormatConditionIntersectionLevelMode.AllLevels:
					id = DashboardWinStringId.IntersectionLevelModeAll;
					break;
				case FormatConditionIntersectionLevelMode.SpecificLevel:
					id = DashboardWinStringId.IntersectionLevelModeSpecific;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static string Localize(this DashboardFormatConditionValueType type) {
			DashboardWinStringId id;
			switch(type){
				case DashboardFormatConditionValueType.Automatic:
					id = DashboardWinStringId.FormatConditionAutomaticValueType;
					break;
				case DashboardFormatConditionValueType.Number:
					id = DashboardWinStringId.FormatConditionNumberValueType;
					break;
				case DashboardFormatConditionValueType.Percent:
					id = DashboardWinStringId.FormatConditionPercentValueType;
					break;
				default:
					return string.Empty;
			}
			return DashboardWinLocalizer.GetString(id);
		}
		public static bool IsGradient(this StyleMode styleMode) {
			return styleMode == StyleMode.GradientStop ||
				   styleMode == StyleMode.GradientNonemptyStop ||
				   styleMode == StyleMode.GradientGenerated ||
				   styleMode == StyleMode.BarGradientStop ||
				   styleMode == StyleMode.BarGradientNonemptyStop ||
				   styleMode == StyleMode.BarGradientGenerated;
		}
		static Image GetIconImage(string iconName) {
			return ImageHelper.GetImage("ConditionalFormatting." + iconName);
		}
	}
}
