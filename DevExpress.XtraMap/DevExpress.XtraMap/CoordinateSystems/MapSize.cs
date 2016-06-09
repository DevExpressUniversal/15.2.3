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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Map.Localization;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using DevExpress.Map;
using System.Diagnostics;
namespace DevExpress.XtraMap {
	[Serializable, StructLayout(LayoutKind.Sequential)] 
	public struct MapSize {
		public static readonly MapSize Empty = new MapSize();
		public static bool operator == (MapSize size1, MapSize size2) {
			return size1.Width == size2.Width && size1.Height == size2.Height;
		}
		public static bool operator != (MapSize size1, MapSize size2) {
			return !(size1 == size2);
		}
		public static bool Equals(MapSize size1, MapSize size2) {
			return size1.width.Equals(size2.width) && size1.height.Equals(size2.height);
		}
		double width;
		double height;
		public MapSize(double width, double height) {
			this.width = width;
			this.height = height;
		}
		public double Width { get { return width; } set { width = value; } }
		public double Height { get { return height; } set { height = value; } }
		public bool IsEmpty { get { return width == 0f && height == 0f; } }
		public override bool Equals(object obj) {
			if (obj != null && obj is MapSize)
				return MapSize.Equals(this, (MapSize)obj);
			return false;
		}
		public override int GetHashCode() {
			return height.GetHashCode() ^ width.GetHashCode();
		}
	}
	[Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(MapPointConverter))]
	public struct MapPoint {
		public static readonly MapPoint Empty = new MapPoint();
		public static bool operator ==(MapPoint Point1, MapPoint Point2) {
			return Point1.X == Point2.X && Point1.Y == Point2.Y;
		}
		public static bool operator !=(MapPoint Point1, MapPoint Point2) {
			return !(Point1 == Point2);
		}
		public static bool Equals(MapPoint Point1, MapPoint Point2) {
			return Point1.x.Equals(Point2.x) && Point1.y.Equals(Point2.y);
		}
		public static MapPoint operator -(MapPoint p, float v) {
			return new MapPoint(p.X - v, p.Y - v);
		}
		public static MapPoint operator +(MapPoint p, float v) {
			return new MapPoint(p.X + v, p.Y + v);
		}
		public static MapPoint operator *(MapPoint p, double v) {
			return new MapPoint(p.X * v, p.Y * v);
		}
		public static MapPoint Parse(string source) {
			double[] parsedDoubles = PointParser.Parse(source, MapLocalizer.GetString(MapStringId.MsgIncorrectStringFormat));
			return new MapPoint(parsedDoubles[0], parsedDoubles[1]);
		}
		double x;
		double y;
		public MapPoint(double x, double y) {
			this.x = x;
			this.y = y;
		}
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public bool IsEmpty { get { return x == 0f && y == 0f; } }
		public override bool Equals(object obj) {
			if (obj != null && obj is MapPoint)
				return MapPoint.Equals(this, (MapPoint)obj);
			return false;
		}
		public override int GetHashCode() {
			return y.GetHashCode() ^ x.GetHashCode();
		}
		public Point ToPoint() {
			return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
		}
		public PointF ToPointF() {
			if (double.IsInfinity(x) || double.IsInfinity(y))
				return new PointF(float.NegativeInfinity, float.NegativeInfinity);
			return new PointF((float)x, (float)y);
		}
		public override string ToString() {
			return this.ToString(CultureInfo.CurrentCulture);
		}
		public string ToString(IFormatProvider provider) {
			NumberFormatInfo formatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			string listSeparator = (formatInfo == null || formatInfo.NumberDecimalSeparator != ",") ? "," : ";";
			return this.x.ToString(provider) + listSeparator + " " + this.y.ToString(provider);
		}
	}
#if DEBUGTEST
	[DebuggerDisplay("L:{left}, T:{top}, R:{right}, B:{bottom}, W:{width}, H:{height}")]
#endif
	public struct MapRect {
		public static readonly MapRect Empty = new MapRect(0, 0, 0, 0);
		public static readonly MapRect Invalid = new MapRect(double.NaN, double.NaN, double.NaN, double.NaN);
		public static MapRect FromLTRB(double left, double top, double right, double bottom) {
			return new MapRect(left, top, right - left, bottom - top);
		}
		public static MapRect Union(MapRect rect1, MapRect rect2) {
			if(!rect1.IsValid)
				return rect2;
			if(!rect2.IsValid)
				return rect1;
			double left = Math.Min(rect1.Left, rect2.Left);
			double bottom = Math.Max(rect1.Bottom, rect2.Bottom);
			double right = Math.Max(rect1.Right, rect2.Right);
			double top = Math.Min(rect1.Top, rect2.Top);
			return MapRect.FromLTRB(left, top, right, bottom);
		}
		public static bool IsIntersected(MapRect rect1, MapRect rect2) {
			return rect1.left < rect2.right && rect1.right > rect2.left && rect1.top < rect2.bottom && rect1.bottom > rect2.top;
		}
		public static MapRect Intersect(MapRect rect1, MapRect rect2) {
			if(!IsIntersected(rect1, rect2))
				return Empty;
			double left = Math.Max(rect1.left, rect2.Left);
			double right = Math.Min(rect1.right, rect2.right);
			double top = Math.Max(rect1.top, rect2.top);
			double bottom = Math.Min(rect1.bottom, rect2.bottom);
			return new MapRect(left, top, right - left, bottom - top);
		}
		public static MapRect Inflate(MapRect rect, double dx, double dy) {
			MapRect result = rect;
			result.Inflate(dx, dy);
			return result;
		}
		public static bool operator ==(MapRect rect1, MapRect rect2) {
			return rect1.Left == rect2.Left && rect1.Top == rect2.Top &&
				 rect1.width == rect2.width && rect1.height == rect2.height;
		}
		public static bool operator !=(MapRect rect1, MapRect rect2) {
			return !(rect1 == rect2);
		}
		public static MapRect operator *(MapRect mapRect, double v) {
			return MapRect.FromLTRB(mapRect.Left * v, mapRect.Top * v, mapRect.Right * v, mapRect.Bottom * v);
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
		public bool IsEmpty { get { return Equals(Empty); } }
		public bool IsValid { get { return !double.IsNaN(left) || !double.IsNaN(top) || !double.IsNaN(right) || !double.IsNaN(bottom); } }
		public MapRect(double left, double top, double width, double height) {
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
		public void Offset(double dx, double dy) {
			left = left + dx;
			top = top + dy;
			CalcRightBottom();
		}
		public void Inflate(double dx, double dy) {
			Offset(-dx, -dy);
			width = width + dx * 2;
			if(width < 0)
				width = 0;
			height = height + dy * 2;
			if(height < 0)
				height = 0;
			CalcRightBottom();
		}
		public bool Contains(MapPoint point) {
			if(point.X <= left || point.X >= right)
				return false;
			if(point.Y <= top || point.Y >= bottom)
				return false;
			return true;
		}
		public bool Contains(MapRect rect) {
			return Contains(new MapPoint(rect.left, rect.top)) && Contains(new MapPoint(rect.right, rect.bottom));
		}
		public override bool Equals(object obj) {
			return (obj is MapRect) && this == (MapRect)obj;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public Rectangle ToRectangle() {
			return new Rectangle(Convert.ToInt32(left), Convert.ToInt32(top), Convert.ToInt32(width), Convert.ToInt32(height));
		}
		public RectangleF ToRectangleF() {
			return new RectangleF(Convert.ToSingle(left), Convert.ToSingle(top), Convert.ToSingle(width), Convert.ToSingle(height));
		}
	}
}
