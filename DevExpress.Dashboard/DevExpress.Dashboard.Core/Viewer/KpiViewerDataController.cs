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
using System.Linq;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class KpiViewerElementData {
		public string Title { get; private set; }
		public SelectionData SelectionData { get; private set; }
		public DeltaValues DeltaValues { get; private set; }
		public List<double> SparklineValues { get; set; }
		public SparklineTooltipValues SparklineTooltipValues { get; set; }
		public KpiViewerElementData(string title, SelectionData selectionData, DeltaValues deltaValues) {
			Title = title;
			SelectionData = selectionData;
			DeltaValues = deltaValues;
		}
		public KpiViewerElementData(string title, SelectionData selectionData, DeltaValues deltaValues, List<double> sparklineValues, SparklineTooltipValues sparklineTooltipValues)
			: this(title, selectionData, deltaValues) {
			SparklineValues = sparklineValues;
			SparklineTooltipValues = sparklineTooltipValues;
		}
	}
	public abstract class KpiViewerDataController {
		readonly MultiDimensionalData data;
		readonly KpiDashboardItemViewModel viewModel;
		readonly bool drilledDown;
		IEnumerable<AxisPoint> axisPoints;
		bool isDataReduced = false;
		protected abstract int SeriesCountPerformanceThreshold { get; }
		protected IEnumerable<AxisPoint> AxisPoints { get { return isDataReduced ? axisPoints.Take(SeriesCountPerformanceThreshold) : axisPoints; } }
		protected MultiDimensionalData Data { get { return data; } }
		protected KpiDashboardItemViewModel ViewModel { get { return viewModel; } }
		public bool IsDataReduced { get { return isDataReduced; } }
		protected KpiViewerDataController(MultiDimensionalData data, KpiDashboardItemViewModel viewModel, bool drilledDown, bool isDesignMode) {
			this.data = data;
			this.viewModel = viewModel;
			this.drilledDown = drilledDown;
			if(!String.IsNullOrEmpty(viewModel.SeriesAxisName)) {
				AxisPoint axisRoot = data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis);
				axisPoints = axisRoot.GetAxisPoints();
				InitializeDataReduction(isDesignMode);
			}
		}
		public IEnumerable<KpiViewerElementData> GetElementData(KpiElementViewModel elementViewModel) {
			IList<KpiViewerElementData> elementsData = new List<KpiViewerElementData>();
			if(!String.IsNullOrEmpty(viewModel.SeriesAxisName)) 
				foreach(AxisPoint axisPoint in AxisPoints) 
					elementsData.Add(CreateElementData(elementViewModel, axisPoint));
			else 
				elementsData.Add(CreateElementData(elementViewModel, null));
			return elementsData;
		}
		protected virtual KpiViewerElementData CreateElementData(KpiElementViewModel elementViewModel, AxisPoint axisPoint) {
			SelectionData selectionData = null;
			DeltaValues deltaValues = GetDeltaValues(elementViewModel, data, axisPoint);
			if(axisPoint != null) {
				selectionData = new SelectionData();
				RootPath(axisPoint).ForEach(point => {
					selectionData.Values.Add(point.UniqueValue);
					selectionData.Captions.Add(point.DisplayText);
				});
			}
			return new KpiViewerElementData(elementViewModel.Title, selectionData, deltaValues);
		}
		void InitializeDataReduction(bool isDesignMode) {
			if(isDesignMode) 
				if(axisPoints.Count() > SeriesCountPerformanceThreshold) 
					isDataReduced = true;
		}
		IEnumerable<AxisPoint> RootPath(AxisPoint axisPoint) {
			return drilledDown ? new[] { axisPoint } : axisPoint.RootPath;
		}
		DeltaValues GetDeltaValues(KpiElementViewModel elementViewModel, MultiDimensionalData data, AxisPoint axisPoint) {
			DeltaValues deltaValues;
			KpiElementDataItemType type = elementViewModel.DataItemType;
			string measureId = elementViewModel.ID;
			if(type == KpiElementDataItemType.Measure) {
				MeasureDescriptor measureDescriptor = data.GetMeasureDescriptorByID(measureId);
				MeasureValue measureValue = axisPoint != null ? data.GetValue(axisPoint, measureDescriptor) : data.GetValue(measureDescriptor);
				decimal value = Helper.ConvertToDecimal(measureValue.Value);
				deltaValues = new DeltaValues(value, measureValue.DisplayText);
			}
			else {
				deltaValues = new DeltaValues();
				DeltaDescriptor deltaDescriptor = data.GetDeltaDescriptorById(measureId);
				DeltaValue deltaValue = axisPoint != null ? data.GetDeltaValue(axisPoint, deltaDescriptor) : data.GetDeltaValue(deltaDescriptor);
				deltaValues.ActualValue = Helper.ConvertToDouble(deltaValue.ActualValue.Value);
				deltaValues.TargetValue = Helper.ConvertToDouble(deltaValue.TargetValue.Value);
				deltaValues.Value = Helper.ConvertToDecimal(deltaValue.DisplayValue.Value);
				deltaValues.ValueText = deltaValue.DisplayValue.DisplayText;
				deltaValues.SubValue1 = Helper.ConvertToDecimal(deltaValue.DisplaySubValue1.Value);
				deltaValues.SubValue1Text = deltaValue.DisplaySubValue1.DisplayText;
				deltaValues.SubValue2 = Helper.ConvertToDecimal(deltaValue.DisplaySubValue2.Value);
				deltaValues.SubValue2Text = deltaValue.DisplaySubValue2.DisplayText;
				deltaValues.IndicatorType = deltaValue.IndicatorType;
				deltaValues.IsGood = deltaValue.IsGood;
			}
			return deltaValues;
		}
	}
}
