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

using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
namespace DevExpress.Pdf.Common {
	public class PdfFontUtils {
		const string errorStr = "Error when reading font file";
		static uint Ttcf {
			get {
				string s = "ttcf";
				return (uint)(s[0] | s[1] << 8 | s[2] << 16 | s[3] << 24);
			}
		}
		public static string Subname = "DEVEXP";
		static string GetFontPostfix(bool bold, bool italic) {
			string postfix = String.Empty;
			if(bold)
				postfix += "Bold";
			if(italic)
				postfix += "Italic";
			return postfix;
		}
		static string AddPostfix(string fontName, bool bold, bool italic) {
			string result = fontName;
			string postfix = GetFontPostfix(bold, italic);
			if(postfix != String.Empty)
				result += "," + postfix;
			return result;
		}
		public static string GetTrueTypeFontName0(string fontName, bool useSubname) {
			string result = PdfStringUtils.ClearSpaces(fontName);
			return (useSubname) ? Subname + "+" + result : result;
		}
		public static string GetTrueTypeFontName(Font font, bool useSubname) {
			return AddPostfix(GetTrueTypeFontName0(font.Name, useSubname), font.Bold, font.Italic);
		}
		public static string GetFontName(Font font) {
			return GetFontName(font.Name, font.Bold, font.Italic);
		}
		public static string GetFontName(string familyName, bool bold, bool italic) {
			return AddPostfix(PdfStringUtils.ClearSpaces(familyName), bold, italic);
		}
		public static bool FontEquals(Font font1, Font font2) {
			if(font1 == null || font2 == null)
				return false;
			return GetTrueTypeFontName(font1, false) == GetTrueTypeFontName(font2, false);
		}
		internal static byte[] CreateTTFFile(Font font) {
			bool isTTC;
			return CreateTTFFile(font, out isTTC);
		}
		[System.Security.SecuritySafeCritical]
		internal static byte[] CreateTTFFile(Font font, out bool isTTC) {
			using(Graphics gr = GraphicsHelper.CreateGraphics()) {
				IntPtr hdc = gr.GetHdc();
				IntPtr oldFont = IntPtr.Zero;
				IntPtr fontH = font.ToHfont();
				try {
					oldFont = GDIFunctions.SelectObject(hdc, fontH);
					uint length = GDIFunctions.GetFontData(hdc, Ttcf, 0, null, 0);
					uint dwTable = length != uint.MaxValue ? Ttcf : 0;
					if(length == uint.MaxValue)
						length = GDIFunctions.GetFontData(hdc, 0, 0, null, 0);
					if(length == uint.MaxValue)
						throw new PdfException(errorStr);
					byte[] data = new byte[length];
					length = GDIFunctions.GetFontData(hdc, dwTable, 0, data, length);
					if(length != data.Length)
						throw new PdfException(errorStr);
					isTTC = dwTable == Ttcf;
					return data;
				} finally {
					GDIFunctions.SelectObject(hdc, oldFont);
					GDIFunctions.DeleteObject(fontH);
					gr.ReleaseHdc(hdc);
				}
			}
		}
	}
}
