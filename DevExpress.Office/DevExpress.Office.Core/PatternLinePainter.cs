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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Drawing {
	#region IPatternLinePaintingSupport
	public interface IPatternLinePaintingSupport {
		Pen GetPen(Color color);
		Pen GetPen(Color color, float thickness);
		void ReleasePen(Pen pen);
		Brush GetBrush(Color color);
		void ReleaseBrush(Brush brush);
		void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
		void DrawLines(Pen pen, PointF[] points);
	}
	#endregion
	#region EmptyPatternLinePaintingSupport
	public class EmptyPatternLinePaintingSupport : IPatternLinePaintingSupport {
		#region IPatternLinePaintingSupport Members
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
		}
		public void DrawLines(Pen pen, PointF[] points) {
		}
		public Brush GetBrush(Color color) {
#if !SL
			return new SolidBrush(color);
#else
			return new SolidColorBrush(color);
#endif
		}
		public Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public Pen GetPen(Color color) {
			return new Pen(color);
		}
		public void ReleaseBrush(Brush brush) {
#if !SL
			brush.Dispose();
#endif
		}
		public void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		#endregion
	}
	#endregion
	#region PatternLinePainter (abstract class)
	public abstract class PatternLinePainter {
		protected virtual float UnitsToPixels(float val, float dpi) {
			return UnitConverter.LayoutUnitsToPixelsF(val, dpi);
		}
		protected virtual float PixelsToUnits(float val, float dpi) {
			return UnitConverter.PixelsToLayoutUnitsF(val, dpi);
		}
		protected virtual float RoundToPixels(float val, float dpi) {
			float pixelVal = UnitsToPixels(val, dpi);
			pixelVal = (float)Math.Round(pixelVal);
			val = PixelsToUnits(pixelVal, dpi);
			return val;
		}
		readonly IPatternLinePaintingSupport painter;
		readonly DocumentLayoutUnitConverter unitConverter;
		protected PatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(painter, "painter");
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.painter = painter;
			this.unitConverter = unitConverter;
		}
		public IPatternLinePaintingSupport Painter { get { return painter; } }
		public DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		protected virtual float[] DotPattern { get { return Parameters.DotPattern; } }
		protected virtual float[] DashPattern { get { return Parameters.DashPattern; } }
		protected virtual float[] DashSmallGapPattern { get { return Parameters.DashSmallGapPattern; } }
		protected virtual float[] DashDotPattern { get { return Parameters.DashDotPattern; } }
		protected virtual float[] DashDotDotPattern { get { return Parameters.DashDotDotPattern; } }
		protected virtual float[] LongDashPattern { get { return Parameters.LongDashPattern; } }
		protected virtual float PixelPenWidth { get { return Parameters.PixelPenWidth; } }
		protected virtual float PixelStep { get { return Parameters.PixelStep; } }
		protected abstract PatternLinePainterParameters Parameters { get; }
		protected abstract Graphics PixelGraphics { get; }
		protected virtual RectangleF RotateBounds(RectangleF bounds) {
			return bounds;
		}
		protected virtual PointF RotatePoint(PointF pointF) {
			return pointF;
		}
		protected virtual void DrawLine(Pen pen, RectangleF bounds) {
			Painter.DrawLine(pen, bounds.X, bounds.Y, bounds.Right, bounds.Y);
		}
		protected virtual void DrawLine(Pen pen, PointF from, PointF to) {
			Painter.DrawLine(pen, from.X, from.Y, to.X, to.Y);
		}
		protected void DrawSolidLine(RectangleF bounds, Color color) {			
			Pen pen = painter.GetPen(color, Math.Min(bounds.Width, bounds.Height));
			try {
				DrawLine(pen, bounds);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
		protected void DrawSolidLine(PointF from, PointF to, Color color, float thickness) {
			Pen pen = painter.GetPen(color, thickness);
			try {
				DrawLine(pen, from, to);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
		public void DrawDoubleSolidLine(RectangleF bounds, Color color) {
			bounds = RotateBounds(bounds);
			float thickness = RoundToPixels(bounds.Height / 4, PixelGraphics.DpiY);
			if (thickness <= PixelsToUnits(1, PixelGraphics.DpiY))
				thickness = PixelsToUnits(1, PixelGraphics.DpiY);
			float halfThickness = thickness / 2;
			RectangleF topBounds = new RectangleF(bounds.X, bounds.Y + halfThickness, bounds.Width, thickness);
			RectangleF bottomBounds = new RectangleF(bounds.X, bounds.Bottom - halfThickness, bounds.Width, thickness);
			float distance = RoundToPixels(bottomBounds.Y - topBounds.Bottom, PixelGraphics.DpiY);
			if (distance <= PixelsToUnits(1, PixelGraphics.DpiY))
				distance = PixelsToUnits(1, PixelGraphics.DpiY);
			bottomBounds.Y = topBounds.Bottom + distance;
			DrawSolidLine(RotateBounds(topBounds), color);
			DrawSolidLine(RotateBounds(bottomBounds), color);
		}
		protected internal void DrawDoubleSolidLine(PointF from, PointF to, Color color, float thickness) {
			thickness /= 3.0f;
			if (thickness <= PixelsToUnits(1, PixelGraphics.DpiY))
				thickness = PixelsToUnits(1, PixelGraphics.DpiY);
			double dx = from.X - to.X;
			double dy = from.Y - to.Y;
			double length = Math.Sqrt(dx * dx + dy * dy);
			if (length == 0)
				return;
			double cosA = dx / length;
			double sinA = dy / length;
			PointF from1 = new PointF((float)(from.X - thickness * sinA), (float)(from.Y + thickness * cosA));
			PointF to1 = new PointF((float)(to.X - thickness * sinA), (float)(to.Y + thickness * cosA));
			DrawSolidLine(from1, to1, color, thickness);
			PointF from2 = new PointF((float)(from.X + thickness * sinA), (float)(from.Y - thickness * cosA));
			PointF to2 = new PointF((float)(to.X + thickness * sinA), (float)(to.Y - thickness * cosA));
			DrawSolidLine(from2, to2, color, thickness);
		}
		public void DrawPatternLine(RectangleF bounds, Color color, float[] pattern) {
			bounds = RotateBounds(bounds);
			Pen pen = new Pen(color);
			try {
				pen.Width = bounds.Height;
				pen.DashPattern = MakeFixedWidthPattern(pattern, bounds.Height);
				DrawLine(pen, RotateBounds(bounds));
			}
			finally {
				pen.Dispose();
			}
		}
		protected internal void DrawPatternLine(PointF from, PointF to, Color color, float thickness, float[] pattern) {
			Pen pen = new Pen(color, thickness);
			try {
				pen.DashPattern = MakeFixedWidthPattern(pattern, thickness);
				DrawLine(pen, from, to);
			}
			finally {
				pen.Dispose();
			}
		}
		protected void DrawWaveUnderline(RectangleF bounds, Color color, float penWidth) {
			Pen pen = painter.GetPen(color, penWidth);
			try {
				DrawWaveUnderline(bounds, pen);
			}
			finally {
				painter.ReleasePen(pen);
			}
		}
		public void DrawWaveUnderline(RectangleF bounds, Pen pen, float step) {
			bounds = RotateBounds(bounds);
			int count = (int)Math.Ceiling(bounds.Width / step);
			if (count <= 1)
				return;
			float x = bounds.X;
			float y = bounds.Top;
			float sum = y + bounds.Bottom;
			PointF[] points = new PointF[count];
			for (int i = 0; i < count; i++, x += step) {
				points[i] = RotatePoint(new PointF(x, y));
				y = sum - y;
			}
			Painter.DrawLines(pen, points);
		}
		public void DrawWaveUnderline(RectangleF bounds, Pen pen) {
			bounds = RotateBounds(bounds);
			float step = RoundToPixels(bounds.Height, PixelGraphics.DpiY);
			step = Math.Max(PixelStep, step);
			DrawWaveUnderline(RotateBounds(bounds), pen, step);
		}
		float[] MakeFixedWidthPattern(float[] pattern, float thickness) {
			int count = pattern.Length;
			float[] result = new float[count];
			for (int i = 0; i < count; i++)
				result[i] = pattern[i] / thickness;
			return result;
		}
		protected virtual RectangleF MakeBoundsAtLeast2PixelsHigh(RectangleF bounds) {
			float height = RoundToPixels(bounds.Height, PixelGraphics.DpiY);
			if (height <= PixelsToUnits(2, PixelGraphics.DpiY)) {
				height = PixelsToUnits(2, PixelGraphics.DpiY);
				bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, height);
			}
			return bounds;
		}
	}
	#endregion
	#region PatternLinePainterParameters (abstract class)
	public abstract class PatternLinePainterParameters {
		#region Fields
		float[] dotPattern;
		float[] dashPattern;
		float[] dashSmallGapPattern;
		float[] dashDotPattern;
		float[] dashDotDotPattern;
		float[] longDashPattern;
		float pixelPenWidth;
		float pixelStep;
		#endregion
		#region Properties
		public float[] DotPattern { get { return dotPattern; } }
		public float[] DashPattern { get { return dashPattern; } }
		public float[] DashSmallGapPattern { get { return dashSmallGapPattern; } }
		public float[] DashDotPattern { get { return dashDotPattern; } }
		public float[] DashDotDotPattern { get { return dashDotDotPattern; } }
		public float[] LongDashPattern { get { return longDashPattern; } }
		public float PixelPenWidth { get { return pixelPenWidth; } }
		public float PixelStep { get { return pixelStep; } }
		#endregion
		public virtual void Initialize(float dpiX) {
			dotPattern = CreatePattern(new float[] { 10, 10 }, dpiX);
			dashPattern = CreatePattern(new float[] { 40, 20 }, dpiX);
			dashSmallGapPattern = CreatePattern(new float[] { 40, 10 }, dpiX);
			dashDotPattern = CreatePattern(new float[] { 40, 13, 13, 13 }, dpiX);
			dashDotDotPattern = CreatePattern(new float[] { 30, 10, 10, 10, 10, 10 }, dpiX);
			longDashPattern = CreatePattern(new float[] { 80, 40 }, dpiX);
			pixelPenWidth = PixelsToUnits(1, dpiX);
			pixelStep = PixelsToUnits(2, dpiX);
		}
		protected float[] CreatePattern(float[] pattern, float dpiX) {
			int count = pattern.Length;
			float[] result = new float[count];
			for (int i = 0; i < count; i++)
				result[i] = PixelsToUnits(pattern[i], dpiX) / 5;
			return result;
		}
		protected internal abstract float PixelsToUnits(float value, float dpi);
	}
	#endregion
	#region PatternLinePainterParametersTable
	public class PatternLinePainterParametersTable : Dictionary<Type, PatternLinePainterParameters> {
	}
	#endregion
}
