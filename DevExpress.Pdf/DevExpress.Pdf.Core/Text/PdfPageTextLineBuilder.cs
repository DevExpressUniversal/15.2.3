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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPageTextLineBuilder : IEnumerable<PdfTextLine> {
		readonly PdfTextParserState parserState;
		readonly PdfPageWordBuilder wordBuilder;
		int wordNumber = 1;
		PdfWordCollection words = new PdfWordCollection();
		IList<PdfWordPart> parts = new List<PdfWordPart>();
		double lineXOffset = double.MinValue;
		public PdfPageTextLineBuilder(PdfTextParserState parserState) {
			this.parserState = parserState;
			this.wordBuilder = new PdfPageWordBuilder(parserState);
		}
		IEnumerator<PdfTextLine> IEnumerable<PdfTextLine>.GetEnumerator() {
			for (; ; ) {
				PdfWord nextWord = wordBuilder.BuildWord(lineXOffset);
				if (nextWord != null) {
					if (!OverlapsWithPreviousWords(nextWord)) {
						words.Add(nextWord);
						IList<PdfWordPart> wordParts = nextWord.Parts;
						lineXOffset = Math.Max(lineXOffset, wordParts[0].Rectangle.Right);
						int partsCount = wordParts.Count;
						if (partsCount > 1) {
							EnumerateWordsAndFillParts();
							yield return new PdfTextLine(words, parts);
							words = new PdfWordCollection();
							for (int i = 1; i < partsCount - 1; i++)
								yield return new PdfTextLine(new List<PdfWord>(), new List<PdfWordPart>() { wordParts[i] });
							parts = new List<PdfWordPart>();
							parts.Add(wordParts[partsCount - 1]);
							lineXOffset = double.MinValue;
						}
					}
				}
				if (parserState.IsLineStart || parserState.IsFinished) {
					if (words.Count != 0 || parts.Count != 0) {
						EnumerateWordsAndFillParts();
						yield return new PdfTextLine(words, parts);
					}
					if (parserState.IsFinished)
						yield break;
					words = new PdfWordCollection();
					parts = new List<PdfWordPart>();
					lineXOffset = double.MinValue;
				}
			}
		}
		bool OverlapsWithPreviousWords(PdfWord word) {
			foreach (PdfWord prevWord in words)
				if (prevWord.Overlaps(word))
					return true;
			return false;
		}
		void EnumerateWordsAndFillParts() {
			foreach (PdfWord word in words) {
				word.WordNumber = wordNumber++;
				parts.Add(word.Parts[0]);
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<PdfTextLine>)this).GetEnumerator();
		}
	}
}
