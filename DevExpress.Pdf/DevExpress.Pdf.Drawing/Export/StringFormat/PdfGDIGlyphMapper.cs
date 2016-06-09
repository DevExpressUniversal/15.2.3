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
using System.Security;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Pdf.Interop;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfGDIGlyphMapper : PdfGlyphMapper {
		static bool HasRTLText(string line) {
			foreach (char c in line)
				if ((c >= 0x590 && c <= 0x5FF) || (c >= 0x600 && c <= 0x6FF))
					return true;
			return false;
		}
		static ushort[] PrepareGlyphs(string originalString, uint actualGlyphCount, ushort[] originalGlyphs, int[] order) {
			List<int> glyphsToRemove = new List<int>();
			for (int i = 0; i < order.Length; i++) {
				if (IsWritingOrderControl(originalString[i]))
					glyphsToRemove.Add(order[i]);
			}
			ushort[] result;
			if (glyphsToRemove.Count == 0) {
				result = new ushort[actualGlyphCount];
				Array.Copy(originalGlyphs, result, actualGlyphCount);
			}
			else {
				result = new ushort[actualGlyphCount - glyphsToRemove.Count];
				glyphsToRemove.Sort();
				int j = 0, k = 0;
				for (int i = 0; i < actualGlyphCount; i++) {
					if (j >= glyphsToRemove.Count || glyphsToRemove[j] != i)
						result[k++] = originalGlyphs[i];
					else
						j++;
				}
			}
			return result;
		}
		internal static IDictionary<int, string> PrepareCharMap(string orignialString, int[] order, ushort[] glyphs) {
			IDictionary<int, string> result = new Dictionary<int, string>();
			List<char> chars = new List<char>(3);
			chars.Add(orignialString[0]);
			int lastOrder = order[0];
			for (int i = 1; i < order.Length; i++) {
				if (lastOrder == order[i]) {
					chars.Add(orignialString[i]);
				}
				else {
					AssignMap(result, glyphs[lastOrder], chars);
					chars.Clear();
					chars.Add(orignialString[i]);
					lastOrder = order[i];
				}
			}
			AssignMap(result, glyphs[lastOrder], chars);
			return result;
		}
		static void AssignMap(IDictionary<int, string> result, ushort glyph, List<char> chars) {
			if (!IsWritingOrderControl(chars[0]) && !result.ContainsKey(glyph))
				result.Add(glyph, new string(chars.ToArray()));
		}
		static bool IsWritingOrderControl(char c) {
			return c >= '\u202A' && c <= '\u202E';
		}
		readonly PdfFontFile fontFile;
		readonly double scaleFactor;
		Graphics graphics;
		Font font;
		IntPtr hFont;
		GCHandle orderHandle;
		GCHandle dxHandle;
		GCHandle caretHandle;
		GCHandle classsHandle;
		GCHandle glyphsHandle;
		int[] order;
		int[] dx;
		int[] caret;
		byte[] clss;
		ushort[] glyphs;
		public PdfGDIGlyphMapper(string fontFamily, FontStyle fontStyle, PdfFontFile fontFile)
			: base(fontFile) {
			this.fontFile = fontFile;
			graphics = Graphics.FromHwnd(IntPtr.Zero);
			this.font = new Font(fontFamily, 72, fontStyle);
			hFont = this.font.ToHfont();
			scaleFactor = graphics.DpiX / 1000.0;
		}
		[SecuritySafeCritical]
		public override PdfGlyphMappingResult MapString(string s, bool useKerning) {
			if (HasRTLText(s)) {
				IntPtr hdc = graphics.GetHdc();
				IntPtr oldFont = Gdi32Interop.SelectObject(hdc, hFont);
				int stringLength = s.Length;
				CreateHandles(stringLength);
				try {
					GCP_RESULTS gcpResults = new GCP_RESULTS();
					gcpResults.StructSize = Marshal.SizeOf(typeof(GCP_RESULTS));
					gcpResults.OutString = new string('\0', stringLength);
					gcpResults.Order = orderHandle.AddrOfPinnedObject();
					gcpResults.Dx = dxHandle.AddrOfPinnedObject();
					gcpResults.CaretPos = caretHandle.AddrOfPinnedObject();
					gcpResults.Cls = classsHandle.AddrOfPinnedObject();
					gcpResults.Glyphs = glyphsHandle.AddrOfPinnedObject();
					gcpResults.GlyphCount = (uint)stringLength;
					gcpResults.MaxFit = 0;
					GCPFlags flags = GCPFlags.GCP_LIGATE | GCPFlags.GCP_REORDER;					
					if (useKerning)
						flags |= GCPFlags.GCP_USEKERNING;
					uint result = Gdi32Interop.GetCharacterPlacement(hdc, s, stringLength, 0, ref gcpResults, (uint)flags);
					if (result != 0) {
						int[] orderArray = (int[])orderHandle.Target;
						ushort[] glyphIndices = PrepareGlyphs(s, gcpResults.GlyphCount, (ushort[])glyphsHandle.Target, orderArray);
						int glyphCount = glyphIndices.Length;
						IList<PdfStringGlyph> resultGlyphs = new List<PdfStringGlyph>();
						for (int i = 0; i < glyphCount; i++) {
							int index = order[i];
							float width = fontFile.GetCharacterWidth(glyphs[index]);
							double glyphOffset = 0;
							if (useKerning)
								glyphOffset = width - dx[index] / scaleFactor;
							resultGlyphs.Add(new PdfStringGlyph(glyphIndices[i], width, glyphOffset));
						}
						return new PdfGlyphMappingResult(new PdfStringGlyphRun(resultGlyphs), PrepareCharMap(s, orderArray, glyphIndices));
					}
					else
						return base.MapString(s, useKerning);
				}
				finally {
					Gdi32Interop.SelectObject(hdc, oldFont);
					DestroyHandles();
					graphics.ReleaseHdc(hdc);
				}
			}
			else
				return base.MapString(s, useKerning);
		}
		[SecuritySafeCritical]
		void CreateHandles(int stringLength) {
			int[] result = new int[stringLength];
			order = new int[stringLength];
			dx = new int[stringLength];
			caret = new int[stringLength];
			clss = new byte[stringLength];
			glyphs = new ushort[stringLength];
			orderHandle = GCHandle.Alloc(order, GCHandleType.Pinned);
			dxHandle = GCHandle.Alloc(dx, GCHandleType.Pinned);
			caretHandle = GCHandle.Alloc(caret, GCHandleType.Pinned);
			classsHandle = GCHandle.Alloc(clss, GCHandleType.Pinned);
			glyphsHandle = GCHandle.Alloc(glyphs, GCHandleType.Pinned);
		}
		[SecuritySafeCritical]
		void DestroyHandles() {
			orderHandle.Free();
			dxHandle.Free();
			caretHandle.Free();
			classsHandle.Free();
			glyphsHandle.Free();
		}
		[SecuritySafeCritical]
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (hFont != IntPtr.Zero) {
				Gdi32Interop.DeleteObject(hFont);
				hFont = IntPtr.Zero;
			}
			if (disposing) {
				font.Dispose();
				graphics.Dispose();
			}
		}
	}
}
