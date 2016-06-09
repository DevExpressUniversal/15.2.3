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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Base;
	public class Token : ICloneable
#if !DXCORE
		,IToken
#endif
	{
		short _Type;
		int _StartPosition;
		int _EndPosition;
		int _Line;
		int _EndLine;
	int _Column;
	int _EndColumn;
		string _Value;
		Token _Next;
		public Token()
			: this(-1, -1, -1, -1, -1, -1, TokenType.Eof, null)
		{
		}
		public Token(int type)
			: this(-1, -1, -1, -1, -1, -1, type, null)
		{
		}
		public Token(SourceRange range, int tokenType, string value)
			: this(-1, -1, range.Start.Line, range.Start.Offset, range.End.Line, range.End.Offset, tokenType, value)
		{
		}
		public Token(int line, int column, int endLine, int endColumn, int tokenType, string value)
			: this(-1, -1, line, column, endLine, endColumn, tokenType, value)
		{
		}		
		public Token(int startPosition, int endPosition, int line, int column, int endLine, int endColumn, int tokenType, string value)
		{
			_StartPosition = startPosition;
			_EndPosition = endPosition;
			_Line = line;
			_Column = column;
			_EndLine = endLine;
			_EndColumn = endColumn;
			_Type = (short)tokenType;
			_Value = tokenType == TokenType.Identifier ? ReplaceUnicodeEscapes(value) : value;
		}
		object System.ICloneable.Clone()
		{
			return Clone();
		}
		string ReplaceUnicodeEscape(string s, int pos) 
		{
			if (s == null)
				return String.Empty;
			string lLeft = s.Substring(0, pos);
			string lCenter = s.Substring(pos, 6);
			string lRight = s.Substring(pos + 6);
			lCenter = lCenter.Substring(2);
			for (int i = 0; i < lCenter.Length; i++)
			{
				if ("0123456789ABCDEFabcdef".IndexOf(lCenter[i]) < 0)
					return s;
			}
			int lValue = Convert.ToInt32(lCenter, 16);
			lCenter = Convert.ToChar(lValue).ToString();
			return lLeft + lCenter + lRight;
		}		
		protected string ReplaceUnicodeEscapes(string s)
		{
			if (s == null)
				return String.Empty;
			string lResult = s;
			int lPos = lResult.IndexOf("\\u");
			while (lPos > -1 && lResult != String.Empty)
			{
				if ((lPos + 6) > lResult.Length)
					break;
				lResult = ReplaceUnicodeEscape(lResult, lPos);
				lPos = lResult.IndexOf("\\u", lPos + 1);
			}
			return lResult;
		}
		#region CloneDataFrom
		protected virtual void CloneDataFrom(Token source, ElementCloneOptions options)
		{
			if (source == null)
				return;						
			_Type = source._Type;
			_StartPosition = source._StartPosition;
			_EndPosition = source._EndPosition;
			_Line = source._Line;
			_EndLine = source._EndLine;
			_Column = source._Column;
			_EndColumn = source._EndColumn;
			_Value = source._Value;
		}
		#endregion
		public SourceRange GetRange()
		{
			return Range;
		}
		public bool Match(int type)
		{
			return _Type == type;
		}
		public override string ToString()
		{
			return EscapedValue + " ";
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Token Clone()
		{
			return Clone(ElementCloneOptions.Default);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Token Clone(ElementCloneOptions options)
		{
			Token lClone = new Token();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}		
		public int StartPosition
		{
			get
			{
				return _StartPosition;
			}
			set
			{
				_StartPosition = value;
			}
		}
		public int EndPosition
		{
			get
			{
				return _EndPosition;
			}
			set
			{
				_EndPosition = value;
			}
		}
		public int Line
		{
			get
			{
				return _Line;
			}
			set
			{
				_Line = value;
			}
		}
		public int EndLine
		{
			get
			{
				if (_EndLine == 0)
					return _Line;
				else
					return _EndLine;
			}
			set
			{
				_EndLine = value;
			}
		}
		public int Column
		{
			get
			{
				return _Column;
			}
			set
			{
				_Column = value;
			}
		}
		public int EndColumn
		{
			get
			{
				if (_EndColumn == 0)
					return _Column;
				return _EndColumn;
			}
			set
			{
				_EndColumn = value;
			}
		}
		public int Type
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = (short)value;
			}
		}
		public SourceRange Range
		{
			get
			{
				return new SourceRange(Line, Column, EndLine, EndColumn);
			}
			set
			{
				_Line = value.Top.Line;
				_Column = value.Top.Offset;				
				_EndLine =  value.Bottom.Line;
				_EndColumn = value.Bottom.Offset;
			}
		}
		public int Length
		{
			get
			{
				if (_EndPosition != -1 && _StartPosition != -1)
					return _EndPosition - _StartPosition;
				else
					if (_Line == _EndLine)
						return _EndColumn - _Column;
				return 0;
			}
		}
		public string EscapedValue
		{
			get
			{
				return Value;
			}
		}
		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
		public Token Next
		{
			get
			{
				return _Next;
			}
			set
			{
				_Next = value;
			}
		}
	}
}
