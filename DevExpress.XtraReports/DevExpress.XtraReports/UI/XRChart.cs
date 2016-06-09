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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Data.Design;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Design;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRDesigner("DevExpress.XtraReports.Design.XRChartDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRChartDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	TypeConverter("DevExpress.XtraCharts.Design.ChartTypeConverter," + AssemblyInfo.SRAssemblyCharts + AssemblyInfo.FullAssemblyVersionExtension),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "XRChart.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRChart", "Chart"),
	XRToolboxSubcategoryAttribute(2, 0),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRChart.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRChart.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRChart : XRControl, IChartContainer, IChartRenderProvider, IChartDataProvider, IChartEventsProvider, IChartInteractionProvider, ISupportInitialize, ICoreReference, IDataContainer {
		static PaddingInfo ToPaddingInfo(RectangleIndents indents) {
			return new PaddingInfo(indents.Left, indents.Right, indents.Top, indents.Bottom);
		}
		static void SetIndents(RectangleIndents indents, PaddingInfo padding) {
			indents.Left = padding.Left;
			indents.Top = padding.Top;
			indents.Right = padding.Right;
			indents.Bottom = padding.Bottom;
		}
		new public static class EventNames {
			public const string CustomDrawSeries = "CustomDrawSeries";
			public const string CustomDrawCrosshair = "CustomDrawCrosshair";
			public const string CustomDrawSeriesPoint = "CustomDrawSeriesPoint";
			public const string CustomDrawAxisLabel = "CustomDrawAxisLabel";
			public const string CustomPaint = "CustomPaint";
			public const string BoundDataChanged = "BoundDataChanged";
			public const string PieSeriesPointExploded = "PieSeriesPointExploded";
			public const string AxisScaleChanged = "AxisScaleChanged";
			public const string AxisVisualRangeChanged = "AxisVisualRangeChanged";
			public const string AxisWholeRangeChanged = "AxisWholeRangeChanged";
			public const string CustomizeAutoBindingSettings = "CustomizeAutoBindingSettings";
			public const string CustomizeSimpleDiagramLayout = "CustomizeSimpleDiagramLayout";
			public const string PivotChartingCustomizeXAxisLabels = "PivotChartingCustomizeXAxisLabels";
			public const string PivotChartingCustomizeResolveOverlappingMode = "PivotChartingCustomizeResolveOverlappingMode";
			public const string PivotChartingCustomizeLegend = "PivotChartingCustomizeLegend";
			public const string PivotGridSeriesExcluded = "PivotGridSeriesExcluded";
			public const string PivotGridSeriesPointsExcluded = "PivotGridSeriesPointsExcluded";
		}
		static readonly object objectSelected = new object();
		static readonly object selectedItemsChanged = new object();
		static readonly object objectHotTracked = new object();
		static readonly object CustomDrawSeriesEvent = new object();
		static readonly object CustomDrawCrosshairEvent = new object();
		static readonly object CustomDrawSeriesPointEvent = new object();
		static readonly object CustomDrawAxisLabelEvent = new object();
		static readonly object CustomPaintEvent = new object();
		static readonly object BoundDataChangedEvent = new object();
		static readonly object PieSeriesPointExplodedEvent = new object();
		static readonly object CustomizeAutoBindingSettingsEvent = new object();
		static readonly object CustomizeSimpleDiagramLayoutEvent = new object();
		static readonly object PivotChartingCustomizeXAxisLabelsEvent = new object();
		static readonly object PivotChartingCustomizeResolveOverlappingModeEvent = new object();
		static readonly object PivotChartingCustomizeLegendEvent = new object();
		static readonly object PivotGridSeriesExcludedEvent = new object();
		static readonly object PivotGridSeriesPointsExcludedEvent = new object();
		static readonly object endLoading = new object();
		static readonly object AxisScaleChangedEvent = new object();
		static readonly object AxisWholeRangeChangedEvent = new object();
		static readonly object AxisVisualRangeChangedEvent = new object();
		Chart chart;
		DataContainer DataContainer { get { return chart.DataContainer; } }
		ReportDataContext dataContext;
		bool loading;
		int lockChangeServiceCounter;
		ChartBitmapContainer3D bitmapContainer;
		ChartImageType imageType = ChartImageType.Metafile;
		bool IsEUD { get { return DesignTool.IsEndUserDesigner(Site); } }
		bool HasCustomDrawAxisLabelsListeners { get { return Events[CustomDrawAxisLabelEvent] != null || ContainsEventScript(EventNames.CustomDrawAxisLabel); } }
		bool HasCustomDrawSeriesListeners { get { return Events[CustomDrawSeriesEvent] != null || ContainsEventScript(EventNames.CustomDrawSeries); } }
		bool HasCustomDrawSeriesPointsListeners { get { return Events[CustomDrawSeriesPointEvent] != null || ContainsEventScript(EventNames.CustomDrawSeriesPoint); } }
		Image Image {
			get {
				Size size = ((IChartRenderProvider)this).DisplayBounds.Size;
				if (chart.Is3DDiagram) {
					if (DesignMode)
						return CreateLowQualityBitmap(size);
					else
						return this.chart.CreateBitmap(size);
				} else {
					if (this.imageType == ChartImageType.Metafile)
						return this.chart.CreateMetafile(size, MetafileFrameUnit.Pixel);
					else
						return this.chart.CreateBitmap(size);
				}
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public Chart Chart { get { return chart; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartAutoLayout"),
#endif
		SRCategory(ReportStringId.CatLayout),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.AutoLayout"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool AutoLayout {
			get { return chart.AutoLayout; }
			set { chart.AutoLayout = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartAnnotations"),
#endif
		SRCategory(ReportStringId.CatElements),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.Annotations"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))
		]
		public AnnotationCollection Annotations { get { return chart.Annotations; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartAnnotationRepository"),
#endif
		SRCategory(ReportStringId.CatElements),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.AnnotationRepository"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))
		]
		public AnnotationRepository AnnotationRepository { get { return chart.AnnotationRepository; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XRChartScripts Scripts { get { return (XRChartScripts)fEventScripts; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartSeriesTemplate"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.SeriesTemplate"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		RefreshProperties(RefreshProperties.All)
		]
		public SeriesBase SeriesTemplate {
			get { return DataContainer.SeriesTemplate; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartSeriesNameTemplate"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.SeriesNameTemplate"),
		SRCategory(ReportStringId.CatData),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SeriesNameTemplate SeriesNameTemplate {
			get { return DataContainer.SeriesNameTemplate; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartSeries"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.Series"),
		SRCategory(ReportStringId.CatElements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.SeriesCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(System.Drawing.Design.UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
		]
		public SeriesCollection Series { get { return DataContainer.Series; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public Series[] SeriesSerializable {
			get { return DataContainer.SeriesSerializable; }
			set { DataContainer.SeriesSerializable = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartDiagram"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.Diagram"),
		SRCategory(ReportStringId.CatElements),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public Diagram Diagram {
			get { return chart.Diagram; }
			set { chart.Diagram = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartLegend"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.Legend"),
		SRCategory(ReportStringId.CatElements),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public Legend Legend { get { return chart.Legend; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.FillStyle"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle {
			get { return chart.FillStyle; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.BackColor"),
		SRCategory(ReportStringId.CatAppearance),
		Browsable(true)
		]
		public new Color BackColor {
			get { return chart.BackColor; }
			set { chart.BackColor = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartDataAdapter"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.DataAdapter"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.DataAdapterEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		TypeConverterAttribute(typeof(DevExpress.XtraReports.Design.DataAdapterConverter))
		]
		public object DataAdapter {
			get { return DataContainer.DataAdapter; }
			set { DataContainer.DataAdapter = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRChartPadding")]
#endif
		public override PaddingInfo Padding {
			get { return ToPaddingInfo(chart.Padding); }
			set { SetIndents(chart.Padding, value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartDataSource"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.DataSource"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public object DataSource {
			get { return DataContainer.DataSource; }
			set { DataContainer.DataSource = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.DataMember"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.Repaint),
		TypeConverter(typeof(DevExpress.XtraReports.Design.DataMemberTypeConverter)),
		Editor("DevExpress.XtraReports.Design.DataContainerDataMemberEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		]
		public string DataMember {
			get { return DataContainer.DataMember; }
			set { DataContainer.DataMember = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartEmptyChartText"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.EmptyChartText"),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public EmptyChartText EmptyChartText { get { return Chart.EmptyChartText; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartSmallChartText"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.SmallChartText"),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SmallChartText SmallChartText { get { return Chart.SmallChartText; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartSeriesDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.SeriesDataMember"),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public string SeriesDataMember {
			get { return DataContainer.SeriesDataMember; }
			set { DataContainer.SeriesDataMember = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartSeriesSorting"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.SeriesSorting"),
		SRCategory(ReportStringId.CatData),
		]
		public SortingMode SeriesSorting {
			get { return chart.DataContainer.BoundSeriesSorting; }
			set { chart.DataContainer.BoundSeriesSorting = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartAppearanceName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.AppearanceName"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter("DevExpress.XtraCharts.Design.AppearanceTypeConverter," + AssemblyInfo.SRAssemblyCharts + AssemblyInfo.FullAssemblyVersionExtension)
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
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public AppearanceRepository AppearanceRepository { get { return chart.AppearanceRepository; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartPaletteName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.PaletteName"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraCharts.Design.PaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts + AssemblyInfo.FullAssemblyVersionExtension)
		]
		public string PaletteName {
			get { return chart.PaletteName; }
			set { chart.PaletteName = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PaletteRepository PaletteRepository {
			get { return chart.PaletteRepository; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartPaletteBaseColorNumber"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.PaletteBaseColorNumber"),
		SRCategory(ReportStringId.CatAppearance),
		]
		public int PaletteBaseColorNumber {
			get { return chart.PaletteBaseColorNumber; }
			set { chart.PaletteBaseColorNumber = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartIndicatorsPaletteName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.IndicatorsPaletteName"),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraCharts.Design.PaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts + AssemblyInfo.FullAssemblyVersionExtension)
		]
		public string IndicatorsPaletteName {
			get { return chart.IndicatorsPaletteName; }
			set { chart.IndicatorsPaletteName = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PaletteRepository IndicatorsPaletteRepository {
			get { return chart.IndicatorsPaletteRepository; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartTitles"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.Titles"),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
		SRCategory(ReportStringId.CatElements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.ChartTitleEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(System.Drawing.Design.UITypeEditor))
		]
		public ChartTitleCollection Titles { get { return chart.Titles; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartBackImage"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.BackImage"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public BackgroundImage BackImage {
			get { return chart.BackImage; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public double SideBySideBarDistanceVariable {
			get { return chart.SideBySideBarDistance; }
			set { chart.SideBySideBarDistance = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public int SideBySideBarDistanceFixed {
			get { return chart.SideBySideBarDistanceFixed; }
			set { chart.SideBySideBarDistanceFixed = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool SideBySideEqualBarWidth {
			get { return chart.SideBySideEqualBarWidth; }
			set { chart.SideBySideEqualBarWidth = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartImageType"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.ImageType"),
		DefaultValue(ChartImageType.Metafile),
		SRCategory(ReportStringId.CatAppearance),
		]
		public ChartImageType ImageType {
			get { return imageType; }
			set { imageType = value; }
		}
		[
		Obsolete("The XRChart.AutoBindingSettingsEnabled property is now obsolete. Use the XRChart.PivotGridDataSourceOptions.AutoBindingSettingsEnabled property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(true),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool AutoBindingSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled = value; }
		}
		[
		Obsolete("The XRChart.AutoLayoutSettingsEnabled property is now obsolete. Use the XRChart.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(true),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool AutoLayoutSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRChartPivotGridDataSourceOptions"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRChart.PivotGridDataSourceOptions"),
		SRCategory(ReportStringId.CatData),
		TypeConverter(typeof(PivotGridDataSourceOptionsTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PivotGridDataSourceOptions PivotGridDataSourceOptions { get { return DataContainer.PivotGridDataSourceOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool KeepTogether {
			get { return base.KeepTogether; }
			set { base.KeepTogether = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override XRControlStyles Styles { get { return base.Styles; } }
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRChartCustomDrawSeries")]
#endif
		public event CustomDrawSeriesEventHandler CustomDrawSeries {
			add { Events.AddHandler(CustomDrawSeriesEvent, value); }
			remove { Events.RemoveHandler(CustomDrawSeriesEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRChartCustomDrawCrosshair")]
#endif
		public event CustomDrawCrosshairEventHandler CustomDrawCrosshair {
			add { Events.AddHandler(CustomDrawCrosshairEvent, value); }
			remove { Events.RemoveHandler(CustomDrawCrosshairEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRChartCustomDrawSeriesPoint")]
#endif
		public event CustomDrawSeriesPointEventHandler CustomDrawSeriesPoint {
			add { Events.AddHandler(CustomDrawSeriesPointEvent, value); }
			remove { Events.RemoveHandler(CustomDrawSeriesPointEvent, value); }
		}
		public event CustomDrawAxisLabelEventHandler CustomDrawAxisLabel {
			add { Events.AddHandler(CustomDrawAxisLabelEvent, value); }
			remove { Events.RemoveHandler(CustomDrawAxisLabelEvent, value); }
		}
		public event CustomPaintEventHandler CustomPaint {
			add { Events.AddHandler(CustomPaintEvent, value); }
			remove { Events.RemoveHandler(CustomPaintEvent, value); }
		}
		public event BoundDataChangedEventHandler BoundDataChanged {
			add { Events.AddHandler(BoundDataChangedEvent, value); }
			remove { Events.RemoveHandler(BoundDataChangedEvent, value); }
		}
		public event PieSeriesPointExplodedEventHandler PieSeriesPointExploded {
			add { Events.AddHandler(PieSeriesPointExplodedEvent, value); }
			remove { Events.RemoveHandler(PieSeriesPointExplodedEvent, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		public event CustomizeAutoBindingSettingsEventHandler CustomizeAutoBindingSettings {
			add { Events.AddHandler(CustomizeAutoBindingSettingsEvent, value); }
			remove { Events.RemoveHandler(CustomizeAutoBindingSettingsEvent, value); }
		}
		public event CustomizeSimpleDiagramLayoutEventHandler CustomizeSimpleDiagramLayout {
			add { Events.AddHandler(CustomizeSimpleDiagramLayoutEvent, value); }
			remove { Events.RemoveHandler(CustomizeSimpleDiagramLayoutEvent, value); }
		}
		public event PivotGridSeriesExcludedEventHandler PivotGridSeriesExcluded {
			add { Events.AddHandler(PivotGridSeriesExcludedEvent, value); }
			remove { Events.RemoveHandler(PivotGridSeriesExcludedEvent, value); }
		}
		public event PivotGridSeriesPointsExcludedEventHandler PivotGridSeriesPointsExcluded {
			add { Events.AddHandler(PivotGridSeriesPointsExcludedEvent, value); }
			remove { Events.RemoveHandler(PivotGridSeriesPointsExcludedEvent, value); }
		}
		public event EventHandler<AxisScaleChangedEventArgs> AxisScaleChanged {
			add { Events.AddHandler(AxisScaleChangedEvent, value); }
			remove { Events.RemoveHandler(AxisScaleChangedEvent, value); }
		}
		public event EventHandler<AxisRangeChangedEventArgs> AxisWholeRangeChanged {
			add { Events.AddHandler(AxisWholeRangeChangedEvent, value); }
			remove { Events.RemoveHandler(AxisWholeRangeChangedEvent, value); }
		}
		public event EventHandler<AxisRangeChangedEventArgs> AxisVisualRangeChanged {
			add { Events.AddHandler(AxisVisualRangeChangedEvent, value); }
			remove { Events.RemoveHandler(AxisVisualRangeChangedEvent, value); }
		}
		public event CustomizeXAxisLabelsEventHandler PivotChartingCustomizeXAxisLabels {
			add { Events.AddHandler(PivotChartingCustomizeXAxisLabelsEvent, value); }
			remove { Events.RemoveHandler(PivotChartingCustomizeXAxisLabelsEvent, value); }
		}
		public event CustomizeResolveOverlappingModeEventHandler PivotChartingCustomizeResolveOverlappingMode {
			add { Events.AddHandler(PivotChartingCustomizeResolveOverlappingModeEvent, value); }
			remove { Events.RemoveHandler(PivotChartingCustomizeResolveOverlappingModeEvent, value); }
		}
		public event CustomizeLegendEventHandler PivotChartingCustomizeLegend {
			add { Events.AddHandler(PivotChartingCustomizeLegendEvent, value); }
			remove { Events.RemoveHandler(PivotChartingCustomizeLegendEvent, value); }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeXAxisLabelsEventHandler CustomizeXAxisLabels {
			add { Events.AddHandler(PivotChartingCustomizeXAxisLabelsEvent, value); }
			remove { Events.RemoveHandler(PivotChartingCustomizeXAxisLabelsEvent, value); }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeResolveOverlappingModeEventHandler CustomizeResolveOverlappingMode {
			add { Events.AddHandler(PivotChartingCustomizeResolveOverlappingModeEvent, value); }
			remove { Events.RemoveHandler(PivotChartingCustomizeResolveOverlappingModeEvent, value); }
		}
		[
		Obsolete("The CustomizeXAxisLabels event is obsolete now. Use the PivotChartingCustomizeXAxisLabels event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeLegendEventHandler CustomizeLegend {
			add { Events.AddHandler(PivotChartingCustomizeLegendEvent, value); }
			remove { Events.RemoveHandler(PivotChartingCustomizeLegendEvent, value); }
		}
		public XRChart() {
			WidthF = 300;
			HeightF = 200;
			chart = new Chart(this);
			chart.Border.Visibility = DefaultBoolean.False;
			Borders = XRControlStyle.Default.Borders;
			BorderColor = XRControlStyle.Default.BorderColor;
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
		void ISupportBarsInteraction.RaiseUIUpdated() { }
		object IServiceProvider.GetService(Type serviceType) {
			IServiceProvider provider = ((IChartContainer)this).ServiceProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
		#endregion
		#region IChartDataProvider implementation
		DataContext IChartDataProvider.DataContext {
			get {
				if(dataContext == null || dataContext.IsDisposed)
					dataContext = !ReportIsLoading ? Report.GetChildDataContext(this, DataSource, DataMember) : null;
				return dataContext;
			}
		}
		object IChartDataProvider.ParentDataSource {
			get { return ParentDataSource; }
		}
		protected virtual object ParentDataSource {
			get { return Report != null ? Report.GetEffectiveDataSource() : null; }
		}
		bool IChartDataProvider.CanUseBoundPoints { get { return !DesignMode; } }
		bool IChartDataProvider.SeriesDataSourceVisible { get { return true; } }
		bool IChartDataProvider.ShouldSerializeDataSource(object dataSource) {
			return dataSource is IComponent;
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
		Rectangle IChartRenderProvider.DisplayBounds {
			get {
				RectangleF rect = DeflateBorderWidth(ClientRectangleF);
				rect = XRConvert.Convert(rect, Dpi, GraphicsDpi.DeviceIndependentPixel);
				return new Rectangle(Point.Empty, System.Drawing.Size.Round(rect.Size));
			}
		}
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		object IChartRenderProvider.LookAndFeel {
			get {
				IChartService serv = GetChartService();
				return serv != null ? serv.GetLookAndFeel(Site) : null;
			}
		}
		IPrintable IChartRenderProvider.Printable { get { return null; } }
		void IChartRenderProvider.Invalidate() {
			IChartService serv = GetChartService();
			if (serv != null)
				serv.Invalidate(this);
		}
		void IChartRenderProvider.InvokeInvalidate() {
			((IChartRenderProvider)this).Invalidate();
		}
		Bitmap IChartRenderProvider.LoadBitmap(string url) {
			return null;
		}
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		#endregion
		#region IChartEventsProvider implementation
		bool IChartEventsProvider.ShouldCustomDrawSeries { get { return HasCustomDrawSeriesListeners; } }
		bool IChartEventsProvider.ShouldCustomDrawSeriesPoints { get { return HasCustomDrawSeriesPointsListeners; } }
		bool IChartEventsProvider.ShouldCustomDrawAxisLabels { get { return HasCustomDrawAxisLabelsListeners; } }
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
		void IChartEventsProvider.OnAxisScaleChanged(AxisScaleChangedEventArgs e) {
			OnAxisScaleChanged(e);
		}
		void IChartEventsProvider.OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) {
			OnAxisVisualRangeChanged(e);
		}
		void IChartEventsProvider.OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) {
			OnAxisWholeRangeChanged(e);
		}
		#endregion
		#region IChartInteractionProvider implementation
		event HotTrackEventHandler IChartInteractionProvider.ObjectSelected {
			add { Events.AddHandler(objectSelected, value); }
			remove { Events.RemoveHandler(objectSelected, value); }
		}
		event SelectedItemsChangedEventHandler IChartInteractionProvider.SelectedItemsChanged {
			add { Events.AddHandler(selectedItemsChanged, value); }
			remove { Events.RemoveHandler(selectedItemsChanged, value); }
		}
		event HotTrackEventHandler IChartInteractionProvider.ObjectHotTracked {
			add { Events.AddHandler(objectHotTracked, value); }
			remove { Events.RemoveHandler(objectHotTracked, value); }
		}
		bool IChartInteractionProvider.EnableChartHitTesting { get { return false; } }
		bool IChartInteractionProvider.CanShowTooltips { get { return DesignMode; } }
		bool IChartInteractionProvider.DragCtrlKeyRequired { get { return DesignMode; } }
		bool IChartInteractionProvider.Capture { get { return false; } set { } }
		bool IChartInteractionProvider.HitTestingEnabled { get { return DesignMode; } }
		ElementSelectionMode IChartInteractionProvider.SelectionMode { get { return DesignMode ? ElementSelectionMode.Single : ElementSelectionMode.None ; } }
		SeriesSelectionMode IChartInteractionProvider.SeriesSelectionMode { get { return SeriesSelectionMode.Series; } }
		Point IChartInteractionProvider.PointToClient(Point p) {
			IChartService serv = GetChartService();
			return serv == null ? p : serv.PointToClient(this, p);
		}
		Point IChartInteractionProvider.PointToCanvas(Point p) {
			IChartService serv = GetChartService();
			return serv == null ? p : serv.PointToCanvas(this, p);
		}
		void IChartInteractionProvider.OnObjectHotTracked(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)this.Events[objectHotTracked];
			if (handler != null)
				handler(this, e);
		}
		void IChartInteractionProvider.OnObjectSelected(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)this.Events[objectSelected];
			if (handler != null)
				handler(this, e);
		}
		void IChartInteractionProvider.OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			SelectedItemsChangedEventHandler handler = (SelectedItemsChangedEventHandler)this.Events[selectedItemsChanged];
			if (handler != null)
				handler(this, e);
		}
		void IChartInteractionProvider.OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			OnCustomDrawCrosshair(e);
		}
		void IChartInteractionProvider.OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
			OnPieSeriesPointExploded(e);
		}
		void IChartInteractionProvider.OnLegendItemChecked(LegendItemCheckedEventArgs e) { }
		void IChartInteractionProvider.OnScroll(ChartScrollEventArgs e) { }
		void IChartInteractionProvider.OnScroll3D(ChartScroll3DEventArgs e) { }
		void IChartInteractionProvider.OnZoom(ChartZoomEventArgs e) { }
		void IChartInteractionProvider.OnZoom3D(ChartZoom3DEventArgs e) { }
		void IChartInteractionProvider.OnQueryCursor(QueryCursorEventArgs e) { }
		#endregion
		#region IChartContainer implementation
		event EventHandler IChartContainer.EndLoading {
			add { Events.AddHandler(endLoading, value); }
			remove { Events.RemoveHandler(endLoading, value); }
		}
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return this; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return this; } }
		bool IChartContainer.ShowDesignerHints {
			get { return RootReport != null ? RootReport.DesignerOptions.ShowDesignerHints : false; }
		}
		Chart IChartContainer.Chart { get { return chart; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return Site; } }
		IComponent IChartContainer.Parent { get { return base.Parent; } }
		bool IChartContainer.DesignMode { get { return DesignMode; } }
		bool IChartContainer.IsEndUserDesigner { get { return IsEUD; } }
		ChartContainerType IChartContainer.ControlType { get { return ChartContainerType.XRControl; } }
		bool IChartContainer.Loading { get { return loading; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return DesignMode && !IsEUD; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		void IChartContainer.LockChangeService() {
			lockChangeServiceCounter++;
		}
		void IChartContainer.UnlockChangeService() {
			lockChangeServiceCounter--;
		}
		void IChartContainer.Changing() {
			if (lockChangeServiceCounter == 0) {
				IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				if (cs != null)
					cs.OnComponentChanging(this, null);
			}
		}
		void IChartContainer.Changed() {
			if (lockChangeServiceCounter == 0) {
				IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				if (cs != null)
					cs.OnComponentChanged(this, null, null, null);
				((IChartRenderProvider)this).Invalidate();
			}
		}
		void IChartContainer.ShowErrorMessage(string message, string title) {
			if (Site == null && PSNativeMethods.AspIsRunning)
				throw new ApplicationException(message);
			string actualTitle = string.IsNullOrEmpty(title) ? Name : title;
			IChartService serv = GetChartService();
			if (serv != null)
				serv.ShowErrorMessage(this, message, actualTitle);
		}
		void IChartContainer.Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object min, object max, bool invalidate) { }
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		#endregion
		#region ISupportInitialize implementation
		public void BeginInit() {
			loading = true;
		}
		public void EndInit() {
			loading = false;
			EventHandler handler = (EventHandler)Events[endLoading];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region ShouldSerialize
		bool ShouldSerializeDiagram() {
			return chart.ShouldSerializeDiagram();
		}
		new bool ShouldSerializeBackColor() {
			return chart.ShouldSerializeBackColor();
		}
		bool ShouldSerializeDataMember() {
			return DataContainer.ShouldSerializeDataMember();
		}
		bool ShouldSerializeSeriesDataMember() {
			return DataContainer.ShouldSerializeSeriesDataMember();
		}
		bool ShouldSerializeSeriesSorting() {
			return chart.ShouldSerializeBoundSeriesSorting();
		}
		bool ShouldSerializeDataSource() {
			return chart.ShouldSerializeDataSource();
		}
		bool ShouldSerializeXmlDataSource() {
			return DataContainer.DataSource != null;
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
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		bool ShouldSerializeEmptyChartText() {
			return Chart.ShouldSerializeEmptyChartText();
		}
		bool ShouldSerializeSmallChartText() {
			return Chart.ShouldSerializeSmallChartText();
		}
		bool ShouldSerializeAutoBindingSettingsEnabled() {
			return false;
		}
		bool ShouldSerializeAutoLayoutSettingsEnabled() {
			return false;
		}
		bool ShouldSerializeAutoLayout() {
			return chart.ShouldSerializeAutoLayout();
		}
		internal protected override bool ShouldSerializePadding() {
			return chart.ShouldSerializePadding();
		}
		public override void ResetPadding() {
			chart.ResetPadding();
		}
		#endregion
		object IDataContainer.GetEffectiveDataSource() {
			return this.DataContainer.ActualDataSource;
		}
		object IDataContainer.GetSerializableDataSource() {
			return DataSourceHelper.ConvertToSerializableDataSource(DataSource);
		}
		void DisposeBitmapContainer() {
			if (this.bitmapContainer != null) {
				this.bitmapContainer.Dispose();
				this.bitmapContainer = null;
			}
		}
		void RefreshChartData() {
			chart.RefreshData(true);
		}
		new void ResetBackColor() {
			BackColor = Color.Empty;
		}
		Bitmap CreateLowQualityBitmap(Size size) {
			if (this.bitmapContainer != null && this.bitmapContainer.Size != size)
				DisposeBitmapContainer();
			if (this.bitmapContainer == null)
				this.bitmapContainer = new ChartBitmapContainer3D(this.chart, size, GraphicsQuality.Lowest);
			this.bitmapContainer.DrawChart(false);
			return bitmapContainer.GetBitmapCopy();
		}
		IChartService GetChartService() {
			return Site == null ? null : Site.GetService(typeof(IChartService)) as IChartService;
		}
		protected virtual void OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			RunEventScript(CustomDrawSeriesEvent, EventNames.CustomDrawSeries, e);
			CustomDrawSeriesEventHandler handler = (CustomDrawSeriesEventHandler)Events[CustomDrawSeriesEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			RunEventScript(CustomDrawCrosshairEvent, EventNames.CustomDrawCrosshair, e);
			CustomDrawCrosshairEventHandler handler = (CustomDrawCrosshairEventHandler)Events[CustomDrawCrosshairEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			RunEventScript(CustomDrawSeriesPointEvent, EventNames.CustomDrawSeriesPoint, e);
			CustomDrawSeriesPointEventHandler handler = (CustomDrawSeriesPointEventHandler)this.Events[CustomDrawSeriesPointEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e) {
			RunEventScript(CustomDrawAxisLabelEvent, EventNames.CustomDrawAxisLabel, e);
			CustomDrawAxisLabelEventHandler handler = (CustomDrawAxisLabelEventHandler)this.Events[CustomDrawAxisLabelEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomPaint(CustomPaintEventArgs e) {
			RunEventScript(CustomPaintEvent, EventNames.CustomPaint, e);
			CustomPaintEventHandler handler = (CustomPaintEventHandler)this.Events[CustomPaintEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnBoundDataChanged(EventArgs e) {
			RunEventScript(BoundDataChangedEvent, EventNames.BoundDataChanged, e);
			BoundDataChangedEventHandler handler = (BoundDataChangedEventHandler)this.Events[BoundDataChangedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
			RunEventScript(PieSeriesPointExplodedEvent, EventNames.PieSeriesPointExploded, e);
			PieSeriesPointExplodedEventHandler handler = (PieSeriesPointExplodedEventHandler)this.Events[PieSeriesPointExplodedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisScaleChanged(AxisScaleChangedEventArgs e) {
			RunEventScript(AxisScaleChangedEvent, EventNames.AxisScaleChanged, e);
			EventHandler<AxisScaleChangedEventArgs> handler = (EventHandler<AxisScaleChangedEventArgs>)this.Events[AxisScaleChangedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) {
			RunEventScript(AxisVisualRangeChangedEvent, EventNames.AxisVisualRangeChanged, e);
			EventHandler<AxisRangeChangedEventArgs> handler = (EventHandler<AxisRangeChangedEventArgs>)this.Events[AxisVisualRangeChangedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) {
			RunEventScript(AxisWholeRangeChangedEvent, EventNames.AxisWholeRangeChanged, e);
			EventHandler<AxisRangeChangedEventArgs> handler = (EventHandler<AxisRangeChangedEventArgs>)this.Events[AxisWholeRangeChangedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomizeAutoBindingSettings(EventArgs e) {
			RunEventScript(CustomizeAutoBindingSettingsEvent, EventNames.CustomizeAutoBindingSettings, e);
			CustomizeAutoBindingSettingsEventHandler handler = (CustomizeAutoBindingSettingsEventHandler)Events[CustomizeAutoBindingSettingsEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomizeSimpleDiagramLayout(CustomizeSimpleDiagramLayoutEventArgs e) {
			RunEventScript(CustomizeSimpleDiagramLayoutEvent, EventNames.CustomizeSimpleDiagramLayout, e);
			CustomizeSimpleDiagramLayoutEventHandler handler = (CustomizeSimpleDiagramLayoutEventHandler)Events[CustomizeSimpleDiagramLayoutEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotChartingCustomizeXAxisLabels(CustomizeXAxisLabelsEventArgs e) {
			RunEventScript(PivotChartingCustomizeXAxisLabelsEvent, EventNames.PivotChartingCustomizeXAxisLabels, e);
			CustomizeXAxisLabelsEventHandler handler = (CustomizeXAxisLabelsEventHandler)Events[PivotChartingCustomizeXAxisLabelsEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotChartingCustomizeResolveOverlappingMode(CustomizeResolveOverlappingModeEventArgs e) {
			RunEventScript(PivotChartingCustomizeResolveOverlappingModeEvent, EventNames.PivotChartingCustomizeResolveOverlappingMode, e);
			CustomizeResolveOverlappingModeEventHandler handler = (CustomizeResolveOverlappingModeEventHandler)Events[PivotChartingCustomizeResolveOverlappingModeEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotChartingCustomizeLegend(CustomizeLegendEventArgs e) {
			RunEventScript(PivotChartingCustomizeLegendEvent, EventNames.PivotChartingCustomizeLegend, e);
			CustomizeLegendEventHandler handler = (CustomizeLegendEventHandler)Events[PivotChartingCustomizeLegendEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) {
			RunEventScript(PivotGridSeriesExcludedEvent, EventNames.PivotGridSeriesExcluded, e);
			PivotGridSeriesExcludedEventHandler handler = (PivotGridSeriesExcludedEventHandler)Events[PivotGridSeriesExcludedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) {
			RunEventScript(PivotGridSeriesPointsExcludedEvent, EventNames.PivotGridSeriesPointsExcluded, e);
			PivotGridSeriesPointsExcludedEventHandler handler = (PivotGridSeriesPointsExcludedEventHandler)Events[PivotGridSeriesPointsExcludedEvent];
			if (handler != null)
				handler(this, e);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				base.Dispose(disposing);
				DisposeBitmapContainer();
				if (chart != null) {
					chart.Dispose();
					chart = null;
				}
			} else
				base.Dispose(disposing);
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.AddDataSource(DataSourceHelper.ConvertToSerializableDataSource(DataSource) as IComponent);
			components.AddDataAdapter(DataAdapter);
		}
		protected override void UpdateBindingCore(XRDataContext dataContext, ImagesContainer images) {
			RefreshChartData();
			base.UpdateBindingCore(dataContext, images);
		}
		protected override void OnSizeChanged(ChangeEventArgs e) {
			if (chart != null) {
				chart.ResetGraphicsCache();
				chart.InvalidateDrawingHelper();
			}
			base.OnSizeChanged(e);
		}
		protected override XRControlScripts CreateScripts() {
			return new XRChartScripts(this);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new ChartBrick(this);
		}
		protected internal override void ValidateDataSource(object newSource, object oldSource, string dataMember) {
			base.ValidateDataSource(newSource, oldSource, dataMember);
			if (newSource != null && DataSource == null && !loading)
				if (DesignMode)
					DataContainer.ProcessChangeBinding();
				else {
					DataContainer.CheckBindingRuntime(newSource);
					RefreshChartData();
				}
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			ChartBrick chartBrick = (ChartBrick)brick;
			Chart source = chartBrick.Chart;
			if (source != null)
				chart.Assign(source);
			base.GetStateFromBrick(brick);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			ChartBrick chartBrick = (ChartBrick)brick;
			chartBrick.DisposeControls();
			chartBrick.Padding = PaddingInfo.Empty;
			chartBrick.SizeMode = ImageSizeMode.Normal;
			chartBrick.Chart = chart;
			chartBrick.Image = Image;
		}
		protected internal override void CopyProperties(XRControl control) {
			XRChart chartControl = control as XRChart;
			if (chartControl != null)
				chart.Assign(chartControl.Chart);
		}
		protected internal override void CopyDataProperties(XRControl control) {
			XRChart chart = control as XRChart;
			if(chart != null && !(chart.DataSource is XRPivotGrid)) {
				DataSource = chart.DataSource;
				DataAdapter = chart.DataAdapter;
				int diff = Series.Count - chart.Series.Count;
				if(diff >= 0)
					for(int i = diff, j = 0; i < Series.Count; i++, j++)
						if(!(chart.Series[j].DataSource is XRPivotGrid))
							Series[i].DataSource = chart.Series[j].DataSource;
			}
			base.CopyDataProperties(control);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			NullDataContext();
		}
		protected override void AfterReportPrint() {
			base.AfterReportPrint();
			NullDataContext();
		}
		void NullDataContext() {
			if(dataContext != null && dataContext.HasOwner(this)) {
				dataContext.Dispose();
				dataContext = null;
			}
		}
		public Series GetSeriesByName(string seriesName) {
			return DataContainer.GetSeriesByName(seriesName);
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
		[Obsolete("This method is obsolete now. Use the ResetLegendTextPattern method instead.")]
		public void ResetLegendPointOptions() {
			ResetLegendTextPattern();
		}
		public void ResetLegendTextPattern() {
			DataContainer.ResetLegendTextPattern();
		}
		public void SaveToStream(Stream stream) {
			chart.SaveLayout(stream);
		}
		public void SaveToFile(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
				SaveToStream(fs);
		}
		public void LoadFromStream(Stream stream) {
			chart.LoadLayout(stream);
		}
		public void LoadFromFile(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				LoadFromStream(fs);
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
		public void BindToData(ViewType viewType, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			DataContainer.BindToData(viewType, dataSource, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(ViewType viewType, object dataSource, string dataMember, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			DataContainer.BindToData(viewType, dataSource, dataMember, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(SeriesViewBase view, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			DataContainer.BindToData(view, dataSource, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(SeriesViewBase view, object dataSource, string dataMember, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			DataContainer.BindToData(view, dataSource, dataMember, seriesDataMember, argumentDataMember, valueDataMembers);
		}
	}
}
namespace DevExpress.XtraCharts {
	class Fix_Q426582 {
	}
}
namespace DevExpress.XtraReports.Native {
	public class ChartBrick : ImageBrick {
		class XRChart2 : XRChart {
			IDataContainer dataContainer;
			protected override object ParentDataSource {
				get { return dataContainer != null ? dataContainer.GetEffectiveDataSource() : null; }
			}
			public XRChart2(IDataContainer dataContainer) {
				this.dataContainer = dataContainer;
			}		
		}
		XRChart chart;
		public Chart Chart {
			get { return chart == null ? null : chart.Chart; }
			set {
				if (value != null) {
					chart = new XRChart2(((XRChart)value.Container).Report);
					chart.Chart.Assign(value);
				}
			}
		}
		public ChartBrick(XRChart chart)
			: base(chart) {
		}
		public void DisposeControls() {
			if (chart != null) {
				chart.Dispose();
				chart = null;
			}
			if (Image != null) {
				Image.Dispose();
				Image = null;
			}
		}
		public override void Dispose() {
			base.Dispose();
			DisposeControls();
		}
	}
}
