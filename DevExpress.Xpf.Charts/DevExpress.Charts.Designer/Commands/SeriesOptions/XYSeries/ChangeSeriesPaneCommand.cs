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
	public class ChangeSeriesPaneCommand : SeriesOptionsCommandBase {
		readonly string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_SeriesPane);
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangeSeriesPaneCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && SeriesModel != null && SeriesModel.Series is XYSeries2D;
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) { }
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			int paneIndex = (int)newValue;
			if (paneIndex == -1)
				model.Pane = ChartModel.DiagramModel.DefaultPaneModel;
			else
				model.Pane = (WpfChartPaneModel)ChartModel.DiagramModel.PanesCollectionModel.ModelCollection[paneIndex];
		}
		protected override void RuntimeApplyInternal(Series series, object value) { }
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			int oldPaneIndex = (int)oldValue;
			if (oldPaneIndex == -1)
				model.Pane = ChartModel.DiagramModel.DefaultPaneModel;
			else
				model.Pane = (WpfChartPaneModel)ChartModel.DiagramModel.PanesCollectionModel.ModelCollection[oldPaneIndex];
		}
		[ SecuritySafeCritical ]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesDesigntimeIndexKey];
			IModelItem seriesAccess = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			int paneIndex;
			IModelItem paneModelItem = null;
			if (historyItem.ExecuteCommandInfo.IndexByNameDictionary.TryGetValue(PaneIndexKey, out paneIndex))
				paneModelItem = chartModelItem.Properties["Diagram"].Value.Properties["Panes"].Collection[paneIndex];
			else {
				paneIndex = -1;
				paneModelItem = chartModelItem.Properties["Diagram"].Value.Properties["DefaultPane"].Value;
				if (paneModelItem == null) {
					chartModelItem.Properties["Diagram"].Value.Properties["DefaultPane"].SetValue(new Pane());
					paneModelItem = chartModelItem.Properties["Diagram"].Value.Properties["DefaultPane"].Value;
				}
			}
			string paneName = paneIndex == -1 ? "pane" : "pane" + paneIndex;
			paneModelItem.Properties["Name"].SetValue(paneName);
			IModelItem bindingItem = designTimeProvider.CreateBindingItem(seriesAccess, paneName);
			DXPropertyIdentifier panePropIdentifier = new DXPropertyIdentifier(typeof(XYDiagram2D), "SeriesPane");
			seriesAccess.Properties[panePropIdentifier].SetValue(bindingItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			int paneIndex;
			Pane seriesPane = null;
			if (historyItem.ExecuteCommandInfo.IndexByNameDictionary.TryGetValue(PaneIndexKey, out paneIndex))
				seriesPane = ((XYDiagram2D)chartControl.Diagram).Panes[paneIndex];
			else
				seriesPane = ((XYDiagram2D)chartControl.Diagram).ActualDefaultPane;
			XYDiagram2D.SetSeriesPane((XYSeries)series, seriesPane);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			WpfChartPaneModel paneModel = (WpfChartPaneModel)parameter;
			int oldPaneIndex = SeriesModel.Pane.GetSelfIndex();
			SeriesModel.Pane = paneModel;
			int paneIndex = paneModel.GetSelfIndex();
			ElementIndexItem[] pathIndexes = new ElementIndexItem[2];
			if (paneIndex >= 0) {
				pathIndexes = new ElementIndexItem[3];
				pathIndexes[2] = new ElementIndexItem(PaneIndexKey, paneIndex);
			}
			pathIndexes[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
			pathIndexes[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			WpfChartSeriesModel seriesModel = SeriesModel;
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, pathIndexes), this, null, oldPaneIndex, paneIndex), seriesModel.Series);
		}
	}
}
