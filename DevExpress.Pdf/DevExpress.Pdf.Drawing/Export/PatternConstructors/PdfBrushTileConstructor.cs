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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfBrushTileConstructor {
		readonly PdfRectangle tileBounds;
		readonly PdfTransformationMatrix transform;
		readonly PdfShading shading;
		readonly WrapMode wrapMode;
		readonly PdfRectangle patternBounds;
		PdfCommandConstructor constructor;
		public PdfBrushTileConstructor(WrapMode wrapMode, PdfRectangle tileBounds, PdfShading shading, PdfTransformationMatrix transform) {
			this.wrapMode = wrapMode;
			this.tileBounds = tileBounds;
			this.shading = shading;
			this.transform = transform;
			patternBounds = new PdfRectangle(tileBounds.Left, tileBounds.Bottom, tileBounds.Width * 2 + tileBounds.Left, tileBounds.Height * 2 + tileBounds.Bottom);
		}
		public PdfPattern CreatePattern(PdfDocumentCatalog documentCatalog) {
			if (wrapMode == WrapMode.Clamp)
				return new PdfShadingPattern(shading, transform);
			PdfTilingPattern pattern = new PdfTilingPattern(transform, patternBounds, patternBounds.Width, patternBounds.Height, documentCatalog);
			constructor = new PdfCommandConstructor(pattern.Commands, pattern.Resources);
			PdfTransformationMatrix xTransform;
			PdfTransformationMatrix yTransform;
			if (wrapMode == WrapMode.TileFlipX || wrapMode == WrapMode.TileFlipXY)
				xTransform = new PdfTransformationMatrix(-1, 0, 0, 1, tileBounds.Right * 2, 0);
			else
				xTransform = new PdfTransformationMatrix(1, 0, 0, 1, tileBounds.Right, 0);
			if (wrapMode == WrapMode.TileFlipY || wrapMode == WrapMode.TileFlipXY)
				yTransform = new PdfTransformationMatrix(1, 0, 0, -1, 0, tileBounds.Top * 2);
			else
				yTransform = new PdfTransformationMatrix(1, 0, 0, 1, 0, tileBounds.Top);
			PdfTransformationMatrix xyTransform = PdfTransformationMatrix.Multiply(yTransform, xTransform);
			AppendTile(new PdfRectangle(tileBounds.Left, tileBounds.Bottom, tileBounds.Right, tileBounds.Top), shading, new PdfTransformationMatrix());
			AppendTile(new PdfRectangle(tileBounds.Right, tileBounds.Bottom, patternBounds.Right, tileBounds.Top), shading, xTransform);
			AppendTile(new PdfRectangle(tileBounds.Left, tileBounds.Top, tileBounds.Right, patternBounds.Top), shading, yTransform);
			AppendTile(new PdfRectangle(tileBounds.Right, tileBounds.Top, patternBounds.Right, patternBounds.Top), shading, xyTransform);
			return pattern;
		}
		void AppendTile(PdfRectangle tileBounds, PdfShading shading, PdfTransformationMatrix shadingTransform) {
			constructor.SetColorForNonStrokingOperations(new PdfColor(new PdfShadingPattern(shading, shadingTransform)));
			constructor.FillRectangle(tileBounds);
		}
	}
}
