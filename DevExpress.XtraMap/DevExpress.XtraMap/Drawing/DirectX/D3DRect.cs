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
using D3D;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraMap.Drawing.DirectX;
using System.Runtime.InteropServices;
namespace DevExpress.XtraMap.Drawing {
	public enum D3DRectType {
		Picture,
		Overlay
	}
	[CLSCompliant(false)]
	public abstract class D3DRect : MapDisposableObject {
		public static readonly uint MaxRectSize = (uint)D3DRenderer.VertexStride * 4;
		IDirect3DDevice9 d3dDevice;
		SizeF size;
		D3DBuffer d3dBuffer;
		D3DRectType rectType;
		Size imageSize;
		RectangleF clipUV = RectangleF.Empty;
		Size textureSize;
		protected IDirect3DDevice9 D3dDevice { get { return d3dDevice; } }
		protected Size TextureSize { get { return textureSize; } }
		public Size ImageSize { get { return imageSize; } }
		public RectangleF ClipUV { get { return clipUV; } }
		public SizeF Size { get { return size; } }
		public D3DRectType Type { get { return rectType; } protected set { rectType = value; } }
		public D3DBuffer RectBuffer { get { return d3dBuffer; } protected set { d3dBuffer = value; } }
		protected D3DRect(IDirect3DDevice9 device, Size imageSize, RectangleF rect, RectangleF clipUV) {
			this.d3dDevice = device;
			Guard.ArgumentNotNull(device, "Device");
			this.size = new SizeF(rect.Width, rect.Height);
			this.textureSize = D3DTexture.RoundToTextureSize(size.ToSize());
			this.imageSize = imageSize;
			this.clipUV = clipUV;
			Restore();
		}
		protected override void DisposeOverride() {
			if(d3dBuffer.Buffer != null){
				MarshalHelper.ReleaseComObject(d3dBuffer.Buffer);
				d3dBuffer.Buffer = null;
			}
			base.DisposeOverride();
		}
		protected abstract void Restore();
	}
	[CLSCompliant(false)]
	public class D3DSpriteRect : D3DRect {
		public D3DSpriteRect(IDirect3DDevice9 device, Size imageSize, RectangleF rect, RectangleF clipUV)
			: base(device, imageSize, rect, clipUV) {
				Type = D3DRectType.Picture;
		}
		protected override void Restore() {
			RectangleF clipRect = ClipUV;
			float dU = 0.5f / (float)ImageSize.Width;
			if(clipRect != RectangleF.Empty) {
				float u0 = clipRect.X / ImageSize.Width;
				float v0 = clipRect.Y / ImageSize.Height;
				float u1 = (clipRect.X + clipRect.Width) / ImageSize.Width;
				float v1 = (clipRect.Y + clipRect.Height) / ImageSize.Height;
				Restore(new RectangleF(u0 + dU, v0, u1 - dU, v1));
			}
			else
				Restore(new RectangleF(.0f, .0f, 1.0f, 1.0f));
		}
		void Restore(RectangleF uvRect) {
			Vertex[] verties = new Vertex[4];
			verties[0].Position = new Vector3(.0f, -TextureSize.Height, .0f);
			verties[0].UV = new Vector2(uvRect.X, uvRect.Height);
			verties[1].Position = new Vector3(TextureSize.Width, -TextureSize.Height, .0f);
			verties[1].UV = new Vector2(uvRect.Width, uvRect.Height);
			verties[2].Position = new Vector3(TextureSize.Width, .0f, .0f);
			verties[2].UV = new Vector2(uvRect.Width, uvRect.Y);
			verties[3].Position = new Vector3(.0f, .0f, .0f);
			verties[3].UV = new Vector2(uvRect.X, uvRect.Y);
			uint bufSize = (uint)(D3DRenderer.VertexStride * verties.Length);
			IDirect3DVertexBuffer9 buf = null;
			if(RectBuffer.Buffer == null) {
				D3dDevice.CreateVertexBuffer(bufSize, D3DUSAGE.WRITEONLY, D3DRenderer.FVF,
										   D3DPOOL.MANAGED, out buf, IntPtr.Zero);
				if(buf == null)
					throw new Exception("Create D3DBuffer error");
				RectBuffer = new D3DBuffer() { Buffer = buf, PrimitiveCount = 2, Size = bufSize };
			}
			IntPtr buffer;
			RectBuffer.Buffer.Lock(0, bufSize, out buffer, 0);
			IntPtr sourcePtr = MarshalHelper.UnsafeAddrOfPinnedArrayElement(verties, 0);
			MarshalHelper.CopyMemory(buffer, sourcePtr, bufSize);
			RectBuffer.Buffer.Unlock();
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct OverlayVertex {
		Vector3 position;
		float rhw;
		uint color;
		Vector2 uv;
		public Vector3 Position { get { return position; } set { position = value; } }
		public float RHW { get { return rhw; } set { rhw = value; } }
		public Vector2 UV { get { return uv; } set { uv = value; } }
		public uint Color { get { return color; } set { color = value; } }
	}
	[CLSCompliant(false)]
	public class D3DOverlayRect : D3DRect {
		PointF position;
		public PointF Position { get { return position; } 
			set {
				if(PointF.Equals(position, value))
					return;
				position = value; 
				Restore(); 
			} }
		public D3DOverlayRect(IDirect3DDevice9 device, Size imageSize, RectangleF rect, RectangleF clipUV)
			: base(device, imageSize, rect, clipUV) {
				Type = D3DRectType.Overlay;
		}
		protected override void Restore() {
			OverlayVertex[] verties = new OverlayVertex[4];
			float dU = Size.Width / TextureSize.Width;
			float dV = Size.Height / TextureSize.Height;
			verties[0].Position = new Vector3(this.position.X - 0.5f, this.position.Y - 0.5f, .0f);
			verties[0].UV = new Vector2(.0f, .0f);
			verties[1].Position = new Vector3(this.position.X - 0.5f + Size.Width, this.position.Y - 0.5f, .0f);
			verties[1].UV = new Vector2(dU, .0f);
			verties[2].Position = new Vector3(this.position.X - 0.5f + Size.Width, this.position.Y - 0.5f + Size.Height, .0f);
			verties[2].UV = new Vector2(dU, dV);
			verties[3].Position = new Vector3(this.position.X - 0.5f, this.position.Y - 0.5f + Size.Height, .0f);
			verties[3].UV = new Vector2(.0f, dV);
			verties[0].RHW = 1.0f;
			verties[1].RHW = 1.0f;
			verties[2].RHW = 1.0f;
			verties[3].RHW = 1.0f;
			verties[0].Color = 0xFFFFFFFF;
			verties[1].Color = 0xFFFFFFFF;
			verties[2].Color = 0xFFFFFFFF;
			verties[3].Color = 0xFFFFFFFF;
			uint bufSize = (uint)(D3DRenderer.OverlayVertexStride * verties.Length);
			IDirect3DVertexBuffer9 buf = null;
			if(RectBuffer.Buffer == null) {
				D3dDevice.CreateVertexBuffer(bufSize, D3DUSAGE.WRITEONLY, D3DRenderer.OverlayFVF,
											D3DPOOL.MANAGED, out buf, IntPtr.Zero);
				if(buf == null)
					throw new Exception("Create D3DBuffer error");
				RectBuffer = new D3DBuffer() { Buffer = buf, PrimitiveCount = 2 };
			}
			IntPtr buffer;
			RectBuffer.Buffer.Lock(0, bufSize, out buffer, 0);
			IntPtr sourcePtr = MarshalHelper.UnsafeAddrOfPinnedArrayElement(verties, 0);
			MarshalHelper.CopyMemory(buffer, sourcePtr, bufSize);
			RectBuffer.Buffer.Unlock();
		}
	}
}
