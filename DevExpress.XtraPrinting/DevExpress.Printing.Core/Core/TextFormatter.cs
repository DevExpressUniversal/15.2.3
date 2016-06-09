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
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Collections;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraPrinting.Native {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, (TextWithSpaces={TextWithSpaces})}")]
#endif
	public class Word {
		string text;
		int leadSpaceCount;
		public string Text { get { return text; } }
		public string TextWithSpaces {
			get {
				string spaces = new String(' ', this.leadSpaceCount);
				return spaces + this.text;
			}
		}
		public int LeadSpaceCount { get { return leadSpaceCount; } }
		public Word(string text)
			: this(text, 1) {
		}
		public Word(string text, int leadSpaceCount) {
			this.text = text;
			this.leadSpaceCount = leadSpaceCount;
		}
		public void IncrementLeadSpaceCount() {
			this.leadSpaceCount++;
		}
		public void DecrementLeadSpaceCount() {
			this.leadSpaceCount--;
		}
		public override bool Equals(object obj) {
			Word word = obj as Word;
			return
				word != null &&
				word.Text == Text &&
				word.LeadSpaceCount == LeadSpaceCount;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public enum WordFormatMode {
		FirstLine,
		NewLine,
		Line,
		ByChar
	}
	public class FontMetrics {
		public static FontMetrics CreateInstance(Font font, GraphicsUnit pageUnit) {
			return font != null ? new FontMetrics(font, pageUnit) : new FontMetrics();
		}
		float ascent;
		float descent;
		float lineSpacing;
		float fontDpi;
		float pageDpi;
		public FontMetrics() {
		}
		public FontMetrics(Font font, GraphicsUnit pageUnit) {
			this.ascent = FontHelper.GetCellAscent(font);
			this.descent = FontHelper.GetCellDescent(font);
			this.lineSpacing = FontHelper.GetLineSpacing(font);
			this.fontDpi = GraphicsDpi.UnitToDpiI(font.Unit);
			this.pageDpi = GraphicsDpi.UnitToDpiI(pageUnit);
		}
		public float CalculateHeight(int lineCount) {
			float fontLineHeight = this.lineSpacing * (lineCount - 1) + this.ascent + this.descent;
			return GraphicsUnitConverter.Convert(fontLineHeight, fontDpi, pageDpi);
		}
	}
	public abstract class WordFormatterAlgorithm {
		protected WordFormatter formatter;
		public WordFormatterAlgorithm(WordFormatter formatter) {
			this.formatter = formatter;
		}
		public abstract bool ApplyWordToLines(Word word);
		public abstract void FormatLastLine(Word word);
		public abstract void SetNextMode(Word word);
		public abstract bool SupportsMode(WordFormatMode mode);
	}
	public class WordFormatter : IDisposable {
		#region inner classes
		protected class WordFormatterLineAlgorithm : WordFormatterAlgorithm {
			public WordFormatterLineAlgorithm(WordFormatter formatter)
				: base(formatter) {
			}
			public override bool ApplyWordToLines(Word word) {
				if(word == null)
					throw new ArgumentNullException("word");
				string text = formatter.GetLine(word);
				if(!TextExceedWidth(text)) {
					formatter.SetLine(text);
					return true;
				}
				return false;
			}
			bool TextExceedWidth(string text) {
				bool noWrap = (formatter.StringFormat.FormatFlags & StringFormatFlags.NoWrap) != 0;
				return noWrap ? 
					!FirstLessOrEqualSecond(formatter.MeasureTextWidth(text), formatter.width) :
					formatter.IsTextWrapped(text, formatter.width);
			}
			public override void FormatLastLine(Word word) {
				if(formatter.TrimByCharacter)
					formatter.FormatLastWordOfLastLine(word);
			}
			public override void SetNextMode(Word word) {
				formatter.mode = GetNextMode(word, formatter.mode);
				if(formatter.mode != WordFormatMode.ByChar)
					formatter.UpdateLines();
			}
			WordFormatMode GetNextMode(Word word, WordFormatMode prevMode) {
				if(GotoNewLineNeeded(word, prevMode))
					return WordFormatMode.NewLine;
				else if(TryFormatByCharNeeded(word, prevMode))
					return WordFormatMode.ByChar;
				else
					throw new WordFormatterException();
			}
			protected virtual bool GotoNewLineNeeded(Word word, WordFormatMode prevMode) {
				return
					(prevMode == WordFormatMode.FirstLine && word.LeadSpaceCount > 0) ||
					(prevMode == WordFormatMode.Line);
			}
			protected virtual bool TryFormatByCharNeeded(Word word, WordFormatMode prevMode) {
				return
					(prevMode == WordFormatMode.FirstLine && word.LeadSpaceCount == 0) ||
					(prevMode == WordFormatMode.NewLine);
			}
			public override bool SupportsMode(WordFormatMode mode) {
				return mode != WordFormatMode.ByChar;
			}
		}
		protected class WordFormatterCharAlgorithm : WordFormatterAlgorithm {
			string text = String.Empty;
			int charIndex = 0;
			public WordFormatterCharAlgorithm(WordFormatter formatter)
				: base(formatter) {
			}
			public override bool ApplyWordToLines(Word word) {
				if(word == null)
					throw new ArgumentNullException("word");
				char ch = word.Text[charIndex];
				if(FirstLessOrEqualSecond(formatter.MeasureTextWidth(text + ch), formatter.width)) {
					ApplyChar(ch);
				} else {
					if(string.IsNullOrEmpty(text))
						ApplyChar(ch);
					formatter.lines.Add(text);
					text = String.Empty;
				}
				if(charIndex < word.Text.Length)
					return false;
				System.Diagnostics.Debug.Assert(string.IsNullOrEmpty(formatter.line));
				formatter.SetLine(text);
				return true;
			}
			void ApplyChar(char ch) {
				text += ch;
				charIndex++;
			}
			public override void FormatLastLine(Word word) {
				formatter.LastLine = formatter.TrimByEllipsisCharacter(formatter.LastLine);
			}
			public override void SetNextMode(Word word) {
			}
			public override bool SupportsMode(WordFormatMode mode) {
				return mode == WordFormatMode.ByChar;
			}
		}
		#endregion
		const string ellipsis = "...";
		static bool FirstLessOrEqualSecond(double number1, double number2) {
			return CompareDoubles(number1, number2) <= 0;
		}
		static int CompareDoubles(double number1, double number2) {
			double epsilon = 0.01;
			if(double.IsNegativeInfinity(number1) && double.IsNegativeInfinity(number2))
				return 0;
			else if(double.IsPositiveInfinity(number1) && double.IsPositiveInfinity(number2))
				return 0;
			else {
				double difference = number1 - number2;
				if(Math.Abs(difference) <= epsilon)
					return 0;
				else if(difference > 0)
					return 1;
				else
					return -1;
			}
		}
		static float MeasureTextWidth(string text, Measurer measurer, Font font, StringFormat stringFormat, GraphicsUnit pageUnit) {
			SizeF sizeF = measurer.MeasureStringI(text, font, 0.0f, stringFormat, pageUnit);
			float width = sizeF.Width;
			float height = sizeF.Height;
			TextFormatter.RotateBasis(stringFormat, ref width, ref height);
			return width;
		}
		bool IsTextWrapped(string text, float baseWidth) {
			SizeF sizeF = measurer.MeasureStringI(text, font, baseWidth, stringFormat, pageUnit);
			float width = sizeF.Width;
			float height = sizeF.Height;
			TextFormatter.RotateBasis(stringFormat, ref width, ref height);
			float lineHeight = fontMetrics.CalculateHeight(1);
			return height > 1.5 * lineHeight;
		}
		internal static string TrimByEllipsisCharacter(string text, Measurer measurer, Font font, StringFormat stringFormat, GraphicsUnit pageUnit, float width) {
			if(string.IsNullOrEmpty(text))
				return text;
			StringBuilder builder = new StringBuilder(text);
			while(builder.Length > 0) {
				builder = new StringBuilder(builder.ToString().TrimEnd(' '));
				string result = builder.ToString() + ellipsis;
				float resultWidth = MeasureTextWidth(result, measurer, font, stringFormat, pageUnit);
				if(FirstLessOrEqualSecond(resultWidth, width))
					return result;
				builder.Remove(builder.Length - 1, 1);
			}
			return text;
		}
		internal static bool HasTrimmedByEllipsis(string text) {
			return !String.IsNullOrEmpty(text) && text.EndsWith(ellipsis);
		}
		bool isDisposed;
		float width;
		float height;
		Font font;
		FontMetrics fontMetrics;
		GraphicsUnit pageUnit;
		StringFormat stringFormat;
		int additionalLineCount;
		string line = String.Empty;
		WordFormatMode mode = WordFormatMode.FirstLine;
		List<string> lines = new List<string>();
		Measurer measurer;
		bool TrimByCharacter { get { return TextFormatter.TrimByCharacter(stringFormat); } }
		internal bool IsDisposed { get { return isDisposed; } }
		internal float Width { get { return width; } }
		internal float Height { get { return height; } }
		internal Font Font { get { return font; } }
		internal GraphicsUnit PageUnit { get { return pageUnit; } }
		internal StringFormat StringFormat { get { return stringFormat; } }
		internal int AdditionalLineCount { get { return additionalLineCount; } }
		internal bool LineLimit { get { return (stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0; } }
		internal string Line { get { return line; } set { line = value; } }
		internal WordFormatMode Mode { get { return mode; } set { mode = value; } }
		internal List<string> LinesForTest { get { return lines; } }
		public string[] Lines { get { return lines.ToArray(); } }
		string LastLine {
			get {
				if(lines.Count == 0)
					throw new WordFormatterException();
				return lines[lines.Count - 1];
			}
			set {
				lines[lines.Count - 1] = value;
			}
		}
		bool NextLineOutOfHeight {
			get {
				return LinesOutOfHeight(lines.Count + 1);
			}
		}
		bool disposeGDIResources = false;
		public WordFormatter(float width, float height, Font font, StringFormat stringFormat, GraphicsUnit pageUnit, Measurer measurer, int additionalLineCount) :
			this(width, height, font, stringFormat, pageUnit, measurer, additionalLineCount, true) { }
		public WordFormatter(float width, float height, Font font, StringFormat stringFormat, GraphicsUnit pageUnit, Measurer measurer, int additionalLineCount, bool disposeGDIResources) {
			this.disposeGDIResources = disposeGDIResources;
			this.width = width;
			this.height = height;
			this.font = font;
			this.stringFormat = stringFormat;
			this.pageUnit = pageUnit;
			this.measurer = measurer;
			this.additionalLineCount = additionalLineCount;
			this.fontMetrics = FontMetrics.CreateInstance(font, pageUnit);
		}
		public WordFormatter(float width, float height, Font font, StringFormat stringFormat, GraphicsUnit pageUnit)
			: this(width, height, font, stringFormat, pageUnit, Measurement.Measurer, 0) {
		}
		float MeasureTextWidth(string text) {
			return MeasureTextWidth(text, measurer, font, stringFormat, pageUnit);
		}
		protected internal virtual bool LinesOutOfHeight(int lineCount) {
			if(lineCount <= 0)
				throw new WordFormatterException();
			int totalLineCount = this.additionalLineCount + lineCount;
			if(!LineLimit)
				totalLineCount--;
			if(totalLineCount == 0)
				return false;
			float heightOfLines = this.fontMetrics.CalculateHeight(totalLineCount);
			return heightOfLines > this.Height;
		}
		public string GetLine(Word word) {
			switch(this.mode) {
				case WordFormatMode.FirstLine:
					return word.TextWithSpaces;
				case WordFormatMode.NewLine:
					return word.Text;
				case WordFormatMode.Line:
					return this.line + word.TextWithSpaces;
				default:
					throw new WordFormatterException();
			}
		}
		public void SetLine(string text) {
			this.mode = WordFormatMode.Line;
			this.line = text;
		}
		string TrimByEllipsisCharacter(string text) {
			return StringFormat.Trimming == StringTrimming.EllipsisCharacter ?
				TrimByEllipsisCharacter(text, measurer, font, stringFormat, pageUnit, width) :
				text;
		}
		protected internal virtual void FormatLastWordOfLastLine(Word lastWord) {
			string value = AppendWord(LastLine, lastWord.TextWithSpaces);
			LastLine = TrimByEllipsisCharacter(value);
		}
		string AppendWord(string line, string word) {
			foreach(char ch in word) {
				if(MeasureTextWidth(line + ch) >= this.width)
					break;
				line += ch;
			}
			return line;
		}
		public void UpdateLines() {
			lines.Add(line);
			line = string.Empty;
		}
		bool FormatFirstLineOnly(List<Word> words) {
			WordFormatterAlgorithm algorithm = CreateAlgorithm();
			foreach(Word word in words) {
				if(!algorithm.ApplyWordToLines(word)) {
					this.lines.Clear();
					return false;
				}
			}
			if(!String.IsNullOrEmpty(line))
				UpdateLines();
			return true;
		}
		bool FormatAllLines(List<Word> words) {
			WordFormatterAlgorithm algorithm = CreateAlgorithm();
			foreach(Word word in words) {
				while(!algorithm.ApplyWordToLines(word)) {
					algorithm.SetNextMode(word);
					if(NextLineOutOfHeight) {
						algorithm.FormatLastLine(word);
						return true;
					}
					if(!algorithm.SupportsMode(mode))
						algorithm = CreateAlgorithm();
				}
				if(!algorithm.SupportsMode(mode))
					algorithm = CreateAlgorithm();
			}
			if(!String.IsNullOrEmpty(line))
				UpdateLines();
			return true;
		}
		public virtual WordFormatterAlgorithm CreateAlgorithm() {
			return mode == WordFormatMode.ByChar ? new WordFormatterCharAlgorithm(this) :
				(WordFormatterAlgorithm)new WordFormatterLineAlgorithm(this);
		}
		public bool FormatWords(List<Word> words, int totalTextLineCount) {
			if(words == null)
				throw new ArgumentNullException("words");
			if(LinesOutOfHeight(1)) {
				if(totalTextLineCount > 1)
					return false;
				else
					return FormatFirstLineOnly(words);
			} else
				return FormatAllLines(words);
		}
		public void Dispose() {
			if(this.font != null) {
				if(disposeGDIResources)
					this.font.Dispose();
				this.font = null;
			}
			if(this.stringFormat != null) {
				if(disposeGDIResources)
					this.stringFormat.Dispose();
				this.stringFormat = null;
			}
			this.isDisposed = true;
		}
	}
	public class WordFormatterException : Exception {
		public WordFormatterException()
			: base() {
		}
	}
	public interface ITextFormatter {
		string[] FormatMultilineText(string multilineText, Font font, float width, float height, StringFormat stringFormat);
		string[] FormatHtmlMultilineText(string multilineText, Font font, float width, float height, StringFormat stringFormat);
		string[] FormatHtmlMultilineText(string multilineText, Font font, float width, float height, StringFormat stringFormat, bool designateNewLines);
	}
	public sealed class TextFormatter : ITextFormatter {
		public const string NonBreakingSpace = "\xA0";
		internal static List<Word> CreateWordArray(string text, StringFormat stringFormat) {
			List<Word> words = new List<Word>();
			Word currentWord = null;
			int trailingSpacesCount = 0;
			string[] fragmentationBySpaces = text.Split(new char[] { ' ' });
			for(int i = fragmentationBySpaces.Length - 1; i >= 0; i--) {
				string fragment = fragmentationBySpaces[i];
				if(fragment != String.Empty) {
					if(currentWord != null)
						words.Insert(0, currentWord);
					string wordText = fragment;
					if(trailingSpacesCount != 0 && (stringFormat.FormatFlags & StringFormatFlags.MeasureTrailingSpaces) != 0) {
						wordText += new String(' ', trailingSpacesCount);
					}
					currentWord = new Word(wordText);
					trailingSpacesCount = 0;
				} else {
					if(currentWord != null)
						currentWord.IncrementLeadSpaceCount();
					else
						trailingSpacesCount++;
				}
			}
			if(currentWord != null) {
				currentWord.DecrementLeadSpaceCount();
				words.Insert(0, currentWord);
			}
			return words;
		}
		internal static List<Word> SplitWordByHyphen(Word word) {
			return SplitWord(word, '-');
		}
		internal static bool TrimByCharacter(StringFormat stringFormat) {
			return stringFormat.Trimming == StringTrimming.Character || stringFormat.Trimming == StringTrimming.EllipsisCharacter;
		}
		internal static string[] SplitTextByNewLine(string text) {
			return text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
		}
		internal static void RotateBasis(StringFormat sf, ref float width, ref float height) {
			bool vertical = (sf.FormatFlags & StringFormatFlags.DirectionVertical) != 0;
			if(vertical) {
				float temp = height;
				height = width;
				width = temp;
			}
		}
		static string[] emptyStringArray = new string[] { string.Empty };
#if DEBUGTEST
		public
#endif
 string[] FormatText(string text, Font font, float width, float height, StringFormat stringFormat, int additionalLineCount, int totalTextLineCount, bool oneWord) {
			List<Word> words = new List<Word>();
			if(oneWord) {
				words.Add(new Word(text, 0));
			} else {
				words = CreateWordArray(text, stringFormat);
				words = SplitWords(words);
			}
			using(WordFormatter wordFormatter = new WordFormatter(width, height, font, stringFormat, pageUnit, measurer, additionalLineCount, false)) {
				bool success = wordFormatter.FormatWords(words, totalTextLineCount);
				return success ? wordFormatter.Lines : null;
			}
		}
#if DEBUGTEST
		public static List<Word> TEST_SplitWords(IList<Word> words) {
			return SplitWords(words);
		}
#endif
		static List<Word> SplitWords(IList<Word> words) {
			char[] separators = new char[] { '-' , '(', '[', '{', ')', ']', '}', '!', '%', '?','\t' };
			List<Word> resultList = new List<Word>(words.Count);
			foreach(char separator in separators) {
				resultList = SplitWords(words, separator);
				words = resultList;
			}
			return resultList;
		}
		static List<Word> SplitWords(IList<Word> words, char separator) {
			List<Word> resultList = new List<Word>(words.Count);
			foreach(Word word in words) {
				resultList.AddRange(SplitWord(word, separator));
			}
			return resultList;
		}
		static bool IsOpeningBrace(char separator) {
			const string openingBraces = "([{";
			return openingBraces.IndexOf(separator) != -1;
		}
		static bool IsClosingBrace(char separator) {
			const string closingBraces = ")]}";
			return closingBraces.IndexOf(separator) != -1;
		}
		static List<Word> SplitWord(Word word, char separator) {
			System.Diagnostics.Debug.Assert(separator != ' ');
			List<Word> words = new List<Word>();
			string[] fragments = word.Text.Split(separator);
			if(fragments.Length == 1) {
				words.Add(word);
				return words;
			}
			if(IsOpeningBrace(separator)) {
				if(fragments[0] != String.Empty) {
					words.Add(new Word(fragments[0], 0));
				}
				for(int i = 1; i < fragments.Length; i++) {
					words.Add(new Word(separator + fragments[i], 0));
				}
			} else if(IsClosingBrace(separator)) {
				for(int i = 0; i < fragments.Length - 1; i++) {
					words.Add(new Word(fragments[i] + separator, 0));
				}
				if(fragments.Last() != string.Empty)
					words.Add(new Word(fragments.Last(), 0));
			}
			else {
				for(int i = 0; i < fragments.Length; i++) {
					string separatorString = (i != fragments.Length - 1) ? Convert.ToString(separator) : String.Empty;
					if(fragments[i] != string.Empty) {
						if(fragments[i].EndsWith(NonBreakingSpace)) {
							words.Add(new Word(fragments[i] + separatorString, 0));
						} else {
							words.Add(new Word(fragments[i], 0));
							if(separatorString != string.Empty) {
								words.Add(new Word(separatorString, 0));
							}
						}
					} else {
						if(separatorString != string.Empty) {
							words.Add(new Word(separatorString, 0));
						}
					}
				}
			}
			if(words.Count > 0) {
				Word word0 = words[0];
				words.RemoveAt(0);
				words.Insert(0, new Word(word0.Text, word.LeadSpaceCount));
			}
			return words;
		}
		GraphicsUnit pageUnit;
		Measurer measurer;
		public TextFormatter(GraphicsUnit pageUnit, Measurer measurer) {
			this.pageUnit = pageUnit;
			this.measurer = measurer;
		}
		public string[] FormatMultilineText(string multilineText, Font font, float width, float height, StringFormat stringFormat) {
			if(width < 0)
				throw new ArgumentException("width");
			RotateBasis(stringFormat, ref width, ref height);
			List<string> resultLines = new List<string>();
			bool noWrap = (stringFormat.FormatFlags & StringFormatFlags.NoWrap) != 0;
			string[] texts = SplitTextByNewLine(multilineText);
			float oneLineHeight = TextFormatter.CalculateHeightOfLines(font, 1, pageUnit);
			float bottom = 0;
			foreach(string text in texts) {
				string[] lines;
				string formattingText = (string.IsNullOrEmpty(text) || stringFormat.FormatFlags.HasFlag(StringFormatFlags.MeasureTrailingSpaces)) ? text : text.TrimEnd(' ');
				if(noWrap) {
					if(TrimByCharacter(stringFormat)) {
						lines = FormatText(formattingText, font, width, height, stringFormat, 0, 1, true);
						if(lines != null) {
							if(lines.Length > 0) {
								string resultText = lines[0];
								if(stringFormat.Trimming == StringTrimming.EllipsisCharacter && lines.Length > 1 && !WordFormatter.HasTrimmedByEllipsis(resultText))
									resultText = WordFormatter.TrimByEllipsisCharacter(resultText, measurer, font, stringFormat, pageUnit, width);
								lines = new string[] { resultText };
							} else if(lines.Length == 0) {
								lines = emptyStringArray;
							}
						}
					} else {
						lines = new string[] { formattingText };
					}
				} else {
					lines = FormatText(formattingText, font, width, height, stringFormat, resultLines.Count, texts.Length, false);
					if(lines == null) {
						if(stringFormat.Trimming == StringTrimming.EllipsisCharacter && resultLines.Count > 0) {
							string textForTrimming = resultLines[resultLines.Count - 1];
							if(!WordFormatter.HasTrimmedByEllipsis(textForTrimming))
								resultLines[resultLines.Count - 1] = WordFormatter.TrimByEllipsisCharacter(textForTrimming, measurer, font, stringFormat, pageUnit, width);
						}
						break;
					}
					if(lines.Length == 0)
						lines = emptyStringArray;
				}
				if(lines == null)
					continue;
				foreach(string line in lines) {
					bottom += oneLineHeight;
					if(bottom - height > 0 && resultLines.Count > 0)
						break;
					resultLines.Add(line);
				}
			}
			return resultLines.ToArray();
		}
		public string[] FormatHtmlMultilineText(string multilineText, Font font, float width, float height, StringFormat stringFormat) {
			return FormatHtmlMultilineText(multilineText, font, width, height, stringFormat, false);
		}
		public string[] FormatHtmlMultilineText(string multilineText, Font font, float width, float height, StringFormat stringFormat, bool designateNewLines) {
			const float measuringInaccuracy = 0.05f;
			List<string> resultLines = new List<string>();
			bool noWrap = (stringFormat.FormatFlags & StringFormatFlags.NoWrap) != 0;
			string[] texts = SplitTextByNewLine(multilineText);
			float pixHeight = TextFormatter.CalculateHeightOfLines(font, 1, GraphicsUnit.Pixel);
			float bottom = 0;
			List<int> lineBreaks = new List<int>();
			foreach(string text in texts) {
				string[] lines;
				string formattingText = (string.IsNullOrEmpty(text) || stringFormat.FormatFlags.HasFlag(StringFormatFlags.MeasureTrailingSpaces)) ? text : text.TrimEnd(' ');
				if(noWrap) {
					lines = FormatText(formattingText, font, width, height, stringFormat, 0, 1, true);
					if(lines != null && lines.Length > 0)
						lines = new string[] { lines[0] };
				} else {
					lines = FormatText(formattingText, font, width, height, stringFormat, resultLines.Count, texts.Length, false);
				}
				if(lines == null)
					break;
				if(lines.Length == 0)
					lines = emptyStringArray;
				foreach(string line in lines) {
					bottom += pixHeight;
					if(bottom - height > measuringInaccuracy && resultLines.Count > 0)
						break;
					resultLines.Add(line);
				}
				lineBreaks.Add(resultLines.Count);
			}
			if(designateNewLines)
				for(int i = 0; i < lineBreaks.Count - 1; i++)
					resultLines.Insert(lineBreaks[i] + i, null);
			return resultLines.ToArray();
		}
		public static float CalculateHeightOfLines(Font font, int lineCount, GraphicsUnit pageUnit) {
			FontMetrics metrics = new FontMetrics(font, pageUnit);
			return metrics.CalculateHeight(lineCount);
		}
	}
}
