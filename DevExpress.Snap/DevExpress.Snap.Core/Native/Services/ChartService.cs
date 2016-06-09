#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.Data.Browsing;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Services.Internal;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Native.Services {
	public class ChartService : IChartService {
		public ISNChartContainer GetChartContainer(ServiceManager serviceManager, DataContext dataContext) {
			ChartContainer chartContainer = new ChartContainer(serviceManager, dataContext);
			SNChart chart = new SNChart(chartContainer);
			chartContainer.Assign(chart);
			return chartContainer;
		}
		public void LoadDefaultChart(SNChart chart) {
			Series series = new Series();
			((ISupportInitialize)series).BeginInit();
			AddPoint(series, 0, 3);
			AddPoint(series, 1, 4);
			AddPoint(series, 2, 2);
			AddPoint(series, 3, 5);
			AddPoint(series, 4, 1);
			series.Tag = SNChart.DefaultSeriesTag;
			chart.Series.Add(series);
			((ISupportInitialize)series).EndInit();
		}
		void AddPoint(Series series, int x, int y) {
			series.Points.Add(new SeriesPoint(x, y));
		}
	}
}
