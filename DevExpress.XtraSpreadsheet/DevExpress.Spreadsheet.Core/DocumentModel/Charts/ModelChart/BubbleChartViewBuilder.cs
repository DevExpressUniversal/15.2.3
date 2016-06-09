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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.Model.ModelChart {
	public class BubbleChartViewBuilder : ModelViewBuilderBase {
		double maxBubbleSize;
		double minArgValue;
		double maxArgValue;
		double minBubbleValue;
		double maxBubbleValue;
		double argumentRange;
		double shift;
		public BubbleChartViewBuilder(ModelChartBuilder modelBuilder, BubbleChartView view)
			: base(modelBuilder, view) {
			Calculate();
		}
		protected new BubbleChartView View { get { return base.View as BubbleChartView; } }
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			return new ChartsModel.BubbleSeries();
		}
		protected override void SetupSeriesDataMembers(ChartsModel.SeriesModel modelSeries, ISeries series) {
			modelSeries.DataMembers[ChartsModel.DataMemberType.Argument] = "Argument";
			modelSeries.DataMembers[ChartsModel.DataMemberType.Value] = "Value0";
			modelSeries.DataMembers[ChartsModel.DataMemberType.ExtValue0] = "Value1";
			modelSeries.ArgumentScaleType = GetArgumentScaleType(series);
			modelSeries.ValueScaleType = GetValueScaleType(series);
			IDataReference seriesArguments = series.Arguments;
			ChartDataReference arguments = seriesArguments as ChartDataReference;
			if (arguments != null && arguments.ValueType == DataReferenceValueType.String)
				seriesArguments = DataReference.Empty;
			ChartDataSource dataSource = new ChartDataSource(seriesArguments, series.Values, ((BubbleSeries)series).BubbleSize);
			SpreadsheetChart.RegisterChartDataSource(dataSource);
			modelSeries.DataSource = dataSource;
		}
		protected override void SetupSeries(ChartsModel.SeriesModel modelSeries, ISeries series) {
			base.SetupSeries(modelSeries, series);
			ChartsModel.BubbleSeries bubbleSeries = (ChartsModel.BubbleSeries)modelSeries;
			double bubbleScale = View.BubbleScale / 100.0;
			minBubbleValue = double.NaN;
			maxBubbleValue = double.NaN;
			CalculateMinMaxBubbleValues(series);
			if (double.IsNaN(minBubbleValue) || minBubbleValue == maxBubbleValue) {
				minBubbleValue = 0.0;
				maxBubbleValue = 1.0;
			}
			double scale = argumentRange * bubbleScale / (maxBubbleSize * 5.0);
			double maxSize = (maxBubbleValue + shift) * scale;
			double minSize = (minBubbleValue + shift) * scale;
			minSize = Math.Max(minSize, maxSize / 5.0);
			if (maxSize <= bubbleSeries.MinSize) {
				bubbleSeries.MinSize = minSize;
				bubbleSeries.MaxSize = maxSize;
			}
			else {
				bubbleSeries.MaxSize = maxSize;
				bubbleSeries.MinSize = minSize;
			}
		}
		void Calculate() {
			minArgValue = double.NaN;
			maxArgValue = double.NaN;
			minBubbleValue = double.NaN;
			maxBubbleValue = double.NaN;
			foreach (ISeries series in View.Series) {
				CalculateMinMaxArguments(series);
				CalculateMinMaxBubbleValues(series);
			}
			CalculateArgumentRange();
			CalculateMaxBubbleSize();
		}
		void CalculateMinMaxArguments(ISeries series) {
			ChartDataReference arguments = GetArgumentReference(series) as ChartDataReference;
			if (arguments == null) {
				SetMinValue(ref minArgValue, 1.0);
				SetMaxValue(ref maxArgValue, series.Values.ValuesCount);
			}
			else {
				for (int i = 0; i < arguments.ValuesCount; i++) {
					SetMinValue(ref minArgValue, (double)arguments[i]);
					SetMaxValue(ref maxArgValue, (double)arguments[i]);
				}
			}
		}
		void CalculateMinMaxBubbleValues(ISeries series) {
			ChartDataReference bubbleSize = ((BubbleSeries)series).BubbleSize as ChartDataReference;
			if (bubbleSize == null) {
				SetMinValue(ref minBubbleValue, 1.0);
				SetMaxValue(ref maxBubbleValue, series.Values.ValuesCount);
			}
			else {
				for (int i = 0; i < bubbleSize.ValuesCount; i++) {
					SetMinValue(ref minBubbleValue, (double)bubbleSize[i]);
					SetMaxValue(ref maxBubbleValue, (double)bubbleSize[i]);
				}
			}
		}
		void CalculateArgumentRange() {
			if (double.IsNaN(minArgValue) || minArgValue == maxArgValue) {
				minArgValue = 0.0;
				maxArgValue = 1.0;
			}
			argumentRange = maxArgValue - minArgValue;
		}
		void CalculateMaxBubbleSize() {
			if (double.IsNaN(minBubbleValue) || minBubbleValue == maxBubbleValue) {
				minBubbleValue = 0.0;
				maxBubbleValue = 1.0;
			}
			if (minBubbleValue == 0.0)
				shift = 1.0;
			else if (minBubbleValue < 0.0)
				shift = 1.0 - minBubbleValue;
			else
				shift = 0.0;
			maxBubbleSize = maxBubbleValue + shift;
		}
		void SetMinValue(ref double targetValue, double value) {
			if (double.IsNaN(targetValue))
				targetValue = value;
			else
				targetValue = Math.Min(targetValue, value);
		}
		void SetMaxValue(ref double targetValue, double value) {
			if (double.IsNaN(targetValue))
				targetValue = value;
			else
				targetValue = Math.Max(targetValue, value);
		}
	}
}
