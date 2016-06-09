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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using DevExpress.Xpf.Utils;
using System.Windows.Controls.Primitives;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using RoutedCommand = DevExpress.Xpf.Core.WPFCompatibility.RoutedCommand;
using DevExpress.Xpf.Bars;
#endif
namespace DevExpress.Xpf.Core {
	public class SmoothScroller : Decorator
#if !SL
	, IManipulationClientEx
#endif    
	{
		enum ScrollingAction { ScrollUp, ScrollDown, NotScrolling }
		static readonly TimeSpan defaultUpdateInterval = new TimeSpan(0, 0, 0, 0, 200);
		public static readonly RoutedCommand ScrollUpCommand;
		public static readonly RoutedCommand ScrollDownCommand;
		public static readonly DependencyProperty AllowScrollUpProperty;
		public static readonly DependencyProperty AllowScrollDownProperty;
		public static readonly DependencyProperty ScrollSpeedProperty;
		public static readonly DependencyProperty AccelerationProperty;
		public static readonly DependencyProperty DecelerationProperty;
		public static readonly DependencyProperty ChildOffsetProperty;
		public static readonly DependencyProperty TopBottomSpaceProperty;
		public static readonly DependencyProperty OrientationProperty;
#if SL
		internal static readonly DependencyProperty IntActualHeightProperty;
		internal static readonly DependencyProperty IntActualWidthProperty;
#endif
		static SmoothScroller() {
			ScrollUpCommand = new RoutedCommand("ScrollUp", typeof(SmoothScroller));
			ScrollDownCommand = new RoutedCommand("ScrollDown", typeof(SmoothScroller));
			CommandManager.RegisterClassCommandBinding(typeof(SmoothScroller), new CommandBinding(ScrollUpCommand, OnScrollUp, OnCanExecuteScrollUp));
			CommandManager.RegisterClassCommandBinding(typeof(SmoothScroller), new CommandBinding(ScrollDownCommand, OnScrollDown, OnCanExecuteScrollDown));
#if !SL
			ClipToBoundsProperty.OverrideMetadata(typeof(SmoothScroller), new FrameworkPropertyMetadata(true));
#else
			IntActualHeightProperty = DependencyProperty.Register("IntActualHeight", typeof(double), typeof(SmoothScroller), new PropertyMetadata(0d, (d,e)=>((SmoothScroller)d).OnIntActualHeightChanged()));
			IntActualWidthProperty = DependencyProperty.Register("IntActualWidth", typeof(double), typeof(SmoothScroller), new PropertyMetadata(0d, (d, e) => ((SmoothScroller)d).OnIntActualWidthChanged()));
#endif
			AllowScrollUpProperty = DependencyPropertyManager.Register("AllowScrollUp", typeof(bool), typeof(SmoothScroller), new FrameworkPropertyMetadata(false, OnAllowScrollUpChanged));
			AllowScrollDownProperty = DependencyPropertyManager.Register("AllowScrollDown", typeof(bool), typeof(SmoothScroller), new FrameworkPropertyMetadata(false, OnAllowScrollDownChanged));
			ScrollSpeedProperty = DependencyPropertyManager.Register("ScrollSpeed", typeof(double), typeof(SmoothScroller), new FrameworkPropertyMetadata(200d));
			AccelerationProperty = DependencyPropertyManager.Register("Acceleration", typeof(double), typeof(SmoothScroller), new FrameworkPropertyMetadata(200d));
			DecelerationProperty = DependencyPropertyManager.Register("Deceleration", typeof(double), typeof(SmoothScroller), new FrameworkPropertyMetadata(200d));
			ChildOffsetProperty = DependencyPropertyManager.Register("ChildOffset", typeof(double), typeof(SmoothScroller), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, OnChildOffsetChanged, CoerceChildOffset));
			TopBottomSpaceProperty = DependencyPropertyManager.Register("TopBottomSpace", typeof(double), typeof(SmoothScroller), new FrameworkPropertyMetadata(0d, (d,e) => ((SmoothScroller)d).OnTopBottomSpaceChanged()));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(SmoothScroller), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged));
		}
		public static void OnScrollUp(object sender, ExecutedRoutedEventArgs e) {
			((SmoothScroller)sender).PerformScrollUpCommand();
		}
		public static void OnScrollDown(object sender, ExecutedRoutedEventArgs e) {
			((SmoothScroller)sender).PerformScrollDownCommand();
		}
		public static void OnCanExecuteScrollUp(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = ((SmoothScroller)sender).AllowScrollUp;
		}
		public static void OnCanExecuteScrollDown(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = ((SmoothScroller)sender).AllowScrollDown;
		}
		public static void OnAllowScrollUpChanged(object sender, DependencyPropertyChangedEventArgs e) {
			CommandManager.InvalidateRequerySuggested();
			((SmoothScroller)sender).UpdateAllowScrolling();
		}
		public static void OnAllowScrollDownChanged(object sender, DependencyPropertyChangedEventArgs e) {
			CommandManager.InvalidateRequerySuggested();
			((SmoothScroller)sender).UpdateAllowScrolling();
		}
		static void OnChildOffsetChanged(object sender, DependencyPropertyChangedEventArgs e) {
			((SmoothScroller)sender).OnChildOffsetChanged();
		}
		private static object CoerceChildOffset(DependencyObject d, object value) {
			return ((SmoothScroller)d).CoerceChildOffset((double)value);
		}
		static void OnOrientationChanged(object sender, DependencyPropertyChangedEventArgs e) {
			((SmoothScroller)sender).UpdateAllowScrollDownBinding();
		}
		public SmoothScroller() {
			Loaded += OnLoaded;
#if SL
			FrameworkElementHelper.SetIsClipped(this, true);
			SizeChanged += OnSizeChanged;
			Initialize();
#endif
		}
		#region Properties
		SizeHelperBase sizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		double actualHeight { get { return Orientation == Orientation.Vertical ? ActualHeight : ActualWidth; } }
		DispatcherTimer timer;
		ScrollingAction action = ScrollingAction.NotScrolling;
		Storyboard storyBoard;
		DoubleAnimation Animation { get { return storyBoard != null ? (DoubleAnimation)storyBoard.Children[0] : null; } }
		protected internal double ContentHeight { get { return (sizeHelper.GetDefineSize(Child.DesiredSize) + TopBottomSpace); } }
		public bool AllowScrollUp {
			get { return (bool)GetValue(AllowScrollUpProperty); }
			set { SetValue(AllowScrollUpProperty, value); }
		}
		public bool AllowScrollDown {
			get { return (bool)GetValue(AllowScrollDownProperty); }
			set { SetValue(AllowScrollDownProperty, value); }
		}
		public double ScrollSpeed {
			get { return (double)GetValue(ScrollSpeedProperty); }
			set { SetValue(ScrollSpeedProperty, value); }
		}
		public double Acceleration {
			get { return (double)GetValue(AccelerationProperty); }
			set { SetValue(AccelerationProperty, value); }
		}
		public double Deceleration {
			get { return (double)GetValue(DecelerationProperty); }
			set { SetValue(DecelerationProperty, value); }
		}
		public double ChildOffset {
			get { return (double)GetValue(ChildOffsetProperty); }
			set { SetValue(ChildOffsetProperty, value); }
		}
		public double TopBottomSpace {
			get { return (double)GetValue(TopBottomSpaceProperty); }
			set { SetValue(TopBottomSpaceProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}		
#if SL
		internal double IntActualHeight {
			get { return (double)GetValue(IntActualHeightProperty); }
			set { SetValue(IntActualHeightProperty, value); }
		}
		internal double IntActualWidth {
			get { return (double)GetValue(IntActualWidthProperty); }
			set { SetValue(IntActualWidthProperty, value); }
		}
#endif
		#endregion
#if !SL
		IInputElement IManipulationClient.GetManipulationContainer() {
			return this;
		}
		Vector IManipulationClient.GetScrollValue() {
			Point pt = new Point(0, ChildOffset);
			return new Vector(sizeHelper.GetSecondaryPoint(pt), sizeHelper.GetDefinePoint(pt));
		}
		Vector IManipulationClient.GetMaxScrollValue() {
			return new Vector(0, ContentHeight - sizeHelper.GetDefineSize(DesiredSize));
		}		
		Vector IManipulationClient.GetMinScrollValue() {
			return new Vector(0, 0);
		}
		Vector IManipulationClientEx.GetMinScrollDelta() {
			return GetMinScrollDeltaOverride();
		}
		protected virtual Vector GetMinScrollDeltaOverride() {
			return new Vector(0, 0);
		}
		void IManipulationClient.Scroll(Vector scrollValue) {
			ChildOffset = sizeHelper.GetDefinePoint(new Point(scrollValue.X, scrollValue.Y));
		}
		ManipulationHelper manipulationHelper;
		ManipulationHelper ManipulationHelper {
			get {
				if(manipulationHelper == null)
					manipulationHelper = new ManipulationHelper(this);
				return manipulationHelper;
			}
		}
		bool ShouldStartManipulationInertia { get; set; }
		protected override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			base.OnManipulationStarting(e);
			ManipulationHelper.OnManipulationStarting(e);
			ShouldStartManipulationInertia = false;
		}
		protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			if(!ShouldStartManipulationInertia) {
				e.Cancel();
				return;
			}
			base.OnManipulationInertiaStarting(e);
			ManipulationHelper.OnManipulationInertiaStarting(e);
		}
		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			base.OnManipulationDelta(e);
			ManipulationHelper.OnManipulationDelta(e);
			double delta = Orientation == System.Windows.Controls.Orientation.Vertical? e.CumulativeManipulation.Translation.Y: e.CumulativeManipulation.Translation.X;
			ShouldStartManipulationInertia |= delta > 0;
		}
