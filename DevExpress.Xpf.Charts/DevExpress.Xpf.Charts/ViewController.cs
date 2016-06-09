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
using DevExpress.Xpf.Core;
using System.Collections.Generic;
namespace DevExpress.Xpf.Charts.Native {
	public class ViewController : IChartDataContainer {
		readonly Diagram diagram;
		readonly Locker changedLocker = new Locker();
		readonly SeriesController seriesController;
		readonly List<IRefinedSeries> activeRefinedSeries = new List<IRefinedSeries>();
		public static RefinedPoint FindRefinedPoint(ISeriesPoint seriesPoint, Series ownerSeries) {
			Diagram diagram = ((IChartElement)ownerSeries).Owner as Diagram;
			IRefinedSeries refinedSeries = diagram.ViewController.GetRefinedSeries(ownerSeries);
			if (refinedSeries == null)
				return null;
			foreach (RefinedPoint refPoint in refinedSeries.Points)
				if (refPoint.SeriesPoint == seriesPoint)
					return refPoint;
			return null;
		}
		readonly Dictionary<ISeries, IRefinedSeries> refinedSeriesContainer = new Dictionary<ISeries, IRefinedSeries>();
		bool dataInUpdating = false;
		bool animationSuspended = false;
		ChartControl Chart { get { return diagram.ChartControl; } }
		public bool Loading { get { return Chart != null ? Chart.Loading : false; } }
		public IList<IRefinedSeries> ActiveRefinedSeries { get { return activeRefinedSeries; } }
		public IList<RefinedSeries> RefinedSeriesForLegend { get { return seriesController.SeriesForLegend; } }
		public SeriesController SeriesController { get { return seriesController; } }
		public ViewController(Diagram diagram) {
			this.diagram = diagram;
			this.seriesController = new SeriesController(this, diagram.CompatibleViewType, diagram is Diagram3D, diagram);		   
		}
		#region IChartDataContainer
		bool IChartDataContainer.DesignMode {
			get { return false; }
		}
		ISeriesBase IChartDataContainer.SeriesTemplate {
			get {
				return diagram.SeriesTemplate;
			}
		}
		bool IChartDataContainer.ShouldUseSeriesTemplate {
			get {
				Series seriesTemplate = diagram.SeriesTemplate;
				return seriesTemplate != null && seriesTemplate.Visible && !string.IsNullOrEmpty(diagram.SeriesDataMember);
			}
		}
		#endregion
		void UpdateRefinedSeriesContainer() {
			refinedSeriesContainer.Clear();
			activeRefinedSeries.Clear();
			foreach (RefinedSeries refinedSeries in seriesController.SeriesForLegend) {
				if (seriesController.ActiveRefinedSeries.Contains(refinedSeries))
					activeRefinedSeries.Add(refinedSeries);
				refinedSeriesContainer.Add(refinedSeries.Series, refinedSeries);
			}
		}
		ChartUpdateInfoBase MakeSeriesControllerUpdate(ChartUpdate update) {
			if ((update.ShouldUpdateSeriesBinding) && (update.UpdateInfo.Sender is ISeries))
				return new SeriesBindingUpdate((ISeries)update.UpdateInfo.Sender);
			return update.UpdateInfo;
		}
		void ProcessChanged(ChartUpdate update) {
			seriesController.ProcessUpdate(MakeSeriesControllerUpdate(update));
			SeriesControllerChanges changes = seriesController.CommitTransaction();
			UpdateRefinedSeriesContainer();
			if ((changes & SeriesControllerChanges.RefinedDataUpdated) > 0) {
				diagram.DisableCache();
				diagram.UpdateActualPanes((update.Change & ChartElementChange.UpdateActualPanes) > 0);
				if ((update.Change & ChartElementChange.ClearDiagramCache) > 0)
					diagram.ClearCache();
				if ((update.Change & ChartElementChange.UpdateAutoSeries) > 0 || (update.Change & ChartElementChange.UpdateAutoSeriesProperties) > 0
				   || (update.Change & ChartElementChange.UpdateChartBinding) > 0 || (update.Change & ChartElementChange.UpdateIndicators) > 0)
					diagram.UpdateLogicalElements();
				if (((changes & SeriesControllerChanges.ShouldUpdateMeasureUnits) != 0))
					diagram.CheckMeasureUnits();
				if (!Loading)
					diagram.Update((update.Change & ChartElementChange.UpdatePanesItems) > 0, (update.Change & ChartElementChange.UpdateXYDiagram2DItems) > 0);
				if (Chart != null)
					Chart.InvalidateChartElementPanel();
				diagram.InvalidateDiagram();
			}
			else if (!Loading) {
				diagram.UpdateSeriesItems();
				diagram.EnableCache();
			}
			if (animationSuspended)
				Animate();
		}
		public void SendDataAgreggationUpdates(IList<AxisBase> axes) {
			if (axes != null && axes.Count > 0) {
				seriesController.OpenTransaction(Loading);
				for (int i = 0; i < axes.Count; i++)
					seriesController.ProcessUpdate(new DataAggregationUpdate(axes[i]));
				seriesController.CommitTransaction();
			}
		}
		public void Animate() {
			if (!dataInUpdating) {
				animationSuspended = changedLocker.IsLocked;
				if (diagram != null && !animationSuspended)
					foreach (Series series in diagram.Series)
						series.Animate();
			}
		}
		public IRefinedSeries GetRefinedSeries(ISeries series) {
			IRefinedSeries result;
			if (refinedSeriesContainer.TryGetValue(series, out result))
				return result;
			return null;
		}
		public IList<ISeries> GetSeriesByAxis(AxisBase axis) {
			return seriesController.GetSeriesByAxis(axis);
		}
		public void BeginUpdateData() {
			dataInUpdating = true;
			seriesController.OpenTransaction(Loading);
		}
		public void EndUpdateData() {
			ProcessChanged(new ChartUpdate(diagram, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateActualPanes | ChartElementChange.UpdateXYDiagram2DItems));
			dataInUpdating = false;
			if (Chart != null && Chart.AnimationMode == ChartAnimationMode.OnDataChanged)
				Animate();
		}
		public void Notify(ChartUpdate update) {
			if (dataInUpdating || Loading)
				seriesController.ProcessUpdate(update.UpdateInfo, Loading);
			else if (!changedLocker.IsLocked) {
				try {
					changedLocker.Lock();
					seriesController.OpenTransaction(false);
					ProcessChanged(update);
				}
				finally {
					changedLocker.Unlock();
				}
			}
		}
	}
}
