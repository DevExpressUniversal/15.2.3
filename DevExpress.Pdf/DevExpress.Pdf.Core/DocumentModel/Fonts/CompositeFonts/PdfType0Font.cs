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
	public abstract class PdfType0Font : PdfFont {
		internal const string Name = "Type0";
		internal const string CidSystemInfoDictionaryKey = "CIDSystemInfo";
		internal const string WidthsDictionaryKey = "W";
		internal const string Subtype2Name = "CIDFontType2";
		protected const string Subtype0Name = "CIDFontType0";
		protected const string CidToGIDMapKey = "CIDToGIDMap";
		const string descendantFontsDictionaryKey = "DescendantFonts";
		const string defaultWidthDictionaryKey = "DW";
		static int ConvertToInt(object value) {
			if (!(value is int))
				PdfDocumentReader.ThrowIncorrectDataException();
			return (int)value;
		}
		internal static PdfType0Font Create(string baseFont, PdfReaderDictionary dictionary) {
			PdfReaderStream toUnicode = null;
			object toUnicodeValue;
			if (dictionary.TryGetValue(ToUnicodeKey, out toUnicodeValue)) {
				PdfName name = toUnicodeValue as PdfName;
				if (name == null)
					toUnicode = dictionary.GetStream(ToUnicodeKey);
				else {
					string actualName = name.Name;
					if (actualName != PdfIdentityEncoding.HorizontalIdentityName && actualName != PdfIdentityEncoding.VerticalIdentityName)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
			}
			IList<object> descendantFonts = dictionary.GetArray(descendantFontsDictionaryKey);
			if (descendantFonts == null || descendantFonts.Count != 1)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfReaderDictionary cidDictionary = descendantFonts[0] as PdfReaderDictionary;
			if (cidDictionary == null) {
				PdfObjectReference reference = descendantFonts[0] as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				cidDictionary = dictionary.Objects.GetDictionary(reference.Number);
				if (cidDictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			string cidFontType = cidDictionary.GetName(PdfDictionary.DictionarySubtypeKey);
			PdfReaderDictionary cidFontDescriptor = cidDictionary.GetDictionary(FontDescriptorDictionaryKey);
			if (cidDictionary.GetName(PdfDictionary.DictionaryTypeKey) != "Font" || String.IsNullOrEmpty(cidFontType) || cidFontDescriptor == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfCompositeFontEncoding encoding = PdfCompositeFontEncoding.Create(GetEncodingValue(dictionary));
			switch (cidFontType) {
				case Subtype0Name:
					return new PdfCIDType0Font(baseFont, toUnicode, cidFontDescriptor, encoding, cidDictionary);
				case Subtype2Name:
					return new PdfCIDType2Font(baseFont, toUnicode, cidFontDescriptor, encoding, cidDictionary);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		readonly PdfCompositeFontEncoding encoding;
		readonly string cidBaseFont;
		readonly PdfCIDSystemInfo systemInfo;
		readonly int defaultWidth;
		readonly short[] cidToGidMap;
		IDictionary<int, double> widths = new SortedDictionary<int, double>();
		bool isIdentityEncoding;
		ICollection<short> fontFileEncoding;
		public PdfCompositeFontEncoding Encoding { get { return encoding; } }
		public string CIDBaseFont { get { return cidBaseFont; } }
		public PdfCIDSystemInfo SystemInfo { get { return systemInfo; } }
		public int DefaultWidth { get { return defaultWidth; } }
		public IDictionary<int, double> Widths { 
			get { return widths; }
			internal set { widths = value; }
		}
		public short[] CidToGidMap { get { return cidToGidMap; } }
		protected internal override PdfEncoding ActualEncoding { get { return encoding; } }
		protected internal override bool UseGlyphIndices { get { return !isIdentityEncoding; } }
		protected internal override string Subtype { get { return PdfType0Font.Name; } }
		protected override IEnumerable<double> ActualWidths { get { return Widths.Values; } }
		protected override ICollection<short> FontFileEncoding { get { return fontFileEncoding; } }
		protected abstract string CIDSubType { get; }
		protected PdfType0Font(string baseFont, PdfReaderDictionary fontDescriptor, PdfCompositeFontEncoding encoding) : base(baseFont, null, fontDescriptor) {
			this.encoding = encoding;
			cidBaseFont = baseFont;
			systemInfo = new PdfCIDSystemInfo("Adobe", "Identity", 0);
			defaultWidth = 1000;
		}
		protected PdfType0Font(string baseFont, PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfCompositeFontEncoding encoding, PdfReaderDictionary dictionary) 
				: base(baseFont, toUnicode, fontDescriptor) {
			this.encoding = encoding;
			cidBaseFont = dictionary.GetName(BaseFontKey);
			PdfReaderDictionary systemInfoDictionary = dictionary.GetDictionary(CidSystemInfoDictionaryKey);
			if (String.IsNullOrEmpty(cidBaseFont) || systemInfoDictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			systemInfo = new PdfCIDSystemInfo(systemInfoDictionary);
			defaultWidth = dictionary.GetInteger(defaultWidthDictionaryKey) ?? 1000;
			IList<object> widthsArray = dictionary.GetArray(WidthsDictionaryKey);
			if (widthsArray != null) {
				PdfObjectCollection objects = dictionary.Objects;
				int startIndex = -1;
				int endIndex = -1;
				foreach (object value in widthsArray) {
					if (startIndex < 0)
						startIndex = ConvertToInt(value);
					else if (endIndex < 0)
						if (value is int)
							endIndex = (int)value;
						else {
							IList<object> widthsRange = objects.TryResolve(value) as IList<object>;
							if (widthsRange == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							foreach (object width in widthsRange)
								AddWidth(startIndex++, PdfDocumentReader.ConvertToDouble(width));
							startIndex = -1;
						}
					else {
						double width = PdfDocumentReader.ConvertToDouble(value);
						for (int i = startIndex; i <= endIndex; i++)
							AddWidth(i, width);
						startIndex = -1;
						endIndex = -1;
					}
				}
				if (startIndex >= 0)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			object cidToGIDMapValue;
			if (dictionary.TryGetValue(CidToGIDMapKey, out cidToGIDMapValue)) {
				PdfName name = cidToGIDMapValue as PdfName;
				if (name == null) {
					PdfObjectReference reference = cidToGIDMapValue as PdfObjectReference;
					if (reference != null) {
						PdfReaderStream stream = dictionary.Objects.GetStream(reference.Number);
						if (stream == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						byte[] data = stream.GetData(true);
						int length = data.Length;
						if (length % 2 > 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						int mappingSize = length / 2;
						cidToGidMap = new short[mappingSize];
						for (int i = 0, j = 0; i < mappingSize; i++) {
							int gid = data[j++] << 8;
							cidToGidMap[i] = (short)(gid + data[j++]);
						}
					}
				}
				else if (name.Name != "Identity")
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		protected void UpdateEncoding() {
			byte[] compactFontFileData = ActualCompactFontFileData;
			if (compactFontFileData != null) {
				PdfCompactFontFormatEncoding cffEncoding = PdfCompactFontFormatParser.GetEncoding(compactFontFileData);
				isIdentityEncoding = cffEncoding.IsIdentityEncoding;
				fontFileEncoding = cffEncoding.Encoding.Values;
			}
		}
		void AddWidth(int key, double width) {
			if (!widths.ContainsKey(key))
				widths.Add(key, width);
		}
		protected internal override void UpdateGlyphCodes(short[] str) {
			base.UpdateGlyphCodes(str);
			if (cidToGidMap != null) {
				int mapLength = cidToGidMap.Length;
				int length = str.Length;
				for (int i = 0; i < length; i++) {
					ushort cid = (ushort)str[i];
					str[i] = cid >= mapLength ? (short)0 : cidToGidMap[cid];
				}
			}
		}
		protected internal override double[] GetGlyphPositions(PdfStringData stringData, double fontSizeFactor, double characterSpacing, double wordSpacing, double scalingFactor, double horizontalScalingFactor) {
			byte[][] charCodes = stringData.CharCodes;
			short[] str = stringData.Str;
			double[] glyphOffsets = stringData.Offsets;
			int stringLength = str.Length;
			double[] positions = new double[stringLength];
			double tx = 0;
			for (int i = 0, j = 1; i < stringLength; i++, j++) {
				int actualCharCode;
				byte[] charCode = charCodes[i];
				int charCodeLength = charCode.Length;
				if (charCodeLength > 0) {
					actualCharCode = charCode[0];
					for (int index = 1; index < charCodeLength; index++)
						actualCharCode = charCode[index] + (actualCharCode << 8);
				}
				else
					actualCharCode = 0;
				short chr = str[i];
				double w0 = 0;
				if (!widths.TryGetValue(chr, out w0))
					w0 = defaultWidth;
				tx += ((w0 - glyphOffsets[j]) * fontSizeFactor * horizontalScalingFactor + characterSpacing + (actualCharCode == 32 ? wordSpacing : 0)) * scalingFactor;
				positions[i] = tx;
			}
			return positions;
		}
		protected override PdfFontDescriptor CreateFontDescriptor(PdfReaderDictionary dictionary) {
			return new PdfCompositeFontDescriptor(this, dictionary);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(descendantFontsDictionaryKey, new object[] { objects.AddDictionary(CreateDescendantDictionary(objects)) });
			return dictionary;
		}
		protected virtual PdfWriterDictionary CreateDescendantDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary descedantFontDictionary = new PdfWriterDictionary(objects);
			descedantFontDictionary.AddName(PdfDictionary.DictionaryTypeKey, DictionaryType);
			descedantFontDictionary.AddName(PdfDictionary.DictionarySubtypeKey, CIDSubType);
			descedantFontDictionary.AddName(BaseFontKey, cidBaseFont);
			descedantFontDictionary.Add(FontDescriptorDictionaryKey, FontDescriptor);
			descedantFontDictionary.Add(CidSystemInfoDictionaryKey, systemInfo);
			descedantFontDictionary.Add(defaultWidthDictionaryKey, defaultWidth);
			if (widths != null && widths.Count > 0) {
				List<object> widthsArray = new List<object>();
				int index = -1;
				List<object> array = new List<object>();
				foreach (KeyValuePair<int, double> w in widths) {
					if (index < 0) {
						index = w.Key;
						widthsArray.Add(index);
						array.Add(w.Value);
					}
					else if (w.Key == ++index)
						array.Add(w.Value);
					else {
						widthsArray.Add(array);
						index = w.Key;
						widthsArray.Add(index);
						array = new List<object>();
						array.Add(w.Value);
					}
				}
				widthsArray.Add(array);
				descedantFontDictionary.Add(WidthsDictionaryKey, widthsArray);
			}
			if (cidToGidMap != null) {
				int length = cidToGidMap.Length;
				byte[] data = new byte[length * 2];
				for (int i = 0; i < length; i++) {
					int index = i * 2;
					short value = cidToGidMap[i];
					data[index] = (byte)(value >> 8);
					data[index + 1] = (byte)(value % 256);
				}
				descedantFontDictionary.Add(CidToGIDMapKey, objects.AddStream(data));
			}
			return descedantFontDictionary;
		}
	}
}
