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

using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.UIAutomation;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
namespace DevExpress.Xpf.WindowsUI {
	public class AppBarToggleButton : AppBarButton {
		#region static
		[ThreadStatic]
		private static Hashtable groupNameToElements;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty GroupNameProperty;
		static AppBarToggleButton() {
			var dProp = new DependencyPropertyRegistrator<AppBarToggleButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsChecked", ref IsCheckedProperty, false,
				(d, e) => ((AppBarToggleButton)d).OnIsCheckedChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("GroupName", ref GroupNameProperty, string.Empty, OnGroupNameChanged);
		}
		static void OnGroupNameChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			AppBarToggleButton button = (AppBarToggleButton)dObj;
			string newValue = e.NewValue as string;
			string oldValue = e.OldValue as string;
			if(!string.IsNullOrEmpty(oldValue))
				Unregister(oldValue, button);
			if(!string.IsNullOrEmpty(newValue))
				Register(newValue, button);
		}
		static void Register(string groupName, AppBarToggleButton button) {
			if(groupNameToElements == null)
				groupNameToElements = new Hashtable(1);
			lock(groupNameToElements) {
				ArrayList elements = (ArrayList)groupNameToElements[groupName];
				if(elements == null) {
					elements = new ArrayList(1);
					groupNameToElements[groupName] = elements;
				}
				else {
					PurgeGroup(elements, null);
				}
				elements.Add(new WeakReference(button));
			}
		}
		static void Unregister(string groupName, AppBarToggleButton button) {
			if(groupNameToElements == null)
				return;
			lock(groupNameToElements) {
				ArrayList elements = (ArrayList)groupNameToElements[groupName];
				if(elements != null) {
					PurgeGroup(elements, button);
					if(elements.Count == 0) {
						groupNameToElements.Remove(groupName);
					}
				}
			}
		}
		static void PurgeGroup(ArrayList elements, object elementToRemove) {
			for(int i = 0; i < elements.Count; ) {
				WeakReference weakReference = (WeakReference)elements[i];
				object element = weakReference.Target;
				if(element == null || element == elementToRemove) {
					elements.RemoveAt(i);
				}
				else {
					i++;
				}
			}
		}
		#endregion
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		public event EventHandler Checked;
		public event EventHandler Unchecked;
		public AppBarToggleButton() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(AppBarToggleButton);
#endif
			IsEnabledChanged += OnIsEnabledChanged;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
		}
		void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateVisualState(true);
		}
		protected virtual void OnIsCheckedChanged(bool oldValue, bool newValue) {
			if(newValue) OnChecked();
			else OnUnchecked();
			var peer = FrameworkElementAutomationPeer.FromElement(this) as AppBarToggleButtonAutomationPeer; ;
			if(peer != null)
				peer.RaiseToggleStateChangedEvent(oldValue, newValue);
			UpdateVisualState(true);
		}
		protected virtual void OnChecked() {
			UpdateButtonGroup();
			if(Checked != null)
				Checked(this, EventArgs.Empty);
		}
		protected virtual void OnUnchecked() {
			if(Unchecked != null)
				Unchecked(this, EventArgs.Empty);
		}
		void UpdateButtonGroup() {
			if(string.IsNullOrEmpty(GroupName)) return;
			if(groupNameToElements == null)
				groupNameToElements = new Hashtable(1);
			lock(groupNameToElements) {
				ArrayList elements = (ArrayList)groupNameToElements[GroupName];
				for(int i = 0; i < elements.Count; ) {
					WeakReference weakReference = (WeakReference)elements[i];
					AppBarToggleButton button = weakReference.Target as AppBarToggleButton;
					if(button == null) {
						elements.RemoveAt(i);
					}
					else {
						if(button != this && button.IsChecked)
							button.UncheckButton();
						i++;
					}
				}
			}
		}
		void UncheckButton() {
			SetCurrentValue(IsCheckedProperty, false);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(Clickable) {
				UpdateVisualState(true);
			}
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateVisualState(true);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateVisualState(true);
		}
		protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsPressedChanged(e);
			UpdateVisualState(true);
		}
		protected override void OnClick() {
			base.OnClick();
			OnToggle();
		}
		internal void OnToggle() {
			IsChecked = !IsChecked;
		}
#if SILVERLIGHT
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			UpdateVisualState(true);
		}
#endif
		private void UpdateNormalState(bool useTransitions) {
			if(!IsEnabled) {
				VisualStateManager.GoToState(this, "Disabled", useTransitions);
			}
			else if(base.IsPressed) {
				VisualStateManager.GoToState(this, "Pressed", useTransitions);
			}
			else if(base.IsMouseOver) {
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			}
			else {
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			}
		}
		private void UpdateCheckedState(bool useTransitions) {
			bool? isChecked = IsChecked;
			if(isChecked == true) {
				if(!IsEnabled) {
					VisualStateManager.GoToState(this, "CheckedDisabled", useTransitions);
				}
				else
					if(base.IsPressed) {
						VisualStateManager.GoToState(this, "CheckedPressed", useTransitions);
					}
					else
						if(base.IsMouseOver) {
							VisualStateManager.GoToState(this, "CheckedMouseOver", useTransitions);
						}
						else {
							VisualStateManager.GoToState(this, "Checked", useTransitions);
						}
			}
			else
				if(isChecked == false) {
					VisualStateManager.GoToState(this, "Unchecked", useTransitions);
				}
				else
					if(!VisualStateManager.GoToState(this, "Indeterminate", useTransitions)) {
						VisualStateManager.GoToState(this, "Unchecked", useTransitions);
					}
		}
		protected virtual void UpdateVisualState(bool useTransitions) {
			UpdateNormalState(useTransitions);
			UpdateCheckedState(useTransitions);
		}
		#region UIAutomation
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new AppBarToggleButtonAutomationPeer(this);
		}
		#endregion
	}
}
