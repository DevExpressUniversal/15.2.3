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
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public abstract class ExpressionConditionBaseInfo : FormatConditionBaseInfo {
		public override string OwnerPredefinedFormatsPropertyName { get { return "PredefinedFormats"; } }
		Format Format { get { return FormatCore as Format; } }
		public override ConditionalFormatMask FormatMask { get { return Format.FormatMask; } }
		public abstract bool CalcCondition(FormatValueProvider provider);
		public override Brush CoerceBackground(Brush value, FormatValueProvider provider) {
			return CoerceValue(value, Format.BackgroundProperty, provider);
		}
		public override TextDecorationCollection CoerceTextDecorations(TextDecorationCollection value, FormatValueProvider provider) {
			return CoerceValue(value, Format.TextDecorationsProperty, provider);
		}
		public override FontFamily CoerceFontFamily(FontFamily value, FormatValueProvider provider) {
			return CoerceValue(value, Format.FontFamilyProperty, provider);
		}
		public override double CoerceFontSize(double value, FormatValueProvider provider) {
			return CoerceValue(value, Format.FontSizeProperty, provider);
		}
		public override FontStretch CoerceFontStretch(FontStretch value, FormatValueProvider provider) {
			return CoerceValue(value, Format.FontStretchProperty, provider);
		}
		public override FontStyle CoerceFontStyle(FontStyle value, FormatValueProvider provider) {
			return CoerceValue(value, Format.FontStyleProperty, provider);
		}
		public override FontWeight CoerceFontWeight(FontWeight value, FormatValueProvider provider) {
			return CoerceValue(value, Format.FontWeightProperty, provider);
		}
		public override Brush CoerceForeground(Brush value, FormatValueProvider provider) {
			return CoerceValue(value, Format.ForegroundProperty, provider);
		}
		public override DataBarFormatInfo CoerceDataBarFormatInfo(DataBarFormatInfo value, FormatValueProvider provider) {
			return CoerceValue(value, Format.IconProperty, provider, (val, icon) => DataBarFormatInfo.AddIcon(val, (ImageSource)icon, Format.IconVerticalAlignment));
		}
		TValue CoerceValue<TValue>(TValue value, DependencyProperty property, FormatValueProvider provider, Func<TValue, object, TValue> valueCombiner = null) {
			if(CalcCondition(provider) && Format.IsPropertyAssigned(property)) {
				object propertyValue = Format.GetValue(property);
				value = valueCombiner != null ? valueCombiner(value, propertyValue) : (TValue)propertyValue;
			}
			return value;
		}
		public override IEnumerable<ConditionalFormatSummaryType> GetSummaries() {
			yield break;
		}
	}
	public class FormatConditionInfo : ExpressionConditionBaseInfo {
		public ConditionRule ValueRule { get; set; }
		public object Value1 { get; set; }
		public object Value2 { get; set; }
		public override bool CalcCondition(FormatValueProvider provider) {
			return ValueRule == ConditionRule.Expression ? FormatConditionBaseInfo.IsFit(provider.Value) : CalcRule(provider);
		}
		bool CalcRule(FormatValueProvider provider) {
			object value = provider.Value;
			object value1 = Convert(value, Value1);
			if(value1 == null && Value1 != null)
				return false;
			DevExpress.Data.ValueComparer comparer = provider.ValueComparer;
			int res1 = comparer.Compare(value, value1);
			switch(ValueRule) {
				case ConditionRule.None:
					return true;
				case ConditionRule.Equal:
					return res1 == 0;
				case ConditionRule.NotEqual:
					return res1 != 0;
				case ConditionRule.Less:
					return res1 < 0;
				case ConditionRule.Greater:
					return res1 > 0;
				case ConditionRule.GreaterOrEqual:
					return res1 >= 0;
				case ConditionRule.LessOrEqual:
					return res1 <= 0;
				case ConditionRule.Between:
				case ConditionRule.NotBetween:
					object value2 = Convert(value, Value2);
					if(value2 == null && Value2 != null)
						return false;
					int res2 = comparer.Compare(value, value2);
					return (ValueRule == ConditionRule.Between) == (res1 > 0 && res2 < 0);
			}
			return false;
		}
		object Convert(object target, object source) {
			if(target != null && source != null) {
				Type valType = target.GetType();
				if(!valType.Equals(source.GetType()))
					try {
						source = System.Convert.ChangeType(source, valType);
					}
					catch {
						source = null;
					}
			}
			return source;
		}
		protected override string ActualExpression { get { return ValueRule == ConditionRule.Expression ? base.ActualExpression : null; } }
	}
	public class TopBottomRuleFormatConditionInfo : ExpressionConditionBaseInfo {
		public TopBottomRule Rule { get; set; }
		public double Threshold { get; set; }
		public override bool CalcCondition(FormatValueProvider provider) {
			if(provider.Value == null)
				return false;
			object averageValue = provider.GetTotalSummaryValue(SummaryType);
			if(averageValue == null)
				return false;
			if(Rule == TopBottomRule.AboveAverage || Rule == TopBottomRule.BelowAverage) {
				decimal? value = IndicatorFormatBase.GetDecimalValue(provider.Value);
				decimal? decimalAverageValue = IndicatorFormatBase.GetDecimalValue(averageValue);
				if(value == null || decimalAverageValue == null)
					return false;
				return provider.ValueComparer.Compare(value, decimalAverageValue) * GetComparisonSign() > 0;
			}
			SortedIndices indices = averageValue as SortedIndices;
			if(indices == null)
				return false;
			int count = (int)(GetUseItemCount() ? Math.Min(int.MaxValue, Threshold) : Math.Max(1, Math.Floor(indices.Count * (Threshold / 100d))));
			return indices.IsTopBottomItem(provider, Math.Max(0, count), GetUseTopItems());
		}
		public override IEnumerable<ConditionalFormatSummaryType> GetSummaries() {
			yield return SummaryType;
		}
		bool GetUseItemCount() {
			switch(Rule) {
				case TopBottomRule.TopItems:
				case TopBottomRule.BottomItems:
					return true;
				case TopBottomRule.TopPercent:
				case TopBottomRule.BottomPercent:
					return false;
				default:
					throw new InvalidOperationException();
			}
		}
		bool GetUseTopItems() {
			switch(Rule) {
				case TopBottomRule.TopItems:
				case TopBottomRule.TopPercent:
					return true;
				case TopBottomRule.BottomItems:
				case TopBottomRule.BottomPercent:
					return false;
				default:
					throw new InvalidOperationException();
			}
		}
		int GetComparisonSign() {
			switch(Rule) {
				case TopBottomRule.AboveAverage:
					return 1;
				case TopBottomRule.BelowAverage:
					return -1;
				default:
					throw new InvalidOperationException();
			}
		}
		ConditionalFormatSummaryType SummaryType {
			get {
				switch(Rule) {
					case TopBottomRule.AboveAverage:
					case TopBottomRule.BelowAverage:
						return ConditionalFormatSummaryType.Average;
					case TopBottomRule.TopItems:
					case TopBottomRule.TopPercent:
					case TopBottomRule.BottomItems:
					case TopBottomRule.BottomPercent:
						return ConditionalFormatSummaryType.SortedList;
					default:
						throw new NotImplementedException();
				}
			}
		}
	}
}
