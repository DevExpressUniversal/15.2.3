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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts.Design {
	public class AxisRangeTypeConverter : ExpandableObjectConverter {
		enum TokenKind { Error, MinValue, MaxValue, MinValueInternal, MaxValueInternal, SideMarginsEnabled, AlwaysShowZeroLevel };
		struct TokenIdentifier {
			TokenKind tokenKind;
			string identifier;
			public TokenKind TokenKind { get { return tokenKind; } }
			public string Identifier { get { return identifier; } }
			public TokenIdentifier(TokenKind tokenKind, string identifier) {
				this.tokenKind = tokenKind;
				this.identifier = identifier;
			}
		}
		const char assignChar = '=';
		const char openBracketChar = '(';
		const char closeBracketChar = ')';
		const char quoteChar = '\'';
		const char delimiterChar = ';';
		const string trueValue = "True";
		const string falseValue = "False";
		const string dateTimeFunctionName = "DateTime";
		static readonly TokenIdentifier[] identifiers;
		static object defaultMinValue;
		static object defaultMaxValue;
		static double defaultMinValueInternal;
		static double defaultMaxValueInternal;
		static bool defaultSideMarginsEnabled;
		static bool defaultAlwaysShowZeroLevel;
		[Obsolete]
		static AxisRangeTypeConverter() {
			Type type = typeof(AxisRange);
			defaultMinValue = AxisRange.MinValueProperty.GetMetadata(type).DefaultValue;
			defaultMaxValue = AxisRange.MaxValueProperty.GetMetadata(type).DefaultValue;
			defaultMinValueInternal = (double)AxisRange.MinValueInternalProperty.GetMetadata(type).DefaultValue;
			defaultMaxValueInternal = (double)AxisRange.MaxValueInternalProperty.GetMetadata(type).DefaultValue;
			defaultSideMarginsEnabled = (bool)AxisRange.SideMarginsEnabledProperty.GetMetadata(type).DefaultValue;
#pragma warning disable 0618
			defaultAlwaysShowZeroLevel = (bool)AxisY2D.AlwaysShowZeroLevelProperty.GetMetadata(type).DefaultValue;
#pragma warning restore 0618
			identifiers = new TokenIdentifier[] { 
				new TokenIdentifier(TokenKind.MinValueInternal, "MinValueInternal"),
				new TokenIdentifier(TokenKind.MaxValueInternal, "MaxValueInternal"),
				new TokenIdentifier(TokenKind.MinValue, "MinValue"),
				new TokenIdentifier(TokenKind.MaxValue, "MaxValue"),
				new TokenIdentifier(TokenKind.SideMarginsEnabled, "SideMarginsEnabled"),
				new TokenIdentifier(TokenKind.AlwaysShowZeroLevel, "AlwaysShowZeroLevel")
			};
		}
		static void ThrowFormatException(string value) {
			throw new FormatException(String.Format("AxisRange de-serialization: The '{0}' string is in an incorrect format.", value));
		}
		static string FindToken(ref string value) {
			bool isQuotation = false;
			for (int i = 0; i < value.Length; i++) {
				if (value[i] == quoteChar)
					isQuotation = !isQuotation;
				else if (value[i] == delimiterChar && !isQuotation) {
					string token = value.Substring(0, i);
					value = value.Substring(i + 1);
					return token;
				}
			}
			string result = value;
			value = null;
			return result;
		}
		static TokenKind ExtractValue(string token, out string value) {
			value = null;
			TokenKind kind = TokenKind.Error;
			foreach (TokenIdentifier identifier in identifiers)
				if (token.StartsWith(identifier.Identifier)) {
					kind = identifier.TokenKind;
					value = token.Substring(identifier.Identifier.Length);
					break;
				}
			if (kind == TokenKind.Error)
				return kind;
			value = value.Trim();
			if (String.IsNullOrEmpty(value) || value[0] != assignChar)
				return TokenKind.Error;
			value = value.Substring(1).Trim();
			if (String.IsNullOrEmpty(value))
				return TokenKind.Error;
			return kind;
		}
		static object GetDeserializedObject(string str) {
			try {
				IFormatProvider provider = CultureInfo.InvariantCulture;
				if (str[0] == quoteChar) {
					if (str.Length < 2 || str[str.Length - 1] != quoteChar)
						return null;
					str = str.Substring(1, str.Length - 2);
					if (String.IsNullOrEmpty(str))
						return null;
					string doubledQuotes = new string(quoteChar, 2);
					int startIndex = 0;
					do {
						int index = str.IndexOf(quoteChar, startIndex);
						if (index == -1)
							break;
						if (str.IndexOf(doubledQuotes, startIndex) != index)
							return null;
						startIndex = index + 2;
					} while (startIndex < str.Length);
					return str.Replace(doubledQuotes, new string(quoteChar, 1));
				}
				if (str.StartsWith(dateTimeFunctionName)) {
					str = str.Substring(dateTimeFunctionName.Length).Trim();
					if (String.IsNullOrEmpty(str) || (str[0] != openBracketChar && str[str.Length - 1] != closeBracketChar))
						return null;
					str = str.Substring(1, str.Length - 2).Trim();
					if (String.IsNullOrEmpty(str))
						return null;
					return DateTimeValueHelper.ConvertFromString(str);
				}
				if (str == trueValue)
					return true;
				if (str == falseValue)
					return false;
				return Convert.ToDouble(str, provider);
			}
			catch {
			}
			return null;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		[Obsolete]
		object ConvertFromInternal(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if (str == null)
				return base.ConvertFrom(context, culture, value);
			object minValue = defaultMinValue;
			object maxValue = defaultMaxValue;
			double minValueInternal = defaultMinValueInternal;
			double maxValueInternal = defaultMaxValueInternal;
			bool sideMarginsEnabled = defaultSideMarginsEnabled;
			bool alwaysShowZeroLevel = defaultAlwaysShowZeroLevel;
			string current = str;
			while (!String.IsNullOrEmpty(current)) {
				current = current.TrimStart(null);
				if (String.IsNullOrEmpty(current))
					break;
				string token = FindToken(ref current);
				string valueToDeserialize = null;
				TokenKind tokenKind = ExtractValue(token, out valueToDeserialize);
				if (tokenKind == TokenKind.Error)
					ThrowFormatException(str);
				object deserialized = GetDeserializedObject(valueToDeserialize);
				if (deserialized == null)
					ThrowFormatException(str);
				switch (tokenKind) {
					case TokenKind.MinValue:
						minValue = deserialized;
						minValueInternal = defaultMinValueInternal;
						break;
					case TokenKind.MaxValue:
						maxValue = deserialized;
						maxValueInternal = defaultMaxValueInternal;
						break;
					case TokenKind.MinValueInternal:
						if (!(deserialized is double))
							ThrowFormatException(str);
						minValueInternal = (double)deserialized;
						minValue = defaultMinValue;
						break;
					case TokenKind.MaxValueInternal:
						if (!(deserialized is double))
							ThrowFormatException(str);
						maxValueInternal = (double)deserialized;
						maxValue = defaultMaxValue;
						break;
					case TokenKind.SideMarginsEnabled:
						if (!(deserialized is bool))
							ThrowFormatException(str);
						sideMarginsEnabled = (bool)deserialized;
						break;
					case TokenKind.AlwaysShowZeroLevel:
						if (!(deserialized is bool))
							ThrowFormatException(str);
						alwaysShowZeroLevel = (bool)deserialized;
						break;
				}
			}
			AxisRange range = new AxisRange();
			if (minValue != defaultMinValue)
				range.MinValue = minValue;
			if (maxValue != defaultMaxValue)
				range.MaxValue = maxValue;
			if (minValueInternal.CompareTo(defaultMinValueInternal) != 0)
				range.MinValueInternal = minValueInternal;
			if (maxValueInternal.CompareTo(defaultMaxValueInternal) != 0)
				range.MaxValueInternal = maxValueInternal;
			if (sideMarginsEnabled != defaultSideMarginsEnabled)
				range.SideMarginsEnabled = sideMarginsEnabled;
			if (alwaysShowZeroLevel != defaultAlwaysShowZeroLevel) {
				AxisY2D.SetAlwaysShowZeroLevel(range, alwaysShowZeroLevel);
				AxisY3D.SetAlwaysShowZeroLevel(range, alwaysShowZeroLevel);
			}
			return range;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
#pragma warning disable 0612
			return ConvertFromInternal(context, culture, value);
#pragma warning restore 0612
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string))
				return null;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
