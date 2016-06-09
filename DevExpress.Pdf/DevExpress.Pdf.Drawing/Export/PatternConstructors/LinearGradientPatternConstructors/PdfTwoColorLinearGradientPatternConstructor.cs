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
	public class PdfTwoColorLinearGradientPatternConstructor : PdfLinearGradientPatternConstructor {
		public PdfTwoColorLinearGradientPatternConstructor(PdfLinearGradientBrush brush, PdfRectangle bBox)
			: base(brush, bBox) {
		}
		protected override IList<double> Positions { get { return Brush.Blend.Positions; } }
		protected override PdfObjectList<PdfCustomFunction> CreateFunctions(PdfDocumentCatalog documentCatalog) {
			double[] startColor = GetPdfColorComponents(Brush.LinearColors[0]);
			double[] endColor = GetPdfColorComponents(Brush.LinearColors[1]);
			PdfCustomFunction interpolationFunction = new PdfExponentialInterpolationFunction(startColor, endColor, 1, FunctionDomain, FunctionsRange);
			double[] positions = Brush.Blend.Factors;
			PdfObjectList<PdfCustomFunction> functions = new PdfObjectList<PdfCustomFunction>(documentCatalog.Objects);
			for (int i = 0; i < positions.Length; i++)
				functions.Add(interpolationFunction);
			return functions;
		}
		protected override PdfRange[] CreateEncodeArray() {
			double[] factors = Brush.Blend.Factors;
			PdfRange[] encode = new PdfRange[factors.Length];
			double startPosition = 0;
			for (int i = 0; i < factors.Length; i++) {
				encode[i] = new PdfRange(startPosition, factors[i]);
				startPosition = factors[i];
			}
			return encode;
		}
	}
}
