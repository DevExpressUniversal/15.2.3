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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
  public partial class JavaScriptScanner : GeneratedScannerBase
  {
	public bool ShouldReadRegExpToken ;
		public JavaScriptScanner(ISourceReader s)
		{
			Initialize(s);
		}
	private void ReadEscapedChar()
	{
	  AddCh();
	  AddCh();
	}
	private void SeparateDivEquals(ref Token token)
	{
	  Token divToken = new Token(token.StartPosition, token.StartPosition + 2, token.Line, token.Column, token.Line, token.Column + 2, Tokens.DIVEQUAL, "/=");
	  string tokenValue = token.Value;
	  if (tokenValue.Length > 2)
		divToken.Next = ScanString(tokenValue.Substring(2, tokenValue.Length - 2), token.Line, token.Column + 2);
	  token = divToken;
	}
	private Token ScanString(string text, int startLine, int startColumn)
	{
	  SourceStringReader reader = new SourceStringReader(text, startLine, startColumn);
	  JavaScriptScanner scanner = new JavaScriptScanner(reader);
	  Token currentToken = scanner.Scan();
	  Token nextToken = scanner.Scan();
	  Token firstToken = currentToken;
	  while (nextToken != null && nextToken.Type != Tokens.EOF)
	  {
		currentToken.Next = nextToken;
		currentToken = nextToken;
		nextToken = scanner.Scan();
	  }
	  return firstToken;
	}
	private void SeparateDiv(ref Token token)
	{
	  Token divToken = new Token(token.StartPosition, token.StartPosition + 1, token.Line, token.Column, token.Line, token.Column + 1, Tokens.SLASH, "/");
	  string tokenValue = token.Value;
	  if (tokenValue.Length > 1)
		divToken.Next = ScanString(tokenValue.Substring(1, tokenValue.Length - 1), token.Line, token.Column + 1);
	  token = divToken;
	}
	private void RescanToken(ref Token token)
	{
	  string tokenValue = token.Value;
	  if (tokenValue.StartsWith("/="))
		SeparateDivEquals(ref token);
	  else
		if (tokenValue.StartsWith("/"))
		  SeparateDiv(ref token);
	}
	private void SetErrorToken()
	{
	  if (t != null)
		t.Type = Tokens.MaxTokens;
	}
	protected override int GetUnicodeLetterIndex()
		{
			return UnicodeLetterIndex;
		}
		protected override int GetNextState(int input)
		{
			return start[input];
		}
	protected void ReadRegExpToken()
	{
	  if (ch == '*' || ch == '\r' || ch == '\n' || ch == EOF)
	  {
		SetErrorToken();
		return; 
	  }
	  if (ch == '\\')
		ReadEscapedChar();
	  while (true)
	  {
		if (ch == '\\')
		  ReadEscapedChar();
		else
		  if (ch == '/')
		  {
			AddCh();
			break;
		  }
		  else
			if (ch == '\r' || ch == '\n' || ch == EOF)
			{
			  SetErrorToken();
			  return;
			}
			else
			  AddCh();
	  }
	  while (Char.IsLetter(ch))
		AddCh();
	  if (t != null)
		t.Type = Tokens.REGEXPLITERAL;
	}
	protected override void NextTokenEnd()
	{
	  base.NextTokenEnd();
	  if (t == null)
		return;
	  if (t.Type == Tokens.MaxTokens)
		RescanToken(ref t);
	}
	protected override Token CreateToken()
	{
	  return new CategorizedToken(TokenLanguage.JavaScript);
	}
  }
}
