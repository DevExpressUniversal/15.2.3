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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.Service;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardWin.Native {
	[
	DashboardItemDesigner(typeof(RangeFilterDashboardItemDesigner))
	]
	public class RangeFilterDashboardItemViewer : ChartDashboardItemViewerBase {
		readonly RangeFilterControlContainer rangeFilterContainer = new RangeFilterControlContainer();
		readonly RangeFilterRangeControl rangeControl;
		readonly Locker locker = new Locker();
		readonly ToolTipController toolTipController;
		public RangeFilterControlContainer RangeFilterContainer { get { return rangeFilterContainer; } }
		public RangeFilterRangeControl RangeControl { get { return rangeControl; } }
		ToolTipController ToolTipController { get { return toolTipController; } }
		DashboardRangeFilterControlViewer RangeFilterControlViewer { get { return (DashboardRangeFilterControlViewer)ControlViewer; } }
		protected override bool VisualInterractivitySupported { get { return false; } }
		public RangeFilterDashboardItemViewer() {
			this.toolTipController = new ToolTipController();
			this.rangeControl = new RangeFilterRangeControl(this.toolTipController);
			SubscribeControlEvents();
			rangeControl.RangeChanged += OnRangeChanged;
		}
		public void SetRange(object minimum, object maximum) {
			ServiceClient.SetRange(DashboardItemName, minimum, maximum);
		}
		public RangeFilterSelection GetCurrentRange(object minimum, object maximum) {
			if(minimum != null && maximum != null)
				return new RangeFilterSelection(minimum, maximum);
			if(SelectedValues != null)
				return new RangeFilterSelection(SelectedValues[0][0], SelectedValues[0][1]);
			return null;
		}
		void UpdateMinMaxValues() {
			if(!locker.IsLocked) {
				locker.Lock();
				try {					
					IList<object> selectionValue = SelectedValues != null && SelectedValues.Count > 0 ? SelectedValues[0] as IList<object> : null;
					if(selectionValue != null && selectionValue.Count == 2)
						RangeFilterControlViewer.UpdateMinMaxValues(selectionValue[0], selectionValue[1]);
				} 
				finally {
					locker.Unlock();
				}
			}
		}
		protected override void UpdateActionModel() {
			base.UpdateActionModel();
			UpdateMinMaxValues();
		}
		protected override void UpdateViewer() {
			if(!locker.IsLocked) {
				locker.Lock();
				try {
					RangeControl.DrawIgnoreUpdatesState = ViewModel.ShouldIgnoreUpdate;
					base.UpdateViewer();
				} finally {
					locker.Unlock();
				}
			}
			UpdateMinMaxValues();
		}
		void OnRangeChanged(object sender, RangeControlRangeEventArgs e) {
			if(!locker.IsLocked) {
				locker.Lock();
				try {
					object minimum = RangeFilterControlViewer.SelectedRangeValues[0];
					object maximum = RangeFilterControlViewer.SelectedRangeValues[1];
					SetRange(minimum, maximum);
				}
				finally {
					locker.Unlock();
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				toolTipController.Dispose();
				rangeControl.Dispose();
				UnsubscribeControlEvents();
				rangeFilterContainer.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override Control GetViewControl() {
			base.GetViewControl();
			rangeControl.Dock = DockStyle.Fill;
			rangeControl.MouseDown += rangeFilterContainer.RaiseMouseDown;
			rangeControl.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			rangeFilterContainer.Controls.Add(rangeControl);
			return rangeFilterContainer;
		}
		protected override Control GetUnderlyingControl() {
			return null;
		}
		public override void OnLookAndFeelChanged() {
			Skin skin = DashboardSkins.GetSkin(LookAndFeel);
			rangeControl.BorderStyle = (skin != null && skin.Properties.GetBoolean("RangeFilterShowBorder", false)) ? BorderStyles.Default : BorderStyles.NoBorder;
		}
		protected override void InitializeControl() {
			UpdateMinMaxValues();
		}
		protected override DashboardChartControlViewerBase CreateControlViewer() {
			return new DashboardRangeFilterControlViewer(rangeControl, ChartControl);
		}
		protected override void UpdateViewModelInternal() {
			base.UpdateViewModelInternal();
			RangeFilterDashboardItemDesigner designer = DesignerProvider.GetDesigner<RangeFilterDashboardItemDesigner>();
			if (designer != null)
				designer.UpdateErrorMessage();
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(rangeControl);
		}
		protected override InteractivityController CreateInteractivityController() {
			RangeFilterInteractivityController controller = new RangeFilterInteractivityController(this);
			return (InteractivityController)controller;
		}
		void SubscribeControlEvents() {
			rangeControl.MouseClick += OnControlMouseClick;
			rangeControl.MouseDoubleClick += OnControlMouseDoubleClick;
			rangeControl.MouseMove += OnControlMouseMove;
			rangeControl.MouseEnter += OnControlMouseEnter;
			rangeControl.MouseLeave += OnControlMouseLeave;
			rangeControl.MouseDown += OnControlMouseDown;
			rangeControl.MouseUp += OnControlMouseUp;
			rangeControl.MouseHover += OnControlMouseHover;
			rangeControl.MouseWheel += OnControlMouseWheel;
		}
		void UnsubscribeControlEvents() {
			rangeControl.MouseClick -= OnControlMouseClick;
			rangeControl.MouseDoubleClick -= OnControlMouseDoubleClick;
			rangeControl.MouseMove -= OnControlMouseMove;
			rangeControl.MouseEnter -= OnControlMouseEnter;
			rangeControl.MouseLeave -= OnControlMouseLeave;
			rangeControl.MouseDown -= OnControlMouseDown;
			rangeControl.MouseUp -= OnControlMouseUp;
			rangeControl.MouseHover -= OnControlMouseHover;
			rangeControl.MouseWheel -= OnControlMouseWheel;
		}
		private void OnControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		private void OnControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		private void OnControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		private void OnControlMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		private void OnControlMouseLeave(object sender, EventArgs e) {
			RaiseMouseLeave();
		}
		private void OnControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		private void OnControlMouseMove(object sender, MouseEventArgs e) {
			RaiseMouseMove(e.Location);
		}
		private void OnControlMouseDoubleClick(object sender, MouseEventArgs e) {
			RaiseDoubleClick(e.Location);
		}
		private void OnControlMouseClick(object sender, MouseEventArgs e) {
			RaiseClick(e.Location);
		}
	}
}
