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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Interop;
namespace DevExpress.Pdf.Drawing {
	public class PdfFontSearch : PdfDisposableObject {
		const string defaultTimesNewRomanFontName = "Times New Roman";
		const string alternateTimesNewRomanFontName = "TimesNewRoman";
		const string courierFontName = "Courier";
		const string arialFontName = "Arial";
		readonly List<PdfEmbeddedEditableFontData> editableFonts = new List<PdfEmbeddedEditableFontData>();
		PdfEmbeddedEditableFontData Search(string fontFamilyName, FontStyle fontStyle) {
			foreach (PdfEmbeddedEditableFontData font in editableFonts)
				if (font.FontFamily == fontFamilyName && font.FontStyle == fontStyle)
					return font;
			foreach (FontFamily fontFamily in FontFamily.Families)
				if (fontFamily.Name == fontFamilyName) {
					PdfEmbeddedEditableFontData editableFont = new PdfEmbeddedEditableFontData(fontStyle, fontFamilyName);
					editableFonts.Add(editableFont);
					return editableFont;
				}
			return null;
		}
		public PdfEmbeddedEditableFontData Search(PdfFont font) {
			if (font == null)
				return Search(defaultTimesNewRomanFontName, FontStyle.Regular);
			PdfFontRegistrationData fontData = PdfFontRegistrator.Create(font, String.Empty).CreateSubstituteFontData();
			FontStyle style = FontStyle.Regular;
			if (fontData.Weight > PdfFontRegistrator.NormalWeight)
				style |= FontStyle.Bold;
			if (fontData.Italic)
				style |= FontStyle.Italic;
			PdfEmbeddedEditableFontData result = Search(fontData.Name, style);
			if (result == null)
				if (fontData.Name.Contains(defaultTimesNewRomanFontName) || fontData.Name.Contains(alternateTimesNewRomanFontName))
					result = Search(defaultTimesNewRomanFontName, style);
				else if (fontData.Name.Contains(courierFontName))
					result = Search("Courier New", style);
				else if (fontData.Name.Contains(arialFontName))
					result = Search(arialFontName, style);
				else if ((fontData.PitchAndFamily & FontPitchAndFamily.FF_ROMAN) > 0)
					result = Search(defaultTimesNewRomanFontName, style);
				else if ((fontData.PitchAndFamily & FontPitchAndFamily.FIXED_PITCH) > 0)
					result = Search("Courier New", style);
				else
					result = Search("Arial", style);
			return result;
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				foreach (PdfEditableFontData editableFontData in editableFonts)
					editableFontData.Dispose();
		}
	}
}
