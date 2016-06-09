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
using System.Text;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfNotEmbeddedEditableFontData : PdfEditableFontData {
		const int firstChar = 32;
		const int lastChar = 255;
		const string winAnsiEncodingName = "WinAnsiEncoding";
		static bool FamilyNamesAreDifferent(PdfFontNameTableDirectoryEntry name) {
			return !string.IsNullOrEmpty(name.MacFamilyName) && name.MacFamilyName != name.FamilyName && !string.IsNullOrEmpty(name.PostScriptName);
		}
		public override bool Embedded { get { return false; } }
		protected override string BaseFont { get { return FontFile != null && FamilyNamesAreDifferent(FontFile.Name) ? FontFile.Name.PostScriptName : GetTrueTypeFontName(false); } }
		public PdfNotEmbeddedEditableFontData(FontStyle fontStyle, string fontFamily) : base(fontStyle, fontFamily) { 
		}
		public override PdfStringFormatter CreateStringFormatter(double fontSize) {
			return new PdfNotEmbeddedStringFormatter(this, fontSize);
		}
		protected override IPdfGlyphMapper CreateMapper() {
			return new PdfNotEmbeddedGlyphMapper(FontFile);
		}
		protected override PdfFont CreateFont(PdfReaderDictionary fontDescriptor) {
			double[] widths = new double[lastChar - firstChar + 1];
			for (int i = firstChar, pos = 0; i <= lastChar; i++, pos++) {
				byte[] ansiBytes = BitConverter.GetBytes(i);
				byte ansi = BitConverter.IsLittleEndian ? ansiBytes[0] : ansiBytes[ansiBytes.Length - 1];
				byte[] unicodeBytes = Encoding.Convert(DXEncoding.GetEncoding(1252), Encoding.Unicode, new byte[] { ansi });
				char ch = Encoding.Unicode.GetChars(unicodeBytes)[0];
				widths[pos] = GetCharWidth(ch);
			}
			return new PdfTrueTypeFont(BaseFont, fontDescriptor, widths);
		}
	}
}
