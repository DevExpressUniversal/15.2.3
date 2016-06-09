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
using System.Collections.Generic;
using System.Text;
using System.Globalization;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class GeneratedScannerBase : IDisposable
  {
	ISourceReader _SourceReader;
		CharProvider _CharProvider;
		bool _OwnReader;
	bool _IsDisposed;
	bool _SaveFormat;
	protected const char EOF = (char)65535;
		protected const char EOL = '\n';
		protected const int eofSym = 0;
		protected char ch;
		protected int pos;
		protected int lineValue;
		protected int lineStart;
		protected int prevLineStart;
		protected int prevLine;
		protected BitArray ignore;
		protected Token t;
		protected Token pt;
		protected Token tokens;
		protected char[] tval = new char[128];
		protected int tlen;
	void CreateFormatingToken()
	{
	  FormattingElements fte = null;
	  if (SaveFormat)
		fte = GetFormatingTokenElement();
	  t = CreateToken();
	  SetFormatingElement(t as FormattingToken, fte);
	}
	void SetFormatingElement(FormattingToken formatingToken, FormattingElements formatingElement)
	{
	  if (formatingToken == null || formatingElement == null)
		return;
	  formatingToken.FormattingElements = formatingElement;
	}
	internal FormattingElements GetFormatingTokenElement()
	{
	  FormattingElements result = null;
	  while (IsFormatChar(ch))
	  {
		if (result == null)
		  result = new FormattingElements();
		FormattingElement fte = CreateFormatingTokenElement(ch);
		if (fte != null)
		  result.Add(fte);
		NextCh();
	  }
	  return result;
	}
	internal FormattingElement CreateFormatingTokenElement(char ch)
	{
	  if (IsWSChar(ch))
		return new FormattingElement(FormattingElementType.WS);
	  if (IsTabChar(ch))
		return new FormattingElement(FormattingElementType.Tab);
	  if (IsEOLChar(ch))
		return new FormattingElement(FormattingElementType.EOL);
	  return null;
	}
		protected void Initialize(ISourceReader s)
		{
			Initialize(s, false);
	}
	protected void Initialize(ISourceReader s, bool ownReader)
		{
			_OwnReader = ownReader;
	  SaveFormat = false;
			IntializeReader(s);
			InitializeIgnoreTable();
			pt = tokens = CreateToken();  
	}
	protected void IntializeReader(ISourceReader s)
		{
			_SourceReader = s;
			_CharProvider = new CharProvider(s.GetStream());
			lineValue = s.StartLine;
			lineStart = s.StartColumn-1;
			if(lineStart > 0)
			{
				pos = lineStart; 
				lineStart = 1;
			}
			else
				pos = -1;
	  NextCh();
	}
	protected static SourceRange GetRange(object a, object b)
		{
			return SourceRangeUtils.GetRange(a, b);
	}
	protected static SourceRange GetRange(object a)
		{
			return SourceRangeUtils.GetRange(a);
	}
	protected static SourceRange GetRange(params object[] objs) 
		{
			return SourceRangeUtils.GetRange(objs);
	}
	protected void BackTrackScannerToToken()
	{
	  pos = t.StartPosition - 1;
	  int startColumn = _SourceReader.StartColumn;
	  if (startColumn - 1 > 0)
		CharProvider.Seek(t.StartPosition - startColumn);
	  else
		CharProvider.Seek(t.StartPosition);
	  lineValue = t.Line;
	  prevLine = lineValue - 1;
	  lineStart = t.StartPosition - t.Column + 1;
	  NextCh();
	  for (int i = 0; i < tlen; i++)
		NextCh();
	}
	protected virtual void AddCh()
	{
	  if (tlen >= tval.Length)
	  {
		char[] newBuf = new char[tval.Length * 2];
		Array.Copy(tval, 0, newBuf, 0, tval.Length);
		tval = newBuf;
	  }
	}
	protected virtual void NextCh()
	{
	  ch = _CharProvider.Get();
	  pos++;
	  if (ch == EOL)
	  {
		prevLine = lineValue;
		lineValue++;
		prevLineStart = lineStart;
		lineStart = pos + 1;
	  }
	  NextChCasing();
	}
	protected virtual Token CreateToken()
		{
			return new Token();
	}
	protected virtual void InitializeIgnoreTable()
		{
	}
	protected virtual int GetUnicodeLetterIndex()
		{
			return -1;
	}
	protected virtual int GetNextState(int input)
		{
			return -1;
	}
	protected virtual void NextChCasing()
		{
	}
	protected virtual Token NextToken()
	{
	  bool stop = NextTokenStart();
	  if (stop)
		return t;
	  NextTokenComments();
	  CreateFormatingToken();
	  t.StartPosition = pos;
	  t.Column = pos - lineStart + 1;
	  t.Line = lineValue;
	  int state;
	  if (ch == EOF)
		state = -1;
	  else
	  {
		if (ch > 255)
		  if (char.IsLetter(ch))
			state = GetNextState(GetUnicodeLetterIndex());
		  else
			state = 0;
		else
		  state = GetNextState(ch);
	  }
	  tlen = 0;
	  NextTokenScan(state);
	  if (t.Value == null)
		t.Value = GlobalStringStorage.Intern(new String(tval, 0, tlen));
	  else
		t.Value = GlobalStringStorage.Intern(t.Value);
	  NextTokenAfterScan();
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
	  NextTokenEnd();
	  if (SaveFormat)
	  {
		SetInPreprocess();
		FormattingElements fte = GetNextFormattingTokenElements();
		FormattingToken ft = t as FormattingToken;
		if (fte != null && ft != null)
		  ft.SetFormattingElement(fte);
		ResetInPreprocess();
	  }
	  return t;
	}
	protected virtual bool NextTokenStart()
		{
			SkipIgnoredChars();
			return false;
	}
	protected virtual void NextTokenComments()
		{
	}
	protected virtual void NextTokenScan(int state)
		{
		}
		protected virtual void NextTokenAfterScan()
		{
	}
	protected virtual void NextTokenEnd()
		{
	}
	protected virtual void SetInPreprocess()
	{
	}
	protected virtual void ResetInPreprocess()
	{
	}
	protected virtual void SkipIgnoredChars()
		{
			while ((ch != EOF) && (ch < 255) && (ignore[ch]))
				NextCh();
	}
	protected virtual FormattingElements GetNextFormattingTokenElements()
	{
	  return GetFormatingTokenElement();
	}
	protected virtual bool NeedSaveFormat()
	{
	  if (!SaveFormat)
		return false;
	  return IsFormatChar(ch);
	}
	protected virtual bool IsFormatChar(char ch)
	{
	  return IsWSChar(ch) || IsTabChar(ch) || IsEOLChar(ch) || IsCRChar(ch);
	}
	protected virtual bool IsWSChar(char ch)
	{
	  return ch == ' ';
	}
	protected virtual bool IsTabChar(char ch)
	{
	  return ch == '\u0009';
	}
	protected virtual bool IsEOLChar(char ch)
	{
	  return ch == '\u000a';
	}
	protected virtual bool IsCRChar(char ch)
	{
	  return ch == '\u000d';
	}
	public void Dispose()
	{
	  _IsDisposed = true;
	  _CharProvider = null;
	  if (_OwnReader && _SourceReader != null)
	  {
		_SourceReader.Dispose();
		_SourceReader = null;
	  }
	}
	public virtual Token Scan()
		{			
			if (tokens.Next == null)
				pt = tokens = NextToken();
			else
				pt = tokens = tokens.Next;
			return tokens;
	}
	public Token Peek(int pos)
		{
			Token result = null;
			for (int i = 0; i < pos; i++)
				result = Peek();
			return result;
	}
	public virtual Token Peek()
		{
	  if (pt.Next == null)
		pt.Next = NextToken();
	  pt = pt.Next;
			return pt;
	}
	public void ResetPeek()
		{
			pt = tokens;
		}
	public void SyncPosition(int positionDelta)
	{
	  SyncPosition(positionDelta, false);
	}
	public void SyncPosition(int positionDelta, bool addChars)
	{
	  for (int i = 0; i < positionDelta; i++)
	  {
		if (addChars)
		  AddCh();
		else
		  NextCh();
	  }
	}
		public void SetTokens(Token newTokens)
		{
			if (newTokens == null)
				return;
			tokens = newTokens;
		}
		public Token GetTokens()
		{
			return tokens;
		}
		public Token GetPt()
		{
			return pt;
		}
		public void SetOnPeek(Token oldPt, Token newPt)
		{
			if (oldPt == null || newPt == null)
				return;
			oldPt.Next = newPt;
			pt = newPt;
		}
	public bool IsUnicodeLetter(char ch)
	{
	  if (char.IsLetter(ch))
		return true;
	  UnicodeCategory category = char.GetUnicodeCategory(ch);
	  switch (category)
	  {
		case UnicodeCategory.NonSpacingMark:
		case UnicodeCategory.SpacingCombiningMark:
		case UnicodeCategory.EnclosingMark:
		  return true;
	  }
	  return false;
	}
	public string FirstSkip()
	{
	  StringBuilder text = new StringBuilder();
	  while (!IsEof(text) && (IsWSChar(ch) || IsCRChar(ch) || IsTabChar(ch) || IsEOLChar(ch)))
	  {
		text.Append(ch);
		NextCh();
	  }
	  return text.ToString();
	}
	public bool IsEof(StringBuilder sb)
	{
	  return IsNextChar(EOF, sb);
	}
	public bool IsNextChar(char c, StringBuilder sb)
	{
	  while (ch == ' ' || ch == '\u0009')
	  {
		sb.Append(ch);
		NextCh();
	  }
	  return ch == c;
	}
	public virtual ISourceReader SourceReader
		{
			get
			{
				return _SourceReader;
			}
	}
	public CharProvider CharProvider
		{
			get
			{
				return _CharProvider;
			}
	}
	public virtual int Position
	{
	  get
	  {
		return pos + 1;
	  }
	}
	public int Line
	{
	  get
	  {
		return lineValue;
	  }
	}
	public int Column
	{
	  get
	  {
		return pos - lineStart;
	  }
	}
	public char CurrentChar
	{
	  get
	  {
		return ch;
	  }
	}
	public bool IsDisposed
	{
	  get { return _IsDisposed; }
	}
	public bool SaveFormat
	{
	  set { _SaveFormat = value; }
	  get { return _SaveFormat; }
	}
	public int EndLineValue
	{
	  get
	  {
		if (ch != EOL)
		  return lineValue;
		return prevLine;
	  }
	}
	internal Token tToken
	{
	  get
	  {
		return t;
	  }
	}
  }
}
