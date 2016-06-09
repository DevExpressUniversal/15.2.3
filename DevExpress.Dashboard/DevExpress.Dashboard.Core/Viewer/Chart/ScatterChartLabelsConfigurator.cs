#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public class ScatterChartLabelsConfigurator : ChartLabelsConfigurator {
		ScatterChartViewerDataController ScatterDataController { get { return (ScatterChartViewerDataController)DataController; } }
		public ScatterChartLabelsConfigurator(DashboardChartControlViewerBase viewer)
			: base(viewer) {
		}
		protected internal override string FormatArgument(object value) {
			return ViewModel.Argument.SummaryArgumentMember != null ? ScatterDataController.FormatArgument(value) : Formatter.Format(value);
		}
		protected override string GetCrosshairLabelElementText(Series series, AxisPoint argumentAxisPoint, string[] displayTexts) {
			return ScatterDataController.GetCrosshairLabelElementText(argumentAxisPoint);
		}
		protected override void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			SeriesPoint seriesPoint = e.SeriesPoint;
			Series series = e.Series;
			AxisPoint argumentAxisPoint = (AxisPoint)seriesPoint.Tag;
			ChartSeriesTemplateViewModel viewModel = Viewer.GetSeriesViewModel(e.Series);
			if(viewModel != null && viewModel.PointLabel.ShowPointLabels) {
				switch(viewModel.PointLabel.ScatterContent) {
					case ScatterPointLabelContentType.Argument:
						e.LabelText = GetArgumentText(argumentAxisPoint);
						break;
					case ScatterPointLabelContentType.Weight:
						e.LabelText = GetWeightText(argumentAxisPoint);
						break;
					case ScatterPointLabelContentType.Values:
						e.LabelText = GetValuesText(argumentAxisPoint);
						break;
					case ScatterPointLabelContentType.ArgumentAndWeight:
						e.LabelText = string.Format("{0}: {1}", GetArgumentText(argumentAxisPoint), GetWeightText(argumentAxisPoint));
						break;
					case ScatterPointLabelContentType.ArgumentAndValues:
						e.LabelText = string.Format("{0}: {1}", GetArgumentText(argumentAxisPoint), GetValuesText(argumentAxisPoint));
						break;
					default:
						break;
				}
			}
		}
		protected override void OnCustomDrawAxisLabel(AxisLabelItemBase labelItem) {
			labelItem.Text = FormatValueAxisLabel(labelItem);
		}
		string GetArgumentText(AxisPoint argumentAxisPoint) {
			return DataController.GetSeriesPointArgumentText(argumentAxisPoint, ", ");
		}
		string GetWeightText(AxisPoint argumentAxisPoint) {
			return ScatterDataController.GetWeightDisplayText(argumentAxisPoint);
		}
		string GetValuesText(AxisPoint argumentAxisPoint) {
			return string.Join(" - ", ScatterDataController.GetValueDisplayTexts(argumentAxisPoint));
		}
	}
}
