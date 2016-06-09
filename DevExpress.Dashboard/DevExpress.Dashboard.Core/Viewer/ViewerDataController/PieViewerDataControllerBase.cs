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
using System.IO;
using System.Reflection;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Native.Performance;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class PieViewerDataControllerBase : ChartViewerDataControllerBase {
		static ApproximationController approximationControllerInstance = null;
		static ApproximationController ApproximationControllerInstance {
			get {
				if(approximationControllerInstance == null) {
					approximationControllerInstance = new ApproximationController();
					Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PerformanceStatisticsPath + ".PiesPerformanceStatistics.csv");
					using(StreamReader reader = new StreamReader(stream))
						approximationControllerInstance.RestoreFromCsv(reader.ReadToEnd());
				}
				return approximationControllerInstance;
			}
		}
		protected override ChartArgumentViewModel ArgumentViewModel {
			get { return PieViewModel.Argument; }
		}
		protected override int SeriesTemplateCount { get { return PieViewModel.ValueDataMembers.Length; } }
		protected override ApproximationController ApproximationController { get { return ApproximationControllerInstance; } }
		protected PieDashboardItemViewModel PieViewModel { get { return (PieDashboardItemViewModel)ViewModel; } }
		protected PieViewerDataControllerBase(PieDashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode)
			: base(viewModel, data, configuratorCache, drillDownState, isDesignMode) {
		}
		protected override ChartMultiDimensionalDataSourceBase GetDataSourceInternal(AxisPoint axisPoint, ChartSeriesTemplateViewModel seriesViewModel, int argumentThreshold, ChartSeriesPointColorHelper colorHelper) {
			return new ChartMultiDimensionalDataSource(Data.GetSlice(axisPoint), PieViewModel.Argument.SummaryArgumentMember,
				typeof(string), PieViewModel.ValueDataMembers, argumentThreshold, colorHelper);
		}
		public override string[] GetSeriesPointDisplayTexts(Series series, SeriesPoint seriesPoint) {
			MultiDimensionalData data = Data.GetSlice((AxisPoint)series.Tag).GetSlice((AxisPoint)seriesPoint.Tag);
			MeasureDescriptor measureDescriptor = data.GetMeasureDescriptorByID(series.ValueDataMembers[0]);
			return new[] { data.GetValue(measureDescriptor).DisplayText };
		}
		public virtual string[] GetAllValueDataMembers() {
			return PieViewModel.ValueDataMembers;
		}
		protected abstract string GetColorMeasureIdInternal(Series series, object seriesPointTag);
		public virtual string[] GetValueDisplayNames(AxisPoint axisPoint) {
			return new[] { String.Join(" - ", GetRootPathDisplayTexts(axisPoint)) };
		}
		protected override string GetColorMeasureId(Series series, object seriesPointTag) {
			if(PieViewModel.ColorDataMembers.Length == 1)
				return PieViewModel.ColorDataMembers[0];
			return GetColorMeasureIdInternal(series, seriesPointTag);
		}
	}
}
