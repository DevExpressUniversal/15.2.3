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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.Office.Drawing {
	#region Painter (abstract class)
	public abstract class Painter : IPatternLinePaintingSupport, IDisposable {
		bool isDisposed;
		Color textForeColor;
		bool allowChangeTextForeColor = true;
		public RectangleF ClipBounds {
			get {
				return RectangularClipBounds;
			}
			set {
				SetClipBounds(value);
			}
		}
		public virtual bool HyperlinksSupported { get { return false; } }
		protected abstract void SetClipBounds(RectangleF bounds);
		protected abstract RectangleF RectangularClipBounds { get; }
		public abstract int DpiY { get; }
		public Color TextForeColor {
			get { return textForeColor; }
			set {
				if (allowChangeTextForeColor)
					textForeColor = value;
			}
		}
		public bool AllowChangeTextForeColor { get { return allowChangeTextForeColor; } set { allowChangeTextForeColor = value; } }
		static Rectangle RectFToRect(RectangleF bounds) {
			return new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
		}
		public virtual void FillRectangle(Brush brush, RectangleF bounds) {
			FillRectangle(brush, RectFToRect(bounds));
		}
		public virtual void FillRectangle(Color color, RectangleF bounds) {
			FillRectangle(color, RectFToRect(bounds));
		}
		public abstract void FillRectangle(Brush brush, Rectangle bounds);
		public abstract void FillRectangle(Color color, Rectangle bounds);
		public abstract void DrawRectangle(Pen pen, Rectangle bounds);
		public virtual void DrawEllipse(Pen pen, Rectangle bounds) { }
		public abstract void FillEllipse(Brush brush, Rectangle bounds);
		public virtual void FillEllipse(Brush brush, RectangleF bounds) {
			FillEllipse(brush, RectFToRect(bounds));
		}
		public virtual void FillEllipse(Brush brush, float x, float y, float width, float height) {
			FillEllipse(brush, new Rectangle((int)x, (int)y, (int)width, (int)height));
		}
#if !SL && !DXPORTABLE
		public abstract void DrawBrick(DevExpress.XtraPrinting.PrintingSystemBase ps, DevExpress.XtraPrinting.VisualBrick brick, Rectangle bounds);
#endif
		public void DrawString(string text, FontInfo fontInfo, Color foreColor, Rectangle rectangle) {
			this.TextForeColor = foreColor;
			DrawString(text, fontInfo, rectangle);
		}
		public void DrawString(string text, FontInfo fontInfo, Color foreColor, Rectangle rectangle, StringFormat stringFormat) {
			this.TextForeColor = foreColor;
			DrawString(text, fontInfo, rectangle, stringFormat);
		}
#if !SL && !DXPORTABLE
		public abstract void DrawString(string text, Brush brush, Font font, float x, float y);
		public abstract SizeF MeasureString(string text, Font font);
#endif
		public abstract void DrawString(string text, FontInfo fontInfo, Rectangle rectangle);
		public abstract void DrawString(string text, FontInfo fontInfo, Rectangle rectangle, StringFormat stringFormat);
		public virtual void DrawSpacesString(string text, FontInfo fontInfo, Color foreColor, Rectangle rectangle) {
			this.TextForeColor = foreColor;
			DrawSpacesString(text, fontInfo, rectangle);
		}
		public abstract void DrawSpacesString(string text, FontInfo fontInfo, Rectangle rectangle);
		public abstract void DrawImage(OfficeImage img, Rectangle bounds);
		public virtual void DrawImage(OfficeImage img, Rectangle bounds, Size imgActualSizeInLayoutUnits, DevExpress.XtraPrinting.ImageSizeMode sizing) {
		}
		public abstract void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
		public abstract void DrawLines(Pen pen, PointF[] points);
		public abstract void FillPolygon(Brush brush, PointF[] points);
		public abstract void ExcludeCellBounds(Rectangle rect, Rectangle rowBounds);
		public abstract void ResetCellBoundsClip();
		public virtual void FinishPaint() { }
		public virtual void SnapWidths(float[] widths) {
			PointF[] points = new PointF[widths.Length + 1];
			points[0] = new PointF(0, 0);
			for (int i = 0; i < widths.Length; i++)
				points[i + 1] = new PointF(widths[i], 0);
			TransformToPixels(points);
			for (int i = widths.Length; i > 0; i--) {
				points[i].X = widths[i - 1] != 0 ? Math.Max((int)(points[i].X - points[0].X + 0.5), 1) : 0;
			}
			points[0].X = 0;
			TransformToLayoutUnits(points);
			for (int i = widths.Length; i > 0; i--) {
				widths[i - 1] = points[i].X - points[0].X;
			}
		}
		public virtual void SnapHeights(float[] heights) {
			PointF[] points = new PointF[heights.Length + 1];
			points[0] = new PointF(0, 0);
			for (int i = 0; i < heights.Length; i++)
				points[i + 1] = new PointF(0, heights[i]);
			TransformToPixels(points);
			for (int i = heights.Length; i > 0; i--) {
				points[i].Y = heights[i - 1] != 0 ? Math.Max((int)(points[i].Y - points[0].Y + 0.5), 1) : 0;
			}
			points[0].Y = 0;
			TransformToLayoutUnits(points);
			for (int i = heights.Length; i > 0; i--) {
				heights[i - 1] = points[i].Y - points[0].Y;
			}
		}
		public virtual PointF GetSnappedPoint(PointF point) {
			PointF[] points = new PointF[1] { point };
			TransformToPixels(points);
			points[0].X = (int)points[0].X;
			points[0].Y = (int)points[0].Y;
			TransformToLayoutUnits(points);
			return points[0];
		}
		protected abstract PointF[] TransformToPixels(PointF[] points);
		protected abstract PointF[] TransformToLayoutUnits(PointF[] points);
		public abstract Pen GetPen(Color color);
		public abstract Pen GetPen(Color color, float thickness);
		public abstract void ReleasePen(Pen pen);
		public abstract Brush GetBrush(Color color);
		public abstract void ReleaseBrush(Brush brush);
		public bool IsDisposed { get { return isDisposed; } }
		public bool TryPushRotationTransform(Point center, float angleInDegrees) {
			bool needApplyTransform = (angleInDegrees % 360f) != 0;
			if (!needApplyTransform)
				return false;
			PushRotationTransform(center, angleInDegrees);
			return true;
		}
		public abstract void PushRotationTransform(Point center, float angleInDegrees);
		public abstract void PopTransform();
		public abstract void PushSmoothingMode(bool highQuality);
		public abstract void PopSmoothingMode();
		public abstract void PushPixelOffsetMode(bool highQualtity);
		public abstract void PopPixelOffsetMode();
		public virtual void SetUriArea(string uri, RectangleF bounds) {
		}
		public virtual RectangleF ApplyClipBounds(RectangleF clipBounds) {
			RectangleF oldClipBounds = ClipBounds;
			SetClipBounds(clipBounds);
			return oldClipBounds;
		}
		public virtual void RestoreClipBounds(RectangleF clipBounds) {
			SetClipBounds(clipBounds);
		}
#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			isDisposed = true;
		}
		public virtual void Dispose() {
			Dispose(true);
		}
#endregion
	}
