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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region RemoveSeriesCommand
	public class RemoveSeriesCommand : ErrorHandledWorksheetCommand {
		readonly ISeries series;
		public RemoveSeriesCommand(IErrorHandler errorHandler, ISeries series)
			: base(series.View.Parent.DocumentModel, errorHandler) {
			this.series = series;
		}
		#region Properties
		IChartView View { get { return series.View; } }
		SeriesCollection SeriesCollection { get { return series.View.Series; } }
		#endregion
		protected internal override void ExecuteCore() {
			SwitchAxesHelper.ClearSecondaryAxes(View);
			SeriesCollection.Remove(series);
			RemoveView();
			ChangeSeriesOrders();
		}
		void RemoveView() {
			ChartViewCollection views = View.Parent.Views;
			if (views.Count > 1 && View.Series.Count == 0)
				views.Remove(View);
		}
		void ChangeSeriesOrders() {
			if (series.Order < SeriesCollection.Count - 1)
				ChangeAllSeriesOrdersCommand.Execute(ErrorHandler, View.Parent);
		}
		protected internal override bool Validate() {
			return View.IsContained && series.IsContained;
		}
	}
	#endregion
	#region ClearViewSeriesCommand
	public class ClearViewSeriesCommand : ErrorHandledWorksheetCommand {
		readonly IChartView view;
		public ClearViewSeriesCommand(IErrorHandler errorHandler, IChartView view)
			: base(view.Parent.DocumentModel, errorHandler) {
			this.view = view;
		}
		protected internal override void ExecuteCore() {
			ChartViewCollection views = view.Parent.Views;
			int index = view.IndexOfView;
			if (index == 0)
				view.Series.Clear();
			else {
				SwitchAxesHelper.ClearSecondaryAxes(view);
				views.RemoveAt(index);
			}
			ChangeAllSeriesOrdersCommand.Execute(ErrorHandler, view.Parent);
		}
		protected internal override bool Validate() {
			return view.IsContained && view.Series.Count > 0;
		}
	}
	#endregion
	#region ClearAllSeriesCommand
	public class ClearAllSeriesCommand : ErrorHandledWorksheetCommand {
		#region Static Member
		internal static void Execute(IErrorHandler errorHandler, IChart chart) {
			ClearAllSeriesCommand command = new ClearAllSeriesCommand(errorHandler, chart);
			command.Execute();
		}
		#endregion
		readonly IChart chart;
		public ClearAllSeriesCommand(IErrorHandler errorHandler, IChart chart)
			: base(chart.DocumentModel, errorHandler) {
			this.chart = chart;
		}
		ChartViewCollection Views { get { return chart.Views; } }
		protected internal override void ExecuteCore() {
			int count = Views.Count;
			for (int i = 1; i < count; i++)
				SwitchAxesHelper.ClearSecondaryAxes(Views[i]);
			Views.ClearLastViews(1);
			Views[0].Series.Clear();
		}
		protected internal override bool Validate() {
			if (Views.Count == 0)
				return false;
			return Views.Count > 1 || Views[0].Series.Count > 0;
		}
	}
	#endregion
	#region ChangeAllSeriesOrdersCommand
	public class ChangeAllSeriesOrdersCommand : ErrorHandledWorksheetCommand {
		#region Static Member
		internal static void Execute(IErrorHandler errorHandler, IChart chart) {
			ChangeAllSeriesOrdersCommand command = new ChangeAllSeriesOrdersCommand(errorHandler, chart);
			command.Execute();
		}
		#endregion
		#region Fields
		readonly IChart chart;
		IList<ISeries> series;
		#endregion
		public ChangeAllSeriesOrdersCommand(IErrorHandler errorHandler, IChart chart)
			: base(chart.DocumentModel, errorHandler) {
			this.chart = chart;
		}
		protected internal override void ExecuteCore() {
			int count = series.Count;
			for (int i = 0; i < count; i++)
				series[i].Order = i;
		}
		protected internal override bool Validate() {
			Chart chart = this.chart as Chart;
			if (chart == null)
				return false;
			series = chart.GetSeriesList();
			return series.Count != 0;
		}
	}
	#endregion
}
