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
using System.Text;
namespace D3D {
	[ComImport, Guid("81BDCBCA-64D4-426d-AE8D-AD0147F4275C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	public interface IDirect3D9 {
		void RegisterSoftwareDevice(IntPtr initFunction);
		[PreserveSig]
		uint GetAdapterCount();
		[return: MarshalAs(UnmanagedType.Struct)]
		D3DADAPTER_IDENTIFIER9 GetAdapterIdentifier(int adapter, uint flags);
		uint GetAdapterModeCount(uint adapter, D3DFORMAT format);
		[return: MarshalAs(UnmanagedType.Struct)]
		D3DDISPLAYMODE EnumAdapterModes(uint adapter, D3DFORMAT format, uint Mode);
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		[PreserveSig]
		int GetAdapterDisplayMode(uint Adapter, ref D3DDISPLAYMODE pMode);
		[PreserveSig]
		int CheckDeviceType(uint adapter, D3DDEVTYPE devType, D3DFORMAT adapterFormat, D3DFORMAT backBufferFormat, bool windowed);
		[PreserveSig]
		int CheckDeviceFormat(uint adapter, D3DDEVTYPE deviceType, D3DFORMAT adapterFormat, uint usage, D3DRESOURCETYPE rType, D3DFORMAT CheckFormat);
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		[PreserveSig]
		int CheckDeviceMultiSampleType(uint adapter, D3DDEVTYPE deviceType, D3DFORMAT surfaceFormat, bool windowed, D3DMULTISAMPLE_TYPE MultiSampleType, out Int32 qualityLevels);
		[PreserveSig]
		int CheckDepthStencilMatch(uint adapter, D3DDEVTYPE deviceType, D3DFORMAT adapterFormat, D3DFORMAT renderTargetFormat, D3DFORMAT depthStencilFormat);
		[PreserveSig]
		int CheckDeviceFormatConversion(uint adapter, D3DDEVTYPE deviceType, D3DFORMAT sourceFormat, D3DFORMAT targetFormat);
		[PreserveSig]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		int GetDeviceCaps(uint adapter, D3DDEVTYPE deviceType, [MarshalAs(UnmanagedType.Struct)] ref D3DCAPS9 caps);
		[PreserveSig]
		IntPtr GetAdapterMonitor(uint adapter);
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		[PreserveSig]
		int CreateDevice(uint adapter, D3DDEVTYPE deviceType, IntPtr hFocusWindow, D3DCREATE BehaviorFlags,
			ref D3DPRESENT_PARAMETERS pPresentationParameters, out IDirect3DDevice9 ppReturnedDeviceInterface);
	}
}
