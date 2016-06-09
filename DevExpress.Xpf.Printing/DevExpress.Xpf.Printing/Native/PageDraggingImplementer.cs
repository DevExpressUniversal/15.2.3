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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Utils;
namespace DevExpress.Xpf.Printing.Native {
	public enum PageDraggingType { DragViaScrollViewer, DragViaTransform }
	public class PageDraggingImplementer {
		const string CS_PAGEDRAGGINGIMPLEMENTER_ID = "CS_PAGEDRAGGINGIMPLEMENTER_ID";
		bool isDraggingMode;
		Point? startDragPoint;
		Point cursorPos;
		public PageDraggingImplementer(IPreviewModel model, IScrollInfo scrollableOwner, PageDraggingType pageDraggingType) {
			Guard.ArgumentNotNull(model, "model");
			Guard.ArgumentNotNull(scrollableOwner, "scrollableOwner");
			Model = model;
			ScrollableOwner = scrollableOwner;
			PageDraggingType = pageDraggingType;
			DragOffset = new Point();
		}
		public bool IsPageDraggingEnabled { get { return isDraggingMode || startDragPoint.HasValue; } }
		public IPreviewModel Model { get; private set; }
		public Point DragOffset { get; private set; }
		public PageDraggingType PageDraggingType { get; private set; }
		IScrollInfo ScrollableOwner { get; set; }
		ScrollViewer ScrollViewer { get { return ScrollableOwner.ScrollOwner; } }
		void TrySetCursor(Cursor cursor) {
			if(Model == null || Model.CursorService == null)
				return;
			Model.CursorService.SetCursor((FrameworkElement)ScrollableOwner, cursor, CS_PAGEDRAGGINGIMPLEMENTER_ID);
			Model.CursorService.SetCursor(ScrollViewer, cursor, CS_PAGEDRAGGINGIMPLEMENTER_ID);
		}
		void TrySetCursor(CustomCursor cursorType) {
			if(Model == null || Model.CursorService == null)
				return;
			Model.CursorService.SetCursor((FrameworkElement)ScrollableOwner, Cursors.None, CS_PAGEDRAGGINGIMPLEMENTER_ID);
			Model.CursorService.SetCursor(ScrollViewer, cursorType, CS_PAGEDRAGGINGIMPLEMENTER_ID);
		}
		void TrySetCursorPosition(Point position) {
			if(Model == null || Model.CursorService == null)
				return;
			Model.CursorService.SetCursorPosition(position, (FrameworkElement)ScrollableOwner, CS_PAGEDRAGGINGIMPLEMENTER_ID);
		}
		void TryBlockCursorService() {
			if(Model == null || Model.CursorService == null)
				return;
			Model.CursorService.HideCustomCursor();
			Model.CursorService.BlockService(CS_PAGEDRAGGINGIMPLEMENTER_ID);
		}
		void TryUnblockCursorService() {
			if(Model == null || Model.CursorService == null)
				return;
			Model.CursorService.UnblockService(CS_PAGEDRAGGINGIMPLEMENTER_ID);
		}
		public void HandleMouseMove(Point position) {
			cursorPos = position;
			if(IsPageDraggingEnabled)
				TrySetCursorPosition(cursorPos);
			if(!startDragPoint.HasValue)
				return;
#if SL
			((UIElement)ScrollableOwner).CaptureMouse();
#endif
			Point endDragPoint = cursorPos;
			double difX = endDragPoint.X - startDragPoint.Value.X;
			double difY = endDragPoint.Y - startDragPoint.Value.Y;
			startDragPoint = endDragPoint;
			switch(PageDraggingType) {
				case PageDraggingType.DragViaScrollViewer:
					ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - difX);
					ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - difY);
					ScrollViewer.UpdateLayout();
					break;
				case PageDraggingType.DragViaTransform:
					DragOffset = new Point(DragOffset.X + difX, DragOffset.Y + difY);
					((ScrollablePageView)ScrollableOwner).UpdatePagePosition();
					break;
				default:
					throw new ArgumentException("PageDraggingType");
			}
		}
		public void HandleKeyUp(Key pressedKey) {
			if(pressedKey == Key.Space)
				isDraggingMode = false;
			if(!startDragPoint.HasValue && pressedKey == Key.Space) {
				TrySetCursor(Cursors.Arrow);
				TryUnblockCursorService();
				((UIElement)ScrollableOwner).ReleaseMouseCapture();
			}
		}
		public void HandleKeyDown(Key pressedKey) {
			if(IsPageDraggingEnabled)
				return;
			if(pressedKey == Key.Space) {
				TryBlockCursorService();
				isDraggingMode = true;
				TrySetCursor(PreviewCustomCursors.HandCursor);
				TrySetCursorPosition(cursorPos);
#if !SL
				((UIElement)ScrollableOwner).CaptureMouse();
#endif
			}
		}
		public void HandleMouseUp(out bool handled) {
			startDragPoint = null;
			if(isDraggingMode) {
				handled = true;
				TrySetCursor(PreviewCustomCursors.HandCursor);
			} else {
				handled = false;
				TrySetCursor(Cursors.Arrow);
				TryUnblockCursorService();
				((UIElement)ScrollableOwner).ReleaseMouseCapture();
			}
		}
		public void HandleMouseDown(Point position) {
			if(IsPageDraggingEnabled) {
				TrySetCursor(PreviewCustomCursors.HandDragCursor);
				startDragPoint = position;
			}
		}
	}
}
