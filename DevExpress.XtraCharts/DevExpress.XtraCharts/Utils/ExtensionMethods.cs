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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class GRealPoint2DExtensions {
		public static PointF ToPointF(this GRealPoint2D point) {
			return new PointF((float)point.X, (float)point.Y);
		}
		public static Point ToPoint(this GRealPoint2D point) {
			return new Point(MathUtils.StrongRound(point.X), MathUtils.StrongRound(point.Y));
		}
		public static DiagramPoint ToDiagramPoint(this GRealPoint2D point) {
			return new DiagramPoint(point.X, point.Y);
		}
	}
	public static class RectangleExtensions {
		public static bool AreWidthAndHeightPositive(this Rectangle rectangle) {
			return (rectangle.Width > 0 && rectangle.Height > 0);
		}
		public static Point LeftTop(this Rectangle rect) {
			return rect.Location;
		}
		public static Point LeftBottom(this Rectangle rect) {
			return new Point(rect.X, rect.Y + rect.Height);
		}
		public static Point RightBottom(this Rectangle rect) {
			return new Point(rect.X + rect.Width, rect.Y + rect.Height);
		}
		public static Point RightTop(this Rectangle rect) {
			return new Point(rect.X + rect.Width, rect.Y);
		}
		public static Rectangle GetTheSameRectPlacedInCenterOf(this Rectangle thisRectangle, Rectangle otherRectangle) {
			Rectangle result = new Rectangle();
			int dx = (otherRectangle.Width - thisRectangle.Width) / 2;
			int dy = (otherRectangle.Height - thisRectangle.Height) / 2;
			result.Size = thisRectangle.Size;
			result.X = otherRectangle.X + dx;
			result.Y = otherRectangle.Y + dy;
			return result;
		}
		public static bool IsInto(this Rectangle thisRectangle, Rectangle otherRectangle) {
			if (Rectangle.Intersect(thisRectangle, otherRectangle) == thisRectangle)
				return true;
			else
				return false;
		}
		public static GRealRect2D ToGRealRect2D(this Rectangle rect) {
			return new GRealRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
		}
	}
	public static class RectangleFExtensions {
		public static bool AreWidthAndHeightPositive(this RectangleF thisRect) {
			return (thisRect.Width > 0 && thisRect.Height > 0);
		}
		public static PointF LeftTop(this RectangleF thisRect) {
			return thisRect.Location;
		}
		public static PointF LeftBottom(this RectangleF thisRect) {
			return new PointF(thisRect.X, thisRect.Y + thisRect.Height);
		}
		public static PointF RightBottom(this RectangleF thisRect) {
			return new PointF(thisRect.X + thisRect.Width, thisRect.Y + thisRect.Height);
		}
		public static PointF RightTop(this RectangleF thisRect) {
			return new PointF(thisRect.X + thisRect.Width, thisRect.Y);
		}
		public static bool IsInto(this RectangleF thisRectangle, RectangleF otherRectangle) {
			if (RectangleF.Intersect(thisRectangle, otherRectangle) == thisRectangle)
				return true;
			else
				return false;
		}
		public static GRealRect2D ToGRealRect2D(this RectangleF rect) {
			return new GRealRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
		}
	}
	public static class PointFExtensions {
		public static PointF Offset(this PointF pointF, float x, float y) {
			return new PointF(pointF.X + x, pointF.Y + y);
		}
	}
	public static class GRect2DExtentions {
		public static Rectangle ToRectangle(this GRect2D thisRect2D) {
			return new Rectangle(thisRect2D.Left, thisRect2D.Top, thisRect2D.Width, thisRect2D.Height);
		}
	}
}
