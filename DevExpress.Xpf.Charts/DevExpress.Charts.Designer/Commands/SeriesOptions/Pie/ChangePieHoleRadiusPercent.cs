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
	public class ChangePieHoleRadiusPercent : SeriesOptionsCommandBase {
		WpfChartSeriesModel SelectedSeriesModel {
			get { return ChartModel.SelectedModel as WpfChartSeriesModel; }
		}
		protected Series SelectedSeries {
			get { return SelectedSeriesModel == null ? null : SelectedSeriesModel.Series; }
		}
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public ChangePieHoleRadiusPercent(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return SelectedSeries is PieSeries && !(SelectedSeries is NestedDonutSeries2D);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			double newValue = Convert.ToDouble(parameter);
			double oldValue = SelectedSeriesModel.HoleRadiusPercent;
			SelectedSeriesModel.HoleRadiusPercent = newValue;
			int seriesIndex = PreviewChart.Diagram.Series.IndexOf(SelectedSeries);
			ElementIndexItem seriesIndexItem = new ElementIndexItem(SeriesIndexKey, seriesIndex);
			ExecuteCommandInfo execComInfo = new ExecuteCommandInfo(parameter, seriesIndexItem);
			HistoryItem historyItem = new HistoryItem(execComInfo, this, oldValue, newValue);
			CommandResult result = new CommandResult(historyItem);
			return result;
		}
		 [SecuritySafeCritical]
		protected override void DesignTimeApplyInternal(IModelItem seriesModelItem, object value) {
			seriesModelItem.Properties["HoleRadiusPercent"].SetValue(value);
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			((PieSeries)series).HoleRadiusPercent = (double)value;
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			model.HoleRadiusPercent = (double)oldValue;
		}
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			model.HoleRadiusPercent = (double)newValue;
		}
	}
}