#endif
		protected override Size MeasureOverride(Size constraint) {
			if(Child != null) {
				Child.Measure(sizeHelper.CreateSize(double.PositiveInfinity, sizeHelper.GetSecondarySize(constraint)));
				if(ContentHeight < sizeHelper.GetDefineSize(constraint))
					ChildOffset = 0;
				else if(ContentHeight - ChildOffset < sizeHelper.GetDefineSize(constraint))
					ChildOffset = ContentHeight - sizeHelper.GetDefineSize(constraint);
				if(constraint.Height == double.PositiveInfinity) constraint.Height = Child.DesiredSize.Height;
				if(constraint.Width == double.PositiveInfinity) constraint.Width = Child.DesiredSize.Width;
				return constraint;
			}
			return new Size();
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if(Child != null) {
				Size res = sizeHelper.CreateSize(Math.Max(sizeHelper.GetDefineSize(Child.DesiredSize), sizeHelper.GetDefineSize(arrangeSize) - TopBottomSpace), sizeHelper.GetSecondarySize(arrangeSize));
				Child.Arrange(new Rect(sizeHelper.CreatePoint(-ChildOffset, 0), res));
			}
			return arrangeSize;
		}
		void Initialize() {
			ClearValue(AllowScrollDownProperty);
			ClearValue(AllowScrollUpProperty);
			SetBinding(AllowScrollUpProperty, new Binding(ChildOffsetProperty.GetName()) {
				Source = this,
				Converter = new ContentOffsetToAllowScrollUpConverter()
			});
			UpdateAllowScrollDownBinding();
		}		
