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
namespace DevExpress.Pdf.Common {
	[System.Security.SuppressUnmanagedCodeSecurity]
	static class GDIFunctions {
		[DllImport("GDI32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
		[DllImport("GDI32.dll")]
		public static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, byte[] lpvBuffer, uint cbData);
		[DllImport("GDI32.dll")]
		public static extern bool DeleteObject(IntPtr hgdiobj);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern bool TranslateCharsetInfo(uint pSrc, [System.Runtime.InteropServices.Out, System.Runtime.InteropServices.In]ref CHARSETINFO lpCs, uint dwFlags);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern int GetTextCharset(IntPtr hdc);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetCharacterPlacement(IntPtr hdc, string lpString, int nCount, int nMaxExtent, [In, Out] ref GCP_RESULTS lpResults, uint dwFlags);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint GetFontLanguageInfo(IntPtr hdc);
		[DllImport("gdi32.dll")]
		public static extern bool SetLayout(IntPtr hdc, uint flags);
	}
}
