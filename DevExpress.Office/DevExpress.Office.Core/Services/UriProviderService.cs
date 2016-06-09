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
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office.Utils;
#if !SL
using System.Drawing.Imaging;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.Office.Services {
	#region IUriProviderService
	[ComVisible(true)]
	public interface IUriProviderService {
		string CreateImageUri(string rootUri, OfficeImage image, string relativeUri);
		string CreateCssUri(string url, string styleText, string relativeUri);
		void RegisterProvider(IUriProvider provider);
		void UnregisterProvider(IUriProvider provider);
	}
	#endregion
	#region IUriProvider
	[ComVisible(true)]
	public interface IUriProvider {
		string CreateImageUri(string rootUri, OfficeImage image, string relativeUri);
		string CreateCssUri(string rootUri, string styleText, string relativeUri);
	}
	#endregion
}
namespace DevExpress.Office.Services.Implementation {
	#region UriProviderCollection
	[ComVisible(true)]
	public class UriProviderCollection : List<IUriProvider> {
	}
	#endregion
	#region UriProviderService
	[ComVisible(true)]
	public class UriProviderService : IUriProviderService {
		readonly UriProviderCollection providers;
		public UriProviderService() {
			this.providers = new UriProviderCollection();
			RegisterDefaultProviders();
		}
		public UriProviderCollection Providers { get { return providers; } }
		public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
			if (image == null)
				return String.Empty;
			string result;
			int count = Providers.Count;
			for (int i = 0; i < count; i++) {
				result = Providers[i].CreateImageUri(rootUri, image, relativeUri);
				if (!String.IsNullOrEmpty(result))
					return result;
			}
			return String.Empty;
		}
		public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
			string result;
			int count = Providers.Count;
			for (int i = 0; i < count; i++) {
				result = Providers[i].CreateCssUri(rootUri, styleText, relativeUri);
				if (!String.IsNullOrEmpty(result))
					return result;
			}
			return String.Empty;
		}
		public void RegisterProvider(IUriProvider provider) {
			if (provider == null)
				return;
			Providers.Insert(0, provider);
		}
		public void UnregisterProvider(IUriProvider provider) {
			if (provider == null)
				return;
			int index = Providers.IndexOf(provider);
			if (index >= 0)
				Providers.RemoveAt(index);
		}
		protected internal virtual void RegisterDefaultProviders() {
#if !SL && !DXPORTABLE
			RegisterProvider(new FileBasedUriProvider());
#endif
		}
	}
	#endregion
	#region EmptyUriProvider
	[ComVisible(true)]
	public class EmptyUriProvider : IUriProvider {
		int counter;
		#region IUriProvider Members
		public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
			counter++;
			return String.Format("img{0}.img", counter);
		}
		public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
			if (String.IsNullOrEmpty(styleText))
				return String.Empty;
			counter++;
			return String.Format("css{0}.css", counter);
		}
		#endregion
	}
	#endregion
	#region DataStringUriProvider
	[ComVisible(true)]
	public class DataStringUriProvider : IUriProvider {
		class ImageBytes {
			readonly byte[] bytes;
			readonly OfficeImageFormat format;
			public ImageBytes(byte[] bytes, OfficeImageFormat format) {
				this.bytes = bytes;
				this.format = format;
			}
			public byte[] Bytes { get { return bytes; } }
			public OfficeImageFormat Format { get { return format; } }
		}
		#region IUriProvider Members
		public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
			if (image == null)
				return String.Empty;
			try {
				ImageBytes bytes = GetImageBytes(image);
				string contentType = OfficeImage.GetContentType(bytes.Format);
				return String.Format("data:{0};base64,{1}", contentType, Convert.ToBase64String(bytes.Bytes));
			}
			catch {
				return String.Empty;
			}
		}
		ImageBytes GetImageBytes(OfficeImage image) {
			try {
				return GetImageBytesCore(image, image.RawFormat);
			}
			catch {
				return GetImageBytesCore(image, OfficeImageFormat.Png);
			}
		}
		ImageBytes GetImageBytesCore(OfficeImage image, OfficeImageFormat imageFormat) {
			byte[] bytes = image.GetImageBytes(imageFormat);
			return new ImageBytes(bytes, imageFormat);
		}
		public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
