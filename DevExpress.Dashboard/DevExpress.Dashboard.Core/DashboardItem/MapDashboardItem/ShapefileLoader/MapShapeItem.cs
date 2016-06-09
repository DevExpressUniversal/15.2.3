#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Linq;
using DevExpress.Map.Native;
using DevExpress.Map;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public class MapShapeItemAttribute : IMapItemAttribute {
		string name;
		object value;
		Type type;
		public string Name { get { return name;} set {name = value; } }
		public object Value { get { return value; } set { this.value = value; } }
		string IMapItemAttribute.Name { get { return name; } set { name = value; } }
		Type IMapItemAttribute.Type { get { return type; } set { type = value; } }
		object IMapItemAttribute.Value { get { return  value; } set { this.value = value; } }
		public MapShapeItemAttribute() {
		}
		public MapShapeItemAttribute(string name, object value, Type type) {
			this.name = name;
			this.value = value;
			this.type  = type;
		}
	}
	public class MapShapeItem : IMapItemCore, ISupportStyleCore {
		readonly List<MapShapeItemAttribute> attributes = new List<MapShapeItemAttribute>();
		internal Color? fill;
		internal Color? stroke;
		internal int? strokeWidth;
		public List<MapShapeItemAttribute> Attributes { get { return attributes; } }
		public Color Fill {
			get { return fill.HasValue ? fill.Value : Color.Empty; }
			set { fill = value; }
		}
		public Color Stroke {
			get { return stroke.HasValue ? stroke.Value : Color.Empty; }
			set { stroke = value; }
		}
		public int StrokeWidth {
			get { return strokeWidth.HasValue ? strokeWidth.Value : 0; }
			set { strokeWidth = value; }
		}
		#region ISupportStyleCore
		void ISupportStyleCore.SetStrokeWidth(double width) {
			int _width = Convert.ToInt32(width);
			StrokeWidth = _width;
		}
		void ISupportStyleCore.SetFill(Color color) {
			Fill = color;
		}
		void ISupportStyleCore.SetStroke(Color color) {
			Stroke = color;
		}
		#endregion
		#region IMapItemCore
		string IMapItemCore.Text { get { return string.Empty; } }
		Color IMapItemCore.TextColor { get { return Color.Empty; } }
		int IMapItemCore.AttributesCount { get { return Attributes.Count; } }
		IMapItemAttribute IMapItemCore.GetAttribute(int index) {
			return Attributes[index];
		}
		IMapItemAttribute IMapItemCore.GetAttribute(string name) {
			foreach(MapShapeItemAttribute attr in Attributes)
				if(attr.Name == name)
					return attr;
			return null;
		}
		#endregion
		internal MapShapeItem() { }
		public void AddAttribute(IMapItemAttribute attribute) {
			attributes.Add(new MapShapeItemAttribute(attribute.Name, attribute.Value, attribute.Type));
		}
		public MapShapeItem CloneWithFilteredAttributes(IEnumerable<string> filteredAttributes) {
			MapShapeItem clone = CreateInstance();
			FillInstance(clone);
			foreach(string relationAttributeName in filteredAttributes) {
				MapShapeItemAttribute relationAttribute = attributes.FirstOrDefault(a => a.Name == relationAttributeName);
				if(relationAttribute != null)
					clone.Attributes.Add(relationAttribute);
			}
			return clone;
		}
		protected virtual MapShapeItem CreateInstance() {
			return new MapShapeItem();
		}
		protected virtual void FillInstance(MapShapeItem item) {
			item.Fill = Fill;
			item.Stroke = Stroke;
			item.StrokeWidth = StrokeWidth;
		}
	}
	public class MapShapeDot : MapShapeItem {
		public double Latitude { get; private set; }
		public double Longitude { get; private set; }
		internal MapShapeDot(double latitude, double longitude) {
			Latitude = latitude;
			Longitude = longitude;
		}
		protected override MapShapeItem CreateInstance() {
			return new MapShapeDot(Latitude, Longitude);
		}
		public override bool Equals(object obj) {
			MapShapeDot dot = (MapShapeDot)obj;
			return Latitude == dot.Latitude && Longitude == dot.Longitude;
		}
		public override int GetHashCode() {
			return Latitude.GetHashCode() ^ Longitude.GetHashCode();
		}
	}
	public class MapShapePolyline : MapShapeItem, IPointContainerCore {
		readonly List<MapShapePoint> points = new List<MapShapePoint>();
		public List<MapShapePoint> Points { get { return points; } }
		internal MapShapePolyline() { }
		protected override MapShapeItem CreateInstance() {
			return new MapShapePath();
		}
		protected override void FillInstance(MapShapeItem item) {
			base.FillInstance(item);
			MapShapePolyline shapePolyline = (MapShapePolyline)item;
			shapePolyline.Points.AddRange(Points);
		}
		int IPointContainerCore.PointCount { get { return points.Count; } }
		void IPointContainerCore.AddPoint(CoordPoint point) {
			points.Add(new MapShapePoint(point.GetY(), point.GetX()));
		}
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return null;
		}
	}
	public class MapShapePath : MapShapeItem, IPathCore {
		readonly List<ShapePathSegment> segments = new List<ShapePathSegment>();
		public List<ShapePathSegment> Segments { get { return segments; } }
		public int SegmentCount { get { return segments.Count; } }
		internal MapShapePath() { }
		public IPathSegmentCore CreateSegment() {
			var segment = new ShapePathSegment();
			segments.Add(segment);
			return segment;
		}
		public IPathSegmentCore GetSegment(int index) {
			return segments[index];
		}
		protected override MapShapeItem CreateInstance() {
			return new MapShapePath();
		}
		protected override void FillInstance(MapShapeItem item) {
			base.FillInstance(item);
			MapShapePath pathItem = (MapShapePath)item;
			pathItem.Segments.AddRange(Segments);
		}
		public override bool Equals(object obj) {
			MapShapePath path = obj as MapShapePath;
			if(path == null) {
				return false;
			}
			return Helper.DataEquals(Segments, path.Segments);
		}
		public override int GetHashCode() {
			return Helper.GetDataHashCode(Segments);
		}
	}
	public class ShapePathSegment : IPathSegmentCore {
		readonly List<MapShapePoint> points = new List<MapShapePoint>();
		public List<MapShapePoint> Points { get { return points; } }
		public bool IsClosed { get; set; }
		public bool IsFilled { get; set; }
		public int PointCount { get { return points.Count; } }
		internal ShapePathSegment() { }
		public CoordPoint GetPoint(int index) {
			var MapPoint = points[index];
			return new GeoPointEx(MapPoint.Latitude, MapPoint.Longitude);
		}
		public void AddPoint(CoordPoint point) {
			points.Add(new MapShapePoint(point.GetY(), point.GetX()));
		}
		int IPathSegmentCore.InnerContourCount { get { return 0; } }
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		CoordBounds IPolygonCore.GetBounds() {
			return CoordBounds.Empty;
		}
		IPointContainerCore IPathSegmentCore.GetInnerCountour(int index) {
			return null;
		}
		public override bool Equals(object obj) {
			ShapePathSegment segment = obj as ShapePathSegment;
			if(segment == null) {
				return false;
			}
			return IsClosed == segment.IsClosed && IsFilled == segment.IsFilled && Helper.DataEquals(Points, segment.Points);
		}
		public IPointContainerCore CreateInnerContour() {
			return null;
		}
		public override int GetHashCode() {
			return IsClosed.GetHashCode() ^ IsFilled.GetHashCode() ^ Helper.GetDataHashCode(Points);
		}
	}
}
