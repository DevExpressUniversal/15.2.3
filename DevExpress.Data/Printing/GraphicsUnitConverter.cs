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
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
#if SL
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraPrinting {
	public class GraphicsUnitConverter {
		static double GetScale(float fromDpi, float toDpi) {
			return (double)toDpi / (double)fromDpi;
		}
		static float ConvF(float val, double scale) {
			return (float)((double)val * scale) + 0f;
		}
		static double ConvD(double val, double scale) {
			return val * scale;
		}
		static int Conv(int val, double scale) {
			return System.Convert.ToInt32(val * scale);
		}
		static public Point Round(PointF point) {
			return new Point(System.Convert.ToInt32(point.X), System.Convert.ToInt32(point.Y));
		}
		static public Rectangle Round(RectangleF rect) {
			Point p = new Point(System.Convert.ToInt32(rect.Left), System.Convert.ToInt32(rect.Top));
			Size s = new Size(System.Convert.ToInt32((float)rect.Right) - p.X, System.Convert.ToInt32((float)rect.Bottom) - p.Y);
			return new Rectangle(p, s);
		}
		static public Rectangle Convert(Rectangle val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public Size Convert(Size val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public Point Convert(Point val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public RectangleF Convert(RectangleF val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public RectangleDF Convert(RectangleDF val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public SizeF Convert(SizeF val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public PointF Convert(PointF val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public MarginsF Convert(MarginsF val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public float Convert(float val, GraphicsUnit fromUnit, GraphicsUnit toUnit) {
			return Convert(val, GraphicsDpi.UnitToDpi(fromUnit), GraphicsDpi.UnitToDpi(toUnit));
		}
		static public Rectangle Convert(Rectangle val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			return Round(Convert((RectangleF)val, fromDpi, toDpi));
		}
		static public Size Convert(Size val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return new Size(Conv(val.Width, s), Conv(val.Height, s));
		}
		static public Point Convert(Point val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return new Point(Conv(val.X, s), Conv(val.Y, s));
		}
		static public int Convert(int val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return Conv(val, s);
		}
		static public RectangleF Convert(RectangleF val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return RectangleF.FromLTRB(ConvF(val.Left, s), ConvF(val.Top, s), ConvF(val.Right, s), ConvF(val.Bottom, s));
		}
		static public RectangleDF Convert(RectangleDF val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return RectangleDF.FromLTRB(ConvD(val.Left, s), ConvD(val.Top, s), ConvD(val.Right, s), ConvD(val.Bottom, s));
		}
		static public SizeF Convert(SizeF val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return new SizeF(ConvF(val.Width, s), ConvF(val.Height, s));
		}
		static public PointF Convert(PointF val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return new PointF(ConvF(val.X, s), ConvF(val.Y, s));
		}
		public static float Convert(float val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return ConvF(val, s);
		}
		public static MarginsF Convert(MarginsF val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			MarginsF result = new MarginsF();
			result.Left = ConvF(val.Left, s);
			result.Right = ConvF(val.Right, s);
			result.Top = ConvF(val.Top, s);
			result.Bottom = ConvF(val.Bottom, s);
			return result;
		}
		public static Margins Convert(Margins val, float fromDpi, float toDpi) {
			if(fromDpi == toDpi)
				return val;
			double s = GetScale(fromDpi, toDpi);
			return new Margins(
				Conv(val.Left, s),
				Conv(val.Right, s),
				Conv(val.Top, s),
				Conv(val.Bottom, s));
		}
		static public RectangleF PixelToDoc(RectangleF val) {
			return Convert(val, GraphicsDpi.Pixel, GraphicsDpi.Document);
		}
		static public SizeF PixelToDoc(SizeF val) {
			return Convert(val, GraphicsDpi.Pixel, GraphicsDpi.Document);
		}
		static public PointF PixelToDoc(PointF val) {
			return Convert(val, GraphicsDpi.Pixel, GraphicsDpi.Document);
		}
		static public float PixelToDoc(float val) {
			return Convert(val, GraphicsDpi.Pixel, GraphicsDpi.Document);
		}
		static public RectangleF DocToPixel(RectangleF val) {
			return Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel);
		}
		static public SizeF DocToPixel(SizeF val) {
			return Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel);
		}
		static public PointF DocToPixel(PointF val) {
			return Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel);
		}
		static public float DocToPixel(float val) {
			return Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel);
		}
		static public MarginsF DocToPixel(MarginsF val) {
			return Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel);
		}
		static public RectangleF DipToDoc(RectangleF rect) {
			return Convert(rect, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Document);
		}
		static public SizeF DipToDoc(SizeF size) {
			return Convert(size, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Document);
		}
		static public float DipToDoc(float val) {
			return Convert(val, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Document);
		}
		static public float DocToDip(float val) {
			return Convert(val, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	public class PSUnitConverter : GraphicsUnitConverter {
		public static PointF PixelToDoc(PointF val, float zoom, PointF scrollPos) {
			return PSNativeMethods.TranslatePointF(PixelToDoc(val, zoom), scrollPos);
		}
		public static RectangleF PixelToDoc(RectangleF val, float zoom, PointF scrollPos) {
			RectangleF result = PixelToDoc(val, zoom);
			result.Offset(scrollPos.X, scrollPos.Y);
			return result;
		}
		public static RectangleF DocToPixel(RectangleF val, float zoom, PointF scrollPos) {
			val.Offset(-scrollPos.X, -scrollPos.Y);
			return DocToPixel(val, zoom);
		}
		public static float PixelToDoc(float val, float zoom) {
			return PixelToDoc(val) / zoom;
		}
		public static SizeF PixelToDoc(SizeF val, float zoom) {
			return MathMethods.Scale(PixelToDoc(val), 1f / zoom);
		}
		public static PointF PixelToDoc(PointF val, float zoom) {
			return MathMethods.Scale(PixelToDoc(val), 1f / zoom);
		}
		public static RectangleF PixelToDoc(RectangleF val, float zoom) {
			return MathMethods.Scale(PixelToDoc(val), 1f / zoom);
		}
		public static float DocToPixel(float val, float zoom) {
			return DocToPixel(val) * zoom;
		}
		public static SizeF DocToPixel(SizeF val, float zoom) {
			return MathMethods.Scale(DocToPixel(val), zoom);
		}
		public static PointF DocToPixel(PointF val, float zoom) {
			return MathMethods.Scale(DocToPixel(val), zoom);
		}
		public static RectangleF DocToPixel(RectangleF val, float zoom) {
			return MathMethods.Scale(DocToPixel(val), zoom);
		}
	}
}
