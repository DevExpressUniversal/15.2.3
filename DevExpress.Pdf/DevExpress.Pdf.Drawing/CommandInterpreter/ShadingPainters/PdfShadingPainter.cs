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
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfShadingPainter {
		protected const int Degree180 = 180;
		protected const double RadianToDegreeFactor = Degree180 / Math.PI;
		static PdfShadingPainter CreatePainter(PdfViewerCommandInterpreter interpreter, int bitmapWidth, int bitmapHeight, PdfShading shading, PdfTransformationMatrix matrix, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor) { 
			PdfShadingPainter painter;
			PdfAxialShading axialShading = shading as PdfAxialShading;
			if (axialShading == null) {
				PdfRadialShading radialShading = shading as PdfRadialShading;
				if (radialShading == null) { 
					PdfGourandShadedTriangleMesh gourandShadedTriangleMesh = shading as PdfGourandShadedTriangleMesh;
					if (gourandShadedTriangleMesh == null) { 
						PdfCoonsPatchMesh coonsPatchMesh = shading as PdfCoonsPatchMesh;
						if (coonsPatchMesh == null) {
							PdfTensorProductPatchMesh tensorProductPatchMesh = shading as PdfTensorProductPatchMesh;
							if (tensorProductPatchMesh == null)
								return null;
							painter = new PdfTensorProductPatchPainter(tensorProductPatchMesh, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight);
						}
						else
							painter = new PdfCoonsPatchPainter(coonsPatchMesh, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight);
					}
					else
						painter = new PdfGourandShadedTriangleMeshPainter(gourandShadedTriangleMesh, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight);
				}
				else
					painter = new PdfRadialShadingPainter(radialShading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight);
			}
			else   
				painter = new PdfAxialShadingPainter(axialShading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight);
			return painter;
		}
		static Bitmap CreateBitmap(PdfViewerCommandInterpreter interpreter, PdfShading shading, int width, int height, PdfTransformationMatrix matrix, bool shouldUseTransparentBackgroundColor) { 
			PdfShadingPainter painter = CreatePainter(interpreter, width, height, shading, matrix, true, shouldUseTransparentBackgroundColor);
			if (painter == null)  
				return null;
			Bitmap bitmap = new Bitmap(width, height);
			using (Graphics graphics = Graphics.FromImage(bitmap)) 
				try { 
					painter.Paint(graphics);
				}
				catch { 
					bitmap.Dispose();
					throw;
				}
			return bitmap;
		}
		public static Bitmap CreateBitmap(PdfViewerCommandInterpreter interpreter, PdfShading shading) { 
			Size bitmapSize = interpreter.CalculateSize(interpreter.GraphicsState.TransformationMatrix, shading.BoundingBox);
			int width = bitmapSize.Width;
			int height = bitmapSize.Height;
			return (width == 0 || height == 0) ? null : CreateBitmap(interpreter, shading, width, height, null, true);
		}
		public static Bitmap CreateBitmap(PdfViewerCommandInterpreter interpreter, PdfShadingPattern pattern) { 
			return CreateBitmap(interpreter, pattern.Shading, interpreter.Width, interpreter.Height, PdfTransformationMatrix.Multiply(pattern.Matrix, interpreter.GraphicsState.TransformationMatrix), false);
		}
		public static void Draw(PdfViewerCommandInterpreter interpreter, Graphics graphics, PdfShading shading) { 
			PdfShadingPainter painter = CreatePainter(interpreter, interpreter.Width, interpreter.Height, shading, interpreter.GraphicsState.TransformationMatrix, false, false);
			if (painter != null)
				painter.Paint(graphics);
		}
		readonly PdfShading shading;
		readonly bool shouldDrawBackground;
		readonly bool shouldUseTransparentBackgroundColor;
		readonly IShadingCoordsConverter coordsConverter;
		IShadingColorConverter colorConverter;
		protected IShadingCoordsConverter CoordsConverter { get { return coordsConverter; } }
		protected IShadingColorConverter ColorConverter { 
			get { return colorConverter; } 
			set { colorConverter = value; }
		}
		public Color ConvertColor(double[] colorComponents) { 
			return PdfRenderingCommandInterpreter.ConvertToGDIPlusColor(shading.TransformFunction(colorComponents));
		}
		protected PdfShadingPainter(PdfShading shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) { 
			this.shading = shading;
			this.shouldDrawBackground = shouldDrawBackground;
			this.shouldUseTransparentBackgroundColor = shouldUseTransparentBackgroundColor;
			coordsConverter = matrix == null ? (IShadingCoordsConverter)new PdfBitmapShadingCoordsConverter(shading, bitmapWidth, bitmapHeight) : 
											   (IShadingCoordsConverter)new PdfUserSpaceShadingCoordsConverter(interpreter, matrix);
			colorConverter = new PdfShadingColorConverter(this);
		}
		protected virtual void Paint(Graphics graphics) {
			if (shouldDrawBackground) { 
				Color color;
				if (shouldUseTransparentBackgroundColor) 
					color = Color.Transparent;
				else { 
					PdfColor backgroundColor = shading.Background;
					if (backgroundColor == null)
						color = Color.Transparent;
					else
						color = PdfRenderingCommandInterpreter.ConvertToGDIPlusColor(backgroundColor);
				}
				graphics.Clear(color);
			}
		}
	}
}
