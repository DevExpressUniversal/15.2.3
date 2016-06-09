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
using System.Security;
using DevExpress.Utils.Text;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Office.PInvoke;
using DevExpress.Data.Helpers;
namespace DevExpress.Office.Drawing {
	#region GdiPlusFontInfoMeasurer
	public class GdiPlusFontInfoMeasurer : FontInfoMeasurer {
		#region Fields
		Font defaultFont;
		Graphics measureGraphics;
		GraphicsToLayoutUnitsModifier graphicsModifier;
		StringFormat measureStringFormat;
		#endregion
		public GdiPlusFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		#region Properties
		public Graphics MeasureGraphics { get { return measureGraphics; } }
		internal GraphicsToLayoutUnitsModifier GraphicsModifier { get { return graphicsModifier; } }
		internal StringFormat MeasureStringFormat { get { return measureStringFormat; } }
		internal Font DefaultFont { get { return defaultFont; } }
		#endregion
		protected internal override void Initialize() {
			this.defaultFont = CreateDefaultFont();
			this.measureGraphics = CreateMeasureGraphics();
			this.graphicsModifier = new GraphicsToLayoutUnitsModifier(measureGraphics, UnitConverter);
			this.measureStringFormat = CreateMeasureStringFormat();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (graphicsModifier != null) {
						graphicsModifier.Dispose();
						graphicsModifier = null;
					}
					if (measureStringFormat != null) {
						measureStringFormat.Dispose();
						measureStringFormat = null;
					}
					if (measureGraphics != null) {
						measureGraphics.Dispose();
						measureGraphics = null;
					}
					if (defaultFont != null) {
						defaultFont.Dispose();
						defaultFont = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual Graphics CreateMeasureGraphics() {
			return Graphics.FromHwnd(IntPtr.Zero);
		}
		protected internal virtual StringFormat CreateMeasureStringFormat() {
			StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();
			sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
			return sf;
		}
		#region Platform specific methods
		protected internal virtual Font CreateDefaultFont() {
			return new Font("Arial", UnitConverter.PointsToFontUnits(10), FontStyle.Regular, (GraphicsUnit)UnitConverter.FontUnit);
		}
		public override float MeasureCharacterWidthF(char character, FontInfo fontInfo) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			SizeF characterSize = measureGraphics.MeasureString(new String(character, 1), gdiPlusFontInfo.Font, int.MaxValue, measureStringFormat);
			return characterSize.Width;
		}
		public override Size MeasureString(string text, FontInfo fontInfo) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			SizeF result = measureGraphics.MeasureString(text, gdiPlusFontInfo.Font, int.MaxValue, measureStringFormat);
			return new Size((int)Math.Ceiling(result.Width), (int)Math.Ceiling(result.Height));
		}
		public virtual Font CreateFont(string familyName, float emSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			try {
				FontStyle style = FontStyle.Regular;
				if (fontBold) style |= FontStyle.Bold;
				if (fontItalic) style |= FontStyle.Italic;
				if (fontStrikeout) style |= FontStyle.Strikeout;
				if (fontUnderline) style |= FontStyle.Underline;
				return new Font(familyName, emSize, style, (GraphicsUnit)UnitConverter.FontUnit);
			}
			catch {
				return (Font)DefaultFont.Clone();
			}
		}
		static char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		public override float MeasureMaxDigitWidthF(FontInfo fontInfo) {
			int count = digits.Length;
			float result = MeasureCharacterWidthF(digits[0], fontInfo);
			for(int i = 1; i < count; i++)
				result = Math.Max(result, MeasureCharacterWidthF(digits[i], fontInfo));
			return result;
		}
		#endregion
	}
	#endregion
	#region GdiFontInfoMeasurer
	public class GdiFontInfoMeasurer : GdiPlusFontInfoMeasurer {
		#region Fields
		const int arraySize = 10;
		float dpi;
		Graphics graphics;
		#endregion
		public GdiFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
			this.dpi = DevExpress.XtraPrinting.GraphicsDpi.Pixel;
			this.graphics = Graphics.FromHwnd(IntPtr.Zero);
		}
		protected Graphics Graphics { get { return graphics; } }
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(this.graphics != null) {
						this.graphics.Dispose();
						this.graphics = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override float MeasureMaxDigitWidthF(FontInfo fontInfo) {
			float width = 0;
			lock (graphics) {
				IntPtr hdc = graphics.GetHdc();
				try {
					IntPtr hFont = ObtainFontHandle(fontInfo);
					IntPtr oldFont = Win32.SelectObject(hdc, hFont);
					try {
						Win32.ABCFLOAT[] abc = new Win32.ABCFLOAT[arraySize];
						bool success = Win32.GetCharABCWidthsFloat(hdc, '0', '9', abc);
						if (success) {
							width = abc[0].GetWidth();
							for (int i = 1; i < arraySize; i++)
								width = Math.Max(width, abc[i].GetWidth());
						}
					}
					finally {
						Win32.SelectObject(hdc, oldFont);
						ReleaseFontHandle(hFont);
					}
				}
				finally {
					graphics.ReleaseHdc(hdc);
				}
			}
			if(width > 0) {
				float result = CharacterWidthToLayoutUnitsF(width, this.dpi);
				return result;
			}
			return base.MeasureMaxDigitWidthF(fontInfo);
		}
		protected virtual float CharacterWidthToLayoutUnitsF(float width, float dpi) {
			return width;
		}
		public override float MeasureCharacterWidthF(char character, FontInfo fontInfo) {
			float width = 0;			
			lock (MeasureGraphics) {
				IntPtr hdc = MeasureGraphics.GetHdc();
				try {
					IntPtr hFont = ObtainFontHandle(fontInfo);
					IntPtr oldFont = Win32.SelectObject(hdc, hFont);
					try {
						Win32.ABCFLOAT[] abc = new Win32.ABCFLOAT[1];
						bool success = Win32.GetCharABCWidthsFloat(hdc, character, character, abc);
						if (success) {
							width = abc[0].GetWidth();
						}
					}
					finally {
						Win32.SelectObject(hdc, oldFont);
						ReleaseFontHandle(hFont);
					}
				}
				finally {
					MeasureGraphics.ReleaseHdc(hdc);
				}
			}
			float result = CharacterWidthToLayoutUnitsF(width, DevExpress.XtraPrinting.GraphicsDpi.Pixel);
			return result;
		}
		protected virtual IntPtr ObtainFontHandle(FontInfo fontInfo) {
			GdiFontInfo gdiFontInfo = (GdiFontInfo)fontInfo;
			return gdiFontInfo.GdiFontHandle;
		}
		protected virtual void ReleaseFontHandle(IntPtr hFont) {
		}
	}
	#endregion
	#region PureGdiFontInfoMeasurer
	public class PureGdiFontInfoMeasurer : GdiFontInfoMeasurer {
		#region Fields
		const int initialBufferItemCount = 64;
		int glyphsBufferSize;
		IntPtr glyphsBuffer;
		int characterWidthsBufferSize;
		IntPtr characterWidthsBuffer;
		#endregion
		public PureGdiFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
			GetCharacterWidthsBuffer(initialBufferItemCount);
			GetGlyphsBuffer(initialBufferItemCount);
		}
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
		[System.Security.SecuritySafeCritical]
		public override Size MeasureString(string text, FontInfo fontInfo) {
			lock (Graphics) {
				IntPtr hdc = Graphics.GetHdc();
				try {
					GdiPlusFontInfo gdiFontInfo = (GdiPlusFontInfo)fontInfo;
					Win32.SelectObject(hdc, gdiFontInfo.GdiFontHandle);
					try {
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
							0,
							ref gcpResults,
							Win32.GcpFlags.GCP_USEKERNING | Win32.GcpFlags.GCP_LIGATE);
						if (sz == 0 && text.Length > 0) {
							gcpResults.lpDx = IntPtr.Zero;
							gcpResults.lpGlyphs = IntPtr.Zero;
							sz = MeasureWithGetCharacterPlacementSlow(hdc, text, ref gcpResults);
						}
						return new Size(sz & 0x0000FFFF, (int)((sz & 0xFFFF0000) >> 16));
					}
					finally {
					}
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
				if (sz != 0) {
					if ((sz & 0x0000FFFF) == 0)
						sz |= (int)((sz & 0xFFFF0000) >> 16) * text.Length;
					return sz;
				}
			}
			return 0;
		}
		protected override IntPtr ObtainFontHandle(FontInfo fontInfo) {
			return ((GdiPlusFontInfo)fontInfo).Font.ToHfont();
		}
		protected override void ReleaseFontHandle(IntPtr hFont) {
			Win32.DeleteObject(hFont);
		}
		protected override float CharacterWidthToLayoutUnitsF(float width, float dpi) {
			return UnitConverter.PixelsToLayoutUnitsF(width, dpi);
		}
	}
	#endregion
}
