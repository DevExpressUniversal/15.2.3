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
	public class ChangeDiagram3DPerspectiveAngleCommand : ChartCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_Diagram3DPercpectiveAngle);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeDiagram3DPerspectiveAngleCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel != null && ChartModel.DiagramModel.Diagram is Diagram3D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.PerspectiveAngle = (double)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.PerspectiveAngle = (double)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["PerspectiveAngle"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((Diagram3D)chartControl.Diagram).PerspectiveAngle = (double)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			double oldValue = ChartModel.DiagramModel.PerspectiveAngle;
			double newValue = Convert.ToDouble(parameter);
			ChartModel.DiagramModel.PerspectiveAngle = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
	public class ChangeDiagram3DZoomPercentCommand : ChartCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Default_Main_Diagram3DZoomPercent);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeDiagram3DZoomPercentCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			if (ChartModel.DiagramModel != null && ChartModel.DiagramModel.Diagram is Diagram3D)
				return true;
			else
				return false;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.ZoomPercent = (double)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			ChartModel.DiagramModel.ZoomPercent = (double)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["ZoomPercent"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			((Diagram3D)chartControl.Diagram).ZoomPercent = (double)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			double oldValue = ChartModel.DiagramModel.ZoomPercent;
			double newValue = Convert.ToDouble(parameter);
			ChartModel.DiagramModel.ZoomPercent = newValue;
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
	}
}
