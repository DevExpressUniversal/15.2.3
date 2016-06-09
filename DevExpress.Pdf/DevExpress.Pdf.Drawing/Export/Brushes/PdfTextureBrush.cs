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

using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfTextureBrush : PdfTilingBrush {
		readonly int xObjectNumber;
		readonly double width;
		readonly double height;
		public PdfTextureBrush(int xObjectNumber, WrapMode wrapMode, double width, double height)
			: base(wrapMode) {
			this.xObjectNumber = xObjectNumber;
			this.width = width;
			this.height = height;
		}
		public override PdfTransparentColor GetColor(PdfRectangle bBox, PdfDocumentCatalog documentCatalog) {
			PdfRectangle bounds = new PdfRectangle(0, 0, width * 2, height * 2);
			double xStep;
			double yStep;
			if (WrapMode == WrapMode.Clamp) {
				xStep = bBox.Width;
				yStep = bBox.Height;
			}
			else {
				xStep = bounds.Width;
				yStep = bounds.Height;
			}
			PdfTilingPattern pattern = new PdfTilingPattern(Transform, bounds, xStep, yStep, documentCatalog);
			PdfCommandConstructor constructor = new PdfCommandConstructor(pattern.Commands, pattern.Resources);
			string objectName =  pattern.Resources.AddXObject(xObjectNumber);
			if (WrapMode == WrapMode.Clamp) {
				constructor.DrawXObject(objectName, new PdfRectangle(0, 0, width, height));
				return new PdfTransparentColor(255, pattern);
			}
			PdfTransformationMatrix xTransform;
			PdfTransformationMatrix yTransform;
			if (WrapMode == WrapMode.TileFlipX || WrapMode == WrapMode.TileFlipXY)
				xTransform = new PdfTransformationMatrix(-1, 0, 0, 1, width * 3, 0);
			else
				xTransform = new PdfTransformationMatrix();
			if (WrapMode == WrapMode.TileFlipY || WrapMode == WrapMode.TileFlipXY)
				yTransform = new PdfTransformationMatrix(1, 0, 0, -1, 0, height * 3);
			else
				yTransform = new PdfTransformationMatrix();
			PdfTransformationMatrix xyTransform = PdfTransformationMatrix.Multiply(xTransform, yTransform);
			constructor.DrawXObject(objectName, new PdfRectangle(0, 0, width, height));
			constructor.DrawXObject(objectName, new PdfRectangle(width, 0, bounds.Width, height), xTransform);
			constructor.DrawXObject(objectName, new PdfRectangle(0, height, width, bounds.Height), yTransform);
			constructor.DrawXObject(objectName, new PdfRectangle(width, height, bounds.Width, bounds.Height), xyTransform);
			return new PdfTransparentColor(255, pattern);
		}
	}
}