#if SL
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			IntActualHeight = e.NewSize.Height;
			IntActualWidth = e.NewSize.Width;
		}
#endif
		#region Implementation
		public void StartScrollDown() {
			double to = ContentHeight - actualHeight;
			StartScroll(to, to - ChildOffset);
		}
		public void StartScrollUp() {
			StartScroll(0, ChildOffset);
		}
		void StartScroll(double to, double distance) {
			StopAnimation();
			if(distance <= 0)
				return;
			double duration;
			double accelerationDistance = (ScrollSpeed * ScrollSpeed) / (2 * Acceleration);
			double decelerationDistance = (ScrollSpeed * ScrollSpeed) / (2 * Deceleration);
			double accelerationTime;
			double decelerationTime;
			if(distance > accelerationDistance + decelerationDistance) {
				double constantSpeedTime = (distance - accelerationDistance - decelerationDistance) / ScrollSpeed;
				accelerationTime = ScrollSpeed / Acceleration;
				decelerationTime = ScrollSpeed / Deceleration;
				duration = accelerationTime + constantSpeedTime + decelerationTime;
			}
			else {
				accelerationTime = Math.Sqrt((2 * distance * Deceleration) / (Acceleration * Deceleration + Acceleration * Acceleration));
				decelerationTime = Math.Sqrt((2 * distance * Acceleration) / (Acceleration * Deceleration + Deceleration * Deceleration));
				duration = accelerationTime + decelerationTime;
			}
			double accelerationRatio = accelerationTime / duration;
			double decelerationRatio = decelerationTime / duration;
			StartAnimation(to, duration, accelerationRatio, decelerationRatio);
		}
		public void StopScrollDown() {
			StopScroll(1);
		}
		public void StopScrollUp() {
			StopScroll(-1);
		}
		public void StopScroll(double distanceSign) {
			StopAnimation();
			if(Animation == null || AreCloseToCurrentContentOffset(Animation.To.Value) || AreCloseToCurrentContentOffset(Animation.From.Value))
				return;
			double currentSpeed = GetCurrentAnimationSpeed(ChildOffset, ScrollSpeed);
			double distance = (currentSpeed * currentSpeed) / (2 * Deceleration);
			double duration = currentSpeed / Deceleration;
			double maxDistance = Math.Abs(ChildOffset - Animation.To.Value);
			if(maxDistance < distance) {
				distance = maxDistance;
				duration = (2 * maxDistance) / currentSpeed;
			}
			StartAnimation(Math.Ceiling(ChildOffset + distanceSign * distance), duration, 0, 1);
		}
		bool AreCloseToCurrentContentOffset(double value) {
			return Math.Abs(ChildOffset - value) < 1;
		}
		void StartAnimation(double to, double duration, double accelerationRatio, double decelerationRatio) {
			storyBoard = new Storyboard();
			storyBoard.Completed += OnStoryboardCompleted;
			Storyboard.SetTargetProperty(storyBoard, new PropertyPath(ChildOffsetProperty.GetName()));
			DoubleAnimation animation = new DoubleAnimation();
			animation.From = ChildOffset;
			animation.To = to;
			animation.Duration = new Duration(TimeSpan.FromSeconds(duration));
#if !SL
			animation.AccelerationRatio = accelerationRatio;
			animation.DecelerationRatio = decelerationRatio;
#else
			animation.SpeedRatio = ScrollSpeed / 300;
			animation.EasingFunction = new QuarticEase();
#endif
			storyBoard.Children.Add(animation);
#if !SL
			storyBoard.Begin(this, HandoffBehavior.Compose, true);
#else
			storyBoard.Begin(this, true);
#endif
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			StopAnimation();			
		}
		protected virtual void PerformScrollUpCommand() {
			if(action == ScrollingAction.ScrollDown) {
				StopTimer();
				StopScrollDown();
			}
			if(action != ScrollingAction.ScrollUp) {
				StartScrollUp();
				action = ScrollingAction.ScrollUp;
			}
			if(timer != null)
				StopTimer();
			StartScrollUpTimer();
		}
		protected virtual void PerformScrollDownCommand() {
			if(action == ScrollingAction.ScrollUp) {
				StopTimer();
				StopScrollUp();
			}
			if(action != ScrollingAction.ScrollDown) {
				StartScrollDown();
				action = ScrollingAction.ScrollDown;
			}
			if(timer != null)
				StopTimer();
			StartScrollDownTimer();
		}
		void StartScrollUpTimer() {
#if !SL
			timer = new DispatcherTimer(DispatcherPriority.Render);
#else
			timer = new DispatcherTimer();
#endif
			timer.Interval = defaultUpdateInterval;
			timer.Tick += new EventHandler(TimerTickScrollUp);
			timer.Start();
		}
		void StartScrollDownTimer() {
#if !SL
			timer = new DispatcherTimer(DispatcherPriority.Render);
#else
			timer = new DispatcherTimer();
#endif
			timer.Interval = defaultUpdateInterval;
			timer.Tick += new EventHandler(TimerTickScrollDown);
			timer.Start();
		}
		void TimerTickScrollUp(object sender, EventArgs e) {
			action = ScrollingAction.NotScrolling;
			StopScrollUp();
			StopTimer();
		}
		void TimerTickScrollDown(object sender, EventArgs e) {
			StopScrollDown();
			action = ScrollingAction.NotScrolling;
			StopTimer();
			Debug.WriteLine("Stop timer");
		}
		void StopTimer() {
			if(timer != null)
				timer.Stop();
			timer = null;
		}
		void StopAnimation() {
			double offset = ChildOffset;
			if (storyBoard != null) {
				storyBoard.Completed -= OnStoryboardCompleted;
#if !SL
				storyBoard.Remove(this);
#else
				storyBoard.Stop();
#endif
				storyBoard = null;
			}
			ChildOffset = offset;
		}
		void ScrollUp() {
			StopAnimation();
			ChildOffset -= 30;
			ChildOffset = Math.Max(ChildOffset, 0);
		}
		void ScrollDown() {
			StopAnimation();
			ChildOffset += 30;
			if(ChildOffset > Math.Max(0, ContentHeight - actualHeight))
				ChildOffset = Math.Max(0, ContentHeight - actualHeight);
		}
		double GetCurrentAnimationSpeed(double currentValue, double animationSpeed) {
			double duration = Animation.Duration.TimeSpan.TotalSeconds;
			double accelerationRatio = 1;
			double decelerationRatio = 1;
#if !SL
			accelerationRatio = Animation.AccelerationRatio;
			decelerationRatio = Animation.DecelerationRatio;
#endif
			double accelerationDistance = (accelerationRatio * animationSpeed * duration) / 2;
			double constantDistance = (1 - accelerationRatio - decelerationRatio) * animationSpeed * duration;
			double decelerationDistance = (decelerationRatio * animationSpeed * duration) / 2;
			double distance = Math.Abs(currentValue - Animation.From.Value);
			if(distance < accelerationDistance)
				return animationSpeed * Math.Sqrt(distance / accelerationDistance);
			if(distance < accelerationDistance + constantDistance)
				return animationSpeed;
			return animationSpeed * Math.Sqrt((distance - accelerationDistance - constantDistance) / decelerationDistance);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			Initialize();
		}
		double CoerceChildOffset(double newValue) {
			double maxOffsetValue = sizeHelper.GetDefineSize(Child.DesiredSize) + TopBottomSpace -
				(Orientation == Orientation.Vertical ? ActualHeight : ActualWidth);
			if(newValue < 0 || maxOffsetValue < 0)
				return 0;
			if(newValue > maxOffsetValue)
				return maxOffsetValue;
			return newValue;
		}
		void OnChildOffsetChanged() {
			UpdateAllowScrollDown();
		}
		void OnTopBottomSpaceChanged() {
			UpdateAllowScrollDown();
		}
		void UpdateAllowScrollDown() {
#if SL
			double currentContentOffset = ChildOffset;
			double topBottomSpace = TopBottomSpace;
			double contentHeight = Orientation == Orientation.Vertical ? IntChildActualHeight : IntChildActualWidth;
			double actualHeight = Orientation == Orientation.Vertical ? IntActualHeight : IntActualWidth;
			AllowScrollDown = (bool)(currentContentOffset < Math.Max(0, (contentHeight + topBottomSpace) - actualHeight));
#endif
		}
