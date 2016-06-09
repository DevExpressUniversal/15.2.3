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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DevExpress.Utils;
using Model = DevExpress.Charts.Model;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class CartesianSeriesPropertiesConfigurator : GeneralSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			XYSeries xySeries = (XYSeries)series;
			ConfigureAxis(xySeries, (Model.CartesianSeriesBase)seriesModel, (XYDiagram2D)diagram);
		}
		void ConfigureAxis(XYSeries series, Model.CartesianSeriesBase seriesModel, XYDiagram2D diagram) {
			SecondaryAxisX2D axisX = GetSecondaryAxisXByIndex(seriesModel.SecondaryArgumentAxisIndex, diagram.SecondaryAxesX);
			if (axisX != null)
				XYDiagram2D.SetSeriesAxisX(series, axisX);
			SecondaryAxisY2D axisY = GetSecondaryAxisYByIndex(seriesModel.SecondaryValueAxisIndex, diagram.SecondaryAxesY);
			if (axisY != null)
				XYDiagram2D.SetSeriesAxisY(series, axisY);
		}
		SecondaryAxisX2D GetSecondaryAxisXByIndex(int index, SecondaryAxisXCollection collection) {
			return (index >= 0 && index < collection.Count) ? collection[index] : null;
		}
		SecondaryAxisY2D GetSecondaryAxisYByIndex(int index, SecondaryAxisYCollection collection) {
			return (index >= 0 && index < collection.Count) ? collection[index] : null;
		}
	}
	public abstract class BarSeries2DPropertiesConfiguratorBase : CartesianSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			((BarSeries2DBase)series).BarWidth = ((Model.ISupportBarWidthSeries)seriesModel).BarWidth;
			if (seriesModel.Label != null) {
				SeriesLabel label = series.ActualLabel;
				switch (seriesModel.Label.Position) {
					case Model.SeriesLabelPosition.Center:
					case Model.SeriesLabelPosition.InsideBase:
					case Model.SeriesLabelPosition.InsideEnd:
						BarSideBySideSeries2D.SetLabelPosition(label, Bar2DLabelPosition.Center);
						break;
					case Model.SeriesLabelPosition.OutsideEnd:
						BarSideBySideSeries2D.SetLabelPosition(label, Bar2DLabelPosition.Outside);
						break;
					default:
						break;
				}
			}
		}
	}
	public class BarSeries2DPropertiesConfigurator : BarSeries2DPropertiesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			((BarSeries2D)series).Model = new SimpleBar2DModel();
		}
	}
	public class RangeBarSeries2DPropertiesConfigurator : BarSeries2DPropertiesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			((RangeBarSeries2D)series).Model = new SimpleRangeBar2DModel();
		}
	}
	public class MarkerSeries2DPropertiesConfigurator : CartesianSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			Model.Marker markerModel = ((Model.ISupportMarkerSeries)seriesModel).Marker;
			if (markerModel != null) {
				ISupportMarker2D markerSeries = (ISupportMarker2D)series;
				if (!(series is PointSeries2D) && !(series is BubbleSeries2D))
					markerSeries.MarkerVisible = markerModel.Visible;
				markerSeries.MarkerSize = markerModel.Size;
				markerSeries.MarkerModel = GetMarkerModel(markerModel.MarkerType);
			}
		} 
	}
	public class BublleSeries2DPropertiesConfigurator : MarkerSeries2DPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			Model.BubbleSeries bubbleModel = seriesModel as Model.BubbleSeries;
			BubbleSeries2D bubbleSeries = series as BubbleSeries2D;
			if (bubbleModel != null && bubbleSeries != null)
				if (bubbleModel.MinSize < bubbleSeries.MaxSize) {
					bubbleSeries.MinSize = bubbleModel.MinSize;
					bubbleSeries.MaxSize = bubbleModel.MaxSize;
				}
				else {
					bubbleSeries.MaxSize = bubbleModel.MaxSize;
					bubbleSeries.MinSize = bubbleModel.MinSize;
				}
		}
	}
	public class TransparencySeriesPropertiesConfigurator : SeriesPropertiesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			((ISupportTransparency)series).Transparency = ((Model.ISupportTransparencySeries)seriesModel).Transparency / 255.0;
		}
	}
	public class PointSeriesLabelPropertiesConfigurator : SeriesPropertiesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			if (seriesModel.Label != null) {
				SeriesLabel label = series.ActualLabel;
				switch (seriesModel.Label.Position) {
					case Model.SeriesLabelPosition.Left:
						MarkerSeries2D.SetAngle(label, 180);
						break;
					case Model.SeriesLabelPosition.Top:
						MarkerSeries2D.SetAngle(label, 90);
						break;
					case Model.SeriesLabelPosition.Righ:
						MarkerSeries2D.SetAngle(label, 0);
						break;
					case Model.SeriesLabelPosition.Bottom:
						MarkerSeries2D.SetAngle(label, 270);
						break;
					default:
						break;
				}
			}			
		}
	}
	public class BubbleSeriesLabelPropertiesConfigurator : PointSeriesLabelPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			if (seriesModel.Label != null) {
				SeriesLabel label = series.ActualLabel;
				switch (seriesModel.Label.Position) {
					case Model.SeriesLabelPosition.Center:
						BubbleSeries2D.SetLabelPosition(label, Bubble2DLabelPosition.Center);
						break;
					default:
						BubbleSeries2D.SetLabelPosition(label, Bubble2DLabelPosition.Outside);
						break;
				}
			}
		}
	}
}
