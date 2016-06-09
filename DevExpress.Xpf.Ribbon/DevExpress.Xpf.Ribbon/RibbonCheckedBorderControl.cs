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
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using System.Windows;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonCheckedBorderControl : ContentControl {
		#region static
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty MergeCheckedStatesProperty;
		public static readonly DependencyProperty IsLeftButtonPressedProperty;
		public static readonly DependencyProperty UseAppFocusValueProperty;
		public static readonly DependencyProperty AppFocusValueProperty;
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty IsInRibbonWindowProperty;
		static RibbonCheckedBorderControl() {
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool?), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));
			MergeCheckedStatesProperty = DependencyPropertyManager.Register("MergeCheckedStates", typeof(bool), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnMergeCheckedStatesPropertyChanged)));
			IsLeftButtonPressedProperty = DependencyPropertyManager.Register("IsLeftButtonPressed", typeof(bool), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsLeftButtonPressedPropertyChanged)));
			UseAppFocusValueProperty = DependencyPropertyManager.Register("UseAppFocusValue", typeof(bool), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnUseAppFocusValuePropertyChanged)));
			AppFocusValueProperty = DependencyPropertyManager.Register("AppFocusValue", typeof(bool), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnAppFocusValuePropertyChanged)));
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(Colors.Transparent));
			IsInRibbonWindowProperty = DependencyPropertyManager.Register("IsInRibbonWindow", typeof(bool), typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsInRibbonWindowPropertyChanged)));
			IsEnabledProperty.OverrideMetadata(typeof(RibbonCheckedBorderControl), new FrameworkPropertyMetadata(true, OnIsEnabledPropertyChanged));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonCheckedBorderControl), typeof(RibbonCheckedBorderControlAutomationPeer), owner => new RibbonCheckedBorderControlAutomationPeer((RibbonCheckedBorderControl)owner));
		}
		static protected void OnIsCheckedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).OnIsCheckedChanged(e.OldValue as bool?);
		}
		static protected void OnMergeCheckedStatesPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).OnMergeCheckedStatesChanged((bool)e.OldValue);
		}
		static protected void OnIsLeftButtonPressedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).OnIsLeftButtonPressedChanged((bool)e.OldValue);
		}
		static protected void OnUseAppFocusValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).OnUseAppFocusValueChanged((bool)e.OldValue);
		}
		static protected void OnAppFocusValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).OnAppFocusValueChanged((bool)e.OldValue);
		}
		static protected void OnIsInRibbonWindowPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).IsInRibbonWindowChanged((bool)e.OldValue);
		}
		static protected void OnIsEnabledPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCheckedBorderControl)o).OnIsEnabledChanged((bool)e.OldValue);
		}		
		#endregion
		#region dep props
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public bool MergeCheckedStates {
			get { return (bool)GetValue(MergeCheckedStatesProperty); }
			set { SetValue(MergeCheckedStatesProperty, value); }
		}
		public bool IsLeftButtonPressed {
			get { return (bool)GetValue(IsLeftButtonPressedProperty); }
			set { SetValue(IsLeftButtonPressedProperty, value); }
		}
		public bool UseAppFocusValue {
			get { return (bool)GetValue(UseAppFocusValueProperty); }
			set { SetValue(UseAppFocusValueProperty, value); }
		}
		public bool AppFocusValue {
			get { return (bool)GetValue(AppFocusValueProperty); }
			set { SetValue(AppFocusValueProperty, value); }
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public bool IsInRibbonWindow {
			get { return (bool)GetValue(IsInRibbonWindowProperty); }
			set { SetValue(IsInRibbonWindowProperty, value); }
		}
		#endregion
		public RibbonCheckedBorderControl() {
		}
		public event EventHandler Click;
		protected virtual void OnIsCheckedChanged(bool? oldValue) {
			UpdateVisualStates();
		}
		protected virtual void OnMergeCheckedStatesChanged(bool oldValue) {
			UpdateVisualStates();
		}
		protected virtual void OnIsLeftButtonPressedChanged(bool oldValue) {
		}
		protected virtual void OnUseAppFocusValueChanged(bool oldValue) {
			UpdateVisualStates();
		}
		protected virtual void OnAppFocusValueChanged(bool oldValue) {
			UpdateVisualStates();
		}
		protected virtual void IsInRibbonWindowChanged(bool oldValue) {
			UpdateVisualStates();
		}
		protected virtual void OnIsEnabledChanged(bool oldValue) {
			UpdateVisualStates();
		}
		void UpdateVisualStates() {
			string state = "Normal";
			string checkedState = "Unchecked";
			if(IsMouseOver) {
				if(IsLeftButtonPressed) {
					state = "Pressed";
				}
				else {
					state = "Hover";
				}
			}
			if(IsChecked == true)
				checkedState = "Checked";
			if(IsChecked == null && !MergeCheckedStates)
				checkedState = "Indeterminate";
			if(MergeCheckedStates) {
				SetVisualState(state + checkedState);
			}
			else {
				SetVisualState(state);
				SetVisualState(checkedState);
			}
			if(UseAppFocusValue) {
				if(AppFocusValue) {
					SetVisualState("Focused");
				}
				else {
					SetVisualState("Unfocused");
				}
			}
			else {
				if(IsFocused)
					SetVisualState("Focused");
				else
					SetVisualState("Unfocused");
			}
			if(IsEnabled) {
				SetVisualState("Enabled");
			}
			else {
				SetVisualState("Disabled");
			}
			if(IsInRibbonWindow) {
				SetVisualState("RibbonWindow");
			}
			else {
				SetVisualState("Standalone");
			}
		}
		protected virtual void SetVisualState(string state) {
			VisualStateManager.GoToState(this, state, false);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			IsLeftButtonPressed = true;
			UpdateVisualStates();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if(IsLeftButtonPressed) {
				RaiseClickEvent();
			}
			IsLeftButtonPressed = false;
			UpdateVisualStates();
		}
		protected internal virtual void RaiseClickEvent() {
			if(Click != null)
				Click(this, new EventArgs());
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsLeftButtonPressed = false;
			UpdateVisualStates();
		}		
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateVisualStates();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualStates();
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			UpdateVisualStates();
		}
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			UpdateVisualStates();
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			UpdateVisualStates();
		}
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
			UpdateVisualStates();
		}
	}
}
