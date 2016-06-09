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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfType1Font : PdfSimpleFont, IType1Font {
		internal const string Name = "Type1";
		internal const string TimesRomanFontName = "Times-Roman";
		internal const string TimesBoldFontName = "Times-Bold";
		internal const string TimesItalicFontName = "Times-Italic";
		internal const string TimesBoldItalicFontName = "Times-BoldItalic";
		internal const string HelveticaObliqueFontName = "Helvetica-Oblique";
		internal const string HelveticaBoldObliqueFontName = "Helvetica-BoldOblique";
		internal const string CourierFontName = "Courier";
		internal const string CourierBoldFontName = "Courier-Bold";
		internal const string CourierObliqueFontName = "Courier-Oblique";
		internal const string CourierBoldObliqueFontName = "Courier-BoldOblique";
		internal const string SymbolFontName = "Symbol";
		internal const string ZapfDingbatsFontName = "ZapfDingbats";
		const string fontFileDictionaryKey = "FontFile";
		const string length1DictionaryKey = "Length1";
		const string length2DictionaryKey = "Length2";
		const string length3DictionaryKey = "Length3";
		const string fontFileSubtype = "Type1C";
		static readonly Dictionary<string, short> symbolWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames.space, 250 }, { PdfGlyphNames.exclam, 333 }, { PdfGlyphNames.universal, 713 }, { PdfGlyphNames.numbersign, 500 }, { PdfGlyphNames.existential, 549 }, 
			{ PdfGlyphNames.percent, 833 }, { PdfGlyphNames.ampersand, 778 }, { PdfGlyphNames.suchthat, 439 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 }, 
			{ PdfGlyphNames.asteriskmath, 500 }, { PdfGlyphNames.plus, 549 }, { PdfGlyphNames.comma, 250 }, { PdfGlyphNames.minus, 549 }, { PdfGlyphNames.period, 250 }, 
			{ PdfGlyphNames.slash, 278 }, { PdfGlyphNames.zero, 500 }, { PdfGlyphNames.one, 500 }, { PdfGlyphNames.two, 500 }, { PdfGlyphNames.three, 500 }, 
			{ PdfGlyphNames.four, 500 }, { PdfGlyphNames.five, 500 }, { PdfGlyphNames.six, 500 }, { PdfGlyphNames.seven, 500 }, { PdfGlyphNames.eight, 500 }, 
			{ PdfGlyphNames.nine, 500 }, { PdfGlyphNames.colon, 278 }, { PdfGlyphNames.semicolon, 278 }, { PdfGlyphNames.less, 549 }, { PdfGlyphNames.equal, 549 }, 
			{ PdfGlyphNames.greater, 549 }, { PdfGlyphNames.question, 444 }, { PdfGlyphNames.congruent, 549 }, { PdfGlyphNames.Alpha, 722 }, { PdfGlyphNames.Beta, 667 }, 
			{ PdfGlyphNames.Chi, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.Epsilon, 611 }, { PdfGlyphNames.Phi, 763 }, { PdfGlyphNames.Gamma, 603 }, 
			{ PdfGlyphNames.Eta, 722 }, { PdfGlyphNames.Iota, 333 }, { PdfGlyphNames.theta1, 631 }, { PdfGlyphNames.Kappa, 722 }, { PdfGlyphNames.Lambda, 686 }, 
			{ PdfGlyphNames.Mu, 889 }, { PdfGlyphNames.Nu, 722 }, { PdfGlyphNames.Omicron, 722 }, { PdfGlyphNames.Pi, 768 }, { PdfGlyphNames.Theta, 741 }, 
			{ PdfGlyphNames.Rho, 556 }, { PdfGlyphNames.Sigma, 592 }, { PdfGlyphNames.Tau, 611 }, { PdfGlyphNames.Upsilon, 690 }, { PdfGlyphNames.sigma1, 439 }, 
			{ PdfGlyphNames.Omega, 768 }, { PdfGlyphNames.Xi, 645 }, { PdfGlyphNames.Psi, 795 }, { PdfGlyphNames.Zeta, 611 }, { PdfGlyphNames.bracketleft, 333 }, 
			{ PdfGlyphNames.therefore, 863 }, { PdfGlyphNames.bracketright, 333 }, { PdfGlyphNames.perpendicular, 658 }, { PdfGlyphNames.underscore, 500 }, { PdfGlyphNames.radicalex, 500 }, 
			{ PdfGlyphNames.alpha, 631 }, { PdfGlyphNames.beta, 549 }, { PdfGlyphNames.chi, 549 }, { PdfGlyphNames.delta, 494 }, { PdfGlyphNames.epsilon, 439 }, 
			{ PdfGlyphNames.phi, 521 }, { PdfGlyphNames.gamma, 411 }, { PdfGlyphNames.eta, 603 }, { PdfGlyphNames.iota, 329 }, { PdfGlyphNames.phi1, 603 }, 
			{ PdfGlyphNames.kappa, 549 }, { PdfGlyphNames.lambda, 549 }, { PdfGlyphNames.mu, 576 }, { PdfGlyphNames.nu, 521 }, { PdfGlyphNames.omicron, 549 }, 
			{ PdfGlyphNames.pi, 549 }, { PdfGlyphNames.theta, 521 }, { PdfGlyphNames.rho, 549 }, { PdfGlyphNames.sigma, 603 }, { PdfGlyphNames.tau, 439 }, 
			{ PdfGlyphNames.upsilon, 576 }, { PdfGlyphNames.omega1, 713 }, { PdfGlyphNames.omega, 686 }, { PdfGlyphNames.xi, 493 }, { PdfGlyphNames.psi, 686 }, 
			{ PdfGlyphNames.zeta, 494 }, { PdfGlyphNames.braceleft, 480 }, { PdfGlyphNames.bar, 200 }, { PdfGlyphNames.braceright, 480 }, { PdfGlyphNames.similar, 549 }, 
			{ PdfGlyphNames.Euro, 750 }, { PdfGlyphNames.Upsilon1, 620 }, { PdfGlyphNames.minute, 247 }, { PdfGlyphNames.lessequal, 549 }, { PdfGlyphNames.fraction, 167 }, 
			{ PdfGlyphNames.infinity, 713 }, { PdfGlyphNames.florin, 500 }, { PdfGlyphNames.club, 753 }, { PdfGlyphNames.diamond, 753 }, { PdfGlyphNames.heart, 753 }, 
			{ PdfGlyphNames.spade, 753 }, { PdfGlyphNames.arrowboth, 1042 }, { PdfGlyphNames.arrowleft, 987 }, { PdfGlyphNames.arrowup, 603 }, { PdfGlyphNames.arrowright, 987 }, 
			{ PdfGlyphNames.arrowdown, 603 }, { PdfGlyphNames.degree, 400 }, { PdfGlyphNames.plusminus, 549 }, { PdfGlyphNames.second, 411 }, { PdfGlyphNames.greaterequal, 549 }, 
			{ PdfGlyphNames.multiply, 549 }, { PdfGlyphNames.proportional, 713 }, { PdfGlyphNames.partialdiff, 494 }, { PdfGlyphNames.bullet, 460 }, { PdfGlyphNames.divide, 549 }, 
			{ PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.equivalence, 549 }, { PdfGlyphNames.approxequal, 549 }, { PdfGlyphNames.ellipsis, 1000 }, { PdfGlyphNames.arrowvertex, 603 }, 
			{ PdfGlyphNames.arrowhorizex, 1000 }, { PdfGlyphNames.carriagereturn, 658 }, { PdfGlyphNames.aleph, 823 }, { PdfGlyphNames.Ifraktur, 686 }, { PdfGlyphNames.Rfraktur, 795 }, 
			{ PdfGlyphNames.weierstrass, 987 }, { PdfGlyphNames.circlemultiply, 768 }, { PdfGlyphNames.circleplus, 768 }, { PdfGlyphNames.emptyset, 823 }, { PdfGlyphNames.intersection, 768 }, 
			{ PdfGlyphNames.union, 768 }, { PdfGlyphNames.propersuperset, 713 }, { PdfGlyphNames.reflexsuperset, 713 }, { PdfGlyphNames.notsubset, 713 }, { PdfGlyphNames.propersubset, 713 }, 
			{ PdfGlyphNames.reflexsubset, 713 }, { PdfGlyphNames.element, 713 }, { PdfGlyphNames.notelement, 713 }, { PdfGlyphNames.angle, 768 }, { PdfGlyphNames.gradient, 713 }, 
			{ PdfGlyphNames.registerserif, 790 }, { PdfGlyphNames.copyrightserif, 790 }, { PdfGlyphNames.trademarkserif, 890 }, { PdfGlyphNames.product, 823 }, { PdfGlyphNames.radical, 549 }, 
			{ PdfGlyphNames.dotmath, 250 }, { PdfGlyphNames.logicalnot, 713 }, { PdfGlyphNames.logicaland, 603 }, { PdfGlyphNames.logicalor, 603 }, { PdfGlyphNames.arrowdblboth, 1042 }, 
			{ PdfGlyphNames.arrowdblleft, 987 }, { PdfGlyphNames.arrowdblup, 603 }, { PdfGlyphNames.arrowdblright, 987 }, { PdfGlyphNames.arrowdbldown, 603 }, { PdfGlyphNames.lozenge, 494 }, 
			{ PdfGlyphNames.angleleft, 329 }, { PdfGlyphNames.registersans, 790 }, { PdfGlyphNames.copyrightsans, 790 }, { PdfGlyphNames.trademarksans, 786 }, { PdfGlyphNames.summation, 713 }, 
			{ PdfGlyphNames.parenlefttp, 384 }, { PdfGlyphNames.parenleftex, 384 }, { PdfGlyphNames.parenleftbt, 384 }, { PdfGlyphNames.bracketlefttp, 384 }, { PdfGlyphNames.bracketleftex, 384 }, 
			{ PdfGlyphNames.bracketleftbt, 384 }, { PdfGlyphNames.bracelefttp, 494 }, { PdfGlyphNames.braceleftmid, 494 }, { PdfGlyphNames.braceleftbt, 494 }, { PdfGlyphNames.braceex, 494 }, 
			{ PdfGlyphNames.angleright, 329 }, { PdfGlyphNames.integral, 274 }, { PdfGlyphNames.integraltp, 686 }, { PdfGlyphNames.integralex, 686 }, { PdfGlyphNames.integralbt, 686 }, 
			{ PdfGlyphNames.parenrighttp, 384 }, { PdfGlyphNames.parenrightex, 384 }, { PdfGlyphNames.parenrightbt, 384 }, { PdfGlyphNames.bracketrighttp, 384 }, { PdfGlyphNames.bracketrightex, 384 }, 
			{ PdfGlyphNames.bracketrightbt, 384 }, { PdfGlyphNames.bracerighttp, 494 }, { PdfGlyphNames.bracerightmid, 494 }, { PdfGlyphNames.bracerightbt, 494 }, { PdfGlyphNames.apple, 790 }	 
		};
		static readonly Dictionary<string, short> zapfDingbatsWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames.space, 278 }, { PdfGlyphNames.a1, 974 }, { PdfGlyphNames.a2, 961 }, { PdfGlyphNames.a202, 974 }, { PdfGlyphNames.a3, 980 }, 
			{ PdfGlyphNames.a4, 719 }, { PdfGlyphNames.a5, 789 }, { PdfGlyphNames.a119, 790 }, { PdfGlyphNames.a118, 791 }, { PdfGlyphNames.a117, 690 }, 
			{ PdfGlyphNames.a11, 960 }, { PdfGlyphNames.a12, 939 }, { PdfGlyphNames.a13, 549 }, { PdfGlyphNames.a14, 855 }, { PdfGlyphNames.a15, 911 }, 
			{ PdfGlyphNames.a16, 933 }, { PdfGlyphNames.a105, 911 }, { PdfGlyphNames.a17, 945 }, { PdfGlyphNames.a18, 974 }, { PdfGlyphNames.a19, 755 }, 
			{ PdfGlyphNames.a20, 846 }, { PdfGlyphNames.a21, 762 }, { PdfGlyphNames.a22, 761 }, { PdfGlyphNames.a23, 571 }, { PdfGlyphNames.a24, 677 }, 
			{ PdfGlyphNames.a25, 763 }, { PdfGlyphNames.a26, 760 }, { PdfGlyphNames.a27, 759 }, { PdfGlyphNames.a28, 754 }, { PdfGlyphNames.a6, 494 }, 
			{ PdfGlyphNames.a7, 552 }, { PdfGlyphNames.a8, 537 }, { PdfGlyphNames.a9, 577 }, { PdfGlyphNames.a10, 692 }, { PdfGlyphNames.a29, 786 }, 
			{ PdfGlyphNames.a30, 788 }, { PdfGlyphNames.a31, 788 }, { PdfGlyphNames.a32, 790 }, { PdfGlyphNames.a33, 793 }, { PdfGlyphNames.a34, 794 }, 
			{ PdfGlyphNames.a35, 816 }, { PdfGlyphNames.a36, 823 }, { PdfGlyphNames.a37, 789 }, { PdfGlyphNames.a38, 841 }, { PdfGlyphNames.a39, 823 }, 
			{ PdfGlyphNames.a40, 833 }, { PdfGlyphNames.a41, 816 }, { PdfGlyphNames.a42, 831 }, { PdfGlyphNames.a43, 923 }, { PdfGlyphNames.a44, 744 }, 
			{ PdfGlyphNames.a45, 723 }, { PdfGlyphNames.a46, 749 }, { PdfGlyphNames.a47, 790 }, { PdfGlyphNames.a48, 792 }, { PdfGlyphNames.a49, 695 }, 
			{ PdfGlyphNames.a50, 776 }, { PdfGlyphNames.a51, 768 }, { PdfGlyphNames.a52, 792 }, { PdfGlyphNames.a53, 759 }, { PdfGlyphNames.a54, 707 }, 
			{ PdfGlyphNames.a55, 708 }, { PdfGlyphNames.a56, 682 }, { PdfGlyphNames.a57, 701 }, { PdfGlyphNames.a58, 826 }, { PdfGlyphNames.a59, 815 }, 
			{ PdfGlyphNames.a60, 789 }, { PdfGlyphNames.a61, 789 }, { PdfGlyphNames.a62, 707 }, { PdfGlyphNames.a63, 687 }, { PdfGlyphNames.a64, 696 }, 
			{ PdfGlyphNames.a65, 689 }, { PdfGlyphNames.a66, 786 }, { PdfGlyphNames.a67, 787 }, { PdfGlyphNames.a68, 713 }, { PdfGlyphNames.a69, 791 }, 
			{ PdfGlyphNames.a70, 785 }, { PdfGlyphNames.a71, 791 }, { PdfGlyphNames.a72, 873 }, { PdfGlyphNames.a73, 761 }, { PdfGlyphNames.a74, 762 }, 
			{ PdfGlyphNames.a203, 762 }, { PdfGlyphNames.a75, 759 }, { PdfGlyphNames.a204, 759 }, { PdfGlyphNames.a76, 892 }, { PdfGlyphNames.a77, 892 }, 
			{ PdfGlyphNames.a78, 788 }, { PdfGlyphNames.a79, 784 }, { PdfGlyphNames.a81, 438 }, { PdfGlyphNames.a82, 138 }, { PdfGlyphNames.a83, 277 }, 
			{ PdfGlyphNames.a84, 415 }, { PdfGlyphNames.a97, 392 }, { PdfGlyphNames.a98, 392 }, { PdfGlyphNames.a99, 668 }, { PdfGlyphNames.a100, 668 }, 
			{ PdfGlyphNames.a89, 390 }, { PdfGlyphNames.a90, 390 }, { PdfGlyphNames.a93, 317 }, { PdfGlyphNames.a94, 317 }, { PdfGlyphNames.a91, 276 }, 
			{ PdfGlyphNames.a92, 276 }, { PdfGlyphNames.a205, 509 }, { PdfGlyphNames.a85, 509 }, { PdfGlyphNames.a206, 410 }, { PdfGlyphNames.a86, 410 }, 
			{ PdfGlyphNames.a87, 234 }, { PdfGlyphNames.a88, 234 }, { PdfGlyphNames.a95, 334 }, { PdfGlyphNames.a96, 334 }, { PdfGlyphNames.a101, 732 }, 
			{ PdfGlyphNames.a102, 544 }, { PdfGlyphNames.a103, 544 }, { PdfGlyphNames.a104, 910 }, { PdfGlyphNames.a106, 667 }, { PdfGlyphNames.a107, 760 }, 
			{ PdfGlyphNames.a108, 760 }, { PdfGlyphNames.a112, 776 }, { PdfGlyphNames.a111, 595 }, { PdfGlyphNames.a110, 694 }, { PdfGlyphNames.a109, 626 }, 
			{ PdfGlyphNames.a120, 788 }, { PdfGlyphNames.a121, 788 }, { PdfGlyphNames.a122, 788 }, { PdfGlyphNames.a123, 788 }, { PdfGlyphNames.a124, 788 }, 
			{ PdfGlyphNames.a125, 788 }, { PdfGlyphNames.a126, 788 }, { PdfGlyphNames.a127, 788 }, { PdfGlyphNames.a128, 788 }, { PdfGlyphNames.a129, 788 }, 
			{ PdfGlyphNames.a130, 788 }, { PdfGlyphNames.a131, 788 }, { PdfGlyphNames.a132, 788 }, { PdfGlyphNames.a133, 788 }, { PdfGlyphNames.a134, 788 }, 
			{ PdfGlyphNames.a135, 788 }, { PdfGlyphNames.a136, 788 }, { PdfGlyphNames.a137, 788 }, { PdfGlyphNames.a138, 788 }, { PdfGlyphNames.a139, 788 }, 
			{ PdfGlyphNames.a140, 788 }, { PdfGlyphNames.a141, 788 }, { PdfGlyphNames.a142, 788 }, { PdfGlyphNames.a143, 788 }, { PdfGlyphNames.a144, 788 }, 
			{ PdfGlyphNames.a145, 788 }, { PdfGlyphNames.a146, 788 }, { PdfGlyphNames.a147, 788 }, { PdfGlyphNames.a148, 788 }, { PdfGlyphNames.a149, 788 }, 
			{ PdfGlyphNames.a150, 788 }, { PdfGlyphNames.a151, 788 }, { PdfGlyphNames.a152, 788 }, { PdfGlyphNames.a153, 788 }, { PdfGlyphNames.a154, 788 }, 
			{ PdfGlyphNames.a155, 788 }, { PdfGlyphNames.a156, 788 }, { PdfGlyphNames.a157, 788 }, { PdfGlyphNames.a158, 788 }, { PdfGlyphNames.a159, 788 }, 
			{ PdfGlyphNames.a160, 894 }, { PdfGlyphNames.a161, 838 }, { PdfGlyphNames.a163, 1016 }, { PdfGlyphNames.a164, 458 }, { PdfGlyphNames.a196, 748 }, 
			{ PdfGlyphNames.a165, 924 }, { PdfGlyphNames.a192, 748 }, { PdfGlyphNames.a166, 918 }, { PdfGlyphNames.a167, 927 }, { PdfGlyphNames.a168, 928 }, 
			{ PdfGlyphNames.a169, 928 }, { PdfGlyphNames.a170, 834 }, { PdfGlyphNames.a171, 873 }, { PdfGlyphNames.a172, 828 }, { PdfGlyphNames.a173, 924 }, 
			{ PdfGlyphNames.a162, 924 }, { PdfGlyphNames.a174, 917 }, { PdfGlyphNames.a175, 930 }, { PdfGlyphNames.a176, 931 }, { PdfGlyphNames.a177, 463 }, 
			{ PdfGlyphNames.a178, 883 }, { PdfGlyphNames.a179, 836 }, { PdfGlyphNames.a193, 836 }, { PdfGlyphNames.a180, 867 }, { PdfGlyphNames.a199, 867 }, 
			{ PdfGlyphNames.a181, 696 }, { PdfGlyphNames.a200, 696 }, { PdfGlyphNames.a182, 874 }, { PdfGlyphNames.a201, 874 }, { PdfGlyphNames.a183, 760 }, 
			{ PdfGlyphNames.a184, 946 }, { PdfGlyphNames.a197, 771 }, { PdfGlyphNames.a185, 865 }, { PdfGlyphNames.a194, 771 }, { PdfGlyphNames.a198, 888 }, 
			{ PdfGlyphNames.a186, 967 }, { PdfGlyphNames.a195, 888 }, { PdfGlyphNames.a187, 831 }, { PdfGlyphNames.a188, 873 }, { PdfGlyphNames.a189, 927 }, 
			{ PdfGlyphNames.a190, 970 }, { PdfGlyphNames.a191, 918 }	 
		};
		internal static bool ReadFontData(IType1Font font, PdfReaderDictionary dictionary) {
			PdfReaderStream fontFile = dictionary.GetStream(fontFileDictionaryKey);
			if (fontFile == null)
				return false;
			PdfReaderDictionary fontFileDictionary = fontFile.Dictionary;
			int? length1 = fontFileDictionary.GetInteger(length1DictionaryKey);
			int? length2 = fontFileDictionary.GetInteger(length2DictionaryKey);
			int? length3 = fontFileDictionary.GetInteger(length3DictionaryKey);
			if (!length1.HasValue || !length2.HasValue || !length3.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			int plainTextLength = length1.Value;
			int cipherTextLength = length2.Value;
			int nullSegmentLength = length3.Value;
			byte[] fontFileData = fontFile.GetData(true);
			if (fontFileData.Length < plainTextLength + cipherTextLength + nullSegmentLength)
				PdfDocumentReader.ThrowIncorrectDataException();
			if (plainTextLength >= 2 && fontFileData[0] == 0x80 && fontFileData[1] == 1) {
				int newPlainTextLength = plainTextLength - 6;
				int newCipherTextLength = cipherTextLength - 6;
				int newNullSegmentLength = nullSegmentLength - 8;
				int newTextLength = newPlainTextLength + newCipherTextLength;
				byte[] newFontFileData = new byte[newTextLength + newNullSegmentLength];
				Array.Copy(fontFileData, 6, newFontFileData, 0, newPlainTextLength);
				Array.Copy(fontFileData, plainTextLength + 6, newFontFileData, newPlainTextLength, newCipherTextLength);
				Array.Copy(fontFileData, plainTextLength + cipherTextLength + 6, newFontFileData, newTextLength, newNullSegmentLength);
				plainTextLength = newPlainTextLength;
				cipherTextLength = newCipherTextLength;
				nullSegmentLength = newNullSegmentLength;
				fontFileData = newFontFileData;
			}
			font.FontFileData = fontFileData;
			font.PlainTextLength = plainTextLength;
			font.CipherTextLength = cipherTextLength;
			font.NullSegmentLength = nullSegmentLength;
			return true;
		}
		internal static bool WriteFontData(IType1Font font, PdfWriterDictionary dictionary) {
			byte[] fontFileData = font.FontFileData;
			if (fontFileData == null)
				return false;
			PdfDictionary streamDictionary = new PdfDictionary();
			streamDictionary.Add(length1DictionaryKey, font.PlainTextLength);
			streamDictionary.Add(length2DictionaryKey, font.CipherTextLength);
			streamDictionary.Add(length3DictionaryKey, font.NullSegmentLength);
			dictionary.Add(fontFileDictionaryKey, dictionary.Objects.AddStream(streamDictionary, fontFileData));
			return true;
		}
		readonly byte[] compactFontFileData;
		readonly byte[] openTypeFontFileData;
		readonly IDictionary<string, short> fontFileEncoding;
		readonly PdfMetadata metadata;
		readonly PdfFontDescriptor actualFontDescriptor;
		byte[] fontFileData;
		int plainTextLength;
		int cipherTextLength;
		int nullSegmentLength;
		public byte[] FontFileData { get { return fontFileData; } }
		public int PlainTextLength { get { return plainTextLength; } }
		public int CipherTextLength { get { return cipherTextLength; } }
		public int NullSegmentLength { get { return nullSegmentLength; } }
		public byte[] CompactFontFileData { get { return compactFontFileData; } }
		public byte[] OpenTypeFontFileData { get { return openTypeFontFileData; } }
		public PdfMetadata Metadata { get { return metadata; } }
		public override PdfFontDescriptor FontDescriptor { get { return actualFontDescriptor; } }
		protected override bool IsCourierFont { 
			get {
				string baseFont = BaseFont;
				return baseFont == CourierFontName || baseFont == CourierBoldFontName || baseFont == CourierObliqueFontName || baseFont == CourierBoldObliqueFontName || baseFont == CourierNewFontName;
			}
		}
		protected override IDictionary<string, short> DefaultWidthsDictionary {
			get {
				switch (BaseFont) {
					case TimesRomanFontName:
					case TimesNewRomanFontName:
						return TimesRomanWidths;
					case TimesBoldFontName:
					case TimesNewRomanBoldFontName:
						return TimesBoldWidths;
					case TimesItalicFontName:
						return TimesItalicWidths;
					case TimesBoldItalicFontName:
						return TimesBoldItalicWidths;
					case HelveticaFontName:
					case HelveticaObliqueFontName:
					case ArialFontName:
						return HelveticaWidths;
					case HelveticaBoldFontName:
					case HelveticaBoldObliqueFontName:
					case ArialBoldFontName:
						return HelveticaBoldWidths;
					case SymbolFontName:
						return symbolWidths;
					case ZapfDingbatsFontName:
						return zapfDingbatsWidths;
					default:
						return null;
				}
			}
		}
		protected override ICollection<short> FontFileEncoding { get { return fontFileEncoding.Values; } }
		protected internal override string Subtype { get { return Name; } }
		byte[] IType1Font.FontFileData { 
			get { return fontFileData; }
			set { fontFileData = value; }
		}
		int IType1Font.PlainTextLength { 
			get { return plainTextLength; }
			set { plainTextLength = value; }
		}
		int IType1Font.CipherTextLength { 
			get { return cipherTextLength; }
			set { cipherTextLength = value; }
		}
		int IType1Font.NullSegmentLength { 
			get { return nullSegmentLength; }
			set { nullSegmentLength = value; }
		}
		internal PdfType1Font(PdfObjectCollection objects, string baseFont, PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfSimpleFontEncoding encoding, int? firstChar, int? lastChar, double[] widths)
				: base(baseFont, toUnicode, fontDescriptor, encoding, firstChar, lastChar, widths) {
			if (fontDescriptor == null) {
				fontDescriptor =  PdfFontDescriptor.GetStandardFontDescriptor(objects, baseFont);
				if (fontDescriptor != null)
					actualFontDescriptor = CreateFontDescriptor(fontDescriptor);
			}
			else {
				actualFontDescriptor = base.FontDescriptor;
				if (!ReadFontData(this, fontDescriptor)) {
					PdfReaderStream fontFile3 = fontDescriptor.GetStream(FontFile3Key);
					openTypeFontFileData = GetOpenTypeFontFileData(fontFile3, true);
					if (openTypeFontFileData == null) {
						if (fontFile3 != null) {
							if (fontFile3.Dictionary.GetName(PdfDictionary.DictionarySubtypeKey) != fontFileSubtype)
								PdfDocumentReader.ThrowIncorrectDataException();
							compactFontFileData = fontFile3.GetData(true);
							fontFileEncoding = PdfCompactFontFormatParser.GetEncoding(compactFontFileData).Encoding;
						}
					}
				}
				metadata = fontDescriptor.GetMetadata();
			}
		}
		protected internal override void UpdateGlyphCodes(short[] str) {
			if (fontFileEncoding != null) {
				PdfSimpleFontEncoding encoding = Encoding;
				int length = str.Length;
				for (int i = 0; i < length; i++) {
					short glyphCode;
					str[i] = fontFileEncoding.TryGetValue(encoding.GetGlyphName((byte)str[i]), out glyphCode) ? glyphCode : (short)0;
				}
			}
		}
		protected internal override void UpdateFontDescriptorDictionary(PdfWriterDictionary dictionary) {
			if (!WriteOpenTypeFontData(dictionary, openTypeFontFileData) && !WriteFontData(this, dictionary) && compactFontFileData != null) {
				PdfObjectCollection objects = dictionary.Objects;
				PdfWriterDictionary streamDictionary = new PdfWriterDictionary(objects);
				streamDictionary.AddName(PdfDictionary.DictionarySubtypeKey, fontFileSubtype);
				dictionary.Add(FontFile3Key, objects.AddStream(streamDictionary, compactFontFileData));
			}
		}
	}
}
