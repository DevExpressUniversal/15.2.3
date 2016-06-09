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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public enum PdfMovementDirection { Left, Down, Right, Up, NextWord, PreviousWord, LineStart, LineEnd, DocumentStart, DocumentEnd }
	public class PdfDataSelector {
		readonly PdfDocumentState documentState;
		readonly PdfSelectionState selectionState;
		readonly PdfTextSelector textSelector;
		readonly PdfImageSelector imageSelector;
		bool selectionStartedOutsideContent;
		PdfDocumentPosition startSelectionPosition;
		public bool SelectionInProgress { get { return textSelector.SelectionInProgress || imageSelector.SelectionInProgress; } }
		public PdfDataSelector(IPdfViewerNavigationController navigationController, PdfDocumentState documentState) {
			this.documentState = documentState;
			selectionState = documentState.SelectionState;
			PdfPageDataCache pageDataCache = new PdfPageDataCache(documentState.Document.Pages, false);
			textSelector = new PdfTextSelector(navigationController, pageDataCache, documentState);
			imageSelector = new PdfImageSelector(navigationController, pageDataCache, documentState);
		}
		public void SetZoomFactor(double zoomFactor) {
			textSelector.SetZoomFactor(zoomFactor);
		}
		public void MoveCaret(PdfMovementDirection direction) {
			textSelector.MoveCaret(IsArrowKeysNavigation(direction) ? GetNormalizedDirection(direction) : direction);
		}
		public void SelectWithCaret(PdfMovementDirection direction) {
			textSelector.SelectWithCaret(IsArrowKeysNavigation(direction) ? GetNormalizedDirection(direction) : direction);
		}
		public void HideCaret() {
			selectionState.Caret = null;
		}
		public void StartSelection(PdfDocumentPosition startPosition) {
			ClearSelection();
			if (!textSelector.StartSelection(startPosition))
				selectionStartedOutsideContent = !imageSelector.StartSelection(startPosition);
		}
		public void StartSelection(PdfMouseAction mouseAction) {
			startSelectionPosition = null;
			PdfDocumentPosition documentPosition = mouseAction.DocumentPosition;
			PdfDocumentContent dataType = GetContentInfoWhileSelecting(documentPosition);
			if (dataType.ContentType == PdfDocumentContentType.Image)
				if (dataType.IsSelected)
					startSelectionPosition = documentPosition;
				else
					StartSelection(documentPosition);
			else
				switch (mouseAction.Clicks) {
					case 2:
						SelectWord(documentPosition);
						break;
					case 3:
						SelectLine(documentPosition);
						break;
					case 4:
						SelectPage(documentPosition);
						break;
					default:
						if (dataType.IsSelected)
							startSelectionPosition = documentPosition;
						else 
							PerformSelection(mouseAction);
						break;
				}
		}
		public void PerformSelection(PdfDocumentPosition position) {
			if (!imageSelector.PerformSelection(position))
				textSelector.PerformSelection(position);
		}
		public void EndSelection(PdfDocumentPosition position) {
			selectionStartedOutsideContent = false;
			textSelector.SelectionInProgress = false;
			imageSelector.SelectionInProgress = false;
			PerformSelection(position);
		}
		public void EndSelection(PdfMouseAction mouseAction) {
			PdfDocumentPosition documentPosition = mouseAction.DocumentPosition;
			if (startSelectionPosition != null && startSelectionPosition.NearTo(documentPosition))
				if (GetContentInfoWhileSelecting(documentPosition).ContentType == PdfDocumentContentType.Image)
					ClearSelection();
				else 
					PerformSelection(mouseAction);
			EndSelection(documentPosition);
		}
		public void SelectWord(PdfDocumentPosition position) {
			textSelector.SelectWord(position);
		}
		public void SelectLine(PdfDocumentPosition position) {
			textSelector.SelectLine(position);
		}
		public void SelectPage(PdfDocumentPosition position) {
			textSelector.SelectPage(position);
		}
		public void Select(PdfDocumentArea documentArea) {
			textSelector.Select(documentArea);
			if (!selectionState.HasSelection)
				selectionState.Selection = GetImageSelection(documentArea);
		}
		public void SelectText(IList<PdfPageTextRange> textRange) {
			textSelector.SelectText(textRange);
		}
		public void SelectText(PdfDocumentPosition position1, PdfDocumentPosition position2) {
			selectionState.Selection = GetTextSelection(position1, position2);
		}
		public void SelectAllText() {
			textSelector.SelectAllText();
		}
		public void SelectImage(PdfDocumentArea documentArea) {
			selectionState.Selection = GetImageSelection(documentArea);
		}
		public void ClearSelection() {
			selectionState.Selection = null;
			textSelector.SelectionInProgress = false;
			imageSelector.EndSelection();
			selectionStartedOutsideContent = false;
		}
		public PdfDocumentContent GetContentInfo(PdfDocumentPosition position) {
			PdfPoint point = position.Point;
			int pageIndex = position.PageIndex;
			bool selected = false;
			if (selectionState.HasSelection)
				foreach (PdfContentHighlight highlight in selectionState.Selection.Highlights) {
					if (highlight.PageIndex == pageIndex)
						foreach (PdfOrientedRectangle rectangle in highlight.Rectangles)
							if (rectangle.BoundingRectangle.Contains(point)) {
								selected = true;
								break;
							}
					if (selected)
						break;
				}
			if (pageIndex != -1)
				foreach (PdfAnnotationState annotationState in documentState.GetPageState(pageIndex).AnnotationStates)
					if (annotationState.Rectangle.Contains(point))
						return new PdfDocumentContent(position, PdfDocumentContentType.Annotation, selected);
			PdfDocumentContentType contentType;
			if (textSelector.HasContent(position))
				contentType = PdfDocumentContentType.Text;
			else if (imageSelector.HasContent(position))
				contentType = PdfDocumentContentType.Image;
			else
				contentType = PdfDocumentContentType.None;
			return new PdfDocumentContent(position, contentType, selected);
		}
		public PdfDocumentContent GetContentInfoWhileSelecting(PdfDocumentPosition position) {
			if (position.PageIndex < 0 || selectionStartedOutsideContent)
				return new PdfDocumentContent(position, PdfDocumentContentType.None, false);
			if (textSelector.SelectionInProgress)
				return new PdfDocumentContent(position, PdfDocumentContentType.Text, false);
			if (imageSelector.SelectionInProgress)
				return new PdfDocumentContent(position, PdfDocumentContentType.Image, false);
			PdfDocumentContent result = GetContentInfo(position);
			return new PdfDocumentContent(position, result.ContentType, result.IsSelected);
		}
		public PdfTextSelection GetTextSelection(PdfDocumentArea documentArea) {
			return textSelector.GetSelection(documentArea);
		}
		public PdfTextSelection GetTextSelection(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			return textSelector.GetSelection(textSelector.GetPageTextRanges(textSelector.FindClosestTextPosition(startPosition, new PdfTextPosition(startPosition.PageIndex, 1, 0)), endPosition));
		}
		public IList<PdfImageSelection> GetImagesSelection(PdfDocumentArea documentArea) {
			return imageSelector.SelectImages(documentArea);
		}
		public IList<PdfImageSelection> GetImagesSelection(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			return imageSelector.SelectImages(startPosition, endPosition);
		}
		PdfMovementDirection GetNormalizedDirection(PdfMovementDirection direction) {
			int angle;
			switch (direction) {
				case PdfMovementDirection.Left:
					angle = 0;
					break;
				case PdfMovementDirection.Down:
					angle = 90;
					break;
				case PdfMovementDirection.Right:
					angle = 180;
					break;
				default:
					angle = 270;
					break;
			}
			switch (PdfPageTreeNode.NormalizeRotate(angle + documentState.RotationAngle)) {
				case 0:
					return PdfMovementDirection.Left;
				case 90:
					return PdfMovementDirection.Down;
				case 180:
					return PdfMovementDirection.Right;
				default:
					return PdfMovementDirection.Up;
			}
		}
		bool IsArrowKeysNavigation(PdfMovementDirection direction) {
			return direction == PdfMovementDirection.Up || direction == PdfMovementDirection.Down || direction == PdfMovementDirection.Left || direction == PdfMovementDirection.Right;
		}
		void PerformSelection(PdfMouseAction mouseAction) {
			if (mouseAction.ModifierKeys.HasFlag(PdfModifierKeys.Shift) && selectionState.Caret != null) {
				textSelector.SelectionInProgress = true;
				PerformSelection(mouseAction.DocumentPosition);
			}
			else
				StartSelection(mouseAction.DocumentPosition);
		}
		PdfSelection GetImageSelection(PdfDocumentArea documentArea) {
			IList<PdfImageSelection> selections = GetImagesSelection(documentArea);
			return selections == null ? null : selections[0];
		}
	}
}
