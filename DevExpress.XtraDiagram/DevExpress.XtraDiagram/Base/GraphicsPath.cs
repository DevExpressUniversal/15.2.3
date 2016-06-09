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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
using PlatformPoint = System.Windows.Point;
using PlatformSize = System.Windows.Size;
namespace DevExpress.XtraDiagram.Base {
	public class XtraGraphicsPath : IDisposable, ICloneable {
		GraphicsPath graphicsPath;
		Point translatePoint;
		public XtraGraphicsPath() {
			this.graphicsPath = new GraphicsPath();
		}
		public XtraGraphicsPath(GraphicsPath graphicsPath) {
			this.graphicsPath = graphicsPath;
			this.translatePoint = Point.Empty;
		}
		public void TranslateTransform(Rectangle rect) {
			TranslateTransform(rect.Location);
		}
		public void TranslateTransform(Point point) {
			this.translatePoint = point;
			GetPath().TranslateTransform(point);
		}
		public void RotateTransform(double angle, Rectangle rect) {
			RotateTransform(angle, rect.GetCenterPoint());
		}
		public void RotateTransform(double angle, Point point) {
			GetPath().RotateTransform((float)angle, point);
		}
		public void OffsetTransform(Point point) {
			Point newPoint = new Point(point.X - this.translatePoint.X, point.Y - this.translatePoint.Y);
			if(newPoint.IsEmpty) return;
			GetPath().TranslateTransform(newPoint);
			this.translatePoint.Offset(newPoint);
		}
		public bool IsVisible(Point pt) {
			return GetPath().IsVisible(pt);
		}
		public bool IsOutlineVisible(Point pt, Pen outlinePen) {
			return GetPath().IsOutlineVisible(pt, outlinePen);
		}
		public Rectangle GetBounds() {
			return GetPath().GetBounds().ToRectangle();
		}
		public GraphicsPath GetPath() { return graphicsPath; }
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		public XtraGraphicsPath Clone() {
			XtraGraphicsPath clone = new XtraGraphicsPath(GetPath());
			clone.Assign(this);
			return clone;
		}
		protected virtual void Assign(XtraGraphicsPath other) {
			this.translatePoint = other.translatePoint;
		}
		#endregion
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.graphicsPath != null) this.graphicsPath.Dispose();
			}
			this.graphicsPath = null;
		}
		#endregion
	}
	public class DiagramGraphicsPath : ICloneable, IDisposable {
		bool closed;
		bool filled;
		XtraGraphicsPath graphicsPath;
		private DiagramGraphicsPath() : this(null, false, false) { }
		public DiagramGraphicsPath(XtraGraphicsPath graphicsPath, bool closed, bool filled) {
			this.graphicsPath = graphicsPath;
			this.closed = closed;
			this.filled = filled;
		}
		public bool IsVisible(Point pt) {
			return GraphicsPath.IsVisible(pt);
		}
		public bool IsOutlineVisible(Point pt, Pen outlinePen) {
			return GraphicsPath.IsOutlineVisible(pt, outlinePen);
		}
		public DiagramGraphicsPath Clone() {
			DiagramGraphicsPath other = new DiagramGraphicsPath();
			other.Assign(this);
			return other;
		}
		protected virtual void Assign(DiagramGraphicsPath other) {
			this.graphicsPath = (XtraGraphicsPath)other.GraphicsPath.Clone();
			this.closed = other.Closed;
			this.filled = other.Filled;
		}
		public void TranslateTransform(Rectangle rect) {
			GraphicsPath.TranslateTransform(rect);
		}
		public void RotateTransform(double angle, Rectangle rect) {
			GraphicsPath.RotateTransform((float)angle, rect);
		}
		public bool Closed { get { return closed; } }
		public bool Filled { get { return filled; } }
		protected XtraGraphicsPath GraphicsPath { get { return graphicsPath; } }
		public void AddLine(float x1, float y1, float x2, float y2) {
			GraphicsPath.GetPath().AddLine(x1, y1, x2, y2);
		}
		public void AddArc(PlatformPoint start, PlatformPoint end, PlatformSize size, SpinDirection direction) {
			GraphicsPath.GetPath().AddArc(start, end, size, direction);
		}
		public void AddBezier(PlatformPoint point1, PlatformPoint point2, PlatformPoint point3, PlatformPoint point4) {
			GraphicsPath.GetPath().AddBezier(point1.ToWinPointF(), point2.ToWinPointF(), point3.ToWinPointF(), point4.ToWinPointF());
		}
		public void AddQuadraticBezier(PlatformPoint point1, PlatformPoint point2, PlatformPoint point3) {
			GraphicsPath.GetPath().AddBezier(point1.ToWinPointF(), point2.ToWinPointF(), point3.ToWinPointF(), point3.ToWinPointF());
		}
		public void CloseFigure() {
			GraphicsPath.GetPath().CloseFigure();
		}
		public virtual bool ContainsText { get { return false; } }
		public virtual void Draw(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			if(Filled) {
				FillInterior(cache, drawArgs);
			}
			DrawBorder(cache, drawArgs);
		}
		protected virtual void FillInterior(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			cache.Graphics.FillPath(drawArgs.InteriorBrush, GraphicsPath.GetPath());
		}
		protected virtual void DrawBorder(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			cache.Graphics.DrawPath(drawArgs.BorderPen, GraphicsPath.GetPath());
		}
		public virtual void DrawShapeShadow(DiagramItemDrawArgs drawArgs, GraphicsCache cache) {
			GraphicsPath.GetPath().TranslateTransform(-drawArgs.ItemBounds.X, -drawArgs.ItemBounds.Y);
			try {
				cache.Graphics.TranslateTransform(0, drawArgs.ShadowSize);
				for(int i = 0; i < drawArgs.ShadowSize; i++) {
					int horz = ((i & 0x1) == 0) ? 1 : -1;
					cache.Graphics.TranslateTransform(horz, 0);
					cache.Graphics.FillPath(cache.GetSolidBrush(Color.FromArgb(drawArgs.GetShadowAlpha(i), drawArgs.ShadowColor)), GraphicsPath.GetPath());
					cache.Graphics.TranslateTransform(0, -1);
				}
			}
			finally {
				GraphicsPath.GetPath().TranslateTransform(drawArgs.ItemBounds.X, drawArgs.ItemBounds.Y);
			}
		}
		protected virtual bool SupportsCaching { get { return false; } }
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool dispose) {
			if(dispose) {
				if(this.graphicsPath != null && !SupportsCaching) this.graphicsPath.Dispose();
			}
			this.graphicsPath = null;
		}
		#endregion
	}
	public abstract class DiagramTextGraphicsPathBase : DiagramGraphicsPath {
		public DiagramTextGraphicsPathBase(XtraGraphicsPath graphicsPath)
			: base(graphicsPath, true, true) {
		}
		public override bool ContainsText { get { return true; } }
		public override void Draw(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			cache.Graphics.FillPath(drawArgs.TextBrush, GraphicsPath.GetPath());
		}
		protected override bool SupportsCaching { get { return true; } }
	}
	public class DiagramShapeTextGraphicsPath : DiagramTextGraphicsPathBase {
		public DiagramShapeTextGraphicsPath(XtraGraphicsPath graphicsPath)
			: base(graphicsPath) {
		}
		public override void Draw(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			base.Draw(cache, drawArgs);
		}
	}
	public class DiagramConnectorTextGraphicsPath : DiagramTextGraphicsPathBase {
		public DiagramConnectorTextGraphicsPath(XtraGraphicsPath graphicsPath)
			: base(graphicsPath) {
		}
		public override void Draw(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			DrawBackground(cache, drawArgs);
			base.Draw(cache, drawArgs);
		}
		protected virtual void DrawBackground(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			Rectangle bounds = GraphicsPath.GetBounds().Inflated(3);
			cache.Graphics.FillRectangle(Brushes.White, bounds);
		}
	}
	public class DiagramGraphicsPathCollection : Collection<DiagramGraphicsPath> {
		public DiagramGraphicsPathCollection() {
		}
		public void ForEach(Action<DiagramGraphicsPath> action) {
			foreach(DiagramGraphicsPath path in Items) {
				action(path);
			}
		}
		public bool Exists(Func<DiagramGraphicsPath, bool> condition) {
			foreach(DiagramGraphicsPath path in Items) {
				if(condition(path)) return true;
			}
			return false;
		}
	}
	public struct DiagramXtraPathCacheKey {
		readonly int itemId;
		readonly string text;
		readonly Size size;
		readonly double angle;
		readonly TextPathOptions options;
		public DiagramXtraPathCacheKey(int itemId, string text, Size size, double angle, TextPathOptions options) {
			this.itemId = itemId;
			this.size = size;
			this.angle = angle;
			this.text = text;
			this.options = options;
		}
		public override bool Equals(object obj) {
			DiagramXtraPathCacheKey other = (DiagramXtraPathCacheKey)obj;
			return IsEquals(other);
		}
		bool IsEquals(DiagramXtraPathCacheKey other) {
			return itemId == other.itemId && text == other.text && size == other.size && MathUtils.IsEquals(angle, other.angle) && options.Equals(other.options);
		}
		public override int GetHashCode() {
			return itemId.GetHashCode() ^ options.GetHashCode();
		}
	}
	public class DiagramXtraPathCache : IDisposable {
		Dictionary<DiagramXtraPathCacheKey, XtraGraphicsPath> items;
		public DiagramXtraPathCache() {
			this.items = new Dictionary<DiagramXtraPathCacheKey, XtraGraphicsPath>();
		}
		public XtraGraphicsPath GetPath(IXtraPathView view, TextPathOptions options, DiagramXtraPathCreateDelegate factory, out bool cacheHit) {
			XtraGraphicsPath result;
			DiagramXtraPathCacheKey key = new DiagramXtraPathCacheKey(view.ItemId, view.Text, view.Size, view.Angle, options);
			if(!items.TryGetValue(key, out result)) {
				items[key] = (result = factory(view, options));
				cacheHit = false;
			}
			else {
				cacheHit = true;
			}
			return result;
		}
		public void ClearCache() {
			if(this.items != null) {
				this.items.ForEachValue(image => image.Dispose());
				this.items.Clear();
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				ClearCache();
			}
			this.items = null;
		}
		#endregion
	}
	public delegate XtraGraphicsPath DiagramXtraPathCreateDelegate(IXtraPathView view, TextPathOptions options);
	public class DiagramItemDrawArgs {
		readonly Brush interiorBrush;
		readonly Pen borderPen;
		readonly Brush textBrush;
		readonly Color shadowColor;
		readonly bool drawShadow;
		readonly int shadowSize;
		readonly Rectangle itemBounds;
		readonly ShapeDescription shape;
		readonly DiagramPaintCache paintCache;
		readonly double angle;
		readonly DoubleCollection shapeParameters;
		public DiagramItemDrawArgs(Brush interiorBrush, Pen borderPen)
			: this(interiorBrush, borderPen, Brushes.Black) {
		}
		public DiagramItemDrawArgs(Brush interiorBrush, Pen borderPen, Brush textBrush)
			: this(interiorBrush, borderPen, textBrush, false, Color.Empty, 0, Rectangle.Empty, null, 0, null, null) {
		}
		public DiagramItemDrawArgs(Brush interiorBrush, Pen borderPen, Brush textBrush, bool drawShadow, Color shadowColor, int shadowSize, Rectangle itemBounds, ShapeDescription shape, double angle, DoubleCollection shapeParameters, DiagramPaintCache paintCache) {
			this.interiorBrush = interiorBrush;
			this.borderPen = borderPen;
			this.textBrush = textBrush;
			this.shadowColor = shadowColor;
			this.drawShadow = drawShadow;
			this.shadowSize = shadowSize;
			this.itemBounds = itemBounds;
			this.shape = shape;
			this.angle = angle;
			this.shapeParameters = shapeParameters;
			this.paintCache = paintCache;
		}
		public int GetShadowAlpha(int i) {
			return i + 5;
		}
		public Brush InteriorBrush { get { return interiorBrush; } }
		public Pen BorderPen { get { return borderPen; } }
		public Brush TextBrush { get { return textBrush; } }
		public bool DrawShadow { get { return drawShadow; } }
		public Color ShadowColor { get { return shadowColor; } }
		public int ShadowSize { get { return shadowSize; } }
		public ShapeDescription Shape { get { return shape; } }
		public Rectangle ItemBounds { get { return itemBounds; } }
		public Size ItemSize { get { return ItemBounds.Size; } }
		public DiagramPaintCache PaintCache { get { return paintCache; } }
		public double Angle { get { return angle; } }
		public DoubleCollection ShapeParameters { get { return shapeParameters; } }
	}
}
