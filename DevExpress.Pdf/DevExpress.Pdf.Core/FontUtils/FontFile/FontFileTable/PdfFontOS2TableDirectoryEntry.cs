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
namespace DevExpress.Pdf.Native {
	public class PdfFontOS2TableDirectoryEntry : PdfFontTableDirectoryEntry {
		enum Version { TrueType_1_5 = 0, TrueType_1_66 = 1, OpenType_1_2 = 2, OpenType_1_4 = 3, OpenType_1_5 = 4 }
		[Flags]
		enum EmbeddingType { InstallableEmbedding = 0x000, RestrictedLicense = 0x002, PreviewPrintEmbedding = 0x004, EditableEmbedding = 0x008,  NoSubsetting = 0x100, BitmapEmbeddingOnly = 0x200 }
		enum WidthClass { UltraCondensed = 1, ExtraCondensed = 2, Condensed = 3, SemiCondensed = 4, Medium = 5, SemiExpanded = 6, Expanded = 7, ExtraExpanded = 8, UltraExpanded = 9 }
		[Flags]
		enum UnicodeRange1 : uint { Empty = 0x00000000, BasicLatin = 0x00000001, Latin1Supplement = 0x00000002, LatinExtendedA = 0x00000004, LatinExtendedB = 0x00000008, IPAExtensions = 0x00000010, 
									SpacingModifiersLetters = 0x00000020, CombiningDiacriticalMarks = 0x00000040, GreekAndCoptic = 0x00000080, Coptic = 0x00000100, Cyrillic = 0x00000200, 
									Armenian = 0x00000400, Hebrew = 0x00000800, Vai = 0x00001000, Arabic = 0x00002000, NKo = 0x00004000, Devanagari = 0x00008000, Bengali = 0x00010000, Gurmukhi = 0x00020000, 
									Gujarati = 0x00040000, Oriya = 0x00080000, Tamil = 0x00100000, Telugu = 0x00200000, Kannada = 0x00400000, Malayalam = 0x00800000, Thai = 0x01000000, Lao = 0x02000000, 
									Georgian = 0x04000000, Balinese = 0x08000000, HangulJamo = 0x10000000, LatinExtendedAdditional = 0x20000000, GreekExtended = 0x40000000, GeneralPunctuation = 0x80000000 }
		[Flags]
		enum UnicodeRange2 : uint { Empty = 0x00000000, SuperscriptsAndSubscripts = 0x00000001, CurrencySymbols = 0x00000002, CombiningDiacriticalMarksForSymbols = 0x00000004, 
									LetterlikeSymbols = 0x00000008, NumberForms = 0x00000010, Arrows = 0x00000020, MathematicalOperators = 0x00000040, MiscellaneousTechnical = 0x00000080, 
									ControlPictures = 0x00000100, OpticalCharacterRecognition = 0x00000200, EnclosedAlphanumerics = 0x00000400, BoxDrawing = 0x00000800, BlockElements = 0x00001000,
									GeometricShapes = 0x00002000, MiscellaneousSymbols = 0x00004000, Dingbats = 0x00008000, CJKSymbolsAndPunctuation = 0x00010000, Hiragana = 0x00020000, 
									Katakana = 0x00040000, Bopomofo = 0x00080000, HangulCompatibilityJamo = 0x00100000, PhagsPa = 0x00200000, EnclosedCJKLettersAndMonths = 0x00400000, 
									CJKCompatibility = 0x00800000, HangulSyllables = 0x01000000, NonPlane0 = 0x02000000, Phoenician = 0x04000000, CJKUnifiedIdeographs = 0x08000000, 
									PrivateUseAreaPlane0 = 0x10000000, CJKStrokes = 0x20000000, AlphabeticPresentationForms = 0x40000000, ArabicPresentationFormsA = 0x80000000 }
		[Flags]
		enum UnicodeRange3 : uint { Empty = 0x00000000, CombiningHalfMarks = 0x00000001, VerticalForms = 0x00000002, SmallFormsVariants = 0x00000004, ArabicPresentationFormsB = 0x00000008,
									HalfwidthAndFullwidthForms = 0x00000010, Specials = 0x00000020, Tibetan = 0x00000040, Syriac = 0x00000080, Thaana = 0x00000100, Sinhala = 0x00000200,
									Myanmar = 0x00000400, Ethiopic = 0x00000800, Cherokee = 0x00001000, UnifiedCanadianAboriginalSyllabics = 0x00002000, Ogham = 0x00004000, 
									Runic = 0x00008000, Khmer = 0x00010000, Mongolian = 0x00020000, BraillePatterns = 0x00040000, YiSyllables = 0x00080000, Tagalog = 0x00100000, 
									OldItalic = 0x00200000, Gothic = 0x00400000, Deseret = 0x00800000, MusicalSymbols = 0x01000000, MathematicalAlphanumericSymbols = 0x02000000, 
									PrivateUsePlane15_16 = 0x04000000, VariationSelectors = 0x08000000, Tags = 0x10000000, Limbu = 0x20000000, TaiLe = 0x40000000, NewTaiLe = 0x80000000 }
		[Flags]
		enum UnicodeRange4 : uint { Empty = 0x00000000, Buginese = 0x00000001, Glagolitic = 0x00000002, Tifinagh = 0x00000004, YijingHexagramSymbols = 0x00000008, 
									SylotiNagri = 0x00000010, LinearBSyllabary = 0x00000020, AncientGreekNumbers = 0x00000040, Ugaritic = 0x00000080, OldPersian = 0x00000100, Shavian = 0x00000200, 
									Osmanya = 0x00000400, CypriotSyllabary = 0x00000800, Kharoshthi = 0x00001000, TaiXuanJingSymbols = 0x00002000, Cuneiform = 0x00004000, 
									CountingRodNumerals = 0x00008000, Sundanese = 0x00010000, Lepcha = 0x00020000, OlChiki = 0x00040000, Saurashtra = 0x00080000, KayahLi = 0x00100000, 
									Rejang = 0x00200000, Cham = 0x00400000, AncientSymbols = 0x00800000, PhaistosDisc = 0x01000000, Carian = 0x02000000, DominoTiles = 0x04000000 }
		[Flags]
		enum Selection { Empty = 0x000, ITALIC = 0x001, UNDERSCORE = 0x002, NEGATIVE = 0x004, OUTLINED = 0x008, 
						 STRIKEOUT = 0x010, BOLD = 0x020, REGULAR = 0x040, USE_TYPO_METRICS = 0x080, WWS = 0x100, OBLIQUE = 0x200 }
		[Flags]
		enum CodePageRange1 : uint { Empty = 0x00000000, Latin1 = 0x00000001, Latin2EasternEurope = 0x00000002, Cyrillic = 0x00000004, Greek = 0x00000008, 
									 Turkish = 0x00000010, Hebrew = 0x00000020, Arabic = 0x00000040, WindowsBaltic = 0x00000080, Vietnamese = 0x00000100, 
									 Thai = 0x00010000, JISJapan = 0x00020000, ChineseSimplified = 0x00040000, KoreanWansung = 0x00080000, ChineseTraditional = 0x00100000, 
									 KoreanJohab = 0x00200000, MacintoshCharacterSet = 0x20000000, OEMCharacterSet = 0x40000000, SymbolCharacterSet = 0x80000000 } 
		[Flags]
		enum CodePageRange2 : uint { Empty = 0x00000000, IBMGreek = 0x00010000, MSDOSRussian = 0x00020000, MSDOSNordic = 0x00040000, Arabic = 0x00080000, MSDOSCanadianFrench = 0x00100000, 
									 Hebrew = 0x00200000, MSDOSIcelandic = 0x00400000, MSDOSPortuguese = 0x00800000, IBMTurkish = 0x01000000, IBMCyrillic = 0x02000000, Latin2 = 0x04000000, 
									 MSDOSBaltic = 0x08000000, GreekFormer437G = 0x10000000, ArabicASMO708 = 0x20000000, WELatin1 = 0x40000000, US = 0x80000000 } 
		internal const string EntryTag = "OS/2";
		const short normalFontWeight = 400;
		const short boldFontWeight = 700;
		Version version;
		short avgCharWidth;
		short weightClass;
		WidthClass widthClass;
		EmbeddingType embeddingType;
		short subscriptXSize;
		short subscriptYSize;
		short subscriptXOffset;
		short subscriptYOffset;
		short superscriptXSize;
		short superscriptYSize;
		short superscriptXOffset;
		short superscriptYOffset;
		short strikeoutSize;
		short strikeoutPosition;
		PdfFontFamilyClass familyClass;
		PdfPanose panose;
		UnicodeRange1 unicodeRange1;
		UnicodeRange2 unicodeRange2;
		UnicodeRange3 unicodeRange3;
		UnicodeRange4 unicodeRange4;
		string vendor;
		Selection selection;
		short firstCharIndex;
		short lastCharIndex;
		short typoAscender;
		short typoDescender;
		short typoLineGap;
		short winAscent;
		short winDescent;
		CodePageRange1 codePageRange1;
		CodePageRange2 codePageRange2;
		short xHeight;
		short capHeight;
		short defaultChar = 0;
		short breakChar = 32;
		short maxContext = 0;
		bool shouldWrite;
		public PdfPanose Panose { get { return panose; } }
		public short TypoLineGap { get { return typoLineGap; } }
		public PdfFontOS2TableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
			PdfBinaryStream stream = TableStream;
			version = (Version)stream.ReadShort();
			avgCharWidth = stream.ReadShort();
			weightClass = stream.ReadShort();
			widthClass = (WidthClass)stream.ReadShort();
			embeddingType = (EmbeddingType)stream.ReadShort();
			subscriptXSize = stream.ReadShort();
			subscriptYSize = stream.ReadShort();
			subscriptXOffset = stream.ReadShort();
			subscriptYOffset = stream.ReadShort();
			superscriptXSize = stream.ReadShort();
			superscriptYSize = stream.ReadShort();
			superscriptXOffset = stream.ReadShort();
			superscriptYOffset = stream.ReadShort();
			strikeoutSize = stream.ReadShort();
			strikeoutPosition = stream.ReadShort();
			familyClass = (PdfFontFamilyClass)stream.ReadShort();
			panose = new PdfPanose(stream);
			unicodeRange1 = (UnicodeRange1)stream.ReadInt();
			unicodeRange2 = (UnicodeRange2)stream.ReadInt();
			unicodeRange3 = (UnicodeRange3)stream.ReadInt();
			unicodeRange4 = (UnicodeRange4)stream.ReadInt();
			vendor = stream.ReadString(4);
			selection = (Selection)stream.ReadShort();
			firstCharIndex = stream.ReadShort();
			lastCharIndex = stream.ReadShort();
			typoAscender = stream.ReadShort();
			typoDescender = stream.ReadShort();
			typoLineGap = stream.ReadShort();
			winAscent = stream.ReadShort();
			winDescent = stream.ReadShort();
			if (version > Version.TrueType_1_5) {
				codePageRange1 = (CodePageRange1)stream.ReadInt();
				codePageRange2 = (CodePageRange2)stream.ReadInt();
			}
			else {
				codePageRange1 = CodePageRange1.Empty;
				codePageRange2 = CodePageRange2.Empty;
			}
		}
		public PdfFontOS2TableDirectoryEntry(IFont font) : base(EntryTag) {
			version = Version.OpenType_1_5;
			avgCharWidth = CalcAverageGlyphWidth(font);
			PdfFontFlags flags;
			bool isBold;
			double ascender;
			double descender;
			double italicAngle;
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor == null) {
				flags = PdfFontFlags.None;
				isBold = false;
				weightClass = normalFontWeight;
				widthClass = WidthClass.Medium;
				ascender = 0;
				descender = 0;
				italicAngle = 0;
				xHeight = 0;
				capHeight = 0;
			}
			else {
				flags = fontDescriptor.Flags;
				isBold = (flags & PdfFontFlags.ForceBold) == PdfFontFlags.ForceBold;
				if (isBold)
					weightClass = boldFontWeight;
				else {
					weightClass = (short)fontDescriptor.FontWeight;
					isBold = weightClass == boldFontWeight;
				}
				switch (fontDescriptor.FontStretch) {
					case PdfFontStretch.Condensed:
						widthClass = WidthClass.Condensed;
						break;
					case PdfFontStretch.Expanded:
						widthClass = WidthClass.Expanded;
						break;
					case PdfFontStretch.ExtraCondensed:
						widthClass = WidthClass.ExtraCondensed;
						break;
					case PdfFontStretch.ExtraExpanded:
						widthClass = WidthClass.ExtraExpanded;
						break;
					case PdfFontStretch.SemiCondensed:
						widthClass = WidthClass.SemiCondensed;
						break;
					case PdfFontStretch.SemiExpanded:
						widthClass = WidthClass.SemiExpanded;
						break;
					case PdfFontStretch.UltraCondensed:
						widthClass = WidthClass.UltraCondensed;
						break;
					case PdfFontStretch.UltraExpanded:
						widthClass = WidthClass.UltraExpanded;
						break;
					default:
						widthClass = WidthClass.Medium;
						break;
				}
				ascender = fontDescriptor.ActualAscent;
				descender = fontDescriptor.ActualDescent;
				italicAngle = Math.Abs(fontDescriptor.ItalicAngle);
				xHeight = (short)fontDescriptor.XHeight;
				capHeight = (short)fontDescriptor.CapHeight;
			}
			embeddingType = EmbeddingType.PreviewPrintEmbedding;
			double em = ascender - descender;
			subscriptXSize = (short)(em / 5);
			subscriptYSize = subscriptXSize;
			subscriptXOffset = Convert.ToInt16(em * Math.Sin(italicAngle * Ratio));
			subscriptYOffset = 0;
			superscriptXSize = subscriptXSize;
			superscriptYSize = subscriptYSize;
			superscriptXOffset = subscriptXOffset;
			superscriptYOffset = (short)(ascender * 4 / 5);
			strikeoutSize = (short)(em / 10);
			strikeoutPosition = (short)(ascender / 2);
			familyClass = PdfFontFamilyClass.NoClassification;
			panose = new PdfPanose();
			unicodeRange1 = UnicodeRange1.Empty;
			unicodeRange2 = UnicodeRange2.Empty;
			unicodeRange3 = UnicodeRange3.Empty;
			unicodeRange4 = UnicodeRange4.Empty;
			vendor = "DX  ";
			selection = Selection.Empty;
			if ((flags & PdfFontFlags.Italic) == PdfFontFlags.Italic)
				selection |= Selection.ITALIC;
			if (isBold)
				selection |= Selection.BOLD;
			int firstChar = Int32.MaxValue;
			int lastChar = Int32.MinValue;
			foreach (int width in font.FontFileEncoding) {
				firstChar = Math.Min(width, firstChar);
				lastChar = Math.Max(width, lastChar);
			}
			firstCharIndex = (short)firstChar;
			lastCharIndex = (short)lastChar;
			typoAscender = Convert.ToInt16(ascender);
			typoDescender = Convert.ToInt16(descender);
			typoLineGap = Convert.ToInt16(TypoLineGapRatio * em);
			winAscent = typoAscender;
			winDescent = Convert.ToInt16(Math.Abs(descender));
			codePageRange1 = CodePageRange1.Latin1;
			codePageRange2 = CodePageRange2.Empty;
			shouldWrite = true;
		}
		public void Validate(IFont font, int unitsPerEm) {
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor != null) {
				PdfRectangle fontBBox = fontDescriptor.FontBBox;
				if (fontBBox != null) {
					double top = fontBBox.Top;
					double bottom = fontBBox.Bottom;
					double factor = unitsPerEm / (top - bottom);
					short newWinAscent = (short)(factor * top);
					if (newWinAscent > winAscent) {
						winAscent = newWinAscent;
						shouldWrite = true;
					}
					short newWinDescent = (short)(-factor * bottom);
					if (newWinDescent > winDescent) {
						winDescent = newWinDescent;
						shouldWrite = true;
					}
				}
			}
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream stream = CreateNewStream();
				stream.WriteShort((short)version);
				stream.WriteShort(avgCharWidth);
				stream.WriteShort(weightClass);
				stream.WriteShort((short)widthClass);
				stream.WriteShort((short)embeddingType);
				stream.WriteShort(subscriptXSize);
				stream.WriteShort(subscriptYSize);
				stream.WriteShort(subscriptXOffset);
				stream.WriteShort(subscriptYOffset);
				stream.WriteShort(superscriptXSize);
				stream.WriteShort(superscriptYSize);
				stream.WriteShort(superscriptXOffset);
				stream.WriteShort(superscriptYOffset);
				stream.WriteShort(strikeoutSize);
				stream.WriteShort(strikeoutPosition);
				stream.WriteShort((short)familyClass);
				panose.Write(stream);
				stream.WriteInt((int)unicodeRange1);
				stream.WriteInt((int)unicodeRange2);
				stream.WriteInt((int)unicodeRange3);
				stream.WriteInt((int)unicodeRange4);
				stream.WriteString(vendor);
				stream.WriteShort((short)selection);
				stream.WriteShort(firstCharIndex);
				stream.WriteShort(lastCharIndex);
				stream.WriteShort(typoAscender);
				stream.WriteShort(typoDescender);
				stream.WriteShort(typoLineGap);
				stream.WriteShort(winAscent);
				stream.WriteShort(winDescent);
				if (version >= Version.TrueType_1_66) {
					stream.WriteInt((int)codePageRange1);
					stream.WriteInt((int)codePageRange2);
					if (version >= Version.OpenType_1_2) {
						stream.WriteShort(xHeight);
						stream.WriteShort(capHeight);
						stream.WriteShort(defaultChar);
						stream.WriteShort(breakChar);
						stream.WriteShort(maxContext);
					}
				}
			}
		}
	}
}
