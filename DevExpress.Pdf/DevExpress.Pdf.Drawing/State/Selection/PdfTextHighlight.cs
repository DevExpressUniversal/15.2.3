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
	public class PdfTextHighlight : PdfContentHighlight {
		readonly PdfPageDataCache pageDataCache;
		readonly PdfPageTextRange wordRange;
		IList<PdfOrientedRectangle> rectangles;
		public override IList<PdfOrientedRectangle> Rectangles {
			get {
				if (rectangles == null) {
					rectangles = new List<PdfOrientedRectangle>();
					int start = wordRange.StartWordNumber;
					bool noStartWord = start == 0;
					int end = wordRange.EndWordNumber;
					bool noEndWord = end == 0;
					List<PdfTextLine> lines = new List<PdfTextLine>(pageDataCache.GetPageLines(wordRange.PageIndex));
					if ((noStartWord && noEndWord) || wordRange.WholePage) {
						foreach (PdfTextLine line in lines)
							rectangles.Add(line.Rectangle);
					}
					else {
						int startRangeOffset = wordRange.StartOffset;
						int endRangeOffset = wordRange.EndOffset;
						foreach (PdfTextLine line in lines) {
							IList<PdfWordPart> wordParts = line.WordParts;
							int wordPartsCount = wordParts.Count;
							if (wordPartsCount > 0) {
								int firstWordPartNumber = wordParts[0].WordNumber;
								bool isEndInLine = line.IsPositionInLine(wordRange.EndWordNumber, endRangeOffset);
								int endWordIndex;
								int endOffset;
								if (isEndInLine) {
									endWordIndex = end - firstWordPartNumber;
									endOffset = endRangeOffset - wordParts[endWordIndex].WrapOffset;
								}
								else {
									endWordIndex = 0;
									endOffset = 0;
								}
								IList<PdfOrientedRectangle> highlightRectangles = null;
								if (line.IsPositionInLine(wordRange.StartWordNumber, startRangeOffset)) {
									int startWordIndex = start - firstWordPartNumber;
									int startOffset = startRangeOffset - wordParts[startWordIndex].WrapOffset;
									highlightRectangles = isEndInLine ? line.GetHighlightRectangles(startWordIndex, startOffset, endWordIndex, endOffset, true) : 
										line.GetHighlightRectangles(startWordIndex, startOffset, true);
								}
								else if (isEndInLine)
									highlightRectangles = line.GetHighlightRectangles(0, 0, endWordIndex, endOffset, true);
								else {
									PdfWordPart lastWordPart = wordParts[wordPartsCount - 1];
									int lastWordPartNumber = lastWordPart.WordNumber;
									if ((noStartWord || start <= firstWordPartNumber) && (noEndWord || lastWordPartNumber - 1 <= end))
										highlightRectangles = line.GetHighlightRectangles(0, 0, lastWordPartNumber - firstWordPartNumber, lastWordPart.Characters.Count, true);
								}
								if (highlightRectangles != null) {
									foreach (PdfOrientedRectangle rect in highlightRectangles)
										if (rect.Width > 0)
											rectangles.Add(rect);
									if (isEndInLine)
										break;
								}
							}
						}
					}   
				}   
				return rectangles;
			}
		}
		public PdfTextHighlight(PdfPageDataCache pageDataCache, PdfPageTextRange wordRange) : base(wordRange.PageIndex) {
			this.pageDataCache = pageDataCache;
			this.wordRange = wordRange;
		}
	}
}
