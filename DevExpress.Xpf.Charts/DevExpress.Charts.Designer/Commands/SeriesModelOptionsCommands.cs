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
using System.Globalization;
using System.Security;
using System.Windows.Data;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ModelOptionsCommand : ChartCommandBase {
		protected const string seriesIndexKey = "seriesIndex";
		protected const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		protected WpfChartSeriesModel SeriesModel { get { return ChartModel.SelectedModel as WpfChartSeriesModel; } }
		public override string ImageName { get { return null; } }
		public ModelOptionsCommand(WpfChartModel model)
			: base(model) {
		}
	}
	public class DisableMarkerCommand : ModelOptionsCommand, IValueConverter {
		Series Series { get { return SeriesModel != null ? SeriesModel.Series : null; } }
		ISupportMarker2D MarkerSeries { get { return Series as ISupportMarker2D; } }
		CircularLineSeries2D CircularLineSeries { get { return Series as CircularLineSeries2D; } }
		CircularAreaSeries2D CircularAreaSeries { get { return Series as CircularAreaSeries2D; } }
		bool Visible {
			get {
				return MarkerSeries != null ? MarkerSeries.MarkerVisible : CircularLineSeries != null ?
					CircularLineSeries.MarkerVisible : CircularAreaSeries != null ? CircularAreaSeries.MarkerVisible : false;
			}
			set {
				if (MarkerSeries != null)
					MarkerSeries.MarkerVisible = value;
				else if (CircularLineSeries != null)
					CircularLineSeries.MarkerVisible = value;
				else if (CircularAreaSeries != null)
					CircularAreaSeries.MarkerVisible = value;
			}
		}
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_NoModelString); } }
		public DisableMarkerCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				ISupportMarker2D series = model.Series as ISupportMarker2D;
				if (series != null && !(series is PointSeries2D) && !(series is BubbleSeries2D))
					return !series.MarkerVisible;
				CircularLineSeries2D circularLineSeries = model.Series as CircularLineSeries2D;
				if (circularLineSeries != null)
					return !circularLineSeries.MarkerVisible;
				CircularAreaSeries2D circularAreaSeries = model.Series as CircularAreaSeries2D;
				if (circularAreaSeries != null)
					return !circularAreaSeries.MarkerVisible;
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return MarkerSeries != null && !(MarkerSeries is PointSeries2D) && !(MarkerSeries is BubbleSeries2D) || CircularLineSeries != null || CircularAreaSeries != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Visible = (bool)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Visible = (bool)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			bool visible = (bool)historyItem.NewValue;
			if (series is ISupportMarker2D)
				((ISupportMarker2D)series).MarkerVisible = visible;
			else if (series is CircularLineSeries2D)
				((CircularLineSeries2D)series).MarkerVisible = visible;
			else if (series is CircularAreaSeries2D)
				((CircularAreaSeries2D)series).MarkerVisible = visible;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["MarkerVisible"].SetValue(historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, Visible, false);
			Visible = false;
			return new CommandResult(historyItem);
		}
	}
	public class Pie3DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Pie3DModel GetModel(object kind) {
			return (Pie3DModel)Activator.CreateInstance(((Pie3DKind)kind).Type);
		}
		public static Pie3DKind GetKind(Type modelType) {
			IEnumerable<Pie3DKind> kinds = Pie3DModel.GetPredefinedKinds();
			foreach (Pie3DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Pie3DKind kind;
		PieSeries3D Series { get { return SeriesModel != null ? SeriesModel.Series as PieSeries3D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public Pie3DModelCommand(WpfChartModel chartModel, Pie3DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				PieSeries3D series = model.Series as PieSeries3D;
				if (series != null)
					return Object.Equals(kind.Type, series.ActualModel.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((PieSeries3D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.ActualModel.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class Pie2DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Pie2DModel GetModel(object kind) {
			return (Pie2DModel)Activator.CreateInstance(((Pie2DKind)kind).Type);
		}
		public static Pie2DKind GetKind(Type modelType) {
			IEnumerable<Pie2DKind> kinds = Pie2DModel.GetPredefinedKinds();
			foreach (Pie2DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Pie2DKind kind;
		PieSeries2D Series { get { return SeriesModel != null ? SeriesModel.Series as PieSeries2D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public Pie2DModelCommand(WpfChartModel chartModel, Pie2DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				PieSeries2D series = model.Series as PieSeries2D;
				if (series != null)
					return Object.Equals(kind.Type, series.Model.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((PieSeries2D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.Model.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class Stock2DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Stock2DModel GetModel(object kind) {
			return (Stock2DModel)Activator.CreateInstance(((Stock2DKind)kind).Type);
		}
		public static Stock2DKind GetKind(Type modelType) {
			IEnumerable<Stock2DKind> kinds = Stock2DModel.GetPredefinedKinds();
			foreach (Stock2DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Stock2DKind kind;
		StockSeries2D Series { get { return SeriesModel != null ? SeriesModel.Series as StockSeries2D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public Stock2DModelCommand(WpfChartModel chartModel, Stock2DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				StockSeries2D series = model.Series as StockSeries2D;
				if (series != null)
					return Object.Equals(kind.Type, series.Model.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((StockSeries2D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.Model.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class CandleStick2DModelCommand : ModelOptionsCommand, IValueConverter {
		public static CandleStick2DModel GetModel(object kind) {
			return (CandleStick2DModel)Activator.CreateInstance(((CandleStick2DKind)kind).Type);
		}
		public static CandleStick2DKind GetKind(Type modelType) {
			IEnumerable<CandleStick2DKind> kinds = CandleStick2DModel.GetPredefinedKinds();
			foreach (CandleStick2DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly CandleStick2DKind kind;
		CandleStickSeries2D Series { get { return SeriesModel != null ? SeriesModel.Series as CandleStickSeries2D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public CandleStick2DModelCommand(WpfChartModel chartModel, CandleStick2DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				CandleStickSeries2D series = model.Series as CandleStickSeries2D;
				if (series != null)
					return Object.Equals(kind.Type, series.Model.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((CandleStickSeries2D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.Model.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class Marker3DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Marker3DModel GetModel(object kind) {
			return (Marker3DModel)Activator.CreateInstance(((Marker3DKind)kind).Type);
		}
		public static Marker3DKind GetKind(Type modelType) {
			IEnumerable<Marker3DKind> kinds = Marker3DModel.GetPredefinedKinds();
			foreach (Marker3DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Marker3DKind kind;
		MarkerSeries3D Series { get { return SeriesModel != null ? SeriesModel.Series as MarkerSeries3D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public Marker3DModelCommand(WpfChartModel chartModel, Marker3DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				MarkerSeries3D series = model.Series as MarkerSeries3D;
				if (series != null)
					return Object.Equals(kind.Type, series.ActualModel.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((MarkerSeries3D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.ActualModel.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class ToggleMarker2DVisibileCommand : ModelOptionsCommand {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		Series Series { get { return SeriesModel != null ? SeriesModel.Series : null; } }
		public ToggleMarker2DVisibileCommand(WpfChartModel chartModel) : base(chartModel) { }
		void ApplyValue(Series series, bool value, out bool oldValue) {
			ISupportMarker2D markerSeries = series as ISupportMarker2D;
			oldValue = false;
			if (markerSeries != null) {
				if (!(markerSeries is PointSeries2D) && !(markerSeries is BubbleSeries2D)) {
					oldValue = markerSeries.MarkerVisible;
					markerSeries.MarkerVisible = value;
				}
			}
			else {
				CircularSeries2D circularSeries = series as CircularSeries2D;
				if (circularSeries != null) {
					if (circularSeries is CircularLineSeries2D) {
						oldValue = ((CircularLineSeries2D)circularSeries).MarkerVisible;
						((CircularLineSeries2D)circularSeries).MarkerVisible = value;
					}
					if (circularSeries is CircularAreaSeries2D) {
						oldValue = ((CircularAreaSeries2D)circularSeries).MarkerVisible;
						((CircularAreaSeries2D)circularSeries).MarkerVisible = value;
					}
				}
			}
		}
		protected override bool CanExecute(object parameter) {
			return parameter is bool;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			bool oldValue;
			ApplyValue(Series, (bool)parameter, out oldValue);
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, Series, oldValue, (bool)parameter));
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series series = (Series)historyItem.TargetObject;
			bool oldValue;
			ApplyValue(series, (bool)historyItem.OldValue, out oldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series series = (Series)historyItem.TargetObject;
			bool oldValue;
			ApplyValue(series, (bool)historyItem.NewValue, out oldValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			bool oldValue;
			ApplyValue(series, (bool)historyItem.NewValue, out oldValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			Series series = (Series)seriesAccess.GetCurrentValue();
			if (!(series is PointSeries2D) && !(series is BubbleSeries2D) || series is CircularLineSeries2D || series is CircularAreaSeries2D)
				seriesAccess.Properties["MarkerVisible"].SetValue((bool)historyItem.NewValue);
		}
	}
	public class Marker2DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Marker2DModel GetModel(object kind) {
			return (Marker2DModel)Activator.CreateInstance(((Marker2DKind)kind).Type);
		}
		public static Marker2DKind GetKind(Type modelType) {
			IEnumerable<Marker2DKind> kinds = Marker2DModel.GetPredefinedKinds();
			foreach (Marker2DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Marker2DKind kind;
		Series Series { get { return SeriesModel != null ? SeriesModel.Series : null; } }
		ISupportMarker2D MarkerSeries { get { return Series as ISupportMarker2D; } }
		CircularSeries2D CircularSeries { get { return Series as CircularSeries2D; } }
		CircularLineSeries2D CircularLineSeries { get { return CircularSeries as CircularLineSeries2D; } }
		CircularAreaSeries2D CircularAreaSeries { get { return CircularSeries as CircularAreaSeries2D; } }
		bool Visible {
			get {
				if (MarkerSeries != null)
					return MarkerSeries is PointSeries2D || MarkerSeries is BubbleSeries2D ? true : MarkerSeries.MarkerVisible;
				if (CircularSeries != null) {
					return CircularLineSeries != null ? CircularLineSeries.MarkerVisible : CircularAreaSeries != null ? CircularAreaSeries.MarkerVisible : true;
				}
				return false;
			}
		}
		public override string Caption { get { return kind.ToString(); } }
		public Marker2DModelCommand(WpfChartModel chartModel, Marker2DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				ISupportMarker2D series = model.Series as ISupportMarker2D;
				if (series != null) {
					bool isSameModels = Object.Equals(kind.Type, series.MarkerModel.GetType());
					if (series is PointSeries2D || series is BubbleSeries2D)
						return isSameModels;
					return series.MarkerVisible && isSameModels;
				}
				else {
					CircularSeries2D circularSeries = model.Series as CircularSeries2D;
					if (circularSeries != null) {
						bool isSameModels = Object.Equals(kind.Type, circularSeries.MarkerModel.GetType());
						if (circularSeries is CircularLineSeries2D)
							return ((CircularLineSeries2D)circularSeries).MarkerVisible && isSameModels;
						else if (circularSeries is CircularAreaSeries2D)
							return ((CircularAreaSeries2D)circularSeries).MarkerVisible && isSameModels;
						return isSameModels;
					}
				}
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return MarkerSeries != null || CircularSeries != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ApplyValue(Series, (Marker2DKind)historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ApplyValue(Series, (Marker2DKind)historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			ApplyValue(series, kind);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["MarkerModel"].SetValue(GetModel(kind));
		}
		void ApplyValue(Series series, Marker2DKind kind) {
			ISupportMarker2D markerSeries = series as ISupportMarker2D;
			Marker2DModel model = Marker2DModelCommand.GetModel(kind);
			if (markerSeries != null) {
				markerSeries.MarkerModel = model;
			}
			else {
				CircularSeries2D circularSeries = series as CircularSeries2D;
				if (circularSeries != null)
					circularSeries.MarkerModel = model;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			CompositeHistoryItem historyItem = new CompositeHistoryItem();
			Marker2DModel model = MarkerSeries != null ? MarkerSeries.MarkerModel : CircularSeries.MarkerModel;
			if (!Visible) {
				ToggleMarker2DVisibileCommand toggleVisibile = new ToggleMarker2DVisibileCommand(ChartModel);
				historyItem.HistoryItems.Add(toggleVisibile.RuntimeExecute(true).HistoryItem);
			}
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			Marker2DKind oldValue = GetKind(model.GetType());
			ApplyValue(Series, kind);
			historyItem.HistoryItems.Add(new HistoryItem(info, this, Series, oldValue, kind));
			return new CommandResult(historyItem);
		}
	}
	public class Bar3DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Bar3DModel GetModel(object kind) {
			return (Bar3DModel)Activator.CreateInstance(((Bar3DKind)kind).Type);
		}
		public static Bar3DKind GetKind(Type modelType) {
			IEnumerable<Bar3DKind> kinds = Bar3DModel.GetPredefinedKinds();
			foreach (Bar3DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Bar3DKind kind;
		BarSeries3D Series { get { return SeriesModel != null ? SeriesModel.Series as BarSeries3D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public Bar3DModelCommand(WpfChartModel chartModel, Bar3DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				BarSeries3D series = model.Series as BarSeries3D;
				if (series != null)
					return Object.Equals(kind.Type, series.ActualModel.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((BarSeries3D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.ActualModel.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class Bar2DModelCommand : ModelOptionsCommand, IValueConverter {
		public static Bar2DModel GetModel(object kind) {
			return (Bar2DModel)Activator.CreateInstance(((Bar2DKind)kind).Type);
		}
		public static Bar2DKind GetKind(Type modelType) {
			IEnumerable<Bar2DKind> kinds = Bar2DModel.GetPredefinedKinds();
			foreach (Bar2DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly Bar2DKind kind;
		BarSeries2D Series { get { return SeriesModel != null ? SeriesModel.Series as BarSeries2D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public Bar2DModelCommand(WpfChartModel chartModel, Bar2DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				BarSeries2D series = model.Series as BarSeries2D;
				if (series != null)
					return Object.Equals(kind.Type, series.Model.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((BarSeries2D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.Model.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
	public class RangeBar2DModelCommand : ModelOptionsCommand, IValueConverter {
		public static RangeBar2DModel GetModel(object kind) {
			return (RangeBar2DModel)Activator.CreateInstance(((RangeBar2DKind)kind).Type);
		}
		public static RangeBar2DKind GetKind(Type modelType) {
			IEnumerable<RangeBar2DKind> kinds = RangeBar2DModel.GetPredefinedKinds();
			foreach (RangeBar2DKind kind in kinds) {
				if (Object.Equals(kind.Type, modelType))
					return kind;
			}
			return null;
		}
		readonly RangeBar2DKind kind;
		RangeBarSeries2D Series { get { return SeriesModel != null ? SeriesModel.Series as RangeBarSeries2D : null; } }
		public override string Caption { get { return kind.ToString(); } }
		public RangeBar2DModelCommand(WpfChartModel chartModel, RangeBar2DKind kind)
			: base(chartModel) {
			this.kind = kind;
		}
		#region IValueConverter
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel model = value as WpfChartSeriesModel;
			if (model != null) {
				RangeBarSeries2D series = model.Series as RangeBarSeries2D;
				if (series != null)
					return Object.Equals(kind.Type, series.Model.GetType());
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
		protected override bool CanExecute(object parameter) {
			return Series != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.OldValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Series.Model = GetModel(historyItem.NewValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			((RangeBarSeries2D)series).Model = GetModel(historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0
				? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex]
				: chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			seriesAccess.Properties["Model"].SetValue(GetModel(historyItem.NewValue));
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (!(bool)parameter))
				return null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(info, this, Series, GetKind(Series.Model.GetType()), kind);
			Series.Model = GetModel(kind);
			return new CommandResult(historyItem);
		}
	}
}
