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
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class IndicatorCommandBase : ChartCommandBase {
		const string SeriesIndexName = "SeriesIndex";
		const string IndicatorIndexName = "IndicatorIndex";
		protected Indicator SelectedIndicator {
			get { return ChartModel.SelectedObject as Indicator; }
		}
		protected WpfChartIndicatorModel SelectedIndicatorModel {
			get { return ChartModel.SelectedModel as WpfChartIndicatorModel; }
		}
		public IndicatorCommandBase(WpfChartModel chartModel)
			: base(chartModel) { }
		Indicator GetIndicatorByElementIndexes(ChartControl chartControl, ExecuteCommandInfo executeCommandInfo) {
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexName];
			XYSeries2D xySeries2D = (XYSeries2D)chartControl.Diagram.Series[seriesIndex];
			int indicatorIndex = executeCommandInfo.IndexByNameDictionary[IndicatorIndexName];
			Indicator indicator = xySeries2D.Indicators[indicatorIndex];
			return indicator;
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.SelectedModel is WpfChartIndicatorModel;
		}
		protected ElementIndexItem[] GetElementIndexes() {
			XYSeries2D indicatorOwner = ChartDesignerPropertiesProvider.GetIndicatorOwner(SelectedIndicator);
			int seriesIndex = PreviewChart.Diagram.Series.IndexOf(indicatorOwner);
			int indicatorIndex = indicatorOwner.Indicators.IndexOf(SelectedIndicator);
			ElementIndexItem seriesIndexItem = new ElementIndexItem(SeriesIndexName, seriesIndex);
			ElementIndexItem indicatorIndexItem = new ElementIndexItem(IndicatorIndexName, indicatorIndex);
			ElementIndexItem[] result = new ElementIndexItem[] { seriesIndexItem, indicatorIndexItem };
			return result;
		}
		protected Indicator GetIndicatorForRuntimeApply(ChartControl chartControl, ExecuteCommandInfo executeCommandInfo) {
			return GetIndicatorByElementIndexes(chartControl, executeCommandInfo);
		}
		protected WpfChartIndicatorModel GetIndicatorModelForUndoRedo(ExecuteCommandInfo executeCommandInfo) {
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexName];
			var seriesModel = (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex];
			int indicatorIndex = executeCommandInfo.IndexByNameDictionary[IndicatorIndexName];
			var indicatorModel = (WpfChartIndicatorModel)seriesModel.IndicatorCollectionModel.ModelCollection[indicatorIndex];
			return indicatorModel;
		}
		protected Indicator GetIndicatorForUndoRedo(ExecuteCommandInfo executeCommandInfo) {
			return GetIndicatorByElementIndexes(PreviewChart, executeCommandInfo);
		}
		protected IModelItem GetIndicatorModelItemForDesignTimeApply(IModelItem chartModelItem, ExecuteCommandInfo executeCommandInfo) {
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexName];
			IModelItem xySeries2DModelItem = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex];
			int indicatorIndex = executeCommandInfo.IndexByNameDictionary[IndicatorIndexName];
			IModelItem indicatorModelItem = xySeries2DModelItem.Properties["Indicators"].Collection[indicatorIndex];
			return indicatorModelItem;
		}
	}
	public class ChangeIndicatorColorCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_Color); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeIndicatorColorCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			indicator.Brush = (Brush)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			indicator.Brush = (Brush)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicatorModelItem.Properties["Brush"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			indicator.Brush = (Brush)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			Brush newValue = new SolidColorBrush((Color)parameter);
			Brush oldValue = SelectedIndicatorModel.Brush;
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			SelectedIndicatorModel.Brush = newValue;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class AddIndicatorLineStyleCommand : IndicatorCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public AddIndicatorLineStyleCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			indicator.LineStyle = (LineStyle)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			indicator.LineStyle = (LineStyle)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			IModelItem lineStyle = chartModelItem.Context.CreateItem(typeof(LineStyle));
			indicatorModelItem.Properties["LineStyle"].SetValue(lineStyle);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			indicator.LineStyle = new LineStyle();
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] pathIndexItems = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, pathIndexItems);
			LineStyle newValue = new LineStyle();
			HistoryItem historyItem = new HistoryItem(execComInfo, this, SelectedIndicator.LineStyle, newValue);
			SelectedIndicator.LineStyle = newValue;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeIndicatorThicknessCommand : IndicatorCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_Thickness);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeIndicatorThicknessCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			indicator.LineStyle.Thickness = (int)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			indicator.LineStyle.Thickness = (int)historyItem.NewValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicatorModelItem.Properties["LineStyle"].Value.Properties["Thickness"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			indicator.LineStyle.Thickness = (int)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositHistoryItem = new CompositeHistoryItem();
			if (SelectedIndicator.LineStyle == null) {
				var command = new AddIndicatorLineStyleCommand(ChartModel);
				CommandResult addLineStyleResult = command.RuntimeExecute(parameter: null);
				compositHistoryItem.HistoryItems.Add(addLineStyleResult.HistoryItem);
			}
			int newValue = (int)parameter;
			int oldValue = SelectedIndicatorModel.Thickness;
			ElementIndexItem[] pathIndexItems = GetElementIndexes();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			HistoryItem changeThicknessHistoryItem = new HistoryItem(info, this, oldValue, newValue);
			compositHistoryItem.HistoryItems.Add(changeThicknessHistoryItem);
			SelectedIndicatorModel.Thickness = newValue;
			CommandResult result = new CommandResult(compositHistoryItem);
			return result;
		}
	}
	public class ChangeIndicatorLegendTextCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Inicator_LegendText); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeIndicatorLegendTextCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var stringAndBoolContainer = (StringAndBoolContainer)historyItem.NewValue;
			indicator.LegendText = stringAndBoolContainer.String;
			indicator.ShowInLegend = stringAndBoolContainer.Bool;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var stringAndBoolContainer = (StringAndBoolContainer)historyItem.OldValue;
			indicator.LegendText = stringAndBoolContainer.String;
			indicator.ShowInLegend = stringAndBoolContainer.Bool;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			var stringAndBoolContainer = (StringAndBoolContainer)historyItem.NewValue;
			indicatorModelItem.Properties["LegendText"].SetValue(stringAndBoolContainer.String);
			indicatorModelItem.Properties["ShowInLegend"].SetValue(stringAndBoolContainer.Bool);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			var stringAndBoolContainer = (StringAndBoolContainer)historyItem.NewValue;
			indicator.LegendText = stringAndBoolContainer.String;
			indicator.ShowInLegend = stringAndBoolContainer.Bool;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			var newValue = new StringAndBoolContainer((string)parameter, parameter == null ? false : true);
			var oldValue = new StringAndBoolContainer(SelectedIndicatorModel.LegendText, SelectedIndicatorModel.ShowInLegend);
			SelectedIndicatorModel.LegendText = newValue.String;
			SelectedIndicatorModel.ShowInLegend = newValue.Bool;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			return new CommandResult(historyItem);
		}
	}
	public class ChangeRegressionLineValueLevelCommand : IndicatorCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeRegressionLineValueLevelCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is RegressionLine;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			((RegressionLine)indicator).ValueLevel = newValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var oldValue = (ValueLevel)historyItem.OldValue;
			((RegressionLine)indicator).ValueLevel = oldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicator = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicator.Properties["ValueLevel"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			((RegressionLine)indicator).ValueLevel = newValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			var valueLevelPresentation = (ValueLevelComboBoxPresentation)parameter;
			ValueLevel newValue = valueLevelPresentation.ValueLevel;
			ValueLevel oldValue = SelectedIndicatorModel.ValueLevel;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			SelectedIndicatorModel.ValueLevel = newValue;
			return new CommandResult(historyItem);
		}
	}
	public class ChangeMovingAverageValueLevelCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_VlaueLevel); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeMovingAverageValueLevelCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is MovingAverage;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			((MovingAverage)indicator).ValueLevel = newValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var oldValue = (ValueLevel)historyItem.OldValue;
			((MovingAverage)indicator).ValueLevel = oldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicator = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicator.Properties["ValueLevel"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Indicator indicator = GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			((MovingAverage)indicator).ValueLevel = newValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			var valueLevelPresentation = (ValueLevelComboBoxPresentation)parameter;
			ValueLevel newValue = valueLevelPresentation.ValueLevel;
			ValueLevel oldValue = SelectedIndicatorModel.ValueLevel;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			SelectedIndicatorModel.ValueLevel = newValue;
			return new CommandResult(historyItem);
		}
	}
	public class ChangeFinancialIndicatorArgument1Command : IndicatorCommandBase {
		readonly ScaleType scaleType;
		public ScaleType ScaleType {
			get { return scaleType; }
		}
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_Argument1); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeFinancialIndicatorArgument1Command(WpfChartModel chartModel, ScaleType scaleType)
			: base(chartModel) {
			this.scaleType = scaleType;
		}
		protected override bool CanExecute(object parameter) {
			if(!(SelectedIndicator is FinancialIndicator))
				return false;
			var xySeries2D = ChartDesignerPropertiesProvider.GetIndicatorOwner(SelectedIndicator);
			return xySeries2D.ActualArgumentScaleType == this.scaleType;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			finIndicator.Argument1 = historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			finIndicator.Argument1 = historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicatorModelItem.Properties["Argument1"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			finIndicator.Argument1 = historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			object newValue = parameter;
			object oldValue = SelectedIndicatorModel.Argument1;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult commandResult = new CommandResult(historyItem);
			SelectedIndicatorModel.Argument1 = newValue;
			return commandResult;
		}
	}
	public class ChangeFinancialIndicatorArgument2Command : IndicatorCommandBase {
		readonly ScaleType scaleType;
		public ScaleType ScaleType {
			get { return scaleType; }
		}
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_Argument2); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeFinancialIndicatorArgument2Command(WpfChartModel chartModel, ScaleType scaleType)
			: base(chartModel) {
			this.scaleType = scaleType;
		}
		protected override bool CanExecute(object parameter) {
			if(!(SelectedIndicator is FinancialIndicator))
				return false;
			var xySeries2D = ChartDesignerPropertiesProvider.GetIndicatorOwner(SelectedIndicator);
			return xySeries2D.ActualArgumentScaleType == this.scaleType;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			finIndicator.Argument2 = historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			finIndicator.Argument2 = historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicatorModelItem.Properties["Argument2"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			finIndicator.Argument2 = historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			object newValue = parameter;
			object oldValue = SelectedIndicatorModel.Argument2;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult commandResult = new CommandResult(historyItem);
			SelectedIndicatorModel.Argument2 = newValue;
			return commandResult;
		}
	}
	public class ChangeFinancialIndicatorValueLevel1 : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_VlaueLevel1); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeFinancialIndicatorValueLevel1(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is FinancialIndicator;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			finIndicator.ValueLevel1 = newValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var oldValue = (ValueLevel)historyItem.OldValue;
			finIndicator.ValueLevel1 = oldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicator = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicator.Properties["ValueLevel1"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			finIndicator.ValueLevel1 = newValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			var valueLevelPresentation = (ValueLevelComboBoxPresentation)parameter;
			ValueLevel newValue = valueLevelPresentation.ValueLevel;
			ValueLevel oldValue = SelectedIndicatorModel.ValueLevel1;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			SelectedIndicatorModel.ValueLevel1 = newValue;
			return new CommandResult(historyItem);
		}
	}
	public class ChangeFinancialIndicatorValueLevel2 : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_ValueLevel2); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeFinancialIndicatorValueLevel2(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is FinancialIndicator;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			finIndicator.ValueLevel2 = newValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			var oldValue = (ValueLevel)historyItem.OldValue;
			finIndicator.ValueLevel2 = oldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicator = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			indicator.Properties["ValueLevel2"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var finIndicator = (FinancialIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			var newValue = (ValueLevel)historyItem.NewValue;
			finIndicator.ValueLevel2 = newValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			var valueLevelPresentation = (ValueLevelComboBoxPresentation)parameter;
			ValueLevel newValue = valueLevelPresentation.ValueLevel;
			ValueLevel oldValue = SelectedIndicatorModel.ValueLevel2;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			SelectedIndicatorModel.ValueLevel2 = newValue;
			return new CommandResult(historyItem);
		}
	}
	public class ToggleTrendLineExtrapolateToInfinityCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_TrenlineExtrapolateToInfinity); }
		}
		public override string ImageName {
			get { return GlyphUtils.BarItemImages + "TrendLineExtrapolateToInfinity"; }
		}
		public ToggleTrendLineExtrapolateToInfinityCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter){
			return SelectedIndicator is TrendLine;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var trendLine = (TrendLine)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			trendLine.ExtrapolateToInfinity = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var trendLine = (TrendLine)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			trendLine.ExtrapolateToInfinity = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem trendLineModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			trendLineModelItem.Properties["ExtrapolateToInfinity"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var trendLine = (TrendLine)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			trendLine.ExtrapolateToInfinity = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null) {
				ChartDebug.WriteWarning("ToggleExtrapolateToInfinityCommand can't be executed because of parameter == null.");
				return null;
			}
			bool newValue = (bool)parameter;
			bool oldValue = SelectedIndicatorModel.ExtrapolateToInfinity;
			SelectedIndicatorModel.ExtrapolateToInfinity = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ToggleFibonacciIndicatorShowLevel23_6Command : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_FibonacciIndicatorShowLevel23_6); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleFibonacciIndicatorShowLevel23_6Command(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter){
			return SelectedIndicator is FibonacciIndicator;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var fibonacciIndicator = (FibonacciIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciIndicator.ShowLevel23_6 = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var fibonacciIndicator = (FibonacciIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciIndicator.ShowLevel23_6 = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem fibonacciIndicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			fibonacciIndicatorModelItem.Properties["ShowLevel23_6"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var fibonacciIndicator = (FibonacciIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			fibonacciIndicator.ShowLevel23_6 = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null) {
				ChartDebug.WriteWarning("ToggleFibonacciIndicatorShowLevel23_6Command can't be executed because of parameter == null.");
				return null;
			}
			bool newValue = (bool)parameter;
			bool oldValue = SelectedIndicatorModel.ShowLevel23_6;
			SelectedIndicatorModel.ShowLevel23_6 = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ToggleFibonacciIndicatorShowLevel76_4Command : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_FibonacciIndicatorShowLevel76_4); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleFibonacciIndicatorShowLevel76_4Command(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is FibonacciIndicator;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var fibonacciIndicator = (FibonacciIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciIndicator.ShowLevel76_4 = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var fibonacciIndicator = (FibonacciIndicator)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciIndicator.ShowLevel76_4 = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem fibonacciIndicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			fibonacciIndicatorModelItem.Properties["ShowLevel76_4"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var fibonacciIndicator = (FibonacciIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			fibonacciIndicator.ShowLevel76_4 = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null) {
				ChartDebug.WriteWarning("ToggleFibonacciIndicatorShowLevel76_4Command can't be executed because of parameter == null.");
				return null;
			}
			bool newValue = (bool)parameter;
			bool oldValue = SelectedIndicatorModel.ShowLevel76_4;
			SelectedIndicatorModel.ShowLevel76_4 = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ToggleFibonacciArcsShowLevel100Command : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_FibonacciArcsShowLevel100); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleFibonacciArcsShowLevel100Command(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is FibonacciArcs;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var fibonacciArcs = (FibonacciArcs)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciArcs.ShowLevel100 = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var fibonacciArcs = (FibonacciArcs)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciArcs.ShowLevel100 = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem fibonacciArcsModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			fibonacciArcsModelItem.Properties["ShowLevel100"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var fibonacciArcs = (FibonacciArcs)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			fibonacciArcs.ShowLevel100 = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null) {
				ChartDebug.WriteWarning("ToggleFibonacciArcsShowLevel100Command can't be executed because of parameter == null.");
				return null;
			}
			bool newValue = (bool)parameter;
			bool oldValue = SelectedIndicatorModel.ShowLevel100;
			SelectedIndicatorModel.ShowLevel100 = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ToggleFibonacciFansShowLevel0Command : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_FibonacciFansShowLevel0); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleFibonacciFansShowLevel0Command(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is FibonacciFans;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var fibonacciFans = (FibonacciFans)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciFans.ShowLevel0 = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var fibonacciFans = (FibonacciFans)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciFans.ShowLevel0 = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem fibonacciFansModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			fibonacciFansModelItem.Properties["ShowLevel0"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var fibonacciFans = (FibonacciFans)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			fibonacciFans.ShowLevel0 = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null) {
				ChartDebug.WriteWarning("ToggleFibonacciFansShowLevel0Command can't be executed because of parameter == null.");
				return null;
			}
			bool newValue = (bool)parameter;
			bool oldValue = SelectedIndicatorModel.ShowLevel0;
			SelectedIndicatorModel.ShowLevel0 = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ToggleFibonacciRetracementShowAdditionalLevelsCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_FibonacciRetracementShowAdditionalLevels); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleFibonacciRetracementShowAdditionalLevelsCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is FibonacciRetracement;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var fibonacciRetracement = (FibonacciRetracement)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciRetracement.ShowAdditionalLevels = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var fibonacciRetracement = (FibonacciRetracement)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			fibonacciRetracement.ShowAdditionalLevels = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem fibonacciRetracementModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			fibonacciRetracementModelItem.Properties["ShowAdditionalLevels"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var fibonacciRetracement = (FibonacciRetracement)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			fibonacciRetracement.ShowAdditionalLevels = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null) {
				ChartDebug.WriteWarning("ToggleFibonacciRetracementShowAdditionalLevelsCommand can't be executed because of parameter == null.");
				return null;
			}
			bool newValue = (bool)parameter;
			bool oldValue = SelectedIndicatorModel.ShowAdditionalLevels;
			SelectedIndicatorModel.ShowAdditionalLevels = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeMovingAverageEnvelopePercentCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_MovingAverageEnvelopePercent); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeMovingAverageEnvelopePercentCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is MovingAverage;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			movingAverage.EnvelopePercent = (double)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			movingAverage.EnvelopePercent = (double)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem movingAverageModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			movingAverageModelItem.Properties["EnvelopePercent"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			movingAverage.EnvelopePercent = (double)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			double newValue = Convert.ToDouble(parameter);
			double oldValue = SelectedIndicatorModel.EnvelopePercent;
			SelectedIndicatorModel.EnvelopePercent = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeMovingAveragePointsCountCommand : IndicatorCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_MovingAveragePointsCount); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeMovingAveragePointsCountCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is MovingAverage;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			movingAverage.PointsCount = (int)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			movingAverage.PointsCount = (int)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem movingAverageModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			movingAverageModelItem.Properties["PointsCount"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			movingAverage.PointsCount = (int)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			int newValue = Convert.ToInt32(parameter);
			int oldValue = SelectedIndicatorModel.PointsCount;
			SelectedIndicatorModel.PointsCount = newValue;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeMovingAverageKindComand : IndicatorCommandBase {
		MovingAverageKind movingAverageKind;
		Dictionary<MovingAverageKind, ChartDesignerStringIDs> captionStringIdByKindDictionary = new Dictionary<MovingAverageKind,ChartDesignerStringIDs>(){
			{MovingAverageKind.Envelope, ChartDesignerStringIDs.Indicators_MovingAverageKind_Envelope},
			{MovingAverageKind.MovingAverage, ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverage},
			{MovingAverageKind.MovingAverageAndEnvelope, ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverageAndEnvelope}
		};
		Dictionary<MovingAverageKind, ChartDesignerStringIDs> descriptionStringIdByKindDictionary = new Dictionary<MovingAverageKind, ChartDesignerStringIDs>(){
			{MovingAverageKind.Envelope, ChartDesignerStringIDs.Indicators_MovingAverageKind_EnvelopeDescription},
			{MovingAverageKind.MovingAverage, ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverageDescription},
			{MovingAverageKind.MovingAverageAndEnvelope, ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverageAndEnvelopeDescription}
		};
		Dictionary<MovingAverageKind, string> imageNameByKindDictionary = new Dictionary<MovingAverageKind, string>(){
			{MovingAverageKind.Envelope, GlyphUtils.GalleryItemImages + "MovingAverageKind/Envelope"},
			{MovingAverageKind.MovingAverage, GlyphUtils.GalleryItemImages + "MovingAverageKind/MovingAverage"},
			{MovingAverageKind.MovingAverageAndEnvelope, GlyphUtils.GalleryItemImages + "MovingAverageKind/MovingAverageAndEnvelope"}
		};
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(captionStringIdByKindDictionary[movingAverageKind]); }
		}
		public override string ImageName {
			get { return imageNameByKindDictionary[movingAverageKind]; }
		}
		public override string Description {
			get { return ChartDesignerLocalizer.GetString(descriptionStringIdByKindDictionary[movingAverageKind]); }
		}
		public MovingAverageKind MovingAverageKind {
			get { return movingAverageKind;  }
		}
		public ChangeMovingAverageKindComand(WpfChartModel chartModel, MovingAverageKind kind)
			: base(chartModel) {
			this.movingAverageKind = kind;
		}
		protected override bool CanExecute(object parameter) {
			return SelectedIndicator is MovingAverage;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			movingAverage.MovingAverageKind = (MovingAverageKind)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForUndoRedo(historyItem.ExecuteCommandInfo);
			movingAverage.MovingAverageKind = (MovingAverageKind)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem movingAverageModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			movingAverageModelItem.Properties["MovingAverageKind"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var movingAverage = (MovingAverage)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			movingAverage.MovingAverageKind = (MovingAverageKind)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			MovingAverageKind newValue = this.movingAverageKind;
			MovingAverageKind oldValue = SelectedIndicatorModel.MovingAverageKind;
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			SelectedIndicatorModel.MovingAverageKind = newValue;
			return new CommandResult(historyItem);
		}
	}
}
