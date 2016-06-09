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
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public class InnerBoundary : IPointContainerCore, IChangedCallbackOwner, ILockableObject, IOwnedElement {
		CoordPointCollection points;
		Action callback = null;
		object owner;
		protected internal MapPathSegment Segment { get { return (MapPathSegment)owner; } }
		[Category(SRCategoryNames.Layout), 
		TypeConverter(typeof(CollectionConverter)), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.CoordPointCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("InnerBoundaryPoints")
#else
	Description("")
#endif
]
		public CoordPointCollection Points {
			get { return points; }
			set {
				if(points == value)
					return;
				((IOwnedElement)points).Owner = null;
				if(value == null)
					value = MapUtils.CreateOwnedPoints(this);
				points = value;
				((IOwnedElement)points).Owner = this;
				RaiseChanged();
			}
		}
		void ResetPoints() { Points = MapUtils.CreateOwnedPoints(this); }
		bool ShouldSerializePoints() { return Points.Count > 0; }
		public InnerBoundary() {
			this.points = MapUtils.CreateOwnedPoints(this);
		}
		#region IPointContainerCore Members
		int IPointContainerCore.PointCount {
			get { return Points.Count; }
		}
		void IPointContainerCore.AddPoint(CoordPoint point) {
			points.Add(point);
		}
		void IPointContainerCore.LockPoints() {
			Points.BeginUpdate();
		}
		void IPointContainerCore.UnlockPoints() {
			Points.EndUpdate();
		}
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return Points[index];
		}
		#endregion
		#region IChangedCallbackOwner members
		void IChangedCallbackOwner.SetParentCallback(Action callback) {
			this.callback = callback;
		}
		#endregion
		#region ILockableObject implementation
		object ILockableObject.UpdateLocker { get { return Segment != null ? ((ILockableObject)Segment).UpdateLocker : null; } }
		#endregion
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		void RaiseChanged() {
			if(callback != null)
				callback();
		}
		public override string ToString() {
			return "(InnerBoundary)";
		}
	}
	public class InnerBoundaryCollection : SupportCallbackCollection<InnerBoundary> {
		readonly MapPathSegment segment;
		public InnerBoundaryCollection(MapPathSegment segment) {
			Guard.ArgumentNotNull(segment, "PathSegment");
			this.segment = segment;
		}
		protected override void OnInsertComplete(int index, InnerBoundary value) {
			((IOwnedElement)value).Owner = segment;
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, InnerBoundary value) {
			((IOwnedElement)value).Owner = null;
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			for(int i = 0; i < Count; i++)
				((IOwnedElement)this[i]).Owner = null;
			return base.OnClear();
		}
		protected override void ItemCallbackCore() {
			segment.OnInnerBoundaryCollectionChanged();
		}
		public override string ToString() {
			return "(InnerBoundaryCollection)";
		}
	}
	public class MapPathSegment : MapSegmentBase, IPathSegmentCore, ISupportCoordPoints, IHitTestableElement, IDisposable, ILockableObject {
		const bool DefaultIsClosed = true;
		const bool DefaultIsFilled = true;
		bool isClosed = DefaultIsClosed;
		bool isFilled = DefaultIsFilled;
		bool disposed = false;
		IHitTestGeometry hitTestGeometry;
		RegionRange regionRange = RegionRange.Empty;
		HitTestKey hitTestKey;
		readonly InnerBoundaryCollection innerBoundaries;
		CoordPointCollection points;
		MapRect bounds = MapRect.Empty;
		protected bool ShouldRecreateHitTestGeometry {
			get {
				if(hitTestGeometry != null && Layer.Map.Capture)
					return false;
				return hitTestGeometry == null;
			}
		}
		protected internal override MapItemStyle DefaultStyle {
			get {
				IMapItemStyleProvider provider = MapPath != null ? MapPath.DefaultStyleProvider : MapItemStyleProvider.Default;
				return isFilled ? provider.PolygonSegmentStyle : provider.PolylineSegmentStyle;
			}
		}
		protected internal override MapShape MapPath { get { return Owner as MapPath; } }
		protected internal MapRect Bounds { get { return bounds; } }
		[Category(SRCategoryNames.Appearance), 
		DefaultValue(DefaultIsClosed),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPathSegmentIsClosed")
#else
	Description("")
#endif
]
		public bool IsClosed {
			get { return isClosed; }
			set {
				if(isClosed == value)
					return;
				isClosed = value;
				OnClosedFilledChanged();
			}
		}
		[Category(SRCategoryNames.Appearance), 
		DefaultValue(DefaultIsFilled),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPathSegmentIsFilled")
#else
	Description("")
#endif
]
		public bool IsFilled {
			get { return isFilled; }
			set {
				if(isFilled == value)
					return;
				isFilled = value;
				OnClosedFilledChanged();
			}
		}
		[Category(SRCategoryNames.Layout), 
		TypeConverter(typeof(CollectionConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.CoordPointCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPathSegmentPoints")
#else
	Description("")
#endif
]
		public CoordPointCollection Points {
			get { return points; }
			set {
				if(points == value)
					return;
				((IChangedCallbackOwner)points).SetParentCallback(null);
				((IOwnedElement)points).Owner = null;
				if(value == null)
					value = new CoordPointCollection();
				points = value;
				((IOwnedElement)points).Owner = this;
				((IChangedCallbackOwner)points).SetParentCallback(OnPointsCollectionChanged);
				UpdateItem();
				InvalidateRender();
			}
		}
		void ResetPoints() { Points = MapUtils.CreateOwnedPoints(this); }
		bool ShouldSerializePoints() { return Points.Count > 0; }
		[Category(SRCategoryNames.Layout), 
		TypeConverter(typeof(CollectionConverter)), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public InnerBoundaryCollection InnerBoundaries { get { return innerBoundaries; } }
		public MapPathSegment() {
			this.innerBoundaries = new InnerBoundaryCollection(this);
			((IChangedCallbackOwner)innerBoundaries).SetParentCallback(OnInnerBoundaryCollectionChanged);
			this.points = MapUtils.CreateOwnedPoints(this);
			((IChangedCallbackOwner)points).SetParentCallback(OnPointsCollectionChanged);
		}
		internal void OnInnerBoundaryCollectionChanged() {
			UpdateItem();
			InvalidateRender();
		}
		IList<CoordPoint> ISupportCoordPoints.Points { get { return Points; } }
		~MapPathSegment() {
			Dispose(false);
		}
		#region IPathSegmentCore
		IPointContainerCore IPathSegmentCore.GetInnerCountour(int index) {
			return InnerBoundaries[index];
		}
		int IPathSegmentCore.InnerContourCount { get { return InnerBoundaries.Count; } }
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return Points[index];
		}
		int IPointContainerCore.PointCount { get { return Points.Count; } }
		bool IPathSegmentCore.IsClosed {
			get {
				return IsClosed;
			}
			set {
				IsClosed = value;
			}
		}
		bool IPathSegmentCore.IsFilled {
			get {
				return IsFilled;
			}
			set {
				IsFilled = value;
			}
		}
		void IPointContainerCore.AddPoint(CoordPoint point) {
			Points.Add(point);
		}
		void IPointContainerCore.LockPoints() {
			Points.BeginUpdate();
		}
		void IPointContainerCore.UnlockPoints() {
			Points.EndUpdate();
		}
		IPointContainerCore IPathSegmentCore.CreateInnerContour() {
			InnerBoundary contour = new InnerBoundary();
			InnerBoundaries.Add(contour);
			return contour;
		}
		CoordBounds IPolygonCore.GetBounds() {
			return new CoordBounds(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom);
		}
		#endregion
		#region IHitTestableElement implementation
		IHitTestableOwner IHitTestableElement.Owner { get { return (IHitTestableOwner)MapPath; } }
		IEnumerable<IHitTestableElement> IHitTestableElement.HitTest(MapUnit unit, Point point) {
			IHitTestGeometry geometry = HitTestGeometry;
			if(geometry == null)
				return new List<IHitTestableElement>();
			IUnitHitTestGeometry unitGeometry = geometry as IUnitHitTestGeometry;
			if(unitGeometry != null && unitGeometry.HitTest(unit))
				return new IHitTestableElement[] { MapPath };
			IScreenHitTestGeometry screenGeometry = geometry as IScreenHitTestGeometry;
			if(screenGeometry != null && screenGeometry.HitTest(point))
				return new IHitTestableElement[] { MapPath };
			return new IHitTestableElement[0];
		}
		MapRect IHitTestableElement.UnitBounds {
			get { return bounds; }
		}
		RegionRange IHitTestableElement.RegionRange {
			get { return regionRange; }
			set { regionRange = value; }
		}
		bool IHitTestableElement.IsHitTestVisible {
			get { return MapPath != null ? MapPath.IsHitTestVisible && MapPath.ActualVisible : false; }
		}
		GeometryType IHitTestableElement.GeometryType {
			get { return GeometryType.Unit; }
		}
		HitTestKey IHitTestableElement.Key { get { return hitTestKey; } set { hitTestKey = value; } }
		object IHitTestableElement.Locker { get { return MapPath.UpdateLocker; } }
		#endregion
		#region ILockableObject implementation
		object ILockableObject.UpdateLocker { get { return MapPath != null ? MapPath.UpdateLocker : null; } }
		#endregion
		void OnClosedFilledChanged() {
			ResetStyle();
			UpdateItem();
			InvalidateRender();
		}
		void OnPointsCollectionChanged() {
			RaiseChanged();
		}
		void ClosingPoints(List<MapUnit> points) {
			if(points[0] != points[points.Count - 1])
				points.Add(points[0]);
		}
		IHitTestGeometry CreateArealHitTestGeometry() {
			return PathHitTestGeometry.CreateFromUnitGeometry(Geometry);
		}
		IHitTestGeometry CreateLinearHitTestGeometry() {
			int count = isClosed ? Points.Count + 1 : Points.Count;
			PointF[] points = new PointF[count];
			for(int i = 0; i < Points.Count; i++)
				points[i] = Layer != null ? UnitConverter.CoordPointToScreenPoint(Points[i]).ToPoint() : Point.Empty;
			if(isClosed)
				points[count - 1] = points[0];
			return new StrokeHitTestGeometry(points, ActualStyle.StrokeWidth);
		}
		protected internal IHitTestGeometry HitTestGeometry {
			get {
				if(Geometry == null)
					return null;
				if(ShouldRecreateHitTestGeometry) {
					hitTestGeometry = CreateRealHitTestGeometry();
					UpdateHitGeometryInPool(this);
				}
				return hitTestGeometry;
			}
		}
		protected internal override void ReleaseHitTestGeometry() {
			MapUtils.DisposeObject(hitTestGeometry);
			hitTestGeometry = null;
			base.ReleaseHitTestGeometry();
		}
		protected virtual void Dispose(bool disposing) {
			if (disposed)
				return;
			disposed = true;
		}
		protected override IMapItemGeometry CreateGeometry() {
			List<MapUnit> units = new List<MapUnit>();
			for(int i = 0; i < Points.Count; i++) {
				MapUnit unit = UnitConverter.CoordPointToMapUnit(Points[i], false);
				units.Add(new MapUnit(unit.X * MapShape.RenderScaleFactor, unit.Y * MapShape.RenderScaleFactor));
			}
			if(IsClosed && units.Count > 1)
				ClosingPoints(units);
			UnitGeometryType type = IsFilled ? UnitGeometryType.Areal : UnitGeometryType.Linear;
			IList<MapUnit[]> inners = new List<MapUnit[]>();
			foreach(InnerBoundary contour in InnerBoundaries) {
				CoordPointCollection contourPoints = contour.Points;
				List<MapUnit> points = new List<MapUnit>();
				for(int i = 0; i < contourPoints.Count; i++) {
					MapUnit unit = UnitConverter.CoordPointToMapUnit(contourPoints[i], false);
					points.Add(unit * MapShape.RenderScaleFactor);
				}
				if(IsClosed && points.Count > 1)
					ClosingPoints(points);
				if(points.Count > 0)
					inners.Add(points.ToArray());
			}
			return new PathUnitGeometry() { Bounds = this.Bounds, Type = type, Points = units.ToArray(), InnerContours = inners };
		}
		protected IHitTestGeometry CreateRealHitTestGeometry() {
			return isFilled ? CreateArealHitTestGeometry() : CreateLinearHitTestGeometry();
		}
		protected override StyleMergerBase CreateStyleMerger() {
			return new MapPathSegmentStyleMerger(this);
		}
		protected override void MergeStyles() {
			StyleMerger.Merge(ActualStyle);
		}
		protected internal override void UpdateBounds() {
			if(Points.Count > 0) {
				MapUnit unit = UnitConverter.CoordPointToMapUnit(Points[0], false);
				double minX = unit.X;
				double minY = unit.Y;
				double maxX = unit.X;
				double maxY = unit.Y;
				for(int i = 1; i < Points.Count; i++) {
					unit = UnitConverter.CoordPointToMapUnit(Points[i], false);
					minX = Math.Min(minX, unit.X);
					minY = Math.Min(minY, unit.Y);
					maxX = Math.Max(maxX, unit.X);
					maxY = Math.Max(maxY, unit.Y);
				}
				this.bounds = new MapRect(minX, minY, maxX - minX, maxY - minY);
			}
			base.UpdateBounds();
		}
		public override string ToString() {
			return "(MapPathSegment)";
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	public class MapPathSegmentCollection : MapSegmentCollectionBase<MapPathSegment> {
		public MapPathSegmentCollection(object owner)
			: base(owner) {
		}
	}
	public class MapPath : MapPathBase<MapPathSegment>, IPathCore, ISupportCoordPoints, IHitTestableOwner {
		MapPathSegmentHelper segmentHelper;
		[Category(SRCategoryNames.Layout), 
		TypeConverter(typeof(CollectionConverter)), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPathSegments")
#else
	Description("")
#endif
]
		public new MapPathSegmentCollection Segments {
			get { return (MapPathSegmentCollection)base.Segments; }
			set { base.Segments = value; }
		}
		void ResetSegments() { Segments = null; }
		bool ShouldSerializeSegments() { return Segments != null; }
		protected internal override MapRect BoundsForTitle { get { return this.segmentHelper.GetMaxSegmentBounds(); } }
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Path; } }
#endif
		public MapPath() {
			this.segmentHelper = new MapPathSegmentHelper(this);
		}
		#region IPathCore
		int IPathCore.SegmentCount {
			get {
				return Segments.Count;
			}
		}
		IPathSegmentCore IPathCore.CreateSegment() {
			MapPathSegment segment = new MapPathSegment();
			Segments.Add(segment);
			return segment;
		}
		IPathSegmentCore IPathCore.GetSegment(int index) {
			return Segments[index];
		}
		#endregion
		#region ISupportCoordPoints
		IList<CoordPoint> ISupportCoordPoints.Points { get { return GetItemPoints(); } }
		#endregion
		#region IHitTestableOwner implementation
		IHitTestableElement[] IHitTestableOwner.Elements {
			get { return Segments.ToArray(); }
		}
		#endregion
		protected IList<CoordPoint> ExtractCoordPoints() {
			return Segments.SelectMany<MapPathSegment, CoordPoint>(s => {
				return s.Points;
			}).ToList();
		}
		protected IList<MapUnit> GetGeometryPoints() {
			return Segments.SelectMany<MapPathSegment, MapUnit>(s => {
				return s.Geometry.GetPoints();
			}).ToList();
		}
		protected override void SetOwner(object value) {
			base.SetOwner(value);
			segmentHelper.UpdatePointFactory(UnitConverter.PointFactory);
		}
		protected override MapSegmentCollectionBase<MapPathSegment> CreateSegmentCollection() {
			return new MapPathSegmentCollection(this);
		}
		protected override MapRect CalculateBounds() {
			MapRect bounds = MapRect.Invalid;
			foreach(MapPathSegment segment in Segments) {
				segment.UpdateBounds();
				bounds = MapRect.Union(bounds, segment.Bounds);
			}
			return bounds;
		}
		protected override void Invalidate() {
			segmentHelper.Reset();
		}
		protected override CoordBounds CalculateNativeBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return ExtractCoordPoints();
		}
		protected internal override Color GetTitleColor() {
			if(Segments.Count == 0)
				return Color.Empty;
			return segmentHelper.GetLargestSegmentTextColor();
		}
		protected internal override Color GetTitleGlowColor() {
			if(Segments.Count == 0)
				return Color.Empty;
			return segmentHelper.GetLargestSegmentTextGlowColor();
		}
		protected internal override CoordPoint GetCenterCore() {
			CoordPoint center = null;
			if(Segments.Count > 0)
				center = segmentHelper.GetMaxSegmentCenter();
			return center ?? base.GetCenterCore();
		}
		protected internal override IList<MapUnit> GetUnitPoints() {
			return GetGeometryPoints();
		}
		protected internal override IList<IList<MapUnit>> GetSegmentGeometries(IList<MapUnit> unitPoints) {
			IList<IList<MapUnit>> segmentsGeometryList = new List<IList<MapUnit>>();
			foreach(MapPathSegment segment in Segments) {
				PathUnitGeometry pathUnitGeometry = (PathUnitGeometry)segment.Geometry;
				segmentsGeometryList.Add(new List<MapUnit>(pathUnitGeometry.Points));
				foreach(MapUnit[] innerSegmentPoints in pathUnitGeometry.InnerContours)
					segmentsGeometryList.Add(new List<MapUnit>(innerSegmentPoints));
			}
			return segmentsGeometryList;
		}
		protected override void SetResourceHolder(IRenderer renderer, IRenderItemProvider provider) {
		}
		public override string ToString() {
			return "(MapPath)";
		}
	}
}
