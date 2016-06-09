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
	public class SetSimpleDiagramLayoutDimensionCommand<DiagramType> : ChartCommandBase where DiagramType : Diagram {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDimensionWithColon);
		public override string Caption {
			get {
				if (typeof(DiagramType) == typeof(SimpleDiagram3D))
					return caption;
				else
					return null;
			}
		}
		public override string ImageName {
			get { return null; }
		}
		public SetSimpleDiagramLayoutDimensionCommand(WpfChartModel chartModel)
			: base(chartModel) {
				if ( !(typeof(DiagramType) == typeof(SimpleDiagram2D) || typeof(DiagramType) == typeof(SimpleDiagram3D)) )
					throw new ChartDesignerException("SetSimpleDiagramLayoutDimentionCommand can work with SimpleDiagram2D or SimpleDiagram3D only");
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel == null)
				return false;
			if (ChartModel.DiagramModel.Diagram is DiagramType)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.Dimension = (int)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.Dimension = (int)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["Dimension"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (chartControl.Diagram is SimpleDiagram2D)
				((SimpleDiagram2D)chartControl.Diagram).Dimension = (int)historyItem.NewValue;
			else
				((SimpleDiagram3D)chartControl.Diagram).Dimension = (int)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			int oldValue = ChartModel.DiagramModel.Dimension;
			int newValue = Convert.ToInt32(parameter);
			ChartModel.DiagramModel.Dimension = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class SetSimpleDiagramLayoutDirectionCommand<DiagramType> : ChartCommandBase where DiagramType : Diagram {
		readonly string captionHorizontal = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDirectionHorizontalCaption);
		readonly string captionVertical = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDirectionVerticalCaption);
		readonly string layoutHorizontalImageName = GlyphUtils.GalleryItemImages + "SimpleDiagram2DLayoutDirection/Horizontal";
		readonly string layoutVerticalImageName = GlyphUtils.GalleryItemImages + "SimpleDiagram2DLayoutDirection/Vertical";
		readonly LayoutDirection layoutDirection;
		public override string Caption {
			get {
				if (layoutDirection == LayoutDirection.Horizontal)
					return captionHorizontal;
				else
					return captionVertical;
			}
		}
		public override string ImageName {
			get {
				if (layoutDirection == LayoutDirection.Horizontal)
					return layoutHorizontalImageName;
				else
					return layoutVerticalImageName;
			}
		}
		public LayoutDirection Direction {
			get { return layoutDirection; }
		}
		public SetSimpleDiagramLayoutDirectionCommand(WpfChartModel chartModel, LayoutDirection direction)
			: base(chartModel) {
				if (!(typeof(DiagramType) == typeof(SimpleDiagram2D) || typeof(DiagramType) == typeof(SimpleDiagram3D)))
					throw new ChartDesignerException("SetSimpleDiagramLayoutDimentionCommand can work with SimpleDiagram2D or SimpleDiagram3D only");
				this.layoutDirection = direction;
		}
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel == null)
				return false;
			if (ChartModel.DiagramModel.Diagram is DiagramType)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.LayoutDirection = (LayoutDirection)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.LayoutDirection = (LayoutDirection)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["LayoutDirection"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			if (chartControl.Diagram is SimpleDiagram2D)
				((SimpleDiagram2D)chartControl.Diagram).LayoutDirection = (LayoutDirection)historyItem.NewValue;
			else
				((SimpleDiagram3D)chartControl.Diagram).LayoutDirection = (LayoutDirection)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			LayoutDirection oldValue = ChartModel.DiagramModel.LayoutDirection;
			LayoutDirection newValue = this.layoutDirection;
			ChartModel.DiagramModel.LayoutDirection = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
}
