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
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum PaneLayoutDirection {
		Vertical,
		Horizontal
	}
	public interface IXYDiagram2D {
		Axis2D AxisX { get; }
		Axis2D AxisY { get; }
		SecondaryAxisCollection SecondaryAxesX { get; }
		SecondaryAxisCollection SecondaryAxesY { get; }
	}
	public abstract partial class XYDiagram2D : Diagram, IIndicatorCalculator, IXYDiagram2D, IXtraSupportDeserializeCollectionItem, IScrollingZoomingOptions, ISupportRangeControl {
		const PaneLayoutDirection DefaultPaneLayoutDirection = PaneLayoutDirection.Vertical;
		const int DefaultPaneDistance = 10;
		const int DefaultAxisXLength = 500;
		const int DefaultAxisYLength = 500;
		const int MaxRangeSnapIterationsCount = 5;
		const double ZoomPercent = 3.0;
		const bool DefaultEnableAxisXScrolling = false;
		const bool DefaultEnableAxisYScrolling = false;
		const bool DefaultEnableAxisXZooming = false;
		const bool DefaultEnableAxisYZooming = false;
		internal static GRealSize2D MinimumSize = new GRealSize2D(200, 150);
		readonly RectangleIndents margins;
		readonly XYDiagramDefaultPane defaultPane;
		readonly XYDiagramPaneCollection panes;
		readonly ScrollingOptions2D scrollingOptions;
		readonly ZoomingOptions2D zoomingOptions;
		readonly RaggedEdgeGeometry raggedGeometry = new RaggedEdgeGeometry();
		readonly WavedEdgeGeometry wavedGeometry = new WavedEdgeGeometry();
		readonly TextMeasurer textMeasurer = new TextMeasurer();
		readonly ChartRangeControlClientDateTimeGridOptions rangeControlDateTimeGridOptions;
		readonly ChartRangeControlClientNumericGridOptions rangeControlNumericGridOptions;
		readonly ChartRangeControlClientQualitativeGridOptions rangeControlQualitativeGridOptions;
		readonly ChartRangeControlClientNumericOptions rangeControlNumericOptions;
		readonly ChartRangeControlClientDateTimeOptions rangeControlDateTimeOptions;
		readonly IndicatorsRepository indicatorsRepository = new IndicatorsRepository();
		bool enableAxisXScrolling = DefaultEnableAxisXScrolling;
		bool enableAxisYScrolling = DefaultEnableAxisYScrolling;
		bool enableAxisXZooming = DefaultEnableAxisXZooming;
		bool enableAxisYZooming = DefaultEnableAxisYZooming;
		PaneLayoutDirection paneLayoutDirection = DefaultPaneLayoutDirection;
		int paneDistance = DefaultPaneDistance;
		XYDiagramPaneBase focusedPane;
		PaneAxesContainerRepository paneAxesContainerRepository;
		AxisPaneContainer axisPaneRepository;
		ZoomCacheEx zoomCacheEx;
		Point gestureZoomCenter;
		double gestureZoomPercent;
		double gestureStartAxisXMin;
		double gestureStartAxisXMax;
		double gestureStartAxisYMin;
		double gestureStartAxisYMax;
		Point crosshairLocation = Point.Empty;
		CrosshairManager crosshairManager;
		bool isEmpty = false;
		XYDiagramPaneBase ActualFocusedPane { get { return focusedPane == null ? defaultPane : focusedPane; } }
		bool ShowRangeControlGridOptions { get { return Chart != null && Chart.IsRangeControlClient; } }
		internal bool HasSmartAxisX { get { return SmartAxesUtils.HasSmartAxis(this); } }
		internal bool ShowRangeControlDateTimeGridOptions { get { return ShowRangeControlGridOptions && ActualAxisX.ScaleType == ActualScaleType.DateTime; } }
		internal bool ShowRangeControlNumericGridOptions { get { return ShowRangeControlGridOptions && ActualAxisX.ScaleType == ActualScaleType.Numerical; } }
		internal bool IsEmpty { get { return isEmpty; } }
		internal CrosshairManager CrosshairManager { get { return crosshairManager; } }
		internal AxisPaneContainer AxisPaneRepository { get { return axisPaneRepository; } }
		internal ZoomCacheEx ZoomCacheEx { get { return zoomCacheEx; } }
		internal XYDiagramAppearance Appearance { get { return CommonUtils.GetActualAppearance(this).XYDiagramAppearance; } }
		internal IList<IPane> ActualPanes {
			get {
				List<IPane> actualPanes = new List<IPane>();
				if (defaultPane.Visible)
					actualPanes.Add(defaultPane);
				foreach (XYDiagramPane pane in panes)
					if (pane.Visible)
						actualPanes.Add(pane);
				return actualPanes;
			}
		}
		internal RaggedEdgeGeometry RaggedGeometry { get { return raggedGeometry; } }
		internal WavedEdgeGeometry WavedGeometry { get { return wavedGeometry; } }
		protected internal override bool SupportTooltips { get { return true; } }
		protected internal override bool DependsOnBounds { get { return IsScrollingEnabled || HasSmartAxisX; } }
		protected internal override bool CanZoomIn {
			get {
				PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
				return paneAxesData != null && paneAxesData.CanZoomIn();
			}
		}
		protected internal override bool CanZoomOut {
			get {
				PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
				return paneAxesData != null && paneAxesData.CanZoomOut;
			}
		}
		protected internal override bool CanZoomWithTouch { get { return ZoomingOptions.UseTouchDevice; } }
		protected internal override bool CanScroll { get { return IsScrollingEnabled; } }
		protected internal override bool CanPan { get { return ScrollingOptions.UseTouchDevice; } }
		protected internal override bool HasAutoLayoutElements { get { return HasAutoLayoutAxes(GetAllAxesX()) || HasAutoLayoutAxes(GetAllAxesY()); } }
		protected internal override GRealSize2D MinimumLayoutSize { get { return MinimumSize; } }
		protected internal abstract Axis2D ActualAxisX { get; }
		protected internal abstract Axis2D ActualAxisY { get; }
		protected internal abstract SecondaryAxisCollection ActualSecondaryAxesX { get; }
		protected internal abstract SecondaryAxisCollection ActualSecondaryAxesY { get; }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsScrollingEnabled {
			get {
				if (defaultPane.ActualEnableAxisXScrolling || defaultPane.ActualEnableAxisYScrolling)
					return true;
				foreach (XYDiagramPaneBase pane in Panes)
					if (pane.ActualEnableAxisXScrolling || pane.ActualEnableAxisYScrolling)
						return true;
				return false;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsZoomingEnabled {
			get {
				if (defaultPane.ActualEnableAxisXZooming || defaultPane.ActualEnableAxisYZooming)
					return true;
				foreach (XYDiagramPaneBase pane in Panes)
					if (pane.ActualEnableAxisXZooming || pane.ActualEnableAxisYZooming)
						return true;
				return false;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DMargins"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.Margins"),
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents Margins { get { return margins; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DDefaultPane"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.DefaultPane"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public XYDiagramDefaultPane DefaultPane { get { return defaultPane; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DPanes"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.Panes"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.XYDiagramPaneCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public XYDiagramPaneCollection Panes { get { return panes; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DScrollingOptions"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ScrollingOptions2D ScrollingOptions { get { return scrollingOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DZoomingOptions"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ZoomingOptions2D ZoomingOptions { get { return zoomingOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DEnableAxisXScrolling"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool EnableAxisXScrolling {
			get { return enableAxisXScrolling; }
			set {
				if (value != enableAxisXScrolling) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisXScrolling = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DEnableAxisYScrolling"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool EnableAxisYScrolling {
			get { return enableAxisYScrolling; }
			set {
				if (value != enableAxisYScrolling) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisYScrolling = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DEnableAxisXZooming"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool EnableAxisXZooming {
			get { return enableAxisXZooming; }
			set {
				if (value != enableAxisXZooming) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisXZooming = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DEnableAxisYZooming"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool EnableAxisYZooming {
			get { return enableAxisYZooming; }
			set {
				if (value != enableAxisYZooming) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisYZooming = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DPaneLayoutDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.PaneLayoutDirection"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public PaneLayoutDirection PaneLayoutDirection {
			get { return paneLayoutDirection; }
			set {
				if (value != paneLayoutDirection) {
					SendNotification(new ElementWillChangeNotification(this));
					paneLayoutDirection = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DPaneDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.PaneDistance"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int PaneDistance {
			get { return paneDistance; }
			set {
				if (value != paneDistance) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPaneDistance));
					SendNotification(new ElementWillChangeNotification(this));
					paneDistance = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.RangeControlDateTimeGridOptions"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DRangeControlDateTimeGridOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ChartRangeControlClientDateTimeGridOptions RangeControlDateTimeGridOptions { get { return rangeControlDateTimeGridOptions; } }
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.RangeControlNumericGridOptions"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DRangeControlNumericGridOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ChartRangeControlClientNumericGridOptions RangeControlNumericGridOptions { get { return rangeControlNumericGridOptions; } }
		protected XYDiagram2D() : base() {
			margins = new RectangleIndents(this, 5);
			defaultPane = new XYDiagramDefaultPane(this);
			panes = new XYDiagramPaneCollection(this);
			scrollingOptions = new ScrollingOptions2D(this);
			zoomingOptions = new ZoomingOptions2D(this);
			zoomCacheEx = new ZoomCacheEx();
			crosshairManager = new CrosshairManager(this);
			rangeControlDateTimeGridOptions = new ChartRangeControlClientDateTimeGridOptions(this);
			rangeControlNumericGridOptions = new ChartRangeControlClientNumericGridOptions(this);
			rangeControlQualitativeGridOptions = new ChartRangeControlClientQualitativeGridOptions(this);
			rangeControlNumericOptions = new ChartRangeControlClientNumericOptions(rangeControlNumericGridOptions);
			rangeControlDateTimeOptions = new ChartRangeControlClientDateTimeOptions(rangeControlDateTimeGridOptions);
		}
		#region IXYDiagram
		IList<IPane> IXYDiagram.Panes { get { return ActualPanes; } }
		IAxisData IXYDiagram.AxisX { get { return ActualAxisX; } }
		IAxisData IXYDiagram.AxisY { get { return ActualAxisY; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesX { get { return ActualSecondaryAxesX; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesY { get { return ActualSecondaryAxesY; } }
		bool IXYDiagram.ScrollingEnabled { get { return IsScrollingEnabled; } }
		bool IXYDiagram.Rotated { get { return ActualRotated; } }
		ICrosshairOptions IXYDiagram.CrosshairOptions { get { return Chart.ActualCrosshairEnabled ? Chart.CrosshairOptions : null; } }
		IList<IPane> IXYDiagram.GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync) {
			return GetCrosshairSyncPanes(focusedPane, isHorizontalSync);
		}
		InternalCoordinates IXYDiagram.MapPointToInternal(IPane pane, GRealPoint2D point) {
			return MapPointToInternal((XYDiagramPaneBase)pane, new Point((int)point.X, (int)point.Y));
		}
		GRealPoint2D IXYDiagram.MapInternalToPoint(IPane pane, IAxisData axisX, IAxisData axisY, double argument, double value) {
			ControlCoordinates coords = MapInternalToPoint((XYDiagramPaneBase)pane, axisX, axisY, argument, value);
			return new GRealPoint2D(coords.Point.X, coords.Point.Y);
		}
		List<IPaneAxesContainer> IXYDiagram.GetPaneAxesContainers(IList<RefinedSeries> activeSeries) {
			paneAxesContainerRepository = new PaneAxesContainerRepository(ActualAxisX, ActualAxisY, ActualSecondaryAxesX, ActualSecondaryAxesY, ActualPanes, activeSeries);
			axisPaneRepository = new AxisPaneContainer(paneAxesContainerRepository);
			List<IPaneAxesContainer> containers = new List<IPaneAxesContainer>();
			foreach (IPane pane in ActualPanes) {
				IPaneAxesContainer container = GetPaneAxesData(pane);
				if (container != null)
					containers.Add(container);
			}
			return containers;
		}
		void IXYDiagram.UpdateCrosshairData(IList<RefinedSeries> seriesCollection) {
			crosshairManager.UpdateCrosshairData(seriesCollection);
		}
		void IXYDiagram.UpdateAutoMeasureUnits() {
			UpdateAutomaticMeasurement(true);
		}
		int IXYDiagram.GetAxisXLength(IAxisData axis) {
			return GetAxisLength(axis);
		}
		#endregion
		#region IWizardXYDiagramPropertiesProvider implementation
		Axis2D IXYDiagram2D.AxisX { get { return ActualAxisX; } }
		Axis2D IXYDiagram2D.AxisY { get { return ActualAxisY; } }
		SecondaryAxisCollection IXYDiagram2D.SecondaryAxesX { get { return ActualSecondaryAxesX; } }
		SecondaryAxisCollection IXYDiagram2D.SecondaryAxesY { get { return ActualSecondaryAxesY; } }
		#endregion
		#region IScrollingZoomingOptions
		bool IScrollingZoomingOptions.UseKeyboardScrolling { get { return scrollingOptions.UseKeyboard; } }
		bool IScrollingZoomingOptions.UseKeyboardZooming { get { return zoomingOptions.UseKeyboard; } }
		bool IScrollingZoomingOptions.UseKeyboardWithMouseZooming { get { return zoomingOptions.UseKeyboardWithMouse; } }
		bool IScrollingZoomingOptions.UseMouseScrolling { get { return scrollingOptions.UseMouse; } }
		bool IScrollingZoomingOptions.UseMouseWheelZooming { get { return zoomingOptions.UseMouseWheel; } }
		bool IScrollingZoomingOptions.UseScrollBarsScrolling { get { return scrollingOptions.UseScrollBars; } }
		bool IScrollingZoomingOptions.UseTouchDevicePanning { get { return scrollingOptions.UseTouchDevice; } }
		bool IScrollingZoomingOptions.UseTouchDeviceZooming { get { return zoomingOptions.UseTouchDevice; } }
		bool IScrollingZoomingOptions.UseTouchDeviceRotation { get { return false; } }
		#endregion
		#region IXtraSupportDeserializeCollectionItem
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			XtraSetIndexCollectionItem(propertyName, e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return XtraCreateCollectionItem(propertyName);
		}
		#endregion
		#region ISupportRangeControl
		RangeControlMapping rangeControlMap;
		List<object> rangeControlRuler = new List<object>();
		ChartRangeControlClientGridOptions ActualRangeControlClientGridOptions {
			get {
				switch (ActualAxisX.ScaleType) {
					case ActualScaleType.DateTime:
						return rangeControlDateTimeGridOptions;
					case ActualScaleType.Numerical:
						return rangeControlNumericGridOptions;
					case ActualScaleType.Qualitative:
					default:
						return rangeControlQualitativeGridOptions;
				}
			}
		}
		RangeControlMapping GetRangeControlMap() {
			if (rangeControlMap == null)
				rangeControlMap = RangeControlMapping.Create(ActualAxisX.WholeRange);
			return rangeControlMap;
		}
		bool ISupportRangeControl.IsValid {
			get {
				return this.Chart.ViewController.ActiveSeriesCount > 0;
			}
		}
		string ISupportRangeControl.InvalidText {
			get {
				return "";
			}
		}
		int ISupportRangeControl.TopIndent {
			get {
				return 0;
			}
		}
		int ISupportRangeControl.BottomIndent {
			get {
				return 0;
			}
		}
		void ISupportRangeControl.OnRangeControlChanged(object sender) {
			((ISupportRangeControl)this).Invalidate(true);
		}
		NormalizedRange ISupportRangeControl.ValidateNormalRange(NormalizedRange range, RangeValidationBase validationBase) {
			ChartRangeControlClientGridOptions options = ActualRangeControlClientGridOptions;
			RangeControlClientSnapCalculator snapCalculator = new RangeControlClientSnapCalculator(options, GetRangeControlMap());
			return snapCalculator.SnapNormalRange(range, validationBase, options.SnapUnit);
		}
		List<object> ISupportRangeControl.GetValuesInRange(object min, object max, int scaleLength) {
			rangeControlRuler.Clear();
			AxisScaleTypeMap scaleMap = ActualAxisX.ScaleTypeMap;
			double refinedMin = scaleMap.NativeToRefined(min);
			double refinedMax = scaleMap.NativeToRefined(max);
			bool minValid = !double.IsNaN(refinedMin) && !double.IsInfinity(refinedMin);
			bool maxValid = !double.IsNaN(refinedMax) && !double.IsInfinity(refinedMax);
			RangeControlMapping map = GetRangeControlMap();
			if (minValid && maxValid) {
				GridGenerator gridGenerator = new GridGenerator();
				rangeControlRuler = gridGenerator.GenerateGrid(refinedMin, refinedMax, scaleLength, map, ActualRangeControlClientGridOptions, scaleMap);
			}
			return rangeControlRuler;
		}
		string ISupportRangeControl.ValueToString(double normalizedValue) {
			object value = UnprojectValue(normalizedValue);
			return FormatRangeControlValue(value);
		}
		string ISupportRangeControl.RulerToString(int rulerIndex) {
			if (rulerIndex < rangeControlRuler.Count) {
				object value = rangeControlRuler[rulerIndex];
				return FormatRangeControlValue(value);
			}
			return String.Empty;
		}
		string FormatRangeControlValue(object value) {
			if (value != null) {
				if (value.GetType() == typeof(string))
					return (string)value;
				ChartRangeControlClientGridOptions gridOptions = ActualRangeControlClientGridOptions;
				try {
					return String.Format(gridOptions.ActualLabelFormatProvider, gridOptions.ActualLabelFormat, value);
				}
				catch {
				}
			}
			return String.Empty;
		}
		object ISupportRangeControl.ProjectBackValue(double normalOffset) {
			return UnprojectValue(normalOffset);
		}
		object UnprojectValue(double normalOffset) {
			double value = GetRangeControlMap().NormalValueToValue(normalOffset);
			if (ActualAxisX.ScaleTypeMap is AxisQualitativeMap) {
				AxisQualitativeMap qualitativeMap = ActualAxisX.ScaleTypeMap as AxisQualitativeMap;
				value = Math.Max(0, Math.Min(qualitativeMap.UniqueValuesCount - 1, value));
			}
			return ActualAxisX.ScaleTypeMap.RefinedToNative(value);
		}
		double ISupportRangeControl.ProjectValue(object value) {
			double refinedValue = ActualAxisX.ScaleTypeMap.NativeToRefined(value);
			return GetRangeControlMap().ValueToNormalValue(refinedValue);
		}
		MinMaxValues SnapRange(double min, double max) {
			ChartRangeControlClientGridOptions options = ActualRangeControlClientGridOptions;
			RangeControlMapping mapping = GetRangeControlMap();
			RangeControlClientSnapCalculator snapCalculator = new RangeControlClientSnapCalculator(options, mapping);
			MinMaxValues snapedRange = snapCalculator.SnapRange(min, max, RangeValidationBase.Average, options.SnapUnit);
			MinMaxValues alignedRange = mapping.EnsureRangeInsideWholeRange(snapedRange);
			return alignedRange;
		}
		void ISupportRangeControl.DrawContent(IRangeControlPaint paint) {
			RangeControlPainter painter = new RangeControlPainter(this, paint);
			painter.Draw();
		}
		void SetAxisRange(double minValueRefined, double maxValueRefined, AxisBase axis) {
			AxisScaleTypeMap scaleMap = axis.ScaleTypeMap;
			double internalMin = scaleMap.RefinedToInternalExact(minValueRefined);
			double internalMax = scaleMap.RefinedToInternalExact(maxValueRefined);
			object min = scaleMap.RefinedToNative(minValueRefined);
			object max = scaleMap.RefinedToNative(maxValueRefined);
			axis.VisualRangeData.ScrollOrZoomRange(min, max, internalMin, internalMax, true);
			axis.UpdateAutoMeasureUnit(true);
		}
		void UpdateAxisRange(double minValueRefined, double maxValueRefined) {
			AxisBase axisX = ActualAxisX;
			IVisualAxisRangeData visualRange = axisX.VisualRangeData;
			if (minValueRefined == visualRange.RefinedMin && maxValueRefined == visualRange.RefinedMax)
				return;
			MinMaxValues modifiedRange = new MinMaxValues(minValueRefined, maxValueRefined);
			MinMaxValues baseRange;
			int rangeSnapIterationsCount = 0;
			do {
				baseRange = SnapRange(modifiedRange.Min, modifiedRange.Max);
				SetAxisRange(baseRange.Min, baseRange.Max, axisX);
				AxisScaleTypeMap scaleMap = axisX.ScaleTypeMap;
				double actualMinValue = scaleMap.InternalToRefinedExact(visualRange.Min);
				double actualMaxValue = scaleMap.InternalToRefinedExact(visualRange.Max);
				modifiedRange = new MinMaxValues(actualMinValue, actualMaxValue);
				rangeSnapIterationsCount++;
			}
			while ((MaxRangeSnapIterationsCount > rangeSnapIterationsCount) && (!baseRange.Equals(modifiedRange)));
		}
		void ISupportRangeControl.RangeChanged(object minValue, object maxValue) {
			AxisScaleTypeMap scaleMap = ActualAxisX.ScaleTypeMap;
			double actualMinValue = (minValue == null) || !scaleMap.IsCompatible(minValue) ? double.NaN : scaleMap.NativeToRefined(minValue);
			double actualMaxValue = (maxValue == null) || !scaleMap.IsCompatible(maxValue) ? double.NaN : scaleMap.NativeToRefined(maxValue);
			RangeControlMapping rangeControlMap = GetRangeControlMap();
			if (double.IsNaN(actualMinValue))
				actualMinValue = rangeControlMap.NormalValueToValue(0.0);
			if (double.IsNaN(actualMaxValue))
				actualMaxValue = rangeControlMap.NormalValueToValue(1.0);
			UpdateAxisRange(actualMinValue, actualMaxValue);
			ActualAxisX.ContainerAdapter.Invalidate();
			Chart.InvalidateDrawingHelper();
			Chart.ClearCache();
		}
		bool ISupportRangeControl.CheckTypeSupport(Type type) {
			return ActualAxisX.ScaleTypeMap.IsCompatibleType(type);
		}
		void ISupportRangeControl.Invalidate(bool redraw) {
			rangeControlMap = null;
			AxisScaleTypeMap scaleMap = ActualAxisX.ScaleTypeMap;
			VisualRange visualRange = ActualAxisX.VisualRange;
			double min = scaleMap.InternalToRefinedExact(visualRange.MinValueInternal);
			double max = scaleMap.InternalToRefinedExact(visualRange.MaxValueInternal);
			Chart.Container.RaiseRangeControlRangeChanged(scaleMap.RefinedToNative(min), scaleMap.RefinedToNative(max), redraw);
		}
		object ISupportRangeControl.NativeValue(double normalOffset) {
			return ((ISupportRangeControl)this).ProjectBackValue(normalOffset);
		}
		object ISupportRangeControl.GetOptions() {
			switch (ActualAxisX.ScaleType) {
				case ActualScaleType.DateTime:
					return rangeControlDateTimeOptions;
				case ActualScaleType.Numerical:
					return rangeControlNumericOptions;
				case ActualScaleType.Qualitative:
				default:
					return null;
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeScrollingZoomingProperties() {
			IChartContainer chartContainer = ChartContainer;
			return chartContainer == null || chartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeMargins() {
			return margins.ShouldSerialize();
		}
		bool ShouldSerializeDefaultPane() {
			return defaultPane.ShouldSerialize();
		}
		bool ShouldSerializeScrollingOptions() {
			return scrollingOptions.ShouldSerialize();
		}
		bool ShouldSerializeZoomingOptions() {
			return zoomingOptions.ShouldSerialize();
		}
		bool ShouldSerializeEnableAxisXScrolling() {
			return ShouldSerializeScrollingZoomingProperties() && (enableAxisXScrolling != DefaultEnableAxisXScrolling);
		}
		void ResetEnableAxisXScrolling() {
			EnableAxisXScrolling = DefaultEnableAxisXScrolling;
		}
		bool ShouldSerializeEnableAxisYScrolling() {
			return ShouldSerializeScrollingZoomingProperties() && (enableAxisYScrolling != DefaultEnableAxisYScrolling);
		}
		void ResetEnableAxisYScrolling() {
			EnableAxisYScrolling = DefaultEnableAxisYScrolling;
		}
		bool ShouldSerializeEnableAxisXZooming() {
			return ShouldSerializeScrollingZoomingProperties() && (enableAxisXZooming != DefaultEnableAxisXZooming);
		}
		void ResetEnableAxisXZooming() {
			EnableAxisXZooming = DefaultEnableAxisXZooming;
		}
		bool ShouldSerializeEnableAxisYZooming() {
			return ShouldSerializeScrollingZoomingProperties() && (enableAxisYZooming != DefaultEnableAxisYZooming);
		}
		void ResetEnableAxisYZooming() {
			EnableAxisYZooming = DefaultEnableAxisYZooming;
		}
		bool ShouldSerializeEnableZooming() {
			return false;
		}
		bool ShouldSerializeEnableScrolling() {
			return false;
		}
		bool ShouldSerializePaneLayoutDirection() {
			return paneLayoutDirection != DefaultPaneLayoutDirection;
		}
		void ResetPaneLayoutDirection() {
			PaneLayoutDirection = DefaultPaneLayoutDirection;
		}
		bool ShouldSerializePaneDistance() {
			return paneDistance != DefaultPaneDistance;
		}
		void ResetPaneDistance() {
			PaneDistance = DefaultPaneDistance;
		}
		bool ShouldSerializeRangeControlDateTimeGridOptions() {
			return rangeControlDateTimeGridOptions.ShouldSerialize();
		}
		bool ShouldSerializeRangeControlNumericGridOptions() {
			return rangeControlNumericGridOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeMargins() || ShouldSerializeDefaultPane() || panes.Count > 0 ||
				ShouldSerializeScrollingOptions() || ShouldSerializeZoomingOptions() || ShouldSerializeEnableAxisXScrolling() ||
				ShouldSerializeEnableAxisYScrolling() || ShouldSerializeEnableAxisXZooming() || ShouldSerializeEnableAxisYZooming() ||
				ShouldSerializePaneLayoutDirection() || ShouldSerializePaneDistance() || ShouldSerializeRangeControlDateTimeGridOptions() ||
				ShouldSerializeRangeControlNumericGridOptions();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Margins":
					return ShouldSerializeMargins();
				case "DefaultPane":
					return ShouldSerializeDefaultPane();
				case "ScrollingOptions":
					return ShouldSerializeScrollingOptions();
				case "ZoomingOptions":
					return ShouldSerializeZoomingOptions();
				case "EnableAxisXScrolling":
					return ShouldSerializeEnableAxisXScrolling();
				case "EnableAxisYScrolling":
					return ShouldSerializeEnableAxisYScrolling();
				case "EnableAxisXZooming":
					return ShouldSerializeEnableAxisXZooming();
				case "EnableAxisYZooming":
					return ShouldSerializeEnableAxisYZooming();
				case "EnableScrolling":
					return ShouldSerializeEnableScrolling();
				case "EnableZooming":
					return ShouldSerializeEnableZooming();
				case "PaneLayoutDirection":
					return ShouldSerializePaneLayoutDirection();
				case "PaneDistance":
					return ShouldSerializePaneDistance();
				case "RangeControlDateTimeGridOptions":
					return ShouldSerializeRangeControlDateTimeGridOptions();
				case "RangeControlNumericGridOptions":
					return ShouldSerializeRangeControlNumericGridOptions();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		protected virtual void XtraSetIndexCollectionItem(string propertyName, object item) {
			if (propertyName == "Panes")
				panes.Add((XYDiagramPane)item);
		}
		protected virtual object XtraCreateCollectionItem(string propertyName) {
			return propertyName == "Panes" ? new XYDiagramPane() : null;
		}
		#endregion
		#region IIndicatorCalculator
		void IIndicatorCalculator.CalculateIndicators(IEnumerable<IRefinedSeries> activeSeries) {
			IndicatorColorGenerator colorGenerator = new IndicatorColorGenerator(Chart);
			indicatorsRepository.Clear();
			foreach (IRefinedSeries refinedSeries in activeSeries) {
				SeriesBase series = refinedSeries.Series as SeriesBase;
				if (series != null && series.View != null) {
					series.ValidateIndicators(refinedSeries);
					foreach (Indicator indicator in ((XYDiagram2DSeriesViewBase)series.View).Indicators) {
						indicatorsRepository.Add(indicator);
						indicator.IndicatorBehavior.Calculate(refinedSeries, colorGenerator);
					}
				}
			}
		}
		#endregion
		void PerformGestureZoom(PaneAxesContainer paneAxesData) {
			if (Chart.CanZoom(gestureZoomCenter)) {
				double zoomPercent = 1 / gestureZoomPercent;
				double axisXMin = paneAxesData.PrimaryAxisX.AxisScaleTypeMap.RefinedToInternal(gestureStartAxisXMin);
				double axisXMax = paneAxesData.PrimaryAxisX.AxisScaleTypeMap.RefinedToInternal(gestureStartAxisXMax);
				double axisYMin = paneAxesData.PrimaryAxisY.AxisScaleTypeMap.RefinedToInternal(gestureStartAxisYMin);
				double axisYMax = paneAxesData.PrimaryAxisY.AxisScaleTypeMap.RefinedToInternal(gestureStartAxisYMax);
				paneAxesData.Zoom(GraphicUtils.ConvertPoint(gestureZoomCenter), axisXMin, axisXMax, axisYMin, axisYMax, zoomPercent, zoomPercent,
					ZoomingKind.Gesture, zoomPercent > 1 ? NavigationType.ZoomOut : NavigationType.ZoomIn);
			}
		}
		bool ShouldUpdateAutomaticLayout(IEnumerable<IAxisData> axes) {
			foreach (IAxisData axis in axes) {
				if (((Axis2D)axis).Visibility == DefaultBoolean.Default)
					return true;
			}
			return false;
		}
		bool HasAutoLayoutAxes(List<Axis2D> axes) {
			foreach (Axis2D axis in axes)
				if (axis.Visibility == DefaultBoolean.Default || axis.Title.Visibility == DefaultBoolean.Default)
					return true;
			return false;
		}
		XYDiagramPaneBase GetPaneByPoint(Point point) {
			foreach (XYDiagramPaneBase pane in ActualPanes)
				if (pane.ContainsPoint(point))
					return pane;
			return null;
		}
		ControlCoordinates CalcDiagramToPointCoordinates(XYDiagramPaneBase pane, IAxisData axisX, IAxisData axisY, object argument, object value) {
			if (pane.MappingList == null) {
				Chart.PerformViewDataCalculation(Rectangle.Empty, false);
				if (pane.MappingList == null)
					return new ControlCoordinates(pane);
			}
			XYDiagramMappingContainer mappingContainer = pane.MappingList.GetMappingContainer((Axis2D)axisX, (Axis2D)axisY);
			if (mappingContainer == null)
				return new ControlCoordinates(pane);
			double argumentInternal = axisX.AxisScaleTypeMap.NativeToInternal(argument);
			IMinMaxValues limits = axisX.WholeRange;
			return (axisX.AxisScaleTypeMap.ScaleType == ActualScaleType.Qualitative && (argumentInternal < limits.Min || argumentInternal > limits.Max)) ?
				new ControlCoordinates(pane) : DiagramToPointCalculator.CalculateCoords(pane, mappingContainer, argumentInternal, axisY.AxisScaleTypeMap.NativeToInternal(value));
		}
		ControlCoordinates CalcDiagramToPoint(object argument, object value) {
			PaneAxesContainer paneAxesData = GetPaneAxesData(defaultPane);
			if (paneAxesData == null)
				return new ControlCoordinates(defaultPane);
			IAxisData axisX = paneAxesData.PrimaryAxisX;
			IAxisData axisY = paneAxesData.PrimaryAxisY;
			return (axisX == null || axisY == null) ? new ControlCoordinates(defaultPane) : CalcDiagramToPointCoordinates(defaultPane, axisX, axisY, argument, value);
		}
		ControlCoordinates CalcDiagramToPoint(object argument, object value, Axis2D axisX, Axis2D axisY) {
			if (AxisPaneRepository == null)
				return new ControlCoordinates(defaultPane);
			XYDiagramPaneBase pane = AxisPaneRepository.GetFirstPaneByAxes(axisX, axisY) as XYDiagramPaneBase;
			return pane == null ? new ControlCoordinates(defaultPane) : CalcDiagramToPointCoordinates(pane, axisX, axisY, argument, value);
		}
		ControlCoordinates CalcDiagramToPoint(object argument, object value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			ControlCoordinates coordinates = new ControlCoordinates(pane);
			PaneAxesContainer paneAxisData = GetPaneAxesData(pane);
			return (paneAxisData == null || !paneAxisData.AxesX.Contains(axisX) || !paneAxisData.AxesY.Contains(axisY)) ?
				new ControlCoordinates(pane) : CalcDiagramToPointCoordinates(pane, axisX, axisY, argument, value);
		}
		ControlCoordinates DiagramToPointInternal(object argument, object value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY, pane);
		}
		void ResetPanesMappings() {
			foreach (XYDiagramPaneBase pane in ActualPanes) {
				pane.MappingList = null;
			}
		}
		object CheckArgument(object argument) {
			PaneAxesContainer container = GetPaneAxesData(defaultPane);
			return container != null ? DiagramToPointUtils.CheckValue(argument, container.PrimaryAxisX.AxisScaleTypeMap.ScaleType) : argument;
		}
		object CheckValue(object value) {
			PaneAxesContainer container = GetPaneAxesData(defaultPane);
			return container != null ? DiagramToPointUtils.CheckValue(value, container.PrimaryAxisY.AxisScaleTypeMap.ScaleType) : value;
		}
		bool ContainOnlyRangeChange(ChartElementChange change) {
			int rangeChanged = (int)ChartElementChange.RangeChanged | (int)ChartElementChange.LimitsChanged;
			return (~rangeChanged & (int)change) == 0;
		}
		void ClearAxesLabelBounds() {
			ActualAxisX.LabelBounds.Clear();
			foreach (Axis2D axis in ActualSecondaryAxesX)
				axis.LabelBounds.Clear();
			ActualAxisY.LabelBounds.Clear();
			foreach (Axis2D axis in ActualSecondaryAxesY)
				axis.LabelBounds.Clear();
		}
		void ApplyAxesLabelBounds(XYDiagramViewData diagramViewData) {
			if (diagramViewData == null)
				return;
			foreach (XYDiagramPaneViewData paneViewData in diagramViewData.PaneViewDataList)
				foreach (AxisViewData axisViewData in paneViewData.AxisViewDataList) {
					IPane pane = paneViewData.Pane;
					Axis2D axis = axisViewData.Axis;
					Rectangle labelRect = axisViewData.CaluclateLabelBounds();
					if (!labelRect.IsEmpty)
						axis.LabelBounds.Add(pane, labelRect);
				}
		}
		Rectangle CalculateRelativeAxesRect(Point point, Rectangle lastMappingBounds) {
			point.X -= lastMappingBounds.Left;
			point.Y -= lastMappingBounds.Top;
			if (point.X < 0 || point.X > lastMappingBounds.Width || point.Y < 0 || point.Y > lastMappingBounds.Height)
				return Rectangle.Empty;
			point.Y = lastMappingBounds.Height - point.Y;
			int x, y, xLength, yLength;
			if (ActualRotated) {
				x = point.Y;
				y = point.X;
				xLength = lastMappingBounds.Height;
				yLength = lastMappingBounds.Width;
			}
			else {
				x = point.X;
				y = point.Y;
				xLength = lastMappingBounds.Width;
				yLength = lastMappingBounds.Height;
			}
			return new Rectangle(x, y, xLength, yLength);
		}
		InternalCoordinates MapPointToInternal(XYDiagramPaneBase pane, Point point) {
			PaneAxesContainer paneAxesData = GetPaneAxesData(pane);
			if (paneAxesData == null)
				return null;
			Rectangle lastMappingBounds = pane.LastMappingBounds.Value;
			if (lastMappingBounds.IsEmpty)
				return null;
			Rectangle axesRect = CalculateRelativeAxesRect(point, lastMappingBounds);
			if (axesRect.IsEmpty)
				return null;
			InternalCoordinates coords = new InternalCoordinates(pane);
			Axis2D axisX = (Axis2D)paneAxesData.PrimaryAxisX;
			if (axisX != null) {
				double? argument = axisX.CalculateInternalValue(pane, axesRect.X, axesRect.Width);
				if (argument.HasValue)
					coords.AddAxisXValue(axisX, argument.Value);
			}
			foreach (Axis2D secondaryAxisX in paneAxesData.SecondaryAxesX) {
				double? argument = secondaryAxisX.CalculateInternalValue(pane, axesRect.X, axesRect.Width);
				if (argument.HasValue)
					coords.AddAxisXValue(secondaryAxisX, argument.Value);
			}
			Axis2D axisY = (Axis2D)paneAxesData.PrimaryAxisY;
			if (axisY != null) {
				double? value = axisY.CalculateInternalValue(pane, axesRect.Y, axesRect.Height);
				if (value.HasValue)
					coords.AddAxisYValue(axisY, value.Value);
			}
			foreach (Axis2D secondaryAxisY in paneAxesData.SecondaryAxesY) {
				double? value = secondaryAxisY.CalculateInternalValue(pane, axesRect.Y, axesRect.Height);
				if (value.HasValue)
					coords.AddAxisYValue(secondaryAxisY, value.Value);
			}
			return coords;
		}
		ControlCoordinates MapInternalToPoint(XYDiagramPaneBase pane, IAxisData axisX, IAxisData axisY, double argument, double value) {
			if (pane.MappingList == null)
				return new ControlCoordinates(pane);
			XYDiagramMappingContainer mappingContainer = pane.MappingList.FindMappingContainer(axisX, axisY);
			if (mappingContainer == null)
				return new ControlCoordinates(pane);
			return DiagramToPointCalculator.CalculateCoords(pane, mappingContainer, argument, value);
		}
		List<IPane> GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync) {
			List<IPane> crosshairPanes = new List<IPane>();
			if ((PaneLayoutDirection == PaneLayoutDirection.Vertical && isHorizontalSync) ||
				(PaneLayoutDirection == PaneLayoutDirection.Horizontal && !isHorizontalSync))
				crosshairPanes.AddRange(ActualPanes);
			else
				crosshairPanes.Add(focusedPane);
			return crosshairPanes;
		}
		List<CrosshairHighlightedPointInfo> GetHighlightedPointsInfo(CrosshairInfoEx crosshairInfos) {
			List<CrosshairHighlightedPointInfo> highlightedPointsLayout = new List<CrosshairHighlightedPointInfo>();
			foreach (CrosshairPaneInfoEx paneInfo in crosshairInfos)
				foreach (CrosshairSeriesPointEx point in paneInfo.SeriesPoints) {
					List<CrosshairHighlightedPointInfo> pointsInfo = ((Series)point.RefinedSeries.Series).ActualCrosshairHighlightPoints ? CalculateHighlightedPointInfo(point) : null;
					if (pointsInfo != null)
						foreach (CrosshairHighlightedPointInfo info in pointsInfo)
							if (!highlightedPointsLayout.Contains(info))
								highlightedPointsLayout.Add(info);
				}
			return highlightedPointsLayout;
		}
		List<CrosshairHighlightedPointInfo> CalculateHighlightedPointInfo(CrosshairSeriesPointEx crosshairPoint) {
			List<CrosshairHighlightedPointInfo> pointInfoList = new List<CrosshairHighlightedPointInfo>();
			if (crosshairPoint != null) {
				XYDiagram2DSeriesViewBase view = crosshairPoint.View as XYDiagram2DSeriesViewBase;
				if (view != null) {
					XYDiagramPaneBase pane = crosshairPoint.View.Pane as XYDiagramPaneBase;
					if (pane != null && pane.MappingList != null) {
						XYDiagramMappingContainer mappingContainer = pane.MappingList.GetMappingContainer(view.ActualAxisX, view.ActualAxisY);
						DrawOptions pointDrawOptions = view.CreatePointDrawOptions(crosshairPoint.RefinedSeries, crosshairPoint.RefinedPoint);
						pointDrawOptions.InitializeSeriesPointDrawOptions(view, crosshairPoint.RefinedSeries, crosshairPoint.PointIndex);
						if (mappingContainer != null)
							foreach (XYDiagramMappingBase mapping in mappingContainer) {
								HighlightedPointLayout pointLayout = view.CalculateHighlightedPointLayout(mapping, crosshairPoint.RefinedPoint, view, pointDrawOptions);
								if (pointLayout != null) {
									Region clipRegion;
									if (pane.SeriesClipRegions.TryGetValue(mapping, out clipRegion)) {
										CrosshairHighlightedPointInfo pointInfo = new CrosshairHighlightedPointInfo(pointLayout, (Series)crosshairPoint.RefinedSeries.Series, clipRegion);
										pointInfoList.Add(pointInfo);
									}
								}
							}
					}
				}
			}
			return pointInfoList;
		}
		CrosshairInfoEx GetCrosshairInfoEx(IList<IRefinedSeries> refinedSeries) {
			if (crosshairLocation.IsEmpty)
				return null;
			XYDiagramPaneBase focusedPane = GetPaneByPoint(crosshairLocation);
			if (focusedPane == null)
				return null;
			GRealPoint2D cursorLocation = new GRealPoint2D(crosshairLocation.X, crosshairLocation.Y);
			CrosshairInfoEx crosshairInfos = crosshairManager.CalculateCrosshairInfo(cursorLocation, ((IDiagram)this).ChartBounds, paneAxesContainerRepository.Values, focusedPane, refinedSeries);
			return crosshairInfos;
		}
		protected virtual int GetArgumentAxisLength(XYDiagramPaneBase pane) {
			if (pane.LastMappingBounds.HasValue)
				return ActualRotated ? pane.LastMappingBounds.Value.Height : pane.LastMappingBounds.Value.Width;
			return DefaultAxisXLength;
		}
		protected virtual int GetValueAxisLength(XYDiagramPaneBase pane) {
			if (pane.LastMappingBounds.HasValue)
				return ActualRotated ? pane.LastMappingBounds.Value.Width : pane.LastMappingBounds.Value.Height;
			return DefaultAxisYLength;
		}
		protected virtual IList<IAxisData> GetArgumentAxesWithAutomaticUnits(PaneAxesContainer container) {
			return container.AxesX;
		}
		protected virtual IList<IAxisData> GetValueAxesWithAutomaticUnits(PaneAxesContainer container) {
			return container.AxesY;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				defaultPane.Dispose();
				panes.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void UpdateDiagramAccordingInfo(SeriesBase seriesTemplate, IEnumerable<IRefinedSeries> activeSeries) {
			if (seriesTemplate != null)
				seriesTemplate.ValidateIndicators(null);
			foreach (XYDiagramPaneBase pane in ActualPanes)
				foreach (Annotation annotation in pane.Annotations) {
					PaneAnchorPoint anchorPoint = annotation.AnchorPoint as PaneAnchorPoint;
					if (anchorPoint != null) {
						anchorPoint.AxisXCoordinate.UpdateAxisValue();
						anchorPoint.AxisYCoordinate.UpdateAxisValue();
					}
				}
			ActualAxisX.UpdateAxisValueContainers();
			ActualAxisX.UpdateIntervals();
			ActualAxisY.UpdateAxisValueContainers();
			ActualAxisY.UpdateIntervals();
			foreach (Axis2D secondaryAxisX in ActualSecondaryAxesX) {
				secondaryAxisX.UpdateAxisValueContainers();
				secondaryAxisX.UpdateIntervals();
			}
			foreach (Axis2D secondaryAxisY in ActualSecondaryAxesY) {
				secondaryAxisY.UpdateAxisValueContainers();
				secondaryAxisY.UpdateIntervals();
			}
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			ChartElementUpdateInfo chartElementUpdateInfo = changeInfo as ChartElementUpdateInfo;
			if (chartElementUpdateInfo == null)
				return base.ProcessChanged(sender, changeInfo);
			ChartElementChange change = chartElementUpdateInfo.Flags;
			if (!ContainOnlyRangeChange(change))
				return base.ProcessChanged(sender, changeInfo);
			return true;
		}
		protected internal override List<VisibilityLayoutRegion> GetAutolayoutElements(Rectangle bounds) {
			List<VisibilityLayoutRegion> regions = new List<VisibilityLayoutRegion>();
			List<IPane> panes = new List<IPane>();
			if (DefaultPane.Visible)
				panes.Add(defaultPane);
			foreach (XYDiagramPaneBase pane in Panes)
				if (pane.Visible)
					panes.Add(pane);
			if (panes.Count > 0) {
				XYDiagramPaneLayoutCalculator calculator = new XYDiagramPaneLayoutCalculator(panes, PaneDistance, PaneLayoutDirection, bounds, bounds);
				List<XYDiagramPaneLayout> layouts = calculator.Calculate();
				if (layouts != null)
					for (int i = 0; i < layouts.Count; i++) {
						PaneAxesContainer paneAxisData = GetPaneAxesData(layouts[i].Pane);
						if (paneAxisData != null) {
							IEnumerable<IAxisData> axes = paneAxisData.Axes;
							if (ShouldUpdateAutomaticLayout(axes)) {
								List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
								foreach (Axis2D axis in axes) {
									if (axis.Visibility == DefaultBoolean.Default)
										elements.Add(axis);
									if (axis.Title.Visibility == DefaultBoolean.Default)
										elements.Add(axis.Title);
								}
								GRealSize2D size = new GRealSize2D(layouts[i].MaxBounds.Width, layouts[i].MaxBounds.Height);
								regions.Add(new VisibilityLayoutRegion(size, elements));
							}
						}
					}
			}
			return regions;
		}
		protected internal IList<AxisBase> UpdateAutomaticMeasurement(bool updateViewController) {
			List<AxisBase> affectedAxes = new List<AxisBase>();
			IList<IPane> panes = ActualPanes;
			foreach (XYDiagramPaneBase pane in panes)
				affectedAxes.AddRange(UpdateAutomaticMeasurement(pane, updateViewController));
			return affectedAxes;
		}
		protected internal IList<AxisBase> UpdateAutomaticMeasurement(XYDiagramPaneBase pane, bool updateViewController = true) {
			List<AxisBase> affectedAxes = new List<AxisBase>();
			PaneAxesContainer paneAxesContainer = GetPaneAxesData(pane);
			if (paneAxesContainer != null) {
				int axisLength = GetArgumentAxisLength(pane);
				foreach (AxisBase axisX in GetArgumentAxesWithAutomaticUnits(paneAxesContainer)) {
					if (axisX.UpdateAutomaticMeasureUnit(axisLength))
						affectedAxes.Add(axisX);
				}
				axisLength = GetValueAxisLength(pane);
				foreach (AxisBase axisY in GetValueAxesWithAutomaticUnits(paneAxesContainer)) {
					if (axisY.UpdateAutomaticMeasureUnit(axisLength))
						affectedAxes.Add(axisY);
				}
			}
			if (updateViewController)
				ViewController.SendDataAgreggationUpdates(affectedAxes);
			return affectedAxes;
		}
		protected internal override void OnUpdateBounds() {
			base.OnUpdateBounds();
			UpdateAutomaticMeasurement(true);
		}
		protected internal override void Update(IEnumerable<IRefinedSeries> activeSeries) {
			base.Update(activeSeries);
			if (paneAxesContainerRepository != null)
				paneAxesContainerRepository.UpdateRanges();
			if (DependsOnBounds && !Loading)
				Chart.PerformViewDataCalculationWithoutEvents(Chart.LastBounds, true);
			else
				ResetPanesMappings();
		}
		protected internal override void ClearResolveOverlappingCache() {
			foreach (XYDiagramPaneBase pane in ActualPanes)
				pane.ResolveOverlappingCache = null;
			List<Axis2D> allAxesX = GetAllAxesX();
			foreach (Axis2D axis in allAxesX) {
				axis.OverlappingCache = null;
				axis.ResetScrollLabelSizeCache();
			}
			List<Axis2D> allAxesY = GetAllAxesY();
			foreach (Axis2D axis in allAxesY) {
				axis.OverlappingCache = null;
				axis.ResetScrollLabelSizeCache();
			}
		}
		protected internal override void ClearAnnotations() {
			base.ClearAnnotations();
			defaultPane.ClearAnnotations();
			foreach (XYDiagramPaneBase pane in panes)
				pane.ClearAnnotations();
		}
		protected internal override bool PerformDragging(int x, int y, int dx, int dy, ChartScrollEventType scrollEventType, object focusedElement) {
			if (!ScrollingOptions.UseTouchDevice)
				return false;
			XYDiagramPaneBase pane = (scrollEventType == ChartScrollEventType.ArrowKeys ? (focusedElement as XYDiagramPaneBase) : focusedPane) ?? defaultPane;
			PaneAxesContainer paneAxesData = GetPaneAxesData(pane);
			if (scrollEventType == ChartScrollEventType.ArrowKeys) {
				dx = -dx;
				dy = -dy;
			}
			return paneAxesData != null ? paneAxesData.Scroll(dx, dy, true, (NavigationType)scrollEventType) : false;
		}
		protected internal override bool CanDrag(Point point, MouseButtons button) {
			focusedPane = GetPaneByPoint(point);
			if (focusedPane == null || !scrollingOptions.UseMouse)
				return false;
			PaneAxesContainer paneAxesData = GetPaneAxesData(focusedPane);
			return paneAxesData != null && paneAxesData.ScrollingEnabled;
		}
		protected internal override bool CanZoom(Point point) {
			focusedPane = GetPaneByPoint(point);
			return focusedPane != null && (EnableAxisXZooming || EnableAxisYZooming);
		}
		protected internal override Point GetZoomRegionPosition(Point p) {
			XYDiagramPaneBase pane = ActualFocusedPane;
			PaneAxesContainer paneAxesData = GetPaneAxesData(pane);
			if (paneAxesData == null)
				return p;
			GPoint2D result = paneAxesData.GetZoomRegionPosition(GraphicUtils.ConvertPoint(p));
			return new Point(result.X, result.Y);
		}
		protected internal override void Zoom(int delta, ZoomingKind zoomingKind, object focusedElement) {
			XYDiagramPaneBase pane = (zoomingKind == ZoomingKind.Keyboard ? (focusedElement as XYDiagramPaneBase) : focusedPane) ?? defaultPane;
			PaneAxesContainer paneAxesData = GetPaneAxesData(pane);
			if (paneAxesData != null)
				paneAxesData.Zoom(delta, zoomingKind);
			UpdateAutomaticMeasurement(pane);
		}
		protected internal override void ZoomIn(Rectangle rect) {
			PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
			if (paneAxesData != null)
				paneAxesData.ZoomIn(GraphicUtils.ConvertRect(rect));
			UpdateAutomaticMeasurement(ActualFocusedPane);
		}
		protected internal override void ZoomIn(Point center) {
			PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
			if (paneAxesData != null)
				paneAxesData.Zoom(GraphicUtils.ConvertPoint(center), 1 / ZoomPercent, ZoomingKind.ZoomIn, NavigationType.ZoomIn);
		}
		protected internal override void ZoomOut(Point center) {
			PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
			if (paneAxesData != null)
				paneAxesData.Zoom(GraphicUtils.ConvertPoint(center), ZoomPercent, ZoomingKind.ZoomOut, NavigationType.ZoomOut);
			UpdateAutomaticMeasurement(ActualFocusedPane);
		}
		protected internal override void UndoZoom() {
			ZoomCacheEx.Pop();
		}
		protected internal override void ClearZoomCache() {
			ZoomCacheEx.Clear();
		}
		protected internal override void DrawZoomRectangle(Graphics gr, Rectangle rect) {
			XYDiagramPaneBase pane = ActualFocusedPane;
			if (pane.ActualEnableAxisXZooming && !pane.ActualEnableAxisYZooming)
				if (ActualRotated)
					pane.DrawZoomRectangleHorizontal(gr, rect);
				else
					pane.DrawZoomRectangleVertical(gr, rect);
			else if (pane.ActualEnableAxisYZooming && !pane.ActualEnableAxisXZooming)
				if (ActualRotated)
					pane.DrawZoomRectangleVertical(gr, rect);
				else
					pane.DrawZoomRectangleHorizontal(gr, rect);
			else
				pane.DrawZoomRectangle(gr, rect);
		}
		protected internal override string GetDesignerHint(Point p) {
			return ChartLocalizer.GetString(ChartStringId.Msg2DScrollingToolTip);
		}
		protected internal override bool Contains(object obj) {
			XYDiagramPane pane = obj as XYDiagramPane;
			return pane != null && panes.Contains(pane);
		}
		protected internal override INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hDC, Rectangle bounds, Rectangle windowsBounds) {
			return new GdiPlusGraphics(gr);
		}
		protected internal override void FinishLoading() {
			base.FinishLoading();
			if ((Chart != null) && (Chart.Container != null))
				((ISupportRangeControl)this).Invalidate(true);
		}
		protected internal override DiagramViewData CalculateViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			ClearAxesLabelBounds();
			XYDiagramViewData diagramViewData = new XYDiagramViewDataCalculator(this).Calculate(textMeasurer, diagramBounds, seriesDataList, performRangeCorrection);
			ApplyAxesLabelBounds(diagramViewData);
			isEmpty = diagramViewData == null;
			return diagramViewData;
		}
		protected internal override IList<ILegendItemData> GetLegendItems() {
			List<ILegendItemData> itemsList = new List<ILegendItemData>();
			itemsList.AddRange(ActualAxisX.ConstantLines.GetLegendItems());
			foreach (Axis2D secondaryAxisX in ActualSecondaryAxesX)
				itemsList.AddRange(secondaryAxisX.ConstantLines.GetLegendItems());
			itemsList.AddRange(ActualAxisY.ConstantLines.GetLegendItems());
			foreach (Axis2D secondaryAxisY in ActualSecondaryAxesY)
				itemsList.AddRange(secondaryAxisY.ConstantLines.GetLegendItems());
			itemsList.AddRange(ActualAxisX.Strips.GetLegendItems());
			foreach (Axis2D secondaryAxisX in ActualSecondaryAxesX)
				itemsList.AddRange(secondaryAxisX.Strips.GetLegendItems());
			itemsList.AddRange(ActualAxisY.Strips.GetLegendItems());
			foreach (Axis2D secondaryAxisY in ActualSecondaryAxesY)
				itemsList.AddRange(secondaryAxisY.Strips.GetLegendItems());
			return itemsList;
		}
		protected internal override string GetInvisibleDiagramMessage() {
			return ChartLocalizer.GetString(ChartStringId.MsgNoPanes);
		}
		protected internal override CoordinatesConversionCache CreateCoordinatesConversionCache() {
			XYDiagramCoordinatesConversionCache cache = new XYDiagramCoordinatesConversionCache();
			foreach (XYDiagramPaneBase pane in ActualPanes)
				cache.Register(pane, pane.MappingList, pane.LastMappingBounds);
			return cache;
		}
		protected internal virtual void BeginZooming() {
		}
		protected internal void EndZooming(ChartZoomEventType eventType, RangesSnapshot oldRange, RangesSnapshot newRange, AxisBase axisX, AxisBase axisY) {
			if (oldRange != newRange) {
				Chart chart = Chart;
				ChartZoomEventArgs args = new ChartZoomEventArgs(eventType, oldRange.XRange, oldRange.YRange, newRange.XRange, newRange.YRange, axisX, axisY);
				chart.ContainerAdapter.OnZoom(args);
				chart.ContainerAdapter.Invalidate();
				chart.InvalidateDrawingHelper();
				chart.ClearCache();
				ClearResolveOverlappingCache();
			}
		}
		protected internal virtual void EndScrolling(ChartScrollOrientation orientation, ChartScrollEventType eventType, RangesSnapshot oldRange, RangesSnapshot newRange, AxisBase axisX, AxisBase axisY) {
			if (oldRange != newRange) {
				Chart chart = Chart;
				ChartScrollEventArgs args = new ChartScrollEventArgs(orientation, eventType, oldRange.XRange, oldRange.YRange, newRange.XRange, newRange.YRange, axisX, axisY);
				chart.ContainerAdapter.OnScroll(args);
				chart.ContainerAdapter.Invalidate();
				chart.InvalidateDrawingHelper();
				chart.ClearCache();
			}
		}
		protected internal abstract XYDiagramMappingBase CreateDiagramMapping(XYDiagramMappingContainer container, AxisIntervalLayout layoutX, AxisIntervalLayout layoutY);
		protected internal override void BeginGestureZoom(Point center, double zoomDelta) {
			gestureZoomCenter = center;
			gestureZoomPercent = zoomDelta;
			PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
			if (paneAxesData != null) {
				gestureStartAxisXMin = paneAxesData.PrimaryAxisX.VisualRange.RefinedMin;
				gestureStartAxisXMax = paneAxesData.PrimaryAxisX.VisualRange.RefinedMax;
				gestureStartAxisYMin = paneAxesData.PrimaryAxisY.VisualRange.RefinedMin;
				gestureStartAxisYMax = paneAxesData.PrimaryAxisY.VisualRange.RefinedMax;
				PerformGestureZoom(paneAxesData);
			}
		}
		protected internal override void PerformGestureZoom(double zoomDelta) {
			gestureZoomPercent *= zoomDelta;
			PaneAxesContainer paneAxesData = GetPaneAxesData(ActualFocusedPane);
			if (paneAxesData != null)
				PerformGestureZoom(paneAxesData);
		}
		protected internal override void UpdateCrosshairLocation(Point cursorLocation) {
			crosshairLocation = cursorLocation;
		}
		protected internal List<CrosshairPaneViewData> CalculateCrosshairViewData(IList<IRefinedSeries> series) {
			CrosshairInfoEx crosshairInfos = GetCrosshairInfoEx(series);
			if (crosshairInfos == null)
				return null;
			CrosshairPaneViewDataFactory crosshairPaneViewDataFactory = new CrosshairPaneViewDataFactory(textMeasurer,
				CommonUtils.GetActualAppearance(this).TextAnnotationAppearance, Chart.CrosshairOptions);
			List<CrosshairHighlightedPointInfo> highlightedPointsInfo = GetHighlightedPointsInfo(crosshairInfos);
			crosshairPaneViewDataFactory.ProcessCrosshairInfoEx(crosshairInfos, highlightedPointsInfo, Chart.CrosshairOptions.CrosshairLabelMode == CrosshairLabelMode.ShowCommonForAllSeries);
			if (crosshairPaneViewDataFactory.CustomDrawCrosshairEventArgs != null) {
				this.ContainerAdapter.OnCustomDrawCrosshair(crosshairPaneViewDataFactory.CustomDrawCrosshairEventArgs);
				crosshairPaneViewDataFactory.ProcessAfterCustomDraw();
			}
			return crosshairPaneViewDataFactory.CrosshairPaneViewDataList;
		}
		internal int GetAxisLength(IAxisData axis) {
			if (AxisPaneRepository != null) {
				if (AxisPaneRepository.ContainsKey(axis)) {
					IList<IPane> panes = AxisPaneRepository[axis];
					foreach (XYDiagramPaneBase pane in panes)
						if (pane.LastMappingBounds.HasValue)
							return ActualRotated ? pane.LastMappingBounds.Value.Height : pane.LastMappingBounds.Value.Width;
				}
			}
			return DefaultAxisXLength;
		}
		internal PaneAxesContainer GetPaneAxesData(IPane pane) {
			if (pane == null || paneAxesContainerRepository == null)
				return null;
			return paneAxesContainerRepository.GetContaiter(pane);
		}
		internal XYDiagramPaneBase GetPaneByID(int paneID) {
			if (paneID == defaultPane.ActualPaneID)
				return defaultPane;
			foreach (XYDiagramPaneBase pane in panes)
				if (pane.ActualPaneID == paneID)
					return pane;
			return null;
		}
		internal List<CrosshairValueItem> GetCrosshairSortedData(IRefinedSeries refinedSeries) {
			crosshairManager.UpdateCrosshairData(Chart.ViewController.ActiveRefinedSeries);
			return crosshairManager.GetCrosshairSortedData(refinedSeries);
		}
		internal List<Indicator> GetIndicatorsByPane(XYDiagramPaneBase pane) {
			return indicatorsRepository.GetIndicators(pane);
		}
		public List<XYDiagramPaneBase> GetAllPanes() {
			List<XYDiagramPaneBase> totalPanes = new List<XYDiagramPaneBase>();
			totalPanes.Add(defaultPane);
			foreach (XYDiagramPane pane in panes)
				totalPanes.Add(pane);
			return totalPanes;
		}
		public List<Axis2D> GetAllAxesX() {
			List<Axis2D> axes = new List<Axis2D>();
			axes.Add(ActualAxisX);
			foreach (Axis2D axis in ActualSecondaryAxesX)
				axes.Add(axis);
			return axes;
		}
		public List<Axis2D> GetAllAxesY() {
			List<Axis2D> axes = new List<Axis2D>();
			axes.Add(ActualAxisY);
			foreach (Axis2D axis in ActualSecondaryAxesY)
				axes.Add(axis);
			return axes;
		}
		public XYDiagramPaneBase FindPaneByName(string name) {
			return name == defaultPane.Name ? (XYDiagramPaneBase)defaultPane : panes.GetPaneByName(name);
		}
		public Axis2D FindAxisXByName(string name) {
			return name == ActualAxisX.Name ? ActualAxisX : ActualSecondaryAxesX.GetAxisByNameInternal(name);
		}
		public Axis2D FindAxisYByName(string name) {
			return name == ActualAxisY.Name ? ActualAxisY : ActualSecondaryAxesY.GetAxisByNameInternal(name);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagram2D diagram = obj as XYDiagram2D;
			if (diagram == null)
				return;
			margins.Assign(diagram.margins);
			defaultPane.Assign(diagram.defaultPane);
			panes.Assign(diagram.panes);
			paneLayoutDirection = diagram.paneLayoutDirection;
			paneDistance = diagram.paneDistance;
			if (Chart == null || Chart.SupportScrollingAndZooming) {
				enableAxisXScrolling = diagram.enableAxisXScrolling;
				enableAxisYScrolling = diagram.enableAxisYScrolling;
				enableAxisXZooming = diagram.enableAxisXZooming;
				enableAxisYZooming = diagram.enableAxisYZooming;
			}
			else {
				enableAxisXScrolling = false;
				enableAxisYScrolling = false;
				enableAxisXZooming = false;
				enableAxisYZooming = false;
			}
			scrollingOptions.Assign(diagram.scrollingOptions);
			zoomingOptions.Assign(diagram.zoomingOptions);
			rangeControlQualitativeGridOptions.Assign(diagram.rangeControlQualitativeGridOptions);
			rangeControlNumericGridOptions.Assign(diagram.rangeControlNumericGridOptions);
			rangeControlDateTimeGridOptions.Assign(diagram.rangeControlDateTimeGridOptions);
		}
		public DiagramCoordinates PointToDiagram(Point p) {
			DiagramCoordinates coordinates = new DiagramCoordinates();
			XYDiagramPaneBase pane = GetPaneByPoint(p);
			if (pane == null) {
				Chart.PerformViewDataCalculation(Rectangle.Empty, false);
				pane = GetPaneByPoint(p);
			}
			if (pane == null)
				return coordinates;
			coordinates.SetPane(pane);
			PaneAxesContainer paneAxesData = GetPaneAxesData(pane);
			if (paneAxesData == null)
				return coordinates;
			Rectangle lastMappingBounds = pane.LastMappingBounds.Value;
			if (lastMappingBounds.IsEmpty)
				return coordinates;
			Rectangle axesRect = CalculateRelativeAxesRect(p, lastMappingBounds);
			if (axesRect.IsEmpty)
				return coordinates;
			Axis2D axisX = (Axis2D)paneAxesData.PrimaryAxisX;
			Axis2D axisY = (Axis2D)paneAxesData.PrimaryAxisY;
			if (axisX != null && axisY != null) {
				coordinates.SetAxes(axisX, axisY);
				double? internalArgument = axisX.CalculateInternalValue(pane, axesRect.X, axesRect.Width);
				double? internalValue = axisY.CalculateInternalValue(pane, axesRect.Y, axesRect.Height);
				if (internalArgument.HasValue && internalValue.HasValue)
					coordinates.SetArgumentAndValue(internalArgument.Value, internalValue.Value);
			}
			foreach (Axis2D secondaryAxisX in paneAxesData.SecondaryAxesX)
				coordinates.AddAxisValue(pane, secondaryAxisX, axesRect.X, axesRect.Width);
			foreach (Axis2D secondaryAxisY in paneAxesData.SecondaryAxesY)
				coordinates.AddAxisValue(pane, secondaryAxisY, axesRect.Y, axesRect.Height);
			return coordinates;
		}
		public ControlCoordinates DiagramToPoint(string argument, double value) {
			return CalcDiagramToPoint(CheckArgument(argument), CheckValue(value));
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value) {
			return CalcDiagramToPoint(CheckArgument(argument), CheckValue(value));
		}
		public ControlCoordinates DiagramToPoint(double argument, double value) {
			return CalcDiagramToPoint(CheckArgument(argument), CheckValue(value));
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value) {
			return CalcDiagramToPoint(CheckArgument(argument), CheckValue(value));
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value) {
			return CalcDiagramToPoint(CheckArgument(argument), CheckValue(value));
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value) {
			return CalcDiagramToPoint(CheckArgument(argument), CheckValue(value));
		}
		public ControlCoordinates DiagramToPoint(string argument, double value, Axis2D axisX, Axis2D axisY) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value, Axis2D axisX, Axis2D axisY) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(double argument, double value, Axis2D axisX, Axis2D axisY) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value, Axis2D axisX, Axis2D axisY) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value, Axis2D axisX, Axis2D axisY) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value, Axis2D axisX, Axis2D axisY) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType), axisX, axisY);
		}
		public ControlCoordinates DiagramToPoint(string argument, double value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return DiagramToPointInternal(argument, value, axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return DiagramToPointInternal(argument, value, axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(double argument, double value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return DiagramToPointInternal(argument, value, axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return DiagramToPointInternal(argument, value, axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return DiagramToPointInternal(argument, value, axisX, axisY, pane);
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value, Axis2D axisX, Axis2D axisY, XYDiagramPaneBase pane) {
			return DiagramToPointInternal(argument, value, axisX, axisY, pane);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class IndicatorsRepository {
		readonly Dictionary<XYDiagramPaneBase, List<Indicator>> indicators;
		public IndicatorsRepository() {
			indicators = new Dictionary<XYDiagramPaneBase, List<Indicator>>();
		}
		public void Add(Indicator indicator) {
			XYDiagramPaneBase pane;
			SeparatePaneIndicator separatePaneIndicator = indicator as SeparatePaneIndicator;
			if (separatePaneIndicator != null)
				pane = separatePaneIndicator.Pane;
			else
				pane = ((XYDiagram2DSeriesViewBase)indicator.OwningSeries.View).ActualPane;
			if (indicators.ContainsKey(pane))
				indicators[pane].Add(indicator);
			else
				indicators.Add(pane, new List<Indicator>() { indicator });
		}
		public void Clear() {
			indicators.Clear();
		}
		public List<Indicator> GetIndicators(XYDiagramPaneBase pane) {
			if (indicators.ContainsKey(pane))
				return indicators[pane];
			return null;
		}
	}
	public static class SmartAxesUtils {
		public static bool HasSmartAxis(XYDiagram2D diagram) {
			foreach (XYDiagramPaneBase pane in diagram.ActualPanes)
				if (!HasSmartAxis(pane))
					return false;
			return true;
		}
		public static bool HasSmartAxis(XYDiagramPaneBase pane) {
			PaneAxesContainer paneAxesData = pane.Diagram.GetPaneAxesData(pane);
			if (paneAxesData != null) {
				foreach (Axis2D axis in paneAxesData.Axes) {
					if (axis.IsSmartAxis)
						return true;
				}
			}
			return false;
		}
	}
}
