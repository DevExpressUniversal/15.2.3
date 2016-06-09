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
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.Xpf.Core.MvvmSample.Helpers {
	public interface IView {
		bool ViewIsReadyToAppear { get; }
		bool ViewIsVisible { get; }
		event EventHandler ViewIsReadyToAppearChanged;
		event EventHandler ViewIsVisibleChanged;
		event EventHandler BeforeViewDisappear;
		event EventHandler AfterViewDisappear;
		void SetViewIsVisible(bool v);
		void RaiseBeforeViewDisappear();
		void RaiseAfterViewDisappear();
	}
	public class StoryboardCollection : List<Storyboard> { }
	[ContentProperty("Content")]
	public class ViewPresenter : Control {
		#region Dependency Properties
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty DefaultStoryboardProperty;
		public static readonly DependencyProperty StoryboardsProperty;
		public static readonly DependencyProperty StoryboardProperty;
		public static readonly DependencyProperty StoryboardSelectorProperty;
		public static readonly DependencyProperty OldContentProperty;
		public static readonly DependencyProperty NewContentProperty;
		public static readonly DependencyProperty OldContentTranslateXProperty;
		public static readonly DependencyProperty NewContentTranslateXProperty;
		public static readonly DependencyProperty StoryboardNameProperty;
		static ViewPresenter() {
			Type ownerType = typeof(ViewPresenter);
			ContentProperty = DependencyProperty.Register("Content", typeof(object), ownerType, new PropertyMetadata(null, RaiseContentChanged));
			DefaultStoryboardProperty = DependencyProperty.Register("DefaultStoryboard", typeof(Storyboard), ownerType, new PropertyMetadata(null
#if !SL
				, null, CoerceStoryboard
#endif
				));
			StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(string), ownerType, new PropertyMetadata(string.Empty));
			StoryboardSelectorProperty = DependencyProperty.Register("StoryboardSelector", typeof(object), ownerType, new PropertyMetadata(null));
			OldContentProperty = DependencyProperty.Register("OldContent", typeof(ContentPresenter), ownerType, new PropertyMetadata(null));
			NewContentProperty = DependencyProperty.Register("NewContent", typeof(ContentPresenter), ownerType, new PropertyMetadata(null));
			OldContentTranslateXProperty = DependencyProperty.Register("OldContentTranslateX", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseOldContentTranslateXChanged));
			NewContentTranslateXProperty = DependencyProperty.Register("NewContentTranslateX", typeof(double), ownerType, new PropertyMetadata(0.0, RaiseNewContentTranslateXChanged));
			StoryboardsProperty = DependencyProperty.Register("Storyboards", typeof(StoryboardCollection), ownerType, new PropertyMetadata(null));
			StoryboardNameProperty = DependencyProperty.RegisterAttached("StoryboardName", typeof(string), ownerType, new PropertyMetadata(null));
		}
#if !SL
		static object CoerceStoryboard(DependencyObject d, object source) {
			Storyboard sb = (Storyboard)source;
			return sb == null ? null : sb.Clone();
		}