#if !SL && !DXPORTABLE
	#region FileBasedUriProvider
	[ComVisible(true)]
	public class FileBasedUriProvider : IUriProvider {
		const string fileUriPrefix = "file:///";
		string lastPath;
		string lastName;
		string lastExtension;
		int lastFileIndex;
		#region IUriProvider Members
		public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
			if (image == null)
				return String.Empty;
			rootUri = Path.GetDirectoryName(rootUri);
			if (!EnsureDirectoryExists(rootUri))
				return String.Empty;
			string fileName = CreateNextFileName(rootUri, "image", GetImageFileExtension(image));
			if (TryToSaveImageInNativeFormat(image.NativeImage, fileName)) {
				if (!String.IsNullOrEmpty(relativeUri))
					return GetRelativeFileName(rootUri, relativeUri, fileName);
				return fileUriPrefix + fileName;
			}
			fileName = CreateNextFileName(rootUri, "image", "png");
			if (TryToSaveImageAsPng(image.NativeImage, fileName)) {
				if (!String.IsNullOrEmpty(relativeUri))
					return GetRelativeFileName(rootUri, relativeUri, fileName);
				return fileUriPrefix + fileName;
			}
			else
				return String.Empty;
		}
		protected internal string GetRelativeFileName(string rootUri, string relativeUri, string fileName) {
			StringBuilder str = new StringBuilder(fileName);
			try {
				if (rootUri.Length > 0)
					str.Remove(0, rootUri.Length + 1);
				str.Insert(0, relativeUri);
			}
			catch {
				Exceptions.ThrowArgumentException("invalid relativeUri", relativeUri);
				return fileUriPrefix + fileName;
			}
			return str.ToString();
		}
		public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
			if (String.IsNullOrEmpty(styleText))
				return String.Empty;
			rootUri = Path.GetDirectoryName(rootUri);
			if (!EnsureDirectoryExists(rootUri))
				return String.Empty;
			string fileName = CreateNextFileName(rootUri, "style", "css");
			SaveCssProperties(fileName, styleText);
			if (!String.IsNullOrEmpty(relativeUri))
				return GetRelativeFileName(rootUri, relativeUri, fileName);
			return fileUriPrefix + fileName;
		}
	#endregion
		protected internal virtual bool TryToSaveImageInNativeFormat(Image image, string fileName) {
			try {
				image.Save(fileName);
				return true;
			}
			catch {
				return false;
			}
		}
		protected internal virtual bool TryToSaveImageAsPng(Image image, string fileName) {
			try {
				ImageCodecInfo codecInfo = OfficeImageWin.GetImageCodecInfo(OfficeImageHelper.GetImageFormat(OfficeImageFormat.Png));
				image.Save(fileName, codecInfo, null);
				return true;
			}
			catch {
				return false;
			}
		}
		protected internal void SaveCssProperties(string fileName, string styleText) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
					writer.Write(styleText);
				}
			}
		}
		protected internal virtual bool EnsureDirectoryExists(string path) {
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return Directory.Exists(path);
		}
		protected internal virtual string CreateNextFileName(string path, string name, string extension) {
			for (int i = GetInitialFileIndex(path, name, extension); ; i++) {
				string fileName = String.Format("{0}{1}.{2}", name, i, extension);
				fileName = Path.Combine(path, fileName);
				if (!File.Exists(fileName)) {
					lastFileIndex = i;
					return fileName;
				}
			}
		}
		protected internal virtual int GetInitialFileIndex(string path, string name, string extension) {
			path = Path.GetFullPath(path);
			if (String.Compare(path, lastPath, StringComparison.OrdinalIgnoreCase) == 0 &&
				String.Compare(name, lastName, StringComparison.OrdinalIgnoreCase) == 0 &&
				String.Compare(extension, lastExtension, StringComparison.OrdinalIgnoreCase) == 0) {
				return lastFileIndex + 1;
			}
			else {
				lastPath = path;
				lastName = name;
				lastExtension = extension;
				lastFileIndex = 0;
				return lastFileIndex;
			}
		}
		protected internal virtual string GetImageFileExtension(OfficeImage image) {
			string extension = OfficeImage.GetExtension(image.RawFormat);
			if (!String.IsNullOrEmpty(extension))
				return extension;
			if (image.RawFormat == OfficeImageFormat.MemoryBmp)
				return "png";
			else
				return "img";
		}
	}
	#endregion
#endif
}
