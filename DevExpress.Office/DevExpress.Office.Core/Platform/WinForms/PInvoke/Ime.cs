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
		[DllImport("imm32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr ImmGetContext(IntPtr hWnd);
		[DllImport("imm32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool ImmSetCandidateWindow(IntPtr hIMC, ref DevExpress.Office.PInvoke.Win32.CANDIDATEFORM lpCandForm);
		[DllImport("imm32.dll", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int ImmGetCompositionStringW(IntPtr hIMC, uint dwIndex, byte[] lpBuf, uint dwBufLen);
		[DllImport("imm32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
		[DllImport("imm32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool ImmSetOpenStatus(IntPtr hIMC, bool fOpen);
		[DllImport("imm32.dll")]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue);
	}
	public static partial class Win32 {
		[SecuritySafeCritical]
		public static IntPtr ImmGetContext(IntPtr hWnd) {
			return PInvokeSafeNativeMethods.ImmGetContext(hWnd);
		}
		[SecuritySafeCritical]
		public static bool ImmSetCandidateWindow(IntPtr hIMC, ref CANDIDATEFORM lpCandForm) {
			return PInvokeSafeNativeMethods.ImmSetCandidateWindow(hIMC, ref lpCandForm);
		}
		[SecuritySafeCritical]
		public static int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen) {
			return PInvokeSafeNativeMethods.ImmGetCompositionStringW(hIMC, (uint)dwIndex, lpBuf, (uint)dwBufLen);
		}
		[SecuritySafeCritical]
		public static int ImmReleaseContext(IntPtr hWnd, IntPtr hIMC) {
			return PInvokeSafeNativeMethods.ImmReleaseContext(hWnd, hIMC);
		}
		[SecuritySafeCritical]
		public static bool ImmSetOpenStatus(IntPtr hIMC, bool fOpen) {
			return PInvokeSafeNativeMethods.ImmSetOpenStatus(hIMC, fOpen);
		}
		[SecuritySafeCritical]
		public static bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue) {
			return PInvokeSafeNativeMethods.ImmNotifyIME(hIMC, dwAction, dwIndex, dwValue);
		}
		public class CfsFlags {
			public const int CFS_DEFAULT = 0x0000;
			public const int CFS_RECT = 0x0001;
			public const int CFS_POINT = 0x0002;
			public const int CFS_FORCE_POSITION = 0x0020;
			public const int CFS_CANDIDATEPOS = 0x0040;
			public const int CFS_EXCLUDE = 0x0080;
		}
		public class GcsFlags {
			public const int GCS_COMPREADSTR = 0x0001;
			public const int GCS_COMPREADATTR = 0x0002;
			public const int GCS_COMPREADCLAUSE = 0x0004;
			public const int GCS_COMPSTR = 0x0008;
			public const int GCS_COMPATTR = 0x0010;
			public const int GCS_COMPCLAUSE = 0x0020;
			public const int GCS_CURSORPOS = 0x0080;
			public const int GCS_DELTASTART = 0x0100;
			public const int GCS_RESULTREADSTR = 0x0200;
			public const int GCS_RESULTREADCLAUSE = 0x0400;
			public const int GCS_RESULTSTR = 0x0800;
			public const int GCS_RESULTCLAUSE = 0x1000;
			public const int CS_INSERTCHAR = 0x2000;
			public const int CS_NOMOVECARET = 0x4000;
		}
		public class CpsFlags {
			public const int CPS_COMPLETE = 0x0001;
			public const int CPS_CONVERT = 0x0002;
			public const int CPS_REVERT = 0x0003;
			public const int CPS_CANCEL = 0x0004;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct CANDIDATEFORM {
			public int dwIndex;
			public int dwStyle;
			public Win32.POINT ptCurrentPos;
			public Win32.RECT rcArea;
		};
		public const int WM_IME_STARTCOMPOSITION = 0x010D;
		public const int WM_IME_ENDCOMPOSITION = 0x010E;
		public const int WM_IME_COMPOSITION = 0x010F;
		public const int NI_OPENCANDIDATE = 0x0010;
		public const int NI_CLOSECANDIDATE = 0x0011;
		public const int NI_SELECTCANDIDATESTR = 0x0012;
		public const int NI_CHANGECANDIDATELIST = 0x0013;
		public const int NI_FINALIZECONVERSIONRESULT = 0x0014;
		public const int NI_COMPOSITIONSTR = 0x0015;
		public const int NI_SETCANDIDATE_PAGESTART = 0x0016;
		public const int NI_SETCANDIDATE_PAGESIZE = 0x0017;
	}
}
