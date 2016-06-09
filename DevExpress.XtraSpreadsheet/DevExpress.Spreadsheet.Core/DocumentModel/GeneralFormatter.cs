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
using System.Globalization;
using System.Diagnostics;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region NumericValue
	class NumericValue {
		public double Value { get; set; }
		public double AbsoluteValue { get; set; }
		public int PowerOfValue { get; set; }
		public int MaxCharWidth { get; set; }
		public int OriginalSignifcantDigits { get; set; }
	}
	#endregion
	#region GeneralFormatterState
	struct GeneralFormatterState {
		public int Power { get; set; }
		public int SignificantDigits { get; set; }
	}
	#endregion
	#region GeneralFormatter
	class GeneralFormatter {
		[ThreadStatic]
		StringBuilder digits;
		const int defaultFixedSignificantDigits = 10;
		const int defaultExpSignificantDigits = 6;
		string decimalSeparator;
		string negativeSign;
		string positiveSign;
		public GeneralFormatter(CultureInfo culture) {
			NumberFormatInfo numberFormat = culture.NumberFormat;
			this.decimalSeparator = numberFormat.NumberDecimalSeparator;
			this.negativeSign = numberFormat.NegativeSign;
			this.positiveSign = numberFormat.PositiveSign;
		}
		void EnsureBufferExists() {
			if (digits == null)
				digits = new StringBuilder(32);
			if (digits.Length > 0)
				digits.Length = 0;
		}
		public string Format(double value, int maxCharWidth) {
			NumericValue v = new NumericValue();
			v.Value = value;
			v.MaxCharWidth = maxCharWidth;
			if (maxCharWidth < 1)
				return String.Empty;
			int sign = Math.Sign(value);
			if (sign == 0)
				return "0";
			v.AbsoluteValue = Math.Abs(value);
			double integralPart = WorksheetFunctionBase.Truncate(v.AbsoluteValue);
			int fixedSignificantDigits = Math.Min(maxCharWidth, defaultFixedSignificantDigits);
			v.OriginalSignifcantDigits = (integralPart == v.AbsoluteValue) ? fixedSignificantDigits + 1 : fixedSignificantDigits; 
			v.PowerOfValue = (int)Math.Floor(Math.Log10(v.AbsoluteValue));
			GeneralFormatterState state = new GeneralFormatterState();
			for (; ; ) {
				state.SignificantDigits = v.OriginalSignifcantDigits;
				StringBuilder formattedAbsoluteValue = FormatCore(v, state);
				string result = GetFormattedResult(v.Value, formattedAbsoluteValue, v.MaxCharWidth);
				if (result.Length <= 0)
					return result;
				if (result[0] != '#')
					return result;
				v.OriginalSignifcantDigits--;
				if (v.OriginalSignifcantDigits <= 0)
					return result;
			}
		}
		StringBuilder FormatCore(NumericValue v, GeneralFormatterState state) {
			EnsureBufferExists();
			state.Power = v.PowerOfValue;
			double digitsValue;
			int pow = state.SignificantDigits - state.Power - 1;
			if (pow > 308) {
				digitsValue = v.AbsoluteValue * GetPowerValue(pow - 307);
				digitsValue = Math.Round(digitsValue * GetPowerValue(308), 0);
			}
			else
				digitsValue = Math.Round(v.AbsoluteValue * GetPowerValue(pow), 0);
			digits = GetIntegralDigits(digitsValue);
			if (digits.Length > state.SignificantDigits) { 
				digits.Remove(digits.Length - 1, 1);
				state.Power++;
			}
			if (ForceUseExponentialFormat(v, state))
				return GetExponential(v, state);
			if (state.Power >= 0)
				return GetPositivePowerGenericResult(v, state);
			digits = GetNegativePowerGenericResult(v, state);
			if (digits.Length <= (v.OriginalSignifcantDigits + 1))
				return digits;
			if (state.Power <= -5) 
				return GetExponential(v, state);
			else {
				state.SignificantDigits--;
				if (state.SignificantDigits == 0) {
					if (v.AbsoluteValue < 1) {
						digits.Length = 0;
						digits.Append('0');
					}
					return digits;
				}
				return FormatCore(v, state);
			}
		}
		StringBuilder GetPositivePowerGenericResult(NumericValue v, GeneralFormatterState state) {
			return InsertDecimalSeparatorAndDropInsignificantZeroes(digits, state.Power + 1);
		}
		StringBuilder GetNegativePowerGenericResult(NumericValue v, GeneralFormatterState state) {
			Debug.Assert(state.Power < 0);
			if (state.Power != -1) {
				string zeros = new string('0', Math.Abs(state.Power) - 1);
				digits.Insert(0, zeros);
			}
			digits.Insert(0, decimalSeparator);
#if !SL
			digits.Insert(0, '0');
#else
			digits.Insert(0, "0");
#endif
			TrimEnd(digits, '0');
			CutDecimalSeparator(digits);
			return digits;
		}
		bool ForceUseExponentialFormat(NumericValue v, GeneralFormatterState state) {
			if (Math.Abs(state.Power) <= 4) {
				if (state.SignificantDigits <= 4) {
					if (v.AbsoluteValue < 1) {
						if (v.Value > 0)
							if (v.MaxCharWidth == 5 && state.SignificantDigits == 1)
								return true;
						if (v.Value < 0)
							if (v.MaxCharWidth == 6 && state.SignificantDigits == 1)
								return true;
						return false;
					}
				}
			}
			return (state.Power > state.SignificantDigits - 1) || (state.Power < 1 - state.SignificantDigits);
		}
		string GetFormattedResult(double value, StringBuilder result, int maxCharWidth) {
			if (value < 0)
				result.Insert(0, negativeSign);
			if (result.Length <= maxCharWidth)
				return result.ToString();
			return GetPlaceHolder(maxCharWidth);
		}
		StringBuilder GetExponential(NumericValue v, GeneralFormatterState state) {
			double absoluteValue = v.AbsoluteValue;
			int power = v.PowerOfValue;
			int expSignificantDigits = Math.Min(state.SignificantDigits, defaultExpSignificantDigits);
			int exponentSignificantDigits = Math.Abs(power) >= 100 ? expSignificantDigits - 1 : expSignificantDigits;
			double mantissaValue;
			int pow = exponentSignificantDigits - power - 1;
			if (pow > 308) {
				mantissaValue = absoluteValue  * GetPowerValue(pow - 307);
				mantissaValue = Math.Round(mantissaValue * GetPowerValue(308), 0);
			}
			else
				mantissaValue = Math.Round(absoluteValue  * GetPowerValue(pow), 0);
			StringBuilder mantissaDigits = GetIntegralDigits(mantissaValue);
			if (mantissaDigits.Length > exponentSignificantDigits) 
				power++;
			StringBuilder result = InsertDecimalSeparatorAndDropInsignificantZeroes(mantissaDigits, 1);
			result.Append("E");
			result.Append((power > 0) ? positiveSign : negativeSign);
			result.Append(Math.Abs(power).ToString("00"));
			return result;
		}
		StringBuilder InsertDecimalSeparatorAndDropInsignificantZeroes(StringBuilder digits, int decimalSeparatorPosition) {
			int zeroesNeed = decimalSeparatorPosition - digits.Length;
			for (int i = 0; i < zeroesNeed; i++)
				digits.Append('0');
			digits.Insert(decimalSeparatorPosition, decimalSeparator);
			TrimEnd(digits, '0');
			CutDecimalSeparator(digits);
			return digits;
		}
		void TrimEnd(StringBuilder text, char ch) {
			for (int i = text.Length - 1; i >= 0; i--) {
				if (text[i] != ch) {
					int from = i + 1;
					int length = text.Length - from;
					if (length > 0)
						text.Remove(i + 1, length);
					break;
				}
			}
		}
		void CutDecimalSeparator(StringBuilder result) {
			if (result.Length <= 0)
				return;
			if (result[result.Length - 1] == decimalSeparator[0])
				result.Remove(result.Length - 1, 1);
		}
		string GetPlaceHolder(int maxCharWidth) {
			return new String('#', maxCharWidth);
		}
		double GetPowerValue(int power) {
			if (power < 0)
				return negativePowerTable[-power];
			else
				return positivePowerTable[power];
		}
		StringBuilder GetIntegralDigits(double integralPart) {
			EnsureBufferExists();
			Debug.Assert(digits.Length == 0);
			while (integralPart > 0.0) {
				int digit = (int)(integralPart % 10);
#if !SL
				digits.Insert(0, (char)('0' + digit));
#else
				digits.Insert(0, new String((char)('0' + digit), 1));
#endif
				integralPart = WorksheetFunctionBase.Truncate(integralPart / 10);
			}
			return digits;
		}
		#region positivePowerTable
		static readonly double[] positivePowerTable = new double[] {
			1e+00, 1e+01, 1e+02, 1e+03, 1e+04, 1e+05, 1e+06, 1e+07, 1e+08, 1e+09,
			1e+10, 1e+11, 1e+12, 1e+13, 1e+14, 1e+15, 1e+16, 1e+17, 1e+18, 1e+19,
			1e+20, 1e+21, 1e+22, 1e+23, 1e+24, 1e+25, 1e+26, 1e+27, 1e+28, 1e+29,
			1e+30, 1e+31, 1e+32, 1e+33, 1e+34, 1e+35, 1e+36, 1e+37, 1e+38, 1e+39,
			1e+40, 1e+41, 1e+42, 1e+43, 1e+44, 1e+45, 1e+46, 1e+47, 1e+48, 1e+49,
			1e+50, 1e+51, 1e+52, 1e+53, 1e+54, 1e+55, 1e+56, 1e+57, 1e+58, 1e+59,
			1e+60, 1e+61, 1e+62, 1e+63, 1e+64, 1e+65, 1e+66, 1e+67, 1e+68, 1e+69,
			1e+70, 1e+71, 1e+72, 1e+73, 1e+74, 1e+75, 1e+76, 1e+77, 1e+78, 1e+79,
			1e+80, 1e+81, 1e+82, 1e+83, 1e+84, 1e+85, 1e+86, 1e+87, 1e+88, 1e+89,
			1e+90, 1e+91, 1e+92, 1e+93, 1e+94, 1e+95, 1e+96, 1e+97, 1e+98, 1e+99,
			1e+100, 1e+101, 1e+102, 1e+103, 1e+104, 1e+105, 1e+106, 1e+107, 1e+108, 1e+109,
			1e+110, 1e+111, 1e+112, 1e+113, 1e+114, 1e+115, 1e+116, 1e+117, 1e+118, 1e+119,
			1e+120, 1e+121, 1e+122, 1e+123, 1e+124, 1e+125, 1e+126, 1e+127, 1e+128, 1e+129,
			1e+130, 1e+131, 1e+132, 1e+133, 1e+134, 1e+135, 1e+136, 1e+137, 1e+138, 1e+139,
			1e+140, 1e+141, 1e+142, 1e+143, 1e+144, 1e+145, 1e+146, 1e+147, 1e+148, 1e+149,
			1e+150, 1e+151, 1e+152, 1e+153, 1e+154, 1e+155, 1e+156, 1e+157, 1e+158, 1e+159,
			1e+160, 1e+161, 1e+162, 1e+163, 1e+164, 1e+165, 1e+166, 1e+167, 1e+168, 1e+169,
			1e+170, 1e+171, 1e+172, 1e+173, 1e+174, 1e+175, 1e+176, 1e+177, 1e+178, 1e+179,
			1e+180, 1e+181, 1e+182, 1e+183, 1e+184, 1e+185, 1e+186, 1e+187, 1e+188, 1e+189,
			1e+190, 1e+191, 1e+192, 1e+193, 1e+194, 1e+195, 1e+196, 1e+197, 1e+198, 1e+199,
			1e+200, 1e+201, 1e+202, 1e+203, 1e+204, 1e+205, 1e+206, 1e+207, 1e+208, 1e+209,
			1e+210, 1e+211, 1e+212, 1e+213, 1e+214, 1e+215, 1e+216, 1e+217, 1e+218, 1e+219,
			1e+220, 1e+221, 1e+222, 1e+223, 1e+224, 1e+225, 1e+226, 1e+227, 1e+228, 1e+229,
			1e+230, 1e+231, 1e+232, 1e+233, 1e+234, 1e+235, 1e+236, 1e+237, 1e+238, 1e+239,
			1e+240, 1e+241, 1e+242, 1e+243, 1e+244, 1e+245, 1e+246, 1e+247, 1e+248, 1e+249,
			1e+250, 1e+251, 1e+252, 1e+253, 1e+254, 1e+255, 1e+256, 1e+257, 1e+258, 1e+259,
			1e+260, 1e+261, 1e+262, 1e+263, 1e+264, 1e+265, 1e+266, 1e+267, 1e+268, 1e+269,
			1e+270, 1e+271, 1e+272, 1e+273, 1e+274, 1e+275, 1e+276, 1e+277, 1e+278, 1e+279,
			1e+280, 1e+281, 1e+282, 1e+283, 1e+284, 1e+285, 1e+286, 1e+287, 1e+288, 1e+289,
			1e+290, 1e+291, 1e+292, 1e+293, 1e+294, 1e+295, 1e+296, 1e+297, 1e+298, 1e+299,
			1e+300, 1e+301, 1e+302, 1e+303, 1e+304, 1e+305, 1e+306, 1e+307, 1e+308,
		};
		#endregion
		#region negativePowerTable
		static readonly double[] negativePowerTable = new double[] {
			1e-00, 1e-01, 1e-02, 1e-03, 1e-04, 1e-05, 1e-06, 1e-07, 1e-08, 1e-09,
			1e-10, 1e-11, 1e-12, 1e-13, 1e-14, 1e-15, 1e-16, 1e-17, 1e-18, 1e-19,
			1e-20, 1e-21, 1e-22, 1e-23, 1e-24, 1e-25, 1e-26, 1e-27, 1e-28, 1e-29,
			1e-30, 1e-31, 1e-32, 1e-33, 1e-34, 1e-35, 1e-36, 1e-37, 1e-38, 1e-39,
			1e-40, 1e-41, 1e-42, 1e-43, 1e-44, 1e-45, 1e-46, 1e-47, 1e-48, 1e-49,
			1e-50, 1e-51, 1e-52, 1e-53, 1e-54, 1e-55, 1e-56, 1e-57, 1e-58, 1e-59,
			1e-60, 1e-61, 1e-62, 1e-63, 1e-64, 1e-65, 1e-66, 1e-67, 1e-68, 1e-69,
			1e-70, 1e-71, 1e-72, 1e-73, 1e-74, 1e-75, 1e-76, 1e-77, 1e-78, 1e-79,
			1e-80, 1e-81, 1e-82, 1e-83, 1e-84, 1e-85, 1e-86, 1e-87, 1e-88, 1e-89,
			1e-90, 1e-91, 1e-92, 1e-93, 1e-94, 1e-95, 1e-96, 1e-97, 1e-98, 1e-99,
			1e-100, 1e-101, 1e-102, 1e-103, 1e-104, 1e-105, 1e-106, 1e-107, 1e-108, 1e-109,
			1e-110, 1e-111, 1e-112, 1e-113, 1e-114, 1e-115, 1e-116, 1e-117, 1e-118, 1e-119,
			1e-120, 1e-121, 1e-122, 1e-123, 1e-124, 1e-125, 1e-126, 1e-127, 1e-128, 1e-129,
			1e-130, 1e-131, 1e-132, 1e-133, 1e-134, 1e-135, 1e-136, 1e-137, 1e-138, 1e-139,
			1e-140, 1e-141, 1e-142, 1e-143, 1e-144, 1e-145, 1e-146, 1e-147, 1e-148, 1e-149,
			1e-150, 1e-151, 1e-152, 1e-153, 1e-154, 1e-155, 1e-156, 1e-157, 1e-158, 1e-159,
			1e-160, 1e-161, 1e-162, 1e-163, 1e-164, 1e-165, 1e-166, 1e-167, 1e-168, 1e-169,
			1e-170, 1e-171, 1e-172, 1e-173, 1e-174, 1e-175, 1e-176, 1e-177, 1e-178, 1e-179,
			1e-180, 1e-181, 1e-182, 1e-183, 1e-184, 1e-185, 1e-186, 1e-187, 1e-188, 1e-189,
			1e-190, 1e-191, 1e-192, 1e-193, 1e-194, 1e-195, 1e-196, 1e-197, 1e-198, 1e-199,
			1e-200, 1e-201, 1e-202, 1e-203, 1e-204, 1e-205, 1e-206, 1e-207, 1e-208, 1e-209,
			1e-210, 1e-211, 1e-212, 1e-213, 1e-214, 1e-215, 1e-216, 1e-217, 1e-218, 1e-219,
			1e-220, 1e-221, 1e-222, 1e-223, 1e-224, 1e-225, 1e-226, 1e-227, 1e-228, 1e-229,
			1e-230, 1e-231, 1e-232, 1e-233, 1e-234, 1e-235, 1e-236, 1e-237, 1e-238, 1e-239,
			1e-240, 1e-241, 1e-242, 1e-243, 1e-244, 1e-245, 1e-246, 1e-247, 1e-248, 1e-249,
			1e-250, 1e-251, 1e-252, 1e-253, 1e-254, 1e-255, 1e-256, 1e-257, 1e-258, 1e-259,
			1e-260, 1e-261, 1e-262, 1e-263, 1e-264, 1e-265, 1e-266, 1e-267, 1e-268, 1e-269,
			1e-270, 1e-271, 1e-272, 1e-273, 1e-274, 1e-275, 1e-276, 1e-277, 1e-278, 1e-279,
			1e-280, 1e-281, 1e-282, 1e-283, 1e-284, 1e-285, 1e-286, 1e-287, 1e-288, 1e-289,
			1e-290, 1e-291, 1e-292, 1e-293, 1e-294, 1e-295, 1e-296, 1e-297, 1e-298, 1e-299,
			1e-300, 1e-301, 1e-302, 1e-303, 1e-304, 1e-305, 1e-306, 1e-307, 1e-308,
		};
		#endregion
	}
	#endregion
}
