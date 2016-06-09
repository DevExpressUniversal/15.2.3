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
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public enum CoordPointType {
		Unknown,
		Geo,
		Cartesian
	}
	public abstract class MapCoordinateSystemCore {
		readonly CoordObjectFactory pointFactory;
		public CoordObjectFactory PointFactory { get { return pointFactory; } }
		public abstract CoordPointType PointType { get; }
		public abstract CoordBounds BoundingBox { get; }
		public abstract CoordPoint Center { get; }
		protected MapCoordinateSystemCore(CoordObjectFactory pointFactory) {
			this.pointFactory = pointFactory;
		}
		public bool IsEqual(MapCoordinateSystemCore coordSystem) {
			if(object.ReferenceEquals(coordSystem, this))
				return true;
			if(this.PointType != coordSystem.PointType)
				return false;
			return EqualsCore(coordSystem);
		}
		protected abstract bool EqualsCore(MapCoordinateSystemCore coordSystem);
		public IMapUnit CoordPointToMapUnit(CoordPoint point, bool shouldNormalize) {
			return CoordPointToMapUnit(shouldNormalize ? point.CreateNormalized() : point);
		}
		public abstract IMapUnit CoordPointToMapUnit(CoordPoint coordPoint);
		public abstract CoordPoint MapUnitToCoordPoint(IMapUnit mapUnit);
		public abstract double CalculateKilometersScale(CoordPoint anchorPoint, double width);
		public double CalculateKilometersScale(CoordPoint centerPoint, int maxScaleLineWidth, MapRectCore viewport, MapSizeCore viewportInPixels) {
			if(BoundingBox.IsEmpty)
				return 0.0;
			MapPointCore screenCenterPoint = CoordPointToScreenPoint(UnmeasurePoint(centerPoint), viewport, viewportInPixels);
			double hemiplaneSign = UnmeasurePoint(centerPoint).GetX() >= Center.GetX() ? 1.0 : -1.0;
			MapPointCore screenScalePoint = new MapPointCore(screenCenterPoint.X - hemiplaneSign * maxScaleLineWidth, screenCenterPoint.Y);
			CoordPoint coordScalePoint = MeasurePoint(ScreenPointToCoordPoint(screenScalePoint, viewport, viewportInPixels));
			return CalculateKilometersScale(centerPoint, Math.Abs(coordScalePoint.GetX() - centerPoint.GetX()));
		}
		public abstract MapSizeCore GeoToKilometersSize(CoordPoint anchorPoint, MapSizeCore size);
		public abstract MapSizeCore KilometersToGeoSize(CoordPoint anchorPoint, MapSizeCore size);
		public abstract CoordPoint MeasurePoint(CoordPoint point);
		public abstract CoordPoint UnmeasurePoint(CoordPoint point);
		public CoordPoint CreateNormalizedPoint(CoordPoint centerPoint) {
			if(BoundingBox.IsEmpty)
				return PointFactory.CreatePoint(centerPoint.GetX(), centerPoint.GetY());
			double x = Math.Max(Math.Min(centerPoint.GetX(), BoundingBox.X2), BoundingBox.X1);
			double y = Math.Max(Math.Min(centerPoint.GetY(), BoundingBox.Y1), BoundingBox.Y2);
			return PointFactory.CreatePoint(x, y);
		}
		public IMapUnit ScreenPointToMapUnit(MapPointCore point, MapRectCore viewport, MapSizeCore viewportInPixels) {
			double pixelFactorX = viewportInPixels.Width > 0 ? viewport.Width / viewportInPixels.Width : 0;
			double pixelFactorY = viewportInPixels.Height > 0 ? viewport.Height / viewportInPixels.Height : 0;
			return new MapUnitCore(point.X * pixelFactorX + viewport.Left, point.Y * pixelFactorY + viewport.Top);
		}
		public MapPointCore MapUnitToScreenPoint(IMapUnit mapUnit, MapRectCore viewport, MapSizeCore viewportInPixels) {
			double unitFactorX = viewport.Width > 0 ? viewportInPixels.Width / viewport.Width : 0;
			double unitFactorY = viewport.Height > 0 ? viewportInPixels.Height / viewport.Height : 0;
			return new MapPointCore((mapUnit.X - viewport.Left) * unitFactorX, (mapUnit.Y - viewport.Top) * unitFactorY);
		}
		public MapPointCore CoordPointToScreenPoint(CoordPoint point, MapRectCore viewport, MapSizeCore viewportInPixels) {
			IMapUnit unit = CoordPointToMapUnit(point, true);
			return MapUnitToScreenPoint(unit, viewport, viewportInPixels);
		}
		public CoordPoint ScreenPointToCoordPoint(MapPointCore point, MapRectCore viewport, MapSizeCore viewportInPixels) {
			IMapUnit unit = ScreenPointToMapUnit(point, viewport, viewportInPixels);
			return MapUnitToCoordPoint(unit);
		}
		public MapRectCore CalculateViewport(double zoomLevel, CoordPoint centerPoint, MapSizeCore viewportInPixels, MapSizeCore initialMapSize) {
			MapSizeCore mapSizeInPixels = MathUtils.CalcMapSizeInPixels(zoomLevel, initialMapSize);
			double width = viewportInPixels.Width / mapSizeInPixels.Width;
			double height = viewportInPixels.Height / mapSizeInPixels.Height;
			IMapUnit center = CoordPointToMapUnit(centerPoint, true);
			return new MapRectCore(center.X - width * 0.5, center.Y - height * 0.5, width, height);
		}
	}
	public class MapGeoCoordinateSystemCore : MapCoordinateSystemCore {
		ProjectionBaseCore projection;
		public override CoordPointType PointType { get { return CoordPointType.Geo; } }
		public override CoordBounds BoundingBox { get { return Projection.GetBoundingBox(); } }
		public override CoordPoint Center { get { return PointFactory.CreatePoint(0, 0); } }
		public ProjectionBaseCore Projection {
			get { return projection; }
			set { projection = value; }
		}
		public MapGeoCoordinateSystemCore(CoordObjectFactory pointFactory)
			: base(pointFactory) {
				this.projection = new SphericalMercatorProjectionCore(PointFactory);
		}
		protected override bool EqualsCore(MapCoordinateSystemCore coordSystem) {
			MapGeoCoordinateSystemCore geoSystem = (MapGeoCoordinateSystemCore)coordSystem;
			return Projection.Equals(geoSystem.Projection);
		}
		public override IMapUnit CoordPointToMapUnit(CoordPoint coordPoint) {
			return Projection.GetMapUnit(coordPoint);
		}
		public override CoordPoint MapUnitToCoordPoint(IMapUnit mapUnit) {
			return Projection.GetCoordPoint(mapUnit);
		}
		public override double CalculateKilometersScale(CoordPoint anchorPoint, double width) {
			MapSizeCore size = GeoToKilometersSize(anchorPoint, new MapSizeCore(width, 0));
			return size.Width;
		}
		public override MapSizeCore GeoToKilometersSize(CoordPoint anchorPoint, MapSizeCore size) {
			CoordVector vector = Projection.CoordToKilometers(anchorPoint, new CoordVector(size.Width, size.Height));
			return new MapSizeCore(vector.X, vector.Y);
		}
		public override MapSizeCore KilometersToGeoSize(CoordPoint anchorPoint, MapSizeCore size) {
			CoordVector geoSize = Projection.KilometersToCoord(anchorPoint, new CoordVector(size.Width, size.Height));
			return new MapSizeCore(geoSize.X, geoSize.Y);
		}
		public override CoordPoint MeasurePoint(CoordPoint point) {
			return point;
		}
		public override CoordPoint UnmeasurePoint(CoordPoint point) {
			return point;
		}
	}
	public class MapCartesianCoordinateSystemCore : MapCoordinateSystemCore {
		static readonly MeasureUnitCore defaultMeasureUnit = MeasureUnitCore.Meter;
		public static MeasureUnitCore DefaultMeasureUnit { get { return defaultMeasureUnit; } }
		CoordBounds bounds = CoordBounds.Empty;
		MeasureUnitCore measureUnit = defaultMeasureUnit;
		public override CoordPointType PointType { get { return CoordPointType.Cartesian; } }
		public override CoordBounds BoundingBox { get { return bounds; } }
		public override CoordPoint Center {
			get {
				double x = (BoundingBox.X1 + BoundingBox.X2) / 2.0;
				double y = (BoundingBox.Y1 + BoundingBox.Y2) / 2.0;
				if(double.IsNaN(x) || double.IsNaN(y))
					return PointFactory.CreatePoint(0, 0);
				else
					return PointFactory.CreatePoint(x, y);
			} 
		}
		public MeasureUnitCore MeasureUnit {
			get { return measureUnit; }
			set {
				if(value == null)
					value = defaultMeasureUnit;
				measureUnit = value; 
			}
		}
		public MapCartesianCoordinateSystemCore(CoordObjectFactory pointFactory)
			: base(pointFactory) {
		}
		protected override bool EqualsCore(MapCoordinateSystemCore coordSystem) {
			MapCartesianCoordinateSystemCore cartesianSystem = (MapCartesianCoordinateSystemCore)coordSystem;
			return MeasureUnit.Equals(cartesianSystem.MeasureUnit);
		}
		public override IMapUnit CoordPointToMapUnit(CoordPoint coordPoint) {
			double x = (coordPoint.GetX() - BoundingBox.X1) / (BoundingBox.X2 - BoundingBox.X1);
			if(double.IsNaN(x) || double.IsInfinity(x))
				x = 0.5;
			double y = 1 - (coordPoint.GetY() - BoundingBox.Y2) / (BoundingBox.Y1 - BoundingBox.Y2);
			if(double.IsNaN(y) || double.IsInfinity(y))
				y = 0.5;
			return new MapUnitCore(x, y);
		}
		public override CoordPoint MapUnitToCoordPoint(IMapUnit mapUnit) {
			if(BoundingBox.IsEmpty)
				return PointFactory.CreatePoint(0, 0);
			double x = mapUnit.X * (BoundingBox.X2 - BoundingBox.X1) + BoundingBox.X1;
			double y = (1 - mapUnit.Y) * (BoundingBox.Y1 - BoundingBox.Y2) + BoundingBox.Y2;
			return PointFactory.CreatePoint(x, y);
		}
		public override double CalculateKilometersScale(CoordPoint anchorPoint, double size) {
			return MeasureUnit.ToMeters(size) / 1000.0;
		}
		public override MapSizeCore GeoToKilometersSize(CoordPoint anchorPoint, MapSizeCore size) {
			double x = size.Width;
			double y = size.Height;
			return new MapSizeCore(x, y);
		}
		public override MapSizeCore KilometersToGeoSize(CoordPoint anchorPoint, MapSizeCore size) {
			double x = size.Width;
			double y = size.Height;
			return new MapSizeCore(x, y);
		}
		public override CoordPoint MeasurePoint(CoordPoint point) {
			double measuredX = MeasureUnit.FromMeters(point.GetX());
			double measuredY = MeasureUnit.FromMeters(point.GetY());
			return PointFactory.CreatePoint(measuredX, measuredY);
		}
		public override CoordPoint UnmeasurePoint(CoordPoint point) {
			double measuredX = MeasureUnit.ToMeters(point.GetX());
			double measuredY = MeasureUnit.ToMeters(point.GetY());
			return PointFactory.CreatePoint(measuredX, measuredY);
		}
		public bool UpdateBoundingBox(CoordBounds rect) {
			CoordBounds oldBounds = BoundingBox;
			this.bounds = CoordBounds.Union(bounds, rect);
			return oldBounds != bounds;
		}
		public void ResetBoundingBox() {
			this.bounds = CoordBounds.Empty;
		}
		public void CorrectBoundingBox() {
			if(this.bounds.IsEmpty)
				return;
			this.bounds.Correct();
		}
	}
}
