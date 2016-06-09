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
	public abstract class SeriesOptionsCommandBase : ChartCommandBase {
		protected const string AxisIndexKey = "Axis";
		protected const string PaneIndexKey = "Pane";
		protected const string SeriesDesigntimeIndexKey = "DesigntimeSeries";
		protected const string SeriesIndexKey = "Series";
		protected WpfChartSeriesModel SeriesModel {
			get { return ChartModel.SelectedModel as WpfChartSeriesModel; }
		}
		public SeriesOptionsCommandBase(WpfChartModel model)
			: base(model) { }
		protected override bool CanExecute(object parameter) {
			return (SeriesModel != null) && (!SeriesModel.IsAutoSeries);
		}
		[SecuritySafeCritical]
		protected abstract void DesignTimeApplyInternal(IModelItem seriesModelItem, object value);
		protected abstract void UndoInternal(WpfChartSeriesModel model, object oldValue);
		protected abstract void RedoInternal(WpfChartSeriesModel model, object newValue);
		protected abstract void RuntimeApplyInternal(Series series, object value);
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			RedoInternal(seriesModel, historyItem.NewValue);
			return seriesModel != null ? seriesModel.Series : null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			WpfChartSeriesModel seriesModel = seriesIndex >= 0 ? (WpfChartSeriesModel)ChartModel.DiagramModel.SeriesCollectionModel.ModelCollection[seriesIndex] : ChartModel.DiagramModel.SeriesTemplateModel;
			UndoInternal(seriesModel, historyItem.OldValue);
			return seriesModel != null ? seriesModel.Series : null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesDesigntimeIndexKey];
			IModelItem seriesModelItem = seriesIndex >= 0 ? chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex] : chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].Value;
			DesignTimeApplyInternal(seriesModelItem, historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series series = seriesIndex >= 0 ? chartControl.Diagram.Series[seriesIndex] : chartControl.Diagram.SeriesTemplate;
			RuntimeApplyInternal(series, historyItem.NewValue);
		}
	}
}
