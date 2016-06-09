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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public abstract class VBScannerBase : GeneratedScannerBase
	{
	const char EOT = (char)129;
		XmlString _XmlString;
		public VBScannerBase(string input)
			: this(new SourceStringReader(input))
		{
		}
		public VBScannerBase(ISourceReader reader)
	  : this(reader, reader.StartLine, reader.StartColumn)
		{
		}
		public VBScannerBase(ISourceReader reader, int line, int offset)
		{
	  if (line <= 0)
		line = 1;
	  if (offset <= 0)
		offset = 1;
	  _XmlString = new XmlString();
	  Initialize(reader);
	  pt = tokens = CreateToken();
	  _XmlString.SetScanner(this);
		}
		void SeparateTypeCharacterFromId(string source, out string idText, out string typeCharText)
		{
			idText = String.Empty;
			typeCharText = String.Empty;
			if (source == null || source.Length == 0)
				return;
			int sourceLength = source.Length;
			idText = source.Substring(0, sourceLength - 1);
			typeCharText = source.Substring(sourceLength - 1, 1);
		}
		private void SetTokenProperties(Token token, int tokenType, int startPosition, int endPosition, int line, int column, int endLine, int endColumn, string tokenText)
		{
	  token.StartPosition = startPosition;
	  token.EndPosition = endPosition;
			token.Line = line;
			token.Column = column;
			token.EndLine = endLine;
			token.EndColumn = endColumn;
			token.Type = tokenType;
			token.Value = GlobalStringStorage.Intern(tokenText);
		}
		private bool IdentifierContainsTypeCharacter(string tokenText)
		{
			if (tokenText == null || tokenText.Length == 0)
				return false;
			char lastChar = tokenText[tokenText.Length - 1];
			return lastChar == '%' || lastChar == '&' || lastChar == '@' || lastChar == '!' || lastChar == '#' || lastChar == '$';
		}
		private int GetTokenTypeForTypeCharacter(string typeCharText)
		{
			if (typeCharText == null || typeCharText.Length == 0)
				return 0;
			if (typeCharText == "%")
				return Tokens.PercentSymbol;
			if (typeCharText == "&")
				return Tokens.BitAnd;
			if (typeCharText == "@")
				return Tokens.CommAtSymbol;
			if (typeCharText == "!")
				return Tokens.ExclamationSymbol;
			if (typeCharText == "#")
				return Tokens.Sharp;
			if (typeCharText == "$")
				return Tokens.DollarSybol;
			return -1;
		}
		Token GetTokenForIdentifierWithTypeCharacter(Token token)
		{
			Token idToken = CreateToken();
	  Token typeCharToken = CreateToken();
			idToken.Next = typeCharToken;
			string idText = String.Empty;
			string typeCharText = String.Empty;
			SeparateTypeCharacterFromId(token.Value, out idText, out typeCharText);
			SetTokenProperties(idToken, Tokens.Identifier, token.StartPosition, token.EndPosition - 1, token.Line, token.Column, token.EndLine, token.EndColumn - 1, idText);
			SetTokenProperties(typeCharToken, GetTokenTypeForTypeCharacter(typeCharText), token.EndPosition - 1, token.EndPosition, token.Line, token.EndColumn - 1, token.EndLine, token.EndColumn, typeCharText);
			return idToken;
		}
		protected Token GetToken(Token token)
		{
			string tokenText = token.Value;
			if (token.Type == Tokens.Identifier && IdentifierContainsTypeCharacter(tokenText))
				return GetTokenForIdentifierWithTypeCharacter(token);
			return token;
		}
	void SkipWs()
	{
	  while (ch == ' ' || ch == '\u0009')
	  {
		NextCh();
	  }
	}
	bool IsLineTerminator()
	{
	  if (ch == '\r')
	  {
		NextCh();
		if (ch == '\n')
		  NextCh();
		return true;
	  }
	  if (ch == '\n')
	  {
		NextCh();
		return true;
	  }
	  return false;
	}
	protected override Token CreateToken()
	{
	  return new FormattingToken(TokenLanguage.Basic);
	}
	protected override void NextTokenEnd()
	{
	  if (t != null && t.Type == Tokens.SingleLineComment && t.Value.StartsWith("'''"))
		t.Type = Tokens.SingleLineXmlComment;
	}
		protected override Token NextToken()
		{
	  Token token = base.NextToken();
	  if (token != null)
		return GetToken(token);
	  return token;
		}
		public char GetTypeCharacter()
		{
			return ch;
		}
		public string GetXmlString(int startBufferIndex, string peekString)
		{
			_XmlString.CurrentChar = CharValue;
			string str = _XmlString.GetXmlString(startBufferIndex, peekString, tokens);
	  NextChar();
			return str;
		}
		public string GetXmlString(int startBufferIndex)
		{
			_XmlString.CurrentChar = CharValue;
			string str = _XmlString.GetXmlString(startBufferIndex, tokens);
	  NextChar();
			return str;
		}
		public string GetXmlImports(int startBufferIndex)
		{
			return GetXmlOneTag(startBufferIndex);
		}
		public string GetXmlOneTag(int startBufferIndex)
		{
			_XmlString.CurrentChar = CharValue;
			string str = _XmlString.GetXmlOneTag(startBufferIndex, tokens);
	  if (ch != '>')
		NextChar();
			return str;
		}
	public void ResetNextChCounter()
	{
	  CharProvider.ResetPeek();
	}
	public void NextChar()
	{
	  NextCh();
	}
	public bool IsEof()
	{
	  return IsNextChar(EOF) || IsNextChar(EOT);
	}
	public void SkipToEOL()
	{
	  while (!IsLineTerminator() && !IsEof())
	  {
		NextCh();
	  }
	}
	public bool IsNextChar(char c)
	{
	  SkipWs();
	  return ch == c;
	}
	public virtual char CharValue
	{
	  get { return ch; }
	}
  }
}
