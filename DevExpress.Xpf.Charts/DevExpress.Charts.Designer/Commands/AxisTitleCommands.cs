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
	public abstract class AxisTitlePositionCommandBase : AxisConditionalCommandBase {
		protected virtual TitleAlignment TitleAlignment {
			get { return SelectedAxisModel.Title.Alignment; }
		}
		protected virtual bool TitleVisibility {
			get { return true; }
		}
		public AxisTitlePositionCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && (SelectedAxisModel != null) && SelectedAxisModel.IsVisible;
		}
		public override bool IsAxisAtCommandState(WpfChartAxisModel axisModel) {
			return AxisTypeMatchCommandType(axisModel) && axisModel.Title.Visible == TitleVisibility && axisModel.Title.Alignment == TitleAlignment;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter is bool && (bool)parameter) {
				CompositeHistoryItem resultItem = new CompositeHistoryItem();
				if (SelectedAxisModel.Title.Visible != TitleVisibility) {
					CommandResult toggleVisibilityResult = new ToggleAxisTitleVisibilityCommand(ChartModel).RuntimeExecute(TitleVisibility);
					resultItem.HistoryItems.Add(toggleVisibilityResult.HistoryItem);
				}
				if (SelectedAxisModel.Title.Alignment != TitleAlignment) {
					CommandResult changeAlignmentResult = new ChangeAxisTitleAlignmentCommand(ChartModel).RuntimeExecute(TitleAlignment);
					resultItem.HistoryItems.Add(changeAlignmentResult.HistoryItem);
				}
				AxisBase resultAxis = SelectedAxisModel.Axis;
				ChartModel.SelectedObject = PreviewChart;
				return new CommandResult(resultItem, resultAxis);
			}
			return null;
		}
	}
	public abstract class AxisXTitlePositionCommandBase : AxisTitlePositionCommandBase {
		public AxisXTitlePositionCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return SelectedAxisModel.Axis is AxisX2D || SelectedAxisModel.Axis is AxisX3D;
		}
	}
	public class AxisXTitlePositionNearCommand : AxisXTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_LeftCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_LeftDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\AxisXNear";
		protected override TitleAlignment TitleAlignment {
			get { return TitleAlignment.Near; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisXTitlePositionNearCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXTitlePositionCenterCommand : AxisXTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_CenterCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_CenterDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\AxisXCenter";
		protected override TitleAlignment TitleAlignment {
			get { return TitleAlignment.Center; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisXTitlePositionCenterCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXTitlePositionFarCommand : AxisXTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_RightCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_RightDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\AxisXFar";
		protected override TitleAlignment TitleAlignment {
			get { return TitleAlignment.Far; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisXTitlePositionFarCommand(WpfChartModel model)
			: base(model) { }
	}
	public abstract class AxisYTitlePositionCommandBase : AxisTitlePositionCommandBase {
		public AxisYTitlePositionCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return SelectedAxisModel.Axis is AxisY2D || SelectedAxisModel.Axis is AxisY3D;
		}
	}
	public class AxisYTitlePositionNearCommand : AxisYTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_BottomCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_BottomDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\AxisYNear";
		protected override TitleAlignment TitleAlignment {
			get { return TitleAlignment.Near; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisYTitlePositionNearCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisYTitlePositionCenterCommand : AxisYTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_CenterCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_CenterDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\AxisYCenter";
		protected override TitleAlignment TitleAlignment {
			get { return TitleAlignment.Center; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisYTitlePositionCenterCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisYTitlePositionFarCommand : AxisYTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_TopCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_TopDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\AxisYFar";
		protected override TitleAlignment TitleAlignment {
			get { return TitleAlignment.Far; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisYTitlePositionFarCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisTitlePositionNoneCommand : AxisTitlePositionCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitlePosition_NoneCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitlePosition_NoneDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisTitlePosition\\None";
		protected override bool TitleVisibility {
			get { return false; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public AxisTitlePositionNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return SelectedAxisModel.Axis is Axis;
		}
	}
	public class AddAxisTitleCommand : AxisOptionsCommandBase {
		readonly string defaultAxisTitleText = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultAxisTitleText);
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public AddAxisTitleCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title = (AxisTitle)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title = (AxisTitle)historyItem.NewValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title = (AxisTitle)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title = (AxisTitle)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title = (AxisTitle)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title = (AxisTitle)historyItem.NewValue;
			}
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title = null;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title = null;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title = null;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title = null;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title = null;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title = null;
			}
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Title"].SetValue(new AxisTitle());
			axisItem.Properties["Title"].Value.Properties["Content"].SetValue(defaultAxisTitleText);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title = new AxisTitle() { Content = defaultAxisTitleText };
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title = new AxisTitle() { Content = defaultAxisTitleText };
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Title = new AxisTitle() { Content = defaultAxisTitleText };
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Title = new AxisTitle() { Content = defaultAxisTitleText };
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chartControl.Diagram).AxisX.Title = new AxisTitle() { Content = defaultAxisTitleText };
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chartControl.Diagram).AxisY.Title = new AxisTitle() { Content = defaultAxisTitleText };
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			Axis axis = (Axis)SelectedAxisModel.Axis;
			axis.Title = new AxisTitle();
			axis.Title.Content = defaultAxisTitleText;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] {new ElementIndexItem(axisIndexKey, axisIndex)} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, axis, null, axis.Title));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ToggleAxisTitleVisibilityCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitleVisibility);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisTitleVisibilityCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is Axis && SelectedAxisModel.IsVisible;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Visible = !(bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Visible = !(bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title.Visible = !(bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title.Visible = !(bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title.Visible = !(bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title.Visible = !(bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Title"].Value.Properties["Visible"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chartControl.Diagram).AxisX.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chartControl.Diagram).AxisY.Title.Visible = (bool)historyItem.ExecuteCommandInfo.Parameter;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			if (((Axis)SelectedAxisModel.Axis).Title == null) {
				AddAxisTitleCommand addTitleCommand = new AddAxisTitleCommand(ChartModel);
				CommandResult addAxisResult = addTitleCommand.RuntimeExecute(null);
				resultItem.HistoryItems.Add(addAxisResult.HistoryItem);
				ChartModel.RecursivelyUpdateChildren();
			}
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			SelectedAxisModel.Title.Visible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, null, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class SetAxisTitleContentCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public SetAxisTitleContentCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && SelectedAxisModel.Title != null && SelectedAxisModel.Title.Visible;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Content = historyItem.NewValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title.Content = historyItem.NewValue;
			}
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Content = historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Content = historyItem.OldValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title.Content = historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title.Content = historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title.Content = historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title.Content = historyItem.OldValue;
			}
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Title"].Value.Properties["Content"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Content = historyItem.NewValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chartControl.Diagram).AxisX.Title.Content = historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chartControl.Diagram).AxisY.Title.Content = historyItem.NewValue;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			string oldValue = SelectedAxisModel.Title.DisplayName;
			SelectedAxisModel.Title.DisplayName = (string)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, oldValue, parameter));
		}
	}
	public class ChangeAxisTitleAlignmentCommand : AxisOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_TitlePosition);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeAxisTitleAlignmentCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && SelectedAxisModel.Title != null && SelectedAxisModel.Title.Visible;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Alignment = (TitleAlignment)historyItem.NewValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title.Alignment = (TitleAlignment)historyItem.NewValue;
			}
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Alignment = (TitleAlignment)historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Alignment = (TitleAlignment)historyItem.OldValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisX.Title.Alignment = (TitleAlignment)historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)PreviewChart.Diagram).AxisY.Title.Alignment = (TitleAlignment)historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisX.Title.Alignment = (TitleAlignment)historyItem.OldValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)PreviewChart.Diagram).AxisY.Title.Alignment = (TitleAlignment)historyItem.OldValue;
			}
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Title"].Value.Properties["Alignment"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Title.Alignment = (TitleAlignment)historyItem.NewValue;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chartControl.Diagram).AxisX.Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chartControl.Diagram).AxisY.Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chartControl.Diagram).AxisX.Title.Alignment = (TitleAlignment)historyItem.NewValue;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chartControl.Diagram).AxisY.Title.Alignment = (TitleAlignment)historyItem.NewValue;
			}
		}
		public override CommandResult RuntimeExecute(object parameter) {
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			TitleAlignment oldValue = SelectedAxisModel.Title.Alignment;
			SelectedAxisModel.Title.Alignment = (TitleAlignment)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, oldValue, parameter));
		}
	}
}
