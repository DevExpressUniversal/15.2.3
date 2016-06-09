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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Flyout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Navigation;
namespace DevExpress.Xpf.WindowsUI.Internal {
	enum FlyoutSourceType { FromTileBar, FromTileNavPane }
	interface IFlyoutServiceProvider : IFlyoutProvider {
		FlyoutService FlyoutService { get; }
	}
	interface IFlyoutProvider {
		DevExpress.Xpf.Editors.Flyout.FlyoutControl FlyoutControl { get; }
		DevExpress.Xpf.Editors.Flyout.FlyoutPlacement Placement { get; }
		IFlyoutEventListener FlyoutEventListener { get; set; }
	}
	interface IFlyoutEventListener {
		void OnFlyoutClosed(bool onClickThrough);
		void OnFlyoutClosed();
		void OnFlyoutOpened();
		void OnMouseLeave();
		void OnMouseEnter();
		WindowsUI.Internal.Flyout.FlyoutBase Flyout { get; }
	}
	class FlyoutManager {
		#region static
		public static FlyoutManager GetFlyoutManager(DependencyObject obj) {
			return (FlyoutManager)obj.GetValue(FlyoutManagerProperty);
		}
		public static void SetFlyoutManager(DependencyObject obj, FlyoutManager value) {
			obj.SetValue(FlyoutManagerProperty, value);
		}
		public static readonly DependencyProperty FlyoutManagerProperty =
			DependencyProperty.RegisterAttached("FlyoutManager", typeof(FlyoutManager), typeof(FlyoutManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		#endregion
		FrameworkElement Owner;
		public FlyoutManager(FrameworkElement owner) {
			Owner = owner;
			if(Owner.IsLoaded) {
				TopElement = LayoutHelper.FindRoot(Owner) as FrameworkElement;
			}
			Owner.Loaded += OnOwnerLoaded;
			Owner.Unloaded += OnOwnerUnloaded;
			Owner.SizeChanged += OnOwnerSizeChanged;
			CloseOnTopElementMouseDown = true;
		}
		#region Subscriptions
		void OnOwnerLoaded(object sender, RoutedEventArgs e) {
			TopElement = LayoutHelper.FindRoot(Owner) as FrameworkElement;
		}
		void OnOwnerUnloaded(object sender, RoutedEventArgs e) {
			TopElement = null;
			windowCore = null;
		}
		void Subscribe() {
			if(TopElement == null) return;
			SubscribeTopElementEvents();
			SubscribeApplicationEvents();
			SubscribeWindowEvents();
		}
		void Unsubscribe() {
			if(TopElement == null) return;
			UnsubscribeTopElementEvents();
			UnsubscribeApplicationEvents();
			UnsubscribeWindowEvents();
		}
		protected virtual void SubscribeApplicationEvents() {
			UnsubscribeApplicationEvents();
			if(Application.Current != null) {
				Application.Current.Deactivated += new EventHandler(OnApplicationDeactivated);
				applicationWr = new WeakReference(Application.Current);
			}
		}
		protected virtual void UnsubscribeApplicationEvents() {
			Application parentApp = applicationWr.Target as Application;
			if(parentApp != null) {
				parentApp.Deactivated -= new EventHandler(OnApplicationDeactivated);
				applicationWr = new WeakReference(null);
			}
		}
		protected virtual void SubscribeWindowEvents() {
			if(Window == null) return;
			UnsubscribeWindowEvents();
			Window.Deactivated += new EventHandler(OwnerWindowDeactivated);
			Window.LocationChanged += new EventHandler(OwnerWindowLoactionChanged);
			Window.SizeChanged += new SizeChangedEventHandler(OwnerWindowSizeChanged);
		}
		protected virtual void UnsubscribeWindowEvents() {
			if(Window == null)
				return;
			Window.Deactivated -= new EventHandler(OwnerWindowDeactivated);
			Window.LocationChanged -= new EventHandler(OwnerWindowLoactionChanged);
			Window.SizeChanged -= new SizeChangedEventHandler(OwnerWindowSizeChanged);
		}
		private void SubscribeTopElementEvents() {
			TopElement.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(TopElementMouseDown), true); 
		}
		private void UnsubscribeTopElementEvents() {
			TopElement.RemoveHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(TopElementMouseDown));
		}
		void ConditionalClose(bool force = false) {
			if(CloseOnTopElementMouseDown || force)
				CloseAll();
		}
		void OnOwnerSizeChanged(object sender, SizeChangedEventArgs e) {
			ConditionalClose(true);
		}
		private void OnApplicationDeactivated(object sender, EventArgs e) {
			ConditionalClose(true);
		}
		private void OwnerWindowDeactivated(object sender, EventArgs e) {
			ConditionalClose();
		}
		private void OwnerWindowLoactionChanged(object sender, EventArgs e) {
			ConditionalClose();
		}
		private void OwnerWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			ConditionalClose();
		}
		void TopElementMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if(!CloseOnTopElementMouseDown)
				return;
			var senderFlyout = FlyoutControl.GetFlyout(e.OriginalSource as DependencyObject);
			foreach(var provider in map.Keys) {
				if(senderFlyout == provider.FlyoutControl)
					return;
			}
			var parents = LayoutTreeHelper.GetVisualParents(e.OriginalSource as DependencyObject);
			foreach(var item in parents) {
				if((item as INavElement).Return(x => x.TileNavPane == Owner, () => false) || item == Owner)
					return;
			}
			foreach(var provider in map.Keys) {
				if(provider.FlyoutControl != null)
					provider.FlyoutControl.IsOpen = false;
			}
		}
		#endregion
		FrameworkElement topElement;
		protected internal FrameworkElement TopElement {
			get { return topElement; }
			set {
				if(TopElement == value) return;
				if(TopElement != null) Unsubscribe();
				topElement = value;
				if(TopElement != null) Subscribe();
			}
		}
		protected internal bool CloseOnTopElementMouseDown { get; set; }
		Window windowCore;
		protected Window Window {
			get {
				if(windowCore == null) windowCore = Window.GetWindow(TopElement);
				return windowCore; 
			}
		}
		protected WeakReference applicationWr = new WeakReference(null);
		public IFlyoutEventListener GetFlyoutTarget(FlyoutControl partFlyoutControl) {
			foreach(var provider in map.Keys) {
				if(provider.FlyoutControl == partFlyoutControl) return map[provider];
			}
			return null;
		}
		Dictionary<IFlyoutProvider, IFlyoutEventListener> map = new Dictionary<IFlyoutProvider, IFlyoutEventListener>();
		public void Register(IFlyoutProvider provider, IFlyoutEventListener target) {
			map[provider] = target;
		}
		public void Show(IFlyoutProvider provider, IFlyoutEventListener target) {
			if(provider.FlyoutControl.IsOpen) {
				var prevTarget = map[provider];
				prevTarget.OnFlyoutClosed();
				target.OnFlyoutOpened();
			}
			else {
				target.OnFlyoutOpened();
				provider.FlyoutControl.IsOpen = true;
				provider.FlyoutControl.Closed -= FlyoutControl_Closed;
				provider.FlyoutControl.Closed += FlyoutControl_Closed;
			}
			map[provider] = target;
		}
		public void Hide(IFlyoutProvider provider, IFlyoutEventListener target) {
			if(provider.FlyoutControl.IsOpen) {
				provider.FlyoutControl.IsOpen = false;
			}
		}
		public void CloseAll() {
			var keys = map.Keys.ToArray();
			foreach(var provider in keys) {
				if(provider.FlyoutControl != null) {
					provider.FlyoutControl.IsOpen = false;
				}
				else
					map.Remove(provider);
			}
		}
		public void CloseAllExcept(IFlyoutProvider provider) {
			var keys = map.Keys;
			foreach(var prov in keys) {
				if(provider == prov) continue;
				prov.FlyoutControl.IsOpen = false;
				map[prov].OnFlyoutClosed();
			}
		}
		void FlyoutControl_Closed(object sender, EventArgs e) {
			var keys = map.Keys;
			foreach(var provider in keys) {
				if(provider.FlyoutControl == sender) {
					map[provider].OnFlyoutClosed();
					map.Remove(provider);
					break;
				}
			}
		}
	}
	class FlyoutService {
		FrameworkElement Owner;
		public FlyoutService(FrameworkElement owner) {
			Owner = owner;
			if(Owner.IsLoaded) {
				TopElement = LayoutHelper.FindRoot(Owner) as FrameworkElement;
			}
			Owner.Loaded += OnOwnerLoaded;
			Owner.Unloaded += OnOwnerUnloaded;
			Owner.SizeChanged += OnOwnerSizeChanged;
			Owner.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnPreviewMouseOutside));
			Owner.AddHandler(Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnPreviewMouseOutside));
		}
		private void OnPreviewMouseOutside(object sender, MouseButtonEventArgs e) {
			Flyout.FlyoutBase affectedFlyout = Flyout.FlyoutBase.GetFlyout(e.OriginalSource as DependencyObject);
			if(e.OriginalSource == OpenedFlyout || OpenedFlyout != affectedFlyout)
				CloseAll(true);
		}
		#region Subscriptions
		void OnOwnerLoaded(object sender, RoutedEventArgs e) {
			TopElement = LayoutHelper.FindRoot(Owner) as FrameworkElement;
		}
		void OnOwnerUnloaded(object sender, RoutedEventArgs e) {
			TopElement = null;
			windowCore = null;
		}
		void OnOwnerSizeChanged(object sender, SizeChangedEventArgs e) {
			CloseAll();
		}
		void Subscribe() {
			if(TopElement == null) return;
			SubscribeTopElementEvents();
			SubscribeApplicationEvents();
			SubscribeWindowEvents();
		}
		void Unsubscribe() {
			if(TopElement == null) return;
			UnsubscribeTopElementEvents();
			UnsubscribeApplicationEvents();
			UnsubscribeWindowEvents();
		}
		protected virtual void SubscribeApplicationEvents() {
			UnsubscribeApplicationEvents();
			if(Application.Current != null) {
				Application.Current.Deactivated += new EventHandler(OnApplicationDeactivated);
				applicationWr = new WeakReference(Application.Current);
			}
		}
		protected virtual void SubscribeWindowEvents() {
			if(Window == null) return;
			UnsubscribeWindowEvents();
			Window.Deactivated += new EventHandler(OwnerWindowDeactivated);
			Window.LocationChanged += new EventHandler(OwnerWindowLoactionChanged);
			Window.SizeChanged += new SizeChangedEventHandler(OwnerWindowSizeChanged);
		}
		private void SubscribeTopElementEvents() {
			TopElement.MouseDown += TopElementPreviewMouseDown;
		}
		protected virtual void UnsubscribeApplicationEvents() {
			Application parentApp = applicationWr.Target as Application;
			if(parentApp != null) {
				parentApp.Deactivated -= new EventHandler(OnApplicationDeactivated);
				applicationWr = new WeakReference(null);
			}
		}
		protected virtual void UnsubscribeWindowEvents() {
			if(Window == null) return;
			Window.Deactivated -= new EventHandler(OwnerWindowDeactivated);
			Window.LocationChanged -= new EventHandler(OwnerWindowLoactionChanged);
			Window.SizeChanged -= new SizeChangedEventHandler(OwnerWindowSizeChanged);
		}
		private void UnsubscribeTopElementEvents() {
			TopElement.MouseDown -= TopElementPreviewMouseDown;
		}
		private void OnApplicationDeactivated(object sender, EventArgs e) {
			CloseAll();
		}
		private void OwnerWindowDeactivated(object sender, EventArgs e) {
			CloseAll();
		}
		private void OwnerWindowLoactionChanged(object sender, EventArgs e) {
			CloseAll();
		}
		private void OwnerWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			CloseAll();
		}
		void TopElementPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
		}
		#endregion
		FrameworkElement topElement;
		protected internal FrameworkElement TopElement {
			get { return topElement; }
			set {
				if(TopElement == value) return;
				if(TopElement != null) Unsubscribe();
				topElement = value;
				if(TopElement != null) Subscribe();
			}
		}
		Window windowCore;
		protected Window Window {
			get {
				if(windowCore == null) windowCore = Window.GetWindow(TopElement);
				return windowCore;
			}
		}
		protected WeakReference applicationWr = new WeakReference(null);
		DevExpress.Xpf.Bars.Native.WeakList<Flyout.FlyoutBase> listeners = new DevExpress.Xpf.Bars.Native.WeakList<Flyout.FlyoutBase>();
		void CloseAll(bool onClickThrough = false) {
			foreach(var flyout in listeners) {
				flyout.Close(onClickThrough);
			}
			SetShownFlyout(null);
		}
		public void RegisterListener(Flyout.FlyoutBase flyout) {
			listeners.CleanResolvedReferences();
			if(!listeners.Contains(flyout)) listeners.Add(flyout);
		}
		public void UnregisterListener(Flyout.FlyoutBase flyout) {
			if(listeners.Contains(flyout)) listeners.Remove(flyout);
			listeners.CleanResolvedReferences();
		}
		internal void SetShownFlyout(Flyout.FlyoutBase shownFlyout) {
			OpenedFlyout = shownFlyout;
		}
		Flyout.FlyoutBase OpenedFlyout;
		public bool IsFlyoutShown { get { return OpenedFlyout != null; } }
		public void Show(Flyout.FlyoutBase flyout) {
			if(OpenedFlyout != null && OpenedFlyout != flyout && OpenedFlyout.FlyoutControl != null && OpenedFlyout.FlyoutControl == flyout.FlyoutControl) {
				OpenedFlyout.FlyoutControl = null;
				OpenedFlyout.IsOpen = false;
			}
			SetShownFlyout(flyout);
			flyout.IsOpen = true;
		}
		public void Hide(Flyout.FlyoutBase flyout) {
			flyout.IsOpen = false;
			if(flyout.FlyoutControl.IsOpen) flyout.FlyoutControl.IsOpen = false;
			if(OpenedFlyout == flyout)
				SetShownFlyout(null);
		}
	}
}
