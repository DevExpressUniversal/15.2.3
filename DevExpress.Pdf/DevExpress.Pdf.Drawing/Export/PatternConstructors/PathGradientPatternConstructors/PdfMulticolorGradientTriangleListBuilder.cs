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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfMulticolorGradientTriangleListBuilder : PdfTriangleListBuilder {
		const double gradientStartColorValue = 0;
		const double gradientEndColorValue = 0.5;
		readonly PdfColorBlend colorBlend;
		readonly PdfPoint focusScales;
		readonly PdfColor centerColor;
		protected override IList<PdfRange> ComponentsRange { get { return new[] { PdfColorRange }; } }
		public PdfMulticolorGradientTriangleListBuilder(PdfPoint centerPoint, PdfColorBlend colorBlend, PdfPoint focusScales) :
			this(centerPoint, colorBlend, focusScales, colorBlend.Colors[colorBlend.Colors.Length - 1]) {
		}
		public PdfMulticolorGradientTriangleListBuilder(PdfPoint centerPoint, PdfColorBlend colorBlend, PdfPoint focusScales, PdfColor centerColor)
			: base(centerPoint, new[] { Color.Black }) {
			this.colorBlend = colorBlend;
			this.focusScales = focusScales;
			this.centerColor = centerColor;
		}
		protected override IEnumerable<PdfTriangle> CreateTreangles(PdfPoint startPoint, PdfPoint endPoint, Color startColor, Color endColor) {
			double a = startPoint.X + (CenterPoint.X - startPoint.X) * (1 - focusScales.X);
			double b = startPoint.Y + (CenterPoint.Y - startPoint.Y) * (1 - focusScales.Y);
			double c = endPoint.X + (CenterPoint.X - endPoint.X) * (1 - focusScales.X);
			double d = endPoint.Y + (CenterPoint.Y - endPoint.Y) * (1 - focusScales.Y);
			PdfColor gradientStartColor = new PdfColor(gradientStartColorValue);
			PdfColor gradientEndColor = new PdfColor(gradientEndColorValue);
			PdfVertex newStartVertex = new PdfVertex(new PdfPoint(a, b), gradientEndColor);
			PdfVertex newEndVertex = new PdfVertex(new PdfPoint(c, d), gradientEndColor);
			return new[] {
				new PdfTriangle(new PdfVertex(startPoint, gradientStartColor), new PdfVertex(endPoint, gradientStartColor), newStartVertex),
				new PdfTriangle(newEndVertex, new PdfVertex(endPoint, gradientStartColor), newStartVertex),
				new PdfTriangle(newEndVertex, new PdfVertex(CenterPoint, new PdfColor(1)), newStartVertex)
			};
		}
		protected override PdfObjectList<PdfCustomFunction> CreateFunction(PdfDocumentCatalog documentCatalog) {
			double factor = (1 - gradientEndColorValue);
			PdfColor[] colors = colorBlend.Colors;
			double[] positions = colorBlend.Positions;
			double[] bounds = new double[colors.Length - 1];
			PdfRange[] encode = new PdfRange[colors.Length];
			PdfObjectList<PdfCustomFunction> functions = new PdfObjectList<PdfCustomFunction>(documentCatalog.Objects);
			PdfRange[] pdfRgbColorRanges = new[] { PdfColorRange, PdfColorRange, PdfColorRange };
			for (int i = 0; i < colors.Length - 1; i++) {
				if (i < colors.Length - 2)
					bounds[i] = positions[i + 1] * factor;
				encode[i] = new PdfRange(0, 1);
				double[] c0 = colors[i].Components;
				double[] c1 = colors[i + 1].Components;
				functions.Add(new PdfExponentialInterpolationFunction(c0, c1, 1, new[] { PdfColorRange }, pdfRgbColorRanges));
			}
			encode[colors.Length - 1] = new PdfRange(0, 1);
			bounds[colors.Length - 2] = gradientEndColorValue;
			functions.Add(new PdfExponentialInterpolationFunction(centerColor.Components, centerColor.Components, 1, new[] { PdfColorRange }, pdfRgbColorRanges));
			PdfStitchingFunction function = new PdfStitchingFunction(bounds, encode, functions, new[] { PdfColorRange }, pdfRgbColorRanges);
			return new PdfObjectList<PdfCustomFunction>(documentCatalog.Objects) { function };
		}
	}
}
