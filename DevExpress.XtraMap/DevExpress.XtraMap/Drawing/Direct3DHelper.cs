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
using System.Linq;
using D3D;
using DevExpress.XtraMap.Drawing.DirectX;
namespace DevExpress.XtraMap.Drawing {
	[CLSCompliant(false)]
	public struct D3DParameters {
		public uint MaxTextureHeight { get; set; }
		public uint MaxTextureWidth { get; set; }
		public uint MaxPrimitiveCount { get; set; }
		public D3DFORMAT DisplayFormat { get; set; }
		public D3DMULTISAMPLE_TYPE ActualMultisampleType { get; set; }
	}
	static class Direct3DHelper {
		internal static readonly D3DCREATE D3DCreateFlags = D3DCREATE.HARDWARE_VERTEXPROCESSING | D3DCREATE.FPU_PRESERVE | D3DCREATE.MULTITHREADED;
		internal static readonly D3DParameters D3DParameters;
		internal static readonly bool AllowUseDirectX;
		static Direct3DHelper() {
			DirectXCheckingHelper helper = new DirectXCheckingHelper();
			AllowUseDirectX = helper.CheckAllowUseDirectX();
			D3DParameters = FillD3DParameters(helper.Format, helper.MultisampleType);
		}
		static D3DParameters FillD3DParameters(D3DFORMAT format, D3DMULTISAMPLE_TYPE multismapleType) {
			D3DParameters parameters = new D3DParameters();
			IDirect3D9 direct3D9 = Direct3D.Direct3DCreate9();
			if(direct3D9 != null) {
				D3DCAPS9 caps = new D3DCAPS9();
				if(D3DUtils.CheckForD3DErrors(direct3D9.GetDeviceCaps(0, D3DDEVTYPE.D3DDEVTYPE_HAL, ref caps), "GetDeviceCaps", false)) {
					parameters.MaxPrimitiveCount = caps.MaxPrimitiveCount;
					parameters.MaxTextureHeight = caps.MaxTextureHeight;
					parameters.MaxTextureWidth = caps.MaxTextureWidth;
					parameters.DisplayFormat = format;
					parameters.ActualMultisampleType = multismapleType;
				}
				MarshalHelper.ReleaseComObject(direct3D9);
			}
			return parameters;
		}
	}
	public class DirectXCheckingHelper {
		delegate bool CheckParameter();
		string trace;
		IDirect3D9 direct3D9;
		D3DFORMAT format;
		D3DMULTISAMPLE_TYPE multisampleType;
		internal D3DFORMAT Format { get { return format; } }
		internal D3DMULTISAMPLE_TYPE MultisampleType { get { return multisampleType; } }
		public string Trace { get { return trace; } }
		public DirectXCheckingHelper() {
			this.trace = "";
			this.direct3D9 = Direct3D.Direct3DCreate9();
		}
		#region delegates
		bool CheckDirectXCreation() {
			return direct3D9 != null;
		}
		bool CheckAdapterCount() {
			return direct3D9.GetAdapterCount() >= 1;
		}
		bool CheckDeviceType() {
			return D3DUtils.CheckForD3DErrors(direct3D9.CheckDeviceType(0, D3DDEVTYPE.D3DDEVTYPE_HAL, format, format, true), "CheckDeviceType", false);
		}
		bool CheckDeviceFormat() {
			return D3DUtils.CheckForD3DErrors(direct3D9.CheckDeviceFormat(0, D3DDEVTYPE.D3DDEVTYPE_HAL, format,
																		   (uint)D3DUSAGE.RENDERTARGET, D3DRESOURCETYPE.D3DRTYPE_TEXTURE, format), "CheckDeviceFormat", false);
		}
		bool CheckDeviceMultisampleLevel(D3DMULTISAMPLE_TYPE multisampleType) {
			Int32 qualityLevels;
			bool res = D3DUtils.CheckForD3DErrors(direct3D9.CheckDeviceMultiSampleType(0, D3DDEVTYPE.D3DDEVTYPE_HAL, format, true, multisampleType, out qualityLevels), "CheckDeviceMultiSampleType", false);
			if(res && qualityLevels > 0) {
				this.multisampleType = multisampleType;
				return true;
			}
			return false;
		}
		bool CheckMultisample() {
			if(CheckDeviceMultisampleLevel(D3DMULTISAMPLE_TYPE.MULTISAMPLE_2_SAMPLES))
				return true;
			return CheckDeviceMultisampleLevel(D3DMULTISAMPLE_TYPE.MULTISAMPLE_NONE);
		}
		bool CheckDeviceCreation() {
			D3DPRESENT_PARAMETERS d3dPP = new D3DPRESENT_PARAMETERS();
			d3dPP.Windowed = true;
			d3dPP.SwapEffect = D3DSWAPEFFECT.DISCARD;
			d3dPP.BackBufferFormat = format;
			d3dPP.BackBufferCount = 1;
			d3dPP.hDeviceWindow = IntPtr.Zero;
			d3dPP.PresentationInterval = D3DPRESENT_INTERVAL.ONE;
			d3dPP.BackBufferWidth = 50;
			d3dPP.BackBufferHeight = 50;
			d3dPP.EnableAutoDepthStencil = false;
			d3dPP.MultiSampleType = this.multisampleType;
			IDirect3DDevice9 d3dDevice = null;
			if(!D3DUtils.CheckForD3DErrors(direct3D9.CreateDevice(0, D3DDEVTYPE.D3DDEVTYPE_HAL, IntPtr.Zero, Direct3DHelper.D3DCreateFlags, ref d3dPP, out d3dDevice), "CreateDevice", false))
				return false;
			MarshalHelper.ReleaseComObject(d3dDevice);
			return true;
		}
		bool CheckUseTesselation() {
			bool result = false;
			using(ITesselator tesselator = new GLUTesselator()) {
				tesselator.Create();
				List<MapPoint[]> polygon = new List<MapPoint[]>();
				MapPoint[] points = new MapPoint[4];
				points[0] = new MapPoint(.0, .0);
				points[1] = new MapPoint(.0, 500.0);
				points[2] = new MapPoint(500.0, 500.0);
				points[3] = new MapPoint(500.0, .0);
				polygon.Add(points);
				Vector3d[] resPoints = tesselator.Tesselate(polygon);
				result = resPoints != null;
			}
			return result;
		}
		#endregion
		bool TraceParameter(CheckParameter parameterFunction, string parameterName) {
			bool functionResult = parameterFunction();
			trace += string.Format("{0}: {1}\r\n", parameterName, functionResult ? "OK" : "failed");
			return functionResult;
		}
		void SetDeviceFormat() {
			D3DDISPLAYMODE displayMode = new D3DDISPLAYMODE();
			D3DUtils.CheckForD3DErrors(direct3D9.GetAdapterDisplayMode(0, ref displayMode), "GetAdapterDisplayMode", false);
			this.format = displayMode.Format;
		}
		void ReleaseObjects() {
			MarshalHelper.ReleaseComObject(direct3D9);
		}
		public bool CheckAllowUseDirectX() {
			if(!TraceParameter(CheckDirectXCreation, "DirectX creation"))
				return false;
			if(!TraceParameter(CheckAdapterCount, "Adapter count check"))
				return false;
			SetDeviceFormat();
			if(!TraceParameter(CheckDeviceType, "Check device type")) {
				ReleaseObjects();
				return false;
			}
			if(!TraceParameter(CheckDeviceFormat, "Check device format")) {
				ReleaseObjects();
				return false;
			}
			if(!TraceParameter(CheckMultisample, "Check multisample")) {
				ReleaseObjects();
				return false;
			}
			if(!TraceParameter(CheckDeviceCreation, "Create device")) {
				ReleaseObjects();
				return false;
			}
			ReleaseObjects();
			return TraceParameter(CheckUseTesselation, "Check allow tesselation");
		}
	}
}
