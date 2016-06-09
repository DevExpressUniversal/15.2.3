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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[ContentProperty("Series")]
	public abstract class Diagram : ChartElement, IDiagram, IInteractiveElement {
		static readonly DependencyPropertyKey SeriesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Series",
			typeof(SeriesCollection), typeof(Diagram), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		static readonly DependencyPropertyKey CrosshairAxisLabelItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("CrosshairAxisLabelItems",
			typeof(ObservableCollection<CrosshairAxisLabelItem>), typeof(Diagram), new PropertyMetadata(null));
		static readonly DependencyPropertyKey CrosshairSeriesLabelItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("CrosshairSeriesLabelItems",
			typeof(ObservableCollection<CrosshairSeriesLabelItem>), typeof(Diagram), new PropertyMetadata(null));
		public static readonly DependencyProperty SeriesProperty = SeriesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SeriesTemplateProperty = DependencyPropertyManager.Register("SeriesTemplate",
			typeof(Series), typeof(Diagram), new PropertyMetadata(null, SeriesTemplateChanged));
		public static readonly DependencyProperty SeriesDataMemberProperty = DependencyPropertyManager.Register("SeriesDataMember",
			typeof(string), typeof(Diagram), new PropertyMetadata(String.Empty, SeriesDataMemberChanged));
		public static readonly DependencyProperty CrosshairAxisLabelItemsProperty = CrosshairAxisLabelItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty CrosshairSeriesLabelItemsProperty = CrosshairSeriesLabelItemsPropertyKey.DependencyProperty;
		static Diagram() {
			Type ownerType = typeof(Diagram);
			PaddingProperty.OverrideMetadata(ownerType,
				new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure, ChartElementHelper.UpdateWithClearDiagramCache));
			MarginProperty.OverrideMetadata(ownerType,
				new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsParentMeasure, ChartElementHelper.UpdateWithClearDiagramCache));
			BorderThicknessProperty.OverrideMetadata(ownerType,
				new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0),
					FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsMeasure, ChartElementHelper.UpdateWithClearDiagramCache));
		}
		static void SeriesTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateAutoSeries);
		}
		static void SeriesDataMemberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateAutoSeries);
		}
		readonly ViewController viewController;
		SelectionInfo selectionInfo;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<CrosshairAxisLabelItem> CrosshairAxisLabelItems { get { return (ObservableCollection<CrosshairAxisLabelItem>)GetValue(CrosshairAxisLabelItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<CrosshairSeriesLabelItem> CrosshairSeriesLabelItems { get { return (ObservableCollection<CrosshairSeriesLabelItem>)GetValue(CrosshairSeriesLabelItemsProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DiagramSeries"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public SeriesCollection Series { get { return (SeriesCollection)GetValue(SeriesProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DiagramSeriesTemplate"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Series SeriesTemplate {
			get { return (Series)GetValue(SeriesTemplateProperty); }
			set { SetValue(SeriesTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DiagramSeriesDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string SeriesDataMember {
			get { return (string)GetValue(SeriesDataMemberProperty); }
			set { SetValue(SeriesDataMemberProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		bool CanGetAutocreatedSeries {
			get {
				if (SeriesTemplate == null || SeriesTemplate.GetDataSource() == null ||
					String.IsNullOrEmpty(SeriesTemplate.ArgumentDataMember) ||
					!SeriesTemplate.AreValueDataMembersSetted())
					return false;
				return true;
			}
		}
		internal ViewController ViewController { get { return viewController; } }
		internal ChartControl ChartControl { get { return ((IChartElement)this).Owner as ChartControl; } }
		protected internal bool DiagramLayoutChanged { get; set; }
		protected internal virtual bool IsKeyboardNavigationEnabled { get { return false; } }
		protected internal virtual bool IsMouseNavigationEnabled { get { return false; } }
		protected internal virtual bool IsManipulationNavigationEnabled { get { return false; } }
		protected internal abstract Rect ActualViewport { get; }
		protected abstract bool Is3DView { get; }
		protected internal abstract CompatibleViewType CompatibleViewType { get; }
		protected override ViewController Controller { get { return viewController; } }
		#region IDiagram Implementation
		GRealRect2D IDiagram.ChartBounds { get { return ChartControl != null ? new GRealRect2D(0, 0, ChartControl.RenderSize.Width, ChartControl.RenderSize.Height) : GRealRect2D.Empty; } }
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return selectionInfo.IsHighlighted; }
			set { selectionInfo.IsHighlighted = value; }
		}
		bool IInteractiveElement.IsSelected {
			get { return selectionInfo.IsSelected; }
			set { selectionInfo.IsSelected = value; }
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform { get { return base.RenderTransform; } set { base.RenderTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin { get { return base.RenderTransformOrigin; } set { base.RenderTransformOrigin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		public Diagram() {
			viewController = new ViewController(this);
			BeginInit();
			this.SetValue(SeriesPropertyKey, ChartElementHelper.CreateInstance<SeriesCollection>(this));
			this.SetValue(CrosshairAxisLabelItemsPropertyKey, new ObservableCollection<CrosshairAxisLabelItem>());
			this.SetValue(CrosshairSeriesLabelItemsPropertyKey, new ObservableCollection<CrosshairSeriesLabelItem>());
			selectionInfo = new SelectionInfo();
		}
		bool UpdateAutoSeries(bool raiseEvent) {
			bool bindingWasChanged = false;
			SeriesCollection series = Series;
			series.BeginInit();
			series.UnlockAutoSeries();
			try {
				while (series.Count > 0 && series[0].IsAutoSeries) {
					bindingWasChanged = true;
					series.RemoveAt(0);
				}
				ChartControl chart = ChartControl;
				Series seriesTemplate = SeriesTemplate;
				string seriesDataMember = SeriesDataMember;
				if (chart != null && seriesTemplate != null && !String.IsNullOrEmpty(seriesDataMember)) {
					object dataSource = chart.ActualDataSource;
					if (dataSource != null && seriesTemplate.Visible && CanGetAutocreatedSeries) {
						seriesTemplate.UpdateDataFilters();
						SimpleChartBindingProcedure bindingProcedure = new SimpleChartBindingProcedure(null, dataSource, String.Empty);
						IList<ISeries> autoCreatedSeries = bindingProcedure.CreateBindingSeries(seriesTemplate, seriesDataMember);
						int index = 0;
						foreach (Series newSeries in autoCreatedSeries)
							if (newSeries.Points.Count > 0) {
								bindingWasChanged = true;
								series.Insert(index++, newSeries);
								newSeries.CompleteBinding();
							}
					}
				}
			}
			finally {
				series.LockAutoSeries();
				series.EndInit(false);
			}
			if (bindingWasChanged && raiseEvent)
				RaiseBoundDataChangedEvent();
			return bindingWasChanged;
		}
		void UpdateAutoSeriesProperties() {
			Series seriesTemplate = SeriesTemplate;
			foreach (Series series in Series) {
				if (!series.IsAutoSeries)
					return;
				series.CopyPropertiesForBinding(seriesTemplate);
			}
		}
		internal void RaiseBoundDataChangedEvent() {
			ChartControl chart = ChartControl;
			if (chart != null)
				chart.RaiseBoundDataChangedEvent();
		}
		protected int GetPointIndex(int pointIndex, ISeriesPointData pointData) {
			return (pointData != null) ? pointData.IndexInSeries : pointIndex;
		}
		internal void Update(bool ShouldUpdatePanesItems, bool ShouldUpdateXYDiagram2DItems) {
			ChartControl chart = ChartControl;
			EnsureSeriesItems();
			UpdateSeriesItems();
			if (chart != null) {
				Legend legend = chart.Legend;
				if (legend != null) {
					List<LegendItem> legendItems = CreateLegendItems();
					foreach (LegendItem item in legendItems)
						item.Legend = legend;
					legend.Items = legendItems;
				}
			}
			if (ShouldUpdateXYDiagram2DItems) {
				UpdateVisualItems();
				UpdateLogicalElements();
			}
			else if (ShouldUpdatePanesItems)
				UpdateVisualItems();
		}
		internal void UpdateBinding() {
			ViewController.BeginUpdateData();
			SeriesCollection collection = Series;
			if (collection != null) {
				bool bindingWasChanged = UpdateAutoSeries(false);
				foreach (Series series in collection)
					if (!series.IsAutoSeries && series.UpdateSeriesBinding(false))
						bindingWasChanged = true;
				if (bindingWasChanged)
					RaiseBoundDataChangedEvent();
			}
			ViewController.EndUpdateData();
		}
		internal bool TryUpdatePalette() {
			foreach (Series series in Series) {
				ISupportPalette paletteColorizer = series.Colorizer as ISupportPalette;
				if (paletteColorizer != null && paletteColorizer.GetPalette() == null) {
					UpdateBinding();
					return true;
				}
			}
			return false;
		}
		protected internal virtual List<VisibilityLayoutRegion> GetElementsForAutoLayout(Size size) { return new List<VisibilityLayoutRegion>(); }
		protected virtual void EnsureSeriesItems() {
		}
		protected internal virtual void UpdateSeriesItems() {
			foreach (Series series in Series) {
				ISeriesItem seriesItem = GetSeriesItem(series);
				if (seriesItem.ShouldUpdate) {
					IRefinedSeries refinedSeries = ViewController.GetRefinedSeries(series);
					seriesItem.Update(refinedSeries);
				}
			}
			ChartControl chart = ChartControl;
			if (chart != null) {
				RoutedEvent customDrawSeriesEvent = ChartControl.CustomDrawSeriesEvent;
				foreach (IRefinedSeries refinedSeries in ViewController.RefinedSeriesForLegend) {
					Series series = (Series)refinedSeries.Series;
					ISeriesItem seriesItem = GetSeriesItem(series);
					DrawOptions drawOptions = new DrawOptions(series);
					seriesItem.DrawOptions = drawOptions;
					CustomDrawSeriesEventArgs args = new CustomDrawSeriesEventArgs(customDrawSeriesEvent, drawOptions, series.GetSeriesLegendText(), series);
					chart.RaiseEvent(args);
					seriesItem.LegendText = args.LegendText;
					Color seriesColor = drawOptions.Color;
					double opacity = series.GetOpacity();
					int pointIndex = 0;
					DrawOptions pointDrawOptions = null;
					series.UpdatePointsSelection();
					foreach (ISeriesPointData pointData in seriesItem.PointData) {
						RefinedPoint refinedPoint = pointData.RefinedPoint;
						if (refinedPoint != null) {
							SeriesPoint point = SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint);
							if (point != null) {
								Color pointColor = point.Brush != null ? point.Brush.Color : series.GetPointOriginalColorForCustomDraw(refinedSeries, GetPointIndex(pointIndex, pointData), seriesColor);
								if (chart.ShouldRaiseCustomDrawSeriesPointEvent) {
									if (pointDrawOptions != drawOptions)
										pointDrawOptions = (DrawOptions)drawOptions.Clone();
									pointDrawOptions.Color = pointColor;
									CustomDrawSeriesPointEventArgs pointArgs = new CustomDrawSeriesPointEventArgs(ChartControl.CustomDrawSeriesPointEvent,
										pointDrawOptions, series.GetPointLegendText(refinedPoint), series, series.GetLabelsTexts(refinedPoint), point);
									chart.RaiseEvent(pointArgs);
									pointData.Color = pointDrawOptions.Color;
									pointData.LegendText = pointArgs.LegendText;
									pointData.LabelsTexts = pointArgs.LabelsTexts;
								}
								else {
									pointData.Color = pointColor;
									pointData.LegendText = series.GetPointLegendText(refinedPoint);
									pointData.LabelsTexts = series.GetLabelsTexts(refinedPoint);
								}
								pointData.Opacity = opacity;
							}
							pointIndex++;
						}
					}
				}
			}
		}
		protected virtual List<LegendItem> CreateLegendItems() {
			List<LegendItem> legendItems = new List<LegendItem>();
			foreach (IRefinedSeries refinedSeries in ViewController.RefinedSeriesForLegend) {
				Series series = (Series)refinedSeries.Series;
				series.UpdateLegendItems(legendItems, GetSeriesItem(series));
			}
			return legendItems;
		}
		protected abstract ISeriesItem GetSeriesItem(Series series);
		protected internal abstract void InvalidateDiagram();
		protected internal virtual void UpdateActualPanes(bool shouldUpdate) {
		}
		protected internal virtual void UpdateVisualItems() {
		}
		protected internal virtual void UpdateLogicalElements() {
		}
		protected internal virtual void NavigationZoomIn() {
		}
		protected internal virtual void NavigationZoomIn(Point position) {
		}
		protected internal virtual void NavigationZoomOut() {
		}
		protected internal virtual bool NavigationZoom(Point position, int delta, ZoomingKind zoomingKind, bool isDragging) {
			return false;
		}
		protected internal virtual void NavigationUndoZoom() {
		}
		protected internal virtual void ProcessMouseMove(Point chartPosition, MouseEventArgs e) {
		}
		protected internal virtual void NavigationZoomIntoRectangle(Rect zoomingRectangle) {
		}
		protected internal virtual bool NavigationScrollHorizontally(int delta, NavigationType navigationType) {
			return false;
		}
		protected internal virtual bool NavigationScrollVertically(int delta, NavigationType navigationType) {
			return false;
		}
		protected internal virtual bool NavigationBeginDrag(Point position, MouseButtonEventArgs e, bool isShiftKey) {
			return false;
		}
		protected internal virtual bool NavigationCanZoomIntoRectangle() {
			return false;
		}
		protected internal virtual void NavigationShowSelection(Point chartPosition, MouseEventArgs e) {
		}
		protected internal virtual void NavigationDrag(Point chartPosition, int dx, int dy, NavigationType navigationType, MouseEventArgs e) {
		}
		protected internal virtual bool NavigationCanZoomIn(Point chartPosition, bool useFocusedPane) {
			return false;
		}
		protected internal virtual bool NavigationCanZoomOut(Point chartPosition, bool useFocusedPane) {
			return false;
		}
		protected internal virtual bool NavigationInDiagram(Point chartPosition) {
			return false;
		}
		protected internal virtual bool NavigationCanDrag(Point chartPosition, bool useFocusedPane) {
			return false;
		}
		protected internal virtual bool ManipulationStart(Point point) {
			return false;
		}
		protected internal virtual void ManipulationZoom(double scaleX, double scaleY) {
		}
		protected internal virtual void ManipulationRotate(double degreeAngle) {
		}
		protected internal virtual Point ManipulationDrag(Point translation) {
			return new Point();
		}
		protected internal virtual void UpdateCrosshairLocation(Point? cursorLocation) {
		}
		protected internal virtual void ClearViewportCache() {
		}
		protected internal virtual void EnableCache() {
		}
		protected internal virtual void DisableCache() {
		}
		protected internal virtual void CheckMeasureUnits() {
		}
		protected override bool ProcessChanging(ChartUpdate updateInfo) {
			if (updateInfo.ShouldUpdateAutoSeries || updateInfo.ShouldUpdateAutoSeriesProperties) {
				DiagramLayoutChanged = true;
				ViewController.BeginUpdateData();
				if (updateInfo.ShouldUpdateAutoSeries)
					UpdateAutoSeries(true);
				else if (updateInfo.ShouldUpdateAutoSeriesProperties)
					UpdateAutoSeriesProperties();
				ViewController.EndUpdateData();
				return false;
			}
			return true;
		}
		protected internal virtual void ClearCache() {
			foreach (Series series in Series)
				if (series.Cache != null)
					series.Cache.Clear();
		}
		public bool ShouldSerializeCrosshairAxisLabelItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeCrosshairSeriesLabelItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public override void EndInit() {
			base.EndInit();
			ViewController.BeginUpdateData();
			UpdateAutoSeries(true);
			ViewController.EndUpdateData();
		}
	}
}
