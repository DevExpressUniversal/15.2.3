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
namespace DevExpress.Pdf.Native {
	public abstract class PdfColorSpaceTransformation {
		readonly int imageWidth;
		readonly int imageHeight;
		readonly int bitsPerComponent;
		readonly IList<PdfRange> colorKeyMask;
		protected int ImageWidth { get { return imageWidth; } }
		protected int ImageHeight { get { return imageHeight; } }
		protected int BitsPerComponent { get { return bitsPerComponent; } }
		protected IList<PdfRange> ColorKeyMask { get { return colorKeyMask; } }
		protected abstract int ComponentsCount { get; }
		protected PdfColorSpaceTransformation(int imageWidth, int imageHeight, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			this.imageWidth = imageWidth;
			this.imageHeight = imageHeight;
			this.bitsPerComponent = bitsPerComponent;
			this.colorKeyMask = colorKeyMask;
		}
		public abstract PdfColorSpaceTransformResult Transform(byte[] data);
		protected byte[] UnpackData(byte[] data) {
			int componentsCount = ComponentsCount;
			int unpackedDataSize = imageWidth * imageHeight * componentsCount;
			if (bitsPerComponent == 8 || data.Length * 8 / bitsPerComponent < unpackedDataSize)
				return data;
			byte[] unpackedData = new byte[unpackedDataSize];
			byte mask = (byte)(0xff >> (8 - bitsPerComponent));
			int startShift = 8 - bitsPerComponent;
			double factor = 255.0 / mask;
			for (int y = 0, src = 0, trg = 0; y < imageHeight; y++, src++) {
				for (int x = 0, shift = startShift; x < imageWidth; x++) {
					for (int component = 0; component < componentsCount; component++) {
						if (shift < 0) {
							shift = startShift;
							src++;
						}
						unpackedData[trg++] = PdfMathUtils.ToByte(((data[src] >> shift) & mask) * factor);
						shift -= bitsPerComponent;
					}
				}
			}
			return unpackedData;
		}
	}
}
