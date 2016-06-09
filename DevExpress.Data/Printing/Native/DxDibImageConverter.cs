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
using System.Text;
using System.IO;
#if SL
using Bmp = System.Windows.Media.Imaging.WriteableBitmap;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DevExpress.Utils.Zip;
using DevExpress.Xpf.Drawing;
#else
using Bmp = System.Drawing.Bitmap;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
#endif
namespace DevExpress.Data.Printing.Native {
	public static class DxDibImageConverter {
		const string dxDibHeader = "DXDIB";
		const int version = 1;
		static int ReadInt(byte[] src, ref int index) {
			int result = 0;
			result |= src[index++];
			result |= src[index++] << 8;
			result |= src[index++] << 16;
			result |= src[index++] << 24;
			return result;
		}
		static void Decompress(byte[] src, int offset, byte[] dst) {
			using (MemoryStream stream = new MemoryStream(src, offset, src.Length - offset))
			using (DeflateStream deflaterStream = new DeflateStream(stream, CompressionMode.Decompress)) {
				deflaterStream.Read(dst, 0, dst.Length);
			}
		}
		public static bool IsDxDib(byte[] bytes) {
			try {
				string header = Encoding.UTF8.GetString(bytes, 0, dxDibHeader.Length);
				return header == dxDibHeader;
			}
			catch {
				return false;
			}
		}
#if SL
		static void WriteInt(byte[] dst, int value, ref int index) {
			dst[index++] = (byte)(value & 0xff);
			dst[index++] = (byte)((value >> 8) & 0xff);
			dst[index++] = (byte)((value >> 16) & 0xff);
			dst[index++] = (byte)((value >> 24) & 0xff);
		}
		static byte[] Compress(byte[] data) {
			using (MemoryStream stream = new MemoryStream()) {
				DeflateStream deflaterStream = new DeflateStream(stream, CompressionMode.Compress);
				deflaterStream.Write(data, 0, data.Length);
				deflaterStream.Close();
				return stream.ToArray();
			}
		}
		static byte[] GetBytes(int[] pixels) {
			byte[] data = new byte[pixels.Length * 4];
			int index = 0;
			foreach (int pixel in pixels)
				WriteInt(data, pixel, ref index);
			return data;
		}
		static WriteableBitmap CreateBitmap(byte[] src, int startIndex, int width, int height) {
			WriteableBitmap bitmap = new WriteableBitmap(width, height);
			int index = startIndex;
			for (int i = 0; i < bitmap.Pixels.Length; i++)
				bitmap.Pixels[i] = ReadInt(src, ref index);
			return bitmap;
		}
		static byte[] Encode(int[] pixels, int width, int height, bool compress) {
			byte[] data = GetBytes(pixels);
			if (compress)
				data = Compress(data);
			byte[] headerBytes = Encoding.UTF8.GetBytes(dxDibHeader);
			byte[] result = new byte[headerBytes.Length + 1 + 1 + 4 + 4 + data.Length];
			headerBytes.CopyTo(result, 0);
			int index = headerBytes.Length;
			result[index++] = version;
			int flags = compress ? (byte)0x1 : (byte)0x0;
			result[index++] = (byte)flags;
			WriteInt(result, width, ref index);
			WriteInt(result, height, ref index);
			data.CopyTo(result, index);
			return result;
		}
		public static byte[] Encode(System.Windows.Controls.Image image, bool compress) {
			BitmapSource bitmapSource = image.Source as BitmapSource;
			if (bitmapSource == null)
				throw new NotSupportedException();
			WriteableBitmap bitmap = new WriteableBitmap(bitmapSource);
			return Encode(bitmap, compress);
		}
		public static byte[] Encode(Bitmap bitmap, bool compress) {
			return Encode(bitmap.Pixels, bitmap.Width, bitmap.Height, compress);
		}
		public static byte[] Encode(WriteableBitmap bitmap, bool compress) {
			return Encode(bitmap.Pixels, bitmap.PixelWidth, bitmap.PixelHeight, compress);
		}
#else
		public static bool HasUnmanagedCodePermitted {
			get {
				try {
					SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
					sp.Demand();
					return true;
				}
				catch (SecurityException) {
					return false;
				}
			}
		}
		[SecuritySafeCritical]
		static Bitmap CreateBitmapUnmanaged(byte[] src, int startIndex, int width, int height) {
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			try {
				Marshal.Copy(src, startIndex, bitmapData.Scan0, src.Length - startIndex);
			}
			finally {
				bitmap.UnlockBits(bitmapData);
			}
			return bitmap;
		}
		static Bitmap CreateBitmapManaged(byte[] src, int startIndex, int width, int height) {
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			for (int i = 0, k = 0; i < src.Length; k++) {
				int y = k / width;
				int x = k - y * width;
				int value = ReadInt(src, ref i);
				Color color = Color.FromArgb(value);
				bitmap.SetPixel(x, y, color);
			}
			return bitmap;
		}
		static Bitmap CreateBitmap(byte[] src, int startIndex, int width, int height) {
			return HasUnmanagedCodePermitted ?
				CreateBitmapUnmanaged(src, startIndex, width, height) :
				CreateBitmapManaged(src, startIndex, width, height);
		}
#endif
		public static Bmp Decode(byte[] data) {
			try {
				string header = Encoding.UTF8.GetString(data, 0, dxDibHeader.Length);
				if (header != dxDibHeader)
					return null;
				int version = data[dxDibHeader.Length];
				if (version != DxDibImageConverter.version)
					return null;
				int index = dxDibHeader.Length + 1;
				int flags = data[index++];
				bool compressed = (flags & 0x1) > 0;
				int width = ReadInt(data, ref index);
				int height = ReadInt(data, ref index);
				if (compressed) {
					byte[] buffer = new byte[width * height * 4];
					Decompress(data, index, buffer);
					data = buffer;
					index = 0;
				}
				return CreateBitmap(data, index, width, height);
			}
			catch {
				return null;
			}
		}
	}
}
