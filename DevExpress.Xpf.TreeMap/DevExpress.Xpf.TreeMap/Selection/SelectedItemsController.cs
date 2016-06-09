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
using System.Collections.Specialized;
using System.Windows.Input;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	public enum SelectionMode { None, Single, Multiple, Extended }
	public delegate void TreeMapSelectionChangedEventHandler(object sender, TreeMapSelectionChangedEventArgs e);
	[NonCategorized]
	public class TreeMapSelectionChangedEventArgs : EventArgs {
		public TreeMapSelectionChangedEventArgs(List<object> selection) {
			Selection = new List<object>();
			Selection.AddRange(selection);
		}
		public List<object> Selection { get; private set; }
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public enum SelectionAction { None, Clear, Add, Remove, ReplaceAll }
	public static class SelectionHelper {
		public static SelectionAction CalculateSelectionAction(SelectionMode selectionMode, ModifierKeys keyModifiers, bool isItemSelected) {
			switch (selectionMode) {
				case SelectionMode.Single:
					return SelectionAction.ReplaceAll;
				case SelectionMode.Multiple:
					return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
				case SelectionMode.Extended: {
						if (keyModifiers == ModifierKeys.Shift)
							return SelectionAction.Add;
						if (keyModifiers == ModifierKeys.Control)
							return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
						return SelectionAction.ReplaceAll;
					}
			}
			return SelectionAction.None;
		}
	}
	public class SelectedItemsController {
		TreeMapControl treeMap;
		List<TreeMapItem> selectedTreeMapItems = new List<TreeMapItem>();
		IList currentList = null;
		Locker updateSelectedItemLocker = new Locker();
		Locker updateSelectedItemsLocker = new Locker();
		public object SelectedItem { get { return treeMap.SelectedItem; } set { treeMap.SelectedItem = value; } }
		public IList SelectedItems { get { return treeMap.SelectedItems; } set { treeMap.SelectedItems = value; } }
		protected bool IsEventsSuspended { get { return updateSelectedItemsLocker.IsLocked; } }
		public SelectedItemsController(TreeMapControl treeMap) {
			this.treeMap = treeMap;
		}
		void SafeSetSelectedItems(IList newList) {
			updateSelectedItemLocker.Lock();
			try {
				SelectedItems = newList;
			}
			finally {
				updateSelectedItemLocker.Unlock();
			}
		}
		void UpdateSelectedItemByList(IList list) {
			SelectedItem = list != null && list.Count > 0 ? list[0] : default(object);
		}
		IList CopySelectedItems() {
			IList collection = CreateNewSelectedItems();
			if (SelectedItems != null)
				foreach (object item in SelectedItems)
					collection.Add(item);
			return collection;
		}
		IList CreateNewSelectedItems() {
			return new ObservableCollection<object>();
		}
		void EnsureSelectedItems() {
			if (SelectedItems == null)
				SelectedItems = CreateNewSelectedItems();
		}
		void AddSelectedItem(object item) {
			EnsureSelectedItems();
			IList newList = CopySelectedItems();
			IList innerList = item as IList;
			if (innerList != null)
				foreach (object innerItem in innerList) {
					if (!newList.Contains(innerItem))
						newList.Add(innerItem);
				}
			else
				if (!newList.Contains(item))
					newList.Add(item);
			SafeSetSelectedItems(newList);
		}
		void RemoveSelectedItem(object item) {
			EnsureSelectedItems();
			IList newList = CopySelectedItems();
			IList innerList = item as IList;
			if (innerList != null) {
				foreach (object innerItem in innerList) {
					newList.Remove(innerItem);
				}
			}
			else {
				newList.Remove(item);
			}
			SafeSetSelectedItems(newList);
		}
		void SuspendEvents() {
			updateSelectedItemsLocker.Lock();
		}
		void ResumeEvents() {
			updateSelectedItemsLocker.Unlock();
		}
		void SetSelectedItem(object item) {
			EnsureSelectedItems();
			if (updateSelectedItemsLocker.IsLocked)
				return;
			IList newList = CreateNewSelectedItems();
			IList innerList = item as IList;
			if (innerList == null)
				newList.Add(item);
			SafeSetSelectedItems(newList);
		}
		bool ContainsInSelectedItems(object source) {
			IList innerList = source as IList;
			if (innerList != null) {
				foreach (var item in innerList) 
					if (SelectedItems.Contains(item))
						return true;
				return false;
			}
			else
				return SelectedItems.Contains(source);
		}
		void SetCurrentList(IList list) {
			UnsubscribeListEvents(currentList);
			currentList = list;
			SubscribeListEvents(currentList);
		}
		void SubscribeListEvents(object list) {
			if (list != null) {
				INotifyCollectionChanged coll = list as INotifyCollectionChanged;
				if (coll != null)
					coll.CollectionChanged += OnCollectionChanged;
			}
		}
		void UnsubscribeListEvents(object list) {
			if (list != null) {
				INotifyCollectionChanged coll = list as INotifyCollectionChanged;
				if (coll != null)
					coll.CollectionChanged -= OnCollectionChanged;
			}
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateActualItemsSelection(treeMap.ActualItems);
		}
		object GetSelectedItem(ITreeMapItem item) {
			if (item.Source is TreeMapItem && ((TreeMapItem)item).IsGroup) {
				selectedTreeMapItems.Clear();
				FillSelectedTreeMapItems(((TreeMapItem)item).Children);
				return selectedTreeMapItems;
			}
			else
				return item.Source;
		}
		void FillSelectedTreeMapItems(TreeMapItemCollection children) {
			foreach (TreeMapItem item in children) {
				if (item.IsGroup)
					FillSelectedTreeMapItems(item.Children);
				else
					selectedTreeMapItems.Add(item);
			}
		}
		internal void UpdateActualItemsSelection(TreeMapItemCollection itemCollection) {
			if (itemCollection != null) {
				foreach (TreeMapItem item in itemCollection) {
					if (item != null && !item.IsGroup) {
						object source = ((ITreeMapItem)item).Source;
						item.IsSelected = (SelectedItems != null && source != null ? ContainsInSelectedItems(source) : false);
					}
					if (item.Children.Count > 0)
						UpdateActualItemsSelection(item.Children);
				}
			}
		}
		public void UpdateItemSelection(SelectionMode selectionMode, ModifierKeys keyModifiers, ITreeMapItem item) {
			object selectedItem = GetSelectedItem(item);
			SelectionAction action = SelectionHelper.CalculateSelectionAction(selectionMode, keyModifiers, SelectedItems != null && ContainsInSelectedItems(selectedItem));
			switch (action) {
				case SelectionAction.None:
					return;
				case SelectionAction.ReplaceAll:
					ClearSelectedItems();
					SetSelectedItem(selectedItem);
					break;
				case SelectionAction.Add:
					AddSelectedItem(selectedItem);
					break;
				case SelectionAction.Remove:
					RemoveSelectedItem(selectedItem);
					break;
			}
		}
		public void ClearSelectedItems() {
			if (SelectedItems == null)
				return;
			IList newList = CreateNewSelectedItems();
			SafeSetSelectedItems(newList);
		}
		public void OnUpdateSelectedItem(object item) {
			if (updateSelectedItemLocker.IsLocked)
				return;
			if (item == null) {
				SelectedItems = null;
			}
			else {
				SetSelectedItem(item);
			}
			UpdateActualItemsSelection(treeMap.ActualItems);
		}
		public void OnUpdateSelectedItems(IList list) {
			SuspendEvents();
			try {
				UpdateSelectedItemByList(list);
				SetCurrentList(list);
			}
			finally {
				ResumeEvents();
				treeMap.RaiseSelectionChanged();
			}
			UpdateActualItemsSelection(treeMap.ActualItems);
		}
	}
}
