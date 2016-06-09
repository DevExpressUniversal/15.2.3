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
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics.CodeAnalysis;
namespace DevExpress.OpenGL {
	static class User32Unsafe {
		[DllImport("user32.dll")]
		internal static extern IntPtr GetDC(IntPtr hWnd);
	}
	public static class User32 {
		[SecuritySafeCritical]
		public static IntPtr GetDC(IntPtr hWnd) {
			return User32Unsafe.GetDC(hWnd);
		}
	}
	#region PIXELFORMATDESCRIPTOR
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential)]
	public class PIXELFORMATDESCRIPTOR {
		public short nSize { get; set; }
		public short nVersion { get; set; }
		public uint dwFlags { get; set; }
		public byte iPixelType { get; set; }
		public byte cColorBits { get; set; }
		public byte cRedBits { get; set; }
		public byte cRedShift { get; set; }
		public byte cGreenBits { get; set; }
		public byte cGreenShift { get; set; }
		public byte cBlueBits { get; set; }
		public byte cBlueShift { get; set; }
		public byte cAlphaBits { get; set; }
		public byte cAlphaShift { get; set; }
		public byte cAccumBits { get; set; }
		public byte cAccumRedBits { get; set; }
		public byte cAccumGreenBits { get; set; }
		public byte cAccumBlueBits { get; set; }
		public byte cAccumAlphaBits { get; set; }
		public byte cDepthBits { get; set; }
		public byte cStencilBits { get; set; }
		public byte cAuxBuffers { get; set; }
		public byte iLayerType { get; set; }
		public byte bReserved { get; set; }
		public int dwLayerMask { get; set; }
		public int dwVisibleMask { get; set; }
		public int dwDamageMask { get; set; }
		public PIXELFORMATDESCRIPTOR() {
			nSize = (short)DevExpress.XtraMap.Drawing.MarshalHelper.SizeOf(this);
			nVersion = 1;
			iPixelType = WGL.PFD_TYPE_RGBA;
			cColorBits = 24;
			cDepthBits = 32;
			iLayerType = WGL.PFD_MAIN_PLANE;
		}
	}
	#endregion
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity, SecurityCritical]
	static class WGLUnsafe {
		internal const string LibraryName = "opengl32.dll";
		internal const string GdiNativeLibraryName = "gdi32.dll";
	}
	[CLSCompliant(false)]
	public static class WGL {
		internal const uint PFD_DOUBLEBUFFER = 0x00000001;
		internal const uint PFD_DRAW_TO_WINDOW = 0x00000004;
		internal const uint PFD_SUPPORT_OPENGL = 0x00000020;
		internal const uint PFD_GENERIC_ACCELERATED = 0x00001000;
		internal const uint PFD_STEREO_DONTCARE = 0x80000000;
		internal const byte PFD_TYPE_RGBA = 0;
		internal const byte PFD_MAIN_PLANE = 0;
	}
}
