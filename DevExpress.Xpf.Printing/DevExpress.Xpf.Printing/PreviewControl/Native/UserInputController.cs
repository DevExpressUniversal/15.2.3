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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.DocumentViewer.Extensions;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class UserInputController : KeyboardAndMouseController {
		const double ScrollXOffset = 10d;
		const double ScrollYOffset = 10d;
		const double LargeModifier = 3d;
		const double LargeStep = 100;
		SelectionService SelectionService { get { return Presenter.SelectionService; } }
		DocumentPresenterControl Presenter { get { return presenter as DocumentPresenterControl; } }
		ScrollViewer ScrollViewer { get { return Presenter.ScrollViewer; } }
		SelectionRectangle SelectionRectangle { get { return Presenter.SelectionRectangle; } }
		protected new DocumentNavigationStrategy NavigationStrategy { get { return (DocumentNavigationStrategy)base.NavigationStrategy; } }
		CursorModeType CursorMode {
			get { return Presenter.CursorMode; }
		}
		internal bool IsSelecting { get; set; }
		bool CanSelect { get; set; }
		public UserInputController(DocumentPresenterControl presenter) : base(presenter) { }
		internal void OnScrollChanged(ScrollChangedEventArgs e) {
			SelectionService.CorrectStartPoint(-e.VerticalChange);
		}
		public override void ProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.ProcessKeyDown(e);
			if(e.Key == Key.Escape) {
				Presenter.Document.ResetMarkedBricks();
				SelectionService.OnKillFocus();
			} if(presenter.ActualDocumentViewer.CommandBarStyle == CommandBarStyle.None && (e.Key ==Key.F && Keyboard.Modifiers == ModifierKeys.Control)) {
				CommandProvider.ShowFindTextCommand.TryExecute(true);
			}
		}
		public override void ProcessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.ProcessMouseLeftButtonDown(e);
			Presenter.Document.ResetMarkedBricks();
			Point cursorLocation = e.GetPosition(ItemsPanel);
			if(CursorMode == CursorModeType.SelectTool)
				SelectionService.OnMouseDown(cursorLocation.ToWinFormsPoint(), MouseButton.Left.ToWinFormsMouseButtons(), ModifierKeysHelper.GetKeyboardModifiers().ToWinFormsModifierKeys());
			if(Presenter.Document == null || CursorMode == CursorModeType.HandTool)
				return;
			((DocumentPreviewControl)Presenter.ActualDocumentViewer).UpdateSelection(SelectionService.HasSelection);
			CanSelect = true;
		}
		public override void ProcessMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.ProcessMouseLeftButtonUp(e);
			Point cursorLocation = e.GetPosition(ItemsPanel);
			if(CursorMode == CursorModeType.SelectTool)
				SelectionService.OnMouseUp(cursorLocation.ToWinFormsPoint(), e.ChangedButton.ToWinFormsMouseButtons(), ModifierKeysHelper.GetKeyboardModifiers().ToWinFormsModifierKeys());
			if(!IsSelecting) {
				ProcessMouseClicks(e);
			} else {
				ReleaseSelectionRectangle();
			}
			CanSelect = false;
			((DocumentPreviewControl)Presenter.ActualDocumentViewer).UpdateSelection(SelectionService.HasSelection);
		}
		void ProcessMouseClicks(MouseButtonEventArgs e) {
			Presenter.NavigationStrategy.ProcessMouseClick(e.GetPosition(Presenter));
		}
		public override void ProcessMouseMove(MouseEventArgs e) {
			base.ProcessMouseMove(e);
			Point cursorLocation = e.GetPosition(ItemsPanel);
			if(CanSelect && !IsSelecting && e.LeftButton == MouseButtonState.Pressed) {
				if(CursorMode == CursorModeType.SelectTool)
					SetupSelectionRectangle(cursorLocation);
				((DocumentPreviewControl)Presenter.ActualDocumentViewer).UpdateSelection(SelectionService.HasSelection);
			}
			if(CursorMode == CursorModeType.SelectTool) {
				if(!IsSelecting)
					ProcessMouseMoveInternal();
				else
					ProcessSelectionMouseMove();
			}
			if(SelectionService.CanSelect) UpdateSelectionRectangle(cursorLocation, 0);
			((DocumentPreviewControl)Presenter.ActualDocumentViewer).UpdateSelection(SelectionService.HasSelection);
		}
		void ProcessMouseMoveInternal() {
			Point cursorLocation = MouseHelper.GetPosition(presenter);
			bool isMouseOutsidePresenter = IsMouseOutsidePresenter(MouseHelper.GetPosition(presenter));
			SelectionService.OnMouseMove(cursorLocation.ToWinFormsPoint(), System.Windows.Forms.MouseButtons.Left, ModifierKeysHelper.GetKeyboardModifiers().ToWinFormsModifierKeys(), isMouseOutsidePresenter);
			Presenter.NavigationStrategy.ProcessMouseMove(cursorLocation);
		}
		void ProcessSelectionMouseMove() {
			Point cursorLocation = MouseHelper.GetPosition(presenter);
			bool isMouseOutsidePresenter = IsMouseOutsidePresenter(MouseHelper.GetPosition(presenter));
			SelectionService.OnMouseMove(cursorLocation.ToWinFormsPoint(), System.Windows.Forms.MouseButtons.Left, ModifierKeysHelper.GetKeyboardModifiers().ToWinFormsModifierKeys(), isMouseOutsidePresenter);
		}
		public override void ProcessMouseWheel(System.Windows.Input.MouseWheelEventArgs e) {
			if(IsSelecting)
				return;
			base.ProcessMouseWheel(e);
		}
		public override void ProcessMouseRightButtonDown(MouseButtonEventArgs e) {
			base.ProcessMouseRightButtonDown(e);
			if(e.LeftButton == MouseButtonState.Pressed) {
				SelectionService.ResetSelectedBricks();
				ReleaseSelectionRectangle();
				((DocumentPreviewControl)Presenter.ActualDocumentViewer).UpdateSelection(SelectionService.HasSelection);
			}
		}
		void SetupSelectionRectangle(Point cursorPosition) {
			IsSelecting = true;
			SelectionRectangle.SetVerticalOffset(ScrollViewer.VerticalOffset, false);
			SelectionRectangle.SetHorizontalOffset(ScrollViewer.HorizontalOffset, false);
			SelectionRectangle.SetViewport(new Size(Math.Max(ScrollViewer.ViewportWidth, ScrollViewer.ExtentWidth), Math.Max(ScrollViewer.ViewportHeight, ScrollViewer.ExtentHeight)));
			SelectionRectangle.SetStartPoint(cursorPosition);
			Mouse.Capture(presenter, CaptureMode.SubTree);
		}
		void UpdateSelectionRectangle(Point cursorPosition, double delta) {
			if(!IsSelecting)
				return;
			SelectionRectangle.SetPointPosition(cursorPosition);
			if(!IsMouseOutsidePresenter(cursorPosition))
				return;
			ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + CalcScrollXOffset(cursorPosition));
			SelectionRectangle.SetHorizontalOffset(ScrollViewer.HorizontalOffset, true);
			ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + CalcScrollYOffset(cursorPosition));
			SelectionRectangle.SetVerticalOffset(ScrollViewer.VerticalOffset, true);
			SelectionService.SetStartPoint(SelectionRectangle.StartPoint.ToWinFormsPoint());
		}
		void ReleaseSelectionRectangle() {
			IsSelecting = false;
			SelectionRectangle.Reset();
			Presenter.ReleaseMouseCapture();
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
	}
}
