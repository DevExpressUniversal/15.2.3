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
using System.Collections.Specialized;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public sealed class CSharpPunctuators
	{
		#region private fields...
		static Hashtable _Punctuators;
		#endregion
		#region CSharpPunctuators()
		private CSharpPunctuators(){}
		#endregion
		#region CSharpPunctuators()
		static CSharpPunctuators()
		{
			lock (typeof(CSharpPunctuators))
			{
				_Punctuators = new Hashtable();
				_Punctuators.Add(Tokens.LBRACE, "{");
				_Punctuators.Add(Tokens.RBRACE, "}");
				_Punctuators.Add(Tokens.LBRACK, "[");
				_Punctuators.Add(Tokens.RBRACK, "]");
				_Punctuators.Add(Tokens.LPAR, "(");
				_Punctuators.Add(Tokens.RPAR, ")");
				_Punctuators.Add(Tokens.DOT, ".");
				_Punctuators.Add(Tokens.COMMA, ",");
				_Punctuators.Add(Tokens.COLON, ":");
				_Punctuators.Add(Tokens.SCOLON, ";");
				_Punctuators.Add(Tokens.PLUS, "+");
				_Punctuators.Add(Tokens.MINUS, "-");
				_Punctuators.Add(Tokens.TIMES, "*");
				_Punctuators.Add(Tokens.DIV, "/");
				_Punctuators.Add(Tokens.MOD, "%");
				_Punctuators.Add(Tokens.AND, "&");
				_Punctuators.Add(Tokens.OR, "|");
				_Punctuators.Add(Tokens.XOR, "^");
				_Punctuators.Add(Tokens.NOT, "!");
				_Punctuators.Add(Tokens.TILDE, "~");
				_Punctuators.Add(Tokens.ASSGN, "=");
				_Punctuators.Add(Tokens.LT, "<");
				_Punctuators.Add(Tokens.GT, ">");
				_Punctuators.Add(Tokens.QUESTION, "?");
				_Punctuators.Add(Tokens.INC, "++");
				_Punctuators.Add(Tokens.DEC, "--");
				_Punctuators.Add(Tokens.DBLAND, "&&");
				_Punctuators.Add(Tokens.DBLOR, "||");
				_Punctuators.Add(Tokens.LTLT, "<<");
				_Punctuators.Add(Tokens.GTGT, ">>");
				_Punctuators.Add(Tokens.EQ, "==");
				_Punctuators.Add(Tokens.NEQ, "!=");
				_Punctuators.Add(Tokens.LOWOREQ, "<=");
				_Punctuators.Add(Tokens.GTEQ, ">=");
				_Punctuators.Add(Tokens.PLUSASSGN, "+=");
				_Punctuators.Add(Tokens.MINUSASSGN, "-=");
				_Punctuators.Add(Tokens.TIMESASSGN, "*=");
				_Punctuators.Add(Tokens.DIVASSGN, "/=");
				_Punctuators.Add(Tokens.MODASSGN, "%=");
				_Punctuators.Add(Tokens.ANDASSGN, "&=");
				_Punctuators.Add(Tokens.ORASSGN, "|=");
				_Punctuators.Add(Tokens.XORASSGN, "^=");
				_Punctuators.Add(Tokens.LSHASSGN, "<<=");
				_Punctuators.Add(Tokens.RSHASSGN, ">>=");
				_Punctuators.Add(Tokens.POINT, "->");
			}
		}
		#endregion
		#region TokenTypes
		public static ICollection TokenTypes
		{
			get
			{
				return _Punctuators.Keys;
			}
		}
		#endregion
		#region GetPunctuator(int token)
		public static string GetPunctuator(int token)
		{
			string name = "";
			if (_Punctuators.ContainsKey(token))
				name = (string)_Punctuators[token];
			return name;
		}
		public static bool IsPunctuator(int token) 
		{
			return _Punctuators.ContainsKey(token);
		}
		#endregion
	}
}
