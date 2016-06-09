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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Mvvm.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
	public class SelectorPropertiesCoercionHelper {
		Locker SyncWithICollectionViewLocker { get; set; }
		bool IsInLookUpMode { get { return EditStrategy.IsInLookUpMode; } }
		ISelectorEditStrategy EditStrategy { get; set; }
		ISelectorEdit Editor { get { return EditStrategy.Editor; } }
		IItemsProvider2 ItemsProvider { get { return EditStrategy.ItemsProvider; } }
		public SelectorPropertiesCoercionHelper() {
			SyncWithICollectionViewLocker = new Locker();
		}
		public void SetOwner(ISelectorEditStrategy strategy) {
			EditStrategy = strategy;
		}
		public void RaiseSelectedIndexChangedEvent(object oldValue, object newValue) {
			int oldIndex = GetIndexFromEditValue(oldValue);
			int newIndex = GetIndexFromEditValue(newValue);
			if (oldIndex.Equals(newIndex))
				return;
			Editor.RaiseEvent(new RoutedEventArgs(EditStrategy.SelectedIndexChangedEvent));
		}
		public object CoerceSelectedItem(object baseItem) {
			return baseItem;
		}
		public IList CoerceSelectedItemsSource(IList value) {
			return value;
		}
		public int CoerceSelectedIndex(int baseIndex) {
			return baseIndex;
		}
		public int GetIndexFromEditValue(object editValue) {
			IList list = editValue as IList;
			if (list != null && list.Count > 0) {
				foreach (object editValueItem in list) {
					int index = ItemsProvider.IndexOfValue(editValueItem, EditStrategy.CurrentDataViewHandle);
					if (index > -1)
						return index;
				}
			}
			return ItemsProvider.IndexOfValue(editValue, EditStrategy.CurrentDataViewHandle);
		}
		public object GetSelectedItemFromEditValue(object editValue) {
			IList list = editValue as IList;
			if (list != null && list.Count > 0) {
				foreach (object editValueItem in list) {
					int index = ItemsProvider.IndexOfValue(editValueItem, EditStrategy.CurrentDataViewHandle);
					if (index > -1)
						return ItemsProvider.GetItemByControllerIndex(index, EditStrategy.CurrentDataViewHandle);
				}
			}
			return ItemsProvider.GetItem(editValue, EditStrategy.CurrentDataViewHandle);
		}
		public object GetEditValueFromSelectedIndex(object index) {
			return ItemsProvider.GetValueByIndex((int)index, EditStrategy.CurrentDataViewHandle);
		}
		public object GetEditValueFromSelectedItem(object selectedItem) {
			object item = FilterSelectedItem(selectedItem);
			return ItemsProvider.GetValueFromItem(item, EditStrategy.CurrentDataViewHandle);
		}
		public void UpdateSelectedItems(object selectedItems) {
			Editor.SelectedItems.Clear();
			IList listSelectedItems = selectedItems as IList;
			if (listSelectedItems != null) {
				IEnumerable<object> enumItems = FilterSelectedItems(listSelectedItems.Cast<object>());
				foreach (object selectedItem in enumItems)
					Editor.SelectedItems.Add(selectedItem);
			}
			else {
				object selectedItem = FilterSelectedItem(selectedItems);
				if (selectedItem != null)
					Editor.SelectedItems.Add(selectedItem);
			}
		}
		public object GetEditValueFromSelectedItems(object selectedItems) {
			IList listSelectedItems = selectedItems as IList;
			if (EditStrategy.IsSingleSelection && !EditStrategy.IsTokenMode)
				return ((listSelectedItems == null) || (listSelectedItems.Count < 1)) ? (object)null : GetEditValueFromSelectedItem(listSelectedItems[0]);
			List<object> values = new List<object>();
			if (listSelectedItems == null) {
				if (selectedItems != null && ItemsProvider.IndexOfValue(selectedItems, EditStrategy.CurrentDataViewHandle) != -1)
					values.Add(selectedItems);
			}
			else {
				for (int i = 0; i < listSelectedItems.Count; i++) {
					if (listSelectedItems[i] == null)
						continue;
					int index = ItemsProvider.GetIndexByItem(listSelectedItems[i], EditStrategy.CurrentDataViewHandle);
					if (index > -1) {
						object value = ItemsProvider.GetValueFromItem(listSelectedItems[i], EditStrategy.CurrentDataViewHandle);
						values.Add(value);
					}
				}
			}
			values = values.Distinct().ToList();
			return values.Any() ? values : null;
		}
		public object GetSelectedItemsFromEditValue(object editValue) {
			IList listEditValue = editValue as IList;
			if ((EditStrategy.IsSingleSelection && !EditStrategy.IsTokenMode) || listEditValue == null)
				return GetSelectedItemFromEditValue(editValue);
			IList listSelectedItems = new List<object>();
			foreach (object value in listEditValue) {
				object item = ItemsProvider.GetItem(value, EditStrategy.CurrentDataViewHandle);
				if (item != null)
					listSelectedItems.Add(item);
			}
			return listSelectedItems;
		}
		public IList GetPreviousSelectedItems(NotifyCollectionChangedEventArgs e) {
			object items = GetSelectedItemsFromEditValue(EditStrategy.EditValue);
			return items is IList ? (IList)items : new List<object>() {items};
		}
		public object GetEditValueFromBaseValue(object baseValue) {
			if (EditStrategy.IsSingleSelection && !EditStrategy.IsTokenMode)
				return ContainsValue(baseValue) ? baseValue : Editor.NullValue;
			if (baseValue == null)
				return null;
			List<object> result = new List<object>();
			IList listEditValue = baseValue as IList;
			if (listEditValue == null && ContainsValue(baseValue))
				result.Add(baseValue);
			else if (listEditValue != null) {
				foreach (object baseValueItem in listEditValue) {
					if (ContainsValue(baseValueItem))
						result.Add(baseValueItem);
				}
			}
			return result;
		}
		bool ContainsValue(object value) {
			return !IsInLookUpMode || ItemsProvider.IndexOfValue(value, EditStrategy.CurrentDataViewHandle) > -1;
		}
		public void SyncICollectionView(object editValue) {
			if (!Editor.IsSynchronizedWithCurrentItem)
				return;
			var icollectionView = (IItemsProviderCollectionViewSupport)ItemsProvider;
			object currentItem = ItemsProvider.GetItem(editValue, EditStrategy.CurrentDataViewHandle);
			if (currentItem == null && ItemsProvider.GetIndexByItem(null, EditStrategy.CurrentDataViewHandle) < 0)
				return;
			SyncWithICollectionViewLocker.DoLockedActionIfNotLocked(() => icollectionView.SetCurrentItem(currentItem));
		}
		public void SyncWithICollectionView(object currentItem) {
			SyncWithICollectionViewLocker.DoIfNotLocked(() => {
				EditStrategy.EditValue = GetEditValueFromSelectedItem(currentItem);
			});
		}
		public void SyncWithICollectionView() {
			SyncWithICollectionViewLocker.DoIfNotLocked(() => {
				var itemsProvider = (IItemsProviderCollectionViewSupport)ItemsProvider;
				itemsProvider.SyncWithCurrentItem();
			});
		}
		public IEnumerable<object> FilterSelectedItems(IEnumerable<object> items) {
			return CustomItem.FilterCustomItems(items);
		}
		public object FilterSelectedItem(object item) {
			return CustomItem.FilterCustomItem(item);
		}
		public object GetEditValueFromItems() {
			ListItemCollection items = Editor.Items;
			if (items.Count == 0)
				return null;
			var selectedItems = items.Cast<object>().Where(element => element is ListBoxEditItem && ((ListBoxEditItem)element).IsSelected).ToList();
			return GetEditValueFromSelectedItems(selectedItems);
		}
		public bool ShouldSyncWithItems { get { return Editor.Items.Count > 0; } }
		public object GetCurrentSelectedItem(ValueContainerService valueContainer) {
			object editValue;
			if (EditStrategy.ProvideEditValue(valueContainer.EditValue, out editValue, UpdateEditorSource.TextInput)) {
				if (EditStrategy.IsTokenMode) {
					int index = EditStrategy.EditableTokenIndex;
					var listEditValue = editValue as IList;
					if (listEditValue == null)
						return ItemsProvider.GetItem(editValue, EditStrategy.CurrentDataViewHandle);
					if (index < listEditValue.Count && index > -1)
						return ItemsProvider.GetItem(listEditValue[index], EditStrategy.TokenDataViewHandle);
					return null;
				}
				return ItemsProvider.GetItem(editValue, EditStrategy.CurrentDataViewHandle);
			}
			return null;
		}
		public IEnumerable GetCurrentSelectedItems(ValueContainerService valueContainer) {
			object editValue;
			List<object> result = new List<object>();
			if (EditStrategy.ProvideEditValue(valueContainer.EditValue, out editValue, UpdateEditorSource.TextInput)) {
				return (editValue as IEnumerable).Return(x => x.Cast<object>().Select(item => ItemsProvider.GetItem(item, EditStrategy.CurrentDataViewHandle)).ToList(), () => result);
			}
			return result;
		}
		public bool OptimizedSyncWithValueOnDataChanged(ItemsProviderDataChangedEventArgs e, object editValue, Action syncWithValue) {
#if !SL
			if (EditStrategy.IsInProcessNewValue)
				return true;
			if (IsOptimizedSyncWithValueOnDataChanged(e.ListChangedType)) {
				if (ShouldSyncWithValueOnDataChanged(e, editValue))
					syncWithValue();
				if (e.ListChangedType == ListChangedType.ItemChanged && e.Descriptor.If(x => x.Name == Editor.DisplayMember).ReturnSuccess())
					((EditStrategyBase)EditStrategy).UpdateDisplayText();
				return true;
			}
#endif
			return false;
		}
		bool ShouldSyncWithValueOnDataChanged(ItemsProviderDataChangedEventArgs e, object editValue) {
			if (!EditStrategy.IsSingleSelection && !EditStrategy.IsTokenMode)
				return true;
			int index = GetValueIndex(ref editValue);
			if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted)
				return index == -1 || e.NewIndex == index;
			if (e.ListChangedType == ListChangedType.ItemChanged) {
				if (e.NewIndex == index)
					return editValue != EditStrategy.ItemsProvider.GetValueByIndex(index, EditStrategy.CurrentDataViewHandle);
				return true;
			}
			return true;
		}
		bool IsOptimizedSyncWithValueOnDataChanged(ListChangedType changeType) {
			return changeType == ListChangedType.ItemAdded || changeType == ListChangedType.ItemDeleted || changeType == ListChangedType.ItemChanged;
		}
		int GetValueIndex(ref object editValue) {
			object resultValue;
			if (EditStrategy.ProvideEditValue(editValue, out resultValue, UpdateEditorSource.ValueChanging))
				editValue = resultValue;
			return EditStrategy.ItemsProvider.IndexOfValue(editValue, EditStrategy.CurrentDataViewHandle);
		}
		public object GetEditValueFromSelectedItemsSource(object baseValue) {
			return GetEditValueFromSelectedItems(baseValue);
		}
		public object GetSelectedItemsSourceFromEditValue(object baseValue) {
			return GetSelectedItemsFromEditValue(baseValue);
		}
		public object GetPreviousSelectedItemsSource(ListChangedEventArgs e) {
			return null;
		}
	}
}
