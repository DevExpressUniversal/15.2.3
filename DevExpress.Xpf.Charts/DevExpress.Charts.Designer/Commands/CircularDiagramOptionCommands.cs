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
	public class ChangeCircularDiagramStartAngleCommand : ChartCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeCircularDiagramStartAngleCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel != null && ChartModel.DiagramModel.Diagram is CircularDiagram2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.StartAngle = (double)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.StartAngle = (double)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["StartAngle"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((CircularDiagram2D)chartControl.Diagram).StartAngle = (double)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			double oldValue = ChartModel.DiagramModel.StartAngle;
			double newValue = Convert.ToDouble(parameter);
			ChartModel.DiagramModel.StartAngle = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeCircularDiagramRotationDirection : ChartCommandBase {
		readonly CircularDiagramRotationDirection rotationDirection;
		readonly string clocwiseCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramRotationDirectionClocwise);
		readonly string conterclocwiseCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramRotationDirectionConterclocwise);
		readonly string clocwiseImageName = GlyphUtils.GalleryItemImages + "CircularDiagramRotationDirection/Clockwise";
		readonly string conterclockwiseImageName = GlyphUtils.GalleryItemImages + "CircularDiagramRotationDirection/Conterclockwise";
		public override string Caption {
			get {
				if (this.rotationDirection == CircularDiagramRotationDirection.Clockwise)
					return clocwiseCaption;
				else
					return conterclocwiseCaption;
			}
		}
		public override string ImageName {
			get {
				if (this.rotationDirection == CircularDiagramRotationDirection.Clockwise)
					return clocwiseImageName;
				else
					return conterclockwiseImageName;
			}
		}
		public CircularDiagramRotationDirection RotationDirection {
			get { return rotationDirection; }
		}
		public ChangeCircularDiagramRotationDirection(WpfChartModel chartModel, CircularDiagramRotationDirection rotationDirection)
			: base(chartModel) {
				this.rotationDirection = rotationDirection;
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel != null && ChartModel.DiagramModel.Diagram is CircularDiagram2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.RotationDirection = (CircularDiagramRotationDirection)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.RotationDirection = (CircularDiagramRotationDirection)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["RotationDirection"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((CircularDiagram2D)chartControl.Diagram).RotationDirection = (CircularDiagramRotationDirection)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null || (bool)parameter == false)
				return null;
			var newValue = this.rotationDirection;
			var oldValue = ChartModel.DiagramModel.RotationDirection; 
			ChartModel.DiagramModel.RotationDirection = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeCircularDiagramShapeStyle : ChartCommandBase {
		readonly CircularDiagramShapeStyle shapeStyle;
		readonly string circleCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramCircleShapeStyleCaption);
		readonly string polygonCaption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_CircularDiagramPolygonShapeStyleCaption);
		readonly string circleImageName = GlyphUtils.GalleryItemImages + "ShapeStyle/Circle";
		readonly string polygonImageName = GlyphUtils.GalleryItemImages + "ShapeStyle/Polygon";
		public override string Caption {
			get {
				if (shapeStyle == CircularDiagramShapeStyle.Circle)
					return circleCaption;
				else
					return polygonCaption;
			}
		}
		public override string ImageName {
			get {
				if (shapeStyle == CircularDiagramShapeStyle.Circle)
					return circleImageName;
				else
					return polygonImageName;
			}
		}
		public CircularDiagramShapeStyle ShapeStyle {
			get { return shapeStyle; }
		}
		public ChangeCircularDiagramShapeStyle(WpfChartModel chartModel, CircularDiagramShapeStyle shapeStyle)
			: base(chartModel) {
				this.shapeStyle = shapeStyle;
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel != null && ChartModel.DiagramModel.Diagram is CircularDiagram2D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.ShapeStyle = (CircularDiagramShapeStyle)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.ShapeStyle = (CircularDiagramShapeStyle)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["ShapeStyle"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((CircularDiagram2D)chartControl.Diagram).ShapeStyle = (CircularDiagramShapeStyle)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null || (bool)parameter == false)
				return null;
			CircularDiagramShapeStyle newValue = this.shapeStyle;
			CircularDiagramShapeStyle oldValue = ChartModel.DiagramModel.ShapeStyle;
			ChartModel.DiagramModel.ShapeStyle = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
}
