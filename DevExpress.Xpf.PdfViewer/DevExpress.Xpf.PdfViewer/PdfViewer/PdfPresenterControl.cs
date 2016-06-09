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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfPresenterControl : DocumentPresenterControl {
		public static readonly DependencyProperty AllowCachePagesProperty;
		public static readonly DependencyProperty CacheSizeProperty;
		static readonly DependencyPropertyKey SearchParameterPropertyKey;
		public static readonly DependencyProperty SearchParameterProperty;
		public static readonly DependencyProperty AllowCurrentPageHighlightingProperty;
		static readonly DependencyPropertyKey SelectionRectanglePropertyKey;
		public static readonly DependencyProperty SelectionRectangleProperty;
		static PdfPresenterControl() {
			Type ownerType = typeof(PdfPresenterControl);
			AllowCachePagesProperty = DependencyPropertyManager.Register("AllowCachePages", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (o, args) => ((PdfPresenterControl)o).AllowCachePagesChanged((bool)args.NewValue)));
			CacheSizeProperty = DependencyPropertyManager.Register("CacheSize", typeof(int), ownerType,
				new FrameworkPropertyMetadata(300000000, FrameworkPropertyMetadataOptions.None, (o, args) => ((PdfPresenterControl)o).CacheSizeChanged((int)args.NewValue)));
			SearchParameterPropertyKey = DependencyPropertyManager.RegisterReadOnly("SearchParameter", typeof(TextSearchParameter), ownerType, new FrameworkPropertyMetadata(null));
			SearchParameterProperty = SearchParameterPropertyKey.DependencyProperty;
			AllowCurrentPageHighlightingProperty = DependencyPropertyManager.Register("AllowCurrentPageHighlighting", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (obj, args) => ((PdfPresenterControl)obj).OnAllowCurrentPageHighlightingChanged((bool)args.NewValue)));
			SelectionRectanglePropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectionRectangle", typeof(SelectionRectangle), ownerType, new FrameworkPropertyMetadata(null));
			SelectionRectangleProperty = SelectionRectanglePropertyKey.DependencyProperty;
		}
		public TextSearchParameter SearchParameter {
			get { return (TextSearchParameter)GetValue(SearchParameterProperty); }
			private set { SetValue(SearchParameterPropertyKey, value); }
		}
		public bool AllowCachePages {
			get { return (bool)GetValue(AllowCachePagesProperty); }
			set { SetValue(AllowCachePagesProperty, value); }
		}
		public int CacheSize {
			get { return (int)GetValue(CacheSizeProperty); }
			set { SetValue(CacheSizeProperty, value); }
		}
		public bool AllowCurrentPageHighlighting {
			get { return (bool)GetValue(AllowCurrentPageHighlightingProperty); }
			set { SetValue(AllowCurrentPageHighlightingProperty, value); }
		}
		public CursorModeType CursorMode {
			get { return ActualPdfViewer.Return(x => x.CursorMode, () => CursorModeType.SelectTool); }
		}
		public SelectionRectangle SelectionRectangle {
			get { return (SelectionRectangle)GetValue(SelectionRectangleProperty); }
			private set { SetValue(SelectionRectanglePropertyKey, value); }
		}
		public Color HighlightSelectionColor {
			get { return ActualPdfViewer.Return(x => x.HighlightSelectionColor, () => Color.FromArgb(89, 96, 152, 192)); }
		}
		public Color CaretColor {
			get { return ActualPdfViewer.Return(x => x.CaretColor, () => Colors.Black); }
		}
		public PdfBehaviorProvider PdfBehaviorProvider {
			get { return base.BehaviorProvider as PdfBehaviorProvider; }
		}
		protected new PdfKeyboardAndMouseController KeyboardAndMouseController {
			get { return base.KeyboardAndMouseController as PdfKeyboardAndMouseController; }
		}
		internal new PdfNavigationStrategy NavigationStrategy {
			get { return base.NavigationStrategy as PdfNavigationStrategy; }
		}
		protected internal InplaceEditingStrategy EditingStrategy { get; private set; }
		internal new IPdfDocument Document {
			get { return base.Document as IPdfDocument; }
		}
		internal new bool HasPages {
			get { return base.HasPages; }
		}
		PdfPresenterDecorator PresenterDecorator { get; set; }
		public new DocumentViewerPanel ItemsPanel {
			get { return base.ItemsPanel; }
		}
		public new ScrollViewer ScrollViewer {
			get { return base.ScrollViewer; }
		}
		public CellEditorOwner ActiveEditorOwner { get; private set; }
		public bool IsInEditing {
			get { return ActiveEditorOwner != null; }
		}
		internal new ImmediateActionsManager ImmediateActionsManager {
			get { return base.ImmediateActionsManager; }
		}
		PdfPageControl ActiveTooltipPage { get; set; }
		public void Update() {
			if (!HasPages)
				return;
			UpdateInternal();
		}
		protected virtual void UpdateInternal() {
			if(ActualPdfViewer != null)
				ActualPdfViewer.HasSelection = Document.SelectionResults != null;
			var documentViewModel = Document as PdfDocumentViewModel;
			if (documentViewModel != null)
				documentViewModel.IsDocumentModified = documentViewModel.DocumentStateController.IsDocumentModified;
			UpdatePagesProperties();
			RenderContent();
		}
		internal void BringCurrentSelectionPointIntoView() {
			KeyboardAndMouseController.BringCurrentSelectionPointIntoView();
		}
		protected internal Rect CalcRect(int pageIndex, PdfPoint topLeft, PdfPoint bottomRight) {
			return new Rect(CalcPoint(pageIndex, topLeft), CalcPoint(pageIndex, bottomRight));
		}
		public PdfPresenterControl() {
			DefaultStyleKey = typeof(PdfPresenterControl);
			SelectionRectangle = new SelectionRectangle(new Point(), Size.Empty);
			EditingStrategy = CreateEditingStrategy();
		}
		protected virtual void OnAllowCurrentPageHighlightingChanged(bool newValue) {
			Document.Do(x => x.SetCurrentPage(BehaviorProvider.PageIndex, newValue));
		}
		protected override void OnDocumentChanged(IDocument oldValue, IDocument newValue) {
			oldValue.With(x => x as PdfDocumentViewModel).Do(x => x.DocumentProgressChanged -= OnDocumentProgressChanged);
			base.OnDocumentChanged(oldValue, newValue);
			newValue.With(x => x as PdfDocumentViewModel).Do(x => x.DocumentProgressChanged += OnDocumentProgressChanged);
			Renderer.Do(x => x.Reset());
			SearchParameter = newValue != null ? new TextSearchParameter() { CurrentPage = PdfBehaviorProvider.Return(x => x.CurrentPageNumber, () => 1) } : null;
		}
		protected virtual InplaceEditingStrategy CreateEditingStrategy() {
			return new InplaceEditingStrategy(this);
		}
		protected override NavigationStrategy CreateNavigationStrategy() {
			return new PdfNavigationStrategy(this);
		}
		protected override KeyboardAndMouseController CreateKeyboardAndMouseController() {
			return new PdfKeyboardAndMouseController(this);
		}
		void OnDocumentProgressChanged(object sender, DocumentProgressChangedEventArgs args) {
			if (args.IsCompleted)
				Initialize();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			UpdateRenderer();
		}
		void UpdateRenderer() {
			if (!IsInitialized || this.IsInDesignTool())
				return;
			UpdateNativeRenderer();
			RenderContent();
		}
		protected override DocumentViewerRenderer CreateNativeRenderer() {
			return new PdfViewerDocumentRenderer(this);
		}
		internal PdfViewerControl ActualPdfViewer {
			get { return ActualDocumentViewer as PdfViewerControl; }
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			PresenterDecorator.Do(x => x.Focus());
		}
		protected override void OnItemsControlLoaded(object sender, RoutedEventArgs e) {
			base.OnItemsControlLoaded(sender, e);
			PresenterDecorator = ((PdfPagesSelector)ItemsControl).PresenterDecorator;
			PresenterDecorator.Do(x => x.PreviewKeyDown += OnDecoratorKeyDown);
		}
		protected override void UnsubscribeFromEvents(DocumentViewerItemsControl itemsControl) {
			base.UnsubscribeFromEvents(itemsControl);
			PresenterDecorator.Do(x => x.PreviewKeyDown -= OnDecoratorKeyDown);
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) { }
		protected override void OnPreviewKeyDown(KeyEventArgs e) { }
		void OnDecoratorKeyDown(object sender, KeyEventArgs e) {
			if (!HasPages)
				return;
			KeyboardAndMouseController.Do(x => x.ProcessKeyDown(e));
		}
		protected override void RenderManagedContent() {
			UpdatePanel();
		}
		void TestFunctionalLimits(RenderedContent renderedContent) {
			if (!PdfBehaviorProvider.Return(x => x.ShouldTestFunctionalLimits, null))
				return;
			if (renderedContent.RenderedPages == null)
				return;
			if (renderedContent.RenderedPages.Any(x => x.Page.HasFunctionalLimits))
				PdfBehaviorProvider.RaiseFunctionalLimitsOccured();
		}
		protected internal virtual void UpdatePagesProperties() {
			if (!HasPages || BehaviorProvider == null || Pages == null)
				return;
			foreach (PdfPageWrapper pageWrapper in Pages) {
				pageWrapper.Pages.Cast<PdfPageViewModel>().ForEach(x => x.RenderContent = CalcRenderContent(pageWrapper, x));
			}
		}
		IEnumerable<PdfElement> CalcRenderContent(PdfPageWrapper pageWrapper, PdfPageViewModel page) {
			if (Document.With(x => x.Caret) != null)
				return CalcCaretObject(pageWrapper, page, Document.Caret);
			return Enumerable.Empty<PdfElement>();
		}
		IEnumerable<PdfElement> CalcCaretObject(PdfPageWrapper pageWrapper, PdfPageViewModel page, PdfCaret caret) {
			if (caret.Position.PageIndex == page.PageIndex) {
				return new List<PdfElement>() { new PdfCaretElement(new SolidColorBrush(CaretColor), pageWrapper, BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle, caret) };
			}
			return null;
		}
		protected internal Point CalcPoint(int pageIndex, PdfPoint point) {
			var pageWrapperIndex = NavigationStrategy.PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = (PdfPageWrapper)Pages.ElementAt(pageWrapperIndex);
			var page = (PdfPageViewModel)pageWrapper.Pages.Single(x => x.PageIndex == pageIndex);
			Point sp = page.GetPoint(point, BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle);
			var pageRect = pageWrapper.GetPageRect(page);
			sp = new Point(sp.X + pageRect.Left, sp.Y + pageRect.Top);
			return sp;
		}
		Rect CalcCaretRect(PdfCaret caret) {
			int pageIndex = caret.Position.PageIndex;
			var pageWrapperIndex = NavigationStrategy.PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = (PdfPageWrapper)Pages.ElementAt(pageWrapperIndex);
			PdfPageViewModel page = pageWrapper.Pages.Single(x => x.PageIndex == pageIndex) as PdfPageViewModel;
			var pageRect = pageWrapper.GetPageRect(page);
			Point startPoint = page.GetPoint(caret.ViewData.TopLeft, BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle);
			startPoint = new Point(startPoint.X + pageRect.Left, startPoint.Y + pageRect.Top);
			Point endPoint = page.GetPoint(new PdfPoint(caret.ViewData.TopLeft.X, caret.ViewData.TopLeft.Y - caret.ViewData.Height), BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle);
			endPoint = new Point(endPoint.X + pageRect.Left, endPoint.Y + pageRect.Top);
			return new Rect(startPoint, endPoint);
		}
		protected internal void UpdatePanel() {
			if (ItemsPanel == null)
				return;
			foreach (UIElement child in ItemsPanel.InternalChildren)
				child.Do(x => x.InvalidateVisual());
			ItemsPanel.InvalidatePanel();
		}
		internal DrawingBrush GenerateRenderMask(IEnumerable<RenderItem> drawingContent) {
			GeometryGroup rectangles = new GeometryGroup();
			foreach (var pair in drawingContent) {
				Rect rect = pair.Rect;
				rect = CalcPageRect(pair, rect);
				Geometry g = new RectangleGeometry(rect);
				rectangles.Children.Add(g);
			}
			GeometryDrawing geometryDrawing = new GeometryDrawing { Geometry = rectangles, Brush = Brushes.Green, Pen = new Pen() };
			return new DrawingBrush(geometryDrawing) { AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top, Stretch = Stretch.None, ViewboxUnits = BrushMappingMode.Absolute };
		}
		Rect CalcPageRect(RenderItem pair, Rect rect) {
			Control ctrl = (Control)ItemsControl.ItemContainerGenerator.ContainerFromItem(pair.PageWrapper);
			double deltaX = (ctrl.Padding.Left + ctrl.Padding.Right) / 2d;
			double deltaY = (ctrl.Padding.Top + ctrl.Padding.Bottom) / 2d;
			rect.Inflate(-deltaX, -deltaY);
			return rect;
		}
		protected internal IEnumerable<RenderItem> GetDrawingContent() {
			return GetDrawingContent(ItemsPanel);
		}
		IEnumerable<RenderItem> GetDrawingContent(DocumentViewerPanel panel) {
			var result = new List<RenderItem>();
			if (panel == null)
				return result;
			foreach (FrameworkElement child in panel.InternalChildren) {
				var pageWrapper = (PageWrapper)ItemsControl.ItemContainerGenerator.ItemFromContainer(child);
				var pageWrapperRect = LayoutHelper.GetRelativeElementRect(child, ItemsControl);
				foreach (var page in pageWrapper.Pages) {
					var rect = pageWrapper.GetPageRect(page);
					rect.Offset(pageWrapperRect.Left, pageWrapperRect.Top);
					if (!IsVisibleChild(rect))
						continue;
					var model = (PdfPageViewModel)page;
					result.Add(new RenderItem { Rect = rect, PageWrapper = pageWrapper, Page = model, NeedsInvalidate = model.NeedsInvalidate, ForceInvalidate = model.ForceInvalidate, });
				}
			}
			return result;
		}
		bool IsVisibleChild(Rect rect) {
			return rect.IntersectsWith(new Rect(0, 0, ItemsControl.ActualWidth, ItemsControl.ActualHeight));
		}
		public void ScrollIntoView(PdfTarget target) {
			NavigationStrategy.ScrollIntoView(target);
		}
		public void ScrollIntoView(int pageIndex, Rect rect, ScrollIntoViewMode mode) {
			NavigationStrategy.ScrollIntoView(pageIndex, rect, mode);
		}
		public void ScrollIntoView(int pageIndex) {
			ScrollIntoView(pageIndex, Rect.Empty, ScrollIntoViewMode.TopLeft);
		}
		protected virtual void AllowCachePagesChanged(bool newValue) {
			if (!IsInitialized)
				return;
			UpdateRenderer();
		}
		protected virtual void CacheSizeChanged(int newValue) {
			if (!IsInitialized)
				return;
			UpdateRenderer();
		}
		protected override void OnBehaviorProviderChanged(BehaviorProvider oldValue, BehaviorProvider newValue) {
			base.OnBehaviorProviderChanged(oldValue, newValue);
			oldValue.Do(x => x.PageIndexChanged -= PageIndexChanged);
			newValue.Do(x => x.PageIndexChanged += PageIndexChanged);
		}
		protected override void OnBehaviorProviderRotateAngleChanged(object sender, RotateAngleChangedEventArgs e) {
			base.OnBehaviorProviderRotateAngleChanged(sender, e);
			UpdatePageWrappersProperties();
		}
		public void EndEditing() {
			EditingStrategy.EndEditing();
		}
		public void StartEditing(PdfEditorSettings editorSettings, IPdfViewerValueEditingCallBack valueEditing) {
			EditingStrategy.StartEditing(editorSettings, valueEditing);
		}
		protected override PageWrapper CreatePageWrapper(IPage page) {
			return new PdfPageWrapper((PdfPageViewModel)page) { ZoomFactor = BehaviorProvider.ZoomFactor, RotateAngle = BehaviorProvider.RotateAngle };
		}
		protected override PageWrapper CreatePageWrapper(IEnumerable<IPage> pages) {
			return new PdfPageWrapper(pages.Cast<PdfPageViewModel>()) { ZoomFactor = BehaviorProvider.ZoomFactor, RotateAngle = BehaviorProvider.RotateAngle };
		}
		protected virtual void UpdatePageWrappersProperties() {
			double maxPageWrapperWidth = 0d;
			double maxPageWrapperMargin = 0d;
			double pageWrapperTwoPageCenter = 0d;
			if (ColumnsCount == 2) {
				pageWrapperTwoPageCenter = CalcPageWrapperTwoPageCenter();
				CalcMaxPageWrapperParams(pageWrapperTwoPageCenter, out maxPageWrapperWidth, out maxPageWrapperMargin);
			}
			foreach (PdfPageWrapper pageWrapper in Pages) {
				pageWrapper.PageWrapperWidth = maxPageWrapperWidth;
				pageWrapper.PageWrapperMargin = maxPageWrapperMargin;
				pageWrapper.PageWrapperTwoPageCenter = pageWrapperTwoPageCenter;
				pageWrapper.InitializeInternal();
			}
			UpdateBehaviorProviderProperties();
		}
		void CalcMaxPageWrapperParams(double pageWrapperCenter, out double maxPageWidth, out double maxPageMargin) {
			maxPageWidth = 0d;
			maxPageMargin = 0d;
			bool isVertical = (BehaviorProvider.RotateAngle / 90) % 2 == 0;
			foreach (var pageWrapper in Pages) {
				double lastPageWidth = isVertical ? pageWrapper.Pages.Last().PageSize.Width : pageWrapper.Pages.Last().PageSize.Height;
				if (lastPageWidth.GreaterThan(maxPageWidth)) {
					maxPageWidth = lastPageWidth;
					maxPageMargin = pageWrapper.CalcMarginSize().Width;
				}
			}
			maxPageWidth += pageWrapperCenter;
		}
		double CalcPageWrapperTwoPageCenter() {
			double maxFirstPageWidth = 0d;
			bool isVertical = (BehaviorProvider.RotateAngle / 90) % 2 == 0;
			foreach (var pageWrapper in Pages) {
				double firstPageWidth = isVertical ? pageWrapper.Pages.First().PageSize.Width : pageWrapper.Pages.First().PageSize.Height;
				if (firstPageWidth.GreaterThan(maxFirstPageWidth))
					maxFirstPageWidth = firstPageWidth;
			}
			return maxFirstPageWidth;
		}
		protected override void Initialize() {
			base.Initialize();
			if (ItemsControl == null || !IsInitialized)
				return;
			if (HasPages) {
				UpdatePageWrappersProperties();
				ImmediateActionsManager.EnqueueAction(new DelegateAction(Update));
				Document.Do(x => x.SetCurrentPage(BehaviorProvider.Return(y => y.PageIndex, () => 1), AllowCurrentPageHighlighting));
			}
		}
		protected override void OnScrollViewerViewportChanged(object sender, ScrollChangedEventArgs e) {
			base.OnScrollViewerViewportChanged(sender, e);
		}
		protected override void UpdatePages() {
			base.UpdatePages();
			ImmediateActionsManager.EnqueueAction(new DelegateAction(() => Update()));
		}
		protected override void OnShowSingleItemChanged(bool newValue) {
			base.OnShowSingleItemChanged(newValue);
			ImmediateActionsManager.EnqueueAction(new DelegateAction(() => Update()));
		}
		public PdfHitTestResult HitTest(Point point) {
			if (HasPages)
				return NavigationStrategy.ProcessHitTest(point);
			return null;
		}
		public Point ConvertDocumentPositionToPoint(PdfDocumentPosition documentPosition) {
			if (HasPages)
				return NavigationStrategy.ProcessConvertDocumentPosition(documentPosition);
			return new Point(0, 0);
		}
		public PdfDocumentPosition ConvertPointToDocumentPosition(Point point) {
			if (HasPages)
				return NavigationStrategy.ProcessConvertPoint(point);
			return null;
		}
		void PageIndexChanged(object sender, PageIndexChangedEventArgs e) {
			Document.Do(x => x.SetCurrentPage(e.PageIndex, AllowCurrentPageHighlighting));
		}
		protected internal void InvalidateRenderCaches() {
			Renderer.Do(x => x.Reset());
		}
		public void AttachEditorToTree(CellEditorOwner cellEditorOwner, int pageIndex, Func<Rect> rectHandler, double angle) {
			var pageControl = (PdfPageControl)LayoutHelper.FindElement(ItemsPanel, fr => (fr as PdfPageControl).If(x => ((PdfPageWrapper)x.DataContext).Pages.Any(page => page.PageIndex == pageIndex)).ReturnSuccess());
			if (pageControl == null)
				return;
			ActiveEditorOwner = cellEditorOwner;
			cellEditorOwner.Page = pageControl;
			pageControl.AddEditor(cellEditorOwner.VisualHost, rectHandler, angle);
		}
		public void DetachEditorFromTree() {
			if (ActiveEditorOwner == null)
				return;
			var page = ActiveEditorOwner.Page;
			page.RemoveEditor();
			ActiveEditorOwner = null;
		}
		public void HideTooltip() {
			if (ActiveTooltipPage == null)
				return;
			ActiveTooltipPage.HidePopup();
			ActiveTooltipPage = null;
		}
		public void ShowTooltip(PdfStickyNoteEditSettings stickyNoteSettings) {
			var pageIndex = stickyNoteSettings.DocumentArea.PageIndex;
			var pageControl = (PdfPageControl)LayoutHelper.FindElement(ItemsPanel, fr => (fr as PdfPageControl).If(x => ((PdfPageWrapper)x.DataContext).Pages.Any(page => page.PageIndex == pageIndex)).ReturnSuccess());
			if (pageControl == null)
				return;
			ActiveTooltipPage = pageControl;
			pageControl.ShowPopup(stickyNoteSettings.Title, stickyNoteSettings.EditValue.ToString(),
				() => CalcRect(pageIndex, stickyNoteSettings.DocumentArea.Area.TopLeft, stickyNoteSettings.DocumentArea.Area.BottomRight));
		}
		public void InvalidateRenderingOnIdle() {
			Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => {
				if (!this.IsInVisualTree())
					return;
				var renderedContent = GetDrawingContent();
				var renderItem = renderedContent.FirstOrDefault(x => x.NeedsInvalidate);
				if (renderItem != null) {
					renderItem.Page.ForceInvalidate = true;
					RenderContent();
				}
			}));
		}
	}
	public class InvalidateRenderingAction : IAggregateAction {
		readonly PdfPresenterControl control;
		public InvalidateRenderingAction(PdfPresenterControl control) {
			this.control = control;
		}
		public void Execute() {
			control.InvalidateRenderingOnIdle();
		}
		public bool CanAggregate(IAction action) {
			return action is InvalidateRenderingAction;
		}
	}
	public class PdfPresenterDecorator : Decorator {
	}
	public class DocumentHasPagesVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var pages = value as IEnumerable<PdfPageViewModel>;
			return pages.Return(x => !x.Any(), () => false) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
