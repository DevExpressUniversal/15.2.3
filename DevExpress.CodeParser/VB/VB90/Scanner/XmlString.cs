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
	public class XmlString : AdaptedScannerBase
	{
	const int XML_CHARS_SIZE = 5000;
		int _XmlStack;
		char[] _XmlChars = new char[XML_CHARS_SIZE];
		int _XmlCharIndex;
	void PrepareXmlString(int startBufferIndex, string peekString, Token tokens)
	{
	  Tokens = tokens;
	  if (peekString == null)
		peekString = "<";
	  ResetCharsFromPeekTokens();
	  DeletePeekElements(startBufferIndex, peekString);
	  ResetXmlSettings();
	  WriteStep();
	}
	bool IsPeekCharsCore(char[] mas, int startIndex)
	{
	  if (mas == null || mas.Length < startIndex)
		return false;
	  ResetCharPeek();
	  ResetPeekIndex();
	  for (int i = startIndex; i < mas.Length; i++)
	  {
		char sym = mas[i];
		bool correctValue = false;
		char nextChar = GetPeekCharCore(out correctValue);
		if (!correctValue || IsEof(nextChar) || sym != nextChar)
		  return false;
	  }
	  return true;
	}
		char[] DoubleXmlChars(char[] buf)
		{
	  char[] newbuf = new char[2 * buf.Length];
	  Array.Copy(buf, newbuf, buf.Length);
			return newbuf;
		}
	void DeleteWhitespaces()
	{
	  while (IsNotSignificantChar())
	  {
		WriteStep();
	  }
	}
	bool BodyStep()
	{
	  if (IsEof() || _XmlStack == 0)
		return false;
	  WriteStep();
	  if (IsEof())
		return false;
	  return true;
	}
	bool IsOneTagXmlString()
	{
	  if (Ch != '<' || !PeekCharIsIdentifier())
		return false;
	  while (!IsPeekChar('>') && !IsEof())
	  {
		StepCore();
	  }
	  return true;
	}
	bool IsTag()
	{
	  bool wasTag = IsTagBegin();
	  bool wasTagEnd = IsTagEnd();
	  bool wasXmlComment = IsXmlComment();
	  bool wasDtdComment = IsDtdTag();
	  bool wasEmbeddedCode = SkipEmbeddedCode();
	  return wasTag || wasTagEnd || wasXmlComment || wasDtdComment || wasEmbeddedCode;
	}
	bool IsPrologTag()
	{
	  if (Ch != '<' || !IsPeekChar('?'))
		return false;
	  WriteStep();
	  SkipToGT();
	  return true;
	}
	bool IsNotSignificantChar()
	{
	  char[] NotSignificantChars = { ' ', '\r', '\n', '\t' };
	  if (NotSignificantChars == null || NotSignificantChars.Length == 0)
		return false;
	  int length = NotSignificantChars.Length;
	  for (int i = 0; i < length; i++)
	  {
		char notSignChar = NotSignificantChars[i];
		if (Ch == notSignChar)
		  return true;
	  }
	  return false;
	}
	bool IsDtdTag()
	{
	  if (Ch != '<' || !IsPeekChar('!'))
		return false;
	  SkipToGT();
	  return true;
	}
	bool IsTagBegin()
	{
	  if (Ch != '<' || !PeekCharIsIdentifier())
		return false;
	  _XmlStack++;
	  while (!IsTagCloseSymbol() && !IsOneLineTagEnd() && !IsEof())
	  {
		Step();
	  }
	  return true;
	}
	bool IsTagCloseSymbol()
	{
	  return Ch == '>' && PrevChar != '%';
	}
	bool IsOneLineTagEnd()
	{
	  if (Ch != '/')
		return false;
	  Step();
	  if (IsEof() || Ch != '>')
		return false;
	  _XmlStack--;
	  return true;
	}
	bool IsTagEnd()
	{
	  if (Ch != '<' || !IsPeekChar('/'))
		return false;
	  SkipToGT();
	  _XmlStack--;
	  return true;
	}
	bool IsIdentifierStart(char chr)
	{
	  int num = chr;
	  return ((90 >= num && num >= 65)
		|| (122 >= num && num >= 97) || (Ch == '_'));
	}
	bool IsXmlComment()
	{
	  if (IsEof() || !IsPeekChars('<', '!', '-', '-'))
		return false;
	  SkipToEndXmlComment();
	  return true;
	}
	bool IsPeekChar(char peekChar)
	{
	  bool correctChar = false;
	  char nextChar = GetPeekChar(out correctChar);
	  if (!correctChar)
		return false;
	  return peekChar == nextChar;
	}
	bool IsPeekChars(params char[] mas)
	{
	  if (mas == null || mas.Length == 0 || Ch != mas[0])
		return false;
	  if (mas.Length == 1)
		return true;
	  return IsPeekCharsCore(mas, 1);
	}
	bool PeekCharIsIdentifier()
	{
	  bool correctChar = false;
	  char peekChar = GetPeekChar(out correctChar);
	  if (!correctChar || IsEof(peekChar))
		return false;
	  return IsIdentifierStart(peekChar) || peekChar == '<';
	}
	void SkipToGT()
	{
	  while (!IsTagCloseSymbol() && !IsEof())
	  {
		StepCore();
	  }
	}
	bool SkipEmbeddedCode()
	{
	  if (!IsPeekChars('<', '%', '='))
		return false;
	  SkipToEndEmbCode();
	  return true;
	}
	void SkipCData()
	{
	  if (!IsPeekChars('[', 'C', 'D', 'A', 'T', 'A'))
		return;
	  SkipToEndCdata();
	}
	bool SkipString()
	{
	  if (Ch == '"' || Ch == '\'')
		SkipToChar(Ch);
	  return true;
	}
	void SkipToChar(char sym)
	{
	  if (IsEof())
		return;
	  WriteStep();
	  while (Ch != sym && !IsEof())
	  {
		WriteStep();
	  }
	}
	void SkipToEndCdata()
	{
	  while (!IsPeekChars(']', ']', '>') && !IsEof())
	  {
		WriteStep();
	  }
	  SkipChars(2);
	}
	void SkipToEndXmlComment()
	{
	  while (!IsPeekChars('-', '-', '>') && !IsEof())
	  {
		WriteStep();
	  }
	  SkipChars(2);
	}
	void SkipToEndEmbCode()
	{
	  while (!IsPeekChars('%', '>') && !IsEof())
	  {
		StepCore();
	  }
	  SkipChars(1);
	}
	void SkipChars(int count)
	{
	  for (int i = 0; i < count; i++)
	  {
		if (IsEof())
		  return;
		WriteStep();
	  }
	}
	bool SkipPrologTags()
	{
	  bool returnValue = false;
	  while (IsPrologTag())
	  {
		WriteStep();
		DeleteWhitespaces();
		returnValue = true;
	  }
	  return returnValue;
	}
	void SkipXmlBody()
	{
	  while (BodyStep())
	  {
		IsTag();
	  }
	}
	void SkipInternalComment()
	{
	  while (IsXmlComment())
	  {
		WriteStep();
		DeleteWhitespaces();
	  }
	}
	bool Step()
	{
	  if (IsEof() || _XmlStack == 0)
		return false;
	  StepCore();
	  return !IsEof();
	}
	void StepCore()
	{
	  WriteStep();
	  SkipString();
	  SkipCData();
	  SkipEmbeddedCode();
	}
	bool _FromCharTokens;
	void WriteStep()
	{
	  if (CharsFromPeekTokens != null && CharsFromPeekTokens.Length != 0 && CharsFromPeekTokens.Length > CharIndex)
	  {
		Ch = CharsFromPeekTokens[CharIndex];
		CharIndex++;
		_FromCharTokens = true;
	  }
	  else
	  {
		if (_FromCharTokens)
		{
		  _FromCharTokens = false;
		  SetCharFromScanner();
		}
		else
		  if (_XmlCharIndex != 0)
			NextCh();
	  }
	  if (IsEof())
		return;
	  if (_XmlCharIndex >= _XmlChars.Length)
		_XmlChars = DoubleXmlChars(_XmlChars);
	  _XmlChars[_XmlCharIndex] = Ch;
	  _XmlCharIndex++;
	}
	void ResetXmlSettings()
	{
	  _XmlStack = 0;
	  _XmlChars = new char[XML_CHARS_SIZE];
	  _XmlCharIndex = 0;
	}
		public string GetXmlImports(int startBufferIndex, Token tokens)
		{
			return GetXmlOneTag(startBufferIndex, tokens);
		}
	public string GetXmlElementRef(int startBufferIndex, Token tokens)
	{
	  return GetXmlOneTag(startBufferIndex, tokens);
	}
		public string GetXmlOneTag(int startBufferIndex, Token tokens)
		{
	  PrepareXmlString(startBufferIndex, "<", tokens);
			if (IsOneTagXmlString())
				return new String(_XmlChars, 0, _XmlCharIndex);
			return String.Empty;
		}
	public string GetXmlString(int startBufferIndex, Token tokens)
	{
	  return GetXmlString(startBufferIndex, "<", tokens);
	}
	public string GetXmlString(int startBufferIndex, string peekString, Token tokens)
		{
	  PrepareXmlString(startBufferIndex, peekString, tokens);
			SkipPrologTags();
			SkipInternalComment();
			if (!IsTag() || (_XmlStack == 0))
				return new String(_XmlChars, 0, _XmlCharIndex);
			SkipXmlBody();
			return new String(_XmlChars, 0, _XmlCharIndex);
		}
	}
}
