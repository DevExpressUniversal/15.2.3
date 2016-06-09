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
using DevExpress.XtraCharts.Native;
using Model = DevExpress.Charts.Model;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public abstract class RadarChartBaseSeriesConfigurator : ChartBaseConfigurator {
		public RadarDiagram ActialDiagram { get { return base.Diagram as RadarDiagram; } }
		public override void FillConfiguratorList() {
			base.FillConfiguratorList();
			AddConfigurator(new ColorEachConfigurator());
			AddConfigurator(new MarkerConfigurator());
		}
	}
	public class RadarChartSeriesConfigurator<T> : RadarChartBaseSeriesConfigurator where T : RadarSeriesViewBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			base.Configure(series, model);
			ConfigureView((T)series.View, model);
		}
		protected virtual void ConfigureView(T view, Model.SeriesModel model) {
		}
	}
	public class RadarPointBasedSeriesConfigurator<T> : RadarChartSeriesConfigurator<T> where T : RadarPointSeriesView { 
	}
	public class RadarLineBasedSeriesConfigurator<T> : RadarPointBasedSeriesConfigurator<T> where T : RadarLineSeriesView {
		protected override void ConfigureView(T view, Model.SeriesModel model) {
			base.ConfigureView(view, model);
			Model.ISupportMarkerSeries modelSeries = model as Model.ISupportMarkerSeries;
			if(modelSeries != null) {
				view.MarkerVisibility = ModelConfigaratorHelper.CalcMarkerVisibility(modelSeries.Marker);
			}
		}
	}
	public class RadarAreaBasedSeriesConfigurator<T> : RadarLineBasedSeriesConfigurator<T> where T : RadarAreaSeriesView {
	}
}
