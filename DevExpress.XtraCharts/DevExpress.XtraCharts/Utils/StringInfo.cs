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
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Text;
using DevExpress.Utils.Text.Internal;
namespace DevExpress.XtraCharts.Native {
	public class StringInfo {
		List<Font> fonts = new List<Font>();
		List<LineInfo> lines = new List<LineInfo>();
		RectangleF bounds;
		public List<LineInfo> Lines { get { return lines; } }
		public RectangleF Bounds { get { return bounds; } }
		public StringInfo(string text, ITextPropertiesProvider propertiesProvider, SizeF baseSize, TextMeasurer textMeasurer, float maxWidth, float maxHeight) {
			SplitLines(text, propertiesProvider, textMeasurer);
			Arrange(baseSize, propertiesProvider.Alignment);
			if (maxWidth > 0) {
				WrapLines(maxWidth);
				Arrange(baseSize, propertiesProvider.Alignment);
				if (maxHeight > 0 && maxHeight < bounds.Height) {
					double height = 0;
					int endLineNumber = 0;
					for (int i = 0; i < lines.Count; i++) {
						if (height + lines[i].Height < maxHeight)
							height += lines[i].Height;
						else {
							endLineNumber = i;
							break;
						}
					}
					lines.RemoveRange(endLineNumber, lines.Count - endLineNumber);
					if (lines.Count > 0)
						lines[lines.Count - 1].Trim(maxWidth);
					Arrange(baseSize, propertiesProvider.Alignment);
				}
			}
		}
		public StringInfo(string text, ITextPropertiesProvider propertiesProvider, SizeF baseSize, TextMeasurer textMeasurer, float maxWidth, int maxLineCount, bool wordWrap) {
			SplitLines(text, propertiesProvider, textMeasurer);
			Arrange(baseSize, propertiesProvider.Alignment);
			if (maxWidth > 0) {
				if (wordWrap)
					WrapLines(maxWidth);
				else {
					foreach (LineInfo trimedLine in lines)
						if (trimedLine.Width > maxWidth)
							trimedLine.Trim(maxWidth);
				}
				Arrange(baseSize, propertiesProvider.Alignment);
				if (maxLineCount > 0 && maxLineCount < lines.Count) {
					lines.RemoveRange(maxLineCount, lines.Count - maxLineCount);
					if (lines.Count > 0)
						lines[lines.Count - 1].Trim(maxWidth);
					Arrange(baseSize, propertiesProvider.Alignment);
				}
			}
		}
		void WrapLines(float maxWidth) {
			List<LineInfo> wrappedLines = new List<LineInfo>();
			foreach (LineInfo wrappedLine in lines)
				if (wrappedLine.Width > maxWidth)
					wrappedLines.AddRange(wrappedLine.Wrap(maxWidth));
				else
					wrappedLines.Add(wrappedLine);
			lines = wrappedLines;
		}
		void SplitLines(string text, ITextPropertiesProvider propertiesProvider, TextMeasurer textMeasurer) {
			List<StringBlock> stringBlocks = StringParser.Parse(propertiesProvider.Font.Size, text, true);
			LineInfo line = new LineInfo(textMeasurer);
			for (int i = 0; i < stringBlocks.Count; i++) {
				bool isFirstBlock = i == 0 || stringBlocks[i - 1].LineNumber != stringBlocks[i].LineNumber;
				bool isLastBlock = i == stringBlocks.Count - 1 || stringBlocks[i].LineNumber != stringBlocks[i + 1].LineNumber;
				Color fontColor = stringBlocks[i].FontSettings.Color != Color.Empty ?
					stringBlocks[i].FontSettings.Color : propertiesProvider.GetTextColor(stringBlocks[i].FontSettings.Color);
				FontStyle fontStyle = propertiesProvider.Font.Style | stringBlocks[i].FontSettings.Style;
				Font font = new Font(propertiesProvider.Font.FontFamily, stringBlocks[i].FontSettings.Size,
					fontStyle, propertiesProvider.Font.Unit);
				TextBlockInfo textBlock = new TextBlockInfo(stringBlocks[i].Text, font, fontColor, isFirstBlock, isLastBlock, textMeasurer);
				textBlock.IsHyperlink = stringBlocks[i].Type == StringBlockType.Link;
				textBlock.Hyperlink = stringBlocks[i].Link;
				if (i != 0 && stringBlocks[i].LineNumber != stringBlocks[i - 1].LineNumber) {
					lines.Add(line);
					line = new LineInfo(textMeasurer);
				}
				line.TextBlocks.Add(textBlock);
			}
			lines.Add(line);
		}
		void Arrange(SizeF baseSize, StringAlignment alignment) {
			float width = 0;
			float height = 0;
			foreach (LineInfo line in lines) {
				line.Calculate();
				height += line.Height;
				if (width < line.Width)
					width = line.Width;
			}
			float x = 0.5f * (baseSize.Width - width);
			float y = 0.5f * (baseSize.Height - height);
			bounds = new RectangleF(x, y, width, height);
			float bottom = -bounds.Height * 0.5f;
			foreach (LineInfo line in lines) {
				bottom += line.Height;
				float left = CalculateLineLeft(line, alignment);
				foreach (TextBlockInfo textBlock in line.TextBlocks) {
					float top = bottom - textBlock.Bounds.Height - line.RedLineOffset + textBlock.FontDescent;
					textBlock.SetLocation(new PointF(left, top));
					left += textBlock.Bounds.Width;
				}
			}
		}
		float CalculateLineLeft(LineInfo line, StringAlignment alignment) {
			switch (alignment) {
				case StringAlignment.Center:
					return -line.Width * 0.5f;
				case StringAlignment.Near:
					return -bounds.Width * 0.5f;
				case StringAlignment.Far:
					return bounds.Width * 0.5f - line.Width;
				default:
					ChartDebug.Fail("Unkown StringAlignment");
					return 0;
			}
		}
	}
	public class LineInfo {
		const string ellipsis = "...";
		float redLineOffset;
		float width;
		float height;
		List<TextBlockInfo> textBlocks = new List<TextBlockInfo>();
		TextMeasurer textMeasurer;
		public float RedLineOffset { get { return redLineOffset; } }
		public float Width { get { return width; } }
		public float Height { get { return height; } }
		public List<TextBlockInfo> TextBlocks { get { return textBlocks; } }
		public LineInfo(TextMeasurer textMeasurer) {
			this.textMeasurer = textMeasurer;
		}
		bool IsEmpty() {
			return textBlocks.Count == 1 && string.IsNullOrEmpty(textBlocks[0].Text.Trim());
		}
		void AddLine(List<LineInfo> lines, LineInfo line) {
			if (line.TextBlocks.Count > 0) {
				line.TextBlocks[line.textBlocks.Count - 1].SetIsLastBlock(true);
				lines.Add(line);
			}
		}
		public void Calculate() {
			redLineOffset = width = height = 0;
			foreach (TextBlockInfo textBlock in textBlocks) {
				width += textBlock.Bounds.Width;
				if (height < textBlock.Bounds.Height) {
					height = textBlock.Bounds.Height;
					redLineOffset = textBlock.FontDescent;
				}
			}
		}
		public void Trim(float maxWidth) {
			if(textBlocks.Count == 0)
				return;
			TextBlockInfo lastBlock = textBlocks[textBlocks.Count - 1];
			TextBlockInfo ellipsisWord = new TextBlockInfo(ellipsis, lastBlock.Font, lastBlock.FontColor, false, true, textMeasurer);
			float width = ellipsisWord.Bounds.Width;
			List<TextBlockInfo> ellipsisBlocks = new List<TextBlockInfo>();
			for (int i = 0; i < textBlocks.Count; i++) {
				if (textBlocks[i].Bounds.Width + width < maxWidth) {
					ellipsisBlocks.Add(textBlocks[i]);
					width += textBlocks[i].Bounds.Width;
				}
				else if (textBlocks[i].Text.Length != 1) {
					for (int j = 0; j < textBlocks[i].Text.Length; j++) {
						TextBlockInfo currentChar = new TextBlockInfo(textBlocks[i].Text[j].ToString(), textBlocks[i].Font,
							textBlocks[i].FontColor, ellipsisBlocks.Count == 0, false, textMeasurer);
						if (width + currentChar.Bounds.Width < maxWidth) {
							ellipsisBlocks.Add(currentChar);
							width += currentChar.Bounds.Width;
						}
						else
							break;
					}
					break;
				}
			}
			for (int i = ellipsisBlocks.Count - 1; i >= 0; i--)
				if (string.IsNullOrEmpty(ellipsisBlocks[i].Text.Trim()))
					ellipsisBlocks.RemoveAt(i);
				else
					break;
			textBlocks = ellipsisBlocks;
			if (textBlocks.Count == 0)
				ellipsisWord.SetIsFirstBlock(true);
			textBlocks.Add(ellipsisWord);
		}
		public List<LineInfo> Wrap(float maxWidth) {
			List<LineInfo> lines = new List<LineInfo>();
			List<TextBlockInfo> words = new List<TextBlockInfo>();
			foreach (TextBlockInfo textBlock in textBlocks)
				words.AddRange(textBlock.SplitByWords());
			LineInfo line = new LineInfo(textMeasurer);
			float lineWidth = 0;
			for (int i = 0; i < words.Count; i++) {
				bool isFirst = lineWidth == 0;
				TextBlockInfo testWord = new TextBlockInfo(words[i].Text, words[i].Font, words[i].FontColor, isFirst, true, textMeasurer);
				if (lineWidth + testWord.Bounds.Width < maxWidth) {
					TextBlockInfo word = new TextBlockInfo(words[i].Text, words[i].Font, words[i].FontColor, isFirst, false, textMeasurer);
					line.TextBlocks.Add(word);
					lineWidth += word.Bounds.Width;
				}
				else if (lineWidth == 0) {
					List<TextBlockInfo> chars = words[i].SplitByChars();
					if (chars.Count > 1) {
						words.RemoveAt(i);
						words.InsertRange(i, chars);
						i--;
					}
					else {
						line.TextBlocks.Add(words[i]);
						AddLine(lines, line);
						line = new LineInfo(textMeasurer);
					}
				}
				else {
					AddLine(lines, line);
					line = new LineInfo(textMeasurer);
					lineWidth = 0;
					i--;
				}
			}
			AddLine(lines, line);
			for (int i = 0; i < lines.Count; i++)
				if ((i == 0 || i == lines.Count - 1 || !lines[i + 1].IsEmpty()) && lines[i].IsEmpty()) {
					lines.RemoveAt(i);
					i--;
				}
			return lines;
		}
	}
	public class TextBlockInfo {
		bool isFirstBlock;
		bool isLastBlock;
		float fontDescent;
		string text;
		Color fontColor;
		Font font;
		RectangleF bounds;
		StringAlignment aligment = StringAlignment.Center;
		TextMeasurer textMeasurer;
		public bool IsFirstBlock { get { return isFirstBlock; } }
		public bool IsLastBlock { get { return isLastBlock; } }
		public bool IsHyperlink { get; set; }
		public float FontDescent { get { return fontDescent; } }
		public string Text { get { return text; } }
		public string Hyperlink { get; set; }
		public Color FontColor { get { return fontColor; } }
		public Font Font { get { return font; } }
		public RectangleF Bounds { get { return bounds; } }
		public StringAlignment Aligment { get { return aligment; } }
		public TextBlockInfo(string text, Font font, Color fontColor, bool isFirstBlock, bool isLastBlock, TextMeasurer textMeasurer) {
			this.isFirstBlock = isFirstBlock;
			this.isLastBlock = isLastBlock;
			this.text = text;
			this.fontColor = fontColor;
			this.font = font;
			this.textMeasurer = textMeasurer;
			Calculate();
		}
		void Calculate() {
			SizeF boundsSizeWithPaddings = textMeasurer.MeasureString(text, font);
			SizeF boundsSize = textMeasurer.MeasureStringTypographic(text, font);
			float height = boundsSizeWithPaddings.Height;
			float width = boundsSize.Width;
			float horizontalPadding = (boundsSizeWithPaddings.Width - boundsSize.Width) / 2;
			if (isFirstBlock && isLastBlock) {
				width = boundsSizeWithPaddings.Width;
				aligment = StringAlignment.Center;
			}
			else if (isFirstBlock) {
				width += horizontalPadding;
				aligment = StringAlignment.Far;
			}
			else if (isLastBlock) {
				width += horizontalPadding;
				aligment = StringAlignment.Near;
			}
			bounds = new RectangleF(0, 0, width, height);
			fontDescent = bounds.Height - textMeasurer.GetFontAscentHeight(font);
		}
		public void SetLocation(PointF location) {
			bounds.Location = location;
		}
		public void SetIsFirstBlock(bool isFirstBlock) {
			if (this.isFirstBlock != isFirstBlock) {
				this.isFirstBlock = isFirstBlock;
				Calculate();
			}
		}
		public void SetIsLastBlock(bool isLastBlock) {
			if (this.isLastBlock != isLastBlock) {
				this.isLastBlock = isLastBlock;
				Calculate();
			}
		}
		public List<TextBlockInfo> SplitByWords() {
			List<TextBlockInfo> words = new List<TextBlockInfo>();
			if (text.Length == 1)
				words.Add(this);
			else {
				string word = string.Empty;
				for (int i = 0; i < text.Length; i++) {
					bool isFirst = isFirstBlock && i == 0;
					if (string.IsNullOrEmpty(text[i].ToString().Trim())) {
						if (!string.IsNullOrEmpty(word))
							words.Add(new TextBlockInfo(word, font, FontColor, isFirst, false, textMeasurer));
						word = string.Empty;
						words.Add(new TextBlockInfo(text[i].ToString(), font, FontColor, isFirst, false, textMeasurer));
					}
					else
						word += text[i].ToString();
					if (i == text.Length - 1 && !string.IsNullOrEmpty(word))
						words.Add(new TextBlockInfo(word, font, FontColor, isFirst, false, textMeasurer));
				}
			}
			return words;
		}
		public List<TextBlockInfo> SplitByChars() {
			List<TextBlockInfo> chars = new List<TextBlockInfo>();
			if (text.Length == 1)
				chars.Add(this);
			else
				for (int i = 0; i < text.Length; i++) {
					bool isFirst = isFirstBlock && i == 0;
					chars.Add(new TextBlockInfo(text[i].ToString(), font, FontColor, isFirst, false, textMeasurer));
				}
			return chars;
		}
	}
}
