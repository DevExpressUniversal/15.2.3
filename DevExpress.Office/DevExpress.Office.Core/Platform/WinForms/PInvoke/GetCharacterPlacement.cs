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
		[DllImport("gdi32.dll", EntryPoint = "GetCharacterPlacementW", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		internal static extern int GetCharacterPlacement(IntPtr hdc, string lpString, int nCount, int nMaxExtent, ref Win32.GCP_RESULTS lpResults, int dwFlags);
	}
	public static partial class Win32 {
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), ComVisible(false)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
		public struct GCP_RESULTS {
			public int lStructSize;
			public IntPtr lpOutString;
			public IntPtr lpOrder;
			public IntPtr lpDx;
			public IntPtr lpCaretPos;
			public IntPtr lpClass;
			public IntPtr lpGlyphs;
			public int nGlyphs;
			public int nMaxFit;
		};
		[Flags]
		public enum GcpFlags {
			GCP_DBCS = 0x0001,
			GCP_REORDER = 0x0002,
			GCP_USEKERNING = 0x0008,
			GCP_GLYPHSHAPE = 0x0010,
			GCP_LIGATE = 0x0020,
			GCP_DIACRITIC = 0x0100,
			GCP_KASHIDA = 0x0400,
			GCP_ERROR = 0x8000,
			GCP_JUSTIFY = 0x00010000,
			GCP_CLASSIN = 0x00080000,
			GCP_MAXEXTENT = 0x00100000,
			GCP_JUSTIFYIN = 0x00200000,
			GCP_DISPLAYZWG = 0x00400000,
			GCP_SYMSWAPOFF = 0x00800000,
			GCP_NUMERICOVERRIDE = 0x01000000,
			GCP_NEUTRALOVERRIDE = 0x02000000,
			GCP_NUMERICSLATIN = 0x04000000,
			GCP_NUMERICSLOCAL = 0x08000000,
		}
		[SecuritySafeCritical]
		public static int GetCharacterPlacement(IntPtr hdc, string lpString, int nCount, int nMaxExtent, ref GCP_RESULTS lpResults, GcpFlags dwFlags) {
			return PInvokeSafeNativeMethods.GetCharacterPlacement(hdc, lpString, nCount, nMaxExtent, ref lpResults, (int)dwFlags);
		}
	}
}
