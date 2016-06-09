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
using System.Runtime.InteropServices;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct TextRange : ICloneable
	{
		public static readonly TextRange Empty;
		[FieldOffset(0)]
		public TextPoint Start;
		[FieldOffset(8)]
		public TextPoint End;
		public TextRange(int startLine, int startOffset, int endLine, int endOffset)
			: this(new TextPoint(startLine, startOffset), new TextPoint(endLine, endOffset))
		{
		}
		public TextRange(TextPoint start, TextPoint end) 
		{
			Start = start;
			End = end;
		}
		public void Set(int startLine, int startOffset, int endLine, int endOffset) 
		{
			Start.Set(startLine, startOffset);
			End.Set(endLine, endOffset);
		}
		public void Set(SourcePoint start, SourcePoint end) 
		{
			Start.Set(start);
			End.Set(end);
		}
		public void Set(SourceRange range) 
		{
			Start.Set(range.Start);
			End.Set(range.End);
		}
		public void SetStart(TextPoint point) 
		{
			Start.Set(point);
		}
		public void SetEnd(TextPoint point) 
		{
			End.Set(point);
		}
		public bool Contains(int line, int offset)
		{
			if (line < Start.Line || line > End.Line)
				return false;
			bool lResult = true;
			if (line == Start.Line)
				lResult = offset >= Start.Offset;
			if (line == End.Line)
				lResult &= offset <= End.Offset;
			return lResult;
		}
	public bool Contains(TextPoint point)
	{
	  return Contains(point.Line, point.Offset);
	}
		public bool Contains(TextRange range)
		{
			return Contains(range.Start) && Contains(range.End);
		}
		public override bool Equals(object obj)
		{
			if (obj is TextRange)
				return Equals((TextRange)obj);
			return false;
		}
		public bool Equals(TextRange range)
		{
			return Start.Equals(range.Start) && End.Equals(range.End);
		}
		public override int GetHashCode()
		{
	  string s = ToString();
	  return s.GetHashCode();
		}
	public override string ToString()
	{
	  return string.Format("{0} -> {1}", Start, End);
	}
		public SourceRange ToSourceRange()
		{
			SourceRange range = SourceRange.Empty;
			range.SetFast(Start.Line, Start.Offset, End.Line, End.Offset);
			return range;
		}
		#region operator ==
		public static bool operator==(TextRange range1, TextRange range2)
		{
			return range1.Equals(range2);
		}
		#endregion
		#region operator !=
		public static bool operator!=(TextRange range1, TextRange range2)
		{
			return !(range1 == range2);
		}
		#endregion		
		public bool IsPoint
		{
			get
			{
				return Start == End;
			}
		}
		public bool IsEmpty
		{
			get
			{
				return Start == TextPoint.Empty && End == TextPoint.Empty;
			}
		}
		public int LineCount
		{
			get
			{
				return LineCountMinusOne + 1;
			}
		}
		public int LineCountMinusOne
		{
			get
			{
				return Math.Abs(End.Line - Start.Line);
			}
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
		public TextRange Clone()
		{
			return new TextRange(this.Start, this.End);
		}
		public static implicit operator SourceRange(TextRange range) 
		{
			return new SourceRange(range.Start, range.End);
		}
		public static implicit operator TextRange(SourceRange range) 
		{
			return new TextRange(range.Start, range.End);
		}
	}
}
