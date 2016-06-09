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
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf;
using DevExpress.Utils;
namespace DevExpress.Xpf.PdfViewer {
	public enum PdfCursors {
		HandCursor,
		HandDragCursor,
		ZoomInCursor,
		ZoomOutCursor,
		ZoomLimitCursor,
		CrossCursor
	}
	public static class CursorHelper {
		static Cursor HandCursor { get; set; }
		static Cursor HandDragCursor { get; set; }
		static Cursor ZoomInCursor { get; set; }
		static Cursor ZoomOutCursor { get; set; }
		static Cursor ZoomLimitCursor { get; set; }
		static Cursor CrossCursor { get; set; }
		public static void SetCursor(FrameworkElement element, Cursor cursor) {
			element.Do(x => x.Cursor = cursor);
		}
		public static void SetCursor(FrameworkElement element, PdfCursors cursor) {
			switch (cursor) {
				case PdfCursors.HandCursor:
					SetCursor(element, HandCursor);
					break;
				case PdfCursors.HandDragCursor:
					SetCursor(element, HandDragCursor);
					break;
				case PdfCursors.ZoomInCursor:
					SetCursor(element, ZoomInCursor);
					break;
				case PdfCursors.ZoomOutCursor:
					SetCursor(element, ZoomOutCursor);
					break;
				case PdfCursors.ZoomLimitCursor:
					SetCursor(element, ZoomLimitCursor);
					break;
				case PdfCursors.CrossCursor:
					SetCursor(element, CrossCursor);
					break;
				default:
					throw new ArgumentOutOfRangeException("cursor");
			}
		}
		static CursorHelper() {
			HandCursor = LoadCursor("Images/Cursors/CursorHand.cur", 11, 13);
			HandDragCursor = LoadCursor("Images/Cursors/CursorHandDrag.cur", 11, 13);
			ZoomInCursor = LoadCursor("Images/Cursors/CursorZoomIn.cur", 11, 11);
			ZoomOutCursor = LoadCursor("Images/Cursors/CursorZoomOut.cur", 11, 11);
			ZoomLimitCursor = LoadCursor("Images/Cursors/CursorZoomLimit.cur", 11, 11);
			CrossCursor = LoadCursor("Images/Cursors/CursorCross.cur", 11, 11);
		}
		static string GetUriString(string cursorFilePath, string rootNamespace) {
			return string.Format("pack://application:,,,/{0}{1};component/{2}", rootNamespace, AssemblyInfo.VSuffix, cursorFilePath);
		}
		static Stream UpdateCursorHotspot(Uri uri, byte hotspotx, byte hotspoty) {
			StreamResourceInfo sri = Application.GetResourceStream(uri);
			Stream s = sri.Stream;
			byte[] buffer = new byte[s.Length];
			s.Read(buffer, 0, (int)s.Length);
			MemoryStream ms = new MemoryStream();
			buffer[10] = hotspotx;
			buffer[12] = hotspoty;
			ms.Write(buffer, 0, (int)s.Length);
			ms.Position = 0;
			return ms;
		}
		static Cursor LoadCursor(string filename, byte hotspotx, byte hotspoty) {
			string uriString = GetUriString(filename, XmlNamespaceConstants.PdfViewerNamespace);
			Uri cursorFileUri = new Uri(uriString, UriKind.RelativeOrAbsolute);
			return new Cursor(UpdateCursorHotspot(cursorFileUri, hotspotx, hotspoty));
		}
	}
	public class CursorAttachedBehavior : Behavior<DXScrollViewer> {
		readonly TimeSpan UpdateCursorTimeSpan = TimeSpan.FromMilliseconds(1000);
		public static readonly DependencyProperty CursorModeProperty = DependencyPropertyManager.Register("CursorMode", typeof(CursorModeType), typeof(CursorAttachedBehavior), 
			new PropertyMetadata(CursorModeType.SelectTool, (obj, args) => ((CursorAttachedBehavior)obj).OnCursorModeChanged((CursorModeType)args.NewValue)));
		public CursorModeType CursorMode {
			get { return (CursorModeType)GetValue(CursorModeProperty); }
			set { SetValue(CursorModeProperty, value); }
		}
		WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler> PreviewKeyDownHandler { get; set; }
		WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler> PreviewKeyUpHandler { get; set; }
		WeakEventHandler<CursorAttachedBehavior, SelectionEventArgs, SelectionEventHandler> SelectionStartedChangedHandler { get; set; }
		WeakEventHandler<CursorAttachedBehavior, SelectionEventArgs, SelectionEventHandler> SelectionEndedChangedHandler { get; set; }
		WeakEventHandler<CursorAttachedBehavior, RoutedEventArgs, RoutedEventHandler> ZoomChangedHandler { get; set; }
		DispatcherTimer UpdateCursorTimer { get; set; }
		FrameworkElement CursorRoot { get; set; }
		PdfViewerControl PdfViewerRoot { get { return (PdfViewerControl)AssociatedObject.With(DocumentViewerControl.GetActualViewer); } }
		public CursorAttachedBehavior() {
			PreviewKeyDownHandler = new WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler>(this,
				(behavior, sender, args) => behavior.PreviewKeyDownHandlerInternal(args),
				(h, sender) => ((FrameworkElement)sender).PreviewKeyDown -= h.Handler,
				h => h.OnEvent);
			PreviewKeyUpHandler = new WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler>(this,
				(behavior, sender, args) => behavior.PreviewKeyUpHandlerInternal(args),
				(h, sender) => ((FrameworkElement)sender).PreviewKeyUp -= h.Handler,
				h => h.OnEvent);
			SelectionStartedChangedHandler = new WeakEventHandler<CursorAttachedBehavior, SelectionEventArgs, SelectionEventHandler>(this,
				(behavior, sender, args) => behavior.SelectionChangedHandlerInternal(args),
				(h, sender) => ((PdfViewerControl)sender).SelectionStarted -= h.Handler,
				h => h.OnEvent);
			SelectionEndedChangedHandler = new WeakEventHandler<CursorAttachedBehavior, SelectionEventArgs, SelectionEventHandler>(this,
				(behavior, sender, args) => behavior.SelectionChangedHandlerInternal(args),
				(h, sender) => ((PdfViewerControl)sender).SelectionEnded -= h.Handler,
				h => h.OnEvent);
			ZoomChangedHandler = new WeakEventHandler<CursorAttachedBehavior, RoutedEventArgs, RoutedEventHandler>(this,
				(behavior, sender, args) => behavior.ZoomChangedHandlerInternal(args),
				(h, sender) => ((PdfViewerControl)sender).ZoomChanged -= h.Handler,
				h => h.OnEvent);
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateCursorTimer = new DispatcherTimer(UpdateCursorTimeSpan, DispatcherPriority.Normal, UpdateCursorTick, AssociatedObject.Dispatcher);
			UpdateCursorTimer.Stop();
			((PdfPagesSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).PreviewMouseMove += OnPreviewMouseMove;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).MouseEnter += OnMouseEnter;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).MouseLeave += OnMouseLeave;
			CursorRoot = LayoutHelper.FindRoot(AssociatedObject) as FrameworkElement;
			CursorRoot.Do(x => x.PreviewKeyDown += PreviewKeyDownHandler.Handler);
			CursorRoot.Do(x => x.PreviewKeyUp += PreviewKeyUpHandler.Handler);
			PdfViewerRoot.Do(x => x.SelectionStarted += SelectionStartedChangedHandler.Handler);
			PdfViewerRoot.Do(x => x.SelectionEnded += SelectionEndedChangedHandler.Handler);
		}
		void UpdateCursorTick(object sender, EventArgs e) {
			if (!ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers)) {
				UpdateCursorTimer.Stop();
				SetDefaultCursor();
			}
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			((PdfPagesSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).PreviewMouseMove -= OnPreviewMouseMove;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).MouseEnter -= OnMouseEnter;
			((PdfPagesSelector)AssociatedObject.TemplatedParent).MouseLeave -= OnMouseLeave;
			CursorRoot.Do(x => x.PreviewKeyDown -= PreviewKeyDownHandler.Handler);
			CursorRoot.Do(x => x.PreviewKeyUp -= PreviewKeyUpHandler.Handler);
			UpdateCursorTimer.Stop();
		}
		protected virtual void OnCursorModeChanged(CursorModeType newValue) {
			SetDefaultCursor();
		}
		void PreviewKeyDownHandlerInternal(KeyEventArgs args) {
			SetDefaultCursor();
			UpdateCursorTimer.Stop();
			UpdateCursorTimer.Start();
		}
		void PreviewKeyUpHandlerInternal(KeyEventArgs args) {
			SetDefaultCursor();
			UpdateCursorTimer.Stop();
		}
		void SelectionChangedHandlerInternal(SelectionEventArgs args) {
			if (PdfViewerRoot == null)
				return;
			SetDefaultCursor(PdfViewerRoot.ConvertDocumentPositionToPixel(args.DocumentPosition));
		}
		void ZoomChangedHandlerInternal(RoutedEventArgs args) {
			SetDefaultCursor();
		}
		void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			SetDefaultCursor(e);
		}
		void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			SetDragCursor(e);
		}
		void OnPreviewMouseMove(object sender, MouseEventArgs e) {
			if (MouseHelper.IsMouseLeftButtonPressed(e) || PdfViewerRoot == null)
				return;
			if (CursorMode == CursorModeType.SelectTool)
				SetSelectToolModeCursor(GetMousePoint(e));
		}
		void SetSelectToolModeCursor(Point p) {
			if (PdfViewerRoot == null || !PdfViewerRoot.Document.Return(x => x.IsLoaded, () => false) || !PdfViewerRoot.Document.Pages.Any())
				return;
			var hitTestResult = PdfViewerRoot.HitTest(p);
			if (hitTestResult == null)
				return;
			switch (new PdfDocumentContent(null, hitTestResult.ContentType, hitTestResult.IsSelected).Cursor) {
				case PdfCursor.Context:
					CursorHelper.SetCursor(AssociatedObject, Cursors.Hand);
					break;
				case PdfCursor.Cross:
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.CrossCursor);
					break;
				case PdfCursor.Hand:
					CursorHelper.SetCursor(AssociatedObject, Cursors.Hand);
					break;
				case PdfCursor.IBeam:
					CursorHelper.SetCursor(AssociatedObject, Cursors.IBeam);
					break;
				case PdfCursor.Default:
					CursorHelper.SetCursor(AssociatedObject, Cursors.Arrow);
					break;
			}
		}
		void OnMouseLeave(object sender, MouseEventArgs e) {
			if (MouseHelper.IsMouseLeftButtonPressed(e))
				return;
			AssociatedObject.Cursor = Cursors.Arrow;
		}
		void OnMouseEnter(object sender, MouseEventArgs e) {
			if (MouseHelper.IsMouseLeftButtonPressed(e))
				return;
			SetDefaultCursor(e);
		}
		void SetDefaultCursor() {
			SetDefaultCursor(null);
		}
		void SetDefaultCursor(Point point) {
			switch (CursorMode) {
				case CursorModeType.HandTool:
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.HandCursor);
					break;
				case CursorModeType.MarqueeZoom:
					SetMarqueeZoomCursor();
					break;
				case CursorModeType.SelectTool:
					SetSelectToolModeCursor(point);
					break;
			}
		}
		void SetDefaultCursor(MouseEventArgs e) {
			var point = GetMousePoint(e);
			SetDefaultCursor(point);
		}
		void SetDragCursor(Point point) {
			switch (CursorMode) {
				case CursorModeType.HandTool:
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.HandDragCursor);
					break;
				case CursorModeType.MarqueeZoom:
					SetMarqueeZoomCursor();
					break;
				case CursorModeType.SelectTool:
					SetSelectToolModeCursor(point);
					break;
			}
		}
		void SetMarqueeZoomCursor() {
			if (KeyboardHelper.IsControlPressed) {
				if (PdfViewerRoot.With(x => x.ActualBehaviorProvider).Return(x => x.CanZoomOut(), () => true))
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.ZoomOutCursor);
				else
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.ZoomLimitCursor);
			}
			else {
				if (PdfViewerRoot.With(x => x.ActualBehaviorProvider).Return(x => x.CanZoomIn(), () => true))
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.ZoomInCursor);
				else
					CursorHelper.SetCursor(AssociatedObject, PdfCursors.ZoomLimitCursor);
			}
		}
		void SetDragCursor(MouseEventArgs e) {
			var point = GetMousePoint(e);
			SetDragCursor(point);
		}
		Point GetMousePoint(MouseEventArgs e) {
			if (e == null || PdfViewerRoot == null)
				return new Point(0, 0);
			return e.GetPosition(PdfViewerRoot);
		}
	}
}
