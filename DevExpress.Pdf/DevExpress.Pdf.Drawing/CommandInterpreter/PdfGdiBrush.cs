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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfGdiBrush : PdfDisposableObject {
		readonly Bitmap bitmap;
		Brush brush;
		public Brush Brush { get { return brush; } }
		PdfGdiBrush(Bitmap bitmap, Brush brush) { 
			this.bitmap = bitmap;
			this.brush = brush;
		}
		TextureBrush CreateTextureBrush(double alphaConstant) { 
			TextureBrush textureBrush = alphaConstant == 1 ? new TextureBrush(bitmap) : 
				new TextureBrush(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), PdfRenderingCommandInterpreter.GetImageOpacityAttributes(alphaConstant));
			brush = textureBrush;
			return textureBrush;
		}
		public PdfGdiBrush(PdfViewerCommandInterpreter interpreter, PdfColor color, double alphaConstant) { 
			bitmap = null;
			if (color != null) { 
				PdfPattern pattern = color.Pattern;
				if (pattern != null) { 
					PdfShadingPattern shadingPattern = pattern as PdfShadingPattern;
					if (shadingPattern != null) { 
						bitmap = PdfShadingPainter.CreateBitmap(interpreter, shadingPattern);
						if (bitmap != null) { 
							PointF location = interpreter.Location;
							CreateTextureBrush(alphaConstant).TranslateTransform(location.X, location.Y);
							return;
						}
					}
					PdfTilingPattern tilingPattern = pattern as PdfTilingPattern;
					if (tilingPattern != null)  {
						PdfTransformationMatrix patternMatrix = pattern.Matrix;
						Size bitmapSize = interpreter.CalculateSize(patternMatrix, new PdfRectangle(0, 0, Math.Abs(tilingPattern.XStep), Math.Abs(tilingPattern.YStep)));
						PdfDocumentState documentState = interpreter.DocumentState;
						int pageIndex = interpreter.PageIndex;
						int angle = PdfPageTreeNode.NormalizeRotate(documentState.RotationAngle + documentState.GetPageState(pageIndex).Page.Rotate); 
						if (angle == 90 || angle == 270)
							bitmapSize = new Size(Math.Min(bitmapSize.Width, interpreter.Height), Math.Min(bitmapSize.Height, interpreter.Width));
						else
							bitmapSize = new Size(Math.Min(bitmapSize.Width, interpreter.Width), Math.Min(bitmapSize.Height, interpreter.Height));
						PdfRectangle boundingBox = tilingPattern.BoundingBox;
						Size keyCellSize = interpreter.CalculateSize(patternMatrix, boundingBox);
						bitmap = PdfViewerCommandInterpreter.GetTilingPatternKeyCellBitmap(documentState, pageIndex, tilingPattern, bitmapSize, keyCellSize, color);
						if (bitmap != null) { 
							PdfTransformationMatrix patternTransformationMatrix = new PdfTransformationMatrix((float)(boundingBox.Width / keyCellSize.Width), 
								0, 0, (float)(-boundingBox.Height / keyCellSize.Height), boundingBox.Left, boundingBox.Top);
							PdfTransformationMatrix brushTransformationMatrix = PdfTransformationMatrix.Multiply(patternTransformationMatrix, patternMatrix);
							brushTransformationMatrix = PdfTransformationMatrix.Multiply(brushTransformationMatrix, interpreter.TilingPatternTransformationMatrix);
							brushTransformationMatrix = PdfTransformationMatrix.Multiply(brushTransformationMatrix, interpreter.DeviceTransformationMatrix);
							Matrix matrix = new Matrix((float)(brushTransformationMatrix.A), (float)(brushTransformationMatrix.B), (float)(brushTransformationMatrix.C), 
								(float)(brushTransformationMatrix.D), (float)(brushTransformationMatrix.E), (float)(brushTransformationMatrix.F));
							using (matrix) 
								CreateTextureBrush(alphaConstant).Transform = matrix;
							return;
						}
					}
				}
			}
			brush = new SolidBrush(PdfRenderingCommandInterpreter.ConvertToGDIPlusColor(color, alphaConstant));
		}
		public PdfGdiBrush Clone() { 
			return new PdfGdiBrush(bitmap == null ? null : (Bitmap)bitmap.Clone(), (Brush)brush.Clone());
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				brush.Dispose();
				if (bitmap != null)
					bitmap.Dispose();
			}
		}
	}
}
