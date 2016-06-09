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
namespace DevExpress.Office.PInvoke {
	static partial class PInvokeSafeNativeMethods {
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hGdiObj);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool DeleteObject(IntPtr hObject);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int SetTextColor(IntPtr hdc, int color);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int SetBkMode(IntPtr hdc, int iBkMode);
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool GetWindowExtEx(IntPtr hdc, out Win32.SIZE lpSize);
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool SetWindowExtEx(
			IntPtr hdc,	   
			int nXExtent,  
			int nYExtent,  
			ref Win32.SIZE lpSize  
			);
		[DllImport("gdiplus.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern UInt32 GdipEmfToWmfBits(IntPtr hEmf, UInt32 bufferSize, byte[] buffer, int mappingMode, Win32.EmfToWmfBitsFlags flags);
		[DllImport("Gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal extern static UInt32 GetMetaFileBitsEx(IntPtr hEmf, UInt32 cbBuffer, byte[] buffer);
		[DllImport("Gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal extern static UInt32 GetEnhMetaFileBits(IntPtr hEmf, UInt32 cbBuffer, byte[] buffer);
		[DllImport("Gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal extern static IntPtr SetMetaFileBitsEx(UInt32 cbBuffer, byte[] buffer);
		[DllImport("Gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal extern static IntPtr SetEnhMetaFileBits(UInt32 bufferSize, byte[] buffer);
		[DllImport("Gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal extern static IntPtr SetWinMetaFileBits(UInt32 bufferSize, byte[] buffer, IntPtr hdc, ref Win32.METAFILEPICT mfp);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool DeleteEnhMetaFile(IntPtr hemf);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool DeleteMetaFile(IntPtr hemf);
		[Flags]
		internal enum TextAlignment {
			TA_NOUPDATECP = 0,
			TA_UPDATECP = 1,
			TA_LEFT = 0,
			TA_RIGHT = 2,
			TA_CENTER = 6,
			TA_HORZ_ALIGNMENT_MASK = 6,
			TA_TOP = 0,
			TA_BOTTOM = 8,
			TA_BASELINE = 24,
			TA_RTLREADING = 256,
			TA_MASK = (TA_BASELINE | TA_CENTER | TA_UPDATECP | TA_RTLREADING),
			VTA_BASELINE = TA_BASELINE,
			VTA_LEFT = TA_BOTTOM,
			VTA_RIGHT = TA_TOP,
			VTA_CENTER = TA_CENTER,
			VTA_BOTTOM = TA_RIGHT,
			VTA_TOP = TA_LEFT
		}
		[DllImport("gdi32.dll")]
		internal static extern int GetTextAlign(IntPtr hdc);
		[DllImport("gdi32.dll")]
		internal static extern int SetTextAlign(IntPtr hdc, int fMode);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool RectVisible(IntPtr hdc, [In] ref Win32.RECT lprc);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool GetWindowOrgEx(IntPtr hdc, out Win32.POINT lpPoint);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool SetWindowOrgEx(IntPtr hdc, int x, int y, ref Win32.POINT lpPoint);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool GetCharABCWidthsFloat(IntPtr hdc, uint uFirstChar, uint uLastChar,
			[Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct)] DevExpress.Office.PInvoke.Win32.ABCFLOAT[] lpabc);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool EnumMetaFile(IntPtr hdc, IntPtr hmf, DevExpress.Office.PInvoke.Win32.EnumMetaFileDelegate lpMetaFunc, IntPtr lParam);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool EnumEnhMetaFile(IntPtr hdc, IntPtr hemf, DevExpress.Office.PInvoke.Win32.EnumMetaFileDelegate lpMetaFunc, IntPtr lParam, ref Win32.RECT lpRect);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr GetStockObject(int fnObject);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr CreateHatchBrush(int fnStyle, int clrref);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, uint dwRop);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int SetROP2(IntPtr hdc, int fnDrawMode);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);
	}
	public static partial class Win32 {
		#region MapMode
		public enum MapMode {
			Text = 1,
			LowMetric = 2,
			HighMetric = 3,
			LowEnglish = 4,
			HighEnglish = 5,
			Twips = 6,
			Isotropic = 7,
			Anisotropic = 8
		}
		#endregion
		public enum EmfToWmfBitsFlags {
			EmfToWmfBitsFlagsDefault = 0x00000000,
			EmfToWmfBitsFlagsEmbedEmf = 0x00000001,
			EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,
			EmfToWmfBitsFlagsNoXORClip = 0x00000004
		};
		#region METAFILEPICT
		[StructLayout(LayoutKind.Sequential)]
		public struct METAFILEPICT {
			int mapMode;
			int xExt;
			int yExt;
			IntPtr hMf;
			public METAFILEPICT(MapMode mapMode, int xExt, int yExt) {
				this.mapMode = (int)mapMode;
				this.xExt = xExt;
				this.yExt = yExt;
				hMf = IntPtr.Zero;
			}
		}
		#endregion
		[SecuritySafeCritical]
		public static IntPtr SelectObject(IntPtr hdc, IntPtr hGdiObj) {
			return PInvokeSafeNativeMethods.SelectObject(hdc, hGdiObj);
		}
		[SecuritySafeCritical]
		public static bool DeleteObject(IntPtr hObject) {
			return PInvokeSafeNativeMethods.DeleteObject(hObject);
		}
		[SecuritySafeCritical]
		public static void SetTextColor(IntPtr hdc, Color color) {
			PInvokeSafeNativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(color));
		}
		public enum BkMode {
			TRANSPARENT = 1,
			OPAQUE = 2,
		}
		[SecuritySafeCritical]
		public static BkMode SetBkMode(IntPtr hdc, BkMode iBkMode) {
			return (BkMode)PInvokeSafeNativeMethods.SetBkMode(hdc, (int)iBkMode);
		}
		[SecuritySafeCritical]
		public static bool GetWindowExtEx(IntPtr hdc, out SIZE lpSize) {
			return PInvokeSafeNativeMethods.GetWindowExtEx(hdc, out lpSize);
		}
		[SecuritySafeCritical]
		public static bool SetWindowExtEx(
			IntPtr hdc,	   
			int nXExtent,  
			int nYExtent,  
			ref SIZE lpSize  
			) {
			return PInvokeSafeNativeMethods.SetWindowExtEx(hdc, nXExtent, nYExtent, ref lpSize);
		}
		[SecuritySafeCritical]
		public static byte[] GdipEmfToWmfBits(IntPtr hEmf, MapMode mappingMode, EmfToWmfBitsFlags flags) {
			UInt32 bufferSize = PInvokeSafeNativeMethods.GdipEmfToWmfBits(hEmf, 0, null, (int)mappingMode, flags);
			byte[] buffer = new byte[bufferSize];
			PInvokeSafeNativeMethods.GdipEmfToWmfBits(hEmf, bufferSize, buffer, (int)mappingMode, flags);
			return buffer;
		}
		[SecuritySafeCritical]
		public static byte[] GetMetaFileBits(IntPtr hEmf) {
			UInt32 size = PInvokeSafeNativeMethods.GetMetaFileBitsEx(hEmf, 0, null);
			byte[] buffer = new byte[size];
			PInvokeSafeNativeMethods.GetMetaFileBitsEx(hEmf, size, buffer);
			return buffer;
		}
		[SecuritySafeCritical]
		public static byte[] GetEnhMetafileBits(IntPtr hEmf) {
			UInt32 size = PInvokeSafeNativeMethods.GetEnhMetaFileBits(hEmf, 0, null);
			byte[] buffer = new byte[size];
			PInvokeSafeNativeMethods.GetEnhMetaFileBits(hEmf, size, buffer);
			return buffer;
		}
		[SecuritySafeCritical]
		public static IntPtr SetMetaFileBitsEx(int cbBuffer, byte[] buffer) {
			return PInvokeSafeNativeMethods.SetMetaFileBitsEx((UInt32)cbBuffer, buffer);
		}
		[SecuritySafeCritical]
		public static IntPtr SetMetaFileBitsEx(long cbBuffer, byte[] buffer) {
			return PInvokeSafeNativeMethods.SetMetaFileBitsEx((UInt32)cbBuffer, buffer);
		}
		[SecuritySafeCritical]
		public static IntPtr SetEnhMetaFileBits(int bufferSize, byte[] buffer) {
			return PInvokeSafeNativeMethods.SetEnhMetaFileBits((UInt32)bufferSize, buffer);
		}
		[SecuritySafeCritical]
		public static IntPtr SetEnhMetaFileBits(long bufferSize, byte[] buffer) {
			return PInvokeSafeNativeMethods.SetEnhMetaFileBits((UInt32)bufferSize, buffer);
		}
		[SecuritySafeCritical]
		public static IntPtr SetWinMetaFileBits(int bufferSize, byte[] buffer, IntPtr hdc, ref Win32.METAFILEPICT mfp) {
			return PInvokeSafeNativeMethods.SetWinMetaFileBits((UInt32)bufferSize, buffer, hdc, ref mfp);
		}
		[SecuritySafeCritical]
		public static IntPtr SetWinMetaFileBits(long bufferSize, byte[] buffer, IntPtr hdc, ref Win32.METAFILEPICT mfp) {
			return PInvokeSafeNativeMethods.SetWinMetaFileBits((UInt32)bufferSize, buffer, hdc, ref mfp);
		}
		[SecuritySafeCritical]
		public static bool DeleteEnhMetaFile(IntPtr hEmf) {
			return PInvokeSafeNativeMethods.DeleteEnhMetaFile(hEmf);
		}
		[SecuritySafeCritical]
		public static bool DeleteMetaFile(IntPtr hEmf) {
			return PInvokeSafeNativeMethods.DeleteMetaFile(hEmf);
		}
		[SecuritySafeCritical]
		public static int SetTextAlign(IntPtr hdc, int value) {
			return PInvokeSafeNativeMethods.SetTextAlign(hdc, value);
		}
		[SecuritySafeCritical]
		public static int SetTextAlign(IntPtr hdc, StringFormat format) {
			PInvokeSafeNativeMethods.TextAlignment align;
			switch (format.Alignment) {
				default:
				case StringAlignment.Near:
					align = PInvokeSafeNativeMethods.TextAlignment.TA_LEFT;
					break;
				case StringAlignment.Center:
					align = PInvokeSafeNativeMethods.TextAlignment.TA_CENTER;
					break;
				case StringAlignment.Far:
					align = PInvokeSafeNativeMethods.TextAlignment.TA_RIGHT;
					break;
			}
			int existingValue = PInvokeSafeNativeMethods.GetTextAlign(hdc);
			if ((existingValue & (int)PInvokeSafeNativeMethods.TextAlignment.TA_HORZ_ALIGNMENT_MASK) != (int)align) {
				existingValue &= ~(int)PInvokeSafeNativeMethods.TextAlignment.TA_HORZ_ALIGNMENT_MASK;
				existingValue |= (int)align;
				return PInvokeSafeNativeMethods.SetTextAlign(hdc, existingValue);
			}
			else
				return existingValue;
		}
		[SecuritySafeCritical]
		public static bool RectVisible(IntPtr hdc, ref RECT lprc) {
			return PInvokeSafeNativeMethods.RectVisible(hdc, ref lprc);
		}
		[SecuritySafeCritical]
		public static bool GetWindowOrgEx(IntPtr hdc, out Win32.POINT lpPoint) {
			return PInvokeSafeNativeMethods.GetWindowOrgEx(hdc, out lpPoint);
		}
		[SecuritySafeCritical]
		public static bool SetWindowOrgEx(IntPtr hdc, int x, int y, ref Win32.POINT lpPoint) {
			return PInvokeSafeNativeMethods.SetWindowOrgEx(hdc, x, y, ref lpPoint);
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct ABCFLOAT {
			public float abcA;
			public float abcB;
			public float abcC;
			public float GetWidth() {
				return abcA + abcB + abcC;
			}
		}
		[SecuritySafeCritical]
		public static bool GetCharABCWidthsFloat(IntPtr hdc, char firstChar, char lastChar, ABCFLOAT[] result) {
			return PInvokeSafeNativeMethods.GetCharABCWidthsFloat(hdc, firstChar, lastChar, result);
		}
		public delegate int EnumMetaFileDelegate(IntPtr hdc, IntPtr handleTable, IntPtr metafileRecord, int objectCount, IntPtr clientData);
		[SecuritySafeCritical]
		public static bool EnumMetaFile(IntPtr hdc, IntPtr hmf, EnumMetaFileDelegate lpMetaFunc, IntPtr lParam) {
			return PInvokeSafeNativeMethods.EnumMetaFile(hdc, hmf, lpMetaFunc, lParam);
		}
		[SecuritySafeCritical]
		public static bool EnumEnhMetaFile(IntPtr hdc, IntPtr hemf, EnumMetaFileDelegate lpMetaFunc, IntPtr lParam, ref Win32.RECT lpRect) {
			return PInvokeSafeNativeMethods.EnumEnhMetaFile(hdc, hemf, lpMetaFunc, lParam, ref lpRect);
		}
		#region StockObject
		public enum StockObject {
			WHITE_BRUSH = 0,
			LTGRAY_BRUSH = 1,
			GRAY_BRUSH = 2,
			DKGRAY_BRUSH = 3,
			BLACK_BRUSH = 4,
			NULL_BRUSH = 5,
			HOLLOW_BRUSH = NULL_BRUSH,
			WHITE_PEN = 6,
			BLACK_PEN = 7,
			NULL_PEN = 8,
			OEM_FIXED_FONT = 10,
			ANSI_FIXED_FONT = 11,
			ANSI_VAR_FONT = 12,
			SYSTEM_FONT = 13,
			DEVICE_DEFAULT_FONT = 14,
			DEFAULT_PALETTE = 15,
			SYSTEM_FIXED_FONT = 16,
			DEFAULT_GUI_FONT = 17,
			DC_BRUSH = 18,
			DC_PEN = 19,
		}
		#endregion
		[SecuritySafeCritical]
		public static IntPtr GetStockObject(StockObject obj) {
			return PInvokeSafeNativeMethods.GetStockObject((int)obj);
		}
		public enum HatchBrushStyle {
			UpwardDiagonal = 3,
			Cross = 4,
			DiagonalCross = 5,
			DownwardDiagonal = 2,
			Horizontal = 0,
			Vertical = 1
		}
		[SecuritySafeCritical]
		public static IntPtr CreateHatchBrush(HatchBrushStyle style, Color color) {
			return PInvokeSafeNativeMethods.CreateHatchBrush((int)style, ColorTranslator.ToWin32(color));
		}
		#region TernaryRasterOperation
		public enum TernaryRasterOperation {
			SRCCOPY = 0x00CC0020, 
			SRCPAINT = 0x00EE0086, 
			SRCAND = 0x008800C6, 
			SRCINVERT = 0x00660046, 
			SRCERASE = 0x00440328, 
			NOTSRCCOPY = 0x00330008, 
			NOTSRCERASE = 0x001100A6, 
			MERGECOPY = 0x00C000CA, 
			MERGEPAINT = 0x00BB0226, 
			PATCOPY = 0x00F00021, 
			PATPAINT = 0x00FB0A09, 
			PATINVERT = 0x005A0049, 
			DSTINVERT = 0x00550009, 
			BLACKNESS = 0x00000042, 
			WHITENESS = 0x00FF0062, 
			CAPTUREBLT = 0x40000000, 
		}
		#endregion
		[SecuritySafeCritical]
		public static bool PatBlt(IntPtr hdc, int x, int y, int width, int height, TernaryRasterOperation rop) {
			return PInvokeSafeNativeMethods.PatBlt(hdc, x, y, width, height, (uint)rop);
		}
		#region BinaryRasterOperation
		public enum BinaryRasterOperation {
			R2_BLACK = 1,
			R2_NOTMERGEPEN = 2,
			R2_MASKNOTPEN = 3,
			R2_NOTCOPYPEN = 4,
			R2_MASKPENNOT = 5,
			R2_NOT = 6,
			R2_XORPEN = 7,
			R2_NOTMASKPEN = 8,
			R2_MASKPEN = 9,
			R2_NOTXORPEN = 10,
			R2_NOP = 11,
			R2_MERGENOTPEN = 12,
			R2_COPYPEN = 13,
			R2_MERGEPENNOT = 14,
			R2_MERGEPEN = 15,
			R2_WHITE = 16
		}
		#endregion
		[SecuritySafeCritical]
		public static BinaryRasterOperation SetROP2(IntPtr hdc, BinaryRasterOperation rop) {
			return (BinaryRasterOperation)PInvokeSafeNativeMethods.SetROP2(hdc, (int)rop);
		}
		#region PenStyle
		[Flags]
		public enum PenStyle {
			PS_SOLID = 0,
			PS_DASH = 1,	   
			PS_DOT = 2,	   
			PS_DASHDOT = 3,	   
			PS_DASHDOTDOT = 4,	   
			PS_NULL = 5,
			PS_INSIDEFRAME = 6,
			PS_USERSTYLE = 7,
			PS_ALTERNATE = 8,
			PS_STYLE_MASK = 0x0000000F,
			PS_ENDCAP_ROUND = 0x00000000,
			PS_ENDCAP_SQUARE = 0x00000100,
			PS_ENDCAP_FLAT = 0x00000200,
			PS_ENDCAP_MASK = 0x00000F00,
			PS_JOIN_ROUND = 0x00000000,
			PS_JOIN_BEVEL = 0x00001000,
			PS_JOIN_MITER = 0x00002000,
			PS_JOIN_MASK = 0x0000F000,
			PS_COSMETIC = 0x00000000,
			PS_GEOMETRIC = 0x00010000,
			PS_TYPE_MASK = 0x000F0000
		}
		#endregion
		[SecuritySafeCritical]
		public static IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, int crColor) {
			return PInvokeSafeNativeMethods.CreatePen((int)fnPenStyle, nWidth, crColor);
		}
		[SecuritySafeCritical]
		public static void MoveTo(IntPtr hdc, int x, int y) {
			PInvokeSafeNativeMethods.MoveToEx(hdc, x, y, IntPtr.Zero);
		}
		[SecuritySafeCritical]
		public static void LineTo(IntPtr hdc, int x, int y) {
			PInvokeSafeNativeMethods.LineTo(hdc, x, y);
		}
	}
}
