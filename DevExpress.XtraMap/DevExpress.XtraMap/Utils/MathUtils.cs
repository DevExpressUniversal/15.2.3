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
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;
namespace DevExpress.XtraMap.Native {
	#region RectUtils
	public sealed class RectUtils {
		RectUtils() {
		}
		public static Rectangle GetTopSideRect(Rectangle r, int size) {
			return new Rectangle(r.X, r.Y, r.Width, size);
		}
		public static Rectangle GetBottomSideRect(Rectangle r, int size) {
			return new Rectangle(r.X, r.Bottom - size, r.Width, size);
		}
		public static Rectangle GetLeftSideRect(Rectangle r, int size) {
			return new Rectangle(r.X, r.Y, size, r.Height);
		}
		public static Rectangle GetRightSideRect(Rectangle r, int size) {
			return new Rectangle(r.Right - size, r.Y, size, r.Height);
		}
		public static Rectangle SetLeft(Rectangle r, int value) {
			if(value > r.Right)
				return Rectangle.Empty;
			return Rectangle.FromLTRB(value, r.Top, r.Right, r.Bottom);
		}
		public static Rectangle SetRight(Rectangle r, int value) {
			if(value < r.Left)
				return Rectangle.Empty;
			return Rectangle.FromLTRB(r.Left, r.Top, value, r.Bottom);
		}
		public static Point GetCenter(Rectangle rect) {
			return new Point(GetHorizontalCenter(rect), GetVerticalCenter(rect));
		}
		public static MapPoint GetCenterPoint(Rectangle rect) {
			Point pt = GetCenter(rect);
			return new MapPoint(pt.X, pt.Y);
		}
		public static MapUnit GetCenter(MapRect bounds) {
			return new MapUnit((bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);
		}
		public static int GetHorizontalCenter(Rectangle rect) {
			return (rect.Left + rect.Right) / 2;
		}
		public static int GetVerticalCenter(Rectangle rect) {
			return (rect.Top + rect.Bottom) / 2;
		}
		public static Rectangle GetLeftSideRect(Rectangle r) {
			return GetLeftSideRect(r, 1);
		}
		public static Rectangle GetRightSideRect(Rectangle r) {
			return GetRightSideRect(r, 1);
		}
		public static Rectangle GetTopSideRect(Rectangle r) {
			return GetTopSideRect(r, 1);
		}
		public static Rectangle GetBottomSideRect(Rectangle r) {
			return GetBottomSideRect(r, 1);
		}
		public static Rectangle CutFromTop(Rectangle r, int height) {
			r.Y += height;
			r.Height -= height;
			return r;
		}
		public static Rectangle CutFromBottom(Rectangle r, int height) {
			r.Height -= height;
			return r;
		}
		public static Rectangle CutFromLeft(Rectangle r, int width) {
			r.X += width;
			r.Width -= width;
			return r;
		}
		public static Rectangle CutFromRight(Rectangle r, int width) {
			r.Width -= width;
			return r;
		}
		public static bool ContainsX(Rectangle r, Point pt) {
			return ContainsX(r, pt.X);
		}
		public static bool ContainsX(Rectangle r, int x) {
			return (r.Left <= x) && (x <= r.Right);
		}
		public static bool ContainsY(Rectangle r, Point pt) {
			return ContainsY(r, pt.Y);
		}
		public static bool ContainsY(Rectangle r, int y) {
			return (r.Top <= y) && (y <= r.Bottom);
		}
		public static Rectangle ExpandToLeft(Rectangle source, int value) {
			source.X -= value;
			source.Width += value;
			return source;
		}
		public static Rectangle ExpandToRight(Rectangle source, int value) {
			source.Width += value;
			return source;
		}
		public static Rectangle ExpandToTop(Rectangle source, int value) {
			source.Y -= value;
			source.Height += value;
			return source;
		}
		public static Rectangle ExpandToBottom(Rectangle source, int value) {
			source.Height += value;
			return source;
		}
		public static Point GetRightBottom(Rectangle rect) {
			return new Point(rect.Right, rect.Bottom);
		}
		public static Point GetLeftBottom(Rectangle rect) {
			return new Point(rect.Left, rect.Bottom);
		}
		public static Point GetRightTop(Rectangle rect) {
			return new Point(rect.Right, rect.Top);
		}
		public static Rectangle AlignRectangle(Rectangle rect, Rectangle baseRect, ContentAlignment aligment) {
			rect.Location = baseRect.Location;
			switch(aligment) {
				case ContentAlignment.TopCenter:
					rect.X += (baseRect.Width - rect.Width) / 2;
					break;
				case ContentAlignment.TopLeft:
					break;
				case ContentAlignment.TopRight:
					rect.X += baseRect.Width - rect.Width;
					break;
				case ContentAlignment.MiddleLeft:
					rect.Y += (baseRect.Height - rect.Height) / 2;
					break;
				case ContentAlignment.MiddleCenter:
					rect.Offset((baseRect.Width - rect.Width) / 2, (baseRect.Height - rect.Height) / 2);
					break;
				case ContentAlignment.MiddleRight:
					rect.Offset(baseRect.Width - rect.Width, (baseRect.Height - rect.Height) / 2);
					break;
				case ContentAlignment.BottomLeft:
					rect.Y += baseRect.Height - rect.Height;
					break;
				case ContentAlignment.BottomCenter:
					rect.Offset((baseRect.Width - rect.Width) / 2, baseRect.Height - rect.Height);
					break;
				case ContentAlignment.BottomRight:
					rect.Offset(baseRect.Width - rect.Width, baseRect.Height - rect.Height);
					break;
			}
			return rect;
		}
		public static Rectangle AlignInsideRectWithInversion(Rectangle rectangle, Rectangle contentRect, ContentAlignment alignment) {
			switch(alignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter: return new Rectangle(contentRect.Location + (Size)rectangle.Location, rectangle.Size);
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight: return new Rectangle(contentRect.Right - rectangle.Width - rectangle.X, contentRect.Y + rectangle.Y, rectangle.Width, rectangle.Height);
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter: return new Rectangle(contentRect.X + rectangle.X, contentRect.Bottom - rectangle.Height - rectangle.Y, rectangle.Width, rectangle.Height);
				case ContentAlignment.BottomRight: return new Rectangle(contentRect.Right - rectangle.Width - rectangle.X, contentRect.Bottom - rectangle.Height - rectangle.Y, rectangle.Width, rectangle.Height);
			}
			return rectangle;
		}
		public static Rectangle[] SplitHorizontally(Rectangle bounds, int cellCount) {
			if(cellCount <= 0)
				return new Rectangle[] { bounds };
			Rectangle[] cells = new Rectangle[cellCount];
			int offset = bounds.X;
			int height = bounds.Height;
			int columnsAreaWidth = bounds.Width;
			int columnWidth = columnsAreaWidth / cellCount;
			int remainder = columnsAreaWidth - columnWidth * cellCount;
			for(int i = 0; i < cellCount; i++, remainder--) {
				int width = columnWidth + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rectangle(offset, bounds.Y, width, height);
				offset += width;
			}
			return cells;
		}
		public static Rectangle[] SplitVertically(Rectangle bounds, int cellCount) {
			if(cellCount <= 0)
				return new Rectangle[] { bounds };
			Rectangle[] cells = new Rectangle[cellCount];
			int offset = bounds.Y;
			int width = bounds.Width;
			int columnsAreaHeight = bounds.Height;
			int columnHeight = columnsAreaHeight / cellCount;
			int remainder = columnsAreaHeight - columnHeight * cellCount;
			for(int i = 0; i < cellCount; i++, remainder--) {
				int height = columnHeight + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rectangle(bounds.X, offset, width, height);
				offset += height;
			}
			return cells;
		}
		public static Rectangle[] TileVertically(Rectangle bounds, int count) {
			Rectangle[] result = new Rectangle[count];
			int height = bounds.Height;
			for(int i = 0; i < count; i++) {
				result[i] = bounds;
				bounds.Y += height;
			}
			return result;
		}
		public static Rectangle UnionNonEmpty(Rectangle first, Rectangle second) {
			if(first == Rectangle.Empty)
				return second;
			if(second == Rectangle.Empty)
				return first;
			return Rectangle.Union(first, second);
		}
		public static bool IntersectsExcludeBounds(Rectangle first, Rectangle second) {
			Rectangle intersection = Rectangle.Intersect(first, second);
			return (intersection.Height * intersection.Width) != 0;
		}
		public static Rectangle ValidateDimensions(Rectangle rectangle) {
			Rectangle resRect = rectangle;
			if(rectangle.Width < 0) {
				resRect.X = rectangle.X + rectangle.Width;
				resRect.Width *= -1;
			}
			if(rectangle.Height < 0) {
				resRect.Y = rectangle.Y + rectangle.Height;
				resRect.Height *= -1;
			}
			return resRect;
		}
		public static Rectangle ValidateSelectedRectangle(Rectangle rectangle, Rectangle clientRect) {
			Rectangle result = ValidateDimensions(rectangle);
			clientRect.Width -= 1;
			clientRect.Height -= 1;
			result = Rectangle.Intersect(result, clientRect);
			return result;
		}
		public static bool IsBoundsEmpty(Rectangle bounds) {
			return IsBoundsEmpty(new MapRect(bounds.Left, bounds.Top, bounds.Width, bounds.Height));
		}
		public static bool IsBoundsEmpty(MapRect bounds) {
			return bounds.Width == 0 || bounds.Height == 0;
		}
		public static Rectangle ValidateNegative(Rectangle rect) {
			Rectangle result = rect;
			if(rect.X < 0){
				result.X = 0;
				result.Width += rect.X;
			}
			if(rect.Y < 0) {
				result.Y = 0;
				result.Height += rect.Y;
			}
			if(result.Width <= 0 || result.Height <= 0) return Rectangle.Empty;
			return result;
		}
		public static Rectangle Offset(Rectangle rectangle, Point point) {
			rectangle.Offset(point);
			return rectangle;
		}
	}
	#endregion
	public static class ContentAlignmentUtils {
		public static bool TopAligned(ContentAlignment alignment) {
			return alignment.HasFlag(ContentAlignment.TopLeft) || alignment.HasFlag(ContentAlignment.TopCenter) || alignment.HasFlag(ContentAlignment.TopRight);
		}
		public static bool MiddleAligned(ContentAlignment alignment) {
			return alignment.HasFlag(ContentAlignment.MiddleLeft) || alignment.HasFlag(ContentAlignment.MiddleCenter) || alignment.HasFlag(ContentAlignment.MiddleRight);
		}
		public static bool BottomAligned(ContentAlignment alignment) {
			return alignment.HasFlag(ContentAlignment.BottomLeft) || alignment.HasFlag(ContentAlignment.BottomCenter) || alignment.HasFlag(ContentAlignment.BottomRight);
		}
		public static bool LeftAligned(ContentAlignment alignment) {
			return alignment.HasFlag(ContentAlignment.TopLeft) || alignment.HasFlag(ContentAlignment.MiddleLeft) || alignment.HasFlag(ContentAlignment.BottomLeft);
		}
		public static bool CenterAligned(ContentAlignment alignment) {
			return alignment.HasFlag(ContentAlignment.TopCenter) || alignment.HasFlag(ContentAlignment.MiddleCenter) || alignment.HasFlag(ContentAlignment.BottomCenter);
		}
		public static bool RightAligned(ContentAlignment alignment) {
			return alignment.HasFlag(ContentAlignment.TopRight) || alignment.HasFlag(ContentAlignment.MiddleRight) || alignment.HasFlag(ContentAlignment.BottomRight);
		}
	}
}
