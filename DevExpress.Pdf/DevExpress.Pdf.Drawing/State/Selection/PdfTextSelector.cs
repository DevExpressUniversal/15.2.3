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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfTextSelector {
		readonly IPdfViewerNavigationController navigationController;
		readonly PdfPageDataCache pageDataCache;
		readonly PdfDocumentState documentState;
		readonly PdfSelectionState selectionState;
		double zoomFactor = 1;
		bool selectionInProgress;
		int selectionStartPageIndex;
		PdfPoint selectionStartPoint;
		PdfTextPosition selectionStartTextPosition;
		int PageCount { get { return pageDataCache.DocumentPages.Count; } }
		double TextExpansionFactorX { get { return 15 / zoomFactor; } }
		double TextExpansionFactorY { get { return 5 / zoomFactor; } }
		PdfTextSelection Selection { get { return GetSelection(GetPageTextRanges(selectionStartTextPosition, selectionState.Caret.Position)); } }
		public bool SelectionInProgress { 
			get { return selectionInProgress; } 
			set { selectionInProgress = value; }
		}
		public PdfTextSelector(IPdfViewerNavigationController navigationController, PdfPageDataCache pageDataCache, PdfDocumentState documentState) {
			this.navigationController = navigationController;
			this.pageDataCache = pageDataCache;
			this.documentState = documentState;
			selectionState = documentState.SelectionState;
		}
		public void SetZoomFactor(double zoomFactor) {
			this.zoomFactor = zoomFactor;
		}
		public bool HasContent(PdfDocumentPosition position) {
			return selectionInProgress || FindStartTextPosition(position) != null;
		}
		public PdfTextSelection GetSelection(IList<PdfPageTextRange> textRange) {
			if (textRange == null)
				return null;
			foreach (PdfPageTextRange wordRange in textRange) {
				int start = wordRange.StartWordNumber;
				int end = wordRange.EndWordNumber;
				if (wordRange.WholePage || ((start <= end || end == 0) && (start != end || wordRange.StartOffset < wordRange.EndOffset)))
					return new PdfTextSelection(pageDataCache, textRange);
			}
			return null;
		}
		public PdfTextSelection GetSelection(PdfDocumentArea documentArea) {
			List<PdfPageTextRange> ranges = new List<PdfPageTextRange>();
			PdfRectangle area = documentArea.Area;
			int pageIndex = documentArea.PageIndex;
			foreach (PdfTextLine line in pageDataCache.GetPageLines(pageIndex))
				if (area.Intersects(line.Rectangle.BoundingRectangle)) {
					PdfPageTextRange range = line.GetTextRange(pageIndex, area);
					if (range != null)
						ranges.Add(range);
				}
			return GetSelection(ranges);
		}
		public void MoveCaret(PdfMovementDirection direction) {
			bool raiseStateChanged = !selectionState.HasSelection;
			switch (CorrectMovementDirection(direction)) {
				case PdfMovementDirection.Left:
					if (!MoveCaretToLeft())
						PerformCaretMoveAction(MoveLeft);
					break;
				case PdfMovementDirection.Right:
					if (!MoveCaretToRight() && selectionState.HasCaret)
						PerformCaretMoveAction(MoveRight);
					break;
				case PdfMovementDirection.Up:
					MoveCaretToLeft();
					PerformCaretMoveAction(MoveUp);
					break;
				case PdfMovementDirection.Down:
					MoveCaretToRight();
					PerformCaretMoveAction(MoveDown);
					break;
				case PdfMovementDirection.NextWord:
					PerformCaretMoveAction(MoveToNextWord);
					break;
				case PdfMovementDirection.PreviousWord:
					PerformCaretMoveAction(MoveToPreviousWord);
					break;
				case PdfMovementDirection.LineStart:
					PerformCaretMoveAction(MoveToLineStart);
					break;
				case PdfMovementDirection.LineEnd:
					PerformCaretMoveAction(MoveToLineEnd);
					break;
				case PdfMovementDirection.DocumentStart:
					PerformCaretMoveAction(MoveToDocumentStart);
					break;
				case PdfMovementDirection.DocumentEnd:
					PerformCaretMoveAction(MoveToDocumentEnd);
					break;
			}
			if (raiseStateChanged)
				selectionState.RaiseStateChanged();
		}
		public void SelectWithCaret(PdfMovementDirection direction) {
			if (!selectionState.HasCaret)
				return;
			StoreSelectionStartTextPosition();
			switch (CorrectMovementDirection(direction)) {
				case PdfMovementDirection.Left:
					MoveLeft();
					break;
				case PdfMovementDirection.Right:
					MoveRight();
					break;
				case PdfMovementDirection.Up:
					MoveUp();
					break;
				case PdfMovementDirection.Down:
					MoveDown();
					break;
				case PdfMovementDirection.NextWord:
					MoveToNextWord();
					break;
				case PdfMovementDirection.PreviousWord:
					MoveToPreviousWord();
					break;
				case PdfMovementDirection.LineStart:
					MoveToLineStart();
					break;
				case PdfMovementDirection.LineEnd:
					MoveToLineEnd();
					break;
				case PdfMovementDirection.DocumentStart:
					MoveToDocumentStart();
					break;
				case PdfMovementDirection.DocumentEnd:
					MoveToDocumentEnd();
					break;
			}
			if (selectionState.HasCaret) {
				selectionState.Selection = Selection;
				EnsureCaretVisibility();
			}
		}
		public PdfTextPosition FindClosestTextPosition(PdfDocumentPosition position, PdfTextPosition textPosition) {
			int pageIndex = position.PageIndex;
			if (pageIndex < 0)
				return null;
			PdfPoint point = position.Point;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			PdfTextLine closestLine = null;
			double minDistance = Double.MaxValue;
			foreach (PdfTextLine line in lines) {
				PdfOrientedRectangle lineRect = line.Rectangle;
				if (lineRect.PointIsInRect(point))
					foreach (PdfWordPart word in line.WordParts)
						if (word.Rectangle.PointIsInRect(point))
							return line.GetTextPosition(pageIndex, point);
				double distance = PdfMathUtils.CalcDistanceToRectangle(point, lineRect.BoundingRectangle);
				if (distance < minDistance) {
					closestLine = line;
					minDistance = distance;
				}
			}
			if (closestLine == null)
				for (int i = lines.Count - 1; i >= 0; i--) {
					PdfTextLine line = lines[i];
					PdfRectangle lineRect = line.Rectangle.BoundingRectangle;
					if (lineRect.Bottom <= point.Y && lineRect.Right >= point.X)
						closestLine = line;
				}
			if (closestLine == null) {
				int wordNumber = textPosition.WordNumber;
				foreach (PdfTextLine line in pageDataCache.GetPageLines(textPosition.PageIndex)) {
					int lastLineIndex = line.Count - 1;
					if (lastLineIndex >= 0 && wordNumber >= line[0].WordNumber && line[lastLineIndex].WordNumber >= wordNumber) {
						closestLine = line;
						break;
					}
				}
			}
			return closestLine == null ? null : closestLine.GetTextPosition(pageIndex, point);
		}
		public bool StartSelection(PdfDocumentPosition position) {
			PdfTextPosition startSelectionTextPosition = FindStartTextPosition(position);
			if (startSelectionTextPosition == null) {
				selectionInProgress = false;
				selectionStartPageIndex = -1;
				if (selectionState.HasCaret) {
					selectionState.Caret = null;
					selectionState.RaiseStateChanged();
				}
			}
			else {
				selectionInProgress = true;
				selectionStartPageIndex = position.PageIndex;
				selectionStartPoint = position.Point;
				UpdateSelection(startSelectionTextPosition);
			}
			return selectionInProgress;
		}
		public void PerformSelection(PdfDocumentPosition position) {
			if (selectionInProgress && (position.PageIndex != selectionStartPageIndex || !position.Point.Equals(selectionStartPoint))) {
				StoreSelectionStartTextPosition();
				IList<PdfPageTextRange> textRanges = GetPageTextRanges(selectionStartTextPosition, position);
				if (textRanges != null) {
					PdfPageTextRange wordRange = textRanges[textRanges.Count - 1];
					PdfPageTextPosition endTextPosition = wordRange.EndTextPosition;
					MoveCaret(new PdfTextPosition(wordRange.PageIndex, endTextPosition == selectionStartTextPosition ? wordRange.StartTextPosition : endTextPosition));
					selectionState.Selection = GetSelection(textRanges);
				}
			}
		}
		public void SelectWord(PdfDocumentPosition position) {
			PdfTextPosition textPosition = FindStartTextPosition(position);
			if (textPosition != null) {
				int wordNumber = textPosition.WordNumber;
				SetSelection(GetSelection(new PdfPageTextRange(textPosition.PageIndex, new PdfPageTextPosition(wordNumber, 0), new PdfPageTextPosition(wordNumber, GetWordEndPosition(textPosition)))));
			}
		}
		public void SelectLine(PdfDocumentPosition position) {
			PdfTextLine line = FindLine(position);
			if (line != null) {
				IList<PdfWordPart> parts = line.WordParts;
				int lastPartIndex = parts.Count - 1;
				if (lastPartIndex < 0)
					SetSelection(null);
				else {
					PdfWordPart lastPart = parts[lastPartIndex];
					SetSelection(GetSelection(new PdfPageTextRange(position.PageIndex, parts[0].WordNumber, 0, lastPart.WordNumber, lastPart.Characters.Count)));
				}
			}
		}
		public void SelectPage(PdfDocumentPosition position) {
			int pageIndex = position.PageIndex;
			if (pageIndex >= 0 && FindStartTextPosition(position) != null)
				SetSelection(GetSelection(new PdfPageTextRange(pageIndex)));
		}
		public void Select(PdfDocumentArea documentArea) {
			SetSelection(GetSelection(documentArea));
		}
		public void SelectText(PdfPageTextRange pageTextRange) {
			SetSelection(GetSelection(pageTextRange));
		}
		public void SelectText(IList<PdfPageTextRange> textRange) {
			SetSelection(GetSelection(textRange));
		}
		public void SelectAllText() {
			int pageCount = PageCount;
			for (int i = 0; i < pageCount; i++)
				if (pageDataCache.GetPageLines(i).Count > 0) {
					IList<PdfPageTextRange> textRange = new List<PdfPageTextRange>();
					for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
						textRange.Add(new PdfPageTextRange(pageIndex));
					SetSelection(new PdfTextSelection(pageDataCache, textRange));
					return;
				}
		}
		public IList<PdfPageTextRange> GetPageTextRanges(PdfTextPosition startTextPosition, PdfDocumentPosition endPosition) {
			return startTextPosition == null ? null : GetPageTextRanges(startTextPosition, FindClosestTextPosition(endPosition, startTextPosition));
		}
		IList<PdfPageTextRange> GetPageTextRanges(PdfTextPosition startTextPosition, PdfTextPosition endTextPosition) {
			if (endTextPosition == null)
				return null;
			List<PdfPageTextRange> textRanges = new List<PdfPageTextRange>();
			int startPageIndex = startTextPosition.PageIndex;
			int endPageIndex = endTextPosition.PageIndex;
			if (startPageIndex == endPageIndex) {
				int startWordNumber = startTextPosition.WordNumber;
				int endWordNumber = endTextPosition.WordNumber;
				if (startWordNumber > endWordNumber || (startWordNumber == endWordNumber && startTextPosition.Offset > endTextPosition.Offset))
					textRanges.Add(new PdfPageTextRange(endPageIndex, endTextPosition, startTextPosition));
				else 
					textRanges.Add(new PdfPageTextRange(startPageIndex, startTextPosition, endTextPosition));
			}
			else {
				if (startPageIndex > endPageIndex) {
					PdfTextPosition temp = startTextPosition;
					startTextPosition = endTextPosition;
					endTextPosition = temp;
					startPageIndex = startTextPosition.PageIndex;
					endPageIndex = endTextPosition.PageIndex;
				}
				textRanges.Add(new PdfPageTextRange(startPageIndex, startTextPosition, new PdfPageTextPosition(0, -1)));
				for (int i = startPageIndex + 1; i < endPageIndex; i++)
					textRanges.Add(new PdfPageTextRange(i));
				textRanges.Add(new PdfPageTextRange(endPageIndex, new PdfPageTextPosition(0, 0), endTextPosition));
			}
			return textRanges;
		}
		bool IsPositionInLine(int pageIndex, int lineIndex, int wordNumber, int offset) {
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			PdfTextLine line = lines[lineIndex];
			if (!line.IsPositionInLine(wordNumber, offset))
				return false;
			IList<PdfWordPart> wordParts = line.WordParts;
			if (wordParts[wordParts.Count - 1].WordNumber != wordNumber)
				return true;
			if (lineIndex < lines.Count - 1)
				return !lines[lineIndex + 1].IsPositionInLine(wordNumber, offset);
			int lastPageIndex = PageCount - 1;
			do {
				if (pageIndex == lastPageIndex)
					return true;
				lines = pageDataCache.GetPageLines(++pageIndex);
			} while (lines == null || lines.Count == 0);
			line= lines[0];
			return !line.IsPositionInLine(wordNumber, offset) || line.WordParts[0].WordNumber != wordNumber;
		}
		PdfTextLine FindLine(PdfTextPosition position) {
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			int lineIndex = 0;
			foreach (PdfTextLine line in pageDataCache.GetPageLines(pageIndex))
				if (IsPositionInLine(pageIndex, lineIndex++, wordNumber, offset))
					return line;
			return null;
		}
		PdfTextLine FindLine(PdfDocumentPosition position) {
			int pageIndex = position.PageIndex;
			if (pageIndex < 0)
				return null;
			PdfPoint point = position.Point;
			double textExpansionFactorX = TextExpansionFactorX;
			double textExpansionFactorY = TextExpansionFactorY;
			PdfTextLine textLine = null;
			foreach (PdfTextLine line in pageDataCache.GetPageLines(pageIndex)) {
				PdfOrientedRectangle rectangle = line.Rectangle;
				IList<PdfWordPart> wordParts = line.WordParts;
				if (rectangle.PointIsInRect(point, textExpansionFactorX))
					foreach (PdfWordPart word in wordParts)
						if (word.Rectangle.PointIsInRect(point, textExpansionFactorX))
							return line;
				if (rectangle.PointIsInRect(point, textExpansionFactorX, textExpansionFactorY))
					foreach (PdfWordPart word in wordParts)
						if (word.Rectangle.PointIsInRect(point, textExpansionFactorX, textExpansionFactorY))
							textLine = line;
			}
			return textLine;
		}
		PdfTextPosition FindStartTextPosition(PdfDocumentPosition position) {
			PdfTextLine line = FindLine(position);
			return line == null ? null : line.GetTextPosition(position.PageIndex, position.Point);
		}
		PdfMovementDirection CorrectMovementDirection(PdfMovementDirection direction) {
			const double degToRad = Math.PI / 180;
			const double range = 20.0;
			const double firstQuadrantDelimiter = (90.0 - range) * degToRad;
			const double secondQuadrantDelimiter = (90.0 + range) * degToRad;
			const double thirdQuadrantDelimiter = (270.0 - range) * degToRad;
			const double fourthQuadrantDelimiter = (270.0 + range) * degToRad;
			PdfTextPosition caretPosition = selectionState.Caret.Position;
			PdfTextLine line = FindLine(caretPosition);
			if (line == null)
				return direction;
			double angle = PdfMathUtils.NormalizeAngle(line.Rectangle.Angle - pageDataCache.DocumentPages[caretPosition.PageIndex].Rotate * degToRad);
			if (angle < firstQuadrantDelimiter)
				return direction;
			if (angle <= secondQuadrantDelimiter)
				switch (direction) {
					case PdfMovementDirection.Left:
						return PdfMovementDirection.Up;
					case PdfMovementDirection.Right:
						return PdfMovementDirection.Down;
					case PdfMovementDirection.Up:
						return PdfMovementDirection.Right;
					case PdfMovementDirection.Down:
						return PdfMovementDirection.Left;
					default:
						return direction;
				}
			if (angle < thirdQuadrantDelimiter)
				switch (documentState.RotationAngle) {
					case 0:
					case 180:
						switch (direction) {
							case PdfMovementDirection.Left:
								return PdfMovementDirection.Right;
							case PdfMovementDirection.Right:
								return PdfMovementDirection.Left;
							default: 
								return direction;
						}
					case 90:
					case 270:
						switch (direction) {
							case PdfMovementDirection.Up:
								return PdfMovementDirection.Down;
							case PdfMovementDirection.Down:
								return PdfMovementDirection.Up;
							default:
								return direction;
						}
					default:
						return direction;
				}
			if (angle <= fourthQuadrantDelimiter)
				switch (direction) {
					case PdfMovementDirection.Left:
						return PdfMovementDirection.Down;
					case PdfMovementDirection.Right:
						return PdfMovementDirection.Up;
					case PdfMovementDirection.Up:
						return PdfMovementDirection.Left;
					case PdfMovementDirection.Down:
						return PdfMovementDirection.Right;
					default:
						return direction;
				}
			return direction;
		}
		PdfCaretViewData GetCaretViewData(PdfTextPosition position) {
			PdfTextLine line = FindLine(position);
			if (line == null)
				return new PdfCaretViewData(new PdfPoint(0, 0), 0, 0);
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			foreach (PdfWordPart wordPart in line.WordParts)
				if (wordPart.IsSuitable(wordNumber, offset)) {
					PdfOrientedRectangle wordPartRectangle = wordPart.Rectangle;
					IList<PdfCharacter> characters = wordPart.Characters;
					int characterOffset = offset - wordPart.WrapOffset;
					PdfPoint location = characterOffset == characters.Count ? characters[characterOffset - 1].Rectangle.TopRight : characters[characterOffset].Rectangle.TopLeft;
					double x;
					double y;
					double angle0 = wordPartRectangle.Angle;
					if (angle0 == 0 || angle0 == Math.PI) {
						x = location.X;
						y = wordPartRectangle.Top;
					}
					else if (angle0 == 3 * Math.PI / 2 || angle0 == Math.PI / 2) {
						x = wordPartRectangle.Left;
						y = location.Y;
					}
					else {
						double x0 = wordPartRectangle.Left;
						double y0 = wordPartRectangle.Top;
						double k0 = Math.Tan(angle0);
						double k = Math.Tan(angle0 + Math.PI / 2);
						x = (x0 * k0 - y0 - location.X * k + location.Y) / (k0 - k);
						y = k0 * (x - x0) + y0;
					}
					return new PdfCaretViewData(new PdfPoint(x, y), wordPartRectangle.Height, line.Rectangle.Angle);
				}
			return new PdfCaretViewData(default(PdfPoint), 0, line.Rectangle.Angle);
		}
		void PerformCaretMoveAction(Action moveAction) {
			if (selectionState.HasCaret && moveAction != null) {
				moveAction();
				selectionState.Selection = null;
			}
		}
		void MoveCaret(PdfTextPosition position) {
			PdfCaretViewData viewData = GetCaretViewData(position);
			selectionState.Caret = new PdfCaret(position, viewData, viewData.TopLeft);
		}
		void SetSelectionCaret(PdfTextPosition position) {
			selectionState.Caret = new PdfCaret(position, GetCaretViewData(position), selectionState.HasCaret ? selectionState.Caret.StartCoordinates : new PdfPoint());
			EnsureCaretVisibility();
		}
		void MoveCaretAndEnsureVisibility(PdfTextPosition position) {
			MoveCaret(position);
			EnsureCaretVisibility();
		}
		void UpdateSelection(PdfTextPosition position) {
			MoveCaret(position);
			if (selectionState.HasSelection)
				selectionState.Selection = null;
			else
				selectionState.RaiseStateChanged();
		}
		bool MoveCaretToLeft() {
			PdfTextSelection textSelection = selectionState.Selection as PdfTextSelection;
			if (textSelection == null)
				return false;
			PdfPageTextRange wordRange = textSelection.TextRange[0];
			UpdateSelection(new PdfTextPosition(wordRange.PageIndex, wordRange.StartTextPosition));
			return true;
		}
		bool MoveCaretToRight() {
			PdfTextSelection textSelection = selectionState.Selection as PdfTextSelection;
			if (textSelection == null)
				return false;
			PdfPageTextRange wordRange = textSelection.TextRange[textSelection.TextRange.Count - 1];
			UpdateSelection(new PdfTextPosition(wordRange.PageIndex, wordRange.EndTextPosition));
			return true;
		}
		void StoreSelectionStartTextPosition() {
			if (!selectionState.HasSelection && selectionState.Caret != null)
				selectionStartTextPosition = selectionState.Caret.Position;
		}
		bool IsFollowingWordPartWithSameWordNumberExist(int pageIndex, int lineIndex, int wordNumber) {
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			if (++lineIndex < lines.Count)
				return lines[lineIndex].WordParts[0].WordNumber == wordNumber;
			int pageCount = PageCount;
			for (int i = pageIndex + 1; i < pageCount; i++) {
				lines = pageDataCache.GetPageLines(i);
				if (lines.Count > 0)
					return lines[0].WordParts[0].WordNumber == wordNumber;
			}
			return false;
		}
		int GetWordEndPosition(PdfTextPosition position) {
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int lineIndex = 0;
			foreach (PdfTextLine line in pageDataCache.GetPageLines(pageIndex)) {
				IList<PdfWordPart> wordParts = line.WordParts;
				int lastWordPartIndex = wordParts.Count - 1;
				if (lastWordPartIndex >= 0) {
					for (int j = 0; j < lastWordPartIndex; j++) {
						PdfWordPart wordPart = wordParts[j];
						if (wordPart.WordNumber == wordNumber)
							return wordPart.EndWordPosition;
					}
					PdfWordPart lastWordPart = wordParts[lastWordPartIndex];
					if (lastWordPart.WordNumber == wordNumber && !IsFollowingWordPartWithSameWordNumberExist(pageIndex, lineIndex, wordNumber))
						return lastWordPart.EndWordPosition;
				}
				lineIndex++;
			}
			return -1;
		}
		void MoveLeft() {
			PdfTextPosition position = selectionState.Caret.Position;
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			if (offset > 0)
				MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber, offset - 1)));
			else if (wordNumber > 1) {
				int wordIndex = wordNumber - 1;
				MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordIndex, GetWordEndPosition(new PdfTextPosition(pageIndex, wordIndex, 0)))));
			}
			else {
				for (int previousPageIndex = pageIndex - 1; previousPageIndex >= 0; previousPageIndex--) {
					IList<PdfTextLine> lines = pageDataCache.GetPageLines(previousPageIndex);
					int lastLineIndex = lines.Count - 1;
					if (lastLineIndex >= 0) {
						IList<PdfWordPart> wordParts = lines[lastLineIndex].WordParts;
						PdfWordPart wordPart = wordParts[wordParts.Count - 1];
						MoveCaretAndEnsureVisibility(new PdfTextPosition(previousPageIndex, new PdfPageTextPosition(wordPart.WordNumber, wordPart.NextWrapOffset)));
						return;
					}
				}
				MoveCaretAndEnsureVisibility(position);
			}
		}
		bool MoveRight(IList<PdfWordPart> wordParts, int pageIndex, int lineIndex, int wordNumber, int offset, bool processLastWordPart) {
			int lastWordPartIndex = wordParts.Count - 1;
			int lastWordNumber = wordParts[lastWordPartIndex].WordNumber;
			if (!processLastWordPart)
				lastWordPartIndex--;
			for (int j = 0; j <= lastWordPartIndex; j++) {
				PdfWordPart wordPart = wordParts[j];
				if (wordPart.IsSuitable(wordNumber, offset)) {
					if (wordPart.NextWrapOffset > offset || (wordNumber == lastWordNumber && IsFollowingWordPartWithSameWordNumberExist(pageIndex, lineIndex, wordNumber)))
						MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber, offset + 1)));
					else
						MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber + 1, wordPart.WordEnded ? 0 : 1)));
					return true;
				}
			}
			return false;
		}
		void MoveRight() {
			PdfTextPosition position = selectionState.Caret.Position;
			int pageIndex = position.PageIndex;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			int lastLineIndex = lines.Count - 1;
			if (lastLineIndex >= 0) {
				int wordNumber = position.WordNumber;
				int offset = position.Offset;
				for (int i = 0; i < lastLineIndex; i++) {
					PdfTextLine line = lines[i];
					if (line.IsPositionInLine(wordNumber, offset) && MoveRight(line.WordParts, pageIndex, i, wordNumber, offset, true))
						return;
				}
				PdfTextLine lastLine = lines[lastLineIndex];
				if (lastLine.IsPositionInLine(wordNumber, offset)) {
					IList<PdfWordPart> wordParts = lastLine.WordParts;
					if (MoveRight(wordParts, pageIndex, lastLineIndex, wordNumber, offset, false))
						return;
					int lastWordPartIndex = wordParts.Count - 1;
					PdfWordPart lastWordPart = wordParts[lastWordPartIndex];
					if (lastWordPart.IsSuitable(wordNumber, offset)) {
						if (lastWordPart.NextWrapOffset > offset || (wordNumber == wordParts[lastWordPartIndex].WordNumber && IsFollowingWordPartWithSameWordNumberExist(pageIndex, lastLineIndex, wordNumber))) {
							MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber, offset + 1)));
							return;
						}
						int pageCount = PageCount;
						for (int nextPageIndex = pageIndex + 1; nextPageIndex < pageCount; nextPageIndex++)
							if (pageDataCache.GetPageLines(nextPageIndex).Count > 0) {
								MoveCaretAndEnsureVisibility(new PdfTextPosition(nextPageIndex, new PdfPageTextPosition(1, 0)));
								return;
							}
					}
				}
			}
			MoveCaretAndEnsureVisibility(position);
		}
		void MoveDown() {
			PdfCaret caret = selectionState.Caret;
			PdfTextPosition position = caret.Position;
			int pageIndex = position.PageIndex;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			int lastLineIndex = lines.Count - 1;
			if (lastLineIndex >= 0) {
				PdfPoint caretPosition = caret.StartCoordinates;
				int wordNumber = position.WordNumber;
				int offset = position.Offset;
				for (int i = 0; i < lastLineIndex; i++)
					if (IsPositionInLine(pageIndex, i, wordNumber, offset)) {
						SetSelectionCaret(lines[i + 1].GetTextPosition(pageIndex, caretPosition));
						return;
					}
				int pageCount = PageCount;
				for (int nextPageIndex = pageIndex + 1; nextPageIndex < pageCount; nextPageIndex++) {
					lines = pageDataCache.GetPageLines(nextPageIndex);
					if (lines.Count > 0) {
						SetSelectionCaret(lines[0].GetTextPosition(nextPageIndex, caretPosition));
						return;
					}
				}
			}
			SetSelectionCaret(position);
		}
		void MoveUp() {
			PdfCaret caret = selectionState.Caret;
			PdfPoint coordinates = caret.StartCoordinates;
			PdfTextPosition position = caret.Position;
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			for (int i = lines.Count - 1; i > 0; i--) {
				PdfTextLine line = lines[i];
				if (line.IsPositionInLine(wordNumber, offset)) {
					PdfTextPosition newPosition = lines[i - 1].GetTextPosition(pageIndex, coordinates);
					wordNumber = newPosition.WordNumber;
					offset = newPosition.Offset;
					SetSelectionCaret(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber, line.IsPositionInLine(wordNumber, offset) ? (offset - 1) : offset)));
					return;
				}
			}
			for (int prevPageIndex = pageIndex - 1; prevPageIndex >= 0; prevPageIndex--) {
				lines = pageDataCache.GetPageLines(prevPageIndex);
				if (lines.Count > 0) {
					SetSelectionCaret(lines[lines.Count - 1].GetTextPosition(prevPageIndex, coordinates));
					return;
				}
			}
			SetSelectionCaret(position);
		}
		void MoveToLineStart() {
			PdfCaret caret = selectionState.Caret;
			PdfTextPosition position = caret.Position;
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			int lineCout = lines.Count;
			for (int i = 0; i < lineCout; i++) {
				PdfTextLine line = lines[i];
				if (line.IsPositionInLine(wordNumber, offset))
					MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(line.WordParts[0].WordNumber, line.WordParts[0].WrapOffset)));
			}
		}
		void MoveToLineEnd() {
			PdfCaret caret = selectionState.Caret;
			PdfTextPosition position = caret.Position;
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			int lineCount = lines.Count;
			for (int i = 0; i < lineCount; i++) {
				PdfTextLine line = lines[i];
				if (line.IsPositionInLine(wordNumber, offset)) {
					IList<PdfWordPart> parts = line.WordParts;
					int lastWordIndex = parts.Count - 1;
					PdfWordPart last = parts[lastWordIndex];
					MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(last.WordNumber, last.Characters.Count)));
				}
			}
		}
		void MoveToNextWord() {
			PdfTextPosition position = selectionState.Caret.Position;
			int pageIndex = position.PageIndex;
			int wordNumber = position.WordNumber;
			int offset = position.Offset;
			int pageCount = documentState.Document.Pages.Count;
			IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
			PdfTextLine lastLine = lines[lines.Count - 1];
			bool hasNextWord = false;
			if (lastLine.Count != 0 && lastLine[lastLine.Count - 1].WordNumber == wordNumber) {
				pageIndex++;
				while (pageIndex < pageCount && !hasNextWord) {
					lines = pageDataCache.GetPageLines(pageIndex++);
					hasNextWord = lines != null && lines.Count > 0;
				}
			}
			else
				hasNextWord = true;
			if (hasNextWord)
				MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber + 1, 0)));
			else {
				lines = pageDataCache.GetPageLines(position.PageIndex);
				int lineCount = lines.Count;
				for (int i = 0; i < lineCount; i++) {
					PdfTextLine line = lines[i];
					if (line.IsPositionInLine(wordNumber, offset))
						foreach (PdfWordPart word in line.WordParts)
							if (word.WordNumber == wordNumber)
								MoveCaretAndEnsureVisibility(new PdfTextPosition(position.PageIndex, new PdfPageTextPosition(wordNumber, word.Characters.Count)));
				}
			}
		}
		void MoveToPreviousWord() {
			PdfTextPosition position = selectionState.Caret.Position;
			int pageIndex = position.PageIndex;
			int offset = position.Offset;
			int wordNumber = offset != 0 ? position.WordNumber : position.WordNumber - 1;
			MoveCaretAndEnsureVisibility(new PdfTextPosition(pageIndex, new PdfPageTextPosition(wordNumber < 1 ? 1 : wordNumber, 0)));
		}
		void MoveToDocumentStart() {
			int pageCount = documentState.Document.Pages.Count;
			for (int i = 0; i < pageCount; i++)
				if (pageDataCache.GetPageLines(i).Count != 0) {
					MoveCaretAndEnsureVisibility(new PdfTextPosition(i, new PdfPageTextPosition(1, 0)));
					return;
				}
		}
		void MoveToDocumentEnd() {
			for (int i = documentState.Document.Pages.Count - 1; i >= 0; i--) {
				IList<PdfTextLine> lines = pageDataCache.GetPageLines(i);
				int lineCount = lines == null ? 0 : lines.Count;
				if (lineCount > 0) {
					IList<PdfWordPart> lastLine = lines[lineCount - 1].WordParts;
					if (lastLine.Count > 0) {
						PdfWordPart lastWord = lastLine[lastLine.Count - 1];
						MoveCaretAndEnsureVisibility(new PdfTextPosition(i, new PdfPageTextPosition(lastWord.WordNumber, lastWord.Characters.Count)));
						return;
					}
				}
			}
		}
		void EnsureCaretVisibility() {
			PdfCaret caret = selectionState.Caret;
			if (caret != null) {
				PdfCaretViewData viewData = caret.ViewData;
				double height = viewData.Height;
				double angle = viewData.Angle;
				PdfPoint topLeft = viewData.TopLeft;
				PdfRectangle bounds = new PdfRectangle(topLeft, new PdfPoint(topLeft.X + height * Math.Sin(angle), topLeft.Y - height * Math.Cos(angle)));
				navigationController.ShowRectangleOnPage(PdfRectangleAlignMode.Edge, caret.Position.PageIndex, bounds);
			}
		}
		PdfTextSelection GetSelection(PdfPageTextRange pageTextRange) {
			return GetSelection(new PdfPageTextRange[] { pageTextRange });
		}
		void SetSelection(PdfTextSelection textSelection) {
			if (textSelection != null) {
				IList<PdfPageTextRange> textRange = textSelection.TextRange;
				int lastRangeIndex = textRange.Count - 1;
				while (lastRangeIndex >= 0) {
					PdfPageTextRange range = textRange[lastRangeIndex];
					int pageIndex = range.PageIndex;
					selectionStartTextPosition = new PdfTextPosition(pageIndex, range.StartTextPosition);
					if (range.WholePage) {
						IList<PdfTextLine> lines = pageDataCache.GetPageLines(pageIndex);
						int linesCount = lines.Count;
						if (linesCount > 0) {
							IList<PdfWordPart> lastLineWords = lines[lines.Count - 1].WordParts;
							PdfWordPart lastWordPart = lastLineWords[lastLineWords.Count - 1];
							MoveCaret(new PdfTextPosition(pageIndex, new PdfPageTextPosition(lastWordPart.WordNumber, lastWordPart.Characters.Count)));
							break;
						}
						else
							lastRangeIndex--;
					}
					else {
						MoveCaret(new PdfTextPosition(pageIndex, range.EndTextPosition));
						break;
					}
				}
			}
			selectionState.Selection = textSelection;
		}
	}
}
