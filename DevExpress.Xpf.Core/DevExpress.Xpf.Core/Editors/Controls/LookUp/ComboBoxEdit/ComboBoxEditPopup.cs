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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors.Popups {
	public enum SelectionEventMode {
		MouseEnter,
		MouseDown,
		MouseUp,
	}
	[ToolboxItem(false)]
	public partial class EditorListBox : ListBox, ISelectorEditInnerListBox {
		[ThreadStatic]
		static ReflectionHelper helper;
		static ReflectionHelper ReflectionHelper {
			get { return helper ?? (helper = CreateReflectionHelper()); }
		}
		static ReflectionHelper CreateReflectionHelper() {
			return new ReflectionHelper();
		}
		static DataTemplate customItemContentTemplate;
		static DataTemplate CustomItemContentTemplate {
			get {
				return customItemContentTemplate ?? (customItemContentTemplate =
					XamlHelper.GetTemplate("<ContentPresenter Content=\"{Binding DisplayValue}\"/>"));
			}
		}
		public static readonly DependencyProperty AllowItemHighlightingProperty;
		static EditorListBox() {
			AllowItemHighlightingProperty = DependencyPropertyManager.Register("AllowItemHighlighting", typeof(bool), typeof(EditorListBox), new FrameworkPropertyMetadata(false));
		}
		SelectorEditInnerListBoxItemsSourceHelper itemsSourceHelper;
		SelectorEditInnerListBoxItemsSourceHelper ItemsSourceHelper {
			get {
				if (itemsSourceHelper == null)
					itemsSourceHelper = CreateItemsSourceHelper();
				return itemsSourceHelper ?? new DummyListSourceHelper();
			}
		}
		SelectionViewModel SelectionViewModel {
			get { return ((ISelectorEditPropertyProvider)ActualPropertyProvider.GetProperties((BaseEdit)OwnerEdit)).SelectionViewModel; }
		}
		bool IsSelectionChangerActive {
			get {
				object changer = ReflectionHelper.GetPropertyValue<object>(this, "SelectionChange", BindingFlags.NonPublic | BindingFlags.Instance);
				return ReflectionHelper.GetPropertyValue<bool>(changer, "IsActive", BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}
		public ISelectorEditStrategy EditStrategy { get { return OwnerEdit.EditStrategy as ISelectorEditStrategy; } }
		public EditorListBox() {
			IsTextSearchEnabled = false;
			IsSynchronizedWithCurrentItem = false;
			SelectionLocker = new Locker();
			SelectionChangedLocker = new Locker();
			SynchronizationLocker = new Locker();
			InnerItemSynchronizationLocker = new Locker();
			Loaded += EditorListBoxLoaded;
			Unloaded += EditorListBoxUnloaded;
			SelectAllAction = new PostponedAction(() => IsSelectionChangerActive);
		}
		readonly Locker clearHighlightedTextLocker = new Locker();
		protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
			base.OnPreviewTextInput(e);
			string text = e.Text;
			if (!string.IsNullOrEmpty(text)) {
				ProcessTextSearch(text);
			}
		}
		private void ProcessTextSearch(string text) {
			object result = null;
			clearHighlightedTextLocker.DoLockedAction(() => {
				if (OwnerEdit.EditStrategy.DoTextSearch(text, GetFocusedItemIndex(), ref result)) {
					FocusSearchResult(result);
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetHighlightedText(EditStrategy.SearchText)));
				}
				else if (string.IsNullOrEmpty(EditStrategy.SearchText))
					SetHighlightedText(EditStrategy.SearchText);
			});
		}
		private int GetFocusedItemIndex() {
			int index = GetFocusedItemIndexCore();
			if (!IsServerMode && index != -1) {
				var container = ItemContainerGenerator.ContainerFromIndex(index);
				var item = ItemContainerGenerator.ItemFromContainer(container);
				index = OwnerEdit.ItemsProvider.GetIndexByItem(item, EditStrategy.CurrentDataViewHandle);
			}
			return index;
		}
		private void FocusSearchResult(object result) {
			if (EditStrategy.IsSingleSelection) {
				SelectedItem = !IsServerMode ? GetItemByValue(result) : GetSelectedItemByEditValueInServerMode(result);
				FocusSelectedItem();
			}
			else
				MakeVisibleAndFocusItem(result);
		}
		object GetItemByValue(object value) {
			return EditStrategy.ItemsProvider.GetItem(value, EditStrategy.CurrentDataViewHandle);
		}
		private void MakeVisibleAndFocusItem(object result) {
			ScrollIntoView(result);
			FocusItem(result);
		}
		private void FocusItem(object item) {
			if (item != null) {
				FocusContainer(ItemContainerGenerator.ContainerFromItem(item));
			}
		}
		internal void FocusContainer(DependencyObject container) {
			ListBoxItem listBoxItem = container as ListBoxItem;
			if (listBoxItem != null)
				listBoxItem.Focus();
		}
		protected virtual SelectorEditInnerListBoxItemsSourceHelper CreateItemsSourceHelper() {
			if (OwnerEdit == null)
				return null;
			return SelectorEditInnerListBoxItemsSourceHelper.CreateHelper(this, !OwnerEdit.AllowCollectionView && !IsServerMode && OwnerEdit.UseCustomItems);
		}
		protected Locker SelectionLocker { get; private set; }
		protected Locker SynchronizationLocker { get; private set; }
		protected SelectionEventMode SelectionEventMode { get; set; }
		Locker InnerItemSynchronizationLocker { get; set; }
		Locker SelectionChangedLocker { get; set; }
		PostponedAction SelectAllAction { get; set; }
		bool IsServerMode { get { return OwnerEdit.ItemsProvider.IsServerMode || OwnerEdit.ItemsProvider.IsAsyncServerMode; } }
		public bool AllowItemHighlighting {
			get { return (bool)GetValue(AllowItemHighlightingProperty); }
			set { SetValue(AllowItemHighlightingProperty, value); }
		}
		public ISelectorEdit OwnerEdit {
			get { return (ISelectorEdit)BaseEdit.GetOwnerEdit(this); }
		}
		public void MakeSelectionVisible() {
			if (SelectionMode == System.Windows.Controls.SelectionMode.Single) {
				if (SelectedItem != null)
					ScrollIntoView(SelectedItem);
			}
			else {
				if (SelectedItems != null && SelectedItems.Count > 0) {
					ScrollIntoView(SelectedItems[SelectedItems.Count - 1]);
				}
			}
		}
		public void FocusSelectedItem() {
			int index = SelectedIndex;
			if (index == -1 && Items.Count > 0)
				index = 0;
			if (index == -1)
				return;
			ListBoxItem item = ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
			if (item != null)
				item.Focus();
		}
		public bool? IsSelectAll { get { return ItemsSourceHelper.IsSelectAll(); } }
		protected virtual internal void NotifyItemMouseMove(ListBoxEditItem item, MouseEventArgs e) {
			SelectItemOnMouseMove(item, e);
			SetLastMousePosition(GetPosition(item, e));
		}
		void SetLastMousePosition(Point p) {
			lastMousePosition = p;
		}
		void SelectItemOnMouseMove(ListBoxEditItem item, MouseEventArgs e) {
			if (SelectionEventMode != SelectionEventMode.MouseEnter || !HasMouseMoved(item, e))
				return;
			if (!IsItemVisible(item))
				return;
			item.SetCurrentValue(ListBoxItem.IsSelectedProperty, true);
		}
		bool IsItemVisible(ListBoxEditItem item) {
			return true;
		}
		bool HasMouseMoved(ListBoxEditItem item, MouseEventArgs e) {
			return lastMousePosition != UninitializedMousePosition && GetPosition(item, e) != lastMousePosition;
		}
		static readonly Point UninitializedMousePosition = new Point(double.NegativeInfinity, double.NegativeInfinity);
		static readonly Point EditBoxMousePosition = new Point(double.PositiveInfinity, double.PositiveInfinity);
		public void SetEditBoxMousePosition() {
			this.lastMousePosition = EditBoxMousePosition;
		}
		Point lastMousePosition = UninitializedMousePosition;
		protected override void OnMouseLeave(MouseEventArgs e) {
			SetLastMousePosition(UninitializedMousePosition);
			base.OnMouseLeave(e);
		}
		Point GetPosition(ListBoxEditItem item, MouseEventArgs e) {
			return e.GetPosition(LayoutHelper.FindParentObject<ListBox>(item));
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ListBoxEditItem();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			SynchronizationLocker.DoLockedAction(() => PrepareContainerForItemOverrideInternal(element, item));
		}
		protected virtual void PrepareContainerForItemOverrideInternal(DependencyObject element, object item) {
			ListBoxItem listBoxItem = element as ListBoxItem;
			if (listBoxItem == null)
				return;
			if (CustomItemHelper.IsCustomItem(item))
				SetupCustomItem(listBoxItem, (ICustomItem)item);
			if (CustomItemHelper.IsTemplatedItem(item))
				UpdateTemplatedItem(listBoxItem, (CustomItem)item);
			if (item is DataProxy)
				UpdateServerModeItem(listBoxItem, (DataProxy)item);
			if (item == null) {
				listBoxItem.Template = null;
			}
		}
		void UpdateServerModeItem(ListBoxItem element, DataProxy item) {
			IServerModeCollectionView visibleListWrapper = (IServerModeCollectionView)ItemsSource;
			int visibleIndex = ItemContainerGenerator.IndexFromContainer(element);
			visibleListWrapper.FetchItem(visibleIndex);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			SynchronizationLocker.DoLockedAction(() => ClearContainerForItemOverrideInternal(element, item));
		}
		protected virtual void ClearContainerForItemOverrideInternal(DependencyObject element, object item) {
			ListBoxItem listBoxItem = element as ListBoxItem;
			if (listBoxItem == null)
				return;
			if (item == null || CustomItemHelper.IsTemplatedItem(item))
				ClearTemplatedItem(listBoxItem, (CustomItem)item);
			if (item is DataProxy)
				ClearServerModeItem(listBoxItem, (DataProxy)item);
		}
		void ClearServerModeItem(ListBoxItem listBoxItem, DataProxy item) {
			IServerModeCollectionView visibleListWrapper = ItemsSource as IServerModeCollectionView;
			if (visibleListWrapper == null)
				return;
			int visibleIndex = ItemContainerGenerator.IndexFromContainer(listBoxItem);
			visibleListWrapper.CancelItem(visibleIndex);
		}
		protected virtual void SetupCustomItem(ListBoxItem element, ICustomItem item) {
			ClearTemplateSelector(element);
			element.ContentTemplate = CustomItemContentTemplate;
		}
		protected virtual void UpdateTemplatedItem(ListBoxItem element, CustomItem itemEx) {
			ClearTemplateSelector(element);
			itemEx.SetOwnerEdit(OwnerEdit);
			element.ContentTemplate = itemEx.ItemTemplate;
			element.Style = itemEx.ItemContainerStyle;
		}
		protected virtual void ClearTemplatedItem(ListBoxItem element, CustomItem item) {
			element.ClearValue(ContentControl.ContentTemplateProperty);
			element.ClearValue(StyleProperty);
		}
		void ClearTemplateSelector(ListBoxItem element) {
			element.ClearValue(ContentControl.ContentTemplateSelectorProperty);
		}
		protected internal virtual void SyncWithOwnerEditWithSelectionLock(bool updateSource) {
			if (OwnerEdit == null)
				return;
			SynchronizationLocker.DoIfNotLocked(() => SyncWithOwnerEdit(updateSource));
		}
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			SelectionChangedLocker.DoLockedAction(() => { base.OnSelectionChanged(e); });
			SelectionChangedLocker.DoIfNotLocked(() => { SelectAllAction.PerformForce(); });
			OnSelectionChangedInternal(e);
			MakeSelectionVisible();
			ClearHighlightedText();
		}
		protected virtual void OnSelectionChangedInternal(SelectionChangedEventArgs e) {
			if (OwnerEdit == null)
				return;
			SynchronizationLocker.DoLockedActionIfNotLocked(() => {
				var visibleListSource = ItemsSource as VisibleListWrapper;
				if (!visibleListSource.If(x => x.SelectionLocker.IsLocked).ReturnSuccess())
					OwnerEdit.EditStrategy.SyncWithEditor();
				if (OwnerEdit != null) {
					SelectedItemsReusingHelper.SetSelectedItems(this, OwnerEdit.SelectedItems);
#if !SL
					SyncSelectAll(e);
#endif
				}
			});
		}
		protected virtual void SyncSelectAll(SelectionChangedEventArgs e) {
			if (SelectionMode == SelectionMode.Single || SelectionViewModel == null)
				return;
			if (ContainsSelectAllItem(e.AddedItems))
				SelectionViewModel.SelectAll = true;
			else if (ContainsSelectAllItem(e.RemovedItems))
				SelectionViewModel.SelectAll = false;
			else
				SelectionViewModel.SetSelectAllWithoutUpdates(ItemsSourceHelper.IsSelectAll());
		}
		bool ContainsSelectAllItem(IList source) {
			return source.Cast<object>().OfType<SelectAllItem>().Any();
		}
		protected internal void SyncWithOwnerEdit(bool updateSource) {
			if (OwnerEdit == null)
				return;
			SynchronizationLocker.DoLockedActionIfNotLocked(() => SyncWithOwnerEditPropertiesInternal(updateSource));
		}
		protected internal virtual void SyncWithOwnerEditPropertiesInternal(bool updateSource) {
			SetPropertiesFromStyleSettings();
			if (updateSource)
				SelectionLocker.DoLockedAction(SyncItemsSource);
			SyncValuesWithOwnerEdit(true);
		}
		protected internal virtual void SyncValuesWithOwnerEdit(bool resetTotals) {
			SyncSelectedItems(resetTotals);
			SyncItemsProperty();
		}
		private void ClearHighlightedText() {
			if (!clearHighlightedTextLocker.IsLocked && !string.IsNullOrEmpty((string)GetValue(TextBlockService.HighlightTextProperty))) {
				SetHighlightedText(string.Empty);
			}
		}
		private void SetHighlightedText(string text) {
			var edit = OwnerEdit as DependencyObject;
			if (edit != null)
				edit.SetValue(TextBlockService.HighlightTextProperty, text);
		}
		protected virtual void SyncItemsProperty() {
			if (OwnerEdit != null && !IsServerMode)
				OwnerEdit.Items.UpdateSelection(GetSelectedItemsFromEditor());
		}
		object GetSelectedItemsFromEditor() {
			return ((ISelectorEditStrategy)OwnerEdit.EditStrategy).GetSelectedItems(OwnerEdit.EditStrategy.EditValue);
		}
		protected virtual void SetPropertiesFromStyleSettings() {
			SelectionMode = OwnerEdit.SelectionMode;
			SelectionEventMode = OwnerEdit.SelectionEventMode;
			AssignGroupStyle();
		}
		protected virtual void AssignGroupStyle() {
#if !SL
			if (OwnerEdit.GroupStyle.SequenceEqual<GroupStyle>(GroupStyle))
				return;
			GroupStyle.Clear();
			foreach (GroupStyle groupStyle in OwnerEdit.GroupStyle)
				GroupStyle.Add(groupStyle);
#endif
		}
		protected virtual void SyncItemsSource() {
			ResetItemsSourceHelper();
			ItemsSourceHelper.AssignItemsSource();
		}
		protected virtual void ResetItemsSourceHelper() {
			itemsSourceHelper = null;
		}
		protected virtual void SyncItemsSourceInternal() {
		}
		protected void SyncSelectedItems(bool updateTotals) {
			if (OwnerEdit == null)
				return;
			if (SelectionMode == SelectionMode.Single) {
				if (IsServerMode) {
					object editValue = EditStrategy.GetCurrentEditableValue();
					SelectedItem = GetSelectedItemByEditValueInServerMode(editValue);
				}
				else {
					SelectedItem = LookUpEditHelper.GetSelectedItem(OwnerEdit);
				}
				return;
			}
			SyncSelectedItemsInternal(updateTotals);
			IEnumerable<object> selectedItems;
			if (IsServerMode) {
				var totalSelectedValues = LookUpEditHelper.GetEditValue(OwnerEdit) as IEnumerable<object>;
				selectedItems = totalSelectedValues == null ? new List<object>() : totalSelectedValues.Select(GetSelectedItemByEditValueInServerMode).Where(x => x != null).ToList();
				SetSelectedItems(selectedItems);
			}
			else {
				var totalSelectedItems = CustomItem.FilterCustomItems(GetSelectedItems());
				selectedItems = Items.Cast<object>().Intersect(totalSelectedItems).ToList();
				if (!SelectedItemsReusingHelper.ShouldReuseSelectedItems(this, selectedItems)) {
					SetSelectedItems(selectedItems);
					SelectedItemsReusingHelper.SetSelectedItems(this, selectedItems);
				}
			}
			SelectionViewModel.SetSelectAllWithoutUpdates(ItemsSourceHelper.IsSelectAll());
		}
		object GetSelectedItemByEditValueInServerMode(object editValue) {
			var visibleListWrapper = ItemsSource as IServerModeCollectionView;
			if (visibleListWrapper == null)
				return -1;
			int index = visibleListWrapper.IndexOfValue(editValue);
			return index > -1 ? visibleListWrapper.GetItem(index) : null;
		}
		protected virtual void SyncSelectedItemsInternal(bool updateTotals) {
		}
		public virtual IEnumerable<object> GetSelectedItems() {
			return GetSelectedItemsInternal();
		}
		protected virtual IEnumerable<object> GetSelectedItemsInternal() {
			return OwnerEdit.GetCurrentSelectedItems().Cast<object>();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (OwnerEdit.EditMode == EditMode.InplaceActive)
				OnInplaceKeyDown(e);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (IsEditorReadOnly() && IsNavKey(e.Key)) {
				e.Handled = true;
				return;
			}
			if (ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				if (e.Key == Key.Down) {
					NavigateToNextSearchedItem();
					e.Handled = true;
				}
				else if (e.Key == Key.Up) { 
					NavigateToPrevSearchedItem();
					e.Handled = true;
				}
			}
			if (e.Key == Key.Escape) {
				e.Handled = true;
				ClearHighlightedText();
			}
		}
		void NavigateToPrevSearchedItem() {
			clearHighlightedTextLocker.DoLockedAction(() => {
				var value = EditStrategy.GetPrevValueFromSearchText(GetFocusedItemIndex());
				if (value != null)
					FocusSearchResult(value);
			});
		}
		void NavigateToNextSearchedItem() {
			clearHighlightedTextLocker.DoLockedAction(() => {
				var value = EditStrategy.GetNextValueFromSearchText(GetFocusedItemIndex());
				if (value != null)
					FocusSearchResult(value);
			});
		}
		protected bool IsEditorReadOnly() {
			return OwnerEdit != null && OwnerEdit.IsReadOnly;
		}
		protected virtual void OnInplaceKeyDown(KeyEventArgs e) {
			if (SelectionMode == SelectionMode.Multiple)
				return;
			if (ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && IsNavKey(e.Key)) {
#if !SL
				ListBoxEditItem container = Keyboard.FocusedElement as ListBoxEditItem;
#else 
				ListBoxEditItem container = FocusManager.GetFocusedElement() as ListBoxEditItem;
#endif
				if (container != null) {
					int index = ItemContainerGenerator.IndexFromContainer(container);
					if (index > -1)
						SelectedIndex = index;
				}
			}
		}
		int GetFocusedItemIndexCore() {
			ListBoxEditItem container = Keyboard.FocusedElement as ListBoxEditItem;
			return container != null ? ItemContainerGenerator.IndexFromContainer(container) : -1;
		}
		protected bool IsNavKey(Key key) {
			return key == Key.Left || key == Key.Right || key == Key.Down || key == Key.Up || key == Key.PageDown || key == Key.PageDown;
		}
		protected internal virtual void OnInnerItemContentChanged(ListBoxItem container) {
			if (OwnerEdit == null)
				return;
			object item = ItemContainerGenerator.ItemFromContainer(container);
			var listNotification = OwnerEdit.ListNotificationOwner;
			var listChangedEventArgs = new NotifyItemsProviderChangedEventArgs(ListChangedType.ItemChanged, item);
			InnerItemSynchronizationLocker.DoLockedActionIfNotLocked(() => listNotification.OnCollectionChanged(listChangedEventArgs));
		}
		void EditorListBoxLoaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, true);
		}
		void EditorListBoxUnloaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);
		}
		internal void NotifyOwnerSelectionChanged(ListBoxEditItem listBoxEditItem, bool isSelected) {
		}
		#region ISelectorEditInnerListBox Members
		ISelectorEditInnerListBox SelectorEditInnerListBox { get { return this as ISelectorEditInnerListBox; } }
		ICustomItem ISelectorEditInnerListBox.GetCustomItem(Func<object, bool> getNeedItem) {
			foreach (ICustomItem item in ItemsSourceHelper.CustomItemsSource) {
				if (getNeedItem(item))
					return item;
			}
			return null;
		}
		void ISelectorEditInnerListBox.SelectAll() {
#if !SL
			if (SynchronizationLocker.IsLocked)
				return;
#endif
			SelectAllAction.PerformPostpone(() => { SelectAll(); });
		}
		void ISelectorEditInnerListBox.UnselectAll() {
#if SL
			SelectedItems.Clear();
#else
			if (SynchronizationLocker.IsLocked)
				return;
			SelectAllAction.PerformPostpone(() => { UnselectAll(); });
#endif
		}
		bool ISelectorEditInnerListBox.IsCustomItem(ICustomItem customItem) {
			ICustomItem item = SelectorEditInnerListBox.GetCustomItem((current) => current == customItem);
			return item != null;
		}
