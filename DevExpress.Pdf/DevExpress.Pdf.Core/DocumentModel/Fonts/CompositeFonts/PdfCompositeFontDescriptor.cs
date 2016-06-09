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

using System.Globalization;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCompositeFontDescriptor : PdfFontDescriptor {
		const string styleDictionaryKey = "Style";
		const string fontDesctiptorsDictionaryKey = "FD";
		const string cidSetKey = "CIDSet";
		internal const string PanoseDictionaryKey = "Panose";
		readonly PdfFontFamilyClass fontFamilyClass;
		readonly PdfPanose panose;
		readonly CultureInfo languageCulture;
		readonly IDictionary<string, PdfFontDescriptor> fontDescriptors;
		readonly IDictionary<short, short> cidMapping;
		public PdfFontFamilyClass FontFamilyClass { get { return fontFamilyClass; } }
		public PdfPanose Panose { get { return panose; } }
		public CultureInfo LanguageCulture { get { return languageCulture; } }
		public IDictionary<string, PdfFontDescriptor> FontDescriptors { get { return fontDescriptors; } }
		public IDictionary<short, short> CIDMapping { get { return cidMapping; } }
		internal PdfCompositeFontDescriptor(PdfType0Font font, PdfReaderDictionary dictionary) : base(font, dictionary) {
			PdfReaderDictionary styleDictionary = dictionary.GetDictionary(styleDictionaryKey);
			if (styleDictionary != null) {
				byte[] panoseData = styleDictionary.GetBytes(PanoseDictionaryKey);
				if (panoseData.Length != 12)
					PdfDocumentReader.ThrowIncorrectDataException();
				using (PdfBinaryStream stream = new PdfBinaryStream(panoseData)) {
					fontFamilyClass = (PdfFontFamilyClass)stream.ReadShort();
					panose = new PdfPanose(stream);
				}
			}
			languageCulture = PdfReaderDictionary.ConvertToCultureInfo(dictionary.GetName(PdfDictionary.DictionaryLanguageKey));
			PdfReaderDictionary fd = dictionary.GetDictionary(fontDesctiptorsDictionaryKey);
			if (fd != null) {
				fontDescriptors = new Dictionary<string, PdfFontDescriptor>(fd.Count);
				PdfObjectCollection objects = dictionary.Objects;
				foreach (KeyValuePair<string, object> pair in fd) {
					PdfReaderDictionary descriptorDictionary = objects.TryResolve(pair.Value) as PdfReaderDictionary;
					if (descriptorDictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					fontDescriptors.Add(pair.Key, new PdfFontDescriptor(font, descriptorDictionary));
				}
			}
			PdfReaderStream cidSet = dictionary.GetStream(cidSetKey);
			if (cidSet != null) 
				try {
					byte[] data = cidSet.GetData(true);
					cidMapping = new Dictionary<short, short>();
					const byte mask = 0x80;
					short cid = 0;
					short code = 0;
					foreach (byte b in data) {
						byte current = b;
						for (int position = 0; position < 8; position++, code++, current <<= 1)
							if ((current & mask) != 0)
								cidMapping[code] = ++cid;
					}
				}
				catch {
				}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			if (fontFamilyClass != PdfFontFamilyClass.NoClassification || !panose.IsDefault)
				using (PdfBinaryStream stream = new PdfBinaryStream()) {
					stream.WriteShort((short)fontFamilyClass);
					panose.Write(stream);
					PdfDictionary styleDictionary = new PdfDictionary();
					styleDictionary.Add(PanoseDictionaryKey, stream.Data);
					dictionary.Add(styleDictionaryKey, styleDictionary);
				}
			if (languageCulture != CultureInfo.InvariantCulture)
				dictionary.AddName(PdfDictionary.DictionaryLanguageKey, languageCulture.Name);
			if (fontDescriptors != null) {
				PdfWriterDictionary fdDictionary = new PdfWriterDictionary(objects);
				foreach (KeyValuePair<string, PdfFontDescriptor> pair in fontDescriptors)
					fdDictionary.Add(pair.Key, pair.Value);
				dictionary.Add(fontDesctiptorsDictionaryKey, fdDictionary);
			}
			if (cidMapping != null) {
				List<byte> data = new List<byte>();
				short current = 0;
				byte b = 0;
				int size = 0;
				foreach (short code in cidMapping.Keys) {
					for (; current < code; current++) {
						b <<= 1;
						if (++size == 8) {
							data.Add(b);
							b = 0;
							size = 0;
						}
					}
					b = (byte)((b << 1) | 1);
					if (++size == 8) {
						data.Add(b);
						b = 0;
						size = 0;
					}
					current++;
				}
				for (; size < 8; size++)
					b <<= 1;
				data.Add(b);
				dictionary.Add(cidSetKey, objects.AddStream(data.ToArray()));
			}
			return dictionary;
		}
	}
}
