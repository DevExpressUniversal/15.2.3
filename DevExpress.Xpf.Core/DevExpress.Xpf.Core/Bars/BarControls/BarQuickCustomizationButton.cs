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

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using DevExpress.Xpf.Utils;
using System;
using DevExpress.Xpf.Bars.Native;
using System.Collections.Generic;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using System.Windows.Input;
namespace DevExpress.Xpf.Bars {
	[Browsable(false)]
	public class BarQuickCustomizationButton : ToggleButton, IEventListenerClient, INavigationElement {
		bool isSelected;
		public BarQuickCustomizationButton() {
			IsTabStop = true;
			Focusable = true;
			FocusVisualStyle = null;
			IsEnabledChanged += OnIsEnabledChanged;
			ClickMode = ClickMode.Press;
		}
		private WeakReference managerWR = new WeakReference(null);
		public BarManager Manager {
			get { return (BarManager)managerWR.Target; }
			set { managerWR = new WeakReference(value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(BarQuickCustomizationButton),
			new FrameworkPropertyMetadata(Orientation.Horizontal, (d, e) => ((BarQuickCustomizationButton)d).OnOrientationPropertyChanged()));
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateStateBeOrientation();
		}
		protected virtual void OnOrientationPropertyChanged() {
			UpdateStateBeOrientation();
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) { base.OnMouseMove(e); UpdateState(); }		
		protected virtual void OnIsSelectedChanged(bool oldValue) { Focus(); UpdateState(); }
		protected override void OnClick() { base.OnClick(); UpdateState(); }		
		protected override void OnMouseEnter(MouseEventArgs e) { base.OnMouseEnter(e); UpdateState(); }
		protected override void OnMouseLeave(MouseEventArgs e) { base.OnMouseLeave(e); UpdateState(); }
		protected override void OnChecked(RoutedEventArgs e) { base.OnChecked(e); UpdateState(); }
		protected override void OnUnchecked(RoutedEventArgs e) { base.OnUnchecked(e); UpdateState(); }
		protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) { base.OnIsPressedChanged(e); UpdateState(); }
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) { UpdateState(); }
		protected virtual void UpdateState() {
			string state = "Normal";
			if (!IsEnabled) {
				state = "Disabled";
			} else if (IsPressed) {
				state = "Pressed";
			} else if ((bool)IsChecked) {
				state = "Checked";
			} else if (IsMouseOver || isSelected) {
				state = "MouseOver";
			}
			VisualStateManager.GoToState(this, state, false);
		}
		protected virtual void UpdateStateBeOrientation() {
			VisualStateManager.GoToState(this, Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical", false);
		}
		bool IEventListenerClient.ReceiveEvent(object sender, EventArgs e) {
			var re = e as RoutedEventArgs;
			if (re == null)
				return false;
			var doSource = re.OriginalSource as DependencyObject;
			if (doSource == null)
				return false;
			if (Equals(Mouse.PreviewMouseDownEvent, re.RoutedEvent) && doSource.VisualParents().Contains(this))
				return true;
			return false;
		}
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			return string.Empty;
		}				
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { yield return typeof(IEventListenerClient); }
		}
		bool INavigationElement.ProcessKeyDown(KeyEventArgs e) {
			if (e.Key == Key.Space || e.Key == (Orientation == Orientation.Horizontal ? Key.Down : Key.Right)) {
				IsChecked = !IsChecked;
				return true;
			}
			return false;  
		}
		INavigationOwner INavigationElement.BoundOwner { get { return BarNameScope.GetService<ICustomizationService>(this).CustomizationHelper.With(x => x.CustomizationMenu).With(x => x.ContentControl); } }
		bool INavigationElement.IsSelected {
			get { return isSelected; }
			set {
				if (value == isSelected) return;
				bool oldValue = isSelected;
				isSelected = value;
				OnIsSelectedChanged(oldValue);
			}
		}
		IBarsNavigationSupport IBarsNavigationSupport.Parent { get { return TreeHelper.GetParent<BarControl>(this, x => true); } }
		int IBarsNavigationSupport.ID { get { return GetHashCode(); } }
		bool IBarsNavigationSupport.IsSelectable { get { return IsVisible; } }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return false; } }
		bool IBarsNavigationSupport.ExitNavigationOnFocusChangedWithin { get { return false; } }
	}
}
