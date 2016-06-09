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
	public class PdfCIDType0Font : PdfType0Font, IType1Font {
		const string fontFileSubtype = "CIDFontType0C";
		readonly byte[] compactFontFileData;
		readonly byte[] openTypeFontFileData;
		byte[] fontFileData;
		int plainTextLength;
		int cipherTextLength;
		int nullSegmentLength;
		int firstChar;
		int lastChar;
		double[] widths;
		public byte[] CompactFontFileData { get { return compactFontFileData; } }
		public byte[] OpenTypeFontFileData { get { return openTypeFontFileData; } }
		public byte[] FontFileData { get { return fontFileData; } }
		public int PlainTextLength { get { return plainTextLength; } }
		public int CipherTextLength { get { return cipherTextLength; } }
		public int NullSegmentLength { get { return nullSegmentLength; } }
		protected override string CIDSubType { get { return Subtype0Name; } }
		protected override byte[] ActualCompactFontFileData { get { return compactFontFileData; } }
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
		int IType1Font.FirstChar {
			get {
				EnsureWidths();
				return firstChar; 
			}
		}
		int IType1Font.LastChar {
			get {
				EnsureWidths();
				return lastChar;
			}
		}
		double[] IType1Font.Widths {
			get {
				EnsureWidths();
				return widths; 
			}
		}
		internal PdfCIDType0Font(string baseFont, PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfCompositeFontEncoding encoding, PdfReaderDictionary dictionary)
				: base(baseFont, toUnicode, fontDescriptor, encoding, dictionary) {
			PdfReaderStream stream = fontDescriptor.GetStream(FontFile3Key);
			openTypeFontFileData = GetOpenTypeFontFileData(stream, true);
			if (openTypeFontFileData == null)
				if (stream == null) {
					if (!(ActualEncoding is PdfPredefinedCompositeFontEncoding) && !PdfType1Font.ReadFontData(this, fontDescriptor))
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else {
					if (stream.Dictionary.GetName(PdfDictionary.DictionarySubtypeKey) != fontFileSubtype)
						PdfDocumentReader.ThrowIncorrectDataException();
					compactFontFileData = stream.GetData(true);
					int length = compactFontFileData.Length;
					if (length != 0) {
						if (compactFontFileData[0] != 1) {
							compactFontFileData = PdfFontFile.GetCFFData(compactFontFileData);
							if (compactFontFileData == null)
								PdfDocumentReader.ThrowIncorrectDataException();
						}
						UpdateEncoding();
					}
				}
		}
		void EnsureWidths() {
			if (widths == null) {
				IDictionary<int, double> actualWidths = Widths;
				ICollection<int> charCodes = actualWidths.Keys;
				int charCodesCount = charCodes.Count;
				if (charCodesCount > 0) {
					int defaultWidth = DefaultWidth;
					List<int> actualCharCodes = new List<int>(charCodes);
					firstChar = actualCharCodes[0];
					lastChar = actualCharCodes[charCodesCount - 1];
					int widthsLength = lastChar - firstChar + 1;
					widths = new double[widthsLength];
					for (int i = 0, charCode = firstChar; charCode <= lastChar; i++, charCode++) {
						double width;
						widths[i] = actualWidths.TryGetValue(charCode, out width) ? width : defaultWidth;
					}
				}
				else {
					firstChar = 0;
					lastChar = 0;
					widths = new double[1];
				}
			}
		}
		protected internal override void UpdateFontDescriptorDictionary(PdfWriterDictionary dictionary) {
			base.UpdateFontDescriptorDictionary(dictionary);
			if (compactFontFileData == null) {
				if (!PdfType1Font.WriteFontData(this, dictionary))
					WriteOpenTypeFontData(dictionary, openTypeFontFileData);
			}
			else {
				PdfObjectCollection objects = dictionary.Objects;
				PdfWriterDictionary streamDictionary = new PdfWriterDictionary(objects);
				streamDictionary.AddName(PdfDictionary.DictionarySubtypeKey, fontFileSubtype);
				dictionary.Add(FontFile3Key, objects.AddStream(streamDictionary, compactFontFileData));
			}
		}
	}
}
