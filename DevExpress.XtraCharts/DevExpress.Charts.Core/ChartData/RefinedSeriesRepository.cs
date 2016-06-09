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

using DevExpress.Charts.ChartData;
using System;
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public interface IRefinedSeriesContainer {
		bool IsContainsProcessedPoints { get; }
		bool IsDesignMode { get; }
		bool IsFirstInContainer(RefinedSeries series);
		bool HasSameContainers(IRefinedSeries refinedSeries1, IRefinedSeries refinedSeries2);
	}
	public class RefinedSeriesRepository : IRefinedSeriesContainer {
		readonly List<RefinedSeries> refinedSeries = new List<RefinedSeries>();
		readonly List<RefinedSeries> activeRefinedSeries = new List<RefinedSeries>();
		readonly List<RefinedSeries> seriesForLegend = new List<RefinedSeries>();
		readonly RefinedSeriesGroupController groupController = new RefinedSeriesGroupController();
		readonly InteractionManager interactionManager;
		readonly IChartDataContainer dataContainer;
		internal RefinedSeriesGroupController GroupController { get { return groupController; } }
		public IEnumerable<RefinedSeries> RefinedSeries { get { return refinedSeries; } }
		public bool IsContainsProcessedSeries { get { return ((activeRefinedSeries != null) && (activeRefinedSeries.Count > 0)); } }
		public bool IsContainsProcessedPoints {
			get {
				foreach (var series in activeRefinedSeries) {
					if (series.IsContainsProcessedPoints)
						return true;
				}
				return false;
			}
		}
		public bool IsContainsProcessedNotEmptyPoints {
			get {
				foreach (var series in activeRefinedSeries) {
					if (series.IsContainsProcessedNotEmptyPoints)
						return true;
				}
				return false;
			}
		}
		public RefinedSeriesRepository(IChartDataContainer dataContainer) {
			interactionManager = new InteractionManager();
			this.dataContainer = dataContainer;
		}
		internal void AddToActive(RefinedSeries series, bool isVisibleAndCompatible, bool shouldBeDrawn) {
			if (isVisibleAndCompatible) {
				seriesForLegend.Add(series);
				if (shouldBeDrawn) {
					series.ActiveIndex = activeRefinedSeries.Count;
					activeRefinedSeries.Add(series);
				}
			}
		}
		internal void RemoveSeriesGroup(IAxisData axis) {
			List<RefinedSeries> list = groupController.RemoveSeriesGroup(axis);
			if (list != null) {
				foreach (RefinedSeries series in list)
					interactionManager.RemoveSeries(series);
			}
		}
		internal void RemoveSeries(ISeries series) {
			RemoveSeries(FindRefinedSeries(series));
		}
		internal void RemoveSeries(ICollection<ISeries> seriesList) {
			foreach (var series in seriesList)
				RemoveSeries(series);
		}
		internal void RemoveSeries(RefinedSeries series) {
			this.RemoveFromSeriesGroups(series);
			refinedSeries.Remove(series);
		}
		internal void InsertSeries(int newIndex, ISeries newItem) {
			refinedSeries.Insert(newIndex, RefineSeries(newItem));
		}
		internal void InsertSeries(int newIndex, ICollection<ISeries> seriesList) {
			if (seriesList == null)
				return;
			RefinedSeries[] block = new RefinedSeries[seriesList.Count];
			int index = 0;
			foreach (var series in seriesList)
				block[index++] = RefineSeries(series);
			refinedSeries.InsertRange(newIndex, block);
		}
		internal void CleanRefinedSeries() {
			refinedSeries.Clear();
			interactionManager.Clear();
			groupController.RemoveAllSeries();
		}
		internal void SwapRefinedSeries(int newIndex, int oldIndex) {
			RefinedSeries swap = refinedSeries[newIndex];
			refinedSeries[newIndex] = refinedSeries[oldIndex];
			refinedSeries[oldIndex] = swap;
			interactionManager.SwapRefinedSeries(refinedSeries[newIndex], refinedSeries[oldIndex]);
		}
		internal void MoveRefinedSeries(int newIndex, int oldIndex) {
			RefinedSeries move = refinedSeries[oldIndex];
			refinedSeries.Remove(move);
			refinedSeries.Insert(newIndex, move);
		}
		internal void UpdateRefinedSeries(int oldIndex, ISeries oldItem, ISeries newItem) {
			if (!object.Equals(oldItem, newItem)) {
				RefinedSeries series = FindRefinedSeries(oldItem);
				this.RemoveFromSeriesGroups(series);
				refinedSeries[oldIndex] = RefineSeries(newItem);
			}
		}
		RefinedSeries RefineSeries(ISeries series) {
			return new RefinedSeries(series, this);
		}
		internal void ClearRefinedSeries() {
			activeRefinedSeries.Clear();
			seriesForLegend.Clear();
		}
		internal void ClearActiveRefinedSeries() {
			activeRefinedSeries.Clear();
			foreach (RefinedSeries series in RefinedSeries)
				this.RemoveFromSeriesGroups(series);
		}
		internal bool UpdateInteraction(bool isContainsProcessedPointsBeforeUpdate, ChartUpdateAggregator updateAggregator, List<RefinedSeries> activityChangedSeries, List<RefinedSeriesGroup> scaleMapChangedGroups, bool IsDesignMode) {
			List<RefinedSeries> addedOrRemovedSeries = new List<RefinedSeries>();
			List<RefinedSeries> seriesForUpdate = new List<RefinedSeries>();
			if (updateAggregator != null) {
				foreach (SingleSeriesUpdate update in updateAggregator.GetUpdates<SingleSeriesUpdate>()) {
					if (update.RefinedSeries.IsActive) {
						if (update.ShouldUpdateInteraction)
							seriesForUpdate.Add(update.RefinedSeries);
						else
							interactionManager.UpdatePoints(update);
					}
				}
				foreach (BatchSeriesUpdate update in updateAggregator.GetUpdates<BatchSeriesUpdate>()) {
					foreach (RefinedSeries series in update.RefinedSeriesList) {
						if (!seriesForUpdate.Contains(series))
							seriesForUpdate.Add(series);
					}
				}
			}
			if (activityChangedSeries != null) {
				foreach (var series in activityChangedSeries) {
					if (!series.IsActive)
						interactionManager.RemoveSeries(series);
					else
						interactionManager.AddSeries(series);
					addedOrRemovedSeries.Add(series);
				}
			}
			if (IsDesignMode && (isContainsProcessedPointsBeforeUpdate != IsContainsProcessedPoints)) {
				foreach (RefinedSeries series in activeRefinedSeries) {
					if (!seriesForUpdate.Contains(series) && !addedOrRemovedSeries.Contains(series))
						seriesForUpdate.Add(series);
				}
			}
			else {
				foreach (var group in scaleMapChangedGroups) {
					foreach (RefinedSeries series in group.RefinedSeries) {
						if (!seriesForUpdate.Contains(series) && !addedOrRemovedSeries.Contains(series))
							seriesForUpdate.Add(series);
					}
				}
			}
			if (seriesForUpdate.Count > 0)
				interactionManager.RecalculateFor(seriesForUpdate);
			return addedOrRemovedSeries.Count > 0 || seriesForUpdate.Count > 0;
		}
		internal void UpdateSeriesGroupsInteraction(ChartUpdateAggregator updateAggregator, bool forceUpdate) {
			if (updateAggregator.ShouldUpdateSeriesGroupsInteraction || forceUpdate)
				foreach (var container in interactionManager.SeriesGroupsInteractionContainers)
					container.Recalculate();
		}
		internal RefinedSeries FindRefinedSeries(Func<RefinedSeries, bool> predicat) {
			foreach (RefinedSeries refinedSeriesItem in refinedSeries) {
				if (predicat(refinedSeriesItem))
					return refinedSeriesItem;
			}
			return null;
		}
		internal RefinedSeries FindRefinedSeries(ISeriesBase series) {
			if (series == null)
				return null;
			return FindRefinedSeries(x => x.Series == series);
		}
		internal List<RefinedSeries> GetRefinedSeries() {
			return refinedSeries;
		}
		internal List<RefinedSeries> GetActiveRefinedSeries() {
			return activeRefinedSeries;
		}
		internal List<RefinedSeries> GetSeriesForLegend() {
			return seriesForLegend;
		}
		internal InteractionManager GetInteractionManager() {
			return interactionManager;
		}
		internal void RemoveFromSeriesGroups(RefinedSeries series) {
			groupController.RemoveFromSeriesGroups(series);
			interactionManager.RemoveSeries(series);
		}
		#region IRefinedSeriesContainer
		bool IRefinedSeriesContainer.IsContainsProcessedPoints { get { return IsContainsProcessedPoints; } }
		bool IRefinedSeriesContainer.IsDesignMode { get { return dataContainer.DesignMode; } }
		bool IRefinedSeriesContainer.IsFirstInContainer(RefinedSeries series) { return interactionManager.IsFirstInContainer(series); }
		bool IRefinedSeriesContainer.HasSameContainers(IRefinedSeries refinedSeries1, IRefinedSeries refinedSeries2) {
			return interactionManager.HasSameContainers(refinedSeries1, refinedSeries2);
		}
		#endregion
	}
	public class RefinedSeriesGroupController {
		readonly Dictionary<RefinedSeriesGroupKey, RefinedSeriesGroup> refinedSeriesGroups = new Dictionary<RefinedSeriesGroupKey, RefinedSeriesGroup>();
		internal Dictionary<RefinedSeriesGroupKey, RefinedSeriesGroup> RefinedSeriesGroups { get { return refinedSeriesGroups; } }
		RefinedSeriesGroup GetGroup(RefinedSeriesGroupKey groupKey, IMinMaxValuesCalculator minMaxCalculator, bool isArgumentGroup) {
			if (groupKey == null)
				return null;
			RefinedSeriesGroup group;
			if (!refinedSeriesGroups.TryGetValue(groupKey, out group)) {
				if (isArgumentGroup)
					group = new RefinedSeriesGroupByArgument(groupKey);
				else
					group = new RefinedSeriesGroupByValue(groupKey, minMaxCalculator);
				refinedSeriesGroups.Add(groupKey, group);
			}
			return group;
		}
		RefinedSeriesGroupKey GetValueGroupKey(RefinedSeries series) {
			return new RefinedSeriesGroupKey(series, false);
		}
		RefinedSeriesGroupKey GetArgumentGroupKey(RefinedSeries series) {
			return new RefinedSeriesGroupKey(series, true);
		}
		void RemoveFromSeriesGroup(RefinedSeriesGroup seriesGroup, RefinedSeries series) {
			if (seriesGroup != null) {
				seriesGroup.RemoveSeries(series);
				if (seriesGroup.IsEmpty)
					refinedSeriesGroups.Remove(seriesGroup.GroupKey);
			}
		}
		RefinedSeriesGroup CreateFakeGroup(IAxisData axisData, IMinMaxValuesCalculator minMaxCalculator) {
			RefinedSeriesGroupKey groupKey = new RefinedSeriesGroupKey(axisData);
			RefinedSeriesGroup group;
			if (axisData.IsArgumentAxis)
				group = new RefinedSeriesGroupByArgument(groupKey);
			else
				group = new RefinedSeriesGroupByValue(groupKey, minMaxCalculator);
			return group;
		}
		internal void RemoveFromSeriesGroups(RefinedSeries series) {
			if (series != null) {
				if (series.ArgumentGroup != null && series.ArgumentGroup.GroupKey != null) {
					IAxisData axisData = series.ArgumentGroup.GroupKey.AxisData as IAxisData;
					RangeHelper.ResetRangeOnRemoveSeriesFromSeriesGroups(axisData);
					RemoveFromSeriesGroup(series.ArgumentGroup, series);
				}
				if (series.ValueGroup != null && series.ValueGroup.GroupKey != null) {
					IAxisData axisData = series.ValueGroup.GroupKey.AxisData as IAxisData;
					RangeHelper.ResetRangeOnRemoveSeriesFromSeriesGroups(axisData);
					RemoveFromSeriesGroup(series.ValueGroup, series);
				}
				series.BindToGroups(null, null);
			}
		}
		internal void RemoveAllSeries() {
			foreach (RefinedSeriesGroup group in refinedSeriesGroups.Values) {
				group.RemoveAllSeries();
			}			
		}	   
		internal RefinedSeriesGroup FindRefinedSeriesGroup(IAxisData axis) {
			foreach (var groupKey in refinedSeriesGroups.Keys) {
				if (groupKey != null && groupKey.AxisData != null && groupKey.AxisData == axis)
					return refinedSeriesGroups[groupKey];
			}
			return null;
		}
		internal RefinedSeriesGroup GetArgumentGroup(RefinedSeries series) {
			return GetGroup(GetArgumentGroupKey(series), null, true);
		}
		internal RefinedSeriesGroup GetValueGroup(RefinedSeries series, IMinMaxValuesCalculator minMaxCalculator) {
			return GetGroup(GetValueGroupKey(series), minMaxCalculator, false);
		}
		internal List<RefinedSeries> RemoveSeriesGroup(IAxisData axis) {
			var seriesGroup = this.FindRefinedSeriesGroup(axis);
			if (seriesGroup == null)
				return null;
			List<RefinedSeries> list = new List<RefinedSeries>(seriesGroup.RefinedSeries);
			foreach (RefinedSeries series in list)
				RemoveFromSeriesGroups(series);
			return list;
		}
		internal RefinedSeriesGroup InsertGroup(IAxisData axis, IMinMaxValuesCalculator calculator) {
			RefinedSeriesGroup group = FindRefinedSeriesGroup(axis);
			if (group == null) {
				RefinedSeriesGroupKey groupKey = new RefinedSeriesGroupKey(axis);
				group = GetGroup(groupKey, calculator, axis.IsArgumentAxis);
				group.UpdateScaleMap(false);
			}
			return group;
		}
		internal void RemoveGroup(IAxisData axis) {
			RemoveSeriesGroup(axis);
			RefinedSeriesGroup seriesGroup = FindRefinedSeriesGroup(axis);
			if (seriesGroup != null)
				refinedSeriesGroups.Remove(seriesGroup.GroupKey);
		}
		internal void ClearGroups(IEnumerable<IAxisData> axes) {
			if (axes != null)
				foreach (IAxisData axis in axes)
					RemoveGroup(axis);
		}
		internal void CalculateFilteredIndexes() {
			foreach (RefinedSeriesGroup refinedSeriesGroup in refinedSeriesGroups.Values) {
				RefinedSeriesGroupByArgument groupByArgument = refinedSeriesGroup as RefinedSeriesGroupByArgument;
				if (groupByArgument != null)
					groupByArgument.CalculateFilteredIndexes();
			}
		}
		internal List<RefinedSeriesGroup> UpdateQualitativeScaleMap() {
			List<RefinedSeriesGroup> changedGroups = new List<RefinedSeriesGroup>();
			foreach (var group in refinedSeriesGroups.Values)
				if (group.ScaleMap.ScaleType == ActualScaleType.Qualitative) {
					group.UpdateScaleMap(true);
					changedGroups.Add(group);
				}
			return changedGroups;
		}
		internal void UpdateAxisData(IEnumerable<IAxisData> axes, IMinMaxValuesCalculator minMaxCalculator) {
			if (axes == null)
				return;
			foreach (IAxisData axisData in axes) {
				RefinedSeriesGroup group = FindRefinedSeriesGroup(axisData);
				if (group == null)
					group = CreateFakeGroup(axisData, minMaxCalculator);
				group.UpdateSideMarginsEnable();
			}
			foreach (IAxisData axisData in axes) {
				RefinedSeriesGroup group = FindRefinedSeriesGroup(axisData);
				if (group == null)
					group = CreateFakeGroup(axisData, minMaxCalculator);
				group.UpdateTransformation();
				group.UpdateScaleBreaks();
				group.UpdateAxisData();
			}
		}
		internal void ClearSeparatePaneIndicators() {
			foreach (var group in this.refinedSeriesGroups.Values) {
				var valueGroup = group as RefinedSeriesGroupByValue;
				if (valueGroup != null)
					valueGroup.ClearIndicators();
			}
		}
	}
}
