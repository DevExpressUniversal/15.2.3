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
	public class ChartLabelsConfigurator : ChartLabelsConfiguratorBase {
		public ChartLabelsConfigurator(DashboardChartControlViewerBase viewer)
			: base(viewer) {
			ChartControl.CustomDrawAxisLabel += (sender, e) => OnCustomDrawAxisLabel(e.Item);
			ChartControl.CustomDrawCrosshair += (sender, e) => OnCustomDrawCrosshair(e.CrosshairElementGroups);
		}
		public override void Update(ChartDashboardItemBaseViewModel chartViewModel, ChartArgumentType argumentType) {
			base.Update(chartViewModel, argumentType);
			UpdateDiagram();
		}
		protected override void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			SeriesPoint seriesPoint = e.SeriesPoint;
			Series series = e.Series;
			ChartSeriesConfigurator configurator = Viewer.GetSeriesConfigurator(series);
			ChartSeriesTemplateViewModel viewModel = Viewer.GetSeriesViewModel(e.Series);
			if(viewModel != null && viewModel.PointLabel.ShowPointLabels) {
				StringBuilder firstLabel = new StringBuilder();
				StringBuilder secondLabel = new StringBuilder();
				object[] values = DataController.GetSeriesPointValues(series, seriesPoint);
				string[] displayTexts = DataController.GetSeriesPointDisplayTexts(series, seriesPoint);
				switch(viewModel.PointLabel.Content) {
					case PointLabelContentType.Argument:
						AppendArgumentString(seriesPoint, firstLabel, secondLabel);
						break;
					case PointLabelContentType.ArgumentAndValue:
						AppendArgumentString(seriesPoint, firstLabel, secondLabel);
						configurator.AppendSeparator(firstLabel, secondLabel);
						configurator.AppendValueString(values, displayTexts, firstLabel, secondLabel);
						break;
					case PointLabelContentType.SeriesName:
						configurator.AppendSeriesName(values, e.Series, firstLabel, secondLabel);
						break;
					default:
						configurator.AppendValueString(values, displayTexts, firstLabel, secondLabel);
						break;
				}
				e.LabelText = firstLabel.ToString();
				e.SecondLabelText = secondLabel.ToString();
			}
		}
		void OnCustomDrawCrosshair(IEnumerable<CrosshairElementGroup> elementGroups) {
			string argumentText = String.Empty;
			foreach (CrosshairElementGroup group in elementGroups) {
				foreach (CrosshairElement element in group.CrosshairElements) {
					AxisPoint argumentAxisPoint = (AxisPoint)element.SeriesPoint.Tag;
					SeriesPoint seriesPoint = element.SeriesPoint;
					Series series = element.Series;
					string[] displayTexts = DataController.GetSeriesPointDisplayTexts(series, seriesPoint);
					if (displayTexts.Any(text => !string.IsNullOrEmpty(text))) {
						string currentArgumentText = DataController.GetSeriesPointArgumentText(argumentAxisPoint, ", ");
						if (argumentText != currentArgumentText)
							element.LabelElement.HeaderText = argumentText = currentArgumentText;
						element.LabelElement.Text = GetCrosshairLabelElementText(series, argumentAxisPoint, displayTexts);
						Color color = Viewer.GetPointColor(element.Series, element.SeriesPoint);
						if (color != Color.Empty)
							element.LabelElement.MarkerColor = color;
					}
					else
						element.Visible = false;
				}
			}
		}
		protected virtual string GetCrosshairLabelElementText(Series series, AxisPoint argumentAxisPoint, string[] displayTexts) {
			return series.Name + Viewer.GetSeriesConfigurator(series).GetCrosshairValueString(displayTexts);
		}
		protected virtual void OnCustomDrawAxisLabel(AxisLabelItemBase labelItem) {
			labelItem.Text = labelItem.Axis is AxisYBase ? FormatValueAxisLabel(labelItem) : FormatArgument(labelItem.AxisValue);
		}
		protected string FormatValueAxisLabel(AxisLabelItemBase labelItem) {
			if(Viewer.IsPercentAxis(labelItem.Axis)) {
				NumericFormatter percentFormatter = new NumericAxisPercentFormatter();
				return percentFormatter.Format(labelItem.AxisValue);
			}
			Range axisRange = labelItem.Axis.VisualRange;
			NumericFormatter formatter = NumericFormatter.CreateInstance(Thread.CurrentThread.CurrentCulture, (double)axisRange.MinValue, (double)axisRange.MaxValue);
			return formatter.Format(labelItem.AxisValue);
		}
		void AppendArgumentString(SeriesPoint seriesPoint, StringBuilder firstLabel, StringBuilder secondLabel) {
			string argument = FormatArgument(GetSeriesPointArgument(seriesPoint));
			firstLabel.Append(argument);
			secondLabel.Append(argument);
		}
		object GetSeriesPointArgument(SeriesPoint point) {
			switch(ArgumentType) {
				case ChartArgumentType.DateTime:
					return point.DateTimeArgument;
				case ChartArgumentType.Integer:
				case ChartArgumentType.Float:
				case ChartArgumentType.Double:
				case ChartArgumentType.Decimal:
					return point.NumericalArgument;
				case ChartArgumentType.String:
				default:
					return point.Argument;
			}
		}
	}
}
