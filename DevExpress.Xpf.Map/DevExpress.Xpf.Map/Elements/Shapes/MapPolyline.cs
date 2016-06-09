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
	public class MapPolyline : MapShape, IPointContainerCore, ISupportCoordPoints, ISupportIntermediatePoints {
		const bool DefaultIsGeodesic = false;
		static ControlTemplate templatePolyline;
		static MapPolyline() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templatePolyline = XamlHelper.GetControlTemplate("<Path x:Name=\"PART_Shape\" Fill =\"{x:Null}\" ><Path.Data><PathGeometry FillRule=\"EvenOdd\"/></Path.Data></Path>");
		}
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty PointsProperty = DependencyPropertyManager.Register("Points",
		   typeof(CoordPointCollection), typeof(MapPolyline), new PropertyMetadata(null, PointsPropertyChanged));
		public static readonly DependencyProperty FillRuleProperty = DependencyPropertyManager.Register("FillRule",
		   typeof(FillRule), typeof(MapPolyline), new PropertyMetadata(FillRule.EvenOdd, AppearancePropertyChanged));
		public static readonly DependencyProperty IsGeodesicProperty = DependencyPropertyManager.Register("IsGeodesic",
			typeof(bool), typeof(MapPolyline), new PropertyMetadata(DefaultIsGeodesic, IsGeodesicPropertyChanged));
		static void PointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPolyline shape = d as MapPolyline;
			if (shape != null) {
				shape.NeedPopulateActualPoints = true;
				CoordPointCollection oldCollection = e.OldValue as CoordPointCollection;
				if (oldCollection != null)
					oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(shape.PointsChanged);
				CoordPointCollection newCollection = e.NewValue as CoordPointCollection;
				if (newCollection != null)
					newCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(shape.PointsChanged);
				shape.UpdateLayout();
			}
		}
		static void IsGeodesicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPolyline polyline = d as MapPolyline;
			if(polyline != null) {
				polyline.NeedPopulateActualPoints = true;
				polyline.UpdateLayout();
			}
		}
		Path Polyline {
			get {
				return Shape as Path;
			}
		}
		[Category(Categories.Layout)]
		public CoordPointCollection Points {
			get { return (CoordPointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}
		[Category(Categories.Appearance)]
		public FillRule FillRule {
			get { return (FillRule)GetValue(FillRuleProperty); }
			set { SetValue(FillRuleProperty, value); }
		}
		[Category(Categories.Layout)]
		public bool IsGeodesic {
			get { return (bool)GetValue(IsGeodesicProperty); }
			set { SetValue(IsGeodesicProperty, value); }
		}
		public IEnumerable<CoordPoint> ActualPoints { get { return ActualVertices.SelectMany(x => x); } }
		readonly List<MapUnit> pointsInMapUnits = new List<MapUnit>();
		IList<IList<CoordPoint>> actualPoints;
		protected internal override ControlTemplate ItemTemplate {
			get {
				return templatePolyline;
			}
		}
		protected internal bool ShouldUseStraightLine {
			get { return !IsGeodesic || CoordinateSystem.PointType != CoordPointType.Geo; }
		}
		protected bool NeedPopulateActualPoints { get; set; }
		protected internal IList<IList<CoordPoint>> ActualVertices {
			get {
				if(NeedPopulateActualPoints)
					this.actualPoints = PopulateActualPoints();
				return actualPoints;
			}
		}
		public MapPolyline() {
			this.SetValue(PointsProperty, new CoordPointCollection());
			ApplyAppearance();
		}
		#region IPointContainerCore implementation
		int IPointContainerCore.PointCount { get { return Points.Count; } }
		void IPointContainerCore.AddPoint(CoordPoint point) {
			Points.Add(point);
		}
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return Points[index];
		}
		#endregion
		#region ISupportCoordPoints implementation
		IList<CoordPoint> ISupportCoordPoints.Points {
			get { return Points != null ? new List<CoordPoint>(Points) : new List<CoordPoint>(); } 
		}
		#endregion
		#region ISupportIntermediatePoints implementation
		IList<CoordPoint> ISupportIntermediatePoints.Vertices { get { return ActualPoints.ToList(); } }
		#endregion
		void PointsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			NeedPopulateActualPoints = true;
			UpdateLayout();
		}
		IList<IList<CoordPoint>> PopulateActualPoints() {
			NeedPopulateActualPoints = false;
			if(ShouldUseStraightLine)
				return new CoordPointCollection[1] { Points ?? new CoordPointCollection() };
			return PopulateGeodesicPoints();
		}
		IList<IList<CoordPoint>> PopulateGeodesicPoints() {
			return OrthodromeHelper.CalculateLine(Points);
		}
		protected override Brush GetFill() {
			return Fill;
		}
		protected internal override void ApplyAppearance() {
			base.ApplyAppearance();
			if (Polyline != null) {
				PathGeometry geometry = (PathGeometry)Polyline.Data;
				Polyline.Data = new PathGeometry(geometry.Figures, FillRule, geometry.Transform);
			}
		}
		protected internal override void ApplyHighlightAppearance() {
			base.ApplyHighlightAppearance();
			if (Polyline != null)
				Polyline.Fill = HighlightFill;
		}
		protected internal override CoordBounds CalculateBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected internal override CoordPoint GetCenterCore() {
			IList<CoordPoint> actualPoints = ActualPoints.ToList();
			int middlePointIdx = actualPoints.Count / 2;
			if(actualPoints.Count % 2 == 1)
				return actualPoints[middlePointIdx];
			else
				return CoordinateSystem.PointFactory.CreatePoint((actualPoints[middlePointIdx].GetX() + actualPoints[middlePointIdx - 1].GetX()) / 2.0, (actualPoints[middlePointIdx].GetY() + actualPoints[middlePointIdx - 1].GetY()) / 2.0);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return Points;
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPolyline();
		}
		protected override void CalculateLayoutInMapUnits() {
			if (Points != null) {
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
				if (leftTop.HasValue) {
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
				if (Polyline != null)
					PopulateInnerPointsCollection(collection);
			}
		}
		void PopulateInnerPointsCollection(IList<PointCollection> collection) {
			PathFigureCollection figures= new PathFigureCollection();
			foreach(PointCollection points in collection)
				if(points.Count > 0)
					figures.Add(new PathFigure() { StartPoint = points[0], Segments = new PathSegmentCollection() { new PolyLineSegment() { Points = points } } });
			((PathGeometry)Polyline.Data).Figures = figures;
		}
		protected override void CalculateLayout() {
			MapUnit rectPoint1 = Layout.LocationInMapUnits;
			MapUnit rectPoint2 = new MapUnit(Layout.LocationInMapUnits.X + Layout.SizeInMapUnits.Width, Layout.LocationInMapUnits.Y + Layout.SizeInMapUnits.Height);
			Point p1 = Layer.MapUnitToScreenZeroOffset(rectPoint1);
			Point p2 = Layer.MapUnitToScreenZeroOffset(rectPoint2);
			if (ActualStrokeStyle != null && ActualStrokeStyle.Thickness > 1) {
				p1.X -= ActualStrokeStyle.Thickness * 0.5;
				p1.Y -= ActualStrokeStyle.Thickness * 0.5;
				p2.X += ActualStrokeStyle.Thickness * 0.5;
				p2.Y += ActualStrokeStyle.Thickness * 0.5;
			}
			double strokeThickness = ActualStrokeStyle != null ? ActualStrokeStyle.Thickness : 0.0;
			Layout.SizeInPixels = new Size(Math.Max(strokeThickness, Math.Abs(p2.X - p1.X)), Math.Max(strokeThickness, Math.Abs(p2.Y - p1.Y)));
			Layout.LocationInPixels = p1;
		}
	}
}
