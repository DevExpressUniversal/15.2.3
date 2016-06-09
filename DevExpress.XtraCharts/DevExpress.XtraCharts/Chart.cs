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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts.Native {
	delegate void RaiseHitTestingDelegate(HotTrackEventArgs e);
	[SerializationContext(typeof(ChartSerializationContext))]
	public partial class Chart : ChartElement, IBackground, IHitTest, IPrintable, IXtraSerializable, IXtraSupportDeserializeCollectionItem, IXtraSupportCreateContentPropertyValue, IXtraSupportAfterDeserialize, ISupportRangeControl {
		const int defaultPadding = 5;
		const DefaultBoolean defaultCrosshairEnabled = DefaultBoolean.True;
		const DefaultBoolean defaultToolTipEnabled = DefaultBoolean.Default;
		static void CheckSize(Size size) {
			if (size.Width <= 0 || size.Height <= 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectImageBounds), "size");
		}
		static int ZoomValue(int value, double zoomDelta) {
			int delta = (int)MathUtils.StrongRound(value * zoomDelta) - value;
			if ((delta & 1) == 1)
				delta += delta > 0 ? 1 : -1;
			return value + delta;
		}
		static Metafile CreateMetafileInstance(Stream stream, Rectangle bounds, MetafileFrameUnit units, EmfType emfType) {
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = graphics.GetHdc();
				try {
					return stream == null ? new Metafile(hdc, bounds, units, emfType) : new Metafile(stream, hdc, bounds, units, emfType);
				}
				finally {
					graphics.ReleaseHdc(hdc);
				}
			}
		}
		readonly IChartContainer container;
		readonly DataContainer dataContainer;
		readonly ViewController viewController;
		readonly Legend legend;
		readonly ChartTitleCollection titles;
		readonly RectangleFillStyle fillStyle;
		readonly RectangularBorder border;
		readonly BackgroundImage backImage;
		readonly RectangleIndents padding;
		readonly ChartOptionsPrint optionsPrint;
		readonly EmptyChartText emptyChartText;
		readonly SmallChartText smallChartText;
		readonly AnnotationRepository annotationRepository;
		readonly AnnotationCollection annotations;
		readonly AnnotationNavigation annotationNavigation;
		readonly HitTestController hitTestController;
		readonly AppearanceRepository appearanceRepository = new AppearanceRepository();
		readonly PaletteRepository paletteRepository = new PaletteRepository();
		readonly PaletteRepository indicatorsPaletteRepository = new PaletteRepository(new IndicatorsPredefinedPaletteList());
		readonly DiagramCache diagramCache = new DiagramCache();
		readonly Drawing2DCache drawing2DCache = new Drawing2DCache();
		readonly Graphics3DCache graphicsCache = new Graphics3DCache();
		readonly OpenGLGraphicsTextureCache textureCache = new OpenGLGraphicsTextureCache();
		readonly IRenderer renderer;
		readonly CrosshairOptions crosshairOptions;
		readonly DummyRangeControlClient dummyDiagramRange = new DummyRangeControlClient();
		readonly ChartContainerAdapter containerAdapter;
		readonly SelectionController selectionController;
		Diagram diagram;
		bool loading = false;
		bool cacheToMemory = false;
		bool needLayoutUpdate = false;
		ChartAppearance appearance;
		Color backColor;
		Palette palette = null;
		string paletteName = String.Empty;
		int paletteBaseColorNumber = 0;
		Palette indicatorsPalette = null;
		string indicatorsPaletteName = String.Empty;
		double sideBySideBarDistance = SideBySideBarDefaults.DefaultBarDistance;
		int sideBySideBarDistanceFixed = SideBySideBarDefaults.DefaultBarDistanceFixed;
		bool sideBySideEqualBarWidth = SideBySideBarDefaults.DefaultEqualBarWidth;
		bool isDisposed = false;
		bool xtraSerializing;
		bool autoLayout;
		SummaryFunctionsStorage summaryFunctions;
		XtraPaletteWrapperCollection xtraPaletteRepository;
		XtraPaletteWrapperCollection xtraIndicatorsPaletteRepository;
		ChartPrinter printer;
		HitTestParams cachedSelectedHitParams;
		ChartDrawingHelper drawingHelper;
		Rectangle lastBounds = Rectangle.Empty;
		ToolTipOptions toolTipOptions;
		ChartBitmapCache cacheBitmap = null;
		DefaultBoolean crosshairEnabled = defaultCrosshairEnabled;
		DefaultBoolean toolTipEnabled = defaultToolTipEnabled;
		int cacheHashCode = 0;
		Graphics graphics;
		Graphics graphicsMiddle;
		Graphics graphicsAbove;
		object rangeControl;
		DefaultBoolean rightToLeft = DefaultBoolean.Default;
		bool SupportDataMember { get { return container.ControlType == ChartContainerType.XRControl; } }
		bool SupportBorder { get { return container.ControlType != ChartContainerType.XRControl; } }
		bool SupportRuntimeSelection { get { return container == null || container.ControlType != ChartContainerType.XRControl; } }
		Graphics Graphics {
			get { return graphics; }
			set { graphics = value; }
		}
		Graphics GraphicsMiddle {
			get { return graphicsMiddle; }
			set { graphicsMiddle = value; }
		}
		Graphics GraphicsAbove {
			get { return graphicsAbove; }
			set { graphicsAbove = value; }
		}
		bool DefaultAutoLayout { get { return ChartContainer.ControlType == ChartContainerType.WinControl; } }
		bool HasAutoLayoutTitles {
			get {
				foreach (ChartTitle title in Titles) {
					if (title.Visibility == DefaultBoolean.Default) {
						return true;
					}
				}
				return false;
			}
		}
		bool ActualToolTipEnabled {
			get {
				foreach (Series series in Series)
					if (series.ActualToolTipEnabled)
						return true;
				return false;
			}
		}
		internal Rectangle LastBounds { get { return lastBounds; } set { lastBounds = value; } }
		internal bool IsRangeControlClient { get { return (rangeControl != null); } }
		internal bool CanUseBoundPoints { get { return containerAdapter.DataProvider != null && containerAdapter.DataProvider.CanUseBoundPoints; } }
		internal bool ShouldUseSeriesTemplate { get { return DataContainer.ShouldUseSeriesTemplate; } }
		internal bool HasBoundSeries { get { return BindingHelper.HasBoundSeries(this); } }
		internal SummaryFunctionsStorage SummaryFunctions { get { return summaryFunctions; } }
		internal bool SupportScrollingAndZooming { get { return container.ControlType == ChartContainerType.WinControl; } }
		internal bool SupportToolTips { get { return container.ControlType != ChartContainerType.XRControl && Diagram != null && Diagram.SupportTooltips; } }
		internal bool SupportCrosshair { get { return container.ControlType != ChartContainerType.XRControl && Diagram is XYDiagram2D; } }
		internal AnnotationNavigation AnnotationNavigation { get { return annotationNavigation; } }
		internal DiagramCache DiagramCache { get { return diagramCache; } }
		internal Drawing2DCache Drawing2DCache { get { return drawing2DCache; } }
		internal OpenGLGraphicsTextureCache TextureCache { get { return textureCache; } }
		internal ChartDrawingHelper DrawingHelper {
			get {
				if (drawingHelper == null)
					drawingHelper = new ChartDrawingHelper(this);
				return drawingHelper;
			}
		}
		internal bool ActualCrosshairHighlightPoints {
			get {
				if (ActualCrosshairEnabled)
					foreach (Series series in Series)
						if (series.ActualCrosshairHighlightPoints)
							return true;
				return false;
			}
		}
		internal bool LockCrosshairForExport { get; set; }
		internal SelectionController SelectionController { get { return selectionController; } }
		internal bool HasAutoLayoutElements { get { return Legend.Visibility == DefaultBoolean.Default || HasAutoLayoutTitles || Diagram.HasAutoLayoutElements; } }
		protected internal override bool Loading { get { return container.Loading || loading; } }
		protected internal override IChartContainer ChartContainer { get { return Container; } }
		protected internal override ChartContainerAdapter ContainerAdapter { get { return containerAdapter; } }
		public IChartContainer Container { get { return container; } }
		public bool Is3DDiagram { get { return diagram is Diagram3D; } }
		public Graphics3DCache Graphics3DCache { get { return graphicsCache; } }
		public HitTestController HitTestController { get { return hitTestController; } }
		public bool HitTestingEnabled { get { return !Is3DDiagram && container != null && containerAdapter.HitTestingEnabled; } }
		public bool SeriesToolTipEnabled { get { return ToolTipOptions.ShowForSeries && container != null && !container.DesignMode; } }
		public bool PointToolTipEnabled { get { return ToolTipOptions.ShowForPoints && container != null && !container.DesignMode; } }
		public bool ActualCrosshairEnabled {
			get {
				foreach (Series series in Series)
					if (series.ActualCrosshairEnabled)
						return true;
				return false;
			}
		}
		public bool ActualSeriesToolTipEnabled {
			get { return ActualToolTipEnabled && ToolTipOptions.ShowForSeries && container != null && !container.DesignMode; }
		}
		public bool ActualPointToolTipEnabled {
			get { return ActualToolTipEnabled && ToolTipOptions.ShowForPoints && container != null && !container.DesignMode; }
		}
		public ChartPrinter Printer {
			get { return printer; }
			set { printer = value; }
		}
		public ViewController ViewController {
			get {
				return viewController;
			}
		}
		public override bool IsDisposed {
			get {
				return base.IsDisposed || isDisposed;
			}
		}
		[
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool AutoLayout {
			get { return autoLayout; }
			set {
				if (value != autoLayout) {
					SendNotification(new ElementWillChangeNotification(this));
					autoLayout = value;
					RaiseControlChanged();
				}
			}
		}
		[NonTestableProperty]
		public SeriesCollection Series { get { return DataContainer.Series; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public DataContainer DataContainer { get { return dataContainer; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true),
		Obsolete
		]
		public Series[] SeriesSerializable {
			get {
				return DataContainer.SeriesSerializable;
			}
			set {
				if (!Loading)
					throw new MemberAccessException();
				DataContainer.SeriesSerializable = value;
			}
		}
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Obsolete
		]
		public SeriesBase SeriesTemplate { get { return DataContainer.SeriesTemplate; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Obsolete
		]
		public SeriesNameTemplate SeriesNameTemplate { get { return DataContainer.SeriesNameTemplate; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public Legend Legend { get { return legend; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public ChartTitleCollection Titles { get { return titles; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RectangularBorder Border { get { return border; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BackgroundImage BackImage { get { return backImage; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RectangleIndents Padding { get { return padding; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ChartOptionsPrint OptionsPrint { get { return optionsPrint; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Obsolete
		]
		public PivotGridDataSourceOptions PivotGridDataSourceOptions { get { return DataContainer.PivotGridDataSourceOptions; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public EmptyChartText EmptyChartText { get { return emptyChartText; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public SmallChartText SmallChartText { get { return smallChartText; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public AnnotationRepository AnnotationRepository { get { return annotationRepository; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public AnnotationCollection Annotations {
			get {
				annotations.Update();
				return annotations;
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public Diagram Diagram {
			get { return diagram; }
			set {
				if (!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgDesignTimeOnlySetting));
				Diagram oldValue = diagram;
				SendNotification(new ElementWillChangeNotification(this));
				diagram = value;
				if (diagram != null)
					diagram.Owner = this;
				RaiseControlChanged(new PropertyUpdateInfo<IDiagram>(this, "Diagram", oldValue, diagram));
			}
		}
		[
		XtraSerializableProperty,
		NonTestableProperty,
		Obsolete
		]
		public string DataMember {
			get {
				return DataContainer.DataMember;
			}
			set {
				DataContainer.DataMember = value;
			}
		}
		[
		XtraSerializableProperty,
		NonTestableProperty,
		Obsolete
		]
		public string SeriesDataMember {
			get {
				return DataContainer.SeriesDataMember;
			}
			set {
				DataContainer.SeriesDataMember = value;
			}
		}
		[
		XtraSerializableProperty,
		Obsolete("Use DataContainer.BoundSeriesSorting instead")
		]
		public SortingMode BoundSeriesSorting {
			get { return DataContainer.BoundSeriesSorting; }
			set { DataContainer.BoundSeriesSorting = value; }
		}
		[
		NonTestableProperty,
		XtraSerializableProperty,
		Obsolete
		]
		public bool RefreshDataOnRepaint {
			get { return DataContainer.RefreshDataOnRepaint; }
			set { DataContainer.RefreshDataOnRepaint = value; }
		}
		[XtraSerializableProperty]
		public bool CacheToMemory {
			get { return cacheToMemory; }
			set {
				if (value != cacheToMemory) {
					cacheToMemory = value;
					if (!cacheToMemory)
						ClearCache();
				}
			}
		}
		[NonTestableProperty]
		public ChartAppearance Appearance {
			get { return appearance; }
			set {
				if (value == null)
					throw new ArgumentException();
				if (value != appearance) {
					SendNotification(new ElementWillChangeNotification(this));
					appearance = value;
					Palette palette = paletteRepository.GetAppearancePalette(appearance.PaletteName);
					if (palette != null)
						this.palette = palette;
					palette = indicatorsPaletteRepository.GetAppearancePalette(appearance.IndicatorsPaletteName);
					if (palette != null)
						indicatorsPalette = palette;
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeControlChanged));
				}
			}
		}
		[XtraSerializableProperty]
		public string AppearanceName {
			get { return appearance.Name; }
			set {
				ChartAppearance appearance = appearanceRepository[value];
				if (appearance == null) {
					if (!Loading)
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAppearanceName), value));
				}
				else
					Appearance = appearance;
			}
		}
		[XtraSerializableProperty]
		public string AppearanceNameSerializable {
			get { return appearance.SerializableName; }
			set {
				ChartAppearance appearance = appearanceRepository.GetAppearance(value, true);
				if (appearance != null)
					Appearance = appearance;
			}
		}
		[XtraSerializableProperty]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		public Color ActualBackColor {
			get { return backColor.IsEmpty ? ((IChartAppearance)appearance).WholeChartAppearance.BackColor : backColor; }
		}
		public Palette Palette {
			get { return palette; }
			set {
				if (value == null)
					throw new ArgumentException();
				if (value != palette) {
					SendNotification(new ElementWillChangeNotification(this));
					if (paletteBaseColorNumber > value.Count)
						paletteBaseColorNumber = (paletteBaseColorNumber - 1) % value.Count + 1;
					palette = value;
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeControlChanged));
				}
			}
		}
		[XtraSerializableProperty]
		public string PaletteName {
			get { return palette.Name; }
			set {
				if (Loading)
					paletteName = value;
				else {
					Palette palette = paletteRepository[value];
					if (palette == null)
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgPaletteNotFound), value));
					Palette = palette;
				}
			}
		}
		[XtraSerializableProperty]
		public int PaletteBaseColorNumber {
			get { return paletteBaseColorNumber; }
			set {
				if (value != paletteBaseColorNumber) {
					if (!Loading && (value < 0 || value > palette.Count))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPaletteBaseColorNumber));
					SendNotification(new ElementWillChangeNotification(this));
					paletteBaseColorNumber = value;
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeControlChanged));
				}
			}
		}
		public Palette IndicatorsPalette {
			get { return indicatorsPalette; }
			set {
				if (value == null)
					throw new ArgumentException();
				if (value != indicatorsPalette) {
					SendNotification(new ElementWillChangeNotification(this));
					indicatorsPalette = value;
					RaiseControlChanged();
				}
			}
		}
		[XtraSerializableProperty]
		public string IndicatorsPaletteName {
			get { return indicatorsPalette.Name; }
			set {
				if (Loading)
					indicatorsPaletteName = value;
				else {
					Palette palette = indicatorsPaletteRepository[value];
					if (palette == null)
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgPaletteNotFound), value));
					IndicatorsPalette = palette;
				}
			}
		}
		[XtraSerializableProperty]
		public double SideBySideBarDistance {
			get { return sideBySideBarDistance; }
			set {
				if (value != sideBySideBarDistance) {
					if (value < 0)
						throw new ArgumentException();
					SendNotification(new ElementWillChangeNotification(this));
					sideBySideBarDistance = value;
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		[XtraSerializableProperty]
		public int SideBySideBarDistanceFixed {
			get { return sideBySideBarDistanceFixed; }
			set {
				if (value != sideBySideBarDistanceFixed) {
					if (value < 0)
						throw new ArgumentException();
					SendNotification(new ElementWillChangeNotification(this));
					sideBySideBarDistanceFixed = value;
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		[XtraSerializableProperty]
		public bool SideBySideEqualBarWidth {
			get { return sideBySideEqualBarWidth; }
			set {
				if (value != sideBySideEqualBarWidth) {
					SendNotification(new ElementWillChangeNotification(this));
					sideBySideEqualBarWidth = value;
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		[XtraSerializableProperty,
		Obsolete]
		public bool RuntimeSelection {
			get { return SelectionMode != ElementSelectionMode.None; }
			set {
				SelectionMode = value && SupportRuntimeSelection ? ElementSelectionMode.Single : ElementSelectionMode.None;
				if (SelectionMode == ElementSelectionMode.None)
					ClearSelection();
			}
		}
		[XtraSerializableProperty]
		public ElementSelectionMode SelectionMode {
			get { return selectionController.ElementSelectionMode; }
			set { selectionController.ElementSelectionMode = value; }
		}
		[XtraSerializableProperty]
		public SeriesSelectionMode SeriesSelectionMode {
			get { return selectionController.SeriesSelectionMode; }
			set { selectionController.SeriesSelectionMode = value; }
		}
		[NonTestableProperty]
		public IList SelectedItems { get { return selectionController.SelectedItems; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SelectedItemsInfos SelectedItemsInfos {
			get { return selectionController.SelectedItemsInfos; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ToolTipOptions ToolTipOptions { get { return toolTipOptions; } }
		[XtraSerializableProperty]
		public DefaultBoolean CrosshairEnabled {
			get { return crosshairEnabled; }
			set {
				if (crosshairEnabled != value) {
					DefaultBoolean oldValue = crosshairEnabled;
					SendNotification(new ElementWillChangeNotification(this));
					crosshairEnabled = value;
					RaiseControlChanged(new PropertyUpdateInfo(this, "CrosshairEnabled"));
				}
			}
		}
		[XtraSerializableProperty]
		public DefaultBoolean ToolTipEnabled {
			get { return toolTipEnabled; }
			set {
				if (toolTipEnabled != value) {
					SendNotification(new ElementWillChangeNotification(this));
					toolTipEnabled = value;
					RaiseControlChanged();
				}
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public CrosshairOptions CrosshairOptions { get { return crosshairOptions; } }
		public AppearanceRepository AppearanceRepository { get { return appearanceRepository; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public PaletteRepository PaletteRepository { get { return paletteRepository; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public XtraPaletteWrapperCollection XtraPaletteRepository {
			get {
				XtraPaletteWrapperCollection xtraPaletteRepository = new XtraPaletteWrapperCollection(paletteRepository);
				if (xtraSerializing && Loading)
					this.xtraPaletteRepository = xtraPaletteRepository;
				return xtraPaletteRepository;
			}
		}
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public PaletteRepository IndicatorsPaletteRepository { get { return indicatorsPaletteRepository; } }
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public XtraPaletteWrapperCollection XtraIndicatorsPaletteRepository {
			get {
				XtraPaletteWrapperCollection xtraPaletteRepository = new XtraPaletteWrapperCollection(indicatorsPaletteRepository);
				if (xtraSerializing && Loading)
					this.xtraPaletteRepository = xtraPaletteRepository;
				return xtraPaletteRepository;
			}
		}
		[NonTestableProperty]
		public bool CanZoomIn { get { return diagram != null && diagram.CanZoomIn; } }
		[NonTestableProperty]
		public bool CanZoomInViaRect { get { return diagram != null && diagram.CanZoomInViaRect; } }
		[NonTestableProperty]
		public bool CanZoomOut { get { return diagram != null && diagram.CanZoomOut; } }
		[XtraSerializableProperty]
		public DefaultBoolean RightToLeft {
			get { return rightToLeft; }
			set {
				if (value != rightToLeft) {
					SendNotification(new ElementWillChangeNotification(this));
					rightToLeft = value;
					RaiseControlChanged();
				}
			}
		}
		#region Obsolete properties
		[Obsolete("Use the PivotGridDataSourceOptions.AutoBindingSettingsEnabled property instead.")]
		public bool AutoBindingSettingsEnabled {
			get { return DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled; }
			set { DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled = value; }
		}
		[Obsolete("Use the PivotGridDataSourceOptions.AutoLayoutSettingsEnabled property instead.")]
		public bool AutoLayoutSettingsEnabled {
			get { return DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled; }
			set { DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled = value; }
		}
		[NonTestableProperty, Obsolete]
		public object ActualDataSource {
			get {
				return DataContainer.ActualDataSource;
			}
		}
		[NonTestableProperty, Obsolete]
		public string ActualSeriesDataMember { get { return DataContainer.ActualSeriesDataMember; } }
		[NonTestableProperty, Obsolete]
		public IList<ISeries> AutocreatedSeries { get { return DataContainer.AutocreatedSeries; } }
		[NonTestableProperty, Obsolete]
		public object DataAdapter {
			get {
				return DataContainer.DataAdapter;
			}
			set {
				DataContainer.DataAdapter = value;
			}
		}
		[NonTestableProperty, Obsolete]
		public object DataSource {
			get { return DataContainer.DataSource; }
			set { DataContainer.DataSource = value; }
		}
		#endregion
		public Chart(IChartContainer container)
			: base() {
			if (container == null)
				throw new ArgumentNullException("container");
			this.container = container;
			this.containerAdapter = new ChartContainerAdapter(container);
			container.EndLoading += new EventHandler(OnEndLoading);
			dataContainer = new DataContainer(this);
			viewController = new ViewController(this);
			Owner = viewController;
			legend = new Legend(this);
			titles = new ChartTitleCollection(this);
			fillStyle = new RectangleFillStyle(this, Color.Empty, FillMode.Solid);
			border = new InsideRectangularBorder(this, true, Color.Empty);
			backImage = new BackgroundImage(this);
			padding = new RectangleIndents(this, defaultPadding);
			optionsPrint = new ChartOptionsPrint(this);
			emptyChartText = new EmptyChartText(this);
			smallChartText = new SmallChartText(this);
			annotationRepository = new AnnotationRepository(this);
			annotations = new AnnotationCollection(new AnnotationCollectionChartBehavior(this));
			annotationNavigation = new AnnotationNavigation(annotationRepository);
			hitTestController = new HitTestController(this);
			appearance = appearanceRepository[ChartLocalizer.GetString(ChartStringId.AppDefault)];
			palette = paletteRepository.GetAppearancePalette(appearance.PaletteName);
			indicatorsPalette = indicatorsPaletteRepository.GetAppearancePalette(appearance.IndicatorsPaletteName);
			ResetSummaryFunctions();
			toolTipOptions = new ToolTipOptions(this);
			renderer = new GdiPlusRenderer();
			crosshairOptions = new CrosshairOptions(this);
			selectionController = new SelectionController(this);
			autoLayout = DefaultAutoLayout;
		}
		#region IPrintable implementation
		void IBasePrintable.Initialize(IPrintingSystem printingSystem, ILink link) {
			if (printer != null)
				printer.Initialize(printingSystem, link);
		}
		void IBasePrintable.Finalize(IPrintingSystem printingSystem, ILink link) {
			if (printer != null)
				printer.Release();
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			if (printer != null)
				printer.CreateArea(areaName, graph);
		}
		bool IPrintable.CreatesIntersectedBricks { get { return true; } }
		UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.AcceptChanges() { throw new NotSupportedException(); }
		bool IPrintable.HasPropertyEditor() { return false; }
		void IPrintable.RejectChanges() { throw new NotSupportedException(); }
		void IPrintable.ShowHelp() { throw new NotSupportedException(); }
		bool IPrintable.SupportsHelp() { return false; }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeDataContainer() {
			return DataContainer.ShouldSerialize();
		}
		bool ShouldSerializeSeriesTemplate() {
			return false;
		}
		bool ShouldSerializeSeriesNameTemplate() {
			return false;
		}
		bool ShouldSerializePivotGridDataSourceOptions() {
			return false;
		}
		bool ShouldSerializeSeriesSerializable() {
			return false;
		}
		bool ShouldSerializeRightToLeft() {
			if (rightToLeft == DefaultBoolean.Default)
				return false;
			else
				return true;
		}
		public bool ShouldSerializeDataMember() {
			return false;
		}
		public bool ShouldSerializeSeriesDataMember() {
			return false;
		}
		public bool ShouldSerializeRefreshDataOnRepaint() {
			return false;
		}
		public bool ShouldSerializeBoundSeriesSorting() {
			return false;
		}
		public bool ShouldSerializeDataSource() {
			return containerAdapter.DataProvider != null && containerAdapter.DataProvider.ShouldSerializeDataSource(DataContainer.DataSource);
		}
		public bool ShouldSerializePadding() {
			return padding.ShouldSerialize();
		}
		public bool ShouldSerializeEmptyChartText() {
			return emptyChartText.ShouldSerialize();
		}
		public bool ShouldSerializeSmallChartText() {
			return smallChartText.ShouldSerialize();
		}
		public bool ShouldSerializeDiagram() {
			return diagram != null && diagram.ShouldSerialize();
		}
		public bool ShouldSerializeAutoBindingSettingsEnabled() {
			return false;
		}
		public bool ShouldSerializeAutoLayoutSettingsEnabled() {
			return false;
		}
		public bool ShouldSerializeCacheToMemory() {
			return cacheToMemory;
		}
		public bool ShouldSerializeAppearanceName() {
			return false;
		}
		public bool ShouldSerializeAppearanceNameSerializable() {
			return !appearance.IsDefault;
		}
		public bool ShouldSerializeBackColor() {
			return !backColor.IsEmpty;
		}
		public bool ShouldSerializePaletteName() {
			return palette.Name != appearance.PaletteName;
		}
		public bool ShouldSerializePaletteBaseColorNumber() {
			return paletteBaseColorNumber != 0;
		}
		public bool ShouldSerializeIndicatorsPaletteName() {
			return indicatorsPalette.Name != appearance.IndicatorsPaletteName;
		}
		public bool ShouldSerializeSideBySideBarDistance() {
			return sideBySideBarDistance != SideBySideBarDefaults.DefaultBarDistance;
		}
		public bool ShouldSerializeSideBySideBarDistanceFixed() {
			return sideBySideBarDistanceFixed != SideBySideBarDefaults.DefaultBarDistanceFixed;
		}
		public bool ShouldSerializeSideBySideEqualBarWidth() {
			return sideBySideEqualBarWidth != SideBySideBarDefaults.DefaultEqualBarWidth;
		}
		public bool ShouldSerializeToolTipOptions() {
			return toolTipOptions.ShouldSerialize();
		}
		public bool ShouldSerializeCrosshairOptions() {
			return crosshairOptions.ShouldSerialize();
		}
		public bool ShouldSerializeCrosshairEnabled() {
			return crosshairEnabled != defaultCrosshairEnabled;
		}
		public bool ShouldSerializeToolTipEnabled() {
			return toolTipEnabled != defaultToolTipEnabled;
		}
		public bool ShouldSerializeRuntimeSelection() {
			return false;
		}
		public bool ShouldSerializeSelectedItemsInfos() {
			return Container != null && Container.ControlType == ChartContainerType.WebControl;
		}
		public bool ShouldSerializeAutoLayout() {
			return autoLayout != DefaultAutoLayout;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		protected internal override bool XtraSerializing { get { return xtraSerializing; } }
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "SeriesSerializable":
					return ShouldSerializeSeriesSerializable();
				case "DataContainer":
					return ShouldSerializeDataContainer();
				case "SeriesTemplate":
					return ShouldSerializeSeriesTemplate();
				case "SeriesNameTemplate":
					return ShouldSerializeSeriesNameTemplate();
				case "Diagram":
					return ShouldSerializeDiagram();
				case "Legend":
					return legend.ShouldSerialize();
				case "FillStyle":
					return fillStyle.ShouldSerialize();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "SeriesDataMember":
					return ShouldSerializeSeriesDataMember();
				case "BoundSeriesSorting":
					return ShouldSerializeBoundSeriesSorting();
				case "Border":
					return SupportBorder && border.ShouldSerialize();
				case "AppearanceName":
					return ShouldSerializeAppearanceName();
				case "PaletteName":
					return ShouldSerializePaletteName();
				case "IndicatorsPaletteName":
					return ShouldSerializeIndicatorsPaletteName();
				case "PaletteBaseColorNumber":
					return ShouldSerializePaletteBaseColorNumber();
				case "BackImage":
					return backImage.ShouldSerialize();
				case "CacheToMemory":
					return ShouldSerializeCacheToMemory();
				case "RefreshDataOnRepaint":
					return ShouldSerializeRefreshDataOnRepaint();
				case "SideBySideBarDistance":
					return ShouldSerializeSideBySideBarDistance();
				case "SideBySideBarDistanceFixed":
					return ShouldSerializeSideBySideBarDistanceFixed();
				case "SideBySideEqualBarWidth":
					return ShouldSerializeSideBySideEqualBarWidth();
				case "OptionsPrint":
					return optionsPrint.ShouldSerialize();
				case "DataMember":
					return SupportDataMember && ShouldSerializeDataMember();
				case "Padding":
					return ShouldSerializePadding();
				case "EmptyChartText":
					return ShouldSerializeEmptyChartText();
				case "SmallChartText":
					return ShouldSerializeEmptyChartText();
				case "AutoBindingSettingsEnabled":
					return ShouldSerializeAutoBindingSettingsEnabled();
				case "AutoLayoutSettingsEnabled":
					return ShouldSerializeAutoLayoutSettingsEnabled();
				case "PivotGridDataSourceOptions":
					return ShouldSerializePivotGridDataSourceOptions();
				case "ToolTipOptions":
					return ShouldSerializeToolTipOptions();
				case "CrosshairEnabled":
					return ShouldSerializeCrosshairEnabled();
				case "CrosshairOptions":
					return ShouldSerializeCrosshairOptions();
				case "ToolTipEnabled":
					return ShouldSerializeToolTipEnabled();
				case "RuntimeSelection":
					return ShouldSerializeRuntimeSelection();
				case "SelectedItemsInfos":
					return ShouldSerializeSelectedItemsInfos();
				case "AutoLayout":
					return ShouldSerializeAutoLayout();
				case "RightToLeft":
					return ShouldSerializeRightToLeft();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		void IXtraSerializable.OnStartSerializing() {
			xtraSerializing = true;
			selectionController.FillSelectedItemInfos();
		}
		void IXtraSerializable.OnEndSerializing() {
			xtraSerializing = false;
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			xtraSerializing = true;
			loading = true;
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			loading = false;
			OnEndLoading(this, EventArgs.Empty);
			xtraSerializing = false;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "SeriesSerializable":
					((IXtraSupportDeserializeCollectionItem)DataContainer).SetIndexCollectionItem(propertyName, e);
					break;
				case "Titles":
					ChartTitle title = e.Item.Value as ChartTitle;
					if (title != null)
						titles.Add(title);
					break;
				case "XtraPaletteRepository": {
						XtraPaletteWrapper paletteWrapper = e.Item.Value as XtraPaletteWrapper;
						if (paletteWrapper != null)
							xtraPaletteRepository.Add(paletteWrapper);
						break;
					}
				case "XtraIndicatorsPaletteRepository": {
						XtraPaletteWrapper paletteWrapper = e.Item.Value as XtraPaletteWrapper;
						if (paletteWrapper != null)
							xtraIndicatorsPaletteRepository.Add(paletteWrapper);
						break;
					}
				case "AnnotationRepository":
					Annotation annotation = e.Item.Value as Annotation;
					if (annotationRepository != null)
						annotationRepository.Add(annotation);
					break;
				case "SelectedItemsInfos":
					SelectedItemInfo selectedInfo = e.Item.Value as SelectedItemInfo;
					if (SelectedItemsInfos != null)
						((IList)SelectedItemsInfos).Add(selectedInfo);
					break;
			}
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "SeriesSerializable":
					return ((IXtraSupportDeserializeCollectionItem)DataContainer).CreateCollectionItem(propertyName, e);
				case "Titles":
					return new ChartTitle();
				case "XtraPaletteRepository":
					return new XtraPaletteWrapper();
				case "XtraIndicatorsPaletteRepository":
					return new XtraPaletteWrapper();
				case "AnnotationRepository":
					return XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) as Annotation;
				case "SelectedItemsInfos":
					return new SelectedItemInfo();
				default:
					return null;
			}
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return e.Item.Name == "Diagram" ? XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "Diagram")
				Diagram = e.Item.Value as Diagram;
		}
		#endregion
		#region ISupportRangeControl implementation
		internal ISupportRangeControl ActualRangeSupport {
			get {
				ISupportRangeControl diagramSupport = Diagram as ISupportRangeControl;
				if (diagramSupport != null)
					return diagramSupport;
				return dummyDiagramRange as ISupportRangeControl;
			}
		}
		bool ISupportRangeControl.IsValid {
			get {
				return ActualRangeSupport.IsValid;
			}
		}
		string ISupportRangeControl.InvalidText {
			get {
				return ActualRangeSupport.InvalidText;
			}
		}
		int ISupportRangeControl.TopIndent {
			get {
				return ActualRangeSupport.TopIndent;
			}
		}
		int ISupportRangeControl.BottomIndent {
			get {
				return ActualRangeSupport.BottomIndent;
			}
		}
		void ISupportRangeControl.OnRangeControlChanged(object sender) {
			rangeControl = sender;
			ActualRangeSupport.OnRangeControlChanged(sender);
		}
		NormalizedRange ISupportRangeControl.ValidateNormalRange(NormalizedRange range, RangeValidationBase validationBase) {
			return ActualRangeSupport.ValidateNormalRange(range, validationBase);
		}
		List<object> ISupportRangeControl.GetValuesInRange(object min, object max, int scaleLength) {
			return ActualRangeSupport.GetValuesInRange(min, max, scaleLength);
		}
		string ISupportRangeControl.ValueToString(double normalizedValue) {
			return ActualRangeSupport.ValueToString(normalizedValue);
		}
		string ISupportRangeControl.RulerToString(int rulerIndex) {
			return ActualRangeSupport.RulerToString(rulerIndex);
		}
		object ISupportRangeControl.ProjectBackValue(double normalOffset) {
			return ActualRangeSupport.ProjectBackValue(normalOffset);
		}
		double ISupportRangeControl.ProjectValue(object value) {
			return ActualRangeSupport.ProjectValue(value);
		}
		void ISupportRangeControl.DrawContent(IRangeControlPaint paint) {
			ActualRangeSupport.DrawContent(paint);
		}
		void ISupportRangeControl.RangeChanged(object minValue, object maxValue) {
			ActualRangeSupport.RangeChanged(minValue, maxValue);
		}
		bool ISupportRangeControl.CheckTypeSupport(Type type) {
			return ActualRangeSupport.CheckTypeSupport(type);
		}
		void ISupportRangeControl.Invalidate(bool redraw) {
			ActualRangeSupport.Invalidate(redraw);
		}
		object ISupportRangeControl.NativeValue(double normalOffset) {
			return ActualRangeSupport.NativeValue(normalOffset);
		}
		object ISupportRangeControl.GetOptions() {
			return ActualRangeSupport.GetOptions();
		}
		#endregion
		#region IBackground implementation
		FillStyleBase IBackground.FillStyle { get { return FillStyle; } }
		bool IBackground.BackImageSupported { get { return true; } }
		#endregion
		#region IHitTest implementation
		HitTestState hitTestState = new HitTestState();
		object IHitTest.Object { get { return container; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		void EnsureChartAnnotationsBindedToDiagram(XYDiagram2D diagram) {
			foreach (Annotation annotation in AnnotationRepository) {
				PaneAnchorPoint anchorPoint = annotation.AnchorPoint as PaneAnchorPoint;
				if (anchorPoint != null) {
					Axis2D axisX = anchorPoint.AxisXCoordinate.Axis;
					Axis2D axisY = anchorPoint.AxisYCoordinate.Axis;
					XYDiagramPaneBase pane = anchorPoint.Pane;
					if (axisX == null || (axisX != diagram.ActualAxisX && !diagram.ActualSecondaryAxesX.ContainsInternal(axisX)))
						anchorPoint.AxisXCoordinate.ActualAxis = diagram.ActualAxisX;
					if (axisY == null || (axisY != diagram.ActualAxisY && !diagram.ActualSecondaryAxesY.ContainsInternal(axisY)))
						anchorPoint.AxisYCoordinate.ActualAxis = diagram.ActualAxisY;
					if (pane == null || (pane != diagram.DefaultPane && !diagram.Panes.Contains(pane)))
						anchorPoint.SetPane(diagram.DefaultPane);
				}
			}
		}
		void EnsureSeriesViewBindedToDiagram(SeriesViewBase seriesView, XYDiagram2D diagram) {
			XYDiagram2DSeriesViewBase view = seriesView as XYDiagram2DSeriesViewBase;
			if (view != null) {
				Axis2D actualAxisX = view.ActualAxisX;
				Axis2D actualAxisY = view.ActualAxisY;
				XYDiagramPaneBase actualPane = view.ActualPane;
				Axis2D diagramActualAxisX = diagram.ActualAxisX;
				Axis2D diagramActualAxisY = diagram.ActualAxisY;
				if (actualAxisX == null || (actualAxisX != diagramActualAxisX && !diagram.ActualSecondaryAxesX.ContainsInternal(actualAxisX)))
					view.ActualAxisX = diagramActualAxisX;
				if (actualAxisY == null || (actualAxisY != diagramActualAxisY && !diagram.ActualSecondaryAxesY.ContainsInternal(actualAxisY)))
					view.ActualAxisY = diagramActualAxisY;
				if (actualPane == null || (actualPane != diagram.DefaultPane && !diagram.Panes.Contains(actualPane)))
					view.ActualPane = diagram.DefaultPane;
				var indicatorOwnerSeriesView = seriesView as XYDiagram2DSeriesViewBase;
				if (indicatorOwnerSeriesView != null) {
					foreach (Indicator indicator in indicatorOwnerSeriesView.Indicators) {
						var separatePaneIndicator = indicator as SeparatePaneIndicator;
						if (separatePaneIndicator != null) {
							bool secondaryAxisRemoved = (separatePaneIndicator.AxisY != diagramActualAxisY && !diagram.ActualSecondaryAxesY.ContainsInternal(separatePaneIndicator.AxisY));
							if (separatePaneIndicator.AxisY == null || secondaryAxisRemoved)
								separatePaneIndicator.ActualAxisY = diagramActualAxisY;
							bool additionalPaneRemoved = (separatePaneIndicator.Pane != diagram.DefaultPane && !diagram.Panes.Contains(separatePaneIndicator.Pane));
							if (separatePaneIndicator.Pane == null || additionalPaneRemoved)
								separatePaneIndicator.ActualPane = diagram.DefaultPane;
						}
					}
				}
			}
		}
		void EnsureChartSeriesViewsBindedToDiagram(XYDiagram2D diagram) {
			foreach (SeriesBase seriesBase in DataContainer.Series)
				EnsureSeriesViewBindedToDiagram(seriesBase.View, diagram);
		}
		INativeGraphics CreateNativeGraphicsCore(Graphics gr, IntPtr hdc, Rectangle bounds, Rectangle windowsBounds, GraphicsQuality graphicsQuality) {
			try {
				if (diagram != null) {
					INativeGraphics graphics = diagram.CreateNativeGraphics(gr, hdc, bounds, windowsBounds);
					if (graphics != null) {
						graphics.GraphicsQuality = graphicsQuality;
						return graphics;
					}
				}
			}
			catch {
			}
			return new GdiPlusGraphics(gr);
		}
		void SetToolTipOptions(ToolTipOptions value) {
			toolTipOptions = value;
			toolTipOptions.Owner = this;
		}
		void CorrectOnBoundsChanging(Rectangle bounds) {
			if (isDisposed || Loading)
				return;
			PerformViewDataCalculationWithoutEvents(bounds, true);
			Diagram.OnUpdateBounds();
			UpdateAutomaticLayout(bounds);
		}
		void UpdateAutomaticLayout(Rectangle bounds) {
			if (!container.DesignMode && AutoLayout && diagram != null) {
				if (!diagram.LastBounds.IsEmpty)
					diagram.UpdateAutomaticLayout(diagram.LastBounds);
				List<ISupportVisibilityControlElement> chartElements = new List<ISupportVisibilityControlElement>();
				if (Legend.Visibility == DefaultBoolean.Default)
					chartElements.Add(Legend);
				foreach (ChartTitle title in Titles)
					if (title.Visibility == DefaultBoolean.Default)
						chartElements.Add(title);
				List<VisibilityLayoutRegion> diagramElements = diagram.GetAutolayoutElements(bounds);
				if (diagramElements != null) {
					if (Legend.Visibility == DefaultBoolean.True)
						foreach (VisibilityLayoutRegion region in diagramElements)
							region.ElementsToRemove.Add(Legend);
					if (diagramElements.Count == 0)
						diagramElements.Add(new VisibilityLayoutRegion(new GRealSize2D(bounds.Width, bounds.Height), new List<ISupportVisibilityControlElement>()));
					VisibilityCalculator calculator = new VisibilityCalculator(diagram.MinimumLayoutSize);
					calculator.CalculateLayout(chartElements, diagramElements);
				}
			}
		}
		void EndLoadAnnotations() {
			foreach (Annotation annotation in AnnotationRepository)
				annotation.OnEndLoading();
		}
		void OnEndLoading(object sender, EventArgs args) {
			viewController.StartOnEndLoading();
			DataContainer.EndLoadSeries(true);
			EndLoadAnnotations();
			if (diagram != null)
				diagram.OnEndLoading();
			if (xtraPaletteRepository != null) {
				xtraPaletteRepository.OnEndLoaing();
				xtraPaletteRepository = null;
			}
			if (xtraIndicatorsPaletteRepository != null) {
				xtraIndicatorsPaletteRepository.OnEndLoaing();
				xtraIndicatorsPaletteRepository = null;
			}
			if (!String.IsNullOrEmpty(paletteName)) {
				Palette palette = paletteRepository[paletteName];
				if (palette != null)
					this.palette = palette;
				paletteName = null;
			}
			if (!String.IsNullOrEmpty(indicatorsPaletteName)) {
				Palette palette = indicatorsPaletteRepository[indicatorsPaletteName];
				if (palette != null)
					indicatorsPalette = palette;
				indicatorsPaletteName = null;
			}
			if (toolTipOptions != null)
				toolTipOptions.OnEndLoading();
			if (crosshairOptions != null)
				crosshairOptions.OnEndLoading();
			dataContainer.UpdateBinding(true, true);
			viewController.ResetLoadingFlag();
			RaiseControlChanged(new OnLoadEndUpdateInfo(this));
			RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific));
			if (diagram != null)
				diagram.FinishLoading();
		}
		GraphicsCommand CreateGraphicsCommandInternal(Rectangle bounds) {
			return null;
		}
		void PrepareRenderInternal(Rectangle bounds) {
			if (diagram != null && (bounds != lastBounds || needLayoutUpdate)) {
				diagram.ClearResolveOverlappingCache();
				lastBounds = bounds;
				if (HasAutoLayoutElements && AutoLayout || diagram.DependsOnBounds)
					CorrectOnBoundsChanging(bounds);
				needLayoutUpdate = false;
			}
		}
		void RenderContent2D(Graphics graphics, Graphics graphicsMiddle, Graphics graphicsAbove, Rectangle bounds, bool shouldUpdateBounds) {
			hitTestController.Clear();
			if (shouldUpdateBounds)
				PrepareRenderInternal(bounds);
			Graphics = graphics;
			GraphicsMiddle = graphicsMiddle;
			GraphicsAbove = graphicsAbove;
			Render(bounds, true);
		}
		void RenderContent2D(Graphics graphics, Rectangle bounds) {
			RenderContent2D(graphics, graphics, graphics, bounds, true);
		}
		void UpdateCache(Rectangle bounds, int cacheCode) {
			PrepareRenderInternal(bounds);
			if (cacheBitmap != null)
				cacheBitmap.Dispose();
			cacheBitmap = new ChartBitmapCache(bounds, ActualCrosshairHighlightPoints);
			RenderContent2D(cacheBitmap.Graphics, cacheBitmap.GraphicsMiddle, cacheBitmap.GraphicsAbove, bounds, false);
			cacheBitmap.DisposeGraphics();
			cacheHashCode = cacheCode;
		}
		bool ShouldUpdateCache(int hashCode) {
			return (hashCode != cacheHashCode) || (cacheBitmap == null) || (!cacheToMemory && containerAdapter.HasCustomDrawEventsListeners);
		}
		bool DrawCachedContent2D(INativeGraphics gr, Rectangle bounds) {
			GdiPlusGraphics graphics = gr as GdiPlusGraphics;
			if (graphics == null)
				return false;
			int hashCode = CalculateDrawingHashcode(bounds.Size);
			if (ShouldUpdateCache(hashCode))
				UpdateCache(bounds, hashCode);
			List<CrosshairPaneViewData> crosshairViewData = null;
			XYDiagram2D xyDiagram2D = diagram as XYDiagram2D;
			if (xyDiagram2D != null && ActualCrosshairEnabled && !LockCrosshairForExport && !xyDiagram2D.IsEmpty) {
				List<IRefinedSeries> series = new List<IRefinedSeries>(ViewController.ActiveRefinedSeries);
				crosshairViewData = xyDiagram2D.CalculateCrosshairViewData(series);
			}
			renderer.Reset(graphics.Graphics, bounds);
			cacheBitmap.Render(renderer);
			if (crosshairViewData != null) {
				foreach (CrosshairPaneViewData crosshairPaneViewData in crosshairViewData)
					crosshairPaneViewData.RenderHighlighting(renderer);
			}
			if (cacheBitmap.GraphicsMiddle != null)
				cacheBitmap.RenderMiddle(renderer);
			if (crosshairViewData != null) {
				foreach (CrosshairPaneViewData crosshairPaneViewData in crosshairViewData)
					crosshairPaneViewData.Render(renderer);
			}
			cacheBitmap.RenderAbove(renderer);
			if (crosshairViewData != null) {
				foreach (CrosshairPaneViewData crosshairPaneViewData in crosshairViewData)
					crosshairPaneViewData.RenderAbove(renderer);
			}
			renderer.Present();
			return true;
		}
		void DrawNonCachedContent2D(INativeGraphics gr, Rectangle bounds) {
			GdiPlusGraphics graphics = gr as GdiPlusGraphics;
			if (graphics != null)
				RenderContent2D(graphics.Graphics, bounds);
			else {
				using (GraphicsCommand command = CreateGraphicsCommandInternal(bounds))
					gr.Execute(command);
			}
		}
		void DrawContent2D(INativeGraphics gr, Rectangle bounds, bool useImageCache) {
			if (bounds.Width > 0 && bounds.Height > 0) {
				if (!useImageCache || !(cacheToMemory || ActualCrosshairEnabled) || !DrawCachedContent2D(gr, bounds))
					DrawNonCachedContent2D(gr, bounds);
			}
		}
		void DrawContent3D(ChartDrawingContext context, bool lockDrawingHelper) {
			if (lockDrawingHelper || !DrawHighQualityImage(context.Graphics, context.Viewport)) {
				PrepareRenderInternal(context.Viewport);
				if (!lockDrawingHelper && drawingHelper != null)
					graphicsCache.GraphicsTree = drawingHelper.GetGraphicsTree(graphicsCache.Container);
				GraphicsCommand command = CreateGraphicsCommand(context, lockDrawingHelper);
				renderer.Present();
				if (command != null) {
					context.NativeGraphics.Execute(command);
					if (!lockDrawingHelper && context.NativeGraphics.GraphicsQuality == GraphicsQuality.Lowest)
						DrawingHelper.Process(context.Viewport.Size, command);
					else
						command.Dispose();
				}
			}
		}
		void DrawMetafile(Metafile metafile, Rectangle bounds) {
			using (Graphics gr = Graphics.FromImage(metafile))
				if (Is3DDiagram)
					using (Bitmap bmp = CreateBitmap(bounds.Size))
						gr.DrawImage(bmp, 0, 0);
				else
					DrawContent(gr, bounds, false);
		}
		HitTestParams PerformHitTesting(Point point, IList<HitTestParams> paramsArray, RaiseHitTestingDelegate raiseHitTesting) {
			ChartHitInfo hitInfo = new ChartHitInfo(point, paramsArray);
			foreach (HitTestParams hitParams in paramsArray) {
				HotTrackEventArgs e = new HotTrackEventArgs(hitParams.Object.Object, GetCorrectAdditionalObject(hitParams.AdditionalObj), hitInfo);
				raiseHitTesting(e);
				if (!e.Cancel)
					return hitParams;
			}
			return null;
		}
		object GetCorrectAdditionalObject(object additionalObject) {
			if (additionalObject is RefinedPoint)
				return SeriesPoint.GetSeriesPoint(((RefinedPoint)additionalObject).SeriesPoint);
			return additionalObject;
		}
		void SelectHitElementInternal(IHitTest hitElement, object addtionalHitObject, bool forceSelect, Keys modifierKeys, ChartFocusedArea focusedArea) {
			selectionController.SelectHitElementInternal(hitElement, addtionalHitObject, forceSelect, modifierKeys, focusedArea);
		}
		void SelectHitElementInternal(IHitTest hitElement, object addtionalHitObject, bool forceSelect, Keys modifierKeys) {
			selectionController.SelectHitElementInternal(hitElement, addtionalHitObject, forceSelect, modifierKeys, null);
		}
		void HotHitElementInternal(IHitTest hitElement, object addtionalHitObject) {
			selectionController.HotHitElementInternal(hitElement, addtionalHitObject);
		}
		void Export(ExportTarget exportTarget, Stream stream) {
			if (printer != null)
				printer.PerformPrintingAction(delegate() { printer.ComponentPrinter.Export(exportTarget, stream); });
		}
		void Export(ExportTarget exportTarget, string fileName) {
			if (printer != null)
				printer.PerformPrintingAction(delegate() { printer.ComponentPrinter.Export(exportTarget, fileName); });
		}
		void Export(ExportTarget exportTarget, Stream stream, ExportOptionsBase options) {
			if (printer != null)
				printer.PerformPrintingAction(delegate() { printer.ComponentPrinter.Export(exportTarget, stream, options); });
		}
		void Export(ExportTarget exportTarget, string fileName, ExportOptionsBase options) {
			if (printer != null)
				printer.PerformPrintingAction(delegate() { printer.ComponentPrinter.Export(exportTarget, fileName, options); });
		}
		void ExportToImageInternal(Stream stream, ImageFormat format) {
			if (containerAdapter.RenderProvider != null) {
				foreach (ImageCodecInfo item in ImageCodecInfo.GetImageEncoders())
					if (item.FormatID.Equals(format.Guid)) {
						using (Bitmap bitmap = CreateBitmap(containerAdapter.RenderProvider.DisplayBounds.Size))
							bitmap.Save(stream, item, null);
						return;
					}
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectImageFormat));
			}
		}
		void ExportToMetafile(Stream stream) {
			if (containerAdapter.RenderProvider != null) {
				using (Metafile metafile = CreateMetafileInstance(stream, containerAdapter.RenderProvider.DisplayBounds, MetafileFrameUnit.Pixel, EmfType.EmfPlusDual))
					DrawMetafile(metafile, containerAdapter.RenderProvider.DisplayBounds);
			}
		}
		void SetObjectSelectionInternal(IHitTest hitElement, object additionalObject, bool forceSelect) {
			HitTestParams hitParams = PerformHitTesting(Point.Empty,
				new HitTestParams[] { new HitTestParams(hitElement, additionalObject, null) }, containerAdapter.OnObjectSelected);
			if (hitParams != null)
				SelectHitElementInternal(hitParams.Object, hitParams.AdditionalObj, forceSelect, Keys.None);
		}
		void RedrawChart() {
			InvalidateDrawingHelper();
			ClearCache();
			containerAdapter.Invalidate();
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !isDisposed) {
				DataContainer.Dispose();
				ClearCache();
				DisposeDrawingHelper();
				isDisposed = true;
				if (container != null)
					container.EndLoading -= new EventHandler(OnEndLoading);
				hitTestController.Clear();
				fillStyle.Dispose();
				legend.Dispose();
				border.Dispose();
				toolTipOptions.Dispose();
				viewController.Dispose();
				smallChartText.Dispose();
				emptyChartText.Dispose();
				padding.Dispose();
				dataContainer.Dispose();
				crosshairOptions.Dispose();
				foreach (Diagram cachedDiagram in diagramCache)
					cachedDiagram.Dispose();
				diagramCache.Clear();
				if (diagram != null) {
					diagram.Dispose();
					diagram = null;
				}
				Series.Dispose();
				Series.Clear();
				annotationRepository.Clear();
				backImage.Dispose();
				titles.Dispose();
				annotations.Dispose();
				annotationRepository.Dispose();
				if (printer != null)
					printer.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override ChartElement CreateObjectForClone() {
			throw new NotSupportedException();
		}
		internal SeriesViewBase GetSeriesViewByName(string seriesName) {
			return DataContainer.GetSeriesByName(seriesName).View;
		}
		internal void PerformViewDataCalculationWithoutEvents(Rectangle bounds, bool correctRanges) {
			containerAdapter.AreEventsEnabled = false;
			PerformViewDataCalculation(bounds, correctRanges);
			containerAdapter.AreEventsEnabled = true;
		}
		internal void PerformViewDataCalculation(Rectangle bounds, bool correctRanges) {
			if (bounds.IsEmpty)
				bounds = containerAdapter.DisplayBounds;
			ChartViewData viewData = ChartViewData.Create(this, (ZPlaneRectangle)bounds, correctRanges);
			if (viewData != null)
				viewData.Dispose();
			diagram.ClearResolveOverlappingCache();
		}
		internal int CalculateDrawingHashcode(Size size) {
			IHitTest selected = hitTestController.Selected;
			int selectedHashCode = selected != null ? selected.GetHashCode() : 0;
			foreach (Object obj in selectionController.SelectedItems)
				selectedHashCode ^= obj != null && !Object.Equals(obj, selected) ? obj.GetHashCode() : 0;
			IHitTest hot = hitTestController.Hot;
			int hotHashCode = hot == null ? 0 : hot.GetHashCode();
			return ((size.Width << 16) + size.Height) ^ ((hotHashCode << 16) + selectedHashCode);
		}
		internal INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hdc, Rectangle bounds, Rectangle windowsBounds, GraphicsQuality graphicsQuality) {
			return CreateNativeGraphicsCore(gr, hdc, bounds, windowsBounds, graphicsQuality);
		}
		internal ChartUpdateInfoBase EnsureDiagramType(Type diagramType) {
			ChartUpdateInfoBase updateForSeriesController = null;
			Type currentDiagramType = (diagram == null) ? null : diagram.GetType();
			if (currentDiagramType != diagramType) {
				if (diagram != null) {
					diagramCache.Save(diagram);
					diagram.ClearAnnotations();
				}
				if (diagramType != null) {
					Diagram previousDiagram = diagram;
					diagram = diagramCache.Load(diagramType);
					if (diagram == null) {
						diagram = (Diagram)Activator.CreateInstance(diagramType);
						diagram.Owner = this;
					}
					diagram.UpdateDiagramProperties(previousDiagram);
					updateForSeriesController = new PropertyUpdateInfo<IDiagram>(this, "Diagram", previousDiagram, diagram);
				}
				else {
					diagram = null;
				}
			}
			XYDiagram2D xyDiagram = diagram as XYDiagram2D;
			if (xyDiagram != null) {
				EnsureChartAnnotationsBindedToDiagram(xyDiagram);
				EnsureChartSeriesViewsBindedToDiagram(xyDiagram);
				if (dataContainer.SeriesTemplate != null)
					EnsureSeriesViewBindedToDiagram(dataContainer.SeriesTemplate.View, xyDiagram);
			}
			return updateForSeriesController;
		}
		internal void DrawContent(Graphics gr, Rectangle bounds, Rectangle windowsBounds, bool enableAntialiasing) {
			lock (this) {
				renderer.IsRightToLeft = container.GetActualRightToLeft();
				using (INativeGraphics graphics = CreateNativeGraphics(gr, IntPtr.Zero, bounds, windowsBounds, enableAntialiasing ? GraphicsQuality.Highest : GraphicsQuality.Lowest)) {
					ViewController.RefreshDataInternal(false);
					if (Is3DDiagram)
						DrawContent3D(new ChartDrawingContext() { Viewport = bounds, Window = windowsBounds, NativeGraphics = graphics, Graphics = gr }, true);
					else {
						DrawContent2D(graphics, bounds, false);
					}
				}
			}
		}
		public void PerformChanging() {
			SendNotification(new ElementWillChangeNotification(this));
		}
		public void PerformChanged() {
			RaiseControlChanged();
		}
		public void ResetPadding() {
			padding.All = defaultPadding;
		}
		public void EnableLoadingMode() { loading = true; }
		public void DisableLoadingMode() { loading = false; }
		public CoordinatesConversionCache GetCoordinatesConversionCache() {
			return drawing2DCache.GetCoordinatesConversionCache(CalculateDrawingHashcode(containerAdapter.DisplayBounds.Size));
		}
		public void SelectHitTestable(ChartElement element) {
			for (ChartElement elem = element; elem != null; elem = elem.Owner) {
				IHitTest hitTest = elem as IHitTest;
				if (hitTest != null) {
					ChartDesignHelper.SelectObject(this, elem);
					return;
				}
			}
		}
		public void SelectOwnerHitTestable(ChartElement element) {
			SelectHitTestable(element.Owner);
		}
		public void SelectOwnerHitTestable(ChartCollectionBase collection) {
			SelectHitTestable(collection.Owner);
		}
		public bool Contains(object obj) {
			return obj == diagram || obj == legend || Series.Contains(obj) || titles.Contains(obj) || annotationRepository.Contains(obj) || DataContainer.SeriesTemplate.Contains(obj) ||
				(diagram != null && diagram.Contains(obj));
		}
		public void SaveLayout(Stream stream) {
			XtraSerializingHelper.SaveLayoutToStream(this, stream);
		}
		public void LoadLayout(Stream stream) {
			Chart emptyChart = new Chart(new EmptyChartContainer(container.ControlType));
			try {
				XtraSerializingHelper.LoadLayoutFromStream(emptyChart, stream);
			}
			catch (XmlException) {
				throw new ChartLoadingException(ChartLocalizer.GetString(ChartStringId.MsgChartLoadingException));
			}
			SendNotification(new ElementWillChangeNotification(this));
			Assign(emptyChart);
			RaiseControlChanged();
		}
		public void RegisterSummaryFunction(string name, string displayName, ScaleType? resultScaleType, int resultDimension, SummaryFunctionArgumentDescription[] argumentDescriptions, SummaryFunction function) {
			summaryFunctions.Add(new SummaryFunctionDescription(name, displayName, resultScaleType, resultDimension, argumentDescriptions, function, false));
		}
		public void UnregisterSummaryFunction(string name) {
			summaryFunctions.Remove(name);
		}
		public void ResetSummaryFunctions() {
			summaryFunctions = DefaultSummaryFunctions.CreateStorage();
		}
		public void RefreshData(bool forceRefresh) {
			if (forceRefresh || !DataContainer.RefreshDataOnRepaint)
				ViewController.RefreshDataInternal(true);
			if (Is3DDiagram) {
				ResetGraphicsCache();
				InvalidateDrawingHelper();
			}
			containerAdapter.Invalidate();
		}
		public void FillDataSource() {
			if (DataContainer.CanFillDataSource()) {
				try {
					DataContainer.FillDataSource();
					viewController.Update();
				}
				catch (Exception e) {
					container.ShowErrorMessage(e.Message, String.Empty);
				}
			}
		}
		public void ClearDataSource() {
			if (DataContainer.CanClearDataSource()) {
				try {
					Data.Native.BindingHelper.ConvertToDataSet(DataContainer.DataSource).Clear();
					DataContainer.UpdateBinding(true, true);
				}
				catch (Exception e) {
					container.ShowErrorMessage(e.Message, string.Empty);
				}
			}
		}
		public void DataSnapshot() {
			DataContainer.DataSnapshot();
		}
		public void ClearCache() {
			needLayoutUpdate = true;
			cacheHashCode = 0;
			if (cacheBitmap != null)
				cacheBitmap.Dispose();
		}
		public void ResetGraphicsCache() {
			graphicsCache.Reset();
		}
		public void LockBinding() {
			DataContainer.LockBinding();
		}
		public void UnlockBinding() {
			DataContainer.UnlockBinding();
		}
		public INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hdc, Rectangle bounds, GraphicsQuality graphicsQuality) {
			return CreateNativeGraphics(gr, hdc, bounds, Rectangle.Empty, graphicsQuality);
		}
		public INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hdc, Size size, GraphicsQuality graphicsQuality) {
			return CreateNativeGraphics(gr, hdc, new Rectangle(Point.Empty, size), graphicsQuality);
		}
		public GraphicsCommand CreateGraphicsCommand(ChartDrawingContext context, bool lockDrawingHelper) {
			return viewController.CreateGraphicsCommand(context, lockDrawingHelper);
		}
		public void Render(Rectangle bounds, bool lockDrawingHelper) {
			renderer.IsRightToLeft = container.GetActualRightToLeft();
			viewController.RenderChart(renderer, bounds, lockDrawingHelper, Graphics, GraphicsMiddle, GraphicsAbove);
		}
		public void InvalidateDrawingHelper() {
			if (drawingHelper != null)
				drawingHelper.Invalidate();
		}
		public void DisposeDrawingHelper() {
			if (drawingHelper != null) {
				drawingHelper.Dispose();
				drawingHelper = null;
			}
		}
		public void DrawContent(Graphics graphics, INativeGraphics gr, Rectangle bounds, bool lockDrawingHelper, bool useImageCache) {
			lock (this) {
				renderer.IsRightToLeft = container.GetActualRightToLeft();
				ViewController.RefreshDataInternal(false);
				if (Is3DDiagram)
					DrawContent3D(new ChartDrawingContext() { Viewport = bounds, Window = bounds, NativeGraphics = gr, Graphics = graphics }, lockDrawingHelper);
				else {
					DrawContent2D(gr, bounds, useImageCache);
					containerAdapter.OnCustomPaint(new CustomPaintEventArgs(graphics, bounds));
				}
			}
		}
		public void DrawContent(Graphics gr, Rectangle bounds, bool useImageCache) {
			using (INativeGraphics graphics = CreateNativeGraphics(gr, IntPtr.Zero, bounds, GraphicsQuality.Highest))
				DrawContent(gr, graphics, bounds, true, useImageCache);
		}
		public void DrawContent(Graphics gr, Rectangle bounds) {
			DrawContent(gr, bounds, true);
		}
		public bool DrawHighQualityImage(Graphics graphics, Rectangle bounds) {
			if (drawingHelper == null || !drawingHelper.IsImageReady)
				return false;
			Image image = drawingHelper.GetImage();
			if (image != null) {
				if (image.Size == bounds.Size) {
					graphics.DrawImage(image, bounds);
					return true;
				}
				image.Dispose();
			}
			return false;
		}
		public Bitmap CreateBitmap(Size size) {
			CheckSize(size);
			return ChartBitmapContainer.Draw(this, size, GraphicsQuality.Highest);
		}
		public Metafile CreateMetafile(Size size, MetafileFrameUnit units) {
			CheckSize(size);
			Rectangle metafileBounds = new Rectangle(Point.Empty, new Size(size.Width + 1, size.Height + 1));
			Rectangle drawingBounds = new Rectangle(Point.Empty, size);
			Metafile metafile = CreateMetafileInstance(null, metafileBounds, units, EmfType.EmfPlusOnly);
			try {
				DrawMetafile(metafile, drawingBounds);
			}
			catch {
				metafile.Dispose();
				throw;
			}
			return metafile;
		}
		public List<object> HitTest(int x, int y) {
			IList<HitTestParams> hitTestParams = HitTestController.Find(new Point(x, y));
			List<object> list = new List<object>();
			foreach (HitTestParams hitParams in hitTestParams) {
				IHitTest hitElement = hitParams.Object;
				object obj = hitElement.Object;
				if (list.IndexOf(obj) == -1)
					list.Add(obj);
			}
			return list;
		}
		public ChartHitInfo CalcHitInfo(Point point) {
			if (Is3DDiagram)
				throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgCalcHitInfoNotSupported));
			return new ChartHitInfo(point, HitTestController.Find(point));
		}
		public ChartHitInfo CalcHitInfo(int x, int y) {
			return CalcHitInfo(new Point(x, y));
		}
		public SeriesPoint GetSeriesPoint(ISeriesPoint point) {
			return SeriesPoint.GetSeriesPoint(point);
		}
		public void ArrangeAnnotation(Annotation annotation) {
			FreePosition position = annotation.ShapePosition as FreePosition;
			if (position == null)
				return;
			int locationX = 0;
			int locationY = 0;
			annotation.UpdateSize();
			Rectangle displayBounds = containerAdapter.DisplayBounds;
			ZPlaneRectangle innerBounds = (ZPlaneRectangle)border.CalcBorderedRect(new RectangleF(0, 0, displayBounds.Width, displayBounds.Height));
			if (innerBounds.AreWidthAndHeightPositive()) {
				for (int i = annotationRepository.Count - 1; i >= 0; i--) {
					FreePosition shapePosition = annotationRepository[i].ShapePosition as FreePosition;
					if (shapePosition != null) {
						Point point = (Point)shapePosition.GetShapeLocation(innerBounds);
						locationX = point.X + Annotation.StartOffset.X;
						locationY = point.Y + Annotation.StartOffset.Y;
						if (locationX + annotation.Width >= innerBounds.Width)
							locationX = point.X;
						if (locationY + annotation.Height >= innerBounds.Height)
							locationY = point.Y;
						break;
					}
				}
			}
			position.DockTarget = null;
			position.SetLeftIndent(locationX);
			position.SetTopIndent(locationY);
			ChartAnchorPoint anchorPoint = annotation.AnchorPoint as ChartAnchorPoint;
			if (anchorPoint != null)
				anchorPoint.SetPosition(locationX, locationY + annotation.Height + 20);
		}
		public void SetObjectSelection(object obj, bool forceSelect) {
			InvalidateDrawingHelper();
			if (Is3DDiagram)
				return;
			if (SeriesSelectionMode != SeriesSelectionMode.Series && obj is Series ||
				SeriesSelectionMode == SeriesSelectionMode.Series && obj is SeriesPoint)
				return;
			if (obj == container)
				obj = this;
			IHitTest hitElement = obj as IHitTest;
			if (hitElement != null)
				SetObjectSelectionInternal(hitElement, null, forceSelect);
			else {
				SeriesPoint seriesPoint = obj as SeriesPoint;
				if (seriesPoint != null && seriesPoint.Series != null)
					SetObjectSelectionInternal(seriesPoint.Series, seriesPoint, forceSelect);
			}
		}
		public void ClearSelection(bool clearHot) {
			InvalidateDrawingHelper();
			selectionController.ClearSelection(true);
			containerAdapter.Invalidate();
		}
		public void ClearSelection() {
			ClearSelection(true);
		}
		public void SelectObjectsAt(Point p, bool isMouseDown) {
			SelectObjectsAt(p, isMouseDown, Keys.None);
		}
		public void SelectObjectsAt(Point p, bool isMouseDown, Keys modifierKeys) {
			if (!Is3DDiagram) {
				HitTestParams hitParams = null;
				if (isMouseDown) {
					hitParams = PerformHitTesting(p, hitTestController.Find(p), containerAdapter.OnObjectSelected);
					cachedSelectedHitParams = hitParams;
				}
				else {
					if (hitTestController.Items.Count > 0)
						hitParams = PerformHitTesting(p, hitTestController.Find(p), containerAdapter.OnObjectSelected);
					else
						hitParams = cachedSelectedHitParams;
					cachedSelectedHitParams = null;
				}
				if (hitParams != null)
					SelectHitElementInternal(hitParams.Object, hitParams.AdditionalObj, false, modifierKeys, hitTestController.FindFocusedArea(p));
			}
		}
		public void SelectObjectsAt(Point p) {
			SelectObjectsAt(p, true);
		}
		public void HighlightObjectsAt(Point p) {
			if (!Is3DDiagram) {
				IHitTest hitElement = null;
				object additionalHitObject = null;
				HitTestParams hitParams = PerformHitTesting(p, hitTestController.Find(p), containerAdapter.OnObjectHotTracked);
				if (hitParams != null) {
					hitElement = hitParams.Object;
					additionalHitObject = hitParams.AdditionalObj;
				}
				if (additionalHitObject is RefinedPoint)
					HotHitElementInternal(hitElement, ((RefinedPoint)additionalHitObject).SeriesPoint);
				else
					HotHitElementInternal(hitElement, additionalHitObject);
			}
		}
		public void SelectHitElement(IHitTest hitElement) {
			if (!Is3DDiagram)
				SelectHitElementInternal(hitElement, null, false, Keys.None);
		}
		public void HotHitElement(IHitTest hitElement) {
			if (!Is3DDiagram)
				HotHitElementInternal(hitElement, null);
		}
		public void ClearHot() {
			if (!Is3DDiagram) {
				new ChartHitTestEnumerator(this).DoLeave(null);
				containerAdapter.Invalidate();
			}
		}
		public Point GetZoomRegionPosition(Point p) {
			return diagram == null ? p : diagram.GetZoomRegionPosition(p);
		}
		public bool CanZoom(Point p) {
			return diagram != null && diagram.CanZoom(p);
		}
		public void Zoom(int delta, ZoomingKind zoomingKind, Object focusedElement) {
			if (diagram != null)
				diagram.Zoom(delta, zoomingKind, focusedElement);
		}
		public void PerformZoomIn(Rectangle rect) {
			if (diagram != null)
				diagram.ZoomIn(rect);
		}
		public void PerformZoomIn(Point point) {
			if (diagram != null)
				diagram.ZoomIn(point);
		}
		public void PerformZoomOut(Point point) {
			if (diagram != null)
				diagram.ZoomOut(point);
		}
		public void UndoZoom() {
			if (diagram != null)
				diagram.UndoZoom();
		}
		public bool CanDrag(Point p, MouseButtons button) {
			return diagram != null && diagram.CanDrag(p, button);
		}
		public bool PerformDragging(int x, int y, int dx, int dy, ChartScrollEventType scrollEventType, Object focusedElement) {
			if (!annotationNavigation.PerformDragging(x, y, dx, dy, focusedElement as IAnnotationDragPoint))
				if (diagram == null || !diagram.PerformDragging(x, y, dx, dy, scrollEventType, focusedElement))
					return false;
			RedrawChart();
			return true;
		}
		public void ExportToHtml(string filePath) {
			Export(ExportTarget.Html, filePath);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			Export(ExportTarget.Html, filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			Export(ExportTarget.Html, stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			Export(ExportTarget.Html, stream, options);
		}
		public void ExportToMht(string filePath) {
			Export(ExportTarget.Mht, filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			Export(ExportTarget.Mht, filePath, options);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			Export(ExportTarget.Mht, stream, options);
		}
		public void ExportToPdf(string filePath) {
			Export(ExportTarget.Pdf, filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			Export(ExportTarget.Pdf, filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			Export(ExportTarget.Pdf, stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			Export(ExportTarget.Pdf, stream, options);
		}
		public void ExportToRtf(string filePath) {
			Export(ExportTarget.Rtf, filePath);
		}
		public void ExportToRtf(Stream stream) {
			Export(ExportTarget.Rtf, stream);
		}
		public void ExportToXls(string filePath) {
			Export(ExportTarget.Xls, filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			Export(ExportTarget.Xls, filePath, options);
		}
		public void ExportToXls(Stream stream) {
			Export(ExportTarget.Xls, stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			Export(ExportTarget.Xls, stream, options);
		}
		public void ExportToXlsx(string filePath) {
			Export(ExportTarget.Xlsx, filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			Export(ExportTarget.Xlsx, filePath, options);
		}
		public void ExportToXlsx(Stream stream) {
			Export(ExportTarget.Xlsx, stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			Export(ExportTarget.Xlsx, stream, options);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			LockCrosshairForExport = true;
			if (format == ImageFormat.Emf || format == ImageFormat.Wmf)
				ExportToMetafile(stream);
			else
				ExportToImageInternal(stream, format);
			LockCrosshairForExport = false;
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {
				ExportToImage(fs, format);
				fs.Close();
			}
		}
		public void DrawZoomRectangle(Graphics gr, Rectangle rect) {
			if (diagram != null)
				diagram.DrawZoomRectangle(gr, rect);
		}
		public void BeginGestureZoom(Point center, double zoomDelta, IAnnotationDragPoint annotationDragPoint) {
			if (annotationDragPoint != null && annotationDragPoint.Annotation.RuntimeResizing)
				PerformGestureZoom(zoomDelta, annotationDragPoint);
			else if (diagram != null)
				diagram.BeginGestureZoom(center, zoomDelta);
		}
		public void PerformGestureZoom(double zoomDelta, IAnnotationDragPoint annotationDragPoint) {
			if (annotationDragPoint != null && annotationDragPoint.Annotation.RuntimeResizing) {
				annotationDragPoint.Annotation.SetWidth(ZoomValue(annotationDragPoint.Annotation.Width, zoomDelta));
				annotationDragPoint.Annotation.SetHeight(ZoomValue(annotationDragPoint.Annotation.Height, zoomDelta));
				RedrawChart();
			}
			else if (diagram != null)
				diagram.PerformGestureZoom(zoomDelta);
		}
		public void PerformGestureRotation(double degreeDelta, IAnnotationDragPoint annotationDragPoint) {
			if (annotationDragPoint != null && annotationDragPoint.Annotation.RuntimeRotation)
				annotationDragPoint.Annotation.SetAngle(MathUtils.StrongRound(MathUtils.NormalizeDegree(annotationDragPoint.Annotation.Angle + degreeDelta)));
			else if (diagram == null || !diagram.PerformGestureRotation(-degreeDelta))
				return;
			RedrawChart();
		}
		public string GetDesignerHint(Point p) {
			return diagram == null ? String.Empty : diagram.GetDesignerHint(p);
		}
		public PaletteEntry[] GetPaletteEntries(int count) {
			List<PaletteEntry> paletteEntries = new List<PaletteEntry>();
			for (int i = 0; i < count; i++)
				paletteEntries.Add(Palette.GetEntry(i, count, paletteBaseColorNumber));
			return paletteEntries.ToArray();
		}
		public Rectangle GetPaneMappingBounds(XYDiagramPaneBase pane) {
			return pane.LastMappingBounds == null ? Rectangle.Empty : pane.LastMappingBounds.Value;
		}
		public PaneAxesContainer GetPaneAxesData(XYDiagramPaneBase pane) {
			XYDiagram2D diagram = Diagram as XYDiagram2D;
			return diagram == null ? null : diagram.GetPaneAxesData(pane);
		}
		public int GetActualAxisID(Axis2D axis) {
			return axis == null ? -1 : axis.ActualAxisID;
		}
		public int GetActualPaneID(XYDiagramPaneBase pane) {
			return pane == null ? -1 : pane.ActualPaneID;
		}
		public IList<AxisInterval> GetAxisIntervals(Axis2D axis) {
			return axis.Intervals;
		}
		public AxisIntervalLayoutCache GetIntervalBoundsCache(Axis2D axis) {
			return axis.IntervalBoundsCache;
		}
		public int GetSeriesViewPointDimension(SeriesViewBase view) {
			return view.PointDimension;
		}
		public override void Assign(ChartElement obj) {
			loading = true;
			base.Assign(obj);
			Chart chart = obj as Chart;
			if (chart != null) {
				rightToLeft = chart.rightToLeft;
				fillStyle.Assign(chart.fillStyle);
				backColor = chart.backColor;
				if (SupportDataMember)
					DataContainer.DataMember = chart.DataContainer.DataMember;
				if (SupportBorder)
					border.Assign(chart.border);
				backImage.Assign(chart.backImage);
				titles.Assign(chart.titles);
				DataContainer.Assign(chart.DataContainer);
				ChartUpdateInfoBase updateInfo;
				if (chart.diagram == null)
					updateInfo = EnsureDiagramType(null);
				else {
					updateInfo = EnsureDiagramType(chart.diagram.GetType());
					diagram.Assign(chart.diagram);
					if (diagram != null)
						diagram.OnEndLoading();
				}
				autoLayout = chart.autoLayout;
				annotationRepository.Assign(chart.annotationRepository);
				legend.Assign(chart.legend);
				sideBySideBarDistance = chart.sideBySideBarDistance;
				sideBySideBarDistanceFixed = chart.sideBySideBarDistanceFixed;
				sideBySideEqualBarWidth = chart.sideBySideEqualBarWidth;
				paletteRepository.Assign(chart.paletteRepository);
				palette = paletteRepository.GetPaletteByName(chart.palette.Name);
				indicatorsPaletteRepository.Assign(chart.indicatorsPaletteRepository);
				indicatorsPalette = indicatorsPaletteRepository.GetPaletteByName(chart.indicatorsPalette.Name);
				appearance = appearanceRepository[chart.appearance.SerializableName];
				if (container != null && container.ControlType != ChartContainerType.XRControl)
					appearance.Assign(chart.Appearance);
				paletteBaseColorNumber = chart.paletteBaseColorNumber;
				CacheToMemory = chart.cacheToMemory;
				optionsPrint.Assign(chart.optionsPrint);
				if (printer != null && chart.printer != null)
					printer.Assign(chart.printer);
				DataContainer.EndLoadSeries(false);
				EndLoadAnnotations();
				summaryFunctions.Assign(chart.summaryFunctions);
				padding.Assign(chart.padding);
				emptyChartText.Assign(chart.emptyChartText);
				smallChartText.Assign(chart.smallChartText);
				SelectionMode = SupportRuntimeSelection ? chart.SelectionMode : ElementSelectionMode.None;
				SeriesSelectionMode = chart.SeriesSelectionMode;
				toolTipOptions.Assign(chart.toolTipOptions);
				crosshairEnabled = chart.crosshairEnabled;
				toolTipEnabled = chart.toolTipEnabled;
				crosshairOptions.Assign(chart.crosshairOptions);
				RaiseControlChanged(updateInfo);
				loading = false;
				DataContainer.UpdateBinding(true, false, false);
				SelectionController.Assign(chart.SelectionController);
				ViewController.OnEndLoading();
				if (diagram != null)
					diagram.FinishLoading();
			}
		}
		public void LoadColorsForDefaultPalette(string actualPaletteName) {
			Palettes.Default.LoadColorsFromActualPalette(actualPaletteName);
		}
		public void Print(PrintSizeMode sizeMode) {
			if (printer != null)
				printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)printer.ComponentPrinter).Print(); }, sizeMode);
		}
		public void PrintDialog(PrintSizeMode sizeMode) {
			if (printer != null)
				printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)printer.ComponentPrinter).PrintDialog(); }, sizeMode);
		}
		public void ShowPrintPreview(PrintSizeMode sizeMode) {
			IChartRenderProvider renderProvider = containerAdapter.RenderProvider;
			if (printer.IsPrintingAvailable && renderProvider != null)
				printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)printer.ComponentPrinter).ShowPreview(renderProvider.LookAndFeel); }, sizeMode);
		}
		public void ShowRibbonPrintPreview(PrintSizeMode sizeMode) {
			IChartRenderProvider renderProvider = containerAdapter.RenderProvider;
			if (printer.IsPrintingAvailable && renderProvider != null)
				printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)printer.ComponentPrinter).ShowRibbonPreview(renderProvider.LookAndFeel); }, sizeMode);
		}
	}
	public class ChartDrawingContext {
		public Rectangle Viewport { get; set; }
		public Rectangle Window { get; set; }
		public INativeGraphics NativeGraphics { get; set; }
		public Graphics Graphics { get; set; }
	}
}
