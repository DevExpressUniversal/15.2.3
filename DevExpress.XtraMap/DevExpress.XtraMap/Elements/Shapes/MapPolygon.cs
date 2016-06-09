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
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public class CoordPointCollection : SupportCallbackCollection<CoordPoint>, IOwnedElement {
		object owner;
		protected internal ILockableObject LockableObject { get { return (ILockableObject)owner; } }
		protected internal object Locker { get { return LockableObject != null ? LockableObject.UpdateLocker : null; } }
		public CoordPointCollection()
			: base(DXCollectionUniquenessProviderType.None) {
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		public new CoordPoint this[int index] {
			get { return InnerList[index]; }
			set {
				InnerList[index] = value;
				OnInsertComplete(index, value);
			}
		}
		protected override void OnValidate(CoordPoint value) {
			if(!CoordinateSystemHelper.IsNumericCoordPoint(value))
				Exceptions.ThrowIncorrectCoordPointException();
		}
		public override int Add(CoordPoint value) {
			int res = -1;
			if(Locker == null)
				res = base.Add(value);
			else
				lock (Locker) {
					res = base.Add(value);
				}
			return res;
		}
		protected override bool RemoveAtCore(int index) {
			bool res = false;
			if(Locker == null)
				res = base.RemoveAtCore(index);
			else
				lock (Locker) {
					res = base.RemoveAtCore(index);
				}
			return res;
		}
		protected override bool OnClear() {
			bool res = false;
			if(Locker == null)
				res = base.OnClear();
			else
				lock (Locker) {
					res = base.OnClear();
				}
			return res;
		}
	}
	public class MapPolygon : MapShape, ISupportCoordPoints, IPolygonCore {
		CoordPointCollection points;
		double? area = null;
#if DEBUGTEST
		protected internal override MapItemType ItemType { 
			get { return MapItemType.Polygon; } 
		}
#endif
		IList<CoordPoint> ISupportCoordPoints.Points {
			get { return GetItemPoints(); } 
		}
		protected double Area {
			get {
				if (!area.HasValue)
					area = CentroidHelper.CalculatePolygonArea(this);
				return area.Value;
			}
		}
		[Category(SRCategoryNames.Layout),
		TypeConverter(typeof(CollectionConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.CoordPointCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPolygonPoints")
#else
	Description("")
#endif
]
		public CoordPointCollection Points {
			get { return points; }
			set {
				if (points == value)
					return;
				((IChangedCallbackOwner)points).SetParentCallback(null);
				((IOwnedElement)points).Owner = null;
				if (value == null)
					value = new CoordPointCollection();
				points = value;
				area = null;
				((IOwnedElement)points).Owner = this;
				((IChangedCallbackOwner)points).SetParentCallback(OnPointsCollectionChanged);
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		void ResetPoints() { Points = MapUtils.CreateOwnedPoints(this); }
		bool ShouldSerializePoints() { return Points.Count > 0; }
		public MapPolygon() {
			this.points = MapUtils.CreateOwnedPoints(this);
			((IChangedCallbackOwner)points).SetParentCallback(OnPointsCollectionChanged);
		}
		#region IPolygonCore implementation
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return Points[index];
		}
		int IPointContainerCore.PointCount { get { return Points.Count; } }
		CoordBounds IPolygonCore.GetBounds() {
			MapRect bounds = Bounds;
			return new CoordBounds(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
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
		#endregion
		void OnPointsCollectionChanged() {
			UpdateBoundingRect();
			UpdateItem(MapItemUpdateType.Layout);
		}	   
		protected override MapRect CalculateBounds() {
			if (Points.Count > 0) {
				MapUnit unit = UnitConverter.CoordPointToMapUnit(Points[0], false);
				double minX = unit.X;
				double minY = unit.Y;
				double maxX = unit.X;
				double maxY = unit.Y;
				for (int i = 1; i < Points.Count; i++) {
					unit = UnitConverter.CoordPointToMapUnit(Points[i], false);
					minX = Math.Min(minX, unit.X);
					minY = Math.Min(minY, unit.Y);
					maxX = Math.Max(maxX, unit.X);
					maxY = Math.Max(maxY, unit.Y);
				}
				return new MapRect(minX, minY, maxX - minX, maxY - minY);
			}
			return MapRect.Empty;
		}
		protected override CoordBounds CalculateNativeBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected override IMapItemGeometry CreateShapeGeometry() {
			UnitGeometry result = new UnitGeometry() { Bounds = base.Bounds };
			if(Points.Count == 0) {
				result.Points = new MapUnit[0];
				return result;
			}		   
			List<MapUnit> units = new List<MapUnit>();
			for (int i = 0; i < Points.Count; i++) {
				MapUnit unit = UnitConverter.CoordPointToMapUnit(Points[i], false);
				units.Add(new MapUnit(unit.X * MapShape.RenderScaleFactor, unit.Y * MapShape.RenderScaleFactor));
			}
			if(units[0] != units[units.Count - 1])
				units.Add(units[0]);
			result.Points = units.ToArray();	
			return result;
		}
		protected internal override CoordPoint GetCenterCore() {
			CoordPoint centroid = CentroidHelper.CalculatePolygonCentroid(UnitConverter.PointFactory, this, Area);
			return MapUtils.IsValidPoint(centroid) ? centroid : base.GetCenterCore();
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return Points;
		}
		public override string ToString() {
			return "(MapPolygon)";
		}
	}
}
