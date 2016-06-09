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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
namespace DevExpress.Utils.Text {
	public class TextUtils {
		[ThreadStatic]
		static FontsCache fontsCache;
		static int tabStopSpacesCount;
		static int leftOffset, rightOffset, topOffset, bottomOffset;
		static bool useKerning;
		static TextUtils() {
			tabStopSpacesCount = 4;
			ResetOffsets();
		}
		public static int TabStopSpacesCount { get { return tabStopSpacesCount; } set { tabStopSpacesCount = value; } }
		public static int LeftOffset { get { return leftOffset; } set { leftOffset = value; } }
		public static int RightOffset { get { return rightOffset; } set { rightOffset = value; } }
		public static int TopOffset { get { return topOffset; } set { topOffset = value; } }
		public static int BottomOffset { get { return bottomOffset; } set { bottomOffset = value; } }
		public static void ResetOffsets() {
			SetOffsets(0, 0, 0, 0);
		}
		public static void SetOffsets(int left, int top, int right, int bottom) {
			LeftOffset = left;
			TopOffset = top;
			RightOffset = right;
			BottomOffset = bottom;
		}
		public static bool UseKerning { get { return useKerning; } set { useKerning = value; } }
		private static FontsCache Fonts {
			get {
				if (fontsCache == null) fontsCache = new FontsCache();
				return fontsCache;
			}
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Point location) {
			DrawString(g, text, font, foreColor, location, Rectangle.Empty, StringFormat.GenericDefault);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Point location, Rectangle clipBounds) {
			DrawString(g, text, font, foreColor, location, clipBounds, StringFormat.GenericDefault);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Point location, StringFormat stringFormat) {
			DrawString(g, text, font, foreColor, location, Rectangle.Empty, stringFormat);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Point location, Rectangle clipBounds, StringFormat stringFormat) {
			DrawString(g, text, font, foreColor, new Rectangle(location, GetStringSize(g, text, font, stringFormat)), clipBounds, stringFormat);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds) {
			DrawString(g, text, font, foreColor, drawBounds, Rectangle.Empty, StringFormat.GenericDefault);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds, Rectangle clipBounds) {
			DrawString(g, text, font, foreColor, drawBounds, clipBounds, StringFormat.GenericDefault);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds, StringFormat stringFormat) {
			DrawString(g, text, font, foreColor, drawBounds, Rectangle.Empty, stringFormat);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds, Rectangle clipBounds, StringFormat stringFormat) {
			DrawString(g, text, font, foreColor, drawBounds, clipBounds, stringFormat, null);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds, StringFormat stringFormat, TextHighLight highLight) {
			DrawString(g, text, font, foreColor, drawBounds, Rectangle.Empty, stringFormat, highLight);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds, Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight) {
			DrawString(g, text, font, foreColor, drawBounds, clipBounds, stringFormat, highLight, null);
		}
		public static void DrawString(Graphics g, string text, Font font, Color foreColor, Rectangle drawBounds, Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight, IWordBreakProvider wordBreakProvider) {
			Fonts.DrawString(g, text, font, foreColor, drawBounds, clipBounds, stringFormat, highLight, wordBreakProvider);
		}
		public static Size GetStringSize(Graphics g, string text, Font font) {
			return GetStringSize(g, text, font, StringFormat.GenericDefault);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat) {
			if (text == null) return Size.Empty;
			return Fonts.GetStringSize(g, text, font, stringFormat);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat, int maxWidth) {
			return GetStringSize(g, text, font, stringFormat, maxWidth, null);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat, int maxWidth, IWordBreakProvider wordBreakProvider) {
			if (text == null) return Size.Empty;
			return Fonts.GetStringSize(g, text, font, stringFormat, maxWidth, wordBreakProvider);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight) {
			bool isCropped;
			return GetStringSize(g, text, font, stringFormat, maxWidth, maxHeight, null, out isCropped);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider) {
			bool isCropped;
			return GetStringSize(g, text, font, stringFormat, maxWidth, maxHeight, wordBreakProvider, out isCropped);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, out bool isCropped) {
			return GetStringSize(g, text, font, stringFormat, maxWidth, maxHeight, null, out isCropped);
		}
		public static Size GetStringSize(Graphics g, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider, out bool isCropped) {
			isCropped = false;
			if (text == null) return Size.Empty;
			return Fonts.GetStringSize(g, text, font, stringFormat, maxWidth, maxHeight, wordBreakProvider, out isCropped);
		}
		public static int[] GetMeasureString(Graphics g, string text, Font font) {
			return GetMeasureString(g, text, font, StringFormat.GenericDefault);
		}
		public static int[] GetMeasureString(Graphics g, string text, Font font, StringFormat stringFormat) {
			return Fonts.GetMeasureString(g, text, font, stringFormat);
		}
		public static int GetStringHeight(Graphics g, string text, Font font, int width) {
			return GetStringHeight(g, text, font, width, StringFormat.GenericDefault);
		}
		public static int GetStringHeight(Graphics g, string text, Font font, int width, StringFormat stringFormat) {
			return Fonts.GetStringHeight(g, text, font, width, stringFormat);
		}
		public static int GetFontHeight(Graphics g, Font font) {
			return Fonts.GetFontHeight(g, font);
		}
		public static int GetFontAscentHeight(Graphics g, Font font) {
			return Fonts.GetFontAscentHeight(g, font);
		}
	}
	public class FontCache : IDisposable {
		const int InitialCharCount = 1024;
		const string StringToMeasureAverageCharWidth = "ABCDEFGWabcdefgw1234567890";
		FontStyle fontStyle;
		IntPtr fontHandle;
		IntPtr fontUnderlineHandle;
		bool underline;
		int height;
		int ascentHeight;
		int averageCharWidth;
		int[] charsWidth;
		int[] abcWidths;
		char firstChar;
		char lastChar;
		char currentLastChar;
		Dictionary<uint, int> kerningPairs;
		public FontCache(Graphics graphics, Font font) {
			this.fontStyle = font.Style;
			this.fontHandle = CreateGdiFont(font);
			this.underline = font.Underline;
			if (!Underline) {
				Font fontUnderline = new Font(font, fontStyle | FontStyle.Underline);
				this.fontUnderlineHandle = CreateGdiFont(fontUnderline);
				fontUnderline.Dispose();
			}
			Calculate(graphics, font);
		}
		public void Dispose() {
			DisposeCore();
		}
		[System.Security.SecuritySafeCritical]
		protected virtual void DisposeCore() {
			Win32Util.Win32API.DeleteObject(this.fontHandle);
			if (this.fontUnderlineHandle != IntPtr.Zero)
				Win32Util.Win32API.DeleteObject(this.fontUnderlineHandle);
		}
		public static char TabStopChar { get { return '\t'; } }
		public static char NewLineChar { get { return '\n'; } }
		public static char ReturnChar { get { return '\r'; } }
		public static char SpaceChar { get { return ' '; } }
		public static bool IsTabStop(char ch) { return ch == TabStopChar; }
		public static bool IsNewLine(char ch) { return ch == NewLineChar; }
		public static bool IsReturn(char ch) { return ch == ReturnChar; }
		public static bool IsSpace(char ch) { return ch == SpaceChar; }
		public int Height { get { return height; } }
		public int AscentHeight { get { return ascentHeight; } }
		public bool IsItalic { get { return (fontStyle & FontStyle.Italic) == FontStyle.Italic; } }
		[System.Security.SecuritySafeCritical]
		IntPtr CreateGdiFont(Font font) {
			Win32Util.LOGFONT lf = new Win32Util.LOGFONT();
			font.ToLogFont(lf);
			if (font.Unit != GraphicsUnit.Point)
				lf.lfHeight = (int)-font.Size;
			IntPtr hFont = Win32Util.Win32API.CreateFont(lf.lfHeight, lf.lfWidth, lf.lfEscapement, lf.lfOrientation, lf.lfWeight, lf.lfItalic, lf.lfUnderline, lf.lfStrikeOut, lf.lfCharSet, lf.lfOutPrecision, lf.lfClipPrecision, lf.lfQuality, lf.lfPitchAndFamily, lf.lfFaceName);
			return hFont;
		}
		bool IsMultiLine(StringFormat stringFormat) {
			return (stringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0;
		}
		public void DrawString(Graphics graphics, Color foreColor, string text, Rectangle drawBounds,
			Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight, IWordBreakProvider wordBreakProvider) {
			DrawStringSC(graphics, foreColor, text, drawBounds,
			clipBounds, stringFormat, highLight, wordBreakProvider);
		}
		public void DrawString(Graphics graphics, Color foreColor, TextOutDraw draw) {
			draw.SetGraphics(graphics);
			DrawStringSCCore(graphics, foreColor, draw);
		}
		public TextOutDraw PrepareTextOut(Graphics graphics, Color foreColor, string text, Rectangle drawBounds,
			Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight, IWordBreakProvider wordBreakProvider) {
			return new TextOutDraw(this, graphics, ValidateString(text, highLight, IsMultiLine(stringFormat)), drawBounds, clipBounds, stringFormat, highLight, wordBreakProvider);
		}
		[System.Security.SecuritySafeCritical]
		internal void DrawStringSC(Graphics graphics, Color foreColor, string text, Rectangle drawBounds,
			Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight, IWordBreakProvider wordBreakProvider) {
			DrawStringSCCore(graphics, foreColor, PrepareTextOut(graphics, foreColor, text, drawBounds, clipBounds, stringFormat, highLight, wordBreakProvider));
		}
		[System.Security.SecuritySafeCritical]
		internal void DrawStringSCCore(Graphics graphics, Color foreColor, TextOutDraw draw) {
			IntPtr hdc = graphics.GetHdc();
			IntPtr oldFontHandle = Win32Util.SelectObject(hdc, FontHandle);
			Win32Util.SetTextColor(hdc, foreColor);
			Win32Util.SetBkMode(hdc, Win32Util.TRANSPARENT);
			draw.DrawString(hdc);
			Win32Util.SelectObject(hdc, oldFontHandle);
			graphics.ReleaseHdcInternal(hdc);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat) {
			bool isCropped;
			return GetStringSize(graphics, text, stringFormat, int.MaxValue, int.MaxValue, null, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat, int maxWidth) {
			bool isCropped;
			return GetStringSize(graphics, text, stringFormat, maxWidth, int.MaxValue, null, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat, int maxWidth, int maxHeight) {
			bool isCropped;
			return GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, null, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider) {
			bool isCropped;
			return GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, wordBreakProvider, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat, int maxWidth, int maxHeight, out bool isCropped) {
			return GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, null, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat, int maxWidth, IWordBreakProvider wordBreakProvider) {
			bool isCropped;
			return GetStringSize(graphics, text, stringFormat, maxWidth, int.MaxValue, wordBreakProvider, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider, out bool isCropped) {
			isCropped = false;
			if (maxWidth <= 0) return GetStringSize(graphics, text, stringFormat);
			TextOutDraw draw = new TextOutDraw(this, graphics, ValidateString(text, IsMultiLine(stringFormat)), new Rectangle(0, 0, maxWidth, maxHeight), Rectangle.Empty, stringFormat, null, wordBreakProvider);
			isCropped = draw.IsCropped;
			return new Size(draw.MaxDrawWidth, draw.LineCount * Height);
		}
		public int[] GetMeasureString(Graphics graphics, string text, StringFormat stringFormat) {
			int[] widths = GetCharactersWidth(graphics, ValidateString(text, true, IsMultiLine(stringFormat)), stringFormat);
			return widths;
		}
		public int GetStringHeight(Graphics graphics, string text, int width, StringFormat stringFormat) {
			TextOutDraw draw = new TextOutDraw(this, graphics, ValidateString(text, IsMultiLine(stringFormat)), new Rectangle(0, 0, width, int.MaxValue), Rectangle.Empty, stringFormat, null, null);
			return draw.LineCount * Height;
		}
		internal bool Underline { get { return underline; } }
		internal IntPtr FontHandle { get { return this.fontHandle; } }
		internal IntPtr FontUnderlineHandle { get { return this.fontUnderlineHandle != IntPtr.Zero ? this.fontUnderlineHandle : this.fontHandle; } }
		string ValidateString(string text, bool isMultiLine) {
			return ValidateString(text, false, null, isMultiLine);
		}
		string ValidateString(string text, bool removeReturn, bool isMultiLine) {
			return ValidateString(text, removeReturn, null, isMultiLine);
		}
		string ValidateString(string text, TextHighLight highLight, bool isMultiLine) {
			return ValidateString(text, false, highLight, isMultiLine);
		}
		public static int MaxSingleLineChars = 10000; 
		public static int MaxMultiLineChars = 10000;
		string ValidateString(string text, bool removeReturn, TextHighLight highLight, bool isMultiLine) {
			if (text == null) return string.Empty;
			int i = 0;
			if (isMultiLine) {
				if (text.Length > MaxMultiLineChars)
					text = text.Substring(0, MaxMultiLineChars);
			}
			else {
				if (text.Length > MaxSingleLineChars)
					text = text.Substring(0, MaxSingleLineChars);
			}
			if (removeReturn) {
				while (i < text.Length) {
					if (IsNewLine(text[i])) {
						text = text.Remove(i, 1);
						if (highLight != null) {
							if (i <= highLight.EndIndex) {
								if (i <= highLight.StartIndex)
									highLight.StartIndex--;
								else highLight.Length--;
							}
						}
					}
					else i++;
				}
			}
			return text;
		}
		public int[] GetCharactersWidth(Graphics graphics, string text, StringFormat stringFormat) {
			PrepareWidthAndKerningArrays(graphics, text);
			int[] widths = new int[text.Length];
			int tabIndex = 0;
			for (int i = 0; i < text.Length; i++) {
				if (!IsFontChar(text[i])) {
					widths[i] = this.averageCharWidth;
					if (IsTabStop(text[i])) {
						widths[i] = GetTabWidth(stringFormat, tabIndex++);
					}
					if (IsNewLine(text[i]) || IsReturn(text[i])) {
						widths[i] = 0;
					}
				}
				else {
					widths[i] = this.charsWidth[text[i] - this.firstChar];
					if (TextUtils.UseKerning) {
						if ((i < text.Length - 1) && IsFontChar(text[i]) && IsFontChar(text[i + 1])) {
							widths[i] += GetKerningPairAmount(text[i], text[i + 1]);
						}
					}
				}
			}
			return widths;
		}
		public int GetCharABCWidths(char ch) {
			if (this.abcWidths == null) return 0;
			if (ch < this.firstChar) return 0;
			int index = ch - this.firstChar;
			if (this.abcWidths.Length <= index) return 0;
			return this.abcWidths[index];
		}
		bool IsFontChar(char ch) {
			return ch >= this.firstChar && ch <= this.lastChar;
		}
		int GetTabWidth(StringFormat stringFormat, int tabIndex) {
			float tabOffSet = 0;
			float[] offSets = stringFormat.GetTabStops(out tabOffSet);
			if (tabIndex < offSets.Length)
				tabOffSet = offSets[tabIndex];
			if (tabOffSet > 0)
				return (int)tabOffSet;
			else return this.charsWidth[SpaceChar - this.firstChar] * TextUtils.TabStopSpacesCount;
		}
		void Calculate(Graphics graphics, Font font) {
			IntPtr hdc = graphics.GetHdc();
			IntPtr oldFontHandle = Win32Util.SelectObject(hdc, FontHandle);
			TEXTMETRIC textMetric = Win32Util.GetTextMetrics(hdc);
			this.firstChar = textMetric.tmFirstChar;
			this.lastChar = textMetric.tmLastChar;
			bool canCalculateMetricExactly = this.firstChar <= this.lastChar;
			if (canCalculateMetricExactly) {
				this.height = textMetric.tmHeight;
				this.ascentHeight = textMetric.tmAscent;				
				this.currentLastChar = this.firstChar + InitialCharCount < this.lastChar ? (char)(this.firstChar + InitialCharCount) : this.lastChar;
				this.averageCharWidth = textMetric.tmAveCharWidth;
				CreateWidthAndKerningArrays(hdc);
			}
			Win32Util.SelectObject(hdc, oldFontHandle);
			graphics.ReleaseHdc(hdc);
			if (!canCalculateMetricExactly)
				CalculateMetricFromFont(graphics, font);
		}
		void CalculateMetricFromFont(Graphics graphics, Font font) {
			this.firstChar = (char)0;
			this.lastChar = (char)0;
			this.height = font.Height;
			int ascent = font.FontFamily.GetCellAscent(font.Style);
			this.ascentHeight = (int)(font.Size * ascent / font.FontFamily.GetEmHeight(font.Style));
			this.currentLastChar = this.firstChar + InitialCharCount < this.lastChar ? (char)(this.firstChar + InitialCharCount) : this.lastChar;
			this.averageCharWidth = (int)(graphics.MeasureString(StringToMeasureAverageCharWidth, font).Width / StringToMeasureAverageCharWidth.Length);
			CreateWidthAndKerningArrays();
		}
		void CreateWidthAndKerningArrays() {
			this.charsWidth = new int[0];
			this.abcWidths = new int[0];
		}
		void CreateWidthAndKerningArrays(IntPtr hdc) {
			this.charsWidth = Win32Util.GetCharWidth(hdc, this.firstChar, this.currentLastChar);
			this.abcWidths = Win32Util.GetCharABCWidths(hdc, this.firstChar, this.currentLastChar);
		}
		void CreateKernings(IntPtr hdc) {
			Win32Util.KerningPair[] pairs = Win32Util.GetKerningPairs(hdc);
			if (pairs != null) {
				kerningPairs = new Dictionary<uint, int>(pairs.Length);
				for (int i = 0; i < pairs.Length; i++) {
					this.kerningPairs[GetKerningPairHashCode(pairs[i].wFirst, pairs[i].wSecond)] = pairs[i].iKernAmount;
				}
			}
			else
				kerningPairs = new Dictionary<uint, int>(0);
		}
		void PrepareWidthAndKerningArrays(Graphics graphics, string text) {
			char newLastChar = this.currentLastChar;
			for (int i = text.Length - 1; i >= 0; i--) {
				if (text[i] > newLastChar)
					newLastChar = text[i];
			}
			if (newLastChar > this.currentLastChar) {
				this.currentLastChar = newLastChar + InitialCharCount < this.lastChar ? (char)(newLastChar + InitialCharCount) : this.lastChar;
				IntPtr hdc = graphics.GetHdc();
				IntPtr oldFontHandle = Win32Util.SelectObject(hdc, FontHandle);
				CreateWidthAndKerningArrays(hdc);
				Win32Util.SelectObject(hdc, oldFontHandle);
				graphics.ReleaseHdc(hdc);
			}
			if (TextUtils.UseKerning && kerningPairs == null) {
				IntPtr hdc = graphics.GetHdc();
				IntPtr oldFontHandle = Win32Util.SelectObject(hdc, FontHandle);
				CreateKernings(hdc);
				Win32Util.SelectObject(hdc, oldFontHandle);
				graphics.ReleaseHdc(hdc);
			}
		}
		uint GetKerningPairHashCode(char wFirst, char wSecond) {
			return (uint)wFirst * 0x10000 + wSecond;
		}
		int GetKerningPairAmount(char wFirst, char wSecond) {
			uint pairHashCode = GetKerningPairHashCode(wFirst, wSecond);
			int amount;
			return this.kerningPairs.TryGetValue(pairHashCode, out amount) ? amount : 0;
		}
	}
	internal class FontsCache : IDisposable {
		Dictionary<GraphicsUnit, ConstDpiFontsCache> dpiHash;
		class GraphicsUnitComparer : IEqualityComparer<GraphicsUnit> {
			public bool Equals(GraphicsUnit x, GraphicsUnit y) {
				return x == y;
			}
			public int GetHashCode(GraphicsUnit obj) {
				return ((int)obj).GetHashCode();
			}
		}
		public FontsCache() {
			this.dpiHash = new Dictionary<GraphicsUnit, ConstDpiFontsCache>(new GraphicsUnitComparer());
		}
		~FontsCache() {
			((IDisposable)this).Dispose();
		}
		public FontCache this[Graphics graphics, Font font] { get { return GetFontCacheByFont(graphics, font); } }
		public void DrawString(Graphics graphics, string text, Font font, Color foreColor, Rectangle drawBounds, Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight, IWordBreakProvider wordBreakProvider) {
			if (graphics == null || text == null || font == null || stringFormat == null) return;
			this[graphics, font].DrawString(graphics, foreColor, text, drawBounds, clipBounds, stringFormat, highLight, wordBreakProvider);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, IWordBreakProvider wordBreakProvider) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, wordBreakProvider);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, wordBreakProvider);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, out bool isCropped) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider, out bool isCropped) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, wordBreakProvider, out isCropped);
		}
		public int[] GetMeasureString(Graphics graphics, string text, Font font, StringFormat stringFormat) {
			return this[graphics, font].GetMeasureString(graphics, text, stringFormat);
		}
		public int GetStringHeight(Graphics graphics, string text, Font font, int width, StringFormat stringFormat) {
			return this[graphics, font].GetStringHeight(graphics, text, width, stringFormat);
		}
		public int GetFontHeight(Graphics graphics, Font font) {
			return this[graphics, font].Height;
		}
		public int GetFontAscentHeight(Graphics graphics, Font font) {
			return this[graphics, font].AscentHeight;
		}
		FontCache GetFontCacheByFont(Graphics graphics, Font font) {
			ConstDpiFontsCache dpiCache;
			if (!dpiHash.TryGetValue(graphics.PageUnit, out dpiCache)) {
				dpiCache = new ConstDpiFontsCache();
				dpiHash.Add(graphics.PageUnit, dpiCache);
			}
			return dpiCache[graphics, font];
		}
		void IDisposable.Dispose() {
			Clear();
		}
		void Clear() {
			foreach (IDisposable cache in this.dpiHash.Values) {
				cache.Dispose();
			}
			this.dpiHash.Clear();
		}
	}
	public class ConstDpiFontsCache : IDisposable {
		Dictionary<Font, FontCache> fonts;
		public ConstDpiFontsCache() {
			this.fonts = new Dictionary<Font, FontCache>();
		}
		public FontCache this[Graphics graphics, Font font] { get { return GetFontCacheByFont(graphics, font); } }
		public void DrawString(Graphics graphics, string text, Font font, Color foreColor, Rectangle drawBounds, Rectangle clipBounds, StringFormat stringFormat, TextHighLight highLight, IWordBreakProvider wordBreakProvider) {
			if (graphics == null || text == null || font == null || stringFormat == null) return;
			this[graphics, font].DrawString(graphics, foreColor, text, drawBounds, clipBounds, stringFormat, highLight, wordBreakProvider);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, IWordBreakProvider wordBreakProvider) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, wordBreakProvider);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, wordBreakProvider);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, out bool isCropped) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, out isCropped);
		}
		public Size GetStringSize(Graphics graphics, string text, Font font, StringFormat stringFormat, int maxWidth, int maxHeight, IWordBreakProvider wordBreakProvider, out bool isCropped) {
			return this[graphics, font].GetStringSize(graphics, text, stringFormat, maxWidth, maxHeight, wordBreakProvider, out isCropped);
		}
		public int[] GetMeasureString(Graphics graphics, string text, Font font, StringFormat stringFormat) {
			return this[graphics, font].GetMeasureString(graphics, text, stringFormat);
		}
		public int GetStringHeight(Graphics graphics, string text, Font font, int width, StringFormat stringFormat) {
			return this[graphics, font].GetStringHeight(graphics, text, width, stringFormat);
		}
		public int GetFontHeight(Graphics graphics, Font font) {
			return this[graphics, font].Height;
		}
		public int GetFontAscentHeight(Graphics graphics, Font font) {
			return this[graphics, font].AscentHeight;
		}
		FontCache GetFontCacheByFont(Graphics graphics, Font font) {
			FontCache fontCache;
			this.fonts.TryGetValue(font, out fontCache);
			return fontCache != null ? fontCache : AddFontCache(graphics, font);
		}
		FontCache AddFontCache(Graphics graphics, Font font) {
			FontCache cache = new FontCache(graphics, font);
			this.fonts[font] = cache;
			return cache;
		}
		void IDisposable.Dispose() {
			Clear();
		}
		void Clear() {
			foreach (FontCache cache in this.fonts.Values) {
				cache.Dispose();
			}
			this.fonts.Clear();
		}
	}
	public class TextHighLight {
		int startIndex;
		int length;
		Color backColor;
		Color foreColor;
		public TextHighLight(int startIndex, int length, Color backColor, Color foreColor) {
			this.startIndex = startIndex;
			this.length = length;
			this.backColor = backColor;
			this.foreColor = foreColor;
		}
		public int StartIndex { get { return startIndex; } set { startIndex = value; } }
		public int Length { get { return length; } set { length = value; } }
		public int EndIndex { get { return StartIndex + Length; } }
		public Color BackColor { get { return backColor; } }
		public Color ForeColor { get { return foreColor; } }
		public bool IsTextHighLighted(int textPos, int textLen) {
			int textEndPos = textPos + textLen;
			return (textPos <= StartIndex && textEndPos >= StartIndex)
				|| (textEndPos >= EndIndex && textPos <= EndIndex)
				|| (textPos >= StartIndex && textEndPos <= EndIndex);
		}
	}
	public interface IWordBreakProvider {
		bool IsWordBreakChar(char ch);
	}
	public class TextOutDraw : IWordBreakProvider {
		Graphics graphics;
		string text;
		Rectangle drawBounds;
		Rectangle clipedBounds;
		int[] widths;
		bool[] hotPrefixes;
		StringFormat format;
		StringAlignment formatAlignment;
		StringFormatFlags formatFlags;
		TextHighLight highLight;
		FontCache fontCache;
		int pos;
		int drawTop;
		int linepos;
		bool isCliped;
		bool measureTrailingSpaces;
		bool isNoWrap;
		bool trimming;
		TextLines lines;
		readonly IWordBreakProvider wordBreakProvider;
		public TextOutDraw(FontCache fontCache, Graphics graphics, string text,
			Rectangle drawBounds, Rectangle clipBounds,
			StringFormat format, TextHighLight highLight, IWordBreakProvider provider) {
			this.fontCache = fontCache;
			this.graphics = graphics;
			this.format = format;
			this.formatAlignment = CalcAlignment(format.Alignment, (format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0);
			this.formatFlags = format.FormatFlags;
			this.highLight = highLight;
			if (provider == null)
				this.wordBreakProvider = this;
			else
				this.wordBreakProvider = provider;
			this.text = ValidateHotPrefix(text);
			this.drawBounds = ApplyTextUtilsOffsets(drawBounds);
			if (clipBounds.IsEmpty)
				this.clipedBounds = drawBounds;
			else {
				this.isCliped = true;
				this.clipedBounds = clipBounds;
				this.clipedBounds.Intersect(drawBounds);
			}
			this.widths = GetCharactersWidth(Text);
			this.isNoWrap = (formatFlags & StringFormatFlags.NoWrap) == StringFormatFlags.NoWrap;
			this.trimming = format.Trimming != StringTrimming.None;
			this.isCliped |= ((formatFlags & StringFormatFlags.NoClip) == 0) ||
				(IsNoWrap && !trimming);
			this.measureTrailingSpaces = (formatFlags & StringFormatFlags.MeasureTrailingSpaces) == StringFormatFlags.MeasureTrailingSpaces;
			CreateLines();
		}
		private StringAlignment CalcAlignment(StringAlignment stringAlignment, bool rightToLeft) {
			if(!rightToLeft) return stringAlignment;
			switch(stringAlignment) {
				case StringAlignment.Near: return StringAlignment.Far;
				case StringAlignment.Far: return StringAlignment.Near;
			}
			return stringAlignment;
		}
		public void SetGraphics(Graphics graphics) { this.graphics = graphics; }
		public FontCache FontCache { get { return fontCache; } }
		public virtual int FontHeight { get { return FontCache.Height; } }
		public virtual int[] GetCharactersWidth(string text) { return FontCache.GetCharactersWidth(this.graphics, text, Format); }
		public string Text { get { return text; } }
		public StringAlignment FormatAlignment {
			get { return formatAlignment; }
		}
		public StringFormat Format { get { return format; } }
		public StringFormatFlags FormatFlags { get { return formatFlags; } }
		public Rectangle DrawBounds { get { return drawBounds; } }
		public Rectangle ClipedBounds { get { return clipedBounds; } }
		public int Pos { get { return pos; } set { pos = value; } }
		public int[] Widths { get { return widths; } }
		public bool IsCliped { get { return isCliped; } }
		public bool MeasureTrailingSpaces { get { return measureTrailingSpaces; } }
		public void DrawString(IntPtr hdc) {
			if (Text.Length == 0) return;
			DrawStringLines(hdc);
		}
		public int LineCount { get { return this.lines != null ? lines.Count : 1; } }
		public int MaxDrawWidth {
			get {
				int width = this.lines.Width;
				return width < DrawBounds.Width ? width : DrawBounds.Width;
			}
		}
		public bool IsCropped { get; private set; }
		protected int GetCharABCWidths(char ch) {
			if (FontCache == null)
				return 0;
			else return FontCache.GetCharABCWidths(ch);
		}
		public int GetCharABCWidths(int index) {
			return GetCharABCWidths(Text[index]);
		}
		bool IsNoWrap { get { return isNoWrap; } }
		void CreateLines() {
			IsCropped = false;
			lines = new TextLines(this);
			if (IsNoWrap) {
				if (Text.IndexOf(FontCache.NewLineChar) > -1)
					CreateReturnLines();
				else CreateNoWrapLines();
			}
			else CreateWrapLines();
		}
		bool CanAddOneMoreLine(int lineTop) {
			if (!trimming && IsNoWrap)
				return this.drawTop < DrawBounds.Height;
			else
				return (this.drawTop + FontHeight <= DrawBounds.Height) || this.lines.Count == 0;
		}
		void CreateWrapLines() {
			while (Pos < Text.Length && CanAddOneMoreLine(this.drawTop)) {
				while (Pos < Text.Length && FontCache.IsNewLine(Text[Pos])) {
					AddNewLineToList();
				}
				if (Pos >= Text.Length || !CanAddOneMoreLine(this.drawTop)) break;
				if (this.widths[Pos] > DrawBounds.Width) break;
				int wordLength = GetNextWord(Pos, Text.Length);
				int newLineWidth = GetTextWidth(this.linepos, this.pos - this.linepos + wordLength);
				if (newLineWidth > DrawBounds.Width) {
					if (Pos - this.linepos > 0) {
						AddNewLineToList();
					}
					else {
						int lineWidth = widths[Pos];
						for (int i = 1; i < wordLength; i++) {
							if (lineWidth + widths[Pos + i] + GetCharABCWidths(Pos + i) > DrawBounds.Width) {
								this.pos += i;
								IsCropped = true;
								break;
							}
							lineWidth += widths[Pos + i];
						}
						AddNewLineToList();
						if (wordLength == 1) break;
					}
				}
				else {
					this.pos += wordLength;
				}
			}
			if (Pos - this.linepos > 0)
				AddNewLineToList();
			if (Pos < (Text.Length - 1)) {
				if (lines.Count > 0)
					lines[lines.Count - 1].UpdateTrimmingLine(Format.Trimming, drawBounds.Width, true, Text);
				IsCropped = true;
			}
			this.pos = 0;
		}
		void CreateReturnLines() {
			int lineLength = 0;
			while (Pos < Text.Length && CanAddOneMoreLine(this.drawTop)) {
				if (FontCache.IsNewLine(Text[Pos])) {
					AddNewLineToList();
					if (lines[lines.Count - 1].Length < lineLength)
						IsCropped = true;
					lineLength = 0;
				}
				else {
					if (Text[Pos] != FontCache.ReturnChar)
						lineLength++;
					this.pos++;
				}
			}
			if (Pos - this.linepos > 0)
				AddNewLineToList();
			var correctText = Text.TrimEnd(FontCache.ReturnChar, FontCache.NewLineChar);
			if (Pos < (correctText.Length - 1))
				IsCropped = true;
			this.pos = 0;
		}
		void CreateNoWrapLines() {
			lines.Add(0, Text.Length);
			lines[0].UpdateTrimmingLine(Format.Trimming, drawBounds.Width);
			if (lines[0].Length < Text.Length)
				IsCropped = true;
		}
		bool IsTrimmingElipsis {
			get {
				return trimming && (Format.Trimming == StringTrimming.EllipsisCharacter
					|| Format.Trimming == StringTrimming.EllipsisPath
					|| Format.Trimming == StringTrimming.EllipsisWord);
			}
		}
		void AddNewLineToList() {
			int length = Pos - this.linepos;
			if (length > 0 && this.trimming) {
				int wordLength = GetNextWord(Pos - 1, Text.Length);
				if (wordLength - 1 > 0) {
					length += wordLength - 1;
					this.pos += wordLength - 1;
				}
			}
			if (length > 0 && !MeasureTrailingSpaces) {
				while (length > 0 && CanRemoveSymbol(this.linepos + length - 1))
					length--;
				while (Pos < Text.Length && CanRemoveSymbol(Pos))
					this.pos++;
			}
			if (Pos < Text.Length && FontCache.IsNewLine(Text[Pos]))
				this.pos++;
			lines.Add(linepos, length);
			if (this.trimming) {
				lines[lines.Count - 1].UpdateTrimmingLine(Format.Trimming, drawBounds.Width);
			}
			this.linepos = Pos;
			this.drawTop += FontHeight;
		}
		bool CanRemoveSymbol(int pos) {
			if (char.IsWhiteSpace(Text[pos]) && !FontCache.IsNewLine(Text[pos]) &&
				(this.hotPrefixes == null || !this.hotPrefixes[pos]))
				return true;
			else return false;
		}
		public int GetNextWord(int startIndex, int textLength) {
			if (wordBreakProvider.IsWordBreakChar(Text[startIndex])) return 1;
			int newPos = startIndex + 1;
			while (newPos < textLength && !wordBreakProvider.IsWordBreakChar(Text[newPos]) && !FontCache.IsNewLine(Text[newPos]))
				newPos++;
			return newPos - startIndex;
		}
		public int GetTextWidth(int startIndex, int length) {
			if (length == 0) return 0;
			int width = 0;
			for (int i = 0; i < length; i++)
				width += widths[startIndex + i];
			return width + GetCharABCWidths(startIndex + length - 1);
		}
		int[] GetLineWidths(int length) {
			return GetLineWidths(Pos, length);
		}
		int[] GetLineWidths(int startIndex, int length) {
			if (startIndex == 0 && length == Widths.Length) return Widths;
			int[] linewidths = new int[length];
			for (int i = 0; i < length; i++)
				linewidths[i] = Widths[startIndex + i];
			return linewidths;
		}
		void DrawStringLines(IntPtr hdc) {
			this.isCliped |= DrawBounds.Height < FontHeight * lines.Count;
			this.drawBounds = CorrectRectangleTop(DrawBounds, FontHeight * lines.Count);
			for (int i = 0; i < this.lines.Count; i++) {
				Rectangle r = new Rectangle(DrawBounds.X, DrawBounds.Y + i * FontHeight, DrawBounds.Width, FontHeight);
				DrawStringLine(hdc, lines[i], r);
			}
		}
		void DrawStringLine(IntPtr hdc, TextLine line, Rectangle bounds) {
			bounds = CorrectRectangleLeft(bounds, line);
			DrawStringLine(hdc, line.Position, line.Length, bounds.X, bounds.Y, this.highLight);
			if (line.HasElipsis) {
				DrawText(hdc, line.ElipsisText, bounds.X + line.TextWidth, bounds.Y, line.ElipsisWidths);
				bounds.X += line.ElipsisWidth;
			}
			if (line.Length2 > 0)
				DrawStringLine(hdc, line.Position2, line.Length2, bounds.X + line.TextWidth, bounds.Y, this.highLight);
		}
		void DrawStringLine(IntPtr hdc, int startPos, int length, int x, int y, TextHighLight highLight) {
			if (highLight == null || !highLight.IsTextHighLighted(startPos, length))
				DrawStringLine(hdc, startPos, length, x, y);
			else {
				if (startPos < highLight.StartIndex)
					DrawStringLine(hdc, ref startPos, ref length, ref x, y, highLight.StartIndex - startPos);
				int len = length < highLight.EndIndex - startPos ? length : highLight.EndIndex - startPos;
				DrawHighLightStringLine(hdc, ref startPos, ref length, ref x, y, len);
				if (length > 0)
					DrawStringLine(hdc, startPos, length, x, y);
			}
		}
		protected virtual void DrawHighLightStringLine(IntPtr hdc, ref int startPos, ref int length, ref int x, int y, int highLightLen) {
			Rectangle hBounds = new Rectangle(x, y, GetTextWidth(startPos, highLightLen), FontHeight);
			if (!ClipedBounds.IsEmpty && hBounds.Right >= ClipedBounds.Right) {
				hBounds.Width = Math.Max(0, ClipedBounds.Right - hBounds.X);
			}
			if (highLight.BackColor != Color.Empty) {
				IntPtr brush = Win32Util.CreateSolidBrush(highLight.BackColor);
				Win32Util.FillRect(hdc, hBounds, brush);
				Win32Util.DeleteObject(brush);
			}
			int prevColor = Win32Util.GetTextColor(hdc);
			Win32Util.SetTextColor(hdc, highLight.ForeColor);
			DrawStringLine(hdc, ref startPos, ref length, ref x, y, highLightLen);
			Win32Util.SetTextColor(hdc, prevColor);
		}
		void DrawStringLine(IntPtr hdc, ref int startPos, ref int length, ref int x, int y, int highLightLen) {
			DrawStringLine(hdc, startPos, highLightLen, x, y);
			x += GetTextWidth(startPos, highLightLen);
			startPos += highLightLen;
			length -= highLightLen;
		}
		void DrawStringLine(IntPtr hdc, int startPos, int length, int x, int y) {
			int[] lineWidths = GetLineWidths(startPos, length);
			bool[] hotkeys = null;
			if (this.hotPrefixes != null) {
				hotkeys = new bool[lineWidths.Length];
				for (int i = 0; i < length; i++)
					hotkeys[i] = this.hotPrefixes[startPos + i];
			}
			string lineText = startPos == 0 && length == Text.Length ? Text : Text.Substring(startPos, length);
			DrawStringLine(hdc, lineText, x, y, lineWidths, hotkeys);
		}
		void DrawStringLine(IntPtr hdc, string lineText, int x, int y, int[] lineWidths, bool[] hotkeys) {
			if (GetHotKeysCount(hotkeys, lineText.Length) == 0)
				DrawStringLine(hdc, lineText, x, y, lineWidths);
			else DrawStringLineWithHotkeys(hdc, lineText, x, y, lineWidths, hotkeys);
		}
		protected virtual void DrawStringLineWithHotkeys(IntPtr hdc, string lineText, int textX, int textY, int[] lineWidths, bool[] hotkeys) {
			string text;
			int[] textWidths;
			int x;
			GetHotKeyInfo(lineText, textX, lineWidths, hotkeys, out text, out textWidths, out x);
			DrawStringLine(hdc, text, x, textY, textWidths);
			IntPtr oldFontHandle = IntPtr.Zero;
			if (FontCache != null)
				oldFontHandle = Win32Util.SelectObject(hdc, FontCache.FontUnderlineHandle);
			x = textX;
			textWidths = new int[1];
			for (int i = 0; i < lineText.Length; i++) {
				if (hotkeys[i]) {
					textWidths[0] = lineWidths[i];
					DrawStringLine(hdc, lineText[i].ToString(), x, textY, textWidths);
				}
				x += lineWidths[i];
			}
			if (oldFontHandle != IntPtr.Zero)
				Win32Util.SelectObject(hdc, oldFontHandle);
		}
		protected void DrawStringLine(IntPtr hdc, string lineText, int x, int y, int[] lineWidths) {
			if (lineText.Length == 0) return;
			lineText = ReplaceTabsWithSpaces(lineText);
			DrawText(hdc, lineText, x, y, lineWidths);
		}
		protected virtual void DrawText(IntPtr hdc, string text, int x, int y, int[] textWidths) {
			Win32Util.ExtTextOut(hdc, x, y, isCliped, ClipedBounds, text, textWidths);
		}
		void GetHotKeyInfo(string text, int x, int[] lineWidths, bool[] hotkeys,
			out string newText, out int[] newLineWidths, out int newX) {
			newX = x;
			newLineWidths = new int[lineWidths.Length - GetHotKeysCount(hotkeys, text.Length)];
			char[] temp = new char[text.Length];
			int index = 0;
			for (int i = 0; i < text.Length; i++) {
				if (!hotkeys[i]) {
					temp[index] = text[i];
					newLineWidths[index++] = lineWidths[i];
				}
				else {
					if (index == 0)
						newX += lineWidths[i];
					else newLineWidths[index - 1] += lineWidths[i];
				}
			}
			newText = new string(temp, 0, index);
		}
		int GetHotKeysCount(bool[] hotkeys, int length) {
			if (hotkeys == null) return 0;
			int count = 0;
			for (int i = 0; i < length; i++)
				if (hotkeys[i]) count++;
			return count;
		}
		protected virtual bool IsFontUnderline { get { return FontCache != null ? FontCache.Underline : false; } }
		string ReplaceTabsWithSpaces(string st) {
			return st.Replace(FontCache.TabStopChar, FontCache.SpaceChar);
		}
		Rectangle CorrectRectangleLeft(Rectangle bounds, TextLine line) {
			if (FormatAlignment != StringAlignment.Near) {
				int lineWidth = line.LineWidth;
				if (FormatAlignment == StringAlignment.Far)
					bounds.X += bounds.Width - lineWidth;
				else bounds.X += (bounds.Width - lineWidth) / 2;
			}
			return bounds;
		}
		Rectangle CorrectRectangleTop(Rectangle bounds) {
			return CorrectRectangleTop(bounds, FontHeight);
		}
		Rectangle CorrectRectangleTop(Rectangle bounds, int height) {
			if (bounds.Height < height && trimming) return bounds;
			if (bounds.Height != FontHeight) {
				switch(Format.LineAlignment) {
					case StringAlignment.Near:
						break;
					case StringAlignment.Far:
						bounds.Y += bounds.Height - height;
						break;
					case StringAlignment.Center:
						bounds.Y += (bounds.Height - height) / 2;
						break;
				}
			}
			return bounds;
		}
		string ValidateHotPrefix(string text) {
			this.hotPrefixes = null;
			if (format.HotkeyPrefix == HotkeyPrefix.None) return text;
			if (format.HotkeyPrefix == HotkeyPrefix.Show && !IsFontUnderline)
				this.hotPrefixes = new bool[text.Length];
			int i = 0;
			bool hotPrefix = false;
			while (i < text.Length) {
				if (text[i] == '&' && !hotPrefix) {
					hotPrefix = true;
					text = text.Remove(i, 1);
				}
				else {
					if (this.hotPrefixes != null && text[i] != '&')
						this.hotPrefixes[i] = hotPrefix;
					hotPrefix = false;
					i++;
				}
			}
			return text;
		}
		Rectangle ApplyTextUtilsOffsets(Rectangle bounds) {
			bounds.X += TextUtils.LeftOffset;
			bounds.Y += TextUtils.TopOffset;
			bounds.Width -= TextUtils.RightOffset + TextUtils.LeftOffset;
			bounds.Height -= TextUtils.BottomOffset + TextUtils.TopOffset;
			return bounds;
		}
		bool IWordBreakProvider.IsWordBreakChar(char ch) {
			return Char.IsWhiteSpace(ch);
		}
	}
	internal class TextLine {
		int position;
		int length;
		int position2;
		int length2;
		TextLines lines;
		string elipsisText;
		int elipsisWidth;
		int[] elipsisWidths;
		static int[] emptyWidths = new int[0];
		public TextLine(TextLines lines) {
			this.lines = lines;
			elipsisText = "";
			elipsisWidths = emptyWidths;
		}
		public int Position { get { return position; } set { position = value; } }
		public int Length { get { return length; } set { length = value; } }
		public int Position2 { get { return position2; } set { position2 = value; } }
		public int Length2 { get { return length2; } set { length2 = value; } }
		public int TextWidth { get { return GetTextWidth(position, length); } }
		public int Text2Width { get { return GetTextWidth(position2, length2); } }
		public string ElipsisText { get { return elipsisText; } }
		public int ElipsisWidth { get { return elipsisWidth; } }
		public int[] ElipsisWidths { get { return elipsisWidths; } }
		public int LineWidth { get { return TextWidth + ElipsisWidth + Text2Width; } }
		public int GetCharWidth(int index) {
			return GetCharWidth(index, false);
		}
		public int GetCharWidth(int index, bool includeItalic) {
			index += Position;
			return Lines.Widths[index] + Lines.GetCharABCWidths(index);
		}
		public bool HasElipsis {
			get { return elipsisText.Length > 0; }
			set {
				if (HasElipsis == value) return;
				elipsisWidth = 0;
				elipsisWidths = emptyWidths;
				if (!value) {
					elipsisText = "";
				}
				else {
					elipsisText = "...";
					elipsisWidths = GetCharactersWidth(elipsisText);
					for (int i = 0; i < elipsisWidths.Length; i++)
						elipsisWidth += elipsisWidths[i];
				}
			}
		}
		public void UpdateTrimmingLine(StringTrimming trimming, int drawBoundsWidth) {
			UpdateTrimmingLine(trimming, drawBoundsWidth, false, string.Empty);
		}
		public void UpdateTrimmingLine(StringTrimming trimming, int drawBoundsWidth, bool wordWrap, string text) {
			if (trimming == StringTrimming.None || (TextWidth <= drawBoundsWidth && !wordWrap)) return;
			int len = 0;
			int lineLength = Length;
			int lineWidth = 0;
			HasElipsis = GetIsTrimmingElipsis(trimming);
			int drawWidth = drawBoundsWidth - ElipsisWidth;
			switch (trimming) {
				case StringTrimming.Character:
				case StringTrimming.EllipsisCharacter:
					TrimmTextToWord(text, wordWrap, lineLength, drawWidth, out len, out lineWidth);
					while (lineWidth + GetCharWidth(len, true) <= drawWidth) {
						lineWidth += GetCharWidth(len++);
						if (wordWrap && ShouldProceed(IsLineLast(text), len)) return;
					}
					break;
				case StringTrimming.Word:
				case StringTrimming.EllipsisWord:
					TrimmTextToWord(text, wordWrap, lineLength, drawWidth, out len, out lineWidth);
					if (wordWrap) {
						if (len == 0) {
							len = text.Length - Position;
							break;
						}
						if (string.Equals(Lines.Draw.Text[Position + len - 1].ToString(), " ")) {
							var pos = Position + len - 1;
							while (pos >= Position) {
								if (!string.Equals(Lines.Draw.Text[pos].ToString(), " ")) break;
								pos--;
							}
							if (pos != (Position + len - 1)) len = pos - Position + 1;
						}
					}
					break;
				case StringTrimming.EllipsisPath:
					int firstWidth = 0;
					int lastWidth = 0;
					int pos2 = lineLength - 1;
					int charWidth = GetCharWidth(0);
					while (firstWidth + lastWidth + charWidth < drawWidth) {
						if (firstWidth <= lastWidth)
							firstWidth += GetCharWidth(len++);
						else lastWidth += GetCharWidth(pos2--);
						if ((Lines.Widths.Length == (len + Position) || pos2 < 0) && !IsLineFirst) {
							Position2 = Lines.Widths.Length - 1;
							Length2 = 0;
							HasElipsis = false;
							return;
						}
						charWidth = firstWidth <= lastWidth ? GetCharWidth(len) : GetCharWidth(pos2);
					}
					pos2++;
					Position2 = pos2;
					Length2 = lineLength - pos2;
					if (wordWrap && !IsLineFirst) Position2 += Position;
					break;
			}
			Length = len;
		}
		void TrimmTextToWord(string text, bool wordWrap, int lineLength, int drawWidth, out int len, out int usedLineWidth) {
			len = 0;
			usedLineWidth = 0;
			var length = wordWrap ? Lines.Draw.Text.Length : lineLength;
			int wordLength = GetNextWord(Position, length);
			int lineWidth = GetTextWidth(Position, wordLength);
			while (lineWidth <= drawWidth) {
				usedLineWidth = lineWidth;
				len += wordLength;
				if (wordWrap && ShouldProceed(IsLineLast(text), len)) return;
				wordLength = GetNextWord(Position + len, Position + length);
				lineWidth = GetTextWidth(Position, len + wordLength);
			}
		}
		bool ShouldProceed(bool isLineLast, int currentLen) {
			if (isLineLast) {
				HasElipsis = false;
				return true;
			}
			if (currentLen >= Length) return true;
			return false;
		}
		bool IsLineFirst { get { return Position == 0; } }
		bool IsLineLast(string text) {
			if (string.IsNullOrEmpty(text)) return true;
			var lastSymbolIndex = GetLastValuableSymbolIndex(text);
			if ((Position + Length - 1) == lastSymbolIndex || lastSymbolIndex < 0) return true;
			return false;
		}
		char[] whiteSpaces;
		char[] WhiteSpaces {
			get {
				if (whiteSpaces != null) return whiteSpaces;
				List<char> res = new List<char>();
				res.AddRange(Environment.NewLine.ToCharArray());
				res.AddRange(
					new char[] {
						' ',
						(char)System.Globalization.UnicodeCategory.SpaceSeparator,
						(char)System.Globalization.UnicodeCategory.ParagraphSeparator
					});
				whiteSpaces = res.ToArray();
				return whiteSpaces;
			}
		}
		int GetLastValuableSymbolIndex(string text) {
			if (string.IsNullOrEmpty(text)) return -1;
			var res = text.TrimEnd(WhiteSpaces);
			return (res.Length - 1);
		}
		protected bool GetIsTrimmingElipsis(StringTrimming trimming) {
			return trimming == StringTrimming.EllipsisCharacter
				|| trimming == StringTrimming.EllipsisPath
				|| trimming == StringTrimming.EllipsisWord;
		}
		protected TextLines Lines { get { return lines; } }
		protected int[] Widths { get { return lines.Widths; } }
		protected int[] GetCharactersWidth(string text) { return lines.GetCharacterWidths(text); }
		protected int GetTextWidth(int pos, int len) {
			return Lines.Draw.GetTextWidth(pos, len);
		}
		protected int GetNextWord(int startIndex, int textLength) {
			return Lines.Draw.GetNextWord(startIndex, textLength);
		}
	}
	internal class TextLines {
		TextOutDraw draw;
		int count = 0;
		TextLine[] lines;
		public int Count { get { return count; } }
		public TextLines(TextOutDraw draw) {
			this.draw = draw;
			this.lines = new TextLine[1];
		}
		public TextLine this[int index] {
			get {
				return (index < count ? lines[index] : null);
			}
		}
		public void Add(int position, int length) {
			TextLine line = AddLine();
			line.Position = position;
			line.Length = length;
		}
		public int Width {
			get {
				int width = 0;
				for (int i = 0; i < Count; i++) {
					int tWidth = this[i].LineWidth;
					if (width < tWidth)
						width = tWidth;
				}
				return width;
			}
		}
		public int[] Widths { get { return Draw.Widths; } }
		public int[] GetCharacterWidths(string text) {
			return Draw.GetCharactersWidth(text);
		}
		public int GetCharABCWidths(int position) {
			return Draw.GetCharABCWidths(position);
		}
		public TextOutDraw Draw { get { return draw; } }
		protected TextLine AddLine() {
			TextLine line = new TextLine(this);
			Add(line);
			return line;
		}
		protected void Add(TextLine line) {
			if (count >= lines.Length) {
				TextLine[] newLines = new TextLine[lines.Length + 3];
				Array.Copy(lines, newLines, lines.Length);
				this.lines = newLines;
			}
			lines[count++] = line;
		}
	}
}
