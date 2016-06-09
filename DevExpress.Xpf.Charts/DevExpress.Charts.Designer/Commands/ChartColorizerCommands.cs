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

using System.Security;
using DevExpress.Xpf.Charts;
using DevExpress.Charts.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ColorizerCollectionCommand : ChartCommandBase {
		protected const string SeriesIndexKey = "Series";
		protected const string ColorizerIndexKey = "Colorizer";
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public ColorizerCollectionCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter != null;
		}
		protected CommandResult CreateCommandResult(Series series, object odValue, object parameter, int keyIndex) {
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(SeriesIndexKey, PreviewChart.Diagram.Series.IndexOf(series));
			indexItems[1] = new ElementIndexItem(ColorizerIndexKey, keyIndex);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, series, odValue, parameter);
			CommandResult result = new CommandResult(historyItem, series);
			return result;
		}
	}
	public class AddKeyColorColorizerKeyCommand : ColorizerCollectionCommand {
		public AddKeyColorColorizerKeyCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = (Series)historyItem.TargetObject;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys.Add(historyItem.ExecuteCommandInfo.Parameter);
			return series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys.RemoveAt(keyIndex);
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			ExecuteCommandInfo executeCommandInfo = historyItem.ExecuteCommandInfo;
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem colorizerModelItem = seriesModelItem.Properties["Colorizer"].Value;
			colorizerModelItem.Properties["Keys"].Collection.Add(executeCommandInfo.Parameter);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys.Add(historyItem.ExecuteCommandInfo.Parameter);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (ChartModel.SelectedModel is WpfChartSeriesModel) {
				Series series = ((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
				KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
				colorizer.Keys.Add(parameter);
				return CreateCommandResult(series, null, parameter, colorizer.Keys.Count - 1);
			}
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be Series");
				return null;
			}
		}
	}
	public class RemoveKeyColorColorizerKeyCommand : ColorizerCollectionCommand {
		int index;
		public RemoveKeyColorColorizerKeyCommand(WpfChartModel chartModel, int startIndex)
			: base(chartModel) {
			this.index = startIndex;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys.RemoveAt(keyIndex);
			return series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys.Insert(keyIndex, historyItem.ExecuteCommandInfo.Parameter);
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			ExecuteCommandInfo executeCommandInfo = historyItem.ExecuteCommandInfo;
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = executeCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem colorizerModelItem = seriesModelItem.Properties["Colorizer"].Value;
			colorizerModelItem.Properties["Keys"].Collection.RemoveAt(keyIndex);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys.RemoveAt(keyIndex);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (ChartModel.SelectedModel is WpfChartSeriesModel) {
				Series series = ((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
				KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
				object oldValue = colorizer.Keys[index];
				colorizer.Keys.RemoveAt(index);
				return CreateCommandResult(series, oldValue, parameter, index);
			}
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be Series");
				return null;
			}
		}
	}
	public class ReplaceKeyColorColorizerKeyCommand : ColorizerCollectionCommand {
		int index;
		public ReplaceKeyColorColorizerKeyCommand(WpfChartModel chartModel, int startIndex)
			: base(chartModel) {
			this.index = startIndex;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys[keyIndex] = historyItem.ExecuteCommandInfo.Parameter;
			return series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys[keyIndex] = historyItem.OldValue;
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			ExecuteCommandInfo executeCommandInfo = historyItem.ExecuteCommandInfo;
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = executeCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem colorizerModelItem = seriesModelItem.Properties["Colorizer"].Value;
			IModelItemCollection keysCollectionModel = colorizerModelItem.Properties["Keys"].Collection;
			keysCollectionModel.RemoveAt(keyIndex);
			keysCollectionModel.Insert(keyIndex, executeCommandInfo.Parameter);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
			colorizer.Keys[keyIndex] = historyItem.ExecuteCommandInfo.Parameter;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (ChartModel.SelectedModel is WpfChartSeriesModel) {
				Series series = ((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
				KeyColorColorizer colorizer = (KeyColorColorizer)series.Colorizer;
				object odValue = colorizer.Keys[index];
				colorizer.Keys[index] = parameter;
				return CreateCommandResult(series, odValue, parameter, index);
			}
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be Series");
				return null;
			}
		}
	}
	public class AddRangeColorizerRangeStopCommand : ColorizerCollectionCommand {
		public AddRangeColorizerRangeStopCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return parameter is double;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = (Series)historyItem.TargetObject;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops.Add((double)historyItem.ExecuteCommandInfo.Parameter);
			return series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops.RemoveAt(keyIndex);
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			ExecuteCommandInfo executeCommandInfo = historyItem.ExecuteCommandInfo;
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem colorizerModelItem = seriesModelItem.Properties["Colorizer"].Value;
			colorizerModelItem.Properties["RangeStops"].Collection.Add(executeCommandInfo.Parameter);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops.Add((double)historyItem.ExecuteCommandInfo.Parameter);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (ChartModel.SelectedModel is WpfChartSeriesModel) {
				Series series = ((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
				RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
				colorizer.RangeStops.Add((double)parameter);
				return CreateCommandResult(series, null, parameter, colorizer.RangeStops.Count - 1);
			}
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be Series");
				return null;
			}
		}
	}
	public class RemoveRangeColorizerRangeStopCommand : ColorizerCollectionCommand {
		int index;
		public RemoveRangeColorizerRangeStopCommand(WpfChartModel chartModel, int startIndex)
			: base(chartModel) {
			this.index = startIndex;
		}
		protected override bool CanExecute(object parameter) {
			return parameter is double;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops.RemoveAt(keyIndex);
			return series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops.Insert(keyIndex, (double)historyItem.ExecuteCommandInfo.Parameter);
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			ExecuteCommandInfo executeCommandInfo = historyItem.ExecuteCommandInfo;
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = executeCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem colorizerModelItem = seriesModelItem.Properties["Colorizer"].Value;
			colorizerModelItem.Properties["RangeStops"].Collection.RemoveAt(keyIndex);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops.RemoveAt(keyIndex);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (ChartModel.SelectedModel is WpfChartSeriesModel) {
				Series series = ((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
				RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
				object oldValue = colorizer.RangeStops[index];
				colorizer.RangeStops.RemoveAt(index);
				return CreateCommandResult(series, oldValue, parameter, index);
			}
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be Series");
				return null;
			}
		}
	}
	public class ReplaceRangeColorizerRangeStopCommand : ColorizerCollectionCommand {
		int index;
		public ReplaceRangeColorizerRangeStopCommand(WpfChartModel chartModel, int startIndex)
			: base(chartModel) {
			this.index = startIndex;
		}
		protected override bool CanExecute(object parameter) {
			return parameter is double;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops[keyIndex] = (double)historyItem.ExecuteCommandInfo.Parameter;
			return series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = (Series)historyItem.TargetObject;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops[keyIndex] = (double)historyItem.OldValue;
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			ExecuteCommandInfo executeCommandInfo = historyItem.ExecuteCommandInfo;
			int seriesIndex = executeCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = executeCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem colorizerModelItem = seriesModelItem.Properties["Colorizer"].Value;
			IModelItemCollection keysCollectionModel = colorizerModelItem.Properties["RangeStops"].Collection;
			keysCollectionModel.RemoveAt(keyIndex);
			keysCollectionModel.Insert(keyIndex, executeCommandInfo.Parameter);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int keyIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ColorizerIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
			colorizer.RangeStops[keyIndex] = (double)historyItem.ExecuteCommandInfo.Parameter;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (ChartModel.SelectedModel is WpfChartSeriesModel) {
				Series series = ((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
				RangeColorizer colorizer = (RangeColorizer)series.Colorizer;
				object odValue = colorizer.RangeStops[index];
				colorizer.RangeStops[index] = (double)parameter;
				return CreateCommandResult(series, odValue, parameter, index);
			}
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be Series");
				return null;
			}
		}
	}
}
