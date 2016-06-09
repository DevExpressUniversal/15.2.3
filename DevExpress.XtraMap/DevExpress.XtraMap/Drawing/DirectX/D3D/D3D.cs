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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
namespace D3D {
	[StructLayout(LayoutKind.Sequential)]
	public struct GUID {
		ulong Data1;
		Int16 Data2;
		Int16 Data3;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		byte[] Data4;
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DADAPTER_IDENTIFIER9 {
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		char[] driver;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		char[] description;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		char[] deviceName;
		public char[] Driver { get { return driver; } set { driver = value; } }
		public char[] Description { get { return description; } set { description = value; } }
		public char[] DeviceName { get { return deviceName; } set { deviceName = value; } }
		public uint DriverVersion{ get; set; }
		public uint VendorId{ get; set; }
		public uint DeviceId{ get; set; }
		public uint SubSysId{ get; set; }
		public uint Revision{ get; set; }
		public GUID DeviceIdentifier{ get; set; }
		public uint WHQLLevel{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DDISPLAYMODE {
		public uint Width{ get; set; }
		public uint Height{ get; set; }
		public uint RefreshRate{ get; set; }
		public D3DFORMAT Format{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DVSHADERCAPS2_0 {
		public uint Caps{ get; set; }
		public int DynamicFlowControlDepth{ get; set; }
		public int NumTemps{ get; set; }
		public int StaticFlowControlDepth{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DPSHADERCAPS2_0 {
		public uint Caps{ get; set; }
		public int DynamicFlowControlDepth{ get; set; }
		public int NumTemps{ get; set; }
		public int StaticFlowControlDepth{ get; set; }
		public int NumInstructionSlots{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DCAPS9 {
		public D3DDEVTYPE DeviceType{ get; set; }
		public uint AdapterOrdinal{ get; set; }
		public uint Caps{ get; set; }
		public uint Caps2{ get; set; }
		public uint Caps3{ get; set; }
		public uint PresentationIntervals{ get; set; }
		public uint CursorCaps{ get; set; }
		public uint DevCaps{ get; set; }
		public uint PrimitiveMiscCaps{ get; set; }
		public uint RasterCaps{ get; set; }
		public uint ZCmpCaps{ get; set; }
		public uint SrcBlendCaps{ get; set; }
		public uint DestBlendCaps{ get; set; }
		public uint AlphaCmpCaps{ get; set; }
		public uint ShadeCaps{ get; set; }
		public uint TextureCaps{ get; set; }
		public uint TextureFilterCaps{ get; set; }
		public uint CubeTextureFilterCaps{ get; set; }
		public uint VolumeTextureFilterCaps{ get; set; }
		public uint TextureAddressCaps{ get; set; }
		public uint VolumeTextureAddressCaps{ get; set; }
		public uint LineCaps{ get; set; }
		public uint MaxTextureWidth { get; set; }
		public uint MaxTextureHeight { get; set; }
		public uint MaxVolumeExtent{ get; set; }
		public uint MaxTextureRepeat{ get; set; }
		public uint MaxTextureAspectRatio{ get; set; }
		public uint MaxAnisotropy{ get; set; }
		public float MaxVertexW{ get; set; }
		public float GuardBandLeft{ get; set; }
		public float GuardBandTop{ get; set; }
		public float GuardBandRight{ get; set; }
		public float GuardBandBottom{ get; set; }
		public float ExtentsAdjust{ get; set; }
		public uint StencilCaps{ get; set; }
		public uint FVFCaps{ get; set; }
		public uint TextureOpCaps{ get; set; }
		public uint MaxTextureBlendStages{ get; set; }
		public uint MaxSimultaneousTextures{ get; set; }
		public uint VertexProcessingCaps{ get; set; }
		public uint MaxActiveLights{ get; set; }
		public uint MaxUserClipPlanes{ get; set; }
		public uint MaxVertexBlendMatrices{ get; set; }
		public uint MaxVertexBlendMatrixIndex{ get; set; }
		public float MaxPointSize{ get; set; }
		public uint MaxPrimitiveCount{ get; set; }
		public uint MaxVertexIndex{ get; set; }
		public uint MaxStreams{ get; set; }
		public uint MaxStreamStride{ get; set; }
		public uint VertexShaderVersion{ get; set; }
		public uint MaxVertexShaderConst{ get; set; }
		public uint PixelShaderVersion{ get; set; }
		public float PixelShader1xMaxValue{ get; set; }
		public uint DevCaps2{ get; set; }
		public float MaxNpatchTessellationLevel{ get; set; }
		public uint Reserved5{ get; set; }
		public uint MasterAdapterOrdinal{ get; set; }
		public uint AdapterOrdinalInGroup{ get; set; }
		public uint NumberOfAdaptersInGroup{ get; set; }
		public uint DeclTypes{ get; set; }
		public uint NumSimultaneousRTs{ get; set; }
		public uint StretchRectFilterCaps{ get; set; }
		public D3DVSHADERCAPS2_0 VS20Caps{ get; set; }
		public D3DPSHADERCAPS2_0 PS20Caps{ get; set; }
		public uint VertexTextureFilterCaps{ get; set; }
		public uint MaxVShaderInstructionsExecuted{ get; set; }
		public uint MaxPShaderInstructionsExecuted{ get; set; }
		public uint MaxVertexShader30InstructionSlots{ get; set; }
		public uint MaxPixelShader30InstructionSlots{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DPRESENT_PARAMETERS {
		public uint BackBufferWidth{ get; set; }
		public uint BackBufferHeight{ get; set; }
		public D3DFORMAT BackBufferFormat{ get; set; }
		public uint BackBufferCount{ get; set; }
		public D3DMULTISAMPLE_TYPE MultiSampleType{ get; set; }
		public uint MultiSampleQuality{ get; set; }
		public D3DSWAPEFFECT SwapEffect{ get; set; }
		public IntPtr hDeviceWindow{ get; set; }
		public bool Windowed{ get; set; }
		public bool EnableAutoDepthStencil{ get; set; }
		public D3DFORMAT AutoDepthStencilFormat{ get; set; }
		public uint Flags{ get; set; }
		public uint FullScreen_RefreshRateInHz{ get; set; }
		public D3DPRESENT_INTERVAL PresentationInterval{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DDEVICE_CREATION_PARAMETERS {
		public uint AdapterOrdinal{ get; set; }
		public D3DDEVTYPE DeviceType{ get; set; }
		public IntPtr hFocusWindow{ get; set; }
		public uint BehaviorFlags{ get; set; }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DVIEWPORT9 {
		public uint X { get; set; }
		public uint Y { get; set; }
		public uint Width { get; set; }
		public uint Height { get; set; }
		public float MinZ { get; set; }
		public float MaxZ { get; set; }
}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DLOCKED_RECT {
		public int Pitch { get; set; }
		public IntPtr pBits { get; set; }
	}
	[StructLayout(LayoutKind.Explicit)]
	public struct RECT {
		[FieldOffset(0)]
		[SuppressMessage("Microsoft.Design", "CA1051:Do not declare visible instance fields")]
		public int left;
		[FieldOffset(4)]
		[SuppressMessage("Microsoft.Design", "CA1051:Do not declare visible instance fields")]
		public int top;
		[FieldOffset(8)]
		[SuppressMessage("Microsoft.Design", "CA1051:Do not declare visible instance fields")]
		public int right;
		[FieldOffset(12)]
		[SuppressMessage("Microsoft.Design", "CA1051:Do not declare visible instance fields")]
		public int bottom;
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DSURFACE_DESC {
		public D3DFORMAT Format { get; set; }
		public D3DRESOURCETYPE Type { get; set; }
		public uint Usage { get; set; }
		public D3DPOOL Pool { get; set; }
		public D3DMULTISAMPLE_TYPE MultiSampleType { get; set; }
		public uint MultiSampleQuality { get; set; }
		public uint Width { get; set; }
		public uint Height { get; set; }
}
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[CLSCompliant(false)]
	public struct D3DVERTEXELEMENT9 {
		public Int16 Stream { get; set; }
		public Int16 Offset { get; set; }
		public byte Type { get; set; }
		public byte Method { get; set; }
		public byte Usage { get; set; }
		public byte UsageIndex { get; set; } 
	}
	public enum D3DSWAPEFFECT {
		DISCARD = 1,
		FLIP = 2,
		COPY = 3
	}
	public enum D3DBACKBUFFER_TYPE {
		MONO = 0,
		LEFT = 1,
		RIGHT = 2,
	}
	public enum D3DTEXTUREFILTERTYPE {
		NONE = 0,
		POINT = 1,
		LINEAR = 2,
		ANISOTROPIC = 3,
		PYRAMIDALQUAD = 6,
		GAUSSIANQUAD = 7,
	}
	public enum D3DSAMPLERSTATETYPE {
		ADDRESSU = 1,
		ADDRESSV = 2,
		ADDRESSW = 3,
		BORDERCOLOR = 4,
		MAGFILTER = 5,
		MINFILTER = 6,
		MIPFILTER = 7,
		MIPMAPLODBIAS = 8,
		MAXMIPLEVEL = 9,
		MAXANISOTROPY = 10,
		SRGBTEXTURE = 11,
		ELEMENTINDEX = 12,
		DMAPOFFSET = 13
	}
	public enum D3DRESOURCETYPE {
		D3DRTYPE_SURFACE = 1,
		D3DRTYPE_VOLUME = 2,
		D3DRTYPE_TEXTURE = 3,
		D3DRTYPE_VOLUMETEXTURE = 4,
		D3DRTYPE_CUBETEXTURE = 5,
		D3DRTYPE_VERTEXBUFFER = 6,
		D3DRTYPE_INDEXBUFFER = 7
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Naming", "CA1712:DoNotPrefixEnumValuesWithTypeName")]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DDEVTYPE : uint {
		D3DDEVTYPE_HAL = 1,
		D3DDEVTYPE_REF = 2,
		D3DDEVTYPE_SW = 3,
		D3DDEVTYPE_NULLREF = 4
	}
	public enum D3DTEXTURESTAGESTATETYPE
{
	D3DTSS_COLOROP		=  1, 
	D3DTSS_COLORARG1	  =  2, 
	D3DTSS_COLORARG2	  =  3, 
	D3DTSS_ALPHAOP		=  4, 
	D3DTSS_ALPHAARG1	  =  5, 
	D3DTSS_ALPHAARG2	  =  6, 
	D3DTSS_BUMPENVMAT00   =  7, 
	D3DTSS_BUMPENVMAT01   =  8, 
	D3DTSS_BUMPENVMAT10   =  9, 
	D3DTSS_BUMPENVMAT11   = 10, 
	D3DTSS_TEXCOORDINDEX  = 11, 
	D3DTSS_BUMPENVLSCALE  = 22, 
	D3DTSS_BUMPENVLOFFSET = 23, 
	D3DTSS_TEXTURETRANSFORMFLAGS = 24, 
	D3DTSS_COLORARG0	  = 26, 
	D3DTSS_ALPHAARG0	  = 27, 
	D3DTSS_RESULTARG	  = 28, 
	D3DTSS_CONSTANT	   = 32
	}
	public enum D3DMULTISAMPLE_TYPE {
		MULTISAMPLE_NONE = 0,
		MULTISAMPLE_NONMASKABLE = 1,
		MULTISAMPLE_2_SAMPLES = 2,
		MULTISAMPLE_3_SAMPLES = 3,
		MULTISAMPLE_4_SAMPLES = 4,
		MULTISAMPLE_5_SAMPLES = 5,
		MULTISAMPLE_6_SAMPLES = 6,
		MULTISAMPLE_7_SAMPLES = 7,
		MULTISAMPLE_8_SAMPLES = 8,
		MULTISAMPLE_9_SAMPLES = 9,
		MULTISAMPLE_10_SAMPLES = 10,
		MULTISAMPLE_11_SAMPLES = 11,
		MULTISAMPLE_12_SAMPLES = 12,
		MULTISAMPLE_13_SAMPLES = 13,
		MULTISAMPLE_14_SAMPLES = 14,
		MULTISAMPLE_15_SAMPLES = 15,
		MULTISAMPLE_16_SAMPLES = 16
	}
	public enum D3DFORMAT {
		D3DFMT_UNKNOWN = 0,
		D3DFMT_R8G8B8 = 20,
		D3DFMT_A8R8G8B8 = 21,
		D3DFMT_X8R8G8B8 = 22,
		D3DFMT_R5G6B5 = 23,
		D3DFMT_X1R5G5B5 = 24,
		D3DFMT_A1R5G5B5 = 25,
		D3DFMT_A4R4G4B4 = 26,
		D3DFMT_R3G3B2 = 27,
		D3DFMT_A8 = 28,
		D3DFMT_A8R3G3B2 = 29,
		D3DFMT_X4R4G4B4 = 30,
		D3DFMT_A2B10G10R10 = 31,
		D3DFMT_A8B8G8R8 = 32,
		D3DFMT_X8B8G8R8 = 33,
		D3DFMT_G16R16 = 34,
		D3DFMT_A2R10G10B10 = 35,
		D3DFMT_A16B16G16R16 = 36,
		D3DFMT_A8P8 = 40,
		D3DFMT_P8 = 41,
		D3DFMT_L8 = 50,
		D3DFMT_A8L8 = 51,
		D3DFMT_A4L4 = 52,
		D3DFMT_V8U8 = 60,
		D3DFMT_L6V5U5 = 61,
		D3DFMT_X8L8V8U8 = 62,
		D3DFMT_Q8W8V8U8 = 63,
		D3DFMT_V16U16 = 64,
		D3DFMT_A2W10V10U10 = 67,
		D3DFMT_D16_LOCKABLE = 70,
		D3DFMT_D32 = 71,
		D3DFMT_D15S1 = 73,
		D3DFMT_D24S8 = 75,
		D3DFMT_D24X8 = 77,
		D3DFMT_D24X4S4 = 79,
		D3DFMT_D16 = 80,
		D3DFMT_D32F_LOCKABLE = 82,
		D3DFMT_D24FS8 = 83,
		D3DFMT_L16 = 81,
		D3DFMT_VERTEXDATA = 100,
		D3DFMT_INDEX16 = 101,
		D3DFMT_INDEX32 = 102,
		D3DFMT_Q16W16V16U16 = 110,
		D3DFMT_R16F = 111,
		D3DFMT_G16R16F = 112,
		D3DFMT_A16B16G16R16F = 113,
		D3DFMT_R32F = 114,
		D3DFMT_G32R32F = 115,
		D3DFMT_A32B32G32R32F = 116,
		D3DFMT_CxV8U8 = 117,
		D3DFMT_FORCE_DWORD = 0x7fffffff
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	[Flags]
	public enum D3DCREATE : uint {
		FPU_PRESERVE = 0x00000002,
		MULTITHREADED = 0x00000004,
		PUREDEVICE = 0x00000010,
		SOFTWARE_VERTEXPROCESSING = 0x00000020,
		HARDWARE_VERTEXPROCESSING = 0x00000040,
		MIXED_VERTEXPROCESSING = 0x00000080,
		DISABLE_DRIVER_MANAGEMENT = 0x00000100,
		ADAPTERGROUP_DEVICE = 0x00000200,
		DISABLE_DRIVER_MANAGEMENT_EX = 0x00000400,
		NOWINDOWCHANGES = 0x00000800
	}
	[CLSCompliant(false)]
	[Flags]
	public enum D3DCLEAR  {
		TARGET = 0x00000001,
		ZBUFFER = 0x00000002,
		STENCIL = 0x00000004
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	[Flags]
	public enum D3DUSAGE : uint {
		NONE = 0x00000000,
		RENDERTARGET = 0x00000001,
		DEPTHSTENCIL = 0x00000002,
		DYNAMIC = 0x00000200,
		AUTOGENMIPMAP = 0x00000400,
		DMAP = 0x00004000,
		QUERY_LEGACYBUMPMAP = 0x00008000,
		QUERY_SRGBREAD = 0x00010000,
		QUERY_FILTER = 0x00020000,
		QUERY_SRGBWRITE = 0x00040000,
		QUERY_POSTPIXELSHADER_BLENDING = 0x00080000,
		QUERY_VERTEXTEXTURE = 0x00100000,
		QUERY_WRAPANDMIP = 0x00200000,
		WRITEONLY = 0x00000008,
		SOFTWAREPROCESSING = 0x00000010,
		DONOTCLIP = 0x00000020,
		POINTS = 0x00000040,
		RTPATCHES = 0x00000080,
		NPATCHES = 0x00000100
	}
	[Flags]
	[SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	[CLSCompliant(false)]
	public enum D3DFVF : uint {
		[SuppressMessage("Microsoft.Naming", "CA1700:DoNotNameEnumValuesReserved")]
		RESERVED0 = 0x001,
		POSITION_MASK = 0x400E,
		XYZ = 0x002,
		XYZRHW = 0x004,
		XYZB1 = 0x006,
		XYZB2 = 0x008,
		XYZB3 = 0x00a,
		XYZB4 = 0x00c,
		XYZB5 = 0x00e,
		XYZW = 0x4002,
		NORMAL = 0x010,
		PSIZE = 0x020,
		DIFFUSE = 0x040,
		SPECULAR = 0x080,
		TEXCOUNT_MASK = 0xf00,
		TEXCOUNT_SHIFT = 8,
		TEX0 = 0x000,
		TEX1 = 0x100,
		TEX2 = 0x200,
		TEX3 = 0x300,
		TEX4 = 0x400,
		TEX5 = 0x500,
		TEX6 = 0x600,
		TEX7 = 0x700,
		TEX8 = 0x800,
		LASTBETA_UBYTE4 = 0x1000,
		LASTBETA_D3DCOLOR = 0x8000
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	[Flags]
	public enum D3DLOCK : uint {
		READONLY = 0x00000010,
		DISCARD = 0x00002000,
		NOOVERWRITE = 0x00001000,
		NOSYSLOCK = 0x00000800,
		DONOTWAIT = 0x00004000,
		NO_DIRTY_UPDATE = 0x00008000
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DPRESENT_INTERVAL :uint {
		DEFAULT = 0x00000000,
		ONE = 0x00000001,
		TWO = 0x00000002,
		THREE = 0x00000004,
		FOUR = 0x00000008,
		IMMEDIATE = 0x80000000
	}
	public enum D3DPOOL {
		DEFAULT = 0,
		MANAGED = 1,
		SYSTEMMEM = 2,
		SCRATCH = 3
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DPRIMITIVETYPE : uint {
		D3DPT_POINTLIST = 1,
		D3DPT_LINELIST = 2,
		D3DPT_LINESTRIP = 3,
		D3DPT_TRIANGLELIST = 4,
		D3DPT_TRIANGLESTRIP = 5,
		D3DPT_TRIANGLEFAN = 6,
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DTRANSFORMSTATETYPE : uint {
		VIEW		  = 2,
		PROJECTION	= 3,
		TEXTURE0	  = 16,
		TEXTURE1	  = 17,
		TEXTURE2	  = 18,
		TEXTURE3	  = 19,
		TEXTURE4	  = 20,
		TEXTURE5	  = 21,
		TEXTURE6	  = 22,
		TEXTURE7	  = 23,
		WORLD		 = 256,
		WORLD1		= 257,
		WORLD2		= 258,
		WORLD3		= 259
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DCULL : uint {
		NONE = 1,
		CW   = 2,
		CCW  = 3
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DFILLMODE : uint {
		POINT = 1,
		WIREFRAME = 2,
		SOLID = 3
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DRENDERSTATETYPE : uint {
		ZENABLE				   = 7,	
		FILLMODE				  = 8,	
		SHADEMODE				 = 9,	
		ZWRITEENABLE			  = 14,   
		ALPHATESTENABLE		   = 15,   
		LASTPIXEL				 = 16,   
		SRCBLEND				  = 19,   
		DESTBLEND				 = 20,   
		CULLMODE				  = 22,   
		ZFUNC					 = 23,   
		ALPHAREF				  = 24,   
		ALPHAFUNC				 = 25,   
		DITHERENABLE			  = 26,   
		ALPHABLENDENABLE		  = 27,   
		FOGENABLE				 = 28,   
		SPECULARENABLE			= 29,   
		FOGCOLOR				  = 34,   
		FOGTABLEMODE			  = 35,   
		FOGSTART				  = 36,   
		FOGEND					= 37,   
		FOGDENSITY				= 38,   
		RANGEFOGENABLE			= 48,   
		STENCILENABLE			 = 52,  
		STENCILFAIL			   = 53,   
		STENCILZFAIL			  = 54,   
		STENCILPASS			   = 55,  
		STENCILFUNC			   = 56,   
		STENCILREF				= 57,   
		STENCILMASK			   = 58,  
		STENCILWRITEMASK		  = 59,  
		TEXTUREFACTOR			 = 60,   
		WRAP0					 = 128,  
		WRAP1					 = 129,  
		WRAP2					 = 130,  
		WRAP3					 = 131,  
		WRAP4					 = 132,  
		WRAP5					 = 133,  
		WRAP6					 = 134,  
		WRAP7					 = 135,  
		CLIPPING				  = 136,
		LIGHTING				  = 137,
		AMBIENT				   = 139,
		FOGVERTEXMODE			 = 140,
		COLORVERTEX			   = 141,
		LOCALVIEWER			   = 142,
		NORMALIZENORMALS		  = 143,
		DIFFUSEMATERIALSOURCE	 = 145,
		SPECULARMATERIALSOURCE	= 146,
		AMBIENTMATERIALSOURCE	 = 147,
		EMISSIVEMATERIALSOURCE	= 148,
		VERTEXBLEND			   = 151,
		CLIPPLANEENABLE		   = 152,
		POINTSIZE				 = 154,  
		POINTSIZE_MIN			 = 155,   
		POINTSPRITEENABLE		 = 156,   
		POINTSCALEENABLE		  = 157,   
		POINTSCALE_A			  = 158,   
		POINTSCALE_B			  = 159,   
		POINTSCALE_C			  = 160,   
		MULTISAMPLEANTIALIAS	  = 161,  
		MULTISAMPLEMASK		   = 162,  
		PATCHEDGESTYLE			= 163,  
		DEBUGMONITORTOKEN		 = 165,  
		POINTSIZE_MAX			 = 166,   
		INDEXEDVERTEXBLENDENABLE  = 167,
		COLORWRITEENABLE		  = 168,  
		TWEENFACTOR			   = 170,   
		BLENDOP				   = 171,   
		POSITIONDEGREE			= 172,   
		NORMALDEGREE			  = 173,   
		SCISSORTESTENABLE		 = 174,
		SLOPESCALEDEPTHBIAS	   = 175,
		ANTIALIASEDLINEENABLE	 = 176,
		MINTESSELLATIONLEVEL	  = 178,
		MAXTESSELLATIONLEVEL	  = 179,
		ADAPTIVETESS_X			= 180,
		ADAPTIVETESS_Y			= 181,
		ADAPTIVETESS_Z			= 182,
		ADAPTIVETESS_W			= 183,
		ENABLEADAPTIVETESSELLATION = 184,
		TWOSIDEDSTENCILMODE	   = 185,   
		CCW_STENCILFAIL		   = 186,   
		CCW_STENCILZFAIL		  = 187,   
		CCW_STENCILPASS		   = 188,   
		CCW_STENCILFUNC		   = 189,   
		COLORWRITEENABLE1		 = 190,   
		COLORWRITEENABLE2		 = 191,   
		COLORWRITEENABLE3		 = 192,   
		BLENDFACTOR			   = 193,   
		SRGBWRITEENABLE		   = 194,   
		DEPTHBIAS				 = 195,
		WRAP8					 = 198,   
		WRAP9					 = 199,
		WRAP10					= 200,
		WRAP11					= 201,
		WRAP12					= 202,
		WRAP13					= 203,
		WRAP14					= 204,
		WRAP15					= 205,
		SEPARATEALPHABLENDENABLE  = 206,  
		SRCBLENDALPHA			 = 207,  
		DESTBLENDALPHA			= 208,  
		BLENDOPALPHA			  = 209  
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DCMPFUNC : uint {
		NEVER = 1,
		LESS = 2,
		EQUAL = 3,
		LESSEQUAL = 4,
		GREATER = 5,
		NOTEQUAL = 6,
		GREATEREQUAL = 7,
		ALWAYS = 8
	}
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum D3DTEXTUREADDRESS : uint {
		WRAP = 1,
		MIRROR = 2,
		CLAMP = 3,
		BORDER = 4,
		MIRRORONCE = 5
	}
	[CLSCompliant(false)]
	public enum D3DDECLTYPE  {
		FLOAT1 = 0, 
		FLOAT2 = 1,  
		FLOAT3 = 2,  
		FLOAT4 = 3,  
		D3DCOLOR = 4,  
		UBYTE4 = 5,  
		SHORT2 = 6,  
		SHORT4 = 7,  
		UBYTE4N = 8,  
		SHORT2N = 9,  
		SHORT4N = 10,  
		USHORT2N = 11,  
		USHORT4N = 12,  
		UDEC3 = 13,  
		DEC3N = 14,  
		FLOAT16_2 = 15,  
		FLOAT16_4 = 16,  
		UNUSED = 17  
	}
	public enum D3DDECLMETHOD {
		DEFAULT = 0,
		PARTIALU,
		PARTIALV,
		CROSSUV,	
		UV,
		LOOKUP,			   
		LOOKUPPRESAMPLED	 
	}
	public enum D3DDECLUSAGE {
		POSITION = 0,
		BLENDWEIGHT,   
		BLENDINDICES,  
		NORMAL,		
		PSIZE,		 
		TEXCOORD,	  
		TANGENT,	   
		BINORMAL,	  
		TESSFACTOR,	
		POSITIONT,	 
		COLOR,		 
		FOG,		   
		DEPTH,		 
		SAMPLE		
	}
	public enum D3DBLEND {
		ZERO = 1,
		ONE = 2,
		SRCCOLOR = 3,
		INVSRCCOLOR = 4,
		SRCALPHA = 5,
		INVSRCALPHA = 6,
		DESTALPHA = 7,
		INVDESTALPHA = 8,
		DESTCOLOR = 9,
		INVDESTCOLOR = 10,
		SRCALPHASAT = 11,
		BOTHSRCALPHA = 12,
		BOTHINVSRCALPHA = 13,
		BLENDFACTOR = 14, 
		INVBLENDFACTOR = 15
	}
	[CLSCompliant(false)]
	public static class Direct3D {
		public const uint FALSE = 0;
		public const uint TRUE = 1;
		public const int D3DTA_DIFFUSE = 0x00000000;
		public const int D3DTA_TEXTURE = 0x00000002;
		public const int D3DPRESENTFLAG_DISCARD_DEPTHSTENCIL = 0x00000002;
		public const uint D3D_OK = 0x00000000;
		public const uint D3DERR_DEVICELOST = 0x88760868;
		public const uint D3DERR_DEVICENOTRESET = 0x88760869;
		public const uint D3DERR_OUTOFVIDEOMEMORY = 0x8876017c;
		public const uint E_OUTOFMEMORY = 0x8007000e;
		public const string STRERR_OUTOFVIDEOMEMORY = "Out of video memory";
#if DEBUG
		public const uint D3D_SDK_VERSION = (32 | 0x80000000);
#else
		public const uint D3D_SDK_VERSION = 32;
#endif
		[DllImport("d3d9.dll")]
		internal static extern IDirect3D9 Direct3DCreate9(uint sdkVersion);
		[SecuritySafeCritical]
		public static IDirect3D9 Direct3DCreate9() {
			return Direct3DCreate9(D3D_SDK_VERSION);
		}
	}
}
