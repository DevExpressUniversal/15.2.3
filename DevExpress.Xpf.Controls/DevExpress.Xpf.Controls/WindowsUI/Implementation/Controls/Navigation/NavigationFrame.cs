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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.Navigation;
namespace DevExpress.Xpf.WindowsUI {
	public enum AnimationDirection { Forward, Back }
	public class FrameAnimation {
		internal FrameAnimation() { }
		public Storyboard Storyboard { get; set; }
		public AnimationType Type { get; set; }
		public AnimationDirection Direction { get; set; }
		public object OldSource { get; internal set; }
		public object NewSource { get; internal set; }
	}
	public class AnimationSelector {
		protected virtual Storyboard SelectStoryboard(FrameAnimation animation) {
			return null;
		}
		protected virtual void SelectCore(FrameAnimation animation) {
			animation.Storyboard = SelectStoryboard(animation);
		}
		public void Select(FrameAnimation animation) {
			SelectCore(animation);
		}
	}
	public enum BackNavigationMode { PreviousScreen, Root }
	public enum NavigationCacheMode {
		Disabled,
		Enabled,
		Required
	}
	public interface INavigationContainer {
		void Navigate(object source, object param);
		void GoBack();
		void GoBack(object param);
		bool CanGoBack { get; }
		void GoForward();
		bool CanGoForward { get; }
	}
	public enum NavigationStatus { NotStarted, Executing, Completed, Aborted, Failed }
	public interface ISupportAsyncNavigation {
		NavigationStatus Status { get; }
	}
	public interface INavigationFrame : INavigationContainer, ISupportAsyncNavigation {
		IJournal Journal { get; }
		bool Navigating(object source, NavigationMode mode, object navigationState);
		void NavigationComplete(object source, object content, object navigationState);
		void NavigationFailed(object source, NavigationException e);
		void NavigationStopped(object source, NavigationMode mode, object navigationState);
		NavigationCacheMode NavigationCacheMode { get; }
	}
#if !SILVERLIGHT
#endif
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	[ContentProperty("Source")]
	public class NavigationFrame : ContentControl, INavigationFrame {
		#region static
		public static readonly DependencyProperty CanGoForwardProperty;
		static readonly DependencyPropertyKey CanGoForwardPropertyKey;
		public static readonly DependencyProperty CanGoBackProperty;
		static readonly DependencyPropertyKey CanGoBackPropertyKey;
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty JournalProperty;
		public static readonly DependencyProperty AnimationTypeProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty AnimationDirectionProperty;
		public static readonly DependencyProperty AnimationSpeedRatioProperty;
		public static readonly DependencyProperty AnimationSelectorProperty;
		public static readonly DependencyProperty AnimationDelayProperty;
		static readonly DependencyPropertyKey AnimationDirectionPropertyKey;
		public static readonly DependencyProperty ContentProviderProperty;
		public static readonly DependencyProperty BackNavigationModeProperty;
		public static readonly DependencyProperty NavigationCacheMaxSizeProperty;
		public static readonly DependencyProperty NavigationCacheModeProperty;
		public static readonly DependencyProperty AllowMergingProperty;
		public static readonly DependencyProperty PrefetchedSourcesProperty;
		static NavigationFrame() {
			var dProp = new DependencyPropertyRegistrator<NavigationFrame>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Source", ref SourceProperty, (object)null, OnSourceChanged);
			dProp.Register("Journal", ref JournalProperty, (IJournal)null, OnJournalChanged);
			dProp.RegisterReadonly("CanGoBack", ref CanGoBackPropertyKey, ref CanGoBackProperty, false);
			dProp.RegisterReadonly("CanGoForward", ref CanGoForwardPropertyKey, ref CanGoForwardProperty, false);
#if DEBUGTEST
			AnimationType defaultAnimationType = AnimationType.None;
#else
			AnimationType defaultAnimationType = AnimationType.Fade;
#endif
			dProp.Register("AnimationType", ref AnimationTypeProperty, defaultAnimationType);
			dProp.RegisterReadonly("AnimationDirection", ref AnimationDirectionPropertyKey, ref AnimationDirectionProperty, AnimationDirection.Forward);
			dProp.Register("AnimationSpeedRatio", ref AnimationSpeedRatioProperty, 1d);
			dProp.Register("AnimationSelector", ref AnimationSelectorProperty, (AnimationSelector)null);
			dProp.Register("AnimationDelay", ref AnimationDelayProperty, new TimeSpan(0, 0, 0, 0, 150));
			dProp.Register("ContentProvider", ref ContentProviderProperty, (INavigationContentProvider)null, OnContentProviderChanged);
			dProp.Register("BackNavigationMode", ref BackNavigationModeProperty, BackNavigationMode.PreviousScreen);
			dProp.Register("NavigationCacheMaxSize", ref NavigationCacheMaxSizeProperty, 10);
			dProp.Register("NavigationCacheMode", ref NavigationCacheModeProperty, NavigationCacheMode.Disabled);
			AllowMergingProperty = DependencyProperty.Register("AllowMerging", typeof(bool?), typeof(NavigationFrame), new FrameworkPropertyMetadata((bool?)null, OnAllowMergingChanged));
			PrefetchedSourcesProperty = DependencyProperty.Register("PrefetchedSources", typeof(IEnumerable), typeof(NavigationFrame), new FrameworkPropertyMetadata(null, OnPrefetchedSourcesChanged));
		}
		static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationFrame frameEx = o as NavigationFrame;
			if(frameEx != null)
				frameEx.OnSourceChanged((object)e.OldValue, (object)e.NewValue);
		}
		static void OnContentProviderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationFrame frameEx = o as NavigationFrame;
			if(frameEx != null)
				frameEx.OnContentProviderChanged((INavigationContentProvider)e.OldValue, (INavigationContentProvider)e.NewValue);
		}
		static void OnJournalChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationFrame frame = o as NavigationFrame;
			frame.OnJournalChanged(e.OldValue as IJournal, e.NewValue as IJournal);
		}
		static void OnAllowMergingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationFrame frameEx = o as NavigationFrame;
			if(frameEx != null)
				frameEx.OnAllowMergingChanged();
		}
		static void OnPrefetchedSourcesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationFrame frame = o as NavigationFrame;						
			frame.OnPrefetchedSourcesChanged((IEnumerable)e.NewValue);
		}
		#endregion
		public NavigationFrame() {
			MergingProperties.SetElementMergingBehavior(this, ElementMergingBehavior.InternalWithExternal);
#if SILVERLIGHT
			this.DefaultStyleKey = typeof(NavigationFrame);
#endif
			Journal = new Journal(ContentProvider) { Navigator = this };
		}
		public void ClearCache() {
			ActualJournal.ClearNavigationCache();
		}
		ViewPresenter PartViewPresenter;
		public override void OnApplyTemplate() {
			if(PartViewPresenter != null && !LayoutTreeHelper.IsTemplateChild(PartViewPresenter, this)) {
				PartViewPresenter.ContentRendered -= PartViewPresenter_AnimationCompleted;
				PartViewPresenter.Dispose();
			}
			base.OnApplyTemplate();
			PartViewPresenter = LayoutTreeHelper.GetTemplateChild<ViewPresenter, NavigationFrame>(this);
			if(PartViewPresenter != null) PartViewPresenter.ContentRendered += PartViewPresenter_AnimationCompleted;
		}
		public event EventHandler ContentRendered;
		void PartViewPresenter_AnimationCompleted(object sender, EventArgs e) {
			if(ContentRendered != null) ContentRendered(this, e);
		}
		protected override void OnContentChanged(object oldContent, object newContent) { }
		protected virtual void OnContentProviderChanged(INavigationContentProvider oldValue, INavigationContentProvider newValue) {
			AssertionException.IsNotNull(Journal);
			Journal.NavigationContentProvider = newValue;
		}
		protected virtual void OnSourceChanged(object oldValue, object newValue) {
			Navigate(newValue);
		}
		void OnJournalChanged(IJournal oldJournal, IJournal newJournal) {
			if(oldJournal != null) oldJournal.PropertyChanged -= (o, e) => UpdateJournalProperties();
			if(newJournal != null) newJournal.PropertyChanged += (o, e) => UpdateJournalProperties();
		}		
		Action AfterNavigateOnPrefetchedSource;
		void OnPrefetchedSourcesChanged(IEnumerable sources) {
			NavigateEnumerable(sources);
		}
		internal void NavigateEnumerable(IEnumerable sources) {
			List<object> objects = sources.Cast<object>().ToList();
			if(objects.Count == 0)
				return;
			ActualJournal.ClearNavigationCache();
			AfterNavigateOnPrefetchedSource = () => {
				ActualJournal.ClearNavigationHistory();
				for(int i = objects.Count - 1; i > 0; i--)
					ActualJournal.PushInForwardStack(objects[i]);				
			};
			Navigate(NavigationMode.New, objects[0], null);
		}
		internal void UpdateJournalProperties() {
			this.CanGoBack = ActualJournal == null ? false : ActualJournal.CanGoBack;
			this.CanGoForward = ActualJournal == null ? false : ActualJournal.CanGoForward;
		}
		public void GoBack(object param = null) {
			NavigationStatus = WindowsUI.NavigationStatus.Executing;
			Navigate(NavigationMode.Back, null, param);
		}
		public void GoForward() {
			NavigationStatus = WindowsUI.NavigationStatus.Executing;
			Navigate(NavigationMode.Forward, null, null);
		}
		public void Navigate(object source) {
			Navigate(source, null);
		}
		public void Navigate(object source, object param) {
			NavigationStatus = WindowsUI.NavigationStatus.Executing;
			Dispatcher.BeginInvoke(new Action(() => {
				Navigate(NavigationMode.New, source, param);
			}));
		}
		void Navigate(NavigationMode mode, object source, object param) {
			switch(mode) {
				case NavigationMode.New:
					AssertionException.IsNotNull(ActualJournal);
					AssertionException.IsFalse(NavigationInProgress);
					if(!NavigationInProgress) {
						NavigationInProgress = true;
						ActualJournal.Navigate(source, param);
					}
					break;
				case NavigationMode.Back:
					AssertionException.IsNotNull(ActualJournal);
					if(ActualJournal.CanGoBack) {
						if(BackNavigationMode == WindowsUI.BackNavigationMode.PreviousScreen)
							ActualJournal.GoBack(param);
						else
							ActualJournal.GoHome(param);
					}
					break;
				case NavigationMode.Forward:
					AssertionException.IsNotNull(ActualJournal);
					if(ActualJournal.CanGoForward) ActualJournal.GoForward();
					break;
			}
		}
		protected virtual void OnAllowMergingChanged() {
			var allowMerging = AllowMerging.Return(x => x.Value, () => false);
			MergingProperties.SetElementMergingBehavior(this, AllowMerging.Return(x => x.Value ? ElementMergingBehavior.InternalWithExternal : ElementMergingBehavior.InternalWithInternal, () => ElementMergingBehavior.Undefined));
			MergingProperties.SetHideElements(this, allowMerging);
		}
		public AnimationType AnimationType {
			get { return (AnimationType)GetValue(AnimationTypeProperty); }
			set { SetValue(AnimationTypeProperty, value); }
		}
		internal AnimationDirection AnimationDirection {
			get { return (AnimationDirection)GetValue(AnimationDirectionProperty); }
			private set { this.SetValue(AnimationDirectionPropertyKey, value); }
		}
		public double AnimationSpeedRatio {
			get { return (double)GetValue(AnimationSpeedRatioProperty); }
			set { SetValue(AnimationSpeedRatioProperty, value); }
		}
		public AnimationSelector AnimationSelector {
			get { return (AnimationSelector)GetValue(AnimationSelectorProperty); }
			set { SetValue(AnimationSelectorProperty, value); }
		}
		public TimeSpan AnimationDelay {
			get { return (TimeSpan)GetValue(AnimationDelayProperty); }
			set { SetValue(AnimationDelayProperty, value); }
		}
		public INavigationContentProvider ContentProvider {
			get { return (INavigationContentProvider)GetValue(ContentProviderProperty); }
			set { SetValue(ContentProviderProperty, value); }
		}
		public object Source {
			get { return (object)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public IJournal Journal {
			get { return (IJournal)GetValue(JournalProperty); }
			set { SetValue(JournalProperty, value); }
		}
		public bool CanGoBack {
			get { return (bool)this.GetValue(CanGoBackProperty); }
			protected set { this.SetValue(CanGoBackPropertyKey, value); }
		}
		public bool CanGoForward {
			get { return (bool)this.GetValue(CanGoForwardProperty); }
			protected set { this.SetValue(CanGoForwardPropertyKey, value); }
		}
		public BackNavigationMode BackNavigationMode {
			get { return (BackNavigationMode)GetValue(BackNavigationModeProperty); }
			set { SetValue(BackNavigationModeProperty, value); }
		}
		public NavigationCacheMode NavigationCacheMode {
			get { return (NavigationCacheMode)GetValue(NavigationCacheModeProperty); }
			set { SetValue(NavigationCacheModeProperty, value); }
		}
		public int NavigationCacheMaxSize {
			get { return (int)GetValue(NavigationCacheMaxSizeProperty); }
			set { SetValue(NavigationCacheMaxSizeProperty, value); }
		}
		public bool? AllowMerging {
			get { return (bool?)GetValue(AllowMergingProperty); }
			set { SetValue(AllowMergingProperty, value); }
		}
		public IEnumerable PrefetchedSources {
			get { return (IEnumerable)GetValue(PrefetchedSourcesProperty); }
			set { SetValue(PrefetchedSourcesProperty, value); }
		}
		private IJournal _AttachedJournal;
		internal IJournal AttachedJournal {
			get { return _AttachedJournal; }
			set {
				if(_AttachedJournal != null)
					_AttachedJournal.PropertyChanged -= (o, e) => UpdateJournalProperties();
				_AttachedJournal = value;
				if(_AttachedJournal != null)
					_AttachedJournal.PropertyChanged += (o, e) => UpdateJournalProperties();
			}
		}
		IJournal ActualJournal { get { return AttachedJournal ?? Journal; } }
		public bool NavigationInProgress { get; private set; }
		public event NavigatingCancelEventHandler Navigating;
		public event NavigatedEventHandler Navigated;
		public event NavigationFailedEventHandler NavigationFailed;
		#region INavigationFrame Members
		IJournal INavigationFrame.Journal {
			get { return ActualJournal; }
		}
		bool INavigationFrame.Navigating(object source, NavigationMode mode, object navigationState) {
			if(mode == NavigationMode.Back)
				AnimationDirection = AnimationDirection.Forward;
			else
				AnimationDirection = AnimationDirection.Back;
			NavigatingEventArgs args = new NavigatingEventArgs(source, mode, navigationState);
			if(Navigating != null) {
				Navigating(this, args);
			}
			if(!args.Cancel) {
				AssertionException.IsNotNull(ActualJournal);
				if(ActualJournal.Current != null) {
					var currentContent = ActualJournal.Current.Content;
					INavigationAware navigationAware = currentContent as INavigationAware;
					FrameworkElement frameworkElement = currentContent as FrameworkElement;
					if(navigationAware != null) {
						navigationAware.NavigatingFrom(args);
					}
					if(frameworkElement != null) {
						INavigationAware na = frameworkElement.DataContext as INavigationAware;
						if(na != null && na != frameworkElement)
							na.NavigatingFrom(args);
					}
				}
			}
			if(!args.Cancel) LockContent();
			return !args.Cancel;
		}
		void INavigationFrame.NavigationComplete(object source, object content, object navigationState) {
			object oldContent = Content;
			Content = content;
#if !SILVERLIGHT
			if(!(content is UIElement) && this.ContentTemplateSelector != null) {
				this.ContentTemplate = ContentTemplateSelector.SelectTemplate(source, content as DependencyObject);
			}
#endif
			NotifyNavigationAware(source, content, navigationState, oldContent);
			NavigationInProgress = false;
			OnNavigationCompleted(NavigationStatus.Completed);
		}
		void OnNavigationCompleted(NavigationStatus navigationStatus) {
#if !SL
			BackgroundHelper.DoInBackground(
				new Action(() => { }),
				new Action(() => { UnlockContent(); }),
				(int)AnimationDelay.TotalMilliseconds,
				System.Threading.ThreadPriority.Normal);
			if(AfterNavigateOnPrefetchedSource != null && navigationStatus == NavigationStatus.Completed) {
				AfterNavigateOnPrefetchedSource();
			}
			AfterNavigateOnPrefetchedSource = null;
#else
			BackgroundHelper.DoInBackground(new Action(() => { }), new Action(() => {
				UnlockContent();
			}), (int)AnimationDelay.TotalMilliseconds);
#endif
			NavigationStatus = navigationStatus;
		}
		void NotifyNavigationAware(object source, object content, object navigationState, object oldContent) {
			NavigationEventArgs args = new NavigationEventArgs(source, content, navigationState);
			if(Navigated != null) {
				Navigated(this, args);
			}
			INavigationAware oldNavigationAware = oldContent as INavigationAware;
			FrameworkElement oldFrameworkElement = oldContent as FrameworkElement;
			if(oldNavigationAware != null) {
				oldNavigationAware.NavigatedFrom(args);
			}
			if(oldFrameworkElement != null) {
				INavigationAware na = oldFrameworkElement.DataContext as INavigationAware;
				if(na != null && na != oldFrameworkElement) na.NavigatedFrom(args);
			}
			AssertionException.IsNotNull(ActualJournal);
			if(ActualJournal.Current != null) {
				var currentContent = ActualJournal.Current.Content;
				INavigationAware navigationAware = currentContent as INavigationAware;
				FrameworkElement frameworkElement = currentContent as FrameworkElement;
				if(navigationAware != null) {
					navigationAware.NavigatedTo(args);
				}
				if(frameworkElement != null) {
					INavigationAware na = frameworkElement.DataContext as INavigationAware;
					if(na != null && na != frameworkElement) na.NavigatedTo(args);
				}
			}
		}
		void INavigationFrame.NavigationFailed(object source, NavigationException ex) {
			if(NavigationFailed != null) {
				NavigationFailedEventArgs args = new NavigationFailedEventArgs(source, ex);
				NavigationFailed(this, args);
			}
			NavigationInProgress = false;
			OnNavigationCompleted(NavigationStatus.Failed);
		}
		void INavigationFrame.NavigationStopped(object source, NavigationMode mode, object navigationState) {
			NavigationInProgress = false;
			OnNavigationCompleted(NavigationStatus.Aborted);
		}
		#endregion
		#region INavigationContainer Members
		internal void LockContent() {
			if(PartViewPresenter != null) PartViewPresenter.LockContentLoading();
		}
		internal void UnlockContent() {
			if(PartViewPresenter != null) PartViewPresenter.UnlockContentLoading();
		}
		void INavigationContainer.Navigate(object source, object param) {
			Navigate(source, param);
		}
		void INavigationContainer.GoBack() {
			GoBack();
		}
		void INavigationContainer.GoBack(object param) {
			GoBack(param);
		}
		bool INavigationContainer.CanGoBack {
			get { return CanGoBack; }
		}
		void INavigationContainer.GoForward() {
			GoForward();
		}
		bool INavigationContainer.CanGoForward {
			get { return CanGoForward; }
		}
		#endregion
		NavigationStatus NavigationStatus;
		#region ISupportAsyncNavigation Members
		NavigationStatus ISupportAsyncNavigation.Status {
			get {
				return NavigationStatus;
			}
		}
		#endregion
	}
}
