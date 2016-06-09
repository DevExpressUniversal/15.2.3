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

using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Native;
using DevExpress.XtraMap.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[
	DXToolboxItem(false),
	DashboardItemDesigner(typeof(MapDashboardItemDesigner))
	]
	public abstract class MapDashboardItemViewer : DataDashboardItemViewer {
		const int ViewportChangerTimerInterval = 500;
		const int CenterPointAvailableOffset = 5;
		const int sizeChangedTimerInterval = 500;
		readonly DashboardMapControl mapControl = new DashboardMapControl();
		readonly MapDashboardItemViewControl viewControl;
		readonly Timer viewportChangedTimer = new Timer();
		readonly Timer sizeChangedTimer = new Timer();
		bool layoutChanging = false;
		Point mouseDownPoint;
		protected MapDashboardItemViewControl MapViewControl { get { return viewControl; } }
		public DashboardMapControl MapControl { get { return mapControl; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		protected MapDashboardItemViewer() {
			viewControl = CreateViewControl(mapControl);
			mapControl.BeginInit();
			mapControl.SizeChanged += OnSizeChanged;
			mapControl.KeyDown += OnMapControlKeyDown;
			mapControl.KeyUp += OnMapControlKeyUp;
			mapControl.LostFocus += OnMapControlLostFocus;
			mapControl.SelectionChanging += OnMapControlSelectionChanging;
			mapControl.MouseDown += OnMapControlMouseDown;
			mapControl.MouseUp += OnMapControlMouseUp;
			mapControl.DashboardMouseWheel += OnMapControlMouseWheel;
			mapControl.MouseClick += OnMapControlClick;
			mapControl.MouseDoubleClick += OnMapControlDoubleClick;
			mapControl.MouseMove += OnMapControlMouseMove;
			mapControl.MouseLeave += OnMapControlMouseLeave;
			mapControl.MouseEnter += OnMapControlMouseEnter;
			mapControl.MouseHover += OnMapControlMouseHover;
			IInnerMapService svc = ((IServiceProvider)mapControl).GetService(typeof(IInnerMapService)) as IInnerMapService;
			InnerMap innerMap = svc.Map;
			innerMap.MapItemHighlighting += OnMapControlHighlighting;
			viewportChangedTimer.Interval = ViewportChangerTimerInterval;
			viewportChangedTimer.Tick += OnViewportChangedTimerTick;
			sizeChangedTimer.Interval = sizeChangedTimerInterval;
			sizeChangedTimer.Tick += OnSizeChangedTimerTick;
			mapControl.SelectionMode = ElementSelectionMode.None;
			mapControl.EndInit();
		}
		protected override void InitializeInternal() {
			base.InitializeInternal();
			DashboardViewer.LayoutControl.DashboardLayoutChanging += OnLayoutChanging;
			DashboardViewer.LayoutControl.DashboardLayoutChanged += OnLayoutChanged;
		}
		protected virtual void SizeChangedAction() {
		}
		protected abstract MapDashboardItemViewControl CreateViewControl(DashboardMapControl mapControl);
		void OnMapControlLostFocus(object sender, EventArgs e) {
			OnDashboardItemViewerLostFocus();
		}
		void OnViewportChangedTimerTick(object sender, EventArgs e) {
			RaiseViewportChangedTimerTick();
		}
		void OnLayoutChanging(object sender, EventArgs e) {
			sizeChangedTimer.Stop();
			layoutChanging = true;
			if(viewportChangedTimer.Enabled)
				RaiseFileLayerViewportChanged();
		}
		void OnLayoutChanged(object sender, EventArgs e) {
			if(layoutChanging) {
				SizeChangedAction();
				layoutChanging = false;
			}
		}
		void OnSizeChangedTimerTick(object sender, EventArgs e) {
			SizeChangedAction();
			sizeChangedTimer.Stop();
		}
		void OnViewportChanged(object sender, ViewportChangedEventArgs e) {
			if(!sizeChangedTimer.Enabled && !layoutChanging) {
				viewportChangedTimer.Stop();
				viewportChangedTimer.Start();
			}
		}
		void OnMapControlMouseDown(object sender, MouseEventArgs e) {
			mouseDownPoint = e.Location;
			RaiseMouseDown(e.Location);
		}
		void OnMapControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnMapControlMouseLeave(object sender, EventArgs e) {
			OnDashboardItemViewerMouseLeave();
			RaiseMouseLeave();
		}
		void OnMapControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnMapControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		void OnMapControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		void OnMapControlKeyUp(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyUp(e);
		}
		void OnMapControlKeyDown(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyDown(e);
		}
		void OnMapControlMouseMove(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseMove(e);
			RaiseMouseMove(e.Location);
		}
		void OnMapControlDoubleClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseDoubleClick(e);
			RaiseDoubleClick(e.Location);
		}
		void OnMapControlClick(object sender, MouseEventArgs e) {
			if(mouseDownPoint == e.Location) {
				OnDashboardItemViewerMouseClick(e);
				RaiseClick(e.Location);
			}
		}
		protected abstract bool CancelSelection(MapItem item);
		protected void OnMapControlSelectionChanging(object sender, MapSelectionChangingEventArgs e) {
			List<object> objectsToRemove = new List<object>();
			foreach(object mapObject in e.Selection) {
				MapItem item = mapObject as MapItem;
				if(item != null && CancelSelection(item)) {
					objectsToRemove.Add(mapObject);
				}
			}
			foreach(object mapObject in objectsToRemove) {
				e.Selection.Remove(mapObject);
			}
		}
		void OnMapControlHighlighting(object sender, MapItemHighlightingEventArgs e) {
			e.Cancel = CancelSelection(e.Item);
		}
		void OnSizeChanged(object sender, EventArgs e) {
			RaiseSizeChanged();
		}
		protected virtual void RaiseSizeChanged() {
			if(viewportChangedTimer.Enabled)
				RaiseFileLayerViewportChanged();
			if(!layoutChanging) {
				sizeChangedTimer.Stop();
				sizeChangedTimer.Start();
			}
			ZoomToCurrentRegion();
		}
		void ZoomToCurrentRegion() {
			mapControl.EnableAnimation = false;
			viewControl.ZoomToCurrentRegion();
			mapControl.EnableAnimation = true;
		}
		protected override Control GetViewControl() {
			return mapControl;
		}
		protected override Control GetUnderlyingControl() {
			return mapControl;
		}
		protected override void PrepareViewControl() {
			mapControl.BorderStyle = BorderStyles.NoBorder;
		}
		protected internal override IList<DashboardItemCaptionButtonInfoCreator> GetButtonInfoCreators() {
			IList<DashboardItemCaptionButtonInfoCreator> creators = base.GetButtonInfoCreators();
			MapDashboardItemDesigner designer = DesignerProvider.GetDesigner<MapDashboardItemDesigner>();
			if(designer == null && ((IDashboardMapControl)mapControl).EnableNavigation)
				creators.Add(new MapInitialExtentPopupMenuCreator());
			return creators;
		}
		internal void UpdateViewer(MapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged) {
			mapControl.DrawIgnoreUpdatesState = ViewModel.ShouldIgnoreUpdate;
			if(ViewModel.ShouldIgnoreUpdate)
				return;
			mapControl.SuspendRender();
			mapControl.EnableAnimation = false;
			Update(viewModel, data, dataChanged);
			mapControl.EnableAnimation = true;
			mapControl.ResumeRender();
			if(MapControl.Layers.Count > 0)
				MapControl.Layers[0].ViewportChanged += OnViewportChanged;
		}
		protected abstract void Update(MapDashboardItemViewModel viewModel, MultiDimensionalData data, bool dataChanged);
		internal virtual void RaiseViewportChangedTimerTick() {
			RaiseFileLayerViewportChanged();
		}
		void RaiseFileLayerViewportChanged() {
			viewportChangedTimer.Stop();
			MapViewportState viewportState = mapControl.GetViewportState();
			GeoPoint newCenter = new GeoPoint(viewportState.CenterPointLatitude, viewportState.CenterPointLongitude);
			if(CheckCenterPointOffset(newCenter))
				RaiseFileLayerViewportChanged(viewportState);
		}
		bool CheckCenterPointOffset(GeoPoint newCenter) {
			MapPoint newCenterPoint = MapControl.CoordPointToScreenPoint(newCenter);
			GeoPoint oldCenter = viewControl.GetCenterPoint();
			MapPoint oldCenterPoint = MapControl.CoordPointToScreenPoint(oldCenter);
			return Math.Abs(newCenterPoint.X - oldCenterPoint.X) > CenterPointAvailableOffset || Math.Abs(newCenterPoint.Y - oldCenterPoint.Y) > CenterPointAvailableOffset;
		}
		protected void RaiseFileLayerViewportChanged(MapViewportState viewportState) {
			viewControl.ClientViewportState = viewportState;
			MapDashboardItemDesigner designer = DesignerProvider.GetDesigner<MapDashboardItemDesigner>();
			if(designer != null)
				designer.OnMapControlViewportChanged(viewportState);
		}
		internal virtual bool ClearClientViewportState() {
			if(viewportChangedTimer.Enabled || viewControl.ClientViewportState != null) {
				viewportChangedTimer.Stop();
				viewControl.ClientViewportState = null;
				ZoomToCurrentRegion();
				return true;
			}
			return false;
		}
		protected override void UpdateViewerByViewModel() {			
				UpdateViewer((MapDashboardItemViewModel)ViewModel, MultiDimensionalData, false);
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();			
				UpdateViewer((MapDashboardItemViewModel)ViewModel, MultiDimensionalData, true);
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(mapControl);
			state.SpecificState = new Dictionary<string, object> { { "MapViewportState", mapControl.GetViewportState() } };
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				mapControl.SizeChanged -= OnSizeChanged;
				mapControl.KeyDown -= OnMapControlKeyDown;
				mapControl.KeyUp -= OnMapControlKeyUp;
				mapControl.MouseUp -= OnMapControlMouseUp;
				mapControl.LostFocus -= OnMapControlLostFocus;
				mapControl.SelectionChanging -= OnMapControlSelectionChanging;
				mapControl.MouseDown -= OnMapControlMouseDown;
				mapControl.MouseUp -= OnMapControlMouseUp;
				mapControl.MouseWheel -= OnMapControlMouseWheel;
				mapControl.MouseClick -= OnMapControlClick;
				mapControl.MouseMove -= OnMapControlMouseMove;
				mapControl.MouseDoubleClick -= OnMapControlDoubleClick;
				mapControl.MouseLeave -= OnMapControlMouseLeave;
				mapControl.MouseEnter -= OnMapControlMouseEnter;
				mapControl.MouseHover -= OnMapControlMouseHover;
				if(MapControl.Layers.Count > 0)
					MapControl.Layers[0].ViewportChanged -= OnViewportChanged;
				viewportChangedTimer.Tick -= OnViewportChangedTimerTick;
				sizeChangedTimer.Tick -= OnSizeChangedTimerTick;
				DashboardViewer.LayoutControl.DashboardLayoutChanging -= OnLayoutChanging;
				DashboardViewer.LayoutControl.DashboardLayoutChanged -= OnLayoutChanged;
				sizeChangedTimer.Dispose();
				viewportChangedTimer.Dispose();
				mapControl.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void SetHighlight(List<AxisPointTuple> highlight) {
		}
		protected override void SetSelection(List<AxisPointTuple> selection) {
			viewControl.UpdateSelection(GetDimestionValueByAxis(selection, TargetAxes));			
		}
		protected override InteractivityController CreateInteractivityController() {
			MapInteractivityController controller = new MapInteractivityController(this);
			return (InteractivityController)(controller);
		}
		public List<AxisPointTuple> GetControlSelection(){
			List<AxisPointTuple> tuples = new List<AxisPointTuple>();
			IList selectionValues = viewControl.GetSelection();
			foreach(IList value in selectionValues) {
				foreach(string axis in TargetAxisCore) {
					AxisPoint axisPoint = GetAxisPoint(axis, value);
					if(axisPoint != null) {
						tuples.Add(MultiDimensionalData.CreateTuple(axisPoint));
					}
				}
			}
			return tuples;
		}
	}
}
