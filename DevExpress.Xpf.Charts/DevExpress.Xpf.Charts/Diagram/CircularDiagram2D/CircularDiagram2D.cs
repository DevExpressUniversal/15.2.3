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
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum CircularDiagramRotationDirection {
		Counterclockwise,
		Clockwise
	}
	public enum CircularDiagramShapeStyle {
		Circle,
		Polygon
	}
	[
	TemplatePart(Name = "PART_Domain", Type = typeof(DomainPanel)),
	TemplatePart(Name = "PART_DomainBackground", Type = typeof(Path)),
	TemplatePart(Name = "PART_InterlaceControls", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_GridLines", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_Series", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_DomainBorder", Type = typeof(Border)),
	]
	public abstract class CircularDiagram2D : Diagram2D, IXYDiagram, IPane, IHitTestableElement {
		const double defaultAxisYLength = 400;
		const double defaultAxisXLength = 2 * defaultAxisYLength * Math.PI;
		public static readonly DependencyProperty ShapeStyleProperty = DependencyPropertyManager.Register("ShapeStyle",
			typeof(CircularDiagramShapeStyle), typeof(CircularDiagram2D), new PropertyMetadata(CircularDiagramShapeStyle.Circle, ChartElementHelper.Update));
		public static readonly DependencyProperty RotationDirectionProperty = DependencyPropertyManager.Register("RotationDirection",
			typeof(CircularDiagramRotationDirection), typeof(CircularDiagram2D), new PropertyMetadata(CircularDiagramRotationDirection.Counterclockwise, ChartElementHelper.Update));
		public static readonly DependencyProperty LabelsResolveOverlappingMinIndentProperty = DependencyPropertyManager.Register("LabelsResolveOverlappingMinIndent",
			typeof(int), typeof(CircularDiagram2D), new PropertyMetadata(-1, ChartElementHelper.Update));
		public static readonly DependencyProperty StartAngleProperty = DependencyPropertyManager.Register("StartAngle",
			typeof(double), typeof(CircularDiagram2D), new PropertyMetadata(0.0, ChartElementHelper.Update));
		public static readonly DependencyProperty DomainBrushProperty = DependencyPropertyManager.Register("DomainBrush",
			typeof(Brush), typeof(CircularDiagram2D));
		public static readonly DependencyProperty DomainBorderBrushProperty = DependencyPropertyManager.Register("DomainBorderBrush",
			typeof(Brush), typeof(CircularDiagram2D));
		public static readonly DependencyProperty DomainBorderGeometryProperty = DependencyPropertyManager.Register("DomainBorderGeometry",
			typeof(GridLineGeometry), typeof(CircularDiagram2D));
		public static readonly DependencyProperty DomainClipGeometryProperty = DependencyPropertyManager.Register("DomainClipGeometry",
			typeof(GridLineGeometry), typeof(CircularDiagram2D));
		public static readonly DependencyProperty AxisItemsProperty = DependencyPropertyManager.Register("AxisItems", typeof(ObservableCollection<object>), typeof(CircularDiagram2D));
		public static readonly DependencyProperty SeriesLabelItemsProperty = DependencyPropertyManager.Register("SeriesLabelItems", typeof(ObservableCollection<SeriesLabelItem>), typeof(CircularDiagram2D));
		public static readonly DependencyProperty ElementsProperty = DependencyPropertyManager.Register("Elements", typeof(ObservableCollection<object>), typeof(CircularDiagram2D));
		public static readonly DependencyProperty GridLinesProperty = DependencyPropertyManager.Register("GridLines", typeof(ObservableCollection<UIElement>), typeof(CircularDiagram2D));
		public static readonly DependencyProperty InterlaceControlsProperty = DependencyPropertyManager.Register("InterlaceControls", typeof(ObservableCollection<UIElement>), typeof(CircularDiagram2D));
		static readonly DependencyPropertyKey SeriesItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("SeriesItems",
			typeof(ReadOnlyObservableCollection<SeriesItem>), typeof(CircularDiagram2D), new PropertyMetadata(null));
		public static readonly DependencyProperty SeriesItemsProperty = SeriesItemsPropertyKey.DependencyProperty;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public CircularDiagramShapeStyle ShapeStyle {
			get { return (CircularDiagramShapeStyle)GetValue(ShapeStyleProperty); }
			set { SetValue(ShapeStyleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public CircularDiagramRotationDirection RotationDirection {
			get { return (CircularDiagramRotationDirection)GetValue(RotationDirectionProperty); }
			set { SetValue(RotationDirectionProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int LabelsResolveOverlappingMinIndent {
			get { return (int)GetValue(LabelsResolveOverlappingMinIndentProperty); }
			set { SetValue(LabelsResolveOverlappingMinIndentProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double StartAngle {
			get { return (double)GetValue(StartAngleProperty); }
			set { SetValue(StartAngleProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush DomainBrush {
			get { return (Brush)GetValue(DomainBrushProperty); }
			set { SetValue(DomainBrushProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush DomainBorderBrush {
			get { return (Brush)GetValue(DomainBorderBrushProperty); }
			set { SetValue(DomainBorderBrushProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<object> AxisItems {
			get { return (ObservableCollection<object>)GetValue(AxisItemsProperty); }
			set { SetValue(AxisItemsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<SeriesLabelItem> SeriesLabelItems {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(SeriesLabelItemsProperty); }
			set { SetValue(SeriesLabelItemsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<UIElement> GridLines {
			get { return (ObservableCollection<UIElement>)GetValue(GridLinesProperty); }
			set { SetValue(GridLinesProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<UIElement> InterlaceControls {
			get { return (ObservableCollection<UIElement>)GetValue(InterlaceControlsProperty); }
			set { SetValue(InterlaceControlsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<object> Elements {
			get { return (ObservableCollection<object>)GetValue(ElementsProperty); }
			set { SetValue(ElementsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyObservableCollection<SeriesItem> SeriesItems { get { return (ReadOnlyObservableCollection<SeriesItem>)GetValue(SeriesItemsProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridLineGeometry DomainBorderGeometry {
			get { return (GridLineGeometry)GetValue(DomainBorderGeometryProperty); }
			set { SetValue(DomainBorderGeometryProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridLineGeometry DomainClipGeometry {
			get { return (GridLineGeometry)GetValue(DomainClipGeometryProperty); }
			set { SetValue(DomainClipGeometryProperty, value); }
		}
		readonly List<Axis2DItem> axisItems = new List<Axis2DItem>();
		readonly Dictionary<AxisBase, GridAndTextDataEx> gridAndTextDataDictionary = new Dictionary<AxisBase, GridAndTextDataEx>();
		Rect viewport;
		UIElement visualContent;
		Point prevRenderOffset = new Point(0, 0);
		ChartControl Chart { get { return ((IChartElement)this).Owner as ChartControl; } }
		bool IsAxisLabelItemsMeasured {
			get {
				foreach (Axis2DItem axisItem in axisItems) {
					IEnumerable<AxisLabelItem> items = axisItem.LabelItems;
					if (items != null)
						foreach (AxisLabelItem labelItem in items)
							if (labelItem.Size.IsEmpty)
								return false;
				}
				return true;
			}
		}
		Rect ContentBounds { get { return visualContent != null ? new Rect(0, 0, visualContent.DesiredSize.Width, visualContent.DesiredSize.Height) : RectExtensions.Zero; } }
		Rect Viewport {
			get { return viewport; }
			set {
				if (value != viewport) {
					viewport = value;
					if (Chart != null)
						Chart.InvalidateChartElementPanel();
					InvalidateArrange();
				}
			}
		}
		internal Transform ViewportRenderTransform {
			get {
				TransformGroup transform = new TransformGroup();
				transform.Children.Add(new TranslateTransform() { X = ActualViewport.X, Y = ActualViewport.Y });
				return transform;
			}
		}
		protected internal override Rect ActualViewport { get { return Viewport; } }
		protected internal override bool IsManipulationNavigationEnabled {
			get { return ChartControl != null && ChartControl.IsManipulationEnabled; }
		}
		protected internal abstract CircularAxisX2D AxisXImpl { get; }
		protected internal abstract CircularAxisY2D AxisYImpl { get; }
		#region IXYDiagram implementation
		void IXYDiagram.UpdateCrosshairData(IList<RefinedSeries> seriesCollection) { }
		GRealRect2D IDiagram.ChartBounds {
			get { throw new NotImplementedException(); }
		}
		IAxisData IXYDiagram.AxisX { get { return AxisXImpl; } }
		IAxisData IXYDiagram.AxisY { get { return AxisYImpl; } }
		IList<IPane> IXYDiagram.Panes { get { return new IPane[] { this }; } }
		bool IXYDiagram.ScrollingEnabled { get { return false; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesX { get { return null; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesY { get { return null; } }
		bool IXYDiagram.Rotated { get { return false; } }
		ICrosshairOptions IXYDiagram.CrosshairOptions { get { return null; } }
		IList<IPane> IXYDiagram.GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync) {
			return null;
		}
		InternalCoordinates IXYDiagram.MapPointToInternal(IPane pane, GRealPoint2D point) {
			return null;
		}
		GRealPoint2D IXYDiagram.MapInternalToPoint(IPane pane, IAxisData axisX, IAxisData axisY, double argument, double value) {
			return new GRealPoint2D(0, 0);
		}		
		List<IPaneAxesContainer> IXYDiagram.GetPaneAxesContainers(IList<RefinedSeries> activeSeries) {
			PaneAxesContainer paneAxesContainer = new PaneAxesContainer(this);
			paneAxesContainer.RegisterAxisX(0, (IAxisData)AxisXImpl);
			paneAxesContainer.RegisterAxisY(0, (IAxisData)AxisYImpl);
			paneAxesContainer.InitializePrimaryAndSecondaryAxes();
			List<IPaneAxesContainer> containers = new List<IPaneAxesContainer>();
			containers.Add(paneAxesContainer);
			return containers;
		}
		void IXYDiagram.UpdateAutoMeasureUnits() {
		}
		int IXYDiagram.GetAxisXLength(IAxisData axis) {
			return 0;
		}
		#endregion
		#region IPane implementation
		int IPane.PaneIndex { get { return -1; } }
		GRealRect2D? IPane.MappingBounds { get { return null; } }
		#endregion
		#region ShouldSerialize
		public bool ShouldSerializeInterlaceControls(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeGridLines(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeElements(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeAxisItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeSeriesLabelItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeSeriesItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		#endregion
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return this; } }
		Object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
		public CircularDiagram2D() { }
		void CalculateDomainGeometry() {
			if (ShapeStyle == CircularDiagramShapeStyle.Circle) {
				DomainBorderGeometry = new GridLineGeometry(GridLineType.Ellipse, new List<Point>() { new Point(0, 0), new Point(ActualViewport.Width - 1, ActualViewport.Height - 1) });
				DomainClipGeometry = new GridLineGeometry(GridLineType.Ellipse, new List<Point>() { new Point(-1, -1), new Point(ActualViewport.Width + 1, ActualViewport.Height + 1) });
			}
			else {
				CircularDiagramMapping mapping = new CircularDiagramMapping(this);
				DomainBorderGeometry = new GridLineGeometry(GridLineType.Polyline, mapping.AxisXMapping.GetMeshPoints(ActualViewport.Width * 0.5));
				DomainClipGeometry = new GridLineGeometry(GridLineType.Polyline, mapping.AxisXMapping.GetMeshPoints(ActualViewport.Width * 0.5 + 1));
			}
		}
		bool UpdateAxis(AxisBase axis, Size size) {
			double length = Math.Min(size.Width, size.Height);
			double axisLength = axis.IsValuesAxis ? 0.5 * length : Math.PI * length;
			if (axisLength == 0)
				axisLength = axis.IsValuesAxis ? defaultAxisYLength : defaultAxisXLength;
			return axis.UpdateAutomaticMeasureUnit(axisLength);
		}
		Thickness CalculateOuterItemsLayout(Rect viewport) {
			IEnumerable<SeriesItem> seriesItemsCollection = SeriesItems;
			if (seriesItemsCollection != null)
				foreach (SeriesItem seriesItem in seriesItemsCollection) {
					CircularSeries2D series = seriesItem.Series as CircularSeries2D;
					if (series != null)
						series.CreateSeriesLabelsLayout(viewport);
				}
			Thickness axesOffsets = new Thickness(0);
			foreach (Axis2DItem axisItem in axisItems)
				CircularAxis2DElementsLayoutCalculator.CalculateLayout(axisItem, viewport, ContentBounds, RotationDirection, StartAngle, ref axesOffsets);
			return axesOffsets;
		}
		Rect CalculateOuterItemsLayoutAndViewport() {
			foreach (Axis2DItem axisItem in axisItems)
				axisItem.TitleIndent = 0.0;
			Rect viewport = GraphicsUtils.MakeRect(ContentBounds.CalcCenter(), Math.Min(ContentBounds.Width, ContentBounds.Height), Math.Min(ContentBounds.Width, ContentBounds.Height));
			Rect result;
			for (; ; ) {
				result = viewport;
				CalculateOuterItemsLayout(viewport);
				if (viewport.Width <= 0 || viewport.Height <= 0)
					return result;
				Rect actualRect = RectExtensions.Zero;
				foreach (Axis2DItem axisItem in axisItems) {
					Rect labelRect = axisItem.LabelRect;
					if (labelRect != RectExtensions.Zero)
						actualRect.Union(labelRect);
				}
				IEnumerable<SeriesItem> seriesItemsCollection = SeriesItems;
				if (seriesItemsCollection != null)
					foreach (SeriesItem seriesItem in seriesItemsCollection) {
						IEnumerable<SeriesLabelItem> labelItems = seriesItem.Series.ActualLabel.Items;
						if (labelItems != null)
							foreach (SeriesLabelItem labelItem in labelItems)
								actualRect = LayoutElementHelper.UnionRect(actualRect, labelItem);
					}
				Thickness correction = new Thickness(MathUtils.StrongRound(Math.Max(ContentBounds.Left - actualRect.Left, 0)), MathUtils.StrongRound(Math.Max(ContentBounds.Top - actualRect.Top, 0)),
													 MathUtils.StrongRound(Math.Max(actualRect.Right - ContentBounds.Right, 0)), MathUtils.StrongRound(Math.Max(actualRect.Bottom - ContentBounds.Bottom, 0)));
				if (GraphicsUtils.IsThicknessEmpty(correction))
					return result;
				double left = viewport.Left + correction.Left;
				double top = viewport.Top + correction.Top;
				viewport = new Rect(left, top, Math.Max(viewport.Right - correction.Right - left, 0), Math.Max(viewport.Bottom - correction.Bottom - top, 0));
				viewport = GraphicsUtils.MakeRect(viewport.CalcCenter(), Math.Min(viewport.Width, viewport.Height), Math.Min(viewport.Width, viewport.Height));
			}
		}
		IList<AxisBase> UpdateMeasureUnits(Size size) {
			IList<AxisBase> affectedAxes = new List<AxisBase>();
			if (UpdateAxis(AxisXImpl, size))
				affectedAxes.Add(AxisXImpl);
			if (UpdateAxis(AxisYImpl, size))
				affectedAxes.Add(AxisYImpl);
			return affectedAxes;
		}
		void CalculateLayout() {
			bool isAxisLabelItemsMeasured = IsAxisLabelItemsMeasured;
			bool shouldLayoutUpdate = UpdateAxesLabelItems();
			UpdateAxisItems();
			if (isAxisLabelItemsMeasured && !shouldLayoutUpdate) {
				Viewport = CalculateOuterItemsLayoutAndViewport();
				CalculateOuterItemsLayout(Viewport);
				foreach (Axis2DItem axisItem in axisItems)
					axisItem.UpdateGeometry(GetGridAndTextData(axisItem.Axis), Viewport);
				IList<SeriesItem> seriesItemsCollection = SeriesItems;
				if (seriesItemsCollection != null) {
					List<XYSeries> seriesCollection = new List<XYSeries>();
					foreach (SeriesItem seriesItem in seriesItemsCollection) {
						CircularSeries2D series = seriesItem.Series as CircularSeries2D;
						if (series != null)
							seriesCollection.Add(series);
					}
					ResolveOverlappingHelper.Process(seriesCollection, ContentBounds, LabelsResolveOverlappingMinIndent);
					foreach (CircularSeries2D series in seriesCollection) {
						series.CreateSeriesPointsLayout();
						series.CreateSeriesLabelConnectorsLayout();
					}
				}
				foreach (GridLinesControl gridLinesControl in GridLines) {
					GridAndTextDataEx gridAndTextData = GetGridAndTextData(gridLinesControl.Axis);
					gridLinesControl.UpdateItems(gridAndTextData != null ? gridAndTextData.GridData : null, viewport);
				}
				foreach (InterlaceControl interlaceControl in InterlaceControls) {
					IList<InterlacedData> interlacedData = new InterlacedData[0];
					AxisBase axis = interlaceControl.Axis;
					if (axis.Interlaced) {
						GridAndTextDataEx gridAndTextData = GetGridAndTextData(interlaceControl.Axis);
						if (gridAndTextData != null)
							interlacedData = gridAndTextData.GridData.InterlacedData;
					}
					interlaceControl.UpdateInterlaceItems(interlacedData, viewport);
				}
				CalculateDomainGeometry();
				CircularDiagram2DPanel panePanel = LayoutHelper.FindElement(this, element => element is CircularDiagram2DPanel) as CircularDiagram2DPanel;
				if (panePanel != null) {
					panePanel.InvalidateMeasure();
				}
			}
			else
				shouldLayoutUpdate = true;
			if (shouldLayoutUpdate && Chart != null)
				Chart.AddInvalidate(InvalidateMeasureFlags.MeasureDiagram);
		}
		internal bool UpdateAxesLabelItems() {
			gridAndTextDataDictionary.Clear();
			double axisXLength = Math.Min(ContentBounds.Width, ContentBounds.Height) * Math.PI;
			IMinMaxValues axisXRange = ((IAxisData)AxisXImpl).VisualRange;
			if (axisXLength > 0 && !Double.IsNaN(axisXRange.Min) && !Double.IsNaN(axisXRange.Max))
				gridAndTextDataDictionary.Add(AxisXImpl, new GridAndTextDataEx(AxisXImpl.GetSeries(), AxisXImpl, false, axisXRange, axisXRange, axisXLength, false));
			double axisYLength = Math.Min(ContentBounds.Width, ContentBounds.Height) * 0.5;
			IMinMaxValues axisYRange = ((IAxisData)AxisYImpl).VisualRange;
			if (axisYLength > 0 && !Double.IsNaN(axisYRange.Min) && !Double.IsNaN(axisYRange.Max))
				gridAndTextDataDictionary.Add(AxisYImpl, new GridAndTextDataEx(AxisYImpl.GetSeries(), AxisYImpl, false, axisYRange, axisYRange, axisYLength, false));
			bool isAxesLabelItemsUpdated = false;
			foreach (Axis2DItem item in axisItems) {
				GridAndTextDataEx itemGridAndTextData = GetGridAndTextData(item.Axis);
				if (itemGridAndTextData != null)
					isAxesLabelItemsUpdated |= item.UpdateLabelItems(itemGridAndTextData.TextData);
			}
			return isAxesLabelItemsUpdated;
		}
		internal List<Axis2DItem> GetAxisItems() {
			return axisItems;
		}
		protected override void EnsureSeriesItems() {
			ObservableCollection<SeriesItem> collection = new ObservableCollection<SeriesItem>();
			foreach (RefinedSeries refinedSeries in ViewController.ActiveRefinedSeries) {
				CircularSeries2D circularSeries = refinedSeries.Series as CircularSeries2D;
				if (circularSeries != null)
					collection.Add(circularSeries.Item);
			}
			int count = collection.Count;
			ReadOnlyObservableCollection<SeriesItem> actualSeriesItems = SeriesItems;
			if (actualSeriesItems == null || actualSeriesItems.Count != count)
				this.SetValue(SeriesItemsPropertyKey, new ReadOnlyObservableCollection<SeriesItem>(collection));
			else
				for (int i = 0; i < count; i++)
					if (!Object.ReferenceEquals(actualSeriesItems[i], collection[i])) {
						this.SetValue(SeriesItemsPropertyKey, new ReadOnlyObservableCollection<SeriesItem>(collection));
						break;
					}
		}
		protected internal override void UpdateSeriesItems() {
			base.UpdateSeriesItems();
			ReadOnlyCollection<SeriesItem> seriesItemsCollection = SeriesItems;
			if (seriesItemsCollection != null) {
				ObservableCollection<SeriesLabelItem> seriesLabelItems = new ObservableCollection<SeriesLabelItem>();
				foreach (SeriesItem seriesItem in seriesItemsCollection) {
					SeriesLabel label = seriesItem.Series.ActualLabel;
					if (label != null) {
						IEnumerable<SeriesLabelItem> items = label.Items;
						if (items != null)
							foreach (SeriesLabelItem labelItem in items)
								seriesLabelItems.Add(labelItem);
					}
				}
				SeriesLabelItems = seriesLabelItems;
			}
		}
		internal GridAndTextDataEx GetGridAndTextData(AxisBase axis) {
			GridAndTextDataEx item;
			return gridAndTextDataDictionary.TryGetValue(axis, out item) ? item : null;
		}
		void UpdateAxisItems() {
			ObservableCollection<object> items = new ObservableCollection<object>();
			ObservableCollection<object> selectionItems = new ObservableCollection<object>();
			foreach (Axis2DItem axisItem in axisItems) {
				items.Add(axisItem);
				IEnumerable<AxisLabelItem> labelItems = axisItem.LabelItems;
				if (labelItems != null)
					foreach (AxisLabelItem labelItem in labelItems)
						items.Add(labelItem);
			}
			foreach (Axis2DItem axisItem in axisItems)
				items.Add(axisItem.SelectionGeometryItem);
			AxisItems = items;
		}
		protected internal override void UpdateVisualItems() {
			axisItems.Clear();
			ObservableCollection<UIElement> gridLines = new ObservableCollection<UIElement>();
			ObservableCollection<UIElement> interlaceControls = new ObservableCollection<UIElement>();
			axisItems.Add(new Axis2DItem(AxisXImpl, null));
			gridLines.Add(new GridLinesControl(AxisXImpl));
			interlaceControls.Add(new InterlaceControl(AxisXImpl));
			axisItems.Add(new Axis2DItem(AxisYImpl, null));
			gridLines.Add(new GridLinesControl(AxisYImpl));
			interlaceControls.Add(new InterlaceControl(AxisYImpl));
			GridLines = gridLines;
			InterlaceControls = interlaceControls;
			UpdateAxisItems();
		}
		protected internal override void UpdateLogicalElements() {
			ObservableCollection<object> elements = new ObservableCollection<object>();
			if (SeriesTemplate != null) {
				elements.Add(SeriesTemplate);
				elements.Add(SeriesTemplate.ActualLabel);
			}
			foreach (Series series in Series)
				elements.Add(series.ActualLabel);
			elements.Add(AxisXImpl);
			elements.Add(AxisXImpl.ActualLabel);
			elements.Add(AxisYImpl);
			elements.Add(AxisYImpl.ActualLabel);
			Elements = elements;
		}
		protected internal override bool ManipulationStart(Point pt) {
			return IsManipulationNavigationEnabled;
		}
		protected internal override void CheckMeasureUnits() {
			Size size = new Size(ActualViewport.Width, ActualViewport.Height);
			IList<AxisBase> affectedAxes = UpdateMeasureUnits(size);
			ViewController.SendDataAgreggationUpdates(affectedAxes);
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Elements == null) {
				UpdateVisualItems();
				UpdateLogicalElements();
			}
			else if (AxisItems == null)
				UpdateVisualItems();
			availableSize = new Size(Math.Floor(availableSize.Width), Math.Floor(availableSize.Height));
			Size result = base.MeasureOverride(availableSize);
			CalculateLayout();
			IList<AxisBase> affectedAxes = UpdateMeasureUnits(availableSize);
			if (affectedAxes.Count > 0) {
				ViewController.SendDataAgreggationUpdates(affectedAxes);
				ChartElementHelper.Update((IChartElement)this, ChartElementChange.UpdateXYDiagram2DItems);
			}
			return result;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			visualContent = GetTemplateChild("PART_VisualContent") as UIElement;
		}
	}
}
