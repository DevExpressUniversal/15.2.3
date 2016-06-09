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

using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.UIAutomation;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using ManipulationInertiaStartingEventArgs = DevExpress.Xpf.Core.Native.SLManipulationInertiaStartingEventArgs;
using ManipulationDeltaEventArgs = DevExpress.Xpf.Core.Native.SLManipulationDeltaEventArgs;
using ManipulationCompletedEventArgs = DevExpress.Xpf.Core.Native.SLManipulationCompletedEventArgs;
using DevExpress.Xpf.Controls.Primitives;
#else
using System.Timers;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Xpf.Controls.Primitives;
#endif
namespace DevExpress.Xpf.WindowsUI {
	interface ISupportManipulation {
		void OnManipulationCompleted(ManipulationCompletedEventArgs e);
		void OnManipulationDelta(ManipulationDeltaEventArgs e);
		void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e);
		bool IsManipulationEnabled { get; }
	}
	public enum ItemCacheMode {
		None,
		CacheAll,
		CacheOnSelecting
	}
#if !SILVERLIGHT
#endif
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	public class FlipView : veSelector, ISupportManipulation {
		private const int AnimationDurationPrevNextItem = 500;
		private const int AnimationDurationManipulationCompleted = 300;
		private const double InertionDeseleration = 0.005;
		#region static
		public static readonly DependencyProperty IsAnimationEnabledProperty;
		public static readonly DependencyProperty IsManipulationInertiaEnabledProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ShowNextButtonProperty;
		public static readonly DependencyProperty ShowPreviousButtonProperty;
		public static readonly DependencyProperty NextCommandProperty;
		public static readonly DependencyProperty PreviousCommandProperty;
		public static readonly DependencyProperty ItemCacheModeProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemSizeProperty;
#if SILVERLIGHT
		public static readonly DependencyProperty IsManipulationEnabledProperty;
#endif
		static FlipView() {
			var dProp = new DependencyPropertyRegistrator<FlipView>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsAnimationEnabled", ref IsAnimationEnabledProperty, false);
			dProp.Register("IsManipulationInertiaEnabled", ref IsManipulationInertiaEnabledProperty, false,
				(d, e) => ((FlipView)d).OnIsManipulationInertiaEnabledChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(d, e) => ((FlipView)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue));
			dProp.Register("ItemSize", ref ItemSizeProperty, 0d,
				(d, e) => ((FlipView)d).OnItemSizeChanged((double)e.OldValue, (double)e.NewValue));
			dProp.Register("ShowNextButton", ref ShowNextButtonProperty, true,
				(d, e) => ((FlipView)d).UpdateKeys());
			dProp.Register("ShowPreviousButton", ref ShowPreviousButtonProperty, true,
				(d, e) => ((FlipView)d).UpdateKeys());
			dProp.Register("ItemCacheMode", ref ItemCacheModeProperty, ItemCacheMode.None);
			dProp.Register("PreviousCommand", ref PreviousCommandProperty, (ICommand)null);
			dProp.Register("NextCommand", ref NextCommandProperty, (ICommand)null);
#if SILVERLIGHT
			dProp.Register("IsManipulationEnabled", ref IsManipulationEnabledProperty, false);
#endif
		}
		#endregion
		ManipulationHelper manipulationHelper;
		public FlipView() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(FlipView);
			keyTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 2) };
			keyTimer.Tick += keyTimer_Tick;
#else
			keyTimer = new Timer(2000);
			keyTimer.Elapsed += keyTimer_Elapsed;
