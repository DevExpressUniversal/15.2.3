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
using System.Text;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPageWordBuilder {
		readonly PdfTextParserState parserState;
		StringBuilder wordBuilder;
		bool rtl;
		PdfCharacterCollection characters;
		IList<PdfWordPart> wordParts;
		double wordMinX;
		double wordMaxX;
		double wordMinY;
		double wordMaxY;
		double wordAngle;
		int wrapOffset;
		public PdfPageWordBuilder(PdfTextParserState parserState) {
			this.parserState = parserState;
		}
		public PdfWord BuildWord(double lineXOffset) {
			ClearWordPartData();
			wrapOffset = 0;
			wordParts = new List<PdfWordPart>();
			for (; ; ) {
				ProcessChar();
				parserState.MoveNext();
				if (parserState.IsSpace || parserState.IsSeparator || parserState.IsFinished) {
					PdfOrientedRectangle partRectangle = new PdfOrientedRectangle(PdfTextUtils.RotatePoint(new PdfPoint(wordMinX, wordMaxY), wordAngle), wordMaxX - wordMinX, wordMaxY - wordMinY, wordAngle);
					foreach (PdfCharacter ch in characters) {
						string unicodeData = ch.UnicodeData;
						rtl = PdfTextUtils.AppendText(wordBuilder, rtl, unicodeData == "\0" ? "\uFFFD" : unicodeData);
					}
					string partText = wordBuilder.ToString();
					bool wordWrap = parserState.IsWrap && lineXOffset <= partRectangle.Left;
					PdfWordPart wordPart = new PdfWordPart(partText, partRectangle, characters.ToList(), wrapOffset, !wordWrap && (parserState.IsSpace || parserState.IsFinished));
					if (wordPart.IsNotEmpty) {
						wordParts.Add(wordPart);
						if (wordWrap && wordPart.Length > 1 && !parserState.IsFinished) {
							wrapOffset += partText.Length;
							ClearWordPartData();
						}
						else
							return new PdfWord(wordParts);
					}
					else
						return null;
				}
			}
		}
		void ClearWordPartData() {
			wordBuilder = new StringBuilder();
			wordMinX = 0;
			wordMaxX = 0;
			wordMinY = 0;
			wordMaxY = 0;
			wordAngle = 0;
			wordBuilder = new StringBuilder();
			characters = new PdfCharacterCollection();
		}
		void ProcessChar() {
			PdfCharacter currentChar = parserState.CurrentCharacter;
			string unicodeData = currentChar.UnicodeData;
			if (unicodeData != "\u00a0" && unicodeData != " " && unicodeData != "\t") {
				PdfTextBlock block = parserState.CurrentCharacterBlock;
				double blockAngle = block.Angle;
				PdfPoint charLocation = PdfTextUtils.RotatePoint(parserState.CurrentCharLocation, -blockAngle);
				double minY = PdfTextUtils.RotatePoint(block.Location, -blockAngle).Y + block.FontDescent;
				double fontHeight = block.FontHeight;
				double maxY = minY + fontHeight;
				if (characters.Count == 0) {
					wordMinX = charLocation.X;
					wordMinY = minY;
					wordMaxY = maxY;
					wordAngle = blockAngle;
				}
				else {
					wordMinY = Math.Min(wordMinY, minY);
					wordMaxY = Math.Max(wordMaxY, maxY);
				}
				PdfOrientedRectangle charRect = currentChar.Rectangle;
				double charWidth = charRect.Width;
				if (charWidth != 0) {
					PdfRectangle charBox = charRect.BoundingRectangle;
					if (charBox != null && parserState.PageCropBox.Contains(charBox)) {
						wordMaxX = Math.Max(wordMaxX, charLocation.X + charWidth);
						wordMinX = Math.Min(charLocation.X, wordMinX);
						characters.Add(currentChar);
					}
				}
			}
		}
	}
}
