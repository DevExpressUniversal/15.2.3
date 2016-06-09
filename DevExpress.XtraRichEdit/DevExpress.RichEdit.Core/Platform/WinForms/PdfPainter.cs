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

#if !DXPORTABLE
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security;
using System.Runtime.InteropServices;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Export {
	public class PdfPainter : PdfPainterBase {
		static float metafileDpiX = -1;
		static float metafileDpiY = -1;
		static float MetafileDpiX {
			get {
				if (metafileDpiX == -1) {
					lock (typeof(PdfPainter)) {
						if (metafileDpiX != -1)
							return metafileDpiX;
						CalcMetafileDpi();
					}
				}
				return metafileDpiX;
			}
		}
		static float MetafileDpiY {
			get {
				if (metafileDpiY == -1) {
					lock (typeof(PdfPainter)) {
						if (metafileDpiY != -1)
							return metafileDpiY;
						CalcMetafileDpi();
					}
				}
				return metafileDpiY;
			}
		}
		static void CalcMetafileDpi() {
			using (MemoryStream stream = new MemoryStream()) {
				Metafile img = MetafileCreator.CreateInstance(stream, 0, 0, MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
				MetafileHeader header = img.GetMetafileHeader();
				metafileDpiX = header.DpiX;
				metafileDpiY = header.DpiY;
			}
		}
		public PdfPainter(DocumentLayout layout) : base(layout) { }
		[SecuritySafeCritical]
		public override void DrawBrick(PrintingSystemBase ps, VisualBrick brick, Rectangle bounds) {
			using (MemoryStream stream = new MemoryStream()) {
				int widthInPixels = LayoutUnitConverter.LayoutUnitsToPixels(bounds.Width, MetafileDpiX);
				int heightInPixels = LayoutUnitConverter.LayoutUnitsToPixels(bounds.Height, MetafileDpiY);
				Metafile img = MetafileCreator.CreateInstance(stream, widthInPixels, heightInPixels, MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
				using (GdiGraphics gdiGraphics = new ImageGraphics(img, ps)) {
					gdiGraphics.ScaleTransform(MetafileDpiX / gdiGraphics.Dpi, MetafileDpiY / gdiGraphics.Dpi);
					Rectangle shiftedBounds = new Rectangle(0, 0, bounds.Width, bounds.Height);
					VisualBrickExporter exporter = (VisualBrickExporter)ExportersFactory.CreateExporter(brick);
					exporter.Draw(gdiGraphics, shiftedBounds, shiftedBounds);
				}
				PdfGraphics.DrawImage(stream, bounds);
			}
		}
		protected override void DrawStringCore(string text, FontInfo fontInfo, TextViewInfo textInfo, float x, float y) {
			SolidBrush textForeBrush = (SolidBrush)Cache.GetSolidBrush(TextForeColor);
			GdiTextViewInfo gdiTextInfo = textInfo as GdiTextViewInfo;
			FontStyle style = fontInfo.CalculateFontStyle();
			if (gdiTextInfo != null && gdiTextInfo.CharacterWidths != IntPtr.Zero) 
				DrawStringWithGlyphPositioning(text, gdiTextInfo, fontInfo.FontFamilyName, style, fontInfo.SizeInPoints, textForeBrush, x, y);
			else
				PdfGraphics.DrawString(text, fontInfo.Font, textForeBrush, x, y);
		}
		[SecuritySafeCritical]
		void DrawStringWithGlyphPositioning(string text, GdiTextViewInfo gdiTextInfo, string fontFamily, FontStyle fontStyle, float fontSize, SolidBrush textForeBrush, float x, float y) {
			int count = gdiTextInfo.GlyphCount;
			ushort[] glyphIndicies = new ushort[count];
			for (int i = 0; i < count; i++)
				glyphIndicies[i] = (ushort)Marshal.ReadInt16(gdiTextInfo.Glyphs, i * 2);
			float[] glyphDistances = new float[count];
			for (int i = 0; i < count; i++)
				glyphDistances[i] = Marshal.ReadInt32(gdiTextInfo.CharacterWidths, i * 4);
			int[] order = new int[count];
			if (gdiTextInfo.Order != IntPtr.Zero)
				for (int i = 0; i < count; i++)
					order[i] = Marshal.ReadInt32(gdiTextInfo.Order, i * 4);
			else
				for (int i = 0; i < count; i++)
					order[i] = i;
			PdfGraphics.DrawString(text, fontFamily, fontStyle, fontSize, textForeBrush, x, y, glyphIndicies, glyphDistances, order);
		}
		protected override bool CanGetImageStream(OfficeImage img) {
			OfficeImageWin imgWin = img.EncapsulatedOfficeNativeImage as OfficeImageWin;
			return (imgWin != null && imgWin.ImageStream != null);
		}
		protected override Stream GetImageStream(OfficeImage img) {
			MemoryStream ms = ((OfficeImageWin)img.EncapsulatedOfficeNativeImage).ImageStream;
			ms.Position = 0;
			return ms;
		}
	}
}
#endif