#endif
			timerElapsed = true;
			manipulationHelper = new ManipulationHelper(this);
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			if(PartNextButtonHorz == null) return;
		}
		protected override ISelectorItem CreateSelectorItem() {
			return new FlipViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is FlipViewItem;
		}
		FlipPanel FlipPanel { get { return PartItemsPanel as FlipPanel; } }
		ScrollViewer PartScrollViewer;
		FadingButton PartPrevButtonHorz, PartNextButtonHorz, PartPrevButtonVert, PartNextButtonVert;
		protected override void ClearTemplateChildren() {
			Action<FadingButton, RoutedEventHandler> clearButton = (button, onClick) => {
				if(button != null) {
					button.Click -= onClick;
					button.ClearValue(UIElement.IsManipulationEnabledProperty);
				}
			};
			clearButton(PartPrevButtonHorz, previousButton_Click);
			clearButton(PartNextButtonHorz, nextButton_Click);
			clearButton(PartPrevButtonVert, previousButton_Click);
			clearButton(PartNextButtonVert, nextButton_Click);
			base.ClearTemplateChildren();
		}
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartScrollViewer = GetTemplateChild("ScrollingHost") as ScrollViewer;
			PartPrevButtonHorz = GetTemplateChild("PreviousButtonHorizontal") as FadingButton;
			PartNextButtonHorz = GetTemplateChild("NextButtonHorizontal") as FadingButton;
			PartPrevButtonVert = GetTemplateChild("PreviousButtonVertical") as FadingButton;
			PartNextButtonVert = GetTemplateChild("NextButtonVertical") as FadingButton;
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			if(PartScrollViewer != null) {
#if !SILVERLIGHT
				this.PartScrollViewer.CanContentScroll = true;
#endif
				this.PartScrollViewer.SizeChanged += PartScrollViewer_SizeChanged;
			}
			Func<FadingButton, ButtonDirection, RoutedEventHandler, FadingButton> initButton = (button, direction, onClick) => {
				if(button != null) {
					button.Direction = direction;
					button.Click += onClick;
#if !SILVERLIGHT
					button.SetBinding(UIElement.IsManipulationEnabledProperty, new Binding("IsManipulationEnabled") { Source = this });
#endif
				}
				return button;
			};
			initButton(PartPrevButtonHorz, ButtonDirection.Left, previousButton_Click);
			initButton(PartNextButtonHorz, ButtonDirection.Right, nextButton_Click);
			initButton(PartPrevButtonVert, ButtonDirection.Up, previousButton_Click);
			initButton(PartNextButtonVert, ButtonDirection.Down, nextButton_Click);
			Dispatcher.BeginInvoke(new Action(() => {
				UpdateKeys();
			}));
		}
		void PartScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(FlipPanel != null) {
				if(Orientation == System.Windows.Controls.Orientation.Horizontal) {
					ItemSize = FlipPanel.ViewportWidth;
					ScrollViewerBehavior.SetHorizontalOffset(PartScrollViewer, SelectedIndex * ItemSize);
				}
				else {
					ItemSize = FlipPanel.ViewportHeight;
					ScrollViewerBehavior.SetVerticalOffset(PartScrollViewer, SelectedIndex * ItemSize);
				}
			}
			else {
				Dispatcher.BeginInvoke(new Action(() =>
				{
					if(Orientation == Orientation.Horizontal) {
						ItemSize = this.PartScrollViewer.ViewportWidth;
						ScrollViewerBehavior.SetHorizontalOffset(PartScrollViewer, SelectedIndex * ItemSize);
					}
					else {
						ItemSize = this.PartScrollViewer.ViewportHeight;
						ScrollViewerBehavior.SetVerticalOffset(PartScrollViewer, SelectedIndex * ItemSize);
					}
				}));
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			ResetTimer();
			UpdateKeys();
		}
#if !SILVERLIGHT
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if(e.OriginalSource is FadingButton || e.OriginalSource == PartScrollViewer)
				switch(e.Key) {
					case Key.Left:
					case Key.Up:
						PrevItem();
						e.Handled = true;
						break;
					case Key.Right:
					case Key.Down:
						NextItem();
						e.Handled = true;
						break;
				}
		}
#endif
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
			UpdateKeys();
			if (PartScrollViewer != null) {
				ItemSize = newValue == Orientation.Horizontal ? PartScrollViewer.ViewportWidth : PartScrollViewer.ViewportHeight;
				SetScrollViewerOffsetDependingOrientation(SelectedIndex * ItemSize);
			}
		}
		protected virtual void OnIsManipulationInertiaEnabledChanged(bool oldValue, bool newValue) {
			manipulationHelper.IsManipulationInertiaEnabled = newValue;
		}
		protected virtual void OnItemSizeChanged(double oldValue, double newValue) { }
		void SyncSelectedIndexWithOffset(double offset) {
			lockSelectedIndex++;
			if(ItemSize != 0)
				SelectedIndex = Math.Min((int)(offset / ItemSize + 0.5), Items.Count - 1);
			lockSelectedIndex--;
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new FlipViewAutomationPeer(this);
		}
		#region Private
		void ScrollWithAnimation(double from, double to, double duration) {
			if(from == to) {
				SyncSelectedIndexWithOffset(to);
				return;
			} else {
				StopAnimation();
			}
			DoubleAnimation animation = new DoubleAnimation() {
				From = from,
				To = to,
				Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
				FillBehavior = FillBehavior.HoldEnd,
				EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn }
			};
			Storyboard.SetTargetProperty(animation, new PropertyPath(Orientation == Orientation.Horizontal ? ScrollViewerBehavior.HorizontalOffsetProperty : ScrollViewerBehavior.VerticalOffsetProperty));
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(animation);
			Storyboard.SetTarget(storyboard, PartScrollViewer);
			storyboard.Completed += delegate {
				currentAnimation = null;
				SetScrollViewerOffsetDependingOrientation(to);
				storyboard.Stop();
				Dispatcher.BeginInvoke(new Action(() => {
					SyncSelectedIndexWithOffset(to);
				}));
			};
			currentAnimation = storyboard;
			storyboard.Begin();
		}
		Storyboard currentAnimation;
		void StopAnimation() {
			if(currentAnimation == null) return;
			currentAnimation.SkipToFill();
			currentAnimation.Stop();
			currentAnimation = null;
		}
		void nextButton_Click(object sender, RoutedEventArgs e) {
			NextItem();
		}
		void previousButton_Click(object sender, RoutedEventArgs e) {
			PrevItem();
		}
		DateTime lastStartTime;
		void ResetTimer() {
			TimeSpan span = DateTime.Now - lastStartTime;
			if(span.TotalMilliseconds < 100) {
				return;
			}
			lastStartTime = DateTime.Now;
			keyTimer.Stop();
			keyTimer.Start();
			timerElapsed = false;
		}
