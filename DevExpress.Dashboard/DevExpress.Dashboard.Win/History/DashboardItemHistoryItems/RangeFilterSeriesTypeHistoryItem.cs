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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class RangeFilterSeriesTypeHistoryItem : IHistoryItem {
		readonly RangeFilterDashboardItem dashboardItem;
		readonly SimpleSeriesType seriesType;
		readonly Dictionary<SimpleSeries, SimpleSeriesType> previousSeriesTypes;
		readonly ChartSeriesConverter prevConverter, nextConverter;
		public string Caption { get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemRangeFilterSeriesType), dashboardItem.Name); } }
		public RangeFilterSeriesTypeHistoryItem(RangeFilterDashboardItem dashboardItem, SimpleSeriesType seriesType) {
			this.dashboardItem = dashboardItem;
			this.seriesType = seriesType;
			this.prevConverter = dashboardItem.DefaultSeriesConverter;
			nextConverter = new ChartSimpleSeriesConverter(seriesType);
			RangeFilterSeriesCollection seriesCollection = dashboardItem.Series;
			previousSeriesTypes = new Dictionary<SimpleSeries, SimpleSeriesType>(seriesCollection.Count);
			foreach (SimpleSeries series in seriesCollection)
				previousSeriesTypes.Add(series, series.SeriesType);
		}
		public void Undo(DashboardDesigner designer) {
			PerformLockedOperation(prevConverter, () => {
				foreach (KeyValuePair<SimpleSeries, SimpleSeriesType> pair in previousSeriesTypes)
					pair.Key.SeriesType = pair.Value;
			});
			designer.SelectedDashboardItem = dashboardItem;
		}
		public void Redo(DashboardDesigner designer) {
			PerformLockedOperation(nextConverter, () => {					
					foreach (SimpleSeries series in dashboardItem.Series) {
						series.SeriesType = seriesType;
					}				
			});
			designer.SelectedDashboardItem = dashboardItem;
		}
		void PerformLockedOperation(ChartSeriesConverter converter, Action action) {
			dashboardItem.Series.BeginUpdate();
			try {
				dashboardItem.DefaultSeriesConverter = converter;
				action();				
			}
			finally {
				dashboardItem.Series.EndUpdate();
			}
		}
	}
}
