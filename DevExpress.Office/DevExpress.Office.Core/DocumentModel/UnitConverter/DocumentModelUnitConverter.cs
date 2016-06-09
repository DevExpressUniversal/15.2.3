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
namespace DevExpress.Office {
	#region DpiSupport (abstract class)
	public abstract class DpiSupport {
		readonly float screenDpiX;
		readonly float screenDpiY;
		readonly float screenDpi;
		protected DpiSupport(float screenDpiX, float screenDpiY, float screenDpi) {
			this.screenDpiX = screenDpiX;
			this.screenDpiY = screenDpiY;
			this.screenDpi = screenDpi;
		}
		protected DpiSupport(float screenDpiX, float screenDpiY)
			: this(screenDpiX, screenDpiY, screenDpiX) {
		}
		protected DpiSupport()
			: this(DocumentModelDpi.DpiX, DocumentModelDpi.DpiY, DocumentModelDpi.Dpi) {
		}
		public float ScreenDpiX { get { return screenDpiX; } }
		public float ScreenDpiY { get { return screenDpiY; } }
		public float ScreenDpi { get { return screenDpi; } }
	}
	#endregion
	#region DocumentModelUnitConverter (astract class)
	public abstract class DocumentModelUnitConverter : DpiSupport, IDocumentModelUnitConverter {
		protected DocumentModelUnitConverter(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
		}
		protected DocumentModelUnitConverter() {
		}
		public abstract int TwipsToModelUnits(int value);
		public abstract Size TwipsToModelUnits(Size value);
		public abstract float MillimetersToModelUnitsF(float value);
		public abstract int PointsToModelUnits(int value);
		public abstract float PointsToModelUnitsF(float value);
		public abstract int PixelsToModelUnits(int value, float dpi);
		public int PixelsToModelUnits(int value) {
			return PixelsToModelUnits(value, ScreenDpi);
		}
		public abstract int EmuToModelUnits(int value);
		public abstract long EmuToModelUnitsL(long value);
		public abstract float EmuToModelUnitsF(int value);
		public abstract int ModelUnitsToEmu(int value);
		public abstract long ModelUnitsToEmuL(long value);
		public abstract int ModelUnitsToEmuF(float value);
		public abstract Size PixelsToModelUnits(Size value, float dpiX, float dpiY);
		public Size PixelsToModelUnits(Size value) {
			return PixelsToModelUnits(value, ScreenDpiX, ScreenDpiY);
		}
		public abstract int HundredthsOfInchToModelUnits(int value);
		public abstract Size HundredthsOfInchToModelUnits(Size value);
		public abstract int HundredthsOfMillimeterToModelUnits(int value);
		public abstract Size HundredthsOfMillimeterToModelUnits(Size value);
		public abstract int HundredthsOfMillimeterToModelUnitsRound(int value);
		public abstract float CentimetersToModelUnitsF(float value);
		public abstract float InchesToModelUnitsF(float value);
		public abstract float PicasToModelUnitsF(float value);
		public abstract int DocumentsToModelUnits(int value);
		public abstract Size DocumentsToModelUnits(Size value);
		public abstract float DocumentsToModelUnitsF(float value);
		public int AdjAngleToModelUnits(int value) {
			return value;
		}
		public int ModelUnitsToAdjAngle(int value) {
			return value;
		}
		public int DegreeToModelUnits(float value) {
			return (int)Math.Round(value * 60000);
		}
		public abstract int FDToModelUnits(int value);
		public abstract int ModelUnitsToTwips(int value);
		public abstract float ModelUnitsToTwipsF(float value);
		public abstract Size ModelUnitsToTwips(Size value);
		public abstract Size ModelUnitsToHundredthsOfMillimeter(Size value);
		public abstract float ModelUnitsToPointsF(float value);
		public abstract float ModelUnitsToPointsFRound(float value);
		public abstract int ModelUnitsToPixels(int value, float dpi);
		public int ModelUnitsToPixels(int value) {
			return ModelUnitsToPixels(value, ScreenDpi);
		}
		public abstract float ModelUnitsToPixelsF(float value, float dpi);
		public float ModelUnitsToPixelsF(float value) {
			return ModelUnitsToPixelsF(value, ScreenDpi);
		}
		public abstract float ModelUnitsToCentimetersF(float value);
		public abstract float ModelUnitsToInchesF(float value);
		public abstract float ModelUnitsToMillimetersF(float value);
		public abstract float ModelUnitsToDocumentsF(float value);
		public abstract int ModelUnitsToHundredthsOfInch(int value);
		public abstract Size ModelUnitsToHundredthsOfInch(Size value);
		public int ModelUnitsToDegree(int value) {
			return value / 60000;
		}
		public float ModelUnitsToDegreeF(int value) {
			return value / 60000f;
		}
		public float ModelUnitsToRadians(int value) {
			return (float)(Math.PI * value / 60000f / 180f);
		}
		public abstract int ModelUnitsToFD(int value);
		public abstract DocumentModelUnitToLayoutUnitConverter CreateConverterToLayoutUnits(DocumentLayoutUnit unit, float dpi);
		public DocumentModelUnitToLayoutUnitConverter CreateConverterToLayoutUnits(DocumentLayoutUnit unit) {
			return CreateConverterToLayoutUnits(unit, ScreenDpi);
		}
	}
	#endregion
}
