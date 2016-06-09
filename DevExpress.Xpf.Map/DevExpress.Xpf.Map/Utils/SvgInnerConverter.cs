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

using DevExpress.Data.Svg;
using DevExpress.Map;
using DevExpress.Map.Native;
using System.Windows;
namespace DevExpress.Xpf.Map.Native {
	public interface ISvgInnerConverter {
		SvgPoint CoordToSvgPoint(CoordPoint location);
		SvgPoint CoordToSvgPoint(CoordPoint location, Point startPoint);
		SvgPoint CoordToSvgPoint(CoordPoint location, double maxOrdinate);
		CoordPoint SvgToCoordPoint(SvgPoint point, Point startPoint);
		SvgSize CoordToSvgSize(CoordPoint location, SvgSize sourceSize, Point startPoint);
		SvgSize SvgToCoordSize(SvgPoint point, SvgSize sourceSize, Point startPoint);
	}
	public class SvgInnerCartesianConverter : ISvgInnerConverter {
		public double MaxOrdinate { get; set; }
		public SvgPoint CoordToSvgPoint(CoordPoint point) {
			return new SvgPoint(point.GetX(), point.GetY());
		}
		public SvgPoint CoordToSvgPoint(CoordPoint point, double maxOrdinate) {
			return new SvgPoint(point.GetX(), maxOrdinate - point.GetY());
		}
		public SvgPoint CoordToSvgPoint(CoordPoint point, Point startPoint) {
			return new SvgPoint(point.GetX() - startPoint.X, MaxOrdinate - point.GetY() - startPoint.Y);
		}
		public CoordPoint SvgToCoordPoint(SvgPoint point, Point startPoint) {
			return new CartesianPoint(point.X + startPoint.X, MaxOrdinate - point.Y + startPoint.Y);
		}
		public SvgSize CoordToSvgSize(CoordPoint location, SvgSize sourceSize, Point startPoint) {
			return sourceSize;
		}
		public SvgSize SvgToCoordSize(SvgPoint point, SvgSize sourceSize, Point startPoint) {
			return sourceSize;
		}
	}
	public class SvgInnerGeoConverter : ISvgInnerConverter {
		IMapView mapView;
		GeoMapCoordinateSystem coordinateSystem;
		public IMapView MapView { get { return mapView; } }
		public GeoMapCoordinateSystem CoordinateSystem { get { return coordinateSystem; } }
		public ProjectionBase Projection { get { return CoordinateSystem.Projection; } }
		public SvgInnerGeoConverter(IMapView mapView, GeoMapCoordinateSystem coordinateSystem) {
			this.mapView = mapView;
			this.coordinateSystem = coordinateSystem;
		}
		public SvgPoint CoordToSvgPoint(CoordPoint point) {
			return CoordToSvgPoint(point, new Point(0.0, 0.0));
		}
		public SvgPoint CoordToSvgPoint(CoordPoint point, double maxOrdinate) {
			return CoordToSvgPoint(point, new Point(0.0, 0.0));
		}
		public SvgPoint CoordToSvgPoint(CoordPoint point, Point startPoint) { 
			MapSizeCore mapSizeInPixels = MathUtils.CalcMapSizeInPixels(1.0, new MapSizeCore(mapView.InitialMapSize.Width, mapView.InitialMapSize.Height));
			Point screenPoint = CoordinateSystem.CoordToScreenPointIdentity(mapView.ViewportInPixels, point, new Size(mapSizeInPixels.Width, mapSizeInPixels.Height));
			return new SvgPoint(screenPoint.X - startPoint.X, screenPoint.Y - startPoint.Y);
		}
		public CoordPoint SvgToCoordPoint(SvgPoint point, Point startPoint) {
			Point screenPoint = new Point(point.X + startPoint.X, point.Y + startPoint.Y);
			MapSizeCore mapSizeInPixels = MathUtils.CalcMapSizeInPixels(1.0, new MapSizeCore(mapView.InitialMapSize.Width, mapView.InitialMapSize.Height));
			Rect viewport = CoordinateSystem.CalculateViewport(1.0, CoordinateSystem.Center, mapView.ViewportInPixels, new Size(mapSizeInPixels.Width, mapSizeInPixels.Height));
			MapUnit unit = CoordinateSystem.ScreenPointToMapUnit(screenPoint, viewport, mapView.ViewportInPixels);
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		public SvgSize CoordToSvgSize(CoordPoint location, SvgSize sourceSize, Point startPoint) {
			Size coordSize = Projection.KilometersToGeoSize((GeoPoint)location, new Size(sourceSize.Width, sourceSize.Height));
			CoordPoint oppositeLocation = location.Offset(coordSize.Width, coordSize.Height);
			SvgPoint oppositePoint = CoordToSvgPoint(oppositeLocation, startPoint);
			SvgPoint point = CoordToSvgPoint(location, startPoint);
			return new SvgSize(oppositePoint.X - point.X, point.Y - oppositePoint.Y);
		}
		public SvgSize SvgToCoordSize(SvgPoint point, SvgSize sourceSize, Point startPoint) {
			CoordPoint location = SvgToCoordPoint(point, startPoint);
			SvgPoint oppositePoint = new SvgPoint(point.X + sourceSize.Width, point.Y - sourceSize.Height);
			CoordPoint oppositeLocation = SvgToCoordPoint(oppositePoint, startPoint);
			Size coordSize = new Size(oppositeLocation.GetX() - location.GetX(), oppositeLocation.GetY() - location.GetY());
			Size size = Projection.GeoToKilometersSize((GeoPoint)location, coordSize);
			return new SvgSize(size.Width, size.Height);
		}
	}
}
