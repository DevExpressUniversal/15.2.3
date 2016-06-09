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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class XYSeries2D : XYSeries, IXYSeriesView {
		public static readonly DependencyProperty CrosshairEnabledProperty = DependencyPropertyManager.Register("CrosshairEnabled",
			typeof(bool?), typeof(XYSeries2D), new PropertyMetadata(null, CrosshairEnabledPropertyChanged));
		public static readonly DependencyProperty CrosshairLabelVisibilityProperty = DependencyPropertyManager.Register("CrosshairLabelVisibility",
			typeof(bool?), typeof(XYSeries2D), new PropertyMetadata(null));
		public static readonly DependencyProperty CrosshairLabelPatternProperty = DependencyPropertyManager.Register("CrosshairLabelPattern",
			typeof(string), typeof(XYSeries2D), new PropertyMetadata(""));
		public static readonly DependencyProperty CrosshairLabelTemplateProperty = DependencyPropertyManager.Register("CrosshairLabelTemplate",
			typeof(DataTemplate), typeof(XYSeries2D), new PropertyMetadata(null));
		static readonly DependencyPropertyKey IndicatorsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Indicators",
		   typeof(IndicatorCollection), typeof(XYSeries2D), new PropertyMetadata());
		public static readonly DependencyProperty IndicatorsProperty = IndicatorsPropertyKey.DependencyProperty;
		static void CrosshairEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYSeries2D series = d as XYSeries2D;
			if (series != null)
				ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "CrosshairEnabled"));
		}
		int paneIndex = -1;
		int axisXIndex = -1;
		int axisYIndex = -1;
		List<SeriesPointItem> highlightedInvisibleMarkerItems = new List<SeriesPointItem>();
		List<SeriesPointItem> highlightedPointItems = new List<SeriesPointItem>();
		protected abstract int PixelsPerArgument { get; }
		protected virtual bool HasInvisibleMarkers { get { return false; } }
		protected override bool NeedFilterVisiblePoints { get { return true; } }
		protected override bool Is3DView { get { return false; } }
		internal bool ActualCrosshairLabelVisible {
			get {
				if (CrosshairLabelVisibility.HasValue)
					return CrosshairLabelVisibility.Value;
				return Chart != null ? Chart.ActualCrosshairOptions.ShowCrosshairLabels : false;
			}
		}
		internal AxisX2D ActualAxisX {
			get {
				XYDiagram2D diagram = Diagram as XYDiagram2D;
				if (diagram == null || SupportedDiagramType != typeof(XYDiagram2D))
					return null;
				SecondaryAxisX2D axis = AxisXInternal;
				return diagram.SecondaryAxesXInternal.Contains(axis) ? axis : diagram.ActualAxisX;
			}
		}
		internal AxisBase ActualAxisY {
			get {
				XYDiagram2D diagram = Diagram as XYDiagram2D;
				if (diagram == null || SupportedDiagramType != typeof(XYDiagram2D))
					return null;
				SecondaryAxisY2D axis = AxisYInternal;
				return diagram.SecondaryAxesYInternal.Contains(axis) ? axis : diagram.ActualAxisY;
			}
		}
		protected virtual IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) { yield return ((IXYPoint)refinedPoint).Value; }
		protected internal override bool IsAxisXReversed {
			get {
				IAxisData axisX = ActualAxisX;
				return axisX != null && axisX.Reverse;
			}
		}
		protected internal override bool IsAxisYReversed {
			get {
				IAxisData axisY = ActualAxisY;
				return axisY != null && axisY.Reverse;
			}
		}
		protected internal override bool ActualCrosshairEnabled {
			get {
				if (Chart != null && Chart.CrosshairSupported) {
					if (CrosshairEnabled.HasValue)
						return CrosshairEnabled.Value;
					return Chart.CrosshairEnabled.HasValue ? Chart.CrosshairEnabled.Value : true;
				}
				return false;
			}
		}
		protected internal override Brush AdditionalGeometrySelectionOpacityMask { get { return VisualSelectionHelper.InvertedSelectionOpacityMask; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeries2DCrosshairEnabled"),
#endif
		Category(Categories.Behavior),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool? CrosshairEnabled {
			get { return (bool?)GetValue(CrosshairEnabledProperty); }
			set { SetValue(CrosshairEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeries2DCrosshairLabelVisibility"),
#endif
		Category(Categories.Behavior),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool? CrosshairLabelVisibility {
			get { return (bool?)GetValue(CrosshairLabelVisibilityProperty); }
			set { SetValue(CrosshairLabelVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeries2DCrosshairLabelPattern"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public string CrosshairLabelPattern {
			get { return (string)GetValue(CrosshairLabelPatternProperty); }
			set { SetValue(CrosshairLabelPatternProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeries2DCrosshairLabelTemplate"),
#endif
		Category(Categories.Appearance)
		]
		public DataTemplate CrosshairLabelTemplate {
			get { return (DataTemplate)GetValue(CrosshairLabelTemplateProperty); }
			set { SetValue(CrosshairLabelTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeries2DIndicators"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public IndicatorCollection Indicators { get { return (IndicatorCollection)GetValue(IndicatorsProperty); } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public int PaneIndex {
			get {
				Pane pane = XYDiagram2D.GetSeriesPane(this);
				XYDiagram2D xyDiagram = Diagram as XYDiagram2D;
				if (pane != null && xyDiagram != null)
					return xyDiagram.Panes.IndexOf(pane);
				return -1;
			}
			set { paneIndex = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public int AxisXIndex {
			get {
				SecondaryAxisX2D axis = AxisXInternal;
				XYDiagram2D xyDiagram = Diagram as XYDiagram2D;
				if (axis != null && xyDiagram != null)
					return xyDiagram.SecondaryAxesXInternal.IndexOf(axis);
				return -1;
			}
			set { axisXIndex = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public int AxisYIndex {
			get {
				SecondaryAxisY2D axis = AxisYInternal;
				XYDiagram2D xyDiagram = Diagram as XYDiagram2D;
				if (axis != null && xyDiagram != null)
					return xyDiagram.SecondaryAxesYInternal.IndexOf(axis);
				return -1;
			}
			set { axisYIndex = value; }
		}
		public XYSeries2D() {
			this.SetValue(IndicatorsPropertyKey, ChartElementHelper.CreateInstance<IndicatorCollection>(this));
		}
		#region IXYSeriesView
		IAxisData IXYSeriesView.AxisXData { get { return (IAxisData)ActualAxisX; } }
		IAxisData IXYSeriesView.AxisYData { get { return (IAxisData)ActualAxisY; } }
		ToolTipPointDataToStringConverter IXYSeriesView.CrosshairConverter { get { return ToolTipPointValuesConverter; } }
		string IXYSeriesView.CrosshairLabelPattern { get { return CrosshairLabelPattern; } }
		IPane IXYSeriesView.Pane { get { return ActualPane; } }
		bool IXYSeriesView.SideMarginsEnabled { get { return true; } }
		int IXYSeriesView.PixelsPerArgument { get { return PixelsPerArgument; } }
		bool IXYSeriesView.CrosshairEnabled { get { return ActualCrosshairEnabled; } }
		IEnumerable<double> IXYSeriesView.GetCrosshairValues(RefinedPoint refinedPoint) {
			return GetCrosshairValues(refinedPoint);
		}
		List<ISeparatePaneIndicator> IXYSeriesView.GetSeparatePaneIndicators() {
			var separatePaneIndicators = new List<ISeparatePaneIndicator>();
			foreach (Indicator indicator in Indicators) {
				ISeparatePaneIndicator separatePaneIndicator = indicator as ISeparatePaneIndicator;
				if (separatePaneIndicator != null)
					separatePaneIndicators.Add(separatePaneIndicator);
			}
			return separatePaneIndicators;
		}
		List<IAffectsAxisRange> IXYSeriesView.GetIndicatorsAffectRange() {
			var separatePaneIndicators = new List<IAffectsAxisRange>();
			foreach (Indicator indicator in Indicators) {
				var separatePaneIndicator = indicator as IAffectsAxisRange;
				if (separatePaneIndicator != null && indicator.Visible)
					separatePaneIndicators.Add(separatePaneIndicator);
			}
			return separatePaneIndicators;
		}
		#endregion
		void UpdateAdditionalGeometryClip() {
			Pane pane = ActualPane as Pane;
			AdditionalLineSeriesGeometry additionalGeometry = AdditionalGeometry;
			if (pane != null && additionalGeometry != null) {
				Rect viewport = pane.Viewport;
				Rect clipBounds = new Rect(0, 0, viewport.Width, viewport.Height);
				GeneralTransform transform = pane.ViewportTransform.Inverse;
				if (transform != null) {
					IUnwindAnimation unwindAnimation = GetActualSeriesAnimation() as IUnwindAnimation;
					if (unwindAnimation != null)
						clipBounds = unwindAnimation.CreateAnimatedClipBounds(clipBounds, SeriesProgress.ActualProgress);
					clipBounds = transform.TransformBounds(clipBounds);
				}
				additionalGeometry.Clip = new RectangleGeometry() { Rect = clipBounds };
			}
		}
		void SetPointStateCore(ISeriesPoint point, bool isHighlighted) {
			List<SeriesPointItem> pointItems = Item.GetPointItems(point);
			if (pointItems != null)
				foreach (SeriesPointItem pointItem in pointItems) {
					pointItem.IsHighlighted = isHighlighted;
					SetInvisibleMarkerHighlighting(pointItem, isHighlighted);
					if (isHighlighted)
						highlightedPointItems.Add(pointItem);
					else
						highlightedPointItems.Remove(pointItem);
				};
		}
		void SetInvisibleMarkerHighlighting(SeriesPointItem pointItem, bool isHighlighted) {
			if (HasInvisibleMarkers && IsPointItemHidden(pointItem)) {
				if (isHighlighted) {
					pointItem.Layout = CreatePointItemLayout(pointItem);
					pointItem.Model = GetModel(pointItem.ValueLevel);
					Item.VisiblePointItems.Add(pointItem);
					highlightedInvisibleMarkerItems.Add(pointItem);
				}
				else if (highlightedInvisibleMarkerItems.Contains(pointItem)) {
					Item.VisiblePointItems.Remove(pointItem);
					pointItem.PointItemPresentation = null;
					highlightedInvisibleMarkerItems.Remove(pointItem);
				}
			}
		}
		protected internal abstract Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel);
		protected internal new Pane ActualPane {
			get {
				XYDiagram2D diagram = Diagram as XYDiagram2D;
				if (diagram == null)
					return null;
				Pane pane = XYDiagram2D.GetSeriesPane(this);
				return diagram.Panes.Contains(pane) ? pane : diagram.ActualDefaultPane;
			}
		}
		protected internal virtual ResolveOverlappingMode LabelsResolveOverlappingMode {
			get {
				SeriesLabel label = ActualLabel;
				return label == null ? ResolveOverlappingMode.None : label.ResolveOverlappingMode;
			}
		}
		protected internal virtual bool IsPointItemHidden(SeriesPointItem pointItem) {
			return false;
		}
		protected abstract SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem);
		protected abstract XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform);
		protected abstract SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem);
		protected bool CalculateLabelItemVisible(PaneMapping mapping, Point initialAnchorPoint) {
			Point roundedInitialPoint = new Point(MathUtils.StrongRound(initialAnchorPoint.X), MathUtils.StrongRound(initialAnchorPoint.Y));
			return mapping.Viewport.Contains(roundedInitialPoint);
		}
		internal double NormalizeLabelAngle(double angle) {
			double actualAngle = -angle;
			Pane pane = ActualPane as Pane;
			IAxisData actualAxisX = ActualAxisX;
			IAxisData actualAxisY = ActualAxisY;
			if (pane == null || actualAxisX == null || actualAxisY == null)
				return MathUtils.NormalizeDegree(actualAngle);
			if (actualAxisX.Reverse)
				actualAngle = 180 - actualAngle;
			if (actualAxisY.Reverse)
				actualAngle = -actualAngle;
			return MathUtils.NormalizeDegree(pane.Rotated ? -90 - actualAngle : actualAngle);
		}
		internal SeriesPointLayout CreatePointItemLayout(SeriesPointItem pointItem) {
			PaneMapping mapping = new PaneMapping((Pane)ActualPane, this);
			return CreatePointItemLayout(mapping, pointItem);
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, bool inLabel) {
			Pane pane = (Pane)ActualPane;
			Transform pointTransform = pane.ViewportRenderTransform;
			PaneMapping mapping = new PaneMapping(pane, this);
			Point toolTipPoint = CalculateToolTipPoint(pointItem, mapping, pointTransform, inLabel);
			Rect paneRect = LayoutHelper.GetRelativeElementRect(pane, Diagram.ChartControl);
			return new Point(toolTipPoint.X + paneRect.Left, toolTipPoint.Y + paneRect.Top);
		}
		protected override bool ProcessChanging(ChartUpdate updateInfo) {
			return base.ProcessChanging(updateInfo) && (updateInfo.Change & ChartElementChange.Diagram3DOnly) == 0;
		}
		protected override bool IsCompatible(Series series) {
			XYSeries2D xySeries = series as XYSeries2D;
			return base.IsCompatible(series) || xySeries != null && ActualAxisX.Equals(xySeries.ActualAxisX);
		}
		protected internal virtual AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return null;
		}
		protected internal override IMapping CreateDiagramMapping() {
			Pane pane = ActualPane as Pane;
			return pane != null ? new PaneMapping(pane, this) : null;
		}
		protected internal override CrosshairMarkerItem CreateCrosshairMarkerItem(IRefinedSeries refinedSeries, RefinedPoint refinedPoint) {
			Color color = Item.DrawOptions.Color;
			if (refinedPoint != null) {
				SeriesPointData pointData = Item.GetPointData(refinedSeries, refinedPoint);
				if (pointData != null) {
					Color? crosshairColor = pointData.GetCrosshairColor(ActualColorEach);
					if (crosshairColor.HasValue)
						color = crosshairColor.Value;
				}
			}
			color.A = (byte)(color.A * GetOpacity());
			SolidColorBrush brush = new SolidColorBrush(color);
			return new CrosshairMarkerItem(this, brush, GetPenBrush(brush), LegendMarkerLineStyle);
		}
		protected override void SeriesProgressChanged() {
			base.SeriesProgressChanged();
			SeriesAnimationBase animation = GetActualSeriesAnimation();
			if (animation == null || animation.ShouldAnimateAdditionalGeometry)
				UpdateAdditionalGeometry();
			if (animation == null || animation.ShouldAnimateClipBounds)
				UpdateAdditionalGeometryClip();
			Pane pane = ActualPane as Pane;
		}
		internal void CreateSeriesPointsLayout() {
			PaneMapping mapping = new PaneMapping((Pane)ActualPane, this);
			foreach (SeriesPointItem pointItem in Item.AllPointItems) {
				SeriesPointLayout layout = CreateSeriesPointLayout(mapping, pointItem);
				if (!(highlightedInvisibleMarkerItems.Contains(pointItem) && layout == null))
					pointItem.Layout = layout;
			}
			UpdateAdditionalGeometry();
			UpdateAdditionalGeometryClip();
		}
		internal void CreateSeriesLabelsLayout() {
			Pane pane = (Pane)ActualPane;
			PaneMapping mapping = new PaneMapping(pane, this);
			Transform labelTransform = pane.ViewportRenderTransform;
			foreach (SeriesLabelItem labelItem in ActualLabel.Items)
				labelItem.Layout = CreateSeriesLabelLayout(labelItem, mapping, labelTransform);
		}
		internal void CreateSeriesLabelConnectorsLayout() {
			if (ActualLabel.Items != null) {
				foreach (SeriesLabelItem labelItem in ActualLabel.Items)
					labelItem.UpdateConnectorItemLayout();
			}
		}
		internal void UpdateSeriesLabelsClipBounds() {
			Pane pane = (Pane)ActualPane;
			Rect viewport = pane.Viewport;
			bool navigationEnabled = pane.NavigationEnabled;
			foreach (SeriesLabelItem labelItem in ActualLabel.Items) {
				SeriesLabel2DLayout layout = labelItem.Layout;
				if (layout != null)
					if (layout.Visible && navigationEnabled) {
						Rect labelBounds = layout.Bounds;
						Rect clipRect = labelBounds;
						clipRect.Intersect(viewport);
						layout.ClipBounds = clipRect.IsEmpty ? RectExtensions.Zero : new Rect(clipRect.X - labelBounds.X, clipRect.Y - labelBounds.Y, clipRect.Width, clipRect.Height);
					}
					else
						layout.ClipBounds = Rect.Empty;
				SeriesLabelConnectorItem connectorItem = labelItem.ConnectorItem;
				if (connectorItem != null) {
					SeriesLabel2DConnectorLayout connectorLayout = connectorItem.Layout;
					if (connectorLayout != null)
						connectorLayout.ClipBounds = navigationEnabled ? viewport : Rect.Empty;
				}
			}
		}
		internal void SetPointState(ISeriesPoint point, RefinedPoint refinedPoint, bool isHighlighted) {
			ISeriesPoint sourcePoint = SeriesPoint.GetSourcePoints(point)[0];
			if (ShouldCalculatePointsData)
				SetPointStateCore(sourcePoint, isHighlighted);
			else {
				if (isHighlighted && refinedPoint != null) {
					SeriesPointData pointData = Item.CreatePointData(refinedPoint);
					Item.AddPointItems(sourcePoint, pointData.PointItems);
					SetPointStateCore(sourcePoint, isHighlighted);
				}
				else {
					SetPointStateCore(sourcePoint, isHighlighted);
					Item.RemovePointItems(sourcePoint);
				}
			}
		}
		internal void ResetHighlightedItems() {
			foreach (SeriesPointItem pointItem in highlightedPointItems) {
				pointItem.IsHighlighted = false;
				SetInvisibleMarkerHighlighting(pointItem, false);
			}
			highlightedPointItems.Clear();
			highlightedInvisibleMarkerItems.Clear();
		}
		internal void CompleteDeserializing() {
			XYDiagram2D diagram = Diagram as XYDiagram2D;
			if (diagram != null) {
				if (paneIndex >= 0 && paneIndex < diagram.Panes.Count)
					XYDiagram2D.SetSeriesPane(this, diagram.Panes[paneIndex]);
				if (axisXIndex >= 0 && axisXIndex < diagram.SecondaryAxesXInternal.Count)
					XYDiagram2D.SetSeriesAxisX(this, diagram.SecondaryAxesXInternal[axisXIndex]);
				if (axisYIndex >= 0 && axisYIndex < diagram.SecondaryAxesYInternal.Count)
					XYDiagram2D.SetSeriesAxisY(this, diagram.SecondaryAxesYInternal[axisYIndex]);
				foreach (Indicator indicator in Indicators)
					indicator.CompleteDeserializing();
			}
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			XYSeries2D xySeries = series as XYSeries2D;
			if (xySeries != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, xySeries, CrosshairEnabledProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, xySeries, CrosshairLabelPatternProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, xySeries, CrosshairLabelTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, xySeries, CrosshairLabelVisibilityProperty);
				if (xySeries.Indicators != null && xySeries.Indicators.Count > 0) {
					Indicators.Clear();
					foreach (Indicator indicator in xySeries.Indicators)
						Indicators.Add(indicator.CloneForBinding());
				}
			}
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			return GetXYDataProvider(patternConstant);
		}
	}
}
