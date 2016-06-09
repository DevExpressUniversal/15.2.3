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

using System;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ChartViewCollection : ChartUndoableCollection<IChartView> {
		public ChartViewCollection(IChart parent)
			: base(parent) {
		}
		protected internal void ClearLastViews(int beginIndex) {
			int count = Count - beginIndex;
			for (int i = 0; i < count; i++)
				RemoveAt(Count - 1);
		}
		protected internal void ClearAllSeries() {
			ForEach(ClearViewSeries);
		}
		void ClearViewSeries(IChartView view) {
			view.Series.Clear();
		}
		protected internal IChartView TryGetView(ChartViewType type, AxisGroup axes) {
			for (int i = 0; i < Count; i++) {
				IChartView currentView = this[i];
				if (currentView.ViewType == type && Object.ReferenceEquals(currentView.Axes, axes))
					return currentView;
			}
			return null;
		}
		protected internal int TryGetViewIndex(ChartViewType type, AxisGroup axes) {
			for (int i = 0; i < Count; i++) {
				IChartView currentView = this[i];
				if (currentView.ViewType == type && Object.ReferenceEquals(currentView.Axes, axes))
					return i;
			}
			return -1;
		}
		protected internal int GetMaxSeriesIndex() {
			int result = -1;
			int viewCount = Count;
			for (int i = 0; i < viewCount; i++) {
				SeriesCollection series = this[i].Series;
				int seriesCount = series.Count;
				for (int j = 0; j < seriesCount; j++)
					result = Math.Max(result, series[j].Index);
			}
			return result;
		}
		protected internal int GetSeriesCount() {
			int result = 0;
			int viewCount = Count;
			for (int i = 0; i < viewCount; i++)
				result += this[i].Series.Count;
			return result;
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			HashSet<IDataReference> processedReferences = new HashSet<IDataReference>();
			foreach (IChartView view in this) {
				view.OnRangeInserting(context);
				foreach (ISeries series in view.Series) {
					foreach (IDataReference dataReference in series.GetDataReferences()) {
						if (!processedReferences.Contains(dataReference)) {
							dataReference.OnRangeInserting(context);
							processedReferences.Add(dataReference);
						}
					}
				}
			}
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			HashSet<IDataReference> processedReferences = new HashSet<IDataReference>();
			foreach (IChartView view in this) {
				view.OnRangeRemoving(context);
				foreach (ISeries series in view.Series) {
					foreach (IDataReference dataReference in series.GetDataReferences()) {
						if (!processedReferences.Contains(dataReference)) {
							dataReference.OnRangeRemoving(context);
							processedReferences.Add(dataReference);
						}
					}
				}
			}
		}
		#endregion
	}
}
