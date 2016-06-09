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
	public class PdfPngPredictor {
		enum PngPrediction {
			None = 0,
			Sub = 1,
			Up = 2,
			Average = 3,
			Paeth = 4
		}
		public static byte[] Decode(byte[] data, PdfFlateLZWDecodeFilter filter) {
			return new PdfPngPredictor(data, filter).Decode();
		}
		readonly byte[] data;
		readonly int dataLength;
		readonly PdfFlateLZWFilterPredictor predictor;
		readonly int colors;
		readonly int bitsFactor;
		readonly int rowLengthInBytes;
		readonly bool rowIsIncomplete;
		readonly int rowsCount;
		readonly int rowSize;
		readonly byte[] actualData;
		readonly byte[] previousRow;
		readonly byte[] previousColors;
		readonly byte[] topLeftColors;
		PdfPngPredictor(byte[] data, PdfFlateLZWDecodeFilter filter) {
			this.data = data;
			dataLength = data.Length;
			int bitsPerComponent = filter.BitsPerComponent;
			if (bitsPerComponent == 16)
				PdfDocumentReader.ThrowIncorrectDataException();
			predictor = filter.Predictor;
			colors = filter.Colors;
			bitsFactor = 8 / bitsPerComponent;
			rowLengthInBytes = filter.Columns * colors;
			rowSize = rowLengthInBytes / bitsFactor;
			rowIsIncomplete = rowLengthInBytes % bitsFactor != 0;
			if (rowIsIncomplete)
				rowSize++;
			int sourceColumns = rowSize + 1;
			rowsCount = dataLength / sourceColumns;
			if (dataLength % sourceColumns != 0)
				rowsCount++;
			actualData = new byte[rowSize * rowsCount];
			previousRow = new byte[rowLengthInBytes];
			previousColors = new byte[colors];
			topLeftColors = new byte[colors];
		}
		byte[] Decode() {
			for (int row = 0, sourceIndex = 0, resultIndex = 0; row < rowsCount; row++) {
				Array.Clear(previousColors, 0, colors);
				Array.Clear(topLeftColors, 0, colors);
				PngPrediction pngPredicion = (PngPrediction)(data[sourceIndex++]);
				for (int component = 0, colorIndex = 0; component < rowSize; component++) {
					if (sourceIndex == dataLength)
						break;
					byte dataElement = data[sourceIndex++];
					byte actualDataElement;
					switch (predictor) {
						case PdfFlateLZWFilterPredictor.TiffPredictor:
							return new byte[0];
						case PdfFlateLZWFilterPredictor.PngUpPrediction:
							if (pngPredicion != PngPrediction.Up)
								PdfDocumentReader.ThrowIncorrectDataException();
							actualDataElement = (byte)(previousRow[component] + dataElement);
							break;
						case PdfFlateLZWFilterPredictor.PngNonePrediction:
						case PdfFlateLZWFilterPredictor.PngOptimumPrediction:
							switch (pngPredicion) {
								case PngPrediction.None:
									actualDataElement = dataElement;
									break;
								case PngPrediction.Sub:
									actualDataElement = (byte)(previousColors[colorIndex] + dataElement);
									break;
								case PngPrediction.Up:
									actualDataElement = (byte)(previousRow[component] + dataElement);
									break;
								case PngPrediction.Average:
									actualDataElement = (byte)((previousColors[colorIndex] + previousRow[component]) / 2 + dataElement);
									break;
								case PngPrediction.Paeth:
									byte left = previousColors[colorIndex];
									byte top = previousRow[component];
									byte topLeft = topLeftColors[colorIndex];
									int prediction = left + top - topLeft;
									int dLeft = Math.Abs(prediction - left);
									int dTop = Math.Abs(prediction - top);
									int dTopLeft = Math.Abs(prediction - topLeft);
									byte choose;
									if (dLeft <= dTop)
										choose = dLeft <= dTopLeft ? left : topLeft;
									else
										choose = dTop <= dTopLeft ? top : topLeft;
									actualDataElement = (byte)(choose + dataElement);
									break;
								default:
									PdfDocumentReader.ThrowIncorrectDataException();
									actualDataElement = dataElement;
									break;
							}
							break;
						default:
							PdfDocumentReader.ThrowIncorrectDataException();
							actualDataElement = dataElement;
							break;
					}
					actualData[resultIndex++] = (byte)(actualDataElement);
					topLeftColors[colorIndex] = previousRow[component];
					previousRow[component] = actualDataElement;
					previousColors[colorIndex] = actualDataElement;
					if (++colorIndex == colors)
						colorIndex = 0;
				}
			}
			return actualData;
		}
	}
}
