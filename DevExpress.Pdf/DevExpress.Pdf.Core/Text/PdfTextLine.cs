#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public class PdfTextLine : List<PdfWord> {
		readonly IList<PdfWordPart> wordParts;
		PdfOrientedRectangle rectangle;
		public IList<PdfWordPart> WordParts { get { return wordParts; } }
		public PdfOrientedRectangle Rectangle {
			get {
				if (rectangle == null)
					rectangle = GetHighlightRectangles(-1, 0, wordParts.Count - 1, -1, false)[0];
				return rectangle;
			}
		}
		public PdfTextLine(IEnumerable<PdfWord> words, IEnumerable<PdfWordPart> wordParts)
			: base(words) {
			this.wordParts = new List<PdfWordPart>(wordParts);
		}
		public IList<PdfOrientedRectangle> GetHighlightRectangles(int sWordIndex, int sOffset, int eWordIndex, int eOffset, bool splitRectangles) {
			IList<PdfCharacter> startCharacters = wordParts[sWordIndex == -1 ? 0 : sWordIndex].Characters;
			IList<PdfOrientedRectangle> rectangles = new List<PdfOrientedRectangle>();
			int startOffset = sOffset;
			if (sOffset > startCharacters.Count)
				return null;
			bool startOnEndOfWord = sOffset == startCharacters.Count;
			if (startOnEndOfWord)
				startOffset--;
			PdfOrientedRectangle startCharRect = startCharacters[startOffset].Rectangle;
			double angle = startCharRect.Angle;
			PdfPoint topLeft = PdfTextUtils.RotatePoint(startCharRect.TopLeft, -angle);
			double left = startOnEndOfWord ? topLeft.X + startCharRect.Width : topLeft.X;
			double top = topLeft.Y;
			double right = left;
			double bottom = top - startCharRect.Height;
			if (sWordIndex == -1)
				sWordIndex++;
			for (int i = sWordIndex; i <= eWordIndex; i++) {
				IList<PdfCharacter> characters = wordParts[i].Characters;
				int start = i == sWordIndex ? sOffset : 0;
				int end = i == eWordIndex && eOffset != -1 ? eOffset : characters.Count;
				if (splitRectangles && (start == 0 || start < end)) {
					PdfOrientedRectangle charRect = characters[start].Rectangle;
					PdfPoint charTopLeft = PdfTextUtils.RotatePoint(charRect.TopLeft, -angle);
					if (charTopLeft.X > right + 2 * charRect.Width) {
						rectangles.Add(new PdfOrientedRectangle(PdfTextUtils.RotatePoint(new PdfPoint(left, top), angle), right - left, top - bottom, angle));
						left = charTopLeft.X;
						top = charTopLeft.Y;
						right = left;
						bottom = top - charRect.Height;
					}
				}
				if (end == 0)
					right = PdfMathUtils.Max(right, PdfTextUtils.RotatePoint(wordParts[i].Characters[0].Rectangle.TopLeft, -angle).X);
				else
					for (int j = start; j < end; j++) {
						PdfOrientedRectangle charRect = characters[j].Rectangle;
						PdfPoint charTopLeft = PdfTextUtils.RotatePoint(charRect.TopLeft, -angle);
						left = PdfMathUtils.Min(charTopLeft.X, left);
						top = PdfMathUtils.Max(charTopLeft.Y, top);
						right = PdfMathUtils.Max(right, charTopLeft.X + charRect.Width);
						bottom = PdfMathUtils.Min(bottom, charTopLeft.Y - charRect.Height);
					}
			}
			rectangles.Add(new PdfOrientedRectangle(PdfTextUtils.RotatePoint(new PdfPoint(left, top), angle), right - left, top - bottom, angle));
			return rectangles;
		}
		public IList<PdfOrientedRectangle> GetHighlightRectangles(int sWordIndex, int sOffset, bool splitRectangles) {
			return GetHighlightRectangles(sWordIndex, sOffset, wordParts.Count - 1, -1, splitRectangles);
		}
		public bool IsPositionInLine(int wordNumber, int offset) {
			int wordPartCount = wordParts.Count;
			for (int i = 0; i < wordPartCount; i++) {
				int charCount = wordParts[i].Characters.Count;
				if (wordNumber == wordParts[i].WordNumber) {
					if (i == 0) {
						int wrapOffset = wordParts[i].WrapOffset;
						return wrapOffset <= offset && offset - wrapOffset <= charCount;
					}
					else
						return offset <= charCount;
				}
			}
			return false;
		}
		public PdfPageTextRange GetTextRange(int pageIndex, PdfRectangle area) {
			PdfPageTextPosition startPosition = null;
			PdfPageTextPosition endPosition = null;
			foreach (PdfWordPart word in wordParts) {
				IList<PdfCharacter> characters = word.Characters;
				int count = characters.Count;
				for (int i = 0; i < count; i++) {
					if (area.Intersects(characters[i].Rectangle.BoundingRectangle)) {
						if (startPosition == null)
							startPosition = new PdfPageTextPosition(word.WordNumber, i);
						endPosition = new PdfPageTextPosition(word.WordNumber, i + 1);
					}
				}
			}
			return startPosition == null || endPosition == null ? null : new PdfPageTextRange(pageIndex, startPosition, endPosition);
		}
		public PdfTextPosition GetTextPosition(int pageIndex, PdfPoint point) {
			if (wordParts.Count < 1)
				return null;
			PdfWordPart closestWordPart = null;
			double angle = wordParts[0].Rectangle.Angle;
			foreach (PdfWordPart word in wordParts) {
				PdfOrientedRectangle wordRectangle = word.Rectangle;
				double wordRotatedLeft = PdfTextUtils.RotatePoint(wordRectangle.TopLeft, -angle).X;
				double X = PdfTextUtils.RotatePoint(point, -angle).X;
				if (wordRotatedLeft + wordRectangle.Width < X)
					closestWordPart = word;
				else {
					PdfOrientedRectangle rect = closestWordPart == null ? null : closestWordPart.Rectangle;
					if (closestWordPart == null || word.Rectangle.PointIsInRect(point) || Math.Abs(PdfTextUtils.RotatePoint(rect.TopLeft, -angle).X + rect.Width - X) >= Math.Abs(wordRotatedLeft - X))
						closestWordPart = word;
					IList<PdfCharacter> characters = closestWordPart.Characters;
					int count = characters.Count;
					for (int j = 0; j < count; j++) {
						PdfOrientedRectangle characterRect = characters[j].Rectangle;
						if (PdfTextUtils.RotatePoint(characterRect.TopLeft, -angle).X + characterRect.Width / 2 >= PdfTextUtils.RotatePoint(point, -angle).X)
							return new PdfTextPosition(pageIndex, closestWordPart.WordNumber, j + closestWordPart.WrapOffset);
					}
				}
			}
			return new PdfTextPosition(pageIndex, closestWordPart.WordNumber, closestWordPart.WrapOffset + closestWordPart.Characters.Count);
		}
	}
}
