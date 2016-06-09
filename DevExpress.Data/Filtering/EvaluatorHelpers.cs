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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using System.IO;
using System.Globalization;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Utils;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Helpers;
using System.Reflection;
namespace DevExpress.Data.Filtering.Helpers {
	public class EvaluatorProperty {
		public static int GetPropertySeparatorDotPos(string property) {
			return GetPropertySeparatorDotPos(property, 0);
		}
		public static int GetPropertySeparatorDotPos(string property, int startPos) {
			if(string.IsNullOrEmpty(property))
				throw new ArgumentNullException("property");
			for(int pos = startPos; pos < property.Length; ++pos) {
				switch(property[pos]) {
					case '.':
						return pos;
					case '<':
						for(; pos < property.Length && property[pos] != '>'; ++pos)
							;
						break;
				}
			}
			return -1;
		}
		public static int CalcCollectionPropertyDepth(string prop) {
			int rv = 1;
			for(int pos = 0; ; ) {
				pos = GetPropertySeparatorDotPos(prop, pos);
				if(pos < 0)
					return rv;
				else {
					++pos;
					++rv;
				}
			}
		}
		public static string[] Split(string prop) {
			List<string> rv = new List<string>();
			int subPropertyStart = 0;
			for(; ; ) {
				int dotPos = GetPropertySeparatorDotPos(prop, subPropertyStart);
				if(dotPos < 0) {
					rv.Add(prop.Substring(subPropertyStart));
					break;
				} else {
					rv.Add(prop.Substring(subPropertyStart, dotPos - subPropertyStart));
					subPropertyStart = dotPos + 1;
				}
			}
			return rv.ToArray();
		}
		EvaluatorProperty subProperty;
		public EvaluatorProperty SubProperty {
			get {
				if(subProperty == null && PropertyPathTokenized.Length > 1) {
					subProperty = new EvaluatorProperty(string.Join(".", PropertyPathTokenized, 1, PropertyPathTokenized.Length - 1));
				}
				return subProperty;
			}
		}
		public readonly int UpDepth;
		public readonly string PropertyPath;
		string[] tokenized = null;
		public string[] PropertyPathTokenized {
			get {
				if(tokenized == null)
					tokenized = Split(PropertyPath);
				return tokenized;
			}
		}
		protected EvaluatorProperty(string sourcePath) {
			PropertyPath = sourcePath;
			UpDepth = 0;
			while(PropertyPath.StartsWith("^.")) {
				++UpDepth;
				PropertyPath = PropertyPath.Substring(2);
			}
			if(PropertyPath == "^") {
				++UpDepth;
				PropertyPath = "This";
			}
		}
		public static EvaluatorProperty Create(OperandProperty property) {
			return new EvaluatorProperty(property.PropertyName);
		}
		public static bool GetIsThisProperty(string propertyName) {
			if(propertyName == null)
				return true;
			if(propertyName.Length == 0)
				return true;
			const string thisLowerString = "this";
			if(propertyName.Length == thisLowerString.Length && propertyName.ToLower() == thisLowerString)
				return true;
			return false;
		}
	}
	public class EvaluatorPropertyCache {
		Dictionary<OperandProperty, EvaluatorProperty> store = new Dictionary<OperandProperty, EvaluatorProperty>();
		public EvaluatorProperty this[OperandProperty property] {
			get {
				EvaluatorProperty result;
				if(!store.TryGetValue(property, out result)) {
					result = EvaluatorProperty.Create(property);
					store.Add(property, result);
				}
				return result;
			}
		}
	}
	public static class LikeData {
		[Obsolete("Use FunctionOperatorType.StartsWith, .EndsWith, .Contains instead")]
		public static string Escape(string autoFilterText) {
			if(autoFilterText.IndexOfAny(new char[] { '_', '%', '[' }) < 0)
				return autoFilterText;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			StringReader reader = new StringReader(autoFilterText);
			for(; ; ) {
				int nextCharAsInt = reader.Read();
				if(nextCharAsInt == -1)
					break;
				char inputChar = (char)nextCharAsInt;
				if(inputChar == '%' || inputChar == '_' || inputChar == '[') {
					result.Append('[');
					result.Append(inputChar);
					result.Append(']');
				} else {
					result.Append(inputChar);
				}
			}
			return result.ToString();
		}
		[Obsolete("Use FunctionOperatorType.StartsWith instead")]
		public static string CreateStartsWithPattern(string autoFilterText) {
			return Escape(autoFilterText) + '%';
		}
		[Obsolete("Use FunctionOperatorType.Contains instead")]
		public static string CreateContainsPattern(string autoFilterText) {
			return '%' + Escape(autoFilterText) + '%';
		}
		public static string ConvertToRegEx(string originalPattern) {
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append('^');
			StringReader reader = new StringReader(originalPattern);
			for(; ; ) {
				int nextCharAsInt = reader.Read();
				if(nextCharAsInt == -1)
					break;
				char inputChar = (char)nextCharAsInt;
				switch(CharUnicodeInfo.GetUnicodeCategory(inputChar)) {
					case UnicodeCategory.DecimalDigitNumber:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.ModifierLetter:
					case UnicodeCategory.OtherLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.UppercaseLetter:
						result.Append(inputChar);
						break;
					default:
						if(inputChar == '%') {
							result.Append(".*");
						} else if(inputChar == '_') {
							result.Append('.');
						} else if(inputChar == '[') {
							result.Append(inputChar);
							for(; ; ) {
								nextCharAsInt = reader.Read();
								if(nextCharAsInt == -1)
									throw new ArgumentException(string.Format("Unbalanced '[' within '{0}' like pattern", originalPattern));
								inputChar = (char)nextCharAsInt;
								result.Append(inputChar);
								if(inputChar == ']')
									break;
							}
						} else {
							result.Append('[');
							if(inputChar == '^' || inputChar == '\\' || inputChar == '-')
								result.Append('\\');
							result.Append(inputChar);
							result.Append(']');
						}
						break;
				}
			}
			result.Append('$');
			return result.ToString();
		}
		internal static Func<string, bool> CreatePredicate(string pat, bool caseSensitive) {
			System.Text.RegularExpressions.RegexOptions options = caseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;
			options |= System.Text.RegularExpressions.RegexOptions.Singleline;
			var regEx = new System.Text.RegularExpressions.Regex(ConvertToRegEx(pat), options);
			return s => s != null ? regEx.IsMatch(s) : false;
		}
		[Obsolete("Use FunctionOperatorType.StartsWith, .EndsWith, .Contains instead")]
		public static string UnEscape(string likePattern) {
			string result = likePattern;
			result = result.Replace("[%]", "%");
			result = result.Replace("[_]", "_");
			result = result.Replace("[[]", "[");
			return result;
		}
	}
	public static class LikeDataCache {
		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<Func<string, bool>>> sensetiveStore = new System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<Func<string, bool>>>();
		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<Func<string, bool>>> insensetiveStore = new System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<Func<string, bool>>>();
		public static Func<string, bool> Get(string pattern) {
			return Get(pattern, false);
		}
		public static Func<string, bool> Get(string pattern, bool caseSensitive) {
			var store = caseSensitive ? sensetiveStore : insensetiveStore;
			if(store.Count > 4096)
				store.Clear();
			var result = store.GetOrAdd(pattern, dummy => new Lazy<Func<string, bool>>(() => LikeData.CreatePredicate(pattern, caseSensitive)));
			return result.Value;
		}
	}
	public static class EvalHelpers {
		static readonly Dictionary<TypeCode, Dictionary<TypeCode, TypeCode>> BinaryNumericPromotions;
#if DEBUGTEST
		public static Dictionary<TypeCode, Dictionary<TypeCode, TypeCode>> BackDoor4BinaryNumericPromotions { get { return BinaryNumericPromotions; } }
#endif
		static EvalHelpers() {
			BinaryNumericPromotions = new Dictionary<TypeCode, Dictionary<TypeCode, TypeCode>>();
			Dictionary<TypeCode, TypeCode> promotions;
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Byte, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Char, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Decimal, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Decimal);
			promotions.Add(TypeCode.Char, TypeCode.Decimal);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Decimal);
			promotions.Add(TypeCode.Int32, TypeCode.Decimal);
			promotions.Add(TypeCode.Int64, TypeCode.Decimal);
			promotions.Add(TypeCode.SByte, TypeCode.Decimal);
			promotions.Add(TypeCode.Single, TypeCode.Double);
			promotions.Add(TypeCode.UInt16, TypeCode.Decimal);
			promotions.Add(TypeCode.UInt32, TypeCode.Decimal);
			promotions.Add(TypeCode.UInt64, TypeCode.Decimal);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Double, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Double);
			promotions.Add(TypeCode.Char, TypeCode.Double);
			promotions.Add(TypeCode.Decimal, TypeCode.Double);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Double);
			promotions.Add(TypeCode.Int32, TypeCode.Double);
			promotions.Add(TypeCode.Int64, TypeCode.Double);
			promotions.Add(TypeCode.SByte, TypeCode.Double);
			promotions.Add(TypeCode.Single, TypeCode.Double);
			promotions.Add(TypeCode.UInt16, TypeCode.Double);
			promotions.Add(TypeCode.UInt32, TypeCode.Double);
			promotions.Add(TypeCode.UInt64, TypeCode.Double);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Int16, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Int32, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Int64, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int64);
			promotions.Add(TypeCode.Char, TypeCode.Int64);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int64);
			promotions.Add(TypeCode.Int32, TypeCode.Int64);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int64);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int64);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.Int64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.SByte, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Single, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Single);
			promotions.Add(TypeCode.Char, TypeCode.Single);
			promotions.Add(TypeCode.Decimal, TypeCode.Double);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Single);
			promotions.Add(TypeCode.Int32, TypeCode.Single);
			promotions.Add(TypeCode.Int64, TypeCode.Single);
			promotions.Add(TypeCode.SByte, TypeCode.Single);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Single);
			promotions.Add(TypeCode.UInt32, TypeCode.Single);
			promotions.Add(TypeCode.UInt64, TypeCode.Single);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.UInt16, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.UInt32, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.UInt32);
			promotions.Add(TypeCode.Char, TypeCode.UInt32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int64);
			promotions.Add(TypeCode.Int32, TypeCode.Int64);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int64);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.UInt64, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.UInt64);
			promotions.Add(TypeCode.Char, TypeCode.UInt64);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.UInt64);
			promotions.Add(TypeCode.Int32, TypeCode.UInt64);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.UInt64);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.UInt64);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.DateTime, promotions);
			promotions.Add(TypeCode.DateTime, TypeCode.DateTime);
			promotions.Add(TypeCode.Object, TypeCode.DateTime);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Object, promotions);
			promotions.Add(TypeCode.DateTime, TypeCode.DateTime);
			promotions.Add(TypeCode.Object, TypeCode.Object);
		}
		public static TypeCode GetBinaryNumericPromotionCode(Type left, Type right, BinaryOperatorType exceptionType, bool raiseException) {
			TypeCode leftTC = DXTypeExtensions.GetTypeCode(NullableHelpers.GetBoxedType(left));
			TypeCode rightTC = DXTypeExtensions.GetTypeCode(NullableHelpers.GetBoxedType(right));
			Dictionary<TypeCode, TypeCode> rights;
			if(!BinaryNumericPromotions.TryGetValue(leftTC, out rights)) {
				if(raiseException)
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, exceptionType.ToString(), left.FullName));
				else return TypeCode.Object;
			}
			TypeCode result;
			if(!rights.TryGetValue(rightTC, out result)) {
				if(raiseException)
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, exceptionType.ToString(), right.FullName));
				else return TypeCode.Object;
			}
			return result;
		}
		static readonly Type stringType = typeof(string);
		static object ConvertValue(object val, Type valType, Type type) {
			if(Object.ReferenceEquals(valType, type))
				return val;
			if(type.IsEnum()) {
				if(Object.ReferenceEquals(valType, stringType))
					return Enum.Parse(type, (string)val, false);
				else
					return Enum.ToObject(type, val);
			}
			if(val is IConvertible)
				return Convert.ChangeType(val, type, CultureInfo.InvariantCulture);
			else
				return val;
		}
		public static int CompareObjects(object left, object right, bool isEqualityCompare, bool caseSensitive, IComparer customComparer) {
			if(customComparer != null) return customComparer.Compare(left, right);
			if(left == null)
				return right == null ? 0 : -1;
			if(right == null)
				return 1;
			Type rightType = right.GetType();
			Type leftType = left.GetType();
			if(Object.ReferenceEquals(rightType, stringType))
				left = left.ToString();
			else
				right = ConvertValue(right, rightType, leftType);
			if(Object.ReferenceEquals(leftType, stringType))
				return System.Globalization.CultureInfo.CurrentCulture.CompareInfo.Compare((string)left, right.ToString(), caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase);
			else {
				if(isEqualityCompare)
					return left.Equals(right) ? 0 : -1;
				IComparable c = left as IComparable;
				if(c == null)
					throw new Exception();
				return c.CompareTo(right);
			}
		}
		static CriteriaOperator MakeTypicalOutlookInterval(CriteriaOperator op, FunctionOperatorType lowerBound, FunctionOperatorType upperBound) {
			return op >= new FunctionOperator(lowerBound) & op < new FunctionOperator(upperBound);
		}
		public static CriteriaOperator ExpandIsOutlookInterval(FunctionOperator theOperator) {
			if(theOperator.Operands.Count != 1)
				throw new ArgumentException("theOperator.Operands.Count != 1");
			CriteriaOperator op = theOperator.Operands[0];
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.IsThisMonth:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisMonth, FunctionOperatorType.LocalDateTimeNextMonth);
				case FunctionOperatorType.IsThisWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisWeek, FunctionOperatorType.LocalDateTimeNextWeek);
				case FunctionOperatorType.IsThisYear:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisYear, FunctionOperatorType.LocalDateTimeNextYear);
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
					return op >= new FunctionOperator(FunctionOperatorType.LocalDateTimeNextYear);
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeNextMonth, FunctionOperatorType.LocalDateTimeNextYear);
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeTwoWeeksAway, FunctionOperatorType.LocalDateTimeNextMonth);
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeNextWeek, FunctionOperatorType.LocalDateTimeTwoWeeksAway);
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeDayAfterTomorrow, FunctionOperatorType.LocalDateTimeNextWeek);
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeTomorrow, FunctionOperatorType.LocalDateTimeDayAfterTomorrow);
				case FunctionOperatorType.IsOutlookIntervalToday:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeToday, FunctionOperatorType.LocalDateTimeTomorrow);
				case FunctionOperatorType.IsOutlookIntervalYesterday:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeYesterday, FunctionOperatorType.LocalDateTimeToday);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisWeek, FunctionOperatorType.LocalDateTimeYesterday);
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeLastWeek, FunctionOperatorType.LocalDateTimeThisWeek);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisMonth, FunctionOperatorType.LocalDateTimeLastWeek);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisYear, FunctionOperatorType.LocalDateTimeThisMonth);
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					return op < new FunctionOperator(FunctionOperatorType.LocalDateTimeThisYear);
				default:
					throw new InvalidOperationException("theOperator.OperatorType is not IsOutlookInterval* or internal error");
			}
		}
		public static DateTime GetWeekStart(DateTime now) {
			return GetWeekStart(now, System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}
		public static DateTime GetWeekStart(DateTime now, DateTimeFormatInfo dtfi) {
			DateTime today = now.Date;
			DayOfWeek current = today.DayOfWeek;
			DayOfWeek wanted = dtfi.FirstDayOfWeek;
			int diff = (((int)current) - ((int)wanted) + 7) % 7;
			return today.AddDays(-diff);
		}
		public static DateTime EvaluateLocalDateTime(FunctionOperatorType type) {
			switch(type) {
				case FunctionOperatorType.LocalDateTimeThisYear:
					return FnLocalDateTimeThisYear();
				case FunctionOperatorType.LocalDateTimeThisMonth:
					return FnLocalDateTimeThisMonth();
				case FunctionOperatorType.LocalDateTimeLastWeek:
					return FnLocalDateTimeLastWeek();
				case FunctionOperatorType.LocalDateTimeThisWeek:
					return FnLocalDateTimeThisWeek();
				case FunctionOperatorType.LocalDateTimeYesterday:
					return FnLocalDateTimeYesterday();
				case FunctionOperatorType.LocalDateTimeToday:
					return FnLocalDateTimeToday();
				case FunctionOperatorType.LocalDateTimeNow:
					return FnLocalDateTimeNow();
				case FunctionOperatorType.LocalDateTimeTomorrow:
					return FnLocalDateTimeTomorrow();
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
					return FnLocalDateTimeDayAfterTomorrow();
				case FunctionOperatorType.LocalDateTimeNextWeek:
					return FnLocalDateTimeNextWeek();
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
					return FnLocalDateTimeTwoWeeksAway();
				case FunctionOperatorType.LocalDateTimeNextMonth:
					return FnLocalDateTimeNextMonth();
				case FunctionOperatorType.LocalDateTimeNextYear:
					return FnLocalDateTimeNextYear();
				default:
					throw new InvalidOperationException("theOperator.OperatorType is not LocalDateTime* or internal error");
			}
		}
		static DateTime FnLocalDateTimeNextYear() {
			DateTime now = DateTime.Now;
			return new DateTime(now.Year + 1, 1, 1);
		}
		static DateTime FnLocalDateTimeNextMonth() {
			DateTime now = DateTime.Now;
			return new DateTime(now.Year, now.Month, 1).AddMonths(1);
		}
		static DateTime FnLocalDateTimeTwoWeeksAway() {
			DateTime now = DateTime.Now;
			return GetWeekStart(now).AddDays(14);
		}
		static DateTime FnLocalDateTimeNextWeek() {
			DateTime now = DateTime.Now;
			return GetWeekStart(now).AddDays(7);
		}
		static DateTime FnLocalDateTimeDayAfterTomorrow() {
			DateTime now = DateTime.Now;
			return now.Date.AddDays(2);
		}
		static DateTime FnLocalDateTimeTomorrow() {
			DateTime now = DateTime.Now;
			return now.Date.AddDays(1);
		}
		static DateTime FnLocalDateTimeNow() {
			DateTime now = DateTime.Now;
			return now;
		}
		static DateTime FnLocalDateTimeToday() {
			DateTime now = DateTime.Now;
			return now.Date;
		}
		static DateTime FnLocalDateTimeYesterday() {
			DateTime now = DateTime.Now;
			return now.Date.AddDays(-1);
		}
		static DateTime FnLocalDateTimeThisWeek() {
			DateTime now = DateTime.Now;
			return GetWeekStart(now);
		}
		static DateTime FnLocalDateTimeLastWeek() {
			DateTime now = DateTime.Now;
			return GetWeekStart(now).AddDays(-7);
		}
		static DateTime FnLocalDateTimeThisMonth() {
			DateTime now = DateTime.Now;
			return new DateTime(now.Year, now.Month, 1);
		}
		static DateTime FnLocalDateTimeThisYear() {
			DateTime now = DateTime.Now;
			return new DateTime(now.Year, 1, 1);
		}
		static int FnDiffMilliSecond(DateTime op1, DateTime op2) {
			return FnDiffSecond(op1, op2) * 1000 + op2.Millisecond - op1.Millisecond;
		}
		static int FnDiffMonth(DateTime op1, DateTime op2) {
			return (op2.Year - op1.Year) * 12 + op2.Month - op1.Month;
		}
		static int FnDiffYear(DateTime op1, DateTime op2) {
			return op2.Year - op1.Year;
		}
		static int FnDiffSecond(DateTime op1, DateTime op2) {
			return (FnDiffMinute(op1, op2) * 60) + op2.Second - op1.Second;
		}
		static int FnDiffMinute(DateTime op1, DateTime op2) {
			return (FnDiffHour(op1, op2) * 60) + op2.Minute - op1.Minute;
		}
		static int FnDiffHour(DateTime op1, DateTime op2) {
			return (FnDiffDay(op1, op2) * 24) + op2.Hour - op1.Hour;
		}
		static int FnDiffDay(DateTime op1, DateTime op2) {
			return ((TimeSpan)(op2.Date - op1.Date)).Days;
		}
		static TypeCode GetBinaryNumericPromotionCode(object left, object right, BinaryOperatorType exceptionType) {
			if(left == null || right == null)
				return TypeCode.Empty;
			return GetBinaryNumericPromotionCode(left.GetType(), right.GetType(), exceptionType, true);
		}
		public static Type GetBinaryNumericPromotionType(Type leftType, Type rightType) {
			TypeCode resultType = EvalHelpers.GetBinaryNumericPromotionCode(leftType, rightType, BinaryOperatorType.Plus, false);
			switch(resultType) {
				case TypeCode.Int32:
					return typeof(Int32);
				case TypeCode.Int64:
					return typeof(Int64);
				case TypeCode.UInt32:
					return typeof(UInt32);
				case TypeCode.UInt64:
					return typeof(UInt64);
				case TypeCode.Single:
					return typeof(Single);
				case TypeCode.Double:
					return typeof(Double);
				case TypeCode.Decimal:
					return typeof(Decimal);
				case TypeCode.DateTime:
					return typeof(DateTime);
				case TypeCode.Object:
				default:
					return typeof(object);
			}
		}
		public static object DoObjectsPlus(object left, object right) {
			if(left is string || right is string) {
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", left, right);
			}
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Plus);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) + Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) + Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) + Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) + Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) + Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) + Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) + Convert.ToDecimal(right);
				case TypeCode.DateTime: {
						Type leftType = left == null ? null : left.GetType();
						Type rightType = right == null ? null : right.GetType();
						if(typeof(DateTime) == leftType && typeof(TimeSpan) == rightType) return (DateTime)left + (TimeSpan)right;
						if(typeof(TimeSpan) == leftType && typeof(DateTime) == rightType) return (DateTime)right + (TimeSpan)left;
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Plus.ToString(), tc.ToString()));
					}
				case TypeCode.Object: {
						Type leftType = left == null ? null : left.GetType();
						Type rightType = right == null ? null : right.GetType();
						if(typeof(TimeSpan) == leftType && typeof(TimeSpan) == rightType) return (TimeSpan)left + (TimeSpan)right;
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Plus.ToString(), tc.ToString()));
					}
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Plus.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsMinus(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Minus);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) - Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) - Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) - Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) - Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) - Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) - Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) - Convert.ToDecimal(right);
				case TypeCode.DateTime: {
						Type leftType = left == null ? null : left.GetType();
						Type rightType = right == null ? null : right.GetType();
						if(typeof(DateTime) == leftType && typeof(DateTime) == rightType) return (DateTime)left - (DateTime)right;
						if(typeof(DateTime) == leftType && typeof(TimeSpan) == rightType) return (DateTime)left - (TimeSpan)right;
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Minus.ToString(), tc.ToString()));
					}
				case TypeCode.Object: {
						Type leftType = left == null ? null : left.GetType();
						Type rightType = right == null ? null : right.GetType();
						if(typeof(TimeSpan) == leftType && typeof(TimeSpan) == rightType) return (TimeSpan)left - (TimeSpan)right;
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Minus.ToString(), tc.ToString()));
					}
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Minus.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsMultiply(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Multiply);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) * Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) * Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) * Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) * Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) * Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) * Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) * Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Multiply.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsDivide(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Divide);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) / Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) / Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) / Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) / Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) / Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) / Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) / Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Divide.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsModulo(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Modulo);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) % Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) % Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) % Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) % Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) % Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) % Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) % Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Modulo.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsBitwiseAnd(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.BitwiseAnd);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) & Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) & Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) & Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) & Convert.ToUInt64(right);
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.BitwiseAnd.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsBitwiseOr(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.BitwiseOr);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) | Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) | Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) | Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) | Convert.ToUInt64(right);
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.BitwiseOr.ToString(), tc.ToString()));
			}
		}
		public static object DoObjectsBitwiseXor(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.BitwiseXor);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) ^ Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) ^ Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) ^ Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) ^ Convert.ToUInt64(right);
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.BitwiseXor.ToString(), tc.ToString()));
			}
		}
		static object FnAbsObject(object op) {
			if(op == null) { return null; }
			switch(DXTypeExtensions.GetTypeCode(op.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return op;
				case TypeCode.Decimal:
					return Math.Abs((Decimal)op);
				case TypeCode.Double:
					return Math.Abs((double)op);
				case TypeCode.Int16:
					return Math.Abs((Int16)op);
				case TypeCode.Int32:
					return Math.Abs((Int32)op);
				case TypeCode.Int64:
					return Math.Abs((Int64)op);
				case TypeCode.SByte:
					return Math.Abs((sbyte)op);
				case TypeCode.Single:
					return Math.Abs((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Abs.ToString(), op.ToString()));
			}
		}
		static int? FnSignObject(object op) {
			if(op == null) { return null; }
			switch(DXTypeExtensions.GetTypeCode(op.GetType())) {
				case TypeCode.Byte:
					return Math.Sign((Byte)op);
				case TypeCode.UInt16:
					return Math.Sign((UInt16)op);
				case TypeCode.UInt32:
					return Math.Sign((UInt32)op);
				case TypeCode.UInt64:
					return ((UInt64)op) == 0 ? 0 : 1;
				case TypeCode.Decimal:
					return Math.Sign((Decimal)op);
				case TypeCode.Double:
					return Math.Sign((double)op);
				case TypeCode.Int16:
					return Math.Sign((Int16)op);
				case TypeCode.Int32:
					return Math.Sign((Int32)op);
				case TypeCode.Int64:
					return Math.Sign((Int64)op);
				case TypeCode.SByte:
					return Math.Sign((sbyte)op);
				case TypeCode.Single:
					return Math.Sign((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Sign.ToString(), op.ToString()));
			}
		}
		static object FnRoundObject(object op, int? precision) {
			if(op == null) return null;
			int op2 = precision ?? 0;
			switch(DXTypeExtensions.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return op;
				case TypeCode.Decimal:
#if CF || SL
						return (Decimal)Math.Round(Convert.ToDouble(op), op2);
#else
					return Math.Round((Decimal)op, op2);
#endif
				case TypeCode.Double:
					return Math.Round((double)op, op2);
				case TypeCode.Single:
					return (Single)Math.Round((Single)op, op2);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Round.ToString(), op.ToString()));
			}
		}
		static object FnCeilingObject(object op) {
			if(op == null)
				return null;
			switch(DXTypeExtensions.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return op;
				case TypeCode.Decimal:
#if SL
					return (Decimal)Math.Ceiling((Double)(Decimal)op);
#else
					return Math.Ceiling((Decimal)op);
#endif
				case TypeCode.Double:
					return Math.Ceiling((Double)op);
				case TypeCode.Single:
					return Math.Ceiling((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Ceiling.ToString(), op.ToString()));
			}
		}
		static object FnFloorObject(object op) {
			if(op == null)
				return null;
			switch(DXTypeExtensions.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return op;
				case TypeCode.Decimal:
#if SL
					return (Decimal)Math.Floor((Double)op);
#else
					return Math.Floor((Decimal)op);
#endif
				case TypeCode.Double:
					return Math.Floor((double)op);
				case TypeCode.Single:
					return Math.Floor((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Floor.ToString(), op.ToString()));
			}
		}
		static object FnMinMaxObjectCore(object a, object b, bool isMin, bool isCaseSensitive) {
			if(a == null)
				return b;
			if(b == null)
				return a;
			int compareResult = CompareObjects(a, b, false, isCaseSensitive, null);
			if(isMin)
				return compareResult <= 0 ? a : b;
			else
				return compareResult >= 0 ? a : b;
		}
		public static class FnMinMaxHelpers {
			public static T FnMinMaxComparableClassCore<T>(T a, T b, bool isMin) where T: class, IComparable<T> {
				if(a == null) return b;
				if(b == null) return a;
				return FnMinMaxComparableNoNullsCore(a, b, isMin);
			}
			public static T FnMinMaxComparableNoNullsCore<T>(T a, T b, bool isMin) where T: IComparable<T> {
				int cr = a.CompareTo(b);
				if(isMin)
					return cr <= 0 ? a : b;
				else
					return cr >= 0 ? a : b;
			}
			public static T FnMinMaxComparableNullableACore<T>(Nullable<T> a, T b, bool isMin) where T: struct, IComparable<T> {
				if(!a.HasValue) return b;
				return FnMinMaxComparableNoNullsCore(a.Value, b, isMin);
			}
			public static T FnMinMaxComparableNullableBCore<T>(T a, Nullable<T> b, bool isMin) where T: struct, IComparable<T> {
				if(!b.HasValue) return a;
				return FnMinMaxComparableNoNullsCore(a, b.Value, isMin);
			}
			public static Nullable<T> FnMinMaxComparableBothNullableCore<T>(Nullable<T> a, Nullable<T> b, bool isMin) where T: struct, IComparable<T> {
				if(!a.HasValue) return b;
				if(!b.HasValue) return a;
				return FnMinMaxComparableNoNullsCore(a.Value, b.Value, isMin);
			}
		}
		static LambdaExpression MakeMinMax(Type[] invokeTypes, bool isMin, bool caseSensitive) {
			if(invokeTypes.Any(t => t == typeof(string))) {
				return MakeLambdaFromSimpleFunc((string x, string y) => {
					if(x == null)
						return y;
					if(y == null)
						return x;
					StringComparer c = caseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
					int cmp = c.Compare(x, y);
					if(!isMin)
						cmp = -cmp;
					return cmp <= 0 ? x : y;
				}, invokeTypes);
			}
			if(invokeTypes.Length != 2 || invokeTypes.Any(t => t == typeof(object))) {
				return MakeLambdaFromSimpleFunc((object x, object y) => FnMinMaxObjectCore(x, y, isMin, caseSensitive), invokeTypes);
			}
			ParameterExpression ap = Expression.Parameter(invokeTypes[0], "ap");
			ParameterExpression bp = Expression.Parameter(invokeTypes[1], "bp");
			Expression body = null;
			Type boxedAType = NullableHelpers.GetBoxedType(ap.Type);
			Type comparableAType = GenericTypeHelper.GetGenericTypeArgument(boxedAType, typeof(IComparable<>));
			if(comparableAType != null && !comparableAType.IsAssignableFrom(boxedAType))
				comparableAType = null;
			Type boxedBType = NullableHelpers.GetBoxedType(bp.Type);
			Type comparableBType = GenericTypeHelper.GetGenericTypeArgument(boxedBType, typeof(IComparable<>));
			if(comparableBType != null && !comparableBType.IsAssignableFrom(boxedBType))
				comparableBType = null;
			if(comparableAType != null && comparableBType != null) {
				if(comparableAType == comparableBType) {
					Type cType = comparableAType;
					string funcName;
					if(cType.IsValueType()) {
						bool aNullable = NullableHelpers.CanAcceptNull(ap.Type);
						bool bNullable = NullableHelpers.CanAcceptNull(bp.Type);
						if(aNullable) {
							if(bNullable) {
								funcName = "FnMinMaxComparableBothNullableCore";
							} else {
								funcName = "FnMinMaxComparableNullableACore";
							}
						} else {
							if(bNullable) {
								funcName = "FnMinMaxComparableNullableBCore";
							} else {
								funcName = "FnMinMaxComparableNoNullsCore";
							}
						}
					} else {
						funcName = "FnMinMaxComparableClassCore";
					}
					body = Expression.Call(typeof(FnMinMaxHelpers), funcName, new Type[] { cType }, ap, bp, Expression.Constant(isMin));
				} else {
					Type targetType = GetBinaryNumericPromotionType(comparableAType, comparableBType);
					if(targetType != typeof(object)) {
						Type aTargetType = NullableHelpers.CanAcceptNull(ap.Type) ? NullableHelpers.GetUnBoxedType(targetType) : targetType;
						Type bTargetType = NullableHelpers.CanAcceptNull(bp.Type) ? NullableHelpers.GetUnBoxedType(targetType) : targetType;
						LambdaExpression nestedLambda = MakeMinMax(new Type[] { aTargetType, bTargetType }, isMin, caseSensitive);
						Expression ae = ap.Type == aTargetType ? (Expression)ap : Expression.Convert(ap, aTargetType);
						Expression be = bp.Type == bTargetType ? (Expression)bp : Expression.Convert(bp, bTargetType);
						body = Expression.Invoke(nestedLambda, ae, be);
					}
				}
			}
			if(body != null)
				return Expression.Lambda(body, ap, bp);
			throw new NotSupportedException("Min or Max for " + boxedAType.FullName + " and " + boxedBType.FullName);
		}
		static Random random = new Random();
		static double FnRnd() {
			lock(random) {
				return random.NextDouble();
			}
		}
		public static string CaseInsensitiveReplaceBody(string sourceString, string oldValue, string newValue) {
			var result = new System.Text.StringBuilder(oldValue.Length <= newValue.Length ? sourceString.Length : 0);
			int lastIndex = 0;
			int index = 0;
			while((index = sourceString.IndexOf(oldValue, lastIndex, StringComparison.CurrentCultureIgnoreCase)) >= 0) {
				if(index > lastIndex) result.Append(sourceString.Substring(lastIndex, index - lastIndex));
				result.Append(newValue);
				lastIndex = index + oldValue.Length;
			}
			if(lastIndex == 0) return sourceString;
			if(lastIndex < sourceString.Length) {
				result.Append(sourceString.Substring(lastIndex));
			}
			return result.ToString();
		}
		static string FnReverse(string str) {
			if(string.IsNullOrEmpty(str))
				return str;
			int len = str.Length;
			char[] arr = new char[len];
			for(int i = 0; i < len; i++) {
				arr[i] = str[len - 1 - i];
			}
			return new string(arr);
		}
		static bool? FnStartsWithCaseSensitive(string str1, string str2) {
			if(str1 == null || str2 == null)
				return null;
			return
				str1.StartsWith(str2, StringComparison.CurrentCulture) ||
				str1.StartsWithInvariantCulture(str2);
		}
		static bool? FnStartsWithCaseInsensitive(string str1, string str2) {
			if(str1 == null || str2 == null)
				return null;
			return
				str1.StartsWith(str2, StringComparison.CurrentCultureIgnoreCase) ||
				str1.StartsWithInvariantCultureIgnoreCase(str2);
		}
		static bool? FnEndsWithCaseSensitive(string str1, string str2) {
			if(str1 == null || str2 == null)
				return null;
			return str1.EndsWith(str2, StringComparison.CurrentCulture);
		}
		static bool? FnEndsWithCaseInsensitive(string str1, string str2) {
			if(str1 == null || str2 == null)
				return null;
			return str1.EndsWith(str2, StringComparison.CurrentCultureIgnoreCase);
		}
		static bool? FnContainsCaseSensitive(string str1, string str2) {
			if(str1 == null || str2 == null)
				return null;
			return
				str1.IndexOf(str2, StringComparison.CurrentCulture) >= 0 ||
				str1.IndexOfInvariantCulture(str2) >= 0;
		}
		static bool? FnContainsCaseInsensitive(string str1, string str2) {
			if(str1 == null || str2 == null)
				return null;
			return
				str1.IndexOf(str2, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
				str1.IndexOfInvariantCultureIgnoreCase(str2) >= 0;
		}
		public static Expression SafeToString(Expression e) {
			if(e.Type == typeof(string))
				return e;
			ParameterExpression objpar = Expression.Parameter(e.Type, "obj");
			Expression body;
			if(e.Type.IsValueType()) {
				if(Nullable.GetUnderlyingType(e.Type) == null) {
					return Expression.Call(e, "ToString", null, null);
				} else {
					body = Expression.Condition(Expression.PropertyOrField(objpar, "HasValue"), Expression.Call(Expression.PropertyOrField(objpar, "Value"), "ToString", null, null), Expression.Constant(null, typeof(string)));
				}
			} else {
				body = Expression.Condition(Expression.Equal(objpar, Expression.Constant(null, objpar.Type)), Expression.Constant(null, typeof(string)), Expression.Call(e.Type.IsInterface() ? (Expression)Expression.Convert(objpar, typeof(object)) : (Expression)objpar, "ToString", null, null));
			}
			return Expression.Invoke(Expression.Lambda(body, objpar), e);
		}
		public class FnConcater {
			System.Text.StringBuilder SB;
			bool Nulled;
			public bool Append(string op) {
				if(op == null) {
					Nulled = true;
					SB = null;
				}
				if(!Nulled) {
					if(SB == null)
						SB = new System.Text.StringBuilder();
					SB.Append(op);
				}
				return !Nulled;
			}
			public override string ToString() {
				if(Nulled || SB == null)
					return null;
				else
					return SB.ToString();
			}
		}
		public static LambdaExpression MakeFnLambda(FunctionOperatorType functionOperatorType, Type[] invokeTypes, bool caseSensitive) {
			switch(functionOperatorType) {
				case FunctionOperatorType.Iif:
				case FunctionOperatorType.IsNull:
				case FunctionOperatorType.Custom:
				case FunctionOperatorType.CustomNonDeterministic:
				case FunctionOperatorType.Concat:
					throw new InvalidOperationException("Should not be there: " + functionOperatorType.ToString());
				case FunctionOperatorType.None:
					throw new InvalidOperationException("None function");
				case FunctionOperatorType.IsNullOrEmpty:
					return MakeLambdaFromSimpleFunc((string x) => string.IsNullOrEmpty(x), invokeTypes);
				case FunctionOperatorType.Trim:
					return MakeLambdaFromSimpleFunc((string x) => x != null ? x.Trim() : null, invokeTypes);
				case FunctionOperatorType.Len:
					return MakeLambdaFromSimpleFunc((string x) => x != null ? (int?)x.Length : null, invokeTypes);
				case FunctionOperatorType.Substring:
					if(invokeTypes.Length == 2)
						return MakeLambdaFromSimpleFunc((string x, int? s) => x != null && s.HasValue ? x.Substring(s.Value) : null, invokeTypes);
					else
						return MakeLambdaFromSimpleFunc((string x, int? s, int? l) => x != null && s.HasValue && l.HasValue ? x.Substring(s.Value, l.Value) : null, invokeTypes);
				case FunctionOperatorType.Upper:
					return MakeLambdaFromSimpleFunc((string x) => x != null ? x.ToUpper() : null, invokeTypes);
				case FunctionOperatorType.Lower:
					return MakeLambdaFromSimpleFunc((string x) => x != null ? x.ToLower() : null, invokeTypes);
				case FunctionOperatorType.Ascii:
					return MakeLambdaFromSimpleFunc((string x) => x != null ? (int?)x[0] : null, invokeTypes);
				case FunctionOperatorType.Char:
					return MakeLambdaFromSimpleFunc((Int64? x) => x.HasValue ? ((char)x.Value).ToString() : null, invokeTypes);
				case FunctionOperatorType.ToStr:
					return MakeLambdaFromSimpleFunc((string x) => x, invokeTypes);
				case FunctionOperatorType.Replace:
					Func<string, string, string, string> replaceFn;
					if(caseSensitive)
						replaceFn = (str, pattern, replace) => str != null && !string.IsNullOrEmpty(pattern) && replace != null ? str.Replace(pattern, replace) : null;
					else
						replaceFn = (str, pattern, replace) => str != null && !string.IsNullOrEmpty(pattern) && replace != null ? CaseInsensitiveReplaceBody(str, pattern, replace) : null;
					return MakeLambdaFromSimpleFunc(replaceFn, invokeTypes);
				case FunctionOperatorType.Reverse:
					return MakeLambdaFromSimpleFunc((string x) => FnReverse(x), invokeTypes);
				case FunctionOperatorType.Insert:
					return MakeLambdaFromSimpleFunc((string x, int? startIndex, string value) => x != null && startIndex.HasValue && value != null ? x.Insert(startIndex.Value, value) : null, invokeTypes);
				case FunctionOperatorType.CharIndex:
					if(caseSensitive) {
						if(invokeTypes.Length == 2)
							return MakeLambdaFromSimpleFunc((string pattern, string body) => pattern != null && body != null ? (int?)body.IndexOf(pattern, StringComparison.CurrentCulture) : null, invokeTypes);
						else if(invokeTypes.Length == 3)
							return MakeLambdaFromSimpleFunc((string pattern, string body, int? startIndex) => pattern != null && body != null && startIndex.HasValue ? (int?)body.IndexOf(pattern, startIndex.Value, StringComparison.CurrentCulture) : null, invokeTypes);
						else
							return MakeLambdaFromSimpleFunc((string pattern, string body, int? startIndex, int? endIndex) => pattern != null && body != null && startIndex.HasValue && endIndex.HasValue ? (int?)body.IndexOf(pattern, startIndex.Value, endIndex.Value, StringComparison.CurrentCulture) : null, invokeTypes);
					} else {
						if(invokeTypes.Length == 2)
							return MakeLambdaFromSimpleFunc((string pattern, string body) => pattern != null && body != null ? (int?)body.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) : null, invokeTypes);
						else if(invokeTypes.Length == 3)
							return MakeLambdaFromSimpleFunc((string pattern, string body, int? startIndex) => pattern != null && body != null && startIndex.HasValue ? (int?)body.IndexOf(pattern, startIndex.Value, StringComparison.CurrentCultureIgnoreCase) : null, invokeTypes);
						else
							return MakeLambdaFromSimpleFunc((string pattern, string body, int? startIndex, int? endIndex) => pattern != null && body != null && startIndex.HasValue && endIndex.HasValue ? (int?)body.IndexOf(pattern, startIndex.Value, endIndex.Value, StringComparison.CurrentCultureIgnoreCase) : null, invokeTypes);
					}
				case FunctionOperatorType.Remove:
					if(invokeTypes.Length == 2)
						return MakeLambdaFromSimpleFunc((string x, int? startIndex) => x != null && startIndex.HasValue ? x.Remove(startIndex.Value) : null, invokeTypes);
					else
						return MakeLambdaFromSimpleFunc((string x, int? startIndex, int? count) => x != null && startIndex.HasValue && count.HasValue ? x.Remove(startIndex.Value, count.Value) : null, invokeTypes);
				case FunctionOperatorType.Abs:
					if(invokeTypes.Length == 1) {
						Type absArgType = invokeTypes[0];
						if(absArgType == typeof(object))
							return MakeLambdaFromSimpleFunc((object x) => EvalHelpers.FnAbsObject(x), invokeTypes);
						Type unNulledArgType = NullableHelpers.GetBoxedType(absArgType);
						switch(DXTypeExtensions.GetTypeCode(unNulledArgType)) {
							case TypeCode.Byte:
							case TypeCode.UInt16:
							case TypeCode.UInt32:
							case TypeCode.UInt64:
								ParameterExpression p = Expression.Parameter(absArgType, "unsignedAbsIsWhat");
								return Expression.Lambda(p, p);
							case TypeCode.Decimal:
								return MakeLambdaFromSimpleFunc((Decimal x) => Math.Abs(x), invokeTypes);
							case TypeCode.Double:
								return MakeLambdaFromSimpleFunc((Double x) => Math.Abs(x), invokeTypes);
							case TypeCode.Int16:
								return MakeLambdaFromSimpleFunc((Int16 x) => Math.Abs(x), invokeTypes);
							case TypeCode.Int32:
								return MakeLambdaFromSimpleFunc((Int32 x) => Math.Abs(x), invokeTypes);
							case TypeCode.Int64:
								return MakeLambdaFromSimpleFunc((Int64 x) => Math.Abs(x), invokeTypes);
							case TypeCode.SByte:
								return MakeLambdaFromSimpleFunc((SByte x) => Math.Abs(x), invokeTypes);
							case TypeCode.Single:
								return MakeLambdaFromSimpleFunc((Single x) => Math.Abs(x), invokeTypes);
						}
					}
					return MakeLambdaFromSimpleFunc((Double x) => Math.Abs(x), invokeTypes);
				case FunctionOperatorType.Sqr:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Sqrt(x), invokeTypes);
				case FunctionOperatorType.Cos:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Cos(x), invokeTypes);
				case FunctionOperatorType.Sin:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Sin(x), invokeTypes);
				case FunctionOperatorType.Atn:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Atan(x), invokeTypes);
				case FunctionOperatorType.Exp:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Exp(x), invokeTypes);
				case FunctionOperatorType.Log:
					if(invokeTypes.Length == 1)
						return MakeLambdaFromSimpleFunc((Double x) => Math.Log(x), invokeTypes);
					else
						return MakeLambdaFromSimpleFunc((Double x, Double newBase) => Math.Log(x, newBase), invokeTypes);
				case FunctionOperatorType.Rnd:
					return MakeLambdaFromSimpleFunc(() => FnRnd(), invokeTypes);
				case FunctionOperatorType.Tan:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Tan(x), invokeTypes);
				case FunctionOperatorType.Power:
					return MakeLambdaFromSimpleFunc((Double x, Double y) => Math.Pow(x, y), invokeTypes);
				case FunctionOperatorType.Sign:
					if(invokeTypes.Length >= 1) {
						if(invokeTypes[0] == typeof(object))
							return MakeLambdaFromSimpleFunc((object x) => FnSignObject(x), invokeTypes);
						switch(DXTypeExtensions.GetTypeCode(NullableHelpers.GetBoxedType(invokeTypes[0]))) {
							case TypeCode.Byte:
								return MakeLambdaFromSimpleFunc((Byte x) => Math.Sign(x), invokeTypes);
							case TypeCode.Char:
								return MakeLambdaFromSimpleFunc((Char x) => Math.Sign(x), invokeTypes);
							case TypeCode.Decimal:
								return MakeLambdaFromSimpleFunc((Decimal x) => Math.Sign(x), invokeTypes);
							case TypeCode.Double:
								return MakeLambdaFromSimpleFunc((Double x) => Math.Sign(x), invokeTypes);
							case TypeCode.Int16:
								return MakeLambdaFromSimpleFunc((Int16 x) => Math.Sign(x), invokeTypes);
							case TypeCode.Int64:
								return MakeLambdaFromSimpleFunc((Int64 x) => Math.Sign(x), invokeTypes);
							case TypeCode.SByte:
								return MakeLambdaFromSimpleFunc((SByte x) => Math.Sign(x), invokeTypes);
							case TypeCode.Single:
								return MakeLambdaFromSimpleFunc((Single x) => Math.Sign(x), invokeTypes);
							case TypeCode.UInt16:
								return MakeLambdaFromSimpleFunc((UInt16 x) => Math.Sign(x), invokeTypes);
							case TypeCode.UInt32:
								return MakeLambdaFromSimpleFunc((UInt32 x) => Math.Sign(x), invokeTypes);
							case TypeCode.UInt64:
								return MakeLambdaFromSimpleFunc((UInt64 x) => x > 0 ? 1 : 0, invokeTypes);
						}
					}
					return MakeLambdaFromSimpleFunc((int x) => Math.Sign(x), invokeTypes);
				case FunctionOperatorType.Round:
					if(invokeTypes.Length == 1) {
						LambdaExpression twoArgsLambda = MakeFnLambda(FunctionOperatorType.Round, new Type[] { invokeTypes[0], typeof(int?) }, caseSensitive);
						ParameterExpression op = Expression.Parameter(invokeTypes[0], "q");
						return Expression.Lambda(Expression.Invoke(twoArgsLambda, op, Expression.Constant(null, typeof(int?))), op);
					}
					if(invokeTypes.Length >= 2) {
						if(invokeTypes[0] == typeof(object))
							return MakeLambdaFromSimpleFunc((object x, int? precision) => FnRoundObject(x, precision), invokeTypes);
						switch(DXTypeExtensions.GetTypeCode(NullableHelpers.GetBoxedType(invokeTypes[0]))) {
							case TypeCode.Byte:
							case TypeCode.Char:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.SByte:
							case TypeCode.UInt16:
							case TypeCode.UInt32:
							case TypeCode.UInt64:
								ParameterExpression q = Expression.Parameter(invokeTypes[0], "q");
								return Expression.Lambda(q, q);
							case TypeCode.Decimal:
								return MakeLambdaFromSimpleFunc((Decimal x, int? precision) => Math.Round(x, precision ?? 0), invokeTypes);
						}
					}
					return MakeLambdaFromSimpleFunc((Double x, int? precision) => Math.Round(x, precision ?? 0), invokeTypes);
				case FunctionOperatorType.Ceiling:
					if(invokeTypes.Length >= 1) {
						if(invokeTypes[0] == typeof(object))
							return MakeLambdaFromSimpleFunc((object x) => FnCeilingObject(x), invokeTypes);
						switch(DXTypeExtensions.GetTypeCode(NullableHelpers.GetBoxedType(invokeTypes[0]))) {
							case TypeCode.Byte:
							case TypeCode.Char:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.SByte:
							case TypeCode.UInt16:
							case TypeCode.UInt32:
							case TypeCode.UInt64:
								ParameterExpression q = Expression.Parameter(invokeTypes[0], "q");
								return Expression.Lambda(q, q);
							case TypeCode.Decimal:
#if SL
								return MakeLambdaFromSimpleFunc((Decimal x, int? precision) => (Decimal)Math.Ceiling((Double)x), invokeTypes);
#else
								return MakeLambdaFromSimpleFunc((Decimal x, int? precision) => Math.Ceiling(x), invokeTypes);
#endif
						}
					}
					return MakeLambdaFromSimpleFunc((Double x) => Math.Ceiling(x), invokeTypes);
				case FunctionOperatorType.Floor:
					if(invokeTypes.Length >= 1) {
						if(invokeTypes[0] == typeof(object))
							return MakeLambdaFromSimpleFunc((object x) => FnFloorObject(x), invokeTypes);
						switch(DXTypeExtensions.GetTypeCode(NullableHelpers.GetBoxedType(invokeTypes[0]))) {
							case TypeCode.Byte:
							case TypeCode.Char:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.SByte:
							case TypeCode.UInt16:
							case TypeCode.UInt32:
							case TypeCode.UInt64:
								ParameterExpression q = Expression.Parameter(invokeTypes[0], "q");
								return Expression.Lambda(q, q);
							case TypeCode.Decimal:
#if SL
								return MakeLambdaFromSimpleFunc((Decimal x, int? precision) => (Decimal)Math.Floor((Double)x), invokeTypes);
#else
								return MakeLambdaFromSimpleFunc((Decimal x, int? precision) => Math.Floor(x), invokeTypes);
#endif
						}
					}
					return MakeLambdaFromSimpleFunc((Double x) => Math.Floor(x), invokeTypes);
				case FunctionOperatorType.Max:
					return MakeMinMax(invokeTypes, false, caseSensitive);
				case FunctionOperatorType.Min:
					return MakeMinMax(invokeTypes, true, caseSensitive);
				case FunctionOperatorType.Acos:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Acos(x), invokeTypes);
				case FunctionOperatorType.Asin:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Asin(x), invokeTypes);
				case FunctionOperatorType.Atn2:
					return MakeLambdaFromSimpleFunc((Double y, Double x) => Math.Atan2(y, x), invokeTypes);
				case FunctionOperatorType.BigMul:
					return MakeLambdaFromSimpleFunc((long a, long b) => a * b, invokeTypes);
				case FunctionOperatorType.Cosh:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Cosh(x), invokeTypes);
				case FunctionOperatorType.Log10:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Log10(x), invokeTypes);
				case FunctionOperatorType.Sinh:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Sinh(x), invokeTypes);
				case FunctionOperatorType.Tanh:
					return MakeLambdaFromSimpleFunc((Double x) => Math.Tanh(x), invokeTypes);
				case FunctionOperatorType.PadLeft:
					if(invokeTypes.Length == 2)
						return MakeLambdaFromSimpleFunc((string str, int? totalLength) => str != null && totalLength.HasValue ? str.PadLeft(totalLength.Value) : null, invokeTypes);
					else
						return MakeLambdaFromSimpleFunc((string str, int? totalLength, string padChar) => str != null && totalLength.HasValue && !string.IsNullOrEmpty(padChar) ? str.PadLeft(totalLength.Value, padChar[0]) : null, invokeTypes);
				case FunctionOperatorType.PadRight:
					if(invokeTypes.Length == 2)
						return MakeLambdaFromSimpleFunc((string str, int? totalLength) => str != null && totalLength.HasValue ? str.PadRight(totalLength.Value) : null, invokeTypes);
					else
						return MakeLambdaFromSimpleFunc((string str, int? totalLength, string padChar) => str != null && totalLength.HasValue && !string.IsNullOrEmpty(padChar) ? str.PadRight(totalLength.Value, padChar[0]) : null, invokeTypes);
				case FunctionOperatorType.StartsWith:
					Func<string, string, bool?> startsWith;
					if(caseSensitive)
						startsWith = FnStartsWithCaseSensitive;
					else
						startsWith = FnStartsWithCaseInsensitive;
					return MakeLambdaFromSimpleFunc(startsWith, invokeTypes, true);
				case FunctionOperatorType.EndsWith:
					Func<string, string, bool?> endsWith;
					if(caseSensitive)
						endsWith = FnEndsWithCaseSensitive;
					else
						endsWith = FnEndsWithCaseInsensitive;
					return MakeLambdaFromSimpleFunc(endsWith, invokeTypes, true);
				case FunctionOperatorType.Contains:
					Func<string, string, bool?> contains;
					if(caseSensitive)
						contains = FnContainsCaseSensitive;
					else
						contains = FnContainsCaseInsensitive;
					return MakeLambdaFromSimpleFunc(contains, invokeTypes, true);
				case FunctionOperatorType.ToInt:
					if(invokeTypes.Length != 1 || invokeTypes[0] == typeof(object) || invokeTypes[0] == typeof(string))
						return MakeLambdaFromSimpleFunc((object x) => x == null ? (int?)null : Convert.ToInt32(x, CultureInfo.InvariantCulture), invokeTypes);
					else {
						Type boxedType = NullableHelpers.GetBoxedType(invokeTypes[0]);
						ParameterExpression p = Expression.Parameter(boxedType, "q");
						LambdaExpression l = Expression.Lambda(Expression.Convert(p, typeof(Int32)), p);
						return MakeLambdaFromSimpleFuncCore(l.Compile(), invokeTypes, false);
					}
				case FunctionOperatorType.ToLong:
					if(invokeTypes.Length != 1 || invokeTypes[0] == typeof(object) || invokeTypes[0] == typeof(string))
						return MakeLambdaFromSimpleFunc((object x) => x == null ? (Int64?)null : Convert.ToInt64(x, CultureInfo.InvariantCulture), invokeTypes);
					else {
						Type boxedType = NullableHelpers.GetBoxedType(invokeTypes[0]);
						ParameterExpression p = Expression.Parameter(boxedType, "q");
						LambdaExpression l = Expression.Lambda(Expression.Convert(p, typeof(Int64)), p);
						return MakeLambdaFromSimpleFuncCore(l.Compile(), invokeTypes, false);
					}
				case FunctionOperatorType.ToFloat:
					if(invokeTypes.Length != 1 || invokeTypes[0] == typeof(object) || invokeTypes[0] == typeof(string))
						return MakeLambdaFromSimpleFunc((object x) => x == null ? (Single?)null : Convert.ToSingle(x, CultureInfo.InvariantCulture), invokeTypes);
					else {
						Type boxedType = NullableHelpers.GetBoxedType(invokeTypes[0]);
						ParameterExpression p = Expression.Parameter(boxedType, "q");
						LambdaExpression l = Expression.Lambda(Expression.Convert(p, typeof(Single)), p);
						return MakeLambdaFromSimpleFuncCore(l.Compile(), invokeTypes, false);
					}
				case FunctionOperatorType.ToDouble:
					if(invokeTypes.Length != 1 || invokeTypes[0] == typeof(object) || invokeTypes[0] == typeof(string))
						return MakeLambdaFromSimpleFunc((object x) => x == null ? (Double?)null : Convert.ToDouble(x, CultureInfo.InvariantCulture), invokeTypes);
					else {
						Type boxedType = NullableHelpers.GetBoxedType(invokeTypes[0]);
						ParameterExpression p = Expression.Parameter(boxedType, "q");
						LambdaExpression l = Expression.Lambda(Expression.Convert(p, typeof(Double)), p);
						return MakeLambdaFromSimpleFuncCore(l.Compile(), invokeTypes, false);
					}
				case FunctionOperatorType.ToDecimal:
					if(invokeTypes.Length != 1 || invokeTypes[0] == typeof(object) || invokeTypes[0] == typeof(string))
						return MakeLambdaFromSimpleFunc((object x) => x == null ? (Decimal?)null : Convert.ToDecimal(x, CultureInfo.InvariantCulture), invokeTypes);
					else {
						Type boxedType = NullableHelpers.GetBoxedType(invokeTypes[0]);
						ParameterExpression p = Expression.Parameter(boxedType, "q");
						LambdaExpression l = Expression.Lambda(Expression.Convert(p, typeof(Decimal)), p);
						return MakeLambdaFromSimpleFuncCore(l.Compile(), invokeTypes, false);
					}
				case FunctionOperatorType.LocalDateTimeThisYear:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeThisYear(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeThisMonth:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeThisMonth(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeLastWeek:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeLastWeek(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeThisWeek:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeThisWeek(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeYesterday:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeYesterday(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeToday:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeToday(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeNow:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeNow(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeTomorrow:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeTomorrow(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeDayAfterTomorrow(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeNextWeek:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeNextWeek(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeTwoWeeksAway(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeNextMonth:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeNextMonth(), invokeTypes);
				case FunctionOperatorType.LocalDateTimeNextYear:
					return MakeLambdaFromSimpleFunc(() => FnLocalDateTimeNextYear(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeNextYear(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeNextMonth() && dt < FnLocalDateTimeNextYear(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeTwoWeeksAway() && dt < FnLocalDateTimeNextMonth(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeNextWeek() && dt < FnLocalDateTimeTwoWeeksAway(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeDayAfterTomorrow() && dt < FnLocalDateTimeNextWeek(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeTomorrow() && dt < FnLocalDateTimeDayAfterTomorrow(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalToday:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeToday() && dt < FnLocalDateTimeTomorrow(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalYesterday:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeYesterday() && dt < FnLocalDateTimeToday(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeThisWeek() && dt < FnLocalDateTimeYesterday(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeLastWeek() && dt < FnLocalDateTimeThisWeek(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeThisMonth() && dt < FnLocalDateTimeLastWeek(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeThisYear() && dt < FnLocalDateTimeThisMonth(), invokeTypes);
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt < FnLocalDateTimeThisYear(), invokeTypes);
				case FunctionOperatorType.IsThisWeek:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeThisWeek() && dt < FnLocalDateTimeNextWeek(), invokeTypes);
				case FunctionOperatorType.IsThisMonth:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeThisMonth() && dt < FnLocalDateTimeNextMonth(), invokeTypes);
				case FunctionOperatorType.IsThisYear:
					return MakeLambdaFromSimpleFunc((DateTime dt) => dt >= FnLocalDateTimeThisYear() && dt < FnLocalDateTimeNextYear(), invokeTypes);
				case FunctionOperatorType.DateDiffTick:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => op2.Ticks - op1.Ticks, invokeTypes);
				case FunctionOperatorType.DateDiffSecond:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffSecond(op1, op2), invokeTypes);
				case FunctionOperatorType.DateDiffMilliSecond:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffMilliSecond(op1, op2), invokeTypes);
				case FunctionOperatorType.DateDiffMinute:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffMinute(op1, op2), invokeTypes);
				case FunctionOperatorType.DateDiffHour:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffHour(op1, op2), invokeTypes);
				case FunctionOperatorType.DateDiffDay:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffDay(op1, op2), invokeTypes);
				case FunctionOperatorType.DateDiffMonth:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffMonth(op1, op2), invokeTypes);
				case FunctionOperatorType.DateDiffYear:
					return MakeLambdaFromSimpleFunc((DateTime op1, DateTime op2) => FnDiffYear(op1, op2), invokeTypes);
				case FunctionOperatorType.GetDate:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Date, invokeTypes);
				case FunctionOperatorType.GetMilliSecond:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Millisecond, invokeTypes);
				case FunctionOperatorType.GetSecond:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Second, invokeTypes);
				case FunctionOperatorType.GetMinute:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Minute, invokeTypes);
				case FunctionOperatorType.GetHour:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Hour, invokeTypes);
				case FunctionOperatorType.GetDay:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Day, invokeTypes);
				case FunctionOperatorType.GetMonth:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Month, invokeTypes);
				case FunctionOperatorType.GetYear:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.Year, invokeTypes);
				case FunctionOperatorType.GetDayOfWeek:
					return MakeLambdaFromSimpleFunc((DateTime d) => (int)d.DayOfWeek, invokeTypes);
				case FunctionOperatorType.GetDayOfYear:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.DayOfYear, invokeTypes);
				case FunctionOperatorType.GetTimeOfDay:
					return MakeLambdaFromSimpleFunc((DateTime d) => d.TimeOfDay.Ticks, invokeTypes);
				case FunctionOperatorType.Now:
					return MakeLambdaFromSimpleFunc(() => DateTime.Now, invokeTypes);
				case FunctionOperatorType.UtcNow:
					return MakeLambdaFromSimpleFunc(() => DateTime.UtcNow, invokeTypes);
				case FunctionOperatorType.Today:
					return MakeLambdaFromSimpleFunc(() => DateTime.Today, invokeTypes);
				case FunctionOperatorType.AddTimeSpan:
					return MakeLambdaFromSimpleFunc((DateTime dt, TimeSpan ts) => dt.Add(ts), invokeTypes);
				case FunctionOperatorType.AddTicks:
					return MakeLambdaFromSimpleFunc((DateTime dt, long ticks) => dt.AddTicks(ticks), invokeTypes);
				case FunctionOperatorType.AddMilliSeconds:
					return MakeLambdaFromSimpleFunc((DateTime dt, double ms) => dt.AddMilliseconds(ms), invokeTypes);
				case FunctionOperatorType.AddSeconds:
					return MakeLambdaFromSimpleFunc((DateTime dt, double s) => dt.AddSeconds(s), invokeTypes);
				case FunctionOperatorType.AddMinutes:
					return MakeLambdaFromSimpleFunc((DateTime dt, double m) => dt.AddMinutes(m), invokeTypes);
				case FunctionOperatorType.AddHours:
					return MakeLambdaFromSimpleFunc((DateTime dt, double h) => dt.AddHours(h), invokeTypes);
				case FunctionOperatorType.AddDays:
					return MakeLambdaFromSimpleFunc((DateTime dt, double d) => dt.AddDays(d), invokeTypes);
				case FunctionOperatorType.AddMonths:
					return MakeLambdaFromSimpleFunc((DateTime dt, int m) => dt.AddMonths(m), invokeTypes);
				case FunctionOperatorType.AddYears:
					return MakeLambdaFromSimpleFunc((DateTime dt, int y) => dt.AddYears(y), invokeTypes);
			}
			throw new NotImplementedException(functionOperatorType.ToString());
		}
		static LambdaExpression MakeLambdaFromSimpleFunc<T>(Func<T> baseLambda, Type[] invokeTypes, bool logicalResult = false) {
			return MakeLambdaFromSimpleFuncCore((Delegate)baseLambda, invokeTypes, logicalResult);
		}
		static LambdaExpression MakeLambdaFromSimpleFunc<X, T>(Func<X, T> baseLambda, Type[] invokeTypes, bool logicalResult = false) {
			return MakeLambdaFromSimpleFuncCore((Delegate)baseLambda, invokeTypes, logicalResult);
		}
		static LambdaExpression MakeLambdaFromSimpleFunc<X, Y, T>(Func<X, Y, T> baseLambda, Type[] invokeTypes, bool logicalResult = false) {
			return MakeLambdaFromSimpleFuncCore((Delegate)baseLambda, invokeTypes, logicalResult);
		}
		static LambdaExpression MakeLambdaFromSimpleFunc<X, Y, Z, T>(Func<X, Y, Z, T> baseLambda, Type[] invokeTypes, bool logicalResult = false) {
			return MakeLambdaFromSimpleFuncCore((Delegate)baseLambda, invokeTypes, logicalResult);
		}
		static LambdaExpression MakeLambdaFromSimpleFunc<W, X, Y, Z, T>(Func<W, X, Y, Z, T> baseLambda, Type[] invokeTypes, bool logicalResult = false) {
			return MakeLambdaFromSimpleFuncCore((Delegate)baseLambda, invokeTypes, logicalResult);
		}
		static LambdaExpression MakeLambdaFromSimpleFuncCore(Delegate baseFunc, Type[] invokeTypes, bool logicalResult) {
			Type[] baseArguments = baseFunc.GetType().GetGenericArguments();
			Type baseReturnType = baseArguments[baseArguments.Length - 1];
			Array.Resize(ref baseArguments, baseArguments.Length - 1);
			if(baseArguments.Length != invokeTypes.Length)
				throw new InvalidOperationException("invalid operands count: " + invokeTypes.Length.ToString() + ", expected: " + (baseArguments.Length).ToString());
			ParameterExpression[] pars = invokeTypes.Select((x, i) => Expression.Parameter(x, "p" + i.ToString())).ToArray();
			List<Expression> invokeArgs = new List<Expression>(pars.Length);
			List<Expression> badArgs = new List<Expression>();
			for(int i = 0; i < pars.Length; ++i) {
				Expression arg = pars[i];
				Type destinationType = baseArguments[i];
				if(NullableHelpers.CanAcceptNull(arg.Type) && !NullableHelpers.CanAcceptNull(destinationType))
					badArgs.Add(arg);
				if(!destinationType.IsAssignableFrom(arg.Type)) {
					if(destinationType == typeof(string)) {
						arg = SafeToString(arg);
					} else if(arg.Type == typeof(object)) {
						arg = SafeObjectToType(arg, destinationType);
					} else {
						arg = Expression.Convert(arg, destinationType);
					}
				} else if(destinationType == typeof(object) && arg.Type != destinationType) {
					arg = Expression.Convert(arg, destinationType);
				} else if(Nullable.GetUnderlyingType(destinationType) == arg.Type) {
					arg = Expression.Convert(arg, destinationType);
				}
				invokeArgs.Add(arg);
			}
			Expression coreInvoke = Expression.Invoke(Expression.Constant(baseFunc), invokeArgs);
			if(badArgs.Count == 0) {
				if(logicalResult && baseReturnType == typeof(bool?))
					return Expression.Lambda(Expression.Coalesce(coreInvoke, Expression.Constant(false)), pars);
				return Expression.Lambda(coreInvoke, pars);
			}
			Expression conditionArg = null;
			foreach(Expression isNullExpression in badArgs.Select(e => MakeIsNull(e))) {
				if(conditionArg == null)
					conditionArg = isNullExpression;
				else
					conditionArg = Expression.OrElse(isNullExpression, conditionArg);
			}
			if(!baseReturnType.IsValueType())
				throw new InvalidOperationException("Internal error: one of the arguments is a value type; baseReturnType is not a value type");
			Type returnType = typeof(Nullable<>).MakeGenericType(baseReturnType);
			Expression body = Expression.Condition(conditionArg, Expression.Constant(null, returnType), coreInvoke.Type == returnType ? coreInvoke : Expression.Convert(coreInvoke, returnType));
			return Expression.Lambda(body, pars);
		}
		static Expression SafeObjectToType(Expression arg, Type destinationType) {
			if(arg.Type != typeof(object))
				throw new InvalidOperationException("arg.Type != typeof(object)");
			if(destinationType == typeof(object)) {
				if(arg.Type == typeof(object))
					return arg;
				else
					return Expression.Convert(arg, typeof(object));
			}
			if(destinationType == typeof(string))
				return SafeToString(arg);
			Type boxedType = NullableHelpers.GetBoxedType(destinationType);
			TypeCode tc = DXTypeExtensions.GetTypeCode(boxedType);
			switch(tc) {
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					break;
				default:
					return Expression.Convert(arg, destinationType);
			}
			if(boxedType == destinationType)
				return Expression.Call(typeof(Convert), "To" + tc.ToString(), null, arg);
			ParameterExpression p = Expression.Parameter(typeof(object), "q");
			Expression coreTo = Expression.Call(typeof(Convert), "To" + tc.ToString(), null, p);
			Expression body = Expression.Condition(MakeIsNull(p), Expression.Constant(null, destinationType), Expression.Convert(coreTo, destinationType));
			LambdaExpression lmb = Expression.Lambda(body, p);
			return Expression.Invoke(lmb, arg);
		}
		static Expression MakeIsNull(Expression expr) {
			if(expr.Type.IsValueType()) {
				if(Nullable.GetUnderlyingType(expr.Type) == null)
					throw new InvalidOperationException("expr is of a value type " + expr.Type.FullName);
				return Expression.Not(Expression.PropertyOrField(expr, "HasValue"));
			} else {
				return Expression.Call(typeof(object), "ReferenceEquals", null, expr, Expression.Constant(null));
			}
		}
		static Expression MakeIsNotNull(Expression expr) {
			if(expr.Type.IsValueType()) {
				if(Nullable.GetUnderlyingType(expr.Type) == null)
					throw new InvalidOperationException("expr is of a value type " + expr.Type.FullName);
				return Expression.PropertyOrField(expr, "HasValue");
			} else {
				return Expression.Not(Expression.Call(typeof(object), "ReferenceEquals", null, expr, Expression.Constant(null)));
			}
		}
	}
}
