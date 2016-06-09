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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Map;
namespace DevExpress.Xpf.Map {
	public class NavigationController {
		readonly MapControl map;
		ContentPresenter zoomRegionPresenter;
		NavigationState state;
		double oldZoomLevel;
		protected internal MapControl Map { get { return map; } }
		protected MapCoordinateSystem CoordinateSystem { get { return map.ActualCoordinateSystem; } }
		protected NavigationState State { get { return state; } }
		internal ContentPresenter ZoomRegionPresenter {
			get { return zoomRegionPresenter; }
			set { zoomRegionPresenter = value; } 
		}
		protected internal bool IsRegionSelecting { get { return State is RegionSelectionStateBase; } }
		public NavigationController(MapControl map) {
			this.map = map;
			this.state = new DefaultState(this);
		}
		internal void SetDefaultState() {
			SetCurrentState(new DefaultState(this));
		}
		protected void SetCurrentState(NavigationState state) {
			State.Deactivate();
			this.state = state;
			State.Activate();
		}
		internal void KeepZoomLevel() {
			this.oldZoomLevel = map.ZoomLevel;
		}
		protected internal bool CanScroll() {
			return Map.EnableScrolling;
		}
		protected internal bool CanZoom() {
			return Map.EnableZooming;
		}
		protected bool CanSelect() {
			return Map.SelectionMode == ElementSelectionMode.Extended || map.SelectionMode == ElementSelectionMode.Multiple;
		}
		NavigationState CalculateStateOnMouseDown(Point point, ModifierKeys keys) {
			if(keys == ModifierKeys.Shift && CanSelect())
				return new SelectItemsRegionSelectionState(this);
			if (keys == (ModifierKeys.Shift | ModifierKeys.Control) && CanZoom())
				return new ZoomRegionSelectionState(this);
			if(CanScroll())
				return new MoveCenterPointState(this) { StartPoint = point };
			return new DefaultState(this);
		}
		internal void Move(Point offset) {
			Map.Move(offset);
		}
		internal void ZoomAndMove(double zoomFactor, Point zoomAnchorPoint, Point moveOffset) {
			double newZoomLevel = Math.Max(Math.Min(oldZoomLevel + Math.Log(zoomFactor, 2), 20), 1);
			if (map.EnableZooming)
				map.SetZoomLevel(newZoomLevel, zoomAnchorPoint);
			if (map.EnableScrolling)
				Move(moveOffset);
		}
		internal void HideZoomRegionPresenter() {
			ZoomRegionPresenter.Visibility = Visibility.Collapsed;
		}
		internal void UpdateZoomRegion(Point lastMousePositionInPix, Point newMousePositionInPix) { 
			double selectedRectWidthInPix = Math.Abs(lastMousePositionInPix.X - newMousePositionInPix.X);
			double selectedRectHeightInPix = Math.Abs(lastMousePositionInPix.Y - newMousePositionInPix.Y);
			double selectedRectleftInPix = Math.Min(lastMousePositionInPix.X, newMousePositionInPix.X);
			double selectedRectTopInPix = Math.Min(lastMousePositionInPix.Y, newMousePositionInPix.Y);
			zoomRegionPresenter.Visibility = Visibility.Visible;
			Canvas.SetLeft(zoomRegionPresenter, selectedRectleftInPix);
			Canvas.SetTop(zoomRegionPresenter, selectedRectTopInPix);
			zoomRegionPresenter.Width = selectedRectWidthInPix;
			zoomRegionPresenter.Height = selectedRectHeightInPix;
		}
		CoordPoint CalculateCenterPoint(Point lastMousePositionInPix, Point newMousePositionInPix) {
			CoordPoint lastMousePositionInMapUnits = CoordinateSystem.ScreenPointToCoordPoint(lastMousePositionInPix, map.Viewport, map.ViewportInPixels);
			CoordPoint newMousePositionInMapUnits = CoordinateSystem.ScreenPointToCoordPoint(newMousePositionInPix, map.Viewport, map.ViewportInPixels);
			double newCenterPointInMapUnitsX = (lastMousePositionInMapUnits.GetX() + newMousePositionInMapUnits.GetX()) / 2;
			double newCenterPointInMapUnitsY = (lastMousePositionInMapUnits.GetY() + newMousePositionInMapUnits.GetY()) / 2;
			return CoordinateSystem.CreatePoint(newCenterPointInMapUnitsX, newCenterPointInMapUnitsY);
		}
		CoordVector CalculateZoomRegionSizeInMapUnits(Point lastMousePositionInPix, Point newMousePositionInPix) {
			MapUnit lastMousePositionInMapUnits = CoordinateSystem.ScreenPointToMapUnit(lastMousePositionInPix, map.Viewport, map.ViewportInPixels);
			MapUnit newMousePositionInMapUnits = CoordinateSystem.ScreenPointToMapUnit(newMousePositionInPix, map.Viewport, map.ViewportInPixels);
			double selectedRectWidthInMapUnits = Math.Abs(lastMousePositionInMapUnits.X - newMousePositionInMapUnits.X);
			double selectedRectHeightInMapUnits = Math.Abs(lastMousePositionInMapUnits.Y - newMousePositionInMapUnits.Y);
			return new CoordVector(selectedRectWidthInMapUnits, selectedRectHeightInMapUnits);
		}
		CoordVector CalculateVieportSizeInMapUnits() {
			Point zeroPoint = new Point(0, 0);
			Point maxPoint = new Point(map.RenderSize.Width, map.RenderSize.Height);
			MapUnit zeroPointInMapUnits = CoordinateSystem.ScreenPointToMapUnit(zeroPoint, map.Viewport, map.ViewportInPixels);
			MapUnit maxPointInMapUnit = CoordinateSystem.ScreenPointToMapUnit(maxPoint, map.Viewport, map.ViewportInPixels);
			double viewPortWidthInMapUnits = maxPointInMapUnit.X - zeroPointInMapUnits.X;
			double viewPortHeightInMapUnits = maxPointInMapUnit.Y - zeroPointInMapUnits.Y;
			return new CoordVector(viewPortWidthInMapUnits, viewPortHeightInMapUnits);
		}
		bool IsRegionEmpty(Point p1, Point p2){
			return p1.Equals(p2);
		}
		internal void ZoomIntoRegion(Point lastMousePositionInPix, Point newMousePositionInPix) {
			if(map.EnableZooming && map.Layers.Count > 0 && !IsRegionEmpty(lastMousePositionInPix, newMousePositionInPix)) {
				map.SetCenterPoint(CalculateCenterPoint(lastMousePositionInPix, newMousePositionInPix));
				CoordVector selectedRectSize = CalculateZoomRegionSizeInMapUnits(lastMousePositionInPix, newMousePositionInPix);
				CoordVector viewportSize = CalculateVieportSizeInMapUnits();
				double calcuatedZoomLevel = NavigationCalculations.CalculateZoomLevelAfterZoomToRegion(viewportSize, selectedRectSize, map.ZoomLevel);
				map.SetZoomLevel(calcuatedZoomLevel);
			}
		}
		internal void SelectItemsByRegion(Point startPos, Point endPos) {
			if(map.Layers.Count == 0 || IsRegionEmpty(startPos, endPos))
				return;
			map.ViewController.SelectItemsByRegion(new Rect(startPos, endPos));
		}
		public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if(State.AllowHandleMouseLButtonDown) {
				NavigationState newState = CalculateStateOnMouseDown(e.GetPosition(Map), Keyboard.Modifiers);
				SetCurrentState(newState);
			}
			State.OnMouseLeftButtonDown(e);
			if (State.AllowMouseCapture && Map != null)
				Mouse.Capture(Map, CaptureMode.SubTree);
		}
		public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			State.OnMouseLeftButtonUp(e);
			SetDefaultState();
			if (State.AllowMouseCapture && Map != null)
				Map.ReleaseMouseCapture();
		}
		public void MouseMove(object sender, MouseEventArgs e) {
			State.OnMouseMove(e);
		}
		public void MouseWheel(object sender, MouseWheelEventArgs e) {
			State.OnMouseWheel(e);
		}
		public void ManipulationStarted(object sender, ManipulationStartedEventArgs e) {
			SetCurrentState(new TouchState(this));
			State.OnManipulationStarted(e);
			oldZoomLevel = map.ZoomLevel;
		}
		public void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			State.OnManipulationComplete(e);
			SetDefaultState();
		}
		public void ManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
			State.OnManipulationDelta(e);
		}
		public void MouseDown(object sender, MouseButtonEventArgs e) {
			Keyboard.Focus(map);
		}
	}
}
