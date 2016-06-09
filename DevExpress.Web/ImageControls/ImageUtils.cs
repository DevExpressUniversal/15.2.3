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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	public delegate void CustomImageProcessingMethod(Graphics graphics, Bitmap thumbnail);
	public class ImageUtils {
		static List<string> ImageFileExtensions = new List<string>(new string[] { ".jpg", ".jpeg", ".bmp", ".png", ".gif", ".tiff", ".tif" });
		public static List<string> GetImageFiles(string directory) {
			List<string> imageFiles = new List<string>();
			foreach(string filePath in Directory.GetFiles(MapPath(directory)))
				if(ImageFileExtensions.Exists(ext => ext == Path.GetExtension(filePath).ToLower()))
					imageFiles.Add(filePath);
			return imageFiles;
		}
		public static string GetAlternateTextByUrl(string url) {
			return string.IsNullOrEmpty(url) ? string.Empty : System.IO.Path.GetFileName(url);
		}
		public static string GetFileExtension(Image image) {
			ImageFormat format = image.RawFormat;
			string fileExtension = ".jpeg";
			if(ImageFormat.Bmp.Equals(format))
				fileExtension = ".bmp";
			else if(ImageFormat.Gif.Equals(format))
				fileExtension = ".gif";
			else if(ImageFormat.Png.Equals(format))
				fileExtension = ".png";
			return fileExtension;
		}
		public static void SaveImage(Image image, ThumbnailInfo thumbInfo, string filePath, CustomImageProcessingMethod customImageProcessingMethod) {
			using(Bitmap thumbnailBitmap = thumbInfo.CreateBitmap(GetPixelFormat(image))) {
				using(Graphics graphics = CreateGraphics(thumbnailBitmap)) {
					DrawImage(graphics, image, thumbInfo.GetRectangle());
					if(customImageProcessingMethod != null)
						customImageProcessingMethod.Invoke(graphics, thumbnailBitmap);
					string extension = Path.GetExtension(filePath).ToLower();
					if(extension == ".jpg" || extension == ".jpeg")
						SaveToJpeg(thumbnailBitmap, filePath);
					else {
						ImageFormat format = ImageFormat.Png;
						if(extension == ".bmp")
							format = ImageFormat.Bmp;
						else if(extension == ".gif")
							format = ImageFormat.Gif;
						thumbnailBitmap.Save(filePath, format);
					}
				}
			}
		}
		public static Bitmap CreateThumbnailImage(Bitmap image, ImageSizeMode sizeMode, Size size) {
			ThumbnailInfo thumbInfo = (ImageUtilsHelper.GetImageResizer(sizeMode)).GetThumbnailInfo(image, size);
			using(Bitmap thumbnailBitmap = thumbInfo.CreateBitmap(GetPixelFormat(image))) {
				using(Graphics graphics = CreateGraphics(thumbnailBitmap)) {
					DrawImage(graphics, image, thumbInfo.GetRectangle());
					MemoryStream ms = new MemoryStream();
					thumbnailBitmap.Save(ms, image.RawFormat);
					return new Bitmap(ms);
				}
			}
		}
		public static ImageCodecInfo GetImageCodecInfo(Bitmap image) {
			ImageCodecInfo myImageCodecInfo = ImageUtils.GetEncoderInfo("image/png");
			if(image.RawFormat.Guid == ImageFormat.Jpeg.Guid)
				myImageCodecInfo = ImageUtils.GetEncoderInfo("image/jpeg");
			else if(image.RawFormat.Guid == ImageFormat.Png.Guid)
				myImageCodecInfo = ImageUtils.GetEncoderInfo("image/png");
			else if(image.RawFormat.Guid == ImageFormat.Bmp.Guid)
				myImageCodecInfo = ImageUtils.GetEncoderInfo("image/bmp");
			else if(image.RawFormat.Guid == ImageFormat.Tiff.Guid)
				myImageCodecInfo = ImageUtils.GetEncoderInfo("image/tiff");
			return myImageCodecInfo;
		}
		public static void CopyImage(string originalImgFilePath, string thumbFilePath) {
			using(FileStream originalFileStream = File.OpenRead(originalImgFilePath)) {
				using(FileStream thumbnailFileStream = new FileStream(thumbFilePath, FileMode.Create)) {
					originalFileStream.CopyTo(thumbnailFileStream);
				}
			}
		}
		public static void SaveToJpeg(Bitmap bitmap, string filePath) {
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
			bitmap.Save(filePath, GetEncoderInfo("image/jpeg"), encoderParameters);
		}
		public static ImageCodecInfo GetEncoderInfo(string mimeType) {
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
			for(int j = 0; j < encoders.Length; ++j) {
				if(encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}
		private static void DrawImage(Graphics graphics, Image image, Rectangle rect) {
			using(Image minifiedImage = new Bitmap(image, rect.Size)) 
				graphics.DrawImage(minifiedImage, rect);
		}
		protected internal static Graphics CreateGraphics(Bitmap bitmap) {
			Graphics g = Graphics.FromImage(bitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			return g;
		}
		private static PixelFormat GetPixelFormat(Image image) {
			return GetPixelFormat(image.PixelFormat);
		}
		protected internal static PixelFormat GetPixelFormat(PixelFormat format) {
			if(CanCreateGraphics(format))
				return format;
			return PixelFormat.Format32bppArgb; 
		}
		private static bool CanCreateGraphics(PixelFormat format) {
			return format == PixelFormat.Format16bppRgb555 ||
				format == PixelFormat.Format16bppRgb565 || format == PixelFormat.Format24bppRgb || format == PixelFormat.Format32bppRgb ||
				format == PixelFormat.Format32bppPArgb ||
				format == PixelFormat.Format48bppRgb || format == PixelFormat.Format64bppPArgb ||
				format == PixelFormat.Format32bppArgb || format == PixelFormat.Format64bppArgb;
		}
		private static string MapPath(string path) {
			return System.Web.Hosting.HostingEnvironment.MapPath(path);
		}
	}
	public enum ThumbnailAction {
		Copy,
		Save
	}
	public class ThumbnailInfo {
		private Size CanvasSize { get; set; }
		private Size Size { get; set; }
		private Point Offset { get; set; }
		public ThumbnailAction Action { get; set; }
		public ThumbnailInfo(Size canvasSize)
			: this(canvasSize, canvasSize, new Point()) {
		}
		public ThumbnailInfo(Size canvasSize, Size size, Point offset) {
			CanvasSize = canvasSize;
			Size = size;
			Offset = offset;
			Action = ThumbnailAction.Save;
		}
		public Bitmap CreateBitmap(PixelFormat pixelFormat) {
			Size bitmapSize = CanvasSize;
			if(Size.Width < bitmapSize.Width || Size.Height < bitmapSize.Height)
				bitmapSize = Size;
			return new Bitmap(bitmapSize.Width, bitmapSize.Height, pixelFormat);
		}
		public Rectangle GetRectangle() {
			return new Rectangle(Offset, Size);
		}
	}
	public class AutogeneratedImageUrls {
		private readonly Dictionary<string, string> Data;
		public Dictionary<string, string>.KeyCollection Keys {
			get { return Data.Keys; }
		}
		public string this[AutogeneratedImageInfo info] {
			get { return Data[info.GetKey()]; }
			set { Data[info.GetKey()] = value; }
		}
		public AutogeneratedImageUrls() {
			Data = new Dictionary<string, string>();
		}
		public void AddImage(AutogeneratedImageInfo info, string url) {
			if(this[info] == null)
				Data.Add(info.GetKey(), url);
			else
				this[info] = url;
		}
	}
	public class AutogeneratedImageInfo {
		public Size Size { get; set; }
		public ImageSizeMode SizeMode { get; set; }
		public bool CreatedFromImage { get; private set; }
		protected internal readonly object CustomData;
		public AutogeneratedImageInfo(Size size, ImageSizeMode sizeMode) {
			Size = size;
			SizeMode = sizeMode;
		}
		public AutogeneratedImageInfo(Size size, ImageSizeMode sizeMode, object customData)
			: this(size, sizeMode) {
			CustomData = customData;
		}
		protected internal string GetKey() {
			return string.Format("{0}_{1}_{2}", Size.Height, Size.Width, (int)SizeMode);
		}
		public static AutogeneratedImageInfo CreateFromImage(OriginalImageBase originalImage) {
			AutogeneratedImageInfo result = new AutogeneratedImageInfo(originalImage.Image.Size, ImageSizeMode.ActualSizeOrFit);
			result.CreatedFromImage = true;
			return result;
		}
	}
	public interface IImageResizer {
		ThumbnailInfo GetThumbnailInfo(Image image, Size size, bool canCopy = false);
	}
	class ActualSizeOrFitResizer : IImageResizer {
		public ThumbnailInfo GetThumbnailInfo(Image image, Size extremSize, bool canCopy = false) {
			if(extremSize.Height == 0)
				extremSize.Height = image.Height;
			if(extremSize.Width == 0)
				extremSize.Width = image.Width;
			if(image.Width > extremSize.Width || image.Height > extremSize.Height)
				return new FitProportionalResizer().GetThumbnailInfo(image, extremSize, canCopy);
			ThumbnailInfo info = new ThumbnailInfo(image.Size);
			info.Action = canCopy ? ThumbnailAction.Copy : ThumbnailAction.Save;
			return info;
		}
	}
	class FillAndCropResizer : IImageResizer {
		public ThumbnailInfo GetThumbnailInfo(Image image, Size extremSize, bool canCopy) {
			int width = extremSize.Width != 0 ? extremSize.Width : image.Width;
			int height = extremSize.Height != 0 ? extremSize.Height : image.Height;
			Size size = new Size();
			Point point = new Point();
			double ratio = (double)image.Width / (double)image.Height;
			size.Width = width;
			size.Height = (int)(size.Width / ratio);
			if(size.Height < height) {
				size.Height = height;
				size.Width = (int)(size.Height * ratio);
			}
			point.X = -(size.Width - width) / 2;
			point.Y = -(size.Height - height) / 2;
			return new ThumbnailInfo(new Size(width, height), size, point);
		}
	}
	class FitProportionalResizer : IImageResizer {
		public ThumbnailInfo GetThumbnailInfo(Image image, Size extremSize, bool canCopy) {
			Size size = new Size();
			double ratio = (double)image.Width / (double)image.Height;
			if(extremSize.Width == 0) {
				double heightRatio = (double)extremSize.Height / (double)image.Height;
				size.Width = (int)(heightRatio * image.Width);
				size.Height = extremSize.Height;
			} else if(extremSize.Height == 0) {
				double widthRatio = (double)extremSize.Width / (double)image.Width;
				size.Height = (int)(widthRatio * image.Height);
				size.Width = extremSize.Width;
			} else {
				size.Width = extremSize.Width;
				size.Height = (int)(size.Width / ratio);
				if(size.Height > extremSize.Height) {
					size.Height = extremSize.Height;
					size.Width = (int)(size.Height * ratio);
				}
			}
			return new ThumbnailInfo(size);
		}
	}
	public class ImageResizeInfo {
		public OriginalImageBase OriginalImage { get; private set; }
		public AutogeneratedImageInfo OutputImageInfo { get; private set; }
		public bool CanOverWrite { get; private set; }
		public string OutputFolder { get; private set; }
		public ImageResizeInfo(OriginalImageBase image, AutogeneratedImageInfo info, string outputFolder, bool overwrite) {
			OriginalImage = image;
			OutputImageInfo = info;
			CanOverWrite = overwrite;
			OutputFolder = outputFolder;
		}
	}
	public interface IThumbnailCreator<TDataType> {
		IEnumerable<AutogeneratedImageUrls> CreateThumbnailsCollection(IEnumerable<TDataType> inputData);
		AutogeneratedImageUrls CreateThumbnails(TDataType inputData);
		string CreateThumbnailImage(TDataType inputData, AutogeneratedImageInfo info);
		bool CanOverwrite { get; set; }
	}
	public interface IImageCreator { 
		string CreateImage(ImageResizeInfo resizeInfo, CustomImageProcessingMethod customImageProcessingMethod = null);
	}
	public abstract class ImageThumbnailCreatorBase<T> : IThumbnailCreator<T> {
		#region helper classes
		protected class CommonImageCreator : IImageCreator {
			protected string ImageSourceFolder { get; private set; }
			public CommonImageCreator(string imageSourceFolder) {
				ImageSourceFolder = imageSourceFolder;
			}
			public string CreateImage(ImageResizeInfo imageResizeInfo, CustomImageProcessingMethod customImageProcessingMethod = null) {
				OriginalImageBase original = imageResizeInfo.OriginalImage;
				AutogeneratedImageInfo imageInfo = imageResizeInfo.OutputImageInfo;
				ImageFileInfo thumbFileInfo = CreateImageFileInfo(original, imageResizeInfo.OutputFolder);
				lock(thumbFileInfo.GetAbsoluteFilePath()) {
					if(CanCreateThumbnail(CreateImageFileInfo(original), thumbFileInfo, imageResizeInfo.CanOverWrite)) {
						IImageResizer resizer = ImageUtilsHelper.GetImageResizer(imageInfo.SizeMode);
						ImageFileInfo originalImgFileInfo = CreateImageFileInfo(original);
						string originalImgFilePath =
							originalImgFileInfo.IsExists() ? originalImgFileInfo.GetAbsoluteFilePath() : string.Empty;
						bool canCopy = !string.IsNullOrEmpty(originalImgFilePath) && customImageProcessingMethod == null;
						ThumbnailInfo info = resizer.GetThumbnailInfo(original.Image, imageInfo.Size, canCopy);
						if(info.Action == ThumbnailAction.Copy)
							ImageUtils.CopyImage(originalImgFilePath, thumbFileInfo.GetAbsoluteFilePath());
						else if(info.Action == ThumbnailAction.Save) {
							if(imageResizeInfo.OutputImageInfo.CreatedFromImage)
								original.Image.Save(thumbFileInfo.GetAbsoluteFilePath());
							else
								ImageUtils.SaveImage(original.Image, info, thumbFileInfo.GetAbsoluteFilePath(), customImageProcessingMethod);
						}
					}
				}
				return thumbFileInfo.GetFilePath();
			}
			protected bool CanCreateThumbnail(ImageFileInfo originalImageFileInfo, ImageFileInfo thumbnailFileInfo, bool overwrite) {
				if(overwrite || !thumbnailFileInfo.IsExists())
					return true;
				return originalImageFileInfo.IsExists() ? originalImageFileInfo > thumbnailFileInfo : false;
			}
			protected ImageFileInfo CreateImageFileInfo(OriginalImageBase originalImage, string rootFolder) {
				return new ImageFileInfo(Path.Combine(rootFolder, originalImage.FileName));
			}
			protected ImageFileInfo CreateImageFileInfo(OriginalImageBase originalImage) {
				if(originalImage is OriginalImage)
					return CreateImageFileInfo(originalImage, ImageSourceFolder);
				return new ImageFileInfo();
			}
		}
		#endregion
		protected IFolderBinder Owner { get; private set; }
		protected IImageCreator ImageCreator { get; private set; }
		public bool CanOverwrite { get; set; }
		protected IEnumerable<AutogeneratedImageInfo> AutogeneratedImageInfoCollection { get; set; }
		public ImageThumbnailCreatorBase(IFolderBinder owner, IImageCreator imageSaver = null) {
			Owner = owner;
			ImageCreator = imageSaver ?? new CommonImageCreator(Owner.ImageSourceFolder);
			AutogeneratedImageInfoCollection = owner.BinderOwner.GetOutputImagesInfo();
		}
		public IEnumerable<AutogeneratedImageUrls> CreateThumbnailsCollection(IEnumerable<T> inputData) {
			foreach(T img in inputData)
				yield return CreateThumbnails(img);
		}
		public AutogeneratedImageUrls CreateThumbnails(T inputData) {
			using(OriginalImageBase image = CreateOriginalImage(inputData)) {
				IEnumerable<ImageResizeInfo> imageResizeInfoCollection = CreateImageResizeInfoCollection(image);
				AutogeneratedImageUrls result = new AutogeneratedImageUrls();
				foreach(ImageResizeInfo imageResizeInfo in imageResizeInfoCollection)
					result[imageResizeInfo.OutputImageInfo] = CreateThumbnail(imageResizeInfo);
				return result;
			}
		}
		public string CreateThumbnailImage(T inputData, AutogeneratedImageInfo info) {
			using(OriginalImageBase image = CreateOriginalImage(inputData)) {
				if(info == null)
					info = AutogeneratedImageInfo.CreateFromImage(image);
				return CreateThumbnail(new ImageResizeInfo(image, info, Owner.GetSubFolder(info), CanOverwrite));
			}
		}
		protected string CreateThumbnail(ImageResizeInfo imageResizeInfo) {
			return ImageCreator.CreateImage(imageResizeInfo, Owner.BinderOwner.GetCustomImageProcessingMethod(imageResizeInfo.OutputImageInfo));
		}
		protected abstract OriginalImageBase CreateOriginalImage(T imageSource);
		protected IEnumerable<ImageResizeInfo> CreateImageResizeInfoCollection(OriginalImageBase image) {
			foreach(AutogeneratedImageInfo imageInfo in AutogeneratedImageInfoCollection) {
				yield return new ImageResizeInfo(image, imageInfo, Owner.GetSubFolder(imageInfo), CanOverwrite);
			}
		}
	}
	public class BinaryImageThumbnailCreator : ImageThumbnailCreatorBase<byte[]> {
		public BinaryImageThumbnailCreator(IFolderBinder owner, IImageCreator imageSaver = null)
			: base(owner, imageSaver) {
		}
		protected override OriginalImageBase CreateOriginalImage(byte[] imageSource) {
			return new BinaryOriginalImage(imageSource);
		}
	}
	public class FileImageThumbnailCreator : ImageThumbnailCreatorBase<string> {
		public FileImageThumbnailCreator(IFolderBinder owner, IImageCreator imageSaver = null)
			: base(owner, imageSaver) {
		}
		protected override OriginalImageBase CreateOriginalImage(string imageSource) {
			return new OriginalImage(() => System.Drawing.Image.FromFile(imageSource), Path.GetFileName(imageSource));
		}
	}
	public class SimpleThumbnailCreator : IThumbnailCreator<string> {
		protected readonly IFolderBinder Owner;
		protected readonly IEnumerable<AutogeneratedImageInfo> AutogeneratedImageInfoCollection;
		public SimpleThumbnailCreator(IFolderBinder owner) {
			Owner = owner;
			AutogeneratedImageInfoCollection = owner.BinderOwner.GetOutputImagesInfo();
		}
		public IEnumerable<AutogeneratedImageUrls> CreateThumbnailsCollection(IEnumerable<string> filePaths) {
			foreach(string filePath in filePaths)
				yield return CreateThumbnails(filePath);
		}
		public AutogeneratedImageUrls CreateThumbnails(string filePath) {
			AutogeneratedImageUrls result = new AutogeneratedImageUrls();
			foreach(AutogeneratedImageInfo info in AutogeneratedImageInfoCollection)
				result[info] = CreateThumbnailImage(filePath, info);
			return result;
		}
		public string CreateThumbnailImage(string filePath, AutogeneratedImageInfo info) {
			return Path.Combine(Owner.ImageSourceFolder, Path.GetFileName(filePath));
		}
		public bool CanOverwrite { get; set; }
	}
	public static class ImageUtilsHelper {
		public static IThumbnailCreator<string> GetThumbnailCreator(IFolderBinder owner, bool canGenerateImages, bool canOverWrite) {
			IThumbnailCreator<string> result = null;
			if(canGenerateImages)
				result = new FileImageThumbnailCreator(owner);
			else
				result = new SimpleThumbnailCreator(owner);
			result.CanOverwrite = canOverWrite;
			return result;
		}
		public static IThumbnailCreator<byte[]> GetThumbnailCreator(IFolderBinder owner, bool canOverWrite) {
			BinaryImageThumbnailCreator result = new BinaryImageThumbnailCreator(owner);
			result.CanOverwrite = canOverWrite;
			return result;
		}
		public static IImageResizer GetImageResizer(ImageSizeMode imageSizeMode) {
			switch(imageSizeMode) {
				case ImageSizeMode.ActualSizeOrFit:
					return new ActualSizeOrFitResizer();
				case ImageSizeMode.FillAndCrop:
					return new FillAndCropResizer();
				case ImageSizeMode.FitProportional:
					return new FitProportionalResizer();
			}
			return null;
		}
		public static string EncodeImageUrl(string imageUrl) {
			if(!string.IsNullOrEmpty(imageUrl) && !imageUrl.StartsWith("http") && !imageUrl.StartsWith("ftp")) {
					string parametr = string.Empty;
					int parametrSeparatorIndex = imageUrl.IndexOf('?');
					if(parametrSeparatorIndex > -1) {
						parametr = imageUrl.Substring(parametrSeparatorIndex);
						imageUrl = imageUrl.Remove(parametrSeparatorIndex);
					}
					return EncodeImageUrlInternal(imageUrl) + parametr;
				} 
			return imageUrl;
		}
		private static string EncodeImageUrlInternal(string imageUrl) {
			bool isPartOfAbsoluteUrl = imageUrl.StartsWith("\\") || imageUrl.StartsWith("/");
			imageUrl = Path.Combine(Path.GetDirectoryName(imageUrl), Path.GetFileName(imageUrl));
			string[] sections = imageUrl.Split(Path.DirectorySeparatorChar);
			for(int i = 0; i < sections.Length; i++)
				EncodePathSection(ref sections[i]);
			return isPartOfAbsoluteUrl ? "\\" + Path.Combine(sections) : Path.Combine(sections);
		}
		private static void EncodePathSection(ref string section) {
			string[] strSplitToSpaces = section.Split(' ');
			section = "";
			for(int i = 0; i < strSplitToSpaces.Length; i++) {
				section += strSplitToSpaces[i] != "~" ? HttpUtility.UrlEncode(strSplitToSpaces[i]) : strSplitToSpaces[i];
				if(i < strSplitToSpaces.Length - 1)
					section += "%20";
			}
		}
	}
}
