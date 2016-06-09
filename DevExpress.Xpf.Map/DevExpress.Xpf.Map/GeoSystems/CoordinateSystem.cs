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
using System.ComponentModel;
using DevExpress.Xpf.Map.Native;
using DevExpress.Map.Native;
using DevExpress.Map;
using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class MapCoordinateSystem : MapDependencyObject {
		readonly MapCoordinateSystemCore coordSystemCore;
		protected internal MapCoordinateSystemCore CoordSystemCore { get { return coordSystemCore; } }
		protected internal CoordObjectFactory PointFactory { get { return coordSystemCore.PointFactory; } }
		protected internal CoordPointType PointType { get { return coordSystemCore.PointType; } }
		protected internal CoordPoint Center { get { return coordSystemCore.Center; } }
		protected internal virtual CoordBounds BoundingBox { get { return coordSystemCore.BoundingBox; } }
		protected internal abstract bool NeedUpdateBoundingBox { get; }
		protected MapCoordinateSystem() {
			this.coordSystemCore = CreateCoordSystemCore();
		}
		protected abstract MapCoordinateSystemCore CreateCoordSystemCore();
		protected internal MapUnit CoordPointToMapUnit(CoordPoint pt, bool shouldNormalize) {
			return new MapUnit(coordSystemCore.CoordPointToMapUnit(pt, shouldNormalize));
		}
		protected internal virtual MapUnit CoordPointToMapUnit(CoordPoint pt) {
			return new MapUnit(coordSystemCore.CoordPointToMapUnit(pt));
		}
		protected internal virtual CoordPoint MapUnitToCoordPoint(MapUnit mapUnit) {
			return coordSystemCore.MapUnitToCoordPoint(mapUnit);
		}
		protected internal MapUnit ScreenPointToMapUnit(Point anchorPoint, Rect viewport, Size viewportInPixels) {
			IMapUnit unit = coordSystemCore.ScreenPointToMapUnit(new MapPointCore(anchorPoint.X, anchorPoint.Y), new MapRectCore(viewport.Left, viewport.Top, viewport.Width, viewport.Height), new MapSizeCore(viewportInPixels.Width, viewportInPixels.Height));
			return new MapUnit(unit);
		}
		protected internal Point MapUnitToScreenPoint(MapUnit unit, Rect viewport, Size viewportInPixels) {
			MapPointCore point = coordSystemCore.MapUnitToScreenPoint(unit, new MapRectCore(viewport.Left, viewport.Top, viewport.Width, viewport.Height), new MapSizeCore(viewportInPixels.Width, viewportInPixels.Height));
			return new Point(point.X, point.Y);
		}
		protected internal CoordPoint ScreenPointToCoordPoint(Point point, Rect viewport, Size viewportInPixels) {
			MapUnit unit = ScreenPointToMapUnit(point, viewport, viewportInPixels);
			return MapUnitToCoordPoint(unit);
		}
		protected internal double CalculateKilometersScale(CoordPoint anchorPoint, double size) {
			return coordSystemCore.CalculateKilometersScale(anchorPoint, size);
		}
		protected internal virtual Size GeoToKilometersSize(CoordPoint anchorPoint, Size size) {
			MapSizeCore result = coordSystemCore.GeoToKilometersSize(anchorPoint, new MapSizeCore(size.Width, size.Height));
			return new Size(result.Width, result.Height);
		}
		protected internal virtual Size KilometersToGeoSize(CoordPoint anchorPoint, Size size) {
			MapSizeCore result = coordSystemCore.KilometersToGeoSize(anchorPoint, new MapSizeCore(size.Width, size.Height));
			return new Size(result.Width, result.Height);
		}
		protected internal CoordPoint MeasurePoint(CoordPoint point) {
			return coordSystemCore.MeasurePoint(point);
		}
		protected internal CoordPoint UnmeasurePoint(CoordPoint point) {
			return coordSystemCore.UnmeasurePoint(point);
		}
		protected internal Rect CalculateViewport(double zoomLevel, CoordPoint centerPoint, Size viewportInPixels, Size initialMapSize) {
			MapRectCore rect = coordSystemCore.CalculateViewport(zoomLevel, centerPoint, new MapSizeCore(viewportInPixels.Width, viewportInPixels.Height), new MapSizeCore(initialMapSize.Width, initialMapSize.Height));
			return rect.IsValid ? new Rect(rect.Left, rect.Top, rect.Width, rect.Height) : Rect.Empty;
		}
		internal Rect CalculateOffsetViewport(double zoomLevel, Size viewportInPixels, Size initialMapSize) {
			return CalculateViewport(zoomLevel, PointFactory.CreatePoint(BoundingBox.X1, BoundingBox.Y1), viewportInPixels, initialMapSize); 
		}
		protected internal bool IsEqual(MapCoordinateSystem system) {
			return coordSystemCore.IsEqual(system.coordSystemCore);
		}
		protected internal virtual void ResetBoundingBox() { }
		protected internal virtual void UpdateBoundingBox(CoordBounds bounds) { }
		protected internal virtual void SetNeedUpdateBoundingBox(bool needUpdate) { }
		protected internal virtual void CorrectBoundingBox() { }
		protected internal CoordPoint CreateNormalizedPoint(CoordPoint centerPoint) {
			return coordSystemCore.CreateNormalizedPoint(centerPoint);
		}
		protected internal Point CoordToScreenPointIdentity(Size viewportInPixels, CoordPoint point, Size mapSizeInPixels) {
			MapUnit unit = CoordPointToMapUnit(point, true);
			Rect viewport = CalculateViewport(1.0, Center, viewportInPixels, mapSizeInPixels);
			return MapUnitToScreenPoint(unit, viewport, viewportInPixels);
		}
		public abstract CoordPoint CreatePoint(double x, double y);
		public abstract CoordPoint CreatePoint(string input);
	}
	public class GeoMapCoordinateSystem : MapCoordinateSystem, ISupportProjection {
		public static readonly DependencyProperty ProjectionProperty = DependencyPropertyManager.Register("Projection",
			typeof(ProjectionBase), typeof(GeoMapCoordinateSystem), new PropertyMetadata(ProjectionBase.DefaultProjection, ProjectionChanged, CoerceProjection));
		static object CoerceProjection(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return ProjectionBase.DefaultProjection;
			return baseValue;
		}
		static void ProjectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GeoMapCoordinateSystem coordSystem = d as GeoMapCoordinateSystem;
			if(coordSystem != null) {
				coordSystem.CoordSystemCore.Projection = coordSystem.ProjectionCore;
			}
		}
		[Category(Categories.Behavior)]
		public ProjectionBase Projection {
			get { return (ProjectionBase)GetValue(ProjectionProperty); }
			set { SetValue(ProjectionProperty, value); }
		}
		void ResetProjection() {
			Projection = ProjectionBase.DefaultProjection;
		}
		protected GeoPoint ToGeoPoint(CoordPoint pt) {
			GeoPoint point = pt as GeoPoint;
			double normalizedY = Math.Max(Math.Min(pt.GetY(), 90.0), -90.0);
			return point != null ? point : (GeoPoint)CreatePoint(pt.GetX(), normalizedY);
		}
		protected new MapGeoCoordinateSystemCore CoordSystemCore { get { return (MapGeoCoordinateSystemCore)base.CoordSystemCore; } }
		protected ProjectionBaseCore ProjectionCore {
			get {
				PredefinedProjectionBase projection = Projection as PredefinedProjectionBase;
				return projection != null ? projection.ProjectionCore : null;
			}
		}
		protected internal override CoordBounds BoundingBox { get { return ProjectionCore != null ? base.BoundingBox : Projection.GetBoundingBox(); } }
		protected internal override bool NeedUpdateBoundingBox { get { return false; } }
		protected override MapCoordinateSystemCore CreateCoordSystemCore() {
			return new MapGeoCoordinateSystemCore(GeoPointFactory.Instance);
		}
		protected internal override MapUnit CoordPointToMapUnit(CoordPoint pt) {
			return ProjectionCore != null ? base.CoordPointToMapUnit(ToGeoPoint(pt)) : Projection.GeoPointToMapUnit(ToGeoPoint(pt));
		}
		protected internal override CoordPoint MapUnitToCoordPoint(MapUnit mapUnit) {
			return ProjectionCore != null ? base.MapUnitToCoordPoint(mapUnit) : Projection.MapUnitToGeoPoint(mapUnit);
		}
		protected internal override Size GeoToKilometersSize(CoordPoint anchorPoint, Size size) {
			return ProjectionCore != null ? base.GeoToKilometersSize(ToGeoPoint(anchorPoint), size) : Projection.GeoToKilometersSize(ToGeoPoint(anchorPoint), size);
		}
		protected internal override Size KilometersToGeoSize(CoordPoint anchorPoint, Size size) {
			return ProjectionCore != null ? base.KilometersToGeoSize(ToGeoPoint(anchorPoint), size) : Projection.KilometersToGeoSize(ToGeoPoint(anchorPoint), size);
		}
		protected override MapDependencyObject CreateObject() {
			return new GeoMapCoordinateSystem();
		}
		public override CoordPoint CreatePoint(double x, double y) {
			return PointFactory.CreatePoint(x, y);
		}
		public override CoordPoint CreatePoint(string input) {
			return GeoPoint.Parse(input);
		}
	}
	public class CartesianMapCoordinateSystem : MapCoordinateSystem {
		static readonly MeasureUnit DefaultMeasureUnit = MapCartesianCoordinateSystemCore.DefaultMeasureUnit;
		public static double ConvertCoordinate(double coordinate, MeasureUnit fromUnit, MeasureUnit toUnit) {
			double valueInMeter = fromUnit.ToMeters(coordinate);
			return toUnit.FromMeters(valueInMeter);
		}
		public static CartesianPoint ConvertPoint(CoordPoint point, MeasureUnit fromUnit, MeasureUnit toUnit) {
			double x = ConvertCoordinate(point.GetX(), fromUnit, toUnit);
			double y = ConvertCoordinate(point.GetY(), fromUnit, toUnit);
			return (CartesianPoint)CartesianPointFactory.Instance.CreatePoint(x, y);
		}
		public static readonly DependencyProperty MeasureUnitProperty = DependencyPropertyManager.Register("MeasureUnit",
			typeof(MeasureUnit), typeof(CartesianMapCoordinateSystem), new PropertyMetadata(DefaultMeasureUnit, OnMeasureUnitChanged));
		static void OnMeasureUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CartesianMapCoordinateSystem coordSystem = d as CartesianMapCoordinateSystem;
			if(coordSystem != null)
				coordSystem.CoordSystemCore.MeasureUnit = coordSystem.MeasureUnit;
		}
		[Category(Categories.Behavior)]
		public MeasureUnit MeasureUnit {
			get { return (MeasureUnit)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		bool needUpdateBoundingBox;
		protected new MapCartesianCoordinateSystemCore CoordSystemCore { get { return (MapCartesianCoordinateSystemCore)base.CoordSystemCore; } }
		protected internal override bool NeedUpdateBoundingBox { get { return needUpdateBoundingBox; } }
		protected override MapCoordinateSystemCore CreateCoordSystemCore() {
			return new MapCartesianCoordinateSystemCore(CartesianPointFactory.Instance);
		}
		protected internal override void SetNeedUpdateBoundingBox(bool needUpdate) {
			this.needUpdateBoundingBox = needUpdate;
		}
		protected internal override void ResetBoundingBox() {
			CoordSystemCore.ResetBoundingBox();
		}
		protected internal override void UpdateBoundingBox(CoordBounds rect) {
			CoordSystemCore.UpdateBoundingBox(rect);
		}
		protected internal override void CorrectBoundingBox() {
			CoordSystemCore.CorrectBoundingBox();
		}
		protected override MapDependencyObject CreateObject() {
			return new CartesianMapCoordinateSystem();
		}
		public override CoordPoint CreatePoint(double x, double y) {
			CoordPoint pointInUserUnits = PointFactory.CreatePoint(x, y);
			CoordPoint pointInMeters = ConvertPoint(pointInUserUnits, MeasureUnit, MeasureUnit.Meter);
			return pointInMeters;
		}
		public override CoordPoint CreatePoint(string input) {
			CoordPoint pointInUserUnits = CartesianPoint.Parse(input);
			CoordPoint pointInMeters = ConvertPoint(pointInUserUnits, MeasureUnit, MeasureUnit.Meter);
			return pointInMeters;
		}
	}
}
