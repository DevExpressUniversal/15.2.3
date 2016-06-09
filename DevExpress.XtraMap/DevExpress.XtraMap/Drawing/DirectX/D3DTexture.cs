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
using System.Drawing;
using System.Drawing.Imaging;
using System.Security;
using D3D;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System.Collections.Generic;
namespace DevExpress.XtraMap.Drawing.DirectX {
	public class TextureCache  {
		const long MaxLowPriorityTextureCount = 100;
		readonly Dictionary<int, D3DTexture> TexturesPool;
		readonly Dictionary<int, D3DTexture> LowPriorityPool;
		readonly long MemoryUsageLimit;
		long TextureMemoryUsage;
		long LowPriorityTextureCount;
		public TextureCache(long maxMemoryUsage) {
			this.MemoryUsageLimit = maxMemoryUsage;
			this.TexturesPool = new Dictionary<int, D3DTexture>();
			this.LowPriorityPool = new Dictionary<int, D3DTexture>();
		}
		void ClearLowPriority() {
			long memoryUsage = 0;
			foreach (KeyValuePair<int, D3DTexture> itemPair in this.LowPriorityPool) {
				memoryUsage += itemPair.Value.GetMemoryUsage();
				itemPair.Value.Dispose();
			}
			this.LowPriorityPool.Clear();
			this.TextureMemoryUsage -= memoryUsage;
			this.LowPriorityTextureCount = 0;
		}
		void ProcessMemoryLimit() {
			if (LowPriorityTextureCount > MaxLowPriorityTextureCount)
				ClearLowPriority();
			if (MemoryUsageLimit < TextureMemoryUsage)
				Clear();
		}
		public void Clear() {
			ClearLowPriority();
			foreach(KeyValuePair<int, D3DTexture> itemPair in this.TexturesPool) {
				itemPair.Value.Dispose();
			}
			this.TexturesPool.Clear();
			this.TextureMemoryUsage = 0; 
		}
		public void Add(int key, D3DTexture texture) {
			if(texture != null) {
				TextureMemoryUsage += texture.GetMemoryUsage();
				ProcessMemoryLimit();
				if (texture.CachePriority != ImageCachePriority.Low)
					this.TexturesPool[key] = texture;
				else {
					this.LowPriorityTextureCount++;
					this.LowPriorityPool[key] = texture;
				}  
			}
		}
		public D3DTexture GetTexture(int key) {
			D3DTexture texture = null;
			if(this.TexturesPool.TryGetValue(key, out texture))
				return texture;
			else
				this.LowPriorityPool.TryGetValue(key, out texture);
			return texture;
		}
	}
	public sealed class D3DTexture : IDisposable {
		IDirect3DTexture9 texture;
		Size size;
		Size imageSize;
		IDirect3DDevice9 d3dDevice;
		PointF uvFactor;
		public Size Size { get { return size; } }
		public Size ImageSize { get { return imageSize; } }
		public PointF UVFactor { get { return uvFactor; } }
		public ImageCachePriority CachePriority { get; set; }
		[CLSCompliant(false)]
		public IDirect3DTexture9 Texture { get { return texture; } }
		[CLSCompliant(false)]
		public D3DTexture(IDirect3DDevice9 device) {
			Guard.ArgumentNotNull(device, "Device");
			this.d3dDevice = device;
		}
		static Size CorrectTextureSize(Size size) {
			Size resSize = new Size();
			resSize.Width = Math.Min(size.Width, (int)Direct3DHelper.D3DParameters.MaxTextureWidth);
			resSize.Height = Math.Min(size.Height, (int)Direct3DHelper.D3DParameters.MaxTextureHeight);
			return resSize;
		}
		D3DLOCKED_RECT ClearTextureData(D3DLOCKED_RECT text_rect) {
			MarshalHelper.FillMemory(text_rect.pBits, (uint)(text_rect.Pitch * Size.Height), 0x00);
			return text_rect;
		}
		void CopyDataToTexture(BitmapData source, D3DLOCKED_RECT dest) {
			long texOffset = dest.pBits.ToInt64();
			long bmpOffset = source.Scan0.ToInt64();
			for(int i = 0; i < source.Height; i++) {
				IntPtr texLine = new IntPtr(texOffset + i * dest.Pitch);
				IntPtr bmpLine = new IntPtr(bmpOffset + i * source.Stride);
				MarshalHelper.CopyMemory(texLine, bmpLine, (uint)source.Stride);
			}
		}
		public long GetMemoryUsage() {
			return this.size.Width * this.size.Height * 4;
		}
		[SecuritySafeCritical]
		public bool SetBitmap(Bitmap bitmap) {
			Guard.ArgumentNotNull(bitmap, "Bitmap");
			Size bitmapSize = ImageSafeAccess.GetSize(bitmap);
			if(bitmapSize == Size.Empty)
				return false;
			this.size = RoundToTextureSize(bitmapSize);
			this.imageSize = bitmapSize;
			this.uvFactor.X = (float)bitmap.Width / (float)size.Width;
			this.uvFactor.Y = (float)bitmap.Height / (float)size.Height;
			uint res = (uint)d3dDevice.CreateTexture((uint)size.Width, (uint)size.Height, 1, (uint)D3DUSAGE.NONE,
									   D3DFORMAT.D3DFMT_A8R8G8B8, D3DPOOL.MANAGED, out texture, IntPtr.Zero);
			if(res == Direct3D.D3DERR_OUTOFVIDEOMEMORY || res == Direct3D.E_OUTOFMEMORY)
				return false;
			Bitmap tempBitmap = BitmapUtils.ToArgbBitmap(bitmap, false);
			D3DLOCKED_RECT text_rect = new D3DLOCKED_RECT();
			RECT texRect = new RECT() { left = 0, top = 0, right = this.size.Width, bottom = this.size.Height };
			if(!D3DUtils.CheckForD3DErrors(texture.LockRect(0, ref text_rect, ref texRect, 0), "LockRect", false)) {
				Dispose();
				return false;
			}
			Rectangle rect = new Rectangle(0, 0, this.imageSize.Width, this.imageSize.Height);
			BitmapData bmpData = tempBitmap.LockBits(rect, ImageLockMode.ReadWrite, tempBitmap.PixelFormat);
			ClearTextureData(text_rect);
			CopyDataToTexture(bmpData, text_rect);
			tempBitmap.UnlockBits(bmpData);
			texture.UnlockRect(0);
			if(!bitmap.Equals(tempBitmap))
				tempBitmap.Dispose();
			return true;
		}
		public void Dispose() {
			if(texture != null) {
				MarshalHelper.ReleaseComObject(texture);
				texture = null;
			}
		}
		internal static Size RoundToTextureSize(Size size) {
			return CorrectTextureSize(D3DUtils.RoundPow2Size(size));
		}
	}
}
