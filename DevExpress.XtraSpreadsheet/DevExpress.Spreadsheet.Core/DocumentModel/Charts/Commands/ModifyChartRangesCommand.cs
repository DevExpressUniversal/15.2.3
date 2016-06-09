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
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IChartSelectDataInfo
	public interface IChartSelectDataInfo {
		Chart Chart { get; set; }
		string Reference { get; set; }
	}
	public class ChartSelectDataInfo : IChartSelectDataInfo {
		public ChartSelectDataInfo(Chart chart, string reference) {
			Chart = chart;
			Reference = reference;
		}
		#region IChartSelectDataInfo Members
		public Chart Chart { get; set; }
		public string Reference { get; set; }
		#endregion
	}
	#endregion
	#region SeriesBuilder
	public class SeriesBuilder {
		protected internal ChartSeriesRangeModelBase Model { get; set; }
		protected internal void ModifySeries(ModifyChartViewRangesWalker walker) {
			IChartView currentView = walker.CurrentView;
			SeriesCollection series = currentView.Series;
			int lastSeriesIndex = walker.LastSeriesIndex;
			int newSeriesCount = Model.GetNewSeriesCount(lastSeriesIndex, walker.SeriesRange);
			int oldSeriesCount = series.Count;
			int replaceSeriesRangeCount = Math.Min(oldSeriesCount, newSeriesCount);
			ReplaceSeriesRange(series, lastSeriesIndex, replaceSeriesRangeCount, walker);
			lastSeriesIndex += replaceSeriesRangeCount;
			walker.RemoveLastViews = newSeriesCount <= oldSeriesCount;
			walker.LastSeriesIndex = lastSeriesIndex;
			if (newSeriesCount < oldSeriesCount) {
				RemoveLastViewSeries(series, oldSeriesCount - newSeriesCount);
				SwitchAxesHelper.ClearSecondaryAxes(currentView);
			}
			if (walker.HasLastCurrentView) {
				AddNewSeries(currentView, lastSeriesIndex, newSeriesCount - oldSeriesCount, walker.MaxSeriesOrder);
				walker.AddLastSeries = true;
			}
		}
		void AddSeries(IChartView view, ISeries series, int index, int order) {
			series.Index = index;
			series.Order = order;
			view.Series.Add(series);
		}
		ISeries CreateSeries(int seriesValueIndex, IChartView view) {
			ISeries series = view.CreateSeriesInstance();
			Model.ApplyData(series, seriesValueIndex);
			return series;
		}
		void ReplaceSeriesRange(SeriesCollection seriesCollection, int beginSeriesRangeIndex, int seriesCount, ModifyChartViewRangesWalker walker) {
			int maxOrder = 0;
			for (int i = 0; i < seriesCount; i++) {
				ISeries series = seriesCollection[i];
				Model.ApplyData(series, i + beginSeriesRangeIndex);
				int seriesOrder = series.Order;
				if (series.Order > maxOrder)
					maxOrder = seriesOrder; 
			}
			walker.MaxSeriesOrder = maxOrder;
		}
		void RemoveLastViewSeries(SeriesCollection series, int count) {
			for (int i = 0; i < count; i++)
				series.RemoveAt(series.Count - 1);
		}
		void AddNewSeries(IChartView view, int beginIndex, int newSeriesCount, int maxSeriesOrder) {
			int order = 1;
			for (int i = beginIndex; i < newSeriesCount + beginIndex; i++, order++) {
				ISeries series = CreateSeries(i, view);
				AddSeries(view, series, i, maxSeriesOrder + order);
			}
		}
	}
	#endregion
	#region ModifyChartViewRangesWalker
	public class ModifyChartViewRangesWalker {
		static SeriesBuilder builder = new SeriesBuilder();
		#region Fields
		CellRange seriesRange;
		IChartView currentView;
		int currentViewIndex;
		#endregion
		#region Properties
		protected internal IChartView CurrentView { get { return currentView; } }
		protected internal CellRange SeriesRange { get { return seriesRange; } }
		protected internal bool RemoveLastViews { get; set; }
		protected internal bool AddLastSeries { get; set; }
		protected internal int LastSeriesIndex { get; set; }
		protected internal int MaxSeriesOrder { get; set; }
		protected internal bool HasLastCurrentView { get { return currentViewIndex == currentView.Parent.Views.Count - 1; } }
		#endregion
		protected internal void Walk(IChart chart, CellRange seriesRange, ChartSeriesRangeModelBase seriesModel) {
			Guard.ArgumentNotNull(chart, "chart");
			Guard.ArgumentNotNull(seriesRange, "seriesRange");
			Guard.ArgumentNotNull(seriesModel, "seriesModel");
			Initialize(chart, seriesRange, seriesModel);
			WalkCore(chart.Views);
			this.seriesRange = null;
		}
		#region Internal
		void Initialize(IChart chart, CellRange seriesRange, ChartSeriesRangeModelBase seriesModel) {
			this.seriesRange = seriesRange;
			chart.SeriesDirection = seriesModel.Direction;
			builder.Model = seriesModel;
			RemoveLastViews = false;
			AddLastSeries = false;
			currentViewIndex = 0;
			LastSeriesIndex = 0;
		}
		void WalkCore(ChartViewCollection views) {
			int viewCount = views.Count;
			for (int i = 0; i < viewCount; i++) {
				if (RemoveLastViews || AddLastSeries)
					break;
				SetCurrentView(views, i);
				ChartAxisHelper.CheckArgumentAxis(currentView, builder.Model.Data.SeriesArguments);
				builder.ModifySeries(this);
			}
			if (RemoveLastViews)
				views.ClearLastViews(currentViewIndex + 1);
		}
		void SetCurrentView(ChartViewCollection views, int index) {
			currentView = views[index];
			currentViewIndex = index;
		}
		#endregion
	}
	#endregion
	#region ModifyChartRangeFromDirectionCommand
	public class ModifyChartRangeFromDirectionCommand : ModifyChartRangesCommand {
		ChartViewSeriesDirection direction;
		public ModifyChartRangeFromDirectionCommand(IDocumentModelPart documentModelPart, ChartViewSeriesDirection direction, IErrorHandler errorHandler)
			: base(documentModelPart, errorHandler) {
			this.direction = direction;
		}
		protected override ChartSeriesRangeModelBase CreateSeriesModel(CellRange seriesRange, ChartViewType viewType) {
			return ChartRangesCalculator.CreateModel(DataRange, seriesRange, viewType, direction);
		}
	}
	#endregion
	#region ModifyChartRangesCommand
	public class ModifyChartRangesCommand : ErrorHandledWorksheetCommand, IChartSelectDataInfo {
		readonly static ModifyChartViewRangesWalker walker = new ModifyChartViewRangesWalker();
		#region Fields
		Chart chart;
		string reference;
		CellRange dataRange;
		#endregion
		public ModifyChartRangesCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler)
			: base(documentModelPart, errorHandler) {
		}
		#region Properties
		public CellRange DataRange { get { return dataRange; } set { dataRange = value; } }
		ChartViewCollection Views { get { return chart.Views; } }
		#region IChartSelectDataInfo Members
		public Chart Chart { get { return chart; } set { chart = value; } }
		public string Reference { get { return reference; } set { reference = value; } }
		#endregion
		#endregion
		protected internal override void ExecuteCore() {
			chart.BeginUpdate();
			try {
				if (dataRange == null)
					ClearAllSeriesCommand.Execute(ErrorHandler, chart);
				else
					SetRanges();
			}
			finally {
				chart.EndUpdate();
			}
		}
		#region Validation
		protected internal override bool Validate() {
			if (Views.Count == 0 || (Views.Count > 1 && Views[0].Series.Count == 0))
				return false;
			if (String.IsNullOrEmpty(reference))
				return true;
			CellRangeBase rangeBase = GetActualRange();
			IModelErrorInfo error = ValidateRange(rangeBase);
			dataRange = rangeBase as CellRange;
			return HandleError(error);
		}
		CellRangeBase GetActualRange() {
			DataContext.PushCurrentWorksheet(Worksheet);
			try {
				return CellRangeBase.TryParse(reference, DataContext);
			} finally {
				DataContext.PopCurrentWorksheet();
			}
		}
		IModelErrorInfo ValidateRange(CellRangeBase range) {
			if (range == null)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			if (range.RangeType == CellRangeType.UnionRange)
				return new ModelErrorInfo(ModelErrorType.UnionRangeNotAllowed);
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet != null && sheet.PivotTables.ContainsItemsInRange(range, true))
				return new ModelErrorInfo(ModelErrorType.ChartDataRangeIntersectPivotTable);
			return null;
		}
		#endregion
		#region SetRanges
		protected void SetRanges() {
			ChartViewType viewType = GetFirstViewType();
			CellRange seriesRange = ChartRangesCalculator.GetSeriesRange(dataRange, viewType);
			ChartSeriesRangeModelBase seriesModel = CreateSeriesModel(seriesRange, viewType);
			walker.Walk(chart, seriesRange, seriesModel);
			ChangeAllSeriesOrdersCommand.Execute(ErrorHandler, chart);
		}
		ChartViewType GetFirstViewType() {
			ChartViewType result = Views[0].ViewType;
			if (Views.Count == 2 && result == ChartViewType.Bar && Views[1].ViewType == ChartViewType.Stock)
				return ChartViewType.Stock;
			return result;
		}
		protected virtual ChartSeriesRangeModelBase CreateSeriesModel(CellRange seriesRange, ChartViewType viewType) {
			if (Views[0].Series.Count == 0)
				return ChartRangesCalculator.CreateModel(dataRange, seriesRange, viewType);
			return ChartRangesCalculator.CreateModel(dataRange, seriesRange, viewType, Chart.SeriesDirection);
		}
		#endregion
	}
	#endregion
}
