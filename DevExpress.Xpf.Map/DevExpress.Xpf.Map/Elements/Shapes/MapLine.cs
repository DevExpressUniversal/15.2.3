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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using DevExpress.Map;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
using System.Windows.Media;
namespace DevExpress.Xpf.Map {
	public class MapLine : MapShape, ISupportCoordPoints, ISupportIntermediatePoints {
		const bool DefaultIsGeodesic = false;
		static ControlTemplate templateLine;
		public static readonly DependencyProperty Point1Property = DependencyPropertyManager.Register("Point1",
			typeof(CoordPoint), typeof(MapLine), new PropertyMetadata(new GeoPoint(0, 0), LineLayoutPropertyChanged, CoercePoint1));
		public static readonly DependencyProperty Point2Property = DependencyPropertyManager.Register("Point2",
		  typeof(CoordPoint), typeof(MapLine), new PropertyMetadata(new GeoPoint(0, 0), LineLayoutPropertyChanged, CoercePoint2));
		public static readonly DependencyProperty IsGeodesicProperty = DependencyPropertyManager.Register("IsGeodesic",
			typeof(bool), typeof(MapLine), new PropertyMetadata(DefaultIsGeodesic, LineLayoutPropertyChanged));
		static MapLine() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templateLine = XamlHelper.GetControlTemplate("<Path x:Name=\"PART_Shape\" Fill =\"{x:Null}\" ><Path.Data><PathGeometry FillRule=\"EvenOdd\"/></Path.Data></Path>");
		}
		static object CoercePoint1(DependencyObject d, object baseValue) {
			if(baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		static object CoercePoint2(DependencyObject d, object baseValue) {
			if(baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		static void LineLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapLine line = d as MapLine;
			if(line != null) {
				line.NeedPopulateActualPoints = true;
				line.UpdateLayout();
			}
		}
		bool needPopulateActualPoints = true;
		IList<IList<CoordPoint>> actualPoints;
		Path Line { get { return Shape as Path; } }
		protected internal override ControlTemplate ItemTemplate {
			get {
				return templateLine;
			}
		}
		protected internal bool ShouldUseStraightLine {
			get { return !IsGeodesic || CoordinateSystem.PointType != CoordPointType.Geo; }
		}
		protected bool NeedPopulateActualPoints {
			get { return needPopulateActualPoints; }
			set { needPopulateActualPoints = value; }
		}
		protected internal IList<IList<CoordPoint>> ActualVertices {
			get {
				if(NeedPopulateActualPoints)
					this.actualPoints = PopulateActualPoints();
				return actualPoints;
			}
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Point1 {
			get { return (CoordPoint)GetValue(Point1Property); }
			set { SetValue(Point1Property, value); }
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Point2 {
			get { return (CoordPoint)GetValue(Point2Property); }
			set { SetValue(Point2Property, value); }
		}
		[Category(Categories.Layout)]
		public bool IsGeodesic {
			get { return (bool)GetValue(IsGeodesicProperty); }
			set { SetValue(IsGeodesicProperty, value); }
		}
		public IEnumerable<CoordPoint> ActualPoints { get { return ActualVertices.SelectMany(x => x); } }
		protected override MapDependencyObject CreateObject() {
			return new MapLine();
		}
		public MapLine() {
			ApplyAppearance();
		}
		#region ISupportCoordPoints implementation
		IList<CoordPoint> ISupportCoordPoints.Points { get { return new List<CoordPoint>() { Point1, Point2 }; } }
		#endregion
		#region ISupportIntermediatePoints implementation
		IList<CoordPoint> ISupportIntermediatePoints.Vertices { get { return ActualPoints.ToList(); } }
		#endregion
		IList<IList<CoordPoint>> PopulateActualPoints() {
			NeedPopulateActualPoints = false;
			if(ShouldUseStraightLine)
				return new CoordPointCollection[] { new CoordPointCollection { Point1, Point2 } };
			return PopulateGeodesicPoints();
		}
		IList<IList<CoordPoint>> PopulateGeodesicPoints() {
			return OrthodromeHelper.CalculateLine(Point1, Point2);
		}
		protected override Brush GetFill() {
			return Fill;
		}
		protected internal override CoordBounds CalculateBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected internal override CoordPoint GetCenterCore() {
			return CoordinateSystem.PointFactory.CreatePoint(0.5 * (Point1.GetX() + Point2.GetX()), 0.5 * (Point1.GetY() + Point2.GetY()));
		}
		protected override void CalculateLayoutInMapUnits() {
			List<List<MapUnit>> pointsInMapUnits = new List<List<MapUnit>>();
			MapUnit? leftTop = null, rightBottom = null;
			for(int i = 0; i < ActualVertices.Count; i++) {
				pointsInMapUnits.Add(new List<MapUnit>());
				foreach(CoordPoint geoPoint in ActualVertices[i]) {
					MapUnit unit = CoordinateSystem.CoordPointToMapUnit(geoPoint);
					if(!leftTop.HasValue || !rightBottom.HasValue)
						leftTop = rightBottom = unit;
					else {
						leftTop = new MapUnit(Math.Min(leftTop.Value.X, unit.X), Math.Min(leftTop.Value.Y, unit.Y));
						rightBottom = new MapUnit(Math.Max(rightBottom.Value.X, unit.X), Math.Max(rightBottom.Value.Y, unit.Y));
					}
					pointsInMapUnits[i].Add(unit);
				}
			}
			if(leftTop.HasValue) {
				Layout.LocationInMapUnits = leftTop.Value;
				Layout.SizeInMapUnits = new Size(rightBottom.Value.X - leftTop.Value.X, rightBottom.Value.Y - leftTop.Value.Y);
				Layout.Location = CoordinateSystem.MapUnitToCoordPoint(Layout.LocationInMapUnits);
			}
			IList<PointCollection> collection = new List<PointCollection>();
			for(int i = 0; i < pointsInMapUnits.Count; i++) {
				collection.Add(new PointCollection());
				foreach(MapUnit unit in pointsInMapUnits[i])
					collection[i].Add(new Point((unit.X - Layout.LocationInMapUnits.X) * ShapeScale, (unit.Y - Layout.LocationInMapUnits.Y) * ShapeScale));
			}
			if(Line != null)
				PopulateInnerPointsCollection(collection);
		}
		void PopulateInnerPointsCollection(IList<PointCollection> collection) {
			PathFigureCollection figures = new PathFigureCollection();
			foreach(PointCollection points in collection)
				if(points.Count > 0)
					figures.Add(new PathFigure() { StartPoint = points[0], Segments = new PathSegmentCollection() { new PolyLineSegment() { Points = points } } });
			Line.Data = new PathGeometry(figures);
		}
		protected override void CalculateLayout() {
			MapUnit rectPoint1 = Layout.LocationInMapUnits;
			MapUnit rectPoint2 = new MapUnit(Layout.LocationInMapUnits.X + Layout.SizeInMapUnits.Width, Layout.LocationInMapUnits.Y + Layout.SizeInMapUnits.Height);
			Point p1 = Layer.MapUnitToScreenZeroOffset(rectPoint1);
			Point p2 = Layer.MapUnitToScreenZeroOffset(rectPoint2);
			if(ActualStrokeStyle != null && ActualStrokeStyle.Thickness > 0) {
				double halfThickness = ActualStrokeStyle.Thickness * 0.5;
				p1.X -= halfThickness;
				p2.X += halfThickness;
				p1.Y -= halfThickness;
				p2.Y += halfThickness;
			}
			double strokeThickness = ActualStrokeStyle != null ? ActualStrokeStyle.Thickness : 0.0;
			Layout.SizeInPixels = new Size(Math.Max(strokeThickness, Math.Abs(p2.X - p1.X)), Math.Max(strokeThickness, Math.Abs(p2.Y - p1.Y)));
			Layout.LocationInPixels = p1;
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new List<CoordPoint>() { Point1, Point2 };
		}
	}
}
