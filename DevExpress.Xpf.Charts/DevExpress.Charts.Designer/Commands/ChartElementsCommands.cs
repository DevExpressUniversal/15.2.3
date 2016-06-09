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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Charts.Designer.Native {
	#region Create Legend Command
	public class CreateLegendCommand : ChartCommandBase {
		public override string Caption {
			get { throw null; }
		}
		public override string ImageName {
			get { throw null; }
		}
		public CreateLegendCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.LegendModel == null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			Legend newLegend = new Legend();
			HistoryItem historyItem = new HistoryItem(info, this, PreviewChart, PreviewChart.Legend, newLegend);
			ChartModel.Chart.Legend = newLegend;
			return new CommandResult(historyItem, newLegend);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Legend = new Legend();
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Legend"].SetValue(new Legend());
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.OldValue != null)
				PreviewChart.Legend = (Legend)historyItem.OldValue;
			else
				PreviewChart.Legend = null;
			return PreviewChart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Legend = (Legend)historyItem.NewValue;
			return PreviewChart.Legend;
		}
	}
	#endregion
	#region Add Secondary Axes Commands
	public class AddSecondaryAxisX : ChartCommandBase {
		const string axisIndexKey = "Axis";
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddSecondaryAxisX);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return GlyphUtils.BarItemImages + "AxisX"; }
		}
		public AddSecondaryAxisX(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.Chart.Diagram is XYDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			SecondaryAxisX2D axis = new SecondaryAxisX2D();
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			diagram.SecondaryAxesX.Add(axis);
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, new ElementIndexItem(axisIndexKey, diagram.SecondaryAxesX.Count-1));
			HistoryItem historyItem = new HistoryItem(info, this, diagram, oldValue: null, newValue: axis);
			return new CommandResult(historyItem, axis);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)chartControl.Diagram;
			diagram.SecondaryAxesX.Add(new SecondaryAxisX2D());
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axis = chartModelItem.Context.CreateItem(typeof(SecondaryAxisX2D));
			IModelItem diagram = chartModelItem.Properties["Diagram"].Value;
			diagram.Properties["SecondaryAxesX"].Collection.Add(axis);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			SecondaryAxisX2D axis = (SecondaryAxisX2D)historyItem.NewValue;
			diagram.SecondaryAxesX.Remove(axis);
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			SecondaryAxisX2D axis = (SecondaryAxisX2D)historyItem.NewValue;
			diagram.SecondaryAxesX.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey], axis);
			return axis;
		}
	}
	public class AddSecondaryAxisY : ChartCommandBase {
		const string axisIndexKey = "Axis";
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddSecondaryAxisY);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return GlyphUtils.BarItemImages + "AxisY"; }
		}
		public AddSecondaryAxisY(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.Chart.Diagram is XYDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			SecondaryAxisY2D axis = new SecondaryAxisY2D();
			diagram.SecondaryAxesY.Add(axis);
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, new ElementIndexItem(axisIndexKey, diagram.SecondaryAxesY.Count - 1));
			HistoryItem historyItem = new HistoryItem(info, this, diagram, oldValue: null, newValue: axis);
			return new CommandResult(historyItem, axis);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)chartControl.Diagram;
			diagram.SecondaryAxesY.Add(new SecondaryAxisY2D());
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axis = chartModelItem.Context.CreateItem(typeof(SecondaryAxisY2D));
			IModelItem diagram = chartModelItem.Properties["Diagram"].Value;
			diagram.Properties["SecondaryAxesY"].Collection.Add(axis);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			SecondaryAxisY2D axis = (SecondaryAxisY2D)historyItem.NewValue;
			diagram.SecondaryAxesY.Remove(axis);
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			SecondaryAxisY2D axis = (SecondaryAxisY2D)historyItem.NewValue;
			diagram.SecondaryAxesY.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey], axis);
			return axis;
		}
	}
	#endregion
	#region Add Constant Lines Commands
	public abstract class AddConstantLineBaseCommand : ConstantLineCommandBase {
		protected WpfChartAxisModel SelectedAxisModel { get { return ChartModel.SelectedModel as WpfChartAxisModel; } }
		public AddConstantLineBaseCommand(WpfChartModel chartModel) :
			base(chartModel) { }
		object GetConstantLineValueInCenterOfAxis(Axis2D axis) {
			double minValue = axis.ActualWholeRange.ActualMinValueInternal;
			double maxValue = axis.ActualWholeRange.ActualMaxValueInternal;
			double valueInternal = minValue + (maxValue - minValue) / 2;
			IScaleMap map = ChartDesignerPropertiesProvider.GetAxisScaleTypeMap(axis);
			object value = map.InternalToNative(valueInternal);
			return value;
		}
		protected ConstantLine CreateConstantLine(Axis2D axis) {
			object value = GetConstantLineValueInCenterOfAxis(axis);
			ConstantLine constantLine = new ConstantLine();
			constantLine.Value = value;
			constantLine.Title = new ConstantLineTitle() { Content =ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultConstantLineTitle) }; 
			return constantLine;
		}
		protected IModelItem CreateConstantLineModelItem(IModelItem axis, IModelItem chart, object value) {
			IModelItem constantLine = chart.Context.CreateItem(typeof(ConstantLine));
			constantLine.Properties["Value"].SetValue(value);
			string defaultConstantLineTitle = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultConstantLineTitle);
			IModelItem constantLineTitle = chart.Context.CreateItem(typeof(ConstantLineTitle));
			constantLineTitle.Properties["Content"].SetValue(defaultConstantLineTitle);
			constantLine.Properties["Title"].SetValue(constantLineTitle);
			return constantLine;
		}
		protected int GetAxisIndex() {
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			return axisIndex;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Axis2D target = GetTargetAxisForRuntimeApply(chartControl, historyItem);
			ConstantLine constantLine = CreateConstantLine(target);
			target.ConstantLinesInFront.Add(constantLine);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetTargetAxisForDesigntimeApply(chartModelItem, historyItem);
			IModelItem constantLine = CreateConstantLineModelItem(axisModelItem, chartModelItem, historyItem.ExecuteCommandInfo.AdditionalInfo);
			axisModelItem.Properties["ConstantLinesInFront"].Collection.Add(constantLine);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Axis2D target = (Axis2D)historyItem.TargetObject;
			target.ConstantLinesInFront.Remove((ConstantLine)historyItem.NewValue);
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Axis2D target = (Axis2D)historyItem.TargetObject;
			target.ConstantLinesInFront.Add((ConstantLine)historyItem.NewValue);
			return historyItem.NewValue;
		}
	}
	public class AddConstantLineXCommand : AddConstantLineBaseCommand {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddConstantLineX);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddConstantLineXDescription);
		public override string Caption { 
			get { return caption; } 
		}
		public override string ImageName { 
			get { return GlyphUtils.GalleryItemImages +  "AddConstantLine\\AddConstantLineX"; } 
		}
		public override string Description {
			get { return description; }
		}
		public AddConstantLineXCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.Chart.Diagram is XYDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			AxisX2D axis;
			if (parameter == null) {
				if (diagram.AxisX == null) {
					AddAxisXCommand addAxisCommand = new AddAxisXCommand(ChartModel);
					CommandResult addAxisResult = addAxisCommand.RuntimeExecute(parameter: null);
					compositeHistoryItem.HistoryItems.Add(addAxisResult.HistoryItem);
				}
				axis = diagram.AxisX;
			}
			else
				axis = (AxisX2D)parameter;
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList(axis);
			ConstantLine constantLine = CreateConstantLine(axis);
			axis.ConstantLinesInFront.Add(constantLine);
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, constantLine.Value, pathIndexItems);
			HistoryItem historyItem = new HistoryItem(info, this, axis, null, constantLine);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem, constantLine);
			return result;
		}
	}
	public class AddConstantLineYCommand : AddConstantLineBaseCommand {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddConstantLineY);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddConstntLineYDescription);
		public override string Caption { 
			get { return caption; } 
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AddConstantLine\\AddConstantLineY"; } 
		}
		public override string Description {
			get { return description; }
		}
		public AddConstantLineYCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.Chart.Diagram is XYDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.Chart.Diagram;
			AxisY2D axis;
			if (parameter == null) {
				if (diagram.AxisY == null) {
					AddAxisYCommand addAxisCommand = new AddAxisYCommand(ChartModel);
					CommandResult addAxisResult = addAxisCommand.RuntimeExecute(parameter: null);
					compositeHistoryItem.HistoryItems.Add(addAxisResult.HistoryItem);
				}
				axis = diagram.AxisY;
			}
			else
				axis = (AxisY2D)parameter;
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList(axis);
			ConstantLine constantLine = CreateConstantLine(axis);
			axis.ConstantLinesInFront.Add(constantLine);
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, constantLine.Value, pathIndexItems);
			HistoryItem historyItem = new HistoryItem(info, this, axis, null, constantLine);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem, constantLine);
			return result;
		}
	}
	#endregion
	#region Add Primary Axes Commands
	public class AddAxisXCommand : ChartCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public AddAxisXCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			Diagram diagram = ChartModel.Chart.Diagram;
			return 
				diagram is XYDiagram2D	||
				diagram is XYDiagram3D	||
				diagram is PolarDiagram2D ||
				diagram is RadarDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Diagram diagram = PreviewChart.Diagram;
			AxisBase axis;
			if (diagram is XYDiagram2D){
				axis = new AxisX2D();
				((XYDiagram2D)diagram).AxisX = (AxisX2D)axis;
			}
			else if (diagram is PolarDiagram2D){
				axis = new PolarAxisX2D();
				((PolarDiagram2D)diagram).AxisX = (PolarAxisX2D)axis;
			}
			else if (diagram is RadarDiagram2D){
				axis = new RadarAxisX2D();
				((RadarDiagram2D)diagram).AxisX = (RadarAxisX2D)axis;
			}
			else if (diagram is XYDiagram3D){
				axis = new AxisX3D();
				 ((XYDiagram3D)diagram).AxisX = (AxisX3D)axis;
			}
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' has no axes or it is unknown diagram type.");
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(info, this, diagram, null, axis);
			CommandResult result = new CommandResult(historyItem, axis);
			return result;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Diagram diagram = chartControl.Diagram;
			if (diagram is XYDiagram2D)
				((XYDiagram2D)diagram).AxisX = new AxisX2D();
			else if (diagram is PolarDiagram2D)
				((PolarDiagram2D)diagram).AxisX = new PolarAxisX2D();
			else if (diagram is RadarDiagram2D)
				((RadarDiagram2D)diagram).AxisX = new RadarAxisX2D();
			else if (diagram is XYDiagram3D)
				 ((XYDiagram3D)diagram).AxisX = new AxisX3D();
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' can not have axes or it is unknown diagram type.");
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem diagram = chartModelItem.Properties["Diagram"].Value;
			Type diagramType = diagram.ItemType;
			IModelItem axis;
			if (diagramType == typeof(XYDiagram2D))
				axis = chartModelItem.Context.CreateItem(typeof(AxisX2D));
			else if (diagramType == typeof(PolarDiagram2D))
				axis = chartModelItem.Context.CreateItem(typeof(PolarAxisX2D));
			else if (diagramType == typeof(RadarDiagram2D))
				axis = chartModelItem.Context.CreateItem(typeof(RadarAxisX2D));
			else if (diagramType == typeof(XYDiagram3D))
				axis = chartModelItem.Context.CreateItem(typeof(AxisX3D));
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "'  can not have axes or it is unknown diagram type.");
			diagram.Properties["AxisX"].SetValue(axis);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			AxisBase selectedAxis = null;
			Diagram diagram = PreviewChart.Diagram;
			if (diagram is XYDiagram2D){
				((XYDiagram2D)diagram).AxisX = null;
				selectedAxis = ((XYDiagram2D)diagram).ActualAxisX;
			}
			else if (diagram is PolarDiagram2D){
				((PolarDiagram2D)diagram).AxisX = null;
				selectedAxis = ((PolarDiagram2D)diagram).ActualAxisX;
			}
			else if (diagram is RadarDiagram2D){
				((RadarDiagram2D)diagram).AxisX = null;
				selectedAxis = ((RadarDiagram2D)diagram).ActualAxisX;
			}
			else if (diagram is XYDiagram3D){
				((XYDiagram3D)diagram).AxisX = null;
				selectedAxis = ((XYDiagram3D)diagram).ActualAxisX;
			}
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' can not have axes or it is unknown diagram type.");
			return selectedAxis;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Diagram diagram = PreviewChart.Diagram;
			if (diagram is XYDiagram2D)
				((XYDiagram2D)diagram).AxisX = (AxisX2D)historyItem.NewValue;
			else if (diagram is PolarDiagram2D)
				((PolarDiagram2D)diagram).AxisX = (PolarAxisX2D)historyItem.NewValue;
			else if (diagram is RadarDiagram2D)
				((RadarDiagram2D)diagram).AxisX = (RadarAxisX2D)historyItem.NewValue;
			else if (diagram is XYDiagram3D)
				((XYDiagram3D)diagram).AxisX = (AxisX3D)historyItem.NewValue;
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' can not have axes or it is unknown diagram type.");
			return historyItem.NewValue;
		}
	}
	public class AddAxisYCommand : ChartCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public AddAxisYCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			Diagram diagram = ChartModel.Chart.Diagram;
			return
				diagram is XYDiagram2D ||
				diagram is XYDiagram3D ||
				diagram is PolarDiagram2D ||
				diagram is RadarDiagram2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Diagram diagram = PreviewChart.Diagram;
			AxisBase axis;
			if (diagram is XYDiagram2D) {
				axis = new AxisY2D();
				((XYDiagram2D)diagram).AxisY = (AxisY2D)axis;
			}
			else if (diagram is PolarDiagram2D) {
				axis = new PolarAxisY2D();
				((PolarDiagram2D)diagram).AxisY = (PolarAxisY2D)axis;
			}
			else if (diagram is RadarDiagram2D) {
				axis = new RadarAxisY2D();
				((RadarDiagram2D)diagram).AxisY = (RadarAxisY2D)axis;
			}
			else if (diagram is XYDiagram3D) {
				axis = new AxisY3D();
				((XYDiagram3D)diagram).AxisY = (AxisY3D)axis;
			}
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' has no axes or it is unknown diagram type.");
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(info, this, diagram, null, axis);
			CommandResult result = new CommandResult(historyItem, axis);
			return result;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Diagram diagram = chartControl.Diagram;
			if (diagram is XYDiagram2D)
				((XYDiagram2D)diagram).AxisY = new AxisY2D();
			else if (diagram is PolarDiagram2D)
				((PolarDiagram2D)diagram).AxisY = new PolarAxisY2D();
			else if (diagram is RadarDiagram2D)
				((RadarDiagram2D)diagram).AxisY = new RadarAxisY2D();
			else if (diagram is XYDiagram3D)
				((XYDiagram3D)diagram).AxisY = new AxisY3D();
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' can not have axes or it is unknown diagram type.");
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem diagram = chartModelItem.Properties["Diagram"].Value;
			Type diagramType = diagram.ItemType;
			IModelItem axis;
			if (diagramType == typeof(XYDiagram2D))
				axis = chartModelItem.Context.CreateItem(typeof(AxisY2D));
			else if (diagramType == typeof(PolarDiagram2D))
				axis = chartModelItem.Context.CreateItem(typeof(PolarAxisY2D));
			else if (diagramType == typeof(RadarDiagram2D))
				axis = chartModelItem.Context.CreateItem(typeof(RadarAxisY2D));
			else if (diagramType == typeof(XYDiagram3D))
				axis = chartModelItem.Context.CreateItem(typeof(RadarAxisY2D));
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "'  can not have axes or it is unknown diagram type.");
			diagram.Properties["AxisY"].SetValue(axis);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			AxisBase selectedAxis = null;
			Diagram diagram = PreviewChart.Diagram;
			if (diagram is XYDiagram2D) {
				((XYDiagram2D)diagram).AxisY = null;
				selectedAxis = ((XYDiagram2D)diagram).ActualAxisY;
			}
			else if (diagram is PolarDiagram2D) {
				((PolarDiagram2D)diagram).AxisY = null;
				selectedAxis = ((PolarDiagram2D)diagram).ActualAxisY;
			}
			else if (diagram is RadarDiagram2D) {
				((RadarDiagram2D)diagram).AxisY = null;
				selectedAxis = ((RadarDiagram2D)diagram).ActualAxisY;
			}
			else if (diagram is XYDiagram3D) {
				((XYDiagram3D)diagram).AxisY = null;
				selectedAxis = ((XYDiagram3D)diagram).ActualAxisY;
			}
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' can not have axes or it is unknown diagram type.");
			return selectedAxis;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Diagram diagram = PreviewChart.Diagram;
			if (diagram is XYDiagram2D)
				((XYDiagram2D)diagram).AxisY = (AxisY2D)historyItem.NewValue;
			else if (diagram is PolarDiagram2D)
				((PolarDiagram2D)diagram).AxisY = (PolarAxisY2D)historyItem.NewValue;
			else if (diagram is RadarDiagram2D)
				((RadarDiagram2D)diagram).AxisY = (RadarAxisY2D)historyItem.NewValue;
			else if (diagram is XYDiagram3D)
				((XYDiagram3D)diagram).AxisY = (AxisY3D)historyItem.NewValue;
			else
				throw new ChartDesignerException("Diagram type '" + diagram.GetType().Name + "' can not have axes or it is unknown diagram type.");
			return historyItem.NewValue;
		}
	}
	#endregion
	#region Add Strip Commands
	public abstract class AddStripCommandBase : ChartCommandBase {
		protected WpfChartAxisModel SelectedAxisModel { get { return ChartModel.SelectedModel as WpfChartAxisModel; } }
		public AddStripCommandBase(WpfChartModel chartModel)
			: base(chartModel) { }
		void CalculateMinMaxStripLimits(Axis2D axis, out object minStripValue, out object maxStripValue) {
			double minValueInternal = axis.ActualWholeRange.ActualMinValueInternal;
			double maxValueInternal = axis.ActualWholeRange.ActualMaxValueInternal;
			double rangeLengthInternal = maxValueInternal - minValueInternal;
			double centralValueInternal = minValueInternal + rangeLengthInternal / 2;
			double minStripValueInternal = centralValueInternal - 0.15 * rangeLengthInternal;
			double maxStripValueInternal = centralValueInternal + 0.15 * rangeLengthInternal;
			IScaleMap map = ChartDesignerPropertiesProvider.GetAxisScaleTypeMap(axis);
			minStripValue = map.InternalToNative(minStripValueInternal);
			maxStripValue = map.InternalToNative(maxStripValueInternal);
		}
		protected Strip CreateStrip(Axis2D axis) {
			Strip strip = new Strip();
			object min;
			object max;
			CalculateMinMaxStripLimits(axis, out min, out max);
			strip.MinLimit = min;
			strip.MaxLimit = max;
			strip.LegendText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultStripLegendText);
			return strip;
		}
		protected IModelItem CreateStripModelItem(IModelItem axis, IModelItem chartModelItem, StripLimits limits) {
			IModelItem strip = chartModelItem.Context.CreateItem(typeof(Strip));
			strip.Properties["MinLimit"].SetValue(limits.MinLimit);
			strip.Properties["MaxLimit"].SetValue(limits.MaxLimit);
			return strip;
		}
		protected int GetAxisIndex() {
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			return axisIndex;
		}
	}
	public class AddStripXCommand : AddStripCommandBase {
		const string stripIndexKey = "Strip";
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddStripX);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddStripXDescription);
		string imageName = GlyphUtils.GalleryItemImages + "AddStrip\\AddStripX";
		public AddStripXCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public override string Description {
			get { return description; }
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.Chart.Diagram is XYDiagram2D)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			XYDiagram2D xyDiagram2D = (XYDiagram2D)ChartModel.Chart.Diagram;
			if (xyDiagram2D.AxisX == null) {
				AddAxisXCommand command = new AddAxisXCommand(ChartModel);
				IHistoryItem addAxisHistoryItem = command.RuntimeExecute(parameter: null).HistoryItem;
				compositeHistoryItem.HistoryItems.Add(addAxisHistoryItem);
			}
			AxisX2D axis = xyDiagram2D.AxisX;
			Strip strip = CreateStrip(axis);
			axis.Strips.Add(strip);
			int index = axis.Strips.Count - 1;
			StripLimits limits = new StripLimits() { MinLimit = strip.MinLimit, MaxLimit = strip.MaxLimit };
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, limits, new ElementIndexItem(stripIndexKey, index));
			HistoryItem historyItem = new HistoryItem(info, this, axis, null, strip);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem, strip);
			return result; 
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D xyDiagram2D = (XYDiagram2D)PreviewChart.Diagram;
			xyDiagram2D.AxisX.Strips.Remove((Strip)historyItem.NewValue);
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D xyDiagram2D = (XYDiagram2D)PreviewChart.Diagram;
			Strip strip = (Strip)historyItem.NewValue;
			xyDiagram2D.AxisX.Strips.Add(strip);
			return strip;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D xyDiagram2D = (XYDiagram2D)chartControl.Diagram;
			AxisX2D axisX2D = xyDiagram2D.AxisX;
			Strip strip = CreateStrip(axisX2D);
			xyDiagram2D.AxisX.Strips.Add(strip);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis = xyDiagram2D.Properties["AxisX"].Value;
			IModelItem strip = CreateStripModelItem(axis, chartModelItem, (StripLimits)historyItem.ExecuteCommandInfo.AdditionalInfo);
			axis.Properties["Strips"].Collection.Add(strip);
		}
	}
	public class AddStripYCommand : AddStripCommandBase {
		const string stripIndexKey = "Strip";
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddStripY);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddStripYDescription);
		string imageName = GlyphUtils.GalleryItemImages + "AddStrip\\AddStripY";
		public AddStripYCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public override string Description {
			get { return description; }
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.Chart.Diagram is XYDiagram2D)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			XYDiagram2D xyDiagram2D = (XYDiagram2D)ChartModel.Chart.Diagram;
			if (xyDiagram2D.AxisY == null) {
				AddAxisYCommand command = new AddAxisYCommand(ChartModel);
				IHistoryItem addAxisHistoryItem = command.RuntimeExecute(parameter: null).HistoryItem;
				compositeHistoryItem.HistoryItems.Add(addAxisHistoryItem);
			}
			AxisY2D axis = xyDiagram2D.AxisY;
			Strip strip = CreateStrip(axis);
			axis.Strips.Add(strip);
			int index = axis.Strips.Count - 1;
			StripLimits limits = new StripLimits() { MinLimit = strip.MinLimit, MaxLimit = strip.MaxLimit };
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, limits, new ElementIndexItem(stripIndexKey, index));
			HistoryItem historyItem = new HistoryItem(info, this, axis, null, strip);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem, strip);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D xyDiagram2D = (XYDiagram2D)PreviewChart.Diagram;
			xyDiagram2D.AxisY.Strips.Remove((Strip)historyItem.NewValue);
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D xyDiagram2D = (XYDiagram2D)PreviewChart.Diagram;
			Strip strip = (Strip)historyItem.NewValue;
			xyDiagram2D.AxisY.Strips.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[stripIndexKey], strip);
			return strip;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D xyDiagram2D = (XYDiagram2D)chartControl.Diagram;
			AxisY2D axisY2D = xyDiagram2D.AxisY;
			Strip strip = CreateStrip(axisY2D);
			xyDiagram2D.AxisY.Strips.Add(strip);
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis = xyDiagram2D.Properties["AxisY"].Value;
			IModelItem strip = CreateStripModelItem(axis, chartModelItem, (StripLimits)historyItem.ExecuteCommandInfo.AdditionalInfo);
			axis.Properties["Strips"].Collection.Add(strip);
		}
	}
	#endregion
	#region Add Pane Commands
	public abstract class AddPaneCommandBase : ChartCommandBase {
		const string paneIndexKey = "Pane";
		Orientation paneOrientation;
		public AddPaneCommandBase(WpfChartModel chartModel, Orientation paneOrientation)
			: base(chartModel) {
			this.paneOrientation = paneOrientation;
		}
		protected override bool CanExecute(object parameter) {
			return ChartModel.Chart.Diagram is XYDiagram2D;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)PreviewChart.Diagram;
			diagram.Panes.Remove((Pane)historyItem.NewValue);
			return ChartModel.Chart;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)PreviewChart.Diagram;
			Pane redoPane = (Pane)historyItem.NewValue;
			diagram.Panes.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[paneIndexKey], redoPane);
			return redoPane;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem diagram = chartModelItem.Properties["Diagram"].Value;
			diagram.Properties["PaneOrientation"].SetValue(this.paneOrientation);
			IModelItem pane = chartModelItem.Context.CreateItem(typeof(Pane));
			diagram.Properties["Panes"].Collection.Add(pane);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			XYDiagram2D diagram = (XYDiagram2D)chartControl.Diagram;
			diagram.PaneOrientation = this.paneOrientation;
			Pane pane = new Pane();
			diagram.Panes.Add(pane);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			XYDiagram2D diagram = (XYDiagram2D)PreviewChart.Diagram;
			ChartModel.DiagramModel.PaneOrientation = this.paneOrientation;
			Pane pane = new Pane();
			diagram.Panes.Add(pane);
			int index = diagram.Panes.Count - 1;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, new ElementIndexItem(paneIndexKey, index));
			HistoryItem historyItem = new HistoryItem(info, this, diagram, null, pane);
			CommandResult result = new CommandResult(historyItem, pane);
			return result;
		}
	}
	public class AddPaneHorizontalCommand : AddPaneCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddPaneHorizontal);
		string imageName = GlyphUtils.GalleryItemImages + "AddPane/Horizontal";
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddPaneHorizontalDescription);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public override string Description {
			get { return description; }
		} 
		public AddPaneHorizontalCommand(WpfChartModel chartModel)
			: base(chartModel, Orientation.Horizontal) { }
	}
	public class AddPaneVerticalCommand : AddPaneCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddPaneVertical);
		string imageName = GlyphUtils.GalleryItemImages + "AddPane/Vertical";
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddPaneVerticalDescription);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public override string Description {
			get { return description; }
		}
		public AddPaneVerticalCommand(WpfChartModel chartModel)
			: base(chartModel, Orientation.Vertical) { }
	}
	#endregion
	#region Add Chart Title Command
	public class AddChartTitleCommand : ChartCommandBase {
		const string titleIndexKey = "Title";
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Elements_AddChartTitle);
		Dock dock;
		HorizontalAlignment horizontalAlignment;
		VerticalAlignment verticalAlignment;
		public override string Caption { get { return caption; } }
		public override string ImageName { get { return GlyphUtils.ElementsPageImages + "AddTitle"; } }
		public AddChartTitleCommand(WpfChartModel chartModel, Dock dock, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
			: base(chartModel) {
			this.dock = dock;
			this.horizontalAlignment = horizontalAlignment;
			this.verticalAlignment = verticalAlignment;
		}
		public AddChartTitleCommand(WpfChartModel wpfChartModel, Dock dock, HorizontalAlignment horizontalAlignment)
			: base(wpfChartModel) {
			this.dock = dock;
			this.horizontalAlignment = horizontalAlignment;
		}
		public AddChartTitleCommand(WpfChartModel wpfChartModel, Dock dock, VerticalAlignment verticalAlignment)
			: base(wpfChartModel) {
			this.dock = dock;
			this.verticalAlignment = verticalAlignment;
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Titles.Remove((Title)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Title title = (Title)historyItem.NewValue;
			PreviewChart.Titles.Insert(historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey], title);
			return title;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem title = chartModelItem.Context.CreateItem(typeof(Title));
			title.Properties["Dock"].SetValue(this.dock);
			title.Properties["HorizontalAlignment"].SetValue(this.horizontalAlignment);
			title.Properties["VerticalAlignment"].SetValue(this.verticalAlignment);
			string localizedContent = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultChartTitleContent);
			title.Properties["Content"].SetValue(localizedContent);
			chartModelItem.Properties["Titles"].Collection.Add(title);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Title newTitle = new Title();
			newTitle.Dock = dock;
			newTitle.HorizontalAlignment = horizontalAlignment;
			newTitle.VerticalAlignment = verticalAlignment;
			string localizedContent = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultChartTitleContent);
			newTitle.Content = localizedContent;
			chartControl.Titles.Add(newTitle);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Title newTitle = new Title();
			newTitle.Dock = dock;
			newTitle.HorizontalAlignment = horizontalAlignment;
			newTitle.VerticalAlignment = verticalAlignment;
			string localizedContent = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultChartTitleContent);
			newTitle.Content = localizedContent;
			PreviewChart.Titles.Add(newTitle);
			int index = PreviewChart.Titles.Count - 1;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, new ElementIndexItem(titleIndexKey, index));
			HistoryItem historyItem = new HistoryItem(info, this, PreviewChart, null, newTitle);
			CommandResult result = new CommandResult(historyItem, newTitle);
			return result;
		}
	}
	#endregion
	#region Add Indicator Command
	public class AddIndicatorCommand<indicatorType> : ChartCommandBase where indicatorType: Indicator{
		const string SeriesIndexKey = "Series";
		const string IndicatorIndexKey = "Indicator";
		const ValueLevel DefaultValueLevelForFinancialSeries = ValueLevel.High;
		readonly Dictionary<Type, string> imageMapping = new Dictionary<Type, string> {
			{typeof(RegressionLine), "RegressionLine"}, 
			{typeof(TrendLine), "TrendLine"},
			{typeof(FibonacciArcs),"FibonacciArcs"},
			{typeof(FibonacciFans), "FibonacciFans"},
			{typeof(FibonacciRetracement), "FibonacciRetracement"},
			{typeof(SimpleMovingAverage), "SimpleMovingAverage"},
			{typeof(WeightedMovingAverage), "WeightedMovingAverage"},
			{typeof(ExponentialMovingAverage), "ExponentialMovingAverage"},
			{typeof(TriangularMovingAverage), "TriangularMovingAverage"}
		};
		public override string Caption {
			get { return ChartDesignerLocalizer.GetLocalizedIndicatorTypeName(typeof(indicatorType)); }
		}
		public override string ImageName { 
			get { return null; }
		}
		public AddIndicatorCommand(WpfChartModel chartModel) 
			: base(chartModel){ }
		static void SetArgumentsAndValueLevels(XYSeries2D xySeries2D, Indicator indicator) {
			if (indicator is FinancialIndicator && xySeries2D.Points.Count > 1) {
				((FinancialIndicator)indicator).Argument1 = xySeries2D.Points[0].Argument;
				((FinancialIndicator)indicator).Argument2 = xySeries2D.Points[xySeries2D.Points.Count - 1].Argument;
				if (xySeries2D is FinancialSeries2D) {
					((FinancialIndicator)indicator).ValueLevel1 = DefaultValueLevelForFinancialSeries;
					((FinancialIndicator)indicator).ValueLevel2 = DefaultValueLevelForFinancialSeries;
				}
			}
			if (indicator is RegressionLine && xySeries2D is FinancialSeries2D)
				((RegressionLine)indicator).ValueLevel = DefaultValueLevelForFinancialSeries;
			if (indicator is MovingAverage && xySeries2D is FinancialSeries2D)
				((MovingAverage)indicator).ValueLevel = DefaultValueLevelForFinancialSeries;
		}
		protected override bool CanExecute(object parameter) {
			if (parameter is XYSeries2D || (ChartModel.SelectedModel is WpfChartSeriesModel && ((WpfChartSeriesModel)ChartModel.SelectedModel).Series is XYSeries2D))
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[IndicatorIndexKey];
			XYSeries2D xySeries2D = seriesIndex >= 0 ? (XYSeries2D)PreviewChart.Diagram.Series[seriesIndex] : (XYSeries2D)PreviewChart.Diagram.SeriesTemplate;
			xySeries2D.Indicators.Insert(indicatorIndex, (Indicator)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			int indicatorIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[IndicatorIndexKey];
			XYSeries2D xySeries2D = seriesIndex >= 0 ? (XYSeries2D)PreviewChart.Diagram.Series[seriesIndex] : (XYSeries2D)PreviewChart.Diagram.SeriesTemplate;
			xySeries2D.Indicators.RemoveAt(indicatorIndex);
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			IModelItem seriesItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			IModelItem indicatorItem = chartModelItem.Context.CreateItem(typeof(indicatorType));
			indicatorItem.Properties["ShowInLegend"].SetValue(true);
			indicatorItem.Properties["LegendText"].SetValue(ChartDesignerLocalizer.GetLocalizedIndicatorTypeName(typeof(indicatorType)));
			var xySeries2D =  (XYSeries2D)seriesItem.GetCurrentValue();
			if (typeof(indicatorType).IsSubclassOf(typeof(FinancialIndicator)) && xySeries2D.Points.Count > 1) {			   
				indicatorItem.Properties["Argument1"].SetValue(xySeries2D.Points[0].Argument);
				indicatorItem.Properties["Argument2"].SetValue(xySeries2D.Points[xySeries2D.Points.Count - 1].Argument);
				if (xySeries2D is FinancialSeries2D) {
				   indicatorItem.Properties["ValueLevel1"].SetValue(DefaultValueLevelForFinancialSeries);
				   indicatorItem.Properties["ValueLevel2"].SetValue(DefaultValueLevelForFinancialSeries);
				}
			}
			if (typeof(indicatorType).IsSubclassOf(typeof(RegressionLine)) && xySeries2D is FinancialSeries2D)
				indicatorItem.Properties["ValueLevel"].SetValue(DefaultValueLevelForFinancialSeries);
			if (typeof(indicatorType).IsSubclassOf(typeof(MovingAverage)) && xySeries2D is FinancialSeries2D)
				indicatorItem.Properties["ValueLevel"].SetValue(DefaultValueLevelForFinancialSeries);
			seriesItem.Properties["Indicators"].Collection.Add(indicatorItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			XYSeries2D xySeries2D = seriesIndex >= 0 ? (XYSeries2D)chartControl.Diagram.Series[seriesIndex] : (XYSeries2D)chartControl.Diagram.SeriesTemplate;
			Indicator indicator = Activator.CreateInstance<indicatorType>();
			indicator.ShowInLegend = true;
			indicator.LegendText = ChartDesignerLocalizer.GetLocalizedIndicatorTypeName(typeof(indicatorType));
			SetArgumentsAndValueLevels(xySeries2D, indicator);
			xySeries2D.Indicators.Add(indicator);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			XYSeries2D xySeries2D;
			if (parameter is XYSeries2D) 
				xySeries2D = (XYSeries2D)parameter;
			else if (ChartModel.SelectedModel is WpfChartSeriesModel && ((WpfChartSeriesModel)ChartModel.SelectedModel).Series is XYSeries2D)
				xySeries2D = (XYSeries2D)((WpfChartSeriesModel)ChartModel.SelectedModel).Series;
			else {
				ChartDebug.WriteWarning("The '" + GetType().Name + "' command can be executed because of parameter must be XYSeries2D");
				return null;
			}
			Indicator indicator = Activator.CreateInstance<indicatorType>();
			indicator.LegendText = ChartDesignerLocalizer.GetLocalizedIndicatorTypeName(typeof(indicatorType));
			indicator.ShowInLegend = true;
			SetArgumentsAndValueLevels(xySeries2D, indicator);
			xySeries2D.Indicators.Add(indicator);
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(SeriesIndexKey, PreviewChart.Diagram.Series.IndexOf(xySeries2D));
			indexItems[1] = new ElementIndexItem(IndicatorIndexKey, xySeries2D.Indicators.Count - 1);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, indexItems);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, null, indicator);
			CommandResult result = new CommandResult(historyItem, indicator);
			return result;
		}
	}
	#endregion
}
