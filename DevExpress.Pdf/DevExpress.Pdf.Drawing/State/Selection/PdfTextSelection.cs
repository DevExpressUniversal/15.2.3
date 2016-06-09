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
using System.Text;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfTextSelection : PdfSelection {
		public static bool AreEqual(PdfTextSelection txt1, PdfTextSelection txt2) {
			if (txt1 == null)
				return txt2 == null;
			if (txt2 == null)
				return false;
			IList<PdfPageTextRange> textRange1 = txt1.textRange;
			IList<PdfPageTextRange> textRange2 = txt2.textRange;
			int count = textRange1.Count;
			if (count != textRange2.Count)
				return false;
			for (int i = 0; i < count; i++)
				if (!PdfPageTextRange.AreEqual(textRange1[i], textRange2[i]))
					return false;
			return true;
		}
		readonly PdfPageDataCache pageDataCache;
		readonly IList<PdfPageTextRange> textRange;
		string text;
		IList<PdfContentHighlight> highlights;
		public IList<PdfPageTextRange> TextRange { get { return textRange; } }
		public string Text {
			get {
				if (text == null) {
					StringBuilder textBuilder = new StringBuilder();
					int count = textRange.Count;
					for (int i = 0; i < count; i++) {
						PdfPageTextRange pageTextRange = textRange[i];
						int start = pageTextRange.StartWordNumber;
						bool noStartWord = start == 0;
						int end = pageTextRange.EndWordNumber;
						bool noEndWord = end == 0;
						int startRangeOffset = pageTextRange.StartOffset;
						int endRangeOffset = pageTextRange.EndOffset;
						foreach (PdfTextLine line in pageDataCache.GetPageLines(pageTextRange.PageIndex)) {
							StringBuilder lineBuilder = new StringBuilder();
							bool pasteNewLine = false;
							foreach (PdfWordPart wordPart in line.WordParts) {
								int wordNumber = wordPart.WordNumber;
								try {
									int wrapOffset = wordPart.WrapOffset;
									int wordPartLength = wordPart.Text.Length;
									int startOffset = startRangeOffset - wrapOffset < 0 ? 0 : startRangeOffset - wrapOffset;
									int endOffset = wordPartLength < endRangeOffset - wrapOffset ? wordPartLength : endRangeOffset - wrapOffset;
									StringBuilder wordText = new StringBuilder();
									bool rtl = false;
									pasteNewLine = wordNumber == end && (endOffset == wordPartLength || endRangeOffset < 0);
									if ((noStartWord && noEndWord) || (noStartWord && wordNumber < end) || (noEndWord && wordNumber > start) || (wordNumber > start && wordNumber < end)) {
									   rtl = PdfTextUtils.AppendText(wordText, rtl, wordPart.Text);
										if (wordPart.WordEnded)
											rtl = PdfTextUtils.AppendText(wordText, rtl, " ");
									}
									else if (wordNumber == end && wordNumber == start) {
										if (startRangeOffset == 0 && endRangeOffset < 0)
											rtl = PdfTextUtils.AppendText(wordText, rtl, wordPart.Text);
										else if (startOffset <= wordPartLength && endOffset >= 0)
											rtl = PdfTextUtils.AppendText(wordText, rtl, PdfTextUtils.Substring(wordPart.Text, startOffset, endOffset - startOffset));
									}
									else if ((noStartWord || wordNumber > start) && wordNumber == end) {
										if (endRangeOffset < 0)
											rtl = PdfTextUtils.AppendText(wordText, rtl, wordPart.Text);
										else if (endOffset >= 0)
											rtl = PdfTextUtils.AppendText(wordText, rtl, PdfTextUtils.Substring(wordPart.Text, 0, endOffset));
									}
									else if ((noEndWord || wordNumber < end) && wordNumber == start && startOffset <= wordPartLength) {
										rtl = PdfTextUtils.AppendText(wordText, rtl, PdfTextUtils.Substring(wordPart.Text, startOffset));
										if (wordPart.WordEnded)
											rtl = PdfTextUtils.AppendText(wordText, rtl, " ");
									}
									if (wordText.Length > 0)
										rtl = PdfTextUtils.AppendText(lineBuilder, rtl, wordText.ToString());
								}
								catch {
								}
							}
							int endWordNumber = line.WordParts[line.WordParts.Count - 1].WordNumber;
							PdfTextUtils.AppendText(textBuilder, false, lineBuilder.ToString().Trim());
							if (lineBuilder.Length > 0 && endWordNumber >= start && (endWordNumber < end || pasteNewLine || noEndWord))
								PdfTextUtils.AppendText(textBuilder, false, Environment.NewLine);
						}
						if (i != count - 1 && pageTextRange.PageIndex == textRange[i + 1].PageIndex && textBuilder.Length != 0 && textBuilder[textBuilder.Length - 1] != '\n')
							PdfTextUtils.AppendText(textBuilder, false, Environment.NewLine);
					}
					text = textBuilder.ToString();
				}
				return text;
			}
		}
		public override PdfDocumentContentType ContentType { get { return PdfDocumentContentType.Text; } }
		public override IList<PdfContentHighlight> Highlights {
			get {
				if (highlights == null) {
					highlights = new List<PdfContentHighlight>();
					foreach (PdfPageTextRange wordRange in textRange)
						highlights.Add(new PdfTextHighlight(pageDataCache, wordRange));
				}
				return highlights;
			}
		}
		public PdfTextSelection(PdfPageDataCache pageDataCache, IList<PdfPageTextRange> textRange) {
			this.pageDataCache = pageDataCache;
			this.textRange = textRange;
		}
		public override PdfRectangle GetBoundingBox(int pageIndex) {
			double minx = double.MaxValue;
			double miny = double.MaxValue;
			double maxx = double.MinValue;
			double maxy = double.MinValue;
			foreach (PdfContentHighlight contentHighlight in Highlights) {
				if (contentHighlight.PageIndex == pageIndex && contentHighlight.Rectangles.Count > 0) {
					foreach (PdfOrientedRectangle rect in contentHighlight.Rectangles) {
						minx = Math.Min(minx, rect.Left);
						miny = Math.Min(miny, rect.Bottom);
						maxx = Math.Max(maxx, rect.Right);
						maxy = Math.Max(maxy, rect.Top);
					}
				}
			}
			try {
				return new PdfRectangle(minx, miny, maxx, maxy);
			}
			catch {
				return null;
			}
		}
	}
}
