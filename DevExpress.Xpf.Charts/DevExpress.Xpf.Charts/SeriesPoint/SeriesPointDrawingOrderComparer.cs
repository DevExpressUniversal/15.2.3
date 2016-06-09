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
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class SeriesPointDrawingOrderComparer : IComparer<SeriesPointItem> {
		static double ActualSeriesPointValue(SeriesPointItem pointItem) {
			BarSeries2D series = pointItem.Series as BarSeries2D;
			if (series != null) {
				IXYSeriesView xyView = series as IXYSeriesView;
				if (xyView != null) {
					AxisY2D axisY = xyView.AxisYData as AxisY2D;
					if (axisY != null && axisY.ReverseInternal)
						return -series.GetPointValue(pointItem.RefinedPoint);
				}
				return series.GetPointValue(pointItem.RefinedPoint);
			}
			return ((IValuePoint)pointItem.RefinedPoint).Value;
		}
		readonly SeriesStackedGroupComparer stackedGroupComparer;
		readonly IList<XYSeries2D> seriesCollection;
		public SeriesPointDrawingOrderComparer(IList<XYSeries2D> series) {
			seriesCollection = series;
			stackedGroupComparer = SeriesStackedGroupComparer.Create(seriesCollection);
		}
		int CompareByValue(SeriesPointItem item1, SeriesPointItem item2) {
			double value1 = ActualSeriesPointValue(item1);
			double value2 = ActualSeriesPointValue(item2);
			return Math.Sign(value1 - value2);
		}
		int CompareBySeriesIndex(SeriesPointItem item1, SeriesPointItem item2) {
			XYSeries2D series1 = item1.Series as XYSeries2D;
			XYSeries2D series2 = item2.Series as XYSeries2D;
			if (series1 != null && series2 != null) {
				double item1SeriesIndex = seriesCollection.IndexOf(series1);
				double item2SeriesIndex = seriesCollection.IndexOf(series2);
				return Math.Sign(item1SeriesIndex - item2SeriesIndex);
			}
			return 0;
		}
		public int Compare(SeriesPointItem item1, SeriesPointItem item2) {
			if (item1 == null || item2 == null || item1.RefinedPoint == null ||
				item2.RefinedPoint == null || item1.Series == null || item2.Series == null)
				return 0;
			if (item1.Series.GetType() == item2.Series.GetType()) {
				if (item1.RefinedPoint.Argument == item2.RefinedPoint.Argument) {
					if (item1.Series is ISupportStackedGroup && item2.Series is ISupportStackedGroup) {
						int orderComparingResult = stackedGroupComparer.Compare(((ISupportStackedGroup)item1.Series).StackedGroup, ((ISupportStackedGroup)item2.Series).StackedGroup);
						return orderComparingResult == 0 ? CompareByValue(item1, item2) : orderComparingResult;
					}
					if (item1.Series is BarStackedSeries2D && item2.Series is BarStackedSeries2D)
						return CompareByValue(item1, item2);
					if (item1.Series is BarSideBySideSeries2D && item2.Series is BarSideBySideSeries2D)
						return Object.ReferenceEquals(item1.Series, item2.Series) ? CompareByValue(item1, item2) : CompareBySeriesIndex(item1, item2);
					return 0;
				}
				else
					return Math.Sign(item1.RefinedPoint.Argument - item2.RefinedPoint.Argument);
			}
			else
				return (item1.Series is BarStackedSeries2D && item2.Series is BarSideBySideSeries2D) ? -1 : 0;
		}
	}
	public class SeriesStackedGroupComparer {
		public static SeriesStackedGroupComparer Create(IList<XYSeries2D> seriesCollection) {
			List<object> stackedGroupDrawingOrder = new List<object>();
			foreach (Series series in seriesCollection) {
				if (series is ISupportStackedGroup) {
					object stackedGroup = ((ISupportStackedGroup)series).StackedGroup;
					if (!stackedGroupDrawingOrder.Contains(stackedGroup))
						stackedGroupDrawingOrder.Add(stackedGroup);
				}
			}
			return new SeriesStackedGroupComparer(stackedGroupDrawingOrder);
		}
		readonly List<object> stackedGroupDrawingOrder = new List<object>();
		SeriesStackedGroupComparer(List<object> stackedGroupDrawingOrder) {
			this.stackedGroupDrawingOrder = stackedGroupDrawingOrder;
		}
		public int Compare(object group1, object group2) {
			if (group1 == null || group2 == null)
				return 0;
			return stackedGroupDrawingOrder.IndexOf(group1) - stackedGroupDrawingOrder.IndexOf(group2);
		}
	}
}
