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
namespace DevExpress.Charts.Native {
	public enum PatternConstants {
		Argument,
		Value,
		PercentValue,
		Series,
		SeriesGroup,
		Value1,
		Value2,
		HighValue,
		LowValue,
		OpenValue,
		CloseValue,
		PointHint,
		Weight,
		ValueDuration
	}
	public interface IPatternHolder {
		PatternDataProvider GetDataProvider(PatternConstants patternConstant);
		string PointPattern { get; } 
	}
	public abstract class PatternDataProvider {
		object context = null;
		public object Context {
			get { return context; }
			set {
				if (context != value)
					context = value;
			}
		}
		public abstract bool CheckContext(object value);
		protected abstract object GetValue();
		public string GetText(string format) {
			object value = GetValue();
			if (value != null) {
				if (!string.IsNullOrEmpty(format))
					return PatternUtils.Format(value, format);
				return value.ToString();
			}
			return string.Empty;
		}
	}
	public class SeriesPatternDataProvider : PatternDataProvider {
		readonly PatternConstants patternConstant;
		ISeries Series { get { return (ISeries)Context; } }
		public SeriesPatternDataProvider(PatternConstants patternConstant) {
			this.patternConstant = patternConstant;
		}
		public override bool CheckContext(object value) {
			return value is ISeries;
		}
		protected override object GetValue() {
			if (Context != null) {
				if (patternConstant == PatternConstants.SeriesGroup) {
					ISupportSeriesGroups groupView = Series.SeriesView as ISupportSeriesGroups;
					return groupView != null ? groupView.SeriesGroup : null;
				}
				return Series.Name;
			}
			return null;
		}
	}
	public class SimplePointPatternDataProvider : PatternDataProvider {
		readonly PatternConstants patternConstant;
		protected RefinedPoint RefinedPoint { get { return Context as RefinedPoint; } }
		public override bool CheckContext(object value) {
			return value is RefinedPoint;
		}
		public SimplePointPatternDataProvider(PatternConstants patternConstant) {
			this.patternConstant = patternConstant;
		}
		object GetValueFormRefinedPoint() {
			object value = null;
			if (Context != null) {
				switch (patternConstant) {
					case PatternConstants.Argument:
						value = RefinedPoint.SeriesPoint.UserArgument;
						break;
					case PatternConstants.Value:
						value = RefinedPoint.SeriesPoint.UserValues[0];
						break;
					case PatternConstants.PercentValue:
						value = GetPercentValue();
						break;
					case PatternConstants.PointHint:
						value = RefinedPoint.SeriesPoint.ToolTipHint;
						break;
				}
			}
			return value;
		}
		protected virtual object GetPercentValue() {
			return ((IPiePoint)RefinedPoint).NormalizedValue;
		}
		protected override object GetValue() {
			if (RefinedPoint != null)
				return GetValueFormRefinedPoint();
			return string.Empty;
		}
	}
	public class FunnelPointPatternDataProvider : SimplePointPatternDataProvider {
		public FunnelPointPatternDataProvider(PatternConstants patternConstant) : base(patternConstant) {
		}
		protected override object GetPercentValue() {
			double value = ((IFunnelPoint)RefinedPoint).NormalizedValue;
			return double.IsNaN(value) ? 0 : value;
		}
	}
	public class PointPatternDataProvider : PatternDataProvider {
		readonly PatternConstants patternConstant;
		AxisScaleTypeMap AxisScaleTypeMap;
		RefinedPoint RefinedPoint { get { return Context as RefinedPoint; } }
		ISeriesPoint SeriesPoint { get { return Context as ISeriesPoint; } }
		public override bool CheckContext(object value) {
			return value is RefinedPoint || value is ISeriesPoint;
		}
		public PointPatternDataProvider(PatternConstants patternConstant, AxisScaleTypeMap axisScaleTypeMap) {
			this.patternConstant = patternConstant;
			this.AxisScaleTypeMap = axisScaleTypeMap;
		}
		object GetValueFormSeriesPoint() {
			if (Context != null)
				switch (patternConstant) {
					case PatternConstants.Argument:
						return SeriesPoint.UserArgument;
					case PatternConstants.Value:
						return SeriesPoint.UserValues.Length > 0 ? SeriesPoint.UserValues[0] : 0.0;
					case PatternConstants.HighValue:
						return SeriesPoint.UserValues.Length > 1 ? SeriesPoint.UserValues[1] : 0.0;
					case PatternConstants.LowValue:
						return SeriesPoint.UserValues.Length > 0 ? SeriesPoint.UserValues[0] : 0.0;
					case PatternConstants.OpenValue:
						return SeriesPoint.UserValues.Length > 2 ? SeriesPoint.UserValues[2] : 0.0;
					case PatternConstants.CloseValue:
						return SeriesPoint.UserValues.Length > 3 ? SeriesPoint.UserValues[3] : 0.0;
					case PatternConstants.Value1:
						return SeriesPoint.UserValues.Length > 0 ? SeriesPoint.UserValues[0] : 0.0;
					case PatternConstants.Value2:
						return SeriesPoint.UserValues.Length > 1 ? SeriesPoint.UserValues[1] : 0.0;
					case PatternConstants.Weight:
						return SeriesPoint.UserValues.Length > 1 ? SeriesPoint.UserValues[1] : 0.0;
					case PatternConstants.PointHint:
						return SeriesPoint.ToolTipHint;
				}
			return null;
		}
		object GetValueFormRefinedPoint() {
			if (Context != null) {
				double internalValue = double.NaN;
				switch (patternConstant) {
					case PatternConstants.Argument:
						internalValue = RefinedPoint.Argument;
						break;
					case PatternConstants.Value:
						internalValue = ((IValuePoint)RefinedPoint).Value;
						break;
					case PatternConstants.PercentValue:
						internalValue = ((IFullStackedPoint)RefinedPoint).NormalizedValue;
						break;
					case PatternConstants.HighValue:
						internalValue = ((IFinancialPoint)RefinedPoint).High;
						break;
					case PatternConstants.LowValue:
						internalValue = ((IFinancialPoint)RefinedPoint).Low;
						break;
					case PatternConstants.OpenValue:
						internalValue = ((IFinancialPoint)RefinedPoint).Open;
						break;
					case PatternConstants.CloseValue:
						internalValue = ((IFinancialPoint)RefinedPoint).Close;
						break;
					case PatternConstants.Value1:
						internalValue = RefinedPoint.Value1;
						break;
					case PatternConstants.Value2:
						internalValue = RefinedPoint.Value2;
						break;
					case PatternConstants.Weight:
						internalValue = ((IXYWPoint)RefinedPoint).Weight;
						break;
					case PatternConstants.PointHint:
						return RefinedPoint.SeriesPoint.ToolTipHint;
					case PatternConstants.ValueDuration:
						return GetDurationValue(RefinedPoint);
				}
				if (double.IsNaN(internalValue) || AxisScaleTypeMap == null)
					return null;
				return AxisScaleTypeMap.InternalToNative(internalValue);
			}
			return null;
		}
		object GetDurationValue(RefinedPoint point) {
			if (double.IsNaN(RefinedPoint.Value1) || double.IsNaN(RefinedPoint.Value2) || AxisScaleTypeMap == null)
				return null;
			object value1 = AxisScaleTypeMap.InternalToNative(RefinedPoint.Value1);
			object value2 = AxisScaleTypeMap.InternalToNative(RefinedPoint.Value2);
			if (value1 is double && value2 is double)
				return Math.Abs((double)value2 - (double)value1);
			else if (value1 is DateTime && value2 is DateTime)
				return ((DateTime)value2 - (DateTime)value1).Duration();
			else
				throw new ArgumentException();
		}
		protected override object GetValue() {
			if (RefinedPoint != null)
				return GetValueFormRefinedPoint();
			if (SeriesPoint != null)
				return GetValueFormSeriesPoint();
			return string.Empty;
		}
	}
	public class ValuesSourcePatternDataProvider : PatternDataProvider {
		readonly PatternConstants patternConstant;
		public ValuesSourcePatternDataProvider(PatternConstants patternConstant) {
			this.patternConstant = patternConstant;
		}
		protected override object GetValue() {
			IPatternValuesSource valuesSource = Context as IPatternValuesSource;
			if (valuesSource == null)
				return null;
			switch (patternConstant) {
				case PatternConstants.Argument:
					return valuesSource.Argument;
				case PatternConstants.Value:
					return valuesSource.Value;
				case PatternConstants.PercentValue:
					return valuesSource.PercentValue;
				case PatternConstants.HighValue:
					return valuesSource.HighValue;
				case PatternConstants.LowValue:
					return valuesSource.LowValue;
				case PatternConstants.OpenValue:
					return valuesSource.OpenValue;
				case PatternConstants.CloseValue:
					return valuesSource.CloseValue;
				case PatternConstants.Value1:
					return valuesSource.Value1;
				case PatternConstants.Value2:
					return valuesSource.Value2;
				case PatternConstants.ValueDuration:
					return valuesSource.ValueDuration;
				case PatternConstants.Weight:
					return valuesSource.Weight;
				case PatternConstants.PointHint:
					return valuesSource.PointHint;
				case PatternConstants.Series:
					return valuesSource.Series;
				case PatternConstants.SeriesGroup:
					return valuesSource.SeriesGroup;
				default: return string.Empty;
			}
		}
		public override bool CheckContext(object value) {
			return value is IPatternValuesSource;
		}
	}
	public class AxisPatternDataProvider : PatternDataProvider {
		public override bool CheckContext(object value) {
			return true;
		}
		public AxisPatternDataProvider() {
		}
		protected override object GetValue() {
			return Context;
		}
	}
	public class RangeColorizerLegendItemDataProvider : PatternDataProvider {
		readonly PatternConstants patternConstants;
		public override bool CheckContext(object value) {
			return value is double[] && ((double[])value).Length > 1;
		}
		public RangeColorizerLegendItemDataProvider(PatternConstants patternConstants) {
			this.patternConstants = patternConstants;
		}
		protected override object GetValue() {
			int index = patternConstants == PatternConstants.Value1 ? 0 : 1;
			return ((double[])Context)[index];
		}
	}
	public class KeyColorColorizerLegendItemDataProvider : PatternDataProvider {
		public override bool CheckContext(object value) {
			return true;
		}
		public KeyColorColorizerLegendItemDataProvider() {
		}
		protected override object GetValue() {
			return Context;
		}
	}
	public class PatternParser {
		public static string FullStackedToolTipPattern(string argumentFormat, string valueFonmat) {
			string argumentPattern = "{A" + argumentFormat + "}";
			string valuePattern = "{V" + valueFonmat + "} ({VP:P2})";
			return argumentPattern + " : " + valuePattern;
		}
		class PatternFragment {
			readonly string fragment;
			readonly string format;
			readonly PatternDataProvider dataProvider;
			public string Fragment { get { return fragment; } }
			public PatternFragment(string fragment, string format, PatternDataProvider dataProvider) {
				this.fragment = fragment;
				this.format = format;
				this.dataProvider = dataProvider;
			}
			public string GetText() {
				return dataProvider.GetText(format);
			}
			public void SetContext(object context) {
				if (dataProvider.CheckContext(context))
					dataProvider.Context = context;
			}
		}
		static Dictionary<string, PatternConstants> constants;
		static PatternParser() {
			constants = new Dictionary<string, PatternConstants>();
			constants.Add("A", PatternConstants.Argument);
			constants.Add("V", PatternConstants.Value);
			constants.Add("VP", PatternConstants.PercentValue);
			constants.Add("S", PatternConstants.Series);
			constants.Add("G", PatternConstants.SeriesGroup);
			constants.Add("W", PatternConstants.Weight);
			constants.Add("V1", PatternConstants.Value1);
			constants.Add("V2", PatternConstants.Value2);
			constants.Add("HV", PatternConstants.HighValue);
			constants.Add("LV", PatternConstants.LowValue);
			constants.Add("OV", PatternConstants.OpenValue);
			constants.Add("CV", PatternConstants.CloseValue);
			constants.Add("HINT", PatternConstants.PointHint);
			constants.Add("VD", PatternConstants.ValueDuration);
		}
		readonly string template;
		readonly IPatternHolder patternHolder;
		readonly List<PatternFragment> fragments = new List<PatternFragment>();
		public PatternParser(string pattern, IPatternHolder patternHolder) {
			this.template = pattern;
			this.patternHolder = patternHolder;
			Parse();
		}
		void Parse() {
			List<string> parsedPattern = SplitString(template, '{', '}');
			if (parsedPattern != null)
				foreach (string fragment in parsedPattern) {
					string format;
					PatternConstants? patternConstant;
					if (PrepareFragment(fragment, out patternConstant, out format)) {
						PatternDataProvider dataProvider = patternHolder.GetDataProvider(patternConstant.Value);
						if (dataProvider != null)
							fragments.Add(new PatternFragment(fragment, format, dataProvider));
					}
				}
		}
		bool PrepareFragment(string fragment, out PatternConstants? patternConstant, out string format) {
			patternConstant = null;
			format = string.Empty;
			if (!(fragment.StartsWith("{") && fragment.EndsWith("}")))
				return false;
			string pattern = fragment.Substring(1, fragment.Length - 2);
			int formatIndex = pattern.IndexOf(":");
			if (formatIndex >= 0) {
				format = pattern.Substring(formatIndex + 1).Trim();
				pattern = pattern.Substring(0, formatIndex);
			}
			pattern = pattern.Trim().ToUpper();
			if (!constants.ContainsKey(pattern))
				return false;
			patternConstant = constants[pattern];
			return true;
		}
		List<string> SplitString(string splitingString, char leftSeparator, char rightSeparator) {
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
		public void SetContext(params object[] contexts) {
			foreach (PatternFragment fragment in fragments)
				foreach (var context in contexts) 
					fragment.SetContext(context);
		}
		public string GetText() {
			string result = template;
			foreach (PatternFragment fragment in fragments)
				result = result.Replace(fragment.Fragment, fragment.GetText());
			return result;
		}
	}
	public class PatternEditorValuesSource : IPatternValuesSource, IPatternHolder {
		const double doubleValue = 12.3456789;
		readonly TimeSpan timeSpanValue = TimeSpan.FromDays(45);
		object argument;
		object value;
		object valueDuration;
		string series;
		object seriesGroup;
		public PatternEditorValuesSource(object argument, object value) {
			this.argument = argument;
			this.value = value;
		}
		public PatternEditorValuesSource(object argument, object value, string series, object seriesGroup) {
			this.argument = argument;
			this.value = value;
			this.valueDuration = value is DateTime ? timeSpanValue : value;
			this.series = series;
			this.seriesGroup = seriesGroup;
		}
		public PatternEditorValuesSource(object axisValue) {
			this.argument = axisValue;
			this.value = axisValue;
		}
		#region IPatternValuesSource Members
		object IPatternValuesSource.Argument { get { return argument; } }
		object IPatternValuesSource.Value { get { return value; } }
		object IPatternValuesSource.Value1 { get { return value; } }
		object IPatternValuesSource.Value2 { get { return value; } }
		object IPatternValuesSource.ValueDuration { get { return valueDuration; } }
		double IPatternValuesSource.PercentValue { get { return doubleValue / 100.0; } }
		double IPatternValuesSource.Weight { get { return doubleValue; } }
		double IPatternValuesSource.LowValue { get { return doubleValue; } }
		double IPatternValuesSource.HighValue { get { return doubleValue; } }
		double IPatternValuesSource.OpenValue { get { return doubleValue; } }
		double IPatternValuesSource.CloseValue { get { return doubleValue; } }
		object IPatternValuesSource.PointHint { get { return "Point Hint"; } }
		string IPatternValuesSource.Series { get { return series; } }
		object IPatternValuesSource.SeriesGroup { get { return seriesGroup; } }
		#endregion
		#region IPatternHolder Members
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
			return new ValuesSourcePatternDataProvider(patternConstant);
		}
		string IPatternHolder.PointPattern {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
