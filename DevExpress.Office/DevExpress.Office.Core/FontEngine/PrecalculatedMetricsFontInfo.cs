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
using DevExpress.Utils.Internal;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Drawing {
	#region PrecalculatedMetricsFontInfo
	public class PrecalculatedMetricsFontInfo : FontInfo {
		Font font;
		FontDescriptor fontDescriptor;
		public PrecalculatedMetricsFontInfo(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline)
			: base(measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline) {
		}
		#region Properties
		public override bool Bold { get { return font.Bold; } }
		public override bool Italic { get { return font.Italic; } }
		public override bool Underline { get { return font.Underline; } }
		public override bool Strikeout { get { return font.Strikeout; } }
		public override float Size { get { return font.Size; } }
		public override string Name { get { return font.Name; } }
		public override string FontFamilyName { get { return font.FontFamily.Name; } }
		public override Font Font { get { return font; } }
		public FontDescriptor FontDescriptor { get { return fontDescriptor; } }
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (font != null) {
						font.Dispose();
						font = null;
					}
					fontDescriptor = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override void CreateFont(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			PrecalculatedMetricsFontInfoMeasurer precalculatedMetricsMeasurer = (PrecalculatedMetricsFontInfoMeasurer)measurer;
			this.font = precalculatedMetricsMeasurer.CreateFont(fontName, doubleFontSize / 2f, fontBold, fontItalic, fontStrikeout, fontUnderline);
#if SL
			this.fontDescriptor = font.FontDescriptor;
#else
			this.fontDescriptor = FontManager.GetFontDescriptor(fontName, fontBold, fontItalic);
#endif
		}
		protected internal override float CalculateFontSizeInPoints() {			
			return DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(font);
		}
		#region Platform specific methods
		protected internal override void CalculateFontVerticalParameters(FontInfoMeasurer measurer) {
			float sizeInPoints = DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(Font);
			float sizeInLayoutUnits = measurer.UnitConverter.PixelsToLayoutUnitsF(Units.DocumentsToPixelsF(Units.PointsToDocumentsF(sizeInPoints), DocumentModelDpi.Dpi), DocumentModelDpi.Dpi);
			TTFontInfo fontInfo = fontDescriptor.FontInfo;
			Ascent = fontInfo.GetAscent(sizeInLayoutUnits);
			Descent = fontInfo.GetDescent(sizeInLayoutUnits);
			LineSpacing = Ascent + Descent + fontInfo.GetLineGap(sizeInLayoutUnits);
		}
		protected internal override void CalculateUnderlineAndStrikeoutParameters(FontInfoMeasurer measurer) {
			float sizeInPoints = DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(Font);
			float sizeInLayoutUnits = measurer.UnitConverter.PixelsToLayoutUnitsF(Units.DocumentsToPixelsF(Units.PointsToDocumentsF(sizeInPoints), DocumentModelDpi.Dpi), DocumentModelDpi.Dpi);
			TTFontInfo fontInfo = fontDescriptor.FontInfo;
			this.UnderlinePosition = fontInfo.GetUnderlinePosition(sizeInLayoutUnits);
			this.UnderlineThickness = fontInfo.GetUnderlineSize(sizeInLayoutUnits);
			this.StrikeoutPosition = fontInfo.GetStrikeOutPosition(sizeInLayoutUnits);
			this.StrikeoutThickness = fontInfo.GetStrikeOutSize(sizeInLayoutUnits);
			this.SubscriptSize = new Size(fontInfo.GetSubscriptXSize(sizeInLayoutUnits), fontInfo.GetSubscriptYSize(sizeInLayoutUnits));
			this.SubscriptOffset = new Point(fontInfo.GetSubscriptXOffset(sizeInLayoutUnits), fontInfo.GetSubscriptYOffset(sizeInLayoutUnits));
			this.SuperscriptSize = new Size(fontInfo.GetSuperscriptXSize(sizeInLayoutUnits), fontInfo.GetSuperscriptYSize(sizeInLayoutUnits));
			this.SuperscriptOffset = new Point(fontInfo.GetSuperscriptXOffset(sizeInLayoutUnits), fontInfo.GetSuperscriptYOffset(sizeInLayoutUnits));
		}
		protected internal override void Initialize(FontInfoMeasurer measurer) {
		}
		#endregion
		protected internal override void CalculateSuperscriptOffset(FontInfo baseFontInfo) {
		}
		protected internal override void CalculateSubscriptOffset(FontInfo baseFontInfo) {
		}
		protected internal override int CalculateFontCharset(FontInfoMeasurer measurer) {
			if (this.Name == "Symbol")
				return 2;
			return 1;
		}
	}
	#endregion
}
