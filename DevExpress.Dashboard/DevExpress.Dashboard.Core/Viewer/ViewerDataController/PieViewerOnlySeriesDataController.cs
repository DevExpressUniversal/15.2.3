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
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public class PieViewerOnlySeriesDataController : PieViewerDataControllerBase {
		public PieViewerOnlySeriesDataController(PieDashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode) :
			base(viewModel, data, configuratorCache, drillDownState, isDesignMode) {
		}
		public override IList GetArgumentUniqueValues(SeriesPoint seriesPoint) {
			return null;
		}
		public override string[] GetAllValueDataMembers() {
			return new[] { PieValuePropertyDescriptor.ValueDataMember };
		}
		public override IList<string> GetValueDataMembers(Series series, SeriesPoint seriesPoint) {
			return new List<string> { PieViewModel.ValueDataMembers[(int)seriesPoint.Tag] };
		}
		protected override ChartMultiDimensionalDataSourceBase GetDataSourceInternal(AxisPoint axisPoint, ChartSeriesTemplateViewModel seriesViewModel, int argumentThreshold, ChartSeriesPointColorHelper colorHelper) {
			return new PieMultiDimensionalOnlySeriesDataSource(Data.GetSlice(axisPoint), PieViewModel.ValueDataMembers, argumentThreshold, colorHelper);
		}
		public override string GetSeriesPointArgumentText(object component, string separator) {
			return Data.GetMeasureDescriptorByID(PieViewModel.ValueDataMembers[(int)component]).Name;
		}
		public override string[] GetSeriesPointDisplayTexts(Series series, SeriesPoint seriesPoint) {
			MeasureDescriptor measureDescriptor = Data.GetMeasureDescriptorByID(PieViewModel.ValueDataMembers[(int)seriesPoint.Tag]);
			return new[] { Data.GetSlice((AxisPoint)series.Tag).GetValue(measureDescriptor).DisplayText };
		}
		protected override string GetColorMeasureIdInternal(Series series, object seriesPointTag) {
			return PieViewModel.ColorDataMembers[(int)seriesPointTag];
		}
		public override AxisPoint GetArgumentAxisPoint(SeriesPoint seriesPoint) {
			return Data.Axes[DashboardDataAxisNames.ChartArgumentAxis].RootPoint;
		}
	}
}
