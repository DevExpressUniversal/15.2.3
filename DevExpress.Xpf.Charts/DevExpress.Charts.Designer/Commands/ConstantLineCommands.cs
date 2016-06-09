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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using System.Security;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ConstantLineCommandBase : ChartCommandBase {
		public const string AxisKey = "Axis";
		public const string SecondaryAxisKey = "SecondaryAxis";
		public const string ConstantLineKey = "ConstantLine";
		protected WpfChartConstantLineModel SelectedConstantLineModel {
			get { return ChartModel.SelectedModel as WpfChartConstantLineModel; }
		}
		public ConstantLine SelectedConstantLine {
			get {
				if (SelectedConstantLineModel == null)
					return null;
				else
					return SelectedConstantLineModel.ConstantLine;
			}
		}
		protected XYDiagram2D PreviewXYDIagram2D {
			get { return (XYDiagram2D)PreviewChart.Diagram; }
		}
		public ConstantLineCommandBase(WpfChartModel chartModel)
			: base(chartModel) { }
		protected ElementIndexItem[] GetPathIndexItemList(Axis2D axis2D = null) {
			return FontCommandUtils.GetConstantLineTitlePathIndexes(axis2D, SelectedConstantLine, PreviewXYDIagram2D);
		}
		protected WpfChartConstantLineModel GetConstantLineModel(HistoryItem historyItem) {
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
			int constantLineIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineKey];
			return (WpfChartConstantLineModel)axisModel.ConstantLinesCollectionModel.ModelCollection[constantLineIndex];
		}
		protected ConstantLine GetTargetConstantLineForRuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[AxisKey];
			XYDiagram2D xyDiagram2D = (XYDiagram2D)chartControl.Diagram;
			Axis2D axis = GetTargetAxisForRuntimeApply(chartControl, historyItem);
			int constantLineIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineKey];
			ConstantLine targetConstantLine = axis.ConstantLinesInFront[constantLineIndex];
			return targetConstantLine;
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
		protected IModelItem GetTargetConstantLineForDesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis = GetTargetAxisForDesigntimeApply(chartModelItem, historyItem);
			int constantLineIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineKey];
			IModelItem constantLine = axis.Properties["ConstantLinesInFront"].Collection[constantLineIndex];
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
		protected int GetConstantLineIndex(ExecuteCommandInfo info) {
			return info.IndexByNameDictionary[ConstantLineKey];
		}
	}
	public class ChangeConstantLineValueCommand : ConstantLineCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_Value); }
		}
		public override string ImageName {
			get { return null; }
		}
		public BarEditValueItemViewModel EditorModel {
			get;
			set;
		}
		public ChangeConstantLineValueCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLineModel != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			object newValue;
			AxisBase axis = ChartDesignerPropertiesProvider.GetConstantLineOwner(SelectedConstantLine);
			AxisScaleTypeMap scaleTypeMap = ChartDesignerPropertiesProvider.GetAxisScaleTypeMap(axis);
			if (scaleTypeMap is AxisQualitativeMap)
				newValue = parameter.ToString();
			else if (scaleTypeMap is AxisNumericalMap) {
				double val;
				bool parsed = double.TryParse(parameter.ToString(), out val);
				if (parsed)
					newValue = val;
				else {
					string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericConstantLineValueCaption);
					string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericConstantLineValueMessage);
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
					string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeConstantLineValueCaption);
					string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeConstantLineValueMessage);
					ShowErrorMessage(caption, message);
					return null;
				}
			}
			else
				throw new ChartDesignerException("Uncknown ScaleTypeMap.");
			object oldValue = SelectedConstantLineModel.Value;
			SelectedConstantLineModel.Value = newValue;
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, oldValue, newValue);
			return new CommandResult(historyItem);
		}
		void ShowErrorMessage(string incompatibleConstantLineValueMsgBoxCaption, string incompatibleConstantLineValueMsgBoxMessage) {
			MessageBox.Show(incompatibleConstantLineValueMsgBoxMessage, incompatibleConstantLineValueMsgBoxCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
			if (EditorModel != null)
				EditorModel.EditValue = EditorModel.Value;
			else
				ChartDebug.WriteWarning("ChangeConstantLineValueCommand.EditorModel property was not set. That's why EditValue in an editor was not reset.");
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Value = historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Value = historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine targetConstantLine = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			targetConstantLine.Value = historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem targetConstantLine = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			targetConstantLine.Properties["Value"].SetValue(historyItem.NewValue);
		}
	}
	public class ChangeConstantLineLegendTextCommand : ConstantLineCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_LegendText); }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeConstantLineLegendTextCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLineModel != null && ChartModel.LegendModel != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			string newValue = (string)parameter;
			string oldValue = SelectedConstantLineModel.LegendText;
			SelectedConstantLineModel.LegendText = newValue;
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, oldValue, newValue);
			return new CommandResult(historyItem);
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			if (historyItem.OldValue == null)
				target.LegendText = null;
			else
				target.LegendText = (string)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			if (historyItem.NewValue == null)
				target.LegendText = null;
			else
				target.LegendText = (string)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			if (historyItem.NewValue == null)
				target.LegendText = null;
			else
				target.LegendText = (string)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem targetConstantLine = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			targetConstantLine.Properties["LegendText"].SetValue(historyItem.NewValue);
		}
	}
	public class CreateConstantLineTitleCommand : ConstantLineCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLine != null)
				return true;
			else
				return false;
		}
		public CreateConstantLineTitleCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] listForAdding = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, listForAdding);
			ConstantLineTitle title = new ConstantLineTitle();
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLine.Title, title);
			SelectedConstantLine.Title = title;
			SelectedConstantLine.Title.Content = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultConstantLineTitle);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine constantLine = (ConstantLine)historyItem.TargetObject;
			constantLine.Title = (ConstantLineTitle)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine constantLine = (ConstantLine)historyItem.TargetObject;
			constantLine.Title = (ConstantLineTitle)historyItem.NewValue;
			return constantLine;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			target.Title = new ConstantLineTitle();
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			IModelItem title = chartModelItem.Context.CreateItem(typeof(ConstantLineTitle));
			target.Properties["Title"].SetValue(title);
		}
	}
	public class ChangeConstantLineTitleTextCommand : ConstantLineCommandBase {
		public override string Caption { get { return null; } }
		public override string ImageName { get { return null; } }
		public ChangeConstantLineTitleTextCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLine != null && SelectedConstantLine.Title != null)
				return true;
			else
				return false;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title.Content = historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title.Content = historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			target.Title.Content = historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			target.Properties["Title"].Value.Properties["Content"].SetValue(historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositHistoryItem = new CompositeHistoryItem();
			string newValue = (string)parameter;
			if (SelectedConstantLine == null) {
				CreateConstantLineTitleCommand command = new CreateConstantLineTitleCommand(ChartModel);
				CommandResult addTitleResult = command.RuntimeExecute(parameter: null);
				compositHistoryItem.HistoryItems.Add(addTitleResult.HistoryItem);
			}
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLineModel.TitleText, newValue);
			compositHistoryItem.HistoryItems.Add(historyItem);
			CommandResult result = new CommandResult(compositHistoryItem);
			SelectedConstantLineModel.TitleText = newValue;
			return result;
		}
	}
	public class ChangeConstantLineThicknessCommand : ConstantLineCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_Thickness);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeConstantLineThicknessCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLine != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			int newValue = (int)parameter;
			CompositeHistoryItem compositHistoryItem = new CompositeHistoryItem();
			if (SelectedConstantLine.LineStyle == null) {
				AddConstantLineStyleCommand command = new AddConstantLineStyleCommand(ChartModel);
				CommandResult addLineStyleResult = command.RuntimeExecute(parameter: null);
				compositHistoryItem.HistoryItems.Add(addLineStyleResult.HistoryItem);
			}
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			HistoryItem changeThicknessHistoryItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLineModel.Thickness, newValue);
			compositHistoryItem.HistoryItems.Add(changeThicknessHistoryItem);
			SelectedConstantLineModel.Thickness = newValue;
			CommandResult result = new CommandResult(compositHistoryItem);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.LineStyle.Thickness = (int)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.LineStyle.Thickness = (int)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			constantLine.LineStyle.Thickness = (int)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			target.Properties["LineStyle"].Value.Properties["Thickness"].SetValue(historyItem.NewValue);
		}
	}
	public class AddConstantLineStyleCommand : ConstantLineCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public AddConstantLineStyleCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLine != null)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.LineStyle = (LineStyle)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.LineStyle = (LineStyle)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			IModelItem lineStyle = chartModelItem.Context.CreateItem(typeof(LineStyle));
			target.Properties["LineStyle"].SetValue(lineStyle);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			target.LineStyle = new LineStyle();
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			LineStyle newValue = new LineStyle();
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLine.LineStyle, newValue);
			SelectedConstantLine.LineStyle = newValue;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeConstantLineColorCommand : ConstantLineCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_LineColor);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeConstantLineColorCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLine != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			SolidColorBrush newValue = new SolidColorBrush((Color)parameter);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLineModel.LineBrush, newValue);
			SelectedConstantLineModel.LineBrush = newValue;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Brush = (SolidColorBrush)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Brush = (SolidColorBrush)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			constantLine.Brush = (SolidColorBrush)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			target.Properties["Brush"].SetValue(historyItem.NewValue);
		}
	}
	public class ChangeConstantLineTitleForegroundCommand : ConstantLineCommandBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitleForeground);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeConstantLineTitleForegroundCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (SelectedConstantLine != null && SelectedConstantLine.Title != null)
				return true;
			else
				return false;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title.Foreground = (SolidColorBrush)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title.Foreground = (SolidColorBrush)historyItem.NewValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			target.Properties["Title"].Value.Properties["Foreground"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			target.Title.Foreground = (SolidColorBrush)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			SolidColorBrush newValue = new SolidColorBrush((Color)parameter);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLineModel.TitleForeground, newValue);
			SelectedConstantLineModel.TitleForeground = newValue;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	#region Change ConstantLineTitle Positions Commands
	public abstract class ChangeConstantLineTitlePositionBase : ConstantLineCommandBase {
		protected virtual ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Near; } }
		protected virtual bool ShowBelowLine { get { return false; } }
		public ChangeConstantLineTitlePositionBase(WpfChartModel chartModel) : base(chartModel) { }
		protected bool IsConstantLineVertical() {
			AxisBase constantLineOwner = ChartDesignerPropertiesProvider.GetConstantLineOwner(SelectedConstantLine);
			if (constantLineOwner == null)
				return false;
			bool isConstantLineVertical = !ChartDesignerPropertiesProvider.IsAxisVertical(constantLineOwner);
			return isConstantLineVertical;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter != null && (bool)parameter != true)
				return null;
			CompositeHistoryItem compositHistoryItem = new CompositeHistoryItem();
			if (SelectedConstantLine.Title == null) {
				CreateConstantLineTitleCommand command = new CreateConstantLineTitleCommand(ChartModel);
				CommandResult addConstatntLineTitleResult = command.RuntimeExecute(parameter: null);
				compositHistoryItem.HistoryItems.Add(addConstatntLineTitleResult.HistoryItem);
			}
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			Axis2D axis = ChartDesignerPropertiesProvider.GetConstantLineOwner(SelectedConstantLine);
			bool isConstantLineVertical = !ChartDesignerPropertiesProvider.IsAxisVertical(axis);
			ConstantLineTitlePosition oldValue = new ConstantLineTitlePosition(SelectedConstantLine.Title.Alignment, SelectedConstantLine.Title.ShowBelowLine, isConstantLineVertical);
			ConstantLineTitlePosition newValue = new ConstantLineTitlePosition(Alignment, ShowBelowLine, isConstantLineVertical);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, oldValue, newValue);
			compositHistoryItem.HistoryItems.Add(historyItem);
			SelectedConstantLineModel.ShowTitleBelowLine = ShowBelowLine;
			SelectedConstantLineModel.TitleAlignment = Alignment;
			CommandResult result = new CommandResult(compositHistoryItem);
			return result;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title.ShowBelowLine = ((ConstantLineTitlePosition)historyItem.OldValue).ShowBelowLine;
			target.Title.Alignment = ((ConstantLineTitlePosition)historyItem.OldValue).Alignment;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title.ShowBelowLine = ((ConstantLineTitlePosition)historyItem.NewValue).ShowBelowLine;
			target.Title.Alignment = ((ConstantLineTitlePosition)historyItem.NewValue).Alignment;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			target.Title.ShowBelowLine = ((ConstantLineTitlePosition)historyItem.NewValue).ShowBelowLine;
			target.Title.Alignment = ((ConstantLineTitlePosition)historyItem.NewValue).Alignment;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			IModelItem title = target.Properties["Title"].Value;
			title.Properties["ShowBelowLine"].SetValue(((ConstantLineTitlePosition)historyItem.NewValue).ShowBelowLine);
			title.Properties["Alignment"].SetValue(((ConstantLineTitlePosition)historyItem.NewValue).Alignment);
		}
	}
	public class ChangeConstantLineTitlePositionNearAboveHorizontal : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearAboveHorizontal);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearAboveHorizontal);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/NearAboveHorizontal";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Near; } }
		protected override bool ShowBelowLine { get { return false; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionNearAboveHorizontal(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && !IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionNearAboveVertical : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearAboveVertical);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearAboveVertical);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/NearAboveVertical";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Near; } }
		protected override bool ShowBelowLine { get { return false; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionNearAboveVertical(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionFarAboveHorizontal : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarAboveHorizontal);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarAboveHorizontal);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/FarAboveHorizontal";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Far; } }
		protected override bool ShowBelowLine { get { return false; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionFarAboveHorizontal(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && !IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionFarAboveVertical : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarAboveVertical);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarAboveVertical);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/FarAboveVertical";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Far; } }
		protected override bool ShowBelowLine { get { return false; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionFarAboveVertical(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionNearBelowHorizontal : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearBelowHorizontal);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearBelowHorizontal);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/NearBelowHorizontal";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Near; } }
		protected override bool ShowBelowLine { get { return true; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionNearBelowHorizontal(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && !IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionNearBelowVertical : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearBelowVertical);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearBelowVertical);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/NearBelowVertical";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Near; } }
		protected override bool ShowBelowLine { get { return true; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionNearBelowVertical(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionFarBelowHorizontal : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarBelowHorizontal);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarBelowHorizontal);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/FarBelowHorizontal";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Far; } }
		protected override bool ShowBelowLine { get { return true; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionFarBelowHorizontal(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && !IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionFarBelowVertical : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarBelowVertical);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarBelowVertical);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/FarBelowVertical";
		protected override ConstantLineTitleAlignment Alignment { get { return ConstantLineTitleAlignment.Far; } }
		protected override bool ShowBelowLine { get { return true; } }
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionFarBelowVertical(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedConstantLine != null && IsConstantLineVertical();
		}
	}
	public class ChangeConstantLineTitlePositionNone : ChangeConstantLineTitlePositionBase {
		string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_None);
		string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_None);
		string imageName = GlyphUtils.GalleryItemImages + "ConstantLineTitlePositions/None";
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeConstantLineTitlePositionNone(WpfChartModel chartModel)
			: base(chartModel) { }
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter != null && (bool)parameter != true)
				return null;
			ElementIndexItem[] pathIndexItems = GetPathIndexItemList();
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, pathIndexItems);
			HistoryItem historyItem = new HistoryItem(info, this, SelectedConstantLine, SelectedConstantLine.Title, null);
			SelectedConstantLine.Title = null;
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			target.Title = (ConstantLineTitle)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ConstantLine target = (ConstantLine)historyItem.TargetObject;
			SelectedConstantLine.Title = (ConstantLineTitle)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine target = GetTargetConstantLineForRuntimeApply(chartControl, historyItem);
			target.Title = null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem target = GetTargetConstantLineForDesigntimeApply(chartModelItem, historyItem);
			target.Properties["Title"].SetValue(null);
		}
	}
	#endregion
	#region Font Commands
	public class ConstantLineTitleFontFamilyCommand : FontFamilyCommand {
		new public WpfChartConstantLineModel FontModel { get { return (WpfChartConstantLineModel)ChartModel.SelectedModel; } }
		public ConstantLineTitleFontFamilyCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			ConstantLine constantLine = ChartModel.SelectedObject as ConstantLine;
			if(constantLine == null)
				return false;
			if(constantLine.Title != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			FontFamily fontFamily = parameter as FontFamily;
			if(fontFamily == null)
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.ConstantLine.Title, FontModel.ConstantLine.Title.FontFamily, fontFamily);
			FontModel.ConstantLine.Title.FontFamily = fontFamily;
			return new CommandResult(historyItem);
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.DiagramModel.Diagram;
			return FontCommandUtils.GetConstantLineTitlePathIndexes(null, (ConstantLine)ChartModel.SelectedObject, diagram);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = FontCommandUtils.GetConstantLineFontHolder(chartControl, historyItem);
			return constantLine.Title;
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem constantLine = FontCommandUtils.GetConstantLineFontHolder(chartModelItem, historyItem);
			return constantLine.Properties["Title"].Value;
		}
	}
	public class ConstantLineTitleFontSizeCommand : FontSizeCommand {
		new public WpfChartConstantLineModel FontModel { get { return (WpfChartConstantLineModel)ChartModel.SelectedModel; } }
		public ConstantLineTitleFontSizeCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			ConstantLine constantLine = ChartModel.SelectedObject as ConstantLine;
			if(constantLine == null)
				return false;
			if(constantLine.Title != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if(parameter == null || !(parameter is int))
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			double fontSize = (int)parameter;
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.ConstantLine.Title, FontModel.ConstantLine.Title.FontSize, fontSize);
			FontModel.ConstantLine.Title.FontSize = fontSize;
			return new CommandResult(historyItem);
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.DiagramModel.Diagram;
			return FontCommandUtils.GetConstantLineTitlePathIndexes(null, (ConstantLine)ChartModel.SelectedObject, diagram);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = FontCommandUtils.GetConstantLineFontHolder(chartControl, historyItem);
			return constantLine.Title;
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem constantLine = FontCommandUtils.GetConstantLineFontHolder(chartModelItem, historyItem);
			return constantLine.Properties["Title"].Value;
		}
	}
	public class ConstantLineTitleFontBoldCommand : FontBoldCommand {
		new public WpfChartConstantLineModel FontModel { get { return (WpfChartConstantLineModel)ChartModel.SelectedModel; } }
		public ConstantLineTitleFontBoldCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			ConstantLine constantLine = ChartModel.SelectedObject as ConstantLine;
			if(constantLine == null)
				return false;
			if(constantLine.Title != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if(parameter == null)
				return null;
			FontWeight fontWeight = (bool)parameter ? FontWeights.Bold : FontWeights.Normal;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.ConstantLine.Title, FontModel.ConstantLine.Title.FontWeight, fontWeight);
			FontModel.ConstantLine.Title.FontWeight = fontWeight;
			return new CommandResult(historyItem);
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.DiagramModel.Diagram;
			return FontCommandUtils.GetConstantLineTitlePathIndexes(null, (ConstantLine)ChartModel.SelectedObject, diagram);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = FontCommandUtils.GetConstantLineFontHolder(chartControl, historyItem);
			return constantLine.Title;
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem constantLine = FontCommandUtils.GetConstantLineFontHolder(chartModelItem, historyItem);
			return constantLine.Properties["Title"].Value;
		}
	}
	public class ConstantLineTitleFontItalicCommand : FontItalicCommand {
		new public WpfChartConstantLineModel FontModel { get { return (WpfChartConstantLineModel)ChartModel.SelectedModel; } }
		public ConstantLineTitleFontItalicCommand(WpfChartModel chartModel)
			: base(chartModel) {
		}
		protected override bool CanExecute(object parameter) {
			ConstantLine constantLine = ChartModel.SelectedObject as ConstantLine;
			if(constantLine == null)
				return false;
			if(constantLine.Title != null)
				return true;
			else
				return false;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if(parameter == null)
				return null;
			FontStyle fontStyle = (bool)parameter ? FontStyles.Italic : FontStyles.Normal;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.ConstantLine.Title, FontModel.ConstantLine.Title.FontStyle, fontStyle);
			FontModel.ConstantLine.Title.FontStyle = fontStyle;
			return new CommandResult(historyItem);
		}
		protected override ElementIndexItem[] GetPathIndexes() {
			XYDiagram2D diagram = (XYDiagram2D)ChartModel.DiagramModel.Diagram;
			return FontCommandUtils.GetConstantLineTitlePathIndexes(null, (ConstantLine)ChartModel.SelectedObject, diagram);
		}
		protected override Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			ConstantLine constantLine = FontCommandUtils.GetConstantLineFontHolder(chartControl, historyItem);
			return constantLine.Title;
		}
		protected override IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem constantLine = FontCommandUtils.GetConstantLineFontHolder(chartModelItem, historyItem);
			return constantLine.Properties["Title"].Value;
		}
	}
	#endregion
}
