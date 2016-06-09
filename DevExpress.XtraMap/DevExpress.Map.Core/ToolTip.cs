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
namespace DevExpress.Map.Native {
	public class ToolTipPatternParser {
		class PatternFragment {
			readonly string fragment;
			readonly string patternName;
			readonly string format;
			object context = null;
			public string Fragment { get { return fragment; } }
			public object Context {
				get { return context; }
				set {
					if (string.IsNullOrEmpty(patternName))
						return;
					if (IsValidContext(value as IMapItemAttribute))
						context = value;
				}
			}
			public PatternFragment(string fragment, string patternName, string format) {
				this.fragment = fragment;
				this.patternName = patternName;
				this.format = format;
			}
			protected bool IsValidContext(IMapItemAttribute attr) {
				return attr != null && attr.Name == patternName;
			}
			string Format(object value, string format) {
				IFormattable formattable = value as IFormattable;
				if (formattable != null) {
					try {
						return formattable.ToString(format, CultureInfo.CurrentCulture);
					}
					catch {
					}
				}
				return fragment;
			}
			public string GetText() {
				IMapItemAttribute attr = Context as IMapItemAttribute;
				if (attr == null || attr.Value == null)
					return fragment;
				object value = attr.Value;
				if (string.IsNullOrEmpty(format)) {
					object[] array = value as object[];
					return array == null || array.Length == 0 ? value.ToString() : array[0].ToString();
				}
				return Format(value, format);
			}
		}
		string pattern;
		readonly List<PatternFragment> fragments = new List<PatternFragment>();
		protected string Pattern {
			get { return pattern; }
			set { pattern = value; }
		}
		public ToolTipPatternParser(string pattern) {
			this.pattern = pattern;
			Parse();
		}
		void Parse() {
			List<string> parsedPattern = SplitString(pattern, '{', '}');
			if (parsedPattern != null)
				foreach (string fragment in parsedPattern) {
					string patternName;
					string format;
					if (PrepareFragment(fragment, out patternName, out format))
						fragments.Add(new PatternFragment(fragment, patternName, format));
				}
		}
		bool PrepareFragment(string fragment, out string patternName, out string format) {
			format = null;
			patternName = null;
			if (!(fragment.StartsWith("{") && fragment.EndsWith("}")))
				return false;
			string patternBody = fragment.Substring(1, fragment.Length - 2);
			int formatIndex = patternBody.IndexOf(":");
			if (formatIndex >= 0) {
				format = patternBody.Substring(formatIndex + 1).Trim();
				patternName = patternBody.Substring(0, formatIndex);
			}
			else
				patternName = patternBody;
			return true;
		}
		List<string> SplitString(string splitingString, char leftSeparator, char rightSeparator) {
			List<string> substrings = new List<string>();
			int leftStringIndex = 0;
			int rightStringIndex = 0;
			int currentIndex = 0;
			if (!string.IsNullOrEmpty(splitingString)) {
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
		public virtual void AddContextRange(IEnumerable<object> contextList) {
			foreach (object item in contextList)
				AddContext(item);
		}
		public void AddContext(object context) {
			foreach (PatternFragment fragment in fragments)
				fragment.Context = context;
		}
		public virtual string GetText() {
			string result = pattern;
			foreach (PatternFragment fragment in fragments)
				result = result.Replace(fragment.Fragment, fragment.GetText());
			return result;
		}
	}
	public abstract class ChartTooltipPatternParser : ToolTipPatternParser {
		public const string AttrBeginDelimiter = "{";
		public const string AttrEndDelimiter = "}";
		public const string ChartAttrBeginDelimiter = "%";
		public const string ChartAttrEndDelimiter = "%";
		public const string ChartArgumentPattern = "A";
		public const string ChartValuePattern = "V";
		object chartItem;
		object chartValue;
		protected abstract bool NeedScanSegments { get; }
		protected object ChartItem { get { return chartItem; } }
		protected object ChartValue { get { return chartValue; } }
		protected ChartTooltipPatternParser(string pattern, object chartItem, object chartValue)
			: base(pattern) {
			this.chartItem = chartItem == null ? "" : chartItem;
			this.chartValue = chartValue == null ? "" : chartValue;
			AddChartProperties();
		}
		protected void ApplyFormattedValue(string beginDelimiter, string endDelimiter, string name, object value) {
			string formattedValuePattern = string.Format("{0}{1}:", beginDelimiter, name);
			int formattedValueBeginIndex = Pattern.IndexOf(formattedValuePattern);
			while (formattedValueBeginIndex >= 0) {
				int formatBeginIndex = formattedValueBeginIndex + formattedValuePattern.Length;
				string formatString = Pattern.Substring(formatBeginIndex);
				int formatLength = formatString.IndexOf(endDelimiter);
				formatString = Pattern.Substring(formatBeginIndex, formatLength);
				string replacedPattern = formattedValuePattern + formatString + endDelimiter;
				int stringOffset = replacedPattern.Length;
				IFormattable formattable = value as IFormattable;
				if (formattable != null) {
					try {
						string formattedValue = formattable.ToString(formatString, CultureInfo.CurrentCulture);
						stringOffset = formattedValue.Length;
						Pattern = Pattern.Replace(replacedPattern, formattedValue);
					}
					catch {
					}
				}
				int startIndex = formattedValueBeginIndex + stringOffset;
				if (startIndex > Pattern.Length)
					break;
				formattedValueBeginIndex = Pattern.IndexOf(formattedValuePattern, startIndex);
			}
		}
		void ApplyChartProperties() {
			Pattern = Pattern.Replace(string.Format("{0}{1}{2}", ChartAttrBeginDelimiter, ChartArgumentPattern, ChartAttrEndDelimiter), ChartItem.ToString());
			Pattern = Pattern.Replace(string.Format("{0}{1}{2}", ChartAttrBeginDelimiter, ChartValuePattern, ChartAttrEndDelimiter), ChartValue.ToString());
		}
		void ApplyFormattedChartProperties() {
			ApplyFormattedValue(ChartAttrBeginDelimiter, ChartAttrEndDelimiter, ChartArgumentPattern, ChartItem);
			ApplyFormattedValue(ChartAttrBeginDelimiter, ChartAttrEndDelimiter, ChartValuePattern, ChartValue);
		}
		protected void AddChartProperties() {
			ApplyChartProperties();
			ApplyFormattedChartProperties();
		}
		public override void AddContextRange(IEnumerable<object> contextList) {
			foreach (object context in contextList) {
				IMapItemAttribute attribute = context as IMapItemAttribute;
				if (attribute != null) {
					ApplyAttributeValue(attribute);
					ApplyFormattedAttributeValue(attribute);
				}
			}
		}
		void ApplyAttributeValue(IMapItemAttribute attribute) {
			object[] array = attribute.Value as object[];
			Pattern = Pattern.Replace(string.Format("{0}{1}{2}", AttrBeginDelimiter, attribute.Name, AttrEndDelimiter), array == null || array.Length == 0 ? attribute.Value.ToString() : array[0].ToString());
		}
		void ApplyFormattedAttributeValue(IMapItemAttribute attribute) {
			ApplyFormattedValue(AttrBeginDelimiter, AttrEndDelimiter, attribute.Name, attribute.Value);
		}
		public override string GetText() {
			return Pattern;
		}
	}
	public class BubbleToolTipPatternParser : ChartTooltipPatternParser {
		protected override bool NeedScanSegments { get { return false; } }
		public BubbleToolTipPatternParser(string pattern, object bubbleItem, object bubbleValue)
			: base(pattern, bubbleItem, bubbleValue) {
		}
	}
	public class PieToolTipPatternParser : ChartTooltipPatternParser {
		public const string SegmentArgumentPattern = "A";
		public const string SegmentValuePattern = "V";
		public const string SegmentPercentPattern = "VP";
		protected override bool NeedScanSegments { get { return true; } }
		IList<object> segmentArguments;
		IList<double> segmentValues;
		double totalValue;
		public PieToolTipPatternParser(string pattern, object chartItem, object chartValue, IList<object> segmentArguments, IList<double> segmentValues)
			: base(pattern, chartItem, chartValue) {
			this.segmentArguments = segmentArguments;
			this.segmentValues = segmentValues;
			if (segmentArguments.Count != segmentValues.Count)
				throw new ArgumentException("Arguments' and values' collections can't have different lengths");
			this.totalValue = GetTotalValue();
			AddSegmentProperties();
		}
		double GetTotalValue() {
			double sum = 0;
			foreach (double value in segmentValues)
				sum += value;
			return sum;
		}
		void ApplySegmentProperties(object argument, double value, int index) {
			Pattern = Pattern.Replace(string.Format("{0}{1}{2}{3}", ChartAttrBeginDelimiter, SegmentArgumentPattern, index, ChartAttrEndDelimiter), argument.ToString());
			Pattern = Pattern.Replace(string.Format("{0}{1}{2}{3}", ChartAttrBeginDelimiter, SegmentValuePattern, index, ChartAttrEndDelimiter), value.ToString());
			Pattern = Pattern.Replace(string.Format("{0}{1}{2}{3}", ChartAttrBeginDelimiter, SegmentPercentPattern, index, ChartAttrEndDelimiter), (value / totalValue).ToString());
		}
		void ApplyFormattedSegmentProperties(object argument, double value, int index) {
			ApplyFormattedValue(ChartAttrBeginDelimiter, ChartAttrEndDelimiter, string.Format("{0}{1}", SegmentArgumentPattern, index), argument);
			ApplyFormattedValue(ChartAttrBeginDelimiter, ChartAttrEndDelimiter, string.Format("{0}{1}", SegmentValuePattern, index), value);
			ApplyFormattedValue(ChartAttrBeginDelimiter, ChartAttrEndDelimiter, string.Format("{0}{1}", SegmentPercentPattern, index), value / totalValue);
		}
		void AddSegmentProperties() {
			for (int i = 0; i < segmentArguments.Count; i++) {
				ApplySegmentProperties(segmentArguments[i], segmentValues[i], i);
				ApplyFormattedSegmentProperties(segmentArguments[i], segmentValues[i], i);
			}
		}
	}
}
