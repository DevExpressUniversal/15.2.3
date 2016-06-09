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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Data.Browsing;
namespace DevExpress.XtraCharts.Native {
	public class ChartContainerAdapter {
		readonly IChartContainer container;
		public IChartDataProvider DataProvider { get { return container != null ? container.DataProvider : null; } }
		public IChartRenderProvider RenderProvider { get { return container != null ? container.RenderProvider : null; } }
		public IChartEventsProvider EventsProvider { get { return container != null ? container.EventsProvider : null; } }
		public IChartInteractionProvider InteractionProvider { get { return container != null ? container.InteractionProvider : null; } }
		public IChartContainer Container { get { return container; } }
		public bool ShouldCustomDrawAxisLabels { get { return EventsProvider != null ? EventsProvider.ShouldCustomDrawAxisLabels : false; } }
		public bool ShouldCustomDrawSeriesPoints { get { return EventsProvider != null ? EventsProvider.ShouldCustomDrawSeriesPoints : false; } }
		public bool ShouldCustomDrawSeries { get { return EventsProvider != null ? EventsProvider.ShouldCustomDrawSeries : false; } }
		public bool HasCustomDrawEventsListeners { get { return ShouldCustomDrawAxisLabels || ShouldCustomDrawSeriesPoints || ShouldCustomDrawSeries; } }
		public bool SelectionEnabled { get { return InteractionProvider != null ? InteractionProvider.SelectionMode != ElementSelectionMode.None : false; } }
		public bool HitTestingEnabled { get { return InteractionProvider != null ? InteractionProvider.HitTestingEnabled : false; } }
		public bool CanShowTooltips { get { return InteractionProvider != null ? InteractionProvider.CanShowTooltips : false; } }
		public bool EnableChartHitTesting { get { return InteractionProvider != null ? InteractionProvider.EnableChartHitTesting : false; } }
		public bool SeriesPointSelectionEnabled { get { return InteractionProvider != null ? InteractionProvider.SeriesSelectionMode != SeriesSelectionMode.Series : false; } }
		public bool DragCtrlKeyRequired { get { return InteractionProvider != null ? InteractionProvider.DragCtrlKeyRequired : false; } }
		public DataContext DataContext { get { return DataProvider != null ? DataProvider.DataContext : null; } }
		public bool AreEventsEnabled { get; set; }
		public bool Capture {
			get { return InteractionProvider != null ? InteractionProvider.Capture : false; }
			set {
				if (InteractionProvider != null)
					InteractionProvider.Capture = value;
			}
		}
		public Rectangle DisplayBounds { get { return RenderProvider != null ? RenderProvider.DisplayBounds : Rectangle.Empty; } }
		public ChartContainerAdapter(IChartContainer container) {
			this.container = container;
			this.AreEventsEnabled = true;
		}
		public void Invalidate() {
			if (RenderProvider != null)
				RenderProvider.Invalidate();
		}
		public void InvokeInvalidate() {
			if (RenderProvider != null)
				RenderProvider.InvokeInvalidate();
		}
		public Bitmap LoadBitmap(string url) {
			return RenderProvider != null ? RenderProvider.LoadBitmap(url) : null;
		}
		public void OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnCustomDrawAxisLabel(e);
		}
		public void OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnCustomDrawSeries(e);
		}
		public void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnCustomDrawSeriesPoint(e);
		}
		public void OnCustomPaint(CustomPaintEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnCustomPaint(e);
		}
		public void OnCustomizeAutoBindingSettings(EventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnCustomizeAutoBindingSettings(e);
		}
		public void OnCustomizeSimpleDiagramLayout(CustomizeSimpleDiagramLayoutEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnCustomizeSimpleDiagramLayout(e);
		}
		public void OnPivotChartingCustomizeLegend(CustomizeLegendEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnPivotChartingCustomizeLegend(e);
		}
		public void OnPivotChartingCustomizeResolveOverlappingMode(CustomizeResolveOverlappingModeEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnPivotChartingCustomizeResolveOverlappingMode(e);
		}
		public void OnPivotChartingCustomizeXAxisLabels(CustomizeXAxisLabelsEventArgs e) {
			if ((EventsProvider != null) && AreEventsEnabled)
				EventsProvider.OnPivotChartingCustomizeXAxisLabels(e);
		}
		public void OnLegendItemChecked(LegendItemCheckedEventArgs e) {
			if ((InteractionProvider != null) && AreEventsEnabled)
				InteractionProvider.OnLegendItemChecked(e);
		}
		public void OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) {
			if ((InteractionProvider != null) && AreEventsEnabled)
				InteractionProvider.OnPieSeriesPointExploded(e);
		}
		public void OnObjectSelected(HotTrackEventArgs e) {
			if ((InteractionProvider != null) && AreEventsEnabled)
				InteractionProvider.OnObjectSelected(e);
		}
		public void OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			if ((InteractionProvider != null) && AreEventsEnabled)
				InteractionProvider.OnSelectedItemsChanged(e);
		}
		public void OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			if ((InteractionProvider != null) && AreEventsEnabled)
				InteractionProvider.OnCustomDrawCrosshair(e);
		}
		public void OnObjectHotTracked(HotTrackEventArgs e) {
			if ((InteractionProvider != null) && AreEventsEnabled)
				InteractionProvider.OnObjectHotTracked(e);
		}
		public void OnScroll(ChartScrollEventArgs e) {
			if (InteractionProvider != null)
				InteractionProvider.OnScroll(e);
		}
		public void OnZoom(ChartZoomEventArgs e) {
			if (InteractionProvider != null)
				InteractionProvider.OnZoom(e);
		}
		public void OnScroll3D(ChartScroll3DEventArgs e) {
			if (InteractionProvider != null)
				InteractionProvider.OnScroll3D(e);
		}
		public void OnZoom3D(ChartZoom3DEventArgs e) {
			if (InteractionProvider != null)
				InteractionProvider.OnZoom3D(e);
		}
		public Point PointToClient(Point p) {
			return InteractionProvider != null ? InteractionProvider.PointToClient(p) : p;
		}
		public Point PointToCanvas(Point p) {
			return InteractionProvider != null ? InteractionProvider.PointToCanvas(p) : p;
		}
	}
}
