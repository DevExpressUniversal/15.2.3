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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts {
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabData),
	Designer("DevExpress.XtraCharts.Design.ChartControlDesigner," + AssemblyInfo.SRAssemblyChartsDesign),
	TypeConverter(typeof(DevExpress.XtraCharts.Design.ChartTypeConverter)),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "ChartControl.bmp"),
	Description("Implements 2D and 3D charting."),
	Docking(DockingBehavior.Ask),
	DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false),
	ChartControl.ChartCustomBindingProperties()
	]
	public partial class ChartControl : Control, IChartContainer, IChartRenderProvider, IChartDataProvider, IChartEventsProvider, IChartInteractionProvider, IPrintable, ISupportInitialize, ICloneable, ICoreReference, IGestureClient, IToolTipControlClient, IRangeControlClientExtension {
		#region DataAccess: ChartCustomBindingPropertiesAttribute nested class
		protected class ChartCustomBindingPropertiesAttribute :
			CustomBindingPropertiesAttribute {
			public class ChartCustomBindingProperty : ICustomBindingProperty {
				string ICustomBindingProperty.Description {
					get { return string.Empty; }
				}
				string ICustomBindingProperty.DisplayName {
					get { return "Series DataMember"; }
				}
				string ICustomBindingProperty.PropertyName {
					get { return "SeriesDataMember"; }
				}
			}
			public override IEnumerable<ICustomBindingProperty> GetCustomBindingProperties() {
				return new ICustomBindingProperty[] { new ChartCustomBindingProperty() };
			}
		}
		#endregion DataAccess#
		static readonly object endLoading = new object();
		static readonly object objectSelected = new object();
		static readonly object selectedItemsChanged = new object();
		static readonly object objectHotTracked = new object();
		static readonly object customDrawSeries = new object();
		static readonly object customDrawCrosshair = new object();
		static readonly object customDrawSeriesPoint = new object();
		static readonly object customDrawAxisLabel = new object();
		static readonly object customPaint = new object();
		static readonly object scroll = new object();
		static readonly object scroll3D = new object();
		static readonly object zoom = new object();
		static readonly object zoom3D = new object();
		static readonly object boundDataChanged = new object();
		static readonly object pieSeriesPointExploded = new object();
		static readonly object dateTimeMeasurementUnitsCalculated = new object();
		static readonly object queryCursor = new object();
		static readonly object customizeAutoBindingSettings = new object();
		static readonly object pivotChartingCustomizeXAxisLabels = new object();
		static readonly object pivotChartingCustomizeResolveOverlappingMode = new object();
		static readonly object customizeSimpleDiagramLayout = new object();
		static readonly object pivotGridSeriesExcluded = new object();
		static readonly object pivotGridSeriesPointsExcluded = new object();
		static readonly object pivotChartingCustomizeLegend = new object();
		static readonly object legendItemChecked = new object();
		static readonly object axisScaleChanged = new object();
		static readonly object axisWholeRangeChanged = new object();
		static readonly object axisVisualRangeChanged = new object();
		[DllImport("user32.dll")]
		static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		static ChartControl() {
		}
		static ToolTipLocation ConvertToolTipDockCorner(ToolTipDockCorner? dockCorner) {
			if (dockCorner.HasValue) {
				switch (dockCorner.Value) {
					case ToolTipDockCorner.TopLeft:
						return ToolTipLocation.RightBottom;
					case ToolTipDockCorner.TopRight:
						return ToolTipLocation.LeftBottom;
					case ToolTipDockCorner.BottomLeft:
						return ToolTipLocation.RightTop;
					case ToolTipDockCorner.BottomRight:
						return ToolTipLocation.LeftTop;
					default:
						return ToolTipLocation.TopCenter;
				}
			}
			return ToolTipLocation.TopCenter;
		}
		static DefaultBoolean RtlToDefaultBoolean(RightToLeft rtl) {
			switch (rtl) {
				case RightToLeft.Inherit:
					return DefaultBoolean.Default;
				case RightToLeft.Yes:
					return DefaultBoolean.True;
				default:
					return DefaultBoolean.False;
			}
		}
		static RightToLeft DefaultBooleanToRtl(DefaultBoolean defaultBool) {
			switch (defaultBool) {
				case DefaultBoolean.Default:
					return RightToLeft.Inherit;
				case DefaultBoolean.True:
					return RightToLeft.Yes;
				default:
					return RightToLeft.No;
			}
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinCharts));
		}
		readonly Chart chart;
		readonly UserLookAndFeel lookAndFeel;
		readonly ChartNavigationController navigationController;
		readonly RangeControlClient rangeControlClient;
		bool isDisposed = false;
		bool loading;
		bool runtimeHitTesting = false;
		bool runtimeRotation = false;
		bool hardwareAcceleration = true;
		int lockChangeServiceCounter = 0;
		IntPtr windowDC = IntPtr.Zero;
		Graphics windowGraphics = null;
		OpenGLGraphics openGLGraphics = null;
		GestureHelper gestureHelper;
		ToolTipController toolTipController;
		ChartCommandFactory commandFactory;
		event EventHandler updateUI;
		ChartPrintingDesigner printingDesigner;
		bool allowGesture = true;
		bool Mode3D { get { return chart.Is3DDiagram; } }
		DataContainer DataContainer { get { return chart.DataContainer; } }
		bool IsChartControlChild { get { return GetType() != typeof(ChartControl); } }
		IRangeControlClientExtension RangeControlClient { get { return rangeControlClient; } }
		internal Chart Chart { get { return chart; } }
		#region Hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Image BackgroundImage { get { return null; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color ForeColor { get { return Color.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Text { get { return String.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Font Font { get { return null; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new object Tag { get { return null; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object DataBindings { get { return null; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { } }
		[Browsable(false)]
		public GraphicsUnit DisplayUnits { get { return GraphicsUnit.Pixel; } }
		[Browsable(false)]
		public Rectangle DisplayBounds { get { return new Rectangle(Point.Empty, Bounds.Size); } }
		#endregion
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlAutoLayout"),
#endif
		Category(Categories.Layout),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public bool AutoLayout {
			get { return chart.AutoLayout; }
			set { chart.AutoLayout = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlLookAndFeel"),
#endif
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlAnnotations"),
#endif
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		]
		public AnnotationCollection Annotations { get { return chart.Annotations; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlAnnotationRepository"),
#endif
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		]
		public AnnotationRepository AnnotationRepository { get { return chart.AnnotationRepository; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSeriesTemplate"),
#endif
		Category(Categories.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		RefreshProperties(RefreshProperties.All)
		]
		public SeriesBase SeriesTemplate { get { return DataContainer.SeriesTemplate; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSeriesNameTemplate"),
#endif
		Category(Categories.Data),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SeriesNameTemplate SeriesNameTemplate { get { return DataContainer.SeriesNameTemplate; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSeries"),
#endif
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.SeriesCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
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
	DevExpressXtraChartsUILocalizedDescription("ChartControlDiagram"),
#endif
		Category(Categories.Elements),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public Diagram Diagram {
			get { return chart.Diagram; }
			set { chart.Diagram = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlLegend"),
#endif
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public Legend Legend { get { return chart.Legend; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlFillStyle"),
#endif
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle { get { return chart.FillStyle; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlBackColor"),
#endif
		Category(Categories.Appearance)
		]
		public new Color BackColor {
			get { return chart.BackColor; }
			set { chart.BackColor = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlDataAdapter"),
#endif
		Category(Categories.Data),
		DefaultValue(null),
		TypeConverter("DevExpress.XtraCharts.Design.DataAdapterTypeConverter," + AssemblyInfo.SRAssemblyCharts)
		]
		public object DataAdapter {
			get { return DataContainer.DataAdapter; }
			set { DataContainer.DataAdapter = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlDataSource"),
#endif
		Category(Categories.Data),
		RefreshProperties(RefreshProperties.All),
		AttributeProvider(typeof(IListSource))
		]
		public object DataSource {
			get { return DataContainer.DataSource; }
			set { DataContainer.DataSource = value; }
		}
		[
		Obsolete("The ChartControl.AutoBindingSettingsEnabled property is now obsolete. Use the ChartControl.PivotGridDataSourceOptions.AutoBindingSettingsEnabled property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public bool AutoBindingSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled = value; }
		}
		[
		Obsolete("The ChartControl.AutoBindingSettingsEnabled property is now obsolete. Use the ChartControl.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public bool AutoLayoutSettingsEnabled {
			get { return chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled; }
			set { chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlPivotGridDataSourceOptions"),
#endif
		Category(Categories.Data),
		TypeConverter(typeof(PivotGridDataSourceOptionsTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PivotGridDataSourceOptions PivotGridDataSourceOptions { get { return DataContainer.PivotGridDataSourceOptions; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSeriesDataMember"),
#endif
		Category(Categories.Data),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public string SeriesDataMember {
			get { return DataContainer.SeriesDataMember; }
			set { DataContainer.SeriesDataMember = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSeriesSorting"),
#endif
		Category(Categories.Data)
		]
		public SortingMode SeriesSorting {
			get { return chart.DataContainer.BoundSeriesSorting; }
			set { chart.DataContainer.BoundSeriesSorting = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlPadding"),
#endif
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public new RectangleIndents Padding {
			get { return chart.Padding; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The ChartControl.Border property is now obsolete. Use the ChartControl.BorderOptions property instead.")
		]
		public RectangularBorder Border { get { return BorderOptions; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlBorderOptions"),
#endif
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RectangularBorder BorderOptions { get { return chart.Border; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlAppearanceName"),
#endif
		Category(Categories.Appearance),
		TypeConverter("DevExpress.XtraCharts.Design.AppearanceTypeConverter," + AssemblyInfo.SRAssemblyCharts)
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
	DevExpressXtraChartsUILocalizedDescription("ChartControlPaletteName"),
#endif
		Category(Categories.Appearance),
		Editor("DevExpress.XtraCharts.Design.PaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts)
		]
		public string PaletteName {
			get { return chart.PaletteName; }
			set {
				if (value == DefaultPalette.DefaultPaletteName) {
					ISkinProvider skinProvider = lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin ? lookAndFeel : null;
					string actualPaletteName = GetActualPaletteNameFromSkin(skinProvider);
					chart.LoadColorsForDefaultPalette(actualPaletteName);
				}
				chart.PaletteName = value;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PaletteRepository PaletteRepository { get { return chart.PaletteRepository; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlIndicatorsPaletteName"),
#endif
		Category(Categories.Appearance),
		Editor("DevExpress.XtraCharts.Design.PaletteTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts)
		]
		public string IndicatorsPaletteName {
			get { return chart.IndicatorsPaletteName; }
			set { chart.IndicatorsPaletteName = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PaletteRepository IndicatorsPaletteRepository { get { return chart.IndicatorsPaletteRepository; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlPaletteBaseColorNumber"),
#endif
		Category(Categories.Appearance)
		]
		public int PaletteBaseColorNumber {
			get { return chart.PaletteBaseColorNumber; }
			set { chart.PaletteBaseColorNumber = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The ChartControl.RuntimeSeriesSelectionMode property is now obsolete. Use the ChartControl.SeriesSelectionMode property instead.")
		]
		public SeriesSelectionMode RuntimeSeriesSelectionMode {
			get { return chart.SeriesSelectionMode; }
			set { chart.SeriesSelectionMode = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The ChartControl.RuntimeSelection property is now obsolete. Use the ChartControl.SelectionMode property instead.")
		]
		public bool RuntimeSelection {
			get { return SelectionMode != ElementSelectionMode.None; }
			set { SelectionMode = value ? ElementSelectionMode.Single : ElementSelectionMode.None; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSelectionMode"),
#endif
		Category(Categories.Behavior),
		DefaultValue(ElementSelectionMode.None),
		RefreshProperties(RefreshProperties.All)
		]
		public ElementSelectionMode SelectionMode {
			get { return chart.SelectionMode; }
			set { chart.SelectionMode = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSeriesSelectionMode"),
#endif
		Category(Categories.Behavior),
		DefaultValue(SeriesSelectionMode.Series)
		]
		public SeriesSelectionMode SeriesSelectionMode {
			get { return chart.SeriesSelectionMode; }
			set { chart.SeriesSelectionMode = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSelectedItems"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IList SelectedItems { get { return chart.SelectedItems; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlRuntimeHitTesting"),
#endif
		Category(Categories.Behavior),
		DefaultValue(false)
		]
		public bool RuntimeHitTesting {
			get { return runtimeHitTesting; }
			set { runtimeHitTesting = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlHardwareAcceleration"),
#endif
		Category(Categories.Behavior),
		DefaultValue(true)
		]
		public bool HardwareAcceleration {
			get { return hardwareAcceleration; }
			set {
				hardwareAcceleration = value;
				InitializeDrawing();
			}
		}
		[
		Obsolete("The ChartControl.RuntimeRotation property is now obsolete. Use the Diagram3D.RuntimeRotation property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool RuntimeRotation {
			get { return runtimeRotation; }
			set {
				if (loading)
					runtimeRotation = value;
				else {
					Diagram3D diagram = chart.Diagram as Diagram3D;
					if (diagram != null)
						diagram.RuntimeRotation = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlTitles"),
#endif
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.ChartTitleEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor))
		]
		public ChartTitleCollection Titles { get { return chart.Titles; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlBackImage"),
#endif
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public BackgroundImage BackImage { get { return chart.BackImage; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlCacheToMemory"),
#endif
		Category(Categories.Behavior)
		]
		public bool CacheToMemory {
			get { return chart.CacheToMemory; }
			set { chart.CacheToMemory = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlRefreshDataOnRepaint"),
#endif
		Category(Categories.Behavior)
		]
		public bool RefreshDataOnRepaint {
			get { return DataContainer.RefreshDataOnRepaint; }
			set { DataContainer.RefreshDataOnRepaint = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlEmptyChartText"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public EmptyChartText EmptyChartText { get { return chart.EmptyChartText; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlSmallChartText"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SmallChartText SmallChartText { get { return chart.SmallChartText; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlToolTipController"),
#endif
		Category(CategoryName.Appearance),
		DefaultValue(null)
		]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if (value == ToolTipController.DefaultController) value = null;
				if (ToolTipController == value) return;
				if (ToolTipController != null) {
					ToolTipController.RemoveClientControl(this);
					if (ToolTipController != ToolTipController.DefaultController)
						ToolTipController.Disposed -= new EventHandler(OnToolTipControllerDisposed);
				}
				toolTipController = value;
				if (ToolTipController != null) {
					ToolTipController.AddClientControl(this);
					if (ToolTipController != ToolTipController.DefaultController)
						ToolTipController.Disposed += new EventHandler(OnToolTipControllerDisposed);
					ToolTipController.DefaultController.RemoveClientControl(this);
				}
				else
					ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlToolTipOptions"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ToolTipOptions ToolTipOptions { get { return chart.ToolTipOptions; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlCrosshairEnabled"),
#endif
		Category(Categories.Behavior),
		DefaultValue(DefaultBoolean.True)
		]
		public DefaultBoolean CrosshairEnabled {
			get { return chart.CrosshairEnabled; }
			set { chart.CrosshairEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlToolTipEnabled"),
#endif
		Category(Categories.Behavior),
		DefaultValue(DefaultBoolean.Default)
		]
		public DefaultBoolean ToolTipEnabled {
			get { return chart.ToolTipEnabled; }
			set { chart.ToolTipEnabled = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlCrosshairOptions"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CrosshairOptions CrosshairOptions { get { return chart.CrosshairOptions; } }
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
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlOptionsPrint"),
#endif
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartOptionsPrint OptionsPrint { get { return chart.OptionsPrint; } }
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlAllowGesture"),
#endif
		Category(Categories.Behavior)
		]
		public bool AllowGesture {
			get { return allowGesture; }
			set { allowGesture = value; }
		}
		[
#if !SL
	DevExpressXtraChartsUILocalizedDescription("ChartControlRightToLeft"),
#endif
		Category(Categories.Appearance),
		DefaultValue(RightToLeft.Inherit)
		]
		public override RightToLeft RightToLeft {
			get { return DefaultBooleanToRtl(chart.RightToLeft); }
			set { chart.RightToLeft = RtlToDefaultBoolean(value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlObjectSelected")]
#endif
		public event HotTrackEventHandler ObjectSelected {
			add { Events.AddHandler(objectSelected, value); }
			remove { Events.RemoveHandler(objectSelected, value); }
		}
		public event SelectedItemsChangedEventHandler SelectedItemsChanged {
			add { Events.AddHandler(selectedItemsChanged, value); }
			remove { Events.RemoveHandler(selectedItemsChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlObjectHotTracked")]
#endif
		public event HotTrackEventHandler ObjectHotTracked {
			add { Events.AddHandler(objectHotTracked, value); }
			remove { Events.RemoveHandler(objectHotTracked, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomDrawSeries")]
#endif
		public event CustomDrawSeriesEventHandler CustomDrawSeries {
			add { Events.AddHandler(customDrawSeries, value); }
			remove { Events.RemoveHandler(customDrawSeries, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomDrawCrosshair")]
#endif
		public event CustomDrawCrosshairEventHandler CustomDrawCrosshair {
			add { Events.AddHandler(customDrawCrosshair, value); }
			remove { Events.RemoveHandler(customDrawCrosshair, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomDrawSeriesPoint")]
#endif
		public event CustomDrawSeriesPointEventHandler CustomDrawSeriesPoint {
			add { Events.AddHandler(customDrawSeriesPoint, value); }
			remove { Events.RemoveHandler(customDrawSeriesPoint, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomDrawAxisLabel")]
#endif
		public event CustomDrawAxisLabelEventHandler CustomDrawAxisLabel {
			add { Events.AddHandler(customDrawAxisLabel, value); }
			remove { Events.RemoveHandler(customDrawAxisLabel, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomPaint")]
#endif
		public event CustomPaintEventHandler CustomPaint {
			add { Events.AddHandler(customPaint, value); }
			remove { Events.RemoveHandler(customPaint, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlScroll")]
#endif
		public event ChartScrollEventHandler Scroll {
			add { Events.AddHandler(scroll, value); }
			remove { Events.RemoveHandler(scroll, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlScroll3D")]
#endif
		public event ChartScroll3DEventHandler Scroll3D {
			add { Events.AddHandler(scroll3D, value); }
			remove { Events.RemoveHandler(scroll3D, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlZoom")]
#endif
		public event ChartZoomEventHandler Zoom {
			add { Events.AddHandler(zoom, value); }
			remove { Events.RemoveHandler(zoom, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlZoom3D")]
#endif
		public event ChartZoom3DEventHandler Zoom3D {
			add { Events.AddHandler(zoom3D, value); }
			remove { Events.RemoveHandler(zoom3D, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlBoundDataChanged")]
#endif
		public event BoundDataChangedEventHandler BoundDataChanged {
			add { Events.AddHandler(boundDataChanged, value); }
			remove { Events.RemoveHandler(boundDataChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlPieSeriesPointExploded")]
#endif
		public event PieSeriesPointExplodedEventHandler PieSeriesPointExploded {
			add { Events.AddHandler(pieSeriesPointExploded, value); }
			remove { Events.RemoveHandler(pieSeriesPointExploded, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlAxisScaleChanged")]
#endif
		public event EventHandler<AxisScaleChangedEventArgs> AxisScaleChanged {
			add { Events.AddHandler(axisScaleChanged, value); }
			remove { Events.RemoveHandler(axisScaleChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlAxisWholeRangeChanged")]
#endif
		public event EventHandler<AxisRangeChangedEventArgs> AxisWholeRangeChanged {
			add { Events.AddHandler(axisWholeRangeChanged, value); }
			remove { Events.RemoveHandler(axisWholeRangeChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlAxisVisualRangeChanged")]
#endif
		public event EventHandler<AxisRangeChangedEventArgs> AxisVisualRangeChanged {
			add { Events.AddHandler(axisVisualRangeChanged, value); }
			remove { Events.RemoveHandler(axisVisualRangeChanged, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlEndLoading")]
#endif
		public event EventHandler EndLoading {
			add { Events.AddHandler(endLoading, value); }
			remove { Events.RemoveHandler(endLoading, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlQueryCursor")]
#endif
		public event QueryCursorEventHandler QueryCursor {
			add { Events.AddHandler(queryCursor, value); }
			remove { Events.RemoveHandler(queryCursor, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomizeAutoBindingSettings")]
#endif
		public event CustomizeAutoBindingSettingsEventHandler CustomizeAutoBindingSettings {
			add { Events.AddHandler(customizeAutoBindingSettings, value); }
			remove { Events.RemoveHandler(customizeAutoBindingSettings, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlPivotChartingCustomizeXAxisLabels")]
#endif
		public event CustomizeXAxisLabelsEventHandler PivotChartingCustomizeXAxisLabels {
			add { Events.AddHandler(pivotChartingCustomizeXAxisLabels, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeXAxisLabels, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlPivotChartingCustomizeResolveOverlappingMode")]
#endif
		public event CustomizeResolveOverlappingModeEventHandler PivotChartingCustomizeResolveOverlappingMode {
			add { Events.AddHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlCustomizeSimpleDiagramLayout")]
#endif
		public event CustomizeSimpleDiagramLayoutEventHandler CustomizeSimpleDiagramLayout {
			add { Events.AddHandler(customizeSimpleDiagramLayout, value); }
			remove { Events.RemoveHandler(customizeSimpleDiagramLayout, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlPivotGridSeriesExcluded")]
#endif
		public event PivotGridSeriesExcludedEventHandler PivotGridSeriesExcluded {
			add { Events.AddHandler(pivotGridSeriesExcluded, value); }
			remove { Events.RemoveHandler(pivotGridSeriesExcluded, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlPivotGridSeriesPointsExcluded")]
#endif
		public event PivotGridSeriesPointsExcludedEventHandler PivotGridSeriesPointsExcluded {
			add { Events.AddHandler(pivotGridSeriesPointsExcluded, value); }
			remove { Events.RemoveHandler(pivotGridSeriesPointsExcluded, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlPivotChartingCustomizeLegend")]
#endif
		public event CustomizeLegendEventHandler PivotChartingCustomizeLegend {
			add { Events.AddHandler(pivotChartingCustomizeLegend, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeLegend, value); }
		}
#if !SL
	[DevExpressXtraChartsUILocalizedDescription("ChartControlLegendItemChecked")]
#endif
		public event LegendItemCheckedEventHandler LegendItemChecked {
			add { Events.AddHandler(legendItemChecked, value); }
			remove { Events.RemoveHandler(legendItemChecked, value); }
		}
		public ChartControl() {
			chart = new Chart(this);
			this.rangeControlClient = new RangeControlClient(chart);
			lookAndFeel = new ControlUserLookAndFeel(this);
			lookAndFeel.StyleChanged += new EventHandler(OnStyleChanged);
			UpdateSkin(lookAndFeel);
			chart.Printer = new ChartPrinter(this);
			navigationController = new ChartNavigationController(this);
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
			Width = 300;
			Height = 200;
			gestureHelper = new GestureHelper(this);
			commandFactory = new ChartCommandFactory(this);
			ToolTipController.DefaultController.AddClientControl(this);
		}
		#region ISupportBarsInteraction implementation
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { updateUI += value; } remove { updateUI -= value; } }
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return commandFactory.CreateCommand(id);
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		void ISupportBarsInteraction.RaiseUIUpdated() {
			RaiseUIUpdated();
		}
		object IServiceProvider.GetService(Type serviceType) {
			IServiceProvider provider = ((IChartContainer)this).ServiceProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
		#endregion
		#region IChartDataProvider implementation
		object IChartDataProvider.ParentDataSource { get { return null; } }
		DataContext IChartDataProvider.DataContext { get { return null; } }
		bool IChartDataProvider.CanUseBoundPoints { get { return true; } }
		bool IChartDataProvider.SeriesDataSourceVisible { get { return true; } }
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
		IPrintable IChartRenderProvider.Printable { get { return this; } }
		object IChartRenderProvider.LookAndFeel { get { return lookAndFeel; } }
		void IChartRenderProvider.Invalidate() {
			InvalidateAndUpdate();
		}
		void IChartRenderProvider.InvokeInvalidate() {
			base.Invalidate();
		}
		Bitmap IChartRenderProvider.LoadBitmap(string url) {
			return null;
		}
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return new ComponentPrinter(iPrintable);
		}
		#endregion
		#region IChartEventsProvider implementation
		bool IChartEventsProvider.ShouldCustomDrawAxisLabels { get { return Events[customDrawAxisLabel] != null || IsChartControlChild; } }
		bool IChartEventsProvider.ShouldCustomDrawSeriesPoints { get { return Events[customDrawSeriesPoint] != null || IsChartControlChild; } }
		bool IChartEventsProvider.ShouldCustomDrawSeries { get { return Events[customDrawSeries] != null || IsChartControlChild; } }
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
		void IChartEventsProvider.OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) {
			OnAxisWholeRangeChanged(e);
		}
		void IChartEventsProvider.OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) {
			OnAxisVisualRangeChanged(e);
		}
		#endregion
		#region IChartInteractionProvider implementation
		bool IChartInteractionProvider.HitTestingEnabled {
			get {
				return (runtimeHitTesting && !DesignMode) || ((IChartInteractionProvider)this).SelectionMode != ElementSelectionMode.None ||
					Events[objectSelected] != null || Events[objectHotTracked] != null;
			}
		}
		ElementSelectionMode IChartInteractionProvider.SelectionMode { get { return DesignMode ? ElementSelectionMode.Single : SelectionMode; } }
		SeriesSelectionMode IChartInteractionProvider.SeriesSelectionMode { get { return DesignMode ? SeriesSelectionMode.Series : SeriesSelectionMode; } }
		bool IChartInteractionProvider.EnableChartHitTesting { get { return true; } }
		bool IChartInteractionProvider.CanShowTooltips { get { return DesignMode; } }
		bool IChartInteractionProvider.DragCtrlKeyRequired { get { return DesignMode; } }
		Point IChartInteractionProvider.PointToCanvas(Point p) {
			return PointToScreen(p);
		}
		void IChartInteractionProvider.OnObjectHotTracked(HotTrackEventArgs e) {
			OnObjectHotTracked(e);
		}
		void IChartInteractionProvider.OnObjectSelected(HotTrackEventArgs e) {
			OnObjectSelected(e);
		}
		void IChartInteractionProvider.OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			OnSelectedItemsChanged(e);
		}
		void IChartInteractionProvider.OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			OnCustomDrawCrosshair(e);
		}
		void IChartInteractionProvider.OnScroll(ChartScrollEventArgs e) {
			OnScroll(e);
		}
		void IChartInteractionProvider.OnScroll3D(ChartScroll3DEventArgs e) {
			OnScroll3D(e);
		}
		void IChartInteractionProvider.OnZoom(ChartZoomEventArgs e) {
			OnZoom(e);
		}
		void IChartInteractionProvider.OnZoom3D(ChartZoom3DEventArgs e) {
			OnZoom3D(e);
		}
		void IChartInteractionProvider.OnQueryCursor(QueryCursorEventArgs e) {
			OnQueryCursor(e);
		}
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
		bool IChartContainer.ShowDesignerHints { get { return DevExpress.Utils.Design.DesignerHintsHelper.ShowDesignerHints; } }
		Chart IChartContainer.Chart { get { return chart; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return Site; } }
		IComponent IChartContainer.Parent { get { return base.Parent; } }
		bool IChartContainer.DesignMode { get { return DesignMode; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		ChartContainerType IChartContainer.ControlType { get { return ChartContainerType.WinControl; } }
		bool IChartContainer.Loading { get { return loading; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return DesignMode; } }
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
			}
			InitializeDrawing();
			base.Invalidate();
		}
		void IChartContainer.ShowErrorMessage(string message, string title) {
			string actualTitle = string.IsNullOrEmpty(title) ? Name : title;
			XtraMessageBox.Show(lookAndFeel, message, actualTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		void IChartContainer.Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) {
			rangeControlClient.RaiseRangeControlRangeChanged(minValue, maxValue, invalidate);
		}
		bool IChartContainer.GetActualRightToLeft() {
			return WindowsFormsSettings.GetIsRightToLeft(this);
		}
		#endregion
		#region IBasePrintable implementation
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			((IBasePrintable)chart).Initialize(ps, link);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			((IBasePrintable)chart).Finalize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			((IBasePrintable)chart).CreateArea(areaName, graph);
		}
		#endregion
		#region IPrintable implementation
		bool IPrintable.CreatesIntersectedBricks { get { return true; } }
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl {
			get {
				UserControl ctrl = new UserControl();
				if (printingDesigner == null)
					printingDesigner = new ChartPrintingDesigner();
				printingDesigner.Initialize(OptionsPrint);
				printingDesigner.Dock = DockStyle.Fill;
				ctrl.Size = printingDesigner.Size;
				ctrl.Controls.Add(printingDesigner);
				ctrl.HandleDestroyed += delegate(object sender, EventArgs e) { ctrl.Controls.Remove(printingDesigner); };
				return ctrl;
			}
		}
		bool IPrintable.HasPropertyEditor() {
			return true;
		}
		bool IPrintable.SupportsHelp() {
			return false;
		}
		void IPrintable.ShowHelp() {
		}
		void IPrintable.AcceptChanges() {
			if (printingDesigner != null)
				printingDesigner.ApplyOptions();
		}
		void IPrintable.RejectChanges() {
		}
		#endregion
		#region ISupportInitialize implementation
		public void BeginInit() {
			loading = true;
			runtimeRotation = false;
		}
		public void EndInit() {
			loading = false;
			OnEndLoading(EventArgs.Empty);
			if (runtimeRotation) {
				Diagram3D diagram = chart.Diagram as Diagram3D;
				if (diagram != null && !diagram.RuntimeRotation)
					diagram.RuntimeRotation = true;
			}
		}
		#endregion
		#region ICloneable implementation
		public object Clone() {
			ChartControl control = new ChartControl();
			control.AssignInternal(this);
			return control;
		}
		#endregion
		#region IGestureClient implementation
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			return navigationController.CheckAllowGestures(point);
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			overPan = navigationController.OnGesturePan(info.Current.Point, delta, info.IsBegin, info.IsEnd);
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
			navigationController.OnGestureRotation(degreeDelta);
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			navigationController.OnGestureZoom(info.Current.Point, zoomDelta, info.IsBegin);
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		#endregion
		#region IToolTipControlClient implementation
		bool IToolTipControlClient.ShowToolTips { get { return chart.ActualSeriesToolTipEnabled || chart.ActualPointToolTipEnabled; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			ChartFocusedArea focusedArea = chart.HitTestController.FindFocusedArea(point);
			object hitTestElement = focusedArea != null ? focusedArea.Element : null;
			if (hitTestElement == null)
				return null;
			ChartToolTipControlInfo info = CalculateToolTipInfo(point, hitTestElement);
			if (info == null)
				return null;
			ToolTipPositionWithOffset offsetPosition = chart.ToolTipOptions.ToolTipPosition as ToolTipPositionWithOffset;
			if (offsetPosition != null) {
				Point? anchorPoint = ToolTipPositionUtils.CalculateInitialToolTipPosition(offsetPosition, focusedArea);
				if (!anchorPoint.HasValue)
					return null;
				ToolTipDockCorner? dockCorner = ToolTipPositionUtils.GetToolTipDockCorner(offsetPosition);
				info.ToolTipLocation = ConvertToolTipDockCorner(dockCorner);
				info.UseCursorOffset = false;
				info.ToolTipPosition = ToolTipPositionUtils.ToolTipOffsetPosition(offsetPosition, anchorPoint.Value);
			}
			return info;
		}
		#endregion
		#region ShouldSerialize
		bool ShouldSerializePadding() {
			return chart.ShouldSerializePadding();
		}
		bool ShouldSerializeDiagram() {
			return chart.ShouldSerializeDiagram();
		}
		bool ShouldSerializeBackColor() {
			return chart.ShouldSerializeBackColor();
		}
		bool ShouldSerializeSeriesDataMember() {
			return DataContainer.ShouldSerializeSeriesDataMember();
		}
		bool ShouldSerializeSeriesSorting() {
			return DataContainer.ShouldSerializeBoundSeriesSorting();
		}
		bool ShouldSerializeDataSource() {
			return chart.ShouldSerializeDataSource();
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
		bool ShouldSerializeCacheToMemory() {
			return chart.ShouldSerializeCacheToMemory();
		}
		bool ShouldSerializeCrosshairEnabled() {
			return chart.ShouldSerializeCrosshairEnabled();
		}
		bool ShouldSerializeRefreshDataOnRepaint() {
			return DataContainer.ShouldSerializeRefreshDataOnRepaint();
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
			return chart.ShouldSerializeToolTipOptions();
		}
		bool ShouldSerializeAllowGesture() {
			return !allowGesture;
		}
		bool ShouldSerializeAutoLayout() {
			return chart.ShouldSerializeAutoLayout();
		}
		#endregion
		#region IRangeControlClient implementation
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add {
				RangeControlClient.RangeChanged += value;	
			}
			remove {
				RangeControlClient.RangeChanged -= value;
			}
		}
		bool IRangeControlClient.IsValidType(Type type) {
			return RangeControlClient.IsValidType(type);
		}
		bool IRangeControlClient.IsValid {
			get {
				return RangeControlClient.IsValid;
			}
		}
		string IRangeControlClient.InvalidText {
			get {
				return RangeControlClient.InvalidText;
			}
		}
		int IRangeControlClient.RangeBoxTopIndent {
			get {
				return RangeControlClient.RangeBoxTopIndent;
			}
		}
		int IRangeControlClient.RangeBoxBottomIndent {
			get {
				return RangeControlClient.RangeBoxBottomIndent;
			}
		}
		bool IRangeControlClient.IsCustomRuler {
			get {
				return RangeControlClient.IsCustomRuler;
			}
		}
		object IRangeControlClient.RulerDelta {
			get {
				return RangeControlClient.RulerDelta;
			}
		}
		double IRangeControlClient.NormalizedRulerDelta {
			get {
				return RangeControlClient.NormalizedRulerDelta;
			}
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) {
			return RangeControlClient.SupportOrientation(orientation);
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return RangeControlClient.GetValue(normalizedValue);
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return RangeControlClient.GetNormalizedValue(value);
		}
		string IRangeControlClient.RulerToString(int ruleIndex) {
			return RangeControlClient.RulerToString(ruleIndex);
		}
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) {
			return RangeControlClient.GetRuler(e);
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			RangeControlClient.ValidateRange(info);
		}
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			RangeControlClient.OnRangeChanged(rangeMinimum, rangeMaximum);
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			RangeControlClient.DrawContent(e);
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return RangeControlClient.DrawRuler(e);
		}
		object IRangeControlClient.GetOptions() {
			return RangeControlClient.GetOptions();
		}
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) {
			RangeControlClient.UpdateHotInfo(hitInfo);
		}
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			RangeControlClient.UpdatePressedInfo(hitInfo);
		}
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) {
			RangeControlClient.OnClick(hitInfo);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			RangeControlClient.OnRangeControlChanged(rangeControl);
		}
		void IRangeControlClient.OnResize() {
			RangeControlClient.OnResize();
		}
		void IRangeControlClient.Calculate(Rectangle contentRect) {
			RangeControlClient.Calculate(contentRect);
		}
		double IRangeControlClient.ValidateScale(double newScale) {
			return RangeControlClient.ValidateScale(newScale);
		}
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return RangeControlClient.ValueToString(normalizedValue);
		}
		#endregion
		#region IRangeControlClientExtension
		object IRangeControlClientExtension.NativeValue(double normalizedValue) {
			return RangeControlClient.NativeValue(normalizedValue);
		}
		#endregion      
		ChartToolTipControlInfo CalculateToolTipInfo(Point point, object hitTestElement) {
			ISeriesPoint hitPoint = hitTestElement as ISeriesPoint;
			SeriesPoint seriesPoint = chart.GetSeriesPoint(hitPoint); ;
			if (seriesPoint != null && hitPoint != null) {
				Series ownerSeries = ((IOwnedElement)seriesPoint).Owner as Series;
				if (ownerSeries == null || Diagram == null)
					return null;
				string pattern = ((IPatternHolder)ownerSeries.View).PointPattern;
				PatternParser patternParser = new PatternParser(pattern, ownerSeries.View);
				patternParser.SetContext(ownerSeries);
				patternParser.SetContext(ViewController.FindRefinedPoint(chart, hitPoint, ownerSeries));
				string toolTipPointText = patternParser.GetText();
				return new ChartToolTipControlInfo(seriesPoint, toolTipPointText);
			}
			Series series = hitTestElement as Series;
			if (series != null) {
				PatternParser patternParser = new PatternParser(series.ToolTipSeriesPattern, series.View);
				patternParser.SetContext(series);
				string toolTipText = patternParser.GetText();
				ChartToolTipControlInfo info = new ChartToolTipControlInfo(series, toolTipText);
				info.ToolTipImage = series.ToolTipImage.Image;
				return info;
			}
			return null;
		}
		void UpdateSkin(ISkinProvider skinProvider) {
			if (chart.Appearance.IsDefault) {
				chart.ResetGraphicsCache();
				chart.InvalidateDrawingHelper();
				ClearCache();
				ChartSkinUtils.LoadFromSkin(chart.Appearance, skinProvider);
			}
			if (chart.Palette == Palettes.Default) {
				string actualPaletteName = GetActualPaletteNameFromSkin(skinProvider);
				actualPaletteName = actualPaletteName == null ? "Office" : actualPaletteName;
				chart.LoadColorsForDefaultPalette(actualPaletteName);
			}
		}
		string GetActualPaletteNameFromSkin(ISkinProvider skinProvider) {
			Skin skin = ChartSkins.GetSkin(skinProvider);
			object paletteName = skin != null ? skin.Properties[ChartSkins.PropertyPaletteName] : null;
			return paletteName != null ? paletteName.ToString() : Palettes.Office.Name;
		}
		void OnImageReady() {
			Invoke(new DrawCompleteDelegate(InvalidateAndUpdate), new object[] { });
		}
		void InvalidateAndUpdate() {
			base.Invalidate();
			if (Mode3D)
				Update();
		}
		[System.Security.SecuritySafeCritical]
		void ReleaseOpenGLGraphics() {
			if (chart != null)
				chart.DisposeDrawingHelper();
			if (openGLGraphics != null) {
				openGLGraphics.Dispose();
				openGLGraphics = null;
			}
			if (windowGraphics != null) {
				windowGraphics.Dispose();
				windowGraphics = null;
			}
			if (windowDC != IntPtr.Zero) {
				ReleaseDC(Handle, windowDC);
				windowDC = IntPtr.Zero;
			}
		}
		[System.Security.SecuritySafeCritical]
		void InitializeDrawing() {
			bool hardware3D = Mode3D && hardwareAcceleration;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, !hardware3D);
			if (hardware3D) {
				if (openGLGraphics == null && chart != null && IsHandleCreated) {
					if (windowDC == IntPtr.Zero)
						windowDC = GetDC(Handle);
					if (windowGraphics == null)
						windowGraphics = Graphics.FromHdc(windowDC);
					openGLGraphics = chart.CreateNativeGraphics(windowGraphics, windowDC, Size, GraphicsQuality.Lowest) as OpenGLGraphics;
				}
			}
			else
				ReleaseOpenGLGraphics();
		}
		void AssignInternal(ChartControl control) {
			chart.Assign(control.chart);
			runtimeHitTesting = control.runtimeHitTesting;
			runtimeRotation = control.runtimeRotation;
			hardwareAcceleration = control.hardwareAcceleration;
			SelectionMode = control.SelectionMode;
			SeriesSelectionMode = control.SeriesSelectionMode;
			toolTipController = control.toolTipController;
			Assign(control);
		}
		void RaiseUIUpdated() {
			if (updateUI != null)
				updateUI(this, new EventArgs());
		}
		void ShowPrintPreviewInternal(PrintSizeMode sizeMode) {
			if (IsPrintingAvailable)
				Chart.Printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)Chart.Printer.ComponentPrinter).ShowPreview(LookAndFeel); }, sizeMode);
		}
		void PrintInternal(PrintSizeMode sizeMode) {
			if (Chart.Printer != null)
				Chart.Printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)Chart.Printer.ComponentPrinter).Print(); }, sizeMode);
		}
		HtmlExportOptions CreateHtmlExportOptions(string htmlCharSet, string title, bool compressed) {
			HtmlExportOptions options = new HtmlExportOptions();
			options.CharacterSet = htmlCharSet;
			options.Title = title;
			options.RemoveSecondarySymbols = compressed;
			return options;
		}
		MhtExportOptions CreateMhtExportOptions(string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions();
			options.CharacterSet = htmlCharSet;
			options.Title = title;
			options.RemoveSecondarySymbols = compressed;
			return options;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			UpdateSkin(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin ? lookAndFeel : null);
			RaiseUIUpdated();
			Invalidate();
		}
		protected void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				isDisposed = true;
				ReleaseOpenGLGraphics();
				lookAndFeel.StyleChanged -= new EventHandler(OnStyleChanged);
				chart.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			InitializeDrawing();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			ReleaseOpenGLGraphics();
			base.OnHandleDestroyed(e);
		}
		protected override void OnSizeChanged(EventArgs e) {
			if (openGLGraphics != null)
				openGLGraphics.Size = Size;
			base.OnSizeChanged(e);
			if (!isDisposed) {
				chart.ClearCache();
				chart.ResetGraphicsCache();
				chart.InvalidateDrawingHelper();
				((IChartRenderProvider)this).Invalidate();
			}
		}
		protected override void OnMove(EventArgs e) {
			base.OnMove(e);
			if (!isDisposed)
				base.Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if (!isDisposed) {
				Rectangle bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
				if (Mode3D && openGLGraphics != null)
					chart.DrawContent(e.Graphics, openGLGraphics, bounds, false, true);
				else {
					chart.DrawContent(e.Graphics, bounds);
					if (navigationController != null)
						navigationController.DrawZoomRectangle(e.Graphics);
				}
			}
			base.OnPaint(e);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if (!Mode3D)
				base.OnPaintBackground(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			navigationController.OnKeyDown(e);
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			navigationController.OnKeyUp(e);
			base.OnKeyUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			navigationController.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			navigationController.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if (TabStop)
				Focus();
			navigationController.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			navigationController.OnMouseUp(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			navigationController.OnMouseWheel(e);
		}
		protected override void OnCursorChanged(EventArgs e) {
			base.OnCursorChanged(e);
			navigationController.OnCursorChanged(Cursor);
		}
		protected override void WndProc(ref Message m) {
			if (!allowGesture || !gestureHelper.WndProc(ref m))
				base.WndProc(ref m);
		}
		protected virtual void OnEndLoading(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[endLoading];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnObjectSelected(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)this.Events[objectSelected];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			SelectedItemsChangedEventHandler handler = (SelectedItemsChangedEventHandler)this.Events[selectedItemsChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnObjectHotTracked(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)this.Events[objectHotTracked];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			CustomDrawSeriesEventHandler handler = (CustomDrawSeriesEventHandler)this.Events[customDrawSeries];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			CustomDrawCrosshairEventHandler handler = (CustomDrawCrosshairEventHandler)this.Events[customDrawCrosshair];
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
		protected virtual void OnScroll(ChartScrollEventArgs e) {
			ChartScrollEventHandler handler = (ChartScrollEventHandler)this.Events[scroll];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnScroll3D(ChartScroll3DEventArgs e) {
			ChartScroll3DEventHandler handler = (ChartScroll3DEventHandler)this.Events[scroll3D];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnZoom(ChartZoomEventArgs e) {
			ChartZoomEventHandler handler = (ChartZoomEventHandler)this.Events[zoom];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnZoom3D(ChartZoom3DEventArgs e) {
			ChartZoom3DEventHandler handler = (ChartZoom3DEventHandler)Events[zoom3D];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnBoundDataChanged(EventArgs e) {
			BoundDataChangedEventHandler handler = (BoundDataChangedEventHandler)this.Events[boundDataChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
			PieSeriesPointExplodedEventHandler handler = (PieSeriesPointExplodedEventHandler)this.Events[pieSeriesPointExploded];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisScaleChanged(AxisScaleChangedEventArgs e) {
			EventHandler<AxisScaleChangedEventArgs> handler = (EventHandler<AxisScaleChangedEventArgs>)this.Events[axisScaleChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e) {
			EventHandler<AxisRangeChangedEventArgs> handler = (EventHandler<AxisRangeChangedEventArgs>)this.Events[axisWholeRangeChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e) {
			EventHandler<AxisRangeChangedEventArgs> handler = (EventHandler<AxisRangeChangedEventArgs>)this.Events[axisVisualRangeChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnQueryCursor(QueryCursorEventArgs e) {
			QueryCursorEventHandler handler = (QueryCursorEventHandler)Events[queryCursor];
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
			var handler = (LegendItemCheckedEventHandler)Events[legendItemChecked];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void Assign(ChartControl control) { }
		internal void ShowRibbonPrintPreview(PrintSizeMode sizeMode) {
			if (IsPrintingAvailable)
				Chart.Printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)Chart.Printer.ComponentPrinter).ShowRibbonPreview(LookAndFeel); }, sizeMode);
		}
		public void ClearSelection() {
			ClearSelection(true);
		}
		public void ClearSelection(bool clearHot) {
			chart.ClearSelection(clearHot);
		}
		public void SetObjectSelection(object obj) {
			chart.SetObjectSelection(obj, true);
		}
		public object[] HitTest(int x, int y) {
			return chart.HitTest(x, y).ToArray();
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
		public ToolTipController GetToolTipController() {
			if (ToolTipController == null)
				return ToolTipController.DefaultController;
			return ToolTipController;
		}
		public ChartHitInfo CalcHitInfo(Point point) {
			return chart.CalcHitInfo(point);
		}
		public ChartHitInfo CalcHitInfo(int x, int y) {
			return chart.CalcHitInfo(x, y);
		}
		public void RefreshData() {
			chart.RefreshData(true);
		}
		public void ClearCache() {
			chart.ClearCache();
		}		
		public void ResetLegendTextPattern() {
			DataContainer.ResetLegendTextPattern();
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
		public void SaveToStream(Stream stream) {
			chart.SaveLayout(stream);
		}
		public void SaveToFile(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
				SaveToStream(fs);
		}
		public void LoadFromStream(Stream stream) {
			stream.Seek(0L, SeekOrigin.Begin);
			if (!XtraSerializingHelper.IsValidXml(stream))
				throw new LayoutStreamException();
			stream.Seek(0L, SeekOrigin.Begin);
			chart.LoadLayout(stream);
		}
		public void LoadFromFile(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				LoadFromStream(fs);
		}
		public void BindToData(ViewType viewType, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			DataContainer.BindToData(viewType, dataSource, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(SeriesViewBase view, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			DataContainer.BindToData(view, dataSource, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void ShowPrintPreview() {
			ShowPrintPreviewInternal(OptionsPrint.SizeMode);
		}
		public void ShowRibbonPrintPreview() {
			ShowRibbonPrintPreview(Chart.Printer.SizeMode);
		}
		public void Print() {
			PrintInternal(OptionsPrint.SizeMode);
		}
		public void ExportToHtml(string filePath) {
			chart.ExportToHtml(filePath);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			chart.ExportToHtml(filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			chart.ExportToHtml(stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			chart.ExportToHtml(stream, options);
		}
		public void ExportToMht(string filePath) {
			chart.ExportToMht(filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			chart.ExportToMht(filePath, options);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			chart.ExportToMht(stream, options);
		}
		public void ExportToPdf(string filePath) {
			chart.ExportToPdf(filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			chart.ExportToPdf(filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			chart.ExportToPdf(stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			chart.ExportToPdf(stream, options);
		}
		public void ExportToRtf(string filePath) {
			chart.ExportToRtf(filePath);
		}
		public void ExportToRtf(Stream stream) {
			chart.ExportToRtf(stream);
		}
		public void ExportToXls(string filePath) {
			chart.ExportToXls(filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			chart.ExportToXls(filePath, options);
		}
		public void ExportToXls(Stream stream) {
			chart.ExportToXls(stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			chart.ExportToXls(stream, options);
		}
		public void ExportToXlsx(string filePath) {
			chart.ExportToXlsx(filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			chart.ExportToXlsx(filePath, options);
		}
		public void ExportToXlsx(Stream stream) {
			chart.ExportToXlsx(stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			chart.ExportToXlsx(stream, options);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			chart.ExportToImage(stream, format);
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			chart.ExportToImage(filePath, format);
		}
		public PaletteEntry[] GetPaletteEntries(int count) {
			return chart.GetPaletteEntries(count);
		}
		#region Obsolete
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
		Obsolete("The CustomizeResolveOverlappingMode event is obsolete now. Use the PivotChartingCustomizeResolveOverlappingMode event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeResolveOverlappingModeEventHandler CustomizeResolveOverlappingMode {
			add { Events.AddHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeResolveOverlappingMode, value); }
		}
		[
		Obsolete("The CustomizeResolveOverlappingMode event is obsolete now. Use the PivotChartingCustomizeResolveOverlappingMode event instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public event CustomizeLegendEventHandler CustomizeLegend {
			add { Events.AddHandler(pivotChartingCustomizeLegend, value); }
			remove { Events.RemoveHandler(pivotChartingCustomizeLegend, value); }
		}
		[
		Obsolete("This method is obsolete now. Use the ResetLegendTextPattern method instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ResetLegendPointOptions() {
			DataContainer.ResetLegendPointOptions();
		}
		[
		Obsolete("This method is obsolete now. Please, use the ChartControl.OptionsPrint.SizeMode property along with the ShowPrintPreview method without any parameters instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ShowPrintPreview(PrintSizeMode sizeMode) {
			ShowPrintPreviewInternal(sizeMode);
		}
		[
		Obsolete("This method is obsolete now. Please, use the ChartControl.OptionsPrint.SizeMode property along with the Print method without any parameters instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void Print(PrintSizeMode sizeMode) {
			PrintInternal(sizeMode);
		}
		[
		Obsolete("This method is now obsolete. You should use the ExportToHtml(string, HtmlExportOptions) method instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			ExportToHtml(filePath, CreateHtmlExportOptions(htmlCharSet, title, compressed));
		}
		[
		Obsolete("This method is now obsolete. You should use the ExportToHtml(Stream, HtmlExportOptions) method instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			ExportToHtml(stream, CreateHtmlExportOptions(htmlCharSet, title, compressed));
		}
		[
		Obsolete("This method is now obsolete. You should use the ExportToMht(string, MhtExportOptions) method instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			ExportToMht(filePath, CreateMhtExportOptions(htmlCharSet, title, compressed));
		}
		[
		Obsolete("This method is now obsolete. You should use the ExportToMht(Stream, MhtExportOptions) method instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			ExportToMht(stream, CreateMhtExportOptions(htmlCharSet, title, compressed));
		}
		#endregion
	}
	internal class ResFinder {
	}
}
