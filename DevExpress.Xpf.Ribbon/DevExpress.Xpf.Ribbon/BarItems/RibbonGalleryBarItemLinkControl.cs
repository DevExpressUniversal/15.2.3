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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;
using System;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Ribbon.Automation;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Ribbon.Themes;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonGalleryBarItemLinkControl : BarItemLinkControl, IPopupOwner {
		#region static
		public static readonly DependencyProperty ActualDropDownGalleryEnabledProperty;
		protected static readonly DependencyPropertyKey ActualDropDownGalleryEnabledPropertyKey;
		public static readonly DependencyProperty ActualGalleryProperty;
		protected static readonly DependencyPropertyKey ActualGalleryPropertyKey;
		public static readonly DependencyProperty IsGalleryVisibleProperty;
		public static readonly DependencyProperty NormalTemplateProperty;
		public static readonly DependencyProperty TouchTemplateProperty;
		static RibbonGalleryBarItemLinkControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGalleryBarItemLinkControl), new FrameworkPropertyMetadata(typeof(RibbonGalleryBarItemLinkControl)));						
			ActualDropDownGalleryEnabledPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDropDownGalleryEnabled", typeof(bool), typeof(RibbonGalleryBarItemLinkControl), new FrameworkPropertyMetadata(true));
			ActualDropDownGalleryEnabledProperty = ActualDropDownGalleryEnabledPropertyKey.DependencyProperty;
			ActualGalleryPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGallery", typeof(Gallery), typeof(RibbonGalleryBarItemLinkControl), new FrameworkPropertyMetadata(null));
			ActualGalleryProperty = ActualGalleryPropertyKey.DependencyProperty;
			IsGalleryVisibleProperty = DependencyPropertyManager.Register("IsGalleryVisible", typeof(bool), typeof(RibbonGalleryBarItemLinkControl), new PropertyMetadata(true));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonGalleryBarItemLinkControl), typeof(RibbonGalleryBarItemLinkControlAutomationPeer), owner => new RibbonGalleryBarItemLinkControlAutomationPeer((RibbonGalleryBarItemLinkControl)owner));
			NormalTemplateProperty = DependencyPropertyManager.Register("NormalTemplate", typeof(ControlTemplate), typeof(RibbonGalleryBarItemLinkControl), new FrameworkPropertyMetadata(null, (d, e) => ((RibbonGalleryBarItemLinkControl)d).OnNormalTemplateChanged((ControlTemplate)e.OldValue)));
			TouchTemplateProperty = DependencyPropertyManager.Register("TouchTemplate", typeof(ControlTemplate), typeof(RibbonGalleryBarItemLinkControl), new FrameworkPropertyMetadata(null, (d, e) => ((RibbonGalleryBarItemLinkControl)d).OnTouchTemplateChanged((ControlTemplate)e.OldValue)));
			ThemeManager.TreeWalkerProperty.OverrideMetadata(typeof(RibbonGalleryBarItemLinkControl), new FrameworkPropertyMetadata(null, (d, e) => ((RibbonGalleryBarItemLinkControl)d).OnTreeWalkerChanged(e.OldValue as ThemeTreeWalker, e.NewValue as ThemeTreeWalker)));
		}		
		#endregion
		#region dep props
		public bool IsGalleryVisible {
			get { return (bool)GetValue(IsGalleryVisibleProperty); }
			set { SetValue(IsGalleryVisibleProperty, value); }
		}
		public bool ActualDropDownGalleryEnabled {
			get { return (bool)GetValue(ActualDropDownGalleryEnabledProperty); }
			protected set { this.SetValue(ActualDropDownGalleryEnabledPropertyKey, value); }
		}
		public Gallery ActualGallery {
			get { return (Gallery)GetValue(ActualGalleryProperty); }
			protected set { this.SetValue(ActualGalleryPropertyKey, value); }
		}							
		#endregion
		public RibbonGalleryBarItemLinkControl() : this(null) { }
		public RibbonGalleryBarItemLinkControl(RibbonGalleryBarItemLink link)
			: base(link) {
		}
		public RibbonGalleryBarItemLink GalleryLink { get { return base.Link as RibbonGalleryBarItemLink; } }
		protected internal RibbonGalleryBarItem GalleryItem { get { return Item as RibbonGalleryBarItem; } }
		Button UpButton {
			get { return upButton; }
			set {
				if(upButton != value) {
					var oldValue = upButton;
					upButton = value;
					OnUpButtonChanged(oldValue);
				}
			}
		}
		Button DownButton {
			get { return downButton; }
			set {
				if(downButton != value) {
					var oldValue = downButton;
					downButton = value;
					OnDownButtonChanged(oldValue);
				}
			}
		}
		internal FrameworkElement DropDownGalleryPlacementTarget { get; set; }
		internal RibbonCheckedBorderControl DropDownButton {
			get { return dropDownButton; }
			set {
				if(dropDownButton != value) {
					var oldValue = dropDownButton;
					dropDownButton = value;
					OnDropDownButtonChanged(oldValue);
				}
			}
		}
		RibbonCheckedBorderControl dropDownButton;
		Button upButton, downButton;
		internal GalleryControl GalleryControl {get; set; }
		protected virtual void OnArrowButtonMouseDown(object sender, MouseButtonEventArgs e) {
			if(DropDownButton != null && DropDownButton.IsChecked.GetValueOrDefault())
				return;
			ShowDropDownGallery();
			e.Handled = true;
		}
		public override void OnApplyTemplate() {
			if(LayoutPanel != null)
				LayoutPanel.MouseLeftButtonDown -= new MouseButtonEventHandler(OnArrowButtonMouseDown);
			base.OnApplyTemplate();
			UpButton = GetTemplateChild("PART_Up") as Button;
			DownButton = GetTemplateChild("PART_Down") as Button;
			DropDownButton = GetTemplateChild("PART_DropDown") as RibbonCheckedBorderControl;
			GalleryControl = GetTemplateChild("PART_GalleryControl") as GalleryControl;
			DropDownGalleryPlacementTarget = GetTemplateChild("PART_PopupPlacementTarget") as FrameworkElement;
			UpButton.Do(x=>x.IsEnabled = false);
			if(LayoutPanel != null)
				LayoutPanel.MouseLeftButtonDown += new MouseButtonEventHandler(OnArrowButtonMouseDown);
		}
		protected override void OnClear() {
			base.OnClear();
			if(GalleryControl != null)
				GalleryControl.Gallery = null;
			if(PopupGallery != null && PopupGallery.IsOpen)
				PopupGallery.IsOpen = false;
			PopupGallery = null;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			LayoutUpdated += new System.EventHandler(OnLayoutUpdated);
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			LayoutUpdated -= new System.EventHandler(OnLayoutUpdated);
		}
		protected override void OnCurrentRibbonStyleChanged(RibbonItemStyles oldValue) {
			base.OnCurrentRibbonStyleChanged(oldValue);
			UpdateIsGalleryVisible();
		}
		protected virtual void UpdateIsGalleryVisible() {
			IsGalleryVisible = !(CurrentRibbonStyle == RibbonItemStyles.SmallWithoutText || CurrentRibbonStyle == RibbonItemStyles.SmallWithText);
		}
		protected override void UpdateByContainerType(LinkContainerType type) {
			base.UpdateByContainerType(type);
			if(type == LinkContainerType.RibbonPageGroup)
				UpdateIsGalleryVisible();
			else
				IsGalleryVisible = false;
		}
		Gallery gallery = null;
		Gallery Gallery {
			get {
				if(gallery == null) {
					gallery = GalleryItem == null || GalleryItem.Gallery == null ? null : GalleryItem.Gallery;
				}
				return gallery;
			}
		}
		Gallery dropDownGallery = null;
		Gallery DropDownGallery {
			get {
				if(dropDownGallery == null) {
					dropDownGallery = GalleryItem == null || GalleryItem.DropDownGallery == null ? null : GalleryItem.DropDownGallery;
				}
				return dropDownGallery;
			}
		}
		public ControlTemplate NormalTemplate {
			get { return (ControlTemplate)GetValue(NormalTemplateProperty); }
			set { SetValue(NormalTemplateProperty, value); }
		}
		public ControlTemplate TouchTemplate {
			get { return (ControlTemplate)GetValue(TouchTemplateProperty); }
			set { SetValue(TouchTemplateProperty, value); }
		}
		GalleryDropDownPopupMenu popupGallery;
		protected internal GalleryDropDownPopupMenu PopupGallery {
			get { return popupGallery; }
			set {
				if(popupGallery == value) return;
				var oldValue = popupGallery;
				popupGallery = value;
				OnPopupGalleryChanged(oldValue);
			}
		}
		protected virtual void OnDownButtonChanged(Button oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnDownButtonClick;
			if(DownButton != null)
				DownButton.Click += OnDownButtonClick;
		}
		protected virtual void OnDropDownButtonChanged(RibbonCheckedBorderControl oldValue) {
			if(oldValue != null) {
				oldValue.MouseLeftButtonDown -= OnDropDownButtonMouseLeftButtonDown;
				oldValue.MouseLeftButtonUp += OnDropDownButtonMouseLeftButtonUp;
			}
			if(DropDownButton != null) {
				DropDownButton.MouseLeftButtonDown += OnDropDownButtonMouseLeftButtonDown;
				DropDownButton.MouseLeftButtonUp += OnDropDownButtonMouseLeftButtonUp;
			}
		}
		protected virtual void OnUpButtonChanged(Button oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnUpButtonClick;
			if(UpButton != null)
				UpButton.Click += OnUpButtonClick;
		}
		void OnDropDownButtonMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if(DropDownButton.IsChecked != true) {
				ShowDropDownGallery();
				if(Mouse.Captured == null)
					Mouse.Capture(DropDownButton);
			}
		}
		void OnDropDownButtonMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if(DropDownButton != null && Mouse.Captured == DropDownButton)
				DropDownButton.ReleaseMouseCapture();
		}
		protected virtual void OnPopupGalleryChanged(GalleryDropDownPopupMenu oldValue) {
			if(oldValue != null) {
				UnsubscribeGalleryDropDownPopupMenuEvents(oldValue);
				oldValue.Gallery = null;
			}
			if(PopupGallery != null) {
				SubscribeGalleryDropDownPopupMenuEvents(PopupGallery);
			}
		}
		protected GalleryDropDownPopupMenu CreatePopupGallery() {
			GalleryDropDownPopupMenu popupGallery = new GalleryDropDownPopupMenu(GalleryItem.DropDownMenuItemLinks, this);
			AddLogicalChild(popupGallery);
			popupGallery.ExpandMode = GetActualExpandMode();
			popupGallery.Gallery = CreateActualDropDownGallery();
			popupGallery.MinWidth = ActualWidth;
			popupGallery.MinHeight = ActualHeight;
			return popupGallery;
		}
		protected override void OnActualShowContentChanged(DependencyPropertyChangedEventArgs e) {
			base.OnActualShowContentChanged(e);
			if(LayoutPanel == null)
				return;
			LayoutPanel.ShowContent = ActualShowContent;
		}
		private void SubscribeGalleryDropDownPopupMenuEvents(GalleryDropDownPopupMenu popup) {
			popup.Closed += new System.EventHandler(OnGalleryDropDownMenuClosed);
		}
		private void UnsubscribeGalleryDropDownPopupMenuEvents(GalleryDropDownPopupMenu popup) {
			popup.Closed -= new System.EventHandler(OnGalleryDropDownMenuClosed);
		}
		void OnGalleryDropDownMenuClosed(object sender, System.EventArgs e) {
			DropDownButton.IsChecked = false;
			IsPressed = false;
			GalleryItem.OnDropDownGalleryClosed(PopupGallery);
			if(PopupGallery.Gallery != null && PopupGallery.Gallery.ClonedFrom == Gallery && PopupGallery.Gallery.ItemCheckMode == GalleryItemCheckMode.Single) {
				GalleryItem item = Gallery.GetFirstCheckedItem();
				ScrollToItem(item);
			}
			RemoveLogicalChild(PopupGallery);
		}
		void InitDropDownGalleryBeforeOpening(Gallery dropDownGallery) {
			dropDownGallery.IsGroupCaptionVisible = dropDownGallery.IsGroupCaptionVisible == DefaultBoolean.Default ? DefaultBoolean.True : Gallery.IsGroupCaptionVisible;
			dropDownGallery.IsItemCaptionVisible = true;
			dropDownGallery.IsItemDescriptionVisible = true;
			dropDownGallery.IsItemDescriptionVisible = true;
			dropDownGallery.SyncWithClone = true;
		}
		Gallery CreateActualDropDownGallery() {
			if(DropDownGallery != null) return DropDownGallery;
			if(Gallery != null) {
				Gallery clone = Gallery.CloneWithoutEvents();
				InitDropDownGalleryBeforeOpening(clone);
				return clone;
			}
			return null;
		}
		internal DependencyObject GetTemplateChildCore(string childName) {
			return GetTemplateChild(childName);
		}
		protected internal void ShowDropDownGallery() {
			PopupGallery = CreatePopupGallery();
			GalleryItem.OnDropDownGalleryInit(PopupGallery);
			IsPressed = true;
			DropDownButton.IsChecked = true;
			PopupGallery.ShowPopup(GetActualPlacementTarget());
		}
		FrameworkElement GetActualPlacementTarget() {
			if(!RibbonItemInfo.IsLargeButton)
				return this;
			return DropDownGalleryPlacementTarget;
		}
		void OnDownButtonClick(object sender, RoutedEventArgs e) {
			int firstVisibleRowIndex = GalleryControl.GetFirstVisibleRowIndex();
			if(firstVisibleRowIndex == -1) return;
			GalleryControl.ScrollToRowByIndex(firstVisibleRowIndex + 1);
			UpdateUpAndDownButtonsState();		   
		}
		protected virtual BarPopupExpandMode GetActualExpandMode() {
			var ribbon = LayoutHelper.FindParentObject<RibbonControl>(this);
			if(ribbon != null && (ribbon.RibbonStyle == RibbonStyle.TabletOffice || ribbon.RibbonStyle == RibbonStyle.OfficeSlim)) {
				return BarPopupExpandMode.TabletOffice;
			}
			return BarPopupExpandMode.Classic;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateUpAndDownButtonsState();
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		void OnUpButtonClick(object sender, RoutedEventArgs e) {
			int firstVisibleRow = GalleryControl.GetFirstVisibleRowIndex();
			if(firstVisibleRow != 0) {				
				GalleryControl.ScrollToRowByIndex(firstVisibleRow - 1);
			}
			else {
				GalleryControl.ScrollToVerticalOffset(0);
			}
			UpdateUpAndDownButtonsState();
		}
		void UpdateUpAndDownButtonsState() {
			if(GalleryControl == null || UpButton == null || DownButton == null) return;
			UpButton.IsEnabled = GalleryControl.ContentVerticalOffset != 0;
			DownButton.IsEnabled = GalleryControl.ScrollableSize.Height - GalleryControl.ViewportSize.Height > GalleryControl.ContentVerticalOffset;
		}
		public override void InitializeRibbonStyle() {
			base.InitializeRibbonStyle();
			if(GalleryControl != null && Gallery != null)
				GalleryControl.DesiredColCount =  Gallery.ColCount;
		}
		protected override RibbonItemStyles CalcRibbonStyleInPageGroup() {
			var ribbon = LayoutHelper.FindParentObject<RibbonControl>(this);
			if(ribbon == null || ribbon.RibbonStyle != RibbonStyle.TabletOffice) {
				return RibbonItemStyles.Large;
			} else
				return RibbonItemStyles.SmallWithText;
		}
		public bool CanReduce() {
			if(GalleryControl == null || GalleryControl.Gallery == null) 
				return false;
			if(GalleryControl.DesiredColCount == 0) return GalleryControl.DesiredColCount > GalleryControl.Gallery.MinColCount;
			if(GalleryControl.Gallery.MinColCount == 0) return GalleryControl.DesiredColCount > 1;
			return GalleryControl.DesiredColCount > GalleryControl.Gallery.MinColCount;
		}
		public bool Reduce() {
			if(!CanReduce()) return false;
			GalleryControl.DesiredColCount = GalleryControl.DesiredColCount - 1;
			return true;
		}
		public void ScrollToItem(GalleryItem item) {
			if(GalleryControl == null || item == null || !IsGalleryVisible)
				return;
			GalleryControl.ScrollToItem(item);			
		}		
		protected internal virtual void OnSourceGalleryChanged() {
			UpdateActualGallery();
		}
		protected virtual void UpdateActualGallery() {
			ActualGallery = GetGallery();
		}
		protected virtual Gallery GetGallery() {
			return Gallery;
		}
		protected virtual bool GetDropDownGalleryEnabled() {
			if(GalleryItem != null)
				return GalleryItem.DropDownGalleryEnabled;
			return false;
		}
		protected virtual void UpdateActualDropDownGalleryEnabled() {
			ActualDropDownGalleryEnabled = GetDropDownGalleryEnabled();
		}
		protected internal virtual void OnSourceDropDownGalleryEnabledChanged() {
			UpdateActualDropDownGalleryEnabled();
		}
		protected override void UpdateActualProperties() {
			UpdateActualGallery();			
			base.UpdateActualProperties();
			UpdateActualDropDownGalleryEnabled();
		}
		protected override void UpdateLayoutPanel() {
			base.UpdateLayoutPanel();
			if(LayoutPanel == null)
				return;
			LayoutPanel.ShowFirstBorder = true;
			LayoutPanel.IsFirstBorderActive = true;
			LayoutPanel.ShowArrow = true;
			LayoutPanel.SecondBorderPlacement = SecondBorderPlacement.Arrow;
			LayoutPanel.CalculateIsMouseOver = true;
			UpdateLayoutPanelArrowAlignment();
		}
		protected virtual void UpdateLayoutPanelArrowAlignment() {
			if(LayoutPanel == null)
				return;
			LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Left;
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return (key) => new BarSubItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}		
		protected virtual void OnNormalTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		protected virtual void OnTouchTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		void OnTreeWalkerChanged(ThemeTreeWalker oldWalker, ThemeTreeWalker newWalker) {
			if(!(oldWalker.If(x => x.IsTouch).ReturnSuccess() && newWalker.If(x => x.IsTouch).ReturnSuccess())) {
				UpdateTemplate();
			}
		}
		void UpdateTemplate() {
			if(ThemeManager.GetTreeWalker(this).If(x => x.IsTouch).ReturnSuccess())
				Template = TouchTemplate;
			else
				Template = NormalTemplate;
		}
		bool IPopupOwner.IsPopupOpen { get { return PopupGallery.Return(x => x.IsOpen, () => false); } }
		bool IPopupOwner.IsOnBar { get { return IsOnBar; } }
		bool IPopupOwner.ActAsDropdown { get { return false; } }
		IPopupControl IPopupOwner.Popup { get { return PopupGallery; } }
		void IPopupOwner.ShowPopup() { ShowDropDownGallery(); }
		void IPopupOwner.ClosePopup() { PopupGallery.Do(x => x.IsOpen = false); }
		void IPopupOwner.ClosePopup(bool ignoreSetMenuMode) { ((IPopupOwner)this).ClosePopup(); }
	}
}
