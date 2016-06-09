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
using D3D;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing.DirectX {
	internal class D3DOptionsHelper : MapDisposableObject {
		static readonly float[] zeroShaderConstant = new float[4];
		public static float[] ZeroShaderConstant { get { return zeroShaderConstant; } }
		readonly IntPtr clipRectPtr = MarshalHelper.AllocHGlobal(MarshalHelper.SizeOf(typeof(RECT)));
		IDirect3DDevice9 d3dDevice;
		Rectangle clipRect;
		Rectangle actualClipRect = Rectangle.Empty;
		bool antiAliasing = true;
		IDirect3DPixelShader9 currentPixelShader;
		IDirect3DVertexShader9 currentVertexShader;
		IDirect3DVertexDeclaration9 currentVertexDeclaration;
		Rectangle currentContentBounds = Rectangle.Empty;
		RECT[] contentRects = new RECT[1];
		public IDirect3DPixelShader9 PixelShader {
			set {
				if (currentPixelShader == value) return;
				currentPixelShader = value;
				SetPixelShaderCore(currentPixelShader);
			}
		}
		public IDirect3DVertexShader9 VertexShader {
			set {
				if (currentVertexShader == value) return;
				currentVertexShader = value;
				SetVertexShaderCore(currentVertexShader);
			}
		}
		public IDirect3DVertexDeclaration9 VertexDeclaration {
			set {
				if (currentVertexDeclaration == value) return;
				currentVertexDeclaration = value;
				SetVertexDeclarationCore(currentVertexDeclaration);
			}
		} 
		public bool AntiAliasing {
			get { return antiAliasing; }
			set {
				if(antiAliasing == value)
					return;
				antiAliasing = value;
				OnAntiAliasingChanged();
			}
		}
		public Rectangle ClipRect {
			get { return clipRect; }
			set {
				if(clipRect == value)
					return;
				clipRect = value;
				OnClipRectChanged();
			}
		}
		public D3DOptionsHelper(IDirect3DDevice9 device) {
			Guard.ArgumentNotNull(device, "D3Ddevice");
			this.d3dDevice = device;
			d3dDevice.SetSamplerState(0, D3DSAMPLERSTATETYPE.MAGFILTER, (uint)D3DTEXTUREFILTERTYPE.LINEAR);
			d3dDevice.SetSamplerState(0, D3DSAMPLERSTATETYPE.MINFILTER, (uint)D3DTEXTUREFILTERTYPE.POINT);
		}
		protected override void DisposeOverride() {
			MarshalHelper.FreeHGlobal(this.clipRectPtr);
		}
		void OnAntiAliasingChanged() {
			uint type = AntiAliasing ? (uint)D3DTEXTUREFILTERTYPE.LINEAR : (uint)D3DTEXTUREFILTERTYPE.POINT;
			d3dDevice.SetSamplerState(0, D3DSAMPLERSTATETYPE.MAGFILTER, type);
			d3dDevice.SetSamplerState(0, D3DSAMPLERSTATETYPE.MINFILTER, type);
		}
		void OnClipRectChanged() {
			uint clipEnabled = Direct3D.FALSE;
			if(MapUtils.IsValidSize(ClipRect.Size)) {
				clipEnabled = Direct3D.TRUE;
				if(actualClipRect != ClipRect) {
					RECT rect = new RECT() { left = ClipRect.Left, top = ClipRect.Top, right = ClipRect.Right, bottom = ClipRect.Bottom };
					MarshalHelper.StructureToPtr(rect, this.clipRectPtr, true);
					d3dDevice.SetScissorRect(this.clipRectPtr);
					this.actualClipRect = ClipRect;
				}
			} 
			this.d3dDevice.SetRenderState(D3DRENDERSTATETYPE.SCISSORTESTENABLE, clipEnabled);
		}
		void ResetClipping() {
			this.actualClipRect = Rectangle.Empty;
			ClipRect = Rectangle.Empty;
		}
		void SetVertexShaderCore(IDirect3DVertexShader9 shader) {
			this.d3dDevice.SetVertexShader(shader);
		}
		void SetPixelShaderCore(IDirect3DPixelShader9 shader) {
			this.d3dDevice.SetPixelShader(shader);
		}
		void SetVertexDeclarationCore(IDirect3DVertexDeclaration9 vertexDeclaration) {
			D3DUtils.CheckForD3DErrors(d3dDevice.SetVertexDeclaration(vertexDeclaration), "SetVertexDeclaration", true);
		}
		void ResetVertexDeclaration() {
			currentVertexDeclaration = null;
		}
		void ResetShaders() {
			currentPixelShader = null;
			currentVertexShader = null;
		}
		Rectangle CreateContentRects(Rectangle contentBounds) {
			this.contentRects = new RECT[1];
			this.contentRects[0].left = contentBounds.Left;
			this.contentRects[0].right = contentBounds.Right;
			this.contentRects[0].top = contentBounds.Top;
			this.contentRects[0].bottom = contentBounds.Bottom;
			return contentBounds;
		}
		public void SetFVF(uint vertexFormat) {
			this.d3dDevice.SetFVF(vertexFormat);
			ResetVertexDeclaration();
		}
		public void Reset() {
			OnAntiAliasingChanged();
			ResetClipping();
			ResetShaders();
			ResetVertexDeclaration();
		}
		public RECT[] GetRectsForClear(Rectangle contentBounds) {
			if (contentBounds != this.currentContentBounds) {
				this.currentContentBounds = contentBounds;
				contentBounds = CreateContentRects(contentBounds);
			}
			return contentRects;
		}
	}
}
