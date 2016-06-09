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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using System.Windows.Media;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.DocumentViewer {
	public class PageWrapper : BindableBase {
		double zoomFactor;
		double rotateAngle;
		double horizontalPageSpacing;
		readonly IEnumerable<IPage> pages;
		public Size VisibleSize { get; protected set; }
		public Size RenderSize { get; protected set; }
		public Size PageSize { get; protected set; }
		public IEnumerable<IPage> Pages { get { return pages; } }
		protected internal bool IsCoverPage { get; set; }
		protected internal bool IsColumnMode { get; set; }
		protected internal bool IsVertical { get { return (RotateAngle / 90) % 2 == 0; } }
		protected internal double HorizontalPageSpacing {
			get { return horizontalPageSpacing; }
			set { SetProperty(ref horizontalPageSpacing, value, () => HorizontalPageSpacing, OnHorizontalPageSpacingChanged); }
		}
		public double ZoomFactor {
			get { return zoomFactor; }
			set { SetProperty(ref zoomFactor, value, () => ZoomFactor, OnZoomFactorChanged); }
		}
		public double RotateAngle {
			get { return rotateAngle; }
			set { SetProperty(ref rotateAngle, value, () => RotateAngle, OnRotateAngleChanged); }
		}
		public PageWrapper(IPage page) : this(new List<IPage> { page }) { }
		public PageWrapper(IEnumerable<IPage> pages) {
			this.pages = pages;
			ZoomFactor = 1d;
			Initialize();
		}
		protected virtual void Initialize() {
			PageSize = CalcPageSize();
			VisibleSize = CalcVisibleSize();
			RenderSize = CalcRenderSize();
		}
		protected virtual void OnZoomFactorChanged() {
			RenderSize = CalcRenderSize();
		}
		protected virtual void OnRotateAngleChanged() {
			PageSize = CalcPageSize();
			VisibleSize = CalcVisibleSize();
			RenderSize = CalcRenderSize();
		}
		protected virtual void OnHorizontalPageSpacingChanged() {
			RenderSize = CalcRenderSize();
		}
		protected virtual Size CalcRenderSize() {
			Size pageMargin = CalcMarginSize();
			return new Size(PageSize.Width * ZoomFactor + pageMargin.Width, PageSize.Height * ZoomFactor + pageMargin.Height);
		}
		protected internal virtual Size CalcMarginSize() {
			double marginWidth = 0d;
			if (RotateAngle.AreClose(90))
				marginWidth = Pages.First().Margin.Bottom + Pages.Last().Margin.Top;
			else if (RotateAngle.AreClose(180))
				marginWidth = Pages.First().Margin.Right + Pages.Last().Margin.Left;
			else if (RotateAngle.AreClose(270))
				marginWidth = Pages.First().Margin.Top + Pages.Last().Margin.Bottom;
			else
				marginWidth = Pages.First().Margin.Left + Pages.Last().Margin.Right;
			double width = marginWidth + (Pages.Count() - 1) * HorizontalPageSpacing;
			double height = 0d;
			foreach (var page in Pages)
				height = Math.Max(IsVertical ? page.Margin.Top + page.Margin.Bottom : page.Margin.Left + page.Margin.Right, height);
			return new Size(width, height);
		}
		protected virtual Size CalcVisibleSize() {
			return CalcSize(x => x.VisibleSize);
		}
		protected virtual Size CalcPageSize() {
			return CalcSize(x => x.PageSize);
		}
		protected internal virtual double CalcFirstPageLeftOffset() {
			return 0d;
		}
		protected virtual double CalcPageTopOffset(IPage page) {
			return 0d;
		}
		Size CalcSize(Func<IPage, Size> sizeHandler) {
			double width = 0;
			double height = 0;
			foreach (var page in pages) {
				Size size = sizeHandler(page);
				width += IsVertical ? size.Width : size.Height;
				height = Math.Max(height, IsVertical ? size.Height : size.Width);
			}
			return new Size(width, height);
		}
		public Rect GetPageRect(IPage page) {
			if (!Pages.Any(x => x.PageIndex == page.PageIndex))
				return Rect.Empty;
			double firstPageMargin = 0d;
			double pageTopOffset = 0d;
			if (RotateAngle.AreClose(90)) {
				firstPageMargin = Pages.First().Margin.Bottom;
				pageTopOffset = page.Margin.Left;
			}
			else if (RotateAngle.AreClose(180)) {
				firstPageMargin = Pages.First().Margin.Right;
				pageTopOffset = page.Margin.Bottom;
			}
			else if (RotateAngle.AreClose(270)) {
				firstPageMargin = Pages.First().Margin.Top;
				pageTopOffset = page.Margin.Right;
			}
			else {
				firstPageMargin = Pages.First().Margin.Left;
				pageTopOffset = page.Margin.Top;
			}
			double leftOffset = CalcFirstPageLeftOffset() + firstPageMargin;
			double topOffset = CalcPageTopOffset(page) + pageTopOffset;
			foreach (var p in Pages) {
				if (p.PageIndex == page.PageIndex)
					break;
				leftOffset += HorizontalPageSpacing + (IsVertical ? p.PageSize.Width : p.PageSize.Height) * ZoomFactor;
			}
			return IsVertical ? new Rect(leftOffset, topOffset, page.PageSize.Width * ZoomFactor, page.PageSize.Height * ZoomFactor) :
				new Rect(leftOffset, topOffset, page.PageSize.Height * ZoomFactor, page.PageSize.Width * ZoomFactor);
		}
	}
	public class PageControl : ItemsControl {
		Panel Panel { get; set; }
		public PageControl() {
			DefaultStyleKey = typeof(PageControl);
			DataContextChanged += OnDataContextChanged;
			Loaded += OnLoaded;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateHorizontalAlignment();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateHorizontalAlignment();
		}
		void UpdateHorizontalAlignment() {
			var itemsPresenter = (ItemsPresenter)GetTemplateChild("PART_ItemsPresenter");
			if(itemsPresenter != null) {
				Panel = (Panel)LayoutHelper.FindElement(itemsPresenter, x => x is Panel);
				Panel.Do(x => x.Margin = new Thickness((DataContext as PageWrapper).Return(y => y.CalcFirstPageLeftOffset(), () => 0d), 0, 0, 0));
			}
		}
		protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ItemsSource = (e.NewValue as PageWrapper).With(x => x.Pages);
			Panel.Do(x => x.Margin = new Thickness((e.NewValue as PageWrapper).Return(y => y.CalcFirstPageLeftOffset(), () => 0d), 0, 0, 0));
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			((PageControlItem)element).DataContext = null;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new PageControlItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is PageControlItem;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			var page = item as IPage;
			if (page == null)
				return;
			((PageControlItem)element).DataContext = page;
			((PageControlItem)element).IsCoverPage = ((PageWrapper)DataContext).IsCoverPage;
			int pageIndex = ((PageWrapper)DataContext).Pages.ToList().IndexOf(page);
			if (pageIndex == 0)
				((PageControlItem)element).Position = PageControlItemPosition.Left;
			else if(pageIndex == ((PageWrapper)DataContext).Pages.Count() - 1)
				((PageControlItem)element).Position = PageControlItemPosition.Right;
			else
				((PageControlItem)element).Position = PageControlItemPosition.Middle;
		}
		protected override Size MeasureOverride(Size constraint) {
			foreach (var item in Items)
				(ItemContainerGenerator.ContainerFromItem(item) as UIElement).Do(x => x.InvalidateMeasure());
			Panel.Do(x => x.Margin = new Thickness((DataContext as PageWrapper).Return(y => y.CalcFirstPageLeftOffset(), () => 0d), 0, 0, 0));
			if (DataContext != null)
				return (DataContext as PageWrapper).Return(x => x.RenderSize, () => base.MeasureOverride(constraint));
			return base.MeasureOverride(constraint);
		}
	}
	public enum PageControlItemPosition {
		Left,
		Middle,
		Right
	}
	public class PageControlItem : Control {
		public static readonly DependencyProperty PositionProperty = DependencyPropertyRegistrator.Register<PageControlItem, PageControlItemPosition>(owner => owner.Position, PageControlItemPosition.Middle, (d, oldValue, newValue) => d.OnPositionChanged(newValue));
		public static readonly DependencyProperty IsCoverPageProperty = DependencyPropertyRegistrator.Register<PageControlItem, bool>(owner => owner.IsCoverPage, false, (d, oldValue, newValue) => d.OnIsCoverPageChanged(newValue));
		public static readonly DependencyProperty IsSelectedProperty = DependencyPropertyRegistrator.Register<PageControlItem, bool>(owner => owner.IsSelected, false, (d, oldValue, newValue) => d.OnIsSelectedChanged(newValue));
		protected virtual void OnIsCoverPageChanged(bool newValue) {
			UpdateMargin(DataContext != null ? ((IPage)DataContext).Margin : new Thickness(0));
		}
		protected virtual void OnPositionChanged(PageControlItemPosition newValue) {
			UpdateMargin(DataContext != null ? ((IPage)DataContext).Margin : new Thickness(0));
		}
		protected virtual void OnIsSelectedChanged(bool newValue) {
		}
		public PageControlItem() {
			DefaultStyleKey = typeof(PageControlItem);
			DataContextChanged += OnDataContextChanged;
		}
		public PageControlItemPosition Position {
			get { return (PageControlItemPosition)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		public bool IsCoverPage {
			get { return (bool)GetValue(IsCoverPageProperty); }
			set { SetValue(IsCoverPageProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		protected bool IsVertical { get { return DocumentViewer.With(x => x.ActualBehaviorProvider).Return(x => (x.RotateAngle / 90) % 2 == 0, () => true); } }
		protected DocumentViewerControl DocumentViewer { get { return DocumentViewerControl.GetActualViewer(this); } }
		protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			IPage page = e.NewValue as IPage;
			UpdateMargin(page != null ? page.Margin : new Thickness(0));
		}
		void UpdateMargin(Thickness margin) {
			if (IsCoverPage) {
				Margin = margin;
				return;
			}
			switch (Position) {
				case PageControlItemPosition.Left:
					margin.Right = DocumentViewer.Return(x => x.HorizontalPageSpacing, () => 0d) / 2d;
					break;
				case PageControlItemPosition.Middle:
					margin.Left = DocumentViewer.Return(x => x.HorizontalPageSpacing, () => 0d) / 2d;
					margin.Right = DocumentViewer.Return(x => x.HorizontalPageSpacing, () => 0d) / 2d;
					break;
				case PageControlItemPosition.Right:
					margin.Left = DocumentViewer.Return(x => x.HorizontalPageSpacing, () => 0d) / 2d;
					break;
			}
			Margin = margin;
		}
		protected override Size MeasureOverride(Size constraint) {
			var page = DataContext as IPage;
			if (DocumentViewer == null || page == null)
				return base.MeasureOverride(constraint);
			return IsVertical ? new Size(page.PageSize.Width * DocumentViewer.ZoomFactor, page.PageSize.Height * DocumentViewer.ZoomFactor) :
				new Size(page.PageSize.Height * DocumentViewer.ZoomFactor, page.PageSize.Width * DocumentViewer.ZoomFactor);
		}
	}
}
