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
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public abstract class CSharpScannerBase : GeneratedScannerBase
	{
	ScannerExtension _ScannerExtension;
		bool _InPreprocess;
	protected ScannerExtension ScannerExtension
	{
	  get { return _ScannerExtension; }
	  set { _ScannerExtension = value; }
	}
		protected override Token CreateToken()
		{
	  return new FormattingToken(TokenLanguage.CSharp);
		}
		protected override bool NextTokenStart()
		{
			return base.NextTokenStart();
		}
		void ChangeTokenType(Token token)
		{
			if (token == null)
				return;
			switch (token.Type)
			{
				case TokenType.AspBlockStart:
					token.Type = Tokens.ASPBLOCKSTART;
					break;
				case TokenType.AspBlockEnd:
					token.Type = Tokens.ASPBLOCKEND;
					break;
				case TokenType.AspCommentStatement:
					token.Type = Tokens.ASPCOMMENT;
					break;
				default:
					break;
			}
		}
		void CheckForInvalidComments(ref Token token)
		{
			string tokenValue = token.Value;
			if (tokenValue == null || tokenValue.Length == 0 || tokenValue.StartsWith("/"))
				return;
			int dotIndex = tokenValue.IndexOf(".");
			if (dotIndex <= 0)
				return;
			string beforeDotText = tokenValue.Substring(0, dotIndex);
	  int tokenType = beforeDotText.StartsWith("@") ? Tokens.STRINGCON : Tokens.INTCON;
	  Token literalToken = new CategorizedToken(token.Line, token.Column, token.EndLine, token.EndColumn - 1, tokenType, beforeDotText);
	  literalToken.StartPosition = token.StartPosition;
	  literalToken.EndPosition = token.EndPosition - 1;
	  Token dotToken = new CategorizedToken(token.EndLine, token.EndColumn - 1, token.EndLine, token.EndColumn, Tokens.DOT, ".");
	  dotToken.StartPosition = token.EndPosition - 1;
	  dotToken.EndPosition = token.EndPosition;
			literalToken.Next = dotToken;
			token = literalToken;
		}
		TokenCollection GetExtensionTokens(Token scannerToken)
		{
			TokenCollection extensionTokens = new TokenCollection();
			if (scannerToken == null)
				return extensionTokens;
			if (scannerToken.Type == Tokens.EOF)
				return _ScannerExtension.GetTailTokens();
			Token extensionToken = null;
			for (extensionToken = _ScannerExtension.Scan(scannerToken.Range.Start); extensionToken != null; extensionToken = _ScannerExtension.Scan(scannerToken.Range.Start))
				extensionTokens.Insert(0, extensionToken);
			return extensionTokens;
		}
		void InsertExtensionTokens(ref Token start, TokenCollection extensionTokens)
		{
			Token extensionToken = null;
			for (int i = 0; i < extensionTokens.Count; i++)
			{
				extensionToken = extensionTokens[i];
				extensionToken.Next = start;
				start = extensionToken;
			}
		}
		public override Token Scan()
		{
			if (_ScannerExtension == null)
				return base.Scan();
			Token tempToken = base.Scan();
			InsertExtensionTokens(ref tokens, GetExtensionTokens(tempToken));
			pt = tokens;
			ChangeTokenType(tokens);
			return tokens;
		}
		public override Token Peek()
		{
			if (_ScannerExtension == null)
				return base.Peek();
			Token tempToken = base.Peek();
			Token currentToken = tokens;
			while (currentToken != null && currentToken.Next != tempToken)
				currentToken = currentToken.Next;
			if (currentToken == null)
				return tempToken;
			InsertExtensionTokens(ref pt, GetExtensionTokens(tempToken));
			currentToken.Next = pt;
			ChangeTokenType(pt);
			return pt;
		}
		protected override void NextTokenEnd()
		{
			if (t == null)
				return;
			switch (t.Type)
			{
				case Tokens.SINGLELINEXML:
					CheckForInvalidComments(ref t);
					break;
				case Tokens.SINGLELINECOMMENT:
					if (t.Value.StartsWith("///") && !t.Value.StartsWith("////"))
						t.Type = Tokens.SINGLELINEXML;
					break;
				case Tokens.MULTILINECOMMENT:
					if (t.Value.StartsWith("/**") && !t.Value.StartsWith("/***"))
						t.Type = Tokens.MULTILINEXML;
					break;
			}
		}
		#region SkipIgnoredChars
		protected override void SkipIgnoredChars()
		{
			while ((ch != EOF) && (ch < 255) && (ignore[ch]))
			{
				if (InPreprocess && (ch == '\u000d' || ch == '\u000a'))
					break;
				NextCh();
			}
		}
		#endregion
	protected override void SetInPreprocess()
	{
	  if(t.Value == "#else")
		InPreprocess = true;
	}
	protected override void ResetInPreprocess()
	{
	  if (t.Value == "#else")
		InPreprocess = false;
	}
		public bool IsNextChar(char c)
		{
			SkipWs();
			return ch == c;
		}
		public bool IsEof()
		{
			return IsNextChar(EOF);
		}
		public string SkipToEOL()
		{
			StringBuilder text = new StringBuilder();
	  while (!IsLineTerminator(text) && !IsEof(text))
	  {
		text.Append(ch);
		NextCh();
	  }
			return text.ToString();
		}
	void SkipWs()
	{
	  while (ch == ' ' || ch == '\u0009')
	  {
		NextCh();
	  }
	}
		bool IsLineTerminator(StringBuilder sb)
		{
			if (ch == '\r')
			{
				sb.Append(ch);
				NextCh();
				if (ch == '\n')
				{
					sb.Append(ch);
					NextCh();
				}
				return true;
			}
			if (ch == '\n')
			{
				sb.Append(ch);
				NextCh();
				return true;
			}
			return false;
		}
		public bool InPreprocess
		{
			set
			{
				_InPreprocess = value;
			}
			get
			{
				return _InPreprocess;
			}
		}
		protected override bool IsEOLChar(char ch)
	{
	  return !InPreprocess && ch == '\u000a';
	}
		protected override bool IsCRChar(char ch)
	{
			return !InPreprocess && ch == '\u000d';
	}
	protected override FormattingElements GetNextFormattingTokenElements()
	{
	  bool oldInPP = InPreprocess;
	  if (t.Type == Tokens.LINETERMINATOR)
		InPreprocess = false;
	  try
	  {
		return base.GetNextFormattingTokenElements();
	  }
	  finally
	  {
		InPreprocess = oldInPP;
	  }
	}
	}
}
