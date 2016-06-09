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
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.Office.PInvoke {
	static partial class PInvokeSafeNativeMethods {
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct CHARSETINFO {
			public int ciCharset;
			public int ciACP;
			[MarshalAs(UnmanagedType.Struct)]
			public DevExpress.Office.PInvoke.Win32.FONTSIGNATURE fSig;
		}
		#region TranslateCharsetInfo
		public const int TCI_SRCCHARSET = 1;
		public const int TCI_SRCCODEPAGE = 2;
		public const int TCI_SRCFONTSIG = 3;
		public const int TCI_SRCLOCALE = 0x1000;
		[DllImport("Gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		public extern static int TranslateCharsetInfo([In, Out] IntPtr pSrc, [In, Out] ref CHARSETINFO lpSc, [In] int dwFlags);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int GetTextCharsetInfo(IntPtr hdc, [In, Out] ref DevExpress.Office.PInvoke.Win32.FONTSIGNATURE lpSig, int dwFlags);
		[DllImport("gdi32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int GetTextCharset(IntPtr hdc);
		#endregion
	}
	public static partial class Win32 {
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Size = (4 + 2) * 4)]
		public struct FONTSIGNATURE {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public Int32[] fsUsb;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public Int32[] fsCsb;
		}
		[SecuritySafeCritical]
		public static FontCharset GetFontCharsetInfo(IntPtr hdc, ref FONTSIGNATURE lpSig) {
			return (FontCharset)PInvokeSafeNativeMethods.GetTextCharsetInfo(hdc, ref lpSig, 0);
		}
		[SecuritySafeCritical]
		public static FontCharset GetFontCharset(IntPtr hdc) {
			return (FontCharset)PInvokeSafeNativeMethods.GetTextCharset(hdc);
		}
	}
}
