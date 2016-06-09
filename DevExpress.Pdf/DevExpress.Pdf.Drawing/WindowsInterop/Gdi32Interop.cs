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
namespace DevExpress.Pdf.Interop {
	static class Gdi32Interop {
		[DllImport("Gdi32.dll", EntryPoint = "AddFontResourceExW")]
		public static extern int AddFontResourceEx([MarshalAs(UnmanagedType.LPWStr)] string fileName, FontCharacteristics characteristics, IntPtr reserved);
		[DllImport("Gdi32.dll", EntryPoint = "RemoveFontResourceExW")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RemoveFontResourceEx([MarshalAs(UnmanagedType.LPWStr)] string fileName, FontCharacteristics characteristics, IntPtr reserved);
		[DllImport("Gdi32.dll")]
		public static extern IntPtr AddFontMemResourceEx(byte[] fontData, int fontDataLength, IntPtr reserved, out int fontCount);
		[DllImport("Gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RemoveFontMemResourceEx(IntPtr fontHandle);
		[DllImport("gdi32.dll", EntryPoint = "CreateFontW")]
		public static extern IntPtr CreateFont(int height, int width, int escapement, int orientation, int weight, [MarshalAs(UnmanagedType.Bool)] bool italic,
			[MarshalAs(UnmanagedType.Bool)] bool underline, [MarshalAs(UnmanagedType.Bool)] bool strikeOut, FontCharSet charSet, FontOutputPrecision outputPrecision,
			FontClipPrecision clipPrecision, FontQuality quality, FontPitchAndFamily pitchAndFamily, [MarshalAs(UnmanagedType.LPWStr)]string name);
		[DllImport("gdi32.dll", EntryPoint = "GetTextMetricsW")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC metric);
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);
		[DllImport("Gdi32.dll")]
		public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);
		[DllImport("Gdi32.dll")]
		public static extern BackgroundMode SetBkMode(IntPtr hdc, BackgroundMode mode);
		[DllImport("Gdi32.dll")]
		public static extern GraphicsMode SetGraphicsMode(IntPtr hdc, GraphicsMode mode);
		[DllImport("Gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWorldTransform(IntPtr hdc, ref XFORM xform);
		[DllImport("Gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ModifyWorldTransform(IntPtr hdc, ref XFORM xForm, ModifyWorldTransformMode mode);
		[DllImport("Gdi32.dll")]
		public static extern int SetTextColor(IntPtr hdc, int color);
		[DllImport("gdi32.dll")]
		public static extern TextAlign SetTextAlign(IntPtr hdc, TextAlign textAlign);
		[DllImport("gdi32.dll", EntryPoint = "ExtTextOutW")]
		public static extern int ExtTextOut(IntPtr hdc, int x, int y, TextOutOptions options, ref RECT dimensions,
			short[] indices, int count, [In, MarshalAs(UnmanagedType.LPArray)] int[] spacing);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, EntryPoint = "GetCharacterPlacementW")]
		public static extern uint GetCharacterPlacement(IntPtr hdc, string lpString, int nCount, int nMaxExtent, [In, Out] ref GCP_RESULTS lpResults, uint dwFlags);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint GetEnhMetaFileBits(IntPtr hemf, uint cbBuffer, [Out] byte[] lpbBuffer);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint GetMetaFileBitsEx(IntPtr hemf, uint cbBuffer, [Out] byte[] lpbBuffer);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint DeleteEnhMetaFile(IntPtr hemf);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint DeleteMetaFile(IntPtr hmf);
		[DllImport("gdi32.dll")]
		public static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, byte[] lpvBuffer, uint cbData);
	}
}
