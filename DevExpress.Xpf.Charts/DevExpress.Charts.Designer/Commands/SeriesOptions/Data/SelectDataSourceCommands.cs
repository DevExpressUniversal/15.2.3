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
using System.Security;
using DevExpress.Xpf.Charts;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public abstract class SelectDataSourceCommandBase : SeriesOptionsCommandBase {
		public SelectDataSourceCommandBase(WpfChartModel model)
			: base(model) {
		}
		protected abstract CommandResult DataSourceRuntimeExecute(object parameter);
		protected abstract void DataSourceUndoInternal(WpfChartSeriesModel model, object oldValue);
		protected abstract void DataSourceRedoInternal(WpfChartSeriesModel model, object oldValue);
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			DataSourceRedoInternal(model, newValue);
			ChartModel.SelectedObject = PreviewChart;
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			DataSourceUndoInternal(model, oldValue);
			ChartModel.SelectedObject = PreviewChart;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CommandResult result = DataSourceRuntimeExecute(parameter);
			if (result != null)
				ChartModel.SelectedObject = PreviewChart;
			return result;
		}
		public abstract bool IsSeriesAtCommandState(WpfChartSeriesModel seriesModel);
	}
	public class SelectDataSourceNoneCommand : SelectDataSourceCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_DataSourceNone); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "DataSource\\DataSourceNone"; }
		}
		public SelectDataSourceNoneCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return SeriesModel != null && (SeriesModel.PredefinedDataSource != null || ((WpfChartModel)SeriesModel.Diagram.Parent).PredefinedDataSource != null);
		}
		[SecuritySafeCritical]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) { }
		protected override void DataSourceRedoInternal(WpfChartSeriesModel model, object newValue) { }
		protected override void RuntimeApplyInternal(Series series, object value) { }
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			if (seriesIndex >= 0)
				seriesModel.DataSource = null;
			ChartModel.SelectedObject = PreviewChart;
			return seriesModel.Series;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			if (seriesIndex >= 0)
				if (((Series)historyItem.TargetObject).DataSource == null)
					seriesModel.SetChartDataSource(historyItem.OldValue);
				else
					seriesModel.SetSeriesDataSource();
			ChartModel.SelectedObject = PreviewChart;
			return seriesModel.Series;
		}
		protected override void DataSourceUndoInternal(WpfChartSeriesModel model, object oldValue) { }
		protected override CommandResult DataSourceRuntimeExecute(object parameter) {
			if (parameter is bool && (bool)parameter) {
				CompositeHistoryItem resultItem = new CompositeHistoryItem();
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				Series selectedSeries = SeriesModel.Series;
				object oldValue = SeriesModel.DataSource;
				if (SeriesModel.IsSeriesTemplatePreview) {
					SelectSeriesDataMemberCommand seriesDataMember = new SelectSeriesDataMemberCommand(ChartModel);
					resultItem.HistoryItems.Add(seriesDataMember.RuntimeExecute(String.Empty).HistoryItem);
				}
				resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, selectedSeries, oldValue, null));
				if (SeriesModel.IsSeriesTemplate) {
					string seriesName = SeriesModel.Name;
					RemoveSeriesTemplateCommand removeTemplate = new RemoveSeriesTemplateCommand(ChartModel);
					resultItem.HistoryItems.Add(removeTemplate.RuntimeExecute(null).HistoryItem);
					AddSeriesCommand addCommand = new AddSeriesCommand(ChartModel, selectedSeries.GetType());
					CommandResult addResult = addCommand.RuntimeExecute(null);
					resultItem.HistoryItems.Add(addResult.HistoryItem);
					selectedSeries = (Series)addResult.SelectedChartObject;
					ChartModel.RecursivelyUpdateChildren();
					ChartModel.SelectedObject = selectedSeries;
					ChangeSeriesNameCommand changeNameCommand = new ChangeSeriesNameCommand(ChartModel);
					resultItem.HistoryItems.Add(changeNameCommand.RuntimeExecute(seriesName).HistoryItem);
					SelectSeriesDataMemberCommand seriesDataMember = new SelectSeriesDataMemberCommand(ChartModel);
					resultItem.HistoryItems.Add(seriesDataMember.RuntimeExecute(String.Empty).HistoryItem);
				}
				else {
					SeriesModel.DataSource = null;
					SelectArgumentDataMemberCommand argumentCommand = new SelectArgumentDataMemberCommand(ChartModel);
					CommandResult result = argumentCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectValueDataMemberCommand valueCommand = new SelectValueDataMemberCommand(ChartModel);
					result = valueCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectWeightDataMemberCommand weightCommand = new SelectWeightDataMemberCommand(ChartModel);
					result = weightCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectValue2DataMemberCommand value2Command = new SelectValue2DataMemberCommand(ChartModel);
					result = value2Command.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectLowValueDataMemberCommand lowValueCommand = new SelectLowValueDataMemberCommand(ChartModel);
					result = lowValueCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectHighValueDataMemberCommand highValueCommand = new SelectHighValueDataMemberCommand(ChartModel);
					result = highValueCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectOpenValueDataMemberCommand openValueCommand = new SelectOpenValueDataMemberCommand(ChartModel);
					result = openValueCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					SelectCloseValueDataMemberCommand closeValueCommand = new SelectCloseValueDataMemberCommand(ChartModel);
					result = closeValueCommand.DataMemberRuntimeExecute(new DataMemberInfo());
					if (result != null)
						resultItem.HistoryItems.Add(result.HistoryItem);
					if (SeriesModel.IsSeriesTemplatePreview) {
						SelectSeriesDataMemberCommand seriesDataMemberCommand = new SelectSeriesDataMemberCommand(ChartModel);
						result = seriesDataMemberCommand.DataMemberRuntimeExecute(new DataMemberInfo());
						if (result != null)
							resultItem.HistoryItems.Add(result.HistoryItem);
					}
				}
				return new CommandResult(resultItem, selectedSeries, RibbonPagesNames.SeriesOptionsDataPage);
			}
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesDesigntimeIndexKey];
			if (seriesIndex >= 0) {
				IModelItem seriesItem = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex];
				seriesItem.Properties["DataSource"].ClearValue();
			}
		}
		public override bool IsSeriesAtCommandState(WpfChartSeriesModel seriesModel) {
			return !seriesModel.IsSeriesTemplate && seriesModel.DataSource == null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			if (seriesIndex >= 0)
				series.DataSource = null;
		}
	}
	public class SelectChartDataSourceCommand : SelectDataSourceCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_ChartDataSource); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "DataSource\\ChartDataSource"; }
		}
		public SelectChartDataSourceCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return SeriesModel != null && ((WpfChartModel)SeriesModel.Diagram.Parent).PredefinedDataSource != null;
		}
		[SecuritySafeCritical]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {
			seriesAccess.Properties["DataSource"].ClearValue();
		}
		protected override void DataSourceRedoInternal(WpfChartSeriesModel model, object newValue) {
			model.SetChartDataSource(newValue);
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			series.DataSource = null;
		}
		protected override void DataSourceUndoInternal(WpfChartSeriesModel model, object oldValue) {
			if (oldValue != null)
				model.SetSeriesDataSource();
			else
				model.DataSource = oldValue;
		}
		protected override CommandResult DataSourceRuntimeExecute(object parameter) {
			if (parameter is bool && (bool)parameter) {
				object oldValue = SeriesModel.DataSource;
				SeriesModel.SetChartDataSource(ChartModel.DataSource);
				Series series = SeriesModel.Series;
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, ChartModel.DataSource), series, RibbonPagesNames.SeriesOptionsDataPage);
			}
			return null;
		}
		public override bool IsSeriesAtCommandState(WpfChartSeriesModel seriesModel) {
			return seriesModel.IsSeriesTemplate || (seriesModel.DataSource != null && seriesModel.DataSource == ChartModel.DataSource);
		}
	}
	public class SelectSeriesDataSourceCommand : SelectDataSourceCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_SeriesDataSource); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "DataSource\\SeriesDataSource"; }
		}
		public SelectSeriesDataSourceCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return SeriesModel != null && SeriesModel.PredefinedDataSource != null;
		}
		[SecuritySafeCritical]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {
			seriesAccess.Properties["DataSource"].SetValue(value);
		}
		protected override void DataSourceRedoInternal(WpfChartSeriesModel model, object newValue) {
			model.SetSeriesDataSource();
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			series.DataSource = value;
		}
		protected override void DataSourceUndoInternal(WpfChartSeriesModel model, object oldValue) {
			if (oldValue != null)
				model.SetChartDataSource(oldValue);
			else
				model.DataSource = oldValue;
		}
		protected override CommandResult DataSourceRuntimeExecute(object parameter) {
			if (parameter is bool && (bool)parameter) {
				object oldValue = SeriesModel.DataSource;
				SeriesModel.SetSeriesDataSource();
				Series series = SeriesModel.Series;
				object newValue = SeriesModel.PredefinedDataSource;
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, newValue), series, RibbonPagesNames.SeriesOptionsDataPage);
			}
			return null;
		}
		public override bool IsSeriesAtCommandState(WpfChartSeriesModel seriesModel) {
			return seriesModel.DataSource != null && seriesModel.DataSource == seriesModel.PredefinedDataSource && seriesModel.Series.DataSource != null;
		}
	}
}
