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
	#region IDocumentModelUnitConverter
	public interface IDocumentModelUnitConverter {
		int TwipsToModelUnits(int value);
		Size TwipsToModelUnits(Size value);
		float MillimetersToModelUnitsF(float value);
		int PointsToModelUnits(int value);
		float PointsToModelUnitsF(float value);
		int PixelsToModelUnits(int value, float dpi);
		Size PixelsToModelUnits(Size value, float dpiX, float dpiY);
		int HundredthsOfInchToModelUnits(int value);
		Size HundredthsOfInchToModelUnits(Size value);
		int HundredthsOfMillimeterToModelUnits(int value);
		Size HundredthsOfMillimeterToModelUnits(Size value);
		int HundredthsOfMillimeterToModelUnitsRound(int value);
		float CentimetersToModelUnitsF(float value);
		float InchesToModelUnitsF(float value);
		float PicasToModelUnitsF(float value);
		int DocumentsToModelUnits(int value);
		Size DocumentsToModelUnits(Size value);
		float DocumentsToModelUnitsF(float value);
		int DegreeToModelUnits(float value);
		int FDToModelUnits(int value);
		int ModelUnitsToTwips(int value);
		float ModelUnitsToTwipsF(float value);
		Size ModelUnitsToTwips(Size value);
		Size ModelUnitsToHundredthsOfMillimeter(Size value);
		float ModelUnitsToPointsF(float value);
		float ModelUnitsToPointsFRound(float value);
		int ModelUnitsToPixels(int value, float dpi);
		float ModelUnitsToPixelsF(float value, float dpi);
		float ModelUnitsToCentimetersF(float value);
		float ModelUnitsToInchesF(float value);
		float ModelUnitsToMillimetersF(float value);
		float ModelUnitsToDocumentsF(float value);
		int ModelUnitsToHundredthsOfInch(int value);
		Size ModelUnitsToHundredthsOfInch(Size value);
		int ModelUnitsToDegree(int value);
		int ModelUnitsToFD(int value);
	}
	#endregion
}
