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
  [Serializable]
  public struct SourceRange : ICloneable
	{
	#region internal fields...
	public SourcePoint Start;
	public SourcePoint End;
	#endregion
	#region public static readonly fields...
	public static readonly SourceRange Empty = new SourceRange(SourcePoint.Empty, SourcePoint.Empty);
	#endregion
	#region SourceRange(int line, int offset)
	public SourceRange(int line, int offset)
	{
	  Start = new SourcePoint(line, offset);
	  End = new SourcePoint(line, offset);
	}
	#endregion
	#region SourceRange(SourcePoint sourcePoint)
	public SourceRange(SourcePoint sourcePoint)
	{
		Start = new SourcePoint(sourcePoint.Line, sourcePoint.Offset);
	  End = Start;
	}
	#endregion
	#region SourceRange(int startLine, int startOffset, int endLine, int endOffset)
	public SourceRange(int startLine, int startOffset, int endLine, int endOffset)
	{
	  Start = new SourcePoint(startLine, startOffset);
	  End = new SourcePoint(endLine, endOffset);
	}
	#endregion
	#region SourceRange(SourcePoint start, SourcePoint end)
	public SourceRange(SourcePoint start, SourcePoint end)
	{
	  Start = new SourcePoint(start.Line, start.Offset);
	  End = new SourcePoint(end.Line, end.Offset);
	}
	#endregion
		#region SourceRange(SourceRange range)
		public SourceRange(SourceRange range)
		{
			Start = new SourcePoint(range.Start.Line, range.Start.Offset);
			End = new SourcePoint(range.End.Line, range.End.Offset);
		}
		#endregion
		public SourceRange(TextPoint start, TextPoint end) {
			Start = start;
			End = end;
		}
		void GetTopAndBottom(out SourcePoint top, out SourcePoint bottom)
		{
			top = Start;
			bottom = End;
			if (!StartPrecedesEnd)
			{
				SourcePoint temp = top;
				top = bottom;
				bottom = temp;
			}
		}
		#region AdjustForInsertion
		public void AdjustForInsertion(SourceRange insertion)
		{
			Start.AdjustForInsertion(insertion);
			End.AdjustForInsertion(insertion);
		}
		#endregion
		#region AdjustForDeletion
		public void AdjustForDeletion(SourceRange deletion)
		{
			Start.AdjustForDeletion(deletion);
			End.AdjustForDeletion(deletion);
		}
		#endregion
		#region SetRange
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("SetRange is Obsolete. Use the Set method instead.")]
		public void SetRange(Token token)
		{
			Set(token);
		}
		#endregion
		#region Set
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Set(Token token)
		{
			Set(token.Line, token.Column, token.Line, token.EndColumn);
		}
		#endregion
		#region Set
		public void Set(int startLine, int startOffset, int endLine, int endOffset)
		{
		Start = new SourcePoint(startLine, startOffset);
			End = new SourcePoint(endLine, endOffset);
		}
		#endregion
		#region Set
		public void Set(SourcePoint startPoint, SourcePoint endPoint)
		{
			Set(startPoint.Line, startPoint.Offset, endPoint.Line, endPoint.Offset);
		}
		#endregion
		#region Set
		public void Set(SourceRange range)
		{
			Set(range.Start.Line, range.Start.Offset, range.End.Line, range.End.Offset);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetFast(int startLine, int startOffset, int endLine, int endOffset)
		{
			Start.SetFast(startLine, startOffset);
			End.SetFast(endLine, endOffset);
		}
		public static void AnchorToSourcePoint(ref object range, SourcePoint point) 
		{
			if (range != null)
			{
				SourceRange lRange = ((SourceRange)range);
				lRange.AnchorToSourcePoint(point);
				range = lRange;
			}
		}
		public static void HoistAnchor(ref object range) 
		{
			if (range != null)
			{
				SourceRange lRange = ((SourceRange)range);
				lRange.RemoveAllBindings();
				range = lRange;
			}
		}
		public static void AdjustForDeletion(ref object range, SourceRange deletion)
		{
			if (range != null)
			{
				SourceRange lRange = ((SourceRange)range);
				lRange.AdjustForDeletion(deletion);
				range = lRange;
			}
		}
		public static void AdjustForInsertion(ref object range, SourceRange insertion)
		{
			if (range != null)
			{
				SourceRange lRange = ((SourceRange)range);
				lRange.AdjustForInsertion(insertion);
				range = lRange;
			}
		}
		public static void Set(ref object range, Token token)
		{
			if (range == null)
				range = SourceRange.Empty;
			SourceRange newRange = (SourceRange)range;
			newRange.Set(token);
			range = newRange;
		}
		public static void Set(ref object range, SourceRange sourceRange)
		{
			if (range == null)
				range = SourceRange.Empty;
			SourceRange newRange = (SourceRange)range;
			newRange.Set(sourceRange);
			range = newRange;
		}
		public static void SetEnd(ref object range, SourcePoint point)
		{
			if (range == null)
				range = SourceRange.Empty;
			SourceRange newRange = (SourceRange)range;
			newRange.End.Set(point);
			range = newRange;
		}
		public static void SetEnd(ref object range, Token token)
		{
			if (range == null)
				range = SourceRange.Empty;
			SourceRange newRange = (SourceRange)range;
			newRange.End.Set(token.Line, token.EndColumn);
			range = newRange;
		}
		public static SourceRange Get(object range)
		{
			return range == null ? SourceRange.Empty : (SourceRange)range;
		}
	#region Equals(object aObj)
	public override bool Equals(object aObj)
	{
	  if (aObj == null)
		return false;
	  if (aObj is SourceRange)
		return Equals((SourceRange)aObj);
	  return false;
	}
	#endregion
	#region Equals(SourceRange range)
	public bool Equals(SourceRange range)
	{
	  if (!Start.Equals(range.Start))
		return false;
	  if (!End.Equals(range.End))
		return false;
	  return true;
	}
	#endregion
	#region Equals(SourcePoint start, SourcePoint end)
	public bool Equals(SourcePoint start, SourcePoint end)
	{
	  return (Start.Equals(start) && End.Equals(end));
	}
	#endregion
	#region Equals(int startLine, int startOffset, int endLine, int endOffset)
	public bool Equals(int startLine, int startOffset, int endLine, int endOffset)
	{
	  return (Start.Equals(startLine, startOffset) && End.Equals(endLine, endOffset));
	}
	#endregion
		#region GetHashCode
	public override int GetHashCode()
	{
	  string s = ToString();
	  return s.GetHashCode();
	}
	#endregion
	#region ToString
	public override string ToString()
	{
	  return String.Format("{0} -> {1}", Start, End);
	}
	#endregion
	#region Contains(int line, int offset)
	public bool Contains(int line, int offset)
	{
	  return Contains(new SourcePoint(line, offset));
	}
	#endregion
	#region Contains(SourcePoint sourcePoint)
		public bool Contains(SourcePoint sourcePoint)
	{
			SourcePoint top;
			SourcePoint bottom;
			GetTopAndBottom(out top, out bottom);
	  return (sourcePoint >= top) && (sourcePoint <= bottom);
	}
	#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool FastContains(SourceRange range)
	{
	  SourcePoint rangeStart = range.Start;
	  SourcePoint rangeEnd = range.End;
	  return (Start <= rangeStart) && (End >= rangeStart) && (Start <= rangeEnd) && (End >= rangeEnd);
	}
		#region Contains(SourceRange range)
		public bool Contains(SourceRange range)
	{
	  SourcePoint top;
	  SourcePoint bottom;
	  GetTopAndBottom(out top, out bottom);
	  SourcePoint start = range.Start;
	  SourcePoint end = range.End;
	  return (start >= top) && (start <= bottom) && (end >= top) && (end <= bottom);
	}
	#endregion
	#region IntersectsWith(SourceRange range)
	public bool IntersectsWith(SourceRange range)
	{
	  return Contains(range.Start) ||
		Contains(range.End) ||
		range.Contains(this);
	}
	#endregion
		#region Intersects(SourceRange range)
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool Intersects(SourceRange range)
	{
	  return (Contains(range.Start)) || (Contains(range.End));
	}
	#endregion
		#region Surrounds
		public bool Surrounds(SourcePoint sourcePoint)
		{
			return (Top < sourcePoint) && (Bottom > sourcePoint);
		}
		#endregion
		#region EndsBefore
		public bool EndsBefore(SourceRange sourceRange)
		{
			return (Bottom < sourceRange.Top);
		}
		#endregion
		public bool EndsBefore(int lineNumber, int columnOffset)
		{
			return (End.Line < lineNumber) || (End.Line == lineNumber && End.Offset < columnOffset);
		}
		public bool StartsAfter(int lineNumber, int columnOffset)
		{
			return (Start.Line > lineNumber) || (Start.Line == lineNumber && Start.Offset > columnOffset);
		}
		#region StartsAfter
		public bool StartsAfter(SourceRange sourceRange)
		{
			return (Top > sourceRange.Bottom);
		}
		#endregion
		public bool StartsBefore(int lineNumber, int columnOffset)
		{
			return (Start.Line < lineNumber) || (Start.Line == lineNumber && Start.Offset < columnOffset);
		}
		public bool EndsAfter(int lineNumber, int columnOffset)
		{
			return (End.Line > lineNumber) || (End.Line == lineNumber && End.Offset > columnOffset);
		}
	#region Overlaps
	public bool Overlaps(SourceRange range)
	{
	  if (Holds(range.Top) || Holds(range.Bottom) || range.Holds(Top))
		return true;
	  return false;
	}
	#endregion
	#region Holds
	public bool Holds(SourcePoint testPoint)
	{
	  return testPoint > Top && testPoint < Bottom;
	}
	#endregion
		#region BindToCode
		public void BindToCode(IDisposableEditPointFactory editPointFactory)
		{
			Start.BindToCode(editPointFactory, true);
			End.BindToCode(editPointFactory, true);
		}
		#endregion
		#region RemoveBinding
		public void RemoveBinding()
		{
			Start.RemoveBinding();
			End.RemoveBinding();
		}
		#endregion
		#region RemoveAllBindings
		public void RemoveAllBindings()
		{
			Start.RemoveAllBindings();
			End.RemoveAllBindings();
		}
		#endregion
		#region AnchorToSourcePoint
		public void AnchorToSourcePoint(SourcePoint sourcePoint)
		{
			Start.AnchorToSourcePoint(sourcePoint, true);
			End.AnchorToSourcePoint(sourcePoint);
		}
		#endregion
		#region HoistAnchor
		public void HoistAnchor()
		{
			Start.HoistAnchor();
			End.HoistAnchor();
		}
		#endregion
		#region OffsetRange
		public SourceRange OffsetRange(int lines, int columns)
		{
			return new SourceRange(Start.OffsetPoint(lines, columns), End.OffsetPoint(lines, columns));
		}
		#endregion
		#region ExtractFromDocument
		public SourceRange ExtractFromDocument(SourcePoint newOrigin)
		{			
			return new SourceRange(Start.ExtractFromDocument(newOrigin), End.ExtractFromDocument(newOrigin));
		}
		#endregion
		#region RestoreToDocument
		public SourceRange RestoreToDocument(SourcePoint customOrigin)
		{
			return new SourceRange(Start.RestoreToDocument(customOrigin), End.RestoreToDocument(customOrigin));			
		}
		#endregion
		#region IsBoundToCode
		public bool IsBoundToCode
		{
			get
			{
				return (Start.IsBoundToCode || End.IsBoundToCode);
			}
		}
		#endregion
		#region IsAnchored
		public bool IsAnchored
		{
			get
			{
				return (Start.IsAnchored || End.IsAnchored);
			}
		}
		#endregion
		static SourcePoint GetMax(SourcePoint a, SourcePoint b) 
		{
			return a <= b ? b : a;
		}
		static SourcePoint GetMin(SourcePoint a, SourcePoint b) 
		{
			return a >= b ? b : a;
		}
		#region Union(SourceRange a, SourceRange b)
		public static SourceRange Union(SourceRange a, SourceRange b)
		{
			SourceRange lResult = SourceRange.Empty;
			if (a == SourceRange.Empty)
				return b;
			else if (b == SourceRange.Empty)
				return a;
			else 
			{
				lResult.Start = GetMin(a.Top, b.Top);
				lResult.End = GetMax(a.Bottom, b.Bottom);
			}
			return lResult;
		}
		#endregion
	#region Bottom
	public SourcePoint Bottom
	{
		get
		{
		if (StartPrecedesEnd)
			  return End;
		else
		  return Start;
		}
	}
	#endregion
		#region CharsIncludedOnLastLine
		public int CharsIncludedOnLastLine
		{
			get
			{
				if (Bottom.Line == Top.Line)
					return Bottom.Offset - Top.Offset;
				else
					return Bottom.Offset;
			}
		}
		#endregion
	#region Height
	public int Height
	{
		get
		{
			return (Bottom.Line - Top.Line) + 1;
		}
	}
	#endregion
		#region IsEmpty
	public bool IsEmpty
	{
	  get
	  {
		return (Start.IsEmpty && End.IsEmpty);
	  }
	}
	#endregion
		#region IsPoint
		public bool IsPoint
		{
			get
			{
				return (Start == End);
			}
		}
		#endregion
		#region LineCount
		public LineRange LineCount
		{
			get
			{
				return new LineRange(Bottom.Line - Top.Line + 1);
			}
		}
		#endregion
		#region LineCountMinusOne
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LineRange LineCountMinusOne
		{
			get
			{
				return new LineRange(Bottom.Line - Top.Line);
			}
		}
		#endregion
	#region LogicalRange
	public SourceRange LogicalRange
	{
	  get
	  {
		if (StartPrecedesEnd)
		  return new SourceRange(Start, End);
		else
		  return new SourceRange(End, Start);
	  }
	}
	#endregion
	#region StartPrecedesEnd
	public bool StartPrecedesEnd
	{
		get
		{
			return (Start.CompareTo(End) <= 0);
		}
	}
	#endregion
	#region Top
	public SourcePoint Top
	{
	  get
	  {
		if (StartPrecedesEnd)
		  return Start;
		else
		  return End;
	  }
	}
	#endregion
	#region operator ==
	public static bool operator==(SourceRange range1, SourceRange range2)
	{
	  return range1.Equals(range2);
	}
	#endregion
	#region operator !=
	public static bool operator!=(SourceRange range1, SourceRange range2)
	{
	  return !(range1 == range2);
	}
	#endregion
		#region operator -
		public static SourceRange operator-(SourceRange sourceRange, LineRange lineRange)
		{
			return new SourceRange(sourceRange.Start.Line - lineRange, sourceRange.Start.Offset,
				sourceRange.End.Line - lineRange, sourceRange.End.Offset);
		}
		#endregion
		#region operator +
		public static SourceRange operator+(SourceRange sourceRange, LineRange lineRange)
		{
			return new SourceRange(sourceRange.Start.Line + lineRange, sourceRange.Start.Offset,
				sourceRange.End.Line + lineRange, sourceRange.End.Offset);
		}
		#endregion
		#region ICloneable
		object ICloneable.Clone()
		{
			return Clone();
		}
		public SourceRange Clone()
		{
			return new SourceRange(this);
		}
		#endregion
	}
}
