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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Grid {
	public enum DefaultFilterItem {
		PopupFilterAll,
		PopupFilterBlanks,
		PopupFilterNonBlanks,
	}
	public abstract class ColumnFilterInfoBase {
		public ColumnBase Column { get; private set; }
		protected DataViewBase View { get { return Column.Owner as DataViewBase; } }
		protected abstract bool ImmediateUpdateFilter { get; }
		PopupBaseEdit currentPopup;
		public ColumnFilterInfoBase(ColumnBase column) {
			Column = column;
		}
		internal void UpdateCurrentPopupData(object[] values) {
			if(currentPopup == null) return;
			UpdatePopupData(currentPopup, values);
		}
		internal void OnPopupOpening(PopupBaseEdit popup) {
			View.IsColumnFilterLoaded = false;
			View.IsColumnFilterOpened = true;
			currentPopup = popup;
			UpdatePopupData(popup);
			UpdatePopupButtonsVisibility(popup);
			UpdateSizeGripVisibility(popup);
			RaiseFilterPopupEvent(popup);
			AfterPopupOpening(popup);
			View.IsColumnFilterLoaded = true;
		}
		internal void OnPopupOpened(PopupBaseEdit popup) {
			View.KeyboardLocker.Lock();
			OnPopupOpenedCore(popup);
		}
		protected virtual void OnPopupOpenedCore(PopupBaseEdit popup) { }
		internal virtual void OnPopupClosed(PopupBaseEdit popup, bool applyFilter) {
			View.IsColumnFilterOpened = false;
			if(applyFilter && !ImmediateUpdateFilter) ApplyFilter(GetFilterCriteria(popup), false);
			ClearPopupData(popup);
			currentPopup = null;
			View.KeyboardLocker.Unlock();
		}
		protected virtual void AfterPopupOpening(PopupBaseEdit popup) {
		}
		protected virtual void RaiseFilterPopupEvent(PopupBaseEdit popup) {
			if(View != null)
				View.RaiseFilterPopupEvent(Column, popup);
		}
		protected void UpdateColumnFilterIfNeeded(Func<CriteriaOperator> getOperator) {
			if(ImmediateUpdateFilter && View.KeyboardLocker.IsLocked) ApplyFilter(getOperator(), true);
		}
#if DEBUGTEST
		internal
#endif
		void ApplyFilter(CriteriaOperator op, bool updateImmediately) {
			if(updateImmediately)
				Column.ApplyColumnFilter(op);
			else if(View != null)
				View.EnqueueImmediateAction(() => Column.ApplyColumnFilter(op));
		}
		internal abstract PopupBaseEdit CreateColumnFilterPopup();
		protected abstract void UpdatePopupData(PopupBaseEdit popup);
		protected virtual void UpdatePopupData(PopupBaseEdit popup, object[] values) { }
		protected virtual void UpdatePopupButtonsVisibility(PopupBaseEdit popup) {
			popup.PopupFooterButtons = ShowButtons();
		}
		protected virtual void UpdateSizeGripVisibility(PopupBaseEdit popup) {
			popup.ShowSizeGrip = true;
		}
		protected virtual PopupFooterButtons ShowButtons() {
			if(ImmediateUpdateFilter)
				return PopupFooterButtons.None;
			return PopupFooterButtons.OkCancel;
		}
		protected abstract CriteriaOperator GetFilterCriteria(PopupBaseEdit popup);
		protected abstract void ClearPopupData(PopupBaseEdit popup);
	}
	public class ListColumnFilterInfo : ColumnFilterInfoBase {
		protected override bool ImmediateUpdateFilter { get { return false; } }
		protected List<CustomComboBoxItem> ColumnMRUFilters { get; set; }
		internal virtual void AddColumnMRUFilter(CustomComboBoxItem filter) {
			if(filter == null) return;
			for(int i = ColumnMRUFilters.Count - 1; i > -1; i--) {
				if(AreCustomItemsEquals(ColumnMRUFilters[i], filter)) {
					ColumnMRUFilters.Remove(ColumnMRUFilters[i]);
				}
			}
			ColumnMRUFilters.Insert(0, filter);
			if(ColumnMRUFilters.Count > View.DataControl.MRUColumnFilterListCount + 1) {
				ColumnMRUFilters.RemoveAt(View.DataControl.MRUColumnFilterListCount + 1);
			}
		}
		void ClearColumnMRUFilter() {
			ColumnMRUFilters.Clear();
		}
		void TruncateColumnMRUFilterToMaxCount() {
			for(int i = View.DataControl.MRUColumnFilterListCount + 1; i < ColumnMRUFilters.Count; i++) {
				ColumnMRUFilters.RemoveAt(i);
			}
		}
		internal override void OnPopupClosed(PopupBaseEdit popup, bool applyFilter) {
			if(applyFilter && !ImmediateUpdateFilter) {
				CustomComboBoxItem selectedItem = ((ComboBoxEdit)popup).SelectedItem as CustomComboBoxItem;
				bool isLastItemDefault = CheckIsItemDefault(selectedItem);
				if(!isLastItemDefault) {
					AddColumnMRUFilter(selectedItem);
				}
			}
			base.OnPopupClosed(popup, applyFilter);
		}
		protected override PopupFooterButtons ShowButtons() {
			return PopupFooterButtons.None;
		}
		protected ComboBoxEditSettings comboBoxEditSettings { get { return Column.EditSettings as ComboBoxEditSettings; } }
		internal override PopupBaseEdit CreateColumnFilterPopup() {
			ComboBoxEdit comboBox = CreateComboBoxEdit();
			comboBox.StyleSettings = new ComboBoxStyleSettings();
			comboBox.ShowNullText = false;
			comboBox.IsTextEditable = false;
			comboBox.AutoComplete = true;
			comboBox.FocusPopupOnOpen = false;
			return comboBox;
		}
		protected ComboBoxEdit CreateComboBoxEdit() {
			return comboBoxEditSettings != null ? (ComboBoxEdit)comboBoxEditSettings.CreateEditor(false, EmptyDefaultEditorViewInfo.Instance, EditorOptimizationMode.Disabled) : new ComboBoxEdit();
		}
		public ListColumnFilterInfo(ColumnBase column)
			: base(column) {
			ColumnMRUFilters = new List<CustomComboBoxItem>();
		}
		protected override void UpdatePopupData(PopupBaseEdit popup) {
			UpdatePopupData(popup, null);
		}
		protected override void UpdatePopupData(PopupBaseEdit popup, object[] values) {
			ComboBoxEdit comboBox = (ComboBoxEdit)popup;
			if(View.HasValidationError) {
				comboBox.ItemsSource = null;
				return;
			}
			List<object> list = GetColumnFilterItems(values);
			comboBox.ItemsPanel = FilterPopupVirtualizingStackPanel.GetItemsPanelTemplate(IsColumnFilterItemsLoading ? Int32.MaxValue : list.Count);
			comboBox.ItemsSource = list;
			comboBox.ShowCustomItems = CanShowCustomItems();
		}
		bool? CanShowCustomItems() {
			if(IsColumnFilterItemsLoading)
				return false;
			return null;
		}
		protected override CriteriaOperator GetFilterCriteria(PopupBaseEdit popup) {
			object selectedItem = ((ComboBoxEdit)popup).SelectedItem;
			if(selectedItem == null)
				return null;
			CustomComboBoxItem customItem = selectedItem as CustomComboBoxItem;
			if(customItem == null) {
				return CreateEqualsOperator(GetValue(selectedItem));
			}
			else return GetFilterCriteria(customItem);
		}
		protected CriteriaOperator GetFilterCriteria(CustomComboBoxItem customItem) {
			if(customItem == null) return null;
			if(customItem.EditValue is CustomComboBoxItem)
				return (customItem.EditValue as CustomComboBoxItem).EditValue as CriteriaOperator;
			if(customItem.EditValue is CriteriaOperator)
				return customItem.EditValue as CriteriaOperator;
			return CreateEqualsOperator(customItem.EditValue);
		}
		CriteriaOperator CreateEqualsOperator(object value) {
			return View.DataControl.CalcColumnFilterCriteriaByValue(Column, value);
		}
		protected override void ClearPopupData(PopupBaseEdit popup) {
			((ComboBoxEdit)popup).ItemsSource = null;
		}
		void AddMRUItemsToList(List<object> list) {
			if(View.DataControl.AllowColumnMRUFilterList) {
				TruncateColumnMRUFilterToMaxCount();
				CriteriaOperator columnCriteria = View.DataControl.GetColumnFilterCriteria(Column);
				foreach(CustomComboBoxItem item in ColumnMRUFilters) {
					CriteriaOperator itemCriteria = GetFilterCriteria(item);
					if(!Object.Equals(itemCriteria, columnCriteria) && list.Count < View.DataControl.MRUColumnFilterListCount) {
						list.Add(item);
					}
				}
#if !SL
				if(list.Count != 0) {
					list.Add(CreateSeparator());
				}
#endif
			}
		}
		object CreateSeparator() {
			var separator = new CustomItem();
			separator.ItemContainerStyle = new Style(typeof(ComboBoxEditItem));
			separator.ItemContainerStyle.Setters.Add(new Setter(ComboBoxEditItem.IsEnabledProperty, false));
			separator.ItemContainerStyle.Setters.Add(new Setter(ComboBoxEditItem.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
			separator.EditValue = new object();
			separator.ItemTemplate = XamlHelper.GetTemplate("<Separator />");
			return separator;
		}
		bool IsWaitingAsyncData(object[] values) {
#if DEBUGTEST
			if(isWaitingAsyncDataDebugTest) return true;
#endif
			return values.Length > 0 && DevExpress.Data.AsyncServerModeDataController.IsNoValue(values[0]);
		}
		bool IsColumnFilterItemsLoading { get; set; }
		List<object> GetColumnFilterItems(object[] values) {
			IsColumnFilterItemsLoading = false;
			List<object> list = new List<object>();
			AddMRUItemsToList(list);
			list.AddRange(GetDefaultItems(Column.IsFiltered));
			if(View == null)
				return list;
			if(values == null)
				values = View.DataControl.GetUniqueColumnValues(Column, GetIncludeFilteredOut(), Column.RoundDateTimeForColumnFilter);
			if(values == null)
				return list;
			if(IsWaitingAsyncData(values)) {
				WaitIndicatorItem waitIndicatorItem = new WaitIndicatorItem() { IsHitTestVisible = false };
				list.Add(waitIndicatorItem);
				IsColumnFilterItemsLoading = true;
				return list;
			}
			if(comboBoxEditSettings != null && Column.ColumnFilterMode != ColumnFilterMode.DisplayText) {
				Array.Sort(values, new ComboBoxItemComparer(comboBoxEditSettings));
			}
			for(int i = 0; i < values.Length; i++) {
				values[i] = GetItem(values[i]);
			}
			if(Column.ColumnFilterMode == ColumnFilterMode.DisplayText || (Column.GetSortMode() == DevExpress.XtraGrid.ColumnSortMode.DisplayText && comboBoxEditSettings == null)) {
				Array.Sort(values, new CustomComboBoxItemComparer());
			}
			int j = 0;
			emptyIncluded = false;
			foreach(object obj in values) {
				if(j == Column.ColumnFilterPopupMaxRecordsCount)
					break;
				if(ShouldIncludeItem(obj)) {
					list.Add(obj);
					j++;
				}
			}
			return list;
		}
		bool emptyIncluded = false;
		bool ShouldIncludeItem(object item) {
			if(item == null) return false;
			CustomComboBoxItem customComboBoxItem = item as CustomComboBoxItem;
			if(customComboBoxItem != null) {
				string editValue = customComboBoxItem.EditValue as String;
				if(View.DataControl.ImplyNullLikeEmptyStringWhenFiltering && (editValue == String.Empty)) {
					if(emptyIncluded) {
						return false;
					} else {
						emptyIncluded = true;
						return true;
					}
				}
				if((editValue == String.Empty) && (Column.ColumnFilterMode == ColumnFilterMode.DisplayText)) {
					return false;
				}
			}
			return true;
		}
		List<object> GetColumnFilterItems() {
			return GetColumnFilterItems(null);
		}
		protected virtual bool GetIncludeFilteredOut() {
			return Column.GetShowAllTableValuesInFilterPopup() || Column.IsFiltered;
		}
		protected object GetValue(object item) {
			if(item is CustomComboBoxItem)
				return (item as CustomComboBoxItem).EditValue;
			if(comboBoxEditSettings != null)
				return comboBoxEditSettings.GetValueFromItem(item);
			return item;
		}
		protected virtual object GetItem(object value) {
			if(View.DataControl.ImplyNullLikeEmptyStringWhenFiltering && (value == null || value is System.DBNull))
				value = string.Empty;
			if(Column.ColumnFilterMode == ColumnFilterMode.DisplayText)
				return new CustomComboBoxItem() { EditValue = value, DisplayValue = value };
			object item;
			if(GetComboBoxItem(value, out item)) return item;
			return new CustomComboBoxItem() { EditValue = value, DisplayValue = View.GetColumnDisplayText(value, Column) };
		}
		protected bool GetComboBoxItem(object value, out object item) {
			item = null;
			if(comboBoxEditSettings != null) {
				if(!string.IsNullOrEmpty(comboBoxEditSettings.ValueMember)) {
					item = comboBoxEditSettings.GetItemFromValue(value);
					return true;
				}
				if(comboBoxEditSettings.ItemTemplate != null) {
					item = value;
					return true;
				}
			}
			return false;
		}
		protected virtual List<object> GetDefaultItems(bool addShowAllItem) {
			List<object> defaultItems = new List<object>();
			if(addShowAllItem) {
				defaultItems.Add(new CustomComboBoxItem() { DisplayValue = View.GetDefaultFilterItemLocalizedString(DefaultFilterItem.PopupFilterAll), EditValue = new CustomComboBoxItem() });
			}
			defaultItems.Add(new CustomComboBoxItem() { DisplayValue = View.GetDefaultFilterItemLocalizedString(DefaultFilterItem.PopupFilterBlanks), EditValue = new CustomComboBoxItem() { EditValue = GetCriteriaOperator(true) } });
			defaultItems.Add(new CustomComboBoxItem() { DisplayValue = View.GetDefaultFilterItemLocalizedString(DefaultFilterItem.PopupFilterNonBlanks), EditValue = new CustomComboBoxItem() { EditValue = GetCriteriaOperator(false) } });
			return defaultItems;
		}
		protected bool CheckIsItemDefault(CustomComboBoxItem item) {
			List<object> defaultItems = GetDefaultItems(true);
			foreach(object obj in defaultItems) {
				if(AreCustomItemsEquals(obj, item)) {
					return true;
				}
			}
			return false;
		}
		protected bool AreCustomItemsEquals(object obj1, object obj2) {
			CustomComboBoxItem item1 = obj1 as CustomComboBoxItem;
			CustomComboBoxItem item2 = obj2 as CustomComboBoxItem;
			if(item1 == null || item2 == null) return false;
			return Object.Equals(item1.DisplayValue, item2.DisplayValue);
		}
		CriteriaOperator GetCriteriaOperator(bool isBlanks) {
			if(!IsStringColumn()) {
				if(isBlanks)
					return new OperandProperty(Column.FieldName).IsNull();
				else
					return new OperandProperty(Column.FieldName).IsNotNull();
			}
			else {
				if(isBlanks)
					return new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(Column.FieldName));
				else
					return new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(Column.FieldName)).Not();
			}
		}
		protected bool IsStringColumn() {
			DevExpress.Data.DataColumnInfo columnInfo = View.DataProviderBase.Columns[Column.FieldName];
			return (Column.ColumnFilterMode == ColumnFilterMode.DisplayText || columnInfo == null || columnInfo.Type.Equals(typeof(string))) && !(Column.EditSettings is CheckEditSettings);
		}
#if DEBUGTEST
		public static bool isWaitingAsyncDataDebugTest;
#endif
	}
	class ComboBoxItemComparer : IComparer {
		ComboBoxEditSettings comboBoxEditSettings;
		public ComboBoxItemComparer(ComboBoxEditSettings comboBoxEditSettings) {
			this.comboBoxEditSettings = comboBoxEditSettings;
		}
		public int Compare(object x, object y) {
			return Comparer<string>.Default.Compare(comboBoxEditSettings.GetDisplayText(x, true), comboBoxEditSettings.GetDisplayText(y, true));
		}
	}
	class CustomComboBoxItemComparer : IComparer {
		public int Compare(object x, object y) {
			CustomComboBoxItem item1 = x as CustomComboBoxItem, item2 = y as CustomComboBoxItem;
			if(item1 == item2)
				return 0;
			if(item1 == null)
				return -1;
			if(item2 == null)
				return 1;
			return Comparer<object>.Default.Compare(item1.DisplayValue, item2.DisplayValue);
		}
	}
	public class CheckedListColumnFilterInfo : ListColumnFilterInfo {
		bool isSelectionLocked = false;
		internal override void AddColumnMRUFilter(CustomComboBoxItem filter) {
		}
		protected override PopupFooterButtons ShowButtons() {
			if(ImmediateUpdateFilter)
				return PopupFooterButtons.None;
			return PopupFooterButtons.OkCancel;
		}
		protected override bool ImmediateUpdateFilter { get { return Column.ImmediateUpdateColumnFilter; } }
		public CheckedListColumnFilterInfo(ColumnBase column)
			: base(column) {
		}
		internal override PopupBaseEdit CreateColumnFilterPopup() {
			ComboBoxEdit comboBoxEdit = CreateComboBoxEdit();
			comboBoxEdit.StyleSettings = new CheckedComboBoxStyleSettings();
			comboBoxEdit.ShowNullText = false;
			comboBoxEdit.IsTextEditable = false;
			comboBoxEdit.AutoComplete = true;
			comboBoxEdit.FocusPopupOnOpen = true;
			return comboBoxEdit;
		}
		protected override List<object> GetDefaultItems(bool addShowAllItem) {
			return new List<object>();
		}
		protected override bool GetIncludeFilteredOut() {
			return Column.GetShowAllTableValuesInCheckedFilterPopup() || Column.IsFiltered;
		}
		protected override CriteriaOperator GetFilterCriteria(PopupBaseEdit popup) {
			return View.GetCheckedFilterPopupFilterCriteria(Column, SelectedItems);
		}
		internal CriteriaOperator GetFilterCriteriaCore() {
			IEnumerable<DateTime> preparedList = PrepareListOfDateTimeItems(SelectedItems);
			if(preparedList != null && Column.RoundDateTimeForColumnFilter)
				return MultiselectRoundedDateTimeFilterHelper.DatesToCriteria(Column.FieldName, preparedList);
			else
				return CreateCriteriaOperator(SelectedItems);
		}
		protected override void OnPopupOpenedCore(PopupBaseEdit popup) {
			base.OnPopupOpenedCore(popup);
			ComboBoxEdit comboBox = (ComboBoxEdit)popup;
			RecreateSelectedItems(comboBox);
			comboBox.PopupContentSelectionChanged += new SelectionChangedEventHandler(PopupListBoxSelectionChanged);
		}
		protected override void UpdatePopupData(PopupBaseEdit popup, object[] values) {
			base.UpdatePopupData(popup, values);
			ComboBoxEdit comboBox = (ComboBoxEdit)popup;
			RecreateSelectedItems(comboBox);
		}
		protected override void ClearPopupData(PopupBaseEdit popup) {
			base.ClearPopupData(popup);
			((ComboBoxEdit)popup).PopupContentSelectionChanged -= new SelectionChangedEventHandler(PopupListBoxSelectionChanged);
		}
		protected object FindItem(IEnumerable source, object value) {
			if(Column.ColumnFilterMode != ColumnFilterMode.DisplayText) {
				object item;
				if(GetComboBoxItem(value, out item)) return item;
			}
			foreach(object item in source)
				if(item is CustomComboBoxItem && object.Equals((item as CustomComboBoxItem).EditValue, value))
					return item;
			return null;
		}
		List<object> SelectedItems = new List<object>();
		void RecreateSelectedItems(ComboBoxEdit comboBox) {
			comboBox.SelectedItems.Clear();
			SelectedItems.Clear();
			CriteriaOperator op = View.DataControl.GetColumnFilterCriteria(Column);
			if(Object.ReferenceEquals(op, null))
				return;
			AddSelectedItems(comboBox, GetSelectedItems(comboBox, op));
		}
		IEnumerable GetSelectedItems(ComboBoxEdit comboBox, CriteriaOperator op) {
			return View.GetCheckedFilterPopupSelectedItems(Column, comboBox, op);
		}
		internal IEnumerable GetSelectedItemsCore(ComboBoxEdit comboBox, CriteriaOperator op) {
			IEnumerable<DateTime> dateTimeItems = PrepareListOfDateTimeItems((IEnumerable)comboBox.ItemsSource);
			if(dateTimeItems != null)
				return GetSelectedItemsForDateTime(op, dateTimeItems);
			else
				return GetSelectedItemsCore(op);
		}
		IEnumerable GetSelectedItemsCore(CriteriaOperator op) {
			List<object> selectedItems = new List<object>();
			if(ContainsIsNullOrEmptyOperator(op))
				selectedItems.Add(string.Empty);
			InOperator inOperator = op as InOperator;
			GroupOperator groupOperator = op as GroupOperator;
			if(IsValidGroupOperator(groupOperator))
				inOperator = (InOperator)groupOperator.Operands.FirstOrDefault(operand => operand is InOperator);
			if(!ReferenceEquals(inOperator, null))
				selectedItems.AddRange(inOperator.Operands.Select(opValue => ((OperandValue)opValue).Value));
			return selectedItems;
		}
		bool IsValidGroupOperator(GroupOperator groupOperator) {
			if(ReferenceEquals(groupOperator, null))
				return false;
			if(groupOperator.OperatorType != GroupOperatorType.Or || groupOperator.Operands.Count != 2)
				return false;
			return !ReferenceEquals(groupOperator.Operands.FirstOrDefault(operand => IsNullOrEmptyOperator(operand)), null) 
				&& !ReferenceEquals(groupOperator.Operands.FirstOrDefault(operand => operand is InOperator), null);
		}
		bool ContainsIsNullOrEmptyOperator(CriteriaOperator op) {
			return IsNullOrEmptyOperator(op) || IsValidGroupOperator(op as GroupOperator);
		}
		bool IsNullOrEmptyOperator(CriteriaOperator op) {
			return op is FunctionOperator && ((FunctionOperator)op).OperatorType == FunctionOperatorType.IsNullOrEmpty;
		}
		internal IEnumerable GetSelectedItemsForDateTime(CriteriaOperator op, IEnumerable<DateTime> dateTimeItems) {
			if(dateTimeItems == null)
				return null;
			IEnumerable<DateTime> checkedItems = MultiselectRoundedDateTimeFilterHelper.GetCheckedDates(op, Column.FieldName, dateTimeItems);
			return checkedItems;
		}
		void AddSelectedItems(ComboBoxEdit comboBox, IEnumerable values) {
			if(values == null)
				return;
			SelectComboBoxItems(comboBox, values);
			foreach(object item in comboBox.SelectedItems)
				SelectedItems.Add(item);
		}
		void BeginSelection(ComboBoxEdit comboBox) {
			isSelectionLocked = true;
			comboBox.BeginInit();
		}
		void EndSelection(ComboBoxEdit comboBox) {
			comboBox.EndInit();
			isSelectionLocked = false;
		}
		void SelectComboBoxItems(ComboBoxEdit comboBox, IEnumerable values) {
			BeginSelection(comboBox);
			foreach(object value in values) {
				object sourceValue = value;
				if(View.DataControl.ImplyNullLikeEmptyStringWhenFiltering && value == null)
					sourceValue = string.Empty;
				object item = FindItem((IEnumerable)comboBox.ItemsSource, sourceValue);
				if(item != null)
					comboBox.SelectedItems.Add(item);
			}
			EndSelection(comboBox);
		}
		void PopupListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(isSelectionLocked)
				return;
			foreach(object obj in e.AddedItems)
				SelectedItems.Add(obj);
			foreach(object obj in e.RemovedItems)
				SelectedItems.Remove(obj);
			UpdateColumnFilterIfNeeded(() => GetFilterCriteria(sender as PopupBaseEdit));
		}
		IEnumerable<DateTime> PrepareListOfDateTimeItems(IEnumerable items) {
			IList<DateTime> res = new List<DateTime>();
			foreach(object item in items) {
				CustomComboBoxItem comboItem = item as CustomComboBoxItem;
				if(item is DateTime)
					res.Add((DateTime)item);
				else if(comboItem != null && comboItem.EditValue is DateTime)
					res.Add((DateTime)comboItem.EditValue);
				else
					return null;
			}
			return res;
		}
		CriteriaOperator CreateCriteriaOperator(IEnumerable items) {
			List<CriteriaOperator> list = new List<CriteriaOperator>();
			bool showBlanks = false;
			foreach(object item in items) {
				object value = GetValue(item);
				if(View.DataControl.ImplyNullLikeEmptyStringWhenFiltering && string.IsNullOrEmpty(value.ToString())) {
					showBlanks = true;
				} else {
					list.Add(new OperandValue(value));
				}
			}
			CriteriaOperator blanksOperator = new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(Column.FieldName));
			CriteriaOperator op = null;
			if(list.Count > 0) {
				InOperator inOperator = new InOperator(new OperandProperty(Column.FieldName));
				inOperator.Operands.AddRange(list);
				op = inOperator;
			}
			if(showBlanks)
				op |= blanksOperator;
			return op;
		}
		protected override object GetItem(object value) {
			if(View.DataControl.ImplyNullLikeEmptyStringWhenFiltering && (value == null || string.IsNullOrEmpty(value.ToString()) || value is System.DBNull))
				return new CustomComboBoxItem() { EditValue = string.Empty, DisplayValue = View.GetDefaultFilterItemLocalizedString(DefaultFilterItem.PopupFilterBlanks) };
			return base.GetItem(value);
		}
	}
	public class CustomColumnFilterInfo : ColumnFilterInfoBase {
		protected override bool ImmediateUpdateFilter { get { return Column.ImmediateUpdateColumnFilter; } }
		public CustomColumnFilterInfo(ColumnBase column)
			: base(column) {
		}
		internal override PopupBaseEdit CreateColumnFilterPopup() {
			return new PopupBaseEdit() { ShowNullText = false, IsTextEditable = false };
		}
		protected override void UpdatePopupData(PopupBaseEdit popup) {
			CustomColumnFilter = View.DataControl.GetColumnFilterCriteria(Column);
			popup.DataContext = this;
			popup.PopupContentTemplate = CreatePopupTemplate();
		}
		ControlTemplate CreatePopupTemplate() {
			string templateString = @"<ControlTemplate " +
#if !SILVERLIGHT
 @"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" " +
#else
 @"xmlns=""http://schemas.microsoft.com/client/2007"" " +
#endif
 @"xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid"">
                                        <dxg:CustomColumnFilterContentPresenter " +
#if !SILVERLIGHT
 @"ColumnFilterInfo=""{Binding Path=DataContext, RelativeSource={RelativeSource TemplatedParent}}""" +
#else
 @"ColumnFilterInfo=""{Binding Path=DataContext.DataContext, RelativeSource={RelativeSource TemplatedParent}}""" +
#endif
 @"/>
                                    </ControlTemplate>";
#if !SILVERLIGHT
			return XamlReader.Parse(templateString) as ControlTemplate;
#else
			return XamlReader.Load(templateString) as ControlTemplate;
#endif
		}
		protected override void ClearPopupData(PopupBaseEdit popup) {
		}
		CriteriaOperator customColumnFilter;
		internal CriteriaOperator CustomColumnFilter {
			get {
				return customColumnFilter;
			}
			set {
				customColumnFilter = value;
				UpdateColumnFilterIfNeeded(() => CustomColumnFilter);
			}
		}
		protected override CriteriaOperator GetFilterCriteria(PopupBaseEdit popup) {
			return CustomColumnFilter;
		}
	}
	[DXToolboxBrowsable(false)]
	public class CustomColumnFilterContentPresenter : ContentPresenter {
		public static readonly DependencyProperty CustomColumnFilterProperty;
		public static readonly DependencyProperty ColumnFilterInfoProperty;
		static CustomColumnFilterContentPresenter() {
			CustomColumnFilterProperty = DependencyProperty.Register("CustomColumnFilter", typeof(CriteriaOperator), typeof(CustomColumnFilterContentPresenter), new PropertyMetadata(null, OnCustomColumnFilterChanged));
			ColumnFilterInfoProperty = DependencyProperty.Register("ColumnFilterInfo", typeof(CustomColumnFilterInfo), typeof(CustomColumnFilterContentPresenter), new PropertyMetadata(null, OnColumnFilterInfoChanged));
		}
		static void OnCustomColumnFilterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CustomColumnFilterContentPresenter)obj).OnCustomColumnFilterChanged();
		}
		static void OnColumnFilterInfoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((CustomColumnFilterContentPresenter)obj).OnColumnFilterInfoChanged();
		}
		public CriteriaOperator CustomColumnFilter {
			get { return (CriteriaOperator)GetValue(CustomColumnFilterProperty); }
			set { SetValue(CustomColumnFilterProperty, value); }
		}
		public CustomColumnFilterInfo ColumnFilterInfo {
			get { return (CustomColumnFilterInfo)GetValue(ColumnFilterInfoProperty); }
			set { SetValue(ColumnFilterInfoProperty, value); }
		}
		void OnCustomColumnFilterChanged() {
			if(ColumnFilterInfo != null)
				ColumnFilterInfo.CustomColumnFilter = CustomColumnFilter;
		}
		void OnColumnFilterInfoChanged() {
			if(ColumnFilterInfo != null) {
				CustomColumnFilter = ColumnFilterInfo.CustomColumnFilter;
				ContentTemplate = ColumnFilterInfo.Column.CustomColumnFilterPopupTemplate;
			}
		}
	}
	public class GridFilterColumn : FilterColumn {
		internal ColumnBase GridColumn { get; set; }
		bool UseDomainDataSourceRestrictions { get; set; }
		bool UseWcfSource { get; set; }
		public GridFilterColumn(ColumnBase column, bool useDomainDataSourceRestrictions, bool useWcfSource)
			: base() {
			GridColumn = column;
			UseDomainDataSourceRestrictions = useDomainDataSourceRestrictions;
			UseWcfSource = useWcfSource;
		}
		public static List<ClauseType> DomainDataSourceClauses = new List<ClauseType> { ClauseType.Contains, ClauseType.EndsWith, ClauseType.BeginsWith, ClauseType.Equals, ClauseType.Greater, ClauseType.GreaterOrEqual, ClauseType.Less, ClauseType.LessOrEqual, ClauseType.DoesNotEqual };
		public override bool IsValidClause(ClauseType clause) {
			bool res = base.IsValidClause(clause);
			if(UseDomainDataSourceRestrictions) {
				if(DomainDataSourceClauses.Contains(clause))
					return res;
				return false;
			}
			if(UseWcfSource) {
				if(clause == ClauseType.Like || clause == ClauseType.NotLike || clause == ClauseType.IsToday
					|| clause == ClauseType.IsYesterday || clause == ClauseType.IsLastWeek || clause == ClauseType.IsPriorThisYear
					|| clause == ClauseType.IsTomorrow || clause == ClauseType.IsNextWeek || clause == ClauseType.IsBeyondThisYear
					|| clause == ClauseType.IsEarlierThisWeek || clause == ClauseType.IsEarlierThisMonth || clause == ClauseType.IsEarlierThisYear
					|| clause == ClauseType.IsLaterThisWeek || clause == ClauseType.IsLaterThisMonth || clause == ClauseType.IsLaterThisYear)
					return false;
			}
			return res;
		}
		public override FilterColumnClauseClass ClauseClass {
			get {
				if(GridColumn.ColumnFilterMode == ColumnFilterMode.DisplayText) {
					return FilterColumnClauseClass.String;
				}
				else if((GridColumn.EditSettings is ComboBoxEditSettings) || (GridColumn.EditSettings is ListBoxEditSettings)) {
					return FilterColumnClauseClass.Lookup;
				} else if((GridColumn.EditSettings is ImageEditSettings) || ColumnType == typeof(byte[])) {
					return FilterColumnClauseClass.Blob;
				}
				else if(ColumnType == typeof(string)) {
					return FilterColumnClauseClass.String;
				}
				else if(ColumnType == typeof(DateTime) || ColumnType == typeof(DateTime?)) {
					return FilterColumnClauseClass.DateTime;
				}  else {
					return FilterColumnClauseClass.Generic;
				}
			}
		}
	}
}
