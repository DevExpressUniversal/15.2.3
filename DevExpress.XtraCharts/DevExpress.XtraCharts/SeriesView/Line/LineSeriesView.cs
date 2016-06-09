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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(LineSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class LineSeriesView : PointSeriesView, ILineSeriesView, IGeometryStripCreator, IGeometryHolder, ICustomTypeDescriptor {
		#region Nested class: LineSeriesViewPropertyDescriptorCollection
		class LineSeriesViewPropertyDescriptorCollection : PropertyDescriptorCollection {
			public LineSeriesViewPropertyDescriptorCollection(ICollection descriptors)
				: base(new PropertyDescriptor[] { }) {
				foreach (PropertyDescriptor pd in descriptors) {
					if (pd.DisplayName == "PointMarkerOptions")
						Add(new CustomPropertyDescriptor(pd, false));
					else
						Add(pd);
				}
			}
		}
		#endregion
		const int PointsCountForAntialiasing = 1000;
		const int DefaultMarkerSize = 10;
		const DefaultBoolean DefaultMarkerVisibility = DefaultBoolean.Default;
		const DefaultBoolean DefaultEnableAntialiasing = DefaultBoolean.Default;
		LineStyle lineStyle;
		DefaultBoolean markerVisibility = DefaultMarkerVisibility;
		DefaultBoolean enableAntialiasing = DefaultEnableAntialiasing;
		protected internal override bool ActualMarkerVisible {
			get {
				if (MarkerVisibility == DefaultBoolean.Default)
					if ((Chart != null && Chart.Container != null && (Chart.Container.ControlType == ChartContainerType.XRControl || Chart.Container.ControlType == ChartContainerType.WebControl)))
						return true;
					else
						return false;
				else
					return MarkerVisibility == DefaultBoolean.True;
			}
		}
		protected internal virtual int DefaultLineThickness { get { return 2; } }
		protected internal virtual bool DefaultLineAntialiasing { get { return true; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnLine); } }
		protected internal override bool ShouldCalculatePointsData {
			get {
				return ActualMarkerVisible ||
					(Series != null && Series.ActualLabelsVisibility) ||
					(ContainerAdapter != null && ContainerAdapter.ShouldCustomDrawSeriesPoints) ||
					(Series.Chart != null && Series.Chart.AnnotationRepository.HasSeriesPointAnchoredAnnotations());
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineSeriesViewLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineSeriesView.LineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle LineStyle { get { return lineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineSeriesViewLineMarkerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineSeriesView.LineMarkerOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker LineMarkerOptions { get { return (Marker)Marker; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new SimpleMarker PointMarkerOptions { get { return base.PointMarkerOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineSeriesViewMarkerVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineSeriesView.MarkerVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean MarkerVisibility {
			get { return markerVisibility; }
			set {
				if (value != markerVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					markerVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineSeriesViewEnableAntialiasing"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineSeriesView.EnableAntialiasing"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (value != enableAntialiasing) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAntialiasing = value;
					RaiseControlChanged();
				}
			}
		}
		public LineSeriesView()
			: base() {
			lineStyle = new LineStyle(this, DefaultLineThickness, DefaultLineAntialiasing);
		}
		#region ICustomTypeDescriptor implementation
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return new LineSeriesViewPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new LineSeriesViewPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true));
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
		#region ILineSeriesView implementation
		LineStyle ILineSeriesView.LineStyle { get { return LineStyle; } }
		bool ILineSeriesView.MarkerVisible { get { return ActualMarkerVisible; } }
		#endregion
		#region IGeometryStripCreator implementation
		IGeometryStrip IGeometryStripCreator.CreateStrip() {
			return CreateStripInternal();
		}
		#endregion
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		#region ShouldSerialize & Reset
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LineStyle":
					return ShouldSerializeLineStyle();
				case "LineMarkerOptions":
					return ShouldSerializeLineMarkerOptions();
				case "MarkerVisibility":
					return ShouldSerializeMarkerVisibility();
				case "EnableAntialiasing":
					return ShouldSerializeEnableAntialiasing();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		bool ShouldSerializeLineStyle() {
			return LineStyle.ShouldSerialize();
		}
		bool ShouldSerializeLineMarkerOptions() {
			return LineMarkerOptions.ShouldSerialize();
		}
		bool ShouldSerializeMarkerVisibility() {
			return markerVisibility != DefaultMarkerVisibility;
		}
		void ResetMarkerVisibility() {
			MarkerVisibility = DefaultMarkerVisibility;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return enableAntialiasing != DefaultEnableAntialiasing;
		}
		void ResetEnableAntialiasing() {
			EnableAntialiasing = DefaultEnableAntialiasing;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeLineStyle() ||
				ShouldSerializeLineMarkerOptions() ||
				ShouldSerializeMarkerVisibility() ||
				ShouldSerializeEnableAntialiasing();
		}
		#endregion
		protected virtual void AddStripElementInternal(IGeometryStrip strip, RefinedPoint pointInfo) {
			((LineStrip)strip).Add(new GRealPoint2D(pointInfo.Argument, ((IValuePoint)pointInfo).Value));
		}
		protected virtual IGeometryStrip CreateStripInternal() {
			return new LineStrip();
		}
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new LineGeometryStripCreator(false);
		}
		protected virtual LineWholeSeriesLayout CreateWholeSeriesLayout(SeriesLayout seriesLayout, List<IGeometryStrip> strips, int lineThickness, Rectangle bounds) {
			return new LineWholeSeriesLayout(seriesLayout, strips, lineThickness, bounds, ((XYDiagramSeriesLayout)seriesLayout).SingleLayout);
		}
		protected override MarkerBase CreateMarker() {
			return new Marker(this, DefaultMarkerSize);
		}
		protected override ChartElement CreateObjectForClone() {
			return new LineSeriesView();
		}
		protected override PointSeriesViewPainter CreatePainter() {
			return new LineSeriesViewPainter(this);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new LineDrawOptions(this);
		}
		protected int GetLineThickness(LineDrawOptions drawOptions, SelectionState selectionState) {
			return GraphicUtils.CorrectThicknessBySelectionState(drawOptions.LineStyle.Thickness, selectionState);
		}
		protected internal virtual bool GetActualAntialiasing(int pointsCount) {
			return DefaultBooleanUtils.ToBoolean(EnableAntialiasing, pointsCount < PointsCountForAntialiasing);
		}
		protected internal override WholeSeriesViewData CalculateWholeSeriesViewData(RefinedSeriesData seriesData, GeometryCalculator geometryCalculator) {
			IList<IGeometryStrip> strips = geometryCalculator.CreateStrips(seriesData.RefinedSeries);
			return new LineAndAreaWholeSeriesViewData(strips);
		}
		protected internal override WholeSeriesLayout CalculateWholeSeriesLayout(XYDiagramMappingBase diagramMapping, SeriesLayout seriesLayout) {
			LineAndAreaWholeSeriesViewData lineViewData = (LineAndAreaWholeSeriesViewData)seriesLayout.SeriesData.WholeViewData;
			List<IGeometryStrip> strips = StripsUtils.MapLineStrips(diagramMapping, lineViewData.Strips);
			return CreateWholeSeriesLayout(seriesLayout, strips, GetLineThickness((LineDrawOptions)seriesLayout.SeriesData.DrawOptions, seriesLayout.SeriesData.SelectionState), diagramMapping.InflatedBounds);
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxContinuousSeriesRangeValues(point, range, isHorizontalCrosshair, crosshairPaneInfo, snapMode);
		}
		internal override SeriesHitTestState CreateHitTestState() {
			return new SeriesHitTestState();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ILineSeriesView view = obj as ILineSeriesView;
			if (view != null) {
				lineStyle.Assign(view.LineStyle);
				lineStyle.SetAntialiasing(DefaultLineAntialiasing);
				LineSeriesView lineView = view as LineSeriesView;
				if (lineView != null) {
					markerVisibility = lineView.markerVisibility;
					enableAntialiasing = lineView.enableAntialiasing;
				}
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			LineSeriesView view = (LineSeriesView)obj;
			return lineStyle.Equals(view.lineStyle) && LineMarkerOptions.Equals(view.LineMarkerOptions);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class LineAndAreaWholeSeriesViewData : WholeSeriesViewData {
		readonly IList<IGeometryStrip> strips;
		public IList<IGeometryStrip> Strips { get { return strips; } }
		public LineAndAreaWholeSeriesViewData(IList<IGeometryStrip> strips) {
			this.strips = strips;
		}
	}
	public abstract class LineWholeSeriesLayoutBase : WholeSeriesLayout {
		int lineThickness;
		bool optimizeHitTesting;
		Rectangle bounds;
		protected abstract bool IsStripsNullOrEmpty { get; }
		protected Rectangle Bounds { get { return bounds; } }
		public int LineThickness { get { return lineThickness; } }
		public LineWholeSeriesLayoutBase(SeriesLayout seriesLayout, int lineThickness, Rectangle bounds, bool optimizeHitTesting)
			: base(seriesLayout) {
			this.lineThickness = lineThickness;
			this.bounds = bounds;
			this.optimizeHitTesting = optimizeHitTesting;
		}
		GraphicsPath CreateHitTestingGraphicsPath() {
			if (IsStripsNullOrEmpty)
				return null;
			GraphicsPath path = new GraphicsPath();
			try {
				FillHitTestingGraphicsPath(path);
				if (path.PointCount == 0)
					return null;
				GraphicsPath result = path;
				path = null;
				return result;
			}
			finally {
				if (path != null)
					path.Dispose();
			}
		}
		IHitRegion CalculateLineHitRegion() {
			GraphicsPath path = CreateHitTestingGraphicsPath();
			if (path == null)
				return new HitRegion();
			LineStyle lineStyle = null;
			if (SeriesLayout.DrawOptions is SwiftPlotDrawOptions)
				lineStyle = ((SwiftPlotDrawOptions)SeriesLayout.DrawOptions).LineStyle;
			else if (SeriesLayout.DrawOptions is LineDrawOptions)
				lineStyle = ((LineDrawOptions)SeriesLayout.DrawOptions).LineStyle;
			using (Pen pen = new Pen(Color.Empty, lineThickness)) {
				if (lineStyle != null)
					pen.LineJoin = lineStyle.LineJoin;
				path.Widen(pen);
			}
			return optimizeHitTesting ? new HitRegion(path) : new HitRegion(bounds, path);
		}
		protected abstract void FillHitTestingGraphicsPath(GraphicsPath path);
		public override HitRegionContainer CalculateHitRegion(DrawOptions drawOptions) {
			HitRegionContainer hitRegion = base.CalculateHitRegion(drawOptions);
			hitRegion.Union(CalculateLineHitRegion());
			return hitRegion;
		}
	}
	public class LineWholeSeriesLayout : LineWholeSeriesLayoutBase {
		List<IGeometryStrip> strips;
		protected override bool IsStripsNullOrEmpty { get { return strips == null || strips.Count == 0; } }
		public List<IGeometryStrip> Strips { get { return strips; } }
		public LineWholeSeriesLayout(SeriesLayout seriesLayout, List<IGeometryStrip> strips, int lineThickness, Rectangle bounds, bool optimizeHitTesting)
			: base(seriesLayout, lineThickness, bounds, optimizeHitTesting) {
			this.strips = strips;
		}
		protected virtual void AddStrips(GraphicsPath path) {
			foreach (LineStrip strip in strips) {
				LineStrip uniqueStrip = strip.CreateUniqueStrip();
				if (!uniqueStrip.IsEmpty) {
					List<LineStrip> splittedStrips = TruncatedLineCalculator.SplitLineStrip(uniqueStrip, Bounds);
					foreach (LineStrip splittedStrip in splittedStrips)
						using (GraphicsPath stripPath = new GraphicsPath()) {
							FillPath(splittedStrip, stripPath);
							path.AddPath(stripPath, false);
						}
				}
			}
		}
		protected virtual void FillPath(LineStrip uniqueStrip, GraphicsPath stripPath) {
			stripPath.AddLines(StripsUtils.Convert(uniqueStrip));
		}
		protected override void FillHitTestingGraphicsPath(GraphicsPath path) {
			AddStrips(path);
		}
	}
}
