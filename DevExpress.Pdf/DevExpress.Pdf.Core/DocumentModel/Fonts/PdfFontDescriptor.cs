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

using System;
using System.Text;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfFontStretch.Normal)]
	public enum PdfFontStretch { UltraCondensed, ExtraCondensed, Condensed, SemiCondensed, Normal, SemiExpanded, Expanded, ExtraExpanded, UltraExpanded }
	[Flags]
	public enum PdfFontFlags {
		None = 0x00000, FixedPitch = 0x00001, Serif = 0x00002, Symbolic = 0x00004, Script = 0x00008,
		Nonsymbolic = 0x00020, Italic = 0x00040, AllCap = 0x10000, SmallCap = 0x20000, ForceBold = 0x40000
	};
	public class PdfFontDescriptor : PdfObject {
		internal const string FontDescriptorDictionaryType = "FontDescriptor";
		internal const string FlagsDictionaryKey = "Flags";
		internal const string AscentDictionaryKey = "Ascent";
		internal const string DescentDictionaryKey = "Descent";
		internal const string FontBBoxDictionaryKey = "FontBBox";
		const string fontNameDictionaryKey = "FontName";
		const string fontFamilyDictionaryKey = "FontFamily";
		const string fontStretchDictionaryKey = "FontStretch";
		const string fontWeightDictionaryKey = "FontWeight";
		const string italicAngleDictionaryKey = "ItalicAngle";
		const string leadingDictionaryKey = "Leading";
		const string capHeightDictionaryKey = "CapHeight";
		const string xHeightDictionaryKey = "XHeight";
		const string stemVDictionaryKey = "StemV";
		const string stemHDictionaryKey = "StemH";
		const string avgWidthDictionaryKey = "AvgWidth";
		const string maxWidthDictionaryKey = "MaxWidth";
		const string missingWidthDictionaryKey = "MissingWidth";
		const string charSetDictionaryKey = "CharSet";
		const int fontWeightNormal = 400;
		static readonly byte[] timesFontFamilyName = new byte[] { (byte)'T', (byte)'i', (byte)'m', (byte)'e', (byte)'s' };
		static readonly byte[] helveticaFontFamilyName = new byte[] { (byte)'H', (byte)'e', (byte)'l', (byte)'v', (byte)'e', (byte)'t', (byte)'i', (byte)'c', (byte)'a' };
		static readonly byte[] courierFontFamilyName = new byte[] { (byte)'C', (byte)'o', (byte)'u', (byte)'r', (byte)'i', (byte)'e', (byte)'r' };
		static readonly byte[] symbolFontFamilyName = new byte[] { (byte)'S', (byte)'y', (byte)'m', (byte)'b', (byte)'o', (byte)'l' };
		static readonly byte[] zapfDingbatsFontFamilyName =
			new byte[] { (byte)'Z', (byte)'a', (byte)'p', (byte)'f', (byte)'D', (byte)'i', (byte)'n', (byte)'g', (byte)'b', (byte)'a', (byte)'t', (byte)'s' };
		internal static PdfReaderDictionary GetStandardFontDescriptor(PdfObjectCollection objects, string fontName) {
			PdfReaderDictionary fontDescriptor = new PdfReaderDictionary(objects, 0, 0);
			fontDescriptor.Add(PdfDictionary.DictionaryTypeKey, new PdfName(FontDescriptorDictionaryType));
			fontDescriptor.Add(fontNameDictionaryKey, new PdfName(fontName));
			switch (fontName) {
				case PdfType1Font.TimesRomanFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, timesFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.SmallCap));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -168, -218, 1000, 898 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 683);
					fontDescriptor.Add(DescentDictionaryKey, -217);
					fontDescriptor.Add(capHeightDictionaryKey, 662);
					fontDescriptor.Add(xHeightDictionaryKey, 450);
					fontDescriptor.Add(stemVDictionaryKey, 84);
					fontDescriptor.Add(stemHDictionaryKey, 28);
					break;
				case PdfType1Font.TimesBoldFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, timesFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.SmallCap | PdfFontFlags.ForceBold));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -168, -218, 1000, 935 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 683);
					fontDescriptor.Add(DescentDictionaryKey, -217);
					fontDescriptor.Add(capHeightDictionaryKey, 676);
					fontDescriptor.Add(xHeightDictionaryKey, 461);
					fontDescriptor.Add(stemVDictionaryKey, 139);
					fontDescriptor.Add(stemHDictionaryKey, 44);
					break;
				case PdfType1Font.TimesItalicFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, timesFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.Italic | PdfFontFlags.SmallCap));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -169, -217, 1010, 883 });
					fontDescriptor.Add(italicAngleDictionaryKey, -15.5);
					fontDescriptor.Add(AscentDictionaryKey, 683);
					fontDescriptor.Add(DescentDictionaryKey, -217);
					fontDescriptor.Add(capHeightDictionaryKey, 653);
					fontDescriptor.Add(xHeightDictionaryKey, 441);
					fontDescriptor.Add(stemVDictionaryKey, 76);
					fontDescriptor.Add(stemHDictionaryKey, 32);
					break;
				case PdfType1Font.TimesBoldItalicFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, timesFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.Italic | PdfFontFlags.SmallCap | PdfFontFlags.ForceBold));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -200, -218, 996, 921 });
					fontDescriptor.Add(italicAngleDictionaryKey, -15);
					fontDescriptor.Add(AscentDictionaryKey, 683);
					fontDescriptor.Add(DescentDictionaryKey, -217);
					fontDescriptor.Add(capHeightDictionaryKey, 669);
					fontDescriptor.Add(xHeightDictionaryKey, 462);
					fontDescriptor.Add(stemVDictionaryKey, 121);
					fontDescriptor.Add(stemHDictionaryKey, 42);
					break;
				case PdfSimpleFont.ArialFontName:
				case PdfType1Font.HelveticaFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, helveticaFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Nonsymbolic | PdfFontFlags.SmallCap));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -166, -225, 1000, 931 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 718);
					fontDescriptor.Add(DescentDictionaryKey, -207);
					fontDescriptor.Add(capHeightDictionaryKey, 718);
					fontDescriptor.Add(xHeightDictionaryKey, 523);
					fontDescriptor.Add(stemVDictionaryKey, 88);
					fontDescriptor.Add(stemHDictionaryKey, 76);
					break;
				case PdfType1Font.HelveticaObliqueFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, helveticaFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Nonsymbolic | PdfFontFlags.Italic | PdfFontFlags.SmallCap));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -170, -225, 1116, 931 });
					fontDescriptor.Add(italicAngleDictionaryKey, -12);
					fontDescriptor.Add(AscentDictionaryKey, 718);
					fontDescriptor.Add(DescentDictionaryKey, -207);
					fontDescriptor.Add(capHeightDictionaryKey, 718);
					fontDescriptor.Add(xHeightDictionaryKey, 523);
					fontDescriptor.Add(stemVDictionaryKey, 88);
					fontDescriptor.Add(stemHDictionaryKey, 76);
					break;
				case PdfType1Font.HelveticaBoldFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, helveticaFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Nonsymbolic | PdfFontFlags.SmallCap | PdfFontFlags.ForceBold));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -170, -228, 1003, 962 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 718);
					fontDescriptor.Add(DescentDictionaryKey, -207);
					fontDescriptor.Add(capHeightDictionaryKey, 718);
					fontDescriptor.Add(xHeightDictionaryKey, 532);
					fontDescriptor.Add(stemVDictionaryKey, 140);
					fontDescriptor.Add(stemHDictionaryKey, 118);
					break;
				case PdfType1Font.HelveticaBoldObliqueFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, helveticaFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.Nonsymbolic | PdfFontFlags.Italic | PdfFontFlags.SmallCap | PdfFontFlags.ForceBold));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -174, -228, 1114, 962 });
					fontDescriptor.Add(italicAngleDictionaryKey, -12);
					fontDescriptor.Add(AscentDictionaryKey, 718);
					fontDescriptor.Add(DescentDictionaryKey, -207);
					fontDescriptor.Add(capHeightDictionaryKey, 718);
					fontDescriptor.Add(xHeightDictionaryKey, 532);
					fontDescriptor.Add(stemVDictionaryKey, 140);
					fontDescriptor.Add(stemHDictionaryKey, 118);
					break;
				case PdfType1Font.CourierFontName:
				case PdfSimpleFont.CourierNewFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, courierFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.FixedPitch | PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.SmallCap));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -23, -250, 715, 805 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 629);
					fontDescriptor.Add(DescentDictionaryKey, -157);
					fontDescriptor.Add(capHeightDictionaryKey, 562);
					fontDescriptor.Add(xHeightDictionaryKey, 426);
					fontDescriptor.Add(stemVDictionaryKey, 51);
					fontDescriptor.Add(stemHDictionaryKey, 51);
					break;
				case PdfType1Font.CourierObliqueFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, courierFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.FixedPitch | PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.Italic | PdfFontFlags.SmallCap));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -27, -250, 849, 805 });
					fontDescriptor.Add(italicAngleDictionaryKey, -12);
					fontDescriptor.Add(AscentDictionaryKey, 629);
					fontDescriptor.Add(DescentDictionaryKey, -157);
					fontDescriptor.Add(capHeightDictionaryKey, 562);
					fontDescriptor.Add(xHeightDictionaryKey, 426);
					fontDescriptor.Add(stemVDictionaryKey, 51);
					fontDescriptor.Add(stemHDictionaryKey, 51);
					break;
				case PdfType1Font.CourierBoldFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, courierFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)(PdfFontFlags.FixedPitch | PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.SmallCap | PdfFontFlags.ForceBold));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -113, -250, 749, 801 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 629);
					fontDescriptor.Add(DescentDictionaryKey, -157);
					fontDescriptor.Add(capHeightDictionaryKey, 562);
					fontDescriptor.Add(xHeightDictionaryKey, 439);
					fontDescriptor.Add(stemVDictionaryKey, 106);
					fontDescriptor.Add(stemHDictionaryKey, 84);
					break;
				case PdfType1Font.CourierBoldObliqueFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, courierFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, 
						(int)(PdfFontFlags.FixedPitch | PdfFontFlags.Serif | PdfFontFlags.Nonsymbolic | PdfFontFlags.Italic | PdfFontFlags.SmallCap | PdfFontFlags.ForceBold));
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -57, -250, 869, 801 });
					fontDescriptor.Add(italicAngleDictionaryKey, -12);
					fontDescriptor.Add(AscentDictionaryKey, 629);
					fontDescriptor.Add(DescentDictionaryKey, -157);
					fontDescriptor.Add(capHeightDictionaryKey, 562);
					fontDescriptor.Add(xHeightDictionaryKey, 439);
					fontDescriptor.Add(stemVDictionaryKey, 106);
					fontDescriptor.Add(stemHDictionaryKey, 84);
					break;
				case PdfType1Font.SymbolFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, symbolFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)PdfFontFlags.Symbolic);
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -180, -293, 1090, 1010 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 0);
					fontDescriptor.Add(DescentDictionaryKey, 0);
					fontDescriptor.Add(capHeightDictionaryKey, 0);
					fontDescriptor.Add(xHeightDictionaryKey, 0);
					fontDescriptor.Add(stemVDictionaryKey, 85);
					fontDescriptor.Add(stemHDictionaryKey, 92);
					break;
				case PdfType1Font.ZapfDingbatsFontName:
					fontDescriptor.Add(fontFamilyDictionaryKey, zapfDingbatsFontFamilyName);
					fontDescriptor.Add(FlagsDictionaryKey, (int)PdfFontFlags.Symbolic);
					fontDescriptor.Add(FontBBoxDictionaryKey, new object[] { -1, -143, 981, 820 });
					fontDescriptor.Add(italicAngleDictionaryKey, 0);
					fontDescriptor.Add(AscentDictionaryKey, 0);
					fontDescriptor.Add(DescentDictionaryKey, 0);
					fontDescriptor.Add(capHeightDictionaryKey, 0);
					fontDescriptor.Add(xHeightDictionaryKey, 0);
					fontDescriptor.Add(stemVDictionaryKey, 90);
					fontDescriptor.Add(stemHDictionaryKey, 28);
					break;
				default:
					return null;
			}
			return fontDescriptor;
		}
		readonly PdfFont font;
		readonly string fontName;
		readonly string fontFamily;
		readonly PdfFontStretch fontStretch;
		readonly int fontWeight = fontWeightNormal;
		readonly PdfFontFlags flags;
		readonly PdfRectangle fontBBox;
		readonly double italicAngle;
		readonly double ascent;
		readonly double descent;
		readonly double leading;
		readonly double capHeight;
		readonly double xHeight;
		readonly double stemV;
		readonly double stemH;
		readonly double avgWidth;
		readonly double maxWidth;
		readonly double missingWidth;
		readonly IList<string> charSet;
		public string FontName { get { return fontName; } }
		public string FontFamily { get { return fontFamily; } }
		public PdfFontStretch FontStretch { get { return fontStretch; } }
		public int FontWeight { get { return fontWeight; } }
		public PdfFontFlags Flags { get { return flags; } }
		public PdfRectangle FontBBox { get { return fontBBox; } }
		public double ItalicAngle { get { return italicAngle; } }
		public double Ascent { get { return ascent; } }
		public double Descent { get { return descent; } }
		public double Leading { get { return leading; } }
		public double CapHeight { get { return capHeight; } }
		public double XHeight { get { return xHeight; } }
		public double StemV { get { return stemV; } }
		public double StemH { get { return stemH; } }
		public double AvgWidth { get { return avgWidth; } }
		public double MaxWidth { get { return maxWidth; } }
		public double MissingWidth { get { return missingWidth; } }
		public IList<string> CharSet { get { return charSet; } }
		internal double ActualAscent { get { return ((ascent == 0 || descent == 0) && fontBBox != null) ? Math.Max(0, fontBBox.Top) : ascent; } }
		internal double ActualDescent { get { return ((ascent == 0 || descent == 0) && fontBBox != null) ? Math.Min(0, fontBBox.Bottom) : descent; } }
		internal double Height { 
			get { 
				if (fontBBox != null) {
					double height = fontBBox.Height;
					if (height != 0)
						return height;
				}
				return ascent - descent;
			} 
		}
		internal PdfFontDescriptor(PdfFont font, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			this.font = font;
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			fontName = dictionary.GetName(fontNameDictionaryKey) ?? font.BaseFont;
			object value;
			if (dictionary.TryGetValue(fontFamilyDictionaryKey, out value)) {
				value = dictionary.Objects.TryResolve(value);
				PdfName name = value as PdfName;
				fontFamily = name != null ? name.Name : dictionary.GetString(fontFamilyDictionaryKey);
			}
			fontStretch = PdfEnumToStringConverter.Parse<PdfFontStretch>(dictionary.GetName(fontStretchDictionaryKey));
			fontWeight = dictionary.GetInteger(fontWeightDictionaryKey) ?? fontWeightNormal;
			if (fontWeight == 0)
				fontWeight = fontWeightNormal;
			int? flagsInteger = dictionary.GetInteger(FlagsDictionaryKey);
			if ((type != null && type != FontDescriptorDictionaryType && type != PdfFont.DictionaryType) || fontWeight <= 0 || !flagsInteger.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			flags = (PdfFontFlags)flagsInteger.Value;
			IList<object> fontBBoxArray = dictionary.GetArray(FontBBoxDictionaryKey);
			italicAngle = dictionary.GetNumber(italicAngleDictionaryKey) ?? 0;
			ascent = dictionary.GetNumber(AscentDictionaryKey) ?? 0;
			if (ascent < 0)
				ascent = -ascent;
			descent = dictionary.GetNumber(DescentDictionaryKey) ?? 0;
			if (descent > 0)
				descent = -descent;
			if (fontBBoxArray == null) {
				if (font.HasSizeAttributes)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else
				fontBBox = new PdfRectangle(fontBBoxArray);
			leading = dictionary.GetNumber(leadingDictionaryKey) ?? 0;
			capHeight = dictionary.GetNumber(capHeightDictionaryKey) ?? 0;
			xHeight = dictionary.GetNumber(xHeightDictionaryKey) ?? 0;
			stemV = dictionary.GetNumber(stemVDictionaryKey) ?? 0;
			stemH = dictionary.GetNumber(stemHDictionaryKey) ?? 0;
			avgWidth = dictionary.GetNumber(avgWidthDictionaryKey) ?? 0;
			maxWidth = dictionary.GetNumber(maxWidthDictionaryKey) ?? 0;
			missingWidth = dictionary.GetNumber(missingWidthDictionaryKey) ?? 0;
			if (stemV < 0 || missingWidth < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			byte[] charSetBytes = dictionary.GetBytes(charSetDictionaryKey);
			if (charSetBytes != null)
				try {
					charSet = PdfDocumentReader.IsUnicode(charSetBytes) ? PdfCharSetStringParser.Parse(PdfDocumentReader.ConvertToUnicodeString(charSetBytes)) : 
																		  PdfObjectParser.ParseNameArray(charSetBytes);
				}
				catch {
				}
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, FontDescriptorDictionaryType);
			dictionary.AddName(fontNameDictionaryKey, fontName);
			dictionary.Add(fontFamilyDictionaryKey, PdfDocumentStream.ConvertToArray(fontFamily), null);
			dictionary.AddEnumName(fontStretchDictionaryKey, fontStretch);
			dictionary.Add(fontWeightDictionaryKey, fontWeight, fontWeightNormal);
			dictionary.Add(FlagsDictionaryKey, (int)flags);
			dictionary.Add(italicAngleDictionaryKey, italicAngle);
			dictionary.Add(leadingDictionaryKey, leading, 0.0);
			dictionary.Add(xHeightDictionaryKey, xHeight, 0.0);
			dictionary.Add(stemHDictionaryKey, stemH, 0.0);
			dictionary.Add(avgWidthDictionaryKey, avgWidth, 0.0);
			dictionary.Add(maxWidthDictionaryKey, maxWidth, 0.0);
			dictionary.Add(missingWidthDictionaryKey, missingWidth, 0.0);
			if (font.HasSizeAttributes) {
				dictionary.Add(FontBBoxDictionaryKey, fontBBox);
				dictionary.Add(AscentDictionaryKey, ascent);
				dictionary.Add(DescentDictionaryKey, descent);
				dictionary.Add(capHeightDictionaryKey, capHeight, 0.0);
				dictionary.Add(stemVDictionaryKey, stemV);
			}
			if (charSet != null) {
				bool isUnicode = false;
				StringBuilder sb = new StringBuilder();
				foreach (string ch in charSet) {
					sb.Append("/");
					sb.Append(ch);
					if (!isUnicode)
						foreach (char c in ch)
							if ((int)c >= 254) {
								isUnicode = true;
								break;
							}
				}
				string charSetString = sb.ToString();
				if (isUnicode)
					dictionary.Add(charSetDictionaryKey, "(" + charSetString + ")");
				else 
					dictionary.AddASCIIString(charSetDictionaryKey, charSetString);
			}
			font.UpdateFontDescriptorDictionary(dictionary);
			return dictionary;
		}
	}
}
