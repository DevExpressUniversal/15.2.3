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
	#region DocumentModelUnitDocumentConverter
	public class DocumentModelUnitDocumentConverter : DocumentModelUnitConverter {
		public DocumentModelUnitDocumentConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
		}
		public DocumentModelUnitDocumentConverter() {
		}
		public override int TwipsToModelUnits(int value) {
			return Units.TwipsToDocuments(value);
		}
		public override Size TwipsToModelUnits(Size value) {
			return Units.TwipsToDocuments(value);
		}
		public override float MillimetersToModelUnitsF(float value) {
			return Units.MillimetersToDocumentsF(value);
		}
		public override int PointsToModelUnits(int value) {
			return Units.PointsToDocuments(value);
		}
		public override float PointsToModelUnitsF(float value) {
			return Units.PointsToDocumentsF(value);
		}
		public override int PixelsToModelUnits(int value, float dpi) {
			return Units.PixelsToDocuments(value, dpi);
		}
		public override Size PixelsToModelUnits(Size value, float dpiX, float dpiY) {
			return Units.PixelsToDocuments(value, dpiX, dpiY);
		}
		public override int HundredthsOfInchToModelUnits(int value) {
			return Units.HundredthsOfInchToDocuments(value);
		}
		public override Size HundredthsOfInchToModelUnits(Size value) {
			return Units.HundredthsOfInchToDocuments(value);
		}
		public override int HundredthsOfMillimeterToModelUnits(int value) {
			return Units.HundredthsOfMillimeterToDocuments(value);
		}
		public override Size HundredthsOfMillimeterToModelUnits(Size value) {
			return Units.HundredthsOfMillimeterToDocuments(value);
		}
		public override int HundredthsOfMillimeterToModelUnitsRound(int value) {
			return (int)Math.Round(300 * value / 2540.0);
		}
		public override float CentimetersToModelUnitsF(float value) {
			return Units.CentimetersToDocumentsF(value);
		}
		public override float InchesToModelUnitsF(float value) {
			return Units.InchesToDocumentsF(value);
		}
		public override float PicasToModelUnitsF(float value) {
			return Units.PicasToDocumentsF(value);
		}
		public override int DocumentsToModelUnits(int value) {
			return value;
		}
		public override Size DocumentsToModelUnits(Size value) {
			return value;
		}
		public override float DocumentsToModelUnitsF(float value) {
			return value;
		}
		public override int ModelUnitsToTwips(int value) {
			return Units.DocumentsToTwips(value);
		}
		public override float ModelUnitsToTwipsF(float value) {
			return Units.DocumentsToTwipsF(value);
		}
		public override Size ModelUnitsToTwips(Size value) {
			return Units.DocumentsToTwips(value);
		}
		public override Size ModelUnitsToHundredthsOfMillimeter(Size value) {
			return Units.DocumentsToHundredthsOfMillimeter(value);
		}
		public override float ModelUnitsToPointsF(float value) {
			return Units.DocumentsToPointsF(value);
		}
		public override float ModelUnitsToPointsFRound(float value) {
			return Units.DocumentsToPointsFRound(value);
		}
		public override int ModelUnitsToPixels(int value, float dpi) {
			return Units.DocumentsToPixels(value, dpi);
		}
		public override float ModelUnitsToPixelsF(float value, float dpi) {
			return Units.DocumentsToPixelsF(value, dpi);
		}
		public override float ModelUnitsToCentimetersF(float value) {
			return Units.DocumentsToCentimetersF(value);
		}
		public override float ModelUnitsToInchesF(float value) {
			return Units.DocumentsToInchesF(value);
		}
		public override float ModelUnitsToMillimetersF(float value) {
			return Units.DocumentsToMillimetersF(value);
		}
		public override float ModelUnitsToDocumentsF(float value) {
			return value;
		}
		public override int ModelUnitsToHundredthsOfInch(int value) {
			return Units.DocumentsToHundredthsOfInch(value);
		}
		public override Size ModelUnitsToHundredthsOfInch(Size value) {
			return Units.DocumentsToHundredthsOfInch(value);
		}
		public override DocumentModelUnitToLayoutUnitConverter CreateConverterToLayoutUnits(DocumentLayoutUnit unit, float dpi) {
			switch (unit) {
				default:
				case DocumentLayoutUnit.Twip:
					return new DocumentModelDocumentsToLayoutTwipsConverter();
				case DocumentLayoutUnit.Document:
					return new DocumentModelDocumentsToLayoutDocumentsConverter();
				case DocumentLayoutUnit.Pixel:
					return new DocumentModelDocumentsToLayoutPixelsConverter(dpi);
			}
		}
		public override int EmuToModelUnits(int value) {
			return Units.EmuToDocuments(value);
		}
		public override long EmuToModelUnitsL(long value) {
			return Units.EmuToDocumentsL(value);
		}
		public override float EmuToModelUnitsF(int value) {
			return Units.EmuToDocumentsF(value);
		}
		public override int ModelUnitsToEmu(int value) {
			return Units.DocumentsToEmu(value);
		}
		public override long ModelUnitsToEmuL(long value) {
			return Units.DocumentsToEmuL(value);
		}
		public override int ModelUnitsToEmuF(float value) {
			return Units.DocumentsToEmuF(value);
		}
		public override int FDToModelUnits(int value) {
			return value * 60000 / 65536;
		}
		public override int ModelUnitsToFD(int value) {
			return value * 65536 / 60000;
		}
	}
	#endregion
}
