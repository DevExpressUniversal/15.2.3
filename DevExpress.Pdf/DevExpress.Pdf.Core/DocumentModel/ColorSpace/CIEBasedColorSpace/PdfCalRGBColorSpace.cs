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
	public class PdfCalRGBColorSpace : PdfCIEBasedColorSpace {
		internal const string TypeName = "CalRGB";
		const string matrixDictionaryKey = "Matrix";
		internal static PdfCalRGBColorSpace Create(PdfObjectCollection collection, IList<object> array) {
			return new PdfCalRGBColorSpace(ResolveColorSpaceDictionary(collection, array));
		}
		static double ColorComponentTransferFunction(double component) {
			return component > 0.04045 ? Math.Pow((component + 0.055) / 1.055, 2.4) : (component / 12.92);
		}
		readonly PdfGamma gamma;
		readonly PdfColorSpaceMatrix matrix;
		public PdfGamma Gamma { get { return gamma; } }
		public PdfColorSpaceMatrix Matrix { get { return matrix; } }
		public override int ComponentsCount { get { return 3; } }
		protected override string Name { get { return TypeName; } }
		PdfCalRGBColorSpace(PdfReaderDictionary dictionary) : base(dictionary) {
			IList<object> gammaArray = dictionary.GetArray(GammaDictionaryKey);
			gamma = gammaArray == null ? new PdfGamma() : new PdfGamma(gammaArray);
			IList<object> matrixArray = dictionary.GetArray(matrixDictionaryKey);
			matrix = matrixArray == null ? new PdfColorSpaceMatrix() : new PdfColorSpaceMatrix(matrixArray);
		}
		protected internal override PdfColor Transform(PdfColor color) {
			double[] components = color.Components;
			double red = Math.Pow(components[0], gamma.Red);
			double green = Math.Pow(components[1], gamma.Green);
			double blue = Math.Pow(components[2], gamma.Blue);
			PdfCIEColor whitePoint = WhitePoint;
			double whitePointZ = whitePoint.Z;
			double x, y, z;
			if (matrix.IsIdentity) {
				red = ColorComponentTransferFunction(red);
				green = ColorComponentTransferFunction(green);
				blue = ColorComponentTransferFunction(blue);
				PdfCIEColor blackPoint = BlackPoint;
				double blackPointX = blackPoint.X;
				double blackPointY = blackPoint.Y;
				double blackPointZ = blackPoint.Z;
				x = PdfColor.ClipColorComponent((red * 0.4124 + green * 0.3576 + blue * 0.1805 - blackPointX) / (whitePoint.X - blackPointX));
				y = PdfColor.ClipColorComponent((red * 0.2126 + green * 0.7152 + blue * 0.0722 - blackPointY) / (whitePoint.Y - blackPointY));
				z = PdfColor.ClipColorComponent((red * 0.0193 + green * 0.1192 + blue * 0.9505 - blackPointZ) / (whitePointZ - blackPointZ));
			}
			else {
				x = matrix.Xa * red + matrix.Xb * green + matrix.Xc * blue;
				y = matrix.Ya * red + matrix.Yb * green + matrix.Yc * blue;
				z = matrix.Za * red + matrix.Zb * green + matrix.Zc * blue;
			}
			return PdfColor.FromXYZ(x, y, z, whitePointZ);
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			PdfColorSpaceTransformResult transformationResult = new PdfRGBColorSpaceTransformation(width, height, bitsPerComponent, colorKeyMask).Transform(data);
			data = transformationResult.Data;
			int length = data.Length;
			byte[] result = new byte[length];
			length /= 3;
			for (int i = 0, src = 0, trg = 0; i < length; i++)
				trg = FillResult(result, Transform(new PdfColor(data[src++] / 255.0, data[src++] / 255.0, data[src++] / 255.0)).Components, trg);
			return new PdfColorSpaceTransformResult(result, transformationResult.MaskData);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			if (!gamma.IsDefault)
				dictionary.Add(GammaDictionaryKey, gamma.ToArray());
			if (!matrix.IsIdentity)
				dictionary.Add(matrixDictionaryKey, matrix.ToArray());
			return dictionary;
		}
	}
}
