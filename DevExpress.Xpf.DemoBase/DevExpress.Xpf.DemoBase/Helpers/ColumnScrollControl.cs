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

using System.Windows.Data;
using DevExpress.Internal.DXWindow;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.XtraSpellChecker.Parser;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.UI;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public interface IColumnScrollControlService {
		void UpdateAfterThemeChanged();
	}
	public class ColumnScrollControlService : ServiceBase, IColumnScrollControlService {
		public void UpdateAfterThemeChanged() {
			((ColumnScrollControl)AssociatedObject).UpdateScrollviewer();
		}
	}
	interface IColumnControl {
		IEnumerable<double> Columns { get; }
		event Action ColumnsChanged;
		Rect VisibleArea { get; set; }
		void OnAnimationStarted();
	}
	class ColumnScrollPanel : Panel {
		internal const double PageButtonsControlHeight = 8;
		readonly Lazy<ColumnScrollControl> control;
		readonly Lazy<DXBorder> scrollViewerContainer;
		readonly Lazy<ItemsControl> pageButtonsControl;
		Border ScrollViewerContainer { get { return scrollViewerContainer.Value; } }
		ItemsControl PageButtonsControl { get { return pageButtonsControl.Value; } }
		ColumnScrollControl Control { get { return control.Value; } }
		public ColumnScrollPanel() {
			scrollViewerContainer = new Lazy<DXBorder>(() => (DXBorder)FindName("scrollViewerContainer"));
			pageButtonsControl = new Lazy<ItemsControl>(() => (ItemsControl)FindName("pageButtonsControl"));
			control = new Lazy<ColumnScrollControl>(() => this.VisualParents().OfType<ColumnScrollControl>().First());
		}
		protected override Size MeasureOverride(Size availableSize) {
			ScrollViewerContainer.Measure(new Size(availableSize.Width, availableSize.Height - PageButtonsControlHeight));
			Control.UpdatePageButtons();
			PageButtonsControl.Measure(new Size(availableSize.Width, PageButtonsControlHeight));
			return availableSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			ScrollViewerContainer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height - PageButtonsControlHeight));
			PageButtonsControl.Arrange(new Rect(0, finalSize.Height - PageButtonsControlHeight, finalSize.Width, PageButtonsControlHeight));
			return finalSize;
		}
	}
	public class ColumnScrollControl : ContentControl {
		public class PageItem : BindableBase {
			bool isChecked = false;
			ColumnScrollControl carousel;
			public void SilentlyCheck(bool value) {
				SetProperty(ref isChecked, value, () => IsChecked);
			}
			public bool IsChecked {
				get { return isChecked; }
				set {
					SetProperty(ref isChecked, value, () => IsChecked, () => {
						if(value) {
							carousel.NavigateToPage(carousel.PageButtons.IndexOf(this));
						}
					});
				}
			}
			public PageItem(ColumnScrollControl carousel) {
				this.carousel = carousel;
			}
		}
		ScrollViewer scrollViewer;
		ColumnScrollPanel panel;
		double hardPadding = 10000d;
		IColumnControl TypedContent { get { return (IColumnControl)Content; } }
		FrameworkElement UIContent { get { return (FrameworkElement)Content; } }
		public int ItemsCount {
			get { return (int)GetValue(ItemsCountProperty); }
			set { SetValue(ItemsCountProperty, value); }
		}
		public static readonly DependencyProperty ItemsCountProperty =
			DependencyProperty.Register("ItemsCount", typeof(int), typeof(ColumnScrollControl), new PropertyMetadata(0));
		public int CurrentPage {
			get { return (int)GetValue(CurrentPageProperty); }
			set { SetValue(CurrentPageProperty, value); }
		}
		public static readonly DependencyProperty CurrentPageProperty =
			DependencyProperty.Register("CurrentPage", typeof(int), typeof(ColumnScrollControl), new PropertyMetadata(0));
		public int PagesCount {
			get { return (int)GetValue(PagesCountProperty); }
			set { SetValue(PagesCountProperty, value); }
		}
		public static readonly DependencyProperty PagesCountProperty =
			DependencyProperty.Register("PagesCount", typeof(int), typeof(ColumnScrollControl), new PropertyMetadata(0));
		public List<PageItem> PageButtons {
			get { return (List<PageItem>)GetValue(PageButtonsProperty); }
			set { SetValue(PageButtonsProperty, value); }
		}
		public static readonly DependencyProperty PageButtonsProperty =
			DependencyProperty.Register("PageButtons", typeof(List<PageItem>), typeof(ColumnScrollControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public bool IsFirstPageSelected {
			get { return (bool)GetValue(IsFirstPageSelectedProperty); }
			set { SetValue(IsFirstPageSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsFirstPageSelectedProperty =
			DependencyProperty.Register("IsFirstPageSelected", typeof(bool), typeof(ColumnScrollControl), new PropertyMetadata(true));
		public bool IsLastPageSelected {
			get { return (bool)GetValue(IsLastPageSelectedProperty); }
			set { SetValue(IsLastPageSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsLastPageSelectedProperty =
			DependencyProperty.Register("IsLastPageSelected", typeof(bool), typeof(ColumnScrollControl), new PropertyMetadata(false));
		public double ScrollViewerHorizontalOffset {
			get { return (double)GetValue(ScrollViewerHorizontalOffsetProperty); }
			set { SetValue(ScrollViewerHorizontalOffsetProperty, value); }
		}
		public static readonly DependencyProperty ScrollViewerHorizontalOffsetProperty =
			DependencyProperty.Register("ScrollViewerHorizontalOffset", typeof(double), typeof(ColumnScrollControl), new PropertyMetadata(0d, (o, a) =>
				((ColumnScrollControl)o).ScrollViewerHorizontalOffsetChanged((double)a.NewValue)
			));
		public bool IsOpacityMaskVisible {
			get { return (bool)GetValue(IsOpacityMaskVisibleProperty); }
			set { SetValue(IsOpacityMaskVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsOpacityMaskVisibleProperty =
			DependencyProperty.Register("IsOpacityMaskVisible", typeof(bool), typeof(ColumnScrollControl), new PropertyMetadata(false));
		public bool IsLeftOpacityMaskVisible {
			get { return (bool)GetValue(IsLeftOpacityMaskVisibleProperty); }
			set { SetValue(IsLeftOpacityMaskVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsLeftOpacityMaskVisibleProperty =
			DependencyProperty.Register("IsLeftOpacityMaskVisible", typeof(bool), typeof(ColumnScrollControl), new PropertyMetadata(false));
		void ScrollViewerHorizontalOffsetChanged(double p) {
			scrollViewer.ScrollToHorizontalOffset(p);
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			if (oldContent != null) {
				((FrameworkElement)oldContent).SizeChanged -= ColumnScrollPanel_SizeChanged;
				((IColumnControl)oldContent).ColumnsChanged -= InvalidatePanelMeasure;
			}
			if (newContent != null) {
				((FrameworkElement)newContent).SizeChanged += ColumnScrollPanel_SizeChanged;
				((IColumnControl)newContent).ColumnsChanged += InvalidatePanelMeasure;
			}
			if(newContent == null)
				return;
			InvalidatePanelMeasure();
		}
		void InvalidatePanelMeasure() {
			panel.Do(p => p.InvalidateMeasure());
		}
		void ColumnScrollPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidatePanelMeasure();
		}
		double GetViewportWidth() {
			return Math.Max(panel.DesiredSize.Width, 0d);
		}
		double GetViewportHeight() {
			return Math.Max(panel.DesiredSize.Height, 0);
		}
		internal void UpdatePageButtons() {
			var content = (IColumnControl)Content;
			if(content == null)
				return;
			ItemsCount = content.Columns.Count();
			if(ItemsCount == 0) {
				PagesCount = 0;
				PageButtons = new List<PageItem>();
				return;
			}
			PagesCount = (int)Math.Ceiling((double)ItemsCount / CalcColumnsPerPage());
			PageButtons = Enumerable.Range(0, PagesCount).Select(_ => new PageItem(this)).ToList();
			NavigateToPage(Math.Min(CurrentPage, PagesCount - 1));
			PageButtons.ElementAt(CurrentPage).SilentlyCheck(true);
			UpdateFirstLastPagesAndOpacityMask();
			UpdateVisibleArea();
		}
		private void UpdateFirstLastPagesAndOpacityMask() {
			IsLastPageSelected = CurrentPage == PagesCount - 1;
			IsFirstPageSelected = CurrentPage == 0;
			if (TypedContent != null && TypedContent.Columns != null && TypedContent.Columns.Any()) {
				double columnWidth = TypedContent.Columns.Average();
				Debug.Assert(Math.Abs(columnWidth - TypedContent.Columns.First()) < 0.01d);
				const double safeMargin = 5d;
				IsOpacityMaskVisible = Math.Abs(scrollViewer.DesiredSize.Width % columnWidth) > safeMargin;
			}
			if (IsLastPageSelected) {
				IsOpacityMaskVisible = false;
			}
		}
		public void NavigateToPage(int page, bool doNothingOnCurrentPage = true, bool instantly = false) {
			page = Math.Min(Math.Max(page, 0), PagesCount - 1);
			if (doNothingOnCurrentPage && CurrentPage == page)
				return;
			if(page == -1)
				return;
			double newPos = page * (int)(scrollViewer.DesiredSize.Width / TypedContent.Columns.First()) * TypedContent.Columns.First();
			if(Math.Abs(scrollViewer.HorizontalOffset - newPos) < 0.001)
				return;
			CurrentPage = page;
			PageButtons.ToList().ForEach(b => b.SilentlyCheck(false));
			PageButtons[page].SilentlyCheck(true);
			UIContent.IsHitTestVisible = false;
			TypedContent.OnAnimationStarted();
			IsLeftOpacityMaskVisible = true;
			var animation = new DoubleAnimation {
				From = scrollViewer.HorizontalOffset,
				To = CalcPageOffset(page),
				Duration = TimeSpan.FromMilliseconds(1000),
				FillBehavior = FillBehavior.Stop,
				EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseOut }
			};
			IsOpacityMaskVisible = true;
			Action onAnimationCompleted = () => {
				UpdateVisibleArea();
				UpdateFirstLastPagesAndOpacityMask();
				UIContent.IsHitTestVisible = true;
				IsLeftOpacityMaskVisible = false;
			};
			ScrollViewerHorizontalOffset = (double)animation.To;
			if (!instantly) {
				animation.Completed += (s, e) => onAnimationCompleted();
				BeginAnimation(ScrollViewerHorizontalOffsetProperty, animation);
			} else {
				onAnimationCompleted();
			}
		}
		int CalcColumnsPerPage() {
			return Math.Max((int)(scrollViewer.DesiredSize.Width / TypedContent.Columns.First()), 1);
		}
		double CalcPageOffset(int page) {
			return page * CalcColumnsPerPage() * TypedContent.Columns.First() + hardPadding;
		}
		void UpdateVisibleArea() {
			TypedContent.VisibleArea = new Rect(
				ScrollViewerHorizontalOffset - hardPadding,
				0,
				GetViewportWidth(),
				GetViewportHeight());
		}
		void NextButton_Click(object sender, RoutedEventArgs e) {
			NavigateToPage(Math.Min(CurrentPage + 1, PagesCount - 1));
		}
		void PrevButton_Click(object sender, RoutedEventArgs e) {
			NavigateToPage(Math.Max(CurrentPage - 1, 0));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			scrollViewer = (ScrollViewer)GetTemplateChild("scrollViewer");
			panel = (ColumnScrollPanel)GetTemplateChild("panel");
			MouseTouchAdapter.SubscribeToPointerUp((Button)GetTemplateChild("prevButton"), this,
				a => NavigateToPage(Math.Max(CurrentPage - 1, 0)));
			MouseTouchAdapter.SubscribeToPointerUp((Button)GetTemplateChild("nextButton"), this,
				a => NavigateToPage(Math.Min(CurrentPage + 1, PagesCount - 1)));
			MouseTouchAdapter.SubscribeToSwipe(scrollViewer, scrollViewer, _ => { }, ProgressSwipe, CompleteSwipe);
			UpdateScrollviewer();
		}
		internal void UpdateScrollviewer() {
			ScrollViewerHorizontalOffset = hardPadding;
			scrollViewer.ScrollToHorizontalOffset(hardPadding);
			if(CurrentPage != -1 && PagesCount > 0)
				NavigateToPage(CurrentPage, false, true);
		}
		void ProgressSwipe(Point position) {
			double correctedAccumulatedDelta = position.X;
			if (CurrentPage == 0 && position.X > 0) {
				correctedAccumulatedDelta = Math.Pow(position.X, 0.55d);
			}
			scrollViewer.ScrollToHorizontalOffset(CalcPageOffset(CurrentPage) - correctedAccumulatedDelta);
			UpdateFirstLastPagesAndOpacityMask();
		}
		void CompleteSwipe(Point position) {
			double toPrevPageDelta = scrollViewer.HorizontalOffset - CalcPageOffset(CurrentPage - 1);
			double toNextPageDelta = CalcPageOffset(CurrentPage + 1) - scrollViewer.HorizontalOffset;
			const double swipeSpaceCoeff = 0.08d;
			if(toPrevPageDelta < toNextPageDelta && Math.Abs(position.X) > swipeSpaceCoeff * scrollViewer.DesiredSize.Width) {
				NavigateToPage(CurrentPage - 1, false);
			} else if(Math.Abs(position.X) > swipeSpaceCoeff * scrollViewer.DesiredSize.Width) {
				NavigateToPage(CurrentPage + 1, false);
			} else {
				NavigateToPage(CurrentPage, false);
			}
		}
		public ColumnScrollControl() {
			this.SizeChanged += (s, e) => UpdateFirstLastPagesAndOpacityMask();
			this.PreviewMouseWheel += ColumnScrollPanel_MouseWheel;
		}
		double accumulatedDelta;
		void ColumnScrollPanel_MouseWheel(object sender, MouseWheelEventArgs e) {
			bool sameSign = (e.Delta > 0 && accumulatedDelta >= 0) || (e.Delta < 0 && accumulatedDelta <= 0);
			if (sameSign) {
				accumulatedDelta += e.Delta;
			} else {
				accumulatedDelta = 0d;
			}
			if (Math.Abs(accumulatedDelta) > 180d) {
				if(accumulatedDelta > 0) {
					PrevButton_Click(sender, e);
				} else if(accumulatedDelta < 0) {
					NextButton_Click(sender, e);
				}
				accumulatedDelta = 0d;
			}
		}
		static ColumnScrollControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnScrollControl), new FrameworkPropertyMetadata(typeof(ColumnScrollControl)));
		}
	}
	class ColumnScrollPanelVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var typed = value as IEnumerable;
			if (typed == null)
				return Visibility.Hidden;
			return typed.Cast<object>().Count() > 1 ? Visibility.Visible : Visibility.Hidden;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
