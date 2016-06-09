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
using System.Text;
using System.Drawing;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Compatibility.System.Drawing;
#if SL || DXPORTABLE
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class RectHelper {
		public static Rectangle CeilingVertical(RectangleF value) {
			return new Rectangle((int)Math.Round((double)value.X), (int)Math.Ceiling((double)value.Y), (int)Math.Round((double)value.Width), (int)Math.Ceiling((double)value.Height));
		}
		public static Rectangle[] EmptyArray {
			get { return new Rectangle[] { }; }
		}
		public static bool RectangleFContains(RectangleF rect1, RectangleF rect2, int digits) {
			double left1 = Math.Round(rect1.Left, digits);
			double top1 = Math.Round(rect1.Top, digits);
			double right1 = Math.Round(rect1.Right, digits);
			double bottom1 = Math.Round(rect1.Bottom, digits);
			double left2 = Math.Round(rect2.Left, digits);
			double top2 = Math.Round(rect2.Top, digits);
			double right2 = Math.Round(rect2.Right, digits);
			double bottom2 = Math.Round(rect2.Bottom, digits);
			return left1 <= left2 && right2 <= right1 && top1 <= top2 && bottom2 <= bottom1;
		}
		public static bool RectangleFContains(RectangleF rect, PointF pt, int digits) {
			double left = Math.Round(rect.Left, digits);
			double top = Math.Round(rect.Top, digits);
			double right = Math.Round(rect.Right, digits);
			double bottom = Math.Round(rect.Bottom, digits);
			double x = Math.Round(pt.X, digits);
			double y = Math.Round(pt.Y, digits);
			return left <= x && x <= right && top <= y && y <= bottom;
		}
		public static bool RectangleFIntersects(RectangleF rect1, RectangleF rect2, int digits) {
			double left1 = Math.Round(rect1.Left, digits);
			double top1 = Math.Round(rect1.Top, digits);
			double right1 = Math.Round(rect1.Right, digits);
			double bottom1 = Math.Round(rect1.Bottom, digits);
			double left2 = Math.Round(rect2.Left, digits);
			double top2 = Math.Round(rect2.Top, digits);
			double right2 = Math.Round(rect2.Right, digits);
			double bottom2 = Math.Round(rect2.Bottom, digits);
			return left2 < right1 && left1 < right2 && top2 < bottom1 && top1 < bottom2;
		}
		public static bool RectangleFEquals(RectangleF rect1, RectangleF rect2, double epsilon) {
			return ComparingUtils.CompareDoubleArrays(new double[] { rect1.Left, rect1.Top, rect1.Right, rect1.Bottom },
				new double[] { rect2.Left, rect2.Top, rect2.Right, rect2.Bottom },
				epsilon) == 0;
		}
		public static bool RectangleFIsEmptySize(RectangleF rect, double epsilon) {
			return ComparingUtils.CompareDoubleArrays(new double[] { rect.Width, rect.Height },
				new double[] { 0, 0 },
				epsilon) == 0;
		}
		public static bool RectangleFIsEmpty(RectangleF rect, double epsilon) {
			return ComparingUtils.CompareDoubleArrays(new double[] { rect.X, rect.Y, rect.Width, rect.Height },
				new double[] { 0, 0, 0, 0 },
				epsilon) == 0;
		}
		public static RectangleF SnapRectangle(RectangleF rect, float dpi, float snapDpi) {
			RectangleF rect1 = GraphicsUnitConverter.Convert(rect, dpi, snapDpi);
			rect1 = RectangleF.FromLTRB((float)Math.Floor(rect1.Left), (float)Math.Floor(rect1.Top), (float)Math.Ceiling(rect1.Right), (float)Math.Ceiling(rect1.Bottom));
			return GraphicsUnitConverter.Convert(rect1, snapDpi, dpi);
		}
		public static RectangleF SnapRectangleHorizontal(RectangleF rect, float dpi, float snapDpi) {
			float left = GraphicsUnitConverter.Convert(rect.Left, dpi, snapDpi);
			float right = GraphicsUnitConverter.Convert(rect.Right, dpi, snapDpi);
			left = GraphicsUnitConverter.Convert((float)Math.Floor(left), snapDpi, dpi);
			right = GraphicsUnitConverter.Convert((float)Math.Ceiling(right), snapDpi, dpi);
			return RectangleF.FromLTRB(left, rect.Top, right, rect.Bottom);
		}
		public static Rectangle InflateRectFToInteger(RectangleF rect) {
			return Rectangle.FromLTRB((int)Math.Floor(rect.Left), (int)Math.Floor(rect.Top), (int)Math.Ceiling(rect.Right), (int)Math.Ceiling(rect.Bottom));
		}
		public static RectangleF InflateRect(RectangleF rect, MarginsF m) {
			return InflateRect(rect, m.Left, m.Top, m.Right, m.Bottom);
		}
		public static Rectangle InflateRect(Rectangle rect, int left, int top, int right, int bottom) {
			return Rectangle.FromLTRB(rect.Left - left, rect.Top - top, rect.Right + right, rect.Bottom + bottom);
		}
		public static RectangleF InflateRect(RectangleF rect, float left, float top, float right, float bottom) {
			return RectangleF.FromLTRB(rect.Left - left, rect.Top - top, rect.Right + right, rect.Bottom + bottom);
		}
		public static RectangleF InflateRect(RectangleF rect, float left, float top, float right, float bottom, BorderSide borders) {
			if ((borders & BorderSide.Left) == 0)
				left = 0;
			if ((borders & BorderSide.Top) == 0)
				top = 0;
			if ((borders & BorderSide.Right) == 0)
				right = 0;
			if ((borders & BorderSide.Bottom) == 0)
				bottom = 0;
			return RectangleF.FromLTRB(rect.Left - left, rect.Top - top, rect.Right + right, rect.Bottom + bottom);
		}
		public static RectangleF InflateRect(RectangleF rect, float width, BorderSide borders) {
			return InflateRect(rect, width, width, width, width, borders);
		}
		public static RectangleF DeflateRect(RectangleF rect, MarginsF m) {
			return DeflateRect(rect, m.Left, m.Top, m.Right, m.Bottom);
		}
		public static RectangleF DeflateRect(RectangleF rect, float left, float top, float right, float bottom) {
			return RectangleF.FromLTRB(rect.Left + left, rect.Top + top, rect.Right - right, rect.Bottom - bottom);
		}
		public static Rectangle DeflateRect(Rectangle rect, int left, int top, int right, int bottom) {
			return Rectangle.FromLTRB(rect.Left + left, rect.Top + top, rect.Right - right, rect.Bottom - bottom);
		}
		public static Rectangle ValidateRect(Rectangle r) {
			return Rectangle.FromLTRB(Math.Min(r.Left, r.Right), Math.Min(r.Top, r.Bottom), Math.Max(r.Left, r.Right), Math.Max(r.Top, r.Bottom));
		}
		public static RectangleF ValidateRectF(RectangleF r) {
			return RectangleF.FromLTRB(Math.Min(r.Left, r.Right), Math.Min(r.Top, r.Bottom), Math.Max(r.Left, r.Right), Math.Max(r.Top, r.Bottom));
		}
		public static bool IsEqual(Rectangle[] arr1, Rectangle[] arr2) {
			if (arr1.Length != arr2.Length)
				return false;
			for (int i = 0; i < arr1.Length; i++) {
				if (!Comparer.Equals(arr1[i], arr2[i]))
					return false;
			}
			return true;
		}
		public static Rectangle RectangleFromPoints(Point pt1, Point pt2) {
			return ValidateRect(Rectangle.FromLTRB(pt1.X, pt1.Y, pt2.X, pt2.Y));
		}
		public static RectangleF RectangleFFromPoints(PointF pt1, PointF pt2) {
			return ValidateRectF(RectangleF.FromLTRB(pt1.X, pt1.Y, pt2.X, pt2.Y));
		}
		public static Rectangle OffsetRect(Rectangle rect, Point pos) {
			rect.Offset(pos);
			return rect;
		}
		public static Rectangle OffsetRect(Rectangle rect, int x, int y) {
			rect.Offset(x, y);
			return rect;
		}
		public static RectangleF OffsetRectF(RectangleF rect, PointF pos) {
			rect.Offset(pos);
			return rect;
		}
		public static RectangleF OffsetRectF(RectangleF rect, float x, float y) {
			rect.Offset(x, y);
			return rect;
		}
		public static Point CenterOf(Rectangle rect) {
			int x = Convert.ToInt32((rect.Left + rect.Right) / 2);
			int y = Convert.ToInt32((rect.Bottom + rect.Top) / 2);
			return new Point(x, y);
		}
		public static PointF CenterOf(RectangleF rect) {
			float x = (rect.Left + rect.Right) / 2;
			float y = (rect.Bottom + rect.Top) / 2;
			return new PointF(x, y);
		}
		public static Rectangle AlignRectangle(Rectangle rect, Rectangle baseRect, ContentAlignment aligment) {
			return Rectangle.Round(AlignRectangleF(rect, baseRect, aligment));
		}
		public static RectangleF AlignRectangleF(RectangleF rect, RectangleF baseRect, ContentAlignment aligment) {
			rect.Location = baseRect.Location;
			switch (aligment) {
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
		public static PointF TopLeft(RectangleF rect) {
			return rect.Location;
		}
		public static PointF TopRight(RectangleF rect) {
			return new PointF(rect.Right, rect.Top);
		}
		public static PointF BottomLeft(RectangleF rect) {
			return new PointF(rect.Left, rect.Bottom);
		}
		public static PointF BottomRight(RectangleF rect) {
			return new PointF(rect.Right, rect.Bottom);
		}
	}
}
