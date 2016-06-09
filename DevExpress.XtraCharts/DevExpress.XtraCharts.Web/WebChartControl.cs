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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Design;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraCharts.Web.Designer.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
namespace DevExpress.XtraCharts.Web {
	internal class ResFinder {
	}
	[
	DXWebToolboxItem(true),
	Designer("DevExpress.XtraCharts.Design.WebChartControlDesigner," + AssemblyInfo.SRAssemblyChartsWebDesign),
	TypeConverter(typeof(ChartTypeConverter)),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "ChartControl.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabData),
	ControlBuilder(typeof(ChartControlBuilder))
	]
	public class WebChartControl : ASPxDataWebControl, IChartContainer, IChartRenderProvider, IChartDataProvider, IChartEventsProvider, IChartInteractionProvider, ISupportInitialize, ICustomTypeDescriptor, ICoreReference, IRequiresLoadPostDataControl, IEndInitAccessor {
		#region Nested classes: PaletteWrapperCollection, WebChartControlPropertyDescriptorCollection
		public class PaletteWrapperCollection : CollectionBase {
			PaletteRepository repository;
			public PaletteWrapper this[int index] { get { return (PaletteWrapper)List[index]; } }
			public PaletteWrapperCollection(PaletteRepository repository) {
				this.repository = repository;
				foreach (string name in repository.GetPaletteNames())
					List.Add(new PaletteWrapper((Palette)repository[name]));
			}
			public int Add(PaletteWrapper wrapper) {
				repository.Add(wrapper.Palette.Name, wrapper.Palette);
				return List.Add(wrapper);
			}
		}
		class WebChartControlPropertyDescriptorCollection : PropertyDescriptorCollection {
			public WebChartControlPropertyDescriptorCollection(ICollection descriptors)
				: base(new PropertyDescriptor[0]) {
				foreach (PropertyDescriptor pd in descriptors)
					if (pd.DisplayName == "Series")
						Add(new CustomPropertyDescriptor(pd, false));
					else
						Add(pd);
			}
		}
		#endregion
		static readonly object customDrawSeries = new object();
		static readonly object customDrawSeriesPoint = new object();
		static readonly object customDrawCrosshair = new object();
		static readonly object customDrawAxisLabel = new object();
		static readonly object customPaint = new object();
		static readonly object boundDataChanged = new object();
		static readonly object pieSeriesPointExploded = new object();
		static readonly object endLoading = new object();
		static readonly object objectSelected = new object();
		static readonly object selectedItemsChanged = new object();
		static readonly object callbackStateLoad = new object();
		static readonly object callbackStateSave = new object();
		static readonly object customCallback = new object();
		static readonly object dateTimeMeasurementUnitsCalculated = new object();
		static readonly object axisScaleChanged = new object();
		static readonly object axisWholeRangeChanged = new object();
		static readonly object axisVisualRangeChanged = new object();
		static readonly object customizeAutoBindingSettings = new object();
		static readonly object customizeSimpleDiagramLayout = new object();
		static readonly object pivotChartingCustomizeXAxisLabels = new object();
		static readonly object pivotChartingCustomizeResolveOverlappingMode = new object();
		static readonly object pivotChartingCustomizeLegend = new object();
		static readonly object pivotGridSeriesExcluded = new object();
		static readonly object pivotGridSeriesPointsExcluded = new object();
		static readonly object legendItemChecked = new object();
		static WebChartControl() {
			WebLoadingHelper.Initialize(GetGlobalLoading, SetGlobalLoading);
		}
		static bool? GetGlobalLoading() {
			HttpContext httpContext = HttpContext.Current;
			if (httpContext != null) {
				int globalLoadingCounter = GetGlobalLoadingCounter(httpContext);
				return globalLoadingCounter > 0;
			}
			return null;
		}
		static void SetGlobalLoading(bool loading) {
			HttpContext httpContext = HttpContext.Current;
			if (httpContext != null) {
				int globalLoadingCounter = GetGlobalLoadingCounter(httpContext);
				if (loading)
					globalLoadingCounter++;
				else if (globalLoadingCounter > 0)
					globalLoadingCounter--;
				httpContext.Items[GlobalLoadingKey] = globalLoadingCounter;
			}
		}
		static int GetGlobalLoadingCounter(HttpContext httpContext) {
			if (!httpContext.Items.Contains(GlobalLoadingKey))
				httpContext.Items.Add(GlobalLoadingKey, 0);
			return (int)httpContext.Items[GlobalLoadingKey];
		}
		[Obsolete("The ProcessImageRequest method is now obsolete. Instead, select the WebChartControl on a Web Page at Visual Studio design time, open its smart tag and choose \"Add an ASPxHttpHandlerModule to Web.Config\".")]
		public static bool ProcessImageRequest(System.Web.UI.Page page) {
			return BinaryStorage.ProcessRequest();
		}
		const string GlobalLoadingKey = "GlobalLoading";
		const string exportPrintingSign = "ChartsExportPrinting";
		const string callBackChartPropertyName = "chart";
		const string imageID = "IMG";
		const string seriesSerializationID = "ser";
		const string indicatorSeriealizationID = "ind";
		const string constantLineSeriealizationID = "constLine";
		const string stripSeriealizationID = "strip";
		const string primaryAxisXSerializationID = "axisX";
		const string primaryAxisYSerializationID = "axisY";
		const string secondaryAxisXSerializationID = "secondaryAxisX";
		const string secondaryAxisYSerializationID = "secondaryAxisY";
		const char serializationIDAndIndexSeparator = '-';
		const char serializationIDsSeparator = '_';
		internal const string ChartDefaultCssResourceName = "DevExpress.XtraCharts.Web.Css.default.css";
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCallbackStateLoad")]
#endif
		public event CallbackStateLoadEventHandler CallbackStateLoad {
			add { Events.AddHandler(callbackStateLoad, value); }
			remove { Events.RemoveHandler(callbackStateLoad, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCallbackStateSave")]
#endif
		public event CallbackStateSaveEventHandler CallbackStateSave {
			add { Events.AddHandler(callbackStateSave, value); }
			remove { Events.RemoveHandler(callbackStateSave, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomCallback")]
#endif
		public event CustomCallbackEventHandler CustomCallback {
			add { Events.AddHandler(customCallback, value); }
			remove { Events.RemoveHandler(customCallback, value); }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomJSProperties"),
#endif
		Category("Data")
		]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomDrawSeries")]
#endif
		public event CustomDrawSeriesEventHandler CustomDrawSeries {
			add { Events.AddHandler(customDrawSeries, value); }
			remove { Events.RemoveHandler(customDrawSeries, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomDrawSeriesPoint")]
#endif
		public event CustomDrawSeriesPointEventHandler CustomDrawSeriesPoint {
			add { Events.AddHandler(customDrawSeriesPoint, value); }
			remove { Events.RemoveHandler(customDrawSeriesPoint, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event CustomDrawCrosshairEventHandler CustomDrawCrosshair {
			add { Events.AddHandler(customDrawCrosshair, value); }
			remove { Events.RemoveHandler(customDrawCrosshair, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomDrawAxisLabel")]
#endif
		public event CustomDrawAxisLabelEventHandler CustomDrawAxisLabel {
			add { Events.AddHandler(customDrawAxisLabel, value); }
			remove { Events.RemoveHandler(customDrawAxisLabel, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomPaint")]
#endif
		public event CustomPaintEventHandler CustomPaint {
			add { Events.AddHandler(customPaint, value); }
			remove { Events.RemoveHandler(customPaint, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlObjectSelected")]
#endif
		public event HotTrackEventHandler ObjectSelected {
			add { Events.AddHandler(objectSelected, value); }
			remove { Events.RemoveHandler(objectSelected, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlSelectedItemsChanged")]
#endif
		public event SelectedItemsChangedEventHandler SelectedItemsChanged {
			add { Events.AddHandler(selectedItemsChanged, value); }
			remove { Events.RemoveHandler(selectedItemsChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlBoundDataChanged")]
#endif
		public event BoundDataChangedEventHandler BoundDataChanged {
			add { Events.AddHandler(boundDataChanged, value); }
			remove { Events.RemoveHandler(boundDataChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlPieSeriesPointExploded")]
#endif
		public event PieSeriesPointExplodedEventHandler PieSeriesPointExploded {
			add { Events.AddHandler(pieSeriesPointExploded, value); }
			remove { Events.RemoveHandler(pieSeriesPointExploded, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlAxisScaleChanged")]
#endif
		public event EventHandler<AxisScaleChangedEventArgs> AxisScaleChanged {
			add { Events.AddHandler(axisScaleChanged, value); }
			remove { Events.RemoveHandler(axisScaleChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlAxisWholeRangeChanged")]
#endif
		public event EventHandler<AxisRangeChangedEventArgs> AxisWholeRangeChanged {
			add { Events.AddHandler(axisWholeRangeChanged, value); }
			remove { Events.RemoveHandler(axisWholeRangeChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlAxisVisualRangeChanged")]
#endif
		public event EventHandler<AxisRangeChangedEventArgs> AxisVisualRangeChanged {
			add { Events.AddHandler(axisVisualRangeChanged, value); }
			remove { Events.RemoveHandler(axisVisualRangeChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlEndLoading")]
#endif
		public event EventHandler EndLoading {
			add { Events.AddHandler(endLoading, value); }
			remove { Events.RemoveHandler(endLoading, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomizeAutoBindingSettings")]
#endif
		public event CustomizeAutoBindingSettingsEventHandler CustomizeAutoBindingSettings {
			add { Events.AddHandler(customizeAutoBindingSettings, value); }
			remove { Events.RemoveHandler(customizeAutoBindingSettings, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlCustomizeSimpleDiagramLayout")]
#endif
		public event CustomizeSimpleDiagramLayoutEventHandler CustomizeSimpleDiagramLayout {
			add { Events.AddHandler(customizeSimpleDiagramLayout, value); }
			remove { Events.RemoveHandler(customizeSimpleDiagramLayout, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlPivotGridSeriesExcluded")]
#endif
		public event PivotGridSeriesExcludedEventHandler PivotGridSeriesExcluded {
			add { Events.AddHandler(pivotGridSeriesExcluded, value); }
			remove { Events.RemoveHandler(pivotGridSeriesExcluded, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlPivotGridSeriesPointsExcluded")]
#endif
		public event PivotGridSeriesPointsExcludedEventHandler PivotGridSeriesPointsExcluded {
			add { Events.AddHandler(pivotGridSeriesPointsExcluded, value); }
			remove { Events.RemoveHandler(pivotGridSeriesPointsExcluded, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlLegendItemChecked")]
#endif
		public event LegendItemCheckedEventHandler LegendItemChecked {
			add { Events.AddHandler(legendItemChecked, value); }
			remove { Events.RemoveHandler(legendItemChecked, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlPivotChartingCustomizeLegend")]
#endif
		public event CustomizeLegendEventHandler PivotChartingCustomizeLegend {
			add { Events.AddHandler(pivotChartingCustomizeLegend, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeLegend, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlPivotChartingCustomizeXAxisLabels")]
#endif
		public event CustomizeXAxisLabelsEventHandler PivotChartingCustomizeXAxisLabels {
			add { Events.AddHandler(pivotChartingCustomizeXAxisLabels, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeXAxisLabels, value); }
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlPivotChartingCustomizeResolveOverlappingMode")]
#endif
		public event CustomizeResolveOverlappingModeEventHandler PivotChartingCustomizeResolveOverlappingMode {
			add { Events.AddHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeLegendEventHandler CustomizeLegend {
			add { Events.AddHandler(pivotChartingCustomizeLegend, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeLegend, value); }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeXAxisLabelsEventHandler CustomizeXAxisLabels {
			add { Events.AddHandler(pivotChartingCustomizeXAxisLabels, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeXAxisLabels, value); }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeResolveOverlappingModeEventHandler CustomizeResolveOverlappingMode {
			add { Events.AddHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
		}
		#region Hidden properties
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Border BorderLeft { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Border BorderTop { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Border BorderRight { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Border BorderBottom { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new BackgroundImage BackgroundImage { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override string Cursor { get { return null; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override bool EncodeHtml { get { return false; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new Font Font { get { return null; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new Color ForeColor { get { return Color.Empty; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override bool Enabled { get { return true; } set { } }
		#endregion
		Chart chart;
		List<Series> seriesSerializable;
		bool loading;
		bool endLoadingCompleted;
		int lockChangeServiceCounter = 0;
		string dtProjectPath = String.Empty;
		System.Web.UI.WebControls.Image image;
		System.Web.UI.WebControls.WebControl panel;
		StateBag callbackState;
		string callBackResult;
		string alternateText = String.Empty;
		string descriptionUrl = String.Empty;
		AspxSerializerWrapper<Diagram> diagramSerializerWrapper;
		ChartToolTipController toolTipController;
		ImageFormat ImageFormat { get { return DesignMode ? ImageFormat.Bmp : ImageFormat.Png; } }
		string ProjectPath {
			get {
				if (WebLoadingHelper.GlobalLoading)
					throw new InvalidOperationException("Can't get project path then loading.");
				if (DesignMode)
					return dtProjectPath;
				if (Page != null)
					return Page.MapPath("~");
				return System.Web.Hosting.HostingEnvironment.MapPath("~");
			}
		}
		bool IsToolTipsEnabled { get { return ToolTipOptions.ShowForSeries || ToolTipOptions.ShowForPoints; } }
		bool HasClientCustomDrawCrosshairEventHandler { get { return ClientSideEvents.CustomDrawCrosshair != ""; } }
		bool HasClientObjectHotTrackedEventHandler { get { return ClientSideEvents.ObjectHotTracked != ""; } }
		bool HasClientObjectSelectedEventHandler { get { return ClientSideEvents.ObjectSelected != ""; } }
		bool HasServerObjectSelectedEventHandler { get { return Events[objectSelected] != null; } }
		bool HasCustomCallbackEventHandler { get { return Events[customCallback] != null; } }
		bool IsCallbackStateInitialize { get { return callbackState != null && callbackState.Count > 0; } }
		bool IsWebChartControlChild { get { return GetType() != typeof(WebChartControl); } }
		StateBag CallbackState {
			get {
				if (callbackState == null) {
					callbackState = new StateBag();
					((IStateManager)callbackState).TrackViewState();
				}
				return callbackState;
			}
		}
		string ActualAlternateText { get { return String.IsNullOrEmpty(alternateText) ? DefaultAlternateTextBuilder.BuildTextForChart(chart) : alternateText; } }
		DevExpress.XtraCharts.Native.DataContainer ChartDataContainer { get { return Chart == null ? null : Chart.DataContainer; } }
		protected WebChartControlStyles Styles { get { return (WebChartControlStyles)StylesInternal; } }
		internal Chart Chart { get { return chart; } }
		protected internal new bool DesignMode { get { return base.DesignMode; } }
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlWidth")]
#endif
		public override Unit Width {
			get { return base.Width; }
			set {
				if (value.Type != UnitType.Pixel)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgWebInvalidWidthUnit));
				base.Width = value;
			}
		}
#if !SL
	[DevExpressXtraChartsWebLocalizedDescription("WebChartControlHeight")]
#endif
		public override Unit Height {
			get { return base.Height; }
			set {
				if (value.Type != UnitType.Pixel)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgWebInvalidHeightUnit));
				base.Height = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlPadding"),
#endif
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RectangleIndents Padding {
			get { return chart.Padding; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlAutoLayout"),
#endif
		Category(Categories.Layout)
		]
		public bool AutoLayout {
			get { return Chart.AutoLayout; }
			set { chart.AutoLayout = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlEmptyChartText"),
#endif
		Category("Behavior"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty
		]
		public EmptyChartText EmptyChartText { get { return Chart.EmptyChartText; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSmallChartText"),
#endif
		Category("Behavior"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty
		]
		public SmallChartText SmallChartText { get { return Chart.SmallChartText; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlPaletteName"),
#endif
		Category("Appearance"),
		Editor("DevExpress.XtraCharts.Design.PaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter(typeof(PaletteTypeConverter))
		]
		public string PaletteName {
			get { return chart.PaletteName; }
			set { chart.PaletteName = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlIndicatorsPaletteName"),
#endif
		Category("Appearance"),
		Editor("DevExpress.XtraCharts.Design.PaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter(typeof(PaletteTypeConverter))
		]
		public string IndicatorsPaletteName {
			get { return chart.IndicatorsPaletteName; }
			set { chart.IndicatorsPaletteName = value; }
		}
		[
		Obsolete("This property is obsolete and isn't used at all."),
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false)
		]
		public object DataAdapter { get { return null; } set { } }
		[
		Obsolete("The WebChartControl.AutoBindingSettingsEnabled property is now obsolete. Use the WebChartControl.PivotGridDataSourceOptions.AutoBindingSettingsEnabled property instead."),
		DefaultValue(true),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool AutoBindingSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled = value; }
		}
		[
		Obsolete("The WebChartControl.AutoBindingSettingsEnabled property is now obsolete. Use the WebChartControl.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled property instead."),
		DefaultValue(true),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool AutoLayoutSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlPivotGridDataSourceOptions"),
#endif
		Category(Categories.Data),
		TypeConverter(typeof(PivotGridDataSourceOptionsTypeConverter)),
		NestedTagProperty
		]
		public PivotGridDataSourceOptions PivotGridDataSourceOptions { get { return ChartDataContainer.PivotGridDataSourceOptions; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlAppearanceName"),
#endif
		Category("Appearance"),
		TypeConverter(typeof(AppearanceTypeConverter))
		]
		public string AppearanceName {
			get { return chart.AppearanceName; }
			set { chart.AppearanceName = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public string AppearanceNameSerializable {
			get { return chart.AppearanceNameSerializable; }
			set { chart.AppearanceNameSerializable = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlBackImage"),
#endif
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty
		]
		public BackgroundImage BackImage { get { return chart.BackImage; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override BorderWrapper Border { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlBorderOptions"),
#endif
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty
		]
		public RectangularBorder BorderOptions { get { return chart.Border; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlDiagram"),
#endif
		Category("Elements"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public Diagram Diagram {
			get { return chart.Diagram; }
			set { chart.Diagram = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NestedTagProperty
		]
		public IList DiagramSerializable {
			get { return diagramSerializerWrapper; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue("")
		]
		public string DiagramTypeName {
			get { return String.Empty; }
			set { }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlFillStyle"),
#endif
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty
		]
		public RectangleFillStyle FillStyle { get { return chart.FillStyle; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlBackColor"),
#endif
		Category("Appearance")
		]
		public new Color BackColor {
			get { return chart.BackColor; }
			set { chart.BackColor = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlLegend"),
#endif
		Category("Elements"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty
		]
		public Legend Legend { get { return chart.Legend; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlPaletteBaseColorNumber"),
#endif
		Category("Appearance")
		]
		public int PaletteBaseColorNumber {
			get { return chart.PaletteBaseColorNumber; }
			set { chart.PaletteBaseColorNumber = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlAnnotations"),
#endif
		Category("Elements"),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public AnnotationCollection Annotations { get { return chart.Annotations; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlAnnotationRepository"),
#endif
		Category("Elements"),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty
		]
		public AnnotationRepository AnnotationRepository { get { return chart.AnnotationRepository; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSeries"),
#endif
		Category("Elements"),
		Editor("DevExpress.XtraCharts.Design.SeriesCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		NestedTagProperty
		]
		public SeriesCollection Series { get { return chart.Series; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NestedTagProperty
		]
		public List<Series> SeriesSerializable {
			get {
				if (seriesSerializable == null) {
					seriesSerializable = new List<Series>();
					seriesSerializable.AddRange(ChartDataContainer.SeriesSerializable);
				}
				else if (!loading) {
					seriesSerializable.Clear();
					seriesSerializable.AddRange(ChartDataContainer.SeriesSerializable);
				}
				return seriesSerializable;
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSeriesDataMember"),
#endif
		Bindable(false),
		Category("Data"),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		]
		public string SeriesDataMember {
			get { return ChartDataContainer.SeriesDataMember; }
			set { ChartDataContainer.SeriesDataMember = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSeriesNameTemplate"),
#endif
		Category("Data"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty
		]
		public SeriesNameTemplate SeriesNameTemplate { get { return ChartDataContainer.SeriesNameTemplate; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSeriesSorting"),
#endif
		Category("Data")
		]
		public SortingMode SeriesSorting {
			get { return chart.DataContainer.BoundSeriesSorting; }
			set { chart.DataContainer.BoundSeriesSorting = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSeriesTemplate"),
#endif
		Category("Data"),
		RefreshProperties(RefreshProperties.All),
		NestedTagProperty
		]
		public SeriesBase SeriesTemplate { get { return ChartDataContainer.SeriesTemplate; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlTitles"),
#endif
		Category("Elements"),
		Editor("DevExpress.XtraCharts.Design.ChartTitleEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty
		]
		public ChartTitleCollection Titles { get { return chart.Titles; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public AppearanceRepository AppearanceRepository { get { return chart.AppearanceRepository; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PaletteRepository PaletteRepository { get { return chart.PaletteRepository; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PaletteRepository IndicatorsPaletteRepository { get { return chart.IndicatorsPaletteRepository; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NestedTagProperty
		]
		public PaletteWrapperCollection PaletteWrappers {
			get { return new PaletteWrapperCollection(chart.PaletteRepository); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NestedTagProperty
		]
		public PaletteWrapperCollection IndicatorsPaletteWrappers {
			get { return new PaletteWrapperCollection(chart.IndicatorsPaletteRepository); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public double SideBySideBarDistanceVariable {
			get { return chart.SideBySideBarDistance; }
			set { chart.SideBySideBarDistance = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public int SideBySideBarDistanceFixed {
			get { return chart.SideBySideBarDistanceFixed; }
			set { chart.SideBySideBarDistanceFixed = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public bool SideBySideEqualBarWidth {
			get { return chart.SideBySideEqualBarWidth; }
			set { chart.SideBySideEqualBarWidth = value; }
		}
		[
		DefaultValue(null),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NestedTagProperty
		]
		public object FakeObject { get { return null; } set { } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlClientInstanceName"),
#endif
		Category("Client-Side"),
		DefaultValue(""),
		AutoFormatDisable
		]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlClientSideEvents"),
#endif
		Category("Client-Side"),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false)
		]
		public ChartClientSideEvents ClientSideEvents {
			get { return (ChartClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlEnableClientSideAPI"),
#endif
		Category("Client-Side"),
		DefaultValue(true),
		AutoFormatDisable
		]
		public bool EnableClientSideAPI {
			get { return EnableClientSideAPIInternal; }
			set { EnableClientSideAPIInternal = value; }
		}
		[
		Obsolete("This property is obsolete and isn't used at all."),
		DefaultValue(true),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public bool EnableClientSidePointToDiagram { get { return true; } set { } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlEnableCallBacks"),
#endif
		Category("Behavior"),
		DefaultValue(true),
		AutoFormatDisable
		]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set {
				EnableCallBacksInternal = value;
				base.AutoPostBackInternal = !EnableCallBacks;
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlEnableCallbackAnimation"),
#endif
		Category("Behavior"),
		DefaultValue(false),
		AutoFormatDisable
		]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlEnableCallbackCompression"),
#endif
		Category("Behavior"),
		DefaultValue(true),
		AutoFormatDisable
		]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSaveStateOnCallbacks"),
#endif
		Category("Behavior"),
		DefaultValue(true),
		AutoFormatDisable
		]
		public bool SaveStateOnCallbacks {
			get { return GetBoolProperty("SaveStateOnCallbacks", true); }
			set { SetBoolProperty("SaveStateOnCallbacks", true, value); }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSettingsLoadingPanel"),
#endif
		Category("Settings"),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay),
		AutoFormatDisable
		]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left),
		AutoFormatEnable
		]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText),
		AutoFormatEnable,
		Localizable(true)
		]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true),
		AutoFormatEnable
		]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true),
		AutoFormatDisable
		]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlLoadingPanelImage"),
#endif
		Category("Images"),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlLoadingPanelStyle"),
#endif
		Category("Styles"),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlLoadingDivStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public new LoadingDivStyle LoadingDivStyle {
			get { return base.LoadingDivStyle; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlClientVisible"),
#endif
		Category("Client-Side"),
		DefaultValue(true),
		AutoFormatDisable,
		Localizable(false)
		]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SelectionMode property instead."),
		DefaultValue(false)
		]
		public bool RuntimeSelection {
			get { return SelectionMode != ElementSelectionMode.None; }
			set { SelectionMode = value ? ElementSelectionMode.Single : ElementSelectionMode.None; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSelectionMode"),
#endif
		Category("Behavior"),
		DefaultValue(ElementSelectionMode.None)
		]
		public ElementSelectionMode SelectionMode {
			get { return chart.SelectionMode; }
			set { chart.SelectionMode = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSeriesSelectionMode"),
#endif
		Category("Behavior"),
		DefaultValue(SeriesSelectionMode.Series)
		]
		public SeriesSelectionMode SeriesSelectionMode {
			get { return chart.SeriesSelectionMode; }
			set { chart.SeriesSelectionMode = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlSelectedItems"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IList SelectedItems { get { return chart.SelectedItems; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlBinaryStorageMode"),
#endif
		DefaultValue(BinaryStorageMode.Default),
		AutoFormatDisable
		]
		public BinaryStorageMode BinaryStorageMode {
			get { return (BinaryStorageMode)GetEnumProperty("BinaryStorageMode", BinaryStorageMode.Default); }
			set { SetEnumProperty("BinaryStorageMode", BinaryStorageMode.Default, value); }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlRightToLeft"),
#endif
		Category(Categories.Layout),
		DefaultValue(DefaultBoolean.Default),
		AutoFormatDisable
		]
		public DefaultBoolean RightToLeft {
			get { return chart.RightToLeft; }
			set {
				RightToLeftInternal = value;
				chart.RightToLeft = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlAlternateText"),
#endif
		Category("Accessibility"),
		DefaultValue(""),
		Localizable(true),
		Bindable(true),
		AutoFormatDisable
		]
		public string AlternateText {
			get { return alternateText; }
			set { alternateText = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlCrosshairEnabled"),
#endif
		Category(Categories.Behavior),
		DefaultValue(DefaultBoolean.Default)
		]
		public DefaultBoolean CrosshairEnabled {
			get { return Chart.CrosshairEnabled; }
			set { Chart.CrosshairEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlToolTipEnabled"),
#endif
		Category(Categories.Behavior),
		DefaultValue(DefaultBoolean.Default)
		]
		public DefaultBoolean ToolTipEnabled {
			get { return Chart.ToolTipEnabled; }
			set { Chart.ToolTipEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlToolTipController"),
#endif
		Category(Categories.Behavior),
		NestedTagProperty,
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(ExpandableObjectConverter))
		]
		public ChartToolTipController ToolTipController { get { return toolTipController; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlCrosshairOptions"),
#endif
		Category(Categories.Behavior),
		NestedTagProperty,
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(CrosshairOptionsTypeConverter))
		]
		public CrosshairOptions CrosshairOptions { get { return Chart.CrosshairOptions; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlToolTipOptions"),
#endif
		Category(Categories.Behavior),
		NestedTagProperty,
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(ExpandableObjectConverter))
		]
		public ToolTipOptions ToolTipOptions { get { return Chart.ToolTipOptions; } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("WebChartControlDescriptionUrl"),
#endif
		Category("Accessibility"),
		DefaultValue(""),
		Localizable(false),
		AutoFormatDisable,
		Editor(typeof(UrlEditor), typeof(UITypeEditor)),
		UrlProperty
		]
		public string DescriptionUrl {
			get { return descriptionUrl; }
			set { descriptionUrl = value; }
		}
		[
		Category("Client-Side"),
		Browsable(false),
		AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dictionary<string, object> JSProperties { get { return JSPropertiesInternal; } }
		public WebChartControl() : base() {
			chart = new Chart(this);
			chart.Printer = new ChartPrinter(this);
			Width = 300;
			Height = 200;
			EnableCallBacks = true;
			diagramSerializerWrapper = new AspxSerializerWrapper<Diagram>(delegate() { return Diagram; },
																		  delegate(Diagram value) { Diagram = value; });
			toolTipController = new ChartToolTipController(Chart);
			((ISupportInitialize)this).BeginInit();
		}
		#region ISupportInitialize implementation
		public void BeginInit() {
			SetLoadingState(true);
			endLoadingCompleted = false;
			seriesSerializable = null;
		}
		public void EndInit() {
			if (!endLoadingCompleted) {
				if (DesignMode)
					AttributesSubstituter.Perform();
				if (seriesSerializable != null && seriesSerializable.Count > 0)
					ChartDataContainer.SeriesSerializable = seriesSerializable.ToArray();
				SetLoadingState(false);
				endLoadingCompleted = true;
				OnEndLoading(EventArgs.Empty);
			}
		}
		#endregion
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
		void ISupportBarsInteraction.RaiseUIUpdated() { }
		object IServiceProvider.GetService(Type serviceType) {
			IServiceProvider provider = ((IChartContainer)this).ServiceProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
		#endregion
		#region IChartDataProvider implementation
		bool IChartDataProvider.CanUseBoundPoints { get { return true; } }
		object IChartDataProvider.ParentDataSource { get { return null; } }
		DataContext IChartDataProvider.DataContext { get { return null; } }
		bool IChartDataProvider.SeriesDataSourceVisible { get { return false; } }
		bool IChartDataProvider.ShouldSerializeDataSource(object dataSource) {
			return dataSource != null;
		}
		void IChartDataProvider.OnBoundDataChanged(EventArgs e) {
			OnBoundDataChanged(e);
		}
		void IChartDataProvider.OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) {
			OnPivotGridSeriesExcluded(e);
		}
		void IChartDataProvider.OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) {
			OnPivotGridSeriesPointsExcluded(e);
		}
		#endregion
		#region IChartRenderProvider implementation
		Rectangle IChartRenderProvider.DisplayBounds { get { return new Rectangle(0, 0, (int)Width.Value, (int)Height.Value); } }
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		object IChartRenderProvider.LookAndFeel { get { return UserLookAndFeel.Default; } }
		IPrintable IChartRenderProvider.Printable { get { return chart; } }
		void IChartRenderProvider.Invalidate() {
			if (Site != null)
				InvalidateDesigner();   
		}
		void IChartRenderProvider.InvokeInvalidate() {
		}
		Bitmap IChartRenderProvider.LoadBitmap(string url) {
			if (!String.IsNullOrEmpty(url))
				try {
					string imageUrl = ResolveFileUrl(url, false);
					if (String.IsNullOrEmpty(imageUrl))
						using (WebClient wc = new WebClient())
						using (MemoryStream stream = new MemoryStream(wc.DownloadData(url)))
						using (Bitmap bitmap = new Bitmap(stream))
							return new Bitmap(bitmap);
					else
						return new Bitmap(imageUrl);
				}
				catch {
				}
			return null;
		}
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		#endregion
		#region IChartEventsProvider implementation
		bool IChartEventsProvider.ShouldCustomDrawAxisLabels { get { return Events[customDrawAxisLabel] != null || IsWebChartControlChild; } }
		bool IChartEventsProvider.ShouldCustomDrawSeriesPoints { get { return Events[customDrawSeriesPoint] != null || IsWebChartControlChild; } }
		bool IChartEventsProvider.ShouldCustomDrawSeries { get { return Events[customDrawSeries] != null || IsWebChartControlChild; } }
		void IChartEventsProvider.OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			OnCustomDrawSeries(e);
		}
		void IChartEventsProvider.OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			OnCustomDrawSeriesPoint(e);
		}
		void IChartEventsProvider.OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e) {
			OnCustomDrawAxisLabel(e);
		}
		void IChartEventsProvider.OnCustomPaint(CustomPaintEventArgs e) {
			OnCustomPaint(e);
		}
		void IChartEventsProvider.OnAxisScaleChanged(AxisScaleChangedEventArgs e) {
			OnAxisScaleChanged(e);
		}
		void IChartEventsProvider.OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) {
			OnAxisWholeRangeChanged(e);
		}
		void IChartEventsProvider.OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) {
			OnAxisVisualRangeChanged(e);
		}
		void IChartEventsProvider.OnCustomizeAutoBindingSettings(EventArgs e) {
			OnCustomizeAutoBindingSettings(e);
		}
		void IChartEventsProvider.OnCustomizeSimpleDiagramLayout(CustomizeSimpleDiagramLayoutEventArgs e) {
			OnCustomizeSimpleDiagramLayout(e);
		}
		void IChartEventsProvider.OnPivotChartingCustomizeXAxisLabels(CustomizeXAxisLabelsEventArgs e) {
			OnPivotChartingCustomizeXAxisLabels(e);
		}
		void IChartEventsProvider.OnPivotChartingCustomizeResolveOverlappingMode(CustomizeResolveOverlappingModeEventArgs e) {
			OnPivotChartingCustomizeResolveOverlappingMode(e);
		}
		void IChartEventsProvider.OnPivotChartingCustomizeLegend(CustomizeLegendEventArgs e) {
			OnPivotChartingCustomizeLegend(e);
		}
		#endregion
		#region IChartInteractionProvider implementation
		event HotTrackEventHandler IChartInteractionProvider.ObjectHotTracked { add { } remove { } }
		bool IChartInteractionProvider.HitTestingEnabled { get { return true; } }
		ElementSelectionMode IChartInteractionProvider.SelectionMode { get { return DesignMode ? ElementSelectionMode.None : SelectionMode; } }
		SeriesSelectionMode IChartInteractionProvider.SeriesSelectionMode { get { return SeriesSelectionMode; } }
		bool IChartInteractionProvider.EnableChartHitTesting { get { return true; } }
		bool IChartInteractionProvider.CanShowTooltips { get { return false; } }
		bool IChartInteractionProvider.DragCtrlKeyRequired { get { return false; } }
		bool IChartInteractionProvider.Capture { get { return false; } set { } }
		Point IChartInteractionProvider.PointToClient(Point p) {
			return p;
		}
		Point IChartInteractionProvider.PointToCanvas(Point p) {
			return p;
		}
		void IChartInteractionProvider.OnObjectSelected(HotTrackEventArgs e) {
			OnObjectSelected(e);
		}
		void IChartInteractionProvider.OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			OnSelectedItemsChanged(e);
		}
		void IChartInteractionProvider.OnObjectHotTracked(HotTrackEventArgs e) { }
		void IChartInteractionProvider.OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) { }
		void IChartInteractionProvider.OnScroll(ChartScrollEventArgs e) { }
		void IChartInteractionProvider.OnScroll3D(ChartScroll3DEventArgs e) { }
		void IChartInteractionProvider.OnZoom(ChartZoomEventArgs e) { }
		void IChartInteractionProvider.OnZoom3D(ChartZoom3DEventArgs e) { }
		void IChartInteractionProvider.OnQueryCursor(QueryCursorEventArgs e) { }
		void IChartInteractionProvider.OnLegendItemChecked(LegendItemCheckedEventArgs e) {
			OnLegendItemChecked(e);
		}
		void IChartInteractionProvider.OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
			OnPieSeriesPointExploded(e);
		}
		#endregion
		#region IChartContainer implementation
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return this; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return this; } }
		bool IChartContainer.ShowDesignerHints { get { return true; } }
		Chart IChartContainer.Chart { get { return chart; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return Site; } }
		IComponent IChartContainer.Parent { get { return base.Parent; } }
		bool IChartContainer.DesignMode { get { return DesignMode; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		ChartContainerType IChartContainer.ControlType { get { return ChartContainerType.WebControl; } }
		bool IChartContainer.Loading { get { return loading; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return true; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		void IChartContainer.LockChangeService() {
			this.lockChangeServiceCounter++;
		}
		void IChartContainer.UnlockChangeService() {
			this.lockChangeServiceCounter--;
		}
		void IChartContainer.Changing() {
			if (!WebLoadingHelper.GlobalLoading && this.lockChangeServiceCounter == 0 && Site != null) {
				IComponentChangeService cs = (IComponentChangeService)Site.GetService(typeof(IComponentChangeService));
				if (cs != null)
					cs.OnComponentChanging(this, null);
			}
		}
		void IChartContainer.Changed() {
			if (!WebLoadingHelper.GlobalLoading && lockChangeServiceCounter == 0 && endLoadingCompleted && Site != null) {
				IComponentChangeService cs = (IComponentChangeService)Site.GetService(typeof(IComponentChangeService));
				if (cs != null)
					cs.OnComponentChanged(this, null, null, null);
			}
		}
		void IChartContainer.ShowErrorMessage(string message, string title) {
			if (Site != null) {
				string actualTitle = string.IsNullOrEmpty(title) ? Site.Name : title;
				MessageBox.Show(message, actualTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		void IChartContainer.Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object min, object max, bool invalidate) { }
		bool IChartContainer.GetActualRightToLeft() {
			return IsRightToLeft();
		}
		#endregion
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
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this, true);
			foreach (Attribute attribute in attributes) {
				BrowsableAttribute browsable = attribute as BrowsableAttribute;
				if (browsable != null && browsable.Browsable)
					return properties;
			}
			return new WebChartControlPropertyDescriptorCollection(properties);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new WebChartControlPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true));
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
		#region ShouldSerialize
		bool ShouldSerializeDiagram() {
			return false;
		}
		bool ShouldSerializeDiagramTypeName() {
			return false;
		}
		bool ShouldSerializeDiagramSerializable() {
			return chart.ShouldSerializeDiagram();
		}
		bool ShouldSerializeBackColor() {
			return chart.ShouldSerializeBackColor();
		}
		bool ShouldSerializeSeriesDataMember() {
			return chart.DataContainer.ShouldSerializeSeriesDataMember();
		}
		bool ShouldSerializeSeriesSorting() {
			return chart.ShouldSerializeBoundSeriesSorting();
		}
		bool ShouldSerializeAppearanceName() {
			return chart.ShouldSerializeAppearanceName();
		}
		bool ShouldSerializeAppearanceNameSerializable() {
			return chart.ShouldSerializeAppearanceNameSerializable();
		}
		bool ShouldSerializePaletteName() {
			return chart.ShouldSerializePaletteName();
		}
		bool ShouldSerializePaletteBaseColorNumber() {
			return chart.ShouldSerializePaletteBaseColorNumber();
		}
		bool ShouldSerializeIndicatorsPaletteName() {
			return chart.ShouldSerializeIndicatorsPaletteName();
		}
		bool ShouldSerializeSideBySideBarDistanceVariable() {
			return chart.ShouldSerializeSideBySideBarDistance();
		}
		bool ShouldSerializeSideBySideBarDistanceFixed() {
			return chart.ShouldSerializeSideBySideBarDistanceFixed();
		}
		bool ShouldSerializeSideBySideEqualBarWidth() {
			return chart.ShouldSerializeSideBySideEqualBarWidth();
		}
		bool ShouldSerializeEmptyChartText() {
			return chart.ShouldSerializeEmptyChartText();
		}
		bool ShouldSerializeSmallChartText() {
			return chart.ShouldSerializeSmallChartText();
		}
		bool ShouldSerializeAutoBindingSettingsEnabled() {
			return false;
		}
		bool ShouldSerializeAutoLayoutSettingsEnabled() {
			return false;
		}
		bool ShouldSerializeToolTipOptions() {
			return Chart.ShouldSerializeToolTipOptions();
		}
		bool ShouldSerializeToolTipController() {
			return ToolTipController.ShouldSerializeProperties;
		}
		bool ShouldSerializeCrosshairOptions() {
			return chart.ShouldSerializeCrosshairOptions();
		}
		bool ShouldSerializeEnableClientSidePointToDiagram() {
			return false;
		}
		bool ShouldSerializeRuntimeSelection() {
			return false;
		}
		bool ShouldSerializeAutoLayout() {
			return chart.ShouldSerializeAutoLayout();
		}
		#endregion
		string CreateImage() {
			Rectangle bounds = new Rectangle(0, 0, (int)Width.Value, (int)Height.Value);
			if (bounds.Width <= 0 || bounds.Height <= 0)
				return String.Empty;
			Bitmap bitmap = chart.CreateBitmap(bounds.Size);
			if (bitmap == null)
				return String.Empty;
			MemoryStream stream = new MemoryStream();
			try {
				bitmap.Save(stream, ImageFormat);
				return BinaryStorage.GetImageUrl(this, stream.ToArray(), BinaryStorageMode);
			}
			catch {
				return String.Empty;
			}
			finally {
				stream.Dispose();
				bitmap.Dispose();
			}
		}
		string SaveChartState() {
			if (chart == null)
				return String.Empty;
			using (Stream stream = new MemoryStream()) {
				using (StreamReader reader = new StreamReader(stream)) {
					chart.SaveLayout(stream);
					stream.Seek(0, SeekOrigin.Begin);
					return reader.ReadToEnd();
				}
			}
		}
		void SerializeClientObjectModel(StringBuilder stb, ClientHitInfoSerializer hitInfoSerializer) {
			ClientWebChartControl clientControl = new ClientWebChartControl(this);
			if (hitInfoSerializer != null)
				clientControl.InitializeHitInfo(hitInfoSerializer.Objects, hitInfoSerializer.AdditionalObjects);
			clientControl.Serialize(stb);
		}
		void LoadChartState(string chartState) {
			if (chart == null)
				return;
			using (MemoryStream stream = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(stream)) {
					writer.Write(chartState);
					writer.Flush();
					stream.Seek(0L, SeekOrigin.Begin);
					chart.LoadLayout(stream);
				}
			}
		}
		void SetLoadingState(bool value) {
			loading = value;
			WebLoadingHelper.GlobalLoading = value;
			WebLoadingHelper.GlobalLoadingStatic = value;
		}
		[SecuritySafeCritical]
		void InvalidateDesigner() {
			IDesignerHost designerHost = Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if (designerHost != null) {
				ControlDesigner designer = designerHost.GetDesigner(this) as ControlDesigner;
				if (designer != null)
					designer.Invalidate();
			}
		}
		void CustomCallbackHandler(string param) {
			OnCustomCallback(new CustomCallbackEventArgs(param));
		}
		void SelectCallbackHandler(string x, string y, string shiftPressed, string ctrlPressed) {
			Rectangle bounds = new Rectangle(0, 0, (int)Width.Value, (int)Height.Value);
			if (bounds.Width > 0 && bounds.Height > 0) {
				using (System.Drawing.Image image = chart.CreateBitmap(bounds.Size)) {
					if (image != null) {
						int xCoord = Convert.ToInt32(x);
						int yCoord = Convert.ToInt32(y);
						bool isShiftPressed = Convert.ToBoolean(shiftPressed);
						bool isCtrlPressed = Convert.ToBoolean(ctrlPressed);
						Keys keyModifiers = Keys.None;
						if (isShiftPressed)
							keyModifiers |= Keys.Shift;
						if (isCtrlPressed)
							keyModifiers |= Keys.Control;
						chart.SelectObjectsAt(new Point(xCoord, yCoord), true, keyModifiers);
					}
				}
			}
		}
		void ChangeCheckedInLegendHandler(string legendItemId) {
			ILegendItem legendItem = GetLegendItemByPath(legendItemId);
			legendItem.CheckedInLegend = !legendItem.CheckedInLegend;
		}
		void LoadLayoutCallbackHandler(string serializedObjectModel) { 
			string json = serializedObjectModel.Substring("LOAD_LAYOUT:".Length);
			byte[] chartXML = JsonConverter.JsonToXml(json, false);
			using (Stream stream = new MemoryStream(chartXML)) {
				this.chart.LoadLayout(stream);
			}
		}
		ILegendItem GetLegendItemByPath(string path) {
			const string exceptionMessage = "The GetLegendItemByPath method can't give legendItem because of incorrect format of the path.";
			string[] elements = path.Split(serializationIDsSeparator);
			string[] elementIdAndNum = elements[0].Split(serializationIDAndIndexSeparator);
			switch (elementIdAndNum[0]) {
				case seriesSerializationID:
					int serIndex = int.Parse(elementIdAndNum[1]);
					Series series = Series[serIndex];
					if (elements.Length == 1)
						return series;
					else {
						string[] indIdAndNum = elements[1].Split(serializationIDAndIndexSeparator);
						int indicatorIndex = int.Parse(indIdAndNum[1]);
						XYDiagram2DSeriesViewBase seriesView = (XYDiagram2DSeriesViewBase)series.View;
						return seriesView.Indicators[indicatorIndex];
					}
				case primaryAxisXSerializationID:
					string[] constLineOrStripIdAndNum = elements[1].Split(serializationIDAndIndexSeparator);
					AxisX axisX = ((XYDiagram)Diagram).AxisX;
					switch (constLineOrStripIdAndNum[0]) {
						case constantLineSeriealizationID:
							int constantLineIndex = int.Parse(constLineOrStripIdAndNum[1]);
							return axisX.ConstantLines[constantLineIndex];
						case stripSeriealizationID:
							int stripIndex = int.Parse(constLineOrStripIdAndNum[1]);
							return axisX.Strips[stripIndex];
						default:
							throw new FormatException(exceptionMessage);
					}
				case secondaryAxisXSerializationID:
					int secondaryAxisIndex = int.Parse(elementIdAndNum[1]);
					SecondaryAxisX seconadaryAxisX = ((XYDiagram)Diagram).SecondaryAxesX[secondaryAxisIndex];
					string[] constLineOrStripIdAndNumForSecAxisX = elements[1].Split(serializationIDAndIndexSeparator);
					switch (constLineOrStripIdAndNumForSecAxisX[0]) {
						case constantLineSeriealizationID:
							int constantLineIndex = int.Parse(constLineOrStripIdAndNumForSecAxisX[1]);
							return seconadaryAxisX.ConstantLines[constantLineIndex];
						case stripSeriealizationID:
							int stripIndex = int.Parse(constLineOrStripIdAndNumForSecAxisX[1]);
							return seconadaryAxisX.Strips[stripIndex];
						default:
							throw new FormatException(exceptionMessage);
					}
				case primaryAxisYSerializationID:
					string[] constLineOrStripIdAndNumForAxisY = elements[1].Split(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					AxisY axisY = xyDiagram.AxisY;
					switch (constLineOrStripIdAndNumForAxisY[0]) {
						case constantLineSeriealizationID:
							int constantLineIndex = int.Parse(constLineOrStripIdAndNumForAxisY[1]);
							return axisY.ConstantLines[constantLineIndex];
						case stripSeriealizationID:
							int stripIndex = int.Parse(constLineOrStripIdAndNumForAxisY[1]);
							return axisY.Strips[stripIndex];
						default:
							throw new FormatException(exceptionMessage);
					}
				case secondaryAxisYSerializationID:
					int secondaryAxisIndexY = int.Parse(elementIdAndNum[1]);
					SecondaryAxisY seconadaryAxisY = ((XYDiagram)Diagram).SecondaryAxesY[secondaryAxisIndexY];
					string[] constLineOrStripIdAndNumForSecAxisY = elements[1].Split(serializationIDAndIndexSeparator);
					switch (constLineOrStripIdAndNumForSecAxisY[0]) {
						case constantLineSeriealizationID:
							int constantLineIndex = int.Parse(constLineOrStripIdAndNumForSecAxisY[1]);
							return seconadaryAxisY.ConstantLines[constantLineIndex];
						case stripSeriealizationID:
							int stripIndex = int.Parse(constLineOrStripIdAndNumForSecAxisY[1]);
							return seconadaryAxisY.Strips[stripIndex];
						default:
							throw new FormatException(exceptionMessage);
					}
			}
			return null;
		}
		ExportPrintingInfo CreateExportPrintingInfo(string filename, string format, bool saveToWindow, bool showPrintDialog) {
			string extension = format;
			string contentType;
			string disposition = saveToWindow ? DispositionTypeNames.Inline : DispositionTypeNames.Attachment;
			MemoryStream stream = new MemoryStream();
			switch (format) {
				case "pdf":
					contentType = "application/pdf";
					PdfExportOptions options = new PdfExportOptions();
					options.ShowPrintDialogOnOpen = showPrintDialog;
					chart.ExportToPdf(stream, options);
					break;
				case "xlsx":
					contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					chart.ExportToXlsx(stream);
					break;
				case "xls":
					contentType = "application/vnd.ms-excel";
					chart.ExportToXls(stream);
					break;
				case "rtf":
					contentType = "application/rtf";
					chart.ExportToRtf(stream);
					break;
				case "mht":
					contentType = "message/rfc822";
					chart.ExportToMht(stream, new MhtExportOptions());
					break;
				case "png":
					contentType = "image/png";
					chart.ExportToImage(stream, ImageFormat.Png);
					break;
				case "jpeg":
					extension = "jpg";
					contentType = "image/jpeg";
					chart.ExportToImage(stream, ImageFormat.Jpeg);
					break;
				case "bmp":
					contentType = "image/bmp";
					chart.ExportToImage(stream, ImageFormat.Bmp);
					break;
				case "tiff":
					contentType = "image/tiff";
					chart.ExportToImage(stream, ImageFormat.Tiff);
					break;
				case "gif":
					contentType = "image/gif";
					chart.ExportToImage(stream, ImageFormat.Gif);
					break;
				default:
					return null;
			}
			return new ExportPrintingInfo(stream, contentType, filename, extension, disposition);
		}
		void PrintingExportCallbackHandler(string command, string filename, string format, string printOptions, bool saveToWindow, bool showPrintDialog) {
			string actualFilename;
			if (!String.IsNullOrEmpty(filename))
				actualFilename = filename;
			else if (!String.IsNullOrEmpty(ClientInstanceName))
				actualFilename = ClientInstanceName;
			else
				actualFilename = "chart";
			ChartPrintingConfigurator.ConfigureChartPrintOptions(chart, printOptions);
			ExportPrintingInfo info = CreateExportPrintingInfo(actualFilename, format, saveToWindow, showPrintDialog);
			if (info != null && HttpContext.Current != null) {
				string url = BinaryStorage.GetResourceUrl(this, info.Stream.ToArray(), BinaryStorageMode, info.ContentType, info.ContentDisposition);
				callBackResult = String.Format("{0}:{1}", command, url);
			}
		}
		string GetCallbackStateString() {
			if (IsCallbackStateInitialize) {
				object obj = ((IStateManager)CallbackState).SaveViewState();
				if (obj != null)
					return new ObjectStateFormatter().Serialize(obj);
			}
			return String.Empty;
		}
		void SetCallbackStateString(string callbackString) {
			if (String.IsNullOrEmpty(callbackString))
				CallbackState.Clear();
			else {
				ObjectStateFormatter formatter = new ObjectStateFormatter();
				((IStateManager)CallbackState).LoadViewState(formatter.Deserialize(callbackString));
			}
		}
		void LoadCallbackState() {
			if (IsCallbackStateInitialize) {
				if (SaveStateOnCallbacks) {
					string chartState = (string)GetCallbackPropertyValue(callBackChartPropertyName, String.Empty);
					if (!String.IsNullOrEmpty(chartState))
						LoadChartState(chartState);
				}
				OnCallbackStateLoad(new CallbackStateLoadEventArgs(this));
			}
		}
		void SaveCallbackState() {
			if (SaveStateOnCallbacks) {
				string chartState = SaveChartState();
				if (String.IsNullOrEmpty(chartState))
					CallbackState.Clear();
				else
					SetCallbackPropertyValue(callBackChartPropertyName, String.Empty, chartState);
			}
			OnCallbackStateSave(new CallbackStateSaveEventArgs(this));
		}
		protected void SetDesignTimeProjectPath(string path) {
			dtProjectPath = path;
		}
		protected bool ProcessServerEvent(string eventArgument) {
			EnsureDataBound();
			callBackResult = string.Empty;
			string[] arguments = eventArgument.Split(new char[] { ':' });
			string callbackID = arguments == null || arguments.Length == 0 ? String.Empty : arguments[0];
			switch (callbackID) {
				case "CUSTOMCALLBACK":
					CustomCallbackHandler(eventArgument.Substring(callbackID.Length + 1));
					return true;
				case "SELECT":
					SelectCallbackHandler(arguments[1], arguments[2], arguments[3], arguments[4]);
					return true;
				case "PRINT":
					PrintingExportCallbackHandler(arguments[0], String.Empty, "pdf", arguments[1], true, true);
					return false;
				case "SAVETODISK":
					PrintingExportCallbackHandler(arguments[0], arguments[3], arguments[1], arguments[2], false, false);
					return false;
				case "SAVETOWINDOW":
					PrintingExportCallbackHandler(arguments[0], String.Empty, arguments[1], arguments[2], true, false);
					return false;
				case "CHANGE_CHECKED_IN_LEGEND":
					ChangeCheckedInLegendHandler(arguments[1]);
					return true;
				case "LOAD_LAYOUT":
					LoadLayoutCallbackHandler(eventArgument);
					return true;
				default:
					return true;
			}
		}
		protected virtual void OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			CustomDrawSeriesEventHandler handler = (CustomDrawSeriesEventHandler)this.Events[customDrawSeries];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			CustomDrawSeriesPointEventHandler handler = (CustomDrawSeriesPointEventHandler)this.Events[customDrawSeriesPoint];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e) {
			CustomDrawAxisLabelEventHandler handler = (CustomDrawAxisLabelEventHandler)this.Events[customDrawAxisLabel];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomPaint(CustomPaintEventArgs e) {
			CustomPaintEventHandler handler = (CustomPaintEventHandler)this.Events[customPaint];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnBoundDataChanged(EventArgs e) {
			BoundDataChangedEventHandler handler = (BoundDataChangedEventHandler)Events[boundDataChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
			PieSeriesPointExplodedEventHandler handler = (PieSeriesPointExplodedEventHandler)Events[pieSeriesPointExploded];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisScaleChanged(AxisScaleChangedEventArgs e) {
			EventHandler<AxisScaleChangedEventArgs> handler = (EventHandler<AxisScaleChangedEventArgs>)Events[axisScaleChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) {
			EventHandler<AxisRangeChangedEventArgs> handler = (EventHandler<AxisRangeChangedEventArgs>)Events[axisWholeRangeChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) {
			EventHandler<AxisRangeChangedEventArgs> handler = (EventHandler<AxisRangeChangedEventArgs>)Events[axisVisualRangeChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnEndLoading(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[endLoading];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnObjectSelected(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)Events[objectSelected];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			SelectedItemsChangedEventHandler handler = (SelectedItemsChangedEventHandler)Events[selectedItemsChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCallbackStateLoad(CallbackStateLoadEventArgs e) {
			CallbackStateLoadEventHandler handler = (CallbackStateLoadEventHandler)Events[callbackStateLoad];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCallbackStateSave(CallbackStateSaveEventArgs e) {
			CallbackStateSaveEventHandler handler = (CallbackStateSaveEventHandler)Events[callbackStateSave];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomCallback(CustomCallbackEventArgs e) {
			CustomCallbackEventHandler handler = (CustomCallbackEventHandler)Events[customCallback];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomizeAutoBindingSettings(EventArgs e) {
			CustomizeAutoBindingSettingsEventHandler handler = (CustomizeAutoBindingSettingsEventHandler)Events[customizeAutoBindingSettings];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomizeSimpleDiagramLayout(CustomizeSimpleDiagramLayoutEventArgs e) {
			CustomizeSimpleDiagramLayoutEventHandler handler = (CustomizeSimpleDiagramLayoutEventHandler)Events[customizeSimpleDiagramLayout];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotChartingCustomizeXAxisLabels(CustomizeXAxisLabelsEventArgs e) {
			CustomizeXAxisLabelsEventHandler handler = (CustomizeXAxisLabelsEventHandler)Events[pivotChartingCustomizeXAxisLabels];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotChartingCustomizeResolveOverlappingMode(CustomizeResolveOverlappingModeEventArgs e) {
			CustomizeResolveOverlappingModeEventHandler handler = (CustomizeResolveOverlappingModeEventHandler)Events[pivotChartingCustomizeResolveOverlappingMode];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotChartingCustomizeLegend(CustomizeLegendEventArgs e) {
			CustomizeLegendEventHandler handler = (CustomizeLegendEventHandler)Events[pivotChartingCustomizeLegend];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) {
			PivotGridSeriesExcludedEventHandler handler = (PivotGridSeriesExcludedEventHandler)Events[pivotGridSeriesExcluded];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) {
			PivotGridSeriesPointsExcludedEventHandler handler = (PivotGridSeriesPointsExcludedEventHandler)Events[pivotGridSeriesPointsExcluded];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnLegendItemChecked(LegendItemCheckedEventArgs e) {
			LegendItemCheckedEventHandler handler = (LegendItemCheckedEventHandler)Events[legendItemChecked];
			if (handler != null)
				handler(this, e);
		}
		protected override void InitInternal() {
			BinaryStorage.ProcessRequest();
			base.InitInternal();
		}
		protected override void OnInit(EventArgs e) {
			((ISupportInitialize)this).EndInit();
			base.OnInit(e);
		}
		protected override void ClearControlFields() {
			if (image != null) {
				image.Dispose();
				image = null;
			}
			if (panel != null) {
				panel.Dispose();
				panel = null;
			}
		}
		protected override void CreateControlHierarchy() {
			panel = RenderUtils.CreateDiv();
			image = RenderUtils.CreateImage();
			Controls.Add(panel);
			panel.Controls.Add(image);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			panel.ControlStyle.CopyFrom(ControlStyle);
			RenderUtils.AssignAttributes(this, panel);
			RenderUtils.SetVisibility(panel, IsClientVisible(), true);
			image.ID = imageID;
			image.GenerateEmptyAlternateText = true;
			image.ImageUrl = image.ResolveUrl(CreateImage());
			image.AlternateText = ActualAlternateText;
			image.DescriptionUrl = descriptionUrl;
			if (EnableClientSideAPI) {
				if (HasClientCustomDrawCrosshairEventHandler || HasClientObjectHotTrackedEventHandler || IsToolTipsEnabled || Chart.ActualCrosshairEnabled)
					RenderUtils.SetStringAttribute(panel, "onmousemove", String.Format(ChartClientSideEvents.ClientMouseMoveEventHandler, ClientID));
				if (HasClientObjectSelectedEventHandler || HasServerObjectSelectedEventHandler || (IsToolTipsEnabled && Diagram != null))
					RenderUtils.SetStringAttribute(panel, "onclick", String.Format(ChartClientSideEvents.ClientClickEventHandler, ClientID));
			}
			if (chart.ActualCrosshairEnabled || chart.ActualPointToolTipEnabled || chart.ActualSeriesToolTipEnabled)
				RenderUtils.AppendMSTouchDraggableClassNameIfRequired(image);
			base.PrepareControlHierarchy();
		}
		protected override void Render(HtmlTextWriter writer) {
			if (!endLoadingCompleted && DesignMode)   
				EndInit();
			if (writer is Html32TextWriter)
				writer = new HtmlTextWriter(writer.InnerWriter);
			base.Render(writer);
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ChartClientSideEvents();
		}
		protected override string GetSkinControlName() {
			return "Chart";
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new ChartDataHelper(this, name);
		}
		protected override bool HasDataInViewState() {
			return false;
		}
		protected override void LoadViewState(object savedState) {
			object[] states = (object[])savedState;
			base.LoadViewState(states[0]);
			string chartState = states[1] as string;
			if (chartState != null)
				LoadChartState(chartState);
		}
		protected override object SaveViewState() {
			object baseState = base.SaveViewState();
			object chartState = SaveChartState();
			return new object[] { baseState, chartState };
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasServerObjectSelectedEventHandler;
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || IsServerSideEventsAssigned() || HasCustomCallbackEventHandler;
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if (HasServerObjectSelectedEventHandler)
				eventNames.Add("ObjectSelected");
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(WebChartControl), "DevExpress.XtraCharts.Web.Scripts.WebChartCore.js");
			RegisterIncludeScript(typeof(WebChartControl), "DevExpress.XtraCharts.Web.Scripts.WebChartControl.js");
			RegisterIncludeScript(typeof(WebChartControl), "DevExpress.XtraCharts.Web.Scripts.WebChartCrosshairRenderer.js");
			RegisterIncludeScript(typeof(WebChartControl), "DevExpress.XtraCharts.Web.Scripts.WebChartPatternEditor.js");
			RegisterFormatterScript();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (EnableClientSideAPI) {
				stb.Append(localVarName + ".LoadHitInfo(");
				ClientHitInfoSerializer hitInfoSerializer = SerializeHitInfo(stb);
				stb.AppendLine(");");
				stb.Append(localVarName + ".InitObjectModel(");
				SerializeClientObjectModel(stb, hitInfoSerializer);
				stb.AppendLine(");");
			}
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(ClientStateProperties.CallbackState, GetCallbackStateString());
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientWebChartControl";
		}
		protected override bool EnableClientSideAPIInternal {
			get { return GetBoolProperty("EnableClientSideAPIInternal", true); }
			set { SetBoolProperty("EnableClientSideAPIInternal", true, value); }
		}
		protected string GetClientCallbackValueString() {
			return GetClientObjectStateValueString(ClientStateProperties.CallbackState);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			SetCallbackStateString(GetClientCallbackValueString());
			if (IsCallbackStateInitialize) {
				LoadCallbackState();
				CallbackState.Clear();
			}
			return false;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			LoadCallbackState();
			bool shouldCreateControlHierarchy = ProcessServerEvent(eventArgument);
			SaveCallbackState();
			if (shouldCreateControlHierarchy) {
				EnsureChildControls();
				PrepareControlHierarchy();
			}
		}
		protected override object GetCallbackResult() {
			Hashtable callbackResult = new Hashtable();
			if(!string.IsNullOrEmpty(callBackResult))
				callbackResult[CallbackResultProperties.Result] = callBackResult;
			else {
				callbackResult[CallbackResultProperties.StateObject] = GetClientObjectState();
				callbackResult["url"] = image.ImageUrl;
				StringBuilder stb = new StringBuilder();
				ClientHitInfoSerializer hitInfoSerializer = SerializeHitInfo(stb);
				callbackResult["hitInfo"] = stb.ToString();
				stb.Clear();
				SerializeClientObjectModel(stb, hitInfoSerializer);
				callbackResult["objectModel"] = stb.ToString();
			}
			return callbackResult;
		}
		protected override StylesBase CreateStyles() {
			return new WebChartControlStyles(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(WebChartControl), ChartDefaultCssResourceName);
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			ProcessServerEvent(eventArgument);
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			if (chart.ActualCrosshairEnabled)
				RegisterCultureInfoScript();
		}
		internal string ResolveFileUrl(string url, bool resolveForSave) {
			string resolvedUrl;
			if (url == String.Empty)
				return null;
			if (File.Exists(url))
				return url;
			if (url.StartsWith("~")) {
				resolvedUrl = Path.Combine(ProjectPath, url.TrimStart('~', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
				if (File.Exists(resolvedUrl) || resolveForSave)
					return resolvedUrl;
			}
			try {
				Uri uri = new Uri(url);
				if (File.Exists(uri.LocalPath) || resolveForSave)
					return uri.LocalPath;
			}
			catch {
			}
			return null;
		}
		protected internal string CalculatePathForRepresentedChartElement(ILegendItem representedElement) {
			StringBuilder path = new StringBuilder();
			if (representedElement is Series) {
				Series series = (Series)representedElement;
				path.Append(seriesSerializationID);
				path.Append(serializationIDAndIndexSeparator);
				path.Append(Series.IndexOf(series));
			}
			else if (representedElement is Indicator) {
				Series inicatorOwner = (Series)((IOwnedElement)representedElement).Owner.Owner;
				path.Append(seriesSerializationID);
				path.Append(serializationIDAndIndexSeparator);
				path.Append(Series.IndexOf((Series)inicatorOwner));
				path.Append(serializationIDsSeparator);
				path.Append(indicatorSeriealizationID);
				path.Append(serializationIDAndIndexSeparator);
				Indicator indicator = (Indicator)representedElement;
				XYDiagram2DSeriesViewBase seriesView = (XYDiagram2DSeriesViewBase)inicatorOwner.View;
				path.Append(seriesView.Indicators.IndexOf(indicator));
			}
			else if (representedElement is ConstantLine) {
				ConstantLine constantLine = (ConstantLine)representedElement;
				Axis constantLineOwner = ((IOwnedElement)constantLine).Owner as Axis;
				if (constantLineOwner is AxisX) {
					path.Append(primaryAxisXSerializationID);
					path.Append(serializationIDsSeparator);
					path.Append(constantLineSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.AxisX.ConstantLines.IndexOf(constantLine));
				}
				if (constantLineOwner is SecondaryAxisX) {
					SecondaryAxisX secondaryAxisX = (SecondaryAxisX)constantLineOwner;
					path.Append(secondaryAxisXSerializationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.SecondaryAxesX.IndexOf(secondaryAxisX));
					path.Append(serializationIDsSeparator);
					path.Append(constantLineSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					path.Append(secondaryAxisX.ConstantLines.IndexOf(constantLine));
				}
				if (constantLineOwner is AxisY) {
					path.Append(primaryAxisYSerializationID);
					path.Append(serializationIDsSeparator);
					path.Append(constantLineSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.AxisY.ConstantLines.IndexOf(constantLine));
				}
				if (constantLineOwner is SecondaryAxisY) {
					SecondaryAxisY secondaryAxisY = (SecondaryAxisY)constantLineOwner;
					path.Append(secondaryAxisYSerializationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.SecondaryAxesY.IndexOf(secondaryAxisY));
					path.Append(serializationIDsSeparator);
					path.Append(constantLineSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					path.Append(secondaryAxisY.ConstantLines.IndexOf(constantLine));
				}
			}
			else if (representedElement is Strip) {
				Strip strip = (Strip)representedElement;
				Axis constantLineOwner = ((IOwnedElement)strip).Owner as Axis;
				if (constantLineOwner is AxisX) {
					path.Append(primaryAxisXSerializationID);
					path.Append(serializationIDsSeparator);
					path.Append(stripSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.AxisX.Strips.IndexOf(strip));
				}
				if (constantLineOwner is SecondaryAxisX) {
					SecondaryAxisX secondaryAxisX = (SecondaryAxisX)constantLineOwner;
					path.Append(secondaryAxisXSerializationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.SecondaryAxesX.IndexOf(secondaryAxisX));
					path.Append(serializationIDsSeparator);
					path.Append(stripSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					path.Append(secondaryAxisX.Strips.IndexOf(strip));
				}
				if (constantLineOwner is AxisY) {
					path.Append(primaryAxisYSerializationID);
					path.Append(serializationIDsSeparator);
					path.Append(stripSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.AxisY.Strips.IndexOf(strip));
				}
				if (constantLineOwner is SecondaryAxisY) {
					SecondaryAxisY secondaryAxisY = (SecondaryAxisY)constantLineOwner;
					path.Append(secondaryAxisYSerializationID);
					path.Append(serializationIDAndIndexSeparator);
					XYDiagram xyDiagram = (XYDiagram)Diagram;
					path.Append(xyDiagram.SecondaryAxesY.IndexOf(secondaryAxisY));
					path.Append(serializationIDsSeparator);
					path.Append(stripSeriealizationID);
					path.Append(serializationIDAndIndexSeparator);
					path.Append(secondaryAxisY.Strips.IndexOf(strip));
				}
			}
			return path.ToString();
		}
		protected internal object GetCallbackPropertyValue(string propertyName, object defaultValue) {
			return ViewStateUtils.GetObjectProperty(CallbackState, propertyName, defaultValue);
		}
		protected internal void SetCallbackPropertyValue(string propertyName, object defaultValue, object value) {
			ViewStateUtils.SetObjectProperty(CallbackState, propertyName, defaultValue, value);
		}
		internal ClientHitInfoSerializer SerializeHitInfo(StringBuilder stb) {
			ClientHitInfoSerializer serializer = new ClientHitInfoSerializer(stb, this);
			serializer.Serialize(chart.HitTestController);
			return serializer;
		}
		public override void Dispose() {
			if (chart != null) {
				chart.Dispose();
				chart = null;
			}
			base.Dispose();
		}
		public Series GetSeriesByName(string seriesName) {
			return chart.DataContainer.GetSeriesByName(seriesName);
		}
		public string[] GetAppearanceNames() {
			return chart.AppearanceRepository.Names;
		}
		public string[] GetPaletteNames() {
			return chart.PaletteRepository.PaletteNames;
		}
		public PaletteEntry[] GetPaletteEntries(int count) {
			return chart.GetPaletteEntries(count);
		}
		[
		Obsolete("This method is obsolete now. Use the ResetLegendTextPattern method instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ResetLegendPointOptions() {
			chart.DataContainer.ResetLegendPointOptions();
		}
		public void ResetLegendTextPattern() {
			chart.DataContainer.ResetLegendTextPattern();
		}
		public void ClearSelection() {
			chart.ClearSelection(true);
		}
		public void SetObjectSelection(object obj) {
			chart.SetObjectSelection(obj, true);
		}
		public void RegisterSummaryFunction(string name, string displayName, int resultDimension, SummaryFunctionArgumentDescription[] argumentDescriptions, SummaryFunction function) {
			chart.RegisterSummaryFunction(name, displayName, null, resultDimension, argumentDescriptions, function);
		}
		public void RegisterSummaryFunction(string name, string displayName, ScaleType resultScaleType, int resultDimension, SummaryFunctionArgumentDescription[] argumentDescriptions, SummaryFunction function) {
			chart.RegisterSummaryFunction(name, displayName, resultScaleType, resultDimension, argumentDescriptions, function);
		}
		public void UnregisterSummaryFunction(string name) {
			chart.UnregisterSummaryFunction(name);
		}
		public void ResetSummaryFunctions() {
			chart.ResetSummaryFunctions();
		}
		public void RefreshData() {
			chart.RefreshData(true);
		}
		public void SaveToStream(Stream stream) {
			chart.SaveLayout(stream);
		}
		public void SaveToFile(string path) {
			string resolvedPath = ResolveFileUrl(path, true);
			if (string.IsNullOrEmpty(resolvedPath))
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPath), path));
			using (FileStream stream = new FileStream(resolvedPath, FileMode.Create, FileAccess.ReadWrite))
				SaveToStream(stream);
		}
		public string SaveToXml() {
			string result = null;
			using (Stream stream = new MemoryStream()) {
				using (StreamReader reader = new StreamReader(stream)) {
					SaveToStream(stream);
					stream.Seek(0, SeekOrigin.Begin);
					result = reader.ReadToEnd();
				}
			}
			return result;
		}
		public void LoadFromStream(Stream stream) {
			stream.Seek(0L, SeekOrigin.Begin);
			if (!XtraSerializingHelper.IsValidXml(stream))
				throw new LayoutStreamException();
			stream.Seek(0L, SeekOrigin.Begin);
			chart.LoadLayout(stream);
		}
		public void LoadFromXml(string layoutXml) {
			if (string.IsNullOrWhiteSpace(layoutXml))
				return;
			using (MemoryStream stream = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(stream)) {
					writer.Write(layoutXml);
					writer.Flush();
					LoadFromStream(stream);
				}
			}
		}
		public void LoadFromFile(string path) {
			string resolvedPath = ResolveFileUrl(path, false);
			if (string.IsNullOrEmpty(resolvedPath))
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPath), path));
			using (FileStream stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read))
				LoadFromStream(stream);
		}
		public void BindToData(ViewType viewType, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			chart.DataContainer.BindToData(viewType, dataSource, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(SeriesViewBase view, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			chart.DataContainer.BindToData(view, dataSource, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			chart.ExportToImage(stream, format);
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			string resolvedPath = ResolveFileUrl(filePath, true);
			if (string.IsNullOrEmpty(resolvedPath))
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPath), filePath));
			chart.ExportToImage(filePath, format);
		}
	}
	public class PaletteWrapper {
		Palette palette;
		[NestedTagProperty]
		public Palette Palette { get { return palette; } }
		public string Name {
			get { return palette.NameSerializable; }
			set { palette.NameSerializable = value; }
		}
		public PaletteScaleMode ScaleMode {
			get { return palette.ScaleModeSerializable; }
			set { palette.ScaleModeSerializable = value; }
		}
		public PaletteWrapper() {
			palette = new Palette(String.Empty);
		}
		public PaletteWrapper(Palette palette) {
			this.palette = palette;
		}
	}
	public class CallbackStateEventArgs : EventArgs {
		WebChartControl chart;
		public WebChartControl Chart { get { return chart; } }
		internal CallbackStateEventArgs(WebChartControl chart) {
			this.chart = chart;
		}
	}
	public delegate void CallbackStateLoadEventHandler(object sender, CallbackStateLoadEventArgs e);
	public class CallbackStateLoadEventArgs : CallbackStateEventArgs {
		internal CallbackStateLoadEventArgs(WebChartControl chart) : base(chart) {
		}
		public object GetPropertyValue(string propertyName) {
			return Chart.GetCallbackPropertyValue(propertyName, null);
		}
	}
	public delegate void CallbackStateSaveEventHandler(object sender, CallbackStateSaveEventArgs e);
	public class CallbackStateSaveEventArgs : CallbackStateEventArgs {
		internal CallbackStateSaveEventArgs(WebChartControl chart) : base(chart) {
		}
		public void SetPropertyValue(string propertyName, object value) {
			Chart.SetCallbackPropertyValue(propertyName, null, value);
		}
	}
	public class CustomCallbackEventArgs : EventArgs {
		string parameter;
		public string Parameter { get { return parameter; } }
		internal CustomCallbackEventArgs(string parameter) {
			this.parameter = parameter;
		}
	}
	public delegate void CustomCallbackEventHandler(object sender, CustomCallbackEventArgs e);
}
namespace DevExpress.XtraCharts.Native {
	public class ChartDataHelper : DataHelper {
		public new WebChartControl Control { get { return (WebChartControl)base.Control; } }
		public ChartDataHelper(WebChartControl control, string name)
			: base(control, name) {
		}
		protected override void PerformDataBinding(IEnumerable data) {
			base.PerformDataBinding(data);
			Control.Chart.DataContainer.DataSource = data;
		}
		public override void PerformSelect() {
			if (!Control.DesignMode)
				base.PerformSelect();
		}
	}
	public class ChartControlBuilder : ControlBuilder, IChartControlBuilder {
		string IChartControlBuilder.BorderTagName { get { return "BorderOptions"; } }
		Type IChartControlBuilder.BorderType { get { return typeof(RectangularBorder); } }
	}
	[Serializable]
	public class ExportPrintingInfo : IDisposable {
		MemoryStream stream;
		string contentType;
		string contentDisposition;
		public MemoryStream Stream { get { return stream; } }
		public string ContentType { get { return contentType; } }
		public string ContentDisposition { get { return contentDisposition; } }
		public ExportPrintingInfo(MemoryStream stream, string contentType, string filename, string extension, string disposition) {
			this.stream = stream;
			this.contentType = contentType;
			contentDisposition = String.Format("{0};filename={1}.{2}", disposition, filename, extension);
		}
		public void Dispose() {
			if (stream != null)
				stream.Close();
		}
	}
	public static class ChartPrintingConfigurator {
		const int optionsCount = 10;
		static int ParseInt(string inputString, int defaultValue) {
			int value;
			return Int32.TryParse(inputString, out value) ? value : defaultValue;
		}
		static void SetPaperSize(ChartPageSettings pageSettings, string kind, string customName, string customWidth, string customHeight) {
			PaperKind paperKind;
			if (Enum.TryParse<PaperKind>(kind, out paperKind)) {
				pageSettings.PaperKind = paperKind;
				if (paperKind == PaperKind.Custom) {
					int width = ParseInt(customWidth, pageSettings.CustomPaperSize.Width);
					int height = ParseInt(customHeight, pageSettings.CustomPaperSize.Height);
					pageSettings.CustomPaperSize = new Size(width, height);
					pageSettings.CustomPaperName = customName;
				}
			}
		}
		static System.Drawing.Printing.Margins ParsePageMargins(string marginLeft, string marginTop, string marginRight, string marginBottom, System.Drawing.Printing.Margins defaultMargins) {
			System.Drawing.Printing.Margins margins = new System.Drawing.Printing.Margins();
			margins.Left = ParseInt(marginLeft, defaultMargins.Left);
			margins.Top = ParseInt(marginTop, defaultMargins.Top);
			margins.Right = ParseInt(marginRight, defaultMargins.Right);
			margins.Bottom = ParseInt(marginBottom, defaultMargins.Bottom);
			return margins;
		}
		public static void ConfigureChartPrintOptions(Chart chart, string printOptions) {
			string[] printOptionsArray = printOptions.Split('&');
			if (printOptionsArray.Length != optionsCount)
				return;
			PrintSizeMode chartSizeMode;
			if (Enum.TryParse<PrintSizeMode>(printOptionsArray[0], out chartSizeMode))
				chart.OptionsPrint.SizeMode = chartSizeMode;
			ChartPageSettings pageSettings = new ChartPageSettings();
			bool landscape;
			if (Boolean.TryParse(printOptionsArray[1], out landscape))
				pageSettings.Landscape = landscape;
			SetPaperSize(pageSettings, printOptionsArray[2], printOptionsArray[3], printOptionsArray[4], printOptionsArray[5]);
			pageSettings.Margins = ParsePageMargins(printOptionsArray[6], printOptionsArray[7], printOptionsArray[8], printOptionsArray[9], pageSettings.Margins);
			((IChartPrinter)chart.Printer).SetPageSettings(pageSettings);
		}
	}
}
