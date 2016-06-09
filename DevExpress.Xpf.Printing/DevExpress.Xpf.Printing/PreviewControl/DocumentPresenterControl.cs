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
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using System.Windows.Controls;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Printing.PreviewControl.Rendering;
using DocumentPresenterControlBase = DevExpress.Xpf.DocumentViewer.DocumentPresenterControl;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.Xpf.Printing.PreviewControl.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Printing.PreviewControl;
using RectangleF = System.Drawing.RectangleF;
using Page = DevExpress.XtraPrinting.Page;
using DevExpress.XtraPrinting.Native;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Printing {
	public class DocumentPresenterControl : DocumentPresenterControlBase, DevExpress.Xpf.Printing.PreviewControl.IPagesPresenter {
		static readonly Type ownerType = typeof(DocumentPresenterControl);
		static readonly DependencyPropertyKey RenderedSourcePropertyKey;
		public static readonly DependencyProperty RenderedSourceProperty;
		static readonly DependencyPropertyKey RenderMaskPropertyKey;
		public static readonly DependencyProperty RenderMaskProperty;
		static readonly DependencyPropertyKey SelectionRectanglePropertyKey;
		public static readonly DependencyProperty SelectionRectangleProperty;
		public static readonly DependencyProperty HighlightSelectionColorProperty;
		static readonly DependencyPropertyKey SearchParameterPropertyKey;
		public static readonly DependencyProperty SearchParameterProperty;
		internal SelectionService SelectionService { get; set; }
		internal bool IsContentLoaded { get; private set; }
		protected internal new DocumentNavigationStrategy NavigationStrategy { get { return base.NavigationStrategy as DocumentNavigationStrategy; } }
		protected internal new DocumentPreviewControl ActualDocumentViewer { get { return base.ActualDocumentViewer as DocumentPreviewControl; } }
		internal UserInputController InputController { get { return base.KeyboardAndMouseController as UserInputController; } }
		public new IDocumentViewModel Document { get { return (IDocumentViewModel)base.Document; } }
		public Color HighlightSelectionColor {
			get { return (Color)GetValue(HighlightSelectionColorProperty); }
			set { SetValue(HighlightSelectionColorProperty, value); }
		}
		public CursorModeType CursorMode {
			get { return this.ActualDocumentViewer.Return(x => x.CursorMode, () => CursorModeType.SelectTool); }
		}
		public ImageSource RenderedSource {
			get { return (ImageSource)GetValue(RenderedSourceProperty); }
			private set { SetValue(RenderedSourcePropertyKey, value); }
		}
		public DrawingBrush RenderMask {
			get { return (DrawingBrush)GetValue(RenderMaskProperty); }
			private set { SetValue(RenderMaskPropertyKey, value); }
		}
		public SelectionRectangle SelectionRectangle {
			get { return (SelectionRectangle)GetValue(SelectionRectangleProperty); }
			private set { SetValue(SelectionRectanglePropertyKey, value); }
		}
		public TextSearchParameter SearchParameter {
			get { return (TextSearchParameter)GetValue(SearchParameterProperty); }
			private set { SetValue(SearchParameterPropertyKey, value); }
		}
		public new ScrollViewer ScrollViewer { get { return base.ScrollViewer; } }
		#region  ctor
		static DocumentPresenterControl() {
			RenderedSourcePropertyKey = DependencyPropertyManager.RegisterReadOnly("RenderedSource", typeof(ImageSource), ownerType, new FrameworkPropertyMetadata(null));
			RenderedSourceProperty = RenderedSourcePropertyKey.DependencyProperty;
			RenderMaskPropertyKey = DependencyPropertyManager.RegisterReadOnly("RenderMask", typeof(DrawingBrush), ownerType, new FrameworkPropertyMetadata(null));
			RenderMaskProperty = RenderMaskPropertyKey.DependencyProperty;
			SelectionRectanglePropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectionRectangle", typeof(SelectionRectangle), ownerType,
				new FrameworkPropertyMetadata(null));
			SelectionRectangleProperty = SelectionRectanglePropertyKey.DependencyProperty;
			HighlightSelectionColorProperty = DependencyPropertyManager.Register("HighlightSelectionColor", typeof(Color), ownerType,
				new FrameworkPropertyMetadata(Color.FromArgb(89, 96, 152, 192), (d, e) => ((DocumentPresenterControl)d).OnSelecitonColorChanged((Color)e.NewValue)));
			SearchParameterPropertyKey = DependencyPropertyManager.RegisterReadOnly("SearchParameter", typeof(TextSearchParameter), ownerType,
				new FrameworkPropertyMetadata(null));
			SearchParameterProperty = SearchParameterPropertyKey.DependencyProperty;
		}
		protected void OnSelecitonColorChanged(Color color) {
			SelectionService.Do(x => {
				x.SelectionColor = color.ToWinFormsColor();
				if(SelectionService.HasSelection)
					Update();
			});
		}
		public DocumentPresenterControl() {
			DefaultStyleKey = ownerType;
			RenderMask = new DrawingBrush { Drawing = new GeometryDrawing() };
			Unloaded += OnUnloaded;
			SelectionRectangle = new SelectionRectangle(new Point(), Size.Empty);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) { }
		protected override void OnBehaviorProviderChanged(BehaviorProvider oldValue, BehaviorProvider newValue) {
			base.OnBehaviorProviderChanged(oldValue, newValue);
			if(oldValue != null)
				oldValue.ZoomChanged -= OnZoomChanged;
			if(newValue != null) {
				newValue.ZoomChanged += OnZoomChanged;
			}
			if(SelectionService == null) {
				SelectionService = new SelectionService(this) { Zoom = (float)newValue.ZoomFactor };
				SelectionService.InvalidatePage += InvalidatePage;
			}
		}
		protected override void OnDocumentChanged(IDocument oldValue, IDocument newValue) {
			base.OnDocumentChanged(oldValue, newValue);
			if(newValue == null) {
				return;
			}
			Renderer.Do(x => x.Reset());
			SelectionService.Do(x => x.OnKillFocus());
			if(BehaviorProvider != null)
				NavigationStrategy.Do(x => x.ScrollToStartUp());
			(oldValue as DocumentViewModel).Do(x => {
				x.Pages.CollectionChanged -= OnPagesCollectionChanged;
				x.DocumentChanged -= OnInnerDocumentChanged;
			});
			(newValue as DocumentViewModel).Do(x => {
				x.Pages.CollectionChanged += OnPagesCollectionChanged;
				x.DocumentChanged += OnInnerDocumentChanged;
			});
			SearchParameter = newValue != null ? new TextSearchParameter() { CurrentPage = BehaviorProvider.Return(x => x.PageIndex + 1, () => 1) } : null;
		}
		void OnPagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			Pages = Document.With(x => GeneratePageWrappers(Document.Pages));
			ItemsControl.ItemsSource = Pages;
			UpdateBehaviorProviderProperties();
			NavigationStrategy.GenerateStartUpState();
			BehaviorProvider.Do(x => x.PageSize = GetMaxPageSize());
			BehaviorProvider.Do(x => x.PageVisibleSize = GetMaxPageVisibleSize());
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace) {
				var pages = GetPages().Select(x => x.First.Index).ToArray();
				(Document as DocumentViewModel).Do(x => x.AfterDrawPages(pages));
			}
			RenderContent();
		}
		void OnInnerDocumentChanged(object sender, EventArgs e) {
			Renderer.Reset();
			var pages = GetPages().Select(x => x.First.Index).ToArray();
			(Document as DocumentViewModel).Do(x => x.AfterDrawPages(pages));
			Update();
		}
		Size GetMaxPageSize() {
			return GetMaxPageSize(x => x.PageSize);
		}
		Size GetMaxPageVisibleSize() {
			return GetMaxPageSize(x => x.VisibleSize);
		}
		Size GetMaxPageSize(Func<PageWrapper, Size> sizeSelector) {
			if(Pages == null)
				return Size.Empty;
			double maxSquare = 0d;
			Size maxPageSize = Size.Empty;
			foreach(var page in Pages) {
				double pageSquare = sizeSelector(page).Width * sizeSelector(page).Height;
				if(pageSquare > maxSquare) {
					maxPageSize = sizeSelector(page);
					maxSquare = pageSquare;
				}
			}
			return maxPageSize;
		}
		void InvalidatePage(object sender, EventArgs e) {
			RenderContent();
		}
		#endregion
		#region DocumentPresenterControl overrides
		protected override void Initialize() {
			base.Initialize();
			if(ItemsControl == null || !IsInitialized)
				return;
			if(Document != null)
				ImmediateActionsManager.EnqueueAction(new DelegateAction(() => Update()));
		}
		protected override void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) {
			base.OnScrollViewerScrollChanged(sender, e);
			RenderContent();
			Document.Do(document => {
				if(document.Pages.Count > 0) {
					var pages = GetPages().Select(x => x.First.Index).ToArray();
					(document as DocumentViewModel).Do(x => x.AfterDrawPages(pages));
				}
			});
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			(ItemsControl as PageSelector).PresenterDecorator.Do(x => x.Focus());
		}
		protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e) {
			base.OnPreviewMouseWheel(e);
			if(this.InputController.IsSelecting)
				e.Handled = true;
		}
		protected override KeyboardAndMouseController CreateKeyboardAndMouseController() {
			return new UserInputController(this);
		}
		protected override NavigationStrategy CreateNavigationStrategy() {
			return new DocumentNavigationStrategy(this);
		}
		#endregion
		protected internal void InvalidateRenderCaches() {
			Renderer.Reset();
		}
		public void Update() {
			if(!this.IsLoaded || !Document.Return(x => x.IsLoaded, () => false))
				return;
			UpdateInternal();
		}
		protected virtual void UpdateInternal() {
			var documentViewModel = Document as DocumentViewModel;
			RenderContent();
		}
		protected override void OnItemsControlLoaded(object sender, RoutedEventArgs e) {
			base.OnItemsControlLoaded(sender, e);
			IsContentLoaded = true;
			ActualDocumentViewer.ActualCommandProvider.Do(x=> x.UpdateCommands());
		}
		void OnPagePaddingChanged(Thickness newValue) {
			RenderContent();
		}
		void OnZoomChanged(object sender, ZoomChangedEventArgs e) {
			SelectionService.Do(x => x.Zoom = (float)e.ZoomFactor);
		}
		void OnCacheSizeChanged(int p) {
			if(!IsInitialized)
				return;
			UpdateRenderer(new Size(ActualWidth, ActualHeight));
		}
		void UpdateRenderer(Size size) {
			if(!IsInitialized || this.IsInDesignTool())
				return;
			UpdateNativeRenderer();
			RenderContent();
		}
		protected override void RenderContent() {
			UpdatePanel();
			RenderNativeContent();
		}
		protected override DocumentViewerRenderer CreateNativeRenderer() {
			return new DocumentPreviewRenderer(this);
		}
		internal DrawingBrush GenerateRenderMask(IEnumerable<RenderItem> drawingContent) {
			GeometryGroup rectangles = new GeometryGroup();
			foreach(var pair in drawingContent) {
				Rect rect = pair.Rectangle;
				rect = CalcPageRect(pair, rect);
				Geometry g = new RectangleGeometry(rect);
				rectangles.Children.Add(g);
			}
			GeometryDrawing geometryDrawing = new GeometryDrawing { Geometry = rectangles, Brush = Brushes.Green, Pen = new Pen() };
			return new DrawingBrush(geometryDrawing) {
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top,
				Stretch = Stretch.None,
				ViewboxUnits = BrushMappingMode.Absolute
			};
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
			if(panel == null)
				return result;
			foreach(FrameworkElement child in panel.InternalChildren) {
				var pageWrapper = (PageWrapper)ItemsControl.ItemContainerGenerator.ItemFromContainer(child);
				var pageWrapperRect = LayoutHelper.GetRelativeElementRect(child, ItemsControl);
				foreach(var page in pageWrapper.Pages) {
					var rect = pageWrapper.GetPageRect(page);
					rect.Offset(pageWrapperRect.Left, pageWrapperRect.Top);
					if(!IsVisibleChild(rect))
						continue;
					result.Add(new RenderItem { Rectangle = rect, PageWrapper = pageWrapper, Page = (PageViewModel)page });
				}
			}
			return result;
		}
		bool IsVisibleChild(Rect rect) {
			return rect.IntersectsWith(new Rect(0, 0, ItemsControl.ActualWidth, ItemsControl.ActualHeight));
		}
		protected internal void UpdatePanel() {
			if(ItemsPanel == null)
				return;
			foreach(UIElement child in ItemsPanel.InternalChildren)
				child.InvalidateVisual();
			ItemsPanel.InvalidatePanel();
		}
		protected override PageWrapper CreatePageWrapper(IEnumerable<IPage> pages) {
			return new PageWrapper(pages) { ZoomFactor = BehaviorProvider.ZoomFactor };
		}
		protected override PageWrapper CreatePageWrapper(IPage page) {
			return new PageWrapper(page) { ZoomFactor = BehaviorProvider.ZoomFactor };
		}
		public IEnumerable<Pair<Page, RectangleF>> GetPages() {
			List<Pair<Page, RectangleF>> pages = new List<Pair<Page, RectangleF>>();
			if(ItemsPanel != null) {
				foreach(FrameworkElement child in ItemsPanel.InternalChildren) {
					var pageWrapper = (PageWrapper)ItemsControl.ItemContainerGenerator.ItemFromContainer(child);
					var pageWrapperRect = LayoutHelper.GetRelativeElementRect(child, ItemsControl);
					foreach(var page in pageWrapper.Pages) {
						var rect = pageWrapper.GetPageRect(page);
						rect.Offset(pageWrapperRect.Left, pageWrapperRect.Top);
						if(!IsVisibleChild(rect))
							continue;
						pages.Add(new Pair<Page, RectangleF>(((PageViewModel)page).Page, rect.ToWinFormsRectangle()));
					}
				}
			}
			return pages;
		}
	}
}
