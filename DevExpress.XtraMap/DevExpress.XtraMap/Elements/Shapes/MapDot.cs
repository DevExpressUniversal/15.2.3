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
using System.Drawing;
namespace DevExpress.XtraMap {
	public enum MapDotShapeKind {
		Rectangle,
		Circle
	}
	public class MapDot : MapShape, ILocatableRenderItem, ISupportCoordLocation, ITemplateGeometryItem, IClusterable, IClusterItem, IPointCore {
		internal const double DefaultSize = 5.0;
		internal const MapDotShapeKind DefaultShapeKind = MapDotShapeKind.Circle;
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		CoordPoint location;
		double size = DefaultSize;
		MapDotShapeKind shapeKind = DefaultShapeKind;
		MapUnit? unitLocation = null;
		MapRect hitTestBounds = MapRect.Empty;
		TemplateGeometryBase template;
		TemplateGeometryBase Template {
			get {
				if(template == null)
					template = TemplateGeometryBase.CreateTemplate(MapUtils.ToTemplateGeometryType(DefaultShapeKind));
				return template;
			}
			set {
				if(template == value) return;
				template = value;
			}
		}
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Dot; } }
#endif
		protected internal override GeometryType GeometryType { get { return GeometryType.Screen; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapDotLocation"),
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
				unitLocation = null;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Location);
			}
		}
		void ResetLocation() { Location = UnitConverter.PointFactory.CreatePoint(0, 0); }
		bool ShouldSerializeLocation() { return Location != UnitConverter.PointFactory.CreatePoint(0, 0); }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapDotSize"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultSize)]
		public double Size {
			get { return size; }
			set {
				if (size == value)
					return;
				size = value < 0 ? DefaultSize : value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapDotShapeKind"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultShapeKind)]
		public MapDotShapeKind ShapeKind {
			get { return shapeKind; }
			set {
				if (shapeKind == value)
					return;
				shapeKind = value;
				OnShapeKindChanged();
			}
		}
		public MapDot() {
			this.location = UnitConverter.PointFactory.CreatePoint(0, 0);
		}
		void OnShapeKindChanged() {
			Template = TemplateGeometryBase.CreateTemplate(MapUtils.ToTemplateGeometryType(ShapeKind));
			UpdateItem(MapItemUpdateType.Layout);
		}
		void CalculateHitTestBounds() {
			MapUnit unitPos = UnitConverter.CoordPointToMapUnit(Location, true);
			this.hitTestBounds = new MapRect(unitPos.X, unitPos.Y, 0, 0);
		}
		MapUnit GetUnitLocation() {
			if(!this.unitLocation.HasValue)
				this.unitLocation = UnitConverter.CoordPointToMapUnit(location, true) * MapShape.RenderScaleFactor;
			return this.unitLocation.Value;
		}
		protected override MapRect CalculateBounds() {
			CalculateHitTestBounds();
			double radius = Size / 2.0;
			MapPoint location = UnitConverter.CoordPointToScreenPoint(Location, false);
			return CalculateLocatableItemBounds(location, radius);
		}
		protected override CoordBounds CalculateNativeBounds() {
			return new CoordBounds(Location, Location);
		}
		protected override IMapItemGeometry CreateShapeGeometry() {
			UnitGeometry result = new UnitGeometry() { Bounds = base.Bounds };
			result.Points = Template.Points;
			return result;
		}
		protected override MapRect GetHitTestUnitBounds() {
			return this.hitTestBounds;
		}
		protected override IHitTestGeometry CreateHitTestGeometry() {
			MapPoint pixelLocation = UnitConverter.CoordPointToScreenPoint(location);
			return new RectangleScreenHitTestGeometry(pixelLocation, new Size((int)size, (int)size), new MapPoint(0.5, 0.5));
		}
		protected internal override CoordPoint GetCenterCore() {
			return Location;
		}
		public override string ToString() {
			return "(MapDot)";
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
		#region ILocatableRenderItem Members
		MapUnit ILocatableRenderItem.Location {
			get {
				return GetUnitLocation();
			}
		}
		Size ILocatableRenderItem.SizeInPixels {
			get { return new Size((int)this.size, (int)this.size); }
		}
		MapPoint ILocatableRenderItem.Origin {
			get { return new MapPoint(0.5, 0.5); }
		}
		MapPoint ILocatableRenderItem.StretchFactor {
			get { return new MapPoint(1.0, 1.0); }
		}
		void ILocatableRenderItem.ResetLocation() {
			this.unitLocation = null;
		}
		#endregion
		#region ITemplateGeometry Members
		TemplateGeometryType ITemplateGeometryItem.Type {
			get {
				return shapeKind == MapDotShapeKind.Circle ? TemplateGeometryType.Circle : TemplateGeometryType.Square;
			}
		}
		double ITemplateGeometryItem.StretchFactor {
			get {
				return size;
			}
		}
		#endregion
		#region IClusterable Members
		IClusterItem IClusterable.CreateInstance() {	   
			return new MapDot();
		}
		#endregion
		#region IClusterItemCore Members
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return GetUnitLocation() / MapShape.RenderScaleFactor;
		}
		object IClusterItemCore.Owner {
			get { return Owner; }
			set {
				if (Owner == value)
					return;
				MapUtils.SetOwner(this, value);
			}
		}
		#endregion
		#region IClusterItem Members
		IList<IClusterable> IClusterItem.ClusteredItems {
			get { return clusteredItems; }
			set { clusteredItems = value; }
		}
		void IClusterItem.ApplySize(double size) {
			Size = size;
		}
		string IClusterItem.Text { get; set; }
		#endregion       
	}
}
