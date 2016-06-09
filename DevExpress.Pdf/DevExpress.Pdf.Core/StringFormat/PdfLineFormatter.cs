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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfLineFormatter {
		readonly PdfLineFormatterState state;
		readonly int spaceGlyphIndex;
		readonly PdfLineTrimmingAlgorithm trimmingAlgorithm;
		readonly bool noWrap;
		readonly bool measureTrailingSpaces;
		readonly int layoutLineCount;
		bool ShouldTrimCurrentLine { get { return noWrap || state.Lines.Count == layoutLineCount - 1; } }
		public PdfLineFormatter(PdfLineFormatterState state, int spaceGlyphIndex, int layoutLineCount, PdfStringFormat format, PdfStringGlyphRun ellipsis) {
			this.state = state;
			this.spaceGlyphIndex = spaceGlyphIndex;
			this.layoutLineCount = layoutLineCount;
			noWrap = format.FormatFlags.HasFlag(PdfStringFormatFlags.NoWrap);
			measureTrailingSpaces = format.FormatFlags.HasFlag(PdfStringFormatFlags.MeasureTrailingSpaces);
			trimmingAlgorithm = PdfLineTrimmingAlgorithm.Create(state, ellipsis, format.Trimming);
		}
		public void FormatLine(PdfStringGlyphRun line) {
			IList<PdfStringGlyphRun> words = line.GetWords(spaceGlyphIndex, measureTrailingSpaces);
			int count = words.Count;
			for (int i = 0; i < count; i++) {
				PdfStringGlyphRun word = words[i];
				if (state.Lines.Count != 0 && state.IsCurrentLineEmpty)
					word = PadLeft(word);
				if (ShouldTrimCurrentLine && trimmingAlgorithm != null) {
					if (trimmingAlgorithm.ProcessWord(word))
						break;
				}
				else {
					if (!noWrap && word.Width > state.LayoutWidth)
						ProcessByChar(word);
					else
						ProcessByWord(word);
				}
			}
			state.FinishLine();
		}
		PdfStringGlyphRun PadLeft(PdfStringGlyphRun word) {
			PdfStringGlyphRun paddedWord = new PdfStringGlyphRun();
			IList<PdfStringGlyph> glyphs = word.Glyphs;
			int count = glyphs.Count;
			int spaceCount = 0;
			for (int i = 0; i < count; i++)
				if (glyphs[i].GlyphIndex != spaceGlyphIndex) {
					spaceCount = i;
					break;
				}
			if (spaceCount == count - 1)
				return word;
			for (int i = spaceCount; i < count; i++)
				paddedWord.Append(glyphs[i]);
			return paddedWord;
		}
		void ProcessByWord(PdfStringGlyphRun word) {
			if (!noWrap && state.CurrentLineWidth + word.Width > state.LayoutWidth) {
				state.FinishLine();
				word = PadLeft(word);
			}
			state.Append(word);
		}
		void ProcessByChar(PdfStringGlyphRun word) {
			if (state.IsCurrentLineEmpty && state.Lines.Count != 0)
				word = PadLeft(word);
			foreach (PdfStringGlyph glyph in word.Glyphs) {
				if (state.IsCurrentLineEmpty)
					state.Append(glyph);
				else {
					if (state.CurrentLineWidth + glyph.Width > state.LayoutWidth)
						state.FinishLine();
					state.Append(glyph);
				}
			}
		}
	}
}