#if SL
		protected override void OnIntChildActualHeightChanged() {
			base.OnIntChildActualHeightChanged();
			UpdateAllowScrollDown();
		}
		protected override void OnIntChildActualWidthChanged() {
			base.OnIntChildActualWidthChanged();
			UpdateAllowScrollDown();
		}
#endif
		void OnIntActualHeightChanged() {
			UpdateAllowScrollDown();
		}
		void OnIntActualWidthChanged() {
			UpdateAllowScrollDown();
		}
		void UpdateAllowScrollDownBinding() {
#if !SL
			MultiBinding allowScrollDownBinding = new MultiBinding() {
				Converter = new ContentOffsetToAllowScrollDownConverter()
			};
			allowScrollDownBinding.Bindings.Add(new Binding(ChildOffsetProperty.GetName()) { Source = this });
			allowScrollDownBinding.Bindings.Add(new Binding(TopBottomSpaceProperty.GetName()) { Source = this });
			allowScrollDownBinding.Bindings.Add(new Binding(Orientation == Orientation.Vertical ? "ActualHeight" : "ActualWidth") { Source = Child });
			allowScrollDownBinding.Bindings.Add(new Binding(Orientation == Orientation.Vertical ? "ActualHeight" : "ActualWidth") { Source = this });
			SetBinding(AllowScrollDownProperty, allowScrollDownBinding);
#endif
		}
		protected virtual void UpdateAllowScrolling() { }
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			if(e.Delta > 0)
				ScrollUp();
			else
				ScrollDown();
		}
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
			base.OnPreviewMouseWheel(e);
		}
		#endregion
		class ContentOffsetToAllowScrollUpConverter : MarkupExtension, IValueConverter {
		#region IValueConverter Members
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				return (bool)((double)value > 0);
			}
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				throw new NotImplementedException();
			}
			#endregion
			public override object ProvideValue(IServiceProvider serviceProvider) {
				return this;
			}
		}
