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
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Office.Drawing;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
#if !DXPORTABLE
using DevExpress.Pdf;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region GdiBoxMeasurer
	public class GdiBoxMeasurer : GdiPlusBoxMeasurer {
		const int initialBufferItemCount = 64;
		int caretPosBufferSize;
		IntPtr caretPosBuffer;
		public GdiBoxMeasurer(DocumentModel documentModel, Graphics gr)
			: base(documentModel, gr) {
			GetCaretPosBuffer(initialBufferItemCount);
		}
		[System.Security.SecuritySafeCritical]
		protected override void Dispose(bool disposing) {
			try {
				if (caretPosBuffer != IntPtr.Zero) {
					Marshal.FreeCoTaskMem(caretPosBuffer);
					caretPosBuffer = IntPtr.Zero;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		[System.Security.SecuritySafeCritical]
		protected internal IntPtr GetCaretPosBuffer(int itemsCount) {
			int size = sizeof(int) * itemsCount;
			if (size > caretPosBufferSize) {
				this.caretPosBuffer = Marshal.ReAllocCoTaskMem(caretPosBuffer, size);
				this.caretPosBufferSize = size;
			}
			return caretPosBuffer;
		}
		protected internal override TextViewInfo CreateTextViewInfo(BoxInfo boxInfo, string text, FontInfo fontInfo) {
			IntPtr hdc = GetHdc();
			try {
				GdiPlusFontInfo gdiFontInfo = (GdiPlusFontInfo)fontInfo;
				Win32.SelectObject(hdc, gdiFontInfo.GdiFontHandle); 
				try {
					return CreateTextViewInfoCore(hdc, text, fontInfo.FontFamilyName);
				}
				finally {
				}
			}
			finally {
				ReleaseHdc(hdc);
			}
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth) {
			TextRunBase run = PieceTable.Runs[boxInfo.StartPos.RunIndex];
			return run.TryAdjustEndPositionToFit(boxInfo, maxWidth, this);			
		}
		[System.Security.SecuritySafeCritical]
		public override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, string text, FontInfo fontInfo, int maxWidth) {
			IntPtr hdc = GetHdc();
			try {
				GdiPlusFontInfo gdiFontInfo = (GdiPlusFontInfo)fontInfo;
				Win32.SelectObject(hdc, gdiFontInfo.GdiFontHandle); 
				try {
					Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
					gcpResults.lStructSize = GCP_RESULTS_STRUCT_SIZE;
					gcpResults.nGlyphs = text.Length * 4;
					int sz = Win32.GetCharacterPlacement(
						hdc,
						text,
						text.Length,
						maxWidth,
						ref gcpResults,
						Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE | Win32.GcpFlags.GCP_MAXEXTENT);
					if (gcpResults.nMaxFit > 0) {
						int newEndOffset = boxInfo.StartPos.Offset + gcpResults.nMaxFit - 1;
						if (newEndOffset > boxInfo.EndPos.Offset)
							return false;
						boxInfo.EndPos = new FormatterPosition(boxInfo.EndPos.RunIndex, newEndOffset, boxInfo.EndPos.BoxIndex);
						return true;
					}
					else
						return false;
				}
				finally {
				}
			}
			finally {
				ReleaseHdc(hdc);
			}
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual IntPtr GetHdc() {
			return Graphics.GetHdc();
		}
		protected internal virtual void ReleaseHdc(IntPtr hdc) {
			Graphics.ReleaseHdc(hdc);
		}		
		static int GCP_RESULTS_STRUCT_SIZE = Marshal.SizeOf(typeof(Win32.GCP_RESULTS));
		[System.Security.SecuritySafeCritical]
		protected internal virtual TextViewInfo CreateTextViewInfoCore(IntPtr hdc, string text, string fontFamilyName) {
			Win32.GCP_RESULTS gcpResults = new Win32.GCP_RESULTS();
			gcpResults.lStructSize = GCP_RESULTS_STRUCT_SIZE;
			gcpResults.lpOutString = IntPtr.Zero;
			gcpResults.lpOrder = IntPtr.Zero;
			gcpResults.lpDx = Marshal.AllocCoTaskMem(sizeof(int) * text.Length); 
			gcpResults.lpCaretPos = GetCaretPosBuffer(text.Length);
			gcpResults.lpClass = IntPtr.Zero;
			gcpResults.lpGlyphs = Marshal.AllocCoTaskMem(sizeof(short) * text.Length);
			gcpResults.nGlyphs = text.Length;
			if (text.Length > 0)
				Marshal.WriteInt16(gcpResults.lpGlyphs, 0);
			int sz = Win32.GetCharacterPlacement(
				hdc,
				text,
				text.Length,
				0,
				ref gcpResults,
				GetGcpFlags(fontFamilyName));
			if (sz == 0 && text.Length > 0)
				sz = MeasureWithGetCharacterPlacementSlow(hdc, text, ref gcpResults);
			int width = sz & 0x0000FFFF;
			int height = (int)(((uint)sz & 0xFFFF0000) >> 16);
			if (text.Length > 0) {
				int caretBasedWidth = Marshal.ReadInt32(gcpResults.lpCaretPos, (text.Length - 1) * sizeof(int));
				if (caretBasedWidth > 0xFFFF) {
					width = caretBasedWidth + Marshal.ReadInt32(gcpResults.lpDx, (text.Length - 1) * sizeof(int));
				}
			}
			GdiTextViewInfo result = new GdiTextViewInfo();
			result.Size = new Size(width, height);
			result.Glyphs = gcpResults.lpGlyphs;
			result.GlyphCount = gcpResults.nGlyphs;
			result.CharacterWidths = gcpResults.lpDx;
			result.Order = gcpResults.lpOrder;
			return result;
		}
		protected virtual Win32.GcpFlags GetGcpFlags(string fontFamilyName) {
			return Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE;
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
		[System.Security.SecuritySafeCritical]
		public override Rectangle[] MeasureCharactersBounds(string text, FontInfo fontInfo, Rectangle bounds) {
			lock (Graphics) {
				IntPtr hdc = Graphics.GetHdc();
				try {
					GdiPlusFontInfo gdiFontInfo = (GdiPlusFontInfo)fontInfo;
					IntPtr oldFont = Win32.SelectObject(hdc, gdiFontInfo.GdiFontHandle);
					try {
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
						Rectangle[] result = CalculateCharactersBounds(gcpResults.lpCaretPos, text.Length, bounds);
						Marshal.FreeCoTaskMem(gcpResults.lpCaretPos);
						return result;
					}
					finally {
						Win32.SelectObject(hdc, oldFont);
					}
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
		[System.Security.SecuritySafeCritical]
		protected internal Rectangle[] CalculateCharactersBounds(IntPtr lpCaretPos, int count, Rectangle runBounds) {
			bool needProcessLigatures = false;
			Rectangle[] result = new Rectangle[count];
			int prevPos = Marshal.ReadInt32(lpCaretPos, 0);
			for (int i = 0; i < count - 1; i++) {
				int nextPos = Marshal.ReadInt32(lpCaretPos, (i + 1) * sizeof(int));
				result[i] = new Rectangle(runBounds.X + prevPos, runBounds.Y, nextPos - prevPos, runBounds.Height);
				needProcessLigatures |= (prevPos == nextPos);
				prevPos = nextPos;
			}
			if (count > 0)
				result[count - 1] = new Rectangle(runBounds.X + prevPos, runBounds.Y, runBounds.Width - prevPos, runBounds.Height);
			if (needProcessLigatures)
				EstimateCaretPositionsForLigatures(result);
			return result;
		}
		protected internal void EstimateCaretPositionsForLigatures(Rectangle[] characterBounds) {
			int count = characterBounds.Length;
			int from = Int32.MaxValue;
			for (int i = 0; i < count; i++) {
				Rectangle bounds = characterBounds[i];
				if (bounds.Width == 0) {
					if (from == Int32.MaxValue)
						from = i;
				}
				else {
					if (from < i) {
						AdjustCharacterBoundsForLigature(characterBounds, from, i);
						from = Int32.MaxValue;
					}
				}
			}
		}
		protected internal void AdjustCharacterBoundsForLigature(Rectangle[] characterBounds, int from, int to) {
			int count = to - from + 1;
			Rectangle[] bounds = RectangleUtils.SplitHorizontally(characterBounds[to], count);
			for (int i = 0; i < count; i++)
				characterBounds[i + from] = bounds[i];
		}
	}
	#endregion
	#region GdiBoxMeasurerLockHdc
	public class GdiBoxMeasurerLockHdc : GdiBoxMeasurer {
		readonly Graphics hdcGraphics;
		IntPtr hdc;
		public GdiBoxMeasurerLockHdc(DocumentModel documentModel, Graphics gr, Graphics hdcGraphics)
			: base(documentModel, gr) {
			Guard.ArgumentNotNull(hdcGraphics, "hdcGraphics");
			this.hdcGraphics = hdcGraphics;
			ObtainCachedHdc();
		}
		[System.Security.SecuritySafeCritical]
		public void ObtainCachedHdc() {
			this.hdc = hdcGraphics.GetHdc();
		}
		[System.Security.SecuritySafeCritical]
		public void ReleaseCachedHdc() {
			if (hdc != IntPtr.Zero) {
				hdcGraphics.ReleaseHdc();
				hdc = IntPtr.Zero;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					ReleaseCachedHdc();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override IntPtr GetHdc() {
			if (hdc != IntPtr.Zero)
				return hdc;
			else
				return base.GetHdc();
		}
		protected internal override void ReleaseHdc(IntPtr dc) {
			if (this.hdc == IntPtr.Zero)
				base.ReleaseHdc(dc);
		}
	}
	#endregion
#if !DXPORTABLE
	public class PdfBoxMeasurer : GdiBoxMeasurerLockHdc {
		PdfCreationOptions creationOptions;
		public PdfBoxMeasurer(DocumentModel documentModel, Graphics gr, Graphics hdcGraphics, PdfCreationOptions creationOptions) 
			: base(documentModel, gr, hdcGraphics) {
				this.creationOptions = creationOptions;
		}
		public bool DisableEmbeddingAllFonts { get; set; }
		public IList<string> NotEmbeddedFontFamilies { get; set; }
		protected override Win32.GcpFlags GetGcpFlags(string fontFamilyName) {
			Win32.GcpFlags flags = Win32.GcpFlags.GCP_USEKERNING;
			if (this.creationOptions.EmbedFont(fontFamilyName))
				flags |= Win32.GcpFlags.GCP_LIGATE;
			return flags;
		}
	}
#endif
	#region GdiTextViewInfo
	public class GdiTextViewInfo : TextViewInfo {
		#region Fields
		IntPtr glyphs;
		int glyphCount;
		IntPtr characterWidths;
		IntPtr order;
		#endregion
		#region Properties
		public IntPtr Glyphs { get { return glyphs; } set { glyphs = value; } }
		public int GlyphCount { get { return glyphCount; } set { glyphCount = value; } }
		public IntPtr CharacterWidths { get { return characterWidths; } set { characterWidths = value; } }
		public IntPtr Order { get { return order; } set { order = value; } }
		#endregion
		#region IDisposable implementation
		[System.Security.SecuritySafeCritical]
		protected override void Dispose(bool disposing) {
			try {
				if (glyphs != IntPtr.Zero) {
					Marshal.FreeCoTaskMem(glyphs);
					glyphs = IntPtr.Zero;
				}
				if (characterWidths != IntPtr.Zero) {
					Marshal.FreeCoTaskMem(characterWidths);
					characterWidths = IntPtr.Zero;
				}
				if (order != IntPtr.Zero) {
					Marshal.FreeCoTaskMem(order);
					order = IntPtr.Zero;
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
		~GdiTextViewInfo() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	#region GdiPlusBoxMeasurer
	public class GdiPlusBoxMeasurer : BoxMeasurer {
		StringFormat stringFormat;
		readonly Graphics gr;
		public GdiPlusBoxMeasurer(DocumentModel documentModel, Graphics gr)
			: base(documentModel) {
			Guard.ArgumentNotNull(gr, "gr");
			this.gr = gr;
			this.stringFormat = CreateStringFormat();
		}
		internal Graphics Graphics { get { return gr; } }
		internal StringFormat StringFormat { get { return stringFormat; } }
		static StringFormat CreateStringFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
			return result;
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (stringFormat != null) {
						stringFormat.Dispose();
						stringFormat = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override TextViewInfo CreateTextViewInfo(BoxInfo boxInfo, string text, FontInfo fontInfo) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			SizeF szf = gr.MeasureString(text, gdiPlusFontInfo.Font, 0, stringFormat);
			Size sz = Size.Empty;
			sz.Width = (int)Math.Ceiling(szf.Width);
			sz.Height = fontInfo.LineSpacing;
			TextViewInfo result = new TextViewInfo();
			result.Size = sz;
			return result;
		}
		protected internal List<CharacterRange[]> CreateCorrectCharacterRanges(string text) {
			const int maxArraySize = 32;
			List<CharacterRange[]> result = new List<CharacterRange[]>();
			List<CharacterRange> currentRange = new List<CharacterRange>(maxArraySize);
			int count = text.Length;
			for (int i = 0; i < count; i++) {
				currentRange.Add(new CharacterRange(i, 1));
				if ((i % maxArraySize) == (maxArraySize - 1)) {
					result.Add(currentRange.ToArray());
					currentRange = new List<CharacterRange>(maxArraySize);
				}
			}
			if (currentRange.Count > 0)
				result.Add(currentRange.ToArray());
			return result;
		}
		protected internal virtual List<Region> MeasureCharactersRegions(string text, FontInfo fontInfo, Rectangle bounds) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			using (StringFormat sf = (StringFormat)stringFormat.Clone()) {
				Font font = gdiPlusFontInfo.Font;
				List<CharacterRange[]> rangeList = CreateCorrectCharacterRanges(text);
				List<Region> result = new List<Region>();
				int count = rangeList.Count;
				for (int i = 0; i < count; i++) {
					sf.SetMeasurableCharacterRanges(rangeList[i]);
					result.AddRange(gr.MeasureCharacterRanges(text, font, bounds, sf));
				}
				return result;
			}
		}
		protected internal virtual Rectangle[] MeasureCharactersBoundsCore(string text, FontInfo fontInfo, Rectangle bounds) {
			List<Region> regions = MeasureCharactersRegions(text, fontInfo, bounds);
			int count = regions.Count;
			Rectangle[] result = new Rectangle[count];
			for (int i = 0; i < count; i++)
				result[i] = Rectangle.Round(regions[i].GetBounds(gr));
			return result;
		}
		public override Rectangle[] MeasureCharactersBounds(string text, FontInfo fontInfo, Rectangle bounds) {
			Matrix oldTransform = gr.Transform.Clone();
			try {
				const float scale = 100;
				gr.ScaleTransform(scale, scale);
				return MeasureCharactersBoundsCore(text, fontInfo, bounds);
			}
			finally {
				gr.Transform = oldTransform.Clone();
			}
		}
		public int SnapToPixels(int value, float dpi) {
			return DocumentModel.LayoutUnitConverter.SnapToPixels(value, dpi);
		}
	}
	#endregion
}
