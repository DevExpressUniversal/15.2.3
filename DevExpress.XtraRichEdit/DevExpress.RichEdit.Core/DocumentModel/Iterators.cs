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
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Model {
	#region PieceTableIterator (abstract class)
	public abstract class PieceTableIterator {
		readonly PieceTable pieceTable;
		protected PieceTableIterator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		public virtual DocumentModelPosition MoveForward(DocumentModelPosition pos) {
			Guard.ArgumentNotNull(pos, "pos");
			DocumentModelPosition result = pos.Clone();
			if (IsEndOfDocument(result))
				return result;
			MoveForwardCore(result);
			return result;
		}
		public virtual DocumentModelPosition MoveBack(DocumentModelPosition pos) {
			Guard.ArgumentNotNull(pos, "pos");
			DocumentModelPosition result = pos.Clone();
			if (IsStartOfDocument(result))
				return result;
			MoveBackCore(result);
			return result;
		}
		protected internal abstract void MoveForwardCore(DocumentModelPosition pos);
		protected internal abstract void MoveBackCore(DocumentModelPosition pos);
		public virtual bool IsNewElement(DocumentModelPosition currentPosition) {
			Guard.ArgumentNotNull(currentPosition, "currentPosition");
			DocumentModelPosition pos = currentPosition.Clone();
			pos = MoveBack(pos);
			pos = MoveForward(pos);
			return pos == currentPosition;
		}
		protected bool IsInsideDocument(DocumentModelPosition currentPosition) {
			DocumentLogPosition logPosition = currentPosition.LogPosition;
			return logPosition >= PieceTable.DocumentStartLogPosition && logPosition <= PieceTable.DocumentEndLogPosition;
		}
		protected internal bool IsEndOfDocument(DocumentModelPosition currentPosition) {
			return currentPosition.LogPosition >= PieceTable.DocumentEndLogPosition;
		}
		protected internal bool IsStartOfDocument(DocumentModelPosition currentPosition) {
			return currentPosition.LogPosition == PieceTable.DocumentStartLogPosition;
		}
		protected void UpdateModelPositionByLogPosition(DocumentModelPosition currentPosition) {
			Paragraph paragraph = PieceTable.Paragraphs[currentPosition.ParagraphIndex];
			DocumentLogPosition logPosition = currentPosition.LogPosition;
			if (logPosition >= currentPosition.RunStartLogPosition && logPosition < currentPosition.RunEndLogPosition)
				return;
			while (logPosition > paragraph.EndLogPosition || logPosition < paragraph.LogPosition) {
				if (logPosition < paragraph.LogPosition)
					currentPosition.ParagraphIndex--;
				else
					currentPosition.ParagraphIndex++;
				paragraph = PieceTable.Paragraphs[currentPosition.ParagraphIndex];
			}
			currentPosition.RunIndex = paragraph.FirstRunIndex;
			DocumentLogPosition runStart = paragraph.LogPosition;
			while (logPosition >= runStart + PieceTable.Runs[currentPosition.RunIndex].Length) {
				runStart = runStart + PieceTable.Runs[currentPosition.RunIndex].Length;
				currentPosition.RunIndex++;
			}
			currentPosition.RunStartLogPosition = runStart;
		}
	}
	#endregion
	#region CharactersDocumentModelIterator
	public class CharactersDocumentModelIterator : PieceTableIterator {
		public CharactersDocumentModelIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			SkipForward(pos);
			UpdateModelPositionByLogPosition(pos);
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			SkipBackward(pos);
			UpdateModelPositionByLogPosition(pos);
		}
		protected internal virtual void SkipForward(DocumentModelPosition pos) {
			pos.LogPosition++;
		}
		protected internal virtual void SkipBackward(DocumentModelPosition pos) {
			pos.LogPosition--;
		}
	}
	#endregion
	#region VisibleCharactersStopAtFieldsDocumentModelIterator
	public class VisibleCharactersStopAtFieldsDocumentModelIterator : CharactersDocumentModelIterator {
		readonly IVisibleTextFilter filter;
		public VisibleCharactersStopAtFieldsDocumentModelIterator(PieceTable pieceTable)
			: base(pieceTable) {
			this.filter = pieceTable.VisibleTextFilter;
		}
		protected internal virtual bool IsFieldDelimiter(DocumentModelPosition pos) {
			TextRunBase run = pos.PieceTable.Runs[pos.RunIndex];
			return run is FieldCodeRunBase || run is FieldResultEndRun;
		}
		protected internal override void SkipForward(DocumentModelPosition pos) {
			if (IsFieldDelimiter(pos))
				return;
			do {
				base.SkipForward(pos);
				if (IsFieldDelimiter(pos)) {
					base.SkipBackward(pos);
					return;
				}
			}
			while (!IsEndOfDocument(pos) && (!filter.IsRunVisible(pos.RunIndex)));
		}
		protected internal override void SkipBackward(DocumentModelPosition pos) {
			if (IsFieldDelimiter(pos))
				return;
			do {
				base.SkipBackward(pos);
				if (IsFieldDelimiter(pos)) {
					base.SkipForward(pos);
					return;
				}
			}
			while (!IsStartOfDocument(pos) && (!filter.IsRunVisible(pos.RunIndex)));
		}
	}
	#endregion
	#region WordsDocumentModelIteratorBase (abstract class)
	public abstract class WordsDocumentModelIteratorBase : PieceTableIterator {
		RunIndex cachedRunIndex = new RunIndex(-1);
		string cachedRunText;
		protected WordsDocumentModelIteratorBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected virtual internal char GetCharacter(DocumentModelPosition pos) {
			if (pos.RunIndex != cachedRunIndex) {
				cachedRunIndex = pos.RunIndex;
				cachedRunText = PieceTable.GetRunNonEmptyText(cachedRunIndex);
			}
			int index = Algorithms.Min(pos.LogPosition, pos.PieceTable.DocumentEndLogPosition) - pos.RunStartLogPosition;
			if (index == cachedRunText.Length) {
				if (index > 0)
					index--;
			}
			char result = cachedRunText[index];
			return result;
		}
		protected internal virtual void SkipForward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsEndOfDocument(pos) && predicate(GetCharacter(pos)))
				iterator.MoveForwardCore(pos);
		}
		protected internal virtual void SkipBackward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsStartOfDocument(pos) && predicate(GetCharacter(pos)))
				iterator.MoveBackCore(pos);
		}
	}
	#endregion
	#region WordsDocumentModelIterator
	public class WordsDocumentModelIterator : WordsDocumentModelIteratorBase {
		#region Fields
		static readonly List<char> Spaces = CreateSpacesList();
		static readonly List<char> SpecialSymbols = CreateSpecialSymbolsList();
		static readonly List<char> WordsDelimiter = CreateWordDelimiters();
		static readonly List<char> NonWordSymbols = CreateNonWordSymbols();
		#endregion
		static List<char> CreateSpacesList() {
			List<char> result = new List<char>();
			result.Add(' ');
			result.Add(Characters.NonBreakingSpace);
			result.Add(Characters.EnSpace);
			result.Add(Characters.EmSpace);
			result.Add(Characters.QmSpace);
			return result;
		}
		static List<char> CreateSpecialSymbolsList() {
			List<char> result = new List<char>();
			result.Add(Characters.PageBreak);
			result.Add(Characters.LineBreak);
			result.Add(Characters.ColumnBreak);
			result.Add(Characters.SectionMark);
			result.Add(Characters.ParagraphMark);
			result.Add(Characters.TabMark);
			return result;
		}
		static List<char> CreateWordDelimiters() {
			List<char> result = new List<char>();
			result.Add(Characters.Dot);
			result.Add(',');
			result.Add('!');
			result.Add('@');
			result.Add('#');
			result.Add('$');
			result.Add('%');
			result.Add('^');
			result.Add('&');
			result.Add('*');
			result.Add('(');
			result.Add(')');
			result.Add(Characters.Dash);
			result.Add(Characters.EmDash);
			result.Add(Characters.EnDash);
			result.Add(Characters.Hyphen);
			result.Add(Characters.Underscore);
			result.Add('=');
			result.Add('+');
			result.Add('[');
			result.Add(']');
			result.Add('{');
			result.Add('}');
			result.Add('\\');
			result.Add('|');
			result.Add(';');
			result.Add(':');
			result.Add('\'');
			result.Add('"');
			result.Add('<');
			result.Add('>');
			result.Add('/');
			result.Add('?');
			result.Add('`');
			result.Add('~');
			result.Add(Characters.TrademarkSymbol);
			result.Add(Characters.CopyrightSymbol);
			result.Add(Characters.RegisteredTrademarkSymbol);
			result.Add(Characters.Ellipsis);
			result.Add(Characters.LeftDoubleQuote);
			result.Add(Characters.LeftSingleQuote);
			result.Add(Characters.RightDoubleQuote);
			result.Add(Characters.OpeningDoubleQuotationMark);
			result.Add(Characters.OpeningSingleQuotationMark);
			result.Add(Characters.ClosingDoubleQuotationMark);
			result.Add(Characters.ClosingSingleQuotationMark);
			return result;
		}
		static List<char> CreateNonWordSymbols() {
			List<char> result = new List<char>();
			result.AddRange(CreateSpacesList());
			result.AddRange(CreateSpecialSymbolsList());
			result.AddRange(CreateWordDelimiters());
			return result;
		}
		public WordsDocumentModelIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public bool IsAtWord(DocumentModelPosition currentPosition) {
			Guard.ArgumentNotNull(currentPosition, "currentPosition");
			return !NonWordSymbols.Contains(GetCharacter(currentPosition));
		}
		public virtual bool IsInsideWord(DocumentModelPosition currentPosition) {
			Guard.ArgumentNotNull(currentPosition, "currentPosition");
			return !NonWordSymbols.Contains(GetCharacter(currentPosition)) && !IsNewElement(currentPosition);
		}
		protected internal virtual bool IsWordsDelimiter(char ch) {
			return WordsDelimiter.Contains(ch);
		}
		protected internal virtual bool IsNotNonWordsSymbols(char ch) {
			return !NonWordSymbols.Contains(ch);
		}
		protected internal virtual bool IsSpace(char ch) {
			return Spaces.Contains(ch);
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			CharactersDocumentModelIterator iterator = new CharactersDocumentModelIterator(PieceTable);
			if (SpecialSymbols.Contains(GetCharacter(pos))) {
				iterator.MoveForwardCore(pos);
				return;
			}
			if (WordsDelimiter.Contains(GetCharacter(pos)))
				SkipForward(iterator, pos, IsWordsDelimiter);
			else
				SkipForward(iterator, pos, IsNotNonWordsSymbols);
			SkipForward(iterator, pos, IsSpace);
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			CharactersDocumentModelIterator iterator = new CharactersDocumentModelIterator(PieceTable);
			iterator.MoveBackCore(pos);
			if (SpecialSymbols.Contains(GetCharacter(pos)))
				return;
			SkipBackward(iterator, pos, IsSpace);
			if (SpecialSymbols.Contains(GetCharacter(pos))) {
				if (!IsStartOfDocument(pos))
					iterator.MoveForwardCore(pos);
				return;
			}
			if (WordsDelimiter.Contains(GetCharacter(pos))) {
				SkipBackward(iterator, pos, IsWordsDelimiter);
				if (!(IsStartOfDocument(pos) && WordsDelimiter.Contains(GetCharacter(pos))))
					iterator.MoveForwardCore(pos);
			}
			else {
				SkipBackward(iterator, pos, IsNotNonWordsSymbols);
				if (!(IsStartOfDocument(pos) && !NonWordSymbols.Contains(GetCharacter(pos))))
					iterator.MoveForwardCore(pos);
			}
		}
	}
	#endregion
	#region VisibleWordsIterator
	public class VisibleWordsIterator : WordsDocumentModelIterator {
		readonly IVisibleTextFilter filter;
		public VisibleWordsIterator(PieceTable pieceTable)
			: base(pieceTable) {
			this.filter = pieceTable.VisibleTextFilter;
		}
		protected internal override void SkipForward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsEndOfDocument(pos) && (predicate(GetCharacter(pos)) || !filter.IsRunVisible(pos.RunIndex)))
				iterator.MoveForwardCore(pos);
		}
		protected internal override void SkipBackward(PieceTableIterator iterator, DocumentModelPosition pos, Predicate<char> predicate) {
			while (!IsStartOfDocument(pos) && (predicate(GetCharacter(pos)) || !filter.IsRunVisible(pos.RunIndex)))
				iterator.MoveBackCore(pos);
		}
	}
	#endregion
	#region ParagraphsDocumentModelIterator
	public class ParagraphsDocumentModelIterator : PieceTableIterator {
		public ParagraphsDocumentModelIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			ParagraphIndex lastParagraphIndex = PieceTable.Paragraphs.Last.Index;
			if (pos.ParagraphIndex > lastParagraphIndex)
				return;
			pos.ParagraphIndex++;
			if (pos.ParagraphIndex <= lastParagraphIndex) {
				Paragraph paragraph = PieceTable.Paragraphs[pos.ParagraphIndex];
				pos.LogPosition = paragraph.LogPosition;
				pos.RunIndex = paragraph.FirstRunIndex;
			}
			else {
				Paragraph lastParagraph = PieceTable.Paragraphs.Last;
				pos.LogPosition = lastParagraph.EndLogPosition + 1;
				pos.RunIndex = lastParagraph.LastRunIndex + 1;
			}
			pos.RunStartLogPosition = pos.LogPosition;
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			Paragraph paragraph = PieceTable.Paragraphs[pos.ParagraphIndex];
			if (pos.LogPosition == paragraph.LogPosition && pos.ParagraphIndex > ParagraphIndex.Zero)
				pos.ParagraphIndex--;
			paragraph = PieceTable.Paragraphs[pos.ParagraphIndex];
			pos.LogPosition = paragraph.LogPosition;
			pos.RunStartLogPosition = pos.LogPosition;
			pos.RunIndex = paragraph.FirstRunIndex;
		}
	}
	#endregion
	#region DocumentsDocumentModelIterator
	public class DocumentsDocumentModelIterator : PieceTableIterator {
		public DocumentsDocumentModelIterator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void MoveForwardCore(DocumentModelPosition pos) {
			pos.ParagraphIndex = PieceTable.Paragraphs.Last.Index;
			Paragraph paragraph = PieceTable.Paragraphs[pos.ParagraphIndex];
			pos.LogPosition = PieceTable.DocumentEndLogPosition + 1;
			pos.RunIndex = paragraph.LastRunIndex;
			pos.RunStartLogPosition = PieceTable.DocumentEndLogPosition + 1;
		}
		protected internal override void MoveBackCore(DocumentModelPosition pos) {
			pos.LogPosition = DocumentLogPosition.Zero;
			pos.ParagraphIndex = ParagraphIndex.Zero;
			pos.RunIndex = RunIndex.Zero;
			pos.RunStartLogPosition = DocumentLogPosition.Zero;
		}
	}
	#endregion
}
