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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using DevExpress.Office.PInvoke;
using DevExpress.Export.Xl;
namespace DevExpress.Office.Drawing {
	#region Enums
	#region FontFamilyIndex
	[Flags]
	public enum FontFamilyIndex : byte {
		DontCare = 0,
		Roman = 1,
		Swiss = 2,
		Modern = 3,
		Script = 4,
		Decorative = 5,
	}
	#endregion
	#region TextMetricsPitchAndFamily
	[Flags]
	public enum TextMetricsPitchAndFamily : byte {
		FixedPitch = 1,
		Vector = 2,
		TrueType = 4,
		Device = 8,
	}
	#endregion
	#region SpreadsheetFontWeight
	public enum SpreadsheetFontWeight {
		Normal = 400,
		Bold = 700,
	}
	#endregion
	#region FontPitchAndFamily
	[Flags]
	public enum FontPitchAndFamily : byte {
		DefaultPitch = 0,
		FixedPitch = 1,
		VariablePitch = 2,
		DontCare = (0 << 4),
		Roman = (1 << 4),
		Swiss = (2 << 4),
		Modern = (3 << 4),
		Script = (4 << 4),
		Decorative = (5 << 4),
	}
	#endregion
	#endregion
	#region FontsInfoHelper
	[SecuritySafeCritical]
	internal static class FontsInfoHelper {
		#region Imports
		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		static extern int EnumFontFamiliesEx(IntPtr hdc, [In] IntPtr lpLogfont, EnumFontExDelegate lpEnumFontFamExProc, IntPtr lParam, uint dwFlags);
		#endregion
		#region Fields
		static readonly Dictionary<string, PInvokeSafeNativeMethods.LogFontCharSet> charSets;
		static readonly Dictionary<string, FontFamilyIndex> fontFamilies;
		#endregion
		#region Constructor
		static FontsInfoHelper() {
			charSets = new Dictionary<string, PInvokeSafeNativeMethods.LogFontCharSet>();
			fontFamilies = new Dictionary<string, FontFamilyIndex>();
			Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
			IntPtr hdc = graphics.GetHdc();
			PInvokeSafeNativeMethods.LOGFONT lf = new PInvokeSafeNativeMethods.LOGFONT();
			lf.lfFaceName = "";
			lf.lfCharSet = PInvokeSafeNativeMethods.LogFontCharSet.DEFAULT;
			IntPtr plogFont = Marshal.AllocHGlobal(Marshal.SizeOf(lf));
			Marshal.StructureToPtr(lf, plogFont, true);
			EnumFontFamiliesEx(hdc, plogFont, EnumFontFamiliesExProc, IntPtr.Zero, 0);
			graphics.ReleaseHdc(hdc);
		}
		#endregion
		#region GetFontCharSet
		public static PInvokeSafeNativeMethods.LogFontCharSet GetFontCharSet(string fontName) {
			PInvokeSafeNativeMethods.LogFontCharSet result;
			if(!charSets.TryGetValue(fontName, out result))
				result = PInvokeSafeNativeMethods.LogFontCharSet.DEFAULT;
			return result;
		}
		#endregion
		#region GetFontFamily
		public static FontFamilyIndex GetFontFamily(string fontName) {
			FontFamilyIndex result;
			if(!fontFamilies.TryGetValue(fontName, out result))
				result = FontFamilyIndex.DontCare;
			return result;
		}
		#endregion
		#region EnumFontFamiliesExProc
		static int EnumFontFamiliesExProc(ref Enumlogfontex logFontEx, IntPtr lpntme, int fontType, int i) {
			if(charSets.ContainsKey(logFontEx.elfLogFont.lfFaceName))
				return 1;
			charSets.Add(logFontEx.elfLogFont.lfFaceName, logFontEx.elfLogFont.lfCharSet);
			fontFamilies.Add(logFontEx.elfLogFont.lfFaceName, (FontFamilyIndex) (logFontEx.elfLogFont.lfPitchAndFamily >> 4));
			return 1;
		}
		#endregion
		#region WinAPI Structures
		public const int FaceSize = 32;
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct Enumlogfontex {
			public PInvokeSafeNativeMethods.LOGFONT elfLogFont;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = FaceSize)] public string elfFullName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = FaceSize)] public string elfStyle;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = FaceSize)] public string elfScript;
		}
		delegate int EnumFontExDelegate(ref Enumlogfontex lpelfe, IntPtr lpntme, int fontType, int lParam);
		#endregion
	}
	#endregion
	#region GDIHeightCalculator
	[SecuritySafeCritical]
	public static class GDIHeightCalculator {
		#region Imports
		[DllImport("gdi32.dll", CharSet = CharSet.Ansi)]
		static extern bool GetTextMetrics(IntPtr hdc, out PInvokeSafeNativeMethods.TEXTMETRIC lptm); 
		#endregion
		[ThreadStatic]
		static Graphics graphics;
		#region GetRowHeightByFont
		public static int GetRowHeightByFont(string fontName, double fontSize, bool italic, bool bold, bool strikeThrough, XlUnderlineType underlineMode, XlScriptType scriptMode) {
			ShortFontInfo shortFontInfo = ShortFontInfo.FromParameters(fontName, fontSize, italic, scriptMode, bold, underlineMode, strikeThrough);
			int result = GetRowHeightPixels(shortFontInfo);
			if(result > 2047)
				result = 2047;
			return result;
		}
		#endregion
		#region GetRowHeightPixels
		static int GetRowHeightPixels(ShortFontInfo shortFontInfo) {
			const int defaultValue = 20;
			if (graphics == null)
				graphics = Graphics.FromHwnd(IntPtr.Zero);
			IntPtr hDC = graphics.GetHdc();
			PInvokeSafeNativeMethods.LOGFONT logFont = GetLogFont(shortFontInfo);
			IntPtr logFontPtr = PInvokeSafeNativeMethods.CreateFont(logFont.lfHeight, logFont.lfWidth, logFont.lfEscapement, logFont.lfOrientation, logFont.lfWeight, logFont.lfItalic, logFont.lfUnderline, logFont.lfStrikeOut, (int) logFont.lfCharSet, logFont.lfOutPrecision, logFont.lfClipPrecision, logFont.lfQuality, logFont.lfPitchAndFamily, logFont.lfFaceName);
			if(logFontPtr == IntPtr.Zero) {
				graphics.ReleaseHdc(hDC);
				return defaultValue;
			}
			IntPtr prevObjectPtr = PInvokeSafeNativeMethods.SelectObject(hDC, logFontPtr);
			if(prevObjectPtr == IntPtr.Zero) {
				graphics.ReleaseHdc(hDC);
				return defaultValue;
			}
			FontHeightMetrics fontHeightMetrics = GetFontHeightMetrics(hDC, shortFontInfo.FontName);
			if(fontHeightMetrics == null) {
				graphics.ReleaseHdc(hDC);
				return defaultValue;				
			}
			int underlineDelta = 0;
			if(((int) shortFontInfo.UnderlineMode & 0x20) != 0) {
				underlineDelta = UnderLineAdvancedInfo.CalcMetricsFromUnderlineMode(hDC, fontHeightMetrics, shortFontInfo.UnderlineMode);
			}
			if(shortFontInfo.ScriptMode != XlScriptType.Baseline) {
				CalMetricsFromScriptMode(shortFontInfo, fontHeightMetrics, hDC, logFont);
			}
			PInvokeSafeNativeMethods.SelectObject(hDC, prevObjectPtr);
			PInvokeSafeNativeMethods.DeleteObject(logFontPtr);
			graphics.ReleaseHdc(hDC);
			int rowHeight = CalcRowHeight(underlineDelta, fontHeightMetrics);
			return rowHeight;
		}
		#endregion
		#region CalcRowHeight
		static int CalcRowHeight(int underlineDelta, FontHeightMetrics fontHeightMetrics) {
			fontHeightMetrics.Descent -= underlineDelta;
			fontHeightMetrics.Height -= underlineDelta;
			int halfOfExternalLeading = Math.Max(fontHeightMetrics.ExternalLeading - fontHeightMetrics.ExternalLeading / 2 - 1, 0);
			int outlineSize = Math.Max(halfOfExternalLeading + fontHeightMetrics.Descent, fontHeightMetrics.ExternalLeading / 2 + fontHeightMetrics.InternalLeading);
			int minSize = fontHeightMetrics.Height >= 2 ? 2 : 1;
			int delta = Math.Max(outlineSize, minSize);
			int rowHeight = underlineDelta + fontHeightMetrics.Ascent - fontHeightMetrics.InternalLeading + delta * 2 + 1;
			return rowHeight;
		}
		#endregion
		#region CalMetricsFromScriptMode
		static void CalMetricsFromScriptMode(ShortFontInfo shortFontInfo, FontHeightMetrics fontHeightMetrics, IntPtr hDC, PInvokeSafeNativeMethods.LOGFONT logFont) {
			int scriptDelta = CalcMetricsFromScriptModeCore(fontHeightMetrics, shortFontInfo.ScriptMode == XlScriptType.Superscript, logFont);
			logFont.lfHeight = -logFont.lfHeight;
			IntPtr scriptedFont = PInvokeSafeNativeMethods.CreateFont(logFont.lfHeight, logFont.lfWidth, logFont.lfEscapement, logFont.lfOrientation, logFont.lfWeight, logFont.lfItalic, logFont.lfUnderline, logFont.lfStrikeOut, (int) logFont.lfCharSet, logFont.lfOutPrecision, logFont.lfClipPrecision, logFont.lfQuality, logFont.lfPitchAndFamily, logFont.lfFaceName);
			if(scriptedFont == IntPtr.Zero) {
				return;
			}
			IntPtr prevObjectPtr = PInvokeSafeNativeMethods.SelectObject(hDC, scriptedFont);
			if(prevObjectPtr == IntPtr.Zero) {
				return;
			}
			FontHeightMetrics scriptedFontHeightMetrics = GetFontHeightMetrics(hDC, shortFontInfo.FontName);
			if(scriptedFontHeightMetrics == null) {
				PInvokeSafeNativeMethods.SelectObject(hDC, prevObjectPtr);
				return;
			}
			if(shortFontInfo.ScriptMode == XlScriptType.Superscript) {
				if(fontHeightMetrics.Ascent < scriptedFontHeightMetrics.Ascent + scriptDelta) {
					fontHeightMetrics.Ascent = scriptedFontHeightMetrics.Ascent + scriptDelta;
				}
			}
			else {
				if(fontHeightMetrics.Descent < scriptedFontHeightMetrics.Descent + scriptDelta) {
					fontHeightMetrics.Descent = scriptedFontHeightMetrics.Descent + scriptDelta;
				}
			}
			fontHeightMetrics.Height = fontHeightMetrics.Ascent + fontHeightMetrics.Descent;
			PInvokeSafeNativeMethods.DeleteObject(scriptedFont);
		}
		#endregion
		#region GetLogFont
		static PInvokeSafeNativeMethods.LOGFONT GetLogFont(ShortFontInfo shortFontInfo) {
			PInvokeSafeNativeMethods.LOGFONT result = new PInvokeSafeNativeMethods.LOGFONT();
			result.lfFaceName = shortFontInfo.FontName;
			result.lfPitchAndFamily = (byte) ((int) shortFontInfo.FontFamily << 4);
			result.lfCharSet = shortFontInfo.CharSet;
			result.lfHeight = -MulDiv(shortFontInfo.FontSizeTwips, (int)DocumentModelDpi.DpiY, 1440); 
			if(shortFontInfo.CharSet == PInvokeSafeNativeMethods.LogFontCharSet.ANSI && shortFontInfo.FontSizeTwips < 160 && shortFontInfo.FontSizeTwips >= 0) {
				result.lfFaceName = "Small Fonts";
				result.lfQuality = 2;
			}
			if(shortFontInfo.FontSizeTwips >= 0 && result.lfHeight > -1)
				result.lfHeight = -1;
			result.lfWeight = (int) shortFontInfo.FontWeight;
			result.lfItalic = Convert.ToByte(shortFontInfo.Italic);
			result.lfStrikeOut = Convert.ToByte(shortFontInfo.StrikeThrough);
			int lcid = CultureInfo.CurrentCulture.LCID;
			if(lcid == 0x412 || lcid == 0x404 || lcid == 0x804 ) {
				if(shortFontInfo.CharSet != PInvokeSafeNativeMethods.LogFontCharSet.SHIFTJIS
				   && shortFontInfo.CharSet != PInvokeSafeNativeMethods.LogFontCharSet.HANGEUL
				   && shortFontInfo.CharSet != PInvokeSafeNativeMethods.LogFontCharSet.CHINESEBIG5
				   && shortFontInfo.CharSet != PInvokeSafeNativeMethods.LogFontCharSet.GB2312)
					result.lfClipPrecision |= 40;
			}
			result.lfQuality = 5;
			return result;
		}
		#endregion
		#region GetFontHeightMetrics
		static FontHeightMetrics GetFontHeightMetrics(IntPtr hdc, string fontName) {
			PInvokeSafeNativeMethods.TEXTMETRIC textmetric;
			GetTextMetrics(hdc, out textmetric);
			int lcid = CultureInfo.CurrentCulture.LCID;
			bool magicLCID = lcid == 0x404 || lcid == 0x411 || lcid <= 0x412 || lcid == 0x804 ;
			if(magicLCID && fontName == "Tahoma" && textmetric.tmMaxCharWidth / 2 >= textmetric.tmAveCharWidth)
				textmetric.tmAveCharWidth = textmetric.tmMaxCharWidth / 2;
			return GetFontHeightMetricsCore(textmetric);
		}
		#endregion
		#region GetFontHeightMetricsCore
		static FontHeightMetrics GetFontHeightMetricsCore(PInvokeSafeNativeMethods.TEXTMETRIC textmetricw) {
			FontHeightMetrics fontHeightMetrics = new FontHeightMetrics();
			fontHeightMetrics.Height = textmetricw.tmHeight;
			fontHeightMetrics.Descent = textmetricw.tmDescent;
			fontHeightMetrics.InternalLeading = textmetricw.tmInternalLeading;
			fontHeightMetrics.ExternalLeading = textmetricw.tmExternalLeading;
			fontHeightMetrics.Ascent = textmetricw.tmAscent;
			fontHeightMetrics.PitchAndFamily = GetPitchAndFamily((TextMetricsPitchAndFamily) textmetricw.tmPitchAndFamily);
			return fontHeightMetrics;
		}
		static FontPitchAndFamily GetPitchAndFamily(TextMetricsPitchAndFamily pitchAndFamily) {
			bool isTmpfDevice = (pitchAndFamily & TextMetricsPitchAndFamily.Device) != 0;
			TextMetricsPitchAndFamily isTmpfFixedPitch = pitchAndFamily & TextMetricsPitchAndFamily.FixedPitch;
			uint temp = (uint)(isTmpfFixedPitch | (TextMetricsPitchAndFamily)(2 * Convert.ToInt32((pitchAndFamily & (TextMetricsPitchAndFamily)0xA) == TextMetricsPitchAndFamily.Vector)) | (TextMetricsPitchAndFamily)(2 * Convert.ToInt32(!isTmpfDevice)));
			FontPitchAndFamily result = (FontPitchAndFamily) (temp & 0xFFFFFFCFu | 8 * (uint) (pitchAndFamily & TextMetricsPitchAndFamily.TrueType | TextMetricsPitchAndFamily.Vector));
			return result;
		}
		#endregion
		#region UnderLineAdvancedInfo
		static class UnderLineAdvancedInfo {
			static int offset { get; set; }
			static int Width1Line { get; set; }
			static int Pos1Line { get; set; }
			static int Width2Lines { get; set; }
			static void RecalculateManual(int descent) {
				Width2Lines = 1;
				if(descent / 4 >= 1) {
					Width2Lines = descent / 4;
				}
				Width1Line = Width2Lines;
				Pos1Line = (descent - 3 * Width2Lines + 1) / 2;
				offset = Pos1Line + 2 * Width2Lines;
			}
			static bool RecalculateFromWinAPI(IntPtr hdc) {
				uint cjCopy = PInvokeSafeNativeMethods.GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
				if(cjCopy == 0)
					return false;
				IntPtr buffer = Marshal.AllocHGlobal((int) cjCopy);
				uint result = PInvokeSafeNativeMethods.GetOutlineTextMetrics(hdc, cjCopy, buffer);
				if(result == 0) {
					Marshal.FreeHGlobal(buffer);
					return false;
				}
				PInvokeSafeNativeMethods.OUTLINETEXTMETRIC pvIn = (PInvokeSafeNativeMethods.OUTLINETEXTMETRIC) Marshal.PtrToStructure(buffer, typeof(PInvokeSafeNativeMethods.OUTLINETEXTMETRIC));
				Pos1Line = Math.Abs(pvIn.otmsUnderscorePosition);
				Width2Lines = Math.Max(pvIn.otmsUnderscoreSize, 1);
				Width1Line = Width2Lines;
				offset = Pos1Line + 2 * Width2Lines;
				if(3 * Width2Lines < Pos1Line) {
					offset = Pos1Line;
					Pos1Line = (Pos1Line - Width2Lines) / 2;
				}
				Marshal.FreeHGlobal(buffer);
				return true;
			}
			public static int CalcMetricsFromUnderlineMode(IntPtr hdc, FontHeightMetrics fontHeightMetrics, XlUnderlineType underlineMode) {
				bool manualOutlineMetrics = true;
				if((fontHeightMetrics.PitchAndFamily & FontPitchAndFamily.Swiss) != 0) {
					manualOutlineMetrics = !RecalculateFromWinAPI(hdc);
				}
				if(manualOutlineMetrics) {
					RecalculateManual(fontHeightMetrics.Descent);
				}
				int linesCount = (int) underlineMode & 0xF;
				int cycles = 0;
				int pos2Lines = 0;
				while(cycles < 10) {
					cycles++;
					if(Width1Line < 1)
						Width1Line = 1;
					if(Pos1Line < 0)
						Pos1Line = 0;
					pos2Lines = offset;
					if(pos2Lines < 0)
						pos2Lines = 0;
					if(Width2Lines < 1)
						Width2Lines = 1;
					if(Pos1Line > 0 && Pos1Line + Width1Line >= pos2Lines)
						Pos1Line = Pos1Line / 2;
					int underlineHeight = linesCount == 2 ? Width2Lines + pos2Lines : Width1Line + Pos1Line;
					if(underlineHeight <= fontHeightMetrics.Descent)
						break;
					if(!manualOutlineMetrics) {
						manualOutlineMetrics = true;
						RecalculateManual(fontHeightMetrics.Descent);
						continue;
					}
					if(fontHeightMetrics.Descent >= 0 && fontHeightMetrics.Descent <= 2) {
						Pos1Line = fontHeightMetrics.Descent - 3;
						pos2Lines = fontHeightMetrics.Descent - 3 + 2;
						Width2Lines = 1;
						break;
					}
					Pos1Line = MulDiv(Pos1Line, fontHeightMetrics.Descent, underlineHeight);
					Width1Line = MulDiv(Width1Line, fontHeightMetrics.Descent, underlineHeight);
					offset = MulDiv(pos2Lines, fontHeightMetrics.Descent, underlineHeight);
					Width2Lines = MulDiv(Width2Lines, fontHeightMetrics.Descent, underlineHeight);
				}
				int underlineDelta = Width2Lines + pos2Lines - Pos1Line;
				fontHeightMetrics.Descent += underlineDelta;
				fontHeightMetrics.Height = fontHeightMetrics.Descent + fontHeightMetrics.Ascent;
				return underlineDelta;
			}
		}
		#endregion
		#region CalcMetricsFromScriptModeCore
		static int CalcMetricsFromScriptModeCore(FontHeightMetrics fontHeightMetrics, bool isSuperScript, PInvokeSafeNativeMethods.LOGFONT logfont) {
			int charHeight = fontHeightMetrics.Height - fontHeightMetrics.InternalLeading;
			int result = isSuperScript ? charHeight / 2 : charHeight / 5;
			result = Math.Abs(result);
			logfont.lfHeight = Math.Max(MulDiv(charHeight, 2, 3), 1);
			return result;
		}
		#endregion
		#region MulDiv
		static int MulDiv(int number, int numerator, int denominator) {
			return (int) (((long) number * numerator + (denominator >> 1)) / denominator);
		}
		#endregion
	}
	#endregion
	#region ShortFontInfo
	class ShortFontInfo {
		public static ShortFontInfo FromParameters(string fontName, double fontSize, bool italic, XlScriptType scriptMode, bool bold, XlUnderlineType underlineMode, bool strikeThrough, bool shadow = false, bool outline = false) {
			ShortFontInfo result = new ShortFontInfo();
			result.FontName = fontName;
			result.FontSizeTwips = (int) (fontSize * 20);
			result.Italic = italic;
			result.StrikeThrough = strikeThrough;
			result.FontWeight = SpreadsheetFontWeight.Normal;
			if(bold)
				result.FontWeight = SpreadsheetFontWeight.Bold;
			result.ScriptMode = scriptMode;
			result.UnderlineMode = underlineMode;
			result.FontFamily = FontsInfoHelper.GetFontFamily(fontName);
			result.CharSet = FontsInfoHelper.GetFontCharSet(fontName);
			return result;
		}
		public string FontName { get; set; }
		public int FontSizeTwips { get; set; } 
		public SpreadsheetFontWeight FontWeight { get; set; }
		public XlScriptType ScriptMode { get; set; }
		public XlUnderlineType UnderlineMode { get; set; }
		public FontFamilyIndex FontFamily { get; set; }
		public PInvokeSafeNativeMethods.LogFontCharSet CharSet { get; set; }
		public bool Italic { get; set; }
		public bool StrikeThrough { get; set; }
	}
	#endregion
	#region FontHeightMetrics
	public class FontHeightMetrics {
		public int Height { get; set; }
		public int Ascent { get; set; }
		public int Descent { get; set; }
		public int InternalLeading { get; set; }
		public int ExternalLeading { get; set; }
		public FontPitchAndFamily PitchAndFamily { get; set; }
	}
	#endregion
}
