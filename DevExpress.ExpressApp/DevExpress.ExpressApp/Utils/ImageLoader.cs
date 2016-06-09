#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public struct ImageInfo {
		private readonly bool isInitialized;
		public static readonly ImageInfo Empty = new ImageInfo(); 
		public const string NullImageUrlValue = "Unknown";
		private string imageName;
		private Image image;
		private string imageUrl;
		private int width;
		private int height;
		private string _MD5Hash;
		public ImageInfo(string imageName, Image image, string imageUrl) {
			this.isInitialized = true;
			this.imageName = imageName;
			this.imageUrl = "";
			if(imageUrl != NullImageUrlValue) {
				this.imageUrl = imageUrl;
			}
			this.width = -1;
			this.height = -1;
			this.image = image;
			if(image != null) {
				this.width = image.Width;
				this.height = image.Height;
			}
			_MD5Hash = null;
		}
		public string ImageName {
			get { return imageName; }
		}
		public Image Image {
			get { return image; }
		}
		public bool IsEmpty {
			get { return !isInitialized; }
		}
		public bool IsUrlEmpty {
			get { return string.IsNullOrEmpty(imageUrl); }
		}
		public string ImageUrl {
			get { return (!IsUrlEmpty) ? imageUrl : NullImageUrlValue; }
		}
		public int Width {
			get { return width; }
		}
		public int Height {
			get { return height; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string MD5Hash {
			get {
				if(_MD5Hash == null) {
					_MD5Hash = CalcMD5Hash();
				}
				return _MD5Hash;
			}
		}
		private string CalcMD5Hash() {
			if(image != null) {
				using(MD5 md5 = MD5.Create()) {
					byte[] hash = md5.ComputeHash(GetImageBytes(new Bitmap(image)));
					StringBuilder sb = new StringBuilder();
					foreach(byte b in hash) {
						sb.Append(b.ToString("x2").ToLower());
					}
					return sb.ToString();
				}
			}
			return string.Empty;
		}
		[System.Security.SecuritySafeCritical]
		private byte[] GetImageBytes(Bitmap bmp) {
			Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			System.Drawing.Imaging.BitmapData bmpData =
			 bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
			 bmp.PixelFormat);
			IntPtr startLineAddress = bmpData.Scan0;
			int bytes = bmpData.Stride * bmp.Height;
			byte[] rgbValues = new byte[bytes];
			System.Runtime.InteropServices.Marshal.Copy(startLineAddress, rgbValues, 0, bytes);
			bmp.UnlockBits(bmpData);
			return rgbValues;
		}
		#region Comparision methods for structure types
		public override int GetHashCode() {
			if(image != null) {
				return image.GetHashCode();
			}
			return Empty.GetHashCode(); 
		}
		public override bool Equals(object obj) {
			if(base.Equals(obj)) {
				return true;
			}
			else if(obj is ImageInfo) {
				return ((ImageInfo)obj).image == image;
			}
			return false;
		}
		public static bool operator ==(ImageInfo left, ImageInfo right) {
			return left.Equals(right);
		}
		public static bool operator !=(ImageInfo left, ImageInfo right) {
			return !left.Equals(right);
		}
		#endregion
	}
	public enum ImageSourceState { Passive, Unknown, Active }
	public class CustomLoadImageFromStreamEventArgs : EventArgs {
		public CustomLoadImageFromStreamEventArgs(Stream imageStream) {
			this.ImageStream = imageStream;
		}
		public Stream ImageStream { get; private set; }
		public Image Image { get; set; }
	}
	public class ImageLoadException : Exception {
		public ImageLoadException(string imageName, bool isEnabled, Exception innerException)
			: base(string.Format("An exception occurs while loading '{0}' image (IsEnabled = {1}): {2}. See inner exception for details.", imageName, isEnabled, innerException.Message), innerException) {
			this.ImageName = imageName;
			this.IsEnabled = isEnabled;
		}
		public string ImageName { get; set; }
		public bool IsEnabled { get; set; }
	}
	public class CustomGetImageInfoEventArgs : HandledEventArgs {
		public CustomGetImageInfoEventArgs(string imageName, bool isEnabled) {
			ImageName = imageName;
			IsEnabled = isEnabled;
		}
		public string ImageName { get; private set; }
		public bool IsEnabled { get; private set; }
		public ImageInfo ImageInfo { get; set; }
	}
	[Browsable(false)]
	public abstract class ImageSource {
		public static event EventHandler<CustomLoadImageFromStreamEventArgs> CustomLoadImageFromStream;
		private static Image LoadImageFromStream(Stream imageStream) {
			if(CustomLoadImageFromStream != null) {
				CustomLoadImageFromStreamEventArgs args = new CustomLoadImageFromStreamEventArgs(imageStream);
				CustomLoadImageFromStream(null, args);
				Guard.ArgumentNotNull(args.Image, "args.Image");
				return args.Image;
			}
			return Image.FromStream(imageStream);
		}
		private ImageSourceState imageSourceState = ImageSourceState.Unknown;
		private Image ConvertToPNG(Image image) {
			MemoryStream stream = new MemoryStream();
			image.Save(stream, ImageFormat.Png);
			return LoadImageFromStream(stream);
		}
		private Image GetPNGFromIcon(Stream iconStream, int necessarySize) {
			iconStream.Position = 0;
			return ConvertToPNG(new Icon(iconStream, necessarySize, necessarySize).ToBitmap());
		}
		private Image ConvertToGrayscale(Image image) {
			Bitmap bm = new Bitmap(image.Width, image.Height);
			using(Graphics g = Graphics.FromImage(bm)) {
				ColorMatrix cm = new ColorMatrix(new float[][]{   
								new float[]{0.3f,0.3f,0.3f,0,0},
								new float[]{0.59f,0.59f,0.59f,0,0},
								new float[]{0.11f,0.11f,0.11f,0,0},
								new float[]{0,0,0,1,0},
								new float[]{0.2f,0.2f,0.2f,0,1}});
				ImageAttributes ia = new ImageAttributes();
				ia.SetColorMatrix(cm);
				g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
			}
			return bm;
		}
		private static bool IsLargeImageName(string imageName) {
			return imageName.Contains(ImageLoader.LargeImageSuffix);
		}
		protected abstract bool FindImageStream(string imageName, out string imageUrl, out Stream imageStream, string imageFolder);
		public ImageInfo FindImageInfo(string imageName, bool isEnabled) {
			return FindImageInfo(imageName, isEnabled, true, string.Empty);
		}
		public ImageInfo FindImageInfo(string imageName, bool isEnabled, string imageFolder) {
			return FindImageInfo(imageName, isEnabled, true, imageFolder);
		}
		public ImageInfo FindImageInfo(string imageName, bool isEnabled, bool findSmallWhenLargeIsNotFound) {
			return FindImageInfo(imageName, isEnabled, findSmallWhenLargeIsNotFound, ImageLoader.Instance.CustomImageFolder);
		}
		public ImageInfo FindImageInfo(string imageName, bool isEnabled, bool findSmallWhenLargeIsNotFound, string imageFolder) {
			try {
				string imageUrl;
				Stream imageStream;
				Image image = null;
				if(FindImageStream(imageName, out imageUrl, out imageStream, imageFolder)) {
					image = LoadImageFromStream(imageStream);
					if(image.RawFormat.Guid == ImageFormat.Bmp.Guid) {
						if(image is Bitmap) {
							((Bitmap)image).MakeTransparent();
						}
						image = ConvertToPNG(image);
					}
					else if(image.RawFormat.Guid == ImageFormat.Icon.Guid) {
						int imageSize = IsLargeImageName(imageName) ? 32 : 16;
						image = GetPNGFromIcon(imageStream, imageSize);
					}
				}
				else {
					if(IsLargeImageName(imageName) && findSmallWhenLargeIsNotFound) {
						if(FindImageStream(imageName.Replace(ImageLoader.LargeImageSuffix, ""), out imageUrl, out imageStream, imageFolder)) {
							Image smallImage = LoadImageFromStream(imageStream);
							if(smallImage.RawFormat.Guid == ImageFormat.Icon.Guid) {
								image = GetPNGFromIcon(imageStream, 32);
							}
						}
					}
				}
				if(image != null) {
					if(!isEnabled) {
						image = ConvertToGrayscale(image);
					}
					return new ImageInfo(imageName, image, imageUrl);
				}
				return ImageInfo.Empty;
			}
			catch(Exception e) {
				throw new ImageLoadException(imageName, isEnabled, e); 
			}
		}
		public abstract List<string> GetImageNames();
		public ImageSourceState ImageSourceState {
			get { return imageSourceState; }
			protected set { imageSourceState = value; }
		}
		protected internal virtual void ClearCache() { }
		#region Obsolete 15.1
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the FindImageStream(string imageName, out string imageUrl, out Stream imageStream, string imageFolder) method instead.", true)]
		protected virtual bool FindImageStream(string imageName, out string imageUrl, out Stream imageStream) {
			throw new NotImplementedException();
		}
		#endregion
	}
	[Browsable(false)]
	public class FileImageSource : ImageSource {
		public const string NodeName = "FileImageSource";
		private string folderName;
		private Dictionary<string, bool> folderExistsCache = new Dictionary<string, bool>();
		private Boolean FolderExists(String fileName) {
			try {
				bool result;
				if(!folderExistsCache.TryGetValue(fileName, out result)) {
					result = Directory.Exists(Path.GetDirectoryName(fileName));
					folderExistsCache[fileName] = result;
				}
				return result;
			}
			catch(PathTooLongException e) {
				Tracing.Tracer.LogError(e);
			}
			return false;
		}
		protected internal override void ClearCache() {
			base.ClearCache();
			folderExistsCache.Clear();
		}
		protected override bool FindImageStream(string imageName, out string imageUrl, out Stream imageStream, string imageFolder) {
			imageUrl = null;
			imageStream = null;
			bool result = false;
			if(!String.IsNullOrEmpty(imageName)) {
				bool useCustomFolder = (!string.IsNullOrEmpty(imageFolder) && FolderExists(imageFolder));
				string targetImageFolder = !useCustomFolder ? Folder : imageFolder;
				result = FindImageStreamCore(imageName, ref imageUrl, ref imageStream, targetImageFolder);
				if(useCustomFolder && !result) {
					result = FindImageStreamCore(imageName, ref imageUrl, ref imageStream, Folder);
				}
			}
			return result;
		}
		private bool FindImageStreamCore(string imageName, ref string imageUrl, ref Stream imageStream, string targetImageFolder) {
			String fullFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(targetImageFolder, imageName));
			if(FolderExists(fullFileName)) {
				String[] fileNames = GetFileNames(fullFileName);
				if(fileNames.Length > 0) {
					imageUrl = Path.Combine(targetImageFolder, Path.Combine(Path.GetDirectoryName(imageName), Path.GetFileName(fileNames[0]))).Replace('\\', '/');
					imageStream = GetMemoryImageStreamFromFile(fileNames[0]);
					if(imageStream != null) {
						return true;
					}
				}
			}
			return false;
		}
		private String[] GetFileNames(string fullFileName) {
			String[] fileNames = Directory.GetFiles(
						Path.GetDirectoryName(fullFileName),
						Path.GetFileName(fullFileName) + ".*");
			return fileNames;
		}
		private MemoryStream GetMemoryImageStreamFromFile(string fileName) {
			if(!string.IsNullOrEmpty(fileName)) {
				Stream imageFileStream = File.OpenRead(fileName);
				try {
					MemoryStream result = new MemoryStream();
					byte[] buf = new byte[1024];
					int realyReaded = 0;
					while((realyReaded = imageFileStream.Read(buf, 0, buf.Length)) != 0) {
						result.Write(buf, 0, realyReaded);
					}
					result.Position = 0;
					return result;
				}
				finally {
					imageFileStream.Close();
				}
			}
			return null;
		}
		public FileImageSource(string propertyName) {
			this.folderName = propertyName;
			ImageSourceState = ImageSourceState.Active;
		}
		public string Folder {
			get {
				return folderName;
			}
			set {
				folderName = value;
			}
		}
		public override List<string> GetImageNames() {
			List<string> imageNames = new List<string>();
			if(Directory.Exists(folderName)) {
				foreach(string imageNameWithExtention in Directory.GetFiles(folderName)) {
					string imageName = Path.GetFileNameWithoutExtension(imageNameWithExtention);
					imageNames.Add(imageName);
				}
			}
			return imageNames;
		}
		public override string ToString() {
			return GetType().Name + ", " + folderName;
		}
	}
	[Browsable(false)]
	public class AssemblyResourceImageSource : ImageSource {
		public const string DefaultImagesFolder = "Images";
		private string rootImagesFolder = DefaultImagesFolder;
		private string assemblyName;
		private Assembly imageAssembly;
		private string[] resourceNames;
		private bool isImagesFolderFound = false;
		private Dictionary<string, string> imageToResourceNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, string> imageNamesMap = new Dictionary<string, string>();
		protected override bool FindImageStream(string imageName, out string imageUrl, out Stream imageStream, string customImagesFolder) {
			imageUrl = null;
			imageStream = null;
			if(ImageSourceState == ImageSourceState.Passive) {
				return false;
			}
			bool isCustomFolder = !string.IsNullOrEmpty(customImagesFolder);
			string targetImagesFolder = isCustomFolder ? rootImagesFolder + "." + customImagesFolder : rootImagesFolder;
			if(ImageSourceState == ImageSourceState.Unknown) {
				imageAssembly = ReflectionHelper.LoadAssembly(AssemblyName, "");
				if(imageAssembly == null) {
					Tracing.Tracer.LogWarning("The '{0}' images resource assembly is not found", AssemblyName);
					ImageSourceState = ImageSourceState.Passive;
					return false;
				}
				resourceNames = imageAssembly.GetManifestResourceNames();
				isImagesFolderFound = IsImagesFolderFound(resourceNames, targetImagesFolder);
				if(!isImagesFolderFound && !isCustomFolder) {
					ImageSourceState = ImageSourceState.Passive;
					return false;
				}
				ImageSourceState = ImageSourceState.Active;
				lock(imageNamesMap) {
					BuildImageNamesMap(imageNamesMap, imageAssembly, resourceNames);
				}
			}
			if(imageNamesMap.ContainsKey(imageName)) {
				imageName = imageNamesMap[imageName];
			}
			string targetImageName = isCustomFolder ? customImagesFolder + "." + imageName : imageName;
			string imageResourceName = "";
			lock(imageToResourceNameMap) {
				if(!string.IsNullOrEmpty(imageName) && !imageToResourceNameMap.TryGetValue(targetImageName, out imageResourceName)) {
					imageResourceName = FindImageResourceName(resourceNames, targetImagesFolder, imageName);
					if(isCustomFolder && string.IsNullOrEmpty(imageResourceName)) {
						imageResourceName = FindImageResourceName(resourceNames, rootImagesFolder, imageName);
					}
					imageToResourceNameMap.Add(targetImageName, imageResourceName);
				}
			}
			if(!string.IsNullOrEmpty(imageResourceName)) {
				imageStream = imageAssembly.GetManifestResourceStream(imageResourceName);
				return true;
			}
			return false;
		}
		private static bool IsImagesFolderFound(string[] resourceNames, string imagesFolder) {
			string toFind = "." + imagesFolder + ".";
			foreach(string currentResourceName in resourceNames) {
				if(currentResourceName.Contains(toFind)) {
					return true;
				}
			}
			return false;
		}
		private static void BuildImageNamesMap(Dictionary<string, string> imageNamesMap, Assembly imageAssembly, string[] resourceNames) {
			string imagesMapTableResourceName = ".Resources.ImageNamesMapTable.txt";
			foreach(string resourceName in resourceNames) {
				if(resourceName.EndsWith(imagesMapTableResourceName)) {
					using(Stream imageNamesStream = imageAssembly.GetManifestResourceStream(resourceName)) {
						if(imageNamesStream != null) {
							byte[] buf = new byte[imageNamesStream.Length];
							imageNamesStream.Read(buf, 0, (int)imageNamesStream.Length);
							foreach(string imageNameMapPairString in Encoding.UTF8.GetString(buf).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
								string[] mapPair = imageNameMapPairString.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
								if(mapPair.Length == 2) {
									imageNamesMap.Add(mapPair[0], mapPair[1]);
								}
							}
						}
					}
					break;
				}
			}
		}
		private static string FindImageResourceName(string[] resourceNames, string imagesFolder, string imageName) {
			string resourceImageNameWithoutExtension = "." + imagesFolder + "." + imageName + '.';
			resourceImageNameWithoutExtension = resourceImageNameWithoutExtension.Replace('\\', '.');
			foreach(string currentResourceName in resourceNames) {
				if(currentResourceName.IndexOf(resourceImageNameWithoutExtension, StringComparison.OrdinalIgnoreCase) >= 0) {
					string resourceImageName = Path.ChangeExtension(resourceImageNameWithoutExtension, Path.GetExtension(currentResourceName));
					if(currentResourceName.EndsWith(resourceImageName, StringComparison.OrdinalIgnoreCase)) {
						return currentResourceName;
					}
				}
			}
			return "";
		}
		public AssemblyResourceImageSource(string assemblyName) : this(assemblyName, null) { }
		public AssemblyResourceImageSource(string assemblyName, string imagesFolder) {
			this.assemblyName = assemblyName;
			if(!string.IsNullOrEmpty(imagesFolder)) {
				this.rootImagesFolder = imagesFolder;
			}
		}
		public string AssemblyName {
			get { return assemblyName; }
			set { assemblyName = value; }
		}
		public string AssemblyVersion {
			get { return imageAssembly != null ? AssemblyHelper.GetVersion(imageAssembly).ToString() : ""; }
		}
		public override List<string> GetImageNames() {
			List<string> imageNames = new List<string>();
			Assembly imagesAssembly = ReflectionHelper.LoadAssembly(assemblyName, "");
			if(imagesAssembly == null) {
				try {
					throw new FileNotFoundException(string.Format("The '{0}' assembly is not found", assemblyName), assemblyName); 
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(e); 
					throw;
				}
			}
			foreach(string resourceName in imagesAssembly.GetManifestResourceNames()) {
				string imagePrefix = assemblyName.Replace(XafApplication.CurrentVersion, "") + "." + rootImagesFolder + ".";
				if(resourceName.StartsWith(imagePrefix)) {
					int imageNameStartIndex = resourceName.IndexOf(imagePrefix) + imagePrefix.Length;
					string imageNameWithExtention = resourceName.Substring(imageNameStartIndex, resourceName.Length - imageNameStartIndex);
					string imageName = Path.GetFileNameWithoutExtension(imageNameWithExtention);
					imageNames.Add(imageName);
				}
			}
			return imageNames;
		}
		public override string ToString() {
			string result = GetType().Name + ", " + assemblyName + ", " + rootImagesFolder;
			if(ImageSourceState != ImageSourceState.Unknown) {
				if(imageAssembly == null) {
					result += " - cannot find the assembly";
				}
				else if(!isImagesFolderFound) {
					result += " - cannot find the folder with images";
				}
			}
			else {
				result += " - the source is not initialized";
			}
			return result;
		}
	}
	public class CustomizeImageInfoEventArgs : EventArgs {
		internal const bool MakeTransparentDefaultValue = false; 
		private ImageInfo imageInfo;
		private string imageName;
		private ImageSource imageSource;
		private bool isEnabled;
		private bool makeTransparent;
		private string imageFolder;
		public CustomizeImageInfoEventArgs(ImageInfo imageInfo, string imageName, ImageSource imageSource, string imageFolder)
			: this(imageInfo, imageName, true, imageSource, imageFolder) {
		}
		public CustomizeImageInfoEventArgs(ImageInfo imageInfo, string imageName, bool isEnabled, ImageSource imageSource, string imageFolder) {
			this.imageInfo = imageInfo;
			this.imageName = imageName;
			this.imageSource = imageSource;
			this.isEnabled = isEnabled;
			this.makeTransparent = MakeTransparentDefaultValue;
			this.imageFolder = imageFolder;
		}
		public ImageSource ImageSource {
			get { return imageSource; }
		}
		public ImageInfo ImageInfo {
			get { return imageInfo; }
			set { imageInfo = value; }
		}
		public string ImageName {
			get { return imageName; }
		}
		public bool IsEnabled {
			get { return isEnabled; }
		}
		[DefaultValue(MakeTransparentDefaultValue)]
		public bool MakeTransparent {
			get { return makeTransparent; }
			set { makeTransparent = value; }
		}
		public string ImageFolder {
			get { return imageFolder; }
		}
	}
	public class ImageLoader {
		public const string ThemeImagesFolderKey = "ImagesFolder";
		private static ImageLoader instance;
		private ImageSource[] imageSources;
		private Dictionary<string, ImageInfo> cachedImages = new Dictionary<string, ImageInfo>(StringComparer.OrdinalIgnoreCase);
		private bool MakeTransparentIfBitmap(Image image) {
			Bitmap bitmap = image as Bitmap;
			if(bitmap != null) {
				bitmap.MakeTransparent();
				return true;
			}
			return false;
		}
		protected ImageLoader() { }
		public static bool IsInitialized {
			get {
				return Instance.imageSources != null;
			}
		}
		public static void Init(params ImageSource[] imageSources) {
			if(IsInitialized) {
				throw new ArgumentException("The Instance has already been initialized.");
			}
			Instance.imageSources = imageSources;
		}
		public static void Reset() {
			Instance.imageSources = null;
			Instance.cachedImages = new Dictionary<string, ImageInfo>(StringComparer.OrdinalIgnoreCase);
		}
		public Image LoadFromFullResource(Assembly assembly, string fullResourceName) {
			Image result = null;
			Stream stream = assembly.GetManifestResourceStream(fullResourceName);
			if(stream == null) {
				throw new ArgumentException(string.Format(
					"The '{0}' resource was not found within the '{1}' assembly",
					fullResourceName, assembly.GetName().Name));
			}
			result = Image.FromStream(stream);
			MakeTransparentIfBitmap(result);
			return result;
		}
		public Image LoadFromResource(Assembly assembly, string resourceName) {
			string[] resourceNames = assembly.GetManifestResourceNames();
			foreach(string resName in resourceNames) {
				if(resName.EndsWith(resourceName)) {
					return LoadFromFullResource(assembly, resName);
				}
			}
			throw new ArgumentException(string.Format(
				"The '{0}' resource was not found within the '{1}' assembly",
				resourceName, assembly.GetName().Name));
		}
		public static ImageLoader Instance {
			get {
				if(instance == null) {
					instance = new ImageLoader();
				}
				return instance;
			}
		}
		internal string CustomImageFolder {
			get {
				string result = null;
				if(ValueManager.ValueManagerType != null) {
					IValueManager<string> manager = ValueManager.GetValueManager<string>(ThemeImagesFolderKey);
					if(manager != null) {
						result = manager.Value;
					}
				}
				return result;
			}
		}
		public ImageInfo GetSmallImageInfo(string imageName) {
			return GetSmallImageInfo(imageName, true);
		}
		public ImageInfo GetSmallImageInfo(string imageName, bool isEnabled) {
			return GetSmallImageInfo(imageName, isEnabled, CustomImageFolder);
		}
		public ImageInfo GetSmallImageInfo(string imageName, string imageFolder) {
			return GetSmallImageInfo(imageName, true, imageFolder);
		}
		public ImageInfo GetSmallImageInfo(string imageName, bool isEnabled, string imageFolder) {
			return GetImageInfo(imageName + SmallImageSuffix, isEnabled, imageFolder);
		}
		public ImageInfo GetImageInfo(string imageName) {
			return GetImageInfo(imageName, true);
		}
		public ImageInfo GetImageInfo(string imageName, string imageFolder) {
			return GetImageInfo(imageName, true, imageFolder);
		}
		public ImageInfo GetImageInfo(string imageName, bool isEnabled) {
			return GetImageInfo(imageName, isEnabled, CustomImageFolder);
		}
		public ImageInfo GetImageInfo(string imageName, bool isEnabled, string imageFolder) {
			if(string.IsNullOrEmpty(imageName)
				|| imageName == LargeImageSuffix
				|| imageName == "_Large"
				|| imageName == SmallImageSuffix
				|| imageName == DialogImageSuffix
				|| !IsInitialized) {
				return ImageInfo.Empty;
			}
			ImageInfo result;
			string cachedImageName = imageFolder + imageName + isEnabled.ToString();
			bool makeTransparent = CustomizeImageInfoEventArgs.MakeTransparentDefaultValue;
			lock (cachedImages) {
				if(!cachedImages.TryGetValue(cachedImageName, out result)) {
					StringBuilder parsedSources = new StringBuilder();
					CustomGetImageInfoEventArgs customGetImageInfoEventArgs = new CustomGetImageInfoEventArgs(imageName, isEnabled);
					if(CustomGetImageInfo != null) {
						CustomGetImageInfo(null, customGetImageInfoEventArgs);
					}
					if(customGetImageInfoEventArgs.Handled) { 
						result = customGetImageInfoEventArgs.ImageInfo;
					}
					else {
						foreach(ImageSource imageSource in imageSources) {
							if(imageSource.ImageSourceState != ImageSourceState.Passive) {
								result = imageSource.FindImageInfo(imageName, isEnabled, imageFolder);
								parsedSources.AppendLine(imageSource.ToString());
								if(!result.IsEmpty) {
									if(CustomizeImageSourceImageInfo != null) {
										parsedSources.AppendLine("CustomizeImageInfo event");
										CustomizeImageInfoEventArgs args = new CustomizeImageInfoEventArgs(result, imageName, isEnabled, imageSource, imageFolder);
										CustomizeImageSourceImageInfo(this, args);
										result = args.ImageInfo;
										makeTransparent = args.MakeTransparent;
									}
									if(!result.IsEmpty) {
										break;
									}
								}
							}
						}
					}
					if(!result.IsEmpty && CustomizeImageInfo != null) {
						CustomizeImageInfoEventArgs args = new CustomizeImageInfoEventArgs(result, imageName, isEnabled, null, imageFolder);
						args.MakeTransparent = makeTransparent;
						CustomizeImageInfo(this, args);
						result = args.ImageInfo;
						makeTransparent = args.MakeTransparent;
					}
					if(result.Image != null && !result.IsEmpty && makeTransparent) {
						MakeTransparentIfBitmap(result.Image);
					}
					cachedImages.Add(cachedImageName, result);
					if(result.Image == null) {
						string message = Tracing.Tracer.GetSeparator("ImageLoader: Image is not found by its name in the following sources:");
						message += Tracing.Tracer.GetMessageByValue("ImageName", imageName, true);
						message += Tracing.Tracer.GetMessageByValue("Sources", parsedSources, true);
						Tracing.Tracer.LogVerboseText(message);
					}
				}
				if(string.IsNullOrEmpty(result.ImageUrl)) {
					throw new ArgumentNullException("ImageUrl");
				}
			}
			return result;
		}
		public ImageInfo GetLargeImageInfo(string imageName) {
			return GetLargeImageInfo(imageName, true);
		}
		public ImageInfo GetLargeImageInfo(string imageName, string imageFolder) {
			return GetLargeImageInfo(imageName, true, imageFolder);
		}
		public ImageInfo GetLargeImageInfo(string imageName, bool isEnabled) {
			return GetLargeImageInfo(imageName, isEnabled, CustomImageFolder);
		}
		public ImageInfo GetLargeImageInfo(string imageName, bool isEnabled, string imageFolder) {
			ImageInfo imageInfo = GetImageInfo(imageName + LargeImageSuffix, isEnabled, imageFolder);
			if(imageInfo.IsEmpty) {
				imageInfo = GetImageInfo(imageName + "_Large", isEnabled, imageFolder);
			}
			return imageInfo;
		}
		public ImageInfo GetDialogImageInfo(string imageName) {
			return GetDialogImageInfo(imageName, true);
		}
		public ImageInfo GetDialogImageInfo(string imageName, string imageFolder) {
			return GetDialogImageInfo(imageName, true, imageFolder);
		}
		public ImageInfo GetDialogImageInfo(string imageName, bool isEnabled) {
			return GetDialogImageInfo(imageName, isEnabled, CustomImageFolder);
		}
		public ImageInfo GetDialogImageInfo(string imageName, bool isEnabled, string imageFolder) {
			return GetImageInfo(imageName + DialogImageSuffix, isEnabled, imageFolder);
		}
		public ImageInfo GetEnumValueImageInfo(object enumValue) {
			return GetEnumValueImageInfo(enumValue.GetType(), enumValue);
		}
		public ImageInfo GetEnumValueImageInfo(object enumValue, string imageFolder) {
			return GetEnumValueImageInfo(enumValue.GetType(), enumValue, imageFolder);
		}
		public ImageInfo GetEnumValueImageInfo(Type enumType, object enumValue) {
			return GetEnumValueImageInfo(enumType, enumValue, CustomImageFolder);
		}
		public ImageInfo GetEnumValueImageInfo(Type enumType, object enumValue, string imageFolder) {
			return GetImageInfo(GetEnumValueImageName(enumType, enumValue), imageFolder);
		}
		public string GetEnumValueImageName(object enumValue) {
			return GetEnumValueImageName(enumValue.GetType(), enumValue);
		}
		public string GetEnumValueImageName(Type enumType, object enumValue) {
			if(enumValue == null) {
				throw new ArgumentNullException("enumValue");
			}
			FieldInfo info = enumType.GetField(enumValue.ToString());
			if(info != null) {
				ImageNameAttribute[] attr = (ImageNameAttribute[])info.GetCustomAttributes(typeof(ImageNameAttribute), false);
				if(attr != null & attr.Length == 1) {
					return attr[0].ImageName;
				}
			}
			return enumType.FullName + '\\' + enumValue.ToString();
		}
		public void ClearCache() {
			lock(cachedImages) {
				cachedImages.Clear();
				if(Instance.imageSources != null) {
					foreach(ImageSource source in Instance.imageSources) {
						source.ClearCache();
					}
				}
			}
		}
		public ImageSource[] ImageSources {
			get { return imageSources; }
		}
		public event EventHandler<CustomizeImageInfoEventArgs> CustomizeImageInfo; 
		public event EventHandler<CustomizeImageInfoEventArgs> CustomizeImageSourceImageInfo; 
		public static event EventHandler<CustomGetImageInfoEventArgs> CustomGetImageInfo; 
		public const string SmallImageSuffix = "_12x12";
		public const string LargeImageSuffix = "_32x32";
		public const string DialogImageSuffix = "_48x48";
	}
	public static class ViewImageNameHelper {
		public static string GetImageName(IModelView modelView) {
			if(modelView == null) {
				return string.Empty;
			}
			string imageName = modelView.ImageName;
			if(string.IsNullOrEmpty(imageName)) {
				IModelObjectView modelObjectView = modelView as IModelObjectView;
				if(modelObjectView != null && modelObjectView.ModelClass != null) {
					imageName = modelObjectView.ModelClass.ImageName;
				}
			}
			return imageName;
		}
		public static string GetImageName(View view) {
			if(view != null) {
				return GetImageName(view.Model);
			}
			return string.Empty;
		}
	}
}
