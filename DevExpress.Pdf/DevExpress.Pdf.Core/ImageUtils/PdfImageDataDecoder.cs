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
	public partial class PdfImageDataDecoder {
		public static byte[] Decode(PdfImage image) {
			PdfDecodedImageData imageData = image.DecodeData();
			return imageData == null ? null : new PdfImageDataDecoder(image, imageData).Decode();
		}
		readonly int imageWidth;
		readonly int imageHeight;
		readonly PdfColorSpace colorSpace;
		readonly byte[] data;
		readonly PdfImageDataType dataType;
		PdfImageDataDecoder(PdfImage image, PdfDecodedImageData imageData) {
			imageWidth = image.Width;
			imageHeight = image.Height;
			colorSpace = image.ColorSpace;
			data = imageData.Data;
			dataType = imageData.DataType;
		}
		byte[] Decode() {
			switch (dataType) {
				case PdfImageDataType.JPXDecode:
					return null;
				case PdfImageDataType.DCTDecode:
					byte[] actualData = data;
					int dataLength = data.Length;
					for (int i = 0; i < dataLength; i++)
						if (!PdfDocumentParser.IsSpaceSymbol(data[i])) {
							if (i != 0) {
								int actualLength = dataLength - i;
								actualData = new byte[actualLength];
								Array.Copy(data, i, actualData, 0, actualLength);
							}
							break;
						}
					PdfDCTDecodeResult decodeResult = PdfDCTDecoder.Decode(actualData, imageWidth, imageHeight);
					byte[] decodedData = decodeResult.Data;
					int componentsCount = colorSpace == null ? 1 : colorSpace.ComponentsCount;
					byte[] result = new byte[imageWidth * imageHeight * componentsCount];
					int actualStride = decodeResult.Stride;
					int lastComponentIndex = componentsCount - 1;
					byte[] components = new byte[componentsCount];
					for (int y = 0, sourcePosition = 0, trg = 0; y < imageHeight; y++, sourcePosition += actualStride)
						for (int x = 0, src = sourcePosition; x < imageWidth; x++) {
							if (componentsCount == 4)
								for (int i = 0; i < 4; i++)
									result[trg++] = (byte)(255 - decodedData[src++]);
							else {
								for (int i = 0; i < componentsCount; i++)
									components[i] = decodedData[src++];
								for (int i = lastComponentIndex; i >= 0; i--)
									result[trg++] = components[i];
							}
						}
					return result;
			}
			return data;
		}
	}
}