#if SILVERLIGHT
		DispatcherTimer keyTimer;
		void keyTimer_Tick(object sender, EventArgs e) {
			keyTimer.Stop();
			timerElapsed = true;
			Dispatcher.BeginInvoke(new Action(UpdateKeys));
		}
#else
		Timer keyTimer;
		void keyTimer_Elapsed(object sender, ElapsedEventArgs e) {
			keyTimer.Stop();
			timerElapsed = true;
			Dispatcher.Invoke(new Action(UpdateKeys), null);
		}
#endif
		#region Key state manager
		bool timerElapsed;
		protected virtual bool CanHideNavigationButtons() {
			return timerElapsed;
		}
		protected virtual void UpdateKeys() {
			if(PartPrevButtonHorz == null)
				return;
			HideButton(PartPrevButtonHorz);
			HideButton(PartPrevButtonVert);
			HideButton(PartNextButtonHorz);
			HideButton(PartNextButtonVert);
			FadingButton previousButton = Orientation == Orientation.Horizontal ? PartPrevButtonHorz : PartPrevButtonVert;
			FadingButton nextButton = Orientation == Orientation.Horizontal ? PartNextButtonHorz : PartNextButtonVert;
#if !SILVERLIGHT
			if (IsMouseOver) 
#endif
			if(!CanHideNavigationButtons()) {
				if(SelectedIndex > 0 && ShowPreviousButton)
					ShowButton(previousButton);
				else
					HideButton(previousButton);
				if(SelectedIndex < Items.Count - 1 && ShowNextButton)
					ShowButton(nextButton);
				else
					HideButton(nextButton);
			}
		}
		void HideButton(FadingButton button) {
			button.SetIsVisible(false);
		}
		void ShowButton(FadingButton button) {
			button.SetIsVisible(true);
		}
		#endregion
		void PrevItem() {
			if(SelectedIndex == 0) return;
			ResetTimer();
			SelectedIndex--;
			if(PreviousCommand != null && PreviousCommand.CanExecute(null))
				PreviousCommand.Execute(null);
		}
		double GetScrollViewerOffsetAfterAnimation() {
			if(this.currentAnimation != null)
				return ((DoubleAnimation)this.currentAnimation.Children[0]).To.Value;
			else
				return PartScrollViewer != null ? (Orientation == Orientation.Horizontal ? ScrollViewerBehavior.GetHorizontalOffset(PartScrollViewer) : ScrollViewerBehavior.GetVerticalOffset(PartScrollViewer)) : 0d;
		}
		void SetScrollViewerOffsetDependingOrientation(double to) {
			if(Orientation == Orientation.Horizontal)
				ScrollViewerBehavior.SetHorizontalOffset(PartScrollViewer, to);
			else
				ScrollViewerBehavior.SetVerticalOffset(PartScrollViewer, to);
		}
		void NextItem() {
			if(SelectedIndex == Items.Count - 1) return;
			ResetTimer();
			SelectedIndex++;
			if(NextCommand != null && NextCommand.CanExecute(null))
				NextCommand.Execute(null);
		}
		private bool IsValidIndex(int index) {
			return index < Items.Count && index >= 0;
		}
		int lockSelectedIndex = 0;
		bool canceled;
		protected override void OnSelectedIndexChanged(int oldValue, int newValue) {
			base.OnSelectedIndexChanged(oldValue, newValue);
			if(canceled) {
				canceled = false;
				return;
			}
			if(lockSelectedIndex == 0 && PartScrollViewer != null) {
				double to = SelectedIndex * ItemSize;
				double from = GetScrollViewerOffsetAfterAnimation();
				if(IsAnimationEnabled)
					ScrollWithAnimation(from, to, AnimationDurationPrevNextItem);
				else
					SetScrollViewerOffsetDependingOrientation(to);
			}
			UpdateKeys();
		}
		#endregion
		public bool IsAnimationEnabled {
			get { return (bool)GetValue(IsAnimationEnabledProperty); }
			set { SetValue(IsAnimationEnabledProperty, value); }
		}
		public bool IsManipulationInertiaEnabled {
			get { return (bool)GetValue(IsManipulationInertiaEnabledProperty); }
			set { SetValue(IsManipulationInertiaEnabledProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool ShowNextButton {
			get { return (bool)GetValue(ShowNextButtonProperty); }
			set { SetValue(ShowNextButtonProperty, value); }
		}
		public bool ShowPreviousButton {
			get { return (bool)GetValue(ShowPreviousButtonProperty); }
			set { SetValue(ShowPreviousButtonProperty, value); }
		}
		public ICommand NextCommand {
			get { return (ICommand)GetValue(NextCommandProperty); }
			set { SetValue(NextCommandProperty, value); }
		}
		public ICommand PreviousCommand {
			get { return (ICommand)GetValue(PreviousCommandProperty); }
			set { SetValue(PreviousCommandProperty, value); }
		}
		public ItemCacheMode ItemCacheMode {
			get { return (ItemCacheMode)GetValue(ItemCacheModeProperty); }
			set { SetValue(ItemCacheModeProperty, value); }
		}
		internal double ItemSize {
			get { return (double)GetValue(ItemSizeProperty); }
			set { SetValue(ItemSizeProperty, value); }
		}
#if SILVERLIGHT
		public bool IsManipulationEnabled {
			get { return (bool)GetValue(IsManipulationEnabledProperty); }
			set { SetValue(IsManipulationEnabledProperty, value); }
		}
#endif
		#region ISupportManipulation
		void OnManipulationCompletedCore(double velocityX, double velocityY) {
			var velocity = velocityX;
			double from = GetScrollViewerOffsetAfterAnimation();
			double to = from;
#if SILVERLIGHT
			double limit = 5;
#else
			double limit = 0.5;
#endif
			if(!manipulationCanceled && Math.Abs(velocity) > limit && !IsManipulationInertiaEnabled) {
				int index = -1;
				if(velocity > 0) {
					index = (int)(from / ItemSize);
				} else {
					index = (int)(from / ItemSize + 1);
					if(IsValidIndex(index))
						to = (index) * ItemSize;
				}
				if(IsValidIndex(index))
					to = (index) * ItemSize;
			} else {
				to = Math.Round(ItemSize * Math.Round(from / ItemSize) + 0.5);
			}
			ScrollWithAnimation(from, to, AnimationDurationManipulationCompleted);
			manipulationInProgress = false;
		}
		bool manipulationCanceled;
		bool manipulationInProgress;
		void OnManipulationDeltaCore(ManipulationDeltaEventArgs e) {
			if(manipulationCanceled || !manipulationInProgress && !RaiseSelectionChanging(SelectedIndex, SelectedIndex - Math.Sign(e.DeltaManipulation.Translation.X)))
				manipulationCanceled = true;
			manipulationInProgress = true;
			if(manipulationCanceled) return;
			double sx = 1.0 + (e.DeltaManipulation.Scale.X - 1.0) / 1.0;
			double sy = 1.0 + (e.DeltaManipulation.Scale.Y - 1.0) / 1.0;
			double prec = 0.0005;
			bool b1 = Math.Abs(sx - 1.0) <= prec;
			bool b2 = Math.Abs(sy - 1.0) <= prec;
			if(b1 && b2) {
				double offset = GetScrollViewerOffsetAfterAnimation();
				if(Orientation == Orientation.Horizontal)
					ScrollViewerBehavior.SetHorizontalOffset(PartScrollViewer, offset - e.DeltaManipulation.Translation.X);
				else
					ScrollViewerBehavior.SetVerticalOffset(PartScrollViewer, offset - e.DeltaManipulation.Translation.Y);
			}
		}
		void ISupportManipulation.OnManipulationCompleted(ManipulationCompletedEventArgs e) {
			OnManipulationCompletedCore(e.FinalVelocities.LinearVelocity.X, e.FinalVelocities.LinearVelocity.Y);
		}
		void ISupportManipulation.OnManipulationDelta(ManipulationDeltaEventArgs e) {
			OnManipulationDeltaCore(e);
		}
		protected override object OnCoerceSelectedIndex(int value) {
			if(!manipulationCanceled) {
				manipulationCanceled = false;
				return base.OnCoerceSelectedIndex(value);
			}
			manipulationCanceled = false;
			return SelectedIndex;
		}
		void ISupportManipulation.OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			e.TranslationBehavior.DesiredDeceleration = InertionDeseleration;
#if SILVERLIGHT
			e.TranslationBehavior.DesiredDeceleration = e.TranslationBehavior.DesiredDeceleration / 10.0;
#endif
		} 
		#endregion
	}
}
