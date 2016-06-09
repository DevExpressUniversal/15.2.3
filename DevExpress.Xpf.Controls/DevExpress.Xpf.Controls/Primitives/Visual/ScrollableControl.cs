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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.Core;
using System;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Controls.Primitives {
	public enum ScrollButtonMode { Default, Overlap }
	interface IScrollablePanel {
		ScrollableControl ScrollOwner { get; set; }
		bool CanScrollPrev { get; }
		bool CanScrollNext { get; }
		void ScrollNext();
		void ScrollPrev();
		bool BringChildIntoView(FrameworkElement child);
		Orientation Orientation { get; set; }
		bool IsLoaded { get; }
	}
	public class ScrollableControl : ContentControl {
		#region static
		public static readonly DependencyProperty ScrollButtonModeProperty = DependencyProperty.RegisterAttached("ScrollButtonMode", typeof(ScrollButtonMode), typeof(ScrollableControl), new PropertyMetadata(ScrollButtonMode.Default, new PropertyChangedCallback(OnScrollableModeChanged)));
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ScrollableControl), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));
		public static readonly DependencyProperty ScrollNextProperty = DependencyProperty.Register("ScrollNext", typeof(IDelegateCommand), typeof(ScrollableControl));
		public static readonly DependencyProperty ScrollPrevProperty = DependencyProperty.Register("ScrollPrev", typeof(IDelegateCommand), typeof(ScrollableControl));
		public static readonly DependencyProperty IsScrollableProperty = DependencyProperty.Register("IsScrollable", typeof(bool), typeof(ScrollableControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsScrollableChanged)));
		private static void OnOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ScrollableControl scrollableControl = o as ScrollableControl;
			if(scrollableControl != null)
				scrollableControl.OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
		}
		public static ScrollButtonMode GetScrollButtonMode(DependencyObject target) {
			return (ScrollButtonMode)target.GetValue(ScrollButtonModeProperty);
		}
		public static void SetScrollButtonMode(DependencyObject target, ScrollButtonMode value) {
			target.SetValue(ScrollButtonModeProperty, value);
		}
		private static void OnScrollableModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			OnScrollButtonModeChanged(o, (ScrollButtonMode)e.OldValue, (ScrollButtonMode)e.NewValue);
		}
		private static void OnScrollButtonModeChanged(DependencyObject o, ScrollButtonMode oldValue, ScrollButtonMode newValue) {
			ScrollableControl scrollableControl = o as ScrollableControl;
			if(scrollableControl != null) 
				scrollableControl.UpdateVisualState();
		}
		private static void OnIsScrollableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ScrollableControl scrollableControl = o as ScrollableControl;
			if(scrollableControl != null)
				scrollableControl.OnIsScrollableChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		public ScrollButtonMode ScrollButtonMode {
			get { return (ScrollButtonMode)GetValue(ScrollButtonModeProperty); }
			set { SetValue(ScrollButtonModeProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public IDelegateCommand ScrollNext {
			get { return (IDelegateCommand)GetValue(ScrollNextProperty); }
			set { SetValue(ScrollNextProperty, value); }
		}
		public IDelegateCommand ScrollPrev {
			get { return (IDelegateCommand)GetValue(ScrollPrevProperty); }
			set { SetValue(ScrollPrevProperty, value); }
		}
		public bool IsScrollable {
			get { return (bool)GetValue(IsScrollableProperty); }
			set { SetValue(IsScrollableProperty, value); }
		}
		public ScrollableControl() {
			DefaultStyleKey = typeof(ScrollableControl);
			ScrollNext = DelegateCommandFactory.Create(OnScrollNext, CanScrollNext, false);
			ScrollPrev = DelegateCommandFactory.Create(OnScrollPrev, CanScrollPrev, false);
		}
		IScrollablePanel EnsureScrollablePanel() {
			if(PartScrollablePanel == null || (this.IsLoaded && !PartScrollablePanel.IsLoaded)) {
				PartScrollablePanel = LayoutHelper.FindElement(this, x => x is IScrollablePanel) as IScrollablePanel;
				if(PartScrollablePanel != null) {
					PartScrollablePanel.ScrollOwner = this;
					PartScrollablePanel.Orientation = Orientation;
				}
			}
			return PartScrollablePanel;
		}
		void OnScrollNext() {
			EnsureScrollablePanel();
			if(PartScrollablePanel != null && PartScrollablePanel.CanScrollNext) PartScrollablePanel.ScrollNext();
		}
		bool CanScrollNext() {
			EnsureScrollablePanel();
			return PartScrollablePanel != null && PartScrollablePanel.CanScrollNext;
		}
		void OnScrollPrev() {
			EnsureScrollablePanel();
			if(PartScrollablePanel != null && PartScrollablePanel.CanScrollPrev) PartScrollablePanel.ScrollPrev();
		}
		bool CanScrollPrev() {
			EnsureScrollablePanel();
			return PartScrollablePanel != null && PartScrollablePanel.CanScrollPrev;
		}
		IScrollablePanel PartScrollablePanel;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartScrollablePanel = Content as IScrollablePanel;
			if(PartScrollablePanel == null) {
				FrameworkElement itemsPresenter = base.Content as System.Windows.Controls.ItemsPresenter;
				if(itemsPresenter != null)
					itemsPresenter.SizeChanged += itemsPresenter_SizeChanged;
			}
			if(PartScrollablePanel != null) {
				PartScrollablePanel.ScrollOwner = this;
				PartScrollablePanel.Orientation = Orientation;
			}
			UpdateVisualState();
		}
		void itemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e) {
			FrameworkElement itemsPresenter = sender as FrameworkElement;
			if(itemsPresenter != null) itemsPresenter.SizeChanged -= itemsPresenter_SizeChanged;
			EnsureScrollablePanel();
		}
		public void InvalidateScroll() {
			ScrollNext.RaiseCanExecuteChanged();
			ScrollPrev.RaiseCanExecuteChanged();
		}
		protected virtual void OnIsScrollableChanged(bool oldValue, bool newValue) {
			UpdateVisualState();
		}
		protected virtual void UpdateVisualState() {
			VisualStateManager.GoToState(this, "EmptyOrientationState", false);
			VisualStateManager.GoToState(this, Orientation.ToString(), false);
			VisualStateManager.GoToState(this, IsScrollable ? (ScrollButtonMode == Primitives.ScrollButtonMode.Default ? "DefaultScroll" : "OverlappedScroll") : "DisabledScroll", false);
		}
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
			UpdateVisualState();
			if(PartScrollablePanel != null) PartScrollablePanel.Orientation = newValue;
		}
	}
	public class ScrollableControlButton : DirectionButton {
		public ScrollableControlButton() {
			DefaultStyleKey = typeof(ScrollableControlButton);
		}
	}
}
