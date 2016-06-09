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
using System.Text;
using System.Drawing;
using System.IO;
#if !DXWINDOW
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.Data.Helpers;
#endif
using System.Drawing.Imaging;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Data {
#else
namespace DevExpress.Data.Utils {
#endif
	public class ImageTool {
#if !DXWINDOW
		public static RectangleF CalculateImageRect(RectangleF clientRect, SizeF imageSize, ImageSizeMode sizeMode) {
			return CalculateImageRectCore(clientRect, GraphicsUnitConverter.DipToDoc(imageSize), sizeMode);
		}
		public static RectangleF CalculateImageRectCore(RectangleF clientRect, SizeF imageSize, ImageSizeMode sizeMode) {
			return CalculateImageRectCore(clientRect, imageSize, sizeMode, ImageAlignment.Default);
		}
		public static RectangleF CalculateImageRect(RectangleF clientRect, SizeF imageSize, ImageSizeMode sizeMode, ImageAlignment alignment) {
			return CalculateImageRectCore(clientRect, GraphicsUnitConverter.DipToDoc(imageSize), sizeMode, alignment);
		}
		public static RectangleF CalculateImageRectCore(RectangleF clientRect, SizeF imageSize, ImageSizeMode sizeMode, ImageAlignment alignment) {
			RectangleF imageRect = new RectangleF(PointF.Empty, imageSize);
			switch(sizeMode) {
				case ImageSizeMode.Squeeze:
					if(imageRect.Width > clientRect.Width || imageRect.Height > clientRect.Height)
						imageRect.Size = MathMethods.ZoomInto(clientRect.Size, imageRect.Size);
					imageRect = RectF.Align(imageRect, clientRect,
						GraphicsConvertHelper.ToHorzBrickAlignment(alignment, sizeMode), GraphicsConvertHelper.ToVertBrickAlignment(alignment, sizeMode));
					break;
				case ImageSizeMode.CenterImage:
					imageRect = RectF.Align(imageRect, clientRect, BrickAlignment.Center, BrickAlignment.Center);
					break;
				case ImageSizeMode.ZoomImage:
					imageRect.Size = MathMethods.ZoomInto(clientRect.Size, imageRect.Size);
					imageRect = RectF.Align(imageRect, clientRect,
						GraphicsConvertHelper.ToHorzBrickAlignment(alignment, sizeMode), GraphicsConvertHelper.ToVertBrickAlignment(alignment, sizeMode));
					break;
				case ImageSizeMode.AutoSize:
					imageRect.Location = clientRect.Location;
					break;
				case ImageSizeMode.Normal:
					imageRect = RectF.Align(imageRect, clientRect,
						GraphicsConvertHelper.ToHorzBrickAlignment(alignment, sizeMode), GraphicsConvertHelper.ToVertBrickAlignment(alignment, sizeMode));
					break;
				case ImageSizeMode.StretchImage:
				case ImageSizeMode.Tile:
					imageRect = clientRect;
					break;
			}
			return imageRect;
		}
		public virtual byte[] ToArray(Image img) {
			if(img == null)
				return new byte[0];
			lock(img) {
				return img is Metafile ? GetMetafileArray((Metafile)img) : ToArray(img, img.RawFormat);
			}
		}
		public byte[] ToArray(Image img, ImageFormat format) {
			return format == ImageFormat.Wmf ? GetWmfImageArray(img) :
				ToArrayCore(img, format);
		}
		protected byte[] ToArrayCore(Image img, ImageFormat format) {
			MemoryStream stream = new MemoryStream();
			try {
				SaveImage(img, stream, format);
				return stream.ToArray();
			} catch {
				return new byte[] { };
			} finally {
				stream.Close();
				((IDisposable)stream).Dispose();
			}
		}
#endif
		public void SaveImage(Image img, Stream stream, ImageFormat format) {
			ImageCodecInfo info = FindEncoder(format);
			if(info == null)
				info = FindEncoder(ImageFormat.Png);
			lock(img) {
				img.Save(stream, info, null);
			}
		}
#if !DXWINDOW
		public Image FromArray(byte[] buffer) {
			if(buffer == null)
				return null;
			Image img = null;
			if(DxDibImageConverter.IsDxDib(buffer))
				img = DxDibImageConverter.Decode(buffer);
			if(img == null && buffer.Length > 78) {
				if(buffer[0] == 0x15 && buffer[1] == 0x1c)  
					img = FromArrayCore(buffer, 78);
			}
			if(img == null)
				img = FromArrayCore(buffer, 0);
			return img;
		}
#endif
		protected virtual Image FromArrayCore(byte[] buffer, int offset) {
			if(buffer == null)
				return null;
			try {
				MemoryStream stream = new MemoryStream(buffer, offset, (int)buffer.Length - offset);
				return ImageFromStream(stream);
			} catch { return null; }
		}
		public static Image ImageFromStream(Stream stream) {
			if(!IsWin7 || !IsUnmanagedCodeGranted)
				return Image.FromStream(stream);
			else
				return Image.FromStream(stream, false, false);
		}
		static bool IsWin7 {
			get {
				Version version = Environment.OSVersion.Version;
				return (version.Major == 6 && version.Minor >= 1) || version.Major > 6;
			}
		}
		static bool IsUnmanagedCodeGranted {
			get {
				return SecurityHelper.IsPermissionGranted(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
			}
		}
		enum EmfToWmfBitsFlags {
			EmfToWmfBitsFlagsDefault = 0x00000000,
			EmfToWmfBitsFlagsEmbedEmf = 0x00000001,
			EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,
			EmfToWmfBitsFlagsNoXORClip = 0x00000004
		};
		const int MM_ANISOTROPIC = 8;
#if !DXWINDOW
		static byte[] GetWmfImageArray(Image image) {
			MemoryStream stream = null;
			Metafile metaFile = null;
			IntPtr hemf = IntPtr.Zero;
			try {
				stream = new MemoryStream();
				metaFile = MetafileCreator.CreateInstance(stream);
				using(Graphics graphics = Graphics.FromImage(metaFile)) {
					graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
				}
				hemf = metaFile.GetHenhmetafile();
				uint bufferSize = GdipEmfToWmfBits(hemf, 0, null, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
				byte[] buffer = new byte[bufferSize];
				GdipEmfToWmfBits(hemf, bufferSize, buffer, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
				return buffer;
			} finally {
				DeleteEnhMetaFile(hemf);
				metaFile.Dispose();
				stream.Close();
			}
		}
#endif
		[System.Runtime.InteropServices.DllImport("gdiplus")]
		static extern uint GdipEmfToWmfBits(IntPtr hemf, uint bufferSize, byte[] buffer, int mappingMode, EmfToWmfBitsFlags flags);
		[System.Runtime.InteropServices.DllImport("gdi32")]
		static extern bool DeleteEnhMetaFile(IntPtr hemf);
		[System.Runtime.InteropServices.DllImport("gdi32")]
		static extern int GetEnhMetaFileBits(IntPtr hemf, int cbBuffer, byte[] lpbBuffer);
		[System.Runtime.InteropServices.DllImport("gdi32")]
		static extern int GetMetaFileBitsEx(IntPtr hmf, int cbBuffer, byte[] lpbBuffer);
		static ImageCodecInfo FindEncoder(ImageFormat format) {
			ImageCodecInfo[] infos = ImageCodecInfo.GetImageEncoders();
			for(int i = 0; i < infos.Length; i++) {
				if(infos[i].FormatID.Equals(format.Guid)) {
					return infos[i];
				}
			}
			return null;
		}
		[System.Security.SecuritySafeCritical]
		static byte[] GetMetafileArray(Metafile metafile) {
			metafile = (Metafile)metafile.Clone();
			IntPtr enhMetafileHandle = metafile.GetHenhmetafile();
			int bufferSize = GetEnhMetaFileBits(enhMetafileHandle, 0, null);
			byte[] buffer = new byte[bufferSize];
			if(GetEnhMetaFileBits(enhMetafileHandle, bufferSize, buffer) <= 0)
				throw new SystemException("GetEnhMetaFileBits");
			DeleteEnhMetaFile(enhMetafileHandle);
			return buffer;
		}
	}
}
