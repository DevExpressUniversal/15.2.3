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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.Office.Utils {
	#region RectangleUtils
	public static class RectangleUtils {
		public static Rectangle[] SplitHorizontally(Rectangle bounds, int cellCount) {
			if (cellCount <= 0)
				return new Rectangle[] { bounds };
			Rectangle[] cells = new Rectangle[cellCount];
			int offset = bounds.X;
			int height = bounds.Height;
			int columnsAreaWidth = bounds.Width;
			int columnWidth = columnsAreaWidth / cellCount;
			int remainder = columnsAreaWidth - columnWidth * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int width = columnWidth + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rectangle(offset, bounds.Y, width, height);
				offset += width;
			}
			return cells;
		}
		public static Point CenterPoint(Rectangle rectangle) {
			return new Point((rectangle.Right + rectangle.Left) / 2, (rectangle.Bottom + rectangle.Top) / 2);
		}
		public static PointF CenterPoint(RectangleF rectangle) {
			return new PointF((rectangle.Right + rectangle.Left) / 2, (rectangle.Bottom + rectangle.Top) / 2);
		}
		public static Rectangle BoundingRectangle(Rectangle bounds, Matrix transform) {
			Point pt0 = transform.TransformPoint(new Point(bounds.Right, bounds.Bottom));
			Point pt1 = transform.TransformPoint(new Point(bounds.Right, bounds.Top));
			Point pt2 = transform.TransformPoint(new Point(bounds.Left, bounds.Bottom));
			Point pt3 = transform.TransformPoint(bounds.Location);
			return RectangleUtils.BoundingRectangle(pt0, pt1, pt2, pt3);
		}
		public static RectangleF BoundingRectangle(RectangleF bounds, Matrix transform) {
			PointF[] points = new PointF[] { new PointF(bounds.Right, bounds.Bottom), new PointF(bounds.Right, bounds.Top), new PointF(bounds.Left, bounds.Bottom), bounds.Location };
			transform.TransformPoints(points);
			return RectangleUtils.BoundingRectangle(points[0], points[1], points[2], points[3]);
		}
		public static Rectangle BoundingRectangle(params Point[] points) {
			int count = points.Length;
			if (count == 0)
				return Rectangle.Empty;
			int minX = points[0].X;
			int maxX = minX;
			int minY = points[0].Y;
			int maxY = minY;
			for (int i = 1; i < count; i++) {
				minX = Math.Min(points[i].X, minX);
				minY = Math.Min(points[i].Y, minY);
				maxX = Math.Max(points[i].X, maxX);
				maxY = Math.Max(points[i].Y, maxY);
			}
			return new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}
		public static RectangleF BoundingRectangle(params PointF[] points) {
			int count = points.Length;
			if (count == 0)
				return RectangleF.Empty;
			float minX = points[0].X;
			float maxX = minX;
			float minY = points[0].Y;
			float maxY = minY;
			for (int i = 1; i < count; i++) {
				minX = Math.Min(points[i].X, minX);
				minY = Math.Min(points[i].Y, minY);
				maxX = Math.Max(points[i].X, maxX);
				maxY = Math.Max(points[i].Y, maxY);
			}
			return new RectangleF(minX, minY, maxX - minX, maxY - minY);
		}
	}
	#endregion
}
