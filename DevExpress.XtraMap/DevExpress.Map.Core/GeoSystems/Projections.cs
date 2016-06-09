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
using DevExpress.Utils;
namespace DevExpress.Map.Native {
	public abstract class ProjectionBaseCore {
		public const double DefaultOffsetX = 0.5;
		public const double DefaultOffsetY = 0.5;
		public const double LonToKilometersRatio = 111.12;
		public const double LatToKilometersRatio = 111.12;
		public CoordObjectFactory CoordFactory { get; private set; }
		protected abstract double MinLatitudeInternal { get; }
		protected abstract double MaxLatitudeInternal { get; }
		protected abstract double MinLongitudeInternal { get; }
		protected abstract double MaxLongitudeInternal { get; }
		public CoordPoint MaxGeoPoint { get { return CoordFactory.CreatePoint(MaxLon, MaxLat); } }
		public CoordPoint MinGeoPoint { get { return CoordFactory.CreatePoint(MinLon, MinLat); } }
		public virtual double DefaultScaleX { get { return 0.5 / Math.PI; } }
		public virtual double DefaultScaleY { get { return -0.5 / Math.PI; } }
		public double MaxLat { get { return MaxLatitudeInternal; } }
		public double MaxLon { get { return MaxLongitudeInternal; } }
		public double MinLat { get { return MinLatitudeInternal; } }
		public double MinLon { get { return MinLongitudeInternal; } }
		public double OffsetX { get; set; }
		public double OffsetY { get; set; }
		public double ScaleX { get; set; }
		public double ScaleY { get; set; }
		protected ProjectionBaseCore(CoordObjectFactory coordFactory) {
			OffsetX = DefaultOffsetX;
			OffsetY = DefaultOffsetY;
			ScaleX = DefaultScaleX;
			ScaleY = DefaultScaleY;
			CoordFactory = coordFactory;
		}
		public static bool operator ==(ProjectionBaseCore x, ProjectionBaseCore y) {
			bool xIsNull = (object)x == null;
			bool yIsNull = (object)y == null;
			if(xIsNull && yIsNull) return true;
			if(xIsNull || yIsNull) return false;
			return object.Equals(x, y);
		}
		public static bool operator !=(ProjectionBaseCore x, ProjectionBaseCore y) {
			return !(x == y);
		}
		public abstract CoordPoint GetCoordPoint(IMapUnit mapUnit);
		public abstract IMapUnit GetMapUnit(CoordPoint point);		
		public virtual CoordVector KilometersToCoord(CoordPoint anchorPoint, CoordVector offset) {
			double x = offset.X / LonToKilometersRatio / Math.Cos(MathUtils.Degree2Radian(anchorPoint.YCoord));
			double y = offset.Y / LatToKilometersRatio;
			return new CoordVector(x, y);
		}
		public virtual CoordVector CoordToKilometers(CoordPoint anchorPoint, CoordVector offset) {
			double x = offset.X * LonToKilometersRatio * Math.Cos(MathUtils.Degree2Radian(anchorPoint.YCoord));
			double y = offset.Y * LatToKilometersRatio;
			return new CoordVector(x, y);
		}
		public CoordBounds GetBoundingBox() {
			return new CoordBounds(MinGeoPoint, MaxGeoPoint);
		}
		public override bool Equals(object obj) {
			if(obj == null || GetType() != obj.GetType())
				return false;
			ProjectionBaseCore parameter = (ProjectionBaseCore)obj;
			double epsilon = 0.00000000001;
			if(ComparingUtils.CompareDoubles(OffsetX, parameter.OffsetX, epsilon) == 0 &&
				ComparingUtils.CompareDoubles(OffsetY, parameter.OffsetY, epsilon) == 0 &&
				ComparingUtils.CompareDoubles(ScaleX, parameter.ScaleX, epsilon) == 0 &&
				ComparingUtils.CompareDoubles(ScaleY, parameter.ScaleY, epsilon) == 0)
				return true;
			else
				return false;
		}
		public override int GetHashCode() {
			return GetType().Name.GetHashCode() |
						  OffsetX.GetHashCode() |
						  OffsetY.GetHashCode() |
						  ScaleX.GetHashCode() |
						  ScaleY.GetHashCode();
		}
	}
	public class EqualAreaProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / Math.PI * 2.0; } }
		public EqualAreaProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double y = Math.Min(1, Math.Max(-1, ((mapUnit.Y - OffsetY) / ScaleY) / Math.PI));
			double lat = ProjectionUtils.Radian2Degree(Math.Asin(y));
			double lon = ProjectionUtils.Radian2Degree((mapUnit.X - OffsetX) / ScaleX);
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double x = lon * ScaleX + OffsetX;
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(point.YCoord, MinLatitude)));
			double y = Math.Sin(lat) * Math.PI * ScaleY + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class EquirectangularProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / 2.0; } }
		public EquirectangularProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double lat = ProjectionUtils.Radian2Degree(mapUnit.Y - OffsetY) / (2 * ScaleY);
			lat = Math.Min(MaxLatitude, Math.Max(MinLatitude, lat));
			double lon = ProjectionUtils.Radian2Degree(mapUnit.X - OffsetX) / ScaleX;
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double x = lon * ScaleX + OffsetX;
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double y = 2 * lat * ScaleY + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class SphericalMercatorProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -85.05112878;
		public const double MaxLatitude = 85.05112878;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public SphericalMercatorProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double lat = ProjectionUtils.Radian2Degree(Math.Atan(Math.Sinh((mapUnit.Y - OffsetY) / ScaleY)));
			double lon = ProjectionUtils.Radian2Degree(mapUnit.X - OffsetX) / ScaleX;
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double x = lon * ScaleX + OffsetX;
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double sinLat = Math.Sin(lat);
			double y = 0.5 * Math.Log((1.0 + sinLat) / (1.0 - sinLat)) * ScaleY + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class EllipticalMercatorProjectionCore : ProjectionBaseCore {
		const double PiHalf = Math.PI / 2.0;
		public const double MinLatitude = -85.08405905;
		public const double MaxLatitude = 85.08405905;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public EllipsoidCore Ellipsoid { get; set; }
		public EllipticalMercatorProjectionCore(CoordObjectFactory geoPointFactory) 
			: base(geoPointFactory) {
			Ellipsoid = EllipsoidCore.WGS84;
		}
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double ts = Math.Exp(-(mapUnit.Y - OffsetY) / ScaleY);
			double phi = PiHalf - 2 * Math.Atan(ts);
			double dphi = 1.0;
			for (int i = 0; (i < 15) && (Math.Abs(dphi) > 0.000000001); i++) {
				double con = Ellipsoid.Eccentricity * Math.Sin(phi);
				dphi = PiHalf - 2 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), Ellipsoid.Eccentricity/2.0)) - phi;
				phi += dphi;
			}
			double lat = ProjectionUtils.Radian2Degree(phi);
			double lon = ProjectionUtils.Radian2Degree((mapUnit.X - OffsetX) / ScaleX);
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double x = lon * ScaleX + OffsetX;
			double lat = ProjectionUtils.Degree2Radian(Math.Min(89.0, Math.Max(point.YCoord, -89.0)));
			double con = Ellipsoid.Eccentricity * Math.Sin(lat);
			con = Math.Pow(((1.0 - con) / (1.0 + con)), Ellipsoid.Eccentricity / 2.0);
			double ts = Math.Tan(0.5 * ((Math.PI * 0.5) + lat)) * con;
			double y = Math.Min(1.0, Math.Max(0.0, Math.Log(ts) * ScaleY + OffsetY));
			return new MapUnitCore(x, y);
		}
	}
	public class MillerProjectionCore : ProjectionBaseCore {
		const double ScaleFactor = 2.30341254337639 / Math.PI;
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY * 0.8; } }
		public MillerProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double y = ((mapUnit.Y - OffsetY) / ScaleY) * ScaleFactor;
			double lat = 1.25 * Math.Atan(Math.Sinh(0.8 * y));
			lat = Math.Min(MaxLatitude, Math.Max(MinLatitude, ProjectionUtils.Radian2Degree(lat)));
			double lon = ProjectionUtils.Radian2Degree((mapUnit.X - OffsetX) / ScaleX);
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double x = lon * ScaleX + OffsetX;
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(point.YCoord, MinLatitude)));
			double y = 1.25 * Math.Log(Math.Tan((Math.PI / 4.0) + (0.4 * lat)));
			y = Math.Min(1.0, Math.Max(0.0, (y / ScaleFactor) * ScaleY + OffsetY));
			return new MapUnitCore(x, y);
		}
	}
	public class EquidistantProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		public const double MeridianOffset = 0.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / Math.Sqrt(2); } }
		public EquidistantProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double x = (mapUnit.X - OffsetX) / ScaleX;
			double y = (mapUnit.Y - OffsetY) / ScaleY / 2.0;
			double lon = ProjectionUtils.Radian2Degree(x) * (1 / Math.Cos(MeridianOffset));
			double lat = ProjectionUtils.Radian2Degree(y);
			lat = Math.Min(MaxLatitude, Math.Max(MinLatitude, lat));
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double x = lon * Math.Cos(MeridianOffset) * ScaleX + OffsetX;
			double y = lat * ScaleY * 2.0 + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class LambertCylindricalEqualAreaProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		public const double MeridianOffset = 0.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / Math.PI; } }
		public LambertCylindricalEqualAreaProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double x = (mapUnit.X - OffsetX) / ScaleX;
			double y = Math.Min(1, Math.Max((mapUnit.Y - OffsetY) / ScaleY / Math.PI, -1));
			double lon = ProjectionUtils.Radian2Degree(x);
			double lat = ProjectionUtils.Radian2Degree(Math.Asin(y));
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double x = lon * ScaleX + OffsetX;
			double y = Math.Sin(lat) * ScaleY * Math.PI + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class BraunStereographicProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		public const double MeridianOffset = 0.0;
		public const double Radius = 1.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / Math.PI * 2.0; } }
		public BraunStereographicProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double x = (mapUnit.X - OffsetX) / ScaleX;
			double y = Math.Atan((mapUnit.Y - OffsetY) / ScaleY / Radius / Math.PI) * 2;
			double lon = ProjectionUtils.Radian2Degree(x / Radius);
			double lat = Math.Min(MaxLatitude, Math.Max(MinLatitude, ProjectionUtils.Radian2Degree(y)));
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double x = Radius * lon * ScaleX + OffsetX;
			double y = Radius * Math.Tan(lat / 2) * ScaleY * Math.PI + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class KavrayskiyProjectionCore : ProjectionBaseCore {
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		public const double MeridianOffset = 0.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / 2.0; } }
		public KavrayskiyProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double x = (mapUnit.X - OffsetX) / ScaleX / Math.Sqrt(3.0) * Math.PI;
			double y = (mapUnit.Y - OffsetY) / ScaleY / 2.0;
			double denom = Math.Sqrt(Math.PI * Math.PI / 3.0 - y * y);
			double lon = !Double.IsNaN(denom) ? (x / denom) : Math.PI * Math.Sign(x);
			double lat = Math.Min(MaxLatitude, Math.Max(MinLatitude, ProjectionUtils.Radian2Degree(y)));
			lon = Math.Min(MaxLongitude, Math.Max(MinLongitude, ProjectionUtils.Radian2Degree(lon)));
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double x = lon * Math.Sqrt((Math.PI * Math.PI) / 3.0 - lat * lat) * ScaleX * Math.Sqrt(3.0) / Math.PI + OffsetX;
			double y = lat * ScaleY * 2.0 + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
	public class SinusoidalProjectionCore : ProjectionBaseCore {
		const double epsilon = 1e-10;
		public const double MinLatitude = -90.0;
		public const double MaxLatitude = 90.0;
		public const double MinLongitude = -180.0;
		public const double MaxLongitude = 180.0;
		public const double MeridianOffset = 0.0;
		public const double k = 1.0;
		protected override double MinLatitudeInternal { get { return MinLatitude; } }
		protected override double MaxLatitudeInternal { get { return MaxLatitude; } }
		protected override double MinLongitudeInternal { get { return MinLongitude; } }
		protected override double MaxLongitudeInternal { get { return MaxLongitude; } }
		public override double DefaultScaleY { get { return base.DefaultScaleY / 2.0; } }
		public SinusoidalProjectionCore(CoordObjectFactory geoPointFactory) : base(geoPointFactory) { }
		public override CoordPoint GetCoordPoint(IMapUnit mapUnit) {
			double normY = Math.Max(0.25, Math.Min(0.75, mapUnit.Y));
			double x = (mapUnit.X - OffsetX) / ScaleX;
			double y = (normY - OffsetY) / ScaleY / 2.0;
			double lat = Math.Min(MaxLatitude, Math.Max(MinLatitude, ProjectionUtils.Radian2Degree(y / k)));
			double cosy = Math.Cos(y);
			double delta = Math.Abs(x - cosy);
			double lon;
			if(delta > epsilon)
				lon = Math.Min(MaxLongitude, Math.Max(MinLongitude, ProjectionUtils.Radian2Degree(x / cosy / k)));
			else
				lon = y > 0 ? MaxLongitude : MinLongitude;
			return CoordFactory.CreatePoint(lon, lat);
		}
		public override IMapUnit GetMapUnit(CoordPoint point) {
			double lon = ProjectionUtils.Degree2Radian(point.XCoord);
			double lat = ProjectionUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, point.YCoord)));
			double x = lon * Math.Cos(lat);
			double y = lat;
			x = x * ScaleX + OffsetX;
			y = y * ScaleY * 2.0 + OffsetY;
			return new MapUnitCore(x, y);
		}
	}
}
