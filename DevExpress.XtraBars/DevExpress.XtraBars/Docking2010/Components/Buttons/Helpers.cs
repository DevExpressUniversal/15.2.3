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
using System.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.Data.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking2010 {
	static class ActionsBarButtonImageHelper {
		public static object PasteImageInGlyphs(object glyphs, Image image) {
			ImageCollection glyphsCollection = new ImageCollection();
			glyphsCollection = GetImageCollection(glyphs);
			ImageCollection result = new ImageCollection();
			result.ImageSize = GetImageSize(glyphs, image);
			foreach(Image glyph in glyphsCollection.Images) {
				result.AddImage(StickImages(glyph, image));
			}
			return result;
		}
		public static object MergeImageLists(object imageList1, object imageList2) {
			ImageCollection longImageCollection = new ImageCollection();
			ImageCollection result = new ImageCollection();
			result.ImageSize = GetImageSize(imageList1, imageList2);
			ImageCollection imageCollection = new ImageCollection();
			if(ImageCollection.GetImageListImageCount(imageList1) > ImageCollection.GetImageListImageCount(imageList2)) {
				longImageCollection = GetImageCollection(imageList1);
				imageCollection = GetImageCollection(imageList2);
			}
			else {
				longImageCollection = GetImageCollection(imageList2);
				imageCollection = GetImageCollection(imageList1);
			}
			for(int i = 0; i < imageCollection.Images.Count; i++) {
				result.AddImage(StickImages(imageCollection.Images[i], longImageCollection.Images[i]));
			}
			for(int i = imageCollection.Images.Count; i < longImageCollection.Images.Count; i++) {
				result.AddImage(ImageCollection.GetImageListImage(longImageCollection, i));
			}
			return result;
		}
		public static ImageCollection CreateImageListFromResources(string name, Assembly asm, Size size) {
			ImageCollection result = new ImageCollection();
			result.ImageSize = size;
			Stream stream = asm.GetManifestResourceStream(name);
			result.Images.AddImageStrip(ImageTool.ImageFromStream(stream));
			return result;
		}
		public static ImageCollection CreateImageListFromImage(Image image, Size size) {
			ImageCollection result = new ImageCollection();
			result.ImageSize = size;
			result.Images.AddImageStrip(image);
			return result;
		}
		public static Image StickImages(Image image1, Image image2) {
			Rectangle largeBounds = Rectangle.Empty;
			Rectangle smallBounds = Rectangle.Empty;
			Image largeImage = null;
			Image smallImage = null;
			if(image1.Height < image2.Height && image1.Width < image2.Width) {
				largeBounds.Size = image2.Size;
				largeImage = image2;
				smallImage = image1;
			}
			else {
				largeBounds.Size = image1.Size;
				largeImage = image1;
				smallImage = image2;
			}
			smallBounds = PlacementHelper.Arrange(smallImage.Size, largeBounds, ContentAlignment.MiddleCenter);
			return DrawImageCore(largeBounds, smallBounds, largeImage, smallImage);
		}
		static ImageCollection GetImageCollection(object imageList) {
			ImageCollection result = new ImageCollection();
			result.ImageSize = ImageCollection.GetImageListSize(imageList);
			for(int i = 0; i < ImageCollection.GetImageListImageCount(imageList); i++) {
				result.AddImage(ImageCollection.GetImageListImage(imageList, i));
			}
			return result;
		}
		static Size GetImageSize(object glyphs, Image image) {
			Size glyphsSize = ImageCollection.GetImageListSize(glyphs);
			if(glyphsSize.Height > image.Height && glyphsSize.Width > image.Width)
				return glyphsSize;
			return image.Size;
		}
		static Size GetImageSize(object imageCollection1, object imageCollection2) {
			Size size1 = ImageCollection.GetImageListSize(imageCollection1);
			Size size2 = ImageCollection.GetImageListSize(imageCollection2);
			if(size1.Height > size2.Height && size1.Width > size2.Width)
				return size1;
			return size2;
		}
		static Image DrawImageCore(Rectangle largeBounds, Rectangle smallBounds, Image largeImage, Image smallImage) {
			Bitmap newImage = new Bitmap(largeImage);
			using(Graphics g = Graphics.FromImage(newImage)) {
				g.DrawImage(smallImage, smallBounds);
			}
			return newImage;
		}
	}
	static class ColoredElementsCache {
		static IDictionary<Color, Image> headerImages = new Dictionary<Color, Image>();
		static IDictionary<Color, Image> actionsBarImages = new Dictionary<Color, Image>();
		static IDictionary<Color, Image> customButtonImages = new Dictionary<Color, Image>();
		public static Color ConvertColor(Color value, Color? color) {
			if(!color.HasValue) return value;
			return Color.FromArgb(DevExpress.Utils.Helpers.ColoredImageHelper.Convert(value.ToArgb(), color.Value.ToArgb()));
		}
		static Image GetCachedImage(IDictionary<Color, Image> cache, Color color, Func<Image> getSourceImage) {
			Image image;
			if(!cache.TryGetValue(color, out image)) {
				image = DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(getSourceImage(), color);
				cache.Add(color, image);
			}
			return image;
		}
		static void ResetCache(IDictionary<Color, Image> cache) {
			foreach(KeyValuePair<Color, Image> pair in cache) {
				if(pair.Value != null)
					pair.Value.Dispose();
			}
			cache.Clear();
		}
		public static Image GetHeaderImage(Color color, Func<Image> getSourceImage) {
			return GetCachedImage(headerImages, color, getSourceImage);
		}
		public static Image GetActionsBarImage(Color color, Func<Image> getSourceImage) {
			return GetCachedImage(actionsBarImages, color, getSourceImage);
		}
		public static Image GetCustomButtonImage(Color color, Func<Image> getSourceImage) {
			return GetCachedImage(actionsBarImages, color, getSourceImage);
		}
		public static void Reset() {
			ResetCache(headerImages);
			ResetCache(actionsBarImages);
			ResetCache(customButtonImages);
		}
	}
}
