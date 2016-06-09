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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.EditStrategy;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public class ListBoxEditStrategy : EditStrategyBase, ISelectorEditStrategy {
		readonly SelectorPropertiesCoercionHelper selectorUpdater = new SelectorPropertiesCoercionHelper();
		readonly Locker selectionChangedLocker = new Locker();
		EditorListBox ListBox { get { return Editor.ListBoxCore; } }
		new ListBoxEdit Editor { get { return (ListBoxEdit)base.Editor; } }
		new ListBoxEditSettings Settings { get { return Editor.Settings; } }
		new ListBoxEditStyleSettings StyleSettings { get { return (ListBoxEditStyleSettings)base.StyleSettings; } }
		public bool IsInLookUpMode { get { return ItemsProvider.IsInLookUpMode; } }
		public bool IsAsyncServerMode { get { return ItemsProvider.IsAsyncServerMode; } }
		public bool IsSyncServerMode { get { return ItemsProvider.IsSyncServerMode; } }
		public bool IsServerMode { get { return IsSyncServerMode || IsAsyncServerMode; } }
		TextSearchEngine TextSearchEngine { get; set; }
		bool HasHighlightedText { get { return !string.IsNullOrEmpty((string)Editor.GetValue(TextBlockService.HighlightTextProperty)); } }
		EditorTextSearchHelper textSearchHelper;
		EditorTextSearchHelper TextSearchHelper {
			get {
				if (textSearchHelper == null)
					textSearchHelper = new EditorTextSearchHelper(this);
				return textSearchHelper;
			}
		}
		IItemsProvider2 ItemsProvider { get { return Editor.ItemsProvider; } }
		bool IsSingleSelection { get { return Editor.Settings.SelectionMode == SelectionMode.Single; } }
		protected bool ShouldLockSelection { get { return selectionChangedLocker.IsLocked; } }
		protected internal SelectorPropertiesCoercionHelper SelectorUpdater { get { return selectorUpdater; } }
		internal object CurrentDataViewHandle { get { return ItemsProvider.CurrentDataViewHandle; } }
		public ListBoxEditStrategy(ListBoxEdit editor)
			: base(editor) {
			SelectorUpdater.SetOwner(this);
			TextSearchEngine = new TextSearchEngine(TextSearchCallback);
		}
		protected virtual int TextSearchCallback(string prefix, int startIndex, bool ignoreStartIndex) {
			return ItemsProvider.FindItemIndexByText(prefix, false, true, ItemsProvider.CurrentDataViewHandle, startIndex, true, ignoreStartIndex);
		}
		protected internal override bool DoTextSearch(string text, int startIndex, ref object result) {
			return TextSearchHelper.DoTextSearch(text, startIndex, ref result);
		}
		protected virtual object GetNextValueFromSearchTextInternal(int startIndex) {
			return GetValueFromSearchTextCore(startIndex, true);
		}
		object GetValueFromSearchTextCore(int startIndex, bool isDown) {
			return HasHighlightedText ? TextSearchHelper.FindValueFromSearchText(startIndex, isDown, false) : null;
		}
		protected virtual object GetPrevValueFromSearchTextInternal(int startIndex) {
			return GetValueFromSearchTextCore(startIndex, false);
		}
		void SetHighlightedText(string text) {
			ImmediateActionsManager.EnqueueAction(new Action(() => {
				Editor.SetValue(TextBlockService.HighlightTextProperty, text);
			}));
		}
		public override void RaiseValueChangedEvents(object oldValue, object newValue) {
			base.RaiseValueChangedEvents(oldValue, newValue);
			if (ShouldLockRaiseEvents)
				return;
			SelectorUpdater.RaiseSelectedIndexChangedEvent(oldValue, newValue);
		}
		public override void OnLostFocus() {
			base.OnLostFocus();
			ClearHighlightedText();
		}
		void ClearHighlightedText() {
			SetHighlightedText(string.Empty);
		}
		public virtual int CoerceSelectedIndex(int baseIndex) {
			CoerceValue(ListBoxEdit.SelectedIndexProperty, baseIndex);
			return SelectorUpdater.CoerceSelectedIndex(baseIndex);
		}
		public virtual object CoerceSelectedItem(object baseItem) {
			CoerceValue(ListBoxEdit.SelectedItemProperty, baseItem);
			return SelectorUpdater.CoerceSelectedItem(baseItem);
		}
		public virtual void SelectedIndexChanged(int oldValue, int newValue) {
			if (ShouldLockUpdate)
				return;
			SyncWithValue(ListBoxEdit.SelectedIndexProperty, oldValue, newValue);
		}
		public virtual void SelectedItemChanged(object oldValue, object newValue) {
			if (ShouldLockUpdate)
				return;
			SyncWithValue(ListBoxEdit.SelectedItemProperty, oldValue, newValue);
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			SelectorUpdater.SyncICollectionView(ValueContainer.EditValue);
			if (Editor.ListBoxCore != null)
				Editor.ListBoxCore.SyncWithOwnerEditWithSelectionLock(false);
		}
		protected override void SyncWithEditorInternal() {
			if (ListBox == null)
				return;
			var selectedItems = GetSelectedItemsFromListBox();
			if (IsSingleSelection) {
				object selectedItem = selectedItems;
				object filteredItem = SelectorUpdater.FilterSelectedItem(selectedItem);
				if (!IsServerMode && ((CustomItemHelper.IsCustomItem(filteredItem) || ItemsProvider.GetIndexByItem(filteredItem, ItemsProvider.CurrentDataViewHandle) > -1))) {
					ValueContainer.SetEditValue(SelectorUpdater.GetEditValueFromSelectedItem(filteredItem), UpdateEditorSource.TextInput);
					return;
				}
				if (IsServerMode) {
					ValueContainer.SetEditValue(ItemsProvider.GetValueByRowKey(filteredItem, ItemsProvider.CurrentDataViewHandle), UpdateEditorSource.TextInput);
					return;
				}
			}
			var selectedItems2 = (IEnumerable<object>)selectedItems;
			var filteredItems = SelectorUpdater.FilterSelectedItems(selectedItems2);
			if (IsSyncServerMode) {
				var editValue = filteredItems.Select(x => ItemsProvider.GetValueByRowKey(x, ItemsProvider.CurrentDataViewHandle)).ToList();
				ValueContainer.SetEditValue(editValue, UpdateEditorSource.TextInput);
				return;
			}
			if (IsAsyncServerMode) {
				var editValue = filteredItems.Select(x => ItemsProvider.GetValueByRowKey(x, ItemsProvider.CurrentDataViewHandle)).ToList();
				ValueContainer.SetEditValue(editValue, UpdateEditorSource.TextInput);
				return;
			}
			ValueContainer.SetEditValue(SelectorUpdater.GetEditValueFromSelectedItems(filteredItems.With(x => x.ToList())), UpdateEditorSource.TextInput);
		}
		object GetSelectedItemsFromListBox() {
			if (IsSingleSelection)
				return GetSelectedRowKey(ListBox.SelectedItem);
			return ListBox.SelectedItems.Cast<object>().Select(GetSelectedRowKey);
		}
		protected virtual object GetSelectedRowKey(object item) {
			if (!IsServerMode)
				return item;
			DataProxy proxy = (DataProxy)item;
			return proxy.With(x => x.f_RowKey);
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(ListBoxEdit.EditValueProperty, baseValue => baseValue, baseValue => SelectorUpdater.GetEditValueFromBaseValue(baseValue));
			PropertyUpdater.Register(ListBoxEdit.SelectedIndexProperty, baseValue => SelectorUpdater.GetEditValueFromSelectedIndex(baseValue), baseValue => SelectorUpdater.GetIndexFromEditValue(baseValue));
			PropertyUpdater.Register(ListBoxEdit.SelectedItemProperty, baseValue => SelectorUpdater.GetEditValueFromSelectedItem(baseValue), baseValue => SelectorUpdater.GetSelectedItemFromEditValue(baseValue));
			PropertyUpdater.Register(ListBoxEdit.SelectedItemsProperty, baseValue => SelectorUpdater.GetEditValueFromSelectedItems(baseValue), baseValue => SelectorUpdater.GetSelectedItemsFromEditValue(baseValue), baseValue => SelectorUpdater.UpdateSelectedItems(baseValue));
		}
		public virtual void ItemSourceChanged(object itemsSource) {
			Settings.AssignToEditCoreLocker.DoIfNotLocked(() => {
				Settings.ItemsSource = itemsSource;
			});
			EnumItemsSource.SetupEnumItemsSource(itemsSource, () => {
				Editor.SetCurrentValue(ListBoxEdit.ValueMemberProperty, EnumSourceHelperCore.ValueMemberName);
				Editor.SetCurrentValue(ListBoxEdit.DisplayMemberProperty, EnumSourceHelperCore.DisplayMemberName);
			});
			SyncWithValue();
		}
		public virtual void SelectedItemsChanged(IList oldSelectedItems, IList selectedItems) {
			CoerceValue(ListBoxEdit.SelectedItemsProperty, selectedItems);
			if (ShouldLockUpdate)
				return;
			SyncWithValue(ListBoxEdit.SelectedItemsProperty, oldSelectedItems, selectedItems);
		}
		protected override void SyncEditCorePropertiesInternal() {
			base.SyncEditCorePropertiesInternal();
			SyncInnerEditorWithOwnerEdit(true);
		}
		#region ISelectorEditStrategy Members
		TextSearchEngine ISelectorEditStrategy.TextSearchEngine { get { return TextSearchEngine; } }
		bool ISelectorEditStrategy.IsTokenMode { get { return false; } }
		object ISelectorEditStrategy.CurrentDataViewHandle { get { return CurrentDataViewHandle; } }
		object ISelectorEditStrategy.TokenDataViewHandle { get { return null; } }
		int ISelectorEditStrategy.EditableTokenIndex { get { return -1; } }
		bool ISelectorEditStrategy.IsInProcessNewValue { get { return false; } }
		string ISelectorEditStrategy.SearchText { get { return TextSearchEngine.Prefix; } }
		void ISelectorEditStrategy.BringToView() {
			Editor.ScrollIntoView(ValueContainer.EditValue);
		}
		object ISelectorEditStrategy.GetNextValueFromSearchText(int startIndex) {
			return GetNextValueFromSearchTextInternal(startIndex);
		}
		object ISelectorEditStrategy.GetPrevValueFromSearchText(int startIndex) {
			return GetPrevValueFromSearchTextInternal(startIndex);
		}
		IItemsProvider2 ISelectorEditStrategy.ItemsProvider { get { return ItemsProvider; } }
		ISelectorEdit ISelectorEditStrategy.Editor {
			get { return Editor; }
		}
		bool ISelectorEditStrategy.IsSingleSelection {
			get { return IsSingleSelection; }
		}
		object ISelectorEditStrategy.EditValue {
			get { return ValueContainer.EditValue; }
			set { ValueContainer.SetEditValue(value, UpdateEditorSource.ValueChanging); }
		}
		object ISelectorEditStrategy.GetInnerEditorItemsSource() {
			if (IsAsyncServerMode)
				return new AsyncServerModeCollectionView((AsyncVisibleListWrapper)ItemsProvider.VisibleListSource);
			if (IsSyncServerMode)
				return new SyncServerModeCollectionView((SyncVisibleListWrapper)ItemsProvider.VisibleListSource);
			return ItemsProvider.VisibleListSource;
		}
		IEnumerable ISelectorEditStrategy.GetInnerEditorCustomItemsSource() {
			List<CustomItem> items = new List<CustomItem>();
			foreach (ICloneable item in GetInnerEditorCustomItemsSource()) {
				CustomItem clone = (CustomItem)item.Clone();
				clone.SetOwnerEdit(Editor);
				items.Add(clone);
			}
			return items;
		}
		object ISelectorEditStrategy.GetCurrentEditableValue() {
			object value = null;
			ProvideEditValue(EditValue, out value, UpdateEditorSource.TextInput);
			return value;
		}
		IEnumerable ISelectorEditStrategy.GetInnerEditorMRUItemsSource() {
			return new ObservableCollection<object>();
		}
		protected virtual IEnumerable GetInnerEditorCustomItemsSource() {
			ObservableCollection<CustomItem> items = new ObservableCollection<CustomItem>();
			if (!StyleSettings.ShowCustomItem(Editor))
				return items;
			if (StyleSettings.ShowCustomItem(Editor)) {
				foreach (CustomItem customItem in StyleSettings.GetCustomItems(Editor))
					items.Add(customItem);
			}
			return items;
		}
		#endregion
		public virtual void SelectAll() {
			List<object> items = new List<object>();
			foreach (object selectedItem in ItemsProvider.VisibleListSource)
				items.Add(ItemsProvider.GetValueFromItem(selectedItem, ItemsProvider.CurrentDataViewHandle));
			ValueContainer.SetEditValue(items, UpdateEditorSource.ValueChanging);
		}
		public virtual void UnSelectAll() {
			ValueContainer.SetEditValue(null, UpdateEditorSource.ValueChanging);
		}
		RoutedEvent ISelectorEditStrategy.SelectedIndexChangedEvent {
			get { return ListBoxEdit.SelectedIndexChangedEvent; }
		}
		public virtual void ItemsProviderChanged(ItemsProviderChangedEventArgs e) {
			if (e is ItemsProviderRefreshedEventArgs)
				ItemsProviderRefreshedInternal();
			if (e is ItemsProviderDataChangedEventArgs)
				ItemsProviderDataChanged((ItemsProviderDataChangedEventArgs)e);
			if (e is ItemsProviderCurrentChangedEventArgs)
				ItemsProviderCurrentChanged((ItemsProviderCurrentChangedEventArgs)e);
			if (e is ItemsProviderSelectionChangedEventArgs)
				ItemsProviderSelectionChanged((ItemsProviderSelectionChangedEventArgs)e);
			else if (e is ItemsProviderOnBusyChangedEventArgs)
				ItemsProviderOnBusyChanged((ItemsProviderOnBusyChangedEventArgs)e);
		}
		protected virtual void ItemsProviderOnBusyChanged(ItemsProviderOnBusyChangedEventArgs e) {
			Editor.IsAsyncOperationInProgress = e.IsBusy;
		}
		protected virtual void ItemsProviderRefreshed(ItemsProviderRefreshedEventArgs itemsProviderRefreshedEventArgs) {
			if (ListBox != null)
				ListBox.SyncWithOwnerEditWithSelectionLock(true);
		}
		protected virtual void ItemsProviderSelectionChanged(ItemsProviderSelectionChangedEventArgs e) {
			if (IsSingleSelection) {
				object editValue = e.IsSelected ? SelectorUpdater.GetEditValueFromBaseValue(e.CurrentValue) : null;
				ValueContainer.SetEditValue(editValue, UpdateEditorSource.ValueChanging);
			}
			else {
				object editValue = SelectorUpdater.GetEditValueFromBaseValue(ValueContainer.EditValue);
				IEnumerable<object> editValueList = (IEnumerable<object>)editValue;
				var currentValue = new[] { e.CurrentValue };
				IEnumerable<object> result = e.IsSelected
					? editValueList.Append(currentValue).Distinct()
					: editValueList == null ? currentValue : editValueList.Except(currentValue);
				ValueContainer.SetEditValue(result.ToList(), UpdateEditorSource.ValueChanging);
			}
			((ISelectorEdit)Editor).Items.UpdateSelection(Editor.SelectedItems);
			if (ListBox != null)
				ListBox.SyncWithOwnerEditWithSelectionLock(false);
		}
		void ItemsProviderCurrentChanged(ItemsProviderCurrentChangedEventArgs e) {
			SelectorUpdater.SyncWithICollectionView(e.CurrentItem);
			UpdateDisplayText();
		}
		public virtual void ItemsProviderDataChanged(ItemsProviderDataChangedEventArgs e) {
			Core.Native.LogBase.Add(Editor, e, "ItemsProviderDataChangedEventHandled");
			Core.Native.LogBase.Add(Editor.Settings, e, "ItemsProviderDataChangedEventHandled");
			bool skipSyncValue = SelectorUpdater.OptimizedSyncWithValueOnDataChanged(e, ValueContainer.EditValue, SyncWithValue);
			if (ListBox != null)
				ListBox.SyncWithOwnerEditWithSelectionLock(false);
			if (!skipSyncValue)
				SyncWithValue();
		}
		public virtual void ItemsProviderRefreshedInternal() {
			if (ListBox != null)
				ListBox.SyncWithOwnerEditWithSelectionLock(true);
			SyncWithValue();
		}
		public virtual void IsSynchronizedWithCurrentItemChanged(bool value) {
			Settings.IsSynchronizedWithCurrentItem = value;
		}
		public void AllowCollectionViewChanged(bool value) {
			Settings.AllowCollectionView = value;
		}
		public virtual void ShowCustomItemsChanged(bool? value) {
			SyncInnerEditorWithOwnerEdit(true);
		}
		protected virtual void SyncInnerEditorWithOwnerEdit(bool updateSource) {
			if (ListBox != null)
				ListBox.SyncWithOwnerEdit(updateSource);
		}
		protected override void AfterApplyStyleSettings() {
			base.AfterApplyStyleSettings();
			SyncInnerEditorWithOwnerEdit(true);
		}
		public virtual void FilterCriteriaChanged(CriteriaOperator criteriaOperator) {
			Settings.FilterCriteria = criteriaOperator;
		}
		public override void OnInitialized() {
			Editor.SubscribeToItemsProviderChanged();
			SelectorUpdater.SyncWithICollectionView();
			base.OnInitialized();
			if (SelectorUpdater.ShouldSyncWithItems && !PropertyUpdater.HasSyncValue)
				ValueContainer.SetEditValue(SelectorUpdater.GetEditValueFromItems(), UpdateEditorSource.ValueChanging);
		}
		public virtual object GetCurrentSelectedItem() {
			return SelectorUpdater.GetCurrentSelectedItem(ValueContainer);
		}
		public virtual IEnumerable GetCurrentSelectedItems() {
			return SelectorUpdater.GetCurrentSelectedItems(ValueContainer);
		}
		public object GetSelectedItems(object editValue) {
			return SelectorUpdater.GetSelectedItemsFromEditValue(editValue);
		}
		public virtual void AllowRejectUnknownValuesChanged(bool newValue) {
			Settings.AllowRejectUnknownValues = newValue;
			if (!IsInSupportInitialize)
				SyncWithValue();
		}
	}
}
