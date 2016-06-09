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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	[Serializable]
	public abstract class MapCoordinateSystem : IOwnedElement {
		readonly MapCoordinateSystemCore coordSystemCore;
		object owner;
		protected internal MapCoordinateSystemCore CoordSystemCore { get { return coordSystemCore; } }
		protected internal CoordObjectFactory PointFactory { get { return coordSystemCore.PointFactory; } }
		protected internal CoordPointType PointType { get { return coordSystemCore.PointType; } }
		protected internal CoordPoint Center { get { return coordSystemCore.Center; } }
		protected internal virtual CoordBounds BoundingBox { get { return coordSystemCore.BoundingBox; } }
		protected InnerMap Map { get { return (InnerMap)owner; } }
		protected internal abstract bool NeedUpdateBoundingBox { get; }
		protected MapCoordinateSystem(object owner) {
			this.owner = owner;
			this.coordSystemCore = CreateCoreCoordSystem();
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if (owner == value)
					return;
				owner = value;
				OnOwnerChanged();
			}
		}
		#endregion
		protected abstract MapCoordinateSystemCore CreateCoreCoordSystem();
		protected virtual void OnOwnerChanged() { }
		protected internal MapUnit CoordPointToMapUnit(CoordPoint point, bool shouldNormalize) {
			return new MapUnit(coordSystemCore.CoordPointToMapUnit(point, shouldNormalize));
		}
		protected internal virtual MapUnit CoordPointToMapUnit(CoordPoint pt) {
			return new MapUnit(coordSystemCore.CoordPointToMapUnit(pt));
		}
		protected internal virtual CoordPoint MapUnitToCoordPoint(MapUnit mapUnit) {
			return coordSystemCore.MapUnitToCoordPoint(MapUnit.Normalize(mapUnit));
		}
		protected internal MapUnit ScreenPointToMapUnit(MapPoint point, MapRect viewport, Size size) {
			IMapUnit unit = coordSystemCore.ScreenPointToMapUnit(new MapPointCore(point.X, point.Y), new MapRectCore(viewport.Left, viewport.Top, viewport.Width, viewport.Height), new MapSizeCore(size.Width, size.Height));
			return new MapUnit(unit);
		}
		protected internal MapPoint MapUnitToScreenPoint(MapUnit unit, MapRect viewport, Size size) {
			MapPointCore point = coordSystemCore.MapUnitToScreenPoint(unit, new MapRectCore(viewport.Left, viewport.Top, viewport.Width, viewport.Height), new MapSizeCore(size.Width, size.Height));
			return new MapPoint(point.X, point.Y);
		}
		protected internal double CalculateKilometersScale(CoordPoint anchorPoint, double size) {
			return coordSystemCore.CalculateKilometersScale(anchorPoint, size);
		}
		protected internal double CalculateKilometersScale(CoordPoint anchorPoint, CoordPoint centerPoint, double zoomLevel, Size viewportInPixels, Size initialMapSize) {
			MapRect viewport = CalculateViewport(zoomLevel, centerPoint, viewportInPixels, initialMapSize);
			return coordSystemCore.CalculateKilometersScale(anchorPoint, InnerMap.DefaultMaxScaleLineWidth, new MapRectCore(viewport.Left, viewport.Top, viewport.Width, viewport.Height), new MapSizeCore(viewportInPixels.Width, viewport.Height));
		}
		protected internal virtual MapSize GeoToKilometersSize(CoordPoint anchorPoint, MapSize size) {
			MapSizeCore result = coordSystemCore.GeoToKilometersSize(anchorPoint, new MapSizeCore(size.Width, size.Height));
			return new MapSize(result.Width, result.Height);
		}
		protected internal virtual MapSize KilometersToGeoSize(CoordPoint anchorPoint, MapSize size) {
			MapSizeCore result = coordSystemCore.KilometersToGeoSize(anchorPoint, new MapSizeCore(size.Width, size.Height));
			return new MapSize(result.Width, result.Height);
		}
		protected internal CoordPoint MeasurePoint(CoordPoint point) {
			return coordSystemCore.MeasurePoint(point);
		}
		protected internal CoordPoint UnmeasurePoint(CoordPoint point) {
			return coordSystemCore.UnmeasurePoint(point);
		}
		protected internal MapRect CalculateViewport(double zoomLevel, CoordPoint centerPoint, Size viewportInPixels, Size initialMapSize) {
			MapRectCore result = coordSystemCore.CalculateViewport(zoomLevel, centerPoint, new MapSizeCore(viewportInPixels.Width, viewportInPixels.Height), new MapSizeCore(initialMapSize.Width, initialMapSize.Height));
			return new MapRect(result.Left, result.Top, result.Width, result.Height);
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
		public abstract CoordPoint CreatePoint(double x,  double y);
		public abstract CoordPoint CreatePoint(string input);
	}
	public class GeoMapCoordinateSystem : MapCoordinateSystem {
		ProjectionBase projection = ProjectionBase.DefaultProjection;
		protected new MapGeoCoordinateSystemCore CoordSystemCore { get { return (MapGeoCoordinateSystemCore)base.CoordSystemCore; } }
		protected ProjectionBaseCore ProjectionCore {
			get {
				ProjectionCoreWrapper projection = Projection as ProjectionCoreWrapper;
				return projection != null ? projection.ProjectionCore : null;
			}
		}
		protected internal override CoordBounds BoundingBox { get { return ProjectionCore != null ? base.BoundingBox : projection.GetBoundingBox(); } }
		protected internal override bool NeedUpdateBoundingBox { get { return false; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("GeoMapCoordinateSystemProjection"),
#endif
		Category(SRCategoryNames.Behavior),
		Editor("DevExpress.XtraMap.Design.ProjectionPickerEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor)),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public ProjectionBase Projection {
			get { return projection; }
			set {
				if (value == null)
					value = ProjectionBase.DefaultProjection;
				if (projection == value)
					return;
				projection = value;
				OnProjectionChanged();
			}
		}
		bool ShouldSerializeProjection() { return !Object.Equals(ProjectionBase.DefaultProjection, Projection); }
		void ResetProjection() { Projection = ProjectionBase.DefaultProjection; }
		internal GeoMapCoordinateSystem(InnerMap innerMap)
			: base(innerMap) {
		}
		public GeoMapCoordinateSystem()
			: this(null) {
		}
		void OnProjectionChanged() {
			CoordSystemCore.Projection = ProjectionCore;
			if (Map != null)
				Map.OnProjectionChanged();
		}
		protected override MapCoordinateSystemCore CreateCoreCoordSystem() {
			return new MapGeoCoordinateSystemCore(GeoPointFactory.Instance);
		}
		protected GeoPoint ToGeoPoint(CoordPoint pt) {
			GeoPoint point = pt as GeoPoint;
			return point != null ? point : (GeoPoint)CreatePoint(pt.GetX(), pt.GetY());
		}
		protected internal override MapUnit CoordPointToMapUnit(CoordPoint pt) {
			return ProjectionCore != null ? base.CoordPointToMapUnit(pt) : Projection.GeoPointToMapUnit(ToGeoPoint(pt));
		}
		protected internal override CoordPoint MapUnitToCoordPoint(MapUnit mapUnit) {
			return ProjectionCore != null ? base.MapUnitToCoordPoint(mapUnit) : Projection.MapUnitToGeoPoint(mapUnit);
		}
		protected internal override MapSize GeoToKilometersSize(CoordPoint anchorPoint, MapSize size) {
			return ProjectionCore != null ? base.GeoToKilometersSize(anchorPoint, size) : Projection.GeoToKilometersSize(ToGeoPoint(anchorPoint), size);
		}
		protected internal override MapSize KilometersToGeoSize(CoordPoint anchorPoint, MapSize size) {
			return ProjectionCore != null ? base.KilometersToGeoSize(anchorPoint, size) : Projection.KilometersToGeoSize(ToGeoPoint(anchorPoint), size);
		}
		public override CoordPoint CreatePoint(double x, double y) {
			return PointFactory.CreatePoint(x, y);
		}
		public override CoordPoint CreatePoint(string input) {
			return GeoPoint.Parse(input);
		}
		public override string ToString() {
			return "(GeoMapCoordinateSystem)";
		}
	}
	public class CartesianMapCoordinateSystem : MapCoordinateSystem {
		public static double ConvertCoordinate(double coordinate, MeasureUnit fromUnit, MeasureUnit toUnit) { 
			double valueInMeter = fromUnit.ToMeters(coordinate);
			return toUnit.FromMeters(valueInMeter);
		}
		public static CartesianPoint ConvertPoint(CoordPoint point, MeasureUnit fromUnit, MeasureUnit toUnit) { 
			double x = ConvertCoordinate(point.GetX(), fromUnit, toUnit);
			double y = ConvertCoordinate(point.GetY(), fromUnit, toUnit);
			return (CartesianPoint)CartesianPointFactory.Instance.CreatePoint(x, y);
		}
		bool needUpdateBoundingBox;
		protected new MapCartesianCoordinateSystemCore CoordSystemCore { get { return (MapCartesianCoordinateSystemCore)base.CoordSystemCore; } }
		protected internal override bool NeedUpdateBoundingBox { get { return needUpdateBoundingBox; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("CartesianMapCoordinateSystemMeasureUnit"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),]
		public MeasureUnit MeasureUnit {
			get { return CoordSystemCore.MeasureUnit; }
			set {
				if (CoordSystemCore.MeasureUnit == value)
					return;
				CoordSystemCore.MeasureUnit = value;
				OnMeasureUnitChanged();
			}
		}
		bool ShouldSerializeMeasureUnit() { return !Object.Equals(MeasureUnit, MapCartesianCoordinateSystemCore.DefaultMeasureUnit); }
		void ResetMeasureUnit() { MeasureUnit = MapCartesianCoordinateSystemCore.DefaultMeasureUnit; }
		public CartesianMapCoordinateSystem() : base(null) { }
		void OnMeasureUnitChanged() {
			if (Map != null)
				Map.OnMeasureUnitChanged();
		}
		protected override MapCoordinateSystemCore CreateCoreCoordSystem() {
			return new MapCartesianCoordinateSystemCore(CartesianPointFactory.Instance);
		}
		protected override void OnOwnerChanged() {
			ResetBoundingBox();
		}
		protected internal override void SetNeedUpdateBoundingBox(bool needUpdate) {
			this.needUpdateBoundingBox = needUpdate;
		}
		protected internal override void UpdateBoundingBox(CoordBounds rect) {
			bool boundsUpdated = CoordSystemCore.UpdateBoundingBox(rect);
			if (Map == null)
				return;
			Map.UpdateAnchorPoint(Center);
			if(boundsUpdated)
				Map.UpdateNavigationPanel();
		}
		protected internal override void ResetBoundingBox() {
			CoordSystemCore.ResetBoundingBox();
		}
		protected internal override void CorrectBoundingBox() {
			CoordSystemCore.CorrectBoundingBox();
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
		public override string ToString() {
			return "(CartesianMapCoordinateSystem)";
		}
	}
}
