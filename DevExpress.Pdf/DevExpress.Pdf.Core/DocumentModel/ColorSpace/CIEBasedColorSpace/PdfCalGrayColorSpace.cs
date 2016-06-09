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
	public class PdfCalGrayColorSpace : PdfCIEBasedColorSpace {
		internal const string TypeName = "CalGray";
		const double oneThird = 1.0 / 3.0;
		internal static PdfCalGrayColorSpace Create(PdfObjectCollection collection, IList<object> array) {
			return new PdfCalGrayColorSpace(ResolveColorSpaceDictionary(collection, array));
		}
		readonly double gamma;
		public double Gamma { get { return gamma; } }
		public override int ComponentsCount { get { return 1; } }
		protected override string Name { get { return TypeName; } }
		PdfCalGrayColorSpace(PdfReaderDictionary dictionary) : base(dictionary) {
			gamma = dictionary.GetNumber(GammaDictionaryKey) ?? PdfGamma.Default;
			if (gamma <= 0)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override PdfColor Transform(PdfColor color) {
			double y = BlackPoint.Y;
			double luminosity = PdfMathUtils.Max(116 * Math.Pow(y + (WhitePoint.Y - y) * Math.Pow(color.Components[0], gamma), oneThird) - 16, 0) / 100.0;
			return new PdfColor(luminosity, luminosity, luminosity);
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			PdfColorSpaceTransformResult transformResult = new PdfGrayColorSpaceTransformation(width, height, bitsPerComponent, colorKeyMask).Transform(data);
			if (transformResult == null)
				return null;
			data = transformResult.Data;
			byte[] result;
			if (transformResult.PixelFormat == PdfPixelFormat.Gray1bit) {
				double[] whiteComponents = Transform(new PdfColor(1)).Components;
				double[] blackComponents = Transform(new PdfColor(0)).Components;
				result = new byte[width * height * 3];
				for (int y = 0, src = 0, trg = 0; y < height; y++) {
					byte b = 0;
					for (int x = 0, mask = 0; x < width; x++) {
						if (mask == 0) {
							b = data[src++];
							mask = 128;
						}
						trg = FillResult(result, (b & mask) == 0 ? blackComponents : whiteComponents, trg);
						mask = mask >> 1;
					}
				}
			}
			else {
				int length = data.Length;
				result = new byte[length * 3];
				for (int i = 0, position = 0; i < length; i++)
					position = FillResult(result, Transform(new PdfColor(data[i] / 255.0)).Components, position);
			}
			return new PdfColorSpaceTransformResult(result);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(GammaDictionaryKey, gamma, PdfGamma.Default);
			return dictionary;
		}
	}
}
