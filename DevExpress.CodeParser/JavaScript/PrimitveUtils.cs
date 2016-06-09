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
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
	public class JavaScriptPrimitiveTypeUtils
	{
		const string STR_0X = "0X";
		const string STR_0x = "0x";
		private static bool IsHexLiteral(string value)
		{
			return value.StartsWith(STR_0x) || value.StartsWith(STR_0X);
		}
		private static bool IsOctLiteral(string value)
		{
			return value.Length > 1 && value.StartsWith("0");
		}
		private static bool ConvertToInt(string value, out Int32 result)
		{
			result = -1;
			try
			{
				Int32 lValue;
				if (IsHexLiteral(value))
				{
					lValue = Convert.ToInt32(value.Substring(2), 16);
					result = lValue;
					return true;
				}
				if (IsOctLiteral(value))
				{
					lValue = Convert.ToInt32(value.Substring(1), 8);
					result = lValue;
					return true;
				}
				lValue = Convert.ToInt32(value,System.Globalization.NumberFormatInfo.InvariantInfo);
				result = lValue;
			}
			catch
			{
				return false;
			}
			return true;
		}
		private static bool ConvertToDecimal(string value, out decimal result)
		{
			result = -1;
			try
			{
				decimal lValue;
				lValue = Convert.ToDecimal(value,System.Globalization.NumberFormatInfo.InvariantInfo);
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
		private static bool GetIntegerLiteralValue(string value, out Int32 intValue)
		{
			intValue = -1;
			if (value == null || value.Length == 0)
				return false;
			Int32 lIntValue;
			if (!ConvertToInt(value, out lIntValue))
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
			if (!ConvertToDecimal(value, out lRealValue))
				return false;
			realValue = lRealValue;
			return true;
		}
		private static int HexToInt(string s)
		{
			if (s == null || s.Length ==0)
				return 0;
			return Convert.ToInt32(s,16);
		}
		private static int OctToInt(string s)
		{
			if (s == null || s.Length ==0)
				return 0;
			return Convert.ToInt32(s,8);
		}
		private static string GetStringLiteralValue(string s)
		{
			if (s == null || s.Length == 0)
				return String.Empty;
			string tempStr = s;
			return RemoveQuotes(tempStr);
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
			return s.Substring(lStartIndex, lLength);
		}
		private static string RemoveCharQuotes(string s)
		{
			if (s == null || s.Length == 0)
				return String.Empty;
			if (s == "''")
				return String.Empty;
			int lStartIndex = 1;
			int lLength = s.Length - lStartIndex - 1;
			return s.Substring(lStartIndex, lLength);
		}		
		public static PrimitiveType ToPrimitiveType(int tokenType, string value)
		{
			if (value == null || value.Length == 0)
				return PrimitiveType.Void;
			switch (tokenType)
			{
				case Tokens.TRUE:
				case Tokens.FALSE:
					return PrimitiveType.Boolean;
				case Tokens.NULL:
					return PrimitiveType.Void;
				case Tokens.STRINGLITERAL:
					return PrimitiveType.String;
				case Tokens.INTEGERLITERAL:
					return PrimitiveType.Int32;
				case Tokens.DECIMALLITERAL:
					return PrimitiveType.Double;
			}
			return PrimitiveType.Int32;
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
				case Tokens.STRINGLITERAL:
					return GetStringLiteralValue(value);
				case Tokens.INTEGERLITERAL:
					Int32 lIntValue;
					if (!GetIntegerLiteralValue(value, out lIntValue))
						return 0;
					return lIntValue;
				case Tokens.DECIMALLITERAL:
					decimal lRealValue;
					if (!GetFloatingPointLiteralValue(value, out lRealValue))
						return 0.0;
					return lRealValue;
			}
			return PrimitiveType.Int32;
		}
	}
}
