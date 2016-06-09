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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class SourceLinesReader : ISourceReader
	{
		SourceLineCollection _SourceLines;
		string _Text;
		bool _Eof;
		int _StartLine;
		int _StartColumn;
		int _CurrentLine;
		int _CurrentColumn;
		int _Line;
		int _LinePos;
		#region SourceLinesReader
		public SourceLinesReader(SourceLineCollection lines)
		{
			_SourceLines = lines;
			_StartLine = 1;
			_StartColumn = 1;
			_CurrentLine = 1;
			_CurrentColumn = 1;
			if (lines != null && lines.Count > 0)
			{
				SourceLine lFirstLine = lines[0];
				_StartLine = lFirstLine.Start.Line;
				_StartColumn = lFirstLine.Start.Offset;
				_CurrentLine = _StartLine;
				_CurrentColumn = _StartColumn;
			}
			_Text = GetText();
		}
		#endregion
		public TextReader GetStream() 
		{
			throw new NotImplementedException();
		}
		#region CurrentChar()
		public char CurrentChar()
		{
			if (SourceLines == null || LineCount == 0)
			{
				_Eof = true;
				return '\0';
			}
			if (_Line >= 0 && _Line <= LineCount - 1)
			{
				SourceLine lLine = SourceLines[_Line];
				return lLine.Text[_LinePos];
			}
			return '\0';
		}
		#endregion
		#region NextChar
		public char NextChar()
		{
			if (SourceLines == null || LineCount == 0)
			{
				_Eof = true;
				return '\0';
			}
			if (_Line == LineCount - 1 && _LinePos >= SourceLines[_Line].Text.Length - 1)
			{
				_Eof = true;
				return '\0';
			}
			if (_Line >= 0 && _Line <= LineCount - 1)
			{
				SourceLine lLine = SourceLines[_Line];
				if (_LinePos == lLine.Text.Length - 1)
				{
					_Line ++;
					_LinePos = 0;
					lLine = SourceLines[_Line];
					CurrentLine = lLine.Start.Line;
					CurrentColumn = lLine.Start.Offset;
					return lLine.Text[_LinePos];
				}
				CurrentColumn ++;
				return lLine.Text[++_LinePos];
			}
			_Eof = true;
			return '\0';
		}
		#endregion
		#region UndoChar
		public void UndoChar()
		{
			if (SourceLines == null || LineCount == 0)
				return;
			if (_Line == 0 && _LinePos == 0)
				return;
			if (_Line >= 0 && _Line <= LineCount - 1)
			{
				SourceLine lLine = SourceLines[_Line];
				if (_LinePos == 0)
				{
					_Line --;
					lLine = SourceLines[_Line];
					_LinePos = lLine.Text.Length - 1;
					CurrentLine = lLine.Start.Line;
					CurrentColumn = lLine.Start.Offset + _LinePos;
					return;
				}
				CurrentColumn --;
				_LinePos --;
			}
		}
		#endregion
		#region Text
		public string Text(int aStartPos, int aLength)
		{
			return _Text.Substring(aStartPos, aLength);
		}
		#endregion
		#region Eof
		public bool Eof()
		{
			return _Eof;
		}
		#endregion
		public void IncreaseColumn()
		{
			_CurrentColumn++;
		}
		#region Position
		public int Position()
		{
			return ToPosition(Line, LinePos);
		}
		#endregion
		#region Seek
		public void Seek(int position)
		{
			int lPos = 0;
			int lMaxPos = GetMaxPosition();
	  if (position < 0)
				lPos = 0;
			else if (position >= lMaxPos)
				lPos = lMaxPos;
			else
				lPos = position;
			if (lPos == lMaxPos)
				_Eof = true;
			else
				_Eof = false;
			SourcePoint lPoint = ToPoint(lPos);
			_Line = lPoint.Line;
			_LinePos = lPoint.Offset;
			SourceLine lLine = SourceLines[_Line];
			CurrentLine = lLine.Start.Line;
			CurrentColumn = _LinePos + lLine.Start.Offset;
		}
		#endregion
		#region ToPosition(int line, int linePos)
		public int ToPosition(int line, int linePos)
		{
			if (SourceLines == null || SourceLines.Count == 0)
				return -1;
	  int lPosition = 0;
			int lLine = line;
			int lLinePos = linePos;
			if (lLine < 0)
				lLine = 0;
			if (lLinePos < 0)
				lLinePos = 0;
			if (lLine > SourceLines.Count - 1)
				lLine = SourceLines.Count -1;
			if (lLinePos > SourceLines[lLine].Text.Length - 1)
				lLinePos = SourceLines[lLine].Text.Length - 1;
			for (int i = 0; i < lLine; i++)
				lPosition += SourceLines[i].Text.Length;
			lPosition += lLinePos;
			return lPosition;
		}
		#endregion
		#region ToPoint
		public SourcePoint ToPoint(int pos)
		{
			if (SourceLines == null || SourceLines.Count == 0)
				return SourcePoint.Empty;
			int lPos = pos;
			if (lPos < 0)
				lPos = 0;
			int lMaxPos = GetMaxPosition();
			if (lPos > lMaxPos)
				lPos = lMaxPos;
			int lLine = 0;
			int lLinePos = 0;
			int lLeftIndex = 0;
			int lRightIndex = 0;
			for (int i = 0; i < LineCount; i++)
			{
				SourceLine lSourceLine = SourceLines[i];
				int lLength = lSourceLine.Text.Length;
				if (i == 0)
					lRightIndex = lLength - 1;
				if (lPos <= lRightIndex)
				{
					lLine = i;
					lLinePos = lPos - lLeftIndex;
					break;
				}
				lLeftIndex = lRightIndex + 1;
				lRightIndex += lLength;
			}
			return new SourcePoint(lLine, lLinePos);
		}
		#endregion
		#region GetMaxPosition()
		public int GetMaxPosition()
		{
			if (SourceLines == null || SourceLines.Count == 0)
				return -1;
			return _Text.Length - 1;
		}
		#endregion
		#region GetText()
		public string GetText()
		{
			StringBuilder lText = new StringBuilder();
			if (SourceLines == null || SourceLines.Count == 0)
				return String.Empty;
			for (int i = 0; i < LineCount; i++)
				lText.Append(SourceLines[i].Text);
			return lText.ToString();
		}
		#endregion
		#region StartLine
		public int StartLine
		{
			get
			{
				return _StartLine;
			}
		}
		#endregion
		#region StartColumn
		public int StartColumn
		{
			get
			{
				return _StartColumn;
			}
		}
		#endregion
		#region CurrentLine
		public int CurrentLine
		{
			get
			{
				return _CurrentLine;
			}
			set
			{
				if (_CurrentLine == value)
					return;
				_CurrentLine = value;
			}
		}
		#endregion
		#region CurrentColumn
		public int CurrentColumn
		{
			get
			{
				return _CurrentColumn;
			}
			set
			{
				if (_CurrentColumn == value)
					return;
				_CurrentColumn = value;
			}
		}
		#endregion
		bool ISourceReader.IsDocumentReader 
		{
			get
			{
				return false;
			}
		}
		public void OffsetSubStream(int line, int column) 
		{
			throw new NotImplementedException("SourceLinesReader.OffsetSubStream");
		}
		public ISourceReader GetSubStream(int startPos, int length, int line, int column)
		{
			throw new NotImplementedException("SourceLinesReader.GetSubStream");
		}
		#region SourceLines
		public SourceLineCollection SourceLines
		{
			get
			{
				return _SourceLines;
			}
		}
		#endregion
		#region LineCount
		public int LineCount
		{
			get
			{
				return SourceLines.Count;
			}
		}
		#endregion
		#region Line
		public int Line
		{
			get
			{
				return _Line;
			}
		}
		#endregion
		#region LinePos
		public int LinePos
		{
			get
			{
				return _LinePos;
			}
		}
		#endregion
		#region ManagesCoordinates
		public bool ManagesCoordinates
		{
			get
			{
				return true;
			}
		}
		#endregion
	public bool IsDisposing
	{
	  get { return false; }
	}
	public bool IsDisposed
	{
	  get { return false; }
	}
	public void Dispose()
	{
	}
  }
}
