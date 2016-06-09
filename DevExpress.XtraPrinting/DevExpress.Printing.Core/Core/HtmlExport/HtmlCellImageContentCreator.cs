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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export.Web {
	public class HtmlCellImageContentCreator {
		[ThreadStatic]
		static Image blankGif;
		static Image BlankGif { get { return blankGif != null ? blankGif : blankGif = ResourceImageHelper.CreateImageFromResources(NativeSR.BlankFileName, typeof(DevExpress.Printing.ResFinder)); } }
		protected IImageRepository imageRepository;
		public HtmlCellImageContentCreator(IImageRepository imageRepository) {
			this.imageRepository = imageRepository;
		}
		public void CreateContent(DXHtmlContainerControl cell, Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imgSize, PaddingInfo padding) {
			Guard.ArgumentNotNull(cell, "cell");
			if(!ValidateImageSrc(image, ref imageSrc))
				return;
			PaddingInfo pixPadding = new PaddingInfo(padding, GraphicsDpi.DeviceIndependentPixel);
			cell.Style.Add(DXHtmlTextWriterStyle.TextAlign, "left");
			cell.Style.Add(DXHtmlTextWriterStyle.VerticalAlign, "top");
			Size htmlImageSize = GetHtmlImageSize(sizeMode, bounds, pixPadding, imgSize);
			DXWebControlBase imageControl = CreateHtmlImage(image, htmlImageSize, imageSrc);
			Rectangle clipBounds = GetClipBounds(sizeMode, align, pixPadding, bounds);
			ClipControl clipControl = HtmlHelper.SetClip(cell, GetImagePosition(sizeMode, align, pixPadding, bounds, htmlImageSize), clipBounds.Size);
			ProcessImage(clipControl.InnerContainer, imageControl, image);
		}
		protected virtual void ProcessImage(DXWebControlBase imgContainer, DXWebControlBase imageControl, Image image) {
			if(!(imageControl is DXHtmlImage) || !IsPng(image)) {
				imgContainer.Controls.Add(imageControl);
				return;
			}
			DXHtmlImage htmlImageIE6 = CloneHtmlImage((DXHtmlImage)imageControl);
			htmlImageIE6.Style.Add("filter", string.Concat("progid:DXImageTransform.Microsoft.AlphaImageLoader(src='", ((DXHtmlImage)imageControl).Src, "',sizingMethod='scale')"));
			htmlImageIE6.Src = GetBlankGifSrc();
			imgContainer.Controls.Add(new DXHtmlLiteralControl("<!--[if lt IE 7]>"));
			imgContainer.Controls.Add(htmlImageIE6);
			imgContainer.Controls.Add(new DXHtmlLiteralControl("<div style=\"display:none\"><![endif]-->"));
			imgContainer.Controls.Add(imageControl);
			imgContainer.Controls.Add(new DXHtmlLiteralControl("<!--[if lt IE 7]></div><![endif]-->"));
		}
		static DXHtmlImage CloneHtmlImage(DXHtmlImage source) {
			DXHtmlImage newImg = new DXHtmlImage();
			foreach(string key in source.Attributes.Keys)
				newImg.Attributes.Add(key, source.Attributes[key]);
			foreach(string key in source.Style.Keys)
				newImg.Style.Add(key, source.Style[key]);
			newImg.Src = source.Src;
			return newImg;
		}
		protected string GetBlankGifSrc() {
			return imageRepository.GetImageSource(BlankGif, false);
		}
		protected static bool IsPng(Image image) {
			return image != null && HtmlImageHelper.GetMimeType(image) == "png";
		}
		protected internal virtual string GetWatermarkImageSrc(Image image) {
			return imageRepository.GetImageSource(image, false);
		}
		protected virtual bool ValidateImageSrc(Image image, ref string imageSrc) {
			if(string.IsNullOrEmpty(imageSrc))
				imageSrc = imageRepository.GetImageSource(image, false);
			return !string.IsNullOrEmpty(imageSrc);
		}
		Rectangle GetClipBounds(ImageSizeMode sizeMode, ImageAlignment align, PaddingInfo pixPadding, Rectangle bounds) {
			if((sizeMode == ImageSizeMode.Normal && align != ImageAlignment.MiddleCenter) || sizeMode == ImageSizeMode.AutoSize) {   
				bounds.Width -= pixPadding.Right;
				bounds.Height -= pixPadding.Bottom;
			}
			return bounds;
		}
		Size GetHtmlImageSize(ImageSizeMode sizeMode, Rectangle bounds, PaddingInfo pixPadding, Size imgSize) {
			if(imgSize == Size.Empty)
				return Size.Empty;
			Size htmlImageSize = imgSize;
			Rectangle boundsWithPadding = Rectangle.Round(pixPadding.Deflate(bounds, GraphicsDpi.DeviceIndependentPixel));
			if(sizeMode == ImageSizeMode.StretchImage || sizeMode == ImageSizeMode.AutoSize || sizeMode == ImageSizeMode.Tile) {
				htmlImageSize.Height = boundsWithPadding.Size.Height;
				htmlImageSize.Width = boundsWithPadding.Size.Width;
			} else if(sizeMode == ImageSizeMode.ZoomImage ||
				(sizeMode == ImageSizeMode.Squeeze &&
				(htmlImageSize.Width > boundsWithPadding.Width || htmlImageSize.Height > boundsWithPadding.Height))) {
				htmlImageSize = MathMethods.ZoomInto(boundsWithPadding.Size, imgSize).ToSize();
			}
			return htmlImageSize;
		}
		PaddingInfo AdjustPadding(PaddingInfo pixPadding, Rectangle bounds, Size imageSize, ImageSizeMode sizeMode) {
			if(sizeMode == ImageSizeMode.ZoomImage ||
				(sizeMode == ImageSizeMode.Squeeze &&
				(imageSize.Width > bounds.Width || imageSize.Height > bounds.Height))) {
				pixPadding.Left = pixPadding.Left + (bounds.Width - (pixPadding.Left + pixPadding.Right + imageSize.Width)) / 2;
				pixPadding.Top = pixPadding.Top + (bounds.Height - (pixPadding.Top + pixPadding.Bottom + imageSize.Height)) / 2;
			}
			return pixPadding;
		}
		protected virtual DXWebControlBase CreateHtmlImage(Image image, Size htmlImageSize, string imageSrc) {
			DXHtmlImage imageControl = new DXHtmlImage();
			SetCointrolSize(imageControl, htmlImageSize);
			imageControl.Attributes.Add("alt", string.Empty);
			imageControl.Src = imageSrc;
			return imageControl;
		}
		static protected void SetCointrolSize(DXHtmlGenericControl control, Size htmlSize) {
			control.Style.Add("width", GetSizeString(htmlSize.Width));
			control.Style.Add("height", GetSizeString(htmlSize.Height));
		}
		static string GetSizeString(int size) {
			return new DXWebUnit(size, DXWebUnitType.Pixel).ToString();
		}
		Point GetImagePosition(ImageSizeMode sizeMode, ImageAlignment align, PaddingInfo pixPadding, Rectangle bounds, Size htmlImageSize) {
			switch(sizeMode) {
				case ImageSizeMode.StretchImage:
				case ImageSizeMode.AutoSize:
				case ImageSizeMode.Tile:
					return new Point(pixPadding.Left, pixPadding.Top);
				case ImageSizeMode.CenterImage:
					return new Point(((bounds.Width - htmlImageSize.Width - pixPadding.Right + pixPadding.Left) / 2), ((bounds.Height - htmlImageSize.Height - pixPadding.Bottom + pixPadding.Top) / 2));
				case ImageSizeMode.ZoomImage:
				case ImageSizeMode.Normal:
				case ImageSizeMode.Squeeze:
					switch(align) {
						case ImageAlignment.TopLeft:
							return new Point(pixPadding.Left, pixPadding.Top);
						case ImageAlignment.TopCenter:
							return new Point(((bounds.Width - htmlImageSize.Width - pixPadding.Right + pixPadding.Left) / 2), pixPadding.Top);
						case ImageAlignment.TopRight:
							return new Point((bounds.Width - htmlImageSize.Width - pixPadding.Right), pixPadding.Top);
						case ImageAlignment.MiddleLeft:
							return new Point(pixPadding.Left, ((bounds.Height - htmlImageSize.Height - pixPadding.Bottom + pixPadding.Top) / 2));
						case ImageAlignment.MiddleCenter:
							return new Point(((bounds.Width - htmlImageSize.Width - pixPadding.Right + pixPadding.Left) / 2), ((bounds.Height - htmlImageSize.Height - pixPadding.Bottom + pixPadding.Top) / 2));
						case ImageAlignment.MiddleRight:
							return new Point((bounds.Width - htmlImageSize.Width - pixPadding.Right), ((bounds.Height - htmlImageSize.Height - pixPadding.Bottom + pixPadding.Top) / 2));
						case ImageAlignment.BottomLeft:
							return new Point(pixPadding.Left, (bounds.Height - htmlImageSize.Height - pixPadding.Bottom));
						case ImageAlignment.BottomCenter:
							return new Point(((bounds.Width - htmlImageSize.Width - pixPadding.Right + pixPadding.Left) / 2), (bounds.Height - htmlImageSize.Height - pixPadding.Bottom));
						case ImageAlignment.BottomRight:
							return new Point((bounds.Width - htmlImageSize.Width - pixPadding.Right), (bounds.Height - htmlImageSize.Height - pixPadding.Bottom));
						case ImageAlignment.Default:
							if(sizeMode == ImageSizeMode.Normal)
								return new Point(pixPadding.Left, pixPadding.Top);
							else
								return new Point(((bounds.Width - htmlImageSize.Width - pixPadding.Right + pixPadding.Left) / 2), ((bounds.Height - htmlImageSize.Height - pixPadding.Bottom + pixPadding.Top) / 2));
					}
					return Point.Empty;
			}
			return Point.Empty;
		}
#if DEBUGTEST
		public Point Test_GetImagePosition(ImageSizeMode sizeMode, ImageAlignment align, PaddingInfo pixPadding, Rectangle bounds, Size htmlImageSize) {
			return GetImagePosition(sizeMode, align, pixPadding, bounds, htmlImageSize);
		}
#endif
	}
}
