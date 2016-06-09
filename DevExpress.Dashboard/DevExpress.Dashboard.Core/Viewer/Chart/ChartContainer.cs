#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardExport;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.DashboardCommon.Viewer {
	public class ChartContainer : IDisposable, IChartContainer, IChartRenderProvider, IChartDataProvider, IChartEventsProvider, IChartInteractionProvider {
		readonly Chart chart;
		readonly RangeControlClient rangeControlClient;
		bool loading;
		Size size;
		public IRangeControlClientExtension RangeControlClient { get { return rangeControlClient; } }
		DataContainer DataContainer { get { return chart.DataContainer; } }
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return this; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return this; } }
		Chart IChartContainer.Chart { get { return chart; } }
		bool IChartContainer.DesignMode { get { return false; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		bool IChartContainer.Loading { get { return loading; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		IComponent IChartContainer.Parent { get { return null; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return null; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return false; } }
		bool IChartContainer.ShowDesignerHints { get { return false; } }
		ISite IChartContainer.Site { get { return null; } set { } }
		ChartContainerType IChartContainer.ControlType { get { return ChartContainerType.WinControl; } }
		public SeriesSelectionMode SeriesSelectionMode { get; set; }
		public object DataSource { get { return DataContainer.DataSource; } set { DataContainer.DataSource = value; } }
		public Diagram Diagram { get { return chart.Diagram; } set { chart.Diagram = value; } }
		public Legend Legend { get { return chart.Legend; } }
		public SeriesCollection Series { get { return chart.Series; } }
		public Size Size { get { return size; } set { size = value; } }
		public RectangularBorder Border { get { return chart.Border; } }
		public IPrintable Printable { get { return (IPrintable)chart; } }
		public RectangleIndents Padding { get { return chart.Padding; } }
		public Palette Palette { get { return chart.Palette; } }
		public event EventHandler EndLoading;
		public event CustomDrawSeriesPointEventHandler CustomDrawSeriesPoint;
		public event CustomDrawAxisLabelEventHandler CustomDrawAxisLabel;
		public event CustomDrawCrosshairEventHandler CustomDrawCrosshair;
		public event CustomDrawSeriesEventHandler CustomDrawSeries;
		public ChartContainer() {
			chart = new Chart(this);
			((IChartAppearance)chart.Appearance).MarkerAppearance.FillStyle.FillMode = FillMode.Solid;
			this.rangeControlClient = new RangeControlClient(chart);
			ChartPrinter printer = new ChartPrinter(this);
			printer.ImageFormat = ExportHelper.SupportMetafileImageFormat ? XtraCharts.Printing.PrintImageFormat.Metafile : XtraCharts.Printing.PrintImageFormat.Bitmap;
			chart.Printer = printer;
		}
		#region ISupportBarsInteraction
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { } remove { } }
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return null;
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ISupportBarsInteraction.RaiseUIUpdated() {
		}
		object IServiceProvider.GetService(Type serviceType) {
			return null;
		}
		#endregion
		#region IChartDataProvider implementation
		event BoundDataChangedEventHandler IChartDataProvider.BoundDataChanged { add { } remove { } }
		bool IChartDataProvider.CanUseBoundPoints { get { return true; } }
		object IChartDataProvider.DataAdapter { get { return DataContainer.DataAdapter; } set { DataContainer.DataAdapter = value; } }
		DataContext IChartDataProvider.DataContext { get { return null; } }
		bool IChartDataProvider.SeriesDataSourceVisible { get { return true; } }
		object IChartDataProvider.ParentDataSource { get { return null; } }
		void IChartDataProvider.OnBoundDataChanged(EventArgs e) {
		}
		void IChartDataProvider.OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) {
		}
		void IChartDataProvider.OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) {
		}
		bool IChartDataProvider.ShouldSerializeDataSource(object dataSource) {
			return false;
		}
		#endregion
		#region IChartRenderProvider implementation
		Rectangle IChartRenderProvider.DisplayBounds { get { return new Rectangle(new Point(0, 0), size); } }
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		object IChartRenderProvider.LookAndFeel { get { return UserLookAndFeel.Default; } }
		IPrintable IChartRenderProvider.Printable { get { return Printable; } }
		void IChartRenderProvider.InvokeInvalidate() {
		}
		Bitmap IChartRenderProvider.LoadBitmap(string url) {
			return null;
		}
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		#endregion
		#region IChartEventsProvider Implementation
		bool IChartEventsProvider.ShouldCustomDrawSeries { get { return false; } }
		bool IChartEventsProvider.ShouldCustomDrawSeriesPoints { get { return true; } }
		bool IChartEventsProvider.ShouldCustomDrawAxisLabels { get { return true; } }
		void IChartEventsProvider.OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e) {
			if (CustomDrawAxisLabel != null)
				CustomDrawAxisLabel(this, e);
		}
		void IChartEventsProvider.OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			if(CustomDrawSeries != null)
				CustomDrawSeries(this, e);
		}
		void IChartEventsProvider.OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			if (CustomDrawSeriesPoint != null)
				CustomDrawSeriesPoint(this, e);
		}
		void IChartEventsProvider.OnCustomPaint(CustomPaintEventArgs e) {
		}
		void IChartEventsProvider.OnCustomizeAutoBindingSettings(EventArgs e) {
		}
		void IChartEventsProvider.OnPivotChartingCustomizeLegend(CustomizeLegendEventArgs e) {
		}
		void IChartEventsProvider.OnPivotChartingCustomizeResolveOverlappingMode(CustomizeResolveOverlappingModeEventArgs e) {
		}
		void IChartEventsProvider.OnCustomizeSimpleDiagramLayout(CustomizeSimpleDiagramLayoutEventArgs e) {
		}
		void IChartEventsProvider.OnPivotChartingCustomizeXAxisLabels(CustomizeXAxisLabelsEventArgs e) {
		}
		void IChartEventsProvider.OnAxisScaleChanged(AxisScaleChangedEventArgs e) { }
		void IChartEventsProvider.OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) { }
		void IChartEventsProvider.OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) { }
		#endregion
		#region IChartInteractionProvider implementation
		event HotTrackEventHandler IChartInteractionProvider.ObjectHotTracked { add { } remove { } }
		event HotTrackEventHandler IChartInteractionProvider.ObjectSelected { add { } remove { } }
		event SelectedItemsChangedEventHandler IChartInteractionProvider.SelectedItemsChanged { add { } remove { } }
		bool IChartInteractionProvider.CanShowTooltips { get { return false; } }
		bool IChartInteractionProvider.Capture { get { return false; } set { } }
		bool IChartInteractionProvider.DragCtrlKeyRequired { get { return false; } }
		bool IChartInteractionProvider.EnableChartHitTesting { get { return false; } }
		bool IChartInteractionProvider.HitTestingEnabled { get { return false; } }
		ElementSelectionMode IChartInteractionProvider.SelectionMode { get { return ElementSelectionMode.Single; } }
		void IChartInteractionProvider.OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			if (CustomDrawCrosshair != null)
				CustomDrawCrosshair(this, e);
		}
		void IChartInteractionProvider.OnObjectHotTracked(HotTrackEventArgs e) {
		}
		void IChartInteractionProvider.OnObjectSelected(HotTrackEventArgs e) {
		}
		void IChartInteractionProvider.OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) { 
		}
		void IChartInteractionProvider.OnQueryCursor(QueryCursorEventArgs e) {
		}
		void IChartInteractionProvider.OnScroll(ChartScrollEventArgs e) {
		}
		void IChartInteractionProvider.OnScroll3D(ChartScroll3DEventArgs e) {
		}
		void IChartInteractionProvider.OnZoom(ChartZoomEventArgs e) {
		}
		void IChartInteractionProvider.OnZoom3D(ChartZoom3DEventArgs e) {
		}
		void IChartInteractionProvider.OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
		}
		void IChartInteractionProvider.OnLegendItemChecked(LegendItemCheckedEventArgs e) {
		}
		Point IChartInteractionProvider.PointToCanvas(Point p) {
			return p;
		}
		Point IChartInteractionProvider.PointToClient(Point p) {
			return p;
		}
		#endregion
		void IChartContainer.Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		void IChartContainer.Changed() {
		}
		void IChartContainer.Changing() {
		}
		void IChartContainer.LockChangeService() {
		}
		void IChartContainer.UnlockChangeService() {
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) {
			rangeControlClient.RaiseRangeControlRangeChanged(minValue, maxValue, invalidate);
		}
		void IChartContainer.ShowErrorMessage(string message, string title) {
		}
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				chart.Dispose();
		}
		public void Dispose() {
			Dispose(true);
		}
		public void BeginUpdate() {
			loading = true;
		}
		public void EndUpdate() {
			loading = false;
			if(EndLoading != null)
				EndLoading(this, EventArgs.Empty);
		}
		public void Invalidate() {
		}
		public Image GetImage(Size size) {
			if(ExportHelper.SupportMetafileImageFormat)
				return chart.CreateMetafile(size, MetafileFrameUnit.Pixel);
			return chart.CreateBitmap(size);
		}
	}
}
