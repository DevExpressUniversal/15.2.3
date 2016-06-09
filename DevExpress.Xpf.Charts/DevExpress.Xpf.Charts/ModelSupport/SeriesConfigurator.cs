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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class ModelDataLabelFormatter : IDataLabelFormatter {
		readonly Model.IDataLabelFormatter formatter;
		readonly Series series;
		public ModelDataLabelFormatter(Model.IDataLabelFormatter formatter, Series series) {
			this.formatter = formatter;
			this.series = series;
		}
		double GetNormalizedValue(ISeriesPoint point) {
			if (series is PieSeries)
				return new PieNormalizedValuesCalculator(series).CalculateNormalizedValue(point);
			return double.NaN;
		}
		public string GetDataLabelText(ISeriesPoint point) {
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(point);
			if (seriesPoint != null) {
				Model.LabelPointData pointData = new Model.LabelPointData(seriesPoint.Tag, ((ISeriesPoint)seriesPoint).UserArgument, GetNormalizedValue(point));
				return formatter.GetDataLabelText(pointData);
			}
			else
				return String.Empty;
		}
	}
	public partial class SeriesConfigurator {
		readonly Dictionary<Type, Type> modelSeriesMapping2D = new Dictionary<Type, Type>();
		readonly Dictionary<Type, SeriesPropertiesConfiguratorBase[]> seriesConfiguratorsMapping = new Dictionary<Type, SeriesPropertiesConfiguratorBase[]>();
		public SeriesConfigurator() {
			FillModelSeriesMapping();
			FillSeriesConfiguratorsMapping();
		}
		void FillModelSeriesMapping2D() {
			modelSeriesMapping2D.Add(typeof(Model.BubbleSeries), typeof(BubbleSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.PointSeries), typeof(PointSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.RadarPointSeries), typeof(RadarPointSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.PolarPointSeries), typeof(PolarPointSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.ScatterLineSeries), typeof(LineScatterSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.LineSeries), typeof(LineSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.StepLineSeries), typeof(LineStepSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.RadarLineSeries), typeof(RadarLineSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.PolarLineSeries), typeof(PolarLineSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.StackedLineSeries), typeof(LineStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.FullStackedLineSeries), typeof(LineFullStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.SideBySideBarSeries), typeof(BarSideBySideSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.StackedBarSeries), typeof(BarStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.FullStackedBarSeries), typeof(BarFullStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.SideBySideStackedBarSeries), typeof(BarSideBySideStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.SideBySideFullStackedBarSeries), typeof(BarSideBySideFullStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.RangeBarSeries), typeof(RangeBarOverlappedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.SideBySideRangeBarSeries), typeof(RangeBarSideBySideSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.AreaSeries), typeof(AreaSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.StepAreaSeries), typeof(AreaStepSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.RangeAreaSeries), typeof(RangeAreaSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.StackedAreaSeries), typeof(AreaStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.FullStackedAreaSeries), typeof(AreaFullStackedSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.RadarAreaSeries), typeof(RadarAreaSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.PolarAreaSeries), typeof(PolarAreaSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.StockSeries), typeof(StockSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.CandleStickSeries), typeof(CandleStickSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.PieSeries), typeof(PieSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.DonutSeries), typeof(PieSeries2D));
			modelSeriesMapping2D.Add(typeof(Model.NestedDonutSeries), typeof(NestedDonutSeries2D));
		}
		void FillSeriesConfiguratorsMapping2D() {
			SeriesPropertiesConfiguratorBase[] barConfigurators = new SeriesPropertiesConfiguratorBase[] { 
				new BarSeries2DPropertiesConfigurator(), 
				new ColorEachSeriesPropertiesConfigurator() 
			};
			SeriesPropertiesConfiguratorBase[] rangeBarConfigurators = new SeriesPropertiesConfiguratorBase[] { 
				new RangeBarSeries2DPropertiesConfigurator(), 
				new ColorEachSeriesPropertiesConfigurator() 
			};
			seriesConfiguratorsMapping.Add(typeof(BarSideBySideSeries2D), barConfigurators);
			seriesConfiguratorsMapping.Add(typeof(BarStackedSeries2D), barConfigurators);
			seriesConfiguratorsMapping.Add(typeof(BarFullStackedSeries2D), barConfigurators);
			seriesConfiguratorsMapping.Add(typeof(BarSideBySideStackedSeries2D), barConfigurators);
			seriesConfiguratorsMapping.Add(typeof(BarSideBySideFullStackedSeries2D), barConfigurators);
			seriesConfiguratorsMapping.Add(typeof(RangeBarOverlappedSeries2D), rangeBarConfigurators);
			seriesConfiguratorsMapping.Add(typeof(RangeBarSideBySideSeries2D), rangeBarConfigurators);
			SeriesPropertiesConfiguratorBase[] markerConfigurators = new SeriesPropertiesConfiguratorBase[] { 
				new MarkerSeries2DPropertiesConfigurator(), 
				new ColorEachSeriesPropertiesConfigurator(),
				new PointSeriesLabelPropertiesConfigurator()
			};
			SeriesPropertiesConfiguratorBase[] bubbleConfigurators = new SeriesPropertiesConfiguratorBase[] { 
				new BublleSeries2DPropertiesConfigurator(), 
				new ColorEachSeriesPropertiesConfigurator(),
				new BubbleSeriesLabelPropertiesConfigurator()
			};
			seriesConfiguratorsMapping.Add(typeof(BubbleSeries2D), bubbleConfigurators);
			seriesConfiguratorsMapping.Add(typeof(PointSeries2D), markerConfigurators);
			seriesConfiguratorsMapping.Add(typeof(LineScatterSeries2D), markerConfigurators);
			seriesConfiguratorsMapping.Add(typeof(LineSeries2D), markerConfigurators);
			seriesConfiguratorsMapping.Add(typeof(LineStepSeries2D), markerConfigurators);
			seriesConfiguratorsMapping.Add(typeof(LineStackedSeries2D), markerConfigurators);
			seriesConfiguratorsMapping.Add(typeof(LineFullStackedSeries2D), markerConfigurators);
			SeriesPropertiesConfiguratorBase[] areaConfigurators = new SeriesPropertiesConfiguratorBase[] { 
				new MarkerSeries2DPropertiesConfigurator(), 
				new ColorEachSeriesPropertiesConfigurator(),
				new TransparencySeriesPropertiesConfigurator()
			};
			seriesConfiguratorsMapping.Add(typeof(AreaSeries2D), areaConfigurators);
			seriesConfiguratorsMapping.Add(typeof(AreaStepSeries2D), areaConfigurators);
			seriesConfiguratorsMapping.Add(typeof(RangeAreaSeries2D), areaConfigurators);
			seriesConfiguratorsMapping.Add(typeof(AreaStackedSeries2D), areaConfigurators);
			seriesConfiguratorsMapping.Add(typeof(AreaFullStackedSeries2D), areaConfigurators);
			SeriesPropertiesConfiguratorBase[] financialConfigurators = new SeriesPropertiesConfiguratorBase[] {
				new GeneralSeriesPropertiesConfigurator()
			};
			seriesConfiguratorsMapping.Add(typeof(StockSeries2D), financialConfigurators);
			seriesConfiguratorsMapping.Add(typeof(CandleStickSeries2D), financialConfigurators);
			seriesConfiguratorsMapping.Add(typeof(PieSeries2D), new SeriesPropertiesConfiguratorBase[] { new PieSeries2DPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(NestedDonutSeries2D), new SeriesPropertiesConfiguratorBase[] { new NestedDonutPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(RadarPointSeries2D), new SeriesPropertiesConfiguratorBase[] { new CircularSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(PolarPointSeries2D), new SeriesPropertiesConfiguratorBase[] { new CircularSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(RadarLineSeries2D), new SeriesPropertiesConfiguratorBase[] { new CircularLineSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(PolarLineSeries2D), new SeriesPropertiesConfiguratorBase[] { new CircularLineSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(RadarAreaSeries2D), new SeriesPropertiesConfiguratorBase[] { new CircularAreaSeriesPropertiesConfigurator() });
			seriesConfiguratorsMapping.Add(typeof(PolarAreaSeries2D), new SeriesPropertiesConfiguratorBase[] { new CircularAreaSeriesPropertiesConfigurator() });
		}
		public void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			SeriesPropertiesConfiguratorBase[] seriesConfigurators;
			if (seriesConfiguratorsMapping.TryGetValue(series.GetType(), out seriesConfigurators))
				foreach (SeriesPropertiesConfiguratorBase configurator in seriesConfigurators)
					configurator.Configure(series, seriesModel, diagram);
		}
	}
	public abstract class SeriesPropertiesConfiguratorBase {
		public abstract void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram);
	}
	public class GeneralSeriesPropertiesConfigurator : SeriesPropertiesConfiguratorBase {
		string GetDataMemberValue(Dictionary<Model.DataMemberType, string> dataMembers, Model.DataMemberType key) {
			string value = null;
			if (dataMembers.TryGetValue(key, out value))
				return value;
			return null;
		}
		void ConfigureDataMembers(Series series, Model.SeriesModel seriesModel) {
			series.ArgumentDataMember = GetDataMemberValue(seriesModel.DataMembers, Model.DataMemberType.Argument);
			string[] valueMembers = GetValueDataMembers(seriesModel.DataMembers);
			if (valueMembers.Length > 0) {
				series.ValueDataMember = valueMembers[0];
				if (valueMembers.Length > 1)
					ConfigureAdditionalValueDataMembers(series, valueMembers);
			}
		}
		void ConfigureAdditionalValueDataMembers(Series series, string[] valueDataMembers) {
			if (series is BubbleSeries2D)
				((BubbleSeries2D)series).WeightDataMember = valueDataMembers[1];
			if (series is BubbleSeries3D)
				((BubbleSeries3D)series).WeightDataMember = valueDataMembers[1];
			if(series is RangeBarSeries2D)
				((RangeBarSeries2D)series).Value2DataMember = valueDataMembers[1];
			FinancialSeries2D financialSeries = series as FinancialSeries2D;
			if (financialSeries != null && valueDataMembers.Length > 3) {
				financialSeries.LowValueDataMember = valueDataMembers[0];
				financialSeries.HighValueDataMember = valueDataMembers[1];
				financialSeries.OpenValueDataMember = valueDataMembers[2];
				financialSeries.CloseValueDataMember = valueDataMembers[3];
			}
		}
		void ConfigureLegendTextPattern(Series series, Model.SeriesModel seriesModel) {
			series.LegendTextPattern = seriesModel.LegendPointPattern;
		}
		void ConfigureScaleTypes(Series series, Model.SeriesModel seriesModel) {
			series.ArgumentScaleType = (ScaleType)seriesModel.ArgumentScaleType;
			series.ValueScaleType = (ScaleType)seriesModel.ValueScaleType;
		}
		string[] GetValueDataMembers(Dictionary<Model.DataMemberType, string> dataMembers) {
			if (dataMembers.Count == 0)
				return new string[0];
			List<string> result = new List<string>();
			foreach (KeyValuePair<Model.DataMemberType, string> item in dataMembers) {
				if (item.Key != Model.DataMemberType.Argument && !string.IsNullOrEmpty(item.Value))
					result.Add(item.Value);
			}
			return result.ToArray();
		}
		void ConfigureSeriesLabels(Series series, Model.SeriesModel seriesModel) {
			series.LabelsVisibility = seriesModel.LabelsVisibility;
			if (seriesModel.Label != null && seriesModel.Label.Formatter != null)
				series.ActualLabel.Formatter = new ModelDataLabelFormatter(seriesModel.Label.Formatter, series);
		}
		protected Marker2DModel GetMarkerModel(Model.MarkerType markerType) {
			switch (markerType) {
				case Model.MarkerType.Circle:
					return new CircleMarker2DModel();
				case Model.MarkerType.Cross:
					return new CrossMarker2DModel();
				case Model.MarkerType.Diamond:
					return new CircleMarker2DModel();
				case Model.MarkerType.Hexagon:
					return new PolygonMarker2DModel();
				case Model.MarkerType.InvertedTriangle:
					return new TriangleMarker2DModel();
				case Model.MarkerType.Pentagon:
					return new CircleMarker2DModel();
				case Model.MarkerType.Plus:
					return new CrossMarker2DModel();
				case Model.MarkerType.Square:
					return new SquareMarker2DModel();
				case Model.MarkerType.Star:
					return new StarMarker2DModel();
				case Model.MarkerType.Triangle:
					return new TriangleMarker2DModel();
				default:
					return new CircleMarker2DModel();
			}
		}
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			series.DisplayName = seriesModel.DisplayName;
			series.ShowInLegend = seriesModel.ShowInLegend;
			series.DataSource = seriesModel.DataSource;
			ConfigureScaleTypes(series, seriesModel);
			ConfigureDataMembers(series, seriesModel);
			ConfigureLegendTextPattern(series, seriesModel);
			ConfigureSeriesLabels(series, seriesModel);
		}
	}
	public class ColorEachSeriesPropertiesConfigurator : GeneralSeriesPropertiesConfigurator {
		public override void Configure(Series series, Model.SeriesModel seriesModel, Diagram diagram) {
			base.Configure(series, seriesModel, diagram);
			XYSeries xySeries = (XYSeries)series;
			if (seriesModel is Model.ISupportColorEachSeries)
				xySeries.ColorEach = ((Model.ISupportColorEachSeries)seriesModel).ColorEach;
		}
	}
}
