#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfTrueTypeFont : PdfSimpleFont, ITrueTypeFont {
		internal const string Name = "TrueType";
		internal const string FontFile2Key = "FontFile2";
		internal const string Length1Key = "Length1";
		const string winAnsiEncodingName = "WinAnsiEncoding";
		static void UpdateGlyphCodes(short[] str, PdfSimpleFontEncoding encoding, IDictionary<string, ushort> glyphCodes) {
			int length = str.Length;
			for (int i = 0; i < length; i++)
				str[i] = PdfUnicodeConverter.GetGlyphCode(str[i], encoding, glyphCodes);
		}
		internal static void UpdateGlyphCodes(short[] str, PdfSimpleFontEncoding encoding) {
			UpdateGlyphCodes(str, encoding, PdfUnicodeConverter.GlyphCodes);
		}
		readonly byte[] fontFileData;
		readonly byte[] patchedFontFileData;
		readonly byte[] openTypeFontFileData;
		readonly PdfMetadata metadata;
		readonly List<PdfFontCmapGlyphRange> glyphRanges;
		readonly IDictionary<string, ushort> glyphMapping;
		public byte[] FontFileData { get { return fontFileData; } }
		public byte[] OpenTypeFontFileData { get { return openTypeFontFileData; } }
		public PdfMetadata Metadata { get { return metadata; } }
		protected override bool IsCourierFont { 
			get {
				string baseFont = BaseFont;
				return baseFont == CourierNewFontName || baseFont == "CourierNew,Bold" || baseFont == "CourierNew,Italic" || baseFont == "CourierNew,BoldItalic";
			}
		}
		protected override IDictionary<string, short> DefaultWidthsDictionary {
			get {
				switch (BaseFont) {
					case TimesNewRomanFontName:
						return TimesRomanWidths;
					case TimesNewRomanBoldFontName:
						return TimesBoldWidths;
					case "TimesNewRoman,Italic":
						return TimesItalicWidths;
					case "TimesNewRoman,BoldItalic":
						return TimesBoldItalicWidths;
					case HelveticaFontName:
					case ArialFontName:
					case "Arial,Italic":
						return HelveticaWidths;
					case HelveticaBoldFontName:
					case ArialBoldFontName:
					case "Arial,BoldItalic":
						return HelveticaBoldWidths;
					default:
						return null;
				}
			}
		}
		protected internal override string Subtype { get { return Name; } }
		byte[] ITrueTypeFont.FontFileData { get { return patchedFontFileData; } }
		internal PdfTrueTypeFont(string baseFont, PdfReaderDictionary fontDescriptor, double[] widths) 
			: base(baseFont, null, fontDescriptor, PdfSimpleFontEncoding.Create(baseFont, new PdfName(winAnsiEncodingName)), 32, 255, widths) {
		}
		internal PdfTrueTypeFont(string baseFont, PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfSimpleFontEncoding encoding, int? firstChar, int? lastChar, double[] widths)
				: base(baseFont, toUnicode, fontDescriptor, encoding, firstChar, lastChar, widths) {
			if (fontDescriptor != null) {
				PdfReaderStream fontFile2 = fontDescriptor.GetStream(FontFile2Key);
				if (fontFile2 == null)
					openTypeFontFileData = GetOpenTypeFontFileData(fontDescriptor, false);
				else {
					fontFileData = fontFile2.GetData(true);
					PdfTrueTypeFontValidationResult validationResult = PdfTrueTypeFontValidator.Validate(this, fontFileData);
					patchedFontFileData = validationResult.Data;
					glyphRanges = validationResult.GlyphRanges;
					glyphMapping = validationResult.GlyphMapping;
				}
				metadata = fontDescriptor.GetMetadata();
			}
		}
		protected internal override void UpdateGlyphCodes(short[] str) {
			PdfFontDescriptor fontDescriptor = FontDescriptor;
			if (fontDescriptor == null || (fontDescriptor.Flags & PdfFontFlags.Symbolic) == 0 || (fontFileData == null && Encoding.BaseEncoding == PdfBaseEncoding.WinAnsi))
				UpdateGlyphCodes(str, Encoding, glyphMapping == null ? PdfUnicodeConverter.GlyphCodes : glyphMapping);
			else if (glyphRanges != null) {
				int glyphRangeCount = glyphRanges.Count;
				int length = str.Length;
				for (int i = 0; i < length; i++) {
					short value = str[i];
					if (value >= 0x20) {
						int rangeIndex = glyphRanges.BinarySearch(new PdfFontCmapGlyphRange(0, value));
						if (rangeIndex < 0) {
							rangeIndex = ~rangeIndex;
							if (rangeIndex >= glyphRangeCount || value < glyphRanges[rangeIndex].Start)
								str[i] = (short)(0xf000 + value);
						}
					}
				}
			}
		}
		protected internal override void UpdateFontDescriptorDictionary(PdfWriterDictionary dictionary) {
			base.UpdateFontDescriptorDictionary(dictionary);
			if (fontFileData == null)
				WriteOpenTypeFontData(dictionary, openTypeFontFileData);
			else {
				PdfDictionary streamDictionary = new PdfDictionary();
				streamDictionary.Add(Length1Key, fontFileData.Length);
				dictionary.Add(FontFile2Key, dictionary.Objects.AddStream(streamDictionary, fontFileData));
			}
			dictionary.Add(PdfMetadata.Name, metadata);
		}
	}
}
