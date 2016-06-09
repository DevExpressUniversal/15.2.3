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
using DevExpress.Xpf.Bars;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Ribbon.Themes;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Bars.Helpers;
using System.Windows.Documents;
using KeyTipAdornerLayer = System.Windows.Documents.AdornerLayer;
namespace DevExpress.Xpf.Ribbon {
	public enum RibbonQuckAccessToolbarPlacement { Title, Header, Footer, Popup, AeroHeader }
	public abstract class RibbonLinksControl : LinksControl {
		public RibbonLinksControl() {
			IsEnabledChanged += OnIsEnabledChanged;
		}		
		protected override bool GetCanEnterMenuMode() { return false; }
		protected override IList<INavigationElement> GetNavigationElements() { return new List<INavigationElement>(); }
		protected override int GetNavigationID() { return GetHashCode(); }
		protected override NavigationKeys GetNavigationKeys() { return NavigationKeys.Arrows | NavigationKeys.Tab | NavigationKeys.CtrlTab; }
		protected override KeyboardNavigationMode GetNavigationMode() { return KeyboardNavigationMode.Cycle; }
		protected override Orientation GetNavigationOrientation() { return Orientation.Vertical; }
		protected override IBarsNavigationSupport GetNavigationParent() { return null; }
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(ItemLinks == null)
				return;
			foreach(BarItemLinkBase linkBase in ItemLinks) {
				linkBase.CoerceValue(BarItemLinkBase.IsEnabledProperty);
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			var ribbon = LayoutHelper.FindParentObject<RibbonControl>(this);
			var linkInfo = element as BarItemLinkInfo;
			if(ribbon != null && linkInfo != null) {
				linkInfo.BarPopupExpandMode = IsRibbonTablet(ribbon.RibbonStyle) ? BarPopupExpandMode.TabletOffice : BarPopupExpandMode.Classic;
			}
		}
		protected virtual bool IsRibbonTablet(RibbonStyle ribbonStyle) {
			return ribbonStyle == RibbonStyle.TabletOffice || ribbonStyle == RibbonStyle.OfficeSlim;
		}
	}
	public class RibbonQuickAccessToolbarControl : RibbonLinksControl {
		#region static
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty PlacementProperty;
		public static readonly DependencyProperty IsPopupOpenedProperty;
		public static readonly DependencyProperty IsDropDownButtonVisibleProperty;
		protected static readonly DependencyPropertyKey IsDropDownButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsCustomizationButtonVisibleProperty;
		protected static readonly DependencyPropertyKey IsCustomizationButtonVisiblePropertyKey;
		public static readonly DependencyProperty ShowCustomizationButtonProperty;	  
		public static readonly DependencyProperty LeftContentTemplateInFooterForOffice2007RibbonStyleProperty;		
		public static readonly DependencyProperty RightContentTemplateInFooterForOffice2007RibbonStyleProperty;
		public static readonly DependencyProperty LeftContentTemplateInHeaderForOffice2007RibbonStyleProperty;		
		public static readonly DependencyProperty RightContentTemplateInHeaderForOffice2007RibbonStyleProperty;		
		public static readonly DependencyProperty LeftContentTemplateInFooterForOffice2010RibbonStyleProperty;
		public static readonly DependencyProperty RightContentTemplateInFooterForOffice2010RibbonStyleProperty;
		public static readonly DependencyProperty LeftContentTemplateInHeaderForOffice2010RibbonStyleProperty;		
		public static readonly DependencyProperty RightContentTemplateInHeaderForOffice2010RibbonStyleProperty;		
		public static readonly DependencyProperty LeftContentTemplateInPopupProperty;
		public static readonly DependencyProperty RightContentTemplateInPopupProperty;
		public static readonly DependencyProperty LeftContentTemplateInTitleProperty;
		public static readonly DependencyProperty RightContentTemplateInTitleProperty;
		public static readonly DependencyProperty DropDownButtonTemplateInFooterProperty;
		public static readonly DependencyProperty DropDownButtonTemplateInHeaderProperty;
		public static readonly DependencyProperty CustomizationButtonTemplateInFooterProperty;
		public static readonly DependencyProperty CustomizationButtonTemplateInHeaderProperty;
		public static readonly DependencyProperty LeftContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty;
		public static readonly DependencyProperty RightContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty;
		public static readonly DependencyProperty LeftContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty;
		public static readonly DependencyProperty RightContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty;
		static RibbonQuickAccessToolbarControl() {			
			RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(typeof(RibbonQuickAccessToolbarControl), new PropertyMetadata(OnRibbonStylePropertyChanged));
			PlacementProperty = DependencyPropertyManager.Register("Placement", typeof(RibbonQuckAccessToolbarPlacement), typeof(RibbonQuickAccessToolbarControl),
					new PropertyMetadata(RibbonQuckAccessToolbarPlacement.Header, OnPlacementPropertyChanged));
			IsPopupOpenedProperty = DependencyPropertyManager.Register("IsPopupOpened", typeof(bool), typeof(RibbonQuickAccessToolbarControl),
				new FrameworkPropertyMetadata(false, OnIsPopupOpenedChanged));
			IsDropDownButtonVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsDropDownButtonVisible", typeof(bool), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(false));
			IsDropDownButtonVisibleProperty = IsDropDownButtonVisiblePropertyKey.DependencyProperty;
			IsCustomizationButtonVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCustomizationButtonVisible", typeof(bool), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(false));
			IsCustomizationButtonVisibleProperty = IsCustomizationButtonVisiblePropertyKey.DependencyProperty;
			ShowCustomizationButtonProperty = DependencyPropertyManager.Register("ShowCustomizationButton", typeof(bool), typeof(RibbonQuickAccessToolbarControl),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnShowCustomizationButtonPropertyChanged)));
			LeftContentTemplateInFooterForOffice2007RibbonStyleProperty = DependencyPropertyManager.Register("LeftContentTemplateInFooterForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			RightContentTemplateInFooterForOffice2007RibbonStyleProperty = DependencyPropertyManager.Register("RightContentTemplateInFooterForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			LeftContentTemplateInHeaderForOffice2007RibbonStyleProperty = DependencyPropertyManager.Register("LeftContentTemplateInHeaderForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));			
			RightContentTemplateInHeaderForOffice2007RibbonStyleProperty = DependencyPropertyManager.Register("RightContentTemplateInHeaderForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));			
			LeftContentTemplateInFooterForOffice2010RibbonStyleProperty = DependencyPropertyManager.Register("LeftContentTemplateInFooterForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			RightContentTemplateInFooterForOffice2010RibbonStyleProperty = DependencyPropertyManager.Register("RightContentTemplateInFooterForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			LeftContentTemplateInHeaderForOffice2010RibbonStyleProperty = DependencyPropertyManager.Register("LeftContentTemplateInHeaderForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));			
			RightContentTemplateInHeaderForOffice2010RibbonStyleProperty = DependencyPropertyManager.Register("RightContentTemplateInHeaderForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));			
			LeftContentTemplateInPopupProperty = DependencyPropertyManager.Register("LeftContentTemplateInPopup", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			RightContentTemplateInPopupProperty = DependencyPropertyManager.Register("RightContentTemplateInPopup", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			LeftContentTemplateInTitleProperty = DependencyPropertyManager.Register("LeftContentTemplateInTitle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			RightContentTemplateInTitleProperty = DependencyPropertyManager.Register("RightContentTemplateInTitle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			DropDownButtonTemplateInFooterProperty = DependencyPropertyManager.Register("DropDownButtonTemplateInFooter", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			DropDownButtonTemplateInHeaderProperty = DependencyPropertyManager.Register("DropDownButtonTemplateInHeader", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			CustomizationButtonTemplateInFooterProperty = DependencyPropertyManager.Register("CustomizationButtonTemplateInFooter", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			CustomizationButtonTemplateInHeaderProperty = DependencyPropertyManager.Register("CustomizationButtonTemplateInHeader", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonQuickAccessToolbarControl), typeof(QuickAccessToolbarAutomationPeer), owner => new QuickAccessToolbarAutomationPeer((RibbonQuickAccessToolbarControl)owner));
			LeftContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty = DependencyPropertyManager.Register("LeftContentTemplateInAeroHeaderForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			RightContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty = DependencyPropertyManager.Register("RightContentTemplateInAeroHeaderForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			LeftContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty = DependencyPropertyManager.Register("LeftContentTemplateInAeroHeaderForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
			RightContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty = DependencyPropertyManager.Register("RightContentTemplateInAeroHeaderForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonQuickAccessToolbarControl), new FrameworkPropertyMetadata(null));
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonQuickAccessToolbarControl)d).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		protected static void OnPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonQuickAccessToolbarControl)d).OnPlacementChanged((RibbonQuckAccessToolbarPlacement)e.OldValue);
		}
		protected static void OnIsPopupOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonQuickAccessToolbarControl)d).OnIsPopupOpenedChanged((bool)e.OldValue);
		}
		protected static void OnShowCustomizationButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonQuickAccessToolbarControl)d).OnShowCustomizationButtonChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public RibbonQuckAccessToolbarPlacement Placement {
			get { return (RibbonQuckAccessToolbarPlacement)GetValue(PlacementProperty); }
			set { SetValue(PlacementProperty, value); }
		}
		public bool IsPopupOpened {
			get { return (bool)GetValue(IsPopupOpenedProperty); }
			set { SetValue(IsPopupOpenedProperty, value); }
		}
		public bool IsDropDownButtonVisible {
			get { return (bool)GetValue(IsDropDownButtonVisibleProperty); }
			protected set { this.SetValue(IsDropDownButtonVisiblePropertyKey, value); }
		}
		public bool IsCustomizationButtonVisible {
			get { return (bool)GetValue(IsCustomizationButtonVisibleProperty); }
			protected set { this.SetValue(IsCustomizationButtonVisiblePropertyKey, value); }
		}
		public bool ShowCustomizationButton {
			get { return (bool)GetValue(ShowCustomizationButtonProperty); }
			set { SetValue(ShowCustomizationButtonProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInFooterForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInFooterForOffice2007RibbonStyleProperty); }
			set { SetValue(LeftContentTemplateInFooterForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInFooterForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInFooterForOffice2007RibbonStyleProperty); }
			set { SetValue(RightContentTemplateInFooterForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInHeaderForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInHeaderForOffice2007RibbonStyleProperty); }
			set { SetValue(LeftContentTemplateInHeaderForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInHeaderForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInHeaderForOffice2007RibbonStyleProperty); }
			set { SetValue(RightContentTemplateInHeaderForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInAeroHeaderForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty); }
			set { SetValue(LeftContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInAeroHeaderForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty); }
			set { SetValue(RightContentTemplateInAeroHeaderForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInAeroHeaderForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty); }
			set { SetValue(LeftContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInAeroHeaderForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty); }
			set { SetValue(RightContentTemplateInAeroHeaderForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInFooterForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInFooterForOffice2010RibbonStyleProperty); }
			set { SetValue(LeftContentTemplateInFooterForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInFooterForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInFooterForOffice2010RibbonStyleProperty); }
			set { SetValue(RightContentTemplateInFooterForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInHeaderForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInHeaderForOffice2010RibbonStyleProperty); }
			set { SetValue(LeftContentTemplateInHeaderForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInHeaderForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInHeaderForOffice2010RibbonStyleProperty); }
			set { SetValue(RightContentTemplateInHeaderForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInPopup {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInPopupProperty); }
			set { SetValue(LeftContentTemplateInPopupProperty, value); }
		}
		public ControlTemplate RightContentTemplateInPopup {
			get { return (ControlTemplate)GetValue(RightContentTemplateInPopupProperty); }
			set { SetValue(RightContentTemplateInPopupProperty, value); }
		}
		public ControlTemplate LeftContentTemplateInTitle {
			get { return (ControlTemplate)GetValue(LeftContentTemplateInTitleProperty); }
			set { SetValue(LeftContentTemplateInTitleProperty, value); }
		}
		public ControlTemplate RightContentTemplateInTitle {
			get { return (ControlTemplate)GetValue(RightContentTemplateInTitleProperty); }
			set { SetValue(RightContentTemplateInTitleProperty, value); }
		}
		public ControlTemplate DropDownButtonTemplateInFooter {
			get { return (ControlTemplate)GetValue(DropDownButtonTemplateInFooterProperty); }
			set { SetValue(DropDownButtonTemplateInFooterProperty, value); }
		}
		public ControlTemplate DropDownButtonTemplateInHeader {
			get { return (ControlTemplate)GetValue(DropDownButtonTemplateInHeaderProperty); }
			set { SetValue(DropDownButtonTemplateInHeaderProperty, value); }
		}
		public ControlTemplate CustomizationButtonTemplateInFooter {
			get { return (ControlTemplate)GetValue(CustomizationButtonTemplateInFooterProperty); }
			set { SetValue(CustomizationButtonTemplateInFooterProperty, value); }
		}
		public ControlTemplate CustomizationButtonTemplateInHeader {
			get { return (ControlTemplate)GetValue(CustomizationButtonTemplateInHeaderProperty); }
			set { SetValue(CustomizationButtonTemplateInHeaderProperty, value); }
		}
		#endregion
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		public RibbonQuickAccessToolbarControl(RibbonQuickAccessToolbar toolbar, int firstVisibleItemIndex) {
			DefaultStyleKey = typeof(RibbonQuickAccessToolbarControl);
			FirstVisibleItemIndex = firstVisibleItemIndex;
			ContainerType = LinkContainerType.RibbonQuickAccessToolbar;
			Toolbar = toolbar;
			Popup = new RibbonQuickAccessToolbarPopup(toolbar);
			Popup.Style = null;
		}
		protected override bool OpenPopupsAsMenu { get { return false; } }
		public int FirstVisibleItemIndex { get; internal set; }
		public int VisibleItemsCount { get; internal set; }
		public RibbonQuickAccessToolbarControl() : this(new RibbonQuickAccessToolbar(), 0) { }
		public RibbonQuickAccessToolbarControl(RibbonQuickAccessToolbar toolbar) : this(toolbar, 0) { }
		protected virtual void OnPlacementChanged(RibbonQuckAccessToolbarPlacement oldValue) {
			UpdateButtonsTemplate();
			UpdateContainerTypeByPlacement();
			UpdateLinksContainerType(NotifyCollectionChangedAction.Reset, null, null);
			UpdateBorderTemplates();			
		}
		protected virtual void UpdateContainerTypeByPlacement() {
			if(Placement == RibbonQuckAccessToolbarPlacement.Footer)
				ContainerType = LinkContainerType.RibbonQuickAccessToolbarFooter;
			else
				ContainerType = LinkContainerType.RibbonQuickAccessToolbar;
		}
		protected virtual void ShowPopup() {
			Popup.Placement = PlacementMode.Bottom;
			Popup.PopupContent = PopupControl;
			PopupControl.ShowCustomizationButton = ShowCustomizationButton;
			if(PopupControl.ItemsPresenter != null)
				PopupControl.ItemsPresenter.InvalidateMeasure();
			Popup.ShowPopup(DropDownButton);
		}
		protected virtual void OnIsPopupOpenedChanged(bool oldValue) {			
			DropDownButton.IsChecked = IsPopupOpened;
			if(IsPopupOpened == true) {
				ShowPopup();
				return;
			}
			else
				Popup.IsOpen = false;			
		}
		RibbonQuickAccessToolbar toolbar;
		public RibbonQuickAccessToolbar Toolbar {
			get { return toolbar; }
			protected set {
				if(Toolbar == value) return;
				toolbar = value;
				OnToolbarChanged();
			}
		}
		protected virtual void OnToolbarChanged() {
			if(Toolbar == null) return;
			ContainerType = LinkContainerType.RibbonQuickAccessToolbar;			
			RecreateItemsSource(((ILinksHolder)Toolbar).ActualLinks);
		}
		protected override NavigationManager CreateNavigationManager() {
			return null;
		}
		public override BarItemLinkCollection ItemLinks {
			get { return Toolbar.ItemLinks; }
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if(Toolbar != null)
				RecreateItemsSource(((ILinksHolder)Toolbar).ActualLinks);
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			foreach(BarItemLinkInfo info in Items) {
				info.LinkBase.LinkInfos.Remove(info);
			}
			UnsubscribeTemplateEvents();
		}
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) {
			UpdateBorderTemplates();
		}
		protected internal virtual void RecreateItemsSource(BarItemLinkCollection itemLinks) {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			ItemsSource = new BarItemLinkInfoCollection(itemLinks);
			if(oldValue != null)
				oldValue.Source = null;
		}
		protected internal virtual void OnShowCustomizationButtonChanged(bool oldValue) {
			UpdateButtonsVisibility();
		}
		protected internal RibbonCheckedBorderControl CustomizationButton { get; private set; }
		protected internal RibbonCheckedBorderControl DropDownButton { get; private set; }
		protected ContentControl OriginItemContainer { get; private set; }
		BarButtonItem moveToolbarItemCore { get; set; }		
		protected BarButtonItem MoveToolbarItem {
			get {
				if(moveToolbarItemCore == null) {
					moveToolbarItemCore = new BarButtonItem();
					moveToolbarItemCore.ItemClick += new ItemClickEventHandler(OnMoveToolbarItemClick);
				}
				return moveToolbarItemCore;
			}
		}
		protected virtual void OnMoveToolbarItemClick(object sender, ItemClickEventArgs e) {
			if(Toolbar.Ribbon.GetToolbarMode() == RibbonQuickAccessToolbarShowMode.ShowAbove)
				Toolbar.Ribbon.SetCurrentValue(RibbonControl.ToolbarShowModeProperty, RibbonQuickAccessToolbarShowMode.ShowBelow);
			else if(Toolbar.Ribbon.GetToolbarMode() == RibbonQuickAccessToolbarShowMode.ShowBelow)
				Toolbar.Ribbon.SetCurrentValue(RibbonControl.ToolbarShowModeProperty, RibbonQuickAccessToolbarShowMode.ShowAbove);
			IsPopupOpened = false;
			CustomizationButton.IsChecked = false;
		}
		QuickAccessToolbarCustomizationMenu customizationMenu;
		protected internal QuickAccessToolbarCustomizationMenu CustomizationMenu {
			get {
				if(customizationMenu == null)
					customizationMenu = CreateCustomizationMenu();
				return customizationMenu;
			}
		}
		protected virtual void InitializeCustomizationMenu() {			
			CustomizationMenu.Placement = PlacementMode.Bottom;
			CustomizationMenu.PlacementTarget = CustomizationButton;
			if (Toolbar.Ribbon.GetToolbarMode() == RibbonQuickAccessToolbarShowMode.ShowAbove) {
				MoveToolbarItem.Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_ShowQuickAccessToolbarBelowTheRibbon);
				MoveToolbarItem.IsEnabled = Toolbar.Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Visible;
			} else {
				MoveToolbarItem.IsEnabled = Toolbar.Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Visible;
				MoveToolbarItem.Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_ShowQuickAccessToolbarAboveTheRibbon);
			}			
			CustomizationMenu.Items.Add(MoveToolbarItem);
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if(Placement == RibbonQuckAccessToolbarPlacement.Popup && Toolbar.Control.VisibleItemsCount == Items.Count) {
				Toolbar.Control.Popup.IsOpen = false;
			}
		}
		protected virtual void ClearCustomizationMenu() {
			CustomizationMenu.Items.Clear();
			CustomizationMenu.Closed -= OnCustomizationMenuClosed;
		}
		protected virtual QuickAccessToolbarCustomizationMenu CreateCustomizationMenu() {
			return new QuickAccessToolbarCustomizationMenu();
		}
		protected ContentControl RightContentControl { get; private set; }
		protected ContentControl LeftContentControl { get; private set; }
		protected internal RibbonQuickAccessToolbarPopup Popup { get; private set; }
		RibbonQuickAccessToolbarControl popupControlCore;
		protected bool IsPopupControlCreated { get { return popupControlCore != null; } }
		protected RibbonQuickAccessToolbarControl PopupControl {
			get {
				if(popupControlCore == null) popupControlCore = CreatePopupControl();
				popupControlCore.FirstVisibleItemIndex = VisibleItemsCount;
				return popupControlCore;
			}
		}
		protected override void OnSpacingModeChanged(SpacingMode oldValue) {
			base.OnSpacingModeChanged(oldValue);
			if (IsPopupControlCreated)
				PopupControl.SpacingMode = SpacingMode;
		}
		protected internal Size BestSize { get; internal set; }
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			CustomizationButton = (RibbonCheckedBorderControl)GetTemplateChild("PART_ToolbarCustomizationButton");
			DropDownButton = (RibbonCheckedBorderControl)GetTemplateChild("PART_ToolbarDropDownButton");
			RightContentControl = (ContentControl)GetTemplateChild("PART_RightContentControl");
			LeftContentControl = (ContentControl)GetTemplateChild("PART_LeftContentControl");
			OriginItemContainer = (ContentControl)GetTemplateChild("PART_OriginItemContainer");
			UpdateButtonsTemplate();
			UpdateBorderTemplates();			
			UpdateButtonsVisibility();
			SubscribeTemplateEvents();
		}
		protected virtual void UpdateButtonsTemplate() {
			if(DropDownButton != null) {
				DropDownButton.Template = Placement == RibbonQuckAccessToolbarPlacement.Footer ? DropDownButtonTemplateInFooter : DropDownButtonTemplateInHeader;				
			}
			if(CustomizationButton != null) {
				CustomizationButton.Template = Placement == RibbonQuckAccessToolbarPlacement.Footer ? CustomizationButtonTemplateInFooter : CustomizationButtonTemplateInHeader;
			}
		}
		protected virtual void SubscribeTemplateEvents() {
			if(DropDownButton != null)
				DropDownButton.MouseLeftButtonDown += new MouseButtonEventHandler(OnDropDownButtonMouseLeftButtonDown);
			if(CustomizationButton != null)
				CustomizationButton.Click += new EventHandler(OnCustomizationButtonClick);
			if(Popup != null) 
				Popup.Closed += new EventHandler(OnPopupClosed);
			if(OriginItemContainer != null) {
				OriginItemContainer.Content = CreateOriginItemControl();
			}
		}
		protected virtual void UnsubscribeTemplateEvents() {
			if(DropDownButton != null) {
				DropDownButton.MouseLeftButtonDown -= OnDropDownButtonMouseLeftButtonDown;
			}
			if(CustomizationButton != null)
				CustomizationButton.Click -= OnCustomizationButtonClick;
			if(Popup != null) Popup.Closed -= OnPopupClosed;
			if(OriginItemContainer != null) {
				OriginItemContainer.Content = null;
			}
		}
		protected virtual BarItemLinkControl CreateOriginItemControl() {
			BarStaticItemLinkControl ctrl = new BarStaticItemLinkControl() { Content = "Wg", CurrentRibbonStyle = RibbonItemStyles.SmallWithText };
			if(Placement == RibbonQuckAccessToolbarPlacement.Footer)
				ctrl.ContainerType = LinkContainerType.RibbonQuickAccessToolbarFooter;
			else
				ctrl.ContainerType = LinkContainerType.RibbonQuickAccessToolbar;			
			return ctrl;
		}
		void OnPopupClosed(object sender, EventArgs e) {
			Popup.PopupContent = null;
			popupControlCore = null;
			DropDownButton.IsChecked = false;
			IsPopupOpened = false;
		}
		protected internal virtual void ShowCustomizationMenu() {
			InitializeCustomizationMenu();
			ToolbarCustomizationMenuShowingEventArgs e = new ToolbarCustomizationMenuShowingEventArgs(CustomizationMenu);
			Toolbar.Ribbon.RaiseToolbarCustomizationMenuShowing(e);
			if(!e.Cancel && CustomizationMenu.ItemLinks.Count != 0) {
				CustomizationMenu.Closed -= OnCustomizationMenuClosed;
				CustomizationMenu.Closed += new EventHandler(OnCustomizationMenuClosed);
				CustomizationMenu.ShowPopup(CustomizationMenu.PlacementTarget);
				CustomizationButton.IsChecked = true;
			}
		}
		void OnCustomizationMenuClosed(object sender, EventArgs e) {
			ClearCustomizationMenu();
			CustomizationButton.IsChecked = false;
			Toolbar.Ribbon.RaiseToolbarCustomizationMenuClosed(new ToolbarCustomizationMenuClosedEventArgs(CustomizationMenu));
		}
		protected void OnDropDownButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {			
			IsPopupOpened = DropDownButton.IsChecked == true ? false : true;
		}
		protected internal virtual void CloseCustomizationMenu() {
			if(CustomizationMenu != null)
				CustomizationMenu.ClosePopup();
			if(CustomizationButton != null)
				CustomizationButton.IsChecked = false;
		}
		protected void OnCustomizationButtonClick(object sender, EventArgs e) {
			if(CustomizationButton.IsChecked.GetValueOrDefault()) {
				CloseCustomizationMenu();
			} else {
				ShowCustomizationMenu();
			}
		}
		protected internal virtual void UpdateButtonsVisibility() {			
			bool isFullyVisible = VisibleItemsCount == Items.Count - FirstVisibleItemIndex;
			if(DropDownButton == null || CustomizationButton == null) return;
			if(!isFullyVisible) {
				DropDownButton.Opacity = 1;
				DropDownButton.IsHitTestVisible = true;
				IsDropDownButtonVisible = true;
				CustomizationButton.Opacity = 0;
				CustomizationButton.IsHitTestVisible = false;
				IsCustomizationButtonVisible = false;
			}
			else {			   
				if(ShowCustomizationButton) {					
					CustomizationButton.Opacity = 1;
					CustomizationButton.IsHitTestVisible = true;
					IsCustomizationButtonVisible = true;
				}
				else {					
					CustomizationButton.Opacity = 0;
					CustomizationButton.IsHitTestVisible = false;
					IsCustomizationButtonVisible = false;
				}				
				DropDownButton.Opacity = 0;
				DropDownButton.IsHitTestVisible = false;
				IsDropDownButtonVisible = false;
			}
		}
		private RibbonQuickAccessToolbarControl CreatePopupControl() {
			return new RibbonQuickAccessToolbarControl(Toolbar, VisibleItemsCount) { Placement = RibbonQuckAccessToolbarPlacement.Popup, SpacingMode = SpacingMode };
		}
		void UpdateBorderTemplates() {
			if(RightContentControl == null || LeftContentControl == null)
				return;
			switch(Placement) {
				case RibbonQuckAccessToolbarPlacement.Footer:
					LeftContentControl.Template = LeftContentTemplateInFooterForOffice2007RibbonStyle;
					RightContentControl.Template = RightContentTemplateInFooterForOffice2007RibbonStyle;;
					break;
				case RibbonQuckAccessToolbarPlacement.Header:
					switch(RibbonStyle) {
						case RibbonStyle.Office2007:
							LeftContentControl.Template = LeftContentTemplateInHeaderForOffice2007RibbonStyle;
							RightContentControl.Template = RightContentTemplateInHeaderForOffice2007RibbonStyle;;
							break;
						case RibbonStyle.TabletOffice:
						case RibbonStyle.OfficeSlim:
						case RibbonStyle.Office2010:
							LeftContentControl.Template = LeftContentTemplateInHeaderForOffice2010RibbonStyle;
							RightContentControl.Template = RightContentTemplateInHeaderForOffice2010RibbonStyle;
							break;
					}
					break;
				case RibbonQuckAccessToolbarPlacement.AeroHeader:
					switch(RibbonStyle) {
						case RibbonStyle.Office2007:
							LeftContentControl.Template = LeftContentTemplateInAeroHeaderForOffice2007RibbonStyle;
							RightContentControl.Template = RightContentTemplateInAeroHeaderForOffice2007RibbonStyle;
							break;
						case RibbonStyle.TabletOffice:
						case RibbonStyle.OfficeSlim:
						case RibbonStyle.Office2010:
							LeftContentControl.Template = LeftContentTemplateInAeroHeaderForOffice2010RibbonStyle;
							RightContentControl.Template = RightContentTemplateInAeroHeaderForOffice2010RibbonStyle;
							break;
					}
					break;
				case RibbonQuckAccessToolbarPlacement.Popup:
					LeftContentControl.Template = LeftContentTemplateInPopup;
					RightContentControl.Template = RightContentTemplateInPopup;
					break;
				case RibbonQuckAccessToolbarPlacement.Title:
					LeftContentControl.Template = LeftContentTemplateInTitle;
					RightContentControl.Template = RightContentTemplateInTitle;
					break;
			}
		}
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Right && Placement == RibbonQuckAccessToolbarPlacement.Popup) {
				if(Toolbar.Ribbon.ShowRibbonPopupMenu(Mouse.DirectlyOver as DependencyObject)) {
					e.Handled = true;
				}
			}
			base.OnPreviewMouseUp(e);
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			SubscribeContainerEvents(element as BarItemLinkControl);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			UnsubscribeContainerEvents(element as BarItemLinkControl);
		}
		protected virtual void SubscribeContainerEvents(BarItemLinkControl linkControl) {
			if(linkControl == null)
				return;
			linkControl.Click += new RoutedEventHandler(OnItemClick);
		}		
		void OnItemClick(object sender, RoutedEventArgs e) {
			CloseCustomizationMenu();
			Toolbar.Control.Popup.IsOpen = false;
		}
		protected virtual void UnsubscribeContainerEvents(BarItemLinkControl linkControl) {
			if(linkControl == null)
				return;
			linkControl.Click -= OnItemClick;	
		}
		internal double GetMinDesiredWidth() {
			if(!IsVisible || ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return DesiredSize.Width;
			double itemWidth = 0d;
			double allItemsWidth = 0d;
			if (Items.Count > 0) {
				var elem = ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
				if (elem != null)
					itemWidth = elem.DesiredSize.Width;
				allItemsWidth = Items.OfType<object>().Take(VisibleItemsCount).Select(child => (UIElement)ItemContainerGenerator.ContainerFromItem(child)).Sum(child => child.DesiredSize.Width);
			}
			return DesiredSize.Width - allItemsWidth + itemWidth + 1;
		}
	}
	public class QuickAccessToolbarCustomizationMenu : PopupMenu {
		public QuickAccessToolbarCustomizationMenu() {
			IsBranchHeader = true;
		}
	}	
}
