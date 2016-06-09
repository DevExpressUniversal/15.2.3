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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Model;
using DevExpress.Compatibility.System.Drawing;
using System.Reflection;
using DevExpress.Compatibility.System.Drawing.Imaging;
using DevExpress.Data.Utils;
using System.Diagnostics;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.Office.Utils {
	#region OfficeImageFormat
	public enum OfficeImageFormat {
		None,
		Bmp,
		Emf,
		Exif,
		Gif,
		Icon,
		Jpeg,
		MemoryBmp,
		Png,
		Tiff,
		Wmf
	}
	#endregion
	#region OfficePixelFormat
	public enum OfficePixelFormat {
		DontCare = 0,
		Undefined = 0,
		Max = 15,
		Indexed = 65536,
		Gdi = 131072,
		Format16bppRgb555 = 135173,
		Format16bppRgb565 = 135174,
		Format24bppRgb = 137224,
		Format32bppRgb = 139273,
		Format1bppIndexed = 196865,
		Format4bppIndexed = 197634,
		Format8bppIndexed = 198659,
		Alpha = 262144,
		Format16bppArgb1555 = 397319,
		PAlpha = 524288,
		Format32bppPArgb = 925707,
		Extended = 1048576,
		Format16bppGrayScale = 1052676,
		Format48bppRgb = 1060876,
		Format64bppPArgb = 1851406,
		Canonical = 2097152,
		Format32bppArgb = 2498570,
		Format64bppArgb = 3424269,
	}
	#endregion
	#region OfficeImage (abstract class)
	[ComVisible(true)]
	public abstract partial class OfficeImage : IDisposable {
		#region CreateContentTypeTable
		static readonly Dictionary<OfficeImageFormat, string> contentTypeTable = CreateContentTypeTable();
		static Dictionary<OfficeImageFormat, string> CreateContentTypeTable() {
			Dictionary<OfficeImageFormat, string> result = new Dictionary<OfficeImageFormat, string>();
			result.Add(OfficeImageFormat.Jpeg, "image/jpeg"); 
			result.Add(OfficeImageFormat.Png, "image/png");
			result.Add(OfficeImageFormat.Bmp, "image/bitmap");
			result.Add(OfficeImageFormat.Tiff, "image/tiff"); 
			result.Add(OfficeImageFormat.Gif, "image/gif"); 
			result.Add(OfficeImageFormat.Icon, "image/x-icon");
			result.Add(OfficeImageFormat.Wmf, "application/x-msmetafile");
			result.Add(OfficeImageFormat.Emf, "application/x-msmetafile");
			return result;
		}
		#endregion
		#region CreateExtensionTable
		static readonly Dictionary<OfficeImageFormat, string> extenstionTable = CreateExtenstionTable();
		static Dictionary<OfficeImageFormat, string> CreateExtenstionTable() {
			Dictionary<OfficeImageFormat, string> result = new Dictionary<OfficeImageFormat, string>();
			result.Add(OfficeImageFormat.Bmp, "bmp");
			result.Add(OfficeImageFormat.Emf, "emf");
			result.Add(OfficeImageFormat.Gif, "gif");
			result.Add(OfficeImageFormat.Icon, "ico");
			result.Add(OfficeImageFormat.Jpeg, "jpg");
			result.Add(OfficeImageFormat.Png, "png");
			result.Add(OfficeImageFormat.Tiff, "tif");
			result.Add(OfficeImageFormat.Wmf, "wmf");
			return result;
		}
		#endregion
		bool suppressStoreToFile;
		string uri = String.Empty;
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageNativeImage")]
#endif
		public abstract Image NativeImage { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageSizeInOriginalUnits")]
#endif
		public abstract Size SizeInOriginalUnits { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageRawFormat")]
#endif
		public abstract OfficeImageFormat RawFormat { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImagePixelFormat")]
#endif
		public abstract OfficePixelFormat PixelFormat { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImagePaletteLength")]
#endif
		public abstract int PaletteLength { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageHorizontalResolution")]
#endif
		public abstract float HorizontalResolution { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageVerticalResolution")]
#endif
		public abstract float VerticalResolution { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageUri")]
#endif
		public virtual string Uri { get { return uri; } set { uri = value; } }
		protected internal virtual bool SuppressStore { get { return suppressStoreToFile; } set { suppressStoreToFile = value; } }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageSizeInPixels")]
#endif
		public Size SizeInPixels { get { return UnitsToPixels(SizeInOriginalUnits); } }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageSizeInHundredthsOfMillimeter")]
#endif
		public Size SizeInHundredthsOfMillimeter { get { return UnitsToHundredthsOfMillimeter(SizeInOriginalUnits); } }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageSizeInDocuments")]
#endif
		public Size SizeInDocuments { get { return UnitsToDocuments(SizeInOriginalUnits); } }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageSizeInTwips")]
#endif
		public Size SizeInTwips { get { return UnitsToTwips(SizeInOriginalUnits); } }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageRootImage")]
#endif
		public virtual OfficeImage RootImage { get { return this; } }
		public abstract OfficeNativeImage EncapsulatedOfficeNativeImage { get; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageIsLoaded")]
#endif
		public virtual bool IsLoaded { get { return true; } }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageDesiredSizeAfterLoad")]
#endif
		public Size DesiredSizeAfterLoad { get; set; }
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageShouldSetDesiredSizeAfterLoad")]
#endif
		public bool ShouldSetDesiredSizeAfterLoad { get; set; }
		public abstract int ImageCacheKey { get; }
		#region Events
		#region NativeImageChanging
		EventHandler onNativeImageChanging;
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageNativeImageChanging")]
#endif
		public event EventHandler NativeImageChanging { add { onNativeImageChanging += value; } remove { onNativeImageChanging -= value; } }
		protected internal virtual void RaiseNativeImageChanging() {
			if (onNativeImageChanging != null)
				onNativeImageChanging(this, EventArgs.Empty);
		}
		#endregion
		#region NativeImageChanged
		NativeImageChangedEventHandler onNativeImageChanged;
#if !SL
	[DevExpressOfficeCoreLocalizedDescription("OfficeImageNativeImageChanged")]
#endif
		public event NativeImageChangedEventHandler NativeImageChanged { add { onNativeImageChanged += value; } remove { onNativeImageChanged -= value; } }
		protected internal virtual void RaiseNativeImageChanged(Size desiredImageSize) {
			if (onNativeImageChanged != null) {
				NativeImageChangedEventArgs args = new NativeImageChangedEventArgs(desiredImageSize);
				onNativeImageChanged(this, args);
			}
		}
		#endregion
		#endregion
		public OfficeImage Clone(IDocumentModel target) {
			OfficeImage result = CreateClone(target);
			result.CopyFrom(this);
			return result;
		}
		protected virtual void CopyFrom(OfficeImage OfficeImage) {
			this.SuppressStore = OfficeImage.SuppressStore;
			this.Uri = OfficeImage.Uri;
		}
		protected abstract OfficeImage CreateClone(IDocumentModel target);
		public abstract bool CanGetImageBytes(OfficeImageFormat imageFormat);
		public virtual void DiscardCachedData() {
		}
		public abstract byte[] GetImageBytes(OfficeImageFormat imageFormat);
		protected internal abstract Stream GetImageBytesStream(OfficeImageFormat imageFormat);
		public virtual byte[] GetImageBytesSafe(OfficeImageFormat imageFormat) {
			if (imageFormat == OfficeImageFormat.MemoryBmp)
				return GetImageBytes(OfficeImageFormat.Png);
			try {
				return GetImageBytes(imageFormat);
			}
			catch {
			}
			return GetImageBytes(OfficeImageFormat.Png);
		}
		public virtual Stream GetImageBytesStreamSafe(OfficeImageFormat imageFormat) {
			try {
				return GetImageBytesStream(imageFormat);
			}
			catch {
			}
			return GetImageBytesStream(OfficeImageFormat.Png);
		}
		public virtual bool IsExportSupported(OfficeImageFormat rawFormat) {
			return true;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		[ComVisible(false)]
		public static string GetContentType(OfficeImageFormat imageFormat) {
			string result;
			if (contentTypeTable.TryGetValue(imageFormat, out result))
				return result;
			else
				return String.Empty;
		}
		[ComVisible(false)]
		public static string GetExtension(OfficeImageFormat imageFormat) {
			string result;
			if (extenstionTable.TryGetValue(imageFormat, out result))
				return result;
			else
				return String.Empty;
		}
		protected internal virtual void EnsureLoadComplete() {
			EnsureLoadComplete(TimeSpan.FromSeconds(5));
		}
		protected internal virtual void EnsureLoadComplete(TimeSpan timeout) {
		}
		protected internal abstract Size UnitsToPixels(Size sizeInUnits);
		protected internal abstract Size UnitsToDocuments(Size sizeInUnits);
		protected internal abstract Size UnitsToHundredthsOfMillimeter(Size sizeInUnits);
		protected internal abstract Size UnitsToTwips(Size sizeInUnits);
		protected internal abstract Size CalculateImageSizeInModelUnits(DocumentModelUnitConverter unitConverter);
	}
	#endregion
	#region OfficeNativeImage (abstract class)
	[ComVisible(true)]
	public abstract partial class OfficeNativeImage : OfficeImage {
		int imageCacheKey;
		IUniqueImageId id;
		public OfficeNativeImage(IUniqueImageId id) {
			Guard.ArgumentNotNull(id, "id");
			this.id = id;
		}
		protected internal IUniqueImageId Id { get { return id; } }
		public override int ImageCacheKey { get { return imageCacheKey; } }
		public override OfficeNativeImage EncapsulatedOfficeNativeImage { get { return this; } }
		public new OfficeNativeImage Clone(IDocumentModel targetModel) {
			return (OfficeNativeImage)base.Clone(targetModel);
		}
		protected internal virtual void SetImageCacheKey(int imageCacheKey) {
			this.imageCacheKey = imageCacheKey;
		}
	}
	#endregion
	#region OfficeReferenceImageBase (abstract class)
	public abstract class OfficeReferenceImageBase<TInnerImage> : OfficeImage where TInnerImage : OfficeImage {
		#region Properties
		protected internal abstract TInnerImage InnerImage { get; }
		public override Image NativeImage { get { return InnerImage.NativeImage; } }
		public override Size SizeInOriginalUnits { get { return InnerImage.SizeInOriginalUnits; } }
		public override OfficeImageFormat RawFormat { get { return InnerImage.RawFormat; } }
		public override OfficePixelFormat PixelFormat { get { return InnerImage.PixelFormat; } }
		public override int PaletteLength { get { return InnerImage.PaletteLength; } }
		public override float HorizontalResolution { get { return InnerImage.HorizontalResolution; } }
		public override float VerticalResolution { get { return InnerImage.VerticalResolution; } }
		public override bool IsLoaded { get { return InnerImage.IsLoaded; } }
		public override int ImageCacheKey { get { return InnerImage.ImageCacheKey; } }
		public override OfficeNativeImage EncapsulatedOfficeNativeImage { get { return InnerImage.EncapsulatedOfficeNativeImage; } }
		#endregion
		public override bool CanGetImageBytes(OfficeImageFormat imageFormat) {
			return InnerImage.CanGetImageBytes(imageFormat);
		}
		public override byte[] GetImageBytes(OfficeImageFormat imageFormat) {
			return InnerImage.GetImageBytes(imageFormat);
		}
		protected internal override Stream GetImageBytesStream(OfficeImageFormat imageFormat) {
			return InnerImage.GetImageBytesStream(imageFormat);
		}
		public override byte[] GetImageBytesSafe(OfficeImageFormat imageFormat) {
			return InnerImage.GetImageBytesSafe(imageFormat);
		}
		public override Stream GetImageBytesStreamSafe(OfficeImageFormat imageFormat) {
			return InnerImage.GetImageBytesStreamSafe(imageFormat);
		}
		protected internal override Size UnitsToPixels(Size sizeInUnits) {
			return InnerImage.UnitsToPixels(sizeInUnits);
		}
		protected internal override Size UnitsToDocuments(Size sizeInUnits) {
			return InnerImage.UnitsToDocuments(sizeInUnits);
		}
		protected internal override Size UnitsToHundredthsOfMillimeter(Size sizeInUnits) {
			return InnerImage.UnitsToHundredthsOfMillimeter(sizeInUnits);
		}
		protected internal override Size UnitsToTwips(Size sizeInUnits) {
			return InnerImage.UnitsToTwips(sizeInUnits);
		}
		protected internal override Size CalculateImageSizeInModelUnits(DocumentModelUnitConverter unitConverter) {
			return InnerImage.CalculateImageSizeInModelUnits(unitConverter);
		}
		protected internal override void EnsureLoadComplete(TimeSpan timeout) {
			InnerImage.EnsureLoadComplete(timeout);
		}
	}
	#endregion
	#region OfficeReferenceImage
	public class OfficeReferenceImage : OfficeReferenceImageBase<OfficeNativeImage> {
		readonly OfficeNativeImage baseImage;
		readonly IDocumentModel owner;
		public OfficeReferenceImage(IDocumentModel owner, OfficeNativeImage baseImage) {
			Guard.ArgumentNotNull(baseImage, "baseImage");
			this.baseImage = baseImage;
			this.owner = owner;
		}
		public override string Uri { get { return InnerImage.Uri; } set { InnerImage.Uri = value; } }
		protected internal override OfficeNativeImage InnerImage { get { return baseImage; } }
		public override OfficeImage RootImage { get { return InnerImage.RootImage; } }
		public OfficeNativeImage NativeRootImage { get { return InnerImage; } }
		public new OfficeReferenceImage Clone(IDocumentModel targetModel) {
			return (OfficeReferenceImage)base.Clone(targetModel);
		}
		protected override OfficeImage CreateClone(IDocumentModel targetModel) {
			if (Object.ReferenceEquals(targetModel, owner) && !Object.ReferenceEquals(owner, null))
				return new OfficeReferenceImage(owner, baseImage);
			else
				return new OfficeReferenceImage(targetModel, baseImage.Clone(targetModel));
		}
	}
	#endregion
	#region OfficeImageHelper
	public static class OfficeImageHelper {
		#region GetRawFormat
		public static OfficeImageFormat GetRtfImageFormat(ImageFormat rawFormat) {
			if (object.Equals(rawFormat, ImageFormat.Bmp)) return OfficeImageFormat.Bmp;
			if (object.Equals(rawFormat, ImageFormat.Emf)) return OfficeImageFormat.Emf;
			if (object.Equals(rawFormat, ImageFormat.Exif)) return OfficeImageFormat.Exif;
			if (object.Equals(rawFormat, ImageFormat.Gif)) return OfficeImageFormat.Gif;
			if (object.Equals(rawFormat, ImageFormat.Icon)) return OfficeImageFormat.Icon;
			if (object.Equals(rawFormat, ImageFormat.Jpeg)) return OfficeImageFormat.Jpeg;
			if (object.Equals(rawFormat, ImageFormat.MemoryBmp)) return OfficeImageFormat.MemoryBmp;
			if (object.Equals(rawFormat, ImageFormat.Png)) return OfficeImageFormat.Png;
			if (object.Equals(rawFormat, ImageFormat.Tiff)) return OfficeImageFormat.Tiff;
			if (object.Equals(rawFormat, ImageFormat.Wmf)) return OfficeImageFormat.Wmf;
			return OfficeImageFormat.None;
		}
		public static ImageFormat GetImageFormat(OfficeImageFormat rawFormat) {
			switch (rawFormat) {
				case OfficeImageFormat.Bmp: return ImageFormat.Bmp;
				case OfficeImageFormat.Emf: return ImageFormat.Emf;
				case OfficeImageFormat.Exif: return ImageFormat.Exif;
				case OfficeImageFormat.Gif: return ImageFormat.Gif;
				case OfficeImageFormat.Icon: return ImageFormat.Icon;
				case OfficeImageFormat.Jpeg: return ImageFormat.Jpeg;
				case OfficeImageFormat.MemoryBmp: return ImageFormat.MemoryBmp;
				case OfficeImageFormat.Png: return ImageFormat.Png;
				case OfficeImageFormat.Tiff: return ImageFormat.Tiff;
				case OfficeImageFormat.Wmf: return ImageFormat.Wmf;
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region MemoryStreamBasedImage
	public class MemoryStreamBasedImage {
		readonly Image image;
		readonly MemoryStream imageStream;
		readonly IUniqueImageId id;
		public MemoryStreamBasedImage(Image image, MemoryStream imageStream) : this(image, imageStream, new NativeImageId(image)) { }
		public MemoryStreamBasedImage(Image image, MemoryStream imageStream, IUniqueImageId id) {
			Guard.ArgumentNotNull(id, "id");
			this.id = id;
			this.image = image;
			this.imageStream = imageStream;
		}
		public IUniqueImageId Id { get { return id; } }
		public Image Image { get { return image; } }
		public MemoryStream ImageStream { get { return imageStream; } }
	}
	#endregion
	#region ImageLoaderHelper
	public static class ImageLoaderHelper {
		public static MemoryStreamBasedImage ImageFromStream(Stream stream) {
			return ImageFromStream(stream, null);
		}
		public static MemoryStreamBasedImage ImageFromStream(Stream stream, IUniqueImageId imageId) {
			MemoryStream imageStream = GetMemoryStream(stream, -1);
#if DXPORTABLE
			Image image = Image.FromStream(imageStream);
#else
			Image image = ImageTool.ImageFromStream(imageStream);
#endif
			return imageId != null ? new MemoryStreamBasedImage(image, imageStream, imageId)
				: new MemoryStreamBasedImage(image, imageStream);
		}
		public static MemoryStreamBasedImage ImageFromFile(string filename) {
			return ImageFromStream(GetStream(filename));
		}
		static Stream GetStream(string fileName) {
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				MemoryStream result = new MemoryStream();
				stream.CopyTo(result);
				result.Position = 0;
				return result;
			}
		}
		public static MemoryStream GetMemoryStream(Stream stream, int length) {
			int count = length < 0 ? (int)stream.Length : length;
			byte[] buffer = new byte[count];
			long position = stream.CanSeek ? stream.Position : -1;
			stream.Read(buffer, 0, count);
			if (stream.CanSeek)
				stream.Position = position;
			MemoryStream result = new MemoryStream(buffer);
			return result;
		}
	}
#endregion
#region DibHelper
	public sealed class DibHelper {
		DibHelper() {
		}
		static MemoryStream CreateBmpFileStreamForDib(Stream dibStream, int dibHeight, int bytesInLine) {
			MemoryStream result = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(result);
			try {
				int dibDataSize = (int)dibStream.Length;
				int fileSize = BitmapFileHelper.sizeofBITMAPFILEHEADER + dibDataSize;
				int bitsSize = dibHeight * bytesInLine;
				int offset = dibDataSize - bitsSize + BitmapFileHelper.sizeofBITMAPFILEHEADER;
				BitmapFileHelper.WriteBITMAPFILEHEADER(writer, fileSize, offset);
				StreamHelper.WriteTo(dibStream, result);
#if DEBUGTEST
				Debug.Assert(result.Length == fileSize);
#endif
			}
			finally {
				writer.Dispose();
			}
			return new MemoryStream(result.GetBuffer());
		}
		public static MemoryStreamBasedImage CreateDib(Stream stream, int width, int height, int bytesInLine) {
			return CreateDib(stream, width, height, bytesInLine, null);
		}
		public static MemoryStreamBasedImage CreateDib(Stream stream, int width, int height, int bytesInLine, IUniqueImageId imageId) {
			MemoryStream bmpFileStream = CreateBmpFileStreamForDib(stream, height, bytesInLine);
			MemoryStreamBasedImage result = ImageLoaderHelper.ImageFromStream(bmpFileStream, imageId);
			Debug.Assert(result.Image.Width == width);
			Debug.Assert(result.Image.Height == height);
			return result;
		}
	}
#endregion
#region BitmapFileHelper
	public sealed class BitmapFileHelper {
		public const int sizeofBITMAPFILEHEADER = 14;
		public const int sizeofBITMAPINFOHEADER = 40;
		BitmapFileHelper() {
		}
		public static void WriteBITMAPFILEHEADER(BinaryWriter writer, int fileSize, int bitsOffset) {
			writer.Write((System.Byte)66);		  
			writer.Write((System.Byte)77);		  
			writer.Write((System.Int32)fileSize);   
			writer.Write((System.Int16)0);		  
			writer.Write((System.Int16)0);		  
			writer.Write((System.Int32)bitsOffset);  
		}
		public static void WriteBITMAPINFOHEADER(BinaryWriter writer, int width, int height, int colorPlanesCount, int bitsPerPixel, int bytesInLine, Color[] palette) {
			int bitsSize = height * bytesInLine;
			writer.Write((System.Int32)sizeofBITMAPINFOHEADER); 
			writer.Write((System.Int32)width);				  
			writer.Write((System.Int32)height);				 
			writer.Write((System.Int16)colorPlanesCount);	   
			writer.Write((System.Int16)bitsPerPixel);		   
			writer.Write((System.Int32)0);					  
			writer.Write((System.Int32)bitsSize);			   
			writer.Write((System.Int32)0);					  
			writer.Write((System.Int32)0);					  
			writer.Write((System.Int32)palette.Length);		 
			writer.Write((System.Int32)0);					  
		}
		public static void WritePalette(BinaryWriter writer, Color[] palette) {
			int count = palette.Length;
			for (int i = 0; i < count; i++) {
				Color c = palette[i];
				writer.Write((System.Byte)c.B);
				writer.Write((System.Byte)c.G);
				writer.Write((System.Byte)c.R);
				writer.Write((System.Byte)0);
			}
		}
	}
#endregion
#region PaletteHelper
	public sealed class PaletteHelper {
		static Color[] palette0 = new Color[] { };
		static Color[] palette2 = PreparePalette2();
		static Color[] palette16 = PreparePalette16();
		static Color[] palette256 = PreparePalette256();
		PaletteHelper() {
		}
		public static Color[] GetPalette(int bitsPerPixel) {
			if (bitsPerPixel == 8)
				return palette256;
			else if (bitsPerPixel == 4)
				return palette16;
			else if (bitsPerPixel == 1)
				return palette2;
			else
				return palette0;
		}
		static Color[] PreparePalette256() {
			Color[] result = PreparePalette(PixelFormat.Format8bppIndexed);
#if DEBUGTEST
			Debug.Assert(256 == result.Length);
#endif
			return result;
		}
		static Color[] PreparePalette16() {
			Color[] result = PreparePalette(PixelFormat.Format4bppIndexed);
#if DEBUGTEST
			Debug.Assert(16 == result.Length);
#endif
			return result;
		}
		static Color[] PreparePalette2() {
			Color[] result = PreparePalette(PixelFormat.Format1bppIndexed);
#if DEBUGTEST
			Debug.Assert(2 == result.Length);
#endif
			return result;
		}
		static Color[] PreparePalette(PixelFormat pixelFormat) {
			using (Bitmap bmp = new Bitmap(10, 10, pixelFormat)) {
				return bmp.Palette.Entries;
			}
		}
	}
#endregion
#region BitmapHelper
	public sealed class BitmapHelper {
		BitmapHelper() {
		}
		public static MemoryStreamBasedImage CreateBitmap(MemoryStream stream, int width, int height, int colorPlanesCount, int bitsPerPixel, int bytesInLine) {
			return CreateBitmap(stream, width, height, colorPlanesCount, bitsPerPixel, bytesInLine);
		}
		public static MemoryStreamBasedImage CreateBitmap(MemoryStream stream, int width, int height, int colorPlanesCount, int bitsPerPixel, int bytesInLine, IUniqueImageId imageId) {
			MemoryStream result = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(result);
			try {
				Color[] palette = PaletteHelper.GetPalette(bitsPerPixel);
				int headerSize = BitmapFileHelper.sizeofBITMAPFILEHEADER + BitmapFileHelper.sizeofBITMAPINFOHEADER;
				int paletteSize = palette.Length * 4;
				int fileSize = headerSize + paletteSize + (int)stream.Length;
				int offset = headerSize + paletteSize;
#if DEBUGTEST
				Debug.Assert(stream.Length == height * bytesInLine);
#endif
				BitmapFileHelper.WriteBITMAPFILEHEADER(writer, fileSize, offset);
				BitmapFileHelper.WriteBITMAPINFOHEADER(writer, width, height, colorPlanesCount, bitsPerPixel, bytesInLine, palette);
				BitmapFileHelper.WritePalette(writer, palette);
				stream.WriteTo(result);
#if DEBUGTEST
				Debug.Assert(result.Length == fileSize);
#endif
			}
			finally {
				writer.Dispose();
			}
			result = new MemoryStream(result.GetBuffer());
			return ImageLoaderHelper.ImageFromStream(result, imageId);
		}
	}
#endregion
#region StreamHelper
	public static class StreamHelper {
		public static void WriteTo(Stream inputStream, Stream outputStream) {
			byte[] buf = new byte[1024];
			int readedByteCount;
			for (;;) {
				readedByteCount = inputStream.Read(buf, 0, buf.Length);
				if (readedByteCount == 0) break;
				outputStream.Write(buf, 0, readedByteCount);
			}
		}
	}
#endregion
	public interface IDesiredSizeSupport {
		Size DesiredSize { get; }
	}
#region UriBasedOfficeImageBase
	public abstract class UriBasedOfficeImageBase : OfficeReferenceImageBase<OfficeReferenceImage>, IDesiredSizeSupport {
#region Fields
		OfficeReferenceImage innerImage;
		readonly int pixelTargetWidth;
		readonly int pixelTargetHeight;
		bool isLoaded;
		bool loadFailed;
#endregion
		protected UriBasedOfficeImageBase(int pixelTargetWidth, int pixelTargetHeight) {
			this.pixelTargetWidth = pixelTargetWidth;
			this.pixelTargetHeight = pixelTargetHeight;
		}
#region Properties
		protected internal override OfficeReferenceImage InnerImage { get { return innerImage; } }   
		public override bool IsLoaded { get { return isLoaded; } }
		public bool LoadFailed { get { return loadFailed; } protected set { loadFailed = value; } }
		public int PixelTargetWidth { get { return pixelTargetWidth; } }
		public int PixelTargetHeight { get { return pixelTargetHeight; } }
		protected internal abstract IDocumentModel DocumentModel { get; }
#endregion
		protected void CreatePlaceHolder() {
			this.innerImage = CreatePlaceHolder(DocumentModel, pixelTargetWidth, pixelTargetHeight);
		}
		public static OfficeReferenceImage CreatePlaceHolder(IDocumentModel documentModel, int pixelTargetWidth, int pixelTargetHeight) {
#if SL || DXPORTABLE
#if DXPORTABLE
			const string imageUri = "DevExpress.Office.Core.Images.ImagePlaceHolder.png";
#else
			const string imageUri = "DevExpress.Office.Images.ImagePlaceHolder.png";
#endif
			Assembly assembly = typeof(UriBasedOfficeImageBase).GetAssembly();
			Stream stream = assembly.GetManifestResourceStream(imageUri);
			if (documentModel == null)
				return new OfficeReferenceImage(null, OfficeImage.CreateImage(stream));
			else
				return documentModel.CreateImage(stream);
#else
			const string imageUri = "DevExpress.Office.Images.ImagePlaceHolder.png";
			Image nativeImage = DevExpress.Utils.CommandResourceImageLoader.CreateBitmapFromResources(imageUri, System.Reflection.Assembly.GetExecutingAssembly());
			int actualPixelWidth = pixelTargetWidth;
			if (actualPixelWidth <= 0)
				actualPixelWidth = nativeImage.Width;
			int actualPixelHeight = pixelTargetHeight;
			if (actualPixelHeight <= 0)
				actualPixelHeight = nativeImage.Height;
			if (actualPixelWidth == nativeImage.Width && actualPixelHeight == nativeImage.Height) {
				if (documentModel != null)
					return documentModel.CreateImage(new MemoryStreamBasedImage(nativeImage, null));
				else
					return new OfficeReferenceImage(null, OfficeImage.CreateImage(new MemoryStreamBasedImage(nativeImage, null)));
			}
			Bitmap bitmap = new Bitmap(actualPixelWidth, actualPixelHeight);
			using (Graphics gr = Graphics.FromImage(bitmap)) {
				gr.DrawImage(nativeImage, new Rectangle(Point.Empty, nativeImage.Size));
				gr.DrawRectangle(Pens.Black, new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1));
			}
			if (documentModel != null)
				return documentModel.CreateImage(new MemoryStreamBasedImage(bitmap, null));
			else
				return new OfficeReferenceImage(null, OfficeImage.CreateImage(new MemoryStreamBasedImage(bitmap, null)));
#endif
		}
		protected internal virtual void ReplaceInnerImage(Stream imageStream) {
			OfficeReferenceImage image;
			try {
				image = DocumentModel.CreateImage(imageStream);
			}
			catch {
				image = null;
			}
			if (image != null) {
				this.isLoaded = true;
				image.Uri = this.Uri;
				ReplaceInnerImage(image);
			}
			else
				this.loadFailed = true;
		}
		protected internal virtual void ReplaceInnerImage(OfficeReferenceImage image) {
			RaiseNativeImageChanging();
			innerImage.Dispose();
			innerImage = image;
			AfterSetInnerImage();
		}
		protected internal void RaiseEventsWhenImageLoaded() {
			RaiseNativeImageChanging();
			AfterSetInnerImage();			
		}
		void AfterSetInnerImage() {
			RaiseNativeImageChanged(GetDesiredSizeInTwips());
		}
		Size GetDesiredSizeInTwips() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			return Units.PixelsToTwips(new Size(PixelTargetWidth, PixelTargetHeight), unitConverter.ScreenDpiX, unitConverter.ScreenDpiY);
		}
		protected internal override void EnsureLoadComplete(TimeSpan timeout) {
			lock (this) {
				LoadActualImageSynchronous();
			}
		}
		protected internal abstract void LoadActualImageSynchronous();
#region IDesiredSizeSupport implementation
		Size IDesiredSizeSupport.DesiredSize { get { return GetDesiredSizeInTwips(); } }
#endregion
	}
#endregion
#region UriBasedOfficeImage
	public class UriBasedOfficeImage : UriBasedOfficeImageBase {
#region Fields
		readonly IDocumentModel documentModel;
		readonly IUriStreamService service;
		readonly IThreadSyncService threadSyncService;
		readonly string[] uriList;
		readonly bool originalAsyncImageLoading;
		bool suppressStorePlaceholder;
#endregion
		public UriBasedOfficeImage(string uri, int pixelTargetWidth, int pixelTargetHeight, IDocumentModel documentModel, bool asyncImageLoading)
			: this(new string[] { uri }, pixelTargetWidth, pixelTargetHeight, documentModel, asyncImageLoading) {
		}
		public UriBasedOfficeImage(string[] uriList, int pixelTargetWidth, int pixelTargetHeight, IDocumentModel documentModel, bool asyncImageLoading)
			: base(pixelTargetWidth, pixelTargetHeight) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(uriList, "uriList");
			Guard.ArgumentPositive(uriList.Length, "uriList.Count");
			this.uriList = uriList;
			this.documentModel = documentModel;
			this.Uri = uriList[0];
			originalAsyncImageLoading = asyncImageLoading;
			CreatePlaceHolder();
			this.threadSyncService = documentModel.GetService<IThreadSyncService>();
			if (threadSyncService == null)
				asyncImageLoading = false;
			this.service = documentModel.GetService<IUriStreamService>();
			if (service != null) {
				if (asyncImageLoading) {
					IThreadPoolService threadPoolService = documentModel.GetService<IThreadPoolService>();
					if (threadPoolService != null)
						threadPoolService.QueueJob(LoadActualImage);
					else
						LoadActualImage(asyncImageLoading);
				}
				else
					LoadActualImage(asyncImageLoading);
			}
		}
		protected internal bool SuppressStorePlaceholder { get { return suppressStorePlaceholder; } set { suppressStorePlaceholder = value; } }
		protected internal override bool SuppressStore { get { return base.SuppressStore || (SuppressStorePlaceholder && !IsLoaded); } set { base.SuppressStore = value; } }
		protected internal override IDocumentModel DocumentModel { get { return documentModel; } }
#region Loaded
		EventHandler onLoaded;
		public event EventHandler Loaded { add { onLoaded += value; } remove { onLoaded -= value; } }
		protected internal virtual void RaiseLoaded() {
			if (onLoaded != null)
				onLoaded(this, EventArgs.Empty);
		}
#endregion
		protected override OfficeImage CreateClone(IDocumentModel targetModel) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			if (IsLoaded)
				return InnerImage.Clone(targetModel);
			else {
				if (Object.ReferenceEquals(targetModel, DocumentModel))
					return new UriBasedOfficeReferenceImage(this, PixelTargetWidth, PixelTargetHeight);
				else
					return new UriBasedOfficeImage(Uri, PixelTargetWidth, PixelTargetHeight, targetModel, originalAsyncImageLoading);
			}
		}
		protected internal virtual void LoadActualImage(object state) {
			int count = uriList.Length;
			for (int i = 0; i < count; i++) {
				this.Uri = uriList[i];
				Stream stream = service.GetStream(Uri);
				if (stream != null) {
					if (stream.CanSeek)
						stream.Seek(0, SeekOrigin.Begin);
					bool asyncImageLoading = (state is bool) ? (bool)state : true;
					if (asyncImageLoading) {
						this.DocumentModel.UriBasedImageReplaceQueue.Add(this, stream);
						threadSyncService.EnqueueInvokeInUIThread(delegate() {  ProcessRegisteredInnerImageReplacements(); });
					}
					else
						ReplaceInnerImage(stream);
					break;
				}
				else
					this.LoadFailed = true;
			}
		}
		protected internal virtual void ProcessRegisteredInnerImageReplacements() {
			if (this.DocumentModel.IsDisposed)
				return;
			this.DocumentModel.UriBasedImageReplaceQueue.ProcessRegisteredImages();
		}
		protected internal override void ReplaceInnerImage(Stream imageStream) {
			lock (this) {
				if (IsLoaded)
					return;
				base.ReplaceInnerImage(imageStream);
				if (IsLoaded)
					RaiseLoaded();
			}
		}
		protected internal override void LoadActualImageSynchronous() {
			lock (this) {
				if (IsLoaded || LoadFailed)
					return;
				if (DocumentModel.UriBasedImageReplaceQueue.ForceImageProcess(this))
					return;
				Stream stream = service.GetStream(Uri);
				if (stream != null)
				{
					if (stream.CanSeek)
						stream.Seek(0, SeekOrigin.Begin);
					ReplaceInnerImage(stream);
				}
			}
		}
	}
#endregion
#region UriBasedOfficeReferenceImage
	public class UriBasedOfficeReferenceImage : UriBasedOfficeImageBase {
		readonly UriBasedOfficeImage baseImage;
		public UriBasedOfficeReferenceImage(UriBasedOfficeImage baseImage, int pixelTargetWidth, int pixelTargetHeight)
			: base(pixelTargetWidth, pixelTargetHeight) {
			Guard.ArgumentNotNull(baseImage, "baseImage");
			this.Uri = baseImage.Uri;
			this.baseImage = baseImage;
			CreatePlaceHolder();
			this.baseImage.Loaded += OnBaseImageLoaded;
		}
		public override bool IsLoaded { get { return baseImage.IsLoaded; } }
		protected internal override OfficeReferenceImage InnerImage { get { return IsLoaded ? baseImage.InnerImage : base.InnerImage; } }
		protected internal override IDocumentModel DocumentModel { get { return baseImage.DocumentModel; } }
		void OnBaseImageLoaded(object sender, EventArgs e) {
			ReplaceInnerImage(baseImage.InnerImage.Clone(baseImage.DocumentModel));
			this.baseImage.Loaded -= OnBaseImageLoaded;
		}
		protected override OfficeImage CreateClone(IDocumentModel targetModel) {
			return baseImage.Clone(targetModel);
		}
		protected internal override void EnsureLoadComplete(TimeSpan timeout) {
			baseImage.EnsureLoadComplete(timeout);
		}
		protected internal override void LoadActualImageSynchronous() {
		}
	}
#endregion
#region UriBasedImageReplaceQueue
	public class UriBasedImageReplaceQueue {
#region UriBasedImageStreamPair
		struct UriBasedImageStreamPair {
			UriBasedOfficeImage image;
			Stream stream;
			public UriBasedImageStreamPair(UriBasedOfficeImage image, Stream stream) {
				this.image = image;
				this.stream = stream;
			}
			public UriBasedOfficeImage Image { get { return image; } }
			public Stream Stream { get { return stream; } }
		}
#endregion
		List<UriBasedImageStreamPair> list;
		IBatchUpdateable owner;
		public UriBasedImageReplaceQueue(IBatchUpdateable owner) {
			this.list = new List<UriBasedImageStreamPair>();
			this.owner = owner;
		}
		public void Add(UriBasedOfficeImage image, Stream stream) {
			lock (list) {
				list.Add(new UriBasedImageStreamPair(image, stream));
			}
		}
		public bool ForceImageProcess(UriBasedOfficeImage image) {
			Stream stream = null;
			lock (list) {
				int index = IndexOf(image);
				if (index < 0)
					return false;
				stream = list[index].Stream;
				list.RemoveAt(index);
			}
			owner.BeginUpdate();
			try {
				image.ReplaceInnerImage(stream);
			}
			finally {
				owner.EndUpdate();
			}
			return true;
		}
		int IndexOf(UriBasedOfficeImage image) {
			for (int i = 0; i < list.Count; i++) {
				UriBasedImageStreamPair item = list[i];
				if (Object.ReferenceEquals(item.Image, image))
					return i;
			}
			return -1;
		}
		public void ProcessRegisteredImages() {
			List<UriBasedImageStreamPair> copy;
			lock (list) {
				if (list.Count == 0)
					return;
				copy = new List<UriBasedImageStreamPair>(list);
				list.Clear();
			}
			owner.BeginUpdate();
			for (int i = 0; i < copy.Count; i++) {
				UriBasedImageStreamPair item = copy[i];
				item.Image.ReplaceInnerImage(item.Stream);
			}
			owner.EndUpdate();
		}
	}
#endregion
#region NativeImageChangedEventHanlder
	public delegate void NativeImageChangedEventHandler(object sender, NativeImageChangedEventArgs e);
#endregion
#region NativeImageChangedEventArgs
	public class NativeImageChangedEventArgs : EventArgs {
		readonly Size desiredImageSizeInTwips;
		public NativeImageChangedEventArgs(Size desiredImageSizeInTwips) {
			this.desiredImageSizeInTwips = desiredImageSizeInTwips;
		}
		public Size DesiredImageSizeInTwips { get { return desiredImageSizeInTwips; } }
	}
#endregion
#region ImageScaleCalculator
	public static class ImageScaleCalculator {
		public static float GetScale(int desired, int originalInModelUnits, float defaultScale) {
			float result = defaultScale;
			if (desired > 0)
				result = Math.Max(1, 100f * desired / Math.Max(1, originalInModelUnits));
			return Math.Max(1, result);
		}
		public static SizeF GetScale(Size desiredSize, Size originalSize, float defaultScale) {
			float scaleX = GetScale(desiredSize.Width, originalSize.Width, defaultScale);
			float scaleY = GetScale(desiredSize.Height, originalSize.Height, defaultScale);
			return new SizeF(scaleX, scaleY);
		}
		public static Size GetDesiredImageSizeInModelUnits(Size desiredSizeInPixels, DocumentModelUnitConverter unitConverter) {
			return unitConverter.PixelsToModelUnits(desiredSizeInPixels);
		}
		public static Size GetDesiredImageSizeInModelUnits(int widthInPixels, int heightInPixels) {
			return Units.PixelsToTwips(new Size(widthInPixels, heightInPixels), DocumentModelBase<int>.DpiX, DocumentModelBase<int>.DpiY);
		}
	}
#endregion
}
namespace DevExpress.Office.Utils.Internal {
	public class InternalOfficeImageHelper {
		public static Size CalculateImageSizeInModelUnits(OfficeImage instance, DocumentModelUnitConverter unitConverter) {
			return instance.CalculateImageSizeInModelUnits(unitConverter);
		}
		public static void EnsureLoadComplete(OfficeImage instance) {
			instance.EnsureLoadComplete();
		}
		public static bool GetSuppressStore(OfficeImage instance) {
			return instance.SuppressStore;
		}
		public static void SetSuppressStore(OfficeImage instance, bool value) {
			instance.SuppressStore = value;
		}
		public static Stream GetImageBytesStream(OfficeImage instance, OfficeImageFormat imageFormat) {
			return instance.GetImageBytesStream(imageFormat);
		}
	}
	public class InternalUriBasedOfficeImageBaseHelper {
		public static void RaiseEventsWhenImageLoaded(UriBasedOfficeImageBase instance) {
			instance.RaiseEventsWhenImageLoaded();
		}
	}
	public class InternalUriBasedOfficeImageHelper {
		public static void SetSuppressStorePlaceholder(UriBasedOfficeImage instance, bool value) {
			instance.SuppressStorePlaceholder = value;
		}
	}
}
