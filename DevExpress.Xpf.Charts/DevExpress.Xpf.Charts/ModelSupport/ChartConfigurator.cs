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
using System.Windows;
using System.Windows.Media;
using Model = DevExpress.Charts.Model;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public abstract class ConfiguratorBase {
		readonly Model.ModelElement model;
		readonly Model.IModelElementContainer container;
		protected Model.ModelElement ModelElement { get { return model; } }
		protected Model.IModelElementContainer Container { get { return container; } }
		protected static Exception MakeUnknownTypeException(object value) {
			if (value == null)
				return new Exception("Unexpected null model value");
			return new Exception("Unknown model type: " + value.GetType().Name);
		}
		public ConfiguratorBase(Model.ModelElement model, Model.IModelElementContainer container) {
			this.model = model;
			this.container = container;
		}
	}
	public abstract class ChartConfigurator : ConfiguratorBase {
		public static ChartConfigurator CreateChartConfigurator(Model.Chart chartModel, Model.IModelElementContainer container) {
			if (chartModel != null) {
				Type chartModelType = chartModel.GetType();
				if (chartModelType == typeof(Model.CartesianChart))
					return new CartesianChartConfigurator(chartModel, container);
				if (chartModelType == typeof(Model.PieChart))
					return new PieChartConfigurator(chartModel, container);
				if (chartModelType == typeof(Model.RadarChart))
					return new RadarChartConfigurator(chartModel, container);
				if (chartModelType == typeof(Model.PolarChart))
					return new PolarChartConfigurator(chartModel, container);
				if (chartModelType == typeof(Model.Cartesian3DChart))
					return new Cartesian3DChartConfigurator(chartModel, container);
				if (chartModelType == typeof(Model.Pie3DChart))
					return new Pie3DChartConfigurator(chartModel, container);
				throw MakeUnknownTypeException(chartModel);
			}
			else
				return null;
		}
		readonly SeriesConfigurator seriesConfigurator;
		protected Model.Chart ChartModel { get { return ModelElement as Model.Chart; } }
		protected virtual bool Is3D { get { return false; } }
		public ChartConfigurator(Model.Chart chartModel, Model.IModelElementContainer container) : base(chartModel, container) {
			seriesConfigurator = new SeriesConfigurator();
		}
		void ConfigureLegend(Legend chartLegend) {
			LegendConfigurator configurator = new LegendConfigurator(ChartModel.Legend, Container);
			configurator.Configure(chartLegend);
		}
		void ConfigureChartTitles(TitleCollection titles) {
			titles.Clear();
			foreach (Model.ChartTitle titleModel in ChartModel.Titles) {
				Title title = new Title();
				Container.Register(title, titleModel);
				string titleContent = string.Empty;
				if(titleModel.Lines != null)
					foreach (string line in titleModel.Lines) {
						titleContent += line;
						if (line != titleModel.Lines[titleModel.Lines.Length - 1])
							titleContent += "\n";
					}
				title.Content = titleContent;
				title.HorizontalAlignment = HorizontalAlignment.Center;
				titles.Add(title);
			}
		}
		void ConfigureSeries(Diagram diagram) {
			diagram.Series.Clear();
			foreach (Model.SeriesModel seriesModel in ChartModel.Series) {
				Series series = seriesConfigurator.CreateSeries(seriesModel, Is3D);
				if (series != null) {
					Container.Register(series, seriesModel);
					diagram.Series.Add(series);
					seriesConfigurator.Configure(series, seriesModel, diagram);
				}
			}
		}
		void ConfigureChartPalette(ChartControl chart) {
			CustomPalette palette = new CustomPalette();
			foreach (Model.PaletteEntry entry in ChartModel.Palette.Entries)
				palette.Colors.Add(Color.FromArgb(entry.Color.A, entry.Color.R, entry.Color.G, entry.Color.B));
			chart.Palette = palette;
		}
		void ConfigureChartAppearance(ChartControl chart, Model.ChartAppearanceOptions appearance) {
			ChartAppeanranceConfigurator configurator = new ChartAppeanranceConfigurator(Container);
			configurator.Configure(chart, appearance);
		}
		void UpdateChart(ChartControl chart, Model.UpdateInfo updateInfo) {
			switch (updateInfo.PropertyName) {
				case "Palette":
					ConfigureChartPalette(chart);
					updateInfo.Handled = true;
					break;
				default:
					return;
			}
		}
		protected abstract void ConfigureDiagram(ChartControl chart);
		public void Configure(ChartControl chart) {
			Container.Clear();
			Container.Register(chart, ChartModel);
			chart.CrosshairEnabled = false;
			ConfigureDiagram(chart);
			ConfigureSeries(chart.Diagram);
			if (ChartModel.Legend != null) {
				Legend legend = new Legend();
				Container.Register(legend, ChartModel.Legend);
				ConfigureLegend(legend);
				chart.Legend = legend;
			}
			else
				chart.Legend = null;
			ConfigureChartTitles(chart.Titles);
			if (ChartModel.Palette != null)
				ConfigureChartPalette(chart);
			ConfigureChartAppearance(chart, ChartModel.Appearance);
		}
		public void Update(ChartControl chart, Model.UpdateInfo updateInfo) {
			if (updateInfo.Element is Model.Chart)
				UpdateChart(chart, updateInfo);
		}
	}
}
