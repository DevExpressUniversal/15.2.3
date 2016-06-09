#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
using DevExpress.DashboardCommon.ViewerData;
namespace DevExpress.DashboardWin.Native {
	public abstract class ChartDashboardItemViewerBase : DataDashboardItemViewer, ISupportColoring {
		DashboardChartControl chartControl;
		DashboardChartControlViewerBase controlViewer;
		bool ShouldProcessInteractivity { get { return controlViewer.ShouldProcessInteractivity; } }
		protected ChartInteractivityMode InteractivityMode { get { return controlViewer.InteractivityMode; } set { controlViewer.InteractivityMode = value; } }
		public DashboardChartControlViewerBase ControlViewer { get { return controlViewer; } }
		public DashboardChartControl ChartControl { get { return chartControl; } }
		public event DashboardItemElementCustomColorEventHandler ElementCustomColor;
		protected IList GetArgumentValue(ChartHitInfo hitInfo) {
			return hitInfo != null ? controlViewer.GetArgumentUniqueValues(hitInfo.SeriesPoint) : null;
		}
		protected IList<string> GetMeasures(ChartHitInfo hitInfo) {
			Series series = hitInfo.Series as Series;
			if(series != null)
				return ControlViewer.GetValueDataMembers(series, hitInfo.SeriesPoint);
			return null;
		}
		public override IEnumerable<string> TargetAxisCore {
			get {
				ChartDashboardItemBaseViewModel viewModel = ViewModel as ChartDashboardItemBaseViewModel;
				IEnumerable<string> result = null;
				if(viewModel != null) {
					if(viewModel.SelectionMode == ChartSelectionModeViewModel.Argument)
						result = new[] { DashboardDataAxisNames.ChartArgumentAxis };
					if(viewModel.SelectionMode == ChartSelectionModeViewModel.Series)
						result = new[] { DashboardDataAxisNames.ChartSeriesAxis };
					if(viewModel.SelectionMode == ChartSelectionModeViewModel.Points)
						result = new[] { DashboardDataAxisNames.ChartArgumentAxis, DashboardDataAxisNames.ChartSeriesAxis };
				}
				return result ?? base.TargetAxisCore;
			}
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			ChartHitInfo hitInfo = chartControl.CalcHitInfo(location);
			bool containsArgumentAxis = TargetAxes.Contains(DashboardDataAxisNames.ChartArgumentAxis);
			bool containsSeriesAxis = TargetAxes.Contains(DashboardDataAxisNames.ChartSeriesAxis);
			if(hitInfo == null || !hitInfo.InSeries || hitInfo.InLegend)
				return null;
			if(onlyTargetAxes && containsArgumentAxis &&  hitInfo.SeriesPoint == null)
				return null;
			DataPointInfo dataPoint = new DataPointInfo();
			if(hitInfo.SeriesPoint != null && (!onlyTargetAxes || containsArgumentAxis)) {
				IList arguments = GetArgumentValue(hitInfo);
				if(arguments != null && arguments.Count > 0)
					dataPoint.DimensionValues.Add(DashboardDataAxisNames.ChartArgumentAxis, arguments);
			}
			if(!onlyTargetAxes || containsSeriesAxis) {
				IList seriesValues = controlViewer.GetSeriesUniqueValues(hitInfo.Series as Series);
				if(seriesValues != null && seriesValues.Count > 0)
					dataPoint.DimensionValues.Add(DashboardDataAxisNames.ChartSeriesAxis, seriesValues);
			}
			dataPoint.Measures.AddRange(GetMeasures(hitInfo));
			return dataPoint;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				chartControl.Dispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void HandleChartClick(MouseEventArgs e) {
			if(ShouldProcessInteractivity)
				OnDashboardItemViewerMouseClick(e);
			RaiseClick(e.Location);
		}
		protected virtual void HandleChartDoubleClick(MouseEventArgs e) {
			if(ShouldProcessInteractivity)
				OnDashboardItemViewerMouseDoubleClick(e);
			RaiseDoubleClick(e.Location);
		}
		protected virtual void HandleChartMouseMove(MouseEventArgs e) {
			if(ShouldProcessInteractivity)
				OnDashboardItemViewerMouseMove(e);
			RaiseMouseMove(e.Location);
		}
		protected virtual void HandleChartMouseLeave() {
			if(ShouldProcessInteractivity)
				OnDashboardItemViewerMouseLeave();
			RaiseMouseLeave();
		}
		protected virtual void HandleChartMouseEnter() {
			RaiseMouseEnter();
		}
		protected virtual void HandleChartMouseHover() {
			RaiseMouseHover();
		}
		protected virtual void HandleChartMouseUp(Point location) {
			RaiseMouseUp(location);
		}
		protected virtual void HandleChartMouseDown(Point location) {
			RaiseMouseDown(location);
		}
		protected virtual void HandleChartMouseWheel(Point location) {
			RaiseMouseWheel();
		}
		void OnChartMouseClick(MouseEventArgs e) {
			HandleChartClick(e);
		}
		void OnChartMouseDoubleClick(MouseEventArgs e) {
			HandleChartDoubleClick(e);
		}
		void OnChartMouseEnter() {
			HandleChartMouseEnter();
		}
		void OnChartMouseLeave() {
			HandleChartMouseLeave();
		}
		void OnChartMouseHover() {
			HandleChartMouseHover();
		}
		void OnChartMouseMove(MouseEventArgs e) {
			HandleChartMouseMove(e);
		}
		void OnChartMouseUp(Point location) {
			HandleChartMouseUp(location);
		}
		void OnChartMouseDown(Point location) {
			HandleChartMouseDown(location);
		}
		void OnChartMouseWheel(Point location) {
			HandleChartMouseWheel(location);
		}
		void OnChartControlKeyDown(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyDown(e);
		}
		void OnChartControlKeyUp(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyUp(e);
		}
		void OnChartControlLostFocus(object sender, EventArgs e) {
			OnDashboardItemViewerLostFocus();
		}
		protected abstract DashboardChartControlViewerBase CreateControlViewer();
		protected virtual void InitializeControl() {
			chartControl.CrosshairOptions.HighlightPoints = false;
			chartControl.RuntimeHitTesting = true;
			chartControl.MouseClick += (sender, e) => {
				OnChartMouseClick(e); 
			};
			chartControl.MouseDoubleClick += (sender, e) => {
				OnChartMouseDoubleClick(e);
			};
			chartControl.MouseMove += (sender, e) => {
				OnChartMouseMove(e);
			};
			chartControl.MouseEnter += (sender, e) => {
				OnChartMouseEnter();
			};
			chartControl.MouseLeave += (sender, e) => {
				OnChartMouseLeave();
			};
			chartControl.MouseUp += (sender, e) => {
				OnChartMouseUp(e.Location);
			};
			chartControl.MouseDown += (sender, e) => {
				OnChartMouseDown(e.Location);
			};
			chartControl.MouseWheel += (sender, e) => {
				OnChartMouseWheel(e.Location);
			};
			chartControl.MouseHover += (sender, e) => {
				OnChartMouseHover();
			};
			chartControl.KeyDown += OnChartControlKeyDown;
			chartControl.KeyUp += OnChartControlKeyUp;
			chartControl.LostFocus += OnChartControlLostFocus;
		}
		protected virtual DashboardChartControl CreateChartControl() {
			return new DashboardChartControl();
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();			
			if(ViewModel.ShouldIgnoreUpdate) {
				ChartControl.Invalidate();
				return;
			}
			DataDashboardItemDesigner designer = DesignerProvider.GetDesigner<DataDashboardItemDesigner>();
			if(designer != null) {
				controlViewer.Update(ViewModel, MultiDimensionalData, GetDrillDownState(), true);
				designer.SetDataReducedImage(controlViewer.IsDataReduced);
			}
			else
				controlViewer.Update(ViewModel, MultiDimensionalData, GetDrillDownState(), false);
			UpdateViewModelInternal();
		}
		protected override Control GetViewControl() {
			chartControl = CreateChartControl();
			controlViewer = CreateControlViewer();
			return chartControl;
		}
		protected override Control GetUnderlyingControl() {
			return chartControl;
		}
		protected override void PrepareViewControl() {
			base.PrepareViewControl();
			controlViewer.InitializeControl();
			controlViewer.ElementCustomColor += OnElementCustomColor;
			InitializeControl();
		}
		protected virtual void UpdateViewModelInternal() {
			ChartDashboardItemBaseViewModel chartViewModel = ViewModel as ChartDashboardItemBaseViewModel;
		}
		protected override void SetHighlight(List<AxisPointTuple> higtlight) {
			UpdateInteractivityMode();
			controlViewer.HighlightValues(higtlight);
		}
		protected override void SetSelection(List<AxisPointTuple> selection) {
			UpdateInteractivityMode();
			controlViewer.SelectValues(selection);
		}
		protected void UpdateInteractivityMode() {
			InteractivityMode = ChartInteractivityMode.None;		
			if(TargetAxes.Contains(DashboardDataAxisNames.ChartArgumentAxis)) InteractivityMode |= ChartInteractivityMode.Argument;
			if(TargetAxes.Contains(DashboardDataAxisNames.ChartSeriesAxis)) InteractivityMode |= ChartInteractivityMode.Series;
		}
		void OnElementCustomColor(object sender, ElementCustomColorEventArgs e) {
			if(ElementCustomColor != null) {
				DashboardItemElementCustomColorEventArgs eventArgs = new DashboardItemElementCustomColorEventArgs(DashboardItemName, MultiDimensionalData, e);
				ElementCustomColor(this, eventArgs);
			}
		}
	}
}
