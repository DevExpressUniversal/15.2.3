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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FlyoutControl = DevExpress.Xpf.Editors.Flyout.FlyoutControl;
using FlyoutPlacement = DevExpress.Xpf.Editors.Flyout.FlyoutPlacement;
namespace DevExpress.Xpf.WindowsUI.Internal.Flyout {
	public abstract partial class FlyoutBase : Control {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IndicatorTargetProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty PlacementProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty PlacementTargetProperty;
		public static readonly DependencyProperty FlyoutStyleProperty;
		public static readonly DependencyProperty IsOpenProperty;
		public static readonly DependencyProperty ShowIndicatorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FlyoutProperty;
		static FlyoutBase() {
			var dProp = new DependencyPropertyRegistrator<FlyoutBase>();
			dProp.OverrideFrameworkMetadata(HorizontalAlignmentProperty, HorizontalAlignment.Center);
			dProp.Register("IndicatorTarget", ref IndicatorTargetProperty, (UIElement)null);
			dProp.Register("Placement", ref PlacementProperty, FlyoutPlacement.Top);
			dProp.Register("PlacementTarget", ref PlacementTargetProperty, (UIElement)null);
			dProp.Register("FlyoutStyle", ref FlyoutStyleProperty, (Style)null);
			dProp.Register("IsOpen", ref IsOpenProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((FlyoutBase)d).OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("ShowIndicator", ref ShowIndicatorProperty, false);
			dProp.RegisterAttachedInherited("Flyout", ref FlyoutProperty, (FlyoutBase)null);
			EventManager.RegisterClassHandler(typeof(MenuFlyout), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
			EventManager.RegisterClassHandler(typeof(MenuFlyout), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnPromotedMouseButton));
			EventManager.RegisterClassHandler(typeof(MenuFlyout), Mouse.MouseUpEvent, new MouseButtonEventHandler(OnPromotedMouseButton));
			EventManager.RegisterClassHandler(typeof(MenuFlyout), Keyboard.PreviewKeyboardInputProviderAcquireFocusEvent, new KeyboardInputProviderAcquireFocusEventHandler(OnPreviewKeyboardInputProviderAcquireFocus), true);
			EventManager.RegisterClassHandler(typeof(MenuFlyout), Keyboard.KeyboardInputProviderAcquireFocusEvent, new KeyboardInputProviderAcquireFocusEventHandler(OnKeyboardInputProviderAcquireFocus), true);
			FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(true));
			Control.IsTabStopProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(false));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(null));
			InputMethod.IsInputMethodSuspendedProperty.OverrideMetadata(typeof(MenuFlyout), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
		}
		internal static FlyoutBase GetFlyout(DependencyObject target) {
			return (FlyoutBase)target.GetValue(FlyoutProperty);
		}
		internal static void SetFlyout(DependencyObject target, FlyoutBase value) {
			target.SetValue(FlyoutProperty, value);
		}
		#endregion
		#region props
		FlyoutControl _FlyoutControl;
		public FlyoutControl FlyoutControl {
			get { return _FlyoutControl; }
			set {
				if(_FlyoutControl == value) return;
				FlyoutControl oldValue = _FlyoutControl;
				_FlyoutControl = value;
				OnFlyoutChanged(oldValue, value);
			}
		}
		internal FlyoutService Service { get; set; }
		internal IFlyoutEventListener Listener { get; set; }
		internal UIElement IndicatorTarget {
			get { return (UIElement)GetValue(IndicatorTargetProperty); }
			set { SetValue(IndicatorTargetProperty, value); }
		}
		internal FlyoutPlacement Placement {
			get { return (FlyoutPlacement)GetValue(PlacementProperty); }
			set { SetValue(PlacementProperty, value); }
		}
		internal UIElement PlacementTarget {
			get { return (UIElement)GetValue(PlacementTargetProperty); }
			set { SetValue(PlacementTargetProperty, value); }
		}
		public Style FlyoutStyle {
			get { return (Style)GetValue(FlyoutStyleProperty); }
			set { SetValue(FlyoutStyleProperty, value); }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public bool ShowIndicator {
			get { return (bool)GetValue(ShowIndicatorProperty); }
			set { SetValue(ShowIndicatorProperty, value); }
		}
		NonLogicalContentControl _NonLogicalContainer;
		NonLogicalContentControl NonLogicalContainer {
			get {
				if(_NonLogicalContainer == null) _NonLogicalContainer = new NonLogicalContentControl();
				return _NonLogicalContainer;
			}
		}
		#endregion
		public event System.EventHandler Opened;
		public event System.EventHandler Closed;
		protected FlyoutBase() {
			SetFlyout(this, this);
			AllowMouseCapture = true;
		}
		private void OnFlyoutChanged(DevExpress.Xpf.Editors.Flyout.FlyoutControl oldValue, DevExpress.Xpf.Editors.Flyout.FlyoutControl value) {
			if(oldValue != null) {
				oldValue.Opened -= OnFlyoutPopupOpened;
				oldValue.Closed -= OnFlyoutPopupClosed;
				this.ClearValue(FlyoutControl.FlyoutProperty);
			}
			if(value != null) {
				value.Opened += OnFlyoutPopupOpened;
				value.Closed += OnFlyoutPopupClosed;
				FlyoutControl.SetFlyout(this, value);
			}
		}
		protected virtual void OnFlyoutPopupClosed(object sender, System.EventArgs e) {
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<object>((x) =>
			{
				FlyoutBase cm = (FlyoutBase)x;
				if(!cm.IsOpen) {
					FocusManager.SetFocusedElement(cm, null);
				}
			}), this);
			IsMenuMode = false;
		}
		protected virtual void OnFlyoutPopupOpened(object sender, System.EventArgs e) {
			IsMenuMode = true;
		}
		void RaiseOnClosed() {
			var handler = Closed;
			if(handler != null) handler(this, System.EventArgs.Empty);
		}
		void RaiseOnOpened() {
			var handler = Opened;
			if(handler != null) handler(this, System.EventArgs.Empty);
		}
		protected virtual void OnOpened() {
			FlyoutControl.Style = FlyoutStyle;
			FlyoutControl.PlacementTarget = PlacementTarget;
			FlyoutControl.Settings = new DevExpress.Xpf.Editors.Flyout.FlyoutSettings() { Placement = Placement, IndicatorTarget = this.IndicatorTarget, ShowIndicator = ShowIndicator };
			NonLogicalContainer.Content = null;
			FlyoutControl.Content = NonLogicalContainer;
			NonLogicalContainer.Content = this;
			FlyoutControl.SetBinding(DevExpress.Xpf.Editors.Flyout.FlyoutControl.HorizontalAlignmentProperty, new Binding("HorizontalAlignment") { Source = this });
			FlyoutControl.SetBinding(DevExpress.Xpf.Editors.Flyout.FlyoutControl.IsOpenProperty, new Binding("IsOpen") { Source = this, Mode = BindingMode.TwoWay });
			RaiseOnOpened();
		}
		protected virtual void OnIsOpenChanged(bool oldValue, bool newValue) {
			if(newValue) {
				if(FlyoutControl != null) {
					OnOpened();
				}
			}
			else {
				if(FlyoutControl != null)
					FlyoutControl.ClearValue(DevExpress.Xpf.Editors.Flyout.FlyoutControl.IsOpenProperty);
				RaiseOnClosed();
			}
		}
		protected virtual bool CanClose { get { return true; } }
		protected virtual void CloseCore() {
			if(CanClose)
				IsOpen = false;
		}
		public void Close(bool onClickThrough) {
			bool shouldNotify = true;
			CloseCore();
			if(shouldNotify)
				Listener.Do(x => x.OnFlyoutClosed(onClickThrough));
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			e.Handled = true;
		}
		class NonLogicalContentControl : ContentControl {
			protected override void OnContentChanged(object oldContent, object newContent) { }
			protected override IEnumerator LogicalChildren {
				get { return new object[0].GetEnumerator(); }
			}
		}
	}
}
