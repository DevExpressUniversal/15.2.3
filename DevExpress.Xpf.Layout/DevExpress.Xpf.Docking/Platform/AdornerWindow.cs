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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Platform {
	public class AdornerWindowContent : Decorator {
		public IView View { get; private set; }
		internal AdornerWindowContent(IView view) {
			View = view;
		}
	}
	public class AdornerWindow : Window, IDisposable {
		bool isOpenCore;
		bool isDisposingCore;
		MatrixTransform transform;
		public AdornerWindow(IAdornerWindowClient client, UIElement container) {
			Manager = DockLayoutManager.GetDockLayoutManager(container);
			WindowHelper.BindFlowDirection(this, Manager);
			Client = client;
			ShowActivated = false;
			AllowsTransparency = true;
			ShowInTaskbar = false;
			Background = Brushes.Transparent;
			Topmost = true;
			WindowStartupLocation = WindowStartupLocation.Manual;
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			View = GetView(Manager, Client);
			RootElement = new AdornerWindowContent(View);
			Window ownerWindow = Window.GetWindow(container);
			FrameworkElement owner = ownerWindow;
			if(owner == null || !Manager.IsDescendantOf(owner)) {
				owner = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopLevelVisual(Manager);
			}
			if(owner != null) {
				transform = Manager.TransformToVisual(owner) as MatrixTransform;
				if(transform != null && !transform.Matrix.IsIdentity) {
					Matrix matrix = transform.Matrix;
					transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
					if(!transform.Matrix.IsIdentity)
						((AdornerWindowContent)RootElement).LayoutTransform = transform;
				}
			}
			this.Content = RootElement;
			SubscribeEvents();
		}
		void SubscribeEvents() {
			PreviewKeyDown += AdornerWindow_PreviewKey;
			PreviewKeyUp += AdornerWindow_PreviewKey;
			PreviewMouseDown += AdornerWindow_PreviewMouseDown;
			MouseUp += AdornerWindow_MouseUp;
		}
		void UnSubscribeEvents() {
			PreviewKeyDown -= AdornerWindow_PreviewKey;
			PreviewKeyUp -= AdornerWindow_PreviewKey;
			PreviewMouseDown -= AdornerWindow_PreviewMouseDown;
			MouseUp -= AdornerWindow_MouseUp;
		}
		void AdornerWindow_MouseUp(object sender, MouseButtonEventArgs e) {
			View.OnMouseUp(EventArgsHelper.Convert(RootElement, e));
		}
		void AdornerWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			View.OnMouseDown(EventArgsHelper.Convert(RootElement, e));
		}
		void AdornerWindow_PreviewKey(object sender, KeyEventArgs e) {
			if(Manager != null) {
				Manager.RaiseEvent(e);
				Manager.ProcessKey(e);
			}
		}
		public IView View { get; private set; }
		static IView GetView(DockLayoutManager manager, IAdornerWindowClient client) {
			IView result = client as IView;
			if(result == null) {
				FloatingPaneWindow fw = client as FloatingPaneWindow;
				if(fw != null) {
					result = manager.GetView(fw.Container as IUIElement);
				}
			}
			return result;
		}
		public bool IsDisposing { get { return isDisposingCore; } }
		void IDisposable.Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				isOpenCore = false;
				UnSubscribeEvents();
				UnSubscribeOwnerWindowEvents(GetEventTarget());
				Close();
				Manager = null;
				RootElement = null;
				Client = null;
			}
			GC.SuppressFinalize(this);
		}
		public DockLayoutManager Manager { get; private set; }
		public UIElement RootElement { get; private set; }
		public IAdornerWindowClient Client { get; internal set; }
		public bool IsOpen {
			get { return isOpenCore; }
			set {
				if(IsOpen == value) return;
				if(value) UpdateFloatingBounds();
				isOpenCore = value;
				OnIsOpenChanged();
			}
		}
		protected void OnIsOpenChanged() {
			if(IsOpen) {
				Topmost = true;
				Show();
				WindowHelper.BringToFront(this);
				SubscribeOwnerWindowEvents(GetEventTarget());
			}
			else {
				UnSubscribeOwnerWindowEvents(GetEventTarget());
				Topmost = false;
				Hide();
			}
		}
		Window eventTarget;
		Window GetEventTarget() {
			if(eventTarget == null) {
				eventTarget = Client as FloatingPaneWindow;
				if(eventTarget == null) eventTarget = Owner;
			}
			return eventTarget;
		}
		public void UpdateFloatingBounds() {
			TrySetOwner();
			Rect bounds = Client.Bounds;
			if(transform != null && !transform.Matrix.IsIdentity) {
				Point sz = transform.Transform(new Point(bounds.Width, bounds.Height));
				bounds.Width = sz.X;
				bounds.Height = sz.Y;
			}
			double indent = GetAdornerIndentWithTransoform();
			bounds = Rect.Inflate(bounds, indent, indent);
			Left = bounds.Left;
			Top = bounds.Top;
			Width = bounds.Width;
			Height = bounds.Height;
		}
		public DockLayoutManager Container {
			get { return DockLayoutManager.GetDockLayoutManager(this); }
		}
		void TrySetOwner() {
			if(Owner != null || Container == null) return;
			Window containerWindow = Window.GetWindow(Container);
			if(containerWindow != null && containerWindow.IsVisible) {
				SubscribeOwnerWindowEvents(GetEventTarget());
			}
		}
		List<Window> subscriptions;
		bool EnsureUnSubscribed(Window ownerWindow) {
			if(subscriptions == null)
				subscriptions = new List<Window>();
			return !subscriptions.Contains(ownerWindow);
		}
		void SubscribeOwnerWindowEvents(Window ownerWindow) {
			if(ownerWindow == null) return;
			if(EnsureUnSubscribed(ownerWindow)) {
				subscriptions.Add(ownerWindow);
				ownerWindow.SizeChanged += Owner_SizeChanged;
				ownerWindow.LocationChanged += Owner_LocationChanged;
			}
		}
		void UnSubscribeOwnerWindowEvents(Window ownerWindow) {
			if(ownerWindow == null) return;
			if(subscriptions != null) {
				subscriptions.Remove(ownerWindow);
			}
			ownerWindow.SizeChanged -= Owner_SizeChanged;
			ownerWindow.LocationChanged -= Owner_LocationChanged;
		}
		void Owner_LocationChanged(object sender, EventArgs e) {
			if(!Container.IsTransparencyDisabled)
				UpdateFloatingBounds();
		}
		void Owner_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(!Container.IsTransparencyDisabled)
				UpdateFloatingBounds();
		}
		const double Indent = 50d;
		internal double GetAdornerIndentWithTransoform() {
			double indent = Indent;
			if(transform != null && !transform.Matrix.IsIdentity) {
				Point transformed = transform.Transform(new Point(indent, indent));
				indent = transformed.X;
			}
			return indent;
		}
		internal double GetAdornerIndentWithoutTransform() {
			return Indent;
		}
	}
	class AdornerWindowHelper : IDisposable {
		bool isDisposing;
		public AdornerWindowHelper(LayoutView view, DockLayoutManager container) {
			View = view;
			Container = container;
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Ref.Dispose(ref adornerWindowCore);
				View = null;
				Container = null;
			}
			GC.SuppressFinalize(this);
		}
		public DockLayoutManager Container { get; private set; }
		public LayoutView View { get; private set; }
		public AdornerWindow AdornerWindow {
			get { return adornerWindowCore; }
		}
		public void Reset() {
			Ref.Dispose(ref adornerWindowCore);
		}
		public void ShowAdornerWindow(bool forceUpdateAdornerBounds = false) {
			if(EnsureAdornerWindow(true)) {
				if(!AdornerWindow.IsOpen)
					AdornerWindow.IsOpen = true;
				else {
					if(forceUpdateAdornerBounds)
						AdornerWindow.UpdateFloatingBounds();
				}
			}
		}
		public void HideAdornerWindow() {
			if(AdornerWindow != null)
				AdornerWindow.IsOpen = false;
		}
		AdornerWindow adornerWindowCore;
		bool EnsureAdornerWindow(bool onShow = false) {
			bool updateAdornerWindw = Container.IsTransparencyDisabled && View is FloatingView && onShow;
			if(adornerWindowCore == null || updateAdornerWindw) {
				DevExpress.Xpf.Core.FloatingMode mode = Container.GetRealFloatingMode();
				if(mode == DevExpress.Xpf.Core.FloatingMode.Window) {
					adornerWindowCore = View.GetAdornerWindow();
					DockLayoutManager.SetDockLayoutManager(AdornerWindow, Container);
				}
			}
			return adornerWindowCore != null;
		}
		UIElement TryGetAdornerWindowRoot() {
			return (AdornerWindow != null) ? AdornerWindow.RootElement : null;
		}
	}
	class Win32AdornerWindowProvider: IDisposable {
		System.Windows.Threading.DispatcherTimer _HideTimer;
		System.Windows.Threading.DispatcherTimer HideTimer {
			get {
				if(_HideTimer == null) {
					_HideTimer = InvokeHelper.GetBackgroundTimer(180);
				}
				return _HideTimer;
			}
		}
		AdornerWindow adornerWindowCore;
		bool isDisposing;
		bool fCancelHiding;
		public Win32AdornerWindowProvider() {
			HideTimer.Tick += OnHideTimerTick;
		}
		void OnHideTimerTick(object sender, EventArgs e) {
			HideTimer.Stop();
			if(!fCancelHiding)
				HideAdornerWindow();
		}
		public void EnqueueHideAdornerWindow() {
			fCancelHiding = false;
			HideTimer.Start();
		}
		public void CancelHideAdornerWindow() {
			fCancelHiding = true;
			HideTimer.Stop();
		}
		public void HideAdornerWindow() {
			if(adornerWindowCore != null)
				adornerWindowCore.IsOpen = false;
		}
		public AdornerWindow GetWindow(IAdornerWindowClient client, DockLayoutManager manager) {
			if(adornerWindowCore == null || adornerWindowCore.IsDisposing) adornerWindowCore = new AdornerWindow(client, manager);
			else adornerWindowCore.Client = client;
			return adornerWindowCore;
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				HideTimer.Stop();
				HideTimer.Tick -= OnHideTimerTick;
				Ref.Dispose(ref adornerWindowCore);
			}
			GC.SuppressFinalize(this);
		}
	}
}
