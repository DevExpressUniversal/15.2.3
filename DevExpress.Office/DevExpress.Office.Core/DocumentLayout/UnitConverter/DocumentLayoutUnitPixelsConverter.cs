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
	#region DocumentLayoutUnitPixelsConverter
	public class DocumentLayoutUnitPixelsConverter : DocumentLayoutUnitConverter {
		readonly float dpi;
		public DocumentLayoutUnitPixelsConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
			this.dpi = screenDpiX;
		}
		public DocumentLayoutUnitPixelsConverter(float dpi)
			: this(dpi, dpi) {
		}
		public override float Dpi { get { return dpi; } }
		public override LayoutGraphicsUnit GraphicsPageUnit { get { return LayoutGraphicsUnit.Pixel; } }
		public override float GraphicsPageScale { get { return 1.0f; } }
		public override LayoutGraphicsUnit FontUnit { get { return LayoutGraphicsUnit.Point; } }
		public override float FontSizeScale { get { return 72.0f / Dpi; } }
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
			return value;
		}
		public override int PixelsToLayoutUnits(int value, float dpi) {
			return value;
		}
		public override float PixelsToLayoutUnitsF(float value, float dpi) {
			return value;
		}
		public override Rectangle PixelsToLayoutUnits(Rectangle value, float dpiX, float dpiY) {
			return value;
		}
		public override Size PixelsToLayoutUnits(Size value, float dpiX, float dpiY) {
			return value;
		}
		public override int DocumentsToLayoutUnits(int value) {
			return Units.DocumentsToPixels(value, Dpi);
		}
		public override Rectangle DocumentsToLayoutUnits(Rectangle value) {
			return Units.DocumentsToPixels(value, Dpi, Dpi);
		}
		public override RectangleF DocumentsToLayoutUnits(RectangleF value) {
			return Units.DocumentsToPixels(value, Dpi, Dpi);
		}
		public override int TwipsToLayoutUnits(int value) {
			return Units.TwipsToPixels(value, Dpi);
		}
		public override long TwipsToLayoutUnits(long value) {
			return Units.TwipsToPixelsL(value, Dpi);
		}
		public override int PointsToLayoutUnits(int value) {
			return Units.PointsToPixels(value, Dpi);
		}
		public override float PointsToLayoutUnitsF(float value) {
			return Units.PointsToPixelsF(value, Dpi);
		}
		public override float LayoutUnitsToPointsF(float value) {
			return Units.PixelsToPointsF(value, Dpi);
		}
		public override int LayoutUnitsToPixels(int value, float dpi) {
			return value;
		}
		public override float LayoutUnitsToPixelsF(float value, float dpi) {
			return value;
		}
		public override Rectangle LayoutUnitsToPixels(Rectangle value, float dpiX, float dpiY) {
			return value;
		}
		public override Point LayoutUnitsToPixels(Point value, float dpiX, float dpiY) {
			return value;
		}
		public override Size LayoutUnitsToPixels(Size value, float dpiX, float dpiY) {
			return value;
		}
		public override int LayoutUnitsToHundredthsOfInch(int value) {
			return Units.PixelsToHundredthsOfInch(value, Dpi);
		}
		public override Size LayoutUnitsToHundredthsOfInch(Size value) {
			return Units.PixelsToHundredthsOfInch(value, Dpi);
		}
		public override Rectangle LayoutUnitsToDocuments(Rectangle value) {
			return Units.PixelsToDocuments(value, Dpi, Dpi);
		}
		public override RectangleF LayoutUnitsToDocuments(RectangleF value) {
			return Units.PixelsToDocuments(value, Dpi, Dpi);
		}
		public override int LayoutUnitsToTwips(int value) {
			return Units.PixelsToTwips(value, Dpi);
		}
		public override long LayoutUnitsToTwips(long value) {
			return Units.PixelsToTwipsL(value, Dpi);
		}
	}
	#endregion
}
