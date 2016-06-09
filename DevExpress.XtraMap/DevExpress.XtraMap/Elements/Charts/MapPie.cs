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
using DevExpress.Utils.Editors;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public class PieSegment : MapSegmentBase, ILocatableRenderItem, IMapChartDataItem, IKeyColorizerElement, IInteractiveElementProvider, ISupportToolTip, IHitTestableElement, IMapItemAttributeOwner {
		const int VerticesPerRadian = 45;
		const double DefaultSegmentValue = 0.0;
		object segmentArgument;
		double segmentValue = DefaultSegmentValue;
		double angleStart = 0.0;
		double angleFinish = 0.0;
		MapUnit? unitLocation = null;
		int rowIndex = MapItem.UndefinedRowIndex;
		int[] listSourceRowIndices = new int[0];
		Color colorizerColor;
		readonly MapItemAttributeCollection attributes;
		protected int[] ListSourceRowIndices {
			get { return listSourceRowIndices; }
			set {
				int[] indices = value != null ? value : new int[0];
				listSourceRowIndices = indices;
			}
		}
		protected internal bool ShouldUseColorizerColor { get { return !MapUtils.IsColorEmpty(colorizerColor); } }
		protected internal override MapItemStyle DefaultStyle {
			get {
				IMapItemStyleProvider provider = MapPath != null ? MapPath.DefaultStyleProvider : MapItemStyleProvider.Default;
				return provider.PieSegmentStyle;
			}
		}
	   protected internal override MapShape MapPath {
			get { return Owner as MapPie; }
		}
		protected internal MapPie MapPie { get { return (MapPie)Owner; } }
		protected IMapChartItem ChartItem { get { return Owner as IMapChartItem; } }
		protected internal double AngleStart {
			get { return angleStart; }
		}
		protected internal double AngleFinish {
			get { return angleFinish; }
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("PieSegmentArgument"),
#endif
		DefaultValue(null), Category(SRCategoryNames.Data),
		Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Argument {
			get { return segmentArgument; }
			set {
				if(object.Equals(segmentArgument, value))
					return;
				segmentArgument = value;
				RaiseChanged();
			}
		}
		[Category(SRCategoryNames.Data), 
		DefaultValue(DefaultSegmentValue),
#if !SL
	DevExpressXtraMapLocalizedDescription("PieSegmentValue")
#else
	Description("")
#endif
]
		public double Value {
			get { return segmentValue; }
			set {
				if(segmentValue == value)
					return;
				segmentValue = value;
				RaiseChanged();
			}
		}
		[Category(SRCategoryNames.Data)]
		public MapItemAttributeCollection Attributes { get { return attributes; } }
		public PieSegment() {
			this.attributes = new MapItemAttributeCollection();
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color Stroke { get { return Color.Empty; } set { ; } }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new int StrokeWidth { get { return 0; } set { ; } }
		#region ILocatableRenderItem Members
		MapUnit ILocatableRenderItem.Location {
			get {
				if(!this.unitLocation.HasValue)
					this.unitLocation = UnitConverter.CoordPointToMapUnit(MapPie.Location, true) * MapShape.RenderScaleFactor;
				return this.unitLocation.Value;
			}
		}
		Size ILocatableRenderItem.SizeInPixels { 
			get {
				int px = MapPie.ActualSizeInPixels;
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
		#region IMapDataItem implementation
		int IMapDataItem.RowIndex { get { return this.rowIndex; } set { this.rowIndex = value; } }
		int[] IMapDataItem.ListSourceRowIndices { get { return ListSourceRowIndices; } set { ListSourceRowIndices = value; } }
		void IMapDataItem.AddAttribute(IMapItemAttribute attribute) {
			AddAttribute(MapItemAttribute.Create(attribute));
		}
		#endregion
		#region IMapChartDataItem implementation
		double IMapChartDataItem.Value { get { return Value; } set { Value = value; } }
		object IMapChartDataItem.Argument { get { return Argument; } set { Argument = value; } }
		#endregion
		#region IColorizerElement implementation
		Color IColorizerElement.ColorizerColor {
			get { return colorizerColor; }
			set {
				if(colorizerColor == value)
					return;
				colorizerColor = value;
				ColorizerColorChanged(colorizerColor);
			}
		}
		#endregion
		#region IKeyColorizerElement implementation
		object IKeyColorizerElement.ColorItemKey {
			get { return Argument; }
		}
		#endregion
		#region IToolTipSupportElement members
		MapItem ISupportToolTip.ActiveObject { get { return MapPie; } }
		string ISupportToolTip.CalculateToolTipText() {
			return MapPie != null ? MapPie.CalculateToolTipText() : string.Empty;
		}
		#endregion
		#region IHitTestableElement implementation
		IHitTestableOwner IHitTestableElement.Owner { get { return (IHitTestableOwner)MapPie; } }
		IEnumerable<IHitTestableElement> IHitTestableElement.HitTest(MapUnit unit, Point point) {
			return new IHitTestableElement[0];
		}
		MapRect IHitTestableElement.UnitBounds { get { return MapRect.Empty; } }
		RegionRange IHitTestableElement.RegionRange { get { return RegionRange.Empty; } set { ; } }
		bool IHitTestableElement.IsHitTestVisible { get { return false; } }
		GeometryType IHitTestableElement.GeometryType { get { return GeometryType.Screen; } }
		HitTestKey IHitTestableElement.Key { get { return new HitTestKey(); } set { ; } }
		object IHitTestableElement.Locker { get { return MapPie != null ? MapPie.UpdateLocker : null; } }
		#endregion
		IInteractiveElement IInteractiveElementProvider.GetInteractiveElement() {
			return MapPie;
		}
		IMapItemAttribute IMapItemAttributeOwner.GetAttribute(string name) {
			return Attributes[name];
		}
		protected override StyleMergerBase CreateStyleMerger() {
			return new PieSegmentStyleMerger(this);
		}
		protected void AddAttribute(MapItemAttribute attr) {
			if(attr == null) return;
			MapItemAttribute founded = Attributes[attr.Name];
			if(founded != null) Attributes.Remove(founded);
			Attributes.Add(attr);
		}
		protected internal override void ResetColorizerColor() {
			colorizerColor = Color.Empty;
		}
		void ColorizerColorChanged(Color color) {
			ResetStyle();
		}
		MapUnit[] CreateSectorContour() {
			double scaleFactor = 0.5 * MapPie.ActualSizeInPixels;
			List<MapUnit> sector = new List<MapUnit>();
			int iterationsCount = (int)Math.Ceiling((AngleFinish - AngleStart) / Math.PI * VerticesPerRadian);
			if(iterationsCount < 0)
				iterationsCount += 2 * VerticesPerRadian;
			for(int i = 0; i < iterationsCount; i++) {
				double phi = AngleStart + i * Math.PI / VerticesPerRadian;
				MapUnit p = new MapUnit(1 + Math.Cos(phi), 1 - Math.Sin(phi));
				sector.Add(p * scaleFactor);
			}
			MapUnit finishPoint = new MapUnit(1 + Math.Cos(AngleFinish), 1 - Math.Sin(AngleFinish));
			sector.Add(finishPoint * scaleFactor);
			if(Value != ChartItem.Value)
				sector.Add(new MapUnit(scaleFactor, scaleFactor));
			return sector.ToArray();
		}
		internal void UpdateAngles(double start, double finish) {
			angleStart = start;
			angleFinish = finish;
		}
		protected override IMapItemGeometry CreateGeometry() {
			return new UnitGeometry() { Bounds = MapPie.Bounds, Points = CreateSectorContour() };
		}
		protected override void MergeStyles() {
			StyleMerger.Merge(ActualStyle);
		}
		protected internal override void ResetStyle() {
			base.ResetStyle();
			ActualStyle.Stroke = Color.Transparent;
			ActualStyle.StrokeWidth = 0;
		}
		protected internal MapElementStyleBase GetDefaultStyle() {
			return MapPie.GetDefaultSegmentStyle();
		}
	}
	public class PieSegmentCollection : MapSegmentCollectionBase<PieSegment> {
		public PieSegmentCollection(object owner)
			: base(owner) {
		}
	}
	public enum RotationDirection {
		Clockwise,
		CounterClockwise
	}
	public class MapPie : MapPathBase<PieSegment>, IMapContainerDataItem, ISupportCoordLocation,  IMapChartItem,  IMapChartDataItem, ITemplateGeometryItem, ILocatableRenderItem, IPointCore {
		internal const int DefaultGroupIndex = -1;
		internal const int DefaultPixelSize = 20;
		readonly static TemplateGeometryBase Template = TemplateGeometryBase.CreateTemplate(TemplateGeometryType.Circle);
		public const double DefaultRotationAngle = 0.0;
		public const RotationDirection DefaultRotationDirection = RotationDirection.CounterClockwise;
		double rotationAngle = DefaultRotationAngle;
		RotationDirection rotationDirection = DefaultRotationDirection;
		CoordPoint location;
		int size = DefaultPixelSize;
		int valueSizeInPixels = DefaultPixelSize;
		List<double> segmentsValue = new List<double>();
		double total = 0;
		MapUnit? unitLocation = null;
		object argument = null;
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Pie; } }
#endif
		internal List<double> SegmentsValue { get { return segmentsValue; } }
		internal double Total { get { return total; } }
		protected internal int ValueSizeInPixels { get { return valueSizeInPixels; } }
		internal int ActualSizeInPixels { get { return ChartData != null ? ValueSizeInPixels : Size; } }
		protected VectorItemsLayer ItemsLayer { get { return (VectorItemsLayer)Layer; } }
		protected IMapChartDataAdapter ChartData { get { return Owner as IMapChartDataAdapter; } }
		protected override bool RenderItself { get { return true; } }
		[
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPieSegments"),
#endif
		DefaultValue(null)
		]
		public new PieSegmentCollection Segments {
			get { return (PieSegmentCollection)base.Segments; }
			set { base.Segments = value; }
		}
		[Category(SRCategoryNames.Layout), 
		DefaultValue(DefaultRotationAngle),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPieRotationAngle")
#else
	Description("")
#endif
]
		public double RotationAngle {
			get { return rotationAngle; }
			set {
				if(rotationAngle == value)
					return;
				rotationAngle = value;
				UpdateSegmentsAngles();
				UpdateItem(MapItemUpdateType.Style);
			}
		}
		[Category(SRCategoryNames.Layout), 
		DefaultValue(DefaultRotationDirection),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPieRotationDirection")
#else
	Description("")
#endif
]
		public RotationDirection RotationDirection {
			get { return rotationDirection; }
			set {
				if(rotationDirection == value)
					return;
				rotationDirection = value;
				UpdateSegmentsAngles();
				UpdateItem(MapItemUpdateType.Style);
			}
		}
		[Category(SRCategoryNames.Layout),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign), 
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPieLocation")
#else
	Description("")
#endif
]
		public CoordPoint Location {
			get { return location; }
			set {
				if(location == value)
					return;
				if(!CoordinateSystemHelper.IsNumericCoordPoint(value))
					Exceptions.ThrowIncorrectCoordPointException();
				location = value;
				unitLocation = null;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Location);
			}
		}
		void ResetLocation() { Location = UnitConverter.PointFactory.CreatePoint(0, 0); }
		bool ShouldSerializeLocation() { return Location != UnitConverter.PointFactory.CreatePoint(0, 0); }
		[Category(SRCategoryNames.Layout), 
		DefaultValue(DefaultPixelSize),
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPieSize")
#else
	Description("")
#endif
]
		public int Size {
			get { return size; }
			set {
				if(size == value)
					return;
				size = value < 0 ? DefaultPixelSize : value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[Browsable(false), DefaultValue(null)]
		public object Argument {
			get { return argument; }
			set {
				if(object.Equals(argument, value))
					return;
				argument = value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		public MapPie() {
			this.location = UnitConverter.PointFactory.CreatePoint(0, 0);
		}
		#region IMapChartItem implementation
		double IMapChartItem.ValueSizeInPixels { 
			get { return valueSizeInPixels; } 
			set {
				int intValue = (int)value;
				if (valueSizeInPixels == intValue)
					return;
				valueSizeInPixels = intValue;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		#endregion
		#region IMapChartDataItem implementation
		double IMapChartDataItem.Value { get { return Total; } set { ; } } 
		object IMapChartDataItem.Argument { get { return Argument; } set { Argument = value; } }
		#endregion
		#region IMapContatinerDataItem implementation
		IMapChartDataItem IMapContainerDataItem.CreateSegment() {
			return new PieSegment();
		}
		void IMapContainerDataItem.AddSegment(IMapChartDataItem child) {
			PieSegment segment = child as PieSegment;
			Segments.Add(segment);
		}
		int IMapDataItem.RowIndex {
			get { return RowIndex; }
			set { RowIndex = value; }
		}
		void IMapDataItem.AddAttribute(IMapItemAttribute attr) {
			AddAttribute(MapItemAttribute.Create(attr));
		}
		#endregion
		#region ITemplateGeometryItem Members
		TemplateGeometryType ITemplateGeometryItem.Type {
			get { return TemplateGeometryType.Circle; }
		}
		double ITemplateGeometryItem.StretchFactor {
			get { return ActualSizeInPixels; }
		}
		#endregion
		#region ILocatableRenderItem Members
		MapUnit ILocatableRenderItem.Location {
			get {
				if(!this.unitLocation.HasValue)
					this.unitLocation = UnitConverter.CoordPointToMapUnit(location, true) * MapShape.RenderScaleFactor;
				return this.unitLocation.Value;
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
		protected internal override void OnSegmentsChanged() {
			UpdateTotalValue();
			UpdatePercentage();
			UpdateSegmentsAngles();
			NotifyChartData();
			base.OnSegmentsChanged();
		}
		void NotifyChartData() {
			if(ChartData != null) ChartData.OnChartItemsUpdated();
		}
		double CalculateFinishAngle(double startAngle, double percent) {
			double angleIncrement = GetAngleIncrement(percent);
			startAngle += RotationDirection == RotationDirection.CounterClockwise ? angleIncrement : -angleIncrement;
			if(startAngle < 0)
				startAngle += Math.PI * 2.0;
			if(startAngle > Math.PI * 2.0)
				startAngle -= Math.PI * 2.0;
			return startAngle;
		}
		double GetAngleIncrement(double percent) {
			return percent * Math.PI * 2.0;
		}
		void UpdateSegmentsAngles() {
			double currentAngle = RotationAngle;
			for(int i = 0; i < Segments.Count; i++) {
				double segmentStartAngle = currentAngle;
				currentAngle = CalculateFinishAngle(currentAngle, SegmentsValue[i]);
				if(RotationDirection == XtraMap.RotationDirection.CounterClockwise)
					Segments[i].UpdateAngles(segmentStartAngle, currentAngle);
				else
					Segments[i].UpdateAngles(currentAngle, segmentStartAngle);
			}
		}
		internal void UpdatePercentage() {
			SegmentsValue.Clear();
			for(int i = 0; i < Segments.Count; i++)
				SegmentsValue.Add(Total == 0 ? 0 : Segments[i].Value / Total);
		}
		internal void UpdateTotalValue() {
			total = 0;
			for(int i = 0; i < Segments.Count; i++)
				total += Segments[i].Value;
		}
		protected override MapSegmentCollectionBase<PieSegment> CreateSegmentCollection() {
			return new PieSegmentCollection(this);
		}
		protected override MapRect CalculateBounds() {
			if (Layer == null || Layer.Map == null)
				return MapRect.Empty;
			double radius = ActualSizeInPixels / 2.0;
			MapPoint location = UnitConverter.CoordPointToScreenPoint(Location, true);
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
		protected override IEnumerable<IHitTestableElement> HitTestInnerItems(MapUnit unit, Point point) {
			MapUnit center = ((ILocatableRenderItem)this).Location;
			MapUnit delta = new MapUnit(unit.X - center.X, center.Y - unit.Y);
			if(delta.X == 0 && delta.Y == 0)
				return new IHitTestableElement[] { Segments[0] };
			double angle = Math.Atan2(delta.Y, delta.X) - RotationAngle;
			if(angle < 0)
				angle = Math.PI * 2.0 + angle;
			foreach(PieSegment segment in Segments)
				if(segment.AngleStart <= angle && segment.AngleFinish >= angle)
					return new IHitTestableElement[] { segment };
			return new IHitTestableElement[0];
		}
		protected internal override ToolTipPatternParser CreateToolTipPatternParser(string pattern) {
			IList<object> segmentArguments = new List<object>();
			IList<double> segmentValues = new List<double>();
			foreach(PieSegment segment in Segments) {
				string argument = segment.Argument != null ? segment.Argument.ToString() : string.Empty;
				segmentArguments.Add(argument);
				segmentValues.Add(segment.Value);
			}
			return new PieToolTipPatternParser(pattern, Argument, Total, segmentArguments, segmentValues);   
		}
		protected internal override CoordPoint GetCenterCore() {
			return Location;
		}
		public override string ToString() {
			return "(MapPie)";
		}
		protected internal override MapElementStyleBase GetDefaultItemStyle() {
			return GetDefaultSegmentStyle();
		}
		protected internal MapElementStyleBase GetDefaultSegmentStyle() {
			return DefaultStyleProvider.PieSegmentStyle;
		}
		protected override IRenderItemStyle GetStyle() {
			IRenderItemStyle style =  base.GetStyle();
			return new MapItemStyle() { Fill = Color.Transparent, Stroke = style.Stroke, StrokeWidth = style.StrokeWidth };
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
	}
}
