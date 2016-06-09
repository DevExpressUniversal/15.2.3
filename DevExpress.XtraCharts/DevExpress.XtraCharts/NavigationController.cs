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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public enum MouseOperation {
		None,
		Drag,
		ZoomIn,
		ZoomOut
	}
	public enum NavigationState {
		Free,
		LeftButtonDragging,
		MiddleButtonDragging,
		Scrolling,
		ZoomingIn,
		ZoomingOut
	}
	public abstract class ChartNavigationControllerBase {
		bool isCursorInitializing = false;
		Cursor userCursor = Cursors.Default;
		const int mouseWheelSensitivity = 1;
		const int keyDraggingStep = 10;
		const int smallScrollStep = 1;
		const int firstScrollInterval = 200;
		const int scrollInterval = 50;
		int latestScrollRange;
		bool enableDragAndZoom = true;
		bool isThumbPanning;
		double latestRelativeScrollPosition;
		IChartContainer container;
		Timer scrollTimer;
		NavigationType latestScrollingType;
		ScrollingOrientation latestScrollingOrientation;
		NavigationState navigationState = NavigationState.Free;
		Point lastDragPoint = Point.Empty;
		Point firstZoomCorner = Point.Empty;
		Point lastZoomCorner = Point.Empty;
		Point overPan = Point.Empty;
		XYDiagramPaneBase focusedPane = null;
		object focusedElement = null;
		CursorType currentDragCursorType = CursorType.None;
		Control cursorHolder;
		Point ClientPoint { get { return ContainerAdapter.PointToClient(Control.MousePosition); } }
		IScrollingZoomingOptions ScrollingZoomingOptions { get { return Chart.Diagram as IScrollingZoomingOptions; } }
		protected Chart Chart { get { return container.Chart; } }
		protected ChartContainerAdapter ContainerAdapter { get { return Chart.ContainerAdapter; } }
		protected IChartContainer Container { get { return container; } }
		protected IAnnotationDragPoint AnnotationDragPoint { get { return focusedElement as IAnnotationDragPoint; } }
		public bool EnableDragAndZoom { get { return enableDragAndZoom; } set { enableDragAndZoom = value; } }
		public bool InProcess { get { return navigationState != NavigationState.Free; } }
		public NavigationState NavigationState { get { return navigationState; } }
		public ChartNavigationControllerBase(IChartContainer container) {
			this.container = container;
			cursorHolder = container as Control;
			scrollTimer = new Timer();
			scrollTimer.Enabled = false;
			scrollTimer.Tick += new EventHandler(OnTimerTick);
		}
		void SetCursor(Cursor cursor, CursorType cursorType) {
			QueryCursorEventArgs e = new QueryCursorEventArgs(cursor, cursorType);
			IChartInteractionProvider interactionProvider = container.InteractionProvider;
			if (interactionProvider != null)
				interactionProvider.OnQueryCursor(e);
			if (!container.DesignMode) {
				isCursorInitializing = true;
				try {
					if (cursorHolder != null)
						cursorHolder.Cursor = e.Cursor;
				}
				finally {
					isCursorInitializing = false;
				}
			}
		}
		void SetCursor(CursorType cursorType) {
			Cursor cursor = (cursorType == CursorType.None) ? userCursor : GetCursorByType(cursorType);
			SetCursor(cursor, cursorType);
		}
		void ResetCursor() {
			SetCursor(userCursor, CursorType.None);
		}
		bool CanDrag(Point p, MouseButtons mouseButtons) {
			return Chart.AnnotationNavigation.CanDrag() ||
				Chart.Diagram != null && Chart.Diagram.CanDrag(p, mouseButtons);
		}
		bool IsZoomMode(Point p, Keys modifierKeys) {
			return navigationState == NavigationState.ZoomingIn ||
				((modifierKeys & Keys.Shift) == Keys.Shift && Chart.CanZoom(p));
		}
		bool IsZoomOutMode(Point p, Keys modifierKeys) {
			return ((modifierKeys & Keys.Alt) == Keys.Alt && Chart.CanZoom(p) && Chart.CanZoomOut);
		}
		bool PerformScrollCommand(Point p) {
			if (Chart.Diagram == null || !Chart.Diagram.CanScroll)
				return false;
			ChartScrollParameters scrollParameters = Chart.HitTestController.FindScrollCommand(p);
			if (scrollParameters == null) {
				focusedPane = null;
				return false;
			}
			focusedPane = scrollParameters.Pane;
			Scroll(p, scrollParameters);
			return true;
		}
		bool PerformDragging(int x, int y, int dx, int dy, ChartScrollEventType scrollEventType) {
			return Chart.PerformDragging(x, y, dx, dy, scrollEventType, focusedElement);
		}
		bool ScrollingProcess(Point dragPoint) {
			if (focusedPane == null || latestScrollRange == 0)
				return false;
			XYDiagram2D diagram = focusedPane.Diagram;
			PaneAxesContainer paneAxesData = diagram.GetPaneAxesData(focusedPane);
			if (paneAxesData == null)
				return false;
			double pixelDelta = ((latestScrollingOrientation == ScrollingOrientation.AxisXScroll) ^ diagram.ActualRotated) ?
				(dragPoint.X - lastDragPoint.X) : (lastDragPoint.Y - dragPoint.Y);
			return paneAxesData.ScrollTo(latestRelativeScrollPosition + pixelDelta / latestScrollRange, latestScrollingOrientation, NavigationType.ThumbPosition);
		}
		void BeginGesturePan(Point location, Point delta) {
			InitializeFocusedElement(location);
			if (focusedElement == null && Chart.Diagram is XYDiagram2D)
				return;
			isThumbPanning = PerformScrollCommand(location);
			if (isThumbPanning)
				overPan = GetOverPan(delta);
			else {
				overPan = Point.Empty;
				PerformGesturePan(location, delta);
			}
		}
		void PerformGesturePan(Point location, Point delta) {
			if (focusedElement == null && Chart.Diagram is XYDiagram2D)
				return;
			Point currentOverPan = GetOverPan(delta);
			if (isThumbPanning) {
				if (((XYDiagram2D)Chart.Diagram).ScrollingOptions.UseScrollBars)
					ScrollingProcess(location);
			}
			else
				PerformDragging(location.X, location.Y, delta.X, delta.Y, ChartScrollEventType.Gesture);
			overPan.X += currentOverPan.X;
			overPan.Y += currentOverPan.Y;
		}
		void EndGesturePan(Point location, Point delta) {
			PerformGesturePan(location, delta);
			if (isThumbPanning) {
				ContainerAdapter.Capture = false;
				navigationState = NavigationState.Free;
				isThumbPanning = false;
			}
			else {
				InitializeFocusedElement(location);
				Chart.SelectObjectsAt(location, false);
			}
		}
		void OnTimerTick(object sender, EventArgs e) {
			scrollTimer.Interval = scrollInterval;
			ChartScrollParameters scrollParameters = Chart.HitTestController.FindScrollCommand(ClientPoint);
			if (scrollParameters != null && scrollParameters.Orientation == latestScrollingOrientation && scrollParameters.NavigationType == latestScrollingType) {
				focusedPane = scrollParameters.Pane;
				PerformScroll(ClientPoint, latestScrollingOrientation, latestScrollingType);
			}
			else
				focusedPane = null;
		}
		void UpdateCursor(MouseEventArgs args, Keys modifierKeys) {
			UpdateCursor(args, modifierKeys, ClientPoint);
		}
		void UpdateCursor(MouseEventArgs args, Keys modifierKeys, Point clientPoint) {
			if (!enableDragAndZoom)
				return;
			CursorType cursorType = CursorType.None;
			if (args != null && GetMouseOperation(args, modifierKeys) == MouseOperation.Drag) {
				if (currentDragCursorType == CursorType.None) {
					IAnnotationDragPoint dragPoint = FindCurrentSpecificCursorArea(args.Location) as IAnnotationDragPoint;
					currentDragCursorType = cursorType = dragPoint != null ? dragPoint.DragCursorType : cursorType = CursorType.Grab;
				}
				else
					cursorType = currentDragCursorType;
			}
			else {
				Point p = args == null ? clientPoint : args.Location;
				if (IsZoomMode(p, modifierKeys) && ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardWithMouseZooming) {
					if (Chart.CanZoomIn)
						cursorType = CursorType.ZoomIn;
					else
						cursorType = CursorType.ZoomLimit;
				}
				else if (IsZoomOutMode(p, modifierKeys) && ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardWithMouseZooming)
					cursorType = CursorType.ZoomOut;
				else if (IsDragMode(p, MouseButtons.None)) {
					if (CanDrag(p, MouseButtons.None)) {
						IAnnotationDragPoint dragPoint = FindCurrentSpecificCursorArea(p) as IAnnotationDragPoint;
						cursorType = dragPoint != null ? dragPoint.CanDragCursorType : CursorType.Hand;
					}
				}
				else if (!container.DesignMode) {
					if (cursorType == CursorType.ZoomIn || cursorType == CursorType.ZoomLimit ||
						cursorType == CursorType.Hand || cursorType == CursorType.ZoomOut || cursorType == CursorType.SizeAll) {
						ResetCursor();
						return;
					}
				}
			}
			SetCursor(cursorType);
		}
		void PerformDragging(int dx, int dy, ChartScrollEventType scrollEventType) {
			PerformDragging(-1, -1, dx, dy, scrollEventType);
		}
		void BeginDrag(Point p, NavigationState navigationState) {
			if (navigationState != NavigationState.Scrolling &&
				(this.navigationState == NavigationState.LeftButtonDragging ||
				 this.navigationState == NavigationState.MiddleButtonDragging ||
				 (!(focusedElement is IAnnotationDragElement) &&
				 !Chart.CanDrag(p, navigationState == NavigationState.MiddleButtonDragging ? MouseButtons.Middle : MouseButtons.Left))))
				return;
			try {
				container.Changing();
			}
			catch (InvalidOperationException) {
				return;
			}
			this.navigationState = navigationState;
			lastDragPoint = p;
		}
		void Drag(Point p, MouseEventArgs e) {
			if (p != lastDragPoint && CanDrag(p, e.Button) &&
				(navigationState == NavigationState.LeftButtonDragging || navigationState == NavigationState.MiddleButtonDragging)) {
				PerformDragging(
					lastDragPoint.X,
					lastDragPoint.Y,
					p.X - lastDragPoint.X,
					p.Y - lastDragPoint.Y,
					navigationState == NavigationState.MiddleButtonDragging ?
						ChartScrollEventType.MiddleButtonMouseDrag :
						ChartScrollEventType.LeftButtonMouseDrag);
				lastDragPoint = p;
			}
		}
		void EndDrag(Point location) {
			currentDragCursorType = CursorType.None;
			switch (navigationState) {
				case NavigationState.Scrolling:
				case NavigationState.LeftButtonDragging:
				case NavigationState.MiddleButtonDragging:
					ContainerAdapter.Capture = false;
					navigationState = NavigationState.Free;
					container.Changed();
					break;
				case NavigationState.ZoomingIn:
					ContainerAdapter.Capture = false;
					if (!Chart.CanZoomInViaRect || firstZoomCorner == lastZoomCorner)
						Chart.PerformZoomIn(lastZoomCorner);
					else {
						Rectangle rect = GraphicUtils.MakeRectangle(firstZoomCorner, lastZoomCorner);
						rect.Width--;
						rect.Height--;
						Chart.PerformZoomIn(rect);
					}
					firstZoomCorner = Point.Empty;
					lastZoomCorner = Point.Empty;
					navigationState = NavigationState.Free;
					break;
				case NavigationState.ZoomingOut:
					if (!location.IsEmpty) {
						ContainerAdapter.Capture = false;
						if (Chart.CanZoom(location))
							ZoomOut(location);
						navigationState = NavigationState.Free;
					}
					break;
			}
		}
		void ZoomOut(Point center) {
			Chart.PerformZoomOut(center);
		}
		void PerformScroll(Point p, ScrollingOrientation scrollingOrientation, NavigationType navigationType) {
			if (enableDragAndZoom) {
				XYDiagram2D diagram = focusedPane.Diagram;
				PaneAxesContainer paneAxesData = diagram.GetPaneAxesData(focusedPane);
				if (paneAxesData != null) {
					bool isHorizontalNavigation = (scrollingOrientation == ScrollingOrientation.AxisXScroll) ^ diagram.ActualRotated;
					Rectangle displayBounds = ContainerAdapter.DisplayBounds;
					int largeScrollStep = Math.Min(displayBounds.Width, displayBounds.Height) - 1;
					if (isHorizontalNavigation)
						switch (navigationType) {
							case NavigationType.LargeDecrement:
								paneAxesData.Scroll(largeScrollStep, 0, false, navigationType);
								break;
							case NavigationType.LargeIncrement:
								paneAxesData.Scroll(-largeScrollStep, 0, false, navigationType);
								break;
							case NavigationType.SmallDecrement:
								paneAxesData.Scroll(smallScrollStep, 0, false, navigationType);
								break;
							case NavigationType.SmallIncrement:
								paneAxesData.Scroll(-smallScrollStep, 0, false, navigationType);
								break;
							default:
								BeginDrag(p, NavigationState.Scrolling);
								ContainerAdapter.Capture = true;
								break;
						}
					else
						switch (navigationType) {
							case NavigationType.LargeDecrement:
								paneAxesData.Scroll(0, -largeScrollStep, false, navigationType);
								break;
							case NavigationType.LargeIncrement:
								paneAxesData.Scroll(0, largeScrollStep, false, navigationType);
								break;
							case NavigationType.SmallDecrement:
								paneAxesData.Scroll(0, -smallScrollStep, false, navigationType);
								break;
							case NavigationType.SmallIncrement:
								paneAxesData.Scroll(0, smallScrollStep, false, navigationType);
								break;
							default:
								BeginDrag(p, NavigationState.Scrolling);
								ContainerAdapter.Capture = true;
								break;
						}
				}
			}
		}
		void Scroll(Point p, ChartScrollParameters scrollParameters) {
			if (scrollParameters.NavigationType != NavigationType.ThumbPosition) {
				scrollTimer.Enabled = true;
				scrollTimer.Interval = firstScrollInterval;
			}
			PerformScroll(p, scrollParameters.Orientation, scrollParameters.NavigationType);
			latestScrollingOrientation = scrollParameters.Orientation;
			latestScrollingType = scrollParameters.NavigationType;
			latestRelativeScrollPosition = scrollParameters.RelativePosition;
			latestScrollRange = scrollParameters.Range;
		}
		void BeginGestureZoom(Point center, double zoomDelta) {
			Chart.BeginGestureZoom(center, zoomDelta, AnnotationDragPoint);
		}
		void PerformGestureZoom(double zoomDelta) {
			Chart.PerformGestureZoom(zoomDelta, AnnotationDragPoint);
		}
		void InitializeFocusedElement(Point p) {
			ChartFocusedArea focusedArea = Chart.HitTestController.FindFocusedArea(p);
			focusedElement = focusedArea != null ? focusedArea.Element : null;
		}
		Point GetOverPan(Point delta) {
			XYDiagramPaneBase pane = focusedElement as XYDiagramPaneBase;
			if (pane == null || delta.IsEmpty || (!pane.Diagram.ScrollingOptions.UseScrollBars && isThumbPanning))
				return Point.Empty;
			PaneAxesContainer paneAxesData = pane.Diagram.GetPaneAxesData(pane);
			if (!isThumbPanning)
				delta = new Point(-delta.X, -delta.Y);
			Axis2D axisX = (Axis2D)paneAxesData.PrimaryAxisX;
			Axis2D axisY = (Axis2D)paneAxesData.PrimaryAxisY;
			if (pane.MappingList == null)
				return Point.Empty;
			XYDiagramMappingContainer mappingContainer = pane.MappingList.FindMappingContainer(axisX, axisY);
			if (mappingContainer == null)
				return Point.Empty;
			Point p1 = DiagramToPointCalculator.CalculateCoords(pane, mappingContainer, axisX.VisualRangeData.Min, axisY.VisualRangeData.Min).Point;
			Point p2 = DiagramToPointCalculator.CalculateCoords(pane, mappingContainer, axisX.VisualRangeData.Max, axisY.VisualRangeData.Max).Point;
			Rectangle bounds = new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
			bounds.Offset(delta);
			p1 = DiagramToPointCalculator.CalculateCoords(pane, mappingContainer, axisX.WholeRangeData.Min, axisY.WholeRangeData.Min).Point;
			p2 = DiagramToPointCalculator.CalculateCoords(pane, mappingContainer, axisX.WholeRangeData.Max, axisY.WholeRangeData.Max).Point;
			Rectangle scrollingBounds = new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
			int overPanX = 0;
			int overPanY = 0;
			if (bounds.Left < scrollingBounds.Left)
				overPanX = isThumbPanning ? bounds.Left - scrollingBounds.Left : scrollingBounds.Left - bounds.Left;
			else if (bounds.Right > scrollingBounds.Right)
				overPanX = isThumbPanning ? bounds.Right - scrollingBounds.Right : scrollingBounds.Right - bounds.Right;
			if (bounds.Top < scrollingBounds.Top)
				overPanY = isThumbPanning ? bounds.Top - scrollingBounds.Top : scrollingBounds.Top - bounds.Top;
			else if (bounds.Bottom > scrollingBounds.Bottom)
				overPanY = isThumbPanning ? bounds.Bottom - scrollingBounds.Bottom : scrollingBounds.Bottom - bounds.Bottom;
			if (isThumbPanning) {
				if (latestScrollingOrientation == ScrollingOrientation.AxisXScroll ^ pane.Diagram.ActualRotated)
					return new Point(overPanX, 0);
				return new Point(0, overPanY);
			}
			return new Point(overPanX, overPanY);
		}
		MouseOperation GetMouseOperation(MouseEventArgs e, Keys modifierKeys) {
			if (!enableDragAndZoom || e == null)
				return MouseOperation.None;
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle) {
				if (IsZoomMode(e.Location, modifierKeys)) {
					if (Chart.CanZoomIn)
						return MouseOperation.ZoomIn;
					return MouseOperation.None;
				}
				if (IsZoomOutMode(e.Location, modifierKeys))
					return MouseOperation.ZoomOut;
				if ((navigationState == NavigationState.LeftButtonDragging || navigationState == NavigationState.MiddleButtonDragging ||
					  IsDragMode(e.Location, e.Button)) && (!ContainerAdapter.DragCtrlKeyRequired || (modifierKeys & Keys.Control) == Keys.Control))
					return MouseOperation.Drag;
			}
			return MouseOperation.None;
		}
		void UpdateToolTip(MouseEventArgs args) {
			if (ContainerAdapter.CanShowTooltips) {
				if (IsDragMode(args.Location, args.Button) && args.Button == MouseButtons.None && Container.ShowDesignerHints) {
					IAnnotationDragPoint dragPoint = FindCurrentSpecificCursorArea(args.Location) as IAnnotationDragPoint;
					string hint = dragPoint != null ? dragPoint.DesignerHint : Chart.GetDesignerHint(args.Location);
					if (hint != String.Empty) {
						ShowHint(hint, ContainerAdapter.PointToCanvas(args.Location));
						return;
					}
				}
				HideHint();
			}
		}
		object FindCurrentSpecificCursorArea(Point p) {
			ChartFocusedArea focusedArea = Chart.HitTestController.FindSpecificCursorArea(p);
			return focusedArea != null ? focusedArea.Element : null;
		}
		bool IsDragMode(Point p, MouseButtons mouseButtons) {
			Object element = FindCurrentSpecificCursorArea(p);
			return element != null && element is IAnnotationDragElement ||
				Chart.Diagram != null && Chart.Diagram.CanDrag(p, mouseButtons);
		}
		void UpdateCrosshair(Point cursorLocation) {
			Chart chart = Chart;
			if (chart.Diagram != null) {
				chart.Diagram.UpdateCrosshairLocation(chart.ActualCrosshairEnabled ? cursorLocation : Point.Empty);
				if (chart.ActualCrosshairEnabled)
					ContainerAdapter.Invalidate();
			}
		}
		protected abstract Cursor GetCursorByType(CursorType cursorType);
		protected abstract void ShowHint(string hing, Point point);
		protected abstract void HideHint();
		public void OnMouseLeave(EventArgs e) {
			if (ContainerAdapter.CanShowTooltips)
				HideHint();
			Chart.ClearHot();
			UpdateCrosshair(Point.Empty);
		}
		public void OnMouseMove(MouseEventArgs e) {
			OnMouseMove(e, Control.ModifierKeys);
		}
		public void OnMouseMove(MouseEventArgs e, Keys modifierKeys) {
			if (navigationState == NavigationState.Scrolling && ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseScrollBarsScrolling) {
				if (!isThumbPanning)
					ScrollingProcess(e.Location);
				return;
			}
			MouseOperation mouseOperation = GetMouseOperation(e, modifierKeys);
			Point p = new Point(e.X, e.Y);
			switch (mouseOperation) {
				case MouseOperation.Drag:
					Drag(p, e);
					break;
				case MouseOperation.ZoomIn:
					lastZoomCorner = Chart.GetZoomRegionPosition(e.Location);
					ContainerAdapter.Invalidate();
					break;
				default:
					EndDrag(Point.Empty);
					break;
			}
			UpdateCursor(e, modifierKeys);
			if (mouseOperation != MouseOperation.Drag) {
				Chart.HighlightObjectsAt(p);
				UpdateToolTip(e);
			}
			if (mouseOperation == MouseOperation.None)
				UpdateCrosshair(e.Location);
			else
				UpdateCrosshair(Point.Empty);
		}
		public void OnMouseDown(MouseEventArgs e) {
			OnMouseDown(e, Control.ModifierKeys);
		}
		public void OnMouseDown(MouseEventArgs e, Keys modifierKeys) {
			UpdateCursor(e, modifierKeys);
			UpdateToolTip(e);
			if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Middle)
				return;
			InitializeFocusedElement(e.Location);
			if (e.Button == MouseButtons.Left && Chart.Legend.UseCheckBoxes && focusedElement is LegendItemViewData) {
				LegendItemViewData legendItemVD = (LegendItemViewData)focusedElement;
				if (legendItemVD.Item is ICheckableLegendItemData && !((ICheckableLegendItemData)legendItemVD.Item).Disabled)
					legendItemVD.ChangeAppropriateObjectCheckedInLegendState();
			}
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle) {
				Chart.AnnotationNavigation.DoSelect(focusedElement as IAnnotationDragElement, container);
				switch (GetMouseOperation(e, modifierKeys)) {
					case MouseOperation.Drag:
						ContainerAdapter.Capture = true;
						BeginDrag(e.Location,
							e.Button == MouseButtons.Middle ?
								NavigationState.MiddleButtonDragging :
								NavigationState.LeftButtonDragging);
						return;
					case MouseOperation.ZoomIn:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardWithMouseZooming) {
							ContainerAdapter.Capture = true;
							firstZoomCorner = lastZoomCorner = e.Location;
							navigationState = NavigationState.ZoomingIn;
							ContainerAdapter.Invalidate();
						}
						return;
					case MouseOperation.ZoomOut:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardWithMouseZooming) {
							navigationState = NavigationState.ZoomingOut;
							ContainerAdapter.Capture = true;
						}
						return;
					default:
						EndDrag(Point.Empty);
						break;
				}
				if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseScrollBarsScrolling && e.Button == MouseButtons.Left)
					PerformScrollCommand(e.Location);
			}
		}
		public void OnMouseUp(MouseEventArgs e) {
			OnMouseUp(e, Control.ModifierKeys);
		}
		public void OnMouseUp(MouseEventArgs e, Keys modifierKeys) {
			scrollTimer.Enabled = false;
			UpdateCursor(null, modifierKeys, e.Location);
			UpdateToolTip(e);
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle) {
				InitializeFocusedElement(e.Location);
				EndDrag(e.Location);
				Chart.SelectObjectsAt(e.Location, true, modifierKeys);
			}
		}
		public void OnMouseWheel(MouseEventArgs e) {
			if (e.Delta == 0)
				return;
			if (Container.DesignMode) {
				if ((ContainerAdapter.DragCtrlKeyRequired && (Control.ModifierKeys & Keys.Control) == Keys.Control) && Chart.CanZoom(e.Location))
					Chart.Zoom(Math.Sign(e.Delta) * mouseWheelSensitivity, ZoomingKind.MouseWheel, focusedElement);
			}
			else if (Chart.CanZoom(e.Location) && ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseMouseWheelZooming)
				Chart.Zoom(Math.Sign(e.Delta) * mouseWheelSensitivity, ZoomingKind.MouseWheel, focusedElement);
		}
		public void OnKeyDown(KeyEventArgs e) {
			if (!enableDragAndZoom)
				return;
			if (e.Control) {
				switch (e.KeyCode) {
					case Keys.Z:
						if (ScrollingZoomingOptions != null) {
							Chart.UndoZoom();
							e.Handled = true;
						}
						break;
					case Keys.Add:
					case Keys.Oemplus:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardZooming) {
							Chart.Zoom(1, ZoomingKind.Keyboard, focusedElement);
							e.Handled = true;
						}
						break;
					case Keys.Subtract:
					case Keys.OemMinus:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardZooming) {
							Chart.Zoom(-1, ZoomingKind.Keyboard, focusedElement);
							e.Handled = true;
						}
						break;
					case Keys.Right:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardScrolling)
							PerformDragging(keyDraggingStep, 0, ChartScrollEventType.ArrowKeys);
						break;
					case Keys.Left:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardScrolling)
							PerformDragging(-keyDraggingStep, 0, ChartScrollEventType.ArrowKeys);
						break;
					case Keys.Up:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardScrolling)
							PerformDragging(0, -keyDraggingStep, ChartScrollEventType.ArrowKeys);
						break;
					case Keys.Down:
						if (ScrollingZoomingOptions != null && ScrollingZoomingOptions.UseKeyboardScrolling)
							PerformDragging(0, keyDraggingStep, ChartScrollEventType.ArrowKeys);
						break;
				}
			}
			else if (e.Shift || e.Alt)
				UpdateCursor(null, Control.ModifierKeys);
		}
		public void OnKeyUp(KeyEventArgs e) {
			if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
				UpdateCursor(null, Control.ModifierKeys);
		}
		public void OnCursorChanged(Cursor currentCursor) {
			if (!isCursorInitializing)
				userCursor = currentCursor;
		}
		public void DrawZoomRectangle(Graphics gr, float zoomFactor) {
			if (navigationState != NavigationState.ZoomingIn || !Chart.CanZoomInViaRect)
				return;
			PointF c1 = firstZoomCorner;
			c1.X *= zoomFactor;
			c1.Y *= zoomFactor;
			PointF c2 = lastZoomCorner;
			c2.X *= zoomFactor;
			c2.Y *= zoomFactor;
			Rectangle rect = GraphicUtils.RoundRectangle(GraphicUtils.MakeRectangle(c1, c2));
			Chart.DrawZoomRectangle(gr, rect);
		}
		public void DrawZoomRectangle(Graphics gr) {
			DrawZoomRectangle(gr, 1.0f);
		}
		public void OnGestureRotation(double degreeDelta) {
			Chart.PerformGestureRotation(degreeDelta, AnnotationDragPoint);
		}
		public void OnGestureZoom(Point center, double zoomDelta, bool isBegin) {
			if (!Chart.Diagram.CanZoomWithTouch)
				return;
			if (isBegin)
				BeginGestureZoom(center, zoomDelta);
			else
				PerformGestureZoom(zoomDelta);
		}
		public Point OnGesturePan(Point start, Point delta, bool isBegin, bool isEnd) {
			if (!Chart.Diagram.CanPan)
				return Point.Empty;
			if (isBegin)
				BeginGesturePan(start, delta);
			else if (isEnd)
				EndGesturePan(start, delta);
			else
				PerformGesturePan(start, delta);
			return overPan;
		}
	}
	public class ChartFocusedArea {
		readonly object element;
		DiagramPoint? relativePosition = null;
		public DiagramPoint? RelativePosition { get { return relativePosition; } }
		public object Element { get { return element; } }
		public ChartFocusedArea(object element) {
			this.element = element;
		}
		public ChartFocusedArea(object element, DiagramPoint? p) {
			relativePosition = p;
			this.element = element;
		}
	}
}
