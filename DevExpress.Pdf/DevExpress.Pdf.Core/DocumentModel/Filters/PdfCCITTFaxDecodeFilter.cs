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
	public enum PdfCCITTFaxEncodingScheme { TwoDimensional, OneDimensional, Mixed };
	public class PdfCCITTFaxDecodeFilter : PdfFilter {
		internal const string Name = "CCITTFaxDecode";
		internal const string ShortName = "CCF";
		const int defaultColumns = 1728;
		const string encodingSchemeDictionaryKey = "K";
		const string endOfLineDictionaryKey = "EndOfLine";
		const string encodedByteAlignDictionaryKey = "EncodedByteAlign";
		const string columnsDictionaryKey = "Columns";
		const string rowsDictionaryKey = "Rows";
		const string endOfBlockDictionaryKey = "EndOfBlock";
		const string blackIs1DictionaryKey = "BlackIs1";
		const string damagedRowsBeforeErrorDictionaryKey = "DamagedRowsBeforeError";
		readonly PdfCCITTFaxEncodingScheme encodingScheme;
		readonly int twoDimensionalLineCount;
		readonly bool endOfLine;
		readonly bool encodedByteAlign;
		readonly int columns = defaultColumns;
		readonly int rows;
		readonly bool endOfBlock = true;
		readonly bool blackIs1;
		readonly int damagedRowsBeforeError;
		public PdfCCITTFaxEncodingScheme EncodingScheme { get { return encodingScheme; } }
		public int TwoDimensionalLineCount { get { return twoDimensionalLineCount; } }
		public bool EndOfLine { get { return endOfLine; } }
		public bool EncodedByteAlign { get { return encodedByteAlign; } }
		public int Columns { get { return columns; } }
		public int Rows { get { return rows; } }
		public bool EndOfBlock { get { return endOfBlock; } }
		public bool BlackIs1 { get { return blackIs1; } }
		public int DamagedRowsBeforeError { get { return damagedRowsBeforeError; } }
		protected internal override string FilterName { get { return Name; } }
		internal PdfCCITTFaxDecodeFilter(PdfReaderDictionary parameters) {
			if (parameters == null)
				encodingScheme = PdfCCITTFaxEncodingScheme.OneDimensional;
			else {
				int k = parameters.GetInteger(encodingSchemeDictionaryKey) ?? 0;
				if (k < 0)
					encodingScheme = PdfCCITTFaxEncodingScheme.TwoDimensional;
				else if (k == 0)
					encodingScheme = PdfCCITTFaxEncodingScheme.OneDimensional;
				else {
					encodingScheme = PdfCCITTFaxEncodingScheme.Mixed;
					twoDimensionalLineCount = k - 1;
				}
				endOfLine = parameters.GetBoolean(endOfLineDictionaryKey) ?? false;
				encodedByteAlign = parameters.GetBoolean(encodedByteAlignDictionaryKey) ?? false;
				columns = parameters.GetInteger(columnsDictionaryKey) ?? defaultColumns;
				rows = parameters.GetInteger(rowsDictionaryKey) ?? 0;
				endOfBlock = parameters.GetBoolean(endOfBlockDictionaryKey) ?? true;
				blackIs1 = parameters.GetBoolean(blackIs1DictionaryKey) ?? false;
				damagedRowsBeforeError = parameters.GetInteger(damagedRowsBeforeErrorDictionaryKey) ?? 0;
				if (columns <= 0 || rows < 0 || damagedRowsBeforeError < 0)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		protected internal override PdfDictionary Write(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			switch (encodingScheme) {
				case PdfCCITTFaxEncodingScheme.TwoDimensional:
					dictionary.Add(encodingSchemeDictionaryKey, -1);
					break;
				case PdfCCITTFaxEncodingScheme.Mixed:
					dictionary.Add(encodingSchemeDictionaryKey, twoDimensionalLineCount + 1);
					break;
			}
			dictionary.Add(endOfLineDictionaryKey, endOfLine, false);
			dictionary.Add(encodedByteAlignDictionaryKey, encodedByteAlign, false);
			dictionary.Add(columnsDictionaryKey, columns, defaultColumns);
			dictionary.Add(rowsDictionaryKey, rows, 0);
			dictionary.Add(endOfBlockDictionaryKey, endOfBlock, true);
			dictionary.Add(blackIs1DictionaryKey, blackIs1, false);
			dictionary.Add(damagedRowsBeforeErrorDictionaryKey, damagedRowsBeforeError, 0);
			return dictionary.Count == 0 ? null : dictionary;
		}
		protected internal override byte[] Decode(byte[] data) {
			return PdfCCITTFaxDecoder.Decode(this, data);
		}
	}
}
