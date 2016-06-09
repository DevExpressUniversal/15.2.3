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

using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;
namespace DevExpress.XtraMap.Native {
	public class DesignMapMouseHandler : MapMouseHandler {
		public DesignMapMouseHandler(InnerMap map)
			: base(map) {
		}
		public override void SwitchToDefaultState() {
			EmptyMouseHandlerState state = new EmptyMouseHandlerState(this);
			SwitchStateCore(state, Point.Empty);
		}
		protected override void CalculateAndSaveHitInfo(MouseEventArgs e) {
		}
	}
	public class MapMouseHandler : MouseHandler {
		readonly InnerMap map;
		MapHitInfo currentHitInfo;
		IMapUiHitInfo currentUiHitInfo;
		Cursor savedCursor;
		Rectangle selectedRegion;
		public Keys KeyModifiers { get; set; }
		public Point MousePosition { get; set; }
		public Rectangle SelectedRegion {
			get { return selectedRegion; }
			set {
				if (selectedRegion == value)
					return;
				this.selectedRegion = value;
				OnSelectionRegionChanged();
			}
		}
		public IMapUiHitInfo CurrentUiHitInfo { get { return currentUiHitInfo; } }
		public MapHitInfo CurrentHitInfo { get { return currentHitInfo; } }
		public InnerMap Map { get { return map; } }
		public bool IsUIElementsHitTest { get { return currentUiHitInfo != null ? currentUiHitInfo.HitElement != MapHitUiElementType.None : false; } }
		public MapMouseHandler(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		public override void Initialize() {
			base.Initialize();
			SwitchToDefaultState();
		}
		public override void RunClickTimer() {
		}
		protected internal virtual void SaveCursor() {
			if (savedCursor == null)
				savedCursor = Map.Cursor;
		}
		protected internal virtual void RestoreCursor() {
			SetCurrentCursor(savedCursor);
			savedCursor = null;
		}
		protected internal virtual void SetCurrentCursor(Cursor cursor) {
			if (cursor != null)
				Map.Cursor = cursor;
		}
		protected internal void UpdateHotTrackCursor() {
			MapHitUiElementType hitTest = CurrentUiHitInfo.HitElement;
			if (hitTest == MapHitUiElementType.ZoomTrackBarThumb ||
				hitTest == MapHitUiElementType.ZoomTrackBar ||
				hitTest == MapHitUiElementType.ZoomIn ||
				hitTest == MapHitUiElementType.ZoomOut ||
				hitTest == MapHitUiElementType.ScrollButtons) {
				SaveCursor();
				SetCurrentCursor(Cursors.Hand);
			}
			else
				RestoreCursor();
		}
		protected override void CalculateAndSaveHitInfo(MouseEventArgs e) {
			MousePosition = e.Location;
			KeyModifiers = Control.ModifierKeys;
			Point point = new Point(e.X, e.Y);
			currentUiHitInfo = map.CalcUiHitInfo(point);
			if(currentUiHitInfo.HitElement == MapHitUiElementType.None && Map.AllowHitTest(e.Clicks > 0))
				currentHitInfo = map.CalcHitInfo(point);
			else {
				currentHitInfo = new MapHitInfo(Point.Empty, new IHitTestableElement[0], currentUiHitInfo);
			}
		}
		protected override AutoScroller CreateAutoScroller() {
			return new MapAutoScroller(this);
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return null;
		}
		protected override void HandleClickTimerTick() {
		}
		protected override void HandleMouseWheel(MouseEventArgs e) {
			map.HideToolTip();
			State.OnMouseWheel(e);
		}
		protected override void StartOfficeScroller(Point clientPoint) {
		}
		public override void SwitchToDefaultState() {
			DefaultMapMouseHandlerState newState = new DefaultMapMouseHandlerState(this);
			SwitchStateCore(newState, Point.Empty);
		}
		protected bool CanChangeStateOnMouseDown(MapMouseHandlerStateBase newState) {
			if(!newState.SupportDisabledScrolling && !Map.EnableScrolling)
				return false;
			if (currentUiHitInfo.HitElement == MapHitUiElementType.MiniMap)
				return true;
			return !IsUIElementsHitTest;
		}
		protected override void HandleMouseDown(MouseEventArgs e) {
			base.HandleMouseDown(e);
			MapMouseHandlerStateBase state = CalculateStateOnMouseDown(e.Location);
			if (CanChangeStateOnMouseDown(state))
				SwitchStateCore(state, e.Location);
			else {
				MapHitUiElementType hitTest = CurrentUiHitInfo.HitElement;
				ApplyHitTest(hitTest, e.Location);
			}
		}
		MapMouseHandlerStateBase CalculateStateOnMouseDown(Point point) {
			if (currentUiHitInfo.HitElement == MapHitUiElementType.MiniMap)
				return new MoveMiniMapCenterPointHandlerState(this, currentUiHitInfo.HitPoint);
			if (KeyModifiers == Keys.Shift) {
				return new RegionSelectionHandlerState(this);
			}
			if (KeyModifiers == (Keys.Shift | Keys.Control))
				return new ZoomToSelectedRegionHandlerState(this);
			if (Map.OperationHelper.CanScroll()) {
				return new MoveCenterPointHandlerState(this, point);
			}
			return new DefaultMapMouseHandlerState(this);
		}
		internal bool CheckUiHitTest(MapHitUiElementType hitTest) {
			return CurrentUiHitInfo != null && CurrentUiHitInfo.HitElement == hitTest;
		}
		void ApplyHitTest(MapHitUiElementType hitTest, Point mouseLocation) {
			map.RenderController.UpdateVisualState(hitTest);
			if (currentUiHitInfo.HitElement == MapHitUiElementType.ZoomTrackBarThumb) {
				DragZoomTrackBarHandlerState zoomTrackBarState = new DragZoomTrackBarHandlerState(this);
				BeginMouseDragHelperState zoomTrackBeginDragState = new BeginMouseDragHelperState(this, zoomTrackBarState, mouseLocation);
				SwitchStateCore(zoomTrackBeginDragState, mouseLocation);
			}
			if (currentUiHitInfo.HitElement == MapHitUiElementType.ZoomIn)
				Map.ZoomIn();
			if (currentUiHitInfo.HitElement == MapHitUiElementType.ZoomOut)
				Map.ZoomOut();
			if (currentUiHitInfo.HitElement == MapHitUiElementType.ScrollButtons) {
				DragScrollButtonsHandlerState scrollButtonsState = new DragScrollButtonsHandlerState(this);
				SwitchStateCore(scrollButtonsState, mouseLocation);
			}
		}
		protected override void HandleMouseUp(MouseEventArgs e) {
			map.Capture = false;
			base.HandleMouseUp(e);
			map.RenderController.ResetVisualState();
		}
		void OnSelectionRegionChanged() {
			Map.RenderController.UpdateSelectedRegion(SelectedRegion);
			Map.Render();
		}
		protected internal MapAnimationStartMode CalculateAnimationStartMode() {
			MapMouseHandlerStateBase state = State as MapMouseHandlerStateBase;
			return state != null ? state.CalculateAnimationStartMode() : MapAnimationStartMode.Runtime;
		}
	}
	public class MapAutoScroller : AutoScroller {
		public MapAutoScroller(MouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override void PopulateHotZones() {
		}
	}
	public abstract class MapMouseHandlerStateBase : MouseHandlerState {
		public MapMouseHandler MapHandler { get { return (MapMouseHandler)base.MouseHandler; } }
		protected InnerMap Map { get { return MapHandler.Map; } }
		protected internal virtual bool SupportDisabledScrolling { get { return true; } }
		protected MapMouseHandlerStateBase(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected MapAnimationStartMode CalculateCapturedAnimationStartMode() {
			return Map.Capture ? MapAnimationStartMode.Interactive : MapAnimationStartMode.Runtime;
		}
		public virtual MapAnimationStartMode CalculateAnimationStartMode() {
			return MapAnimationStartMode.Runtime;
		}
		public void UpdateOverlaysIfNessesary() {
			if(MapHandler.CurrentUiHitInfo != null && MapHandler.CurrentUiHitInfo.HitElement == MapHitUiElementType.Overlay)
				Map.UpdateOverlays();
		}
	}
	public class EmptyMouseHandlerState : MapMouseHandlerStateBase {
		public EmptyMouseHandlerState(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
	}
	public class DefaultMapMouseHandlerState : MapMouseHandlerStateBase {
		bool mouseWheelCaptured = false;
		public DefaultMapMouseHandlerState(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		void UpdateZoomTrackBar(MouseEventArgs e) {
			RenderController controller = Map.RenderController;
			controller.BeginUpdate();
			try {
				controller.UpdateZoomLevel(e.Location);
			}
			finally {
				controller.EndUpdate();
			}
		}
		public override void Finish() {
			base.Finish();
			MapHandler.RestoreCursor();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!MapHandler.IsUIElementsHitTest)
				Map.InteractionController.HandleMouseMove(e, MapHandler.CurrentHitInfo);
			else 
				MapHandler.UpdateHotTrackCursor(); 
			UpdateOverlaysIfNessesary();
			Map.UpdateNavigationPanel(e.Location, true);
			Map.RenderController.PerformUpdate(UpdateActionType.Render);
		}
		public override void OnMouseDown(MouseEventArgs e) {
			if (MapHandler.CurrentUiHitInfo.HitElement == MapHitUiElementType.ZoomTrackBar)
				UpdateZoomTrackBar(e);
			base.OnMouseDown(e);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			Map.InteractionController.HandleMouseUp(e, MapHandler.CurrentUiHitInfo, MapHandler.CurrentHitInfo);
			base.OnMouseUp(e);
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			mouseWheelCaptured = true;
			int delta = e.Delta;
			if(delta != 0 && Map.EnableZooming) {
				double zoomDelta = CalculateZoomDeltaOnMouseWheel(delta);
				bool isMiniMap = MapHandler.CheckUiHitTest(MapHitUiElementType.MiniMap);
				ApplyZoomToMap(zoomDelta, e.Location, isMiniMap);
			}
			mouseWheelCaptured = false;
			base.OnMouseWheel(e);
		}
		void ApplyZoomToMap(double zoomDelta, Point anchorPoint, bool isMiniMap) {
			double newZoomValue = Map.ZoomLevel + zoomDelta;
			double newZoomLevel = MathUtils.MinMax(newZoomValue, Map.MinZoomLevel, Map.MaxZoomLevel);
			if (!isMiniMap)
				Map.Zoom(newZoomLevel, new MapPoint(anchorPoint.X, anchorPoint.Y));
			else if (Map.MiniMap.EnableZooming)
				Map.Zoom(newZoomLevel);
		}
		double CalculateZoomDeltaOnMouseWheel(int delta) {
			if (delta == 0)
				return 0.0;
			double zoomDelta = delta / 120.0f;
			if (zoomDelta < 0) {
				double roundedZoomLevel = Math.Round(Map.ZoomLevel + zoomDelta);
				if (roundedZoomLevel <= 1.0)
					zoomDelta = -0.1;
			}
			else {
				if (Map.ZoomLevel < 1.0)
					zoomDelta = 0.1;
			}
			return zoomDelta;
		}
		public override MapAnimationStartMode CalculateAnimationStartMode() {
			if(Map.KeyboardShifting)
				return CalculateCapturedAnimationStartMode();
			return this.mouseWheelCaptured ? MapAnimationStartMode.Interactive : MapAnimationStartMode.Runtime;
		}
	}
	public class MoveCenterPointHandlerState : MapMouseHandlerStateBase {
		Point lastDragPoint;
		Point translateOffset = new Point();
		protected internal override bool SupportDisabledScrolling { get { return false; } }
		public MoveCenterPointHandlerState(MapMouseHandler mouseHandler, Point startDragPoint)
			: base(mouseHandler) {
			this.lastDragPoint = startDragPoint;
		}
		public override bool StopClickTimerOnStart { get { return true; } }
		public override void Start() {
			base.Start();
			Map.Capture = true;
			MapHandler.SaveCursor();
		}
		public override void Finish() {
			Map.Capture = false;
			MapHandler.RestoreCursor();
			base.Finish();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Point point = e.Location;
			if (lastDragPoint == point) return;
			this.translateOffset = new Point(lastDragPoint.X - e.X, lastDragPoint.Y - e.Y);
			lastDragPoint = point;
			RenderController controller = Map.RenderController;
			MapHandler.SetCurrentCursor(DragCursors.HandDragCursor);
			Map.Shift(translateOffset);
			Map.UpdateNavigationPanel(e.Location, false);
			UpdateOverlaysIfNessesary();
			controller.RegisterUpdate(UpdateActionType.Render);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if (translateOffset.IsEmpty) {
				Map.InteractionController.HandleMouseUp(e, MapHandler.CurrentUiHitInfo, MapHandler.CurrentHitInfo);
			}
			MouseHandler.SwitchToDefaultState();
		}
		public override MapAnimationStartMode CalculateAnimationStartMode() {
			return CalculateCapturedAnimationStartMode();
		}
	}
	public class DragZoomTrackBarHandlerState : MapMouseHandlerStateBase {
		public DragZoomTrackBarHandlerState(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override void OnMouseMove(MouseEventArgs e) {
			Map.RenderController.UpdateZoomLevel(e.Location);
			Map.UpdateTrackBarToolTip(e.Location, true);
			base.OnMouseMove(e);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			MouseHandler.SwitchToDefaultState();
			Map.UpdateTrackBarToolTip(e.Location, false);
			base.OnMouseUp(e);
		}
	}
	public class DragScrollButtonsHandlerState : MapMouseHandlerStateBase, IDisposable {
		const int DragTimerInterval = 100;
		Timer dragTimer;
		Point dragOffset = Point.Empty;
		bool disposed = false;
		protected internal override bool SupportDisabledScrolling { get { return false; } }
		public DragScrollButtonsHandlerState(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
			InitDragTimer();
		}
		~DragScrollButtonsHandlerState() {
			Dispose(false);
		}
		void DragTimerTick(object sender, EventArgs e) {
			Map.Shift(dragOffset);
		}
		void BeginDragging() {
			UpdateDragOffset(MapHandler.MousePosition);
			dragTimer.Start();
		}
		void EndDragging() {
			dragTimer.Stop();
			dragTimer.Tick -= DragTimerTick;
			dragTimer.Dispose();
		}
		void UpdateDragOffset(Point mousePosition) {
			dragOffset = Map.RenderController.GetScrollingOffset(mousePosition);
		}
		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
		void InitDragTimer() {
			dragTimer = new Timer();
			dragTimer.Interval = DragTimerInterval;
			dragTimer.Tick += DragTimerTick;
		}
		protected virtual void Dispose(bool disposing) {
			if (disposed)
				return;
			if (disposing && dragTimer != null)
				dragTimer.Dispose();
			disposed = true;
		}
		public override void Start() {
			base.Start();
			MapHandler.SaveCursor();
			MapHandler.SetCurrentCursor(Cursors.Hand);
			BeginDragging();
		}
		public override void Finish() {
			base.Finish();
			MapHandler.RestoreCursor();
			EndDragging();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			UpdateDragOffset(e.Location);
			Cursor curr = MapHandler.CurrentUiHitInfo.HitElement == MapHitUiElementType.ScrollButtons ? Cursors.Hand : DragCursors.HandDragCursor;
			MapHandler.SetCurrentCursor(curr);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			MouseHandler.SwitchToDefaultState();
			base.OnMouseUp(e);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	public class RegionSelectionHandlerState : MapMouseHandlerStateBase {
		Point startPoint;
		public RegionSelectionHandlerState(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override bool StopClickTimerOnStart { get { return true; } }
		public override void Start() {
			base.Start();
			this.startPoint = MapHandler.MousePosition;
			MapHandler.SelectedRegion = new Rectangle(MapHandler.MousePosition, Size.Empty);
			Map.Capture = true;
		}
		protected virtual void OnSelectionComplete(Rectangle region) {
			if (Map.OperationHelper.CanSelectedByRegion())
				Map.InteractionController.SelectItemsByRegion(region);
		}
		public override void Finish() {
			Map.Capture = false;
			OnSelectionComplete(MapHandler.SelectedRegion);
			MapHandler.SelectedRegion = Rectangle.Empty;
			base.Finish();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			MapHandler.SelectedRegion = CalculateSelectedRegion(e.Location);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			MouseHandler.SwitchToDefaultState();
		}
		Rectangle CalculateSelectedRegion(Point point) {
			return RectUtils.ValidateSelectedRectangle(new Rectangle(this.startPoint.X, this.startPoint.Y, point.X - this.startPoint.X, point.Y - this.startPoint.Y), Map.ContentRectangle);
		}
	}
	public class ZoomToSelectedRegionHandlerState : RegionSelectionHandlerState {
		public ZoomToSelectedRegionHandlerState(MapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override void OnSelectionComplete(Rectangle region) {
			Map.RenderController.ZoomToRegion(region);
		}
	}
	public class MoveMiniMapCenterPointHandlerState : MapMouseHandlerStateBase {
		Point lastDragPoint;
		Point translateOffset = new Point();
		protected internal override bool SupportDisabledScrolling { get { return false; } }
		public MoveMiniMapCenterPointHandlerState(MapMouseHandler mouseHandler, Point startDragPoint)
			: base(mouseHandler) {
			this.lastDragPoint = startDragPoint;
			if (Map.OperationHelper.CanScroll() && Map.MiniMap.EnableScrolling) {
				if (Map.MiniMap.Behavior.Center != null)
					Map.MiniMap.UpdateCenterPoint(TranslateToMiniMap(startDragPoint));
			}
		}
		public override bool StopClickTimerOnStart { get { return true; } }
		Point TranslateToMiniMap(Point screenPoint) {
			Point pt = Map.MiniMap.Bounds.Location;
			screenPoint.Offset(-pt.X, -pt.Y);
			return screenPoint;
		}
		public override void Start() {
			base.Start();
			Map.Capture = true;
			MapHandler.SaveCursor();
		}
		public override void Finish() {
			Map.Capture = false;
			MapHandler.RestoreCursor();
			base.Finish();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Point point = e.Location;
			if (lastDragPoint == point) return;
			this.translateOffset = new Point(lastDragPoint.X - e.X, lastDragPoint.Y - e.Y);
			lastDragPoint = point;
			if (Map.MiniMap.EnableScrolling && Map.OperationHelper.CanScroll()) {
				RenderController controller = Map.RenderController;
				controller.BeginUpdate();
				try {
					MapHandler.SetCurrentCursor(DragCursors.HandDragCursor);
					if (Map.MiniMap.Behavior.Center == null)
						Map.MiniMap.Shift(translateOffset);
					else
						Map.MiniMap.UpdateCenterPoint(TranslateToMiniMap(e.Location));
					Map.UpdateNavigationPanel(e.Location, false);
					controller.RegisterUpdate(UpdateActionType.Render);
				}
				finally {
					controller.EndUpdate();
				}
			}
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if (translateOffset.IsEmpty && Map.MiniMap.SetMapCenterOnClick) {
				Map.MiniMap.UpdateCenterPoint(TranslateToMiniMap(e.Location));
			}
			MouseHandler.SwitchToDefaultState();
		}
		public override MapAnimationStartMode CalculateAnimationStartMode() {
			return CalculateCapturedAnimationStartMode();
		}
	}
}
