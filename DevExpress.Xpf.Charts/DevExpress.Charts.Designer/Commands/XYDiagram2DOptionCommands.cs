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
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class ToggleXYDiagram2DRotatedCommand : ChartCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_XYDiagramRotated);
		readonly string imageName = GlyphUtils.BarItemImages + "XYDiagram2DRotated";
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ToggleXYDiagram2DRotatedCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel == null)
				return false;
			if (ChartModel.DiagramModel.Diagram is XYDiagram2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.IsRotated = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.IsRotated = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["Rotated"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((XYDiagram2D)chartControl.Diagram).Rotated = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null)
				return null;
			bool value = (bool)parameter;
			bool oldValue = ChartModel.DiagramModel.IsRotated;
			ChartModel.DiagramModel.IsRotated = value;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, value);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangePanesOrientationCommand : ChartCommandBase {
		readonly string captionHorizontal = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_PaneOrientationHorizontal);
		readonly string imageNameHorizontal = GlyphUtils.GalleryItemImages + "AddPane/Horizontal";
		readonly string captionVertical = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_PaneOrientationVertical);
		readonly string imageNameVertical = GlyphUtils.GalleryItemImages + "AddPane/Vertical";
		readonly Orientation orientation;
		public override string Caption {
			get {
				if (orientation == Orientation.Horizontal)
					return captionHorizontal;
				else
					return captionVertical;
			}
		}
		public override string ImageName {
			get {
				if (orientation == Orientation.Horizontal)
					return imageNameHorizontal;
				else
					return imageNameVertical;
			}
		}
		public Orientation Orientation {
			get { return orientation; }
		}
		public ChangePanesOrientationCommand(WpfChartModel model, Orientation newOrientation)
			: base(model) {
			this.orientation = newOrientation;
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel == null)
				return false;
			if (ChartModel.DiagramModel.Diagram is XYDiagram2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			((XYDiagram2D)ChartModel.DiagramModel.Diagram).PaneOrientation = (Orientation)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			((XYDiagram2D)ChartModel.DiagramModel.Diagram).PaneOrientation = (Orientation)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["PaneOrientation"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((XYDiagram2D)chartControl.Diagram).PaneOrientation = (Orientation)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (!(parameter is bool) || !((bool)parameter))
				return null;
			Orientation oldValue = ChartModel.DiagramModel.PaneOrientation;
			ChartModel.DiagramModel.PaneOrientation = this.orientation;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, Orientation.Horizontal);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeDiagramAxisNavigationEnabledCommand : ChartCommandBase {
		public enum AxisKind {
			X,
			Y,
		}
		readonly string enableAxisXNavigationCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_EnableAxisXNavigationCheckCaption);
		readonly string enableAxisYNavigationCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_EnableAxisYNavigationCheckCaption);
		readonly AxisKind axisKind;
		public override string Caption {
			get {
				if (axisKind == AxisKind.X)
					return enableAxisXNavigationCaption;
				else
					return enableAxisYNavigationCaption;
			}
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeDiagramAxisNavigationEnabledCommand(WpfChartModel model, AxisKind axisKind)
			: base(model) {
				this.axisKind = axisKind;
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel == null)
				return false;
			if (ChartModel.DiagramModel.Diagram is XYDiagram2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			bool newValue = (bool)historyItem.NewValue;
			if (axisKind == AxisKind.X)
				ChartModel.DiagramModel.EnableAxisXNavigation = newValue;
			else
				ChartModel.DiagramModel.EnableAxisYNavigation = newValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			bool oldValue = (bool)historyItem.OldValue;
			if (axisKind == AxisKind.X)
				ChartModel.DiagramModel.EnableAxisXNavigation = oldValue;
			else
				ChartModel.DiagramModel.EnableAxisYNavigation = oldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			string propertyName;
			if (axisKind == AxisKind.X)
				propertyName = "EnableAxisXNavigation";
			else
				propertyName = "EnableAxisYNavigation";
			chartModelItem.Properties["Diagram"].Value.Properties[propertyName].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (axisKind == AxisKind.X)
				((XYDiagram2D)chartControl.Diagram).EnableAxisXNavigation = (bool)historyItem.NewValue;
			else
				((XYDiagram2D)chartControl.Diagram).EnableAxisYNavigation = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			bool oldValue;
			bool newValue = (bool)parameter;
			if (axisKind == AxisKind.X) {
				oldValue = ChartModel.DiagramModel.EnableAxisXNavigation;
				ChartModel.DiagramModel.EnableAxisXNavigation = newValue;
			}
			else {
				oldValue = ChartModel.DiagramModel.EnableAxisYNavigation;
				ChartModel.DiagramModel.EnableAxisYNavigation = newValue;
			}
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
}
