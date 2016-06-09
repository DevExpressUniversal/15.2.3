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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public abstract class ChartBaseSeriesCollectionConfigurator {
		readonly SeriesConfiguratorFactoryBase factory;
		protected internal SeriesConfiguratorFactoryBase Factory { get { return factory; } }
		public virtual Diagram Diagram { get; set; }
		protected ChartBaseSeriesCollectionConfigurator() {
			this.factory = CreateFactory();
		}
		protected abstract SeriesConfiguratorFactoryBase CreateFactory();
		void ConfigurateSeriesAxes(Series series, Model.SeriesModel item) {
			SeriesConfiguratorBase configurator = Factory.CreateSeriesConfigurator(series.View);
			if (configurator != null) {
				configurator.Diagram = Diagram;
				configurator.ConfigureSecondaryAxes(series, item);
			}
		}
		internal void ConfigurateSeries(Series series, Model.SeriesModel item) {
			SeriesConfiguratorBase configurator = Factory.CreateSeriesConfigurator(series.View);
			if (configurator != null) {
				configurator.Diagram = Diagram;
				configurator.Configure(series, item);
			}
		}
		public void CreateSeries(Chart chart, Model.Chart model, Model.IModelElementContainer container) {
			chart.Series.Clear();
			IList<Model.SeriesModel> modelSeries = model.Series;
			foreach(Model.SeriesModel item in model.Series) {
				SeriesViewBase view = Factory.CreateSeriesView(item);
				if(view == null)
					continue;
				Series series = new Series();
				series.View = view;
				container.Register(series, item);
				chart.Series.Add(series);
			}
		}
		public void Configurate(Chart chart, Model.Chart model, Model.IModelElementContainer container) {
			Diagram = chart.Diagram;
			IList<Model.SeriesModel> modelSeries = model.Series;
			foreach(Model.SeriesModel item in modelSeries) {
				Series series = (Series)container.FindViewObject(item);
				ConfigurateSeries(series, item);
			}
		}
		public void ConfigurateSecondaryAxes(Chart chart, Model.Chart model, Model.IModelElementContainer container) {
			IList<Model.SeriesModel> modelSeries = model.Series;
			foreach (Model.SeriesModel item in modelSeries) {
				Series series = (Series)container.FindViewObject(item);
				ConfigurateSeriesAxes(series, item);
			}
		}
	}
	public class ChartSeriesCollectionConfigurator : ChartBaseSeriesCollectionConfigurator {
		public ChartSeriesCollectionConfigurator() {
		}
		protected override SeriesConfiguratorFactoryBase CreateFactory() {
			return new SeriesConfiguratorFactory();
		}
	}
	public class Chart3DSeriesCollectionConfigurator : ChartBaseSeriesCollectionConfigurator {
		public Chart3DSeriesCollectionConfigurator() {
		}
		protected override SeriesConfiguratorFactoryBase CreateFactory() {
			return new Series3DConfiguratorFactory();
		}
	}
}
