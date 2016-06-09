#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfParallelogram {
		readonly double lowerLeftX;
		readonly double lowerLeftY;
		readonly double upperLeftX;
		readonly double upperLeftY;
		readonly double upperRightX;
		readonly double upperRightY;
		readonly double lowerRightX;
		readonly double lowerRightY;
		public double LowerLeftX { get { return lowerLeftX; } }
		public double LowerLeftY { get { return lowerLeftY; } }
		public double UpperLeftX { get { return upperLeftX; } }
		public double UpperLeftY { get { return upperLeftY; } }
		public double UpperRightX { get { return upperRightX; } }
		public double UpperRightY { get { return upperRightY; } }
		public double LowerRightX { get { return lowerRightX; } }
		public double LowerRightY { get { return lowerRightY; } }
		internal PdfParallelogram(IList<object> array) {
			if (array.Count != 8)
				PdfDocumentReader.ThrowIncorrectDataException();
			lowerLeftX = PdfDocumentReader.ConvertToDouble(array[0]);
			lowerLeftY = PdfDocumentReader.ConvertToDouble(array[1]);
			upperLeftX = PdfDocumentReader.ConvertToDouble(array[2]);
			upperLeftY = PdfDocumentReader.ConvertToDouble(array[3]);
			upperRightX = PdfDocumentReader.ConvertToDouble(array[4]);
			upperRightY = PdfDocumentReader.ConvertToDouble(array[5]);
			lowerRightX = PdfDocumentReader.ConvertToDouble(array[6]);
			lowerRightY = PdfDocumentReader.ConvertToDouble(array[7]);
		}
		internal double[] ToWriteableObject() {
			return new double[] { lowerLeftX, lowerLeftY, upperLeftX, upperLeftY, upperRightX, upperRightY, lowerRightX, lowerRightY };
		}
	}
}
