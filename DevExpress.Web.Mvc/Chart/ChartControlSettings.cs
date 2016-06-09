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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Web.Internal;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.Mvc {
	public class ChartControlSettings : SettingsBase, IChartContainer, IChartRenderProvider, IDisposable {
		Chart chart;
		ChartToolTipController toolTipController;
		SettingsLoadingPanel settingsLoadingPanel;
		DevExpress.XtraCharts.Native.DataContainer DataContainer { get { return chart.DataContainer; } }
		protected internal Chart Chart { get { return chart; } }
		protected internal CustomCallbackEventHandler CustomCallbackInternal { get; set; }
		public bool AutoLayout {
			get { return chart.AutoLayout; }
			set { chart.AutoLayout = value; }
		}
		public bool ClientVisible {
			get { return ClientVisibleInternal; }
			set { ClientVisibleInternal = value; }
		}
		public bool EnableClientSideAPI {
			get { return EnableClientSideAPIInternal; }
			set { EnableClientSideAPIInternal = value; }
		}
		public string AlternateText { get; set; }
		public string DescriptionUrl { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool SaveStateOnCallbacks { get; set; }
		public ElementSelectionMode SelectionMode {
			get { return chart.SelectionMode; }
			set { chart.SelectionMode = value; }
		}
		public SeriesSelectionMode SeriesSelectionMode {
			get { return chart.SeriesSelectionMode; }
			set { chart.SeriesSelectionMode = value; }
		}
		public int PaletteBaseColorNumber {
			get { return chart.PaletteBaseColorNumber; }
			set { chart.PaletteBaseColorNumber = value; }
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public string AppearanceName {
			get { return chart.AppearanceName; }
			set { chart.AppearanceName = value; }
		}
		public string IndicatorsPaletteName {
			get { return chart.IndicatorsPaletteName; }
			set { chart.IndicatorsPaletteName = value; }
		}
		public string PaletteName {
			get { return chart.PaletteName; }
			set { chart.PaletteName = value; }
		}		
		public string SeriesDataMember {
			get { return DataContainer.SeriesDataMember; }
			set { DataContainer.SeriesDataMember = value; }
		}
		public Color BackColor {
			get { return chart.BackColor; }
			set { chart.BackColor = value; }
		}
		public SortingMode SeriesSorting {
			get { return chart.DataContainer.BoundSeriesSorting; }
			set { chart.DataContainer.BoundSeriesSorting = value; }
		}
		public DefaultBoolean RightToLeft {
			get { return chart.RightToLeft; }
			set { chart.RightToLeft = value; }
		}
		public DefaultBoolean CrosshairEnabled {
			get { return Chart.CrosshairEnabled; }
			set { Chart.CrosshairEnabled = value; }
		}
		public DefaultBoolean ToolTipEnabled {
			get { return Chart.ToolTipEnabled; }
			set { Chart.ToolTipEnabled = value; }
		}
		public ChartToolTipController ToolTipController {
			get { return toolTipController; }
		}
		public ToolTipOptions ToolTipOptions {
			get { return Chart.ToolTipOptions; }
		}
		public CrosshairOptions CrosshairOptions {
			get { return Chart.CrosshairOptions; }
		}
		public AnnotationCollection Annotations { get { return chart.Annotations; } }
		public AnnotationRepository AnnotationRepository { get { return chart.AnnotationRepository; } }
		public DevExpress.XtraCharts.BackgroundImage BackImage { get { return chart.BackImage; } }
		public RectangularBorder BorderOptions { get { return chart.Border; } }
		public Diagram Diagram { get { return chart.Diagram; } }
		public EmptyChartText EmptyChartText { get { return chart.EmptyChartText; } }
		public RectangleFillStyle FillStyle { get { return chart.FillStyle; } }
		public Legend Legend { get { return chart.Legend; } }
		public RectangleIndents Padding { get { return chart.Padding; } }
		public PivotGridDataSourceOptions PivotGridDataSourceOptions { get { return DataContainer.PivotGridDataSourceOptions; } }
		public SeriesCollection Series { get { return DataContainer.Series; } }
		public SeriesBase SeriesTemplate { get { return DataContainer.SeriesTemplate; } }
		public SeriesNameTemplate SeriesNameTemplate { get { return DataContainer.SeriesNameTemplate; } }
		public SmallChartText SmallChartText { get { return chart.SmallChartText; } }
		public ChartTitleCollection Titles { get { return chart.Titles; } }
		public BinaryStorageMode BinaryStorageMode { get; set; }
		public ImageProperties LoadingPanelImage { get { return ImagesInternal.LoadingPanel; } }
		public ChartClientSideEvents ClientSideEvents { get { return (ChartClientSideEvents)ClientSideEventsInternal; } }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public WebChartControlStyles Styles { get { return (WebChartControlStyles)StylesInternal; } }
		public PaletteRepository PaletteRepository { get { return chart.PaletteRepository; } }
		public override Unit Width {
			get { return base.Width; }
			set {
				if(value.Type != UnitType.Pixel)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgWebInvalidWidthUnit));
				base.Width = value;
			}
		}
		public override Unit Height {
			get { return base.Height; }
			set {
				if(value.Type != UnitType.Pixel)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgWebInvalidHeightUnit));
				base.Height = value;
			}
		}
		public BoundDataChangedEventHandler BoundDataChanged { get; set; }
		public CustomDrawAxisLabelEventHandler CustomDrawAxisLabel { get; set; }
		public CustomDrawSeriesEventHandler CustomDrawSeries { get; set; }
		public CustomDrawSeriesPointEventHandler CustomDrawSeriesPoint { get; set; }
		public CustomPaintEventHandler CustomPaint { get; set; }
		public CustomizeAutoBindingSettingsEventHandler CustomizeAutoBindingSettings { get; set; }
		public CustomizeSimpleDiagramLayoutEventHandler CustomizeSimpleDiagramLayout { get; set; }
		public PivotGridSeriesExcludedEventHandler PivotGridSeriesExcluded { get; set; }
		public PivotGridSeriesPointsExcludedEventHandler PivotGridSeriesPointsExcluded { get; set; }
		public EventHandler<AxisScaleChangedEventArgs> AxisScaleChanged { get; set; }
		public EventHandler<AxisRangeChangedEventArgs> AxisWholeRangeChanged { get; set; }
		public EventHandler<AxisRangeChangedEventArgs> AxisVisualRangeChanged { get; set; }
		public SelectedItemsChangedEventHandler SelectedItemsChanged { get; set; }
		public HotTrackEventHandler ObjectSelected { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public LegendItemCheckedEventHandler LegendItemChecked { get; set; }
		public CustomizeXAxisLabelsEventHandler PivotChartingCustomizeXAxisLabels { get; set; }
		public CustomizeResolveOverlappingModeEventHandler PivotChartingCustomizeResolveOverlappingMode { get; set; }
		public CustomizeLegendEventHandler PivotChartingCustomizeLegend { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled { get { return true; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return false; } set { } }
		#region Obsolete
		[
		Obsolete("This property is now obsolete. Use the PivotGridDataSourceOptions.AutoBindingSettingsEnabled property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool AutoBindingSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled = value; }
		}
		[
		Obsolete("This property is now obsolete. Use the PivotGridDataSourceOptions.AutoLayoutSettingsEnabled property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool AutoLayoutSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled = value; }
		}
		[
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		[
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		[
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[
		Obsolete("This property is obsolete and isn't used at all."),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool EnableClientSidePointToDiagram { get { return true; } set { } }
		[
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool ShowLoadingPanel { get { return SettingsLoadingPanel.Enabled; } set { SettingsLoadingPanel.Enabled = value; } }
		[
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[
		Obsolete("This property is now obsolete. Use the SelectionMode property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool RuntimeSelection {
			get { return SelectionMode != ElementSelectionMode.None; }
			set { SelectionMode = value ? ElementSelectionMode.Single : ElementSelectionMode.None; }
		}
		[
		Obsolete("Use the ChartControlSettings.CustomActionRouteValues property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public CustomCallbackEventHandler CustomCallback {
			get { return CustomCallbackInternal; }
			set { CustomCallbackInternal = value; }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public CustomizeXAxisLabelsEventHandler CustomizeXAxisLabels {
			get { return PivotChartingCustomizeXAxisLabels; }
			set { PivotChartingCustomizeXAxisLabels = value; }
		}
		[
		Obsolete("The CustomizeResolveOverlappingMode event is obsolete now. Use the PivotChartingCustomizeResolveOverlappingMode event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public CustomizeResolveOverlappingModeEventHandler CustomizeResolveOverlappingMode {
			get { return PivotChartingCustomizeResolveOverlappingMode; }
			set { PivotChartingCustomizeResolveOverlappingMode = value; }
		}
		[
		Obsolete("The CustomizeLegend event is obsolete now. Use the PivotChartingCustomizeLegend event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public CustomizeLegendEventHandler CustomizeLegend {
			get { return PivotChartingCustomizeLegend; }
			set { PivotChartingCustomizeLegend = value; }
		}
		#endregion
		public ChartControlSettings() {
			this.chart = new Chart(this);
			this.toolTipController = new ChartToolTipController();
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			AlternateText = string.Empty;
			DescriptionUrl = string.Empty;
			EnableClientSideAPIInternal = true;
			Height = 200;
			SaveStateOnCallbacks = true;
			ToolTip = string.Empty;
			Width = 300;
		}
		#region ISupportBarsInteraction implementation
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { } remove { } }
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return null;
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		void ISupportBarsInteraction.RaiseUIUpdated() {
		}
		object IServiceProvider.GetService(Type serviceType) {
			IServiceProvider provider = ((IChartContainer)this).ServiceProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
		#endregion 
		#region IChartRenderProvider implementation
		Rectangle IChartRenderProvider.DisplayBounds { get { return new Rectangle(0, 0, (int)Width.Value, (int)Height.Value); } }
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		object IChartRenderProvider.LookAndFeel { get { return null; } }
		IPrintable IChartRenderProvider.Printable { get { return null; } }
		void IChartRenderProvider.Invalidate() {
		}
		void IChartRenderProvider.InvokeInvalidate() {
		}
		Bitmap IChartRenderProvider.LoadBitmap(string url) {
			return null;
		}
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		#endregion
		#region IChartContainer
		event EventHandler IChartContainer.EndLoading { add { } remove { } }
		IChartDataProvider IChartContainer.DataProvider { get { return null; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return null; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return null; } }
		bool IChartContainer.DesignMode { get { return true; } }
		bool IChartContainer.Loading { get { return false; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return true; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		bool IChartContainer.ShowDesignerHints { get { return false; } }
		Chart IChartContainer.Chart { get { return chart; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		ChartContainerType IChartContainer.ControlType { get { return ChartContainerType.WebControl; } }
		IComponent IChartContainer.Parent { get { return null; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return null; } }
		ISite IChartContainer.Site { get { return null; } set { } }
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
		void IChartContainer.ShowErrorMessage(string message, string title) {
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object min, object max, bool invalidate) {
		}
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		#endregion
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ChartClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new WebChartControlStyles(null);
		}
		public void SaveToStream(Stream stream) {
			chart.SaveLayout(stream);
		}
		public void SaveToFile(string path) {
			using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
				SaveToStream(stream);
		}
		public void LoadFromStream(Stream stream) {
			stream.Seek(0L, SeekOrigin.Begin);
			if (!XtraSerializingHelper.IsValidXml(stream))
				throw new LayoutStreamException();
			stream.Seek(0L, SeekOrigin.Begin);
			chart.LoadLayout(stream);
		}
		public void LoadFromFile(string path) {
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				LoadFromStream(stream);
		}
		public void RegisterSummaryFunction(string name, string displayName, ScaleType? resultScaleType, int resultDimension, SummaryFunctionArgumentDescription[] argumentDescriptions, SummaryFunction function) {
			chart.RegisterSummaryFunction(name, displayName, resultScaleType, resultDimension, argumentDescriptions, function);
		}
		public void Dispose() {
			if (chart != null) {
				chart.Dispose();
				chart = null;
			}
		}
	}
}
