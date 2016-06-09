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
	public struct GRect2D {
		public static readonly GRect2D Empty = new GRect2D(0, 0, 0, 0);
		public static bool IsIntersected(GRect2D rect1, GRect2D rect2) {
			return rect1.left < rect2.right && rect1.right > rect2.left && rect1.top < rect2.bottom && rect1.bottom > rect2.top;
		}
		public static GRect2D Intersect(GRect2D rect1, GRect2D rect2) {
			if (!IsIntersected(rect1, rect2))
				return Empty;
			int left = Math.Max(rect1.left, rect2.Left);
			int right = Math.Min(rect1.right, rect2.right);
			int top = Math.Max(rect1.top, rect2.top);
			int bottom = Math.Min(rect1.bottom, rect2.bottom);
			return new GRect2D(left, top, right - left, bottom - top);			
		}
		int left;
		int right;
		int top;
		int bottom;
		int width;
		int height;
		public int Left { get { return left; } }
		public int Right { get { return right; } }
		public int Top { get { return top; } }
		public int Bottom { get { return bottom; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public bool IsEmpty { get { return width == 0 || height == 0; } }
		void CalcRightBottom() {
			right = left + width;
			bottom = top + height;
		}
		public GRect2D(int left, int top, int width, int height) {
			ChartDebug.Assert(width >= 0);
			ChartDebug.Assert(height >= 0);
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;			
			right = left + width;
			bottom = top + height;
		}
		public bool Contains(GRect2D rect) {
			if (IsEmpty)
				return false;
			if (rect.left < left || rect.left > right || rect.right < left || rect.right > right)
				return false;
			if (rect.top < top || rect.top > bottom || rect.bottom < top || rect.bottom > bottom)
				return false;
			return true;
		}
		public bool Contains(GPoint2D point) {
			if (IsEmpty)
				return false;
			if (point.X < left || point.X > right)
				return false;
			if (point.Y < top || point.Y > bottom)
				return false;
			return true;
		}
		public void Offset(int dx, int dy) {
			left = left + dx;
			top = top + dy;
			CalcRightBottom();
		}
		public void Inflate(int dx, int dy) {
			Offset(-dx, -dy);
			width = width + dx * 2;
			if (width < 0)
				width = 0;
			height = height + dy * 2;
			if (height < 0)
				height = 0;
			CalcRightBottom();
		}
		public bool AreWidthAndHeightPositive() {
			return (Width > 0 && Height > 0);
		}
	}
}
