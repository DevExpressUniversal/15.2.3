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

using System.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	internal class PdfOpenTypeFontRegistrator : PdfInMemoryFontRegistrator {
		const string segoeUiFontName = "Segoe UI";
		static bool segoeUIPesent;
		static PdfOpenTypeFontRegistrator() {
			foreach (FontFamily family in FontFamily.Families)
				if (family.Name.Contains(segoeUiFontName)) {
					segoeUIPesent = true;
					break;
				}
		}
		public PdfOpenTypeFontRegistrator(PdfFont font, IType1Font type1Font) 
			: base(font, type1Font.OpenTypeFontFileData == null ? PdfOpenTypeFontCreator.Create(type1Font) : type1Font.OpenTypeFontFileData) {
		}
		protected internal override PdfFontRegistrationData CreateSubstituteFontData() {
			PdfFont font = Font;
			FontParameters fontParameters = GetSubstituteFontParameters(font, font.FontName);
			double widthFactor = 0;
			string name = fontParameters.Name;
			switch (name) {
				case PdfType1Font.CourierFontName:
				case PdfType1Font.CourierBoldFontName:
				case PdfType1Font.CourierObliqueFontName:
				case PdfType1Font.CourierBoldObliqueFontName:
				case PdfType1Font.CourierNewFontName:
					name = CourierNewFontName;
					break;
				case PdfType1Font.TimesRomanFontName:
				case PdfType1Font.TimesBoldFontName:
				case PdfType1Font.TimesItalicFontName:
				case PdfType1Font.TimesBoldItalicFontName:
					name = TimesNewRomanFontName;
					break;
				case PdfType1Font.HelveticaFontName:
				case PdfType1Font.HelveticaObliqueFontName:
				case PdfType1Font.HelveticaBoldFontName:
				case PdfType1Font.HelveticaBoldObliqueFontName:
					name = "Arial";
					break;
				case PdfType1Font.SymbolFontName:
				case PdfType1Font.ZapfDingbatsFontName:
					name = segoeUIPesent ? segoeUiFontName : "Arial Unicode MS";
					break;
				default:
					widthFactor = font.WidthToHeightFactor;
					break;
			}
			return new PdfFontRegistrationData(name, widthFactor, fontParameters.Weight, fontParameters.IsItalic, GetPitchAndFamily(font), false, null);
		}
	}
}
