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
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfLinearGradientBrushContainer : PdfTilingBrushContainer {
		public PdfLinearGradientBrushContainer(LinearGradientBrush brush) {
			PdfRectangle rect = PdfGraphicsConverter.RectangleFtoPdfRectangle(brush.Rectangle);
			PdfLinearGradientBrush linearGradientBrush = new PdfLinearGradientBrush(rect, brush.LinearColors[0], brush.LinearColors[1]);
			using (Matrix transform = brush.Transform)
				linearGradientBrush.Transform = PdfGraphicsConverter.MatrixToPdfTransformationMatrix(transform);
			if (brush.Blend == null) {
				PdfColorBlend interpolationColors = new PdfColorBlend();
				interpolationColors.Positions = Array.ConvertAll<float, double>(brush.InterpolationColors.Positions, p => p);
				interpolationColors.Colors = Array.ConvertAll<Color, PdfColor>(brush.InterpolationColors.Colors, p => new PdfColor(p.R / 255.0, p.G / 255.0, p.B / 255.0));
				linearGradientBrush.InterpolationColors = interpolationColors;
			}
			else {
				PdfBlend blend = new PdfBlend();
				blend.Positions = Array.ConvertAll<float, double>(brush.Blend.Positions, p => p);
				blend.Factors = Array.ConvertAll<float, double>(brush.Blend.Factors, p => p);
				linearGradientBrush.Blend = blend;
			}
			SetBrush(linearGradientBrush);
		}
		public PdfLinearGradientBrushContainer(PdfLinearGradientBrush brush) {
			SetBrush(brush);
		}
	}
}
