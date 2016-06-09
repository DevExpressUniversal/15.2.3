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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	[TemplatePart(Name = PART_Popup, Type = typeof(PopupMenuBase))]
	[TemplatePart(Name = PART_ArrowContent, Type = typeof(ContentControl))]
	[TemplatePart(Name = PART_ArrowControl, Type = typeof(Control))]
	[TemplatePart(Name = PART_ContentToArrowIndent, Type = typeof(Border))]
	[TemplateVisualState(Name = "Normal", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Hot", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Pressed", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Customization", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Disabled", GroupName = "ItemState")]
	public class BarSubItemLinkControl : BarButtonItemLinkControl, IPopupOwner
{
		#region Constants
		const string PART_Popup = "PART_Popup";
		const string PART_ArrowContent = "PART_ArrowContent";
		const string PART_ArrowControl = "PART_ArrowControl";
		const string PART_ContentToArrowIndent = "PART_ContentToArrowIndent";
		#endregion Constants
		#region Dependency Properties
		public static readonly DependencyProperty ContentToArrowIndentProperty;
		public static readonly DependencyProperty ShowContentInArrowProperty;
		public static readonly DependencyProperty ActualArrowAlignmentProperty;
		public static readonly DependencyProperty ItemClickBehaviourProperty;	   
		static BarSubItemLinkControl() {
			ItemClickBehaviourProperty = DependencyPropertyManager.Register("ItemClickBehaviour", typeof(PopupItemClickBehaviour), typeof(BarSubItemLinkControl), new FrameworkPropertyMetadata(PopupItemClickBehaviour.Undefined));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarSubItemLinkControl), typeof(BarSubItemLinkControlAutomationPeer), owner => new BarSubItemLinkControlAutomationPeer((BarSubItemLinkControl)owner));
			ContentToArrowIndentProperty = DependencyPropertyManager.Register("ContentToArrowIndent", typeof(double), typeof(BarSubItemLinkControl), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure, null));
			ShowContentInArrowProperty = DependencyPropertyManager.Register("ShowContentInArrow", typeof(bool), typeof(BarSubItemLinkControl), new UIPropertyMetadata(false));
			ActualArrowAlignmentProperty = DependencyPropertyManager.Register("ActualArrowAlignment", typeof(Dock), typeof(BarSubItemLinkControl), new FrameworkPropertyMetadata(Dock.Right, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarSubItemLinkControl)d).OnActualArrowAlignmentChanged(e)));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarSubItemLinkControl), new FrameworkPropertyMetadata(typeof(BarSubItemLinkControl)));
			ActualIsHoverEnabledPropertyKey.OverrideMetadata(typeof(BarSubItemLinkControl), new PropertyMetadata(false));
		}
		#endregion Dependency Properties
		PopupMenuBase popup;
		public PopupItemClickBehaviour ItemClickBehaviour {
			get { return (PopupItemClickBehaviour)GetValue(ItemClickBehaviourProperty); }
			set { SetValue(ItemClickBehaviourProperty, value); }
		}
		public double ContentToArrowIndent {
			get { return (double)GetValue(ContentToArrowIndentProperty); }
			set { SetValue(ContentToArrowIndentProperty, value); }
		}
		public bool ShowContentInArrow {
			get { return (bool)GetValue(ShowContentInArrowProperty); }
			set { SetValue(ShowContentInArrowProperty, value); }
		}
		public Dock ActualArrowAlignment {
			get { return (Dock)GetValue(ActualArrowAlignmentProperty); }
			set { SetValue(ActualArrowAlignmentProperty, value); }
		}
		public bool IsPopupOpen { get { return Popup != null && Popup.IsOpen; } }
		public BarSubItemLink SubItemLink { get { return base.Link as BarSubItemLink; } }
		public BarSubItem SubItem { get { return Item as BarSubItem; } }
		protected object GetTemplateFromProvider(DependencyProperty prop, BarSubItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarSubItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return (key) => new BarSubItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		public BarSubItemLinkControl() : this(null) {
		}
		public BarSubItemLinkControl(BarSubItemLink link)
			: base(link) {
				this.popupExpandMode = BarPopupExpandMode.Classic;
		}
		public void ShowPopup() {
			if(Popup == null) return;
			if(!IsPressed) IsPressed = true;
			RaiseGetItemData();
			ShowPopupCore();
		}
		protected void ShowPopupCore() {
			if (Popup == null || Popup.IsOpen) return;
			Popup.IsBranchHeader = true;
			Popup.IsRoot = IsOnBar || IsLinkInRibbon;
			Popup.Placement = PlacementMode.Bottom;
			Popup.VerticalOffset = 0d;
			if (IsOnBar) {
				popup.Placement = PlacementMode.Bottom;
				if (Orientation == Orientation.Vertical) {
					Popup.Placement = PlacementMode.Right;
					if (RotateWhenVertical) {
						Popup.VerticalOffset = Math.Max(0, RenderSize.Width - RenderSize.Height);
					}
				}
			} else {
				popup.Placement = BarManagerHelper.GetPopupPlacement(this);
			}		 
			Popup.ShowPopup(GetTemplateChild("PART_PopupPlacementTarget") as UIElement ?? this);
			BarPopupBase.UpdateSubMenuBounds(this, Popup);
			UpdateToolTip();
		}
		public void ClosePopup() {
			if(Popup == null) return;
			Popup.ContentControl.ReleaseFocus();
			Popup.ClosePopup();
			ShowToolTip();
		}
		public void ClosePopup(bool ignoreSetMenuMode) {
			Popup.ClosePopup(ignoreSetMenuMode);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(LayoutPanel != null) {
				Popup = GetTemplateChild("PART_Popup") as PopupMenuBase;
			} else
				Popup = null;
			UpdateLayoutByContainerType(ContainerType);
			Popup.Do(popup => popup.SetCurrentValue(BarPopupExpandable.ExpandModeProperty, popupExpandMode));
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if(ItemsOwner != null) {
				AssignPopupContentControlLinksHolder();
			}
		}
		protected internal PopupMenuBase Popup {
			get { return popup; }
			private set {
				if (value == popup)
					return;
				PopupMenuBase oldValue = popup;
				popup = value;
				OnPopupChanged(oldValue);
			}
		}
		protected virtual void OnPopupChanged(PopupMenuBase oldValue) {
			if (oldValue != null) {
				oldValue.Closed -= OnPopupClosed;
				oldValue.Opened -= OnPopupOpened;
				oldValue.Opening -= OnPopupOpening;
			}			
			if (Popup != null) {
				Popup.OwnerLinkControl = this;
				Popup.Closed += OnPopupClosed;
				Popup.Opened += OnPopupOpened;
				Popup.Opening += OnPopupOpening;
				if (ItemsOwner != null) {
					ItemsOwner.Popup = Popup;
					AssignPopupContentControlLinksHolder();
				}
				UpdatePopupSpacingMode();
			}
		}
		protected virtual void AssignPopupContentControlLinksHolder() {
			ItemsOwner.LinksHolder = SubItem;
		}
		protected SubMenuBarControl ItemsOwner { get { return Popup.With(x => x.ContentControl); } }
		BarPopupExpandMode popupExpandMode;
		public override void SetExpandMode(BarPopupExpandMode expandMode) {
			popupExpandMode = expandMode;
			if(Popup != null)
				Popup.ExpandMode = popupExpandMode;
		}
		protected override bool ShouldDeactivateMenuOnAccessKey { get { return false; } }
		protected override void OnAccessKeyCore(AccessKeyEventArgs e) {
			CloseNeighboringSubItems();
			OpenPopupWithKeyboard();
		}
		protected void CloseNeighboringSubItems() {
			if (Link == null)
				return;
			foreach (BarItemLink link in Link.Links.OfType<BarSubItemLink>()) {
				((BarSubItemLinkControl)link.LinkControl).Do(lc => lc.ClosePopupWithKeyboard());
			}
		}
		protected override void OnToolTipOpening(ToolTipEventArgs e) {
			if(IsPopupOpen) {
				if(Mouse.DirectlyOver is DependencyObject && ((DependencyObject)Mouse.DirectlyOver).VisualParents().FirstOrDefault(d => d == this) != null) {
					return;
				}
				e.Handled = true;
			}
		}
		protected virtual void HideToolTip() {
			ToolTip toolTip = this.GetToolTip() as ToolTip;
			if(toolTip != null) toolTip.Visibility = Visibility.Collapsed;
		}
		protected virtual void ShowToolTip() {
			ToolTip toolTip = this.GetToolTip() as ToolTip;
			if(toolTip != null)
				toolTip.Visibility = Visibility.Visible;
			else UpdateToolTip();
		}
		protected internal virtual bool HasVisibleItems() {
			if(SubItem == null)
				return false;
			bool hasVisibleItem = false;			
			foreach(BarItemLinkBase linkBase in SubItem.ItemLinks) {				
				if(Link.ActualIsVisible) {
					hasVisibleItem = true;
					break;
				}
			}
			return hasVisibleItem;
		}
		protected internal void ForceUpdatePopupContentControlLinkHolder() {
			if(ItemsOwner != null) {
				ItemsOwner.LinksHolder = null;
				AssignPopupContentControlLinksHolder();
			}
		}
		protected override void UpdateLayoutByContainerType(LinkContainerType type) {
			base.UpdateLayoutByContainerType(type);
			if(LayoutPanel == null) return;
			if(type == LinkContainerType.MainMenu) {
				LayoutPanel.ShowArrow = false;
				return;
			}
			LayoutPanel.ShowArrow = true;
		}
		protected override void OnCurrentRibbonStyleChanged(RibbonItemStyles oldValue) {
			base.OnCurrentRibbonStyleChanged(oldValue);
			if(ContainerType == LinkContainerType.RibbonPageGroup) {
				if(CurrentRibbonStyle == RibbonItemStyles.Large) {
					ActualArrowAlignment = Dock.Bottom;
					ShowContentInArrow = true;
				} else {
					ActualArrowAlignment = Dock.Right;
					ShowContentInArrow = false;
				}
			}
		}
		protected override void OnActualGlyphAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			base.OnActualGlyphAlignmentChanged(e);
		}
		protected virtual void OnActualArrowAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLayoutPanelArrowAlignment();
		}
		protected override void UpdateLayoutPanel() {
			base.UpdateLayoutPanel();
			UpdateLayoutPanelArrowAlignment();
		}
		protected virtual void UpdateLayoutPanelArrowAlignment() {
			if(LayoutPanel == null) return;
			if(CurrentRibbonStyle == RibbonItemStyles.Large) {
				LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Top;
			}
			switch(ActualArrowAlignment) {
				case Dock.Bottom:
					LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Top;
					break;
				case Dock.Left:
					LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Right;
					break;
				case Dock.Right:
					LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Left;
					break;
				case Dock.Top:
					LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Bottom;
					break;
			}
		}
		protected internal override bool ProcessKeyDown(KeyEventArgs e) {
			switch(e.Key) {
				case Key.Down:
					if(IsOnBar && Orientation == Orientation.Horizontal)
						return OpenPopupWithKeyboard();
					break;
				case Key.Left:
					if ((!IsOnBar || Orientation == Orientation.Vertical) && FlowDirection == FlowDirection.RightToLeft)
						return OpenPopupWithKeyboard();
					break;
				case Key.Right:
					if ((!IsOnBar || Orientation == Orientation.Vertical) && FlowDirection == FlowDirection.LeftToRight)
						return OpenPopupWithKeyboard();
					break;
				case Key.Enter:
					return OpenPopupWithKeyboard();
			}
			return base.ProcessKeyDown(e);
		}
		protected bool OpenPopupWithKeyboard() {
			if(!IsPopupOpen && Popup!=null) {
				openedWithKeyboard = true;
				SelectFirstLinkOnPopupOpened = true;
				IsPressed = true;
				return true;
			}
			return false;
		}
		protected bool ClosePopupWithKeyboard() {
			if(IsPopupOpen) {
				IsPressed = false;
				return true;
			}
			return false;
		}
		protected override void UpdateActualShowArrow() {
			ActualShowArrow = true;
		}
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateActualItemClickBehaviour();
		}
		protected internal virtual void UpdateActualItemClickBehaviour() {
			if (SubItemLink != null && SubItemLink.ItemClickBehaviour != PopupItemClickBehaviour.Undefined) {
				ItemClickBehaviour = SubItemLink.ItemClickBehaviour;
				return;
			}
			if (SubItem != null && SubItem.ItemClickBehaviour != PopupItemClickBehaviour.Undefined) {
				ItemClickBehaviour = SubItem.ItemClickBehaviour;
				return;
			}
		}
		protected override void UpdateIsPressed() {
			if(IsLinkInCustomizationMode)
				return;
			base.UpdateIsPressed();
		}
		protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsPressedChanged(e);
			if((bool)e.NewValue) {
				ShowPopupCore();
			} else
				ClosePopup();
		}
		protected virtual bool GetCanOpenMenu() {
			if(SubItem != null)
				return SubItem.CanOpenMenu;
			if(SubItemLink != null)
				return SubItemLink.CanOpenMenu;
			return false;
		}
		protected override object OnIsPressedCoerce(object value) {
			bool res = (bool)base.OnIsPressedCoerce(value);
			if(res && res != IsPressed && !GetCanOpenMenu() && !BlendHelper.IsInBlend)
				res = false;
			return res;
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if(Popup != null)
				PopupMenuManager.CancelPopupClosing(Popup);
			if(!IsOnBar && (!IsLinkInRibbon || ContainerType == LinkContainerType.DropDownGallery) && !IsLinkInRadialMenu || IsLinkInApplicationMenu) {
				if(!IsLinkInCustomizationMode) {
					if (IsPopupOpen)
						PopupMenuManager.CancelPopupClosing(Popup);
					else if (popupExpandMode != BarPopupExpandMode.TabletOffice)
						PopupMenuManager.ShowPopup(Popup, false, ShowPopup);
				}
			}			
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			if (!IsPopupOpen)
				PopupMenuManager.CancelPopupOpening(Popup);
		}
		protected override void UnpressOnMouseLeave(MouseEventArgs e) {
			if(!LayoutHelper.IsChildElement(this, e.OriginalSource as DependencyObject))
				base.UnpressOnMouseLeave(e);
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			if(IsPopupOpen) return;
			base.OnLostMouseCapture(e);
		}
		protected internal override void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {			
			if (IsLinkInRadialMenu) {
				e.Handled = true;
				return;
			}
			if(IsLinkInCustomizationMode) {
				if (ShowCustomizationBorder)
					IsPressed = !IsPressed;
				BarNameScope.GetService<ICustomizationService>(this).Select(this);
			} else {
				if(IsOnBar || IsLinkInRibbon) {
					CheckCloseAllPopups(e);
				}
				IsPressed = true;
			}
			e.Handled = true;
		}
		void CheckCloseAllPopups(MouseButtonEventArgs e) {
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if (MouseHelper.Captured == this)
				MouseHelper.ReleaseCapture(this);   
			OnClick();
			e.Handled = true;
		}
		protected override bool ShouldPressItem() {
			if(BlendHelper.IsInBlend) return true;
			if (!HasVisibleItems() && Manager != null && !BarManagerCustomizationHelper.IsInCustomizationMode(this)) return false;
			return base.ShouldPressItem();
		}
		protected override bool CanStartDragCore(object sender, MouseButtonEventArgs e) {
			Point pt = e.GetPosition(this);
			if(pt.X < 0 || pt.Y < 0 || pt.X > ActualWidth || pt.Y > ActualHeight) return false;
			return base.CanStartDragCore(sender, e);
		}
		protected override bool CanShowToolTip() {
			if (Popup != null && Popup.IsMouseOver) {
				return false;
			}
			return base.CanShowToolTip();
		}
		internal bool SelectFirstLinkOnPopupOpened = false;
		bool openedWithKeyboard = false;
		void OnPopupOpening(object source, EventArgs e) {
		}
		void OnPopupOpened(object source, EventArgs e) {
			if(openedWithKeyboard) Popup.ContentControl.CaptureFocus();
			if (SelectFirstLinkOnPopupOpened) {
				SelectFirstLinkOnPopupOpened = false;
				NavigationTree.SelectElement(Popup.ContentControl);
				if (!IsLinkControlInMenu)
					HideToolTip();
			}
			if(SubItemLink != null)
				SubItemLink.IsOpened = true;
			RaisePopup();
		}
		protected virtual void RaisePopup() {
			if(SubItemLink != null)
				SubItemLink.RaisePopup();
			else if(SubItem != null)
				SubItem.RaisePopup();
		}
		protected virtual void RaiseCloseUp() {
			if(SubItemLink != null)
				SubItemLink.RaiseCloseUp();
			else if(SubItem != null)
				SubItem.RaiseCloseUp();
		}
		protected virtual void RaiseGetItemData() {
			if(SubItemLink != null)
				SubItemLink.RaiseGetItemData();
			else if(SubItem != null)
				SubItem.RaiseGetItemData();
		}
		void OnPopupClosed(object source, EventArgs e) {
			Popup.ContentControl.ReleaseFocus();
			openedWithKeyboard = false;
			ShowToolTip();
			if(IsPressed) IsPressed = false; 
			UpdateIsHighlighted();
			Popup.CurrentItem = null;			
			if(SubItemLink != null)
				SubItemLink.IsOpened = false;
			RaiseCloseUp();
		}
		internal void OnIsOpenedChanged() {
			if(IsOnBar || IsLinkInRibbon) {
				IsPressed = SubItemLink.IsOpened;
			} else {
				if(SubItemLink.IsOpened)
					ShowPopup();
				else
					ClosePopup();
			}
		}					
		protected override void OnSpacingModeChanged(SpacingMode oldValue) {
			base.OnSpacingModeChanged(oldValue);
			UpdatePopupSpacingMode();
		}
		protected virtual void UpdatePopupSpacingMode() {
			if (Popup != null && Popup.ContentControl != null)
				Popup.ContentControl.SpacingMode = SpacingMode;
		}
		bool IPopupOwner.ActAsDropdown {
			get { return true; }
		}
		IPopupControl IPopupOwner.Popup {
			get { return Popup; }
		}
		protected internal override INavigationOwner GetBoundOwner() {
			return Popup.With(x=>x.ContentControl);
		}
	}   
}
