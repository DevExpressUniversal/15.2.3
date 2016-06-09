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
namespace DevExpress.Pdf.Drawing {
	public class PdfTensorProductPatchPainter : PdfPatchMeshPainter {
		public PdfTensorProductPatchPainter(PdfTensorProductPatchMesh shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
			: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) {
		}
		protected override IList<PdfRenderingPatch> CreatePatches(PdfMeshShading shading) {
			List<PdfRenderingPatch> patches = new List<PdfRenderingPatch>();
			PdfTensorProductPatchMesh mesh = shading as PdfTensorProductPatchMesh;
			if (mesh != null) {
				IShadingCoordsConverter coordsConverter = CoordsConverter;
				IShadingColorConverter colorConverter = ColorConverter;
				foreach (PdfTensorProductPatch shadingPatch in mesh.Patches) { 
					PdfPoint[,] shadingControlPoints = shadingPatch.ControlPoints;
					PointF[,] controlPoints = new PointF[4, 4];
					for (int i = 0; i < 4; i++)
						for (int j = 0; j < 4; j++)
							controlPoints[i, j] = coordsConverter.Convert(shadingControlPoints[i, j]);
					PdfColor[] shadingColors = shadingPatch.Colors;
					Color[] colors = new Color[] { colorConverter.Convert(shadingColors[0].Components), colorConverter.Convert(shadingColors[1].Components),
												   colorConverter.Convert(shadingColors[2].Components), colorConverter.Convert(shadingColors[3].Components) };
					patches.Add(new PdfRenderingTensorProductPatch(controlPoints, colors));
				}
			}
			return patches;
		}
	}
}
