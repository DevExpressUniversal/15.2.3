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
		[DllImport("user32.dll", EntryPoint = "DrawTextExW", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int DrawTextEx(IntPtr hdc, string lpchText, int cchText,
		   ref Win32.RECT lprc, int dwDTFormat, IntPtr lpDTParams);
	}
	public static partial class Win32 {
		[Flags]
		public enum DrawTextFlags {
			DT_TOP = 0x00000000,
			DT_LEFT = 0x00000000,
			DT_CENTER = 0x00000001,
			DT_RIGHT = 0x00000002,
			DT_VCENTER = 0x00000004,
			DT_BOTTOM = 0x00000008,
			DT_WORDBREAK = 0x00000010,
			DT_SINGLELINE = 0x00000020,
			DT_EXPANDTABS = 0x00000040,
			DT_TABSTOP = 0x00000080,
			DT_NOCLIP = 0x00000100,
			DT_EXTERNALLEADING = 0x00000200,
			DT_CALCRECT = 0x00000400,
			DT_NOPREFIX = 0x00000800,
			DT_INTERNAL = 0x00001000,
			DT_EDITCONTROL = 0x00002000,
			DT_PATH_ELLIPSIS = 0x00004000,
			DT_END_ELLIPSIS = 0x00008000,
			DT_MODIFYSTRING = 0x00010000,
			DT_RTLREADING = 0x00020000,
			DT_WORD_ELLIPSIS = 0x00040000,
			DT_NOFULLWIDTHCHARBREAK = 0x00080000,
			DT_HIDEPREFIX = 0x00100000,
			DT_PREFIXONLY = 0x00200000,
		}
		[SecuritySafeCritical]
		public static int DrawTextEx(IntPtr hdc, string text, ref Win32.RECT bounds, DrawTextFlags flags) {
			return PInvokeSafeNativeMethods.DrawTextEx(hdc, text, text.Length, ref bounds, (int)flags, IntPtr.Zero);
		}
	}
}
