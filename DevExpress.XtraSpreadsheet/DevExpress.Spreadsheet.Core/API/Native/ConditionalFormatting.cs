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
#if !SL
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region ConditionalFormattingDataBarAxisPosition
	public enum ConditionalFormattingDataBarAxisPosition {
		None = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingDataBarAxisPosition.None,
		Automatic = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingDataBarAxisPosition.Automatic,
		Middle = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingDataBarAxisPosition.Middle
	}
	#endregion
	#region ConditionalFormattingDataBarDirection
	public enum ConditionalFormattingDataBarDirection {
		Context = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingDataBarDirection.Context,
		LeftToRight = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingDataBarDirection.LeftToRight,
		RightToLeft = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingDataBarDirection.RightToLeft
	}
	#endregion
	#region ConditionalFormattingTimePeriod
	public enum ConditionalFormattingTimePeriod {
		Last7Days = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.Last7Days,
		LastMonth = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.LastMonth,
		LastWeek = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.LastWeek,
		NextMonth = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.NextMonth,
		NextWeek = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.NextWeek,
		ThisMonth = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.ThisMonth,
		ThisWeek = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.ThisWeek,
		Today = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.Today,
		Tomorrow = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.Tomorrow,
		Yesterday = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTimePeriod.Yesterday,
	}
	#endregion
	#region ConditionalFormattingAverageCondition
	public enum ConditionalFormattingAverageCondition {
		Above = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingAverageCondition.Above,
		AboveOrEqual = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingAverageCondition.AboveOrEqual,
		Below = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingAverageCondition.Below,
		BelowOrEqual = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingAverageCondition.BelowOrEqual
	}
	#endregion
	#region ConditionalFormattingTextCondition
	public enum ConditionalFormattingTextCondition {
		Contains = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTextCondition.Contains,
		NotContains = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTextCondition.NotContains,
		BeginsWith = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTextCondition.BeginsWith,
		EndsWith = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingTextCondition.EndsWith
	}
	#endregion
	#region ConditionalFormattingSpecialCondition
	public enum ConditionalFormattingSpecialCondition {
		ContainError = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingSpecialCondition.ContainError,
		NotContainError = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingSpecialCondition.NotContainError,
		ContainBlanks = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingSpecialCondition.ContainBlanks,
		ContainNonBlanks = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingSpecialCondition.ContainNonBlanks,
		ContainUniqueValue = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingSpecialCondition.ContainUniqueValue,
		ContainDuplicateValue = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingSpecialCondition.ContainDuplicateValue,
	}
	#endregion
	#region ConditionalFormattingRankCondition
	public enum ConditionalFormattingRankCondition {
		TopByRank = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingRankCondition.TopByRank,
		TopByPercent = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingRankCondition.TopByPercent,
		BottomByRank = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingRankCondition.BottomByRank,
		BottomByPercent = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingRankCondition.BottomByPercent
	}
	#endregion
	#region ConditionalFormattingExpressionCondition
	public enum ConditionalFormattingExpressionCondition {
		EqualTo = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingExpressionCondition.EqualTo,
		UnequalTo = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingExpressionCondition.InequalTo,
		LessThan = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingExpressionCondition.LessThan,
		LessThanOrEqual = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingExpressionCondition.LessThanOrEqual,
		GreaterThan = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingExpressionCondition.GreaterThan,
		GreaterThanOrEqual = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingExpressionCondition.GreaterThanOrEqual
	}
	#endregion
	#region ConditionalFormattingRangeCondition
	public enum ConditionalFormattingRangeCondition {
		Inside = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingRangeCondition.Inside,
		Outside = DevExpress.XtraSpreadsheet.Model.ConditionalFormattingRangeCondition.Outside
	}
	#endregion
	#region ConditionalFormattingValueType
	public enum ConditionalFormattingValueType {
		Unknown,
		Formula,
		Number,
		Percent,
		Percentile, 
		MinMax,
		Auto
	}
	#endregion
	#region ConditionalFormattingValue
	public interface ConditionalFormattingValue {
		ConditionalFormattingValueType ValueType { get; }
		string Value { get; }
	}
	#endregion
	public enum ConditionalFormattingValueOperator {
		Greater,
		GreaterOrEqual
	}
	#region ConditionalFormattingIconSetValue
	public interface ConditionalFormattingIconSetValue : ConditionalFormattingValue {
		ConditionalFormattingValueOperator ComparisonOperator { get; }
	}
	#endregion
	#region Obsolete interfaces
	[Obsolete("Use the ConditionalFormattingValue interface instead.", true)]
	public interface ConditionalFormattingInsideValue : ConditionalFormattingValue {
	}
	[Obsolete("Use the ConditionalFormattingValue interface instead.", true)]
	public interface ConditionalFormattingExtremumValue : ConditionalFormattingValue {
		bool IsExtremum { get; }
	}
	[Obsolete("Use the ConditionalFormattingIconSetValue interface instead.", true)]
	public interface ConditionalFormattingIconSetInsideValue : ConditionalFormattingIconSetValue {
	}
	#endregion
	#region ConditionalFormatting
	public interface ConditionalFormatting {
		int Priority { get; set; } 
		bool StopIfTrue { get; set; }
		Range Range { get; set; }
		[Obsolete("Use the Range property instead.", false)]
		IList<Range> Ranges { get; set; }
	}
	#endregion
	#region ISupportsFormatting
	public interface ISupportsFormatting {
		Formatting Formatting { get; }
	}
	#endregion
	#region RankConditionalFormatting
	public interface RankConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		ConditionalFormattingRankCondition Condition { get; set; }
		int Rank { get; set; }
	}
	#endregion
	#region AverageConditionalFormatting
	public interface AverageConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		ConditionalFormattingAverageCondition Condition { get; set; }
		int StdDev { get; set; }
	}
	#endregion
	#region RangeConditionalFormatting
	public interface RangeConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		ConditionalFormattingRangeCondition Condition { get; set; }
		string LowBound { get; set; }
		string HighBound { get; set; }
	}
	#endregion
	#region TextConditionalFormatting
	public interface TextConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		ConditionalFormattingTextCondition Condition { get; set; }
		string Text { get; set; }
	}
	#endregion
	#region SpecialConditionalFormatting
	public interface SpecialConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		ConditionalFormattingSpecialCondition Condition { get; set; }
	}
	#endregion
	#region TimePeriodConditionalFormatting
	public interface TimePeriodConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		ConditionalFormattingTimePeriod Condition { get; set; }
	}
	#endregion
	#region ExpressionConditionalFormatting
	public interface ExpressionConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		string Value { get; set; }
		ConditionalFormattingExpressionCondition Condition { get; set; }
	}
	#endregion
	#region FormulaExpressionConditionalFormatting
	public interface FormulaExpressionConditionalFormatting : ConditionalFormatting, ISupportsFormatting {
		string Expression { get; set; }
	}
	#endregion
	#region ColorScale3ConditionalFormatting
	public interface ColorScale3ConditionalFormatting : ColorScale2ConditionalFormatting {
		ConditionalFormattingValue MidPoint { get; }
		Color MidPointColor { get; }
	}
	#endregion
	#region ColorScale2ConditionalFormatting
	public interface ColorScale2ConditionalFormatting : ConditionalFormatting {
		ConditionalFormattingValue MinPoint { get; }
		Color MinPointColor { get; }
		ConditionalFormattingValue MaxPoint { get; }
		Color MaxPointColor { get; }
	}
	#endregion
	#region DataBarConditionalFormatting
	public interface DataBarConditionalFormatting : ConditionalFormatting {
		ConditionalFormattingValue LowBound { get; }
		ConditionalFormattingValue HighBound { get; }
		Color Color { get; set; }
		bool ShowValue { get; set; }
		ConditionalFormattingDataBarAxisPosition AxisPosition { get; set; }
		ConditionalFormattingDataBarDirection Direction { get; set; }
		bool GradientFill { get; set; }
		Color NegativeBarColor { get; set; }
		Color BorderColor { get; set; }
		Color NegativeBarBorderColor { get; set; }
		Color AxisColor { get; set; }
		int MinLength { get; set; }
		int MaxLength { get; set; }
	}
	#endregion
	#region IconSetConditionalFormatting
	public interface IconSetConditionalFormatting : ConditionalFormatting {
		bool ShowValue { get; set; }
		IconSetType IconSet { get; set; }
		bool Reversed { get; set; }
		ConditionalFormattingIconSetValue[] Values { get; }
		bool IsCustom { get; set; }
		void SetCustomIcon(int index, ConditionalFormattingCustomIcon value);
		ConditionalFormattingCustomIcon GetIcon(int index);
	}
	#endregion
	#region IconSetType
	public enum IconSetType {
		None = DevExpress.XtraSpreadsheet.Model.IconSetType.None,
		Arrows3 = DevExpress.XtraSpreadsheet.Model.IconSetType.Arrows3,
		ArrowsGray3 = DevExpress.XtraSpreadsheet.Model.IconSetType.ArrowsGray3,
		Flags3 = DevExpress.XtraSpreadsheet.Model.IconSetType.Flags3,
		TrafficLights13 = DevExpress.XtraSpreadsheet.Model.IconSetType.TrafficLights13,
		TrafficLights23 = DevExpress.XtraSpreadsheet.Model.IconSetType.TrafficLights23,
		Signs3 = DevExpress.XtraSpreadsheet.Model.IconSetType.Signs3,
		Symbols3 = DevExpress.XtraSpreadsheet.Model.IconSetType.Symbols3,
		Symbols23 = DevExpress.XtraSpreadsheet.Model.IconSetType.Symbols23,
		Stars3 = DevExpress.XtraSpreadsheet.Model.IconSetType.Stars3,
		Triangles3 = DevExpress.XtraSpreadsheet.Model.IconSetType.Triangles3,
		Arrows4 = DevExpress.XtraSpreadsheet.Model.IconSetType.Arrows4,
		ArrowsGray4 = DevExpress.XtraSpreadsheet.Model.IconSetType.ArrowsGray4,
		RedToBlack4 = DevExpress.XtraSpreadsheet.Model.IconSetType.RedToBlack4,
		Rating4 = DevExpress.XtraSpreadsheet.Model.IconSetType.Rating4,
		TrafficLights4 = DevExpress.XtraSpreadsheet.Model.IconSetType.TrafficLights4,
		Arrows5 = DevExpress.XtraSpreadsheet.Model.IconSetType.Arrows5,
		ArrowsGray5 = DevExpress.XtraSpreadsheet.Model.IconSetType.ArrowsGray5,
		Rating5 = DevExpress.XtraSpreadsheet.Model.IconSetType.Rating5,
		Quarters5 = DevExpress.XtraSpreadsheet.Model.IconSetType.Quarters5,
		Boxes5 = DevExpress.XtraSpreadsheet.Model.IconSetType.Boxes5
	}
	#endregion
	#region ConditionalFormattingCustomIcon
	public struct ConditionalFormattingCustomIcon {
		public IconSetType IconSet { get; set; }
		public int IconIndex { get; set; }
	}
	#endregion
	#region ConditionalFormattingCollection
	public interface ConditionalFormattingCollection {
		int Count { get; }
		ConditionalFormatting this[int index] { get; }
		AverageConditionalFormatting AddAverageConditionalFormatting(Range range, ConditionalFormattingAverageCondition condition);
		AverageConditionalFormatting AddAverageConditionalFormatting(Range range, ConditionalFormattingAverageCondition condition, int stdDev);
		RankConditionalFormatting AddRankConditionalFormatting(Range range, ConditionalFormattingRankCondition condition, int rank);
		RangeConditionalFormatting AddRangeConditionalFormatting(Range range, ConditionalFormattingRangeCondition condition, string lowBound, string highBound);
		TextConditionalFormatting AddTextConditionalFormatting(Range range, ConditionalFormattingTextCondition condition, string text);
		SpecialConditionalFormatting AddSpecialConditionalFormatting(Range range, ConditionalFormattingSpecialCondition condition);
		TimePeriodConditionalFormatting AddTimePeriodConditionalFormatting(Range range, ConditionalFormattingTimePeriod timePeriod);
		ExpressionConditionalFormatting AddExpressionConditionalFormatting(Range range, ConditionalFormattingExpressionCondition condition, string value);
		FormulaExpressionConditionalFormatting AddFormulaExpressionConditionalFormatting(Range range, string expression);
		ColorScale3ConditionalFormatting AddColorScale3ConditionalFormatting(Range range, ConditionalFormattingValue minPoint, Color minPointColor, ConditionalFormattingValue midPoint, Color midPointColor,
																		ConditionalFormattingValue maxPoint, Color maxPointColor);
		ColorScale2ConditionalFormatting AddColorScale2ConditionalFormatting(Range range, ConditionalFormattingValue minPoint, Color minPointColor,ConditionalFormattingValue maxPoint, Color maxPointColor);
		DataBarConditionalFormatting AddDataBarConditionalFormatting(Range range, ConditionalFormattingValue lowBound, ConditionalFormattingValue highBound, Color color);
		IconSetConditionalFormatting AddIconSetConditionalFormatting(Range range, IconSetType iconSet, ConditionalFormattingIconSetValue[] points);
		ConditionalFormattingIconSetValue CreateIconSetValue(ConditionalFormattingValueType valueType, string value, ConditionalFormattingValueOperator comparisonOperator);
		ConditionalFormattingValue CreateValue(ConditionalFormattingValueType valueType, string value);
		ConditionalFormattingValue CreateValue(ConditionalFormattingValueType valueType);
		void Remove(ConditionalFormatting formatting);
		int IndexOf(ConditionalFormatting formatting);
		bool Contains(ConditionalFormatting formatting);
		void RemoveAt(int index);
		void Remove(Range range);
		void Clear();
	}
	#endregion    
}
