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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Docking.Customization {
	public class CustomizationController : DependencyObject, ICustomizationController {
		#region static
		public static readonly DependencyProperty IsCustomizationProperty;
		public static readonly DependencyProperty CustomizationItemsProperty;
		public static readonly DependencyProperty CustomizationRootProperty;
		public static readonly DependencyProperty DragInfoProperty;
		public static readonly RoutedEvent DragInfoChangedEvent;
		static CustomizationController() {
			var dProp = new DependencyPropertyRegistrator<CustomizationController>();
			dProp.Register("IsCustomization", ref IsCustomizationProperty, false, OnIsCustomizationChanged, CoerceIsCustomization);
			dProp.Register("CustomizationItems", ref CustomizationItemsProperty, (ObservableCollection<BaseLayoutItem>)null);
			dProp.Register("CustomizationRoot", ref CustomizationRootProperty, (LayoutGroup)null, OnCustomizationRootChanged, CoerceCustomizationRoot);
			dProp.Register("DragInfo", ref DragInfoProperty, (DragInfo)null);
			dProp.RegisterDirectEvent<DragInfoChangedEventHandler>("DragInfoChanged", ref DragInfoChangedEvent);
		}
		static void OnIsCustomizationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			DockLayoutManager manager = ((CustomizationController)obj).Container;
			manager.CoerceValue(DockLayoutManager.IsCustomizationProperty);
			manager.RaiseEvent(new IsCustomizationChangedEventArgs((bool)e.NewValue) { Source = manager });
		}
		static object CoerceIsCustomization(DependencyObject dObj, object value) {
			return ((CustomizationController)dObj).Container.IsInDesignTime ? true : value;
		}
		static void OnCustomizationRootChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CustomizationController)obj).OnCustomizationRootChanged((LayoutGroup)e.NewValue);
		}
		static object CoerceCustomizationRoot(DependencyObject obj, object baseValue) {
			return ((CustomizationController)obj).CoerceCustomizationRoot((LayoutGroup)baseValue);
		}
		#endregion static
		public bool IsCustomization {
			get { return (bool)GetValue(IsCustomizationProperty); }
			set { SetValue(IsCustomizationProperty, value); }
		}
		public DragInfo DragInfo {
			get { return (DragInfo)GetValue(DragInfoProperty); }
			set { SetValue(DragInfoProperty, value); }
		}
		public ObservableCollection<BaseLayoutItem> CustomizationItems {
			get { return (ObservableCollection<BaseLayoutItem>)GetValue(CustomizationItemsProperty); }
			set { SetValue(CustomizationItemsProperty, value); }
		}
		public LayoutGroup CustomizationRoot {
			get { return (LayoutGroup)GetValue(CustomizationRootProperty); }
			set { SetValue(CustomizationRootProperty, value); }
		}
		public event DragInfoChangedEventHandler DragInfoChanged {
			add { Container.AddHandler(DragInfoChangedEvent, value); }
			remove { Container.RemoveHandler(DragInfoChangedEvent, value); }
		}
		protected Selection Selection { get { return Container.LayoutController.Selection; } }
		bool isDisposing;
		BarManagerMenuController itemContextMenuController, itemsSelectorMenuController, layoutControlItemContextMenuController;
		BarManagerMenuController layoutControlItemCustomizationMenuController;
		BarManagerMenuController hiddenItemsMenuController;
		ContextMenuManager contextMenuManager;
		public CustomizationController(DockLayoutManager container) {
			Container = container;
			Container.Unloaded += OnContainerUnloaded;
			Container.ClosedPanels.CollectionChanged += OnClosedPanelsCollectionChanged;
			CustomizationItems = new ObservableCollection<BaseLayoutItem>();
			contextMenuManager = new ContextMenuManager(this);
		}
		void OnContainerUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			contextMenuManager.Reset();
		}
		public BarManagerMenuController ItemContextMenuController { get { return itemContextMenuController ?? (itemContextMenuController = CreateMenuController()).Do(InitializeMenuController); } }
		public BarManagerMenuController ItemsSelectorMenuController { get { return itemsSelectorMenuController ?? (itemsSelectorMenuController = CreateMenuController()).Do(InitializeMenuController); } }
		public BarManagerMenuController LayoutControlItemContextMenuController { get { return layoutControlItemContextMenuController ?? (layoutControlItemContextMenuController = CreateMenuController()).Do(InitializeMenuController); } }
		public BarManagerMenuController LayoutControlItemCustomizationMenuController { get { return layoutControlItemCustomizationMenuController ?? (layoutControlItemCustomizationMenuController = CreateMenuController()).Do(InitializeMenuController); } }
		public BarManagerMenuController HiddenItemsMenuController { get { return hiddenItemsMenuController ?? (hiddenItemsMenuController = CreateMenuController()).Do(InitializeMenuController); } }
		protected virtual void InitializeMenuController(BarManagerMenuController controller) {
			controller.DataContext = null;
			DockLayoutManager.AddLogicalChild(Container, controller);
		}
		protected virtual BarManagerMenuController CreateMenuController() {
			return new BarManagerMenuController();
		}
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDisposing() {
			Container.Unloaded -= OnContainerUnloaded;
			Container.ClosedPanels.CollectionChanged -= OnClosedPanelsCollectionChanged;
			barManagerCore = null;
			ItemContextMenu = null;
			ItemsSelectorMenu = null;
			LayoutControlItemContextMenu = null;
			itemContextMenuController = null;
			itemsSelectorMenuController = null;
			layoutControlItemContextMenuController = null;
			layoutControlItemCustomizationMenuController = null;
			ClosedItemsBar = null;
			DisposeCustomizationForm();
			contextMenuManager.Dispose();
			Container = null;
		}
		void DisposeCustomizationForm() {
			if(CustomizationForm != null) {
				CustomizationForm.BeginUpdate();
				CustomizationForm.Hidden -= CustomizationFormClosed;
				if(CustomizationForm.IsOpen)
					CustomizationForm.IsOpen = false;
				CustomizationForm.Close();
				CustomizationForm.Content = null;
				CustomizationForm = null;
			}
			if(CustomizationControl != null) {
				DockLayoutManager.SetDockLayoutManager(CustomizationControl, null);
				CustomizationControl.Dispose();
				CustomizationControl = null;
			}
		}
		protected virtual void OnCustomizationRootChanged(LayoutGroup root) {
			ClearSelection();
			CustomizationItems.Clear();
			CustomizationItems.Add(root);
		}
		protected virtual object CoerceCustomizationRoot(LayoutGroup baseValue) {
			return (baseValue != null && baseValue.IsUngroupped && Container != null) ? Container.LayoutRoot : baseValue;
		}
		public void UpdateDragInfo(DragInfo value) {
			if(DragInfo == value) return;
			DragInfo = value;
			Container.RaiseEvent(new DragInfoChangedEventArgs(DragInfo));
		}
		protected virtual BarManager CreateBarManager() {
			return new DockBarManager(Container);
		}
		protected virtual ClosedItemsBar CreateClosedItemsBar() {
			return new ClosedItemsBar(Container.ClosedItemsPanel);
		}
		protected virtual ItemContextMenu CreateItemContextMenu() {
			return new ItemContextMenu(Container);
		}
		protected virtual ItemsSelectorMenu CreateItemsSelectorMenu() {
			return new ItemsSelectorMenu(Container);
		}
		protected virtual LayoutControlItemContextMenu CreateLayoutControlItemContextMenu() {
			return new LayoutControlItemContextMenu(Container);
		}
		protected virtual LayoutControlItemCustomizationMenu CreateLayoutControlItemCustomizationMenu() {
			return new LayoutControlItemCustomizationMenu(Container);
		}
		protected virtual HiddenItemContextMenu CreateHiddenItemMenu() {
			return new HiddenItemContextMenu(Container);
		}
		public DockLayoutManager Container { get; private set; }
		ClosedPanelsBarVisibility visibilityCore;
		public ClosedPanelsBarVisibility ClosedPanelsBarVisibility {
			get { return visibilityCore; }
			set {
				if(visibilityCore == value) return;
				visibilityCore = value;
				UpdateClosedItemsBarVisibility();
			}
		}
		protected ClosedPanelsBarVisibility ActualClosedItemsBarVisibility {
			get {
				if(ClosedPanelsBarVisibility == ClosedPanelsBarVisibility.Default)
					return ClosedPanelsBarVisibility.Manual;
				return ClosedPanelsBarVisibility;
			}
		}
		bool CanShowClosedItems {
			get {
				bool hasClosedItems = Container.ClosedPanels.Count > 0;
				return hasClosedItems && ClosedPanelsBarVisibility != ClosedPanelsBarVisibility.Never;
			}
		}
		bool IsAutoShowClosedItems {
			get { return ActualClosedItemsBarVisibility == ClosedPanelsBarVisibility.Auto; }
		}
		bool IsHideClosedItems {
			get { return !Container.IsLoaded || ActualClosedItemsBarVisibility == ClosedPanelsBarVisibility.Never; }
		}
		protected virtual void OnClosedPanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(EnsureClosedItemsBar()) {
				switch(e.Action) {
					case NotifyCollectionChangedAction.Add: ClosedItemsBar.AddItems(e.NewItems); break;
					case NotifyCollectionChangedAction.Remove: ClosedItemsBar.RemoveItems(e.OldItems); break;
					case NotifyCollectionChangedAction.Reset: ClosedItemsBar.ResetItems(); break;
				}
				UpdateClosedItemsBarVisibility();
			}
		}
		protected void UpdateClosedItemsBarVisibility() {
			if(IsHideClosedItems)
				HideClosedItemsBar();
			else
				if(IsAutoShowClosedItems) {
					if(Container.ClosedPanels.Count == 0) HideClosedItemsBar();
					else ShowClosedItemsBar();
				}
		}
		public void UpdateClosedItemsBar() {
			if(EnsureClosedItemsBar()) {
				ClosedItemsBar.UpdateItems(Container.ClosedPanels);
				UpdateClosedItemsBarVisibility();
			}
		}
		BarManager barManagerCore;
		bool isOwnBarManager;
		public BarManager BarManager {
			get {
				if(barManagerCore == null) {
					barManagerCore = LayoutHelper.FindParentObject<BarManager>(Container);
					if(barManagerCore == null) {
						barManagerCore = CreateBarManager();
						isOwnBarManager = true;
						DockLayoutManager.AddToVisualTree(Container, barManagerCore);
						DockLayoutManager.AddLogicalChild(Container, barManagerCore);
					}
				}
				return barManagerCore;
			}
		}
		protected ItemContextMenu ItemContextMenu { get; private set; }
		protected ItemsSelectorMenu ItemsSelectorMenu { get; private set; }
		protected LayoutControlItemContextMenu LayoutControlItemContextMenu { get; private set; }
		protected LayoutControlItemCustomizationMenu LayoutControlItemCustomizationMenu { get; private set; }
		protected HiddenItemContextMenu HiddenItemContextMenu { get; private set; }
		public void ShowItemSelectorMenu(UIElement source, BaseLayoutItem[] items) {
			CloseMenu();
			ItemsSelectorMenu = contextMenuManager.ItemsSelectorMenu;
			ItemsSelectorMenuController.Menu = ItemsSelectorMenu;
			ItemsSelectorMenu.Show(source, items);
		}
		public void ShowContextMenu(BaseLayoutItem item) {
			CloseMenu();
			ItemContextMenu = contextMenuManager.ItemsContextMenu;
			ItemContextMenuController.Menu = ItemContextMenu;
			ItemContextMenu.Show(item);
		}
		public void ShowControlItemContextMenu(BaseLayoutItem item) {
			CloseMenu();
			if(IsCustomization)
				ShowControlItemCustomizationMenu(item);
			else
				ShowControlItemNormalMenu(item);
		}
		private void ShowControlItemNormalMenu(BaseLayoutItem item) {
			LayoutControlItemContextMenu = contextMenuManager.LayoutControlItemMenu;
			LayoutControlItemContextMenuController.Menu = LayoutControlItemContextMenu;
			LayoutControlItemContextMenu.Show(item);
		}
		private void ShowControlItemCustomizationMenu(BaseLayoutItem item) {
			LayoutControlItemCustomizationMenu = contextMenuManager.LayoutControlItemCustomizationMenu;
			LayoutControlItemCustomizationMenuController.Menu = LayoutControlItemCustomizationMenu;
			UIElement source = MenuSource ?? item.GetUIElement<IUIElement>() as UIElement;
			LayoutControlItemCustomizationMenu.Show(source, Selection.ToArray());
			MenuSource = null;
		}
		public void ShowHiddenItemMenu(BaseLayoutItem item) {
			CloseMenu();
			HiddenItemContextMenu = contextMenuManager.HiddenItemMenu;
			HiddenItemsMenuController.Menu = HiddenItemContextMenu;
			UIElement source = MenuSource ?? item.GetUIElement<IUIElement>() as UIElement;
			HiddenItemContextMenu.Show(item, source);
			MenuSource = null;
		}
		public void CloseMenu() {
			contextMenuManager.Reset();
			menuItemCounter = 0;
		}
		public UIElement MenuSource { get; set; }
		public ClosedItemsBar ClosedItemsBar { get; private set; }
		public FloatingContainer CustomizationForm { get; private set; }
		public CustomizationControl CustomizationControl { get; private set; }
		public bool IsClosedPanelsVisible {
			get { return ClosedItemsBar != null && ClosedItemsBar.Visible; }
		}
		public bool ClosedPanelsVisibility {
			get { return IsClosedPanelsVisible; }
			set {
				if(value) ShowClosedItemsBar();
				else HideClosedItemsBar();
			}
		}
		public bool IsCustomizationFormVisible {
			get { return CustomizationForm != null && CustomizationForm.IsOpen; }
		}
		protected bool HasBarManager {
			get { return barManagerCore != null; }
		}
		protected bool EnsureClosedItemsBar() {
			if(Container.ClosedItemsPanel == null) return false;
			if(ClosedItemsBar == null) {
				if(HasBarManager)
					ClosedItemsBar = CreateClosedItemsBar();
			}
			else {
				if(ClosedItemsBar.Panel == null)
					ClosedItemsBar.UpdatePanel(Container.ClosedItemsPanel);
			}
			return ClosedItemsBar != null;
		}
		public void HideClosedItemsBar() {
			if(!IsClosedPanelsVisible) return;
			Container.LockClosedPanelsVisibility();
			try {
				ClosedItemsBar.Visible = false;
			}
			finally {
				Container.UnlockClosedPanelsVisibility();
			}
			if(BarManager.Bars.Contains(ClosedItemsBar)) {
				BarManager.Bars.Remove(ClosedItemsBar);
			}
			closedItemCounter = ClosedItemsBar.ItemLinks.Count;
		}
		public void ShowClosedItemsBar() {
			if(IsClosedPanelsVisible || !CanShowClosedItems) return;
			if(EnsureClosedItemsBar()) {
				if(!BarManager.Bars.Contains(ClosedItemsBar)) {
					BarManager.Bars.Add(ClosedItemsBar);
				}
				Container.LockClosedPanelsVisibility();
				try {
					ClosedItemsBar.Visible = true;
				}
				finally {
					Container.UnlockClosedPanelsVisibility();
				}
			}
		}
		public void ShowCustomizationForm() {
			if(!IsCustomization || IsCustomizationFormVisible) return;
			if(EnsureCustomizationForm()) {
				Container.AddToLogicalTree(CustomizationForm, CustomizationControl);
				EnsureDockLayoutManager();
				CustomizationForm.IsOpen = true;
				Container.RaiseEvent(new CustomizationFormVisibleChangedEventArgs(true) { Source = Container });
			}
		}
		void EnsureDockLayoutManager() {
			if(CustomizationControl == null || Container == null) return;
			DockLayoutManager.SetDockLayoutManager(CustomizationControl, Container);
		}
		int lockHideCustomization = 0;
		void CustomizationFormClosed(object sender, RoutedEventArgs e) {
			Container.RaiseEvent(new CustomizationFormVisibleChangedEventArgs(false) { Source = Container });
			if(lockHideCustomization > 0) return;
			EndCustomization();
		}
		public void HideCustomizationForm() {
			if(CustomizationForm == null) return;
			lockHideCustomization++;
			CustomizationForm.IsOpen = false;
			Container.RemoveFromLogicalTree(CustomizationForm, CustomizationControl);
			DockLayoutManager.SetDockLayoutManager(CustomizationControl, null);
			lockHideCustomization--;
		}
		public void BeginCustomization() {
			if(!Container.AllowCustomization) return;
			IsCustomization = true;
			CustomizationRoot = Container.LayoutRoot;
			ShowCustomizationForm();
		}
		public void EndCustomization() {
			ClearSelection();
			HideCustomizationForm();
			DisposeCustomizationForm();
			IsCustomization = false;
		}
		void ClearSelection() {
			if(CustomizationRoot == null || Container == null) return;
			IView view = Container.GetView(CustomizationRoot);
			Container.ViewAdapter.SelectionService.ClearSelection(view);
		}
		protected bool IsDocumentSelectorContainerVisible {
			get { return DocumentSelectorContainer != null && DocumentSelectorContainer.IsOpen; }
		}
		public bool IsDocumentSelectorVisible { 
			get { return IsDocumentSelectorContainerVisible; } 
		}
		public void ShowDocumentSelectorForm() {
			if(!Container.AllowDocumentSelector) return;
			if(EnsureDocumentSelectorContainer()) {
				DocumentSelectorControl.InitializeItems(Container);
				if(!DocumentSelectorControl.HasItemsToShow) return;
				Container.AddToLogicalTree(DocumentSelectorContainer, DocumentSelectorControl);
				DocumentSelectorControl.SetSelectedItem();
				DocumentSelectorControl.PanelsCaption = DockingLocalizer.GetString(DockingStringId.DocumentSelectorPanels);
				DocumentSelectorControl.DocumentsCaption = DockingLocalizer.GetString(DockingStringId.DocumentSelectorDocuments);
				DocumentSelectorContainer.ContainerTemplate = DocumentSelectorContainerFactory.GetTemplate(Container);
				Size size = DocumentSelectorContainerFactory.GetSize(Container, new Size(400, 220));
				DocumentSelectorContainer.FloatSize = size;
				DocumentSelectorContainer.FloatLocation = DocumentSelectorContainerFactory.GetLocation(Container, size);
				DocumentSelectorContainer.IsOpen = true;
			}
		}
		public void HideDocumentSelectorForm() {
			if(!IsDocumentSelectorContainerVisible) return;
			DocumentSelectorControl.Close();
		}
		protected FloatingContainer DocumentSelectorContainer { get; private set; }
		protected DocumentSelector DocumentSelectorControl { get; private set; }
		protected bool EnsureDocumentSelectorContainer() {
			if(DocumentSelectorContainer == null) {
				if(EnsureDocumentSelectorControl()) {
					DocumentSelectorContainer = CreateDocumentSelectorContainer();
				}
			}
			return DocumentSelectorContainer != null;
		}
		protected bool EnsureDocumentSelectorControl() {
			if(DocumentSelectorControl == null) {
				DocumentSelectorControl = CreateDocumentSelectorControl();
				DevExpress.Xpf.Docking.Platform.WindowHelper.BindFlowDirection(DocumentSelectorControl, Container);
			}
			return DocumentSelectorControl != null;
		}
		protected virtual DocumentSelector CreateDocumentSelectorControl() {
			return new DocumentSelector();
		}
		protected virtual FloatingContainer CreateDocumentSelectorContainer() {
			return DocumentSelectorContainerFactory.CreateDocumentSelectorContainer(Container, DocumentSelectorControl);
		}
		protected FloatingContainer DragCursorContainer { get; private set; }
