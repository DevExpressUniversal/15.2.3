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
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool SetCaretPos(int X, int Y);
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool ShowCaret(IntPtr hWnd);
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool HideCaret(IntPtr hWnd);
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool DestroyCaret();
		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int GetCaretBlinkTime();
	}
	public static partial class Win32 {
		[SecuritySafeCritical]
		public static bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int width, int height) {
			return PInvokeSafeNativeMethods.CreateCaret(hWnd, hBitmap, width, height);
		}
		[SecuritySafeCritical]
		public static bool SetCaretPos(int x, int y) {
			return PInvokeSafeNativeMethods.SetCaretPos(x, y);
		}
		[SecuritySafeCritical]
		public static bool ShowCaret(IntPtr hWnd) {
			return PInvokeSafeNativeMethods.ShowCaret(hWnd);
		}
		[SecuritySafeCritical]
		public static bool HideCaret(IntPtr hWnd) {
			return PInvokeSafeNativeMethods.HideCaret(hWnd);
		}
		[SecuritySafeCritical]
		public static bool DestroyCaret() {
			return PInvokeSafeNativeMethods.DestroyCaret();
		}
		[SecuritySafeCritical]
		public static int GetCaretBlinkTime() {
			return PInvokeSafeNativeMethods.GetCaretBlinkTime();
		}
	}
}
