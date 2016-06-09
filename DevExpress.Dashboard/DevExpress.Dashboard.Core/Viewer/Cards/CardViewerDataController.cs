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
using System.Collections.Generic;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class CardViewerDataController : KpiViewerDataController {
		const int cardsCountPerformanceThreshold = 6400;
		IEnumerable<AxisPoint> sparklineAxisPoints;
		protected override int SeriesCountPerformanceThreshold { get { return cardsCountPerformanceThreshold; } }
		public CardViewerDataController(MultiDimensionalData data, CardDashboardItemViewModel viewModel, bool drilledDown, bool isDesignMode)
			: base(data, viewModel, drilledDown, isDesignMode) {
			if(!String.IsNullOrEmpty(viewModel.SparklineAxisName)) {
				AxisPoint sparklineAxisRoot = data.GetAxisRoot(viewModel.SparklineAxisName);
				sparklineAxisPoints = sparklineAxisRoot.GetAxisPoints();
			}
		}
		protected override KpiViewerElementData CreateElementData(KpiElementViewModel elementViewModel, AxisPoint axisPoint) {
			KpiViewerElementData elementData = base.CreateElementData(elementViewModel, axisPoint);
			SparklineTooltipValues sparklineTooltipValues = null;
			List<double> sparklineValues = null;
			IList<Tuple<double, string>> sparklineData = GetSparklineData(elementViewModel, axisPoint);
			if(sparklineData.Count > 0) {
				sparklineValues = sparklineData.Select(tuple => tuple.Item1).ToList();
				sparklineTooltipValues = new SparklineTooltipValues {
					Start = sparklineData.First().Item2,
					End = sparklineData.Last().Item2,
					Min = sparklineData.OrderBy(tuple => tuple.Item1).First().Item2,
					Max = sparklineData.OrderBy(tuple => tuple.Item1).Last().Item2
				};
			}
			elementData.SparklineTooltipValues = sparklineTooltipValues;
			elementData.SparklineValues = sparklineValues;
			return elementData;
		}
		IList<Tuple<double, string>> GetSparklineData(KpiElementViewModel elementViewModel, AxisPoint axisPoint) {
			IList<Tuple<double, string>> tuples = new List<Tuple<double, string>>();
			MeasureDescriptor measureDescriptor = Data.GetMeasureDescriptorByID(elementViewModel.ID);
			if(sparklineAxisPoints != null) {
				IEnumerable<MeasureValue> measureValues = sparklineAxisPoints.Select(point => Data.GetValue(axisPoint, point, measureDescriptor));
				foreach(MeasureValue measureValue in measureValues) {
					double value = Convert.ToDouble(measureValue.Value);
					string displayText = GetSparklineTooltipText(measureValue, measureDescriptor);
					tuples.Add(new Tuple<double, string>(value, displayText));
				}
			}
			return tuples;
		}
		string GetSparklineTooltipText(MeasureValue measureValue, MeasureDescriptor measureDescriptor) {
			return measureValue.Value != null ? measureValue.DisplayText : measureDescriptor.Format(0);
		}
	}
}
