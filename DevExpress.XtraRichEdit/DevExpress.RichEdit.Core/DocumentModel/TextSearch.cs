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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Model {
	#region BufferedDocumentCharacterIterator (abstract class)
	public abstract class BufferedDocumentCharacterIterator {
		int offset;
		StringBuilder buffer;
		DocumentModelPosition startPosition;
		DocumentModelPosition endPosition;
		protected BufferedDocumentCharacterIterator(DocumentModelPosition startPosition) {
			this.startPosition = startPosition.Clone();
			this.endPosition = startPosition.Clone();
			this.buffer = new StringBuilder();
		}
		public DocumentModelPosition StartPosition { get { return startPosition; } }
		public DocumentModelPosition EndPosition { get { return endPosition; } }
		public PieceTable PieceTable { get { return StartPosition.PieceTable; } }
		public abstract DocumentModelPosition CurrentPosition { get; }
		public virtual char this[int index] { get { return Buffer[GetCharacterIndex(index)]; } }
		public int BufferLength { get { return Buffer.Length - Offset; } }
		protected internal StringBuilder Buffer { get { return buffer; } }
		protected int Offset { get { return offset; } set { offset = value; } }
		public virtual void AppendBuffer(int minLength) {
			int delta = minLength - (buffer.Length - offset);
			if (delta <= 0)
				return;
			if (Buffer.Length > 0)
				FlushBuffer();
			AppendBufferCore(minLength);
			EnsureStartPositionIsVisible();
		}
		protected virtual void EnsureStartPositionIsVisible() {
			if (!IsRunVisible(StartPosition.RunIndex))
				StartPosition.CopyFrom(GetPositionBackward(EndPosition, Buffer.Length));
		}
		public virtual void ShiftBuffer(int delta) {
			Offset += delta;
			CurrentPosition.CopyFrom(GetPosition(delta));
		}
		protected bool IsRunVisible(RunIndex runIndex) {
			return PieceTable.VisibleTextFilter.IsRunVisible(runIndex);
		}
		public virtual DocumentModelPosition GetPosition(int offset) {
			return GetPosition(CurrentPosition, offset);
		}
		protected DocumentModelPosition GetPositionForward(DocumentModelPosition position, int offset) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex runIndex = position.RunIndex;
			int positionOffset = runs[runIndex].Length - position.RunOffset;
			int totalOffset = 0;
			if (IsRunVisible(position.RunIndex)) {
				if (offset < positionOffset) {
					DocumentModelPosition result = position.Clone();
					result.LogPosition += offset;
					return result;
				}
				totalOffset = positionOffset;
			}
			RunIndex lastRunIndex = new RunIndex(runs.Count - 1);
			DocumentLogPosition pos = position.RunStartLogPosition + runs[runIndex].Length;
			for (; ; ) {
				runIndex++;
				if (runIndex > lastRunIndex)
					break;
				TextRunBase run = runs[runIndex];
				if (IsRunVisible(runIndex)) {
					if (offset < (totalOffset + run.Length)) {
						DocumentModelPosition result = new DocumentModelPosition(PieceTable);
						result.LogPosition = pos + (offset - totalOffset);
						result.RunStartLogPosition = pos;
						result.RunIndex = runIndex;
						result.ParagraphIndex = run.Paragraph.Index;
						return result;
					}
					totalOffset += run.Length;
				}
				pos += run.Length;
			}
			Exceptions.ThrowInternalException();
			return null;
		}
		protected DocumentModelPosition GetPositionBackward(DocumentModelPosition position, int offset) {
			int totalOffset = 0;
			if (IsRunVisible(position.RunIndex)) {
				if (offset <= position.RunOffset) {
					DocumentModelPosition result = position.Clone();
					result.LogPosition -= offset;
					return result;
				}
				totalOffset = position.RunOffset;
			}
			DocumentLogPosition pos = position.RunStartLogPosition;
			TextRunCollection runs = PieceTable.Runs;
			RunIndex runIndex = position.RunIndex;
			for (; ; ) {
				runIndex--;
				if (runIndex < RunIndex.Zero)
					break;
				TextRunBase run = runs[runIndex];
				if (IsRunVisible(runIndex)) {
					if (offset <= (totalOffset + run.Length)) {
						DocumentModelPosition result = new DocumentModelPosition(PieceTable);
						result.LogPosition = pos - (offset - totalOffset);
						result.RunStartLogPosition = pos - run.Length;
						result.RunIndex = runIndex;
						result.ParagraphIndex = run.Paragraph.Index;
						return result;
					}
					totalOffset += run.Length;
				}
				pos -= run.Length;
			}
			Exceptions.ThrowInternalException();
			return null;
		}
		protected string GetRunText(TextRunBase run) {
			if (run is ParagraphRun)
				return "\n";
			return run.GetNonEmptyText(PieceTable.TextBuffer);
		}
		public bool Compare(int offset, Char ch, bool ignoreCase) {
			char documentChar = this[offset];
			if (Comare(documentChar, ch, ignoreCase)) {
				if (documentChar == Characters.ObjectMark || documentChar == Characters.FloatingObjectMark) {
					DocumentModelPosition pos = GetPosition(offset);
					if (!(PieceTable.Runs[pos.RunIndex] is TextRun))
						return false;
				}
				return true;
			}
			else
				return false;
		}
		bool Comare(char ch1, char ch2, bool ignoreCase) {
			if (ignoreCase)
				return Char.ToLowerInvariant(ch1) == Char.ToLowerInvariant(ch2);
			return ch1 == ch2;
		}
		protected abstract void AppendBufferCore(int minLength);
		protected abstract void FlushBuffer();
		protected internal abstract DocumentModelPosition GetPosition(DocumentModelPosition position, int offset);
		protected internal abstract int GetCharacterIndex(int index);
		protected internal abstract bool IsCharacterExist(int index);
	}
	#endregion
	#region BufferedDocumentCharacterIteratorForward
	public class BufferedDocumentCharacterIteratorForward : BufferedDocumentCharacterIterator {
		public BufferedDocumentCharacterIteratorForward(DocumentModelPosition startPosition)
			: base(startPosition) {
		}
		public override DocumentModelPosition CurrentPosition { get { return StartPosition; } }
		protected internal override int GetCharacterIndex(int index) {
			return index + Offset;
		}
		protected override void AppendBufferCore(int minLength) {
			RunIndex lastRunIndex = new RunIndex(PieceTable.Runs.Count - 1);
			TextRunCollection runs = PieceTable.Runs;
			while (Buffer.Length < minLength && EndPosition.RunIndex < lastRunIndex) {
				TextRunBase run = runs[EndPosition.RunIndex];
				if (IsRunVisible(EndPosition.RunIndex)) {
					if (EndPosition.RunOffset > 0)
						Buffer.Append(GetRunText(run).Substring(EndPosition.RunOffset));
					else
						Buffer.Append(GetRunText(run));
				}
				EndPosition.LogPosition += run.Length - EndPosition.RunOffset;
				EndPosition.RunStartLogPosition = EndPosition.LogPosition;
				EndPosition.RunIndex++;
				EndPosition.ParagraphIndex = runs[EndPosition.RunIndex].Paragraph.Index;
			}
		}
		protected override void FlushBuffer() {
			string newValue = Buffer.ToString(Offset, Buffer.Length - Offset);
			Buffer.Length = 0;
			Buffer.Append(newValue);
			Offset = 0;
		}
		protected internal override bool IsCharacterExist(int index) {
			return GetCharacterIndex(index) < Buffer.Length;
		}
		protected internal override DocumentModelPosition GetPosition(DocumentModelPosition position, int offset) {
			return GetPositionForward(position, offset);
		}
	}
	#endregion
	#region BufferedDocumentCharacterIteratorBackward
	public class BufferedDocumentCharacterIteratorBackward : BufferedDocumentCharacterIterator {
		public BufferedDocumentCharacterIteratorBackward(DocumentModelPosition startPosition)
			: base(startPosition) {
		}
		public override DocumentModelPosition CurrentPosition { get { return EndPosition; } }
		protected internal override int GetCharacterIndex(int index) {
			return (Buffer.Length - 1) - (index + Offset);
		}
		protected override void AppendBufferCore(int minLength) {
			TextRunCollection runs = PieceTable.Runs;
			DocumentLogPosition logPosition = StartPosition.LogPosition;
			if (StartPosition.RunOffset > 0) {
				if (IsRunVisible(StartPosition.RunIndex)) {
					TextRunBase run = runs[StartPosition.RunIndex];
					Buffer.Insert(0, GetRunText(run).Substring(0, StartPosition.RunOffset));
				}
				logPosition = StartPosition.RunStartLogPosition;
			}
			RunIndex runIndex = StartPosition.RunIndex;
			while (Buffer.Length < minLength && runIndex > new RunIndex(0)) {
				runIndex--;
				if (IsRunVisible(runIndex))
					Buffer.Insert(0, runs[runIndex].GetNonEmptyText(PieceTable.TextBuffer));
				logPosition -= runs[runIndex].Length;
			}
			StartPosition.LogPosition = logPosition;
			StartPosition.RunStartLogPosition = logPosition;
			StartPosition.RunIndex = runIndex;
			StartPosition.ParagraphIndex = runs[runIndex].Paragraph.Index;
		}
		protected override void FlushBuffer() {
			Buffer.Length -= Offset;
			Offset = 0;
		}
		protected internal override bool IsCharacterExist(int index) {
			return GetCharacterIndex(index) >= 0;
		}
		protected internal override DocumentModelPosition GetPosition(DocumentModelPosition position, int offset) {
			return GetPositionBackward(position, offset);
		}
	}
	#endregion
	#region BMTextSearchBase
	public abstract class BMTextSearchBase {
		#region Fields
		const int UnicodeAlphabetSize = 65536;
		int[] badCharacterShiftTable;
		int[] goodSuffixShiftTable;
		string originalPattern;
		string pattern;
		bool caseSensitive;
		readonly PieceTable pieceTable;
		#endregion
		protected BMTextSearchBase(PieceTable pieceTable, string pattern, bool caseSensitive) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.caseSensitive = caseSensitive;
			this.originalPattern = pattern;
			InitializeTables();
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public string OriginalPattern { get { return originalPattern; } }
		public bool CaseSensitive { get { return caseSensitive; } }
		protected internal string Pattern { get { return pattern; } set { pattern = value; } }
		protected internal int[] BadCharacterShiftTable { get { return badCharacterShiftTable; } }
		protected internal int[] GoodSuffixShiftTable { get { return goodSuffixShiftTable; } }
		#endregion
		public abstract DocumentModelPosition Search(DocumentLogPosition start, DocumentLogPosition end);
		protected virtual void InitializeTables() {
			this.pattern = CalculateWorkPattern();
			this.badCharacterShiftTable = CreateBadCharacterShiftTable();
			this.goodSuffixShiftTable = CreateGoodSuffixShiftTable();
		}
		protected virtual string CalculateWorkPattern() {
			return OriginalPattern;
		}
		protected char GetCharacter(DocumentCharacterIterator iterator, int offset) {
			char result = iterator.GetCharacter(offset);
			return CaseSensitive ? result : Char.ToLowerInvariant(result);
		}
		protected internal int[] CreateUnicodeAlphabetTable(int initialValue) {
			int[] result = new int[UnicodeAlphabetSize];
			for (int i = 0; i < UnicodeAlphabetSize; i++)
				result[i] = initialValue;
			return result;
		}
		protected internal void PopulateBadCharacterShiftTable(string pattern, int[] table) {
			int patternLength = pattern.Length;
			for (int i = 0; i < patternLength - 1; i++) {
				char ch = pattern[i];
				int offset = patternLength - i - 1;
				table[(int)ch] = offset;
			}
		}
		protected internal int[] CreateBadCharacterShiftTable() {
			int[] result = CreateUnicodeAlphabetTable(Pattern.Length);
			if (CaseSensitive)
				PopulateBadCharacterShiftTable(Pattern, result);
			else {
				PopulateBadCharacterShiftTable(Pattern.ToLowerInvariant(), result);
				PopulateBadCharacterShiftTable(Pattern.ToUpperInvariant(), result);
			}
			return result;
		}
		protected internal int[] CreateSuffixTable(string pattern) {
			int patternLength = pattern.Length;
			int[] result = new int[patternLength];
			if (!CaseSensitive)
				pattern = pattern.ToLowerInvariant();
			result[patternLength - 1] = patternLength;
			int g = patternLength - 1;
			int f = 0;
			for (int i = patternLength - 2; i >= 0; i--) {
				if (i > g && result[i + patternLength - 1 - f] < i - g)
					result[i] = result[i + patternLength - 1 - f];
				else {
					if (i < g)
						g = i;
					f = i;
					while (g >= 0 && pattern[g] == pattern[g + patternLength - 1 - f])
						g--;
					result[i] = f - g;
				}
			}
			return result;
		}
		protected internal int[] CreateGoodSuffixShiftTable() {
			int patternLength = Pattern.Length;
			int[] result = new int[patternLength];
			for (int i = 0; i < patternLength; i++)
				result[i] = patternLength;
			int[] suffixes = CreateSuffixTable(Pattern);
			for (int i = patternLength - 1; i >= 0; i--) {
				if (suffixes[i] == i + 1) {
					for (int j = 0; j < patternLength - i - 1; j++) {
						if (result[j] == patternLength)
							result[j] = patternLength - i - 1;
					}
				}
			}
			for (int i = 0; i < patternLength - 1; i++)
				result[patternLength - 1 - suffixes[i]] = patternLength - i - 1;
			return result;
		}
	}
	#endregion
	#region BMTextSearchForward
	public class BMTextSearchForward : BMTextSearchBase {
		public BMTextSearchForward(PieceTable pieceTable, string pattern, bool caseSensitive)
			: base(pieceTable, pattern, caseSensitive) {
		}
		public override DocumentModelPosition Search(DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(PieceTable, start);
			BufferedDocumentCharacterIterator iterator = new BufferedDocumentCharacterIteratorForward(position);
			int patternLength = Pattern.Length;
			int shift = patternLength;
			int u = 0;
			while (iterator.CurrentPosition.LogPosition < end) {
				iterator.AppendBuffer(patternLength);
				int offset = patternLength - 1;
				if (!iterator.IsCharacterExist(offset))
					break;
				while (offset >= 0 && iterator.Compare(offset, Pattern[offset], !CaseSensitive)) {
					offset--;
					if (u != 0 && offset == patternLength - 1 - shift)
						offset -= u;
				}
				if (offset < 0) {
					if (iterator.GetPosition(Pattern.Length).LogPosition <= end)
						return iterator.StartPosition;
					return null;
				}
				else {
					int v = patternLength - 1 - offset;
					int turboShift = u - v;
					int bcShift = BadCharacterShiftTable[iterator[offset]] - patternLength + 1 + offset;
					shift = Math.Max(turboShift, bcShift);
					shift = Math.Max(shift, GoodSuffixShiftTable[offset]);
					if (shift == GoodSuffixShiftTable[offset])
						u = Math.Min(patternLength - shift, v);
					else {
						if (turboShift < bcShift)
							shift = Math.Max(shift, u + 1);
						u = 0;
					}
				}
				iterator.ShiftBuffer(shift);
			}
			return null;
		}
	}
	#endregion
	#region BMTextSearchBackward
	public class BMTextSearchBackward : BMTextSearchBase {
		public BMTextSearchBackward(PieceTable pieceTable, string pattern, bool caseSensitive)
			: base(pieceTable, pattern, caseSensitive) {
		}
		protected override string CalculateWorkPattern() {
			string pattern = base.CalculateWorkPattern();
			StringBuilder sb = new StringBuilder();
			for (int i = pattern.Length - 1; i >= 0; i--)
				sb.Append(pattern[i]);
			return sb.ToString();
		}
		public override DocumentModelPosition Search(DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(PieceTable, end);
			BufferedDocumentCharacterIterator iterator = new BufferedDocumentCharacterIteratorBackward(position);
			int patternLength = Pattern.Length;
			int shift = patternLength;
			int u = 0;
			while (iterator.CurrentPosition.LogPosition > start) {
				iterator.AppendBuffer(patternLength);
				int offset = patternLength - 1;
				if (!iterator.IsCharacterExist(offset))
					break;
				while (offset >= 0 && iterator.Compare(offset, Pattern[offset], !CaseSensitive)) {
					offset--;
					if (u != 0 && offset == shift)
						offset -= u;
				}
				if (offset < 0) {
					DocumentModelPosition result = iterator.GetPosition(patternLength);
					if (result.LogPosition >= start)
						return result;
					return null;
				}
				else {
					int v = patternLength - 1 - offset;
					int turboShift = u - v;
					int bcShift = BadCharacterShiftTable[iterator[offset]] - patternLength + 1 + offset;
					shift = Math.Max(turboShift, bcShift);
					shift = Math.Max(shift, GoodSuffixShiftTable[offset]);
					if (shift == GoodSuffixShiftTable[offset])
						u = Math.Min(patternLength - shift, v);
					else {
						if (turboShift < bcShift)
							shift = Math.Max(shift, u + 1);
						u = 0;
					}
				}
				iterator.ShiftBuffer(shift);
			}
			return null;
		}
	}
	#endregion
	#region DocumentCharacterIterator (abstract class)
	public abstract class DocumentCharacterIterator {
		#region Fields
		DocumentModelPosition currentPosition;
		DocumentLogPosition startPosition;
		DocumentLogPosition endPosition;
		readonly PieceTable pieceTable;
		RunIndex cachedRunIndex = new RunIndex(-1);
		bool isCachedRunVisible;
		#endregion
		protected DocumentCharacterIterator(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end)
			: this(pieceTable) {
			SetInterval(start, end);
		}
		protected DocumentCharacterIterator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		public DocumentLogPosition StartPosition { get { return startPosition; } internal set { startPosition = value; } }
		public DocumentLogPosition EndPosition { get { return endPosition; } internal set { endPosition = value; } }
		public DocumentModelPosition CurrentPosition {
			get {
				if (currentPosition == null)
					InitializeCurrentPosition();
				return currentPosition;
			}
			internal set { currentPosition = value; }
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		protected TextRunBase CurrentRun { get { return PieceTable.Runs[CurrentPosition.RunIndex]; } }
		#endregion
		public abstract bool CanMoveOnOffset(int offset);
		protected abstract void InitializeCurrentPosition();
		protected internal virtual void SetInterval(DocumentLogPosition start, DocumentLogPosition end) {
			if (start < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("startPosition", start);
			if (end < start || end > PieceTable.DocumentEndLogPosition)
				Exceptions.ThrowArgumentException("endPosition", end);
			this.startPosition = start;
			this.endPosition = end;
		}
		protected internal virtual void SetCurrentPosition(DocumentModelPosition pos) {
			this.currentPosition = pos;
		}
		protected internal virtual void InvalidateCurrentPosition() {
			SetCurrentPosition(null);
		}
		public virtual char GetCharacter() {
			return GetCharacterByPosition(CurrentPosition);
		}
		public virtual char GetCharacter(int offset) {
			DocumentModelPosition charPosition = GetModelPosition(offset);
			return GetCharacterByPosition(charPosition);
		}
		public virtual char GetCharacter(int offset, out DocumentLogPosition pos) {
			DocumentModelPosition charPosition = GetModelPosition(offset);
			pos = charPosition.LogPosition;
			return GetCharacterByPosition(charPosition);
		}
		protected internal DocumentModelPosition GetModelPosition(int offset) {
			return GetModelPosition(CurrentPosition, offset);
		}
		protected internal DocumentModelPosition GetModelPosition(DocumentModelPosition pos, int offset) {
			DocumentModelPosition result = pos.Clone();
			ShiftModelPosition(result, offset);
			return result;
		}
		protected internal char GetCharacterByPosition(DocumentModelPosition pos) {
			string runText = PieceTable.Runs[pos.RunIndex].GetNonEmptyText(PieceTable.TextBuffer);
			int offset = pos.LogPosition - pos.RunStartLogPosition;
			if (offset < 0 || offset >= runText.Length)
				return Char.MinValue;
			return runText[offset];
		}
		protected internal virtual bool IsVisible(DocumentModelPosition pos) {
			RunIndex runIndex = pos.RunIndex;
			if (runIndex != cachedRunIndex) {
				cachedRunIndex = runIndex;
				isCachedRunVisible = pieceTable.VisibleTextFilter.IsRunVisible(runIndex);
			}
			return isCachedRunVisible;
		}
		public virtual bool HasNextChar() {
			return CanMoveOnOffset(0);
		}
		public virtual void ShiftCurrentPosition(int offset) {
			ShiftModelPosition(CurrentPosition, offset);
		}
		public virtual void MoveNext() {
			ShiftModelPosition(CurrentPosition, 1);
		}
		protected internal void ShiftModelPosition(DocumentModelPosition pos, int offset) {
			int count = Math.Abs(offset);
			for (int i = 0; i < count; i++)
				MoveToVisiblePosition(pos);
		}
		protected abstract void MoveToVisiblePosition(DocumentModelPosition result);
	}
	#endregion
	#region DocumentCharacterIteratorForward
	public class DocumentCharacterIteratorForward : DocumentCharacterIterator {
		public DocumentCharacterIteratorForward(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end)
			: base(pieceTable, start, end) {
		}
		internal DocumentCharacterIteratorForward(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void InitializeCurrentPosition() {
			CurrentPosition = PositionConverter.ToDocumentModelPosition(PieceTable, StartPosition);
		}
		public override bool CanMoveOnOffset(int offset) {
			return (CurrentPosition.LogPosition + offset <= EndPosition);
		}
		protected override void MoveToVisiblePosition(DocumentModelPosition pos) {
			do {
				if (pos.LogPosition < PieceTable.DocumentEndLogPosition)
					DocumentModelPosition.MoveForwardCore(pos);
				else
					pos.LogPosition++;
			}
			while (!IsVisible(pos));
		}
	}
	#endregion
	#region DocumentCharacterIteratorBackward
	public class DocumentCharacterIteratorBackward : DocumentCharacterIterator {
		public DocumentCharacterIteratorBackward(PieceTable pieceTable, DocumentLogPosition startLogPosition, DocumentLogPosition endLogPosition)
			: base(pieceTable, startLogPosition, endLogPosition) {
		}
		internal DocumentCharacterIteratorBackward(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void InitializeCurrentPosition() {
			CurrentPosition = PositionConverter.ToDocumentModelPosition(PieceTable, EndPosition);
		}
		public override bool CanMoveOnOffset(int offset) {
			return (CurrentPosition.LogPosition - offset >= StartPosition);
		}
		protected override void MoveToVisiblePosition(DocumentModelPosition pos) {
			do {
				if (pos.LogPosition > PieceTable.DocumentStartLogPosition)
					DocumentModelPosition.MoveBackwardCore(pos);
				else
					pos.LogPosition--;
			}
			while (!IsVisible(pos));
		}
	}
	#endregion
	[Flags]
	public enum SearchOptions {
		None = 0x00000000,
		CaseSensitive = 0x00000001,
		WholeWord = 0x00000002
	}
	#region DocumentCharacterIteratorFactory (abstract class)
	public abstract class DocumentCharacterIteratorFactory {
		readonly PieceTable pieceTable;
		protected DocumentCharacterIteratorFactory(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get { return pieceTable; } }
		public abstract BufferedDocumentCharacterIterator CreateIterator(DocumentLogPosition start, DocumentLogPosition end);
		public abstract BufferedDocumentCharacterIterator CreateIterator(RunInfo prevResult, DocumentLogPosition start, DocumentLogPosition end);
	}
	#endregion
	#region DocumentCharacterIteratorFactoryForward
	public class DocumentCharacterIteratorFactoryForward : DocumentCharacterIteratorFactory {
		public DocumentCharacterIteratorFactoryForward(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override BufferedDocumentCharacterIterator CreateIterator(DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition startModelPos = PositionConverter.ToDocumentModelPosition(PieceTable, start);
			return new BufferedDocumentCharacterIteratorForward(startModelPos);
		}
		public override BufferedDocumentCharacterIterator CreateIterator(RunInfo prevResult, DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition pos = DocumentModelPosition.MoveForward(prevResult.Start);
			BufferedDocumentCharacterIterator result = new BufferedDocumentCharacterIteratorForward(pos);
			return result;
		}
	}
	#endregion
	#region DocumentCharacterIteratorFactoryBackward
	public class DocumentCharacterIteratorFactoryBackward : DocumentCharacterIteratorFactory {
		public DocumentCharacterIteratorFactoryBackward(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override BufferedDocumentCharacterIterator CreateIterator(DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition pos = PositionConverter.ToDocumentModelPosition(PieceTable, end);
			return new BufferedDocumentCharacterIteratorBackward(pos);
		}
		public override BufferedDocumentCharacterIterator CreateIterator(RunInfo prevResult, DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition pos = DocumentModelPosition.MoveBackward(prevResult.End);
			BufferedDocumentCharacterIterator result = new BufferedDocumentCharacterIteratorBackward(pos);
			return result;
		}
	}
	#endregion
	#region SearchResult
	public class SearchResult {
		readonly RunInfo value;
		internal SearchResult(RunInfo value) {
			this.value = value;
		}
		internal SearchResult()
			: this(null) {
		}
		public RunInfo Value { get { return value; } }
		public virtual bool Success { get { return value != null; } }
	}
	#endregion
	#region RegexSearchResult
	public class RegexSearchResult : SearchResult {
		readonly BufferedRegexSearchResult matchInfo;
		internal RegexSearchResult(RunInfo result, BufferedRegexSearchResult matchInfo)
			: base(result) {
			this.matchInfo = matchInfo;
		}
		public BufferedRegexSearchResult MatchInfo { get { return matchInfo; } }
	}
	#endregion
	public interface ISearchStrategy {
		SearchResult Match(string pattern, SearchOptions options, DocumentLogPosition start, DocumentLogPosition end);
	}
	#region SearchIntervalCalculator (abstract class)
	public abstract class SearchIntervalCalculator {
		public abstract DocumentLogPosition CalculateStartPosition(DocumentLogPosition start, DocumentLogPosition end, SearchResult prevResult);
		public abstract DocumentLogPosition CalculateEndPosition(DocumentLogPosition start, DocumentLogPosition end, SearchResult prevResult);
	}
	#endregion
	#region SearchItntervalCalculatorForward
	public class SearchItntervalCalculatorForward : SearchIntervalCalculator {
		public override DocumentLogPosition CalculateStartPosition(DocumentLogPosition start, DocumentLogPosition end, SearchResult prevResult) {
			if (prevResult != null)
				return DocumentModelPosition.MoveForward(prevResult.Value.Start).LogPosition;
			else
				return start;
		}
		public override DocumentLogPosition CalculateEndPosition(DocumentLogPosition start, DocumentLogPosition end, SearchResult prevResult) {
			return end;
		}
	}
	#endregion
	#region SearchItntervalCalculatorBackward
	public class SearchItntervalCalculatorBackward : SearchIntervalCalculator {
		public override DocumentLogPosition CalculateStartPosition(DocumentLogPosition start, DocumentLogPosition end, SearchResult prevResult) {
			return start;
		}
		public override DocumentLogPosition CalculateEndPosition(DocumentLogPosition start, DocumentLogPosition end, SearchResult prevResult) {
			if (prevResult != null)
				return DocumentModelPosition.MoveBackward(prevResult.Value.End).LogPosition;
			else
				return end;
		}
	}
	#endregion
	#region TextSearchStrategyBase (abstract class)
	public abstract class TextSearchStrategyBase<T> : ISearchStrategy {
		readonly PieceTable pieceTable;
		protected TextSearchStrategyBase(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public virtual SearchResult Match(string pattern, SearchOptions options, DocumentLogPosition start, DocumentLogPosition end) {
			if (String.IsNullOrEmpty(pattern))
				Exceptions.ThrowArgumentException("pattern", pattern);
			bool wholeWord = (options & SearchOptions.WholeWord) != 0;
			T searcher = CreateSearcher(PreparePattern(pattern), options);
			SearchResult result = null;
			SearchIntervalCalculator calculator = CreateIntervalCalculator();
			for (; ; ) {
				result = MatchCore(searcher, calculator.CalculateStartPosition(start, end, result), calculator.CalculateEndPosition(start, end, result));
				if (!result.Success || !wholeWord || IsWordWhole(result.Value))
					break;
			}
			return result;
		}
		protected internal virtual string PreparePattern(string pattern) {
			return pattern.Replace("\r\n", "\n");
		}
		protected abstract SearchResult MatchCore(T searcher, DocumentLogPosition start, DocumentLogPosition end);
		protected abstract SearchIntervalCalculator CreateIntervalCalculator();
		protected abstract T CreateSearcher(string pattern, SearchOptions options);
		protected internal virtual bool IsWordWhole(RunInfo runInfo) {
			char prevChar = GetPrevCharacter(runInfo.Start);
			if (prevChar != Char.MinValue && Char.IsLetterOrDigit(prevChar))
				return false;
			char nextChar = GetNextCharacter(runInfo.End);
			if (nextChar != Char.MinValue && Char.IsLetterOrDigit(nextChar))
				return false;
			return true;
		}
		protected internal virtual char GetPrevCharacter(DocumentModelPosition start) {
			if (start.LogPosition <= PieceTable.DocumentStartLogPosition)
				return Char.MinValue;
			if (start.RunOffset > 0)
				return PieceTable.Runs[start.RunIndex].GetNonEmptyText(PieceTable.TextBuffer)[start.RunOffset - 1];
			RunIndex runIndex = start.RunIndex - 1;
			string runText = PieceTable.Runs[runIndex].GetNonEmptyText(PieceTable.TextBuffer);
			return runText[runText.Length - 1];
		}
		protected internal virtual char GetNextCharacter(DocumentModelPosition end) {
			if (end.LogPosition >= PieceTable.DocumentEndLogPosition)
				return Char.MinValue;
			return PieceTable.Runs[end.RunIndex].GetNonEmptyText(PieceTable.TextBuffer)[end.RunOffset];
		}
	}
	#endregion
	#region TextSearchByStringStrategy
	public abstract class TextSearchByStringStrategy : TextSearchStrategyBase<BMTextSearchBase> {
		protected TextSearchByStringStrategy(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override SearchResult MatchCore(BMTextSearchBase searcher, DocumentLogPosition start, DocumentLogPosition end) {
			DocumentModelPosition result = searcher.Search(start, end);
			if (result == null)
				return new SearchResult();
			BufferedDocumentCharacterIteratorForward iterator = new BufferedDocumentCharacterIteratorForward(result);
			RunInfo runInfo = new RunInfo(PieceTable);
			runInfo.Start.CopyFrom(result);
			runInfo.End.CopyFrom(iterator.GetPosition(searcher.Pattern.Length));
			return new SearchResult(runInfo);
		}
	}
	#endregion
	#region TextSearchByStringForwardStrategy
	public class TextSearchByStringForwardStrategy : TextSearchByStringStrategy {
		public TextSearchByStringForwardStrategy(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override BMTextSearchBase CreateSearcher(string pattern, SearchOptions options) {
			bool caseSensitive = (options & SearchOptions.CaseSensitive) != 0;
			return new BMTextSearchForward(PieceTable, pattern, caseSensitive);
		}
		protected override SearchIntervalCalculator CreateIntervalCalculator() {
			return new SearchItntervalCalculatorForward();
		}
	}
	#endregion
	#region TextSearchByStringBackwardStrategy
	public class TextSearchByStringBackwardStrategy : TextSearchByStringStrategy {
		public TextSearchByStringBackwardStrategy(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override SearchIntervalCalculator CreateIntervalCalculator() {
			return new SearchItntervalCalculatorBackward();
		}
		protected override BMTextSearchBase CreateSearcher(string pattern, SearchOptions options) {
			bool caseSensitive = (options & SearchOptions.CaseSensitive) != 0;
			return new BMTextSearchBackward(PieceTable, pattern, caseSensitive);
		}
	}
	#endregion
	#region TextSearchByRegexStrategy (abstract class)
	public abstract class TextSearchByRegexStrategy : TextSearchStrategyBase<BufferedRegexSearchBase> {
		protected TextSearchByRegexStrategy(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override SearchResult MatchCore(BufferedRegexSearchBase searcher, DocumentLogPosition start, DocumentLogPosition end) {
			BufferedRegexSearchResult result = searcher.Match(start, end);
			if (result == null)
				return new SearchResult();
			BufferedDocumentCharacterIteratorForward iterator = new BufferedDocumentCharacterIteratorForward(result.Offset);
			RunInfo runInfo = new RunInfo(PieceTable);
			runInfo.Start.CopyFrom(iterator.GetPosition(result.Match.Index));
			runInfo.End.CopyFrom(iterator.GetPosition(runInfo.Start, result.Match.Length));
			return new RegexSearchResult(runInfo, result);
		}
	}
	#endregion
	#region TextSearchByRegexForwardStrategy
	public class TextSearchByRegexForwardStrategy : TextSearchByRegexStrategy {
		public TextSearchByRegexForwardStrategy(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override SearchIntervalCalculator CreateIntervalCalculator() {
			return new SearchItntervalCalculatorForward();
		}
		protected override BufferedRegexSearchBase CreateSearcher(string pattern, SearchOptions options) {
			bool ignoreCase = (options & SearchOptions.CaseSensitive) == 0;
			RegexOptions regexOptions = (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			Regex regex = null;
			try {
				regex = new Regex(pattern, regexOptions);
			}
			catch {
				throw new IncorrectRegularExpressionException();
			}
			int bufferSize = PieceTable.DocumentModel.SearchOptions.RegExResultMaxGuaranteedLength;
			return new BufferedRegexSearchForward(PieceTable, regex, bufferSize);
		}
	}
	#endregion
	public class IncorrectRegularExpressionException : Exception {
		public IncorrectRegularExpressionException()
			: base(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_IncorrectPattern)) {
		}
	}
	#region TextSearchByRegexBackwardStrategy
	public class TextSearchByRegexBackwardStrategy : TextSearchByRegexStrategy {
		public TextSearchByRegexBackwardStrategy(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override SearchIntervalCalculator CreateIntervalCalculator() {
			return new SearchItntervalCalculatorBackward();
		}
		protected override BufferedRegexSearchBase CreateSearcher(string pattern, SearchOptions options) {
			bool ignoreCase = (options & SearchOptions.CaseSensitive) == 0;
			RegexOptions regexOptions = RegexOptions.RightToLeft | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			Regex regex = null;
			try {
				regex = new Regex(pattern, regexOptions);
			}
			catch {
				throw new IncorrectRegularExpressionException();
			}
			int bufferSize = PieceTable.DocumentModel.SearchOptions.RegExResultMaxGuaranteedLength;
			return new BufferedRegexSearchBackward(PieceTable, regex, bufferSize);
		}
	}
	#endregion
}
