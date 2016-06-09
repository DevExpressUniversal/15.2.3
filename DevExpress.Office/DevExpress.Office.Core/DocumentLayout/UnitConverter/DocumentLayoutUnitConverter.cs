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
namespace DevExpress.Office.Layout {
	#region DocumentLayoutUnitConverter (abstract class)
	public abstract class DocumentLayoutUnitConverter : DpiSupport {
		public static DocumentLayoutUnitConverter Create(DocumentLayoutUnit unit, float dpiX, float dpiY) {
			switch (unit) {
				default:
				case DocumentLayoutUnit.Twip:
					return new DocumentLayoutUnitTwipsConverter(dpiX, dpiY);
				case DocumentLayoutUnit.Document:
					return new DocumentLayoutUnitDocumentConverter(dpiX, dpiY);
				case DocumentLayoutUnit.Pixel:
					return new DocumentLayoutUnitPixelsConverter(dpiX, dpiY);
			}
		}
		public static DocumentLayoutUnitConverter Create(DocumentLayoutUnit unit, float dpi) {
			return Create(unit, dpi, dpi);
		}
		protected DocumentLayoutUnitConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
		}
		protected DocumentLayoutUnitConverter() {
		}
		public abstract float Dpi { get; }
		public abstract LayoutGraphicsUnit GraphicsPageUnit { get; }
		public abstract float GraphicsPageScale { get; }
		public abstract LayoutGraphicsUnit FontUnit { get; }
		public abstract float FontSizeScale { get; }
		public abstract int PointsToFontUnits(int value);
		public abstract float PointsToFontUnitsF(float value);
		public abstract float DocumentsToFontUnitsF(float value);
		public abstract float InchesToFontUnitsF(float value);
		public abstract float MillimetersToFontUnitsF(float value);
		public abstract int SnapToPixels(int value, float dpi);
		public int SnapToPixels(int value) {
			return SnapToPixels(value, ScreenDpi);
		}
		public abstract int PixelsToLayoutUnits(int value, float dpi);
		public int PixelsToLayoutUnits(int value) {
			return PixelsToLayoutUnits(value, ScreenDpi);
		}
		public abstract float PixelsToLayoutUnitsF(float value, float dpi);
		public float PixelsToLayoutUnitsF(float value) {
			return PixelsToLayoutUnitsF(value, ScreenDpi);
		}
		public abstract Rectangle PixelsToLayoutUnits(Rectangle value, float dpiX, float dpiY);
		public Rectangle PixelsToLayoutUnits(Rectangle value) {
			return PixelsToLayoutUnits(value, ScreenDpiX, ScreenDpiY);
		}
		public abstract Size PixelsToLayoutUnits(Size value, float dpiX, float dpiY);
		public Size PixelsToLayoutUnits(Size value) {
			return PixelsToLayoutUnits(value, ScreenDpiX, ScreenDpiY);
		}
		public abstract int DocumentsToLayoutUnits(int value);
		public abstract Rectangle DocumentsToLayoutUnits(Rectangle value);
		public abstract RectangleF DocumentsToLayoutUnits(RectangleF value);
		public abstract int TwipsToLayoutUnits(int value);
		public abstract long TwipsToLayoutUnits(long value);
		public abstract int PointsToLayoutUnits(int value);
		public abstract float PointsToLayoutUnitsF(float value);
		public abstract int LayoutUnitsToPixels(int value, float dpi);
		public int LayoutUnitsToPixels(int value) {
			return LayoutUnitsToPixels(value, ScreenDpi);
		}
		public abstract float LayoutUnitsToPixelsF(float value, float dpi);
		public float LayoutUnitsToPixelsF(float value) {
			return LayoutUnitsToPixelsF(value, ScreenDpi);
		}
		public abstract float LayoutUnitsToPointsF(float value);
		public abstract Rectangle LayoutUnitsToPixels(Rectangle value, float dpiX, float dpiY);
		public Rectangle LayoutUnitsToPixels(Rectangle value) {
			return LayoutUnitsToPixels(value, ScreenDpiX, ScreenDpiY);
		}
		public abstract Point LayoutUnitsToPixels(Point value, float dpiX, float dpiY);
		public Point LayoutUnitsToPixels(Point value) {
			return LayoutUnitsToPixels(value, ScreenDpiX, ScreenDpiY);
		}
		public abstract Size LayoutUnitsToPixels(Size value, float dpiX, float dpiY);
		public Size LayoutUnitsToPixels(Size value) {
			return LayoutUnitsToPixels(value, ScreenDpiX, ScreenDpiY);
		}
		public abstract int LayoutUnitsToHundredthsOfInch(int value);
		public abstract Size LayoutUnitsToHundredthsOfInch(Size value);
		public abstract Rectangle LayoutUnitsToDocuments(Rectangle value);
		public abstract RectangleF LayoutUnitsToDocuments(RectangleF value);
		public abstract int LayoutUnitsToTwips(int value);
		public abstract long LayoutUnitsToTwips(long value);
	}
	#endregion
	#region LayoutGraphicsUnit
	public enum LayoutGraphicsUnit {
		Pixel = 2,
		Point = 3,
		Document = 5,
	}
	#endregion
}
