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
using System.Windows.Input;
namespace DevExpress.Charts.Designer.Native {
	public abstract class AxisOptionsCommandBase : ChartCommandBase {
		protected const string axisIndexKey = "Axis";
		protected WpfChartAxisModel SelectedAxisModel {
			get { return ChartModel.SelectedModel as WpfChartAxisModel; }
		}
		public AxisOptionsCommandBase(WpfChartModel model)
			: base(model) {
		}
		protected ElementIndexItem GetAxisIndexItem(WpfChartAxisModel axisModel, Diagram diagram) {
			int index = SelectedAxisModel.GetSelfIndex();
			var item = new ElementIndexItem(axisIndexKey, index);
			return item;
		}
		protected CommandResult CreateAxisIfNecessary() {
			CommandResult result = null;
			if (SelectedAxisModel != null && SelectedAxisModel.IsPrimaryAxis) {
				ChartCommandBase addAxisCommand = null;
				if (SelectedAxisModel.Axis is AxisX2D)
					if (((XYDiagram2D)PreviewChart.Diagram).AxisX == null)
						addAxisCommand = new AddAxisXCommand(ChartModel);
				if (SelectedAxisModel.Axis is AxisY2D)
					if (((XYDiagram2D)PreviewChart.Diagram).AxisY == null)
						addAxisCommand = new AddAxisYCommand(ChartModel);
				if (SelectedAxisModel.Axis is AxisX3D)
					if (((XYDiagram3D)PreviewChart.Diagram).AxisX == null)
						addAxisCommand = new AddAxisXCommand(ChartModel);
				if (SelectedAxisModel.Axis is AxisY3D)
					if (((XYDiagram3D)PreviewChart.Diagram).AxisY == null)
						addAxisCommand = new AddAxisYCommand(ChartModel);
				if (SelectedAxisModel.Axis is RadarAxisX2D)
					if (((RadarDiagram2D)PreviewChart.Diagram).AxisX == null)
						addAxisCommand = new AddAxisXCommand(ChartModel);
				if (SelectedAxisModel.Axis is RadarAxisY2D)
					if (((RadarDiagram2D)PreviewChart.Diagram).AxisY == null)
						addAxisCommand = new AddAxisYCommand(ChartModel);
				if (SelectedAxisModel.Axis is PolarAxisX2D)
					if (((PolarDiagram2D)PreviewChart.Diagram).AxisX == null)
						addAxisCommand = new AddAxisXCommand(ChartModel);
				if (SelectedAxisModel.Axis is PolarAxisY2D)
					if (((PolarDiagram2D)PreviewChart.Diagram).AxisY == null)
						addAxisCommand = new AddAxisYCommand(ChartModel);
				if (addAxisCommand != null) 
					result = addAxisCommand.RuntumeExecuteAndUpdateChartModel(null);	 
			}
			return result;
		}
		protected int CreateAxisIfNeeded(CompositeHistoryItem historyItem) {
			CommandResult result = CreateAxisIfNecessary();
			if (result != null) 
				historyItem.HistoryItems.Add(result.HistoryItem);
			return SelectedAxisModel != null ? SelectedAxisModel.GetSelfIndex() : -1;
		}
		protected void CreateLabelIfNeeded() {
			if (SelectedAxisModel.Axis.Label == null) {
				ICommand command = new AddAxisLabelCommand(ChartModel);
				command.Execute(null);
			}
		}
		protected AxisBase GetAxis(ChartControl chart, HistoryItem historyItem) {
			AxisBase axis = null;
			int axisIndex = -1;
			object target = historyItem.TargetObject is AxisBase ? historyItem.TargetObject : ((WpfChartAxisModel)historyItem.TargetObject).Axis;
			if (historyItem.ExecuteCommandInfo.IsValidPath)
				axisIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			if (axisIndex >= 0) {
				if (target.GetType() == typeof(SecondaryAxisX2D))
					axis = ((XYDiagram2D)chart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
				if (target.GetType() == typeof(SecondaryAxisY2D))
					axis = ((XYDiagram2D)chart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
			}
			else {
				if (target.GetType() == typeof(AxisX2D))
					axis = ((XYDiagram2D)chart.Diagram).AxisX;
				if (target.GetType() == typeof(AxisY2D))
					axis = ((XYDiagram2D)chart.Diagram).AxisY;
				if (target.GetType() == typeof(AxisX3D))
					axis = ((XYDiagram3D)chart.Diagram).AxisX;
				if (target.GetType() == typeof(AxisY3D))
					axis = ((XYDiagram3D)chart.Diagram).AxisY;
				if (target.GetType() == typeof(PolarAxisX2D))
					axis = ((PolarDiagram2D)chart.Diagram).AxisX;
				if (target.GetType() == typeof(PolarAxisY2D))
					axis = ((PolarDiagram2D)chart.Diagram).AxisY;
				if (target.GetType() == typeof(RadarAxisX2D))
					axis = ((RadarDiagram2D)chart.Diagram).AxisX;
				if (target.GetType() == typeof(RadarAxisY2D))
					axis = ((RadarDiagram2D)chart.Diagram).AxisY;
			}
			return axis;
		}
		protected IModelItem GetAxisModelItem(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem axisModelItem = null;
			int axisIndex = -1;
			object target = historyItem.TargetObject is AxisBase ? historyItem.TargetObject : ((WpfChartAxisModel)historyItem.TargetObject).Axis;
			if (historyItem.ExecuteCommandInfo.IsValidPath)
				axisIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			if (axisIndex >= 0) {
				if (target.GetType() == typeof(SecondaryAxisX2D))
					axisModelItem = chartModelItem.Properties["Diagram"].Value.Properties["SecondaryAxesX"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
				if (target.GetType() == typeof(SecondaryAxisY2D))
					axisModelItem = chartModelItem.Properties["Diagram"].Value.Properties["SecondaryAxesY"].Collection[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
			}
			else {
				if (target is AxisX2D || historyItem.TargetObject is AxisX3D || historyItem.TargetObject is CircularAxisX2D)
					axisModelItem = chartModelItem.Properties["Diagram"].Value.Properties["AxisX"].Value;
				if (target is AxisY2D || historyItem.TargetObject is AxisY3D || historyItem.TargetObject is CircularAxisY2D)
					axisModelItem = chartModelItem.Properties["Diagram"].Value.Properties["AxisY"].Value;
			}
			return axisModelItem;
		}
		protected WpfChartAxisModel GetAxisModel(HistoryItem historyItem) {
			int axisIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey];
			Type axisType = historyItem.TargetObject.GetType();
			if (axisIndex >= 0) {
				if (axisType == typeof(SecondaryAxisX2D))
					return (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelX.ModelCollection[axisIndex];
				if (axisType == typeof(SecondaryAxisY2D))
					return (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelY.ModelCollection[axisIndex];
			}
			else {
				if (axisType == typeof(AxisX2D) || axisType == typeof(AxisX3D) || axisType == typeof(PolarAxisX2D) || axisType == typeof(RadarAxisX2D))
					return ChartModel.DiagramModel.PrimaryAxisModelX;
				if (axisType == typeof(AxisY2D) || axisType == typeof(AxisY3D) || axisType == typeof(PolarAxisY2D) || axisType == typeof(RadarAxisY2D))
					return ChartModel.DiagramModel.PrimaryAxisModelY;
			}
			return null;
		}
	}
	public abstract class AxisConditionalCommandBase : AxisOptionsCommandBase {
		public AxisConditionalCommandBase(WpfChartModel model)
			: base(model) {
		}
		protected virtual void DesigntimeApplyInternal(IModelItem axisModelItem, object value) {
			;
		}
		protected abstract bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel);
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && AxisTypeMatchCommandType(SelectedAxisModel);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			return null;
		}
		public abstract bool IsAxisAtCommandState(WpfChartAxisModel axisModel);
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) { ; }
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			DesigntimeApplyInternal(axisItem, historyItem.NewValue);
		}
	}
	public class ToggleAxisVisibilityCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisVisibilityCommand(WpfChartModel model)
			: base(model) { }
		void SetAxisVisible(AxisBase axis, bool visible) {
			if (axis is Axis2D)
				((Axis2D)axis).Visible = visible;
			if (axis is CircularAxisY2D)
				((CircularAxisY2D)axis).Visible = visible;
		}
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && (SelectedAxisModel.Axis is Axis2D || SelectedAxisModel.Axis is CircularAxisY2D);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			SetAxisVisible((AxisBase)historyItem.TargetObject, (bool)historyItem.NewValue);
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			SetAxisVisible((AxisBase)historyItem.TargetObject, (bool)historyItem.OldValue);
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Visible"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Visible = (bool)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Visible = (bool)historyItem.NewValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Visible = (bool)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Visible = (bool)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					((RadarDiagram2D)chartControl.Diagram).AxisY.Visible = (bool)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					((PolarDiagram2D)chartControl.Diagram).AxisY.Visible = (bool)historyItem.NewValue;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.IsVisible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, !(bool)parameter, (bool)parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ChangeAxisAlignmentCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeAxisAlignmentCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is Axis2D && SelectedAxisModel.IsVisible;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((Axis2D)historyItem.TargetObject).Alignment = (AxisAlignment)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((Axis2D)historyItem.TargetObject).Alignment = (AxisAlignment)historyItem.OldValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Alignment"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Alignment = (AxisAlignment)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Alignment = (AxisAlignment)historyItem.ExecuteCommandInfo.Parameter;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Alignment = (AxisAlignment)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Alignment = (AxisAlignment)historyItem.ExecuteCommandInfo.Parameter;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			AxisAlignment oldValue = SelectedAxisModel.Alignment;
			SelectedAxisModel.Alignment = (AxisAlignment)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex) } : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, oldValue, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public abstract class AxisReverseCommandBase : AxisConditionalCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisReverse);
		public override string Caption {
			get { return caption; }
		}
		public AxisReverseCommandBase(WpfChartModel model)
			: base(model) { }
		public override bool IsAxisAtCommandState(WpfChartAxisModel axisModel) {
			return AxisTypeMatchCommandType(axisModel) && axisModel.Reverse;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter != null) {
				CommandResult axisReverseResult = new ToggleAxisReverseCommand(ChartModel).RuntimeExecute((bool)parameter);
				AxisBase resultAxis = SelectedAxisModel.Axis;
				ChartModel.SelectedObject = PreviewChart;
				return new CommandResult(axisReverseResult.HistoryItem, resultAxis);
			}
			return null;
		}
	}
	public class AxisX2DReverseCommand : AxisReverseCommandBase {
		public override string ImageName {
			get { return GlyphUtils.BarItemImages + "AxisOptions\\AxisXReverse"; }
		}
		public AxisX2DReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is AxisX2D;
		}
	}
	public class AxisY2DReverseCommand : AxisReverseCommandBase {
		public override string ImageName {
			get { return GlyphUtils.BarItemImages + "AxisOptions\\AxisYReverse"; }
		}
		public AxisY2DReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is AxisY2D;
		}
	}
	public class ToggleAxisReverseCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is Axis2D;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((Axis2D)historyItem.TargetObject).Reverse = (bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((Axis2D)historyItem.TargetObject).Reverse = !(bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Reverse"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Reverse = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Reverse = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Reverse = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Reverse = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.Reverse = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public abstract class AxisInterlacedCommandBase : AxisConditionalCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_Interlaced);
		protected abstract Type AxisType {
			get;
		}
		public override string Caption {
			get { return caption; }
		}
		public AxisInterlacedCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis.GetType() == AxisType || axisModel.Axis.GetType().IsSubclassOf(AxisType);
		}
		public override bool IsAxisAtCommandState(WpfChartAxisModel axisModel) {
			return AxisTypeMatchCommandType(axisModel) && axisModel.Interlaced;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter != null) {
				CommandResult axisInterlacedResult = new ToggleAxisInterlacedCommand(ChartModel).RuntimeExecute((bool)parameter);
				AxisBase resultAxis = SelectedAxisModel.Axis;
				ChartModel.SelectedObject = PreviewChart;
				return new CommandResult(axisInterlacedResult.HistoryItem, resultAxis);
			}
			return null;
		}
	}
	public class AxisX2DInterlacedCommand : AxisInterlacedCommandBase {
		protected override Type AxisType {
			get { return typeof(AxisX2D); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AxisInterlaced\\AxisX2D"; }
		}
		public AxisX2DInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisX2D;
		}
	}
	public class AxisY2DInterlacedCommand : AxisInterlacedCommandBase {
		protected override Type AxisType {
			get { return typeof(AxisY2D); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AxisInterlaced\\AxisY2D"; }
		}
		public AxisY2DInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisY2D;
		}
	}
	public class AxisX3DInterlacedCommand : AxisInterlacedCommandBase {
		protected override Type AxisType {
			get { return typeof(AxisX3D); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AxisInterlaced\\AxisX3D"; }
		}
		public AxisX3DInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisX3D;
		}
	}
	public class AxisY3DInterlacedCommand : AxisInterlacedCommandBase {
		protected override Type AxisType {
			get { return typeof(AxisY3D); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AxisInterlaced\\AxisY3D"; }
		}
		public AxisY3DInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisY3D;
		}
	}
	public class CircularAxisX2DInterlacedCommand : AxisInterlacedCommandBase {
		protected override Type AxisType {
			get { return typeof(CircularAxisX2D); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AxisInterlaced\\CircularAxisX2D"; }
		}
		public CircularAxisX2DInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is CircularAxisX2D;
		}
	}
	public class CircularAxisY2DInterlacedCommand : AxisInterlacedCommandBase {
		protected override Type AxisType {
			get { return typeof(CircularAxisY2D); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + "AxisInterlaced\\CircularAxisY2D"; }
		}
		public CircularAxisY2DInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is CircularAxisY2D;
		}
	}
	public class ToggleAxisInterlacedCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_Interlaced);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return GlyphUtils.BarItemImages + "AxisInterlaced"; }
		}
		public ToggleAxisInterlacedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((AxisBase)historyItem.TargetObject).Interlaced = (bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((AxisBase)historyItem.TargetObject).Interlaced = !(bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Interlaced"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = GetAxis(chartControl, historyItem);
			axis.Interlaced = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null || !(parameter is bool))
				return null;
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.Interlaced = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ToggleAxisGridLinesVisibleCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_MajorVisible);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisGridLinesVisibleCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((AxisBase)historyItem.TargetObject).GridLinesVisible = (bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((AxisBase)historyItem.TargetObject).GridLinesVisible = !(bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["GridLinesVisible"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = null;
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					axis = ((XYDiagram2D)chartControl.Diagram).AxisX;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					axis = ((XYDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					axis = ((RadarDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					axis = ((PolarDiagram2D)chartControl.Diagram).AxisY;
			}
			axis.GridLinesVisible = (bool)historyItem.ExecuteCommandInfo.Parameter;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.GridLinesVisible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex) } : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ToggleAxisGridLinesMinorVisibledCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_MinorVisible);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisGridLinesMinorVisibledCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((AxisBase)historyItem.TargetObject).GridLinesMinorVisible = (bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((AxisBase)historyItem.TargetObject).GridLinesMinorVisible = !(bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["GridLinesMinorVisible"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			AxisBase axis = null;
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					axis = ((XYDiagram2D)chartControl.Diagram).AxisX;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					axis = ((XYDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					axis = ((RadarDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					axis = ((PolarDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisX2D))
					axis = ((RadarDiagram2D)chartControl.Diagram).AxisX;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisX2D))
					axis = ((PolarDiagram2D)chartControl.Diagram).AxisX;
			}
			axis.GridLinesMinorVisible = (bool)historyItem.ExecuteCommandInfo.Parameter;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.GridLinesMinorVisible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ToggleAxisTickmarksVisibleCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_MajorVisible);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisTickmarksVisibleCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && (SelectedAxisModel.Axis is Axis2D || SelectedAxisModel.Axis is CircularAxisY2D);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			if (historyItem.TargetObject is Axis2D)
				((Axis2D)historyItem.TargetObject).TickmarksVisible = (bool)historyItem.NewValue;
			if (historyItem.TargetObject is CircularAxisY2D)
				((CircularAxisY2D)historyItem.TargetObject).TickmarksVisible = (bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.TargetObject is Axis2D)
				((Axis2D)historyItem.TargetObject).TickmarksVisible = !(bool)historyItem.NewValue;
			if (historyItem.TargetObject is CircularAxisY2D)
				((CircularAxisY2D)historyItem.TargetObject).TickmarksVisible = !(bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["TickmarksVisible"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.TargetObject is Axis2D) {
				Axis2D axis = null;
				if (historyItem.ExecuteCommandInfo.IsValidPath) {
					if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
						axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
					if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
						axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
				}
				else {
					if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
						axis = ((XYDiagram2D)chartControl.Diagram).AxisX;
					if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
						axis = ((XYDiagram2D)chartControl.Diagram).AxisY;
				}
				axis.TickmarksVisible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			else if (historyItem.TargetObject is CircularAxisY2D) {
				CircularAxisY2D axis = null;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					axis = ((RadarDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					axis = ((PolarDiagram2D)chartControl.Diagram).AxisY;
				axis.TickmarksMinorVisible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.TickmarksVisible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ToggleAxisTickmarksMinorVisibleCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_MinorVisible);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisTickmarksMinorVisibleCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && (SelectedAxisModel.Axis is Axis2D || SelectedAxisModel.Axis is CircularAxisY2D);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			if (historyItem.TargetObject is Axis2D)
				((Axis2D)historyItem.TargetObject).TickmarksMinorVisible = (bool)historyItem.NewValue;
			if (historyItem.TargetObject is CircularAxisY2D)
				((CircularAxisY2D)historyItem.TargetObject).TickmarksMinorVisible = (bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.TargetObject is Axis2D)
				((Axis2D)historyItem.TargetObject).TickmarksMinorVisible = !(bool)historyItem.NewValue;
			if (historyItem.TargetObject is CircularAxisY2D)
				((CircularAxisY2D)historyItem.TargetObject).TickmarksMinorVisible = !(bool)historyItem.NewValue;
			return historyItem.TargetObject;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["TickmarksMinorVisible"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.TargetObject is Axis2D) {
				Axis2D axis = null;
				if (historyItem.ExecuteCommandInfo.IsValidPath) {
					if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
						axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
					if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
						axis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]];
				}
				else {
					if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
						axis = ((XYDiagram2D)chartControl.Diagram).AxisX;
					if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
						axis = ((XYDiagram2D)chartControl.Diagram).AxisY;
				}
				axis.TickmarksMinorVisible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			else if (historyItem.TargetObject is CircularAxisY2D) {
				CircularAxisY2D axis = null;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					axis = ((RadarDiagram2D)chartControl.Diagram).AxisY;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					axis = ((PolarDiagram2D)chartControl.Diagram).AxisY;
				axis.TickmarksMinorVisible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			SelectedAxisModel.TickmarksMinorVisible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class AddConstantLineToAxisXCommand : AddConstantLineXCommand {
		public AddConstantLineToAxisXCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisX2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			AxisBase targetAxis = null;
			if (GetAxisIndex() != -1)
				targetAxis = SelectedAxisModel.Axis;
			return base.RuntimeExecute(targetAxis);
		}
	}
	public class AddConstantLineToAxisYCommand : AddConstantLineYCommand {
		public AddConstantLineToAxisYCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisY2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			AxisBase targetAxis = null;
			if (GetAxisIndex() != -1)
				targetAxis = SelectedAxisModel.Axis;
			return base.RuntimeExecute(targetAxis);
		}
	}
	public class AddStripToAxisXCommand : AddStripXCommand {
		public AddStripToAxisXCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisX2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			AxisBase targetAxis = null;
			if (GetAxisIndex() != -1)
				targetAxis = SelectedAxisModel.Axis;
			return base.RuntimeExecute(targetAxis);
		}
	}
	public class AddStripToAxisYCommand : AddStripYCommand {
		public AddStripToAxisYCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisY2D;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			AxisBase targetAxis = null;
			if (GetAxisIndex() != -1)
				targetAxis = SelectedAxisModel.Axis;
			return base.RuntimeExecute(targetAxis);
		}
	}
}
