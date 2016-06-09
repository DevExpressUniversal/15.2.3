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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartTrendlineCommandGroupBase (abstract class)
	public abstract class ChartTrendlineCommandGroupBase : ChartCommandGroupBase {
		protected ChartTrendlineCommandGroupBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool CanModifyChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return false;
			foreach (IChartView view in chart.Views)
				if (!CanModifyView(view))
					return false;
			return true;
		}
		protected virtual bool CanModifyView(IChartView view) {
			if (view.Series.Count <= 0)
				return false;
			foreach (ISeries series in view.Series)
				if (!CanModifySeries(series))
					return false;
			return true;
		}
		protected virtual bool CanModifySeries(ISeries series) {
			return series is SeriesWithErrorBarsAndTrendlines;
		}
	}
	#endregion
	#region ChartModifyViewSeriesCommandBase
	public abstract class ChartModifyViewSeriesCommandBase : ChartModifyViewCommandBase {
		protected ChartModifyViewSeriesCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool CanModifyView(IChartView view) {
			if (view.Series.Count <= 0)
				return false;
			foreach (ISeries series in view.Series)
				if (!CanModifySeries(series))
					return false;
			return true;
		}
		protected override void ModifyView(IChartView view) {
			foreach (ISeries series in view.Series)
				ModifySeries(series);
		}
		protected abstract bool CanModifySeries(ISeries series);
		protected abstract void ModifySeries(ISeries series);
	}
	#endregion
	#region ChartModifyTrendlineCommandBase (abstract class)
	public abstract class ChartModifyTrendlineCommandBase : ChartModifyViewSeriesCommandBase {
		protected ChartModifyTrendlineCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool CanModifySeries(ISeries series) {
			return series is SeriesWithErrorBarsAndTrendlines;
		}
		protected override void ModifySeries(ISeries series) {
			SeriesWithErrorBarsAndTrendlines trendlineSeries = series as SeriesWithErrorBarsAndTrendlines;
			if (trendlineSeries == null)
				return;
		}
	}
	#endregion
}
