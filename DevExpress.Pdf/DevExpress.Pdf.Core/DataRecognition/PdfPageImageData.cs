#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

namespace DevExpress.Pdf.Native {
	public class PdfPageImageData {
		static readonly PdfPoint bottomLeft = new PdfPoint(0, 0);
		static readonly PdfPoint[] corners = new PdfPoint[] { new PdfPoint(0, 1), new PdfPoint(1, 0), new PdfPoint(1, 1) };
		readonly PdfImage image;
		readonly PdfTransformationMatrix matrix;
		PdfRectangle boundingRectangle;
		public PdfImage Image { get { return image; } }
		public PdfTransformationMatrix Matrix { get { return matrix; } }
		public PdfRectangle BoundingRectangle {
			get {
				if (boundingRectangle == null) {
					PdfPoint point = matrix.Transform(bottomLeft);
					double minX = point.X;
					double maxX = minX;
					double minY = point.Y;
					double maxY = minY;
					foreach (PdfPoint corner in corners) {
						point = matrix.Transform(corner);
						double x = point.X;
						if (x < minX)
							minX = x;
						else if (x > maxX)
							maxX = x;
						double y = point.Y;
						if (y < minY)
							minY = y;
						else if (y > maxY)
							maxY = y;
					}
					boundingRectangle = new PdfRectangle(minX, minY, maxX, maxY);
				}
				return boundingRectangle;
			}
		}
		public PdfPageImageData(PdfImage image, PdfTransformationMatrix matrix) {
			this.image = image;
			this.matrix = matrix;
		}
		public bool Equals(PdfPageImageData imageData) {
			return image == imageData.image && matrix.Equals(imageData.matrix);
		}
	}
}
