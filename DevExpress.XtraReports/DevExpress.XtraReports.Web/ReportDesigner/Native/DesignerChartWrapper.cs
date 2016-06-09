#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	[ToolboxItem(false)]
	public class DesignerChartWrapper : XRChart, IChartContainer {
		public static DesignerChartWrapper CreateFromContract(XRChartLayout contract) {
			var chart = new DesignerChartWrapper();
			chart.SizeF = new SizeF(contract.Width, contract.Height);
			var xmlLayout = JsonConverter.JsonToXml(contract.Chart);
			using(var stream = new MemoryStream(xmlLayout)) {
				chart.LoadFromStream(stream);
			}
			return chart;
		}
		ISeriesView baseView;
		ISeries baseSeries;
		public int[] GetIndexesOfIncompatibleViews() {
			baseSeries = null;
			baseView = null;
			var indexes = new List<int>();
			var seriesRepository = new RefinedSeriesRepository(Chart.DataContainer);
			var dataContainer = (IChartDataContainer)Chart.DataContainer;
			var incompatibilityCalculator = new RefinedSeriesIncompatibilityCalculator(new SeriesIncompatibilityStatistics());
			if(dataContainer.ShouldUseSeriesTemplate) {
				baseView = dataContainer.SeriesTemplate.SeriesView;
				IXYSeriesView xySeriesView = dataContainer.SeriesTemplate.SeriesView as IXYSeriesView;
				if(xySeriesView != null)
					incompatibilityCalculator.AddTemplateView(xySeriesView, dataContainer.SeriesTemplate.ArgumentScaleType, dataContainer.SeriesTemplate.ValueScaleType);
			} else {
				baseSeries = Chart.Series.FirstOrDefault();
				if(baseSeries != null)
					baseView = baseSeries.SeriesView;
			}
			incompatibilityCalculator.Initialize(baseSeries, baseView);
			int i = 0;
			foreach(Series series in Series) {
				if(!series.IsAutoCreated) {
					var refSeries = new RefinedSeries(series, seriesRepository);
					if(!incompatibilityCalculator.IsCompatible(refSeries)) {
						indexes.Add(i);
					}
					i++;
				}
			}
			return indexes.ToArray();
		}
		bool IChartContainer.DesignMode {
			get { return true; }
		}
	}
}
