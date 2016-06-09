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
using DevExpress.Charts.NotificationCenter;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraCharts.Native {
	public class ViewController : ChartElement, INotificationObserver {
		[Flags]
		enum ChartUpdateType {
			NonUpdate = 0,
			UpdateSeriesBinding = 1,
			UpdateSmartBindig = 2,
			UpdateRangeControl = 4,
			ThrowChanged = 8,
			ClearZoomCache = 16,
			ClearTextureCache = 32,
			ClearDiagramCache = 64,
			ThrowRangeControlOptionsChanged = 128,
			InvalidateRangeControl = 256,
			UpdateSelectionState = 512,
			UpdateSelectionStateForLegend = 1024
		}
		public static RefinedPoint FindRefinedPoint(Chart chart, ISeriesPoint seriesPoint, Series ownerSeries) {
			IRefinedSeries refinedSeries = chart.ViewController.FindRefinedSeries(ownerSeries);
			if (refinedSeries == null)
				return null;
			foreach (RefinedPoint refPoint in refinedSeries.Points)
				if (refPoint.SeriesPoint == seriesPoint)
					return refPoint;
			return null;
		}
		readonly NotificationCenter notificationCenter;
		readonly Chart chart;
		readonly SeriesController seriesController;
		bool endLoading = false;
		bool drawing = false;
		bool bindingInProcess = false;
		bool Drawing {
			get { return drawing; }
			set { drawing = value; }
		}
		DataContainer DataContainer {
			get { return chart.DataContainer; }
		}
		protected internal override NotificationCenter NotificationCenter {
			get {
				return notificationCenter;
			}
		}
		public bool HasDataToPresent { get { return seriesController.IsContainsProcessedSeries || DataContainer.ShouldUseSeriesTemplate; } }
		public bool HasProcessedPoints { get { return seriesController.IsContainsProcessedPoints; } }
		public bool HasProcessedNotEmptyPoints { get { return seriesController.IsContainsProcessedNotEmptyPoints; } }
		public bool IsDesignMode { get { return chart.Container.DesignMode; } }
		public int ActiveSeriesCount { get { return seriesController.ActiveRefinedSeries.Count; } }
		public SeriesIncompatibilityStatistics SeriesIncompatibilityStatistics { get { return seriesController.SeriesIncompatibilityStatistics; } }
		public IList<RefinedSeries> ActiveRefinedSeries { get { return seriesController.ActiveRefinedSeries; } }
		public IList<RefinedSeries> RefinedSeriesForLegend { get { return seriesController.SeriesForLegend; } }
		public ViewController(Chart chart) {
			this.chart = chart;
			this.notificationCenter = new NotificationCenter();
			this.seriesController = new SeriesController(DataContainer);
			RegisterObserver();
		}
		#region INotificationObserver
#if !SL
		[System.Diagnostics.DebuggerStepThrough]
#endif
		void INotificationObserver.Notify(Notification notification) {
			if (notification is ElementWillChangeNotification)
				Notify(notification as ElementWillChangeNotification);
			else if (notification is RangeDataBase.RangeChangedEventNotification)
				Notify(notification as RangeDataBase.RangeChangedEventNotification);
			else if (notification is DataContainer.BindingWasUpdatedNotification)
				Notify(notification as DataContainer.BindingWasUpdatedNotification);
			else if (notification is DataContainer.BindingWillUpdateNotification)
				Notify(notification as DataContainer.BindingWillUpdateNotification);
			else if (notification is ChartElement.ElementChangedNotification)
				Notify(notification as ChartElement.ElementChangedNotification);
		}
		#endregion
		#region Notifications
		void RegisterObserver() {
			notificationCenter.RegisterObserver(this, typeof(ElementWillChangeNotification));
			notificationCenter.RegisterObserver(this, typeof(RangeDataBase.RangeChangedEventNotification));
			notificationCenter.RegisterObserver(this, typeof(DataContainer.BindingWillUpdateNotification));
			notificationCenter.RegisterObserver(this, typeof(DataContainer.BindingWasUpdatedNotification));
			notificationCenter.RegisterObserver(this, typeof(ChartElement.ElementChangedNotification));
		}
		object sender = null;
		void Notify(ElementWillChangeNotification notification) {
			if (!chart.Loading && !chart.IsDisposed && !Drawing) {
				chart.Container.Changing();
				sender = notification.Sender;
			}
		}
		void Notify(DataContainer.BindingWillUpdateNotification notification) {
			NotificationCenter.DenyNotifications(typeof(ElementWillChangeNotification));
			if (notification.ShouldUpdateChart && !chart.Loading && !chart.IsDisposed && !Drawing)
				chart.Container.Changing();
			seriesController.OpenTransaction(Loading || endLoading);
			bindingInProcess = true;
		}
		void Notify(DataContainer.BindingWasUpdatedNotification notification) {
			ClearCaches(ChartUpdateType.ClearDiagramCache | ChartUpdateType.ClearZoomCache | ChartUpdateType.ClearTextureCache);
			UpdateDiagramType();
			IChartDataProvider dataProvider = chart.ContainerAdapter.DataProvider;
			if (dataProvider != null)
				dataProvider.OnBoundDataChanged(EventArgs.Empty);
			SeriesControllerChanges changes = seriesController.CommitTransaction();
			if ((changes & SeriesControllerChanges.RefinedDataUpdated) == SeriesControllerChanges.RefinedDataUpdated) {
				UpdateAccordingDataChanges(changes);
				Update(new OnLoadEndUpdateInfo(null));
			}
			if ((changes & SeriesControllerChanges.VisualArgumentRangeUpdated) != 0)
				chart.ActualRangeSupport.Invalidate(false);
			else if ((changes & SeriesControllerChanges.WholeArgumentRangeUpdated) != 0)
				chart.ActualRangeSupport.Invalidate(true);
			if ((changes & SeriesControllerChanges.ShouldResetSelectedItems) != 0)
				chart.SelectionController.ClearSelection(false);
			if (notification.ShouldUpdateChart && !chart.Loading && !chart.IsDisposed && !Drawing && (chart.Container != null))
				chart.Container.Changed();
			NotificationCenter.AllowNotifications(typeof(ElementWillChangeNotification));
			bindingInProcess = false;
		}
		void UpdateDiagramType() {
			notificationCenter.DenyNotifications(typeof(ChartElement.ElementChangedNotification));
			ChartUpdateInfoBase updateInfo = chart.EnsureDiagramType(DetectDiagramType());
			if (updateInfo != null)
				seriesController.ProcessUpdate(updateInfo, false);
			notificationCenter.AllowNotifications(typeof(ChartElement.ElementChangedNotification));
		}
		void Notify(ChartElement.ElementChangedNotification notification) {
			if (bindingInProcess)
				seriesController.ProcessUpdate(notification.UpdateInfo, false);
			else
				Update(notification.UpdateInfo);
		}
		void Notify(RangeDataBase.RangeChangedEventNotification notification) {
			notification.Range.RaiseRangeChanged(notification.OldRange, notification.NewRange);
		}
		#endregion
		Type DetectDiagramType() {
			if (DataContainer.ShouldUseSeriesTemplate)
				return DataContainer.SeriesTemplate.View.DiagramType;
			foreach (Series series in DataContainer.Series)
				if (series.Visible)
					return series.View.DiagramType;
			return null;
		}
		void UpdateSeriesTemplate() {
			if (chart.ShouldUseSeriesTemplate) {
				SeriesBase seriesTemplate = chart.DataContainer.SeriesTemplate;
				XYDiagram2DSeriesViewBase view = seriesTemplate.View as XYDiagram2DSeriesViewBase;
				if (view != null) {
					UpdateScaleType(view.ActualAxisX, (ActualScaleType)seriesTemplate.ActualArgumentScaleType);
					UpdateScaleType(view.ActualAxisY, (ActualScaleType)seriesTemplate.ValueScaleType);
				}
			}
		}
		void ClearCaches(ChartUpdateType updateType) {
			chart.ResetGraphicsCache();
			chart.InvalidateDrawingHelper();
			chart.ClearCache();
			chart.HitTestController.Clear();
			if ((updateType & ChartUpdateType.ClearZoomCache) != 0 && chart.Diagram != null)
				chart.Diagram.ClearZoomCache();
			if ((updateType & ChartUpdateType.ClearTextureCache) != 0)
				chart.TextureCache.Clear();
			if (((updateType & ChartUpdateType.ClearDiagramCache) != 0) && (chart.Diagram != null))
				chart.Diagram.ClearResolveOverlappingCache();
		}
		void RaiseRangeChangedEventsIfNecessary() {
			foreach (RefinedSeriesGroup group in seriesController.RefinedSeriesGroups.Values) {
				if (group.GroupKey.AxisData != null) {
					RangeDataBase visualRange = (group.GroupKey.AxisData.VisualRange) as RangeDataBase;
					RangeDataBase wholeRange = (group.GroupKey.AxisData.WholeRange) as RangeDataBase;
					if (visualRange != null)
						visualRange.RaiseChangedEventIfNecessary();
					if (wholeRange != null)
						wholeRange.RaiseChangedEventIfNecessary();
				}
			}
		}
		void UpdateScaleType(IAxisData axis, ActualScaleType scaleType) {
			if (axis.AxisScaleTypeMap.ScaleType != scaleType) {
				AxisScaleTypeMap scaleTypeMap;
				switch (scaleType) {
					case ActualScaleType.Numerical:
						if (axis.NumericScaleOptions.ScaleMode != ScaleModeNative.Continuous && !double.IsNaN(axis.NumericScaleOptions.MeasureUnit)) {
							scaleTypeMap = new AxisNumericalMap(axis.NumericScaleOptions.MeasureUnit);
							break;
						}
						scaleTypeMap = new AxisNumericalMap();
						break;
					case ActualScaleType.DateTime:
						scaleTypeMap = new AxisDateTimeMap(axis.DateTimeScaleOptions.MeasureUnit, axis.DateTimeScaleOptions.WorkdaysOptions);
						break;
					case ActualScaleType.Qualitative:
						scaleTypeMap = new AxisQualitativeMap(new List<object>());
						break;
					default:
						scaleTypeMap = new AxisNumericalMap();
						break;
				}
				axis.AxisScaleTypeMap = scaleTypeMap;
			}
		}
		ChartUpdateType ProcessUpdateInfo(ChartUpdateInfoBase updateInfo) {
			ChartUpdateType result = ChartUpdateType.NonUpdate;
			var chartElementUpdate = updateInfo as ChartElementUpdateInfo;
			if (chartElementUpdate != null) {
				result |= ChartUpdateType.ThrowChanged;
				if ((chartElementUpdate.Flags & ChartElementChange.KeepZoomCache) == 0)
					result |= ChartUpdateType.ClearZoomCache;
				if ((chartElementUpdate.Flags & ChartElementChange.ClearTextureCache) != 0)
					result |= ChartUpdateType.ClearTextureCache;
				if ((chartElementUpdate.Flags & ChartElementChange.DataBindingChanged) != 0)
					result |= ChartUpdateType.UpdateSeriesBinding | ChartUpdateType.InvalidateRangeControl;
				if ((chartElementUpdate.Flags & ChartElementChange.SmartBindingChanged) != 0)
					result |= ChartUpdateType.UpdateSmartBindig | ChartUpdateType.InvalidateRangeControl;
				if ((chartElementUpdate.Flags & ChartElementChange.RangeControlChanged) != 0)
					result |= ChartUpdateType.InvalidateRangeControl;
				result |= ChartUpdateType.ClearDiagramCache;
				if (chartElementUpdate.Sender is RangeControlOptions || chartElementUpdate.Sender is ChartRangeControlClientGridOptions)
					result |= ChartUpdateType.InvalidateRangeControl;
			}
			if (updateInfo is DataAggregationUpdate) {
				result |= ChartUpdateType.InvalidateRangeControl;
				result |= ChartUpdateType.UpdateSelectionState;
			}
			if (updateInfo is PropertyUpdateInfo) {
				PropertyUpdateInfo propertyUpdateInfo = updateInfo as PropertyUpdateInfo;
				if (propertyUpdateInfo.Name == Series.CheckedInLegendProperty)
					result |= ChartUpdateType.UpdateSelectionStateForLegend;
			}
			if ((updateInfo is SeriesCollectionUpdateInfo) ||
				(updateInfo is SeriesPointCollectionUpdateInfo) ||
				(updateInfo is SeriesGroupsInteractionUpdateInfo))
				result |= ChartUpdateType.ThrowChanged;
			return result;
		}
		void UpdateAccordingDataChanges(SeriesControllerChanges changes) {
			notificationCenter.DenyNotifications(typeof(ChartElement.ElementChangedNotification));
			UpdateDiagramType();
			if ((chart.Diagram != null) && !endLoading)
				chart.Diagram.Update(ActiveRefinedSeries);
			if (((changes & SeriesControllerChanges.ShouldUpdateMeasureUnits) != 0) && !endLoading) {
				XYDiagram2D xyDiagram = chart.Diagram as XYDiagram2D;
				if (xyDiagram != null) {
					IList<AxisBase> affectedAxes = xyDiagram.UpdateAutomaticMeasurement(false);
					SendDataAgreggationUpdates(affectedAxes);
					if ((chart.Diagram != null) && !endLoading)
						chart.Diagram.Update(ActiveRefinedSeries);
				}
			}
			notificationCenter.AllowNotifications(typeof(ChartElement.ElementChangedNotification));
		}
		void Update(ChartUpdateInfoBase updateInfo) {
			if (!Drawing && !chart.IsDisposed) {
				if (!chart.Loading && !endLoading) {
					notificationCenter.DenyNotifications(typeof(ElementWillChangeNotification));
					notificationCenter.DenyNotifications(typeof(ChartElement.ElementChangedNotification));
					ChartUpdateType updateType = ProcessUpdateInfo(updateInfo);
					ClearCaches(updateType);
					seriesController.OpenTransaction(chart.Loading);
					seriesController.ProcessUpdate(updateInfo, false);
					ChartUpdateInfoBase diagramUpdateInfo = chart.EnsureDiagramType(DetectDiagramType());
					if (diagramUpdateInfo != null)
						seriesController.ProcessUpdate(diagramUpdateInfo, false);
					if (chart.Diagram != null)
						UpdateSeriesTemplate();
					SeriesControllerChanges changes = seriesController.CommitTransaction();
					if ((changes & SeriesControllerChanges.RefinedDataUpdated) != 0)
						UpdateAccordingDataChanges(changes);
					if ((updateType & ChartUpdateType.ThrowChanged) != 0) {
						if (chart.Container != null)
							chart.Container.RaiseUIUpdated();
					}
					if ((updateType & ChartUpdateType.InvalidateRangeControl) != 0)
						chart.ActualRangeSupport.Invalidate(true);
					if ((changes & SeriesControllerChanges.VisualArgumentRangeUpdated) != 0)
						chart.ActualRangeSupport.Invalidate(false);
					else if ((changes & SeriesControllerChanges.WholeArgumentRangeUpdated) != 0)
						chart.ActualRangeSupport.Invalidate(true);
					if ((updateType & ChartUpdateType.UpdateSelectionState) != 0)
						chart.SelectionController.SyncSelectionStates();
					if ((updateType & ChartUpdateType.UpdateSelectionStateForLegend) != 0)
						chart.SelectionController.UpdateSelectedItemsForLegend();
					if ((changes & SeriesControllerChanges.ShouldResetSelectedItems) != 0)
						chart.SelectionController.ClearSelection(false);
					notificationCenter.AllowNotifications(typeof(ChartElement.ElementChangedNotification));
					notificationCenter.AllowNotifications(typeof(ElementWillChangeNotification));
					if (!(updateInfo is OnLoadEndUpdateInfo))
						chart.Container.Changed();
				}
				else
					seriesController.ProcessUpdate(updateInfo, true);
				RaiseRangeChangedEventsIfNecessary();
			}
		}
		protected override ChartElement CreateObjectForClone() {
			throw new NotImplementedException();
		}
		protected internal List<Series> GetActiveSeriesList() {
			List<Series> result = new List<Series>();
			foreach (IRefinedSeries refinedSeries in ActiveRefinedSeries)
				result.Add((Series)refinedSeries.Series);
			return result;
		}
		public void SendDataAgreggationUpdates(IList<AxisBase> axes) {
			seriesController.OpenTransaction(Loading);
			for (int i = 0; i < axes.Count; i++)
				seriesController.ProcessUpdate(new DataAggregationUpdate(axes[i]));
			seriesController.CommitTransaction();
			chart.ActualRangeSupport.Invalidate(true);
		}
		public List<ISeries> GetISeriesForLegendList() {
			List<ISeries> result = new List<ISeries>();
			foreach (IRefinedSeries refinedSeries in RefinedSeriesForLegend)
				result.Add(refinedSeries.Series);
			return result;
		}
		public void Update() {
			Update(null);
		}
		public void RenderChart(IRenderer renderer, Rectangle bounds, bool lockDrawingHelper, Graphics Graphics, Graphics GraphicsMiddle, Graphics GraphicsAbove) {
			Drawing = true;
			try {
				ChartViewData viewData = ChartViewData.Create(chart, (ZPlaneRectangle)bounds, false);
				if (viewData == null)
					return;
				using (viewData) {
					viewData.CalculateLayout(lockDrawingHelper);
					renderer.Reset(Graphics, bounds);
					viewData.Render(renderer);
					if (chart.ActualCrosshairHighlightPoints) {
						renderer.Present();
						renderer.Reset(GraphicsMiddle, bounds);
					}
					viewData.RenderMiddle(renderer);
					renderer.Present();
					renderer.Reset(GraphicsAbove, bounds);
					viewData.RenderAbove(renderer);
					renderer.Present();
				}
			}
			finally {
				Drawing = false;
			}
		}
		public GraphicsCommand CreateGraphicsCommand(ChartDrawingContext context, bool lockDrawingHelper) {
			Drawing = true;
			try {
				if (IsDesignMode != seriesController.IsLastUpdateInDesignMode)
					seriesController.RecreateQualitativeScaleMaps();
				ChartViewData viewData = ChartViewData.Create(chart, (ZPlaneRectangle)new Rectangle(new Point(), context.Viewport.Size), false);
				if (viewData == null)
					return null;
				using (viewData) {
					viewData.CalculateLayout(lockDrawingHelper);
					if (context.Viewport.Size.Width > 0 && context.Viewport.Size.Height > 0)
						return viewData.CreateGraphicsCommand(new Rectangle(new Point(), context.Viewport.Size));
					return null;
				}
			}
			finally {
				Drawing = false;
			}
		}
		public bool RefreshDataInternal(bool forceRefresh) {
			if (Drawing || chart.Loading)
				return false;
			if (!forceRefresh) {
				forceRefresh = DataContainer.ForceRefresh();
			}
			if (!forceRefresh)
				return false;
			seriesController.ProcessUpdate(new RefreshDataUpdateInfo(chart), Loading);
			DataContainer.UpdateBinding(true, true, false);
			return true;
		}
		public Series[] GetAllSeriesArray() {
			List<RefinedSeries> refinedSeries = seriesController.RefinedSeries;
			if (refinedSeries.Count != 0) {
				Series[] series = new Series[refinedSeries.Count];
				for (int i = 0; i < refinedSeries.Count; i++)
					series[i] = (Series)refinedSeries[i].Series;
				return series;
			}
			return new Series[0];
		}
		public IList<ISeries> GetSeriesByAxis(AxisBase axis) {
			return seriesController.GetSeriesByAxis(axis);
		}
		public IRefinedSeries FindRefinedSeries(Series series) {
			foreach (RefinedSeries refinedSeries in seriesController.ActiveRefinedSeries)
				if (object.ReferenceEquals(refinedSeries.Series, series))
					return refinedSeries;
			return null;
		}
		public void StartOnEndLoading() {
			endLoading = true;
		}
		public void OnEndLoading() {
			endLoading = false;
			Update(new OnLoadEndUpdateInfo(chart));
		}
		public void ResetLoadingFlag() {
			endLoading = false;
		}
		public IMinMaxValues GetAxisRange(AxisBase axis) {
			return seriesController.GetAxisRange(axis);
		}
	}
}
