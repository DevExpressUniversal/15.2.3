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
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using RichEditLayoutPage = DevExpress.XtraRichEdit.Layout.Page;
using PlatformBrush = System.Windows.Media.Brush;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.XtraRichEdit;
using System.Windows.Threading;
using DevExpress.Office.Internal;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
using DevExpress.Xpf.Drawing;
using Padding = DevExpress.Xpf.Core.Native.Padding;
#else
using PlatformIndependentColor = System.Drawing.Color;
using Padding = System.Windows.Forms.Padding;
#endif
namespace DevExpress.Xpf.RichEdit.Views {
	public class CacheElement {
		public RichEditViewPage AgPage { get; set; }
		public RichEditLayoutPage Page { get; set; }
		public bool DoUpdate { get; set; }
		public int NumPages { get; set; }
		public CacheElement(RichEditViewPage agPage, RichEditLayoutPage page) {
			AgPage = agPage;
			Page = page;
		}
	}
	public interface IXpfRichEditView {
		XpfRichEditViewAdapter Adapter { get; }
		void UpdateBackground();
	}
	public class XpfSimpleView : SimpleView, IXpfRichEditView {
		readonly XpfRichEditViewAdapter adapter;
		PlatformBrush cachedBackground;
		PlatformBrush cachedBorderBackground;
		public XpfSimpleView(IRichEditControl control)
			: base(control) {
			this.adapter = new XpfRichEditViewAdapter(this);
		}
		public XpfRichEditViewAdapter Adapter { get { return adapter; } }
		RichEditControl XpfControl { get { return (RichEditControl)Control; } }
		protected internal override void Activate(Rectangle viewBounds) {
			RichEditControl control = (RichEditControl)Control;
			this.cachedBackground = XpfControl.Surface.Background;
			if (control.SurfaceBorder != null)
				this.cachedBorderBackground = control.SurfaceBorder.Background;
			else
				this.cachedBorderBackground = null;
			UpdateBackground();
			control.OnViewPaddingChanged();
			base.Activate(viewBounds);
		}
		protected internal override void Deactivate() {
			XpfControl.Surface.Background = cachedBackground;
			XpfControl.SurfaceBorder.Background = cachedBorderBackground;
			base.Deactivate();
		}
		public void UpdateBackground() {
			RichEditControl control = (RichEditControl)Control;
			PlatformBrush brush = XpfRichEditViewAdapter.GetPageBackgroundColor(this);
			XpfControl.Surface.Background = brush;
			if (control.SurfaceBorder != null)
				control.SurfaceBorder.Background = brush;
		}
		protected internal override Rectangle CalculateVisiblePageBounds(Rectangle pageBounds, PageViewInfo pageViewInfo) {
			return Rectangle.Empty;
		}
	}
	public class XpfDraftView : DraftView, IXpfRichEditView {
		readonly XpfRichEditViewAdapter adapter;
		PlatformBrush cachedBackground;
		PlatformBrush cachedBorderBackground;
		public XpfDraftView(IRichEditControl control)
			: base(control) {
			this.adapter = new XpfRichEditViewAdapter(this);
		}
		public XpfRichEditViewAdapter Adapter { get { return adapter; } }
		RichEditControl XpfControl { get { return (RichEditControl)Control; } }
		protected internal override Padding ActualPadding {
			get {
				Padding result = Padding;
				HorizontalRulerControl horizontalRuler = Adapter.Control.HorizontalRuler;
				if (horizontalRuler != null && horizontalRuler.InnerIsVisible) {
					int offset = (horizontalRuler as IRulerControl).GetRulerSizeInPixels();
					if (Adapter.Control.VerticalRuler != null && Adapter.Control.VerticalRuler.InnerIsVisible)
						result.Left = Math.Max(result.Left - offset, offset / 3);
					else
						result.Left = Math.Max(result.Left, 4 * offset / 3);
				}
				return result;
			}
		}
		protected internal override void Activate(Rectangle viewBounds) {
			RichEditControl control = (RichEditControl)Control;
			this.cachedBackground = XpfControl.Surface.Background;
			if (control.SurfaceBorder != null)
				this.cachedBorderBackground = control.SurfaceBorder.Background;
			else
				this.cachedBorderBackground = null;
			UpdateBackground();
			control.OnViewPaddingChanged();
			base.Activate(viewBounds);
		}
		protected internal override void Deactivate() {
			XpfControl.Surface.Background = cachedBackground;
			XpfControl.SurfaceBorder.Background = cachedBorderBackground;
			base.Deactivate();
		}
		public void UpdateBackground() {
			RichEditControl control = (RichEditControl)Control;
			PlatformBrush brush = XpfRichEditViewAdapter.GetPageBackgroundColor(this);
			XpfControl.Surface.Background = brush;
			if (control.SurfaceBorder != null)
				control.SurfaceBorder.Background = brush;
		}
		protected internal override Rectangle CalculateVisiblePageBounds(Rectangle pageBounds, PageViewInfo pageViewInfo) {
			return Rectangle.Empty;
		}
	}
	public class XpfPrintLayoutView : PrintLayoutView, IXpfRichEditView {
		readonly XpfRichEditViewAdapter adapter;
		public XpfPrintLayoutView(IRichEditControl control)
			: base(control) {
			this.adapter = new XpfRichEditViewAdapter(this);
		}
		public XpfRichEditViewAdapter Adapter { get { return adapter; } }
		protected internal override void Activate(Rectangle viewBounds) {
			RichEditControl control = (RichEditControl)Control;
			control.OnViewPaddingChanged();
			base.Activate(viewBounds);
		}
		public void UpdateBackground() {
		}
	}
	public class XpfReadingLayoutView : ReadingLayoutView, IXpfRichEditView {
		readonly XpfRichEditViewAdapter adapter;
		public XpfReadingLayoutView(IRichEditControl control)
			: base(control) {
			this.adapter = new XpfRichEditViewAdapter(this);
		}
		public XpfRichEditViewAdapter Adapter { get { return adapter; } }
		public void UpdateBackground() {
		}
	}
	public class XpfRichEditViewPageFactory : IRichEditViewVisitor {
		RichEditViewPage page;
		public RichEditViewPage CreatePage(RichEditView view) {
			page = null;
			view.Visit(this);
			return page;
		}
		#region IRichEditViewVisitor Members
		void IRichEditViewVisitor.Visit(ReadingLayoutView view) {
			page = new RichEditViewPage();
		}
		void IRichEditViewVisitor.Visit(PrintLayoutView view) {
			page = new RichEditViewPage();
		}
		void IRichEditViewVisitor.Visit(DraftView view) {
			page = new DraftViewPage();
		}
		void IRichEditViewVisitor.Visit(SimpleView view) {
			page = new SimpleViewPage();
		}
		#endregion
	}
	public class XpfRichEditViewAdapter {
		readonly RichEditView view;
		readonly Dictionary<int, CacheElement> pageElementCache = new Dictionary<int, CacheElement>();
		public XpfRichEditViewAdapter(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public Dictionary<int, CacheElement> PageElementCache { get { return pageElementCache; } }
		public RichEditView View { get { return view; } }
		public RichEditControl Control { get { return (RichEditControl)View.Control; } }
		void RefreshSelection() {
			DeleteSelection();
			CreateSelection();
			CreateCaret();
		}
		public int FirstPageNumber {
			get {
				PageViewInfoCollection pageViewInfos = View.PageViewInfos;
				if (pageViewInfos.Count <= 0)
					return 1;
				if (pageViewInfos.Count == 1)
					return pageViewInfos[0].Index + 1;
				PageViewInfoGenerator generator = View.PageViewInfoGenerator;
				if (generator == null)
					return pageViewInfos[0].Index + 1;
				int firstHeight = generator.CalculatePageVisibleLogicalHeight(pageViewInfos[0], generator.ViewPortBounds);
				int secondHeight = generator.CalculatePageVisibleLogicalHeight(pageViewInfos[1], generator.ViewPortBounds);
				if (firstHeight > secondHeight)
					return pageViewInfos[0].Index + 1;
				else
					return pageViewInfos[1].Index + 1;
			}
		}
		public virtual void Refresh(RefreshAction action, ICustomMarkExporter customMarkExporter) {
			DateTime dt = DateTime.Now;
			if ((action & RefreshAction.AllDocument) != 0 || (action & RefreshAction.Zoom) != 0) {
				foreach (var p in pageElementCache) { p.Value.DoUpdate = true; }
				UpdatePageElements(customMarkExporter);
			}
			else if ((action & RefreshAction.Transforms) != 0 || (action & RefreshAction.Selection) != 0) {
				UpdatePageElements(customMarkExporter);
			}
			if ((action & RefreshAction.Transforms) != 0 || (action & RefreshAction.Zoom) != 0) {
				if (Control.HorizontalRuler != null)
					Control.HorizontalRuler.Repaint();
				if (Control.VerticalRuler != null)
					Control.VerticalRuler.Repaint();
			}
			RefreshSelection();
			System.Diagnostics.Debug.WriteLine("Refresh Action: {0}, Time: {1}\n", action, DateTime.Now - dt);
		}
		internal static PlatformBrush GetPageBackgroundColor(RichEditView view) {
			DocumentProperties properties = view.DocumentModel.DocumentProperties;
			PlatformIndependentColor pageBackColor = properties.PageBackColor;
			XpfSimpleView simpleView = view as XpfSimpleView;
			bool showPageBackground = simpleView != null ? true : properties.DisplayBackgroundShape;
			if (showPageBackground && !DXColor.IsEmpty(pageBackColor))
				return new SolidColorBrush(XpfTypeConverter.ToPlatformColor(pageBackColor));
			return GetPageBackgroundColorCore(view);
		}
		internal static PlatformBrush GetPageBackgroundColorCore(RichEditView view) {
			if (DXColor.IsEmpty(view.ActualBackColor))
				return new SolidColorBrush(Colors.White);
			return new SolidColorBrush(XpfTypeConverter.ToPlatformColor(view.ActualBackColor));
		}
		void UpdatePageElements(ICustomMarkExporter customMarkExporter) {
			IXpfRichEditView xpfView = View as IXpfRichEditView;
			if (xpfView != null)
				xpfView.UpdateBackground();
			int count = View.PageViewInfos.Count;
			Dictionary<RichEditViewPage, object> newList = new Dictionary<RichEditViewPage, object>();
			UIElementCollection children = Control.Surface.Children;
			for (int i = 0; i < count; i++) {
				RichEditViewPage pageElement = GetPageElement(View.PageViewInfos[i], customMarkExporter);
				if (pageElement.Parent == null)
					children.Add(pageElement);
				Rect bounds = CalculateActualPageBounds(View.PageViewInfos[i].ClientBounds);
				RichEditLayoutPage page = View.PageViewInfos[i].Page;
				pageElement.Background = GetPageBackgroundColor(View);
				SetPageTransform(pageElement, bounds, page);
				newList.Add(pageElement, null);
				if (pageElement.IsReady)
					ExportCustomMark(pageElement, customMarkExporter);
			}
			for (int i = children.Count - 1; i >= 0; i--) {
				RichEditViewPage pageElement = children[i] as RichEditViewPage;
				if (pageElement != null && !newList.ContainsKey(pageElement))
					children.RemoveAt(i);
			}
		}
		Rect SetClippingCore(UIElement visualRoot, Rect bounds) {
			GeneralTransform transform = TransformToVisual(Control.Surface, visualRoot);
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			rectangleGeometry.Rect = transform.TransformBounds(bounds);
			visualRoot.Clip = rectangleGeometry;
			return bounds;
		}
		RichEditViewPage GetPageElement(PageViewInfo pageInfo, ICustomMarkExporter customMarkExporter) {
			if (PageElementCache.ContainsKey(pageInfo.Index)) {
				CacheElement elem = PageElementCache[pageInfo.Index];
				if ((elem.Page == pageInfo.Page && !elem.DoUpdate) && !ShouldRenewNumPages(elem)) {
					return elem.AgPage;
				}
				return CreatePageElement(pageInfo, elem.AgPage, customMarkExporter);
			}
			else
				return CreatePageElement(pageInfo, customMarkExporter);
		}
		bool ShouldRenewNumPages(CacheElement elem) {
			return elem.NumPages != elem.Page.NumPages && elem.AgPage.Parent == null;
		}
		private RichEditViewPage CreatePageElement(PageViewInfo pageInfo, RichEditViewPage pageElement, ICustomMarkExporter customMarkExporter) {
			Rect rect = CalculateActualPageBounds(pageInfo.ClientBounds);
			RichEditLayoutPage page = pageInfo.Page;
			SetPageTransform(pageElement, rect, page);
			SafeExportTo(pageInfo, pageElement, customMarkExporter);
			CacheElement cachedPage = new CacheElement(pageElement, pageInfo.Page);
			cachedPage.NumPages = pageInfo.Page.NumPages;
			PageElementCache[pageInfo.Index] = cachedPage;
			return pageElement;
		}
		void SafeExportTo(PageViewInfo pageInfo, RichEditViewPage pageElement, ICustomMarkExporter customMarkExporter) {
			if (pageElement.IsReady)
				ExportTo(pageInfo, pageElement, customMarkExporter);
			else {
				DeferredPageExporter exporter = new DeferredPageExporter();
				exporter.ExportTo(this, pageInfo, pageElement, customMarkExporter);
			}
		}
		void CreateSelection() {
			int count = View.PageViewInfos.Count;
			View.SelectionLayout.Update();
			CommentSelectionLayout layout = View.SelectionLayout as CommentSelectionLayout;
			if (layout != null) {
				for (int i = 0; i < count; i++) {
					PageViewInfo pageViewInfo = View.PageViewInfos[i];
					CommentViewInfoHelper helper = new CommentViewInfoHelper();
					CommentViewInfo commentViewInfo = helper.FindCommentViewInfo(pageViewInfo.Page, layout.PieceTable);
					if (commentViewInfo != null) {
						XpfCommentSelectionPainter commentPainter = new XpfCommentSelectionPainter(Control.Surface, View.DocumentModel, View.ZoomFactor,
							View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth(), CalculateActualValue(commentViewInfo.ContentBounds.X), CalculateActualValue(commentViewInfo.ContentBounds.Y));
						DrawSelections(pageViewInfo, commentPainter);
					}
				}
			}
			else {
				XpfSelectionPainter painter = new XpfSelectionPainter(Control.Surface, View.DocumentModel, View.ZoomFactor, View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth());
				for (int i = 0; i < count; i++) {
					PageViewInfo page = View.PageViewInfos[i];
					DrawSelections(page, painter);
				}
				painter.Selection.Recalculate();
			}
		}
		void DrawSelections(PageViewInfo pageViewInfo, XpfSelectionPainter painter) {
			PageSelectionLayoutsCollection selections = View.SelectionLayout.GetPageSelection(pageViewInfo.Page);
			if (selections != null) {
				painter.Info = pageViewInfo;
				for (int selectionIndex = 0; selectionIndex < selections.Count; selectionIndex++) {
					selections[selectionIndex].Draw(painter);
				}
			}
		}
		void DeleteSelection() {
			UIElementCollection surfaceChildren = Control.Surface.Children;
			for (int i = surfaceChildren.Count - 1; i >= 0; i--) {
				FrameworkElement elem = surfaceChildren[i] as FrameworkElement;
				if (elem is IXpfRichEditSelection || elem is XpfRichEditCaret)
					surfaceChildren.RemoveAt(i);
			}
		}
		protected virtual void SetPageTransform(RichEditViewPage page, Rect bounds, RichEditLayoutPage layoutPage) {
			page.Width = bounds.Width;
			page.Height = bounds.Height;
			page.SetValue(Canvas.LeftProperty, bounds.Left);
			page.SetValue(Canvas.TopProperty, bounds.Top);
			if ((View.DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible) && (layoutPage.Comments.Count > 0)) {
				SetPageCommentsProperties(page, bounds, layoutPage);
			}
			else
				page.CommentsVisibility = SetCommentsVisibility(View.DocumentModel.CommentOptions.Visibility, layoutPage);
			if (page.SuperRoot == null || page.CommentRoot == null) {
				page.ApplyTemplate();
				if (page.SuperRoot == null) return;
			}
			page.SuperRoot.RenderTransform = new ScaleTransform() { ScaleX = View.ZoomFactor, ScaleY = View.ZoomFactor };
			if (page.CommentRoot != null) {
				page.CommentRoot.RenderTransform = CreateTransformGroup(-page.CommentsLeft / View.ZoomFactor, -page.CommentsTop / View.ZoomFactor);
			}
		}
		void SetPageCommentsProperties(RichEditViewPage page, Rect bounds, RichEditLayoutPage layoutPage) {
			page.CommentsHeight = CalculateActualValue(layoutPage.CommentBounds.Height) * View.ZoomFactor;
			page.CommentsWidth = CalculateActualValue(layoutPage.CommentBounds.Width) * View.ZoomFactor;
			page.CommentsTop = CalculateActualValue(layoutPage.CommentBounds.Top) * View.ZoomFactor;
			page.CommentsLeft = page.Width - page.CommentsWidth;
			page.CommentsVisibility = SetCommentsVisibility(View.DocumentModel.CommentOptions.Visibility, layoutPage);
		}
		Visibility SetCommentsVisibility(RichEditCommentVisibility commentsVisibility, RichEditLayoutPage layoutPage) {
			if ((commentsVisibility == RichEditCommentVisibility.Visible) && (layoutPage.Comments.Count > 0))
				return Visibility.Visible;
#if SL
			return Visibility.Collapsed;
#else
			return Visibility.Hidden;
#endif
		}
		double CalculateActualValue(int value) {
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			return unitConverter.LayoutUnitsToPixelsF(value);
		}
		Rect CalculateActualPageBounds(Rectangle bounds) {
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			Rect result = new Rect();
			result.Width = unitConverter.LayoutUnitsToPixelsF(bounds.Width);
			result.Height = unitConverter.LayoutUnitsToPixelsF(bounds.Height);
			result.X = (double)unitConverter.LayoutUnitsToPixels(bounds.Left - View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth());
			result.Y = (double)unitConverter.LayoutUnitsToPixelsF(bounds.Top);
			return result;
		}
		protected internal virtual RichEditViewPage GetNewPage() {
			XpfRichEditViewPageFactory factory = new XpfRichEditViewPageFactory();
			return factory.CreatePage(View);
		}
		protected virtual RichEditViewPage CreatePageElement(PageViewInfo pageInfo, ICustomMarkExporter customMarkExporter) {
			RichEditViewPage pageElement = GetNewPage();
			Rect rect = CalculateActualPageBounds(pageInfo.ClientBounds);
			RichEditLayoutPage page = pageInfo.Page;
			SetPageTransform(pageElement, rect, page);
			SafeExportTo(pageInfo, pageElement, customMarkExporter);
			CacheElement cachedPage = new CacheElement(pageElement, pageInfo.Page);
			cachedPage.NumPages = pageInfo.Page.NumPages;
			PageElementCache.Add(pageInfo.Index, cachedPage);
			return pageElement;
		}
		protected internal virtual void ExportTo(PageViewInfo pageInfo, RichEditViewPage p, ICustomMarkExporter customMarkExporter) {
			RichEditLayoutPage page = pageInfo.Page;
			int pageIndex = page.PageIndex;
			PageCollection pages = View.DocumentLayout.Pages;
			if (pageIndex < 0 || pageIndex >= pages.Count)
				return;
			if (!Object.ReferenceEquals(page, View.DocumentLayout.Pages[pageIndex]))
				page = View.DocumentLayout.Pages[pageIndex];
			Rectangle clipBounds = View.CalculatePageContentClipBounds(pageInfo);
			ClipPageContent(clipBounds, p);
			Painter painter = ((RichEditControl)Control).MeasurementAndDrawingStrategy.CreateDocumentPainter(p.DrawingSurface);
			painter.ClipBounds = clipBounds;
			XpfPainter xpfPainter = painter as XpfPainter;
			if (xpfPainter != null) {
				xpfPainter.ZoomFactor = View.ZoomFactor;
				xpfPainter.ImageSourceCache = Control.ImageSourceCache;
			}
			DocumentLayoutExporter exporter = CreateExporter(painter);
			exporter.SetBackColor(View.ActualBackColor, pageInfo.Bounds);
			DevExpress.XtraRichEdit.API.Layout.PagePainter pagePainter = new XtraRichEdit.API.Layout.PagePainter();
			DevExpress.XtraRichEdit.API.Layout.LayoutPage layoutPage = Control.DocumentLayout.GetPage(pageIndex);
			pagePainter.Initialize(exporter, layoutPage);
			pagePainter.DrawCommentsManually = true;
			BeforePagePaintEventArgs args = new BeforePagePaintEventArgs(pagePainter, layoutPage, exporter, XtraRichEdit.API.Layout.CanvasOwnerType.Control);
			Control.InnerControl.RaiseBeforePagePaint(args);
			if (args.Painter == null)
				return;
			pagePainter = args.Painter;
			pagePainter.DrawCommentsManually = true;
			pagePainter.Draw();
			DocumentLayoutExporter notPrintableExporter = new XpfNotPrintableGraphicsBoxExporter(View.DocumentModel, View, painter, customMarkExporter);
			DevExpress.XtraRichEdit.API.Layout.PagePainter notPrintablePagePainter = new XtraRichEdit.API.Layout.PagePainter(true);
			notPrintablePagePainter.Initialize(notPrintableExporter, layoutPage);
			notPrintablePagePainter.Draw();
			int count = pageInfo.Page.Comments.Count;
			if (p.CommentChildren != null) {
				int countCommentChildren = p.CommentChildren.Count;
				RichEditCommentCollection commentCollection = new RichEditCommentCollection();
				commentCollection.CommentChildren = p.CommentChildren;
				for (int i = 0; i < count; i++) {
					CommentViewInfo commentViewInfo = pageInfo.Page.Comments[i];
					RichEditComment comment = CreateRichEditComment(commentCollection, p, i);
					comment.MoreButtonCommand = new ShowReviewPaneCommand(Control, commentViewInfo);
					if (comment.DrawingSurface == null)
						comment.ApplyTemplate();
					SetCommentProperties(comment, commentViewInfo);
					SetMoreButtonProperties(comment, commentViewInfo);
					Painter commentPainter = ((RichEditControl)Control).MeasurementAndDrawingStrategy.CreateDocumentPainter(comment.DrawingSurface);
					comment.RootRenderTransform = new TranslateTransform(CalculateActualValue(-commentViewInfo.Bounds.X), CalculateActualValue(-commentViewInfo.Bounds.Y));
					commentPainter.DrawString(commentViewInfo.CommentHeading, commentViewInfo.CommentHeadingFontInfo, DXColor.Black, commentViewInfo.CommentHeadingBounds);
					if (commentViewInfo.Comment.ParentComment == null)
						DrawCommentLine(commentPainter, commentViewInfo, pageInfo.Page);
					commentPainter.ApplyClipBounds(commentViewInfo.ContentBounds);
					if (!commentViewInfo.CommentContainTableInFirstRow) {
						DocumentLayoutExporter commentExporter = CreateCommentExporter(commentPainter, commentViewInfo.ContentBounds);
						commentExporter.SetBackColor(View.ActualBackColor, commentViewInfo.Bounds);
						pagePainter.Initialize(commentExporter, layoutPage);
						pagePainter.DrawCommentManually(layoutPage.Comments[i]);
						commentExporter.FinishExport();
						DocumentLayoutExporter notPrintableCommentExporter = new XpfNotPrintableGraphicsBoxExporter(View.DocumentModel, View, commentPainter, customMarkExporter, commentViewInfo.ContentBounds.Location);
						DevExpress.XtraRichEdit.API.Layout.PagePainter notPrintableCommentPainter = new XtraRichEdit.API.Layout.PagePainter(true);
						notPrintableCommentPainter.Initialize(notPrintableCommentExporter, layoutPage);
						notPrintableCommentPainter.DrawCommentManually(layoutPage.Comments[i]);
					}
				}
				if (count < countCommentChildren) {
					for (int i = (countCommentChildren - 1); i >= count; i--)
						p.CommentChildren.RemoveAt(i);
				}
			}
			exporter.FinishExport();
		}
#if SL
		TranslateTransform CreateTransform(double x, double y) {
			TranslateTransform result = new TranslateTransform();
			result.X = x;
			result.Y = y;
			return result;
		}
#endif
		TransformGroup CreateTransformGroup(double offsetX, double offsetY) {
			TransformGroup result = new TransformGroup();
			TranslateTransform translateTransform = new TranslateTransform(offsetX, offsetY);
			ScaleTransform scaleTransform = new ScaleTransform() { ScaleX = View.ZoomFactor, ScaleY = View.ZoomFactor };
			result.Children.Add(translateTransform);
			result.Children.Add(scaleTransform);
			return result;
		}
		RichEditComment CreateRichEditComment(RichEditCommentCollection commentCollection, RichEditViewPage p, int indexComment) {
			int countComment = commentCollection.CommentChildren.Count;
			if (indexComment >= countComment) {
				RichEditComment result = new RichEditComment();
				if (p.CommentChildren != null)
					p.CommentChildren.Add(result);
				return result;
			}
			else
				return (RichEditComment)commentCollection.CommentChildren[indexComment];
		}
		void SetCommentProperties(RichEditComment comment, CommentViewInfo commentViewInfo) {
			comment.Width = CalculateActualValue(commentViewInfo.Bounds.Width);
			comment.Height = CalculateActualValue(commentViewInfo.Bounds.Height);
#if !SL
			comment.RenderTransform = new TranslateTransform(CalculateActualValue(commentViewInfo.Bounds.X), CalculateActualValue(commentViewInfo.Bounds.Y));
#else
			comment.RenderTransform = CreateTransform(CalculateActualValue(commentViewInfo.Bounds.X), CalculateActualValue(commentViewInfo.Bounds.Y));
#endif
			PlatformIndependentColor commentColor = View.DocumentModel.CommentColorer.GetColor(commentViewInfo.Comment);
			System.Windows.Media.Color commentMediaColor = System.Windows.Media.Color.FromArgb(commentColor.A, commentColor.R, commentColor.G, commentColor.B);
			comment.Background = new SolidColorBrush(commentMediaColor);
			comment.HorizontalAlignment = HorizontalAlignment.Left;
		}
		void SetMoreButtonProperties(RichEditComment comment, CommentViewInfo commentViewInfo) {
			if (commentViewInfo.ActualSize > commentViewInfo.Bounds.Height)
				comment.MoreButtonVisibility = Visibility.Visible;
			else
#if !SL
				comment.MoreButtonVisibility = Visibility.Hidden;
#else 
				comment.MoreButtonVisibility = Visibility.Collapsed;
#endif
		}
		void DrawCommentLine(Painter painter, CommentViewInfo commentViewInfo, DevExpress.XtraRichEdit.Layout.Page page) {
			Rectangle tightBounds = commentViewInfo.Character.TightBounds;
			Rectangle pageCommentBounds = page.CommentBounds;
			Rectangle commentViewInfoBounds = commentViewInfo.Bounds;
			float x1 = tightBounds.Left;
			float y1 = tightBounds.Bottom;
			float x2 = pageCommentBounds.Left;
			float y2 = y1;
			PlatformIndependentColor fillColor = View.DocumentModel.CommentColorer.GetColor(commentViewInfo.Comment);
			PlatformIndependentColor borderColor = CommentOptions.SetBorderColor(fillColor);
#if !SL
			System.Drawing.Pen pen = new System.Drawing.Pen(borderColor);
#else
			Pen pen = new Pen(borderColor);
#endif
			painter.DrawLine(pen, x1, y1, x2, y2);
			x1 = x2;
			int radius = 5;
			x2 = (commentViewInfoBounds.Left + (int)Control.LineOffset.X);
			y2 = (commentViewInfoBounds.Top + radius + (int)Control.LineOffset.Y);
			painter.PushPixelOffsetMode(true);
			painter.PushSmoothingMode(true);
			painter.DrawLine(pen, x1, y1, x2, y2);
			painter.PopPixelOffsetMode();
			painter.PopSmoothingMode();
		}
		protected virtual DocumentLayoutExporter CreateCommentExporter(Painter painter, Rectangle bounds) {
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			int minReadableTextHeight = (int)Math.Round(unitConverter.PixelsToLayoutUnitsF(6) / View.ZoomFactor);
			GraphicsDocumentLayoutExporterAdapter adapter = new XpfGraphicsDocumentLayoutExporterAdapter();
			DocumentLayoutExporter exporter = View.CreateDocumentLayoutExporter(painter, adapter, null, bounds);
			exporter.MinReadableTextHeight = minReadableTextHeight;
			exporter.ShowWhitespace = Control.InnerControl.Options.ShowHiddenText;
			return exporter;
		}
		protected internal virtual void ClipPageContent(Rectangle clipBounds, RichEditViewPage p) {
			if (p == null || p.Root == null)
				return;
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			Rect rect = new Rect();
			rect.X = unitConverter.LayoutUnitsToPixelsF(clipBounds.X);
			rect.Y = unitConverter.LayoutUnitsToPixelsF(clipBounds.Y);
			rect.Width = unitConverter.LayoutUnitsToPixelsF(clipBounds.Width);
#if SL
			rect.Width = Math.Min(rect.Width, Control.MaxClippingValue / this.View.ZoomFactor);
#endif
			rect.Height = unitConverter.LayoutUnitsToPixelsF(clipBounds.Height);
#if !SL
			int maxClipping = (int)Math.Truncate(500000 / Math.Max(1, this.View.ZoomFactor));
			rect.Width = Math.Min(clipBounds.Width, maxClipping);
			rect.Height = Math.Min(clipBounds.Height, maxClipping);
#endif
			RectangleGeometry clipGeometry = new RectangleGeometry();
			clipGeometry.Rect = rect;
			p.SuperRoot.Dispatcher.BeginInvoke(new Action(() => {
				if (p.SuperRoot != null)
					p.SuperRoot.Clip = clipGeometry;
			}));
		}
		protected virtual void ExportCustomMark(RichEditViewPage p, ICustomMarkExporter customMarkExporter) {
			if (p.DrawingSurface == null)
				return;
			CustomMarkVisualInfoCollection visualInfoCollection = p.DrawingSurface.CustomMarkVisualInfoCollection;
			int count = visualInfoCollection.Count;
			if (count == 0)
				return;
			RichEditControl richEditControl = View.Control as RichEditControl;
			if (richEditControl == null)
				return;
			GeneralTransform transform = TransformToVisual(p, richEditControl);
			for (int i = 0; i < count; i++) {
				CustomMarkVisualInfo info = visualInfoCollection[i];
				Rectangle bounds = info.Bounds;
				Rect initialBounds = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				Rect r2 = p.SuperRoot.RenderTransform.TransformBounds(initialBounds);
				Rect r = transform.TransformBounds(r2);
				customMarkExporter.ExportCustomMarkBox(info.CustomMark, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
			}
		}
		GeneralTransform TransformToVisual(UIElement from, UIElement to) {
			try {
				from.UpdateLayout();
				return from.TransformToVisual(to);
			}
			catch {
				TranslateTransform result = new TranslateTransform();
				result.X = 0;
				result.Y = 0;
				return result;
			}
		}
		protected virtual DocumentLayoutExporter CreateExporter(Painter painter) {
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			int minReadableTextHeight = (int)Math.Round(unitConverter.PixelsToLayoutUnitsF(6) / View.ZoomFactor);
			GraphicsDocumentLayoutExporterAdapter adapter = new XpfGraphicsDocumentLayoutExporterAdapter();
			DocumentLayoutExporter exporter = View.CreateDocumentLayoutExporter(painter, adapter, null, new Rectangle(0, 0, int.MaxValue, int.MaxValue));
			exporter.MinReadableTextHeight = minReadableTextHeight;
			exporter.ShowWhitespace = Control.InnerControl.Options.ShowHiddenText;
			return exporter;
		}
		protected void CreateCaret() {
			XpfRichEditCaret caret = new XpfRichEditCaret();
			if (!View.CaretPosition.Update(DocumentLayoutDetailsLevel.Character))
				return;
			if (View.CaretPosition.PageViewInfo == null)
				return;
			if (!Control.IsCaretVisible || !Control.InnerIsFocused)
				return;
			if (Control.ReadOnly && !Control.ShowCaretInReadOnly)
				return;
			if (View.DocumentModel.Selection.Length > 0)
				return;
			Rectangle pageViewInfoCaretBounds = View.CaretPosition.PageViewInfo.ClientBounds;
			var pos = View.CaretPosition.CalculateCaretBounds();
			pos.Y = (int)(pos.Y * View.ZoomFactor);
			pos.X = (int)(pos.X * View.ZoomFactor);
			pos.X = (int)(pos.X - View.HorizontalScrollController.GetPhysicalLeftInvisibleWidth());
			pos.Y = (int)(pos.Y + pageViewInfoCaretBounds.Y);
			pos.X = (int)(pos.X + pageViewInfoCaretBounds.X);
			DocumentLayoutUnitConverter unitConverter = View.DocumentModel.LayoutUnitConverter;
			caret.SetValue(Canvas.TopProperty, (double)unitConverter.LayoutUnitsToPixelsF(pos.Y));
			caret.SetValue(Canvas.LeftProperty, (double)unitConverter.LayoutUnitsToPixelsF(pos.X));
			caret.Width = unitConverter.LayoutUnitsToPixelsF(pos.Width * View.ZoomFactor);
			caret.Height = unitConverter.LayoutUnitsToPixelsF(pos.Height * View.ZoomFactor);
			Control.Surface.Children.Add(caret);
		}
	}
	#region DeferredPageExporter
	public class DeferredPageExporter {
		XpfRichEditViewAdapter adapter;
		PageViewInfo pageInfo;
		RichEditViewPage pageElement;
		ICustomMarkExporter customMarkExporter;
		public DeferredPageExporter() {
		}
		public void ExportTo(XpfRichEditViewAdapter adapter, PageViewInfo pageInfo, RichEditViewPage pageElement, ICustomMarkExporter customMarkExporter) {
			this.adapter = adapter;
			this.pageInfo = pageInfo;
			this.pageElement = pageElement;
			this.customMarkExporter = customMarkExporter;
			pageElement.PageReady += OnPageReady;
		}
		void OnPageReady(object sender, EventArgs e) {
			pageElement.PageReady -= OnPageReady;
			adapter.ExportTo(pageInfo, pageElement, customMarkExporter);
			this.adapter = null;
			this.pageInfo = null;
			this.pageElement = null;
			this.customMarkExporter = null;
		}
	}
	#endregion
	public class RichEditCommentCollection {
		UIElementCollection commentChildren;
		public UIElementCollection CommentChildren { get { return commentChildren; } set { commentChildren = value; } }
	}
}
