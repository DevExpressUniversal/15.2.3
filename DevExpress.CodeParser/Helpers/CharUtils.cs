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
using System.Text;
using System.Globalization;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum SymbolType
	{
		Letter,
		Digit,
		WhiteSpace,
		Others,
		Unknown
	}
	public sealed class CharUtils
	{
		CharUtils() {}
		static SymbolType ResolveSymbol(Char ch)
		{
			if((ch >= 'a' && ch <= 'z')||(ch >= 'A' && ch <= 'Z'))
				return SymbolType.Letter;
			if(ch >= '0' && ch <= '9')
				return SymbolType.Digit;
			if(ch == ';'||ch == '.' ||ch == '{' ||ch == '}')
				return SymbolType.Others;
			switch ((int)ch)
			{
				case 0x0020:
				case 0x0009:
				case 0x000B:
				case 0x000C:
					return SymbolType.WhiteSpace;
			}
			return SymbolType.Unknown;
		}
		public static char Translate(char c)
		{
			if (c == (char)0x201C || c == (char)0x201D)
			{
				c = '"';
			}
			switch(c) 
			{
				case '\r':
				case '\n':
				case '\t':
				case '\b':
					return c;
			}
			if (c >= ' ' && c <= '~')
				return c;
			UnicodeCategory uc = char.GetUnicodeCategory(c);
			switch(uc) 
			{
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.LetterNumber:
					return 'w';
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.SpacingCombiningMark:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.ConnectorPunctuation:
				case UnicodeCategory.Format:
					return '$';
			}
			return '~';
		}		
		public static bool IsNewLine(char ch)
		{
			switch ((int)ch)
			{
				case 0x000D:
				case 0x000A:
				case 0x2028:
				case 0x2029:
					return true;
			}
			return false;
		}
		public static bool IsNewLine(string s)
		{
			switch (s)
			{
				case "\r":
				case "\n":
				case "\r\n":
				case "\u2028":
				case "\u2029":
					return true;
				default:
					return false;
			}
		}				
		public static bool IsWhiteSpace(char ch)
		{
			SymbolType type = ResolveSymbol(ch);
			if(type == SymbolType.WhiteSpace) return true;
			if(type != SymbolType.Unknown)
				return false;
			else
			{
				UnicodeCategory lCategory = Char.GetUnicodeCategory(ch);
				if (lCategory == UnicodeCategory.SpaceSeparator)
					return true;
			}
			return false;
		}
		public static bool IsLetter(Char ch)
		{
			SymbolType type = ResolveSymbol(ch); 
			if(type == SymbolType.Letter)
				return true;
			if(type == SymbolType.Unknown)
				return Char.IsLetter(ch);	
			return false;	
		}
		public static bool IsDigit(Char ch)
		{
			SymbolType type = ResolveSymbol(ch); 
			if(type == SymbolType.Digit)
				return true;
			if(type == SymbolType.Unknown)
				return Char.IsDigit(ch);	
			return false;	
		}
		public static bool IsLetterOrDigit(Char ch)
		{
			SymbolType type = ResolveSymbol(ch); 
			if(type == SymbolType.Digit || type == SymbolType.Letter)
				return true;
			if(type == SymbolType.Unknown)
				return Char.IsLetterOrDigit(ch);	
			return false;	
		}
	}
}
