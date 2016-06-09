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
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.UIAutomation;
using System.Windows.Media;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Security;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.XtraPrinting.Native;
using DevExpress.Xpf.Docking.Platform.Win32;
using DevExpress.Xpf.Layout.Core;
using System.Reflection;
namespace DevExpress.Xpf.Docking.Platform {
	interface IDraggableWindow {
		DockLayoutManager Manager { get; }
		FloatGroup FloatGroup { get; }
		Window Window { get; }
	}
	class Win32DragService {
		public bool IsInEvent { get { return IsDragging || IsResizing; } }
		public bool IsDragging { get; private set; }
		public bool IsResizing { get; private set; }
		public SizingAction SizingAction { get; internal set; }
		DragOperation dragOperation;
		internal bool TryStartDragging(IDraggableWindow window, bool isFloating) {
			if(IsInEvent) return false;
			if(Mouse.LeftButton == MouseButtonState.Pressed) {
				if(CanSuspendDragging(window)) return false;
				dragOperation = new DragOperation(window);
				FloatingView view = window.Manager.GetView(window.FloatGroup) as FloatingView;
				view.ReleaseCapture();
				IsDragging = true;
				if(isFloating) {
					var mousePos = DevExpress.Xpf.Docking.Platform.Win32.NativeHelper.GetMousePositionSafe();
					var dragOrigin = window.Manager.ViewAdapter.DragService.DragOrigin;
					var screenPoint = window.Manager.PointToScreen(dragOrigin);
					double dx = mousePos.X - screenPoint.X;
					double dy = mousePos.Y - screenPoint.Y;
					window.Window.Left += dx;
					window.Window.Top += dy;
				}
				IntPtr handle = new System.Windows.Interop.WindowInteropHelper(window.Window).Handle;
				using(new OverrideCursor(Cursors.Arrow)) {
					DevExpress.XtraPrinting.Native.Win32.SendMessage(handle, 0x112, 0xf012, IntPtr.Zero);
					DevExpress.XtraPrinting.Native.Win32.SendMessage(handle, 0x202, 0, IntPtr.Zero);
				}
			}
			return true;
		}
		bool CanSuspendDragging(IDraggableWindow window) {
			var viewElement = window.Manager.GetViewElement(window.FloatGroup.GetUIElement<FloatPanePresenter>()) as IDockLayoutElement;
			if(viewElement != null) {
				var dragItem = viewElement.CheckDragElement().Item;
				return window.Manager.RaiseItemCancelEvent(dragItem, DockLayoutManager.DockItemStartDockingEvent) || window.Manager.RaiseDockOperationStartingEvent(DockOperation.Move, dragItem);
			}
			return false;
		}
		public void DoDragging() {
			if(IsDragging) {
				FloatingView view = dragOperation.Window.Manager.GetView(dragOperation.Window.FloatGroup) as FloatingView;
				var buttons = DevExpress.Xpf.Layout.Core.Platform.MouseButtons.Left;
				var pos = NativeHelper.GetMousePositionSafe();
				pos = view.RootUIElement.PointFromScreen(pos);
				var ea = new Layout.Core.Platform.MouseEventArgs(pos, buttons);
				view.OnMouseMove(ea);
			}
		}
		public void FinishDragging() {
			if(!IsDragging && !IsResizing) return;
			IsDragging = IsResizing = false;
			if(Mouse.LeftButton == MouseButtonState.Released) {
				FloatingView view = dragOperation.Window.Manager.GetView(dragOperation.Window.FloatGroup) as FloatingView;
				var buttons = DevExpress.Xpf.Layout.Core.Platform.MouseButtons.None;
				var pos = NativeHelper.GetMousePositionSafe();
				pos = view.RootUIElement.PointFromScreen(pos);
				var ea = new Layout.Core.Platform.MouseEventArgs(pos, buttons, Layout.Core.Platform.MouseButtons.Left);
				view.OnMouseUp(ea);
			}
			Ref.Dispose(ref dragOperation);
		}
		internal bool TryStartSizing(IDraggableWindow floatingPaneWindow) {
			if(IsInEvent) return false;
			if(Mouse.LeftButton == MouseButtonState.Pressed) {
				if(floatingPaneWindow.Manager.ViewAdapter.DragService.OperationType == Layout.Core.OperationType.FloatingResizing) {
					FloatingView view = floatingPaneWindow.Manager.GetView(floatingPaneWindow.FloatGroup) as FloatingView;
					if(view != floatingPaneWindow.Manager.ViewAdapter.DragService.DragSource) return false;
					dragOperation = new DragOperation(floatingPaneWindow);
					if(Mouse.LeftButton == MouseButtonState.Pressed) {
						IsResizing = true;
						view.ReleaseCapture();
						using(new OverrideCursor(SizingAction.ToCursor())) {
							WindowHelper.DragSize(floatingPaneWindow.Window, SizingAction);
							return true;
						}
					}
				}
			}
			return false;
		}
		public void DoSizing() {
			if(!IsResizing) return;
			FloatingView view = dragOperation.Window.Manager.GetView(dragOperation.Window.FloatGroup) as FloatingView;
			var buttons = DevExpress.Xpf.Layout.Core.Platform.MouseButtons.Left;
			var pos = NativeHelper.GetMousePositionSafe();
			pos = view.RootUIElement.PointFromScreen(pos);
			var ea = new Layout.Core.Platform.MouseEventArgs(pos, buttons);
			view.OnMouseMove(ea);
		}
		class OverrideCursor : IDisposable {
			static Stack<Cursor> s_Stack = new Stack<Cursor>();
			public OverrideCursor(Cursor changeToCursor) {
				s_Stack.Push(changeToCursor);
				if(Mouse.OverrideCursor != changeToCursor)
					Mouse.OverrideCursor = changeToCursor;
			}
			public void Dispose() {
				s_Stack.Pop();
				Cursor cursor = s_Stack.Count > 0 ? s_Stack.Peek() : null;
				if(cursor != Mouse.OverrideCursor)
					Mouse.OverrideCursor = cursor;
			}
		}
		class DragOperation : IDisposable {
			public IDraggableWindow Window { get; private set; }
			public FloatGroup FloatGroup { get; private set; }
			public DockLayoutManager Manager { get; private set; }
			public DragOperation(IDraggableWindow window) {
				Window = window;
				FloatGroup = Window.FloatGroup;
				Manager = Window.Manager;
			}
			public bool IsDisposing { get; private set; }
			public void Dispose() {
				if(!IsDisposing) {
					IsDisposing = true;
					Manager = null;
					FloatGroup = null;
					Window = null;
				}
				GC.SuppressFinalize(this);
			}
		}
	}
}
