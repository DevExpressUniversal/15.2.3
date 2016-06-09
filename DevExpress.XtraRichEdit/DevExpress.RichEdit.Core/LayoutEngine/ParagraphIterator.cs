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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Services;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region ParagraphIteratorBase (abstract class)
	public abstract class ParagraphIteratorBase {
		#region Fields
		readonly PieceTable pieceTable;
		readonly Paragraph paragraph;
		int maxOffset;
		int runStartIndex;
		readonly RunIndex endRunIndex;
		FormatterPosition position;
		readonly IVisibleTextFilter visibleTextFilter;
		#endregion
		protected ParagraphIteratorBase(Paragraph paragraph, PieceTable pieceTable, IVisibleTextFilter visibleTextFilter) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(visibleTextFilter, "visibleTextFilter");
			this.paragraph = paragraph;
			this.pieceTable = pieceTable;
			this.visibleTextFilter = visibleTextFilter;
			this.endRunIndex = visibleTextFilter.FindVisibleParagraphRunForward(paragraph.LastRunIndex);
			this.position = CreatePosition();
			SetRunIndexCore(visibleTextFilter.FindVisibleRunForward(paragraph.FirstRunIndex));
		}	  
		#region Properties
		protected PieceTable Table { get { return pieceTable; } }
		protected internal IVisibleTextFilter VisibleTextFilter { get { return visibleTextFilter; } }
		public RunIndex RunIndex { get { return position.RunIndex; } }
		public int Offset { get { return position.Offset; } }
		public virtual bool IsEnd { get { return this.RunIndex >= endRunIndex; } }
		public virtual char CurrentChar {
			get {
#if DEBUGTEST
				Debug.Assert(Offset <= maxOffset);
#endif
				return pieceTable.TextBuffer[runStartIndex + Offset];
			}
		}
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		protected internal Paragraph Paragraph { get { return paragraph; } }
		#endregion
		protected internal abstract FormatterPosition CreatePosition();
		protected internal virtual bool IsParagraphMarkRun() {
			return PieceTable.Runs[RunIndex] is ParagraphRun;
		}
		protected internal virtual bool IsFloatingObjectAnchorRun() {
			return PieceTable.Runs[RunIndex] is FloatingObjectAnchorRun;
		}
		protected internal virtual bool IsInlinePictureRun() {
			return PieceTable.Runs[RunIndex] is InlinePictureRun;
		}
		protected internal virtual bool IsParagraphFrame() {
			return PieceTable.Runs[RunIndex].Paragraph.GetMergedFrameProperties() != null;
		}
		void SetRunIndexCore(RunIndex runIndex) {
			position.SetRunIndex(runIndex);
			TextRunBase run = pieceTable.Runs[RunIndex];
			maxOffset = run.Length - 1;
			runStartIndex = run.StartIndex;
		}
		public virtual void SetPosition(FormatterPosition pos) {
#if DEBUGTEST
			Debug.Assert(VisibleTextFilter.IsRunVisible(pos.RunIndex));
#endif
			SetRunIndexCore(pos.RunIndex);
			position.SetOffset(pos.Offset);
		}
		protected internal virtual void SetPositionCore(FormatterPosition pos) {
			SetRunIndexCore(pos.RunIndex);
			position.SetOffset(pos.Offset);
		}
		protected virtual void SetPosition(RunIndex runIndex, int offset) {
			SetRunIndexCore(runIndex);
			position.SetOffset(offset);
		}
		public virtual ParagraphIteratorResult Next() {
			if (Offset < maxOffset) {
				NextOffset();
				return ParagraphIteratorResult.Success;
			}
			while (!IsEnd) {
				NextRun();
				if (VisibleTextFilter.IsRunVisible(RunIndex))
					return IsEnd ? ParagraphIteratorResult.Finished : ParagraphIteratorResult.RunFinished;
			}
			return ParagraphIteratorResult.Finished;
		}
		protected virtual void NextOffset() {
			position.SetOffset(position.Offset + 1);
		}
		protected virtual void NextRun() {
			SetRunIndexCore(RunIndex + 1);
			position.SetOffset(0);
		}
		public virtual FormatterPosition GetCurrentPosition() {
			return new FormatterPosition(this.RunIndex, this.Offset, 0);
		}
		public virtual FormatterPosition GetPreviousPosition() {
			if (this.Offset == 0)
				return GetPreviousVisibleRunPosition();
			else
				return GetPreviousOffsetPosition();
		}
		protected virtual FormatterPosition GetPreviousVisibleRunPosition() {
			RunIndex runIndex = VisibleTextFilter.GetPrevVisibleRunIndex(this.RunIndex);
			return new FormatterPosition(runIndex, pieceTable.Runs[runIndex].Length - 1, 0);
		}
		protected virtual FormatterPosition GetPreviousOffsetPosition() {
			return new FormatterPosition(this.RunIndex, this.Offset - 1, 0);
		}
	}
	#endregion
	#region ParagraphCharacterIterator
	public class ParagraphCharacterIterator : ParagraphIteratorBase {
		public ParagraphCharacterIterator(Paragraph paragraph, PieceTable table, IVisibleTextFilter visibleTextFilter)
			: base(paragraph, table, visibleTextFilter) {
		}
		protected internal override FormatterPosition CreatePosition() {
			return new FormatterPosition(RunIndex.Zero, 0, 0);
		}
	}
	#endregion
	#region ParagraphBoxIterator
	public class ParagraphBoxIterator : ParagraphIteratorBase {
		int boxIndex;
		public ParagraphBoxIterator(Paragraph paragraph, PieceTable table, IVisibleTextFilter visibleTextFilter)
			: base(paragraph, table, visibleTextFilter) {
#if DEBUGTEST
			CheckParagraphBoxCollectionIntegrity();
#endif
		}
		protected internal override FormatterPosition CreatePosition() {
			return new FormatterPosition(RunIndex.Zero, 0, 0);
		}
		#region CheckParagraphBoxCollectionIntegrity
#if DEBUGTEST
		void CheckParagraphBoxCollectionIntegrity() {
			ParagraphBoxCollection boxes = Paragraph.BoxCollection;
			if (Paragraph.BoxCollection.Count == 0) {
				Debug.Assert(Paragraph.BoxCollection.IsValid);
				RunIndex start = Paragraph.FirstRunIndex;
				RunIndex end = Paragraph.LastRunIndex;
				for (RunIndex index = start; index <= end; index++) {
					Debug.Assert(!VisibleTextFilter.IsRunVisible(index));
				}
				return;
			}
			int count = boxes.Count;
			Box prevBox = null;
			int totalBoxLength = 0;
			for (int i = 0; i < count; i++) {
				Box box = boxes[i];
				Debug.Assert(box.StartPos.RunIndex == box.EndPos.RunIndex);
				if (prevBox != null) {
					if (prevBox.EndPos.RunIndex == box.StartPos.RunIndex)
						Debug.Assert(box.StartPos.Offset == prevBox.EndPos.Offset + 1);
					else {
						Debug.Assert(box.StartPos.Offset == 0);
						for (RunIndex runIndex = prevBox.EndPos.RunIndex + 1; runIndex < box.StartPos.RunIndex; runIndex++)
							Debug.Assert(!VisibleTextFilter.IsRunVisible(runIndex));
						Debug.Assert(box.StartPos.RunIndex >= prevBox.EndPos.RunIndex + 1);
					}
				}
				prevBox = box;
				totalBoxLength += box.EndPos.Offset - box.StartPos.Offset + 1;
			}
			int paragraphLength = 0;
			ParagraphIndex paragraphIndex = Paragraph.Index;
			Paragraph paragraph;
			do {
				paragraph = PieceTable.Paragraphs[paragraphIndex];
				for (RunIndex runIndex = paragraph.FirstRunIndex; runIndex <= paragraph.LastRunIndex; runIndex++) {
					if (VisibleTextFilter.IsRunVisible(runIndex))
						paragraphLength += Table.Runs[runIndex].Length;					
				}
				paragraphIndex++;
			}while(!VisibleTextFilter.IsRunVisible(paragraph.LastRunIndex));
			Debug.Assert(totalBoxLength == paragraphLength);
		}
#endif
		#endregion
		public Box CurrentBox { get { return Paragraph.BoxCollection[boxIndex]; } }
		internal int BoxIndex { get { return boxIndex; } set { boxIndex = value; } }
		public void NextBox() {
			NextBoxCore();
			base.SetPosition(CurrentBox.StartPos);
		}
		public override ParagraphIteratorResult Next() {
			ParagraphIteratorResult result = base.Next();
			if (Offset > CurrentBox.EndPos.Offset || result != ParagraphIteratorResult.Success)
				NextBoxCore();
			return result;
		}
		public override void SetPosition(FormatterPosition pos) {
			base.SetPosition(pos);
			BoxIndex = pos.BoxIndex;
		}
		protected override FormatterPosition GetPreviousVisibleRunPosition() {
			FormatterPosition pos = base.GetPreviousVisibleRunPosition();
#if DEBUGTEST
			Debug.Assert(CurrentBox.StartPos.Offset == 0 && CurrentBox.StartPos.RunIndex == RunIndex);
#endif
			pos.SetBoxIndex(boxIndex - 1);
			return pos;
		}
		protected override FormatterPosition GetPreviousOffsetPosition() {
			FormatterPosition pos = base.GetPreviousOffsetPosition();
#if DEBUGTEST
			Debug.Assert(CurrentBox.StartPos.RunIndex == RunIndex && CurrentBox.EndPos.RunIndex == RunIndex);
#endif
			if (pos.Offset < CurrentBox.StartPos.Offset) {
#if DEBUGTEST
				Debug.Assert(CurrentBox.StartPos.Offset == Offset);
#endif
				pos.SetBoxIndex(BoxIndex - 1);
			}
			else
				pos.SetBoxIndex(BoxIndex);
			return pos;
		}
		public override FormatterPosition GetCurrentPosition() {
			FormatterPosition pos = base.GetCurrentPosition();
			pos.SetBoxIndex(BoxIndex);
			return pos;
		}
		void NextBoxCore() {
			if (boxIndex < Paragraph.BoxCollection.Count - 1)
				boxIndex++;
#if DEBUGTEST
			else
				Debug.Assert(IsEnd);
#endif
		}
		public void SetNextPosition(FormatterPosition prevPosition) {
			SetPosition(prevPosition.RunIndex, prevPosition.Offset + 1);
			this.BoxIndex = prevPosition.BoxIndex;
		}
	}
	#endregion
	#region SyllableBoxIterator
	public class SyllableBoxIterator : ParagraphBoxIterator {
		#region Fields
		FormatterPositionCollection hyphenPositions;
		FormatterPosition hyphenPos;
		int hyphenPosIndex;
		bool hyphenAtCurrentPosition;
		readonly IHyphenationService hyphenationService;
		readonly ParagraphBoxIterator iterator;
		FormatterPosition endPos;
		#endregion
		public SyllableBoxIterator(ParagraphBoxIterator iterator, IHyphenationService hyphenationService)
			: base(iterator.Paragraph, iterator.PieceTable, iterator.VisibleTextFilter) {
			Guard.ArgumentNotNull(hyphenationService, "hyphenationService");
			base.SetPosition(iterator.GetCurrentPosition());
			this.hyphenationService = hyphenationService;
			this.iterator = iterator;
#if DEBUGTEST
			SetInvalidHyphenPos();
#endif
			HyphenizeCurrentWord();
		}
		#region Properties
		public IHyphenationService Hyphenizer { get { return hyphenationService; } }
		internal ParagraphBoxIterator Iterator { get { return iterator; } }
		internal FormatterPositionCollection HyphenPositions { get { return hyphenPositions; } }
		protected virtual char HyphenChar { get { return Characters.Hyphen; } }
		#endregion
		public override char CurrentChar {
			get {
				if (hyphenAtCurrentPosition)
					return HyphenChar;
				else
					return base.CurrentChar;
			}
		}
		public override bool IsEnd { get { return (RunIndex == endPos.RunIndex && Offset >= endPos.Offset) || RunIndex > endPos.RunIndex; } }
		void SetInvalidHyphenPos() {
			this.hyphenPos = new FormatterPosition(new RunIndex(-1), -1, -1);
			this.hyphenPosIndex = -1;
		}
		protected override void SetPosition(RunIndex runIndex, int offset) {
			base.SetPosition(runIndex, offset);
			hyphenAtCurrentPosition = false;
			int count = hyphenPositions.Count;
			for (int i = 0; i < count; i++) {
				FormatterPosition pos = hyphenPositions[i];
				if (pos.RunIndex > runIndex || (pos.RunIndex == runIndex && pos.Offset > offset)) {
					hyphenPosIndex = i;
					hyphenPos = hyphenPositions[hyphenPosIndex];
					break;
				}
			}
		}
		public override void SetPosition(FormatterPosition pos) {
			SetPosition(pos.RunIndex, pos.Offset);
			BoxIndex = pos.BoxIndex;
		}
		internal string GetCurrentWord() {
			StringBuilder result = new StringBuilder();
			while (!iterator.IsEnd) {
				char ch = iterator.CurrentChar;
				if (IsEndOfWord(ch))
					break;
				result.Append(ch);
				iterator.Next();
			}
			endPos = iterator.GetCurrentPosition();
			return result.ToString();
		}
		protected virtual void HyphenizeCurrentWord() {
			string word = GetCurrentWord();
			List<int> hyphenIndices = hyphenationService.Hyphenize(word);
			hyphenPositions = new FormatterPositionCollection();
			int hyphenIndicesCount = hyphenIndices.Count;
			if (hyphenIndicesCount <= 0) {
				SetInvalidHyphenPos();
				return;
			}
			FormatterPosition startWordPosition = GetCurrentPosition();
			int i = 0;
			int hyphenIndex = 0;
			while (!IsEnd && !IsWhiteSpace(base.CurrentChar)) {
				if (i == hyphenIndices[hyphenIndex]) {
					hyphenPositions.Add(GetCurrentPosition());
					hyphenIndex++;
					if (hyphenIndex >= hyphenIndicesCount)
						break;
				}
				i++;
				base.Next();
			}
			Debug.Assert(hyphenPositions.Count == hyphenIndicesCount);
			hyphenPosIndex = 0;
			hyphenPos = hyphenPositions[hyphenPosIndex];
			SetPosition(startWordPosition);
		}
		static bool IsEndOfWord(char ch) {
			if (IsWhiteSpace(ch))
				return true;
			if (IsBreak(ch))
				return true;
			return false;
		}
		static bool IsBreak(char ch) {
			return ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak;
		}
		static bool IsWhiteSpace(char ch) {
			return ch == ' ' || ch == '\t';
		}
		public override ParagraphIteratorResult Next() {
			if (hyphenAtCurrentPosition) {
				hyphenAtCurrentPosition = false;
				hyphenPosIndex++;
				if (hyphenPosIndex >= hyphenPositions.Count)
					SetInvalidHyphenPos();
				else
					hyphenPos = hyphenPositions[hyphenPosIndex];
				return ParagraphIteratorResult.Success;
			}
			ParagraphIteratorResult result = base.Next();
			if (result != ParagraphIteratorResult.Finished) {
				if (RunIndex == hyphenPos.RunIndex && Offset == hyphenPos.Offset) {
					hyphenAtCurrentPosition = true;
				}
				if (IsEnd)
					return ParagraphIteratorResult.Finished;
			}
			return result;
		}
	}
	#endregion
	#region ParagraphIteratorResult
	public enum ParagraphIteratorResult {
		Success,
		RunFinished,
		Finished,
	}
	#endregion
}
