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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class GaugeViewerDataController : KpiViewerDataController {
		const int gaugesCountPerformanceThreshold = 2400;
		Dictionary<string, GaugeRangeModel> ranges;
		GaugeDashboardItemViewModel GaugeDashboardItemViewModel { get { return (GaugeDashboardItemViewModel)ViewModel; } }
		protected override int SeriesCountPerformanceThreshold { get { return gaugesCountPerformanceThreshold; } }
		public GaugeViewerDataController(MultiDimensionalData data, GaugeDashboardItemViewModel viewModel, bool drilledDown, bool isDesignMode)
			: base(data, viewModel, drilledDown, isDesignMode) {
			ranges = new Dictionary<string, GaugeRangeModel>();
		}
		public GaugeRangeModel CalculateGaugeRange(GaugeViewModel gaugeViewModel) {
			string id = gaugeViewModel.ID;
			GaugeRangeModel rangeModel = null;
			if(!ranges.TryGetValue(id, out rangeModel)) {
				IEnumerable<double> values = GetGaugeValues(id, gaugeViewModel.DataItemType);
				GaugeRangeCalculator calculator = new GaugeRangeCalculator(values, GaugeDashboardItemViewModel.ViewType, gaugeViewModel.MinValue, gaugeViewModel.MaxValue);
				rangeModel = calculator.GetGaugeRangeModel();
				ranges.Add(id, rangeModel);
			}
			return rangeModel;
		}
		IList<double> GetGaugeValues(string gaugeId, KpiElementDataItemType dataItemType) {
			IList<double> values = new List<double>();
			if(!String.IsNullOrEmpty(ViewModel.SeriesAxisName)) {
				foreach(AxisPoint point in AxisPoints) {
					foreach(double value in GetValue(gaugeId, dataItemType, point))
						values.Add(value);
				}
			}
			else {
				foreach(double value in GetValue(gaugeId, dataItemType, null))
					values.Add(value);
			}
			return values;
		}
		IEnumerable<double> GetValue(string gaugeId, KpiElementDataItemType dataItemType, AxisPoint axisPoint) {
			IList<double> values = new List<double>();
			if(dataItemType == KpiElementDataItemType.Measure) {
				MeasureValue measureValue = axisPoint != null ? Data.GetValue(axisPoint, gaugeId) : Data.GetValue(gaugeId);
				values.Add(Helper.ConvertToDouble(measureValue.Value));
			}
			else {
				DeltaValue deltaValue = axisPoint != null ? Data.GetDeltaValue(axisPoint, gaugeId) : Data.GetDeltaValue(gaugeId);
				values.Add(Helper.ConvertToDouble(deltaValue.ActualValue.Value));
				values.Add(Helper.ConvertToDouble(deltaValue.TargetValue.Value));
			}
			return values;
		}
		public double GetGaugeMin(string gaugeId) {
			GaugeRangeModel rangeModel = null;
			return ranges.TryGetValue(gaugeId, out rangeModel) ? rangeModel.MinRangeValue : 0d;
		}
		public double GetGaugeMax(string gaugeId) {
			GaugeRangeModel rangeModel = null;
			return ranges.TryGetValue(gaugeId, out rangeModel) ? rangeModel.MaxRangeValue : 0d;
		}
	}
}
