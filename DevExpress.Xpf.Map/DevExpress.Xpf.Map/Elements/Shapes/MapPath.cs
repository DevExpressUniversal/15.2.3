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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapPath : MapShape, IPathCore, ISupportCoordPoints {
		static ControlTemplate templatePath;
		MapPathSegmentHelper segmentHelper;
		static MapPath() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templatePath = XamlHelper.GetControlTemplate("<Path x:Name=\"PART_Shape\"/>");
		}
		public static readonly DependencyProperty DataProperty = DependencyPropertyManager.Register("Data",
		   typeof(MapGeometry), typeof(MapPath), new PropertyMetadata(null, DataPropertyChanged));
		protected static void DataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPath path = d as MapPath;
			if (path != null) {
				IOwnedElement element = e.OldValue as IOwnedElement;
				if (element != null)
					element.Owner = null;
				element = e.NewValue as IOwnedElement;
				if (element != null)
					element.Owner = path;
			}
		}
		protected internal override ControlTemplate ItemTemplate {
			get {
				return templatePath;
			}
		}
		[Category(Categories.Layout)]
		public MapGeometry Data {
			get { return (MapGeometry)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public MapPath() {
			ApplyAppearance();
			this.segmentHelper = new MapPathSegmentHelper(this);
		}
		#region IPathCore
		int IPathCore.SegmentCount {
			get {
				if (Data is MapPathGeometry) {
					MapPathGeometry geometry = (MapPathGeometry)Data;
					return geometry.Figures.Count;
				}
				return 0;
			}
		}
		IPathSegmentCore IPathCore.CreateSegment() {
			if (Data is MapPathGeometry) {
				MapPathGeometry geometry = (MapPathGeometry)Data;
				MapPathFigure figure = new MapPathFigure();
				MapPolyLineSegment segment = new MapPolyLineSegment();
				figure.Segments.Add(segment);
				geometry.Figures.Add(figure);
				return segment;
			}
			return null;
		}
		IPathSegmentCore IPathCore.GetSegment(int index) {
			if (Data is MapPathGeometry) {
				MapPathGeometry geometry = (MapPathGeometry)Data;
				if (geometry.Figures.Count > index)
					if (geometry.Figures[index].Segments.Count > 0)
						return geometry.Figures[index].Segments[0] as IPathSegmentCore;
			}
			return null;
		}
		#endregion
		#region ISupportCoordPoints
		IList<CoordPoint> ISupportCoordPoints.Points { get { return ExtractCoordPoints(); } }
		#endregion
		protected internal override CoordBounds CalculateBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected internal override CoordPoint GetCenterCore() {
			CoordPoint center = center = segmentHelper.GetMaxSegmentCenter();
			return center ?? base.GetCenterCore();
		}
		protected internal override Size GetAvailableSizeForTitle() {
			Rect bounds = segmentHelper.GetMaxSegmentBounds();
			if(bounds.IsEmpty)
				return new Size();
			Point leftTop = Layer.MapUnitToScreenZeroOffset(new MapUnit(bounds.Left, bounds.Top));
			Point rightBottom = Layer.MapUnitToScreenZeroOffset(new MapUnit(bounds.Right, bounds.Bottom));
			return new Size(rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y);
		}
		protected internal override Rect CalculateTitleLayout(Size titleSize, VectorLayerBase layer) {
			MapPathGeometry pathGeometry = Data as MapPathGeometry;
			if(pathGeometry == null)
				return base.CalculateTitleLayout(titleSize, layer);
			CoordPoint center = segmentHelper.GetMaxSegmentCenter();
			if(center == null)
				return base.CalculateTitleLayout(titleSize, layer);
			Point centerPoint = layer.CoordPointToScreenPointZeroOffset(center, true, false);
			centerPoint = new Point(centerPoint.X - titleSize.Width / 2, centerPoint.Y - titleSize.Height / 2);
			return new Rect(centerPoint, titleSize);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return ExtractCoordPoints();
		}
		protected IList<CoordPoint> ExtractCoordPoints() {
			List<CoordPoint> result = new List<CoordPoint>();
			MapPathGeometry geometry = Data as MapPathGeometry;
			if (geometry != null) {
				foreach (MapPathFigure figure in geometry.Figures) { 
					IEnumerable<CoordPoint> segmentPoints = figure.Segments.SelectMany<MapPathSegment, CoordPoint>( 
						s => {
							ISupportCoordPoints supportPoints = s as ISupportCoordPoints;
							return supportPoints != null ? supportPoints.Points : new CoordPoint[0];
						}
					);
					result.AddRange(segmentPoints);
					result.Add(figure.StartPoint);
				}
			}
			return result;
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPath();
		}
		protected override void CalculateLayout() {
			base.CalculateLayout();
			segmentHelper.Reset();
		}
		protected override void OnOwnerChanged() {
			base.OnOwnerChanged();
			segmentHelper.UpdatePointFactory(CoordinateSystem.PointFactory);
		}
		protected override void CalculateLayoutInMapUnits() {
			Path path = Shape as Path;
			if (Data != null) {
				Data.UpdateGeometry(ShapeScale);
				Rect bounds = Data.GetBounds(ShapeScale);
				Layout.LocationInMapUnits = bounds.IsEmpty ? new MapUnit(0, 0) : new MapUnit(bounds.Left, bounds.Top);
				Layout.SizeInMapUnits = bounds.IsEmpty ? new Size(0, 0) : new Size(bounds.Width, bounds.Height);
				if (path != null)
					path.Data = Data.Geometry;
			}
		}
	}
	public abstract class MapPathElement : MapDependencyObject, IOwnedElement {
		protected static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPathElement geometry = d as MapPathElement;
			if (geometry != null)
				geometry.UpdateLayout();
		}
		protected static void AppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPathElement geometry = d as MapPathElement;
			if (geometry != null)
				geometry.ApplyAppearance();
		}
		object owner;
		protected VectorLayerBase Layer { get { return Shape != null ? Shape.Layer : null; } }
		protected ProjectionBase Projection {
			get {
				if (Layer == null)
					return MapControl.DefaultMapProjection;
				return Layer.ActualProjection;
			}
		}
		protected MapCoordinateSystem CoordinateSystem { get { return Layer != null ? Layer.ActualCoordinateSystem : MapControl.DefaultCoordinateSystem; } }
		protected internal abstract MapShape Shape { get; }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				owner = value;
				OwnerChanged();
			}
		}
		#endregion
		internal void UpdateLayout() {
			if (Shape != null)
				Shape.UpdateLayout();
		}
		protected virtual void OwnerChanged() {
			ApplyAppearance();
			UpdateLayout();
		}
		protected virtual void ApplyAppearance() {
		}
	}
	public abstract class MapGeometry : MapPathElement {
		protected internal override MapShape Shape { get { return ((IOwnedElement)this).Owner as MapShape; } }
		protected internal abstract Geometry Geometry { get; }
		protected internal abstract void UpdateGeometry(double shapeScale);
		protected Point Offset { get; set; }
		internal Rect GetBounds(double shapeScale) {
			if (Geometry != null && !Geometry.Bounds.IsEmpty)
				return new Rect(Offset, new Size(Geometry.Bounds.Width / shapeScale, Geometry.Bounds.Height / shapeScale));
			else
				return Rect.Empty;
		}
	}
	public class MapRectangleGeometry : MapGeometry {
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
			   typeof(CoordPoint), typeof(MapRectangleGeometry), new PropertyMetadata(new GeoPoint(0, 0), LayoutPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty HeightProperty = DependencyPropertyManager.Register("Height",
		  typeof(double), typeof(MapRectangleGeometry), new PropertyMetadata(0.0, LayoutPropertyChanged));
		public static readonly DependencyProperty WidthProperty = DependencyPropertyManager.Register("Width",
		  typeof(double), typeof(MapRectangleGeometry), new PropertyMetadata(0.0, LayoutPropertyChanged));
		static object CoerceLocation(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Location {
			get { return (CoordPoint)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		[Category(Categories.Layout)]
		public double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		[Category(Categories.Layout)]
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		RectangleGeometry geometry;
		protected internal override Geometry Geometry { get { return geometry; } }
		protected override MapDependencyObject CreateObject() {
			return new MapRectangleGeometry();
		}
		protected internal override void UpdateGeometry(double shapeScale) {
			Size size = CoordinateSystem.KilometersToGeoSize(Location, new Size(Width, Height));
			MapUnit leftTop = CoordinateSystem.CoordPointToMapUnit(Location);
			MapUnit rightBottom = CoordinateSystem.CoordPointToMapUnit(CoordinateSystem.PointFactory.CreatePoint(Location.GetX() + size.Width, Location.GetY() - size.Height));
			geometry = new RectangleGeometry() { Rect = new Rect(0.0, 0.0, (rightBottom.X - leftTop.X) * shapeScale, (rightBottom.Y - leftTop.Y) * shapeScale) };
			Offset = new Point(leftTop.X, leftTop.Y);
		}
	}
	public class MapEllipseGeometry : MapGeometry {
		public static readonly DependencyProperty CenterProperty = DependencyPropertyManager.Register("Center",
		   typeof(CoordPoint), typeof(MapEllipseGeometry), new PropertyMetadata(new GeoPoint(0, 0), LayoutPropertyChanged, CoerceCenter));
		public static readonly DependencyProperty RadiusYProperty = DependencyPropertyManager.Register("RadiusY",
		  typeof(double), typeof(MapEllipseGeometry), new PropertyMetadata(0.0, LayoutPropertyChanged));
		public static readonly DependencyProperty RadiusXProperty = DependencyPropertyManager.Register("RadiusX",
		  typeof(double), typeof(MapEllipseGeometry), new PropertyMetadata(0.0, LayoutPropertyChanged));
		static object CoerceCenter(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Center {
			get { return (CoordPoint)GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}
		[Category(Categories.Layout)]
		public double RadiusY {
			get { return (double)GetValue(RadiusYProperty); }
			set { SetValue(RadiusYProperty, value); }
		}
		[Category(Categories.Layout)]
		public double RadiusX {
			get { return (double)GetValue(RadiusXProperty); }
			set { SetValue(RadiusXProperty, value); }
		}
		EllipseGeometry geometry;
		protected internal override Geometry Geometry { get { return geometry; } }
		protected internal override void UpdateGeometry(double shapeScale) {
			Size size = CoordinateSystem.KilometersToGeoSize(Center, new Size(RadiusX, RadiusY));
			MapUnit leftTop = CoordinateSystem.CoordPointToMapUnit(CoordinateSystem.PointFactory.CreatePoint(Center.GetX() - size.Width, Center.GetY() + size.Height));
			MapUnit rightBottom = CoordinateSystem.CoordPointToMapUnit(CoordinateSystem.PointFactory.CreatePoint(Center.GetX() + size.Width, Center.GetY() - size.Height));
			double radiusX = 0.5 * (rightBottom.X - leftTop.X) * shapeScale;
			double radiusY = 0.5 * (rightBottom.Y - leftTop.Y) * shapeScale;
			geometry = new EllipseGeometry() { Center = new Point(radiusX, radiusY), RadiusX = radiusX, RadiusY = radiusY };
			Offset = new Point(leftTop.X, leftTop.Y);
		}
		protected override MapDependencyObject CreateObject() {
			return new MapEllipseGeometry();
		}
	}
	public class MapPathGeometry : MapGeometry {
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty FiguresProperty = DependencyPropertyManager.Register("Figures",
		   typeof(MapPathFigureCollection), typeof(MapPathGeometry), new PropertyMetadata(null, FiguresPropertyChanged));
		public static readonly DependencyProperty FillRuleProperty = DependencyPropertyManager.Register("FillRule",
		   typeof(FillRule), typeof(MapPathGeometry), new PropertyMetadata(FillRule.EvenOdd, AppearancePropertyChanged));
		[Category(Categories.Data)]
		public MapPathFigureCollection Figures {
			get { return (MapPathFigureCollection)GetValue(FiguresProperty); }
			set { SetValue(FiguresProperty, value); }
		}
		[Category(Categories.Appearance)]
		public FillRule FillRule {
			get { return (FillRule)GetValue(FillRuleProperty); }
			set { SetValue(FillRuleProperty, value); }
		}
		static void FiguresPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPathGeometry geometry = d as MapPathGeometry;
			if (geometry != null) {
				IOwnedElement element = e.OldValue as IOwnedElement;
				if (element != null)
					element.Owner = null;
				element = e.NewValue as IOwnedElement;
				if (element != null)
					element.Owner = geometry;
				geometry.UpdateLayout();
			}
		}
		PathGeometry geometry;
		protected internal override Geometry Geometry { get { return geometry; } }
		public MapPathGeometry() {
			geometry = new PathGeometry();
			this.SetValue(FiguresProperty, new MapPathFigureCollection());
		}
		protected override void ApplyAppearance() {
			base.ApplyAppearance();
			geometry.FillRule = FillRule;
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPathGeometry();
		}
		protected internal override void UpdateGeometry(double shapeScale) {
			if (Figures != null) {
				geometry = new PathGeometry { Figures = new PathFigureCollection() };
				double left = double.PositiveInfinity;
				double top = double.PositiveInfinity;
				foreach (MapPathFigure figure in Figures) {
					Point figureOffset = figure.UpdateFigureOffset();
					left = Math.Min(left, figureOffset.X);
					top = Math.Min(top, figureOffset.Y);
				}
				Offset = new Point(double.IsPositiveInfinity(left) ? 0 : left, double.IsPositiveInfinity(top) ? 0 : top);
				foreach (MapPathFigure figure in Figures) {
					figure.UpdateGeometry(Offset, shapeScale);
					geometry.Figures.Add(figure.Figure);
				}
			}
			else
				geometry = null;
		}
	}
	public class MapPathFigure : MapPathElement {
		public static readonly DependencyProperty IsClosedProperty = DependencyPropertyManager.Register("IsClosed",
		   typeof(bool), typeof(MapPathFigure), new PropertyMetadata(true, AppearancePropertyChanged));
		public static readonly DependencyProperty IsFilledProperty = DependencyPropertyManager.Register("IsFilled",
		   typeof(bool), typeof(MapPathFigure), new PropertyMetadata(true, AppearancePropertyChanged));
		public static readonly DependencyProperty StartPointProperty = DependencyPropertyManager.Register("StartPoint",
		   typeof(CoordPoint), typeof(MapPathFigure), new PropertyMetadata(new GeoPoint(0, 0), StartPointPropertyChanged, CoerceStartPoint));
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty SegmentsProperty = DependencyPropertyManager.Register("Segments",
		   typeof(MapSegmentCollection), typeof(MapPathFigure), new PropertyMetadata(null, SegmentsPropertyChanged));
		static object CoerceStartPoint(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		[Category(Categories.Appearance)]
		public bool IsClosed {
			get { return (bool)GetValue(IsClosedProperty); }
			set { SetValue(IsClosedProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool IsFilled {
			get { return (bool)GetValue(IsFilledProperty); }
			set { SetValue(IsFilledProperty, value); }
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint StartPoint {
			get { return (CoordPoint)GetValue(StartPointProperty); }
			set { SetValue(StartPointProperty, value); }
		}
		[Category(Categories.Data)]
		public MapSegmentCollection Segments {
			get { return (MapSegmentCollection)GetValue(SegmentsProperty); }
			set { SetValue(SegmentsProperty, value); }
		}
		static void StartPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPathFigure figure = d as MapPathFigure;
			if (figure != null)
				figure.UpdateLayout();
		}
		static void SegmentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPathFigure figure = d as MapPathFigure;
			if (figure != null) {
				IOwnedElement element = e.OldValue as IOwnedElement;
				if (element != null)
					element.Owner = null;
				element = e.NewValue as IOwnedElement;
				if (element != null)
					element.Owner = figure;
				figure.UpdateLayout();
			}
		}
		PathFigure figure = new PathFigure();
		MapUnit initialStartPoint;
		protected internal MapGeometry Geometry { get { return ((IOwnedElement)this).Owner as MapGeometry; } }
		protected internal override MapShape Shape { get { return Geometry != null ? Geometry.Shape : null; } }
		internal PathFigure Figure { get { return figure; } }
		public MapPathFigure() {
			this.SetValue(SegmentsProperty, new MapSegmentCollection());
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPathFigure();
		}
		protected override void ApplyAppearance() {
			base.ApplyAppearance();
			figure.IsClosed = IsClosed;
			figure.IsFilled = IsFilled;
		}
		internal void UpdateGeometry(Point offset, double shapeScale) {
			if (Layer != null) {
				figure = new PathFigure();
				figure.StartPoint = new Point((initialStartPoint.X - offset.X) * shapeScale, (initialStartPoint.Y - offset.Y) * shapeScale);
				if (Segments != null) {
					figure.Segments = new PathSegmentCollection();
					foreach (MapPathSegment segment in Segments) {
						segment.UpdateGeometry(offset, shapeScale);
						figure.Segments.Add(segment.Segment);
					}
				}
				else
					figure.Segments = null;
				ApplyAppearance();
			}
		}
		internal Point UpdateFigureOffset() {
			Point offset = new Point();
			if (Layer != null && Segments != null) {
				double left = double.PositiveInfinity;
				double top = double.PositiveInfinity;
				foreach (MapPathSegment segment in Segments) {
					Point segmentOffset = segment.UpdateSegmentOffset();
					left = Math.Min(left, segmentOffset.X);
					top = Math.Min(top, segmentOffset.Y);
				}
				initialStartPoint = CoordinateSystem.CoordPointToMapUnit(StartPoint);
				left = Math.Min(left, initialStartPoint.X);
				top = Math.Min(top, initialStartPoint.Y);
				offset = new Point(double.IsPositiveInfinity(left) ? 0 : left, double.IsPositiveInfinity(top) ? 0 : top);
			}
			return offset;
		}
	}
	public class MapPathFigureCollection : MapDependencyObjectCollection<MapPathFigure> {
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			MapGeometry geometry = Owner as MapGeometry;
			if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
				geometry.UpdateLayout();
			base.OnCollectionChanged(e);
		}
	}
	public abstract class MapPathSegment : MapPathElement {
		protected MapPathFigure Figure { get { return ((IOwnedElement)this).Owner as MapPathFigure; } }
		protected internal override MapShape Shape { get { return Figure != null ? Figure.Shape : null; } }
		protected internal abstract PathSegment Segment { get; }
		protected internal abstract void UpdateGeometry(Point offset, double shapeScale);
		protected internal abstract Point UpdateSegmentOffset();
	}
	public class MapSegmentCollection : MapDependencyObjectCollection<MapPathSegment> {
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			MapPathFigure figure = Owner as MapPathFigure;
			if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
				figure.UpdateLayout();
			base.OnCollectionChanged(e);
		}
	}
	public class MapPolyLineSegment : MapPathSegment, IPathSegmentCore, ISupportCoordPoints {
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty PointsProperty = DependencyPropertyManager.Register("Points",
		   typeof(CoordPointCollection), typeof(MapPolyLineSegment), new PropertyMetadata(null, PointsPropertyChanged));
		[Category(Categories.Data)]
		public CoordPointCollection Points {
			get { return (CoordPointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}
		static void PointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPolyLineSegment segment = d as MapPolyLineSegment;
			if (segment != null) {
				CoordPointCollection oldCollection = e.OldValue as CoordPointCollection;
				if (oldCollection != null)
					oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(segment.PointsChanged);
				CoordPointCollection newCollection = e.NewValue as CoordPointCollection;
				if (newCollection != null)
					newCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(segment.PointsChanged);
				segment.UpdateLayout();
			}
		}
		PolyLineSegment segment;
		List<MapUnit> initialUnits;
		CoordBounds bounds;
		protected internal override PathSegment Segment { get { return segment; } }
		internal Rect Bounds { get { return new Rect(bounds.X1, bounds.Y1, bounds.Width, bounds.Height); } }
		public MapPolyLineSegment() {
			this.SetValue(PointsProperty, new CoordPointCollection());
		}
		#region IPathSegmentCore
		bool IPathSegmentCore.IsClosed {
			get {
				return Figure.IsClosed;
			}
			set {
				Figure.IsClosed = value;
			}
		}
		bool IPathSegmentCore.IsFilled {
			get {
				return Figure.IsFilled;
			}
			set {
				Figure.IsFilled = value;
			}
		}
		int IPathSegmentCore.InnerContourCount {
			get { return 0; }
		}
		int IPointContainerCore.PointCount {
			get { return Points.Count; }
		}
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return Points[index];
		}
		void IPointContainerCore.AddPoint(CoordPoint point) {
			if (Points.Count == 0)
				Figure.StartPoint = point;
			Points.Add(point);   
		}
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		IPointContainerCore IPathSegmentCore.GetInnerCountour(int index) {
			return null;
		}
		IPointContainerCore IPathSegmentCore.CreateInnerContour() {
			if(Figure != null) {
				MapPathGeometry geometry = Figure.Geometry as MapPathGeometry;
				if(geometry != null) {
					MapPathFigure figure = new MapPathFigure();
					MapPolyLineSegment segment = new MapPolyLineSegment();
					figure.Segments.Add(segment);
					geometry.Figures.Add(figure);
					return segment;
				}
			}
			return null; 
		}
		CoordBounds IPolygonCore.GetBounds() {
			return bounds;
		}
		#endregion
		IList<CoordPoint> ISupportCoordPoints.Points {
			get {
				List<CoordPoint> result = new List<CoordPoint>();
				if (Points != null)
					result.AddRange(Points);
				return result;
			}
		}
		void PointsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateLayout();
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPolyLineSegment();
		}
		protected internal override void UpdateGeometry(Point offset, double shapeScale) {
			segment = new PolyLineSegment();
			PointCollection collection = new PointCollection();
			if (Layer != null && Points != null && initialUnits != null) {
				foreach (MapUnit mapUnit in initialUnits)
					collection.Add(new Point((mapUnit.X - offset.X) * shapeScale, (mapUnit.Y - offset.Y) * shapeScale));
			}
			segment.Points = collection;
		}
		protected internal override Point UpdateSegmentOffset() {
			Point offset = new Point();
			if (Layer != null && Points != null) {
				initialUnits = new List<MapUnit>();
				double left = double.PositiveInfinity;
				double top = double.PositiveInfinity;
				double right = double.NegativeInfinity;
				double bottom = double.NegativeInfinity;
				foreach (CoordPoint point in Points) {
					MapUnit unit = CoordinateSystem.CoordPointToMapUnit(point);
					initialUnits.Add(unit);
					left = Math.Min(unit.X, left);
					top = Math.Min(unit.Y, top);
					right = Math.Max(unit.X, right);
					bottom = Math.Max(unit.Y, bottom);
				}
				this.bounds = new CoordBounds(left, top, right, bottom);
				offset = new Point(double.IsPositiveInfinity(left) ? 0 : left, double.IsPositiveInfinity(top) ? 0 : top);
			}
			return offset;
		}
	}
}
