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
	public class PdfCoonsPatchPainter : PdfPatchMeshPainter {
		public PdfCoonsPatchPainter(PdfCoonsPatchMesh shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
			: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) {
		}
		PdfRenderingBezierCurve Convert(PdfBezierCurve curve) {
			IShadingCoordsConverter coordConverter = CoordsConverter;
			return new PdfRenderingBezierCurve(coordConverter.Convert(curve.Vertex1.Point), 
				coordConverter.Convert(curve.ControlPoint1), coordConverter.Convert(curve.ControlPoint2), coordConverter.Convert(curve.Vertex2.Point));
		}
		protected override IList<PdfRenderingPatch> CreatePatches(PdfMeshShading shading) {
			List<PdfRenderingPatch> patches = new List<PdfRenderingPatch>();
			PdfCoonsPatchMesh mesh = shading as PdfCoonsPatchMesh;
			if (mesh != null) {
				IShadingColorConverter colorConverter = ColorConverter;
				foreach (PdfCoonsPatch shadingPatch in mesh.Patches) { 
					PdfBezierCurve left = shadingPatch.Left;
					PdfBezierCurve right = shadingPatch.Right;
					Color[] colors = new Color[] { colorConverter.Convert(left.Vertex1.Color.Components), colorConverter.Convert(left.Vertex2.Color.Components),
												   colorConverter.Convert(right.Vertex1.Color.Components), colorConverter.Convert(right.Vertex2.Color.Components) };
					patches.Add(new PdfRenderingCoonsPatch(Convert(left), Convert(shadingPatch.Top), Convert(right), Convert(shadingPatch.Bottom), colors));
				}
			}
			return patches;
		}
	}
}
