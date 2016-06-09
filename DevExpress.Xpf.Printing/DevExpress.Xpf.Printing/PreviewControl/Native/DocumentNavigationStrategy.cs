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

using DevExpress.Xpf.DocumentViewer;
using DevExpress.Mvvm.Native;
using System;
using System.Windows;
using System.Linq;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using PointF = System.Drawing.PointF;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class DocumentNavigationStrategy : NavigationStrategy {
		protected new DocumentPresenterControl Presenter { get { return base.Presenter as DocumentPresenterControl; } }
		PrintingSystemBase PrintingSystem {
			get { return ((DocumentViewModel)Presenter.Document).PrintingSystem; }
		}
		public DocumentNavigationStrategy(DocumentPresenterControl presenter) : base(presenter) { }
		public override void ProcessZoomChanged(ZoomChangedEventArgs e) {
			base.ProcessZoomChanged(e);
			Presenter.Do(x => x.InvalidateRenderCaches());
		}
		public override void ProcessScrollViewerScrollChanged(System.Windows.Controls.ScrollChangedEventArgs e) {
			base.ProcessScrollViewerScrollChanged(e);
			Presenter.InputController.Do(x => x.OnScrollChanged(e));
		}
		protected internal void ShowBrick(BrickPagePair brickPagePair) {
			if(brickPagePair.GetBrick(PrintingSystem.Pages) != null) {
				ShowBrickCore(brickPagePair);
			} else {
				(Presenter.Document as DocumentViewModel).Do(x => {
					x.EnsureBrickOnPage(brickPagePair, ShowBrickCore);
				});
			}
		}
		void ShowBrickCore(BrickPagePair brickPagePair) {
			var brickBounds = brickPagePair.GetBrickBounds(PrintingSystem.Pages);
			var brickPoint = PSUnitConverter.DocToPixel(new PointF(brickBounds.X, brickBounds.Y));
			int pageIndex = brickPagePair.PageIndex;
			var pageOffset = CalculatePageOffset(pageIndex);
			var offSetY = pageOffset.Y + brickPoint.Y * BehaviorProvider.ZoomFactor;
			if(Presenter.IsSearchControlVisible) {
				var bounds = GraphicsUnitConverter.Convert(brickBounds, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
				ScrollIntoView(pageIndex, new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height), ScrollIntoViewMode.Center);
			} else ScrollViewer.ScrollToVerticalOffset(offSetY);
		}
		protected internal Brick GetBrick(Point point) {
			var page = GetPage(point);
			var pageOffset = CalculatePageOffset(page.PageIndex);
			Point relativePoint = new Point(point.X - pageOffset.X, point.Y + ScrollViewer.VerticalOffset - pageOffset.Y);
			var brick = page.GetBrick(relativePoint, BehaviorProvider.ZoomFactor);
			return brick;
		}
		protected PageViewModel GetPage(Point point) {
			int pageIndex = PositionCalculator.GetPageIndex(point.Y + ScrollViewer.VerticalOffset, point.X + ScrollViewer.HorizontalOffset, ItemsPanel.CalcPageHorizontalOffset);
			int pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = Presenter.Pages.ElementAt(pageWrapperIndex);
			PageViewModel page = pageWrapper.Pages.Single(x => x.PageIndex == pageIndex) as PageViewModel;
			return page;
		}
		Point CalculatePageOffset(int pageIndex) {
			int pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
			var pageWrapper = Presenter.Pages.ElementAt(pageWrapperIndex);
			PageViewModel page = pageWrapper.Pages.Single(x => x.PageIndex == pageIndex) as PageViewModel;
			var pageRect = pageWrapper.GetPageRect(page);
			double pageOffsetY = PositionCalculator.GetPageVerticalOffset(pageIndex, 0d) + pageRect.Top;
			double pageOffsetX = ItemsPanel.CalcPageHorizontalOffset(pageWrapper.RenderSize.Width) + pageRect.Left;
			return new Point(pageOffsetX, pageOffsetY);
		}
		protected internal virtual void ProcessMouseMove(Point point) {
			var page = GetPage(point);
			if(page == null)
				return;
			var brick = GetBrick(point);
			Presenter.ActualDocumentViewer.RaiseDocumentPreiewMouseMove(new DocumentPreviewMouseEventArgs(page.PageIndex, brick));
		}
		protected internal virtual void ProcessMouseClick(Point point) {
			var page = GetPage(point);
			if(page == null)
				return;
			var brick = GetBrick(point);
			brick.Do(x=> ProcessNavigateBrickInternal(page.Page, x));
			Presenter.ActualDocumentViewer.RaiseDocumentPreiewMouseClick(new DocumentPreviewMouseEventArgs(page.PageIndex, brick));
		}
		void ProcessNavigateBrickInternal(Page page, Brick brick) {
			(brick as VisualBrick).Do(visual => {
				(Presenter.Document as ReportDocumentViewModel).Do(x=> x.HandleBrickClick(page, brick));
				BrickPagePair brickPagePair = visual == null ? BrickPagePair.Empty : visual.NavigationPair;
				if(brickPagePair != BrickPagePair.Empty) {
					ShowBrick(brickPagePair);
				} else if(!string.IsNullOrEmpty(brick.Url) && String.Compare(brick.Url, SR.BrickEmptyUrl, true) != 0)
					ProcessLaunchHelper.StartProcess(brick.Url);
			});
		}
		protected internal virtual void ProcessMouseDoubleClick(Point point) {
			var page = GetPage(point);
			if(page == null)
				return;
			var brick = GetBrick(point);
			brick.Do(x => ProcessNavigateBrickInternal(page.Page, x));
			Presenter.ActualDocumentViewer.RaiseDocumentPreiewMouseDoubleClick(new DocumentPreviewMouseEventArgs(page.PageIndex, brick));
		}
	}
}
