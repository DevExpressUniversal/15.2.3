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
using System.Linq;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core;
#if SILVERLIGHT
using DefinitionBase = System.Windows.DependencyObject;
using DevExpress.Xpf.Docking.VisualElements;
#endif
#if !SILVERLIGHT
using SWC = System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Docking.Platform.Win32;
#else
using SWC = DevExpress.Xpf.Layout.Core;
#endif
namespace DevExpress.Xpf.Docking.Platform {
	abstract class BaseResizingPreviewHelper : IResizingPreviewHelper {
		public BaseResizingPreviewHelper(IView view) {
			DevExpress.Xpf.Layout.Core.Base.AssertionException.IsNotNull(view);
			viewCore = view;
		}
		IView viewCore;
		public LayoutView View { get { return viewCore as LayoutView; } }
		public DockLayoutManager Owner { get { return View.Container; } }
		protected virtual DevExpress.Xpf.Core.FloatingMode Mode { get { return Owner.GetRealFloatingMode(); } }
		protected ILayoutElement Element { get; private set; }
		protected Point StartPoint { get; private set; }
		public Size MinSize { get; set; }
		public Size MaxSize { get; set; }
		public BoundsHelper BoundsHelper { get; set; }
		protected Rect InitialBounds;
		void ForceUpdateElementBounds(ILayoutElement element) {
			element.Invalidate();
			element.EnsureBounds();
		}
		protected internal Rect GetItemBounds(ILayoutElement element) {
			if(element != null) {
				ForceUpdateElementBounds(element);
				return ElementHelper.GetRect(element);
			}
			return Rect.Empty;
		}
		#region IResizingPreviewHelper Members
		public void InitResizing(Point start, ILayoutElement element) {
			Element = element;
			StartPoint = start;
			OnInitResizing();
		}
		public void Resize(Point change) {
			OnResizing(change);
		}
		public void EndResizing() {
			OnEndResizing();
		}
		#endregion
		protected abstract UIElement GetUIElement();
		protected virtual void OnInitResizing() {
			if(BoundsHelper == null) BoundsHelper = new Platform.BoundsHelper(View, Element, MinSize);
			InitialBounds = Mode == Core.FloatingMode.Adorner ? GetInitialAdornerBounds() :
			GetInitialWindowBounds();
			View.ResizingWindowHelper.ShowResizeWindow(Resizer, InitialBounds, Mode);
		}
		protected virtual void OnResizing(Point change) {
			Rect bounds1 = Mode == Core.FloatingMode.Adorner ? GetAdornerBounds(change) :
				GetWindowBounds(change);
			View.ResizingWindowHelper.UpdateResizeWindow(bounds1);
		}
		protected virtual void OnEndResizing() {
			View.ResizingWindowHelper.HideResizeWindow();
		}
		protected virtual Rect GetInitialAdornerBounds() {
			return Rect.Empty;
		}
		protected virtual Rect GetInitialWindowBounds() {
			return Rect.Empty;
		}
		protected virtual Rect GetAdornerBounds(Point change) {
			return Rect.Empty;
		}
		protected virtual Rect GetWindowBounds(Point change) {
			return Rect.Empty;
		}
		UIElement resizer;
		protected UIElement Resizer {
			get {
				if(resizer == null) {
					resizer = GetUIElement();
				}
				return resizer;
			}
		}
#if !SILVERLIGHT
		protected Rect CorrectBounds(Rect bounds1) {
			PresentationSource pSource = PresentationSource.FromDependencyObject(Owner);
			Point location = bounds1.Location;
			if(pSource != null)
				location = Owner.PointToScreen(bounds1.Location);
			if(pSource != null) {
				bounds1 = new Rect(
						pSource.CompositionTarget.TransformFromDevice.Transform(location), bounds1.Size
					);
			}
			else bounds1 = new Rect(location, bounds1.Size);
			return bounds1;
		}
#endif
	}
	class LayoutResizingPreviewHelper : BaseResizingPreviewHelper {
		LayoutGroup BackgroundGroup;
		public LayoutResizingPreviewHelper(IView view, LayoutGroup backgroundGroup)
			: base(view) {
			BackgroundGroup = backgroundGroup;
		}
		protected override UIElement GetUIElement() {
			return new ShadowResizePointer();
		}
		protected override Core.FloatingMode Mode {
			get { return Core.FloatingMode.Adorner; }
		}
		protected override Rect GetInitialAdornerBounds() {
			return GetItemBounds(Element);
		}
		protected override Rect GetAdornerBounds(Point change) {
			Rect newPointerBounds = InitialBounds;
			RectHelper.Offset(ref newPointerBounds, change.X, change.Y);
			return newPointerBounds;
		}
		protected override void OnInitResizing() {
			base.OnInitResizing();
			Rect bounds = GetItemBounds(Owner.GetViewElement(BackgroundGroup));
			View.ResizingWindowHelper.UpdateBackground(bounds);
		}
	}
	class AutoHideResizingPreviewHelper : BaseResizingPreviewHelper {
		public AutoHideResizingPreviewHelper(LayoutView view)
			: base(view) {
		}
		SWC.Dock Dock { get { return AutoHidePaneElement.DockType; } }
		AutoHidePaneElement AutoHidePaneElement { get { return Element as AutoHidePaneElement; } }
		protected override Rect GetInitialAdornerBounds() {
			return GetItemBounds(Element);
		}
		protected override Rect GetAdornerBounds(Point change) {
			change = new Point(change.X - StartPoint.X, change.Y - StartPoint.Y);
			Rect newPointerBounds = new Rect(InitialBounds.TopLeft(), InitialBounds.BottomRight());
			if(!newPointerBounds.IsEmpty) {
				switch(Dock) {
					case SWC.Dock.Left:
						newPointerBounds.Width += change.X;
						break;
					case SWC.Dock.Right:
						RectHelper.SetLeft(ref newPointerBounds, newPointerBounds.Left + change.X);
						break;
					case SWC.Dock.Top:
						newPointerBounds.Height += change.Y;
						break;
					case SWC.Dock.Bottom:
						RectHelper.SetTop(ref newPointerBounds, newPointerBounds.Top + change.Y);
						break;
				}
			}
			return newPointerBounds;
		}
#if !SILVERLIGHT
		protected override Rect GetWindowBounds(Point change) {
			Point screenPoint = View.ClientToScreen(change);
			return CorrectBounds(BoundsHelper.CalcBounds(screenPoint));
		}
		protected override Rect GetInitialWindowBounds() {
			Point screenPoint = View.ClientToScreen(StartPoint);
			return CorrectBounds(BoundsHelper.CalcBounds(screenPoint));
		}
#endif
		protected override UIElement GetUIElement() {
			return new AutoHideResizePointer() { Dock = Dock };
		}
	}
	class MDIDocumentResizingPreviewHelper : BaseResizingPreviewHelper {
		public MDIDocumentResizingPreviewHelper(LayoutView view)
			: base(view) {
		}
		protected override Core.FloatingMode Mode {
			get { return Core.FloatingMode.Adorner; }
		}
		protected override Rect GetInitialAdornerBounds() {
			return GetItemBounds(Element);
		}
		protected override Rect GetAdornerBounds(Point change) {
			return DevExpress.Xpf.Layout.Core.ResizeHelper.CalcResizing(InitialBounds, StartPoint, change, MinSize, MaxSize);
		}
		protected override UIElement GetUIElement() {
			return new FloatingResizePointer();
		}
	}
	class FloatingResizingPreviewHelper : BaseResizingPreviewHelper {
		public FloatingResizingPreviewHelper(LayoutView view)
			: base(view) {
		}
		protected override Rect GetInitialAdornerBounds() {
			Rect bounds = BoundsHelper.CalcBounds(StartPoint);
			Point client = View.ScreenToClient(bounds.TopLeft());
			RectHelper.SetLocation(ref bounds, client);
			return bounds;
		}
#if !SILVERLIGHT
		protected override Rect GetInitialWindowBounds() {
			Rect floatingBounds = BoundsHelper.CalcBounds(StartPoint);
			floatingBounds = CorrectBounds(floatingBounds);
			return floatingBounds;
		}
		protected override Rect GetWindowBounds(Point change) {
			Point screenPoint = View.ClientToScreen(change);
			return CorrectBounds(BoundsHelper.CalcBounds(screenPoint));
		}
#endif
		protected override Rect GetAdornerBounds(Point change) {
			Rect bounds1 = BoundsHelper.CalcBounds(View.ClientToScreen(change));
			Point client = View.ScreenToClient(bounds1.TopLeft());
			RectHelper.SetLocation(ref bounds1, client);
			return bounds1;
		}
		protected override UIElement GetUIElement() {
			return new FloatingResizePointer();
		}
	}
#if !SILVERLIGHT
	class ResizingOverlayWindow : Window, IDisposable {
		MatrixTransform transform;
		public ResizingOverlayWindow(UIElement container) {
			ResizeMode = ResizeMode.NoResize;
			WindowStyle = System.Windows.WindowStyle.None;
			ShowInTaskbar = false;
			AllowsTransparency = true;
			Background = null;
			ShowActivated = false;
			IsHitTestVisible = false;
			Window ownerWindow = Window.GetWindow(container);
			FrameworkElement owner = ownerWindow;
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(container);
			if(manager.OwnsFloatWindows) Owner = ownerWindow;
			if(owner == null || !manager.IsDescendantOf(owner)) {
				owner = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopLevelVisual(manager);
			}
			if(owner != null) {
				transform = manager.TransformToVisual(owner) as MatrixTransform;
				if(transform != null && !transform.Matrix.IsIdentity) {
					Matrix matrix = transform.Matrix;
					transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
				}
			}
		}
		public ResizingOverlayWindow() {
			ResizeMode = ResizeMode.NoResize;
			WindowStyle = System.Windows.WindowStyle.None;
			ShowInTaskbar = false;
			AllowsTransparency = true;
			Background = null;
			ShowActivated = false;
			IsHitTestVisible = false;
		}
		public void UpdateBounds(Rect bounds) {
			const short SWP_NOZORDER = 0X4;
			const int SWP_SHOWWINDOW = 0x0040;
			const short SWP_NOACTIVATE = 0x0010;
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			if(presentationSource != null)
				bounds.Transform(PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice);
			if(transform != null && !transform.Matrix.IsIdentity) {
				Point sz = transform.Transform(new Point(bounds.Width, bounds.Height));
				bounds.Width = sz.X;
				bounds.Height = sz.Y;
			}
			var hwndSource = System.Windows.Interop.HwndSource.FromVisual(this) as System.Windows.Interop.HwndSource;
			if(hwndSource != null) {
				NativeHelper.SetWindowPosSafe(hwndSource.Handle, IntPtr.Zero, (int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height, SWP_NOACTIVATE | SWP_NOZORDER | SWP_SHOWWINDOW);
			}
			else {
				Left = bounds.X;
				Top = bounds.Y;
				Width = bounds.Width;
				Height = bounds.Height;
			}
		}
		#region IDisposable Members
		bool isDisposingCore;
		void IDisposable.Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				Close();
			}
			GC.SuppressFinalize(this);
		}
		#endregion
	}
