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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public abstract class FilterElementDashboardItemViewer : DataDashboardItemViewer {
		readonly FilterElementDashboardItemViewControl viewControl;
		protected abstract IFilterElementControl ElementControl { get; }
		DashboardFilterElementControl DashboardFilterElementControl { get { return (DashboardFilterElementControl)ElementControl; } }
		protected override bool AllowPrintSingleItem { get { return false; } }
		protected FilterElementDashboardItemViewer() {
			viewControl = new FilterElementDashboardItemViewControl(ElementControl);
		}
		protected override Control GetViewControl() {
			return DashboardFilterElementControl;
		}
		protected override void PrepareViewControl() {
			base.PrepareViewControl();
			ElementControl.SelectionChanged += OnFilterElementSelectionChanged;
		}
		void OnFilterElementSelectionChanged(object sender, EventArgs e) {
			((IInteractivityControllerClient)this).PerformMasterFilterOperation((List<AxisPointTuple>)viewControl.GetSelection());
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(DashboardFilterElementControl);
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();
			if(ViewModel.ShouldIgnoreUpdate)
				return;
			UnsubscribeControlEvents();
			viewControl.Update((FilterElementDashboardItemViewModel)ViewModel, MultiDimensionalData);
			SubscribeControlEvents();
		}
		protected override void SetSelection(List<AxisPointTuple> selection) {
			base.SetSelection(selection);
			viewControl.SetSelection(selection); 
		}
		protected override void PerformClearMasterFilterOperationInternal(bool isRangeFilter) {
			ServiceClient.SetMasterFilter(DashboardItemName, new List<object>().Cast<IEnumerable<object>>());
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				UnsubscribeControlEvents();
			}
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			int indice = DashboardFilterElementControl.Editor.GetIndex(location);
			if(indice >= 0) {
				IList dimensionValues = viewControl.GetDimensionValues(indice);
				if(dimensionValues.Count > 0) {
					DataPointInfo info = new DataPointInfo();
					info.DimensionValues.Add(DashboardDataAxisNames.DefaultAxis, dimensionValues);
					return info;
				}
			}
			return null;
		}
		protected virtual int GetIndice(Point location) { 
			return -1;
		}
		void SubscribeControlEvents() {
			Control control = (Control)(DashboardFilterElementControl.Editor);
			if(control != null) {
				control.MouseClick += OnControlMouseClick;
				control.MouseDoubleClick += OnControlMouseDoubleClick;
				control.MouseMove += OnControlMouseMove;
				control.MouseEnter += OnControlMouseEnter;
				control.MouseLeave += OnControlMouseLeave;
				control.MouseDown += OnControlMouseDown;
				control.MouseUp += OnControlMouseUp;
				control.MouseHover += OnControlMouseHover;
				control.MouseWheel += OnControlMouseWheel;
			}
		}
		void UnsubscribeControlEvents() {
			Control control = (Control)(DashboardFilterElementControl.Editor);
			if(control != null) {
				control.MouseClick -= OnControlMouseClick;
				control.MouseDoubleClick -= OnControlMouseDoubleClick;
				control.MouseMove -= OnControlMouseMove;
				control.MouseEnter -= OnControlMouseEnter;
				control.MouseLeave -= OnControlMouseLeave;
				control.MouseDown -= OnControlMouseDown;
				control.MouseUp -= OnControlMouseUp;
				control.MouseHover -= OnControlMouseHover;
				control.MouseWheel -= OnControlMouseWheel;
			}
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
