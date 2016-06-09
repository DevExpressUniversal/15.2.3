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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_HorizontalSmallIncrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_HorizontalLargeIncrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_HorizontalSmallDecrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_HorizontalLargeDecrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_HorizontalThumbIncrease", Type = typeof(AxisScrollBarThumbResizer)),
	TemplatePart(Name = "PART_HorizontalThumbDecrease", Type = typeof(AxisScrollBarThumbResizer)),
	TemplatePart(Name = "PART_HorizontalThumb", Type = typeof(AxisScrollBarThumb)),
	TemplatePart(Name = "PART_HorizontalContainer", Type = typeof(Grid)),
	TemplatePart(Name = "PART_HorizontalRoot", Type = typeof(FrameworkElement)),
	TemplatePart(Name = "PART_VerticalSmallIncrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_VerticalLargeIncrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_VerticalSmallDecrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_VerticalLargeDecrease", Type = typeof(RepeatButton)),
	TemplatePart(Name = "PART_VerticalThumbIncrease", Type = typeof(AxisScrollBarThumbResizer)),
	TemplatePart(Name = "PART_VerticalThumbDecrease", Type = typeof(AxisScrollBarThumbResizer)),
	TemplatePart(Name = "PART_VerticalThumb", Type = typeof(AxisScrollBarThumb)),
	TemplatePart(Name = "PART_VerticalContainer", Type = typeof(Grid)),
	TemplatePart(Name = "PART_VerticalRoot", Type = typeof(FrameworkElement)),
	TemplateVisualState(Name = "Disabled", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Normal", GroupName = "CommonStates"),
	TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates"),
	NonCategorized
	]
	public class AxisScrollBar : Control {
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), typeof(AxisScrollBar), new PropertyMetadata(true, VisiblePropertyChanged));
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation",
			typeof(Orientation), typeof(AxisScrollBar), new PropertyMetadata(Orientation.Vertical, OrientationPropertyChanged));
		public static readonly DependencyProperty MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(double), typeof(AxisScrollBar), new PropertyMetadata(0.0, ValuePropertyChanged));
		public static readonly DependencyProperty MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(double), typeof(AxisScrollBar), new PropertyMetadata(1.0, ValuePropertyChanged));
		public static readonly DependencyProperty CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(AxisScrollBar), new PropertyMetadata(null));
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisScrollBar scrollBar = d as AxisScrollBar;
			if (scrollBar != null) {
				scrollBar.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
				scrollBar.UpdateTrackLayout(true);
			}
		}
		static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisScrollBar scrollBar = d as AxisScrollBar;
			if (scrollBar != null)
				scrollBar.OnOrientationChanged();
		}
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisScrollBar scrollBar = d as AxisScrollBar;
			if (scrollBar != null)
				scrollBar.UpdateTrackLayout(false);
		}
		static double GetControlWidth(Control control) {
			return control == null ? 0 : control.DesiredSize.Width;
		}
		static double GetControlHeight(Control control) {
			return control == null ? 0 : control.DesiredSize.Height;
		}
		static double GetControlMinWidth(Control control) {
			if (control == null)
				return 0;
			Thickness margin = control.Margin;
			return control.MinWidth + margin.Left + margin.Right;
		}
		static double GetControlMinHeight(Control control) {
			if (control == null)
				return 0;
			Thickness margin = control.Margin;
			return control.MinHeight + margin.Top + margin.Bottom;
		}
		AxisScrollBarThumb horizontalThumb;
		AxisScrollBarThumbResizer horizontalThumbIncrease;
		AxisScrollBarThumbResizer horizontalThumbDecrease;
		RepeatButton horizontalSmallIncrease;
		RepeatButton horizontalSmallDecrease;
		RepeatButton horizontalLargeIncrease;
		RepeatButton horizontalLargeDecrease;
		Grid horizontalContainer;
		FrameworkElement horizontalRoot;
		AxisScrollBarThumb verticalThumb;
		AxisScrollBarThumbResizer verticalThumbIncrease;
		AxisScrollBarThumbResizer verticalThumbDecrease;
		RepeatButton verticalSmallIncrease;
		RepeatButton verticalSmallDecrease;
		RepeatButton verticalLargeIncrease;
		RepeatButton verticalLargeDecrease;
		Grid verticalContainer;
		FrameworkElement verticalRoot;
		double dragMinValue;
		double dragMaxValue;
		bool isMouseOver;
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public double MinValue {
			get { return (double)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		public double MaxValue {
			get { return (double)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		double LargeChange { get { return Math.Abs(MaxValue - MinValue); } }
		double SmallChange {
			get {
				double actualSize = Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight;
				double largeChange = LargeChange;
				return largeChange + actualSize == largeChange ? 0 : largeChange / actualSize;
			}
		}
		internal double TrackLength {
			get {
				return Orientation == Orientation.Horizontal ?
					(ActualWidth - GetControlWidth(horizontalSmallDecrease) - GetControlWidth(horizontalSmallIncrease)) :
					(ActualHeight - GetControlHeight(verticalSmallDecrease) - GetControlHeight(verticalSmallIncrease));
			}
		}
		internal double ThumbMinSize {
			get {
				return Orientation == Orientation.Horizontal ?
					Math.Max(GetControlMinWidth(horizontalThumb), GetControlMinWidth(horizontalThumbDecrease) + GetControlMinWidth(horizontalThumbIncrease)) :
					Math.Max(GetControlMinHeight(verticalThumb), GetControlMinHeight(verticalThumbDecrease) + GetControlMinHeight(verticalThumbIncrease));
			}
		}
		public AxisScrollBar() {
			DefaultStyleKey = typeof(AxisScrollBar);
			SizeChanged += (d, e) => UpdateTrackLayout(true);
		}
		void UpdateTrackLayout(bool correctRanges) {
			if (Visibility == Visibility.Visible) {
				Grid container;
				AxisScrollBarThumb thumb;
				AxisScrollBarThumbResizer decreaseResizer;
				AxisScrollBarThumbResizer increaseResizer;
				RepeatButton decreaseButton;
				DependencyProperty sizeProperty;
				if (Orientation == Orientation.Horizontal) {
					container = horizontalContainer;
					thumb = horizontalThumb;
					decreaseResizer = horizontalThumbDecrease;
					increaseResizer = horizontalThumbIncrease;
					decreaseButton = horizontalLargeDecrease;
					sizeProperty = Control.WidthProperty;
				}
				else {
					container = verticalContainer;
					thumb = verticalThumb;
					decreaseResizer = verticalThumbDecrease;
					increaseResizer = verticalThumbIncrease;
					decreaseButton = verticalLargeDecrease;
					sizeProperty = Control.HeightProperty;
				}
				double trackLength = TrackLength;
				if (trackLength < 0) {
					if (container != null)
						container.Visibility = Visibility.Collapsed;
					return;
				}
				if (container != null)
					container.Visibility = Visibility.Visible;
				double thumbMinSize = ThumbMinSize;
				if (trackLength < thumbMinSize || trackLength < 1) {
					if (thumb != null)
						thumb.Visibility = Visibility.Collapsed;
					if (decreaseResizer != null)
						decreaseResizer.Visibility = Visibility.Collapsed;
					if (increaseResizer != null)
						increaseResizer.Visibility = Visibility.Collapsed;
				}
				else {
					if (thumb != null) {
						double thumbSize = trackLength * (MaxValue - MinValue);
						if (thumbSize < thumbMinSize) {
							if (correctRanges) {
								double halfCorrectDiapason = thumbMinSize / trackLength / 2.0;
								double center = (MinValue + MaxValue) / 2.0;
								double correctedMin = center - halfCorrectDiapason;
								double correctedMax = center + halfCorrectDiapason;
								if (correctedMin < 0) {
									correctedMax -= correctedMin;
									correctedMin = 0;
								}
								else if (correctedMax > 1) {
									correctedMin -= correctedMax - 1;
									correctedMax = 1;
								}
								ExecuteCommand(NavigationType.SetRange, new MinMaxValues(correctedMin, correctedMax));
								return;
							}
							thumbSize = thumbMinSize;
						}
						trackLength -= thumbSize;
						thumb.SetValue(sizeProperty, thumbSize);
						thumb.Visibility = Visibility.Visible;
					}
					if (decreaseResizer != null)
						decreaseResizer.Visibility = Visibility.Visible;
					if (increaseResizer != null)
						increaseResizer.Visibility = Visibility.Visible;
					if (decreaseButton != null) {
						double minVaue = MinValue;
						double remainSize = minVaue + 1 - MaxValue;
						decreaseButton.SetValue(sizeProperty, Math.Max(0, minVaue + remainSize == minVaue ? trackLength / 2.0 : trackLength * minVaue / remainSize));
					}
				}
			}
		}
		void ExecuteCommand(NavigationType navigationType, MinMaxValues values) {
			ICommand command = Command;
			if (command != null) {
				object commandParameter = new AxisRangePositions(navigationType, values.Min, values.Max);
				if (command.CanExecute(commandParameter))
					command.Execute(commandParameter);
			}
		}
		void ExecuteCommand(NavigationType navigationType, double shift) {
			double newMin = MinValue + shift;
			double newMax = MaxValue + shift;
			double minLimit = 0;
			double maxLimit = 1.0;
			double delta = newMax - newMin;
			if (newMin < minLimit) {
				newMin = minLimit;
				newMax = newMin + delta;
			}
			else if (newMax > maxLimit) {
				newMax = maxLimit;
				newMin = newMax - delta;
			}
			if (newMin != MinValue || newMax != MaxValue || navigationType == NavigationType.ZoomThumbRelease)
				ExecuteCommand(navigationType, new MinMaxValues(newMin, newMax));
		}
		void LargeDecrement() {
			double trackLength = TrackLength;
			if (trackLength > ThumbMinSize && trackLength > 1)
				ExecuteCommand(NavigationType.LargeDecrement, -LargeChange);
		}
		void LargeIncrement() {
			double trackLength = TrackLength;
			if (trackLength > ThumbMinSize && trackLength > 1)
				ExecuteCommand(NavigationType.LargeIncrement, LargeChange);
		}
		void SmallDecrement() {
			ExecuteCommand(NavigationType.SmallDecrement, -SmallChange);
		}
		void SmallIncrement() {
			ExecuteCommand(NavigationType.SmallIncrement, SmallChange);
		}
		void OnThumbDragStarted(object sender, DragStartedEventArgs e) {
			StartThumbDrag();
		}
		void OnThumbDragCompleted(object sender, DragCompletedEventArgs e) {
			ExecuteCommand(NavigationType.ZoomThumbRelease, 0);
		}
		void OnThumbDragDelta(object sender, DragDeltaEventArgs e) {
			dragMinValue += CalculateDragDelta(e);
			ExecuteCommand(NavigationType.ThumbPosition, dragMinValue - MinValue);
		}
		void OnThumbDecreaseDragDelta(object sender, DragDeltaEventArgs e) {
			ThumbDecreaseDragDelta(e.HorizontalChange, e.VerticalChange);
		}
		void OnThumbIncreaseDragDelta(object sender, DragDeltaEventArgs e) {
			ThumbIncreaseDragDelta(e.HorizontalChange, e.VerticalChange);
		}
		void ThumbDecreaseDragDelta(double horizontalChange, double verticalChange) {
			dragMinValue += CalculateDragDelta(horizontalChange, verticalChange);
			double newMinValue = Math.Min(MaxValue - ThumbMinSize / TrackLength, Math.Max(0, dragMinValue));
			if (newMinValue != MinValue)
				ExecuteCommand(NavigationType.SetRange, new MinMaxValues(newMinValue, MaxValue));
		}
		void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateVisualState(false);
		}
		void ThumbIncreaseDragDelta(double horizontalChange, double verticalChange) {
			dragMaxValue += CalculateDragDelta(horizontalChange, verticalChange);
			double newMaxValue = Math.Max(MinValue + ThumbMinSize / TrackLength, Math.Min(1.0, dragMaxValue));
			if (newMaxValue != MaxValue)
				ExecuteCommand(NavigationType.SetRange, new MinMaxValues(MinValue, newMaxValue));
		}
		void UpdateVisualState(bool useTransitions) {
			if ((horizontalThumb != null && !horizontalThumb.IsDragging) || (verticalThumb != null && !verticalThumb.IsDragging))
				if (!IsEnabled)
					VisualStateManager.GoToState(this, "Disabled", useTransitions);
				else if (isMouseOver)
					VisualStateManager.GoToState(this, "MouseOver", useTransitions);
				else
					VisualStateManager.GoToState(this, "Normal", useTransitions);
		}
		void UpdateVisualState() {
			UpdateVisualState(true);
		}
		void OnOrientationChanged() {
			if (Orientation == Orientation.Horizontal) {
				if (horizontalRoot != null)
					horizontalRoot.Visibility = Visibility.Visible;
				if (verticalRoot != null)
					verticalRoot.Visibility = Visibility.Collapsed;
			}
			else {
				if (horizontalRoot != null)
					horizontalRoot.Visibility = Visibility.Collapsed;
				if (verticalRoot != null)
					verticalRoot.Visibility = Visibility.Visible;
			}
			UpdateTrackLayout(true);
		}
		double CalculateDragDelta(DragDeltaEventArgs e) {
			return CalculateDragDelta(e.HorizontalChange, e.VerticalChange);
		}
		double CalculateDragDelta(double horizontalChange, double verticalChange) {
			double trackLength = TrackLength;
			if (trackLength < 1)
				return 0;
			double delta = (Orientation == Orientation.Horizontal ? horizontalChange : -verticalChange) / trackLength;
			return delta;
		}
		internal void StartThumbDrag() {
			dragMinValue = MinValue;
			dragMaxValue = MaxValue;
		}
		internal void ThumbDragDelta(double horizontalChange, double verticalChange) {
			dragMinValue += CalculateDragDelta(horizontalChange, verticalChange);
			ExecuteCommand(NavigationType.ThumbPosition, dragMinValue - MinValue);
		}
		internal void DragThumbResizer(AxisScrollBarThumbResizer thumbResizer, double horizontalChange, double verticalChange) {
			if (thumbResizer == horizontalThumbIncrease || thumbResizer == verticalThumbIncrease)
				ThumbIncreaseDragDelta(horizontalChange, verticalChange);
			if (thumbResizer == horizontalThumbDecrease || thumbResizer == verticalThumbDecrease)
				ThumbDecreaseDragDelta(horizontalChange, verticalChange);
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			UpdateVisualState();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			isMouseOver = true;
			UpdateVisualState();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			isMouseOver = false;
			UpdateVisualState();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			horizontalContainer = GetTemplateChild("PART_HorizontalContainer") as Grid;
			horizontalRoot = GetTemplateChild("PART_HorizontalRoot") as FrameworkElement;
			verticalContainer = GetTemplateChild("PART_VerticalContainer") as Grid;
			verticalRoot = GetTemplateChild("PART_VerticalRoot") as FrameworkElement;
			horizontalThumb = GetTemplateChild("PART_HorizontalThumb") as AxisScrollBarThumb;
			if (horizontalThumb != null) {
				horizontalThumb.DragStarted += new DragStartedEventHandler(OnThumbDragStarted);
				horizontalThumb.DragDelta += new DragDeltaEventHandler(OnThumbDragDelta);
			}
			horizontalThumbIncrease = GetTemplateChild("PART_HorizontalThumbIncrease") as AxisScrollBarThumbResizer;
			if (horizontalThumbIncrease != null) {
				horizontalThumbIncrease.DragStarted += new DragStartedEventHandler(OnThumbDragStarted);
				horizontalThumbIncrease.DragDelta += new DragDeltaEventHandler(OnThumbIncreaseDragDelta);
				horizontalThumbIncrease.DragCompleted += new DragCompletedEventHandler(OnThumbDragCompleted);
			}
			horizontalThumbDecrease = GetTemplateChild("PART_HorizontalThumbDecrease") as AxisScrollBarThumbResizer;
			if (horizontalThumbDecrease != null) {
				horizontalThumbDecrease.DragStarted += new DragStartedEventHandler(OnThumbDragStarted);
				horizontalThumbDecrease.DragDelta += new DragDeltaEventHandler(OnThumbDecreaseDragDelta);
				horizontalThumbDecrease.DragCompleted += new DragCompletedEventHandler(OnThumbDragCompleted);
			}
			verticalThumb = GetTemplateChild("PART_VerticalThumb") as AxisScrollBarThumb;
			if (verticalThumb != null) {
				verticalThumb.DragStarted += new DragStartedEventHandler(OnThumbDragStarted);
				verticalThumb.DragDelta += new DragDeltaEventHandler(OnThumbDragDelta);
			}
			verticalThumbIncrease = GetTemplateChild("PART_VerticalThumbIncrease") as AxisScrollBarThumbResizer;
			if (verticalThumbIncrease != null) {
				verticalThumbIncrease.DragStarted += new DragStartedEventHandler(OnThumbDragStarted);
				verticalThumbIncrease.DragDelta += new DragDeltaEventHandler(OnThumbIncreaseDragDelta);
				verticalThumbIncrease.DragCompleted += new DragCompletedEventHandler(OnThumbDragCompleted);
			}
			verticalThumbDecrease = GetTemplateChild("PART_VerticalThumbDecrease") as AxisScrollBarThumbResizer;
			if (verticalThumbDecrease != null) {
				verticalThumbDecrease.DragStarted += new DragStartedEventHandler(OnThumbDragStarted);
				verticalThumbDecrease.DragDelta += new DragDeltaEventHandler(OnThumbDecreaseDragDelta);
				verticalThumbDecrease.DragCompleted += new DragCompletedEventHandler(OnThumbDragCompleted);
			}
			horizontalLargeIncrease = GetTemplateChild("PART_HorizontalLargeIncrease") as RepeatButton;
			if (horizontalLargeIncrease != null)
				horizontalLargeIncrease.Click += (d, e) => LargeIncrement();
			horizontalLargeDecrease = GetTemplateChild("PART_HorizontalLargeDecrease") as RepeatButton;
			if (horizontalLargeDecrease != null)
				horizontalLargeDecrease.Click += (d, e) => LargeDecrement();
			horizontalSmallIncrease = GetTemplateChild("PART_HorizontalSmallIncrease") as RepeatButton;
			if (horizontalSmallIncrease != null)
				horizontalSmallIncrease.Click += (d, e) => SmallIncrement();
			horizontalSmallDecrease = GetTemplateChild("PART_HorizontalSmallDecrease") as RepeatButton;
			if (horizontalSmallDecrease != null)
				horizontalSmallDecrease.Click += (d, e) => SmallDecrement();
			verticalLargeIncrease = GetTemplateChild("PART_VerticalLargeIncrease") as RepeatButton;
			if (verticalLargeIncrease != null)
				verticalLargeIncrease.Click += (d, e) => LargeIncrement();
			verticalLargeDecrease = GetTemplateChild("PART_VerticalLargeDecrease") as RepeatButton;
			if (verticalLargeDecrease != null)
				verticalLargeDecrease.Click += (d, e) => LargeDecrement();
			verticalSmallIncrease = GetTemplateChild("PART_VerticalSmallIncrease") as RepeatButton;
			if (verticalSmallIncrease != null)
				verticalSmallIncrease.Click += (d, e) => SmallIncrement();
			verticalSmallDecrease = GetTemplateChild("PART_VerticalSmallDecrease") as RepeatButton;
			if (verticalSmallDecrease != null)
				verticalSmallDecrease.Click += (d, e) => SmallDecrement();
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			OnOrientationChanged();
			UpdateVisualState(false);
		}
	}
}
