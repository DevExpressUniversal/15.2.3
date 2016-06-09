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
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Themes;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	public class ArrowControl : Control {
		#region static
		public static readonly DependencyProperty LinkControlProperty;
		public static readonly DependencyProperty ArrowAlignmentProperty;
		public static readonly DependencyProperty LinkContainerTypeProperty;
		static ArrowControl() {
			LinkControlProperty = DependencyPropertyManager.Register("LinkControl", typeof(BarItemLinkControl), typeof(ArrowControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLinkControlPropertyChanged)));
			ArrowAlignmentProperty = DependencyPropertyManager.Register("ArrowAlignment", typeof(Dock), typeof(ArrowControl), new FrameworkPropertyMetadata(Dock.Left, new PropertyChangedCallback(OnArrowAlignmentPropertyChanged)));
			LinkContainerTypeProperty = DependencyPropertyManager.Register("LinkContainerType", typeof(LinkContainerType), typeof(ArrowControl), new FrameworkPropertyMetadata(LinkContainerType.None, new PropertyChangedCallback(OnLinkContainerTypePropertyChanged)));
			FocusableProperty.OverrideMetadata(typeof(ArrowControl), new FrameworkPropertyMetadata(false));
		}
		protected static void OnLinkContainerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ArrowControl)d).OnLinkContainerTypeChanged(e);
		}
		protected static void OnArrowAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ArrowControl)d).OnArrowAlignmentChanged(e);
		}
		protected static void OnLinkControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ArrowControl)d).OnLinkControlChanged(e);
		}
		#endregion
		public ArrowControl() {
			Loaded += new RoutedEventHandler(OnLoaded);
			IsTabStop = false;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			LinkControl = LayoutHelper.FindParentObject<BarItemLinkControl>(this);
			if (LinkControl != null)
				UpdateVisualStateByArrowAlignment();
		}
		public BarItemLinkControl LinkControl {
			get { return (BarItemLinkControl)GetValue(LinkControlProperty); }
			set { SetValue(LinkControlProperty, value); }
		}
		public Dock ArrowAlignment {
			get { return (Dock)GetValue(ArrowAlignmentProperty); }
			set { SetValue(ArrowAlignmentProperty, value); }
		}
		public LinkContainerType LinkContainerType {
			get { return (LinkContainerType)GetValue(LinkContainerTypeProperty); }
			set { SetValue(LinkContainerTypeProperty, value); }
		}
		protected virtual void OnArrowAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByArrowAlignment();
		}
		protected virtual void UpdateVisualStateByArrowAlignment() {
			VisualStateManager.GoToState(this, ArrowAlignment.ToString(), false);
		}
		protected virtual void OnLinkContainerTypeChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnLinkControlChanged(DependencyPropertyChangedEventArgs e) {
			if (LinkControl == null)
				return;
			LinkContainerType = LinkControl.ContainerType;
			UpdateVisualStateByArrowAlignment();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualStateByArrowAlignment();
		}
	}
	public class BarSubItemArrowControl : ArrowControl {
		public BarSubItemLinkControl SubItemLinkControl { get { return (BarSubItemLinkControl)LinkControl; } }
	}
	public class BarSplitButtonItemArrowControl : ArrowControl {
		public BarSplitButtonItemLinkControl SubItemLinkControl { get { return (BarSplitButtonItemLinkControl)LinkControl; } }
		protected virtual void SetBindings() {
			Binding arrowAlignmentBinding = new Binding("ActualArrowAlignment");
			arrowAlignmentBinding.Source = LinkControl;
			BindingOperations.SetBinding(this, ArrowControl.ArrowAlignmentProperty, arrowAlignmentBinding);
		}
		protected override void OnLinkControlChanged(DependencyPropertyChangedEventArgs e) {
			base.OnLinkControlChanged(e);
			SetBindings();
		}
	}
	public class ArrowTemplateProvider : DependencyObject {
		public static readonly DependencyProperty ArrowStyleInBarProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInBar", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInMainMenu", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInMenu", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInStatusBar", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInRibbonPageGroup", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInButtonGroup", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInQuickAccessToolbar", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInQuickAccessToolbarFooter", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInRibbonPageHeader", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInRibbonStatusBarLeft", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("ArrowStyleInRibbonStatusBarRight", typeof(Style), typeof(ArrowTemplateProvider), new UIPropertyMetadata(null));
		public static Style GetArrowStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInRibbonStatusBarRightProperty);
		}
		public static void SetArrowStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetArrowStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetArrowStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetArrowStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInRibbonPageHeaderProperty);
		}
		public static void SetArrowStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetArrowStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetArrowStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetArrowStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInQuickAccessToolbarProperty);
		}
		public static void SetArrowStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetArrowStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInButtonGroupProperty);
		}
		public static void SetArrowStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInButtonGroupProperty, value);
		}
		public static Style GetArrowStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInRibbonPageGroupProperty);
		}
		public static void SetArrowStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetArrowStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInStatusBarProperty);
		}
		public static void SetArrowStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInStatusBarProperty, value);
		}
		public static Style GetArrowStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInMenuProperty);
		}
		public static void SetArrowStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInMenuProperty, value);
		}
		public static Style GetArrowStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInMainMenuProperty);
		}
		public static void SetArrowStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInMainMenuProperty, value);
		}
		public static Style GetArrowStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(ArrowStyleInBarProperty);
		}
		public static void SetArrowStyleInBar(DependencyObject target, Style value) {
			target.SetValue(ArrowStyleInBarProperty, value);
		}
	}
}
