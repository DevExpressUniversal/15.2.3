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

extern alias Platform;
using System;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class SelectDataMemberCommandBase : SeriesOptionsCommandBase {
		public override string ImageName {
			get { return null; }
		}
		public SelectDataMemberCommandBase(WpfChartModel model)
			: base(model) {
		}
		ScaleType ScaleTypeCoreToScaleType(Scale scaleTypeCore) {
			switch (scaleTypeCore) {
				case Scale.Auto:
					return ScaleType.Auto;
				case Scale.DateTime:
					return ScaleType.DateTime;
				case Scale.Numerical:
					return ScaleType.Numerical;
				case Scale.Qualitative:
					return ScaleType.Qualitative;
			}
			return ScaleType.Auto;
		}
		protected bool AllDataMembersEmpty() {
			return String.IsNullOrEmpty(SeriesModel.ArgumentDataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.ValueDataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.Value2DataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.WeightDataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.LowValueDataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.HighValueDataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.OpenValueDataMember.DataMember) &&
				String.IsNullOrEmpty(SeriesModel.CloseValueDataMember.DataMember);
		}
		protected bool AllDataMembersSet() {
			bool valueDataMembersSet = false;
			Series series = SeriesModel.Series;
			if (series is FinancialSeries2D) {
				FinancialSeries2D financialSeries = series as FinancialSeries2D;
				valueDataMembersSet = !String.IsNullOrEmpty(financialSeries.LowValueDataMember) && !String.IsNullOrEmpty(financialSeries.HighValueDataMember) && !String.IsNullOrEmpty(financialSeries.OpenValueDataMember) && !String.IsNullOrEmpty(financialSeries.CloseValueDataMember);
			}
			else if (series is BubbleSeries2D) {
				BubbleSeries2D bubbleSeries2D = series as BubbleSeries2D;
				valueDataMembersSet = !String.IsNullOrEmpty(bubbleSeries2D.ValueDataMember) && !String.IsNullOrEmpty(bubbleSeries2D.WeightDataMember);
			}
			else if (series is BubbleSeries3D) {
				BubbleSeries3D bubbleSeries3D = series as BubbleSeries3D;
				valueDataMembersSet = !String.IsNullOrEmpty(bubbleSeries3D.ValueDataMember) && !String.IsNullOrEmpty(bubbleSeries3D.WeightDataMember);
			}
			else if (series is RangeBarSeries2D) {
				RangeBarSeries2D rangeBarSeries = series as RangeBarSeries2D;
				valueDataMembersSet = !String.IsNullOrEmpty(rangeBarSeries.ValueDataMember) && !String.IsNullOrEmpty(rangeBarSeries.Value2DataMember);
			}
			else if (series is RangeAreaSeries2D) {
				RangeAreaSeries2D rangeAreaSeries = series as RangeAreaSeries2D;
				valueDataMembersSet = !String.IsNullOrEmpty(rangeAreaSeries.ValueDataMember) && !String.IsNullOrEmpty(rangeAreaSeries.Value2DataMember);
			}
			else
				valueDataMembersSet = !String.IsNullOrEmpty(series.ValueDataMember);
			return !String.IsNullOrEmpty(SeriesModel.ArgumentDataMember.DataMember) && !String.IsNullOrEmpty(SeriesModel.Diagram.SeriesDataMember.DataMember) && valueDataMembersSet;
		}
		protected void ChangeValueScaleType(string dataMember) {
			ScaleType newScaleType = ScaleTypeCoreToScaleType(BindingHelperCore.GetScaleType(SeriesModel.DataSource, dataMember));
			if (newScaleType != SeriesModel.ValueScaleType)
				SeriesModel.ValueScaleType = newScaleType;
		}
		protected void ChangeValueScaleType(string dataMember, CompositeHistoryItem historyItem) {
			ScaleType newScaleType = ScaleTypeCoreToScaleType(BindingHelperCore.GetScaleType(SeriesModel.DataSource, dataMember));
			if (newScaleType != SeriesModel.ValueScaleType) {
				ChangeSeriesValueScaleTypeCommand valueScaleTypeCommand = new ChangeSeriesValueScaleTypeCommand(ChartModel);
				historyItem.HistoryItems.Add(valueScaleTypeCommand.RuntimeExecute(newScaleType).HistoryItem);
			}
		}
		protected void CopyDataMembers(WpfChartSeriesModel sourceSeries, CompositeHistoryItem historyItem) {
			SelectArgumentDataMemberCommand argumentCommand = new SelectArgumentDataMemberCommand(ChartModel);
			CommandResult result = argumentCommand.RuntimeExecute(sourceSeries.ArgumentDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectValueDataMemberCommand valueCommand = new SelectValueDataMemberCommand(ChartModel);
			result = valueCommand.RuntimeExecute(sourceSeries.ValueDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectWeightDataMemberCommand weightCommand = new SelectWeightDataMemberCommand(ChartModel);
			result = weightCommand.RuntimeExecute(sourceSeries.WeightDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectValue2DataMemberCommand value2Command = new SelectValue2DataMemberCommand(ChartModel);
			result = value2Command.RuntimeExecute(sourceSeries.Value2DataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectLowValueDataMemberCommand lowValueCommand = new SelectLowValueDataMemberCommand(ChartModel);
			result = lowValueCommand.RuntimeExecute(sourceSeries.LowValueDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectHighValueDataMemberCommand highValueCommand = new SelectHighValueDataMemberCommand(ChartModel);
			result = highValueCommand.RuntimeExecute(sourceSeries.HighValueDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectOpenValueDataMemberCommand openValueCommand = new SelectOpenValueDataMemberCommand(ChartModel);
			result = openValueCommand.RuntimeExecute(sourceSeries.OpenValueDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
			SelectCloseValueDataMemberCommand closeValueCommand = new SelectCloseValueDataMemberCommand(ChartModel);
			result = closeValueCommand.RuntimeExecute(sourceSeries.CloseValueDataMember);
			if (result != null) {
				historyItem.HistoryItems.Add(result.HistoryItem);
				ChartModel.SelectedObject = result.SelectedChartObject;
			}
		}
		protected void ReplaceSeriesBySeriesTempalte(CompositeHistoryItem historyItem) {
			WpfChartSeriesModel sourceSeries = SeriesModel;
			RemoveSeriesCommand removeCommand = new RemoveSeriesCommand(ChartModel);
			historyItem.HistoryItems.Add(removeCommand.RuntimeExecute(sourceSeries.Series).HistoryItem);
			CreateSeriesTemplateCommand createTemplate = new CreateSeriesTemplateCommand(ChartModel);
			CommandResult createTemplateResult = createTemplate.RuntimeExecute(null);
			historyItem.HistoryItems.Add(createTemplateResult.HistoryItem);
			ChartModel.RecursivelyUpdateChildren();
			ChartModel.SelectedObject = createTemplateResult.SelectedChartObject;
			ChangeSeriesNameCommand nameCommand = new ChangeSeriesNameCommand(ChartModel);
			historyItem.HistoryItems.Add(nameCommand.RuntimeExecute(sourceSeries.Name).HistoryItem);
			ChangeSeriesValueScaleTypeCommand scaleTypeCommand = new ChangeSeriesValueScaleTypeCommand(ChartModel);
			historyItem.HistoryItems.Add(scaleTypeCommand.RuntimeExecute(sourceSeries.ValueScaleType).HistoryItem);
			CopyDataMembers(sourceSeries, historyItem);
		}
		protected void ReplaceSeriesTempalteBySeries(CompositeHistoryItem historyItem) {
			AddSeriesCommand addCommand = new AddSeriesCommand(ChartModel, SeriesModel.Series.GetType());
			CommandResult addResult = addCommand.RuntimeExecute(null);
			historyItem.HistoryItems.Add(addResult.HistoryItem);
			WpfChartSeriesModel sourceSeries = SeriesModel;
			ChartModel.RecursivelyUpdateChildren();
			ChartModel.SelectedObject = addResult.SelectedChartObject;
			ChangeSeriesNameCommand nameCommand = new ChangeSeriesNameCommand(ChartModel);
			historyItem.HistoryItems.Add(nameCommand.RuntimeExecute(sourceSeries.Name).HistoryItem);
			ChangeSeriesValueScaleTypeCommand scaleTypeCommand = new ChangeSeriesValueScaleTypeCommand(ChartModel);
			historyItem.HistoryItems.Add(scaleTypeCommand.RuntimeExecute(sourceSeries.ValueScaleType).HistoryItem);
			CopyDataMembers(sourceSeries, historyItem);
			SelectChartDataSourceCommand chartDataSource = new SelectChartDataSourceCommand(ChartModel);
			historyItem.HistoryItems.Add(chartDataSource.RuntimeExecute(true).HistoryItem);
			ChartModel.SelectedObject = addResult.SelectedChartObject;
			if (!String.IsNullOrEmpty(ChartModel.DiagramModel.SeriesDataMember.DataMember)) {
				ChangeIsSeriesTemplatePreviewCommand seriesTemplatePreview = new ChangeIsSeriesTemplatePreviewCommand(ChartModel);
				historyItem.HistoryItems.Add(seriesTemplatePreview.RuntimeExecute(true).HistoryItem);
			}
			RemoveSeriesTemplateCommand removeTemplateCommand = new RemoveSeriesTemplateCommand(ChartModel);
			historyItem.HistoryItems.Add(removeTemplateCommand.RuntimeExecute(null).HistoryItem);
		}
		protected bool ShouldReplaceSeriesBySeriesTemplate() {
			return ChartModel.DiagramModel.SeriesTemplateModel == null && AllDataMembersSet();
		}
		protected abstract void DataMemberUndoInternal(WpfChartSeriesModel model, object oldValue);
		protected abstract void DataMemberRedoInternal(WpfChartSeriesModel model, object oldValue);
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			DataMemberRedoInternal(model, newValue);
			ChartModel.SelectedObject = PreviewChart;
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			DataMemberUndoInternal(model, oldValue);
			ChartModel.SelectedObject = PreviewChart;
		}
		public abstract string GetCommandDataMember();
		public abstract CommandResult DataMemberRuntimeExecute(object parameter);
		public virtual ScaleType GetCommandScaleType() {
			return SeriesModel.ValueScaleType;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CommandResult result = DataMemberRuntimeExecute(parameter);
			if (result != null)
				ChartModel.SelectedObject = PreviewChart;
			return result;
		}
	}
}
