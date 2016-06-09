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

using DevExpress.Map;
using DevExpress.Map.Native;
using System;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public enum DistanceMeasureUnit : int {
		Kilometer = 0,
		Mile = 1
	}
	public abstract class ProjectionBase {
		public static ProjectionBase DefaultProjection { get { return new SphericalMercatorProjection(); } }
		public static bool operator ==(ProjectionBase x, ProjectionBase y) {
			if((object)x == null && (object)y == null)
				return true;
			if((object)x == null ^ (object)y == null)
				return false;
			ProjectionCoreWrapper proj1 = x as ProjectionCoreWrapper;
			ProjectionCoreWrapper proj2 = y as ProjectionCoreWrapper;
			if((object)proj1 != null && (object)proj2 != null)
				return proj1.ProjectionCore == proj2.ProjectionCore;
			return (object)proj1 == (object)proj2;
		}
		public static bool operator !=(ProjectionBase x, ProjectionBase y) {
			return !(x == y);
		}
		public const double LonToKilometersRatio = ProjectionBaseCore.LonToKilometersRatio;
		public const double LatToKilometersRatio = ProjectionBaseCore.LatToKilometersRatio;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ProjectionBaseOffsetX"),
#endif
		DefaultValue(ProjectionBaseCore.DefaultOffsetX)]
		public virtual double OffsetX { get; set; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ProjectionBaseOffsetY"),
#endif
		DefaultValue(ProjectionBaseCore.DefaultOffsetY)]
		public virtual double OffsetY { get; set; }
#if !SL
	[DevExpressXtraMapLocalizedDescription("ProjectionBaseScaleX")]
#endif
		public virtual double ScaleX { get; set; }
		bool ShouldSerializeScaleX() { return ScaleX != DefaultProjection.ScaleX; }
		void ResetScaleX() { ScaleX = DefaultProjection.ScaleX; }
#if !SL
	[DevExpressXtraMapLocalizedDescription("ProjectionBaseScaleY")]
