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
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using DevExpress.Utils;
using DevExpress.Office.PInvoke;
using DevExpress.Data.Utils;
using DevExpress.Office.Model;
namespace DevExpress.Office.Utils {
	#region OfficeImage
	public partial class OfficeImage {
		[ComVisible(false)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800")]
		public static OfficeNativeImage CreateImage(Image nativeImage) {
			return CreateImage(new MemoryStreamBasedImage(nativeImage, null));
		}
		public static OfficeNativeImage CreateImage(MemoryStreamBasedImage streamBasedImage) {
			Image nativeImage = streamBasedImage.Image;
			if (nativeImage.RawFormat.Equals(ImageFormat.Wmf))
				return new OfficeWmfImageWin((Metafile)nativeImage, streamBasedImage.Id);
			else if (nativeImage.RawFormat.Equals(ImageFormat.Emf))
				return new OfficeEmfImageWin((Metafile)nativeImage, streamBasedImage.Id);
			else
				return new OfficeImageWin(streamBasedImage.Image, streamBasedImage.ImageStream, streamBasedImage.Id);
		}
		[ComVisible(false)]
		public static OfficeNativeImage CreateImage(Stream stream) {
			return CreateImage(stream, -1, null);
		}
		[ComVisible(false)]
		public static OfficeNativeImage CreateImage(Stream stream, IUniqueImageId id) {
			return CreateImage(stream, -1, id);
		}
		static OfficeNativeImage CreateImage(Stream stream, int length, IUniqueImageId id) {
			MemoryStream ms = ImageLoaderHelper.GetMemoryStream(stream, length);
			MemoryStreamBasedImage image;
			try {
				image = ImageLoaderHelper.ImageFromStream(ms);
			}
			catch(ArgumentException) {
				ms.Position = 0;
				if (!(ms.Length >= 2 && ms.ReadByte() == 0x1F && ms.ReadByte() == 0x8B))
					throw;
				ms.Position = 0;
				MemoryStream decompressedStream = new MemoryStream();
				using(System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress, false)) {
					byte[] buffer = new byte[4096];
					while (true) {
						int count = gzipStream.Read(buffer, 0, buffer.Length);
						if (count <= 0)
							break;
						decompressedStream.Write(buffer, 0, count);
					}
				}
				decompressedStream.Position = 0;
				image = ImageLoaderHelper.ImageFromStream(decompressedStream);
			}
			image = new MemoryStreamBasedImage(image.Image, image.ImageStream, id ?? new NativeImageId(image.Image));
			return CreateImage(image);
		}
	}
	#endregion
	#region OfficeImageWin
	public class OfficeImageWin : OfficeNativeImage {
		#region imageEncodersInfo
		static Dictionary<Guid, ImageCodecInfo> imageEncodersInfo = CreateImageEncodersInfo();
		static Dictionary<Guid, ImageCodecInfo> CreateImageEncodersInfo() {
			Dictionary<Guid, ImageCodecInfo> result = new Dictionary<Guid, ImageCodecInfo>();
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
			int count = encoders.Length;
			for (int i = 0; i < count; i++) {
				ImageCodecInfo info = encoders[i];
				Guid formatId = info.FormatID;
				result.Add(formatId, info);
			}
			return result;
		}
		#endregion
		Image nativeImage;
		MemoryStream imageStream;
		public OfficeImageWin(Image image, IUniqueImageId id) : this(image, null, id) { }
		public OfficeImageWin(Image image, MemoryStream imageStream, IUniqueImageId id)
			: base(id) {
			Guard.ArgumentNotNull(image, "image");
			this.SetNativeImage(image, imageStream);
		}
		#region Properties
		public override Image NativeImage { get { return nativeImage; } }
		public override OfficeImageFormat RawFormat { get { return OfficeImageHelper.GetRtfImageFormat(NativeImage.RawFormat); } }
		public override int PaletteLength { get { return NativeImage.Palette.Entries.Length; } }
		public override Size SizeInOriginalUnits { get { return NativeImage.Size; } }
		public override OfficePixelFormat PixelFormat { get { return (OfficePixelFormat)NativeImage.PixelFormat; } }
		public override float HorizontalResolution { get { return NativeImage.HorizontalResolution; } }
		public override float VerticalResolution { get { return NativeImage.VerticalResolution; } }
		public MemoryStream ImageStream { get { return imageStream; } }
		#endregion
		void SetNativeImage(Image value, MemoryStream imageStream) {
			if (NativeImage == value)
				return;
			RaiseNativeImageChanging();
			SetNativeImageCore(value, imageStream);
			OnNativeImageChanged(value.Size);
		}
		void SetNativeImageCore(Image value, MemoryStream imageStream) {
			nativeImage = value;
			this.imageStream = imageStream;
		}
		protected internal Size EnsureNonZeroSize(Size size) {
			return new Size(Math.Max(1, size.Width), Math.Max(1, size.Height));
		}
		protected internal override Size UnitsToDocuments(Size sizeInUnits) {
			return EnsureNonZeroSize(Units.PixelsToDocuments(sizeInUnits, HorizontalResolution, VerticalResolution));
		}
		protected internal override Size CalculateImageSizeInModelUnits(DocumentModelUnitConverter unitConverter) {
			return EnsureNonZeroSize(unitConverter.PixelsToModelUnits(SizeInOriginalUnits, HorizontalResolution, VerticalResolution));
		}
		protected internal override Size UnitsToHundredthsOfMillimeter(Size sizeInUnits) {
			return EnsureNonZeroSize(Units.PixelsToHundredthsOfMillimeter(sizeInUnits, HorizontalResolution, VerticalResolution));
		}
		protected internal override Size UnitsToTwips(Size sizeInUnits) {
			return EnsureNonZeroSize(Units.PixelsToTwips(sizeInUnits, HorizontalResolution, VerticalResolution));
		}
		protected internal override Size UnitsToPixels(Size sizeInUnits) {
			return sizeInUnits;
		}
		protected internal virtual void OnNativeImageChanged(Size desiredSize) {
			RaiseNativeImageChanged(desiredSize);
		}
		#region Clone
		protected override OfficeImage CreateClone(IDocumentModel target) {
			OfficeImage img = target.GetImageById(Id);
			if (!object.ReferenceEquals(img, null))
				return img;
			return new OfficeImageWin(CloneNativeImage(), imageStream, Id);
		}
		protected Image CloneNativeImage() {
			if (NativeImage.RawFormat.Guid == ImageFormat.Gif.Guid) {
				MemoryStream stream = new MemoryStream();
				NativeImage.Save(stream, GetImageCodecInfo(ImageFormat.Gif), null);
				return Image.FromStream(stream);
			}
			return (Image)NativeImage.Clone();
		}
		#endregion
		#region GetImageCodecInfo
		protected internal static ImageCodecInfo GetImageCodecInfo(ImageFormat format) {
			ImageCodecInfo result;
			imageEncodersInfo.TryGetValue(format.Guid, out result);
			return result;
		}
		#endregion
		public override bool CanGetImageBytes(OfficeImageFormat imageFormat) {
			return true;
		}
		#region GetImageBytes
		public override byte[] GetImageBytes(OfficeImageFormat imageFormat) {
			if (imageFormat.Equals(OfficeImageFormat.Wmf))
				return GetWmfImageBytes();
			else if (imageFormat.Equals(OfficeImageFormat.Emf))
				return GetEmfImageBytes();
			else
				return GetBitmapImageBytes(NativeImage, imageFormat);
		}
		#endregion
		#region GetImageBytesStream
		protected internal override Stream GetImageBytesStream(OfficeImageFormat imageFormat) {
			if (imageFormat.Equals(OfficeImageFormat.Wmf))
				return GetWmfImageBytesStream();
			else if (imageFormat.Equals(OfficeImageFormat.Emf))
				return GetEmfImageBytesStream();
			else
				return GetBitmapImageBytesStream(NativeImage, imageFormat);
		}
		#endregion
		#region GetBitmapImageBytesStream
		static ChunkedMemoryStream GetBitmapImageBytesStream(Image nativeImage, OfficeImageFormat imageFormat) {
			ChunkedMemoryStream stream = new ChunkedMemoryStream();
			ImageCodecInfo codecInfo = GetImageCodecInfo(OfficeImageHelper.GetImageFormat(imageFormat));
			nativeImage.Save(stream, codecInfo, null);
			stream.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
		#endregion
		#region GetBitmapImageBytes
		static byte[] GetBitmapImageBytes(Image nativeImage, OfficeImageFormat imageFormat) {
			ChunkedMemoryStream stream = GetBitmapImageBytesStream(nativeImage, imageFormat);
			try {
				return stream.ToArray();
			}
			finally {
				stream.Close();
				((IDisposable)stream).Dispose();
			}
		}
		#endregion
		public override byte[] GetImageBytesSafe(OfficeImageFormat imageFormat) {
			try {
				return GetImageBytesSafeCore(imageFormat);
			}
			catch {
			}
			if (TryRepairImage()) {
				try {
					return GetImageBytesSafeCore(imageFormat);
				}
				catch {
				}
			}
			ReplaceInvalidImage();
			return GetImageBytesSafeCore(imageFormat);
		}
		protected virtual bool TryRepairImage() {
			try {
				if (imageStream == null)
					return false;
				Image newImage = Image.FromStream(imageStream);
				SetNativeImage(newImage, imageStream);
				return true;
			}
			catch {
				return false;
			}			
		}
		protected virtual void ReplaceInvalidImage() {
			Image newImage = new Bitmap(nativeImage.Width, nativeImage.Height);
			SetNativeImage(newImage, null);
		}
		protected byte[] GetImageBytesSafeCore(OfficeImageFormat imageFormat) {
			try {
				return base.GetImageBytesSafe(imageFormat);
			}
			catch {
			}
			return GetImageCopyPngBytes();
		}		
		byte[] GetImageCopyPngBytes() {
			using (Bitmap bmp = new Bitmap(NativeImage)) {
				return GetBitmapImageBytes(bmp, OfficeImageFormat.Png);
			}
		}
		public override Stream GetImageBytesStreamSafe(OfficeImageFormat imageFormat) {
			try {
				return GetImageBytesStreamSafeCore(imageFormat);
			}
			catch {
			}
			if (TryRepairImage()) {
				try {
					return GetImageBytesStreamSafeCore(imageFormat);
				}
				catch {
				}
			}
			ReplaceInvalidImage();
			return GetImageBytesStreamSafeCore(imageFormat);
		}
		protected Stream GetImageBytesStreamSafeCore(OfficeImageFormat imageFormat) {
			try {
				return base.GetImageBytesStreamSafe(imageFormat);
			}
			catch {
			}
			return GetImageCopyPngBytesStream();
		}
		Stream GetImageCopyPngBytesStream() {
			using (Bitmap bmp = new Bitmap(NativeImage)) {
				return GetBitmapImageBytesStream(bmp, OfficeImageFormat.Png);
			}
		}
		#region GetWmfImageArray
		public virtual byte[] GetWmfImageBytes() {
			IntPtr hEmf = IntPtr.Zero;
			using (MemoryStream stream = new MemoryStream()) {
				using (Metafile metaFile = DevExpress.XtraPrinting.Native.MetafileCreator.CreateInstance(stream, SizeInPixels.Width + 1, SizeInPixels.Height + 1, MetafileFrameUnit.Pixel)) {
					try {
						using (Graphics graphics = Graphics.FromImage(metaFile)) {
							graphics.DrawImage(NativeImage, new Rectangle(0, 0, SizeInPixels.Width, SizeInPixels.Height));
						}
						hEmf = metaFile.GetHenhmetafile();
						return Win32.GdipEmfToWmfBits(hEmf, Win32.MapMode.Anisotropic, Win32.EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
					}
					finally {
						MetafileHelper.DeleteMetafileHandle(hEmf);
						stream.Close();
					}
				}
			}
		}
		#endregion
		#region GetWmfImageBytesStream
		public virtual Stream GetWmfImageBytesStream() {
			byte[] bytes = GetWmfImageBytes();
			return new MemoryStream(bytes, 0, bytes.Length, false, true);
		}
		#endregion
		#region GetEmfImageBytes
		public virtual byte[] GetEmfImageBytes() {
			using (Metafile mf = (Metafile)NativeImage.Clone()) {
				IntPtr hEmf = IntPtr.Zero;
				try {
					hEmf = mf.GetHenhmetafile();
					return Win32.GetEnhMetafileBits(hEmf);
				}
				finally {
					if (hEmf != IntPtr.Zero) {
						MetafileHelper.DeleteMetafileHandle(hEmf);
					}
				}
			}
		}
		#endregion
		#region GetEmfImageBytesStream
		public virtual Stream GetEmfImageBytesStream() {
			byte[] bytes = GetEmfImageBytes();
			return new MemoryStream(bytes, 0, bytes.Length, false, true);
		}
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (NativeImage != null) {
						NativeImage.Dispose();
						SetNativeImageCore(null, null);
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		public override void DiscardCachedData() {
			if (this.imageStream != null) {
				this.imageStream.Close();
				this.imageStream = null;
			}
		}
	}
	#endregion
	#region OfficeMetafileImageWin (abstract class)
	public abstract class OfficeMetafileImageWin : OfficeImageWin {
		Size metafileSizeInHundredthsOfMillimeter;
		protected OfficeMetafileImageWin(Metafile image, IUniqueImageId id)
			: base(image, null, id) {
			this.metafileSizeInHundredthsOfMillimeter = EnsureNonZeroSize(Size.Round(image.PhysicalDimension));
		}
		#region Properties
		public Size MetafileSizeInHundredthsOfMillimeter {
			get {
				return metafileSizeInHundredthsOfMillimeter;
			}
			set { metafileSizeInHundredthsOfMillimeter = value; }
		}
		protected internal abstract bool OverrideResolution { get; }
		public override float HorizontalResolution { get { return MetafileHelper.MetafileResolution; } }
		public override float VerticalResolution { get { return MetafileHelper.MetafileResolution; } }
		public override Size SizeInOriginalUnits { get { return MetafileSizeInHundredthsOfMillimeter; } }
		protected internal override Size UnitsToHundredthsOfMillimeter(Size sizeInUnits) { return sizeInUnits; }
		protected internal override Size CalculateImageSizeInModelUnits(DocumentModelUnitConverter unitConverter) {
			return EnsureNonZeroSize(unitConverter.HundredthsOfMillimeterToModelUnits(SizeInOriginalUnits));
		}
		protected internal override Size UnitsToDocuments(Size sizeInUnits) {
			return EnsureNonZeroSize(Units.HundredthsOfMillimeterToDocuments(sizeInUnits));
		}
		protected internal override Size UnitsToPixels(Size sizeInUnits) {
			return EnsureNonZeroSize(Units.HundredthsOfMillimeterToPixels(sizeInUnits, VerticalResolution, HorizontalResolution));
		}
		protected internal override Size UnitsToTwips(Size sizeInUnits) {
			return EnsureNonZeroSize(Units.HundredthsOfMillimeterToTwips(sizeInUnits));
		}
		#endregion
		protected internal override void OnNativeImageChanged(Size desiredSize) {
			metafileSizeInHundredthsOfMillimeter = Size.Empty;
			base.OnNativeImageChanged(desiredSize);
		}
		public override byte[] GetWmfImageBytes() {
			using (Metafile metafile = (Metafile)NativeImage.Clone()) {
				IntPtr hEmf = metafile.GetHenhmetafile();
				try {
					return Win32.GdipEmfToWmfBits(hEmf, Win32.MapMode.Anisotropic, Win32.EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
				}
				finally {
					MetafileHelper.DeleteMetafileHandle(hEmf);
				}
			}
		}
		protected override bool TryRepairImage() {
			return false;
		}
		protected override void ReplaceInvalidImage() {			
		}
	}
	#endregion
	#region OfficeWmfImageWin
	public class OfficeWmfImageWin : OfficeMetafileImageWin {
		public OfficeWmfImageWin(Metafile image, IUniqueImageId id)
			: base(image, id) {
		} 
		protected internal override bool OverrideResolution { get { return false; } }
		#region Clone
		protected override OfficeImage CreateClone(IDocumentModel documentModel) {
			OfficeImage img = documentModel.GetImageById(Id);
			if (!object.ReferenceEquals(img, null))
				return img;
			return new OfficeWmfImageWin((Metafile)NativeImage.Clone(), Id);
		}
		#endregion
		public override byte[] GetWmfImageBytes() {
			using (Metafile metafile = (Metafile)NativeImage.Clone()) {
				IntPtr hEmf = metafile.GetHenhmetafile();
				try {
					return Win32.GetMetaFileBits(hEmf);
				}
				finally {
					MetafileHelper.DeleteMetafileHandle(hEmf);
				}
			}
		}
	}
	#endregion
	#region OfficeEmfImageWin
	public class OfficeEmfImageWin : OfficeMetafileImageWin {
		public OfficeEmfImageWin(Metafile image, IUniqueImageId id)
			: base(image, id) {
		}
		protected internal override bool OverrideResolution { get { return true; } }
		#region Clone
		protected override OfficeImage CreateClone(IDocumentModel targetModel) {
			OfficeImage img = targetModel.GetImageById(Id);
			if (!object.ReferenceEquals(img, null))
				return img;
			return new OfficeEmfImageWin((Metafile)NativeImage.Clone(), Id);
		}
		#endregion
	}
	#endregion
	#region MetafileHelper
	public static class MetafileHelper {
		#region Fields
		public const int MetafileResolution = 96;
		#endregion
		static Metafile TryCreateMetafile(IntPtr handle) {
			if (handle == IntPtr.Zero)
				return null;
			try {
				return TryCreateMetafileCore(handle);
			}
			catch {
				return null;
			}
		}
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[System.Security.SecuritySafeCritical]
		static Metafile TryCreateMetafileCore(IntPtr handle) {
			return new Metafile(handle, true);
		}
		public static Image CreateMetafile(MemoryStream stream, Win32.MapMode mapMode, int pictureWidth, int pictureHeight) {
			byte[] bytes = stream.GetBuffer();
			IntPtr handle = Win32.SetMetaFileBitsEx(stream.Length, bytes);
			Metafile result = TryCreateMetafile(handle);
			if (result != null)
				return result;
			else
				MetafileHelper.DeleteMetafileHandle(handle);
			handle = Win32.SetEnhMetaFileBits(stream.Length, bytes);
			result = TryCreateMetafile(handle);
			if (result != null)
				return result;
			else
				MetafileHelper.DeleteMetafileHandle(handle);
			Win32.METAFILEPICT mfp = new Win32.METAFILEPICT(mapMode, pictureWidth, pictureHeight);
			handle = Win32.SetWinMetaFileBits((UInt32)stream.Length, bytes, IntPtr.Zero, ref mfp);
			result = TryCreateMetafile(handle);
			if (result != null)
				return result;
			else
				MetafileHelper.DeleteMetafileHandle(handle);
			MemoryStream streamCopy = new MemoryStream(bytes, 0, (int)stream.Length);
			try {
				return ImageLoaderHelper.ImageFromStream(streamCopy).Image;
			}
			catch {
				throw new ArgumentException("Invalid metafile.");
			}
		}
		public static void DeleteMetafileHandle(IntPtr handle) {
			if (!Win32.DeleteEnhMetaFile(handle))
				Win32.DeleteMetaFile(handle);
		}
		[DllImport("user32.dll")]
		static extern bool OpenClipboard(IntPtr hWndNewOwner);
		[DllImport("user32.dll")]
		static extern int IsClipboardFormatAvailable(int wFormat);
		[DllImport("user32.dll")]
		static extern IntPtr GetClipboardData(int wFormat);
		[DllImport("user32.dll")]
		static extern int CloseClipboard();
		const int CF_ENHMETAFILE = 14;
		const int CF_METAFILEPICT = 3;
		delegate byte[] GetMetafileBytesDelegate(IntPtr handle);
		public static Metafile GetEnhMetafileFromClipboard() {
			return GetMetafileFromClipboardCore(CF_ENHMETAFILE, Win32.GetEnhMetafileBits);
		}
		public static Metafile GetMetafileFromClipboard() {
			return GetMetafileFromClipboardCore(CF_METAFILEPICT, Win32.GetMetaFileBits);
		}
		[System.Security.SecuritySafeCritical]
		static Metafile GetMetafileFromClipboardCore(int uFormat, GetMetafileBytesDelegate getter) {
			if (!OpenClipboard(IntPtr.Zero))
				return null;
			IntPtr hmeta = IntPtr.Zero;
			try {
				if (IsClipboardFormatAvailable(uFormat) == 0)
					return null;
				hmeta = GetClipboardData(uFormat);
				using (MemoryStream stream = new MemoryStream(getter(hmeta))) {
					return new Metafile(stream);
				}
			}
			finally {
				if (hmeta != IntPtr.Zero)
					DeleteMetafileHandle(hmeta);
				CloseClipboard();
			}
		}
	}
	#endregion
}
