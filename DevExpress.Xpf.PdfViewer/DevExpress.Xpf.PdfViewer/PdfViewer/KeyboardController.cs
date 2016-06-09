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

using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.PdfViewer.Extensions;
using DevExpress.Xpf.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.PdfViewer.Internal {
	public class PdfKeyboardAndMouseController : KeyboardAndMouseController {
		const double ScrollXOffset = 10d;
		const double ScrollYOffset = 10d;
		const double LargeModifier = 3d;
		const double LargeStep = 100;
		readonly Locker clearSelectionLocker = new Locker();
		PdfCommandProvider PdfCommandProvider {
			get { return base.CommandProvider as PdfCommandProvider; }
		}
		PdfPresenterControl Presenter {
			get { return presenter as PdfPresenterControl; }
		}
		new PdfNavigationStrategy NavigationStrategy {
			get { return base.NavigationStrategy as PdfNavigationStrategy; }
		}
		PdfBehaviorProvider BehaviorProvider {
			get { return Presenter.PdfBehaviorProvider; }
		}
		PdfDocumentViewModel Document {
			get { return presenter.Document as PdfDocumentViewModel; }
		}
		SelectionRectangle SelectionRectangle {
			get { return Presenter.SelectionRectangle; }
		}
		CursorModeType CursorMode {
			get { return Presenter.CursorMode; }
		}
		ScrollViewer ScrollViewer {
			get { return Presenter.ScrollViewer; }
		}
		bool IsSelecting { get; set; }
		public PdfKeyboardAndMouseController(PdfPresenterControl presenter)
			: base(presenter) {
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if (Presenter.IsInEditing) {
				Presenter.ActiveEditorOwner.ProcessKeyDown(e);
				return;
			}
			ProcessKeyDownInternal(e);
		}
		protected virtual void ProcessKeyDownInternal(KeyEventArgs e) {
			if (e.Handled)
				return;
			switch (e.Key) {
				case Key.A:
					if (IsCaretCreated() && KeyboardHelper.IsShiftPressed && KeyboardHelper.IsControlPressed) {
						PdfCommandProvider.With(x => x.UnselectAllCommand).Do(x => x.Execute(null));
						e.Handled = true;
					}
					break;
				case Key.Up:
					if (IsCaretCreated())
						PdfCommandProvider.With(x => x.SelectionCommand).Do(x => x.Execute(KeyboardHelper.IsShiftPressed ? PdfSelectionCommand.SelectUp : PdfSelectionCommand.MoveUp));
					else
						PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineUp));
					e.Handled = true;
					break;
				case Key.Down:
					if (IsCaretCreated())
						PdfCommandProvider.With(x => x.SelectionCommand).Do(x => x.Execute(KeyboardHelper.IsShiftPressed ? PdfSelectionCommand.SelectDown : PdfSelectionCommand.MoveDown));
					else
						PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineDown));
					e.Handled = true;
					break;
				case Key.Right:
					if (Presenter.ActualPdfViewer.IsSearchControlVisible)
						break;
					if (IsCaretCreated()) {
						PdfSelectionCommand selectionCommand;
						if (KeyboardHelper.IsShiftPressed && KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.SelectNextWord;
						else if (KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.MoveNextWord;
						else if (KeyboardHelper.IsShiftPressed)
							selectionCommand = PdfSelectionCommand.SelectRight;
						else
							selectionCommand = PdfSelectionCommand.MoveRight;
						PdfCommandProvider.With(x => x.SelectionCommand).Do(x => x.Execute(selectionCommand));
					}
					else {
						PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineRight));
					}
					e.Handled = true;
					break;
				case Key.Left:
					if (Presenter.ActualPdfViewer.IsSearchControlVisible)
						break;
					if (IsCaretCreated()) {
						PdfSelectionCommand selectionCommand;
						if (KeyboardHelper.IsShiftPressed && KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.SelectPreviousWord;
						else if (KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.MovePreviousWord;
						else if (KeyboardHelper.IsShiftPressed)
							selectionCommand = PdfSelectionCommand.SelectLeft;
						else
							selectionCommand = PdfSelectionCommand.MoveLeft;
						PdfCommandProvider.With(x => x.SelectionCommand).Do(x => x.Execute(selectionCommand));
					}
					else {
						PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineLeft));
					}
					e.Handled = true;
					break;
				case Key.PageUp:
					PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.PageUp));
					e.Handled = true;
					break;
				case Key.PageDown:
					PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.PageDown));
					e.Handled = true;
					break;
				case Key.Home:
					if (Presenter.ActualPdfViewer.IsSearchControlVisible)
						break;
					if (IsCaretCreated()) {
						PdfSelectionCommand selectionCommand;
						if (KeyboardHelper.IsShiftPressed && KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.SelectDocumentStart;
						else if (KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.MoveDocumentStart;
						else if (KeyboardHelper.IsShiftPressed)
							selectionCommand = PdfSelectionCommand.SelectLineStart;
						else
							selectionCommand = PdfSelectionCommand.MoveLineStart;
						PdfCommandProvider.With(x => x.SelectionCommand).Do(x => x.Execute(selectionCommand));
					}
					else {
						PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.Home));
					}
					e.Handled = true;
					break;
				case Key.End:
					if (Presenter.ActualPdfViewer.IsSearchControlVisible)
						break;
					if (IsCaretCreated()) {
						PdfSelectionCommand selectionCommand;
						if (KeyboardHelper.IsShiftPressed && KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.SelectDocumentEnd;
						else if (KeyboardHelper.IsControlPressed)
							selectionCommand = PdfSelectionCommand.MoveDocumentEnd;
						else if (KeyboardHelper.IsShiftPressed)
							selectionCommand = PdfSelectionCommand.SelectLineEnd;
						else
							selectionCommand = PdfSelectionCommand.MoveLineEnd;
						PdfCommandProvider.With(x => x.SelectionCommand).Do(x => x.Execute(selectionCommand));
					}
					else {
						PdfCommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.End));
					}
					e.Handled = true;
					break;
				case Key.Tab:
					if (Presenter.ActualPdfViewer.IsSearchControlVisible)
						break;
					if (ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)))
						Document.DocumentStateController.TabBackward();
					else {
						Document.DocumentStateController.TabForward();
					}
					e.Handled = true;
					break;
				case Key.Space:
				case Key.Enter:
					if (Presenter.ActualPdfViewer.IsSearchControlVisible)
						break;
					Document.DocumentStateController.SubmitFocus();
					break;
			}
		}
		public override void ProcessMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.ProcessMouseLeftButtonDown(e);
			if (Presenter.Document == null || CursorMode != CursorModeType.MarqueeZoom && CursorMode != CursorModeType.SelectTool)
				return;
			if (CursorMode == CursorModeType.MarqueeZoom && (KeyboardHelper.IsControlPressed && !BehaviorProvider.CanZoomOut() || !KeyboardHelper.IsControlPressed && !BehaviorProvider.CanZoomIn()))
				return;
			Point cursorPosition = e.GetPosition(ItemsPanel);
			if (CursorMode == CursorModeType.SelectTool)
				Document.DocumentStateController.MouseDown(new PdfMouseAction(NavigationStrategy.CalcDocumentPosition(cursorPosition), PdfMouseButton.Left,
					ModifierKeysHelper.GetKeyboardModifiers().ToPdfModifierKeys(), e.ClickCount));
			SetupSelectionRectangle(cursorPosition);
		}
		void SetupSelectionRectangle(Point cursorPosition) {
			var hitTest = Presenter.HitTest(cursorPosition);
			if (CursorMode == CursorModeType.SelectTool && hitTest.ContentType != PdfDocumentContentType.Text && hitTest.ContentType != PdfDocumentContentType.Image)
				return;
			IsSelecting = true;
			SelectionRectangle.SetVerticalOffset(ScrollViewer.VerticalOffset, false);
			SelectionRectangle.SetHorizontalOffset(ScrollViewer.HorizontalOffset, false);
			SelectionRectangle.SetViewport(new Size(Math.Max(ScrollViewer.ViewportWidth, ScrollViewer.ExtentWidth), Math.Max(ScrollViewer.ViewportHeight, ScrollViewer.ExtentHeight)));
			SelectionRectangle.SetStartPoint(cursorPosition);
			Mouse.Capture(presenter, CaptureMode.SubTree);
		}
		public override void ProcessMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.ProcessMouseLeftButtonUp(e);
			Point cursorPosition = e.GetPosition(ItemsPanel);
			if (CursorMode == CursorModeType.MarqueeZoom)
				ProcessMarqueeZoom();
			else if (CursorMode == CursorModeType.SelectTool)
				Document.DocumentStateController.MouseUp(new PdfMouseAction(NavigationStrategy.CalcDocumentPosition(cursorPosition), PdfMouseButton.Left,
					ModifierKeysHelper.GetKeyboardModifiers().ToPdfModifierKeys(), e.ClickCount));
			ReleaseSelectionRectangle();
		}
		public override void ProcessMouseRightButtonDown(MouseButtonEventArgs e) {
			base.ProcessMouseRightButtonDown(e);
			if (e.LeftButton == MouseButtonState.Pressed) {
				Point cursorPosition = e.GetPosition(ItemsPanel);
				Document.DocumentStateController.MouseUp(new PdfMouseAction(NavigationStrategy.CalcDocumentPosition(cursorPosition), PdfMouseButton.Left,
					ModifierKeysHelper.GetKeyboardModifiers().ToPdfModifierKeys(), e.ClickCount));
				ReleaseSelectionRectangle();
			}
		}
		void ReleaseSelectionRectangle() {
			IsSelecting = false;
			SelectionRectangle.Reset();
			presenter.ReleaseMouseCapture();
		}
		void ProcessMarqueeZoom() {
			if (IsMarqueeSmallChange())
				MarqueeScrollSmallChange(SelectionRectangle.StartPoint);
			else
				NavigationStrategy.ProcessMarqueeZoom(SelectionRectangle.Rectangle, SelectionRectangle.X, SelectionRectangle.Y);
		}
		public override void ProcessMouseMove(MouseEventArgs e) {
			base.ProcessMouseMove(e);
			Point cursorPosition = e.GetPosition(ItemsPanel);
			UpdateSelectionRectangle(cursorPosition);
			if (CursorMode == CursorModeType.SelectTool) {
				if (!IsSelecting)
					ProcessMouseMoveInternal();
				else 
					ProcessSelectionMouseMove();
			}
		}
		void ProcessMouseMoveInternal() {
			Point cursorPosition = MouseHelper.GetPosition(presenter);
			bool isMouseOutsidePresenter = IsMouseOutsidePresenter(cursorPosition);
			Document.DocumentStateController.MouseMove(new PdfMouseAction(NavigationStrategy.CalcDocumentPosition(cursorPosition), GetMouseButton(),
				ModifierKeysHelper.GetKeyboardModifiers().ToPdfModifierKeys(), 0, isMouseOutsidePresenter));
		}
		PdfMouseButton GetMouseButton() {
			return Mouse.LeftButton == MouseButtonState.Pressed ? PdfMouseButton.Left : PdfMouseButton.None;
		}
		void ProcessSelectionMouseMove() {
			bool isMouseOutsidePresenter = IsMouseOutsidePresenter(MouseHelper.GetPosition(presenter));
			Document.DocumentStateController.MouseMove(new PdfMouseAction(NavigationStrategy.CalcDocumentPosition(SelectionRectangle.AnchorPoint), PdfMouseButton.Left,
				ModifierKeysHelper.GetKeyboardModifiers().ToPdfModifierKeys(), 0, isMouseOutsidePresenter));
		}
		bool IsMarqueeSmallChange() {
			return SelectionRectangle.IsEmpty || (SelectionRectangle.Width.LessThan(20d) && SelectionRectangle.Height.LessThan(20d));
		}
		void MarqueeScrollSmallChange(Point anchorPoint) {
			NavigationStrategy.ZoomToAnchorPoint(!KeyboardHelper.IsControlPressed, anchorPoint);
		}
		void UpdateSelectionRectangle(Point cursorPosition) {
			if (!IsSelecting)
				return;
			SelectionRectangle.SetPointPosition(cursorPosition);
			if (!IsMouseOutsidePresenter(cursorPosition))
				return;
			ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + CalcScrollXOffset(cursorPosition));
			SelectionRectangle.SetHorizontalOffset(ScrollViewer.HorizontalOffset, true);
			ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + CalcScrollYOffset(cursorPosition));
			SelectionRectangle.SetVerticalOffset(ScrollViewer.VerticalOffset, true);
		}
		bool IsCaretCreated() {
			return Presenter.Document.With(x => x.Caret) != null;
		}
		double CalcScrollXOffset(Point position) {
			double deltaX = position.X.GreaterThan(ItemsPanel.ActualWidth) ? position.X - ItemsPanel.ActualWidth : 0d;
			deltaX = position.X.LessThan(0d) ? position.X : deltaX;
			return Math.Min(Math.Abs(deltaX), ScrollXOffset) * Math.Sign(deltaX) * (IsLargeStep(deltaX) ? LargeModifier : 1d);
		}
		double CalcScrollYOffset(Point position) {
			double deltaY = position.X.GreaterThan(ItemsPanel.ActualHeight) ? position.Y - ItemsPanel.ActualHeight : 0d;
			deltaY = position.Y.LessThan(0d) ? position.Y : deltaY;
			return Math.Min(Math.Abs(deltaY), ScrollYOffset) * Math.Sign(deltaY) * (IsLargeStep(deltaY) ? LargeModifier : 1d);
		}
		bool IsLargeStep(double delta) {
			return Math.Abs(delta) > LargeStep;
		}
		bool IsMouseOutsidePresenter(Point cursorPosition) {
			return cursorPosition.X.GreaterThan(ItemsPanel.ActualWidth) || (cursorPosition.X.LessThan(0) || (cursorPosition.Y.GreaterThan(ItemsPanel.ActualHeight) || cursorPosition.Y.LessThan(0)));
		}
		public void BringCurrentSelectionPointIntoView() {
			UpdateSelectionRectangle(MouseHelper.GetPosition(ItemsPanel));
			if (CursorMode == CursorModeType.SelectTool)
				ProcessSelectionMouseMove();
		}
	}
}
