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
using System.Security;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class AxisLabelOrientationCommandBase : AxisConditionalCommandBase {
		protected virtual int LabelAngle {
			get { return SelectedAxisModel.Label.Angle; }
		}
		protected virtual bool LabelStaggered {
			get { return SelectedAxisModel.Label.Staggered; }
		}
		protected virtual bool LabelVisibility {
			get { return true; }
		}
		public AxisLabelOrientationCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && (SelectedAxisModel != null) && SelectedAxisModel.IsVisible;
		}
		public override bool IsAxisAtCommandState(WpfChartAxisModel axisModel) {
			return AxisTypeMatchCommandType(axisModel) && axisModel.Label.Visible == LabelVisibility && axisModel.Label.Staggered == LabelStaggered && axisModel.Label.Angle == LabelAngle;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter is bool && (bool)parameter) {
				CompositeHistoryItem resultItem = new CompositeHistoryItem();
				if (SelectedAxisModel.Label.Visible != LabelVisibility) {
					CommandResult toggleVisibilityResult = new ToggleAxisLabelsVisibilityCommand(ChartModel).RuntimeExecute(LabelVisibility);
					resultItem.HistoryItems.Add(toggleVisibilityResult.HistoryItem);
				}
				if (SelectedAxisModel.Label.Staggered != LabelStaggered) {
					CommandResult changeAlignmentResult = new ToggleAxisLabelsStaggeredCommand(ChartModel).RuntimeExecute(LabelStaggered);
					resultItem.HistoryItems.Add(changeAlignmentResult.HistoryItem);
				}
				if (SelectedAxisModel.Label.Angle != LabelAngle) {
					CommandResult setAngleResult = new SetAxisLabelAngleCommand(ChartModel).RuntimeExecute(LabelAngle);
					resultItem.HistoryItems.Add(setAngleResult.HistoryItem);
				}
				AxisBase resultAxis = SelectedAxisModel.Axis;
				ChartModel.SelectedObject = PreviewChart;
				return new CommandResult(resultItem, resultAxis);
			}
			return null;
		}
	}
	public abstract class AxisXLabelOrientationCommandBase : AxisLabelOrientationCommandBase {
		public AxisXLabelOrientationCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is AxisX2D || axisModel.Axis is AxisX3D;
		}
	}
	public class AxisXLabelOrientationNormalCommand : AxisXLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_NormalCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_NormalDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisXNormal";
		protected override int LabelAngle {
			get { return 0; }
		}
		protected override bool LabelStaggered {
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
		public AxisXLabelOrientationNormalCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXLabelOrientationStaggeredCommand : AxisXLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_StaggeredCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_StaggeredDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisXStaggered";
		protected override int LabelAngle {
			get { return 0; }
		}
		protected override bool LabelStaggered {
			get { return true; }
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
		public AxisXLabelOrientationStaggeredCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXLabelOrientationRotated90Command : AxisXLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated90Caption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated90Description);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisX90";
		protected override int LabelAngle {
			get { return 90; }
		}
		protected override bool LabelStaggered {
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
		public AxisXLabelOrientationRotated90Command(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXLabelOrientationRotatedMinus90Command : AxisXLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus90Caption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus90Description);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisXMinus90";
		protected override int LabelAngle {
			get { return -90; }
		}
		protected override bool LabelStaggered {
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
		public AxisXLabelOrientationRotatedMinus90Command(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXLabelOrientationRotated45Command : AxisXLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated45Caption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated45Description);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisX45";
		protected override int LabelAngle {
			get { return 45; }
		}
		protected override bool LabelStaggered {
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
		public AxisXLabelOrientationRotated45Command(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXLabelOrientationRotatedMinus45Command : AxisXLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus45Caption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus45Description);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisXMinus45";
		protected override int LabelAngle {
			get { return -45; }
		}
		protected override bool LabelStaggered {
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
		public AxisXLabelOrientationRotatedMinus45Command(WpfChartModel model)
			: base(model) { }
	}
	public abstract class AxisYLabelOrientationCommanBase : AxisLabelOrientationCommandBase {
		public AxisYLabelOrientationCommanBase(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is AxisY2D || axisModel.Axis is AxisY3D;
		}
	}
	public class AxisYLabelOrientationNormalCommand : AxisYLabelOrientationCommanBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_NormalCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_DescriptionCaption);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisYNormal";
		protected override int LabelAngle {
			get { return 0; }
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
		public AxisYLabelOrientationNormalCommand(WpfChartModel model)
			: base(model) { }
	}
	public class AxisYLabelOrientationRotated90Command : AxisYLabelOrientationCommanBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_Rotated90Caption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_Rotated90Description);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisY90";
		protected override int LabelAngle {
			get { return 90; }
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
		public AxisYLabelOrientationRotated90Command(WpfChartModel model)
			: base(model) { }
	}
	public class AxisYLabelOrientationRotatedMinus90Command : AxisYLabelOrientationCommanBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_RotatedMinus90Caption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_RotatedMinus90Description);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisYMinus90";
		protected override int LabelAngle {
			get { return -90; }
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
		public AxisYLabelOrientationRotatedMinus90Command(WpfChartModel model)
			: base(model) { }
	}
	public class CircularAxisXLabelOrientationNormalCommand : AxisLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_CircularXLabelOrientation_NormalCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_CircularXLabelOrientation_NormalDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\CircularAxisXNormal";
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public CircularAxisXLabelOrientationNormalCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is CircularAxisX2D;
		}
	}
	public class CircularAxisYLabelOrientationNormalCommand : AxisLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_CircularYLabelOrientation_NormalCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_CircularYLabelOrientation_NormalDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\CircularAxisYNormal";
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public CircularAxisYLabelOrientationNormalCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is CircularAxisY2D;
		}
	}
	public abstract class AxisLabelOrientationNoneCommandBase : AxisLabelOrientationCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisLabelNoneCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisLabelNoneDescription);
		protected override bool LabelVisibility {
			get { return false; }
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public AxisLabelOrientationNoneCommandBase(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXLabelOrientationNoneCommand : AxisLabelOrientationNoneCommandBase {
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisXNone";
		public override string ImageName {
			get { return imageName; }
		}
		public AxisXLabelOrientationNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is AxisX2D || axisModel.Axis is AxisX3D;
		}
	}
	public class AxisYLabelOrientationNoneCommand : AxisLabelOrientationNoneCommandBase {
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\AxisYNone";
		public override string ImageName {
			get { return imageName; }
		}
		public AxisYLabelOrientationNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is AxisY2D || axisModel.Axis is AxisY3D;
		}
	}
	public class CircularAxisXLabelOrientationNoneCommand : AxisLabelOrientationNoneCommandBase {
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\CircularAxisXNone";
		public override string ImageName {
			get { return imageName; }
		}
		public CircularAxisXLabelOrientationNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is CircularAxisX2D;
		}
	}
	public class CircularAxisYLabelOrientationNoneCommand : AxisLabelOrientationNoneCommandBase {
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisLabelOrientation\\CircularAxisYNone";
		public override string ImageName {
			get { return imageName; }
		}
		public CircularAxisYLabelOrientationNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis is CircularAxisY2D;
		}
	}
	public class AddAxisLabelCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public AddAxisLabelCommand(WpfChartModel model)
			: base(model) { }
		void SetAxisLabelValue(HistoryItem historyItem, ChartControl chart, AxisLabel value) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label = value;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label = value;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chart.Diagram).AxisX.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chart.Diagram).AxisY.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chart.Diagram).AxisX.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chart.Diagram).AxisY.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisX2D))
					((PolarDiagram2D)chart.Diagram).AxisX.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					((PolarDiagram2D)chart.Diagram).AxisY.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisX2D))
					((RadarDiagram2D)chart.Diagram).AxisX.Label = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					((RadarDiagram2D)chart.Diagram).AxisY.Label = value;
			}
		}
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			IModelItem labelItem = chartModelItem.Context.CreateItem(typeof(AxisLabel));
			axisItem.Properties["Label"].SetValue(labelItem);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			SetAxisLabelValue(historyItem, PreviewChart, (AxisLabel)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			SetAxisLabelValue(historyItem, PreviewChart, null);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			SetAxisLabelValue(historyItem, chartControl, new AxisLabel());
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			int axisIndex = CreateAxisIfNeeded(resultItem);
			AxisBase axis = SelectedAxisModel.Axis;
			axis.Label = new AxisLabel();
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, axis, null, axis.Label));
			return new CommandResult(resultItem, axis);
		}
	}
	public class ToggleAxisLabelsVisibilityCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisLabelsVisibilityCommand(WpfChartModel model)
			: base(model) { }
		void SetAxisLabelVisibility(HistoryItem historyItem, ChartControl chart, bool value) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label.Visible = value;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chart.Diagram).AxisX.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chart.Diagram).AxisY.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chart.Diagram).AxisX.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chart.Diagram).AxisY.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisX2D))
					((PolarDiagram2D)chart.Diagram).AxisX.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					((PolarDiagram2D)chart.Diagram).AxisY.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisX2D))
					((RadarDiagram2D)chart.Diagram).AxisX.Label.Visible = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					((RadarDiagram2D)chart.Diagram).AxisY.Label.Visible = value;
			}
		}
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && SelectedAxisModel.Label != null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Label"].Value.Properties["Visible"].SetValue(historyItem.NewValue);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			SetAxisLabelVisibility(historyItem, PreviewChart, (bool)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			SetAxisLabelVisibility(historyItem, PreviewChart, (bool)historyItem.OldValue);
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			SetAxisLabelVisibility(historyItem, chartControl, (bool)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			if (SelectedAxisModel.Axis.Label == null) {
				AddAxisLabelCommand addLabelCommand = new AddAxisLabelCommand(ChartModel);
				CommandResult addLabelResult = addLabelCommand.RuntimeExecute(null);
				resultItem.HistoryItems.Add(addLabelResult.HistoryItem);
				ChartModel.RecursivelyUpdateChildren();
			}
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			SelectedAxisModel.Label.Visible = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, !(bool)parameter, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class ToggleAxisLabelsStaggeredCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ToggleAxisLabelsStaggeredCommand(WpfChartModel model)
			: base(model) { }
		void SetAxisLabelStaggered(HistoryItem historyItem, ChartControl chart, bool value) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label.Staggered = value;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chart.Diagram).AxisX.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chart.Diagram).AxisY.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chart.Diagram).AxisX.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chart.Diagram).AxisY.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisX2D))
					((PolarDiagram2D)chart.Diagram).AxisX.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					((PolarDiagram2D)chart.Diagram).AxisY.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisX2D))
					((RadarDiagram2D)chart.Diagram).AxisX.Label.Staggered = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					((RadarDiagram2D)chart.Diagram).AxisY.Label.Staggered = value;
			}
		}
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && SelectedAxisModel.Axis is Axis && SelectedAxisModel.Label != null && SelectedAxisModel.Label.Visible;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			SetAxisLabelStaggered(historyItem, PreviewChart, (bool)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			SetAxisLabelStaggered(historyItem, PreviewChart, (bool)historyItem.OldValue);
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Label"].Value.Properties["Staggered"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			SetAxisLabelStaggered(historyItem, chartControl, (bool)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			if (SelectedAxisModel.Axis.Label == null) {
				AddAxisLabelCommand addLabelCommand = new AddAxisLabelCommand(ChartModel);
				resultItem.HistoryItems.Add(addLabelCommand.RuntimeExecute(null).HistoryItem);
				ChartModel.RecursivelyUpdateChildren();
			}
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			SelectedAxisModel.Label.Staggered = (bool)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, !(bool)parameter, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
	public class SetAxisLabelAngleCommand : AxisOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public SetAxisLabelAngleCommand(WpfChartModel model)
			: base(model) { }
		void SetAxisLabelAngle(HistoryItem historyItem, ChartControl chart, int value) {
			if (historyItem.ExecuteCommandInfo.IsValidPath) {
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisX2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesX[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(SecondaryAxisY2D))
					((XYDiagram2D)chart.Diagram).SecondaryAxesY[historyItem.ExecuteCommandInfo.IndexByNameDictionary[axisIndexKey]].Label.Angle = value;
			}
			else {
				if (historyItem.TargetObject.GetType() == typeof(AxisX2D))
					((XYDiagram2D)chart.Diagram).AxisX.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY2D))
					((XYDiagram2D)chart.Diagram).AxisY.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisX3D))
					((XYDiagram3D)chart.Diagram).AxisX.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(AxisY3D))
					((XYDiagram3D)chart.Diagram).AxisY.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisX2D))
					((PolarDiagram2D)chart.Diagram).AxisX.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(PolarAxisY2D))
					((PolarDiagram2D)chart.Diagram).AxisY.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisX2D))
					((RadarDiagram2D)chart.Diagram).AxisX.Label.Angle = value;
				if (historyItem.TargetObject.GetType() == typeof(RadarAxisY2D))
					((RadarDiagram2D)chart.Diagram).AxisY.Label.Angle = value;
			}
		}
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.IsVisible && SelectedAxisModel.Axis is Axis && SelectedAxisModel.Label != null && SelectedAxisModel.Label.Visible;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			SetAxisLabelAngle(historyItem, PreviewChart, (int)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			SetAxisLabelAngle(historyItem, PreviewChart, (int)historyItem.OldValue);
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem axisItem = GetAxisModelItem(chartModelItem, historyItem);
			axisItem.Properties["Label"].Value.Properties["Angle"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			SetAxisLabelAngle(historyItem, chartControl, (int)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			if (SelectedAxisModel.Axis.Label == null) {
				AddAxisLabelCommand addLabelCommand = new AddAxisLabelCommand(ChartModel);
				resultItem.HistoryItems.Add(addLabelCommand.RuntimeExecute(null).HistoryItem);
				ChartModel.RecursivelyUpdateChildren();
			}
			int axisIndex = -1;
			if (SelectedAxisModel.Axis is SecondaryAxisX2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesX.IndexOf((SecondaryAxisX2D)SelectedAxisModel.Axis);
			else if (SelectedAxisModel.Axis is SecondaryAxisY2D)
				axisIndex = ((XYDiagram2D)PreviewChart.Diagram).SecondaryAxesY.IndexOf((SecondaryAxisY2D)SelectedAxisModel.Axis);
			int oldValue = SelectedAxisModel.Label.Angle;
			SelectedAxisModel.Label.Angle = (int)parameter;
			ElementIndexItem[] pathIndex = axisIndex >= 0 ? pathIndex = new ElementIndexItem[] { new ElementIndexItem(axisIndexKey, axisIndex)
			} : null;
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndex), this, SelectedAxisModel.Axis, oldValue, parameter));
			return new CommandResult(resultItem, SelectedAxisModel.Axis);
		}
	}
}
