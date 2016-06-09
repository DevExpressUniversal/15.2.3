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

using System.Drawing;
using System.Security;
using System.Drawing.Imaging;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfImageConverter {
		const PixelFormat transparentPixelFormatMask = PixelFormat.Alpha | PixelFormat.Indexed;
		protected const byte pngUpPrediction = 2;
		[SecuritySafeCritical]
		public static PdfImageConverter Create(Image image) {
			int width = image.Width;
			int height = image.Height;
			Bitmap bmp = (Bitmap)image;
			bool format24 = bmp.PixelFormat == PixelFormat.Format24bppRgb;
			bool isCloned = false;
			try {
				if (!format24) {
					if ((bmp.PixelFormat & transparentPixelFormatMask) != 0)
						bmp = bmp.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);
					else {
						bmp = bmp.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format24bppRgb);
						format24 = true;
					}
					isCloned = true;
				}
				BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bmp.PixelFormat);
				int stride = bitmapData.Stride;
				PdfImageConverterBitmapDataReader stream = new PdfImageConverterBitmapDataReader(bitmapData);
				PdfImageConverter converter;
				if (format24)
					converter = new PdfImageConverter24bpp(stream, width, height, stride);
				else
					converter = new PdfImageConverter32bpp(stream, width, height, stride);
				bmp.UnlockBits(bitmapData);
				return converter;
			}
			finally {
				if (isCloned)
					bmp.Dispose();
			}
		}
		readonly byte[] imageData;
		public byte[] ImageData { get { return imageData; } }
		public virtual byte[] SMask { get { return null; } }
		public virtual bool HasMask { get { return false; } }
		protected PdfImageConverter(int width, int height, int stride) {
			imageData = new byte[width * height * 3 + height];
		}
	}
}
