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
using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.Utils;
namespace DevExpress.Charts.Native {
	public static class PatternUtils {
		public const string ArgumentValueSeparator = ": ";
		public const string ArgumentPattern = "{A}";
		public const string ArgumentPatternLowercase = "{a}";
		public const string ValuesPattern = "{V}";
		public const string ValuesPatternLowercase = "{v}";
		public const string SeriesNamePattern = "{S}";
		public const string SeriesNamePatternLowercase = "{s}";
		public const string StackedGroupPattern = "{G}";
		public const string StackedGroupPatternLowercase = "{g}";
		public const string ArgumentAndValuesPattern = ArgumentPattern + ArgumentValueSeparator + ValuesPattern;
		public const string ArgumentPlaceholder = ToolTipPatternUtils.ArgumentPattern;
		public const string ValuePlaceholder = ToolTipPatternUtils.ValuePattern;
		public const string Value1Placeholder = ToolTipPatternUtils.Value1Pattern;
		public const string Value2Placeholder = ToolTipPatternUtils.Value2Pattern;
		public const string PercentValuePlaceholder = ToolTipPatternUtils.PercentValuePattern;
		public const string HighValuePlaceholder = ToolTipPatternUtils.HighValuePattern;
		public const string LowValuePlaceholder = ToolTipPatternUtils.LowValuePattern;
		public const string OpenValuePlaceholder = ToolTipPatternUtils.OpenValuePattern;
		public const string CloseValuePlaceholder = ToolTipPatternUtils.CloseValuePattern;
		public const string SeriesNamePlaceholder = ToolTipPatternUtils.SeriesNamePattern;
		public const string ValueDurationPlaceholder = ToolTipPatternUtils.ValueDurationPattern;
		public const string WeightPlaceholder = ToolTipPatternUtils.WeightPattern;
		public const string DurationFormatTotalDays = "TD";
		public const string DurationFormatTotalHours = "TH";
		public const string DurationFormatTotalMinutes = "TM";
		public const string DurationFormatTotalSeconds = "TS";
		public const string DurationFormatTotalMilliseconds = "TMS";
		public const string QuarterFormat = "q";
		static int ExtractElement(string pattern, out string element) {
			element = String.Empty;
			int index = pattern.IndexOf('{');
			if (index == -1 || index == pattern.Length - 1)
				return -1;
			if (pattern[index + 1] == '{') {
				element = "{{";
				return index;
			}
			int endIndex = pattern.IndexOf('}', index + 1);
			if (endIndex == -1)
				return -1;
			int openBraceIndex = pattern.IndexOf("{", index + 1);
			if (openBraceIndex >= 0 && openBraceIndex < endIndex)
				return openBraceIndex;
			element = pattern.Substring(index, endIndex - index + 1);
			return index;
		}
		static string FormatTimeSpan(TimeSpan value, string format) {
			switch (format.ToUpperInvariant()) {
				case PatternUtils.DurationFormatTotalDays:
					return value.TotalDays.ToString(CultureInfo.CurrentCulture);
				case PatternUtils.DurationFormatTotalHours:
					return value.TotalHours.ToString(CultureInfo.CurrentCulture);
				case PatternUtils.DurationFormatTotalMinutes:
					return value.TotalMinutes.ToString(CultureInfo.CurrentCulture);
				case PatternUtils.DurationFormatTotalSeconds:
					return value.TotalSeconds.ToString(CultureInfo.CurrentCulture);
				case PatternUtils.DurationFormatTotalMilliseconds:
					return value.TotalMilliseconds.ToString(CultureInfo.CurrentCulture);
				default: return value.ToString(format, CultureInfo.CurrentCulture);
			}
		}
		public static string SelectFormat(DateTimeGridAlignmentNative gridAlignment) {
			switch (gridAlignment) {
				case DateTimeGridAlignmentNative.Year:
					return "yyyy";
				case DateTimeGridAlignmentNative.Quarter:
				case DateTimeGridAlignmentNative.Month:
					return "y";
				case DateTimeGridAlignmentNative.Week:
				case DateTimeGridAlignmentNative.Day:
					return "d";
				case DateTimeGridAlignmentNative.Hour:
				case DateTimeGridAlignmentNative.Minute:
					return "t";
				case DateTimeGridAlignmentNative.Second:
					return "T";
				case DateTimeGridAlignmentNative.Millisecond:
					return DateTimeUtilsExt.CreateLongTimePatternWithMillisecond();
				default:
					return "g";
			}
		}
		public static string CorrectTextForPattern(string pattern) {
			string result = pattern;
			result = result.Replace(ArgumentPattern, "{" + ArgumentPattern);
			result = result.Replace(ArgumentPatternLowercase, "{" + ArgumentPatternLowercase);
			result = result.Replace(ValuesPattern, "{" + ValuesPattern);
			result = result.Replace(ValuesPatternLowercase, "{" + ValuesPatternLowercase);
			result = result.Replace(SeriesNamePattern, "{" + SeriesNamePattern);
			result = result.Replace(SeriesNamePatternLowercase, "{" + SeriesNamePatternLowercase);
			return result;
		}
		public static List<string> ParsePattern(string pattern) {
			List<string> parsedPattern = new List<string>();
			if (pattern == null)
				return parsedPattern;
			int elementIndex;
			while (pattern.Length > 0) {
				string element;
				elementIndex = ExtractElement(pattern, out element);
				if (elementIndex == -1) {
					parsedPattern.Add(pattern);
					return parsedPattern;
				}
				if (elementIndex > 0)
					parsedPattern.Add(pattern.Substring(0, elementIndex));
				if (element.Length > 0)
					parsedPattern.Add(element == "{{" ? "{" : element);
				pattern = pattern.Substring(elementIndex + element.Length);
			}
			return parsedPattern;
		}
		public static string ReplacePlaceholder(string pattern, string oldPlaceholder, string newPlaceholder) {
			string newPattern = pattern;
			MatchCollection matches = Regex.Matches(pattern, "{" + oldPlaceholder + "(:[^}]*)?}");
			foreach (Match match in matches) {
				string format = match.Groups[1].Value;
				string newPatternPart = string.Empty;
				if (!string.IsNullOrEmpty(match.Groups[0].Value))
					newPatternPart = newPlaceholder + format;
				if (!string.IsNullOrEmpty(newPatternPart))
					newPattern = newPattern.Replace(match.Groups[0].Value, "{" + newPatternPart + "}");
			}
			return newPattern;
		}
		public static string ReplacePlaceholder(string pattern, string oldPlaceholder, string newPlaceholder1, string newPlaceholder2) {
			return ReplacePlaceholder(pattern, oldPlaceholder, newPlaceholder1, newPlaceholder2, ", ");
		}
		public static string ReplacePlaceholder(string pattern, string oldPlaceholder, string newPlaceholder1, string newPlaceholder2, string separator) {
			string newPattern = pattern;
			MatchCollection matches = Regex.Matches(pattern, "{" + oldPlaceholder + "(:[^}]*)?}");
			foreach (Match match in matches) {
				string format = match.Groups[1].Value;
				string newPatternPart = string.Empty;
				if (!string.IsNullOrEmpty(match.Groups[0].Value))
					newPatternPart = newPlaceholder1 + format + "}" + separator + "{" + newPlaceholder2 + format;
				if (!string.IsNullOrEmpty(newPatternPart))
					newPattern = newPattern.Replace(match.Groups[0].Value, "{" + newPatternPart + "}");
			}
			return newPattern;
		}
		public static string GetMinValuePlaceholder(RefinedPoint refinedPoint, Scale valueScaleType) {
			if (refinedPoint.SeriesPoint.UserValues.Length < 2)
				return PatternUtils.ValuePlaceholder;
			else
				switch (valueScaleType) {
					case Scale.Numerical:
						if (refinedPoint.SeriesPoint.UserValues[1] > refinedPoint.SeriesPoint.UserValues[0])
							return PatternUtils.Value1Placeholder;
						else
							return PatternUtils.Value2Placeholder;
					case Scale.DateTime:
						if (refinedPoint.SeriesPoint.DateTimeValues[1] > refinedPoint.SeriesPoint.DateTimeValues[0])
							return PatternUtils.Value1Placeholder;
						else
							return PatternUtils.Value2Placeholder;
					default:
						return string.Empty;
				}
		}
		public static string GetMaxValuePlaceholder(RefinedPoint refinedPoint, Scale valueScaleType) {
			if (refinedPoint.SeriesPoint.UserValues.Length < 2)
				return PatternUtils.ValuePlaceholder;
			else
				switch (valueScaleType) {
					case Scale.Numerical:
						if (refinedPoint.SeriesPoint.UserValues[1] < refinedPoint.SeriesPoint.UserValues[0])
							return PatternUtils.Value1Placeholder;
						else
							return PatternUtils.Value2Placeholder;
					case Scale.DateTime:
						if (refinedPoint.SeriesPoint.DateTimeValues[1] < refinedPoint.SeriesPoint.DateTimeValues[0])
							return PatternUtils.Value1Placeholder;
						else
							return PatternUtils.Value2Placeholder;
					default:
						return string.Empty;
				}
		}
		public static string Format(object value, string format) {
			IFormattable formattable = value as IFormattable;
			try {
				if (formattable != null) {
					if (value is DateTime && format == PatternUtils.QuarterFormat) {
						DateTime dateTimeValue = (DateTime)value;
						return dateTimeValue.ToString(QuarterFormatter.FormatDateTime(dateTimeValue, "QQ ", "Q{0}") + "yyyy");
					}
					if (value is TimeSpan)
						return FormatTimeSpan((TimeSpan)value, format);
					return formattable.ToString(format, CultureInfo.CurrentCulture);
				}
			}
			catch {
			}
			return value is string ? (string)value : value.ToString() + ":" + format;
		}
		public static string GetArgumentFormat(string pattern) {
			Match match = Regex.Match(pattern, "{" + ArgumentPlaceholder + "(:[^}]*)?}");
			return match.Groups[1].Value.Replace(":", string.Empty);
		}
		public static string ConstructDefaultPattern(IAxisData axis) {
			if (axis != null && axis.AxisScaleTypeMap != null) {
				string format = string.Empty;
				switch (axis.AxisScaleTypeMap.ScaleType) {
					case ActualScaleType.Numerical:
						format = ":G";
						break;
					case ActualScaleType.DateTime:
						format = ":" + PatternUtils.SelectFormat(axis.DateTimeScaleOptions.GridAlignment);
						break;
					case ActualScaleType.Qualitative:
					default: 
						break;
				}
				return axis.IsValueAxis ? "{" + PatternUtils.ValuePlaceholder + format + "}" : "{" + PatternUtils.ArgumentPlaceholder + format + "}";
			}
			else
				return "{" + PatternUtils.ValuePlaceholder + "}";
		}
		public static string GetAxisLabelFormat(string pattern) {
			Match match = Regex.Match(pattern, "{" + ArgumentPlaceholder + "(:[^}]*)?}");
			if(String.IsNullOrEmpty(match.Groups[1].Value))
				match = Regex.Match(pattern, "{" + ValuePlaceholder + "(:[^}]*)?}");
			return match.Groups[1].Value.Replace(":", string.Empty);
		}
		public static IEnumerable<string> GetCustomDateTimeFormats() {
			return new string[] { QuarterFormat, "%d" };
		}
	}
	public static class ToolTipPatternUtils {
		public const string ArgumentPattern = "A";
		public const string ValuePattern = "V";
		public const string SeriesNamePattern = "S";
		public const string StackedGroupPattern = "G";
		public const string Value1Pattern = "V1";
		public const string Value2Pattern = "V2";
		public const string WeightPattern = "W";
		public const string HighValuePattern = "HV";
		public const string LowValuePattern = "LV";
		public const string OpenValuePattern = "OV";
		public const string CloseValuePattern = "CV";
		public const string PercentValuePattern = "VP";
		public const string PointHintPattern = "HINT";
		public const string ValueDurationPattern = "VD";
		public static readonly string[] BaseViewPointPatterns = new string[3] { ArgumentPattern, ValuePattern, SeriesNamePattern };
		public static readonly string[] PercentViewPointPatterns = new string[4] { ArgumentPattern, ValuePattern, SeriesNamePattern, PercentValuePattern };
		public static readonly string[] StackedGroupViewPointPatterns = new string[4] { ArgumentPattern, ValuePattern, SeriesNamePattern, StackedGroupPattern };
		public static readonly string[] FullStackedGroupViewPointPatterns = new string[5] { ArgumentPattern, ValuePattern, SeriesNamePattern, PercentValuePattern, StackedGroupPattern };
		public static readonly string[] BubbleViewPointPatterns = new string[4] { ArgumentPattern, ValuePattern, SeriesNamePattern, WeightPattern };
		public static readonly string[] RangeViewPointPatterns = new string[5] { ArgumentPattern, Value1Pattern, Value2Pattern, ValueDurationPattern, SeriesNamePattern };
		public static readonly string[] FinancialViewPointPatterns = new string[6] { ArgumentPattern, HighValuePattern, LowValuePattern, OpenValuePattern, CloseValuePattern, SeriesNamePattern };
		public static readonly string[] BaseViewSeriesPatterns = new string[1] { SeriesNamePattern };
		public static readonly string[] StackedGroupViewSeriesPatterns = new string[2] { SeriesNamePattern, StackedGroupPattern };
		public static bool PrepareFragment(string fragment, out string pattern, out string format) {
			pattern = string.Empty;
			format = string.Empty;
			if (!(fragment.StartsWith("{") && fragment.EndsWith("}")))
				return false;
			pattern = fragment.Substring(1, fragment.Length - 2);
			int formatIndex = pattern.IndexOf(":");
			if (formatIndex >= 0) {
				format = pattern.Substring(formatIndex + 1).Trim();
				pattern = pattern.Substring(0, formatIndex);
			}
			pattern = pattern.Trim().ToUpper();
			return true;
		}
		public static List<string> SplitString(string splitingString, char leftSeparator, char rightSeparator) {
			List<string> substrings = new List<string>();
			int leftStringIndex = 0;
			int rightStringIndex = 0;
			int currentIndex = 0;
			if (!String.IsNullOrEmpty(splitingString)) {
				foreach (char charElement in splitingString) {
					if (charElement == leftSeparator)
						leftStringIndex = currentIndex;
					else
						if (charElement == rightSeparator) {
							rightStringIndex = currentIndex;
							substrings.Add(splitingString.Substring(leftStringIndex, rightStringIndex - leftStringIndex + 1));
						}
					currentIndex++;
				}
				return substrings;
			}
			else
				return null;
		}
	}
	public abstract class ToolTipPointDataToStringConverter {
		const string defaultStackedGroupPattern = " : {G}";
		readonly string defaulPointPattern;
		readonly ISeries series;
		readonly bool showOpen;
		readonly bool showClose;
		readonly bool allowArgument;
		readonly bool allowValue;
		object hint;
		protected string DefaultArgumentPattern { get { return "{A" + GetDefaultArgumentFormat() + "}"; } }
		protected abstract string DefaultValuePattern { get; }
		protected internal bool AllowArgument { get { return allowArgument; } }
		protected internal bool AllowValue { get { return allowValue; } }
		protected internal bool AllowAllPatternValues { get { return allowArgument && allowValue; } }
		public string DefaulPointPattern { get { return defaulPointPattern; } }
		protected virtual string GroupedPointValuePattern { get { return ""; } }
		public object Hint { 
			get { return hint; }
			set { hint = value; }
		}
		protected bool ShowOpen { get { return showOpen; } }
		protected bool ShowClose { get { return showClose; } }
		protected ToolTipPointDataToStringConverter(ISeries series, bool showOpen, bool showClose, bool showStackedGroup, bool allowArgument, bool allowValue) {
			this.showOpen = showOpen;
			this.showClose = showClose;
			this.series = series;
			this.allowArgument = allowArgument;
			this.allowValue = allowValue;
			string stackedGroupPattern = showStackedGroup ? defaultStackedGroupPattern : "";
			string argumentPattern = allowArgument ? DefaultArgumentPattern : "";
			string valuePattern = allowValue ? DefaultValuePattern : "";
			this.defaulPointPattern = argumentPattern + stackedGroupPattern + valuePattern;
		}
		protected ToolTipPointDataToStringConverter(ISeries series, bool showOpen, bool showClose, bool showStackedGroup) : this(series, showOpen, showClose, showStackedGroup, true, true) {
		}
		public ToolTipPointDataToStringConverter(ISeries series) : this(series, false) {
		}
		public ToolTipPointDataToStringConverter(ISeries series, bool showStackedGroup) : this(series, true, true, showStackedGroup) {
		}
		object[] GetPointValues(ISeriesPoint point, ISeries series) {
			object[] values = new object[point.UserValues.Length];
			switch (series.ValueScaleType) {
				case Scale.Numerical:
					point.UserValues.CopyTo(values, 0);
					return values;
				case Scale.DateTime:
					point.DateTimeValues.CopyTo(values, 0);
					return values;
				default:
					return null;
			}
		}
		string GetDefaultFormat(Scale scaleType) {
			if (scaleType == Scale.DateTime)
				return ":d";
			return "";
		}
		string GetDefaultArgumentFormat() {
			IXYSeriesView view = series.SeriesView as IXYSeriesView;
			if ((view != null) && (view.AxisXData != null) && (series.ArgumentScaleType == Scale.DateTime) && (view.AxisXData.Label != null))
				return ":" + GetFormatString(GetActualMeasureUnit(view.AxisXData.DateTimeScaleOptions));
			return GetDefaultFormat(series.ArgumentScaleType);
		}
		DateTimeMeasureUnitNative GetActualMeasureUnit(IDateTimeScaleOptions scaleOptions) {
			return (scaleOptions.ScaleMode == ScaleModeNative.Continuous) ? (DateTimeMeasureUnitNative)scaleOptions.GridAlignment : scaleOptions.MeasureUnit;
		}
		string GetFormatString(DateTimeMeasureUnitNative dateTimeMeasureUnitNative) {
			switch (dateTimeMeasureUnitNative) {
				case DateTimeMeasureUnitNative.Day:
					return "d";
				case DateTimeMeasureUnitNative.Hour:
					return "t";
				case DateTimeMeasureUnitNative.Millisecond:
					return DateTimeUtilsExt.CreateLongTimePatternWithMillisecond();
				case DateTimeMeasureUnitNative.Minute:
					return "t";
				case DateTimeMeasureUnitNative.Month:
					return "MMMM yyyy";
				case DateTimeMeasureUnitNative.Quarter:
					return "MMMM yyyy";
				case DateTimeMeasureUnitNative.Second:
					return "t";
				case DateTimeMeasureUnitNative.Week:
					return "d";
				case DateTimeMeasureUnitNative.Year:
					return "yyyy";
				default:
					return "";
			}
		}
		protected string GetDefaultValueFormat() {
			return GetDefaultFormat(series.ValueScaleType);
		}
		protected string ConvertToString(object value, string format) {
			if (value is double || value is DateTime)
				return CrosshairPatternUtils.Format(value, format);
			ChartDebug.Fail("Incorrect value type.");
			return value == null ? String.Empty : value.ToString();
		}
		protected string GetValueText(int valueIndex, ISeriesPoint point, ISeries series, string format) {
			object[] values = GetPointValues(point, series);
			if (valueIndex >= values.Length) {
				ChartDebug.Fail("Incorrect series point values count.");
				return string.Empty;
			}
			return ConvertToString(values[valueIndex], format);
		}
		public string GetGroupedPointPattern(bool allowArgument, bool allowValue) {
			string argumentPattern = allowArgument ? DefaultArgumentPattern : "";
			string valuePattern = allowValue ? GroupedPointValuePattern : "";
			return "{S} : " + argumentPattern + valuePattern;
		}
		public string GetArgumentText(ISeriesPoint point, ISeries series, string format) {
			if (point.UserArgument == null)
				return string.Empty;
			switch (series.ArgumentScaleType) {
				case Scale.Qualitative:
					return point.QualitativeArgument;
				case Scale.Numerical:
					return CrosshairPatternUtils.Format(point.NumericalArgument, format);
				case Scale.DateTime:
					return CrosshairPatternUtils.Format(point.DateTimeArgument, format);
				default:
					throw new Exception("Incorrect argument scale type.");
			}
		}
		public string GetHintText() {
			if (hint != null)
				return hint.ToString();
			else
				return String.Empty;
		}
	}
	public class ToolTipValueToStringConverter : ToolTipPointDataToStringConverter {
		protected override string DefaultValuePattern { get { return " : " + GroupedPointValuePattern; } }
		protected override string GroupedPointValuePattern { get { return "{V" + GetDefaultValueFormat() + "}"; } }
		protected ToolTipValueToStringConverter(ISeries series, bool allowArgument, bool allowValue) : base(series, true, true, false, allowArgument, allowValue) {
		}
		public ToolTipValueToStringConverter(ISeries series) : base(series) {
		}
		public ToolTipValueToStringConverter(ISeries series, bool showStackedGroup) : base(series, showStackedGroup) {
		}
		public string GetValueText(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(0, point, series, format);
		}
	}
	public class ToolTipRangeValueToStringConverter : ToolTipPointDataToStringConverter {
		protected override string DefaultValuePattern { get { return " : " + GroupedPointValuePattern; } }
		protected override string GroupedPointValuePattern { get { return "{V1" + GetDefaultValueFormat() + "} : {V2" + GetDefaultValueFormat() + "}"; } }
		public ToolTipRangeValueToStringConverter(ISeries series) : base(series) { 
		}
		public string GetValue1Text(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(0, point, series, format);
		}
		public string GetValue2Text(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(1, point, series, format);
		}
	}
	public class ToolTipBubbleValueToStringConverter : ToolTipValueToStringConverter {
		protected override string DefaultValuePattern { get { return " : " + GroupedPointValuePattern; } }
		protected override string GroupedPointValuePattern { get { return "{V" + GetDefaultValueFormat() + "} : {W" + GetDefaultValueFormat() + "}"; } }
		public ToolTipBubbleValueToStringConverter(ISeries series) : base(series) { 
		}
		public string GetWeightText(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(1, point, series, format);
		}
	}
	public class ToolTipFinancialValueToStringConverter : ToolTipPointDataToStringConverter {
		protected override string DefaultValuePattern { get { return GroupedPointValuePattern; } }
		protected override string GroupedPointValuePattern {
			get {
				return "\nHigh: {HV" + GetDefaultValueFormat() + "}\nLow: {LV" + GetDefaultValueFormat() + "}" +
					(ShowOpen ? "\nOpen: {OV" + GetDefaultValueFormat() + "}" : "") + (ShowClose ? "\nClose: {CV" + GetDefaultValueFormat() + "}" : "");
			}
		}
		public ToolTipFinancialValueToStringConverter(ISeries series) : this(series, true, true) { 
		}
		public ToolTipFinancialValueToStringConverter(ISeries series, bool showOpen, bool showClose) : base(series, showOpen, showClose, false) {
		}
		public string GetLowValueText(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(0, point, series, format);
		}
		public string GetHighValueText(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(1, point, series, format);
		}
		public string GetOpenValueText(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(2, point, series, format);
		}
		public string GetCloseValueText(ISeriesPoint point, ISeries series, string format) {
			return GetValueText(3, point, series, format);
		}
	}
	public abstract class ToolTipPercentValueToStringConverter : ToolTipValueToStringConverter {
		protected override string DefaultValuePattern { get { return " : " + GroupedPointValuePattern; } }
		protected override string GroupedPointValuePattern { get { return "{V" + GetDefaultValueFormat() + "} ({VP:P2})"; } }
		public ToolTipPercentValueToStringConverter(ISeries series) : base(series) { 
		}
		public ToolTipPercentValueToStringConverter(ISeries series, bool showStackedGroup) : base(series, showStackedGroup) {
		}
	}
	public class ToolTipFullStackedValueToStringConverter : ToolTipPercentValueToStringConverter {
		public ToolTipFullStackedValueToStringConverter(ISeries series) : base(series) { 
		}
		public ToolTipFullStackedValueToStringConverter(ISeries series, bool showStackedGroup) : base(series, showStackedGroup) {
		}
	}
	public class ToolTipSimpleDiagramValueToStringConverter : ToolTipPercentValueToStringConverter {
		public ToolTipSimpleDiagramValueToStringConverter(ISeries series) : base(series) { 
		}
	}
	public class CrosshairGroupHeaderValueToStringConverter : ToolTipValueToStringConverter {
		protected override string DefaultValuePattern { get { return GroupedPointValuePattern; } }
		protected override string GroupedPointValuePattern { get { return "{V" + GetDefaultValueFormat() + "}"; } }
		public CrosshairGroupHeaderValueToStringConverter(ISeries series, bool allowArgument, bool allowValue) : base(series, allowArgument, allowValue) {
		}
	}
	public class CrosshairPatternUtils {
		public static string Format(object value, string format) {
			IFormattable formattable = value as IFormattable;
			try {
				if (formattable != null) {
					if (value is DateTime && format == PatternUtils.QuarterFormat) {
						DateTime dateTimeValue = (DateTime)value;
						return dateTimeValue.ToString(QuarterFormatter.FormatDateTime(dateTimeValue, "QQ ", "Q{0}") + "yyyy");
					}
					return formattable.ToString(format, CultureInfo.CurrentCulture);
				}
			}
			catch {
			}
			return value is string ? (string)value : value.ToString() + ":" + format;
		}
	}
}
