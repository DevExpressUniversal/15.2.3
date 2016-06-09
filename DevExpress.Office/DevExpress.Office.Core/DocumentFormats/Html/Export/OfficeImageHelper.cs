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

using System;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Export.Html {
	public class OfficeHtmlImageHelper {
		readonly IDocumentModel documentModel;
		readonly DocumentModelUnitConverter unitConverter;
		public OfficeHtmlImageHelper(IDocumentModel documentModel, DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.documentModel = documentModel;
			this.unitConverter = unitConverter;
			CalculateDefaultImageResolution();
		}
		public float HorizontalResolution { get; set; }
		public float VerticalResolution { get; set; }
		bool ShouldConvertImage(OfficeImage image) {
#if !SL && !DXPORTABLE
			switch (image.RawFormat) {
				default:
					return false;
				case OfficeImageFormat.Wmf:
				case OfficeImageFormat.Emf:
				case OfficeImageFormat.Exif:
				case OfficeImageFormat.Icon:
				case OfficeImageFormat.MemoryBmp:
				case OfficeImageFormat.None:
					return true;
			}
#else
			return false;
#endif
		}
		public void ApplyImageProperties(WebImageControl imageControl, OfficeImage image, Size actualSize, IOfficeImageRepository imageRepository, bool disposeConvertedImagesImmediately) {
			ApplyImageProperties(imageControl, image, actualSize, imageRepository, disposeConvertedImagesImmediately, true);
		}
		public void ApplyImageProperties(WebImageControl imageControl, OfficeImage image, Size actualSize, IOfficeImageRepository imageRepository, bool disposeConvertedImagesImmediately, bool correctWidth) {
			ApplyImageProperties(imageControl, image, actualSize, imageRepository, disposeConvertedImagesImmediately, correctWidth, false);
		}
		public void ApplyImageProperties(WebImageControl imageControl, OfficeImage image, Size actualSize, IOfficeImageRepository imageRepository, bool disposeConvertedImagesImmediately, bool correctWidth, bool alwaysWriteImageSize) {
			ApplyImageProperties(imageControl, image, actualSize, imageRepository, disposeConvertedImagesImmediately, correctWidth, alwaysWriteImageSize, false);
		}
		public void ApplyImageProperties(WebImageControl imageControl, OfficeImage image, Size actualSize, IOfficeImageRepository imageRepository, bool disposeConvertedImagesImmediately, bool correctWidth, bool alwaysWriteImageSize, bool keepImageSize) {
			if (ShouldConvertImage(image)) {
				if (keepImageSize) {					 
					imageControl.ImageUrl = imageRepository.GetImageSource(documentModel.CreateImage(new Bitmap(image.NativeImage)), disposeConvertedImagesImmediately);
					alwaysWriteImageSize = true;
				}
				else
					imageControl.ImageUrl = imageRepository.GetImageSource(GetHtmlImage(image, actualSize, correctWidth), disposeConvertedImagesImmediately);
			}
			else {
				imageControl.ImageUrl = imageRepository.GetImageSource(image, false);
				alwaysWriteImageSize = true;
			}
			if (alwaysWriteImageSize) {
				imageControl.Attributes.Add("width", unitConverter.ModelUnitsToPixels(actualSize.Width, HorizontalResolution).ToString());
				imageControl.Attributes.Add("height", unitConverter.ModelUnitsToPixels(actualSize.Height, VerticalResolution).ToString());
			}
		}
#if !SL && !DXPORTABLE
		OfficeImage GetHtmlImage(OfficeImage image, Size actualSize, bool correctWidth) {
			int finalWidth = unitConverter.ModelUnitsToPixels(actualSize.Width, HorizontalResolution);
			int finalHeight = unitConverter.ModelUnitsToPixels(actualSize.Height, VerticalResolution);
			OfficeImage result = CreateBitmapFromSourceImage(image, finalWidth, finalHeight, correctWidth);
			result.Uri = image.Uri;
			return result;
		}
		OfficeImage CreateBitmapFromSourceImage(OfficeImage image, int width, int height, bool correctWidth) {
			const int gdiPlusIssueWidthCorrectionValue = 1;
			Bitmap bitmap = new Bitmap(Math.Max(1, correctWidth ? width - gdiPlusIssueWidthCorrectionValue : width), Math.Max(1, height));
			using (Graphics gr = Graphics.FromImage(bitmap)) {
				gr.Clear(DXColor.Transparent);
				gr.DrawImage(image.NativeImage, 0, 0, width, height);
			}
			return documentModel.CreateImage(bitmap);
		}
		void CalculateDefaultImageResolution() {
			using (Bitmap image = new Bitmap(1, 1)) {
				HorizontalResolution = image.HorizontalResolution;
				VerticalResolution = image.VerticalResolution;
			}
		}
#else
		public OfficeImage GetHtmlImage(OfficeImage image, Size actualSize, bool correctWidth) {
			return null;
		}
		void CalculateDefaultImageResolution() {
			HorizontalResolution = 96.0f;
			VerticalResolution = 96.0f;
		}
#endif
	}
}
