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
using System.Security;
using DevExpress.Xpf.Charts;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public class ChangeIndicatorAxisYCommand : IndicatorCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Indicator_AxisY);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeIndicatorAxisYCommand(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && SelectedIndicatorModel != null && SelectedIndicator is SeparatePaneIndicator;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			WpfChartIndicatorModel indicatorModel = GetIndicatorModelForUndoRedo(historyItem.ExecuteCommandInfo);
			int axisIndex = (int)historyItem.NewValue;
			if (axisIndex == -1)
				indicatorModel.AxisY = ChartModel.DiagramModel.PrimaryAxisModelY;
			else
				indicatorModel.AxisY = (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelY.ModelCollection[axisIndex];
			return indicatorModel != null ? indicatorModel.Indicator : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			WpfChartIndicatorModel indicatorModel = GetIndicatorModelForUndoRedo(historyItem.ExecuteCommandInfo);
			int oldPaneIndex = (int)historyItem.OldValue;
			if (oldPaneIndex == -1)
				indicatorModel.AxisY = ChartModel.DiagramModel.PrimaryAxisModelY;
			else
				indicatorModel.AxisY = (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelY.ModelCollection[oldPaneIndex];
			return indicatorModel != null ? indicatorModel.Indicator : null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem indicatorModelItem = GetIndicatorModelItemForDesignTimeApply(chartModelItem, historyItem.ExecuteCommandInfo);
			int axisIndex = (int)historyItem.NewValue;
			IModelItem bindingItem = null;
			if (axisIndex >= 0) {
				IModelItem axisModelItem = chartModelItem.Properties["Diagram"].Value.Properties["SecondaryAxesY"].Collection[axisIndex];
				string axisName = "axisy" + axisIndex;
				axisModelItem.Properties["Name"].SetValue(axisName);
				bindingItem = designTimeProvider.CreateBindingItem(indicatorModelItem, axisName);
			}
			DXPropertyIdentifier axisYPropIdentifier = new DXPropertyIdentifier(typeof(XYDiagram2D), "IndicatorAxisY");
			indicatorModelItem.Properties[axisYPropIdentifier].SetValue(bindingItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			var separatePaneIndicator = (SeparatePaneIndicator)GetIndicatorForRuntimeApply(chartControl, historyItem.ExecuteCommandInfo);
			int axisIndex = (int)historyItem.NewValue;
			SecondaryAxisY2D indicatorAxis = null;
			if (axisIndex != -1)
				indicatorAxis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesY[axisIndex];
			XYDiagram2D.SetIndicatorAxisY(separatePaneIndicator, indicatorAxis);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			var axisModel = (WpfChartAxisModel)parameter;
			int oldAxisIndex = SelectedIndicatorModel.AxisY.GetSelfIndex();
			SelectedIndicatorModel.AxisY = axisModel;
			int axisIndex = axisModel.GetSelfIndex();
			ElementIndexItem[] elementIndexes = GetElementIndexes();
			WpfChartIndicatorModel indicatorModel = SelectedIndicatorModel;
			var executeCommandInfo = new ExecuteCommandInfo(parameter, elementIndexes);
			var historyItem = new HistoryItem(executeCommandInfo, this, null, oldAxisIndex, axisIndex);
			return new CommandResult(historyItem, indicatorModel.Indicator);
		}
	}
}
