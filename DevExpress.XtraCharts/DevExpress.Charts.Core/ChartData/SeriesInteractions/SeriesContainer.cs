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
namespace DevExpress.Charts.Native {
	public abstract class SeriesContainer {
		readonly ISeriesView view;
		readonly List<RefinedSeries> series = new List<RefinedSeries>();
		public ISeriesView SeriesView { get { return view; } }
		public List<RefinedSeries> Series { get { return series; } }
		public bool IsEmpty { get { return series.Count == 0; } }
		public SeriesContainer(ISeriesView view) {
			if (view == null)
				throw new ArgumentNullException();
			this.view = view;
		}
		void BatchPointsUpdate(BatchPointsUpdate batchUpdateResult) {
			switch (batchUpdateResult.Operation) {
				case ChartCollectionOperation.InsertItem:
					for (int i = 0; i < batchUpdateResult.AffectedPoints.Length; i++)
						InsertRefinedPoint(batchUpdateResult.RefinedSeries, batchUpdateResult.AffectedPoints[i]);
					break;
				case ChartCollectionOperation.RemoveItem:
					for (int i = 0; i < batchUpdateResult.AffectedPoints.Length; i++)
						RemoveRefinedPoint(batchUpdateResult.RefinedSeries, batchUpdateResult.AffectedPoints[i]);
					break;
				case ChartCollectionOperation.Reset:
					ResetRefinedSeries(batchUpdateResult.RefinedSeries);
					break;
				default:
					break;
			}
		}
		void SinglePointUpdate(SinglePointUpdate singleUpdateResult) {
			switch (singleUpdateResult.Operation) {
				case ChartCollectionOperation.Reset:
					ResetRefinedSeries(singleUpdateResult.RefinedSeries);
					break;
				case ChartCollectionOperation.InsertItem:
					InsertRefinedPoint(singleUpdateResult.RefinedSeries, singleUpdateResult.AffectedPoint);
					break;
				case ChartCollectionOperation.RemoveItem:
					RemoveRefinedPoint(singleUpdateResult.RefinedSeries, singleUpdateResult.AffectedPoint);
					break;
				case ChartCollectionOperation.UpdateItem:
					UpdateRefinedPoint(singleUpdateResult.RefinedSeries, singleUpdateResult.DeprectedPoint, singleUpdateResult.AffectedPoint);
					break;
				case ChartCollectionOperation.MoveItem:
				case ChartCollectionOperation.SwapItem:
				default:
					break;
			}
		}
		void UpdateRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			UpdateRefinedPoint(refinedSeries, point, point);
		}
		void UpdateRefinedPoint(RefinedSeries refinedSeries, RefinedPoint oldPoint, RefinedPoint newPoint) {
			RemoveRefinedPoint(refinedSeries, oldPoint);
			InsertRefinedPoint(refinedSeries, newPoint);
		}
		protected abstract void InsertRefinedPoints(int seriesIndex, RefinedSeries refinedSeries);
		protected abstract void RemoveRefinedPoints(int seriesIndex, RefinedSeries refinedSeries);
		protected abstract void InsertRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point);
		protected abstract void RemoveRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point);
		protected abstract void ResetRefinedSeries(RefinedSeries refinedSeries);
		public abstract void Recalculate();
		internal void UpdatePoints(SingleSeriesUpdate updateResult) {
			var singleUpdateResult = updateResult as SinglePointUpdate;
			if (singleUpdateResult != null)
				SinglePointUpdate(singleUpdateResult);
			else if (updateResult is BatchPointsUpdate)
				BatchPointsUpdate((BatchPointsUpdate)updateResult);
		}
		public bool Contains(RefinedSeries refinedSeries) {
			return Series.Contains(refinedSeries);
		}
	}
}
