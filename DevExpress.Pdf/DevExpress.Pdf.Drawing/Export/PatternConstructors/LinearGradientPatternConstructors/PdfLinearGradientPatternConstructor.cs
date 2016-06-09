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
	public abstract class PdfLinearGradientPatternConstructor {
		public static PdfLinearGradientPatternConstructor Create(PdfLinearGradientBrush brush, PdfRectangle bBox) {
			if (brush.InterpolationColors == null)
				return new PdfTwoColorLinearGradientPatternConstructor(brush, bBox);
			else
				return new PdfMultiColorLinearGradientPatternConstructor(brush, bBox);
		}
		static readonly PdfRange[] functionsRange = new[] { new PdfRange(0, 1), new PdfRange(0, 1), new PdfRange(0, 1) };
		static readonly PdfRange[] functionDomain = new[] { new PdfRange(0, 1) };
		protected static double[] GetPdfColorComponents(Color color) {
			return new double[] { color.R / 255f, color.G / 255f, color.B / 255f };
		}
		protected static PdfRange[] FunctionsRange { get { return functionsRange; } }
		protected static PdfRange[] FunctionDomain { get { return functionDomain; } }
		readonly PdfLinearGradientBrush brush;
		protected PdfLinearGradientBrush Brush { get { return brush; } }
		protected abstract IList<double> Positions { get; }
		protected PdfLinearGradientPatternConstructor(PdfLinearGradientBrush brush, PdfRectangle bBox) {
			this.brush = brush;
		}
		public PdfPattern CreatePattern(PdfDocumentCatalog documentCatalog) {
			PdfCustomFunction function = CreateFunction(documentCatalog);
			PdfRectangle gradientRect = brush.Rectangle;
			PdfPoint startPoint = new PdfPoint(gradientRect.Left, gradientRect.Bottom + gradientRect.Height / 2);
			PdfPoint endPoint = new PdfPoint(gradientRect.Right, gradientRect.Bottom + gradientRect.Height / 2);
			PdfAxialShading shading = new PdfAxialShading(startPoint, endPoint, new PdfObjectList<PdfCustomFunction>(documentCatalog.Objects) { function });
			PdfBrushTileConstructor patternConstructor = new PdfBrushTileConstructor(brush.WrapMode, brush.Rectangle, shading, brush.Transform);
			return patternConstructor.CreatePattern(documentCatalog);
		}
		protected abstract PdfObjectList<PdfCustomFunction> CreateFunctions(PdfDocumentCatalog documentCatalog);
		protected abstract PdfRange[] CreateEncodeArray();
		PdfCustomFunction CreateFunction(PdfDocumentCatalog documentCatalog) {
			double[] bounds = new double[Positions.Count - 1];
			for (int i = 0; i < Positions.Count - 1; i++)
				bounds[i] = Positions[i];
			return new PdfStitchingFunction(bounds, CreateEncodeArray(), CreateFunctions(documentCatalog), functionDomain, functionsRange);
		}
	}
}
