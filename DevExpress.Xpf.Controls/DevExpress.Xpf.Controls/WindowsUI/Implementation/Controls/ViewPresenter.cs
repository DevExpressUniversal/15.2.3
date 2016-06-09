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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Internal {
#if SILVERLIGHT
	public class StoryboardCollection : DependencyObjectCollection<Storyboard> {
#else
	public class StoryboardCollection : FreezableCollection<Storyboard> {
#endif
		#region static
		static StoryboardCollection _Empty;
		internal static StoryboardCollection Empty {
			get {
				if(_Empty == null) {
					_Empty = new StoryboardCollection();
				}
				return _Empty;
			}
		}
		#endregion
		public StoryboardCollection() {
		}
#if !SILVERLIGHT
		public StoryboardCollection(int capacity)
			: base(capacity) {
		}
		public StoryboardCollection(IEnumerable<Storyboard> collection)
			: base(collection) {
		}
		protected override Freezable CreateInstanceCore() {
			return new StoryboardCollection();
		}
#endif
	}
	[ContentProperty("Content")]
	public class ViewPresenter : Control, IDisposable {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public static readonly DependencyProperty DefaultStoryboardProperty;
		public static readonly DependencyProperty StoryboardsProperty;
		public static readonly DependencyProperty StoryboardProperty;
		public static readonly DependencyProperty AnimationSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty OldContentProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty NewContentProperty;
		public static readonly DependencyProperty OldContentTranslateXProperty;
		public static readonly DependencyProperty NewContentTranslateXProperty;
		public static readonly DependencyProperty OldContentTranslateYProperty;
		public static readonly DependencyProperty NewContentTranslateYProperty;
		public static readonly DependencyProperty StoryboardNameProperty;
		public static readonly DependencyProperty AnimationDirectionProperty;
		public static readonly DependencyProperty AnimationTypeProperty;
		public static readonly DependencyProperty AnimationSpeedRatioProperty;
		public static readonly DependencyProperty AnimationDelayProperty;
		static ViewPresenter() {
			var dProp = new DependencyPropertyRegistrator<ViewPresenter>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Content", ref ContentProperty, (object)null,
				(d, e) => ((ViewPresenter)d).OnContentChanged(e.OldValue, e.NewValue));
			dProp.Register("ContentTemplate", ref ContentTemplateProperty, (DataTemplate)null);
			dProp.Register("ContentTemplateSelector", ref ContentTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.Register("AnimationSelector", ref AnimationSelectorProperty, (AnimationSelector)null);
			dProp.Register("AnimationType", ref AnimationTypeProperty, AnimationType.Fade);
			dProp.Register("AnimationDirection", ref AnimationDirectionProperty, AnimationDirection.Forward);
			dProp.Register("AnimationSpeedRatio", ref AnimationSpeedRatioProperty, 1d);
			dProp.Register("DefaultStoryboard", ref DefaultStoryboardProperty, (Storyboard)null
#if !SILVERLIGHT
, null, CoerceStoryboard
#endif
);
			dProp.Register("Storyboard", ref StoryboardProperty, string.Empty);
			dProp.Register("OldContent", ref OldContentProperty, (ContentPresenter)null);
			dProp.Register("NewContent", ref NewContentProperty, (ContentPresenter)null);
			dProp.Register("OldContentTranslateX", ref OldContentTranslateXProperty, 0d,
				(d, e) => ((ViewPresenter)d).OnOldContentTranslateXChanged());
			dProp.Register("NewContentTranslateX", ref NewContentTranslateXProperty, 0d,
				(d, e) => ((ViewPresenter)d).OnNewContentTranslateXChanged());
			dProp.Register("OldContentTranslateY", ref OldContentTranslateYProperty, 0d,
				(d, e) => ((ViewPresenter)d).OnOldContentTranslateYChanged());
			dProp.Register("NewContentTranslateY", ref NewContentTranslateYProperty, 0d,
				(d, e) => ((ViewPresenter)d).OnNewContentTranslateYChanged());
			dProp.Register("Storyboards", ref StoryboardsProperty, StoryboardCollection.Empty);
			dProp.RegisterAttached("StoryboardName", ref StoryboardNameProperty, (string)null);
			dProp.Register("AnimationDelay", ref AnimationDelayProperty, new TimeSpan(0, 0, 0, 0, 150));
		}
#if !SILVERLIGHT
		static object CoerceStoryboard(DependencyObject d, object source) {
			Storyboard sb = (Storyboard)source;
			return sb == null ? null : sb.Clone();
		}
#endif
		public static string GetStoryboardName(Storyboard sb) {
			return (string)sb.GetValue(StoryboardNameProperty);
		}
		public static void SetStoryboardName(Storyboard sb, string value) {
			sb.SetValue(StoryboardNameProperty, value);
		}
		#endregion
		bool animationInProgress = false;
		Storyboard storyboard;
		ContentPresenter root;
		public ViewPresenter() {
#if SILVERLIGHT
			this.DefaultStyleKey = typeof(ViewPresenter);
#endif
			SizeChanged += OnSizeChanged;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		protected virtual bool ViewIsReadyToAppear(object view) {
			return !IsContentLocked;
		}
		protected virtual void SetViewIsVisible(object view, bool value) { }
		protected virtual void RaiseBeforeViewDisappear(object view) { }
		protected virtual void RaiseAfterViewDisappear(object view) { }
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) { }
		FrameAnimation CreateFrameAnimation(object oldContent, object newContent) {
			FrameAnimation animation = new FrameAnimation() { OldSource = oldContent, NewSource = newContent, Type = AnimationType, Direction = AnimationDirection };
			return animation;
		}
		double ValidateAnimationSpeedRatio() {
			if(double.IsInfinity(AnimationSpeedRatio) || double.IsNaN(AnimationSpeedRatio) || AnimationSpeedRatio <= 0)
				return 1d;
			return AnimationSpeedRatio;
		}
		void SetStoryboard(object oldContent, object newContent) {
			FrameAnimation settings = CreateFrameAnimation(oldContent, newContent);
			AnimationSelector.Do(x => x.Select(settings));
			actualAnimationDirection = settings.Direction;
			if(settings.Storyboard != null)
				storyboard = settings.Storyboard;
			else {
				if(string.IsNullOrEmpty(Storyboard)) {
					if(Storyboards == null || Storyboards.Count == 0)
						storyboard = DefaultStoryboard;
					else {
						if(settings.Type == WindowsUI.AnimationType.Fade)
							storyboard = DefaultStoryboard;
						if(settings.Type == WindowsUI.AnimationType.SlideHorizontal)
							storyboard = (from sb in Storyboards where GetStoryboardName(sb) == (settings.Direction == WindowsUI.AnimationDirection.Forward ? "FromLeft" : "FromRight") select sb).Single();
						if(settings.Type == WindowsUI.AnimationType.SlideVertical)
							storyboard = (from sb in Storyboards where GetStoryboardName(sb) == (settings.Direction == WindowsUI.AnimationDirection.Forward ? "FromTop" : "FromBottom") select sb).Single();
					}
				} else {
					string name = Storyboard;
					storyboard = (from sb in Storyboards where GetStoryboardName(sb) == name select sb).Single();
				}
			}
			if(storyboard != null) {
#if !SILVERLIGHT
				storyboard = storyboard.Clone();
				if(!storyboard.IsPropertyAssigned(Timeline.DurationProperty)) {
					storyboard.SpeedRatio = ValidateAnimationSpeedRatio();
				}
#else
				storyboard.SpeedRatio = AnimationSpeedRatio;
#endif
			}
		}
		AnimationDirection actualAnimationDirection;
		AnimationDirection oldAnimationDirection;
		AnimationContext _Context;
		AnimationContext Context {
			get {
				if(_Context == null)
					_Context = new AnimationContext();
				return _Context;
			}
		}
		protected virtual void OnContentChanged(object oldValue, object newValue) {
			if(CurrentView == null || IsDisposing)
				return;
			if(animationInProgress) {
				Context.PostponedAnimation = true;
				Context.OldContent = oldValue;
				Context.NewContent = newValue;
				return;
			} else
				Context.PostponedAnimation = false;			
			if(PreviousView.Content == newValue) {
				return;
			}
			if(PreviousView != null && oldValue != null) {
				PreviousView.ContentTemplate = ContentTemplate;
#if !SILVERLIGHT
				PreviousView.ContentTemplateSelector = ContentTemplateSelector;
#endif
			}
			oldAnimationDirection = actualAnimationDirection;
			IsHitTestVisible = false;
			if(AnimationType != WindowsUI.AnimationType.None)
				animationInProgress = true;
			NewContent = CurrentView;
			CurrentView = PreviousView;
			PreviousView = NewContent;
			if(OldContent != null && OldContent.Content != null) {
				OldContent.IsHitTestVisible = false;
				SetViewIsVisible(OldContent.Content, false);
				RaiseBeforeViewDisappear(OldContent.Content);
			}
			PrepareNewContent(newValue);
			Canvas.SetZIndex(NewContent, 5);
			InitTransform(NewContent);
			SetStoryboard(oldValue, newValue);
			if(OldContent == null || storyboard == null) {
				NewContent.Opacity = 1.0;
				FinishContentChanging();
				storyboard = null;
				return;
			}
			ResetTransform();
			System.Windows.Media.Animation.Storyboard.SetTarget(storyboard, this);
			storyboard.Completed += OnStoryboardCompleted;
			if(ViewIsReadyToAppear(newValue))
				OnNewValueViewIsReadyToAppearChanged(newValue, null);
		}
		private void PrepareNewContent(object newValue) {
			NewContent.Content = newValue;
			NewContent.RenderTransformOrigin = new Point(0.5, 0.5);
			NewContent.Opacity = 0;
			NewContent.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding("ContentTemplate") { Source = this });
#if !SILVERLIGHT
			NewContent.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding("ContentTemplateSelector") { Source = this });
#endif
			NewContent.IsHitTestVisible = true;
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			NewContent.Opacity = 1.0;
			storyboard.Completed -= OnStoryboardCompleted;
			storyboard.Stop();
			storyboard = null;
			AnimationStarted = false;
			FinishContentChanging();
			InvokePostponed();
		}
		private void InvokePostponed() {
			Dispatcher.BeginInvoke(new Action(() => {
				if(Context.PostponedAnimation) {
					LockContentLoading();
					Context.PostponedAnimation = false;
					OnContentChanged(Context.OldContent, Context.NewContent);
					BackgroundHelper.DoInBackground(new Action(() => { }), new Action(() => {
						UnlockContentLoading();
					}), (int)AnimationDelay.TotalMilliseconds, System.Threading.ThreadPriority.Normal);					
				}
			}));
		}
		void ResetTransform() {
			if(AnimationType == AnimationType.SlideHorizontal) {
				NewContentTranslateX = actualAnimationDirection == AnimationDirection.Back ? 1 : -1;
				NewContentTranslateY = 0;
			}
			if(AnimationType == AnimationType.SlideVertical) {
				NewContentTranslateY = actualAnimationDirection == AnimationDirection.Back ? 1 : -1;
				NewContentTranslateX = 0;
			}
			OldContentTranslateX = OldContentTranslateY = 0;
		}
		bool AnimationStarted = false;
		void OnNewValueViewIsReadyToAppearChanged(object sender, EventArgs e) {
			if(Context.PostponedAnimation && AnimationStarted) {
				return;
			}
			if(storyboard != null) {
				storyboard.Begin();
				AnimationStarted = true;
			} else
				animationInProgress = false;
		}
		public event EventHandler ContentRendered;
		void RaiseContentRendered() {
			var handler = ContentRendered;
			if(!animationInProgress && handler != null)
				handler(this, EventArgs.Empty);
		}
		void FinishContentChanging() {
			var oldContent = OldContent;
			var newContent = NewContent;
			if(OldContent != null) {
				if(OldContent.Content != null)
					RaiseAfterViewDisappear(OldContent.Content);
				OldContent.Content = null;
				OldContent.ContentTemplate = null;
#if !SILVERLIGHT
				OldContent.ContentTemplateSelector = null;
#endif
			}
			OldContent = NewContent;
			NewContent = null;
			Canvas.SetZIndex(OldContent, 10);
			InitTransform(OldContent);
			if(OldContent != null && OldContent.Content != null)
				SetViewIsVisible(OldContent.Content, true);
			animationInProgress = false;
			RaiseContentRendered();
			IsHitTestVisible = true;
			var allowMerging = DevExpress.Xpf.Bars.MergingProperties.GetElementMergingBehavior(this);
			oldContent.Do((x) => DevExpress.Xpf.Bars.MergingProperties.SetElementMergingBehavior(x, Bars.ElementMergingBehavior.InternalWithInternal));
			newContent.Do((x) => DevExpress.Xpf.Bars.MergingProperties.SetElementMergingBehavior(x, allowMerging));			
		}
		void InitTransform(FrameworkElement fe) {
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform());
			transform.Children.Add(new TranslateTransform());
			fe.RenderTransform = transform;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateTransform();
		}
		private void UpdateTransform() {
			Clip = new RectangleGeometry() { Rect = new Rect(0.0, 0.0, ActualWidth, ActualHeight) };
			if(OldContent != null) {
				OnOldContentTranslateXChanged();
				OnOldContentTranslateYChanged();
			}
			if(NewContent != null) {
				OnNewContentTranslateXChanged();
				OnNewContentTranslateYChanged();
			}
		}
		void OnOldContentTranslateXChanged() {
			if(!animationInProgress)
				return;
			OldContentTranslate.X = OldContentTranslateX * ActualWidth;
		}
		void OnNewContentTranslateXChanged() {
			if(!animationInProgress || NewContentTranslate == null) {
				NewContentTranslateX = 0.0;
				return;
			}
			NewContentTranslate.X = NewContentTranslateX * ActualWidth;
		}
		void OnOldContentTranslateYChanged() {
			if(!animationInProgress)
				return;
			OldContentTranslate.Y = OldContentTranslateY * ActualHeight;
		}
		void OnNewContentTranslateYChanged() {
			if(!animationInProgress || NewContentTranslate == null) {
				NewContentTranslateY = 0.0;
				return;
			}
			NewContentTranslate.Y = NewContentTranslateY * ActualHeight;
		}
		ContentPresenter CurrentView;
		ContentPresenter PreviousView;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			root = (ContentPresenter)GetTemplateChild("Root");
			CurrentView = (ContentPresenter)GetTemplateChild("PART_CurrentView");
			PreviousView = (ContentPresenter)GetTemplateChild("PART_PreviousView");
			if(Content != null)
				OnContentChanged(null, Content);
		}
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		public AnimationDirection AnimationDirection {
			get { return (AnimationDirection)GetValue(AnimationDirectionProperty); }
			set { SetValue(AnimationDirectionProperty, value); }
		}
		public double AnimationSpeedRatio {
			get { return (double)GetValue(AnimationSpeedRatioProperty); }
			set { SetValue(AnimationSpeedRatioProperty, value); }
		}
		public AnimationType AnimationType {
			get { return (AnimationType)GetValue(AnimationTypeProperty); }
			set { SetValue(AnimationTypeProperty, value); }
		}
		public Storyboard DefaultStoryboard { get { return (Storyboard)GetValue(DefaultStoryboardProperty); } set { SetValue(DefaultStoryboardProperty, value); } }
		public string Storyboard { get { return (string)GetValue(StoryboardProperty); } set { SetValue(StoryboardProperty, value); } }
		public AnimationSelector AnimationSelector {
			get { return (AnimationSelector)GetValue(AnimationSelectorProperty); }
			set { SetValue(AnimationSelectorProperty, value); }
		}
		public ContentPresenter OldContent {
			get { return (ContentPresenter)GetValue(OldContentProperty); }
			private set { SetValue(OldContentProperty, value); }
		}
		public ContentPresenter NewContent {
			get { return (ContentPresenter)GetValue(NewContentProperty); }
			private set { SetValue(NewContentProperty, value); }
		}
		public double OldContentTranslateX {
			get { return (double)GetValue(OldContentTranslateXProperty); }
			set { SetValue(OldContentTranslateXProperty, value); }
		}
		public double NewContentTranslateX {
			get { return (double)GetValue(NewContentTranslateXProperty); }
			set { SetValue(NewContentTranslateXProperty, value); }
		}
		public double OldContentTranslateY {
			get { return (double)GetValue(OldContentTranslateYProperty); }
			set { SetValue(OldContentTranslateYProperty, value); }
		}
		public double NewContentTranslateY {
			get { return (double)GetValue(NewContentTranslateYProperty); }
			set { SetValue(NewContentTranslateYProperty, value); }
		}
		public StoryboardCollection Storyboards {
			get { return (StoryboardCollection)GetValue(StoryboardsProperty); }
			set { SetValue(StoryboardsProperty, value); }
		}
		public TimeSpan AnimationDelay {
			get { return (TimeSpan)GetValue(AnimationDelayProperty); }
			set { SetValue(AnimationDelayProperty, value); }
		}
		TranslateTransform OldContentTranslate {
			get { return ((TransformGroup)OldContent.RenderTransform).Children[1] as TranslateTransform; }
		}
		TranslateTransform NewContentTranslate {
			get {
				TransformGroup g = NewContent.RenderTransform as TransformGroup;
				if(g == null)
					return null;
				return g.Children[1] as TranslateTransform;
			}
		}
		int loadingCount = 0;
		bool IsContentLocked { get { return loadingCount > 0; } }
		internal void LockContentLoading() {
			loadingCount++;
		}
		internal void UnlockContentLoading() {
			if(IsContentLocked) {
				loadingCount--;
			}
			if(!IsContentLocked)
				OnNewValueViewIsReadyToAppearChanged(Content, EventArgs.Empty);
		}
		#region IDisposable Members
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() {
			ClearValue(ContentProperty);
			ClearValue(ContentTemplateProperty);
			ClearValue(ContentTemplateSelectorProperty);
			ClearValue(AnimationDirectionProperty);
			ClearValue(AnimationTypeProperty);
			if(CurrentView != null)
				CurrentView.Content = null;
			if(PreviousView != null)
				PreviousView.Content = null;
		}
		#endregion
		class AnimationContext {
			public bool PostponedAnimation { get; set; }
			public object OldContent { get; set; }
			public object NewContent { get; set; }
		}
	}
}
