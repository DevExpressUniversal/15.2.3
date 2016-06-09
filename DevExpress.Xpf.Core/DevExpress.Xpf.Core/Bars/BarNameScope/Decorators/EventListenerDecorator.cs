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
using DevExpress.Xpf.Core.Internal;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
namespace DevExpress.Xpf.Bars.Native {	
	public interface IEventListenerDecoratorService {
		bool HasWindow { get; }
		bool CheckSkipEventProcessing(object sender, EventArgs e);
		bool CheckSkipEventProcessing(object sender, EventArgs e, ScopeSearchSettings settings);
	}
	class EventListenerDecoratorService : IEventListenerDecoratorService {
		readonly EventListenerDecorator decorator;
		public EventListenerDecoratorService(EventListenerDecorator decorator) { this.decorator = decorator; }
		public bool HasWindow { get { return decorator.Return(x => x.HasWindow, () => false); } }
		public bool CheckSkipEventProcessing(object sender, EventArgs e) {
			if (decorator == null)
				return false;
			return decorator.CheckSkipEventProcessing(sender, e);
		}
		public bool CheckSkipEventProcessing(object sender, EventArgs e, ScopeSearchSettings settings) {
			if (decorator == null)
				return false;
			return decorator.CheckSkipEventProcessing(sender, e, settings);
		}
	}
	public class WinFormsMessageFilter : DispatcherObject, System.Windows.Forms.IMessageFilter {
		const int WM_LBUTTONDOWN = 0x0201;
		const int MK_LBUTTON = 0x0001;
		const int WM_MOVING = 0x0216;
		const int WM_MOVE = 0x0003;
		const int WM_NCLBUTTONDOWN = 0x00A1;
		const int WM_ACTIVATEAPP = 0x001C;
		WeakReference eventListenerDecoratorReference = new WeakReference(null);
		HwndSource source;
		System.Windows.Forms.Form elementHostForm;
		System.Windows.Forms.FormWindowState windowState = System.Windows.Forms.FormWindowState.Normal;
		public WindowState? CurrentWindowState {
			get {
				if (elementHostForm == null)
					return null;
				if (windowState == System.Windows.Forms.FormWindowState.Maximized)
					return WindowState.Maximized;
				if (windowState == System.Windows.Forms.FormWindowState.Minimized)
					return WindowState.Minimized;
				return WindowState.Normal;
			}
		}
		public bool? IsWindowActive {
			get {
				if (elementHostForm == null)
					return null;
				return elementHostForm == System.Windows.Forms.Form.ActiveForm;
			}
		}
		public bool IsAttached { get; private set; }
		public WinFormsMessageFilter(EventListenerDecorator eventListenerDecorator, HwndSource source) {
			this.eventListenerDecoratorReference = new WeakReference(eventListenerDecorator);
			this.source = source;
		}
		public bool Attach() {
			if(source!=null)
				elementHostForm = System.Windows.Forms.Control.FromChildHandle(source.Handle).With(x => x.FindForm());
			if (elementHostForm != null) {
				windowState = elementHostForm.WindowState;
				elementHostForm.Activated += FormActivated;
				elementHostForm.Deactivate += FormDeactivated;
				elementHostForm.SizeChanged += FormSizeChanged;
				return (IsAttached = true);
			}
			return (IsAttached = false);
		}
		private void FormSizeChanged(object sender, EventArgs e) {
			EventListenerDecorator eventListenerDecorator = (EventListenerDecorator)eventListenerDecoratorReference.Target;
			if (eventListenerDecorator == null)
				return;
			if (windowState != elementHostForm.WindowState)
				eventListenerDecorator.OwnerWindowStateChanged(sender, e);
		}
		void FormDeactivated(object sender, EventArgs e) {
			EventListenerDecorator eventListenerDecorator = (EventListenerDecorator)eventListenerDecoratorReference.Target;
			if (eventListenerDecorator == null)
				return;
			eventListenerDecorator.OwnerWindowDeactivated(sender, e);
		}
		void FormActivated(object sender, EventArgs e) {
			EventListenerDecorator eventListenerDecorator = (EventListenerDecorator)eventListenerDecoratorReference.Target;
			if (eventListenerDecorator == null)
				return;
			eventListenerDecorator.OwnerWindowActivated(sender, e);
		}
		public void Detach() {
			if (elementHostForm != null) {
				elementHostForm.Activated -= FormActivated;
				elementHostForm.Deactivate -= FormDeactivated;
				elementHostForm.SizeChanged -= FormSizeChanged;
				IsAttached = false;
			}
		}
		public bool PreFilterMessage(ref System.Windows.Forms.Message m) {
			EventListenerDecorator eventListenerDecorator = (EventListenerDecorator)eventListenerDecoratorReference.Target;
			if (eventListenerDecorator == null)
				return false;
			if (source.IsDisposed) {
				eventListenerDecorator.Detach();
				return false;
			}
			if (m.Msg == WM_LBUTTONDOWN) {
				if (m.WParam == (new IntPtr(MK_LBUTTON))) {
					eventListenerDecorator.OnMouseLeftButtonDownMessage();
				}
			}
			if(m.Msg == WM_MOVING || m.Msg == WM_MOVE || m.Msg == WM_NCLBUTTONDOWN) {
				eventListenerDecorator.OwnerWindowLocationChanged(null, null);
			}			
			return false;
		}
	}
	public class EventListenerDecorator : IBarNameScopeDecorator {
		static readonly Action<RoutedEventArgs, object> setSource;
		static readonly Action<RoutedEventArgs, object> setOriginalSource;
		BarNameScope scope;
		protected FrameworkElement EventSource { get; private set; }
		protected Window Window { get; private set; }
		protected WinFormsMessageFilter MessageFilter { get; private set; }
		protected WindowState? CurrentWindowState {
			get {
				if (Window == null) {
					if (MessageFilter != null)
						return MessageFilter.CurrentWindowState;
					return null;
				}
				return Window.WindowState;
			}
		}		
		protected bool? IsWindowActive {
			get {
				if (Window == null) {
					if (MessageFilter != null)
						return MessageFilter.IsWindowActive;
					return null;
				}
				return Window.IsActive;
			}
		}
		public bool HasWindow {
			get { return Window != null || MessageFilter != null && MessageFilter.IsAttached; }
		}
		static EventListenerDecorator() {
			setSource = ReflectionHelper.CreateFieldSetter<RoutedEventArgs, object>(typeof(RoutedEventArgs), "_source", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			setOriginalSource = ReflectionHelper.CreateFieldSetter<RoutedEventArgs, object>(typeof(RoutedEventArgs), "_originalSource", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseDownEvent, new MouseButtonEventHandler(OwnerWindowPreviewMouseDown), true);
		}
		public void Attach(BarNameScope scope) {
			this.scope = scope;
			bool hasWindow = false;
			if (scope.Target != null)
				AttachScopeTargetEvents();			
			Window = GetWindow(scope, out hasWindow);
			if (Window != null) {
				Window.Activated += new EventHandler(OwnerWindowActivated);
				Window.Deactivated += new EventHandler(OwnerWindowDeactivated);
				Window.LocationChanged += new EventHandler(OwnerWindowLocationChanged);
				Window.StateChanged += new EventHandler(OwnerWindowStateChanged);
				Window.SizeChanged += new SizeChangedEventHandler(OwnerWindowSizeChanged);				
				if(scope.Target == Window) {
					(PresentationSource.FromVisual(Window) as HwndSource).AddHook(OnWindowMessageHook);
				}
			} else if (scope.Parent == null && !hasWindow) {
#if DEBUGTEST
				if (scope.Target is DevExpress.Xpf.Core.Tests.TestWindow)
					return;
#endif
				MessageFilter = CreateMessageFilter(scope.Target.With(PresentationSource.FromDependencyObject) as HwndSource);
				System.Windows.Forms.Application.AddMessageFilter(MessageFilter);
				if (!MessageFilter.Attach()) {
					DetachMessageFilter();
					EventSource = PresentationSource.FromDependencyObject(scope.Target).With(x=>x.RootVisual) as FrameworkElement;
					if (EventSource != null) {
						EventSource.SizeChanged += OwnerWindowSizeChanged;
						EventSource.PreviewMouseDown += OwnerWindowPreviewMouseDown;
					}
				}
			}
		}
		const int WM_KILLFOCUS = 0x0008;
		protected virtual IntPtr OnWindowMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if (msg == WM_KILLFOCUS) {
				NavigationTree.ExitMenuMode(false, false);
			}
			return IntPtr.Zero;
		}
		protected virtual void AttachScopeTargetEvents() {
			AddHandler(scope.Target, MenuModeHelper.EnterMenuModeEvent, new RoutedEventHandler(OnEnterMenuMode));
		}		
		protected virtual void DetachScopeTargetEvents() {
			RemoveHandler(scope.Target, MenuModeHelper.EnterMenuModeEvent, new RoutedEventHandler(OnEnterMenuMode));
		}
		void OnEnterMenuMode(object sender, RoutedEventArgs e) {
			if (CheckSkipEventProcessing(sender, e, ScopeSearchSettings.Descendants | ScopeSearchSettings.Local))
				e.Handled = true;
		}
		protected virtual void AddHandler(object target, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo = false) {
			if (target is UIElement)
				((UIElement)target).AddHandler(routedEvent, handler, handledEventsToo);
			if (target is ContentElement)
				((ContentElement)target).AddHandler(routedEvent, handler, handledEventsToo);
		}
		protected virtual void RemoveHandler(object target, RoutedEvent routedEvent, Delegate handler) {
			if (target is UIElement)
				((UIElement)target).RemoveHandler(routedEvent, handler);
			if (target is ContentElement)
				((ContentElement)target).RemoveHandler(routedEvent, handler);
		}
		protected virtual WinFormsMessageFilter CreateMessageFilter(HwndSource source) {
			return new WinFormsMessageFilter(this, source);
		}
		protected static Window GetWindow(BarNameScope scope, out bool hasWindow) {
			var wnd = Window.GetWindow(scope.Target);
			hasWindow = wnd != null;
			return wnd.If(x => x.Dispatcher != null && x.Dispatcher.CheckAccess());
		}
		public void Detach() {
			if (scope.Target != null)
				DetachScopeTargetEvents();
			if (Window != null) {
				Window.Activated -= new EventHandler(OwnerWindowActivated);
				Window.Deactivated -= new EventHandler(OwnerWindowDeactivated);
				Window.LocationChanged -= new EventHandler(OwnerWindowLocationChanged);
				Window.StateChanged -= new EventHandler(OwnerWindowStateChanged);
				Window.SizeChanged -= new SizeChangedEventHandler(OwnerWindowSizeChanged);
				if (scope.Target == Window) {
					(PresentationSource.FromVisual(Window) as HwndSource).RemoveHook(OnWindowMessageHook);
				}
			}
			DetachMessageFilter();
			if (EventSource != null) {
				EventSource.SizeChanged -= OwnerWindowSizeChanged;
				EventSource.PreviewMouseDown -= OwnerWindowPreviewMouseDown;
				EventSource = null;
			}
		}
		protected virtual void DetachMessageFilter() {
			if (MessageFilter != null) {
				MessageFilter.Detach();
				System.Windows.Forms.Application.RemoveMessageFilter(MessageFilter);
				MessageFilter = null;
			}
		}
		protected internal virtual void OnMouseLeftButtonDownMessage() {
			var tree = scope.ScopeTree;
			var eventArgs = new MouseButtonEventArgs(Mouse.PrimaryDevice, (int)DateTime.Now.Ticks, MouseButton.Left) { RoutedEvent = Mouse.PreviewMouseDownEvent };
			setSource(eventArgs, Mouse.DirectlyOver);
			setOriginalSource(eventArgs, Mouse.DirectlyOver);
			if (tree != null) {				
				if (tree.Find(
					x => x.GetService<IEventListenerDecoratorService>().CheckSkipEventProcessing(null, eventArgs)
					, ScopeSearchSettings.Ancestors | ScopeSearchSettings.Descendants | ScopeSearchSettings.Local)
					.Any())
					return;				
			}
			PopupMenuManager.CloseAllPopups(Mouse.DirectlyOver, eventArgs);
		}
		static void OwnerWindowPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			var wSender = ((DependencyObject)sender);
			if (!wSender.With(x => x.Dispatcher).If(x => x.CheckAccess()).ReturnSuccess())
				return;
			var scopeTree = BarNameScope.GetScope((DependencyObject)sender).With(x => x.ScopeTree);
			if (scopeTree != null) {
				if (scopeTree.Find(
					x => x.GetService<IEventListenerDecoratorService>().CheckSkipEventProcessing(sender, e)
					, ScopeSearchSettings.Ancestors | ScopeSearchSettings.Descendants | ScopeSearchSettings.Local)
					.Any())
					return;
			}			
			PopupMenuManager.CloseAllPopups(sender, e);
		}
		protected virtual void OwnerWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			if (CheckSkipEventProcessing(sender, e))
				return;
			PopupMenuManager.CloseAllPopups();
		}
		protected internal virtual void OwnerWindowStateChanged(object sender, EventArgs e) {
			if (CheckSkipEventProcessing(sender, e))
				return;
			PopupMenuManager.CloseAllPopups();
			if (CurrentWindowState.HasValue) {
				var cs = scope.GetService<ICustomizationService>();
				if (CurrentWindowState == WindowState.Minimized) {
					cs.HideCustomizationForm();
					scope.With(x => x.Target).Do(x => BarManagerHelper.HideFloatingBars(x, false, false));
				} else {
					cs.RestoreCustomizationForm();
					if (IsWindowActive == true)
						scope.With(x => x.Target).Do(x => BarManagerHelper.ShowFloatingBars(x));
				}
			}
		}
		protected internal virtual void OwnerWindowLocationChanged(object sender, EventArgs e) {
			if (CheckSkipEventProcessing(sender, e))
				return;
			PopupMenuManager.CloseAllPopups();
		}
		protected internal virtual void OwnerWindowActivated(object sender, EventArgs e) {
			if (CheckSkipEventProcessing(sender, e))
				return;
			if (CurrentWindowState == WindowState.Minimized)
				return;
			scope.GetService<ICustomizationService>().RestoreCustomizationForm();
			scope.With(x => x.Target).Do(x => BarManagerHelper.ShowFloatingBars(x));
		}
		protected internal virtual void OwnerWindowDeactivated(object sender, EventArgs e) {
			if (CheckSkipEventProcessing(sender, e))
				return;
			PopupMenuManager.CloseAllPopups();
			scope.With(x => x.Target).Do(x => BarManagerHelper.HideFloatingBars(x, false, false));
		}
		public bool CheckSkipEventProcessing(object sender, EventArgs e, ScopeSearchSettings settings) {			
			var scopeTree = scope.With(x => x.ScopeTree);
			if (scopeTree != null) {
				if (scopeTree.Find(x => x.GetService<IEventListenerDecoratorService>().CheckSkipEventProcessing(sender, e), settings).Any())
					return true;
			}
			return false;
		}
		public bool CheckSkipEventProcessing(object sender, EventArgs e) {
			foreach (IEventListenerClient listener in scope[typeof(IEventListenerClient)].Values) {
				if (listener.ReceiveEvent(sender, e))
					return true;
			}
			return false;
		}		
	}
	public interface IEventListenerClient : IMultipleElementRegistratorSupport {
		bool ReceiveEvent(object sender, EventArgs e);
	}
}
