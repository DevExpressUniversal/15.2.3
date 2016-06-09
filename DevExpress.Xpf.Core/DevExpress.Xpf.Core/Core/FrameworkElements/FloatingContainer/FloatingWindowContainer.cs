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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public class FloatingWindowContainer : FloatingContainer {
		static FloatingWindowContainer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingWindowContainer), new FrameworkPropertyMetadata(typeof(FloatingWindowContainer)));
		}
		public WindowContentHolder Window { get; private set; }
		protected override void CloseCore() {
			base.CloseCore();
			if(Window == null || IsClosingCanceled) return;
			Window.Closing -= Window_Closing;
			Window.KeyDown -= OnWindowKeyDown;
			Window.Close();
			Window = null;
		}
		protected override FloatingMode GetFloatingMode() {
			return FloatingMode.Window;
		}
		protected virtual WindowContentHolder CreateWindow(){
			return new WindowContentHolder(this);
		}
		protected override UIElement CreateContentContainer() {
			Window = CreateWindow();
			Window.Focusable = ContainerFocusable;
			Window.Closing += Window_Closing;
			Window.KeyDown += OnWindowKeyDown;
			OnSizeToContentChangedCore(SizeToContent);
			return Window;
		}
		public override void Activate() {
			if(Window != null) Window.Activate();
		}
		internal bool allowProcessClosing = true;
		void Window_Closing(object sender, CancelEventArgs e) {
			if(!allowProcessClosing) return;
			e.Cancel = true;
			IsOpen = false;
		}
		protected virtual void OnWindowKeyDown(object sender, KeyEventArgs e) {
			if(CloseOnEscape && e.Key == Key.Escape) {
				e.Handled = true;
				ProcessHiding();
			}
		}
		protected override bool IsAlive {
			get { return Window != null; }
		}
		int lockFloatingBoundsChanging = 0;
		protected override void UpdateFloatingBoundsCore(Rect bounds) {
			if(!IsAlive || lockFloatingBoundsChanging > 0) return;
			lockFloatingBoundsChanging++;
			bounds = new Rect(bounds.Location, EnsureAutoSize(bounds.Size));
			ActualSize = bounds.Size;
			Window.UseScreenCoordinates = UseScreenCoordinates;
			Window.SetFloatingBounds(Owner, bounds);
			lockFloatingBoundsChanging--;
		}
		Size EnsureAutoSize(Size size) {
			if(SizeToContent != System.Windows.SizeToContent.Manual) {
				Size autoSize = GetLayoutAutoSize();
				if(autoSize != Size.Empty) {
					double w = autoSize.Width; double h = autoSize.Height;
					h = Math.Min(System.Windows.SystemParameters.WorkArea.Height, h);
					w = Math.Min(System.Windows.SystemParameters.WorkArea.Width, w);
					if(SizeToContent == System.Windows.SizeToContent.Width)
						w = MeasureAutoSize(size.Width, w, Window.Width);
					if(SizeToContent == System.Windows.SizeToContent.Height)
						h = MeasureAutoSize(size.Height, h, Window.Height);
					if(SizeToContent == System.Windows.SizeToContent.WidthAndHeight) {
						w = MeasureAutoSize(size.Width, w, Window.Width);
						h = MeasureAutoSize(size.Height, h, Window.Height);
					}
					size = new Size(w, h);
					if(FloatSize != size) {
						lockFloatingBoundsChanging++;
						if(FloatSize != new Size(0, 0))
							FloatSize = size;
						if(Window.Width != w)
							Window.Width = w;
						if(Window.Height != h)
							Window.Height = h;
						lockFloatingBoundsChanging--;
					}
				}
			}
			return size;
		}
		protected override void OnSizeToContentChangedCore(SizeToContent newVal) {
			base.OnSizeToContentChangedCore(newVal);
			if(Window != null && !AllowSizing)
				Window.SizeToContent = newVal;
		}
		protected virtual void UpdateStartupLocation() {
			Window.Width = FloatSize.Width;
			Window.Height = FloatSize.Height;
			Window.SetStartupLocation(Owner, ContainerStartupLocation);
		}
		protected override void UpdateIsOpenCore(bool isOpen) {
			if(isOpen) {
			   isAutoSizeUpdating++;
			   bool fAsyncShowModal = false;
				try {
					UpdateStartupLocation();
					Window.ShowActivated = ShowActivated;
					if(AllowShowAnimations) Window.Opacity = 0;
					if(ShowModal && TryAsyncShowModal()) {
						fAsyncShowModal = true;
						return;
					}
					Window.Show();
				}
				finally {
					if(!fAsyncShowModal)
						OnOpened();
					--isAutoSizeUpdating;
				}
			}
			else Window.Hide();
		}
		void OnOpened() {
			UpdateFloatingBoundsCore(new Rect(FloatLocation, FloatSize));
			if(ContainerStartupLocation == WindowStartupLocation.CenterOwner && Window.Owner != null) {
				Rect bounds = GetScreenBounds(Window);
				Rect ownerBounds = GetScreenBounds(Window.Owner);
				double left = ownerBounds.Left + (ownerBounds.Width - bounds.Width) * 0.5;
				double top = ownerBounds.Top + (ownerBounds.Height - bounds.Height) * 0.5;
				System.Windows.Media.Matrix transformTo;
				Rect screen = GetScreen(Window.Owner, out transformTo);
				Point location = transformTo.Transform(new Point(left, top));
				if(location.X + Window.Width > screen.Right)
					location.X = screen.Right - Window.Width;
				location.X = Math.Max(screen.Left, location.X);
				if(location.Y + Window.Height > screen.Bottom)
					location.Y = screen.Bottom - Window.Height;
				location.Y = Math.Max(screen.Top, location.Y);
				Window.Left = location.X;
				Window.Top = location.Y;
				Window.CheckRelativeLocation();
			}
			EnsureMinSize();
			if(AllowShowAnimations) Window.BeginAnimation(OpacityProperty,
				new System.Windows.Media.Animation.DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(150))));
		}
		bool TryAsyncShowModal() {
			bool shouldInvokeShowModal = false;
			if(Owner != null) {
				bool isInFloatingContainer = FloatingContainer.GetFloatingContainer(Owner) != null;
				bool isInElementHost = Owner.GetType().Name.EndsWith("AvalonAdapter"); 
				shouldInvokeShowModal = isInFloatingContainer || isInElementHost;
			}
			if (shouldInvokeShowModal)
				Dispatcher.BeginInvoke(new System.Action(
					delegate() {
						if (!Window.IsVisible) {
							isAutoSizeUpdating++;
							Window.IsVisibleChanged += Window_IsVisibleChanged;
							Window.ShowDialog();
						}
					}));
			return shouldInvokeShowModal;
		}
		void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Window.IsVisibleChanged -= Window_IsVisibleChanged;
			OnOpened();
			isAutoSizeUpdating--;
		}
		protected override void AddDecoratorToContentContainer(NonLogicalDecorator decorator) {
			Window.Content = decorator;
		}
		protected override Point ScreenToLogical(Point point) {
			PresentationSource pSource = PresentationSource.FromVisual(Window);
			if(pSource != null)
				return pSource.CompositionTarget.TransformFromDevice.Transform(point);
			return point;
		}
		static Rect GetScreen(Window w, out System.Windows.Media.Matrix transformTo) {
			Rect wBounds = new Rect(w.Left, w.Top, w.Width, w.Height);
			transformTo = System.Windows.Media.Matrix.Identity;
			PresentationSource pSource = PresentationSource.FromVisual(w);
			if(pSource != null) {
				transformTo = pSource.CompositionTarget.TransformToDevice;
				var transformFrom = pSource.CompositionTarget.TransformFromDevice;
				wBounds.Location = transformFrom.Transform(wBounds.Location);
				NativeMethods.RECT rect = GetScreenRect(ref wBounds);
				Point lt = transformFrom.Transform(new Point(rect.left, rect.top));
				Point br = transformFrom.Transform(new Point(rect.right, rect.bottom));
				rect.left = (int)Math.Floor(lt.X); rect.top = (int)Math.Floor(lt.Y);
				rect.right = (int)Math.Ceiling(br.X); rect.bottom = (int)Math.Ceiling(br.Y);
				wBounds = new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
			}
			return wBounds;
		}
		static Rect GetScreenBounds(Window w) {
			Rect wBounds = new Rect(w.Left, w.Top, w.Width, w.Height);
			PresentationSource pSource = PresentationSource.FromVisual(w);
			if(pSource != null) {
				var transformFrom = pSource.CompositionTarget.TransformFromDevice;
				wBounds.Location = transformFrom.Transform(wBounds.Location);
				if(w.WindowState == WindowState.Maximized) {
					NativeMethods.RECT rect = GetScreenRect(ref wBounds);
					if(pSource != null) {
						Point lt = transformFrom.Transform(new Point(rect.left, rect.top));
						Point br = transformFrom.Transform(new Point(rect.right, rect.bottom));
						rect.left = (int)Math.Floor(lt.X); rect.top = (int)Math.Floor(lt.Y);
						rect.right = (int)Math.Ceiling(br.X); rect.bottom = (int)Math.Ceiling(br.Y);
					}
					wBounds = new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
				}
			}
			return wBounds;
		}
		[System.Security.SecuritySafeCritical]
		static NativeMethods.RECT GetScreenRect(ref Rect wBounds) {
			NativeMethods.RECT rect = new NativeMethods.RECT(wBounds);
			IntPtr handle = NativeMethods.MonitorFromRect(ref rect, 2);
			if(handle != IntPtr.Zero) {
				NativeMethods.MONITORINFOEX info = new NativeMethods.MONITORINFOEX();
				NativeMethods.GetMonitorInfo(new System.Runtime.InteropServices.HandleRef(null, handle), info);
				rect = info.rcWork;
			}
			return rect;
		}
	}
}
