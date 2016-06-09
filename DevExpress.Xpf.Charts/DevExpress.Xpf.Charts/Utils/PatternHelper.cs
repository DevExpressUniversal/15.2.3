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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Localization;
namespace DevExpress.Xpf.Charts.Native {
	public static class PatternHelper {
		public static string GetPointTextByFormatString(string formatString, Series series, RefinedPoint refinedPoint) {
			ToolTipPointDataToStringConverter converter = series.ToolTipPointValuesConverter;
			converter.Hint = SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint).ToolTipHint;
			PatternParser patternParser = new PatternParser(formatString, (IPatternHolder)series);
			patternParser.SetContext(refinedPoint, series);
			return patternParser.GetText(); 
		}
		public static string GetSeriesTextByFormatString(string pattern, Series series) {
			PatternParser patternParser = new PatternParser(pattern, (IPatternHolder)series);
			patternParser.SetContext(series);
			return patternParser.GetText();
		}
	}
	public static class PatternEditorUtils {
		const int RandomPointsCount = 1;
		static IPatternValuesSource CreateValuesSourceFromAxis(IAxisData axis) {
			return new PatternEditorValuesSource(axis.VisualRange.MaxValue);
		}
		static IPatternValuesSource CreateValuesSourceFromSeries(Series series) {
			ISideBySideStackedBarSeriesView stackedSeries = series as ISideBySideStackedBarSeriesView;
			object seriesGroup = stackedSeries != null ? stackedSeries.StackedGroup : "StackedGroup";
			PatternEditorValuesSource valuesSource = new PatternEditorValuesSource(GetPointArgument(series), GetPointValue(series), series.DisplayName, seriesGroup);
			return valuesSource;
		}
		static IPatternValuesSource CreateValuesSourceFromChart(ChartControl chart) {
			Series tempSeries = null;
			if (chart.Diagram != null && chart.Diagram.Series.Count > 0)
				tempSeries = chart.Diagram.Series[0];
			else if (chart.Diagram != null)
				tempSeries = chart.Diagram.SeriesTemplate.CreateCopyForBinding();
			else
				tempSeries = new BarSideBySideSeries2D();
			return new PatternEditorValuesSource(GetPointArgument(tempSeries), GetPointValue(tempSeries));
		}
		static object GetPointArgument(Series series) {
			object argument = null;
			switch (series.ActualArgumentScaleType) {
				case ScaleType.Numerical:
					argument = 12.3456789;
					break;
				case ScaleType.DateTime:
					argument = DateTime.Now;
					break;
				case ScaleType.Qualitative:
					if (series.Points.Count > 0)
						argument = series.Points[0].Argument;
					else
						argument = "A";
					break;
				default:
					argument = null;
					break;
			}
			return argument;
		}
		static object GetPointValue(Series series) {
			object value = null;
			switch (series.ValueScaleType) {
				case ScaleType.Numerical:
					value = 12.3456789;
					break;
				case ScaleType.DateTime:
					value = DateTime.Now;
					break;
				default:
					value = null;
					break;
			}
			return value;
		}
		public static string GetDescriptionForPatternPlaceholder(string pattern) {
			switch (pattern) {
				case ToolTipPatternUtils.ArgumentPattern: return ChartLocalizer.GetString(ChartStringId.ArgumentPatternDescription);
				case ToolTipPatternUtils.ValuePattern: return ChartLocalizer.GetString(ChartStringId.ValuePatternDescription);
				case ToolTipPatternUtils.SeriesNamePattern: return ChartLocalizer.GetString(ChartStringId.SeriesNamePatternDescription);
				case ToolTipPatternUtils.StackedGroupPattern: return ChartLocalizer.GetString(ChartStringId.StackedGroupPatternDescription);
				case ToolTipPatternUtils.Value1Pattern: return ChartLocalizer.GetString(ChartStringId.Value1PatternDescription);
				case ToolTipPatternUtils.Value2Pattern: return ChartLocalizer.GetString(ChartStringId.Value2PatternDescription);
				case ToolTipPatternUtils.WeightPattern: return ChartLocalizer.GetString(ChartStringId.WeightPatternDescription);
				case ToolTipPatternUtils.HighValuePattern: return ChartLocalizer.GetString(ChartStringId.HighValuePatternDescription);
				case ToolTipPatternUtils.LowValuePattern: return ChartLocalizer.GetString(ChartStringId.LowValuePatternDescription);
				case ToolTipPatternUtils.OpenValuePattern: return ChartLocalizer.GetString(ChartStringId.OpenValuePatternDescription);
				case ToolTipPatternUtils.CloseValuePattern: return ChartLocalizer.GetString(ChartStringId.CloseValuePatternDescription);
				case ToolTipPatternUtils.PercentValuePattern: return ChartLocalizer.GetString(ChartStringId.PercentValuePatternDescription);
				case ToolTipPatternUtils.PointHintPattern: return ChartLocalizer.GetString(ChartStringId.PointHintPatternDescription);
				case ToolTipPatternUtils.ValueDurationPattern: return ChartLocalizer.GetString(ChartStringId.ValueDurationPatternDescription);
				default: return "Unknown pattern format!";
			}
		}
		public static string GetPatternPlaceholderForDescription(string patternDescription) {
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.ArgumentPatternDescription))
				return ToolTipPatternUtils.ArgumentPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.ValuePatternDescription))
				return ToolTipPatternUtils.ValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.SeriesNamePatternDescription))
				return ToolTipPatternUtils.SeriesNamePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.StackedGroupPatternDescription))
				return ToolTipPatternUtils.StackedGroupPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.Value1PatternDescription))
				return ToolTipPatternUtils.Value1Pattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.Value2PatternDescription))
				return ToolTipPatternUtils.Value2Pattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.WeightPatternDescription))
				return ToolTipPatternUtils.WeightPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.HighValuePatternDescription))
				return ToolTipPatternUtils.HighValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.LowValuePatternDescription))
				return ToolTipPatternUtils.LowValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.OpenValuePatternDescription))
				return ToolTipPatternUtils.OpenValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.CloseValuePatternDescription))
				return ToolTipPatternUtils.CloseValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.PercentValuePatternDescription))
				return ToolTipPatternUtils.PercentValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.PointHintPatternDescription))
				return ToolTipPatternUtils.PointHintPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.ValueDurationPatternDescription))
				return ToolTipPatternUtils.ValueDurationPattern;
			return "";
		}
		public static string[] GetAvailablePlaceholders(object element, string propertyPath) {
			if (element is Series)
				switch (propertyPath) {
					case "ToolTipPointPattern":
						return GetAvailableToolTipPlaceholdersForPoint((Series)element);
					case "ToolTipSeriesPattern":
						return GetAvailableToolTipPlaceholdersForSeries((Series)element);
					default:
						return GetAvailablePlaceholdersForPoint((Series)element);
				}
			if (element is ChartControl)
				return GetAvailablePlaceholdersForCrosshairGroupHeader(((ChartControl)element).ActualCrosshairOptions);
			if (element is Axis)
				return GetAvailablePlaceholdersForAxis((Axis)element);
			return new string[0];
		}
		public static string[] GetAvailablePlaceholdersForPoint(Series series) {
			return series != null ? series.GetAvailablePointPatternPlaceholders() : new string[0];
		}
		public static string[] GetAvailableToolTipPlaceholdersForPoint(Series series) {
			if (series != null) {
				string[] basePatterns = series.GetAvailablePointPatternPlaceholders();
				string[] toolTipPaterns = new string[basePatterns.Length + 1];
				basePatterns.CopyTo(toolTipPaterns, 0);
				toolTipPaterns[basePatterns.Length] = ToolTipPatternUtils.PointHintPattern;
				return toolTipPaterns;
			}
			else
				return new string[0];
		}
		public static string[] GetAvailableToolTipPlaceholdersForSeries(Series series) {
			return series != null ? series.GetAvailableSeriesPatternPlaceholders() : new string[0];
		}
		public static string[] GetAvailablePlaceholdersForCrosshairGroupHeader(CrosshairOptions crosshairOptions) {
			return crosshairOptions.GetAvailablePatternPlaceholders();
		}
		public static string[] GetAvailablePlaceholdersForAxis(Axis axis) {
			return axis.GetAvailablePatternPlaceholders();
		}
		public static IPatternValuesSource CreatePatternValuesSource(object instance) {
			if (instance is Series)
				return CreateValuesSourceFromSeries((Series)instance);
			if (instance is ChartControl)
				return CreateValuesSourceFromChart((ChartControl)instance);
			if (instance is Axis)
				return CreateValuesSourceFromAxis((Axis)instance);
			return null;
		}
		public static string GetPatternText(string pattern, IPatternValuesSource valuesSource) {
			IPatternHolder patternHolder = valuesSource as IPatternHolder;
			PatternParser patternParser = new PatternParser(pattern, patternHolder);
			patternParser.SetContext(valuesSource);
			return patternParser.GetText();
		}
		public static string GetActualPattern(ChartElement element) {
			if (element is Series)
				return ((Series)element).ActualLegendTextPattern;
			if (element is SeriesLabel)
				return ((SeriesLabel)element).Series.ActualLabelTextPattern;
			if (element is AxisLabel)
				return ((AxisLabel)element).ActualTextPattern;
			return string.Empty;
		}
	}
}
