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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using System.Security;
namespace DevExpress.Charts.Designer.Native {
	public abstract class StripCommandBase : ChartCommandBase {
		public const string AxisKey = "Axis";
		public const string SecondaryAxisKey = "SecondaryAxis";
		public const string StripKey = "Strip";
		protected WpfChartStripModel SelectedStripModel {
			get { return ChartModel.SelectedModel as WpfChartStripModel; }
		}
		public Strip SelectedStrip {
			get {
				if (SelectedStripModel == null)
					return null;
				else
					return SelectedStripModel.Strip;
			}
		}
		protected XYDiagram2D PreviewXYDIagram2D {
			get { return (XYDiagram2D)PreviewChart.Diagram; }
		}
		public StripCommandBase(WpfChartModel chartModel)
			: base(chartModel) { }
		protected ElementIndexItem[] GetPathIndexItemList(Axis2D axis2D = null) {
			Axis2D axis;
			if (axis2D == null)
				axis = ChartDesignerPropertiesProvider.GetStripOwner(SelectedStrip);
			else
				axis = axis2D;
			List<ElementIndexItem> list = new List<ElementIndexItem>();
			list = new List<ElementIndexItem>();
			if (axis is AxisX2D)
				list.Add(new ElementIndexItem(StripCommandBase.AxisKey, -1));
			else if (axis is AxisY2D)
				list.Add(new ElementIndexItem(StripCommandBase.AxisKey, -2));
			else if (axis is SecondaryAxisX2D) {
				list.Add(new ElementIndexItem(StripCommandBase.AxisKey, -3));
				int secondaryAxisXIndex = PreviewXYDIagram2D.SecondaryAxesX.IndexOf((SecondaryAxisX2D)axis);
				list.Add(new ElementIndexItem(StripCommandBase.SecondaryAxisKey, secondaryAxisXIndex));
			}
			else if (axis is SecondaryAxisY2D) {
				list.Add(new ElementIndexItem(StripCommandBase.AxisKey, -4));
				int secondaryAxisYIndex = PreviewXYDIagram2D.SecondaryAxesY.IndexOf((SecondaryAxisY2D)axis);
				list.Add(new ElementIndexItem(StripCommandBase.SecondaryAxisKey, secondaryAxisYIndex));
			}
			else
				throw new ChartDesignerException("Unknown axis type for constant line.");
			int stripIndex;
			if (SelectedStrip != null)
				stripIndex = axis.Strips.IndexOf(SelectedStrip);
			else
				stripIndex = -1;
			list.Add(new ElementIndexItem(StripCommandBase.StripKey, stripIndex));
			return list.ToArray();
		}
		protected WpfChartStripModel GetStripModel(HistoryItem historyItem) {
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[AxisKey];
			WpfChartAxisModel axisModel;
			switch (axisPseudoIndex) {
				case -1:
					axisModel = ChartModel.DiagramModel.PrimaryAxisModelX;
					break;
				case -2:
					axisModel = ChartModel.DiagramModel.PrimaryAxisModelY;
					break;
				case -3:
					int secondaryAxisXIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SecondaryAxisKey];
					axisModel = (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelX.ModelCollection[secondaryAxisXIndex];
					break;
				case -4:
					int secondaryAxisYIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SecondaryAxisKey];
					axisModel = (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelX.ModelCollection[secondaryAxisYIndex];
					break;
				default:
					throw new ChartDesignerException("Unknown pseudo index.");
			}
			int stripIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[StripKey];
			return (WpfChartStripModel)axisModel.StripCollectionModel.ModelCollection[stripIndex];
		}
		protected Strip GetTargetStripForRuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[AxisKey];
			XYDiagram2D xyDiagram2D = (XYDiagram2D)chartControl.Diagram;
			Axis2D axis = GetTargetAxisForRuntimeApply(chartControl, historyItem);
			int constantLineIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[StripKey];
			Strip targetStrip = axis.Strips[constantLineIndex];
			return targetStrip;
		}
		protected Axis2D GetTargetAxisForRuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[AxisKey];
			XYDiagram2D xyDiagram2D = (XYDiagram2D)chartControl.Diagram;
			Axis2D axis;
			switch (axisPseudoIndex) {
				case -1:
					axis = xyDiagram2D.AxisX;
					break;
				case -2:
					axis = xyDiagram2D.AxisY;
					break;
				case -3:
					int secondaryAxisXIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SecondaryAxisKey];
					axis = xyDiagram2D.SecondaryAxesX[secondaryAxisXIndex];
					break;
				case -4:
					int secondaryAxisYIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SecondaryAxisKey];
					axis = xyDiagram2D.SecondaryAxesY[secondaryAxisYIndex];
					break;
				default:
					throw new ChartDesignerException("Unknown pseudo index.");
			}
			return axis;
		}
		protected IModelItem GetTargetStripForDesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis = GetTargetAxisForDesigntimeApply(chartModelItem, historyItem);
			int stripIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[StripKey];
			IModelItem constantLine = axis.Properties["Strips"].Collection[stripIndex];
			return constantLine;
		}
		protected IModelItem GetTargetAxisForDesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis;
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[AxisKey];
			switch (axisPseudoIndex) {
				case -1:
					axis = xyDiagram2D.Properties["AxisX"].Value;
					break;
				case -2:
					axis = xyDiagram2D.Properties["AxisY"].Value;
					break;
				case -3:
					int secondaryAxisXIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SecondaryAxisKey];
					axis = xyDiagram2D.Properties["SecondaryAxesX"].Collection[secondaryAxisXIndex];
					break;
				case -4:
					int secondaryAxisYIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SecondaryAxisKey];
					axis = xyDiagram2D.Properties["SecondaryAxesY"].Collection[secondaryAxisYIndex];
					break;
				default:
					throw new ChartDesignerException("Unknown pseudo index.");
			}
			return axis;
		}
	}
	public class ChangeStripMinLimitCommand : StripCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_MinLimit);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public BarEditValueItemViewModel EditorModel {
			get;
			set;
		}
		public ChangeStripMinLimitCommand(WpfChartModel model)
			: base(model) {
		}
		void ShowErrorMessage(string incompatibleConstantLineValueMsgBoxCaption, string incompatibleConstantLineValueMsgBoxMessage) {
			MessageBox.Show(incompatibleConstantLineValueMsgBoxMessage, incompatibleConstantLineValueMsgBoxCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
			if (EditorModel != null)
				EditorModel.EditValue = EditorModel.Value;
			else
				ChartDebug.WriteWarning("ChangeStripMinLimitCommand.EditorModel property was not set. That's why EditValue in an editor was not reset.");
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedStripModel != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			object newValue;
			AxisBase axis = ChartDesignerPropertiesProvider.GetStripOwner(SelectedStrip);
			AxisScaleTypeMap scaleTypeMap = ChartDesignerPropertiesProvider.GetAxisScaleTypeMap(axis);
			if (scaleTypeMap is AxisQualitativeMap)
				newValue = parameter.ToString();
			else if (scaleTypeMap is AxisNumericalMap) {
				double val;
				bool parsed = double.TryParse(parameter.ToString(), out val);
				if (parsed)
					newValue = val;
				else {
					string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMinLimitCaption);
					string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMinLimitMessage);
					ShowErrorMessage(caption, message);
					return null;
				}
			}
			else if (scaleTypeMap is AxisDateTimeMap) {
				DateTime val;
				bool parsed = DateTime.TryParse(parameter.ToString(), out val);
				if (parsed)
					newValue = val;
				else {
					string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMinLimitCaption);
					string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMinLimitMessage);
					ShowErrorMessage(caption, message);
					return null;
				}
			}
			else
				throw new ChartDesignerException("Uncknown ScaleTypeMap.");
			object oldValue = SelectedStripModel.MinLimit;
			SelectedStripModel.MinLimit = newValue;
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedStrip, oldValue, newValue);
			return new CommandResult(historyItem);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.MinLimit = historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.MinLimit = historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Strip targetStrip = GetTargetStripForRuntimeApply(chartControl, historyItem);
			targetStrip.MinLimit = historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem targetStrip = GetTargetStripForDesigntimeApply(chartModelItem, historyItem);
			targetStrip.Properties["MinLimit"].SetValue(historyItem.NewValue);
		}
	}
	public class ChangeStripMaxLimitCommand : StripCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_MaxLimit);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public BarEditValueItemViewModel EditorModel {
			get;
			set;
		}
		public ChangeStripMaxLimitCommand(WpfChartModel model)
			: base(model) {
		}
		void ShowErrorMessage(string incompatibleConstantLineValueMsgBoxCaption, string incompatibleConstantLineValueMsgBoxMessage) {
			MessageBox.Show(incompatibleConstantLineValueMsgBoxMessage, incompatibleConstantLineValueMsgBoxCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
			if (EditorModel != null)
				EditorModel.EditValue = EditorModel.Value;
			else
				ChartDebug.WriteWarning("ChangeStripMaxLimitCommand.EditorModel property was not set. That's why EditValue in an editor was not reset.");
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedStripModel != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			object newValue;
			AxisBase axis = ChartDesignerPropertiesProvider.GetStripOwner(SelectedStrip);
			AxisScaleTypeMap scaleTypeMap = ChartDesignerPropertiesProvider.GetAxisScaleTypeMap(axis);
			if (scaleTypeMap is AxisQualitativeMap)
				newValue = parameter.ToString();
			else if (scaleTypeMap is AxisNumericalMap) {
				double val;
				bool parsed = double.TryParse(parameter.ToString(), out val);
				if (parsed)
					newValue = val;
				else {
					string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMaxLimitCaption);
					string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMaxLimitMessage);
					ShowErrorMessage(caption, message);
					return null;
				}
			}
			else if (scaleTypeMap is AxisDateTimeMap) {
				DateTime val;
				bool parsed = DateTime.TryParse(parameter.ToString(), out val);
				if (parsed)
					newValue = val;
				else {
					string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMaxLimitCaption);
					string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMaxLimitMessage);
					ShowErrorMessage(caption, message);
					return null;
				}
			}
			else
				throw new ChartDesignerException("Uncknown ScaleTypeMap.");
			object oldValue = SelectedStripModel.MaxLimit;
			SelectedStripModel.MaxLimit = newValue;
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedStrip, oldValue, newValue);
			return new CommandResult(historyItem);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.MaxLimit = historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.MaxLimit = historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Strip targetStrip = GetTargetStripForRuntimeApply(chartControl, historyItem);
			targetStrip.MaxLimit = historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem targetStrip = GetTargetStripForDesigntimeApply(chartModelItem, historyItem);
			targetStrip.Properties["MaxLimit"].SetValue(historyItem.NewValue);
		}
	}
	public class ChangeStripAxisLabelTextCommand : StripCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_AxisLabelText);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeStripAxisLabelTextCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedStripModel != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			string newValue = (string)parameter;
			string oldValue = (string)SelectedStripModel.AxisLabelText;
			SelectedStripModel.AxisLabelText = newValue;
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedStrip, oldValue, newValue);
			return new CommandResult(historyItem);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.AxisLabelText = (string)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.AxisLabelText = (string)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Strip targetStrip = GetTargetStripForRuntimeApply(chartControl, historyItem);
			targetStrip.AxisLabelText = (string)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem targetStrip = GetTargetStripForDesigntimeApply(chartModelItem, historyItem);
			targetStrip.Properties["AxisLabelText"].SetValue(historyItem.NewValue);
		}
	}
	public class ChangeStripLegendTextCommand : StripCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_LegendText);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeStripLegendTextCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedStripModel != null && ChartModel.LegendModel != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			string newValue = (string)parameter;
			string oldValue = (string)SelectedStripModel.LegendText;
			SelectedStripModel.LegendText = newValue;
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedStrip, oldValue, newValue);
			return new CommandResult(historyItem);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.LegendText = (string)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.LegendText = (string)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Strip targetStrip = GetTargetStripForRuntimeApply(chartControl, historyItem);
			targetStrip.LegendText = (string)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem targetStrip = GetTargetStripForDesigntimeApply(chartModelItem, historyItem);
			targetStrip.Properties["LegendText"].SetValue(historyItem.NewValue);
		}
	}
	public class ChangeStripBrushCommand : StripCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_Brush);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeStripBrushCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (SelectedStrip != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			SolidColorBrush newValue = new SolidColorBrush((Color)parameter);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedStrip, SelectedStripModel.Brush, newValue);
			SelectedStripModel.Brush = newValue;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.Brush = (SolidColorBrush)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Strip target = (Strip)historyItem.TargetObject;
			target.Brush = (SolidColorBrush)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Strip target = GetTargetStripForRuntimeApply(chartControl, historyItem);
			target.Brush = (SolidColorBrush)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetStripForDesigntimeApply(chartModelItem, historyItem);
			target.Properties["Brush"].SetValue(historyItem.NewValue);
		}
	}
}
