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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Interop;
namespace DevExpress.Pdf.Drawing {
	internal abstract class PdfFontRegistrator : PdfDisposableObject {
		internal const int NormalWeight = 400;
		protected const int BoldWeight = 700;
		protected const string TimesNewRomanFontName = "Times New Roman";
		protected const string CourierNewFontName = "Courier New";
		public static int GetWeight(PdfFont font) {
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor == null)
				return NormalWeight;
			return ((fontDescriptor.Flags & PdfFontFlags.ForceBold) == PdfFontFlags.ForceBold) ? BoldWeight: fontDescriptor.FontWeight;
		}
		public static bool IsItalic(PdfFont font) {
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			return fontDescriptor != null && fontDescriptor.ItalicAngle != 0;
		}
		public static FontPitchAndFamily GetPitchAndFamily(PdfFont font) {
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor == null)
				return FontPitchAndFamily.VARIABLE_PITCH;
			PdfFontFlags flags = fontDescriptor.Flags;
			FontPitchAndFamily pitchAndFamily = ((flags & PdfFontFlags.FixedPitch) == PdfFontFlags.FixedPitch) ? FontPitchAndFamily.FIXED_PITCH : FontPitchAndFamily.VARIABLE_PITCH;
			if ((flags & PdfFontFlags.Serif) > 0)
				pitchAndFamily |= FontPitchAndFamily.FF_ROMAN;
			if ((flags & PdfFontFlags.Script) > 0)
				pitchAndFamily |= FontPitchAndFamily.FF_SCRIPT;
			return pitchAndFamily;
		}
		public static PdfFontRegistrator Create(PdfFont font, string fontFolderName) {
			ITrueTypeFont trueTypeFont = font as ITrueTypeFont;
			if (trueTypeFont != null)
				return new PdfTrueTypeFontRegistrator(font, trueTypeFont);
			IType1Font type1Font = font as IType1Font;
			if (type1Font == null)
				return null;
			return type1Font.FontFileData == null ? (PdfFontRegistrator)new PdfOpenTypeFontRegistrator(font, type1Font) : (PdfFontRegistrator)new PdfType1FontRegistrator(font, type1Font, fontFolderName);
		}
		readonly PdfFont font;
		protected PdfFont Font { get { return font; } }
		protected PdfFontRegistrator(PdfFont font) {
			this.font = font;
		}
		public PdfFontRegistrationData Register() {
			string fontName = PerformRegister();
			return String.IsNullOrEmpty(fontName) ?  CreateSubstituteFontData() : 
				new PdfFontRegistrationData(fontName, 0, GetWeight(font), false, FontPitchAndFamily.DEFAULT_PITCH | FontPitchAndFamily.FF_DONTCARE, true, this);
		}
		protected internal virtual PdfFontRegistrationData CreateSubstituteFontData() {
			return new PdfFontRegistrationData(font.FontName, 0, GetWeight(font), IsItalic(font), GetPitchAndFamily(font), false, null);
		}
		protected override void Dispose(bool disposing) {
			Unregister();
		}
		protected abstract string PerformRegister();
		public abstract void Unregister();
	}
}
