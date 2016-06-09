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
using System.Reflection;
using DevExpress.Xpf.Charts;
using System.Security;
using Microsoft.Windows.Design.Metadata;
using System.Windows.Input;
namespace DevExpress.Charts.Designer.Native {
	public class PropertyGridCommandParameter {
		readonly string propertyName;
		readonly object value;
		readonly ChartModelElement model;
		public string PropertyName { get { return propertyName; } }
		public object Value { get { return value; } }
		public ChartModelElement Model { get { return model; } }
		public PropertyGridCommandParameter(string propertyName, object value, ChartModelElement model) {
			this.propertyName = propertyName;
			this.value = value;
			this.model = model;
		}
	}
	public delegate void SetAttached(object targetObject, object value);
	public delegate object GetAttached(object targetObject);
	public class PropertyGridCommandParameterAttached : PropertyGridCommandParameter {
		readonly Type propertyOwnerType;
		readonly SetAttached setAttachedProperty;
		readonly GetAttached getAttachedProperty;
		readonly string targetObjectPath;
		public Type PropertyOwnerType { get { return propertyOwnerType; } }
		public SetAttached SetAttachedProperty { get { return setAttachedProperty; } }
		public GetAttached GetAttachedProperty { get { return getAttachedProperty; } }
		public string TargetObjectPath { get { return targetObjectPath; } }
		public PropertyGridCommandParameterAttached(string targetObjectPath, string propertyName, object value, ChartModelElement model, Type propertyOwnerType, SetAttached setAttachedProperty, GetAttached getAttachedProperty)
			: this(propertyName, value, model, propertyOwnerType, setAttachedProperty, getAttachedProperty) {
			this.targetObjectPath = targetObjectPath;
		}
		public PropertyGridCommandParameterAttached(string propertyName, object value, Type propertyOwnerType, SetAttached setAttachedProperty, GetAttached getAttachedProperty)
			: this(propertyName, value, null, propertyOwnerType, setAttachedProperty, getAttachedProperty) {
		}
		public PropertyGridCommandParameterAttached(string propertyName, object value, ChartModelElement model, Type propertyOwnerType, SetAttached setAttachedProperty, GetAttached getAttachedProperty)
			: base(propertyName, value, model) {
			this.propertyOwnerType = propertyOwnerType;
			this.setAttachedProperty = setAttachedProperty;
			this.getAttachedProperty = getAttachedProperty;
		}
	}
	public abstract class PropertyGridCommandBase : ChartCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public PropertyGridCommandBase(WpfChartModel model)
			: base(model) {
		}
	}
	public class SetChartPropertyCommand : PropertyGridCommandBase {
		public SetChartPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			ChartModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return historyItem.TargetObject;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			ChartModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return historyItem.TargetObject;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				object oldValue = ChartModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter), this, ChartModel.SelectedModel.ChartElement, oldValue, commandParameter.Value), ChartModel.SelectedModel.ChartElement);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(chartControl, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(chartModelItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetSeriesPropertyCommand : PropertyGridCommandBase {
		const string seriesIndexKey = "Series";
		protected const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		WpfChartSeriesModel SeriesModel { get { return ChartModel.SelectedModel as WpfChartSeriesModel; } }
		public SetSeriesPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return SeriesModel != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			seriesModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return seriesModel != null ? seriesModel.Series : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			seriesModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return seriesModel != null ? seriesModel.Series : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				object oldValue = SeriesModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), SeriesModel.Series);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(series, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(seriesAccess, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetIndicatorPropertyCommand : PropertyGridCommandBase {
		const string indicatorIndexKey = "Indicator";
		const string seriesIndexKey = "Series";
		const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		public SetIndicatorPropertyCommand(WpfChartModel model) : base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartIndicatorModel;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[indicatorIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			WpfChartIndicatorModel indicatorModel = (WpfChartIndicatorModel)seriesModel.IndicatorCollectionModel.ModelCollection[indicatorIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			indicatorModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return indicatorModel != null ? indicatorModel.Indicator : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[indicatorIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			WpfChartIndicatorModel indicatorModel = (WpfChartIndicatorModel)seriesModel.IndicatorCollectionModel.ModelCollection[indicatorIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			indicatorModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return indicatorModel != null ? indicatorModel.Indicator : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			WpfChartIndicatorModel indicatorModel = (WpfChartIndicatorModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				object oldValue = indicatorModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[3];
				XYSeries2D series = ((XYSeries2D)((WpfChartSeriesModel)indicatorModel.Parent.Parent).Series);
				int seriesIndex = PreviewChart.Diagram.Series.IndexOf(series);
				int indicatorIndex = series.Indicators.IndexOf(indicatorModel.Indicator);
				indexItems[0] = new ElementIndexItem(seriesIndexKey, seriesIndex);
				indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, ((WpfChartSeriesModel)indicatorModel.Parent.Parent).GetSelfDesigntimeIndex());
				indexItems[2] = new ElementIndexItem(indicatorIndexKey, indicatorIndex);
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), indicatorModel.Indicator);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[indicatorIndexKey];			
			XYSeries2D series = seriesIndex >= 0 ? (XYSeries2D)chartControl.Diagram.Series[seriesIndex] : (XYSeries2D)chartControl.Diagram.SeriesTemplate;
			Indicator indicator = series.Indicators[indicatorIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(indicator, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[indicatorIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem indicatorItem = seriesAccess.Properties["Indicators"].Collection[indicatorIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(indicatorItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetSeriesLabelAttachedPropertyCommand : PropertyGridCommandBase {
		const string seriesIndexKey = "Series";
		const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		WpfChartSeriesModel SeriesModel { get { return ChartModel.SelectedModel as WpfChartSeriesModel; } }
		public SetSeriesLabelAttachedPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(SeriesModel.Series.Label, historyItem.NewValue);
			return seriesModel != null ? seriesModel.Series : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(SeriesModel.Series.Label, historyItem.OldValue);
			return seriesModel != null ? seriesModel.Series : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)parameter;
			object oldValue = commandParameter.GetAttachedProperty(SeriesModel.Series.Label);
			commandParameter.SetAttachedProperty(SeriesModel.Series.Label, commandParameter.Value);
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), SeriesModel.Series);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(series.Label, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem labelItem = seriesAccess.Properties["Label"].Value;
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			DXPropertyIdentifier propIdentifier = new DXPropertyIdentifier(commandParameter.PropertyOwnerType, commandParameter.PropertyName);
			labelItem.Properties[propIdentifier].SetValue(historyItem.NewValue);
		}
	}
	public class SetSeriesPointPropertyCommand : PropertyGridCommandBase {
		const string seriesIndexKey = "Series";
		const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		const string pointIndexKey = "Point";
		WpfChartSeriesModel SeriesModel { get { return ChartModel.SelectedModel as WpfChartSeriesModel; } }
		public SetSeriesPointPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		void SetProperty(object targetObject, string name, object value) {
			if (!String.IsNullOrEmpty(name)) {
				Type objectType = targetObject.GetType();
				PropertyInfo propertyInfo = objectType.GetProperty(name);
				propertyInfo.SetValue(targetObject, value, new object[0]);
			}
		}
		object SetPointProperty(HistoryItem historyItem, object value) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int pointIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			WpfChartSeriesPointModel pointModel = (WpfChartSeriesPointModel)seriesModel.SeriesPointCollectionModel.ModelCollection[pointIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			pointModel.SetChartElementProperty(commandParameter.PropertyName, value);
			return seriesModel != null ? seriesModel.Series : null;
		}
		protected override bool CanExecute(object parameter) {
			return SeriesModel != null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			return SetPointProperty(historyItem, historyItem.OldValue);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			return SetPointProperty(historyItem, historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName) && commandParameter.Model is WpfChartSeriesPointModel) {
				WpfChartSeriesPointModel pointModel = commandParameter.Model as WpfChartSeriesPointModel;
				object oldValue = pointModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[3];
				indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				indexItems[2] = new ElementIndexItem(pointIndexKey, pointModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), SeriesModel.Series);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int pointIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			SeriesPoint point = series.Points[pointIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			SetProperty(point, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			int pointIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem pointItem = seriesAccess.Properties["Points"].Collection[pointIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			pointItem.Properties[commandParameter.PropertyName].SetValue(commandParameter.Value);
		}
	}
	public class SetSeriesPointAttachedPropertyCommand : PropertyGridCommandBase {
		const string seriesIndexKey = "Series";
		const string seriesDesigntimeIndexKey = "DesigntimeSeries";
		const string pointIndexKey = "Point";
		WpfChartSeriesModel SeriesModel { get { return ChartModel.SelectedModel as WpfChartSeriesModel; } }
		public SetSeriesPointAttachedPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return SeriesModel != null;
		}
		object SetPointProperty(HistoryItem historyItem, object value) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int pointIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			WpfChartSeriesPointModel pointModel = (WpfChartSeriesPointModel)seriesModel.SeriesPointCollectionModel.ModelCollection[pointIndex];
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(pointModel.SeriesPoint, value);
			return seriesModel != null ? seriesModel.Series : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			return SetPointProperty(historyItem, historyItem.NewValue);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			return SetPointProperty(historyItem, historyItem.OldValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)parameter;
			if (commandParameter.Model is WpfChartSeriesPointModel) {
				WpfChartSeriesPointModel pointModel = commandParameter.Model as WpfChartSeriesPointModel;
				object oldValue = commandParameter.GetAttachedProperty(pointModel.SeriesPoint);
				commandParameter.SetAttachedProperty(pointModel.SeriesPoint, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[3];
				indexItems[0] = new ElementIndexItem(seriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(seriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				indexItems[2] = new ElementIndexItem(pointIndexKey, pointModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), SeriesModel.Series);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			int pointIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			SeriesPoint point = series.Points[pointIndex];
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(point, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesDesigntimeIndexKey];
			int pointIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[pointIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem pointItem = seriesAccess.Properties["Points"].Collection[pointIndex];
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			DXPropertyIdentifier propIdentifier = new DXPropertyIdentifier(commandParameter.PropertyOwnerType, commandParameter.PropertyName);
			pointItem.Properties[propIdentifier].SetValue(historyItem.NewValue);
		}
	}
	public class SetAxisPropertyCommand : AxisOptionsCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public SetAxisPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartAxisModel;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			axisModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			axisModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				CreateAxisIfNeeded(resultItem);
				if (axisModel != SelectedAxisModel)
					axisModel = SelectedAxisModel;
				object oldValue = axisModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(axisIndexKey, axisModel.GetSelfIndex());
				resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, axisModel.Axis, oldValue, commandParameter.Value));
				return new CommandResult(resultItem, axisModel.Axis);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = GetAxis(chartControl, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(axis, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(axisItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetResolveOverlappingOptionsPropertyCommand : AxisOptionsCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public SetResolveOverlappingOptionsPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartAxisModel;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(Axis2D.GetResolveOverlappingOptions(axisModel.Label.Label), commandParameter.PropertyName, historyItem.OldValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(Axis2D.GetResolveOverlappingOptions(axisModel.Label.Label), commandParameter.PropertyName, historyItem.NewValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				CreateAxisIfNeeded(resultItem);
				if (axisModel != SelectedAxisModel)
					axisModel = SelectedAxisModel;
				object target = Axis2D.GetResolveOverlappingOptions(axisModel.Axis.Label);
				object oldValue = CommandUtils.GetObjectProperty(target, commandParameter.PropertyName);
				CommandUtils.SetObjectProperty(target, commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(axisIndexKey, axisModel.GetSelfIndex());
				resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, axisModel.Axis, oldValue, commandParameter.Value));
				return new CommandResult(resultItem, axisModel.Axis);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = GetAxis(chartControl, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(Axis2D.GetResolveOverlappingOptions(axis.Label), commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			IModelItem labelItem = axisItem.Properties["Label"].Value;
			DXPropertyIdentifier propIdentifier = new DXPropertyIdentifier(typeof(Axis2D), "ResolveOverlappingOptions");
			IModelItem resolveOverlappingOptionsItem = labelItem.Properties[propIdentifier].Value;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(resolveOverlappingOptionsItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetAxisAttachedPropertyCommand : AxisOptionsCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public SetAxisAttachedPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartAxisModel;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			object targetObject = axisModel.GetChartElementProperty(commandParameter.TargetObjectPath);
			commandParameter.SetAttachedProperty(targetObject, historyItem.NewValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			object targetObject = axisModel.GetChartElementProperty(commandParameter.TargetObjectPath);
			commandParameter.SetAttachedProperty(targetObject, historyItem.OldValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)parameter;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				CreateAxisIfNeeded(resultItem);
				if (axisModel != SelectedAxisModel)
					axisModel = SelectedAxisModel;
				object targetObject = axisModel.GetChartElementProperty(commandParameter.TargetObjectPath);
				object oldValue = commandParameter.GetAttachedProperty(targetObject);
				commandParameter.SetAttachedProperty(targetObject, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(axisIndexKey, axisModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, axisModel.Axis, oldValue, commandParameter.Value), axisModel.Axis);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = GetAxis(chartControl, historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			object targetObject = CommandUtils.GetObjectProperty(axis, commandParameter.TargetObjectPath);
			commandParameter.SetAttachedProperty(targetObject, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			IModelItem attachedPropertyItem = CommandUtils.GetModelItemProperty(axisItem, commandParameter.TargetObjectPath);
			DXPropertyIdentifier propIdentifier = new DXPropertyIdentifier(commandParameter.PropertyOwnerType, commandParameter.PropertyName);
			attachedPropertyItem.Properties[propIdentifier].SetValue(historyItem.NewValue);
		}
	}
	public class SetAxisLabelPropertyCommand : AxisOptionsCommandBase {
		WpfChartAxisModel SelectedAxisModel {
			get { return ChartModel.SelectedModel as WpfChartAxisModel; }
		}
		public override string Caption { get { throw new NotImplementedException(); } }
		public override string ImageName { get { throw new NotImplementedException(); } }
		public SetAxisLabelPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartAxisModel;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			axisModel.Label.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			axisModel.Label.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				CreateLabelIfNeeded();
				WpfChartAxisLabelModel labelModel = SelectedAxisModel.Label;
				object oldValue = labelModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(axisIndexKey, SelectedAxisModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, SelectedAxisModel.Axis, oldValue, commandParameter.Value), SelectedAxisModel.Axis);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = GetAxis(chartControl, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(axis.Label, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			IModelItem labelItem = axisItem.Properties["Label"].Value;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(labelItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetAxisLabelAttachedPropertyCommand : AxisOptionsCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public SetAxisLabelAttachedPropertyCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartAxisModel;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(axisModel.Label.Label, historyItem.NewValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartAxisModel axisModel = GetAxisModel(historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(axisModel.Label.Label, historyItem.OldValue);
			return axisModel != null ? axisModel.Axis : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)parameter;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				CreateLabelIfNeeded();
				if (axisModel != SelectedAxisModel)
					axisModel = SelectedAxisModel;
				object targetObject = axisModel.Label.Label;
				object oldValue = commandParameter.GetAttachedProperty(targetObject);
				commandParameter.SetAttachedProperty(targetObject, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(axisIndexKey, axisModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, axisModel.Axis, oldValue, commandParameter.Value), axisModel.Axis);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = GetAxis(chartControl, historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			commandParameter.SetAttachedProperty(axis.Label, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			PropertyGridCommandParameterAttached commandParameter = (PropertyGridCommandParameterAttached)historyItem.ExecuteCommandInfo.Parameter;
			IModelItem attachedPropertyItem = axisItem.Properties["Label"].Value;
			DXPropertyIdentifier propIdentifier = new DXPropertyIdentifier(commandParameter.PropertyOwnerType, commandParameter.PropertyName);
			attachedPropertyItem.Properties[propIdentifier].SetValue(historyItem.NewValue);
		}
	}
	public class SetChartTitlePropertyCommand : PropertyGridCommandBase {
		const string titleIndexKey = "Title";
		public SetChartTitlePropertyCommand(WpfChartModel model) : base(model) {
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartTitleModel;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int titleIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey];
			WpfChartTitleModel titleModel = (WpfChartTitleModel)ChartModel.TitleCollectionModel.ModelCollection[titleIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			titleModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return titleModel != null ? titleModel.Title : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int titleIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey];
			WpfChartTitleModel titleModel = (WpfChartTitleModel)ChartModel.TitleCollectionModel.ModelCollection[titleIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			titleModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return titleModel != null ? titleModel.Title : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			WpfChartTitleModel titleModel = (WpfChartTitleModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				object oldValue = titleModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(titleIndexKey, titleModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), titleModel.Title);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int titelIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey];
			Title title = chartControl.Titles[titelIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(title, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int titelIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey];
			IModelItem titleItem = chartModelItem.Properties["Titles"].Collection[titelIndex];
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(titleItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetConstantLinePropertyCommand : ConstantLineCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public SetConstantLinePropertyCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartConstantLineModel constantLineModel = GetConstantLineModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			constantLineModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return constantLineModel != null ? constantLineModel.ConstantLine : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartConstantLineModel constantLineModel = GetConstantLineModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			constantLineModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return constantLineModel != null ? constantLineModel.ConstantLine : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				object oldValue = SelectedConstantLineModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = GetPathIndexItemList();
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), SelectedConstantLineModel.ConstantLine);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(constantLine, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem constantLineItem = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(constantLineItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetStripPropertyCommand : StripCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public SetStripPropertyCommand(WpfChartModel chartModel) : base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartStripModel stripModel = GetStripModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			stripModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return stripModel != null ? stripModel.Strip : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartStripModel stripModel = GetStripModel(historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			stripModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return stripModel != null ? stripModel.Strip : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				object oldValue = SelectedStripModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = GetPathIndexItemList();
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), SelectedStripModel.Strip);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Strip strip = GetTargetStripForRuntimeApply(chartControl, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(strip, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem stripItem = GetTargetStripForDesigntimeApply(chartModelItem, historyItem);
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(stripItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class SetPanePropertyCommand : PropertyGridCommandBase {
		const string paneIndexKey = "Pane";
		public SetPanePropertyCommand(WpfChartModel model) : base(model) {
		}
		void CreatePaneIfNeeded() {
			if (((XYDiagram2D)ChartModel.DiagramModel.Diagram).DefaultPane == null) {
				ICommand command = new AddDefaultPaneCommand(ChartModel);
				command.Execute(null);
			}
		}
		protected override bool CanExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			return commandParameter != null && commandParameter.Model is WpfChartPaneModel;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey];
			WpfChartPaneModel paneModel = (paneIndex >= 0) ? (WpfChartPaneModel)ChartModel.DiagramModel.PanesCollectionModel.ModelCollection[paneIndex] : ChartModel.DiagramModel.DefaultPaneModel;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			paneModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.OldValue);
			return paneModel != null ? paneModel.Pane : null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey];
			WpfChartPaneModel paneModel = (paneIndex >= 0) ? (WpfChartPaneModel)ChartModel.DiagramModel.PanesCollectionModel.ModelCollection[paneIndex] : ChartModel.DiagramModel.DefaultPaneModel;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			paneModel.SetChartElementProperty(commandParameter.PropertyName, historyItem.NewValue);
			return paneModel != null ? paneModel.Pane : null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)parameter;
			WpfChartPaneModel paneModel = (WpfChartPaneModel)commandParameter.Model;
			if (!String.IsNullOrEmpty(commandParameter.PropertyName)) {
				if (paneModel.GetSelfIndex() < 0) {
					CreatePaneIfNeeded();
					paneModel = ChartModel.SelectedModel as WpfChartPaneModel;
				}
				object oldValue = paneModel.SetChartElementProperty(commandParameter.PropertyName, commandParameter.Value);
				ElementIndexItem[] indexItems = new ElementIndexItem[1];
				indexItems[0] = new ElementIndexItem(paneIndexKey, paneModel.GetSelfIndex());
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, commandParameter.Value), paneModel.Pane);
			}
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey];
			XYDiagram2D diagram = chartControl.Diagram as XYDiagram2D;
			Pane pane = paneIndex >= 0 ? diagram.Panes[paneIndex] : diagram.DefaultPane;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetObjectProperty(pane, commandParameter.PropertyName, historyItem.NewValue);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey];
			IModelItem paneItem = paneIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Panes"].Collection[paneIndex] : chartModelItem.Properties["Diagram"].Value.Properties["DefaultPane"].Value;
			PropertyGridCommandParameter commandParameter = (PropertyGridCommandParameter)historyItem.ExecuteCommandInfo.Parameter;
			CommandUtils.SetModelItemProperty(paneItem, commandParameter.PropertyName, commandParameter.Value);
		}
	}
	public class AddDefaultPaneCommand : ChartCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public AddDefaultPaneCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)PreviewChart.Diagram;
			diagram.DefaultPane = null;
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)PreviewChart.Diagram;
			Pane redoPane = (Pane)historyItem.NewValue;
			diagram.DefaultPane = redoPane;
			return redoPane;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem diagram = chartModelItem.Properties["Diagram"].Value;
			IModelItem pane = chartModelItem.Context.CreateItem(typeof(Pane));
			diagram.Properties["DefaultPane"].SetValue(pane);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)chartControl.Diagram;
			diagram.DefaultPane = new Pane();
		}
		public override CommandResult RuntimeExecute(object parameter) {
			XYDiagram2D diagram = (XYDiagram2D)PreviewChart.Diagram;
			Pane pane = new Pane();
			diagram.DefaultPane = pane;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(info, this, diagram, null, pane);
			CommandResult result = new CommandResult(historyItem, pane);
			return result;
		}
	}
}
