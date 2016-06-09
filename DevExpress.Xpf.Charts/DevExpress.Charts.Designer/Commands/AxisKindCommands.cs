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

using System;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class AxisKindCommandBase : AxisConditionalCommandBase {
		protected virtual AxisAlignment AxisAlignment {
			get { return SelectedAxisModel.Alignment; }
		}
		protected abstract Type AxisType {
			get;
		}
		protected virtual bool AxisVisibility {
			get { return true; }
		}
		public AxisKindCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool AxisTypeMatchCommandType(WpfChartAxisModel axisModel) {
			return axisModel.Axis.GetType() == AxisType || axisModel.Axis.GetType().IsSubclassOf(AxisType);
		}
		public override bool IsAxisAtCommandState(WpfChartAxisModel axisModel) {
			return AxisTypeMatchCommandType(axisModel) && axisModel.IsVisible == AxisVisibility && axisModel.Alignment == AxisAlignment;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter is bool && (bool)parameter) {
				CompositeHistoryItem resultItem = new CompositeHistoryItem();
				if (SelectedAxisModel.IsVisible != AxisVisibility) {
					CommandResult toggleVisibilityResult = new ToggleAxisVisibilityCommand(ChartModel).RuntimeExecute(AxisVisibility);
					resultItem.HistoryItems.Add(toggleVisibilityResult.HistoryItem);
				}
				if (SelectedAxisModel.Alignment != AxisAlignment) {
					CommandResult changeAlignmentResult = new ChangeAxisAlignmentCommand(ChartModel).RuntimeExecute(AxisAlignment);
					resultItem.HistoryItems.Add(changeAlignmentResult.HistoryItem);
				}
				AxisBase resultAxis = SelectedAxisModel.Axis;
				ChartModel.SelectedObject = PreviewChart;
				return new CommandResult(resultItem, resultAxis);
			}
			return null;
		}
	}
	public abstract class AxisXKindCommandBase : AxisKindCommandBase {
		protected override Type AxisType {
			get { return typeof(AxisX2D); }
		}
		public AxisXKindCommandBase(WpfChartModel model)
			: base(model) { }
	}
	public class AxisXKindNearNotReverseCommand : AxisXKindCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_BottomCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_BottomDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\AxisXBottom";
		protected override AxisAlignment AxisAlignment {
			get { return AxisAlignment.Near; }
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
		public AxisXKindNearNotReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisX2D;
		}
	}
	public class AxisXKindFarNotReverseCommand : AxisXKindCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_TopCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_TopDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\AxisXTop";
		protected override AxisAlignment AxisAlignment {
			get { return AxisAlignment.Far; }
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
		public AxisXKindFarNotReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisX2D;
		}
	}
	public abstract class AxisYKindCommandBase : AxisKindCommandBase {
		protected override Type AxisType {
			get { return typeof(AxisY2D); }
		}
		public AxisYKindCommandBase(WpfChartModel model)
			: base(model) { }
	}
	public class AxisYKindNearNotReverseCommand : AxisYKindCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_LeftCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_LeftDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\AxisYLeft";
		protected override AxisAlignment AxisAlignment {
			get { return AxisAlignment.Near; }
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
		public AxisYKindNearNotReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisY2D;
		}
	}
	public class AxisYKindFarNotReverseCommand : AxisYKindCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_RightCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_RightDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\AxisYRight";
		protected override AxisAlignment AxisAlignment {
			get { return AxisAlignment.Far; }
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
		public AxisYKindFarNotReverseCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is AxisY2D;
		}
	}
	public class CircularAxisYKindVisibleCommand : AxisKindCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_CircularAxisYVisibleCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKind_CircularAxisYVisibleDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\CircularAxisYVisible";
		protected override Type AxisType {
			get { return typeof(CircularAxisY2D); }
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
		public CircularAxisYKindVisibleCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is CircularAxisY2D;
		}
		public override bool IsAxisAtCommandState(WpfChartAxisModel axisModel) {
			return axisModel.Axis.GetType().IsSubclassOf(AxisType) && axisModel.IsVisible;
		}
	}
	public class AxisKindNoneCommand : AxisKindCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKindNoneCaption);
		readonly string description = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.AxisOptions_AxisKindNoneDescription);
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\None";
		protected override Type AxisType {
			get { return typeof(AxisBase); }
		}
		protected override bool AxisVisibility {
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
		public AxisKindNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is Axis2D;
		}
	}
	public class CircularAxisYKindNoneCommand : AxisKindNoneCommand {
		readonly string imageName = GlyphUtils.GalleryItemImages + "AxisKind\\CircularAxisYNone";
		protected override Type AxisType {
			get { return typeof(CircularAxisY2D); }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public CircularAxisYKindNoneCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return SelectedAxisModel != null && SelectedAxisModel.Axis is CircularAxisY2D;
		}
	}
}
