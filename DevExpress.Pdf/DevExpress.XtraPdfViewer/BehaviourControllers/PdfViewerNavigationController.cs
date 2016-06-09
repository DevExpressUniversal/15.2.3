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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfViewerNavigationController : IPdfViewerNavigationController {
		const int maxScrollDelta = 15;
		readonly PdfViewer viewer;
		readonly PdfDocumentViewer documentViewer;
		readonly PdfDocumentViewStateHistoryController historyController;
		public PdfDocumentViewStateHistoryController HistoryController { get { return historyController; } }
		int IPdfViewerNavigationController.CurrentPageNumber { get { return viewer.CurrentPageNumber; } }
		public PdfViewerNavigationController(PdfViewer viewer) {
			this.viewer = viewer;
			this.documentViewer = viewer.Viewer;
			this.historyController = new PdfDocumentViewStateHistoryController(viewer);
		}
		PdfPoint IPdfViewerNavigationController.GetClientPoint(PdfDocumentPosition position) {
			PointF point = documentViewer.DocumentToViewer(position);
			return new PdfPoint(point.X, point.Y);
		}
		void IPdfViewerNavigationController.ShowDocumentPosition(PdfTarget target) {
			viewer.ShowDocumentPosition(target);
		}
		void IPdfViewerNavigationController.ShowRectangleOnPage(PdfRectangleAlignMode alignMode, int pageNumber, PdfRectangle rectangleBounds) {
			viewer.ShowRectangleOnPage(alignMode, pageNumber, rectangleBounds);
		}
		void IPdfViewerNavigationController.Invalidate(PdfDocumentStateChangedFlags flags, int pageIndex) {
			if (flags.HasFlag(PdfDocumentStateChangedFlags.Rotate))
				historyController.PerformLockedOperation(() => {
					viewer.PageCache.Clear();
					viewer.CreatePageViews(viewer.Document, viewer.DocumentState);
					documentViewer.UpdateAll();				
				});
			documentViewer.InvalidateView();
		}
		void IPdfViewerNavigationController.BringCurrentSelectionPointIntoView() {
			RectangleF rect = documentViewer.ClientRectangle;
			PointF mousePosition = documentViewer.PointToClient(Control.MousePosition);
			int xScroll = 0;
			int yScroll = 0;
			double zoomFactor = 3.5 * viewer.ZoomFactor / 100;
			if (mousePosition.X < rect.Left)
				xScroll = (int)(-Math.Min(Math.Abs(mousePosition.X - rect.Left) / zoomFactor, maxScrollDelta));
			else if (mousePosition.X > rect.Right)
				xScroll = (int)Math.Min(Math.Abs(mousePosition.X - rect.Right) / zoomFactor, maxScrollDelta);
			if (mousePosition.Y < rect.Top)
				yScroll = (int)(-Math.Min(Math.Abs(mousePosition.Y - rect.Top) / zoomFactor, maxScrollDelta));
			else if (mousePosition.Y > rect.Bottom)
				yScroll = (int)Math.Min(Math.Abs(mousePosition.Y - rect.Bottom) / zoomFactor, maxScrollDelta);
			viewer.ScrollHorizontal(xScroll);
			viewer.ScrollVertical(yScroll);
		}
		void IPdfViewerNavigationController.GoToNextPage() {
			documentViewer.PerformPageNavigation(PdfPageNavigationMode.Next);
		}
		void IPdfViewerNavigationController.GoToPreviousPage() {
			documentViewer.PerformPageNavigation(PdfPageNavigationMode.Previous);
		}
		void IPdfViewerNavigationController.GoToFirstPage() {
			documentViewer.PerformPageNavigation(PdfPageNavigationMode.Home);
		}
		void IPdfViewerNavigationController.GoToLastPage() {
			documentViewer.PerformPageNavigation(PdfPageNavigationMode.Last);
		}
	}
}
