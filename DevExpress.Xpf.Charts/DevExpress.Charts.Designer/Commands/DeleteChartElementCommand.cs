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
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using System.Security;
namespace DevExpress.Charts.Designer.Native {
	public class RemoveElementCommand : ChartCommandBase {
		public override string Caption {
			get { return "Remove Element"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveElementCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter != null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) { }
		public override CommandResult RuntimeExecute(object parameter) {
			ChartCommandBase command = null;
			if (parameter is Diagram)
				command = new RemoveDiagramCommand(ChartModel);
			else if (parameter is Legend)
				command = new RemoveLegendCommand(ChartModel);
			else if (parameter is Series)
				command = new RemoveSeriesCommand(ChartModel);
			else if (parameter is Pane)
				command = new RemovePaneCommand(ChartModel);
			else if (parameter is SecondaryAxisX2D || parameter is SecondaryAxisY2D)
				command = new RemoveSecondaryAxisCommand(ChartModel);
			else if (parameter is ConstantLine)
				command = new RemoveConstantLineCommand(ChartModel);
			else if (parameter is Strip)
				command = new RemoveStripCommand(ChartModel);
			if (parameter is Title)
				command = new RemoveChartTitleCommand(ChartModel);
			if (parameter is Indicator)
				command = new RemoveIndicatorCommand(ChartModel);
			if (command != null)
				return command.RuntimeExecute(parameter);
			return null;
		}
	}
	public class RemoveLegendCommand : ChartCommandBase {
		public override string Caption {
			get { return "Remove Legend"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveLegendCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter), this, PreviewChart, PreviewChart.Legend, null);
			PreviewChart.Legend = null;
			return new CommandResult(result, PreviewChart);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Legend = historyItem.OldValue as Legend;
			return historyItem.OldValue;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Legend = historyItem.NewValue as Legend;
			return PreviewChart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Legend = null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Legend"].SetValue(null);
		}
	}
	public class RemoveSeriesCommand : ChartCommandBase {
		const string seriesIndexKey = "Series";
		protected const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		public override string Caption {
			get { return "Remove Series"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveSeriesCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter != null && parameter is Series;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			Series series = parameter as Series;
			int seriesIndex = ((WpfChartSeriesModel)ChartModel.SelectedModel).GetSelfIndex();
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, seriesIndex);
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, ((WpfChartSeriesModel)ChartModel.SelectedModel).GetSelfDesigntimeIndex());
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, PreviewChart, series, null));
			if (seriesIndex >= 0)
				PreviewChart.Diagram.Series.RemoveAt(seriesIndex);
			else {
				PreviewChart.Diagram.SeriesTemplate = null;
				SelectSeriesDataMemberCommand seriesDataMember = new SelectSeriesDataMemberCommand(ChartModel);
				resultItem.HistoryItems.Add(seriesDataMember.RuntimeExecute(string.Empty).HistoryItem);
			}
			return new CommandResult(resultItem, PreviewChart);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			if (seriesIndex >= 0)
				PreviewChart.Diagram.Series.Insert(seriesIndex, historyItem.OldValue as Series);
			else
				PreviewChart.Diagram.SeriesTemplate = historyItem.OldValue as Series;
			return historyItem.OldValue as Series;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			if (seriesIndex >= 0)
				PreviewChart.Diagram.Series.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]);
			else
				PreviewChart.Diagram.SeriesTemplate = null;
			return PreviewChart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			if (seriesIndex >= 0)
				chartControl.Diagram.Series.RemoveAt(seriesIndex);
			else
				chartControl.Diagram.SeriesTemplate = null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			if (seriesIndex >= 0)
				chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection.RemoveAt(seriesIndex);
			else
				chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].ClearValue();
		}
	}
	public class RemovePaneCommand : ChartCommandBase {
		const string paneIndexKey = "Pane";
		public override string Caption {
			get { return "Remove Pane"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemovePaneCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter != null && parameter is Pane && PreviewChart.Diagram is XYDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Pane pane = parameter as Pane;
			XYDiagram2D diagram = PreviewChart.Diagram as XYDiagram2D;
			if (diagram.ActualDefaultPane != pane) {
				int index = diagram.Panes.IndexOf(pane);
				HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter, new ElementIndexItem(paneIndexKey, index)), this, PreviewChart, pane, null);
				diagram.Panes.RemoveAt(index);
				return new CommandResult(result, PreviewChart);
			}
			return new RemoveDiagramCommand(ChartModel).RuntimeExecute(parameter);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D diagram = PreviewChart.Diagram as XYDiagram2D;
			diagram.Panes.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey], historyItem.OldValue as Pane);
			return historyItem.OldValue as Pane;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D diagram = PreviewChart.Diagram as XYDiagram2D;
			diagram.Panes.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey]);
			return PreviewChart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D diagram = chartControl.Diagram as XYDiagram2D;
			diagram.Panes.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey]);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["Panes"].Collection.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey]);
		}
	}
	public class RemoveSecondaryAxisCommand : ChartCommandBase {
		const string axisIndexKey = "Axis";
		public override string Caption {
			get { return "Remove Axis"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveSecondaryAxisCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter != null && (parameter is SecondaryAxisX2D || parameter is SecondaryAxisY2D);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			XYDiagram2D diagram = PreviewChart.Diagram as XYDiagram2D;
			if (parameter is SecondaryAxisY2D) {
				int index = diagram.SecondaryAxesY.IndexOf((SecondaryAxisY2D)parameter);
				HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter, new ElementIndexItem(axisIndexKey, index)), this, PreviewChart, parameter, null);
				diagram.SecondaryAxesY.RemoveAt(index);
				return new CommandResult(result, PreviewChart);
			}
			else if (parameter is SecondaryAxisX2D) {
				int index = diagram.SecondaryAxesX.IndexOf((SecondaryAxisX2D)parameter);
				HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter, new ElementIndexItem(axisIndexKey, index)), this, PreviewChart, parameter, null);
				diagram.SecondaryAxesX.RemoveAt(index);
				return new CommandResult(result, PreviewChart);
			}
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D diagram = PreviewChart.Diagram as XYDiagram2D;
			if (historyItem.OldValue is SecondaryAxisY2D) {
				diagram.SecondaryAxesY.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey], historyItem.OldValue as SecondaryAxisY2D);
			}
			else if (historyItem.OldValue is SecondaryAxisX2D) {
				diagram.SecondaryAxesX.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey], historyItem.OldValue as SecondaryAxisX2D);
			}
			return historyItem.OldValue as Axis2D;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D diagram = PreviewChart.Diagram as XYDiagram2D;
			if (historyItem.OldValue is SecondaryAxisY2D) {
				diagram.SecondaryAxesY.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]);
			}
			else if (historyItem.OldValue is SecondaryAxisX2D) {
				diagram.SecondaryAxesX.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]);
			}
			return PreviewChart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D diagram = chartControl.Diagram as XYDiagram2D;
			if (historyItem.OldValue is SecondaryAxisY2D) {
				diagram.SecondaryAxesY.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]);
			}
			else if (historyItem.OldValue is SecondaryAxisX2D) {
				diagram.SecondaryAxesX.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]);
			}
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			if (historyItem.OldValue is SecondaryAxisY2D) {
				chartModelItem.Properties["Diagram"].Value.Properties["SecondaryAxesY"].Collection.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]);
			}
			else if (historyItem.OldValue is SecondaryAxisX2D) {
				chartModelItem.Properties["Diagram"].Value.Properties["SecondaryAxesX"].Collection.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]);
			}
		}
	}
	public class RemoveConstantLineCommand : ConstantLineCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveConstantLineCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return parameter is ConstantLine;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] indexes = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexes);
			Axis2D axis = ChartDesignerPropertiesProvider.GetConstantLineOwner(SelectedConstantLine);
			HistoryItem historyItem = new HistoryItem(info, this, axis, SelectedConstantLine, null);
			CommandResult result = new CommandResult(historyItem, ChartModel.Chart);
			axis.ConstantLinesInFront.Remove(SelectedConstantLine);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Axis2D axis = (Axis2D)historyItem.TargetObject;
			int index = GetConstantLineIndex(historyItem.ExecuteCommandInfo);
			axis.ConstantLinesInFront.Insert(index, (ConstantLine)historyItem.OldValue);
			return (ConstantLine)historyItem.OldValue;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Axis2D axis = (Axis2D)historyItem.TargetObject;
			axis.ConstantLinesInFront.Remove((ConstantLine)historyItem.OldValue);
			return ChartModel.Chart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Axis2D axis = GetTargetAxisForRuntimeApply(chartControl, historyItem);
			ConstantLine constantLine = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			axis.ConstantLinesInFront.Remove(constantLine);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axis = GetTargetAxisForDesigntimeApply(chartModelItem, historyItem);
			int index = GetConstantLineIndex(historyItem.ExecuteCommandInfo);
			if (index != -1)
				axis.Properties["ConstantLinesInFront"].Collection.RemoveAt(index);
		}
	}
	public class RemoveStripCommand : StripCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveStripCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return parameter is Strip;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] indexes = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexes);
			Axis2D axis = ChartDesignerPropertiesProvider.GetStripOwner(SelectedStrip);
			HistoryItem historyItem = new HistoryItem(info, this, axis, SelectedStrip, null);
			CommandResult result = new CommandResult(historyItem, ChartModel.Chart);
			axis.Strips.Remove(SelectedStrip);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Axis2D axis = (Axis2D)historyItem.TargetObject;
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[StripKey];
			axis.Strips.Insert(index, (Strip)historyItem.OldValue);
			return (Strip)historyItem.OldValue;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Axis2D axis = (Axis2D)historyItem.TargetObject;
			axis.Strips.Remove((Strip)historyItem.OldValue);
			return ChartModel.Chart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Axis2D axis = GetTargetAxisForRuntimeApply(chartControl, historyItem);
			Strip strip = GetTargetStripForRuntimeApply(chartControl, historyItem);
			axis.Strips.Remove(strip);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axis = GetTargetAxisForDesigntimeApply(chartModelItem, historyItem);
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[StripKey];
			axis.Properties["Strips"].Collection.RemoveAt(index);
		}
	}
	public class RemoveChartTitleCommand : ChartCommandBase {
		const string titleIndexKey = "Chart Title";
		public override string Caption {
			get { return "Remove Chart Title"; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveChartTitleCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter is Title;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Title title = parameter as Title;
			int titleIndex = PreviewChart.Titles.IndexOf(title);
			HistoryItem result = new HistoryItem(new ExecuteCommandInfo(parameter, new ElementIndexItem(titleIndexKey, titleIndex)), this, PreviewChart, title, null);
			PreviewChart.Titles.RemoveAt(titleIndex);
			return new CommandResult(result, PreviewChart);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Titles.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey], historyItem.OldValue as Title);
			return historyItem.OldValue as Title;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Titles.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey]);
			return PreviewChart;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Titles.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey]);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Titles"].Collection.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey]);
		}
	}
	public class RemoveIndicatorCommand : ChartCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveIndicatorCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter is Indicator;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Series"];
			XYSeries2D xySeries2D = (XYSeries2D)PreviewChart.Diagram.Series[seriesIndex];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Indicator"];
			xySeries2D.Indicators.Insert(indicatorIndex, (Indicator)historyItem.OldValue);
			return historyItem.OldValue;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Series"];
			XYSeries2D xySeries2D = (XYSeries2D)PreviewChart.Diagram.Series[seriesIndex];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Indicator"];
			xySeries2D.Indicators.RemoveAt(indicatorIndex);
			return PreviewChart;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Series"];
			IModelItem seriesModelItem = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Indicator"];
			seriesModelItem.Properties["Indicators"].Collection.RemoveAt(indicatorIndex);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Series"];
			XYSeries2D xySeries2D = (XYSeries2D)chartControl.Diagram.Series[seriesIndex];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary["Indicator"];
			xySeries2D.Indicators.RemoveAt(indicatorIndex);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Indicator indicator = (Indicator)parameter;
			XYSeries2D xySeries2D = ChartDesignerPropertiesProvider.GetIndicatorOwner(indicator);
			int seriesIndex = PreviewChart.Diagram.Series.IndexOf(xySeries2D);
			int indicatorIndex = xySeries2D.Indicators.IndexOf(indicator);
			ElementIndexItem seriesIndesItem = new ElementIndexItem("Series", seriesIndex);
			ElementIndexItem indicatorIndexItem = new ElementIndexItem("Indicator", indicatorIndex);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, seriesIndesItem, indicatorIndexItem);
			HistoryItem result = new HistoryItem(execComInfo, this, indicator, null);
			xySeries2D.Indicators.RemoveAt(indicatorIndex);
			return new CommandResult(result, PreviewChart);
		}
	}
}
