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
using DevExpress.Utils.Editors;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public enum MarkerType {
		Default,
		Square,
		Diamond,
		Triangle,
		InvertedTriangle,
		Circle,
		Plus,
		Cross,
		Star5,
		Star6,
		Star8,
		Pentagon,
		Hexagon
	}
	public class MapBubble : MapShape, ILocatableRenderItem, ISupportCoordLocation, IMapChartDataItem, IMapChartGroupDataItem, ITemplateGeometryItem, IMapChartItem, IKeyColorizerElement, IPointCore {
		const double DefaultBubbleValue = 0.0;
		internal const int DefaultPixelSize = 20;
		MarkerType markerType = MarkerType.Default;
		TemplateGeometryBase template;
		double itemValue = DefaultBubbleValue;
		object groupKey;
		object argument;
		CoordPoint location;
		MapUnit? unitLocation = null;
		int size = DefaultPixelSize;
		int valueSizeInPixels = DefaultPixelSize;
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Bubble; } }
#endif
		protected VectorItemsLayer ItemsLayer { get { return (VectorItemsLayer)Layer; } }
		protected BubbleChartDataAdapter ChartData { get { return Owner as BubbleChartDataAdapter; } }
		protected internal double DefaultValue { get { return DefaultBubbleValue; } }
		protected internal int ValueSizeInPixels { get { return valueSizeInPixels; } }
		internal int ActualSizeInPixels { get { return ChartData != null ? ValueSizeInPixels : Size; } }
		protected internal MarkerType ActualMarkerType { get { return ChartData != null && MarkerType == MarkerType.Default ? ChartData.MarkerType : MarkerType; } }
		protected MarkerType TemplateMarkerType { get; set; }
		[Browsable(false), DefaultValue(null)]
		public object Group {
			get { return groupKey; }
			set {
				if(object.Equals(groupKey, value))
					return;
				groupKey = value;
				InvalidateRender();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("PieSegmentArgument"),
#endif
		DefaultValue(null), Category(SRCategoryNames.Data),
		Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Argument {
			get { return argument; }
			set {
				if(object.Equals(argument, value))
					return;
				argument = value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[CLSCompliant(false)]
		protected internal TemplateGeometryBase Template {
			get {
				if(template == null || ActualMarkerType != TemplateMarkerType) {
					TemplateMarkerType = ActualMarkerType;
					template = TemplateGeometryBase.CreateTemplate(MapUtils.ToTemplateGeometryType(ActualMarkerType));
				}
				return template;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapBubbleLocation"),
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
	DevExpressXtraMapLocalizedDescription("MapBubbleMarkerType"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(MarkerType.Default)]
		public MarkerType MarkerType {
			get { return markerType; }
			set {
				if (markerType == value)
					return;
				markerType = value;
				OnMarketTypeChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapBubbleSize"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultPixelSize)]
		public int Size {
			get { return size; }
			set {
				if (size == value)
					return;
				size = value < 0 ? DefaultPixelSize : value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapBubbleValue"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(DefaultBubbleValue)]
		public double Value {
			get { return itemValue; }
			set {
				if (object.Equals(itemValue, value))
					return;
				itemValue = value;
			}
		}
		public MapBubble() {
			this.location = UnitConverter.PointFactory.CreatePoint(0, 0);
		}
		#region IKeyColorizerElement implementation
		object IKeyColorizerElement.ColorItemKey {
			get { return Argument; }
		}
		#endregion
		#region IMapChartItem implementation
		double IMapChartItem.ValueSizeInPixels { 
			get { return ValueSizeInPixels; }
			set {
				int intValue = (int)value;
				if(valueSizeInPixels == intValue)
					return;
				valueSizeInPixels = intValue;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		#endregion
		#region ILocatableRenderItem implementation
		MapUnit ILocatableRenderItem.Location {
			get {
				return GetUnitLocation() * MapShape.RenderScaleFactor;
			}
		}
		Size ILocatableRenderItem.SizeInPixels {
			get {
				int px = ActualSizeInPixels;
				return new Size(px, px); 
			}
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
		#region IMapChartDataItem implementation
		double IMapChartDataItem.Value { get { return Value; } set { Value = value; } }
		object IMapChartDataItem.Argument { get { return Argument; } set { Argument = value; } }
		object IMapChartGroupDataItem.GroupKey { get { return Group; } set { Group = value; } }
		#endregion
		#region ITemplateGeometryItem Members
		TemplateGeometryType ITemplateGeometryItem.Type {
			get {
				return MapUtils.ToTemplateGeometryType(ActualMarkerType);
			}
		}
		double ITemplateGeometryItem.StretchFactor { get { return ActualSizeInPixels; } }
		#endregion
		MapUnit GetUnitLocation() {
			if(!this.unitLocation.HasValue)
				this.unitLocation = UnitConverter.CoordPointToMapUnit(location, true);
			return this.unitLocation.Value;
		}
		void OnMarketTypeChanged() {
			UpdateItem(MapItemUpdateType.Layout);
		}
		protected override MapRect CalculateBounds() {
			double halfSize = ActualSizeInPixels / 2.0;
			MapPoint location = UnitConverter.CoordPointToScreenPoint(this.location);
			MapUnit unit1 = UnitConverter.ScreenPointToMapUnit(new MapPoint(location.X - halfSize, location.Y - halfSize));
			MapUnit unit2 = UnitConverter.ScreenPointToMapUnit(new MapPoint(location.X + halfSize, location.Y + halfSize));
			return new MapRect(unit1.X, unit1.Y, unit2.X - unit1.X, unit2.Y - unit1.Y);
		}
		protected override CoordBounds CalculateNativeBounds() {
			return new CoordBounds(Location, Location);
		}
		protected override IMapItemGeometry CreateShapeGeometry() {
			UnitGeometry result = new UnitGeometry() { Bounds = base.Bounds, Points = Template.Points };
			return result;
		}
		protected override MapRect GetHitTestUnitBounds() {
			return MapRect.Empty;
		}
		protected internal override CoordPoint GetCenterCore() {
			return Location;
		}
		protected override IHitTestGeometry CreateHitTestGeometry() {
			MapUnit[] geometry = Template.Points;
			PointF[] points = new PointF[geometry.Length];
			PointF centerInPixel = UnitConverter.CoordPointToScreenPoint(Location).ToPointF();
			int actualSize = ActualSizeInPixels;
			double halfSize = actualSize / 2.0;
			for(int i = 0; i < geometry.Length; i++) {
				float x = Convert.ToSingle(geometry[i].X * actualSize + centerInPixel.X - halfSize);
				float y = Convert.ToSingle(geometry[i].Y * actualSize + centerInPixel.Y - halfSize);
				points[i] = new PointF(x, y);
			}
			return new PathScreenHitTestGeometry(points, new List<PointF[]>(), ActualStyle.StrokeWidth);
		}
		protected internal override ToolTipPatternParser CreateToolTipPatternParser(string pattern) {
			return new BubbleToolTipPatternParser(pattern, Argument, Value);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
		public override string ToString() {
			return "(MapBubble)";
		}
	}
}
