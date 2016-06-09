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

using D3D;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Drawing.DirectX;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraMap.Drawing {
	[CLSCompliant(false)]
	public class D3DRenderer : RendererBase {
		internal static readonly bool AllowUseDirectX = Direct3DHelper.AllowUseDirectX;
		internal static readonly uint VertexStride = (uint)MarshalHelper.SizeOf(typeof(Vertex));
		internal static readonly uint LiteVertexStride = (uint)MarshalHelper.SizeOf(typeof(LiteVertex));
		internal static readonly uint OverlayVertexStride = (uint)MarshalHelper.SizeOf(typeof(OverlayVertex));
		internal const uint FVF = (uint)(D3DFVF.XYZ | D3DFVF.TEX1);
		internal const uint OverlayFVF = (uint)(D3DFVF.XYZRHW | D3DFVF.DIFFUSE | D3DFVF.TEX1);
		const long DefaultVideoMemoryLimit = 256 << 20; 
		const float AvailableVideoMemoryPercentage = 0.25f;
		const float RectsMemoryPercentage = 0.1f;
		readonly List<D3DRect> rectsPool;
		IntPtr targetHandle;
		IDirect3D9 direct3D9; 
		IDirect3DDevice9 d3dDevice;
		D3DPRESENT_PARAMETERS d3dPP;
		IDirect3DPixelShader9 pixelShader;
		IDirect3DVertexShader9 vertexShader;
		IDirect3DVertexDeclaration9 vertexDeclaration;
		MapPoint d3dOffset;
		Size viewPortSize;
		bool isInitalized;
		ITesselator tesselator;
		bool useTransformAsDPParams;
		Matrix4x4 Transform;
		long videoMemoryLimit = DefaultVideoMemoryLimit;
		long rectsMemoryLimit;
		TextureCache textureCache;
		bool IsValidClientSize { get { return MapUtils.IsValidSize(viewPortSize); } }
		Matrix4x4 ViewProjection { get; set; }
		List<D3DRect> RectsPool { get { return rectsPool; } }
		D3DOptionsHelper OptionsHelper { get; set; }
		long VideoMemoryLimit {
			get { return videoMemoryLimit; }
			set {
				videoMemoryLimit = value > 0 ? value : DefaultVideoMemoryLimit;
			}
		}
		internal IDirect3DDevice9 Device { get { return d3dDevice; } }
		internal bool IsInitalized { get { return isInitalized; } }
		public D3DRenderer(IntPtr handle) {
			if(handle == IntPtr.Zero)
				throw new ArgumentException("null Handle");
			this.targetHandle = handle;
			this.tesselator = new GLUTesselator();
			this.tesselator.Create();
			this.rectsPool = new List<D3DRect>();
			this.isInitalized = false;
		}
		protected override bool Initialize(object context) {
			return IsValidClientSize;
		}
		protected override IRenderItemResourceHolder CreateItemResourceHolder(IRenderItemProvider provider, IRenderItem owner) {
			D3DResourceHolder holder = new D3DResourceHolder(tesselator, provider, owner);
			return holder;
		}
		protected override void RenderGeometry(IRenderItemResourceHolder holder, IRenderItemStyle style) {
			D3DResourceHolder d3dHolder = holder as D3DResourceHolder;
			if(d3dHolder == null)
				return;
			OptionsHelper.VertexShader = this.vertexShader;
			OptionsHelper.PixelShader = this.pixelShader;
			d3dDevice.SetVertexShaderConstantF(4, Transform.ToArray(), 4);
			RenderShape(d3dHolder, style);
		}
		protected override void SetRenderParam() {
			OptionsHelper.VertexDeclaration = this.vertexDeclaration;
			OptionsHelper.VertexShader = this.vertexShader;
			OptionsHelper.PixelShader = this.pixelShader;
			d3dDevice.SetVertexShaderConstantF(0, ViewProjection.ToArray(), 4);  
		}
		protected override void SetScaledTransform(bool antiAliasing) {
			OptionsHelper.AntiAliasing = antiAliasing;
			Transform = Matrix4x4.Identity;
			double dX = this.d3dOffset.X + RenderOffset.X;
			double dY = this.d3dOffset.Y - RenderOffset.Y;
			Transform.M11 = (float)RenderScaleFactorX;
			Transform.M12 = (float)(RenderScaleFactorX - (double)(float)RenderScaleFactorX);
			Transform.M13 = (float)RenderScaleFactorY;
			Transform.M14 = (float)(RenderScaleFactorY - (double)(float)RenderScaleFactorY);
			Transform.M21 = (float)dX;
			Transform.M22 = (float)(dX - (double)(float)dX);
			Transform.M23 = (float)dY;
			Transform.M24 = (float)(dY - (double)(float)dY);
			useTransformAsDPParams = true;
		}
		protected override void SetLocatableTransform(ILocatableRenderItem locatableItem, bool antiAliasing) {
			OptionsHelper.AntiAliasing = antiAliasing;
			MapUnit itemLocation = locatableItem.Location;
			MapPoint stretchFactor = locatableItem.StretchFactor;
			double imageOriginX = -locatableItem.SizeInPixels.Width * stretchFactor.X * locatableItem.Origin.X;
			double imageOriginY = locatableItem.SizeInPixels.Height * stretchFactor.Y * locatableItem.Origin.Y;
			double x = (RenderScaleFactorX * itemLocation.X * stretchFactor.X);
			double y = -(RenderScaleFactorY * itemLocation.Y * stretchFactor.Y);
			x += d3dOffset.X + RenderOffset.X + imageOriginX;
			y += d3dOffset.Y - RenderOffset.Y + imageOriginY;
			Transform = Matrix4x4.CreateTranslation(x, y, 0.0);
			ApplyTemplate(locatableItem as ITemplateGeometryItem);
			useTransformAsDPParams = false;
		}
		protected override void SetImageTileTransform(ILocatableRenderItem locatableItem, bool antiAliasing) {
			OptionsHelper.AntiAliasing = antiAliasing;
			MapUnit itemLocation = locatableItem.Location;
			MapPoint stretchFactor = locatableItem.StretchFactor;
			Transform = Matrix4x4.CreateScale(stretchFactor.X, stretchFactor.Y, 1.0);
			Transform.M41 = Convert.ToSingle(this.d3dOffset.X + (RenderOffset.X + itemLocation.X) * stretchFactor.X);
			Transform.M42 = Convert.ToSingle(this.d3dOffset.Y - (RenderOffset.Y + itemLocation.Y) * stretchFactor.Y);
			useTransformAsDPParams = false;
		}
		protected override void SetViewSize(Size size) {
			this.viewPortSize = size;
			this.d3dOffset = CalcD3DOffset();
		}
		protected override void StartRender(IRenderContext renderContext) {
			base.StartRender(renderContext);
			if(!this.isInitalized)
				InitD3D();
			uint res = CheckDeviceStatus();
			if(res == Direct3D.D3DERR_DEVICELOST) {
				CanRenderer = false;
				return;
			}
			if(res == Direct3D.D3DERR_DEVICENOTRESET || !CheckActualViewPortSize())
				if(!Reset()) {
					CanRenderer = false;
					return;
				}
			BeginScene(renderContext.ContentBounds);
		}
		protected override void BeforeRenderOverlay() {
			OptionsHelper.VertexShader = null;
			OptionsHelper.PixelShader = null;
		}
		protected override void RenderImage(IImageGeometry geometry) {
			Image image = geometry.Image;
			bool alignImage = geometry.AlignImage; 
			RectangleF imageRect = geometry.ImageRect;
			if(alignImage) {
				imageRect.Inflate(1, 1);
				imageRect.Offset(0, 1);
			}
			RectangleF clipRect = geometry.ClipRect;
			bool storingInPool = geometry.StoringInPool;
			ImageCachePriority priority = geometry.CachePriority;
			lock(image) {
				D3DSpriteRect sprite = GetRect(image.Size, imageRect, clipRect, D3DRectType.Picture) as D3DSpriteRect;
				D3DTexture texture = storingInPool ? GetTexture(image, priority, alignImage) : CreateTexure(image, priority, alignImage);
				RenderSprite(sprite, texture, imageRect.Location, geometry.Transparency);
				if(!storingInPool && texture != null)
					texture.Dispose();
			}
		}
		protected override void SetClip(Rectangle clipRect) {
			OptionsHelper.ClipRect = D3DUtils.ValidateClipRect(clipRect, this.viewPortSize);
		}
		protected override void ResetClip() {
			OptionsHelper.ClipRect = Rectangle.Empty;
		}
		protected override void RenderOverlay(IRenderOverlay overlay) {
			RenderOverlay(overlay.OverlayImage, overlay.OverlayImageSize, overlay.ScreenPosition, overlay.StoringInPool);
		}
		protected override void DrawRectangle(Rectangle rect, Color fill, Color stroke) {
			OptionsHelper.VertexShader = null;
			OptionsHelper.PixelShader = null;
			d3dDevice.SetTexture(0, null);
			OverlayVertex[] vertices = CreateSelectedRegion(rect);
			uint areaColor = MapUtils.ColorToUInt(fill);
			for(int i = 0; i < vertices.Length; i++)
				vertices[i].Color = areaColor;
			IntPtr rectData = MarshalHelper.UnsafeAddrOfPinnedArrayElement(vertices, 0);
			OptionsHelper.SetFVF(D3DRenderer.OverlayFVF);
			d3dDevice.DrawPrimitiveUP(D3DPRIMITIVETYPE.D3DPT_TRIANGLEFAN, 2, rectData, D3DRenderer.OverlayVertexStride);
			uint borderColor = MapUtils.ColorToUInt(stroke);
			for(int i = 0; i < vertices.Length; i++)
				vertices[i].Color = borderColor;
			d3dDevice.DrawPrimitiveUP(D3DPRIMITIVETYPE.D3DPT_LINESTRIP, 4, rectData, D3DRenderer.OverlayVertexStride);
		}
		protected override void EndRender() {
			EndScene();
			if((int)CheckDeviceStatus() == Direct3D.D3D_OK)
				d3dDevice.Present(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}
		protected override void StartUpdateItems() {
		}
		protected override void DisposeCore() {
			 CleanD3D();
			 if(OptionsHelper != null) {
				 OptionsHelper.Dispose();
				 OptionsHelper = null;
			 }
			if(tesselator != null) {
				tesselator.Dispose();
				tesselator = null;
			}
		}
		protected override void RenderBorderFinally(Graphics gr, IRenderStyleProvider provider) {
			if (provider == null || gr == null) return;
			DevExpress.Utils.Drawing.BorderPainter painter = MapUtils.GetBorderPainter(provider);
			using (GraphicsCache cache = new GraphicsCache(gr)) {
				painter.DrawObject(new DevExpress.Utils.Drawing.BorderObjectInfoArgs(cache, null, provider.Bounds));
			}
			PreventBorderFlickering(gr);
		}
		OverlayVertex[] CreateSelectedRegion(Rectangle selectedRegion) {
			OverlayVertex[] vertices = new OverlayVertex[5];
			vertices[0].Position = new Vector3(selectedRegion.X - 0.5f, selectedRegion.Y - 0.5f, .0f);
			vertices[0].UV = new Vector2(.0f, .0f);
			vertices[1].Position = new Vector3(selectedRegion.X - 0.5f + selectedRegion.Width, selectedRegion.Y - 0.5f, .0f);
			vertices[1].UV = new Vector2(1.0f, .0f);
			vertices[2].Position = new Vector3(selectedRegion.X - 0.5f + selectedRegion.Width, selectedRegion.Y - 0.5f + selectedRegion.Height, .0f);
			vertices[2].UV = new Vector2(1.0f, 1.0f);
			vertices[3].Position = new Vector3(selectedRegion.X - 0.5f, selectedRegion.Y - 0.5f + selectedRegion.Height, .0f);
			vertices[3].UV = new Vector2(.0f, 1.0f);
			vertices[0].RHW = 1.0f;
			vertices[1].RHW = 1.0f;
			vertices[2].RHW = 1.0f;
			vertices[3].RHW = 1.0f;
			vertices[4] = vertices[0];
			return vertices;
		}
		void RenderOverlay(Image image, Size imageSize, PointF pos, bool storingInPool) {
			RectangleF imageRect = new RectangleF(0, 0, imageSize.Width, imageSize.Height);
			D3DOverlayRect overlay = GetRect(imageSize, imageRect, RectangleF.Empty, D3DRectType.Overlay) as D3DOverlayRect;
			overlay.Position = pos;
			D3DTexture texture = storingInPool ? GetTexture(image, ImageCachePriority.Normal, false) : CreateTexure(image, ImageCachePriority.Normal, false);
			if (texture != null)
				d3dDevice.SetTexture(0, texture.Texture);
			OptionsHelper.SetFVF(D3DRenderer.OverlayFVF);
			d3dDevice.SetStreamSource(0, overlay.RectBuffer.Buffer, 0, D3DRenderer.OverlayVertexStride);
			d3dDevice.DrawPrimitive(D3DPRIMITIVETYPE.D3DPT_TRIANGLEFAN, 0, overlay.RectBuffer.PrimitiveCount);
			d3dDevice.SetTexture(0, null);
			if (!storingInPool && texture != null)
				texture.Dispose();
		}
		D3DTexture CreateTexure(Image image, ImageCachePriority cachePriority, bool addTransparentBorder) {
			D3DTexture texture = new D3DTexture(d3dDevice) { CachePriority = cachePriority };
			Bitmap bmp = addTransparentBorder ? AddTransparentBorder(image) : image as Bitmap;
			if(!texture.SetBitmap(bmp)) {
				this.textureCache.Clear();
				if(!texture.SetBitmap(bmp)) {
					if(addTransparentBorder)
						bmp.Dispose();
					return null;
				}
			}
			if(addTransparentBorder)
				bmp.Dispose();
			return texture;
		}
		Bitmap AddTransparentBorder(Image image) {
			Bitmap bmp = new Bitmap(image.Width + 2, image.Height + 2);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImageUnscaledAndClipped(image, new Rectangle(1,1,image.Width, image.Height));
			return bmp;
		}
		void RenderShape(D3DResourceHolder holder, IRenderItemStyle style) {
		   OptionsHelper.VertexDeclaration = this.vertexDeclaration;
		   float[] param = new float[] { this.useTransformAsDPParams ? 1.0f : -1.0f, style.StrokeWidth * 0.5f, .0f, .0f };
		   this.d3dDevice.SetVertexShaderConstantF(8, param, 1);
		   this.d3dDevice.SetPixelShaderConstantF(1, D3DOptionsHelper.ZeroShaderConstant, 1);
		   if(MapUtils.CanDrawColor(style.Fill)) {
			   this.d3dDevice.SetPixelShaderConstantF(0, D3DUtils.GetRGBAColor(style.Fill.ToArgb()), 1);
			   RenderShapeArea(holder);
		   }
		   if(MapUtils.CanDrawColor(style.Stroke)) {
			   this.d3dDevice.SetPixelShaderConstantF(0, D3DUtils.GetRGBAColor(style.Stroke.ToArgb()), 1);
			   RenderShapeContours(holder);
		   }
		}
		void RenderShapeArea(D3DResourceHolder holder) {
			for (int i = 0; i < holder.Areas.Count; i++)
				RenderData(holder.Areas[i]);
		}
		void RenderShapeContours(D3DResourceHolder holder) {
		   for (int i = 0; i < holder.Contours.Count; i++)
				RenderData(holder.Contours[i]);
		}
		void RenderData(BufferParam param) {
			if (param.PrimitiveCount > 0)
				this.d3dDevice.DrawPrimitiveUP(D3DPRIMITIVETYPE.D3DPT_TRIANGLELIST, param.PrimitiveCount, param.VerticesPtr, LiteVertexStride);
		}
		void RenderSprite(D3DSpriteRect sprite, D3DTexture texture, PointF position, byte transparency) {
			if (sprite == null || texture == null)
				return;
			Matrix4x4 transform = Transform;
			transform.Translation(position.X, position.Y, .0f);
			OptionsHelper.PixelShader = this.pixelShader;
			OptionsHelper.VertexShader = null;
			d3dDevice.SetTransform(D3DTRANSFORMSTATETYPE.WORLD, transform);
			d3dDevice.SetTexture(0, texture.Texture);
			this.d3dDevice.SetPixelShaderConstantF(1, new float[] { 1.0f, (255 - transparency) / 255.0f, 0.0f, 0.0f }, 1);
			OptionsHelper.SetFVF(D3DRenderer.FVF);
			d3dDevice.SetStreamSource(0, sprite.RectBuffer.Buffer, 0, D3DRenderer.VertexStride);
			d3dDevice.DrawPrimitive(D3DPRIMITIVETYPE.D3DPT_TRIANGLEFAN, 0, sprite.RectBuffer.PrimitiveCount);
			d3dDevice.SetTexture(0, null);
		}
		MapPoint CalcD3DOffset() {
			return new MapPoint(-viewPortSize.Width * 0.5, viewPortSize.Height * 0.5);
		}
		void CreateDeviceParameters() {
			d3dPP.Windowed = true;
			d3dPP.SwapEffect = D3DSWAPEFFECT.DISCARD;
			d3dPP.BackBufferFormat = Direct3DHelper.D3DParameters.DisplayFormat;
			d3dPP.BackBufferCount = 1;
			d3dPP.hDeviceWindow = this.targetHandle;
			d3dPP.PresentationInterval = D3DPRESENT_INTERVAL.ONE;
			d3dPP.BackBufferWidth = (uint)this.viewPortSize.Width;
			d3dPP.BackBufferHeight = (uint)this.viewPortSize.Height;
			d3dPP.EnableAutoDepthStencil = false;
			d3dPP.MultiSampleType = Direct3DHelper.D3DParameters.ActualMultisampleType;
		}
		void SetRenderOptions() {
			Matrix4x4 matview = Matrix4x4.Identity;
			D3DUtils.CheckForD3DErrors(d3dDevice.SetTransform(D3DTRANSFORMSTATETYPE.VIEW, matview), "SetTransform", true);
			Matrix4x4 matproj = Matrix4x4.CreateMatrixOrthoLH(viewPortSize.Width, viewPortSize.Height, 1.0f, -1.0f);
			D3DUtils.CheckForD3DErrors(d3dDevice.SetTransform(D3DTRANSFORMSTATETYPE.PROJECTION, matproj), "SetTransform", true);
			D3DUtils.CheckForD3DErrors(d3dDevice.SetRenderState(D3DRENDERSTATETYPE.CULLMODE, (uint)D3DCULL.NONE), "SetRenderState", true);
			D3DUtils.CheckForD3DErrors(d3dDevice.SetRenderState(D3DRENDERSTATETYPE.LIGHTING, Direct3D.FALSE), "SetRenderState", true);
			D3DUtils.CheckForD3DErrors(d3dDevice.SetRenderState(D3DRENDERSTATETYPE.ZENABLE, Direct3D.FALSE), "SetRenderState", true);
			D3DUtils.CheckForD3DErrors(d3dDevice.SetRenderState(D3DRENDERSTATETYPE.ZWRITEENABLE, Direct3D.FALSE), "SetRenderState", true);
			ViewProjection = Matrix4x4.Multiply(matview, matproj);
			d3dDevice.SetTextureStageState(0, D3DTEXTURESTAGESTATETYPE.D3DTSS_COLORARG1, Direct3D.D3DTA_DIFFUSE);
			d3dDevice.SetTextureStageState(0, D3DTEXTURESTAGESTATETYPE.D3DTSS_COLORARG2, Direct3D.D3DTA_TEXTURE);
			d3dDevice.SetSamplerState(0, D3DSAMPLERSTATETYPE.ADDRESSU, (uint)D3DTEXTUREADDRESS.CLAMP);
			d3dDevice.SetSamplerState(0, D3DSAMPLERSTATETYPE.ADDRESSV, (uint)D3DTEXTUREADDRESS.CLAMP);
			OptionsHelper.AntiAliasing = true;
			d3dDevice.SetRenderState(D3DRENDERSTATETYPE.SRCBLEND, (uint)D3DBLEND.SRCALPHA);
			d3dDevice.SetRenderState(D3DRENDERSTATETYPE.DESTBLEND, (uint)D3DBLEND.INVSRCALPHA);
			d3dDevice.SetRenderState(D3DRENDERSTATETYPE.ALPHABLENDENABLE, Direct3D.TRUE);   
		}
		bool CheckActualViewPortSize() {
			return (viewPortSize.Height == d3dPP.BackBufferHeight &&
					viewPortSize.Width == d3dPP.BackBufferWidth);
		}
		D3DRect GetRect(Size sourceSize, RectangleF rect, RectangleF clipUV, D3DRectType rectType) {
			foreach(D3DRect item in RectsPool) {
				if(item.ImageSize == sourceSize && rect.Size == item.Size && item.Type == rectType && item.ClipUV == clipUV)
					return item;
			}
			D3DRect d3dRect = rectType == D3DRectType.Overlay ? new D3DOverlayRect(d3dDevice, sourceSize, rect, clipUV) as D3DRect :
													 new D3DSpriteRect(d3dDevice, sourceSize, rect, clipUV) as D3DRect;
			if(CheckRectMemoryLimit())
				ClearRectsPool();
			RectsPool.Add(d3dRect);
			return d3dRect;
		}
		D3DTexture GetTexture(Image image, ImageCachePriority cachePriority, bool addTransparentBorder) {
			Guard.ArgumentNotNull(image, "image");
			int key = image.GetHashCode();
			D3DTexture texture = this.textureCache.GetTexture(key);
			if (texture == null) {
				texture = CreateTexure(image, cachePriority, addTransparentBorder);
				this.textureCache.Add(key, texture);	
			}
			return texture;
		}
		void ApplyTemplate(ITemplateGeometryItem template) {
			if (template == null)
				return;
			float stretch = Convert.ToSingle(template.StretchFactor);
			Transform.Scale(stretch, stretch, 1.0f);
		}
		void BeginScene(Rectangle contentBounds) {
			uint color = MapUtils.ColorToUInt(RenderContext.BackColor);
			RECT[] rects = OptionsHelper.GetRectsForClear(contentBounds);
			d3dDevice.Clear((uint)rects.Length, rects, D3DCLEAR.TARGET, color, 1.0f, 0);
			d3dDevice.BeginScene();
		}
		void EndScene() {
			d3dDevice.EndScene();
		}
		uint CheckDeviceStatus() {
			return (uint)d3dDevice.TestCooperativeLevel();
		}
		bool DeviceReset() {
			if(this.d3dDevice == null || !IsValidClientSize) return false;
			d3dPP.BackBufferWidth = (uint)viewPortSize.Width;
			d3dPP.BackBufferHeight = (uint)viewPortSize.Height;
			return D3DUtils.CheckForD3DErrors(this.d3dDevice.Reset(ref d3dPP), "Reset", false);
		}
		bool Reset() {
			Invalidate();
			if(DeviceReset()) {
				OnDeviceReset();
				return true;
			}
			return false;
		}
		void OnDeviceReset() {
			SetRenderOptions();
			OptionsHelper.Reset();
		}
		void Invalidate() {
			ClearVideoResources();
		}
		void ClearRectsPool() {
			foreach(D3DRect item in RectsPool) {
				item.Dispose();
			}
			RectsPool.Clear();
		}
		void CleanResources() {
			Invalidate();
		}
		bool CheckRectMemoryLimit() { 
			long memoryUsage = rectsPool.Count() * D3DRect.MaxRectSize;
			return memoryUsage > this.rectsMemoryLimit;
		}
		void ClearVideoResources() {
			if (this.textureCache != null)
				this.textureCache.Clear();
			ClearRectsPool();
		}
		void PreventBorderFlickering(Graphics gr) {
			gr.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, 1, 1));
		}
		void CreatePixelShader() {
			byte[] data = MapUtils.GetByteArrayFromResource("DevExpress.XtraMap.HLSL.shape.ps");
			IntPtr shaderData = MarshalHelper.AllocHGlobal(data.Length);
			try {
				MarshalHelper.Copy(data, 0, shaderData, data.Length);
				D3DUtils.CheckForD3DErrors(d3dDevice.CreatePixelShader(shaderData, out pixelShader), "CreatePixelShader", true);
			}
			finally {
				MarshalHelper.FreeHGlobal(shaderData);
			}
		}
		void CreateVertexShader() {
			byte[] data = MapUtils.GetByteArrayFromResource("DevExpress.XtraMap.HLSL.shape.vs");
			IntPtr shaderData = MarshalHelper.AllocHGlobal(data.Length);
			try {
				MarshalHelper.Copy(data, 0, shaderData, data.Length);
				D3DUtils.CheckForD3DErrors(d3dDevice.CreateVertexShader(shaderData, out vertexShader), "CreateVertexShader", true);
			}
			finally {
				MarshalHelper.FreeHGlobal(shaderData);
			}
		}
		void CreateVertexDeclaration() {
			D3DVERTEXELEMENT9[] el = new D3DVERTEXELEMENT9[3];
			el[0].Stream = 0;
			el[0].Offset = 0;
			el[0].Type = (byte)D3DDECLTYPE.FLOAT4;
			el[0].Method = (byte)D3DDECLMETHOD.DEFAULT;
			el[0].Usage = (byte)D3DDECLUSAGE.POSITION;
			el[0].UsageIndex = 0;
			el[1].Stream = 0;
			el[1].Offset = 16;
			el[1].Type = (byte)D3DDECLTYPE.FLOAT2;
			el[1].Method = (byte)D3DDECLMETHOD.DEFAULT;
			el[1].Usage = (byte)D3DDECLUSAGE.NORMAL;
			el[1].UsageIndex = 0;
			el[2].Stream = 0xFF;
			el[2].Offset = 0;
			el[2].Type = (byte)D3DDECLTYPE.UNUSED;
			el[2].Method = 0;
			el[2].Usage = 0;
			el[2].UsageIndex = 0;
			D3DUtils.CheckForD3DErrors(d3dDevice.CreateVertexDeclaration(el, out vertexDeclaration), "CreateVertexDeclaration", true);
		}
		internal void InitD3D() {
			if (this.isInitalized)
				return;
			if ((direct3D9 = Direct3D.Direct3DCreate9()) == null) {
				return;
			}
			CreateDeviceParameters();
			D3DUtils.CheckForD3DErrors(direct3D9.CreateDevice(0, D3DDEVTYPE.D3DDEVTYPE_HAL, this.targetHandle, Direct3DHelper.D3DCreateFlags, ref d3dPP, out d3dDevice), "CreateDevice", true);
			Guard.ArgumentNotNull(d3dDevice, "Device");
			OptionsHelper = new D3DOptionsHelper(this.d3dDevice);
			SetRenderOptions();
			CreatePixelShader();
			CreateVertexShader();
			CreateVertexDeclaration();
			CalculateMemoryLimit();
			this.textureCache = new TextureCache((long)(VideoMemoryLimit * (1.0f - RectsMemoryPercentage))); 
			isInitalized = true;
		}
		void CalculateMemoryLimit() {
			this.VideoMemoryLimit = (long)(d3dDevice.GetAvailableTextureMem() * AvailableVideoMemoryPercentage);
			this.rectsMemoryLimit = (long)(VideoMemoryLimit * RectsMemoryPercentage);
		}
		internal void CleanD3D() {
			CleanResources();
			if(pixelShader != null) {
				MarshalHelper.ReleaseComObject(pixelShader);
				pixelShader = null;
			}
			if(vertexShader != null) {
				MarshalHelper.ReleaseComObject(vertexShader);
				vertexShader = null;
			}
			if(vertexDeclaration != null) {
				MarshalHelper.ReleaseComObject(vertexDeclaration);
				vertexDeclaration = null;
			}
			if(d3dDevice != null) {
				MarshalHelper.ReleaseComObject(d3dDevice);
				d3dDevice = null;
			}
			if(direct3D9 != null) {
				MarshalHelper.ReleaseComObject(direct3D9);
				direct3D9 = null;
			}
			isInitalized = false;
		}
	}
}
