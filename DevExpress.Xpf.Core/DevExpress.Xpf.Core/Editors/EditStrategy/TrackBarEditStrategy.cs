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
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors {
	public class TrackBarEditStrategy : RangeEditStrategyBase {
		readonly int magicRoundNumber = 10;
		protected internal override bool IsLockedByValueChanging { get { return base.IsLockedByValueChanging || selectionStartLocker.IsLocked || selectionEndLocker.IsLocked; } }
		new TrackBarEdit Editor { get { return base.Editor as TrackBarEdit; } }
		new TrackBarEditPropertyProvider PropertyProvider { get { return Editor.PropertyProvider as TrackBarEditPropertyProvider; } }
		ButtonBase LeftStepButton { get; set; }
		ButtonBase RightStepButton { get; set; }
		Thumb Thumb { get; set; }
		Thumb LeftThumb { get; set; }
		Thumb RightThumb { get; set; }
		Thumb CenterThumb { get; set; }
		ButtonBase LeftSideButton { get; set; }
		ButtonBase RightSideButton { get; set; }
		new TrackBarEditInfo Info { get { return base.Info as TrackBarEditInfo; } }
		ReservedSpaceCalculator ReservedSpaceCalculator { get; set; }
		readonly PostponedAction updateThumbPositionAction;
		public TrackBarEditStrategy(TrackBarEdit editor)
			: base(editor) {
			ReservedSpaceCalculator = new ReservedSpaceCalculator();
			updateThumbPositionAction = new PostponedAction(ShouldPostponeUpdateThumbPosition);
		}
		protected virtual bool ShouldPostponeUpdateThumbPosition() {
#if SL
			return true;
#else
			return false;
#endif
		}
		protected internal override void OnApplyTemplate() {
			base.OnApplyTemplate();
			FindElements();
			UpdatePositions();
		}
		protected override RangeEditBaseInfo CreateEditInfo() {
			return new TrackBarEditInfo();
		}
		#region property synchronization
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(RangeBaseEdit.ValueProperty, baseValue => baseValue, baseValue => Editor.IsRange ? 0d : ObjectToDoubleConverter.TryConvert(baseValue));
			PropertyUpdater.Register(TrackBarEdit.SelectionStartProperty, baseValue => baseValue, UpdateSelectionStartInternal);
			PropertyUpdater.Register(TrackBarEdit.SelectionEndProperty, baseValue => baseValue, UpdateSelectionEndInternal);
		}
		object GetEditValueFromSelectionStart(object selectionStart) {
			if(!Editor.IsRange)
				return selectionStart;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			range.SelectionStart = ToRange((double)selectionStart);
			return range;
		}
		object GetEditValueFromSelectionEnd(object selectionEnd) {
			if(!Editor.IsRange)
				return selectionEnd;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			range.SelectionEnd = ToRange((double)selectionEnd);
			return range;
		}
		object UpdateSelectionStartInternal(object value) {
			TrackBarEditRange range = value as TrackBarEditRange;
			if(range == null) {
				double result = ObjectToDoubleConverter.TryConvert(value);
				range = new TrackBarEditRange() { SelectionStart = result, SelectionEnd = result };
			} 
			return Editor.IsRange ? range.SelectionStart : 0d;
		}
		object UpdateSelectionEndInternal(object value) {
			TrackBarEditRange range = value as TrackBarEditRange;
			if(range == null) {
				double result = ObjectToDoubleConverter.TryConvert(value);
				range = new TrackBarEditRange() { SelectionStart = result, SelectionEnd = result };
			}
			return Editor.IsRange ? range.SelectionEnd : 0d;
		}
		#endregion
		public virtual void IncrementLarge() {
			Increment(Editor.LargeStep);
		}
		public virtual void DecrementLarge() {
			Decrement(Editor.LargeStep);
		}
		void FindElements() {
			FindButtons();
			FindThumb();
#if !SL
			UpdateReservedSpaceCalculator();
#endif
		}
		void UpdateReservedSpaceCalculator() {
			ReservedSpaceCalculator.ClearValue(ReservedSpaceCalculator.ThumbLengthProperty);
			ReservedSpaceCalculator.ClearValue(ReservedSpaceCalculator.LeftThumbLengthProperty);
			ReservedSpaceCalculator.ClearValue(ReservedSpaceCalculator.RightThumbLengthProperty);
			if(Thumb != null)
				BindingOperations.SetBinding(ReservedSpaceCalculator, ReservedSpaceCalculator.ThumbLengthProperty, CreateThumbLengthBinding(Thumb));
			if(LeftThumb != null)
				BindingOperations.SetBinding(ReservedSpaceCalculator, ReservedSpaceCalculator.LeftThumbLengthProperty, CreateThumbLengthBinding(LeftThumb));
			if(RightThumb != null)
				BindingOperations.SetBinding(ReservedSpaceCalculator, ReservedSpaceCalculator.RightThumbLengthProperty, CreateThumbLengthBinding(RightThumb));
			BindingOperations.SetBinding(Info, TrackBarEditInfo.ReservedSpaceProperty, new Binding() {
				Source = ReservedSpaceCalculator,
				Path = new PropertyPath(ReservedSpaceCalculator.ReservedSpaceProperty),
				Mode = BindingMode.OneWay
			});
		}
		Binding CreateThumbLengthBinding(Thumb thumb) {
			Binding binding = new Binding();
			binding.Path = new PropertyPath(Editor.Orientation == Orientation.Horizontal ? FrameworkElement.ActualWidthProperty : FrameworkElement.ActualHeightProperty);
			binding.Source = thumb;
			binding.Mode = BindingMode.OneWay;
			return binding;
		}
		void FindButtons() {
			LeftStepButton = LayoutHelper.FindElementByName(Editor, "PART_LeftStepButton") as ButtonBase;
			RightStepButton = LayoutHelper.FindElementByName(Editor, "PART_RightStepButton") as ButtonBase;
			LeftSideButton = LayoutHelper.FindElementByName(Editor, "left") as ButtonBase;
			RightSideButton = LayoutHelper.FindElementByName(Editor, "right") as ButtonBase;
		}
		void FindThumb() {
			UnsubscribeEvents();
			Thumb = LayoutHelper.FindElementByName(Editor, "PART_Thumb") as Thumb;
			LeftThumb = LayoutHelper.FindElementByName(Editor, "PART_LeftThumb") as Thumb;
			RightThumb = LayoutHelper.FindElementByName(Editor, "PART_RightThumb") as Thumb;
			CenterThumb = LayoutHelper.FindElementByName(Editor, "PART_CenterThumb") as Thumb;
			SubscribeEvents();
		}
		void UnsubscribeEvents() {
			if(Thumb != null) {
				Thumb.DragStarted -= new DragStartedEventHandler(Thumb_DragStarted);
				Thumb.DragCompleted -= new DragCompletedEventHandler(Thumb_DragCompleted);
				Thumb.DragDelta -= new DragDeltaEventHandler(Thumb_DragDelta);
			}
			if(LeftThumb != null) {
				LeftThumb.DragStarted -= new DragStartedEventHandler(Thumb_DragStarted);
				LeftThumb.DragCompleted -= new DragCompletedEventHandler(Thumb_DragCompleted);
				LeftThumb.DragDelta -= new DragDeltaEventHandler(LeftThumb_DragDelta);
			}
			if(RightThumb != null) {
				RightThumb.DragStarted -= new DragStartedEventHandler(Thumb_DragStarted);
				LeftThumb.DragCompleted -= new DragCompletedEventHandler(Thumb_DragCompleted);
				RightThumb.DragDelta -= new DragDeltaEventHandler(RightThumb_DragDelta);
			}
		}
		void SubscribeEvents() {
			if(Thumb != null) {
				Thumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				Thumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
				Thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
			}
			if(LeftThumb != null) {
				LeftThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				LeftThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
				LeftThumb.DragDelta += new DragDeltaEventHandler(LeftThumb_DragDelta);
			}
			if(RightThumb != null) {
				RightThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				LeftThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
				RightThumb.DragDelta += new DragDeltaEventHandler(RightThumb_DragDelta);
			}
			if (CenterThumb != null) {
				CenterThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
				CenterThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
				CenterThumb.DragDelta += new DragDeltaEventHandler(CenterThumb_DragDelta);
			}
		}
		void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) {
#if SL
			((FrameworkElement)sender).MouseMove -= new MouseEventHandler(TrackBarEditStrategy_MouseMove);
			mouseHelper = new MouseEventArgsHelper();
			Editor.ReleaseMouseCapture();
#endif
		}
		void Thumb_DragStarted(object sender, DragStartedEventArgs e) {
#if SL
			Editor.CaptureMouse();
			mouseHelper = new MouseEventArgsHelper();
			((FrameworkElement)sender).MouseMove += new MouseEventHandler(TrackBarEditStrategy_MouseMove);
#endif
			prevDragPoint = GetMousePosition(Editor);
		}
