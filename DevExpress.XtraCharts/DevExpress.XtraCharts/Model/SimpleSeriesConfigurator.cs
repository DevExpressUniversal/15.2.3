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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public class TransparencyConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			Model.ISupportTransparencySeries modelSeries = model as Model.ISupportTransparencySeries;
			if(modelSeries == null) return;
			ISupportTransparency view = series.View as ISupportTransparency;
			if(view != null) view.Transparency = modelSeries.Transparency;
		}
	}
	public class ColorEachConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			Model.ISupportColorEachSeries modelSeries = model as Model.ISupportColorEachSeries;
			if(modelSeries == null) return;
			IColorEachSupportView view = series.View as IColorEachSupportView;
			if(view != null) view.ColorEach = modelSeries.ColorEach;
		}
	}
	public class BarWidthConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			IBarSeriesView barView = series.View as IBarSeriesView;
			Model.BarSeries barModel = model as Model.BarSeries;
			if (barView != null && barModel != null)
				barView.BarWidth = barModel.BarWidth;
		}
	}
	public class Bar3DModelConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			Bar3DSeriesView barView = series.View as Bar3DSeriesView;
			Model.ISupportBar3DModelSeries barSeriesModel = model as Model.ISupportBar3DModelSeries;
			if (barView != null && barSeriesModel != null) {
				Bar3DModel barModel;
				switch (barSeriesModel.Model) {
					case Model.Bar3DModel.Box: barModel = Bar3DModel.Box;
						break;
					case Model.Bar3DModel.Cone: barModel = Bar3DModel.Cone;
						break;
					case Model.Bar3DModel.Cylinder: barModel = Bar3DModel.Cylinder;
						break;
					case Model.Bar3DModel.Pyramid: barModel = Bar3DModel.Pyramid;
						break;
					default: barModel = Bar3DModel.Box;
						break;
				}
				barView.Model = barModel;
			}
		}
	}
	public class DataPropertiesConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			series.DataSource = model.DataSource;
			series.ArgumentDataMember = GetDataMemberValue(model.DataMembers, Model.DataMemberType.Argument);
			string[] valueMembers = GetValueDataMembers(model.DataMembers);
			series.ValueDataMembers.AddRange(valueMembers);
		}
		string GetDataMemberValue(Dictionary<Model.DataMemberType, string> dataMembers, Model.DataMemberType key) {
			string value = null;
			if(dataMembers.TryGetValue(key, out value))
				return value;
			return null;
		}
		string[] GetValueDataMembers(Dictionary<Model.DataMemberType, string> dataMembers) {
			if(dataMembers.Count == 0)
				return new string[0];
			List<string> result = new List<string>();
			foreach(KeyValuePair<Model.DataMemberType, string> item in dataMembers) {
				if(item.Key != Model.DataMemberType.Argument && !string.IsNullOrEmpty(item.Value))
					result.Add(item.Value);
			}
			return result.ToArray();
		}
	}
	public class SplineConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			ISplineSeriesView view = series.View as ISplineSeriesView;
			if(view != null) view.LineTensionPercent = 100;
		}
	}
	public class SeriesCommonPropertiesConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			series.Name = model.DisplayName;
			series.LabelsVisibility = model.LabelsVisibility ? DefaultBoolean.True : DefaultBoolean.False;
		}
	}
	public class ScaleTypeConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			series.ArgumentScaleType = (ScaleType)model.ArgumentScaleType;
			series.ValueScaleType = (ScaleType)model.ValueScaleType;
		}
	}
	public class LegendPropertiesConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			series.ShowInLegend = model.ShowInLegend;
			series.LegendTextPattern = !string.IsNullOrEmpty(model.LegendPointPattern) ? model.LegendPointPattern : PointOptionsHelper.DefaultPattern;
		}
	}
	public class MarkerConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			Model.ISupportMarkerSeries modelSeries = model as Model.ISupportMarkerSeries;
			if(modelSeries == null) return;
			Model.Marker modelMarker = modelSeries.Marker;
			if(modelMarker == null) return;
			IPointSeriesView view = series.View as IPointSeriesView;
			if(view == null) return;
			MarkerBase markerBase = view.Marker;
			markerBase.Kind = (MarkerKind)modelMarker.MarkerType;
			SimpleMarker simpleMarker = markerBase as SimpleMarker;
			if(simpleMarker != null)
				simpleMarker.Size = modelMarker.Size;
			Marker marker = markerBase as Marker;
			if(marker != null && modelMarker.Color != Model.ColorARGB.Empty)
				marker.Color = ModelConfigaratorHelper.ToColor(modelMarker.Color);
		}
	}
	public class SecondaryAxesConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			Model.CartesianSeriesBase modelSeries = (Model.CartesianSeriesBase)model;
			XYDiagramSeriesViewBase view = (XYDiagramSeriesViewBase)series.View;
			XYDiagram diagram = Diagram as XYDiagram;
			if(diagram == null) return;
			int indexX = modelSeries.SecondaryArgumentAxisIndex;
			AxisXBase axisX = GetSecondaryAxisByIndex(diagram.SecondaryAxesX, indexX) as AxisXBase;
			if(axisX != null)
				view.AxisX = axisX;
			int indexY = modelSeries.SecondaryValueAxisIndex;
			AxisYBase axisY = GetSecondaryAxisByIndex(diagram.SecondaryAxesY, indexY) as AxisYBase;
			if(axisY != null)
				view.AxisY = axisY;
		}
		AxisBase GetSecondaryAxisByIndex(SecondaryAxisCollection collection, int index) {
			return (index >= 0 && index < collection.Count) ? collection[index] : null;
		}
	}
	public class SeriesLabelConfigurator : SeriesConfiguratorBase {
		public override void Configure(Series series, Model.SeriesModel model) {
			if (model.Label != null) {
				series.Label.EnableAntialiasing = model.Label.EnableAntialiasing;
				if (model.Label.Formatter != null)
					series.Label.Formatter = new ModelDataLabelFormatter(model.Label.Formatter, series.View);
			}
		}
	}
	public class ModelDataLabelFormatter : IDataLabelFormatter {
		readonly Model.IDataLabelFormatter formatter;
		readonly SeriesViewBase seriesView;
		public ModelDataLabelFormatter(Model.IDataLabelFormatter formatter, SeriesViewBase seriesView) {
			this.formatter = formatter;
			this.seriesView = seriesView;
		}
		double GetNormalizedValue(RefinedPoint point) {
			if (seriesView is PieSeriesViewBase)
				return ((IPiePoint)point).NormalizedValue;
			if (seriesView is FunnelSeriesView)
				return ((IFunnelPoint)point).NormalizedValue;
			if (seriesView is FullStackedBarSeriesView ||
				seriesView is SideBySideFullStackedBarSeriesView ||
				seriesView is FullStackedArea3DSeriesView ||
				seriesView is FullStackedAreaSeriesView ||
				seriesView is FullStackedBar3DSeriesView ||
				seriesView is FullStackedLine3DSeriesView ||
				seriesView is FullStackedLineSeriesView ||
				seriesView is FullStackedSplineArea3DSeriesView ||
				seriesView is SideBySideFullStackedBar3DSeriesView)
				return ((IFullStackedPoint)point).NormalizedValue;
			return double.NaN;
		}
		public string GetDataLabelText(RefinedPoint point) {
			SeriesPoint seriesPoint = point.SeriesPoint as SeriesPoint;
			if (seriesPoint != null) {
				Model.LabelPointData pointData = new Model.LabelPointData(seriesPoint.Tag, ((ISeriesPoint)seriesPoint).UserArgument, GetNormalizedValue(point));
				return formatter.GetDataLabelText(pointData);
			}
			else
				return String.Empty;
		}
	}
	public static class ModelConfigaratorHelper {
		public static DefaultBoolean CalcMarkerVisibility(Model.Marker model) {
			if(model == null)
				return DefaultBoolean.Default;
			return model.Visible ? DefaultBoolean.True : DefaultBoolean.False;
		}
		public static Color ToColor(Model.ColorARGB model) {
			return model != Model.ColorARGB.Empty ? Color.FromArgb(model.A, model.R, model.G, model.B) : Color.Empty;
		}
	}
}