#if !SL
		class ContentOffsetToAllowScrollDownConverter : MarkupExtension, IMultiValueConverter {
		#region IValueConverter Members
			public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				double currentContentOffset = (double)values[0];
				double topBottomSpace = (double)values[1];
				double contentHeight = values[2] != DependencyProperty.UnsetValue ? (double)values[2] : 0;
				double actualHeight = (double)values[3];
				return (bool)(currentContentOffset < Math.Max(0, (contentHeight + topBottomSpace) - actualHeight));
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
				throw new NotImplementedException();
			}
		#endregion
			public override object ProvideValue(IServiceProvider serviceProvider) {
				return this;
			}
		}
#endif
	}
#if !SL
	public interface IManipulationClient {
		IInputElement GetManipulationContainer();
		Vector GetScrollValue();
		Vector GetMaxScrollValue();
		Vector GetMinScrollValue();		
		void Scroll(Vector scrollValue);
	}
	public interface IManipulationClientEx : IManipulationClient {
		Vector GetMinScrollDelta();
	}
	public class ScrollInfoManipulationClient : IManipulationClient {
		IScrollInfo ScrollInfo { get; set; }
		public ScrollInfoManipulationClient(IScrollInfo scrollInfo) {
			ScrollInfo = scrollInfo;
		}
		IInputElement IManipulationClient.GetManipulationContainer() {
			return ScrollInfo.ScrollOwner;
		}
		Vector IManipulationClient.GetScrollValue() {
			return new Vector(ScrollInfo.HorizontalOffset, ScrollInfo.VerticalOffset);
		}
		Vector IManipulationClient.GetMaxScrollValue() {
			return new Vector(ScrollInfo.ExtentWidth - ScrollInfo.ViewportWidth, ScrollInfo.ExtentHeight - ScrollInfo.ViewportHeight);
		}
		Vector IManipulationClient.GetMinScrollValue() {
			return new Vector(0, 0);
		}
		void IManipulationClient.Scroll(Vector scrollValue) {
			ScrollInfo.SetHorizontalOffset(scrollValue.X);
			ScrollInfo.SetVerticalOffset(scrollValue.Y);
		}
	}
	public class ManipulationHelper {
		static Vector emptyVector = new Vector(0, 0);
		public Point accumulator;
		public ManipulationHelper(IManipulationClient client) {
			Client = client;
			DesiredDeceleration = 10 * 96;
			BoundaryFeedbackTime = 200;
			AllowVerticalScrolling = true;
		}
		public ManipulationHelper(IScrollInfo scrollInfo) : this(new ScrollInfoManipulationClient(scrollInfo)) { 
		}
		public bool AllowVerticalScrolling { get; set; }
		public bool AllowHorizontalScrolling { get; set; }
		public double BoundaryFeedbackTime { get; set; }
		public double DesiredDeceleration { get; set; }
		IManipulationClient Client { get; set; }
		protected long BoundTicks { get; private set; }
		Vector ScrollValue { get; set; }
		protected Vector MaxOverDelta { get; private set; }
		protected Vector Velocity { get; private set; }
		protected Vector BackDeceleration { get; private set; }
		protected bool IsObjectBounded { get; private set; }
		public virtual void OnManipulationStarting(ManipulationStartingEventArgs e) {
			e.ManipulationContainer = Client.GetManipulationContainer();
			ScrollValue = Client.GetScrollValue();
			IsObjectBounded = false;
			accumulator = new Point();
			e.Handled = true;
		}
		public virtual void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			e.TranslationBehavior.DesiredDeceleration = DesiredDeceleration / (1000 * 1000);
			e.Handled = true;
		}
		Vector GetOverDelta() {
			Vector res = new Vector();
			if(AllowHorizontalScrolling && ScrollValue.X < Client.GetMinScrollValue().X) {
				res.X = Client.GetMinScrollValue().X - ScrollValue.X;
			}
			if(AllowVerticalScrolling && ScrollValue.Y < Client.GetMinScrollValue().Y) {
				res.Y = Client.GetMinScrollValue().Y - ScrollValue.Y;
			}
			if(AllowHorizontalScrolling &&  ScrollValue.X > Client.GetMaxScrollValue().X) {
				res.X = Client.GetMaxScrollValue().X - ScrollValue.X;
			}
			if(AllowVerticalScrolling && ScrollValue.Y > Client.GetMaxScrollValue().Y) {
				res.Y = Client.GetMaxScrollValue().Y - ScrollValue.Y;
			}
			return res;
		}
		Vector GetOverDelta2(double t) {
			Vector res = new Vector();
			if(AllowHorizontalScrolling)
				res.X = MaxOverDelta.X + Velocity.X * t - BackDeceleration.X * t * t;
			if(AllowVerticalScrolling)
				res.Y = MaxOverDelta.Y + Velocity.Y * t - BackDeceleration.Y * t * t;
			return res;
		}
		public virtual void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			ScrollValue = new Vector(ScrollValue.X - e.DeltaManipulation.Translation.X, ScrollValue.Y - e.DeltaManipulation.Translation.Y);
			DevExpress.Xpf.Core.Native.PointHelper.Offset(ref accumulator, e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
			IManipulationClientEx clientEx = Client as IManipulationClientEx;
			if (clientEx != null) {
				if (AllowHorizontalScrolling && clientEx.GetMinScrollDelta().X > Math.Abs(accumulator.X) || AllowVerticalScrolling && clientEx.GetMinScrollDelta().Y > Math.Abs(accumulator.Y)) {
					e.Handled = true;
					return;
				}					
			}
			Client.Scroll(ScrollValue);
			if(IsObjectBounded) {
				long ms = (DateTime.Now.Ticks - BoundTicks) / TimeSpan.TicksPerMillisecond;
				if(ms < BoundaryFeedbackTime) {
					Vector delta = GetOverDelta();
					if(ms > BoundaryFeedbackTime * 0.5) {
						if(MaxOverDelta == emptyVector) {
							MaxOverDelta = delta;
							Velocity = e.Velocities.LinearVelocity;
							BackDeceleration = new Vector((MaxOverDelta.X + Velocity.X * (BoundaryFeedbackTime * 0.5)) / (0.25 * BoundaryFeedbackTime * BoundaryFeedbackTime), (MaxOverDelta.Y + Velocity.Y * (BoundaryFeedbackTime * 0.5)) / (0.25 * BoundaryFeedbackTime * BoundaryFeedbackTime));
						}
						delta = GetOverDelta2(ms - BoundaryFeedbackTime * 0.5);
					}
					e.ReportBoundaryFeedback(new ManipulationDelta(delta, 0, new Vector(), new Vector()));
				}
				else {
					e.Complete();
				}
			}
			else {
				if((AllowHorizontalScrolling && (ScrollValue.X > Client.GetMaxScrollValue().X || ScrollValue.X < Client.GetMinScrollValue().X)) || 
					(AllowVerticalScrolling && (ScrollValue.Y > Client.GetMaxScrollValue().Y || ScrollValue.Y < Client.GetMinScrollValue().Y))) {
					BoundTicks = DateTime.Now.Ticks;
					MaxOverDelta = new Vector(0,0);
					IsObjectBounded = true;
				}
			}
			e.Handled = true;
		}
	}
#endif
}
