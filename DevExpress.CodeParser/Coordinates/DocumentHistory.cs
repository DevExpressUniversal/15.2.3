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
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	class TextPointWrapper
	{
		public TextPoint Point;
	}
	public class DocumentHistorySlice
	{
		ArrayList _RangeTransforms;
		TextPointWrapper pointWrapper = new TextPointWrapper();
		public DocumentHistorySlice()
		{
			_RangeTransforms = new ArrayList();
			TextRange lRange = new TextRange(1, 1, int.MaxValue / 2, short.MaxValue / 2);
			RangeTransform lTransform = new RangeTransform(lRange, 0, 0);
			AddTransform(lTransform);
		}
		int Find(RangeTransform transform)
		{
			return _RangeTransforms.BinarySearch(transform, RangeTransform.RangeComparer);
		}
		int FindTransformed(TextPoint point)
		{
			pointWrapper.Point = point;
			return _RangeTransforms.BinarySearch(pointWrapper, RangeTransform.TransformedPointComparer);
		}
		int FindOriginal(TextPoint point)
		{
			pointWrapper.Point = point;
			return _RangeTransforms.BinarySearch(pointWrapper, RangeTransform.OriginalPointComparer);
		}
		void AddTransform(RangeTransform transform)
		{
			int lIndex = Find(transform);
			_RangeTransforms.Insert(~lIndex, transform);
		}
		void RemoveTransform(int i)
		{
			_RangeTransforms.RemoveAt(i);
		}
		void GetLineOffset(ref int line, ref int offset)
		{
			if(line == 0 && offset == 0)
				return;
			int lIndex = FindOriginal(new TextPoint(line, offset));
			if (lIndex < 0) 
			{
				line = -1;
				offset = -1;
				return;
			}
			RangeTransform lTransform = (RangeTransform)_RangeTransforms[lIndex];
			lTransform.Apply(ref line, ref offset);
		}
		RangeTransform FindContainingTransform(TextPoint point)
		{
			int lIndex = FindTransformed(point);
			if (lIndex < 0)
				return null;
			RangeTransform lTransform = (RangeTransform)_RangeTransforms[lIndex];
			if(lTransform.IsStart(point))
				return null;
			return lTransform;
		}
		void RemoveTransforms(TextRange range)
		{
			int lStart = FindTransformed(range.Start);
			if(lStart < 0)
				lStart = ~lStart;
			else
				lStart++;
			int lEnd = FindTransformed(range.End);
			if(lEnd < 0)
				lEnd = ~lEnd;
			int lCount = lEnd - lStart;
			if(lCount > 0)
				_RangeTransforms.RemoveRange(lStart, lCount);
		}
		void ShiftTrailingTransforms(TextRange range, bool shiftUp)
		{
			int lCount = _RangeTransforms.Count;
			int lIndex = FindTransformed(range.Start);
			if(lIndex < 0)
				lIndex = ~lIndex;
			int lLine = shiftUp ? range.Start.Line : range.End.Line;
			int lLineCount = shiftUp ? -range.LineCountMinusOne : range.LineCountMinusOne;
			int lOffsetTransform = shiftUp ? range.Start.Offset - range.End.Offset :
				range.End.Offset - range.Start.Offset;
			for (int i = lIndex; i < lCount; i++)
			{
				RangeTransform lTransform = (RangeTransform)_RangeTransforms[i];
				lTransform.Shift(lLine, lLineCount, lOffsetTransform);
			}
		}
		void TransformDeletedPoint(ref int line, ref int offset)
		{
			TextPoint lPoint = new TextPoint(line, offset);
			int lIndex = FindOriginal(lPoint);
			if (lIndex < 0)
			{
				lIndex = ~lIndex;
				lIndex--;
			}
			if (lIndex < 0)
			{
				line = 0;
				offset = 0;
				return;
			}
			RangeTransform lTransform = (RangeTransform)_RangeTransforms[lIndex];
			line = lTransform.EndLine;
			offset = lTransform.EndOffset;
			lTransform.Apply(ref line, ref offset);
		}
		bool WasDeleted(TextPoint point)
		{
			return point.Line == -1 && point.Offset == -1;
		}
		TextRange TransformRangeWithRecover(TextPoint start, TextPoint end)
		{
			TextPoint transformedStart = TransformStart(start);
			if (start == end)
				return new TextRange(transformedStart, transformedStart);
			TextPoint lNewStart = TransformStart(start);
			TextPoint lNewEnd = TransformEnd(end);
			if (WasDeleted(lNewStart) && WasDeleted(lNewEnd))
				return TextRange.Empty;
			int lLine;
			int lOffset;
			if (WasDeleted(lNewStart))
			{
				lLine = start.Line;
				lOffset = start.Offset;
				TransformDeletedPoint(ref lLine, ref lOffset);
				lNewStart = new TextPoint(lLine, lOffset ++);
			}
			if (WasDeleted(lNewEnd))
			{
				lLine = end.Line;
				lOffset = end.Offset - 1;
				TransformDeletedPoint(ref lLine, ref lOffset);
				lNewEnd = new TextPoint(lLine, lOffset);
			}
			if (WasDeleted(lNewStart))
				lNewStart = TextPoint.Empty;
			if (WasDeleted(lNewEnd))
				lNewEnd = TextPoint.Empty;
			return new TextRange(lNewStart, lNewEnd);
		}
		public void Insert(TextRange range)
		{
			if (range.IsEmpty)
				return;
			if (range.IsPoint)
				return;
	  lock (this)
	  {
		RangeTransform lTransform = FindContainingTransform(range.Start);
		if (lTransform != null)
		  AddTransform(lTransform.Insert(range));
		ShiftTrailingTransforms(range, false);
	  }
		}
		public void Delete(TextRange range)
		{
	  lock (this)
	  {
		if (range.IsEmpty)
		  return;
		if (range.IsPoint)
		  return;
		RangeTransform lStartTransform = FindContainingTransform(range.Start);
		RangeTransform lEndTransform = FindContainingTransform(range.End);
		if (lStartTransform != null && lStartTransform == lEndTransform)
		  AddTransform(lStartTransform.Delete(range));
		else
		{
		  if (lStartTransform != null)
			lStartTransform.TrimEnd(range.Start);
		  if (lEndTransform != null)
			lEndTransform.TrimStart(range.End);
		  RemoveTransforms(range);
		}
		ShiftTrailingTransforms(range, true);
	  }
		}
		public TextPoint TransformStart(TextPoint point)
		{
	  lock (this)
	  {
		int lLine = point.Line;
		int lOffset = point.Offset;
		GetPoint(ref lLine, ref lOffset);
		return new TextPoint(lLine, lOffset);
	  }
		}
		public TextPoint TransformEnd(TextPoint point)
		{
	  lock (this)
	  {
		int lLine = point.Line;
		int lOffset = point.Offset;
		bool decreased = false;
		if (lOffset != 1)
		{
		  lOffset--;
		  decreased = true;
		}
		GetPoint(ref lLine, ref lOffset);
		if (lOffset != -1 && decreased)
		  lOffset++;
		return new TextPoint(lLine, lOffset);
	  }
		}
		public TextRange Transform(TextRange range)
		{
	  lock (this)
	  {
				TextPoint transformedStart = TransformStart(range.Start);
				if (range.IsPoint)
					return new TextRange(transformedStart, transformedStart);
				return new TextRange(transformedStart, TransformEnd(range.End));
	  }
		}
		public TextRange TransformWithRecover(TextRange range)
		{
	  lock (this)
	  {
		return TransformRangeWithRecover(range.Start, range.End);
	  }
		}		
		public void GetPoint(ref int line, ref int offset)
		{
	  lock (this)
	  {
		GetLineOffset(ref line, ref offset);
	  }
		}
		public int GetLine(int line, int offset)
		{
	  lock (this)
	  {
		GetLineOffset(ref line, ref offset);
		return line;
	  }
		}
		public int GetOffset(int line, int offset)
		{
	  lock (this)
	  {
		GetLineOffset(ref line, ref offset);
		return offset;
	  }
		}
		bool IsDeletion(RangeTransform start, RangeTransform end)
		{
	  lock (this)
	  {
		int startLine = start != null ? start.EndLine : 1;
		int startOffset = start != null ? start.EndOffset : 1;
		return startLine != end.StartLine || startOffset != end.StartOffset;
	  }
		}
	bool IsInsertion(RangeTransform start, RangeTransform end)
	{
	  lock (this)
	  {
		return !IsDeletion(start, end);
	  }
	}
	static TextRange GetEditOperationRange(RangeTransform prev, RangeTransform next)
	{
	  int startLine = 1;
	  int startOffset = 1;
	  if (prev == null && next.StartLine == 1 && next.StartOffset == 1 && !next.HasTransformValue)
		return TextRange.Empty;
	  if (prev != null)
	  {
		startLine = prev.EndLine;
		startOffset = prev.EndOffset;
		prev.Apply(ref startLine, ref startOffset);
	  }
	  int endLine = next.StartLine;
	  int endOffset = next.StartOffset;
	  next.Apply(ref endLine, ref endOffset);
	  return new TextRange(startLine, startOffset, endLine, endOffset);
	}
	TextRange GetInsertionRange(RangeTransform prev, RangeTransform next)
	{
	  TextRange range = GetEditOperationRange(prev, next);
	  if (range.IsEmpty)
		return range;
	  if (IsInsertion(prev, next) || !range.IsPoint)
		return range;
	  return TextRange.Empty;
	}
	TextRange GetDeletionRange(RangeTransform prev, RangeTransform next)
	{
	  if (!IsDeletion(prev, next))
		return TextRange.Empty;
	  TextRange range = GetEditOperationRange(prev, next);
	  if (range.IsEmpty)
		return TextRange.Empty;
	  if (!range.IsPoint)
		range.SetEnd(range.Start);
	  return range;
	}
		public TextRange[] GetDeletions()
		{
	  lock (this)
	  {
		List<TextRange> result = new List<TextRange>();
		RangeTransform prev = (RangeTransform)_RangeTransforms[0];
		TextRange range = GetDeletionRange(null, prev);
		if (!range.IsEmpty)
		  result.Add(range);
		for (int i = 1; i < _RangeTransforms.Count; i++)
		{
		  RangeTransform next = (RangeTransform)_RangeTransforms[i];
		  range = GetDeletionRange(prev, next);
		  if (!range.IsEmpty)
			result.Add(range);
		  prev = next;
		}
		return result.ToArray();
	  }
		}   
	public TextRange[] GetInsertions()
	{
	  lock (this)
	  {
		List<TextRange> result = new List<TextRange>();
		RangeTransform prev = (RangeTransform)_RangeTransforms[0];
		TextRange range = GetInsertionRange(null, prev);
		if (!range.IsEmpty)
		  result.Add(range);
		for (int i = 1; i < _RangeTransforms.Count; i++)
		{
		  RangeTransform next = (RangeTransform)_RangeTransforms[i];
		  range = GetInsertionRange(prev, next);
		  if (!range.IsEmpty)
			result.Add(range);
		  prev = next;
		}
		return result.ToArray();
	  }
	}
	}
	public class DocumentHistory
	{
		ArrayList _Slices;
		public DocumentHistory()
		{
			_Slices = new ArrayList();
			CreateCurrent();
		}
		int Count
		{
			get
			{
				return _Slices.Count;
			}
		}
		DocumentHistorySlice this[int i]
		{
			get
			{
				return (DocumentHistorySlice)_Slices[i];
			}
		}
		void CreateCurrent()
		{
			_Slices.Add(new DocumentHistorySlice());
		}
	protected virtual void OnBeforeClear()
	{
	  if (BeforeClear != null)
		BeforeClear(this, EventArgs.Empty);
	}
	protected virtual void OnAfterClear()
	{
	  if (AfterClear != null)
		AfterClear(this, EventArgs.Empty);
	}
		public void Insert(TextRange range)
		{
	  lock (this)
	  {
		int lCount = Count;
		for (int i = 0; i < lCount; i++)
		  this[i].Insert(range);
	  }
		}
		public void Delete(TextRange range)
		{
	  lock (this)
	  {
		int lCount = Count;
		for (int i = 0; i < lCount; i++)
		  this[i].Delete(range);
	  }
		}
		public DocumentHistorySlice Branch()
		{
	  lock (this)
	  {
		DocumentHistorySlice lNew = new DocumentHistorySlice();
		_Slices.Add(lNew);
		return lNew;
	  }
		}
		public void Clear()
		{
	  lock (this)
	  {
		OnBeforeClear();
		_Slices.Clear();
		CreateCurrent();
		OnAfterClear();
	  }
		}
		class SourceRangeContainer
		{
			public int Index;
			public SourceRange Range;
			public SourceRange NewRange;
			public SourceRangeContainer(int index, SourceRange range, SourceRange newRange)
			{
				Index = index;
				Range = range;
				NewRange = newRange;
			}			
		}
		class SourceRangeContainerComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				SourceRangeContainer first = x as SourceRangeContainer;
				SourceRangeContainer second = y as SourceRangeContainer;
				if (first == null && second == null)
					return 0;
				if (first == null)
					return -1;
				if (second == null)
					return 1;
				if (first.Range.Start == second.Range.Start)
					if (first.Range.End == second.Range.End)
					{
						int firstIndex = first.Index;
						int secondIndex = second.Index;
						if (firstIndex == secondIndex)
							return 0;
						if (firstIndex < secondIndex)
							return -1;
						return 1;
					}
					else
						return first.Range.End < second.Range.End ? -1 : 1;
				if (first.Range.Start < second.Range.Start)
					return -1;
				return 1;
			}
		}
		public SourceRange[] UpdateRanges(SourceRange[] ranges, int newLength)
		{
			if (ranges == null || ranges.Length == 0)
				return ranges;
			int[] lengths = new int[ranges.Length];
			for (int i = 0; i < lengths.Length; i++)
				lengths[i] = newLength;
			return UpdateRanges(ranges, newLength);
		}
	public SourceRange[] UpdateRanges(SourceRange[] ranges, int[] newLengths)
	{
	  if (ranges == null || ranges.Length == 0)
		return ranges;
	  SourceRange[] newRanges = new SourceRange[ranges.Length];
	  for (int i = 0; i < newLengths.Length; i++)	 
		newRanges[i] = new SourceRange(ranges[i].Start, ranges[i].Start.OffsetPoint(0, newLengths[i]));
	  return UpdateRanges(ranges, newRanges);
	}
		public SourceRange[] UpdateRanges(SourceRange[] ranges, SourceRange[] newRanges)
		{
			if (ranges == null || ranges.Length == 0)
				return new SourceRange[0];
			if (newRanges == null || newRanges.Length == 0)
				return ranges;
			if (ranges.Length != newRanges.Length)
				return ranges;
			SourceRange[] result = new SourceRange[ranges.Length];
			ranges.CopyTo(result, 0);
			SourceRangeContainer[] rangeContainers = new SourceRangeContainer[ranges.Length];
			for (int i = 0; i < rangeContainers.Length; i++)
				rangeContainers[i] = new SourceRangeContainer(i, ranges[i], newRanges[i]);
			Array.Sort(rangeContainers, new SourceRangeContainerComparer());
			for (int j = 0; j < rangeContainers.Length; j++)
			{
				SourceRangeContainer rangeContainer = rangeContainers[j];
				SourceRange range = rangeContainer.Range;
		SourceRange newRange = rangeContainer.NewRange;
				DocumentHistorySlice slice = Current;
				range = slice.Transform(range);
				Delete(range);
		newRange = newRange.ExtractFromDocument(newRange.Start);
		newRange = newRange.RestoreToDocument(range.Start);		
		Insert(newRange);
				result[rangeContainer.Index] = newRange;
			}
			return result;
		}		
		public DocumentHistorySlice Current
		{
			get
			{
		lock (this)
		{
		  return this[Count - 1];
		}
			}
		}
	public event EventHandler BeforeClear;
	public event EventHandler AfterClear;
	}
}
