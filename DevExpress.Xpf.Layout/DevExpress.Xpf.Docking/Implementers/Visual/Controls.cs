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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Core.Native;
using System;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class ControlBox : FrameworkElement {
		#region static
		public static readonly DependencyProperty HotButtonProperty;
		public static readonly DependencyProperty PressedButtonProperty;
		public static readonly DependencyProperty MDIButtonMinimizeTemplateProperty;
		public static readonly DependencyProperty MDIButtonRestoreTemplateProperty;
		public static readonly DependencyProperty MDIButtonCloseTemplateProperty;
		public static readonly DependencyProperty MDIButtonBorderStyleProperty;
		static ControlBox() {
			var dProp = new DependencyPropertyRegistrator<ControlBox>();
			dProp.RegisterAttachedInherited("HotButton", ref HotButtonProperty, HitTestType.Undefined);
			dProp.RegisterAttachedInherited("PressedButton", ref PressedButtonProperty, HitTestType.Undefined);
			dProp.Register("MDIButtonMinimizeTemplate", ref MDIButtonMinimizeTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((ControlBox)dObj).OnMDIButtonMinimizeChanged((DataTemplate)e.NewValue));
			dProp.Register("MDIButtonRestoreTemplate", ref MDIButtonRestoreTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((ControlBox)dObj).OnMDIButtonRestoreChanged((DataTemplate)e.NewValue));
			dProp.Register("MDIButtonCloseTemplate", ref MDIButtonCloseTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((ControlBox)dObj).OnMDIButtonCloseChanged((DataTemplate)e.NewValue));
			dProp.Register("MDIButtonBorderStyle", ref MDIButtonBorderStyleProperty, (Style)null,
				(dObj, e) => ((ControlBox)dObj).OnMDIButtonBorderStyleChanged((Style)e.NewValue));
		}
		public static HitTestType GetHotButton(DependencyObject obj) {
			return (HitTestType)obj.GetValue(HotButtonProperty);
		}
		public static void SetHotButton(DependencyObject obj, HitTestType value) {
			obj.SetValue(HotButtonProperty, value);
		}
		public static HitTestType GetPressedButton(DependencyObject obj) {
			return (HitTestType)obj.GetValue(PressedButtonProperty);
		}
		public static void SetPressedButton(DependencyObject obj, HitTestType value) {
			obj.SetValue(PressedButtonProperty, value);
		}
		#endregion static
		public ControlBox() {
			Loaded += new RoutedEventHandler(ControlBox_Loaded);
		}
		void ControlBox_Loaded(object sender, RoutedEventArgs e) {
			OnMDIButtonCloseChanged(MDIButtonCloseTemplate);
			OnMDIButtonRestoreChanged(MDIButtonRestoreTemplate);
			OnMDIButtonMinimizeChanged(MDIButtonMinimizeTemplate);
			OnMDIButtonBorderStyleChanged(MDIButtonBorderStyle);
		}
		public Style MDIButtonBorderStyle {
			get { return (Style)GetValue(MDIButtonBorderStyleProperty); }
			set { SetValue(MDIButtonBorderStyleProperty, value); }
		}
		public DataTemplate MDIButtonMinimizeTemplate {
			get { return (DataTemplate)GetValue(MDIButtonMinimizeTemplateProperty); }
			set { SetValue(MDIButtonMinimizeTemplateProperty, value); }
		}
		public DataTemplate MDIButtonRestoreTemplate {
			get { return (DataTemplate)GetValue(MDIButtonRestoreTemplateProperty); }
			set { SetValue(MDIButtonRestoreTemplateProperty, value); }
		}
		public DataTemplate MDIButtonCloseTemplate {
			get { return (DataTemplate)GetValue(MDIButtonCloseTemplateProperty); }
			set { SetValue(MDIButtonCloseTemplateProperty, value); }
		}
		protected virtual void OnMDIButtonMinimizeChanged(DataTemplate value) {
			PrepareMDIButton(GetBarItem(MDIMenuBar.ItemType.Minimize), value);
		}
		protected virtual void OnMDIButtonRestoreChanged(DataTemplate value) {
			PrepareMDIButton(GetBarItem(MDIMenuBar.ItemType.Restore), value);
		}
		protected virtual void OnMDIButtonCloseChanged(DataTemplate value) {
			PrepareMDIButton(GetBarItem(MDIMenuBar.ItemType.Close), value);
		}
		protected virtual void PrepareMDIButton(BarMDIButtonItem barItem, DataTemplate value) {
			if(barItem != null) {
				barItem.GlyphTemplate = value;
			}
		}
		protected virtual void OnMDIButtonBorderStyleChanged(Style value) {
			DockLayoutManager manager = DockLayoutManager.Ensure(this);
			if(manager != null && !manager.IsDisposing) {
				manager.MDIController.MDIMenuBar.UpdateMDIButtonBorderStyle(manager, value);
			}
		}
		BarMDIButtonItem GetBarItem(MDIMenuBar.ItemType type) {
			DockLayoutManager manager = DockLayoutManager.Ensure(this);
			return (manager != null && !manager.IsDisposing) ?
				manager.MDIController.MDIMenuBar.GetBarItem(type) : null;
		}
	}
	[TemplatePart(Name = "PART_CustomContent", Type = typeof(ContentPresenter))]
	[TemplatePart(Name = "PART_CloseButton", Type = typeof(ControlBoxButtonPresenter))]
	public class BaseControlBoxControl : psvControl {
		#region static
		public static readonly DependencyProperty ButtonWidthProperty;
		public static readonly DependencyProperty ButtonHeightProperty;
		public static readonly DependencyProperty CloseButtonTemplateProperty;
		public static readonly DependencyProperty LayoutItemProperty;
		static BaseControlBoxControl() {
			var dProp = new DependencyPropertyRegistrator<BaseControlBoxControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("ButtonWidth", ref ButtonWidthProperty, double.NaN);
			dProp.Register("ButtonHeight", ref ButtonHeightProperty, double.NaN);
			dProp.Register("CloseButtonTemplate", ref CloseButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, ea) => ((BaseControlBoxControl)dObj).OnLayoutItemChanged((BaseLayoutItem)ea.NewValue));
		}
		#endregion static
		public BaseControlBoxControl() {
			Background = FreezedBrushes.Transparent;
			DockPane.SetHitTestType(this, Base.HitTestType.ControlBox);
		}
		protected Panel PartButtonsPanel { get; private set; }
		public ContentPresenter PartCustomContent { get; private set; }
		public ControlBoxButtonPresenter PartCloseButton { get; private set; }
		internal static object GetToolTip(DockingStringId dockingStringId) {
			string text = DockingLocalizer.GetString(dockingStringId);
			return string.IsNullOrEmpty(text) ? null : text;
		}
		protected override void OnDispose() {
			ClearControlBoxBindings();
			LayoutUpdated -= BaseControlBoxControl_LayoutUpdated;
			LayoutItem = null;
			if(PartButtonsPanel != null) {
				PartButtonsPanel.LayoutUpdated -= PartButtonsPanel_LayoutUpdated;
				PartButtonsPanel = null;
			}
			if(PartCloseButton != null) {
				PartCloseButton.Dispose();
				PartCloseButton = null;
			}
			base.OnDispose();
		}
		protected T EnsurePresenter<T>(T part, string name) where T : psvContentPresenter {
			if(part != null && !LayoutItemsHelper.IsTemplateChild(part, this))
				part.Dispose();
			return GetTemplateChild(name) as T;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartButtonsPanel = LayoutItemsHelper.GetChild<Panel>(this);
			PartCustomContent = GetTemplateChild("PART_CustomContent") as ContentPresenter;
			PartCloseButton = EnsurePresenter(PartCloseButton, "PART_CloseButton");
			if(PartCloseButton != null)
				PartCloseButton.SetToolTip(GetToolTip(DockingStringId.ControlButtonClose));
			LayoutUpdated += BaseControlBoxControl_LayoutUpdated;
		}
		void BaseControlBoxControl_LayoutUpdated(object sender, System.EventArgs e) {
			LayoutUpdated -= BaseControlBoxControl_LayoutUpdated;
			EnsureControlBoxContent();
			if(PartButtonsPanel != null)
				PartButtonsPanel.LayoutUpdated += PartButtonsPanel_LayoutUpdated;
		}
		void PartButtonsPanel_LayoutUpdated(object sender, System.EventArgs e) {
			PartButtonsPanel.LayoutUpdated -= PartButtonsPanel_LayoutUpdated;
			BaseHeadersPanel.Invalidate(this);
			FloatPanePresenter.Invalidate(this);
		}
		void EnsureControlBoxContent() {
			if(LayoutItem != null)
				SetControlBoxBindings();
			else
				ClearControlBoxBindings();
		}
		protected virtual void SetControlBoxBindings() {
			if(PartCustomContent != null) {
				BindingHelper.SetBinding(PartCustomContent, ContentPresenter.ContentProperty, LayoutItem, "ControlBoxContent");
				BindingHelper.SetBinding(PartCustomContent, ContentPresenter.ContentTemplateProperty, LayoutItem, "ControlBoxContentTemplate");
				BindingHelper.SetBinding(PartCustomContent, ContentPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsControlBoxVisibleProperty, new BooleanToVisibilityConverter());
			}
			if(PartCloseButton != null)
				BindingHelper.SetBinding(PartCloseButton, ContentPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsCloseButtonVisibleProperty, new BooleanToVisibilityConverter());
		}
		protected virtual void ClearControlBoxBindings() {
			if(PartCustomContent != null) {
				PartCustomContent.ClearValue(ContentPresenter.ContentProperty);
				PartCustomContent.ClearValue(ContentPresenter.ContentTemplateProperty);
				PartCustomContent.ClearValue(ContentPresenter.VisibilityProperty);
			}
			if(PartCloseButton != null)
				PartCloseButton.ClearValue(ContentPresenter.VisibilityProperty);
		}
		protected virtual void OnLayoutItemChanged(BaseLayoutItem item) {
			EnsureControlBoxContent();
		}
		protected internal virtual void SetHotButton(HitTestType hitTest) {
			if(PartCloseButton != null)
				PartCloseButton.SetIsHot(hitTest == HitTestType.CloseButton);
		}
		protected internal virtual void SetPressedButton(HitTestType hitTest) {
			if(PartCloseButton != null)
				PartCloseButton.SetIsPresesd(hitTest == HitTestType.CloseButton);
		}
		public double ButtonWidth {
			get { return (double)GetValue(ButtonWidthProperty); }
			set { SetValue(ButtonWidthProperty, value); }
		}
		public double ButtonHeight {
			get { return (double)GetValue(ButtonHeightProperty); }
			set { SetValue(ButtonHeightProperty, value); }
		}
		public DataTemplate CloseButtonTemplate {
			get { return (DataTemplate)GetValue(CloseButtonTemplateProperty); }
			set { SetValue(CloseButtonTemplateProperty, value); }
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
	}
	[TemplatePart(Name = "PART_PinButton", Type = typeof(ControlBoxButtonPresenter))]
	public class PanelControlBoxControl : BaseControlBoxControl {
		#region static
		public static readonly DependencyProperty PinButtonTemplateProperty;
		public static readonly DependencyProperty MaximizeButtonTemplateProperty;
		public static readonly DependencyProperty RestoreButtonTemplateProperty;
		public static readonly DependencyProperty HideButtonTemplateProperty;
		public static readonly DependencyProperty ExpandButtonTemplateProperty;
		public static readonly DependencyProperty CollapseButtonTemplateProperty;
		static PanelControlBoxControl() {
			var dProp = new DependencyPropertyRegistrator<PanelControlBoxControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("PinButtonTemplate", ref PinButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("MaximizeButtonTemplate", ref MaximizeButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("RestoreButtonTemplate", ref RestoreButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("HideButtonTemplate", ref HideButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("ExpandButtonTemplate", ref ExpandButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("CollapseButtonTemplate", ref CollapseButtonTemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public PanelControlBoxControl() {
		}
		protected override void OnDispose() {
			if(PartPinButton != null) {
				PartPinButton.Dispose();
				PartPinButton = null;
			}
			base.OnDispose();
		}
		public ControlBoxButtonPresenter PartPinButton { get; private set; }
		public ControlBoxButtonPresenter PartMaximizeButton { get; private set; }
		public ControlBoxButtonPresenter PartRestoreButton { get; private set; }
		public ControlBoxButtonPresenter PartHideButton { get; private set; }
		public ControlBoxButtonPresenter PartExpandButton { get; private set; }
		public ControlBoxButtonPresenter PartCollapseButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartPinButton = EnsurePresenter(PartPinButton, "PART_PinButton");
			PartMaximizeButton = EnsurePresenter(PartMaximizeButton, "PART_MaximizeButton");
			PartRestoreButton = EnsurePresenter(PartRestoreButton, "PART_RestoreButton");
			PartHideButton = EnsurePresenter(PartHideButton, "PART_HideButton");
			PartExpandButton = EnsurePresenter(PartRestoreButton, "PART_ExpandButton");
			PartCollapseButton = EnsurePresenter(PartCollapseButton, "PART_CollapseButton");
			if(PartPinButton != null)
				PartPinButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonAutoHide));
			if(PartMaximizeButton != null)
				PartMaximizeButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonMaximize));
			if(PartRestoreButton != null)
				PartRestoreButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonRestore));
			PartHideButton.Do(x => x.AttachToolTip(GetToolTip(DockingStringId.ControlButtonHide)));
			PartExpandButton.Do(x => x.AttachToolTip(GetToolTip(DockingStringId.ControlButtonExpand)));
			PartCollapseButton.Do(x => x.AttachToolTip(GetToolTip(DockingStringId.ControlButtonCollapse)));
		}
		protected override void SetControlBoxBindings() {
			base.SetControlBoxBindings();
			if(PartPinButton != null)
				BindingHelper.SetBinding(PartPinButton, UIElement.VisibilityProperty, LayoutItem, BaseLayoutItem.IsPinButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(PartMaximizeButton != null)
				BindingHelper.SetBinding(PartMaximizeButton, UIElement.VisibilityProperty, LayoutItem, BaseLayoutItem.IsMaximizeButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(PartRestoreButton != null)
				BindingHelper.SetBinding(PartRestoreButton, UIElement.VisibilityProperty, LayoutItem, BaseLayoutItem.IsRestoreButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(LayoutItem is LayoutPanel) {
				PartHideButton.Do(x => BindingHelper.SetBinding(x, UIElement.VisibilityProperty, LayoutItem, LayoutPanel.IsHideButtonVisibleProperty, new BooleanToVisibilityConverter()));
				PartExpandButton.Do(x => BindingHelper.SetBinding(x, UIElement.VisibilityProperty, LayoutItem, LayoutPanel.IsExpandButtonVisibleProperty, new BooleanToVisibilityConverter()));
				PartCollapseButton.Do(x => BindingHelper.SetBinding(x, UIElement.VisibilityProperty, LayoutItem, LayoutPanel.IsCollapseButtonVisibleProperty, new BooleanToVisibilityConverter()));
			}
		}
		protected override void ClearControlBoxBindings() {
			if(PartPinButton != null)
				PartPinButton.ClearValue(ContentPresenter.VisibilityProperty);
			if(PartMaximizeButton != null)
				PartMaximizeButton.ClearValue(ContentPresenter.VisibilityProperty);
			if(PartRestoreButton != null)
				PartRestoreButton.ClearValue(ContentPresenter.VisibilityProperty);
			PartHideButton.Do(x => x.ClearValue(ContentPresenter.VisibilityProperty));
			PartExpandButton.Do(x => x.ClearValue(ContentPresenter.VisibilityProperty));
			PartCollapseButton.Do(x => x.ClearValue(ContentPresenter.VisibilityProperty));
			base.ClearControlBoxBindings();
		}
		protected internal override void SetHotButton(HitTestType hitTest) {
			base.SetHotButton(hitTest);
			PartPinButton.Do(x => x.SetIsHot(hitTest == HitTestType.PinButton));
			PartMaximizeButton.Do(x => x.SetIsHot(hitTest == HitTestType.MaximizeButton));
			PartRestoreButton.Do(x => x.SetIsHot(hitTest == HitTestType.RestoreButton));
			PartHideButton.Do(x => x.SetIsHot(hitTest == HitTestType.HideButton));
			PartExpandButton.Do(x => x.SetIsHot(hitTest == HitTestType.ExpandButton));
			PartCollapseButton.Do(x => x.SetIsHot(hitTest == HitTestType.CollapseButton));
		}
		protected internal override void SetPressedButton(HitTestType hitTest) {
			base.SetPressedButton(hitTest);
			PartPinButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.PinButton));
			PartMaximizeButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.MaximizeButton));
			PartRestoreButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.RestoreButton));
			PartHideButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.HideButton));
			PartExpandButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.ExpandButton));
			PartCollapseButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.CollapseButton));
		}
		public DataTemplate PinButtonTemplate {
			get { return (DataTemplate)GetValue(PinButtonTemplateProperty); }
			set { SetValue(PinButtonTemplateProperty, value); }
		}
		public DataTemplate MaximizeButtonTemplate {
			get { return (DataTemplate)GetValue(MaximizeButtonTemplateProperty); }
			set { SetValue(MaximizeButtonTemplateProperty, value); }
		}
		public DataTemplate RestoreButtonTemplate {
			get { return (DataTemplate)GetValue(RestoreButtonTemplateProperty); }
			set { SetValue(RestoreButtonTemplateProperty, value); }
		}
		public DataTemplate HideButtonTemplate {
			get { return (DataTemplate)GetValue(HideButtonTemplateProperty); }
			set { SetValue(HideButtonTemplateProperty, value); }
		}
		public DataTemplate ExpandButtonTemplate {
			get { return (DataTemplate)GetValue(ExpandButtonTemplateProperty); }
			set { SetValue(ExpandButtonTemplateProperty, value); }
		}
		public DataTemplate CollapseButtonTemplate {
			get { return (DataTemplate)GetValue(CollapseButtonTemplateProperty); }
			set { SetValue(CollapseButtonTemplateProperty, value); }
		}
	}
	[TemplatePart(Name = "PART_MinimizeButton", Type = typeof(ControlBoxButtonPresenter))]
	[TemplatePart(Name = "PART_MaximizeButton", Type = typeof(ControlBoxButtonPresenter))]
	[TemplatePart(Name = "PART_RestoreButton", Type = typeof(ControlBoxButtonPresenter))]
	public class WindowControlBoxControl : BaseControlBoxControl {
		#region static
		public static readonly DependencyProperty MinimizeButtonTemplateProperty;
		public static readonly DependencyProperty MaximizeButtonTemplateProperty;
		public static readonly DependencyProperty RestoreButtonTemplateProperty;
		static WindowControlBoxControl() {
			var dProp = new DependencyPropertyRegistrator<WindowControlBoxControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("MinimizeButtonTemplate", ref MinimizeButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("MaximizeButtonTemplate", ref MaximizeButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("RestoreButtonTemplate", ref RestoreButtonTemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public WindowControlBoxControl() {
		}
		protected override void OnDispose() {
			if(PartMinimizeButton != null) {
				PartMinimizeButton.Dispose();
				PartMinimizeButton = null;
			}
			if(PartMaximizeButton != null) {
				PartMaximizeButton.Dispose();
				PartMaximizeButton = null;
			}
			if(PartRestoreButton != null) {
				PartRestoreButton.Dispose();
				PartRestoreButton = null;
			}
			base.OnDispose();
		}
		public ControlBoxButtonPresenter PartMinimizeButton { get; private set; }
		public ControlBoxButtonPresenter PartMaximizeButton { get; private set; }
		public ControlBoxButtonPresenter PartRestoreButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartMinimizeButton = EnsurePresenter(PartMinimizeButton, "PART_MinimizeButton");
			PartMaximizeButton = EnsurePresenter(PartMaximizeButton, "PART_MaximizeButton");
			PartRestoreButton = EnsurePresenter(PartRestoreButton, "PART_RestoreButton");
			if(PartMaximizeButton != null)
				PartMaximizeButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonMaximize));
			if(PartMinimizeButton != null)
				PartMinimizeButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonMinimize));
			if(PartRestoreButton != null)
				PartRestoreButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonRestore));
		}
		protected override void SetControlBoxBindings() {
			base.SetControlBoxBindings();
			if(PartMinimizeButton != null)
				BindingHelper.SetBinding(PartMinimizeButton, ContentPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsMinimizeButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(PartMaximizeButton != null)
				BindingHelper.SetBinding(PartMaximizeButton, ContentPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsMaximizeButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(PartRestoreButton != null)
				BindingHelper.SetBinding(PartRestoreButton, ContentPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsRestoreButtonVisibleProperty, new BooleanToVisibilityConverter());
		}
		protected override void ClearControlBoxBindings() {
			if(PartMinimizeButton != null)
				PartMinimizeButton.ClearValue(ContentPresenter.VisibilityProperty);
			if(PartMaximizeButton != null)
				PartMaximizeButton.ClearValue(ContentPresenter.VisibilityProperty);
			if(PartRestoreButton != null)
				PartRestoreButton.ClearValue(ContentPresenter.VisibilityProperty);
			base.ClearControlBoxBindings();
		}
		protected internal override void SetHotButton(HitTestType hitTest) {
			base.SetHotButton(hitTest);
			if(PartMaximizeButton != null)
				PartMaximizeButton.SetIsHot(hitTest == HitTestType.MaximizeButton);
			if (PartMinimizeButton!=null)
				PartMinimizeButton.SetIsHot(hitTest == HitTestType.MinimizeButton);
			if(PartRestoreButton != null)
				PartRestoreButton.SetIsHot(hitTest == HitTestType.RestoreButton);
		}
		protected internal override void SetPressedButton(HitTestType hitTest) {
			base.SetPressedButton(hitTest);
			if(PartMaximizeButton != null)
				PartMaximizeButton.SetIsPresesd(hitTest == HitTestType.MaximizeButton);
			if(PartMinimizeButton != null)
				PartMinimizeButton.SetIsPresesd(hitTest == HitTestType.MinimizeButton);
			if(PartRestoreButton != null)
				PartRestoreButton.SetIsPresesd(hitTest == HitTestType.RestoreButton);
		}
		public DataTemplate MinimizeButtonTemplate {
			get { return (DataTemplate)GetValue(MinimizeButtonTemplateProperty); }
			set { SetValue(MinimizeButtonTemplateProperty, value); }
		}
		public DataTemplate MaximizeButtonTemplate {
			get { return (DataTemplate)GetValue(MaximizeButtonTemplateProperty); }
			set { SetValue(MaximizeButtonTemplateProperty, value); }
		}
		public DataTemplate RestoreButtonTemplate {
			get { return (DataTemplate)GetValue(RestoreButtonTemplateProperty); }
			set { SetValue(RestoreButtonTemplateProperty, value); }
		}
	}
	[TemplatePart(Name = "PART_ScrollPrevButton", Type = typeof(ControlBoxButtonPresenter))]
	[TemplatePart(Name = "PART_ScrollNextButton", Type = typeof(ControlBoxButtonPresenter))]
	[TemplatePart(Name = "PART_DropDownButton", Type = typeof(ControlBoxButtonPresenter))]
	[TemplatePart(Name = "PART_RestoreButton", Type = typeof(ControlBoxButtonPresenter))]
	public class TabHeaderControlBoxControl : BaseControlBoxControl {
		#region static
		public static readonly DependencyProperty ScrollPrevButtonTemplateProperty;
		public static readonly DependencyProperty ScrollNextButtonTemplateProperty;
		public static readonly DependencyProperty DropDownButtonTemplateProperty;
		public static readonly DependencyProperty RestoreButtonTemplateProperty;
		public static readonly DependencyProperty LocationProperty;
		static TabHeaderControlBoxControl() {
			var dProp = new DependencyPropertyRegistrator<TabHeaderControlBoxControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("ScrollPrevButtonTemplate", ref ScrollPrevButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("ScrollNextButtonTemplate", ref ScrollNextButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("DropDownButtonTemplate", ref DropDownButtonTemplateProperty, (DataTemplate)null);
			dProp.Register("RestoreButtonTemplate", ref RestoreButtonTemplateProperty, (DataTemplate)null);
			dProp.RegisterAttachedInherited("Location", ref LocationProperty, CaptionLocation.Default);
		}
		public static CaptionLocation GetLocation(DependencyObject obj) {
			return (CaptionLocation)obj.GetValue(LocationProperty);
		}
		public static void SetLocation(DependencyObject obj, DockLayoutManager value) {
			obj.SetValue(LocationProperty, value);
		}
		#endregion static
		public TabHeaderControlBoxControl() {
		}
		protected override void OnDispose() {
			if(PartScrollPrevButton != null) {
				PartScrollPrevButton.Dispose();
				PartScrollPrevButton = null;
			}
			if(PartScrollNextButton != null) {
				PartScrollNextButton.Dispose();
				PartScrollNextButton = null;
			}
			if(PartDropDownButton != null) {
				PartDropDownButton.Dispose();
				PartDropDownButton = null;
			}
			if(PartRestoreButton != null) {
				PartRestoreButton.Dispose();
				PartRestoreButton = null;
			}
			base.OnDispose();
		}
		public ControlBoxButtonPresenter PartScrollPrevButton { get; private set; }
		public ControlBoxButtonPresenter PartScrollNextButton { get; private set; }
		public ControlBoxButtonPresenter PartDropDownButton { get; private set; }
		public ControlBoxButtonPresenter PartRestoreButton { get; private set; }
		public StackPanel PartStackPanel { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartScrollPrevButton = EnsurePresenter(PartScrollPrevButton, "PART_ScrollPrevButton");
			PartScrollNextButton = EnsurePresenter(PartScrollPrevButton, "PART_ScrollNextButton");
			PartDropDownButton = EnsurePresenter(PartDropDownButton, "PART_DropDownButton");
			PartRestoreButton = EnsurePresenter(PartRestoreButton, "PART_RestoreButton");
			PartStackPanel = GetTemplateChild("PART_StackPanel") as StackPanel;
			if(PartScrollPrevButton != null)
				PartScrollPrevButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonScrollPrev));
			if(PartScrollNextButton != null)
				PartScrollNextButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonScrollNext));
			if(PartRestoreButton != null)
				PartRestoreButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonRestore));
		}
		protected override void SetControlBoxBindings() {
			base.SetControlBoxBindings();
			if(PartScrollPrevButton != null) {
				BindingHelper.SetBinding(PartScrollPrevButton, ControlBoxButtonPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsScrollPrevButtonVisibleProperty, new BooleanToVisibilityConverter());
				BindingHelper.SetBinding(PartScrollPrevButton, ControlBoxButtonPresenter.IsEnabledProperty, LayoutItem, LayoutGroup.TabHeaderCanScrollPrevProperty);
			}
			if(PartScrollNextButton != null) {
				BindingHelper.SetBinding(PartScrollNextButton, ControlBoxButtonPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsScrollNextButtonVisibleProperty, new BooleanToVisibilityConverter());
				BindingHelper.SetBinding(PartScrollNextButton, ControlBoxButtonPresenter.IsEnabledProperty, LayoutItem, LayoutGroup.TabHeaderCanScrollNextProperty);
			}
			if(PartDropDownButton != null)
				BindingHelper.SetBinding(PartDropDownButton, ContentPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsDropDownButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(PartRestoreButton != null)
				BindingHelper.SetBinding(PartRestoreButton, ControlBoxButtonPresenter.VisibilityProperty, LayoutItem, BaseLayoutItem.IsRestoreButtonVisibleProperty, new BooleanToVisibilityConverter());
			if(PartStackPanel != null) {
				BindingHelper.SetBinding(PartStackPanel, StackPanel.OrientationProperty, LayoutItem, BaseLayoutItem.CaptionLocationProperty, new TabHeaderCaptionLocationToOrientationConverter());
			}
		}
		protected override void ClearControlBoxBindings() {
			if(PartScrollPrevButton != null) {
				PartScrollPrevButton.ClearValue(ControlBoxButtonPresenter.VisibilityProperty);
				PartScrollPrevButton.ClearValue(ControlBoxButtonPresenter.IsEnabledProperty);
			}
			if(PartScrollNextButton != null) {
				PartScrollNextButton.ClearValue(ControlBoxButtonPresenter.VisibilityProperty);
				PartScrollNextButton.ClearValue(ControlBoxButtonPresenter.IsEnabledProperty);
			}
			if(PartDropDownButton != null)
				PartDropDownButton.ClearValue(ControlBoxButtonPresenter.VisibilityProperty);
			if(PartRestoreButton != null)
				PartRestoreButton.ClearValue(ControlBoxButtonPresenter.VisibilityProperty);
			if(PartStackPanel != null)
				PartStackPanel.ClearValue(StackPanel.OrientationProperty);
			base.ClearControlBoxBindings();
		}
		protected internal override void SetHotButton(HitTestType hitTest) {
			base.SetHotButton(hitTest);
			if(PartScrollPrevButton != null)
				PartScrollPrevButton.SetIsHot(hitTest == HitTestType.ScrollPrevButton);
			if(PartScrollNextButton != null)
				PartScrollNextButton.SetIsHot(hitTest == HitTestType.ScrollNextButton);
			if(PartDropDownButton != null)
				PartDropDownButton.SetIsHot(hitTest == HitTestType.DropDownButton);
			if(PartRestoreButton != null)
				PartRestoreButton.SetIsHot(hitTest == HitTestType.RestoreButton);
		}
		protected internal override void SetPressedButton(HitTestType hitTest) {
			base.SetPressedButton(hitTest);
			if(PartScrollPrevButton != null)
				PartScrollPrevButton.SetIsPresesd(hitTest == HitTestType.ScrollPrevButton);
			if(PartScrollNextButton != null)
				PartScrollNextButton.SetIsPresesd(hitTest == HitTestType.ScrollNextButton);
			if(PartDropDownButton != null)
				PartDropDownButton.SetIsPresesd(hitTest == HitTestType.DropDownButton);
			if(PartRestoreButton != null)
				PartRestoreButton.SetIsPresesd(hitTest == HitTestType.RestoreButton);
		}
		public DataTemplate ScrollPrevButtonTemplate {
			get { return (DataTemplate)GetValue(ScrollPrevButtonTemplateProperty); }
			set { SetValue(ScrollPrevButtonTemplateProperty, value); }
		}
		public DataTemplate ScrollNextButtonTemplate {
			get { return (DataTemplate)GetValue(ScrollNextButtonTemplateProperty); }
			set { SetValue(ScrollNextButtonTemplateProperty, value); }
		}
		public DataTemplate DropDownButtonTemplate {
			get { return (DataTemplate)GetValue(DropDownButtonTemplateProperty); }
			set { SetValue(DropDownButtonTemplateProperty, value); }
		}
		public DataTemplate RestoreButtonTemplate {
			get { return (DataTemplate)GetValue(RestoreButtonTemplateProperty); }
			set { SetValue(RestoreButtonTemplateProperty, value); }
		}
	}
	[TemplatePart(Name = "PART_PinButton", Type = typeof(ControlBoxButtonPresenter))]
	public class DocumentTabHeaderControlBoxControl : BaseControlBoxControl {
		#region static
		public static readonly DependencyProperty PinButtonTemplateProperty;
		static DocumentTabHeaderControlBoxControl() {
			var dProp = new DependencyPropertyRegistrator<DocumentTabHeaderControlBoxControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("PinButtonTemplate", ref PinButtonTemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public DocumentTabHeaderControlBoxControl() {
		}
		protected override void OnDispose() {
			if(PartPinButton != null) {
				PartPinButton.Dispose();
				PartPinButton = null;
			}
			base.OnDispose();
		}
		public ControlBoxButtonPresenter PartPinButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartPinButton = EnsurePresenter(PartPinButton, "PART_PinButton");
			if(PartPinButton != null)
				PartPinButton.AttachToolTip(GetToolTip(DockingStringId.ControlButtonTogglePinStatus));
		}
		protected override void SetControlBoxBindings() {
			base.SetControlBoxBindings();
			if(PartPinButton != null)
				BindingHelper.SetBinding(PartPinButton, UIElement.VisibilityProperty, LayoutItem, BaseLayoutItem.IsPinButtonVisibleProperty, new BooleanToVisibilityConverter());
		}
		protected override void ClearControlBoxBindings() {
			if(PartPinButton != null)
				PartPinButton.ClearValue(ContentPresenter.VisibilityProperty);
			base.ClearControlBoxBindings();
		}
		protected internal override void SetHotButton(HitTestType hitTest) {
			base.SetHotButton(hitTest);
			PartPinButton.Do(x => x.SetIsHot(hitTest == HitTestType.PinButton));
		}
		protected internal override void SetPressedButton(HitTestType hitTest) {
			base.SetPressedButton(hitTest);
			PartPinButton.Do(x => x.SetIsPresesd(hitTest == HitTestType.PinButton));
		}
		public DataTemplate PinButtonTemplate {
			get { return (DataTemplate)GetValue(PinButtonTemplateProperty); }
			set { SetValue(PinButtonTemplateProperty, value); }
		}
	}
	[TemplatePart(Name = "PART_ExpandButton", Type = typeof(ControlBoxButtonPresenter))]
	public class GroupBoxControlBoxControl : BaseControlBoxControl {
		#region static
		public static readonly DependencyProperty ExpandButtonTemplateProperty;
		static GroupBoxControlBoxControl() {
			var dProp = new DependencyPropertyRegistrator<GroupBoxControlBoxControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("ExpandButtonTemplate", ref ExpandButtonTemplateProperty, (DataTemplate)null);
		}
		#endregion static
		public GroupBoxControlBoxControl() {
		}
		protected override void OnDispose() {
			if(PartExpandButton != null) {
				PartExpandButton.Dispose();
				PartExpandButton = null;
			}
			base.OnDispose();
		}
		public ControlBoxButtonPresenter PartExpandButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartExpandButton = EnsurePresenter(PartExpandButton, "PART_ExpandButton");
		}
		protected override void SetControlBoxBindings() {
			base.SetControlBoxBindings();
			if(PartExpandButton != null)
				BindingHelper.SetBinding(PartExpandButton, ContentPresenter.VisibilityProperty, LayoutItem, LayoutGroup.AllowExpandProperty, new BooleanToVisibilityConverter());
		}
		protected override void ClearControlBoxBindings() {
			if(PartExpandButton != null)
				PartExpandButton.ClearValue(ContentPresenter.VisibilityProperty);
			base.ClearControlBoxBindings();
		}
		protected internal override void SetHotButton(HitTestType hitTest) {
			base.SetHotButton(hitTest);
			if(PartExpandButton != null) {
				PartExpandButton.SetIsHot(hitTest == HitTestType.ExpandButton);
			}
		}
		protected internal override void SetPressedButton(HitTestType hitTest) {
			base.SetPressedButton(hitTest);
			if(PartExpandButton != null) {
				PartExpandButton.SetIsPresesd(hitTest == HitTestType.ExpandButton);
			}
		}
		public DataTemplate ExpandButtonTemplate {
			get { return (DataTemplate)GetValue(ExpandButtonTemplateProperty); }
			set { SetValue(ExpandButtonTemplateProperty, value); }
		}
	}
	[TemplatePart(Name = "PART_Button", Type = typeof(ControlBoxButton))]
	public class ControlBoxButtonPresenter : psvContentPresenter {
		static ControlBoxButtonPresenter() {
			var dProp = new DependencyPropertyRegistrator<ControlBoxButtonPresenter>();
		}
		bool isInDesigner;
		public ControlBoxButtonPresenter() {
			isInDesigner = this.IsInDesignTool();
		}
		protected override void OnDispose() {
			if(PartButton != null) {
				PartButton.ClearValue(psvControl.IsEnabledProperty);
				PartButton.Dispose();
				PartButton = null;
			}
			base.OnDispose();
		}
		public ControlBoxButton PartButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartButton != null && !LayoutItemsHelper.IsTemplateChild(PartButton, this)) 
				PartButton.Dispose();
			PartButton = GetTemplateChild("PART_Button") as ControlBoxButton;
			if(PartButton != null) {
				this.Forward(PartButton, psvControl.IsEnabledProperty, "IsEnabled");
			}
		}
		public void SetIsHot(bool isHot) {
			if(isInDesigner) return;
		}
		public void SetIsPresesd(bool isPressed) {
			if(isInDesigner) return;
		}
		internal void SetToolTip(object tooltip) {
			this.AttachToolTip(tooltip);
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.Docking.UIAutomation.ControlBoxButtonPresenterAutomationPeer(this);
		}
		protected internal bool AutomationClick() {
			if(PartButton == null) return false;
			bool result = false;
			if(VisualTreeHelper.GetChildrenCount(PartButton) == 1) {
				DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(PartButton);
				BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
				DocumentPanel document = item as DocumentPanel;
				DocumentGroup documentGroup = item as DocumentGroup;
				DependencyObject child = VisualTreeHelper.GetChild(PartButton, 0);
				HitTestType resultType = DockPane.GetHitTestType(child);
				switch(resultType) {
					case HitTestType.CloseButton:
						result = manager.DockController.Close(item);
						break;
					case HitTestType.PinButton:
						if(item.IsAutoHidden)
							result = manager.DockController.Dock(item);
						else
							result = manager.DockController.Hide(item);
						break;
					case HitTestType.RestoreButton:
						result = manager.MDIController.Restore(document);
						break;
					case HitTestType.DropDownButton:
						if(documentGroup != null) {
							result = true;
							manager.CustomizationController.ShowItemSelectorMenu(this, documentGroup.GetItems());
						}
						break;
					case HitTestType.MinimizeButton:
						result = manager.MDIController.Minimize(document);
						break;
					case HitTestType.MaximizeButton:
						result = manager.MDIController.Maximize(document);
						break;
					case HitTestType.ExpandButton:
						LayoutGroup group = item as LayoutGroup;
						if(group != null) {
							result = true;
							group.Expanded = !group.Expanded;
						}
						break;
				}
			}
			return result;
		}
		#endregion
	}
	interface ISupportVisualStates {
		void UpdateVisualState();
	}
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	public class ControlBoxButton : psvControl, IWeakEventListener, ISupportVisualStates {
		VisualStateController VisualStateController;
		public ControlBoxButton() {
			VisualStateController = new VisualStateController(this);
		}
		FrameworkElement PartButtonBorder;
		public override void OnApplyTemplate() {
			if(PartButtonBorder != null)
				VisualStateController.Remove(PartButtonBorder);
			base.OnApplyTemplate();
			_controlBox = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<BaseControlBoxControl>(this);
			Item = _controlBox != null ? _controlBox.LayoutItem : null;
			PartButtonBorder = LayoutItemsHelper.GetTemplateChild<ControlBoxButtonBorder>(this);
			if(PartButtonBorder != null)
				VisualStateController.Add(PartButtonBorder);
		}
		protected override void OnDispose() {
			Item = null;
			base.OnDispose();
		}
		BaseControlBoxControl _controlBox;
		BaseLayoutItem _item;
		protected BaseLayoutItem Item {
			get { return _item; }
			set {
				if(_item == value) return;
				BaseLayoutItem oldValue = _item;
				_item = value;
				OnItemChanged(oldValue, value);
			}
		}
		protected virtual void OnItemChanged(BaseLayoutItem oldValue, BaseLayoutItem newValue) {
			if(oldValue != null)
				BaseLayoutItemWeakEventManager.RemoveListener(oldValue, this);
			if(newValue != null)
				BaseLayoutItemWeakEventManager.AddListener(newValue, this);
		}
		protected virtual void UpdateVisualState() {
			if(Item != null) {
				VisualStateManager.GoToState(this, "EmptyActiveState", false);
				bool isInTab = _controlBox is TabHeaderControlBoxControl;
				string selectedState = Item.IsActive ? "Active" : (Item.IsSelectedItem && isInTab ? "Selected" : "Inactive");
				VisualStateManager.GoToState(this, selectedState, false);
			}
		}
		#region IWeakEventListener Members
		public bool ReceiveWeakEvent(System.Type managerType, object sender, System.EventArgs e) {
			if(managerType == typeof(BaseLayoutItemWeakEventManager)) {
				VisualStateController.UpdateState();
				return true;
			}
			return false;
		}
		void ISupportVisualStates.UpdateVisualState() {
			UpdateVisualState();
		}
		#endregion
	}
	[TemplateVisualState(Name = "Pinned", GroupName = "PinnedStates")]
	[TemplateVisualState(Name = "Unpinned", GroupName = "PinnedStates")]
	public class DockPaneControlBoxButton : ControlBoxButton {
		#region static
		public static readonly DependencyProperty IsAutoHiddenProperty;
		static DockPaneControlBoxButton() {
			var dProp = new DependencyPropertyRegistrator<DockPaneControlBoxButton>();
			dProp.Register("IsAutoHidden", ref IsAutoHiddenProperty, false,
				(dObj, ea) => ((DockPaneControlBoxButton)dObj).OnIsAutoHiddenChanged((bool)ea.NewValue));
		}
		#endregion
		protected virtual void OnIsAutoHiddenChanged(bool isAutoHidden) {
			UpdateVisualState();
		}
		protected override void UpdateVisualState() {
			base.UpdateVisualState();
			if(Item != null)
				if(Item.IsAutoHidden || Item.IsPinnedTab) {
					VisualStateManager.GoToState(this, "Pinned", false);
					Dock dock = (Item.Parent as AutoHideGroup).Return(x => x.DockType, () => Dock.Left);
					VisualStateManager.GoToState(this, dock.ToString(), false);
				}
				else
					VisualStateManager.GoToState(this, "Unpinned", false);
		}
		public bool IsAutoHidden {
			get { return ((bool)base.GetValue(IsAutoHiddenProperty)); }
			set { base.SetValue(IsAutoHiddenProperty, value); }
		}
	}
	[TemplateVisualState(Name = "Expanded", GroupName = "ExpandedStates")]
	[TemplateVisualState(Name = "Collapsed", GroupName = "ExpandedStates")]
	public class GroupBoxControlBoxButton : ControlBoxButton {
		#region static
		public static readonly DependencyProperty IsExpandedProperty;
		static GroupBoxControlBoxButton() {
			var dProp = new DependencyPropertyRegistrator<GroupBoxControlBoxButton>();
			dProp.Register("IsExpanded", ref IsExpandedProperty, false,
				(dObj, ea) => ((GroupBoxControlBoxButton)dObj).OnIsExpandedChanged((bool)ea.NewValue));
		}
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(Group != null) {
				BindingHelper.SetBinding(this, IsExpandedProperty, Group, "IsExpanded");
			}
			UpdateToolTip();
		}
		protected virtual void UpdateToolTip() {
			object tooltip = BaseControlBoxControl.GetToolTip(IsExpanded ? DockingStringId.MenuItemCollapseGroup : DockingStringId.MenuItemExpandGroup);
			this.AttachToolTip(tooltip);
		}
		protected virtual void OnIsExpandedChanged(bool isExpanded) {
			UpdateVisualState();
			UpdateToolTip();
		}
		LayoutGroup Group { get { return Item as LayoutGroup; } }
		protected override void UpdateVisualState() {
			base.UpdateVisualState();
			if(IsExpanded)
				VisualStateManager.GoToState(this, "Expanded", false);
			else
				VisualStateManager.GoToState(this, "Collapsed", false);
		}
		public bool IsExpanded {
			get { return ((bool)base.GetValue(IsExpandedProperty)); }
			set { base.SetValue(IsExpandedProperty, value); }
		}
	}
	[TemplateVisualState(Name = VisualStateController.NormalState, GroupName = "CommonStates")]
	[TemplateVisualState(Name = VisualStateController.MouseOverState, GroupName = "CommonStates")]
	[TemplateVisualState(Name = VisualStateController.PressedState, GroupName = "CommonStates")]
	[TemplateVisualState(Name = VisualStateController.DisabledState, GroupName = "CommonStates")]
	public class ControlBoxButtonBorder : ContentControl {
		#region static
		static ControlBoxButtonBorder() {
			var dProp = new DependencyPropertyRegistrator<ControlBoxButtonBorder>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		public ControlBoxButtonBorder() {
			Focusable = false;
		}
	}
}