#endregion
#region EmptyPainter
	public class EmptyPainter : Painter {
		protected override RectangleF RectangularClipBounds { get { return RectangleF.Empty; } }
		public override int DpiY { get { return 96; } }
		protected override void SetClipBounds(RectangleF bounds) {
		}
		public override void FillRectangle(Brush brush, Rectangle bounds) {
		}
		public override void FillRectangle(Color color, Rectangle bounds) {
		}
		public override void FillEllipse(Brush brush, Rectangle bounds) {
		}
		public override void DrawRectangle(Pen pen, Rectangle bounds) {
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle rectangle) {
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle rectangle, StringFormat stringFormat) {
		}
#if !SL && !DXPORTABLE
		public override void DrawString(string text, Brush brush, Font font, float x, float y) {
		}
		public override SizeF MeasureString(string text, Font font) {
			return SizeF.Empty;
		}
#endif
		public override void DrawSpacesString(string text, FontInfo fontInfo, Rectangle rectangle) {
		}
		public override void DrawImage(OfficeImage img, Rectangle bounds) {
		}
		public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
		}
		public override void DrawLines(Pen pen, PointF[] points) {
		}
#if !SL && !DXPORTABLE
		public override void DrawBrick(DevExpress.XtraPrinting.PrintingSystemBase ps, XtraPrinting.VisualBrick brick, Rectangle bounds) {
		}
