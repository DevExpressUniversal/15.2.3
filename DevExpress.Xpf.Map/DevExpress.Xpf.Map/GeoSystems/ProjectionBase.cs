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
using System.Windows;
using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.Xpf.Map.Native;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	public enum DistanceMeasureUnit {
		Kilometer,
		Mile
	}
	public abstract class ProjectionBase {
		public static ProjectionBase DefaultProjection { get { return new SphericalMercatorProjection(); } }
		public static bool operator ==(ProjectionBase x, ProjectionBase y) {
			if((object)x == null && (object)y == null)
				return true;
			if((object)x == null ^ (object)y == null)
				return false;
			PredefinedProjectionBase proj1 = x as PredefinedProjectionBase;
			PredefinedProjectionBase proj2 = y as PredefinedProjectionBase;
			if((object)proj1 != null && (object)proj2 != null)
				return proj1.ProjectionCore == proj2.ProjectionCore;
			return (object)proj1 == (object)proj2;
		}
		public static bool operator !=(ProjectionBase x, ProjectionBase y) {
			return !(x == y);
		}
		public const double LonToKilometersRatio = ProjectionBaseCore.LonToKilometersRatio;
		public const double LatToKilometersRatio = ProjectionBaseCore.LatToKilometersRatio;
		[Category(Categories.Behavior)]
		public virtual double OffsetX { get; set; }
		[Category(Categories.Behavior)]
		public virtual double OffsetY { get; set; }
		[Category(Categories.Behavior)]
		public virtual double ScaleX  { get; set; }
		[Category(Categories.Behavior)]
		public virtual double ScaleY  { get; set; }
		protected internal virtual CoordBounds GetBoundingBox() { return CoordBounds.Empty; }
		public abstract GeoPoint MapUnitToGeoPoint(MapUnit mapPoint);
		public abstract MapUnit GeoPointToMapUnit(GeoPoint geoPoint);
		public abstract Size GeoToKilometersSize(GeoPoint anchorPoint, Size size);
		public abstract Size KilometersToGeoSize(GeoPoint anchorPoint, Size size);
		public override bool Equals(object obj) {
			PredefinedProjectionBase proj1 = this as PredefinedProjectionBase;
			PredefinedProjectionBase proj2 = obj as PredefinedProjectionBase;
			if(proj1 != null && proj2 != null)
				return proj1.ProjectionCore.Equals(proj2.ProjectionCore);
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			PredefinedProjectionBase proj = this as PredefinedProjectionBase;
			return proj != null ? proj.ProjectionCore.GetHashCode() : base.GetHashCode();
		}
	}
	public abstract class PredefinedProjectionBase : ProjectionBase {
		ProjectionBaseCore projectionCore;
		internal GeoPoint MaxGeoPoint { get { return projectionCore != null ? (GeoPoint)projectionCore.MaxGeoPoint : new GeoPoint(); } }
		internal GeoPoint MinGeoPoint { get { return projectionCore != null ? (GeoPoint)projectionCore.MinGeoPoint : new GeoPoint(); } }
		protected internal ProjectionBaseCore ProjectionCore { get { return projectionCore; } }
		[Category(Categories.Behavior)]
		public override double OffsetX { get { return projectionCore.OffsetX; } set { projectionCore.OffsetX = value; } }
		[Category(Categories.Behavior)]
		public override double OffsetY { get { return projectionCore.OffsetY; } set { projectionCore.OffsetY = value; } }
		[Category(Categories.Behavior)]
		public override double ScaleX { get { return projectionCore.ScaleX; } set { projectionCore.ScaleX = value; } }
		[Category(Categories.Behavior)]
		public override double ScaleY { get { return projectionCore.ScaleY; } set { projectionCore.ScaleY = value; } }
		protected PredefinedProjectionBase() {
			this.projectionCore = GetProjectionCore();
		}
		protected abstract ProjectionBaseCore GetProjectionCore();
		protected internal override CoordBounds GetBoundingBox() {
			return new CoordBounds(MinGeoPoint, MaxGeoPoint);
		}
		public override GeoPoint MapUnitToGeoPoint(MapUnit mapPoint) {
			return (GeoPoint)projectionCore.GetCoordPoint(mapPoint);
		}
		public override MapUnit GeoPointToMapUnit(GeoPoint geoPoint) {
			return new MapUnit(projectionCore.GetMapUnit(geoPoint));
		}
		public override Size GeoToKilometersSize(GeoPoint anchorPoint, Size size) {
			CoordVector offset = projectionCore.CoordToKilometers(anchorPoint, new CoordVector(size.Width, size.Height));
			return new Size(offset.X, offset.Y);
		}
		public override Size KilometersToGeoSize(GeoPoint anchorPoint, Size size) {
			CoordVector offset = projectionCore.KilometersToCoord(anchorPoint, new CoordVector(size.Width, size.Height));
			return new Size(offset.X, offset.Y);
		}
	}
	public class EqualAreaProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new EqualAreaProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Equal Area projection";
		}
	}
	public class EquirectangularProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new EquirectangularProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Equrectangular projection";
		}
	}
	public class SphericalMercatorProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new SphericalMercatorProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Spherical Mercator projection";
		}
	}
	public class EllipticalMercatorProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new EllipticalMercatorProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Elliptical Mercator projection";
		}
	}
	public class MillerProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new MillerProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Miller projection";
		}
	}
	public class EquidistantProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new EquidistantProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Equidistant projection";
		}
	}
	public class LambertCylindricalEqualAreaProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new LambertCylindricalEqualAreaProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Lambert Cylindrical Equal Area projection";
		}
	}
	public class BraunStereographicProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new BraunStereographicProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Braun Stereographic projection";
		}
	}
	public class KavrayskiyProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new KavrayskiyProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Kavrayskiy projection";
		}
	}
	public class SinusoidalProjection : PredefinedProjectionBase {
		protected override ProjectionBaseCore GetProjectionCore() {
			return new SinusoidalProjectionCore(GeoPointFactory.Instance);
		}
		public override string ToString() {
			return "Sinusoidal projection";
		}
	}
}
