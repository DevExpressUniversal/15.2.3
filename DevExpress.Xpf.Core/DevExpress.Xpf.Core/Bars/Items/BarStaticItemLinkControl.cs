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
using System.Windows;
using DevExpress.Xpf.Bars.Themes;
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Automation;
using System.Windows.Data;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class BarStaticItemLinkControl : BarItemLinkControl, ISupportAutoSize {
		#region static
		public static readonly DependencyProperty BorderPaddingProperty;
		public static readonly DependencyProperty ActualContentAlignmentProperty;
		protected static readonly DependencyPropertyKey ActualContentAlignmentPropertyKey;
		public static readonly DependencyProperty ActualMinWidthProperty;
		protected static readonly DependencyPropertyKey ActualMinWidthPropertyKey;		
		protected static readonly DependencyPropertyKey ActualBorderPaddingPropertyKey;
		public static readonly DependencyProperty ActualBorderPaddingProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		static BarStaticItemLinkControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarStaticItemLinkControl), typeof(BarStaticItemLinkControlAutomationPeer), owner => new BarStaticItemLinkControlAutomationPeer((BarStaticItemLinkControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(typeof(BarStaticItemLinkControl)));
			ActualIsHoverEnabledPropertyKey.OverrideMetadata(typeof(BarStaticItemLinkControl), new PropertyMetadata(false));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnShowBorderPropertyChanged)));
			ActualContentAlignmentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContentAlignment", typeof(HorizontalAlignment), typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			ActualContentAlignmentProperty = ActualContentAlignmentPropertyKey.DependencyProperty;
			ActualMinWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualMinWidth", typeof(double), typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(0.0));
			ActualMinWidthProperty = ActualMinWidthPropertyKey.DependencyProperty;
			BorderPaddingProperty = DependencyPropertyManager.Register("BorderPadding", typeof(Thickness), typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnShowBorderPropertyChanged)));
			ActualBorderPaddingPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderPadding", typeof(Thickness), typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualBorderPaddingProperty = ActualBorderPaddingPropertyKey.DependencyProperty;
			FocusableProperty.OverrideMetadata(typeof(BarStaticItemLinkControl), new FrameworkPropertyMetadata(false));
		}
		protected static void OnShowBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLinkControl)d).OnShowBorderChanged(e);
		}
		protected static void OnBorderPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLinkControl)d).OnBorderPaddingPropertyChanged(e);
		}
		#endregion
		#region dep props
		public Thickness BorderPadding {
			get { return (Thickness)GetValue(BorderPaddingProperty); }
			set { SetValue(BorderPaddingProperty, value); }
		}
		public HorizontalAlignment ActualContentAlignment {
			get { return (HorizontalAlignment)GetValue(ActualContentAlignmentProperty); }
			protected set { this.SetValue(ActualContentAlignmentPropertyKey, value); }
		}
		public Thickness ActualBorderPadding {
			get { return (Thickness)GetValue(ActualBorderPaddingProperty); }
			protected internal set { this.SetValue(ActualBorderPaddingPropertyKey, value); }
		}
		public double ActualMinWidth {
			get { return (double)GetValue(ActualMinWidthProperty); }
			protected set { this.SetValue(ActualMinWidthPropertyKey, value); }
		}
		#endregion
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		public BarStaticItemLinkControl() : base() {
		}
		public BarStaticItemLinkControl(BarStaticItemLink link) : base(link) {
		}
		public BarStaticItemLink StaticItemLink { get { return base.Link as BarStaticItemLink; } }
		protected BarStaticItem StaticItem { get { return Item as BarStaticItem; } }
		protected object GetTemplateFromProvider(DependencyProperty prop, BarStaticItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarStaticItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return key => new BarStaticItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		protected internal override void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDownCore(e);
			MouseButtonState state = e.ButtonState;
			if(state == MouseButtonState.Pressed) {
				MouseHelper.Capture(this);
				if(MouseHelper.Captured == this) {
					if(state == MouseButtonState.Pressed)
						IsPressed = true;
					else
						MouseHelper.ReleaseCapture(this);
				}
			}
			IsSelected = BarManagerCustomizationHelper.IsInCustomizationMode(this);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			e.Handled = true;
			bool pressed = IsPressed;
			if(MouseHelper.Captured == this)
				MouseHelper.ReleaseCapture(this);
			if(pressed)
				OnClick();
			base.OnMouseLeftButtonUp(e);
		}
		protected override RibbonItemStyles CalcRibbonStyleInQAT() {
			if (Link.ActualRibbonStyle == RibbonItemStyles.SmallWithoutText)
				return RibbonItemStyles.SmallWithoutText;
			return RibbonItemStyles.SmallWithText;
		}
		protected override RibbonItemStyles CalcRibbonStyleInStatusBar() {
			return CalcRibbonStyleInQAT();
		}
		protected override RibbonItemStyles CalcRibbonStyleInPageGroup() {
			return CalcRibbonStyleInQAT();
		}
		#region ISupportAutoSize Members
		BarItemAutoSizeMode ISupportAutoSize.GetAutoSize() {
			return StaticItem == null ? BarItemAutoSizeMode.None : StaticItem.AutoSizeMode;
		}
		double ISupportAutoSize.GetWidth() {
			return StaticItem== null ? double.NaN : StaticItem.ItemWidth;
		}
		double ISupportAutoSize.GetMinWidth() {
			return StaticItemLink == null ? 0d : StaticItemLink.ActualMinWidth;
		}
		#endregion
		protected internal virtual void OnSourceLinkWidthChanged() {
			UpdateVisualStateByWidth();
		}
		protected internal double GetLinkWidth() {
			if(StaticItemLink == null)
				return double.NaN;
			return GetHasWidth() ? StaticItemLink.ActualLinkWidth : double.NaN;
		}
		bool GetHasWidth() {
			if(StaticItemLink != null)
				return StaticItemLink.HasWidth;
			if(Item is BarStaticItem)
				return ((BarStaticItem)Item).ItemWidth != 0;
			return false;
		}
		protected internal virtual void UpdateVisualStateByWidth() {
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateVisualStateByWidth();
			UpdateTextBorderPadding();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
		protected override void UpdateLayoutPanel() {
			if(LayoutPanel == null) return;
			base.UpdateLayoutPanel();
			UpdateLayoutPanelAlignment();
			UpdateLayoutPanelWidth();
		}
		protected internal virtual void UpdateLayoutPanelAlignment() {
			if(LayoutPanel == null || StaticItem==null) return;
			LayoutPanel.HorizontalAlignment = StaticItem.AutoSizeMode == BarItemAutoSizeMode.Fill || StaticItem.AutoSizeMode == BarItemAutoSizeMode.None ? HorizontalAlignment.Stretch : HorizontalAlignment.Left;
		}
		protected override void UpdateLayoutPanelElementContentProperties() {
			if (LayoutPanel != null) {
				LayoutPanel.ContentHorizontalAlignment = ActualContentAlignment;
			}
		}
		protected internal virtual void UpdateLayoutPanelWidth() {
			if(LayoutPanel == null || StaticItem == null) return;
			if(StaticItem.AutoSizeMode == BarItemAutoSizeMode.None) {
				LayoutPanel.Width = this.StaticItem.ItemWidth;
			} else {
				LayoutPanel.Width = double.NaN;
			}
			UpdateLayoutPanelAlignment();
			LayoutPanel.MinWidth = ActualMinWidth;
		}
		protected override void UpdateLayoutPanelShowContent() {
			base.UpdateLayoutPanelShowContent();
			if(LayoutPanel != null && ActualShowContent)
				Dispatcher.BeginInvoke(new Action(UpdateLayoutPanelElementContentProperties));
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		protected virtual void OnShowBorderChanged(DependencyPropertyChangedEventArgs e) {
			UpdateTextBorderPadding();
		}
		protected virtual void OnBorderPaddingPropertyChanged(DependencyPropertyChangedEventArgs e) {
			UpdateTextBorderPadding();
		}
		private void UpdateTextBorderPadding() {
			ActualBorderPadding = ShowBorder ? BorderPadding : new Thickness();
		}
		protected internal override bool GetIsSelectable() {
			return base.GetIsSelectable() && LayoutHelper.FindElement(this, x => x != this && x.Focusable).ReturnSuccess();
		}
		#region actual properties updaters
		protected internal virtual void OnSourceMinWidthChanged() {
			UpdateActualMinWidth();
		}
		protected internal virtual void OnSourceContentAlignmentChanged() {
			UpdateActualContentAlignment();
		}
		protected internal virtual void OnSourceShowBorderChanged() {
			UpdateShowBorder();
		}
		protected virtual void UpdateActualMinWidth() {
			ActualMinWidth = GetMinWidth();
			UpdateLayoutPanelWidth();
		}
		protected virtual void UpdateShowBorder() {
			ShowBorder = StaticItem == null ? true : StaticItem.ShowBorder;
		}
		protected virtual void UpdateActualContentAlignment() {
			ActualContentAlignment = GetActualContentAlignment();
			UpdateLayoutPanelElementContentProperties();
		}
		protected override void UpdateItemVisualState() {
			if(LayoutPanel == null) return;
			if(ShowCustomizationBorder) {
				LayoutPanel.BorderState = BorderState.Customization;
			} else if(!IsEnabled) {
				LayoutPanel.BorderState = BorderState.Disabled;
			} else {
				LayoutPanel.BorderState = BorderState.Normal;
			}
		}
		protected virtual double GetMinWidth() {
			if(StaticItemLink != null) {
				if(StaticItemLink.UserMinWidth != (double)BarStaticItemLink.UserMinWidthProperty.GetMetadata(typeof(BarStaticItemLink)).DefaultValue)
					return StaticItemLink.UserMinWidth;				
			}
			if(StaticItem != null)
				return StaticItem.ItemMinWidth;
			return 0;
		}
		protected virtual HorizontalAlignment GetActualContentAlignment() {
			if(Item is BarStaticItem)
				return ((BarStaticItem)Item).ContentAlignment;
			if(StaticItemLink != null)
				return StaticItemLink.ContentAlignment;
			return HorizontalAlignment.Left;
		}
		protected virtual bool ShouldHideBorder() {
			return ContainerType == LinkContainerType.RibbonPageGroup || ContainerType == LinkContainerType.RibbonPageHeader ||
				ContainerType == LinkContainerType.RibbonQuickAccessToolbar || ContainerType == LinkContainerType.RibbonQuickAccessToolbarFooter || IsLinkControlInMenu;
		}		
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateActualContentAlignment();
			UpdateActualMinWidth();
			UpdateVisualStateByWidth();
			MinWidth = ActualMinWidth;
			if(ShouldHideBorder())
				ShowBorder = false;
			else if(StaticItem != null)
				ShowBorder = StaticItem.ShowBorder;
		}
		#endregion
	}
	public interface ISupportAutoSize {
		BarItemAutoSizeMode GetAutoSize();
		double GetWidth();
		double GetMinWidth();
	}
}
