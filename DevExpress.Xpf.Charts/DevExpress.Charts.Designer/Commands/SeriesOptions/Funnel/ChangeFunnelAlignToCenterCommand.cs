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
using System;
using System.Security;
using DevExpress.Xpf.Charts;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public class ChangeFunnelAlignToCenterCommand : SeriesOptionsCommandBase {
		const string seriesIndexKey = "SeriesIndexKey";
		WpfChartSeriesModel SelectedSeriesModel {
			get { return ChartModel.SelectedModel as WpfChartSeriesModel; }
		}
		Series SelectedSeries {
			get { return SelectedSeriesModel == null ? null : SelectedSeriesModel.Series; }
		}
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_FunnelAlignToCenter); }
		}
		public override string ImageName {
			get { return GlyphUtils.GalleryItemImages + @"SeriesPointAndSeriesModels\FunnelAlignToCenter"; }
		}
		public ChangeFunnelAlignToCenterCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedSeries is FunnelSeries2D;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			var funnelSeries = (FunnelSeries2D)PreviewChart.Diagram.Series[seriesIndex];
			funnelSeries.AlignToCenter = (bool)historyItem.NewValue;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			var funnelSeries = (FunnelSeries2D)PreviewChart.Diagram.Series[seriesIndex];
			funnelSeries.AlignToCenter = (bool)historyItem.OldValue;
			return null;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			IModelItem seriesModelItem = chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection[seriesIndex];
			seriesModelItem.Properties["AlignToCenter"].SetValue(historyItem.NewValue);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[seriesIndexKey];
			var funnelSeries = (FunnelSeries2D)chartControl.Diagram.Series[seriesIndex];
			funnelSeries.AlignToCenter = (bool)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null)
				return null;
			bool newValue = Convert.ToBoolean(parameter);
			bool oldValue = SelectedSeriesModel.AlignToCenter;
			SelectedSeriesModel.AlignToCenter = newValue;
			int seriesIndex = PreviewChart.Diagram.Series.IndexOf(SelectedSeries);
			ElementIndexItem seriesIndexItem = new ElementIndexItem(seriesIndexKey, seriesIndex);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, seriesIndexItem);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
		protected override void DesignTimeApplyInternal(IModelItem seriesModelItem, object value) {
			throw new NotImplementedException();
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			throw new NotImplementedException();
		}
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			throw new NotImplementedException();
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			throw new NotImplementedException();
		}
	}
}
