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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
namespace DevExpress.Office.Drawing {
	#region GdiPainter (abstract class)
	public abstract class GdiPainter : GdiPlusPainter {
		#region Fields
		readonly DocumentLayoutUnitConverter unitConverter;
		const int initialBufferItemCount = 64;
		int glyphsBufferSize;
		IntPtr glyphsBuffer;
		int characterWidthsBufferSize;
		IntPtr characterWidthsBuffer;
		#endregion
		public GdiPainter(IGraphicsCache cache, DocumentLayoutUnitConverter unitConverter)
			: base(cache) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			GetCharacterWidthsBuffer(initialBufferItemCount);
			GetGlyphsBuffer(initialBufferItemCount);
		}
		protected override void SetClipBounds(RectangleF bounds) {
			base.SetClipBounds(bounds);
			APISetClip(Rectangle.Round(bounds));
		}
		[System.Security.SecuritySafeCritical]
		void APISetClip(Rectangle bounds) {
			IntPtr hdc = Graphics.GetHdc();
			try {
				bounds = SetClipToHDC(bounds, hdc);
			}
			finally {
				Graphics.ReleaseHdc(hdc);
			}
		}
		[System.Security.SecuritySafeCritical]
		internal static Rectangle SetClipToHDC(Rectangle clipBounds, IntPtr hdc) {
			Win32.POINT[] points = new Win32.POINT[2];
			points[0] = new Win32.POINT(clipBounds.Left, clipBounds.Top);
			points[1] = new Win32.POINT(clipBounds.Right, clipBounds.Bottom);
			LPtoDP(hdc, points, points.Length); 
			IntPtr hRgn = CreateRectRgn(points[0].X, points[0].Y, points[1].X, points[1].Y);
			try {
				ExtSelectClipRgn(hdc, hRgn, RGN_COPY);
			}
			finally {
				Win32.DeleteObject(hRgn);
			}
			return clipBounds;
		}
		[System.Security.SecuritySafeCritical]
		internal static Rectangle ExcludeClipFromHDC(Rectangle bounds, IntPtr hdc) {
			Win32.POINT[] points = new Win32.POINT[2];
			points[0] = new Win32.POINT(bounds.Left, bounds.Top);
			points[1] = new Win32.POINT(bounds.Right, bounds.Bottom);
			LPtoDP(hdc, points, points.Length); 
			ExcludeClipRect(hdc, points[0].X, points[0].Y, points[1].X, points[1].Y);
			return bounds;
		}
		[System.Security.SecuritySafeCritical]
		void APIExcludeClip(Rectangle bounds) {
			if (bounds.Width < 1 || bounds.Height < 1)
				return;
			IntPtr hdc = Graphics.GetHdc();
			try {
				bounds = ExcludeClipFromHDC(bounds, hdc);
			}
			finally {
				Graphics.ReleaseHdc(hdc);
			}
		}
		[DllImport("gdi32.dll")]
		static extern bool LPtoDP(IntPtr hdc, [In, Out] Win32.POINT[] lpPoints, int nCount);
		[DllImport("GDI32.dll")]
		static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
		[DllImport("GDI32.dll")]
		static extern int ExtSelectClipRgn(IntPtr hdc, IntPtr hrgn, int mode);
		[DllImport("GDI32.dll")]
		static extern int ExcludeClipRect(IntPtr hdc, int left, int top, int right, int bottom);
		const int RGN_AND = 1, RGN_OR = 2, RGN_XOR = 3, RGN_DIFF = 4, RGN_COPY = 5;
		#region IDisposable implementation
		[System.Security.SecuritySafeCritical]
		protected override void Dispose(bool disposing) {
			try {
				if (glyphsBuffer != IntPtr.Zero) {
					Marshal.FreeCoTaskMem(glyphsBuffer);
					glyphsBuffer = IntPtr.Zero;
				}
				if (characterWidthsBuffer != IntPtr.Zero) {
					Marshal.FreeCoTaskMem(characterWidthsBuffer);
					characterWidthsBuffer = IntPtr.Zero;
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
		~GdiPainter() {
			Dispose(false);
		}
		#endregion
		public override void ExcludeCellBounds(Rectangle rect, Rectangle rowBounds) {
			base.ExcludeCellBounds(rect, rowBounds);
			APIExcludeClip(rect);
		}
		[System.Security.SecuritySafeCritical]
		protected internal IntPtr GetCharacterWidthsBuffer(int itemsCount) {
			int size = sizeof(int) * itemsCount;
			if (size > characterWidthsBufferSize) {
				this.characterWidthsBuffer = Marshal.ReAllocCoTaskMem(characterWidthsBuffer, size);
				this.characterWidthsBufferSize = size;
			}
			return characterWidthsBuffer;
		}
		[System.Security.SecuritySafeCritical]
		protected internal IntPtr GetGlyphsBuffer(int itemsCount) {
			int size = sizeof(short) * itemsCount;
			if (size > glyphsBufferSize) {
				this.glyphsBuffer = Marshal.ReAllocCoTaskMem(glyphsBuffer, size);
				this.glyphsBufferSize = size;
			}
			return glyphsBuffer;
		}
		public override void DrawSpacesString(string text, FontInfo fontInfo, Rectangle bounds) {
			if (HasTransform)
				base.DrawSpacesString(text, fontInfo, bounds);
			else
				DrawStringImpl(text, fontInfo, TextForeColor, bounds, StringFormat, SpacesPlaceholdersDrawStringImplementation);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle bounds) {
			DrawString(text, fontInfo, bounds, StringFormat);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle bounds, StringFormat stringFormat) {
			if (HasTransform)
				base.DrawString(text, fontInfo, bounds, stringFormat);
			else
				DrawStringImpl(text, fontInfo, TextForeColor, bounds, stringFormat, DefaultDrawStringImplementation);
		}
		protected internal virtual void DefaultDrawStringImplementation(IntPtr hdc, string text, FontInfo fontInfo, Rectangle bounds, StringFormat stringFormat) {
			DrawNonCachedString(hdc, fontInfo, text, bounds, stringFormat);
		}
		[System.Security.SecuritySafeCritical]
		protected internal void SpacesPlaceholdersDrawStringImplementation(IntPtr hdc, string text, FontInfo fontInfo, Rectangle bounds, StringFormat stringFormat) {
			int count = text.Length;
			if (count <= 0)
				return;
			GdiStringViewInfo stringViewInfo = GenerateStringViewInfo(hdc, text);
			Win32.RECT clipRect = Win32.RECT.FromRectangle(bounds);
			int spaceWidth = bounds.Width / count;
			int remainder = bounds.Width - spaceWidth * count;
			for (int i = 0; i < count; i++) {
				int width = spaceWidth + ((remainder > 0) ? 1 : 0);
				Marshal.WriteInt32(stringViewInfo.CharacterWidths, i * 4, width);
			}
			Win32.ExtTextOut(
				hdc,
				bounds.X,
				bounds.Y,
				Win32.EtoFlags.ETO_GLYPH_INDEX,
				ref clipRect,
				stringViewInfo.Glyphs,
				stringViewInfo.GlyphCount,
				stringViewInfo.CharacterWidths);
		}
		protected internal delegate void DrawStringActualImplementationDelegate(IntPtr hdc, string text, FontInfo fontInfo, Rectangle bounds, StringFormat stringFormat);
		protected internal void DrawStringImpl(string text, FontInfo fontInfo, Color foreColor, Rectangle bounds, StringFormat stringFormat, DrawStringActualImplementationDelegate impl) {
			bounds = CorrectTextDrawingBounds(fontInfo, bounds);
			bounds.X = unitConverter.SnapToPixels(bounds.X, Graphics.DpiX);
			IntPtr hdc = Graphics.GetHdc();
			try {
				Win32.RECT boundsRect = Win32.RECT.FromRectangle(bounds);
				if (!Win32.RectVisible(hdc, ref boundsRect))
					return;
				try {
					GdiPlusFontInfo gdiFontInfo = (GdiPlusFontInfo)fontInfo;
					Win32.SelectObject(hdc, gdiFontInfo.GdiFontHandle); 
					try {
						try {
							Win32.SetTextColor(hdc, foreColor);
							try {
								Win32.SetBkMode(hdc, Win32.BkMode.TRANSPARENT); 
								try {
									impl(hdc, text, fontInfo, bounds, stringFormat);
								}
								finally {
								}
							}
							finally {
							}
						}
						finally {
						}
					}
					finally {
					}
				}
				finally {
				}
			}
			finally {
				Graphics.ReleaseHdc(hdc);
			}
		}
		protected virtual IntPtr GetMeasureHdc(IntPtr hdc) {
			return hdc;
		}
		protected virtual void ReleaseMeasureHdc(IntPtr measureHdc) {
		}
		protected internal virtual void DrawNonCachedString(IntPtr hdc, FontInfo fontInfo, string text, Rectangle bounds, StringFormat stringFormat) {
			IntPtr measureHdc = GetMeasureHdc(hdc);
			try {
				GdiPlusFontInfo gdiFontInfo = (GdiPlusFontInfo)fontInfo;
				Win32.SelectObject(measureHdc, gdiFontInfo.GdiFontHandle); 
				try {
					DrawNonCachedStringCore(hdc, measureHdc, text, bounds, stringFormat);
				}
				finally {
				}
			}
			finally {
				ReleaseMeasureHdc(measureHdc);
			}
		}
		protected internal virtual void DrawNonCachedStringCore(IntPtr hdc, IntPtr measureHdc, string text, Rectangle bounds, StringFormat stringFormat) {
			if (stringFormat.Alignment == StringAlignment.Near && stringFormat.LineAlignment == StringAlignment.Near)
				DrawNonCachedStringCoreAlignTopLeft(hdc, measureHdc, text, bounds);
			else {
				int prevValue = Win32.SetTextAlign(hdc, (int)(PInvokeSafeNativeMethods.TextAlignment.TA_LEFT | PInvokeSafeNativeMethods.TextAlignment.TA_TOP));
				try {
					DrawNonCachedStringCoreAligned(hdc, measureHdc, text, bounds, stringFormat);
				}
				finally {
					Win32.SetTextAlign(hdc, prevValue);
				}
			}
		}
		protected internal virtual void DrawNonCachedStringCoreAlignTopLeft(IntPtr hdc, IntPtr measureHdc, string text, Rectangle bounds) {
			Win32.RECT clipRect = Win32.RECT.FromRectangle(bounds);
			Win32.ExtTextOut(
				hdc,
				bounds.X,
				bounds.Y,
				Win32.EtoFlags.ETO_NONE,
				ref clipRect,
				text,
				text.Length,
				null);
		}
		[System.Security.SecuritySafeCritical()]
		protected internal virtual void DrawNonCachedStringCoreAligned(IntPtr hdc, IntPtr measureHdc, string text, Rectangle bounds, StringFormat stringFormat) {
			Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
			gcpResults.lStructSize = Marshal.SizeOf(typeof(Win32.GCP_RESULTS));
			gcpResults.lpDx = GetCharacterWidthsBuffer(text.Length);
			gcpResults.lpGlyphs = GetGlyphsBuffer(text.Length);
			gcpResults.nGlyphs = text.Length;
			if (text.Length > 0)
				Marshal.WriteInt16(gcpResults.lpGlyphs, 0);
			int sz = Win32.GetCharacterPlacement(
				hdc,
				text,
				text.Length,
				Int32.MaxValue,
				ref gcpResults,
				Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
			if (sz == 0 && text.Length > 0) {
				gcpResults.lpDx = IntPtr.Zero;
				gcpResults.lpGlyphs = IntPtr.Zero;
				sz = MeasureWithGetCharacterPlacementSlow(hdc, text, ref gcpResults);
			}
			SetTextAlign(hdc, stringFormat);
			Point position = GetAlignedTextPosition(bounds, new Size(sz & 0x0000FFFF, (int)((sz & 0xFFFF0000) >> 16)), stringFormat);
			Win32.RECT clipRect = Win32.RECT.FromRectangle(bounds);
			Win32.ExtTextOut(
				hdc,
				position.X,
				position.Y,
				Win32.EtoFlags.ETO_NONE,
				ref clipRect,
				text,
				text.Length,
				null);
		}
		void SetTextAlign(IntPtr hdc, StringFormat stringFormat) {
			Win32.SetTextAlign(hdc, (int)(CalculateHorizontalTextAlign(stringFormat) | CalculateVerticalTextAlign(stringFormat)));
		}
		PInvokeSafeNativeMethods.TextAlignment CalculateHorizontalTextAlign(System.Drawing.StringFormat stringFormat) {
			switch (stringFormat.Alignment) {
				default:
				case StringAlignment.Near:
					return PInvokeSafeNativeMethods.TextAlignment.TA_LEFT;
				case StringAlignment.Far:
					return PInvokeSafeNativeMethods.TextAlignment.TA_RIGHT;
				case StringAlignment.Center:
					return PInvokeSafeNativeMethods.TextAlignment.TA_CENTER;
			}
		}
		PInvokeSafeNativeMethods.TextAlignment CalculateVerticalTextAlign(System.Drawing.StringFormat stringFormat) {
			return PInvokeSafeNativeMethods.TextAlignment.TA_TOP;
		}
		Point GetAlignedTextPosition(Rectangle bounds, Size textSize, StringFormat stringFormat) {
			return new Point(GetAlignedTextLeft(bounds, textSize.Width, stringFormat), GetAlignedTextTop(bounds, textSize.Height, stringFormat));
		}
		int GetAlignedTextLeft(Rectangle bounds, int textWidth, StringFormat stringFormat) {
			switch (stringFormat.Alignment) {
				default:
				case StringAlignment.Near:
					return bounds.Left;
				case StringAlignment.Far:
					return bounds.Right;
				case StringAlignment.Center:
					return (bounds.Left + bounds.Right) / 2;
			}
		}
		int GetAlignedTextTop(Rectangle bounds, int textHeight, StringFormat stringFormat) {
			switch (stringFormat.LineAlignment) {
				default:
				case StringAlignment.Near:
					return bounds.Top;
				case StringAlignment.Far:
					return bounds.Top + bounds.Height - textHeight;
				case StringAlignment.Center:
					return bounds.Top + (bounds.Height - textHeight) / 2;
			}
		}
		[System.Security.SecuritySafeCritical]
		protected internal GdiStringViewInfo GenerateStringViewInfo(IntPtr hdc, string text) {
			Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
			gcpResults.lStructSize = Marshal.SizeOf(typeof(Win32.GCP_RESULTS));
			gcpResults.lpOutString = IntPtr.Zero;
			gcpResults.lpOrder = IntPtr.Zero;
			gcpResults.lpDx = GetCharacterWidthsBuffer(text.Length);
			gcpResults.lpCaretPos = IntPtr.Zero;
			gcpResults.lpClass = IntPtr.Zero;
			gcpResults.lpGlyphs = GetGlyphsBuffer(text.Length);
			gcpResults.nGlyphs = text.Length;
			if (text.Length > 0)
				Marshal.WriteInt16(gcpResults.lpGlyphs, 0);
			int sz = Win32.GetCharacterPlacement(
				hdc,
				text,
				text.Length,
				0,
				ref gcpResults,
				Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
			if (sz == 0 && text.Length > 0) {
				gcpResults.lpDx = IntPtr.Zero;
				gcpResults.lpGlyphs = IntPtr.Zero;
				sz = MeasureWithGetCharacterPlacementSlow(hdc, text, ref gcpResults);
			}
			GdiStringViewInfo result = new GdiStringViewInfo();
			result.Glyphs = gcpResults.lpGlyphs;
			result.GlyphCount = gcpResults.nGlyphs;
			result.CharacterWidths = gcpResults.lpDx;
			return result;
		}
		[System.Security.SecuritySafeCritical]
		int MeasureWithGetCharacterPlacementSlow(IntPtr hdc, string text, ref Win32.GCP_RESULTS gcpResults) {
			int step = Math.Max(1, (int)Math.Ceiling(text.Length / 2.0)); 
			int add = step;
			for (int i = 0; i < 3; i++, add += step) {
				gcpResults.lpDx = Marshal.ReAllocCoTaskMem(gcpResults.lpDx, sizeof(int) * (text.Length + add)); 
				gcpResults.lpGlyphs = Marshal.ReAllocCoTaskMem(gcpResults.lpGlyphs, sizeof(short) * (text.Length + add));
				gcpResults.nGlyphs = text.Length + add;
				if (text.Length > 0)
					Marshal.WriteInt16(gcpResults.lpGlyphs, 0);
				int sz = Win32.GetCharacterPlacement(
					hdc,
					text,
					text.Length,
					0,
					ref gcpResults,
					Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
				if (sz != 0) {
					if ((sz & 0x0000FFFF) == 0)
						sz |= (int)((sz & 0xFFFF0000) >> 16) * text.Length;
					return sz;
				}
			}
			return 0;
		}
	}
	#endregion
	#region GdiStringViewInfo
	public class GdiStringViewInfo {
		#region Fields
		IntPtr glyphs;
		IntPtr characterWidths;
		int glyphCount;
		#endregion
		#region Properties
		public IntPtr Glyphs { get { return glyphs; } set { glyphs = value; } }
		public IntPtr CharacterWidths { get { return characterWidths; } set { characterWidths = value; } }
		public int GlyphCount { get { return glyphCount; } set { glyphCount = value; } }
		#endregion
	}
	#endregion
}
