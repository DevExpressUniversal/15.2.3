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
	public class StockChartViewBuilder : ModelViewBuilderBase {
		public StockChartViewBuilder(ModelChartBuilder modelBuilder, StockChartView view)
			: base(modelBuilder, view) {
		}
		protected new StockChartView View { get { return base.View as StockChartView; } }
		protected override ChartsModel.SeriesModel CreateModelSeries(ISeries series) {
			if ((!IsFirstSeries && SpreadsheetChart.Views.Count == 1) || (SeriesIndex > 1 && SpreadsheetChart.Views.Count == 2))
				return null;
			if (View.Series.Count == 3)
				return new ChartsModel.StockSeries();
			if (View.Series.Count == 4)
				return new ChartsModel.CandleStickSeries();
			return null;
		}
		protected override void SetupSeriesDataMembers(ChartsModel.SeriesModel modelSeries, ISeries series) {
			modelSeries.DataMembers[ChartsModel.DataMemberType.Argument] = "Argument";
			modelSeries.DataMembers[ChartsModel.DataMemberType.Value] = "Value0";
			modelSeries.DataMembers[ChartsModel.DataMemberType.ExtValue0] = "Value1";
			modelSeries.DataMembers[ChartsModel.DataMemberType.ExtValue1] = "Value2";
			modelSeries.DataMembers[ChartsModel.DataMemberType.ExtValue2] = "Value3";
			modelSeries.ArgumentScaleType = GetArgumentScaleType(series);
			modelSeries.ValueScaleType = GetValueScaleType(series);
			SeriesCollection viewSeries = View.Series;
			int count = viewSeries.Count;
			ISeries seriesOpen = count == 3 ? viewSeries[2] : viewSeries[0];
			ISeries seriesHigh = count == 3 ? viewSeries[0] : viewSeries[1];
			ISeries seriesLow = count == 3 ? viewSeries[1] : viewSeries[2];
			ISeries seriesClose = count == 3 ? viewSeries[2] : viewSeries[3];
			ChartDataSource dataSource = new ChartDataSource(series.Arguments, seriesLow.Values, seriesHigh.Values, seriesOpen.Values, seriesClose.Values);
			SpreadsheetChart.RegisterChartDataSource(dataSource);
			modelSeries.DataSource = dataSource;
		}
	}
}
