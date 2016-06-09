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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using DevExpress.Office;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.SpellChecker {
	#region RichEditTextController (abstract class)
	public abstract class RichEditTextController : ISpellCheckTextController, ISupportMultiCulture {
		readonly InnerRichEditControl control;
		string guid;
		protected RichEditTextController(InnerRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.guid = Guid.NewGuid().ToString();
		}
		public InnerRichEditControl Control { get { return control; } }
		protected DocumentModel DocumentModel { get { return Control.DocumentModel; } }
		protected virtual PieceTable PieceTable { get { return DocumentModel.ActivePieceTable; } }
		protected abstract SpellCheckerWordIterator GetWordIterator(PieceTable pieceTable);
		protected SentencePositionCalculator GetSentencePositionCalculator(PieceTable pieceTable) {
			return new SentencePositionCalculator(GetWordIterator(pieceTable));
		}
		protected internal virtual bool CanDoNextStep(DocumentPosition position) {
			if (position != null) {
				PieceTable pieceTable = position.Position.PieceTable;
				return position.Position.LogPosition < pieceTable.DocumentEndLogPosition;
			}
			else
				return true;
		}
		protected internal virtual bool DeleteWord(DocumentPosition start, DocumentPosition end) {
			DocumentModelPosition startPosition = start.Position;
			DocumentModelPosition finishPosition = end.Position;
			PreparePositionsForDelete(startPosition, finishPosition);
			PieceTable pieceTable = startPosition.PieceTable;
			if (!DeleteWordCore(pieceTable, start.LogPosition, end.LogPosition))
				return false;
			start.InvalidatePosition();
			end.InvalidatePosition();
			return true;
		}
		bool DeleteWordCore(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition finish) {
			if (!CanEditRange(pieceTable, start, finish))
				return false;
			pieceTable.DeleteContent(start, finish - start, start >= pieceTable.DocumentEndLogPosition);
			return true;
		}
		internal void PreparePositionsForDelete(DocumentModelPosition start, DocumentModelPosition finish) {
			Debug.Assert(Object.ReferenceEquals(start.PieceTable, finish.PieceTable));
			VisibleCharactersIterator iterator = new VisibleCharactersIterator(start.PieceTable);
			if (!TryShiftStartPosition(start, iterator))
				TryShiftEndPosition(finish, iterator);
		}
		bool TryShiftStartPosition(DocumentModelPosition pos, VisibleCharactersIterator iterator) {
			if (pos.LogPosition <= pos.PieceTable.DocumentStartLogPosition)
				return false;
			DocumentModelPosition prevPos = iterator.MoveBack(pos);
			if (Char.IsWhiteSpace(GetCharacter(prevPos))) {
				pos.CopyFrom(prevPos);
				return true;
			}
			return false;
		}
		bool TryShiftEndPosition(DocumentModelPosition pos, VisibleCharactersIterator iterator) {
			if (pos.LogPosition >= pos.PieceTable.DocumentEndLogPosition)
				return false;
			char ch = GetCharacter(pos);
			if (Char.IsWhiteSpace(ch)) {
				iterator.MoveForwardCore(pos);
				return true;
			}
			return false;
		}
		static char GetCharacter(DocumentModelPosition pos) {
			PieceTable pieceTable = pos.PieceTable;
			string runText = pieceTable.Runs[pos.RunIndex].GetNonEmptyText(pieceTable.TextBuffer);
			return runText[pos.RunOffset];
		}
		protected internal virtual DocumentPosition GetNextPosition(DocumentPosition pos) {
			DocumentModelPosition position = GetModelPosition(pos);
			SpellCheckerWordIterator iterator = GetWordIterator(position.PieceTable);
			return CreateDocumentPosition(iterator.MoveForward(position));
		}
		DocumentModelPosition GetModelPosition(DocumentPosition position) {
			if (position != null)
				return position.Position;
			return new DocumentModelPosition(PieceTable);
		}
		DocumentPosition CreateDocumentPosition(DocumentModelPosition pos) {
			return new DocumentPosition(pos);
		}
		protected internal virtual string GetPreviousWord(DocumentPosition pos) {
			DocumentPosition prevWordEndPosition = GetPrevPosition(pos);
			if (prevWordEndPosition == pos)
				return String.Empty;
			DocumentPosition prevWordStartPosition = GetWordStartPosition(prevWordEndPosition);
			return GetWord(prevWordStartPosition, prevWordEndPosition);
		}
		protected internal virtual string GetWord(DocumentPosition start, DocumentPosition finish) {
			if (!start.IsValid || !finish.IsValid)
				return String.Empty;
			if (Position.Equals(start, finish))
				return String.Empty;
			PieceTable pieceTable = start.Position.PieceTable;
			return GetVisibleText(pieceTable, start.Position, finish.Position);
		}
		string GetVisibleText(PieceTable pieceTable, DocumentModelPosition start, DocumentModelPosition end) {
			if (end.LogPosition - start.LogPosition <= 0)
				return String.Empty;
			TextRunBase startRun = pieceTable.Runs[start.RunIndex];
			if (start.RunIndex == end.RunIndex)
				return GetRunText(pieceTable, start.RunIndex, start.RunOffset, end.RunOffset - 1);
			StringBuilder result = new StringBuilder();
			result.Append(GetRunText(pieceTable, start.RunIndex, start.RunOffset, startRun.Length - 1));
			for (RunIndex i = start.RunIndex + 1; i < end.RunIndex; i++)
				result.Append(GetRunText(pieceTable, i));
			if (end.RunOffset > 0)
				result.Append(GetRunText(pieceTable, end.RunIndex, 0, end.RunOffset - 1));
			return result.ToString();
		}
		string GetRunText(PieceTable pieceTable, RunIndex index) {
			return GetRunText(pieceTable, index, 0, pieceTable.Runs[index].Length - 1);
		}
		string GetRunText(PieceTable pieceTable, RunIndex index, int from, int to) {
			if (pieceTable.VisibleTextFilter.IsRunVisible(index))
				return pieceTable.Runs[index].GetText(pieceTable.TextBuffer, from, to);
			return null;
		}
		protected internal virtual DocumentPosition GetWordStartPosition(DocumentPosition pos) {
			DocumentModelPosition position = GetModelPosition(pos);
			SpellCheckerWordIterator iterator = GetWordIterator(position.PieceTable);
			return CreateDocumentPosition(iterator.MoveToWordStart(position));
		}
		protected internal virtual bool HasLetters(DocumentPosition start, DocumentPosition finish) {
			if (Position.Compare(start, finish) >= 0)
				return false;
			string word = GetWord(start, finish);
			if (String.IsNullOrEmpty(word))
				return false;
			int charsCount = word.Length;
			for (int i = 0; i < charsCount; i++) {
				if (Char.IsLetterOrDigit(word[i]))
					return true;
			}
			return false;
		}
		protected internal virtual bool ReplaceWord(DocumentPosition start, DocumentPosition end, string word) {
			string oldWord = GetWord(start, end);
			if (String.Equals(oldWord, word, StringComparison.Ordinal))
				return false;
			if (!ReplaceWord(start.Position, end.Position, word))
				return false;
			start.InvalidatePosition();
			end.InvalidatePosition();
			return true;
		}
		internal bool ReplaceWord(DocumentModelPosition start, DocumentModelPosition end, string word) {
			char ch = GetCharacter(end);
			if (ch == '.' && word.EndsWith(".", StringComparison.Ordinal))
				end = DocumentModelPosition.MoveForward(end);
			PieceTable pieceTable = start.PieceTable;
			return ReplaceWordCore(pieceTable, word, start.LogPosition, end.LogPosition);
		}
		bool ReplaceWordCore(PieceTable pieceTable, string word, DocumentLogPosition start, DocumentLogPosition finish) {
			if (!CanEditRange(pieceTable, start, finish))
				return false;
			pieceTable.ReplaceText(start, finish - start, word);
			return true;
		}
		bool CanEditRange(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition finish) {
			return Control.IsEditable && pieceTable.CanEditRange(start, finish);
		}
		protected internal virtual DocumentPosition GetPrevPosition(DocumentPosition pos) {
			DocumentModelPosition position = GetModelPosition(pos);
			SpellCheckerWordIterator iterator = GetWordIterator(position.PieceTable);
			return CreateDocumentPosition(iterator.MoveBack(position));
		}
		protected internal bool NotLetterOrDigit(char ch) {
			return !Char.IsLetterOrDigit(ch);
		}
		protected internal virtual DocumentPosition GetSentenceEndPosition(DocumentPosition pos) {
			DocumentModelPosition result = GetModelPosition(pos);
			SentencePositionCalculator calculator = GetSentencePositionCalculator(result.PieceTable);
			result = calculator.GetSentenceEndPosition(result);
			return CreateDocumentPosition(result);
		}
		protected internal virtual DocumentPosition GetSentenceStartPosition(DocumentPosition pos) {
			DocumentModelPosition result = GetModelPosition(pos);
			SentencePositionCalculator calculator = GetSentencePositionCalculator(result.PieceTable);
			result = calculator.GetSentenceStartPosition(result);
			return CreateDocumentPosition(result);
		}
		#region ISpellCheckTextController Members
		public string Text { get { return guid; } set { guid = Guid.NewGuid().ToString(); } }
		public char this[Position position] { get { return GetCharacter((DocumentPosition)position); } }
		char GetCharacter(DocumentPosition position) {
			if (!position.IsValid)
				position.UpdatePosition();
			DocumentModelPosition modelPosition = position.Position;
			TextRunBase run = PieceTable.Runs[modelPosition.RunIndex];
			return PieceTable.TextBuffer[run.StartIndex + modelPosition.RunOffset];
		}
		public Position GetSentenceStartPosition(Position pos) {
			return GetSentenceStartPosition((DocumentPosition)pos);
		}
		public Position GetSentenceEndPosition(Position pos) {
			return GetSentenceEndPosition((DocumentPosition)pos);
		}
		public bool CanDoNextStep(Position position) {
			return CanDoNextStep((DocumentPosition)position);
		}
		public bool DeleteWord(ref Position start, ref Position finish) {
			return DeleteWord((DocumentPosition)start, (DocumentPosition)finish);
		}
		public Position GetNextPosition(Position pos) {
			return GetNextPosition((DocumentPosition)pos);
		}
		public Position GetPrevPosition(Position pos) {
			return GetPrevPosition((DocumentPosition)pos);
		}
		public string GetPreviousWord(Position pos) {
			return GetPreviousWord((DocumentPosition)pos);
		}
		public Position GetTextLength(string text) {
			if (IsFinishPositionRequest(text)) {
				PieceTable pieceTable = PieceTable;
				return new DocumentPosition(DocumentModelPosition.FromRunStart(pieceTable, new RunIndex(pieceTable.Runs.Count - 1)));
			}
			return new PositionOffset(text.Length);
		}
		bool IsFinishPositionRequest(string text) {
			return text == this.Text;
		}
		public string GetWord(Position start, Position finish) {
			return GetWord((DocumentPosition)start, (DocumentPosition)finish);
		}
		public Position GetWordStartPosition(Position pos) {
			return GetWordStartPosition((DocumentPosition)pos);
		}
		public bool HasLetters(Position start, Position finish) {
			return HasLetters((DocumentPosition)start, (DocumentPosition)finish);
		}
		public bool ReplaceWord(Position start, Position finish, string word) {
			return ReplaceWord((DocumentPosition)start, (DocumentPosition)finish, word);
		}
		#endregion
		#region ISupportMultiCulture Members
		public CultureInfo GetCulture(Position start, Position end) {
			return GetCulture((DocumentPosition)start, (DocumentPosition)end);
		}
		public bool ShouldCheckWord(Position start, Position end) {
			return ShouldCheckWord((DocumentPosition)start, (DocumentPosition)end);
		}
		protected CultureInfo GetCulture(DocumentPosition start, DocumentPosition end) {
			if (!Control.Options.SpellChecker.AutoDetectDocumentCulture)
				return null;
			DocumentModelPosition sartModelPos = start.Position;
			DocumentModelPosition endModelPos = end.Position;
			LangInfo? langInfo = sartModelPos.PieceTable.GetLanguageInfo(sartModelPos, endModelPos);
			return langInfo.HasValue ? langInfo.Value.Latin : null;
		}
		public bool ShouldCheckWord(DocumentPosition start, DocumentPosition end) {
			if (Control.Options.SpellChecker.IgnoreNoProof)
				return true;
			DocumentModelPosition sartModelPos = start.Position;
			DocumentModelPosition endModelPos = end.Position;
			return sartModelPos.PieceTable.ShouldCheckWord(sartModelPos, endModelPos);			
		}
		#endregion
	}
	#endregion
	#region RichEditSpellCheckController
	public class RichEditSpellCheckController : RichEditTextController, ISpellCheckTextControlController {
		public RichEditSpellCheckController(IRichEditControl control)
			: base(GetInnerControl(control)) {
		}
		static InnerRichEditControl GetInnerControl(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			return control.InnerControl;
		}
		public RichEditSpellCheckController(InnerRichEditControl control)
			: base(control) {
		}
		protected override SpellCheckerWordIterator GetWordIterator(PieceTable pieceTable) {
			return new SpellCheckerWordIterator(pieceTable);
		}
		#region Properties
		protected Selection Selection { get { return DocumentModel.Selection; } }
		#endregion
		#region ISpellCheckTextController Members
		#endregion
		#region ISpellCheckTextControlController Members
		#region Properties
		public string EditControlText { get { return String.Empty; } set { } }
		public bool HasSelection { get { return DocumentModel.Selection.Length > 0; } }
		public bool IsReadOnly { get { return !Control.IsEditable; } }
		public Position SelectionStart { get { return new DocumentPosition(Selection.Interval.NormalizedStart.Clone()); } }
		public Position SelectionFinish { get { return new DocumentPosition(Selection.Interval.NormalizedEnd.Clone()); } }
		#endregion
		public void HideSelection() {
		}
		public bool IsSelectionVisible() { 
			return true;
		}
		public void ScrollToCaretPos() {
			Control.ActiveView.EnsureCaretVisible();
		}
		public void Select(Position start, Position finish) {
			Select((DocumentPosition)start, (DocumentPosition)finish);
		}
		protected internal virtual void Select(DocumentPosition start, DocumentPosition finish) {
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.Selection.Start = start.Position.LogPosition;
				DocumentModel.Selection.End = finish.Position.LogPosition;
			}
			finally {
				DocumentModel.EndUpdate();
			}
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Selection);
		}
		public void ShowSelection() {
		}
		public void UpdateText() {
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region ISupportMultiCulture Members
		#endregion
	}
	#endregion
	#region SentencePositionCalculator
	public class SentencePositionCalculator {
		SpellCheckerWordIterator wordIterator;
		List<char> simpleSentenceSeparators;
		List<char> specialSentenceSeparators;
		public SentencePositionCalculator(SpellCheckerWordIterator wordIterator) {
			this.wordIterator = wordIterator;
			this.simpleSentenceSeparators = new List<char>() { '.', '!', '?' };
			this.specialSentenceSeparators = new List<char>() { Characters.ParagraphMark, Characters.ColumnBreak, Characters.PageBreak, Characters.SectionMark, Characters.LineBreak, Characters.FloatingObjectMark };
		}
		public DocumentModelPosition GetSentenceEndPosition(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			while (!IsEndOfDocument(result, wordIterator)) {
				result = GetSentenceEndPositionCore(result);
				DocumentModelPosition start = FindPrevLetterOrDigitPosition(result);
				DocumentModelPosition finish = FindLetterOrDigitPosition(result);
				result = ProcessPunctuation(start, finish);
				if (!Char.IsLetter(GetCharacter(result))) {
					wordIterator.SkipForward(result, IsSimpleSentenceSeparator);
					break;
				}
			}
			return result;
		}
		DocumentModelPosition GetSentenceEndPositionCore(DocumentModelPosition pos) {
			if (IsEndOfDocument(pos, wordIterator))
				return pos;
			DocumentModelPosition result = wordIterator.MoveToWordEnd(pos);
			while (!IsEndOfDocument(result, wordIterator) && !IsSentenceSeparator(GetCharacter(result))) {
				wordIterator.SkipForward(result, IsNotWordSymbolAndNotSentenceSeparator);
				result = wordIterator.MoveToWordEnd(result);
			}
			return result;
		}
		public DocumentModelPosition GetSentenceStartPosition(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			while (!IsStartOfDocument(result, wordIterator)) {
				result = GetSentenceStartPositionCore(result);
				if (IsStartOfDocument(result, wordIterator))
					return result;
				DocumentModelPosition start = FindPrevLetterOrDigitPosition(result);
				DocumentModelPosition finish = FindLetterOrDigitPosition(result);
				if (!Char.IsLetter(GetCharacter(ProcessPunctuation(start, finish))))
					break;
				else
					result.CopyFrom(start);
			}
			return result;
		}
		DocumentModelPosition GetSentenceStartPositionCore(DocumentModelPosition pos) {
			if (IsStartOfDocument(pos, wordIterator))
				return pos;
			DocumentModelPosition result = wordIterator.MoveToWordStart(pos);
			while (!IsStartOfDocument(result, wordIterator)) {
				DocumentModelPosition sentenceStart = FindSentenceStart(result);
				if (sentenceStart != null)
					return sentenceStart;
				wordIterator.MoveToPrevChar(result);
				result = wordIterator.MoveToWordStart(result);
			}
			return result;
		}
		DocumentModelPosition FindSentenceStart(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			wordIterator.MoveToPrevChar(result);
			wordIterator.SkipBackward(result, IsNotWordSymbolAndNotSentenceSeparator);
			if (!IsSentenceSeparator(GetCharacter(result)))
				return null;
			wordIterator.SkipForward(result, ch => { return IsSentenceSeparator(ch) || IsSpace(ch); });
			return result;
		}
		DocumentModelPosition ProcessPunctuation(DocumentModelPosition start, DocumentModelPosition finish) {
			DocumentModelPosition currentPosition = FindAbbreviation(start, finish);
			if (currentPosition != null)
				return currentPosition;
			currentPosition = start.Clone();
			bool isNextWordStartWithUpperCase = Char.IsUpper(GetCharacter(finish));
			int sentenceSeparatorsCount = 0;
			while (currentPosition <= finish) {
				char ch = GetCharacter(currentPosition);
				if (IsSpecialSentenceSeparator(ch))
					return currentPosition;
				if (!IsSimpleSentenceSeparator(ch)) {
					sentenceSeparatorsCount = 0;
					wordIterator.MoveToNextChar(currentPosition);
					continue;
				}
				DocumentModelPosition spacePositionAfterThreeDots = FindThreeDotsWithSpace(currentPosition);
				if (spacePositionAfterThreeDots != null) {
					DocumentModelPosition separatorPosition = FindSentenceSeparatorAfterThreeDotsWithSpace(spacePositionAfterThreeDots);
					if (separatorPosition != null)
						return separatorPosition;
					DocumentModelPosition punctuationPosition = FindPunctuationAfterThreeDotsWithSpace(spacePositionAfterThreeDots);
					if (punctuationPosition != null) {
						sentenceSeparatorsCount = 0;
						currentPosition.CopyFrom(punctuationPosition);
						continue;
					}
					if (isNextWordStartWithUpperCase)
						return spacePositionAfterThreeDots;
				}
				DocumentModelPosition spacePositionAfterSimpleSentenceSeparator = FindSentenceSeparatorWithSpace(currentPosition);
				if (spacePositionAfterSimpleSentenceSeparator != null)
					return spacePositionAfterSimpleSentenceSeparator;
				bool isThreeDotsSkipped = SkipThreeDots(currentPosition);
				sentenceSeparatorsCount = isThreeDotsSkipped ? 0 : ++sentenceSeparatorsCount;
				if (sentenceSeparatorsCount > 1)
					return currentPosition;
				if (!isThreeDotsSkipped)
					wordIterator.MoveToNextChar(currentPosition);
			}
			return finish;
		}
		DocumentModelPosition FindSentenceSeparatorAfterThreeDotsWithSpace(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			wordIterator.SkipForward(result, IsSpace);
			return IsSentenceSeparator(GetCharacter(result)) ? result : null;
		}
		DocumentModelPosition FindPunctuationAfterThreeDotsWithSpace(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			wordIterator.SkipForward(result, IsSpace);
			return IsNotWordSymbolAndNotSentenceSeparator(GetCharacter(result)) ? result : null;
		}
		DocumentModelPosition FindAbbreviation(DocumentModelPosition start, DocumentModelPosition finish) {
			if (!Char.IsLetter(GetCharacter(start)))
				return null;
			if (!IsStartOfDocument(start, wordIterator) && PrevCharIsLetter(start))
				return null;
			if (!NextCharIsDot(start))
				return null;
			int sentenceSeparatorsCount = 0;
			DocumentModelPosition currentPosition = start.Clone();
			while (currentPosition != finish) {
				if (sentenceSeparatorsCount > 1)
					return null;
				wordIterator.MoveToNextChar(currentPosition);
				char ch = GetCharacter(currentPosition);
				if (IsSpecialSentenceSeparator(ch))
					return null;
				sentenceSeparatorsCount = IsSimpleSentenceSeparator(ch) ? ++sentenceSeparatorsCount : 0;
			}
			return finish;
		}
		bool PrevCharIsLetter(DocumentModelPosition pos) {
			DocumentModelPosition newPos = pos.Clone();
			wordIterator.MoveToPrevChar(newPos);
			return Char.IsLetter(GetCharacter(newPos));
		}
		bool NextCharIsDot(DocumentModelPosition pos) {
			DocumentModelPosition newPos = pos.Clone();
			wordIterator.MoveToNextChar(newPos);
			return IsDot(GetCharacter(newPos));
		}
		DocumentModelPosition FindLetterOrDigitPosition(DocumentModelPosition startPosition) {
			DocumentModelPosition result = startPosition.Clone();
			wordIterator.SkipForward(result, wordIterator.IsNotLetterOrDigit);
			return result;
		}
		DocumentModelPosition FindPrevLetterOrDigitPosition(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			if (Char.IsLetter(GetCharacter(pos)))
				wordIterator.MoveToPrevChar(result);
			wordIterator.SkipBackward(result, wordIterator.IsNotLetterOrDigit);
			return result;
		}
		DocumentModelPosition FindSentenceSeparatorWithSpace(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			if (!IsSimpleSentenceSeparator(GetCharacter(result)))
				return null;
			wordIterator.MoveToNextChar(result);
			return IsSpace(GetCharacter(result)) ? result : null;
		}
		DocumentModelPosition FindThreeDotsWithSpace(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			if (!SkipThreeDots(result))
				return null;
			return IsSpace(GetCharacter(result)) ? result : null;
		}
		bool SkipThreeDots(DocumentModelPosition pos) {
			if (!IsDot(GetCharacter(pos)))
				return false;
			DocumentModelPosition newPos = pos.Clone();
			int remainingDotsCount = 2;
			while (remainingDotsCount > 0 && !IsEndOfDocument(newPos, wordIterator)) {
				wordIterator.MoveToNextChar(newPos);
				if (!IsDot(GetCharacter(newPos)))
					return false;
				remainingDotsCount--;
			}
			wordIterator.MoveToNextChar(newPos);
			pos.CopyFrom(newPos);
			return true;
		}
		char GetCharacter(DocumentModelPosition pos) {
			return wordIterator.GetCharacter(pos);
		}
		bool IsNotWordSymbolAndNotSentenceSeparator(char ch) {
			return wordIterator.IsNotLetterOrDigit(ch) && !IsSentenceSeparator(ch);
		}
		bool IsDot(char ch) {
			return ch == Characters.Dot;
		}
		bool IsSpace(char ch) {
			return ch == Characters.Space || ch == Characters.NonBreakingSpace;
		}
		bool IsSpecialSentenceSeparator(char ch) {
			return this.specialSentenceSeparators.Contains(ch);
		}
		bool IsSimpleSentenceSeparator(char ch) {
			return simpleSentenceSeparators.Contains(ch);
		}
		bool IsSentenceSeparator(char ch) {
			return IsSimpleSentenceSeparator(ch) || IsSpecialSentenceSeparator(ch);
		}
		bool IsStartOfDocument(DocumentModelPosition pos, SpellCheckerWordIterator wordIterator) {
			return wordIterator.IsStartOfDocument(pos);
		}
		bool IsEndOfDocument(DocumentModelPosition pos, SpellCheckerWordIterator wordIterator) {
			return wordIterator.IsEndOfDocument(pos);
		}
	} 
	#endregion
}
