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

using DevExpress.Data.Svg;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System;
namespace DevExpress.Map {
	[Serializable]
	public abstract class CoordPoint {
		public static bool operator ==(CoordPoint point1, CoordPoint point2) {
			object obj1 = (object)point1;
			object obj2 = (object)point2;
			if (obj1 == null && obj2 == null)
				return true;
			if ((obj1 == null && obj2 != null) || (obj1 != null && obj2 == null))
				return false;
			if (!(point1 is CoordPoint) || !(point2 is CoordPoint))
				return false;
			if (double.IsNaN(point1.XCoord) && double.IsNaN(point2.XCoord) && double.IsNaN(point1.YCoord) && double.IsNaN(point2.YCoord))
				return true;
			return point1.XCoord == point2.XCoord && point1.YCoord == point2.YCoord;
		}
		public static bool operator !=(CoordPoint point1, CoordPoint point2) {
			return !(point1 == point2);
		}
		double xCoord = double.NaN;
		double yCoord = double.NaN;
		protected internal double XCoord { get { return xCoord; } set { xCoord = value; } }
		protected internal double YCoord { get { return yCoord; } set { yCoord = value; } }
		protected CoordPoint(double x, double y) {
			XCoord = x;
			YCoord = y;
		}
		protected internal abstract CoordPoint CreateNormalized();
		public abstract CoordPoint Offset(double offsetX, double offsetY);
		public virtual double GetX() {
			return XCoord;
		}
		public virtual double GetY() {
			return YCoord;
		}
		public override bool Equals(object o) {
			CoordPoint pt = o as CoordPoint;
			if (o != null && pt != null)
				return pt == this;
			return false;
		}
		public override int GetHashCode() {
			return XCoord.GetHashCode() ^ YCoord.GetHashCode();
		}
		protected string GetCoordinateSeparator(IFormatProvider provider) {
			NumberFormatInfo formatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			return formatInfo == null || formatInfo.NumberDecimalSeparator != "," ? "," : ";";
		}
		public override string ToString() {
			return ToString(CultureInfo.CurrentCulture);
		}
		public virtual string ToString(IFormatProvider provider) {
			return string.Format("{0}{1} {2}", XCoord.ToString(provider), GetCoordinateSeparator(provider), YCoord.ToString(provider));
		}
	}
}
namespace DevExpress.Map.Native {
	public struct CoordVector {
		public static bool operator ==(CoordVector vector1, CoordVector vector2) {
			return vector1.X == vector2.X && vector1.Y == vector2.Y;
		}
		public static bool operator !=(CoordVector vector1, CoordVector vector2) {
			return !(vector1 == vector2);
		}
		public static bool Equals(CoordVector vector1, CoordVector vector2) {
			return vector1.x.Equals(vector2.x) && vector1.y.Equals(vector2.y);
		}
		double x;
		double y;
		public CoordVector(double x, double y) {
			this.x = x;
			this.y = y;
		}
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public bool IsEmpty { get { return x == 0f && y == 0f; } }
		public override bool Equals(object obj) {
			if(obj != null && obj is CoordVector)
				return CoordVector.Equals(this, (CoordVector)obj);
			return false;
		}
		public override int GetHashCode() {
			return y.GetHashCode() ^ x.GetHashCode();
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
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct CoordBounds {
		public static bool operator ==(CoordBounds bounds1, CoordBounds bounds2) {
			if (bounds1.IsEmpty)
				return bounds2.IsEmpty;
			return bounds1.Y1 == bounds2.Y1 &&
				bounds1.X1 == bounds2.X1 &&
				bounds1.Y2 == bounds2.Y2 &&
				bounds1.X2 == bounds2.X2;
		}
		public static bool operator !=(CoordBounds bounds1, CoordBounds bounds2) {
			return !(bounds1 == bounds2);
		}
		public static CoordBounds Union(CoordBounds rect1, CoordBounds rect2) {
			if (rect1.IsEmpty)
				return rect2;
			if (rect2.IsEmpty)
				return rect1;
			double x1 = Math.Min(rect1.x1, rect2.x1);
			double y1 = Math.Max(rect1.y1, rect2.y1);
			double x2 = Math.Max(rect1.x2, rect2.x2);
			double y2 = Math.Min(rect1.y2, rect2.y2);
			return new CoordBounds(x1, y1, x2, y2);
		}
		public static CoordBounds Union(CoordBounds bounds, double x, double y) {
			CoordBounds additionalBounds = new CoordBounds(x, y, x, y);
			if (bounds.IsEmpty)
				return additionalBounds;
			return Union(bounds, additionalBounds);
		}
		readonly static CoordBounds zero = new CoordBounds(0, 0, 0, 0);
		readonly static CoordBounds empty = new CoordBounds(double.NaN, double.NaN, double.NaN, double.NaN);
		public static CoordBounds Zero { get { return zero; } }
		public static CoordBounds Empty { get { return empty; } }
		public bool IsEmpty { get { return double.IsNaN(x1) && double.IsNaN(y1) && double.IsNaN(x2) && double.IsNaN(y2); } }
		double x1;
		double y1;
		double x2;
		double y2;
		public double X1 { get { return x1; } }
		public double X2 { get { return x2; } }
		public double Y1 { get { return y1; } }
		public double Y2 { get { return y2; } }
		public double Width { get { return X2 - X1; } }
		public double Height { get { return Y1 - Y2; } }
		public CoordBounds(CoordPoint point1, CoordPoint point2) :
			this(point1.XCoord, point1.YCoord, point2.XCoord, point2.YCoord) {
		}
		public CoordBounds(double x1, double y1, double x2, double y2) {
			this.x1 = Math.Min(x1, x2);
			this.y1 = Math.Max(y1, y2);
			this.x2 = Math.Max(x1, x2);
			this.y2 = Math.Min(y1, y2);
		}
		public CoordBounds(SvgRect svgRect) :
			this(svgRect.X1, svgRect.Y1, svgRect.X2, svgRect.Y2) {
		}
		public void Correct() {
			double offset = 0.5 * (Width - Height);
			if (Width > Height) {
				y1 += offset;
				y2 -= offset;
			}
			else {
				x1 += offset;
				x2 -= offset;
			}
		}
		public bool IsPointInBounds(CoordPoint point) {
			if (IsEmpty) 
				return false;
			double x = point.GetX();
			double y = point.GetY();
			return x >= x1 && x <= x2 && y <= y1 && y >= y2;
		}
		public override bool Equals(object o) {
			if (o != null && o is CoordBounds)
				return this == (CoordBounds)o;
			return false;
		}
		public override int GetHashCode() {
			return y1.GetHashCode() ^ x1.GetHashCode() ^ y2.GetHashCode() ^ x2.GetHashCode();
		}
		public override string ToString() {
			return string.Format("({0}; {1}) - ({2}; {3})", x1, y1, x2, y2);
		}
	}
}
