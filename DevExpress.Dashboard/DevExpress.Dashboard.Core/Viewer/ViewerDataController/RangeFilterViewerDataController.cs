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

using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class RangeFilterViewerDataController : ChartViewerDataControllerBase {
		protected override ChartArgumentViewModel ArgumentViewModel { get { return ((RangeFilterDashboardItemViewModel)ViewModel).Argument; } }
		protected override int SeriesTemplateCount { get { return ((RangeFilterDashboardItemViewModel)ViewModel).SeriesTemplates.Count; } }
		public RangeFilterViewerDataController(RangeFilterDashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode)
			: base(viewModel, data, configuratorCache, drillDownState, isDesignMode) {
		}
		protected override ChartMultiDimensionalDataSourceBase GetDataSourceInternal(AxisPoint axisPoint, ChartSeriesTemplateViewModel seriesViewModel, int argumentThreshold, ChartSeriesPointColorHelper colorHelper) {
			return new ChartMultiDimensionalDataSource(Data.GetSlice(axisPoint), ArgumentViewModel.SummaryArgumentMember,
				GetArgumentType(ArgumentType), seriesViewModel.DataMembers, argumentThreshold, colorHelper);
		}
		public object GetRangeValue(object value) {
			if(ArgumentViewModel.HasArguments && ArgumentViewModel.IsOrderedDiscrete) {
				AxisPoint axisPoint = Data.GetAxisPointByUniqueValues(DashboardDataAxisNames.ChartArgumentAxis, new[] { value });
				if(axisPoint != null)
					return axisPoint.Index;
			}
			return value;
		}
		public object GetArgumentValue(object value) {
			string strValue = value as string;
			if(strValue != null) {
				int index;
				if(!int.TryParse(strValue, out index))
					return null;
				AxisPoint rootPoint = Data.GetAxisRoot(DashboardDataAxisNames.ChartArgumentAxis);
				AxisPoint point = rootPoint.FindPointByIndex(index);
				return point.UniqueValue;
			}
			return value;
		}
	}
}