#endif
		static void RaiseContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).RaiseContentChanged(e.OldValue, e.NewValue);
		}
		static void RaiseOldContentTranslateXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).RaiseOldContentTranslateXChanged();
		}
		static void RaiseNewContentTranslateXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ViewPresenter)d).RaiseNewContentTranslateXChanged();
		}
		#endregion
		bool animationInProgress = false;
		Grid grid;
		Storyboard storyboard;
		bool contentChanged = false;
		ContentPresenter root;
		public ViewPresenter() {
			this.DefaultStyleKey = typeof(ViewPresenter);
			SizeChanged += OnSizeChanged;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public static string GetStoryboardName(Storyboard sb) { return (string)sb.GetValue(StoryboardNameProperty); }
		public static void SetStoryboardName(Storyboard sb, string value) { sb.SetValue(StoryboardNameProperty, value); }
		public object Content { get { return GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
		public Storyboard DefaultStoryboard { get { return (Storyboard)GetValue(DefaultStoryboardProperty); } set { SetValue(DefaultStoryboardProperty, value); } }
		public string Storyboard { get { return (string)GetValue(StoryboardProperty); } set { SetValue(StoryboardProperty, value); } }
		public object StoryboardSelector { get { return GetValue(StoryboardSelectorProperty); } set { SetValue(StoryboardSelectorProperty, value); } }
		public ContentPresenter OldContent { get { return (ContentPresenter)GetValue(OldContentProperty); } private set { SetValue(OldContentProperty, value); } }
		public ContentPresenter NewContent { get { return (ContentPresenter)GetValue(NewContentProperty); } private set { SetValue(NewContentProperty, value); } }
		public double OldContentTranslateX { get { return (double)GetValue(OldContentTranslateXProperty); } set { SetValue(OldContentTranslateXProperty, value); } }
		public double NewContentTranslateX { get { return (double)GetValue(NewContentTranslateXProperty); } set { SetValue(NewContentTranslateXProperty, value); } }
		public StoryboardCollection Storyboards { get { return (StoryboardCollection)GetValue(StoryboardsProperty); } set { SetValue(StoryboardsProperty, value); } }
		TranslateTransform OldContentTranslate { get { return ((TransformGroup)OldContent.RenderTransform).Children[1] as TranslateTransform; } }
		TranslateTransform NewContentTranslate { get { return ((TransformGroup)NewContent.RenderTransform).Children[1] as TranslateTransform; } }
		protected virtual void SubscribeToViewIsReadyToAppearChanged(object view, EventHandler handler) {
			IView v = view as IView;
			if(v != null)
				v.ViewIsReadyToAppearChanged += handler;
		}
		protected virtual void UnsubscribeFromViewIsReadyToAppearChanged(object view, EventHandler handler) {
			IView v = view as IView;
			if(v != null)
				v.ViewIsReadyToAppearChanged -= handler;
		}
		protected virtual bool ViewIsReadyToAppear(object view) {
			IView v = view as IView;
			return v == null ? true : v.ViewIsReadyToAppear;
		}
		protected virtual void SetViewIsVisible(object view, bool value) {
			IView v = view as IView;
			if(v != null)
				v.SetViewIsVisible(value);
		}
		protected virtual void RaiseBeforeViewDisappear(object view) {
			IView v = view as IView;
			if(v != null)
				v.RaiseBeforeViewDisappear();
		}
		protected virtual void RaiseAfterViewDisappear(object view) {
			IView v = view as IView;
			if(v != null)
				v.RaiseAfterViewDisappear();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			BuildVisualTree();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			ClearVisualTree();
		}
		void BuildVisualTree() {
			if(this.grid == null) {
				this.grid = new Grid();
				this.root.Content = this.grid;
			}
			if(this.grid.Children.Count == 0) {
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
		void SetStoryboard(object oldContent, object newContent) {
			if(string.IsNullOrEmpty(Storyboard) && StoryboardSelector == null || Storyboards == null || Storyboards.Count == 0) {
				this.storyboard = DefaultStoryboard;
			} else {
				string name = string.IsNullOrEmpty(Storyboard) ? GetStoryboardSelector()(oldContent, newContent) : Storyboard;
				this.storyboard = (from sb in Storyboards where GetStoryboardName(sb) == name select sb).Single();
			}
		}
		Func<object, object, string> GetStoryboardSelector() {
			string value = StoryboardSelector as string;
			if(value != null) return (o, n) => value;
			Func<object, string> byNewSelector = StoryboardSelector as Func<object, string>;
			if(byNewSelector != null) return (o, n) => byNewSelector(n);
			Func<object, object, string> byOldAndNewSelector = StoryboardSelector as Func<object, object, string>;
			if(byOldAndNewSelector != null) return byOldAndNewSelector;
			throw new InvalidCastException("StoryboardSelector");
		}
		void RaiseContentChanged(object oldValue, object newValue) {
			if(this.animationInProgress) {
				this.contentChanged = true;
				return;
			}
			this.animationInProgress = true;
			if(OldContent != null && OldContent.Content != null) {
				OldContent.IsHitTestVisible = false;
				SetViewIsVisible(OldContent.Content, false);
				RaiseBeforeViewDisappear(OldContent.Content);
			}
			NewContent = new ContentPresenter() { Content = newValue, RenderTransformOrigin = new Point(0.5, 0.5), Opacity = 0.0 };
			NewContent.IsHitTestVisible = true;
			Canvas.SetZIndex(NewContent, 5);
			InitTransform(NewContent);
			if(this.grid != null)
				this.grid.Children.Add(NewContent);
			SetStoryboard(oldValue, newValue);
			if(OldContent == null || this.storyboard == null) {
				NewContent.Opacity = 1.0;
				FinishContentChanging();
				return;
			}
			System.Windows.Media.Animation.Storyboard.SetTarget(this.storyboard, this);
			this.storyboard.Completed += OnStoryboardCompleted;
			SubscribeToViewIsReadyToAppearChanged(newValue, OnNewValueViewIsReadyToAppearChanged);
			if(ViewIsReadyToAppear(newValue))
				OnNewValueViewIsReadyToAppearChanged(newValue, null);
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			NewContent.Opacity = 1.0;
			this.storyboard.Completed -= OnStoryboardCompleted;
			this.storyboard.Stop();
			this.storyboard = null;
			FinishContentChanging();
		}
		void OnNewValueViewIsReadyToAppearChanged(object sender, EventArgs e) {
			UnsubscribeFromViewIsReadyToAppearChanged(sender, OnNewValueViewIsReadyToAppearChanged);
			this.storyboard.Begin();
		}
		void FinishContentChanging() {
			if(OldContent != null) {
				if(this.grid != null)
					this.grid.Children.Remove(OldContent);
				if(OldContent.Content != null)
					RaiseAfterViewDisappear(OldContent.Content);
				OldContent.Content = null;
			}
			OldContent = NewContent;
			NewContent = null;
			Canvas.SetZIndex(OldContent, 10);
			InitTransform(OldContent);
			if(OldContent != null && OldContent.Content != null)
				SetViewIsVisible(OldContent.Content, true);
			this.animationInProgress = false;
			if(this.contentChanged) {
				this.contentChanged = false;
				RaiseContentChanged(OldContent, Content);
			}
		}
		void InitTransform(FrameworkElement fe) {
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform());
			transform.Children.Add(new TranslateTransform());
			fe.RenderTransform = transform;
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
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.root = (ContentPresenter)GetTemplateChild("Root");
			BuildVisualTree();
		}
	}
}
