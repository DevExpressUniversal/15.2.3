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
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public class MapRectangle : MapShape, IRectangleCore, IPolygonCore {
		internal const double DefaultWidth = 0.0;
		internal const double DefaultHeight = 0.0;
		public static MapRectangle CreateByCenter(MapCoordinateSystem coordSystem, CoordPoint center, double width, double height) {
			MapRectangle result = new MapRectangle() { Height = height, Width = width };
			result.center = center;
			result.Location = BoundingRectItemHelper.CalculateLocation(coordSystem.CoordSystemCore, center, width, height);
			return result;
		}
		CoordPoint location;
		CoordPoint center;
		double width = DefaultWidth;
		double height = DefaultHeight;
		IList<CoordPoint> innerVertices;
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Rectangle; } }
#endif
		internal IList<CoordPoint> InnerVertices {
			get {
				if (innerVertices == null)
					innerVertices = GetVertices();
				return innerVertices;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapRectangleLocation"),
#endif
		Category(SRCategoryNames.Layout),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		]
		public CoordPoint Location {
			get { return location; }
			set {
				if (location == value)
					return;
				if (!CoordinateSystemHelper.IsNumericCoordPoint(value))
					Exceptions.ThrowIncorrectCoordPointException();
				location = value;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Location);
			}
		}
		void ResetLocation() { Location = UnitConverter.PointFactory.CreatePoint(0, 0); }
		bool ShouldSerializeLocation() { return Location != UnitConverter.PointFactory.CreatePoint(0, 0); }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapRectangleWidth"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultWidth)]
		public double Width {
			get { return width; }
			set {
				if (width == value)
					return;
				width = value;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapRectangleHeight"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultHeight)]
		public double Height {
			get { return height; }
			set {
				if (height == value)
					return;
				height = value;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		#region IPolygonCore implementation
		CoordPoint IPointContainerCore.GetPoint(int index) {
			return InnerVertices[index];
		}
		int IPointContainerCore.PointCount { get { return InnerVertices.Count; } }
		CoordBounds IPolygonCore.GetBounds() {
			MapRect bounds = Bounds;
			return new CoordBounds(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
		}
		void IPointContainerCore.AddPoint(CoordPoint point) {
		}
		void IPointContainerCore.LockPoints() {
		}
		void IPointContainerCore.UnlockPoints() {
		}
		#endregion
		public MapRectangle() {
			this.location = UnitConverter.PointFactory.CreatePoint(0, 0);
		}
		void ResetVertices() {
			this.innerVertices = null;
		}
		CoordPoint CalculateNativeCenter() {
			MapSize size = UnitConverter.MeasureUnitToCoordSize(Location, new MapSize(Width / 2.0, Height / 2.0));
			return UnitConverter.PointFactory.CreatePoint(Location.GetX() + size.Width, Location.GetY() - size.Height);
		}
		internal List<CoordPoint> GetVertices() {
			MapSize offset = UnitConverter.MeasureUnitToCoordSize(Location, new MapSize(Width, Height));
			CoordPoint leftTop = Location;
			CoordPoint rightTop = UnitConverter.PointFactory.CreatePoint(Location.GetX() + offset.Width, Location.GetY());
			CoordPoint rightBottom = UnitConverter.PointFactory.CreatePoint(Location.GetX() + offset.Width, Location.GetY() - offset.Height);
			CoordPoint leftBottom = UnitConverter.PointFactory.CreatePoint(Location.GetX(), Location.GetY() - offset.Height);
			return new List<CoordPoint>() { leftTop, rightTop, rightBottom, leftBottom, leftTop };
		}
		protected override IMapItemGeometry CreateShapeGeometry() {
			MapUnit unit1 = UnitConverter.CoordPointToMapUnit(Location, false);
			MapSize size = UnitConverter.MeasureUnitToCoordSize(Location, new MapSize(Width, Height));
			CoordPoint coordPoint = UnitConverter.PointFactory.CreatePoint(Location.GetX() + size.Width, Location.GetY() - size.Height);
			MapUnit unit2 = UnitConverter.CoordPointToMapUnit(coordPoint, false);
			UnitGeometry geometry = new UnitGeometry() { Bounds = base.Bounds };
			MapUnit[] points = new MapUnit[5];
			points[0] = unit1 * RenderScaleFactor;
			points[1] = new MapUnit(unit2.X * RenderScaleFactor, unit1.Y * RenderScaleFactor);
			points[2] = unit2 * RenderScaleFactor;
			points[3] = new MapUnit(unit1.X * RenderScaleFactor, unit2.Y * RenderScaleFactor);
			points[4] = points[0];
			geometry.Points = points;
			return geometry; 
		}
		protected override MapRect CalculateBounds() {
			ResetVertices();
			MapUnit unit1 = UnitConverter.CoordPointToMapUnit(Location, false);
			MapSize size = UnitConverter.MeasureUnitToCoordSize(Location, new MapSize(Width, Height));
			CoordPoint coordPoint = UnitConverter.PointFactory.CreatePoint(Location.GetX() + size.Width, Location.GetY() - size.Height);
			MapUnit unit2 = UnitConverter.CoordPointToMapUnit(coordPoint, false);
			return new MapRect(unit1.X, unit1.Y, unit2.X - unit1.X, unit2.Y - unit1.Y);
		}
		protected override CoordBounds CalculateNativeBounds() {
			MapSize size = UnitConverter.MeasureUnitToCoordSize(Location, new MapSize(Width, Height));
			return new CoordBounds(Location, Location.Offset(size.Width, -size.Height));
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			MapUnit p1 = new MapUnit(Bounds.Left, Bounds.Top);
			MapUnit p2 = new MapUnit(Bounds.Right, Bounds.Bottom);
			return new List<CoordPoint> { UnitConverter.MapUnitToCoordPoint(p1), UnitConverter.MapUnitToCoordPoint(p2) };
		}
		protected internal override CoordPoint GetCenterCore() {
			return center ?? CalculateNativeCenter();
		}
		public override string ToString() {
			return "(MapRectangle)";
		}
	}
}
