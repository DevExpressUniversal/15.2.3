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
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon.Themes;
using System.Windows.Data;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Ribbon {
	public class BarButtonGroupLinkControl : BarItemLinkControl {
		#region static
		public static readonly DependencyProperty IsBorderVisibleProperty;
		protected internal static readonly DependencyPropertyKey IsBorderVisiblePropertyKey;
		public static readonly DependencyProperty DefaultMarginProperty;
		public static readonly DependencyProperty ActualMarginProperty;
		protected static readonly DependencyPropertyKey ActualMarginPropertyKey;
		public static readonly DependencyProperty ResourceHolderTemplateInRibbonPageGroupProperty;
		public static readonly DependencyProperty ResourceHolderTemplateProperty;
		public static readonly DependencyProperty CustomizationButtonStyleProperty;
		protected static readonly DependencyPropertyKey IsCustomizationButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsCustomizationButtonVisibleProperty;
		public static readonly DependencyProperty IsEmptyProperty;
		protected internal static readonly DependencyPropertyKey IsEmptyPropertyKey;
		public static readonly DependencyProperty CustomizationButtonContentProperty;
		protected static readonly DependencyPropertyKey CustomizationButtonContentPropertyKey;
		public static readonly DependencyProperty HideDesignTimeAdditionalElementsProperty;
		static BarButtonGroupLinkControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(typeof(BarButtonGroupLinkControl)));
			ResourceHolderTemplateProperty = DependencyPropertyManager.Register("ResourceHolderTemplate", typeof(ControlTemplate), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			IsBorderVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsBorderVisible", typeof(bool), typeof(BarButtonGroupLinkControl),
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsBorderVisiblePropertyChanged)));
			IsBorderVisibleProperty = IsBorderVisiblePropertyKey.DependencyProperty;
			DefaultMarginProperty = DependencyPropertyManager.Register("DefaultMargin", typeof(Thickness), typeof(BarButtonGroupLinkControl),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnDefaultMarginPropertyChanged)));
			ActualMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualMargin", typeof(Thickness), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(new Thickness(0)));
			ActualMarginProperty = ActualMarginPropertyKey.DependencyProperty;
			ResourceHolderTemplateInRibbonPageGroupProperty = DependencyPropertyManager.Register("ResourceHolderTemplateInRibbonPageGroup", typeof(ControlTemplate), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(null));
			CustomizationButtonStyleProperty = DependencyPropertyManager.Register("CustomizationButtonStyle", typeof(Style), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(null));
			IsCustomizationButtonVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCustomizationButtonVisible", typeof(bool), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(false));
			IsCustomizationButtonVisibleProperty = IsCustomizationButtonVisiblePropertyKey.DependencyProperty;
			IsEmptyPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsEmpty", typeof(bool), typeof(BarButtonGroupLinkControl),
				new FrameworkPropertyMetadata(true, OnIsEmptyPropertyChanged));
			IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;
			CustomizationButtonContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("CustomizationButtonContent", typeof(object), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(string.Empty));
			CustomizationButtonContentProperty = CustomizationButtonContentPropertyKey.DependencyProperty;
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarButtonGroupLinkControl), typeof(BarButtonGroupLinkControlAutomationPeer), owner => new BarButtonGroupLinkControlAutomationPeer((BarButtonGroupLinkControl)owner));
			HideDesignTimeAdditionalElementsProperty = DependencyPropertyManager.RegisterAttached("HideDesignTimeAdditionalElements", typeof(bool), typeof(BarButtonGroupLinkControl), new FrameworkPropertyMetadata(false));
		}
		protected static void OnDefaultMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarButtonGroupLinkControl)d).UpdateActualMargin();
		}
		protected static void OnIsBorderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarButtonGroupLinkControl)d).UpdateActualMargin();
		}
		protected static void OnIsEmptyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarButtonGroupLinkControl)d).OnIsEmptyChanged((bool)e.OldValue);
		}
		public static bool GetHideDesignTimeAdditionalElements(DependencyObject obj) {
			return (bool)obj.GetValue(HideDesignTimeAdditionalElementsProperty);
		}
		public static void SetHideDesignTimeAdditionalElements(DependencyObject obj, bool value) {
			obj.SetValue(HideDesignTimeAdditionalElementsProperty, value);
		}
		#endregion
		#region dep props
		public ControlTemplate ResourceHolderTemplate {
			get { return (ControlTemplate)GetValue(ResourceHolderTemplateProperty); }
			set { SetValue(ResourceHolderTemplateProperty, value); }
		}
		public bool IsBorderVisible {
			get { return (bool)GetValue(IsBorderVisibleProperty); }
			protected internal set { this.SetValue(IsBorderVisiblePropertyKey, value); }
		}
		public Thickness DefaultMargin {
			get { return (Thickness)GetValue(DefaultMarginProperty); }
			set { SetValue(DefaultMarginProperty, value); }
		}
		public Thickness ActualMargin {
			get { return (Thickness)GetValue(ActualMarginProperty); }
			protected set { this.SetValue(ActualMarginPropertyKey, value); }
		}
		public ControlTemplate ResourceHolderTemplateInRibbonPageGroup {
			get { return (ControlTemplate)GetValue(ResourceHolderTemplateInRibbonPageGroupProperty); }
			set { SetValue(ResourceHolderTemplateInRibbonPageGroupProperty, value); }
		}
		public Style CustomizationButtonStyle {
			get { return (Style)GetValue(CustomizationButtonStyleProperty); }
			set { SetValue(CustomizationButtonStyleProperty, value); }
		}
		public bool IsCustomizationButtonVisible {
			get { return (bool)GetValue(IsCustomizationButtonVisibleProperty); }
			protected internal set { this.SetValue(IsCustomizationButtonVisiblePropertyKey, value); }
		}
		public bool IsEmpty {
			get { return (bool)GetValue(IsEmptyProperty); }
			protected internal set { this.SetValue(IsEmptyPropertyKey, value); }
		}
		public object CustomizationButtonContent {
			get { return (object)GetValue(CustomizationButtonContentProperty); }
			protected set { this.SetValue(CustomizationButtonContentPropertyKey, value); }
		}
		#endregion
		public BarButtonGroupLinkControl() {
			UpdateCustomizationButtonVisiblity();
			UpdateCustomizationButtonContent();
		}
		public BarButtonGroupLinkControl(BarButtonGroupLink link) : base(link) {
			UpdateCustomizationButtonVisiblity();
			UpdateCustomizationButtonContent();
		}
		protected internal UIElement Separator { get; set; }
		protected internal BarButtonGroupItemsControl ItemsControl { get; set; }
		public BarButtonGroupLink ButtonGroupLink { get { return LinkBase as BarButtonGroupLink; } }
		public BarButtonGroup ButtonGroup { get { return Item as BarButtonGroup; } }
		public override void OnApplyTemplate() {
			if(ItemsControl != null) ItemsControl.LinkControl = null;
			base.OnApplyTemplate();			
			ItemsControl = (BarButtonGroupItemsControl)GetTemplateChild("PART_ItemsControl");
			Separator = (UIElement)GetTemplateChild("PART_Separator");
			if(ItemsControl != null) ItemsControl.LinkControl = this;			
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override void UpdateStyleByContainerType(LinkContainerType type) {
			base.UpdateStyleByContainerType(type);
			switch(type) {
				case LinkContainerType.RibbonPageGroup:
					ResourceHolderTemplate = ResourceHolderTemplateInRibbonPageGroup;					
					break;
			}
		}
		protected virtual void UpdateActualMargin() {
			if(IsBorderVisible)
				ActualMargin = new Thickness(0);
			else
				ActualMargin = DefaultMargin;
		}
		protected virtual void UpdateCustomizationButtonVisiblity() {
			if(!this.IsInDesignTool()) {
				IsCustomizationButtonVisible = false;
				return;
			}
			RibbonControl ribbon = LayoutHelper.FindParentObject<RibbonControl>(this);
			IsCustomizationButtonVisible = ribbon != null && !BarButtonGroupLinkControl.GetHideDesignTimeAdditionalElements(ribbon);
		}
		protected virtual void OnIsEmptyChanged(bool oldValue) {
			UpdateCustomizationButtonContent();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateCustomizationButtonVisiblity();
		}
		protected virtual void UpdateCustomizationButtonContent() {			
			CustomizationButtonContent = IsEmpty ? "[Empty Group]" : "[Group]";
		}
		internal void UpdateItemsControlSource() {
			if(ItemsControl != null) ItemsControl.UpdateItemsSource();			
		}
		internal BarItemLinkControlBase GetLinkControl(int index) {
			if(ItemsControl == null)
				return null;
			BarItemLinkInfo info = ItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as BarItemLinkInfo;
			return info == null ? null : info.LinkControl;
		}
	}
}
