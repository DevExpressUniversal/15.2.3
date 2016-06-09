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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Data.Browsing;
using DevExpress.Utils.Commands;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts.ModelSupport {
	class ModelChartContainer : IChartContainer, IChartRenderProvider, IChartDataProvider, IChartInteractionProvider {
		Chart chart;
		public ModelChartContainer() {
		}
		internal void SetChart(Chart chart) {
			this.chart = chart;
		}
		#region IChartContainer implementation
		event EventHandler IChartContainer.EndLoading { add { } remove { } }
		Chart IChartContainer.Chart { get { return chart; } }
		ChartContainerType IChartContainer.ControlType { get { return ChartContainerType.WinControl; } }
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return null; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return this; } }
		bool IChartContainer.DesignMode { get { return false; } }
		bool IChartContainer.Loading { get { return false; } }
		bool IChartContainer.ShowDesignerHints { get { return false; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return false; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return null; } }
		ISite IChartContainer.Site { get { return null; } set { } }
		IComponent IChartContainer.Parent { get { return null; } }
		void IChartContainer.Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		void IChartContainer.Changing() { }
		void IChartContainer.Changed() { }
		void IChartContainer.LockChangeService() { }
		void IChartContainer.UnlockChangeService() { }
		void IChartContainer.ShowErrorMessage(string message, string title) { }
		void IChartContainer.RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) { }
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		#endregion
		#region ISupportBarsInteraction implementation
		void ISupportBarsInteraction.RaiseUIUpdated() { }
		#endregion
		#region IServiceProvider implementation
		object IServiceProvider.GetService(Type serviceType) {
			return null;
		}
		#endregion
		#region ICommandAwareControl<ChartCommandId> implementation
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { } remove { } }
		Utils.KeyboardHandler.CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() { }
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(Commands.ChartCommandId id) {
			return null;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() { }
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		#endregion
		#region IChartRenderProvider implementation
		Rectangle IChartRenderProvider.DisplayBounds { get { return Rectangle.Empty; } }
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		object IChartRenderProvider.LookAndFeel { get { return null; } }
		IPrintable IChartRenderProvider.Printable { get { return null; } }
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		void IChartRenderProvider.Invalidate() { }
		void IChartRenderProvider.InvokeInvalidate() { }
		Bitmap IChartRenderProvider.LoadBitmap(string url) {
			return null;
		}
		#endregion
		#region IChartDataProvider implementation
		event BoundDataChangedEventHandler IChartDataProvider.BoundDataChanged { add { } remove { } }
		bool IChartDataProvider.CanUseBoundPoints { get { return true; } }
		object IChartDataProvider.DataAdapter { get { return null; } set { } }
		DataContext IChartDataProvider.DataContext { get { return null; } }
		object IChartDataProvider.DataSource { get { return null; } set { } }
		bool IChartDataProvider.SeriesDataSourceVisible { get { return false; } }
		object IChartDataProvider.ParentDataSource { get { return null; } }
		void IChartDataProvider.OnBoundDataChanged(EventArgs e) { }
		void IChartDataProvider.OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) { }
		void IChartDataProvider.OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) { }
		bool IChartDataProvider.ShouldSerializeDataSource(object dataSource) {
			return false;
		}
		#endregion
		#region IChartInteractionProvider implementation
		event HotTrackEventHandler IChartInteractionProvider.ObjectHotTracked { add { } remove { } }
		event HotTrackEventHandler IChartInteractionProvider.ObjectSelected { add { } remove { } }
		event SelectedItemsChangedEventHandler IChartInteractionProvider.SelectedItemsChanged { add { } remove { } }
		bool IChartInteractionProvider.CanShowTooltips { get { return false; } }
		bool IChartInteractionProvider.Capture { get { return false; } set { } }
		bool IChartInteractionProvider.DragCtrlKeyRequired { get { return false; } }
		bool IChartInteractionProvider.EnableChartHitTesting { get { return true; } }
		bool IChartInteractionProvider.HitTestingEnabled { get { return true; } }
		ElementSelectionMode IChartInteractionProvider.SelectionMode { get { return ElementSelectionMode.Single; } }
		SeriesSelectionMode IChartInteractionProvider.SeriesSelectionMode { get { return SeriesSelectionMode.Series; } }
		void IChartInteractionProvider.OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) { }
		void IChartInteractionProvider.OnLegendItemChecked(LegendItemCheckedEventArgs e) { }
		void IChartInteractionProvider.OnObjectHotTracked(HotTrackEventArgs e) { }
		void IChartInteractionProvider.OnObjectSelected(HotTrackEventArgs e) { }
		void IChartInteractionProvider.OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) { }
		void IChartInteractionProvider.OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) { }
		void IChartInteractionProvider.OnQueryCursor(QueryCursorEventArgs e) { }
		void IChartInteractionProvider.OnScroll(ChartScrollEventArgs e) { }
		void IChartInteractionProvider.OnScroll3D(ChartScroll3DEventArgs e) { }
		void IChartInteractionProvider.OnZoom(ChartZoomEventArgs e) { }
		void IChartInteractionProvider.OnZoom3D(ChartZoom3DEventArgs e) { }
		Point IChartInteractionProvider.PointToClient(Point p) {
			return p;
		}
		Point IChartInteractionProvider.PointToCanvas(Point p) {
			return p;
		}
		#endregion
	}
}
