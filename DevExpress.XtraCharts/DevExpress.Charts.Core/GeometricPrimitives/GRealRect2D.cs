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
namespace DevExpress.Charts.Native {
	public struct GRealRect2D {
		public static readonly GRealRect2D Empty = new GRealRect2D(0, 0, 0, 0);
		public static bool IsIntersected(GRealRect2D rect1, GRealRect2D rect2)
		{
			return rect1.left < rect2.right && rect1.right > rect2.left && rect1.top < rect2.bottom && rect1.bottom > rect2.top;
		}
		public static GRealRect2D Intersect(GRealRect2D rect1, GRealRect2D rect2)
		{
			if (!IsIntersected(rect1, rect2))
				return Empty;
			double left = Math.Max(rect1.left, rect2.Left);
			double right = Math.Min(rect1.right, rect2.right);
			double top = Math.Max(rect1.top, rect2.top);
			double bottom = Math.Min(rect1.bottom, rect2.bottom);
			return new GRealRect2D(left, top, right - left, bottom - top);
		}
		public static GRealRect2D Inflate(GRealRect2D rect, double dx, double dy) {
			GRealRect2D result = rect;
			result.Inflate(dx, dy);
			return result;
		}
		double left;
		double right;
		double top;
		double bottom;
		double width;
		double height;
		public double Left { get { return left; } }
		public double Right { get { return right; } }
		public double Top { get { return top; } }
		public double Bottom { get { return bottom; } }
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public GRealPoint2D Center { get { return new GRealPoint2D(0.5 * (left + Right), 0.5 * (bottom + top)); } }
		public bool IsEmpty { get { return Equals(Empty); } }
		public GRealSize2D Size { get { return new GRealSize2D(width, height); } }
		public GRealRect2D(GRealPoint2D p1, GRealPoint2D p2) {
			this.left = Math.Min(p1.X, p2.X);
			this.top = Math.Min(p1.Y, p2.Y);
			this.width = Math.Abs(p1.X - p2.X);
			this.height = Math.Abs(p1.Y - p2.Y);
			right = left + width;
			bottom = top + height;
		}
		public GRealRect2D(double left, double top, double width, double height) {
			ChartDebug.Assert(width >= 0);
			ChartDebug.Assert(height >= 0);
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
			right = left + width;
			bottom = top + height;
		}
		void CalcRightBottom() {
			right = left + width;
			bottom = top + height;
		}
		public void Offset(double dx, double dy){
			left = left + dx;
			top = top + dy;
			CalcRightBottom();
		}
		public void Inflate(double dx, double dy) {
			Offset(-dx, -dy);
			width = width + dx * 2;
			if (width < 0)
				width = 0;
			height = height + dy * 2;
			if (height < 0)
				height = 0;
			CalcRightBottom();
		}
		public bool Contains(GRealPoint2D point) {
			if (point.X < left || point.X > right)
				return false;
			if (point.Y < top || point.Y > bottom)
				return false;
			return true;
		}
		public bool ContainsIncludeBounds(GRealPoint2D point) {
			if (point.X <= left || point.X >= right)
				return false;
			if (point.Y <= top || point.Y >= bottom)
				return false;
			return true;
		}
		public bool Contains(GRealRect2D rect) {
			return Contains(new GRealPoint2D(rect.left, rect.top)) && Contains(new GRealPoint2D(rect.right, rect.bottom)); 
		}
		public override string ToString() {
			return string.Format("Location: {0}, {1}; Size: {2}, {3}", left, top, width, height);
		}
	}
}
