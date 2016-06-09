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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  #if DXCORE
  using BaseTokenType = DevExpress.CodeRush.StructuralParser.Base.TokenType;
#else
  using BaseTokenType = DevExpress.CodeParser.Base.TokenType;
#endif
  public class TokenType: Base.TokenType
	{
		const int BaseIndex = DescendantStartIndex;
		public const int Imports = NamespaceReference;
		public const int Boolean = Bool;
		public const int Integer = Int;
		public const int Structure = Struct;
		public const int UInteger = Uint;
		public const int ULongToken = Ulong;
		public const int UShortToken = Ushort;
		public const int AddHandler = BaseIndex + 1;
		public const int AddRessof = BaseIndex + 2;
		public const int Alias = BaseIndex + 3;
		public const int And = BaseIndex + 4;
		public const int AndAlso = BaseIndex + 5;
		public const int Ansi = BaseIndex + 6;
		public const int Assembly = BaseIndex + 7;
		public const int Auto = BaseIndex + 8;
		public const int ByRef = BaseIndex + 9;
		public const int ByVal = BaseIndex + 10;
		public const int Call = BaseIndex + 11;				
		public const int Ctype = BaseIndex + 24;
		public const int Date = BaseIndex + 25;
		public const int Declare = BaseIndex + 26;
		public const int Dim = BaseIndex + 27;
		public const int DirectCast = BaseIndex + 28;
		public const int Each = BaseIndex + 29;
		public const int ElseIf = BaseIndex + 30;
		public const int End = BaseIndex + 31;
		public const int EndIf = BaseIndex + 32;
		public const int Erase = BaseIndex + 33;
		public const int Error = BaseIndex + 34;
		public const int Exit = BaseIndex + 35;
		public const int Friend = BaseIndex + 36;
		public const int Function = BaseIndex + 37;
		public const int GettypeToken = BaseIndex + 38;
		public const int GoSub = BaseIndex + 39;
		public const int Handles = BaseIndex + 40;
		public const int Implements = BaseIndex + 41;
		public const int Inherits = BaseIndex + 42;
		public const int Let = BaseIndex + 43;
		public const int Lib = BaseIndex + 44;
		public const int Like = BaseIndex + 45;
		public const int Loop = BaseIndex + 46;
		public const int Me = BaseIndex + 47;
		public const int Mod = BaseIndex + 48;
		public const int MustInherit = BaseIndex + 50;
		public const int MustOverride = BaseIndex + 51;
		public const int MyBase = BaseIndex + 52;
		public const int MyClass = BaseIndex + 53;
		public const int Next = BaseIndex + 54;
		public const int Not = BaseIndex + 55;
		public const int Nothing = BaseIndex + 56;
		public const int NotInheritable = BaseIndex + 57;
		public const int NotOverridable = BaseIndex + 58;
		public const int On = BaseIndex + 59;
		public const int Option = BaseIndex + 60;
		public const int Optional = BaseIndex + 61;
		public const int Or = BaseIndex + 62;
		public const int OrElse = BaseIndex + 63;
		public const int Overloads = BaseIndex + 64;
		public const int Overridable = BaseIndex + 65;
		public const int Overrides = BaseIndex + 66;
		public const int ParamArray = BaseIndex + 67;
		public const int Preserve = BaseIndex + 68;
		public const int Property = BaseIndex + 69;
		public const int RaiseEvent = BaseIndex + 70;
		public const int ReDim = BaseIndex + 71;
		public const int RemoveHandler = BaseIndex + 72;
		public const int Resume = BaseIndex + 73;
		public const int Select = BaseIndex + 74;
		public const int Shadows = BaseIndex + 75;
		public const int Shared = BaseIndex + 76;
		public const int Single = BaseIndex + 77;
		public const int Step = BaseIndex + 78;
		public const int Stop = BaseIndex + 79;
		public const int Sub = BaseIndex + 80;
		public const int SyncLock = BaseIndex + 81;
		public const int Then = BaseIndex + 82;
		public const int To = BaseIndex + 83;
		public const int Unicode = BaseIndex + 84;
		public const int Until = BaseIndex + 85;
		public const int Variant = BaseIndex + 86;
		public const int Wend = BaseIndex + 87;
		public const int When = BaseIndex + 88;
		public const int With = BaseIndex + 89;
		public const int WithEvents = BaseIndex + 90;
		public const int WriteOnly = BaseIndex + 91;
		public const int Xor = BaseIndex + 92;
		public const int Sharp = BaseIndex + 94;
		public const int LineContinuation = BaseIndex + 96;
		public const int BackSlash = BaseIndex + 99;
		public const int BackSlashEquals = BaseIndex + 100;
		public const int ConstDirective = BaseIndex + 104;
		public const int ElseIfDirective = BaseIndex + 105;
		public const int ExternalSourceDirective = BaseIndex + 106;
		public const int EndExternalSourceDirective = BaseIndex + 107;
		public const int DateLiteral = BaseIndex + 108;
		public const int IsNot = BaseIndex + 109;
		public const int ExternalCheckSum = BaseIndex + 110;
		public const int Using = BaseIndex + 111;
		public const int Global = BaseIndex + 112;
		public const int Of = BaseIndex + 113;
		public const int Mid = BaseIndex + 114;
		public const int Off = BaseIndex + 115;
		public const int Strict = BaseIndex + 116;
		public const int Compare = BaseIndex + 117;
		public const int Binary = BaseIndex + 118;
		public const int Text = BaseIndex + 119;		
		public const int ProtectedFriend = BaseIndex + 121;
		public const int Custom = BaseIndex + 122;
		public const int IsTrue = BaseIndex + 123;
		public const int IsFalse = BaseIndex + 124;
		public const int Widening = BaseIndex + 125;
		public const int Narrowing = BaseIndex + 126;
		public const int TryCast = BaseIndex + 127;
		public const int CSByte = BaseIndex + 128;
		public const int CUInt = BaseIndex + 130;
		public const int CULng = BaseIndex + 131;
		public const int CUShort = BaseIndex + 132;
		public const int ColonEqual = BaseIndex + 136;
		public const int OpenParenOf = BaseIndex + 137;
		public const int LineterminatorElse = BaseIndex + 138;
		#region GetTokenType
		public static new int GetTokenType(string name)
		{
	  if (name == "gettype")
		return GettypeToken;
	  if (name == "ulong")
		return ULongToken;
	  if (name == "ushort")
		return UShortToken;
			int lResult = BaseTokenType.GetTokenType(name);
			if (lResult != BaseTokenType.UnknownToken)
				return lResult;
			return TokenType.GetTokenType(typeof(TokenType), name);
		}
		#endregion
		#region GetTokenName
		public static new string GetTokenName(int token)
		{
	  if (token == GettypeToken)
		return "Gettype";
	  if (token == ULongToken)
		return "ULong";
	  if (token == UShortToken)
		return "UShort";
			string lName = TokenType.GetTokenName(typeof(TokenType), token);
			if (lName != "UnknownToken")
				return lName;
			return BaseTokenType.GetTokenName(token);
		}
		#endregion
	}
}
