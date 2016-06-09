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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts.Native {
	public class NavigationController {
		#region enums: NavigationState, NavigationAction
		enum NavigationState {
			Free,
			MouseDown,
			MouseStarting,
			MouseDragging,
			ManipulationStartingSingle,
			ManipulationStartingMultiple,
			ManipulationSingle,
			ManipulationMultiple,
			Inertia,
			MouseLeave
		}
		enum NavigationAction {
			MouseBegin,
			MouseDown,
			MouseUp,
			MouseMove,
			MouseFinish,
			MouseLeave,
			MouseWheel,
			TouchStartSingle,
			TouchStartMultiple,
			TouchChange,
			TouchFinish,
			Inertia,
			InertiaFinish
		}
		#endregion
		const int keyScrollStep = 10;
		const int mouseWheelDeltaFactor = 120;
		const int handCursorsXOffset = -11;
		const int handCursorsYOffset = -8;
		const int inertiaVelocityExpirePeriod = 100;
		const double inertialDeceleration = 0.9;
		const int inertiaAnimationInterval = 10;
		const int touchDelta = 5;
		readonly ChartControl chart;
		int mouseWheelDelta;
		Point inertiaVelocity;
		Point lastChartPosition;
		Point startZoomPoint;
		Point overPan;
		double touchZoomBeginDistance;
		bool touchUses = false;
		DateTime inertiaVelocityExpire;
		DispatcherTimer animationTimer;
		NavigationState currentState;
		bool multipleTouchStarted = false;
		Point lastTouchPoint = new Point();
		Point initialTouchPoint = new Point();
		bool popupEnabled = true;
		bool isPointerInChart = false;
		bool IsMouseAccessible { get { return (Mouse.Captured == null) || (Mouse.Captured == chart.NavigationLayer); } }
		bool IsAltKey { get { return (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt; } }
		bool IsShiftKey { get { return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift; } }
		bool IsCtrlKey { get { return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control; } }
		bool KeyboardEnabled { get { return Diagram != null && Diagram.IsKeyboardNavigationEnabled; } }
		bool MouseEnabled { get { return Diagram != null && chart.NavigationLayer != null && Diagram.IsMouseNavigationEnabled; } }
		bool ManipulationEnabled { get { return Diagram != null && chart.NavigationLayer != null && Diagram.IsManipulationNavigationEnabled; } }
		bool InteractionEnabledOnMouseUp { get { return chart.SeriesToolTipEnabled || chart.PointsToolTipEnabled || chart.SelectionMode != ElementSelectionMode.None; } }
		Diagram Diagram { get { return chart.Diagram; } }
		public NavigationController(ChartControl chart) {
			this.chart = chart;
			animationTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, inertiaAnimationInterval) };
			animationTimer.Tick += AnimationTimerTicked;
		}
		List<IManipulator> ManipulatorsList(IEnumerable<IManipulator> collection) {
			List<IManipulator> list = new List<IManipulator>();
			foreach (IManipulator manipulator in collection)
				list.Add(manipulator);
			return list;
		}
		BitmapCursor ClipCursor(BitmapCursor cursor) {
			if (cursor != null) {
				Point offset = cursor.Offset;
				Point cursorImageRightBottom = new Point(cursor.Image.PixelWidth + offset.X, cursor.Image.PixelHeight + offset.Y);
				if ((lastChartPosition.X >= -offset.X) &&
					(lastChartPosition.X <= (chart.ActualWidth - cursorImageRightBottom.X)) &&
					(lastChartPosition.Y >= -offset.Y) &&
					(lastChartPosition.Y <= (chart.ActualHeight - cursorImageRightBottom.Y)) &&
					isPointerInChart)
					return cursor;
			}
			return null;
		}
		BitmapCursor GetKeyboardCursor(bool useFocusedPane) {
			if (KeyboardEnabled) {
				if (!useFocusedPane && !Diagram.NavigationInDiagram(lastChartPosition))
					return null;
				if (IsShiftKey)
					return Diagram.NavigationCanZoomIn(lastChartPosition, useFocusedPane) ? DragCursors.ZoomInCursor : DragCursors.ZoomLimitCursor;
				if (IsAltKey)
					return Diagram.NavigationCanZoomOut(lastChartPosition, useFocusedPane) ? DragCursors.ZoomOutCursor : DragCursors.ZoomLimitCursor;
			}
			return null;
		}
		void UpdateToolTip(Point chartPosition, ToolTipNavigationAction navigationAction) {
			if (popupEnabled)
				chart.ActualToolTipController.UpdateToolTip(chartPosition, chart, navigationAction, touchUses);
		}
		void UpdateTouchState(bool newTouchState) {
			touchUses = newTouchState;
		}
		void UpdateTouchState(MouseEventArgs eventArgs) {
			if (eventArgs.StylusDevice != null)
				UpdateTouchState(true);
			else
				UpdateTouchState(false);
		}
		void UpdateCursor(Point cursorPosition) {
			BitmapCursor cursor = null;			
			switch (currentState) {
				case NavigationState.Free:
				case NavigationState.Inertia:
					cursor = GetKeyboardCursor(false);	
					if ((cursor == null) && MouseEnabled && Diagram.NavigationCanDrag(lastChartPosition, false))
						cursor = DragCursors.HandCursor;
					break;
				case NavigationState.MouseStarting:
					cursor = GetKeyboardCursor(true);
					if ((cursor == null) && MouseEnabled && Diagram.NavigationCanDrag(lastChartPosition, false))
						cursor = DragCursors.HandDragCursor;
					break;
				case NavigationState.MouseDragging:
					cursor = GetKeyboardCursor(true);
					if ((cursor == null) && MouseEnabled && Diagram.NavigationCanDrag(lastChartPosition, true))
						cursor = DragCursors.HandDragCursor;
					break;
			}
			chart.NavigationLayer.UpdateCursor(ClipCursor(cursor), cursorPosition);
		}
		void UpdateCrosshair(Point? cursorLocation) {
			Diagram diagram = Diagram;
			if (diagram != null) {
				diagram.UpdateCrosshairLocation(chart.ActualCrosshairEnabled ? cursorLocation : null);
				if (chart != null && chart.CrosshairPanel != null)
					CommonUtils.InvalidateArrange(chart.CrosshairPanel);
			}
		}
		void UpdateOnMouseUp(Point chartPosition) {
			if (!InteractionEnabledOnMouseUp)
				return;
			ToolTipHitInfo hitInfo = new ToolTipHitInfo(chart, chartPosition, true, touchUses);
			chart.ActualToolTipController.UpdateToolTip(chartPosition, chart, ToolTipNavigationAction.MouseUp, hitInfo);
			bool touchMultipleSelectionStarted = initialTouchPoint.Distance(chartPosition) < touchDelta;
			if (chartPosition == lastChartPosition || touchMultipleSelectionStarted)
				UpdateSelection(hitInfo, touchMultipleSelectionStarted);
		}
		void UpdateInertiaVelocity(Point velocity) {
			DateTime time = DateTime.Now;
			int deltaMsec = (int)(time - inertiaVelocityExpire).TotalMilliseconds;
			if ((deltaMsec > inertiaVelocityExpirePeriod) ||
				 (Math.Abs(inertiaVelocity.X * inertiaVelocity.Y) < Math.Abs(velocity.X * velocity.Y))) {
				inertiaVelocity = velocity;
				inertiaVelocityExpire = time;
			}
		}
		void AnimationTimerTicked(object sender, EventArgs args) {
			ProcessAction(NavigationAction.Inertia, null);
		}
		bool InChart(Point point) {
			return ((point.X >= 0) && (point.Y >= 0) && (point.X < chart.ActualWidth) && (point.Y < chart.ActualHeight));
		}
		bool ProcessMouseLeave(NavigationAction action, Point chartPosition, params object[] args) {
			currentState = NavigationState.Free;
			return ProcessFree(action, chartPosition, args);
		}
		bool IsPlusKey(Key key) {
			return (key == Key.Add) || (key == Key.OemPlus);
		}
		bool IsMinusKey(Key key) {
			return (key == Key.Subtract) || (key == Key.OemMinus);
		}
		bool ProcessFree(NavigationAction action, Point chartPosition, params object[] args) {
			bool touchesBegin;
			switch (action) {
				case NavigationAction.MouseBegin:
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseDown);
					if (!Diagram.NavigationBeginDrag(chartPosition, (MouseButtonEventArgs)args[0], IsShiftKey)) {
						currentState = NavigationState.MouseDown;
						return false;
					}
					chart.NavigationLayer.CaptureMouse();
					startZoomPoint = chartPosition;
					lastChartPosition = chartPosition;
					inertiaVelocity = new Point();
					inertiaVelocityExpire = DateTime.Now;
					currentState = NavigationState.MouseStarting;
					return false;
				case NavigationAction.TouchStartSingle:
					touchesBegin = Diagram.ManipulationStart(chartPosition);
					lastChartPosition = chartPosition;
					if (touchesBegin) {
						inertiaVelocity = new Point();
						inertiaVelocityExpire = DateTime.Now;
						currentState = NavigationState.ManipulationStartingSingle;
					}
					return touchesBegin;
				case NavigationAction.TouchStartMultiple:
					touchesBegin = Diagram.ManipulationStart(chartPosition);
					if (touchesBegin) {
						multipleTouchStarted = true;
						Point sndPoint = (Point)args[1];
						initialTouchPoint = lastTouchPoint = sndPoint;
						lastChartPosition = chartPosition;
						inertiaVelocity = new Point();
						inertiaVelocityExpire = DateTime.Now;
						touchZoomBeginDistance = ((Point)args[0]).Distance(sndPoint);
						currentState = NavigationState.ManipulationStartingMultiple;
					}
					return touchesBegin;
				case NavigationAction.MouseWheel:
					bool retval = false;
					mouseWheelDelta += (int)args[0];
					if (Math.Abs((int)mouseWheelDelta) >= mouseWheelDeltaFactor) {
						retval = Diagram.NavigationZoom(chartPosition, mouseWheelDelta / mouseWheelDeltaFactor, ZoomingKind.MouseWheel, false);
						mouseWheelDelta = 0;
					}
					UpdateToolTip(chartPosition, ToolTipNavigationAction.Zooming);
					return retval;
				case NavigationAction.MouseMove:
					Diagram.ProcessMouseMove(chartPosition, (MouseEventArgs)args[0]); 
					lastChartPosition = chartPosition;
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseFreeMove);
					return true;
				case NavigationAction.MouseLeave:
					currentState = NavigationState.MouseLeave;
					return true;
				case NavigationAction.MouseDown:
					currentState = NavigationState.MouseDown;
					lastChartPosition = chartPosition;
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseDown);
					return false;
				case NavigationAction.TouchFinish:
					UpdateOnMouseUp(chartPosition);
					return false;
			}
			return false;
		}
		bool ProcessMouseStarting(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.MouseMove:
					if (chartPosition == lastChartPosition)
						return true;
					currentState = NavigationState.MouseDragging;
					return ProcessMouseDragging(action, chartPosition, args);
				case NavigationAction.MouseFinish:
					if (IsShiftKey)
						Diagram.NavigationZoomIn(chartPosition);
					if (IsAltKey)
						Diagram.NavigationZoomOut();
					UpdateOnMouseUp(chartPosition);
					currentState = NavigationState.Free;
					chart.NavigationLayer.ReleaseMouseCapture();
					break;
				case NavigationAction.MouseLeave:
					currentState = NavigationState.Free;
					break;
			}
			return false;
		}
		bool ProcessMouseDragging(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.MouseMove:
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseDragging);
					if (IsAltKey)
						return true;
					MouseEventArgs e = (MouseEventArgs)args[0];
					if (IsShiftKey && Diagram.NavigationCanZoomIntoRectangle())
						Diagram.NavigationShowSelection(chartPosition, e);
					else {
						Point velocity = new Point(chartPosition.X - lastChartPosition.X, chartPosition.Y - lastChartPosition.Y);
						Diagram.NavigationDrag(chartPosition, (int)MathUtils.StrongRound(velocity.X), (int)MathUtils.StrongRound(velocity.Y), NavigationType.LeftButtonMouseDrag, e);
						UpdateInertiaVelocity(velocity);
					}
					Diagram.ProcessMouseMove(chartPosition, e);
					lastChartPosition = chartPosition;
					return true;
				case NavigationAction.MouseFinish:
					NavigationLayer navigationLayer = chart.NavigationLayer;
					if (IsShiftKey || IsAltKey) {
						if (IsShiftKey && Diagram.NavigationCanZoomIntoRectangle()) {
							Diagram.NavigationZoomIntoRectangle(new Rect(startZoomPoint, chartPosition));
							navigationLayer.HideSelection();
						}
						if (IsAltKey)
							Diagram.NavigationZoomOut();
						currentState = NavigationState.Free;
					}
					else {
						UpdateInertiaVelocity(new Point());
						animationTimer.Start();
						currentState = NavigationState.Inertia;
					}
					if (navigationLayer.IsMouseCaptured) {
						navigationLayer.ReleaseMouseCapture();
						return true;
					}
					break;
			}
			return false;
		}
		bool ProcessMouseDown(NavigationAction action, Point chartPosition, params object[] args) {
			if (action == NavigationAction.MouseFinish || action == NavigationAction.MouseUp) {
				UpdateOnMouseUp(chartPosition);
				currentState = NavigationState.Free;
			}
			return false;
		}
		bool ProcessManipulationStartingSingle(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.TouchStartMultiple:
					currentState = NavigationState.Free;
					return ProcessFree(action, chartPosition, args);
				case NavigationAction.TouchChange:
					if (chartPosition == lastChartPosition)
						return true;
					overPan = new Point();
					currentState = NavigationState.ManipulationSingle;
					return ProcessManipulationSingle(action, chartPosition, args);
				case NavigationAction.TouchFinish:
					if (!multipleTouchStarted)
						UpdateOnMouseUp(chartPosition);
					multipleTouchStarted = false;
					currentState = NavigationState.Free;
					return true;
			}
			return false;
		}
		bool ProcessManipulationStartingMultiple(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.TouchStartSingle:
					currentState = NavigationState.Free;
					UpdateOnMouseUp(lastTouchPoint);
					return ProcessFree(action, chartPosition, args);
				case NavigationAction.TouchChange:
					if (chartPosition == lastChartPosition)
						return true;
					if (args.Length > 0)
						lastTouchPoint = (Point)args[1];
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseDragging);
					currentState = NavigationState.ManipulationMultiple;
					return ProcessManipulatoinMultiple(action, chartPosition, args);
				case NavigationAction.TouchFinish:
					multipleTouchStarted = false;
					currentState = NavigationState.Free;
					return true;
			}
			return false;
		}
		bool ProcessManipulationSingle(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.TouchStartMultiple:
					currentState = NavigationState.Free;
					return ProcessFree(action, chartPosition, args);
				case NavigationAction.TouchChange:
					if (args.Length > 0) {
						currentState = NavigationState.Free;
						return ProcessFree(NavigationAction.TouchStartMultiple, chartPosition, args);
					}
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseDragging);
					Point velocity = new Point(chartPosition.X - lastChartPosition.X, chartPosition.Y - lastChartPosition.Y);
					Point over = Diagram.ManipulationDrag(velocity);
					UpdateInertiaVelocity(velocity);
					lastChartPosition = chartPosition;
					overPan.X += over.X;
					overPan.Y += over.Y;
					return true;
				case NavigationAction.TouchFinish:
					animationTimer.Start();
					currentState = NavigationState.Inertia;
					return true;
			}
			return false;
		}
		bool ProcessManipulatoinMultiple(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.TouchStartSingle:
					currentState = NavigationState.Free;
					return ProcessFree(action, chartPosition, args);
				case NavigationAction.TouchChange:
					if (args.Length == 0) {
						currentState = NavigationState.Free;
						UpdateOnMouseUp(lastTouchPoint);
						return ProcessFree(NavigationAction.TouchStartSingle, chartPosition, args);
					}
					UpdateToolTip(chartPosition, ToolTipNavigationAction.MouseDragging);
					Point sndPoint = (Point)args[1];
					double pinchScale = ((Point)args[0]).Distance(sndPoint) / touchZoomBeginDistance;
					Diagram.ManipulationZoom(pinchScale, pinchScale);
					lastTouchPoint = sndPoint;
					return true;
				case NavigationAction.TouchFinish:
					currentState = NavigationState.Free;
					return true;
			}
			return false;
		}
		bool ProcessInertia(NavigationAction action, Point chartPosition, params object[] args) {
			switch (action) {
				case NavigationAction.MouseBegin:
					animationTimer.Stop();
					currentState = NavigationState.Free;
					return ProcessFree(action, chartPosition, args);
				case NavigationAction.TouchStartSingle:
					animationTimer.Stop();
					currentState = NavigationState.Free;
					return ProcessFree(action, chartPosition, args);
				case NavigationAction.InertiaFinish:
					animationTimer.Stop();
					currentState = NavigationState.Free;
					UpdateToolTip(lastChartPosition, ToolTipNavigationAction.MouseDragEnd);
					return true;
				case NavigationAction.Inertia:
					inertiaVelocity.X *= inertialDeceleration;
					inertiaVelocity.Y *= inertialDeceleration;
					Diagram.ManipulationDrag(inertiaVelocity);
					if ((Math.Abs(inertiaVelocity.X) < 1) && (Math.Abs(inertiaVelocity.Y) < 1))
						return ProcessAction(NavigationAction.InertiaFinish, null);
					UpdateToolTip(chartPosition, ToolTipNavigationAction.Inertia);
					return true;
				case NavigationAction.MouseMove:
					Diagram.ProcessMouseMove(lastChartPosition, (MouseEventArgs)args[0]);
					lastChartPosition = chartPosition;
					return true;
			}
			return false;
		}
		bool ProcessAction(NavigationAction action, Point? chartNullablePosition, params object[] args) {
			Point chartPosition = chartNullablePosition ?? new Point(0, 0);
			bool actionResult = false;
			switch (currentState) {
				case NavigationState.MouseLeave:
					actionResult = ProcessMouseLeave(action, chartPosition, args);
					break;
				case NavigationState.Free:
					actionResult = ProcessFree(action, chartPosition, args);
					break;
				case NavigationState.MouseStarting:
					actionResult = ProcessMouseStarting(action, chartPosition, args);
					break;
				case NavigationState.MouseDragging:
					actionResult = ProcessMouseDragging(action, chartPosition, args);
					break;
				case NavigationState.ManipulationStartingSingle:
					actionResult = ProcessManipulationStartingSingle(action, chartPosition, args);
					break;
				case NavigationState.ManipulationStartingMultiple:
					actionResult = ProcessManipulationStartingMultiple(action, chartPosition, args);
					break;
				case NavigationState.ManipulationSingle:
					actionResult = ProcessManipulationSingle(action, chartPosition, args);
					break;
				case NavigationState.ManipulationMultiple:
					actionResult = ProcessManipulatoinMultiple(action, chartPosition, args);
					break;
				case NavigationState.Inertia:
					actionResult = ProcessInertia(action, chartPosition, args);
					break;
				case NavigationState.MouseDown:
					actionResult = ProcessMouseDown(action, chartPosition, args);
					break;
			}
			UpdateCursor(chartNullablePosition ?? lastChartPosition);
			return actionResult;
		}
		IList<IInteractiveElement> FindSeriesPointsByArgument(string argument) {
			List<IInteractiveElement> points = new List<IInteractiveElement>();
			if (chart.Diagram != null && chart.Diagram.Series != null) {
				foreach (Series series in chart.Diagram.Series)
					foreach (SeriesPoint point in series.Points)
						if (point.Argument == argument) {
							points.Add(point);
							break;
						}
			}
			return points;
		}
		IList<IInteractiveElement> GetSourcePoints(RefinedPoint refinedPoint) {			
			List<IInteractiveElement> sourcePoints = new List<IInteractiveElement>();
			if (refinedPoint != null) {
				if (refinedPoint.SeriesPoint is SeriesPoint)
					sourcePoints.Add((SeriesPoint)refinedPoint.SeriesPoint);
				if (refinedPoint.SeriesPoint is AggregatedSeriesPoint) {
					AggregatedSeriesPoint aggregatedPoint = refinedPoint.SeriesPoint as AggregatedSeriesPoint;
					foreach (RefinedPoint sourcePoint in aggregatedPoint.SourcePoints)
						sourcePoints.Add((SeriesPoint)sourcePoint.SeriesPoint);
				}
			}
			return sourcePoints;
		}
		internal void UpdateSelection(ToolTipHitInfo hitInfo, bool selectionStarted) {
			switch (chart.SeriesSelectionMode) {
				case SeriesSelectionMode.Series:
					chart.SelectionController.UpdateSelection(hitInfo.Series, Keyboard.Modifiers, selectionStarted);
					break;
				case SeriesSelectionMode.Point:
					chart.SelectionController.UpdateSelection(GetSourcePoints(hitInfo.RefinedPoint), Keyboard.Modifiers, selectionStarted);
					break;
				case SeriesSelectionMode.Argument:
					if (hitInfo.SeriesPoint != null) {
						IList<IInteractiveElement> points = FindSeriesPointsByArgument(hitInfo.SeriesPoint.Argument);
						chart.SelectionController.UpdateSelection(points, Keyboard.Modifiers, selectionStarted);
					}
					break;
			}
		}
		public void KeyDown(object sender, KeyEventArgs e) {
			UpdateTouchState(false);
			if (KeyboardEnabled) {
				switch (e.Key) {
					case Key.Z:
						if (IsCtrlKey) {
							Diagram.NavigationUndoZoom();
							UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
						}
						break;
					case Key.Left:
						if (IsCtrlKey && Diagram.NavigationScrollHorizontally(keyScrollStep, NavigationType.ArrowKeys)) {
							e.Handled = true;
							UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
						}
						break;
					case Key.Right:
						if (IsCtrlKey && Diagram.NavigationScrollHorizontally(-keyScrollStep, NavigationType.ArrowKeys)) {
							e.Handled = true;
							UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
						}
						break;
					case Key.Up:
						if (IsCtrlKey && Diagram.NavigationScrollVertically(keyScrollStep, NavigationType.ArrowKeys)) {
							e.Handled = true;
							UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
						}
						break;
					case Key.Down:
						if (IsCtrlKey && Diagram.NavigationScrollVertically(-keyScrollStep, NavigationType.ArrowKeys)) {
							e.Handled = true;
							UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
						}
						break;
					case Key.Tab:
						if (IsShiftKey && chart.IsKeyboardFocusWithin) {
							e.Handled = true;
							chart.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
						}
						break;
					default:
						if (IsCtrlKey) {
							if (IsPlusKey(e.Key)) {
								Diagram.NavigationZoomIn();
								UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
							}
							if (IsMinusKey(e.Key)) {
								Diagram.NavigationZoomOut();
								UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyDown);
							}
						}
						break;
				}
			}
			UpdateCursor(lastChartPosition);
		}
		public void KeyUp(object sender, KeyEventArgs e) {
			UpdateTouchState(false);
			NavigationLayer navigationLayer = chart.NavigationLayer;
			if (!IsShiftKey && navigationLayer != null && navigationLayer.IsSelectionVisible) {
				navigationLayer.HideSelection();
				UpdateToolTip(lastChartPosition, ToolTipNavigationAction.KeyUp);
			}
			UpdateCursor(lastChartPosition);
		}
		public void MouseMove(object sender, MouseEventArgs e) {
			isPointerInChart = true;
			Point point = e.GetPosition(chart);
			UpdateTouchState(e);
			if (MouseEnabled) {
				if (IsMouseAccessible)
					ProcessAction(NavigationAction.MouseMove, point, e);
			}
			else {
				UpdateCursor(point);
				UpdateToolTip(point, ToolTipNavigationAction.MouseFreeMove);				
			}
			UpdateCrosshair(point);
		}
		public void MouseWheel(object sender, MouseWheelEventArgs e) {
			UpdateTouchState(e);
			if (MouseEnabled)
				e.Handled = ProcessAction(NavigationAction.MouseWheel, e.GetPosition(chart), e.Delta);
		}
		public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			UpdateTouchState(e);
			if (chart.IsTabStop) {
				NavigationAction mouseLeftButtonDownNavigationAction = MouseEnabled ? NavigationAction.MouseBegin : NavigationAction.MouseDown;
				e.Handled = ProcessAction(mouseLeftButtonDownNavigationAction, e.GetPosition(chart), e);
			}  
		}
		public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			UpdateTouchState(e);
			Point position = e.GetPosition(chart);
			NavigationAction mouseLeftButtonUpNavigationAction = MouseEnabled ? NavigationAction.MouseFinish : NavigationAction.MouseUp;
			e.Handled = ProcessAction(mouseLeftButtonUpNavigationAction, position);
		}
		public void MouseLeave(object sender, MouseEventArgs e) {
			isPointerInChart = false;
			UpdateTouchState(e);
			Point position = e.GetPosition(chart);
			if (MouseEnabled)
				ProcessAction(NavigationAction.MouseLeave, position);
			UpdateCrosshair(null);
			UpdateToolTip(position, ToolTipNavigationAction.MouseLeave);
		}
		public void MouseEnter(object sender, MouseEventArgs e) {
			isPointerInChart = true;
			currentState = NavigationState.Free;
		}
		public void ApplicationActivated(object sender, EventArgs e) {
			chart.Dispatcher.BeginInvoke(new Action(delegate { popupEnabled = true; }));			
		}
		public void ApplicationDeactivated(object sender, EventArgs e) {
			chart.Dispatcher.BeginInvoke(new Action(delegate { 
				UpdateCrosshair(null);
				UpdateToolTip(new Point(0, 0), ToolTipNavigationAction.MouseLeave);
				popupEnabled = false;
			}));			
		}
		public void ManipulationStarted(object sender, ManipulationStartedEventArgs e) {
			if (!ManipulationEnabled) {
				lastChartPosition = e.ManipulationOrigin;
				return;
			}
			UpdateTouchState(true);
			List<IManipulator> manipulators = ManipulatorsList(e.Manipulators);
			if (manipulators.Count > 1) {
				IManipulator touchA = manipulators[0];
				IManipulator touchB = manipulators[1];
				e.Handled = ProcessAction(NavigationAction.TouchStartMultiple, e.ManipulationOrigin, touchA.GetPosition(chart), touchB.GetPosition(chart));
			}
			else {
				e.Handled = ProcessAction(NavigationAction.TouchStartSingle, e.ManipulationOrigin);
			}
		}
		public void ManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
			if (!ManipulationEnabled)
				return;
			UpdateTouchState(true);
			List<IManipulator> manipulators = ManipulatorsList(e.Manipulators);
			bool eventHandled = false;
			if (manipulators.Count > 1) {
				IManipulator touchA = manipulators[0];
				IManipulator touchB = manipulators[1];
				eventHandled = ProcessAction(NavigationAction.TouchChange, e.ManipulationOrigin, touchA.GetPosition(chart), touchB.GetPosition(chart));
			}
			else {
				IManipulator touch = manipulators[0];
				eventHandled = ProcessAction(NavigationAction.TouchChange, e.ManipulationOrigin);
				if (overPan.X != 0 || overPan.Y != 0)
					e.ReportBoundaryFeedback(new ManipulationDelta(new Vector(overPan.X, overPan.Y), 0, new Vector(), new Vector()));
			}
			e.Handled = eventHandled;
		}
		public void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			UpdateTouchState(true);
			ProcessAction(NavigationAction.TouchFinish, e.ManipulationOrigin);
		}
	}
}
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_Selection", Type = typeof(ContentPresenter)),
	TemplatePart(Name = "PART_Cursor", Type = typeof(Image)),
	NonCategorized
	]
	public partial class NavigationLayer : Control {
		public static readonly DependencyProperty ChartControlProperty = DependencyPropertyManager.Register("ChartControl", typeof(ChartControl), typeof(NavigationLayer));
		static NavigationLayer() {
			FocusableProperty.OverrideMetadata(typeof(NavigationLayer), new FrameworkPropertyMetadata(false));
		}
		Rect selectionRect;
		Cursor chartCursor;
		Cursor afterQuerryChartCursor;
		ContentPresenter selectionBorder;
		Image cursorImage;
		public bool IsSelectionVisible { get { return !selectionRect.IsEmpty; } }
		public ChartControl ChartControl {
			get { return (ChartControl)GetValue(ChartControlProperty); }
			set { SetValue(ChartControlProperty, value); }
		}
		public NavigationLayer() : base() {
			DefaultStyleKey = typeof(NavigationLayer);
		}
		public void ShowSelection(Rect rect) {
			selectionRect = rect;
			InvalidateArrange();
		}
		public void HideSelection() {
			selectionRect = new Rect();
			InvalidateArrange();
		}
		public void UpdateCursor(BitmapCursor bitmapCursor, Point position) {
			if (cursorImage != null && ChartControl != null) {
				if (bitmapCursor != null && ChartControl.Cursor != afterQuerryChartCursor)
					chartCursor = ChartControl.Cursor;				
				Cursor cursor = bitmapCursor != null ? Cursors.None : chartCursor;
				QueryChartCursorEventArgs args;
				if (bitmapCursor != null)
					args = new QueryChartCursorEventArgs(ChartControl.QueryChartCursorEvent, cursor, bitmapCursor.Image, position, bitmapCursor.Offset);
				else
					args = new QueryChartCursorEventArgs(ChartControl.QueryChartCursorEvent, cursor, null, position, new Point(0, 0));
				ChartControl.RaiseEvent(args);
				if (args.Cursor != Cursors.None) {
					cursorImage.Source = null;
					cursorImage.Visibility = Visibility.Collapsed;					
				}
				else {
					cursorImage.Source = args.CursorImage;
					cursorImage.Visibility = Visibility.Visible;
					Canvas.SetLeft(cursorImage, args.Position.X + args.CursorImageOffset.X);
					Canvas.SetTop(cursorImage, args.Position.Y + args.CursorImageOffset.Y);
				}
				afterQuerryChartCursor = args.Cursor;
				ChartControl.Cursor = args.Cursor;
			}
		}	   
		protected override Size MeasureOverride(Size constraint) {
			if (selectionBorder != null)
				selectionBorder.Measure(new Size(selectionRect.Width, selectionRect.Height));
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			base.ArrangeOverride(arrangeBounds);
			if (selectionBorder != null)
				selectionBorder.Arrange(selectionRect);
			return arrangeBounds;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			selectionBorder = GetTemplateChild("PART_Selection") as ContentPresenter;
			cursorImage = GetTemplateChild("PART_Cursor") as Image;
		}
	}
}
