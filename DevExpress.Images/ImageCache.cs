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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security;
using DevExpress.Utils.Design;
using DevExpress.Utils.Svg;
namespace DevExpress.Images {
	public class ImageResourceCache {
		Dictionary<string, Stream> resources;
		SvgImageCache svgImageCache;
		SvgRenderedImageCache svgRenderedImageCache;
		IList<string> svgImages;
		protected ImageResourceCache() {
			resources = new Dictionary<string, Stream>();
			svgImages = new List<string>();
			svgImageCache = new SvgImageCache();
			svgRenderedImageCache = new SvgRenderedImageCache();
		}
		public Image GetImage(string resourceName) {
			if(!this.resources.ContainsKey(resourceName)) {
				return null;
			}
			return Image.FromStream(this.resources[resourceName]);
		}
		public Image GetImageById(string id, ImageSize imageSize, ImageType imageType) {
			string prefix = string.Empty;
			var parts = id.Split('\\', '/');
			if(parts.Count() > 1) {
				prefix = parts.First().ToLower();
				id = parts.Last();
			}
			string fileName = ImageCacheUtils.GetFileName(id, imageSize, imageType);
			IList<string> acceptableKeys = this.resources.Keys.Where(key => ImageCacheUtils.IsMatch(key, prefix, fileName, imageType)).ToList();
			if(acceptableKeys.Count == 0 && !string.IsNullOrEmpty(prefix))
				acceptableKeys = this.resources.Keys.Where(key => ImageCacheUtils.IsMatch(key, string.Empty, fileName, imageType)).ToList();
			if(acceptableKeys.Count > 0) {
				string acceptableKey = acceptableKeys.First();
				if(imageType == ImageType.Svg)
					return GetSvgImage(acceptableKey, null, ConvertToSize(imageSize));
				else
					return GetImage(acceptableKeys.First());
			}
			return null;
		}
		Size ConvertToSize(ImageSize imageSize) {
			switch(imageSize) { 
				case ImageSize.Size16x16:
					return new Size(16,16);
				case ImageSize.Size32x32:
					return new Size(32,32);
			}
			return new Size(32, 32);
		}
		public void ResetSvgCache() {
			svgRenderedImageCache.Clear();
		}
		public Image GetSvgImageById(string id, ISvgPaletteProvider paletteProvider, int width, int height) {
			string prefix = string.Empty;
			var parts = id.Split('\\', '/');
			if(parts.Count() > 1) {
				prefix = parts.First().ToLower();
				id = parts.Last();
			}
			ImageType imageType = ImageType.Svg;
			Size imageSize = new Size(width, height);
			string fileName = ImageCacheUtils.GetFileName(id, ImageSize.Any, imageType);
			IList<string> acceptableKeys = this.resources.Keys.Where(key => ImageCacheUtils.IsMatch(key, prefix, fileName, imageType)).ToList();
			if(acceptableKeys.Count > 0)
				return GetSvgImage(acceptableKeys.First(), paletteProvider, imageSize);
			return null;
		}
		public Image GetSvgImage(string id, ISvgPaletteProvider paletteProvider, Size imageSize) {
			SvgImage svgImage;
			if(!svgImageCache.TryGetValue(id, out svgImage)) {
				svgImage = DevExpress.Utils.Svg.SvgLoader.LoadFromStream(this.resources[id]);
				svgImageCache.Add(id, svgImage);
			}
			Image image;
			if(!svgRenderedImageCache.TryGetValue(id, imageSize, paletteProvider, out image)) {
				image = svgImage.Render(imageSize, paletteProvider);
				svgRenderedImageCache.Add(new SvgImageKey(id, imageSize, paletteProvider != null ? paletteProvider.Clone() : null), image);
			}
			return image;
		}
		public Stream GetResourceByFileName(string fileName, ImageType imageType = ImageType.Colored) {
			var keys = from key in this.resources.Keys where key.EndsWith(fileName, StringComparison.OrdinalIgnoreCase) && ImageCollectionHelper.GetImageType(key) == imageType select key;
			if(keys.Count() != 1) {
				return null;
			}
			return GetResource(keys.First());
		}
		public Stream GetResource(string resourceName) {
			if(!this.resources.ContainsKey(resourceName))
				return null;
			return this.resources[resourceName];
		}
		public string[] GetAllResourceKeys() {
			return resources.Keys.Count == 0 ? null : resources.Keys.ToArray();
		}
		static ImageResourceCache defaultCore = null;
		public static ImageResourceCache Default {
			get {
				if(defaultCore == null) defaultCore = DoLoad();
				return defaultCore;
			}
		}
		internal ICollection GetKeys() {
			return this.resources.Keys;
		}
		protected internal IList<string> SvgImages {
			get { return svgImages; }
		}
		[SecuritySafeCritical]
		static ImageResourceCache DoLoad() {
			ImageResourceCache cache = new ImageResourceCache();
			using(ResourceReader reader = DoLoadResourceReader()) {
				foreach(IDictionaryEnumerator dict in GetResourceEnumerator(reader)) {
					string key = (string)dict.Key as string;
					if(IsImageBasedResource(key)) 
						cache.resources.Add(key, (Stream)dict.Value);
					if(key.EndsWith(".svg", StringComparison.Ordinal)) {
						var parts = key.Split('\\', '/');
						cache.svgImages.Add(parts.Last().Replace(".svg", ""));
					}
				}
			}
			return cache;
		}
		static bool IsImageBasedResource(string key) {
			return key.EndsWith(".png", StringComparison.Ordinal) || key.EndsWith(".svg", StringComparison.Ordinal);
		}
		static IEnumerable<IDictionaryEnumerator> GetResourceEnumerator(ResourceReader reader) {
			IDictionaryEnumerator dict = reader.GetEnumerator();
			while(dict.MoveNext())
				yield return dict;
		}
		[SecuritySafeCritical]
		static ResourceReader DoLoadResourceReader() {
			return new ResourceReader(ImagesAssembly.GetManifestResourceStream(ImageCollectionHelper.ResourceName));
		}
		public static readonly Assembly ImagesAssembly = Assembly.GetExecutingAssembly();
	}
	static class ImageCacheUtils {
		public static bool IsMatch(string key, string prefix, string fileName, ImageType imageType) {
			if(!key.EndsWith(fileName, StringComparison.OrdinalIgnoreCase)) return false;
			string[] parts = Split(key);
			bool result = parts.First().Equals(GetCategory(imageType), StringComparison.OrdinalIgnoreCase) && parts.Last().Equals(fileName, StringComparison.OrdinalIgnoreCase);
			if(!string.IsNullOrEmpty(prefix))
				result &= key.Contains(prefix);
			return result;
		}
		public static string[] Split(string key) {
			return key.Split('\\', '/');
		}
		static string GetCategory(ImageType imageType) {
			switch(imageType) {
				case ImageType.Colored: return "Images";
				case ImageType.DevAV: return "DevAV";
				case ImageType.GrayScaled: return "GrayScaleImages";
				case ImageType.Office2013: return "Office2013";
				case ImageType.Svg: return "SvgImages";
				default: throw new ArgumentException("imageType");
			}
		}
		public static string GetFileName(string id, ImageSize imageSize, ImageType imageType) {
			if(imageType == ImageType.Svg) return id + SvgImageSuffix;
			if(imageSize == ImageSize.Size16x16) return id + ImageSuffix;
			return id + LargeImageSuffix;
		}
		public static readonly string SvgImageSuffix = ".svg";
		public static readonly string ImageSuffix = "_16x16.png";
		public static readonly string LargeImageSuffix = "_32x32.png";
	}
}
