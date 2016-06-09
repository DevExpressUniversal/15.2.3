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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Map;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	public class MapPolygon : MapShape, ISupportCoordPoints, IPolygonCore {
		static ControlTemplate templatePolygon;
		static MapPolygon() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templatePolygon = XamlHelper.GetControlTemplate("<Polygon x:Name=\"PART_Shape\"/>");
		}
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty PointsProperty = DependencyPropertyManager.Register("Points",
		   typeof(CoordPointCollection), typeof(MapPolygon), new PropertyMetadata(null, PointsPropertyChanged));
		public static readonly DependencyProperty FillRuleProperty = DependencyPropertyManager.Register("FillRule",
		   typeof(FillRule), typeof(MapPolygon), new PropertyMetadata(FillRule.EvenOdd, AppearancePropertyChanged));
		Polygon Polygon {
			get {
				return Shape as Polygon;
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
		static void PointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPolygon shape = d as MapPolygon;
			if (shape != null) {
				CoordPointCollection oldCollection = e.OldValue as CoordPointCollection;
				if (oldCollection != null)
					oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(shape.PointsChanged);
				CoordPointCollection newCollection = e.NewValue as CoordPointCollection;
				if (newCollection != null)
					newCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(shape.PointsChanged);
				shape.UpdateLayout();
			}
		}
		protected internal override ControlTemplate ItemTemplate {
			get { 
				return templatePolygon; 
			}
		}
		protected double Area {
			get { return CentroidHelper.CalculatePolygonArea(this); }
		}
		public MapPolygon() {
			this.SetValue(PointsProperty, new CoordPointCollection());
			ApplyAppearance();
		}
		#region IPolygonCore implementation
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return Points[index];
		}
		int IPointContainerCore.PointCount { get { return Points.Count; } }
		CoordBounds IPolygonCore.GetBounds() {
			return CalculateBounds();
		}
		void IPointContainerCore.AddPoint(CoordPoint point) {
			Points.Add(point);
		}
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		#endregion
		void PointsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateLayout();
		}
		protected internal override void ApplyAppearance() {
			base.ApplyAppearance();
			if (Polygon != null)
				Polygon.FillRule = FillRule;
		}
		protected internal override CoordBounds CalculateBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected internal override CoordPoint GetCenterCore() {
			CoordPoint centroid = CentroidHelper.CalculatePolygonCentroid(CoordinateSystem.PointFactory, this, Area);
			return MathUtils.IsNumeric(centroid.GetX()) && MathUtils.IsNumeric(centroid.GetY()) ? centroid : base.GetCenterCore();
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return Points;
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPolygon();
		}
		IList<CoordPoint> ISupportCoordPoints.Points {
			get {
				List<CoordPoint> result = new List<CoordPoint>();
				if (Points != null)
					result.AddRange(Points);
				return result;
			}
		}
		protected override void CalculateLayoutInMapUnits() {
			List<MapUnit> pointsInMapUnits = new List<MapUnit>();
			MapUnit? leftTop = null, rightBottom = null;
			if (Points == null)
				return;
			foreach (CoordPoint coordPoint in Points) {
				MapUnit unit = CoordinateSystem.CoordPointToMapUnit(coordPoint);
				if (!leftTop.HasValue || !rightBottom.HasValue)
					leftTop = rightBottom = unit;
				else {
					leftTop = new MapUnit(Math.Min(leftTop.Value.X, unit.X), Math.Min(leftTop.Value.Y, unit.Y));
					rightBottom = new MapUnit(Math.Max(rightBottom.Value.X, unit.X), Math.Max(rightBottom.Value.Y, unit.Y));
				}
				pointsInMapUnits.Add(unit);
			}
			if (leftTop.HasValue) {
				Layout.LocationInMapUnits = leftTop.Value;
				Layout.SizeInMapUnits = new Size(rightBottom.Value.X - leftTop.Value.X, rightBottom.Value.Y - leftTop.Value.Y);
				Layout.Location = CoordinateSystem.MapUnitToCoordPoint(Layout.LocationInMapUnits);
			}
			PointCollection collection = new PointCollection();
			foreach (MapUnit unit in pointsInMapUnits)
				collection.Add(new Point((unit.X - Layout.LocationInMapUnits.X) * ShapeScale, (unit.Y - Layout.LocationInMapUnits.Y) * ShapeScale));
			if (Polygon != null)
				Polygon.Points = collection;
		}
	}
}