#if SL
		MouseEventArgsHelper mouseHelper = new MouseEventArgsHelper();
		void TrackBarEditStrategy_MouseMove(object sender, MouseEventArgs e) {
			mouseHelper = new MouseEventArgsHelper(e);
			Editor.UpdateLayout();
			updateThumbPositionAction.PerformForce();
		}
#endif
		Locker dragLocker = new Locker();
		double prevDragPoint;
		void Thumb_DragDelta(object sender, DragDeltaEventArgs e) {
			if (!Editor.BeginInplaceEditing()) return;
			updateThumbPositionAction.PerformPostpone(() => {
				double value = ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue);
				double delta = GetDelta(Thumb);
				if (Math.Abs(delta) > 0)
					ValueContainer.SetEditValue(ToRange(value + delta), UpdateEditorSource.TextInput);
			});
		}
		double GetDelta(Thumb thumb) {
			return Editor.IsSnapToTickEnabled ? GetSnapToTicksDelta(thumb) : GetNormalDelta(thumb);
		}
		double GetSnapToTicksDelta(Thumb thumb) {
			double currentDragPosition = GetMousePosition(Editor);
			double delta = GetMousePosition(thumb) - GetOrientationSign() * GetLength(thumb) / 2.0;
			int dragSign = Math.Sign(currentDragPosition - prevDragPoint);
			prevDragPoint = currentDragPosition;
			return dragSign != Math.Sign(delta) ? 0d : GetSmallStepCount(NormalizeByLength(delta)) * Editor.SmallStep;
		}
		double GetNormalDelta(Thumb thumb) {
			double currentDragPosition = GetMousePosition(Editor);
			double delta = GetMousePosition(thumb) - GetOrientationSign() * GetLength(thumb) / 2.0;
			int dragSign = Math.Sign(currentDragPosition - prevDragPoint);
			prevDragPoint = currentDragPosition;
			return dragSign != Math.Sign(delta) ? 0d : NormalizeByLength(delta) * (Editor.Maximum - Editor.Minimum);
		}
		void LeftThumb_DragDelta(object sender, DragDeltaEventArgs e) {
			if (!Editor.BeginInplaceEditing()) return;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			double delta = GetDelta(LeftThumb);
			if(Math.Abs(delta) > 0) {
				double value = range.SelectionStart + delta;
				if(value > range.SelectionEnd)
					value = range.SelectionEnd;
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = ToRange(value), SelectionEnd = range.SelectionEnd }, UpdateEditorSource.TextInput);
			}
		}
		void RightThumb_DragDelta(object sender, DragDeltaEventArgs e) {
			if (!Editor.BeginInplaceEditing()) return;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			double delta = GetDelta(RightThumb);
			if(Math.Abs(delta) > 0) {
				double value = range.SelectionEnd + delta;
				if(value < range.SelectionStart)
					value = range.SelectionStart;
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = range.SelectionStart, SelectionEnd = ToRange(value) }, UpdateEditorSource.TextInput);
			}
		}
		void CenterThumb_DragDelta(object sender, DragDeltaEventArgs e) {
			if (!Editor.BeginInplaceEditing()) return;
			double delta = GetDelta(CenterThumb);
			if (Math.Abs(delta) > 0) 
				SpinSelectionRange(Math.Abs(delta), Math.Sign(delta) == 1);
		}
		protected override double ToRange(double value) {
			double result = base.ToRange(value);
			if (Editor.IsSnapToTickEnabled)
				return Math.Round(result, magicRoundNumber);
			return result;
		}
		double GetSmallStepCount(double delta) {
			return Math.Round(delta * GetRange() / GetSmallStep(), 0);
		}
		double GetSmallStep() {
			return Editor.SmallStep.CompareTo(0) == 0 ? 1d : Editor.SmallStep;
		}
		double RoundNormalValue(double value) {
			if(value < 0)
				return 0;
			else if(value > 1)
				return 1;
			return value;
		}
		double NormalizeByLength(double length, FrameworkElement baseElement) {
			return length / GetLength(baseElement);
		}
		double NormalizeByLength(double length) {
			double range = GetLength(Panel) - GetLength(LeftStepButton) - GetLength(RightStepButton);
			range = range - (Editor.IsRange ? ReservedSpaceCalculator.LeftThumbLength + ReservedSpaceCalculator.RightThumbLength : ReservedSpaceCalculator.ThumbLength);
			return length / range;
		}
		double GetLength(FrameworkElement element) {
			if(element == null)
				return 0;
			return Editor.Orientation == Orientation.Horizontal ? element.ActualWidth : element.ActualHeight;
		}
		int GetOrientationSign() {
			return Editor.Orientation == Orientation.Horizontal ? 1 : -1;
		}
		double GetMousePosition(FrameworkElement element) {
			Point position;
#if !SL
			position = Mouse.GetPosition(element);
#else
			position = mouseHelper.GetMousePosition(element); ;
#endif
			return Editor.Orientation == Orientation.Horizontal ? position.X : GetOrientationSign() * position.Y;
		}
		protected override void ProcessPreviewKeyDownInternal(KeyEventArgs e) {
			base.ProcessPreviewKeyDownInternal(e);
			if(Editor.EditMode == EditMode.InplaceActive && !ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)))
				return;
			if(e.Key == Key.Left || e.Key == Key.Down) {
				Editor.DecrementSmall(Editor.IsRange ? TrackBarIncrementTargetEnum.SelectionRange : TrackBarIncrementTargetEnum.Value);
				e.Handled = true;
			}
			if(e.Key == Key.Right || e.Key == Key.Up) {
				Editor.IncrementSmall(Editor.IsRange ? TrackBarIncrementTargetEnum.SelectionRange : TrackBarIncrementTargetEnum.Value);
				e.Handled = true;
			}
			if(e.Key == Key.PageDown) {
				Editor.DecrementLarge(Editor.IsRange ? TrackBarIncrementTargetEnum.SelectionRange : TrackBarIncrementTargetEnum.Value);
				e.Handled = true;
			}
			if(e.Key == Key.PageUp) {
				Editor.IncrementLarge(Editor.IsRange ? TrackBarIncrementTargetEnum.SelectionRange : TrackBarIncrementTargetEnum.Value);
				e.Handled = true;
			}
		}
		Locker selectionStartLocker = new Locker();
		Locker selectionEndLocker = new Locker();
		public double CoerceSelectionStart(double value) {
			double result = value;
			if(selectionEndLocker.IsLocked && value > Editor.SelectionEnd)
				result = Editor.SelectionEnd;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(PropertyUpdater.SyncValue);
			range.SelectionStart = result;
			CoerceValue(TrackBarEdit.SelectionStartProperty, range);
			return result;
		}
		public virtual void IsSnapToTickEnabledChanged(bool value) {
		}
		public virtual void SelectionStartChanged(double oldValue, double value) {
			if(ShouldLockUpdate)
				return;
			TrackBarEditRange oldRange = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			oldRange.SelectionStart = oldValue;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			range.SelectionStart = value;
			SyncWithValue(TrackBarEdit.SelectionStartProperty, oldRange, range);
			UpdatePositions();
		}
		public virtual double CoerceSelectionEnd(double value) {
			double result = value;
			if(selectionStartLocker.IsLocked && value < Editor.SelectionStart)
				result = Editor.SelectionStart;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(PropertyUpdater.SyncValue);
			range.SelectionEnd = result;
			CoerceValue(TrackBarEdit.SelectionEndProperty, range);
			return result;
		}
		public virtual void SelectionEndChanged(double oldValue, double value) {
			if(ShouldLockUpdate)
				return;
			TrackBarEditRange oldRange = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			oldRange.SelectionEnd = oldValue;
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			range.SelectionEnd = value;
			SyncWithValue(TrackBarEdit.SelectionEndProperty, oldRange, range);
			UpdatePositions();
		}
		public override void UpdatePositions() {
			if(!Editor.IsRange) {
				base.UpdatePositions();
				return;
			}
			double selectionStart = GetNormalValue(Editor.SelectionStart);
			double selectionEnd = GetNormalValue(Editor.SelectionEnd);
			double selectionLength = selectionEnd - selectionStart;
			Info.SelectionLength = selectionLength;
			Info.LeftSidePosition = selectionStart;
			Info.RightSidePosition = 1 - selectionEnd;
			UpdateDisplayText();
		}
		double GetReservedSpace() {
			return ReservedSpaceCalculator.GetReservedSpace();
		}
		public override void OrientationChanged(Orientation orientation) {
			base.OrientationChanged(orientation);
		}
		TrackBarIncrementTargetEnum GetSpinMode(object parameter) {
			return parameter != null ? (TrackBarIncrementTargetEnum)parameter : TrackBarIncrementTargetEnum.Value;
		}
		public override void Minimize(object parameter) {
			Minimize(GetSpinMode(parameter));
		}
		public override void Maximize(object parameter) {
			Maximize(GetSpinMode(parameter));
		}
		public override void IncrementLarge(object parameter) {
			Increment(Editor.LargeStep, GetSpinMode(parameter));
		}
		public override void DecrementLarge(object parameter) {
			Decrement(Editor.LargeStep, GetSpinMode(parameter));
		}
		public override void IncrementSmall(object parameter) {
			Increment(Editor.SmallStep, GetSpinMode(parameter));
		}
		public override void DecrementSmall(object parameter) {
			Decrement(Editor.SmallStep, GetSpinMode(parameter));
		}
		public void Minimize(TrackBarIncrementTargetEnum target) {
			if(!Editor.IsRange) {
				base.Minimize(target);
				return;
			}
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			if(target == TrackBarIncrementTargetEnum.Value)
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = Editor.Minimum, SelectionEnd = Editor.Minimum }, UpdateEditorSource.TextInput);
			if(target == TrackBarIncrementTargetEnum.SelectionStart)
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = Editor.Minimum, SelectionEnd = range.SelectionEnd }, UpdateEditorSource.TextInput);
			if(target == TrackBarIncrementTargetEnum.SelectionEnd)
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = range.SelectionStart, SelectionEnd = range.SelectionStart }, UpdateEditorSource.TextInput);
			if(target == TrackBarIncrementTargetEnum.SelectionRange) {
				double delta = range.SelectionEnd - range.SelectionStart;
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = Editor.Minimum, SelectionEnd = Editor.Minimum + delta }, UpdateEditorSource.TextInput);
			}
		}
		public void Maximize(TrackBarIncrementTargetEnum target) {
			if(!Editor.IsRange) {
				base.Maximize(target);
				return;
			}
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			if(target == TrackBarIncrementTargetEnum.Value)
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = Editor.Maximum, SelectionEnd = Editor.Maximum }, UpdateEditorSource.TextInput);
			if(target == TrackBarIncrementTargetEnum.SelectionStart)
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = Editor.SelectionEnd, SelectionEnd = range.SelectionEnd}, UpdateEditorSource.TextInput);
			if(target == TrackBarIncrementTargetEnum.SelectionEnd)
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = range.SelectionStart, SelectionEnd = Editor.Maximum}, UpdateEditorSource.TextInput);
			if(target == TrackBarIncrementTargetEnum.SelectionRange) {
				double delta = range.SelectionEnd - range.SelectionStart;
				ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = Editor.Maximum - delta, SelectionEnd = Editor.Maximum}, UpdateEditorSource.TextInput);
			}
		}
		public void Increment(double value, TrackBarIncrementTargetEnum target) {
			if (!Editor.IsRange) {
				Increment(value);
				return;
			}
			if (target == TrackBarIncrementTargetEnum.Value) 
				Spin(value, true);
			if(target == TrackBarIncrementTargetEnum.SelectionStart)
				SpinSelectionStart(value, true);
			if(target == TrackBarIncrementTargetEnum.SelectionEnd)
				SpinSelectionEnd(value, true);
			if(target == TrackBarIncrementTargetEnum.SelectionRange)
				SpinSelectionRange(value, true);
		}
		public void Decrement(double value, TrackBarIncrementTargetEnum target) {
			if(!Editor.IsRange) {
				Decrement(value);
				return;
			}
			if(target == TrackBarIncrementTargetEnum.Value)
				Spin(value, false);
			if(target == TrackBarIncrementTargetEnum.SelectionStart)
				SpinSelectionStart(value, false);
			if(target == TrackBarIncrementTargetEnum.SelectionEnd)
				SpinSelectionEnd(value, false);
			if(target == TrackBarIncrementTargetEnum.SelectionRange)
				SpinSelectionRange(value, false);
		}
		void Spin(double value, bool increment) {
			double realValue = (Editor.SelectionEnd + Editor.SelectionStart) / 2 + (increment ? value : -value);
			ValueContainer.SetEditValue(ObjectToTrackBarRangeConverter.TryConvert(ToRange(realValue)), UpdateEditorSource.TextInput);
		}
		void SpinSelectionStart(double value, bool increment) {
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			double realvalue = range.SelectionStart + (increment ? value : -value);
			if (realvalue > range.SelectionEnd)
				realvalue = range.SelectionEnd;
			ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = ToRange(realvalue), SelectionEnd = range.SelectionEnd }, UpdateEditorSource.TextInput);
		}
		void SpinSelectionEnd(double value, bool increment) {
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			double realvalue = range.SelectionEnd + (increment ? value : -value);
			if(realvalue < range.SelectionStart)
				realvalue = range.SelectionStart;
			ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = range.SelectionStart, SelectionEnd = ToRange(realvalue) }, UpdateEditorSource.TextInput);
		}
		void SpinSelectionRange(double value, bool increment) {
			if (!Editor.IsRange) {
				if (increment)
					ValueContainer.SetEditValue(ToRange(ValueContainer.EditValue.TryConvertToDouble() + value), UpdateEditorSource.TextInput);
				else
					ValueContainer.SetEditValue(ToRange(ValueContainer.EditValue.TryConvertToDouble() - value), UpdateEditorSource.TextInput);
				return;
			}
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			double realValue;
			if(increment) 
				realValue = ToRange(range.SelectionEnd + value) - range.SelectionEnd;
			else
				realValue = ToRange(range.SelectionStart - value) - range.SelectionStart;
			ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = range.SelectionStart + realValue, SelectionEnd = range.SelectionEnd + realValue }, UpdateEditorSource.TextInput);
		}
		public override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			if(!AllowKeyHandling)
				return;
			int deltaCount = e.Delta / 120;
			if(Math.Sign(deltaCount) > 0) {
				for(int i = 0; i < Math.Abs(deltaCount); i++)
					IncrementSmall(Editor.IsRange ? TrackBarIncrementTargetEnum.SelectionRange : TrackBarIncrementTargetEnum.Value);
			}
			else {
				for(int i = 0; i < Math.Abs(deltaCount); i++)
					DecrementSmall(Editor.IsRange ? TrackBarIncrementTargetEnum.SelectionRange : TrackBarIncrementTargetEnum.Value);
			}
		}
		public override void MaximumChanged(double value) {
			if(!Editor.IsRange) {
				base.MaximumChanged(value);
				return;
			}
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = ToRange(range.SelectionStart), SelectionEnd = ToRange(range.SelectionEnd) }, UpdateEditorSource.TextInput);
			UpdatePositions();
		}
		public override void MinimumChanged(double value) {
			if(!Editor.IsRange) {
				base.MinimumChanged(value);
				return;
			}
			TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
			ValueContainer.SetEditValue(new TrackBarEditRange() { SelectionStart = ToRange(range.SelectionStart), SelectionEnd = ToRange(range.SelectionEnd) }, UpdateEditorSource.TextInput);
			UpdatePositions();
		}
		protected override void AfterApplyStyleSettings() {
			base.AfterApplyStyleSettings();
			SyncWithValue();
		}
		public override void PreviewMouseUp(MouseButtonEventArgs e) {
			base.PreviewMouseUp(e);
			if (!PropertyProvider.IsMoveToPointEnabled)
				return;
			double value;
			if (LeftSideButton != null && LeftSideButton.IsMouseOver) {
				value = CalcMoveOffset(Panel, e, false);
				SpinSelectionRange(value, false);
				e.Handled = true;
			}
			if (RightSideButton != null && RightSideButton.IsMouseOver) {
				value = CalcMoveOffset(Panel, e, true);
				SpinSelectionRange(value, true);
				e.Handled = true;
			}
		}
		public override void PreviewMouseDown(MouseButtonEventArgs e) {
			if (!PropertyProvider.IsMoveToPointEnabled)
				return;
			if (LeftSideButton != null && LeftSideButton.IsMouseOver)
				e.Handled = true;
			if (RightSideButton != null && RightSideButton.IsMouseOver) {
				e.Handled = true;
			}
			base.PreviewMouseDown(e);
		}
		internal double GetSnapToTickRealValue(double normalizeValue) {
			return GetSnapToTickRealValue(normalizeValue, GetSmallStep());
		}
		double GetSnapToTickRealValue(double normalizeValue, double snapLength) {
			double realValue = GetRealValue(normalizeValue);
			return GetNearestTickValue(realValue, snapLength);
		}
