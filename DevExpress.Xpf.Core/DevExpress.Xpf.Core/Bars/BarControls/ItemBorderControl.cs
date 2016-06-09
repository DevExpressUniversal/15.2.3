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
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Themes;
namespace DevExpress.Xpf.Bars {
	public enum HideBorderSide { None, Left, Right, Bottom, Top, All }
	public enum BorderState { Default, Normal, Hover, Pressed, Checked, Indeterminate, HoverChecked, Focused, Customization, Disabled }
	public class ItemBorderControl : ContentControl {
		#region static
		public static readonly DependencyProperty HideBorderSideProperty;
		public static readonly DependencyProperty ArrowHideBorderSideProperty;
		public static readonly DependencyProperty IsActiveProperty;
		public static readonly DependencyProperty StateProperty;
		public static readonly DependencyProperty LinkControlProperty;
		public static readonly DependencyProperty ItemPositionProperty;
		public static readonly DependencyProperty NormalTemplateProperty;
		public static readonly DependencyProperty HoverTemplateProperty;
		public static readonly DependencyProperty PressedTemplateProperty;
		public static readonly DependencyProperty CustomizationTemplateProperty;
		static ItemBorderControl() {
			HideBorderSideProperty = DependencyPropertyManager.Register("HideBorderSide", typeof(HideBorderSide), typeof(ItemBorderControl), new FrameworkPropertyMetadata(HideBorderSide.None, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnHideBorderSidePropertyChanged)));
			ArrowHideBorderSideProperty = DependencyPropertyManager.Register("ArrowHideBorderSide", typeof(HideBorderSide), typeof(ItemBorderControl), new FrameworkPropertyMetadata(HideBorderSide.None, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnArrowHideBorderSidePropertyChanged)));
			IsActiveProperty = DependencyPropertyManager.Register("IsActive", typeof(bool), typeof(ItemBorderControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsActivePropertyChanged)));
			StateProperty = DependencyPropertyManager.Register("State", typeof(BorderState), typeof(ItemBorderControl), new FrameworkPropertyMetadata(BorderState.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStatePropertyChanged)));
			LinkControlProperty = DependencyPropertyManager.Register("LinkControl", typeof(BarItemLinkControl), typeof(ItemBorderControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLinkControlPropertyChanged)));
			ItemPositionProperty = DependencyPropertyManager.Register("ItemPosition", typeof(HorizontalItemPositionType), typeof(ItemBorderControl), new FrameworkPropertyMetadata(HorizontalItemPositionType.Single, new PropertyChangedCallback(OnItemPositionPropertyChanged)));
			NormalTemplateProperty = DependencyPropertyManager.Register("NormalTemplate", typeof(ControlTemplate), typeof(ItemBorderControl), new FrameworkPropertyMetadata(null));
			HoverTemplateProperty = DependencyPropertyManager.Register("HoverTemplate", typeof(ControlTemplate), typeof(ItemBorderControl), new FrameworkPropertyMetadata(null));
			PressedTemplateProperty = DependencyPropertyManager.Register("PressedTemplate", typeof(ControlTemplate), typeof(ItemBorderControl), new FrameworkPropertyMetadata(null));
			CustomizationTemplateProperty = DependencyPropertyManager.Register("CustomizationTemplate", typeof(ControlTemplate), typeof(ItemBorderControl), new FrameworkPropertyMetadata(null));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemBorderControl), new FrameworkPropertyMetadata(typeof(ItemBorderControl)));
		}
		protected static void OnItemPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemBorderControl)d).OnItemPositionChanged(e);
		}
		protected static void OnLinkControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemBorderControl)d).OnLinkControlChanged(e);
		}
		protected static void OnHideBorderSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemBorderControl)d).OnHideBorderSideChanged(e);
		}
		protected static void OnArrowHideBorderSidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemBorderControl)d).OnArrowHideBorderSideChanged(e);
		}
		protected static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemBorderControl)d).OnIsActiveChanged(e);
		}
		protected static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ItemBorderControl)d).OnStateChanged(e);
		}
		#endregion
		public ItemBorderControl() {
			Loaded += new RoutedEventHandler(OnLoaded);
			IsTabStop = false;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			LinkControl = LayoutHelper.FindParentObject<BarItemLinkControl>(this);
			UpdateVisualStates();
		}
		public ControlTemplate NormalTemplate {
			get { return (ControlTemplate)GetValue(NormalTemplateProperty); }
			set { SetValue(NormalTemplateProperty, value); }
		}
		public ControlTemplate HoverTemplate {
			get { return (ControlTemplate)GetValue(HoverTemplateProperty); }
			set { SetValue(HoverTemplateProperty, value); }
		}
		public ControlTemplate PressedTemplate {
			get { return (ControlTemplate)GetValue(PressedTemplateProperty); }
			set { SetValue(PressedTemplateProperty, value); }
		}
		public ControlTemplate CustomizationTemplate {
			get { return (ControlTemplate)GetValue(CustomizationTemplateProperty); }
			set { SetValue(CustomizationTemplateProperty, value); }
		}
		public HorizontalItemPositionType ItemPosition {
			get { return (HorizontalItemPositionType)GetValue(ItemPositionProperty); }
			set { SetValue(ItemPositionProperty, value); }
		}
		public BarItemLinkControl LinkControl {
			get { return (BarItemLinkControl)GetValue(LinkControlProperty); }
			set { SetValue(LinkControlProperty, value); }
		}
		public HideBorderSide HideBorderSide {
			get { return (HideBorderSide)GetValue(HideBorderSideProperty); }
			set { SetValue(HideBorderSideProperty, value); }
		}
		public HideBorderSide ArrowHideBorderSide {
			get { return (HideBorderSide)GetValue(ArrowHideBorderSideProperty); }
			set { SetValue(ArrowHideBorderSideProperty, value); }
		}
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public BorderState State {
			get { return (BorderState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		protected virtual void OnLinkControlChanged(DependencyPropertyChangedEventArgs e) {
			CreateBindings();
		}
		protected virtual void CreateBindings() {
		}
		protected virtual void UpdateVisualStateByItemPosition() {
			if(LinkControl == null)
				return;
			LinkContainerType type = LinkControl.ContainerType;
			if(type != LinkContainerType.BarButtonGroup)
				return;
			VisualStateManager.GoToState(this, "ItemPosition" + ItemPosition.ToString(), false);
		}
		protected virtual void OnItemPositionChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByItemPosition();
			UpdateVisualStateByHideBorderSide();
		}
		protected virtual void OnHideBorderSideChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByHideBorderSide();
		}
		protected virtual void UpdateVisualStateByHideBorderSide() {
			VisualStateManager.GoToState(this, HideBorderSide.ToString(), false);
		}
		protected virtual void OnArrowHideBorderSideChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByArrowHideBorderSide();
		}
		protected virtual void UpdateVisualStateByArrowHideBorderSide() {
			VisualStateManager.GoToState(this, "Arrow" + HideBorderSide.ToString(), false);
		}
		protected virtual void OnIsActiveChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByActive();
		}
		protected virtual void UpdateVisualStateByActive() {
			VisualStateManager.GoToState(this, IsActive ? "Active" : "Inactive", false);
		}
		protected virtual void OnStateChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByState();
		}
		protected virtual void UpdateVisualStateByState() {
			switch(State) {
				case BorderState.Normal:
				case BorderState.Disabled:
					if(NormalTemplate != null) {
						Template = NormalTemplate;
						VisualStateManager.GoToState(this, State.ToString(), false);
						return;
					}
					break;
				case BorderState.Hover:
				case BorderState.HoverChecked:
					if(HoverTemplate != null) {
						Template = HoverTemplate;
						return;
					}
					break;
				case BorderState.Pressed:
				case BorderState.Checked:
					if(PressedTemplate != null) {
						Template = PressedTemplate;
						return;
					}
					break;
				case BorderState.Customization:
					if(CustomizationTemplate != null) {
						Template = CustomizationTemplate;
						return;
					}
					break;
			}
			Template = NormalTemplate;
			VisualStateManager.GoToState(this, State.ToString(), false);
		}
		protected virtual void UpdateVisualStates() {
			UpdateVisualStateByArrowHideBorderSide();
			UpdateVisualStateByActive();
			UpdateVisualStateByState();
			UpdateVisualStateByItemPosition();
			UpdateVisualStateByHideBorderSide();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualStates();
		}
	}
	public enum HorizontalItemPositionType { Default, Single, Left, Center, Right }
	public class ItemPositionTypeProvider : DependencyObject {
		public static readonly DependencyProperty HorizontalItemPositionProperty;
		static ItemPositionTypeProvider() {
			HorizontalItemPositionProperty = DependencyPropertyManager.RegisterAttached("HorizontalItemPosition", typeof(HorizontalItemPositionType), typeof(ItemPositionTypeProvider),
				new FrameworkPropertyMetadata(HorizontalItemPositionType.Single, FrameworkPropertyMetadataOptions.AffectsMeasure, OnHorizontalItemPositionPropertyChanged));
		}
		public static void SetHorizontalItemPosition(DependencyObject d, HorizontalItemPositionType value) { d.SetValue(HorizontalItemPositionProperty, value); }
		public static HorizontalItemPositionType GetHorizontalItemPosition(DependencyObject d) { return (HorizontalItemPositionType)d.GetValue(HorizontalItemPositionProperty); }
		static protected void OnHorizontalItemPositionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			BarItemLinkControl lC = obj as BarItemLinkControl;
			if(lC != null)
				lC.UpdateLayoutPanelHorizontalItemPosition();
		}
	}
}
