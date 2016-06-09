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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Collections.Specialized;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using DevExpress.Data.Linq;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.EditStrategy {
	public class ComboBoxEditStrategy : LookUpEditStrategyBase {
		new ComboBoxEdit Editor { get { return base.Editor as ComboBoxEdit; } }
		new internal BaseComboBoxStyleSettings StyleSettings { get { return (BaseComboBoxStyleSettings)base.StyleSettings; } }
		protected internal override bool AllowSpin { get { return base.AllowSpin && ItemsProvider.GetCount(CurrentDataViewHandle) > 0 && (IsSingleSelection || IsInTokenMode); } }
		public ComboBoxEditStrategy(ComboBoxEdit editor)
			: base(editor) {
		}
		protected override IEnumerable GetInnerEditorCustomItemsSource() {
			List<CustomItem> list = new List<CustomItem>();
			if (!StyleSettings.ShowCustomItem(Editor))
				return list;
			list.AddRange(base.GetInnerEditorCustomItemsSource().Cast<CustomItem>());
			return list;
		}
		protected override object GetVisibleListSouce() {
			if (IsAsyncServerMode)
				return new AsyncServerModeCollectionView((AsyncVisibleListWrapper)base.GetVisibleListSouce());
			if (IsSyncServerMode)
				return new SyncServerModeCollectionView((SyncVisibleListWrapper)base.GetVisibleListSouce());
			return base.GetVisibleListSouce();
		}
	}
	public abstract partial class LookUpEditStrategyBase : TextEditStrategy, ISelectorEditStrategy {
		protected static object SpecialNull = new object();
		readonly SelectorPropertiesCoercionHelper selectorUpdater = new SelectorPropertiesCoercionHelper();
		internal TextSearchEngine TextSearchEngine { get; set; }
		protected internal override bool AllowSpin { get { return base.AllowSpin && !Editor.IsPopupOpen; } }
		new LookUpEditBase Editor { get { return (LookUpEditBase)base.Editor; } }
		new LookUpEditSettingsBase Settings { get { return Editor.Settings; } }
		internal new BaseLookUpStyleSettings StyleSettings { get { return (BaseLookUpStyleSettings)base.StyleSettings; } }
		protected internal SelectorPropertiesCoercionHelper SelectorUpdater { get { return selectorUpdater; } }
		protected bool IsSingleSelection { get { return LookUpEditHelper.GetIsSingleSelection(Editor); } }
		bool IsSingleEditValue { get { return IsSingleSelection && !IsInTokenMode; } }
		bool ShouldUpdateAutoSearchText { get { return (Editor.AutoComplete || IncrementalFiltering); } }
		protected VisualClientOwner VisualClient { get { return Editor.VisualClient; } }
		public bool IsInLookUpMode { get { return ItemsProvider.IsInLookUpMode; } }
		bool ShouldUpdateAutoSearchBeforeValidating { get { return Editor.ValidateOnTextInput; } }
		UpdateDataSourceOnAsyncProcessNewValueEventHelper ProcessNewValueHelper { get; set; }
		public virtual bool IsEditingCompleted { get { return !(ValueContainer.EditValue is LookUpEditableItem); } }
		public override bool IsInProcessNewValueDialog { get { return ProcessNewValueHelper.LockerByProcessNewValueWindow; } }
		new LookUpEditBasePropertyProvider PropertyProvider { get { return base.PropertyProvider as LookUpEditBasePropertyProvider; } }
		protected internal bool IsInTokenMode { get { return Editor.IsTokenMode && Editor.EditCore is TokenEditor; } }
		public virtual object CurrentDataViewHandle { get { return PropertyProvider.EditMode == EditMode.InplaceActive ? PropertyProvider.IncrementalFilteringHandle : ItemsProvider.CurrentDataViewHandle; } }
		object TokenEditorDataViewHandle { get { return PropertyProvider.TokenEditorDataViewHandle; } }
		public virtual bool ShouldRaiseProcessValueConversion { get { return IsInLookUpMode && !IsServerMode; } }
		protected internal bool IsAsyncServerMode { get { return ItemsProvider.IsAsyncServerMode; } }
		protected internal bool IsSyncServerMode { get { return ItemsProvider.IsSyncServerMode; } }
		protected bool IsServerMode { get { return PropertyProvider.IsServerMode; } }
		protected virtual bool IncrementByFindIncremental { get { return PropertyProvider.IncrementalFiltering && !string.IsNullOrEmpty(Editor.AutoSearchText); } }
		bool HasHighlightedText { get { return !string.IsNullOrEmpty((string)Editor.GetValue(TextBlockService.HighlightTextProperty)); } }
		EditorTextSearchHelper textSearchHelper;
		EditorTextSearchHelper TextSearchHelper {
			get {
				if (textSearchHelper == null) {
					textSearchHelper = new EditorTextSearchHelper(this);
				}
				return textSearchHelper;
			}
		}
		protected Locker ProcessValidateOnEnterLocker { get; set; }
		Locker AsyncServerModeUpdateLocker { get; set; }
		PostponedAction AcceptPopupValueAction { get; set; }
		Locker FilterChangedLocker { get; set; }
		protected LookUpEditStrategyBase(LookUpEditBase editor)
			: base(editor) {
			SelectorUpdater.SetOwner(this);
			ProcessNewValueHelper = new UpdateDataSourceOnAsyncProcessNewValueEventHelper(Editor);
			ProcessValidateOnEnterLocker = new Locker();
			AsyncServerModeUpdateLocker = new Locker();
			FilterChangedLocker = new Locker();
			AcceptPopupValueAction = new PostponedAction(() => VisualClient.IsPopupOpened);
			TextSearchEngine = CreateTextSearchEngine(editor);
		}
		protected virtual TextSearchEngine CreateTextSearchEngine(LookUpEditBase editor) {
			return new TextSearchEngine(TextSearchCallback);
		}
		protected virtual int TextSearchCallback(string prefix, int startIndex, bool ignoreStartIndex) {
			return ItemsProvider.FindItemIndexByText(prefix, Editor.IsCaseSensitiveSearch, Editor.AutoComplete, CurrentDataViewHandle, startIndex, true, ignoreStartIndex);
		}
		protected override EditorSpecificValidator CreateEditorValidatorService() {
			return new LookUpEditValidator(this, selectorUpdater, Editor);
		}
		public override void RaiseValueChangedEvents(object oldValue, object newValue) {
			base.RaiseValueChangedEvents(oldValue, newValue);
			if (ShouldLockRaiseEvents)
				return;
			SelectorUpdater.RaiseSelectedIndexChangedEvent(oldValue, newValue);
		}
		public virtual int CoerceSelectedIndex(int baseIndex) {
			CoerceValue(LookUpEditBase.SelectedIndexProperty, baseIndex);
			return SelectorUpdater.CoerceSelectedIndex(baseIndex);
		}
		public virtual object CoerceSelectedItem(object baseItem) {
			CoerceValue(LookUpEditBase.SelectedItemProperty, baseItem);
			return SelectorUpdater.CoerceSelectedItem(baseItem);
		}
		public virtual void SelectedIndexChanged(int oldValue, int newValue) {
			if (ShouldLockUpdate)
				return;
			SyncWithValue(LookUpEditBase.SelectedIndexProperty, oldValue, newValue);
		}
		public virtual void SelectedItemChanged(object oldValue, object newValue) {
			if (ShouldLockUpdate)
				return;
			SyncWithValue(LookUpEditBase.SelectedItemProperty, oldValue, newValue);
		}
		public virtual void Find(object parameter) {
			CriteriaOperator filterCriteria;
			if (parameter is CriteriaOperator)
				filterCriteria = parameter as CriteriaOperator;
			else {
				string searchText = Convert.ToString(parameter);
				filterCriteria = ItemsProvider.CreateDisplayFilterCriteria(searchText, PropertyProvider.FilterCondition);
			}
			ItemsProvider.SetDisplayFilterCriteria(filterCriteria, CurrentDataViewHandle);
		}
		public virtual void AddNew(object parameter) {
			string inputText = string.IsNullOrEmpty(parameter as string) ? EditBox.Text : (string)parameter;
			ProcessNewValueCore(inputText);
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			VisualClient.SyncValues();
			EditBox.SyncWithValue(ValueContainer.UpdateSource);
			UpdateSelectedItemValue();
			SelectorUpdater.SyncICollectionView(ValueContainer.EditValue);
			UpdateDisplayFilterInTokenMode();
		}
		private void SetEditBoxEditValue() {
			PropertyProvider.SetDisplayText(CoerceDisplayText(null));
			EditBox.EditValue = GetEditValueForTokenEditor(PropertyProvider.DisplayText);
			if (PropertyProvider.SuppressFeatures)
				return;
			Editor.DisplayText = PropertyProvider.DisplayText;
		}
		private object GetEditValueForTokenEditor(string displayText) {
			var valueContainerValue = ValueContainer.EditValue;
			var editValue = valueContainerValue is LookUpEditableItem ? ((LookUpEditableItem)valueContainerValue).EditValue : valueContainerValue;
			int index = GetCurrentEditableTokenIndex();
			IList<CustomItem> oldValue = null;
			IList listEditValue = editValue as IList;
			if (EditBox.EditValue != null && EditBox.EditValue is TokenEditorCustomItem)
				oldValue = ((TokenEditorCustomItem)EditBox.EditValue).EditValue as IList<CustomItem>;
			if (index > -1 && oldValue != null && index < oldValue.Count) {
				object value = null;
				if (listEditValue != null && oldValue.Count <= listEditValue.Count) {
					value = index < listEditValue.Count ? listEditValue[index] : null;
				}
				string text = value is LookUpEditableItem ? (string)((LookUpEditableItem)value).DisplayValue : null;
				oldValue[index] = GetCustomItemFromValue(value, text);
				return new TokenEditorCustomItem() { Index = index, EditValue = oldValue };
			}
			else if (editValue != null) {
				TokenEditorCustomItem newEditBoxValue = null;
				if (listEditValue != null) {
					List<CustomItem> items = new List<CustomItem>();
					foreach (var value in listEditValue) {
						CustomItem item = GetCustomItemFromValue(value);
						if (item != null)
							items.Add(item);
					}
					if (items.Count > 0) {
						newEditBoxValue = new TokenEditorCustomItem() { Index = GetCurrentEditableTokenIndex() };
						newEditBoxValue.EditValue = items;
					}
				}
				else {
					newEditBoxValue = new TokenEditorCustomItem() { Index = GetCurrentEditableTokenIndex() };
					newEditBoxValue.EditValue = new List<CustomItem>() { new CustomItem() { EditValue = editValue, DisplayText = GetDisplayTextByValue(editValue) } };
				}
				return newEditBoxValue;
			}
			if (ShouldShowNullText)
				return new TokenEditorCustomItem() { NullText = displayText };
			return null;
		}
		private CustomItem GetCustomItemFromValue(object value, string text = null) {
			var newItem = new CustomItem();
			if (value != null) {
				if (value is LookUpEditableItem) {
					var lookupItem = value as LookUpEditableItem;
					newItem.EditValue = lookupItem;
					newItem.DisplayText = (string)lookupItem.DisplayValue;
				}
				else {
					newItem.EditValue = value;
					newItem.DisplayText = text ?? GetDisplayTextByValue(value);
				}
			}
			return newItem;
		}
		private string GetDisplayTextByValue(object value) {
			if (IncrementalFiltering)
				return Editor.GetDisplayText(value, ApplyDisplayTextConversion);
			int index = ItemsProvider.IndexOfValue(value, CurrentDataViewHandle);
			return index > -1 ? ItemsProvider.GetDisplayValueByIndex(index, CurrentDataViewHandle).ToString() : value.Return(x => x.ToString(), () => ((ChangeTextItem)GetEditableObject()).Text);
		}
		protected virtual void UpdateSelectedItemValue() {
			object value = Editor.SelectedItem;
			ContentControl contentControl = value as ContentControl;
			if (contentControl != null) {
				value = contentControl.Content;
				if (Editor != null && Editor.ItemTemplate == null && Editor.ItemTemplateSelector == null) {
					UIElement element = value as UIElement;
					if (element != null) {
						Editor.SelectedValueRenderer.Element = element;
						Editor.SelectedValueRenderer.Render();
						return;
					}
					Editor.SelectedValueRenderer.Element = null;
				}
			}
			Editor.SelectedItemValue = value;
		}
		public virtual void SelectedItemsChanged(IList oldSelectedItems, IList selectedItems) {
			CoerceValue(LookUpEditBase.SelectedItemsProperty, selectedItems);
			if (ShouldLockUpdate)
				return;
			SyncWithValue(LookUpEditBase.SelectedItemsProperty, oldSelectedItems, selectedItems);
		}
		protected internal override void ProcessEditModeChanged(EditMode oldValue, EditMode newValue) {
			UpdateIncrementalFilteringSnapshot(oldValue, newValue);
			base.ProcessEditModeChanged(oldValue, newValue);
		}
		protected internal virtual void UpdateIncrementalFilteringSnapshot(EditMode oldValue, EditMode newValue) {
			if (oldValue == EditMode.InplaceActive)
				ItemsProvider.ReleaseSnapshot(PropertyProvider.IncrementalFilteringHandle);
			if (newValue == EditMode.InplaceActive)
				ItemsProvider.RegisterSnapshot(PropertyProvider.IncrementalFilteringHandle);
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(BaseEdit.EditValueProperty, baseValue => baseValue, baseValue => SelectorUpdater.GetEditValueFromBaseValue(baseValue));
			PropertyUpdater.Register(TextEditBase.TextProperty, GetEditValueFromText, GetTextFromEditValue);
			PropertyUpdater.Register(LookUpEditBase.SelectedIndexProperty, baseValue => SelectorUpdater.GetEditValueFromSelectedIndex(baseValue), baseValue => SelectorUpdater.GetIndexFromEditValue(baseValue));
			PropertyUpdater.Register(LookUpEditBase.SelectedItemProperty, baseValue => SelectorUpdater.GetEditValueFromSelectedItem(baseValue), baseValue => SelectorUpdater.GetSelectedItemFromEditValue(baseValue));
			PropertyUpdater.Register(LookUpEditBase.SelectedItemsProperty, baseValue => SelectorUpdater.GetEditValueFromSelectedItems(baseValue), baseValue => SelectorUpdater.GetSelectedItemsFromEditValue(baseValue), baseValue => SelectorUpdater.UpdateSelectedItems(baseValue));
		}
		public virtual void ItemSourceChanged(object itemsSource) {
			Settings.AssignToEditCoreLocker.DoIfNotLocked(() => {
				Settings.ItemsSource = itemsSource;
			});
			SyncWithValue();
		}
		#region AutoComplete
		protected internal override bool AllowTextInput { get { return base.AllowTextInput && CanProcessAutoSearchText; } }
		protected virtual bool CanProcessAutoSearchText { get { return Editor.AutoComplete && Editor.IsEditorActive && !VisualClient.IsKeyboardFocusWithin && AllowEditing; } }
		protected virtual bool CanProcessReadOnlyAutoSearchText { get { return Editor.AutoComplete && Editor.IsEditorActive && !VisualClient.IsKeyboardFocusWithin && AllowKeyHandling && !Editor.IsReadOnly && !PropertyProvider.IsTextEditable && !IsInTokenMode && PropertyProvider.SelectionMode == SelectionMode.Single; } }
		protected string AutoSearchText { get { return Editor.AutoSearchText; } set { Editor.AutoSearchText = value; } }
		protected bool IncrementalFiltering { get { return PropertyProvider.IncrementalFiltering; } }
		protected virtual bool IsImmediatePopup { get { return Editor.ImmediatePopup; } }
		public override void AfterOnGotFocus() {
			if (IncrementalFiltering && !string.IsNullOrEmpty(Editor.AutoSearchText))
				return;
			base.AfterOnGotFocus();
		}
		public virtual void ProcessAutoCompleteNavKey(KeyEventArgs e) {
			if (e.Handled)
				return;
			if (CanProcessReadOnlyAutoSearchText)
				ProcessReadOnlyAutoSearchText(e);
			else {
				if (!CanProcessAutoSearchText || !AllowEditing)
					return;
				ProcessEditableAutoSearchText(e);
			}
		}
		void ProcessEditableAutoSearchText(KeyEventArgs e) {
			if (e.Key == Key.Escape && !AllTextSelected && Editor.SelectionLength > 0) {
				AutoSearchText = string.Empty;
				Editor.SelectAll();
				e.Handled = true;
				return;
			}
			if (IsInAutoCompleteSelection) {
				if (ModifierKeysHelper.NoModifiers(ModifierKeysHelper.GetKeyboardModifiers(e))) {
					if (e.Key == Key.Left || e.Key == Key.Right) {
						bool leftArrow = e.Key == Key.Left;
						int selectionStart = Editor.SelectionStart;
						int selectionLength = Editor.SelectionLength;
						if (leftArrow) {
							if (selectionStart > 0) { selectionStart--; selectionLength++; }
						}
						else {
							if (selectionStart < EditBox.Text.Length) { selectionStart++; selectionLength = Math.Max(0, selectionLength - 1); }
						}
						bool found = ItemsProvider.FindItemIndexByText(EditBox.Text, Editor.IsCaseSensitiveSearch, Editor.AutoComplete, CurrentDataViewHandle) != -1;
						if (selectionLength > 0 && !found)
							selectionLength = 0;
						Editor.SelectionStart = selectionStart;
						Editor.SelectionLength = selectionLength;
						if (found && (selectionStart > 0 || selectionLength > 0))
							AutoSearchText = EditBox.Text.Substring(0, Math.Min(selectionStart, EditBox.Text.Length));
						e.Handled = true;
					}
				}
				if (e.Key == Key.A && ModifierKeysHelper.IsOnlyCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && !ModifierKeysHelper.IsAltPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
					SelectAll();
					e.Handled = true;
				}
				if (e.Key == Key.Back && Editor.SelectionStart > 0) {
					string text;
					if (Editor.ClearSelectionOnBackspace) {
						text = Editor.SelectionLength > 0
							? Editor.AutoSearchText.Substring(0, Editor.AutoSearchText.Length)
							: Editor.AutoSearchText.Substring(0, Math.Max(0, Editor.AutoSearchText.Length - 1));
						ProcessChangeText(new ChangeTextItem() { Text = text, AutoCompleteTextDeleted = true, UpdateAutoCompleteSelection = CalcUpdateAutoCompleteSelection() });
					}
					else {
						text = EditBox.Text.Substring(0, Math.Max(0, Editor.SelectionStart - 1));
						AutoSearchText = text;
						ProcessChangeText(new ChangeTextItem() { Text = text, UpdateAutoCompleteSelection = true });
					}
					e.Handled = true;
				}
				if (e.Key == Key.Delete) {
					string text = EditBox.Text.Substring(0, Math.Min(EditBox.SelectionStart, EditBox.Text.Length));
					ProcessChangeText(new ChangeTextItem() { Text = text, AutoCompleteTextDeleted = true, UpdateAutoCompleteSelection = true });
					e.Handled = true;
				}
				if (e.Key == Key.Home && EditBox.SelectedText != EditBox.Text) {
					bool found = FindItemIndexByText(EditBox.Text, Editor.AutoComplete) != -1;
					if (found) {
						SelectAll();
						e.Handled = true;
					}
				}
				if (e.Key == Key.End && !string.IsNullOrEmpty(EditBox.Text) && EditBox.SelectionStart != EditBox.Text.Length) {
					ProcessChangeText(EditBox.Text, Editor.AutoComplete);
					e.Handled = true;
				}
			}
			else {
				if (e.Key == Key.Delete) {
					string text = EditBox.Text.Remove(EditBox.CaretIndex, Math.Max(1, EditBox.SelectionLength));
					ProcessChangeText(new ChangeTextItem() { Text = text, AutoCompleteTextDeleted = true, UpdateAutoCompleteSelection = false });
					FlushAutoSearchText();
					e.Handled = true;
				}
			}
			if (e.Key == Key.End && ModifierKeysHelper.GetKeyboardModifiers(e) != ModifierKeys.Shift) {
				string text = EditBox.Text;
				ProcessChangeText(text, true);
				e.Handled = true;
			}
		}
		bool CalcUpdateAutoCompleteSelection() {
			return IsInAutoCompleteSelection;
		}
		void ProcessReadOnlyAutoSearchText(KeyEventArgs e) {
			if (e.Key == Key.Back) {
				if (TextSearchEngine.DeleteLastCharacter())
					UpdateValueAndHighlightedText(ItemsProvider.GetValueByIndex(TextSearchEngine.MatchedItemIndex, CurrentDataViewHandle), false);
				e.Handled = true;
			}
			else if (ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				if (e.Key == Key.Down) {
					NavigateToNextSearchedItem();
					e.Handled = true;
				}
				else if (e.Key == Key.Up) {
					NavigateToPrevSearchedItem();
					e.Handled = true;
				}
			}
		}
		void NavigateToPrevSearchedItem() {
			UpdateValueAndHighlightedText(GetPrevValueFromSearchTextInternal(GetStartIndexToSeachText()));
		}
		void UpdateValueAndHighlightedText(object value, bool immediatePopup = false) {
			if (value != null) {
				ValueContainer.SetEditValue(value, UpdateEditorSource.TextInput);
				UpdateDisplayText();
				if (immediatePopup)
					ShowImmediatePopup();
				SetHighlightedText(TextSearchEngine.SeachText);
			}
		}
		void NavigateToNextSearchedItem() {
			UpdateValueAndHighlightedText(GetNextValueFromSearchTextInternal(GetStartIndexToSeachText()));
		}
		public override void SelectAll() {
			base.SelectAll();
			AutoSearchText = string.Empty;
		}
		public override void Select(int start, int length) {
			base.Select(start, length);
			SetAutoCompleteAutoSearchText(start);
		}
		protected virtual void SetAutoSearchText(string text) {
			AutoSearchText = text;
		}
		protected virtual void SetAutoCompleteAutoSearchText(int start) {
			if (!Editor.AutoComplete)
				return;
			if (!IsInAutoCompleteSelection) {
				SetAutoSearchText(string.Empty);
				return;
			}
			int length = start > 0 ? start : 0;
			ChangeTextItem item = (ChangeTextItem)GetEditableObject();
			string editText = item.Text;
			length = Math.Min(length, editText.Length);
			AutoSearchText = editText.Substring(0, length);
		}
		partial void MouseDownInternal(MouseButtonEventArgs e);
		partial void MouseUpInternal(MouseButtonEventArgs e);
		public override void PreviewMouseDown(MouseButtonEventArgs e) {
			base.PreviewMouseDown(e);
			MouseDownInternal(e);
		}
		public override void PreviewMouseUp(MouseButtonEventArgs e) {
			base.PreviewMouseUp(e);
			FlushAutoSearchText();
			MouseUpInternal(e);
		}
		protected bool AllTextSelected { get { return Editor.SelectionLength == ((ChangeTextItem)GetEditableObject()).Text.Length && Editor.SelectionLength > 0; } }
		protected internal bool IsInAutoCompleteSelection { get { return (EditBox.SelectionStart + EditBox.SelectionLength) == EditBox.Text.Length; } }
		public virtual int FindItemIndexByText(string text, bool autoComplete) {
			return ItemsProvider.FindItemIndexByText(text, Editor.IsCaseSensitiveSearch, autoComplete, handle: GetCurrentDataViewHandleInTokenMode());
		}
		int FindItemIndexByText(string text, bool autoComplete, object handle) {
			return ItemsProvider.FindItemIndexByText(text, Editor.IsCaseSensitiveSearch, autoComplete, handle: handle);
		}
		internal object GetCurrentDataViewHandleInTokenMode() {
			return IsInTokenMode ? TokenEditorDataViewHandle : CurrentDataViewHandle;
		}
		#endregion
		public virtual void SelectedItemsCollectionChanged(NotifyCollectionChangedEventArgs e) {
			CoerceValue(ComboBoxEdit.SelectedItemsProperty, Editor.SelectedItems);
			if (ShouldLockUpdate)
				return;
			SyncWithValue(ComboBoxEdit.SelectedItemsProperty, SelectorUpdater.GetPreviousSelectedItems(e), Editor.SelectedItems);
		}
		#region ISelectorEditStrategy Members
		TextSearchEngine ISelectorEditStrategy.TextSearchEngine { get { return TextSearchEngine; } }
		bool ISelectorEditStrategy.IsTokenMode { get { return Editor.IsTokenMode; } }
		object ISelectorEditStrategy.TokenDataViewHandle { get { return GetCurrentDataViewHandleInTokenMode(); } }
		int ISelectorEditStrategy.EditableTokenIndex { get { return GetCurrentEditableTokenIndex(); } }
		bool ISelectorEditStrategy.IsInProcessNewValue { get { return ProcessNewValueHelper.IsInProcessNewValue; } }
		string ISelectorEditStrategy.SearchText { get { return TextSearchEngine.Prefix; } }
		void ISelectorEditStrategy.BringToView() {
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
		RoutedEvent ISelectorEditStrategy.SelectedIndexChangedEvent {
			get { return ComboBoxEdit.SelectedIndexChangedEvent; }
		}
		object ISelectorEditStrategy.GetInnerEditorItemsSource() {
			return GetVisibleListSouce();
		}
		protected virtual object GetVisibleListSouce() {
			return ItemsProvider.GetVisibleListSource(GetCurrentDataViewHandleInTokenMode());
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
			if (ProvideEditValue(EditValue, out value, UpdateEditorSource.TextInput)) {
				if (IsInTokenMode) {
					int index = GetCurrentEditableTokenIndex();
					var listValue = value as IList;
					return (index != -1 && listValue != null && listValue.Count > index) ? listValue[index] : listValue;
				}
			}
			return value;
		}
		protected virtual IEnumerable GetInnerEditorCustomItemsSource() {
			List<CustomItem> items = new List<CustomItem>();
			if (!StyleSettings.ShowCustomItem(Editor))
				return items;
			if (StyleSettings.ShowCustomItem(Editor))
				items.AddRange(StyleSettings.GetCustomItems(Editor));
			return items;
		}
		IEnumerable ISelectorEditStrategy.GetInnerEditorMRUItemsSource() {
			List<CustomItem> items = new List<CustomItem>();
			return items;
		}
		#endregion
		public virtual void AcceptPopupValue() {
			if (Editor.IsReadOnly)
				return;
			object selectedItems = IsSingleSelection ? VisualClient.GetSelectedItem() : VisualClient.GetSelectedItems().Cast<object>().ToList();
			AcceptPopupValueAction.PerformPostpone(() => AcceptPopupValueInternal(selectedItems));
		}
		protected object GetFilteredValueFromSelectedItems(object selectedItems) {
			if (IsSingleSelection) {
				object selectedItem = selectedItems;
				object filteredItem = SelectorUpdater.FilterSelectedItem(selectedItem);
				if (!IsServerMode && ((CustomItemHelper.IsCustomItem(filteredItem) || ItemsProvider.GetIndexByItem(filteredItem, CurrentDataViewHandle) > -1))) {
					return SelectorUpdater.GetEditValueFromSelectedItem(filteredItem);
				}
				if (IsServerMode)
					return ItemsProvider.GetValueByRowKey(filteredItem, CurrentDataViewHandle);
				return SpecialNull;
			}
			IEnumerable<object> selectedItems2 = (IEnumerable<object>)selectedItems;
			IEnumerable<object> filteredItems = SelectorUpdater.FilterSelectedItems(selectedItems2);
			if (IsServerMode)
				return filteredItems.Select(x => ItemsProvider.GetValueByRowKey(x, CurrentDataViewHandle)).ToList();
			return SelectorUpdater.GetEditValueFromSelectedItems(filteredItems.ToList());
		}
		protected virtual void AcceptPopupValueInternal(object selectedItems) {
			EditBox.BeforeAcceptPopupValue();
			object filteredItems = GetFilteredValueFromSelectedItems(selectedItems);
			if (filteredItems == SpecialNull)
				return;
			if (IsInTokenMode && IsSingleSelection)
				AcceptPopupValueInternalInSingleTokenMode(filteredItems);
			else {
				ValueContainer.SetEditValue(CreateEditValueForAcceptPopupValue(selectedItems, filteredItems), UpdateEditorSource.ValueChanging);
			}
			EditBox.AfterAcceptPopupValue();
			UpdateDisplayText();
			SelectAll();
		}
		object CreateEditValueForAcceptPopupValue(object selectedItems, object filteredItems) {
			if (IsAsyncServerMode) {
				int index = ItemsProvider.IndexOfValue(filteredItems, CurrentDataViewHandle);
				var lookUpEditableItem = ValueContainer.EditValue as LookUpEditableItem;
				var editableItem = index > -1
						? CreateEditableItemForExistentValue(index, filteredItems, lookUpEditableItem.With(x => x.ChangeTextItem))
						: CreateEditableItemForNonexistentValue(index, FilterSelectedItemFromUnsetValue(selectedItems), lookUpEditableItem.With(x => x.ChangeTextItem));
				editableItem.ForbidFindIncremental = true;
				return editableItem;
			}
			if (IsSingleSelection)
				return filteredItems;
			return FilterSelectedItemsFromEmptyList(filteredItems);
		}
		object FilterSelectedItemsFromEmptyList(object filteredItems) {
			return (filteredItems as IList).If(x => x.Count > 0).Return(x => filteredItems, null);
		}
		object FilterSelectedItemFromUnsetValue(object selectedItem) {
			return LookUpPropertyDescriptorBase.IsUnsetValue(selectedItem) ? null : selectedItem;
		}
		private void AcceptPopupValueInternalInSingleTokenMode(object newValue) {
			if (newValue == null)
				return;
			var listEditValue = ValueContainer.EditValue is LookUpEditableItem ? ((LookUpEditableItem)ValueContainer.EditValue).EditValue as IList<object> : ValueContainer.EditValue as IList<object>;
			var newListValue = new List<object>();
			if (listEditValue != null) {
				var editCoreEditValue = EditBox.EditValue as TokenEditorCustomItem;
				int index = editCoreEditValue != null ? editCoreEditValue.Index : -1;
				newListValue.AddRange(listEditValue);
				if (index > -1 && index < newListValue.Count) {
					var listEditCoreValue = editCoreEditValue.EditValue as IList;
					if (listEditCoreValue != null && listEditCoreValue.Count == newListValue.Count)
						newListValue[index] = newValue;
					else
						newListValue.Insert(index, newValue);
				}
				else
					newListValue.Add(newValue);
			}
			else
				newListValue.Add(newValue);
			ValueContainer.SetEditValue(new LookUpEditableItem() { EditValue = newListValue, DisplayValue = GetDisplayTextByValue(newValue) }, UpdateEditorSource.ValueChanging);
		}
		void FlushAutoSearchText() {
			AutoSearchText = string.Empty;
		}
		public object GetEditValueFromText(object baseValue) {
			string value = baseValue as string;
			if (value == null)
				return IsSingleEditValue ? null : new List<object>();
			if (IsSingleEditValue)
				return FindEditValueFromText(baseValue);
			List<object> list = new List<object>();
			string[] values = value.Split(new[] { Editor.SeparatorString }, StringSplitOptions.RemoveEmptyEntries);
			list.AddRange(values.Select(FindEditValueFromText).Where(editValue => ItemsProvider.IndexOfValue(editValue, CurrentDataViewHandle) > -1));
			return list;
		}
		object FindEditValueFromText(object baseValue) {
			if (IsInLookUpMode)
				return ItemsProvider.GetValueByIndex(ItemsProvider.FindItemIndexByText(Convert.ToString(baseValue), true, false, CurrentDataViewHandle), CurrentDataViewHandle);
			return Convert.ToString(baseValue);
		}
		protected override object GetEditableObject() {
			string text = EditBox.Text;
			if (!IsInTokenMode && Editor.AutoComplete && IsInAutoCompleteSelection)
				text = text.Substring(0, EditBox.SelectionStart);
			if (!AsyncServerModeUpdateLocker.IsLocked)
				return new ChangeTextItem() { Text = text, UpdateAutoCompleteSelection = CalcUpdateAutoCompleteSelection() };
			var item = ValueContainer.EditValue as LookUpEditableItem;
			if (item == null)
				return text;
			if (item.AutoCompleteTextDeleted || item.AsyncLoading) {
				ChangeTextItem changeTextItem = item.ChangeTextItem;
				changeTextItem.Do(x => x.AsyncLoading = item.AsyncLoading);
				return changeTextItem;
			}
			return new ChangeTextItem() { Text = text, UpdateAutoCompleteSelection = CalcUpdateAutoCompleteSelection() };
		}
		protected override void SyncWithEditorInternal() {
			ProcessChangeText((ChangeTextItem)GetEditableObject());
			if (CanProcessAutoSearchText && !IsInAutoCompleteSelection)
				FlushAutoSearchText();
		}
		protected override void UpdateDisplayTextInternal() {
			if (IsInTokenMode)
				SetEditBoxEditValue();
			else
				base.UpdateDisplayTextInternal();
		}
		protected override void UpdateDisplayTextAndRestoreCursorPosition() {
			CursorPositionSnapshot snapshot = new CursorPositionSnapshot(EditBox.SelectionStart, EditBox.SelectionLength, Editor.EditBox.Text, Editor.AutoComplete);
			UpdateDisplayTextInternal();
			snapshot.ApplyToEdit(Editor);
		}
		protected virtual void ProcessChangeText(ChangeTextItem item) {
			string editText = item.Text;
			bool updateAutoSearchSelection = item.UpdateAutoCompleteSelection;
			UpdateAutoSearchBeforeValidate(item);
			int index = FindItemIndexByText(editText, Editor.AutoComplete);
			object value = CreateEditableItem(index, item);
			ValueContainer.SetEditValue(value, UpdateEditorSource.TextInput);
			UpdateAutoSearchAfterValidate(item);
			UpdateDisplayText();
			UpdateAutoSearchSelection(updateAutoSearchSelection);
			ShowImmediatePopup();
		}
		protected virtual void ProcessChangeText(string editText, bool updateAutoSearchSelection) {
			ProcessChangeText(new ChangeTextItem() { Text = editText, UpdateAutoCompleteSelection = updateAutoSearchSelection });
		}
		public virtual void ShowImmediatePopup() {
			if (IsImmediatePopup && !Editor.IsPopupOpen && !AsyncServerModeUpdateLocker.IsLocked)
				Editor.ShowPopup();
			else if (Editor.IsPopupOpen) {
				VisualClient.SyncValues(true);
			}
		}
		protected virtual void UpdateAutoSearchBeforeValidate(ChangeTextItem item) {
			if (!ShouldUpdateAutoSearchText) {
				AutoSearchText = string.Empty;
				return;
			}
			if (!ShouldUpdateAutoSearchBeforeValidating)
				return;
			UpdateAutoSearchText(item, IsInLookUpMode);
		}
		protected virtual void UpdateAutoSearchAfterValidate(ChangeTextItem item) {
			if (!ShouldUpdateAutoSearchText || ShouldUpdateAutoSearchBeforeValidating)
				return;
			UpdateAutoSearchText(item, false);
		}
		public virtual void UpdateAutoSearchText(ChangeTextItem item, bool reverse) {
			if (!ShouldUpdateAutoSearchText)
				return;
			string editText = item.Text;
			string autoText = reverse ? AutoSearchText : editText;
			int index = FindItemIndexByText(editText, Editor.AutoComplete);
			if (index > -1) {
				string displayText = Convert.ToString(ItemsProvider.GetDisplayValueByIndex(index, CurrentDataViewHandle));
				autoText = TextBlockService.GetStartWithPart(displayText, editText);
			}
			else {
				if (item.AsyncLoading)
					return;
			}
			AutoSearchText = autoText;
		}
		protected virtual object CreateEditableItem(int index, ChangeTextItem item) {
			if (IsInTokenMode)
				return CreateEditableItemInTokenMode(index, item);
			return CreateSingleEditableItem(index, item);
		}
		private object CreateSingleEditableItem(int index, ChangeTextItem item) {
			if (!IsInLookUpMode) {
				if (item.AutoCompleteTextDeleted)
					return CreateEditableItemForNonexistentValue(index, item.Text, item);
				return index > -1
					? CreateEditableItemForExistentValue(index, SelectorUpdater.GetEditValueFromSelectedIndex(index), item)
					: CreateEditableItemForNonexistentValue(index, item.Text, item);
			}
			object editValue = index > -1 ? SelectorUpdater.GetEditValueFromSelectedIndex(index) : Editor.EditValue;
			if (Editor.AssignNullValueOnClearingEditText && index == -1 && string.IsNullOrEmpty(item.Text))
				editValue = Editor.NullValue;
			if ((!Editor.ValidateOnTextInput && !Editor.AutoComplete) || item.AutoCompleteTextDeleted || ProcessValidateOnEnterLocker)
				return new LookUpEditableItem {
					DisplayValue = item.Text,
					EditValue = editValue,
					ChangeTextItem = item,
					AsyncLoading = Editor.IsAsyncOperationInProgress,
					AutoCompleteTextDeleted = item.AutoCompleteTextDeleted,
					Completed = false,
				};
			if (index > -1)
				return CreateEditableItemForExistentValue(index, editValue, item);
			return Editor.AssignNullValueOnClearingEditText && string.IsNullOrEmpty(item.Text)
				? CreateEditableItemForExistentValue(index, editValue, item)
				: CreateEditableItemForNonexistentValue(-1, editValue, item);
		}
		LookUpEditableItem CreateEditableItemForExistentValue(int index, object editValue, ChangeTextItem item) {
			var editableItem = new LookUpEditableItem {
				ChangeTextItem = item,
				DisplayValue = ItemsProvider.GetDisplayValueByIndex(index, CurrentDataViewHandle),
				EditValue = editValue,
				AutoCompleteTextDeleted = item.Return(x => x.AutoCompleteTextDeleted, () => false),
				AsyncLoading = Editor.IsAsyncOperationInProgress,
				Completed = true,
			};
			return editableItem;
		}
		private object CreateEditableItemInTokenMode(int index, ChangeTextItem editItem) {
			var editValue = EditBox.EditValue as TokenEditorCustomItem;
			if (editValue == null || editValue.EditValue == null)
				return null;
			var listEditValue = editValue.EditValue as IList<CustomItem>;
			string editText = editItem.Text;
			if (listEditValue != null && listEditValue.Count > 0) {
				var newEditValue = new List<object>();
				int editableValueIndex = editValue.Index;
				for (int i = 0; i < listEditValue.Count; i++) {
					var item = listEditValue[i];
					bool isEditableItem = i == editableValueIndex;
					if (isEditableItem) {
						int valueIndex = FindItemIndexByText(editText, Editor.AutoComplete);
						object value = CreateSingleEditableItem(valueIndex, editItem);
						if ((value == Editor.EditValue) || (value is LookUpEditableItem && ((LookUpEditableItem)value).EditValue == Editor.EditValue))
							value = new LookUpEditableItem() { EditValue = GetValueFromEditValueByIndex(i, editText), DisplayValue = editText };
						if (value != null)
							newEditValue.Add(value);
						else
							newEditValue.Add(new LookUpEditableItem());
					}
					else {
						var oldValue = item.EditValue;
						if (oldValue != null)
							newEditValue.Add(item.EditValue);
					}
				}
				return newEditValue.Count > 0 ? new LookUpEditableItem() { EditValue = newEditValue, DisplayValue = editText } : null;
			}
			else {
				object singleValue = null;
				var editableIndex = editValue != null ? editValue.Index : -1;
				singleValue = CreateSingleEditableItem(index, editItem);
				if ((singleValue == Editor.EditValue) || (singleValue is LookUpEditableItem && ((LookUpEditableItem)singleValue).EditValue == Editor.EditValue)) {
					var candidate = GetValueFromEditValueByIndex(editableIndex, editText);
					if (candidate == null && string.IsNullOrEmpty(editText))
						singleValue = null;
					else
						singleValue = new LookUpEditableItem() { EditValue = candidate, DisplayValue = editText };
				}
				if (singleValue != null) {
					var resultValue = singleValue is IList<object> ? new List<object>((IList<object>)singleValue) : new List<object>() { singleValue };
					return new LookUpEditableItem() { EditValue = new List<object>() { resultValue }, DisplayValue = editText };
				}
			}
			return null;
		}
		private object GetValueFromEditValueByIndex(int index, string editText) {
			if (Editor.AssignNullValueOnClearingEditText && index == -1 && string.IsNullOrEmpty(editText))
				return Editor.NullValue;
			var listEditValue = Editor.EditValue as IList<object>;
			if (listEditValue != null)
				return index > -1 && index < listEditValue.Count ? listEditValue[index] : null;
			return null;
		}
		private int GetCurrentEditableTokenIndex() {
			return (EditBox.EditValue as TokenEditorCustomItem).Return(x => x.Index, () => -1);
		}
		protected virtual LookUpEditableItem CreateEditableItemForNonexistentValue(int index, object value, ChangeTextItem item) {
			return new LookUpEditableItem() {
				DisplayValue = item.Return(x => x.Text, () => string.Empty),
				EditValue = value,
				ChangeTextItem = item,
				AutoCompleteTextDeleted = item.Return(x => x.AutoCompleteTextDeleted, () => false),
				AsyncLoading = Editor.IsAsyncOperationInProgress,
				Completed = CalcLookUpEditableItemCompleted(index),
			};
		}
		bool CalcLookUpEditableItemCompleted(int index) {
			if (IsAsyncServerMode)
				return index > -1;
			return !IsInLookUpMode || index > -1;
		}
		protected override bool ShouldRestoreCursorPosition() {
			return base.ShouldRestoreCursorPosition() || AsyncServerModeUpdateLocker.IsLocked;
		}
		void UpdateAutoSearchSelection(bool updateSelection) {
			if (!Editor.AutoComplete || Editor.IsAsyncOperationInProgress)
				return;
			if (updateSelection) {
				var value = GetCurrentEditableValue();
				string primaryText = Convert.ToString(GetDisplayValue(value));
				EditBox.Select(AutoSearchText.Length, Math.Max(0, primaryText.Length - AutoSearchText.Length));
				return;
			}
		}
		private object GetCurrentEditableValue() {
			if (IsInTokenMode)
				return GetEditableValueInTokenMode();
			object value;
			if (ProvideEditValue(ValueContainer.EditValue, out value, UpdateEditorSource.TextInput))
				return value;
			return null;
		}
		private object GetEditableValueInTokenMode() {
			if (ValueContainer.EditValue is LookUpEditableItem) {
				LookUpEditableItem item = ValueContainer.EditValue as LookUpEditableItem;
				int index = GetCurrentEditableTokenIndex();
				var listEditValue = (IList<object>)item.EditValue;
				return index > -1 && index < listEditValue.Count ? listEditValue[index] : null;
			}
			else {
				var listValue = ValueContainer.EditValue as IList<object>;
				int index = GetCurrentEditableTokenIndex();
				if (listValue != null && index > -1 && index < listValue.Count)
					return listValue[index];
			}
			return null;
		}
		protected virtual object GetDisplayValue(object editValue) {
			var lookupItem = editValue as LookUpEditableItem;
			return lookupItem != null ? lookupItem.DisplayValue : ItemsProvider.GetDisplayValueByEditValue(editValue, CurrentDataViewHandle);
		}
		public virtual bool ProcessNewValueCore(string editText) {
			using (ProcessNewValueHelper.Subscribe()) {
				ProcessNewValueEventArgs args = new ProcessNewValueEventArgs(LookUpEditBase.ProcessNewValueEvent) { DisplayText = editText };
				Editor.RaiseEvent(args);
				if (!args.Handled)
					return false;
				if (args.PostponedValidation)
					ProcessNewValueHelper.UpdateDataAsync();
				else
					ItemsProvider.DoRefresh();
				return true;
			}
		}
		public override bool ProcessNewValue(string editText) {
			if (string.IsNullOrEmpty(editText) || ProcessNewValueHelper.IsInProcessNewValue)
				return false;
			return ProcessNewValueCore(editText);
		}
		public object GetTextFromEditValue(object baseValue) {
			return Editor.GetDisplayText(baseValue, false);
		}
		public override bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (IsInTokenMode && EditBox.NeedsKey(key, modifiers))
				return true;
			if (key == Key.Up || key == Key.Down)
				return ModifierKeysHelper.ContainsModifiers(modifiers);
			return base.NeedsKey(key, modifiers);
		}
		protected internal override object ConvertEditValueForFormatDisplayText(object editValue) {
			object convertedValue = base.ConvertEditValueForFormatDisplayText(editValue);
			convertedValue = GetDisplayValue(convertedValue);
			IList list = convertedValue as IList;
			if (list != null && list.Count == 0)
				convertedValue = null;
			return convertedValue;
		}
		public virtual void ItemProviderChanged(ItemsProviderChangedEventArgs e) {
			if (e is ItemsProviderRefreshedEventArgs)
				ItemsProviderRefreshed((ItemsProviderRefreshedEventArgs)e);
			else if (e is ItemsProviderDataChangedEventArgs)
				ItemsProviderDataChanged((ItemsProviderDataChangedEventArgs)e);
			else if (e is ItemsProviderCurrentChangedEventArgs)
				ItemsProviderCurrentChanged((ItemsProviderCurrentChangedEventArgs)e);
			else if (e is ItemsProviderSelectionChangedEventArgs)
				ItemsProviderSelectionChanged((ItemsProviderSelectionChangedEventArgs)e);
			else if (e is ItemsProviderRowLoadedEventArgs)
				ItemsProviderRowLoaded((ItemsProviderRowLoadedEventArgs)e);
			else if (e is ItemsProviderOnBusyChangedEventArgs)
				ItemsProviderOnBusyChanged((ItemsProviderOnBusyChangedEventArgs)e);
			else if (e is ItemsProviderViewRefreshedEventArgs)
				ItemsProviderViewRefreshed((ItemsProviderViewRefreshedEventArgs)e);
			else if (e is ItemsProviderFindIncrementalCompletedEventArgs)
				ItemsProviderFindIncrementalCompleted((ItemsProviderFindIncrementalCompletedEventArgs)e);
		}
		void ItemsProviderFindIncrementalCompleted(ItemsProviderFindIncrementalCompletedEventArgs e) {
			if (ValueContainer.HasValueCandidate)
				ServerModeSyncWithEditor();
		}
		void ItemsProviderViewRefreshed(ItemsProviderViewRefreshedEventArgs e) {
			object handle = e.Handle;
			if (handle == CurrentDataViewHandle) {
				if (ValueContainer.HasValueCandidate)
					ServerModeSyncWithEditor();
				return;
			}
			if (handle == null) {
			}
		}
		protected virtual void ItemsProviderSelectionChanged(ItemsProviderSelectionChangedEventArgs e) {
			if (Editor.IsPopupOpen)
				return;
			if (IsSingleSelection) {
				object editValue;
				if (e.IsSelected)
					editValue = SelectorUpdater.GetEditValueFromBaseValue(e.CurrentValue);
				else {
					ComboBoxEditItem item = ItemsProvider.GetItem(ValueContainer.EditValue, CurrentDataViewHandle) as ComboBoxEditItem;
					editValue = item != null && item.IsSelected ? ValueContainer.EditValue : null;
				}
				ValueContainer.SetEditValue(editValue, UpdateEditorSource.ValueChanging);
			}
			else {
				object editValue = SelectorUpdater.GetEditValueFromBaseValue(ValueContainer.EditValue);
				IEnumerable<object> editValueList = (IEnumerable<object>)editValue;
				var currentValue = new[] { e.CurrentValue };
				IEnumerable<object> result = e.IsSelected
					? editValueList.Append(currentValue).Distinct()
					: editValueList == null ? new List<object>() : editValueList.Except(currentValue);
				ValueContainer.SetEditValue(result.ToList(), UpdateEditorSource.ValueChanging);
			}
			((ISelectorEdit)Editor).Items.UpdateSelection(SelectorUpdater.GetSelectedItemsFromEditValue(ValueContainer.EditValue));
			VisualClient.SyncProperties(false);
		}
		protected virtual void ItemsProviderRefreshed(ItemsProviderRefreshedEventArgs e) {
			Core.Native.LogBase.Add(Editor, e, "ItemsProviderRefreshedEventHandled");
			Core.Native.LogBase.Add(Editor.Settings, e, "ItemsProviderRefreshedEventHandled");
			if (IsAsyncServerMode) {
				var lookUpEditableItem = ValueContainer.EditValue as LookUpEditableItem;
				if (lookUpEditableItem.If(x => x.AsyncLoading).ReturnSuccess())
					ServerModeSyncWithEditor();
			}
			else if (IsServerMode) {
				if (ValueContainer.HasValueCandidate)
					return;
				SyncWithValue();
			}
			VisualClient.SyncProperties(true);
		}
		protected virtual void ItemsProviderDataChanged(ItemsProviderDataChangedEventArgs e) {
			Core.Native.LogBase.Add(Editor, e, "ItemsProviderDataChangedEventHandled");
			Core.Native.LogBase.Add(Editor.Settings, e, "ItemsProviderDataChangedEventHandled");
			if (SelectorUpdater.OptimizedSyncWithValueOnDataChanged(e, ValueContainer.EditValue, SyncWithValueOnDataChanged))
				return;
			SyncWithValueOnDataChanged();
			VisualClient.SyncProperties(true);
		}
		protected virtual void SyncWithValueOnDataChanged() {
			if (ValueContainer.HasValueCandidate)
				return;
			FilterChangedLocker.DoIfNotLocked(SyncWithValue);
		}
		protected virtual void ItemsProviderCurrentChanged(ItemsProviderCurrentChangedEventArgs e) {
			Core.Native.LogBase.Add(Editor, e, "ItemsProviderCurrentChangedEventHandled");
			Core.Native.LogBase.Add(Editor.Settings, e, "ItemsProviderCurrentChangedEventHandled");
			if (FilterChangedLocker)
				return;
			SelectorUpdater.SyncWithICollectionView(e.CurrentItem);
			UpdateDisplayText();
		}
		protected virtual void ItemsProviderRowLoaded(ItemsProviderRowLoadedEventArgs e) {
			object handle = e.Handle;
			if (handle == null) {
				object value;
				bool result = ProvideEditValue(ValueContainer.EditValue, out value, UpdateEditorSource.TextInput);
				if (result) {
					if (object.Equals(value, e.Value))
						ServerModeSyncWithValue(value);
				}
			}
		}
		void ServerModeSyncWithEditor() {
			AsyncServerModeUpdateLocker.DoLockedAction(SyncWithEditor);
		}
		void ServerModeSyncWithValue(object value) {
			ChangeTextItem item = null;
			AsyncServerModeUpdateLocker.DoLockedAction(() => item = GetEditableObject() as ChangeTextItem);
			int index = ItemsProvider.IndexOfValue(value, CurrentDataViewHandle);
			var editableItem = index > -1 ? CreateEditableItemForExistentValue(index, value, item) : CreateEditableItemForNonexistentValue(index, value, item);
			editableItem.ForbidFindIncremental = true;
			ValueContainer.SetEditValue(editableItem, UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		protected virtual void ItemsProviderOnBusyChanged(ItemsProviderOnBusyChangedEventArgs e) {
			Editor.IsAsyncOperationInProgress = e.IsBusy;
			if (e.IsBusy)
				return;
			if (!IsInTokenMode) {
				var lookUpEditableItem = ValueContainer.EditValue as LookUpEditableItem;
				bool asyncLoading = lookUpEditableItem.If(x => x.AsyncLoading).ReturnSuccess();
				if (asyncLoading)
					SyncWithEditor();
				else if (lookUpEditableItem == null)
					SyncWithValue();
			}
		}
		public virtual void PopupClosed() {
			VisualClient.PopupClosed();
		}
		public override void OnLostFocus() {
			FlushAutoSearchText();
			if (Editor.IsTokenMode)
				UnregisterTokenSnapshot();
			base.OnLostFocus();
			SetHighlightedText(string.Empty);
		}
		public override void OnGotFocus() {
			base.OnGotFocus();
			if (Editor.IsTokenMode) {
				RegisterTokenSnapshot();
				UpdateDisplayFilterInTokenMode();
			}
		}
		public override void OnInitialized() {
			Editor.UnsubscribeToItemsProviderChanged();
			Editor.SubscribeToItemsProviderChanged();
			SelectorUpdater.SyncWithICollectionView();
			base.OnInitialized();
			if (SelectorUpdater.ShouldSyncWithItems && !PropertyUpdater.HasSyncValue)
				ValueContainer.SetEditValue(SelectorUpdater.GetEditValueFromItems(), UpdateEditorSource.ValueChanging);
		}
		protected override void ProcessPreviewKeyDownInternal(KeyEventArgs e) {
			if (IsInTokenMode && EditBox.NeedsNavigationKey(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e))) {
				if (e.Key != Key.Enter && EditBox.ProccessKeyDown(e))
					return;
			}
			ProcessAutoCompleteNavKey(e);
			if (Editor.LeavePopupGesture(e))
				return;
			base.ProcessPreviewKeyDownInternal(e);
			VisualClient.ProcessKeyDown(e);
		}
		protected internal override bool NeedsEnterKey(ModifierKeys modifiers) {
			if (IsInTokenMode && Editor.EditMode == EditMode.InplaceActive) {
				if (ModifierKeysHelper.IsCtrlPressed(modifiers))
					return EditBox.NeedsEnterKey();
				return false;
			}
			else
				return base.NeedsEnterKey(modifiers);
		}
		private void ReraiseKeyDown(KeyEventArgs e) {
			ReraiseEventHelper.ReraiseEvent(e, Editor.EditCore, UIElement.PreviewKeyDownEvent, UIElement.KeyDownEvent, (x) => new KeyEventArgs(x.KeyboardDevice, x.InputSource, x.Timestamp, x.Key));
		}
		public override void SetNullValue(object parameter) {
			base.SetNullValue(parameter);
			SetAutoSearchText(string.Empty);
		}
		protected internal override bool CanSpinDown() {
			return base.CanSpinDown() && (PropertyProvider.IsSingleSelection || IsInTokenMode);
		}
		protected internal override bool CanSpinUp() {
			return base.CanSpinUp() && (PropertyProvider.IsSingleSelection || IsInTokenMode);
		}
		protected override bool SpinDown() {
			bool result = ChangeIndexAndHandle(1);
			UpdateDisplayText();
			return result;
		}
		protected override bool SpinUp() {
			bool result = ChangeIndexAndHandle(-1);
			UpdateDisplayText();
			return result;
		}
		protected virtual bool ChangeIndexAndHandle(int d) {
			var handle = GetCurrentDataViewHandleInTokenMode();
			var editValue = GetCurrentEditableValue();
			int newIndex = FindNextIndex(editValue, Math.Sign(d) > 0, handle);
			if (IndexInRange(newIndex)) {
				var value = IsInTokenMode
					? CreateEditableItemInTokenMode(newIndex, new ChangeTextItem { Text = GetDisplayTextByValue(ItemsProvider.GetValueByIndex(newIndex, handle)) })
					: ItemsProvider.GetValueByIndex(newIndex, handle);
				ValueContainer.SetEditValue(value, UpdateEditorSource.TextInput);
			}
			return true;
		}
		int FindNextIndex(object editValue, bool searchNext, object handle) {
			int selectedIndex = ItemsProvider.IndexOfValue(editValue, handle);
			if (IncrementByFindIncremental) {
				string text = Editor.AutoSearchText;
				return ItemsProvider.FindItemIndexByText(text, Editor.IsCaseSensitiveSearch, true, handle, selectedIndex, searchNext, true);
			}
			int controllerIndex = ItemsProvider.GetControllerIndexByIndex(selectedIndex, handle);
			if (controllerIndex < 0)
				controllerIndex = -1;
			int newControllerIndex = controllerIndex + CalcOffset(searchNext);
			return ItemsProvider.GetIndexByControllerIndex(newControllerIndex, handle);
		}
		int CalcOffset(bool searchNext) {
			int selectedIndex = ItemsProvider.IndexOfValue(GetCurrentEditableValue(), CurrentDataViewHandle);
			int controllerIndex = ItemsProvider.GetControllerIndexByIndex(selectedIndex, CurrentDataViewHandle);
			if (controllerIndex < 0)
				controllerIndex = -1;
			int offset = 0;
			do {
				offset += searchNext ? 1 : -1;
				if (!IndexInRange(controllerIndex + offset))
					return 0;
			} while (!IsEnabledContainer(controllerIndex + offset));
			return offset;
		}
		bool IndexInRange(int index) {
			int controllerIndex = ItemsProvider.GetControllerIndexByIndex(index, CurrentDataViewHandle);
			return controllerIndex > -1 && controllerIndex < ItemsProvider.GetCount(CurrentDataViewHandle);
		}
		protected virtual bool IsEnabledContainer(int controllerIndex) {
			object container = ItemsProvider.GetItemByControllerIndex(controllerIndex, CurrentDataViewHandle);
			return GetIsEnabled(container);
		}
		bool GetIsEnabled(object container) {
			return !(container is FrameworkElement) || ((FrameworkElement)container).IsEnabled;
		}
		public virtual object GetPopupContentItemsSource() {
			return ItemsProvider.VisibleListSource;
		}
		internal VisualClientOwner GetVisualClient() {
			return VisualClient;
		}
		public void RaisePopupContentSelectionChanged(IList removedItems, IList addedItems) {
			SelectionChangedEventArgs args = new SelectionChangedEventArgs(LookUpEditBase.PopupContentSelectionChangedEvent, removedItems, addedItems);
			if (StyleSettings.ProcessContentSelectionChanged(VisualClient.InnerEditor, args))
				Editor.RaiseEvent(args);
		}
		public void OnIncrementalFilteringChanged() {
			UpdateDisplayFilter();
		}
		public virtual void UpdateDisplayFilter() {
			FilterChangedLocker.DoLockedAction(() => {
				if (!IsInTokenMode)
					ItemsProvider.SetDisplayFilterCriteria(CreateDisplayFilter(), CurrentDataViewHandle);
				else
					UpdateDisplayFilterInTokenMode();
			});
		}
		private CriteriaOperator CreateDisplayFilter() {
			return ItemsProvider.CreateDisplayFilterCriteria(IncrementalFiltering ? AutoSearchText : string.Empty, PropertyProvider.FilterCondition);
		}
		private void UpdateDisplayFilterInTokenMode() {
			if (IsInTokenMode && IncrementalFiltering) {
				var handle = GetCurrentDataViewHandleInTokenMode();
				if (handle == null)
					return;
				object provideValue = null;
				ProvideEditValue(EditValue, out provideValue, UpdateEditorSource.TextInput);
				var editValue = provideValue as IList<object>;
				if (editValue != null) {
					if (ProvideEditValue(EditValue, out provideValue, UpdateEditorSource.TextInput))
						ItemsProvider.SetFilterCriteria(CreateDisplayFilterInTokenMode(editValue), handle);
					else
						ItemsProvider.SetDisplayFilterCriteria(null, handle);
				}
			}
		}
		CriteriaOperator CreateDisplayFilterInTokenMode(IList<object> editValue) {
			return CriteriaOperator.And(CreateDisplayFilter(), CreateValueFilter(editValue));
		}
		CriteriaOperator CreateValueFilter(IList<object> editValue) {
			return ItemsProvider.HasValueMember ? CreateValueMemberFilter(editValue) : CreateDisplayMemberFilter(editValue);
		}
		CriteriaOperator CreateDisplayMemberFilter(IList<object> editValue) {
			var values = editValue.Select(x => ItemsProvider.GetDisplayValueByEditValue(x, CurrentDataViewHandle)).ToList();
			string propertyName = ItemsProvider.GetDisplayPropertyName(GetCurrentDataViewHandleInTokenMode());
			return CreateValueFilterCore(values, propertyName);
		}
		CriteriaOperator CreateValueMemberFilter(IList<object> editValue) {
			string propertyName = ItemsProvider.GetValuePropertyName(GetCurrentDataViewHandleInTokenMode());
			return CreateValueFilterCore(new List<object>(editValue), propertyName);
		}
		CriteriaOperator CreateValueFilterCore(List<object> values, string propertyName) {
			RemoveCurrentEditableValueFromFilter(values);
			return values.Count > 0 ? new NotOperator(new InOperator(propertyName, values)) : null;
		}
		private void RemoveCurrentEditableValueFromFilter(List<object> values) {
			int index = GetCurrentEditableTokenIndex();
			if (index != -1 && index < values.Count)
				values.RemoveAt(index);
		}
		public override bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			if (IsInProcessNewValueDialog) {
				base.ProvideEditValue(value, out provideValue, updateSource);
				return false;
			}
			object editValue = value;
			LookUpEditableItem item = value as LookUpEditableItem;
			if (item == null && !IsInLookUpMode)
				return base.ProvideEditValue(value, out provideValue, updateSource);
			bool result = true;
			if (item != null) {
				if (IsInTokenMode) {
					provideValue = ProvideTokenEditValue(item);
					return !IsInProcessNewValueDialog;
				}
				if (!IsInLookUpMode && !IsAsyncServerMode)
					return base.ProvideEditValue(item.EditValue, out provideValue, updateSource);
				editValue = ProvideSingleEditValue(item);
				result = !IsInProcessNewValueDialog;
			}
			provideValue = editValue;
			return result;
		}
		private List<object> ProvideTokenEditValue(LookUpEditableItem item) {
			List<object> listEditValue = new List<object>();
			var itemEditValue = item.EditValue as IList<object>;
			if (itemEditValue != null) {
				foreach (var current in itemEditValue) {
					var currentValue = current is LookUpEditableItem ? ((LookUpEditableItem)current).EditValue : current;
					int index = ItemsProvider.IndexOfValue(currentValue, CurrentDataViewHandle);
					object candidate = null;
					if (index > -1)
						candidate = SelectorUpdater.GetEditValueFromSelectedIndex(index);
					else
						candidate = currentValue;
					if (candidate != null && !string.IsNullOrEmpty(candidate.ToString()))
						listEditValue.Add(candidate);
				}
			}
			return listEditValue.Count > 0 ? listEditValue : null;
		}
		private object ProvideSingleEditValue(LookUpEditableItem item) {
			if (item.ForbidFindIncremental)
				return item.EditValue;
			int index = FindItemIndexByText(item.DisplayValue.With(x => x.ToString()), Editor.AutoComplete);
			return index > -1 ? SelectorUpdater.GetEditValueFromSelectedIndex(index) : item.EditValue;
		}
		public virtual void AutoSeachTextChanged(string text) {
			UpdateDisplayFilter();
			VisualClient.SyncProperties(false);
		}
		protected internal override void AddILogicalOwnerChild(object child) {
			base.AddILogicalOwnerChild(child);
			ProcessNewValueHelper.SetFloatingContainer(child);
		}
		protected internal override void RemoveILogicalOwnerChild(object child) {
			base.RemoveILogicalOwnerChild(child);
			ProcessNewValueHelper.ClearFloatingContainer(child);
		}
		internal object GetSelectedItemInternal() {
			return SelectorUpdater.GetSelectedItemFromEditValue(EditValue);
		}
		public virtual void ValueMemberChanged(string valueMember) {
			object item = ItemsProvider.GetItem(ValueContainer.EditValue, CurrentDataViewHandle);
			Settings.ValueMember = valueMember;
			if (item != null)
				ValueContainer.SetEditValue(ItemsProvider.GetValueFromItem(item, CurrentDataViewHandle), UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		public virtual void DisplayMemberChanged(string displayMember) {
			Settings.DisplayMember = displayMember;
			UpdateDisplayText();
		}
		public virtual bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return VisualClient.IsClosePopupWithAcceptGesture(key, modifiers);
		}
		public virtual bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return VisualClient.IsClosePopupWithCancelGesture(key, modifiers);
		}
		internal protected override void ValidateOnEnterKeyPressed(KeyEventArgs e) {
			if (e != null && e.Handled || VisualClient.PostPopupValue || Editor.IsPopupOpen)
				return;
			if (AllowEditing && ShouldProcessNewValueOnEnter())
				ProcessValidateOnEnterLocker.DoLockedAction(() => ProcessChangeText(new ChangeTextItem() { Text = EditBox.Text }));
			string text = EditBox.Text;
			base.ValidateOnEnterKeyPressed(e);
			if (text != EditBox.Text)
				UpdateAutoSearchAfterValidate(new ChangeTextItem { Text = EditBox.Text });
			UpdateAutoSearchSelection(true);
		}
		bool ShouldProcessNewValueOnEnter() {
			string editText = EditBox.Text;
			return IsInLookUpMode && ItemsProvider.FindItemIndexByText(EditBox.Text, Editor.IsCaseSensitiveSearch, false, CurrentDataViewHandle) == -1 &&
				(IsSingleSelection ||
				(!string.IsNullOrEmpty(editText) && !editText.Contains(Editor.SeparatorString)));
		}
		public virtual void AutoCompleteChanged(bool value) {
			ItemsProvider.ResetDisplayTextCache();
		}
		public virtual void PopupDestroyed() {
			VisualClient.PopupDestroyed();
			AcceptPopupValueAction.PerformForce();
		}
		public virtual void ClearSelectionOnBackspace(bool value) {
		}
		public virtual void IsSynchronizedWithCurrentItemChanged(bool value) {
			Settings.IsSynchronizedWithCurrentItem = value;
		}
		public virtual void AllowCollectionViewChanged(bool value) {
			Settings.AllowCollectionView = value;
		}
		public virtual void ShowCustomItemsChanged(bool? value) {
			VisualClient.SyncProperties(true);
		}
		public virtual void SelectAllItems() {
			if (IsSingleSelection)
				return;
			List<object> selectedItems = new List<object>();
			selectedItems.AddRange(ItemsProvider.VisibleListSource.Cast<object>());
			ValueContainer.SetEditValue(SelectorUpdater.GetEditValueFromSelectedItems(selectedItems), UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		public virtual void UnselectAllItems() {
			object selectedItems = IsSingleSelection ? null : new List<object>();
			ValueContainer.SetEditValue(selectedItems, UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		public virtual void FindModeChanged(FindMode? findMode) {
			Settings.FindMode = findMode;
		}
		public virtual void FilterConditionChanged(FilterCondition? filterCondition) {
			Settings.FilterCondition = filterCondition;
			UpdateDisplayFilter();
		}
		protected override void AfterApplyStyleSettings() {
			base.AfterApplyStyleSettings();
			SyncWithValue();
			VisualClient.SyncProperties(true);
		}
		public virtual void AddNewCommandChanged() {
			PropertyProvider.AddNewCommand = Editor.AddNewCommand;
		}
		public virtual void SelectionModeChanged(SelectionMode? value) {
			VisualClient.SyncProperties(false);
		}
		public virtual void FilterByColumnsModeChanged(FilterByColumnsMode? value) {
			VisualClient.SyncProperties(false);
		}
		public virtual void FilterCriteriaChanged(CriteriaOperator criteriaOperator) {
			Settings.FilterCriteria = criteriaOperator;
		}
		protected internal override bool ShouldShowNullTextInternal(object editValue) {
			return base.ShouldShowNullTextInternal(editValue) && (!PropertyUpdater.HasSyncValue || IsNullValue(PropertyUpdater.SyncValue));
		}
		public virtual object GetCurrentSelectedItem() {
			return SelectorUpdater.GetCurrentSelectedItem(ValueContainer);
		}
		public virtual IEnumerable GetCurrentSelectedItems() {
			return SelectorUpdater.GetCurrentSelectedItems(ValueContainer);
		}
		protected override bool IsNativeNullValue(object value) {
			return base.IsNativeNullValue(value) || (!(value is string) && (value is IEnumerable) && !((IEnumerable)value).Cast<object>().Any());
		}
		protected override BaseEditingSettingsService CreateTextInputSettingsService() {
			return new LookUpEditBaseSettingsService(Editor);
		}
		public override void OnPreviewTextInput(TextCompositionEventArgs e) {
			base.OnPreviewTextInput(e);
			if (e.Handled)
				return;
			if (CanProcessReadOnlyAutoSearchText) {
				string text = e.Text;
				if (!string.IsNullOrEmpty(text)) {
					ProcessTextSearch(text);
					e.Handled = true;
					return;
				}
			}
			VisualClient.PreviewTextInput(e);
		}
		void ProcessTextSearch(string text) {
			object result = null;
			if (DoTextSearch(text, GetStartIndexToSeachText(), ref result)) {
				UpdateValueAndHighlightedText(result, true);
			}
			else if (string.IsNullOrEmpty(TextSearchEngine.Prefix))
				SetHighlightedText(TextSearchEngine.Prefix);
		}
		int GetStartIndexToSeachText() {
			object editValue;
			ProvideEditValue(ValueContainer.EditValue, out editValue, UpdateEditorSource.TextInput);
			return SelectorUpdater.GetIndexFromEditValue(editValue);
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
		internal void SetEditValueOnTokenEditorValueChanged() {
			ValueContainer.SetEditValue(CreateEditableItemInTokenMode(-1, new ChangeTextItem()), UpdateEditorSource.ValueChanging);
		}
		internal void OnTokenEditorTokenClosed() {
			UpdateDisplayFilterInTokenMode();
			UpdateAutoSearchText(new ChangeTextItem() { Text = string.Empty }, false);
			UpdateDisplayText();
		}
		public object GetSelectedItems(object editValue) {
			return SelectorUpdater.GetSelectedItemsFromEditValue(editValue);
		}
		public void OnTokenModeChanged() {
		}
		private void UnregisterTokenSnapshot() {
			if (TokenEditorDataViewHandle != null) {
				ItemsProvider.ReleaseSnapshot(TokenEditorDataViewHandle);
				PropertyProvider.TokenEditorDataViewHandle = null;
			}
		}
		private void RegisterTokenSnapshot() {
			PropertyProvider.TokenEditorDataViewHandle = PropertyProvider.GenerateHandle();
			ItemsProvider.RegisterSnapshot(TokenEditorDataViewHandle);
		}
		public virtual void AllowRejectUnknownValuesChanged(bool newValue) {
			Settings.AllowRejectUnknownValues = newValue;
			if (!IsInSupportInitialize)
				SyncWithValue();
		}
		public override void OnLoaded() {
			if (!PropertyProvider.CreatedFromSettings) {
				IItemsProviderOwner settings = Settings;
				settings.IsLoaded = true;
				if (ItemsProvider.IsServerMode || ItemsProvider.NeedsRefresh)
					ItemsProvider.DoRefresh();
			}
			base.OnLoaded();
		}
		protected internal override void OnUnloaded() {
			base.OnUnloaded();
			if (!PropertyProvider.CreatedFromSettings) {
				IItemsProviderOwner settings = Settings;
				settings.IsLoaded = false;
				if (ItemsProvider.IsServerMode)
					ItemsProvider.DoRefresh();
			}
		}
	}
	public class UpdateDataSourceOnAsyncProcessNewValueEventHelper : IDisposable {
		LookUpEditBase Editor { get; set; }
		FrameworkElement Window { get; set; }
		bool DoUpdate { get; set; }
		Locker InProcessLocker { get; set; }
		public bool LockerByProcessNewValueWindow { get { return IsInProcessNewValue && Window != null; } }
		public bool IsInProcessNewValue { get { return InProcessLocker.IsLocked; } }
		public UpdateDataSourceOnAsyncProcessNewValueEventHelper(LookUpEditBase editor) {
			Editor = editor;
			InProcessLocker = new Locker();
		}
		public void SetFloatingContainer(object element) {
			if (!InProcessLocker)
				return;
			Window = (FrameworkElement)element;
			Window.Focus();
		}
		internal void ClearFloatingContainer(object child) {
			if (!object.ReferenceEquals(child, Window))
				return;
			UpdateDataInternal(DoUpdate);
			DisposeInternal();
		}
		void ResetWindow() {
			Window = null;
		}
		void UpdateDataInternal(bool force) {
			if (!force)
				return;
			Editor.ItemsProvider.DoRefresh();
			Editor.EditStrategy.ValidateOnEnterKeyPressed(null);
		}
		public void UpdateDataAsync() {
			if (Window != null)
				DoUpdate = true;
			else
				UpdateDataInternal(true);
		}
		internal IDisposable Subscribe() {
			InProcessLocker.Lock();
			return this;
		}
		internal void DisposeInternal() {
			InProcessLocker.Unlock();
			ResetWindow();
			if (!Editor.GetIsEditorKeyboardFocused())
				Editor.Focus();
			DoUpdate = false;
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			if (!InProcessLocker || DoUpdate)
				return;
			DisposeInternal();
		}
		#endregion
	}
	public class ChangeTextItem {
		public string Text { get; set; }
		public bool UpdateAutoCompleteSelection { get; set; }
		public bool AutoCompleteTextDeleted { get; set; }
		public bool AsyncLoading { get; set; }
	}
	public class LookUpEditableItem : ICustomItem {
		public bool Completed { get; set; }
		public ChangeTextItem ChangeTextItem { get; set; }
		public object DisplayValue { get; set; }
		public object EditValue { get; set; }
		public bool ProcessNewValueCompleted { get; set; }
		public bool AsyncLoading { get; set; }
		public bool AutoCompleteTextDeleted { get; set; }
		public bool ForbidFindIncremental { get; set; }
	}
	public class EditorTextSearchHelper {
		public EditorTextSearchHelper(ISelectorEditStrategy editStrategy) {
			EditStrategy = editStrategy;
		}
		object Handle { get { return EditStrategy.CurrentDataViewHandle; } }
		IItemsProvider2 ItemsProvider { get { return EditStrategy.ItemsProvider; } }
		TextSearchEngine SeachEngine { get { return EditStrategy.TextSearchEngine; } }
		ISelectorEditStrategy EditStrategy { get; set; }
		public bool DoTextSearch(string text, int startIndex, ref object result) {
			if (SeachEngine.DoSearch(text, startIndex)) {
				int listSourceIndex = ItemsProvider.GetIndexByControllerIndex(SeachEngine.MatchedItemIndex, Handle);
				result = ItemsProvider.GetValueByIndex(listSourceIndex, Handle);
			}
			return result != null;
		}
		public object FindValueFromSearchText(int startIndex, bool isDown, bool isCaseSensitiveSearch) {
			int index = ItemsProvider.FindItemIndexByText(SeachEngine.SeachText, isCaseSensitiveSearch, true, Handle, startIndex, isDown, true);
			return index != -1 ? ItemsProvider.GetValueByIndex(index, Handle) : null;
		}
	}
}
