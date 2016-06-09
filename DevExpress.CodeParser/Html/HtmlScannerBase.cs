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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Html
#else
namespace DevExpress.CodeParser.Html
#endif
{
  using Xml;
	public abstract class HtmlScannerBase : XmlScannerBase
	{
		char _PreviousChar;
		bool _ShouldCheckForAttribute = false;
	const string STR_Model = "model";
	const string STR_Inherits = "inherits";
	const string STR_Helper = "helper";
	const string STR_Functions = "functions";
	const string STR_Section = "section";
	const string STR_CDATAStart = "<![CDATA";
	const string STR_CDATAEnd = "]]>";
	const string STR_CodeEmbeddingStart = "<%";
	const string STR_CodeEmbeddingEnd = "%>";
		protected override Token CreateToken()
		{
	  return new CategorizedToken(TokenLanguage.Html);
		}
	private void ReadRazorComment()
	{
	  CharProvider.ResetPeek();
	  string twoLastChars = ch + CharProvider.Peek().ToString();
	  while (ch != EOF)
	  {
		if (twoLastChars == "*@")
		{
		  AddCh();
		  AddCh();
		  return;
		}
		AddCh();
		CharProvider.ResetPeek();
		twoLastChars = ch + CharProvider.Peek().ToString();
	  }
	}
	private void ReadSectionName(Token sectionToken)
	{
	  Token t = CreateToken();
	  while (ch == ' ' || ch == '\r' || ch == '\n')
		NextCh();
	  t.StartPosition = pos;
	  t.Column = pos - lineStart + 1;
	  if (t.Column <= 0)
		t.Column = pos - prevLineStart + 1;
	  t.Line = lineValue;
	  t.Type = Tokens.NAME;
	  tlen = 0;
	  CharProvider.ResetPeek();
	  while (Char.IsLetterOrDigit(ch))
		AddCh();
	  while (ch == ' ' || ch == '\r' || ch == '\n')
		NextCh();
	  if (t.Value == null)
		t.Value = new String(tval, 0, tlen);
	  t.EndPosition = pos;
	  if (ch != EOL)
	  {
		t.EndColumn = pos - lineStart + 1;
		t.EndLine = lineValue;
	  }
	  else
	  {
		t.EndColumn = pos - prevLineStart + 1;
		t.EndLine = prevLine;
	  }
	  sectionToken.Next = t;
	}
	private Token GetRazorStartToken()
	{
	  if (!IsRazorEmbeddingStartChar(ch))
		return null;
	  Token t = CreateToken();
	  t.StartPosition = pos;
	  t.Column = pos - lineStart + 1;
	  if (t.Column <= 0)
		t.Column = pos - prevLineStart + 1;
	  t.Line = lineValue;
	  t.Type = RazorStartCharToken;
	  tlen = 0;
	  CharProvider.ResetPeek();
	  AddCh();
	  if (ch == ':')
	  {
		t.Type = Tokens.RAZORSTARTCHARCOLON;
		AddCh();
	  }
	  if (ch == '*')
	  {
		AddCh();
		ReadRazorComment();
		t.Type = Tokens.RAZORCOMMENT;
	  }
	  if (ArePeekChars(STR_Model))
	  {
		for (int i = 0; i < STR_Model.Length; i++)
		{
		  AddCh();
		  t.Type = Tokens.RAZORMODEL;
		}
	  }
	  if (ArePeekChars(STR_Inherits))
	  {
		for (int i = 0; i < STR_Inherits.Length; i++)
		{
		  AddCh();
		  t.Type = Tokens.RAZORINHERITS;
		}
	  }
	  if (ArePeekChars(STR_Section))
	  {
		for (int i = 0; i < STR_Section.Length; i++)
		  AddCh();
		t.Type = Tokens.RAZORSECTION;
	  }
	  if (ArePeekChars(STR_Functions))
	  {
		for (int i = 0; i < STR_Functions.Length; i++)
		  AddCh();
		t.Type = Tokens.RAZORFUNCTIONS;
	  }
	  if (ArePeekChars(STR_Helper))
	  {
		for (int i = 0; i < STR_Helper.Length; i++)
		{
		  AddCh();
		  t.Type = Tokens.RAZORHELPER;
		}
		while (ch == ' ' || ch == '\r' || ch == '\n')
		  NextCh();
	  }
	  if (t.Value == null)
		t.Value = new String(tval, 0, tlen);
	  t.EndPosition = pos;
	  if (ch != EOL)
	  {
		t.EndColumn = pos - lineStart + 1;
		t.EndLine = lineValue;
	  }
	  else
	  {
		t.EndColumn = pos - prevLineStart + 1;
		t.EndLine = prevLine;
	  }
	  if (t.Type == Tokens.RAZORSECTION)
		ReadSectionName(t);
	  return t;
	}
	protected override void AddCh()
		{
			_PreviousChar = ch;
			base.AddCh();
		}
	bool SkipString()
		{
			if (ch != '"')
				return false;
			if (_PreviousChar == '\\')
				return false;
			AddCh();
			while (ch != EOF)
			{
				if (ch == '"' && _PreviousChar != '\\')
				{
					AddCh();
					if (ch == EOF)
						return false;
					if (ch == '"')
					{
						AddCh();
					}
					else
					{
						return true;
					}
				}
				else
				{
					AddCh();
				}
			}
			return false;
		}
		bool SkipCData()
		{
	  if (!ArePeekChars(STR_CDATAStart))
				return false;				
			SkipChars(8);
			while(ch != EOF)
			{
				AddCh();
				CharProvider.ResetPeek();
				if (ArePeekChars(STR_CDATAEnd))
				{
					SkipChars(3);
					return true;
				}
			}
			return false;
		}
		void SkipChars(int count)
		{
			if (ch == EOF)
				return;
			for (int i = 0; i < count; i++)
			{
				AddCh();
				if (ch == EOF)
				{
					return;
				}
			}
		}
		bool ArePeekChars(string chars)
		{
	  if (string.IsNullOrEmpty(chars))
				return false;
			if (ch != chars[0])
				return false;
	  int length = chars.Length;
			if (ch == EOF)
			{
				if (length == 1)
					return true;
				return false;
			}
			CharProvider.ResetPeek();
			for (int i = 1; i < length; i++)
			{
				char sym = chars[i];
				char nextChar = CharProvider.Peek();
				if (nextChar == EOF)
					return false;
				if (sym != nextChar)
					return false;
			}
			return true;
		}
	bool SkipCodeEmbedding(bool isEmbeddingStart)
	{
	  CharProvider.ResetPeek();
	  int codeEmbeddingCount = 1;
	  if (isEmbeddingStart)
	  {
		codeEmbeddingCount = 0;
		if (!ArePeekChars(STR_CodeEmbeddingStart))
		  return false;
	  }
	  while (ch != EOF)
	  {
		if (ArePeekChars(STR_CodeEmbeddingStart))
		  codeEmbeddingCount++;
		if (ArePeekChars(STR_CodeEmbeddingEnd))
		{
		  codeEmbeddingCount--;
		  if (codeEmbeddingCount == 0)
			break;
		}
		if (!SkipString() && !SkipCData())
		{
		  AddCh();
		}
	  }
	  return true;
	}
		public Token GetCodeEmbeddingText()
		{
			Token t = CreateToken();
			t.StartPosition = pos;			
			t.Column = pos - lineStart + 1;
			if (t.Column <= 0)
				t.Column = pos - prevLineStart + 1;
			t.Line = lineValue; 
			CharProvider.ResetPeek();
			tlen = 0;
	  SkipCodeEmbedding(false);
			t.EndPosition = pos;			
			if (ch != EOL)
			{
				t.EndColumn = pos - lineStart + 1;
				t.EndLine = lineValue;
			}
			else
			{
				t.EndColumn = pos - prevLineStart + 1;
				t.EndLine = prevLine;
			}
			if(t.Value == null)
				t.Value = new String(tval, 0, tlen);
			return t;
		}
	protected override void NextTokenAfterScan()
	{
	  if (t.Type != Tokens.COMMENT)
		return;
	  string value = t.Value;
	  if (string.IsNullOrEmpty(value) || value.EndsWith("-->"))
		return;
	  while (ch != EOF)
	  {
		if (ch == '-')
		{
		  AddCh();
		  if (ch == '-')
		  {
			AddCh();
			if (ch == '>')
			{
			  AddCh();
			  break;
			}
		  }
		}
		AddCh();
	  }
	  t.Value = new String(tval, 0, tlen);
	}
		protected override bool NextTokenStart()
		{
			if (ShouldCheckForXmlText)
			{
				char currentChar = PeekForSignificantChar();
				if (currentChar != '<' && currentChar != 65535 && !IsReference(currentChar) && !IsRazorEmbeddingStartChar(ch))
				{
					t = GetXmlTextToken();
					return true;
				}
			}
			SkipIgnoredChars();
			if (ShouldReturnCharDataToken)
			{
				t = GetCharDataToken();
				return true;
			}
	  if (IsRazorEmbeddingStartChar(ch))
	  {
		t = GetRazorStartToken();
		return true;
	  }
			if (ShouldReturnPIChars)
			{
				t = GetPICharToken();
				return true;
			}
			if (ShouldCheckForAttribute)
			{ 
				ShouldCheckForAttribute = false;
				if (ch != '\'' && ch != '>' && ch != '\"' && ch != '<')
				{
					t = GetAttributeValue();
					return true;
				}
			}
			return false;
		}	
		bool IsEmptyCloseTag()
		{
			if (ch != '/')
				return false;
			CharProvider.ResetPeek();
			char nextChar = CharProvider.Peek();
			return nextChar == '>';
		}
		protected Token GetAttributeValue()
		{
			Token t = CreateToken();
			t.StartPosition = pos;			
			t.Column = pos - lineStart + 1;
			if (t.Column <= 0)
				t.Column = pos - prevLineStart + 1;
			t.Line = lineValue; 
			t.Type = Tokens.UNQUOTEDATTRIBUTEVALUE;
			tlen = 0;
			while (ch != EOF && ch != '>' && ch != ' ' && ch != EOL && ch != '\t' && !IsEmptyCloseTag())
			{
				AddCh();
			}
			if(t.Value == null)
				t.Value = new String(tval, 0, tlen);
			t.EndPosition = pos;			
			if (ch != EOL)
			{
				t.EndColumn = pos - lineStart + 1;
				t.EndLine = lineValue;
			}
			else
			{
				t.EndColumn = pos - prevLineStart + 1;
				t.EndLine = prevLine;
			}
			return t;
		}
		protected override int CDataTokenType
		{
			get
			{
				return Tokens.CDATA;
			}
		}
		protected override int PICharsTokenType
		{
			get
			{
				return Tokens.PICHARS;
			}
		}
	public int RazorStartCharToken
	{
	  get
	  {
		return Tokens.RAZORSTARTCHAR;
	  }
	}
	public Token ReadStringLiteral(int quoteTokenType, HtmlParser parser)
	{
	  char endStringChar = '"';
	  if (quoteTokenType == Tokens.SINGLEQUOTE)
		endStringChar = '\'';
	  Token t = CreateToken();
	  t.StartPosition = pos;
	  t.Column = pos - lineStart + 1;
	  if (t.Column <= 0)
		t.Column = pos - prevLineStart + 1;
	  t.Line = lineValue;
	  t.Type = Tokens.QUOTEDLITERAL;
	  tlen = 0;
	  CharProvider.ResetPeek();
	  while (ch != EOF)
	  {
		if (IsRazorEmbeddingStartChar(ch))
		{
		  AddCh();
		  parser.ParseRazorInlineExpressionInString(pos, SourceReader);
		}		
		if (ch == endStringChar)
		{
		  if (CharProvider.Peek() == endStringChar)
		  {
			AddCh();
			AddCh();
		  }
		  else
			break;
		}
		else
		{
		  if (IsRazor)
			AddCh();
		  else if (!SkipCodeEmbedding(true))
			  AddCh();
		}
	  }
	  if (ch == endStringChar)
		AddCh();
	  if (t.Value == null)
		t.Value = new String(tval, 0, tlen);
	  t.EndPosition = pos;
	  if (ch != EOL)
	  {
		t.EndColumn = pos - lineStart + 1;
		t.EndLine = lineValue;
	  }
	  else
	  {
		t.EndColumn = pos - prevLineStart + 1;
		t.EndLine = prevLine;
	  }
	  return t;
	}
		public void GetAspDirectiveString(out string contentString, out SourceRange contentRange)
		{
			String result = String.Empty;
			contentRange = SourceRange.Empty;
			CharProvider.ResetPeek();
			tlen = 0;
			int startCol = pos - lineStart + 1;
			if (startCol <= 0)
				startCol = pos - prevLineStart + 1;
			int startLine = lineValue; 
			string twoLastChars = ch + CharProvider.Peek().ToString();
			while (ch != EOF)
			{
				if (twoLastChars == "%>")
				{
					while (CharProvider.HasPeekedChars)
						AddCh();
					break;
				}
				else
				{
					AddCh();
					twoLastChars = twoLastChars[1].ToString() + CharProvider.Peek().ToString();
				}
			}
			int endCol;
			int endLine;
			if (ch != EOL)
			{
				endCol = pos - lineStart + 1;
				endLine = lineValue;
			}
			else
			{
				endCol = pos - prevLineStart + 1;
				endLine = prevLine;
			}
			contentRange = new SourceRange(startLine, startCol, endLine, endCol);
			result = new String(tval, 0, tlen);
			contentString = result;
		}
	public string ReadSingleLine(out SourceRange lineRange)
	{
	  String result = String.Empty;
	  lineRange = SourceRange.Empty;
	  CharProvider.ResetPeek();
	  tlen = 0;
	  int startCol = pos - lineStart + 1;
	  if (startCol <= 0)
		startCol = pos - prevLineStart + 1;
	  int startLine = lineValue;
	  while (ch != EOF && ch != EOL)
	  {
		AddCh();
	  }
	  int endCol;
	  int endLine;
	  if (ch != EOL)
	  {
		endCol = pos - lineStart + 1;
		endLine = lineValue;
	  }
	  else
	  {
		endCol = pos - prevLineStart + 1;
		endLine = prevLine;
	  }
	  lineRange = new SourceRange(startLine, startCol, endLine, endCol);
	  result = new String(tval, 0, tlen);
	  return result;
	}
		public void GetElementContent(string elementName, out string contentString, out SourceRange contentRange)
		{
			String result = String.Empty;
			contentRange = SourceRange.Empty;
			CharProvider.ResetPeek();
			tlen = 0;
			int startCol = pos - lineStart + 1;
			if (startCol <= 0)
				startCol = pos - prevLineStart + 1;
			int startLine = lineValue; 
			string twoLastChars = ch + CharProvider.Peek().ToString();
			while (ch != EOF)
			{
				if (twoLastChars == "</")
				{
					if (IsElementName(elementName))
					{
						break;
					}
					else
					{
						while (CharProvider.HasPeekedChars)
							AddCh();
						twoLastChars = ch + CharProvider.Peek().ToString();	
					}
				}
				else
				{
					AddCh();
					twoLastChars = twoLastChars[1].ToString() + CharProvider.Peek().ToString();
				}
			}
			int endCol;
			int endLine;
			if (ch != EOL)
			{
				endCol = pos - lineStart + 1;
				endLine = lineValue;
			}
			else
			{
				endCol = pos - prevLineStart + 1;
				endLine = prevLine;
			}
			contentRange = new SourceRange(startLine, startCol, endLine, endCol);
			result = new String(tval, 0, tlen);
			contentString = result;
		}
		public bool ShouldCheckForAttribute
		{
			get
			{
				return _ShouldCheckForAttribute;
			}
			set
			{
				_ShouldCheckForAttribute = value;
			}
		}
	}
}
