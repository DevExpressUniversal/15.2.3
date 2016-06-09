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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Xpf.Core;
#if WPF
using DevExpress.Xpf.Scheduler;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Media.Imaging;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Media;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
#if SL
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using DependencyProperty = System.Windows.DependencyProperty;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using System.IO;
using System.Reflection;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public enum IntervalDisplayType { Start, End };
	#region AppointmentClockControlBase
	public abstract class AppointmentClockControlBase : Control {
		#region Properties
		#region ViewInfo
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = CreateViewInfoProperty();
		static DependencyProperty CreateViewInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentClockControlBase, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoPropertyChanged(e.OldValue, e.NewValue));
		}
		#endregion
		#region TimeText
		public string TimeText {
			get { return (string)GetValue(TimeTextProperty); }
			set { SetValue(TimeTextProperty, value); }
		}
		public static readonly DependencyProperty TimeTextProperty = CreateTimeTextProperty();
		static DependencyProperty CreateTimeTextProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentClockControlBase, string>("TimeText", String.Empty);
		}
		#endregion
		#region TimeTextVisibility
		public Visibility TimeTextVisibility {
			get { return (Visibility)GetValue(TimeTextVisibilityProperty); }
			set { SetValue(TimeTextVisibilityProperty, value); }
		}
		public static readonly DependencyProperty TimeTextVisibilityProperty = CreateTimeTextVisibilityProperty();
		static DependencyProperty CreateTimeTextVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentClockControlBase, Visibility>("TimeTextVisibility", Visibility.Visible);
		}
		#endregion
		#endregion
		protected virtual void OnViewInfoPropertyChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if (oldValue != null)
				oldValue.PropertiesChanged -= new EventHandler(ViewInfoPropertiesChanged);
			if (newValue != null)
				newValue.PropertiesChanged += new EventHandler(ViewInfoPropertiesChanged);
			UpdateProperties();
		}
		void ViewInfoPropertiesChanged(object sender, EventArgs e) {
			UpdateProperties();
		}
		protected virtual bool UpdateProperties() {
			if (ViewInfo == null) {
				TimeText = String.Empty;
				TimeTextVisibility = Visibility.Collapsed;
				return false;
			}
			return true;
		}
		protected Visibility CalculateVisibility(bool visible) {
			return visible ? Visibility.Visible : Visibility.Collapsed;
		}
	}
	#endregion
	#region HorizontalAppointmentClockControl
	public class HorizontalAppointmentClockControl : AppointmentClockControlBase {
		static HorizontalAppointmentClockControl() {
		}
		public HorizontalAppointmentClockControl() {
			DefaultStyleKey = typeof(AppointmentClockControlBase);
		}
		public static readonly DependencyProperty IntervalDisplayTypeProperty = CreateIntervalDisplayTypeProperty();
		static DependencyProperty CreateIntervalDisplayTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalAppointmentClockControl, IntervalDisplayType>("IntervalDisplayType", IntervalDisplayType.Start, (d, e) => { d.UpdateProperties(); });
		}
		public IntervalDisplayType IntervalDisplayType {
			get { return (IntervalDisplayType)GetValue(IntervalDisplayTypeProperty); }
			set { SetValue(IntervalDisplayTypeProperty, value); }
		}
		protected override bool UpdateProperties() {
			if (base.UpdateProperties() == false)
				return false;
			if (IntervalDisplayType == IntervalDisplayType.Start) {
				TimeText = ViewInfo.StartTimeText;
				TimeTextVisibility = CalculateVisibility(ViewInfo.Options.ShowStartTime);
			}
			else {
				TimeText = ViewInfo.EndTimeText;
				TimeTextVisibility = CalculateVisibility(ViewInfo.Options.ShowEndTime);
			}
			return true;
		}
	}
	#endregion
	#region HorizontalAppointmentClockControlBase
	public abstract class HorizontalAppointmentClockControlBase : AppointmentClockControlBase {
		static HorizontalAppointmentClockControlBase() {
		}
		protected HorizontalAppointmentClockControlBase() {
			DefaultStyleKey = typeof(HorizontalAppointmentClockControlBase);
		}
		public RotateTransform MinuteHandTransform { get; private set; }
		public abstract DateTime Time { get; }
		#region AngleMinutes
		public double AngleMinutes {
			get { return (double)GetValue(AngleMinutesProperty); }
			set { SetValue(AngleMinutesProperty, value); }
		}
		public static readonly DependencyProperty AngleMinutesProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalAppointmentClockControlBase, double>("AngleMinutes", 0);
		#endregion
		#region AngleHours
		public double AngleHours {
			get { return (double)GetValue(AngleHoursProperty); }
			set { SetValue(AngleHoursProperty, value); }
		}
		public static readonly DependencyProperty AngleHoursProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalAppointmentClockControlBase, double>("AngleHours", 0);
		#endregion
		#region IsDayTime
		private static readonly DependencyPropertyKey IsDayTimePropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<HorizontalAppointmentClockControlBase, bool>("IsDayTime", false);
		public static readonly DependencyProperty IsDayTimeProperty = IsDayTimePropertyKey.DependencyProperty;
		public bool IsDayTime {
			get { return (bool)GetValue(IsDayTimeProperty); }
			protected set { this.SetValue(IsDayTimePropertyKey, value); }
		}
		#endregion
		#region IsNightTime
		private static readonly DependencyPropertyKey IsNightTimePropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<HorizontalAppointmentClockControlBase, bool>("IsNightTime", false);
		public static readonly DependencyProperty IsNightTimeProperty = IsNightTimePropertyKey.DependencyProperty;
		public bool IsNightTime {
			get { return (bool)GetValue(IsNightTimeProperty); }
			protected set { this.SetValue(IsNightTimePropertyKey, value); }
		}
		#endregion
		protected override bool UpdateProperties() {
			if (base.UpdateProperties() == false)
				return false;
			TimeText = GetTimeText();
			Visibility = CalculateClockVisibility();
			CalculateClockHandsPosition(Time);
			ChangeVisualState();
			return true;
		}
		protected virtual void ChangeVisualState() {
			IsDayTime = false;
			IsNightTime = false;
			if (ViewInfo.ShowTimeAsClock) {
				if (Time.TimeOfDay < TimeSpan.FromHours(12))
					IsDayTime = true;
				else
					IsNightTime = true;
			}
		}
		protected abstract string GetTimeText();
		protected abstract Visibility CalculateClockVisibility();
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (ViewInfo == null)
				return;
			CalculateClockHandsPosition(Time);
			ChangeVisualState();
		}
		void CalculateClockHandsPosition(DateTime time) {
			AngleHours = GetAngleByPositionInDegrees(GetHourHandPosition(time.Hour, time.Minute));
			AngleMinutes = GetAngleByPositionInDegrees(time.Minute);
		}
		int GetHourHandPosition(int hour, int minute) {
			return (hour % 12) * 5 + minute / 12;
		}
		void CalculateClockHand(RotateTransform transform, int minuteHandPosition) {
			transform.Angle = GetAngleByPositionInDegrees(minuteHandPosition);
		}
		double GetAngleByPositionInDegrees(int minuteHandPosition) {
			return (double)(6 * minuteHandPosition);
		}
	}
	#endregion
	#region HorizontalAppointmentStartClockControl
	public class HorizontalAppointmentStartClockControl : HorizontalAppointmentClockControlBase {
		public override DateTime Time { get { return ViewInfo.AppointmentStart; } }
		protected override string GetTimeText() {
			return ViewInfo.StartTimeText;
		}
		protected override Visibility CalculateClockVisibility() {
			return CalculateVisibility((ViewInfo.HasLeftBorder || ViewInfo.SameDay) && ViewInfo.Options.ShowStartTime);
		}
	}
	#endregion
	#region HorizontalAppointmentEndClockControl
	public class HorizontalAppointmentEndClockControl : HorizontalAppointmentClockControlBase {
		public override DateTime Time { get { return ViewInfo.AppointmentEnd; } }
		protected override string GetTimeText() {
			return ViewInfo.EndTimeText;
		}
		protected override Visibility CalculateClockVisibility() {
			return CalculateVisibility((ViewInfo.HasRightBorder || ViewInfo.SameDay) && ViewInfo.Options.ShowEndTime);
		}
	}
	#endregion
	#region AppointmentContinueDateControl
	public abstract class AppointmentContinueDateControl : Control {
		protected AppointmentContinueDateControl() {
			ArrowImage = CreateArrowImage();
		}
		#region ViewInfo
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentContinueDateControl, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		protected virtual void OnViewInfoChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if (oldValue != null || newValue == null)
				ClearValue(VisibilityProperty);
			if (newValue != null) {
				Binding bd = new Binding(GetShouldShowContinueItemPropertyName());
				bd.Mode = BindingMode.OneWay;
				bd.Source = newValue;
				bd.Converter = ConverterHolder.BooleanToVisibilityConverter;
				SetBinding(VisibilityProperty, bd);
			}
		}
		#endregion
		#region ArrowImage
		public static readonly DependencyProperty ArrowImageProperty = CreateArrowImageProperty();
		private static DependencyProperty CreateArrowImageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentContinueDateControl, BitmapImage>("ArrowImage", null);
		}
		public BitmapImage ArrowImage {
			get { return (BitmapImage)GetValue(ArrowImageProperty); }
			set { SetValue(ArrowImageProperty, value); }
		}
		#endregion
		#region Text
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty = CreateTextProperty();
		static DependencyProperty CreateTextProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentContinueDateControl, string>("Text", String.Empty);
		}
		#endregion
		protected abstract string GetShouldShowContinueItemPropertyName();
		protected abstract BitmapImage CreateArrowImage();
	}
	#endregion
	public class AppointmentContinueItemPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			double totalWidth = 0;
			double maxHeight = 0;
			int count = Children.Count;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				child.Measure(availableSize);
				Size desiredSize = child.DesiredSize;
				totalWidth += desiredSize.Width;
				maxHeight = Math.Max(maxHeight, desiredSize.Height);
			}
			return new Size(double.IsInfinity(availableSize.Width) ? totalWidth : availableSize.Width, maxHeight);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			double availableWidth = arrangeBounds.Width;
			double totalWidth = 0;
			int count = Children.Count;
			for (int i = 0; i < count; i++) {
				Size childSize = Children[i].DesiredSize;
				double childWidth = childSize.Width;
				if (availableWidth >= childWidth || totalWidth == 0) {
					Children[i].Arrange(new Rect(new Point(totalWidth, 0), childSize));
					totalWidth += childWidth;
				}
				else
					break;
				availableWidth -= childWidth;
			}
			return new Size(totalWidth, arrangeBounds.Height);
		}
	}
	#region AppointmentContinueStartDateControl
	public class AppointmentContinueStartDateControl : AppointmentContinueDateControl {
		public AppointmentContinueStartDateControl() {
			DefaultStyleKey = typeof(AppointmentContinueStartDateControl);
		}
		protected override BitmapImage CreateArrowImage() {
			return DefaultAppointmentImages.AppointmentStartContinueArrow;
		}
		protected override string GetShouldShowContinueItemPropertyName() {
#if SL
			return VisualAppointmentViewInfo.ShowStartContinueItemProperty.GetName();
#else
			return VisualAppointmentViewInfo.ShowStartContinueItemProperty.Name;
#endif
		}
	}
	#endregion
	#region AppointmentContinueEndDateControl
	public class AppointmentContinueEndDateControl : AppointmentContinueDateControl {
		public AppointmentContinueEndDateControl() {
			DefaultStyleKey = typeof(AppointmentContinueEndDateControl);
		}
		protected override BitmapImage CreateArrowImage() {
			return DefaultAppointmentImages.AppointmentEndContinueArrow;
		}
		protected override string GetShouldShowContinueItemPropertyName() {
#if SL
			return VisualAppointmentViewInfo.ShowEndContinueItemProperty.GetName();
#else
			return VisualAppointmentViewInfo.ShowEndContinueItemProperty.Name;
#endif
		}
	}
	#endregion
	#region HorizontalAppointmentContentPanel
	public class HorizontalAppointmentContentPanel : Panel {
		const double MinTextBlockWidth = 20;
		const double MinContentHeight = 16;
		protected override Size MeasureOverride(Size constraint) {
			Size actualSize = new Size();
			double availableWidth = constraint.Width;
			double maxHeight = 0;
			for (int i = 0; i < Children.Count; i++) {
				UIElement child = Children[i];
				if (availableWidth > 0.0) {
					child.Measure(new Size(constraint.Width, constraint.Height));
					double childWidth = child.DesiredSize.Width;
					double childHeight = child.DesiredSize.Height;
					double actualWidth = Math.Min(availableWidth, childWidth);
					if (child is TextBlock && actualWidth < MinTextBlockWidth) {
						childWidth = actualWidth;
						childHeight = Math.Max(maxHeight, MinContentHeight);
						child.Measure(new Size(availableWidth, maxHeight));
					}
					maxHeight = Math.Max(maxHeight, childHeight);
					if (childWidth > availableWidth) {
						child.Measure(new Size(availableWidth, constraint.Height));
						availableWidth = 0.0;
					}
					else
						availableWidth -= childWidth;
					actualSize.Width += child.DesiredSize.Width;
					actualSize.Height = Math.Max(actualSize.Height, childHeight);
				}
				else
					child.Measure(Size.Empty);
			}
			return actualSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Point location = new Point();
			for (int i = 0; i < Children.Count; i++) {
				UIElement child = Children[i];
				double childWidth = child.DesiredSize.Width;
				double childHeight = Math.Max(finalSize.Height, child.DesiredSize.Height);
				Rect finalRect = new Rect(location, new Size(childWidth, childHeight));
				child.Arrange(finalRect);
				location.X += childWidth;
			}
			return finalSize;
		}
	}
	#endregion
	#region VerticalAppointmentContentPanel
	public class VerticalAppointmentContentPanel : Panel {
		#region AlwaysOnNewRow
		public static readonly DependencyProperty AlwaysOnNewRowProperty = CreateAlwaysOnNewRowProperty();
		static DependencyProperty CreateAlwaysOnNewRowProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<VerticalAppointmentContentPanel, bool>("AlwaysOnNewRow", false, FrameworkPropertyMetadataOptions.None, null);
		}
		public static bool GetAlwaysOnNewRow(UIElement element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(AlwaysOnNewRowProperty);
		}
		public static void SetAlwaysOnNewRow(UIElement element, bool value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(AlwaysOnNewRowProperty, value);
		}
		#endregion
		protected override Size MeasureOverride(Size constraint) {
			Size currentLineSize = new Size();
			Size actualSize = new Size();
			double availableHeight = constraint.Height;
			for (int i = 0; i < Children.Count; i++) {
				UIElement element = Children[i];
				element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Size childSize = element.DesiredSize;
				double availableWidth = constraint.Width - currentLineSize.Width;
				if (!Contains(childSize, availableWidth, availableHeight) || GetAlwaysOnNewRow(element)) {
					actualSize.Width = Math.Max(currentLineSize.Width, actualSize.Width);
					actualSize.Height += currentLineSize.Height;
					availableHeight -= currentLineSize.Height;
					if (DoubleUtil.LessThanOrClose(childSize.Height, availableHeight)) {
						if (DoubleUtil.GreaterThan(childSize.Width, constraint.Width)) {
							element.Measure(new Size(constraint.Width, availableHeight));
							childSize = element.DesiredSize;
							actualSize.Width = Math.Max(childSize.Width, actualSize.Width);
							actualSize.Height += childSize.Height;
							availableHeight -= childSize.Height;
							currentLineSize = new Size();
						}
						else
							currentLineSize = childSize;
					}
					else
						element.Measure(Size.Empty);
				}
				else {
					currentLineSize.Width += childSize.Width;
					currentLineSize.Height = Math.Max(childSize.Height, currentLineSize.Height);
				}
			}
			actualSize.Width = Math.Max(currentLineSize.Width, actualSize.Width);
			actualSize.Height += currentLineSize.Height;
			return actualSize;
		}
		bool Contains(Size size, double width, double heigth) {
			return DoubleUtil.LessThanOrClose(size.Width, width) && DoubleUtil.LessThanOrClose(size.Height, heigth);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double totalHeight = 0.0;
			Size currentLineSize = new Size();
			int start = 0;
			int end = 0;
			int childrenCount = Children.Count;
			while (end < childrenCount) {
				UIElement element = Children[end];
				Size childSize = element.DesiredSize;
				double availableWidth = finalSize.Width - currentLineSize.Width;
				bool needToPlaceOnNewLine = DoubleUtil.GreaterThan(childSize.Width, availableWidth) || GetAlwaysOnNewRow(element);
				if (needToPlaceOnNewLine) {
					ArrangeLine(totalHeight, currentLineSize.Height, start, end);
					totalHeight += currentLineSize.Height;
					currentLineSize = childSize;
					if (DoubleUtil.GreaterThan(childSize.Width, finalSize.Width)) {
						ArrangeLine(totalHeight, childSize.Height, end, ++end);
						totalHeight += childSize.Height;
						currentLineSize = new Size();
					}
					start = end;
				}
				else {
					currentLineSize.Width += childSize.Width;
					currentLineSize.Height = Math.Max(childSize.Height, currentLineSize.Height);
				}
				end++;
			}
			if (start < Children.Count)
				ArrangeLine(totalHeight, currentLineSize.Height, start, Children.Count);
			return finalSize;
		}
		void ArrangeLine(double height, double lineHeight, int start, int end) {
			double x = 0.0;
			for (int i = start; i < end; i++) {
				UIElement element = Children[i];
				if (element != null) {
					Size size = element.DesiredSize;
					element.Arrange(new Rect(x, height, size.Width, lineHeight));
					x += size.Width;
				}
			}
		}
	}
	#endregion
	#region VerticalAppointmentClockControl
	public class VerticalAppointmentClockControl : AppointmentClockControlBase {
		public VerticalAppointmentClockControl() {
			DefaultStyleKey = typeof(VerticalAppointmentClockControl);
		}
		bool ShowStartTime { get { return ViewInfo.Options.ShowStartTime; } }
		bool ShowEndTime { get { return ViewInfo.Options.ShowEndTime; } }
		protected override bool UpdateProperties() {
			if (base.UpdateProperties() == false)
				return false;
			string startTime = ShowStartTime ? ViewInfo.StartTimeText : String.Empty;
			string endTime = ShowEndTime ? ViewInfo.EndTimeText : String.Empty;
			TimeText = String.Format("{0}{1}", startTime, endTime);
			TimeTextVisibility = CalculateVisibility(ShowStartTime | ShowEndTime);
			return true;
		}
	}
	#endregion
	#region AppointmentImagesControl
	public class AppointmentImagesControl : Control {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ReminderImageProperty;
		public static readonly DependencyProperty ShouldShowReminderImageProperty;
		public static readonly DependencyProperty RecurrenceImageProperty;
		public static readonly DependencyProperty ShouldShowRecurrenceImageProperty;
		public static readonly DependencyProperty ChangedRecurrenceImageProperty;
		public static readonly DependencyProperty ShouldShowChangedRecurrenceImageProperty;
		static AppointmentImagesControl() {
			OrientationProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, Orientation>("Orientation", Orientation.Horizontal);
			ReminderImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, BitmapImage>("ReminderImage", null);
			ShouldShowReminderImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, bool>("ShouldShowReminderImage", false);
			RecurrenceImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, BitmapImage>("RecurrenceImage", null);
			ShouldShowRecurrenceImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, bool>("ShouldShowRecurrenceImage", false);
			ChangedRecurrenceImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, BitmapImage>("ChangedRecurrenceImage", null);
			ShouldShowChangedRecurrenceImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, bool>("ShouldShowChangedRecurrenceImage", false);
		}
		public AppointmentImagesControl() {
			DefaultStyleKey = typeof(AppointmentImagesControl);
			ReminderImage = DefaultAppointmentImages.Reminder;
			RecurrenceImage = DefaultAppointmentImages.Recurrence;
			ChangedRecurrenceImage = DefaultAppointmentImages.ChangedRecurrence;
		}
		#region ViewInfo
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = CreateViewInfoProperty();
		static DependencyProperty CreateViewInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentImagesControl, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		}
		#endregion
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		#endregion
		#region ReminderImage
		public BitmapImage ReminderImage {
			get { return (BitmapImage)GetValue(ReminderImageProperty); }
			set { SetValue(ReminderImageProperty, value); }
		}
		#endregion
		#region ShouldShowReminderImage
		bool lastShouldShowReminderImage = false;
		public bool ShouldShowReminderImage {
			get { return (bool)GetValue(ShouldShowReminderImageProperty); }
			set {
				if (lastShouldShowReminderImage == value)
					return;
				lastShouldShowReminderImage = value;
				SetValue(ShouldShowReminderImageProperty, value);
			}
		}
		#endregion
		#region RecurrenceImage
		public BitmapImage RecurrenceImage {
			get { return (BitmapImage)GetValue(RecurrenceImageProperty); }
			set { SetValue(RecurrenceImageProperty, value); }
		}
		#endregion
		#region ShouldShowRecurrenceImage
		bool lastShouldShowRecurrenceImage = false;
		public bool ShouldShowRecurrenceImage {
			get { return (bool)GetValue(ShouldShowRecurrenceImageProperty); }
			set {
				if (lastShouldShowRecurrenceImage == value)
					return;
				lastShouldShowRecurrenceImage = value;
				SetValue(ShouldShowRecurrenceImageProperty, value);
			}
		}
		#endregion
		#region ChangedRecurrenceImage
		public BitmapImage ChangedRecurrenceImage {
			get { return (BitmapImage)GetValue(ChangedRecurrenceImageProperty); }
			set { SetValue(ChangedRecurrenceImageProperty, value); }
		}
		#endregion
		#region ShouldShowChangedRecurrenceImage
		bool lastShouldShowChangedRecurrenceImage = false;
		public bool ShouldShowChangedRecurrenceImage {
			get { return (bool)GetValue(ShouldShowChangedRecurrenceImageProperty); }
			set {
				if (lastShouldShowChangedRecurrenceImage == value)
					return;
				lastShouldShowChangedRecurrenceImage = value;
				SetValue(ShouldShowChangedRecurrenceImageProperty, value);
			}
		}
		#endregion
		protected internal void OnViewInfoChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if (oldValue != null)
				UnsubscribeAppointmentControlEvents(oldValue);
			if (newValue != null)
				SubscribeAppointmentControlEvents(newValue);
			UpdateImagesVisibility();
		}
		protected internal virtual void UnsubscribeAppointmentControlEvents(VisualAppointmentViewInfo viewInfo) {
			viewInfo.PropertiesChanged -= new EventHandler(OnViewInfoPropertyChanged);
		}
		protected internal virtual void SubscribeAppointmentControlEvents(VisualAppointmentViewInfo viewInfo) {
			viewInfo.PropertiesChanged += new EventHandler(OnViewInfoPropertyChanged);
		}
		private void OnViewInfoPropertyChanged(object sender, EventArgs e) {
			UpdateImagesVisibility();
		}
		protected internal virtual void UpdateImagesVisibility() {
			if (ViewInfo == null)
				return;
			Appointment apt = ((IAppointmentView)ViewInfo).Appointment;
			ShouldShowReminderImage = ViewInfo.Options.ShowBell;
			ShouldShowRecurrenceImage = ViewInfo.Options.ShowRecurrence && (apt.Type == AppointmentType.Occurrence);
			ShouldShowChangedRecurrenceImage = ViewInfo.Options.ShowRecurrence && apt.IsException;
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class DefaultAppointmentImages {
		static BitmapImage reminder;
		static BitmapImage recurrence;
		static BitmapImage changedRecurrence;
		static BitmapImage appointmentStartContinueArrow;
		static BitmapImage appointmentEndContinueArrow;
		static void DefaultAppointemntImages() {
		}
		public static BitmapImage Recurrence {
			get {
				if (recurrence == null)
					recurrence = CreateImage(AppointmentImagesNames.Recurrence);
				return recurrence;
			}
		}
		public static BitmapImage ChangedRecurrence {
			get {
				if (changedRecurrence == null)
					changedRecurrence = CreateImage(AppointmentImagesNames.ChangedRecurrence);
				return changedRecurrence;
			}
		}
		public static BitmapImage Reminder {
			get {
				if (reminder == null)
					reminder = CreateImage(AppointmentImagesNames.Reminder);
				return reminder;
			}
		}
		public static BitmapImage AppointmentStartContinueArrow {
			get {
				if (appointmentStartContinueArrow == null)
					appointmentStartContinueArrow = CreateImage(AppointmentImagesNames.AppointmentStartContinueArrow);
				return appointmentStartContinueArrow;
			}
		}
		public static BitmapImage AppointmentEndContinueArrow {
			get {
				if (appointmentEndContinueArrow == null)
					appointmentEndContinueArrow = CreateImage(AppointmentImagesNames.AppointmentEndContinueArrow);
				return appointmentEndContinueArrow;
			}
		}
		static BitmapImage CreateImage(string name) {
			string imagePath = string.Format("{0}.{1}.png", AppointmentImagesNames.ResourcePath, name);
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);
			XtraSchedulerDebug.CheckNotNull(imagePath, stream);
			return ImageHelper.CreateImageFromStream(stream);
		}
	}
	public abstract class AppointmentBorderBase : CalculatedBorderBase {
		#region Properties
		#region ViewInfo
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentBorderBase, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		void OnViewInfoChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if (newValue != null)
				newValue.PropertiesChanged += new EventHandler(OnViewInfoPropertiesChanged);
			if (oldValue != null)
				oldValue.PropertiesChanged -= new EventHandler(OnViewInfoPropertiesChanged);
			Update();
		}
		#endregion
		#endregion
		protected virtual void OnViewInfoPropertiesChanged(object sender, EventArgs e) {
			Update();
		}
		protected override void UpdatePadding() {
			Padding = DefaultPadding;
		}
		protected override void UpdateBorderThickness() {
			if (ViewInfo == null)
				return;
			Thickness borderThickness = DefaultBorderThickness;
			double left = (ViewInfo.HasLeftBorder) ? borderThickness.Left : 0;
			double right = (ViewInfo.HasRightBorder) ? borderThickness.Right : 0;
			double top = (ViewInfo.HasTopBorder) ? borderThickness.Top : 0;
			double bottom = (ViewInfo.HasBottomBorder) ? borderThickness.Bottom : 0;
			BorderThickness = new Thickness(left, top, right, bottom);
		}
	}
	public class AppointmentBorder : AppointmentBorderBase {
		protected override void UpdateCornerRadius() {
			if (ViewInfo == null)
				return;
			CornerRadius cornerRadius = DefaultCornerRadius;
			double topLeft = (ViewInfo.HasLeftBorder && ViewInfo.HasTopBorder) ? cornerRadius.TopLeft : 0;
			double topRight = (ViewInfo.HasRightBorder && ViewInfo.HasTopBorder) ? cornerRadius.TopRight : 0;
			double bottomLeft = (ViewInfo.HasLeftBorder && ViewInfo.HasBottomBorder) ? cornerRadius.BottomLeft : 0;
			double bottomRight = (ViewInfo.HasRightBorder && ViewInfo.HasBottomBorder) ? cornerRadius.BottomRight : 0;
			CornerRadius = new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
		}
		protected override void UpdateMargin() {
			if (ViewInfo == null)
				return;
			Thickness margin = DefaultMargin;
			Thickness noBorderMargin = NoBorderMargin;
			double left = ViewInfo.HasLeftBorder ? margin.Left : noBorderMargin.Left;
			double right = ViewInfo.HasRightBorder ? margin.Right : noBorderMargin.Right;
			double top = ViewInfo.HasTopBorder ? margin.Top : noBorderMargin.Top;
			double bottom = ViewInfo.HasBottomBorder ? margin.Bottom : noBorderMargin.Bottom;
			Margin = new Thickness(left, top, right, bottom);
		}
	}
}
