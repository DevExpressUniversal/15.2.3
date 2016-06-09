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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Data.Helpers;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.Pdf.Common;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfFont : IDisposable {
		public const float MaxFontSize = 300;
		[System.Security.SecuritySafeCritical]
		static int GetFontCodePage(Font font) {
			Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
			IntPtr hdc = graphics.GetHdc();
			IntPtr fontHandle = font.ToHfont();
			IntPtr oldFont = GDIFunctions.SelectObject(hdc, fontHandle);
			try {
				int info = GDIFunctions.GetTextCharset(hdc);
				CHARSETINFO charSetInfo = new CHARSETINFO();
				GDIFunctions.TranslateCharsetInfo((uint)info, ref charSetInfo, 1);
				return charSetInfo.ciACP;
			}
			finally {
				GDIFunctions.SelectObject(hdc, oldFont);
				GDIFunctions.DeleteObject(fontHandle);
				graphics.ReleaseHdc(hdc);
				graphics.Dispose();
			}
		}
		Font font;
		protected PdfFontBase innerFont;
		TTFFile ttfFile;		
		string name;
		float scaleFactor;
		PdfCharCache charCache;
		bool needToEmbedFont = true;
		protected bool compressed;
		bool? emulateBold;
		internal TTFFile TTFFile { get { return ttfFile; } }
		public PdfDictionary Dictionary { get { return (innerFont != null)? innerFont.Dictionary: null; } }
		public float ScaleFactor { get { return scaleFactor; } }
		public PdfCharCache CharCache { 
			get { 
				if(charCache == null)
					charCache = CreatePdfFontCache();
				return charCache; 
			} 
		}
		public Font Font { get { return font; } }
		public string Name { get { return name; } }
		public bool NeedToEmbedFont { get { return needToEmbedFont; } set { needToEmbedFont = value; } }
		public PdfFont(Font font, bool compressed) {
			this.font = (Font)font.Clone();
			this.compressed = compressed;
			if(ShouldCreateTTFFile)
				InitTTFFile(CreateTTFFile());
		}
		internal PdfFont(Font font, TTFFile ttfFile, bool compressed) {
			this.font = (Font)font.Clone();
			this.compressed = compressed;
			InitTTFFile(ttfFile);
		}
		protected virtual bool ShouldCreateTTFFile {
			get {
				return true;
			}
		}
		protected virtual PdfCharCache CreatePdfFontCache() {
			return new PdfCharCache();
		}
		void InitTTFFile(TTFFile ttfFile) {
			if(ttfFile == null) return;
			this.ttfFile = ttfFile;
			scaleFactor = (float)1000 / ttfFile.Head.UnitsPerEm;
		}
		TTFFile CreateTTFFile(byte[] data, bool isTtcFile, int fontCodePage) {
			if(isTtcFile) {
				TTCFile ttcFile = new TTCFile();
				string fontNameEnglish = font.FontFamily.GetName(TTFName.EnglishLanguageID);
				return ttcFile.Read(data, fontNameEnglish, fontCodePage);
			}
			else {
				TTFFile file = new TTFFile(0, fontCodePage);
				file.Read(data);
				return file;
			}
		}
		TTFFile CreateTTFFile() {
			using(Font fakeFont = CreateFakeFont()) {
				bool isTTC;
				byte[] data = PdfFontUtils.CreateTTFFile(fakeFont, out isTTC);
				return CreateTTFFile(data, isTTC, GetFontCodePage(fakeFont));
			}
		}
		Font CreateFakeFont() {
			string fontName = "Symbol";
			return 
				this.font.Name == fontName ?
				new Font(fontName, 9, this.Font.Style) :
				(Font)this.font.Clone();
		}
		protected internal  virtual void CreateInnerFont() {
			if(innerFont != null)
				return;
			if(NeedToEmbedFont)
				innerFont = new PdfType0Font(this, this.compressed);
			else
				innerFont = new PdfTrueTypeFont(this, this.compressed);
		}
		protected internal void SetName(string name) {
			if(this.name == null)
				this.name = name;
		}
		protected internal void WriteFontSubset(Stream stream) {
			ttfFile.Write(stream, CharCache, PdfFontUtils.GetTrueTypeFontName0(font.Name, true));
		}
		internal ushort GetGlyphByChar(char ch) {
			return ttfFile.GetGlyphIndex(ch);
		}
		internal float GetCharWidth(ushort glyphIndex) {
			return ttfFile.GetCharWidth(glyphIndex) * scaleFactor;
		}
		public float GetCharWidth(char ch) {
			return ttfFile.GetCharWidth(ch) * scaleFactor;
		}
		public string ProcessText(TextRun textRun) {
			if(innerFont == null)
				throw new PdfException("The inner font doesn't specified yet");
			return innerFont.ProcessText(textRun);
		}
		public void Register(PdfXRef xRef) {
			if(innerFont != null)
				innerFont.Register(xRef);
		}
		public void Write(StreamWriter writer) {
			if(innerFont != null)
				innerFont.Write(writer);
		}
		public void FillUp() {
			if(innerFont != null) {
				CharCache.CalculateGlyphIndeces(this.ttfFile);
				innerFont.FillUp();
			}
		}
		public void Dispose() {
			if(this.font != null) {
				this.font.Dispose();
				this.font = null;
			}
			if(this.innerFont != null) {
				this.innerFont.Dispose();
				this.innerFont = null;
			}
		}
		public bool EmulateBold(PdfFonts fonts) {
			if(!emulateBold.HasValue) {
				emulateBold = ShouldEmulateBold(fonts);
			}
			return emulateBold.Value;
		}
		bool ShouldEmulateBold(PdfFonts fonts) {
			if(!font.Bold || !font.FontFamily.IsStyleAvailable(FontStyle.Regular))
				return false;
			using(Font regularFont = new Font(font, FontStyle.Regular)) {
				PdfFont pdfRegularFont = fonts.FindFont(regularFont);
				bool dispose = false;
				if(pdfRegularFont == null) {
					pdfRegularFont = PdfFonts.CreatePdfFont(regularFont, this.compressed);
					dispose = true;
				}
				bool result = TTFFile.IsIdentical(TTFFile, pdfRegularFont.TTFFile);
				if(dispose)
					pdfRegularFont.Dispose();
				return result;
			}
		}
	}
	public class PdfFonts  {
		internal static PdfFont CreatePdfFont(Font font, bool compressed) {
			return SecurityHelper.IsUnmanagedCodeGrantedAndHasZeroHwnd && !PdfGraphics.EnableAzureCompatibility
				? new PdfFont(font, compressed) : new PartialTrustPdfFont(font, compressed);
		}
		ArrayList list = new ArrayList();
		public int Count { get { return list.Count; } }
		public PdfFont this[int index] { get { return list[index] as PdfFont; } }
		PdfFont Add(PdfFont pdfFont) {
			this.list.Add(pdfFont);
			pdfFont.SetName("F" + Convert.ToString(Count));
			return pdfFont;
		}		
		public void AddUnique(PdfFont pdfFont) {
			if(!this.list.Contains(pdfFont))
				Add(pdfFont);
		}
		public PdfFont FindFont(Font font) {
			foreach(PdfFont pdfFont in this.list)
				if(pdfFont.Font.Equals(font) || PdfFontUtils.FontEquals(pdfFont.Font, font))
					return pdfFont;
			return null;
		}
		public PdfFont RegisterFont(Font font, bool compressed) {
			PdfFont res = FindFont(font);
			if(res != null) return res;
			return Add(CreatePdfFont(font, compressed));
		}
		public void DisposeAndClear() {
			foreach(PdfFont pdfFont in this.list)
				pdfFont.Dispose();
			this.list.Clear();
		}
		public void FillUp() {
			foreach(PdfFont pdfFont in this.list)
				pdfFont.FillUp();
		}
		public void Register(PdfXRef xRef) {
			foreach(PdfFont pdfFont in this.list)
				pdfFont.Register(xRef);
		}
		public void Write(StreamWriter writer) {
			foreach(PdfFont pdfFont in this.list)
				pdfFont.Write(writer);
		}
	}
	public enum PdfFontFlags {
		FixedWidthFont = 1,
		SerifFont = 2,
		SymbolicFont = 3,
		ScriptFont = 4,
		UsesTheAdobeStandardRomanCharacterSet = 6,
		Italic = 7,
		AllCapFont = 17,
		SmallCapFont = 18,
		ForceBoldAtSmallTextSize = 19
	}
	public class PartialTrustPdfCharCache : PdfCharCache {
		protected override bool ShouldExpandCompositeGlyphs { get { return false; } }
	}
}
