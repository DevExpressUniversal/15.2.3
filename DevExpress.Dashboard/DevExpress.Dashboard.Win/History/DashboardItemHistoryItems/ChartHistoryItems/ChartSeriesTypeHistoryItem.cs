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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess;
namespace DevExpress.DashboardWin.Native {
	public class ChartSeriesTypeHistoryItem : DashboardItemHistoryItem<ChartDashboardItem> {
		readonly Dictionary<ChartPane, NotifyingCollection<ChartSeries>> previousState = new Dictionary<ChartPane, NotifyingCollection<ChartSeries>>();
		readonly ChartSeriesConverter prevConverter, nextConverter;
		readonly Dictionary<ChartPane, NotifyingCollection<ChartSeries>> nextState = new Dictionary<ChartPane, NotifyingCollection<ChartSeries>>();
		DimensionsHistoryContext dimensionsContext;
		protected override DashboardWinStringId CaptionId {
			get { return DashboardWinStringId.HistoryItemChartSeriesType; }
		}
		public ChartSeriesTypeHistoryItem(ChartDashboardItem chartItem, ChartSeriesConverter converter)
			: base(chartItem) {
			prevConverter = chartItem.DefaultSeriesConverter;
			nextConverter = converter;
			foreach (ChartPane pane in chartItem.Panes) {
				var prevSeries = new NotifyingCollection<ChartSeries>();
				var nextSeries = new NotifyingCollection<ChartSeries>();
				previousState.Add(pane, prevSeries);
				nextState.Add(pane, nextSeries);
				foreach (ChartSeries initialSeries in pane.Series) {
					prevSeries.Add(initialSeries);
					nextSeries.AddRange(converter.PrepareSeries(initialSeries));
				}
			}
		}
		protected override void PerformRedo() {			
			DashboardItem.Dashboard.BeginUpdate();
			try {
				ApplyState(nextState, nextConverter);
				dimensionsContext = new DimensionsHistoryContext(DashboardItem);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		protected override void PerformUndo() {			
			DashboardItem.Dashboard.BeginUpdate();
			try {
				ApplyState(previousState, prevConverter);
				if (dimensionsContext != null)
					dimensionsContext.Undo();
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
		void ApplyState(Dictionary<ChartPane, NotifyingCollection<ChartSeries>> state, ChartSeriesConverter converter) {
			ChartDashboardItem chartItem = DashboardItem;
			chartItem.DefaultSeriesConverter = converter;
			foreach (ChartPane pane in chartItem.Panes) {
				pane.Series.BeginUpdate();
				pane.Series.Clear();
				pane.Series.AddRange(state[pane]);
				pane.Series.EndUpdate();
			}
		}
	}
}
