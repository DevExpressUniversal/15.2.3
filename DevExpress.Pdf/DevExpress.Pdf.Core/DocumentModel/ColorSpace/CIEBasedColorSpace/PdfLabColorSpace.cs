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
	public class PdfLabColorSpace : PdfCIEBasedColorSpace {
		internal const string TypeName = "Lab";
		const string rangeDictionaryKey = "Range";
		const double min = -100;
		const double max = 100;
		const double sixDivTwentyNine = 6.0 / 29.0;
		const double fourDivTwentyNine = 4.0 / 29.0;
		const double oneHundredEightDivEightHumdredFortyOne = 108.0 / 841.0;
		internal static PdfLabColorSpace Create(PdfObjectCollection collection, IList<object> array) {
			return new PdfLabColorSpace(ResolveColorSpaceDictionary(collection, array));
		}
		static double CorrectRange(PdfRange range, double value) {
			double min = range.Min;
			return (value - min) / (range.Max - min) * 200 - 100;
		}
		static double GammaFunction(double x) {
			return x >= sixDivTwentyNine ? (x * x * x) : ((x - fourDivTwentyNine) * oneHundredEightDivEightHumdredFortyOne);
		}
		readonly PdfRange rangeA;
		readonly PdfRange rangeB;
		public PdfRange RangeA { get { return rangeA; } }
		public PdfRange RangeB { get { return rangeB; } }
		public override int ComponentsCount { get { return 3; } }
		protected override string Name { get { return TypeName; } }
		internal PdfLabColorSpace(PdfReaderDictionary dictionary) : base(dictionary) {
			IList<object> range = dictionary.GetArray(rangeDictionaryKey);
			if (range == null) {
				rangeA = new PdfRange(min, max);
				rangeB = new PdfRange(min, max);
			}
			else {
				if (range.Count != 4)
					PdfDocumentReader.ThrowIncorrectDataException();
				double minA = PdfDocumentReader.ConvertToDouble(range[0]);
				double maxA = PdfDocumentReader.ConvertToDouble(range[1]);
				double minB = PdfDocumentReader.ConvertToDouble(range[2]);
				double maxB = PdfDocumentReader.ConvertToDouble(range[3]);
				if (maxA < minA || maxB < minB)
					PdfDocumentReader.ThrowIncorrectDataException();
				rangeA = new PdfRange(minA, maxA);
				rangeB = new PdfRange(minB, maxB);
			}
		}
		protected internal override PdfRange[] CreateDefaultDecodeArray(int bitsPerComponent) {
			return new PdfRange[] { new PdfRange(0, max), new PdfRange(rangeA.Min, rangeA.Max), new PdfRange(rangeB.Min, rangeB.Max) };
		}
		protected internal override PdfColor Transform(PdfColor color) {
			double[] components = color.Components;
			double m = (components[0] + 16) / 116;
			PdfCIEColor whitePoint = WhitePoint;
			double whitePointZ = whitePoint.Z;
			PdfCIEColor blackPoint = BlackPoint;
			double blackPointX = blackPoint.X;
			double blackPointY = blackPoint.Y;
			double blackPointZ = blackPoint.Z;
			return PdfColor.FromXYZ(blackPointX + (whitePoint.X - blackPointX) * GammaFunction(m + CorrectRange(RangeA, components[1]) / 500), 
				 blackPointY + (whitePoint.Y - blackPointY) * GammaFunction(m), blackPointZ + (whitePointZ - blackPointZ) * GammaFunction(m - CorrectRange(RangeB, components[2]) / 200), whitePointZ);
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			double rangeAMin = rangeA.Min;
			double rangeALength = rangeA.Max - rangeAMin;
			double rangeBMin = rangeB.Min;
			double rangeBLength = rangeB.Max - rangeBMin;
			int length = data.Length;
			byte[] result = new byte[length];
			for (int i = 0, src = 0, trg = 0; i < length / 3; i++)
				trg = FillResult(result, Transform(new PdfColor(data[src++] / 2.55, rangeAMin + data[src++] / 255.0 * rangeALength, rangeBMin + data[src++] / 255.0 * rangeBLength)).Components, trg);
			return new PdfColorSpaceTransformResult(result);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			double rangeAMin = rangeA.Min;
			double rangeAMax = rangeA.Max;
			double rangeBMin = rangeB.Min;
			double rangeBMax = rangeB.Max;
			if (rangeAMin != min || rangeAMax != max || rangeBMin != min || rangeBMax != max)
				dictionary.Add(rangeDictionaryKey, new double[] { rangeAMin, rangeAMax, rangeBMin, rangeBMax });
			return dictionary;
		}
	}
}