#if DEBUGTEST
		int ISelectorEditInnerListBox.ItemsSourceCount { get { return SelectorEditInnerListBox.ContentItemsSource.Cast<object>().Count<object>() + SelectorEditInnerListBox.CustomItemsSource.Cast<object>().Count<object>(); } }
		IEnumerable ISelectorEditInnerListBox.ContentItemsSource { get { return ItemsSourceHelper.ContentItemsSource; } }
		IEnumerable ISelectorEditInnerListBox.CustomItemsSource { get { return ItemsSourceHelper.CustomItemsSource; } }
		string PrintItems(IEnumerable<object> items) {
			string result = "";
			if (items == null)
				return result;
			foreach (var item in items)
				result += item.ToString() + " ";
			return result;
		}
#endif
		#endregion
	}
	[ToolboxItem(false)]
	public partial class PopupListBox : EditorListBox {
		readonly PopupListBoxNavigationHelper navigationHelper;
		public PopupListBox()
			: base() {
			this.SetDefaultStyleKey(typeof(PopupListBox));
			Focusable = false;
			this.navigationHelper = new PopupListBoxNavigationHelper(this);
#if SL
			ConstructorSLPart();
#endif
		}
		IEnumerable<object> TotalSelectedItems { get; set; }
		protected PopupListBoxNavigationHelper NavigationHelper { get { return navigationHelper; } }
		protected bool IsSelectionLocked { get { return SelectionLocker.IsLocked; } }
		bool CloseOnMouseUp { get; set; }
		bool CloseUsingDispatcher { get; set; }
		LookUpEditBasePropertyProvider PropertyProvider {
			get { return (LookUpEditBasePropertyProvider)ActualPropertyProvider.GetProperties(OwnerEdit); }
		}
		protected override void SetPropertiesFromStyleSettings() {
			BaseComboBoxStyleSettings settings = (BaseComboBoxStyleSettings)OwnerEdit.PropertyProvider.StyleSettings;
			if (ItemContainerStyle == null)
				ItemContainerStyle = settings.GetItemContainerStyle(OwnerEdit);
			SelectionMode = settings.GetSelectionMode(OwnerEdit);
			SelectionEventMode = settings.GetSelectionEventMode(OwnerEdit);
			CloseOnMouseUp = settings.GetClosePopupOnMouseUp(OwnerEdit);
			CloseUsingDispatcher = settings.CloseUsingDispatcher;
			AssignGroupStyle();
		}
		internal void SetupEditor() {
			SelectionLocker.DoLockedAction(() => {
				UpdateTotals();
				SyncWithOwnerEditWithSelectionLock(true);
			});
		}
		void UpdateTotals() {
			TotalSelectedItems = ((ISelectorEdit)OwnerEdit).GetCurrentSelectedItems().Cast<object>();
		}
		protected internal override void SyncWithOwnerEditWithSelectionLock(bool updateSource) {
			base.SyncWithOwnerEditWithSelectionLock(updateSource);
			MakeVisibleIfNeeded();
		}
		protected override IEnumerable<object> GetSelectedItemsInternal() {
			if (OwnerEdit.EditStrategy.IsLockedByValueChanging)
				return base.GetSelectedItemsInternal();
			return TotalSelectedItems ?? new List<object>();
		}
		protected override void SyncItemsSourceInternal() {
			base.SyncItemsSourceInternal();
			if (SelectionMode != SelectionMode.Single) {
				SelectedItems.Clear();
				SelectedItemsReusingHelper.ClearSelectedItems(this);
			}
		}
		void MakeVisibleIfNeeded() {
			if (OwnerEdit.EditStrategy.StyleSettings.GetActualScrollToSelectionOnPopup(OwnerEdit) || (OwnerEdit.Settings.AutoComplete && OwnerEdit.Settings.ImmediatePopup))
				MakeSelectionVisible();
		}
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			if (OwnerEdit == null || !OwnerEdit.IsPopupOpen)
				return;
			SelectionLocker.DoLockedActionIfNotLocked(() => LookUpEditHelper.RaisePopupContentSelectionChangedEvent(
				OwnerEdit, CustomItem.FilterCustomItems(e.RemovedItems.Cast<object>()).ToList(), CustomItem.FilterCustomItems(e.AddedItems.Cast<object>()).ToList()));
			TotalSelectedItems = TotalSelectedItems.Union(CustomItem.FilterCustomItems(e.AddedItems.Cast<object>()));
			SelectionLocker.DoLockedActionIfNotLocked(() => {
				TotalSelectedItems = TotalSelectedItems.Except(CustomItem.FilterCustomItems(e.RemovedItems.Cast<object>()));
			});
			MakeVisibleIfNeeded();
		}
		protected override void OnSelectionChangedInternal(SelectionChangedEventArgs e) {
			SynchronizationLocker.DoLockedActionIfNotLocked(() => SyncSelectAll(e));
		}
		protected override void SyncSelectedItemsInternal(bool updateTotals) {
			base.SyncSelectedItemsInternal(updateTotals);
			if (updateTotals)
				UpdateTotals();
		}
		public new ComboBoxEdit OwnerEdit {
			get { return base.OwnerEdit as ComboBoxEdit; }
		}
		internal bool ProcessDownKey(KeyEventArgs e) {
			if (IsEditorReadOnly())
				return false;
			ModifierKeys mod = ModifierKeysHelper.GetKeyboardModifiers(e);
			if (ModifierKeysHelper.IsAltPressed(mod))
				return false;
			if (!((ISelectorEditStrategy)OwnerEdit.EditStrategy).IsSingleSelection)
				return false;
			Key key = e.Key;
#if !SL
			if (e.Key == Key.System)
				key = e.SystemKey;
#endif
			switch (key) {
				case Key.Enter:
					break;
				case Key.Up:
					e.Handled = true;
					NavigationHelper.MovePrev();
					break;
				case Key.Down:
					e.Handled = true;
					NavigationHelper.MoveNext();
					break;
				case Key.Home:
					e.Handled = true;
					NavigationHelper.MoveFirst();
					break;
				case Key.End:
					e.Handled = true;
					NavigationHelper.MoveLast();
					break;
#if !SL
				case Key.PageDown:
					e.Handled = true;
					NavigationHelper.MovePageDown();
					break;
				case Key.PageUp:
					e.Handled = true;
					NavigationHelper.MovePageUp();
					break;
#endif
			}
			MakeSelectionVisible();
			return e.Handled;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ComboBoxEditItem();
		}
		protected override void OnInplaceKeyDown(KeyEventArgs e) { }
		internal void NotifyComboBoxItemMouseUp(ComboBoxEditItem item) {
			if (!OwnerEdit.IsReadOnly)
				CloseOwnerEditPopupIfNeeded(item);
		}
		void CloseOwnerEditPopupIfNeeded(ComboBoxEditItem item) {
			if (OwnerEdit != null) {
				if (CloseOnMouseUp && CanCloseOnMouseUp(item)) {
					if (CloseUsingDispatcher)
						Dispatcher.BeginInvoke(new Action(() => OwnerEdit.Do(x => x.ClosePopup())));
					else
						OwnerEdit.ClosePopup();
				}
			}
		}
		bool CanCloseOnMouseUp(ComboBoxEditItem item) {
			if (OwnerEdit == null)
				return false;
			if (!OwnerEdit.ItemsProvider.IsServerMode)
				return true;
			var visibleListWrapper = (IServerModeCollectionView)ItemsSource;
			int visibleIndex = ItemContainerGenerator.IndexFromContainer(item);
			return !visibleListWrapper.IsTempItem(visibleIndex);
		}
		internal void NotifyComboBoxItemMouseDown(ComboBoxEditItem item) {
			if (OwnerEdit != null && SelectionEventMode == SelectionEventMode.MouseDown)
				item.SetCurrentValue(ListBoxItem.IsSelectedProperty, true);
		}
	}
	#region PopupListBoxNavigationHelper
	public class PopupListBoxNavigationHelper {
		public PopupListBoxNavigationHelper(PopupListBox listBox) {
			ListBox = listBox;
		}
		public PopupListBox ListBox { get; private set; }
		protected ItemCollection Items { get { return ListBox.Items; } }
		protected int SelectedIndex { get { return ListBox.SelectedIndex; } }
		public void MoveLast() {
			Move(Items.Count - 1, -1, -1);
		}
		public void MoveFirst() {
			Move(0, Items.Count, 1);
		}
		public void MoveNext() {
			Move(SelectedIndex + 1, Items.Count, 1);
		}
		public void MovePrev() {
			Move(Math.Max(0, SelectedIndex - 1), -1, -1);
		}
		protected virtual void Move(int startIndex, int stopIndex, int delta) {
			if (Items.Count == 0)
				return;
			for (int i = startIndex; i != stopIndex; i += delta) {
				object item = Items[i];
				DependencyObject container = ListBox.ItemContainerGenerator.ContainerFromIndex(i);
				if (CanSelectItem(item) && CanSelectItem(container)) {
					ListBox.SelectedIndex = i;
					ListBox.If(x => x.IsKeyboardFocusWithin).Do(x => x.FocusContainer(container));
					break;
				}
			}
		}
		protected virtual bool CanSelectItem(object item) {
			UIElement d = item as UIElement;
			return (d == null) || UIElementHelper.IsEnabled(d);
		}
		void DoNavigationAction(bool down, Action upAction, Action downAction) {
			if (down)
				downAction();
			else
				upAction();
		}
		ScrollViewer Find(DependencyObject obj) {
			while (obj != null) {
				ScrollViewer res = obj as ScrollViewer;
				if (res != null)
					return res;
				obj = VisualTreeHelper.GetParent(obj);
			}
			return null;
		}
#if !SL
		public void MovePageUp() {
			MovePage(false);
		}
		public void MovePageDown() {
			MovePage(true);
		}
		protected virtual void MovePage(bool down) {
			int index = SelectedIndex < 0 ? 0 : SelectedIndex;
			if (index >= Items.Count)
				return;
			ScrollViewer sv = Find(ListBox.ItemContainerGenerator.ContainerFromIndex(index));
			if (sv == null)
				return;
			if (!sv.CanContentScroll) {
				DoNavigationAction(down, MoveFirst, MoveLast);
				return;
			}
			DoNavigationAction(down, sv.PageUp, sv.PageDown);
			sv.UpdateLayout();
			if (down)
				Move((int)(sv.VerticalOffset + sv.ViewportHeight) - 1, -1, -1);
			else
				Move((int)sv.VerticalOffset, Items.Count, 1);
		}
#endif
	}
	#endregion
	public class SelectionViewModel : FrameworkElement {
		public static readonly DependencyProperty SelectAllProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		static SelectionViewModel() {
			Type ownerType = typeof(SelectionViewModel);
			SelectAllProperty = DependencyPropertyManager.Register("SelectAll", typeof(bool?), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((SelectionViewModel)d).SelectAllChanged((bool?)e.NewValue)));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((SelectionViewModel)d).IsSelectedChanged((bool)e.NewValue)));
		}
		public bool? SelectAll {
			get { return (bool?)GetValue(SelectAllProperty); }
			set { SetValue(SelectAllProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		Func<ISelectorEditInnerListBox> GetListBox { get; set; }
		Locker SelectAllLocker { get; set; }
		public SelectionViewModel(Func<ISelectorEditInnerListBox> getListBox) {
			GetListBox = getListBox;
			SelectAllLocker = new Locker();
		}
		protected virtual void IsSelectedChanged(bool newValue) {
			SelectAllLocker.DoLockedActionIfNotLocked(() => SelectionChanged(newValue));
		}
		protected virtual void SelectAllChanged(bool? newValue) {
			SelectAllLocker.DoLockedActionIfNotLocked(() => SelectionChanged(newValue));
		}
		public virtual void SetSelectAllWithoutUpdates(bool? selectAll) {
			SelectAllLocker.DoLockedAction(() => {
				SelectAll = selectAll;
				if (selectAll.HasValue) {
					IsSelected = selectAll.Value;
				}
			});
		}
		public virtual void SelectionChanged(bool? isSelectAll) {
			var listBox = GetListBox();
			if (listBox == null)
				return;
			if (!isSelectAll.HasValue)
				return;
			if (isSelectAll.Value)
				listBox.SelectAll();
			else
				listBox.UnselectAll();
		}
	}
}
namespace DevExpress.Xpf.Editors {
	public abstract partial class ListBoxEditItemBase : ListBoxItem {
		protected internal virtual void SubscribeToSelection() {
			SubscribeToSelectionInternal();
		}
		protected internal virtual void UnsubscribeFromSelection() {
			UnsubscribeFromSelectionInternal();
		}
		partial void SubscribeToSelectionInternal();
		partial void UnsubscribeFromSelectionInternal();
	}
	public class ListBoxRadioButton : RadioButton {
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = false;
		}
	}
	public partial class ListBoxEditItem : ListBoxEditItemBase {
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty InternalAllowItemHighlightingProperty;
		static ListBoxEditItem() {
			Type ownerType = typeof(ListBoxEditItem);
			InternalAllowItemHighlightingProperty = DependencyPropertyManager.Register("InternalAllowItemHighlighting", typeof(bool), ownerType,
				new PropertyMetadata((d, e) => ((ListBoxEditItem)d).PropertyChangedInternalAllowItemHighlighting()));
		}
		public ListBoxEditItem() {
			this.SetDefaultStyleKey(typeof(ListBoxEditItem));
#if SL
			ConstructorSLPart();
#endif
			UpdateContentAction = new PostponedAction(() => NotifyOwner == null);
			UpdateSelectionAction = new PostponedAction(() => NotifyOwner == null);
			SelectionLocker = new Locker();
		}
		IListNotificationOwner notifyOwner;
		internal IListNotificationOwner NotifyOwner {
			get { return notifyOwner; }
			set {
				notifyOwner = value;
				ProcessNotifyOwner(IsSelected);
			}
		}
		PostponedAction UpdateContentAction { get; set; }
		PostponedAction UpdateSelectionAction { get; set; }
		Locker SelectionLocker { get; set; }
		public EditorListBox Owner { get { return ItemsControl.ItemsControlFromItemContainer(this) as EditorListBox; } }
		bool InternalAllowItemHighlighting { get { return (bool)GetValue(InternalAllowItemHighlightingProperty); } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateContentAction.Perform();
			if (Owner != null)
				SetBinding(InternalAllowItemHighlightingProperty, new Binding(EditorListBox.AllowItemHighlightingProperty.GetName()) { Source = Owner });
			UpdateVisualState(false);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			e.SetHandled(true);
			Owner.Do(x => x.NotifyItemMouseMove(this, e));
			base.OnMouseMove(e);
		}
		protected virtual void OnIsMouseOverChanged() {
			UpdateVisualState(true);
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if (Owner != null && Owner.OwnerEdit.IsReadOnly) {
				e.Handled = true;
				return;
			}
			if (e.OriginalSource == null)
				return;
			if (!IsInnerListBox(e.OriginalSource as FrameworkElement)) {
				Owner.If(x => x.IsKeyboardFocusWithin).Do(x => Focus());
			}
		}
		bool IsInnerListBox(FrameworkElement eventSource) {
			if (eventSource == null)
				return false;
			EditorListBox edit = LayoutHelper.FindElement(eventSource, frameworkElement => frameworkElement is EditorListBox) as EditorListBox;
			if (edit != null || edit != Owner)
				return true;
			return false;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			Dispatcher.BeginInvoke(new Action(() => Focus()), new object[] { });
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == IsMouseOverProperty)
				OnIsMouseOverChanged();
		}
		protected virtual void UpdateVisualState(bool useTransitions) {
			VisualStateManager.GoToState(this, IsMouseOver && InternalAllowItemHighlighting ? "Highlighted" : "Unhighlighted", useTransitions);
		}
		void PropertyChangedInternalAllowItemHighlighting() {
			UpdateVisualState(true);
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			UpdateContentAction.PerformPostpone(() => {
				var visibleListWrapper = Owner != null ? Owner.ItemsSource as VisibleListWrapper : null;
				if (visibleListWrapper != null)
					visibleListWrapper.EventLocker.DoLockedAction(() => NotifyOwner.OnCollectionChanged(new NotifyItemsProviderChangedEventArgs(ListChangedType.ItemChanged, this)));
				else
					NotifyOwner.OnCollectionChanged(new NotifyItemsProviderChangedEventArgs(ListChangedType.ItemChanged, this));
			});
		}
		protected override void OnSelected(RoutedEventArgs e) {
			base.OnSelected(e);
			ProcessNotifyOwner(true);
		}
		protected override void OnUnselected(RoutedEventArgs e) {
			base.OnUnselected(e);
			ProcessNotifyOwner(false);
		}
		protected virtual void ProcessNotifyOwner(bool isSelected) {
			UpdateContentAction.Perform();
			if (Owner != null)
				return;
			SelectionLocker.DoLockedActionIfNotLocked(CreateUpdateSelectionAction(isSelected));
		}
		Action CreateUpdateSelectionAction(bool isSelected) {
			Action notifyOwnerAction = () => NotifyOwner.OnCollectionChanged(new NotifyItemsProviderSelectionChangedEventArgs(this, isSelected));
			return () => UpdateSelectionAction.PerformPostpone(notifyOwnerAction);
		}
		protected internal virtual void ChangeSelectionWithLock(bool isSelected) {
			SelectionLocker.DoLockedAction(() => SetCurrentValue(IsSelectedProperty, isSelected));
		}
	}
	[ToolboxItem(false)]
	public partial class ComboBoxEditItem : ListBoxEditItem {
		public ComboBoxEditItem() {
			Unloaded += OnUnloaded;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			if (NotifyOwner == null)
				SetCurrentValue(IsSelectedProperty, false);
		}
		protected PopupListBox ParentListBox { get { return Owner as PopupListBox; } }
		protected override void OnMouseEnter(MouseEventArgs e) {
			e.SetHandled(true);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if (Owner != null)
				ParentListBox.NotifyComboBoxItemMouseUp(this);
			base.OnMouseLeftButtonUp(e);
			e.SetHandled(true);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if (Owner != null)
				ParentListBox.NotifyComboBoxItemMouseDown(this);
			base.OnMouseLeftButtonDown(e);
			e.SetHandled(true);
		}
	}
}
namespace DevExpress.Xpf.Editors.Internal {
	[DXToolboxBrowsable(false)]
	public class ImagePresenter : Control {
		const string ImageName = "PART_Image";
		public ImagePresenter() {
			DataContextChanged += ListBoxEditItemImageHolderDataContextChanged;
			Focusable = false;
		}
		public Image Image { get; protected set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Image = GetTemplateChild(ImageName) as Image;
			ApplyImageProperties();
		}
		void ListBoxEditItemImageHolderDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ApplyImageProperties();
		}
		void ApplyImageProperties() {
			if (Image == null)
				return;
			EnumMemberInfo info = DataContext as EnumMemberInfo;
			if (info != null && info.ShowImage) {
				Image.Source = info.Image;
				Image.Visibility = info.Image != null ? Visibility.Visible : Visibility.Collapsed;
			}
			else {
				Image.Visibility = Visibility.Collapsed;
			}
			Image.Margin = Padding;
		}
		static ImagePresenter() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePresenter), new FrameworkPropertyMetadata(typeof(ImagePresenter)));
		}
	}
	public class EditItemContentPresenter : ContentPresenter {
		public static readonly DependencyProperty VerticalContentAlignmentProperty;
		public VerticalAlignment VerticalContentAlignment {
			get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
			set { SetValue(VerticalContentAlignmentProperty, value); }
		}
		static DataTemplate StringContentTemplate { get; set; }
		static EditItemContentPresenter() {
			VerticalContentAlignmentProperty = DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(EditItemContentPresenter), new PropertyMetadata(VerticalAlignment.Center));
			FrameworkElementFactory factory = new FrameworkElementFactory { Type = typeof(TextBlock) };
			factory.SetValue(VerticalAlignmentProperty, new TemplateBindingExtension(VerticalContentAlignmentProperty));
			factory.SetBinding(TextBlock.TextProperty, new Binding() { RelativeSource = RelativeSource.TemplatedParent, Path = new PropertyPath(ContentProperty), Converter = ContentTemplateConverter.Instance });
			StringContentTemplate = new DataTemplate { VisualTree = factory };
			StringContentTemplate.Seal();
		}
		protected override DataTemplate ChooseTemplate() {
			DataTemplate baseTemplate = base.ChooseTemplate();
			if (baseTemplate.GetType().Name != "DefaultTemplate")
				return baseTemplate;
			return StringContentTemplate;
		}
	}
	public class ContentTemplateConverter : IValueConverter {
		static ContentTemplateConverter instance;
		public static ContentTemplateConverter Instance {
			get {
				if (instance == null) {
					instance = new ContentTemplateConverter();
				}
				return instance;
			}
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return RenderTriggerHelper.GetConvertedValue(targetType, value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PopupListBoxDisplayMemberPathConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string)
				return GetDisplayMemberPathCore((string)value);
			ISelectorEdit selector = value as ISelectorEdit;
			if (selector != null && selector.ItemTemplate == null)
				return GetDisplayMemberPathCore(selector.DisplayMember);
			return GetDisplayMemberPathCore(string.Empty);
		}
		string GetDisplayMemberPathCore(string value) {
			return value == "" ? null : value;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DummySelectionProvider : ISelectionProvider {
		#region ISelectionProvider Members
		public void SelectAll() {
		}
		public void UnselectAll() {
		}
		public void SetSelectAll(bool? value) {
		}
		#endregion
	}
	public class SelectionProvider : ISelectionProvider {
		ISelectorEditInnerListBox ListBox { get; set; }
		public SelectionProvider(ISelectorEditInnerListBox listBox) {
			ListBox = listBox;
		}
		#region ISelectionProvider Members
		public void SelectAll() {
			ListBox.SelectAll();
		}
		public void UnselectAll() {
			ListBox.UnselectAll();
		}
		public void SetSelectAll(bool? value) {
		}
		#endregion
	}
	public class SelectedItemsReusingHelper : DependencyObject {
		public static readonly DependencyProperty SelectedItemsProperty;
		static readonly DependencyPropertyKey SelectedItemsPropertyKey;
		static SelectedItemsReusingHelper() {
			Type ownerType = typeof(SelectedItemsReusingHelper);
			SelectedItemsPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("SelectedItems", typeof(IEnumerable), ownerType, new PropertyMetadata(null));
			SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;
		}
		public static IEnumerable GetSelectedItems(DependencyObject d) {
			return (IEnumerable)d.GetValue(SelectedItemsProperty);
		}
		internal static void SetSelectedItems(DependencyObject d, IEnumerable value) {
			d.SetValue(SelectedItemsPropertyKey, ToComparable(value));
		}
		public static void ClearSelectedItems(DependencyObject d) {
			d.ClearValue(SelectedItemsPropertyKey);
		}
		public static bool ShouldReuseSelectedItems(DependencyObject d, IEnumerable selectedItems) {
			List<object> reuseItems = (List<object>)GetSelectedItems(d);
			if (reuseItems == null || selectedItems == null)
				return false;
			return reuseItems.SequenceEqual<object>(ToComparable(selectedItems));
		}
		static IEnumerable<object> ToComparable(IEnumerable source) {
			return source.Cast<object>().ToList();
		}
	}
}
