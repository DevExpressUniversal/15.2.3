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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public class CSharpPrimitiveTypeUtils
	{
		const string STR_0X = "0X";
		const string STR_0x = "0x";
		const string STR_L = "L";
		const string STR_U = "u";
		const string STR_Lu = "lu";
		const string STR_Ul = "ul";
		const string STR_F = "f";
		const string STR_D = "d";
		const string STR_M = "m";
		private CSharpPrimitiveTypeUtils() { }
		private static bool IsHexLiteral(string value)
		{
			return value.StartsWith(STR_0x) || value.StartsWith(STR_0X);
		}
		private static bool ConvertToDecimal(string value, out decimal result)
		{
			result = -1;
			try
			{
				decimal lValue;
				if (IsHexLiteral(value))
				{
					lValue = Convert.ToUInt64(value.Substring(2), 16);
					result = lValue;
					return true;
				}
				lValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture.NumberFormat);
				result = lValue;
			}
			catch
			{
				return false;
			}
			return true;
		}
		private static PrimitiveType GetIntegerTypeFromValue(string value)
		{
			decimal lValue;
			if (!ConvertToDecimal(value, out lValue))
				return PrimitiveType.Int32;
			return PrimitiveTypeUtils.GetPrimitiveType(lValue);
		}
		private static string GetIntegerLiteralSuffix(string value)
		{
			string lValue = value.ToLower();
			if (EndsWith(lValue, STR_Ul))
				return STR_Ul;
			else if (EndsWith(lValue, STR_Lu))
				return STR_Lu;
			else if (EndsWith(lValue, STR_U))
				return STR_U;
			else if (EndsWith(lValue, STR_L))
				return STR_L;
			return String.Empty;
		}
		private static string GetFloatingPointLiteralSuffix(string value)
		{
			string lValue = value.ToLower();
			if (EndsWith(lValue, STR_F))
				return STR_F;
			else if (EndsWith(lValue, STR_D))
				return STR_D;
			else if (EndsWith(lValue, STR_M))
				return STR_M;
			return String.Empty;
		}
		private static bool GetIntegerLiteralValue(string value, out decimal intValue)
		{
			intValue = -1;
			if (value == null || value.Length == 0)
				return false;
			decimal lIntValue;
			string lSuffix = GetIntegerLiteralSuffix(value);
			if (lSuffix != null && lSuffix.Length > 0)
			{
				string lValue = value.Substring(0, value.Length - lSuffix.Length);
				if (!ConvertToDecimal(lValue, out lIntValue))
					return false;
				intValue = lIntValue;
				return true;
			}
			if (!ConvertToDecimal(value, out lIntValue))
				return false;
			intValue = lIntValue;
			return true;
		}
		private static bool GetFloatingPointLiteralValue(string value, out decimal realValue)
		{
			realValue = -1;
			if (value == null || value.Length == 0)
				return false;
			decimal lRealValue;
			string lSuffix = GetFloatingPointLiteralSuffix(value);
			if (lSuffix != null && lSuffix.Length > 0)
			{
				string lValue = value.Substring(0, value.Length - lSuffix.Length);
				if (!ConvertToDecimal(lValue, out lRealValue))
					return false;
				realValue = lRealValue;
				return true;
			}
			if (!ConvertToDecimal(value, out lRealValue))
				return false;
			realValue = lRealValue;
			return true;
		}
		private static PrimitiveType NarrowIntegerLiteralType(string value)
		{
			if (value == null || value.Length == 0)
				return PrimitiveType.Int32;
			decimal lIntValue;
			if (!GetIntegerLiteralValue(value, out lIntValue))
				return PrimitiveType.Int32;
			string lSuffix = GetIntegerLiteralSuffix(value);
			if (lSuffix != null && lSuffix.Length > 0)
				return NarrowIntegerLiteralType(lSuffix, lIntValue);
			PrimitiveType lType = GetIntegerTypeFromValue(value);
			switch (lType)
			{
				case PrimitiveType.SByte:
				case PrimitiveType.Byte:
				case PrimitiveType.Int16:
				case PrimitiveType.UInt16:
				case PrimitiveType.Int32:
					return PrimitiveType.Int32;
			}
			if (lType == PrimitiveType.UInt32 || lType == PrimitiveType.UInt64)
				return lType;
			return PrimitiveType.Int64;
		}
		private static PrimitiveType NarrowIntegerLiteralType(string suffix, decimal intValue)
		{
			if (suffix == null && suffix.Length == 0)
				return PrimitiveType.Int32;
			if (suffix == STR_Ul || suffix == STR_Lu)
				return PrimitiveType.UInt64;
			if (suffix == STR_U)
			{
				if (PrimitiveTypeUtils.IsUInt(intValue))
					return PrimitiveType.UInt32;
				else if (PrimitiveTypeUtils.IsULong(intValue))
					return PrimitiveType.UInt64;
			}
			if (suffix == STR_L)
			{
				if (PrimitiveTypeUtils.IsLong(intValue))
					return PrimitiveType.Int64;
		else if  (PrimitiveTypeUtils.IsULong(intValue))
					return PrimitiveType.UInt64;
			}
			return PrimitiveType.Int32;
		}
		static bool EndsWith(string str, string strEnd)
		{
			if (str == null || strEnd == null)
				return false;
			return str.EndsWith(strEnd, StringComparison.CurrentCultureIgnoreCase);
		}
		private static PrimitiveType NarrowFloatingPointLiteralType(string value)
		{
			string lSuffix = GetFloatingPointLiteralSuffix(value);
			if (lSuffix == null || lSuffix.Length == 0)
				return PrimitiveType.Double;
			switch (lSuffix)
			{
				case STR_F:
					return PrimitiveType.Single;
				case STR_D:
					return PrimitiveType.Double;
				case STR_M:
					return PrimitiveType.Decimal;
			}
			return PrimitiveType.Double;
		}
		private static string RemoveQuotes(string s)
		{
			if (s == null || s.Length == 0)
				return String.Empty;
			if (s == "@\"\"")
				return String.Empty;
			if (s == "\"\"")
				return String.Empty;
			int lStartIndex = 1;
			if (s.StartsWith("@\""))
				lStartIndex = 2;
			int lLength = s.Length - lStartIndex - 1;
			s = s.Substring(lStartIndex, lLength);
			return ReplaceEscapes(s);
		}
		private static string RemoveCharQuotes(string s)
		{
			if (s == null || s.Length == 0)
				return String.Empty;
			if (s == "''")
				return String.Empty;
			int lStartIndex = 1;
			int lLength = s.Length - lStartIndex - 1;
			s = s.Substring(lStartIndex, lLength);
			return ReplaceEscapes(s);
		}
		static int GetEscapeSequenceIndex(string s, int startIndex)
		{
			if (startIndex == -1)
				return -1;
			string[] escapeSequencies = { @"\\'", @"\\\""", @"\\r", @"\\n", @"\\a", @"\\0",
																		@"\\b", @"\\f", @"\\t", @"\\v", @"\\U", @"\\x", @"\\u"};
			for (int i = 0; i < escapeSequencies.Length; i++)
			{
 				int index = s.IndexOf(escapeSequencies[i], startIndex);
				if (index != -1)
					return index;
			}
			return -1;
		}
		static string ReplaceEscapes(string s)
		{
			if (s == null || s.Length == 0)
				return s;
			if (s.StartsWith("@"))
				return s;
			int doubleSlashIndex = s.IndexOf(@"\\");
			int escapeCharIndex = GetEscapeSequenceIndex(s, doubleSlashIndex);
			while (doubleSlashIndex >= 0)
			{
				if (doubleSlashIndex == escapeCharIndex)
					doubleSlashIndex++;
				else
				{
					s = s.Remove(doubleSlashIndex, 2);
					s = s.Insert(doubleSlashIndex, @"\");
				}
				doubleSlashIndex = s.IndexOf(@"\\", doubleSlashIndex);
				escapeCharIndex = GetEscapeSequenceIndex(s, doubleSlashIndex);
				if (doubleSlashIndex >= s.Length)
					return s;
				if (String.IsNullOrEmpty(s))
					return s;
			}
			return s;
		}
		public static PrimitiveType ToPrimitiveType(Token token)
		{
			return ToPrimitiveType(token, UnaryOperatorType.None);
		}
		public static PrimitiveType ToPrimitiveType(Token token, UnaryOperatorType typeOperator)
		{
			if (token == null)
				return PrimitiveType.Int32;
			return ToPrimitiveType(token.Type, token.EscapedValue, typeOperator);
		}
		public static PrimitiveType ToPrimitiveType(int tokenType, string value)
		{
			return ToPrimitiveType(tokenType, value, UnaryOperatorType.None);
		}
		public static PrimitiveType ToPrimitiveType(int tokenType, string value, UnaryOperatorType typeOperator)
		{
			if (value == null || value.Length == 0)
				return PrimitiveType.Void;
			if (typeOperator == UnaryOperatorType.UnaryNegation)
				value = "-" + value;
			switch (tokenType)
			{
				case Tokens.TRUE:
				case Tokens.FALSE:
					return PrimitiveType.Boolean;
				case Tokens.NULL:
					return PrimitiveType.Void;
				case Tokens.STRINGCON:
					return PrimitiveType.String;
				case Tokens.CHARCON:
					return PrimitiveType.Char;
				case Tokens.INTCON:
					return NarrowIntegerLiteralType(value);
				case Tokens.REALCON:
					return NarrowFloatingPointLiteralType(value);
			}
			return PrimitiveType.Int32;
		}
		public static object ToPrimitiveValue(Token token)
		{
			if (token == null)
				return null;
			return ToPrimitiveValue(token.Type, token.EscapedValue);
		}
		public static object ToPrimitiveValue(int tokenType, string value)
		{
			if (value == null || value.Length == 0)
				return PrimitiveType.Void;
			switch (tokenType)
			{
				case Tokens.TRUE:
					return true;
				case Tokens.FALSE:
					return false;
				case Tokens.NULL:
					return null;
				case Tokens.STRINGCON:
					return RemoveQuotes(value);
				case Tokens.CHARCON:
				{
					string charValue = RemoveCharQuotes(value);
					if (charValue.Length == 1)
						return Convert.ToChar(charValue);
					else
						return charValue;
				}
				case Tokens.INTCON:
					decimal lIntValue;
					if (!GetIntegerLiteralValue(value, out lIntValue))
						return null;
					return lIntValue;
				case Tokens.REALCON:
					decimal lRealValue;
					if (!GetFloatingPointLiteralValue(value, out lRealValue))
						return null;
					return lRealValue;
			}
			return PrimitiveType.Int32;
		}
		public static string GetTypeSuffix(object value, PrimitiveType type)
		{
			if (type == PrimitiveType.SByte)
				return String.Empty;
			if (type == PrimitiveType.Byte)
				return String.Empty;
			if (type == PrimitiveType.Int32)
				return String.Empty;
			if (type == PrimitiveType.Int16)
				return String.Empty;
			if (type == PrimitiveType.UInt32)
				return STR_U;
			if (type == PrimitiveType.UInt16)
				return String.Empty;
			if (type == PrimitiveType.UInt64)
				return STR_Ul;
			if (type == PrimitiveType.Int64)
				return STR_L;
			if (type == PrimitiveType.Single)
				return STR_F;
			if (type == PrimitiveType.Double)
				return STR_D;
			if (type == PrimitiveType.Decimal)
				return STR_M;
			return GetTypeSuffix(value);
		}
		static string GetTypeSuffix(object value)
		{
			if (value == null)
				return String.Empty;
			if (value is SByte)
				return String.Empty;
			if (value is Byte)
				return String.Empty;
			if (value is Int16)
				return String.Empty;
			if (value is Int32)
				return String.Empty;
			if (value is UInt32)
				return STR_U;
			if (value is UInt16)
				return String.Empty;
			if (value is UInt64)
				return STR_Ul;
			if (value is Int64)
				return STR_L;
			if (value is Single)
				return STR_F;
			if (value is Double)
				return STR_D;
			if (value is Decimal)
				return STR_M;
			return String.Empty;
		}
	}
}
