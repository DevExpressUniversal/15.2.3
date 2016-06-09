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

namespace DevExpress.Charts.Native {
	public abstract class SeriesInteractionContainer : SeriesContainer, ISupportMinMaxValues {
		readonly SeriesComparerByActiveIndex seriesComparer = new SeriesComparerByActiveIndex();
		public abstract double Max { get; }
		public abstract double Min { get; }
		public SeriesInteractionContainer(ISeriesView view)
			: base(view) {
		}
		protected override void ResetRefinedSeries(RefinedSeries refinedSeries) {
			RemoveSeries(refinedSeries);
			AddSeries(refinedSeries);
		}
		public abstract double GetAbsMinValue();
		public virtual void AddSeries(RefinedSeries refinedSeries) {
			int index = GetSeriesIndex(refinedSeries);
			if (index >= 0)
				RemoveSeries(refinedSeries);
			else
				index = ~index;
			refinedSeries.InteractionContainer = this;
			Series.Insert(index, refinedSeries);
			InsertRefinedPoints(index, refinedSeries);
		}
		public virtual void RemoveSeries(RefinedSeries refinedSeries) {
			int index = GetSeriesIndex(refinedSeries);
			if (index >= 0) {
				refinedSeries.InteractionContainer = null;
				Series.RemoveAt(index);
				RemoveRefinedPoints(index, refinedSeries);
			}
		}
		public int GetSeriesIndex(RefinedSeries refinedSeries) {
			return Series.BinarySearch(refinedSeries, seriesComparer);
		}
		public void SwapSeries(RefinedSeries series1, RefinedSeries series2) {
			int index1 = GetSeriesIndex(series1);
			int index2 = GetSeriesIndex(series2);
			if (index1 >= 0 && index2 >= 0) {
				Series[index1] = series2;
				Series[index2] = series1;
				Recalculate();
			}
		}
	}
}
