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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
using System.Runtime.Serialization;
namespace DevExpress.DashboardCommon.Viewer {
	public class ScatterChartViewerDataController : AxesChartViewerDataController {
		ScatterChartDashboardItemViewModel ScatterChartViewModel { get { return (ScatterChartDashboardItemViewModel)ChartViewModel; } }
		public ScatterChartViewerDataController(ScatterChartDashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode)
			: base(viewModel, data, configuratorCache, drillDownState, isDesignMode) {
		}
		protected override ChartMultiDimensionalDataSourceBase GetDataSourceInternal(AxisPoint axisPoint, ChartSeriesTemplateViewModel seriesViewModel, int argumentThreshold, ChartSeriesPointColorHelper colorHelper) {
			return new ScatterChartMultiDimensionalDataSource(Data.GetSlice(axisPoint), ChartViewModel.Argument.SummaryArgumentMember,
				ScatterChartViewModel.AxisXDataMember, seriesViewModel.DataMembers, argumentThreshold, colorHelper);
		}
		public override ChartMultiDimensionalDataSourceBase GetDataSource(AxisPoint axisPoint) {
			return new ScatterChartMultiDimensionalDataSource(Data, ChartViewModel.Argument.SummaryArgumentMember, ScatterChartViewModel.AxisXDataMember, ArgumentValuesLimit);
		}
		public override ChartMultiDimensionalDataSourceBase GetDataSource() {
			return new ScatterChartMultiDimensionalDataSource(Data, ChartViewModel.Argument.SummaryArgumentMember, ScatterChartViewModel.AxisXDataMember, new string[0], -1, null);
		}
		public string FormatArgument(object value) {
			return Data.GetMeasureDescriptorByID(ScatterChartViewModel.AxisXDataMember).Format(value);
		}
		public string GetCrosshairLabelElementText(AxisPoint argumentAxisPoint) {
			IList<string> results = new List<string>();
			foreach(KeyValuePair<MeasureDescriptor, MeasureValue> pair in GetMeasureValues(argumentAxisPoint)) {
				MeasureDescriptor descriptor = pair.Key;
				MeasureValue value = pair.Value;
				results.Add(string.Format("{0}: {1}", descriptor.Name, value.DisplayText));
			}
			return string.Join("\n", results);
		}
		public IList<string> GetValueDisplayTexts(AxisPoint argumentAxisPoint) {
			return GetMeasureValues(argumentAxisPoint).Values.Select(val => val.DisplayText).ToList();
		}
		public string GetWeightDisplayText(AxisPoint argumentAxisPoint) {			
			string[] dataMembers = ScatterChartViewModel.Panes[0].SeriesTemplates[0].DataMembers;
			if(dataMembers.Length > 1) {
				string measureId = dataMembers[1];
				MultiDimensionalData data = Data.GetSlice(argumentAxisPoint);
				MeasureDescriptor descriptor = data.GetMeasureDescriptorByID(measureId);
				return data.GetValue(descriptor).DisplayText;
			}
			return null;
		}
		Dictionary<MeasureDescriptor, MeasureValue> GetMeasureValues(AxisPoint argumentAxisPoint) {
			MultiDimensionalData data = Data.GetSlice(argumentAxisPoint);
			IList<string> dataMembers = new List<string>();
			dataMembers.Add(ScatterChartViewModel.AxisXDataMember);
			dataMembers.AddRange(ScatterChartViewModel.Panes[0].SeriesTemplates[0].DataMembers);
			Dictionary<MeasureDescriptor, MeasureValue> result = new Dictionary<MeasureDescriptor, MeasureValue>();
			foreach(string id in dataMembers) {
				MeasureDescriptor descriptor = data.GetMeasureDescriptorByID(id);
				result.Add(descriptor, data.GetValue(descriptor));
			}
			return result;
		}
	}
}
