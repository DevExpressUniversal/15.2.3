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
using System.Text;
using System.Drawing;
using System.Security;
using System.Collections.Generic;
using DevExpress.Data.Helpers;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Interop;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfEditableFontData : PdfDisposableObject {
		public static PdfEditableFontData Create(FontStyle fontStyle, string fontFamily, bool embedded) {
			if (!SecurityHelper.IsUnmanagedCodeGrantedAndHasZeroHwnd)
				return new PdfPartialTrustEditableFontData(fontStyle, fontFamily);
			return embedded ? (PdfEditableFontData)new PdfEmbeddedEditableFontData(fontStyle, fontFamily)
							: (PdfEditableFontData)new PdfNotEmbeddedEditableFontData(fontStyle, fontFamily);
		}
		[SecuritySafeCritical]
		static byte[] GetFontFile(Font font, out bool isTTC) {
			const uint ttcf = (uint)('t' | 't' << 8 | 'c' << 16 | 'f' << 24);
			using (Bitmap b = new Bitmap(1, 1))
			using (Graphics gr = Graphics.FromImage(b)) {
				IntPtr hdc = gr.GetHdc();
				IntPtr oldFont = IntPtr.Zero;
				IntPtr fontH = font.ToHfont();
				try {
					oldFont = Gdi32Interop.SelectObject(hdc, fontH);
					uint length = Gdi32Interop.GetFontData(hdc, ttcf, 0, null, 0);
					uint dwTable = length != uint.MaxValue ? ttcf : 0;
					isTTC = dwTable == ttcf;
					if (length == uint.MaxValue)
						length = Gdi32Interop.GetFontData(hdc, 0, 0, null, 0);
					if (length == uint.MaxValue)
						return null;
					byte[] data = new byte[length];
					length = Gdi32Interop.GetFontData(hdc, dwTable, 0, data, length);
					if (length != data.Length)
						return null;
					return data;
				}
				finally {
					Gdi32Interop.SelectObject(hdc, oldFont);
					Gdi32Interop.DeleteObject(fontH);
					gr.ReleaseHdc(hdc);
				}
			}
		}
		static PdfFontFile CreateFontFile(Font font) {
			const int EnglishLanguageID = 1033;
			bool isTTC;
			byte[] data = GetFontFile(font, out isTTC);
			if (data == null)
				throw new ArgumentException("font");
			string fontNameEnglish = font.FontFamily.GetName(EnglishLanguageID);
			return isTTC ? PdfTrueTypeCollectionFontFile.ReadFontFile(data, fontNameEnglish) : new PdfFontFile(PdfFontFile.TTFVersion, data);
		}
		readonly FontStyle fontStyle;
		readonly string fontFamily;
		readonly PdfFontFile fontFile;
		readonly IPdfGlyphMapper mapper;
		readonly PdfCharacterCache characterCache;
		readonly PdfObjectCollection objects = new PdfObjectCollection(null);
		readonly PdfFontMetrics metrics;
		readonly bool shouldEmulateItalic;
		PdfFont pdfFont;
		public abstract bool Embedded { get; }
		public bool Italic { get { return fontStyle.HasFlag(FontStyle.Italic); } }
		public bool Bold { get { return fontStyle.HasFlag(FontStyle.Bold); } }
		public bool Underline { get { return fontStyle.HasFlag(FontStyle.Underline); } }
		public bool Strikeout { get { return fontStyle.HasFlag(FontStyle.Strikeout); } }
		public FontStyle FontStyle { get { return fontStyle; } }
		public string FontFamily { get { return fontFamily; } }
		public bool ShouldEmulateItalic { get { return shouldEmulateItalic; } }
		public IPdfGlyphMapper Mapper { get { return mapper; } }
		public PdfFontMetrics Metrics { get { return metrics; } }
		public PdfFont PdfFont {
			get {
				if (pdfFont == null)
					InitializePdfFont();
				return pdfFont;
			}
		}
		protected abstract string BaseFont { get; }
		protected PdfCharacterCache CharacterCache { get { return characterCache; } }
		protected virtual bool ShouldCreateTTFFile { get { return true; } }
		internal PdfFontFile FontFile { get { return fontFile; } }
		public abstract PdfStringFormatter CreateStringFormatter(double fontSize);
		public void RegisterString(IDictionary<int, string> mapping) {
			characterCache.RegisterString(mapping);
		}
		public virtual void UpdateFont() {
			PdfFont.ToUnicode = PdfCMapStreamParser.Parse(PdfCharacterMapping.CreateCharacterMappingData(CharacterCache.Mapping, "DEVEXP"));
		}
		public virtual double GetCharWidth(char ch) {
			return fontFile == null ? 0 : fontFile.GetCharacterWidth(mapper.MapCharacter(ch));
		}
		internal double GetCharWidth(ushort glyphIndex) {
			return fontFile == null ? 0 : fontFile.GetCharacterWidth(glyphIndex);
		}
		protected PdfEditableFontData(FontStyle fontStyle, string fontFamily) {
			this.fontStyle = fontStyle;
			this.fontFamily = fontFamily;
			characterCache = new PdfCharacterCache();
			using (Font font = new Font(fontFamily, 12, fontStyle)) {
				if (ShouldCreateTTFFile) {
					fontFile = CreateFontFile(font);
					metrics = new PdfFontMetrics(fontFile);
				}
				else
					metrics = new PdfGraphicsFontMetrics(font.FontFamily, font.Style);
			}
			shouldEmulateItalic = Italic && fontFile != null && fontFile.Post != null && Math.Abs(fontFile.Post.ItalicAngle) < float.Epsilon;
			mapper = CreateMapper();
		}
		protected abstract PdfFont CreateFont(PdfReaderDictionary fontDescriptor);
		protected abstract IPdfGlyphMapper CreateMapper();
		protected virtual void FillFontDescriptor(PdfReaderDictionary fontDescriptorDictionary) {
			if (fontFile != null) {
				PdfFontHheaTableDirectoryEntry hhea = fontFile.Hhea;
				PdfFontHeadTableDirectoryEntry head = fontFile.Head;
				PdfFontFlags flags = PdfFontFlags.Nonsymbolic;
				PdfPanose panose = fontFile.OS2 != null ? fontFile.OS2.Panose : new PdfPanose();
				if (panose.Proportion == PdfPanoseProportion.Monospaced) 
					flags |= PdfFontFlags.FixedPitch;
				if (panose.SerifStyle != PdfPanoseSerifStyle.NormalSans && panose.SerifStyle != PdfPanoseSerifStyle.ObtuseSans && panose.SerifStyle != PdfPanoseSerifStyle.PerpendicularSans)
					flags |= PdfFontFlags.Serif;
				if (panose.FamilyKind == PdfPanoseFamilyKind.LatinHandWritten) 
					flags |= PdfFontFlags.Script;
				if (Italic)
					flags |= PdfFontFlags.Italic;
				fontDescriptorDictionary.Add("ItalicAngle", (double)(fontFile.Post == null ? 0 : fontFile.Post.ItalicAngle));
				fontDescriptorDictionary.Add(PdfFontDescriptor.FlagsDictionaryKey, (int)flags);
			}
			fontDescriptorDictionary.Add("CapHeight", 500);
			fontDescriptorDictionary.Add("StemV", 0);
			fontDescriptorDictionary.Add("FontWeight", Bold ? 700 : 400);
			fontDescriptorDictionary.Add(PdfFontDescriptor.AscentDictionaryKey, (int)Math.Round(metrics.EmAscent));
			fontDescriptorDictionary.Add(PdfFontDescriptor.DescentDictionaryKey, (int)Math.Round(-metrics.EmDescent));
			PdfRectangle bBox = metrics.EmBBox;
			fontDescriptorDictionary.Add(PdfFontDescriptor.FontBBoxDictionaryKey, new object[] { (int)Math.Round(bBox.Left), (int)Math.Round(bBox.Bottom), (int)Math.Round(bBox.Right),
				(int)Math.Round(bBox.Top) });
		}
		protected void InitializePdfFont() {
			PdfReaderDictionary fontDescriptorDictionary = CreateFontDescriptorDictionary();
			pdfFont = CreateFont(fontDescriptorDictionary);
			objects.AddResolvedObject(pdfFont);
			pdfFont.ObjectNumber = -1;
		}
		protected PdfReaderDictionary CreateFontDescriptorDictionary() {
			PdfReaderDictionary fontDescriptorDictionary = new PdfReaderDictionary(objects, PdfObject.DirectObjectNumber, 0);
			fontDescriptorDictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(PdfFontDescriptor.FontDescriptorDictionaryType));
			FillFontDescriptor(fontDescriptorDictionary);
			return fontDescriptorDictionary;
		}
		protected string GetTrueTypeFontName(bool useSubname) {
			const string subName = "DEVEXP";
			StringBuilder nameBuilder = new StringBuilder();
			if (useSubname) {
				nameBuilder.Append(subName);
				nameBuilder.Append('+');
			}
			foreach (char c in fontFamily)
				if (c != ' ' && c != '\x000D' && c != '\x000A')
					nameBuilder.Append(c);
			string postfix = String.Empty;
			if (Bold)
				postfix += "Bold";
			if (Italic)
				postfix += "Italic";
			if (!String.IsNullOrEmpty(postfix)) {
				nameBuilder.Append(",");
				nameBuilder.Append(postfix);
			}
			return nameBuilder.ToString();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				IDisposable disposableMapper = mapper as IDisposable;
				if (disposableMapper != null)
					disposableMapper.Dispose();
				if (fontFile != null)
					fontFile.Dispose();
			}
		}
	}
}
