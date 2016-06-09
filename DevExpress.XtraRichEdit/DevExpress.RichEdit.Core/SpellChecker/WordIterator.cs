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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using System.Text;
namespace DevExpress.XtraRichEdit.SpellChecker {
	#region CharactersIterator
	public class CharactersIterator : PieceTableIterator {
		public CharactersIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			DocumentModelPosition.MoveForwardCore(pos);
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			DocumentModelPosition.MoveBackwardCore(pos);
		}
	}
	#endregion
	#region SpellCheckerWordIterator1
	public class SpellCheckerWordIterator1 : WordsDocumentModelIteratorBase {
		#region Static methods and members
		static List<char> WordsSeparators = CreateSeparatorsTable();
		static List<char> InWordSymbols = CreateInWordSymbolsTable();
		static List<char> CreateSeparatorsTable() {
			List<char> result = new List<char>();
			result.Add(' ');
			result.Add('\t');
			result.Add('\n');
			result.Add('\r');
			result.Add('[');
			result.Add(']');
			result.Add('<');
			result.Add('>');
			result.Add(Characters.ObjectMark);
			result.Add(Characters.NonBreakingSpace);
			result.Add(Characters.EnSpace);
			result.Add(Characters.EmSpace);
			result.Add(Characters.QmSpace);
			result.Add(Characters.LineBreak);
			result.Add(Characters.ColumnBreak);
			result.Add(Characters.SectionMark);
			result.Add(Characters.PageBreak);
			return result;
		}
		static List<char> CreateInWordSymbolsTable() {
			List<char> result = new List<char>();
			result.Add('\'');
			result.Add(',');
			result.Add('‘');
			result.Add('`');
			result.Add('_');
			return result;
		}
		#endregion
		readonly IVisibleTextFilter textFilter;
		public SpellCheckerWordIterator1(PieceTable pieceTable)
			: base(pieceTable) {
			this.textFilter = pieceTable.VisibleTextFilter;
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			PieceTableIterator iterator = new CharactersIterator(PieceTable);
			DocumentModelPosition posClone = pos.Clone();
			if (!IsLetterOrDigit(GetCharacter(posClone)))
				SkipForward(iterator, posClone, IsNotLetterOrDigit);
			if (IsEndOfDocument(posClone))
				return;
			pos.CopyFrom(posClone);
			MoveToWordEndCore(pos, iterator);
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			PieceTableIterator iterator = new CharactersIterator(PieceTable);
			if (!IsLetterOrDigit(GetCharacter(pos)))
				SkipBackward(iterator, pos, IsNotLetterOrDigit);
			SkipBackward(iterator, pos, IsNotWordsSeparators);
			SkipBackward(iterator, pos, IsNotLetterOrDigit);
			if (!IsStartOfDocument(pos))
				iterator.MoveForwardCore(pos);
		}
		protected internal virtual void MoveToWordEndCore(DocumentModelPosition pos, PieceTableIterator iterator) {
			SkipVisibleForward(iterator, pos, IsLetterOrDigit);
			if ((IsWordsSeparators(GetCharacter(pos)) && textFilter.IsRunVisible(pos.RunIndex)) || IsEndOfDocument(pos))
				return;
			DocumentModelPosition posClone = pos.Clone();
			SkipForward(iterator, posClone, IsSymbols);
			if (IsLetterOrDigit(GetCharacter(posClone))) {
				pos.CopyFrom(posClone);
				MoveToWordEndCore(pos, iterator);
			}
		}
		protected internal virtual void MoveToWordStartCore(DocumentModelPosition pos, PieceTableIterator iterator) {
			SkipVisibleBackward(iterator, pos, IsLetterOrDigit);
			if (IsWordsSeparators(GetCharacter(pos)) && textFilter.IsRunVisible(pos.RunIndex)) {
				iterator.MoveForwardCore(pos);
				return;
			}
			if (IsStartOfDocument(pos))
				return;
			DocumentModelPosition posClone = pos.Clone();
			SkipBackward(iterator, posClone, IsSymbols);
			if (IsLetterOrDigit(GetCharacter(posClone))) {
				pos.CopyFrom(posClone);
				MoveToWordStartCore(pos, iterator);
			}
			else
				iterator.MoveForwardCore(pos);
		}
		public virtual DocumentModelPosition MoveToNextWordStart(DocumentModelPosition endWordPos) {
			DocumentModelPosition result = endWordPos.Clone();
			if (IsEndOfDocument(result))
				return result;
			PieceTableIterator iterator = new CharactersIterator(PieceTable);
			SkipForward(iterator, result, IsNotWordsSeparators);
			SkipForward(iterator, result, IsNotLetterOrDigit);
			return result;
		}
		public virtual DocumentModelPosition MoveToWordStart(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			if (IsStartOfDocument(result))
				return result;
			PieceTableIterator iterator = new CharactersIterator(PieceTable);
			if (!IsLetterOrDigit(GetCharacter(result)))
				SkipBackward(iterator, result, IsNotLetterOrDigit);
			MoveToWordStartCore(result, iterator);
			return result;
		}
		public virtual DocumentModelPosition ValidateWordStartPosition(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			PieceTableIterator iterator = new CharactersIterator(PieceTable);
			if (!textFilter.IsRunVisible(result.RunIndex))
				SkipUnvisibleForward(iterator, result);
			if (!IsLetterOrDigit(GetCharacter(result))) {
				SkipForward(iterator, result, IsNotLetterOrDigit);
				return result;
			}
			if (IsStartOfDocument(result))
				return result;
			else
				return MoveToWordStart(result);
		}
		void SkipUnvisibleForward(PieceTableIterator iterator, DocumentModelPosition pos) {
			while (!textFilter.IsRunVisible(pos.RunIndex)) {
				iterator.MoveForwardCore(pos);
			}
		}
		public virtual DocumentModelPosition MoveToWordEnd(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			if (IsEndOfDocument(result))
				return result;
			PieceTableIterator iterator = new CharactersIterator(PieceTable);
			MoveToWordEndCore(result, iterator);
			if (result > pos)
				iterator.MoveBackCore(result);
			return result;
		}
		protected internal override void SkipForward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsEndOfDocument(pos) && (predicate(GetCharacter(pos)) || !textFilter.IsRunVisible(pos.RunIndex)))
				iterator.MoveForwardCore(pos);
		}
		protected internal override void SkipBackward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsStartOfDocument(pos) && (predicate(GetCharacter(pos)) || !textFilter.IsRunVisible(pos.RunIndex)))
				iterator.MoveBackCore(pos);
		}
		protected internal virtual void SkipVisibleForward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsEndOfDocument(pos) && predicate(GetCharacter(pos)) && textFilter.IsRunVisible(pos.RunIndex))
				iterator.MoveForwardCore(pos);
		}
		protected internal virtual void SkipVisibleBackward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsStartOfDocument(pos) && predicate(GetCharacter(pos)) && textFilter.IsRunVisible(pos.RunIndex))
				iterator.MoveBackCore(pos);
		}
		protected internal virtual bool IsSymbols(char ch) {
			return IsNotWordsSeparators(ch) && !IsLetterOrDigit(ch);
		}
		protected internal virtual bool IsNotWordsSeparators(char ch) {
			return !IsWordsSeparators(ch);
		}
		protected internal virtual bool IsWordsSeparators(char ch) {
			return WordsSeparators.Contains(ch);
		}
		protected internal virtual bool IsNotLetterOrDigit(char ch) {
			return !IsLetterOrDigit(ch);
		}
		protected internal virtual bool IsLetterOrDigit(char ch) {
			return Char.IsLetterOrDigit(ch);
		}
		protected internal virtual bool IsInWordSymbols(char ch) {
			return InWordSymbols.Contains(ch);
		}
	}
	#endregion
	#region VisibleCharactersIterator
	public class VisibleCharactersIterator : PieceTableIterator {
		RunIndex cachedRunIndex = new RunIndex(-1);
		bool isCachedRunVisible;
		public VisibleCharactersIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected IVisibleTextFilter VisibleTextFilter { get { return PieceTable.VisibleTextFilter; } }
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			DocumentModelPosition.MoveForwardCore(pos);
			if (!IsRunVisible(pos.RunIndex)) {
				pos.LogPosition = VisibleTextFilter.GetNextVisibleLogPosition(pos, false);
				UpdateModelPositionByLogPosition(pos);
			}
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			DocumentModelPosition.MoveBackwardCore(pos);
			if (!IsRunVisible(pos.RunIndex)) {
				pos.LogPosition = VisibleTextFilter.GetPrevVisibleLogPosition(pos, false);
				UpdateModelPositionByLogPosition(pos);
			}
		}
		protected internal virtual bool IsRunVisible(RunIndex runIndex) {
			if (cachedRunIndex != runIndex) {
				cachedRunIndex = runIndex;
				isCachedRunVisible = VisibleTextFilter.IsRunVisible(runIndex);
			}
			return isCachedRunVisible;
		}
	}
	#endregion
	#region SpellCheckerWordIterator
	public class SpellCheckerWordIterator : PieceTableIterator {
		#region Static methods and members
		static List<char> WordsSeparators = CreateSeparatorsTable();
		static List<char> InWordSymbols = CreateInWordSymbolsTable();
		static List<char> CreateSeparatorsTable() {
			List<char> result = new List<char>();
			result.Add(' ');
			result.Add('\t');
			result.Add('\n');
			result.Add('\r');
			result.Add('(');
			result.Add(')');
			result.Add('[');
			result.Add(']');
			result.Add('{');
			result.Add('}');
			result.Add('<');
			result.Add('>');
			result.Add('\uFFFC');
			result.Add('\u00A0');
			result.Add('\u2002');
			result.Add('\u2003');
			result.Add('\u2005');
			result.Add('\u000B');
			result.Add('\u000E');
			result.Add((char)0x1D);
			result.Add('\u000C');
			return result;
		}
		static List<char> CreateInWordSymbolsTable() {
			List<char> result = new List<char>();
			result.Add('\'');
			result.Add(',');
			result.Add('‘');
			result.Add('`');
			result.Add('_');
			return result;
		}
		#endregion
		RunIndex cachedRunIndex = new RunIndex(-1);
		string cachedRunText;
		public SpellCheckerWordIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected IVisibleTextFilter VisibleTextFilter { get { return PieceTable.VisibleTextFilter; } }
		protected internal void SkipForward(DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsEndOfDocument(pos) && predicate(GetCharacter(pos)))
				MoveToNextChar(pos);
		}
		internal void MoveToNextChar(DocumentModelPosition pos) {
			pos.LogPosition++;
			if (pos.LogPosition <= pos.RunEndLogPosition)
				return;
			MoveToNextRun(pos, pos.LogPosition);
			TextRunCollection runs = PieceTable.Runs;
			RunIndex endRunIndex = new RunIndex(runs.Count - 1);
			while (pos.RunIndex < endRunIndex && !VisibleTextFilter.IsRunVisible(pos.RunIndex)) {
				pos.LogPosition += runs[pos.RunIndex].Length;
				MoveToNextRun(pos, pos.LogPosition);
			}
		}
		void MoveToNextRun(DocumentModelPosition pos, DocumentLogPosition runStartPos) {
			pos.RunIndex++;
			pos.ParagraphIndex = PieceTable.Runs[pos.RunIndex].Paragraph.Index;
			pos.RunStartLogPosition = runStartPos;
		}
		protected internal void SkipBackward(DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsStartOfDocument(pos) && predicate(GetCharacter(pos)))
				MoveToPrevChar(pos);
		}
		internal void MoveToPrevChar(DocumentModelPosition pos) {
			pos.LogPosition--;
			if (pos.LogPosition >= pos.RunStartLogPosition)
				return;
			TextRunCollection runs = PieceTable.Runs;
			MoveToPrevRun(pos, pos.LogPosition);
			RunIndex firstRunIndex = new RunIndex(0);
			while (pos.RunIndex > firstRunIndex && !VisibleTextFilter.IsRunVisible(pos.RunIndex)) {
				pos.LogPosition = pos.RunEndLogPosition - runs[pos.RunIndex].Length;
				MoveToPrevRun(pos, pos.LogPosition);
			}
		}
		void MoveToPrevRun(DocumentModelPosition pos, DocumentLogPosition runEndPos) {
			pos.RunIndex--;
			TextRunBase run = PieceTable.Runs[pos.RunIndex];
			pos.ParagraphIndex = run.Paragraph.Index;
			pos.RunStartLogPosition = runEndPos - run.Length + 1;
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			DocumentModelPosition posClone = pos.Clone();
			if (!IsLetterOrDigit(GetCharacter(posClone)))
				SkipForward(posClone, IsNotLetterOrDigit);
			if (IsEndOfDocument(posClone))
				return;
			pos.CopyFrom(posClone);
			MoveToWordEndCore(pos);
		}
		internal char GetCharacter(DocumentModelPosition pos) {
			if (pos.RunIndex != this.cachedRunIndex) {
				this.cachedRunIndex = pos.RunIndex;
				this.cachedRunText = PieceTable.GetRunNonEmptyText(cachedRunIndex);
			}
			return this.cachedRunText[pos.RunOffset];
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			if (!IsLetterOrDigit(GetCharacter(pos)))
				SkipBackward(pos, IsNotLetterOrDigit);
			SkipBackward(pos, IsNotWordsSeparators);
			SkipBackward(pos, IsNotLetterOrDigit);
			if (IsStartOfDocument(pos))
				return;
			MoveToWordStartCore(pos);
			MoveToWordEndCore(pos);
		}
		public DocumentModelPosition MoveToWordEnd(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			if (IsEndOfDocument(result))
				return result;
			MoveToWordEndCore(result);
			return result;
		}
		protected internal void MoveToWordEndCore(DocumentModelPosition pos) {
			StringBuilder word = new StringBuilder();
			while (true) {
				bool isPrevCharNotLetterOrDigit = word.Length > 0 && !IsLetterOrDigit(word[word.Length - 1]);
				if (IsEndOfDocument(pos) || IsWordSeparator(pos) || (!IsLetterOrDigit(pos) && isPrevCharNotLetterOrDigit)) {
					if (isPrevCharNotLetterOrDigit && !IsAbbreviation(word))
						MoveToPrevChar(pos);
					return;
				}
				word.Append(GetCharacter(pos));
				MoveToNextChar(pos);
			}
		}
		bool IsLetterOrDigit(DocumentModelPosition pos) {
			return IsLetterOrDigit(GetCharacter(pos));
		}
		bool IsWordSeparator(DocumentModelPosition pos) {
			return IsWordSeparator(GetCharacter(pos));
		}
		bool IsAbbreviation(StringBuilder word) {
			int length = word.Length;
			if (length != 4)
				return false;
			bool result = true;
			for (int i = 0; i < length; i++) {
				char ch = word[i];
				switch (i) {
					case 0:
					case 2:
						result &= Char.IsLetter(ch);
						break;
					case 1:
					case 3:
						result &= ch == '.';
						break;
				}
			}
			return result;
		}
		public DocumentModelPosition MoveToWordStart(DocumentModelPosition pos) {
			return MoveToWordStart(pos, true);
		}
		protected internal virtual DocumentModelPosition MoveToWordStart(DocumentModelPosition pos, bool skipNotLetterOrDigitChar) {
			if (IsStartOfDocument(pos))
				return pos;
			DocumentModelPosition result = pos.Clone();
			if (!IsLetterOrDigit(GetCharacter(result))) {
				if (!skipNotLetterOrDigitChar)
					return result;
				SkipBackward(result, IsNotLetterOrDigit);
			}
			MoveToWordStartCore(result);
			return result;
		}
		void MoveToWordStartCore(DocumentModelPosition pos) {
			SkipBackward(pos, IsLetterOrDigit);
			if (IsWordSeparator(GetCharacter(pos))) {
				MoveToNextChar(pos);
				return;
			}
			if (IsStartOfDocument(pos))
				return;
			DocumentModelPosition newPos = pos.Clone();
			MoveToPrevChar(newPos);
			if (!IsLetterOrDigit(GetCharacter(newPos))) {
				MoveToNextChar(pos);
				return;
			}
			pos.CopyFrom(newPos);
			MoveToWordStartCore(pos);
		}
		public virtual void MoveToNextWordEnd(DocumentModelPosition pos) {
			if (IsEndOfDocument(pos))
				return;
			if (!IsLetterOrDigit(GetCharacter(pos)))
				SkipForward(pos, IsNotLetterOrDigit);
			SkipForward(pos, IsNotWordsSeparators);
			SkipForward(pos, IsNotLetterOrDigit);
			MoveToWordEndCore(pos);
		}
		public virtual void MoveToPrevWordStart(DocumentModelPosition pos) {
			if (IsStartOfDocument(pos))
				return;
			if (!IsLetterOrDigit(GetCharacter(pos)))
				SkipBackward(pos, IsNotLetterOrDigit);
			SkipBackward(pos, IsNotWordsSeparators);
			SkipBackward(pos, IsNotLetterOrDigit);
			MoveToWordStartCore(pos);
		}
		protected internal virtual bool IsNotWordsSeparators(char ch) {
			return !IsWordSeparator(ch);
		}
		protected internal virtual bool IsWordSeparator(char ch) {
			return WordsSeparators.Contains(ch);
		}
		protected internal virtual bool IsNotLetterOrDigit(char ch) {
			return !IsLetterOrDigit(ch);
		}
		protected internal virtual bool IsLetterOrDigit(char ch) {
			return Char.IsLetterOrDigit(ch);
		}
	}
	#endregion
	#region VisibleCharactersIteratorEx
	public class VisibleCharactersIteratorEx : PieceTableIterator {
		RunIndex cachedRunIndex = new RunIndex(-1);
		bool isCachedRunVisible;
		public VisibleCharactersIteratorEx(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected IVisibleTextFilter VisibleTextFilter { get { return PieceTable.VisibleTextFilter; } }
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			DocumentModelPosition.MoveForwardCore(pos);
			while (!IsRunVisible(pos.RunIndex)) {
				TextRunBase run = PieceTable.Runs[pos.RunIndex];
				pos.RunIndex++;
				pos.ParagraphIndex = PieceTable.Runs[pos.RunIndex].Paragraph.Index;
				pos.RunStartLogPosition += run.Length;
				pos.LogPosition = pos.RunStartLogPosition;
			}
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			DocumentModelPosition.MoveBackwardCore(pos);
			while (!IsRunVisible(pos.RunIndex)) {
				TextRunBase run = PieceTable.Runs[pos.RunIndex];
				pos.LogPosition = pos.RunEndLogPosition - run.Length;
				pos.RunIndex--;
				pos.ParagraphIndex = PieceTable.Runs[pos.RunIndex].Paragraph.Index;
				pos.RunStartLogPosition = pos.LogPosition - PieceTable.Runs[pos.RunIndex].Length + 1;
			}
		}
		protected internal virtual bool IsRunVisible(RunIndex runIndex) {
			if (cachedRunIndex != runIndex) {
				cachedRunIndex = runIndex;
				isCachedRunVisible = VisibleTextFilter.IsRunVisible(runIndex);
			}
			return isCachedRunVisible;
		}
	}
	#endregion
}
