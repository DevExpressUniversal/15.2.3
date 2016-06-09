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
using System.Security;
namespace DevExpress.Office.PInvoke {
	static partial class PInvokeSafeNativeMethods {
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr GetClipboardData(int uFormat);
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool OpenClipboard(IntPtr hWndNewOwner);
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool CloseClipboard();
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int RegisterClipboardFormat(string lpszFormat);
		[DllImport("user32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool IsClipboardFormatAvailable(int format);
		[System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetClipboardData",  SetLastError = true)]
		internal static extern System.IntPtr SetClipboardData(int uFormat, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hMem);
		[System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "EmptyClipboard", SetLastError = true)]
		[return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
		internal static extern bool EmptyClipboard();
	}
	public static partial class Win32 {
		[SecuritySafeCritical]
		public static IntPtr GetClipboardData(int format) {
			return PInvokeSafeNativeMethods.GetClipboardData(format);
		}
		[SecuritySafeCritical]
		public static bool OpenClipboard(IntPtr hWndNewOwner) {
			return PInvokeSafeNativeMethods.OpenClipboard(hWndNewOwner);
		}
		[SecuritySafeCritical]
		public static bool CloseClipboard() {
			return PInvokeSafeNativeMethods.CloseClipboard();
		}
		[SecuritySafeCritical]
		public static int RegisterClipboardFormat(string formatName) {
			return PInvokeSafeNativeMethods.RegisterClipboardFormat(formatName);
		}
		[SecuritySafeCritical]
		public static bool IsClipboardFormatAvailable(int format) {
			return PInvokeSafeNativeMethods.IsClipboardFormatAvailable(format);
		}
		[SecuritySafeCritical]
		public static IntPtr SetClipboardData(int uFormat, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hMem) {
			return PInvokeSafeNativeMethods.SetClipboardData(uFormat, hMem);
		}
		[SecuritySafeCritical]
		public static bool EmptyClipboard() {
			return PInvokeSafeNativeMethods.EmptyClipboard();
		}
	}
}
