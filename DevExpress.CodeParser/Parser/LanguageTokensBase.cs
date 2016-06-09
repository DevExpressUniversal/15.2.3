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
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using TokenType = Base.TokenType;
	public abstract class LanguageTokensBase
	{
		protected abstract StringCollection CreateKeywords();
		public virtual bool IsNewLine(char ch)
		{
			return CharUtils.IsNewLine(ch);
		}
		public virtual bool IsNewLine(string s)
		{
			return CharUtils.IsNewLine(s);
		}				
		public virtual bool IsWhiteSpace(char ch)
		{
			return CharUtils.IsWhiteSpace(ch);
		}		
		public abstract bool IsKeyword(string word);
		public abstract bool IsIdentifier(string word);
		public abstract bool IsDirective(int type);
		public abstract bool IsStringLiteral(int type);
		public abstract bool IsNumber(int type);
		public abstract bool IsComment(int type);
		public abstract bool IsXmlDocComment(int type);		
		public abstract bool IsStandardType(string word);
		public virtual bool IsOperator(int type)
		{
			switch (type)
			{
				case TokenType.Plus:
				case TokenType.Minus:
				case TokenType.BitAnd:
				case TokenType.BitOr:
				case TokenType.AndAnd:
				case TokenType.OrOr:
				case TokenType.XorSymbol:
				case TokenType.PlusPlus:
				case TokenType.MinusMinus:			
				case TokenType.Asterisk:
				case TokenType.Slash:
				case TokenType.PercentSymbol:
				case TokenType.LessThan:
				case TokenType.GreaterThan:
				case TokenType.LessOrEqual:
				case TokenType.GreaterOrEqual:
				case TokenType.Is:
				case TokenType.As:
				case TokenType.NotEquals:
				case TokenType.DoubleEquals:
				case TokenType.ExclamationSymbol:
				case TokenType.EqualsSymbol:
				case TokenType.PlusEqual:
				case TokenType.MinusEqual:
				case TokenType.MulEqual:
				case TokenType.DivEqual:
				case TokenType.ModEqual:
				case TokenType.AndEqual:
				case TokenType.OrEqual:
				case TokenType.XorEqual:
				case TokenType.ShiftLeftEqual:
				case TokenType.ShiftRightEqual:
				case TokenType.ShiftLeft:
				case TokenType.ShiftRight:
				case TokenType.Dot:
					return true;
			}
			return false;
		}
		public abstract int ToTokenType(string token);
	}
}