#endif
		public override void FillPolygon(Brush brush, PointF[] points) {
		}
		public override void ExcludeCellBounds(Rectangle rect, Rectangle rowBounds) {
		}
		public override void ResetCellBoundsClip() {
		}
		public override Pen GetPen(Color color) {
			return new Pen(color);
		}
		public override Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public override void ReleasePen(Pen pen) {
		}
		public override Brush GetBrush(Color color) {
#if !SL
			return Brushes.Black;
#else
			return new SolidColorBrush(color);
#endif
		}
		public override void ReleaseBrush(Brush brush) {
		}
		public override void SnapWidths(float[] widths) {
		}
		public override void SnapHeights(float[] heights) {
		}
		public override PointF GetSnappedPoint(PointF point) {
			return point;
		}
		protected override PointF[] TransformToPixels(PointF[] points) {
			return points;
		}
		protected override PointF[] TransformToLayoutUnits(PointF[] points) {
			return points;
		}
		public override void PushRotationTransform(Point center, float angleInRadians) {
		}
		public override void PopTransform() {
		}
		public override void PushSmoothingMode(bool highQuality) {
		}
		public override void PopSmoothingMode() {
		}
		public override void PushPixelOffsetMode(bool highQualtity) {
		}
		public override void PopPixelOffsetMode() {
		}
	}
#endregion
	#region PainterCache
	public class PainterCache : IDisposable {
		struct PenKey {
			public Color Color;
			public int Width;
			public PenKey(Color color, int width) {
				this.Color = color;
				this.Width = width;
			}
		}
		class PenKeyComparer : IEqualityComparer<PenKey> {
			public bool Equals(PenKey x, PenKey y) {
				return x.Color == y.Color && x.Width == y.Width;
			}
			public int GetHashCode(PenKey obj) {
				return obj.Color.GetHashCode() ^ obj.Width;
			}
		}
		class ColorComparer : IEqualityComparer<Color> {
			public bool Equals(Color x, Color y) {
				return x == y;
			}
			public int GetHashCode(Color obj) {
				return obj.GetHashCode();
			}
		}
		[ThreadStatic]
		static PainterCache defaultCache;
		public static PainterCache DefaultCache {
			get {
				if (defaultCache == null) defaultCache = new PainterCache();
				return defaultCache;
			}
		}
		Dictionary<Color, SolidBrush> solidBrushes;
		Dictionary<PenKey, Pen> pens;
		public PainterCache() {
			this.solidBrushes = new Dictionary<Color, SolidBrush>(new ColorComparer());
			this.pens = new Dictionary<PenKey, Pen>(new PenKeyComparer());
		}
		public SolidBrush GetSolidBrush(Color color) {
#if !DXPORTABLE
			if (color.IsSystemColor)
			return (SolidBrush)SystemBrushes.FromSystemColor(color);
#endif
			SolidBrush brush;
			if (this.solidBrushes.TryGetValue(color, out brush))
				return brush;
			brush = new SolidBrush(color);
			this.solidBrushes.Add(color, brush);
			return brush;
		}
		public Pen GetPen(Color color) {
			return GetPen(color, 1);
		}
		public Pen GetPen(Color color, int width) {
#if !DXPORTABLE
			if (width == 1 && color.IsSystemColor)
				return SystemPens.FromSystemColor(color);
#endif
			PenKey key = new PenKey(color, width);
			Pen pen;
			if (this.pens.TryGetValue(key, out pen))
				return pen;
			pen = new Pen(color, width);
			this.pens.Add(key, pen);
			return pen;
		}
		void Clear() {
			ClearCache(this.pens);
			ClearCache(this.solidBrushes);
		}
		void ClearCache(IDictionary cache) {
			if (cache == null) return;
			foreach (IDisposable obj in cache.Values)
				obj.Dispose();
			cache.Clear();
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~PainterCache() {
			Dispose(false);
		}
		void Dispose(bool disposing) {
			if (disposing)
				Clear();
		}
	}
#endregion
}
