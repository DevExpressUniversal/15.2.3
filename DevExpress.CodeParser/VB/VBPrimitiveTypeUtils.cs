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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class VBPrimitiveTypeUtils
	{
		const string STR_H = "&H";
		const string STR_h = "&h";
		const string STR_O = "&O";
		const string STR_o = "&o";
		const string STR_S = "s";
		const string STR_US = "US";
		const string STR_uS = "uS";
		const string STR_Us = "Us";
		const string STR_us = "us";
		const string STR_I = "i";
		const string STR_UI = "UI";
		const string STR_uI = "uI";
		const string STR_Ui = "Ui";
		const string STR_ui = "ui";
		const string STR_L = "L";
		const string STR_UL = "UL";
		const string STR_uL = "uL";
		const string STR_Ul = "Ul";
		const string STR_ul = "ul";
		const string STR_F = "f";
		const string STR_R = "r";
		const string STR_D = "d";
		const string STR_F_ = "!";
		const string STR_R_ = "#";
		const string STR_D_ = "@";
		private VBPrimitiveTypeUtils() { }
		static bool ConvertToDecimal(string value, out decimal result)
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
				if (IsOctalLiteral(value))
				{
					lValue = Convert.ToUInt64(value.Substring(2), 8);
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
		static PrimitiveType GetIntegerTypeFromValue(string value)
		{
			decimal lValue;
			if (!ConvertToDecimal(value, out lValue))
				return PrimitiveType.Int32;
			return PrimitiveTypeUtils.GetPrimitiveType(lValue);
		}
		static bool EndsWith(string str, string strEnd)
		{
			if (str == null || strEnd == null)
				return false;
			return str.EndsWith(strEnd, StringComparison.CurrentCultureIgnoreCase);
		}
		static string GetIntegerLiteralSuffix(string value)
		{
			string lValue = value.ToLower();
			if (EndsWith(lValue, STR_US))
				return STR_US;
			if (EndsWith(lValue, STR_uS))
				return STR_uS;
			if (EndsWith(lValue, STR_Us))
				return STR_Us;
			if (EndsWith(lValue, STR_us))
				return STR_us;
			if (EndsWith(lValue, STR_S))
				return STR_S;
			if (EndsWith(lValue, STR_UI))
				return STR_UI;
			if (EndsWith(lValue, STR_uI))
				return STR_uI;
			if (EndsWith(lValue, STR_Ui))
				return STR_Ui;
			if (EndsWith(lValue, STR_ui))
				return STR_ui;
			if (EndsWith(lValue, STR_I))
				return STR_I;
			if (EndsWith(lValue, STR_UL))
				return STR_UL;
			if (EndsWith(lValue, STR_uL))
				return STR_uL;
			if (EndsWith(lValue, STR_Ul))
				return STR_Ul;
			if (EndsWith(lValue, STR_ul))
				return STR_ul;
			if (EndsWith(lValue, STR_L))
				return STR_L;
			return String.Empty;
		}
		static string GetFloatingPointLiteralSuffix(string value)
		{
			string lValue = value.ToLower();
			if (EndsWith(lValue, STR_F))
				return STR_F;
			else if (EndsWith(lValue, STR_R))
				return STR_R;
			else if (EndsWith(lValue, STR_D))
				return STR_D;
			else if (EndsWith(lValue, STR_F_))
				return STR_F_;
			else if (EndsWith(lValue, STR_R_))
				return STR_R_;
			else if (EndsWith(lValue, STR_D_))
				return STR_D_;
			return String.Empty;
		}
		static bool GetIntegerLiteralValue(string value, out decimal intValue)
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
		static bool GetFloatingPointLiteralValue(string value, out decimal realValue)
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
		static public PrimitiveType NarrowIntegerLiteralType(string value)
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
				case PrimitiveType.UInt16:
				case PrimitiveType.Int16:
				case PrimitiveType.Int32:
					return PrimitiveType.Int32;
				default:
					return PrimitiveType.Int64;
			}
		}
		static PrimitiveType NarrowIntegerLiteralType(string suffix, decimal intValue)
		{
			if (suffix == null && suffix.Length == 0)
				return PrimitiveType.Int32;
			switch (suffix)
			{
				case STR_S:
					return PrimitiveType.Int16;
				case STR_US:
				case STR_uS:
				case STR_Us:
				case STR_us:
					return PrimitiveType.UInt16;
				case STR_I:
					return PrimitiveType.Int32;
				case STR_UI:
				case STR_uI:
				case STR_Ui:
				case STR_ui:
					return PrimitiveType.UInt32;
				case STR_L:
					return PrimitiveType.Int64;
				case STR_UL:
				case STR_uL:
				case STR_Ul:
				case STR_ul:
					return PrimitiveType.UInt64;
			}
			return PrimitiveType.Int32;
		}
		static public PrimitiveType NarrowFloatingPointLiteralType(string value)
		{
			string lSuffix = GetFloatingPointLiteralSuffix(value);
			if (lSuffix == null || lSuffix.Length == 0)
				return PrimitiveType.Double;
			switch (lSuffix)
			{
				case STR_F:
					return PrimitiveType.Single;
				case STR_R:
					return PrimitiveType.Double;
				case STR_D:
					return PrimitiveType.Decimal;
				case STR_F_:
					return PrimitiveType.Single;
				case STR_R_:
					return PrimitiveType.Double;
				case STR_D_:
					return PrimitiveType.Decimal;
			}
			return PrimitiveType.Double;
		}
	static public PrimitiveType NarrowCharAndStrignLiteralType(string value)
	{
	  if (value == null || value.Length == 0)
		return PrimitiveType.String;
	  if (HasCharSuffix(value))
		return PrimitiveType.Char;
	  return PrimitiveType.String;
	}
	static bool HasCharSuffix(string value)
	{
	  if(value.EndsWith("C") || value.EndsWith("c"))
		return true;
	  return false;
	}
		static string RemoveQuotes(string s)
		{
			if (s == null || s.Length == 0)
				return String.Empty;
			if (s == "\"\"")
				return String.Empty;
			int lStartIndex = 1;
			int lLength = s.Length - lStartIndex - 1;
			return s.Substring(lStartIndex, lLength);
		}
		static string RemoveCharQuotes(string s)
		{
			if (s == null || s.Length == 0)
				return String.Empty;
			if (s == "\"\"c")
				return String.Empty;
			if (s == "\"\"C")
				return String.Empty;
			int lStartIndex = 1;
			int lLength = s.Length - lStartIndex - 2;
			return s.Substring(lStartIndex, lLength);
		}		
		public static PrimitiveType ToPrimitiveType(Token token)
		{
			if (token == null)
				return PrimitiveType.Int32;
			return ToPrimitiveType(token.Type, token.EscapedValue);
		}
		public static PrimitiveType ToPrimitiveType(int tokenType, string value)
		{
			if (value == null || value.Length == 0)
				return PrimitiveType.Void;
			switch (tokenType)
			{
				case TokenType.True:
				case TokenType.False:
					return PrimitiveType.Boolean;
				case TokenType.Nothing:
					return PrimitiveType.Void;
				case TokenType.StringLiteral:
					return PrimitiveType.String;
				case TokenType.CharacterLiteral:
					return PrimitiveType.Char;
				case TokenType.DateLiteral:
					return PrimitiveType.DateTime;
				case TokenType.IntegerLiteral:
					return NarrowIntegerLiteralType(value);
				case TokenType.FloatingPointLiteral:
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
				case TokenType.True:
					return true;
				case TokenType.False:
					return false;
				case TokenType.Null:
					return null;
				case TokenType.StringLiteral:
					return RemoveQuotes(value);
				case TokenType.CharacterLiteral:
					return RemoveCharQuotes(value);
				case TokenType.DateLiteral:
					return DateTime.MinValue;
				case TokenType.IntegerLiteral:
					decimal lIntValue;
					if (!GetIntegerLiteralValue(value, out lIntValue))
						return null;
					return lIntValue;
				case TokenType.FloatingPointLiteral:
					decimal lRealValue;
					if (!GetFloatingPointLiteralValue(value, out lRealValue))
						return null;
					return lRealValue;
			}
			return null;
		}
		public static object ToPrimitiveValueFromPrimitiveType(PrimitiveType primitiveType, string value)
		{
			if (value == null || value.Length == 0)
				return PrimitiveType.Void;
			switch (primitiveType)
			{
				case PrimitiveType.Boolean:
					if (value.ToLower() == "TRUE")
					{
						return true;
					}
					else
					{
						return false;
					}
				case PrimitiveType.Void:
					return null;
				case PrimitiveType.String:
					return RemoveQuotes(value);
				case PrimitiveType.Char:
					return RemoveCharQuotes(value);
				case PrimitiveType.DateTime:
		  if (String.IsNullOrEmpty(value))
			return DateTime.MinValue;
		  DateTime dateResult = DateTime.MinValue;
		  if (DateTime.TryParse(value, out dateResult))
			return dateResult;
		  return DateTime.MinValue;
				case PrimitiveType.Int16:
				case PrimitiveType.Int32:
				case PrimitiveType.Int64:
					decimal lIntValue;
					if (!GetIntegerLiteralValue(value, out lIntValue))
						return null;
					return lIntValue;
				case PrimitiveType.Double:
					decimal lRealValue;
					if (!GetFloatingPointLiteralValue(value, out lRealValue))
						return null;
					return lRealValue;
			}
			return null;
		}
		public static bool IsHexLiteral(string value)
		{
			return value.StartsWith(STR_H) || value.StartsWith(STR_h);
		}
		public static bool IsOctalLiteral(string value)
		{
			return value.StartsWith(STR_O) || value.StartsWith(STR_o);
		}
		public static string GetTypeSuffix(object value, PrimitiveType type)
		{
			if (type == PrimitiveType.SByte)
				return String.Empty;
			if (type == PrimitiveType.Byte)
				return String.Empty;
			if (type == PrimitiveType.Int32)
				return String.Empty;
			if (type == PrimitiveType.UInt32)
				return STR_Ui;
			if (type == PrimitiveType.UInt16)
				return STR_Us;
			if (type == PrimitiveType.UInt64)
				return STR_Ul;
			if (type == PrimitiveType.Int16)
				return STR_S;
			if (type == PrimitiveType.Int64)
				return STR_L;
			if (type == PrimitiveType.Single)
				return STR_F;
			if (type == PrimitiveType.Double)
				return STR_R;
			if (type == PrimitiveType.Decimal)
				return STR_D;
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
			if (value is Int32)
				return String.Empty;
			if (value is UInt32)
				return STR_Ui;
			if (value is UInt16)
				return STR_Us;
			if (value is UInt64)
				return STR_Ul;
			if (value is Int16)
				return STR_S;
			if (value is Int64)
				return STR_L;
			if (value is Single)
				return STR_F;
			if (value is Double)
				return STR_R;
			if (value is Decimal)
				return STR_D;
			return String.Empty;
		}
	}
}
