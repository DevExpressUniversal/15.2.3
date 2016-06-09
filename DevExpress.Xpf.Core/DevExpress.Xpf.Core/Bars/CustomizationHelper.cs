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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using DevExpress.Xpf.Bars.Customization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Utils.Serializing;
using System.Text;
namespace DevExpress.Xpf.Bars {
	public class BarManagerCustomizationHelper : DependencyObject, IRuntimeCustomizationHost {
		#region static
		public static readonly DependencyProperty ToolbarGlyphSizeProperty;
		public static readonly DependencyProperty MenuGlyphSizeProperty;
		public static readonly DependencyProperty ShowScreenTipsProperty;
		public static readonly DependencyProperty ShowShortcutInScreenTipsProperty;
		static int sNameIndexer = 0;
		static readonly string sNamePrefix;
		public static string GetSerializationName(DependencyObject obj) {
			var value = (string)obj.GetValue(SerializationNameProperty);
			if (!string.IsNullOrEmpty(value))
				return value;
			do {
				var name = (obj as IFrameworkInputElement).With(x => x.Name);
				if (!String.IsNullOrEmpty(name)) {
					value = name;
					break;
				}
				int count = 0;
				var parents = TreeHelper.GetParents(obj, true, true).TakeWhile(x => {
					if(!EmptyName(x) || x is IRuntimeCustomizationHost)
						count++;
					return count <= 1;
				}).Reverse().ToList();
				if (parents.Count == 1) {
					value = CompressString(sNamePrefix + obj.GetType().FullName + sNameIndexer++.ToString());
					break;
				}				
				StringBuilder builder = new StringBuilder();
				builder.Append((parents.First() as IFrameworkInputElement).Name);
				for (int i = 0; i < parents.Count - 1; i++) {
					var parent = parents[i];
					var index = LogicalTreeHelper.GetChildren(parent).OfType<object>().ToList().IndexOf(parents[i + 1]);
					builder.Append(parent.GetType().Name);
					builder.Append(index);
				}
				value = CompressString(builder.ToString());
			} while (false);
			SetSerializationName(obj, value);
			return value;
		}
		static string CompressString(string source) {
			StringBuilder builder = new StringBuilder();
			builder.Append("cName");
			foreach (var b in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(source))) {
				builder.Append(b.ToString("X2"));
			}
			return builder.ToString();
		}
		private static bool EmptyName(DependencyObject x) {
			return String.IsNullOrEmpty((x as IFrameworkInputElement).With(fi => fi.Name));
		}
		public static void SetSerializationName(DependencyObject obj, string value) {
			if((obj as Freezable).If(x => x.IsFrozen).ReturnSuccess())
				return;
			obj.SetValue(SerializationNameProperty, value);
		}
		public static readonly DependencyProperty SerializationNameProperty =
			DependencyProperty.RegisterAttached("SerializationName", typeof(string), typeof(BarManagerCustomizationHelper), new PropertyMetadata(null));
		public GlyphSize ToolbarGlyphSize {
			get { return (GlyphSize)GetValue(ToolbarGlyphSizeProperty); }
			set { SetValue(ToolbarGlyphSizeProperty, value); }
		}
		public GlyphSize MenuGlyphSize {
			get { return (GlyphSize)GetValue(MenuGlyphSizeProperty); }
			set { SetValue(MenuGlyphSizeProperty, value); }
		}
		public bool ShowScreenTips {
			get { return (bool)GetValue(ShowScreenTipsProperty); }
			set { SetValue(ShowScreenTipsProperty, value); }
		}
		public bool ShowShortcutInScreenTips {
			get { return (bool)GetValue(ShowShortcutInScreenTipsProperty); }
			set { SetValue(ShowShortcutInScreenTipsProperty, value); }
		}		
		public static GlyphSize GetMenuGlyphSize(DependencyObject obj) {
			var el = TreeHelper.GetParent(obj, x => (GlyphSize)x.GetValue(BarManager.MenuGlyphSizeProperty) != GlyphSize.Default);
			if (el == null)
				return GlyphSize.Default;
			return (GlyphSize)el.GetValue(BarManager.MenuGlyphSizeProperty);
		}
		private static readonly object customizationModeChanged = new object();
		protected internal static readonly DependencyPropertyKey IsCustomizationModePropertyKey;
		public static readonly DependencyProperty IsCustomizationModeProperty;
		static BarManagerCustomizationHelper() {
			sNamePrefix = Guid.NewGuid().ToString("N");
			ToolbarGlyphSizeProperty = DependencyPropertyManager.Register("ToolbarGlyphSize", typeof(GlyphSize), typeof(BarManagerCustomizationHelper), new FrameworkPropertyMetadata(GlyphSize.Default, new PropertyChangedCallback((d, e) => ((BarManagerCustomizationHelper)d).OnToolbarGlyphSizeChanged((GlyphSize)e.OldValue))));
			MenuGlyphSizeProperty = DependencyPropertyManager.Register("MenuGlyphSize", typeof(GlyphSize), typeof(BarManagerCustomizationHelper), new FrameworkPropertyMetadata(GlyphSize.Default, new PropertyChangedCallback((d, e) => ((BarManagerCustomizationHelper)d).OnMenuGlyphSizeChanged((GlyphSize)e.OldValue))));
			ShowScreenTipsProperty = DependencyPropertyManager.Register("ShowScreenTips", typeof(bool), typeof(BarManagerCustomizationHelper), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => ((BarManagerCustomizationHelper)d).OnShowScreenTipsChanged((bool)e.OldValue))));
			ShowShortcutInScreenTipsProperty = DependencyPropertyManager.Register("ShowShortcutInScreenTips", typeof(bool), typeof(BarManagerCustomizationHelper), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => ((BarManagerCustomizationHelper)d).OnShowShortcutInScreenTipsChanged((bool)e.OldValue))));
			IsCustomizationModePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCustomizationMode", typeof(bool), typeof(BarManagerCustomizationHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnIsCustomizationModePropertyChanged), new CoerceValueCallback(OnIsCustomizationModeCoerce)));
			IsCustomizationModeProperty = IsCustomizationModePropertyKey.DependencyProperty;			
		}
		public void HideItemCustomizationMenu(object menuTarget) { HideMenuCore(menuTarget, ItemCustomizationMenu, HideItemCustomizationMenu); }
		public void HideCustomizationMenu(object menuTarget) { HideMenuCore(menuTarget, CustomizationMenu, HideCustomizationMenu); }
		public void HideToolbarsCustomizationMenu(object menuTarget) { HideMenuCore(menuTarget, ToolbarsCustomizationMenu, HideToolbarsCustomizationMenu); }
		void HideMenuCore(object target, PopupMenu menu, Action hideMenuAction) {
			if (menu == null || !menu.IsOpen)
				return;
			if (target == null)
				hideMenuAction();
			if (MenuTarget == target)
				hideMenuAction();
		}
		protected static void OnIsCustomizationModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarManagerCustomizationHelper)d).OnIsCustomizationModeChanged(e);
		}
		protected static object OnIsCustomizationModeCoerce(DependencyObject d, object baseValue) {
			return baseValue;
		}
		#endregion
		EventHandlerList events;
		public BarManagerCustomizationHelper() {
			this.events = new EventHandlerList();
			this.Strategy = new CustomizationStrategy(this);
		}
		protected virtual void OnToolbarGlyphSizeChanged(GlyphSize oldValue) {
			if (!IsCustomizationMode)
				return;
			Strategy.OnToolbarGlyphSizeChanged(ToolbarGlyphSize, oldValue);
			foreach (Bar bar in GetBars()) {
				if (bar.DockInfo.BarControl == null) continue;
				for (int i = 0; i < bar.DockInfo.BarControl.Items.Count; i++) {
					BarItemLinkControl lc = bar.DockInfo.BarControl.GetLinkControl(i) as BarItemLinkControl;
					if (lc != null)
						lc.UpdateActualGlyph();
				}
				bar.DockInfo.BarControl.CalculateMaxGlyphSize();
			}
		}
		protected virtual void OnMenuGlyphSizeChanged(GlyphSize oldValue) {
			if (!IsCustomizationMode)
				return;
			Strategy.OnMenuGlyphSizeChanged(MenuGlyphSize, oldValue);
			foreach (BarItem item in GetItems()) {
				item.ExecuteActionOnLinkControls(x => x.UpdateActualGlyph());
			}
		}
		protected virtual void OnShowScreenTipsChanged(bool oldValue) {
			Strategy.OnShowScreenTipsChanged(ShowScreenTips, oldValue);
		}
		protected virtual void OnShowShortcutInScreenTipsChanged(bool oldValue) {
			Strategy.OnShowShortcutInScreenTipsChanged(ShowShortcutInScreenTips, oldValue);
		}
		protected virtual void OnScopeChanged(BarNameScope oldValue) {
			UnsubscribeManager(oldValue.With(x => x.Target) as BarManager);
			SubscribeManager(Scope.With(x => x.Target) as BarManager);
			while (children.Count > 0)
				RemoveLogicalChild(children[0]);
			UITarget = Scope.With(x=>x.Target as UIElement);
		}
		protected void SubscribeManager(BarManager barManager) {
			if (barManager == null)
				return;
			BindingOperations.SetBinding(this, ToolbarGlyphSizeProperty, new Binding() { Path = new PropertyPath(BarManager.ToolbarGlyphSizeProperty), Mode = BindingMode.TwoWay, Source = barManager });
			BindingOperations.SetBinding(this, MenuGlyphSizeProperty, new Binding() { Path = new PropertyPath(BarManager.MenuGlyphSizeProperty), Mode = BindingMode.TwoWay, Source = barManager });
			BindingOperations.SetBinding(this, ShowScreenTipsProperty, new Binding() { Path = new PropertyPath(BarManager.ShowScreenTipsProperty), Mode = BindingMode.TwoWay, Source = barManager });
			BindingOperations.SetBinding(this, ShowShortcutInScreenTipsProperty, new Binding() { Path = new PropertyPath(BarManager.ShowShortcutInScreenTipsProperty), Mode = BindingMode.TwoWay, Source = barManager });			
		}
		protected void UnsubscribeManager(BarManager barManager) {
			if (barManager == null)
				return;
			ClearValue(ToolbarGlyphSizeProperty);
			ClearValue(MenuGlyphSizeProperty);
			ClearValue(ShowScreenTipsProperty);
			ClearValue(ShowShortcutInScreenTipsProperty);
		}
		protected EventHandlerList Events { get { return events; } }
		protected internal BarDragElementPopup DragPopup { get { return BarDragElementPopup.currentDragPopup; } }
		PopupMenu toolbarCustomizationMenu;
		protected internal PopupMenu ToolbarsCustomizationMenu {
			get {
				if(toolbarCustomizationMenu == null)
					toolbarCustomizationMenu = new PopupMenu() { GlyphSize = GlyphSize.Small };
				return toolbarCustomizationMenu;
			}
		}		   
		public bool ShowToolbarsCustomizationMenu(UIElement element) {
			if(ToolbarsCustomizationMenu.IsOpen) CloseCustomizationForm();
			if(IsCustomizationMode || BarManager.GetDXContextMenu(element) != null) return false;
			PopupMenuManager.CloseAllPopups();
			this.toolbarCustomizationMenu = null;
			MenuTarget = element;
			var toolbarCustomizationItem = new ToolbarListItem();
			toolbarCustomizationItem.ListItemType = ToolbarListItemType.ShowBars;
			toolbarCustomizationItem.SelectedToolbar = LayoutHelper.FindParentObject<BarControl>(element).With(x => x.Bar);
			ToolbarsCustomizationMenu.Items.Clear();
			ToolbarsCustomizationMenu.Items.Add(toolbarCustomizationItem);
			ToolbarsCustomizationMenu.Closed += new EventHandler(OnToolbarsCustomizationMenuPopupClosed);
			BarManager.SetDXContextMenu(element, ToolbarsCustomizationMenu);
			return ToolbarsCustomizationMenu.IsOpen;
		}
		void OnToolbarsCustomizationMenuPopupClosed(object sender, EventArgs e) {
			ToolbarsCustomizationMenu.Closed -= new EventHandler(OnToolbarsCustomizationMenuPopupClosed);
			if(MenuTarget == null) return;
			BarManager.SetDXContextMenu(MenuTarget, null);
		}
		protected UIElement MenuTarget { get; set; }
		public bool ShowCustomizationMenu(Bar source, UIElement target) {
			if (source == null)
				return false;
			HideCustomizationMenu();
			this.customizationMenu = null;
			MenuTarget = target;
			InitializeCustomizationMenu(source);
			if (target != null) {
				CustomizationMenu.Placement = PlacementMode.Bottom;
				CustomizationMenu.ShowPopup(target);				
			} else {
				CustomizationMenu.Placement = PlacementMode.Mouse;
				CustomizationMenu.IsOpen = true;
			}
			return CustomizationMenu.IsOpen;
		}
		public void HideCustomizationMenu() {
			if(customizationMenu != null) {
				CustomizationMenu.ClosePopup();
				cmm.PerformForce();
			}				
		}
		public void HideCustomizationMenus() {
			HideCustomizationMenu();
			HideToolbarsCustomizationMenu();
			HideItemCustomizationMenu();
		}
		public void HideToolbarsCustomizationMenu() {
			if(toolbarCustomizationMenu != null)
				ToolbarsCustomizationMenu.ClosePopup();
		}
		public void HideItemCustomizationMenu() {
			if(itemCustomizationMenu != null)
				ItemCustomizationMenu.ClosePopup();
		}
		PopupMenu customizationMenu;
		protected virtual void CreateAddOrRemoveButtonsMenu() {
			BarControl bc = LayoutHelper.FindParentObject<BarControl>(MenuTarget);
			if (bc != null && !bc.ActualShowQuickCustomizationButtonCore)
				return;
			BarSubItem item = new BarSubItem();
			item.IsPrivate = true;
			item.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_AddOrRemoveItemCaption);
			BarSubItemLink subLink = item.CreateLink(true) as BarSubItemLink;
			CustomizationMenu.ItemLinks.Add(subLink);
			var toolbarCustomizationItem = new ToolbarListItem();
			toolbarCustomizationItem.AllowUpdateItem = false;
			toolbarCustomizationItem.ListItemType = ToolbarListItemType.ShowBarsAndItems;
			toolbarCustomizationItem.SelectedToolbar = bc != null ? bc.Bar : null;
			toolbarCustomizationItem.AllowUpdateItem = true;
			item.Items.Add(toolbarCustomizationItem);
		}
		protected virtual void CreateCollapsedItems(Bar source) {
			var list = new List<BarItemLinkBase>();
			FillCollapsedItemsList(list, new List<ILinksHolder>(), source);
			foreach (var link in list)
				CustomizationMenu.ItemLinks.Add(link);
		}
		void FillCollapsedItemsList(List<BarItemLinkBase> list, List<ILinksHolder> enteredElements, ILinksHolder holder) {
			enteredElements.Add(holder);
			foreach (BarItemLink link in holder.ActualLinks) {
				var item = (link as BarItemLink).With(x => x.Item) as BarLinkContainerItem;
				if (item != null) {
					FillCollapsedItemsList(list, enteredElements, item);
					continue;
				}					
				if (IsCollapsedItemLink(link)) {
					BarItemLinkBase itemLink = (BarItemLinkBase)((ICloneable)link).Clone();
					CustomizationMenu.ItemLinks.Add(itemLink);
				}
			}			
		}
		protected internal virtual bool IsCollapsedItemLink(BarItemLink link) {
			if(link == null || link.LinkControl == null) return false;
			return (link.LinkControl.Visibility == Visibility.Collapsed || link.LinkControl.LinkInfo.IsHidden()) && link.ActualIsVisible;
		}
		protected virtual void InitializeCustomizationMenu(Bar source) {
			CustomizationMenu.ItemLinks.Clear();
			CreateCollapsedItems(source);
			if (!source.ActualAllowCustomizationMenu)
				return;
			CreateAddOrRemoveButtonsMenu();
		}
		protected internal PopupMenu CustomizationMenu {
			get {
				if(customizationMenu == null) {
					customizationMenu = new PopupMenu() { };
					customizationMenu.Opened += OnCustomizationMenuOpened;
					customizationMenu.IsOpenChanged += new EventHandler((d, e) => { if (!((PopupMenu)d).IsOpen) OnCustomizationMenuClosed(d, e); });
				}
				return customizationMenu;
			}
		}
		protected virtual void OnCustomizationMenuOpened(object sender, EventArgs e) {
			NavigationTree.SelectElement(((PopupMenu)sender).ContentControl);
		}
		PostponedAction cmm = new PostponedAction(() => true);
		protected virtual void OnCustomizationMenuClosed(object sender, EventArgs e) {
			cmm.PerformPostpone(OnCustomizationMenuClosedAsync);
			Dispatcher.BeginInvoke(new Action(cmm.PerformForce));
		}
		void OnCustomizationMenuClosedAsync() {
			var pt = CustomizationMenu.PlacementTarget;
			if (pt is BarQuickCustomizationButton)
				((BarQuickCustomizationButton)pt).IsChecked = false;
			CustomizationMenu.PlacementTarget = null;
		}
		protected internal FloatingContainer CustomizationForm { get; set; }
		public CustomizationControl CustomizationControl {
			get { return customizationControl ?? (currentCustomizationControl ?? (currentCustomizationControl = CreateDefaultCustomizationControl())); }
			set { customizationControl = value; }
		}
		CustomizationControl customizationControl = null;
		CustomizationControl currentCustomizationControl = null;
		protected virtual CustomizationControl CreateDefaultCustomizationControl() {
			return new CustomizationControl(this);
		}
		public void ShowCustomizationForm() {
			if (AllowCustomization)
				return;
			FocusObserver.Reset();
			CustomizationForm = FloatingContainerFactory.Create(FloatingMode.Window);
			CustomizationForm.BeginUpdate();			
			CustomizationForm.UseActiveStateOnly = true;
			CustomizationForm.Caption = BarsLocalizer.GetString(BarsStringId.CustomizationForm_Caption);
			Window win = Scope.Target.With(Window.GetWindow);
			CustomizationForm.Owner = Scope.Target as FrameworkElement;
			AddLogicalChild(CustomizationForm);			
			IsOwnerWindowActive = win != null && win.IsActive;
			CustomizationForm.AllowSizing = true;
			CustomizationForm.Content = CustomizationControl;
			CustomizationForm.CloseOnEscape = true;
			CustomizationForm.Hidden += OnCustomizationFormHidden;
			BarManager.SkipFloatingBarHiding = true;
			CustomizationForm.ContainerStartupLocation = WindowStartupLocation.CenterOwner;
			CustomizationForm.FloatSize = ValuesProvider.Return(x => x.CustomizationFormFloatSize, () => new Size(450, 400));
			CustomizationForm.MinHeight = ValuesProvider.Return(x => x.CustomizationFormMinHeight, () => 300);
			CustomizationForm.MinWidth = ValuesProvider.Return(x => x.CustomizationFormMinWidth, () => 300);
			CustomizationForm.EndUpdate();
			initializedCategories = InitializeCategories();
			CustomizationForm.IsOpen = true;			
			IsCustomizationMode = true;
		}
		public bool IsCustomizationFormVisible { 
			get { 
				return CustomizationForm != null && CustomizationForm.IsOpen; 
			} 
		}
		bool IsOwnerWindowActive { get; set; }
		public void OnCustomizationFormHidden(object sender, RoutedEventArgs e) {
			IsCustomizationMode = false;
			CustomizationForm.Hidden -= OnCustomizationFormHidden;
			if(!CustomizationFormClosing)
				CustomizationForm.Close();
			RemoveLogicalChild(CustomizationForm);			
			CustomizationForm = null;
			if(!BrowserInteropHelper.IsBrowserHosted)
				CheckActivateOwnerWindow();
			currentCustomizationControl = null;
		}
		protected virtual void CheckActivateOwnerWindow() {
			Window win = Scope.Target.With(Window.GetWindow);
			if(win != null && IsOwnerWindowActive)
				win.Activate();
		}
		public event EventHandler IsCustomizationModeChanged {
			add { Events.AddHandler(customizationModeChanged, value); }
			remove { Events.RemoveHandler(customizationModeChanged, value); }
		}
		protected virtual void RaiseIsCustomizationModeChanged() {
			EventHandler h = Events[customizationModeChanged] as EventHandler;
			if(h != null) h(this, EventArgs.Empty);
		}
		[ThreadStatic]
		static Locker customizationModeLocker;
		static Locker CustomizationModeLocker {
			get { return customizationModeLocker ?? (customizationModeLocker = new Locker()); }
		}
		protected virtual void OnIsCustomizationModeChanged(DependencyPropertyChangedEventArgs e) {
			if (IsCustomizationMode)
				CustomizationModeLocker.Lock();
			else
				CustomizationModeLocker.Unlock();
			if (!IsCustomizationMode)
				UninitializeCategories();
			if (SelectedLink != null) {
				foreach(BarItemLinkInfo linkInfo in SelectedLink.LinkInfos) {
					BarItemLinkControl lc = linkInfo.LinkControl as BarItemLinkControl;
					if(lc != null) {
						lc.ShowCustomizationBorder = false;
						lc.UpdateIsEnabled();
					}
				}
			}
			foreach(BarItem item in GetItems()) {
				foreach(BarItemLinkBase link in item.Links) {
					link.UpdateActualIsVisible();
				}
				item.ExecuteActionOnBaseLinkControls((lc) => lc.OnCustomizationModeChanged());
			}
			HideOpenedCustomizedPopups();
			BarManager.SkipFloatingBarHiding = false;
			RaiseIsCustomizationModeChanged();
			SelectedLinkControl = null;			
		}		
		protected virtual void HideOpenedCustomizedPopups() {
			PopupMenuManager.CloseAllPopups();
		}			   
		public bool IsCustomizationMode {
			get { return (bool)GetValue(IsCustomizationModeProperty); }
			protected internal set { this.SetValue(IsCustomizationModePropertyKey, value); }
		}
		public bool IsPreparedToQuickCustomizationMode { get; set; }
		PopupMenu itemCustomizationMenu;
		protected internal PopupMenu ItemCustomizationMenu {
			get {
				if(itemCustomizationMenu == null) {
					itemCustomizationMenu = new PopupMenu() { GlyphSize = GlyphSize.Small };
					itemCustomizationMenu.ItemClickBehaviour = PopupItemClickBehaviour.None;
					itemCustomizationMenu.Closed += OnItemCustomizationMenuClosed;
				}
				return itemCustomizationMenu;
			}
		}		
		protected virtual void InitializeItemCustomizationMenu() {
			ItemCustomizationMenu.ItemLinks.Clear();
			ItemCustomizationMenu.ItemLinks.Add(CreateItemResetButton());
			ItemCustomizationMenu.ItemLinks.Add(CreateItemDeleteButton());
			ItemCustomizationMenu.ItemLinks.Add(new BarItemLinkSeparator());
			ItemCustomizationMenu.ItemLinks.Add(CreateLinkCaptionEditor());
			ItemCustomizationMenu.ItemLinks.Add(new BarItemLinkSeparator());
			ItemCustomizationMenu.ItemLinks.Add(CreateAlignmentEditor());
			ItemCustomizationMenu.ItemLinks.Add(CreateDisplayModeEditor());
			ItemCustomizationMenu.ItemLinks.Add(CreateGlyphAlignmentEditor());
			ItemCustomizationMenu.ItemLinks.Add(CreateGlyphSizeEditor());
			ItemCustomizationMenu.ItemLinks.Add(new BarItemLinkSeparator());
			ItemCustomizationMenu.ItemLinks.Add(CreateItemVisibleCheck());
			ItemCustomizationMenu.ItemLinks.Add(CreateItemBeginGroupCheck());
		}
		BarCheckItemLink CreateItemVisibleCheck() {
			BarCheckItem visibleItem = new BarCheckItem();
			visibleItem.IsPrivate = true;
			visibleItem.IsThreeState = false;
			visibleItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_VisibleItemCaption);
			visibleItem.IsChecked = SelectedLink.IsVisible;
			visibleItem.CheckedChanged += new ItemClickEventHandler(OnVisibleCheckChanged);
			return (BarCheckItemLink)visibleItem.CreateLink(true);
		}
		BarCheckItemLink CreateItemBeginGroupCheck() {
			BarCheckItem beginGroupItem = new BarCheckItem();
			beginGroupItem.IsPrivate = true;
			beginGroupItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_BeginGroupItemCaption);
			int linkIndex = SelectedLink.Links.IndexOf(SelectedLink);
			if(linkIndex > 0 && SelectedLink.Links[linkIndex - 1] is BarItemLinkSeparator)
				beginGroupItem.IsChecked = true;
			else
				beginGroupItem.IsChecked = false;
			beginGroupItem.CheckedChanged += new ItemClickEventHandler(OnBeginGroupCheckChanged);
			return (BarCheckItemLink)beginGroupItem.CreateLink(true);
		}
		BarButtonItemLink CreateItemDeleteButton() {
			BarButtonItem deleteItem = new BarButtonItem();
			deleteItem.IsPrivate = true;
			deleteItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_DeleteItemCaption);
			deleteItem.ItemClick += new ItemClickEventHandler(OnItemDeleteItemClick);
			return (BarButtonItemLink)deleteItem.CreateLink(true);
		}
		BarButtonItemLink CreateItemResetButton() {
			BarButtonItem resetItem = new BarButtonItem();
			resetItem.IsPrivate = true;
			resetItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_ResetItemCaption);
			resetItem.ItemClick += new ItemClickEventHandler(OnItemResetItemClick);
			return (BarButtonItemLink)resetItem.CreateLink(true);
		}
		BarEditItemLink CreateLinkCaptionEditor() {
			BarEditItem editItem = new BarEditItem() { EditHorizontalAlignment = HorizontalAlignment.Right };
			editItem.IsPrivate = true;
			editItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_LinkItemCaption);
			editItem.EditWidth = 100;
			editItem.EditSettings = new TextEditSettings();
			editItem.EditValueChanged += new RoutedEventHandler(OnItemCaptionEditValueChanged);
			if(SelectedLink.ActualContent is string)
				editItem.EditValue = (string)SelectedLink.ActualContent;
			else
				editItem.IsEnabled = false;
			return (BarEditItemLink)editItem.CreateLink(true);
		}
		BarEditItemLink CreateDisplayModeEditor() {
			BarEditItem editItem = new BarEditItem() { EditHorizontalAlignment = HorizontalAlignment.Right };
			editItem.IsPrivate = true;
			editItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_DisplayModeItemCaption);
			editItem.EditWidth = 100;
			ComboBoxEditSettings cb = new ComboBoxEditSettings();
			cb.IsTextEditable = false;
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(BarItemDisplayMode.Default));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(BarItemDisplayMode.Content));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(BarItemDisplayMode.ContentAndGlyph));
			editItem.EditSettings = cb;
			editItem.EditValueChanged += new RoutedEventHandler(OnItemDisplayModeChanged);
			editItem.EditValue = BarsLocalizer.GetStringFromEnumItem(SelectedLink.BarItemDisplayMode);
			return (BarEditItemLink)editItem.CreateLink(true);
		}
		BarEditItemLink CreateGlyphAlignmentEditor() {
			BarEditItem editItem = new BarEditItem() { EditHorizontalAlignment = HorizontalAlignment.Right };
			editItem.IsPrivate = true;
			editItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_GlyphAlignmentItemCaption);
			editItem.EditWidth = 100;
			ComboBoxEditSettings cb = new ComboBoxEditSettings();
			cb.IsTextEditable = false;
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(Dock.Left));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(Dock.Top));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(Dock.Right));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(Dock.Bottom));
			editItem.EditSettings = cb;
			editItem.EditValueChanged += new RoutedEventHandler(OnItemGlyphAlignmentChanged);
			editItem.EditValue = SelectedLinkControl.ActualGlyphAlignment;
			return (BarEditItemLink)editItem.CreateLink(true);
		}
		BarEditItemLink CreateAlignmentEditor() {
			BarEditItem editItem = new BarEditItem() { EditHorizontalAlignment = HorizontalAlignment.Right };
			editItem.IsPrivate = true;
			editItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_AlignmentItemCaption);
			editItem.EditWidth = 100;
			ComboBoxEditSettings cb = new ComboBoxEditSettings();
			cb.IsTextEditable = false;
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(BarItemAlignment.Default));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(BarItemAlignment.Near));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(BarItemAlignment.Far));
			editItem.EditSettings = cb;
			editItem.EditValueChanged += new RoutedEventHandler(OnItemAlignmentChanged);
			editItem.EditValue = BarsLocalizer.GetStringFromEnumItem(SelectedLinkControl.Link.Alignment);
			return (BarEditItemLink)editItem.CreateLink(true);
		}
		BarEditItemLink CreateGlyphSizeEditor() {
			BarEditItem editItem = new BarEditItem() { EditHorizontalAlignment = HorizontalAlignment.Right };
			editItem.IsPrivate = true;
			editItem.Content = BarsLocalizer.GetString(BarsStringId.CustomizationMenu_GlyphSizeItemCaption);			
			editItem.EditWidth = 100;
			ComboBoxEditSettings cb = new ComboBoxEditSettings();
			cb.IsTextEditable = false;
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(GlyphSize.Default));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(GlyphSize.Small));
			cb.Items.Add(BarsLocalizer.GetStringFromEnumItem(GlyphSize.Large));
			editItem.EditSettings = cb;
			editItem.EditValueChanged += new RoutedEventHandler(OnItemGlyphSizeChanged);
			editItem.EditValue = BarsLocalizer.GetStringFromEnumItem(SelectedLink.UserGlyphSize);
			return (BarEditItemLink)editItem.CreateLink(true);
		}
		protected virtual void OnBeginGroupCheckChanged(object sender, ItemClickEventArgs e) {
			if (SelectedLink == null)
				return;
			Strategy.OnChangeLinkBeginGroup(SelectedLink, (bool)((BarCheckItem)e.Item).IsChecked);			
		}
		protected virtual void OnVisibleCheckChanged(object sender, ItemClickEventArgs e) {
			if (SelectedLink == null)
				return;
			Strategy.OnChangeLinkIsVisible(SelectedLink, ((BarCheckItem)e.Item).IsChecked.HasValue ? ((BarCheckItem)e.Item).IsChecked.Value : false);			
		}
		protected virtual void OnItemGlyphSizeChanged(object sender, EventArgs e) {
			if (SelectedLink == null)
				return;
			int index = GetSelectedItemIndex(sender as BarEditItem);
			if(index == -1)
				return;			
			Strategy.OnChangeLinkGlyphSize(SelectedLink, BarsLocalizer.CreateEnumItemFromIndex<GlyphSize>(index));
		}
		protected virtual void OnItemGlyphAlignmentChanged(object sender, EventArgs e) {
			if (SelectedLink == null)
				return;
			int index = GetSelectedItemIndex(sender as BarEditItem);
			if(index == -1)
				return;
			Strategy.OnChangeLinkGlyphAlignment(SelectedLink, BarsLocalizer.CreateEnumItemFromIndex<Dock>(index));
		}
		protected virtual void OnItemAlignmentChanged(object sender, EventArgs e) {
			if (SelectedLink == null)
				return;
			int index = GetSelectedItemIndex(sender as BarEditItem);
			if(index == -1)
				return;
			Strategy.OnChangeLinkAlignment(SelectedLink, BarsLocalizer.CreateEnumItemFromIndex<BarItemAlignment>(index));			
		}
		protected virtual void OnItemCaptionEditValueChanged(object sender, EventArgs e) {
			if (SelectedLink == null)
				return;
			if (((BarEditItem)sender).EditValue == null) return;
			Strategy.OnRenameLink(SelectedLink, (string)((BarEditItem)sender).EditValue);			
		}
		protected int GetSelectedItemIndex(BarEditItem editor) {
			if(editor == null || editor.EditValue == null)
				return -1;
			ComboBoxEditSettings settings = editor.EditSettings as ComboBoxEditSettings;
			if(settings == null)
				return -1;
			return settings.Items.IndexOf(editor.EditValue);
		}
		protected virtual void OnItemDisplayModeChanged(object sender, EventArgs e) {
			if (SelectedLink == null)
				return;
			int index = GetSelectedItemIndex(sender as BarEditItem);
			if(index == -1) return;
			Strategy.OnChangeLinkDisplayMode(SelectedLink, BarsLocalizer.CreateEnumItemFromIndex<BarItemDisplayMode>(index));			
		}
		protected void OnItemDeleteItemClick(object sender, ItemClickEventArgs e) {
			if (SelectedLink == null)
				return;
			Strategy.OnRemoveLink(SelectedLink);
		}
		protected void OnItemResetItemClick(object sender, ItemClickEventArgs e) {
			if (SelectedLink == null)
				return;
			Strategy.OnResetLink(SelectedLink);
		}
		protected internal BarItemLink SelectedLink { get { return SelectedLinkControl == null ? null : SelectedLinkControl.Link; } }
		private BarItemLinkControl selectedLinkControl;
		public BarItemLinkControl SelectedLinkControl {
			get { return selectedLinkControl; }
			set {
				BarItemLinkControl oldValue = selectedLinkControl;
				selectedLinkControl = value;
				if(oldValue!=value)
					OnSelectedLinkControlChanged(oldValue);
			}
		}
		protected virtual void OnSelectedLinkControlChanged(BarItemLinkControl oldValue) {
			BarItemLink oldLink = oldValue == null ? null : oldValue.Link;
			if(oldLink != null) {
				foreach(BarItemLinkInfo linkInfo in oldLink.LinkInfos) {
					BarItemLinkControl lc = linkInfo.LinkControl as BarItemLinkControl;
					if(lc != null) {
						lc.ShowCustomizationBorder = false;
						lc.UpdateIsEnabled();
					}
				}
			}
			if(SelectedLinkControl != null)
				SelectedLinkControl.ShowCustomizationBorder = SelectedLinkControl.Link == null ? false : !SelectedLinkControl.Link.IsPrivate;
		}
		public bool ShowItemCustomizationMenu(BarItemLinkControl linkControl) {
			if (!IsCustomizationMode)
				return false;
			if(ItemCustomizationMenu != null) {
				ItemCustomizationMenu.ClosePopup();
			}
			var link = linkControl.Link;
			if(link != null && !link.AllowShowCustomizationMenu)
				return false;
			this.itemCustomizationMenu = null;
			MenuTarget = linkControl;
			SelectedLinkControl = linkControl;
			if(link == null) return false;
			InitializeItemCustomizationMenu();
			ItemCustomizationMenu.Placement = PlacementMode.Bottom;
			ItemCustomizationMenu.ShowPopup(linkControl);
			return ItemCustomizationMenu.IsOpen;
		}
		protected virtual void OnItemCustomizationMenuClosed(object sender, EventArgs e) {
			if (ItemCustomizationMenu.IsOpen)
				return;
			SelectedLinkControl = null;
		}		
		bool CustomizationFormClosing { get; set; }
		public void CloseCustomizationForm() {
			if(CustomizationForm != null) {
				CustomizationFormClosing = true;
				CustomizationForm.Close();
				CustomizationFormClosing = false;
			}
			CustomizationForm = null;
		}
		public static bool IsInCustomizationMode(object obj) {
			return CustomizationModeLocker.IsLocked && BarNameScope.GetService<ICustomizationService>(obj).IsCustomizationMode;
		}
		public static bool IsInCustomizationMenu(BarItemLinkControlBase lc) {
			if (lc == null)
				return false;
			var popup = BarManagerHelper.GetPopup(lc);
			if (popup == null)
				return false;
			var ch = BarNameScope.GetService<ICustomizationService>(lc).CustomizationHelper;
			if (ch == null)
				return false;
			return ch.IsCustomizationMenu(popup);
		}
		protected internal virtual bool IsCustomizationMenu(BarPopupBase barPopupBase) {
			if (barPopupBase == null)
				return false;
			if (MenuIsCustomizationMenu(barPopupBase)) return true;
			return IsCustomizationMenu(PopupMenuManager.GetParentPopup(barPopupBase));
		}
		bool MenuIsCustomizationMenu(BarPopupBase menu) {
			return menu == toolbarCustomizationMenu || menu == itemCustomizationMenu || menu == customizationMenu;
		}
		public bool AllowCustomization { get; set; }
		BarNameScope scope;
		IEnumerable<BarManagerCategory> initializedCategories;
		CustomizationStrategy strategy;
		protected internal CustomizationStrategy Strategy {
			get { return strategy; }
			set { strategy = value; }
		}
		protected UIElement UITarget { get; private set; }
		public BarNameScope Scope {
			get { return scope; }
			set {
				if (value == scope) return;
				BarNameScope oldValue = scope;
				scope = value;
				OnScopeChanged(oldValue);
			}
		}		
		public virtual IEnumerable<BarItem> GetItems() {
			if (Scope == null)
				return Enumerable.Empty<BarItem>();
			return Scope.GetService<IElementRegistratorService>().GetElements<BarItem>(ScopeSearchSettings.Local);
		}
		public virtual IEnumerable<Bar> GetBars() {
			if (Scope == null)
				return Enumerable.Empty<Bar>();
			return Scope.GetService<IElementRegistratorService>().GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local).OfType<Bar>().Where(x => !x.IsRemoved && !x.IsPrivate);
		}
		public IEnumerable<BarManagerCategory> GetCategories() {
			return initializedCategories;
		}
		protected virtual void UninitializeCategories() {
			if (initializedCategories == null)
				return;
			foreach (var category in initializedCategories) {
				RemoveLogicalChild(category);
			}
			initializedCategories = Enumerable.Empty<BarManagerCategory>();
		}		
		protected virtual IEnumerable<BarManagerCategory> InitializeCategories() {
			if (Scope == null)
				return Enumerable.Empty<BarManagerCategory>();
			var manager = Scope.Target as BarManager;
			List<BarManagerCategory> result = new List<BarManagerCategory>();
			if (manager != null) {
				result.Add(manager.Categories.AllItems);
				result.Add(manager.Categories.UnassignedItems);				
				foreach (BarManagerCategory cat in manager.Categories) {
					result.Add(cat);
				}
				return result;
			}
			var items = Scope.GetService<IElementRegistratorService>().GetElements<BarItem>();
			var definedCategories = items.Select(x => x.Category).Where(MayBe.ReturnSuccess);
			var undefinedCategoryNames = items.Select(x => x.CategoryName).Where(x => !String.IsNullOrEmpty(x)).Except(definedCategories.Select(x => x.Name));
			var undefinedCategories = undefinedCategoryNames.Select(x => new BarManagerCategory() { Caption = x, Name = x });			
			result = definedCategories.Concat(undefinedCategories).OrderBy(x=>x.Caption).ToList();
			result.Insert(0, BarManagerCategoryCollection.CreateUnassignedItemsCategory(null));
			result.Insert(0, BarManagerCategoryCollection.CreateAllItemsCategory(null));			
			foreach (var category in result) {
				AddLogicalChild(category);
			}
			return result;
		}
		List<DependencyObject> children = new List<DependencyObject>();
		protected internal void RemoveLogicalChild(DependencyObject obj) {
			if (UITarget == null)
				return;
			if (children.Remove(obj)) {
				if (obj is BarManagerCategory && ((BarManagerCategory)obj).ScopeNode == UITarget) {
					((BarManagerCategory)obj).ScopeNode = null;
					return;
				}
				LogicalTreeWrapper.SetRoot(obj, null);
			}				
		}
		protected internal void AddLogicalChild(DependencyObject obj) {
			if (UITarget == null)
				return;
			if (children.Contains(obj))
				return;
			if (LogicalTreeHelper.GetParent(obj) != null)
				return;
			children.Add(obj);
			if(obj is BarManagerCategory && ((BarManagerCategory)obj).ScopeNode == null) {
				((BarManagerCategory)obj).ScopeNode = UITarget;
				return;
			}
			LogicalTreeWrapper.SetRoot(obj, UITarget);
		}
		protected BarManagerThemeDependentValuesProvider ValuesProvider {
			get { return Scope.Target.With(BarManager.GetBarManager).With(x => x.ValuesProvider); }
		}
		#region IRuntimeCustomizationHost Members
		DependencyObject IRuntimeCustomizationHost.FindTarget(string targetName) {
			return Scope.GetService<IElementRegistratorService>().GetElements<IFrameworkInputElement>(targetName, ScopeSearchSettings.Local).FirstOrDefault() as DependencyObject;
		}
		RuntimeCustomizationCollection runtimeCustomizations;
		RuntimeCustomizationCollection IRuntimeCustomizationHost.RuntimeCustomizations { get { return runtimeCustomizations ?? (runtimeCustomizations = new RuntimeCustomizationCollection(this)); } }
		#endregion
	}
	public delegate void RemoveBarEventHandler(object sender, RemoveBarEventArgs args);
	public abstract class CollectionModificationEventArgs : CancelRoutedEventArgs {
		public object GetSource(object element) {
			var dObj = element as DependencyObject;
			if (dObj == null)
				return null;
			return ItemsAttachedBehaviorProperties.GetSource(dObj);
		}
	}
	public class RemoveBarEventArgs : CollectionModificationEventArgs {
		public BarManager Manager { get; private set; }
		public Bar Bar { get; private set; }
		public RemoveBarEventArgs(BarManager manager, Bar bar) {
			Manager = manager;
			Bar = bar;
			RoutedEvent = CustomizationEvents.RemoveBarEvent;
		}
	}
	public delegate void CreateNewBarEventHandler(object sender, CreateNewBarEventArgs args);
	public class CreateNewBarEventArgs : CancelRoutedEventArgs {
		public BarManager Manager { get; private set; }
		public string Caption { get; set; }
		public CreateNewBarEventArgs(BarManager manager, string caption) {			
			Manager = manager;
			Caption = caption;
			RoutedEvent = CustomizationEvents.CreateNewBarEvent;
		}
	} 
	public delegate void BeginGroupChangedEventHandler(object sender, BeginGroupChangedEventArgs args);
	public class BeginGroupChangedEventArgs : CancelRoutedEventArgs {
		public ILinksHolder Holder { get; private set; }
		public BarItemLink Link { get; private set; }
		public bool Value { get; private set; }
		public BeginGroupChangedEventArgs(BarItemLink link, ILinksHolder holder, bool newValue) {
			Link = link;
			Holder = holder;
			RoutedEvent = CustomizationEvents.BeginGroupChangedEvent;
		}
	}
	public class CustomizationEvents {
		public static readonly RoutedEvent CreateNewBarEvent = EventManager.RegisterRoutedEvent("CreateNewBar", RoutingStrategy.Bubble, typeof(CreateNewBarEventHandler), typeof(CustomizationEvents));
		public static readonly RoutedEvent RemoveBarEvent = EventManager.RegisterRoutedEvent("RemoveBar", RoutingStrategy.Bubble, typeof(RemoveBarEventHandler), typeof(CustomizationEvents));
		public static readonly RoutedEvent BeginGroupChangedEvent = EventManager.RegisterRoutedEvent("BeginGroupChanged", RoutingStrategy.Bubble, typeof(BeginGroupChangedEventHandler), typeof(CustomizationEvents));
	}	
	public class CustomizationStrategy {
		BarManagerCustomizationHelper helper;
		public static bool SynchronizeModificationWithSourceCollections { get; set; }
		protected BarManagerCustomizationHelper Helper { get { return helper; } }
		BarManager Manager { get { return Helper.Scope.With(x => x.Target).With(BarManager.GetBarManager); } }
		public CustomizationStrategy(BarManagerCustomizationHelper helper) {
			this.helper = helper;
		}
		protected void AddCustomization(RuntimeCustomization customization, bool skipApply = false) {
			var manager = Manager;
			if (manager != null) {
				manager.RuntimeCustomizations.Add(customization);
			} else {
				customization.Host = ((IRuntimeCustomizationHost)Helper);
			}
			if (!skipApply)
				customization.Apply();
		}
		#region baritem drag-n-drop
		public virtual void DragItem(IBarItem source, ILinksHolder targetHolder, BarItemLink target, DragType dragType, bool insertAfter) {
			if (dragType == DragType.Remove) {
				OnRemoveLink(source as BarItemLink);
				return;
			}
			var targetIndex = -1;
			if (target != null) {
				targetIndex = target.Links.IndexOf(target);
				if (insertAfter)
					targetIndex += 1;
			}			
			if (dragType == DragType.Copy) {
				OnCopyLink(source, targetHolder, target, targetIndex, insertAfter);
			}
			if (dragType == DragType.Move) {
				var sourcelink = source as BarItemLink;
				OnMoveLink(sourcelink, targetHolder, target, targetIndex, insertAfter);
			}
		}
		public virtual void OnCopyLink(IBarItem source, ILinksHolder holder, BarItemLink target, int index, bool insertAfter, bool skipEvent = false) {
			holder = holder ?? target.Links.Holder;
			if (!skipEvent) {
				if (holder != null && SyncInsertWithSource(
				holder.With(x => x.ItemsSource),
				source,
				index))
					return;
			}
			AddCustomization(new RuntimeCopyLinkCustomization(source, holder, target, insertAfter));
		}
		public virtual void OnMoveLink(BarItemLink source, ILinksHolder holder, BarItemLink target, int index, bool insertAfter) {
			holder = holder ?? target.With(x => x.Links).With(x => x.Holder);
			if (SyncMoveWithSources(
				source.With(x => x.Links).With(x => x.Holder).With(x => x.ItemsSource),
				holder.With(x => x.ItemsSource),
				source,
				index
				))
				return;
			OnCopyLink(source, holder, target, index, insertAfter, true);
			OnRemoveLink(source, true);
		}	   
		public virtual void OnRemoveLink(BarItemLink link, bool skipEvent = false) {
			if (link == null)
				return;
			var holder = link.Links.With(x => x.Holder);
			if (!skipEvent) {
				if (SyncRemoveWithSource(
				holder.With(x => x.ItemsSource),
				link.Links,
				link))
					return;
			}			
			if (!link.CommonBarItemCollectionLink && link.CreatedByCustomizationDialog)
				AddCustomization(new RuntimeRemoveLinkCustomization(link));
			else
				AddCustomization(new RuntimePropertyCustomization(link) {
					NewValue = true,
					PropertyName = "IsRemoved"
				});
		}
		#endregion //baritem drag-n-drop
		#region baritem customizations
		public virtual void OnResetLink(BarItemLink link) {
			link.UserContent = null;
			link.UserGlyphAlignment = null;
			link.UserGlyphSize = GlyphSize.Default;
			link.BarItemDisplayMode = BarItemDisplayMode.Default;
			link.IsVisible = link.Item.IsVisible;
		}		   
		public virtual void OnRenameLink(BarItemLink link, string newContent) {
			AddCustomization(new RuntimePropertyCustomization(link) {
				NewValue = newContent,
				PropertyName = "UserContent"
			});			
		}
		public virtual void OnChangeLinkAlignment(BarItemLink link, BarItemAlignment newAlignment) {
			AddCustomization(new RuntimePropertyCustomization(link) {
				NewValue = newAlignment,
				PropertyName = "Alignment"
			});
		}
		public virtual void OnChangeLinkDisplayMode(BarItemLink link, BarItemDisplayMode newDisplayMode) {
			AddCustomization(new RuntimePropertyCustomization(link) {
				NewValue = newDisplayMode,
				PropertyName = "BarItemDisplayMode"
			});
		}
		public virtual void OnChangeLinkGlyphAlignment(BarItemLink link, Dock newAlignment) {
			AddCustomization(new RuntimePropertyCustomization(link) {
				NewValue = newAlignment,
				PropertyName = "UserGlyphAlignment"
			});
		}
		public virtual void OnChangeLinkGlyphSize(BarItemLink link, GlyphSize newGlyphSize) {
			AddCustomization(new RuntimePropertyCustomization(link) {
				NewValue = newGlyphSize,
				PropertyName = "UserGlyphSize"
			});
		}
		public virtual void OnChangeLinkIsVisible(BarItemLink link, bool newValue) {
			AddCustomization(new RuntimePropertyCustomization(link) {
				NewValue = newValue,
				PropertyName = "IsVisible"
			});
		}
		public virtual void OnChangeLinkBeginGroup(BarItemLink link, bool beginGroup) {
			var holder = link.Links.Holder;
			var args = new BeginGroupChangedEventArgs(link, holder, beginGroup);
			link.RaiseEvent(args);
			if (args.Cancel)
				return;
			if (holder.With(x => x.ItemsSource).ReturnSuccess())
				return;
			int linkIndex = link.Links.IndexOf(link);
			if (beginGroup) {
				link.Links.Insert(linkIndex, new BarItemLinkSeparator());
			} else {
				link.Links.RemoveAt(linkIndex - 1);
			}
		}
		#endregion //baritem customizations
		#region bar customizations
		public virtual void OnCreateNewBar(string caption) {
			CreateNewBarEventArgs args = new CreateNewBarEventArgs(Manager, caption);
			Manager.RaiseEvent(args);
			if (args.Cancel)
				return;
			AddCustomization(new RuntimeCreateNewBarCustomization(args.Caption));			
		}
		public virtual void OnChangeBarVisibility(Bar bar, bool newValue) {
			AddCustomization(new RuntimePropertyCustomization(bar) {
				PropertyName = "Visible",
				NewValue = newValue
			});
		}
		public virtual void OnRenameBar(Bar bar, string newCaption) {
			AddCustomization(new RuntimePropertyCustomization(bar) {
				PropertyName = "Caption",
				NewValue = newCaption
			});
			bar.Caption = newCaption;
		}
		public virtual void OnRemoveBar(Bar bar) {
			RemoveBarEventArgs args = new RemoveBarEventArgs(bar.Manager, bar);
			bar.RaiseEvent(args);
			if (args.Cancel)
				return;
			var canRemove = bar.Manager != null && bar.Manager.Bars.Contains(bar);
			if (canRemove && SyncRemoveWithSource(
				bar.Manager.BarsSource as IEnumerable, 
				bar.Manager.Bars, 
				bar))
				return;
			if (bar.CreatedByCustomizationDialog && canRemove)
				bar.Manager.Bars.Remove(bar);
			else
				AddCustomization(new RuntimePropertyCustomization(bar) {
					PropertyName = "IsRemoved",
					NewValue = true
				});
		}
		#endregion //bar customizations
		#region source synchronization
		class ListObject {
			IEnumerable enumerable;
			ListObjectWorkerBase worker;
			public bool IsValid { get; private set; }
			public ListObject(IEnumerable enumerable) {
				this.enumerable = enumerable;
				var ilist = enumerable as IList;
				if (ilist != null)
					worker = new SimpleListObjectWorker(ilist);
				else {					
					var genericList = enumerable as IList<object>;
					if (genericList != null)
						worker = new GenericListObjectWorker(genericList);
				}
				IsValid = worker != null;
			}
			public int IndexOf(object item) { return worker.IndexOf(item); }
			public void RemoveAt(int index) { worker.RemoveAt(index); }
			public int Count { get { return worker.Count; } }
			public void Insert(int index, object item) { worker.Insert(index, item); }
		}
		abstract class ListObjectWorkerBase {
			public abstract int IndexOf(object item);
			public abstract void RemoveAt(int index);
			public abstract int Count { get; }
			public abstract void Insert(int index, object item);
		}
		class SimpleListObjectWorker : ListObjectWorkerBase {
			IList ilist;
			public SimpleListObjectWorker(IList ilist) { this.ilist = ilist; }
			public override int IndexOf(object item) { return ilist.IndexOf(item); }
			public override void RemoveAt(int index) { ilist.RemoveAt(index); }
			public override int Count { get { return ilist.Count; } }
			public override void Insert(int index, object item) { ilist.Insert(index, item); }
		}
		class GenericListObjectWorker : ListObjectWorkerBase {
			IList<object> ilist;
			public GenericListObjectWorker(IList<object> ilist) { this.ilist = ilist; }
			public override int IndexOf(object item) { return ilist.IndexOf(item); }
			public override void RemoveAt(int index) { ilist.RemoveAt(index); }
			public override int Count { get { return ilist.Count; } }
			public override void Insert(int index, object item) { ilist.Insert(index, item); }
		}
		protected virtual bool SyncRemoveWithSource(IEnumerable sourceCollection, IEnumerable<DependencyObject> resultCollection, DependencyObject resultObject) {
			if (!SynchronizeModificationWithSourceCollections)
				return false;
			var sourceList = new ListObject(sourceCollection);
			if (!sourceList.IsValid || resultCollection == null || resultObject == null)
				return false;
			var sourceObject = ItemsAttachedBehaviorProperties.GetSource(resultObject);
			if(sourceObject==null)
				return false;
			var index = resultCollection.ToList().IndexOf(resultObject);
			if (index == -1) {
				index = sourceList.IndexOf(sourceObject);
			}
			if (index == -1)
				return false;
			sourceList.RemoveAt(index);
			return true;
		}
		protected virtual bool SyncInsertWithSource(IEnumerable targetCollection, IBarItem sourceLink, int index) {
			if (!SynchronizeModificationWithSourceCollections)
				return false;
			var targetList = new ListObject(targetCollection);
			var linkSource = (sourceLink as DependencyObject).With(ItemsAttachedBehaviorProperties.GetSource);
			if (!targetList.IsValid || linkSource == null)
				return false;
			if (index < 0)
				index = 0;
			if (index > targetList.Count)
				index = targetList.Count;
			targetList.Insert(index, linkSource);
			return true;
		}
		protected virtual bool SyncMoveWithSources(IEnumerable sourceCollection, IEnumerable targetCollection, BarItemLink link, int index) {
			if (sourceCollection == null)
				sourceCollection = targetCollection;
			if (targetCollection == null)
				targetCollection = sourceCollection;
			var source = link.With(ItemsAttachedBehaviorProperties.GetSource);
			if (sourceCollection == null || targetCollection == null || source == null)
				return false;
			if (sourceCollection != targetCollection) {
				return SyncRemoveWithSource(sourceCollection, link.Links, link)
					&& SyncInsertWithSource(targetCollection, link, index);
			}			
			var list = new ListObject(sourceCollection);
			var currentIndex = list.IndexOf(source);
			if (currentIndex == index)
				return true;
			list.RemoveAt(currentIndex);
			if (currentIndex < index)
				index -= 1;
			list.Insert(index, source);
			return true;
		}
		#endregion //source synchronization        
		public virtual void OnToolbarGlyphSizeChanged(GlyphSize newValue, GlyphSize oldValue) {
			AddCustomization(new RuntimePropertyCustomization(Manager) {
				PropertyName = "ToolbarGlyphSize",
				OldValue = oldValue,
				NewValue = newValue,
				ActOnHost = true,
				Overwrite = true
			}, true);
		}
		public virtual void OnMenuGlyphSizeChanged(GlyphSize newValue, GlyphSize oldValue) {
			AddCustomization(new RuntimePropertyCustomization(Manager) {
				PropertyName = "MenuGlyphSize",
				OldValue = oldValue,
				NewValue = newValue,
				ActOnHost = true,
				Overwrite = true
			}, true);
		}
		public virtual void OnShowScreenTipsChanged(bool newValue, bool oldValue) {
			AddCustomization(new RuntimePropertyCustomization(Manager) {
				PropertyName = "ShowScreenTips",
				OldValue = oldValue,
				NewValue = newValue,
				ActOnHost = true,
				Overwrite = true
			}, true);
		}
		public virtual void OnShowShortcutInScreenTipsChanged(bool newValue, bool oldValue) {
			AddCustomization(new RuntimePropertyCustomization(Manager) {
				PropertyName = "ShowShortcutInScreenTips",
				OldValue = oldValue,
				NewValue = newValue,
				ActOnHost = true,
				Overwrite = true
			}, true);
		}
	}
}