#if !SL
		double GetTickFrequency() {
			return Editor.TickFrequency != 0 ? Editor.TickFrequency : 5;
		}
#endif
		double GetNearestTickValue(double realValue, double snapLength) {
			double lengthToLastTick = Editor.Maximum - realValue;
			if (lengthToLastTick < snapLength) {
				double previousTick = Math.Ceiling(Math.Max(0, GetRange() - snapLength) / snapLength) * snapLength + Editor.Minimum;
				if (Editor.Maximum - realValue < (Editor.Maximum - previousTick) / 2d) {
					return Editor.Maximum;
				}
				else {
					return previousTick;
				}
			}
			double tickCount = Math.Round((realValue - Editor.Minimum) / snapLength);
			double tickValueCandidate = Math.Min(Editor.Maximum, tickCount * snapLength + Editor.Minimum);
			return tickValueCandidate;
		}
		double GetTickBarLength() {
			return GetLength(Panel) + ReservedSpaceCalculator.ReservedSpace;
		}
		double CalcMoveOffset(UIElement element, MouseButtonEventArgs e, bool increment) {
			Point point = e.GetPosition(element);
			double normalizedValue = GetNormalizedValue(point);
			double realValue = Editor.IsSnapToTickEnabled ? GetSnapToTickRealValue(normalizedValue) : GetRealValue(normalizedValue);
			if (Editor.IsRange) {
				TrackBarEditRange range = ObjectToTrackBarRangeConverter.TryConvert(ValueContainer.EditValue);
				return increment ? realValue - range.SelectionEnd : range.SelectionStart - realValue;
			}
			return increment ? realValue - ValueContainer.EditValue.TryConvertToDouble() : ValueContainer.EditValue.TryConvertToDouble() - realValue;
		}
		double GetNormalizedValue(Point point) {
			double position = Editor.Orientation == Orientation.Horizontal ? point.X : point.Y;
			position = position - GetLength(LeftStepButton) - (Editor.IsRange ? ReservedSpaceCalculator.LeftThumbLength : ReservedSpaceCalculator.ThumbLength / 2d);
			double normalizedValue = NormalizeByLength(position);
			return Editor.Orientation == Orientation.Horizontal ? normalizedValue : 1.0 - normalizedValue;
		}
	}
	class ReservedSpaceCalculator : DependencyObject {
		public static readonly DependencyProperty ReservedSpaceProperty;
		public static readonly DependencyProperty ThumbLengthProperty;
		public static readonly DependencyProperty LeftThumbLengthProperty;
		public static readonly DependencyProperty RightThumbLengthProperty;
		static ReservedSpaceCalculator() {
			Type ownerType = typeof(ReservedSpaceCalculator);
			ReservedSpaceProperty = DependencyProperty.Register("ReservedSpace", typeof(double), ownerType, new PropertyMetadata(0d));
			ThumbLengthProperty = DependencyProperty.Register("ThumbLength", typeof(double), ownerType, new PropertyMetadata(0d, PropertyChanged));
			LeftThumbLengthProperty = DependencyProperty.Register("LeftThumbLength", typeof(double), ownerType, new PropertyMetadata(0d, PropertyChanged));
			RightThumbLengthProperty = DependencyProperty.Register("RightThumbLength", typeof(double), ownerType, new PropertyMetadata(0d, PropertyChanged));
		}
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ReservedSpaceCalculator calc = (ReservedSpaceCalculator)d;
			calc.ReservedSpace = calc.ThumbLength + calc.LeftThumbLength + calc.RightThumbLength;
		}
		public double ThumbLength {
			get { return (double)GetValue(ThumbLengthProperty); }
			set { SetValue(ThumbLengthProperty, value); }
		}
		public double ReservedSpace {
			get { return (double)GetValue(ReservedSpaceProperty); }
			set { SetValue(ReservedSpaceProperty, value); }
		}
		public double LeftThumbLength {
			get { return (double)GetValue(LeftThumbLengthProperty); }
			set { SetValue(LeftThumbLengthProperty, value); }
		}
		public double RightThumbLength {
			get { return (double)GetValue(RightThumbLengthProperty); }
			set { SetValue(RightThumbLengthProperty, value); }
		}
		public double GetReservedSpace() {
			return ThumbLength + LeftThumbLength + RightThumbLength;
		}
	}
}
