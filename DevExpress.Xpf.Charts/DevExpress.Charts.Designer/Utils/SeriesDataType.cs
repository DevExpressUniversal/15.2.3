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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using Microsoft.Windows.Design.Metadata;
using System.Windows.Media;
using Platform::DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Charts.Designer.Native {
	public enum ChartDataEditorColumnType {
		Numeric,
		DateTime,
		Text,
		Brush
	}
	public class ChartDataEditorGridColumn {
		readonly string fieldName;
		readonly string header;
		readonly ChartDataEditorColumnType type;
		public string FieldName {
			get { return fieldName; }
		}
		public string Header {
			get { return header; }
		}
		public ChartDataEditorColumnType Type {
			get { return type; }
		}
		public ChartDataEditorGridColumn(string fieldName, string header, ChartDataEditorColumnType type) {
			this.fieldName = fieldName;
			this.header = header;
			this.type = type;
		}
	}
	public class SeriesPointCommandParameter {
		readonly WpfChartSeriesPointModel seriesPointModel;
		readonly object value;
		public WpfChartSeriesPointModel SeriesPointModel {
			get { return seriesPointModel; }
		}
		public object Value {
			get { return value; }
		}
		public SeriesPointCommandParameter(WpfChartSeriesPointModel seriesPointModel, object value) {
			this.seriesPointModel = seriesPointModel;
			this.value = value;
		}
	}
	public class ChartDataEditorPointCollectionModel : NotifyPropertyChangedObject {
		readonly WpfChartModel chartModel;
		readonly AddSeriesPointCommand addPointCommand;
		readonly RemoveSeriesPointCommand removePointCommand;
		readonly SetSeriesPointArgumentCommand setArgumentCommand;
		readonly SetSeriesPointValueCommand setValueCommand;
		readonly SetSeriesPointValue2Command setValue2Command;
		readonly SetSeriesPointWeightCommand setWeightCommand;
		readonly SetSeriesPointOpenCommand setOpenCommand;
		readonly SetSeriesPointCloseCommand setCloseCommand;
		readonly SetSeriesPointHighCommand setHighCommand;
		readonly SetSeriesPointLowCommand setLowCommand;
		readonly SetSeriesPointBrushCommand setBrushCommand;
		readonly ClearSeriesDataCommand clearSeriesDataCommand;
		readonly BindingList<ChartDataEditorPointModel> gridRows;
		ChartDataEditorPointModel selectedRow;
		ObservableCollection<ChartDataEditorGridColumn> gridColumns;
		WpfChartSeriesModel seriesModel;
		bool addCommandExecuting;
		bool isEnabled;
		public ICommand SetArgumentCommand {
			get { return setArgumentCommand; }
		}
		public ICommand SetValueCommand {
			get { return setValueCommand; }
		}
		public ICommand SetValue2Command {
			get { return setValue2Command; }
		}
		public ICommand SetWeightCommand {
			get { return setWeightCommand; }
		}
		public ICommand SetOpenCommand {
			get { return setOpenCommand; }
		}
		public ICommand SetCloseCommand {
			get { return setCloseCommand; }
		}
		public ICommand SetHighCommand {
			get { return setHighCommand; }
		}
		public ICommand SetLowCommand {
			get { return setLowCommand; }
		}
		public ICommand SetBrushCommand {
			get { return setBrushCommand; }
		}
		public ICommand AddPointCommand {
			get { return addPointCommand; }
		}
		public ICommand RemovePointCommand {
			get { return removePointCommand; }
		}
		public ICommand ClearSeriesDataCommand {
			get { return clearSeriesDataCommand; }
		}
		public WpfChartSeriesModel SeriesModel {
			get { return seriesModel; }
			set {
				if (seriesModel != value) {
					Subscribe(seriesModel, value);
					seriesModel = value;
					UpdateColumns();
					UpdateRows();
					OnPropertyChanged("SeriesModel");
					IsEnabled = GetEnabledStatus();
				}
			}
		}
		public ObservableCollection<ChartDataEditorGridColumn> GridColumns {
			get { return gridColumns; }
		}
		public BindingList<ChartDataEditorPointModel> GridRows {
			get { return gridRows; }
		}
		public ChartDataEditorPointModel SelectedRow {
			get { return selectedRow; }
			set {
				if (selectedRow != value) {
					selectedRow = value;
					OnPropertyChanged("SelectedRow");
				}
			}
		}
		public bool IsEnabled {
			get { return isEnabled; }
			private set {
				if (isEnabled != value) {
					isEnabled = value;
					OnPropertyChanged("IsEnabled");
				}
			}
		}
		public ChartDataEditorPointCollectionModel(WpfChartModel chartModel) {
			this.chartModel = chartModel;
			this.removePointCommand = new RemoveSeriesPointCommand(chartModel);
			this.addPointCommand = new AddSeriesPointCommand(chartModel);
			this.setArgumentCommand = new SetSeriesPointArgumentCommand(chartModel);
			this.setValueCommand = new SetSeriesPointValueCommand(chartModel);
			this.setValue2Command = new SetSeriesPointValue2Command(chartModel);
			this.setWeightCommand = new SetSeriesPointWeightCommand(chartModel);
			this.setOpenCommand = new SetSeriesPointOpenCommand(chartModel);
			this.setCloseCommand = new SetSeriesPointCloseCommand(chartModel);
			this.setHighCommand = new SetSeriesPointHighCommand(chartModel);
			this.setLowCommand = new SetSeriesPointLowCommand(chartModel);
			this.setBrushCommand = new SetSeriesPointBrushCommand(chartModel);
			this.clearSeriesDataCommand = new ClearSeriesDataCommand(chartModel);
			this.gridColumns = new ObservableCollection<ChartDataEditorGridColumn>();
			this.gridRows = new BindingList<ChartDataEditorPointModel>();
			this.gridRows.AddingNew += AddingNewPoint;
			chartModel.PropertyChanged += ChartModelPropertyChanged;
		}
		bool GetEnabledStatus() {
			return (SeriesModel != null) && (!SeriesModel.IsAutoSeries) && (!SeriesModel.IsSeriesTemplate) && (!SeriesModel.IsAutoPointsAdded);
		}
		void Subscribe(WpfChartSeriesModel oldModel, WpfChartSeriesModel newModel) {
			if (oldModel != null)
				oldModel.SeriesPointCollectionModel.CollectionUpdated -= SeriesPointCollectionModelUpdated;
			if (newModel != null)
				newModel.SeriesPointCollectionModel.CollectionUpdated += SeriesPointCollectionModelUpdated;
		}
		void ChartModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "SelectedModel")
				if (chartModel.SelectedModel is WpfChartSeriesModel)
					SeriesModel = (WpfChartSeriesModel)chartModel.SelectedModel;
				else
					SeriesModel = null;
		}
		void UpdateRows() {
			gridRows.Clear();
			if (seriesModel != null) {
				foreach (WpfChartSeriesPointModel pointModel in seriesModel.SeriesPointCollectionModel.ModelCollection)
					gridRows.Add(new ChartDataEditorPointModel(this, pointModel));
			}
		}
		void UpdateColumns() {
			gridColumns.Clear();
			if (seriesModel != null) {
				List<ChartDataEditorGridColumn> columns = DataEditorUtils.GenerateColumns(seriesModel.Series);
				foreach (var column in columns)
					gridColumns.Add(column);
			}
		}
		int FindModel(WpfChartSeriesPointModel model) {
			for (int index = 0; index < gridRows.Count; index++) {
				if (gridRows[index].SeriesPointModel == model)
					return index;
			}
			return -1;
		}
		void InsertModel(int index, WpfChartSeriesPointModel model) {
			if (FindModel(model) < 0)
				gridRows.Insert(index, new ChartDataEditorPointModel(this, model));
		}
		void RemoveModel(WpfChartSeriesPointModel model) {
			int index = FindModel(model);
			if (index >= 0)
				gridRows.RemoveAt(index);
		}
		void SeriesPointCollectionModelUpdated(ChartCollectionUpdateEventArgs args) {
			if (!addCommandExecuting) {
				foreach (InsertedItem item in args.AddedItems)
					InsertModel(item.Index, (WpfChartSeriesPointModel)item.Item);
				foreach (WpfChartSeriesPointModel model in args.RemovedItems)
					RemoveModel(model);
			}
		}
		void AddingNewPoint(object sender, AddingNewEventArgs e) {
			addCommandExecuting = true;
			if (AddPointCommand.CanExecute(SeriesModel))
				AddPointCommand.Execute(SeriesModel);
			e.NewObject = new ChartDataEditorPointModel(this, SeriesModel.SeriesPointCollectionModel.LastPointModel);
			addCommandExecuting = false;
		}
	}
	public class ChartDataEditorPointModel : NotifyPropertyChangedObject {
		readonly WpfChartSeriesPointModel seriesPointModel;
		readonly ChartDataEditorPointCollectionModel collectionModel;
		public WpfChartSeriesPointModel SeriesPointModel {
			get { return seriesPointModel; }
		}
		public object Argument {
			get { return seriesPointModel.Argument; }
			set { ExecuteCommand(collectionModel.SetArgumentCommand, value); }
		}
		public object Value {
			get { return seriesPointModel.Value; }
			set { ExecuteCommand(collectionModel.SetValueCommand, value); }
		}
		public double Value2 {
			get { return seriesPointModel.Value2; }
			set { ExecuteCommand(collectionModel.SetValue2Command, value); }
		}
		public double Weight {
			get { return seriesPointModel.Weight; }
			set { ExecuteCommand(collectionModel.SetWeightCommand, value); }
		}
		public double Open {
			get { return seriesPointModel.Open; }
			set { ExecuteCommand(collectionModel.SetOpenCommand, value); }
		}
		public double Close {
			get { return seriesPointModel.Close; }
			set { ExecuteCommand(collectionModel.SetCloseCommand, value); }
		}
		public double High {
			get { return seriesPointModel.High; }
			set { ExecuteCommand(collectionModel.SetHighCommand, value); }
		}
		public double Low {
			get { return seriesPointModel.Low; }
			set { ExecuteCommand(collectionModel.SetLowCommand, value); }
		}
		public SolidColorBrush Brush {
			get { return seriesPointModel.Brush == null ? null : seriesPointModel.Brush; }
			set { ExecuteCommand(collectionModel.SetBrushCommand, value); }
		}
		public ChartDataEditorPointModel(ChartDataEditorPointCollectionModel collectionModel, WpfChartSeriesPointModel seriesPointModel) {
			this.collectionModel = collectionModel;
			this.seriesPointModel = seriesPointModel;
			this.seriesPointModel.PropertyChanged += UpdateBindings;
		}
		[SkipOnPropertyChangedMethodCall]
		void UpdateBindings(object sender, PropertyChangedEventArgs e) {
			OnPropertyChanged(e.PropertyName);
		}
		void ExecuteCommand(ICommand command, object value) {
			var parameter = new SeriesPointCommandParameter(seriesPointModel, value);
			if (command.CanExecute(parameter))
				command.Execute(parameter);
		}
	}
	public class ChartDataEditorColumnTemplateSelector : DataTemplateSelector {
		public DataTemplate NumericTemplate { 
			get; 
			set; 
		}
		public DataTemplate DateTimeTemplate { 
			get; 
			set; 
		}
		public DataTemplate TextTemplate { 
			get; 
			set; 
		}
		public DataTemplate BrushTemplate {
			get;
			set;
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			ChartDataEditorGridColumn column = (ChartDataEditorGridColumn)item;
			switch (column.Type) {
				case ChartDataEditorColumnType.Numeric:
					return NumericTemplate;
				case ChartDataEditorColumnType.DateTime:
					return DateTimeTemplate;
				case ChartDataEditorColumnType.Text:
					return TextTemplate;
				case ChartDataEditorColumnType.Brush:
					return BrushTemplate;
			}
			return TextTemplate;
		}
	}
	public static class DataEditorUtils {
		public static void SetSeriesPointArgumentSafely(SeriesPoint point, object value) {
			if ((value != null) && (value.GetType() == typeof(DateTime)))
				point.Argument = ((DateTime)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
			else
				point.Argument = (string)value;
		}
		public static List<ChartDataEditorGridColumn> GenerateColumns(Series series) {
			List<ChartDataEditorGridColumn> columns = new List<ChartDataEditorGridColumn>();
			ChartDataEditorColumnType argumentType = ChartDataEditorColumnType.Text;
			switch (series.ArgumentScaleType) {
				case ScaleType.DateTime:
					argumentType = ChartDataEditorColumnType.DateTime;
					break;
				case ScaleType.Numerical:
					argumentType = ChartDataEditorColumnType.Numeric;
					break;
				default:
				case ScaleType.Qualitative:
					argumentType = ChartDataEditorColumnType.Text;
					break;
			}
			string argumentColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_ArgumentColumnHeader);
			string valueColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_ValueColumnHeader);
			string openValueColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_OpenValueColumnHeader);
			string closeValueColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_CloseValueColumnHeader);
			string highValueColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_HighValueColumnHeader);
			string lowValueColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_LowValueColumnHeader);
			string value2ColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_Value2ColumnHeader);
			string weightColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_WeightValueColumnHeader);
			string brushColumnHeader = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DataEditor_BrushColumnHeader);
			columns.Add(new ChartDataEditorGridColumn("Argument", argumentColumnHeader, argumentType));
			if (!(series is FinancialSeries2D))
				columns.Add(new ChartDataEditorGridColumn("Value", valueColumnHeader, (series.ValueScaleType == ScaleType.DateTime) ? ChartDataEditorColumnType.DateTime : ChartDataEditorColumnType.Numeric));
			else {
				columns.Add(new ChartDataEditorGridColumn("Open", openValueColumnHeader, ChartDataEditorColumnType.Numeric));
				columns.Add(new ChartDataEditorGridColumn("Close", closeValueColumnHeader, ChartDataEditorColumnType.Numeric));
				columns.Add(new ChartDataEditorGridColumn("High", highValueColumnHeader, ChartDataEditorColumnType.Numeric));
				columns.Add(new ChartDataEditorGridColumn("Low", lowValueColumnHeader, ChartDataEditorColumnType.Numeric));
			}
			if ((series is RangeAreaSeries2D) || (series is RangeBarSeries2D))
				columns.Add(new ChartDataEditorGridColumn("Value2", value2ColumnHeader, ChartDataEditorColumnType.Numeric));
			if ((series is BubbleSeries3D) || (series is BubbleSeries2D))
				columns.Add(new ChartDataEditorGridColumn("Weight", weightColumnHeader, ChartDataEditorColumnType.Numeric));
			if (!(series is FinancialSeries2D))
				columns.Add(new ChartDataEditorGridColumn("Brush", brushColumnHeader, ChartDataEditorColumnType.Brush));
			return columns;
		}
		public static double GetAttachedValue2(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (series is RangeAreaSeries2D)
				return RangeAreaSeries2D.GetValue2(seriesPoint);
			else if (series is RangeBarSeries2D)
				return RangeBarSeries2D.GetValue2(seriesPoint);
			return double.NaN;
		}
		public static double GetAttachedWeight(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (series is BubbleSeries3D)
				return BubbleSeries3D.GetWeight(seriesPoint);
			else if (series is BubbleSeries2D)
				return BubbleSeries2D.GetWeight(seriesPoint);
			return double.NaN;
		}
		public static double GetAttachedOpen(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				return FinancialSeries2D.GetOpenValue(seriesPoint);
			return double.NaN;
		}
		public static double GetAttachedClose(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				return FinancialSeries2D.GetCloseValue(seriesPoint);
			return double.NaN;
		}
		public static double GetAttachedHigh(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				return FinancialSeries2D.GetHighValue(seriesPoint);
			return double.NaN;
		}
		public static double GetAttachedLow(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				return FinancialSeries2D.GetLowValue(seriesPoint);
			return double.NaN;
		}
		public static void SetAttachedValue2(SeriesPoint seriesPoint, double value) {
			Series series = seriesPoint.Series;
			if (series is RangeAreaSeries2D)
				RangeAreaSeries2D.SetValue2(seriesPoint, value);
			else if (series is RangeBarSeries2D)
				RangeBarSeries2D.SetValue2(seriesPoint, value);
		}
		public static void SetAttachedWeight(SeriesPoint seriesPoint, double value) {
			Series series = seriesPoint.Series;
			if (series is BubbleSeries3D)
				BubbleSeries3D.SetWeight(seriesPoint, value);
			else if (series is BubbleSeries2D)
				BubbleSeries2D.SetWeight(seriesPoint, value);
		}
		public static void SetAttachedOpen(SeriesPoint seriesPoint, double value) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				FinancialSeries2D.SetOpenValue(seriesPoint, value);
		}
		public static void SetAttachedClose(SeriesPoint seriesPoint, double value) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				FinancialSeries2D.SetCloseValue(seriesPoint, value);
		}
		public static void SetAttachedHigh(SeriesPoint seriesPoint, double value) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				FinancialSeries2D.SetHighValue(seriesPoint, value);
		}
		public static void SetAttachedLow(SeriesPoint seriesPoint, double value) {
			Series series = seriesPoint.Series;
			if (series is FinancialSeries2D)
				FinancialSeries2D.SetLowValue(seriesPoint, value);
		}
		public static void SetAttachedValue2(IModelItem seriesItem, IModelItem seriesPointItem, double value) {
			DXPropertyIdentifier? propIdentifier = null;
			if (typeof(RangeAreaSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(RangeAreaSeries2D), "Value2");
			else if (typeof(RangeBarSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(RangeBarSeries2D), "Value2");
			if (propIdentifier.HasValue)
				seriesPointItem.Properties[propIdentifier.Value].SetValue(value);
		}
		public static void SetAttachedWeight(IModelItem seriesItem, IModelItem seriesPointItem, double value) {
			DXPropertyIdentifier? propIdentifier = null;
			if (typeof(BubbleSeries3D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(BubbleSeries3D), "Weight");
			else if (typeof(BubbleSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(BubbleSeries2D), "Weight");
			if (propIdentifier.HasValue)
				seriesPointItem.Properties[propIdentifier.Value].SetValue(value);
		}
		public static void SetAttachedOpen(IModelItem seriesItem, IModelItem seriesPointItem, double value) {
			DXPropertyIdentifier? propIdentifier = null;
			if (typeof(FinancialSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(FinancialSeries2D), "Open");
			if (propIdentifier.HasValue)
				seriesPointItem.Properties[propIdentifier.Value].SetValue(value);
		}
		public static void SetAttachedClose(IModelItem seriesItem, IModelItem seriesPointItem, double value) {
			DXPropertyIdentifier? propIdentifier = null;
			if (typeof(FinancialSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(FinancialSeries2D), "Close");
			if (propIdentifier.HasValue)
				seriesPointItem.Properties[propIdentifier.Value].SetValue(value);
		}
		public static void SetAttachedLow(IModelItem seriesItem, IModelItem seriesPointItem, double value) {
			DXPropertyIdentifier? propIdentifier = null;
			if (typeof(FinancialSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(FinancialSeries2D), "Low");
			if (propIdentifier.HasValue)
				seriesPointItem.Properties[propIdentifier.Value].SetValue(value);
		}
		public static void SetAttachedHigh(IModelItem seriesItem, IModelItem seriesPointItem, double value) {
			DXPropertyIdentifier? propIdentifier = null;
			if (typeof(FinancialSeries2D).IsAssignableFrom(seriesItem.ItemType))
				propIdentifier = new DXPropertyIdentifier(typeof(FinancialSeries2D), "High");
			if (propIdentifier != null)
				seriesPointItem.Properties[propIdentifier.Value].SetValue(value);
		}
	}
}