#if DEBUGTEST
		internal
#endif
		protected DragCursorControl DragCursorControl { get; private set; }
		public bool IsDragCursorVisible {
			get { return DragCursorContainer != null && DragCursorContainer.IsOpen; }
		}
		protected bool EnsureDragCursorContainer() {
			if(DragCursorContainer == null) {
				if(EnsureDragCursorControl()) {
					DragCursorContainer = CreateDragCursorContainer();
				}
			}
			else DragCursorContainer.Content = DragCursorControl;
			(DragCursorContainer as FloatingWindowContainer)
				.With(x => x.Window)
				.Do(x => x.Owner = Window.GetWindow(Container));
			return DragCursorContainer != null;
		}
		protected bool EnsureDragCursorControl() {
			if(DragCursorControl == null)
				DragCursorControl = CreateDragCursorControl();
			return DragCursorControl != null;
		}
		protected virtual FloatingContainer CreateDragCursorContainer() {
			return DragCursorFactory.CreateDragCursorContainer(Container, DragCursorControl);
		}
		protected virtual DragCursorControl CreateDragCursorControl() {
			return new DragCursorControl();
		}
		Point GetDragCursorPos(Point point) {
			return new Point(point.X + 5, point.Y + 5);
		}
		public void ShowDragCursor(Point point, BaseLayoutItem item) {
			if(IsDragCursorVisible) return;
			if(EnsureDragCursorContainer()) {
				BaseLayoutItem itemToShow = item;
				TabbedGroup tg = item as TabbedGroup;
				if(tg != null && tg.CaptionImage == null && tg.CaptionTemplate == null && tg.Caption == null && string.IsNullOrEmpty(tg.ActualCaption))
					if(tg.SelectedItem != null) itemToShow = tg.SelectedItem;
				DragCursorControl.DataContext = itemToShow;
				DockLayoutManager.SetLayoutItem(DragCursorControl, itemToShow);
				DragCursorControl.CursorType = itemToShow.ItemType == LayoutItemType.Panel || itemToShow.ItemType == LayoutItemType.Document || itemToShow.ItemType == LayoutItemType.TabPanelGroup ?
					DragCursorType.Panel : DragCursorControl.CursorType = DragCursorType.Item;
				DragCursorContainer.FloatLocation = GetDragCursorPos(point);
				Container.AddToLogicalTree(DragCursorContainer, DragCursorControl);
				DragCursorContainer.IsOpen = true;
			}
		}
		public void HideDragCursor() {
			if(!IsDragCursorVisible) return;
			DragCursorContainer.IsOpen = false;
			Container.RemoveFromLogicalTree(DragCursorContainer, DragCursorControl);
			DragCursorContainer.Content = null;
			DragCursorControl.ClearValue(FrameworkElement.DataContextProperty);
			DockLayoutManager.SetLayoutItem(DragCursorControl, null);
		}
		public void SetDragCursorPosition(Point point) {
			if(!IsDragCursorVisible) return;
			DragCursorContainer.FloatLocation = GetDragCursorPos(point);
		}
		public FrameworkElement[] GetChildren() {			
			List<FrameworkElement> children = new List<FrameworkElement>();
			if (barManagerCore != null && isOwnBarManager) {
				children.Add(barManagerCore);
			}
			if (!IsCustomization)
				return children.ToArray();
			if(IsCustomizationFormVisible) {
				if(CustomizationForm != null)
					children.Add(CustomizationForm);
				if(CustomizationControl != null) {
					children.Add(CustomizationControl as FrameworkElement);
					FrameworkElement[] elements = CustomizationControl.GetChildren();
					for(int i = 0; i < elements.Length; i++) {
						if(elements[i] != null) children.Add(elements[i]);
					}
				}
			}
			if(IsDragCursorVisible) {
				if(DragCursorContainer != null)
					children.Add(DragCursorContainer);
				if(DragCursorControl != null) {
					children.Add(DragCursorControl);
					FrameworkElement[] elements = DragCursorControl.GetChildren();
					for(int i = 0; i < elements.Length; i++) {
						if(elements[i] != null) children.Add(elements[i]);
					}
				}
			}
			if(IsDocumentSelectorContainerVisible) {
				if(DocumentSelectorContainer != null)
					children.Add(DocumentSelectorContainer);
				if(DocumentSelectorControl != null) {
					children.Add(DocumentSelectorControl);
					FrameworkElement[] elements = DocumentSelectorControl.GetChildren();
					for(int i = 0; i < elements.Length; i++) {
						if(elements[i] != null) children.Add(elements[i]);
					}
				}
			}			
			return children.ToArray();
		}
		protected bool EnsureCustomizationForm() {
			if(CustomizationForm == null) {
				if(EnsureCustomizationControl()) {
					CustomizationForm = CreateCustomizationForm();
					CustomizationForm.Hidden += new RoutedEventHandler(CustomizationFormClosed);
				}
			}
			return CustomizationForm != null;
		}
		protected bool EnsureCustomizationControl() {
			if(CustomizationControl == null) {
				CustomizationControl = CreateCustomizationControl();
			}
			return CustomizationControl != null;
		}
		protected virtual CustomizationControl CreateCustomizationControl() {
			return new CustomizationControl();
		}
		protected virtual FloatingContainer CreateCustomizationForm() {
			return CustomizationFormFactory.CreateCustomizationForm(Container, CustomizationControl);
		}
		static int menuItemCounter = 0;
		static int closedItemCounter = 0;
		public static string GetUniqueMenuItemName() {
			return string.Format("MenuItem{0}", ++menuItemCounter);
		}
		public static string GetUniqueClosedItemName() {
			return string.Format("ClosedItem{0}", ++closedItemCounter);
		}
		public T CreateCommand<T>() where T : CustomizationControllerCommand, new() {
			return new T() { Controller = this };
		}
		public void DesignTimeRaiseEvent(object sender, System.Windows.RoutedEventArgs e) {
			if(isDisposing) return;
			IUIElement element = LayoutItemsHelper.GetIUIParent(sender as DependencyObject).GetRootUIScope();
			if(element != null) {
				DevExpress.Xpf.Docking.Platform.LayoutView view = Container.GetView(element) as DevExpress.Xpf.Docking.Platform.LayoutView;
				if(view != null) view.OnDesignTimeEvent(sender, e);
			}
		}
		class DockBarManager : BarManager {
			public DockBarManager(DockLayoutManager container) {
				AllowCustomization = false;
				CreateStandardLayout = false;
				DevExpress.Xpf.Core.Serialization.DXSerializer.SetEnabled(this, false);
			}
		}
		class ContextMenuManager : IDisposable {
			CustomizationController Controller;
			public ContextMenuManager(CustomizationController controller) {
				Controller = controller;
			}
			List<BaseLayoutElementMenu> menus = new List<BaseLayoutElementMenu>();
			ItemsSelectorMenu itemsSelectorMenu;
			public ItemsSelectorMenu ItemsSelectorMenu {
				get {
					if(itemsSelectorMenu == null) {
						itemsSelectorMenu = Controller.CreateItemsSelectorMenu();
						menus.Add(ItemsSelectorMenu);
					}
					return itemsSelectorMenu;
				}
			}
			ItemContextMenu itemContextMenu;
			public ItemContextMenu ItemsContextMenu {
				get {
					if(itemContextMenu == null) {
						itemContextMenu = Controller.CreateItemContextMenu();
						menus.Add(itemContextMenu);
					}
					return itemContextMenu;
				}
			}
			LayoutControlItemContextMenu controlItemMenu;
			public LayoutControlItemContextMenu LayoutControlItemMenu {
				get {
					if(controlItemMenu == null) {
						controlItemMenu = Controller.CreateLayoutControlItemContextMenu();
						menus.Add(controlItemMenu);
					}
					return controlItemMenu;
				}
			}
			LayoutControlItemCustomizationMenu controlItemCustomizationMenu;
			public LayoutControlItemCustomizationMenu LayoutControlItemCustomizationMenu {
				get {
					if(controlItemCustomizationMenu == null) {
						controlItemCustomizationMenu = Controller.CreateLayoutControlItemCustomizationMenu();
						menus.Add(controlItemCustomizationMenu);
					}
					return controlItemCustomizationMenu;
				}
			}
			HiddenItemContextMenu hiddenItemMenu;
			public HiddenItemContextMenu HiddenItemMenu {
				get {
					if(hiddenItemMenu == null) {
						hiddenItemMenu = Controller.CreateHiddenItemMenu();
						menus.Add(hiddenItemMenu);
					}
					return hiddenItemMenu;
				}
			}
			public void Reset() {
				foreach(BaseLayoutElementMenu menu in menus) {
					menu.Close();
					menu.ClearItems();
				}
			}
			#region IDisposable Members
			bool isDisposing;
			public void Dispose() {
				if(!isDisposing) {
					isDisposing = true;
					Controller = null;
				}
				GC.SuppressFinalize(this);
			}
			#endregion
		}
	}
	public static class CustomizationControllerHelper {
		static ICustomizationController GetCustomizationController(DockLayoutManager container) {
			return (container != null) ? container.CustomizationController : null;
		}
		public static CustomizationControllerCommand CreateCommand(DockLayoutManager container) {
			ICustomizationController controller = GetCustomizationController(container);
			return controller.IsClosedPanelsVisible ? (CustomizationControllerCommand)CreateCommand<HideClosedItemsCommand>(container) :
				(CustomizationControllerCommand)CreateCommand<ShowClosedItemsCommand>(container);
		}
		public static T CreateCommand<T>(DockLayoutManager container) where T : CustomizationControllerCommand, new() {
			ICustomizationController controller = GetCustomizationController(container);
			return (controller != null) ? controller.CreateCommand<T>() : null;
		}
		public static void AssignCommand(BarItem item, ICommand command, UIElement commandTarget) {
			if(item == null) return;
			item.Command = command;
			item.CommandTarget = commandTarget;
		}
	}
	public class DragInfo {
		public BaseLayoutItem Item { get; private set; }
		public BaseLayoutItem Target { get; private set; }
		public DropType Type { get; private set; }
		readonly int _hash;
		public DragInfo(BaseLayoutItem item, BaseLayoutItem target, DropType type) {
			Item = item;
			Target = target;
			Type = type;
			_hash = (int)type ^
				((item != null) ? item.GetHashCode() : 0) ^
				((target != null) ? target.GetHashCode() : 0);
		}
		public override bool Equals(object obj) {
			DragInfo info = obj as DragInfo;
			if((object)info == null) return false;
			return _hash == info._hash;
		}
		public override int GetHashCode() {
			return _hash;
		}
		public static bool operator ==(DragInfo left, DragInfo right) {
			return object.Equals(left, right);
		}
		public static bool operator !=(DragInfo left, DragInfo right) {
			return !object.Equals(left, right);
		}
	}
	public delegate void DragInfoChangedEventHandler(
			object sender, DragInfoChangedEventArgs e
		);
	public class DragInfoChangedEventArgs : RoutedEventArgs {
		public DragInfo Info { get; private set; }
		public DragInfoChangedEventArgs(DragInfo info) {
			Info = info;
			RoutedEvent = CustomizationController.DragInfoChangedEvent;
		}
	}
	#region Commands
	public abstract class CustomizationControllerCommand : ICommand {
		internal CustomizationControllerCommand() { }
		protected internal ICustomizationController Controller { get; set; }
		protected abstract void ExecuteCore(ICustomizationController controller);
		protected abstract bool CanExecuteCore(ICustomizationController controller);
		#region ICommand
		event EventHandler CanExecuteChangedCore;
		event EventHandler ICommand.CanExecuteChanged {
			add { CanExecuteChangedCore += value; }
			remove { CanExecuteChangedCore -= value; }
		}
		protected void RaiseCanExecuteChanged() {
			if(CanExecuteChangedCore != null)
				CanExecuteChangedCore(this, EventArgs.Empty);
		}
		bool ICommand.CanExecute(object parameter) {
			return (Controller != null) && CanExecuteCore(Controller);
		}
		void ICommand.Execute(object parameter) {
			if(Controller == null) return;
			ExecuteCore(Controller);
		}
		#endregion ICommand
		#region static
		static CustomizationControllerCommand() {
			ShowClosedItems = new CustomizationControllerCommandLink("ShowClosedItems", new ShowClosedItemsCommand());
			HideClosedItems = new CustomizationControllerCommandLink("HideClosedItems", new HideClosedItemsCommand());
		}
		public static RoutedCommand ShowClosedItems { get; private set; }
		public static RoutedCommand HideClosedItems { get; private set; }
		internal static void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
			ICustomizationController controller = GetCustomizationController(DockLayoutManager.GetDockLayoutManager(sender as DependencyObject));
			CustomizationControllerCommand command = ((CustomizationControllerCommandLink)e.Command).Command;
			e.CanExecute = (controller != null) && command.CanExecuteCore(controller);
		}
		internal static void Executed(object sender, ExecutedRoutedEventArgs e) {
			ICustomizationController controller = GetCustomizationController(DockLayoutManager.GetDockLayoutManager(sender as DependencyObject));
			if(controller != null && e.Parameter != null) {
				CustomizationControllerCommand command = ((CustomizationControllerCommandLink)e.Command).Command;
				command.ExecuteCore(controller);
			}
		}
		static ICustomizationController GetCustomizationController(DockLayoutManager container) {
			return (container != null) ? container.CustomizationController : null;
		}
		#endregion static
		class CustomizationControllerCommandLink : RoutedCommand {
			public CustomizationControllerCommandLink(string name, CustomizationControllerCommand command) :
				base(name, typeof(CustomizationControllerCommand)) {
				Command = command;
			}
			public CustomizationControllerCommand Command { get; private set; }
		}
	}
	public class ShowClosedItemsCommand : CustomizationControllerCommand {
		protected override bool CanExecuteCore(ICustomizationController controller) {
			bool canShow = controller.ClosedPanelsBarVisibility != ClosedPanelsBarVisibility.Never;
			return canShow && controller.Container.ClosedPanels.Count > 0 && !controller.IsClosedPanelsVisible;
		}
		protected override void ExecuteCore(ICustomizationController controller) {
			controller.ShowClosedItemsBar();
			if(controller.Container.ClosedPanelsBarVisibility == ClosedPanelsBarVisibility.Auto) {
				controller.Container.ClosedPanelsBarVisibility = ClosedPanelsBarVisibility.Manual;
			}
		}
	}
	public class HideClosedItemsCommand : CustomizationControllerCommand {
		protected override bool CanExecuteCore(ICustomizationController controller) {
			return controller.IsClosedPanelsVisible;
		}
		protected override void ExecuteCore(ICustomizationController controller) {
			controller.HideClosedItemsBar();
			if(controller.Container.ClosedPanelsBarVisibility == ClosedPanelsBarVisibility.Auto) {
				controller.Container.ClosedPanelsBarVisibility = ClosedPanelsBarVisibility.Manual;
			}
		}
	}
	public class ShowCustomizationFormCommand : CustomizationControllerCommand {
		protected override bool CanExecuteCore(ICustomizationController controller) {
			return !controller.IsCustomizationFormVisible && controller.IsCustomization;
		}
		protected override void ExecuteCore(ICustomizationController controller) {
			controller.ShowCustomizationForm();
		}
	}
	public class HideCustomizationFormCommand : CustomizationControllerCommand {
		protected override bool CanExecuteCore(ICustomizationController controller) {
			return controller.IsCustomizationFormVisible && controller.IsCustomization;
		}
		protected override void ExecuteCore(ICustomizationController controller) {
			controller.HideCustomizationForm();
		}
	}
	public class EndCustomizationCommand : CustomizationControllerCommand {
		protected override bool CanExecuteCore(ICustomizationController controller) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ICustomizationController controller) {
			controller.EndCustomization();
		}
	}
	public class BeginCustomizationCommand : CustomizationControllerCommand {
		protected override bool CanExecuteCore(ICustomizationController controller) {
			return !controller.IsCustomization;
		}
		protected override void ExecuteCore(ICustomizationController controller) {
			controller.BeginCustomization();
		}
	}
	#endregion Commands
}
