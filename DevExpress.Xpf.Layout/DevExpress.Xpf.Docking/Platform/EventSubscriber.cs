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
using SWI = System.Windows.Input;
namespace DevExpress.Xpf.Docking.Platform {
	abstract class ViewEventSubscriber<T> : IDisposable {
		protected Layout.Core.Platform.BaseView View { get; private set; }
		public T Root { get; set; }
		public T Source { get; private set; }
		protected ViewEventSubscriber(T source, Layout.Core.Platform.BaseView view) {
			Source = source;
			Root = source;
			View = view;
			if(Source != null)
				Subscribe(Source);
		}
		protected abstract void Subscribe(T element);
		protected abstract void UnSubscribe(T element);
		public void Dispose() {
			if(Source != null) {
				UnSubscribe(Source);
				Source = default(T);
			}
			View = null;
			Root = default(T);
			GC.SuppressFinalize(this);
		}
	}
	class ViewEventSubscriberHelper {
		public static bool IsInPopup(SWI.MouseEventArgs e) {
			return false;
		}
		static bool IsInPopupCore(DependencyObject dObj) {
			return DockLayoutManagerHelper.IsPopupRoot(Core.Native.LayoutHelper.FindRoot(dObj));
		}
		static bool IsChildElement(DependencyObject container, DependencyObject dObj) {
			return DevExpress.Xpf.Core.Native.LayoutHelper.IsChildElement(container, dObj) || container == dObj;
		}
		public static bool CanHandlePreviewEvent(DependencyObject container, SWI.MouseEventArgs e) {
			DependencyObject dObj = e.OriginalSource as DependencyObject;
			return dObj != null && (!IsInPopupCore(dObj) || IsChildElement(container, dObj));
		}
	}
	class MouseEventSubscriber : ViewEventSubscriber<UIElement> {
		public MouseEventSubscriber(UIElement source, Layout.Core.Platform.BaseView view)
			: base(source, view) {
		}
		protected override void Subscribe(UIElement element) {
			element.PreviewMouseDown += RootUIElementPreviewMouseDown;
			element.MouseMove += RootUIElementMouseMove;
			element.MouseUp += RootUIElementMouseUp;
			element.MouseDown += RootUIElementMouseDown;
			element.MouseLeave += RootUIElementMouseLeave;
		}
		protected override void UnSubscribe(UIElement element) {
			element.PreviewMouseDown -= RootUIElementPreviewMouseDown;
			element.MouseMove -= RootUIElementMouseMove;
			element.MouseUp -= RootUIElementMouseUp;
			element.MouseLeave -= RootUIElementMouseLeave;
			element.MouseDown -= RootUIElementMouseDown;
		}
		void RootUIElementMouseUp(object sender, SWI.MouseButtonEventArgs e) {
			if(((LayoutView)View).Container.Win32DragService.IsInEvent) return;
			var mouseArgs = EventArgsHelper.Convert(Root, e);
			View.OnMouseUp(mouseArgs);
			e.Handled = mouseArgs.Handled; 
		}
		void RootUIElementMouseMove(object sender, SWI.MouseEventArgs e) {
			if(((LayoutView)View).Container.Win32DragService.IsInEvent) return;
			if(CanSkipMoveEvent(e)) return;
			View.OnMouseMove(EventArgsHelper.Convert(Root, e));
		}
		void RootUIElementPreviewMouseDown(object sender, SWI.MouseButtonEventArgs e) {
			if(((LayoutView)View).Container.Win32DragService.IsInEvent) return;
			if(ViewEventSubscriberHelper.CanHandlePreviewEvent(sender as DependencyObject, e))
				View.OnMouseDown(EventArgsHelper.Convert(Root, e));
		}
		void RootUIElementMouseDown(object sender, SWI.MouseButtonEventArgs e) {
			e.Handled = View != null && View.CanHandleMouseDown();
		}
		void RootUIElementMouseLeave(object sender, SWI.MouseEventArgs e) {
			View.OnMouseLeave();
		}
		bool CanSkipMoveEvent(SWI.MouseEventArgs e) {
			return View.Adapter.DragService.DragSource != null && e.LeftButton == SWI.MouseButtonState.Released;
		}
		internal void OnDesignTimeEvent(object sender, RoutedEventArgs e) {
			if(!(e is SWI.MouseEventArgs)) return;
			if(e is SWI.MouseButtonEventArgs) {
				if(e.RoutedEvent == UIElement.PreviewMouseDownEvent)
					RootUIElementPreviewMouseDown(sender, (SWI.MouseButtonEventArgs)e);
				if(e.RoutedEvent == UIElement.MouseUpEvent)
					RootUIElementMouseUp(sender, (SWI.MouseButtonEventArgs)e);
			}
			else {
				if(e.RoutedEvent == UIElement.MouseMoveEvent) {
					RootUIElementMouseMove(sender, (SWI.MouseEventArgs)e);
				}
			}
		}
	}
	class KeyboardEventSubscriber : ViewEventSubscriber<IInputElement> {
		public KeyboardEventSubscriber(IInputElement rootUIElement, Layout.Core.Platform.BaseView view)
			: base(rootUIElement, view) {
		}
		protected override void Subscribe(IInputElement element) {
			element.PreviewKeyDown += RootUIElementKeyDown;
			element.PreviewKeyUp += RootUIElementKeyDown;
		}
		protected override void UnSubscribe(IInputElement element) {
			element.PreviewKeyDown -= RootUIElementKeyDown;
			element.PreviewKeyUp -= RootUIElementKeyDown;
		}
		void RootUIElementKeyDown(object sender, SWI.KeyEventArgs e) {
			if(e.IsDown)
				View.OnKeyDown(e.Key);
			if(e.IsUp)
				View.OnKeyUp(e.Key);
			if(View != null && View.Adapter.DragService.OperationType != Layout.Core.OperationType.Regular)
				if(e.Key == SWI.Key.Tab) e.Handled = true;
		}
	}
}
