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

using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Charts.RangeControlClient.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.RangeControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts.RangeControlClient {
	public abstract class ChartRangeControlClient : Control, IRangeControlClient {
		#region Dependency Properties
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(object), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, OnDataSourcePropertyChanged));
		public static readonly DependencyProperty ArgumentDataMemberProperty =
			DependencyProperty.Register("ArgumentDataMember", typeof(string), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, ArgumentDataMemberPropertyChanged));
		public static readonly DependencyProperty ValueDataMemberProperty =
			DependencyProperty.Register("ValueDataMember", typeof(string), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, ValueDataMemberPropertyChanged));
		public static readonly DependencyProperty ThumbLabelFormatStringProperty =
			DependencyProperty.Register("ThumbLabelFormatString", typeof(string), typeof(ChartRangeControlClient),
			new PropertyMetadata(null));
		public static readonly DependencyProperty ShowArgumentLabelsProperty =
			DependencyProperty.Register("ShowArgumentLabels", typeof(bool), typeof(ChartRangeControlClient),
			new PropertyMetadata(true, ShowArgumentLabelsPropertyChanged));
		public static readonly DependencyProperty ArgumentLabelFormatStringProperty =
			DependencyProperty.Register("ArgumentLabelFormatString", typeof(string), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, ArgumentLabelFormatStringPropertyChanged));
		public static readonly DependencyProperty ArgumentLabelTemplateProperty =
			DependencyProperty.Register("ArgumentLabelTemplate", typeof(DataTemplate), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, ArgumentLabelTemplatePropertyChanged));
		public static readonly DependencyProperty ShowGridLinesProperty =
			DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(ChartRangeControlClient),
			new PropertyMetadata(true, ShowGridLinesChanged));
		public static readonly DependencyProperty GridLinesBrushProperty =
			DependencyProperty.Register("GridLinesBrush", typeof(SolidColorBrush), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, GridLinesBrushChanged));
		public static readonly DependencyProperty GridSpacingProperty =
			DependencyProperty.Register("GridSpacing", typeof(double), typeof(ChartRangeControlClient),
			new PropertyMetadata(1.0, GridSpacingPropertyChanged));
		public static readonly DependencyProperty ViewProperty =
			DependencyProperty.Register("View", typeof(RangeControlClientView), typeof(ChartRangeControlClient),
			new PropertyMetadata(null, RangeControlClientViewChanged));
		static void RangeControlClientViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.ActualView = e.NewValue as RangeControlClientView;
		}
		static void OnDataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.itemsProvider.ItemsSource = e.NewValue;
		}
		static void ArgumentDataMemberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.itemsProvider.PointArgumentMember = e.NewValue != null ? e.NewValue.ToString() : null;
		}
		static void ValueDataMemberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.itemsProvider.PointValueMember = e.NewValue != null ? e.NewValue.ToString() : null;
		}
		static void ShowArgumentLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null) {
				client.UpdateLabels();
				client.InvalidateLayout();
			}
		}
		static void ArgumentLabelFormatStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.UpdateLabels();
		}
		static void ArgumentLabelTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.UpdateLabels();
		}
		static void ShowGridLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null && e.NewValue != null) {
				client.gridLinesItem.Visible = (bool)e.NewValue;
				client.UpdateItems();
			}
		}
		static void GridLinesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null)
				client.gridLinesItem.Brush = e.NewValue as SolidColorBrush;
		}
		static void GridSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartRangeControlClient client = d as ChartRangeControlClient;
			if (client != null && e.NewValue != null) {
				client.ChartGridCalculator.GridSpacing = (double)e.NewValue;
				client.UpdateItems();
			}
		}
		#endregion
		abstract protected GridCalculator ChartGridCalculator { get; }
		readonly RangeClientItemsManager itemsManager;
		readonly SparklineItemsProvider itemsProvider = new SparklineItemsProvider();
		readonly RangeClientGridLinesItem gridLinesItem = new RangeClientGridLinesItem();
		readonly List<object> logicalChildren = new List<object>();
		bool sparklineRangeChanging = false;
		SparklineControl sparkline = new AreaSparklineControl();
		AxisGridMapping GridMap { get { return ChartGridCalculator.GridMapping; } }
		RangeClientState rangeClientState = new RangeClientState();
		ItemsControl itemsContainer;
		Rect clientBounds;
		Size viewport;
		IRangeClientScaleMap scaleMap;
		RangeControlClientView view;
		ItemsControl ItemsContainer {
			get { return itemsContainer; }
			set {
				if (itemsContainer != value) {
					if (itemsContainer != null)
						itemsContainer.ItemsSource = null;
					itemsContainer = value;
					if (itemsContainer != null)
						itemsContainer.ItemsSource = itemsManager.Items;
					UpdateItems();
				}
			}
		}
		protected override IEnumerator LogicalChildren { get { return logicalChildren.GetEnumerator(); } }
		RangeControlClientView ActualView {
			get { return view; }
			set {
				if (view != value) {
					RangeControlClientView oldView = view;
					view = value != null ? value : CreateDefaultView();
					RangeControlClientViewChanged(oldView, view);
				}
			}
		}
		internal RangeClientState RangeClientState { get { return rangeClientState; } }
		[Category(Categories.Data)]
		public object ItemsSource {
			get { return GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		[Category(Categories.Data)]
		public string ArgumentDataMember {
			get { return (string)GetValue(ArgumentDataMemberProperty); }
			set { SetValue(ArgumentDataMemberProperty, value); }
		}
		[Category(Categories.Data)]
		public string ValueDataMember {
			get { return (string)GetValue(ValueDataMemberProperty); }
			set { SetValue(ValueDataMemberProperty, value); }
		}
		[Category(Categories.Appearance)]
		public string ThumbLabelFormatString {
			get { return (string)GetValue(ThumbLabelFormatStringProperty); }
			set { SetValue(ThumbLabelFormatStringProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool ShowArgumentLabels {
			get { return (bool)GetValue(ShowArgumentLabelsProperty); }
			set { SetValue(ShowArgumentLabelsProperty, value); }
		}
		[Category(Categories.Appearance)]
		public string ArgumentLabelFormatString {
			get { return (string)GetValue(ArgumentLabelFormatStringProperty); }
			set { SetValue(ArgumentLabelFormatStringProperty, value); }
		}
		[Category(Categories.Appearance)]
		public DataTemplate ArgumentLabelTemplate {
			get { return (DataTemplate)GetValue(ArgumentLabelTemplateProperty); }
			set { SetValue(ArgumentLabelTemplateProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool ShowGridLines {
			get { return (bool)GetValue(ShowGridLinesProperty); }
			set { SetValue(ShowGridLinesProperty, value); }
		}
		[Category(Categories.Appearance)]
		public SolidColorBrush GridLinesBrush {
			get { return (SolidColorBrush)GetValue(GridLinesBrushProperty); }
			set { SetValue(GridLinesBrushProperty, value); }
		}
		[Category(Categories.Layout)]
		public double GridSpacing {
			get { return (double)GetValue(GridSpacingProperty); }
			set { SetValue(GridSpacingProperty, value); }
		}
		[Category(Categories.Appearance)]
		public RangeControlClientView View {
			get { return (RangeControlClientView)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		protected ChartRangeControlClient() {
			this.itemsManager = new RangeClientItemsManager(typeof(SparklineControl), typeof(RangeClientAxisLabelItem), typeof(RangeClientGridLinesItem));
			this.SizeChanged += OnSizeChanged;
			this.Loaded += ChartRangeControlClient_Loaded;
			this.DataContextChanged += ChartRangeControlClient_DataContextChanged;
			this.ActualView = CreateDefaultView();
		}
		RangeControlClientView CreateDefaultView() {
			return new RangeControlClientAreaView();
		}
		void ChartRangeControlClient_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (View != null)
				View.DataContext = e.NewValue;
		}
		#region IRangeControlClient
		bool IRangeControlClient.GrayOutNonSelectedRange {
			get { return true; }
		}
		bool IRangeControlClient.ConvergeThumbsOnZoomingOut {
			get { return false; }
		}
		bool IRangeControlClient.SnapSelectionToGrid {
			get { return false; }
		}
		bool IRangeControlClient.AllowThumbs {
			get { return true; }
		}
		Rect IRangeControlClient.ClientBounds {
			get { return clientBounds; }
		}
		string IRangeControlClient.FormatText(object value) {
			string formatString = ThumbLabelFormatString;
			if (string.IsNullOrEmpty(formatString))
				formatString = "{0}";
			return value != null ? string.Format(formatString, value) : string.Empty;
		}
		double IRangeControlClient.GetComparableValue(object realValue) {
			if (realValue == null)
				return 0.0;
			SparklineScaleType scaleType;
			double? result = SparklineMathUtils.ConvertToDouble(realValue, out scaleType);
			return result.HasValue ? result.Value : 0.0;
		}
		object IRangeControlClient.GetRealValue(double comparable) {
			return ConvertToNative(comparable);
		}
		object IRangeControlClient.GetSnappedValue(object value, bool isLeft) {
			return ChartGridCalculator.GetSnappedValue(value, rangeClientState.WholeStart, rangeClientState.WholeEnd);
		}
		RangeControlClientHitTestResult IRangeControlClient.HitTest(Point point) {
			if ((scaleMap != null) && (GridMap != null) && clientBounds.Width > 0 && clientBounds.Height > 0 && itemsProvider.Points.Count > 0) {
				double internalValue = scaleMap.GetInternalValue(point);
				object nativeValue = ConvertToNative(internalValue);
				return ChartGridCalculator.HitTest(nativeValue, viewport.Width, rangeClientState.WholeStart, rangeClientState.WholeEnd);
			}
			return new RangeControlClientHitTestResult(RangeControlClientRegionType.Nothing);
		}
		void IRangeControlClient.Invalidate(Size viewport) {
			this.viewport = viewport;
			UpdateItems();
			InvalidateLayout(viewport);
			sparkline.InvalidateVisual();
		}
		event EventHandler<LayoutChangedEventArgs> clientLayoutChanged;
		event EventHandler<LayoutChangedEventArgs> IRangeControlClient.LayoutChanged {
			add { clientLayoutChanged += value; }
			remove { clientLayoutChanged -= value; }
		}
		object IRangeControlClient.Start {
			get { return rangeClientState.ActualStart; }
		}
		object IRangeControlClient.End {
			get { return rangeClientState.ActualEnd; }
		}
		object IRangeControlClient.SelectionStart {
			get { return rangeClientState.SelectedStart; }
		}
		object IRangeControlClient.SelectionEnd {
			get { return rangeClientState.SelectedEnd; }
		}
		object IRangeControlClient.VisibleStart {
			get { return rangeClientState.VisibleStart; }
		}
		object IRangeControlClient.VisibleEnd {
			get { return rangeClientState.VisibleEnd; }
		}
		bool IRangeControlClient.SetRange(object wholeStart, object wholeEnd, Size viewportSize) {
			bool result = true;
			if (rangeClientState.ActualStart != wholeStart || this.rangeClientState.ActualEnd != wholeEnd) {
				object start = TryParseValue(wholeStart);
				object end = TryParseValue(wholeEnd);
				if (CheckValueType(start))
					rangeClientState.WholeStart = start;
				if (CheckValueType(end))
					rangeClientState.WholeEnd = end;
				if (CheckValueType(start) && CheckValueType(end)) {
					if (!sparklineRangeChanging) {
						sparklineRangeChanging = true;
						sparkline.ArgumentRange.Auto = false;
						sparkline.ArgumentRange.Limit1 = rangeClientState.ActualStart;
						sparkline.ArgumentRange.Limit2 = rangeClientState.ActualEnd;
						sparklineRangeChanging = false;
					}
					UpdateItems();
				}
				else {
					result = false;
					sparkline.ArgumentRange.Auto = true;
				}
				InvalidateLayout(viewportSize);
			}
			return result;
		}
		bool IRangeControlClient.SetSelectionRange(object selectionStart, object selectionEnd, Size viewportSize, bool isSnapped) {
			bool result = false;
			object start = TryParseValue(selectionStart);
			object end = TryParseValue(selectionEnd);
			if (rangeClientState.SelectedStart == start && rangeClientState.SelectedEnd == end)
				return true;
			if (CheckValueType(start))
				rangeClientState.SelectedStart = start;
			if (CheckValueType(end))
				rangeClientState.SelectedEnd = end;
			result = CheckValueType(start) && CheckValueType(end);
			if (!result)
				UpdateItems();
			InvalidateLayout(viewportSize);
			return result;
		}
		bool IRangeControlClient.SetVisibleRange(object visibleStart, object visibleEnd, Size viewportSize) {
			object start = TryParseValue(visibleStart);
			object end = TryParseValue(visibleEnd);
			bool isValidVisibleStart = CheckValueType(visibleStart);
			bool isValidVisibleEnd = CheckValueType(visibleEnd);
			if (isValidVisibleStart && !isValidVisibleEnd)
				rangeClientState.VisibleStart = visibleStart;
			else if(!isValidVisibleStart && isValidVisibleEnd)
				rangeClientState.VisibleEnd = visibleEnd;
			else if (isValidVisibleStart && isValidVisibleEnd) {
				object correctMin, correctMax;
				CorrectVisibleRange(rangeClientState.VisibleStart, rangeClientState.VisibleEnd, start, end, out correctMin, out correctMax);
				rangeClientState.VisibleStart = correctMin;
				rangeClientState.VisibleEnd = correctMax;
				UpdateItems();
				return false;
			}
			InvalidateLayout(viewportSize);
			return true;
		}
		#endregion
		double ConvertToDouble(object value) {
			SparklineScaleType scaleType;
			double? result = SparklineMathUtils.ConvertToDouble(value, out scaleType);
			return result.HasValue ? result.Value : double.NaN;
		}
		IList<double> UpdateLabels() {
			List<double> result = new List<double>();
			if (viewport.Width <= 1)
				return result;
			IList<double> gridValues = ChartGridCalculator.CalculateGrid(viewport.Width, rangeClientState.VisibleStart, rangeClientState.VisibleEnd);
			string formatString = ArgumentLabelFormatString;
			if (string.IsNullOrEmpty(formatString))
				formatString = "{0}";
			DataTemplate template = ArgumentLabelTemplate;
			bool showLabels = ShowArgumentLabels;
			int gridIndex = 0;
			foreach (RangeClientAxisLabelItem item in itemsManager.GetItems<RangeClientAxisLabelItem>()) {
				if (gridIndex < gridValues.Count) {
					object itemValue = GridMap.InternalToNative(gridValues[gridIndex++]);
					double realValue = ConvertToDouble(itemValue);
					item.Value = itemValue;
					item.Text = string.Format(formatString, itemValue);
					item.InternalValue = realValue;
					item.Template = template;
					item.Visible = showLabels;
					result.Add(realValue);
				}
				else
					item.Visible = false;
			}
			for (; gridIndex < gridValues.Count; gridIndex++) {
				object itemValue = GridMap.InternalToNative(gridValues[gridIndex]);
				double realValue = ConvertToDouble(itemValue);
				RangeClientAxisLabelItem item = new RangeClientAxisLabelItem() { Text = string.Format(formatString, itemValue), Template = template, Value = itemValue, InternalValue = realValue, Visible = showLabels };
				itemsManager.Add<RangeClientAxisLabelItem>(item);
				result.Add(realValue);
			}
			return result;
		}
		void InvalidateLayout() {
			RangeClientItemsPanel itemsPanel = LayoutHelper.FindElement(this, element => element is RangeClientItemsPanel) as RangeClientItemsPanel;
			if (itemsPanel != null)
				itemsPanel.InvalidateMeasure();
		}
		void InvalidateLayout(Size clientSize) {
			RangeClientItemsPanel itemsPanel = LayoutHelper.FindElement(this, element => element is RangeClientItemsPanel) as RangeClientItemsPanel;
			if (itemsPanel != null)
				itemsPanel.UpdateClientSize(clientSize);
		}
		protected void UpdateItems() {
			IList<double> gridValues = UpdateLabels();
			gridLinesItem.UpdateGrid(gridValues);
			InvalidateLayout();
		}
		void ChartRangeControlClient_Loaded(object sender, RoutedEventArgs e) {
			RaiseClientLayoutChanged();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			RaiseClientLayoutChanged();
		}
		void InitSparklineControl(SparklineControl sparkline) {
			sparkline.SparklineArgumentRangeChanged += sparkline_SparklineArgumentRangeChanged;
			Binding binding = new Binding("Points") { Source = itemsProvider };
			BindingOperations.SetBinding(sparkline, SparklineControl.PointsProperty, binding);
			sparkline.SparklinePointsChanged += sparkline_SparklinePointsChanged;
		}
		void sparkline_SparklineArgumentRangeChanged(object sender, SparklineRangeChangedEventArgs e) {
			if (sparklineRangeChanging)
				return;
			sparklineRangeChanging = true;
			rangeClientState.SparklineStart = TryParseValue(e.MinValue);
			rangeClientState.SparklineEnd = TryParseValue(e.MaxValue);
			if (clientLayoutChanged != null)
				clientLayoutChanged(this, new LayoutChangedEventArgs(LayoutChangedType.Data, e.MinValue, e.MaxValue));
			sparklineRangeChanging = false;
		}
		void sparkline_SparklinePointsChanged(object sender, EventArgs e) {
			RaiseClientLayoutChanged();
		}		
		void RangeControlClientViewChanged(RangeControlClientView oldSparklineView, RangeControlClientView sparklineView) {
			RemoveLogicalChild(oldSparklineView);
			AddLogicalChild(sparklineView);
			if (oldSparklineView != null) 
				sparklineView.DataContext = oldSparklineView.DataContext;			
			SparklineControl newSparkline = sparklineView.Sparkline;
			SparklineControl oldSparkline = sparkline;
			oldSparkline.SparklinePointsChanged -= sparkline_SparklinePointsChanged;
			BindingOperations.ClearBinding(oldSparkline, SparklineControl.PointsProperty);
			this.sparkline = newSparkline;
			InitSparklineControl(newSparkline);
			if (rangeClientState.IsRangeSet) {
				sparklineRangeChanging = true;
				newSparkline.ArgumentRange.Limit1 = rangeClientState.ActualStart;
				newSparkline.ArgumentRange.Limit2 = rangeClientState.ActualEnd;
				newSparkline.ArgumentRange.Auto = false;
				sparklineRangeChanging = false;
			}
			itemsManager.Clear();
			itemsManager.Add<SparklineControl>(newSparkline);
			itemsManager.Add<RangeClientGridLinesItem>(gridLinesItem);
			UpdateItems();
		}
		void RaiseClientLayoutChanged() {
			if (clientLayoutChanged != null)
				clientLayoutChanged(this, new LayoutChangedEventArgs(LayoutChangedType.Layout));
		}
		void RaiseClientDataChanged(Object start, Object end) {
			if (clientLayoutChanged != null)
				clientLayoutChanged(this, new LayoutChangedEventArgs(LayoutChangedType.Data, start, end));
		}
		protected abstract object ConvertToNative(double value);
		protected abstract void CorrectVisibleRange(object oldStart, object oldEnd, object newStart, object newEnd, out object correctMin, out object correctMax);
		protected abstract bool CheckValueType(object value);
		protected abstract object TryParseValue(object value);
		protected internal new void AddLogicalChild(object child) {
			if (!logicalChildren.Contains(child)) {
				logicalChildren.Add(child);
				base.AddLogicalChild(child);
			}
		}
		protected internal new void RemoveLogicalChild(object child) {
			if (logicalChildren.Contains(child)) {
				logicalChildren.Remove(child);
				base.RemoveLogicalChild(child);
			}
		}
		internal IRangeClientScaleMap UpdateScaleMap(double rangeWidth) {
			double visualMin = Math.Min(ConvertToDouble(rangeClientState.ActualEnd), ConvertToDouble(rangeClientState.ActualStart));
			double visualMax = Math.Max(ConvertToDouble(rangeClientState.ActualEnd), ConvertToDouble(rangeClientState.ActualStart));
			scaleMap = new RangeClientScaleMap(visualMin, visualMax, rangeWidth);
			return scaleMap;
		}
		internal void SetClientSize(Rect value) {
			if (clientBounds != value) {
				clientBounds = value;
				RaiseClientLayoutChanged();
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsContainer = GetTemplateChild("PART_ItemsContainer") as ItemsControl;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public abstract class GridCalculator {
		delegate bool IsLeftDel(double value, double alignedLeft, double alignedRight);
		readonly List<double> grid = new List<double>();
		protected bool IsSnapAligmentSet { get; set; }
		protected bool IsAutoGridSpacing { get; set; }
		protected AxisGridMapping SnapMapping { get; set; }
		public AxisGridMapping GridMapping { get; protected set; }
		public double GridSpacing { get; set; }
		public GridCalculator() {
			IsAutoGridSpacing = true;
			IsSnapAligmentSet = false;
			GridSpacing = 1;
		}
		double CalcGridStep(double rangeWidth, object visibleRangeStart, object visibleRangeEnd) {
			double width = rangeWidth;
			double visualRange = Math.Abs(GridMapping.NativeToInternal(visibleRangeEnd) - GridMapping.NativeToInternal(visibleRangeStart));
			if (width > 1)
				return visualRange / width * 200;
			else
				return double.NaN;
		}
		double GetGridSpacing(double rangeWidth, object visibleRangeStart, object visibleRangeEnd) {
			double gridSpacing;
			if (IsAutoGridSpacing) {
				double autoGridSpacing = CalcGridStep(rangeWidth, visibleRangeStart, visibleRangeEnd);
				gridSpacing = 1;
				if (!double.IsNaN(autoGridSpacing)) {
					gridSpacing = GridMapping.InternalToAligned(autoGridSpacing);
					gridSpacing = gridSpacing != 0 ? gridSpacing : 1;
				}
			}
			else
				gridSpacing = GridSpacing;
			return gridSpacing;
		}
		object GetAutoSnapValue(object value, object wholeStart, object wholeEnd, IsLeftDel isLeft) {
			AxisGridMapping gridMap = GridMapping;
			double intenalValue = gridMap.NativeToInternal(value);
			double internalRangeMin = gridMap.NativeToInternal(wholeStart);
			double internalRangeMax = gridMap.NativeToInternal(wholeEnd);
			List<double> gridForSnap = new List<double>();
			gridForSnap.Add(internalRangeMin);
			for (int i = 0; i < grid.Count; i++) {
				if (grid[i] > internalRangeMin && grid[i] < internalRangeMax)
					gridForSnap.Add(grid[i]);
			}
			gridForSnap.Add(internalRangeMax);
			double prevInternalValue = gridForSnap[0];
			for (int i = 1; i < gridForSnap.Count; i++) {
				double nextInternalValue = gridForSnap[i];
				if (intenalValue >= prevInternalValue && intenalValue <= nextInternalValue) {
					if (isLeft(intenalValue, prevInternalValue, nextInternalValue))
						return gridMap.InternalToNative(prevInternalValue);
					else
						return gridMap.InternalToNative(nextInternalValue);
				}
				prevInternalValue = gridForSnap[i];
			}
			return isLeft(intenalValue, intenalValue, intenalValue) ? wholeStart : wholeEnd;
		}
		object GetManualSnapValue(object value, object wholeStart, object wholeEnd, IsLeftDel isLeft) {
			double internalValue = SnapMapping.NativeToInternal(value);
			double leftAlignmentValue = Math.Floor(SnapMapping.InternalToAligned(internalValue));
			double leftInternalValue = SnapMapping.AlignedToInternal(leftAlignmentValue);
			double rightInternalValue = SnapMapping.AlignedToInternal(leftAlignmentValue + 1);
			double internalRangeMin = GridMapping.NativeToInternal(wholeStart);
			double internalRangeMax = GridMapping.NativeToInternal(wholeEnd);
			leftInternalValue = Math.Max(leftInternalValue, internalRangeMin);
			rightInternalValue = Math.Min(rightInternalValue, internalRangeMax);
			if (isLeft(internalValue, leftInternalValue, rightInternalValue))
				return SnapMapping.InternalToNative(leftInternalValue);
			return SnapMapping.InternalToNative(rightInternalValue);
		}
		object GetSnappedValue(object value, bool isLeft, object wholeStart, object wholeEnd) {
			if (IsSnapAligmentSet)
				return GetManualSnapValue(value, wholeStart, wholeEnd, (internalValue, alignedLeft, alignedRight) => { return isLeft; });
			return GetAutoSnapValue(value, wholeStart, wholeEnd, (internalValue, alignedLeft, alignedRight) => { return isLeft; });
		}
		public RangeControlClientHitTestResult HitTest(object nativeValue, double rangeWidth, object wholeStart, object wholeEnd) {
			double internalValue = GridMapping.NativeToInternal(nativeValue);
			if (!double.IsNaN(internalValue)) {
				object minNative = GetSnappedValue(nativeValue, true, wholeStart, wholeEnd);
				object maxNative = GetSnappedValue(nativeValue, false, wholeStart, wholeEnd);
				return new RangeControlClientHitTestResult(RangeControlClientRegionType.ItemInterval, minNative, maxNative);
			}
			return new RangeControlClientHitTestResult(RangeControlClientRegionType.Nothing);
		}
		public object GetSnappedValue(object value, object wholeStart, object wholeEnd) {
			if (IsSnapAligmentSet)
				return GetManualSnapValue(value, wholeStart, wholeEnd, 
					(internalValue, alignedLeft, alignedRight) => { return internalValue < alignedLeft + (alignedRight - alignedLeft) / 2; });
			return GetAutoSnapValue(value, wholeStart, wholeEnd, 
				(internalValue, alignedLeft, alignedRight) => { return internalValue < alignedLeft + (alignedRight - alignedLeft) / 2; });
		}
		public IList<double> CalculateGrid(double rangeWidth, object visibleStart, object visibleEnd) {
			double min = Math.Min(GridMapping.NativeToInternal(visibleStart), GridMapping.NativeToInternal(visibleEnd));
			double max = Math.Max(GridMapping.NativeToInternal(visibleStart), GridMapping.NativeToInternal(visibleEnd));
			double gridSpacing = GetGridSpacing(rangeWidth, visibleStart, visibleEnd);
			double aligmentMin = Math.Floor(GridMapping.InternalToAligned(min) / gridSpacing) * gridSpacing;
			double aligmentMax = Math.Floor(GridMapping.InternalToAligned(max) / gridSpacing) * gridSpacing;
			grid.Clear();
			for (double gridValue = aligmentMin; gridValue <= aligmentMax; gridValue += gridSpacing)
				grid.Add(GridMapping.AlignedToInternal(gridValue));
			return grid;
		}
	}
	public class DateTimeGridCalculator : GridCalculator {
		DateTimeMeasurementUnit snapAlignment;
		public DateTimeGridCalculator(DevExpress.Xpf.Charts.RangeControlClient.DateTimeGridAlignment gridAligment) {
			GridMapping = new AxisDateTimeGridMapping(new AxisDateTimeMap(), gridAligment == RangeControlClient.DateTimeGridAlignment.Auto ? DateTimeGridAlignmentNative.Day : (DateTimeGridAlignmentNative)gridAligment, 0);
		}
		public void UpdateSnapAligment(DateTimeMeasurementUnit snapAligment) {
			IsSnapAligmentSet = true;
			this.snapAlignment = snapAligment;
			SnapMapping = new AxisDateTimeGridMapping(new AxisDateTimeMap(), (DateTimeGridAlignmentNative)snapAligment, 0);
		}
		public void UpdateGridAligment(DevExpress.Xpf.Charts.RangeControlClient.DateTimeGridAlignment gridAligment) {
			IsAutoGridSpacing = gridAligment == DevExpress.Xpf.Charts.RangeControlClient.DateTimeGridAlignment.Auto;
			GridMapping  = new AxisDateTimeGridMapping(new AxisDateTimeMap(), IsAutoGridSpacing ? DateTimeGridAlignmentNative.Day : (DateTimeGridAlignmentNative)gridAligment, 0);
		}
	}
	public class NumericGridCalculator : GridCalculator {
		double snapAlignment;
		public NumericGridCalculator(double gridAlignment) {
			GridMapping = new AxisNumericGridMapping(new AxisNumericalMap(), gridAlignment == 0 ? 1 : gridAlignment, 0);
		}
		public void UpdateSnapAligment(double snapAligment) {
			IsSnapAligmentSet = true;
			this.snapAlignment = snapAligment;
			SnapMapping = new AxisNumericGridMapping(new AxisNumericalMap(), snapAlignment, 0);
		}
		public void UpdateGridAligment(double gridAlignment) {
			IsAutoGridSpacing = gridAlignment == 0;
			GridMapping = new AxisNumericGridMapping(new AxisNumericalMap(), gridAlignment, 0);
		}
	}
}
