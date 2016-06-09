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

using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class PageViewContentPanel : Panel {
		#region static
		public static readonly DependencyProperty ItemCacheModeProperty;
		public static readonly DependencyProperty AnimationTypeProperty;
		public static readonly DependencyProperty AnimationDirectionProperty;
		public static readonly DependencyProperty AnimationSpeedRatioProperty;
		static PageViewContentPanel() {
			var dProp = new DependencyPropertyRegistrator<PageViewContentPanel>();
			dProp.Register("ItemCacheMode", ref ItemCacheModeProperty, ItemCacheMode.None,
				(d, e) => ((PageViewContentPanel)d).OnItemCacheModeChanged((ItemCacheMode)e.OldValue, (ItemCacheMode)e.NewValue));
			dProp.Register("AnimationType", ref AnimationTypeProperty, AnimationType.None);
			dProp.Register("AnimationDirection", ref AnimationDirectionProperty, SelectorAnimationDirection.Forward);
			dProp.Register("AnimationSpeedRatio", ref AnimationSpeedRatioProperty, 0d);
		}
		#endregion
		public ItemCacheMode ItemCacheMode {
			get { return (ItemCacheMode)GetValue(ItemCacheModeProperty); }
			set { SetValue(ItemCacheModeProperty, value); }
		}
		public SelectorAnimationDirection AnimationDirection {
			get { return (SelectorAnimationDirection)GetValue(AnimationDirectionProperty); }
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
		public PageView Owner { get; private set; }
		AnimationProvider animationProvider;
		protected internal ContentPresenter SelectedItem { get; set; }
		protected internal Dictionary<object, ContentPresenter> Items { get; set; }
		public PageViewContentPanel() {
			Items = new Dictionary<object, ContentPresenter>();
			animationProvider = new AnimationProvider(this) { AllowInitialAnimation = false };
			SizeChanged += OnSizeChanged;
		}
		protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			Clip = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));
		}
		public void Initialize(PageView owner) {
			if(Owner == owner) return;
			if(owner == null) {
				UnInitialize();
				return;
			}
			Owner = owner;
			Owner.ItemsChanged += OnOwnerItemsChanged;
			Owner.SelectionChanged += OnOwnerSelectionChanged;
			Update();
		}
		public void UnInitialize() {
			if(Owner == null) return;
			Owner.ItemsChanged -= OnOwnerItemsChanged;
			Owner.SelectionChanged -= OnOwnerSelectionChanged;
			Clear();
			Owner = null;
			SelectedItem = null;
		}
		void Update() {
			UpdateItems();
			UpdateSelection();
		}
		protected virtual void OnItemCacheModeChanged(ItemCacheMode oldValue, ItemCacheMode newValue) {
			if(Owner != null) {
				if(newValue == WindowsUI.ItemCacheMode.None) Clear();
				Update();
			}
		}
		protected virtual void OnOwnerItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Add)
				OnOwnerItemsAdd(e);
			else if(e.Action == NotifyCollectionChangedAction.Remove)
				OnOwnerItemsRemove(e);
			else if(e.Action == NotifyCollectionChangedAction.Replace)
				OnOwnerItemsReplace(e);
			else if(e.Action == NotifyCollectionChangedAction.Reset) {
				Clear();
			}
			Update();
		}
		void OnOwnerItemsAdd(NotifyCollectionChangedEventArgs e) {
			if(e.NewItems == null || ItemCacheMode != ItemCacheMode.CacheAll) return;
			for(int i = 0; i < e.NewItems.Count; i++) {
				object item = e.NewItems[i];
				if(item == null || Items.ContainsKey(item)) continue;
				InsertItem(item, e.NewStartingIndex + i);
			}
		}
		void OnOwnerItemsRemove(NotifyCollectionChangedEventArgs e) {
			if(e.OldItems == null) return;
			for(int i = 0; i < e.OldItems.Count; i++) {
				object item = e.OldItems[i];
				if(item == null) continue;
				RemoveItem(item);
			}
		}
		protected virtual PageViewItem GetContainer(object content) {
			return content as PageViewItem ?? Owner.GetContainer(content);
		}
		void OnOwnerItemsReplace(NotifyCollectionChangedEventArgs e) {
			if(e.NewItems == null || e.OldItems == null) return;
			object oldTabItem = e.OldItems[0];
			object newTabItem = e.NewItems[0];
			if(oldTabItem == null || newTabItem == null || !Items.ContainsKey(oldTabItem)) return;
			RemoveItem(oldTabItem);
			InsertItem(newTabItem, e.NewStartingIndex);
		}
		bool fRequestClear = false;
		Locker animationLocker = new Locker();
		protected virtual void OnOwnerSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(e.OriginalSource != Owner) return;
			if(Owner.Items.Count != Items.Count) {
				if(ItemCacheMode == WindowsUI.ItemCacheMode.None) {
					fRequestClear = AnimationType != WindowsUI.AnimationType.None;
					if(!fRequestClear) Clear();
				}
				UpdateItems();
			}
			animationLocker.Lock();
			UpdateSelection();
			animationLocker.Unlock();
			InvalidateMeasure();
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(SelectedItem == null)
				return base.MeasureOverride(availableSize);
			foreach(UIElement child in InternalChildren) {
				child.Measure(availableSize);
			}
			return SelectedItem.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(SelectedItem != null) {
				SelectedItem.Arrange(new Rect(new Point(), finalSize));
				return new Size(SelectedItem.ActualWidth, SelectedItem.ActualHeight);
			}
			return base.ArrangeOverride(finalSize);
		}
		protected virtual void UpdateItems() {
			if(ItemCacheMode == ItemCacheMode.CacheAll)
				SynchronizeAllTabs();
			else
				SynchronizeSelectedTab();
		}
		void SynchronizeSelectedTab() {
			var selectedTab = Owner.IsIndexInRange(Owner.SelectedIndex) ? Owner.Items[Owner.SelectedIndex] : null;
			if(selectedTab != null && !Items.ContainsKey(selectedTab))
				AddItem(selectedTab);
		}
		void SynchronizeAllTabs() {
			allowUpdateSelection = false;
			for(int i = 0; i < Owner.Items.Count; i++) {
				object tab = Owner.Items[i];
				if(tab == null || Items.ContainsKey(tab)) continue;
				AddItem(tab);
			}
			allowUpdateSelection = true;
		}
		bool allowUpdateSelection = true;
		protected internal virtual void UpdateSelection() {
			if(Owner == null || !allowUpdateSelection)
				return;
			object item = Owner.IsIndexInRange(Owner.SelectedIndex) ? Owner.Items[Owner.SelectedIndex] : null;
			var newSelectedItem = item != null && Items.ContainsKey(item) ? Items[item] : null;
			HideItemsExcept(SelectedItem == null || AnimationType == AnimationType.None ? null : SelectedItem, newSelectedItem);
			if(SelectedItem != newSelectedItem )
				animationProvider.Animate(SelectedItem, newSelectedItem, AnimationType, AnimationDirection, AnimationSpeedRatio, OnAnimationComplete);
			SelectedItem = newSelectedItem;
		}
		void OnAnimationComplete() {
			if(fRequestClear) {
				Clear(SelectedItem);
				fRequestClear = false;
			}
		}
		protected virtual void Clear(object selectedItem) {
			var keysToRemove = Items.Where(x => x.Value != selectedItem).ToDictionary(x => x.Key, x => x.Value).Keys.ToList();
			foreach(var key in keysToRemove) {
				Children.Remove(Items[key]);
				Items.Remove(key);
			}
		}
		void HideItemsExcept(params ContentPresenter[] presenters) {
			foreach(UIElement child in Children) {
				child.Visibility = presenters.Contains(child) ? Visibility.Visible : Visibility.Hidden;
			}
		}
		protected virtual void CollapseAllItems(UIElement selectedElement) {
			foreach(UIElement item in Children)
				item.Visibility = item == selectedElement ? Visibility.Visible : Visibility.Hidden;
		}
		protected virtual void AddItem(object item) {
			InsertItem(item, Children.Count);
		}
		protected virtual void InsertItem(object item, int index) {
			var container = GetContainer(item);
			if(container == null) return;
			var cp = new FastContentPresenter() { Visibility = Visibility.Collapsed };
			BindItem(cp, container);
			Children.Add(cp);
			Items.Add(item, cp);
		}
		protected virtual void RemoveItem(object item) {
			if(item == null || !Items.ContainsKey(item)) return;
			Children.Remove(Items[item]);
			Items.Remove(item);
		}
		protected virtual void Clear() {
			foreach(ContentPresenter cp in Children)
				ClearBind(cp);
			Items.Clear();
			Children.Clear();
			SelectedItem = null;
		}
		protected virtual void BindItem(ContentPresenter cp, PageViewItem pageViewItem) {
			cp.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") { Source = pageViewItem });
			cp.SetBinding(ContentPresenter.ContentTemplateProperty, new MultiBinding { Bindings = { new Binding("ContentTemplate") { Source = pageViewItem }, new Binding("ContentTemplate") { Source = Owner } }, Converter = new СontentTemplateMultiBindingConverter() });
			cp.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new MultiBinding { Bindings = { new Binding("ContentTemplateSelector") { Source = pageViewItem }, new Binding("ContentTemplateSelector") { Source = Owner } }, Converter = new СontentTemplateMultiBindingConverter() });
		}
		protected virtual void ClearBind(ContentPresenter cp) {
			BindingOperations.ClearAllBindings(cp);
			cp.Content = null;
			cp.ContentTemplate = null;
		}
		class FastContentPresenter : ContentPresenter { }
	}
	class СontentTemplateMultiBindingConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return values[0] ?? values[1];
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
	public class AnimationProvider : DependencyObject {
		#region static
		public static readonly DependencyProperty OldContentTranslateXProperty;
		public static readonly DependencyProperty NewContentTranslateXProperty;
		public static readonly DependencyProperty OldContentTranslateYProperty;
		public static readonly DependencyProperty NewContentTranslateYProperty;
		public static readonly DependencyProperty OldContentOpacityProperty;
		public static readonly DependencyProperty NewContentOpacityProperty;
		public static readonly DependencyProperty StoryboardNameProperty;
		static AnimationProvider() {
			var dProp = new DependencyPropertyRegistrator<AnimationProvider>();
			dProp.Register("OldContentTranslateX", ref OldContentTranslateXProperty, 0d,
				(d, e) => ((AnimationProvider)d).OnOldContentTranslateXChanged());
			dProp.Register("NewContentTranslateX", ref NewContentTranslateXProperty, 0d,
				(d, e) => ((AnimationProvider)d).OnNewContentTranslateXChanged());
			dProp.Register("OldContentTranslateY", ref OldContentTranslateYProperty, 0d,
				(d, e) => ((AnimationProvider)d).OnOldContentTranslateYChanged());
			dProp.Register("NewContentTranslateY", ref NewContentTranslateYProperty, 0d,
				(d, e) => ((AnimationProvider)d).OnNewContentTranslateYChanged());
			dProp.Register("OldContentOpacity", ref OldContentOpacityProperty, 1d);
			dProp.Register("NewContentOpacity", ref NewContentOpacityProperty, 1d);
			dProp.RegisterAttached<string>("StoryboardName", ref StoryboardNameProperty);
		}
		public static string GetStoryboardName(DependencyObject obj) {
			return (string)obj.GetValue(StoryboardNameProperty);
		}
		public static void SetStoryboardName(DependencyObject obj, string value) {
			obj.SetValue(StoryboardNameProperty, value);
		}
		#endregion
		StoryboardCollection Storyboards;
		Storyboard DefaultStoryboard;
		Storyboard CurrentStoryboard;
		FrameworkElement owner;
		ContentPresenter oldContent;
		ContentPresenter newContent;
		AnimationType animationType;
		SelectorAnimationDirection oldAnimationDirection;
		SelectorAnimationDirection animationDirection;
		double animationSpeedRatio;
		bool animationInProgress;
		public AnimationProvider(FrameworkElement owner) {
			this.owner = owner;
			AssertionException.IsNotNull(owner, "Parameter cannot be null");
			owner.SizeChanged += OnSizeChanged;
			var resources = new ResourceDictionary() { Source = AssemblyHelper.GetResourceUri(typeof(AnimationProvider).Assembly, "WindowsUI/Themes/Generic/ViewPresenter.xaml") };
			DefaultStoryboard = resources["defaultStoryboard"] as Storyboard;
			Storyboards = resources["storyboards"] as StoryboardCollection;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateTransform();
		}
		Action OnCompleteAction;
		internal bool AllowInitialAnimation { get; set; }
		public void Animate(ContentPresenter oldContentPresenter, ContentPresenter newContentPresenter, AnimationType animationType, SelectorAnimationDirection animationDirection, double animationSpeedRatio, Action onComplete) {
			if(animationType == AnimationType.None) {
				return;
			}
			oldContent = oldContentPresenter;
			newContent = newContentPresenter;
			this.animationType = animationType;
			this.animationDirection = animationDirection;
			this.animationSpeedRatio = animationSpeedRatio;
			if(animationInProgress && animationType != AnimationType.Fade) {
				if(oldAnimationDirection == animationDirection) {
					CurrentStoryboard.Pause();
					UpdateTransform();
					CurrentStoryboard.Resume();
				}
				else {
					ReverseAnimation();
				}
				oldAnimationDirection = animationDirection;
				return;
			}
			oldAnimationDirection = animationDirection;
			InitTransform();
			ResetTransform();
			SetBindings();
			SetCurrentStoryboard();
			animationInProgress = true;
			if(oldContent != null || AllowInitialAnimation) {
				OnCompleteAction = onComplete;
				CurrentStoryboard.Begin();
			}
			else OnAnimatationCompleted();
		}
		void OnAnimatationCompleted() {
			animationInProgress = false;
			if(CurrentStoryboard != null)
				CurrentStoryboard.Completed -= CurrentStoryboardCompleted;
			ClearCurrentStoryboard();
			ClearBindings();
			if(oldContent != null)
				oldContent.Visibility = Visibility.Collapsed;
			newContent = null;
			if(OnCompleteAction != null) OnCompleteAction();
			OnCompleteAction = null;
		}
		void CurrentStoryboardCompleted(object sender, EventArgs e) {
			OnAnimatationCompleted();
		}
		private void ReverseAnimation() {
			var oldX = OldContentTranslateX;
			var newX = NewContentTranslateX;
			var oldY = OldContentTranslateY;
			var newY = NewContentTranslateY;
			CurrentStoryboard.Stop();
			owner.BeginAnimation(OldContentTranslateXProperty, null);
			owner.BeginAnimation(NewContentTranslateXProperty, null);
			owner.BeginAnimation(OldContentTranslateYProperty, null);
			owner.BeginAnimation(NewContentTranslateYProperty, null);
			OldContentTranslateX = newX;
			NewContentTranslateX = oldX;
			OldContentTranslateY = newY;
			NewContentTranslateY = oldY;
			UpdateTransform();
			SetCurrentStoryboard();
			CurrentStoryboard.Begin();
		}
		private void ClearCurrentStoryboard() {
			if(CurrentStoryboard != null)
				CurrentStoryboard.Stop();
			owner.BeginAnimation(NewContentTranslateXProperty, null);
			owner.BeginAnimation(OldContentTranslateXProperty, null);
			NewContentTranslateX = 0;
			NewContentTranslateY = 0;
			UpdateTransform();
		}
		private void SetBindings() {
			if (oldContent != null)
				oldContent.SetBinding(ContentPresenter.OpacityProperty, new Binding("OldContentOpacity") { Source = this });
			if (newContent != null)
				newContent.SetBinding(ContentPresenter.OpacityProperty, new Binding("NewContentOpacity") { Source = this });
		}
		private void ClearBindings() {
			if(oldContent != null)
				oldContent.ClearValue(ContentPresenter.OpacityProperty);
			if(newContent != null)
				newContent.ClearValue(ContentPresenter.OpacityProperty);
		}
		void InitTransform() {
			if(oldContent != null && !(oldContent.RenderTransform is TranslateTransform))
				oldContent.RenderTransform = new TranslateTransform();
			if(newContent != null && !(newContent.RenderTransform is TranslateTransform))
				newContent.RenderTransform = new TranslateTransform();
		}
		void ResetTransform() {
			if(animationType == AnimationType.SlideHorizontal) {
				NewContentTranslateX = animationDirection == SelectorAnimationDirection.Back ? 1 : -1;
				NewContentTranslateY = 0;
			}
			if(animationType == AnimationType.SlideVertical) {
				NewContentTranslateY = animationDirection == SelectorAnimationDirection.Back ? 1 : -1;
				NewContentTranslateX = 0;
			}
			OldContentTranslateX = OldContentTranslateY = 0;
		}
		private void UpdateTransform() {
			InitTransform();
			OnOldContentTranslateXChanged();
			OnOldContentTranslateYChanged();
			OnNewContentTranslateXChanged();
			OnNewContentTranslateYChanged();
		}
		void OnOldContentTranslateXChanged() {
			if(oldContent != null && oldContent.RenderTransform is TranslateTransform)
				((TranslateTransform)oldContent.RenderTransform).X = OldContentTranslateX * owner.ActualWidth;
		}																				   
		void OnNewContentTranslateXChanged() {
			if(newContent != null && newContent.RenderTransform is TranslateTransform) {
				((TranslateTransform)newContent.RenderTransform).X = NewContentTranslateX * owner.ActualWidth;
			}																			   
		}																				   
		void OnOldContentTranslateYChanged() {											  
			if(oldContent != null && oldContent.RenderTransform is TranslateTransform)	  
				((TranslateTransform)oldContent.RenderTransform).Y = OldContentTranslateY * owner.ActualHeight;
		}																				   
		void OnNewContentTranslateYChanged() {											  
			if(newContent != null && newContent.RenderTransform is TranslateTransform)	  
				((TranslateTransform)newContent.RenderTransform).Y = NewContentTranslateY * owner.ActualHeight;
		}
		double ValidateAnimationSpeedRatio(double animationSpeedRatio) {
			if(double.IsInfinity(animationSpeedRatio) || double.IsNaN(animationSpeedRatio) || animationSpeedRatio <= 0) return 1d;
			return animationSpeedRatio;
		}
		void SetCurrentStoryboard() {
			if(CurrentStoryboard!= null) CurrentStoryboard.Completed -= CurrentStoryboardCompleted;
			if(animationType == AnimationType.None) return;
			Storyboard storyboard = null;
			if(Storyboards == null || Storyboards.Count == 0 || animationType == AnimationType.Fade) storyboard = DefaultStoryboard;
			if(animationType == AnimationType.SlideHorizontal) {
				storyboard = (from sb in Storyboards where GetStoryboardName(sb) == (animationDirection == SelectorAnimationDirection.Forward ? "FromLeft" : "FromRight") select sb).Single();
			}
			if(animationType == AnimationType.SlideVertical) {
				storyboard = (from sb in Storyboards where GetStoryboardName(sb) == (animationDirection == SelectorAnimationDirection.Forward ? "FromTop" : "FromBottom") select sb).Single();
			}
			if(storyboard != null) {
				storyboard = storyboard.Clone();
				if(!storyboard.IsPropertyAssigned(Timeline.DurationProperty)) {
					storyboard.SpeedRatio = ValidateAnimationSpeedRatio(animationSpeedRatio);
				}
				CurrentStoryboard = storyboard;
			}
			Storyboard.SetTarget(CurrentStoryboard, this);
			CurrentStoryboard.Completed += CurrentStoryboardCompleted;
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
		public double NewContentOpacity {
			get { return (double)GetValue(NewContentOpacityProperty); }
			set { SetValue(NewContentOpacityProperty, value); }
		}
		public double OldContentOpacity {
			get { return (double)GetValue(OldContentOpacityProperty); }
			set { SetValue(OldContentOpacityProperty, value); }
		}
	}
}
