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
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Ribbon.Internal {
	public class RibbonAutoHideControl : ContentControl {
		#region static
		public static readonly DependencyProperty IsAutoHideActiveProperty;
		public static readonly DependencyProperty IsRibbonShownProperty;
		static RibbonAutoHideControl() {
			Type ownerType = typeof(RibbonAutoHideControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			IsAutoHideActiveProperty = DependencyPropertyManager.Register("IsAutoHideActive", typeof(bool), ownerType, new PropertyMetadata(false, OnIsAutoHideActivePropertyChanged));
			IsRibbonShownProperty = DependencyPropertyManager.Register("IsRibbonShown", typeof(bool), ownerType, new PropertyMetadata(false, OnIsRibbonShownPropertyChanged, OnCoerceIsRibbonShownProperty));
		}
		static object OnCoerceIsRibbonShownProperty(DependencyObject d, object baseValue) {
			return ((RibbonAutoHideControl)d).OnIsRibbonShownCoerce((bool)baseValue);
		}
		static void OnIsRibbonShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonAutoHideControl)d).OnIsRibbonShownChanged((bool)e.OldValue);
		}
		static void OnIsAutoHideActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonAutoHideControl)d).OnIsAutoHidePropertyChanged((bool)e.OldValue);
		}
		#endregion
		#region Dependency Properties
		public bool IsAutoHideActive {
			get { return (bool)GetValue(IsAutoHideActiveProperty); }
			set { SetValue(IsAutoHideActiveProperty, value); }
		}
		public bool IsRibbonShown {
			get { return (bool)GetValue(IsRibbonShownProperty); }
			set { SetValue(IsRibbonShownProperty, value); }
		}
		#endregion
		#region Properties
		public ContentPresenter ContentPresenter { get; private set; }
		public Popup Popup { get; private set; }
		public ContentControl ContentContainer { get; private set; }
		public ToggleButton ShowAutoHidePopupButton { get; set; }
		DXRibbonWindow RibbonWindow {
			get { return ribbonWindow; }
			set {
				if(ribbonWindow == value)
					return;
				var oldValue = value;
				ribbonWindow = value;
				OnRibbonWindowChanged(oldValue);
			}
		}
		FrameworkElement ControlBoxContainer {
			get { return controlBoxContainer; }
			set { if (controlBoxContainer == value)
				return;
			var oldValue = controlBoxContainer;
			controlBoxContainer = value;
			OnControlBoxContainerChanged(oldValue);
			}
		}
		#endregion
		public RibbonAutoHideControl() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
			ContentContainer = GetTemplateChild("PART_ContainerControl") as ContentControl;
			Popup = GetTemplateChild("PART_RibbonAutoHidePopup") as Popup;
			ShowAutoHidePopupButton = GetTemplateChild("PART_ShowAutoHidePopupButton") as ToggleButton;
			SetCurrentValue(IsRibbonShownProperty, false);
			UpdateContentPlacement();
			UpdateControlBoxContainer();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			RibbonWindow = Window.GetWindow(this) as DXRibbonWindow;
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			RibbonWindow = null;
		}
		protected virtual void OnIsAutoHidePropertyChanged(bool oldValue) {
			if(!IsAutoHideActive)
				SetCurrentValue(IsRibbonShownProperty, false);
		}
		protected virtual void OnIsRibbonShownChanged(bool oldValue) {
			UpdateContentPlacement();
		}
		protected virtual void OnRibbonWindowChanged(DXRibbonWindow oldValue) {
			UpdateControlBoxContainer();
		}
		protected virtual void OnControlBoxContainerChanged(FrameworkElement oldValue) {
			oldValue.Do(container => container.SizeChanged -= OnContolBoxContainerSizeChanged);
			ControlBoxContainer.Do(container => container.SizeChanged += OnContolBoxContainerSizeChanged);
			UpdateShowPopupButtonMargin();
		}
		protected virtual void OnContolBoxContainerSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateShowPopupButtonMargin();
		}
		protected virtual object OnIsRibbonShownCoerce(bool baseValue) {
			if(!IsAutoHideActive)
				return false;
			return baseValue;
		}
		protected virtual void UpdateContentPlacement() {
			if(ContentContainer == null || Popup == null)
				return;
			if(IsRibbonShown) {
				ContentContainer.SetCurrentValue(ContentControl.ContentProperty, null);
				Popup.SetCurrentValue(Popup.ChildProperty, ContentPresenter);
			} else {
				Popup.ClearValue(Popup.ChildProperty);
				ContentContainer.ClearValue(ContentControl.ContentProperty);
			}
		}
		protected virtual void UpdateControlBoxContainer() {
			ControlBoxContainer = RibbonWindow.With(window => window.GetControlBoxContainer());
		}
		protected virtual void UpdateShowPopupButtonMargin() {
			if (ShowAutoHidePopupButton == null)
				return;
			if (ControlBoxContainer != null) {
				Point point = ControlBoxContainer.TranslatePoint(new Point(), RibbonWindow);
				double right = FlowDirection == FlowDirection.RightToLeft ? point.X : RibbonWindow.ActualWidth - point.X;
				var margin = ShowAutoHidePopupButton.Margin;
				margin.Right = right;
				ShowAutoHidePopupButton.Margin = margin;
			} else
				ShowAutoHidePopupButton.ClearValue(MarginProperty);
		}
		FrameworkElement controlBoxContainer;
		DXRibbonWindow ribbonWindow;
	}
}
