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
using DevExpress.Xpf.Charts;
using System.Security;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public abstract class SeriesPointCommand : ChartCommandBase {
		protected const string seriesIndexKey = "Series";
		protected const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		protected const string pointIndexKey = "Point";
		public override string Caption {
			get {
				return null;
			}
		}
		public override string ImageName {
			get {
				return null;
			}
		}
		protected override bool CanExecute(object parameter) {
			return (parameter != null);
		}
		public SeriesPointCommand(WpfChartModel chartModel) : base(chartModel) { }
	}
	public class AddSeriesPointCommand : SeriesPointCommand {
		public AddSeriesPointCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		public override CommandResult RuntimeExecute(object parameter) {
			WpfChartSeriesModel seriesModel = (WpfChartSeriesModel)parameter;
			Series series = seriesModel.Series;
			SeriesPoint point = new SeriesPoint();
			series.Points.Add(point);
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, seriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, seriesModel.GetSelfDesigntimeIndex());
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(null, indexItems), this, null, null, point));
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series series = PreviewChart.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.Remove((SeriesPoint)historyItem.NewValue);
			return series;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = PreviewChart.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.Add((SeriesPoint)historyItem.NewValue);
			return series;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Series series = chartControl.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.Add(new SeriesPoint());
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem seriesAccess = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey]];
			seriesAccess.Properties["Points"].Collection.Add(seriesAccess.Context.CreateItem(typeof(SeriesPoint)));
		}
	}
	public class RemoveSeriesPointCommand : SeriesPointCommand {
		public RemoveSeriesPointCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		public override CommandResult RuntimeExecute(object parameter) {
			WpfChartSeriesPointModel seriesPointModel = (WpfChartSeriesPointModel)parameter;
			Series series = seriesPointModel.SeriesModel.Series;
			series.Points.Remove(seriesPointModel.SeriesPoint);
			ElementIndexItem[] indexItems = new ElementIndexItem[3];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, seriesPointModel.SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, seriesPointModel.SeriesModel.GetSelfDesigntimeIndex());
			indexItems[2] = new ElementIndexItem(pointIndexKey, seriesPointModel.GetSelfIndex());
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(null,indexItems), this, null, seriesPointModel.SeriesPoint, null), series);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series series = PreviewChart.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey], (SeriesPoint)historyItem.OldValue);
			return series;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = PreviewChart.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]);
			return series;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Series series = chartControl.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem seriesAccess = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey]];
			seriesAccess.Properties["Points"].Collection.RemoveAt(historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]);
		}
	}
	public class ClearSeriesDataCommand : SeriesPointCommand {
		public ClearSeriesDataCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return true; 
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series series = PreviewChart.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.AddRange((SeriesPoint[])historyItem.OldValue);
			return series;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = PreviewChart.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.Clear();
			return series;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem seriesAccess = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey]];
			seriesAccess.Properties["Points"].Collection.Clear();
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Series series = chartControl.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			series.Points.Clear();
		}
		public override CommandResult RuntimeExecute(object parameter) {
			WpfChartSeriesModel seriesModel = (WpfChartSeriesModel)ChartModel.SelectedModel;
			Series series = seriesModel.Series;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, seriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, seriesModel.GetSelfDesigntimeIndex());
			SeriesPoint[] oldCollection = new SeriesPoint[series.Points.Count];
			series.Points.CopyTo(oldCollection, 0);
			series.Points.Clear();
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(null, indexItems), this, null, oldCollection, null), series);
		}
	}
	public abstract class SeriesPointPropertyCommand : SeriesPointCommand {
		public SeriesPointPropertyCommand(WpfChartModel chartModel) : base(chartModel) { }
		public override CommandResult RuntimeExecute(object parameter) {
			SeriesPointCommandParameter p = (SeriesPointCommandParameter)parameter;
			object oldValue = GetProperty(p.SeriesPointModel);
			SetProperty(p.SeriesPointModel, p.Value);
			ElementIndexItem[] indexItems = new ElementIndexItem[3];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, p.SeriesPointModel.SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, p.SeriesPointModel.SeriesModel.GetSelfDesigntimeIndex());
			indexItems[2] = new ElementIndexItem(pointIndexKey, p.SeriesPointModel.GetSelfIndex());
			HistoryItem record = new HistoryItem(new ExecuteCommandInfo(p.Value,indexItems), this, null, oldValue, p.Value);
			return new CommandResult(record, null);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var seriesModel = (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			var seriesPointModel = (WpfChartSeriesPointModel)seriesModel.SeriesPointCollectionModel.ModelCollection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]];
			SetProperty(seriesPointModel, historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var seriesModel = (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			var seriesPointModel = (WpfChartSeriesPointModel)seriesModel.SeriesPointCollectionModel.ModelCollection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]];
			SetProperty(seriesPointModel, historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Series series = chartControl.Diagram.Series[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey]];
			SeriesPoint seriesPoint = series.Points[historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]];
			SetProperty(seriesPoint, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem seriesAccess = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey]];
			IModelItem seriesPointAccess = seriesAccess.Properties["Points"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey]];
			SetProperty(seriesAccess, seriesPointAccess, historyItem.NewValue);
		}
		protected abstract object GetProperty(WpfChartSeriesPointModel seriesPointModel);
		protected abstract void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value);
		protected abstract void SetProperty(SeriesPoint seriesPoint, object value);
		protected abstract void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value);
	}
	public class SetSeriesPointArgumentCommand : SeriesPointPropertyCommand {
		public SetSeriesPointArgumentCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Argument;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Argument = value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetSeriesPointArgumentSafely(seriesPoint, value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			seriesPointItem.Properties["Argument"].SetValue(value);
		}
	}
	public class SetSeriesPointValueCommand : SeriesPointPropertyCommand {
		public SetSeriesPointValueCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Value;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Value = value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			if (value is DateTime)
				seriesPoint.DateTimeValue = (DateTime)value;
			else
				seriesPoint.Value = (double)value;
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			seriesPointItem.Properties["Value"].SetValue(value);
		}
	}
	public class SetSeriesPointValue2Command : SeriesPointPropertyCommand {
		public SetSeriesPointValue2Command(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Value2;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Value2 = (double)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetAttachedValue2(seriesPoint, (double)value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			DataEditorUtils.SetAttachedValue2(seriesItem, seriesPointItem, (double)value);
		}
	}
	public class SetSeriesPointWeightCommand : SeriesPointPropertyCommand {
		public SetSeriesPointWeightCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Weight;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Weight = (double)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetAttachedWeight(seriesPoint, (double)value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			DataEditorUtils.SetAttachedWeight(seriesItem, seriesPointItem, (double)value);
		}
	}
	public class SetSeriesPointOpenCommand : SeriesPointPropertyCommand {
		public SetSeriesPointOpenCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Open;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Open = (double)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetAttachedOpen(seriesPoint, (double)value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			DataEditorUtils.SetAttachedOpen(seriesItem, seriesPointItem, (double)value);
		}
	}
	public class SetSeriesPointCloseCommand : SeriesPointPropertyCommand {
		public SetSeriesPointCloseCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Close;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Close = (double)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetAttachedClose(seriesPoint, (double)value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			DataEditorUtils.SetAttachedClose(seriesItem, seriesPointItem, (double)value);
		}
	}
	public class SetSeriesPointHighCommand : SeriesPointPropertyCommand {
		public SetSeriesPointHighCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.High;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.High = (double)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetAttachedHigh(seriesPoint, (double)value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			DataEditorUtils.SetAttachedHigh(seriesItem, seriesPointItem, (double)value);
		}
	}
	public class SetSeriesPointLowCommand : SeriesPointPropertyCommand {
		public SetSeriesPointLowCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Low;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Low = (double)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			DataEditorUtils.SetAttachedLow(seriesPoint, (double)value);
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			DataEditorUtils.SetAttachedLow(seriesItem, seriesPointItem, (double)value);
		}
	}
	public class SetSeriesPointBrushCommand : SeriesPointPropertyCommand {
		public SetSeriesPointBrushCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override object GetProperty(WpfChartSeriesPointModel seriesPointModel) {
			return seriesPointModel.Brush;
		}
		protected override void SetProperty(WpfChartSeriesPointModel seriesPointModel, object value) {
			seriesPointModel.Brush = (SolidColorBrush)value;
		}
		protected override void SetProperty(SeriesPoint seriesPoint, object value) {
			seriesPoint.Brush = (SolidColorBrush)value;
		}
		protected override void SetProperty(IModelItem seriesItem, IModelItem seriesPointItem, object value) {
			seriesPointItem.Properties["Brush"].SetValue((SolidColorBrush)value);
		}
	}
}
