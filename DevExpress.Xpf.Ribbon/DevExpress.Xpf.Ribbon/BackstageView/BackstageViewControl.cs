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
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Xpf.Bars;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using System.Windows.Threading;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class BackstageViewControl : ItemsControl, IComplexLayout {
		#region static      
		private static readonly object selectedTabChangedEventHandler = new object();
		public static readonly DependencyProperty ControlPaneStyleProperty;		
		public static readonly DependencyProperty BackgroundGlyphProperty;
		public static readonly DependencyProperty ActualBackgroundGlyphProperty;
		protected static readonly DependencyPropertyKey ActualBackgroundGlyphPropertyKey;
		public static readonly DependencyProperty BackgroundStyleProperty;
		public static readonly DependencyProperty BackgroundGlyphStyleProperty;
		public static readonly DependencyProperty TabPaneStyleProperty;
		public static readonly DependencyProperty ItemsPresenterStyleProperty;
		public static readonly DependencyProperty SelectedTabProperty;
		public static readonly DependencyProperty ActualControlPaneProperty;
		protected static readonly DependencyPropertyKey ActualControlPanePropertyKey;		
		public static readonly DependencyProperty TabPaneMinWidthProperty;
		public static readonly DependencyProperty SelectedTabIndexProperty;
		public static readonly DependencyProperty DisableDefaultBackgroundGlyphProperty;		
		public static readonly DependencyProperty IsBackgroundGlyphVisibleProperty;
		protected static readonly DependencyPropertyKey IsBackgroundGlyphVisiblePropertyKey;
		public static readonly DependencyProperty IsFullScreenProperty;
		public static readonly DependencyProperty IsOpenProperty;
		public static readonly DependencyProperty EnableWindowTitleShrinkProperty;
		static BackstageViewControl() {
			BackgroundGlyphProperty = DependencyPropertyManager.Register("BackgroundGlyph", typeof(ImageSource), typeof(BackstageViewControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundGlyphPropertyChanged)));
			ActualBackgroundGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBackgroundGlyph", typeof(ImageSource), typeof(BackstageViewControl),
				new FrameworkPropertyMetadata(null));
			ActualBackgroundGlyphProperty = ActualBackgroundGlyphPropertyKey.DependencyProperty;
			ControlPaneStyleProperty = DependencyPropertyManager.Register("ControlPaneStyle", typeof(Style), typeof(BackstageViewControl), new FrameworkPropertyMetadata(null));			
			BackgroundStyleProperty = DependencyPropertyManager.Register("BackgroundStyle", typeof(Style), typeof(BackstageViewControl), new FrameworkPropertyMetadata(null));
			BackgroundGlyphStyleProperty = DependencyPropertyManager.Register("BackgroundGlyphStyle", typeof(Style), typeof(BackstageViewControl), new FrameworkPropertyMetadata(null));
			EnableWindowTitleShrinkProperty = DependencyPropertyManager.Register("EnableWindowTitleShrink", typeof(bool), typeof(BackstageViewControl), new FrameworkPropertyMetadata(false, OnEnableWindowTitleShrinkPropertyChanged));
			TabPaneStyleProperty = DependencyPropertyManager.Register("TabPaneStyle", typeof(Style), typeof(BackstageViewControl), new FrameworkPropertyMetadata(null));
			ItemsPresenterStyleProperty = DependencyPropertyManager.Register("ItemsPresenterStyle", typeof(Style), typeof(BackstageViewControl), new FrameworkPropertyMetadata(null));
			SelectedTabProperty = DependencyPropertyManager.Register("SelectedTab", typeof(BackstageTabItem), typeof(BackstageViewControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTabPropertyChanged)));
			ActualControlPanePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualControlPane", typeof(object), typeof(BackstageViewControl), new FrameworkPropertyMetadata(null));
			ActualControlPaneProperty = ActualControlPanePropertyKey.DependencyProperty;						
			TabPaneMinWidthProperty = DependencyPropertyManager.Register("TabPaneMinWidth", typeof(double), typeof(BackstageViewControl), new FrameworkPropertyMetadata(0d));
			SelectedTabIndexProperty = DependencyPropertyManager.Register("SelectedTabIndex", typeof(int), typeof(BackstageViewControl),
				new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedTabIndexPropertyChanged)));
			DisableDefaultBackgroundGlyphProperty = DependencyPropertyManager.Register("DisableDefaultBackgroundGlyph", typeof(bool), typeof(BackstageViewControl),
				new FrameworkPropertyMetadata(true, OnDisableDefaultBackgroundGlyphPropertyChanged));
			IsBackgroundGlyphVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsBackgroundGlyphVisible", typeof(bool), typeof(BackstageViewControl), new FrameworkPropertyMetadata(true));
			IsBackgroundGlyphVisibleProperty = IsBackgroundGlyphVisiblePropertyKey.DependencyProperty;
			IsFullScreenProperty = DependencyPropertyManager.Register("IsFullScreen", typeof(bool), typeof(BackstageViewControl), new FrameworkPropertyMetadata(false, OnIsFullScreenPropertyChanged));
			IsOpenProperty = DependencyPropertyManager.Register("IsOpen", typeof(bool), typeof(BackstageViewControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsOpenPropertyChanged)));
			RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(typeof(BackstageViewControl), new FrameworkPropertyMetadata(OnRibbonStylePropertyChanged));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BackstageViewControl), typeof(BackstageViewControlAutomationPeer), owner => new BackstageViewControlAutomationPeer((BackstageViewControl)owner));
		}
		protected static void OnBackgroundGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnBackgroundGlyphChanged(e.OldValue as ImageSource);
		}
		protected static void OnSelectedTabPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnSelectedTabChanged(e.OldValue as BackstageTabItem);
		}
		protected static void OnSelectedTabIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnSelectedTabIndexChanged((int)e.OldValue);
		}
		protected static void OnDisableDefaultBackgroundGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnDisableDefaultBackgroundGlyphPropertyChanged((bool)e.OldValue);
		}
		static void OnEnableWindowTitleShrinkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnEnableWindowTitleShrinkChanged((bool)e.OldValue);
		}
		protected static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnIsOpenChanged((bool)e.OldValue);
		}
		public static readonly DependencyProperty RibbonStyleProperty;
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		protected static void OnIsFullScreenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewControl)d).OnIsFullScreenChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public Style ControlPaneStyle {
			get { return (Style)GetValue(ControlPaneStyleProperty); }
			set { SetValue(ControlPaneStyleProperty, value); }
		}
		public ImageSource BackgroundGlyph {
			get { return (ImageSource)GetValue(BackgroundGlyphProperty); }
			set { SetValue(BackgroundGlyphProperty, value); }
		}
		public ImageSource ActualBackgroundGlyph {
			get { return (ImageSource)GetValue(ActualBackgroundGlyphProperty); }
			protected set { this.SetValue(ActualBackgroundGlyphPropertyKey, value); }
		}
		public Style BackgroundStyle {
			get { return (Style)GetValue(BackgroundStyleProperty); }
			set { SetValue(BackgroundStyleProperty, value); }
		}
		public Style BackgroundGlyphStyle {
			get { return (Style)GetValue(BackgroundGlyphStyleProperty); }
			set { SetValue(BackgroundGlyphStyleProperty, value); }
		}
		public bool EnableWindowTitleShrink {
			get { return (bool)GetValue(EnableWindowTitleShrinkProperty); }
			set { SetValue(EnableWindowTitleShrinkProperty, value); }
		}
		public Style TabPaneStyle {
			get { return (Style)GetValue(TabPaneStyleProperty); }
			set { SetValue(TabPaneStyleProperty, value); }
		}
		public Style ItemsPresenterStyle {
			get { return (Style)GetValue(ItemsPresenterStyleProperty); }
			set { SetValue(ItemsPresenterStyleProperty, value); }
		}
		public BackstageTabItem SelectedTab {
			get { return (BackstageTabItem)GetValue(SelectedTabProperty); }
			set { SetValue(SelectedTabProperty, value); }
		}
		public object ActualControlPane {
			get { return (object)GetValue(ActualControlPaneProperty); }
			protected set { this.SetValue(ActualControlPanePropertyKey, value); }
		}
		public double TabPaneMinWidth {
			get { return (double)GetValue(TabPaneMinWidthProperty); }
			set { SetValue(TabPaneMinWidthProperty, value); }
		}
		public int SelectedTabIndex {
			get { return (int)GetValue(SelectedTabIndexProperty); }
			set { SetValue(SelectedTabIndexProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool DisableDefaultBackgroundGlyph {
			get { return (bool)GetValue(DisableDefaultBackgroundGlyphProperty); }
			set { SetValue(DisableDefaultBackgroundGlyphProperty, value); }
		}
		public bool IsBackgroundGlyphVisible {
			get { return (bool)GetValue(IsBackgroundGlyphVisibleProperty); }
			protected set { this.SetValue(IsBackgroundGlyphVisiblePropertyKey, value); }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public bool IsFullScreen {
			get { return (bool)GetValue(IsFullScreenProperty); }
			set { SetValue(IsFullScreenProperty, value); }
		}
		#endregion
		#region events
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		public event RibbonPropertyChangedEventHandler SelectedTabChanged {
			add { Events.AddHandler(selectedTabChangedEventHandler, value); }
			remove { Events.RemoveHandler(selectedTabChangedEventHandler, value); }
		}
		protected void RaiseSelectedTabChanged(RibbonPropertyChangedEventArgs e) {
			RibbonPropertyChangedEventHandler handler = Events[selectedTabChangedEventHandler] as RibbonPropertyChangedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		#endregion
		public BackstageViewControl() {
			Foreground = Brushes.Black;
			DefaultStyleKey = typeof(BackstageViewControl);
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			UpdateActualBackgroundGlyph();
			TabsCore = new List<BackstageItemBase>();
			ImmediateActionsManager = new ImmediateActionsManager(this);
		}
		protected internal ImmediateActionsManager ImmediateActionsManager { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonControl Ribbon { get;  protected internal set; }
		protected internal BackstageViewContentHost Host {
			get { return host; }
			private set {
				if(value == host) return;
				BackstageViewContentHost oldValue = host;
				host = value;
				OnHostChanged(oldValue);
			}
		}
		protected virtual void OnIsFullScreenChanged(bool oldValue) { }
		protected internal void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateSelectedTabAndIndex();
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			var ribbon = LayoutHelper.FindLayoutOrVisualParentObject<RibbonControl>(this, true);
			if(ribbon != null)
				SetBinding(RibbonStyleProperty, new Binding(RibbonStyleProperty.Name) { Source = ribbon });
		}
		protected internal void UpdateSelectedTabAndIndex() {
			int count = GetTabCount();
			if (SelectedTabIndex >= 0 && SelectedTabIndex < count)
				SetCurrentValue(SelectedTabProperty, GetTabFromIndex(SelectedTabIndex));
			else
				SetCurrentValue(SelectedTabProperty, GetFirstEnabledTab(count) ?? GetTabFromIndex(0));
		}
		BackstageTabItem GetFirstEnabledTab(int count) {
			for(int i = 0; i < count; i++) {
				var tab = GetTabFromIndex(i);
				if(tab != null && tab.IsEnabled) {
					return tab;
				}
			}
			return null;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			if(ComplexLayoutStateCore == ComplexLayoutState.Updating && RibbonSelectedPageControl.IsItemsControlFullyLoaded(this))
				SetComplexLayoutState(Bars.ComplexLayoutState.Updated);
			ImmediateActionsManager.ExecuteActions();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
			SetComplexLayoutState(ComplexLayoutState.Updating);
			BindingOperations.ClearBinding(this, RibbonStyleProperty);
		}
		EventHandlerList events;
		public int TabCount { get { return GetTabCount(); } }
		[Browsable(false)]
		public List<BackstageTabItem> Tabs { get { return GetTabList(); } }
		protected internal List<BackstageItemBase> TabsCore { get; private set; }
		protected bool IsSelectionSynchronizationActive { get; set; }		
		protected virtual void OnBackgroundGlyphChanged(ImageSource oldValue) {
			UpdateActualBackgroundGlyph();
		}
		protected virtual void OnDisableDefaultBackgroundGlyphPropertyChanged(bool oldValue) {
			UpdateActualBackgroundGlyph();
		}
		protected virtual void OnEnableWindowTitleShrinkChanged(bool oldValue) { }
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return (item is BackstageItemBase);
		}
		protected virtual void OnSelectedTabChanged(BackstageTabItem oldValue) {
			ResetItemsFocus(false);
			if(!IsSelectionSynchronizationActive) {
				IsSelectionSynchronizationActive = true;
				SetCurrentValue(SelectedTabIndexProperty, GetTabIndex(SelectedTab));
				IsSelectionSynchronizationActive = false;
			}
			if(oldValue != null)
				oldValue.IsSelected = false;
			if(SelectedTab != null) {
				SelectedTab.IsSelected = true;
			}
			UpdateActualControlPane();
			RaiseSelectedTabChanged(new RibbonPropertyChangedEventArgs(oldValue, SelectedTab));
			FocusFirstAvailableDescendant();
		}		
		protected void ResetItemsFocus(bool ignoreTabItems) {
			foreach(object item in Items) {
				BackstageItemBase backstageItem = GetBackstageItemFromItem(item);				
				if(backstageItem == null || (ignoreTabItems && backstageItem is BackstageTabItem))
					continue;
				backstageItem.ActualIsFocused = false;
			}
		}
		protected virtual void OnSelectedTabIndexChanged(int oldValue) {
			if(!IsSelectionSynchronizationActive) {
				IsSelectionSynchronizationActive = true;
				SetCurrentValue(SelectedTabProperty, GetTabFromIndex(SelectedTabIndex));
				IsSelectionSynchronizationActive = false;
			}
		}
		protected virtual void OnIsOpenChanged(bool oldValue) {
			UpdateOnIsOpenChanged();
		}
		void UpdateOnIsOpenChanged() {
			if(this.IsInDesignTool() || Ribbon == null || !Ribbon.IsInVisualTree())
				return;
			if (IsOpen) {
				if (!Ribbon.IsBackStageViewOpen)
					Ribbon.ShowApplicationMenu();
				FocusFirstAvailableDescendant();
			}
			if(!IsOpen)
				Ribbon.CloseApplicationMenu();
		}
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) { }
		protected internal void UpdateFocus() {
			Focus();
			FocusFirstAvailableDescendant();
		}
		void FocusFirstAvailableDescendant() {
			ImmediateActionsManager.EnqueueAction(new Action(() => {
				if (Host == null) return;
				Host.FocusFirstAvailableDescendant();
			}));
		}
		protected virtual void UpdateActualBackgroundGlyph() {
			if(BackgroundGlyph != null) {
				ActualBackgroundGlyph = BackgroundGlyph;
				IsBackgroundGlyphVisible = true;
				return;
			}
			if(DisableDefaultBackgroundGlyph) {
				IsBackgroundGlyphVisible = false;
				ActualBackgroundGlyph = null;				
				return;
			}
			ThemeTreeWalker themeTreeWalker = ThemeManager.GetTreeWalker(this);
			if(themeTreeWalker != null) {
				ImageSource themeGlyph = ImageHelper.GetThemeImage(themeTreeWalker.ThemeName, "BackstageBackgroundGlyph");
				if(themeGlyph != null) {
					ActualBackgroundGlyph = themeGlyph;
					IsBackgroundGlyphVisible = true;
					return;
				}
			}
			ActualBackgroundGlyph = ImageHelper.GetImage("BackstageBackgroundGlyph.png");
			IsBackgroundGlyphVisible = ActualBackgroundGlyph != null;
		}
		protected int GetTabCount() {
			int tabCount = 0;
			foreach (object item in Items) {
				if (item is BackstageTabItem) {
					tabCount++;
				} else {
					var container = ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
					if (container == null)
						continue;
					if (VisualTreeHelper.GetChildrenCount(container) > 0 && VisualTreeHelper.GetChild(container, 0) is BackstageTabItem)
						tabCount++;
				}
			}
			return tabCount;
		}
		protected List<BackstageTabItem> GetTabList() {
			return Items.Cast<object>().OfType<BackstageTabItem>().ToList();
		}	   
		protected internal virtual void UpdateActualControlPane() {
			if(SelectedTab == null) {
				ActualControlPane = null;
				return;
			}
			ActualControlPane = SelectedTab.ControlPane;
		}
		public int GetTabIndex(BackstageTabItem tabItem) {
			if (tabItem == null)
				return (int)SelectedTabIndexProperty.DefaultMetadata.DefaultValue;
			return TabsCore.IndexOf(tabItem);
		}
		public BackstageTabItem GetTabFromIndex(int tabIndex) {
			var tab = GetFromTabsCore(tabIndex);
			if(tab != null)
				return tab;
			int index = 0;
			foreach(object item in Items) {
				if(item is BackstageTabItem) {
					if(index == tabIndex)
						return item as BackstageTabItem;
					index++;
				}
			}
			return null;
		}
		BackstageTabItem GetFromTabsCore(int tabIndex) {
			if(tabIndex >= 0 && tabIndex < TabsCore.Count) {
				return TabsCore[tabIndex] as BackstageTabItem;
			}
			return null;
		}
		private WeakReference peerWr = new WeakReference(null);
		internal System.Windows.Automation.Peers.AutomationPeer Peer {
			get { return (System.Windows.Automation.Peers.AutomationPeer)peerWr.Target; }
			set { peerWr = new WeakReference(value); }
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			var peer = DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
			Peer = peer;
			return peer;
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				foreach(object item in Items) {
					if(item is BackstageItemBase)
						((BackstageItemBase)item).Backstage = this;
				}			   
			}
			if(e.OldItems != null) {
				foreach(object item in e.OldItems) {
					if(item is BackstageItemBase) {
						if(item is BackstageTabItem && ((BackstageTabItem)item).IsSelected)
							SetCurrentValue(SelectedTabProperty, null);
						((BackstageItemBase)item).Backstage = null;						
					}
				}
			}
			if(e.NewItems != null) {
				foreach(object item in e.NewItems) {
					if(item is BackstageItemBase)
						((BackstageItemBase)item).Backstage = this;
				}
			}
		}
		protected internal virtual void OnItemClick(BackstageItemBase item) {
			ResetItemsFocus(false);
		}
		public void Close() {
			if (Ribbon == null)
				return;
			Ribbon.CloseApplicationMenu();
		}
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.OnKeyDown(e);
			switch(e.Key) {
				case Key.Down:
					OnDownKeyDown();
					e.Handled = true;
					break;
				case Key.Up:
					OnUpKeyDown();
					e.Handled = true;
					break;
				case Key.Escape:
					OnEscKeyDown();
					e.Handled = true;
					break;
				case Key.Tab:
					if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
						OnCtrlTabKeyDown((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift);
						e.Handled = true;
					}
					break;
				case Key.Space:
					e.Handled = OnSpaceKeyDown();
					break;
				case Key.Enter:
					e.Handled = OnEnterKeyDown();
					break;
			}
		}
		protected internal virtual void OnUpKeyDown() {
			NavigateToPriorFocusableItem();
		}
		protected internal virtual void OnDownKeyDown() {
			NavigateToNextFocusableItem();
		}
		protected internal virtual void OnEscKeyDown() {
			Close();			
		}
		protected internal virtual void OnTabKeyDown() {
			NavigateToNextFocusableItem();
		}
		protected internal virtual void OnCtrlTabKeyDown(bool forward) {
			if (forward)
				NavigateToNextFocusableItem();
			else
				NavigateToPriorFocusableItem();
		}
		protected internal virtual bool OnSpaceKeyDown() {
			return ExecuteCurrentFocusedItemAction();
		}
		protected internal virtual bool OnEnterKeyDown() {
			return ExecuteCurrentFocusedItemAction();
		}
		protected virtual bool ExecuteCurrentFocusedItemAction() {
			BackstageItemBase item = GetCurrentFocusedItem();
			if(item == null)
				return false;
			if(item is BackstageButtonItem) {								
				item.OnClick();
				return true;
			}
			return false;
		}
		protected virtual void NavigateToNextFocusableItem() {
			SetCurrentFocusedItem(GetCurrentFocusedItem(), GetNextFocusableItem(GetCurrentFocusedItem()));
		}
		protected void SetCurrentFocusedItem(BackstageItemBase oldFocusedItem, BackstageItemBase newFocusedItem) {
			if(oldFocusedItem == null && SelectedTab != null) {
				SelectedTab.ActualIsFocused = true;
			}
			if(newFocusedItem != null) {
				if(newFocusedItem is BackstageTabItem) {
					ResetItemsFocus(false);
					SetCurrentValue(BackstageViewControl.SelectedTabProperty, newFocusedItem);
				}
				else
					ResetItemsFocus(true);
				newFocusedItem.ActualIsFocused = true;
			}
		}
		protected virtual void OnHostChanged(BackstageViewContentHost oldValue) {
			if(oldValue != null)
				oldValue.BackstageView = null;
			if(Host != null)
				Host.BackstageView = this;
		}
		protected virtual void NavigateToPriorFocusableItem() {
			SetCurrentFocusedItem(GetCurrentFocusedItem(), GetPriorFocusableItem(GetCurrentFocusedItem()));
		}
		protected internal virtual BackstageItemBase GetCurrentFocusedItem() {
			List<BackstageItemBase> focusableItemList = GetFocusableItemList();
			BackstageItemBase result = null;
			foreach(BackstageItemBase item in focusableItemList) {
				if(item.ActualIsFocused) {
					if(!(item is BackstageTabItem)) {
						result = item;
					}
					else if(result == null)
						result = item;
				}
			}
			return result;
		}
		BackstageItemBase GetBackstageItemFromContentPresenter(ContentPresenter presenter) {			
			if(presenter == null || VisualTreeHelper.GetChildrenCount(presenter) == 0)
				return null;
			return VisualTreeHelper.GetChild(presenter, 0) as BackstageItemBase;						
		}
		BackstageItemBase GetBackstageItemFromItem(object item) {			
			BackstageItemBase backstageItem = item as BackstageItemBase;
			if(backstageItem != null)
				return backstageItem;
			return GetBackstageItemFromContentPresenter(ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter);
		}
		List<BackstageItemBase> GetFocusableItemList() {
			List<BackstageItemBase> list = new List<BackstageItemBase>();
			foreach(object item in Items) {
				BackstageItemBase backstageItem = GetBackstageItemFromItem(item);
				if(backstageItem == null || !backstageItem.ActualIsEnabled)
					continue;
				if(backstageItem is BackstageSeparatorItem)
					continue;
				list.Add(backstageItem);
			}
			return list;
		}
		BackstageItemBase GetNextFocusableItem(BackstageItemBase item) {
			List<BackstageItemBase> focusableItemList = GetFocusableItemList();			
			if(focusableItemList.Count == 0)
				return null;
			int index = 0;
			if(item == null) {
				if(SelectedTab != null)
					index = focusableItemList.IndexOf(SelectedTab) + 1;
			}
			else
				index = focusableItemList.IndexOf(item) + 1;
			if(index >= focusableItemList.Count)
				index = 0;
			return focusableItemList[index];
		}
		BackstageItemBase GetPriorFocusableItem(BackstageItemBase item) {
			List<BackstageItemBase> focusableItemList = GetFocusableItemList();
			if(focusableItemList.Count == 0)
				return null;
			int index = focusableItemList.Count - 1;
			if(item == null) {
				if(SelectedTab != null)
					index = focusableItemList.IndexOf(SelectedTab) - 1;
			}
			else
				index = focusableItemList.IndexOf(item) - 1;
			if(index < 0)
				index = focusableItemList.Count - 1;
			return focusableItemList[index];
		}
		internal double GetLeftPartActualWidth() {
			if (Host == null || Host.LeftElement == null)
				return 0d;
			return Host.LeftElement.ActualWidth;
		}
		public override void OnApplyTemplate() {
			Host = GetTemplateChild("PART_ContentHost") as BackstageViewContentHost;
			UpdateActualBackgroundGlyph();
			base.OnApplyTemplate();
			Dispatcher.BeginInvoke(new Action(() => {
				var peer = Peer;
				if (peer != null) {		  
					peer.ResetChildrenCache();
					peer.RaiseAutomationEvent(System.Windows.Automation.Peers.AutomationEvents.StructureChanged);
				}
			}));			
		}
		#region IComplexLayout Members
		protected ComplexLayoutState ComplexLayoutStateCore { get; set; }
		public ComplexLayoutState ComplexLayoutState {
			get { return ComplexLayoutStateCore; }
		}
		public event ComplexLayoutStateChangedEventHandler ComplexLayoutStateChanged;
		protected virtual void SetComplexLayoutState(ComplexLayoutState state) {
			ComplexLayoutState oldState = ComplexLayoutStateCore;			
			ComplexLayoutStateCore = state;
			if(oldState != state) {
				if(ComplexLayoutStateChanged != null) {
					ComplexLayoutStateChanged(this, new ComplexLayoutStateChangedEventArgs(ComplexLayoutStateCore));
				}
			}
		}
		#endregion
		BackstageViewContentHost host;
	}
	public class BackstageViewLeftPartContentControl : ContentControl { }
	public class BackstageViewContentHost : Control {
		#region Dep props
		public object LeftContent {
			get { return (object)GetValue(LeftContentProperty); }
			set { SetValue(LeftContentProperty, value); }
		}
		public object RightContent {
			get { return (object)GetValue(RightContentProperty); }
			set { SetValue(RightContentProperty, value); }
		}
		public static TimeSpan GetAnimationTime(DependencyObject obj) {
			return (TimeSpan)obj.GetValue(AnimationTimeProperty);
		}
		public static void SetAnimationTime(DependencyObject obj, TimeSpan value) {
			obj.SetValue(AnimationTimeProperty, value);
		}
		public static readonly DependencyProperty AnimationTimeProperty =
			DependencyPropertyManager.RegisterAttached("AnimationTime", typeof(TimeSpan), typeof(BackstageViewContentHost), new FrameworkPropertyMetadata(new TimeSpan(0)));
		public static readonly DependencyProperty RightContentProperty =
			DependencyPropertyManager.Register("RightContent", typeof(object), typeof(BackstageViewContentHost), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty LeftContentProperty =
			DependencyPropertyManager.Register("LeftContent", typeof(object), typeof(BackstageViewContentHost), new FrameworkPropertyMetadata(null));
		#endregion
		public BackstageViewContentHost() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			SizeChanged += OnSizeChanged;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(!BackstageView.IsFullScreen)
				return;
		}
		public FrameworkElement LeftElement { get; set; }
		public FrameworkElement RightElement { get; set; }
		public RibbonCheckedBorderControl GlyphElement {
			get { return glyphElement; }
			set {
				if(glyphElement != value) {
					var oldValue = glyphElement;
					glyphElement = value;
					OnGlyphElementChanged(oldValue);
				}
			}
		}
		public BackstageViewControl BackstageView {
			get { return backstageView; }
			set {
				if(BackstageView == value) return;
				OnBackstageViewChanging(value);
				var oldValue = BackstageView;
				backstageView = value;
				OnBackstageViewChanged(oldValue);
			}
		}
		public Panel Root { get; set; }
		public TimeSpan AnimationTime { get; set; }
		public RibbonControl Ribbon { get { return BackstageView == null ? null : BackstageView.Ribbon; } }
		BackstageViewOffice2013Clipper Clipper {
			get { return clipper; }
			set {
				if(Clipper == value) return;
				var oldValue = Clipper;
				clipper = value;
				OnClipperChanged(oldValue);
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LeftElement = GetTemplateChild("PART_Left") as FrameworkElement;
			RightElement = GetTemplateChild("PART_Right") as FrameworkElement;
			if (LeftElement != null) {
				LeftElement.Focusable = false;
				KeyboardNavigation.SetTabNavigation(LeftElement, KeyboardNavigationMode.None);
				KeyboardNavigation.SetControlTabNavigation(LeftElement, KeyboardNavigationMode.None);
				KeyboardNavigation.SetDirectionalNavigation(LeftElement, KeyboardNavigationMode.None);
			}
			if (RightElement != null) {
				RightElement.Focusable = false;
				KeyboardNavigation.SetTabNavigation(RightElement, KeyboardNavigationMode.Cycle);
				KeyboardNavigation.SetControlTabNavigation(RightElement, KeyboardNavigationMode.None);
				KeyboardNavigation.SetDirectionalNavigation(RightElement, KeyboardNavigationMode.Cycle);
			}
			GlyphElement = GetTemplateChild("PART_Glyph") as RibbonCheckedBorderControl;
			Root = GetTemplateChild("PART_Root") as Panel;
			if(BackstageView != null) {
				BackstageView.ImmediateActionsManager.EnqueueAction(() => FocusFirstAvailableDescendant());
			}
			Clipper = new BackstageViewOffice2013Clipper(this);
		}
		protected virtual void OnGlyphElementChanged(RibbonCheckedBorderControl oldValue) {
			if(oldValue != null) {
				oldValue.MouseLeftButtonDown -= OnGlyphElementMouseLeftButtonDown;
			}
			if(GlyphElement != null) {
				GlyphElement.MouseLeftButtonDown += OnGlyphElementMouseLeftButtonDown;
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if(BackstageView.IsOpen && BackstageView.IsFullScreen && GlyphElement != null) {
				VisualStateManager.GoToState(GlyphElement, "Loading", false);
				UpdateClip();
				PerformLoadingAnimation();
			}
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			if(BackstageView == null || !BackstageView.IsFullScreen)
				return;
			ShowRibbonElements();
			if(GlyphElement != null)
				VisualStateManager.GoToState(GlyphElement, "Intermediate", false);
			VisualStateManager.GoToState(this, "Intermediate", false);
		}		
		protected internal virtual void FocusFirstAvailableDescendant() {
			if (RightElement == null) return;
			VisualTreeEnumerator enumerator = new VisualTreeEnumerator(RightElement);
			while (enumerator.MoveNext()) {
				FrameworkElement element = enumerator.Current as FrameworkElement;
				if (element != null && element.IsVisible && element.Focusable) {
					element.Focus();
					break;
				}
			}
		}
		protected virtual void OnBackstageViewChanging(BackstageViewControl value) {
			if(BackstageView != null && Clipper != null)
				BackstageView.ClearValue(BackstageViewControl.ClipProperty);
			ShowRibbonElements();
			Clipper = new BackstageViewOffice2013Clipper(this);
		}
		protected internal virtual void ShowRibbonElements() {
			ShowAppButton();
			ShowQATAndPageCategories();
		}
		protected virtual void OnBackstageViewChanged(BackstageViewControl oldValue) { }
		DispatcherTimer tickTimer;
		DispatcherTimer TickTimer {
			get {
				if(tickTimer == null) {
					tickTimer = new DispatcherTimer();
					tickTimer.Tick += (o, e) => {
						tickTimer.Stop();
						OnAfterCloseAnimationFinished();
					};
				}
				return tickTimer;
			}
		}		
		void OnGlyphElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			OnCloseAnimationStarted();
			if(AnimationTime.TotalMilliseconds == 0) {
				OnAfterCloseAnimationFinished();
			} else {
				TickTimer.Interval = AnimationTime;
				TickTimer.Start();
			}
		}
		void OnCloseAnimationStarted() {			
			if(!BackstageView.IsFullScreen || Ribbon == null)
				return;
			VisualStateManager.GoToState(GlyphElement, "Unloading", false);
			PerformUnloadingAnimation();
		}
		void OnAfterCloseAnimationFinished() {
			if(BackstageView == null && (BackstageView = LayoutHelper.FindParentObject<BackstageViewControl>(this)) == null) {
				return;
			}
			BackstageView.IsOpen = false;
			if(RightElement != null)
				RightElement.BeginAnimation(FrameworkElement.OpacityProperty, null);
			if(Root != null && Root.RenderTransform != null)
				Root.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);
		}
		void PerformUnloadingAnimation() {
			PerformFadeStoryboard(true);
			PerformOpacityStoryboard(true);
		}
		void PerformLoadingAnimation() {
			HideAppButton();
			HideQATAndPageCategories();
			if(Ribbon != null) {
				AnimationTime = GetAnimationTime(GlyphElement);
				PerformFadeStoryboard(false);
				PerformOpacityStoryboard(false);
			}
		}
		void ShowQATAndPageCategories() {
			if(Ribbon != null) {
				if(Ribbon.Toolbar != null && Ribbon.Toolbar.Control != null) {
					ShowUIElement(Ribbon.Toolbar.Control);
				}
				if(Ribbon.CategoriesPane != null && Ribbon.CategoriesPane.RibbonItemsPanel != null) {
					ShowUIElement(Ribbon.CategoriesPane);
				}
			}
		}
		void HideQATAndPageCategories() {
			if(Ribbon != null) {
				if(Ribbon.Toolbar != null && Ribbon.Toolbar.Control != null) {
					HideUIElement(Ribbon.Toolbar.Control);
				}
				if(Ribbon.CategoriesPane != null && Ribbon.CategoriesPane.RibbonItemsPanel != null) {
					HideUIElement(Ribbon.CategoriesPane);
				}
			}
		}
		void OnClipperChanged(BackstageViewOffice2013Clipper oldValue) {
			if(oldValue != null)
				oldValue.Host = null;
			UpdateClip();
		}
		void PerformFadeStoryboard(bool fadeout) {
			if(Root == null || AnimationTime.TotalMilliseconds == 0 || LeftElement == null) return;
			System.Windows.Media.Animation.Storyboard storyboard = new System.Windows.Media.Animation.Storyboard();
			System.Windows.Media.Animation.DoubleAnimation fadeAnimation = new System.Windows.Media.Animation.DoubleAnimation();
			fadeAnimation.Duration = AnimationTime;
			fadeAnimation.From = fadeout ? 0d : -LeftElement.ActualWidth;
			fadeAnimation.To = fadeout? -LeftElement.ActualWidth : 0d;
			fadeAnimation.EasingFunction = new System.Windows.Media.Animation.ExponentialEase();
			System.Windows.Media.Animation.Storyboard.SetTarget(fadeAnimation, Root);
			System.Windows.Media.Animation.Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
			storyboard.Children.Add(fadeAnimation);
			storyboard.Begin(this);
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			((System.Windows.Media.Animation.ClockGroup)sender).Completed -= OnStoryboardCompleted;
			ShowRibbonElements();
		}
		void PerformOpacityStoryboard(bool fadeout) {
			if(AnimationTime.TotalMilliseconds == 0 || RightElement == null) return;
			System.Windows.Media.Animation.Storyboard storyboard = new System.Windows.Media.Animation.Storyboard();
			System.Windows.Media.Animation.DoubleAnimation opacityAnimation = new System.Windows.Media.Animation.DoubleAnimation();
			opacityAnimation.Duration = AnimationTime;
			opacityAnimation.From = fadeout ? 1d : 0d;
			opacityAnimation.To = fadeout ? 0d : 1d;
			opacityAnimation.EasingFunction = new System.Windows.Media.Animation.ExponentialEase();
			System.Windows.Media.Animation.Storyboard.SetTarget(opacityAnimation, RightElement);
			System.Windows.Media.Animation.Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
			storyboard.Children.Add(opacityAnimation);
			if(fadeout)
				storyboard.Completed += OnStoryboardCompleted;
			storyboard.Begin(this);
		}
		void ShowAppButton() {
			if(BackstageView != null && BackstageView.Ribbon != null && BackstageView.Ribbon.ApplicationButton != null) {
				ShowUIElement(BackstageView.Ribbon.ApplicationButton);
			}
		}
		void HideAppButton() {
			if(BackstageView != null && BackstageView.Ribbon != null && BackstageView.Ribbon.ApplicationButton != null) {
				HideUIElement(BackstageView.Ribbon.ApplicationButton);
			}
		}
		void HideUIElement(UIElement elem) {
			elem.Opacity = 0;
			elem.IsHitTestVisible = false;
		}
		void ShowUIElement(UIElement elem) {
			elem.Opacity = 1;
			elem.IsHitTestVisible = true;
		}
		void UpdateClip() {
			Clipper.UpdateClip();
		}
		BackstageViewOffice2013Clipper clipper;
		BackstageViewControl backstageView;
		RibbonCheckedBorderControl glyphElement;
	}
	public class BackstageViewOffice2013Clipper {
		public BackstageViewContentHost Host {
			get { return backstageViewContentHost; }
			set {
				if(value == backstageViewContentHost) return;
				BackstageViewContentHost oldValue = backstageViewContentHost;
				backstageViewContentHost = value;
				OnBackstageViewContentHostChanged(oldValue);
			}
		}
		public BackstageViewControl BackstageView {
			get { return Host == null ? null : Host.BackstageView; }
		}
		public RibbonControl Ribbon {
			get { return Host.Ribbon; }
		}
		public FrameworkElement LeftElement {
			get { return Host.LeftElement; }
		}
		public FrameworkElement RightElement {
			get { return Host.RightElement; }
		}
		public BackstageViewOffice2013Clipper(BackstageViewContentHost host) {
			if(host == null)
				throw new ArgumentNullException("host");
			Host = host;
		}
		public virtual void UpdateClip() {
			if(BackstageView == null)
				return;
			var clip = CreateClip();
			if (clip != null)
				BackstageView.Clip = clip;
			else
				BackstageView.ClearValue(BackstageViewControl.ClipProperty);
			UpdateRightContentMargin();
		}
		public virtual void UpdateRightContentMargin() {
			if(Ribbon == null || RightElement == null)
				return;
			if(!BackstageView.EnableWindowTitleShrink) {
				RightElement.ClearValue(FrameworkElement.MarginProperty);
				return;
			}
			if(Ribbon.PageHeaderLinksControl.HasVisibleItems()) {
				Size pageHeadersSize = GetPageHeaderLinksControlSize();
				Point pageHeaderPosition = GetPageHeaderLinksControlPosition(Ribbon);
				RightElement.Margin = new Thickness(0, Math.Ceiling(pageHeaderPosition.Y + pageHeadersSize.Height), 0, 0);
			} else
				if(!Ribbon.IsAeroMode) {
					Point headerPosition = Ribbon.CategoriesPane.TranslatePoint(new Point(), Ribbon);
					RightElement.Margin = new Thickness(0, Math.Ceiling(headerPosition.Y + Ribbon.CategoriesPane.ActualHeight), 0, 0);
				} else
					RightElement.ClearValue(FrameworkElement.MarginProperty);
		}
		protected virtual void OnBackstageViewContentHostChanged(BackstageViewContentHost oldValue) {
			if(oldValue != null) {
				oldValue.SizeChanged -= OnBackstageViewSizeChanged;
				if(oldValue.LeftElement != null)
					oldValue.LeftElement.SizeChanged -= OnLeftElementSizeChanged;
			}
			if(Host != null) {
				Host.SizeChanged += OnBackstageViewSizeChanged;
				if(Host.LeftElement != null)
					Host.LeftElement.SizeChanged += OnLeftElementSizeChanged;
			}
		}
		protected virtual Geometry CreateWindowTitleGeometry(Point position) {
			Point leftTop = new Point(LeftElement.ActualWidth, 0d);
			Size size = new Size(Math.Max(0, BackstageView.ActualWidth - LeftElement.ActualWidth), Math.Max(0, position.Y));
			return new RectangleGeometry(new Rect(leftTop, size));
		}
		void OnBackstageViewSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateClip();
		}
		void OnLeftElementSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateClip();
		}
		Geometry CreateClip() {
			if(Ribbon == null || !Host.BackstageView.IsFullScreen || !Host.BackstageView.EnableWindowTitleShrink)
				return null;
			Point pageHeaderPosition = GetPageHeaderLinksControlPosition(Host);
			var ribbonWindowTitleGeometry = CreateWindowTitleGeometry(pageHeaderPosition);
			var pageHeadersGeometry = new RectangleGeometry(new Rect(pageHeaderPosition, GetPageHeaderLinksControlSize()));
			var ribbonPagesGeometry = Geometry.Combine(ribbonWindowTitleGeometry, pageHeadersGeometry, GeometryCombineMode.Union, Transform.Identity);
			return Geometry.Combine(new RectangleGeometry(new Rect(BackstageView.RenderSize)), ribbonPagesGeometry, GeometryCombineMode.Exclude, Transform.Identity);
		}
		Point GetPageHeaderLinksControlPosition(UIElement relativeTo) {
			if (Ribbon == null || Ribbon.PageHeaderLinksControl == null)
				return new Point();
			var control = Ribbon.PageHeaderLinksControl.HasItems ? (FrameworkElement)Ribbon.PageHeaderLinksControl : Ribbon.RightTabContainer;
			Point position = control.TranslatePoint(new Point(), relativeTo);
			if(Ribbon.WindowHelper != null && Ribbon.WindowHelper.IsAeroMode) {
				position.Y -= Ribbon.HeaderBorder.RenderSize.Height;
			}
			return position;
		}
		Size GetPageHeaderLinksControlSize() {
			return Ribbon == null || Ribbon.PageHeaderLinksControl == null || !Ribbon.PageHeaderLinksControl.HasItems ? new Size() : Ribbon.PageHeaderLinksControl.RenderSize;
		}
		BackstageViewContentHost backstageViewContentHost;
	}
}
