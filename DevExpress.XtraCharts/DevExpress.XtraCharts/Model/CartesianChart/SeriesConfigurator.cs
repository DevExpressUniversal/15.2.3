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
	public abstract class CartesianChartBaseSeriesConfigurator : ChartBaseConfigurator {
		public override void FillConfiguratorList() {
			base.FillConfiguratorList();
			AddConfigurator(new ColorEachConfigurator());
			AddConfigurator(new MarkerConfigurator());
			AddConfigurator(new BarWidthConfigurator());
			AddConfigurator(new Bar3DModelConfigurator());
		}
	}
	public abstract class CartesianChartSeriesConfigurator : CartesianChartBaseSeriesConfigurator {
		public XYDiagram ActialDiagram { get { return base.Diagram as XYDiagram; } }
		public override void FillConfiguratorList() {
			base.FillConfiguratorList();
			AddConfigurator(new SecondaryAxesConfigurator());
		}
	}
	public abstract class CartesianSeriesConfigurator<T> : CartesianChartSeriesConfigurator where T : SeriesViewBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			base.Configure(series, model);
			ConfigureView((T)series.View, (Model.CartesianSeriesBase)model);
		}
		protected virtual void ConfigureView(T view, Model.CartesianSeriesBase model) {
		}
	}
	public class BarBasedSeriesConfigurator<T> : CartesianSeriesConfigurator<T> where T : BarSeriesView {
		protected override void ConfigureView(T view, Model.CartesianSeriesBase model) {
			base.ConfigureView(view, model);
			if (model.Label != null) {
				BarSeriesLabel label = (BarSeriesLabel)view.Label;
				switch (model.Label.Position) {
					case Model.SeriesLabelPosition.Center:
						label.Position = BarSeriesLabelPosition.Center;
						break;
					case Model.SeriesLabelPosition.InsideBase:
						label.Position = BarSeriesLabelPosition.BottomInside;
						break;
					case Model.SeriesLabelPosition.InsideEnd:
						label.Position = BarSeriesLabelPosition.TopInside;
						break;
					case Model.SeriesLabelPosition.OutsideEnd:
						label.Position = BarSeriesLabelPosition.Top;
						break;
					default:
						break;
				}
			}
		}
	}
	public class PointBasedSeriesConfigurator<T> : CartesianSeriesConfigurator<T> where T : PointSeriesViewBase {
		protected override void ConfigureView(T view, Model.CartesianSeriesBase model) {
			base.ConfigureView(view, model);
			if (model.Label != null) {
				PointSeriesLabel label = (PointSeriesLabel)view.Label;
				Model.SeriesLabelPosition modelPosition = model.Label.Position;
				label.Position = modelPosition == Model.SeriesLabelPosition.Center ? PointLabelPosition.Center : PointLabelPosition.Outside;
				switch (modelPosition) {
					case Model.SeriesLabelPosition.Left:
						label.Angle = 180;
						break;
					case Model.SeriesLabelPosition.Top:
						label.Angle = 90;
						break;
					case Model.SeriesLabelPosition.Righ:
						label.Angle = 0;
						break;
					case Model.SeriesLabelPosition.Bottom:
						label.Angle = 270;
						break;
					default:
						break;
				}
			}
		}
	}
	public class BubbleSeriesConfigurator : PointBasedSeriesConfigurator<BubbleSeriesView> {
		protected override void ConfigureView(BubbleSeriesView view, Model.CartesianSeriesBase model) {
			base.ConfigureView(view, model);
			Model.BubbleSeries bubbleModel = model as Model.BubbleSeries;
			if (bubbleModel != null)
				if (bubbleModel.MinSize < view.MaxSize) {
					view.MinSize = bubbleModel.MinSize;
					view.MaxSize = bubbleModel.MaxSize;
				}
				else {
					view.MaxSize = bubbleModel.MaxSize;
					view.MinSize = bubbleModel.MinSize;
				}
		}
	}
	public class LineBasedSeriesConfigurator<T> : PointBasedSeriesConfigurator<T> where T : LineSeriesView {
		protected override void ConfigureView(T view, Model.CartesianSeriesBase model) {
			base.ConfigureView(view, model);
			Model.ISupportMarkerSeries modelSeries = model as Model.ISupportMarkerSeries;
			if(modelSeries != null)
				view.MarkerVisibility = ModelConfigaratorHelper.CalcMarkerVisibility(modelSeries.Marker);
		}
	}
	public class AreaBasedSeriesConfigurator<T> : CartesianSeriesConfigurator<T> where T : AreaSeriesViewBase { 
	}
	public class FinancialSeriesConfigurator<T> : CartesianSeriesConfigurator<T> where T : FinancialSeriesViewBase {
	}
	public abstract class CartesianSeries3DConfigurator<T> : CartesianChartBaseSeriesConfigurator where T : XYDiagram3DSeriesViewBase {
		public Diagram3D ActialDiagram { get { return base.Diagram as Diagram3D; } }
	}
	public class Bar3DBasedSeriesConfigurator<T> : CartesianSeries3DConfigurator<T> where T : Bar3DSeriesView {
	}
	public class Line3DBasedSeriesConfigurator<T> : CartesianSeries3DConfigurator<T> where T : Line3DSeriesView {
	}
	public class Area3DBasedSeriesConfigurator<T> : Line3DBasedSeriesConfigurator<T> where T : Area3DSeriesView {
	}
}
