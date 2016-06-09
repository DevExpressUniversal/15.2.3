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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfOrientedRectangle {
		readonly double top;
		readonly double left;
		readonly double width;
		readonly double height;
		readonly double angle;
		PdfRectangle boundingRectangle;
		List<PdfPoint> vertices;
		public double Left { get { return left; } }
		public double Top { get { return top; } }
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public double Angle { get { return angle; } }
		internal PdfPoint TopLeft { get { return new PdfPoint(left, top); } }
		internal double Bottom { get { return top + width * Math.Sin(angle) - height * Math.Cos(angle); } }
		internal double Right { get { return left + width * Math.Cos(angle) + height * Math.Sin(angle); } }
		internal PdfPoint TopRight {
			get {
				PdfPoint rotatedTopLeft = PdfTextUtils.RotatePoint(TopLeft, -angle);
				return PdfTextUtils.RotatePoint(new PdfPoint(rotatedTopLeft.X + width, rotatedTopLeft.Y), angle);
			}
		}
		internal IList<PdfPoint> Vertices {
			get {
				if (vertices == null) {
					vertices = new List<PdfPoint>();
					PdfPoint topLeft = TopLeft;
					double xOffset = topLeft.X;
					double yOffset = topLeft.Y;
					double cos = Math.Cos(angle);
					double sin = Math.Sin(angle);
					vertices.Add(topLeft);
					vertices.Add(new PdfPoint(xOffset + width * cos, yOffset + width * sin));
					vertices.Add(new PdfPoint(xOffset + width * cos + height * sin, yOffset - height * cos + width * sin));
					vertices.Add(new PdfPoint(xOffset + height * sin, yOffset - height * cos));
				}
				return vertices;
			}
		}
		internal PdfRectangle BoundingRectangle {
			get {
				if (boundingRectangle != null) 
					return boundingRectangle;
				double minx, maxx, miny, maxy;
				PdfPoint realTopLeft = TopLeft;
				PdfPoint rotatedTopLeft = PdfTextUtils.RotatePoint(realTopLeft, -angle);
				PdfPoint realTopRight = PdfTextUtils.RotatePoint(new PdfPoint(rotatedTopLeft.X + width, rotatedTopLeft.Y), angle);
				PdfPoint realBottomLeft = PdfTextUtils.RotatePoint(new PdfPoint(rotatedTopLeft.X, rotatedTopLeft.Y - height), angle);
				PdfPoint realBottomRight = PdfTextUtils.RotatePoint(new PdfPoint(rotatedTopLeft.X + width, rotatedTopLeft.Y - height), angle);
				minx = PdfMathUtils.Min(PdfMathUtils.Min(realTopLeft.X, realTopRight.X), PdfMathUtils.Min(realBottomLeft.X, realBottomRight.X));
				miny = PdfMathUtils.Min(PdfMathUtils.Min(realTopLeft.Y, realTopRight.Y), PdfMathUtils.Min(realBottomLeft.Y, realBottomRight.Y));
				maxx = PdfMathUtils.Max(PdfMathUtils.Max(realTopLeft.X, realTopRight.X), PdfMathUtils.Max(realBottomLeft.X, realBottomRight.X));
				maxy = PdfMathUtils.Max(PdfMathUtils.Max(realTopLeft.Y, realTopRight.Y), PdfMathUtils.Max(realBottomLeft.Y, realBottomRight.Y));
				try {
					boundingRectangle = new PdfRectangle(minx, miny, maxx, maxy);
				}
				catch (ArgumentOutOfRangeException) {
					boundingRectangle = new PdfRectangle(minx, miny, maxx + 1, maxy + 1);
				}
				return boundingRectangle;
			}
		}
		internal PdfOrientedRectangle(PdfPoint topLeft, double width, double height, double angle) {
			top = topLeft.Y;
			left = topLeft.X;
			this.width = width;
			this.height = height;
			this.angle = PdfMathUtils.NormalizeAngle(angle);
		}
		internal bool Overlaps(PdfOrientedRectangle rectangle) {
			const double overlapDistance = 1;
			return Math.Abs(rectangle.left - left) < overlapDistance && Math.Abs(rectangle.top - top) < overlapDistance &&
				Math.Abs(rectangle.width - width) < overlapDistance && Math.Abs(rectangle.height - height) < overlapDistance && angle == rectangle.angle;
		}
		internal bool PointIsInRect(PdfPoint point, double expandX = 0, double expandY = 0) {
			PdfPoint rotatedPoint = PdfTextUtils.RotatePoint(point, -angle);
			PdfPoint rotatedTopLeft = PdfTextUtils.RotatePoint(TopLeft, -angle);
			return rotatedPoint.X >= (rotatedTopLeft.X - expandX) && rotatedPoint.X <= (rotatedTopLeft.X + width + expandX) && rotatedPoint.Y <= (rotatedTopLeft.Y + expandY) && rotatedPoint.Y >= (rotatedTopLeft.Y - height - expandY);
		}
	}
}
