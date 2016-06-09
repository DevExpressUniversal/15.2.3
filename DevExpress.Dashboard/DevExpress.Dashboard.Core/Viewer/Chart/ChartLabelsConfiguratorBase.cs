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
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class ChartLabelsConfiguratorBase {
		readonly DashboardChartControlViewerBase viewer;
		ChartDashboardItemBaseViewModel viewModel;
		FormatterBase formatter;
		protected DashboardChartControlViewerBase Viewer { get { return viewer; } }
		protected ChartDashboardItemBaseViewModel ViewModel { get { return viewModel; } }
		protected ChartViewerDataControllerBase DataController { get { return viewer.DataController; } }
		protected IDashboardChartControl ChartControl { get { return viewer.ChartControl; } }
		protected virtual ChartArgumentViewModel ArgumentViewModel { get { return viewModel.Argument; } }
		protected internal FormatterBase Formatter {
			get {
				if(formatter == null) {
					if(ArgumentType == ChartArgumentType.DateTime)
						formatter = FormatterBase.CreateFormatter(ArgumentViewModel.AxisXLabelFormat);
					else
						if(ArgumentType == ChartArgumentType.String)
							formatter = FormatterBase.CreateFormatter(new ValueFormatViewModel());
						else
							formatter = FormatterBase.CreateFormatter(new ValueFormatViewModel(GetNumericFormatViewModel()));
				}
				return formatter;
			}
		}
		protected ChartArgumentType ArgumentType { get { return DataController.ArgumentType; } }
		protected ChartLabelsConfiguratorBase(DashboardChartControlViewerBase viewer) {
			this.viewer = viewer;
			ChartControl.CustomDrawSeriesPoint += (sender, e) => OnCustomDrawSeriesPoint(e);
		}
		public virtual void Update(ChartDashboardItemBaseViewModel chartViewModel, ChartArgumentType argumentType) {
			this.viewModel = chartViewModel;
			formatter = null;
		}
		protected abstract void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e);
		protected void UpdateDiagram() {
			XYDiagram diagram = Viewer.ChartControl.Diagram as XYDiagram;
			if(diagram != null) {
				AxisX axisX = diagram.AxisX;
				if(ArgumentViewModel.IsContinuousDateTimeScale) {
					axisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Continuous;
				}
				else {
					axisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Manual;
					DateTimeMeasureUnit measureUnit;
					switch(ArgumentViewModel.DateTimePresentationUnit) {
						case DateTimePresentationUnit.Quarter:
							measureUnit = DateTimeMeasureUnit.Quarter;
							break;
						case DateTimePresentationUnit.Month:
							measureUnit = DateTimeMeasureUnit.Month;
							break;
						case DateTimePresentationUnit.Hour:
							measureUnit = DateTimeMeasureUnit.Hour;
							break;
						case DateTimePresentationUnit.Minute:
							measureUnit = DateTimeMeasureUnit.Minute;
							break;
						case DateTimePresentationUnit.Second:
							measureUnit = DateTimeMeasureUnit.Second;
							break;
						default:
							measureUnit = DateTimeMeasureUnit.Day;
							break;
					}
					axisX.DateTimeScaleOptions.MeasureUnit = measureUnit;
				}
				axisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.None;
				axisX.DateTimeScaleOptions.AutoGrid = true;
				bool isQualitativeScale = ArgumentType == ChartArgumentType.String && !ArgumentViewModel.IsOrderedDiscrete;
				AxisLabelResolveOverlappingOptions resolveOverlappingOptions = axisX.Label.ResolveOverlappingOptions;
				resolveOverlappingOptions.AllowRotate = isQualitativeScale;
				resolveOverlappingOptions.AllowStagger = isQualitativeScale;
			}
		}
		NumericFormatViewModel GetNumericFormatViewModel() {
			NumericFormatViewModel source = ArgumentViewModel.AxisXLabelFormat == null ? null : ArgumentViewModel.AxisXLabelFormat.NumericFormat;
			if(source == null) {
				return new NumericFormatViewModel(NumericFormatType.Number, 2, DataController.GetArgumentDataItemNumericUnit(), false, false, 0, String.Empty);
			} else {
				return new NumericFormatViewModel() {
					FormatType = source.FormatType,
					Precision = source.Precision,
					Unit = source.Unit == DataItemNumericUnit.Auto ? DataController.GetArgumentDataItemNumericUnit() : source.Unit,
					IncludeGroupSeparator = source.IncludeGroupSeparator,
					ForcePlusSign = source.ForcePlusSign,
					SignificantDigits = source.SignificantDigits,
					CurrencyCulture = source.CurrencyCulture
				};
			}
		}
		protected internal virtual string FormatArgument(object value) {
			return ArgumentType == ChartArgumentType.String ? DataController.GetArgumentDisplayText(value) : Formatter.Format(value);
		}
	}
}
