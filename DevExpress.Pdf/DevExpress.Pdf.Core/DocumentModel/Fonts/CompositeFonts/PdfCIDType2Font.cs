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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCIDType2Font : PdfType0Font, ITrueTypeFont {
		const string length1Key = "Length1";
		byte[] openTypeFontFileData;
		byte[] fontFileData;
		byte[] patchedFontFileData;
		byte[] compactFontFileData;
		public byte[] OpenTypeFontFileData { get { return openTypeFontFileData; } }
		public byte[] FontFileData { get { return fontFileData; } }
		protected override string CIDSubType { get { return Subtype2Name; } }
		protected override byte[] ActualCompactFontFileData { get { return compactFontFileData; } }
		protected internal override bool UseGlyphIndices { get { return compactFontFileData == null || base.UseGlyphIndices; } }
		byte[] ITrueTypeFont.FontFileData { get { return patchedFontFileData; } }
		internal PdfCIDType2Font(string baseFont, PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfCompositeFontEncoding encoding, PdfReaderDictionary dictionary)
				: base(baseFont, toUnicode, fontDescriptor, encoding, dictionary) {
			PdfReaderStream fontFile2 = fontDescriptor.GetStream(PdfTrueTypeFont.FontFile2Key);
			if (fontFile2 == null) {
				openTypeFontFileData = GetOpenTypeFontFileData(fontDescriptor, false);
				PatchFontFileData(openTypeFontFileData);
			}
			else
				SetFontFileData(fontFile2.GetData(true));
		}
		internal PdfCIDType2Font(string baseFont, PdfReaderDictionary fontDescriptor) : base(baseFont, fontDescriptor, PdfIdentityEncoding.HorizontalIdentity) {
			PdfReaderStream stream = fontDescriptor.GetStream(PdfTrueTypeFont.FontFile2Key);
			fontFileData = stream == null ? null : stream.GetData(true);
			patchedFontFileData = fontFileData;
		}
		internal void SetFontFileData(byte[] data) {
			fontFileData = data;
			PatchFontFileData(fontFileData);
		}
		void PatchFontFileData(byte[] fontFileData) {
			if (fontFileData != null) {
				compactFontFileData = PdfFontFile.GetCFFData(fontFileData);
				if (compactFontFileData == null)
					patchedFontFileData = PdfTrueTypeFontValidator.Validate(this, fontFileData).Data;
				else 
					UpdateEncoding();
			}
		}
		protected internal override void UpdateFontDescriptorDictionary(PdfWriterDictionary dictionary) {
			if (fontFileData == null)
				WriteOpenTypeFontData(dictionary, openTypeFontFileData);
			else {
				PdfDictionary streamDictionary = new PdfDictionary();
				streamDictionary.Add(length1Key, fontFileData.Length);
				dictionary.Add(PdfTrueTypeFont.FontFile2Key, dictionary.Objects.AddStream(streamDictionary, fontFileData));
			}
		}
		protected override PdfWriterDictionary CreateDescendantDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDescendantDictionary(objects);
			if(CidToGidMap == null)
				dictionary.Add(CidToGIDMapKey, new PdfName("Identity"));
			return dictionary;
		}
	}
}