#endif
	public class ResizingWindowHelper : IDisposable {
		public LayoutView View { get; private set; }
		public ResizingWindowHelper(LayoutView view) {
			View = view;
			Container = View.Container;
		}
		UIElement Container;
#if !SILVERLIGHT
		ResizingOverlayWindow windowCore;
		ResizingOverlayWindow Window {
			get {
				if(windowCore == null) {
					windowCore = new ResizingOverlayWindow(Container);
				}
				return windowCore;
			}
		}
		void ShowWindowInBounds(Rect bounds) {
			Window.UpdateBounds(bounds);
			Window.Show();
		}
		void UpdateWindowBounds(Rect bounds) {
			Window.UpdateBounds(bounds);
		}
		void ShowResizeOverlayWindow(object content, Rect initialBounds, UIElement owner) {
			Window.Content = content;
			ShowWindowInBounds(initialBounds);
			windowShowCount++;
		}
#endif
		#region IDisposable Members
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Container = null;
#if !SILVERLIGHT
				Ref.Dispose(ref windowCore);
#endif
			}
			GC.SuppressFinalize(this);
		}
		#endregion
		Core.FloatingMode Mode;
		ShadowResizeAdorner Adorner;
		int windowShowCount = 0;
		bool windowShown { get { return windowShowCount > 0; } }
		public void ShowResizeWindow(UIElement Resizer, Rect InitialBounds, Core.FloatingMode mode) {
			Mode = mode;
			if(Mode == Core.FloatingMode.Adorner) {
				if(View == null) return;
				View.AdornerHelper.TryShowAdornerWindow(true);
				Adorner = View.AdornerHelper.GetShadowResizeAdorner();
				if(Adorner != null) {
					Adorner.StartResizing(Resizer, InitialBounds);
					Adorner.Update(true);
				}
			}
			else {
#if !SILVERLIGHT
				ShowResizeOverlayWindow(Resizer, InitialBounds, View.Container);
#endif
			}
		}
		public void UpdateResizeWindow(Rect bounds1) {
			if(Mode == Core.FloatingMode.Adorner) {
				if(Adorner != null) {
					Adorner.Resize(bounds1);
				}
			}
			else {
#if !SILVERLIGHT
				UpdateWindowBounds(bounds1);
#endif
			}
		}
		public void HideResizeWindow() {
			if(Mode == Core.FloatingMode.Adorner) {
				if(Adorner != null) {
					Adorner.EndResizing();
					Adorner.Update(false);
				}
				if(View != null) {
					View.AdornerHelper.TryHideAdornerWindow();
				}
				Adorner = null;
			}
			else {
#if !SILVERLIGHT
				if(windowCore != null) {
					Window.Hide();
					windowShowCount--;
				}
#endif
			}
		}
		public void UpdateBackground(Rect bounds) {
			if(Adorner != null)
				Adorner.ShowBackground(bounds);
		}
		internal void Reset() {
			if(windowShown)
				HideResizeWindow();
		}
	}
}
