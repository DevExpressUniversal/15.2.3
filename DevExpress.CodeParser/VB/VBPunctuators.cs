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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public sealed class VBPunctuators
	{
		static Hashtable _Punctuators;
		#region VBPunctuators
		private VBPunctuators(){}
		#endregion
		#region VBPunctuators()
		static VBPunctuators()
		{
			lock (typeof(VBPunctuators))
			{
				_Punctuators = new Hashtable();
				_Punctuators.Add(TokenType.Colon, ":");
				_Punctuators.Add(TokenType.Comma, ",");
				_Punctuators.Add(TokenType.CurlyBraceOpen, "{");
				_Punctuators.Add(TokenType.CurlyBraceClose, "}");
				_Punctuators.Add(TokenType.Dot, ".");
				_Punctuators.Add(TokenType.ExclamationSymbol, "!");
				_Punctuators.Add(TokenType.ParenClose, ")");
				_Punctuators.Add(TokenType.ParenOpen , "(");
				_Punctuators.Add(TokenType.Sharp, "#");
				_Punctuators.Add(TokenType.BitAnd, "&");
				_Punctuators.Add(TokenType.Asterisk, "*");
				_Punctuators.Add(TokenType.BackSlash, "\\");
				_Punctuators.Add(TokenType.EqualsSymbol, "=");
				_Punctuators.Add(TokenType.LessThan, "<");
				_Punctuators.Add(TokenType.Minus, "-");
				_Punctuators.Add(TokenType.GreaterThan, ">");
				_Punctuators.Add(TokenType.Plus, "+");
				_Punctuators.Add(TokenType.XorSymbol, "^");
				_Punctuators.Add(TokenType.Slash, "/");
				_Punctuators.Add(TokenType.NotEquals, "<>");
				_Punctuators.Add(TokenType.XorEqual, "^=");
				_Punctuators.Add(TokenType.MulEqual, "*=");
				_Punctuators.Add(TokenType.DivEqual, "/=");
				_Punctuators.Add(TokenType.BackSlashEquals, "\\=");
				_Punctuators.Add(TokenType.PlusEqual, "+=");
				_Punctuators.Add(TokenType.MinusEqual, "-=");
				_Punctuators.Add(TokenType.AndEqual, "&=");
				_Punctuators.Add(TokenType.ShiftLeftEqual, "<<=");
				_Punctuators.Add(TokenType.ShiftRightEqual, ">>=");
				_Punctuators.Add(TokenType.ShiftLeft, "<<");
				_Punctuators.Add(TokenType.ShiftRight, ">>");
				_Punctuators.Add(TokenType.LessOrEqual, "<=");
				_Punctuators.Add(TokenType.GreaterOrEqual, ">=");
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
		#region GetPunctuator(int aToken)
		public static string GetPunctuator(int token)
		{
			return (string)_Punctuators[token];
		}
		#endregion
	}
}
