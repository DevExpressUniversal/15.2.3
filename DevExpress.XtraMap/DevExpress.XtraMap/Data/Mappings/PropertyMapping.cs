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
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using System.Drawing;
using System.Globalization;
using DevExpress.Utils;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Map;
using DevExpress.Utils.Editors;
namespace DevExpress.XtraMap {
	#region Color Converters
	public abstract class ColorConverter {
		public abstract Color ConvertToColor(object val);
		protected int GetColorAsInt32(object val) {
			return unchecked((int)(Convert.ToInt64(val, CultureInfo.InvariantCulture)));
		}
	}
	public class OleColorConverter : ColorConverter {
		public override Color ConvertToColor(object val) {
			int color = GetColorAsInt32(val);
			return DXColor.FromOle(color);
		}
		public override string ToString() {
			return "(OleColorConverter)";
		}
	}
	public class ArgbColorConverter : ColorConverter {
		public override Color ConvertToColor(object val) {
			int color = GetColorAsInt32(val);
			return DXColor.FromArgb(color);
		}
		public override string ToString() {
			return "(ArgbColorConverter)";
		}
	}
	public class NamedColorConverter : ColorConverter {
		public override Color ConvertToColor(object val) {
			string color = Convert.ToString(val);
			return DXColor.FromName(color);
		}
		public override string ToString() {
			return "(NamedColorConverter)";
		}
	}
	#endregion
	public abstract class MapItemMappingBaseCollection<T> : OwnedCollection<T> where T : IOwnedElement {
		protected MapItemMappingBaseCollection(LayerDataManager dataManager) : base(dataManager) {
		}
		protected override void OnCollectionChanged(CollectionChangedEventArgs<T> e) {
			base.OnCollectionChanged(e);
			LayerDataManager mgr = Owner as LayerDataManager;
			if(mgr != null) mgr.OnMappingsChanged();
		}
	}
	public class MapItemPropertyMappingCollection : MapItemMappingBaseCollection<MapItemPropertyMappingBase>, ISupportSwapItems {
		internal MapItemPropertyMappingCollection(LayerDataManager dataManager)
			: base(dataManager) {
		}
		#region ISupportSwapItems members
		void ISupportSwapItems.Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			MapItemPropertyMappingBase swapItem = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapItem;
		}
		#endregion
	}
	public abstract class MapItemPropertyMappingBase : MapItemMappingBase, IOwnedElement, ILayerDataManagerProvider {
		object owner;
		object defaultValue;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Name { get { return string.Empty; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemPropertyMappingBaseMember"),
#endif
		DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public new string Member {
			get { return base.Member; }
			set {
				if(base.Member== value)
					return;
				base.Member = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemPropertyMappingBaseDefaultValue"),
#endif
		DefaultValue(null), Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object DefaultValue {
			get { return defaultValue; }
			set {
				if(defaultValue == value)
					return;
				defaultValue = value;
				OnChanged();
			}
		}
	   #region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
			}
		}
		#endregion
		protected MapItemPropertyMappingBase() {
			this.defaultValue = GetDefaultValue();
		}
		protected virtual object GetDefaultValue() {
			return null;
		}
		protected virtual void OnChanged() {
			LayerDataManager dataManager = owner as LayerDataManager;
			if(dataManager != null) {
				dataManager.OnMappingsChanged();
			}
		}
		#region ILayerDataManagerProvider Members
		LayerDataManager ILayerDataManagerProvider.DataManager {
			get { return owner as LayerDataManager; }
		}
		#endregion
		protected int GetValueAsInt(object val) {
			return Convert.ToInt32(val);
		}
		protected double GetValueAsDouble(object val) {
			return Convert.ToDouble(val);
		}
		protected bool GetValueAsBool(object val) {
			return Convert.ToBoolean(val);
		}
		protected string GetValueAsString(object val) {
			return Convert.ToString(val);
		}
		protected double GetValueAsCoordinate(object val) {
			return MapUtils.ConvertToDouble(val);
		}
	}
	public abstract class MapItemPropertyMappingBase<T> : MapItemPropertyMappingBase where T : MapItem {
		protected override void SetDefaultValueInternal(IMapDataItem item, object obj) {
			T shape = item as T;
			if(shape != null && DefaultValue != null) 
				SetPropertyValue(shape, DefaultValue);
		}
		protected override void SetValueInternal(IMapDataItem item, object obj) {
			T shape = item as T;
			if(shape != null) 
				SetPropertyValue(shape, obj);
	   }
		protected abstract void SetPropertyValue(T item, object obj);
	}
	public abstract class MapShapePropertyMappingBase : MapItemPropertyMappingBase<MapShape> {
	}
	public abstract class MapDotPropertyMappingBase : MapItemPropertyMappingBase<MapDot> {
	}
	public abstract class MapEllipsePropertyMappingBase : MapItemPropertyMappingBase<MapEllipse> {
	}
	public abstract class MapLinePropertyMappingBase : MapItemPropertyMappingBase<MapLine> {
	}
	public abstract class MapPathPropertyMappingBase : MapItemPropertyMappingBase<MapPath> {
	}
	public abstract class MapPolygonPropertyMappingBase : MapItemPropertyMappingBase<MapPolygon> {
	}
	public abstract class MapPolylinePropertyMappingBase : MapItemPropertyMappingBase<MapPolyline> {
	}
	public abstract class MapRectanglePropertyMappingBase : MapItemPropertyMappingBase<MapRectangle> {
	}
	public abstract class MapCustomElementPropertyMappingBase : MapItemPropertyMappingBase<MapCustomElement> {
	}
	public abstract class MapCalloutPropertyMappingBase : MapItemPropertyMappingBase<MapCallout> {
	}
	public abstract class MapBubblePropertyMappingBase : MapItemPropertyMappingBase<MapBubble> {
	}
	public abstract class MapPiePropertyMappingBase : MapItemPropertyMappingBase<MapPie> {
	}
	public class MapItemVisiblePropertyMapping : MapItemPropertyMappingBase<MapItem> {
		[DefaultValue(false)]
		public new bool DefaultValue {
			get { return GetValueAsBool(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.Visible = GetValueAsBool(obj);
		}
		public override string ToString() {
			return "(MapItemVisiblePropertyMapping)";
		}
	}
	public abstract class MapItemColorMappingBase : MapItemPropertyMappingBase<MapItem> {
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemColorMappingBaseColorConverter"),
#endif
 DefaultValue(null),
		Editor("DevExpress.XtraMap.Design.ColorConverterPickerEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor)),
		]
		public ColorConverter ColorConverter { get; set; }
		public new Color DefaultValue {
			get { return GetValueAsColor(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		void ResetDefaultValue() {
			DefaultValue = Color.Empty;
		}
		bool ShouldSerializeDefaultValue() {
			return DefaultValue != Color.Empty;
		}
		protected virtual Color GetValueAsColor(object val) {
			if(ColorConverter != null)
				return ColorConverter.ConvertToColor(val);
			return val != null ? (Color)val : DXColor.Empty;
		}
	}
	public class MapItemFillMapping : MapItemColorMappingBase {
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.Fill = GetValueAsColor(obj);
		}
		public override string ToString() {
			return "(MapItemFillMapping)";
		}
	}
	public class MapItemStrokeMapping : MapItemColorMappingBase {
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.Stroke = GetValueAsColor(obj);
		}
		public override string ToString() {
			return "(MapItemStrokeMapping)";
		}
	}
	public class MapItemStrokeWidthMapping : MapItemPropertyMappingBase<MapItem> {
		[DefaultValue(MapItemStyle.EmptyStrokeWidth)]
		public new int DefaultValue {
			get { return GetValueAsInt(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapItemStyle.EmptyStrokeWidth;
		}
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.StrokeWidth = GetValueAsInt(obj);
		}
		public override string ToString() {
			return "(MapItemStrokeWidthMapping)";
		}
	}
	public class MapItemSelectedFillMapping : MapItemColorMappingBase {
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.SelectedFill = GetValueAsColor(obj);
		}
		public override string ToString() {
			return "(MapItemSelectedFillMapping)";
		}
	}
	public class MapItemSelectedStrokeMapping : MapItemColorMappingBase {
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.SelectedStroke = GetValueAsColor(obj);
		}
		public override string ToString() {
			return "(MapItemSelectedStrokeMapping)";
		}
	}
	public class MapItemSelectedStrokeWidthMapping : MapItemPropertyMappingBase<MapItem> {
		[DefaultValue(MapItemStyle.EmptyStrokeWidth)]
		public new int DefaultValue {
			get { return GetValueAsInt(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapItemStyle.EmptyStrokeWidth;
		}
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.SelectedStrokeWidth = GetValueAsInt(obj);
		}
		public override string ToString() {
			return "(MapItemSelectedStrokeWidthMapping)";
		}
	}
	public class MapItemHighlightedFillMapping : MapItemColorMappingBase {
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.HighlightedFill = GetValueAsColor(obj);
		}
		public override string ToString() {
			return "(MapItemHighlightedFillMapping)";
		}
	}
	public class MapItemHighlightedStrokeMapping : MapItemColorMappingBase {
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.HighlightedStroke = GetValueAsColor(obj);
		}
		public override string ToString() {
			return "(MapItemHighlightedStrokeMapping)";
		}
	}
	public class MapItemHighlightedStrokeWidthMapping : MapItemPropertyMappingBase<MapItem> {
		[DefaultValue(MapItemStyle.EmptyStrokeWidth)]
		public new int DefaultValue {
			get { return GetValueAsInt(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapItemStyle.EmptyStrokeWidth;
		}
		protected override void SetPropertyValue(MapItem item, object obj) {
			item.HighlightedStrokeWidth = GetValueAsInt(obj);
		}
		public override string ToString() {
			return "(MapItemHighlightedStrokeWidthMapping)";
		}
	}
	public class MapDotSizeMapping : MapDotPropertyMappingBase {
		[DefaultValue(MapDot.DefaultSize)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapDot.DefaultSize;
		}
		protected override void SetPropertyValue(MapDot item, object obj) {
			item.Size = GetValueAsDouble(obj);
		}
		public override string ToString() {
			return "(MapDotSizeMapping)";
		}
	}
	public class MapDotShapeKindMapping : MapDotPropertyMappingBase {
		[DefaultValue(MapDot.DefaultShapeKind)]
		public new MapDotShapeKind DefaultValue {
			get { return (MapDotShapeKind)GetValueAsInt(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapDot.DefaultShapeKind;
		}
		protected override void SetPropertyValue(MapDot item, object obj) {
			item.ShapeKind = (MapDotShapeKind)GetValueAsInt(obj);
		}
		public override string ToString() {
			return "(MapDotShapeKindMapping)";
		}
	}
	public class MapLinePoint1YMapping : MapLinePropertyMappingBase {
		[DefaultValue(MapLine.DefaultY1)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override void SetPropertyValue(MapLine item, object obj) {
			item.Point1 = CoordinateSystem.CreatePoint(item.Point1.GetX(), GetValueAsCoordinate(obj));
		}
		public override string ToString() {
			return "(MapLinePoint1YMapping)";
		}
	}
	public class MapLinePoint1XMapping : MapLinePropertyMappingBase {
		[DefaultValue(MapLine.DefaultX1)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override void SetPropertyValue(MapLine item, object obj) {
			item.Point1 = CoordinateSystem.CreatePoint(GetValueAsCoordinate(obj), item.Point1.GetY());
		}
		public override string ToString() {
			return "(MapLinePoint1XMapping)";
		}
	}
	public class MapLinePoint2YMapping : MapLinePropertyMappingBase {
		[DefaultValue(MapLine.DefaultY2)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapLine.DefaultY2;
		}
		protected override void SetPropertyValue(MapLine item, object obj) {
			item.Point2 = CoordinateSystem.CreatePoint(item.Point2.GetX(), GetValueAsCoordinate(obj));
		}
		public override string ToString() {
			return "(MapLinePoint2YMapping)";
		}
	}
	public class MapLinePoint2XMapping : MapLinePropertyMappingBase {
		[DefaultValue(MapLine.DefaultX2)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapLine.DefaultX2;
		}
		protected override void SetPropertyValue(MapLine item, object obj) {
			item.Point2 = CoordinateSystem.CreatePoint(GetValueAsCoordinate(obj), item.Point2.GetY());
		}
		public override string ToString() {
			return "(MapLinePoint2XMapping)";
		}
	}
	public class MapBubbleSizeMapping : MapBubblePropertyMappingBase {
		[DefaultValue(MapBubble.DefaultPixelSize)]
		public new int DefaultValue {
			get { return GetValueAsInt(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapBubble.DefaultPixelSize;
		}
		protected override void SetPropertyValue(MapBubble item, object obj) {
			item.Size = GetValueAsInt(obj);
		}
		public override string ToString() {
			return "(MapBubbleSizeMapping)";
		}
	}
	public class MapBubbleMarkerTypeMapping : MapBubblePropertyMappingBase {
		[DefaultValue(MarkerType.Default)]
		public new MarkerType DefaultValue {
			get { return (MarkerType)GetValueAsInt(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MarkerType.Default;
		}
		protected override void SetPropertyValue(MapBubble item, object obj) {
			item.MarkerType = (MarkerType)GetValueAsInt(obj);
		}
		public override string ToString() {
			return "(MapBubbleMarkerTypeMapping)";
		}
	}
	public class MapEllipseHeightMapping : MapEllipsePropertyMappingBase {
		[DefaultValue(MapEllipse.DefaultHeight)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapEllipse.DefaultHeight;
		}
		protected override void SetPropertyValue(MapEllipse item, object obj) {
			item.Height = GetValueAsDouble(obj);
		}
		public override string ToString() {
			return "(MapEllipseHeightMapping)";
		}
	}
	public class MapEllipseWidthMapping : MapEllipsePropertyMappingBase {
		[DefaultValue(MapEllipse.DefaultWidth)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapEllipse.DefaultWidth;
		}
		protected override void SetPropertyValue(MapEllipse item, object obj) {
			item.Width = GetValueAsDouble(obj);
		}
		public override string ToString() {
			return "(MapEllipseWidthMapping)";
		}
	}
	public class MapRectangleHeightMapping : MapRectanglePropertyMappingBase {
		[DefaultValue(MapRectangle.DefaultHeight)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapRectangle.DefaultHeight;
		}
		protected override void SetPropertyValue(MapRectangle item, object obj) {
			item.Height = GetValueAsDouble(obj);
		}
		public override string ToString() {
			return "(MapRectangleHeightMapping)";
		}
	}
	public class MapRectangleWidthMapping : MapRectanglePropertyMappingBase {
		[DefaultValue(MapRectangle.DefaultWidth)]
		public new double DefaultValue {
			get { return GetValueAsDouble(base.DefaultValue); }
			set { base.DefaultValue = value; }
		}
		protected override object GetDefaultValue() {
			return MapRectangle.DefaultWidth;
		}
		protected override void SetPropertyValue(MapRectangle item, object obj) {
			item.Width = GetValueAsDouble(obj);
		}
		public override string ToString() {
			return "(MapRectangleWidthMapping)";
		}
	}
	public abstract class MappingPointCoordinateProvider {
		public abstract int GetPointCount(object obj);
		public abstract double GetPointX(int pointIndex, object obj);
		public abstract double GetPointY(int pointIndex, object obj);
	}
	public abstract class MapItemPointCollectionMappingBase<T> : MapItemPropertyMappingBase<T> where T : MapItem {
		[DefaultValue(null)]
		public MappingPointCoordinateProvider PointCoordinateProvider { get; set; }
		protected override void SetPropertyValue(T item, object obj) {
			if(PointCoordinateProvider == null)
				return;
			int count = PointCoordinateProvider.GetPointCount(obj);
			for(int i = 0; i < count; i++) {
				double x = PointCoordinateProvider.GetPointX(i, obj);
				double y = PointCoordinateProvider.GetPointY(i, obj);
				if (CanAddPoint(x, y)) {
					CoordPoint pt = CoordinateSystem.CreatePoint(x, y);
					AddPoint(item, pt);
				}
			}
		}
		protected virtual CoordPoint CreatePoint(double x, double y) {
			return CoordinateSystem.CreatePoint(x, y);
		}
		protected virtual bool CanAddPoint(double x, double y) { 
			return CoordinateSystemHelper.IsNumericCoordinate(x) && CoordinateSystemHelper.IsNumericCoordinate(y);
		}
		protected virtual void AddPoint(T item, CoordPoint point) {
			IPointContainerCore container = item as IPointContainerCore;
			if (container != null) container.AddPoint(point);
		}
	}
}
