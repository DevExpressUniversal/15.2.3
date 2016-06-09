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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Native;
using System;
using DevExpress.Map;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap.Native {
	public abstract class MapUnitConverter {
		readonly MapCoordinateSystem coordinateSystem;
		protected internal virtual MapCoordinateSystem CoordinateSystem { get { return coordinateSystem; } }
		public CoordObjectFactory PointFactory { get { return CoordinateSystem.PointFactory; } }
		public abstract MapUnit CoordPointToMapUnit(CoordPoint point, bool shouldNormalize);
		public abstract MapPoint CoordPointToScreenPoint(CoordPoint point);
		public abstract MapPoint CoordPointToScreenPoint(CoordPoint point, bool useSprings);
		public abstract MapPoint CoordPointToScreenPointIdentity(CoordPoint point);
		public abstract MapSize MeasureUnitToCoordSize(CoordPoint anchorPoint, MapSize size);
		public abstract MapSize CoordSizeToMeasureUnit(CoordPoint anchorPoint, MapSize size);
		public abstract CoordPoint MapUnitToCoordPoint(MapUnit unit);
		public abstract MapPoint MapUnitToScreenPoint(MapUnit mapUnit, bool useSpringsAnimation);
		public abstract MapPoint MapUnitToScreenPoint(MapUnit mapUnit, double zoomLevel, CoordPoint centerPoint, Size viewportInPixels);
		public abstract CoordPoint ScreenPointToCoordPoint(MapPoint point);
		public abstract CoordPoint ScreenPointToCoordPoint(MapPoint point, bool useSpringsAnimation);
		public abstract CoordPoint ScreenPointToCoordPointIdentity(MapPoint point);
		public abstract MapUnit ScreenPointToMapUnit(MapPoint point);
		public abstract MapUnit ScreenPointToMapUnit(MapPoint point, bool useSpringsAnimation);
		protected MapUnitConverter(MapCoordinateSystem coordinateSystem) {
			this.coordinateSystem = coordinateSystem;
		}
		public abstract MapRect CalculateViewport(double zoomLevel, CoordPoint centerPoint, Size viewportInPixels, Size initialMapSize);
	}
	#region EmptyUnitConverter
	public class EmptyUnitConverter : MapUnitConverter {
		static readonly EmptyUnitConverter instance = new EmptyUnitConverter();
		public static EmptyUnitConverter Instance { get { return instance; } }
		EmptyUnitConverter() : base(InnerMap.DefaultCoordinateSystem) {
		}
		public override MapUnit CoordPointToMapUnit(CoordPoint point, bool shouldNormalize) {
			return new MapUnit();
		}
		public override CoordPoint MapUnitToCoordPoint(MapUnit unit) {
			return null;
		}
		public override MapPoint CoordPointToScreenPoint(CoordPoint point) {
			return MapPoint.Empty;
		}
		public override MapPoint CoordPointToScreenPoint(CoordPoint point, bool useSprings) {
			return MapPoint.Empty;
		}
		public override MapPoint CoordPointToScreenPointIdentity(CoordPoint point) {
			return MapPoint.Empty;
		}
		public override MapSize MeasureUnitToCoordSize(CoordPoint anchorPoint, MapSize size) {
			return MapSize.Empty;
		}
		public override MapSize CoordSizeToMeasureUnit(CoordPoint anchorPoint, MapSize size) {
			return MapSize.Empty;
		}
		public override CoordPoint ScreenPointToCoordPoint(MapPoint point) {
			return null;
		}
		public override CoordPoint ScreenPointToCoordPoint(MapPoint point, bool useSpringsAnimation) {
			return null;
		}
		public override CoordPoint ScreenPointToCoordPointIdentity(MapPoint point) {
			return null;
		}
		public override MapPoint MapUnitToScreenPoint(MapUnit mapUnit, bool useSpringsAnimation) {
			return MapPoint.Empty;
		}
		public override MapPoint MapUnitToScreenPoint(MapUnit mapUnit, double zoomLevel, CoordPoint centerPoint, Size viewportInPixels) {
			return MapPoint.Empty;
		}
		public override MapUnit ScreenPointToMapUnit(MapPoint point) {
			return new MapUnit();
		}
		public override MapUnit ScreenPointToMapUnit(MapPoint point, bool useSpringsAnimation) {
			return new MapUnit();
		}
		public override MapRect CalculateViewport(double zoomLevel, CoordPoint centerPoint, Size viewportInPixels, Size initialMapSize) {
			return MapRect.Empty;
		}
	}
	#endregion
	public class MapViewUnitConverter : MapUnitConverter {
		IMapView view;
		protected MapViewportInternal CurrentViewport { get { return view.Viewport; } }
		protected double ActualZoomLevel { get { return view.ZoomLevel; } }
		protected CoordPoint ActualCenterPoint { get { return view.CenterPoint; } }
		protected Size InitialMapSize { get { return view.InitialMapSize; } }
		#region viewport methods
		MapRect GetViewportBounds(bool useSprings) {
			return useSprings ? CurrentViewport.AnimatedViewportRect : CalculateViewport(ActualZoomLevel, ActualCenterPoint, CurrentViewport.ViewportInPixels, InitialMapSize);
		}
		public override MapRect CalculateViewport(double zoomLevel, CoordPoint centerPoint, Size viewportInPixels, Size initialMapSize) {
			return CoordinateSystem.CalculateViewport(zoomLevel, centerPoint, viewportInPixels, initialMapSize);
		}
		#endregion
		#region Units converter methods
		public override MapUnit CoordPointToMapUnit(CoordPoint point, bool shouldNormalize) {
			return CoordinateSystem.CoordPointToMapUnit(point, shouldNormalize);
		}
		public override MapPoint CoordPointToScreenPoint(CoordPoint point) {
			return CoordPointToScreenPoint(point, false);
		}
		public override MapPoint CoordPointToScreenPoint(CoordPoint point, bool useSprings) {
			bool shouldNormalize = !useSprings;
			MapUnit unit = CoordPointToMapUnit(point, shouldNormalize);
			return MapUnitToScreenPoint(unit, useSprings);
		}
		public override MapPoint CoordPointToScreenPointIdentity(CoordPoint point) {
			MapUnit unit = CoordinateSystem.CoordPointToMapUnit(point, true);
			MapRect viewport = CoordinateSystem.CalculateViewport(1.0, PointFactory.CreatePoint(0, 0), CurrentViewport.ViewportInPixels, InitialMapSize);
			return CoordinateSystem.MapUnitToScreenPoint(unit, viewport, CurrentViewport.ViewportInPixels);
		}
		public override MapSize MeasureUnitToCoordSize(CoordPoint anchorPoint, MapSize size) {
			return CoordinateSystem.KilometersToGeoSize(anchorPoint, size);
		}
		public override MapSize CoordSizeToMeasureUnit(CoordPoint anchorPoint, MapSize size) {
			return CoordinateSystem.GeoToKilometersSize(anchorPoint, size);
		}
		public override CoordPoint MapUnitToCoordPoint(MapUnit unit) {
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		public override MapPoint MapUnitToScreenPoint(MapUnit mapUnit, bool useSpringsAnimation) {
			MapRect viewportRect = GetViewportBounds(useSpringsAnimation);
			return CoordinateSystem.MapUnitToScreenPoint(mapUnit, viewportRect, CurrentViewport.ViewportInPixels);
		}
		public override MapPoint MapUnitToScreenPoint(MapUnit mapUnit, double zoomLevel, CoordPoint centerPoint, Size viewportInPixels) {
			MapRect viewport = CoordinateSystem.CalculateViewport(zoomLevel, centerPoint, viewportInPixels, view.InitialMapSize);
			MapPoint point = CoordinateSystem.MapUnitToScreenPoint(mapUnit, viewport, viewportInPixels);
			point.X += viewport.Left * viewportInPixels.Width;
			point.Y += viewport.Top * viewportInPixels.Height;
			return point;
		}
		public override CoordPoint ScreenPointToCoordPoint(MapPoint point) {
			return ScreenPointToCoordPoint(point, true);
		}
		public override CoordPoint ScreenPointToCoordPoint(MapPoint point, bool useSpringsAnimation) {
			MapRect viewportRect = GetViewportBounds(useSpringsAnimation);
			MapUnit unit = CoordinateSystem.ScreenPointToMapUnit(point, viewportRect, CurrentViewport.ViewportInPixels);
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		public override CoordPoint ScreenPointToCoordPointIdentity(MapPoint point) {
			MapRect viewport = CoordinateSystem.CalculateViewport(1.0, PointFactory.CreatePoint(0, 0), CurrentViewport.ViewportInPixels, InitialMapSize);
			MapUnit units = CoordinateSystem.ScreenPointToMapUnit(point, viewport, CurrentViewport.ViewportInPixels);
			return CoordinateSystem.MapUnitToCoordPoint(units);
		}
		public override MapUnit ScreenPointToMapUnit(MapPoint point) {
			return ScreenPointToMapUnit(point, true);
		}
		public override MapUnit ScreenPointToMapUnit(MapPoint point, bool useSpringsAnimation) {
			MapRect viewPort = GetViewportBounds(useSpringsAnimation);
			return CoordinateSystem.ScreenPointToMapUnit(point, viewPort, CurrentViewport.ViewportInPixels);
		}
		#endregion
		public MapViewUnitConverter(IMapView map, MapCoordinateSystem coordinateSystem) : base(coordinateSystem) {
			this.view = map;
		}
	}
}
