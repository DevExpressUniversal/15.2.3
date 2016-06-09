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

using DevExpress.Charts.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Charts.ChartData {
	public interface IMinMaxValuesCalculator {
		MinMaxValues GetMinMaxValues(ICollection<RefinedSeries> seriesCollection, IMinMaxValues rangeForFiltering, List<IAffectsAxisRange> indicators);
		double GetMinAbsValue(ICollection<RefinedSeries> seriesCollection);
	}
	public class InteractionManager : IMinMaxValuesCalculator {
		readonly Dictionary<object, SeriesContainer> containers = new Dictionary<object, SeriesContainer>();
		readonly Dictionary<object, SeriesInteractionContainer> seriesGroupsContainers = new Dictionary<object, SeriesInteractionContainer>();
		public bool IsEmpty { get { return containers.Count < 1; } }
		public ICollection<object> InteractionContainersKeys { get { return containers.Keys; } }
		public ICollection<SeriesContainer> InteractionContainers { get { return containers.Values; } }
		public ICollection<object> SeriesGroupsInteractionContainersKeys { get { return seriesGroupsContainers.Keys; } }
		public ICollection<SeriesInteractionContainer> SeriesGroupsInteractionContainers { get { return seriesGroupsContainers.Values; } }
		#region IMinMaxValuesCalculator implementation
		MinMaxValues IMinMaxValuesCalculator.GetMinMaxValues(ICollection<RefinedSeries> seriesCollection, IMinMaxValues rangeForFiltering, List<IAffectsAxisRange> indicators) {
			MinMaxValues minMaxValues;
			if (MinMaxValues.IsEmptyValue(rangeForFiltering))
				minMaxValues = GetMinMaxValuesForWholePoints(seriesCollection);
			else
				minMaxValues = GetMinMaxValuesWithFiltering(seriesCollection, rangeForFiltering);
			foreach (IAffectsAxisRange indicator in indicators) 
				minMaxValues = minMaxValues.Union(indicator.GetMinMaxValues(rangeForFiltering));
			return minMaxValues;
		}
		double IMinMaxValuesCalculator.GetMinAbsValue(ICollection<RefinedSeries> seriesCollection) {
			List<object> interactionKeys = FindInteractionKeys(seriesCollection);
			double min = double.MaxValue;
			foreach (var interactionKey in interactionKeys) {
				ISupportMinMaxValues minMaxContainer = containers[interactionKey] as ISupportMinMaxValues;
				if (minMaxContainer != null) {
					double value = minMaxContainer.GetAbsMinValue();
					if (value != 0)
						min = Math.Min(min, minMaxContainer.GetAbsMinValue());
				}
			}
			return min;
		}
		#endregion
		bool IsFullStackedSeries(ISeriesView view) {
			Type pointType = view.PointInterfaceType;
			return pointType.Equals(typeof(IFullStackedPoint));
		}
		bool IsStackedSeries(ISeriesView view) {
			Type pointType = view.PointInterfaceType;
			return pointType.Equals(typeof(IStackedPoint));
		}
		MinMaxValues GetMinMaxValuesForWholePoints(ICollection<RefinedSeries> seriesCollection) {
			List<object> interactionKeys = FindInteractionKeys(seriesCollection);
			MinMaxValues range = MinMaxValues.Empty;
			foreach (var interactionKey in interactionKeys) {
				SeriesContainer container = containers[interactionKey];
				ISplineSeriesView splineView = container.SeriesView as ISplineSeriesView;
				if (splineView != null && splineView.ShouldCorrectRanges) {
					GeometryCalculator geometryCalculator = new GeometryCalculator();
					foreach (RefinedSeries series in container.Series) {
						IList<IGeometryStrip> strips = geometryCalculator.CreateStrips(series);
						foreach (IBezierStrip strip in strips) {
							MinMaxValues splineMinMax = strip.CalculateMinMaxValues();
							range = range.Union(splineMinMax);
						}
					}
				}
				ISupportMinMaxValues minMaxContainer = container as ISupportMinMaxValues;
				if (minMaxContainer != null) {
					range.Union(minMaxContainer.Min);
					range.Union(minMaxContainer.Max);
				}
			}
			return range;
		}
		MinMaxValues GetMinMaxValuesWithFiltering(ICollection<RefinedSeries> seriesCollection, IMinMaxValues rangeForFiltering) {
			List<object> interactionKeys = FindInteractionKeys(seriesCollection);
			MinMaxValues range = MinMaxValues.Empty;
			foreach (var interactionKey in interactionKeys) {
				SeriesContainer container = containers[interactionKey];
				ISupportMinMaxValues minMaxContainer = container as ISupportMinMaxValues;
				MinMaxValues totalRange = MinMaxValues.Empty;
				if (minMaxContainer != null) {
					totalRange.Union(minMaxContainer.Min);
					totalRange.Union(minMaxContainer.Max);
				}
				else
					return totalRange;
				foreach (RefinedSeries series in container.Series) {
					int minIndexX, maxIndexX;
					if (IsFullStackedSeries(series.SeriesView))
						continue;
					FindIndex(series.FinalPointsSortedByArgument, rangeForFiltering, out minIndexX, out maxIndexX);
					if (minIndexX == 0 && maxIndexX == series.Points.Count - 1)
						range = range.Union(totalRange);
					else
						range = range.Union(GetMinMaxFromSeries(totalRange, series, minIndexX, maxIndexX));
				}
			}
			return range;
		}
		void FindIndex(IList<RefinedPoint> points, IMinMaxValues range, out int minIndex, out int maxIndex) {
			minIndex = 0;
			maxIndex = -1;
			if (points.Count == 0)
				return;
			bool pointIsFound;
			int index = GetIndexByArgument(points, range.Min, out pointIsFound);
			if (index > (points.Count - 1) && !pointIsFound)
				return;
			minIndex = index;
			index = GetIndexByArgument(points, range.Max, out pointIsFound);
			if (index <= 0 && !pointIsFound) {
				minIndex = 0;
				return;
			}
			else if (index >= points.Count - 1)
				maxIndex = points.Count - 1;
			else {
				if (!pointIsFound)
					maxIndex = index - 1;
				else
					maxIndex = index;
			}
		}
		int GetIndexByArgument(IList<RefinedPoint> points, double argument, out bool pointIsFound) {
			RefinedPoint fakePoint = new RefinedPoint(null, argument, 0);
			RefinedPointArgumentComparer pointArgumentComparer = new RefinedPointArgumentComparer();
			SortedRefinedPointCollection sortedPoints = points as SortedRefinedPointCollection;
			int index = -1;
			if (sortedPoints != null) {
				RefinedPointsArgumentComparer comparer = new RefinedPointsArgumentComparer();
				index = sortedPoints.BinarySearch(fakePoint, comparer);
			}
			else if (points is List<RefinedPoint>)
				index = ((List<RefinedPoint>)points).BinarySearch(fakePoint, pointArgumentComparer);
			pointIsFound = true;
			if (index < 0) {
				index = ~index;
				pointIsFound = false;
			}
			return index;
		}
		MinMaxValues GetMinMaxFromSeries(MinMaxValues totalRange, RefinedSeries refinedSeries, int minIndexX, int maxIndexX) {
			MinMaxValues filteredRange = new MinMaxValues(double.MaxValue, double.MinValue);
			int i = minIndexX;
			int j = maxIndexX;
			bool isStackedPoint = IsStackedSeries(refinedSeries.SeriesView);
			while (i <= j) {
				RefinedPoint point = (RefinedPoint)refinedSeries.FinalPointsSortedByArgument[i];
				filteredRange.Union(refinedSeries.SeriesView.GetRefinedPointMin(point));
				filteredRange.Union(refinedSeries.SeriesView.GetRefinedPointMax(point));
				if (isStackedPoint) {
					filteredRange.Min = Math.Max(filteredRange.Min, ((IStackedPoint)point).TotalMinValue);
					filteredRange.Union(((IStackedPoint)point).TotalMaxValue);
				}
				point = (RefinedPoint)refinedSeries.FinalPointsSortedByArgument[j];
				filteredRange.Union(refinedSeries.SeriesView.GetRefinedPointMin(point));
				filteredRange.Union(refinedSeries.SeriesView.GetRefinedPointMax(point));
				if (isStackedPoint) {
					filteredRange.Min = Math.Max(filteredRange.Min, ((IStackedPoint)point).TotalMinValue);
					filteredRange.Union(((IStackedPoint)point).TotalMaxValue);
				}
				if (filteredRange == totalRange)
					break;
				i++;
				j--;
			}
			return filteredRange;
		}
		object GetKey(RefinedSeries refinedSeries) {
			if (refinedSeries.SeriesView.NeedSeriesInteraction) {
				IXYSeriesView xyView = refinedSeries.SeriesView as IXYSeriesView;
				if (xyView != null)
					return new InteractionKey(xyView, false);
			}
			return refinedSeries;
		}
		object GetSeriesGroupsKey(RefinedSeries refinedSeries) {
			if (refinedSeries.SeriesView.NeedSeriesGroupsInteraction)
				return new InteractionKey(refinedSeries.SeriesView, true);
			return null;
		}
		List<object> FindInteractionKeys(ICollection<RefinedSeries> seriesCollection) {
			List<object> interactionKeys = new List<object>();
			foreach (var series in seriesCollection) {
				foreach (var interactionKey in containers.Keys) {
					if (containers[interactionKey].Contains(series)) {
						if (!interactionKeys.Contains(interactionKey))
							interactionKeys.Add(interactionKey);
						break;
					}
				}
			}
			return interactionKeys;
		}
		SeriesContainer GetContainer(RefinedSeries refinedSeries, bool onlyExisting) {
			object key = GetKey(refinedSeries);
			SeriesContainer container = null;
			if (containers.ContainsKey(key))
				container = containers[key];
			else if (!onlyExisting) {
				container = refinedSeries.SeriesView.CreateContainer();
				containers.Add(key, container);
			}
			return container;
		}
		SeriesInteractionContainer GetSeriesGroupsContainer(RefinedSeries refinedSeries, bool onlyExisting) {
			object key = GetSeriesGroupsKey(refinedSeries);
			SeriesInteractionContainer container = null;
			if (key != null) {
				if (seriesGroupsContainers.ContainsKey(key))
					container = seriesGroupsContainers[key];
				else if (!onlyExisting) {
					container = refinedSeries.SeriesView.CreateSeriesGroupsContainer();
					seriesGroupsContainers.Add(key, container);
				}
			}
			return container;
		}
		public void SwapRefinedSeries(RefinedSeries series1, RefinedSeries series2) {
			SeriesInteractionContainer container1 = GetContainer(series1, false) as SeriesInteractionContainer;
			SeriesInteractionContainer container2 = GetContainer(series2, false) as SeriesInteractionContainer;
			if (object.Equals(container1, container2) && container1 != null)
				container1.SwapSeries(series1, series2);
			SeriesInteractionContainer groupsContainer1 = GetSeriesGroupsContainer(series1, false);
			SeriesInteractionContainer groupsContainer2 = GetSeriesGroupsContainer(series2, false);
			if (object.Equals(groupsContainer1, groupsContainer2) && groupsContainer1 != null)
				groupsContainer1.SwapSeries(series1, series2);
		}
		public void AddSeries(RefinedSeries refinedSeries) {
			SeriesContainer container = GetContainer(refinedSeries, false);
			SeriesInteractionContainer interactionContainer = container as SeriesInteractionContainer;
			if (interactionContainer != null)
				interactionContainer.AddSeries(refinedSeries);
			else
				((SingleSeriesContainer)container).SetRefinedSeries(refinedSeries);
			SeriesInteractionContainer seriesGroupsContainer = GetSeriesGroupsContainer(refinedSeries, false);
			if (seriesGroupsContainer != null)
				seriesGroupsContainer.AddSeries(refinedSeries);
		}
		public void RemoveSeries(RefinedSeries refinedSeries) {
			SeriesContainer container;
			foreach (object interactionKey in containers.Keys) {
				container = containers[interactionKey];
				if (container.Contains(refinedSeries)) {
					SeriesInteractionContainer interactionContainer = container as SeriesInteractionContainer;
					if (interactionContainer != null && interactionContainer.Contains(refinedSeries)) {
						interactionContainer.RemoveSeries(refinedSeries);
						if (container.IsEmpty)
							containers.Remove(interactionKey);
					}
					else
						containers.Remove(interactionKey);
					break;
				}
			}
			foreach (object interactionKey in seriesGroupsContainers.Keys) {
				SeriesInteractionContainer seriesGroupsContainer = seriesGroupsContainers[interactionKey];
				if (seriesGroupsContainer.Contains(refinedSeries)) {
					seriesGroupsContainer.RemoveSeries(refinedSeries);
					if (seriesGroupsContainer.IsEmpty)
						seriesGroupsContainers.Remove(interactionKey);
					break;
				}
			}
		}
		internal void UpdatePoints(SingleSeriesUpdate updateResult) {
			var container = GetContainer(updateResult.RefinedSeries, true);
			if (container != null)
				container.UpdatePoints(updateResult);
			SeriesInteractionContainer seriesGroupsContainer = GetSeriesGroupsContainer(updateResult.RefinedSeries, true);
			if (seriesGroupsContainer != null)
				seriesGroupsContainer.UpdatePoints(updateResult);
		}
		public void RecalculateFor(ICollection<RefinedSeries> seriesCollection) {
			List<object> interactionKeys = FindInteractionKeys(seriesCollection);
			foreach (var interactionKey in interactionKeys)
				if (containers.ContainsKey(interactionKey))
					containers[interactionKey].Recalculate();
		}
		public bool IsFirstInContainer(RefinedSeries series) {
			SeriesContainer container = GetContainer(series, true);
			return container != null && !container.IsEmpty && container.Series[0] == series;
		}
		public bool HasSameContainers(IRefinedSeries refinedSeries1, IRefinedSeries refinedSeries2) {
			SeriesInteractionContainer seriesGroupsContainer1 = GetSeriesGroupsContainer((RefinedSeries)refinedSeries1, true);
			SeriesInteractionContainer seriesGroupsContainer2 = GetSeriesGroupsContainer((RefinedSeries)refinedSeries2, true);
			return object.Equals(GetContainer((RefinedSeries)refinedSeries1, true), GetContainer((RefinedSeries)refinedSeries2, true))
				|| seriesGroupsContainer1 != null && seriesGroupsContainer1.Equals(seriesGroupsContainer2);
		}
		public void Clear() {
			containers.Clear();
			seriesGroupsContainers.Clear();
		}
	}
}
