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
using System.Text;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif 
{
	public class CodeWriter 
	{
		#region private fields...
		CodeGenOptions _Options;
	TextWriter _Writer;
		int _IndentLevel;
	int _PrecedingWhiteSpaceCount;
		string _IndentString;
		string _LastLine = String.Empty;
	readonly string _LineContinuation; 
	public bool WriteWithoutIndentStringOneTime;
	int _LineTerminatorCount;
	Stack<string> _Alignment = new Stack<string>();
	Stack<int> _IndentLevelStack = new Stack<int>();
	#endregion
	public void IncreaseAlignment()
	{
	  string lastLineWithoutIndent = LastLineInSpacesWithouIndent;
	  if (lastLineWithoutIndent == null)
		lastLineWithoutIndent = String.Empty;
	  _Alignment.Push(lastLineWithoutIndent);
	}
	public void DecreaseAlignment()
	{
	  if (_Alignment.Count == 0)
		return;
	  _Alignment.Pop();
	}
	string Alignment
	{
	  get
	  {
		if (_Alignment.Count == 0)
		  return String.Empty;
		return _Alignment.Peek();
	  }
	}
	public CodeWriter(TextWriter writer, CodeGenOptions options, string lineContinuation) {
		_LineContinuation = lineContinuation;
		_Writer = writer;
		_Options = options;
	}
	public CodeWriter(TextWriter writer, CodeGenOptions options)
		: this(writer, options, String.Empty) {
	}
	public CodeWriter() : this(new StringWriter(new StringBuilder()), CodeGenOptions.Default)
	{
	}
		string GetIndentString(int level) 
		{
			if (level == 0)
				return String.Empty;
			StringBuilder lWorker = new StringBuilder();
			for (int i = 0; i < level; i++)
				lWorker.Append(Options.IndentString);
			return lWorker.ToString();
		}
		string GetPrecedingString()
		{
	  return new string(' ', _PrecedingWhiteSpaceCount);
		}
		string[] SplitLines(string s)
		{
	  return StringHelper.SplitLines(s, false);
		}
		string[] SplitLines(string s, bool split)
		{
			return split ? SplitLines(s) : new string[1] {s};
		}
	void WriteInternal(string s)
	{
	  _Writer.Write(s);
	  _LastLine += s;
	}
		public void Indent() 
		{
			_Writer.Write(IndentString);
		}
		public void IncreaseIndent() 
		{
	  IndentLevel++;
		}
		public void DecreaseIndent() 
		{
	  if (IndentLevel <= 0)
		return;
	  IndentLevel--;
		}
	public void WriteLines(string[] lines, bool finishWithCR)
	{
	  int count = lines.Length;
	  if (count == 0)
		return;
	  string line;
	  if (LastLine == String.Empty)
		line = IndentString + Alignment + lines[0];
	  else
		line = lines[0];
	  if (count == 1 && !finishWithCR)
		WriteInternal(line);
	  else
	  {
		_Writer.WriteLine(line);
		_LineTerminatorCount++;
		_LastLine = String.Empty;
	  }
	  for (int i = 1; i < count; i++)
	  {
		line = (lines[i] == String.Empty) ? String.Empty : IndentString + Alignment + lines[i];
		if (i + 1 == count)
		{
		  if (finishWithCR)
		  {
			_Writer.WriteLine(line);
			_LineTerminatorCount++;
			_LastLine = String.Empty;
		  }
		  else
			WriteInternal(line);
		}
		else
		{
		  _Writer.WriteLine(line);
		  _LineTerminatorCount++;
		}
	  }
	}
		public void Write(string s)
		{
			Write(s, true);
		}
		public void Write(string s, bool split)
		{
			WriteLines(SplitLines(s, split), false);
		}
		public void Write(string format, params object[] arg)
		{
			String s = String.Format(format, arg);
			WriteLines(SplitLines(s), false);
		}
		public void WriteClearFormat(string s)
		{
			string tempIndentString = IndentString;
			IndentString = String.Empty;
			Write(s);
			IndentString = tempIndentString;
		}
		public void WriteLine(string s)
		{
			WriteLine(s, true);
		}
		public void WriteLine(string s, bool split)
		{
			if (s == String.Empty)
				WriteLine();
			else
				WriteLines(SplitLines(s, split), true);
		}
		public void WriteLine(string format, params object[] arg)
		{
			String s = String.Format(format, arg);
			WriteLines(SplitLines(s, false), true);
		}
		public void WriteLine()
		{
			_Writer.WriteLine();
	  _LineTerminatorCount++;
			_LastLine = String.Empty;
		}
		public void WriteOpenBrace(LanguageElement element)
		{
			if (_Options == null && _Options.BraceSettings == null)
				WriteLine(" {");
			BracesOnNewLineOptions braceSetting = _Options.BraceSettings[element];
			switch (braceSetting)
			{
				case BracesOnNewLineOptions.LeaveOnPrevious:
					WriteLine(" {");
					break;
				case BracesOnNewLineOptions.PlaceOnNewLine:
		  if (!string.IsNullOrEmpty(LastLine))
			WriteLine();
					WriteLine("{");
					break;
			}
		}
	public void ClearIndent()
	{
	  _IndentLevelStack.Push(IndentLevel);
	  IndentLevel = 0;
	}
	public void RestoreIndent()
	{
	  if (_IndentLevelStack.Count == 0)
		return;
	  IndentLevel = _IndentLevelStack.Pop();
	}
		protected internal string IndentString 
		{
			get 
			{
		if (_Options != null && _Options.SaveFormat)
		  return String.Empty;
				if (WriteWithoutIndentStringOneTime)
				{
					WriteWithoutIndentStringOneTime = false;
					return String.Empty;
				}
				string precedingString = GetPrecedingString();
				return precedingString + _IndentString;
			}
			set 
			{
				if (_IndentString == value)
					return;
				_IndentString = value;
			}
		}
	public int IndentLevel
	{
	  get { return _IndentLevel; }
	  set
	  {
		if (_IndentLevel == value)
		  return;
		_IndentLevel = value;
		IndentString = GetIndentString(_IndentLevel);
	  }
	}
	public string Code 
		{
			get 
			{
				return _Writer.ToString();
			}
		}
		public CodeGenOptions Options 
		{
			get 
			{
				return _Options;
			}
		}
		public bool IsAtWhitespace
		{
			get
			{
				string code = Code;
				if (code == null || code.Length == 0)
					return false;
				return Char.IsWhiteSpace(code[code.Length - 1]);
			}
		}
	public int PrecedingWhiteSpaceCount
	{
	  get { return _PrecedingWhiteSpaceCount; }
	  set { _PrecedingWhiteSpaceCount = value; }
	}
	public int LineTerminatorCount
	{
	  get { return _LineTerminatorCount; }
	  set { _LineTerminatorCount = value; }
	}
	public string LastLine
	{
	  get { return _LastLine; }
	}
	public string LastLineInSpaces
	{
	  get
	  {
		return new string(' ', LastLine.Length);
	  }
	}
	public string LastLineInSpacesWithouIndent
	{
	  get
	  {
		string lastLineInSpaces = LastLineInSpaces;
		if (string.IsNullOrEmpty(lastLineInSpaces))
		  return string.Empty;
		return lastLineInSpaces.Remove(0, IndentLevel * Options.IndentString.Length);
	  }
	}
  }
}
