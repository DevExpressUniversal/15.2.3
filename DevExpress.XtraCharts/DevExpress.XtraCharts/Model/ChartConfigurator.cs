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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Charts;
using DevExpress.XtraCharts.Native;
using Model = DevExpress.Charts.Model;
using System.Drawing;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public class ChartConfigurator {
		ChartAppeanranceConfigurator appearanceConfigurator;
		Chart chart;
		Model.IModelElementContainer container;
		Model.Chart model;
		public Chart Chart { get { return chart; } }
		public Model.Chart Model { get { return model; } }
		public ChartConfigurator(Chart chart) {
			Guard.ArgumentNotNull(chart, "chart");
			this.chart = chart;
		}
		void ConfigureChart(Native.Chart chart, Model.Chart model) {
			ConfigureTitles(chart.Titles, model.Titles);
			if (model.Palette != null)
				ConfigureChartPalette(chart, model.Palette);
			DiagramConfigurator configuratior = DiagramConfigurator.CreateInstance(model);
			configuratior.Configure(Chart.Diagram, model);
			Model.CartesianChart cartesianChart = model as Model.CartesianChart;
			if (cartesianChart != null)
				ConfigureCartesianChart(chart, cartesianChart);
			configuratior.ConfigureAxes(Chart.Diagram, model, container, appearanceConfigurator.DiagramCofigurator);
		}
		void ConfigureTitles(ChartTitleCollection titles, Charts.Model.ChartTitleCollection model) {
			titles.Clear();
			foreach(Model.ChartTitle item in model) {
				ChartTitle title = new ChartTitle();
				title.Lines = item.Lines;
				title.EnableAntialiasing = item.EnableAntialiasing;
				titles.Add(title);
			}
		}
		void ConfigureCartesianChart(Native.Chart chart, Model.CartesianChart cartesianChart) {
			chart.SideBySideBarDistance = cartesianChart.BarDistance;
			chart.SideBySideBarDistanceFixed = cartesianChart.BarDistanceFixed;
		}
		void ConfigureDataSource(object dataSource) {
			Chart.DataContainer.DataSource = dataSource;
		}
		ChartBaseSeriesCollectionConfigurator CreateSeriesCollectionConfigurator(Model.Chart model) {
			if(model is Model.IChart3D)
				return new Chart3DSeriesCollectionConfigurator();
			return new ChartSeriesCollectionConfigurator();
		}
		void ConfigurateLegend(Model.Legend legend) {
			LegendConfigurator configurator = new LegendConfigurator();
			configurator.Configure(chart.Legend, legend);
			if (legend != null)
				container.Register(chart.Legend, legend);
		}
		void ConfigureChartPalette(Native.Chart chart, Model.Palette palette) {
			Palette chartPalette = new Palette("palette", PaletteScaleMode.Extrapolate);
			foreach (Model.PaletteEntry entry in palette.Entries) {
				PaletteEntry paletteEntry = new PaletteEntry(ModelConfigaratorHelper.ToColor(entry.Color));
				if(!entry.Color2.IsEmpty)
					paletteEntry.Color2 = ModelConfigaratorHelper.ToColor(entry.Color2);
				chartPalette.Add(paletteEntry);
			}
			chart.PaletteRepository.RegisterPalette(chartPalette);
			chart.PaletteName = "palette";
		}
		void ConfigureChartAppearance(Native.Chart chart, Charts.Model.ChartAppearanceOptions appearance) {
			appearanceConfigurator.Configure(chart, appearance);
		}
		void UpdateChart(Model.UpdateInfo updateInfo) {
			switch (updateInfo.PropertyName) {
				case "Palette":
					ConfigureChartPalette(chart, (Model.Palette)updateInfo.Value);
					updateInfo.Handled = true;
					break;
				case "Appearance":
					ConfigureChartAppearance(chart, (Model.ChartAppearanceOptions)updateInfo.Value);
					updateInfo.Handled = true;
					break;
				default:
					return;
			}
		}
		public void Configure(Model.Chart model, Model.IModelElementContainer container) {
			this.model = model;
			this.container = container;
			if(model == null) {
				return;
			}
			container.Clear();
			container.Register(chart, model);
			ChartBaseSeriesCollectionConfigurator seriesConfigurator = CreateSeriesCollectionConfigurator(model);
			seriesConfigurator.CreateSeries(Chart, model, container);
			appearanceConfigurator = new ChartAppeanranceConfigurator(chart.Diagram, container);
			seriesConfigurator.Configurate(Chart, model, container);
			ConfigureDataSource(model.DataSource);
			ConfigurateLegend(model.Legend);
			ConfigureChart(chart, model);
			seriesConfigurator.ConfigurateSecondaryAxes(Chart, model, container);
			ConfigureChartAppearance(chart, model.Appearance);
		}
		public void Update(Model.UpdateInfo updateInfo) {
			if (updateInfo.Element is Model.Chart)
				UpdateChart(updateInfo);
		}
	}
	public class ChartConfiguratorException : Exception {
		public ChartConfiguratorException(string message)
			: base(message) {
		}
	}
}
