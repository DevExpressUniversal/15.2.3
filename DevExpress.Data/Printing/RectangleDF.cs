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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.Drawing;
#if SL
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraPrinting.Native {
	public struct RectangleDF {
		public static readonly RectangleDF Empty = new RectangleDF();
		double x, y;
		float width, height;
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public double Left {  get { return x; } }
		public double Top { get { return y; } }
		public double Right { get { return x + width; } }
		public double Bottom { get { return y + height; } }
		public float Width { get { return width; } set { width = value; } }
		public float Height { get { return height; } set { height = value; } }
		public bool IsEmpty {
			get {
				return this.Width <= 0d || this.Height <= 0d;
			}
		}
		public RectangleDF(double x, double y, float width, float height) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		public static RectangleDF Offset(RectangleF val, double dx, double dy) {
			RectangleDF valDF = RectangleDF.FromRectangleF(val);
			valDF.Offset(dx, dy);
			return valDF;
		}
		public static RectangleDF Offset(RectangleDF val, double dx, double dy) {
			val.Offset(dx, dy);
			return val;
		}
		public void Offset(double dx, double dy) {
			x += dx;
			y += dy;
		}
		public static RectangleDF FromLTRB(double left, double top, double right, double bottom) {
			return new RectangleDF(left, top, (float)(right - left), (float)(bottom - top));
		}
		public static RectangleDF FromRectangleF(RectangleF val) {
			RectangleDF rect = new RectangleDF();
			rect.x = val.X;
			rect.y = val.Y;
			rect.width = val.Width;
			rect.height = val.Height;
			return rect;
		}
		public RectangleF ToRectangleF() {
			RectangleF rect = new RectangleF();
			rect.X = (float)this.X;
			rect.Y = (float)this.Y;
			rect.Width = this.Width;
			rect.Height = this.Height;
			return rect;
		}
		public Rectangle ToRectangle() {
			return new Rectangle((int) Math.Round((double) X), (int) Math.Round((double) Y), (int) Math.Round((double) Width), (int) Math.Round((double) Height));
		}
		public void Intersect(RectangleDF rect) {
			RectangleDF intersectionRect = Intersect(rect, this);
			this.X = intersectionRect.X;
			this.Y = intersectionRect.Y;
			this.Width = intersectionRect.Width;
			this.Height = intersectionRect.Height;
		}
		public static RectangleDF Intersect(RectangleDF a, RectangleDF b) {
			double x = Math.Max(a.X, b.X);
			double right = Math.Min(a.X + a.Width, b.X + b.Width);
			double y = Math.Max(a.Y, b.Y);
			double bottom = Math.Min(a.Y + a.Height, b.Y + b.Height);
			if((right >= x) && (bottom >= y)) {
				return new RectangleDF(x, y, (float)(right - x), (float)(bottom - y));
			}
			return Empty;
		}
		public static implicit operator RectangleDF(Rectangle r) {
			return new RectangleDF((double)r.X, (double)r.Y, (float)r.Width, (float)r.Height);
		}
		public override string ToString() {
			return ("{X=" + this.X.ToString(System.Globalization.CultureInfo.CurrentCulture)
				+ ",Y=" + this.Y.ToString(System.Globalization.CultureInfo.CurrentCulture)
				+ ",Width=" + this.Width.ToString(System.Globalization.CultureInfo.CurrentCulture)
				+ ",Height=" + this.Height.ToString(System.Globalization.CultureInfo.CurrentCulture)
				+ "}");
		}
	}
}
namespace DevExpress.Utils.Serializing {
	using System.Xml;
	public abstract class StructDoubleConverter : StructConverter<double> {
		protected StructDoubleConverter() {
		}
		protected override string ElementToString(double obj) {
			return XmlConvert.ToString(obj);
		}
		protected override double ToType(string str) {
			return XmlConvert.ToDouble(str);
		}
	}
	class RectangleDFConverter : StructDoubleConverter {
		public static readonly RectangleDFConverter Instance = new RectangleDFConverter();
		public override Type Type { get { return typeof(RectangleDF); } }
		RectangleDFConverter() {
		}
		protected override double[] GetValues(object obj) {
			RectangleDF rectangle = (RectangleDF)obj;
			return new double[] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height };
		}
		protected override object CreateObject(double[] values) {
			return new RectangleDF(values[0], values[1], (float)values[2], (float)values[3]);
		}
	}
}
