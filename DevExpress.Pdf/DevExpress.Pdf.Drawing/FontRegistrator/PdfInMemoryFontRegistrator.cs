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
using System.Security;
using DevExpress.Pdf.Interop;
namespace DevExpress.Pdf.Drawing {
	internal abstract class PdfInMemoryFontRegistrator : PdfFontRegistrator {
		protected struct FontParameters {
			readonly string name;
			readonly int weight;
			readonly bool isItalic;
			public string Name { get { return name; } }
			public int Weight { get { return weight; } }
			public bool IsItalic { get { return isItalic; } }
			public FontParameters(string name, int weight, bool isItalic) {
				this.name = name;
				this.weight = weight;
				this.isItalic = isItalic;
			}
		}
		const string bold = "Bold";
		const string italic = "Italic";
		readonly byte[] fontFileData;
		IntPtr fontHandle;
		protected PdfInMemoryFontRegistrator(PdfFont font, byte[] fontFileData) : base(font) {
			this.fontFileData = fontFileData;
		}
		protected FontParameters GetSubstituteFontParameters(PdfFont font, string fontName) {
			bool isItalic = IsItalic(font);
			bool forceBold;
			int fontWeight;
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor == null) {
				forceBold = false;
				fontWeight = NormalWeight;
			}
			else {
				forceBold = (fontDescriptor.Flags & PdfFontFlags.ForceBold) == PdfFontFlags.ForceBold;
				fontWeight = fontDescriptor.FontWeight;
			}
			int delimiterPosition = fontName.IndexOf(',');
			if (delimiterPosition < 0)
				delimiterPosition = fontName.IndexOf('-');
			if (delimiterPosition > 0) {
				bool found = true;
				string kind = fontName.Substring(delimiterPosition + 1);
				switch (kind) {
					case bold:
						isItalic = false;
						forceBold = true;
						break;
					case italic:
						isItalic = true;
						break;
					case "BoldItalic":
						isItalic = true;
						forceBold = true;
						break;
					default:
						isItalic = false;
						found = false;
						break;
				}
				if (found)
					fontName = fontName.Substring(0, delimiterPosition);
			}
			else {
				forceBold = fontName.Contains(bold);
				isItalic = fontName.Contains(italic);
			}
			return new FontParameters(fontName, forceBold ? BoldWeight : fontWeight, isItalic);
		}
		[SecuritySafeCritical]
		protected override string PerformRegister() {
			if (fontFileData == null)
				return null;
			int fontCount;
			fontHandle = Gdi32Interop.AddFontMemResourceEx(fontFileData, fontFileData.Length, IntPtr.Zero, out fontCount);
			return fontHandle == IntPtr.Zero ? null : Font.BaseFont;
		}
		[SecuritySafeCritical]
		public override void Unregister() {
			if (fontHandle != IntPtr.Zero)
				Gdi32Interop.RemoveFontMemResourceEx(fontHandle);
		}
	}
}
