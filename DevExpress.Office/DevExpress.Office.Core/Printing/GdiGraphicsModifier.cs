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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Text;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.Printing {
	#region GdiGraphicsModifier
	public class GdiGraphicsModifier : DevExpress.XtraPrinting.Native.GraphicsModifier {
		readonly DocumentLayoutUnitConverter unitConverter;
		HdcDpiToDocuments dpiModifier;
		HdcZoomModifier zoomModifier;
		Graphics measureGraphics;
		HdcDpiToDocuments measureDpiModifier;
		DrawStringExtTextOutDelegate extTextOut;
		Win32.EtoFlags etoFlagsForTextOutput = Win32.EtoFlags.ETO_CLIPPED;
		public GdiGraphicsModifier(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			this.UseGlyphs = true;
		}
		public bool UseGlyphs {
			get { return this.extTextOut == DrawStringExtTextOutGlyph; }
			set {
				if (UseGlyphs == value)
					return;
				if (value)
					this.extTextOut = DrawStringExtTextOutGlyph;
				else
					this.extTextOut = DrawStringExtTextOutText;
			}
		}
		public bool UseClipBoundsWithoutGlyphs {
			get { return etoFlagsForTextOutput == Win32.EtoFlags.ETO_CLIPPED; }
			set {
				if (UseClipBoundsWithoutGlyphs == value)
					return;
				if (value)
					etoFlagsForTextOutput = Win32.EtoFlags.ETO_CLIPPED;
				else
					etoFlagsForTextOutput = Win32.EtoFlags.ETO_NONE;
			}
		}
		public override void Dispose() {
			if (measureGraphics != null) {
				this.measureGraphics.Dispose();
				this.measureGraphics = null;
			}
			CleanupTransforms();
		}
		public override void OnGraphicsDispose() {
			CleanupTransforms();
		}
		protected internal virtual void CleanupTransforms() {
			if(measureDpiModifier != null) {
				measureDpiModifier.Dispose();
				measureDpiModifier = null;
			}
			if (dpiModifier != null) {
				dpiModifier.Dispose();
				dpiModifier = null;
			}
			if(zoomModifier != null) {
				zoomModifier.Dispose();
				zoomModifier = null;
			}
			this.zoomFactor = defaultZoomFactor;
		}
		public override void SetPageUnit(Graphics gr, GraphicsUnit value) {
			System.Diagnostics.Debug.Assert(value == GraphicsUnit.Document);
			System.Diagnostics.Debug.Assert(dpiModifier == null);
			gr.PageUnit = value;
			this.dpiModifier = new HdcDpiToDocuments(gr, new Size(4096, 4096));
			if(this.measureGraphics == null)
				this.measureGraphics = Graphics.FromHwnd(IntPtr.Zero);
			this.measureGraphics.PageUnit = value;
			this.measureDpiModifier = new HdcDpiToDocuments(gr, new Size(4096, 4096));
			float sx = gr.Transform.Elements[0];
			this.zoomModifier = new HdcZoomModifier(gr, sx);
			this.zoomFactor = sx;
		}
		GraphicsState previousGraphicsState;
		GraphicsToLayoutUnitsModifier modifier;
		public void SwitchToLayoutUnits(Graphics gr) {
			this.previousGraphicsState = gr.Save();
			this.modifier = new GraphicsToLayoutUnitsModifier(gr, unitConverter);
			gr.ScaleTransform(this.zoomFactor, this.zoomFactor);
			this.zoomModifier = new HdcZoomModifier(gr, this.zoomFactor);
		}
		public void SwitchToDocuments(Graphics gr) {
			this.zoomModifier.Dispose();
			this.modifier.Dispose();
			gr.Restore(previousGraphicsState);
		}
		delegate void DrawStringExtTextOutDelegate(IntPtr hdc, string text, Rectangle bounds, IntPtr fontHandle, Win32.RECT clipRect, StringFormat format);
		delegate void ExtTextOutAction(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, StringFormat format, int textHeight);
		public override void DrawString(Graphics gr, string text, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			RectangleF clipBounds = gr.ClipBounds;
			IntPtr hdc = gr.GetHdc();
			try {
				SolidBrush solidBrush = brush as SolidBrush;
				if (solidBrush != null)
					Win32.SetTextColor(hdc, solidBrush.Color);
				Win32.SetBkMode(hdc, Win32.BkMode.TRANSPARENT);
				IntPtr fontHandle = GdiPlusFontInfo.CreateGdiFont(font, unitConverter);
				IntPtr oldFont = Win32.SelectObject(hdc, fontHandle);
				GdiPainter.SetClipToHDC(Rectangle.Ceiling(clipBounds), hdc);
				try {
					if((format.FormatFlags & StringFormatFlags.NoWrap) == 0) {
						DrawStringDrawTextEx(hdc, text, Win32.RECT.FromRectangle(Rectangle.Ceiling(bounds)), format);
					} else {
						RectangleF clipBoundsInLayoutUnits = unitConverter.DocumentsToLayoutUnits(bounds);
						clipBoundsInLayoutUnits.Intersect(clipBounds);
						Win32.RECT clipRect = Win32.RECT.FromRectangle(Rectangle.Ceiling(clipBoundsInLayoutUnits));
						this.extTextOut(hdc, text, Rectangle.Ceiling(bounds), fontHandle, clipRect, format);
					}
				}
				finally {
					GdiPainter.ExcludeClipFromHDC(Rectangle.Ceiling(bounds), hdc);
					Win32.SelectObject(hdc, oldFont);
					Win32.DeleteObject(fontHandle);
				}
			}
			finally {
				gr.ReleaseHdc(hdc);
			}
		}
		void DrawStringDrawTextEx(IntPtr hdc, string text, Win32.RECT rect, StringFormat format) {
			Win32.DrawTextFlags flags = CalculateDrawTextFlags(format);
			int yOffset = CalculateDrawTextVerticalOffset(hdc, text, rect, format, flags);
			rect.Top += yOffset;
			Win32.DrawTextEx(hdc, text, ref rect, flags);
		}
		int CalculateDrawTextVerticalOffset(IntPtr hdc, string text, Win32.RECT rect, StringFormat format, Win32.DrawTextFlags flags) {
			if ((format.FormatFlags & StringFormatFlags.NoWrap) != 0)
				return 0;
			if (format.LineAlignment == StringAlignment.Near)
				return 0;
			int rectHeight = rect.Height;
			flags &= ~(Win32.DrawTextFlags.DT_VCENTER | Win32.DrawTextFlags.DT_BOTTOM);
			int height = Win32.DrawTextEx(hdc, text, ref rect, flags | Win32.DrawTextFlags.DT_CALCRECT);
			if (format.LineAlignment == StringAlignment.Center)
				return (rectHeight - height) / 2;
			else
				return rectHeight - height;
		}
		Win32.DrawTextFlags CalculateDrawTextFlags(StringFormat format) {
			Win32.DrawTextFlags result = Win32.DrawTextFlags.DT_NOPREFIX;
			if ((format.FormatFlags & StringFormatFlags.NoWrap) == 0)
				result |= Win32.DrawTextFlags.DT_WORDBREAK;
			else
				result |= Win32.DrawTextFlags.DT_SINGLELINE;
			switch (format.Alignment) {
				case StringAlignment.Near:
					result |= Win32.DrawTextFlags.DT_LEFT;
					break;
				case StringAlignment.Center:
					result |= Win32.DrawTextFlags.DT_CENTER;
					break;
				case StringAlignment.Far:
					result |= Win32.DrawTextFlags.DT_RIGHT;
					break;
			}
			if ((result & Win32.DrawTextFlags.DT_SINGLELINE) != 0) {
				switch (format.LineAlignment) {
					case StringAlignment.Near:
						result |= Win32.DrawTextFlags.DT_TOP;
						break;
					case StringAlignment.Center:
						result |= Win32.DrawTextFlags.DT_VCENTER;
						break;
					case StringAlignment.Far:
						result |= Win32.DrawTextFlags.DT_BOTTOM;
						break;
				}
			}
			return result;
		}
		struct ExtendedGcpResults {
			public Win32.GCP_RESULTS Results { get; set; }
			public int TextHeight { get; set; }
		}
		[System.Security.SecuritySafeCritical]
		ExtendedGcpResults CalculateTextHeightAndGlyphs(string text, IntPtr fontHandle) {
			ExtendedGcpResults result = new ExtendedGcpResults();
			Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
			gcpResults.lStructSize = Marshal.SizeOf(typeof(Win32.GCP_RESULTS));
			gcpResults.lpOutString = IntPtr.Zero;
			gcpResults.lpOrder = IntPtr.Zero;
			gcpResults.lpDx = Marshal.AllocCoTaskMem(sizeof(int) * text.Length);
			gcpResults.lpCaretPos = IntPtr.Zero;
			gcpResults.lpClass = IntPtr.Zero;
			gcpResults.lpGlyphs = Marshal.AllocCoTaskMem(sizeof(short) * text.Length);
			gcpResults.nGlyphs = text.Length;
			if (text.Length > 0)
				Marshal.WriteInt16(gcpResults.lpGlyphs, 0);
			lock (measureGraphics) {
				IntPtr measureHdc = measureGraphics.GetHdc();
				try {
					Win32.SelectObject(measureHdc, fontHandle);
					int sz = Win32.GetCharacterPlacement(
						measureHdc,
						text,
						text.Length,
						0,
						ref gcpResults,
						Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
					if (sz == 0 && text.Length > 0)
						sz = MeasureWithGetCharacterPlacementSlow(measureHdc, text, ref gcpResults);
					result.TextHeight = (sz >> 16);
				}
				finally {
					measureGraphics.ReleaseHdc(measureHdc);
				}
			}
			result.Results = gcpResults;
			return result;
		}
		#region DrawStringExtTextOut (text version)
		[System.Security.SecuritySafeCritical]
		void DrawStringExtTextOutText(IntPtr hdc, string text, Rectangle bounds, IntPtr fontHandle, Win32.RECT clipRect, StringFormat format) {
			ExtendedGcpResults gcpResults = CalculateTextHeightAndGlyphs(text, fontHandle);
			ExtTextOut(hdc, bounds, clipRect, text, format, gcpResults.TextHeight);
			Marshal.FreeCoTaskMem(gcpResults.Results.lpGlyphs);
			Marshal.FreeCoTaskMem(gcpResults.Results.lpDx);
		}
		void ExtTextOut(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, string text, StringFormat format, int textHeight) {
			int yOffset = CalculateYOffset(bounds, textHeight, format.LineAlignment);
			switch (format.Alignment) {
				case StringAlignment.Near:
					ExtTextOutAlignLeft(hdc, bounds, clipRect, text, format, yOffset);
					break;
				case StringAlignment.Center:
					ExtTextOutAlignCenter(hdc, bounds, clipRect, text, format, yOffset);
					break;
				case StringAlignment.Far:
					ExtTextOutAlignRight(hdc, bounds, clipRect, text, format, yOffset);
					break;
			}
		}
		void ExtTextOutAlignLeft(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, string text, StringFormat format, int yOffset) {
			ExtTextOutCore(hdc, bounds.Left, bounds.Top + yOffset, clipRect, text);
		}
		void ExtTextOutAlignCenter(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, string text, StringFormat format, int yOffset) {
			int prevValue = Win32.SetTextAlign(hdc, format);
			try {
				ExtTextOutCore(hdc, (bounds.Left + bounds.Right) / 2, bounds.Top + yOffset, clipRect, text);
			}
			finally {
				Win32.SetTextAlign(hdc, prevValue);
			}
		}
		void ExtTextOutAlignRight(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, string text, StringFormat format, int yOffset) {
			int prevValue = Win32.SetTextAlign(hdc, format);
			try {
				ExtTextOutCore(hdc, bounds.Right, bounds.Top + yOffset, clipRect, text);
			}
			finally {
				Win32.SetTextAlign(hdc, prevValue);
			}
		}
		void ExtTextOutCore(IntPtr hdc, int x, int y, Win32.RECT clipRect, string text) {
			Win32.ExtTextOut(
				hdc,
				x,
				y,
				etoFlagsForTextOutput,
				ref clipRect,
				text,
				text.Length,
				null);
		}
		#endregion
		#region DrawStringExtTextOut (glyph version)
		[System.Security.SecuritySafeCritical]
		void DrawStringExtTextOutGlyph(IntPtr hdc, string text, Rectangle bounds, IntPtr fontHandle, Win32.RECT clipRect, StringFormat format) {
			ExtendedGcpResults gcpResults = CalculateTextHeightAndGlyphs(text, fontHandle);
			ExtTextOut(hdc, bounds, clipRect, gcpResults.Results, format, gcpResults.TextHeight);
			Marshal.FreeCoTaskMem(gcpResults.Results.lpGlyphs);
			Marshal.FreeCoTaskMem(gcpResults.Results.lpDx);
		}
		void ExtTextOut(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, Win32.GCP_RESULTS gcpResults, StringFormat format, int textHeight) {
			int yOffset = CalculateYOffset(bounds, textHeight, format.LineAlignment);
			switch (format.Alignment) {
				case StringAlignment.Near:
					ExtTextOutAlignLeft(hdc, bounds, clipRect, gcpResults, format, yOffset);
					break;
				case StringAlignment.Center:
					ExtTextOutAlignCenter(hdc, bounds, clipRect, gcpResults, format, yOffset);
					break;
				case StringAlignment.Far:
					ExtTextOutAlignRight(hdc, bounds, clipRect, gcpResults, format, yOffset);
					break;
			}
		}
		void ExtTextOutAlignLeft(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, Win32.GCP_RESULTS gcpResults, StringFormat format, int yOffset) {
			ExtTextOutCore(hdc, bounds.Left, bounds.Top + yOffset, clipRect, gcpResults);
		}
		void ExtTextOutAlignCenter(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, Win32.GCP_RESULTS gcpResults, StringFormat format, int yOffset) {
			int prevValue = Win32.SetTextAlign(hdc, format);
			try {
				ExtTextOutCore(hdc, (bounds.Left + bounds.Right) / 2, bounds.Top + yOffset, clipRect, gcpResults);
			}
			finally {
				Win32.SetTextAlign(hdc, prevValue);
			}
		}
		void ExtTextOutAlignRight(IntPtr hdc, Rectangle bounds, Win32.RECT clipRect, Win32.GCP_RESULTS gcpResults, StringFormat format, int yOffset) {
			int prevValue = Win32.SetTextAlign(hdc, format);
			try {
				ExtTextOutCore(hdc, bounds.Right, bounds.Top + yOffset, clipRect, gcpResults);
			}
			finally {
				Win32.SetTextAlign(hdc, prevValue);
			}
		}
		void ExtTextOutCore(IntPtr hdc, int x, int y, Win32.RECT clipRect, Win32.GCP_RESULTS gcpResults) {
			Win32.ExtTextOut(
				hdc,
				x,
				y,
				Win32.EtoFlags.ETO_GLYPH_INDEX,
				ref clipRect,
				gcpResults.lpGlyphs,
				gcpResults.nGlyphs,
				gcpResults.lpDx);
		}
		#endregion
		int CalculateYOffset(Rectangle bounds, int textHeight, StringAlignment alignment) {
			if (textHeight <= 0)
				textHeight = bounds.Height;
			switch (alignment) {
				default:
				case StringAlignment.Near:
					return 0;
				case StringAlignment.Center:
					return (bounds.Height - textHeight) / 2;
				case StringAlignment.Far:
					return (bounds.Height - textHeight);
			}
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
				if (sz != 0)
					return sz;
			}
			return 0;
		}
		public override void ScaleTransform(Graphics gr, float sx, float sy, MatrixOrder order) {
			System.Diagnostics.Debug.Assert(order == MatrixOrder.Prepend);
			System.Diagnostics.Debug.Assert(sx == sy);
			gr.ScaleTransform(sx, sy, order);
			this.zoomModifier = new HdcZoomModifier(gr, sx);
			this.zoomFactor = sx;
		}
		const float defaultZoomFactor = 1.0f;
		float zoomFactor = defaultZoomFactor;
		public override void DrawImage(Graphics gr, Image image, Point position) {
			Size imageSizeInPixels = image.Size;
			RectangleF referenceImageBounds = new Rectangle(position, DevExpress.XtraReports.UI.XRConvert.Convert(imageSizeInPixels, GraphicsUnit.Pixel, gr.PageUnit));
			RectangleF correctedBounds = SnapToDevicePixelsHelper.GetCorrectedBounds(gr, imageSizeInPixels, referenceImageBounds);
			gr.DrawImage(image, correctedBounds.Location);
		}
		public override void DrawImage(Graphics gr, Image image, RectangleF bounds) {
			RectangleF correctedBounds = SnapToDevicePixelsHelper.GetCorrectedBounds(gr, image.Size, bounds);
			gr.DrawImage(image, correctedBounds);
		}
	}
	#endregion
	#region GdiMeasurer
	public class GdiMeasurer : DevExpress.XtraPrinting.Native.Measurer {
		readonly DocumentLayoutUnitConverter unitConverter;
		public GdiMeasurer(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = new DocumentLayoutUnitDocumentConverter();
		}
		public Graphics Graphics { get { return this.Graph; } }
		public override void Dispose() {
		}
		public override RectangleF GetRegionBounds(Region rgn, GraphicsUnit pageUnit) {
			return rgn.GetBounds(Graphics);
		}
		[System.Security.SecuritySafeCritical]
		public override Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat, GraphicsUnit pageUnit) {
			Rectangle bounds = Rectangle.Round(layoutRect);
			lock (Graphics) {
				IntPtr hdc = Graphics.GetHdc();
				try {
					IntPtr fontHandle = GdiPlusFontInfo.CreateGdiFont(font, unitConverter);
					IntPtr oldFont = Win32.SelectObject(hdc, fontHandle);
					Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
					gcpResults.lStructSize = Marshal.SizeOf(typeof(Win32.GCP_RESULTS));
					gcpResults.lpOutString = IntPtr.Zero;
					gcpResults.lpOrder = IntPtr.Zero;
					gcpResults.lpDx = IntPtr.Zero;
					gcpResults.lpCaretPos = Marshal.AllocCoTaskMem(sizeof(int) * text.Length);
					gcpResults.lpClass = IntPtr.Zero;
					gcpResults.lpGlyphs = IntPtr.Zero;
					gcpResults.nGlyphs = text.Length;
					int sz = Win32.GetCharacterPlacement(
						hdc,
						text,
						text.Length,
						0,
						ref gcpResults,
						Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
					if (sz == 0 && text.Length > 0)
						sz = MeasureCharactersWithGetCharacterPlacementSlow(hdc, text, ref gcpResults);
					int count = text.Length;
					Rectangle[] result = new Rectangle[count];
					int prevPos = Marshal.ReadInt32(gcpResults.lpCaretPos, 0);
					for (int i = 0; i < count - 1; i++) {
						int nextPos = Marshal.ReadInt32(gcpResults.lpCaretPos, (i + 1) * sizeof(int));
						result[i] = new Rectangle(bounds.X + prevPos, bounds.Y, nextPos - prevPos, bounds.Height);
						prevPos = nextPos;
					}
					if (count > 0)
						result[count - 1] = new Rectangle(bounds.X + prevPos, bounds.Y, bounds.Width - prevPos, bounds.Height);
					Marshal.FreeCoTaskMem(gcpResults.lpCaretPos);
					Win32.SelectObject(hdc, oldFont);
					Win32.DeleteObject(fontHandle);
					Region[] rgns = new Region[result.Length];
					for (int i = 0; i < result.Length; i++)
						rgns[i] = new Region(result[i]);
					return rgns;
				}
				finally {
					Graphics.ReleaseHdc(hdc);
				}
			}
		}
		[System.Security.SecuritySafeCritical]
		int MeasureCharactersWithGetCharacterPlacementSlow(IntPtr hdc, string text, ref Win32.GCP_RESULTS gcpResults) {
			int step = Math.Max(1, (int)Math.Ceiling(text.Length / 2.0)); 
			int add = step;
			for (int i = 0; i < 3; i++, add += step) {
				gcpResults.lpCaretPos = Marshal.ReAllocCoTaskMem(gcpResults.lpCaretPos, sizeof(int) * (text.Length + add)); 
				gcpResults.nGlyphs = text.Length + add;
				int sz = Win32.GetCharacterPlacement(
					hdc,
					text,
					text.Length,
					0,
					ref gcpResults,
					Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
				if (sz != 0)
					return sz;
			}
			return 0;
		}
		public override SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit) {
			return MeasureString(text, font, Point.Empty, stringFormat, pageUnit);
		}
		[System.Security.SecuritySafeCritical]
		public override SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit pageUnit) {
			lock (Graphics) {
				IntPtr hdc = Graphics.GetHdc();
				try {
					IntPtr fontHandle = GdiPlusFontInfo.CreateGdiFont(font, unitConverter);
					IntPtr oldFont = Win32.SelectObject(hdc, fontHandle);
					Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
					gcpResults.lStructSize = Marshal.SizeOf(typeof(Win32.GCP_RESULTS));
					gcpResults.lpOutString = IntPtr.Zero;
					gcpResults.lpOrder = IntPtr.Zero;
					gcpResults.lpDx = Marshal.AllocCoTaskMem(sizeof(int) * text.Length); 
					gcpResults.lpCaretPos = Marshal.AllocCoTaskMem(sizeof(int) * text.Length);
					gcpResults.lpClass = IntPtr.Zero;
					gcpResults.lpGlyphs = IntPtr.Zero;
					gcpResults.nGlyphs = text.Length;
					int sz = Win32.GetCharacterPlacement(
						hdc,
						text,
						text.Length,
						0,
						ref gcpResults,
						Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
					if (sz == 0 && text.Length > 0)
						sz = MeasureWithGetCharacterPlacementSlow(hdc, text, ref gcpResults);
					int width = sz & 0x0000FFFF;
					int height = (int)(((uint)sz & 0xFFFF0000) >> 16);
					int caretBasedWidth = Marshal.ReadInt32(gcpResults.lpCaretPos, (text.Length - 1) * sizeof(int));
					if (caretBasedWidth > 0xFFFF) {
						width = caretBasedWidth + Marshal.ReadInt32(gcpResults.lpDx, (text.Length - 1) * sizeof(int));
					}
					Marshal.FreeCoTaskMem(gcpResults.lpCaretPos);
					Marshal.FreeCoTaskMem(gcpResults.lpDx);
					Win32.SelectObject(hdc, oldFont);
					Win32.DeleteObject(fontHandle);
					return new SizeF(width, height);
				}
				finally {
					Graphics.ReleaseHdc(hdc);
				}
			}
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
				if (sz != 0)
					return sz;
			}
			return 0;
		}
	}
	#endregion
}
