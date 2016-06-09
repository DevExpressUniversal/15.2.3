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
	#region DocumentLayoutUnitDocumentConverter
	public class DocumentLayoutUnitDocumentConverter : DocumentLayoutUnitConverter {
		public DocumentLayoutUnitDocumentConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
		}
		public DocumentLayoutUnitDocumentConverter() {
		}
		public override float Dpi { get { return 300.0f; } }
		public override LayoutGraphicsUnit GraphicsPageUnit { get { return LayoutGraphicsUnit.Document; } }
		public override float GraphicsPageScale { get { return 1.0f; } }
		public override LayoutGraphicsUnit FontUnit { get { return LayoutGraphicsUnit.Document; } }
		public override float FontSizeScale { get { return 1.0f; } }
		public override int PointsToFontUnits(int value) {
			return Units.PointsToDocuments(value);
		}
		public override float PointsToFontUnitsF(float value) {
			return Units.PointsToDocumentsF(value);
		}
		public override float DocumentsToFontUnitsF(float value) {
			return value;
		}
		public override float InchesToFontUnitsF(float value) {
			return Units.InchesToDocumentsF(value);
		}
		public override float MillimetersToFontUnitsF(float value) {
			return Units.MillimetersToDocumentsF(value);
		}
		public override int SnapToPixels(int value, float dpi) {
			return value;
		}
		public override int PixelsToLayoutUnits(int value, float dpi) {
			return Units.PixelsToDocuments(value, dpi);
		}
		public override float PixelsToLayoutUnitsF(float value, float dpi) {
			return Units.PixelsToDocumentsF(value, dpi);
		}
		public override Rectangle PixelsToLayoutUnits(Rectangle value, float dpiX, float dpiY) {
			return Units.PixelsToDocuments(value, dpiX, dpiY);
		}
		public override Size PixelsToLayoutUnits(Size value, float dpiX, float dpiY) {
			return Units.PixelsToDocuments(value, dpiX, dpiY);
		}
		public override int DocumentsToLayoutUnits(int value) {
			return value;
		}
		public override Rectangle DocumentsToLayoutUnits(Rectangle value) {
			return value;
		}
		public override RectangleF DocumentsToLayoutUnits(RectangleF value) {
			return value;
		}
		public override int TwipsToLayoutUnits(int value) {
			return Units.TwipsToDocuments(value);
		}
		public override long TwipsToLayoutUnits(long value) {
			return Units.TwipsToDocumentsL(value);
		}
		public override int PointsToLayoutUnits(int value) {
			return Units.PointsToDocuments(value);
		}
		public override float PointsToLayoutUnitsF(float value) {
			return Units.PointsToDocumentsF(value);
		}
		public override float LayoutUnitsToPointsF(float value) {
			return Units.DocumentsToPointsF(value);
		}
		public override int LayoutUnitsToPixels(int value, float dpi) {
			return Units.DocumentsToPixels(value, dpi);
		}
		public override float LayoutUnitsToPixelsF(float value, float dpi) {
			return Units.DocumentsToPixelsF(value, dpi);
		}
		public override Rectangle LayoutUnitsToPixels(Rectangle value, float dpiX, float dpiY) {
			return Units.DocumentsToPixels(value, dpiX, dpiY);
		}
		public override Point LayoutUnitsToPixels(Point value, float dpiX, float dpiY) {
			return Units.DocumentsToPixels(value, dpiX, dpiY);
		}
		public override Size LayoutUnitsToPixels(Size value, float dpiX, float dpiY) {
			return Units.DocumentsToPixels(value, dpiX, dpiY);
		}
		public override int LayoutUnitsToHundredthsOfInch(int value) {
			return Units.DocumentsToHundredthsOfInch(value);
		}
		public override Size LayoutUnitsToHundredthsOfInch(Size value) {
			return Units.DocumentsToHundredthsOfInch(value);
		}
		public override Rectangle LayoutUnitsToDocuments(Rectangle value) {
			return value;
		}
		public override RectangleF LayoutUnitsToDocuments(RectangleF value) {
			return value;
		}
		public override int LayoutUnitsToTwips(int value) {
			return Units.DocumentsToTwips(value);
		}
		public override long LayoutUnitsToTwips(long value) {
			return Units.DocumentsToTwipsL(value);
		}
	}
	#endregion
}
