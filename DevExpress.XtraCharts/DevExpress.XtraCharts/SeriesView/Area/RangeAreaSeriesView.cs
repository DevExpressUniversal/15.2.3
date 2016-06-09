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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RangeAreaSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeAreaSeriesView : AreaSeriesView {
		const int defaultMarkerSize = 10;
		const int valueCount = 2;
		const DefaultBoolean DefaultMarker1Visibility = DefaultBoolean.Default;
		const DefaultBoolean DefaultMarker2Visibility = DefaultBoolean.Default;
		Marker marker1;
		Marker marker2;
		CustomBorder border1;
		CustomBorder border2;
		DefaultBoolean marker1Visibility = DefaultMarker1Visibility;
		DefaultBoolean marker2Visibility = DefaultMarker2Visibility;
		internal bool ActualMarker1Visible {
			get {
				if (Marker1Visibility == DefaultBoolean.Default)
					return false;
				else
					return Marker1Visibility == DefaultBoolean.True;
			}
		}
		internal bool ActualMarker2Visible {
			get {
				if (Marker2Visibility == DefaultBoolean.Default)
					return false;
				else
					return Marker2Visibility == DefaultBoolean.True;
			}
		}
		protected override Marker ActualMarkerOptions { get { return null; } }
		protected override CustomBorder ActualBorder { get { return null; } }
		protected internal override int PointDimension { get { return valueCount; } }
		protected internal override ValueLevel[] SupportedValueLevels {
			get { return new ValueLevel[] { ValueLevel.Value_1, ValueLevel.Value_2 }; }
		}
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnRangeArea); } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = " : " + "{V1" + GetDefaultFormat(Series.ValueScaleType) + "} : {V2" + GetDefaultFormat(Series.ValueScaleType) + "}";
				return argumentPattern + valuePattern;
			}
		}
		protected internal override bool ActualMarkerVisible {
			get {
				if (Marker1Visibility == DefaultBoolean.Default && Marker2Visibility == DefaultBoolean.Default)
					if ((Chart != null && Chart.Container != null && (Chart.Container.ControlType == ChartContainerType.XRControl || Chart.Container.ControlType == ChartContainerType.WebControl)))
						return true;
					else
						return false;
				else
					return Marker1Visibility == DefaultBoolean.True || Marker2Visibility == DefaultBoolean.True;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesViewMarker1"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesView.Marker1"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker Marker1 { get { return this.marker1; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesViewMarker2"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesView.Marker2"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker Marker2 { get { return this.marker2; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Marker MarkerOptions { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesViewBorder1"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesView.Border1"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CustomBorder Border1 { get { return border1; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesViewBorder2"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesView.Border2"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CustomBorder Border2 { get { return border2; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new CustomBorder Border { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesViewMarker1Visibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesView.Marker1Visibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean Marker1Visibility {
			get { return marker1Visibility; }
			set {
				if (value != marker1Visibility) {
					SendNotification(new ElementWillChangeNotification(this));
					marker1Visibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeAreaSeriesViewMarker2Visibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeAreaSeriesView.Marker2Visibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean Marker2Visibility {
			get { return marker2Visibility; }
			set {
				if (value != marker2Visibility) {
					SendNotification(new ElementWillChangeNotification(this));
					marker2Visibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new DefaultBoolean MarkerVisibility { get { return DefaultBoolean.Default; } }
		public RangeAreaSeriesView() : base() {
			marker1 = new Marker(this, defaultMarkerSize);
			marker2 = new Marker(this, defaultMarkerSize);
			border1 = new CustomBorder(this, true, Color.Empty);
			border2 = new CustomBorder(this, true, Color.Empty);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Marker1":
					return ShouldSerializeMarker1();
				case "Marker2":
					return ShouldSerializeMarker2();
				case "Border1":
					return ShouldSerializeBorder1();
				case "Border2":
					return ShouldSerializeBorder2();
				case "Marker1Visibility":
					return ShouldSerializeMarker1Visibility();
				case "Marker2Visibility":
					return ShouldSerializeMarker2Visibility();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeMarker1() {
			return Marker1.ShouldSerialize();
		}
		bool ShouldSerializeMarker2() {
			return Marker2.ShouldSerialize();
		}
		bool ShouldSerializeBorder1() {
			return Border1.ShouldSerialize();
		}
		bool ShouldSerializeBorder2() {
			return Border2.ShouldSerialize();
		}
		bool ShouldSerializeMarker1Visibility() {
			return marker1Visibility != DefaultMarker1Visibility;
		}
		void ResetMarker1Visibility() {
			Marker1Visibility = DefaultMarker1Visibility;
		}
		bool ShouldSerializeMarker2Visibility() {
			return marker2Visibility != DefaultMarker2Visibility;
		}
		void ResetMarker2Visibility() {
			Marker2Visibility = DefaultMarker2Visibility;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeMarker1() ||
				ShouldSerializeMarker2() ||
				ShouldSerializeBorder1() ||
				ShouldSerializeBorder2() ||
				ShouldSerializeMarker1Visibility() ||
				ShouldSerializeMarker2Visibility();
		}
		#endregion
		internal override string LabelPatternToLegendPattern() {
			if (SeriesBase != null && Label != null)
				return PatternUtils.ReplacePlaceholder(Label.ActualTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder, PatternUtils.Value2Placeholder);
			else
				return base.LabelPatternToLegendPattern();
		}
		protected override Type PointInterfaceType {
			get {
				return typeof(IRangePoint);
			}
		}
		protected override SeriesContainer CreateContainer() {
			return new RangeSeriesContainer(this);
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			IRangePoint rangeAreaPoint = refinedPoint;
			yield return rangeAreaPoint.Min;
			yield return rangeAreaPoint.Max;
		}
		protected internal override MinMaxValues GetSeriesPointValues(RefinedPoint pointInfo) {
			IRangePoint rangeBarPoint = (IRangePoint)pointInfo;
			double value1 = rangeBarPoint.Value1;
			double value2 = rangeBarPoint.Value2;
			return new MinMaxValues(value1, value2);
		}		
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			RefinedPoint pointInfo = pointData.RefinedPoint;
			MinMaxValues values = GetSeriesPointValues(pointInfo);
			DiagramPoint value1Point = diagramMapping.GetScreenPointNoRound(pointInfo.Argument, values.Min);
			DiagramPoint value2Point = diagramMapping.GetScreenPointNoRound(pointInfo.Argument, values.Max);
			RangeAreaDrawOptions rangeAreaDrawOptions = (RangeAreaDrawOptions)pointData.DrawOptions;
			IPolygon value1Polygon = rangeAreaDrawOptions.Marker1Visible ? rangeAreaDrawOptions.Marker1.CalculatePolygon(new GRealPoint2D(Math.Round(value1Point.X), Math.Round(value1Point.Y)), false) : null;
			IPolygon value2Polygon = rangeAreaDrawOptions.Marker2Visible ? rangeAreaDrawOptions.Marker2.CalculatePolygon(new GRealPoint2D(Math.Round(value2Point.X), Math.Round(value2Point.Y)), false) : null;
			return new RangeAreaSeriesPointLayout(pointData, value2Point, value2Polygon, value1Point, value1Polygon);
		}
		protected internal override HighlightedPointLayout CalculateHighlightedPointLayout(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint, ISeriesView seriesView, DrawOptions drawOptions) {
			MinMaxValues values = GetSeriesPointValues(refinedPoint);
			DiagramPoint point1 = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, values.Min);
			DiagramPoint point2 = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, values.Max);
			int polygonSize = CalculateCrosshairPolygonSize(diagramMapping, refinedPoint);
			IPolygon polygon = null;
			if (diagramMapping.Bounds.Contains((int)point1.X, (int)point1.Y))
				polygon = Marker1.CalculatePolygon(new GRealPoint2D(Math.Round(point1.X), Math.Round(point1.Y)), true);
			IPolygon polygon2 = null;
			if (diagramMapping.Bounds.Contains((int)point2.X, (int)point2.Y))
				polygon2 = Marker2.CalculatePolygon(new GRealPoint2D(Math.Round(point2.X), Math.Round(point2.Y)), true);
			PointDrawOptionsBase pointDrawOptions = drawOptions as PointDrawOptionsBase;
			if (pointDrawOptions != null) {
				System.Drawing.Color color = GetMarkerSelfColor(pointDrawOptions);
				System.Drawing.Color borderColor = GetBorderDrawColor(pointDrawOptions);
				return new HighlightedRangePointLayout(polygon, polygon2, color, borderColor);
			}
			return null;
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new RangeAreaDrawOptions(this);
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipRangeValueToStringConverter(Series);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.RangeViewPointPatterns;
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			return AnnotationHelper.CalculateAchorPointForCenterAreaPointWithoutScrolling(mapping.Container, pointData.RefinedPoint.Argument, GetSeriesPointValues(pointData.RefinedPoint));
		}
		protected override PointSeriesViewPainter CreatePainter() {
			return new RangeAreaSeriesViewPainter(this);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new RangeAreaSeriesLabel();
		}
		protected internal override PointOptions CreatePointOptions() {
			return new RangeAreaPointOptions();
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new RangeAreaGeometryStripCreator();
		}
		protected internal override void RenderHighlightedPoint(IRenderer renderer, HighlightedPointLayout pointLayout) {
			HighlightedRangePointLayout rangeAreaPointLayout = pointLayout as HighlightedRangePointLayout;
			if (rangeAreaPointLayout != null) {
				if (rangeAreaPointLayout.Polygon != null) {
					renderer.EnablePolygonAntialiasing(true);
					rangeAreaPointLayout.Polygon.Render(renderer, new SolidFillOptions(), rangeAreaPointLayout.Color, rangeAreaPointLayout.Color, rangeAreaPointLayout.BorderColor, 1);
					renderer.RestorePolygonAntialiasing();
				}
				if (rangeAreaPointLayout.Polygon2 != null) {
					renderer.EnablePolygonAntialiasing(true);
					rangeAreaPointLayout.Polygon2.Render(renderer, new SolidFillOptions(), rangeAreaPointLayout.Color, rangeAreaPointLayout.Color, rangeAreaPointLayout.BorderColor, 1);
					renderer.RestorePolygonAntialiasing();
				}
			}
		}
		protected override void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			AnnotationHelper.CalculateAchorPointLayoutForCenterAreaPoint(annotation, this, anchorPointLayoutList, pointData);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RangeAreaSeriesView();
		}
		public override string GetValueCaption(int index) {
			if (index >= PointDimension)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember) + " " + (index + 1).ToString();
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			RangeAreaSeriesView view = (RangeAreaSeriesView)obj;
			return
				marker2.Equals(view.marker2) &&
				marker1.Equals(view.marker1) &&
				border1.Equals(view.border1) &&
				border2.Equals(view.border2);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RangeAreaSeriesView view = obj as RangeAreaSeriesView;
			if(view == null)
				return;
			this.marker2.Assign(view.marker2);
			this.marker1.Assign(view.marker1);
			border1.Assign(view.border1);
			border2.Assign(view.border2);
			marker1Visibility = view.marker1Visibility;
			marker2Visibility = view.marker2Visibility;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class RangeAreaSeriesPointLayout : AreaSeriesPointLayout {
		IPolygon zeroPolygon;
		public IPolygon ZeroPolygon { get { return zeroPolygon; } }
		public RangeAreaSeriesPointLayout(RefinedPointData pointData, DiagramPoint actualPoint, IPolygon polygon, DiagramPoint zeroPoint, IPolygon zeroPolygon) : base(pointData, actualPoint, polygon, zeroPoint) {
			this.zeroPolygon = zeroPolygon;
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer hitRegion = base.CalculateHitRegion();
			IHitRegion zeroPolygonRegion = GraphicUtils.MakeHitRegion(this.zeroPolygon);
			hitRegion.Union(zeroPolygonRegion);
			return hitRegion;
		}
	}
}
