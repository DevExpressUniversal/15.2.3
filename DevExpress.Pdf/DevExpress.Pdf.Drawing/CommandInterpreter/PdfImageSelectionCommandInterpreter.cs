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
	public class PdfImageSelectionCommandInterpreter : PdfRenderingCommandInterpreter {
		public static Bitmap GetSelectionBitmap(PdfPage page, PdfImageSelection imageSelection, PdfImageDataStorage imageDataStorage, int rotationAngle, float dpi) {
			try {
				using (PdfImageSelectionCommandInterpreter interpreter = new PdfImageSelectionCommandInterpreter(page, imageSelection, imageDataStorage, rotationAngle, dpi)) {
					interpreter.Execute();
					return interpreter.selectionBitmap;
				}
			}
			catch {
				return null;
			}
		}
		readonly PdfImageSelection selection;
		readonly RotateFlipType rotateType;
		Bitmap selectionBitmap;
		PdfImageSelectionCommandInterpreter(PdfPage page, PdfImageSelection selection, PdfImageDataStorage imageDataStorage, int rotationAngle, float dpi)
				: base(page, 0, imageDataStorage, dpi / DefaultPageDpi) {
			this.selection = selection;
			switch (PdfPageTreeNode.NormalizeRotate(page.Rotate + rotationAngle)) {
				case 90:
					rotateType = RotateFlipType.Rotate90FlipNone;
					break;
				case 180:
					rotateType = RotateFlipType.Rotate180FlipNone;
					break;
				case 270:
					rotateType = RotateFlipType.Rotate270FlipNone;
					break;
				default:
					rotateType = RotateFlipType.RotateNoneFlipNone;
					break;
			}
		}
		public override void DrawImage(PdfImage image) {
			if (selection != null) {
				PdfPageImageData pageImageData = selection.PageImageData;
				if (pageImageData.Equals(new PdfPageImageData(image, GraphicsState.TransformationMatrix))) {
					PdfImageData imageData = GetImageData(image);
					if (imageData != null) {
						float scale = Scale;
						PdfRectangle boundingRectangle = pageImageData.BoundingRectangle;
						float width = (float)(boundingRectangle.Width * scale);
						int bitmapWidth = width < 1 ? 1 : Convert.ToInt32(width);
						float height = (float)(boundingRectangle.Height * scale);
						int bitmapHeight = height < 1 ? 1 : Convert.ToInt32(height);
						using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight)) {
							using (Graphics graphics = Graphics.FromImage(bitmap)) {
								graphics.Clear(image.IsMask ? Color.Transparent : Color.White);
								graphics.InterpolationMode = GetInterpolationMode(imageData.Width, imageData.Height, image.BitsPerComponent);
								PerformRendering(image, imageData, sourceBitmap => {
									using (Matrix matrix = GetImageMatrix(PointF.Empty)) {
										Point[] points = new Point[] { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 0) };
										matrix.TransformPoints(points);
										int dx = 0;
										int dy = 0;
										foreach (Point p in points) {
											dx = Math.Min(dx, p.X);
											dy = Math.Min(dy, p.Y);
										}
										if (dx < 0 || dy < 0)
											matrix.Translate(-dx, -dy, MatrixOrder.Append);
										graphics.Transform = matrix;
									}
									DrawImage(graphics, sourceBitmap, ImageDestinationPoints);
								});
								PdfRectangle clipRectangle = selection.ClipRectangle;
								float x = Math.Min(bitmapWidth - 1, (float)(clipRectangle.Left * scale));
								float y = Math.Min(bitmapHeight - 1, (float)(height - clipRectangle.Top * scale));
								RectangleF rect = new RectangleF(x, y,
									Math.Max(1, Math.Min((float)(clipRectangle.Width * scale), bitmapWidth - x)), Math.Max(1, Math.Min((float)(clipRectangle.Height * scale), bitmapHeight - y)));
								selectionBitmap = bitmap.Clone(rect, bitmap.PixelFormat);
								selectionBitmap.RotateFlip(rotateType);
							}
						}
					}
				}
			}
		}
		public override void BeginText() {
		}
		public override void EndText() {
		}
		public override void SetTextMatrix(PdfTransformationMatrix matrix) {
		}
		public override void StrokePaths() {
		}
		public override void FillPaths(bool useNonzeroWindingRule) {
		}
		public override void ClipPaths(bool useNonzeroWindingRule) {
		}
		public override void DrawShading(PdfShading shading) {
		}
		protected override void DrawString(PdfStringData data, PdfPoint location, double[] glyphOffsets) {
		}
		protected override void UpdateGraphicsState(PdfGraphicsStateChange change) {
		}
	}
}
