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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
namespace DevExpress.DemoData.Helpers {
	public interface IView {
		bool IsReady { get; }
		event EventHandler Ready;
		bool IsVisible { get; set; }
		void OnHide();
		void OnClear();
	}
	[ContentProperty("Content")]
	public class ViewPresenter : Control {
		static readonly object ActualContentChangedCommandParameterDefaultValue = new object();
		#region Dependency Properties
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ActualContentProperty;
		public static readonly DependencyProperty DefaultStoryboardProperty;
		public static readonly DependencyProperty StoryboardProperty;
		public static readonly DependencyProperty StoryboardSelectorProperty;
		public static readonly DependencyProperty OldContentProperty;
		public static readonly DependencyProperty NewContentProperty;
		public static readonly DependencyProperty OldContentTranslateXProperty;
		public static readonly DependencyProperty NewContentTranslateXProperty;
		public static readonly DependencyProperty CachedContent1Property;
		public static readonly DependencyProperty CachedContent2Property;
		public static readonly DependencyProperty ActualContentChangedCommandProperty;
		public static readonly DependencyProperty ActualContentChangedCommandParameterProperty;
		public static readonly DependencyProperty UseAnimationProperty;
		public static readonly DependencyProperty DelayContentLoadProperty;
		static ViewPresenter() {
			Type ownerType = typeof(ViewPresenter);
			ContentProperty = DependencyProperty.Register("Content", typeof(object), ownerType, new PropertyMetadata(null, RaiseContentChanged));
			ActualContentProperty = DependencyProperty.Register("ActualContent", typeof(object), ownerType, new PropertyMetadata(null, RaiseActualContentChanged));
			DefaultStoryboardProperty = DevExpress.DemoData.Helpers.StoryboardProperty.Register("DefaultStoryboard", ownerType, null);
			StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(string), ownerType, new PropertyMetadata(string.Empty));
			StoryboardSelectorProperty = DependencyProperty.Register("StoryboardSelector", typeof(Func<object, string>), ownerType, new PropertyMetadata(null));
			OldContentProperty = DependencyProperty.Register("OldContent", typeof(ContentControl), ownerType, new PropertyMetadata(null));
			NewContentProperty = DependencyProperty.Register("NewContent", typeof(ContentControl), ownerType, new PropertyMetadata(null));
			OldContentTranslateXProperty = DependencyProperty.Register("OldContentTranslateX", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseOldContentTranslateXChanged));
			NewContentTranslateXProperty = DependencyProperty.Register("NewContentTranslateX", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseNewContentTranslateXChanged));
			CachedContent1Property = DependencyProperty.Register("CachedContent1", typeof(object), ownerType, new PropertyMetadata(null));
			CachedContent2Property = DependencyProperty.Register("CachedContent2", typeof(object), ownerType, new PropertyMetadata(null));
			ActualContentChangedCommandProperty = DependencyProperty.Register("ActualContentChangedCommand", typeof(ICommand), ownerType, new PropertyMetadata(null, RaiseActualContentChangedCommandChanged));
			ActualContentChangedCommandParameterProperty = DependencyProperty.Register("ActualContentChangedCommandParameter", typeof(object), ownerType, new PropertyMetadata(ActualContentChangedCommandParameterDefaultValue, RaiseActualContentChangedCommandParameterChanged));
			UseAnimationProperty = DependencyProperty.Register("UseAnimation", typeof(bool), ownerType, new PropertyMetadata(true));
			DelayContentLoadProperty = DependencyProperty.Register("DelayContentLoad", typeof(int), ownerType, new PropertyMetadata(0));
		}
		object actualContentValue = null;
		ICommand actualContentChangedCommandValue = null;
		object actualContentChangedCommandParameterValue = ActualContentChangedCommandParameterDefaultValue;
		static void RaiseContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).RaiseContentChanged(e.OldValue, e.NewValue);
		}
		static void RaiseActualContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).actualContentValue = e.NewValue;
			((ViewPresenter)d).RaiseActualContentChanged(e);
		}
		static void RaiseOldContentTranslateXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).RaiseOldContentTranslateXChanged();
		}
		static void RaiseNewContentTranslateXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).RaiseNewContentTranslateXChanged();
		}
		static void RaiseActualContentChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).actualContentChangedCommandValue = (ICommand)e.NewValue;
			((ViewPresenter)d).RaiseActualContentChangedCommandChanged(e);
		}
		static void RaiseActualContentChangedCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).actualContentChangedCommandParameterValue = e.NewValue;
		}
		#endregion
		bool animationInProgress = false;
		Grid grid;
		Storyboard storyboard;
		bool contentChanged = false;
		ContentPresenter root;
		ContentControl cachedContent1Presenter;
		ContentControl cachedContent2Presenter;
		public ViewPresenter() {
			SizeChanged += OnSizeChanged;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			Template = XamlReaderHelper.CreateControlTemplate(typeof(ViewPresenter), typeof(ContentPresenter));
			DevExpress.Internal.DXWindow.FocusHelper.SetFocusable(this, false);
		}
		public object Content { get { return GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
		public object ActualContent { get { return actualContentValue; } private set { SetValue(ActualContentProperty, value); } }
		public object CachedContent1 { get { return GetValue(CachedContent1Property); } set { SetValue(CachedContent1Property, value); } }
		public object CachedContent2 { get { return GetValue(CachedContent2Property); } set { SetValue(CachedContent2Property, value); } }
		public ContentControl CachedContentPresenter { get; set; }
		public string ContentTemplatePath { get; set; }
		public Storyboard DefaultStoryboard { get { return (Storyboard)GetValue(DefaultStoryboardProperty); } set { SetValue(DefaultStoryboardProperty, value); } }
		public string Storyboard { get { return (string)GetValue(StoryboardProperty); } set { SetValue(StoryboardProperty, value); } }
		public Func<object, string> StoryboardSelector { get { return (Func<object, string>)GetValue(StoryboardSelectorProperty); } set { SetValue(StoryboardSelectorProperty, value); } }
		public ContentControl OldContent { get { return (ContentControl)GetValue(OldContentProperty); } private set { SetValue(OldContentProperty, value); } }
		public ContentControl NewContent { get { return (ContentControl)GetValue(NewContentProperty); } private set { SetValue(NewContentProperty, value); } }
		public double OldContentTranslateX { get { return (double)GetValue(OldContentTranslateXProperty); } set { SetValue(OldContentTranslateXProperty, value); } }
		public double NewContentTranslateX { get { return (double)GetValue(NewContentTranslateXProperty); } set { SetValue(NewContentTranslateXProperty, value); } }
		TranslateTransform OldContentTranslate { get { return ((TransformGroup)OldContent.RenderTransform).Children[1] as TranslateTransform; } }
		TranslateTransform NewContentTranslate { get { return ((TransformGroup)NewContent.RenderTransform).Children[1] as TranslateTransform; } }
		public ICommand ActualContentChangedCommand { get { return actualContentChangedCommandValue; } set { SetValue(ActualContentChangedCommandProperty, value); } }
		public object ActualContentChangedCommandParameter { get { return actualContentChangedCommandParameterValue; } set { SetValue(ActualContentChangedCommandParameterProperty, value); } }
		public bool UseAnimation { get { return (bool)GetValue(UseAnimationProperty); } set { SetValue(UseAnimationProperty, value); } }
		public int DelayContentLoad { get { return (int)GetValue(DelayContentLoadProperty); } set { SetValue(DelayContentLoadProperty, value); } }
		public event DepPropertyChangedEventHandler ActualContentChanged;
		protected virtual void SubscribeToReady(object view, EventHandler handler) {
			IView v = view as IView;
			if(v != null)
				v.Ready += handler;
		}
		protected virtual void UnsubscribeFromReady(object view, EventHandler handler) {
			IView v = view as IView;
			if(v != null)
				v.Ready -= handler;
		}
		protected virtual bool IsReady(object view) {
			IView v = view as IView;
			return v == null ? true : v.IsReady;
		}
		protected virtual void SetIsVisible(object view, bool value) {
			IView v = view as IView;
			if(v != null)
				v.IsVisible = value;
		}
		protected virtual void OnHide(object view) {
			IView v = view as IView;
			if(v != null)
				v.OnHide();
		}
		protected virtual void OnClear(object view) {
			IView v = view as IView;
			if(v != null)
				v.OnClear();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			BuildVisualTree();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			ClearVisualTree();
		}
		void BuildVisualTree() {
			if(this.root == null) return;
			if(this.grid == null) {
				this.grid = new Grid();
				this.root.Content = this.grid;
			}
			if(this.grid.Children.Count == 0) {
				cachedContent1Presenter = new ContentControl() { RenderTransformOrigin = new Point(0.5, 0.5), Opacity = 0.0, IsHitTestVisible = false, IsEnabled = false, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch };
				cachedContent2Presenter = new ContentControl() { RenderTransformOrigin = new Point(0.5, 0.5), Opacity = 0.0, IsHitTestVisible = false, IsEnabled = false, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch };
				DisableFocusable(cachedContent1Presenter);
				DisableFocusable(cachedContent2Presenter);
				cachedContent1Presenter.SetBinding(ContentControl.ContentProperty, new Binding("CachedContent1") { Source = this, Mode = BindingMode.OneWay });
				cachedContent2Presenter.SetBinding(ContentControl.ContentProperty, new Binding("CachedContent2") { Source = this, Mode = BindingMode.OneWay });
				this.grid.Children.Add(cachedContent1Presenter);
				this.grid.Children.Add(cachedContent2Presenter);
				if(OldContent != null)
					this.grid.Children.Add(OldContent);
				if(NewContent != null)
					this.grid.Children.Add(NewContent);
			}
		}
		void ClearVisualTree() {
			if(this.root != null)
				this.root.Content = null;
			if(this.grid != null)
				this.grid.Children.Clear();
			this.grid = null;
		}
		void SetStoryboard(object content) {
			if(string.IsNullOrEmpty(Storyboard) && StoryboardSelector == null) {
				this.storyboard = DefaultStoryboard;
			} else {
				string name = string.IsNullOrEmpty(Storyboard) ? StoryboardSelector(content) : Storyboard;
				this.storyboard = Resources[name] as Storyboard;
			}
		}
		void RaiseContentChanged(object oldValue, object newValue) {
			if(this.animationInProgress) {
				this.contentChanged = true;
				return;
			}
			this.animationInProgress = true;
			StartContentChanging(newValue);
		}
		void StartContentChanging(object newContent) {
			if(OldContent != null && OldContent.Content != null) {
				SetIsVisible(OldContent.Content, false);
				OnHide(OldContent.Content);
			}
			int isCachedContent = 0;
			if(object.Equals(newContent, CachedContent1))
				isCachedContent = 1;
			else if(object.Equals(newContent, CachedContent2))
				isCachedContent = 2;
			if(isCachedContent > 0) {
				if(isCachedContent == 1)
					NewContent = cachedContent1Presenter;
				else if(isCachedContent == 2)
					NewContent = cachedContent2Presenter;
				NewContent.IsHitTestVisible = true;
				NewContent.IsEnabled = true;
			} else {
				DataTemplate newValueTemplate = BindingPathHelper.GetValue(newContent, ContentTemplatePath) as DataTemplate;
				NewContent = new ContentControl() { Content = XamlReaderHelper.CreateVisualTree(newContent, newValueTemplate), RenderTransformOrigin = new Point(0.5, 0.5), Opacity = 0.0, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch };
				DisableFocusable(NewContent);
			}
			Canvas.SetZIndex(NewContent, 5);
			InitTransform(NewContent);
			if(isCachedContent == 0 && this.grid != null)
				this.grid.Children.Add(NewContent);
			SetStoryboard(newContent);
			if(OldContent == null || this.storyboard == null || !UseAnimation) {
				NewContent.Opacity = 1.0;
				FinishContentChanging();
				return;
			}
			InitTransform(OldContent);
			System.Windows.Media.Animation.Storyboard.SetTarget(this.storyboard, this);
			this.storyboard.Completed += OnStoryboardCompleted;
			SubscribeToReady(NewContent.Content, OnNewValueReady);
			if(IsReady(NewContent.Content))
				OnNewValueReady(newContent, null);
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			NewContent.Opacity = 1.0;
			this.storyboard.Completed -= OnStoryboardCompleted;
			this.storyboard.Stop();
			this.storyboard = null;
			FinishContentChanging();
		}
		void OnNewValueReady(object sender, EventArgs e) {
			UnsubscribeFromReady(sender, OnNewValueReady);
			this.storyboard.Begin();
		}
		void FinishContentChanging() {
			if(OldContent != null) {
				if(!object.Equals(OldContent.Content, CachedContent1) && !object.Equals(OldContent.Content, CachedContent2)) {
					if(this.grid != null)
						this.grid.Children.Remove(OldContent);
					if(OldContent.Content != null)
						OnClear(OldContent.Content);
					OldContent.Content = null;
					OldContent.ContentTemplate = null;
				} else {
					OldContent.Opacity = 0.0;
					OldContent.IsHitTestVisible = false;
					OldContent.IsEnabled = false;
				}
				ClearTransform(OldContent);
			}
			OldContent = NewContent;
			NewContent = null;
			Canvas.SetZIndex(OldContent, 10);
			ClearTransform(OldContent);
			if(OldContent != null && OldContent.Content != null)
				SetIsVisible(OldContent.Content, true);
			this.animationInProgress = false;
			BackgroundHelper.DoInBackground(null, () => {
				ActualContent = OldContent.Content;
				if(this.contentChanged) {
					this.contentChanged = false;
					RaiseContentChanged(null, Content);
				}
			}, DelayContentLoad);
		}
		void InitTransform(FrameworkElement fe) {
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform());
			transform.Children.Add(new TranslateTransform());
			fe.RenderTransform = transform;
		}
		void ClearTransform(FrameworkElement fe) {
			fe.RenderTransform = null;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			Clip = new RectangleGeometry() { Rect = new Rect(0.0, 0.0, ActualWidth, ActualHeight) };
			if(OldContent != null)
				RaiseOldContentTranslateXChanged();
			if(NewContent != null)
				RaiseNewContentTranslateXChanged();
		}
		void RaiseOldContentTranslateXChanged() {
			if(!this.animationInProgress) return;
			OldContentTranslate.X = OldContentTranslateX * ActualWidth;
		}
		void RaiseNewContentTranslateXChanged() {
			if(!this.animationInProgress) return;
			NewContentTranslate.X = NewContentTranslateX * ActualWidth;
		}
		void DisableFocusable(ContentControl control) {
			control.Focusable = false;
			control.IsTabStop = false;
		}
		void RaiseActualContentChanged(DependencyPropertyChangedEventArgs e) {
			RaiseActualContentChanged(e.OldValue, e.NewValue);
		}
		void RaiseActualContentChanged(object oldValue, object newValue) {
			object parameter = ActualContentChangedCommandParameter;
			if(parameter == ActualContentChangedCommandParameterDefaultValue)
				parameter = newValue;
			if(ActualContentChangedCommand != null && ActualContentChangedCommand.CanExecute(parameter))
				ActualContentChangedCommand.Execute(parameter);
			if(ActualContentChanged != null)
				ActualContentChanged(this, new DepPropertyChangedEventArgs(ActualContentProperty, oldValue, newValue));
		}
		void RaiseActualContentChangedCommandChanged(DependencyPropertyChangedEventArgs e) {
			RaiseActualContentChanged(null, ActualContent);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.root = (ContentPresenter)GetTemplateChild(XamlReaderHelper.RootName);
			this.root.SetBinding(ContentPresenter.FlowDirectionProperty, new Binding("FlowDirection") { Source = this, Mode = BindingMode.OneWay });
			BuildVisualTree();
		}
	}
}
