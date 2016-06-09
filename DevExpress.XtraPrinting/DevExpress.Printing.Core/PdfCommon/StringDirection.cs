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
using System.Drawing;
using System.Runtime.InteropServices;
namespace DevExpress.Pdf.Common {
	public class StringDirection : IDisposable {
		const int GCP_REORDER = 0x0002;
		const uint GCP_LIGATE = 0x0020;
		const uint LAYOUT_RTL = 0x00000001;
		GCHandle orderHandle;
		GCHandle dxHandle;
		GCHandle caretPosHandle;
		GCHandle classHandle;
		GCHandle glyphsHandle;
		Graphics gr;
		IntPtr hdc;
		Font font;
		IntPtr fontH;
		IntPtr oldFont;
		public StringDirection(Font font) {
			Initialize(font, false);
		}
		public StringDirection(Font font, bool rtl) {
			Initialize(font, rtl);
		}
		[System.Security.SecuritySafeCritical]
		void InitializeHandles(int strLength) {
			this.orderHandle = GCHandle.Alloc(new int[strLength], GCHandleType.Pinned);
			this.dxHandle = GCHandle.Alloc(new int[strLength], GCHandleType.Pinned);
			this.caretPosHandle = GCHandle.Alloc(new int[strLength], GCHandleType.Pinned);
			this.classHandle = GCHandle.Alloc(new byte[strLength], GCHandleType.Pinned);
			this.glyphsHandle = GCHandle.Alloc(new ushort[strLength], GCHandleType.Pinned);
		}
		[System.Security.SecuritySafeCritical]
		void FreeHandles() {
			this.orderHandle.Free();
			this.dxHandle.Free();
			this.caretPosHandle.Free();
			this.classHandle.Free();
			this.glyphsHandle.Free();
		}
		[System.Security.SecuritySafeCritical]
		GCP_RESULTS PrepareGCP_RESULTS(int strLength) {
			GCP_RESULTS gcp_Result = new GCP_RESULTS();
			gcp_Result.lStructSize = (uint)Marshal.SizeOf(typeof(GCP_RESULTS));
			gcp_Result.lpOutString = new String('\0', strLength);
			gcp_Result.lpOrder = this.orderHandle.AddrOfPinnedObject();
			gcp_Result.lpDx = this.dxHandle.AddrOfPinnedObject();
			gcp_Result.lpCaretPos = this.caretPosHandle.AddrOfPinnedObject();
			gcp_Result.lpClass = this.classHandle.AddrOfPinnedObject();
			gcp_Result.lpGlyphs = this.glyphsHandle.AddrOfPinnedObject();
			gcp_Result.nGlyphs = (uint)strLength;
			gcp_Result.nMaxFit = 0;
			return gcp_Result;
		}
		public TextRun ProcessString(string str) {
			return ProcessString(str, hdc);
		}
		[System.Security.SecuritySafeCritical]
		TextRun ProcessString(string str, IntPtr hdc) {
			int strLength = str.Length;
			InitializeHandles(strLength);
			try {
				GCP_RESULTS gcp_Result = PrepareGCP_RESULTS(strLength);
				uint result = GDIFunctions.GetCharacterPlacement(hdc, str, strLength, 0, ref gcp_Result, GCP_REORDER | GCP_LIGATE);
				TextRun si = new TextRun();
				if(result != 0) {
					si.Text = gcp_Result.lpOutString;
					si.Glyphs = PrepareGlyphs(str, gcp_Result.nGlyphs, (ushort[])this.glyphsHandle.Target, (int[])orderHandle.Target);
					si.CharMap = PrepareCharMap(str, (int[])orderHandle.Target, (ushort[])glyphsHandle.Target);
				} else {
					si.Text = str;
				}
				return si;
			} finally {
				FreeHandles();
			}
		}
		static ushort[] PrepareGlyphs(string originalString, uint actualGlyphCount, ushort[] originalGlyphs, int[] order) {
			List<int> glyphsToRemove = new List<int>();
			for(int i = 0; i < order.Length; i++) {
				if(IsWritingOrderControl(originalString[i]))
					glyphsToRemove.Add(order[i]);
			}
			ushort[] result;
			if(glyphsToRemove.Count == 0) {
				result = new ushort[actualGlyphCount];
				Array.Copy(originalGlyphs, result, actualGlyphCount);
			} else {
				result = new ushort[actualGlyphCount - glyphsToRemove.Count];
				glyphsToRemove.Sort();
				int j = 0, k = 0;
				for(int i = 0; i < actualGlyphCount; i++) {
					if(j >= glyphsToRemove.Count || glyphsToRemove[j] != i)
						result[k++] = originalGlyphs[i];
					else
						j++;
				}
			}
			return result;
		}
		static Dictionary<ushort, char[]> PrepareCharMap(string orignialString, int[] order, ushort[] glyphs) {
			Dictionary<ushort, char[]> result = new Dictionary<ushort, char[]>();
			List<char> chars = new List<char>(3);
			chars.Add(orignialString[0]);
			int lastOrder = order[0];
			for(int i = 1; i < order.Length; i++) {
				if(lastOrder == order[i]) {
					chars.Add(orignialString[i]);
				} else {
					AssignMap(result, glyphs[lastOrder], chars);
					chars.Clear();
					chars.Add(orignialString[i]);
					lastOrder = order[i];
				}
			}
			AssignMap(result, glyphs[lastOrder], chars);
			return result;
		}
		static void AssignMap(Dictionary<ushort, char[]> result, ushort glyph, List<char> chars) {
			if(!IsWritingOrderControl(chars[0]))
				result[glyph] = chars.ToArray();
		}
		static bool IsWritingOrderControl(char c) {
			return c >= '\u202A' && c <= '\u202E';
		}
#if DEBUGTEST
		internal static ushort[] Test_PrepareGlyphs(string originalString, uint actualGlyphCount, ushort[] originalGlyphs, int[] order) {
			return PrepareGlyphs(originalString, actualGlyphCount, originalGlyphs, order);
		}
		internal static Dictionary<ushort, char[]> Test_PrepareCharMap(string originalString, int[] order, ushort[] glyphs) {
			return PrepareCharMap(originalString, order, glyphs);
		}
#endif
		[System.Security.SecuritySafeCritical]
		void Initialize(Font originalFont, bool rtl) {
			gr = Graphics.FromHwnd(IntPtr.Zero);
			hdc = gr.GetHdc();
			try {
				font = (Font)originalFont.Clone();
				fontH = font.ToHfont();
				oldFont = GDIFunctions.SelectObject(hdc, fontH);
				if(rtl)
					GDIFunctions.SetLayout(hdc, LAYOUT_RTL);
			} catch {
				gr.ReleaseHdc(hdc);
				gr.Dispose();
				throw;
			}
		}
		[System.Security.SecuritySafeCritical]
		public void Dispose() {
			GDIFunctions.SelectObject(hdc, oldFont);
			GDIFunctions.DeleteObject(fontH);
			font.Dispose();
			gr.ReleaseHdc(hdc);
			gr.Dispose();
		}
	}
}
