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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Layout {
	#region DocumentLayoutUnitTwipsConverter
	public class DocumentLayoutUnitTwipsConverter : DocumentLayoutUnitConverter {
		public DocumentLayoutUnitTwipsConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
		}
		public DocumentLayoutUnitTwipsConverter() {
		}
		public override float Dpi { get { return 1440.0f; } }
		public override LayoutGraphicsUnit GraphicsPageUnit { get { return LayoutGraphicsUnit.Point; } }
		public override float GraphicsPageScale { get { return 1.0f / 20.0f; } }
		public override LayoutGraphicsUnit FontUnit { get { return LayoutGraphicsUnit.Point; } }
		public override float FontSizeScale { get { return 1.0f / 20.0f; } }
		public override int PointsToFontUnits(int value) {
			return value;
		}
		public override float PointsToFontUnitsF(float value) {
			return value;
		}
		public override float DocumentsToFontUnitsF(float value) {
			return Units.DocumentsToPointsF(value);
		}
		public override float InchesToFontUnitsF(float value) {
			return Units.InchesToPointsF(value);
		}
		public override float MillimetersToFontUnitsF(float value) {
			return Units.MillimetersToPointsF(value);
		}
		public override int SnapToPixels(int value, float dpi) {
			return (int)Math.Round(Dpi * Math.Round(dpi * value / Dpi) / dpi);
		}
		public override int PixelsToLayoutUnits(int value, float dpi) {
			return Units.PixelsToTwips(value, dpi);
		}
		public override float PixelsToLayoutUnitsF(float value, float dpi) {
			return Units.PixelsToTwipsF(value, dpi);
		}
		public override Rectangle PixelsToLayoutUnits(Rectangle value, float dpiX, float dpiY) {
			return Units.PixelsToTwips(value, dpiX, dpiY);
		}
		public override Size PixelsToLayoutUnits(Size value, float dpiX, float dpiY) {
			return Units.PixelsToTwips(value, dpiX, dpiY);
		}
		public override int DocumentsToLayoutUnits(int value) {
			return Units.DocumentsToTwips(value);
		}
		public override Rectangle DocumentsToLayoutUnits(Rectangle value) {
			return Units.DocumentsToTwips(value);
		}
		public override RectangleF DocumentsToLayoutUnits(RectangleF value) {
			return Units.DocumentsToTwips(value);
		}
		public override int TwipsToLayoutUnits(int value) {
			return value;
		}
		public override long TwipsToLayoutUnits(long value) {
			return value;
		}
		public override int PointsToLayoutUnits(int value) {
			return Units.PointsToTwips(value);
		}
		public override float PointsToLayoutUnitsF(float value) {
			return Units.PointsToTwipsF(value);
		}
		public override float LayoutUnitsToPointsF(float value) {
			return Units.TwipsToPointsF(value);
		}
		public override int LayoutUnitsToPixels(int value, float dpi) {
			return Units.TwipsToPixels(value, dpi);
		}
		public override float LayoutUnitsToPixelsF(float value, float dpi) {
			return Units.TwipsToPixelsF(value, dpi);
		}
		public override Rectangle LayoutUnitsToPixels(Rectangle value, float dpiX, float dpiY) {
			return Units.TwipsToPixels(value, dpiX, dpiY);
		}
		public override Point LayoutUnitsToPixels(Point value, float dpiX, float dpiY) {
			return Units.TwipsToPixels(value, dpiX, dpiY);
		}
		public override Size LayoutUnitsToPixels(Size value, float dpiX, float dpiY) {
			return Units.TwipsToPixels(value, dpiX, dpiY);
		}
		public override int LayoutUnitsToHundredthsOfInch(int value) {
			return Units.TwipsToHundredthsOfInch(value);
		}
		public override Size LayoutUnitsToHundredthsOfInch(Size value) {
			return Units.TwipsToHundredthsOfInch(value);
		}
		public override Rectangle LayoutUnitsToDocuments(Rectangle value) {
			return Units.TwipsToDocuments(value);
		}
		public override RectangleF LayoutUnitsToDocuments(RectangleF value) {
			return Units.TwipsToDocuments(value);
		}
		public override int LayoutUnitsToTwips(int value) {
			return value;
		}
		public override long LayoutUnitsToTwips(long value) {
			return value;
		}
	}
	#endregion
}
