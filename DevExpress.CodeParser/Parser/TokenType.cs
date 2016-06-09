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
using System.Reflection;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Base
#else
namespace DevExpress.CodeParser.Base
#endif
{
	public class TokenType
	{
		public const int UnknownToken = -1;
		public const int Identifier = 1;
		public const int Keyword = 2;
		public const int Literal = 3;
		public const int Operator = 4;
		public const int Separator = 5;
		public const int Eof = 6;
		public const int Abstract = 7;
		public const int Add = 8;
		public const int AngleBracketClose = 9;		
		public const int AngleBracketOpen = 10;		
		public const int As = 11;
		public const int Base = 12;
		public const int Bool = 13;
		public const int BracketClose = 14;
		public const int BracketOpen = 15;
		public const int Break = 16;
		public const int Byte = 17;
		public const int Case = 18;
		public const int Catch = 19;
		public const int Char = 20;
		public const int Checked = 21;
		public const int Class = 22;
		public const int Colon = 23;
		public const int Comma = 24;
		public const int Const = 25;
		public const int Continue = 26;
		public const int CurlyBraceOpen = 27;
		public const int CurlyBraceClose = 28;
		public const int Decimal = 29;
		public const int Default = 30;
		public const int DefineDirective = 31;
		public const int Delegate = 32;
		public const int Do = 33;
		public const int Dot = 34;
		public const int Double = 35;
		public const int ElifDirective = 36;
		public const int Else = 37;
		public const int ElseDirective = 38;
		public const int EndifDirective = 39;
		public const int EndRegion = 40;
		public const int Enum = 41;
		public const int EqualsSymbol = 42;
		public const int ErrorDirective = 43;
		public const int Event = 44;
		public const int Explicit = 45;
		public const int Extern = 46;
		public const int False = 47;
		public const int Finally = 48;
		public const int Fixed = 49;
		public const int Float = 50;
		public const int For = 51;
		public const int Foreach = 52;
		public const int Get = 53;
		public const int Goto = 54;
		public const int If = 55;
		public const int IfDirective = 56;
		public const int Implicit = 57;
		public const int In = 58;
		public const int Int = 59;
		public const int Interface = 60;
		public const int Internal = 61;
		public const int Is = 62;
		public const int Label = 63;
		public const int LineDirective = 64;
		public const int Lock = 65;
		public const int Long = 66;
		public const int Namespace = 67;
		public const int New = 68;
		public const int Null = 69;
		public const int Object = 70;
		public const int Override = 71;
		public const int Out = 72;
		public const int ParenOpen = 73;
		public const int ParenClose = 74;
		public const int Params = 75;
		public const int Private = 76;
		public const int Protected = 77;
		public const int Public = 78;
		public const int Readonly = 79;
		public const int Region = 80;
		public const int Ref = 81;
		public const int Rem = 82;
		public const int Remove = 83;
		public const int Return = 84;
		public const int Sbyte = 85;
		public const int Sealed = 86;
		public const int SemiColon = 87;
		public const int Set = 88;
		public const int Short = 89;
		public const int Sizeof = 90;
		public const int Stackalloc = 91;
		public const int Static = 92;
		public const int String = 93;
		public const int Struct = 94;
		public const int Switch = 95;
		public const int This = 96;
		public const int Throw = 97;
		public const int True = 98;
		public const int Try = 99;
		public const int Typeof = 100;
		public const int Virtual = 101;
		public const int Uint = 102;
		public const int Ulong = 103;
		public const int Unchecked = 104;
		public const int UndefDirective = 105;
		public const int Unsafe = 106;
		public const int Ushort = 107;
		public const int NamespaceReference = 108;
		public const int Void = 109;
		public const int Volatile = 110;
		public const int WarningDirective = 111;
		public const int While = 112;
		public const int Asterisk = 113;
		public const int Tilde = 114;
		public const int Plus = 115;
		public const int Minus = 116;
		public const int Slash = 117;
		public const int PercentSymbol = 118;
		public const int XorSymbol = 119;
		public const int ExclamationSymbol = 120;
		public const int LessThan = 121;
		public const int GreaterThan = 122;
		public const int QuestionSymbol = 123;
		public const int PlusPlus = 124;
		public const int MinusMinus = 125;
		public const int ShiftLeft = 126;
		public const int ShiftRight = 127;
		public const int DoubleEquals = 128;
		public const int LessOrEqual = 129;
		public const int GreaterOrEqual = 130;
		public const int CharacterLiteral = 131;
		public const int StringLiteral = 132;
		public const int IntegerLiteral = 133;
		public const int FloatingPointLiteral = 134;
		public const int BitAnd = 135;
		public const int BitOr = 136;
		public const int AndAnd = 137;
		public const int OrOr = 138;
		public const int NotEquals = 139;
		public const int PlusEqual = 140;
		public const int MinusEqual = 141;
		public const int MulEqual = 142;
		public const int DivEqual = 143;
		public const int ModEqual = 144;
		public const int AndEqual = 145;
		public const int OrEqual = 146;
		public const int XorEqual = 147;
		public const int ShiftLeftEqual = 148;
		public const int ShiftRightEqual = 149;
		public const int Arrow = 150;
		public const int Comment = 151;
		public const int SingleLineComment = 152;
		public const int XmlComment = 153;
		public const int SingleLineXmlComment = 154;
		public const int MethodBody = 155;
		public const int Module = 156;
		public const int Partial = 157;
		public const int Where = 158;
		public const int Yield = 159;
		public const int NullCoalescing = 160;
		public const int AliasQualifier = 161;
		public const int LineTerminator = 162;
		public const int CBool = 163;
		public const int CByte = 164;
		public const int CChar = 165;
		public const int CDate = 166;
		public const int CDbl = 167;
		public const int CDec = 168;
		public const int Cint = 169;
		public const int Clng = 170;
		public const int Cobj = 171;
		public const int Cshort = 172;
		public const int Csng = 173;
		public const int Cstr = 174;
		public const int AspBlockStart = 175;
		public const int AspBlockEnd = 176;
		public const int AspCommentStatement = 177;
		#region GetTokenType(string typeName)
		protected static int GetTokenType(Type type, string name)
		{
			FieldInfo lFieldInfo = type.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
			if (lFieldInfo != null)
				return (int)lFieldInfo.GetValue(null);
			else
				return UnknownToken;
		}
		#endregion
		#region GetTokenName(Type type, int token)
		protected static string GetTokenName(Type type, int token)
		{
			FieldInfo[] lFields = type.GetFields(BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
			if (lFields != null)
			{
				for (int i = 0; i < lFields.Length; i++)
				{
					if ((int)lFields[i].GetValue(null) == token)
						return lFields[i].Name;
				}
			}
			return "UnknownToken";
		}
		#endregion
		#region GetTokenType(string typeName)
		public static int GetTokenType(string name)
		{
			return GetTokenType(typeof(TokenType), name);
		}
		#endregion
		#region GetTokenName(int type)
		public static string GetTokenName(int type)
		{
			return GetTokenName(typeof(TokenType), type);
		}
		#endregion
		public const int DescendantStartIndex = 1000;
	}
}
