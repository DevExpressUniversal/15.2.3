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
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using System.Drawing;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(Chart)),
	HasOptionsControl,
	TypeConverter(typeof(ChartTypeConverter))]
	public class DesignerChartModel : DesignerChartElementModelBase {
		readonly SeriesCollectionModel series;
		readonly ChartTitleCollectionModel titles;
		readonly AnnotationRepositoryModel annotationRepository;
		readonly DataContainerModel dataContainer;
		readonly ChangeChartTypeCommand changeChartTypeCommand;
		DesignerDiagramModel diagramModel;
		LegendModel legendModel;
		RectangleFillStyleModel fillStyleModel;
		RectangularBorderModel borderModel;
		BackgroundImageModel backImageModel;
		RectangleIndentsModel paddingModel;
		ToolTipOptionsModel toolTipOptionsModel;
		CrosshairOptionsModel crosshairOptionsModel;
		EmptyChartTextModel emptyChartTextModel;
		SmallChartTextModel smallChartTextModel;
		internal SeriesCollectionModel Series { get { return series; } }
		internal ChartTitleCollectionModel Titles { get { return titles; } }
		internal AnnotationRepositoryModel AnnotationRepository { get { return annotationRepository; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override ChartElement ChartElement { get { return Chart; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.ChartKey; } }
		internal DataContainerModel DataContainer {
			get { return dataContainer; }
		}
		[Category("Behavior")]
		public DefaultBoolean CrosshairEnabled {
			get { return Chart.CrosshairEnabled; }
			set {
				SetProperty("CrosshairEnabled", value);
			}
		}
		[Category("Appearance")]
		public DefaultBoolean RightToLeft {
			get { return Chart.RightToLeft; }
			set {
				SetProperty("RightToLeft", value);
			}
		}
		[Category("Behavior")]
		public ElementSelectionMode SelectionMode {
			get { return Chart.SelectionMode; }
			set {
				SetProperty("SelectionMode", value);
			}
		}
		[Category("Behavior")]
		public SeriesSelectionMode SeriesSelectionMode {
			get { return Chart.SeriesSelectionMode; }
			set {
				SetProperty("SeriesSelectionMode", value);
			}
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public DesignerDiagramModel Diagram { get { return diagramModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public LegendModel Legend { get { return legendModel; } }
		[PropertyForOptions("Behavior"),
		Category("Layout")]
		public bool AutoLayout {
			get { return Chart.AutoLayout; }
			set { SetProperty("AutoLayout", value); }
		}
		[Category("Behavior")]
		public bool CacheToMemory {
			get { return Chart.CacheToMemory; }
			set { SetProperty("CacheToMemory", value); }
		}
		[Category("Appearance")]
		public string AppearanceName {
			get { return Chart.AppearanceName; }
			set { SetProperty("AppearanceName", value); }
		}
		[PropertyForOptions(0, "Appearance", 0),
		Browsable(false)]
		public ChartAppearanceModel Appearance {
			get {
				return new ChartAppearanceModel() { Appearance = Chart.Appearance, CurrentPaletteIndex = Chart.PaletteBaseColorNumber };
			}
			set {
				SetProperty("Appearance", value.Appearance);
				SetProperty("PaletteBaseColorNumber", value.CurrentPaletteIndex);
			}
		}
		[Browsable(false)]
		public ChartAppearance ChartAppearance {
			get { return Chart.Appearance; }
			set { SetProperty("Appearance", value); }
		}
		[PropertyForOptions(1, "Appearance", 0),
		Browsable(false)]
		public Palette Palette {
			get { return Chart.Palette; }
			set { SetProperty("Palette", value); }
		}
		[PropertyForOptions(2, "Appearance"),
		Category("Appearance")]
		public Color BackColor {
			get { return Chart.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[Editor("DevExpress.XtraCharts.Designer.Native.PaletteNameUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraCharts.Design.PaletteTypeConverter," + AssemblyInfo.SRAssemblyCharts),
		Category("Appearance")]
		public string PaletteName {
			get { return Chart.PaletteName; }
			set { SetProperty("PaletteName", value); }
		}
		[Category("Appearance")]
		public int PaletteBaseColorNumber {
			get { return Chart.PaletteBaseColorNumber; }
			set { SetProperty("PaletteBaseColorNumber", value); }
		}
		[Category("Appearance")]
		public string IndicatorsPaletteName {
			get { return Chart.IndicatorsPaletteName; }
			set { SetProperty("IndicatorsPaletteName", value); }
		}
		[Category("Behavior")]
		public DefaultBoolean ToolTipEnabled {
			get { return Chart.ToolTipEnabled; }
			set { SetProperty("ToolTipEnabled", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions(),
		AllocateToGroup("Border"),
		Category("Appearance")
		]
		public RectangularBorderModel Border { get { return borderModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public BackgroundImageModel BackImage { get { return backImageModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleIndentsModel Padding { get { return paddingModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Misc")]
		public ChartOptionsPrint OptionsPrint { get { return Chart.OptionsPrint; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public EmptyChartTextModel EmptyChartText { get { return emptyChartTextModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public SmallChartTextModel SmallChartText { get { return smallChartTextModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public ToolTipOptionsModel ToolTipOptions { get { return toolTipOptionsModel; } }
		[Category("Behavior")]
		public CrosshairOptionsModel CrosshairOptions { get { return crosshairOptionsModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Data")
		]
		public DesignerSeriesModelBase SeriesTemplate { get { return dataContainer.SeriesTemplate; } }
		public DesignerChartModel(CommandManager commandManager, Chart chart)
			: base(commandManager, chart) {
			this.series = new SeriesCollectionModel(Chart.Series, CommandManager, chart);
			this.titles = new ChartTitleCollectionModel(Chart.Titles, CommandManager, chart);
			this.annotationRepository = new AnnotationRepositoryModel(Chart.AnnotationRepository, CommandManager, chart);
			this.changeChartTypeCommand = new ChangeChartTypeCommand(CommandManager, chart);
			this.legendModel = new LegendModel(Chart.Legend, CommandManager);
			this.fillStyleModel = new RectangleFillStyleModel(Chart.FillStyle, CommandManager);
			this.borderModel = (RectangularBorderModel)ModelHelper.CreateBorderModelInstance(Chart.Border, CommandManager);
			this.backImageModel = new BackgroundImageModel(Chart.BackImage, CommandManager);
			this.paddingModel = new RectangleIndentsModel(Chart.Padding, CommandManager);
			this.toolTipOptionsModel = new ToolTipOptionsModel(Chart.ToolTipOptions, CommandManager);
			this.crosshairOptionsModel = new CrosshairOptionsModel(Chart.CrosshairOptions, CommandManager);
			this.emptyChartTextModel = new EmptyChartTextModel(Chart.EmptyChartText, CommandManager);
			this.smallChartTextModel = new SmallChartTextModel(Chart.SmallChartText, CommandManager);
			this.dataContainer = new DataContainerModel(Chart.DataContainer, CommandManager);
			Update();
		}
		protected internal override bool IsSupportsDataControl(bool isDesignTime) {
			return isDesignTime ? true : dataContainer.DataSource != null;
		}
		protected override void ProcessMessage(ViewMessage message) {
			if (message.Name == "Appearance") {
				BatchSetProperties(new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("PaletteBaseColorNumber", ((ChartAppearanceModel)message.Value).CurrentPaletteIndex),
					new KeyValuePair<string, object>("Appearance", ((ChartAppearanceModel)message.Value).Appearance)
				});
			}
			else if (message.Name == "Palette") {
				Palette palette = message.Value as Palette;
				ChartAppearance newAppearance = null;
				foreach (ChartAppearance appearance in Chart.AppearanceRepository)
					if (appearance.PaletteName == palette.Name) {
						newAppearance = appearance;
						break;
					}
				if (newAppearance == null)
					base.ProcessMessage(message);
				else {
					BatchSetProperties(new List<KeyValuePair<string, object>>() {
						new KeyValuePair<string, object>("Palette", palette),
						new KeyValuePair<string, object>("Appearance", newAppearance)
					});
				}
			}
			else
				base.ProcessMessage(message);
		}
		protected override void AddChildren() {
			if (diagramModel != null)
				Children.Add(diagramModel);
			if (legendModel != null)
				Children.Add(legendModel);
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (borderModel != null)
				Children.Add(borderModel);
			if (backImageModel != null)
				Children.Add(backImageModel);
			if (paddingModel != null)
				Children.Add(paddingModel);
			if (toolTipOptionsModel != null)
				Children.Add(toolTipOptionsModel);
			if (crosshairOptionsModel != null)
				Children.Add(crosshairOptionsModel);
			if (emptyChartTextModel != null)
				Children.Add(emptyChartTextModel);
			if (smallChartTextModel != null)
				Children.Add(smallChartTextModel);
			if (dataContainer != null)
				Children.Add(dataContainer);
			Children.Add(series);
			Children.Add(titles);
			Children.Add(annotationRepository);
			base.AddChildren();
		}
		public override void Update() {
			if (diagramModel == null || diagramModel.Diagram != Chart.Diagram) 
				diagramModel = ModelHelper.CreateDiagramModelInstance(Chart.Diagram, CommandManager, Chart);
			if (diagramModel != null)
				diagramModel.Parent = this;
			ClearChildren();
			AddChildren();
			base.Update();
			if (Chart.DataContainer.AutocreatedSeries.Count > 0)
				series.Children.Add(SeriesTemplate);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this);
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			return SeriesTemplate != null ? SeriesTemplate.GetDataMembersInfo() : null;
		}
		public void ChangeChartType(ViewType viewType) {
			changeChartTypeCommand.Execute(viewType);
		}
	}
}
