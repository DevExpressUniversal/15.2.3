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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region ICharacterIterator
	public interface ICharacterIterator {
		char GetNextChar();
		bool HasNextChar();
	}
	#endregion
	#region BufferedRegexSearchResult
	public class BufferedRegexSearchResult {
		readonly Match match;
		readonly DocumentModelPosition offset;
		int absolutePosition = -1;
		internal BufferedRegexSearchResult(Match match, DocumentModelPosition offset) {
			this.match = match;
			this.offset = offset;
		}
		public Match Match { get { return match; } }
		public DocumentModelPosition Offset { get { return offset; } }
		public int AbsolutePosition { get { return absolutePosition; } set { absolutePosition = value; } }
	}
	#endregion
	#region BufferedRegexSearchResultCollection
	public class BufferedRegexSearchResultCollection : List<BufferedRegexSearchResult> {
	}
	#endregion
	#region BufferedRegexSearch (old)
	#endregion
	#region Old
	#endregion
	public class BufferedRegexSearchResultTable : Dictionary<int, BufferedRegexSearchResult> {
	}
	#region RegexSearchInfo
	[Flags]
	public enum RegexSearchInfo {
		None,
		StartExtended,
		EndExtended
	}
	#endregion
	#region SearchConditions
	public class SearchConditions {
		int startIndex;
		int length;
		string input;
		public SearchConditions(string input) {
			this.input = input;
		}
		public SearchConditions(string input, int startIndex, int length)
			: this(input) {
			this.startIndex = startIndex;
			this.length = length;
		}
		public string Input { get { return input; } set { input = value; } }
		public int StartIndex { get { return startIndex; } set { startIndex = value; } }
		public int Length { get { return length; } set { length = value; } }
	}
	#endregion
	#region BufferedRegexSearchBase (abstract class)
	public abstract class BufferedRegexSearchBase {
		#region Fields
		readonly int bufferSize;
		readonly int bufferShiftSize;
		readonly StringBuilder buffer;
		int bufferOffset;
		RegexSearchInfo searchInfo;
		readonly Regex regex;
		DocumentModelPosition bufferStartPosition;
		DocumentModelPosition currentInputPosition;
		DocumentLogPosition endPosition;
		DocumentLogPosition startPosition;
		readonly PieceTable pieceTable;
		#endregion
		protected BufferedRegexSearchBase(PieceTable pieceTable, Regex regex)
			: this(pieceTable, regex, 128) {
		}
		protected BufferedRegexSearchBase(PieceTable pieceTable, Regex regex, int maxGuaranteedSearchResultLength)
			: this(pieceTable, regex, 2 * maxGuaranteedSearchResultLength, maxGuaranteedSearchResultLength) {
		}
		internal BufferedRegexSearchBase(PieceTable pieceTable, Regex regex, int bufferSize, int bufferShiftSize) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(regex, "regex");
			Guard.ArgumentPositive(bufferSize, "bufferSize");
			Guard.ArgumentPositive(bufferShiftSize, "bufferShiftSize");
			this.pieceTable = pieceTable;
			this.regex = regex;
			this.bufferSize = bufferSize;
			this.bufferShiftSize = bufferShiftSize;
			this.buffer = new StringBuilder(bufferSize);
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		internal int BufferShiftSize { get { return bufferShiftSize; } }
		internal int BufferSize { get { return bufferSize; } }
		internal StringBuilder Buffer { get { return buffer; } }
		internal DocumentModelPosition BufferStartPosition { get { return bufferStartPosition; } set { bufferStartPosition = value; } }
		internal DocumentModelPosition BufferInputPosition { get { return currentInputPosition; } set { currentInputPosition = value; } }
		internal int BufferOffset { get { return bufferOffset; } set { bufferOffset = value; } }
		protected RegexSearchInfo SearchInfo { get { return searchInfo; } set { searchInfo = value; } }
		protected Regex Regex { get { return regex; } }
		protected DocumentLogPosition EndPosition { get { return endPosition; } }
		protected DocumentLogPosition StartPosition { get { return startPosition; } }
		#endregion
		public BufferedRegexSearchResult Match(DocumentLogPosition start, DocumentLogPosition end) {
			InitializeSearchInterval(start, end);
			DocumentModelPosition position = CalculateStartPosition();
			Initialize(start, end, position);
			BufferedDocumentCharacterIterator iterator = CreateIterator(position);
			for (; ; ) {
				AppendBuffer(iterator);
				if (buffer.Length == 0)
					break;
				SearchConditions conditions = new SearchConditions(Buffer.ToString(), 0, Buffer.Length);
				Match match = CalculateMatch(iterator, Regex, conditions);
				if (match != null && match.Success)
					return CreateResult(match);
				ShiftBuffer(iterator, BufferShiftSize);
			}
			return null;
		}
		void Initialize(DocumentLogPosition start, DocumentLogPosition end, DocumentModelPosition currentPosition) {
			this.bufferStartPosition = currentPosition.Clone();
			this.currentInputPosition = currentPosition.Clone();
			this.buffer.Length = 0;
			this.bufferOffset = 0;
		}
		protected internal virtual void InitializeSearchInterval(DocumentLogPosition start, DocumentLogPosition end) {
			this.searchInfo = RegexSearchInfo.None;
			if (start > PieceTable.DocumentStartLogPosition) {
				DocumentLogPosition prevVisiblePosition = PieceTable.VisibleTextFilter.GetPrevVisibleLogPosition(start, true);
				this.startPosition = Algorithms.Max(PieceTable.DocumentStartLogPosition, prevVisiblePosition);
				this.searchInfo |= RegexSearchInfo.StartExtended;
			}
			else
				this.startPosition = start;
			if (end < PieceTable.DocumentEndLogPosition) {
				DocumentLogPosition nextVisiblePosition = PieceTable.VisibleTextFilter.GetNextVisibleLogPosition(end, true);
				this.endPosition = Algorithms.Min(PieceTable.DocumentEndLogPosition, nextVisiblePosition); 
				this.searchInfo |= RegexSearchInfo.EndExtended;
			}
			else
				this.endPosition = end;
		}
		protected abstract BufferedDocumentCharacterIterator CreateIterator(DocumentModelPosition position);
		protected abstract bool CanMoveNext();
		protected internal BufferedRegexSearchResult CreateResult(Match match) {
			return new BufferedRegexSearchResult(match, BufferStartPosition.Clone());
		}
		public BufferedRegexSearchResultCollection Matches(DocumentLogPosition start, DocumentLogPosition end) {
			InitializeSearchInterval(start, end);
			DocumentModelPosition position = CalculateStartPosition();
			Initialize(start, end, position);
			BufferedDocumentCharacterIterator iterator = CreateIterator(position);
			List<BufferedRegexSearchResult> results = new List<BufferedRegexSearchResult>();
			for (; ; ) {
				AppendBuffer(iterator);
				bool isSuccess = ProcessBuffer(iterator, Regex, results);
				if (buffer.Length < BufferSize)
					return CreateResult(results);
				int shiftSize = 0;
				if (isSuccess) {
					int lastIndex = results.Count - 1;
					shiftSize = CalculateBufferShiftSize(results[lastIndex]);
				}
				shiftSize = Math.Max(shiftSize, BufferShiftSize);
				ShiftBuffer(iterator, shiftSize);
			}
		}
		protected abstract int CalculateBufferShiftSize(BufferedRegexSearchResult result);
		protected abstract DocumentModelPosition CalculateStartPosition();
		protected internal virtual bool ProcessBuffer(BufferedDocumentCharacterIterator iterator, Regex regEx, List<BufferedRegexSearchResult> results) {
			SearchConditions conditions = new SearchConditions(Buffer.ToString(), 0, Buffer.Length);
			bool result = false;
			for (; ; ) {
				Match match = CalculateMatch(iterator, regEx, conditions);
				if (match == null || !match.Success)
					break;
				BufferedRegexSearchResult searchResult = CreateResult(match);
				int absolutePosition = GetMatchAbsolutePosition(match);
				BufferedRegexSearchResult lastResult = results.Count > 0 ? results[results.Count - 1] : null;
				if (lastResult == null || absolutePosition > lastResult.AbsolutePosition) {
					searchResult.AbsolutePosition = absolutePosition;
					results.Add(searchResult);
					result = true;
				}
				CalculateSearchConditions(match, conditions);
			}
			return result;
		}
		protected internal BufferedRegexSearchResultCollection CreateResult(List<BufferedRegexSearchResult> results) {
			BufferedRegexSearchResultCollection result = new BufferedRegexSearchResultCollection();
			int count = results.Count;
			for (int i = 0; i < count; i++)
				result.Add(results[i]);
			return result;
		}
		protected internal virtual void ShiftBuffer(BufferedDocumentCharacterIterator iterator, int delta) {
			int shiftSize = Math.Min(delta, Buffer.Length);
			bufferOffset += shiftSize;
			Buffer.Length -= shiftSize;
		}
		protected internal virtual Match CalculateMatchCore(Regex regEx, SearchConditions conditions) {
			return regEx.Match(conditions.Input, conditions.StartIndex, conditions.Length);
		}
		protected bool IsMatchInTheEndOfBuffer(Match result) {
			return result.Index + result.Length == Buffer.Length;
		}
		protected bool IsMatchInTheStartOfBuffer(Match match) {
			return match.Index == 0;
		}
		protected virtual bool ShouldRecalcMatchAtStartOfBuffer(SearchConditions conditions) {
			if ((SearchInfo & RegexSearchInfo.StartExtended) != 0) {
				Debug.Assert(conditions.StartIndex == 0);
				conditions.StartIndex += 1;
				conditions.Length -= 1;
				return true;
			}
			else
				return false;
		}
		protected virtual bool ShouldRecalcMatchAtEndOfBuffer(SearchConditions conditions) {
			if ((SearchInfo & RegexSearchInfo.EndExtended) != 0) {
				conditions.Length -= 1;
				return true;
			}
			else
				return false;
		}
		protected abstract void CalculateSearchConditions(Match prevMatch, SearchConditions conditions);
		protected internal abstract void AppendBuffer(BufferedDocumentCharacterIterator iterator);
		protected internal abstract Match CalculateMatch(BufferedDocumentCharacterIterator iterator, Regex regEx, SearchConditions conditions);
		protected internal abstract int GetMatchAbsolutePosition(Match match);
	}
	#endregion
	#region BufferedRegexSearchForward
	public class BufferedRegexSearchForward : BufferedRegexSearchBase {
		public BufferedRegexSearchForward(PieceTable pieceTable, Regex regex)
			: base(pieceTable, regex) {
		}
		public BufferedRegexSearchForward(PieceTable pieceTable, Regex regex, int maxGuaranteedSearchResultLength)
			: base(pieceTable, regex, maxGuaranteedSearchResultLength) {
		}
		internal BufferedRegexSearchForward(PieceTable pieceTable, Regex regex, int bufferSize, int bufferShiftSize)
			: base(pieceTable, regex, bufferSize, bufferShiftSize) {
		}
		protected internal override Match CalculateMatchCore(Regex regEx, SearchConditions conditions) {
			string input = conditions.Input.Substring(0, conditions.StartIndex + conditions.Length);
			return regEx.Match(input, conditions.StartIndex);
		}
		protected override void CalculateSearchConditions(Match prevMatch, SearchConditions conditions) {
			int newIndex = prevMatch.Index + (prevMatch.Length > 0 ? prevMatch.Length : 1);
			conditions.Length -= (newIndex - conditions.StartIndex);
			conditions.StartIndex = newIndex;
		}
		protected internal override void AppendBuffer(BufferedDocumentCharacterIterator iterator) {
			int delta = BufferSize - Buffer.Length;
			iterator.AppendBuffer(delta);
			int count = Math.Min(delta, iterator.BufferLength);
			int charIndex = 0;
			for (; charIndex < count; charIndex++) {
				if (!CanMoveNext())
					break;
				Buffer.Append(iterator[charIndex]);
				BufferInputPosition = iterator.GetPosition(BufferInputPosition, 1);
			}
			iterator.ShiftBuffer(charIndex);
		}
		protected internal override void ShiftBuffer(BufferedDocumentCharacterIterator iterator, int delta) {
			int count = Buffer.Length;
			for (int sourceIndex = delta, targetIndex = 0; sourceIndex < count; sourceIndex++, targetIndex++)
				Buffer[targetIndex] = Buffer[sourceIndex];
			int shiftSize = Math.Min(delta, count);
			BufferStartPosition = iterator.GetPosition(BufferStartPosition, shiftSize);
			base.ShiftBuffer(iterator, delta);
		}
		protected internal override Match CalculateMatch(BufferedDocumentCharacterIterator iterator, Regex regEx, SearchConditions conditions) {
			if (conditions.Length <= 0)
				return System.Text.RegularExpressions.Match.Empty;
			while (true) {
				Match result = CalculateMatchCore(regEx, conditions);
				if (result.Success) {
					if (IsMatchInTheStartOfBuffer(result)) {
						if (BufferOffset > 0) {
							CalculateSearchConditions(result, conditions);
							continue;
						}
						else if (ShouldRecalcMatchAtStartOfBuffer(conditions))
							continue;
					}
					if (IsMatchInTheEndOfBuffer(result)) {
						if (CanMoveNext())
							return null;
						else if (ShouldRecalcMatchAtEndOfBuffer(conditions))
							continue;
					}
					return result;
				}
				else
					return result;
			}
		}
		protected internal override int GetMatchAbsolutePosition(Match match) {
			return match.Index + BufferOffset;
		}
		protected override BufferedDocumentCharacterIterator CreateIterator(DocumentModelPosition position) {
			return new BufferedDocumentCharacterIteratorForward(position);
		}
		protected override DocumentModelPosition CalculateStartPosition() {
			return PositionConverter.ToDocumentModelPosition(PieceTable, StartPosition);
		}
		protected override bool CanMoveNext() {
			return BufferInputPosition.LogPosition < EndPosition;
		}
		protected override int CalculateBufferShiftSize(BufferedRegexSearchResult result) {
			return result.Match.Index + result.Match.Length;
		}
	}
	#endregion
	#region BufferedRegexSearchBackward
	public class BufferedRegexSearchBackward : BufferedRegexSearchBase {
		public BufferedRegexSearchBackward(PieceTable pieceTable, Regex regex)
			: base(pieceTable, regex) {
		}
		public BufferedRegexSearchBackward(PieceTable pieceTable, Regex regex, int maxGuaranteedSearchResultLength)
			: base(pieceTable, regex, maxGuaranteedSearchResultLength) {
		}
		internal BufferedRegexSearchBackward(PieceTable pieceTable, Regex regex, int bufferSize, int bufferShiftSize)
			: base(pieceTable, regex, bufferSize, bufferShiftSize) {
		}
		protected override void CalculateSearchConditions(Match prevMatch, SearchConditions conditions) {
			conditions.Length = prevMatch.Index - conditions.StartIndex;
		}
		protected internal override void AppendBuffer(BufferedDocumentCharacterIterator iterator) {
			int delta = BufferSize - Buffer.Length;
			iterator.AppendBuffer(delta);
			int count = Math.Min(delta, iterator.BufferLength);
			char[] charBuffer = new char[count];
			int index = 0;
			int charIndex = count - 1;
			for (; charIndex >= 0; charIndex--) {
				if (!CanMoveNext())
					break;
				charBuffer[charIndex] = iterator[index];
				BufferInputPosition = iterator.GetPosition(BufferInputPosition, 1);
				index++;
			}
			int startIndex = charIndex + 1;
			int appendCharsCount = count - startIndex;
			iterator.ShiftBuffer(appendCharsCount);
			BufferStartPosition = iterator.GetPosition(BufferStartPosition, index);
			Buffer.Insert(0, charBuffer, startIndex, appendCharsCount);
		}
		protected internal override Match CalculateMatch(BufferedDocumentCharacterIterator iterator, Regex regEx, SearchConditions conditions) {
			if (conditions.Length <= 0)
				return System.Text.RegularExpressions.Match.Empty;
			while (true) {
				Match result = CalculateMatchCore(regEx, conditions);
				if (result.Success) {
					if (IsMatchInTheStartOfBuffer(result)) {
						if (CanMoveNext())
							return null;
						else if (ShouldRecalcMatchAtStartOfBuffer(conditions))
							continue;
					}
					if (IsMatchInTheEndOfBuffer(result)) {
						if (BufferOffset > 0) {
							CalculateSearchConditions(result, conditions);
							continue;
						}
						else if (ShouldRecalcMatchAtEndOfBuffer(conditions))
							continue;
					}
					return result;
				}
				else
					return result;
			}
		}
		protected internal override int GetMatchAbsolutePosition(Match match) {
			return (BufferOffset + Buffer.Length) - match.Index;
		}
		protected override BufferedDocumentCharacterIterator CreateIterator(DocumentModelPosition position) {
			return new BufferedDocumentCharacterIteratorBackward(position);
		}
		protected override DocumentModelPosition CalculateStartPosition() {
			return PositionConverter.ToDocumentModelPosition(PieceTable, EndPosition);
		}
		protected override bool CanMoveNext() {
			return BufferInputPosition.LogPosition > StartPosition;
		}
		protected override int CalculateBufferShiftSize(BufferedRegexSearchResult result) {
			return Buffer.Length - result.Match.Index;
		}
	}
	#endregion
}
