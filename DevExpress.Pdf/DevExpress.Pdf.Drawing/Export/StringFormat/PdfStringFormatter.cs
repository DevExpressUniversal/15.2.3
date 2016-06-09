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
using System.Drawing;
using System.Drawing.Text;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfStringFormatter {
		static string[] lineEndings = new string[] { "\r\n", "\r", "\n" };
		static string ProcessHotkeyPrefixes(string text, PdfHotkeyPrefix prefix) {
			const char hotkeyPrefixSymbol = '&';
			if (prefix == PdfHotkeyPrefix.None)
				return text;
			bool inPrefix = false;
			StringBuilder builder = new StringBuilder();
			foreach (char ch in text)
				if (!inPrefix && ch == hotkeyPrefixSymbol)
					inPrefix = true;
				else {
					builder.Append(ch);
					inPrefix = false;
				}
			return builder.ToString();
		}
		static IList<PdfStringGlyphRun> LimitLines(IList<PdfStringGlyphRun> result, int lineCount) {
			if (result.Count < lineCount)
				return result;
			IList<PdfStringGlyphRun> lines = new List<PdfStringGlyphRun>(lineCount);
			for (int i = 0; i < lineCount; i++)
				lines.Add(result[i]);
			return lines;
		}
		readonly PdfEditableFontData fontData;
		readonly double fontSize;
		readonly PdfStringGlyphRun ellipsis;
		readonly int spaceGlyphIndex;
		public PdfStringFormatter(PdfEditableFontData fontData, double fontSize) {
			this.fontData = fontData;
			this.fontSize = fontSize;
			PdfGlyphMappingResult ellipsisMapping = fontData.Mapper.MapString("...", false);
			ellipsis = ellipsisMapping.GlyphRun;
			fontData.RegisterString(ellipsisMapping.Mapping);
			PdfGlyphMappingResult space = fontData.Mapper.MapString(" ", false);
			fontData.RegisterString(space.Mapping);
			IList<PdfStringGlyph> spaceGlyphs = space.GlyphRun.Glyphs;
			spaceGlyphIndex = (spaceGlyphs == null || spaceGlyphs.Count == 0) ? 32 : spaceGlyphs[0].GlyphIndex;
		}
		public IList<PdfStringGlyphRun> FormatString(string str, PdfPoint point, PdfStringFormat format, bool useKerning) {
			IPdfGlyphMapper mapper = fontData.Mapper;
			str = ProcessHotkeyPrefixes(str, format.HotkeyPrefix);
			string[] splittedStr = str.Split(lineEndings, StringSplitOptions.None);
			List<PdfStringGlyphRun> result = new List<PdfStringGlyphRun>();
			foreach (string line in splittedStr) {
				PdfGlyphMappingResult processedString = mapper.MapString(line, useKerning);
				fontData.RegisterString(processedString.Mapping);
				result.Add(processedString.GlyphRun);
			}
			return result;
		}
		public IList<PdfStringGlyphRun> FormatString(string str, PdfRectangle layoutRect, PdfStringFormat format, bool useKerning) {
			IPdfGlyphMapper mapper = fontData.Mapper;
			str = ProcessHotkeyPrefixes(str, format.HotkeyPrefix);
			string[] splittedStr = str.Split(lineEndings, StringSplitOptions.None);
			PdfFontMetrics metrics = fontData.Metrics;
			double lineHeight = metrics.GetLineSpacing(fontSize);
			double firstLineHeight = metrics.GetAscent(fontSize) + metrics.GetDescent(fontSize);
			double layoutHeight = layoutRect.Height;
			bool limitLines = format.FormatFlags.HasFlag(PdfStringFormatFlags.LineLimit);
			int lineCount = 0;
			if (layoutHeight < firstLineHeight)
				lineCount = limitLines ? 0 : 1;
			else {
				double remainingLinesHeight = layoutHeight - firstLineHeight;
				lineCount = 1 + (int)(limitLines ? Math.Floor(remainingLinesHeight / lineHeight) : Math.Ceiling(remainingLinesHeight / lineHeight));
			}
			limitLines = limitLines || format.Trimming != PdfStringTrimming.None;
			double emFactor = 1000 / fontSize;
			double leftMargin = format.LeadingMarginFactor * lineHeight * emFactor;
			double rightMargin = format.TrailingMarginFactor * lineHeight * emFactor;
			double left = leftMargin + layoutRect.Left * emFactor;
			double bottom = layoutRect.Bottom * emFactor + leftMargin;
			double right = layoutRect.Right * emFactor - rightMargin;
			double top = layoutRect.Top * emFactor - rightMargin;
			PdfRectangle emLayout = new PdfRectangle(left, bottom, right < left ? left : right, top < bottom ? bottom : top);
			PdfLineFormatterState state = CreateState(emLayout);
			PdfLineFormatter formatter = new PdfLineFormatter(state, spaceGlyphIndex, lineCount, format, ellipsis);
			foreach (string line in splittedStr) {
				PdfGlyphMappingResult processedString = mapper.MapString(line, useKerning);
				fontData.RegisterString(processedString.Mapping);
				formatter.FormatLine(processedString.GlyphRun);
				if (limitLines && state.Lines.Count >= lineCount)
					break;
			}
			IList<PdfStringGlyphRun> formattedLines = state.Lines;
			if (limitLines)
				formattedLines = LimitLines(formattedLines, lineCount);
			return formattedLines;
		}
		protected virtual PdfLineFormatterState CreateState(PdfRectangle emLayoutRect) {
			return new PdfLineFormatterState(emLayoutRect);
		}
	}
}
