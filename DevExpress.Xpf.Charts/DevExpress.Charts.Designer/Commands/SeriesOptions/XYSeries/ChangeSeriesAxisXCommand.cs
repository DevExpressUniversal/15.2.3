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
	public class ChangeSeriesAxisXCommand : SeriesOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_ArgumentAxis);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeSeriesAxisXCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && SeriesModel != null && SeriesModel.Series is XYSeries2D;
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) { }
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			int axisIndex = (int)newValue;
			if (axisIndex == -1)
				model.AxisX = ChartModel.DiagramModel.PrimaryAxisModelX;
			else
				model.AxisX = (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelX.ModelCollection[axisIndex];
		}
		protected override void RuntimeApplyInternal(Series series, object value) { }
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			int oldAxisIndex = (int)oldValue;
			if (oldAxisIndex == -1)
				model.AxisX = ChartModel.DiagramModel.PrimaryAxisModelX;
			else
				model.AxisX = (WpfChartAxisModel)ChartModel.DiagramModel.SecondaryAxesCollectionModelX.ModelCollection[oldAxisIndex];
		}
		[ SecuritySafeCritical ]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			int axisIndex;
			IModelItem bindingItem = null;
			if (historyItem.ExecuteCommandInfo.IndexByNameDictionary.TryGetValue(AxisIndexKey, out axisIndex)) {
				IModelItem axisModelItem = chartModelItem.Properties["Diagram"].Value.Properties["SecondaryAxesX"].Collection[axisIndex];
				string axisName = "axisx" + axisIndex;
				axisModelItem.Properties["Name"].SetValue(axisName);
				bindingItem = designTimeProvider.CreateBindingItem(seriesAccess, axisName);
			}
			DXPropertyIdentifier axisXPropIdentifier = new DXPropertyIdentifier(typeof(XYDiagram2D), "SeriesAxisX");
			seriesAccess.Properties[axisXPropIdentifier].SetValue(bindingItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			int axisIndex;
			SecondaryAxisX2D seriesAxis = null;
			if (historyItem.ExecuteCommandInfo.IndexByNameDictionary.TryGetValue(AxisIndexKey, out axisIndex))
				seriesAxis = ((XYDiagram2D)chartControl.Diagram).SecondaryAxesX[axisIndex];
			XYDiagram2D.SetSeriesAxisX((XYSeries)series, seriesAxis);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			WpfChartAxisModel axisModel = (WpfChartAxisModel)parameter;
			int oldAxisIndex = SeriesModel.AxisX.GetSelfIndex();
			SeriesModel.AxisX = axisModel;
			int axisIndex = axisModel.GetSelfIndex();
			ElementIndexItem[] pathIndexes = new ElementIndexItem[2];
			if (axisIndex >= 0) {
				pathIndexes = new ElementIndexItem[3];
				pathIndexes[2] = new ElementIndexItem(AxisIndexKey, axisIndex);
			}
			pathIndexes[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
			pathIndexes[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			WpfChartSeriesModel seriesModel = SeriesModel;
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndexes), this, null, oldAxisIndex, axisIndex), seriesModel.Series);
		}
	}
}
