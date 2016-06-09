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
using DevExpress.Pdf;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Drawing;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfNavigationStrategy : NavigationStrategy {
		PdfPresenterControl DocumentPresenter { get { return Presenter as PdfPresenterControl; } }
		internal new PositionCalculator PositionCalculator { get { return base.PositionCalculator; } }
		public SelectionRectangle SelectionRectangle { get { return DocumentPresenter.SelectionRectangle; } }
		public PdfNavigationStrategy(PdfPresenterControl presenter) : base(presenter) { }
		public void ScrollIntoView(PdfTarget target) {
			if (target == null || !DocumentPresenter.HasPages)
				return;
			PdfTargetScroll targetScroll = target as PdfTargetScroll;
			if (targetScroll != null)  
				switch (targetScroll.AlignMode) {
					case PdfRectangleAlignMode.Center:
						ProcessTargetEnsureVisibility(target);
						break;
					case PdfRectangleAlignMode.Edge:
						ProcessTargetEnsureCaretVisibility(target);
						break;
				}
			else
				switch (target.Mode) {
					case PdfTargetMode.XYZ:
						ProcessTargetXYZ(target);
						break;
					case PdfTargetMode.Fit:
					case PdfTargetMode.FitBBox:
						ProcessTargetFit(target);
						break;
					case PdfTargetMode.FitRectangle:
						ProcessTargetFitRectangle(target);
						break;
					case PdfTargetMode.FitHorizontally:
					case PdfTargetMode.FitBBoxHorizontally:
						ProcessTargetFitHorizontally(target);
						break;
					case PdfTargetMode.FitVertically:
					case PdfTargetMode.FitBBoxVertically:
						ProcessTargetFitVertically(target);
						break;
				}
		}
		void ProcessTargetEnsureCaretVisibility(PdfTarget target) {
			var x = target.X.GetValueOrDefault();
			var y = target.Y.GetValueOrDefault();
			var caretRect = DocumentPresenter.CalcRect(target.PageIndex, new PdfPoint(x, y - target.Height), new PdfPoint(x + 1, y));
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(target.PageIndex);
			ScrollIntoView(pageWrapperIndex, caretRect, ScrollIntoViewMode.Edge);
		}
		void ProcessTargetFit(PdfTarget target) {
			ChangePosition(new NavigationState { Angle = BehaviorProvider.RotateAngle, PageIndex = target.PageIndex, ZoomFactor = CalcFitZoomFactor(target.PageIndex), OffsetX = 0, OffsetY = 0 });
		}
		double CalcFitZoomFactor(int pageIndex) {
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = Presenter.Pages.ElementAt(pageWrapperIndex);
			var pageSize = pageWrapper.RenderSize;
			var pageMargin = pageWrapper.CalcMarginSize();
			double diff = !UsePageHeight(pageSize) ? BehaviorProvider.Viewport.Width / (pageSize.Width - pageMargin.Width) : BehaviorProvider.Viewport.Height / (pageSize.Height - pageMargin.Height);
			return Math.Min(5d, Math.Max(0.1d, BehaviorProvider.ZoomFactor * diff));
		}
		bool UsePageHeight(Size pageSize) {
			var viewportProp = BehaviorProvider.Viewport.Width / BehaviorProvider.Viewport.Height;
			return pageSize.Width.LessThan(pageSize.Height * viewportProp);
		}
		void ProcessTargetFitRectangle(PdfTarget target) {
			int pageIndex = target.PageIndex;
			var x = target.X.GetValueOrDefault();
			var y = target.Y.GetValueOrDefault();
			var rect = DocumentPresenter.CalcRect(pageIndex, new PdfPoint(x, y - target.Height), new PdfPoint(x + target.Width, y));
			var zoomFactor = CalcFitRectangleZoomFactor(BehaviorProvider.Viewport, rect);
			var point = DocumentPresenter.CalcPoint(pageIndex, new PdfPoint(x, y));
			ScrollToAnchorPoint(zoomFactor, CalcHorizontalAnchorPointForFitRectangle(rect, point.X), CalcVerticalAnchorPointForFitRectangle(rect, point.Y), UndoActionType.Scroll);
		}
		void ProcessTargetFitHorizontally(PdfTarget target) {
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(target.PageIndex);
			var pageWrapper = Presenter.Pages.ElementAt(pageWrapperIndex);
			var pageSize = pageWrapper.PageSize;
			double zoomFactor = BehaviorProvider.Viewport.Width / pageSize.Width;
			int pageIndex = target.PageIndex;
			var anchorPoint = CalcAnchorPoint(0, target.Y, pageIndex);
			var targetOffset = CalcTargetOffset(anchorPoint, target.PageIndex);
			var newState = new NavigationState { Angle = BehaviorProvider.RotateAngle, PageIndex = target.PageIndex, ZoomFactor = zoomFactor, OffsetX = 0, OffsetY = targetOffset.Y };
			ChangePosition(newState);
		}
		void ProcessTargetFitVertically(PdfTarget target) {
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(target.PageIndex);
			var pageWrapper = Presenter.Pages.ElementAt(pageWrapperIndex);
			var pageSize = pageWrapper.RenderSize;
			var pageMargin = pageWrapper.CalcMarginSize();
			double zoomFactor = BehaviorProvider.Viewport.Height / (pageSize.Height - pageMargin.Height);
			int pageIndex = target.PageIndex;
			var anchorPoint = CalcAnchorPoint(target.X, 0, pageIndex);
			var targetOffset = CalcTargetOffset(anchorPoint, pageIndex);
			var newState = new NavigationState { Angle = BehaviorProvider.RotateAngle, PageIndex = target.PageIndex, ZoomFactor = zoomFactor, OffsetX = targetOffset.X, OffsetY = 0 };
			ChangePosition(newState);
		}
		Point CalcTargetOffset(PdfPoint pdfPoint, int pageIndex) {
			var point = DocumentPresenter.CalcPoint(pageIndex, pdfPoint);
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = (PdfPageWrapper)Presenter.Pages.ElementAt(pageWrapperIndex);
			var pageSize = pageWrapper.RenderSize;
			return new Point(point.X / pageSize.Width, point.Y / pageSize.Height);
		}
		Point CalcTargetOffset(double? x, double? y, int pageIndex) {
			var anchorPoint = CalcAnchorPoint(x, y, pageIndex);
			var offset = CalcTargetOffset(anchorPoint, pageIndex);
			double relativeYOffset = y.HasValue ? offset.Y : 0;
			double relativeXOffset = x.HasValue ? offset.X : 0;
			return new Point(relativeXOffset, relativeYOffset);
		}
		void ProcessTargetXYZ(PdfTarget target) {
			int pageIndex = target.PageIndex;
			var targetOffset = CalcTargetOffset(target.X, target.Y, pageIndex);
			var newState = new NavigationState { Angle = BehaviorProvider.RotateAngle, PageIndex = pageIndex, ZoomFactor = target.Zoom.GetValueOrDefault(BehaviorProvider.ZoomFactor), OffsetX = targetOffset.X, OffsetY = targetOffset.Y };
			ChangePosition(newState);
		}
		PdfPoint CalcAnchorPoint(double? x, double? y, int pageIndex) {
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = (PdfPageWrapper)Presenter.Pages.ElementAt(pageWrapperIndex);
			var topLeft = pageWrapper.CalcTopLeftAngle(Presenter.BehaviorProvider.RotateAngle, pageIndex);
			return new PdfPoint(x ?? topLeft.X, y ?? topLeft.Y);
		}
		void ProcessTargetEnsureVisibility(PdfTarget target) {
			int pageIndex = target.PageIndex;
			var x = target.X.GetValueOrDefault();
			var y = target.Y.GetValueOrDefault();
			var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			ScrollIntoView(pageWrapperIndex, DocumentPresenter.CalcRect(pageIndex, new PdfPoint(x, y), new PdfPoint(x + target.Width, y + target.Height)), ScrollIntoViewMode.Center);
		}
		PdfDocumentPosition CalcDocumentPositionInternal(Point point) {
			var presenterOffset = DocumentPresenter.GetPosition(DocumentPresenter.ActualPdfViewer);
			var hitTestPoint = new Point(point.X - presenterOffset.X, point.Y - presenterOffset.Y);
			return CalcDocumentPosition(hitTestPoint);
		}
		protected internal PdfDocumentPosition CalcDocumentPosition(Point point) {
			int pageIndex = Presenter.ShowSingleItem ? ItemsPanel.IndexCalculator.VerticalOffsetToIndex(ItemsPanel.GetVirtualVerticalOffset()) :
				PositionCalculator.GetPageIndex(point.Y + ScrollViewer.VerticalOffset, point.X + ScrollViewer.HorizontalOffset, ItemsPanel.CalcPageHorizontalOffset);
			int pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = DocumentPresenter.Pages.ElementAt(pageWrapperIndex);
			PdfPageViewModel page = pageWrapper.Pages.Single(x => x.PageIndex == pageIndex) as PdfPageViewModel;
			var pageRect = pageWrapper.GetPageRect(page);
			double pageOffsetY = PositionCalculator.GetPageVerticalOffset(pageIndex, 0d) + pageRect.Top;
			double pageOffsetX = ItemsPanel.CalcPageHorizontalOffset(pageWrapper.RenderSize.Width) + pageRect.Left;
			double relativePointY = Presenter.ShowSingleItem ? point.Y - ItemsPanel.CalcStartPosition(true) : point.Y + ScrollViewer.VerticalOffset - pageOffsetY;
			Point relativePoint = new Point(point.X - pageOffsetX, relativePointY);
			PdfPoint pdfPoint = page.GetPdfPoint(relativePoint, BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle);
			return new PdfDocumentPosition(pageIndex + 1, pdfPoint);
		}
		protected internal Point CalcViewerPosition(PdfDocumentPosition position) {
			int pageIndex = position.PageIndex;
			int pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = DocumentPresenter.Pages.ElementAt(pageWrapperIndex);
			PdfPageViewModel page = pageWrapper.Pages.Single(x => x.PageIndex == pageIndex) as PdfPageViewModel;
			double pageOffsetX = ItemsPanel.CalcPageHorizontalOffset(pageWrapper.RenderSize.Width) + page.Margin.Left;
			double pageOffsetY = PositionCalculator.GetPageVerticalOffset(pageIndex, 0d) + page.Margin.Top;
			Point inPagePoint = page.GetPoint(position.Point, BehaviorProvider.ZoomFactor, BehaviorProvider.RotateAngle);
			double relativePointY = Presenter.ShowSingleItem ? pageOffsetY + inPagePoint.Y + ItemsPanel.CalcStartPosition(true) : pageOffsetY + inPagePoint.Y - ScrollViewer.VerticalOffset;
			Point relativePoint = new Point(pageOffsetX + inPagePoint.X - ScrollViewer.HorizontalOffset, relativePointY);
			return relativePoint;
		}
		public PdfHitTestResult ProcessHitTest(Point point) {
			PdfDocumentPosition documentPosition = CalcDocumentPositionInternal(point);
			var hitTestResult = DocumentPresenter.Document.HitTest(documentPosition);
			return new PdfHitTestResult(hitTestResult.DocumentPosition, hitTestResult.ContentType, hitTestResult.IsSelected);
		}
		public Point ProcessConvertDocumentPosition(PdfDocumentPosition documentPosition) {
			Point presenterOffset = DocumentPresenter.GetPosition(DocumentPresenter.ActualPdfViewer);
			var relativePoint = CalcViewerPosition(documentPosition);
			return new Point(relativePoint.X + presenterOffset.X + ScrollViewer.HorizontalOffset, relativePoint.Y + presenterOffset.Y);
		}
		public PdfDocumentPosition ProcessConvertPoint(Point point) {
			return CalcDocumentPositionInternal(point);
		}
		public override void ProcessZoomChanged(ZoomChangedEventArgs e) {
			base.ProcessZoomChanged(e);
		}
		public override void ProcessRotateAngleChanged(RotateAngleChangedEventArgs e) {
			base.ProcessRotateAngleChanged(e);
			DocumentPresenter.Do(x => x.InvalidateRenderCaches());
		}
		protected override void UpdatePagesRotateAngleInternal(double rotateAngle) {
			base.UpdatePagesRotateAngleInternal(rotateAngle);
			DocumentPresenter.Do(x => x.InvalidateRenderCaches());
			DocumentPresenter.With(x => x.Document).Do(x => x.UpdateDocumentRotateAngle(rotateAngle));
		}
	}
}
