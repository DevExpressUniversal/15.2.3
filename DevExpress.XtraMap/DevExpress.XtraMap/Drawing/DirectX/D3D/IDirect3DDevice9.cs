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
using System.Runtime.InteropServices;
namespace D3D {
	[ComImport, Guid("D0223B96-BF7A-43fd-92BD-A43B0D82B9EB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	public interface IDirect3DDevice9 {
		[PreserveSig]
		Int64 TestCooperativeLevel();
		[PreserveSig]
		uint GetAvailableTextureMem();
		[PreserveSig]
		uint EvictManagedResources();
		[PreserveSig]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		uint GetDirect3D(out IDirect3D9 ppD3D9);
		[PreserveSig]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		uint GetDeviceCaps(ref D3DCAPS9 pCaps);
		[return: MarshalAs(UnmanagedType.Struct)]
		D3DDISPLAYMODE GetDisplayMode(uint iSwapChain);
		[return: MarshalAs(UnmanagedType.Struct)]
		D3DDEVICE_CREATION_PARAMETERS GetCreationParameters();
		void SetCursorProperties_Placeholder();
		void SetCursorPosition_Placeholder();
		void ShowCursor(bool show);
		void CreateAdditionalSwapChain_Placeholder();
		void GetSwapChain_Placeholder();
		void GetNumberOfSwapChains_Placeholder();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		[PreserveSig]
		int Reset([In] ref D3DPRESENT_PARAMETERS pPresentationParameters);
		[PreserveSig]
		int Present(IntPtr sourceRect, IntPtr destRect, IntPtr destWindowOverride, IntPtr dirtyRegion);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int GetBackBuffer(uint iSwapChain, uint iBackBuffer, D3DBACKBUFFER_TYPE Type,out IDirect3DSurface9 pBackBuffer);
		void GetRasterStatus_Placeholder();
		void SetDialogBoxMode_Placeholder();
		void SetGammaRamp_Placeholder();
		void GetGammaRamp_Placeholder();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		[PreserveSig]
		int CreateTexture([In]uint Width, [In]uint Height, [In]uint Levels, [In]uint Usage, [In]D3DFORMAT Format, [In]D3DPOOL Pool, [Out]out IDirect3DTexture9 ppTexture, [In] IntPtr pSharedHandle);
		void CreateVolumeTexture_Placeholder();
		void CreateCubeTexture_Placeholder();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		[PreserveSig]
		int CreateVertexBuffer([In] uint length, [In] D3DUSAGE usage, [In] uint FVF, [In] D3DPOOL pool, [Out] out IDirect3DVertexBuffer9 ppDirect3DVertexBuffer9, [In] IntPtr pSharedHandle);
		void CreateIndexBuffer_Placeholder();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int CreateRenderTarget(uint Width, uint Height, D3DFORMAT Format, D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 pSurface, IntPtr pSharedHandle);
		void CreateDepthStencilSurface_Placeholder();
		void UpdateSurface_Placeholder();
		void UpdateTexture_Placeholder();
		[PreserveSig]
		int  GetRenderTargetData(IDirect3DSurface9 pRenderTarget, IDirect3DSurface9 pDestSurface);
		int GetFrontBufferData(uint iSwapChain, IDirect3DSurface9 pDestSurface);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		int StretchRect(IDirect3DSurface9 pSourceSurface,  ref RECT pSourceRect, IDirect3DSurface9 pDestSurface,  ref RECT pDestRect, D3DTEXTUREFILTERTYPE Filter);
		void ColorFill_Placeholder();
		[PreserveSig]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int CreateOffscreenPlainSurface(uint Width, uint Height, D3DFORMAT Format, D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);
		int SetRenderTarget(uint RenderTargetIndex, IDirect3DSurface9 pRenderTarget);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int GetRenderTarget(uint RenderTargetIndex, out IDirect3DSurface9 pRenderTarget);
		void SetDepthStencilSurface_Placeholder();
		void GetDepthStencilSurface_Placeholder();
		int BeginScene();
		int EndScene();
		int Clear(uint count, [MarshalAs(UnmanagedType.LPArray)]RECT[] rects, D3DCLEAR flags, uint color, float z, uint stencil);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		int SetTransform(D3DTRANSFORMSTATETYPE State, ref Matrix4x4 pMatrix);
		void GetTransform_Placeholder();
		void MultiplyTransform_Placeholder();
		int SetViewport(D3DVIEWPORT9 pViewport);
		void GetViewport_Placeholder();
		void SetMaterial_Placeholder();
		void GetMaterial_Placeholder();
		void SetLight_Placeholder();
		void GetLight_Placeholder();
		void LightEnable_Placeholder();
		void GetLightEnable_Placeholder();
		void SetClipPlane_Placeholder();
		void GetClipPlane_Placeholder();
		int SetRenderState(D3DRENDERSTATETYPE State, uint Value);
		void GetRenderState_Placeholder();
		void CreateStateBlock_Placeholder();
		void BeginStateBlock_Placeholder();
		void EndStateBlock_Placeholder();
		void SetClipStatus_Placeholder();
		void GetClipStatus_Placeholder();
		void GetTexture_Placeholder();
		int SetTexture(uint Stage, IDirect3DTexture9 pTexture);
		void GetTextureStageState_Placeholder();
		int SetTextureStageState(uint Stage, D3DTEXTURESTAGESTATETYPE Type, uint Value);
		void GetSamplerState_Placeholder();
		int SetSamplerState(uint Sampler,D3DSAMPLERSTATETYPE Type, uint Value);
		void ValidateDevice_Placeholder();
		void SetPaletteEntries_Placeholder();
		void GetPaletteEntries_Placeholder();
		void SetCurrentTexturePalette_Placeholder();
		void GetCurrentTexturePalette_Placeholder();
		int SetScissorRect([In] IntPtr pRect);
		void GetScissorRect_Placeholder();
		void SetSoftwareVertexProcessing_Placeholder();
		void GetSoftwareVertexProcessing_Placeholder();
		void SetNPatchMode_Placeholder();
		void GetNPatchMode_Placeholder();
		int DrawPrimitive(D3DPRIMITIVETYPE PrimitiveType, uint StartVertex, uint PrimitiveCount);
		void DrawIndexedPrimitive_Placeholder();
		[PreserveSig]
		int DrawPrimitiveUP(D3DPRIMITIVETYPE PrimitiveType, uint PrimitiveCount, [In] IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);
		void DrawIndexedPrimitiveUP_Placeholder();
		void ProcessVertices_Placeholder();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int CreateVertexDeclaration( [In, MarshalAs(UnmanagedType.LPArray)] D3DVERTEXELEMENT9[] pVertexElements, out IDirect3DVertexDeclaration9 pDecl);
		int SetVertexDeclaration(IDirect3DVertexDeclaration9 pDecl);
		void GetVertexDeclaration_Placeholder();
		int SetFVF(uint FVF);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int GetFVF(out uint pFVF);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int CreateVertexShader(IntPtr pFunction, out IDirect3DVertexShader9 pShader);
		int SetVertexShader(IDirect3DVertexShader9 pShader);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int GetVertexShader(out IDirect3DVertexShader9 pShader);
		int SetVertexShaderConstantF(uint StartRegister, [MarshalAs(UnmanagedType.LPArray)]float[] pConstantData, uint Vector4fCount);
		void GetVertexShaderConstantF_Placeholder();
		void SetVertexShaderConstantI_Placeholder();
		void GetVertexShaderConstantI_Placeholder();
		void SetVertexShaderConstantB_Placeholder();
		void GetVertexShaderConstantB_Placeholder();
		int SetStreamSource(uint StreamNumber, [In] IDirect3DVertexBuffer9 pStreamData, uint OffsetInBytes, uint Stride);
		void GetStreamSource_Placeholder();
		void SetStreamSourceFreq_Placeholder();
		void GetStreamSourceFreq_Placeholder();
		void SetIndices_Placeholder();
		void GetIndices_Placeholder();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int CreatePixelShader(IntPtr pFunction, out IDirect3DPixelShader9 ppShader);
		int SetPixelShader(IDirect3DPixelShader9 pShader);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		int GetPixelShader(out IDirect3DPixelShader9 pShader);
		int SetPixelShaderConstantF(uint StartRegister, [MarshalAs(UnmanagedType.LPArray)]float[] pConstantData, uint Vector4fCount);
		void GetPixelShaderConstantF_Placeholder();
		void SetPixelShaderConstantI_Placeholder();
		void GetPixelShaderConstantI_Placeholder();
		void SetPixelShaderConstantB_Placeholder();
		void GetPixelShaderConstantB_Placeholder();
		void DrawRectPatch_Placeholder();
		void DrawTriPatch_Placeholder();
		void DeletePatch_Placeholder();
		void CreateQuery_Placeholder();
	}
}
