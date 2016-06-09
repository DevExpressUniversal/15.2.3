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

using DevExpress.XtraExport.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DevExpress.Export.Xl;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Printing {
	public class FormatConditionRuleBase : IFormatConditionRuleBase {
		public virtual bool IsValid { get { return true; } }
		internal Type ColumnType { get; set; }
		public static System.Drawing.Color GetColor(Brush brush, bool isPositiveBrush = true) {
			if(brush is SolidColorBrush)
				return GetColor(((SolidColorBrush)brush).Color);
			var gradientBrush = brush as LinearGradientBrush;
			if(gradientBrush != null && gradientBrush.GradientStops.Count > 0) {
				if(isPositiveBrush)
					return GetColor(gradientBrush.GradientStops.First().Color);
				else
					return GetColor(gradientBrush.GradientStops.Last().Color);
			}
			return System.Drawing.Color.Empty;
		}
		public static System.Drawing.Color GetColor(Color color) {
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
	public class FormatConditionRuleDataBarExportWrapper : FormatConditionRuleBase, IFormatConditionRuleDataBar {
		readonly DataBarFormatCondition FormatCondition;
		DataBarFormat Format { get { return FormatCondition.Format; } }
		public bool GradientFill { get { return IsGradientFill(); } }
		public bool AllowNegativeAxis {
			get { return true; }
		}
		public System.Drawing.Color AxisColor {
			get { return GetColor(Format.ZeroLineBrush); }
		}
		public System.Drawing.Color BorderColor {
			get { return GetColor(Format.BorderBrush); }
		}
		public int Direction {
			get { return (int)XlDataBarDirection.LeftToRight; }
		}
		public bool DrawAxis {
			get { return true; }
		}
		public bool DrawAxisAtMiddle {
			get { return false; }
		}
		public System.Drawing.Color FillColor {
			get { return GetColor(Format.Fill); }
		}
		public object MaxValue {
			get { return IndicatorFormatConditionInfo.GetParsedDecimalValue(FormatCondition.MaxValue); }
		}
		public object MinValue {
			get { return IndicatorFormatConditionInfo.GetParsedDecimalValue(FormatCondition.MinValue); }
		}
		public System.Drawing.Color NegativeBorderColor {
			get { return GetColor(Format.BorderBrushNegative); }
		}
		public System.Drawing.Color NegativeFillColor {
			get { return GetColor(Format.FillNegative, false); }
		}
		public string PredefinedName {
			get { return FormatCondition.PredefinedFormatName; }
		}
		public bool ShowBarOnly {
			get { return false; }
		}
		bool IsGradientFill() {
			return Format.Fill is LinearGradientBrush || Format.FillNegative is LinearGradientBrush;
		}
		public XlCondFmtValueObjectType MaxType {
			get { return XlCondFmtValueObjectType.Number; }
		}
		public XlCondFmtValueObjectType MinType {
			get { return XlCondFmtValueObjectType.Number; }
		}
		public FormatConditionRuleDataBarExportWrapper(DataBarFormatCondition formatCondition) {
			FormatCondition = formatCondition;
		}
	}
	class FormatConverter {
		readonly Format Format;
		public FormatConverter(Format format) {
			Format = format;
		}
		public XlDifferentialFormatting GetXlDifferentialFormatting() {
			XlDifferentialFormatting appearance = new XlDifferentialFormatting();
			appearance.Fill = GetXlFill();
			appearance.Font = GetXlFont();
			return appearance;
		}
		XlFill GetXlFill() {
			if(Format == null || Format.Background == null)
				return null;
			XlFill fill = new XlFill();
			fill.BackColor = FormatConditionRuleBase.GetColor(Format.Background);
			fill.PatternType = XlPatternType.Solid;
			return fill;
		}
		XlFont GetXlFont() {
			XlFont font = new XlFont();
			if(Format == null)
				return font;
			if(Format.FontSize > 1)
				font.Size = Format.FontSize;
			if(Format.FontFamily != null)
				font.Name = Format.FontFamily.Source;
			font.Color = FormatConditionRuleBase.GetColor(Format.Foreground);
			font.Bold = Format.FontWeight >= FontWeights.Bold;
			font.StrikeThrough = IsStrikeThrough();
			font.Italic = Format.FontStyle == FontStyles.Italic || Format.FontStyle == FontStyles.Oblique;
			font.Extend = Format.FontStretch >= FontStretches.Expanded;
			font.Condense = Format.FontStretch <= FontStretches.Condensed;
			if(IsUnderline())
				font.Underline = XlUnderlineType.Single;
			return font;
		}
		bool IsStrikeThrough() {
			return Format.TextDecorations != null && Format.TextDecorations.Contains(TextDecorations.Strikethrough[0]);
		}
		bool IsUnderline() {
			return Format.TextDecorations != null && Format.TextDecorations.Contains(TextDecorations.Underline[0]);
		}
	}
	public class FormatConditionRuleValueExportWrapper : FormatConditionRuleBase, IFormatConditionRuleValue {
		readonly FormatCondition FormatCondition;
		readonly XlDifferentialFormatting appearance;
		public XlDifferentialFormatting Appearance {
			get { return appearance; }
		}
		public FormatConditions Condition {
			get { return (FormatConditions)FormatCondition.ValueRule; }
		}
		public string Expression {
			get { return FormatCondition.Expression; }
		}
		public object Value1 {
			get { return GetConvertedValue(FormatCondition.Value1); }
		}
		public object Value2 {
			get { return GetConvertedValue(FormatCondition.Value2); }
		}
		public FormatConditionRuleValueExportWrapper(FormatCondition formatCondition) {
			FormatCondition = formatCondition;
			appearance = new FormatConverter(FormatCondition.Format).GetXlDifferentialFormatting();
		}
		object GetConvertedValue(object value) {
			if(ColumnType != null && value != null) {
				if(!ColumnType.Equals(value.GetType()))
					try {
						value = System.Convert.ChangeType(value, ColumnType);
					}
					catch {
						value = null;
					}
			}
			return value;
		}
	}
	public class FormatConditionRuleIconSetExportWrapper : FormatConditionRuleBase, IFormatConditionRuleIconSet {
		readonly IconSetFormatCondition FormatCondition;
		readonly XlCondFmtIconSetType? iconSetTypeCore;
		public XlCondFmtIconSetType IconSetType {
			get { return iconSetTypeCore.Value; }
		}
		public bool Percent {
			get { return FormatCondition.Format.ElementThresholdType == ConditionalFormattingValueType.Percent; }
		}
		public bool Reverse {
			get { return false; }
		}
		public bool ShowValues {
			get { return true; }
		}
		public override bool IsValid { get { return iconSetTypeCore.HasValue; } }
		public FormatConditionRuleIconSetExportWrapper(IconSetFormatCondition formatCondition) {
			FormatCondition = formatCondition;
			iconSetTypeCore = GetXlIconSetType(FormatCondition.Format);
		}
		static XlCondFmtIconSetType? GetXlIconSetType(IconSetFormat iconSetFormat) {
			return iconSetFormat.IconSetType;
		}
		public IList<XlCondFmtValueObject> Values {
			get {
				var values = new List<XlCondFmtValueObject>();
				foreach(var icon in FormatCondition.Format.Elements.Reverse()) {
					XlCondFmtValueObjectType ot = FormatCondition.Format.ElementThresholdType == ConditionalFormattingValueType.Percent ? XlCondFmtValueObjectType.Percent : XlCondFmtValueObjectType.Number;
					XlCondFmtValueObject xcfvo = (icon.Threshold == Double.NegativeInfinity) ? new XlCondFmtValueObject() { Value = 0, ObjectType = ot } : new XlCondFmtValueObject() { ObjectType = ot, Value = icon.Threshold };
					values.Add(xcfvo);
				}
				if(values.Count > 0) values[0].ObjectType = XlCondFmtValueObjectType.Percent;
				return values;
			}
		}
	}
	public class FormatConditionRuleColorScale2ExportWrapper : FormatConditionRuleBase, IFormatConditionRule2ColorScale {
		protected readonly ColorScaleFormatCondition FormatCondition;
		public System.Drawing.Color MaxColor {
			get { return GetColor(FormatCondition.Format.ColorMax); }
		}
		public object MaxValue {
			get { return IndicatorFormatConditionInfo.GetParsedDecimalValue(FormatCondition.MaxValue); }
		}
		public XlCondFmtValueObjectType MaxType {
			get { return XlCondFmtValueObjectType.Number; }
		}
		public System.Drawing.Color MinColor {
			get { return GetColor(FormatCondition.Format.ColorMin); }
		}
		public object MinValue {
			get { return IndicatorFormatConditionInfo.GetParsedDecimalValue(FormatCondition.MinValue); }
		}
		public XlCondFmtValueObjectType MinType {
			get { return XlCondFmtValueObjectType.Number; }
		}
		public FormatConditionRuleColorScale2ExportWrapper(ColorScaleFormatCondition formatCondition) {
			FormatCondition = formatCondition;
		}
		protected decimal? ActualMinValue { get { return MinValue as decimal?; } }
		protected decimal? ActualMaxValue { get { return MaxValue as decimal?; } }
	}
	public class FormatConditionRuleColorScale3ExportWrapper : FormatConditionRuleColorScale2ExportWrapper, IFormatConditionRule3ColorScale {
		public System.Drawing.Color MidpointColor {
			get { return GetColor(FormatCondition.Format.ColorMiddle.Value); }
		}
		public object MidpointValue {
			get {
				if(ActualMinValue.HasValue && ActualMaxValue.HasValue)
					return (ActualMinValue + ActualMaxValue) / 2;
				return null;
			}
		}
		public XlCondFmtValueObjectType MidpointType {
			get { return XlCondFmtValueObjectType.Number; }
		}
		public FormatConditionRuleColorScale3ExportWrapper(ColorScaleFormatCondition formatCondition) : base(formatCondition) { }
	}
	public class FormatConditionRuleTopBottomExportWrapper : FormatConditionRuleBase, IFormatConditionRuleTopBottom {
		readonly XlDifferentialFormatting appearance;
		readonly TopBottomRuleFormatCondition FormatCondition;
		const int MinRank = 1;
		const int MaxPercent = 100;
		const int MaxItemCount = 1000;
		public XlDifferentialFormatting Appearance {
			get { return appearance; }
		}
		public int Rank {
			get { return GetRank(); }
		}
		public bool Bottom {
			get { return FormatCondition.Rule == TopBottomRule.BottomItems || FormatCondition.Rule == TopBottomRule.BottomPercent; }
		}
		public bool Percent {
			get { return FormatCondition.Rule == TopBottomRule.BottomPercent || FormatCondition.Rule == TopBottomRule.TopPercent; }
		}
		public FormatConditionRuleTopBottomExportWrapper(TopBottomRuleFormatCondition formatCondition) {
			FormatCondition = formatCondition;
			appearance = new FormatConverter(formatCondition.Format).GetXlDifferentialFormatting();
		}
		int GetRank() {
			double maxRank = Percent ? MaxPercent : MaxItemCount;
			double rank = Math.Max(MinRank, FormatCondition.Threshold);
			rank = Math.Min(maxRank, rank);
			return Convert.ToInt32(rank);
		}
	}
	public class FormatConditionRuleAboveBelowAverageExportWrapper : FormatConditionRuleBase, IFormatConditionRuleAboveBelowAverage {
		readonly XlCondFmtAverageCondition condition;
		readonly XlDifferentialFormatting formatting;
		public XlCondFmtAverageCondition Condition {
			get { return condition; }
		}
		public XlDifferentialFormatting Formatting {
			get { return formatting; }
		}
		public FormatConditionRuleAboveBelowAverageExportWrapper(TopBottomRuleFormatCondition formatCondition) {
			switch(formatCondition.Rule) {
				case TopBottomRule.AboveAverage: condition = XlCondFmtAverageCondition.Above; break;
				case TopBottomRule.BelowAverage: condition = XlCondFmtAverageCondition.Below; break;
			}
			formatting = new FormatConverter(formatCondition.Format).GetXlDifferentialFormatting();
		}
	}
	public class FormatConditionRuleDateOccuringExportWrapper : FormatConditionRuleBase, IFormatConditionRuleDateOccuring {
		readonly XlCondFmtTimePeriod dateTypeCore;
		readonly XlDifferentialFormatting formattingCore;
		public XlCondFmtTimePeriod DateType {
			get { return dateTypeCore; }
		}
		public XlDifferentialFormatting Formatting {
			get { return formattingCore; }
		}
		public FormatConditionRuleDateOccuringExportWrapper(Format format, DateOccurringConditionRule rule) {
			formattingCore = new FormatConverter(format).GetXlDifferentialFormatting();
			dateTypeCore = GetTimePeriodByDateOccuringRule(rule).Value;
		}
		public static XlCondFmtTimePeriod? GetTimePeriodByDateOccuringRule(DateOccurringConditionRule rule) {
			switch(rule) {
				case DateOccurringConditionRule.InTheLast7Days:
					return XlCondFmtTimePeriod.Last7Days;
				case DateOccurringConditionRule.LastMonth:
					return XlCondFmtTimePeriod.LastMonth;
				case DateOccurringConditionRule.LastWeek:
					return XlCondFmtTimePeriod.LastWeek;
				case DateOccurringConditionRule.NextMonth:
					return XlCondFmtTimePeriod.NextMonth;
				case DateOccurringConditionRule.NextWeek:
					return XlCondFmtTimePeriod.NextWeek;
				case DateOccurringConditionRule.ThisMonth:
					return XlCondFmtTimePeriod.ThisMonth;
				case DateOccurringConditionRule.ThisWeek:
					return XlCondFmtTimePeriod.ThisWeek;
				case DateOccurringConditionRule.Today:
					return XlCondFmtTimePeriod.Today;
				case DateOccurringConditionRule.Tomorrow:
					return XlCondFmtTimePeriod.Tomorrow;
				case DateOccurringConditionRule.Yesterday:
					return XlCondFmtTimePeriod.Yesterday;
				default:
					return null;
			}
		}
	}
}
