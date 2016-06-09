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
using DevExpress.Office.Layout;
using DevExpress.Utils.Internal;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Drawing {
	#region PrecalculatedMetricsFontInfoMeasurer
	public class PrecalculatedMetricsFontInfoMeasurer : FontInfoMeasurer {
		public PrecalculatedMetricsFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override void Initialize() {
		}
		public Size MeasureMultilineText(string s, FontInfo fontInfo, int availableWidth) {
			PrecalculatedMetricsFontInfo precalculatedMetricsFontInfo = (PrecalculatedMetricsFontInfo)fontInfo;
			FontDescriptor fontDescriptor = precalculatedMetricsFontInfo.FontDescriptor;
			if (fontDescriptor.FontInfo != null) {
				float fontSize = Units.DocumentsToPixelsF(Units.PointsToDocumentsF(DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(precalculatedMetricsFontInfo.Font)), DocumentModelDpi.Dpi);
				SizeF res = fontDescriptor.FontInfo.MeasureMultilineText(s, UnitConverter.LayoutUnitsToPixelsF(availableWidth), fontSize);
				return UnitConverter.PixelsToLayoutUnits(new Size((int)Math.Round(res.Width), (int)Math.Round(res.Height)), DocumentModelDpi.DpiX, DocumentModelDpi.DpiY);
			}
			throw new ArgumentException("font.Name");
		}
		#region MeasureString
		public override Size MeasureString(string s, FontInfo fontInfo) {
			PrecalculatedMetricsFontInfo precalculatedMetricsFontInfo = (PrecalculatedMetricsFontInfo)fontInfo;
			FontDescriptor fontDescriptor = precalculatedMetricsFontInfo.FontDescriptor;
			if (fontDescriptor.FontInfo != null) {
				SizeF res = fontDescriptor.FontInfo.MeasureText(s, Units.DocumentsToPixelsF(Units.PointsToDocumentsF(DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(precalculatedMetricsFontInfo.Font)), DocumentModelDpi.Dpi));
				return UnitConverter.PixelsToLayoutUnits(new Size((int)Math.Round(res.Width), (int)Math.Round(res.Height)), DocumentModelDpi.DpiX, DocumentModelDpi.DpiY);
			}
			throw new ArgumentException("font.Name");
		}
		#endregion
		#region MeasureCharacterBounds
		public Rectangle[] MeasureCharactersBounds(string s, PrecalculatedMetricsFontInfo fontInfo) {
			FontDescriptor fontDescriptor = fontInfo.FontDescriptor;
			if (fontDescriptor.FontInfo != null) {
				float sizeInPoints = fontInfo.SizeInPoints;
				float sizeInLayoutUnits = UnitConverter.PixelsToLayoutUnitsF(Units.DocumentsToPixelsF(Units.PointsToDocumentsF(sizeInPoints), DocumentModelDpi.Dpi), DocumentModelDpi.Dpi);
				Rectangle[] result = fontDescriptor.FontInfo.MeasureCharacterBounds(s, sizeInLayoutUnits);
				return result;
			}
			throw new ArgumentException("font.Name");
		}
		#endregion
		#region Platform specific methods
		public override float MeasureCharacterWidthF(char character, FontInfo fontInfo) {
			return MeasureString(character.ToString(), fontInfo).Width;
		}
		public virtual Font CreateFont(string familyName, float emSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			FontStyle style = FontStyle.Regular;
			if (fontBold) style |= FontStyle.Bold;
			if (fontItalic) style |= FontStyle.Italic;
			if (fontStrikeout) style |= FontStyle.Strikeout;
			if (fontUnderline) style |= FontStyle.Underline;
			return new Font(familyName, emSize, style, (GraphicsUnit)UnitConverter.FontUnit);
		}
		public override float MeasureMaxDigitWidthF(FontInfo fontInfo) {
			float result = MeasureString("0", fontInfo).Width;
			for(int i = 1; i < 10; i++)
				result = Math.Max(result, MeasureString(i.ToString(), fontInfo).Width);
			return result;
		}
		#endregion
	}
	#endregion
}
