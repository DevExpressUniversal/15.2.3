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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public enum RangeControlStateType { Normal, MoveSelection, Zoom, Scrolling, Selection, Click, ThumbDragging, DoubleClick, ScrollingOrDragging, Holding }
	public enum RangeControlHitTestType { None, ScrollableArea, SelectionArea, LabelArea, ThumbsArea }
	public enum SelectionResizeDirection { None, Left, Right }
	enum InputDeviceType { Mouse, Touch }
	internal static class ChangeCursorHelper {
		public static void ResetCursorToDefault() {
			SetCursor(null);
		}
		public static void SetHandCursor() {
			SetCursor(Cursors.Hand);
		}
		public static void SetResizeCursor() {
			SetCursor(Cursors.SizeWE);
		}
		private static void SetCursor(Cursor cursor) {
			if (Mouse.OverrideCursor != cursor) Mouse.OverrideCursor = cursor;
		}
	}
	public class ClickHelper {
		const double MinDoubleClickTime = 1000d;
		const double MinTouchWidth = 10d;
		public ClickHelper(FrameworkElement inputContainer) {
			InputContainer = inputContainer;
			SubcribeInputEvents();
		}
		public event EventHandler<InputEventArgs> Click;
		public event EventHandler<InputEventArgs> DoubleClick;
		Point CurrentClickPosition { get; set; }
		Point LastClickPosition { get; set; }
		DateTime LastClickTime { get; set; }
		FrameworkElement InputContainer { get; set; }
		private void RaiseClick(InputEventArgs args) {
			if (Click != null)
				this.Click(this, args);
		}
		private void RaiseDoubleClick(InputEventArgs args) {
			if (DoubleClick != null)
				this.DoubleClick(this, args);
		}
		private void SubcribeInputEvents() {
			InputContainer.MouseUp += MouseUp;
			InputContainer.TouchUp += TouchUp;
		}
		void TouchUp(object sender, TouchEventArgs e) {
			DetectInput(e, false);
		}
		private void MouseUp(object sender, MouseButtonEventArgs e) {
			CurrentClickPosition = e.GetPosition(InputContainer);
			DetectInput(e, true);
		}
		private void DetectInput(InputEventArgs e, bool isMouse) {
			InitLastClickPosition();
			bool isDouble = isMouse ? IsDoubleClick() : IsDoubleTap();
			if (isDouble) RaiseDoubleClick(e);
			else RaiseClick(e);
			LastClickPosition = CurrentClickPosition;
		}
		private bool IsDoubleTap() {
			double xDelta = Math.Abs(CurrentClickPosition.X - LastClickPosition.X);
			double yDelta = Math.Abs(CurrentClickPosition.Y - LastClickPosition.Y);
			return xDelta < MinTouchWidth && yDelta < MinTouchWidth;
		}
		private bool IsDoubleClick() {
			return IsMinTime() && LastClickPosition == CurrentClickPosition;
		}
		private void InitLastClickPosition() {
			if (LastClickPosition == new Point()) {
				LastClickPosition = CurrentClickPosition;
				LastClickTime = DateTime.Now;
			}
		}
		private bool IsMinTime() {
			return (DateTime.Now - LastClickTime).TotalMilliseconds < MinDoubleClickTime;
		}
	}
	public class RangeControlController {
		const double MinManipulationDelta = 4d;
		const double MinTouchWidth = 10d;
		const double Deceleration = 0.001d;
		const double MinExpansionDelta = 0.2;
		public RangeControlController(FrameworkElement clientContainer, RangeControl rangeControl, FrameworkElement manipulationContainer) {
			this.ClientContainer = clientContainer;
			this.Owner = rangeControl;
			this.ManipulationContainer = manipulationContainer;
			Stylus.SetIsPressAndHoldEnabled(ClientContainer, false);
			SubscribeEvents();
		}
		public RangeControl Owner { get; private set; }
		public RangeControlStateType State { get; private set; }
		public RangeControlHitTestType HitTestType { get; private set; }
		FrameworkElement ClientContainer { get; set; }
		FrameworkElement ManipulationContainer { get; set; }
		bool IsHolding { get; set; }
		bool IsDragStarted { get; set; }
		DateTime LastClickTime { get; set; }
		int MinTapTime { get { return 300; } }
		int HoldingDelay { get { return 150; } }
		double MoveDelta { get; set; }
		bool IsStopIntertia { get; set; }
		bool HasActiveManipulation { get; set; }
		InputDeviceType ActiveDevice { get; set; }
		bool isAutoScrollInProcess;
		bool IsAutoScrollInProcess {
			get { return isAutoScrollInProcess; }
			set {
				if (isAutoScrollInProcess != value) {
					isAutoScrollInProcess = value;
					if (!value) Owner.StopAutoScroll();
				}
			}
		}
		public void StopInetria() {
			IsStopIntertia = true;
		}
		private void SubscribeEvents() {
			ClientContainer.MouseDown += MouseDown;
			ClientContainer.MouseUp += MouseUp;
			ClientContainer.MouseMove += MouseMove;
			ClientContainer.MouseWheel += MouseWheel;
			ClientContainer.TouchDown += TouchDown;
			ClientContainer.TouchUp += TouchUp;
			ClientContainer.TouchMove += TouchMove;
			ClientContainer.LostMouseCapture += LostMouseCapture;
			ClientContainer.MouseLeave += MouseLeave;
			ManipulationContainer.ManipulationDelta += ManipulationDelta;
			ManipulationContainer.ManipulationInertiaStarting += ManipulationInertiaStarting;
			ManipulationContainer.ManipulationCompleted += ManipulationCompleted;
			ManipulationContainer.ManipulationStarting += ManipulationStarting;
		}
		internal void ManipulationStarting(object sender, ManipulationStartingEventArgs e) {
			e.ManipulationContainer = ManipulationContainer;
			e.Mode = ManipulationModes.Translate | ManipulationModes.Scale;
			e.Handled = true;
		}
		internal void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			e.Handled = true;
			if (e.IsInertial && e.FinalVelocities.LinearVelocity.X != 0 && State == RangeControlStateType.Scrolling) e.Cancel();
			else ResetCore();
		}
		internal void ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e) {
			e.Handled = true;
			e.TranslationBehavior.DesiredDeceleration = Deceleration;
		}
		internal void ManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
			e.Handled = true;
			HasActiveManipulation = true;
			if (!CanProcessManipulationDelta(e) || IsAutoScrollInProcess) return;
			StopHoldingTimer();
			ProcessManipulationDelta(e);
		}
		internal void ProcessManipulationDelta(ManipulationDeltaEventArgs e) {
			if (e.IsInertial && CanStopInertia()) {
				e.Complete();
				return;
			}
			ProcessDelta(e);
		}
		private bool CanStopInertia() {
			return IsStopIntertia || State != RangeControlStateType.Scrolling;
		}
		private void ProcessDelta(ManipulationDeltaEventArgs e) {
			double delta = e.DeltaManipulation.Translation.X;
			switch (State) {
				case RangeControlStateType.Scrolling:
					Owner.ScrollByDelta(-delta);
					break;
				case RangeControlStateType.MoveSelection:
					if (!delta.AreClose(0d))
						Owner.MoveSelection(delta);
					break;
				case RangeControlStateType.Selection:
					Owner.ResizeSelection(e.ManipulationOrigin.X, true);
					break;
				case RangeControlStateType.Zoom:
					ProcessZoom(e);
					break;
			}
		}
		private void ProcessZoom(ManipulationDeltaEventArgs e) {
			double newExpansion = e.DeltaManipulation.Expansion.X;
			OldExpansion = OldExpansion == 0 ? newExpansion : OldExpansion;
			if (CanProcessZoom(newExpansion)) {
				double deltaX = e.DeltaManipulation.Scale.X;
				double deltaY = e.DeltaManipulation.Scale.Y;
				double delta = (deltaX + deltaY);
				Owner.ZoomByPinch(delta, e.ManipulationOrigin.X);
			}
			OldExpansion = newExpansion;
		}
		double CummulativeDelta = 0;
		private bool CanProcessManipulationDelta(ManipulationDeltaEventArgs e) {
			if (HitTestType == RangeControlHitTestType.ThumbsArea) return false;
			CummulativeDelta += Math.Abs(e.DeltaManipulation.Translation.X);
			return CummulativeDelta > MinManipulationDelta;
		}
		double OldExpansion = 0;
		private bool CanProcessZoom(double newExpansion) {
			return Math.Abs(newExpansion) > MinExpansionDelta && OldExpansion != newExpansion;
		}
		void MouseWheel(object sender, MouseWheelEventArgs e) {
			ProcessWheel(e);
		}
		void MouseLeave(object sender, MouseEventArgs e) {
			ResetCursor();
		}
		private void ResetCursor() {
			ChangeCursorHelper.ResetCursorToDefault();
		}
		void LostMouseCapture(object sender, MouseEventArgs e) {
			ResetCursor();
		}
		private void ProcessWheel(MouseWheelEventArgs e) {
			Owner.ZoomByWheel(e.Delta, e.GetPosition(ClientContainer).X);
		}
		private void TouchMove(object sender, TouchEventArgs e) {
			double position = e.GetTouchPoint(Owner).Position.X;
			UpdateMoveDelta(position);
			DetectAutoScroll(position);
			if (State == RangeControlStateType.ThumbDragging) {
				StopHoldingTimer();
				DetectAutoScroll(position);
				if (IsMoveDeltaChanged() && !IsAutoScrollInProcess)
					ProcessDragging(e.GetTouchPoint(ClientContainer).Position, e.Device);
			}
		}
		Locker prepareSelectionLocker = new Locker();
		void MouseMove(object sender, MouseEventArgs e) {
			double position = e.GetPosition(Owner).X;
			UpdateCursor(e.GetPosition(ClientContainer));
			UpdateMoveDelta(position);
			if (e.LeftButton == MouseButtonState.Pressed) {
				DetectAutoScroll(position);
				if (IsMoveDeltaChanged() && !IsAutoScrollInProcess)
					ProcessMouseMove(e);
			}
		}
		private void UpdateCursor(Point position) {
			if(State == RangeControlStateType.MoveSelection) return;
			if (Owner.HitTest(position) == RangeControlHitTestType.ThumbsArea) ChangeCursorHelper.SetResizeCursor();
			else ResetCursor();
		}
		private void DetectAutoScroll(double position) {
			if (!Owner.AllowScroll) return;
			bool isAutoScrollDetected = Owner.IsPositionOutOfBounds(position);
			if (isAutoScrollDetected && !IsAutoScrollInProcess)
				ProcessAutoScroll(position);
			IsAutoScrollInProcess = isAutoScrollDetected;
		}
		private void ProcessAutoScroll(double position) {
			if (State == RangeControlStateType.ThumbDragging || State == RangeControlStateType.Selection)
				Owner.StartAutoScroll(position, true);
			else if (State == RangeControlStateType.MoveSelection)
				Owner.StartAutoScroll(position, false);
		}
		private void UpdateMoveDelta(double newPosition) {
			MoveDelta = CalcMoveDelta(newPosition);
		}
		private bool IsMoveDeltaChanged() {
			return !MoveDelta.AreClose(0d);
		}
		private void ProcessMouseMove(MouseEventArgs e) {
			switch (State) {
				case RangeControlStateType.ThumbDragging:
					ProcessDragging(e.GetPosition(ClientContainer), e.Device);
					break;
				case RangeControlStateType.MoveSelection:
					if (!prepareSelectionLocker.IsLocked) {
						ChangeCursorHelper.SetHandCursor();
						prepareSelectionLocker.Lock();
						Owner.PrepareMoveSelection();
					}
					Owner.MoveSelection(MoveDelta);
					break;
				case RangeControlStateType.Selection:
					Owner.ResizeSelection(e.GetPosition(ClientContainer).X);
					break;
			}
		}
		private void StopHoldingTimer() {
			if (holdingTimer != null) {
				holdingTimer.Stop();
				holdingTimer = null;
			}
		}
		double lastMovePosition = double.NaN;
		private double CalcMoveDelta(double position) {
			if (double.IsNaN(lastMovePosition))
				lastMovePosition = position;
			double delta = position - lastMovePosition;
			lastMovePosition = position;
			return delta;
		}
		private void ProcessDragging(Point position, InputDevice device) {
			double time = (DateTime.Now - LastClickTime).TotalMilliseconds;
			if (HitTestType == RangeControlHitTestType.ThumbsArea && activeDevices.Count > 0 && !IsHolding && time > MinTapTime) {
				if (!IsDragStarted) InitializeDragging(position, device);
				if (device == draggingDevice)
					Owner.ProcessSelectionResizing(position.X);
			}
		}
		InputDevice draggingDevice;
		private void InitializeDragging(Point position, InputDevice device) {
			State = RangeControlStateType.ThumbDragging;
			Owner.ThumbDragStarted(currentDownPosition, position.X - currentDownPosition.X);
			IsDragStarted = true;
			draggingDevice = device;
		}
		DateTime lastClickTime;
		Point lastClickPosition;
		void TouchUp(object sender, TouchEventArgs e) {
			ProcessUp(e.GetTouchPoint(ClientContainer).Position, e.Device, false);
			ClientContainer.ReleaseTouchCapture(e.TouchDevice);
			if (!HasActiveManipulation)
				ResetCore();
		}
		void MouseUp(object sender, MouseButtonEventArgs e) {
			ProcessUp(e.GetPosition(ClientContainer), e.Device, true);
			ClientContainer.ReleaseMouseCapture();
			ResetCore();
		}
		private void ProcessUp(Point position, InputDevice device, bool isMouse) {
			if (State != RangeControlStateType.Zoom && State != RangeControlStateType.Normal) {
				if (IsDoubleClick(position, isMouse)) ProcessDoubleClick();
				else if (CanProcessClick(position)) ProcessClick();
			}
			RemoveInputDevice(device);
		}
		private bool CanProcessClick(Point upPosition) {
			return (currentDownPosition == upPosition) && !IsHolding;
		}
		private bool IsDoubleClick(Point clickPosition, bool isMouse) {
			DateTime clickTime = DateTime.Now;
			bool isPositionsEquals = isMouse ? IsMouseClickPositionsEquals(clickPosition, lastClickPosition) : IsTouchClickPositions(clickPosition, lastClickPosition);
			bool isDoubleClick = ((clickTime - lastClickTime).TotalMilliseconds < 1000 && isPositionsEquals);
			lastClickTime = clickTime;
			lastClickPosition = clickPosition;
			return isDoubleClick;
		}
		private bool IsTouchClickPositions(Point current, Point last) {
			double xDelta = Math.Abs(current.X - last.X);
			double yDelta = Math.Abs(current.Y - last.Y);
			return xDelta < MinTouchWidth && yDelta < MinTouchWidth;
		}
		private bool IsMouseClickPositionsEquals(Point current, Point last) {
			return last == current;
		}
		Point currentDownPosition;
		List<InputDevice> activeDevices = new List<InputDevice>();
		void TouchDown(object sender, TouchEventArgs e) {
			ActiveDevice = InputDeviceType.Touch;
			if (State == RangeControlStateType.ThumbDragging) return;
			ClientContainer.CaptureTouch(e.TouchDevice);
			ProcessDown(e.GetTouchPoint(ClientContainer).Position, e.Device);
		}
		void MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ActiveDevice = InputDeviceType.Mouse;
			if (State == RangeControlStateType.ThumbDragging) return;
			ClientContainer.CaptureMouse();
			ProcessDown(e.GetPosition(ClientContainer), e.Device);
		}
		private void ProcessDown(Point position, InputDevice device) {
			AddInputDevice(device);
			currentDownPosition = position;
			SetUpCurrentState();
			if (ActiveDevice == InputDeviceType.Touch) StartHoldingTimer();
			else PrepareResizeSelection();
		}
		private void PrepareResizeSelection() {
			if (State == RangeControlStateType.Selection) {
				Owner.SelectByHitTest();
				Owner.PrepareResizeSelection();
			}
		}
		DispatcherTimer holdingTimer;
		private void StartHoldingTimer() {
			StopHoldingTimer();
			holdingTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, HoldingDelay) };
			holdingTimer.Tick += (s, e) => {
				holdingTimer.Stop();
				ProcessHolding();
			};
			holdingTimer.Start();
		}
		void AddInputDevice(InputDevice device) {
			activeDevices.Add(device);
		}
		void RemoveInputDevice(InputDevice device) {
			if (activeDevices.Contains(device))
				activeDevices.Remove(device);
		}
		private void SetUpCurrentState() {
			UpdateHitTest();
			if (activeDevices.Count > 1) {
				State = RangeControlStateType.Zoom;
				StopHoldingTimer();
				Owner.PrepareZoom();
			} else {
				if (HitTestType == RangeControlHitTestType.ThumbsArea) State = RangeControlStateType.ThumbDragging;
				else State = ActiveDevice == InputDeviceType.Touch ? RangeControlStateType.Scrolling : GetStateFromHitTest();
			}
		}
		private RangeControlStateType GetStateFromHitTest() {
			return HitTestType == RangeControlHitTestType.SelectionArea ? RangeControlStateType.MoveSelection : RangeControlStateType.Selection;
		}
		private void UpdateHitTest() {
			HitTestType = Owner.HitTest(currentDownPosition);
		}
		private void ProcessDoubleClick() {
			State = RangeControlStateType.DoubleClick;
			Owner.ZoomByDoubleTap(currentDownPosition);
		}
		private void ProcessClick() {
			State = RangeControlStateType.Click;
			LastClickTime = DateTime.Now;
			if (HitTestType == RangeControlHitTestType.LabelArea) Owner.SelectGroupInterval();
			else Owner.SelectByHitTest();
		}
		private void ProcessHolding() {
			if (HitTestType != RangeControlHitTestType.LabelArea) {
				IsHolding = true;
				if (Owner.IsInsideSelectionArea(currentDownPosition)) ProcessHoldInsideSelection();
				else ProcessHoldOutsideSelection();
			} else {
				State = RangeControlStateType.Selection;
				Owner.SelectGroupInterval();
			}
		}
		private void ProcessHoldOutsideSelection() {
			State = RangeControlStateType.Selection;
			HitTestType = RangeControlHitTestType.ScrollableArea;
			Owner.SelectByHitTest();
			Owner.PrepareResizeSelection();
		}
		private void ProcessHoldInsideSelection() {
			State = RangeControlStateType.MoveSelection;
			HitTestType = RangeControlHitTestType.SelectionArea;
			Owner.PrepareMoveSelection();
		}
		public void Reset() {
			ResetCore();
		}
		private void ResetCore() {
			Owner.OnControllerReset(State);
			ChangeCursorHelper.ResetCursorToDefault();
			IsAutoScrollInProcess = false;
			State = RangeControlStateType.Normal;
			StopHoldingTimer();
			prepareSelectionLocker.Unlock();
			IsHolding = false;
			IsStopIntertia = false;
			HasActiveManipulation = true;
			OldExpansion = 0;
			CummulativeDelta = 0;
			IsDragStarted = false;
			MoveDelta = 0;
			lastMovePosition = double.NaN;
		}
	}
}
