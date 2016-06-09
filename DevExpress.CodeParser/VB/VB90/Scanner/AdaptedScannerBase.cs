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
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class AdaptedScannerBase
	{
		protected const char EOT = (char)129;
		protected const char EOF = (char)65535;
		string _CharsFromPeekTokens = String.Empty;
		int _CharIndex;
		int _CharPeekIndex = 1;
	VBScannerBase _Scanner;
	char _PrevChar = ' ';
	char ch = ' ';
	Token tokens;
	void AddCharsFromPeek(string str)
	{
	  if (str == null || str == String.Empty || CharsFromPeekTokens == null)
		return;
	  CharsFromPeekTokens = CharsFromPeekTokens + str;
	}
	ArrayList GetXmlTokens(Token peekToken, int startIndex)
	{
	  if (peekToken == null)
		return null;
	  ArrayList xmlTokens = new ArrayList();
	  while (peekToken != null)
	  {
		if (peekToken.StartPosition >= startIndex)
		{
		  xmlTokens.Add(peekToken);
		}
		peekToken = peekToken.Next;
	  }
	  return xmlTokens;
	}
	string GetIntervalValue(SourceRange fRange, SourceRange sRange)
	{
	  if (fRange == SourceRange.Empty || sRange == SourceRange.Empty)
		return String.Empty;
	  return GetIntervalValue(fRange.End, sRange.Start);
	}
	string GetIntervalValue(SourcePoint fPoint, SourcePoint sPoint)
	{
	  if (fPoint == SourcePoint.Empty || sPoint == SourcePoint.Empty)
		return String.Empty;
	  int wsCount;
	  string returnStr = String.Empty;
	  if (fPoint.Line == sPoint.Line)
	  {
		wsCount = sPoint.Line - fPoint.Line;
		returnStr = GetWhitespaces(wsCount);
	  }
	  else
	  {
		int lnCount = sPoint.Offset - fPoint.Offset;
		returnStr = GetLineTerms(lnCount);
		returnStr += GetWhitespaces(sPoint.Offset);
	  }
	  return returnStr;
	}
	string GetLineTerms(int count)
	{
	  if (count <= 0)
		return String.Empty;
	  string str = String.Empty;
	  string eolStr = "\r\n";
	  for (int i = 0; i < count; i++)
	  {
		str += eolStr;
	  }
	  return str;
	}
	string GetWhitespaces(int count)
	{
	  if (count <= 0)
		return String.Empty;
	  return new String(' ', count);
	}
	char GetCharsInPeekTokens()
	{
	  if (!HasCharsInPeekTokens())
	  {
		_CharPeekIndex++;
		return ' ';
	  }
	  char returnChar = CharsFromPeekTokens[_CharPeekIndex + CharIndex - 1];
	  _CharPeekIndex++;
	  return returnChar;
	}
	bool HasCharsInPeekTokens()
	{
	  if (CharsFromPeekTokens == String.Empty || CharsFromPeekTokens == null || (CharIndex != 0 && CharsFromPeekTokens.Length <= (_CharPeekIndex + CharIndex - 1)))
		return false;
	  return true;
	}
	protected char PeekChar(out bool correctChar)
	{
	  correctChar = true;
	  return _Scanner.CharProvider.Peek();
	}
	protected void NextCh()
	{
	  _PrevChar = ch;
	  _Scanner.NextChar();
	  ch =_Scanner.CharValue;
	}
	protected void SetCharFromScanner()
	{
	  _PrevChar = ch;
	  ch = _Scanner.CharValue;
	}
	protected void ResetCharPeek()
	{
	  _Scanner.CharProvider.ResetPeek();
	}
		protected void ResetCharsFromPeekTokens()
		{
			CharIndex = 0;
			CharsFromPeekTokens = String.Empty;
		}
		protected void SetCharsFromPeekTokens(ArrayList xmlTokens)
		{
			if (xmlTokens == null || xmlTokens.Count == 0)
				return;
			ResetCharsFromPeekTokens();
			int count = xmlTokens.Count - 1;
			for (int i = 0; i <= count; i++)
			{
				Token fToken = xmlTokens[i] as Token;
				Token sToken = null;
				if (i + 1 <= count)
				{
					sToken = xmlTokens[i + 1] as Token;
				}
				SetCharsFromPeekTokens(fToken, sToken);
			}
		}
		protected void SetCharsFromPeekTokens(Token fToken, Token sToken)
		{
			if (fToken == null)
				return;
			AddCharsFromPeek(fToken.Value);
			if (sToken == null)
			{
				return;
			}
			string str = GetIntervalValue(fToken.Range, sToken.Range);
			AddCharsFromPeek(str);
		}
		protected void ResetPeekIndex()
		{
			_CharPeekIndex = 1;
		}
	protected bool IsEof()
	{
	  return IsEof(ch);
	}
	protected bool IsEof(char chr)
	{
	  return chr == EOF || chr == EOT;
	}
		protected void DeletePeekElements(int startIndex, string peekString)
		{
			if (startIndex <= 0 || tokens == null || tokens == null)
				return;
			Token peekToken = tokens;
			int peekStPos = 0;
	  ArrayList xmlTokens = GetXmlTokens(peekToken, startIndex);
			while (peekToken.Next != null)
			{
				peekStPos = peekToken.Next.StartPosition;
				if (peekStPos >= startIndex)
				{
					peekToken.Next = null;
					break;
				}
				peekToken = peekToken.Next;
			}
			if (xmlTokens != null && xmlTokens.Count != 0)
			{
				SetCharsFromPeekTokens(xmlTokens);
				if (peekString != null && peekString != String.Empty)
				{
					int index = CharsFromPeekTokens.IndexOf(peekString);
		  if (index != 0)
			CharsFromPeekTokens = peekString + CharsFromPeekTokens;
				}
			}
	  else
		CharsFromPeekTokens = peekString;
		}
	protected char GetPeekChar(out bool correctChar)
		{
			ResetPeekIndex();
	  correctChar = false;
			if (HasCharsInPeekTokens())
			{
				correctChar = true;
				return GetCharsInPeekTokens();
			}
	  if (Ch != _Scanner.CharValue)
	  {
		correctChar = true;
		return _Scanner.CharValue;
	  }
	  ResetCharPeek();
	  char nextChar = PeekChar(out correctChar);
			 if (IsEof(nextChar) || !correctChar)
				return nextChar;
			correctChar = true;
			return nextChar;
		}
		protected char GetPeekCharCore(out bool correctChar)
		{
			correctChar = false;
			if (HasCharsInPeekTokens())
			{
				correctChar = true;
				return GetCharsInPeekTokens();
			}
	  return PeekChar(out correctChar);
		}
	protected string CharsFromPeekTokens
	{
	  get { return _CharsFromPeekTokens; }
	  set { _CharsFromPeekTokens = value; }
	}
	protected int CharIndex
	{
	  get { return _CharIndex; }
	  set { _CharIndex = value; }
	}
	protected char PrevChar
	{
	  get { return _PrevChar; }
	  set { _PrevChar = value; }
	}
	protected Token Tokens
	{
	  get { return tokens; }
	  set { tokens = value; }
	}
	protected char Ch
	{
	  get { return ch; }
	  set { ch = value; }
	}
	public void SetScanner(VBScannerBase scanner)
	{
	  _Scanner = scanner;
	}
	public char CurrentChar
	{
	  get
	  {
		return ch;
	  }
	  set
	  {
		ch = value;
	  }
	}
	}
}
