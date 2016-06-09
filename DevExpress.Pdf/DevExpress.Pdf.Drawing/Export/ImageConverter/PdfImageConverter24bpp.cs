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
	public class PdfImageConverter24bpp : PdfImageConverter {
		public PdfImageConverter24bpp(PdfImageConverterBitmapDataReader dataReader, int width, int height, int stride)
			: base(width, height, stride) {
			byte[] imageData = ImageData;
			int imageRowLength = width * 3;
			byte[] imageRow = new byte[imageRowLength];
			byte[] lastImageRow = new byte[imageRowLength];
			for (int y = 0, resultPosition = 0; y < height; y++) {
				imageData[resultPosition++] = pngUpPrediction;
				dataReader.ReadNextRow(imageRow, imageRowLength);
				for (int x = 0, xpos = 0, position = 0; x < width; x++) {
					byte red = imageRow[position++];
					byte green = imageRow[position++];
					byte blue = imageRow[position++];
					imageData[resultPosition++] = (byte)(blue - lastImageRow[xpos]);
					imageData[resultPosition++] = (byte)(green - lastImageRow[xpos + 1]);
					imageData[resultPosition++] = (byte)(red - lastImageRow[xpos + 2]);
					lastImageRow[xpos++] = blue;
					lastImageRow[xpos++] = green;
					lastImageRow[xpos++] = red;
				}
			}
		}
	}
}
