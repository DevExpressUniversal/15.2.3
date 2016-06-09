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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.Utils.Text {
	public class HdcDpiModifier : IDisposable {
		readonly Graphics gr;
		readonly Size viewPort;
		readonly int dpi;
		Win32Util.SIZE oldWindowExt = new Win32Util.SIZE();
		Win32Util.SIZE oldViewportExt = new Win32Util.SIZE();
		int oldMapMode;
		public HdcDpiModifier(Graphics gr, Size viewPort, int dpi) {
			this.gr = gr;
			this.viewPort = viewPort;
			this.dpi = dpi;
			ApplyHDCDpi();
		}
		protected virtual int Dpi { get { return dpi; } }
		public void Dispose() {
			RestoreHDCDpi();
		}
		[SecuritySafeCritical]
		protected virtual void ApplyHDCDpi() {
			int graphicsDpiX = (int)Math.Round(gr.DpiX);
			int graphicsDpiY = (int)Math.Round(gr.DpiY);
			IntPtr hdc = gr.GetHdc();
			try {
				oldMapMode = Win32Util.Win32API.GetMapMode(hdc);
				Win32Util.Win32API.SetMapMode(hdc, Win32Util.MM_ANISOTROPIC);
				Win32Util.Win32API.SetWindowExtEx(hdc, viewPort.Width * Dpi / graphicsDpiX, viewPort.Height * Dpi / graphicsDpiY, ref oldWindowExt);
				Win32Util.Win32API.SetViewportExtEx(hdc, viewPort.Width, viewPort.Height, ref oldViewportExt);
			}
			finally {
				gr.ReleaseHdc(hdc);
			}
		}
		[System.Security.SecuritySafeCritical]
		protected virtual void RestoreHDCDpi() {
			IntPtr hdc = gr.GetHdc();
			try {
				Win32Util.Win32API.SetViewportExtEx(hdc, oldViewportExt.cx, oldViewportExt.cy, ref oldViewportExt);
				Win32Util.Win32API.SetWindowExtEx(hdc, oldWindowExt.cx, oldWindowExt.cy, ref oldWindowExt);
				Win32Util.Win32API.SetMapMode(hdc, oldMapMode);
			}
			finally {
				gr.ReleaseHdc(hdc);
			}
		}
	}
	public class HdcDpiToDocuments : HdcDpiModifier {
		public HdcDpiToDocuments(Graphics gr, Size viewPort)
			: base(gr, viewPort, (int)DevExpress.XtraPrinting.GraphicsDpi.Document) {
		}
	}
	public class HdcDpiEmptyModifier : HdcDpiModifier {
		public HdcDpiEmptyModifier(Graphics gr, Size viewPort)
			: base(gr, viewPort, (int)DevExpress.XtraPrinting.GraphicsDpi.Pixel) {
		}
		protected override void ApplyHDCDpi() {
		}
		protected override void RestoreHDCDpi() {
		}
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct TEXTMETRIC {
		public int tmHeight;
		public int tmAscent;
		public int tmDescent;
		public int tmInternalLeading;
		public int tmExternalLeading;
		public int tmAveCharWidth;
		public int tmMaxCharWidth;
		public int tmWeight;
		public int tmOverhang;
		public int tmDigitizedAspectX;
		public int tmDigitizedAspectY;
		public char tmFirstChar;
		public char tmLastChar;
		public char tmDefaultChar;
		public char tmBreakChar;
		public byte tmItalic;
		public byte tmUnderlined;
		public byte tmStruckOut;
		public byte tmPitchAndFamily;
		public byte tmCharSet;
	}
	[System.Security.SecuritySafeCritical]	
	internal class Win32Util {
		[StructLayout(LayoutKind.Sequential)]
		public struct SIZE {
			public int cx;
			public int cy;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), ComVisible(false)]
		public class LOGFONT {
			public int lfHeight;
			public int lfWidth;
			public int lfEscapement;
			public int lfOrientation;
			public int lfWeight;
			public byte lfItalic;
			public byte lfUnderline;
			public byte lfStrikeOut;
			public byte lfCharSet;
			public byte lfOutPrecision;
			public byte lfClipPrecision;
			public byte lfQuality;
			public byte lfPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string lfFaceName;
		}
		private const int ETO_CLIPPED = 0x0004;
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int x;
			public int y;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct OUTLINETEXTMETRIC {
			public uint otmSize;
			public TEXTMETRIC otmTextMetrics;
			public byte otmFiller;
			public PANOSE otmPanoseNumber;
			public uint otmfsSelection;
			public uint otmfsType;
			public int otmsCharSlopeRise;
			public int otmsCharSlopeRun;
			public int otmItalicAngle;
			public uint otmEMSquare;
			public int otmAscent;
			public int otmDescent;
			public uint otmLineGap;
			public uint otmsCapEmHeight;
			public uint otmsXHeight;
			public RECT otmrcFontBox;
			public int otmMacAscent;
			public int otmMacDescent;
			public uint otmMacLineGap;
			public uint otmusMinimumPPEM;
			public POINT otmptSubscriptSize;
			public POINT otmptSubscriptOffset;
			public POINT otmptSuperscriptSize;
			public POINT otmptSuperscriptOffset;
			public uint otmsStrikeoutSize;
			public int otmsStrikeoutPosition;
			public int otmsUnderscoreSize;
			public int otmsUnderscorePosition;
			public uint otmpFamilyName;
			public uint otmpFaceName;
			public uint otmpStyleName;
			public uint otmpFullName;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct PANOSE {
			public byte bFamilyType;
			public byte bSerifStyle;
			public byte bWeight;
			public byte bProportion;
			public byte bContrast;
			public byte bStrokeVariation;
			public byte bArmStyle;
			public byte bLetterform;
			public byte bMidline;
			public byte bXHeight;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct KerningPair {
			public char wFirst;
			public char wSecond;
			public int iKernAmount;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct ABC {
			public int abcA;
			public int abcB;
			public int abcC;
		}
		public const int MM_ANISOTROPIC = 8;
		[System.Security.SuppressUnmanagedCodeSecurity]
		public class Win32API {
			[DllImport("gdi32.dll")]
			public static extern int SetMapMode(IntPtr hdc, int mapMode);
			[DllImport("gdi32.dll")]
			public static extern int GetMapMode(IntPtr hdc);
			[DllImport("gdi32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SetWindowExtEx(
				IntPtr hdc,	   
				int nXExtent,  
				int nYExtent,  
				ref SIZE lpSize  
				);
			[DllImport("gdi32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SetViewportExtEx(
				IntPtr hdc,	   
				int nXExtent,  
				int nYExtent,  
				ref SIZE lpSize  
				);
			[DllImport("USER32.dll", CharSet = CharSet.Auto)]
			public static extern int DrawText(IntPtr hdc, String text, int textLen, ref RECT gdiRect, int format);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateSolidBrush(int color);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteObject(IntPtr hObject);
			[DllImport("User32.dll")]
			public static extern int FillRect(IntPtr hdc, ref RECT r, IntPtr brush);
			[DllImport("gdi32.dll")]
			public static extern int GetTextColor(IntPtr hdc);
			[DllImport("gdi32.dll")]
			public static extern int SetBkMode(IntPtr hdc, int mode);
			[DllImport("gdi32.dll")]
			public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);
			[DllImport("gdi32.dll")]
			public static extern int SetTextColor(IntPtr hdc, int color);
			[DllImport("gdi32.dll")]
			public static extern int SetBkColor(IntPtr hdc, int color);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
			public static extern int ExtTextOut(IntPtr hdc, int x, int y, int options,
				ref RECT clip, string str, int len,
				[In, MarshalAs(UnmanagedType.LPArray)] int[] widths);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
			public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC lptm);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
			public static extern uint GetOutlineTextMetrics(IntPtr hdc, uint strSize, IntPtr lptm);
			[DllImport("gdi32.dll", EntryPoint = "GetCharWidthW")]
			public static extern bool GetCharWidth(IntPtr hdc, uint firstChar, uint lastChar,
				[Out, MarshalAs(UnmanagedType.LPArray)] int[] widths);
			[DllImport("gdi32.dll", EntryPoint="GetCharABCWidthsW")]
			public static extern bool GetCharABCWidths(IntPtr hdc, uint firstChar, uint lastChar,
				[Out, MarshalAs(UnmanagedType.LPArray)] ABC[] widths);
			[DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
			public static extern int GetKerningPairs(IntPtr hdc, int nNumPairs,
				[In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
				KerningPair[] kerningPairs);
			[DllImport("gdi32.dll")]
			public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
			[DllImport("GDI32.dll")]
			public static extern IntPtr GetStockObject(int fnObject);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateFont(
				int nHeight,			   
				int nWidth,				
				int nEscapement,		   
				int nOrientation,		  
				int fnWeight,			  
				int fdwItalic,		   
				int fdwUnderline,		
				int fdwStrikeOut,		
				int fdwCharSet,		  
				int fdwOutputPrecision,  
				int fdwClipPrecision,	
				int fdwQuality,		  
				int fdwPitchAndFamily,   
				string lpszFace		   
				);
		}
		public static IntPtr CreateSolidBrush(IntPtr hdc) {
			return Win32API.CreateSolidBrush(GetTextColor(hdc));
		}
		public static IntPtr CreateSolidBrush(int color) {
			return Win32API.CreateSolidBrush(color);
		}
		public static IntPtr CreateSolidBrush(Color color) {
			return Win32API.CreateSolidBrush(ColorTranslator.ToWin32(color));
		}
		public static void DeleteObject(IntPtr hObject) {
			Win32API.DeleteObject(hObject);
		}
		public static void FillRect(IntPtr hdc, Rectangle bounds, IntPtr hBrush) {
			RECT rect = GetRect(bounds);
			Win32API.FillRect(hdc, ref rect, hBrush);
		}
		public static int GetTextColor(IntPtr hdc) {
			return Win32API.GetTextColor(hdc);
		}
		public const int TRANSPARENT = 1;
		public static void SetBkMode(IntPtr hdc, int mode) {
			Win32API.SetBkMode(hdc, mode);
		}
		public static IntPtr SelectObject(IntPtr hdc, IntPtr handle) {
			return Win32API.SelectObject(hdc, handle);
		}
		public static void SetTextColor(IntPtr hdc, Color color) {
			Win32API.SetTextColor(hdc, ColorTranslator.ToWin32(color));
		}
		public static void SetTextColor(IntPtr hdc, int color) {
			Win32API.SetTextColor(hdc, color);
		}
		public static void SetBkColor(IntPtr hdc, Color color) {
			Win32API.SetBkColor(hdc, ColorTranslator.ToWin32(color));
		}
		public static void ExtTextOut(IntPtr hdc, int x, int y, bool isCliped, Rectangle clip, string str, int[] spacings) {
			RECT rect = GetRect(clip);
			if (!isCliped)
				rect.top = y;
			int format = isCliped ? ETO_CLIPPED : 0;
			Win32API.SetBkColor(hdc, 1);
			IntPtr saveBrush = SelectObject(hdc, Win32API.GetStockObject(0)); 
			Win32API.ExtTextOut(hdc, x, y, format, ref rect, str, str.Length, spacings);
			SelectObject(hdc, saveBrush);
		}
		public static TEXTMETRIC GetTextMetrics(IntPtr hdc) {
			TEXTMETRIC lptm;
			bool rc = Win32API.GetTextMetrics(hdc, out lptm);
			return lptm;
		}
		public static bool GetOutlineTextMetrics(IntPtr hdc, ref OUTLINETEXTMETRIC lptm) {
			uint size = Win32API.GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
			if (size == 0) return false;
			IntPtr buffer = Marshal.AllocHGlobal((int)size);
			bool res = true;
			try {
				res = Win32API.GetOutlineTextMetrics(hdc, size, buffer) > 0;
				if (res)
					lptm = (OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(OUTLINETEXTMETRIC));
			}
			finally {
				Marshal.FreeHGlobal(buffer);
			}
			return res;
		}
		public static int[] GetCharWidth(IntPtr hdc, uint firstChar, uint lastChar) {
			uint charsCount = lastChar - firstChar + 1;
			int[] widths = new int[charsCount];
			bool res = Win32API.GetCharWidth(hdc, firstChar, lastChar, widths);
			return widths;
		}
		public static int[] GetCharABCWidths(IntPtr hdc, uint firstChar, uint lastChar) {
			uint charsCount = lastChar - firstChar + 1;
			int[] widths = new int[charsCount];
			ABC[] abcWidths = new ABC[charsCount];
			bool res = Win32API.GetCharABCWidths(hdc, firstChar, lastChar, abcWidths);
			for (int i = 0; i < widths.Length; i++) {
				if (res)
					widths[i] = abcWidths[i].abcC < 0 ? -abcWidths[i].abcC : 0;
				else widths[i] = 0;
			}
			return widths;
		}
		public static KerningPair[] GetKerningPairs(IntPtr hdc) {
			int count = Win32API.GetKerningPairs(hdc, 0, null);
			if (count == 0) return null;
			KerningPair[] pairs = new KerningPair[count];
			Win32API.GetKerningPairs(hdc, count, pairs);
			return pairs;
		}
		const int LOGPIXELSX = 88;
		const int LOGPIXELSY = 90;
		public static int GetLogicPixelPerInchX(IntPtr hdc) {
			return Win32API.GetDeviceCaps(hdc, LOGPIXELSX);
		}
		public static int GetLogicPixelPerInchY(IntPtr hdc) {
			return Win32API.GetDeviceCaps(hdc, LOGPIXELSY);
		}
		static RECT GetRect(Rectangle bounds) {
			RECT rect;
			rect.top = bounds.Top;
			rect.left = bounds.Left;
			rect.bottom = bounds.Bottom;
			rect.right = bounds.Right;
			return rect;
		}
	}
	public static class HdcPixelUtils {
		public static int GetLogicPixelPerInchX(IntPtr hdc) {
			return Win32Util.GetLogicPixelPerInchX(hdc);
		}
		public static int GetLogicPixelPerInchY(IntPtr hdc) {
			return Win32Util.GetLogicPixelPerInchY(hdc);
		}
	}
}
