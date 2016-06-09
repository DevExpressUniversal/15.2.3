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
namespace DevExpress.Office {
	#region DocumentModelUnitTwipsConverter
	public class DocumentModelUnitTwipsConverter : DocumentModelUnitConverter {
		public DocumentModelUnitTwipsConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
		}
		public DocumentModelUnitTwipsConverter() {
		}
		public override int TwipsToModelUnits(int value) {
			return value;
		}
		public override Size TwipsToModelUnits(Size value) {
			return value;
		}
		public override float MillimetersToModelUnitsF(float value) {
			return Units.MillimetersToTwipsF(value);
		}
		public override int PointsToModelUnits(int value) {
			return Units.PointsToTwips(value);
		}
		public override float PointsToModelUnitsF(float value) {
			return Units.PointsToTwipsF(value);
		}
		public override int PixelsToModelUnits(int value, float dpi) {
			return Units.PixelsToTwips(value, dpi);
		}
		public override Size PixelsToModelUnits(Size value, float dpiX, float dpiY) {
			return Units.PixelsToTwips(value, dpiX, dpiY);
		}
		public override int HundredthsOfInchToModelUnits(int value) {
			return Units.HundredthsOfInchToTwips(value);
		}
		public override Size HundredthsOfInchToModelUnits(Size value) {
			return Units.HundredthsOfInchToTwips(value);
		}
		public override int HundredthsOfMillimeterToModelUnits(int value) {
			return Units.HundredthsOfMillimeterToTwips(value);
		}
		public override Size HundredthsOfMillimeterToModelUnits(Size value) {
			return Units.HundredthsOfMillimeterToTwips(value);
		}
		public override int HundredthsOfMillimeterToModelUnitsRound(int value) {
			return (int)Math.Round(1440 * value / 2540.0);
		}
		public override float CentimetersToModelUnitsF(float value) {
			return Units.CentimetersToTwipsF(value);
		}
		public override float InchesToModelUnitsF(float value) {
			return Units.InchesToTwipsF(value);
		}
		public override float PicasToModelUnitsF(float value) {
			return Units.PicasToTwipsF(value);
		}
		public override int DocumentsToModelUnits(int value) {
			return Units.DocumentsToTwips(value);
		}
		public override Size DocumentsToModelUnits(Size value) {
			return Units.DocumentsToTwips(value);
		}
		public override float DocumentsToModelUnitsF(float value) {
			return Units.DocumentsToTwipsF(value);
		}
		public override int ModelUnitsToTwips(int value) {
			return value;
		}
		public override float ModelUnitsToTwipsF(float value) {
			return value;
		}
		public override Size ModelUnitsToTwips(Size value) {
			return value;
		}
		public override Size ModelUnitsToHundredthsOfMillimeter(Size value) {
			return Units.TwipsToHundredthsOfMillimeter(value);
		}
		public override float ModelUnitsToPointsF(float value) {
			return Units.TwipsToPointsF(value);
		}
		public override float ModelUnitsToPointsFRound(float value) {
			return Units.TwipsToPointsFRound(value);
		}
		public override int ModelUnitsToPixels(int value, float dpi) {
			return Units.TwipsToPixels(value, dpi);
		}
		public override float ModelUnitsToPixelsF(float value, float dpi) {
			return Units.TwipsToPixelsF(value, dpi);
		}
		public override float ModelUnitsToCentimetersF(float value) {
			return Units.TwipsToCentimetersF(value);
		}
		public override float ModelUnitsToInchesF(float value) {
			return Units.TwipsToInchesF(value);
		}
		public override float ModelUnitsToMillimetersF(float value) {
			return Units.TwipsToMillimetersF(value);
		}
		public override float ModelUnitsToDocumentsF(float value) {
			return Units.TwipsToDocumentsF(value);
		}
		public override int ModelUnitsToHundredthsOfInch(int value) {
			return Units.TwipsToHundredthsOfInch(value);
		}
		public override Size ModelUnitsToHundredthsOfInch(Size value) {
			return Units.TwipsToHundredthsOfInch(value);
		}
		public override DocumentModelUnitToLayoutUnitConverter CreateConverterToLayoutUnits(DocumentLayoutUnit unit, float dpi) {
			switch (unit) {
				default:
				case DocumentLayoutUnit.Twip:
					return new DocumentModelTwipsToLayoutTwipsConverter();
				case DocumentLayoutUnit.Document:
					return new DocumentModelTwipsToLayoutDocumentsConverter();
				case DocumentLayoutUnit.Pixel:
					return new DocumentModelTwipsToLayoutPixelsConverter(dpi);
			}
		}
		public override int EmuToModelUnits(int value) {
			return Units.EmuToTwips(value);
		}
		public override long EmuToModelUnitsL(long value) {
			return Units.EmuToTwipsL(value);
		}
		public override float EmuToModelUnitsF(int value) {
			return Units.EmuToTwipsF(value);
		}
		public override int ModelUnitsToEmu(int value) {
			return Units.TwipsToEmu(value);
		}
		public override long ModelUnitsToEmuL(long value) {
			return Units.TwipsToEmuL(value);
		}
		public override int ModelUnitsToEmuF(float value) {
			return Units.TwipsToEmuF(value);
		}
		public override int FDToModelUnits(int value) {
			return (int)((long)value * 1875L / 2048L);
		}
		public override int ModelUnitsToFD(int value) {
			return (int)((long)value * 2048L / 1875L);
		}
	}
	#endregion
}
