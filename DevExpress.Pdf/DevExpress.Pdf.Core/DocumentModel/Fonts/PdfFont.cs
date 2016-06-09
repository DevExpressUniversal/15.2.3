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
	public abstract class PdfFont : PdfObject, IFont {
		internal const string DictionaryType = "Font";
		internal const string BaseFontKey = "BaseFont";
		internal const string FontDescriptorDictionaryKey = "FontDescriptor";
		protected const string ToUnicodeKey = "ToUnicode";
		protected const string FontFile3Key = "FontFile3";
		const string openTypeFontSubtype = "OpenType";
		const string encodingKey = "Encoding";
		const int subsetNameLength = 6;
		const int subsetPrefixLength = subsetNameLength + 1;
		internal static PdfFont CreateFont(PdfReaderDictionary fontDictionary) {
			string type = fontDictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string subtype = fontDictionary.GetName(PdfDictionary.DictionarySubtypeKey);
			string baseFont = fontDictionary.GetName(PdfFont.BaseFontKey);
			if (type == null && subtype == null && baseFont == null)
				return null;
			if ((type != null && type != DictionaryType) || (subtype != PdfType3Font.Name && String.IsNullOrEmpty(baseFont)))
				PdfDocumentReader.ThrowIncorrectDataException();
			return (subtype == null || subtype != PdfType0Font.Name) ? (PdfFont)PdfSimpleFont.Create(subtype, baseFont, fontDictionary) : (PdfFont)PdfType0Font.Create(baseFont, fontDictionary);
		}
		internal static object GetEncodingValue(PdfReaderDictionary dictionary) {
			object value;
			return dictionary.TryGetValue(encodingKey, out value) ? dictionary.Objects.TryResolve(value) : null;
		}
		readonly string baseFont;
		readonly string subsetName = String.Empty;
		readonly string fontName;
		readonly PdfFontDescriptor fontDescriptor;
		PdfCharacterMapping toUnicode;
		double averageWidth;
		protected PdfFontDescriptor RawFontDescriptor { get { return fontDescriptor; } }
		internal double WidthToHeightFactor { 
			get { 
				PdfFontDescriptor fontDescriptor = FontDescriptor;
				if (fontDescriptor == null)
					return 0;
				if (averageWidth == 0) {
					averageWidth = fontDescriptor.AvgWidth;
					if (averageWidth == 0) {
						double sum = 0.0;
						int count = 0;
						foreach (double width in ActualWidths)
							if (width > 0) {
								sum += width;
								count++;
							}
						if (count > 0)
							averageWidth = sum / count;
					}   
				}
				return averageWidth / fontDescriptor.Height;
			}
		}
		public string BaseFont { get { return baseFont; } }
		public string SubsetName { get { return subsetName; } }
		public string FontName { get { return fontName; } }
		public PdfCharacterMapping ToUnicode { 
			get { return toUnicode; } 
			internal set { toUnicode = value; } 
		}
		IEnumerable<double> IFont.GlyphWidths { get { return ActualWidths; } }
		byte[] IFont.CompactFontFileData { get { return ActualCompactFontFileData; } }
		ICollection<short> IFont.FontFileEncoding { get { return FontFileEncoding; } }
		public virtual PdfFontDescriptor FontDescriptor { get { return fontDescriptor; } }
		protected virtual byte[] ActualCompactFontFileData { get { return null; } }
		protected virtual ICollection<short> FontFileEncoding { get { return null; } }
		protected internal virtual bool HasSizeAttributes { get { return true; } }
		protected internal virtual bool UseGlyphIndices { get { return false; } }
		protected internal abstract string Subtype { get; }
		protected internal abstract PdfEncoding ActualEncoding { get; }
		protected abstract IEnumerable<double> ActualWidths { get; }
		protected PdfFont(string baseFont, PdfReaderStream toUnicodeStream, PdfReaderDictionary fontDescriptorDictionary) {
			this.baseFont = baseFont;
			if (baseFont.Length >= subsetPrefixLength && baseFont[subsetNameLength] == '+') {
				subsetName = baseFont.Substring(0, subsetNameLength);
				foreach (char c in subsetName)
					if (!Char.IsUpper(c)) {
						subsetName = String.Empty;
						break;
					}
			}
			fontName = String.IsNullOrEmpty(subsetName) ? baseFont : baseFont.Substring(subsetPrefixLength);
			if (fontDescriptorDictionary != null)
				fontDescriptor = CreateFontDescriptor(fontDescriptorDictionary);
			if (toUnicodeStream != null) 
				try {
					toUnicode = PdfCMapStreamParser.Parse(toUnicodeStream.GetData(true));
				}
				catch {
				}
		}
		protected byte[] GetOpenTypeFontFileData(PdfReaderStream stream, bool suppressException) {
			if (stream == null)
				return null;
			if (stream.Dictionary.GetName(PdfDictionary.DictionarySubtypeKey) != openTypeFontSubtype) {
				if (suppressException)
					return null;
				PdfDocumentReader.ThrowIncorrectDataException();
			}
			return stream.GetData(true);
		}
		protected byte[] GetOpenTypeFontFileData(PdfReaderDictionary dictionary, bool suppressException) {
			return GetOpenTypeFontFileData(dictionary.GetStream(FontFile3Key), suppressException);
		}
		protected bool WriteOpenTypeFontData(PdfWriterDictionary dictionary, byte[] openTypeFontData) {
			if (openTypeFontData == null)
				return false;
			PdfObjectCollection objects = dictionary.Objects;
			PdfWriterDictionary streamDictionary = new PdfWriterDictionary(objects);
			streamDictionary.AddName(PdfDictionary.DictionarySubtypeKey, openTypeFontSubtype);
			dictionary.Add(FontFile3Key, objects.AddStream(streamDictionary, openTypeFontData));
			return true;
		}
		protected virtual PdfFontDescriptor CreateFontDescriptor(PdfReaderDictionary dictionary) {
			return new PdfFontDescriptor(this, dictionary);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, DictionaryType);
			string subtype = Subtype;
			if (subtype != null)
				dictionary.AddName(PdfDictionary.DictionarySubtypeKey, subtype);
			dictionary.AddName(BaseFontKey, baseFont);
			object encodingValue = ActualEncoding.Write(objects);
			if (encodingValue != null)
				dictionary.Add(encodingKey, encodingValue);
			if (toUnicode != null)
				dictionary.Add(ToUnicodeKey, objects.AddStream(toUnicode.Data));
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
		protected internal virtual void UpdateGlyphCodes(short[] str) {
		}
		protected internal virtual void UpdateFontDescriptorDictionary(PdfWriterDictionary dictionary) {
		}
		protected internal abstract double[] GetGlyphPositions(PdfStringData stringData, double fontSizeFactor, double characterSpacing, double wordSpacing, double scalingFactor, double horizontalScalingFactor);
	}
}
