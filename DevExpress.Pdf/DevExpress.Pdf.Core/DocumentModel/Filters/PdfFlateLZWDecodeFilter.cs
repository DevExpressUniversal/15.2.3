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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfFlateLZWFilterPredictor { 
		NoPrediction = 1,
		TiffPredictor = 2,
		PngNonePrediction = 10,
		PngSubPrediction = 11,
		PngUpPrediction = 12,
		PngAveragePrediction = 13,
		PngPaethPrediction = 14,
		PngOptimumPrediction = 15
	}
	public abstract class PdfFlateLZWDecodeFilter : PdfFilter {
		const PdfFlateLZWFilterPredictor defaultPredictor = PdfFlateLZWFilterPredictor.NoPrediction;
		const int defaultColors = 1;
		const int defaultBitsPerComponent = 8;
		const int defaultColumns = 1;
		const string predictorDictionaryKey = "Predictor";
		const string colorsDictionaryKey = "Colors";
		const string bitsPerComponentDictionaryKey = "BitsPerComponent";
		const string columnsDictionaryKey = "Columns";
		readonly PdfFlateLZWFilterPredictor predictor;
		readonly int colors;
		readonly int bitsPerComponent;
		readonly int columns;
		public PdfFlateLZWFilterPredictor Predictor { get { return predictor; } }
		public int Colors { get { return colors; } }
		public int BitsPerComponent { get { return bitsPerComponent; } }
		public int Columns { get { return columns; } }
		protected PdfFlateLZWDecodeFilter(PdfReaderDictionary parameters) {
			if (parameters == null) {
				predictor = defaultPredictor;
				colors = defaultColors;
				bitsPerComponent = defaultBitsPerComponent;
				columns = defaultColumns;
			}
			else {
				int? pred = parameters.GetInteger(predictorDictionaryKey);
				predictor = pred.HasValue ? (PdfFlateLZWFilterPredictor)pred.Value : defaultPredictor;
				colors = parameters.GetInteger(colorsDictionaryKey) ?? defaultColors;
				bitsPerComponent = parameters.GetInteger(bitsPerComponentDictionaryKey) ?? defaultBitsPerComponent;
				columns = parameters.GetInteger(columnsDictionaryKey) ?? defaultColumns;
				bool validPredictor = false;
				foreach (object value in Enum.GetValues(typeof(PdfFlateLZWFilterPredictor)))
					if (value.Equals(predictor)) {
						validPredictor = true;
						break;
					}
				if (!validPredictor || colors < 1 || (bitsPerComponent != 1 && bitsPerComponent != 2 && bitsPerComponent != 4 && bitsPerComponent != 8 && bitsPerComponent != 16) || columns < 1)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		protected internal override PdfDictionary Write(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(predictorDictionaryKey, (int)predictor, (int)defaultPredictor);
			dictionary.Add(colorsDictionaryKey, colors, defaultColors);
			dictionary.Add(bitsPerComponentDictionaryKey, bitsPerComponent, defaultBitsPerComponent);
			dictionary.Add(columnsDictionaryKey, columns, defaultColumns);
			return dictionary.Count == 0 ? null : dictionary;
		}
		protected internal override byte[] Decode(byte[] data) {
			byte[] result = PerformDecode(data);
			return predictor == PdfFlateLZWFilterPredictor.NoPrediction ? 
				result : PdfPngPredictor.Decode(result, this);
		}
		protected abstract byte[] PerformDecode(byte[] data);
	}
}
