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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfTextureBrushContainer : PdfBrushContainer {
		readonly Image image;
		readonly WrapMode wrapMode;
		readonly PdfTransformationMatrix brushTransform;
		public PdfTextureBrushContainer(TextureBrush brush) {
			image = (Image)brush.Image.Clone();
			wrapMode = brush.WrapMode;
			using (Matrix matrix = brush.Transform)
				brushTransform = PdfGraphicsConverter.MatrixToPdfTransformationMatrix(matrix);
		}
		public PdfTextureBrushContainer(Image image, WrapMode wrapMode, PdfTransformationMatrix transform) {
			this.image = (Image)image.Clone();
			this.wrapMode = wrapMode;
			this.brushTransform = transform;
		}
		internal override PdfBrush GetBrush(PdfGraphicsCommandConstructor context) {
			int number;
			if (image is Bitmap)
				number = context.ImageCache.GetPdfImageObjectNumber(image, false, null, 0);
			else
				number = context.ImageCache.GetPdfFormObjectNumber((Metafile)image);
			PdfTextureBrush brush = new PdfTextureBrush(number, wrapMode, image.Width, image.Height);
			float factorY = context.FactorY;
			PdfTransformationMatrix transform = new PdfTransformationMatrix(1 / context.FactorX, 0, 0, 1 / factorY, 0, context.BBox.Height - image.Height / factorY);
			PdfTransformationMatrix gdiTransform = new PdfTransformationMatrix(1, 0, 0, -1, 0, context.BBox.Height);
			transform = PdfTransformationMatrix.Multiply(transform, gdiTransform);
			transform = PdfTransformationMatrix.Multiply(transform, brushTransform);
			brush.Transform = PdfTransformationMatrix.Multiply(transform, PdfTransformationMatrix.Invert(gdiTransform));
			return brush;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				image.Dispose();
		}
	}
}
