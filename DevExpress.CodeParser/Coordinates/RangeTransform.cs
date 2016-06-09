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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class RangeTransform : IComparable
	{
		static IComparer _RangeComparer;
		static IComparer _OriginalPointComparer;
		static IComparer _TransformedPointComparer;
		int _StartLine;
		int _EndLine;
		int _StartOffset;
		int _EndOffset;
		int _LineTransform;
		int _OffsetTransform;
		class RangeTransformComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
					return -1;
				return ((RangeTransform)x).CompareTo((RangeTransform)y);
			}
		}
		class TransformedTextPointComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
					return -1;
				return ((RangeTransform)x).CompareToTransformed(((TextPointWrapper)y).Point);
			}
		}
		class OriginalTextPointComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
					return -1;
				return ((RangeTransform)x).CompareToOriginal(((TextPointWrapper)y).Point);
			}
		}
		public RangeTransform(RangeTransform source)
		{
			_StartLine = source._StartLine;
			_EndLine = source._EndLine;
			_StartOffset = source._StartOffset;
			_EndOffset = source._EndOffset;
			_LineTransform = source._LineTransform;
			_OffsetTransform = source._OffsetTransform;
		}
		public RangeTransform(TextRange range, int lineTransform, int offsetTransform)
		{
			_StartLine = range.Start.Line;
			_EndLine = range.End.Line;
			_StartOffset = range.Start.Offset;
			_EndOffset = range.End.Offset;
			_LineTransform = lineTransform;
			_OffsetTransform = offsetTransform;
		}
		void Move(int line, int offset)
		{
			int lStartLine = _StartLine;
			int lStartOffset = _StartOffset;
			Apply(ref lStartLine, ref lStartOffset);
			if (lStartLine == line)
				_OffsetTransform += offset;
		}
		int IComparable.CompareTo(object obj)
		{
			return CompareTo((RangeTransform)obj);
		}
		public int CompareTo(RangeTransform transform)
		{
			if (transform == null)
				return 1;
			if (_StartLine < transform._StartLine)
				return -1;
			if (_StartLine == transform._StartLine)
			{
				if (_StartOffset == transform._StartOffset)
					return 0;
				if (_StartOffset < transform._StartOffset)
					return -1;
			}
			return 1;
		}
		public int CompareToTransformed(TextPoint point)
		{
			if (ContainsTransformed(point.Line, point.Offset))
				return 0;
			int lLine = point.Line;
			int lOffset = point.Offset;
			Restore(ref lLine, ref lOffset);
			if (_StartLine < lLine)
				return -1;
			if (_StartLine == lLine)
			{
				if (_StartOffset < lOffset)
					return -1;
			}
			return 1;
		}
		public int CompareToOriginal(TextPoint point)
		{
			if (ContainsOriginal(point.Line, point.Offset))
				return 0;
			int lLine = point.Line;
			int lOffset = point.Offset;
			if (_StartLine < lLine)
				return -1;
			if (_StartLine == lLine)
			{
				if (_StartOffset < lOffset)
					return -1;
			}
			return 1;
		}
		public bool IsStart(TextPoint point) 
		{
			int lStartLine = _StartLine;
			int lStartOffset = _StartOffset;
			Apply(ref lStartLine, ref lStartOffset);
			if (lStartLine == point.Line)
				return lStartOffset == point.Offset;
			return false;
		}
		public bool IsContainedInTransformed(TextRange range)
		{
			int lStartLine = _StartLine;
			int lEndLine = _EndLine;
			int lStartOffset = _StartOffset;
			int lEndOffset = _EndOffset;
			Apply(ref lStartLine, ref lStartOffset);
			Apply(ref lEndLine, ref lEndOffset);
			return range.Contains(lStartLine, lStartOffset) &&
				range.Contains(lEndLine, lEndOffset);
		}
		public bool ContainsOriginal(int line, int offset)
		{
			if (line < _StartLine || line > _EndLine)
				return false;
			if (_StartLine == _EndLine)
				return offset >= _StartOffset && offset < _EndOffset;
			if (line == _StartLine)
				return offset >= _StartOffset;
			if (line == _EndLine)
				return offset < _EndOffset;
			return true;
		}
		public bool ContainsTransformed(int line, int offset)
		{
			Restore(ref line, ref offset);
			return ContainsOriginal(line, offset);
		}
		public bool StartsAfter(TextPoint point)
		{
			int lStartLine = _StartLine;
			int lStartOffset = _StartOffset;
			Apply(ref lStartLine, ref lStartOffset);
			if (lStartLine == point.Line)
				return lStartOffset >= point.Offset;
			return lStartLine > point.Line;
		}
		public void Shift(int line, int lineTransform, int offsetTransform)
		{
			_LineTransform += lineTransform;
			Move(line, offsetTransform);
		}
		public void Apply(ref int line, ref int offset)
		{
			if (line == _StartLine)
				offset += _OffsetTransform;
			line += _LineTransform;
		}
		public void Restore(ref int line, ref int offset)
		{
			line -= _LineTransform;
			if (line == _StartLine)
				offset -= _OffsetTransform;			
		}
		public void TrimStart(TextPoint point)
		{
			int lNewLine = point.Line;
			_StartOffset = point.Offset;
			Restore(ref lNewLine, ref _StartOffset);
			if (lNewLine != _StartLine) 
			{
				_OffsetTransform = 0;
				_StartLine = lNewLine;
			}
		}
		public void TrimEnd(TextPoint point)
		{
			_EndLine = point.Line;
			_EndOffset = point.Offset;
			Restore(ref _EndLine, ref _EndOffset);
		}
		public RangeTransform Insert(TextRange range)
		{
			RangeTransform lBottom = new RangeTransform(this);
			TrimEnd(range.Start);
			lBottom.TrimStart(range.Start);
			return lBottom;
		}
		public RangeTransform Delete(TextRange range)
		{
			RangeTransform lBottom = new RangeTransform(this);
			TrimEnd(range.Start);
			lBottom.TrimStart(range.End);
			return lBottom;
		}		
		public static IComparer RangeComparer
		{
			get
			{
				if (_RangeComparer == null)
					_RangeComparer = new RangeTransformComparer();
				return _RangeComparer;
			}
		}
		public static IComparer OriginalPointComparer
		{
			get
			{
				if (_OriginalPointComparer == null)
					_OriginalPointComparer = new OriginalTextPointComparer();
				return _OriginalPointComparer;
			}
		}
		public static IComparer TransformedPointComparer
		{
			get
			{
				if (_TransformedPointComparer == null)
					_TransformedPointComparer = new TransformedTextPointComparer();
				return _TransformedPointComparer;
			}
		}
		public int StartLine
		{
			get
			{
				return _StartLine;
			}
			set
			{
				_StartLine = value;
			}
		}
		public int EndLine
		{
			get
			{
				return _EndLine;
			}
			set
			{
				_EndLine = value;
			}
		}
		public int StartOffset
		{
			get
			{
				return _StartOffset;
			}
			set
			{
				_StartOffset = value;
			}
		}
		public int EndOffset
		{
			get
			{
				return _EndOffset;
			}
			set
			{
				_EndOffset = value;
			}
		}
	public bool HasTransformValue
	{
	  get
	  {
		return _LineTransform != 0 || _OffsetTransform != 0;
	  }
	}
	}
}
