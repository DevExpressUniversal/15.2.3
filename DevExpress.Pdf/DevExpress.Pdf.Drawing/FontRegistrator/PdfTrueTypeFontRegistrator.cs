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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	internal class PdfTrueTypeFontRegistrator : PdfInMemoryFontRegistrator {
		const string timesNewRomanPSMTPrefix = "TimesNewRomanPS";
		const string mtSuffix = "MT";
		public PdfTrueTypeFontRegistrator(PdfFont font, ITrueTypeFont trueTypeFont) 
			: base(font, trueTypeFont.CompactFontFileData == null ? trueTypeFont.FontFileData : PdfOpenTypeFontCreator.Create(trueTypeFont)) {
		}
		protected internal override PdfFontRegistrationData CreateSubstituteFontData() {
			PdfFont font = Font;
			string name = font.FontName;
			if (name.EndsWith(mtSuffix)) {
				if (name.StartsWith(timesNewRomanPSMTPrefix)) {
					name = name.Replace(timesNewRomanPSMTPrefix, TimesNewRomanFontName);
					name = name.Remove(name.Length - mtSuffix.Length);
				}
				else if (name.StartsWith(PdfSimpleFont.ArialFontName))
					name = name.Remove(name.Length - mtSuffix.Length);
			}
			FontParameters fontParameters = GetSubstituteFontParameters(font, name);
			name = fontParameters.Name;
			switch (name) {
				case PdfSimpleFont.CourierNewFontName:
					name = CourierNewFontName;
					break;
				case "TimesNewRoman":
					name = TimesNewRomanFontName;
					break;
			}
			return new PdfFontRegistrationData(name, 0, fontParameters.Weight, fontParameters.IsItalic, GetPitchAndFamily(font), false, null);
		}
	}
}
