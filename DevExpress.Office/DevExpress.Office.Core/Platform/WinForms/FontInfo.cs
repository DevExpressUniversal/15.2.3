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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.Drawing {
	#region GdiPlusFontInfo
	public class GdiPlusFontInfo : FontInfo {
		#region Fields
		string fontName;
		string fontFamilyName;
		Font font;
		IntPtr gdiFontHandle;
		Dictionary<char, bool> characterDrawingAbilityTable;
		static readonly int systemFontQuality = CalculateSystemFontQuality();
		#endregion
		public GdiPlusFontInfo(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline)
			: base(measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline) {
		}
		#region Properties
		public IntPtr GdiFontHandle { get { return gdiFontHandle; } set { gdiFontHandle = value; } }
		public override bool Bold { get { return font.Bold; } }
		public override bool Italic { get { return font.Italic; } }
		public override bool Underline { get { return font.Underline; } }
		public override bool Strikeout { get { return font.Strikeout; } }
		public override float Size { get { return font.Size; } }
		public override string Name { get { return fontName; } }
		public override string FontFamilyName { get { return fontFamilyName; } }
		public override Font Font { get { return font; } }
		#endregion
		protected internal override void Initialize(FontInfoMeasurer measurer) {
			this.characterDrawingAbilityTable = new Dictionary<char, bool>();
			this.gdiFontHandle = CreateGdiFontInLayoutUnits(measurer);
		}
		protected virtual IntPtr CreateGdiFontInLayoutUnits(FontInfoMeasurer measurer) {
			return CreateGdiFontInLayoutUnits(Font, measurer);
		}
		protected internal override void CreateFont(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			GdiPlusFontInfoMeasurer gdiPlusMeasurer = (GdiPlusFontInfoMeasurer)measurer;
			this.font = gdiPlusMeasurer.CreateFont(fontName, doubleFontSize / 2f, fontBold, fontItalic, fontStrikeout, fontUnderline);
			this.fontName = font.Name; 
			this.fontFamilyName = font.FontFamily.Name;
		}
		protected internal override float CalculateFontSizeInPoints() {
			return DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(font);
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (font != null) {
						font.Dispose();
						font = null;
					}
				}
				if (this.gdiFontHandle != IntPtr.Zero) {
					Win32.DeleteObject(this.gdiFontHandle);
					this.gdiFontHandle = IntPtr.Zero;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override void Dispose() {
			base.Dispose();
			GC.SuppressFinalize(this);
		}
		~GdiPlusFontInfo() {
			Dispose(false);
		}
		#endregion
		#region Platform specific methods
		#region static
		internal PInvokeSafeNativeMethods.OUTLINETEXTMETRIC? GetOutlineTextMetrics(Graphics gr) {
			lock (gr) {
				IntPtr hdc = gr.GetHdc();
				try {
					IntPtr oldFontHandle = Win32.SelectObject(hdc, gdiFontHandle);
					try {
						return GetOutlineTextMetrics(hdc);
					}
					finally {
						Win32.SelectObject(hdc, oldFontHandle);
					}
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
		}
		protected internal static int CalculateActualFontQuality(int quality) {
			if (quality != 0)
				return 0;
			return systemFontQuality;
		}
		static int CalculateSystemFontQuality() {
			try {
				switch (SystemInformation.FontSmoothingType) {
					case 1: 
						return 4; 
					case 2: 
						return 6; 
					default:
						return 0;
				}
			}
			catch (NotSupportedException) { 
				return 0;
			}
		}
		[System.Security.SecuritySafeCritical]
		internal static IntPtr CreateGdiFontInLayoutUnits(Font font, FontInfoMeasurer measurer) {
			PInvokeSafeNativeMethods.LOGFONT lf = new PInvokeSafeNativeMethods.LOGFONT();
			font.ToLogFont(lf);
			lf.lfHeight = -(int)Math.Round(font.Size / measurer.UnitConverter.FontSizeScale);
			lf.lfQuality = (byte)CalculateActualFontQuality((int)lf.lfQuality);
			IntPtr hFont = PInvokeSafeNativeMethods.CreateFont(lf.lfHeight, lf.lfWidth, lf.lfEscapement, lf.lfOrientation, lf.lfWeight, lf.lfItalic, lf.lfUnderline, lf.lfStrikeOut, 1, lf.lfOutPrecision, lf.lfClipPrecision, lf.lfQuality, lf.lfPitchAndFamily, lf.lfFaceName);
			return hFont;
		}
		[System.Security.SecuritySafeCritical]
		internal static IntPtr CreateGdiFont(Font font, DocumentLayoutUnitConverter unitConverter) {
			float fontSizeInLayoutUnits = CalculateFontSizeInLayoutUnits(font, unitConverter);
			PInvokeSafeNativeMethods.LOGFONT lf = new PInvokeSafeNativeMethods.LOGFONT();
			font.ToLogFont(lf);
			lf.lfHeight = (int)-Math.Round(fontSizeInLayoutUnits / unitConverter.FontSizeScale);
			lf.lfQuality = (byte)CalculateActualFontQuality((int)lf.lfQuality);
			IntPtr hFont = PInvokeSafeNativeMethods.CreateFont(lf.lfHeight, lf.lfWidth, lf.lfEscapement, lf.lfOrientation, lf.lfWeight, lf.lfItalic, lf.lfUnderline, lf.lfStrikeOut, 1, lf.lfOutPrecision, lf.lfClipPrecision, lf.lfQuality, lf.lfPitchAndFamily, lf.lfFaceName);
			return hFont;
		}
		static float CalculateFontSizeInLayoutUnits(Font font, DocumentLayoutUnitConverter unitConverter) {
			switch (font.Unit) {
				case GraphicsUnit.Document:
					return unitConverter.DocumentsToFontUnitsF(font.Size);
				case GraphicsUnit.Inch:
					return unitConverter.InchesToFontUnitsF(font.Size);
				case GraphicsUnit.Millimeter:
					return unitConverter.MillimetersToFontUnitsF(font.Size);
				case GraphicsUnit.Point:
					return unitConverter.PointsToFontUnitsF(font.Size);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		[System.Security.SecuritySafeCritical]
		static PInvokeSafeNativeMethods.OUTLINETEXTMETRIC? GetOutlineTextMetrics(IntPtr hdc) {
			uint bufferSize = PInvokeSafeNativeMethods.GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
			if (bufferSize == 0)
				return null;
			IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
			try {
				if (PInvokeSafeNativeMethods.GetOutlineTextMetrics(hdc, bufferSize, buffer) != 0) {
					return (PInvokeSafeNativeMethods.OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(PInvokeSafeNativeMethods.OUTLINETEXTMETRIC));
				}
			}
			finally {
				Marshal.FreeHGlobal(buffer);
			}
			return null;
		}
		#endregion
		protected internal override void CalculateFontVerticalParameters(FontInfoMeasurer measurer) {
			Font font = Font;
			FontFamily family = font.FontFamily;
			FontStyle style = font.Style;
			float sizeInUnits = font.Size / measurer.UnitConverter.FontSizeScale;
			float emSize = family.GetEmHeight(style);
			float ratio = sizeInUnits / emSize;
			Ascent = (int)Math.Ceiling(family.GetCellAscent(style) * ratio);
			Descent = (int)Math.Ceiling(family.GetCellDescent(style) * ratio);
			LineSpacing = (int)Math.Ceiling(family.GetLineSpacing(style) * ratio);
		}		
		protected internal override void CalculateUnderlineAndStrikeoutParameters(FontInfoMeasurer measurer) {
			GdiPlusFontInfoMeasurer gdiPlusMeasurer = (GdiPlusFontInfoMeasurer)measurer;
			PInvokeSafeNativeMethods.OUTLINETEXTMETRIC? otm = GetOutlineTextMetrics(gdiPlusMeasurer.MeasureGraphics);
			if (otm != null)
				CalculateUnderlineAndStrikeoutParametersCore(otm.Value);
		}
		protected internal override int CalculateFontCharset(FontInfoMeasurer measurer) {
			return CalculateFontCharsetCore(measurer);
		}
		[System.Security.SecuritySafeCritical]
		int CalculateFontCharsetCore(FontInfoMeasurer measurer) {
			GdiPlusFontInfoMeasurer gdiPlusMeasurer = (GdiPlusFontInfoMeasurer)measurer;
			Graphics gr = gdiPlusMeasurer.MeasureGraphics;
			lock (gr) {
				IntPtr hdc = gr.GetHdc();
				try {
					IntPtr oldFontHandle = Win32.SelectObject(hdc, gdiFontHandle);
					try {
						return (int)Win32.GetFontCharset(hdc);
					}
					finally {
						Win32.SelectObject(hdc, oldFontHandle);
					}
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
		}
		#endregion
		internal void CalculateUnderlineAndStrikeoutParametersCore(PInvokeSafeNativeMethods.OUTLINETEXTMETRIC otm) {
			this.UnderlinePosition = -otm.otmsUnderscorePosition;
			this.UnderlineThickness = otm.otmsUnderscoreSize;
			this.StrikeoutPosition = otm.otmsStrikeoutPosition;
			this.StrikeoutThickness = (int)otm.otmsStrikeoutSize;
			this.SubscriptSize = (Size)otm.otmptSubscriptSize;
			this.SubscriptOffset = otm.otmptSubscriptOffset;
			this.SuperscriptOffset = otm.otmptSuperscriptOffset;
			Point offset = this.SuperscriptOffset;
			offset.Y = -offset.Y;
			this.SuperscriptOffset = offset;
			this.SuperscriptSize = (Size)otm.otmptSuperscriptSize;
		}
		protected internal override void CalculateSuperscriptOffset(FontInfo baseFontInfo) {
			int result = baseFontInfo.SuperscriptOffset.Y;
			int y = baseFontInfo.AscentAndFree - this.AscentAndFree + result;
			if (y < 0)
				result -= y;
			this.SuperscriptOffset = new Point(this.SuperscriptOffset.X, result);
		}
		protected internal override void CalculateSubscriptOffset(FontInfo baseFontInfo) {
			int result = baseFontInfo.SubscriptOffset.Y;
			int maxOffset = baseFontInfo.LineSpacing - this.LineSpacing + this.AscentAndFree - baseFontInfo.AscentAndFree;
			if (result > maxOffset)
				result = maxOffset;
			this.SubscriptOffset = new Point(this.SubscriptOffset.X, result);
		}
		public virtual bool CanDrawCharacter(UnicodeRangeInfo unicodeRangeInfo, Graphics gr, char character) {
			bool result;
			if (!characterDrawingAbilityTable.TryGetValue(character, out result)) {
				result = CalculateCanDrawCharacter(unicodeRangeInfo, gr, character);
				characterDrawingAbilityTable.Add(character, result);
			}
			return result;
		}
		protected internal virtual bool CalculateCanDrawCharacter(UnicodeRangeInfo unicodeRangeInfo, Graphics gr, char character) {
			UnicodeSubrange unicodeSubRange = unicodeRangeInfo.LookupSubrange(character);
			if (unicodeSubRange != null) {
				System.Diagnostics.Debug.Assert(unicodeSubRange.Bit < 126); 
				BitArray bits = CalculateSupportedUnicodeSubrangeBits(unicodeRangeInfo, gr);
				return bits[unicodeSubRange.Bit];
			}
			else
				return false;
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual List<FontCharacterRange> GetFontUnicodeRanges(Graphics gr) {
			lock (gr) {
				IntPtr hdc = gr.GetHdc();
				try {
					Win32.SelectObject(hdc, GdiFontHandle);
					int size = Win32.GetFontUnicodeRanges(hdc, IntPtr.Zero);
					IntPtr glyphSet = Marshal.AllocHGlobal(size);
					try {
						PInvokeSafeNativeMethods.GetFontUnicodeRanges(hdc, glyphSet);
						List<FontCharacterRange> result = new List<FontCharacterRange>();
						int glyphCount = Marshal.ReadInt32(glyphSet, 12);
						for (int i = 0; i < glyphCount; i++) {
							int low = (UInt16)Marshal.ReadInt16(glyphSet, 16 + i * 4);
							int count = (UInt16)Marshal.ReadInt16(glyphSet, 18 + i * 4);
							result.Add(new FontCharacterRange(low, low + count - 1));
						}
						return result;
					}
					finally {
						Marshal.FreeHGlobal(glyphSet);
					}
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
		}
		protected internal virtual BitArray CalculateSupportedUnicodeSubrangeBits(UnicodeRangeInfo unicodeRangeInfo, Graphics gr) {
			Win32.FONTSIGNATURE fontSignature = new Win32.FONTSIGNATURE();
			lock (gr) {
				IntPtr hdc = gr.GetHdc();
				try {
					Win32.SelectObject(hdc, GdiFontHandle);
					Win32.FontCharset fontCharset = Win32.GetFontCharsetInfo(hdc, ref fontSignature);
					if (fontCharset == Win32.FontCharset.Default) 
						return new BitArray(128, false);
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
			Int32[] unicodeSubrangeBits = fontSignature.fsUsb;
			System.Diagnostics.Debug.Assert(unicodeSubrangeBits.Length == 4);
			return new BitArray(unicodeSubrangeBits);
		}
	}
	#endregion
	#region GdiFontInfo
	public class GdiFontInfo : GdiPlusFontInfo {
		public GdiFontInfo(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline)
			: base(measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline) {
		}
	}
	#endregion
}
