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
using System.Security;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class AxisRangeCommandBase : AxisOptionsCommandBase {
		public AxisRangeCommandBase(WpfChartModel chartModel) :
			base(chartModel) { }
		protected CommandResult CreateWholeRangeIfNecessary(WpfChartAxisModel axisModel) {
			AxisBase axis = axisModel.Axis;
			CommandResult createVisibleRangeResult = null;
			if (axis is Axis && ((Axis)axis).WholeRange == null ||
				axis is PolarAxisY2D && ((PolarAxisY2D)axis).WholeRange == null ||
				axis is RadarAxisX2D && ((RadarAxisX2D)axis).WholeRange == null ||
				axis is RadarAxisY2D && ((RadarAxisY2D)axis).WholeRange == null)
				createVisibleRangeResult = new CreateAxisWholeRangeCommand(ChartModel).RuntimeExecute(null);
			return createVisibleRangeResult;
		}
		protected CommandResult CreateVisualRangeIfNecessary(WpfChartAxisModel axisModel) {
			AxisBase axis = axisModel.Axis;
			CommandResult createVisualRangeResult = null;
			if (axis is Axis2D && ((Axis2D)axis).VisualRange == null)
				createVisualRangeResult = new CreateAxisVisualRangeCommand(ChartModel).RuntimeExecute(null);
			return createVisualRangeResult;
		}
	}
	public class CreateAxisWholeRangeCommand : AxisRangeCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public CreateAxisWholeRangeCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		void SetRange(AxisBase axis, Range range) {
			if (axis is Axis)
				((Axis)axis).WholeRange = range;
			else if (axis is PolarAxisY2D)
				((PolarAxisY2D)axis).WholeRange = range;
			else if (axis is RadarAxisX2D)
				((RadarAxisX2D)axis).WholeRange = range;
			else if (axis is RadarAxisY2D)
				((RadarAxisY2D)axis).WholeRange = range;
			else
				throw new ChartDesignerException(axis.GetType().Name + " is uncknown axis type, which contains Range.");
		}
		Range GetVisibleRange(AxisBase axis) {
			if (axis is Axis)
				return ((Axis)axis).WholeRange;
			else if (axis is PolarAxisY2D)
				return ((PolarAxisY2D)axis).WholeRange;
			else if (axis is RadarAxisX2D)
				return ((RadarAxisX2D)axis).WholeRange;
			else if (axis is RadarAxisY2D)
				return ((RadarAxisY2D)axis).WholeRange;
			else
				throw new ChartDesignerException(axis.GetType().Name + " is uncknown axis type, which contains Range.");
		}
		protected override bool CanExecute(object parameter) {
			if (!(ChartModel.SelectedModel is WpfChartAxisModel))
				return false;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axisBase = axisModel.Axis;
			if (axisBase is Axis || axisBase is CircularAxisY2D || axisBase is RadarAxisX2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			object targetObject = historyItem.TargetObject;
			if (targetObject is Axis)
				((Axis)targetObject).WholeRange = (Range)historyItem.NewValue;
			else if (targetObject is PolarAxisY2D)
				((PolarAxisY2D)targetObject).WholeRange = (Range)historyItem.NewValue;
			else if (targetObject is RadarAxisX2D)
				((RadarAxisX2D)targetObject).WholeRange = (Range)historyItem.NewValue;
			else if (targetObject is RadarAxisY2D)
				((RadarAxisY2D)targetObject).WholeRange = (Range)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			object targetObject = historyItem.TargetObject;
			if (targetObject is Axis)
				((Axis)targetObject).WholeRange = (Range)historyItem.OldValue;
			else if (targetObject is PolarAxisY2D)
				((PolarAxisY2D)targetObject).WholeRange = (Range)historyItem.OldValue;
			else if (targetObject is RadarAxisX2D)
				((RadarAxisX2D)targetObject).WholeRange = (Range)historyItem.OldValue;
			else if (targetObject is RadarAxisY2D)
				((RadarAxisY2D)targetObject).WholeRange = (Range)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetAxisModelItem(chartModelItem, historyItem);
			IModelItem rangeModelItem = chartModelItem.Context.CreateItem(typeof(Range));
			axisModelItem.Properties["WholeRange"].SetValue(rangeModelItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			AxisBase axis = (AxisBase)historyItem.TargetObject;
			if (axis is SecondaryAxisX2D)
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[index].WholeRange = new Range();
			else if (axis is SecondaryAxisY2D)
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[index].WholeRange = new Range();
			else if (axis is AxisX2D)
				((XYDiagram2D)chartControl.Diagram).AxisX.WholeRange = new Range();
			else if (axis is AxisY2D)
				((XYDiagram2D)chartControl.Diagram).AxisY.WholeRange = new Range();
			else if (axis is AxisX3D)
				((XYDiagram3D)chartControl.Diagram).AxisX.WholeRange = new Range();
			else if (axis is AxisY3D)
				((XYDiagram3D)chartControl.Diagram).AxisY.WholeRange = new Range();
			else if (axis is PolarAxisY2D)
				((PolarDiagram2D)chartControl.Diagram).AxisY.WholeRange = new Range();
			else if (axis is RadarAxisX2D)
				((RadarDiagram2D)chartControl.Diagram).AxisX.WholeRange = new Range();
			else if (axis is RadarAxisY2D)
				((RadarDiagram2D)chartControl.Diagram).AxisY.WholeRange = new Range();
			else
				throw new ChartDesignerException(historyItem.TargetObject.GetType().Name + " is unknown axis type with Range.");
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			CommandResult createAxisResult = CreateAxisIfNecessary();
			if (createAxisResult != null)
				compositeHistoryItem.HistoryItems.Add(createAxisResult.HistoryItem);
			var axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axis = axisModel.Axis;
			Range oldValue = GetVisibleRange(axis);
			Range newValue = new Range();
			SetRange(axis, newValue);
			ElementIndexItem axisIndexItem = GetAxisIndexItem(axisModel, ChartModel.DiagramModel.Diagram);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, axisIndexItem);
			HistoryItem createRangeHistoryItem = new HistoryItem(execComInfo, this, axis, oldValue, newValue);
			compositeHistoryItem.HistoryItems.Add(createRangeHistoryItem);
			CommandResult result = new CommandResult(compositeHistoryItem);
			return result;
		}
	}
	public class CreateAxisVisualRangeCommand : AxisRangeCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public CreateAxisVisualRangeCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (!(ChartModel.SelectedModel is WpfChartAxisModel))
				return false;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axisBase = axisModel.Axis;
			if (axisBase is Axis2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((Axis2D)historyItem.TargetObject).VisualRange = (Range)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((Axis2D)historyItem.TargetObject).VisualRange = (Range)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetAxisModelItem(chartModelItem, historyItem);
			IModelItem rangeModelItem = chartModelItem.Context.CreateItem(typeof(Range));
			axisModelItem.Properties["VisualRange"].SetValue(rangeModelItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			AxisBase axis = (AxisBase)historyItem.TargetObject;
			if (axis.GetType() == typeof(AxisX2D))
				((XYDiagram2D)chartControl.Diagram).AxisX.VisualRange = new Range();
			else if (axis.GetType() == typeof(AxisY2D))
				((XYDiagram2D)chartControl.Diagram).AxisY.VisualRange = new Range();
			else if (axis.GetType() == typeof(SecondaryAxisX2D))
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[index].VisualRange = new Range();
			else if (axis.GetType() == typeof(SecondaryAxisY2D))
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[index].VisualRange = new Range();
			else
				throw new ChartDesignerException(historyItem.TargetObject.GetType().Name + " is unknown type with ScrollingRange.");
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			CommandResult createAxisResult = CreateAxisIfNecessary();
			if (createAxisResult != null)
				compositeHistoryItem.HistoryItems.Add(createAxisResult.HistoryItem);
			var axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			Axis2D axis = (Axis2D)axisModel.Axis;
			Range oldValue = axis.VisualRange;
			Range newValue = new Range();
			axis.VisualRange = newValue;
			ElementIndexItem axisIndexItem = GetAxisIndexItem(axisModel, ChartModel.DiagramModel.Diagram);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, axisIndexItem);
			HistoryItem createRangeHistoryItem = new HistoryItem(execComInfo, this, axis, oldValue, newValue);
			compositeHistoryItem.HistoryItems.Add(createRangeHistoryItem);
			CommandResult result = new CommandResult(compositeHistoryItem);
			return result;
		}
	}
	public class SetAxisVisualRangeMaxValueCommand : AxisRangeCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_VisibleRangeMaxValue);
		readonly ScaleType scaleType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ScaleType ScaleType {
			get { return scaleType; }
		}
		public SetAxisVisualRangeMaxValueCommand(WpfChartModel chartModel, ScaleType scaleType)
			: base(chartModel) {
			this.scaleType = scaleType;
		}
		protected override bool CanExecute(object parameter) {
			if (!(ChartModel.SelectedModel is WpfChartAxisModel))
				return false;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axisBase = axisModel.Axis;
			if (!(axisBase is Axis2D))
				return false;
			if (this.scaleType == axisModel.ScaleType)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.VisualRangeMaxValue = historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.VisualRangeMaxValue = historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetAxisModelItem(chartModelItem, historyItem);
			axisModelItem.Properties["VisualRange"].Value.Properties["MaxValue"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			AxisBase axis = ((WpfChartAxisModel)historyItem.TargetObject).Axis;
			if (axis is SecondaryAxisX2D)
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[index].VisualRange.MaxValue = historyItem.NewValue;
			else if (axis is SecondaryAxisY2D)
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[index].VisualRange.MaxValue = historyItem.NewValue;
			else if (axis is AxisX2D)
				((XYDiagram2D)chartControl.Diagram).AxisX.VisualRange.MaxValue = historyItem.NewValue;
			else if (axis is AxisY2D)
				((XYDiagram2D)chartControl.Diagram).AxisY.VisualRange.MaxValue = historyItem.NewValue;
			else
				throw new ChartDesignerException(historyItem.TargetObject.GetType().Name + " is unknown type with range.");
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			CommandResult createVisibleRangeResult = CreateVisualRangeIfNecessary((WpfChartAxisModel)ChartModel.SelectedModel);
			if (createVisibleRangeResult != null)
				compositeHistoryItem.HistoryItems.Add(createVisibleRangeResult.HistoryItem);
			var axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			object oldValue = axisModel.VisualRangeMaxValue;
			object newValue = parameter;
			axisModel.VisualRangeMaxValue = newValue;
			ElementIndexItem indexItem = GetAxisIndexItem(axisModel, ChartModel.DiagramModel.Diagram);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, indexItem);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, axisModel, oldValue, newValue);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem);
			return result;
		}
	}
	public class SetAxisVisualRangeMinValueCommand : AxisRangeCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_VisibleRangeMinValue);
		readonly ScaleType scaleType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ScaleType ScaleType {
			get { return scaleType; }
		}
		public SetAxisVisualRangeMinValueCommand(WpfChartModel chartModel, ScaleType scaleType)
			: base(chartModel) {
			this.scaleType = scaleType;
		}
		protected override bool CanExecute(object parameter) {
			if (!(ChartModel.SelectedModel is WpfChartAxisModel))
				return false;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axisBase = axisModel.Axis;
			if (!(axisBase is Axis || axisBase is CircularAxisY2D || axisBase is RadarAxisX2D))
				return false;
			if (this.scaleType == axisModel.ScaleType)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.VisualRangeMinValue = historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.VisualRangeMinValue = historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetAxisModelItem(chartModelItem, historyItem);
			axisModelItem.Properties["VisualRange"].Value.Properties["MinValue"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			AxisBase axis = ((WpfChartAxisModel)historyItem.TargetObject).Axis;
			if (axis is SecondaryAxisX2D)
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[index].VisualRange.MinValue = historyItem.NewValue;
			else if (axis is SecondaryAxisY2D)
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[index].VisualRange.MinValue = historyItem.NewValue;
			else if (axis is AxisX2D)
				((XYDiagram2D)chartControl.Diagram).AxisX.VisualRange.MinValue = historyItem.NewValue;
			else if (axis is AxisY2D)
				((XYDiagram2D)chartControl.Diagram).AxisY.VisualRange.MinValue = historyItem.NewValue;
			else
				throw new ChartDesignerException(historyItem.TargetObject.GetType().Name + " is unknown axis type with Range.");
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			CommandResult createVisibleRangeResult = CreateVisualRangeIfNecessary((WpfChartAxisModel)ChartModel.SelectedModel);
			if (createVisibleRangeResult != null)
				compositeHistoryItem.HistoryItems.Add(createVisibleRangeResult.HistoryItem);
			var axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			object oldValue = axisModel.VisualRangeMinValue;
			object newValue = parameter;
			axisModel.VisualRangeMinValue = newValue;
			ElementIndexItem indexItem = GetAxisIndexItem(axisModel, ChartModel.DiagramModel.Diagram);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, indexItem);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, axisModel, oldValue, newValue);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem);
			return result;
		}
	}
	public class SetAxisWholeRangeMaxValueCommand : AxisRangeCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_WholeRangeMaxValue);
		readonly ScaleType scaleType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ScaleType ScaleType {
			get { return scaleType; }
		}
		public SetAxisWholeRangeMaxValueCommand(WpfChartModel chartModel, ScaleType scaleType)
			: base(chartModel) {
			this.scaleType = scaleType;
		}
		protected override bool CanExecute(object parameter) {
			if (!(ChartModel.SelectedModel is WpfChartAxisModel))
				return false;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axisBase = axisModel.Axis;
			if (!(axisBase is Axis || axisBase is CircularAxisY2D || axisBase is RadarAxisX2D))
				return false;
			if (this.scaleType == axisModel.ScaleType)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.WholeRangeMaxValue = historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.WholeRangeMaxValue = historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetAxisModelItem(chartModelItem, historyItem);
			axisModelItem.Properties["WholeRange"].Value.Properties["MaxValue"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			AxisBase axis = ((WpfChartAxisModel)historyItem.TargetObject).Axis;
			if (axis.GetType() == typeof(AxisX2D))
				((XYDiagram2D)chartControl.Diagram).AxisX.WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(AxisY2D))
				((XYDiagram2D)chartControl.Diagram).AxisY.WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(SecondaryAxisX2D))
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[index].WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(SecondaryAxisY2D))
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[index].WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(AxisX3D))
				((XYDiagram3D)chartControl.Diagram).AxisX.WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(AxisY3D))
				((XYDiagram3D)chartControl.Diagram).AxisY.WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(PolarAxisY2D))
				((PolarDiagram2D)chartControl.Diagram).AxisY.WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(RadarAxisX2D))
				((RadarDiagram2D)chartControl.Diagram).AxisX.WholeRange.MaxValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(RadarAxisY2D))
				((RadarDiagram2D)chartControl.Diagram).AxisY.WholeRange.MaxValue = historyItem.NewValue;
			else
				throw new ChartDesignerException(historyItem.TargetObject.GetType().Name + " is unknown axis type with ScrollingRange.");
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			CommandResult createWholeRangeResult = CreateWholeRangeIfNecessary((WpfChartAxisModel)ChartModel.SelectedModel);
			if (createWholeRangeResult != null)
				compositeHistoryItem.HistoryItems.Add(createWholeRangeResult.HistoryItem);
			var axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			object oldValue = axisModel.WholeRangeMaxValue;
			object newValue = parameter;
			axisModel.WholeRangeMaxValue = newValue;
			ElementIndexItem indexItem = GetAxisIndexItem(axisModel, ChartModel.DiagramModel.Diagram);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, indexItem);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, axisModel, oldValue, newValue);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem);
			return result;
		}
	}
	public class SetAxisWholeRangeMinValueCommand : AxisRangeCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_WholeRangeMinValue);
		readonly ScaleType scaleType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ScaleType ScaleType {
			get { return scaleType; }
		}
		public SetAxisWholeRangeMinValueCommand(WpfChartModel chartModel, ScaleType scaleType)
			: base(chartModel) {
			this.scaleType = scaleType;
		}
		protected override bool CanExecute(object parameter) {
			if (!(ChartModel.SelectedModel is WpfChartAxisModel))
				return false;
			WpfChartAxisModel axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			AxisBase axisBase = axisModel.Axis;
			if (!(axisBase is Axis || axisBase is CircularAxisY2D || axisBase is RadarAxisX2D))
				return false;
			if (this.scaleType == axisModel.ScaleType)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.WholeRangeMinValue = historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			var axisModel = (WpfChartAxisModel)historyItem.TargetObject;
			axisModel.WholeRangeMinValue = historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisModelItem = GetAxisModelItem(chartModelItem, historyItem);
			axisModelItem.Properties["WholeRange"].Value.Properties["MinValue"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int index = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			AxisBase axis = ((WpfChartAxisModel)historyItem.TargetObject).Axis;
			if (axis.GetType() == typeof(AxisX2D))
				((XYDiagram2D)chartControl.Diagram).AxisX.WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(AxisY2D))
				((XYDiagram2D)chartControl.Diagram).AxisY.WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(SecondaryAxisX2D))
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[index].WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(SecondaryAxisY2D))
				((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[index].WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(AxisX3D))
				((XYDiagram3D)chartControl.Diagram).AxisX.WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(AxisY3D))
				((XYDiagram3D)chartControl.Diagram).AxisY.WholeRange.MinValue = historyItem.NewValue;
			else if (axis is PolarAxisY2D)
				((PolarDiagram2D)chartControl.Diagram).AxisY.WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(RadarAxisX2D))
				((RadarDiagram2D)chartControl.Diagram).AxisX.WholeRange.MinValue = historyItem.NewValue;
			else if (axis.GetType() == typeof(RadarAxisY2D))
				((RadarDiagram2D)chartControl.Diagram).AxisY.WholeRange.MinValue = historyItem.NewValue;
			else
				throw new ChartDesignerException(historyItem.TargetObject.GetType().Name + " is unknown axis type with ScrollingRange.");
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistoryItem = new CompositeHistoryItem();
			CommandResult createWholeRangeResult = CreateWholeRangeIfNecessary((WpfChartAxisModel)ChartModel.SelectedModel);
			if (createWholeRangeResult != null)
				compositeHistoryItem.HistoryItems.Add(createWholeRangeResult.HistoryItem);
			var axisModel = (WpfChartAxisModel)ChartModel.SelectedModel;
			object oldValue = axisModel.WholeRangeMinValue;
			object newValue = parameter;
			axisModel.WholeRangeMinValue = newValue;
			ElementIndexItem indexItem = GetAxisIndexItem(axisModel, ChartModel.DiagramModel.Diagram);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, indexItem);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, axisModel, oldValue, newValue);
			compositeHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositeHistoryItem);
			return result;
		}
	}
}
