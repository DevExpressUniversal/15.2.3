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
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.ComponentModel;
namespace DevExpress.Xpf.Controls {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum PositionInfoType {
		PositionDuration,
		Position,
		DurationLeft
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum ChaptersDisplayType {
		Text,
		Position
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class ChapterPanel : Panel {
		internal MediaSliderControl Container { get { return (MediaSliderControl)FindControlHelper.FindParentControlByType(this, typeof(MediaSliderControl)); } }
		internal double Duration {
			get {
				if((Container == null) || !Container.MediaElement.NaturalDuration.HasTimeSpan)
					return 0d;
				return Container.MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
			}
		}
		internal double TrackWidth {
			get {
				if(Container == null || Container.Slider == null) return 0d;
				return Container.Slider.TrackWidth;
			}
		}
		public ChapterPanel() {
			SizeChanged += OnSizeChanged;
		}
		protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidateMeasure();
			InvalidateArrange();
			UpdateLayout();
		}
		protected override Size MeasureOverride(Size availableSize) {
			double height = 0d;
			double width = 0d;
			foreach(UIElement child in Children) {
				Size needSize = CalculateElementSize(child);
				child.Measure(new Size(needSize.Width, availableSize.Height));
				width += child.DesiredSize.Width;
				height = Math.Max(child.DesiredSize.Height, height);
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double left = finalSize.Width - (ActualWidth - TrackWidth) / 2;
			for(int i = Children.Count - 1; i >= 0; i--) {
				UIElement element = Children[i];
				Size elemSize = CalculateElementSize(element);
				left -= elemSize.Width;
				Rect finalRect = new Rect(new Point(left, 0d), elemSize);
				element.Arrange(finalRect);
			}
			return finalSize;
		}
		Size CalculateElementSize(UIElement element) {
			PositionSliderItem item = element as PositionSliderItem;
			Size size = new Size();
			double num = 0d;
			int idx = Children.IndexOf(item);
			if((item != null) && (Duration != 0d)) {
				if(item.Chapter.Position.TotalMilliseconds >= Duration) return new Size();
				if(idx < (Children.Count - 1)) {
					PositionSliderItem nextItem = Children[idx + 1] as PositionSliderItem;
					if(nextItem.Chapter.Position.TotalMilliseconds <= Duration)
						num = nextItem.Chapter.Position.TotalMilliseconds - item.Chapter.Position.TotalMilliseconds;
					else num = Duration - item.Chapter.Position.TotalMilliseconds;
				} else
					num = Duration - item.Chapter.Position.TotalMilliseconds;
				num = num > 0 ? num : 0d;
				size.Width = TrackWidth * num / Duration;
				size.Height = element.DesiredSize.Height;
			}
			return size;
		}
	}
	[TemplatePart(Name = "PART_ChaptersBorder", Type = typeof(Border)),
	TemplatePart(Name = "PART_PositionSlider", Type = typeof(PositionSlider))]
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class MediaSliderControl : ItemsControl {
		public static readonly DependencyProperty ChaptersVisibilityProperty =
			DependencyProperty.Register("ChaptersVisibility", typeof(Visibility), typeof(MediaSliderControl), new PropertyMetadata(Visibility.Visible));
		public static readonly DependencyProperty ChaptersDisplayTypeProperty =
			DependencyProperty.Register("ChaptersDisplayType", typeof(ChaptersDisplayType), typeof(MediaSliderControl),
			new PropertyMetadata(ChaptersDisplayType.Text, (d, e) => ((MediaSliderControl)d).OnChapterDiplayTypeChanged()));
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty MediaManagerProperty =
			DependencyProperty.Register("MediaManager", typeof(MediaManager), typeof(MediaSliderControl), new PropertyMetadata(null));
		public ChaptersDisplayType ChaptersDisplayType {
			get { return (ChaptersDisplayType)GetValue(ChaptersDisplayTypeProperty); }
			set { SetValue(ChaptersDisplayTypeProperty, value); }
		}
		public Visibility ChaptersVisibility {
			get { return (Visibility)GetValue(ChaptersVisibilityProperty); }
			set { SetValue(ChaptersVisibilityProperty, value); }
		}
		public MediaSliderControl() {
			DefaultStyleKey = typeof(MediaSliderControl);
			Loaded += OnLoaded;
			DisplayMemberPath = "Text";
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InitializeControls();
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new PositionSliderItem();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			PositionSliderItem posSliderItem = element as PositionSliderItem;
			posSliderItem.Owner = this;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return (item is PositionSliderItem);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			MediaManager = FindControlHelper.GetMediaManager(VisualTreeHelper.GetParent(this));
			SubscribeEvents();
		}
		protected virtual void OnMediaElementMediaOpened(object sender, RoutedEventArgs e) {
			InitializeSource();
		}
		protected virtual void OnMediaManagerSelectedItemChanged(object sender, SelectedItemChangedEventArgs e) {
			chaptersBorder.Opacity = 0;
			ItemsSource = null;
		}
		protected virtual void OnMediaManagerCurrentStateChanged(object sender, CurrentStateChangedEventArgs e) {
			if(e.State == MediaManagerState.Playing) {
				IsHitTestVisible = true;
			} else if(e.State == MediaManagerState.Stopped || e.State == MediaManagerState.Closed) {
				IsHitTestVisible = false;
			}
		}
		protected virtual void OnChapterDiplayTypeChanged() {
			ChangeDisplayType();
		}
		internal MediaElement MediaElement { get { return ((MediaManager == null) ? null : MediaManager.MediaElement); } }
		internal MediaManager MediaManager {
			get { return (MediaManager)GetValue(MediaManagerProperty); }
			set { SetValue(MediaManagerProperty, value); }
		}
		internal PositionSlider Slider { get; set; }
		void SubscribeEvents() {
			if(MediaManager != null) {
				MediaManager.CurrentStateChanged += OnMediaManagerCurrentStateChanged;
				MediaManager.SelectedItemChanged += OnMediaManagerSelectedItemChanged;
				MediaElement.MediaOpened += OnMediaElementMediaOpened;
			}
		}
		void InitializeControls() {
			Slider = GetTemplateChild("PART_PositionSlider") as PositionSlider;
			chaptersBorder = GetTemplateChild("PART_ChaptersBorder") as Border;
		}
		void InitializeSource() {
			var chapters = MediaManager.SelectedItem.Chapters;
			chapters.Sort();
			ItemsSource = chapters;
			UpdateChaptersVisibility();
		}
		void UpdateChaptersVisibility() {
			if(chaptersBorder == null) return;
			if(MediaManager.SelectedItem == null || MediaManager.SelectedItem.Chapters.Count == 0)
				chaptersBorder.Opacity = 0;
			else chaptersBorder.Opacity = 1;
		}
		void ChangeDisplayType() {
			switch(ChaptersDisplayType) {
				case ChaptersDisplayType.Text:
					DisplayMemberPath = "Text";
					break;
				case ChaptersDisplayType.Position:
					DisplayMemberPath = "Position";
					break;
			}
		}
		Border chaptersBorder;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PositionSliderItem : ContentControl {
		public MediaChapter Chapter { get { return Content != null ? Content as MediaChapter : null; } }
		public MediaSliderControl Owner { get; set; }
		public PositionSliderItem() {
			DefaultStyleKey = typeof(PositionSliderItem);
			MouseLeftButtonDown += OnMouseLeftButtonDown;
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			VisualStateManager.GoToState(this, "MouseOver", false);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			VisualStateManager.GoToState(this, "Normal", false);
		}
		protected virtual void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			PositionSliderItem item = sender as PositionSliderItem;
			if(item == null) return;
			Owner.MediaElement.Position = Chapter.Position;
		}
	}
	[TemplatePart(Name = "PART_Track", Type = typeof(Border)),
	TemplatePart(Name = "PART_downloadProgress", Type = typeof(Border)),
	TemplatePart(Name = "PART_ProgressTrack", Type = typeof(Border)),
	TemplatePart(Name = "PART_progress", Type = typeof(Border)),
	TemplatePart(Name = "PART_Thumb", Type = typeof(Thumb)),
	TemplatePart(Name = "RART_PopupPositionTip", Type = typeof(PopupBase)),
	TemplatePart(Name = "PART_PopupText", Type = typeof(TextBlock))]
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PositionSlider : RangeBase {
		internal double CalculatedWidth { get { return ActualWidth - ThumbWidth; } }
		internal MediaManager MediaManager {
			get { return mm; }
			set {
				if(value == null || mm == value) return;
				mm = value;
				OnMediaManagerChanged();
			}
		}
		internal MediaElement MediaElement { get { return MediaManager != null ? MediaManager.MediaElement : null; ; } }
		internal double TrackWidth { get { return trackBorder != null ? trackBorder.ActualWidth : 0d; } }
		internal double ThumbWidth { get { return thumb != null ? thumb.ActualWidth : 0d; } }
		public PositionSlider() {
			DefaultStyleKey = typeof(PositionSlider);
			timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) };
			SubscribeEvents();
			DisableSlider();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			thumb = GetTemplateChild("PART_Thumb") as Thumb;
			progressBorder = GetTemplateChild("PART_progress") as Border;
			downloadBorder = GetTemplateChild("PART_downloadProgress") as Border;
			trackBorder = GetTemplateChild("PART_Track") as Border;
			progressTrackBorder = GetTemplateChild("PART_ProgressTrack") as Border;
			positionPopup = GetTemplateChild("RART_PopupPositionTip") as PopupBase;
			popupTextBlock = GetTemplateChild("PART_PopupText") as TextBlock;
			if(positionPopup == null) return;
			positionPopup.PlacementTarget = this;
		}
		protected override void OnValueChanged(double oldValue, double newValue) {
			base.OnValueChanged(oldValue, newValue);
			UpdateProgress();
			UpdatePopup();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			DoOnMouseEnter(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(isMouseDown)
				Value = GetValueByMousePosition(e.GetPosition(this));
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			DoOnMouseLeave(e);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(MediaManager == null) MediaManager = FindControlHelper.GetMediaManager(VisualTreeHelper.GetParent(this));
		}
		protected virtual void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Value = GetValueByMousePosition(e.GetPosition(this));
			CaptureMouse();
			isMouseDown = true;
			oldState = MediaManager.CurrentState;
			MediaElement.Pause();
		}
		protected virtual void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			MediaElement.Position = TimeSpan.FromMilliseconds(Value);
			ReleaseMouseCapture();
			isMouseDown = false;
			if(MediaManagerState.Playing == oldState)
				MediaElement.Play();
		}
		protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateProgress();
		}
		protected virtual void OnTimerTick(object sender, EventArgs e) {
			if(MediaElement == null) return;
			if(!isMouseDown)
				Value = MediaElement.Position.TotalMilliseconds;
			else
				MediaElement.Position = TimeSpan.FromMilliseconds(Value);
		}
		protected virtual void OnMediaManagerChanged() {
			SubscribeMediaManagerEvents();
		}
		protected virtual void OnMediaManagerSelectedItemChanged(object sender, SelectedItemChangedEventArgs e) {
			DisableSlider();
		}
		protected virtual void OnMediaManagerCurrentStateChanged(object sender, CurrentStateChangedEventArgs e) {
			if(e.State == MediaManagerState.Playing) {
				EnableSlider();
			} else if(e.State == MediaManagerState.Stopped || e.State == MediaManagerState.Closed) {
				DisableSlider();
			}
		}
		protected internal virtual void OnMediaElementMediaEnded(object sender, RoutedEventArgs e) {
			DisableSlider();
		}
		protected internal virtual void OnMediaElementMediaOpened(object sender, RoutedEventArgs e) {
			if(((MediaElement)sender).NaturalDuration.HasTimeSpan) {
				EnableSlider();
			} else
				DisableSlider();
		}
		void UpdateProgress() {
			progressBorder.Width = Value * CalculatedWidth / Maximum;
			if(progressTrackBorder != null) {
				double width = trackBorder != null ? trackBorder.ActualWidth : ActualWidth;
				progressTrackBorder.Width = Value * width / Maximum;
			}
		}
		double GetValueByMousePosition(Point point) {
			return (point.X - ThumbWidth / 2) / CalculatedWidth * Maximum;
		}
		void SubscribeEvents() {
			Loaded += OnLoaded;
			SizeChanged += OnSizeChanged;
#if WPF
			PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
#else
			MouseLeftButtonDown += OnMouseLeftButtonDown;
			MouseLeftButtonUp += OnMouseLeftButtonUp;
			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;
#endif
			timer.Tick += OnTimerTick;
		}
		void SubscribeMediaManagerEvents() {
			if(MediaManager != null) {
				MediaManager.SelectedItemChanged += OnMediaManagerSelectedItemChanged;
				MediaManager.CurrentStateChanged += OnMediaManagerCurrentStateChanged;
			}
			if(MediaElement != null) {
				MediaElement.MediaOpened += OnMediaElementMediaOpened;
				MediaElement.MediaEnded += OnMediaElementMediaEnded;
			}
		}
		void DisableSlider() {
			timer.Stop();
			Maximum = 1d;
			Value = 0d;
		}
		void EnableSlider() {
			if(MediaElement.NaturalDuration.HasTimeSpan) {
				Maximum = MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
				timer.Start();
			}
			if(downloadBorder != null)
				SetDownloadBorderVisibility();
		}
		void SetDownloadBorderVisibility() {
#if WPF
			if(MediaElement.Source.IsFile)
				downloadBorder.Visibility = Visibility.Collapsed;
			else
				downloadBorder.Visibility = Visibility.Visible;
#endif
		}
		void DoOnMouseEnter(MouseEventArgs e) {
			if(positionPopup != null)
				positionPopup.IsOpen = true;
			VisualStateManager.GoToState(this, "MouseOver", false);
		}
		void DoOnMouseLeave(MouseEventArgs e) {
			if(positionPopup != null)
				positionPopup.IsOpen = false;
			VisualStateManager.GoToState(this, "Normal", false);
		}
		void UpdatePopup() {
			if(positionPopup == null || popupTextBlock == null) return;
			string strPosition = TimeSpan.FromMilliseconds(Value).ToString();
			popupTextBlock.Text = strPosition.Contains('.') ? strPosition.Remove(strPosition.IndexOf('.')) : strPosition;
			double offset = popupTextBlock != null ? progressBorder.ActualWidth - popupTextBlock.ActualWidth / 3 : progressBorder.ActualWidth;
			positionPopup.HorizontalOffset = offset;
		}
		bool isMouseDown;
		Thumb thumb;
		Border progressBorder;
		Border downloadBorder;
		Border trackBorder;
		Border progressTrackBorder;
		DispatcherTimer timer;
		MediaManager mm;
		MediaManagerState oldState;
		PopupBase positionPopup;
		TextBlock popupTextBlock;
	}
}
