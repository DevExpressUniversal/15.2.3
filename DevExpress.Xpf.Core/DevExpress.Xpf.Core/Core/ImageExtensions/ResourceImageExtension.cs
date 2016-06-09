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

using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Utils.Native;
namespace DevExpress.Xpf.Core {
	public class ResourceImageExtension : MarkupExtension {
		public string ResourceName { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if(string.IsNullOrEmpty(ResourceName))
				throw new InvalidOperationException("The value of the ResourceName property is not set.");
			ResourceManager rm = CreateResourceManager();
			using(ResourceSet set = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true))
			using(UnmanagedMemoryStream stream = (UnmanagedMemoryStream)set.GetObject(ResourceName, true)) {
				return CreateImage(stream);
			}
		}
		ResourceManager CreateResourceManager() {
#if SILVERLIGHT
			string rootNamespace = GetType().Assembly.FullName.Split(',').First();
#else
			string rootNamespace = GetType().Assembly.GetName().Name;
#endif
			string baseName = rootNamespace + ".g";
			return new ResourceManager(baseName, GetType().Assembly);
		}
		ImageSource CreateImage(Stream stream) {
#if SILVERLIGHT
			BitmapImage image = new BitmapImage();
			image.SetSource(stream);
			return image;
#else
			BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
			BitmapSource bitmapSource = decoder.Frames.First();
			return bitmapSource;
#endif
		}
		public static DXImageExtension GetDXImageExtension(ImageType imageType) {
			switch(imageType) {
				case ImageType.Colored:
					return new DXImageExtension();
				case ImageType.GrayScaled:
					return new DXImageGrayscaleExtension();
				case ImageType.Office2013:
					return new DXImageOffice2013Extension();
			}
			throw new NotImplementedException(imageType.ToString());
		}
	}
#if !SILVERLIGHT
	public static class DXImageHelper {
		static DXImageHelper() {
			ImageSourceHelper.RegisterPackScheme();
		}
		static IDXImagesProvider imageProvider;
		internal static IDXImagesProvider ImagesProvider { get { return imageProvider ?? (imageProvider = CreateImageProvider()); } }
		static IDXImagesProvider CreateImageProvider() {
			return CreateImageProviderInstance();
		}
		static IDXImagesProvider CreateImageProviderInstance() {
			return (IDXImagesProvider)Activator.CreateInstance(Type.GetType("DevExpress.Images.DXImageServicesImp, " + AssemblyInfo.SRAssemblyImages + AssemblyHelper.GetFullNameAppendix()));
		}
		public static ImageSource GetImageSource(string path) {
			return ImageSourceHelper.GetImageSource(GetImageUri(path));
		}
		public static Uri GetImageUri(string path) {
			return new Uri(@"pack://application:,,,/" + AssemblyInfo.SRAssemblyImages + ";component/" + path, UriKind.RelativeOrAbsolute);
		}
		public static ImageSource GetImageSource(string id, ImageSize imageSize, ImageType imageType = ImageType.Colored) {
			return ImageSourceHelper.GetImageSource(GetImageUri(id, imageSize, imageType));
		}
		public static Uri GetImageUri(string id, ImageSize imageSize, ImageType imageType = ImageType.Colored) {
			return new Uri(GetFile(id, imageSize, imageType), UriKind.RelativeOrAbsolute);
		}
		internal static string GetFile(string name, ImageSize imageSize, ImageType imageType) {
			return ImagesProvider.GetFile(name, imageSize, imageType);
		}
	}
	public class DXImageExtension : MarkupExtension {
		[TypeConverter(typeof(DXImageConverter))]
		public DXImageInfo Image { get; set; }
		public DXImageExtension() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Image.With(x => ImageSourceHelper.GetImageSource(x.MakeUri()));
		}
	}
	public class DXImageGrayscaleExtension : DXImageExtension {
		[TypeConverter(typeof(DXImageGrayscaleConverter))]
		public new DXImageInfo Image { get { return base.Image; } set { base.Image = value; } }
	}
	public class DXImageOffice2013Extension : DXImageExtension {
		[TypeConverter(typeof(DXImageOffice2013Converter))]
		public new DXImageInfo Image { get { return base.Image; } set { base.Image = value; } }
	}
	public class DXImageInfo {
		readonly IDXImageInfo info;
		public DXImageInfo(IDXImageInfo info) {
			this.info = info;
		}
		public override string ToString() {
			string name = info.Name;
			if(info.Group == "Navigation" && info.Name.StartsWith("Next_"))
				name = "Navigate" + name;
			if(info.Group == "Actions" && info.Name.StartsWith("Delete_") && info.ImageType == ImageType.GrayScaled)
				name = name.Replace("Delete_", "Remove_");
			return name;
		}
		public Uri MakeUri() {
			return new Uri(info.MakeUri(), UriKind.Absolute);
		}
		public string Group { get { return info.Group; } }
	}
#endif
}
namespace DevExpress.Xpf.Core.Native {
#if !SILVERLIGHT
	public class DXImageConverter : TypeConverter {
		public static IEnumerable<IDXImageInfo> GetDistinctImages(IDXImagesProvider imagesProvider) {
			return imagesProvider.GetAllImages().Where(x => {
				if(x.ImageType == ImageType.GrayScaled) {
					if(x.Group == "Edit" && x.Name.StartsWith("New_"))
						return false;
					if(x.Group == "Edit" && x.Name.StartsWith("Remove_"))
						return false;
				}
				return true;
			});
		}
		static readonly Lazy<Dictionary<string, DXImageInfo>> ImagesColored = GetImages(ImageType.Colored);
		protected readonly static Lazy<Dictionary<string, DXImageInfo>> ImagesGrayscale = GetImages(ImageType.GrayScaled);
		protected readonly static Lazy<Dictionary<string, DXImageInfo>> ImagesOffice2013 = GetImages(ImageType.Office2013);
		static Lazy<Dictionary<string, DXImageInfo>> GetImages(ImageType imageType) {
			return new Lazy<Dictionary<string, DXImageInfo>>(() => {
				return GetDistinctImages(DXImageHelper.ImagesProvider).Where(x => x.ImageType == imageType).Select(x => new DXImageInfo(x)).ToDictionary(x => x.ToString().ToLowerInvariant());
			});
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if(stringValue != null) {
				return Images.Value.GetValueOrDefault(stringValue.ToLowerInvariant());
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(Images.Value.Values);
		}
		protected virtual Lazy<Dictionary<string, DXImageInfo>> Images { get { return ImagesColored; } }
	}
	public class DXImageGrayscaleConverter :  DXImageConverter {
		protected override Lazy<Dictionary<string, DXImageInfo>> Images { get { return ImagesGrayscale; } }
	}
	public class DXImageOffice2013Converter : DXImageConverter {
		protected override Lazy<Dictionary<string, DXImageInfo>> Images { get { return ImagesOffice2013; } }
	}  
#endif
}
