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
	public class CollectionManager {
		const int SortedByArgumentPointsIndex = 0;
		const int SortedBySettingsPointsIndex = 1;
		RefinedSeries refinedSeries;
		int minUpdatePointIndex = -1;
		RefinedPointCollection points;
		List<SortedRefinedPointCollection> sortedCollections;
		RefinedPointsArgumentComparer argumentComparer = new RefinedPointsArgumentComparer();
		bool IsContainsSortedBySettingsPoints { get { return sortedCollections.Count > SortedBySettingsPointsIndex; } }
		public RefinedPointCollectionBase Points { get { return points; } }
		public RefinedPointCollectionBase PointsSortedByArgument { get { return sortedCollections[SortedByArgumentPointsIndex]; } }
		public RefinedPointCollectionBase PointsSortedBySettings {
			get {
				if (IsContainsSortedBySettingsPoints)
					return sortedCollections[SortedBySettingsPointsIndex];
				return points;
			}
		}
		public CollectionManager(RefinedSeries refinedSeries) {
			int count = 0;
			this.refinedSeries = refinedSeries;
			if (refinedSeries.Series.ActualPoints != null)
				count = refinedSeries.Series.ActualPoints.Count;
			points = new RefinedPointCollection(count);
			sortedCollections = new List<SortedRefinedPointCollection>();
			SortedRefinedPointCollection sortedByArgumentPoints = new SortedRefinedPointCollection(Points.Count, new RefinedPointsArgumentAndIndexComparer());
			sortedCollections.Add(sortedByArgumentPoints);
			if (refinedSeries.Series.SeriesPointsSortingMode != SortMode.None) {
				SortedRefinedPointCollection sortedPoints = new SortedRefinedPointCollection(Points.Count, new SeriesPointSettingsComparer(refinedSeries.Series.SeriesPointsSortingMode == SortMode.Ascending, refinedSeries.Series.SeriesPointsSortingKey, refinedSeries.Series.ValueScaleType, refinedSeries.Series.ArgumentScaleType));
				sortedCollections.Add(sortedPoints);
			}
		}
		RefinedPoint RefinePoint(IPointProcessor processor, ISeriesPoint seriesPoint) {
			if (seriesPoint != null) {
				RefinedPoint res = new RefinedPoint(seriesPoint);
				ProcessPoint(processor, res);
				return res;
			}
			return null;
		}
		void ProcessPoint(IPointProcessor processor, RefinedPoint point) {
			if (processor != null) {
				var argumentMap = (refinedSeries.ArgumentGroup == null) ? null : refinedSeries.ArgumentGroup.ScaleMap;
				var valueMap = (refinedSeries.ValueGroup == null) ? null : refinedSeries.ValueGroup.ScaleMap;
				point.IsEmpty = point.SeriesPoint.IsEmpty((Scale)refinedSeries.ValueScaleType);
				processor.ProcessPoint(point, argumentMap, valueMap);
			}
		}
		SinglePointUpdate PerformUpdate(IPointProcessor processor, ChartCollectionOperation operation, ISeriesPoint oldPoint, int oldIndex, ISeriesPoint newPoint, int newIndex) {
			RefinedPoint refinedPoint = null;
			switch (operation) {
				case ChartCollectionOperation.Reset:
					points.Clear();
					sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Clear(); });
					return new SinglePointUpdate(operation, refinedSeries, null);
				case ChartCollectionOperation.InsertItem:
					refinedPoint = RefinePoint(processor, newPoint);
					TryUpdatePointsIndices(newIndex, refinedPoint);
					points.Insert(newIndex, refinedPoint);
					if (refinedSeries.ArgumentGroup != null)
						UpdatePointsIndices();
					sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Add(refinedPoint); });
					return new SinglePointUpdate(operation, refinedSeries, refinedPoint);
				case ChartCollectionOperation.RemoveItem:
					refinedPoint = Points[oldIndex];
					points.Remove(refinedPoint);
					sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Remove(refinedPoint); });
					return new SinglePointUpdate(operation, refinedSeries, refinedPoint);
				case ChartCollectionOperation.UpdateItem:
					refinedPoint = RefinePoint(processor, newPoint);
					var oldRefinedPoint = Points[newIndex];
					points[newIndex] = refinedPoint;
					refinedPoint.Index = oldRefinedPoint.Index;
					sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Update(oldRefinedPoint, refinedPoint); });
					((SortedRefinedPointCollection)PointsSortedByArgument).Sort();
					return new SinglePointUpdate(operation, refinedSeries, ChartUpdateType.Empty, refinedPoint, oldRefinedPoint);
				case ChartCollectionOperation.MoveItem:
					TryUpdatePointsIndices(newIndex, points[oldIndex]);
					points.Move(oldIndex, newIndex);
					if (refinedSeries.ArgumentGroup != null)
						UpdatePointsIndices();
					((SortedRefinedPointCollection)PointsSortedByArgument).Sort();
					break;
				case ChartCollectionOperation.SwapItem:
					points.Swap(oldIndex, newIndex);
					if (points[oldIndex].Argument == points[newIndex].Argument) {
						var olPointIndex = points[oldIndex].Index;
						points[oldIndex].Index = points[newIndex].Index;
						points[newIndex].Index = olPointIndex;
						((SortedRefinedPointCollection)PointsSortedByArgument).Sort();
					}
					break;
				case ChartCollectionOperation.Clear:
					break;
				default:
					throw new NotImplementedException("ChartCollectionOperation: " + operation);
			}
			return null;
		}
		internal List<RefinedPoint> FindAllPointsWithSameArgument(RefinedPoint refinedPoint) {
			List<RefinedPoint> pointsWithSameArgument = new List<RefinedPoint>();
			int index = ((SortedRefinedPointCollection)PointsSortedByArgument).BinarySearch(refinedPoint, argumentComparer);
			if (index >= 0) {
				for (int i = index; i < PointsSortedByArgument.Count && refinedPoint.Argument == PointsSortedByArgument[i].Argument; i++) {
					if (refinedPoint != PointsSortedByArgument[i])
						pointsWithSameArgument.Add(PointsSortedByArgument[i]);
				}
				for (int i = index - 1; i >= 0 && refinedPoint.Argument == PointsSortedByArgument[i].Argument; i--) {
					if (refinedPoint != PointsSortedByArgument[i])
						pointsWithSameArgument.Add(PointsSortedByArgument[i]);
				}
			}
			return pointsWithSameArgument;
		}
		bool DetectPointIndexOverflow(int pointsCount, int pointIndex) {
			if (pointIndex < 0)
				return false;
			return (Int32.MaxValue - pointIndex) < pointsCount;
		}
		void TryUpdatePointsIndices(int sourceCollectionStartIndex, params RefinedPoint[] refinedPoints) {
			if (minUpdatePointIndex < 0) {
				if (sourceCollectionStartIndex == points.Count) {
					int previousPointIndex = sourceCollectionStartIndex == 0 ? -1 : points[sourceCollectionStartIndex - 1].Index;
					int startIndex = previousPointIndex + 1;
					if (!DetectPointIndexOverflow(refinedPoints.Length, previousPointIndex))
						for (int pointIndex = 0; pointIndex < refinedPoints.Length; pointIndex++) {
							refinedPoints[pointIndex].Index = startIndex + pointIndex;
						}
					else
						minUpdatePointIndex = 0;
				}
				else
					minUpdatePointIndex = sourceCollectionStartIndex;
			}
			else
				minUpdatePointIndex = Math.Min(minUpdatePointIndex, sourceCollectionStartIndex);
		}
		BatchPointsUpdate PerformBatchUpdate(IPointProcessor processor,ChartCollectionOperation operation, ICollection<ISeriesPoint> oldPoints, int oldIndex, ICollection<ISeriesPoint> newPoints, int newIndex) {
			RefinedPoint[] refinedPoints;
			switch (operation) {
				case ChartCollectionOperation.InsertItem:
					if (newPoints == null)
						break;
					refinedPoints = new RefinedPoint[newPoints.Count];
					int index = 0;
					foreach (var point in newPoints)
						refinedPoints[index++] = RefinePoint(processor, point);
					TryUpdatePointsIndices(newIndex, refinedPoints);
					points.InsertRange(newIndex, refinedPoints);
					if (refinedSeries.ArgumentGroup != null)
						UpdatePointsIndices();
					sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.AddRange(refinedPoints); });
					return new BatchPointsUpdate(operation, refinedSeries, refinedPoints);
				case ChartCollectionOperation.RemoveItem:
					if ((oldIndex < 0) || (oldPoints == null))
						break;
					int bufferSize = oldPoints.Count;
					refinedPoints = new RefinedPoint[bufferSize];
					for (int i = 0; i < bufferSize; i++)
						refinedPoints[i] = Points[oldIndex + i];
					for (int i = 0; i < bufferSize; i++) {
						PointsSortedByArgument.Remove(refinedPoints[i]);
						if (IsContainsSortedBySettingsPoints)
							sortedCollections[SortedBySettingsPointsIndex].Remove(refinedPoints[i]);
						points.Remove(refinedPoints[i]);
					}
					return new BatchPointsUpdate(operation, refinedSeries, refinedPoints);
				case ChartCollectionOperation.Reset:
					points.Clear();
					sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Clear(); });
					return new BatchPointsUpdate(operation, refinedSeries, null);
				case ChartCollectionOperation.Clear:
					break;
				default:
					throw new NotSupportedException("The batch " + operation + " operation doesn't supported");
			}
			return null;
		}
		RefinedPoint FindRefinePointBySeriesPoint(ISeriesPoint seriesPoint) {
			RefinedPoint refinedPoint = null;
			for (int i = 0; i < points.Count; i++)
				if (points[i].SeriesPoint == seriesPoint) {
					refinedPoint = points[i];
					break;
				}
			return refinedPoint;
		}
		List<RefinedPoint> GetPointsByArgument(double argument) {
			RefinedPoint fakePoint = new RefinedPoint(null, argument, 0);
			SortedRefinedPointCollection sortedPoints = (SortedRefinedPointCollection)PointsSortedByArgument;
			if (sortedPoints.Count == 0)
				return null;
			int index = sortedPoints.BinarySearch(fakePoint, argumentComparer);
			if (index < 0) {
				index = ~index;
				if (index > sortedPoints.Count - 1)
					index = sortedPoints.Count - 1;
				else if (index > 0) {
					if (argument - sortedPoints[index - 1].Argument <= sortedPoints[index].Argument - argument)
						index -= 1;
				}
				argument = sortedPoints[index].Argument;
			}
			List<RefinedPoint> pointsByArgument = new List<RefinedPoint>();
			for (int i = index; ((i < PointsSortedByArgument.Count) && (PointsSortedByArgument[i].Argument == argument)); i++)
				pointsByArgument.Add(PointsSortedByArgument[i]);
			for (int i = index - 1; ((i >= 0) && (PointsSortedByArgument[i].Argument == argument)); i--)
				pointsByArgument.Add(PointsSortedByArgument[i]);
			return pointsByArgument;
		}
		internal SingleSeriesUpdate UpdateCollections(IPointProcessor processor, CollectionUpdateInfo updateInfo) {
			if (updateInfo is SeriesPointCollectionBatchUpdateInfo) {
				SeriesPointCollectionBatchUpdateInfo batchUpdate = updateInfo as SeriesPointCollectionBatchUpdateInfo;
				return PerformBatchUpdate(processor, batchUpdate.Operation, batchUpdate.OldItem, batchUpdate.OldIndex, batchUpdate.NewItem, batchUpdate.NewIndex);
			}
			SeriesPointCollectionUpdateInfo pointUpdateInfo = updateInfo as SeriesPointCollectionUpdateInfo;
			return PerformUpdate(processor, pointUpdateInfo.Operation, pointUpdateInfo.OldItem, pointUpdateInfo.OldIndex, pointUpdateInfo.NewItem, pointUpdateInfo.NewIndex);
		}
		internal SingleSeriesUpdate UpdatePoint(IPointProcessor processor, PropertyUpdateInfo<ISeries, ISeriesPoint> seriesPointUpdateInfo) {
			if (seriesPointUpdateInfo.Name != "Values" && seriesPointUpdateInfo.Name != "Argument")
				return null;
			RefinedPoint oldPoint = FindRefinePointBySeriesPoint(seriesPointUpdateInfo.Sender as ISeriesPoint);
			if(oldPoint == null)
				return null;
			RefinedPoint newPoint = new RefinedPoint(oldPoint.SeriesPoint) { Index = oldPoint.Index };
			ProcessPoint(processor, newPoint);
			int index = points.IndexOf(oldPoint);
			points[index] = newPoint;
			SingleSeriesUpdate updateResult = new SinglePointUpdate(ChartCollectionOperation.UpdateItem, this.refinedSeries, ChartUpdateType.Empty, newPoint, oldPoint);
			sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Remove(oldPoint); });
			sortedCollections.ForEach((SortedRefinedPointCollection pointsCollection) => { pointsCollection.Add(newPoint); });
			return updateResult;
		}
		public void UpdatePointsIndices() {
			if (minUpdatePointIndex >= 0) {
				int newPointIndex = minUpdatePointIndex == 0 ? 0 : points[minUpdatePointIndex - 1].Index + 1;
				for (int pointIndex = minUpdatePointIndex; pointIndex < points.Count; pointIndex++) {
					points[pointIndex].Index = newPointIndex;
					newPointIndex++;
				}
			}
			minUpdatePointIndex = -1;
		}
		public void ProcessPoints(IPointProcessor processor) {
			for (int i = 0; i < points.Count; i++)
				ProcessPoint(processor, points[i]);
			sortedCollections[SortedByArgumentPointsIndex].Sort();
		}
		public void ProcessSortingPointModeUpdate(SortMode oldSortingMode, SortMode newSortingMode, SeriesPointKeyNative pointsSortingKey) {
			if (oldSortingMode == newSortingMode)
				return;
			if (newSortingMode == SortMode.None) {
				if (sortedCollections.Count > 1)
					sortedCollections.RemoveAt(SortedBySettingsPointsIndex);
			}
			else {
				bool isAscending = newSortingMode == SortMode.Ascending;
				SeriesPointSettingsComparer comparer = new SeriesPointSettingsComparer(isAscending, pointsSortingKey, refinedSeries.Series.ValueScaleType, refinedSeries.Series.ArgumentScaleType);
				if (oldSortingMode == SortMode.None) {
					SortedRefinedPointCollection sortedPoints = new SortedRefinedPointCollection(Points.Count, comparer);
					sortedPoints.Initialize(Points);
					sortedCollections.Add(sortedPoints);
				}
				else
					sortedCollections[SortedBySettingsPointsIndex].SetComparer(comparer);
				sortedCollections[SortedBySettingsPointsIndex].Sort();
			}
		}
		public void ProcessSortingPointKeyUpdate(SeriesPointKeyNative oldSortingKey, SeriesPointKeyNative newSortingKey, SortMode pointsSortingMode) {
			if (newSortingKey == oldSortingKey || pointsSortingMode == SortMode.None)
				return;
			bool isAscending = pointsSortingMode == SortMode.Ascending;
			SeriesPointSettingsComparer comparer = new SeriesPointSettingsComparer(isAscending, newSortingKey, refinedSeries.Series.ValueScaleType, refinedSeries.Series.ArgumentScaleType);
			sortedCollections[SortedBySettingsPointsIndex].Sort(comparer);
		}
		public RefinedPoint GetMinPoint(double argument) {
			List<RefinedPoint> pointsByArgument = GetPointsByArgument(argument);
			if (pointsByArgument == null)
				return null;
			double minValue = double.MaxValue;
			RefinedPoint pointWithMinValue = null;
			foreach (var point in pointsByArgument) {
				double value = refinedSeries.Series.SeriesView.GetRefinedPointMin(point);
				if (minValue > value) {
					minValue = value;
					pointWithMinValue = point;
				}
			}
			return pointWithMinValue;
		}
		public RefinedPoint GetMaxPoint(double argument) {
			List<RefinedPoint> pointsByArgument = GetPointsByArgument(argument);
			if (pointsByArgument == null)
				return null;
			double maxValue = double.MinValue;
			RefinedPoint pointWithMaxValue = null;
			foreach (var point in pointsByArgument) {
				double value = refinedSeries.Series.SeriesView.GetRefinedPointMax(point);
				if (maxValue < value) {
					maxValue = value;
					pointWithMaxValue = point;
				}
			}
			return pointWithMaxValue;
		}
		public double GetMinAbsArgument() {
			double argument = 0;
			if (PointsSortedByArgument.Count == 0)
				return double.MaxValue;
			RefinedPoint fakePoint = new RefinedPoint(null, argument, 0);
			int index = ((SortedRefinedPointCollection)PointsSortedByArgument).BinarySearch(fakePoint, argumentComparer);
			if (index < 0) {
				index = ~index;
				if (index == 0)
					return PointsSortedByArgument[0].Argument;
				if (index >= PointsSortedByArgument.Count)
					return PointsSortedByArgument[PointsSortedByArgument.Count - 1].Argument;
				return Math.Min(Math.Abs(PointsSortedByArgument[index].Argument), Math.Abs(PointsSortedByArgument[index - 1].Argument));
			}
			if (index == 0) {
				double value = PointsSortedByArgument[0].Argument;
				double value1 = PointsSortedByArgument.Count > 1 ? PointsSortedByArgument[1].Argument : 0;
				if (value == 0 && value1 > 0)
					return value1;
				return PointsSortedByArgument[0].Argument;
			}
			if (index >= PointsSortedByArgument.Count - 1)
				return PointsSortedByArgument[PointsSortedByArgument.Count - 1].Argument;
			return Math.Min(Math.Abs(PointsSortedByArgument[index + 1].Argument), Math.Abs(PointsSortedByArgument[index - 1].Argument));
		}
	}
}
