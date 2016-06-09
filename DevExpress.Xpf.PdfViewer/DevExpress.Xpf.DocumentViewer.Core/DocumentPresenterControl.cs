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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer.Extensions;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using Size = System.Windows.Size;
namespace DevExpress.Xpf.DocumentViewer {
	public enum PageDisplayMode {
		Single,
		Columns,
		Wrap
	}
	public class DocumentPresenterControl : Control {
		public static readonly DependencyProperty PagesProperty;
		public static readonly DependencyProperty IsSearchControlVisibleProperty;
		static DocumentPresenterControl() {
			Type ownerType = typeof(DocumentPresenterControl);
			PagesProperty = DependencyPropertyManager.Register("Pages", typeof(ObservableCollection<PageWrapper>), ownerType,
				new FrameworkPropertyMetadata(null));
			IsSearchControlVisibleProperty = DependencyPropertyManager.Register("IsSearchControlVisible", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
		}
		void OnDocumentChangedInternal(IDocument oldValue, IDocument newValue) {
			OnDocumentChanged(oldValue, newValue);
			Initialize();
		}
		void OnBehaviorProviderChangedInternal(BehaviorProvider oldValue, BehaviorProvider newValue) {
			oldValue.Do(x => x.ZoomChanged -= OnBehaviorProviderZoomChanged);
			oldValue.Do(x => x.PageIndexChanged -= OnBehaviorProviderPageIndexChanged);
			oldValue.Do(x => x.RotateAngleChanged -= OnBehaviorProviderRotateAngleChanged);
			UpdateBehaviorProviderProperties();
			newValue.Do(x => x.ZoomChanged += OnBehaviorProviderZoomChanged);
			newValue.Do(x => x.PageIndexChanged += OnBehaviorProviderPageIndexChanged);
			newValue.Do(x => x.RotateAngleChanged += OnBehaviorProviderRotateAngleChanged);
			OnBehaviorProviderChanged(oldValue, newValue);
		}
		DocumentViewerRenderer nativeRenderer;
		IDocument document;
		BehaviorProvider behaviorProvider;
		PageDisplayMode pageDisplayMode;
		int columnsCount = 1;
		bool showCoverPage;
		double horizontalPageSpacing;
		bool showSingleItem;
		protected DocumentViewerRenderer Renderer { get { return nativeRenderer; } }
		public IDocument Document {
			get { return document; }
			internal set {
				if (document.Return(x => x.Equals(value), () => false))
					return;
				IDocument oldValue = document;
				document = value;
				OnDocumentChangedInternal(oldValue, value);
			}
		}
		public BehaviorProvider BehaviorProvider {
			get { return behaviorProvider; }
			internal set {
				if (behaviorProvider.Return(x => x.Equals(value), () => false))
					return;
				BehaviorProvider oldValue = behaviorProvider;
				behaviorProvider = value;
				OnBehaviorProviderChangedInternal(oldValue, value);
			}
		}
		public PageDisplayMode PageDisplayMode {
			get { return pageDisplayMode; }
			set {
				if (pageDisplayMode == value)
					return;
				PageDisplayMode oldValue = pageDisplayMode;
				pageDisplayMode = value;
				OnPageDisplayModeChanged(oldValue, value);
			}
		}
		public int ColumnsCount {
			get { return columnsCount; }
			set {
				if (columnsCount == value)
					return;
				int oldValue = columnsCount;
				columnsCount = value;
				OnColumnsCountChanged(oldValue, value);
			}
		}
		public bool ShowCoverPage {
			get { return showCoverPage; }
			set {
				if (showCoverPage == value)
					return;
				showCoverPage = value;
				OnShowCoverPageChanged(value);
			}
		}
		public double HorizontalPageSpacing {
			get { return horizontalPageSpacing; }
			set {
				if (horizontalPageSpacing.AreClose(value))
					return;
				double oldValue = horizontalPageSpacing;
				horizontalPageSpacing = value;
				OnHorizontalPageSpacingChanged(oldValue, value);
			}
		}
		public bool ShowSingleItem {
			get { return showSingleItem; }
			set {
				if (showSingleItem == value)
					return;
				showSingleItem = value;
				OnShowSingleItemChanged(value);
			}
		}
		public ObservableCollection<PageWrapper> Pages {
			get { return (ObservableCollection<PageWrapper>)GetValue(PagesProperty); }
			set { SetValue(PagesProperty, value); }
		}
		public bool IsSearchControlVisible {
			get { return (bool)GetValue(IsSearchControlVisibleProperty); }
			set { SetValue(IsSearchControlVisibleProperty, value); }
		}
		public UndoRedoManager UndoRedoManager {
			get { return ActualDocumentViewer.With(x => x.UndoRedoManager); }
		}
		protected internal DocumentViewerItemsControl ItemsControl { get; private set; }
		protected internal NativeImage NativeImage { get; private set; }
		INativeImageRendererCallback NativeImageRendererCallback { get; set; }
		protected internal DocumentViewerPanel ItemsPanel { get; private set; }
		protected internal ScrollViewer ScrollViewer { get; private set; }
		protected internal DocumentViewerControl ActualDocumentViewer { get { return DocumentViewerControl.GetActualViewer(this); } }
		protected internal ImmediateActionsManager ImmediateActionsManager { get { return immediateActionsManager; } }
		protected internal bool HasPages { get { return Document != null && Pages.Return(x => x.Any(), () => false); } }
		protected internal NavigationStrategy NavigationStrategy { get; private set; }
		protected KeyboardAndMouseController KeyboardAndMouseController { get; private set; }
		readonly ImmediateActionsManager immediateActionsManager;
		public DocumentPresenterControl() {
			DefaultStyleKey = typeof(DocumentPresenterControl);
			NavigationStrategy = CreateNavigationStrategy();
			KeyboardAndMouseController = CreateKeyboardAndMouseController();
			immediateActionsManager = new ImmediateActionsManager(this);
			LayoutUpdated += OnLayoutUpdated;
			Loaded += OnLoaded;
			UpdateNativeRenderer();
		}
		protected virtual void UpdateNativeRenderer() {
			this.nativeRenderer = CreateNativeRenderer();
			NativeImage.Do(x => x.Renderer = nativeRenderer);
		}
		protected virtual DocumentViewerRenderer CreateNativeRenderer() {
			return new DocumentViewerRenderer(this);
		}
		Size GetMaxPageSize() {
			return GetMaxPageSize(x => x.PageSize);
		}
		Size GetMaxPageVisibleSize() {
			return GetMaxPageSize(x => x.VisibleSize);
		}
		Size GetMaxPageSize(Func<PageWrapper, Size> sizeSelector) {
			if (!HasPages)
				return Size.Empty;
			double maxWidth = 0d;
			double maxHeight = 0d;
			foreach (var page in Pages) {
				maxWidth = Math.Max(maxWidth, sizeSelector(page).Width);
				maxHeight = Math.Max(maxHeight, sizeSelector(page).Height);
			}
			return new Size(maxWidth, maxHeight);
		}
		void UpdateViewportSize() {
			BehaviorProvider.Do(x => x.Viewport = GetViewportSize());
		}
		void OnScrollViewerScrollChangedInternal(object sender, System.Windows.Controls.ScrollChangedEventArgs e) {
			if (!e.ViewportHeightChange.IsZero() || !e.ViewportWidthChange.IsZero())
				OnScrollViewerViewportChanged(sender, e);
			OnScrollViewerScrollChanged(sender, e);
		}
		void OnScrollViewerLoaded(object sender, RoutedEventArgs e) {
			UpdateViewportSize();
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			ImmediateActionsManager.ExecuteActions();
		}
		void OnLoaded(object sender, EventArgs e) {
			ActualDocumentViewer.Do(x => x.AttachDocumentPresenterControl(this));
			NavigationStrategy.GenerateStartUpState();
		}
		public void Scroll(ScrollCommand command) {
			NavigationStrategy.ProcessScroll(command);
		}
		public void ScrollToVerticalOffset(double offset) {
			NavigationStrategy.ProcessScrollTo(offset, true);
		}
		public void ScrollToHorizontalOffset(double offset) {
			NavigationStrategy.ProcessScrollTo(offset, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsControl.Do(UnsubscribeFromEvents);
			ItemsControl = GetTemplateChild("PART_ItemsControl") as DocumentViewerItemsControl;
			ItemsControl.Do(SubscribeToEvents);
			NativeImage.Do(x => x.Renderer = null);
			NativeImage = GetTemplateChild("PART_NativeImage") as NativeImage;
			NativeImage.Do(x => x.Renderer = (INativeImageRenderer)Renderer);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			ActualDocumentViewer.Do(x => x.AttachDocumentPresenterControl(this));
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (!HasPages)
				return;
			KeyboardAndMouseController.ProcessKeyDown(e);
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			base.OnPreviewMouseMove(e);
			if (!HasPages)
				return;
			KeyboardAndMouseController.ProcessMouseMove(e);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (!HasPages)
				return;
			KeyboardAndMouseController.ProcessMouseLeftButtonDown(e);
			Focus();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			ReleaseMouseCapture();
			if (!HasPages)
				return;
			KeyboardAndMouseController.ProcessMouseLeftButtonUp(e);
		}
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
			base.OnPreviewMouseWheel(e);
			if (!HasPages)
				return;
			KeyboardAndMouseController.ProcessMouseWheel(e);
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(e);
			if (!HasPages)
				return;
			KeyboardAndMouseController.ProcessMouseRightButtonDown(e);
		}
		protected virtual void OnPageDisplayModeChanged(PageDisplayMode oldValue, PageDisplayMode newValue) {
			Initialize();
		}
		protected virtual void OnColumnsCountChanged(int oldValue, int newValue) {
			Initialize();
		}
		protected virtual void OnShowCoverPageChanged(bool newValue) {
			Initialize();
		}
		protected virtual void OnHorizontalPageSpacingChanged(double oldValue, double newValue) {
			Initialize();
		}
		protected virtual void OnShowSingleItemChanged(bool newValue) {
			ItemsPanel.Do(x => x.ShowSingleItem = newValue);
		}
		protected virtual Size GetViewportSize() {
			if (ItemsPanel == null || !ItemsControl.Return(x => x.IsInitialized, () => false) || !HasPages)
				return Size.Empty;
			Size marginSize = CalcPagesMarginSize();
			return new Size(Math.Max(0d, ItemsPanel.ActualWidth - marginSize.Width), Math.Max(0d, ItemsPanel.ActualHeight - marginSize.Height));
		}
		protected virtual Size CalcPagesMarginSize() {
			Size maxMarginSize = new Size();
			foreach (var pageWrapper in Pages) {
				var marginSize = pageWrapper.CalcMarginSize();
				if ((marginSize.Width * marginSize.Height).GreaterThanOrClose(maxMarginSize.Width * maxMarginSize.Height))
					maxMarginSize = marginSize;
			}
			return maxMarginSize;
		}
		protected virtual void UnsubscribeFromEvents(DocumentViewerItemsControl itemsControl) {
			ScrollViewer.Do(x => x.ScrollChanged -= OnScrollViewerScrollChangedInternal);
			ScrollViewer.Do(x => x.Loaded -= OnScrollViewerLoaded);
			itemsControl.Loaded -= OnItemsControlLoaded;
		}
		protected virtual void SubscribeToEvents(DocumentViewerItemsControl itemsControl) {
			itemsControl.Loaded += OnItemsControlLoaded;
		}
		protected virtual void OnBehaviorProviderZoomChanged(object sender, ZoomChangedEventArgs e) {
			if (HasPages)
				NavigationStrategy.ProcessZoomChanged(e);
			if (PageDisplayMode == PageDisplayMode.Wrap)
				UpdatePages();
		}
		protected virtual void OnBehaviorProviderPageIndexChanged(object sender, PageIndexChangedEventArgs e) {
			NavigationStrategy.ProcessPageIndexChanged(e.PageIndex);
		}
		protected virtual void OnBehaviorProviderRotateAngleChanged(object sender, RotateAngleChangedEventArgs e) {
			if (HasPages)
				NavigationStrategy.ProcessRotateAngleChanged(e);
		}
		protected virtual void OnItemsControlLoaded(object sender, RoutedEventArgs e) {
			ItemsPanel = ItemsControl.Panel;
			ScrollViewer = ItemsControl.ScrollViewer;
			ScrollViewer.Do(x => x.ScrollChanged += OnScrollViewerScrollChangedInternal);
			ScrollViewer.Do(x => x.Loaded += OnScrollViewerLoaded);
			Initialize();
		}
		protected virtual void UpdateBehaviorProviderProperties() {
			BehaviorProvider.Do(x => x.PageSize = GetMaxPageSize());
			BehaviorProvider.Do(x => x.PageVisibleSize = GetMaxPageVisibleSize());
			BehaviorProvider.Do(x => x.Viewport = GetViewportSize());
		}
		protected virtual void Initialize() {
			if (ItemsControl == null || !IsInitialized || BehaviorProvider == null || ScrollViewer == null)
				return;
			Pages = Document.Return(x => x.IsLoaded, () => false) ? GeneratePageWrappers(Document.Pages) : null;
			ItemsControl.ItemsSource = Pages;
			ItemsPanel.ShowSingleItem = ShowSingleItem;
			NavigationStrategy.GenerateStartUpState();
			UpdateBehaviorProviderProperties();
			NativeImage.Invalidate();
			if (Pages != null)
				ImmediateActionsManager.EnqueueAction(new DelegateAction(() => NavigationStrategy.ScrollToStartUp()));
		}
		protected virtual ObservableCollection<PageWrapper> GeneratePageWrappers(IEnumerable<IPage> pages) {
			var wrappers = new List<PageWrapper>();
			switch(PageDisplayMode) {
				case PageDisplayMode.Single:
					wrappers = pages.Select(x => CreatePageWrapper(x)).ToList();
					break;
				case PageDisplayMode.Columns:
					if (ShowCoverPage) {
						wrappers.Add(CreatePageWrapper(new [] { pages.First() }));
						wrappers[0].IsCoverPage = true;
						wrappers.AddRange(pages.Skip(1).Partition(ColumnsCount).Select(x => CreatePageWrapper(x)));
					}
					else
						wrappers = pages.Partition(ColumnsCount).Select(x => CreatePageWrapper(x)).ToList();
					wrappers.ForEach(x => x.IsColumnMode = true);
					break;
				case PageDisplayMode.Wrap:
					wrappers = WrapPages(pages).Select(x => CreatePageWrapper(x)).ToList();
					break;
			}
			wrappers.ForEach(x => x.HorizontalPageSpacing = HorizontalPageSpacing);
			return new ObservableCollection<PageWrapper>(wrappers);
		}
		protected virtual IEnumerable<IPage[]> WrapPages(IEnumerable<IPage> pages) {
			if (BehaviorProvider.ZoomMode != ZoomMode.Custom && BehaviorProvider.ZoomMode != ZoomMode.ActualSize)
				return pages.Select(x => new [] { x });
			var viewport = GetViewportSize();
			var result = new List<IPage[]>();
			double pagesWidth = 0d;
			var row = new List<IPage>();
			foreach (var page in pages) {
				double pageWidth = page.PageSize.Width * BehaviorProvider.ZoomFactor + page.Margin.Left + page.Margin.Right;
				pagesWidth += pageWidth;
				if(pagesWidth.GreaterThan(viewport.Width) && row.Count > 0) {
					result.Add(row.ToArray());
					row.Clear();
					pagesWidth = pageWidth;
				}
				row.Add(page);
			}
			if (row.Count > 0)
				result.Add(row.ToArray());
			return result;
		}
		protected virtual PageWrapper CreatePageWrapper(IPage page) {
			return new PageWrapper(page);
		}
		protected virtual PageWrapper CreatePageWrapper(IEnumerable<IPage> pages) {
			return new PageWrapper(pages);
		}
		protected virtual void OnDocumentChanged(IDocument oldValue, IDocument newValue) {
		}
		protected virtual void OnBehaviorProviderChanged(BehaviorProvider oldValue, BehaviorProvider newValue) {
		}
		protected virtual void OnSelectedIndexChanged(int oldValue, int newValue) {
		}
		protected virtual void OnScrollViewerScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e) {
			if (!HasPages)
				return;
			NavigationStrategy.ProcessScrollViewerScrollChanged(e);
			RenderContent();
		}
		protected virtual void OnScrollViewerViewportChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e) {
			UpdateViewportSize();
			if (PageDisplayMode == PageDisplayMode.Wrap)
				UpdatePages();
		}
		protected virtual void UpdatePages() {
			Pages = Document.Return(x => x.IsLoaded, () => false) ? GeneratePageWrappers(Document.Pages) : null;
			ItemsControl.ItemsSource = Pages;
			UpdateBehaviorProviderProperties();
		}
		protected virtual NavigationStrategy CreateNavigationStrategy() {
			return new NavigationStrategy(this);
		}
		protected virtual KeyboardAndMouseController CreateKeyboardAndMouseController() {
			return new KeyboardAndMouseController(this);
		}
		protected virtual void RenderContent() {
			RenderNativeContent();
			RenderManagedContent();
		}
		protected virtual void RenderNativeContent() {
			Renderer.Do(x => x.Invalidate());
		}
		protected virtual void RenderManagedContent() {
		}
	}
}
