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
namespace DevExpress.CodeRush.StructuralParser.Xml
#else
namespace DevExpress.CodeParser.Xml
#endif
{
	public abstract class XmlScannerBase: GeneratedScannerBase
	{		
		bool _ShouldReturnCharDataToken = false;
		bool _ShouldReturnPIChars = false;
		bool _ShouldCheckForXmlText = false;
	private bool _IsRazor;
	protected bool IsRazor
	{
	  get { return _IsRazor; }
	  set { _IsRazor = value; }
	}
	protected bool IsRazorEmbeddingStartChar(char ch)
	{
	  return IsRazor && ch == '@';
	}
		protected override Token CreateToken()
		{
			return new Token();
		}
		protected char PeekForSignificantChar()
		{
			if (ch == EOF || ch > 255 || !ignore[ch])
				return ch;
			CharProvider.ResetPeek();
			char currentChar = CharProvider.Peek();
			while (currentChar != EOF &&  currentChar < 255 && ignore[currentChar])
				currentChar = CharProvider.Peek();
			return currentChar;
		}
		protected override bool NextTokenStart()
		{
			if (ShouldCheckForXmlText)
			{
				char currentChar = PeekForSignificantChar();
				if (currentChar != '<' && !IsReference(currentChar) && !IsRazorEmbeddingStartChar(currentChar))
				{
					t = GetXmlTextToken();
					return true;
				}
				if (currentChar == '<' && ch != '<')
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
			if (ShouldReturnPIChars)
			{
				t = GetPICharToken();
				return true;
			}
			return false;
		}
		protected bool IsElementName(string name)
		{
			char currentPeekChar = CharProvider.Peek();
			while (currentPeekChar == '\n' || currentPeekChar == '\r' || currentPeekChar == '\t' || currentPeekChar == ' ')
				currentPeekChar = CharProvider.Peek();
			for (int i = 0; i < name.Length; i++)
				if (Char.ToUpper(name[i]) != Char.ToUpper(currentPeekChar))
					return false;
				else
					currentPeekChar = CharProvider.Peek();
			if (currentPeekChar == '\n' || currentPeekChar == '\r' || currentPeekChar == '\t' || currentPeekChar == ' ' || currentPeekChar == '>')
				return true;
			else
				return false;
		}
		protected Token GetCharDataToken()
		{
			Token t = CreateToken();
			t.StartPosition = pos;			
			t.Column = pos - lineStart + 1;
			if (t.Column <= 0)
				t.Column = pos - prevLineStart + 1;
			t.Line = lineValue; 
			t.Type = CDataTokenType;
			tlen = 0;
			CharProvider.ResetPeek();
			string threeLastChars = ch.ToString() + CharProvider.Peek().ToString() + CharProvider.Peek().ToString();
			while (ch != EOF && threeLastChars != "]]>")
			{
				AddCh();
				threeLastChars = threeLastChars[1].ToString() + threeLastChars[2].ToString() + CharProvider.Peek().ToString();
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
			_ShouldReturnCharDataToken = false;
			return t;
		}
		protected Token GetPICharToken()
		{
			Token t = CreateToken();
			t.StartPosition = pos;			
			t.Column = pos - lineStart + 1;
			if (t.Column <= 0)
				t.Column = pos - prevLineStart + 1;
			t.Line = lineValue; 
			t.Type = PICharsTokenType;
			tlen = 0;
			CharProvider.ResetPeek();
			string twoLastChars = ch + CharProvider.Peek().ToString();
			while (ch != EOF && twoLastChars != "?>")
			{
				AddCh();
				twoLastChars = twoLastChars[1].ToString() + CharProvider.Peek().ToString();
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
			ShouldReturnPIChars = false;
			return t;
		}
		protected Token GetXmlTextToken()
		{
			Token t = CreateToken();
			if (ch == EOL)
			{
				t.Column = pos - prevLineStart + 1;
				t.Line = prevLine;
			}
			else
			{
				t.Column = pos - lineStart + 1;
				t.Line = lineValue; 
			}
			t.StartPosition = pos;			
			t.Type = CDataTokenType;
			tlen = 0;
			while (ch != EOF && ch != '<' && !IsReference(ch) && !IsRazorEmbeddingStartChar(ch))
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
			if (ch == EOF && t.Value.Length == 0)
				t.Type = Tokens.EOF;
			return t;
		}
		protected bool IsReference(char currentChar)
		{
			if (currentChar != '&')
				return false;
			char nextCh = CharProvider.Peek();
			if (nextCh != ' ' && nextCh != '\r' && nextCh != '\n')
				return true;
			return false;
		}
		protected virtual int CDataTokenType
		{
			get
			{
				return Tokens.CDATA;
			}
		}
		protected virtual int PICharsTokenType
		{
			get
			{
				return Tokens.PICHARS;
			}
		}
		public bool ShouldReturnPIChars
		{
			get
			{
				return _ShouldReturnPIChars;
			}
			set
			{
				_ShouldReturnPIChars = value;
			}
		}
		public bool ShouldCheckForXmlText
		{
			get
			{
				return _ShouldCheckForXmlText;
			}
			set
			{
				_ShouldCheckForXmlText = value;
			}
		}
		public bool ShouldReturnCharDataToken
		{
			get
			{
				return _ShouldReturnCharDataToken;
			}
			set
			{
				_ShouldReturnCharDataToken = value;
			}
		}
	}
}
