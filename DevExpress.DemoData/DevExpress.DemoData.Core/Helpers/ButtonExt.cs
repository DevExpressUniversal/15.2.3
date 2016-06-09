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
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
namespace DevExpress.DemoData.Helpers {
	public static class IsMouseOverHelper {
		#region Dependency Properties
		public static readonly DependencyProperty EnableIsMouseOverProperty;
		public static readonly DependencyProperty IsMouseOverProperty;
		public static readonly DependencyProperty IsMouseOrStylusOverProperty;
		static IsMouseOverHelper() {
			Type ownerType = typeof(IsMouseOverHelper);
			EnableIsMouseOverProperty = DependencyProperty.RegisterAttached("EnableIsMouseOver", typeof(object), ownerType, new PropertyMetadata(null, RaiseEnableIsMouseOverChanged));
			IsMouseOverProperty = DependencyProperty.RegisterAttached("IsMouseOver", typeof(bool), ownerType, new PropertyMetadata(false, RaiseIsMouseOverChanged));
			IsMouseOrStylusOverProperty = DependencyProperty.RegisterAttached("IsMouseOrStylusOver", typeof(bool), ownerType, new PropertyMetadata(false, RaiseIsMouseOrStylusOverChanged));
			EventManager.RegisterClassHandler(typeof(UIElement), Mouse.LostMouseCaptureEvent, (MouseEventHandler)OnLostMouseCapture);
		}
		#endregion
		static Dictionary<TargetWeakReference, bool> listenElements = new Dictionary<TargetWeakReference, bool>();
		public static object GetEnableIsMouseOver(UIElement e) { return e.GetValue(EnableIsMouseOverProperty); }
		public static void SetEnableIsMouseOver(UIElement e, object v) { e.SetValue(EnableIsMouseOverProperty, v); }
		public static bool GetIsMouseOver(UIElement e) { return (bool)e.GetValue(IsMouseOverProperty); }
		private static void SetIsMouseOver(UIElement e, bool v) { e.SetValue(IsMouseOverProperty, v); }
		public static bool GetIsMouseOrStylusOver(UIElement e) { return (bool)e.GetValue(IsMouseOrStylusOverProperty); }
		public static void SetIsMouseOrStylusOver(UIElement e, bool v) { e.SetValue(IsMouseOrStylusOverProperty, v); }
		static void OnLostMouseCapture(object sender, MouseEventArgs e) {
			foreach(TargetWeakReference w in new List<TargetWeakReference>(listenElements.Keys)) {
				UIElement element = w.Target as UIElement;
				if(element == null) {
					listenElements.Remove(w);
					continue;
				}
				if(!element.IsMouseOver) {
					if(element.Dispatcher.CheckAccess())
						SetIsMouseOrStylusOver(element, false);
					else
						element.Dispatcher.BeginInvoke((Action<UIElement, bool>)SetIsMouseOrStylusOver, element, false);
				}
			}
		}
		static void RaiseIsMouseOrStylusOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement element = (UIElement)d;
			bool newValue = (bool)e.NewValue;
			SetIsMouseOver(element, newValue && !element.IsStylusOver);
		}
		static void RaiseIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement element = (UIElement)d;
			bool newValue = (bool)e.NewValue;
			Action<bool> onIsMouseOverChanged = GetEnableIsMouseOver(element) as Action<bool>;
			if(onIsMouseOverChanged != null)
				onIsMouseOverChanged(newValue);
		}
		static void RaiseEnableIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement element = (UIElement)d;
			if(e.NewValue != null) {
				listenElements.Add(new TargetWeakReference(element), true);
				element.MouseEnter += OnElementMouseEnter;
				element.MouseLeave += OnElementMouseLeave;
				element.IsHitTestVisibleChanged += OnIsHitTestVisibleChanged;
			} else {
				listenElements.Remove(new TargetWeakReference(element));
				element.MouseEnter -= OnElementMouseEnter;
				element.MouseLeave -= OnElementMouseLeave;
				element.IsHitTestVisibleChanged -= OnIsHitTestVisibleChanged;
			}
		}
		static void OnElementMouseLeave(object sender, MouseEventArgs e) {
			UIElement element = (UIElement)sender;
			if(Mouse.Captured == null)
				SetIsMouseOrStylusOver(element, false);
		}
		static void OnElementMouseEnter(object sender, MouseEventArgs e) {
			UIElement element = (UIElement)sender;
			SetIsMouseOrStylusOver(element, true);
		}
		static void OnIsHitTestVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UIElement element = (UIElement)sender;
			if(!(bool)e.NewValue)
				SetIsMouseOrStylusOver(element, false);
		}
	}
	public interface IButtonExt {
		void PerformClick();
		void PerformTap();
		bool IsPressedByStylus { get; set; }
	}
	public enum ButtonCommonState { Normal, MouseOver, Pressed, Disabled }
	public class ButtonExt : Button, IButtonExt {
		#region Dependency Properties
		public static readonly DependencyProperty CommonStateProperty;
		public static readonly DependencyProperty IsMouseOverExtProperty;
		public static readonly DependencyProperty InvokeCommandExtOnClickProperty;
		public static readonly DependencyProperty InvokeCommandExtOnTapProperty;
		public static readonly DependencyProperty CommandExtProperty;
		public static readonly DependencyProperty CommandExtParameterProperty;
		public static readonly DependencyProperty InvokeCommandExtOnRightClickProperty;
		public static readonly DependencyProperty IsEnabledExtProperty;
		static readonly DependencyProperty VisibilityListenProperty;
		static ButtonExt() {
			Type ownerType = typeof(ButtonExt);
			CommonStateProperty = DependencyProperty.Register("CommonState", typeof(ButtonCommonState), ownerType, new PropertyMetadata(ButtonCommonState.Normal, RaiseCommonStateChanged));
			IsMouseOverExtProperty = DependencyProperty.Register("IsMouseOverExt", typeof(bool), ownerType, new PropertyMetadata(false, OnIsMouseOverExtChanged));
			VisibilityListenProperty = DependencyProperty.Register("VisibilityListen", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed, RaiseVisibilityListenChanged));
			InvokeCommandExtOnClickProperty = DependencyProperty.Register("InvokeCommandExtOnClick", typeof(bool), ownerType, new PropertyMetadata(true));
			InvokeCommandExtOnTapProperty = DependencyProperty.Register("InvokeCommandExtOnTap", typeof(bool), ownerType, new PropertyMetadata(true));
			CommandExtProperty = DependencyProperty.Register("CommandExt", typeof(ICommand), ownerType, new PropertyMetadata(null, OnCommandExtChanged));
			CommandExtParameterProperty = DependencyProperty.Register("CommandExtParameter", typeof(object), ownerType, new PropertyMetadata(null, OnCommandExtParameterChanged));
			InvokeCommandExtOnRightClickProperty = DependencyProperty.Register("InvokeCommandExtOnRightClick", typeof(bool), ownerType, new PropertyMetadata(false));
			IsEnabledExtProperty = DependencyProperty.Register("IsEnabledExt", typeof(bool), ownerType, new PropertyMetadata(true, OnIsEnabledExtChanged));
		}
		ButtonCommonState commonStateValue = ButtonCommonState.Normal;
		static void RaiseCommonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonExt)d).commonStateValue = (ButtonCommonState)e.NewValue;
		}
		static void RaiseVisibilityListenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonExt)d).RaiseVisibilityChanged(e);
		}
		static void OnIsMouseOverExtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonExt)d).OnIsMouseOverExtChanged(e);
		}
		static void OnCommandExtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonExt)d).OnCommandExtChanged(e);
		}
		static void OnCommandExtParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonExt)d).OnCommandExtParameterChanged(e);
		}
		static void OnIsEnabledExtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ButtonExt)d).OnIsEnabledExtChanged(e);
		}
		#endregion
		bool stylusMoved;
		bool skipClick = true;
		public ButtonExt() {
			SetBinding(VisibilityListenProperty, new Binding("Visibility") { Source = this, Mode = BindingMode.OneWay });
			IsEnabledChanged += OnIsEnabledChanged;
			IsMouseOverHelper.SetEnableIsMouseOver(this, (Action<bool>)OnIsMouseOverChanged);
			MouseDoubleClick += OnMouseDoubleClick;
			MouseLeftButtonDown += OnMouseLeftButtonDown;
			StylusDown += OnStylusDown;
			StylusUp += OnStylusUp;
			StylusMove += OnStylusMove;
			MouseRightButtonDown += OnMouseRightButtonDown;
		}
		public event DepPropertyChangedEventHandler VisibilityChanged;
		public ButtonCommonState CommonState { get { return commonStateValue; } private set { SetValue(CommonStateProperty, value); } }
		public bool IsMouseOverExt { get { return (bool)GetValue(IsMouseOverExtProperty); } private set { SetValue(IsMouseOverExtProperty, value); } }
		public bool InvokeCommandExtOnClick { get { return (bool)GetValue(InvokeCommandExtOnClickProperty); } set { SetValue(InvokeCommandExtOnClickProperty, value); } }
		public bool InvokeCommandExtOnTap { get { return (bool)GetValue(InvokeCommandExtOnTapProperty); } set { SetValue(InvokeCommandExtOnTapProperty, value); } }
		public bool InvokeCommandExtOnRightClick { get { return (bool)GetValue(InvokeCommandExtOnRightClickProperty); } set { SetValue(InvokeCommandExtOnRightClickProperty, value); } }
		public ICommand CommandExt { get { return (ICommand)GetValue(CommandExtProperty); } set { SetValue(CommandExtProperty, value); } }
		public object CommandExtParameter { get { return GetValue(CommandExtParameterProperty); } set { SetValue(CommandExtParameterProperty, value); } }
		public bool IsEnabledExt { get { return (bool)GetValue(IsEnabledExtProperty); } private set { SetValue(IsEnabledExtProperty, value); } }
		public bool IsPressedByStylus { get; set; }
		public virtual void PerformClick() {
			if(InvokeCommandExtOnClick && CommandExt != null && CommandExt.CanExecute(CommandExtParameter))
				CommandExt.Execute(CommandExtParameter);
		}
		public virtual void PerformTap() {
			if(InvokeCommandExtOnTap && CommandExt != null && CommandExt.CanExecute(CommandExtParameter))
				CommandExt.Execute(CommandExtParameter);
		}
		public virtual void PerformRightClick() {
			if(InvokeCommandExtOnRightClick && CommandExt != null && CommandExt.CanExecute(CommandExtParameter))
				CommandExt.Execute(CommandExtParameter);
		}
		protected virtual void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			PerformRightClick();
		}
		protected virtual void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			skipClick = true;
		}
		protected virtual void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if(e.StylusDevice != null) return;
			if(skipClick) {
				skipClick = false;
				return;
			}
			PerformClick();
		}
		protected virtual void OnStylusDown(object sender, StylusDownEventArgs e) {
			stylusMoved = false;
		}
		protected virtual void OnStylusUp(object sender, StylusEventArgs e) {
			if(!stylusMoved)
				PerformTap();
		}
		protected virtual void OnStylusMove(object sender, StylusEventArgs e) {
			stylusMoved = true; 
		}
		void OnIsMouseOverChanged(bool isMouseOver) {
			IsMouseOverExt = isMouseOver;
			UpdateStates();
		}
		protected virtual void RaiseVisibilityChanged(DependencyPropertyChangedEventArgs e) {
			if(VisibilityChanged != null)
				VisibilityChanged(this, new DepPropertyChangedEventArgs(e));
		}
		protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsPressedChanged(e);
			UpdateStates();
		}
		protected virtual void OnIsMouseOverExtChanged(DependencyPropertyChangedEventArgs e) {}
		protected virtual void OnCommandExtChanged(DependencyPropertyChangedEventArgs e) {
			ICommand oldValue = (ICommand)e.OldValue;
			ICommand newValue = (ICommand)e.NewValue;
			if(oldValue != null)
				oldValue.CanExecuteChanged -= OnCommandExtCanExecuteChanged;
			if(newValue != null)
				newValue.CanExecuteChanged += OnCommandExtCanExecuteChanged;
			UpdateIsEnabledExt();
		}
		void OnCommandExtCanExecuteChanged(object sender, EventArgs e) {
			UpdateIsEnabledExt();
		}
		protected virtual void OnCommandExtParameterChanged(DependencyPropertyChangedEventArgs e) {
			UpdateIsEnabledExt();
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateIsEnabledExt();
		}
		void UpdateIsEnabledExt() {
			IsEnabledExt = IsEnabled && (CommandExt == null || CommandExt.CanExecute(CommandExtParameter));
		}
		void OnIsEnabledExtChanged(DependencyPropertyChangedEventArgs e) {
			UpdateStates();
		}
		void UpdateStates() {
			if(!IsEnabledExt) {
				CommonState = ButtonCommonState.Disabled;
			} else if(IsPressed) {
				CommonState = ButtonCommonState.Pressed;
			} else if(IsMouseOverExt) {
				CommonState = ButtonCommonState.MouseOver;
			} else {
				CommonState = ButtonCommonState.Normal;
			}
		}
	}
	public enum ToggleButtonCheckState { Unchecked, Checked }
	public class ToggleButtonExt : ButtonExt {
		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButtonExt), new PropertyMetadata(false,
				(d, e) => ((ToggleButtonExt)d).OnIsCheckedChanged(e)));
		public static readonly DependencyProperty CheckStateProperty =
			DependencyProperty.Register("CheckState", typeof(ToggleButtonCheckState), typeof(ToggleButtonExt), new PropertyMetadata(ToggleButtonCheckState.Unchecked));
		public static readonly DependencyProperty ToggleOnClickProperty =
			DependencyProperty.Register("ToggleOnClick", typeof(bool), typeof(ToggleButtonExt), new PropertyMetadata(true));
		public static readonly DependencyProperty ToggleOnTapProperty =
			DependencyProperty.Register("ToggleOnTap", typeof(bool), typeof(ToggleButtonExt), new PropertyMetadata(true));
		public static readonly DependencyProperty ToggleOnMouseOverProperty =
			DependencyProperty.Register("ToggleOnMouseOver", typeof(bool), typeof(ToggleButtonExt), new PropertyMetadata(false));
		public bool IsChecked { get { return (bool)GetValue(IsCheckedProperty); } set { SetValue(IsCheckedProperty, value); } }
		public ToggleButtonCheckState CheckState { get { return (ToggleButtonCheckState)GetValue(CheckStateProperty); } set { SetValue(CheckStateProperty, value); } }
		public bool ToggleOnClick { get { return (bool)GetValue(ToggleOnClickProperty); } set { SetValue(ToggleOnClickProperty, value); } }
		public bool ToggleOnTap { get { return (bool)GetValue(ToggleOnTapProperty); } set { SetValue(ToggleOnTapProperty, value); } }
		public bool ToggleOnMouseOver { get { return (bool)GetValue(ToggleOnMouseOverProperty); } set { SetValue(ToggleOnMouseOverProperty, value); } }
		public override void PerformClick() {
			base.PerformClick();
			if(ToggleOnClick)
				IsChecked = true;
		}
		public override void PerformTap() {
			base.PerformTap();
			if(ToggleOnTap)
				IsChecked = true;
		}
		protected override void OnIsMouseOverExtChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsMouseOverExtChanged(e);
			bool newValue = (bool)e.NewValue;
			if(ToggleOnMouseOver)
				IsChecked = newValue;
		}
		protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e) {
			UpdateCheckState();
		}
		void UpdateCheckState() {
			if(IsChecked)
				CheckState = ToggleButtonCheckState.Checked;
			else
				CheckState = ToggleButtonCheckState.Unchecked;
		}
	}
	public enum ListBoxItemCommonState { Normal, MouseOver, Disabled }
	public enum ListBoxItemSelectionState { Unselected, MouseOverUnselected, Selected, MouseOverSelected }
	public class ListBoxItemExt : ListBoxItem {
		#region Dependency Properties
		public static readonly DependencyProperty CommonStateProperty;
		public static readonly DependencyProperty SelectionStateProperty;
		static readonly DependencyProperty VisibilityListenProperty;
		static readonly DependencyProperty IsSelectedListenProperty;
		static ListBoxItemExt() {
			Type ownerType = typeof(ListBoxItemExt);
			CommonStateProperty = DependencyProperty.Register("CommonState", typeof(ListBoxItemCommonState), ownerType, new PropertyMetadata(ListBoxItemCommonState.Normal, RaiseCommonStateChanged));
			SelectionStateProperty = DependencyProperty.Register("SelectionState", typeof(ListBoxItemSelectionState), ownerType, new PropertyMetadata(ListBoxItemSelectionState.Unselected, RaiseSelectionStateChanged));
			VisibilityListenProperty = DependencyProperty.Register("VisibilityListen", typeof(Visibility), ownerType, new PropertyMetadata(Visibility.Collapsed, RaiseVisibilityListenChanged));
			IsSelectedListenProperty = DependencyProperty.Register("IsSelectedListen", typeof(bool), ownerType, new PropertyMetadata(false, RaiseIsSelectedListenChanged));
		}
		ListBoxItemCommonState commonStateValue = ListBoxItemCommonState.Normal;
		ListBoxItemSelectionState selectionStateValue = ListBoxItemSelectionState.Unselected;
		static void RaiseCommonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxItemExt)d).commonStateValue = (ListBoxItemCommonState)e.NewValue;
		}
		static void RaiseSelectionStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxItemExt)d).selectionStateValue = (ListBoxItemSelectionState)e.NewValue;
		}
		static void RaiseVisibilityListenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxItemExt)d).RaiseVisibilityChanged(e);
		}
		static void RaiseIsSelectedListenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxItemExt)d).RaiseIsSelectedChanged(e);
		}
		#endregion
		bool isMouseOver = false;
		public ListBoxItemExt() {
			SetBinding(VisibilityListenProperty, new Binding("Visibility") { Source = this, Mode = BindingMode.OneWay });
			SetBinding(IsSelectedListenProperty, new Binding("IsSelected") { Source = this, Mode = BindingMode.OneWay });
			IsEnabledChanged += OnIsEnabledChanged;
			IsMouseOverHelper.SetEnableIsMouseOver(this, (Action<bool>)OnIsMouseOverChanged);
		}
		public event DepPropertyChangedEventHandler VisibilityChanged;
		public event DepPropertyChangedEventHandler IsSelectedChanged;
		public ListBoxItemCommonState CommonState { get { return commonStateValue; } private set { SetValue(CommonStateProperty, value); } }
		public ListBoxItemSelectionState SelectionState { get { return selectionStateValue; } private set { SetValue(SelectionStateProperty, value); } }
		protected virtual void RaiseVisibilityChanged(DependencyPropertyChangedEventArgs e) {
			if(VisibilityChanged != null)
				VisibilityChanged(this, new DepPropertyChangedEventArgs(e));
		}
		void OnIsMouseOverChanged(bool isMouseOver) {
			this.isMouseOver = isMouseOver;
			UpdateStates();
		}
		void RaiseIsSelectedChanged(DependencyPropertyChangedEventArgs e) {
			if(IsSelectedChanged != null)
				IsSelectedChanged(this, new DepPropertyChangedEventArgs(e));
			UpdateStates();
		}
		void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateStates();
		}
		void UpdateStates() {
			if(!IsEnabled) {
				CommonState = ListBoxItemCommonState.Disabled;
			} else if(isMouseOver) {
				CommonState = ListBoxItemCommonState.MouseOver;
			} else {
				CommonState = ListBoxItemCommonState.Normal;
			}
			if(IsSelected) {
				if(isMouseOver) {
					SelectionState = ListBoxItemSelectionState.MouseOverSelected;
				} else {
					SelectionState = ListBoxItemSelectionState.Selected;
				}
			} else {
				if(isMouseOver) {
					SelectionState = ListBoxItemSelectionState.MouseOverUnselected;
				} else {
					SelectionState = ListBoxItemSelectionState.Unselected;
				}
			}
		}
	}
	public class ListBoxEx : ListBox {
		protected override DependencyObject GetContainerForItemOverride() {
			return new ListBoxItemExt();
		}
	}
}
