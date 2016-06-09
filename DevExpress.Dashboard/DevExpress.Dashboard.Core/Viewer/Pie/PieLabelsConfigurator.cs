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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public class PieLabelsConfigurator : ChartLabelsConfiguratorBase {
		PieDashboardItemViewModel PieViewModel { get { return (PieDashboardItemViewModel)ViewModel; } }
		public PieLabelsConfigurator(DashboardChartControlViewerBase viewer)
			: base(viewer) {
		}
		string GetToolTipPattern(PieValueType valueType) {
			switch(valueType) {
				case PieValueType.ValueAndPercent:
					return "{0} ({1})";
				case PieValueType.ArgumentAndPercent:
				case PieValueType.ArgumentAndValue:
					return "{0}: {1}";
				case PieValueType.ArgumentValueAndPercent:
					return "{0}: {1} ({2})";
				default:
					return String.Empty;
			}
		}
		protected override void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			if(e.SeriesPoint.Values[0] == 0)
				return;
			e.LabelText = GetFormattedLabel(e.Series, e.SeriesPoint, PieViewModel.LabelContentType);
		}
		public string GetFormattedLabel(Series series, SeriesPoint seriesPoint, PieValueType valueType) {
			CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			double sum;
			string percentDisplayText = String.Empty;
			string valueDisplayText = String.Empty;
			string argumentDisplayText = String.Empty;
			switch(valueType) {
				case PieValueType.Argument:
					return DataController.GetSeriesPointArgumentText(seriesPoint.Tag, ", ");
				case PieValueType.Percent:
					sum = series.Points.Sum(sp => sp.UserValues[0]);
					percentDisplayText = NumericFormatter.CreateInstance(culture, PieViewModel.PercentFormatViewModel).Format(seriesPoint.Values[0] / sum);
					return percentDisplayText;
				case PieValueType.Value:
					return DataController.GetSeriesPointDisplayTexts(series, seriesPoint)[0];
				case PieValueType.ValueAndPercent:
					valueDisplayText = DataController.GetSeriesPointDisplayTexts(series, seriesPoint)[0];
					sum = series.Points.Sum(sp => sp.UserValues[0]);
					percentDisplayText = NumericFormatter.CreateInstance(culture, PieViewModel.PercentFormatViewModel).Format(seriesPoint.Values[0] / sum);
					return String.Format(GetToolTipPattern(valueType), valueDisplayText, percentDisplayText);
				case PieValueType.ArgumentAndPercent:
					argumentDisplayText = DataController.GetSeriesPointArgumentText(seriesPoint.Tag, ", ");
					sum = series.Points.Sum(sp => sp.UserValues[0]);
					percentDisplayText = NumericFormatter.CreateInstance(culture, PieViewModel.PercentFormatViewModel).Format(seriesPoint.Values[0] / sum);
					return String.Format(GetToolTipPattern(valueType), argumentDisplayText, percentDisplayText);
				case PieValueType.ArgumentAndValue:
					argumentDisplayText = DataController.GetSeriesPointArgumentText(seriesPoint.Tag, ", ");
					valueDisplayText = DataController.GetSeriesPointDisplayTexts(series, seriesPoint)[0];
					return String.Format(GetToolTipPattern(valueType), argumentDisplayText, valueDisplayText);
				case PieValueType.ArgumentValueAndPercent:
					argumentDisplayText = DataController.GetSeriesPointArgumentText(seriesPoint.Tag, ", ");
					valueDisplayText = DataController.GetSeriesPointDisplayTexts(series, seriesPoint)[0];
					sum = series.Points.Sum(sp => sp.UserValues[0]);
					percentDisplayText = NumericFormatter.CreateInstance(culture, PieViewModel.PercentFormatViewModel).Format(seriesPoint.Values[0] / sum);
					return String.Format(GetToolTipPattern(valueType), argumentDisplayText, valueDisplayText, percentDisplayText);
				default:
					return String.Empty;
			}
		}
	}
}
