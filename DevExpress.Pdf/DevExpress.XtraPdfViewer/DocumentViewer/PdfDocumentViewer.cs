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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer.Interop;
namespace DevExpress.XtraPdfViewer.Native {
	public enum PdfPageNavigationMode { Previous, Next, Home, End, Last };
	[DXToolboxItem(false)]
	public class PdfDocumentViewer : DocumentViewerBase {
		const float documentDpi = 300.0f;
		const float dpiFactor = PdfRenderingCommandInterpreter.DpiFactor * 100;
		static float screenToDocumentFactorX;
		static float screenToDocumentFactorY;
		static float clientToDocumentFactorX;
		static float clientToDocumentFactorY;
		static float documentToViewerFactorX;
		static float documentToViewerFactorY;
		public static float DocumentToViewerFactorX { get { return documentToViewerFactorX; } }
		public static float DocumentToViewerFactorY { get { return documentToViewerFactorY; } }
		public static float ClientToDocumentFactorX { get { return clientToDocumentFactorX; } }
		public static float ClientToDocumentFactorY { get { return clientToDocumentFactorY; } }
		static PdfDocumentViewer() {
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
				float screenDpiX = graphics.DpiX;
				float screenDpiY = graphics.DpiY;
				screenToDocumentFactorX = documentDpi / screenDpiX;
				screenToDocumentFactorY = documentDpi / screenDpiY;
				clientToDocumentFactorX = screenToDocumentFactorX * PdfRenderingCommandInterpreter.DpiFactor;
				clientToDocumentFactorY = screenToDocumentFactorY * PdfRenderingCommandInterpreter.DpiFactor;
				documentToViewerFactorX = dpiFactor / screenDpiX;
				documentToViewerFactorY = dpiFactor / screenDpiY;
			}
		}
		static float GetScrollPosition(ScrollBarBase scrollBar, int dimension) {
			float scrollLength = scrollBar.Maximum + scrollBar.Minimum - dimension;
			return scrollLength == 0 ? 0 : (scrollBar.Value / scrollLength);
		}
		[SecuritySafeCritical]
		static Cursor CreateCursor(string fileName, int hotSpotX, int hotSpotY) {
			try {
				using (Bitmap bmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraPdfViewer.Images.Cursors." + fileName, Assembly.GetExecutingAssembly())) {
					IntPtr ptr = bmp.GetHicon();
					CursorInterop.IconInfo tmp = new CursorInterop.IconInfo();
					CursorInterop.GetIconInfo(ptr, ref tmp);
					tmp.xHotspot = hotSpotX;
					tmp.yHotspot = hotSpotY;
					tmp.fIcon = false;
					ptr = CursorInterop.CreateIconIndirect(ref tmp);
					return new Cursor(ptr);
				}
			}
			catch {
				return Cursors.Default;
			}
		}
		readonly PdfViewer viewer;
		readonly PdfCaretView caretView;
		PdfDocumentContent currentContent;
		Cursor contextCursor;
		Cursor crossCursor;
		Cursor ContextCursor {
			get { return contextCursor ?? (contextCursor = CreateCursor("ContextCursor.png", 1, 3)); }
		}
		Cursor CrossCursor {
			get { return crossCursor ?? (crossCursor = CreateCursor("CrossCursor.png", 15, 15)); }
		}
		Cursor ActualCursor {
			get {
				if (currentContent == null)
					return null;
				switch (currentContent.Cursor) {
					case PdfCursor.Context:
						return ContextCursor;
					case PdfCursor.Cross:
						return CrossCursor;
					case PdfCursor.Hand:
						return Cursors.Hand;
					case PdfCursor.IBeam:
						return Cursors.IBeam;
					case PdfCursor.Default:
						return Cursors.Default;
					default:
						throw new InvalidOperationException();
				}
			}
		}
		internal float VerticalScreenToDocumentFactor { get { return screenToDocumentFactorY / ZoomFactor; } }
		internal RectangleF VisibleRect {
			get {
				Size viewControlSize = ViewControl.Size;
				int y = viewer.FindToolWindowHeight;
				Rectangle clientRect = new Rectangle(0, y, viewControlSize.Width, viewControlSize.Height - y);
				return GetVisibleRect(clientRect);
			}
		}
		public PdfViewer Viewer { get { return viewer; } }
		public PdfCaretView CaretView { get { return caretView; } }
		public PdfZoomMode ZoomMode {
			get {
				switch (ViewMode) {
					case PageViewModes.RowColumn:
						return PdfZoomMode.PageLevel;
					case PageViewModes.PageWidth:
						return PdfZoomMode.FitToWidth;
					case PageViewModes.TextWidth:
						return PdfZoomMode.FitToVisible;
					default:
						return Zoom == 1 ? PdfZoomMode.ActualSize : PdfZoomMode.Custom;
				}
			}
		}
		public float ZoomFactor { get { return Zoom; } }
		public float HorizontalScrollPosition {
			get { return GetScrollPosition(hScrollBar, ViewControl.Width); }
			set {
				int position = CalculateScrollPosition(hScrollBar, ViewControl.Width, value);
				if (hScrollBar.Value != position)
					SetScrollHorizontal(position);
			}
		}
		public float VerticalScrollPosition {
			get { return GetScrollPosition(vScrollBar, ViewControl.Height); }
			set {
				int position = CalculateScrollPosition(vScrollBar, ViewControl.Height, value);
				if (vScrollBar.Value != position)
					SetScrollVertical(position);
			}
		}
		protected override bool CanHandleClick { get { return false; } }
		protected override bool CanSelectViewControl { get { return false; } }
		public event EventHandler FunctionalLimitsOccurred;
		public PdfDocumentViewer(PdfViewer viewer) {
			this.viewer = viewer;
			caretView = new PdfCaretView(viewer);
		}
		public void PerformPageNavigation(PdfPageNavigationMode mode) {
			viewer.HistoryController.PerformLockedOperation(() => {
				switch (mode) {
					case PdfPageNavigationMode.Next:
						SelectNextPage();
						break;
					case PdfPageNavigationMode.Previous:
						SelectPrevPage();
						break;
					case PdfPageNavigationMode.Home:
						SelectFirstPage();
						break;
					case PdfPageNavigationMode.End:
						ViewManager.SetVertScroll(float.MaxValue);
						viewer.HandleScrolling();
						break;
					case PdfPageNavigationMode.Last:
						SelectLastPage();
						break;
				}
			});
			viewer.RegisterCurrentDocumentViewState(PdfNavigationMode.Page);
		}
		public void ScrollVertical(int amount) {
			ViewManager.OffsetVertScroll(amount * screenToDocumentFactorY);
			ViewControl.Invalidate();
			UpdateScrollBars();
		}
		public void ScrollHorizontal(int amount) {
			ViewManager.OffsetHorzScroll(amount * screenToDocumentFactorX);
			ViewControl.Invalidate();
			UpdateScrollBars();
		}
		public void UpdateZoomMode(PdfZoomMode zoomMode) {
			switch (zoomMode) {
				case PdfZoomMode.ActualSize:
					Zoom = 1.0f;
					break;
				case PdfZoomMode.PageLevel:
					SetPageView(1, 1);
					break;
				case PdfZoomMode.FitToWidth:
					SetPageView(PageViewModes.PageWidth);
					break;
				case PdfZoomMode.FitToVisible:
					SetPageView(PageViewModes.TextWidth);
					break;
				default:
					Zoom = Zoom;
					break;
			}
		}
		public void InvalidateView() {
			InvalidateViewControl();
		}
		public void UpdateAll() {
			UpdateEverything();
		}
		public PointF DocumentToViewer(PdfDocumentPosition position) {
			PdfPoint point = position.Point;
			int pageIndex = position.PageIndex;
			if (pageIndex < 0)
				return new PointF((float)(point.X * clientToDocumentFactorX), (float)(point.Y * clientToDocumentFactorY));
			ViewManager viewManager = ViewManager;
			IList<IPage> pages = viewManager.Pages;
			if (pageIndex >= pages.Count)
				return new PointF();
			PdfPoint userPoint = viewer.Document.Pages[pageIndex].ToUserSpace(point, clientToDocumentFactorX, clientToDocumentFactorY, viewer.RotationAngle);
			PointF location = viewManager.GetPageRect(pages[pageIndex]).Location;
			return new PointF((float)(userPoint.X + location.X), (float)(userPoint.Y + location.Y));
		}
		public PointF DocumentToClient(PdfDocumentPosition documentPosition) {
			PointF documentPoint = DocumentToViewer(documentPosition);
			SizeF offset = ViewManager.DocOffset;
			float zoomFactor = Zoom;
			return new PointF((float)(documentPoint.X - offset.Width) / screenToDocumentFactorX * zoomFactor, (float)(documentPoint.Y - offset.Height) / screenToDocumentFactorY * zoomFactor);
		}
		public PointF GetDocumentPoint(PointF screenPoint) { 
			float zoomFactor = Zoom;
			PointF documentPoint = new PointF((float)(screenPoint.X * screenToDocumentFactorX / zoomFactor), (float)(screenPoint.Y * screenToDocumentFactorY / zoomFactor));
			ViewManager viewManager = ViewManager;
			SizeF docOffset = viewManager.DocOffset;
			documentPoint.X += docOffset.Width;
			documentPoint.Y += docOffset.Height;
			return documentPoint;
		}
		PdfDocumentPosition GetDocumentPosition(PointF screenPoint, IList<RectangleF> pageRectangles) { 
			pageRectangles = pageRectangles ?? new List<RectangleF>();
			PointF documentPoint = GetDocumentPoint(screenPoint);
			ViewManager viewManager = ViewManager;
			IList<IPage> pages = viewManager.Pages;
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				RectangleF pageRect = viewManager.GetPageRect(pages[i]);
				if (pageRect.Contains(documentPoint)) {
					PdfPoint pagePoint = new PdfPoint(documentPoint.X - pageRect.Left, documentPoint.Y - pageRect.Top);
					return DocumentToPage(i + 1, viewer.Document.Pages[i], pagePoint);
				}
				pageRectangles.Add(pageRect);
			}
			return new PdfDocumentPosition(0, new PdfPoint(documentPoint.X, documentPoint.Y)); 
		}
		public PdfDocumentPosition GetDocumentPosition(PointF screenPoint) { 
			return GetDocumentPosition(screenPoint, null);
		}
		public PdfDocumentPosition ClientToDocument(PointF screenPoint) {
			IList<RectangleF> pageRectangles = new List<RectangleF>();
			PdfDocumentPosition documentPosition = GetDocumentPosition(screenPoint, pageRectangles);
			if (documentPosition.PageNumber != 0)
				return documentPosition;
			PointF documentPoint = GetDocumentPoint(screenPoint);
			double x = documentPoint.X;
			double y = documentPoint.Y;
			PdfPoint point = new PdfPoint(x, y);
			double minDistance = double.MaxValue;
			int closestRectangleIndex = -1;
			int count = pageRectangles.Count;
			for (int i = 0; i < count; i++) {
				RectangleF rect = pageRectangles[i];
				double distance = PdfMathUtils.CalcDistanceToRectangle(point, new PdfRectangle(rect.Left, rect.Top, rect.Right, rect.Bottom));
				if (distance < minDistance) {
					minDistance = distance;
					closestRectangleIndex = i;
				}
			}
			if (closestRectangleIndex == -1)
				return new PdfDocumentPosition(0, point);
			RectangleF minRect = pageRectangles[closestRectangleIndex];
			if (x < minRect.Left)
				x = 0;
			else if (x > minRect.Right)
				x = minRect.Width;
			else
				x -= minRect.Left;
			if (y < minRect.Top)
				y = 0;
			else if (y > minRect.Bottom)
				y = minRect.Height;
			else
				y -= minRect.Top;
			return DocumentToPage(closestRectangleIndex + 1, viewer.Document.Pages[closestRectangleIndex], new PdfPoint(x, y));
		}
		public void Select(RectangleF clientRectangle) {
			RectangleF documentRect = GetVisibleRect(clientRectangle);
			ViewManager viewManager = ViewManager;
			IList<IPage> pages = viewManager.Pages;
			IList<PdfPage> pdfPages = viewer.Document.Pages;
			int count = pages.Count;
			List<PdfPageTextRange> textRange = new List<PdfPageTextRange>();
			List<PdfDocumentArea> areas = new List<PdfDocumentArea>();
			PdfDataSelector dataSelector = viewer.DataSelector;
			for (int i = 0; i < count; i++) {
				RectangleF pageRect = viewManager.GetPageRect(pages[i]);
				RectangleF intersection = RectangleF.Intersect(pageRect, documentRect);
				if (!intersection.IsEmpty) {
					PointF location = pageRect.Location;
					SizeF offset = new SizeF(location.X, location.Y);
					PdfPage page = pdfPages[i];
					PdfPoint pdfTopLeft = PageToDocument(page, PointF.Subtract(intersection.Location, offset));
					PdfPoint pdfBottomRight = PageToDocument(page, PointF.Subtract(new PointF(intersection.Right, intersection.Bottom), offset));
					PdfDocumentArea area = new PdfDocumentArea(i + 1, new PdfRectangle(pdfTopLeft.X, pdfBottomRight.Y, pdfBottomRight.X, pdfTopLeft.Y));
					areas.Add(area);
					PdfTextSelection textSelection = dataSelector.GetTextSelection(area);
					if (textSelection != null)
						textRange.AddRange(textSelection.TextRange);
				}
			}
			dataSelector.SelectText(textRange);
			if (!viewer.HasSelection)
				foreach (PdfDocumentArea area in areas) {
					dataSelector.SelectImage(area);
					if (viewer.HasSelection)
						break;
				}
		}
		public void UpdateCursor(Point point) {
			PdfDataSelector dataSelector = viewer.DataSelector;
			if (dataSelector != null)
				try {
					currentContent = dataSelector.GetContentInfoWhileSelecting(ClientToDocument(point));
					if (!HandTool)
						SetCursor(ActualCursor);
				}
				catch {
				}
		}
		public void RaiseFunctionalLimitsOccurred() {
			if (FunctionalLimitsOccurred != null)
				FunctionalLimitsOccurred(this, EventArgs.Empty);
		}
		RectangleF GetVisibleRect(RectangleF rect) {
			float factorX = screenToDocumentFactorX / ZoomFactor;
			float factorY = VerticalScreenToDocumentFactor;
			SizeF docOffset = ViewManager.DocOffset;
			return new RectangleF(docOffset.Width + rect.X * factorX, docOffset.Height + rect.Y * factorY, rect.Width * factorX, rect.Height * factorY);
		}
		PdfPoint PageToDocument(PdfPage page, PointF pagePoint) {
			return page.FromUserSpace(new PdfPoint(pagePoint.X, pagePoint.Y), clientToDocumentFactorX, clientToDocumentFactorY, viewer.RotationAngle);
		}
		PdfDocumentPosition DocumentToPage(int pageNumber, PdfPage page, PdfPoint point) {
			return new PdfDocumentPosition(pageNumber, page.FromUserSpace(point, clientToDocumentFactorX, clientToDocumentFactorY, viewer.RotationAngle));
		}
		int CalculateScrollPosition(ScrollBarBase scrollBar, int dimension, float value) {
			return Convert.ToInt32(Math.Min(1, Math.Max(0, value)) * (scrollBar.Maximum + scrollBar.Minimum - dimension) / Zoom);
		}
		void SetScrollHorizontal(int amount) {
			ViewManager.SetHorizScroll(amount * screenToDocumentFactorX);
			ViewControl.Invalidate();
			UpdateScrollBars();
			viewer.HandleScrolling();
		}
		void SetScrollVertical(int amount) {
			ViewManager.SetVertScroll(amount * screenToDocumentFactorY);
			ViewControl.Invalidate();
			UpdateScrollBars();
			viewer.HandleScrolling();
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2135")]
#endif
		[
		SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
		SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
		SecuritySafeCritical
		]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (viewer.Document != null && (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab)) && viewer.AcceptsTab) {
				if (keyData.HasFlag(Keys.Control))
					return base.ProcessCmdKey(ref msg, keyData);
				PdfDocumentStateController documentStateController = viewer.DocumentStateController;
				((PdfViewerValueEditingController)documentStateController.ViewerController.ValueEditingController).PostValue();
				if (keyData.HasFlag(Keys.Shift))
					documentStateController.TabBackward();
				else
					documentStateController.TabForward();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2135")]
#endif
		[UIPermissionAttribute(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
		[UIPermissionAttribute(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		[SecuritySafeCritical]
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys keys = keyData & Keys.KeyCode;
			if (keys == Keys.Tab && viewer.AcceptsTab && (keyData & Keys.Control) != Keys.None)
				keyData &= ~Keys.Control;
			return base.ProcessDialogKey(keyData);
		}
		protected override ViewManager CreateViewManager(ViewControl viewControl) {
			return new PdfDocumentViewerViewManager(this, viewControl);
		}
		protected override ScrollController CreateScrollController(XtraEditors.HScrollBar hScrollBar, XtraEditors.VScrollBar vScrollBar) {
			return new PdfDocumentViewerScrollController(this, hScrollBar, vScrollBar);
		}
		protected override void BeforeSetDocument() {
			PdfViewerDocument viewerDocument = Document as PdfViewerDocument;
			if (viewerDocument != null)
				viewerDocument.BeginDraw -= caretView.DeleteCaret;
		}
		protected override void AfterSetDocument() {
			PdfViewerDocument viewerDocument = Document as PdfViewerDocument;
			if (viewerDocument != null)
				viewerDocument.BeginDraw += caretView.DeleteCaret;
		}
		protected override bool TryGetCursor(IPage page, ref Cursor value) {
			base.TryGetCursor(page, ref value);
			if (!HandTool || (currentContent != null && currentContent.ContentType == PdfDocumentContentType.Annotation))
				value = ActualCursor;
			return value != null;
		}
		protected override void OnViewKeyDown(object sender, KeyEventArgs e) {
			if (!e.Alt)
				base.OnViewKeyDown(sender, e);
		}
		protected override bool HandleKey(Keys key) {
			switch (key) {
				case Keys.Enter:
					if (!viewer.HasFocus)
						ScrollPageDown();
					break;
				case Keys.PageDown:
					ScrollPageDown();
					break;
				case Keys.PageUp:
					ScrollPageUp();
					break;
				case Keys.Home:
					if (!viewer.HasCaret)
						PerformPageNavigation(PdfPageNavigationMode.Home);
					break;
				case Keys.End:
					if (!viewer.HasCaret)
						PerformPageNavigation(PdfPageNavigationMode.End);
					break;
				case Keys.Add:
				case Keys.Oemplus:
					if (Zoom < MaxZoom && ModifierKeys == Keys.Control)
						ZoomIn();
					break;
				case Keys.Subtract:
				case Keys.OemMinus:
					if (Zoom > MinZoom && ModifierKeys == Keys.Control)
						ZoomOut();
					break;
				default:
					if (HandTool || viewer.Caret == null)
						return base.HandleKey(key);
					break;
			}
			return false;
		}
		protected override void OnViewKeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Return)
				OnClick(e);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				caretView.Dispose();
		}
	}
}
