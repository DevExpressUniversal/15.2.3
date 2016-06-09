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
using System.Windows.Media;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_ControlBox", Type = typeof(BaseControlBoxControl))]
	[TemplatePart(Name = "PART_CaptionControl", Type = typeof(CaptionControl))]
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class TabbedPaneItem : psvSelectorItem, IUIElement, ITabHeader {
		#region static
		public static readonly DependencyProperty CaptionOrientationProperty;
		public static readonly DependencyProperty CaptionLocationProperty;
		public static readonly DependencyProperty PinnedProperty;
		public static readonly DependencyProperty PinLocationProperty;
		static TabbedPaneItem() {
			var dProp = new DependencyPropertyRegistrator<TabbedPaneItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("CaptionOrientation", ref CaptionOrientationProperty, Orientation.Horizontal);
			dProp.Register("CaptionLocation", ref CaptionLocationProperty, CaptionLocation.Default,
				(dObj, ea) => ((TabbedPaneItem)dObj).OnCaptionLocationChanged((CaptionLocation)ea.NewValue));
			dProp.Register("Pinned", ref PinnedProperty, false);
			dProp.Register("PinLocation", ref PinLocationProperty, TabHeaderPinLocation.Default);
		}
		#endregion static
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
		public TabbedPaneItem() {
		}
		protected override void OnDispose() {
			if(PartCaptionControlPresenter != null) {
				PartCaptionControlPresenter.Dispose();
				PartCaptionControlPresenter = null;
			}
			if(PartControlBox != null) {
				PartControlBox.Dispose();
				PartControlBox = null;
			}
			base.OnDispose();
		}
		public Orientation CaptionOrientation {
			get { return (Orientation)GetValue(CaptionOrientationProperty); }
			set { SetValue(CaptionOrientationProperty, value); }
		}
		public CaptionLocation CaptionLocation {
			get { return (CaptionLocation)GetValue(CaptionLocationProperty); }
			set { SetValue(CaptionLocationProperty, value); }
		}
		public bool Pinned {
			get { return (bool)GetValue(PinnedProperty); }
			set { SetValue(PinnedProperty, value); }
		}
		public TabHeaderPinLocation PinLocation {
			get { return (TabHeaderPinLocation)GetValue(PinLocationProperty); }
			set { SetValue(PinLocationProperty, value); }
		}
		public BaseControlBoxControl PartControlBox { get; private set; }
		public TemplatedCaptionControl PartCaptionControlPresenter { get; private set; }
		Thickness controlBoxThemeMargin;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartControlBox = GetTemplateChild("PART_ControlBox") as BaseControlBoxControl;
			PartCaptionControlPresenter = GetTemplateChild("PART_CaptionControlPresenter") as TemplatedCaptionControl;
			controlBoxThemeMargin = (PartControlBox != null) ? PartControlBox.Margin : new Thickness(0);
			UpdateVisualState();
		}
		#region IHeader Members
		public bool IsPinned { get { return Pinned; } }
		public Rect ArrangeRect { get; private set; }
		public void Apply(ITabHeaderInfo info) {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
			item.CoerceValue(BaseLayoutItem.IsCaptionVisibleProperty);
			item.CoerceValue(BaseLayoutItem.IsCaptionImageVisibleProperty);
			Visibility = info.IsVisible ? Visibility.Visible : Visibility.Collapsed;
			Size measuredSize = info.Rect.GetSize();
			if(PartCaption != null || MathHelper.IsEmpty(measuredSize)) {
				Measure(measuredSize);
			}
			psvPanel.SetZIndex(this, info.ZIndex);
			ArrangeRect = info.Rect;
		}
		public CaptionControl PartCaption { 
			get { return PartCaptionControlPresenter != null ? PartCaptionControlPresenter.PartCaption : null; } 
		}
		public ITabHeaderInfo CreateInfo(Size size) {
			Visibility = Visibility.Visible;
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
			item.CoerceValue(BaseLayoutItem.IsCaptionVisibleProperty);
			item.CoerceValue(BaseLayoutItem.IsCaptionImageVisibleProperty);
			UpdateControlBoxMargins(item);
			Measure(size);
			FrameworkElement caption = PartCaption;
			if(caption == null) caption = PartCaptionControlPresenter;
			return new BaseHeaderInfo(this, caption, CaptionOrientation == Orientation.Horizontal, PartControlBox, IsSelected, PinLocation, Pinned);
		}
		#endregion
		protected virtual bool IsControlBoxActuallyVisible(BaseLayoutItem item) {
			return item.IsCloseButtonVisible || item.ControlBoxContent != null;
		}
		void UpdateControlBoxMargins(BaseLayoutItem item) {
			if(PartControlBox == null)
				return;
			if(IsControlBoxActuallyVisible(item))
				PartControlBox.Margin = controlBoxThemeMargin;
			else
				PartControlBox.Margin = new Thickness(0); 
		}
		protected override void OnIsSelectedChanged(bool selected) {
			base.OnIsSelectedChanged(selected);
			UpdateVisualState();
		}
		protected virtual void OnCaptionLocationChanged(CaptionLocation captionLocation) {
			UpdateVisualState();
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateVisualState();
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateVisualState();
		}
		void UpdateCommonState() {
			if(IsMouseOver)
				VisualStateManager.GoToState(this, "MouseOver", false);
			else
				VisualStateManager.GoToState(this, "Normal", false);
		}
		protected virtual void UpdateSelectionState() {
			VisualStateManager.GoToState(this, "EmptySelectionState", false);
			if(IsSelected)
				VisualStateManager.GoToState(this, "Selected", false);
			else
				VisualStateManager.GoToState(this, "Unselected", false);
		}
		void UpdateLocationState() {
			switch(CaptionLocation) {
				case CaptionLocation.Left:
					VisualStateManager.GoToState(this, "Left", false);
					break;
				case CaptionLocation.Right:
					VisualStateManager.GoToState(this, "Right", false);
					break;
				case CaptionLocation.Bottom:
					VisualStateManager.GoToState(this, "Bottom", false);
					break;
				default:
					VisualStateManager.GoToState(this, "Top", false);
					break;
			}
		}
		protected virtual void UpdateVisualState() {
			UpdateSelectionState();
			if(!IsSelected)
				UpdateCommonState();
			UpdateLocationState();
		}
		protected override void Subscribe(BaseLayoutItem item) {
			base.Subscribe(item);
			if(item != null) {
				item.VisualChanged += OnItemVisualChanged;
				if(item is LayoutPanel) {
					BindingHelper.SetBinding(this, PinLocationProperty, item, "TabPinLocation");
					BindingHelper.SetBinding(this, PinnedProperty, item, "Pinned");
				}
			}
		}
		protected override void Unsubscribe(BaseLayoutItem item) {
			base.Unsubscribe(item);
			if(item != null) {
				item.VisualChanged -= OnItemVisualChanged;
				if(item is LayoutPanel) {
					BindingHelper.ClearBinding(this, PinLocationProperty);
					BindingHelper.ClearBinding(this, PinnedProperty);
				}
			}
		}
		protected virtual void OnItemVisualChanged(object sender, EventArgs e) {
			UpdateVisualState();
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new UIAutomation.TabbedPaneItemAutomationPeer(this);
		}
		protected internal bool AutomationClick() {
			if(LayoutItem != null && Container != null) {
				Container.DockController.Activate(LayoutItem);
				return LayoutItem.Parent.SelectedItem == LayoutItem;
			}
			return false;
		}
		#endregion UIAutomation
	}
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class DocumentPaneItem : TabbedPaneItem {
		public static readonly DependencyProperty TabColorProperty;
		public static readonly DependencyProperty HasTabColorProperty;
		readonly static DependencyPropertyKey HasTabColorPropertyKey;
		static DocumentPaneItem() {
			var dProp = new DependencyPropertyRegistrator<DocumentPaneItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("TabColor", ref TabColorProperty, Colors.Transparent,
				(dObj, e) => ((DocumentPaneItem)dObj).OnTabColorChanged((Color)e.OldValue, (Color)e.NewValue));
			dProp.RegisterReadonly("HasTabColor", ref HasTabColorPropertyKey, ref HasTabColorProperty, false);
		}
		public DocumentPaneItem() {
		}
		protected override void UpdateVisualState() {
			base.UpdateVisualState();
			UpdateColorState();
		}
		protected override void UpdateSelectionState() {
			VisualStateManager.GoToState(this, "EmptySelectionState", false);
			if(IsSelected) {
				if(LayoutItem.IsActive)
					VisualStateManager.GoToState(this, "Selected", false);
				else
					VisualStateManager.GoToState(this, "Inactive", false);
			}
			else
				VisualStateManager.GoToState(this, "Unselected", false);
		}
		void UpdateColorState() {
			VisualStateManager.GoToState(this, "EmptyColorState", false);
			if(HasTabColor) {
				if(IsSelected) {
					if(LayoutItem.IsActive)
						VisualStateManager.GoToState(this, "ColorSelected", false);
					else
						VisualStateManager.GoToState(this, "ColorInactive", false);
				}
				else {
					VisualStateManager.GoToState(this, IsMouseOver ? "ColorMouseOver" : "ColorUnselected", false);
				}
			}
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnLayoutItemChanged(item, oldItem);
			if(item != null)
				item.Forward(this, TabColorProperty, "ActualTabBackgroundColor");
			else ClearValue(TabColorProperty);
		}
		protected virtual void OnTabColorChanged(Color oldValue, Color newValue) {
			SetValue(HasTabColorPropertyKey, newValue != Colors.Transparent);
			Dispatcher.BeginInvoke(new Action(() =>
			{
				UpdateVisualState();
			}));
		}
		protected override bool IsControlBoxActuallyVisible(BaseLayoutItem item) {
			return base.IsControlBoxActuallyVisible(item) || (bool)item.GetValue(BaseLayoutItem.IsPinButtonVisibleProperty);
		}
		public Color TabColor {
			get { return (Color)GetValue(TabColorProperty); }
			set { SetValue(TabColorProperty, value); }
		}
		public bool HasTabColor {
			get { return (bool)GetValue(HasTabColorProperty); }
		}
	}
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class TabbedLayoutGroupItem : TabbedPaneItem {
		static TabbedLayoutGroupItem() {
			var dProp = new DependencyPropertyRegistrator<TabbedLayoutGroupItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public TabbedLayoutGroupItem() {
		}
	}
}
