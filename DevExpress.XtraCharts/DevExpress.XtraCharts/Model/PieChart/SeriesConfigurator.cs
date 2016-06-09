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
	public abstract class PieChartBaseConfigurator : ChartBaseConfigurator {
		public override void FillConfiguratorList() {
			base.FillConfiguratorList();
			AddConfigurator(new PieSeriesCommonConfigurator()); 
		}
	}
	public class PieSeriesCommonConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			Model.PieSeriesBase modelSeries = (Model.PieSeriesBase)model;
			PieSeriesViewBase view = (PieSeriesViewBase)series.View;
			view.SweepDirection = (PieSweepDirection)modelSeries.SweepDirection;
			switch(modelSeries.ExplodeMode) {
				case Model.PieExplodeMode.All:
					view.ExplodeMode = PieExplodeMode.All;
					view.ExplodedDistancePercentage = modelSeries.ExplodedDistancePercentage;
					break;
				case Model.PieExplodeMode.UsePoints:
					view.ExplodeMode = PieExplodeMode.UsePoints;
					view.ExplodedDistancePercentage = modelSeries.ExplodedDistancePercentage;
					view.ExplodedPoints.Clear();
					foreach(int pointIndex in modelSeries.ExplodedPointsIndexes)
						view.ExplodedPoints.Add((SeriesPoint)series.Points[pointIndex]);
					break;
				default:
					view.ExplodeMode = PieExplodeMode.None;
					break;
			}
		}
	}
	public class GenericPieSeriesConfigurator<T> : PieChartBaseConfigurator where T : PieSeriesView {
		public override void Configure(Series series, Model.SeriesModel model) {
			base.Configure(series, model);
			ConfigureView((T)series.View, (Model.PieSeriesBase)model);
		}
		protected virtual void ConfigureView(T view, Model.PieSeriesBase model) {
			view.Rotation = model.RotationAngle;
			if (model.Label != null) {
				PieSeriesLabel label = (PieSeriesLabel)view.Label;
				switch (model.Label.Position) {
					case Model.SeriesLabelPosition.OutsideEnd:
						label.Position = PieSeriesLabelPosition.Outside;
						break;
					case Model.SeriesLabelPosition.Center:
						label.Position = PieSeriesLabelPosition.Inside;
						break;
					case Model.SeriesLabelPosition.BestFit:
						label.Position = PieSeriesLabelPosition.TwoColumns;
						label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
						break;
					default:
						break;
				}
			}
		}
	}
	public class PieSeriesConfigurator : GenericPieSeriesConfigurator<PieSeriesView> { 
	}
	public class DonutSeriesConfigurator : GenericPieSeriesConfigurator<DoughnutSeriesView> {
		protected override void ConfigureView(DoughnutSeriesView view, Model.PieSeriesBase model) {
			base.ConfigureView(view, model);
			view.HoleRadiusPercent = ((Model.DonutSeries)model).HoleRadiusPercent;
		}
	}
	public class GenericPie3DSeriesConfigurator<T> : PieChartBaseConfigurator where T : Pie3DSeriesView, new() {
		public override void Configure(Series series, Model.SeriesModel model) {
			base.Configure(series, model);
			ConfigureView((T)series.View, (Model.PieSeriesBase)model);
		}
		protected virtual void ConfigureView(T view, Model.PieSeriesBase model) {
			view.Depth = model.DepthPercent;
		}
	}
	public class NestedDonutSeriesConfigurator : GenericPieSeriesConfigurator<NestedDoughnutSeriesView> {
		protected override void ConfigureView(NestedDoughnutSeriesView view, Model.PieSeriesBase model) {
			base.ConfigureView(view, model);
			view.HoleRadiusPercent = ((Model.DonutSeries)model).HoleRadiusPercent;
			view.InnerIndent = 0;
		}
	}
}
