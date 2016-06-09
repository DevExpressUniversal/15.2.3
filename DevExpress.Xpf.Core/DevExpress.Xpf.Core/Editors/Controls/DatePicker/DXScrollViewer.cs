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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
namespace DevExpress.Xpf.Editors {
	public static class DXScrollViewerExtensions {
		public static readonly DependencyProperty ScrollDataProperty =
			DependencyPropertyManager.RegisterAttached("ScrollData", typeof(ScrollData),
			typeof(DXScrollViewerExtensions), new PropertyMetadata(null, OnScrollDataChanged));
		public static ScrollData GetScrollData(DependencyObject d) {
			return (ScrollData)d.GetValue(ScrollDataProperty);
		}
		public static void SetScrollData(DependencyObject d, ScrollData value) {
			d.SetValue(ScrollDataProperty, value);
		}
		static void OnScrollDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScrollData oldScrollData = (ScrollData)e.OldValue;
			ScrollData newScrollData = (ScrollData)d.GetValue(ScrollDataProperty);
			DXScrollViewer scrollViewer = d as DXScrollViewer;
			if (oldScrollData != null)
				oldScrollData.ScrollOwner = null;
			if (newScrollData != null)
				newScrollData.ScrollOwner = scrollViewer;
		}
		public static void AnimateScrollToVerticalOffset(this DXScrollViewer scrollViewer, double offset, Action onStart = null, Action onCompleted = null, Action preCompleted = null, Func<double, double> ensureStep = null, ScrollDataAnimationEase animationEase = ScrollDataAnimationEase.BeginAnimation) {
			if (scrollViewer.VerticalOffset.AreClose(offset))
				return;
			ScrollData data = GetScrollData(scrollViewer);
			if (data == null) {
				data = new ScrollData();
				SetScrollData(scrollViewer, data);
			}
			onStart.Do(x => x());
			data.AnimateScrollToVerticalOffset(offset, preCompleted, onCompleted, ensureStep, animationEase);
		}
		public static void StopCurrentAnimatedScroll(this DXScrollViewer scrollViewer) {
			ScrollData data = GetScrollData(scrollViewer);
			if (data == null)
				return;
			data.StopCurrentAnimatedScroll();
		}
	}
	public enum ScrollDataAnimationEase {
		BeginAnimation,
		Linear
	}
	public class ScrollData : FrameworkElement {
		const double AnimationSpeed = 0.8d;
		readonly Slider slider;
		Storyboard animation;
		DoubleAnimation verticalOffsetAnimation;
		bool StopAnimation { get; set; }
		double To { get; set; }
		Action PreCompleted { get; set; }
		Action OnCompleted { get; set; }
		Func<double, double> EnsureStep { get; set; }
		public ScrollData() {
			slider = new Slider { SmallChange = 0.0000000001, Minimum = double.MinValue, Maximum = double.MaxValue };
			slider.ValueChanged += OnVerticalOffsetChanged;
		}
		public DXScrollViewer ScrollOwner { get; set; }
		void OnVerticalOffsetChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			if (!StopAnimation) {
				ScrollOwner.Do(x => x.IsIntermediate = true);
				if (e.NewValue.AreClose(To))
					PreCompleted.Do(x => x());
				ScrollOwner.Do(x => x.ScrollToVerticalOffset(EnsureStep != null ? EnsureStep(e.NewValue) : e.NewValue));
				if (e.NewValue.AreClose(To))
					OnCompleted.Do(x => x());
			}
		}
		public void AnimateScrollToVerticalOffset(double offset, Action preCompleted, Action onCompleted, Func<double, double> ensureStep, ScrollDataAnimationEase animationEase) {
			if (ScrollOwner == null)
				return;
			StopCurrentAnimatedScroll();
			slider.Value = ScrollOwner.VerticalOffset;
			EnsureStep = ensureStep;
			PreCompleted = preCompleted;
			OnCompleted = onCompleted;
			To = offset;
			animation = new Storyboard();
			double distance = Math.Abs(offset - ScrollOwner.VerticalOffset);
			if (distance.IsZero()) {
				PreCompleted.Do(x => x());
				ScrollOwner.ScrollToVerticalOffset(offset);
				return;
			}
			verticalOffsetAnimation = new DoubleAnimation {
				From = ScrollOwner.VerticalOffset, To = offset, Duration = TimeSpan.FromMilliseconds(distance / AnimationSpeed)
			};
			switch (animationEase) {
				case ScrollDataAnimationEase.BeginAnimation:
					verticalOffsetAnimation.EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut, Power = 2d};
					break;
				case ScrollDataAnimationEase.Linear:
					break;
			}
			animation.Children.Add(verticalOffsetAnimation);
			Storyboard.SetTarget(animation, slider);
			Storyboard.SetTargetProperty(verticalOffsetAnimation, new PropertyPath("Value"));
			StopAnimation = false;
			animation.Begin();
		}
		public void StopCurrentAnimatedScroll() {
			if (animation != null) {
				StopAnimation = true;
				animation.Stop();
			}
		}
	}
	public delegate void ViewChangedEventHandler(object sender, ViewChangedEventArgs e);
	public class DXScrollViewer : ScrollViewer {
		public static readonly DependencyProperty VerticalSnapPointsAlignmentProperty;
		public static readonly DependencyProperty HorizontalSnapPointsAlignmentProperty;
		public static readonly DependencyProperty IsLoopedProperty;
		public static readonly RoutedEvent ViewChangedEvent;
		static readonly TimeSpan MouseWheelCompleteInterval = TimeSpan.FromMilliseconds(300);
		const double ManipulationDeceleration = 0.008d;
		readonly DispatcherTimer timerToCompleteScrollByWheelManipulation;
		static DXScrollViewer() {
			Type ownerType = typeof(DXScrollViewer);
			VerticalSnapPointsAlignmentProperty = DependencyPropertyManager.Register("VerticalSnapPointsAlignment", typeof(SnapPointsAlignment), ownerType,
				new PropertyMetadata(SnapPointsAlignment.Center));
			HorizontalSnapPointsAlignmentProperty = DependencyPropertyManager.Register("HorizontalSnapPointsAlignment", typeof(SnapPointsAlignment), ownerType,
				new PropertyMetadata(SnapPointsAlignment.Center));
			IsLoopedProperty = DependencyPropertyManager.Register("IsLooped", typeof(bool), ownerType,
				new PropertyMetadata(false));
			ViewChangedEvent = EventManager.RegisterRoutedEvent("ViewChanged", RoutingStrategy.Bubble, typeof(ViewChangedEventHandler), ownerType);
		}
		public DXScrollViewer() {
			timerToCompleteScrollByWheelManipulation = new DispatcherTimer();
			timerToCompleteScrollByWheelManipulation.Tick += CompleteScrollByWheelManipulation;
			manipulationPairs = new List<Tuple<double, long>>();
		}
		public SnapPointsAlignment VerticalSnapPointsAlignment {
			get { return (SnapPointsAlignment)GetValue(VerticalSnapPointsAlignmentProperty); }
			set { SetValue(VerticalSnapPointsAlignmentProperty, value); }
		}
		public SnapPointsAlignment HorizontalSnapPointsAlignment {
			get { return (SnapPointsAlignment)GetValue(HorizontalSnapPointsAlignmentProperty); }
			set { SetValue(HorizontalSnapPointsAlignmentProperty, value); }
		}
		public bool IsLooped {
			get { return (bool)GetValue(IsLoopedProperty); }
			set { SetValue(IsLoopedProperty, value); }
		}
		public event ViewChangedEventHandler ViewChanged {
			add { AddHandler(ViewChangedEvent, value); }
			remove { RemoveHandler(ViewChangedEvent, value); }
		}
		public bool IsIntermediate { get; set; }
		bool manipulated;
		Point? PreviousPosition { get; set; }
		DateTime manipulationBeginTime;
		IList<Tuple<double, long>> manipulationPairs;
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			IsIntermediate = false;
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			if (e.Handled)
				return;
			this.StopCurrentAnimatedScroll();
			if (ScrollInfo != null) {
				if (e.Delta > 0)
					ScrollInfo.MouseWheelDown();
				else
					ScrollInfo.MouseWheelUp();
			}
			e.Handled = true;
			timerToCompleteScrollByWheelManipulation.Stop();
			timerToCompleteScrollByWheelManipulation.Interval = MouseWheelCompleteInterval;
			timerToCompleteScrollByWheelManipulation.Start();
		}
		void RaiseViewChangedEvent(bool isIntermediate) {
			RaiseEvent(new ViewChangedEventArgs(isIntermediate) { RoutedEvent = ViewChangedEvent, Source = this });
		}
		protected override void OnScrollChanged(ScrollChangedEventArgs e) {
			base.OnScrollChanged(e);
			RaiseViewChangedEvent(IsIntermediate);
			if (IsIntermediate)
				IsIntermediate = false;
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			this.StopCurrentAnimatedScroll();
			PreviousPosition = e.GetPosition(this);
			manipulated = false;
			manipulationBeginTime = DateTime.Now;
			manipulationPairs = new List<Tuple<double, long>>();
			CaptureMouse();
			base.OnPreviewMouseDown(e);
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			if (PreviousPosition.HasValue) {
				Point currentPosition = e.GetPosition(this);
				double dy = currentPosition.Y - PreviousPosition.Value.Y;
				PreviousPosition = currentPosition;
				if (DoubleExtensions.GreaterThanOrClose(Math.Abs(dy), 1d)) {
					IsIntermediate = true;
					long elapsed = (DateTime.Now - manipulationBeginTime).Milliseconds;
					if (elapsed > 0)
						manipulationPairs.Add(new Tuple<double, long>(dy, elapsed));
					ScrollToVerticalOffset(EnsureVerticalOffset(VerticalOffset - dy));
					manipulated = true;
					manipulationBeginTime = DateTime.Now;
				}
			}
			base.OnPreviewMouseMove(e);
		}
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			PreviousPosition = null;
			CalcManipulationInertia();
			ReleaseMouseCapture();
			if (manipulated)
				e.Handled = true;
			base.OnPreviewMouseUp(e);
		}
		internal double EnsureVerticalOffset(double offset) {
			if (!IsLooped)
				return offset;
			if (DoubleExtensions.GreaterThanOrClose(offset, 0d) && DoubleExtensions.LessThan(offset, ExtentHeight))
				return offset;
			return DoubleExtensions.GreaterThan(offset, 0d)
				? offset % ExtentHeight
				: ExtentHeight - Math.Abs(offset) % ExtentHeight;
		}
		double CoerceVerticalOffset(double offset) {
			if (!IsLooped && DoubleExtensions.GreaterThanOrClose(offset, ScrollableHeight))
				return ScrollableHeight;
			if (!IsLooped && DoubleExtensions.LessThanOrClose(offset, 0d))
				return 0d;
			return offset;
		}
		void CompleteScrollByWheelManipulation(object sender, EventArgs e) {
			double verticalSnap = GetClosestSnapPoint(Orientation.Vertical, VerticalOffset);
			if (verticalSnap.AreClose(VerticalOffset))
				RaiseViewChangedEvent(false);
			this.AnimateScrollToVerticalOffset(verticalSnap, null, null, () => IsIntermediate = false);
			timerToCompleteScrollByWheelManipulation.Stop();
		}
		void CalcManipulationInertia() {
			double averageSpeed = 0d;
			foreach (var pair in manipulationPairs) {
				double speed = pair.Item1 / pair.Item2;
				if (averageSpeed.AreClose(0d))
					averageSpeed = speed;
				else
					averageSpeed = (speed + averageSpeed) / 2d;
			}
			double distance = averageSpeed / ManipulationDeceleration;
			double offset = IsLooped ? VerticalOffset - distance : CoerceVerticalOffset(VerticalOffset - distance);
			ScrollToClosestSnapPoint(offset);
		}
		#region Scroll to snap point
		void ScrollToClosestSnapPoint(double offset) {
			double verticalSnapPoint = GetClosestSnapPoint(Orientation.Vertical, offset);
			if (verticalSnapPoint.AreClose(VerticalOffset))
				RaiseViewChangedEvent(false);
			this.AnimateScrollToVerticalOffset(verticalSnapPoint, null, null, () => IsIntermediate = false, EnsureVerticalOffset);
		}
		double GetClosestSnapPoint(Orientation orientation, double currentOffset) {
			IScrollSnapPointsInfo scrollSnapPointsInfo = ScrollInfo as IScrollSnapPointsInfo;
			if (scrollSnapPointsInfo == null)
				return currentOffset;
			switch (orientation == Orientation.Vertical ? VerticalSnapPointsAlignment : HorizontalSnapPointsAlignment) {
				case SnapPointsAlignment.Near:
					break;
				case SnapPointsAlignment.Far:
					currentOffset += (orientation == Orientation.Vertical ? ViewportHeight : ViewportWidth);
					break;
				case SnapPointsAlignment.Center:
					currentOffset += (orientation == Orientation.Vertical ? ViewportHeight / 2d : ViewportWidth / 2d);
					break;
			}
			if (orientation == Orientation.Vertical ? !scrollSnapPointsInfo.AreVerticalSnapPointsRegular : !scrollSnapPointsInfo.AreHorizontalSnapPointsRegular) {
				IEnumerable<float> snapPoints = scrollSnapPointsInfo.GetIrregularSnapPoints(orientation, orientation == Orientation.Vertical
					? VerticalSnapPointsAlignment : HorizontalSnapPointsAlignment);
				if (!snapPoints.Any())
					return currentOffset;
				Dictionary<float, float> distances = new Dictionary<float, float>();
				foreach (float snapPoint in snapPoints)
					distances.Add(snapPoint, Math.Abs((float)currentOffset - snapPoint));
				double offset = distances.OrderBy(x => x.Value).First().Key;
				switch (orientation == Orientation.Vertical ? VerticalSnapPointsAlignment : HorizontalSnapPointsAlignment) {
					case SnapPointsAlignment.Near:
						return offset;
					case SnapPointsAlignment.Far:
						return offset - (orientation == Orientation.Vertical ? ViewportHeight : ViewportWidth);
					case SnapPointsAlignment.Center:
						return offset - (orientation == Orientation.Vertical ? ViewportHeight / 2d : ViewportWidth / 2d);
				}
			}
			else {
				float offset;
				float regularSnapPoints = scrollSnapPointsInfo.GetRegularSnapPoints(orientation, orientation == Orientation.Vertical
					? VerticalSnapPointsAlignment : HorizontalSnapPointsAlignment, out offset);
				double half = regularSnapPoints / 2d;
				double limit = orientation == Orientation.Vertical ? ExtentHeight : ExtentWidth;
				if (IsLooped) {
					offset -= (float)limit;
					limit *= 2;
				}
				for (double o = offset; DoubleExtensions.LessThanOrClose(o, limit); o += regularSnapPoints) {
					double distance = Math.Abs(currentOffset - o);
					if (DoubleExtensions.LessThanOrClose(distance, half)) {
						switch (orientation == Orientation.Vertical ? VerticalSnapPointsAlignment : HorizontalSnapPointsAlignment) {
							case SnapPointsAlignment.Near:
								return o;
							case SnapPointsAlignment.Far:
								return o - (orientation == Orientation.Vertical ? ViewportHeight : ViewportWidth);
							case SnapPointsAlignment.Center:
								return o - (orientation == Orientation.Vertical ? ViewportHeight / 2d : ViewportWidth / 2d);
						}
					}
				}
			}
			return currentOffset;
		}
		#endregion
	}
}
