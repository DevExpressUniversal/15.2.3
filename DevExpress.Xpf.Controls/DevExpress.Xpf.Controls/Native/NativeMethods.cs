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
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
namespace DevExpress.Xpf.Controls.Native {
	internal static class Native {
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern bool DrawMenuBar(IntPtr hWnd);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern int GetMenuItemCount(IntPtr hMenu);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern Int32 EnableMenuItem(System.IntPtr hMenu, Int32 uIDEnableItem, Int32 uEnable);
		internal const Int32 MF_BYPOSITION = 0x00000400;
		internal const Int32 MF_GRAYED = 0x00000001;
		internal const Int32 MF_DISABLED = 0x00000002;
		internal const Int32 SC_MOVE = 0xF010;
		internal const Int32 WM_SYSCOMMAND = 0x112;
		internal const Int32 WM_INITMENUPOPUP = 0x117;
		[SecurityCritical]
		[DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetCapture();
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetFocus();
		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO {
			public DevExpress.Xpf.Core.NativeMethods.POINT ptReserved;
			public DevExpress.Xpf.Core.NativeMethods.POINT ptMaxSize;
			public DevExpress.Xpf.Core.NativeMethods.POINT ptMaxPosition;
			public DevExpress.Xpf.Core.NativeMethods.POINT ptMinTrackSize;
			public DevExpress.Xpf.Core.NativeMethods.POINT ptMaxTrackSize;
		};
		[StructLayout(LayoutKind.Sequential)]
		public class MONITORINFO {
			public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
			public DevExpress.Xpf.Core.NativeMethods.RECT rcMonitor = new DevExpress.Xpf.Core.NativeMethods.RECT();
			public DevExpress.Xpf.Core.NativeMethods.RECT rcWork = new DevExpress.Xpf.Core.NativeMethods.RECT();
			public int dwFlags = 0;
		}
		[DllImport("user32")]
		internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
		[DllImport("User32")]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
		[SecuritySafeCritical]
		internal static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam) {
			MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
			int MONITOR_DEFAULTTONEAREST = 0x00000002;
			System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
			if(monitor != System.IntPtr.Zero) {
				MONITORINFO monitorInfo = new MONITORINFO();
				GetMonitorInfo(monitor, monitorInfo);
				DevExpress.Xpf.Core.NativeMethods.RECT rcWorkArea = monitorInfo.rcWork;
				DevExpress.Xpf.Core.NativeMethods.RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
				mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
				mmi.ptMinTrackSize.x = 0;
				mmi.ptMinTrackSize.y = 0;
				mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
				mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
			}
			Marshal.StructureToPtr(mmi, lParam, true);
		}
	}
}
