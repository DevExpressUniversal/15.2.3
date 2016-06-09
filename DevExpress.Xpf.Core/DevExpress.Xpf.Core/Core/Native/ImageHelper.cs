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

using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System;
using System.Windows.Navigation;
#if SL && !SLDESIGN
using PlatformIndependentImage = System.Windows.Controls.Image;
using PlatformImage = System.Windows.Controls.Image;
#else
#if !SLDESIGN
using PlatformIndependentImage = System.Drawing.Image;
#endif
using PlatformImage = System.Windows.Controls.Image;
#endif
#if SLDESIGN
namespace DevExpress.Xpf.Core.Design.CoreUtils {
#else
namespace DevExpress.Xpf.Core.Native {
#endif
	public static class ImageHelper {
#if !SL
		public static void UpdateBaseUri(DependencyObject d, ImageSource source) {
			if (!(source is IUriContext) || source.IsFrozen || (!(((IUriContext)source).BaseUri == null) || !(BaseUriHelper.GetBaseUri(d) != null)))
				return;
			((IUriContext)source).BaseUri = BaseUriHelper.GetBaseUri(d);
		}
#endif
		public static BitmapImage CreateImageFromStream(Stream stream) {
			BitmapImage bitmapImage = new BitmapImage();
#if !SL || SLDESIGN
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = stream;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
#else
			bitmapImage.SetSource(stream);
#endif
			return bitmapImage;
		}
		public static BitmapImage CreateImageFromCoreEmbeddedResource(string resource) {
			return CreateImageFromEmbeddedResource(Assembly.GetExecutingAssembly(), "DevExpress.Xpf.Core." + resource);
		}
		public static BitmapImage CreateImageFromEmbeddedResource(Assembly assembly, string resource) {
			Stream stream = assembly.GetManifestResourceStream(resource);
			return stream != null ? CreateImageFromStream(stream) : null;
		}
#if !SLDESIGN
		public static PlatformImage CreatePlatformImage(PlatformIndependentImage img) {
#if SL
			return img;
#else
			PlatformImage image = new PlatformImage();
			image.Source = CreateImageSource(img);
			return image;
#endif
		}
		public static ImageSource CreateImageSource(PlatformIndependentImage img) {
			BitmapImage imageSource = new BitmapImage();
			imageSource.BeginInit();
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			img.Save(stream, FindImageCodec(img.RawFormat), null);
			stream.Seek(0, System.IO.SeekOrigin.Begin);
			imageSource.StreamSource = stream;
			imageSource.EndInit();
			imageSource.Freeze();
			return imageSource;
		}
#if !SL
		static System.Drawing.Imaging.ImageCodecInfo FindImageCodec(System.Drawing.Imaging.ImageFormat imageFormat) {
			System.Drawing.Imaging.ImageCodecInfo[] encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
			int count = encoders.Length;
			System.Drawing.Imaging.ImageCodecInfo pngEncoder = null;
			for (int i = 0; i < count; i++) {
				System.Drawing.Imaging.ImageCodecInfo current = encoders[i];
				if (current.Equals(imageFormat.Guid))
					return current;
				if (pngEncoder == null && current.FormatID.Equals(System.Drawing.Imaging.ImageFormat.Png.Guid))
					pngEncoder = current;
			}
			return pngEncoder;
		}
#endif
#endif
	}
}
