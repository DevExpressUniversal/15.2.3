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
using System.Runtime.InteropServices;
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Resources;
using System.Drawing;
using System.Globalization;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Base {
	[TypeConverter(typeof(TextSpacingConverter))]
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct TextSpacing {
		int leftCore;
		int rightCore;
		int topCore;
		int bottomCore;
		public TextSpacing(TextSpacing s) {
			leftCore = s.Left;
			topCore = s.Top;
			rightCore = s.Right;
			bottomCore = s.Bottom;
		}
		public TextSpacing(int left, int top, int right, int bottom) {
			leftCore = left;
			topCore = top;
			rightCore = right;
			bottomCore = bottom;
		}
		public int Left {
			get { return leftCore; }
			set { leftCore = value; }
		}
		public int Top {
			get { return topCore; }
			set { topCore = value; }
		}
		public int Right {
			get { return rightCore; }
			set { rightCore = value; }
		}
		public int Bottom {
			get { return bottomCore; }
			set { bottomCore = value; }
		}
		public int Height {
			get { return Top + Bottom; }
		}
		public int Width {
			get { return Left + Right; }
		}
		public override bool Equals(object obj) {
			if(!(obj is TextSpacing)) return false;
			return this == (TextSpacing)obj;
		}
		public static bool operator ==(TextSpacing s1, TextSpacing s2) {
			return ((((s1.Left == s2.Left) && (s1.Top == s2.Top)) && (s1.Right == s2.Right)) && (s1.Bottom == s2.Bottom));
		}
		public static bool operator !=(TextSpacing s1, TextSpacing s2) {
			return !(s1 == s2);
		}
		public override int GetHashCode() {
			return (((Left ^ ((Top << 13) | (Top >> 0x13))) ^ ((Width << 0x1a) | (Width >> 6))) ^ ((Height << 7) | (Height >> 0x19)));
		}
	}
	public interface IThickness {
		int All { get; set; }
		int Left { get; set; }
		int Top { get; set; }
		int Right { get; set; }
		int Bottom { get; set; }
		int Width { get; }
		int Height { get; }
	}
	[TypeConverter(typeof(ThicknessConverter))]
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct Thickness : IThickness {
		int leftCore;
		int rightCore;
		int topCore;
		int bottomCore;
		public Thickness(IThickness t) {
			leftCore = t.Left;
			topCore = t.Top;
			rightCore = t.Right;
			bottomCore = t.Bottom;
		}
		public Thickness(int all) {
			leftCore = all;
			topCore = all;
			rightCore = all;
			bottomCore = all;
		}
		public Thickness(int left, int top, int right, int bottom) {
			leftCore = left;
			topCore = top;
			rightCore = right;
			bottomCore = bottom;
		}
		[RefreshProperties(RefreshProperties.All)]
		public int All {
			get {
				if(leftCore == bottomCore && leftCore == rightCore && topCore == bottomCore)
					return leftCore;
				return -1;
			}
			set {
				leftCore = value;
				topCore = value;
				rightCore = value;
				bottomCore = value;
			}
		}
		public int Left {
			get { return leftCore; }
			set { leftCore = value; }
		}
		public int Top {
			get { return topCore; }
			set { topCore = value; }
		}
		public int Right {
			get { return rightCore; }
			set { rightCore = value; }
		}
		public int Bottom {
			get { return bottomCore; }
			set { bottomCore = value; }
		}
		[Browsable(false)]
		public int Height {
			get { return Top + Bottom; }
		}
		[Browsable(false)]
		public int Width {
			get { return Left + Right; }
		}
		public override bool Equals(object obj) {
			if(!(obj is IThickness)) return false;
			return this == (IThickness)obj;
		}
		public static bool operator ==(Thickness t1, Thickness t2) {
			return ((((t1.Left == t2.Left) && (t1.Top == t2.Top)) && (t1.Right == t2.Right)) && (t1.Bottom == t2.Bottom));
		}
		public static bool operator !=(Thickness t1, Thickness t2) {
			return !(t1 == t2);
		}
		public static bool operator ==(Thickness t1, IThickness t2) {
			return ((((t1.Left == t2.Left) && (t1.Top == t2.Top)) && (t1.Right == t2.Right)) && (t1.Bottom == t2.Bottom));
		}
		public static bool operator !=(Thickness t1, IThickness t2) {
			return !(t1 == t2);
		}
		public override int GetHashCode() {
			return (((Left ^ ((Top << 13) | (Top >> 0x13))) ^ ((Width << 0x1a) | (Width >> 6))) ^ ((Height << 7) | (Height >> 0x19)));
		}
	}
	[TypeConverter(typeof(FactorF2DConverter))]
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct FactorF2D {
		public static readonly FactorF2D Empty;
		float x, y;
		public FactorF2D(float x, float y) {
			this.x = x;
			this.y = y;
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return ((x == 0f) && (y == 0f)); }
		}
		public float XFactor {
			get { return x; }
			set { x = value; }
		}
		public float YFactor {
			get { return y; }
			set { y = value; }
		}
		public static FactorF2D operator +(FactorF2D f, Size sz) {
			return Add(f, sz);
		}
		public static FactorF2D operator -(FactorF2D f, Size sz) {
			return Subtract(f, sz);
		}
		public static FactorF2D operator +(FactorF2D f, SizeF sz) {
			return Add(f, sz);
		}
		public static FactorF2D operator -(FactorF2D f, SizeF sz) {
			return Subtract(f, sz);
		}
		public static FactorF2D operator +(FactorF2D f1, FactorF2D f2) {
			return Add(f1, f2);
		}
		public static FactorF2D operator -(FactorF2D f1, FactorF2D f2) {
			return Subtract(f1, f2);
		}
		public static bool operator ==(FactorF2D left, FactorF2D right) {
			return ((left.XFactor == right.XFactor) && (left.YFactor == right.YFactor));
		}
		public static bool operator !=(FactorF2D left, FactorF2D right) {
			return !(left == right);
		}
		public static FactorF2D Add(FactorF2D f1, FactorF2D f2) {
			return new FactorF2D(f1.XFactor + f2.XFactor, f1.YFactor + f2.YFactor);
		}
		public static FactorF2D Subtract(FactorF2D f1, FactorF2D f2) {
			return new FactorF2D(f1.XFactor - f2.XFactor, f1.YFactor - f2.YFactor);
		}
		public static FactorF2D Add(FactorF2D pt, Size sz) {
			return new FactorF2D(pt.XFactor + sz.Width, pt.YFactor + sz.Height);
		}
		public static FactorF2D Subtract(FactorF2D pt, Size sz) {
			return new FactorF2D(pt.XFactor - sz.Width, pt.YFactor - sz.Height);
		}
		public static FactorF2D Add(FactorF2D pt, SizeF sz) {
			return new FactorF2D(pt.XFactor + sz.Width, pt.YFactor + sz.Height);
		}
		public static FactorF2D Subtract(FactorF2D pt, SizeF sz) {
			return new FactorF2D(pt.XFactor - sz.Width, pt.YFactor - sz.Height);
		}
		public override bool Equals(object obj) {
			if(!(obj is FactorF2D)) return false;
			FactorF2D p = (FactorF2D)obj;
			return (((p.x == this.x) && (p.y == this.y)) && p.GetType().Equals(base.GetType()));
		}
		public override int GetHashCode() {
			return (int)((uint)x ^ (uint)y);
		}
		public override string ToString() {
			return string.Format(CultureInfo.CurrentCulture, "{{XFactor={0}, YFactor={1}}}", new object[] { x, y });
		}
		public static implicit operator FactorF2D(Point point) {
			return new FactorF2D(point.X, point.Y);
		}
		public static implicit operator FactorF2D(PointF point) {
			return new FactorF2D(point.X, point.Y);
		}
		public static implicit operator FactorF2D(Size size) {
			return new FactorF2D(size.Width, size.Height);
		}
		public static implicit operator FactorF2D(SizeF size) {
			return new FactorF2D(size.Width, size.Height);
		}
		public static implicit operator PointF(FactorF2D pt) {
			return new PointF(pt.x, pt.y);
		}
		public static implicit operator SizeF(FactorF2D pt) {
			return new SizeF(pt.x, pt.y);
		}
		public static explicit operator Point(FactorF2D vector) {
			return Point.Round(vector);
		}
		public static explicit operator Size(FactorF2D vector) {
			return Size.Round(vector);
		}
		static FactorF2D() {
			Empty = new FactorF2D();
		}
	}
	[TypeConverter(typeof(PointF2DConverter))]
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct PointF2D {
		public static readonly PointF2D Empty;
		float x, y;
		public PointF2D(float x, float y) {
			this.x = x;
			this.y = y;
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return ((x == 0f) && (y == 0f)); }
		}
		public float X {
			get { return x; }
			set { x = value; }
		}
		public float Y {
			get { return y; }
			set { y = value; }
		}
		public static PointF2D operator +(PointF2D pt, Size sz) {
			return Add(pt, sz);
		}
		public static PointF2D operator -(PointF2D pt, Size sz) {
			return Subtract(pt, sz);
		}
		public static PointF2D operator +(PointF2D pt, SizeF sz) {
			return Add(pt, sz);
		}
		public static PointF2D operator -(PointF2D pt, SizeF sz) {
			return Subtract(pt, sz);
		}
		public static bool operator ==(PointF2D left, PointF2D right) {
			return ((left.X == right.X) && (left.Y == right.Y));
		}
		public static bool operator !=(PointF2D left, PointF2D right) {
			return !(left == right);
		}
		public static PointF2D Add(PointF2D pt, Size sz) {
			return new PointF2D(pt.X + sz.Width, pt.Y + sz.Height);
		}
		public static PointF2D Subtract(PointF2D pt, Size sz) {
			return new PointF2D(pt.X - sz.Width, pt.Y - sz.Height);
		}
		public static PointF2D Add(PointF2D pt, SizeF sz) {
			return new PointF2D(pt.X + sz.Width, pt.Y + sz.Height);
		}
		public static PointF2D Subtract(PointF2D pt, SizeF sz) {
			return new PointF2D(pt.X - sz.Width, pt.Y - sz.Height);
		}
		public override bool Equals(object obj) {
			if(!(obj is PointF2D)) return false;
			PointF2D p = (PointF2D)obj;
			return (((p.X == this.X) && (p.Y == this.Y)) && p.GetType().Equals(base.GetType()));
		}
		public override int GetHashCode() {
			return (int)((uint)x ^ (uint)y);
		}
		public override string ToString() {
			return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[] { x, y });
		}
		public static implicit operator PointF2D(Point point) {
			return new PointF2D(point.X, point.Y);
		}
		public static implicit operator PointF2D(PointF point) {
			return new PointF2D(point.X, point.Y);
		}
		public static implicit operator PointF2D(Size size) {
			return new PointF2D(size.Width, size.Height);
		}
		public static implicit operator PointF2D(SizeF size) {
			return new PointF2D(size.Width, size.Height);
		}
		public static implicit operator PointF(PointF2D pt) {
			return new PointF(pt.x, pt.Y);
		}
		public static implicit operator SizeF(PointF2D pt) {
			return new SizeF(pt.x, pt.Y);
		}
		public static explicit operator Point(PointF2D vector) {
			return Point.Round(vector);
		}
		public static explicit operator Size(PointF2D vector) {
			return Size.Round(vector);
		}
		static PointF2D() {
			Empty = new PointF2D();
		}
	}
	[TypeConverter(typeof(RectangleF2DConverter))]
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct RectangleF2D {
		public static readonly RectangleF2D Empty;
		private float x;
		private float y;
		private float width;
		private float height;
		public RectangleF2D(float x, float y, float width, float height) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		public RectangleF2D(PointF location, SizeF size) {
			this.x = location.X;
			this.y = location.Y;
			this.width = size.Width;
			this.height = size.Height;
		}
		public RectangleF2D(PointF2D location, SizeF size) {
			this.x = location.X;
			this.y = location.Y;
			this.width = size.Width;
			this.height = size.Height;
		}
		public static RectangleF2D FromLTRB(float left, float top, float right, float bottom) {
			return new RectangleF2D(left, top, right - left, bottom - top);
		}
		[Browsable(false)]
		public PointF2D Location {
			get { return new PointF2D(this.X, this.Y); }
			set {
				this.X = value.X;
				this.Y = value.Y;
			}
		}
		[Browsable(false)]
		public SizeF Size {
			get { return new SizeF(this.Width, this.Height); }
			set {
				this.Width = value.Width;
				this.Height = value.Height;
			}
		}
		public float X {
			get { return this.x; }
			set { this.x = value; }
		}
		public float Y {
			get { return this.y; }
			set { this.y = value; }
		}
		public float Width {
			get { return this.width; }
			set { this.width = value; }
		}
		public float Height {
			get { return this.height; }
			set { this.height = value; }
		}
		[Browsable(false)]
		public float Left {
			get { return this.X; }
		}
		[Browsable(false)]
		public float Top {
			get { return this.Y; }
		}
		[Browsable(false)]
		public float Right {
			get { return (this.X + this.Width); }
		}
		[Browsable(false)]
		public float Bottom {
			get { return (this.Y + this.Height); }
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return (this.width > 0f) ? (this.height <= 0f) : true; }
		}
		public override bool Equals(object obj) {
			if(!(obj is RectangleF2D)) return false;
			RectangleF2D r = (RectangleF2D)obj;
			return ((((r.x == this.x) && (r.x == this.x)) && (r.width == this.width)) && (r.height == this.height));
		}
		public static bool operator ==(RectangleF2D left, RectangleF2D right) {
			return ((((left.x == right.x) && (left.y == right.y)) && (left.width == right.width)) && (left.height == right.height));
		}
		public static bool operator !=(RectangleF2D left, RectangleF2D right) {
			return !(left == right);
		}
		public bool Contains(float x, float y) {
			return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
		}
		public bool Contains(PointF2D pt) {
			return this.Contains(pt.X, pt.Y);
		}
		public bool Contains(RectangleF2D rect) {
			return ((((this.X <= rect.X) && ((rect.X + rect.Width) <= (this.X + this.Width))) && (this.Y <= rect.Y)) && ((rect.Y + rect.Height) <= (this.Y + this.Height)));
		}
		public override int GetHashCode() {
			return (int)(((((uint)this.X) ^ ((((uint)this.Y) << 13) | (((uint)this.Y) >> 0x13))) ^ ((((uint)this.Width) << 0x1a) | (((uint)this.Width) >> 6))) ^ ((((uint)this.Height) << 7) | (((uint)this.Height) >> 0x19)));
		}
		public void Inflate(float x, float y) {
			this.X -= x;
			this.Y -= y;
			this.Width += 2f * x;
			this.Height += 2f * y;
		}
		public void Inflate(SizeF size) {
			this.Inflate(size.Width, size.Height);
		}
		public static RectangleF2D Inflate(RectangleF2D rect, float x, float y) {
			RectangleF2D r = rect;
			r.Inflate(x, y);
			return r;
		}
		public void Intersect(RectangleF2D rect) {
			RectangleF2D r = Intersect(rect, this);
			this.X = r.X;
			this.Y = r.Y;
			this.Width = r.Width;
			this.Height = r.Height;
		}
		public static RectangleF2D Intersect(RectangleF2D a, RectangleF2D b) {
			float x = Math.Max(a.X, b.X);
			float num2 = Math.Min((float)(a.X + a.Width), (float)(b.X + b.Width));
			float y = Math.Max(a.Y, b.Y);
			float num4 = Math.Min((float)(a.Y + a.Height), (float)(b.Y + b.Height));
			if((num2 >= x) && (num4 >= y)) {
				return new RectangleF2D(x, y, num2 - x, num4 - y);
			}
			return Empty;
		}
		public bool IntersectsWith(RectangleF2D rect) {
			return ((((rect.X < (this.X + this.Width)) && (this.X < (rect.X + rect.Width))) && (rect.Y < (this.Y + this.Height))) && (this.Y < (rect.Y + rect.Height)));
		}
		public static RectangleF2D Union(RectangleF2D a, RectangleF2D b) {
			float x = Math.Min(a.X, b.X);
			float num2 = Math.Max((float)(a.X + a.Width), (float)(b.X + b.Width));
			float y = Math.Min(a.Y, b.Y);
			float num4 = Math.Max((float)(a.Y + a.Height), (float)(b.Y + b.Height));
			return new RectangleF2D(x, y, num2 - x, num4 - y);
		}
		public void Offset(PointF2D pos) {
			this.Offset(pos.X, pos.Y);
		}
		public void Offset(float x, float y) {
			this.X += x;
			this.Y += y;
		}
		public override string ToString() {
			return ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + ",Width=" + this.Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + this.Height.ToString(CultureInfo.CurrentCulture) + "}");
		}
		public static implicit operator RectangleF2D(Rectangle r) {
			return new RectangleF2D((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
		}
		public static implicit operator RectangleF2D(RectangleF rect) {
			return new RectangleF2D(rect.X, rect.Y, rect.Width, rect.Height);
		}
		public static implicit operator RectangleF(RectangleF2D rect) {
			return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}
		public static explicit operator Rectangle(RectangleF2D rect) {
			return Rectangle.Round(rect);
		}
		static RectangleF2D() {
			Empty = new RectangleF2D();
		}
	}
}
