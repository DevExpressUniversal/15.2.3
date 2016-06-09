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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class PieDashboardItemViewer : ChartDashboardItemViewerBase, IWinContentProvider {
		ContentScrollableControl contentScrollableControl;
		PieDashboardItemViewModel PieViewModel { get { return (PieDashboardItemViewModel)ViewModel; } }
		DashboardPieControlViewer PieControlViewer { get { return (DashboardPieControlViewer)ControlViewer; } }
		IContentProvider IWinContentProvider.ContentProvider { get { return PieControlViewer; } }
		Control IWinContentProvider.ContentControl { get { return ChartControl; } }
		public ContentScrollableControl ContentScrollableControl { get { return contentScrollableControl; } }
		bool IsInteractivityBySeries { get { return InteractivityMode == ChartInteractivityMode.Series; } }
		protected override string ElementName { get { return DashboardLocalizer.GetString(DashboardStringId.ElementNamePies); } }
		event EventHandler<PaintEventArgs> painted;
		event EventHandler<PaintEventArgs> IWinContentProvider.Painted {
			add { painted = (EventHandler<PaintEventArgs>)Delegate.Combine(painted, value); }
			remove { painted = (EventHandler<PaintEventArgs>)Delegate.Remove(painted, value); }
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			IList sliceData = null;
			IList pieData = null;
			bool containsArgumentAxis = TargetAxes.Contains(DashboardDataAxisNames.ChartArgumentAxis);
			bool containsSeriesAxis = TargetAxes.Contains(DashboardDataAxisNames.ChartSeriesAxis);
			if(!onlyTargetAxes || containsArgumentAxis) 
				sliceData = GetSliceData(location, ChartControl.CalcHitInfo(location));
			if(!onlyTargetAxes || containsSeriesAxis) 
				pieData = GetPieData(location);
			DataPointInfo dataPoint = new DataPointInfo();
			IList<string> measures = GetMeasures(location);
			if(sliceData != null) {
				dataPoint.DimensionValues.Add(DashboardDataAxisNames.ChartArgumentAxis, sliceData);
			}
			if(pieData != null) {
				dataPoint.DimensionValues.Add(DashboardDataAxisNames.ChartSeriesAxis, pieData);
			}
			if(measures != null && measures.Count > 0)
				dataPoint.Measures.AddRange(measures);
			return dataPoint.DimensionValues.Count > 0 || dataPoint.Measures.Count > 0 ? dataPoint : null;
		}
		void RaisePainted(PaintEventArgs e) {
			if(painted != null)
				painted(this, e);
		}
		IList GetSliceData(Point location, ChartHitInfo hitInfo) {
			IList sliceValues = null;
			if(hitInfo != null && hitInfo.InSeries && hitInfo.SeriesPoint != null)
				sliceValues = GetArgumentValue(hitInfo);
			return sliceValues;
		}
		IList GetPieData(Point location) {
			IValuesProvider valuesProvider = contentScrollableControl.GetHitItem(location) as IValuesProvider;
			if(valuesProvider != null) {
				IList selectionValues = valuesProvider.SelectionValues;
				return selectionValues != null && selectionValues.Count > 0 ? selectionValues : null;
			}
			return null;
		}
		IList<string> GetMeasures(Point location) {
			PieDashboardItemViewModel viewModel = ViewModel as PieDashboardItemViewModel;
			if(viewModel != null && viewModel.ProvideValuesAsArguments) {
				ChartHitInfo hitInfo = ChartControl.CalcHitInfo(location);
				return GetMeasures(hitInfo);
			}
			IValuesProvider valuesProvider = contentScrollableControl.GetHitItem(location) as IValuesProvider;
			if(valuesProvider != null) {
				return new List<string> { valuesProvider.MeasureID };
			}
			return new List<string>();
		}
		protected override void HandleChartClick(MouseEventArgs e) {
		}
		protected override void HandleChartDoubleClick(MouseEventArgs e) {
		}
		protected override void HandleChartMouseMove(MouseEventArgs e) {
		}
		protected override void HandleChartMouseEnter() {
		}
		protected override void HandleChartMouseLeave() {
		}
		protected override void HandleChartMouseHover() {
		}
		protected override void HandleChartMouseUp(Point location) {
		}
		protected override void HandleChartMouseDown(Point location) {
		}
		protected override void HandleChartMouseWheel(Point location) {
		}
		void OnContentScrollableControlMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		void OnContentScrollableControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		void OnContentScrollableControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnContentScrollableControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		void OnContentScrollableControlMouseMove(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseMove(e);
			RaiseMouseMove(e.Location);
		}
		void OnContentScrollableControlMouseClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseClick(e);
			RaiseClick(e.Location);
		}
		void OnContentScrollableControlMouseDoubleClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseDoubleClick(e);
			RaiseDoubleClick(e.Location);
		}
		void OnContentScrollableControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnContentScrollableControlMouseLeave(object sender, EventArgs e) {
			OnDashboardItemViewerMouseLeave();
			RaiseMouseLeave();
		}
		void OnBeforeToolTipShow(object sender, ToolTipControllerShowEventArgs e) {
			ToolTipController controller = (ToolTipController)sender;
			SeriesPoint seriesPoint = (SeriesPoint)controller.ActiveObject;
			foreach(Series series in ChartControl.Series){
				if (series.Points.Contains(seriesPoint)) {
					e.ToolTip = PieControlViewer.GetFormattedLabel(series, seriesPoint, PieViewModel.TooltipContentType);
					return;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (contentScrollableControl != null) {
					contentScrollableControl.MouseClick -= OnContentScrollableControlMouseClick;
					contentScrollableControl.MouseDoubleClick -= OnContentScrollableControlMouseDoubleClick;
					contentScrollableControl.MouseMove -= OnContentScrollableControlMouseMove;
					contentScrollableControl.MouseEnter -= OnContentScrollableControlMouseEnter;
					contentScrollableControl.MouseLeave -= OnContentScrollableControlMouseLeave;
					contentScrollableControl.MouseDown -= OnContentScrollableControlMouseDown;
					contentScrollableControl.MouseUp -= OnContentScrollableControlMouseUp;
					contentScrollableControl.MouseHover -= OnContentScrollableControlMouseHover;
					contentScrollableControl.MouseWheel -= OnContentScrollableControlMouseWheel;
					contentScrollableControl.Dispose();
				}
				ToolTipController toolTipController = ChartControl.ToolTipController;
				if (toolTipController != null){
					toolTipController.BeforeShow -= OnBeforeToolTipShow;
					toolTipController.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		public override void OnLookAndFeelChanged() {
			Skin skin = CommonSkins.GetSkin(LookAndFeel);
			BackColor = skin.Colors.GetColor(CommonColors.Window);
		}
		protected override void InitializeControl() {
			base.InitializeControl();
			ToolTipController toolTipController = new ToolTipController();
			toolTipController.AppearanceTitle.Options.UseTextOptions = true;
			toolTipController.AppearanceTitle.TextOptions.HAlignment = HorzAlignment.Far;
			toolTipController.AppearanceTitle.TextOptions.VAlignment = VertAlignment.Top;
			toolTipController.AutoPopDelay = 90000;
			toolTipController.Rounded = true;
			toolTipController.ShowBeak = true;
			toolTipController.ToolTipLocation = ToolTipLocation.TopRight;
			toolTipController.BeforeShow += OnBeforeToolTipShow;
			ChartControl.ToolTipController = toolTipController;
			ChartControl.Painted += (sender, e) => RaisePainted(e);
		}
		protected override Control GetViewControl() {
			base.GetViewControl();
			contentScrollableControl = new ContentScrollableControl(this);
			contentScrollableControl.MouseClick += OnContentScrollableControlMouseClick;
			contentScrollableControl.MouseDoubleClick += OnContentScrollableControlMouseDoubleClick;
			contentScrollableControl.MouseMove += OnContentScrollableControlMouseMove;
			contentScrollableControl.MouseEnter += OnContentScrollableControlMouseEnter;
			contentScrollableControl.MouseLeave += OnContentScrollableControlMouseLeave;
			contentScrollableControl.MouseDown += OnContentScrollableControlMouseDown;
			contentScrollableControl.MouseUp += OnContentScrollableControlMouseUp;
			contentScrollableControl.MouseHover += OnContentScrollableControlMouseHover;
			contentScrollableControl.MouseWheel += OnContentScrollableControlMouseWheel;
			return contentScrollableControl;
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(contentScrollableControl);
			contentScrollableControl.PrepareScrollingState(state);
		}
		void UpdateTooltipEnabled(PieDashboardItemViewModel viewModel) {
			ChartControl.ToolTipEnabled = viewModel.TooltipContentType == PieValueType.None ? DefaultBoolean.False : DefaultBoolean.True;
		}
		protected override void UpdateViewModelInternal() {
			base.UpdateViewModelInternal();
			contentScrollableControl.Model.ContentArrangementOptions = ContentArrangementOptions.AlignCenter;
			contentScrollableControl.Model.InitializeContent(PieViewModel.ContentDescription);
			contentScrollableControl.ClearSelection();
			UpdateTooltipEnabled(PieViewModel);
		}
		protected override DashboardChartControlViewerBase CreateControlViewer() {
			return new DashboardPieControlViewer(ChartControl);
		}
		protected override DashboardChartControl CreateChartControl() {
			return new DashboardPieControl();
		}
		protected override void SetHighlight(List<AxisPointTuple> higtlight) {
			UpdateInteractivityMode();
			if(IsInteractivityBySeries) {
				contentScrollableControl.HighlightValues(GetDimestionValueByAxis(higtlight, TargetAxes));
			} else {
				base.SetHighlight(higtlight);
			}
		}
		protected override void SetSelection(List<AxisPointTuple> selectedDimension) {
			UpdateInteractivityMode();
			if(IsInteractivityBySeries) {
				contentScrollableControl.SelectValues(GetDimestionValueByAxis(selectedDimension, TargetAxes));
			} else {
				base.SetSelection(selectedDimension);
			}
		}
	}
}
