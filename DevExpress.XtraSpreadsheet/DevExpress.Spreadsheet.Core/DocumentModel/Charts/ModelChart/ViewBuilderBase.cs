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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model.ModelChart {
	#region IModelViewBuilder
	public interface IModelViewBuilder {
		void Build();
	}
	#endregion
	#region ModelViewBuilderBase
	public abstract class ModelViewBuilderBase : IModelViewBuilder {
		#region Fields
		ModelChartBuilder modelBuilder;
		IChartView view;
		#endregion
		protected ModelViewBuilderBase(ModelChartBuilder modelBuilder, IChartView view) {
			this.modelBuilder = modelBuilder;
			this.view = view;
		}
		protected ModelChartBuilder ModelBuilder { get { return modelBuilder; } }
		protected ChartsModel.Chart ModelChart { get { return modelBuilder.ModelChart; } }
		protected Chart SpreadsheetChart { get { return modelBuilder.SpreadsheetChart; } }
		protected IChartView View { get { return view; } }
		protected bool IsFirstSeries { get { return this.modelBuilder.SeriesIndex == 0; } }
		protected int SeriesIndex { get { return this.modelBuilder.SeriesIndex; } }
		#region IViewBuilder Members
		public virtual void Build() {
			List<ISeries> seriesList = GetSeriesList();
			foreach (ISeries series in seriesList) {
				if (series.IsCompatible(view)) {
					ChartsModel.SeriesModel modelSeries = CreateModelSeries(series);
					if (modelSeries != null) {
						SetupSeries(modelSeries, series);
						ModelChart.Series.Add(modelSeries);
						this.modelBuilder.SeriesIndex++;
					}
				}
			}
		}
		#endregion
		protected abstract ChartsModel.SeriesModel CreateModelSeries(ISeries series);
		protected virtual List<ISeries> GetSeriesList() {
			return view.Series.ByOrder();
		}
		protected virtual void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			modelSeries.DisplayName = GetDisplayName(series);
			SetupSeriesDataMembers(modelSeries, series);
			SetupSeriesShowInLegend(modelSeries);
			SetupSeriesAxes(modelSeries as ChartsModel.CartesianSeriesBase, series);
			SetupSeriesTransparency(modelSeries, series);
			SetupSeriesVaryColors(modelSeries);
			SetupSeriesDataLabels(modelSeries, series as SeriesWithDataLabelsAndPoints);
		}
		protected virtual void SetupSeriesDataMembers(ChartsModel.SeriesModel modelSeries, ISeries series) {
			modelSeries.DataMembers[ChartsModel.DataMemberType.Argument] = "Argument";
			modelSeries.DataMembers[ChartsModel.DataMemberType.Value] = "Value0";
			modelSeries.ArgumentScaleType = GetArgumentScaleType(series);
			modelSeries.ValueScaleType = GetValueScaleType(series);
			ChartDataSource dataSource = new ChartDataSource(GetArgumentReference(series), series.Values);
			SpreadsheetChart.RegisterChartDataSource(dataSource);
			modelSeries.DataSource = dataSource;
		}
		protected ChartsModel.ValueScaleType GetValueScaleType(ISeries series) {
			ChartsModel.ValueScaleType valueScaleType;
			switch (series.Values.ValueType) {
				case DataReferenceValueType.DateTime:
					valueScaleType = ChartsModel.ValueScaleType.DateTime;
					break;
				case DataReferenceValueType.Number:
					valueScaleType = ChartsModel.ValueScaleType.Numerical;
					break;
				default:
					throw new ArgumentException("Invalid DataReferenceValueType: " + series.Values.ValueType.ToString());
			}
			return valueScaleType;
		}
		protected ChartsModel.ArgumentScaleType GetArgumentScaleType(ISeries series) {
			ChartsModel.ArgumentScaleType argumentScaleType;
			IDataReference arguments = GetArgumentReference(series);
			switch (arguments.ValueType) {
				case DataReferenceValueType.DateTime:
					argumentScaleType = ChartsModel.ArgumentScaleType.DateTime;
					break;
				case DataReferenceValueType.Number:
					argumentScaleType = IsXYSeries(series) ? ChartsModel.ArgumentScaleType.Numerical : ChartsModel.ArgumentScaleType.Qualitative;
					break;
				case DataReferenceValueType.String:
					argumentScaleType = ChartsModel.ArgumentScaleType.Auto;
					break;
				default:
					throw new ArgumentException("Invalid DataReferenceValueType: " + series.Arguments.ValueType.ToString());
			}
			return argumentScaleType;
		}
		protected bool IsXYSeries(ISeries series) {
			return series.SeriesType == ChartSeriesType.Bubble || series.SeriesType == ChartSeriesType.Scatter;
		}
		protected IDataReference GetArgumentReference(ISeries series) {
			IDataReference result = series.Arguments;
			if (result.Equals(DataReference.Empty)) {
				IChartView view = series.View;
				foreach (ISeries viewSeries in view.Series) {
					if (!viewSeries.Arguments.Equals(DataReference.Empty)) {
						result = viewSeries.Arguments;
						break;
					}
				}
			}
			ChartDataReference arguments = result as ChartDataReference;
			if (arguments != null && arguments.ValueType == DataReferenceValueType.String && IsXYSeries(series))
				result = DataReference.Empty;
			return result;
		}
		protected virtual void SetupSeriesShowInLegend(ChartsModel.SeriesModel modelSeries) {
			modelSeries.ShowInLegend = !IsDeletedFromLegend(ModelBuilder.SeriesIndex);
		}
		protected void SetupSeriesAxes(ChartsModel.CartesianSeriesBase modelSeries, ISeries series) {
			if (modelSeries == null || View.Axes != SpreadsheetChart.SecondaryAxes)
				return;
			if (View.AxesType == AxisGroupType.CategoryValue) {
				AxisBase axis = View.Axes.Find(AxisDataType.Agrument);
				if (axis != null && !axis.Delete)
					modelSeries.SecondaryArgumentAxisIndex = 0;
				axis = View.Axes.Find(AxisDataType.Value);
				if (axis != null && !axis.Delete)
					modelSeries.SecondaryValueAxisIndex = 0;
			}
			else if (View.AxesType == AxisGroupType.XY) {
				AxisBase axis = View.Axes.GetItem(0);
				if (axis != null && !axis.Delete)
					modelSeries.SecondaryArgumentAxisIndex = 0;
				axis = View.Axes.GetItem(1);
				if (axis != null && !axis.Delete)
					modelSeries.SecondaryValueAxisIndex = 0;
			}
		}
		protected virtual void SetupSeriesTransparency(ChartsModel.SeriesModel modelSeries, ISeries series) {
			ChartsModel.ISupportTransparencySeries seriesWithTransparency = modelSeries as ChartsModel.ISupportTransparencySeries;
			if (seriesWithTransparency != null) {
				IDrawingFill fill = ((SeriesBase)series).ShapeProperties.Fill;
				if (fill.FillType == DrawingFillType.None)
					seriesWithTransparency.Transparency = 255;
				else if (fill.FillType == DrawingFillType.Solid) {
					DrawingSolidFill solidFill = fill as DrawingSolidFill;
					seriesWithTransparency.Transparency = (byte)(255 - solidFill.Color.FinalColor.A);
				}
				else
					seriesWithTransparency.Transparency = 0;
			}
		}
		protected virtual void SetupSeriesMarker(ChartsModel.SeriesModel modelSeries, ISeries series) {
			ChartsModel.ISupportMarkerSeries markerSeries = modelSeries as ChartsModel.ISupportMarkerSeries;
			ISeriesWithMarker seriesWithMarker = series as ISeriesWithMarker;
			if (markerSeries != null && seriesWithMarker != null) {
				ChartsModel.Marker modelMarker = new ChartsModel.Marker(modelSeries);
				Marker marker = seriesWithMarker.Marker;
				modelMarker.Visible = (marker.Symbol != MarkerStyle.None) && 
					(marker.ShapeProperties.Fill.FillType != DrawingFillType.None || marker.ShapeProperties.Outline.Fill.FillType != DrawingFillType.None);
				RadarChartView radarView = series.View as RadarChartView;
				if (radarView != null)
					modelMarker.Visible = modelMarker.Visible && (radarView.RadarStyle != RadarChartStyle.Filled);
				modelMarker.MarkerType = GetMarkerType(series.Index, marker);
				modelMarker.Size = GetMarkerSize(marker);
				markerSeries.Marker = modelMarker;
			}
		}
		protected virtual void SetupSeriesVaryColors(ChartsModel.SeriesModel modelSeries) {
			ChartsModel.ISupportColorEachSeries colorEachSeries = modelSeries as ChartsModel.ISupportColorEachSeries;
			ChartViewWithVaryColors varyColorsView = View as ChartViewWithVaryColors;
			if (colorEachSeries == null || varyColorsView == null || View.Series.Count > 1)
				return;
			colorEachSeries.ColorEach = varyColorsView.VaryColors;
			if (varyColorsView.VaryColors)
				modelSeries.LegendPointPattern = "{A}";
		}
		protected virtual void SetupSeriesDataLabels(ChartsModel.SeriesModel modelSeries, SeriesWithDataLabelsAndPoints series) {
			ChartViewWithDataLabels chartView = View as ChartViewWithDataLabels;
			if (series == null || chartView == null)
				return;
			modelSeries.LabelsVisibility = DataLabelsHelper.IsDataLabelsVisible(chartView.DataLabels, series.DataLabels);
			ChartsModel.SeriesLabel label = new ChartsModel.SeriesLabel(modelSeries);
			label.Position = GetDataLabelsPosition(chartView.DataLabels, series.DataLabels);
			ChartsModel.IDataLabelFormatter formatter = series as ChartsModel.IDataLabelFormatter;
			if (formatter != null)
				label.Formatter = formatter;
			label.EnableAntialiasing = SpreadsheetChart.GetActualTextAntialiasing();
			modelSeries.Label = label;
		}
		ChartsModel.SeriesLabelPosition GetDataLabelsPosition(DataLabels viewDataLabels, DataLabels seriesDataLabels) {
			DataLabelPosition position = DataLabelsHelper.GetDataLabelsPosition(viewDataLabels, seriesDataLabels, View.DefaultDataLabelPosition);
			switch (position) {
				case DataLabelPosition.Left:
					return ChartsModel.SeriesLabelPosition.Left;
				case DataLabelPosition.Top:
					return ChartsModel.SeriesLabelPosition.Top;
				case DataLabelPosition.Right:
					return ChartsModel.SeriesLabelPosition.Righ;
				case DataLabelPosition.Bottom:
					return ChartsModel.SeriesLabelPosition.Bottom;
				case DataLabelPosition.Center:
					return ChartsModel.SeriesLabelPosition.Center;
				case DataLabelPosition.BestFit:
					return ChartsModel.SeriesLabelPosition.BestFit;
				case DataLabelPosition.InsideBase:
					return ChartsModel.SeriesLabelPosition.InsideBase;
				case DataLabelPosition.InsideEnd:
					return ChartsModel.SeriesLabelPosition.InsideEnd;
				case DataLabelPosition.OutsideEnd:
					return ChartsModel.SeriesLabelPosition.OutsideEnd;
			}
			return ChartsModel.SeriesLabelPosition.Center;
		}
		protected bool IsDeletedFromLegend(int index) {
			LegendEntry legendEntry = SpreadsheetChart.Legend.Entries.FindByIndex(index);
			if (legendEntry == null)
				return false;
			return legendEntry.Delete;
		}
		string GetDisplayName(ISeries series) {
			string result = series.GetSeriesText();
			if (string.IsNullOrEmpty(result))
				result = " ";
			return result;
		}
		protected ChartsModel.MarkerType GetMarkerType(int seriesIndex, Marker marker) {
			switch (marker.Symbol) {
				case MarkerStyle.Diamond:
					return ChartsModel.MarkerType.Diamond;
				case MarkerStyle.Square:
					return ChartsModel.MarkerType.Square;
				case MarkerStyle.Triangle:
					return ChartsModel.MarkerType.Triangle;
				case MarkerStyle.X:
					return ChartsModel.MarkerType.Cross;
				case MarkerStyle.Star:
					return ChartsModel.MarkerType.Star;
				case MarkerStyle.Circle:
					return ChartsModel.MarkerType.Circle;
				case MarkerStyle.Plus:
					return ChartsModel.MarkerType.Plus;
				case MarkerStyle.Dot:
					return ChartsModel.MarkerType.Pentagon; 
				case MarkerStyle.Dash:
					return ChartsModel.MarkerType.Hexagon; 
				case MarkerStyle.Auto:
					int markerIndex = seriesIndex % 9;
					switch (markerIndex) {
						case 0:
							return ChartsModel.MarkerType.Diamond;
						case 1:
							return ChartsModel.MarkerType.Square;
						case 2:
							return ChartsModel.MarkerType.Triangle;
						case 3:
							return ChartsModel.MarkerType.Cross;
						case 4:
							return ChartsModel.MarkerType.Star;
						case 5:
							return ChartsModel.MarkerType.Circle;
						case 6:
							return ChartsModel.MarkerType.Plus;
						case 7:
							return ChartsModel.MarkerType.Pentagon; 
						case 8:
							return ChartsModel.MarkerType.Hexagon; 
					}
					break;
			}
			return ChartsModel.MarkerType.InvertedTriangle;
		}
		protected int GetMarkerSize(Marker marker) {
			DocumentModel documentModel = marker.DocumentModel;
			return documentModel.LayoutUnitConverter.LayoutUnitsToPixels(
				documentModel.LayoutUnitConverter.PointsToLayoutUnits(marker.Size),
				DocumentModel.Dpi);
		}
	}
	#endregion
	#region ModelViewBuilderFactory
	class ModelViewBuilderFactory : IChartViewVisitor {
		ModelChartBuilder modelBuilder;
		IModelViewBuilder viewBuilder;
		public ModelViewBuilderFactory(ModelChartBuilder modelBuilder) {
			this.modelBuilder = modelBuilder;
		}
		public IModelViewBuilder CreateViewBuilder(IChartView view) {
			viewBuilder = null;
			view.Visit(this);
			return viewBuilder;
		}
		#region IChartViewVisitor Members
		public void Visit(Area3DChartView view) {
			viewBuilder = new Area3DChartViewBuilder(modelBuilder, view);
		}
		public void Visit(AreaChartView view) {
			viewBuilder = new AreaChartViewBuilder(modelBuilder, view);
		}
		public void Visit(Bar3DChartView view) {
			viewBuilder = new Bar3DChartViewBuilder(modelBuilder, view);
		}
		public void Visit(BarChartView view) {
			viewBuilder = new BarChartViewBuilder(modelBuilder, view);
		}
		public void Visit(BubbleChartView view) {
			viewBuilder = new BubbleChartViewBuilder(modelBuilder, view);
		}
		public void Visit(DoughnutChartView view) {
			viewBuilder = new DoughnutChartViewBuilder(modelBuilder, view);
		}
		public void Visit(Line3DChartView view) {
			viewBuilder = new Line3DChartViewBuilder(modelBuilder, view);
		}
		public void Visit(LineChartView view) {
			viewBuilder = new LineChartViewBuilder(modelBuilder, view);
		}
		public void Visit(OfPieChartView view) {
		}
		public void Visit(Pie3DChartView view) {
			viewBuilder = new Pie3DChartViewBuilder(modelBuilder, view);
		}
		public void Visit(PieChartView view) {
			viewBuilder = new PieChartViewBuilder(modelBuilder, view);
		}
		public void Visit(RadarChartView view) {
			viewBuilder = new RadarChartViewBuilder(modelBuilder, view);
		}
		public void Visit(ScatterChartView view) {
			viewBuilder = new ScatterChartViewBuilder(modelBuilder, view);
		}
		public void Visit(StockChartView view) {
			viewBuilder = new StockChartViewBuilder(modelBuilder, view);
		}
		public void Visit(Surface3DChartView view) {
		}
		public void Visit(SurfaceChartView view) {
		}
		#endregion
	}
	#endregion
}
