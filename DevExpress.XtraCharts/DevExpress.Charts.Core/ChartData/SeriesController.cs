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
using DevExpress.Charts.ChartData;
namespace DevExpress.Charts.Native {
	[Flags]
	public enum SeriesControllerChanges : int {
		NoChanges = 0,
		RefinedDataUpdated = 1,
		WholeArgumentRangeUpdated = 2,
		VisualArgumentRangeUpdated = 4,
		ShouldUpdateMeasureUnits = 8,
		ShouldResetSelectedItems = 16,
	}
	[Flags]
	public enum SeriesControllerUpdateSeriesActions : int {
		NoAction = 0,
		UpdateSortingPointMode = 1,
		UpdateSortingPointKey = 2,
		RemoveFromGroup = 4,
		UpdateRandomPoints = 8,
		UpdateFilter = 16,
		UpdateArgumentScale = 32
	}
	public partial class SeriesController {
		#region Nested Classes : SeriesControllerTransaction
		public class SeriesControllerTransaction {
			readonly SeriesController controller;
			readonly ChartUpdateAggregator aggregator;
			SeriesControllerChanges currentChanges;
			bool pointsUpdated;
			bool wasProcessedPointsOnOpen;
			bool allowCommit;
			bool loading;
			public SeriesControllerTransaction(SeriesController seriesController, bool loading) {
				this.controller = seriesController;
				this.pointsUpdated = false;
				this.allowCommit = false;
				this.wasProcessedPointsOnOpen = seriesController.IsContainsProcessedPoints;
				this.loading = loading;
				this.aggregator = new ChartUpdateAggregator();
			}
			public void Process(ChartUpdateInfoBase updateInfo) {
				if (updateInfo != null) {
					if (updateInfo is LightUpdateInfo) {
						currentChanges |= controller.ProcessLightUpdate((LightUpdateInfo)updateInfo);
						return;
					}
					allowCommit = true;
					if (loading || (updateInfo is OnLoadEndUpdateInfo) || (updateInfo is RefreshDataUpdateInfo))
						controller.seriesRepository.ClearActiveRefinedSeries();
					PropertyUpdateInfo propertyUpdateInfo = updateInfo as PropertyUpdateInfo;
					IList<ChartUpdate> actions = new List<ChartUpdate>();
					if (propertyUpdateInfo != null) {
						actions = controller.ProcessPropertyUpdate(propertyUpdateInfo);
						pointsUpdated = pointsUpdated || actions.Count > 0;
					}
					else if (updateInfo is SeriesGroupsInteractionUpdateInfo)
						actions.Add(new ChartUpdate(ChartUpdateType.UpdateSeriesGroupsInteraction));
					else if (updateInfo is RangeResetUpdateInfo)
						actions.Add(new RangeResetUpdate(((RangeResetUpdateInfo)updateInfo).Range));
					else if (updateInfo is OnLoadEndUpdateInfo) {
						actions.Add(new ChartUpdate(ChartUpdateType.UpdateArgumentScale));
						actions.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
						actions.Add(new ChartUpdate(ChartUpdateType.DeserializeRange));
						actions.Add(new ChartUpdate(ChartUpdateType.UpdatePointsIndices));
					}
					else if (updateInfo is RefreshDataUpdateInfo)
						actions.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
					else if (updateInfo is DataAggregationUpdate)
						actions = controller.ProcessDataAggregationUpdate(((DataAggregationUpdate)updateInfo).Sender);
					else if (updateInfo is SeriesBindingUpdate) {
						ISeries series = ((SeriesBindingUpdate)updateInfo).Sender;
						RefinedSeries updatedSeries = controller.seriesRepository.FindRefinedSeries(series);
						if (updatedSeries != null)
							actions.Add(new SingleSeriesUpdate(updatedSeries, ChartUpdateType.UpdateScaleMap));
					}
					else if (updateInfo is ReprocessPointsUpdate) {
						RefinedSeries updatedSeries = controller.ReprocessPoints(((ReprocessPointsUpdate)updateInfo).Series);
						if (updatedSeries != null) {
							aggregator.Add(new SingleSeriesUpdate(updatedSeries, ChartUpdateType.UpdateInteraction |
								ChartUpdateType.UpdateSeriesGroupsInteraction |
								ChartUpdateType.UpdateScaleMap |
								ChartUpdateType.UpdateSideMargins |
								ChartUpdateType.UpdateCrosshair));
							pointsUpdated = true;
						}
					}
					else {
						actions = controller.ProcessCollectionUpdate(updateInfo);
						pointsUpdated = pointsUpdated || actions.Count > 0;
						if (pointsUpdated)
							actions.Add(new ChartUpdate(ChartUpdateType.ResetSelectedItems));
					}
					aggregator.AddRange(actions);
				}
			}
			public SeriesControllerChanges Commit() {
				if (allowCommit && !this.loading)
					currentChanges |= controller.ApplyUpdates(aggregator, pointsUpdated, wasProcessedPointsOnOpen);
				return currentChanges;
			}
		}
		#endregion
		internal RefinedSeries RefineSeries(ISeries series, SeriesController seriesController) {
			return new RefinedSeries(series, seriesController.seriesRepository);
		}
		readonly SeriesIncompatibilityStatistics incompatibilityStatistics = new SeriesIncompatibilityStatistics();
		SeriesControllerTransaction transaction;
		RefinedSeriesRepository seriesRepository;
		readonly IChartDataContainer dataContainer;
		IDiagram diagram;
		CompatibleViewType? compatibleViewFormDiagram;
		bool isDiagram3D;
		public List<RefinedSeries> RefinedSeries { get { return seriesRepository.GetRefinedSeries(); } }
		public List<RefinedSeries> ActiveRefinedSeries { get { return seriesRepository.GetActiveRefinedSeries(); } }
		public List<RefinedSeries> SeriesForLegend { get { return seriesRepository.GetSeriesForLegend(); } }
		public Dictionary<RefinedSeriesGroupKey, RefinedSeriesGroup> RefinedSeriesGroups { get { return seriesRepository.GroupController.RefinedSeriesGroups; } }
		public SeriesIncompatibilityStatistics SeriesIncompatibilityStatistics { get { return incompatibilityStatistics; } }
		public InteractionManager InteractionManager { get { return seriesRepository.GetInteractionManager(); } }
		public bool IsContainsProcessedSeries { get { return seriesRepository.IsContainsProcessedSeries; } }
		public bool IsContainsProcessedPoints { get { return seriesRepository.IsContainsProcessedPoints; } }
		public bool IsContainsProcessedNotEmptyPoints { get { return seriesRepository.IsContainsProcessedNotEmptyPoints; } }
		public bool IsLastUpdateInDesignMode { get; private set; }
		public SeriesController(IChartDataContainer dataContainer) {
			seriesRepository = new RefinedSeriesRepository(dataContainer);
			this.dataContainer = dataContainer;
		}
		public SeriesController(IChartDataContainer dataContainer, CompatibleViewType compatibleViewFormDiagram, bool isDiagram3D, IDiagram diagram)
			: this(dataContainer) {
			this.compatibleViewFormDiagram = compatibleViewFormDiagram;
			this.isDiagram3D = isDiagram3D;
			this.diagram = diagram;
		}
		void UpdateRefinedSeriesList(SeriesCollectionUpdateInfo updateInfo) {
			switch (updateInfo.Operation) {
				case ChartCollectionOperation.Reset:
					seriesRepository.CleanRefinedSeries();
					break;
				case ChartCollectionOperation.InsertItem:
					seriesRepository.InsertSeries(updateInfo.NewIndex, updateInfo.NewItem);
					break;
				case ChartCollectionOperation.UpdateItem:
					seriesRepository.UpdateRefinedSeries(updateInfo.OldIndex, updateInfo.OldItem, updateInfo.NewItem);
					break;
				case ChartCollectionOperation.RemoveItem:
					seriesRepository.RemoveSeries(updateInfo.OldItem);
					break;
				case ChartCollectionOperation.MoveItem:
					seriesRepository.MoveRefinedSeries(updateInfo.NewIndex, updateInfo.OldIndex);
					break;
				case ChartCollectionOperation.SwapItem:
					seriesRepository.SwapRefinedSeries(updateInfo.NewIndex, updateInfo.OldIndex);
					break;
			}
		}
		void EnsureSeriesGroups() {
			List<IAxisData> axesToRemove = new List<IAxisData>();
			foreach (var seriesGroup in RefinedSeriesGroups) {
				IAxisData axis = seriesGroup.Key.AxisData;
				if (axis != null && axis.IsDisposed)
					axesToRemove.Add(axis);
			}
			seriesRepository.GroupController.ClearGroups(axesToRemove);
		}
		void UpdateAxis(object sender, IAxisData axis, ChartCollectionOperation operation) {
			switch (operation) {
				case ChartCollectionOperation.Reset:
					EnsureSeriesGroups();
					break;
				case ChartCollectionOperation.InsertItem:
					seriesRepository.GroupController.InsertGroup(axis, InteractionManager);
					break;
				case ChartCollectionOperation.RemoveItem:
					seriesRepository.GroupController.RemoveGroup(axis);
					break;
				case ChartCollectionOperation.UpdateItem:
					break;
				case ChartCollectionOperation.MoveItem:
					break;
				case ChartCollectionOperation.SwapItem:
					break;
				default:
					break;
			}
		}
		void UpdateAxis(AxisCollectionUpdateInfo secondaryAxis) {
			UpdateAxis(secondaryAxis.Sender, secondaryAxis.NewItem, secondaryAxis.Operation);
			if (secondaryAxis.OldItem != null)
				UpdateAxis(secondaryAxis.Sender, secondaryAxis.OldItem, secondaryAxis.Operation);
		}
		void BatchUpdateAxis(AxisCollectionBatchUpdateInfo batchUpdateAxis) {
			foreach (IAxisData axis in batchUpdateAxis.NewItem)
				UpdateAxis(batchUpdateAxis.Sender, axis, batchUpdateAxis.Operation);
			if (batchUpdateAxis.OldItem != null)
				foreach (IAxisData axis in batchUpdateAxis.OldItem)
					UpdateAxis(batchUpdateAxis.Sender, axis, batchUpdateAxis.Operation);
		}
		void BatchUpdateRefinedSeriesList(SeriesCollectionBatchUpdateInfo batchSeriesUpdate) {
			switch (batchSeriesUpdate.Operation) {
				case ChartCollectionOperation.InsertItem:
					seriesRepository.InsertSeries(batchSeriesUpdate.NewIndex, batchSeriesUpdate.NewItem);
					break;
				case ChartCollectionOperation.RemoveItem:
					if ((batchSeriesUpdate.OldIndex < 0) || (batchSeriesUpdate.OldItem == null))
						break;
					seriesRepository.RemoveSeries(batchSeriesUpdate.OldItem);
					break;
				default:
					throw new NotSupportedException("The batch " + batchSeriesUpdate.Operation + " operation doesn't supported");
			}
		}
		void UpdateAxis(PropertyUpdateInfo<IAxisData> axisUpdateInfo) {
			if (axisUpdateInfo.NewValue == axisUpdateInfo.OldValue)
				return;
			RefinedSeries series = seriesRepository.FindRefinedSeries(x => { return x.Series.SeriesView != null && x.Series.SeriesView == axisUpdateInfo.Sender; });
			seriesRepository.RemoveFromSeriesGroups(series);
		}
		void UpdateScaleMap(IAxisData axis) {
			if (axis == null || axis.AxisScaleTypeMap == null)
				return;
			if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.Qualitative) {
				seriesRepository.RemoveSeriesGroup(axis);
			}
		}
		RefinedSeries ReprocessPoints(ISeries series) {
			var refinedSeries = seriesRepository.FindRefinedSeries(series);
			if (refinedSeries != null) {
				refinedSeries.UpdateData();
				return refinedSeries;
			}
			return null;
		}
		ISeries GetBaseSeries() {
			foreach (RefinedSeries series in seriesRepository.RefinedSeries)
				if ((series.Series != null) && (series.Series.SeriesView != null) && series.Series.Visible) {
					if (!compatibleViewFormDiagram.HasValue ||
						((compatibleViewFormDiagram == series.Series.SeriesView.CompatibleViewType) &&
						(isDiagram3D == series.Series.SeriesView.Is3DView)))
						return series.Series;
				}
			return null;
		}
		void ProcessDiagramPropertyUpdate(PropertyUpdateInfo<IDiagram> updateInfo) {
			IDiagram newDiagram = updateInfo.NewValue as IDiagram;
			if (newDiagram != null)
				this.diagram = newDiagram;
		}
		List<ChartUpdate> UpdateSeries(PropertyUpdateInfo updateInfo, SeriesControllerUpdateSeriesActions actions) {
			List<ChartUpdate> result = new List<ChartUpdate>();
			RefinedSeries refinedSeries = seriesRepository.FindRefinedSeries(updateInfo.Sender as ISeriesBase);
			if ((actions & SeriesControllerUpdateSeriesActions.UpdateSortingPointMode) != 0) {
				PropertyUpdateInfo<SortMode> sortModeUpdate = updateInfo as PropertyUpdateInfo<SortMode>;
				refinedSeries.ProcessSortingPointModeUpdate(sortModeUpdate.OldValue, sortModeUpdate.NewValue, refinedSeries.Series.SeriesPointsSortingKey);
			}
			if ((actions & SeriesControllerUpdateSeriesActions.UpdateSortingPointKey) != 0) {
				PropertyUpdateInfo<SeriesPointKeyNative> sortKeyUpdate = updateInfo as PropertyUpdateInfo<SeriesPointKeyNative>;
				refinedSeries.ProcessSortingPointKeyUpdate(sortKeyUpdate.OldValue, sortKeyUpdate.NewValue, refinedSeries.Series.SeriesPointsSortingMode);
			}
			if ((actions & SeriesControllerUpdateSeriesActions.UpdateArgumentScale) != 0)
				refinedSeries.UpdateArgumentScale();
			if ((actions & SeriesControllerUpdateSeriesActions.RemoveFromGroup) != 0)
				seriesRepository.RemoveFromSeriesGroups(refinedSeries);
			if ((actions & SeriesControllerUpdateSeriesActions.UpdateRandomPoints) != 0)
				refinedSeries.UpdateRandomPoints();
			if ((actions & SeriesControllerUpdateSeriesActions.UpdateFilter) != 0)
				if (refinedSeries.UpdateFilters())
					result.Add(new SingleSeriesUpdate(refinedSeries, ChartUpdateType.UpdateScaleMap | ChartUpdateType.UpdateInteraction));
			return result;
		}
		List<ChartUpdate> ProcessSeriesPropertiesUpdate(PropertyUpdateInfo updateInfo) {
			List<ChartUpdate> result = new List<ChartUpdate>();
			SeriesControllerUpdateSeriesActions actions = SeriesControllerUpdateSeriesActions.NoAction;
			RefinedSeries refinedSeries = seriesRepository.FindRefinedSeries(updateInfo.Sender as ISeriesBase);
			if (refinedSeries != null) {
				if (updateInfo.Name == "StackedGroup")
					actions |= SeriesControllerUpdateSeriesActions.RemoveFromGroup;
				else if (updateInfo.Name == "ArgumentScaleType") {
					actions |= SeriesControllerUpdateSeriesActions.RemoveFromGroup | SeriesControllerUpdateSeriesActions.UpdateRandomPoints | SeriesControllerUpdateSeriesActions.UpdateArgumentScale;
					result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
				}
				else if ((updateInfo.Name == "ValueScaleType") || (updateInfo.Name == "View") || (updateInfo.Name == "Pane"))
					actions |= SeriesControllerUpdateSeriesActions.RemoveFromGroup | SeriesControllerUpdateSeriesActions.UpdateRandomPoints;
				else if (updateInfo.Name == "SortingMode") {
					actions |= SeriesControllerUpdateSeriesActions.UpdateSortingPointMode | SeriesControllerUpdateSeriesActions.RemoveFromGroup;
					result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
				}
				else if (updateInfo.Name == "SeriesPointKey")
					actions |= SeriesControllerUpdateSeriesActions.UpdateSortingPointKey | SeriesControllerUpdateSeriesActions.RemoveFromGroup;
				else if (updateInfo.Name == "PointsFilterOptions") {
					actions |= SeriesControllerUpdateSeriesActions.UpdateFilter;
					result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
				}
				else if (updateInfo.Name == "CheckedInLegend")
					result.Add(new ChartUpdate(ChartUpdateType.UpdateSeriesGroupsInteraction | ChartUpdateType.UpdateCrosshair));
				else if (updateInfo.Name == "Visible")
					result.Add(new ChartUpdate(ChartUpdateType.UpdateSeriesGroupsInteraction | ChartUpdateType.UpdateCrosshair));
				else if (updateInfo.Name == "SideBySideProperties")
					result.Add(new ChartUpdate(ChartUpdateType.UpdateSeriesGroupsInteraction));
				else if (updateInfo.Sender is INestedDoughnutSeriesView)
					ProcessNestedDonutPropertiesUpdate(result, updateInfo);
			}
			if (updateInfo.Name == "View" || updateInfo.Name == "Pane")
				result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			result.AddRange(UpdateSeries(updateInfo, actions));
			return result;
		}
		void ProcessNestedDonutPropertiesUpdate(List<ChartUpdate> updates, PropertyUpdateInfo updateInfo) {
			switch (updateInfo.Name) {
				case "Group":
				case "InnerIndent":
				case "Weight":
					updates.Add(new ChartUpdate(ChartUpdateType.UpdateSeriesGroupsInteraction));
					break;
				case "HoleRadius":
				case "ExplodedDistance":
					var nestedDonut = (INestedDoughnutSeriesView)updateInfo.Sender;
					if (nestedDonut.IsOutside.HasValue && nestedDonut.IsOutside.Value)
						updates.Add(new ChartUpdate(ChartUpdateType.UpdateSeriesGroupsInteraction));
					break;
			}
		}
		List<ChartUpdate> ProcessAxisPropertiesUpdate(PropertyUpdateInfo updateInfo) {
			string property = updateInfo.Name;
			List<ChartUpdate> result = new List<ChartUpdate>();
			IAxisData axisData = (IAxisData)updateInfo.Sender;
			RefinedSeriesGroup seriesGroup = seriesRepository.GroupController.FindRefinedSeriesGroup(axisData);
			if (seriesGroup != null) {
				if (property == "WorkdaysOnly")
					result.AddRange(ProcessDataAggregationUpdate(axisData));
				if ((property == "WorkdaysOnly") || (property == "Workdays") || (property == "Holidays") || (property == "ExactWorkdays")) {
					List<RefinedSeries> updatedSeries = new List<RefinedSeries>();
					foreach (RefinedSeries series in seriesGroup.RefinedSeries)
						if (series.UpdateFilters())
							updatedSeries.Add(series);
					if (updatedSeries.Count > 0)
						result.Add(new BatchSeriesUpdate(updatedSeries, ChartUpdateType.UpdateScaleMap | ChartUpdateType.UpdateInteraction));
				}
				if (property == "QualitativeScaleComparer")
					result.Add(new ScaleSortingUpdate(seriesGroup, ChartUpdateType.UpdateScaleMap | ChartUpdateType.UpdateInteraction | ChartUpdateType.UpdateCrosshair));
			}
			if (property == "GridAlignment")
				result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			return result;
		}
		List<ChartUpdate> ProcessSeriesPointPropertiesUpdate(PropertyUpdateInfo updateInfo) {
			List<ChartUpdate> result = new List<ChartUpdate>();
			PropertyUpdateInfo<ISeries, ISeriesPoint> seriesPointUpdateInfo = updateInfo as PropertyUpdateInfo<ISeries, ISeriesPoint>;
			if (seriesPointUpdateInfo != null) {
				RefinedSeries refinedSeries = seriesRepository.FindRefinedSeries(seriesPointUpdateInfo.Owner);
				if (refinedSeries != null) {
					ChartUpdate pointUpdateResult = refinedSeries.UpdatePoint(seriesPointUpdateInfo);
					if (pointUpdateResult != null && refinedSeries.IsActive) {
						if (pointUpdateResult.ShouldUpdateScaleMap)
							seriesRepository.RemoveFromSeriesGroups(refinedSeries);
						else
							result.Add(pointUpdateResult);
					}
				}
			}
			return result;
		}
		List<ChartUpdate> ProcessDataAggregationUpdate(IAxisData axis) {
			List<ChartUpdate> results = new List<ChartUpdate>();
			RefinedSeriesGroup seriesGroup = seriesRepository.GroupController.FindRefinedSeriesGroup(axis);
			if (seriesGroup != null) {
				PointsAggregationUpdate updateResult = new PointsAggregationUpdate(axis, seriesGroup, seriesGroup.RefinedSeries, ChartUpdateType.UpdateInteraction);
				results.Add(updateResult);
				results.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
				foreach (RefinedSeries series in seriesGroup.RefinedSeries)
					series.UpdateRandomPoints();
			}
			return results;
		}
		List<ChartUpdate> ProcessAxisChangeUpdate(PropertyUpdateInfo<IAxisData> axisUpdateInfo) {
			List<ChartUpdate> result = new List<ChartUpdate>();
			if (axisUpdateInfo.Sender is ISeriesView) {
				UpdateAxis(axisUpdateInfo);
				if (axisUpdateInfo.OldValue != null)
					UpdateScaleMap(axisUpdateInfo.OldValue);
				if (axisUpdateInfo.NewValue != null)
					UpdateScaleMap(axisUpdateInfo.NewValue);
				result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			}
			if (axisUpdateInfo.Sender is IDiagram)
				ProcessPrimaryAxisUpdate(axisUpdateInfo.OldValue);
			return result;
		}
		IList<ChartUpdate> ProcessPropertyUpdate(PropertyUpdateInfo updateInfo) {
			List<ChartUpdate> result = new List<ChartUpdate>();
			if (updateInfo.Sender is ISeriesBase)
				result.AddRange(ProcessSeriesPropertiesUpdate(updateInfo));
			if (updateInfo.Sender is IAxisData)
				result.AddRange(ProcessAxisPropertiesUpdate(updateInfo));
			if (updateInfo.Sender is ISeriesPoint)
				result.AddRange(ProcessSeriesPointPropertiesUpdate(updateInfo));
			if (updateInfo.Sender is ICrosshairOptions)
				if (updateInfo.Name == "SnapMode")
					result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			if (updateInfo is PropertyUpdateInfo<IAxisData>)
				result.AddRange(ProcessAxisChangeUpdate((PropertyUpdateInfo<IAxisData>)updateInfo));
			if (updateInfo is AxisElementUpdateInfo) {
				UpdateScaleMap(((AxisElementUpdateInfo)updateInfo).Axis);
				result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			}
			if (updateInfo is PropertyUpdateInfo<IDiagram>) {
				PropertyUpdateInfo<IDiagram> diagramUpdateInfo = updateInfo as PropertyUpdateInfo<IDiagram>;
				ProcessDiagramPropertyUpdate(diagramUpdateInfo);
				if (diagramUpdateInfo.NewValue is IXYDiagram)
					result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			}
			if (updateInfo.Name == "DataSource")
				result.Add(new ChartUpdate(ChartUpdateType.DeserializeRange));
			if (updateInfo.Name == "CrosshairEnabled")
				result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			return result;
		}
		void ProcessPrimaryAxisUpdate(IAxisData oldAxis) {
			seriesRepository.RemoveSeriesGroup(oldAxis);
		}
		IList<ChartUpdate> ProcessCollectionUpdate(ChartUpdateInfoBase updateInfo) {
			List<ChartUpdate> result = new List<ChartUpdate>();
			if (updateInfo is CollectionUpdateInfo)
				result.Add(new ChartUpdate(ChartUpdateType.UpdateCrosshair));
			SeriesCollectionBatchUpdateInfo batchSeriesUpdate = updateInfo as SeriesCollectionBatchUpdateInfo;
			if (batchSeriesUpdate != null)
				BatchUpdateRefinedSeriesList(batchSeriesUpdate);
			SeriesCollectionUpdateInfo seriesUpdate = updateInfo as SeriesCollectionUpdateInfo;
			if (seriesUpdate != null)
				UpdateRefinedSeriesList(seriesUpdate);
			AxisCollectionUpdateInfo updateAxis = updateInfo as AxisCollectionUpdateInfo;
			if (updateAxis != null)
				UpdateAxis(updateAxis);
			AxisCollectionBatchUpdateInfo batchUpdateAxis = updateInfo as AxisCollectionBatchUpdateInfo;
			if (batchUpdateAxis != null)
				BatchUpdateAxis(batchUpdateAxis);
			if ((updateInfo is SeriesPointCollectionUpdateInfo) || (updateInfo is SeriesPointCollectionBatchUpdateInfo)) {
				CollectionUpdateInfo pointsUpdateInfo = (CollectionUpdateInfo)updateInfo;
				ISeries series;
				if (updateInfo is SeriesPointCollectionUpdateInfo)
					series = ((SeriesPointCollectionUpdateInfo)updateInfo).Series;
				else
					series = ((SeriesPointCollectionBatchUpdateInfo)updateInfo).Series;
				RefinedSeries refinedSeries = seriesRepository.FindRefinedSeries(series);
				if (refinedSeries != null) {
					ChartUpdate updateResult = refinedSeries.UpdatePoints(pointsUpdateInfo);
					if (updateResult != null && refinedSeries.IsActive) {
						if (updateResult.ShouldUpdateScaleMap)
							seriesRepository.RemoveFromSeriesGroups(refinedSeries);
						else
							result.Add(updateResult);
					}
				}
			}
			return result;
		}
		RefinedSeriesIncompatibilityCalculator CreateIncompatibilityCalculator(SeriesIncompatibilityStatistics incompatibilityStatistics) {
			incompatibilityStatistics.Clear();
			RefinedSeriesIncompatibilityCalculator incompatibilityCalculator = new RefinedSeriesIncompatibilityCalculator(incompatibilityStatistics);
			ISeriesView baseView = null;
			ISeries baseSeries = null;
			if (dataContainer.ShouldUseSeriesTemplate) {
				baseView = dataContainer.SeriesTemplate.SeriesView;
				IXYSeriesView xySeriesView = dataContainer.SeriesTemplate.SeriesView as IXYSeriesView;
				if (xySeriesView != null)
					incompatibilityCalculator.AddTemplateView(xySeriesView, dataContainer.SeriesTemplate.ArgumentScaleType, dataContainer.SeriesTemplate.ValueScaleType);
			}
			else {
				baseSeries = GetBaseSeries();
				if (baseSeries != null)
					baseView = baseSeries.SeriesView;
			}
			incompatibilityCalculator.Initialize(baseSeries, baseView);
			return incompatibilityCalculator;
		}
		List<RefinedSeries> UpdateActiveRefinedSeries() {
			this.seriesRepository.ClearRefinedSeries();
			RefinedSeriesIncompatibilityCalculator incompatibilityCalculator = CreateIncompatibilityCalculator(incompatibilityStatistics);
			List<RefinedSeries> updatedSeries = new List<RefinedSeries>();
			this.seriesRepository.GroupController.ClearSeparatePaneIndicators();
			if (incompatibilityCalculator.CanCalculate) {
				foreach (RefinedSeries refinedSeries in seriesRepository.RefinedSeries) {
					bool isVisibleAndCompatible = incompatibilityCalculator.IsVisibleAndCompatible(refinedSeries);
					bool shouldBeDrawn = refinedSeries.Series.ShouldBeDrawnOnDiagram;
					seriesRepository.AddToActive(refinedSeries, isVisibleAndCompatible, shouldBeDrawn);
					if (TryUpdateGroupForSeries(refinedSeries, isVisibleAndCompatible && shouldBeDrawn)) {
						refinedSeries.InvalidateData();
						updatedSeries.Add(refinedSeries);
					}
					IXYSeriesView xyView = refinedSeries.SeriesView as IXYSeriesView;
					if (xyView != null)
						AddIndicatorsInRefinedSeriesGroup(xyView, isVisibleAndCompatible && shouldBeDrawn);
				}
				foreach (RefinedSeries refinedSeries in seriesRepository.RefinedSeries) {
					if (refinedSeries.NoArgumentScaleType)
						refinedSeries.SetArgumentScale(incompatibilityCalculator.GetAxisXMasterScaleType(refinedSeries));
				}
			}
			else {
				foreach (RefinedSeries refinedSeries in seriesRepository.RefinedSeries) {
					if (TryUpdateGroupForSeries(refinedSeries, false))
						updatedSeries.Add(refinedSeries);
				}
			}
			return updatedSeries;
		}
		void AddIndicatorsInRefinedSeriesGroup(IXYSeriesView xyView, bool shouldOwningSeriesBeDrawn) {
			List<IAffectsAxisRange> indicators = xyView.GetIndicatorsAffectRange();
			if (indicators == null)
				return;
			foreach (IAffectsAxisRange indicator in indicators) {
				var group = (RefinedSeriesGroupByValue)this.seriesRepository.GroupController.FindRefinedSeriesGroup(indicator.AxisYData);
				if (group == null) 
					group = (RefinedSeriesGroupByValue)this.seriesRepository.GroupController.InsertGroup(indicator.AxisYData, InteractionManager);
					group.AddIndicator(indicator);
			}
		}
		bool TryUpdateGroupForSeries(RefinedSeries series, bool shouldBeDrawn) {
			bool isSeriesActivityChanged = shouldBeDrawn ^ series.IsActive;
			if (isSeriesActivityChanged) {
				if (series.IsActive) {
					seriesRepository.RemoveFromSeriesGroups(series);
				}
				else {
					RefinedSeriesGroup argumentGroup = seriesRepository.GroupController.GetArgumentGroup(series);
					RefinedSeriesGroup valueGroup = seriesRepository.GroupController.GetValueGroup(series, InteractionManager);
					if (argumentGroup != null)
						argumentGroup.AddSeries(series, shouldBeDrawn);
					if (valueGroup != null)
						valueGroup.AddSeries(series, shouldBeDrawn);
					series.BindToGroups(argumentGroup, valueGroup);
				}
				return true;
			}
			else {
				if (!shouldBeDrawn && !series.Series.ShouldBeDrawnOnDiagram) {
					RefinedSeriesGroup argumentGroup = seriesRepository.GroupController.GetArgumentGroup(series);
					RefinedSeriesGroup valueGroup = seriesRepository.GroupController.GetValueGroup(series, InteractionManager);
					if (argumentGroup != null)
						argumentGroup.AddSeries(series, false);
					if (valueGroup != null)
						valueGroup.AddSeries(series, false);
					return true;
				}
				return false;
			}
		}
		List<RefinedSeriesGroup> UpdateScaleMapForGroups(ChartUpdateAggregator updateAggregator, List<RefinedSeries> activityChangedSeries) {
			HashSet<RefinedSeriesGroup> changedGroups = new HashSet<RefinedSeriesGroup>();
			foreach (var series in activityChangedSeries) {
				changedGroups.Add(series.ArgumentGroup);
				changedGroups.Add(series.ValueGroup);
			}
			foreach (SingleSeriesUpdate update in updateAggregator.GetUpdates<SingleSeriesUpdate>()) {
				changedGroups.Add(update.RefinedSeries.ArgumentGroup);
				changedGroups.Add(update.RefinedSeries.ValueGroup);
			}
			foreach (ScaleSortingUpdate update in updateAggregator.GetUpdates<ScaleSortingUpdate>()) {
				changedGroups.Add(update.GroupToUpdate);
			}
			List<RefinedSeriesGroup> updatedGroups = new List<RefinedSeriesGroup>();
			foreach (var group in changedGroups) {
				if (group != null) {
					bool scaleMapUpdated = group.UpdateScaleMap(false);
					if (scaleMapUpdated) {
						updatedGroups.Add(group);
						if (group.GroupKey.AxisData != null && group.GroupKey.AxisData.WholeRange != null)
							ResetAxisRanges(group.GroupKey.AxisData);
					}
				}
			}
			return updatedGroups;
		}
		void ResetAxisRanges(IAxisData axisData) {
			if (!axisData.FixedRange) {
				if (!TryConvertRangeValues(axisData.WholeRange, axisData.AxisScaleTypeMap))
					axisData.WholeRange.Reset(false);
				if (!TryConvertRangeValues(axisData.VisualRange, axisData.AxisScaleTypeMap))
					axisData.VisualRange.Reset(false);
			}
		}
		bool TryConvertRangeValues(IAxisRangeData rangeData, AxisScaleTypeMap map) {
			double minInternal, maxInternal;
			object min, max;
			if (rangeData.CorrectionMode == RangeCorrectionMode.InternalValues) {
				return true;
			}
			else {
				bool possibleConvertNativeToInternalMin = map.TryNativeToInternal(rangeData.MinValue, out minInternal);
				if (rangeData.MinValue != null && !possibleConvertNativeToInternalMin)
					return false;
				bool possibleConvertNativeToInternalMax = map.TryNativeToInternal(rangeData.MaxValue, out maxInternal);
				if (rangeData.MaxValue != null && !possibleConvertNativeToInternalMax)
					return false;
			}
			min = rangeData.MinValue == null ? null : map.InternalToNative(minInternal);
			max = rangeData.MaxValue == null ? null : map.InternalToNative(maxInternal);
			rangeData.UpdateRange(min, max, double.NaN, double.NaN);
			return true;
		}
		bool ProcessPoints(List<RefinedSeriesGroup> scaleMapChangedGroups, List<RefinedSeries> activityChangedSeries) {
			List<RefinedSeries> processedSeries = new List<RefinedSeries>();
			if (scaleMapChangedGroups != null) {
				foreach (var group in scaleMapChangedGroups) {
					foreach (var series in group.RefinedSeries)
						if (!processedSeries.Contains(series)) {
							series.UpdateData();
							processedSeries.Add(series);
						}
				}
			}
			if (activityChangedSeries != null) {
				foreach (var series in activityChangedSeries) {
					if (!processedSeries.Contains(series)) {
						series.UpdateData();
						processedSeries.Add(series);
					}
				}
			}
			return processedSeries.Count > 0;
		}
		SeriesControllerChanges ProcessLightUpdate(LightUpdateInfo updateInfo) {
			RangeChangedUpdateInfo rangeUpdateInfo = updateInfo as RangeChangedUpdateInfo;
			if (rangeUpdateInfo != null) {
				seriesRepository.GroupController.CalculateFilteredIndexes();
				if (rangeUpdateInfo.IsArgumentAxis) {
					if (rangeUpdateInfo.Sender is IWholeAxisRangeData)
						return SeriesControllerChanges.WholeArgumentRangeUpdated;
					else if (rangeUpdateInfo.Sender is IVisualAxisRangeData)
						return SeriesControllerChanges.VisualArgumentRangeUpdated;
				}
			}
			return SeriesControllerChanges.NoChanges;
		}
		IEnumerable<IAxisData> GetAllAxes(IXYDiagram diagram) {
			if (diagram == null)
				return null;
			List<IAxisData> axes = new List<IAxisData>();
			if (diagram.AxisX != null)
				axes.Add((IAxisData)diagram.AxisX);
			if (diagram.AxisY != null)
				axes.Add((IAxisData)diagram.AxisY);
			if (diagram.SecondaryAxesX != null)
				foreach (var axis in diagram.SecondaryAxesX)
					axes.Add((IAxisData)axis);
			if (diagram.SecondaryAxesY != null)
				foreach (var axis in diagram.SecondaryAxesY)
					axes.Add((IAxisData)axis);
			return axes;
		}
		void UpdateRange(ChartUpdateAggregator updateAggregator) {
			IEnumerable<IAxisData> axes = GetAllAxes(diagram as IXYDiagram);
			seriesRepository.GroupController.UpdateAxisData(axes, InteractionManager);
			if (axes != null) {
				foreach (IAxisData axisData in axes)
					axisData.UpdateUserValues();
				if (updateAggregator.ShouldDeserializeRange)
					foreach (IAxisData axisData in axes)
						axisData.Deserialize();
			}
			List<IAxisRangeData> rangesForReset = new List<IAxisRangeData>();
			foreach (RangeResetUpdate rangeReset in updateAggregator.GetUpdates<RangeResetUpdate>())
				rangesForReset.Add(rangeReset.Range);
			VisualRangeUpdateMode visualRangeUpdateMode = updateAggregator.HasUpdates<PointsAggregationUpdate>() ? VisualRangeUpdateMode.ProportionalFromWholeRange : VisualRangeUpdateMode.Default;
			RangeUpdateEnqueuer.Update(seriesRepository.GroupController, rangesForReset, visualRangeUpdateMode, diagram as IXYDiagram, SeriesForLegend);
			seriesRepository.GroupController.CalculateFilteredIndexes();
		}
		void UpdateCrosshairData(ChartUpdateAggregator updateAggregator) {
			if (diagram is IXYDiagram && updateAggregator.ShouldUpdateCrosshair)
				((IXYDiagram)diagram).UpdateCrosshairData((IList<RefinedSeries>)seriesRepository.GetActiveRefinedSeries());
		}
		void UpdatePointsAggregation(ChartUpdateAggregator updateAggregator) {
			foreach (PointsAggregationUpdate update in updateAggregator.GetUpdates<PointsAggregationUpdate>()) {
				if (update.Axis != null)
					update.Axis.UpdateAutoMeasureUnit();
				if (update.GroupToUpdate != null)
					update.GroupToUpdate.UpdateScaleMap(true);
			}
		}
		void UpdatePointsIndices(ChartUpdateAggregator updateAggregator) {
			if (updateAggregator.ShouldUpdatePointsIndices) {
				foreach (RefinedSeries series in seriesRepository.RefinedSeries) {
					series.UpdatePointsIndices();
				}
			}
		}
		void RecalculateSeriesArgumentScales(ChartUpdateAggregator updateAggregator) {
			if (updateAggregator.ShouldUpdateArgumentScale)
				foreach (RefinedSeries series in seriesRepository.RefinedSeries) {
					series.RecalculateArgumentScale();
				}
			else
				foreach (SingleSeriesUpdate update in updateAggregator.GetUpdates<SingleSeriesUpdate>()) {
					update.RefinedSeries.UpdateArgumentScale();
				}
		}
		SeriesControllerChanges ApplyUpdates(ChartUpdateAggregator updateAggregator, bool pointsUpdated, bool isContainsProcessedPointsBeforeUpdate) {
			SeriesControllerChanges changes = SeriesControllerChanges.RefinedDataUpdated;
			RecalculateSeriesArgumentScales(updateAggregator);
			List<RefinedSeries> activityChangedSeries = UpdateActiveRefinedSeries();
			List<RefinedSeriesGroup> scaleMapChangedGroups = UpdateScaleMapForGroups(updateAggregator, activityChangedSeries);
			UpdatePointsAggregation(updateAggregator);
			bool hasProcessedSeries = ProcessPoints(scaleMapChangedGroups, activityChangedSeries);
			UpdatePointsIndices(updateAggregator);
			bool interactionWasUpdated = seriesRepository.UpdateInteraction(isContainsProcessedPointsBeforeUpdate, updateAggregator, activityChangedSeries, scaleMapChangedGroups, dataContainer.DesignMode);
			seriesRepository.UpdateSeriesGroupsInteraction(updateAggregator, interactionWasUpdated);
			CalculateIndicators(); 
			UpdateRange(updateAggregator);
			UpdateCrosshairData(updateAggregator);
			if (ShouldUpdateSelectedItems(updateAggregator))
				changes |= SeriesControllerChanges.ShouldResetSelectedItems;
			if (diagram is IXYDiagram && hasProcessedSeries || pointsUpdated)
				changes |= SeriesControllerChanges.ShouldUpdateMeasureUnits;
			return changes;
		}
		void CalculateIndicators() {
			var xyDiagram = this.diagram as IIndicatorCalculator;
			if (xyDiagram != null)
				xyDiagram.CalculateIndicators(ActiveRefinedSeries);
		}
		bool ShouldUpdateSelectedItems(ChartUpdateAggregator updateAggregator) {
			foreach (var chartUpdate in updateAggregator.Updates) {
				if (chartUpdate.UpdateType == ChartUpdateType.ResetSelectedItems)
					return true;
			}
			return false;
		}
		public void RecreateQualitativeScaleMaps() {
			bool isContainsProcessedPointsBeforeUpdate = IsContainsProcessedPoints;
			List<RefinedSeriesGroup> changedGroups = seriesRepository.GroupController.UpdateQualitativeScaleMap();
			ProcessPoints(changedGroups, null);
			seriesRepository.UpdateInteraction(isContainsProcessedPointsBeforeUpdate, null, null, changedGroups, ((IRefinedSeriesContainer)seriesRepository).IsDesignMode);
			IsLastUpdateInDesignMode = dataContainer.DesignMode;
		}
		public void OpenTransaction(bool loading) {
			if (transaction == null)
				transaction = new SeriesControllerTransaction(this, loading);
		}
		public SeriesControllerChanges CommitTransaction() {
			if (transaction != null) {
				SeriesControllerChanges changes = transaction.Commit();
				transaction = null;
				return changes;
			}
			return SeriesControllerChanges.NoChanges;
		}
		public void ProcessUpdate(ChartUpdateInfoBase updateInfo) {
			ProcessUpdate(updateInfo, false);
		}
		public void ProcessUpdate(ChartUpdateInfoBase updateInfo, bool loading) {
			IsLastUpdateInDesignMode = dataContainer.DesignMode;
			if (transaction == null) {
				transaction = new SeriesControllerTransaction(this, loading);
				transaction.Process(updateInfo);
				transaction.Commit();
				transaction = null;
			}
			else
				transaction.Process(updateInfo);
		}
		public IMinMaxValues GetAxisRange(IAxisData axis) {
			RefinedSeriesGroup group = seriesRepository.GroupController.FindRefinedSeriesGroup(axis);
			return group != null ? (IMinMaxValues)group.GetMinMaxValuesFromSeries() : (IMinMaxValues)axis.WholeRange;
		}
		public IList<ISeries> GetSeriesByAxis(IAxisData axis) {
			IList<ISeries> seriesList = new List<ISeries>();
			RefinedSeriesGroupKey groupKey = null;
			foreach (RefinedSeriesGroupKey key in RefinedSeriesGroups.Keys) {
				if (key.AxisData == axis) {
					groupKey = key;
					break;
				}
			}
			if (groupKey != null) {
				RefinedSeriesGroup group = RefinedSeriesGroups[groupKey];
				foreach (IRefinedSeries series in group.RefinedSeries)
					seriesList.Add(series.Series);
			}
			return seriesList;
		}
	}
}
