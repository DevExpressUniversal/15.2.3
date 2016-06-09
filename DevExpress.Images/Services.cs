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
using System.Linq;
using DevExpress.Utils.Design;
#if !SL
using System.Drawing;
using System.Reflection;
#endif
namespace DevExpress.Images {
	public class DXImageServicesImp : IDXImagesProviderEx {
		public string GetFile(string name, ImageSize imageSize, ImageType imageType) {
			IEnumerable<string> files = ImagesAssemblyImageList.GetUrisByName(name);
			return files.FirstOrDefault(x => {
				if(ImageCollectionHelper.GetImageType(x) != imageType) {
					return false;
				}
				switch(imageSize) {
					case ImageSize.Any:
						return true;
					case ImageSize.Size16x16:
						return x.ToLower().EndsWith("/" + name.ToLower() + "_16x16.png");
					case ImageSize.Size32x32:
						return x.ToLower().EndsWith("/" + name.ToLower() + "_32x32.png");
				}
				throw new NotImplementedException();
			});
		}
		public Image GetImage(string id, ImageSize imageSize, ImageType imageType) {
			return ImageResourceCache.Default.GetImageById(id, imageSize, imageType);
		}
		public IEnumerable<string> GetFiles(string name) {
			return from key in ImagesAssemblyImageList.Images
				   let uri = key.MakeUri()
					  where uri.ToLower().Contains(name.ToLower()) && uri.EndsWith(".png")
					  select uri;
		}
		public bool IsGrayScaledImage(string key) {
			return ImageCollectionHelper.GetImageType(key) == ImageType.GrayScaled;
		}
		public bool IsOffice2013Image(string key) {
			return ImageCollectionHelper.GetImageType(key) == ImageType.Office2013;
		}
		public bool IsDevAVImage(string key) {
			return ImageCollectionHelper.GetImageType(key) == ImageType.DevAV;
		}
		public bool IsBrowsable(string key) {
			return ImagesAssemblyImageInfo.IsBrowsable(key);
		}
		public IDXImageInfo[] GetAllImages() {
			return ImagesAssemblyImageList.Images;
		}
		public string[] GetBaseImages() {
			var publicFields = typeof(DXImages).GetFields(BindingFlags.Static | BindingFlags.Public);
			var values = from field in publicFields let val = field.GetValue(null).ToString() where field.FieldType == typeof(string) orderby val select val;
			return values.ToArray();
		}
		public Image GetSvgImage(string id, ISvgPaletteProvider paletteProvider, int width = 32, int height = 32) {
			return ImageResourceCache.Default.GetSvgImageById(id, paletteProvider, width, height);
		}
		public IEnumerable<string> GetSvgImages() {
			return ImageResourceCache.Default.SvgImages;
		}
	}
	public class ImagesAssemblyType { };
}
