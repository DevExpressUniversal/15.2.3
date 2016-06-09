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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using DevExpress.Utils.Design;
namespace DevExpress.Utils.Svg {
	public abstract class SvgElement {
		string fillCore;
		ISvgPaletteProvider colorPaletteProviderCore;
		public SvgElement() { }
		static ColorConverter converter = new ColorConverter();
		[SvgPropertyNameAliasAttribute("fill")]
		public string Fill {
			get {
				if(fillCore == null) {
					Color color = Color.FromArgb(255, Color.Black);
					fillCore = ColorTranslator.ToHtml(color);
				}
				return fillCore;
			}
			set { fillCore = value; }
		}
		[SvgPropertyNameAliasAttribute("opacity")]
		public string Opacity { get; set; }
		public float Scale { get; private set; }
		public float Offset { get; private set; }
		protected virtual Color GetColor() {
			Color baseColor = Color.Black;
			if(colorPaletteProviderCore != null)
				baseColor = colorPaletteProviderCore.GetColor(Fill);
			else
				baseColor = (Color)converter.ConvertFromString(Fill);
			double opacity = 1;
			if(Double.TryParse(Opacity, NumberStyles.Number, CultureInfo.InvariantCulture, out opacity)) {
				baseColor = Color.FromArgb((int)(opacity * 255), baseColor);
			}
			return baseColor;
		}
		public void Render(Graphics g, ISvgPaletteProvider paletteProvider, float scale, float offset) {
			colorPaletteProviderCore = paletteProvider;
			Scale = scale;
			Offset = offset;
			RenderCore(g, scale, offset);
		}
		public float ScaleValue(float v) {
			if(Scale == 1.0f)
				return v;
			return v * Scale;
		}
		internal float ScaleAndOffset(float v) {
			if(Scale == 1.0f)
				return v + Offset;
			return (v + Offset) * Scale;
		}
		internal PointF ScaleAndOffset(PointF point) {
			if(Scale == 1.0f)
				return new PointF(point.X + Offset, point.Y + Offset);
			return new PointF((point.X + Offset) * Scale, (point.Y + Offset) * Scale);
		}
		protected virtual void RenderCore(Graphics g, float scale, float offset) { }
	}
	public class SvgPathSegmentCollection : CollectionBase {
		int lockUpdateCore;
		public SvgPathSegment Last { get; private set; }
		public PointF LastPoint { get; private set; }
		public event CollectionChangeEventHandler CollectionChanged;
		public SvgPathSegmentCollection() {
			LastPoint = Point.Empty;
			lockUpdateCore = 0;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			OnElementAdded((SvgPathSegment)value);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			OnElementRemoved((SvgPathSegment)value);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		public SvgPathSegment this[int index] { get { return (SvgPathSegment)List[index]; } }
		public virtual void AddRange(SvgPathSegment[] segments) {
			BeginUpdate();
			try {
				foreach(SvgPathSegment segment in segments) {
					AddSegment(segment);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual bool Add(SvgPathSegment segment) {
			return AddSegment(segment);
		}
		public virtual int IndexOf(SvgPathSegment segment) { return List.IndexOf(segment); }
		public virtual bool Insert(int index, SvgPathSegment segment) {
			if(CanAdd(segment)) {
				List.Insert(index, segment);
				return true;
			}
			return false;
		}
		public virtual bool Remove(SvgPathSegment element) {
			if(List.Contains(element)) {
				List.Remove(element);
				return !List.Contains(element);
			}
			return false;
		}
		public virtual bool Contains(object element) { return List.Contains(element); }
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void AddSegmentInternal(SvgPathSegment segment) {
			List.Add(segment);
		}
		bool AddSegment(SvgPathSegment segment) {
			if(CanAdd(segment)) {
				List.Add(segment);
				return true;
			}
			return false;
		}
		protected virtual void OnElementAdded(SvgPathSegment segment) {
			Last = segment;
			LastPoint = segment.End;
		}
		protected virtual void OnElementRemoved(SvgPathSegment segment) { }
		protected override void OnClear() {
			base.OnClear();
			if(Count == 0) return;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--)
					RemoveAt(n);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdateCore != 0) return;
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
		public override string ToString() {
			return ToStringCore();
		}
		protected virtual string ToStringCore() {
			return string.Empty;
		}
		public void CopyTo(Array target, int index) {
			List.CopyTo(target, index);
		}
		protected bool CanAdd(SvgPathSegment element) {
			return List.IndexOf(element) < 0;
		}
		public virtual void BeginUpdate() {
			lockUpdateCore++;
		}
		public virtual void CancelUpdate() {
			lockUpdateCore--;
		}
		public virtual void EndUpdate() {
			if(--lockUpdateCore == 0)
				RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public SvgPathSegment[] ToArray() {
			return InnerList.ToArray(typeof(SvgPathSegment)) as SvgPathSegment[];
		}
		public void ForEach(Action<SvgPathSegment> action) {
			foreach(SvgPathSegment item in InnerList) {
				action(item);
			}
		}
		public SvgPathSegment[] CleanUp() {
			SvgPathSegment[] elements = ToArray();
			RemoveRange(elements);
			return elements;
		}
		public void RemoveRange(SvgPathSegment[] elements) {
			for(int i = 0; i < elements.Length; i++)
				Remove(elements[i]);
		}
		public SvgPathSegment FindFirst(Predicate<SvgPathSegment> match) {
			foreach(SvgPathSegment element in List) {
				if(!match(element)) continue;
				return element;
			}
			return default(SvgPathSegment);
		}
		public bool Contains(SvgPathSegment element) {
			return List.Contains(element);
		}
	}
	[SvgElementNameAlias("path")]
	public class SvgPath : SvgElement {
		SvgPathSegmentCollection segments;
		GraphicsPathCache paths;
		string pathDataCore;
		public SvgPath() {
			segments = new SvgPathSegmentCollection();
			paths = new GraphicsPathCache();
		}
		[SvgPropertyNameAliasAttribute("d")]
		public string PathData {
			get { return pathDataCore; }
			set { 
				pathDataCore = value;
				SVGPathParser.Parse(PathData, segments);
			}
		}
		protected void BuildGraphicsPath(GraphicsPath renderPath, float scale, float offset) { 
			segments.ForEach(x => x.AddToPath(renderPath, scale, offset));
		}
		protected override void RenderCore(Graphics g, float scale, float offset) {
			GraphicsPath renderPath;
			if(!paths.TryGetValue(scale, offset, out renderPath)) {
				renderPath = new GraphicsPath();
				BuildGraphicsPath(renderPath, scale, offset);
				paths.Add(new PathKey(scale, offset), renderPath);
			}
			g.FillPath(new SolidBrush(GetColor()), renderPath);
		}
		public PointF CurrentPoint { get; set; }
		class PathKey {
			public PathKey(float scale, float offset) {
				Scale = scale;
				Offset = offset;
			}
			public float Scale { get; set; }
			public float Offset { get; set; }
		}
		class GraphicsPathCache : Dictionary<PathKey, GraphicsPath> {
			public bool TryGetValue(float scale, float offset, out GraphicsPath path) {
				path = null;
				foreach(var item in Keys) {
					if(item.Scale == scale && item.Offset == offset) {
						path = this[item];
						return true;
					}
				}
				return false;
			}
		}
	}
	[SvgElementNameAlias("circle")]
	public class SvgCircle : SvgElement {
		float centerXCore;
		float radiusCore;
		float centerYCore;
		[SvgPropertyNameAliasAttribute("cx")]
		public float CenterX {
			get { return centerXCore; }
			set { centerXCore = value; }
		}
		[SvgPropertyNameAliasAttribute("cy")]
		public float CenterY {
			get { return centerYCore; }
			set { centerYCore = value; }
		}
		[SvgPropertyNameAliasAttribute("r")]
		public float Radius {
			get { return radiusCore; }
			set { radiusCore = value; }
		}
		protected override void RenderCore(Graphics g, float scale, float offset) {
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			path.AddEllipse(ScaleAndOffset(CenterX - Radius), ScaleAndOffset(CenterY - Radius),ScaleValue(Radius * 2),ScaleValue(Radius * 2));
			path.CloseFigure();
			g.FillPath(new SolidBrush(GetColor()), path);
		}
	}
	[SvgElementNameAlias("ellipse")]
	public class SvgEllipse : SvgElement {
		float centerXCore;
		float centerYCore;
		float radiusXCore;
		float radiusYCore;
		[SvgPropertyNameAliasAttribute("cx")]
		public float CenterX {
			get { return centerXCore; }
			set { centerXCore = value; }
		}
		[SvgPropertyNameAliasAttribute("cy")]
		public float CenterY {
			get { return centerYCore; }
			set { centerYCore = value; }
		}
		[SvgPropertyNameAliasAttribute("rx")]
		public float RadiusX {
			get { return radiusXCore; }
			set { radiusXCore = value; }
		}
		[SvgPropertyNameAliasAttribute("ry")]
		public float RadiusY {
			get { return radiusYCore; }
			set { radiusYCore = value; }
		}
		protected override void RenderCore(Graphics g, float scale, float offset) {
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			path.AddEllipse(ScaleAndOffset(CenterX - RadiusX), ScaleAndOffset(CenterY - RadiusY), ScaleValue(RadiusX * 2), ScaleValue(RadiusY * 2));
			path.CloseFigure();
			g.FillPath(new SolidBrush(GetColor()), path);
		}
	}
	[SvgElementNameAlias("line")]
	public class SvgLine : SvgElement {
		float startXCore;
		float startYCore;
		float endXCore;
		float endYCore;
		public float StartX {
			get { return startXCore; }
			set { startXCore = value; }
		}
		public float StartY {
			get { return startYCore; }
			set { startYCore = value; }
		}
		public float EndX {
			get { return endXCore; }
			set { endXCore = value; }
		}
		public float EndY {
			get { return endYCore; }
			set { endYCore = value; }
		}
		protected override void RenderCore(Graphics g, float scale, float offset) {
			g.DrawLine(new Pen(GetColor()),
				ScaleAndOffset(StartX),
				ScaleAndOffset(StartY),
				ScaleAndOffset(EndX),
				ScaleAndOffset(EndY));
		}
	}
	[SvgElementNameAlias("polygon")]
	public class SvgPolygon : SvgElement {
		string pointsCore;
		List<PointF> graphicsPoints = new List<PointF>();
		[SvgPropertyNameAliasAttribute("points")]
		public string Points {
			get { return pointsCore; }
			set {
				pointsCore = value;
				graphicsPoints.AddRange(CoordinateParser.GetPoints(pointsCore));
			}
		}
		protected override void RenderCore(Graphics g, float scale, float offset) {
			List<PointF> points = new List<PointF>();
			foreach(var item in graphicsPoints) {
				points.Add(new PointF(ScaleAndOffset(item.X), ScaleAndOffset(item.Y)));
			}
			var state = g.Save();
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			Color c = GetColor();
			g.FillPolygon(new SolidBrush(GetColor()), points.ToArray());
			g.Restore(state);
		}
	}
	[SvgElementNameAlias("polyline")]
	public class SvgPolyline : SvgPolygon {
	}
	[SvgElementNameAlias("rect")]
	public class SvgRectangle : SvgElement {
		float xCore;
		float yCore;
		float widthCore;
		float heightCore;
		float cornerRadiusXCore;
		float cornerRadiusYCore;
		[SvgPropertyNameAliasAttribute("x")]
		public float X {
			get { return xCore; }
			set { xCore = value; }
		}
		[SvgPropertyNameAliasAttribute("y")]
		public float Y {
			get { return yCore; }
			set { yCore = value; }
		}
		[SvgPropertyNameAliasAttribute("width")]
		public float Width {
			get { return widthCore; }
			set { widthCore = value; }
		}
		[SvgPropertyNameAliasAttribute("height")]
		public float Height {
			get { return heightCore; }
			set { heightCore = value; }
		}
		public float CornerRadiusX {
			get { return cornerRadiusXCore; }
			set { cornerRadiusXCore = value; }
		}
		public float CornerRadiusY {
			get { return cornerRadiusYCore; }
			set { cornerRadiusYCore = value; }
		}
		protected override void RenderCore(Graphics g, float scale, float offset) {
			g.FillRectangle(new SolidBrush(GetColor()), ScaleAndOffset(X), ScaleAndOffset(Y), ScaleValue(Width), ScaleValue(Height));
		}
	}
	[SvgElementNameAlias("g")]
	public class SvgGroup : SvgElement { }
	public class SvgElementNameAliasAttribute : Attribute {
		public SvgElementNameAliasAttribute(string name) {
			Name = name;
		}
		public string Name { get; private set; }
	}
	public class SvgPropertyNameAliasAttribute : Attribute {
		public SvgPropertyNameAliasAttribute(string name) {
			Name = name;
		}
		public string Name { get; private set; }
	}
}
