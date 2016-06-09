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
		[DllImport("gdi32.dll", EntryPoint = "ExtTextOutW", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int ExtTextOut(IntPtr hdc, int x, int y, int options, ref Win32.RECT clip, string str, int len, [In, MarshalAs(UnmanagedType.LPArray)] int[] widths);
		[DllImport("gdi32.dll", EntryPoint = "ExtTextOutW", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int ExtTextOut(IntPtr hdc, int x, int y, int options, ref Win32.RECT clip, IntPtr str, int len, IntPtr widths);
	}
	public static partial class Win32 {
		[Flags]
		public enum EtoFlags {
			ETO_NONE = 0x0000,
			ETO_OPAQUE = 0x0002,
			ETO_CLIPPED = 0x0004,
			ETO_GLYPH_INDEX = 0x0010,
			ETO_RTLREADING = 0x0080,
			ETO_NUMERICSLOCAL = 0x0400,
			ETO_NUMERICSLATIN = 0x0800,
			ETO_IGNORELANGUAGE = 0x1000,
			ETO_PDY = 0x2000,
		}
		[SecuritySafeCritical]
		public static int ExtTextOut(IntPtr hdc, int x, int y, EtoFlags options, ref Win32.RECT clip, string str, int len, [In, MarshalAs(UnmanagedType.LPArray)] int[] widths) {
			return PInvokeSafeNativeMethods.ExtTextOut(hdc, x, y, (int)options, ref clip, str, len, widths);
		}
		[SecuritySafeCritical]
		public static int ExtTextOut(IntPtr hdc, int x, int y, EtoFlags options, ref Win32.RECT clip, IntPtr str, int len, IntPtr widths) {
			return PInvokeSafeNativeMethods.ExtTextOut(hdc, x, y, (int)options, ref clip, str, len, widths);
		}
	}
}
