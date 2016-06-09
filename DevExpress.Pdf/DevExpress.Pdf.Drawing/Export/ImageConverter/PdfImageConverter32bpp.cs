#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

namespace DevExpress.Pdf.Drawing {
	public class PdfImageConverter32bpp : PdfImageConverter {
		readonly byte[] sMask;
		bool hasMask;
		public override byte[] SMask { get { return sMask; } }
		public override bool HasMask { get { return hasMask; } }
		public PdfImageConverter32bpp(PdfImageConverterBitmapDataReader dataReader, int width, int height, int stride)
			: base(width, height, stride) {
			sMask = new byte[width * height + height];
			int imageRowLength = width * 4;
			byte[] imageData = ImageData;
			byte[] imageRow = new byte[imageRowLength];
			byte[] lastImageRow = new byte[width * 3];
			byte[] lastMaskRow = new byte[width];
			for (int y = 0, maskPos = 0, resultPosition = 0; y < height; y++) {
				imageData[resultPosition++] = pngUpPrediction;
				sMask[maskPos++] = pngUpPrediction;
				dataReader.ReadNextRow(imageRow , imageRowLength);
				for (int x = 0, xpos = 0, position = 0; x < width; x++) {
					byte red = imageRow[position++];
					byte green = imageRow[position++];
					byte blue = imageRow[position++];
					byte alpha = 255;
					alpha = imageRow[position++];
					if (alpha != 255) {
						hasMask = true;
					}
					sMask[maskPos++] = (byte)(alpha - lastMaskRow[x]);
					imageData[resultPosition++] = (byte)(blue - lastImageRow[xpos]);
					imageData[resultPosition++] = (byte)(green - lastImageRow[xpos + 1]);
					imageData[resultPosition++] = (byte)(red - lastImageRow[xpos + 2]);
					lastImageRow[xpos++] = blue;
					lastImageRow[xpos++] = green;
					lastImageRow[xpos++] = red;
					lastMaskRow[x] = alpha;
				}
			}
		}
	}
}