#endif
		public virtual double ScaleY { get; set; }
		bool ShouldSerializeScaleY() { return ScaleY != DefaultProjection.ScaleY; }
		void ResetScaleY() { ScaleY = DefaultProjection.ScaleY; }
		protected ProjectionBase() { }
		protected internal virtual CoordBounds GetBoundingBox() { return CoordBounds.Empty; }
		public abstract GeoPoint MapUnitToGeoPoint(MapUnit mapPoint);
		public abstract MapUnit GeoPointToMapUnit(GeoPoint geoPoint);
		public abstract MapSize GeoToKilometersSize(GeoPoint anchorPoint, MapSize size);
		public abstract MapSize KilometersToGeoSize(GeoPoint anchorPoint, MapSize size);
		public override bool Equals(object obj) {
			ProjectionCoreWrapper proj1 = this as ProjectionCoreWrapper;
			ProjectionCoreWrapper proj2 = obj as ProjectionCoreWrapper;
			if(proj1 != null && proj2 != null)
				return proj1.ProjectionCore.Equals(proj2.ProjectionCore);
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			ProjectionCoreWrapper proj = this as ProjectionCoreWrapper;
			return proj != null ? proj.ProjectionCore.GetHashCode() : base.GetHashCode();
		}
	}
	public abstract class ProjectionCoreWrapper : ProjectionBase {
		ProjectionBaseCore projectionCore;
		internal ProjectionBaseCore ProjectionCore { get { return projectionCore; } }
		[ DefaultValue(ProjectionBaseCore.DefaultOffsetX)]
		public override double OffsetX { get { return projectionCore.OffsetX; } set { projectionCore.OffsetX = value; } }
		[ DefaultValue(ProjectionBaseCore.DefaultOffsetY)]
		public override double OffsetY { get { return projectionCore.OffsetY; } set { projectionCore.OffsetY = value; } }
		public override double ScaleX { get { return projectionCore.ScaleX; } set { projectionCore.ScaleX = value; } }
		bool ShouldSerializeScaleX() { return ScaleX != projectionCore.DefaultScaleX; }
		void ResetScaleX() { ScaleX = projectionCore.DefaultScaleX; }
		public override double ScaleY { get { return projectionCore.ScaleY; } set { projectionCore.ScaleY = value; } }
		bool ShouldSerializeScaleY() { return ScaleY != projectionCore.DefaultScaleY; }
		void ResetScaleY() { ScaleY = projectionCore.DefaultScaleY; }
		protected ProjectionCoreWrapper() {
			projectionCore = CreateProjectionCore();
		}
		protected abstract ProjectionBaseCore CreateProjectionCore();
		public override GeoPoint MapUnitToGeoPoint(MapUnit mapPoint) {
			return (GeoPoint)projectionCore.GetCoordPoint(mapPoint);
		}
		public override MapUnit GeoPointToMapUnit(GeoPoint geoPoint) {
			return new MapUnit(projectionCore.GetMapUnit(geoPoint));
		}
		public override MapSize GeoToKilometersSize(GeoPoint anchorPoint, MapSize size) {
			CoordVector offset = projectionCore.CoordToKilometers(anchorPoint, new CoordVector(size.Width, size.Height));
			return new MapSize(offset.X, offset.Y);
		}
		public override MapSize KilometersToGeoSize(GeoPoint anchorPoint, MapSize size) {
			CoordVector offset = projectionCore.KilometersToCoord(anchorPoint, new CoordVector(size.Width, size.Height));
			return new MapSize(offset.X, offset.Y);
		}
		protected internal override CoordBounds GetBoundingBox() {
			if (projectionCore != null)
				return new CoordBounds((GeoPoint)projectionCore.MinGeoPoint, (GeoPoint)projectionCore.MaxGeoPoint);
			return CoordBounds.Empty;	
		}
	}
	public class EqualAreaProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new EqualAreaProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(EqualAreaProjection)";
		}
	}
	public class EquirectangularProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new EquirectangularProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(EquirectangularProjection)";
		}
	}
	public class SphericalMercatorProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new SphericalMercatorProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(SphericalMercatorProjection)";
		}
	}
	public class EllipticalMercatorProjection : ProjectionCoreWrapper {
#if !SL
	[DevExpressXtraMapLocalizedDescription("EllipticalMercatorProjectionEllipsoid")]
#endif
		public Ellipsoid Ellipsoid {
			get {
				EllipsoidCore ellipsoidCore = ((EllipticalMercatorProjectionCore)ProjectionCore).Ellipsoid;
				return Ellipsoid.CreateByTwoAxes(ellipsoidCore.SemimajorAxis, ellipsoidCore.SemiminorAxis);
			}
			set { ((EllipticalMercatorProjectionCore)ProjectionCore).Ellipsoid = value.EllipsoidCore; }
		}
		void ResetEllipsoid() { Ellipsoid = Ellipsoid.WGS84; }
		bool ShouldSerializeEllipsoid() { return Ellipsoid != Ellipsoid.WGS84; }
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new EllipticalMercatorProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(EllipticalMercatorProjection)";
		}
	}
	public class MillerProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new MillerProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(MillerProjection)";
		}
	}
	public class EquidistantProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new EquidistantProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(EquidistantProjection)";
		}
	}
	public class LambertCylindricalEqualAreaProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new LambertCylindricalEqualAreaProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(LambertCylindricalEqualAreaProjection)";
		}
	}
	public class BraunStereographicProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new BraunStereographicProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(BraunStereographicProjection)";
		}
	}
	public class KavrayskiyProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new KavrayskiyProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(KavrayskiyProjection)";
		}
	}
	public class SinusoidalProjection : ProjectionCoreWrapper {
		protected override ProjectionBaseCore CreateProjectionCore() {
			return new SinusoidalProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "(SinusoidalProjection)";
		}
	}
}
