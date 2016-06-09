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
using System.Globalization;
using DevExpress.Data.PivotGrid;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public abstract class NumericFormatter : FormatterBase {
		const string nonBreakingSpace = "\u00a0";
		protected static string JoinWithNonBreakingSpace(params string[] strings) {
			return String.Join(nonBreakingSpace, strings);
		}
		protected static string EscapeSpecials(string input) {
			if(input.Contains("'"))
				input = input.Replace("'", "\\'");
			if(input.Contains(","))
				input = input.Replace(",", "\\,");
			if(input.Contains("."))
				input = input.Replace(".", "\\.");
			if(input.Contains(";"))
				input = input.Replace(";", "\\;");
			if(input.Contains("0"))
				input = input.Replace("0", "\\0");
			if(input.Contains("#"))
				input = input.Replace("#", "\\#");
			if(input.Contains("%"))
				input = input.Replace("%", "\\%");
			if(input.Contains("‰"))
				input = input.Replace("‰", "\\‰");
			return input;
		}
		public static NumericFormatter CreateInstance(NumericFormatViewModel formatViewModel) {
			return CreateInstance(Helper.CurrentCulture, formatViewModel);
		}
		public static NumericFormatter CreateInstance(CultureInfo clientCulture, NumericFormatViewModel formatViewModel) {
			switch (formatViewModel.FormatType) {
				case NumericFormatType.Number:
					return new NumericNumberFormatter(clientCulture, formatViewModel, formatViewModel.Unit != DataItemNumericUnit.Auto);
				case NumericFormatType.Currency:
					return new NumericCurrencyFormatter(clientCulture, formatViewModel, formatViewModel.Unit != DataItemNumericUnit.Auto);
				case NumericFormatType.Scientific:
					return new NumericScientificFormatter(clientCulture, formatViewModel);
				case NumericFormatType.Percent:
					return new NumericPercentFormatter(clientCulture, formatViewModel);
				default:
					return new NumericGeneralFormatter(clientCulture, formatViewModel);
			}
		}
		public static NumericFormatter CreateInstance(CultureInfo clientCulture, object minValue, object maxValue) {
			DataItemNumericUnit unit = CalculateUnit(minValue, maxValue);
			return new NumericNumberFormatter(clientCulture, new NumericFormatViewModel(NumericFormatType.Number, 2, unit, false, false, 0, String.Empty), false);
		}
		public static DataItemNumericUnit CalculateUnit(object minValue, object maxValue) {
			decimal range = Math.Abs(ConvertToDecimal(maxValue) - ConvertToDecimal(minValue));
			DataItemNumericUnit unit;
			if(range >= 1000000000)
				unit = DataItemNumericUnit.Billions;
			else if(range >= 1000000)
				unit = DataItemNumericUnit.Millions;
			else if(range >= 1000)
				unit = DataItemNumericUnit.Thousands;
			else
				unit = DataItemNumericUnit.Ones;
			return unit;
		}
		readonly CultureInfo clientCulture;
		readonly NumericFormatViewModel formatViewModel;
		protected NumericFormatViewModel FormatViewModel { get { return formatViewModel; } }
		protected NumberFormatInfo NumberFormatInfo { get { return clientCulture.NumberFormat; } }
		public override string FormatPattern { get { return GetGeneralFormatPattern(); } }
		protected NumericFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel) {
			this.clientCulture = clientCulture;
			this.formatViewModel = formatViewModel;
		}
		protected override string FormatInternal(object value) {
			string formatString = String.Format("{{0:{0}}}", GetFormatPattern(value));
			if(value != null && DataBindingHelper.IsTimeSpanType(value.GetType()))
				return string.Format(formatString, ((TimeSpan)value).Ticks);
			return string.Format(formatString, value);
		}
		public abstract string GetFormatPattern(object value);
		public abstract string GetGeneralFormatPattern();
		static decimal ConvertToDecimal(object value) {
			if(value != null && DataBindingHelper.IsTimeSpanType(value.GetType())) {
				return Convert.ToDecimal(((TimeSpan)value).Ticks);
			}
			if(!DashboardSpecialValuesInternal.IsDashboardSpecialValue(value)) {
				return Convert.ToDecimal(value);
			}
			return 0;
		}
	}
	public class NumericGeneralFormatter : NumericFormatter {
		public NumericGeneralFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel) : base(clientCulture, formatViewModel) {
		}
		public override string GetFormatPattern(object value) {
			return "G";
		}
		public override string GetGeneralFormatPattern() {
			return "G";
		}
	}
	public class NumericScientificFormatter : NumericFormatter {
		public override string FormatPattern { get { return FullFormatPattern; } }
		string FullFormatPattern { get { return String.Format("{0}{1}", "E", FormatViewModel.Precision); } }
		public NumericScientificFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel)
			: base(clientCulture, formatViewModel) {
		}
		public override string GetFormatPattern(object value) {
			return FullFormatPattern;
		}
		public override string GetGeneralFormatPattern() {
			return "E";
		}
	}
	public abstract class NumericCustomFormatter : NumericFormatter {
		protected virtual string Symbol { get { return string.Empty; } }
		protected virtual int Precision { get { return FormatViewModel.Precision; } }		
		protected abstract bool ShowTrailingZeros { get; }
		protected abstract string UnitSymbol { get; }
		protected abstract string UnitDivider { get; }
		public override string FormatPattern { get { return FullFormatPattern; } }
		string FullFormatPattern { get { return String.Format("{0};{1};{2}", GetPositiveValuePattern(), GetNegativeValuePattern(), GetZeroValuePattern()); } }
		protected NumericCustomFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel)
			: base(clientCulture, formatViewModel) {
		}
		string GetValueFormat() {
			string groupSeparatorSymbol = FormatViewModel.IncludeGroupSeparator ? "," : String.Empty;
			string unitDivider = UnitDivider;
			if(Precision == 0)
				return String.Format("#{0}0{1}", groupSeparatorSymbol, unitDivider);
			string fractionalSymbol = ShowTrailingZeros ? "0" : "#";
			string fractionalSymbols = string.Empty;
			int precisionCounter = Precision;
			while(precisionCounter != 0) {
				fractionalSymbols += fractionalSymbol;
				precisionCounter--;
			}
			return String.Format("#{0}0{1}.{2}", groupSeparatorSymbol, unitDivider, fractionalSymbols);
		}
		string GetPositiveValuePattern() {
			int patternNumber = GetPatternNumber(true);
			string pattern = GetPositivePattern(patternNumber);
			string sign = FormatViewModel.ForcePlusSign ? "+" : String.Empty;
			return String.Format(pattern, sign, GetValueFormat(), UnitSymbol, Symbol);
		}
		string GetNegativeValuePattern() {
			int patternNumber = GetPatternNumber(false);
			string pattern = GetNegativePattern(patternNumber);
			return String.Format(pattern, GetValueFormat(), UnitSymbol, Symbol);
		}
		string GetZeroValuePattern() {
			int patternNumber = GetPatternNumber(true);
			string pattern = GetPositivePattern(patternNumber);
			return String.Format(pattern, String.Empty, GetValueFormat(), UnitSymbol, Symbol);
		}
		protected abstract int GetPatternNumber(bool isPositive);
		protected virtual string GetPositivePattern(int patternNumber) {
			switch(patternNumber) {
				case 0:
					return "{0}{3}{1}{2}";
				case 1:
					return "{0}{1}{2}{3}";
				case 2:
					return  JoinWithNonBreakingSpace("{0}{3}", "{1}{2}");
				case 3:
					return JoinWithNonBreakingSpace("{0}{1}{2}", "{3}");
				default:
					return "{0}{1}{2}{3}";
			}
		}
		protected virtual string GetNegativePattern(int patternNumber) {
			switch(patternNumber) {
				case 0:
					return "({2}{0}{1})";
				case 1:
					return "-{2}{0}{1}";
				case 2:
					return "{2}-{0}{1}";
				case 3:
					return "{2}{0}{1}-";
				case 4:
					return "({0}{1}{2})";
				case 5:
					return "-{0}{1}{2}";
				case 6:
					return "{0}{1}-{2}";
				case 7:
					return "{0}{1}{2}-";
				case 8:
					return JoinWithNonBreakingSpace("-{0}{1}", "{2}");
				case 9:
					return JoinWithNonBreakingSpace("-{2}", "{0}{1}");
				case 10:
					return JoinWithNonBreakingSpace("{0}{1}", "{2}-");
				case 11:
					return JoinWithNonBreakingSpace("{2}", "{0}{1}-");
				case 12:
					return JoinWithNonBreakingSpace("{2}", "-{0}{1}");
				case 13:
					return JoinWithNonBreakingSpace("{0}{1}-", "{2}");
				case 14:
					return JoinWithNonBreakingSpace("({2}", "{0}{1})");
				case 15:
					return JoinWithNonBreakingSpace("({0}{1}", "{2})");
				default:
					return "-{0}{1}";
			}
		}
		public override string GetFormatPattern(object value) {
			return FullFormatPattern;
		}
	}
	public class NumericAxisPercentFormatter : NumericPercentFormatter {
		protected override bool ShowTrailingZeros { get { return false; } }
		static NumericFormatViewModel AxisFormatViewModel {
			get { return new NumericFormatViewModel() { FormatType = NumericFormatType.Percent, Precision = 2 }; }
		}
		public NumericAxisPercentFormatter() : base(Helper.CurrentCulture, AxisFormatViewModel) {
		}
	}
	public class NumericPercentFormatter : NumericCustomFormatter {
		protected override string Symbol { get { return NumberFormatInfo.PercentSymbol; } }
		protected override string UnitSymbol { get { return string.Empty; } }
		protected override string UnitDivider { get { return string.Empty; } }
		protected override bool ShowTrailingZeros { get { return true; } }
		public NumericPercentFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel) : base(clientCulture, formatViewModel) {
		}
		protected override int GetPatternNumber(bool isPositive) {
			return isPositive ? NumberFormatInfo.PercentPositivePattern : NumberFormatInfo.PercentNegativePattern;
		}
		protected override string GetPositivePattern(int patternNumber) {
			switch(patternNumber) {
				case 0:
					return JoinWithNonBreakingSpace("{0}{1}{2}", "{3}");
				case 1:
					return "{0}{1}{2}{3}";
				case 2:
					return "{0}{3}{1}{2}";
				case 3:
					return JoinWithNonBreakingSpace("{0}{3}", "{1}{2}");
				default:
					return "{1}{2}{3}{4}";
			}
		}
		protected override string GetNegativePattern(int patternNumber) {
			switch(patternNumber) {
				case 0:
					return JoinWithNonBreakingSpace("-{0}{1}", "{2}");
				case 1:
					return "-{0}{1}{2}";
				case 2:
					return "-{2}{0}{1}";
				case 3:
					return "{2}-{0}{1}";
				case 4:
					return "{2}{0}{1}-";
				case 5:
					return "{0}{1}-{2}";
				case 6:
					return "{0}{1}{2}-";
				case 7:
					return JoinWithNonBreakingSpace("-{2}", "{0}{1}");
				case 8:
					return JoinWithNonBreakingSpace("{0}{1}", "{2}-");
				case 9:
					return JoinWithNonBreakingSpace("{2}", "{0}{1}-");
				case 10:
					return JoinWithNonBreakingSpace("{2}", "-{0}{1}");
				case 11:
					return JoinWithNonBreakingSpace("{0}{1}-", "{2}");
				default:
					return "-{0}{1}";
			}
		}
		public override string GetGeneralFormatPattern() {
			return "P";
		}
	}
	public abstract class NumericCustomUnitFormatter : NumericCustomFormatter {
		string unitSymbol;
		string unitDevider;
		readonly bool showTrailingZeros;
		int precision;
		protected override string UnitSymbol { get { return unitSymbol; } }
		protected override string UnitDivider { get { return unitDevider; } }
		protected override bool ShowTrailingZeros { get { return showTrailingZeros; } }
		protected override int Precision { get { return precision; } }
		void GenerateUnitParameters(object value) {
			DataItemNumericUnit unit;
			if(FormatViewModel.Unit == DataItemNumericUnit.Auto) {
				double dValue;
				if(Object.Equals(PivotSummaryValue.ErrorValue, value) || value == null)
					dValue = 0;
				else if(DataBindingHelper.IsTimeSpanType(value.GetType()))
					dValue = ((TimeSpan)value).Ticks;
				else {
					try {
						dValue = Math.Abs(Convert.ToDouble(value));
					}
					catch(FormatException) {
						dValue = 0;
					}
					catch(OverflowException) {
						dValue = 0;
					}
				}
				if(dValue >= 1000000000) {
					unit = DataItemNumericUnit.Billions;
					dValue /= 1000000000;
				}
				else if(dValue >= 1000000) {
					unit = DataItemNumericUnit.Millions;
					dValue /= 1000000;
				}
				else if(dValue >= 1000) {
					unit = DataItemNumericUnit.Thousands;
					dValue /= 1000;
				}
				else
					unit = DataItemNumericUnit.Ones;
				if(dValue > 0 && dValue < 1) {
					precision = FormatViewModel.SignificantDigits;
					double smallValue = Convert.ToDouble(Math.Pow(10, -FormatViewModel.SignificantDigits));
					while(dValue < smallValue) {
						smallValue /= 10;
						precision++;
					}
				}
				else {
					if(dValue >= 100)
						precision = FormatViewModel.SignificantDigits - 3;
					else if(dValue >= 10)
						precision = FormatViewModel.SignificantDigits - 2;
					else
						precision = FormatViewModel.SignificantDigits - 1;
				}
			}
			else {
				unit = FormatViewModel.Unit;
				precision = FormatViewModel.Precision;
			}
			switch(unit) {
				case DataItemNumericUnit.Thousands:
					unitSymbol = EscapeSpecials(DashboardLocalizer.GetString(DashboardStringId.NumericFormatUnitSymbolThousands));
					unitDevider = ",";
					break;
				case DataItemNumericUnit.Millions:
					unitSymbol = EscapeSpecials(DashboardLocalizer.GetString(DashboardStringId.NumericFormatUnitSymbolMillions));
					unitDevider = ",,";
					break;
				case DataItemNumericUnit.Billions:
					unitSymbol = EscapeSpecials(DashboardLocalizer.GetString(DashboardStringId.NumericFormatUnitSymbolBillions));
					unitDevider = ",,,";
					break;
				default:
					unitSymbol = String.Empty;
					unitDevider = String.Empty;
					break;
			}
		}
		protected NumericCustomUnitFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel, bool showTrailingZeros) : base(clientCulture, formatViewModel) {
			this.showTrailingZeros = showTrailingZeros;
		}
		public override string GetFormatPattern(object value) {
			GenerateUnitParameters(value);
			return base.GetFormatPattern(value);
		}
		public override string GetGeneralFormatPattern() {
			return "G";
		}
	}
	public class NumericNumberFormatter : NumericCustomUnitFormatter {
		public NumericNumberFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel, bool showTrailingZeros) : base(clientCulture, formatViewModel, showTrailingZeros) {
		}
		protected override int GetPatternNumber(bool isPositive) {
			return -1;
		}
	}
	public class NumericCurrencyFormatter : NumericCustomUnitFormatter {
		readonly NumberFormatInfo currencyFormatInfo;
		protected override string Symbol { get { return EscapeSpecials(currencyFormatInfo.CurrencySymbol); } }
		public NumericCurrencyFormatter(CultureInfo clientCulture, NumericFormatViewModel formatViewModel, bool showTrailingZeros) : base(clientCulture, formatViewModel, showTrailingZeros) {
			string currencyCulture = formatViewModel.CurrencyCulture ?? clientCulture.Name;
			currencyFormatInfo = new CultureInfo(currencyCulture).NumberFormat;
		}
		protected override int GetPatternNumber(bool isPositive) {
			return isPositive ? currencyFormatInfo.CurrencyPositivePattern : currencyFormatInfo.CurrencyNegativePattern;
		}
		public override string GetGeneralFormatPattern() {
			return "C";
		}
	}
}
